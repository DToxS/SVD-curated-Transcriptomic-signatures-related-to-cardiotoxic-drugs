#stopCluster(parallel_clusters)
rm(list = ls());
#######################################################################################################################
## Generate tasks - BEGIN
get_eigenassays_per_dataset = FALSE
delete_task_reports = TRUE
add_inFrontOf_progress_report_fileName = "SVD_13"
source('SVD_global_parameter.R')
library(dendextend)

minimum_fractions_of_max_f1score = c(1)
ea_correlation_parameters = c("Ttest_pvalue")
entityClasses = c("Drug")
reference_valueTypes = c("Coefficient_of_eigenassay")

f1_score_weight = -1;#-1 indicates outlier based f1soceweight selection, 0.95

tasks = c()
task_no = 0;

indexGlobalTasks=1
for (indexGlobalTasks in 1:length(globally_assigned_tasks[,1]))
{#Begin - indexGlobalTasks
  current_global_task_line = globally_assigned_tasks[indexGlobalTasks,]
  for (index_ea_correlation_parameter in 1:length(ea_correlation_parameters))
  {#Begin
    ea_correlation_parameter = ea_correlation_parameters[index_ea_correlation_parameter]
    for (indexMinOfMaxF1Cutoff in 1:length(minimum_fractions_of_max_f1score))
    {#Begin
       minimum_fraction_of_max_f1score = minimum_fractions_of_max_f1score[indexMinOfMaxF1Cutoff]
       for (indexRV in 1:length(reference_valueTypes))
       {#Begin
          reference_valueType = reference_valueTypes[indexRV]
          for (indexE in 1:length(entityClasses))
          {#Begin
             entityClass= entityClasses[indexE]
             task_no = task_no + 1
             current_task_line = current_global_task_line
             current_task_line$EntityClass = entityClass
             current_task_line$Task_no = task_no
             current_task_line$Reference_valueType = reference_valueType
             current_task_line$Minimum_fraction_of_max_f1score = minimum_fraction_of_max_f1score
             current_task_line$Ea_correlation_parameter = ea_correlation_parameter
             current_task_line$F1_score_weight = f1_score_weight
             if (length(tasks)==0) { tasks = current_task_line}
             else { tasks = rbind(tasks,current_task_line)}
          }#End
       }#End
    }#End
  }#End
}#End

tasks$AddOrAnalyze_coCulture_data = FALSE
cc_tasks = tasks
cc_tasks$AddOrAnalyze_coCulture_data = TRUE
tasks = rbind(tasks,cc_tasks)

## Generate tasks - End
#######################################################################################################################


length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

#core_tasks = tasks; indexCore=1;indexCoreTask=2



parallel_clusters = makeCluster(cores_count)
clusterEvalQ(parallel_clusters, {
  library("ClassDiscovery")
  library("colorspace")
  library("dendextend")
  library("ape")
  library("fields")
  library("colormap")
  library("beeswarm")
  library("gplots")
  library("grid")
  library("gridExtra")
  library("ggplot2")
  library("genefilter")
  library("doParallel")
}
);

for (indexCore in 1:cores_count)
{#Begin
  startIndex = min(floor((indexCore-1) * tasks_per_core+1),length_tasks);
  endIndex = min(floor(indexCore * tasks_per_core),length_tasks)
  core_tasks = tasks[startIndex:endIndex,]
  clusterCall(parallel_clusters[indexCore], function(d) {assign('core_tasks', d, pos=.GlobalEnv)}, core_tasks)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('indexCore', d, pos=.GlobalEnv)}, indexCore)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('lincs_results_directory', d, pos=.GlobalEnv)}, lincs_results_directory)
}#End

cluster_generation_correct = TRUE;

combined_core_tasks <- do.call('rbind', clusterEvalQ(parallel_clusters, core_tasks))
if (length(combined_core_tasks[,1])!= length_tasks) { cluster_generation_correct = FALSE }
if (cluster_generation_correct)
{#Begin
  for (indexC in 1:length(combined_core_tasks[,1]))
  {#Begin
    if (combined_core_tasks$Dataset[indexC] != tasks$Dataset[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Correlation_method[indexC] != tasks$Correlation_method[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Preprocess_data[indexC] != tasks$Preprocess_data[indexC]) { cluster_generation_correct=FALSE;}
  }#End
}#End

if (!cluster_generation_correct)
{#Begin
  stopCluster(parallel_clusters) 
}#End


clusterEvalQ(parallel_clusters,
{#Begin - Parallel clusters
    
length_core_tasks = length(core_tasks[,1])  


for (indexCoreTask in 1:length_core_tasks)
{#Begin - indexCoreTask
   
current_task_line = core_tasks[indexCoreTask,]  
dataset = current_task_line$Dataset
dataset_coCulture =  current_task_line$Dataset_coCulture
ea_correlation_parameter = current_task_line$Ea_correlation_parameter
preprocess_data = current_task_line$Preprocess_data
reference_valueType = current_task_line$Reference_valueType
f1_score_weight = current_task_line$F1_score_weight
addOrAnalyze_coCulture_data = current_task_line$AddOrAnalyze_coCulture_data


## Set global parameter - BEGIN
source('SVD_coreTaskSpecific_parameter.R')
## Set global parameter - END

source('SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites.R')
summary = read.table(file=complete_degs_summary_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')

if (addOrAnalyze_coCulture_data)
{#Begin - Read, prepare and add coCulture DEGs and summary
  source('SVD_readPrepare_and_add_coCulture_degMatrix.R')
  summary_coCulture = read.table(file=complete_degs_summary_coCulture_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
  summary = rbind(summary,summary_coCulture)
}#End - Read, prepare and add coCulture DEGs and summary

if (file.exists(complete_collapsed_entitySpecific_cluster_validation_f1_fileName))
{#Begin - Read and prepare collapsed_real_validations and eigenassay_statistics
   collapsed_real_validations = read.table(file=complete_collapsed_entitySpecific_cluster_validation_f1_fileName,stringsAsFactors = FALSE,header = TRUE,sep='\t')
   collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Dataset == current_task_line$Dataset,]
   collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Decomposition_method == current_task_line$Decomposition_method,]
   collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Correlation_method == current_task_line$Correlation_method,]
   collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Preprocess_data == current_task_line$Preprocess_data,]
   collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$EA_correlation_parameter == current_task_line$Ea_correlation_parameter,]
   collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Reference_valueType == current_task_line$Reference_valueType,]
   collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$EntityClass==current_task_line$EntityClass,]
   if (f1_score_weight!=-1)
   { collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$F1_score_weight==f1_score_weight,] }
   if (f1_score_weight==-1)
   { collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Final_selection_for_outlier_based_decomposition==outlier_finalSelection_yes_label,] }
   collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Final_selection %in% c("Final selection in full data","Final selection for sample in full data"),]

   eigenassay_statistics = read.csv(file = complete_eigenassay_correlation_statistics_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
   eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Dataset == current_task_line$Dataset,]
   eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Decomposition_method == current_task_line$Decomposition_method,]
   eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Correlation_method == current_task_line$Correlation_method,]
   eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Preprocess_data == current_task_line$Preprocess_data,]
   eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Reference_valueType == current_task_line$Reference_valueType,]
   eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$EntityClass == current_task_line$EntityClass,]
}#End - Read and prepare collapsed_real_validations and eigenassay_statistics

if (exists("collapsed_real_validations"))
{#Begin - Generate drug-specific subspace data and combine into one matrix: Combined_subspace_data
   Combined_subspace_data = c()
   entities = unique(collapsed_real_validations$Entity)
   indexE=27
   for (indexE in 1:length(entities))
   {#Begin
      current_entity = entities[indexE]
      indexCurrentEntityCollapsed = which(collapsed_real_validations$Entity==current_entity)
      current_collapsed_real_validation = collapsed_real_validations[indexCurrentEntityCollapsed,]
      eigenassays_count = unique(current_collapsed_real_validation$Eigenassays_count)
      if (length(eigenassays_count)!=1) { stop("(length(eigenassays_count)!=1)") }
      indexOutlier = which(current_collapsed_real_validation$Is_outlier==outlier_finalSelection_yes_label)
      outlierCellLine = current_collapsed_real_validation$Final_selection_for_sample[indexOutlier]
      
      currentEntity_eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Entity==current_entity,]
      currentEntity_eigenassay_statistics = currentEntity_eigenassay_statistics[order(currentEntity_eigenassay_statistics[[ea_correlation_parameter]]),]
      current_eigenassays = currentEntity_eigenassay_statistics$Eigenassay[1:eigenassays_count]
      if (length(current_eigenassays)!=length(unique(current_eigenassays))) { stop("length(current_eigenassays)!=length(unique(current_eigenassays)") }
      #current_cum_eigensexpression_fraction = sum(currentEntity_eigenassay_statistics$Eigenexpression_fraction[1:eigenassays_count])
      source('SVD_generate_reduced_data_and_setColnames_to_fullDataColnames.R')
      
      {#Begin - Add to all reduced dataset
        index_current_entity_cols = grep(current_entity,colnames(Data_current))
        add_data_current = Data_current[,index_current_entity_cols]
        current_add_colnames = colnames(add_data_current)
        if (length(outlierCellLine)>0)
        {#Begin
           indexOutlier = grep(outlierCellLine,current_add_colnames)
           indexNoOutlier = 1:length(current_add_colnames)
           indexNoOutlier = indexNoOutlier[!indexNoOutlier %in% indexOutlier]
           current_add_colnames[indexOutlier] = paste(current_add_colnames[indexOutlier],".Outlier",sep='')
           current_add_colnames[indexNoOutlier] = paste(current_add_colnames[indexNoOutlier],".Regular",sep='')
        }#End
        if (length(outlierCellLine)==0)
        {#Begin
          current_add_colnames = paste(current_add_colnames,".Regular")
        }#End
        colnames(add_data_current) = current_add_colnames
        
        if (length(Combined_subspace_data)==0) { Combined_subspace_data = add_data_current }
        else { Combined_subspace_data = cbind(Combined_subspace_data,add_data_current) }
      }#End - Add to all reduced dataset
   }#End
}#End - Generate drug-specific subspace data and combine into one matrix: Combined_subspace_data

colnames(Data) = paste(colnames(Data),".Regular",sep='')

Get_all_color_and_label_vectors = function(current_colnames,current_task_line,summary)
{#Begin - Function: Get_all_color_and_label_vectors
  ## Generate colors - BEGIN
  source('SVD_colors.R')
  ## Generate colors - END
  current_drug_colors = replicate(length(current_colnames),"gray60")
  current_drug_labels = replicate(length(current_colnames),"error")

  current_drugClass_colors = replicate(length(current_colnames),"gray60")
  current_drugClass_labels = replicate(length(current_colnames),"error")

  current_cellline_colors = replicate(length(current_colnames),"gray60")
  current_cellline_labels = replicate(length(current_colnames),"error")

  current_sigDEGs_colors = replicate(length(current_colnames),"gray60")
  current_sigDEGs_labels = replicate(length(current_colnames),"error")

  max_sigDEGs = max(summary$Significant_degs_based_on_FDR_count)
  min_sigDEGs = min(summary$Significant_degs_based_on_FDR_count)
  colfunc <- colorRampPalette(c("dodgerblue3", "orange"))
  #colors_for_color_map = rainbow(1000, start = rgb2hsv(col2rgb('blue'))[1], end = rgb2hsv(col2rgb('orange'))[1])
  colors_for_color_map = colfunc(1000)

  indexColNames=156
  for (indexColNames in 1:length(current_colnames))
  {#Begin
    current_colname = current_colnames[indexColNames]
    splitStrings = strsplit(current_colname,"[.]")[[1]]
    drug = splitStrings[3]
    indexCurrentDrug = which(summary$Treatment==drug)
    drugFullName = unique(summary$Treatment_full_name[indexCurrentDrug])

    cellline = splitStrings[2]
    outlier = splitStrings[5]
    drugClass = drug_drugClass[[drug]]
    
    summary$Cell_line = gsub("Cell_line.","",summary$Cell_line)
    
    indexCurrentDrug_in_summary = which(summary$Treatment==drug)
    indexCurrentCellline_in_summary = which(summary$Cell_line==cellline)
    indexCurrentDrugCellline_in_summary = indexCurrentDrug_in_summary[indexCurrentDrug_in_summary %in% indexCurrentCellline_in_summary]
    if (length(indexCurrentDrugCellline_in_summary)!=1) { rm(summary)}
    
    indexColor = which(drugs_for_highlight_colors==drug) 
    current_drug_colors[indexColNames] = drug_highlight_colors[indexColor]
    
    if (outlier=="Regular"){ current_drug_labels[indexColNames] = full_drugNames[[drug]] }
    if (outlier=="Outlier"){ current_drug_labels[indexColNames] = paste("Outlier - ",full_drugNames[[drug]],sep='') }
    
    indexColor = which(cellLines_for_highlight_colors==cellline) 
    current_cellline_colors[indexColNames] = cellLine_highlight_colors[indexColor]
    cellline_splitStrings = strsplit(cellline,"_")[[1]]
    current_cellline_labels[indexColNames] = cellline_splitStrings[1]
    
    if (length(cellline_splitStrings)==3)
    {#Begin 
       current_cellline_labels[indexColNames] = paste(cellline_splitStrings[3]," - ",current_cellline_labels[indexColNames],sep='')
       current_drug_labels[indexColNames] = paste(cellline_splitStrings[3]," - ",current_drug_labels[indexColNames],sep='')
    }#End
    
    current_drugClass_labels[indexColNames] = drugClass
    current_drugClass_colors[indexColNames] = drugClass_colors[[drugClass]]
    
    current_significant_degs_count = summary$Significant_degs_based_on_FDR_count[indexCurrentDrugCellline_in_summary]
    current_sigDEGs_labels[indexColNames] = current_significant_degs_count
    current_colorIndex = round((current_significant_degs_count - min_sigDEGs) * (1 /(max_sigDEGs-min_sigDEGs)) * length(colors_for_color_map))
    if (current_colorIndex==0) { current_colorIndex = 1}
    if (current_colorIndex>length(colors_for_color_map)) { current_colorIndex = length(colors_for_color_map)}
    current_sigDEGs_colors[indexColNames] = colors_for_color_map[current_colorIndex]
  }#End
  
  names(current_drug_colors) = current_drug_labels
  names(current_cellline_colors) = current_cellline_labels
  names(current_drugClass_colors) = current_drugClass_labels
  names(current_sigDEGs_colors) = current_sigDEGs_labels
  labels_and_colors_list = list()
  labels_and_colors_list[["Drugs"]] = current_drug_colors
  labels_and_colors_list[["Cell_lines"]] = current_cellline_colors
  labels_and_colors_list[["DrugClasses"]] = current_drugClass_colors
  labels_and_colors_list[["SigDEGs"]] = current_sigDEGs_colors
  return (labels_and_colors_list)
}#End - Function: Get_all_color_and_label_vectors

if (dim(Data)[1]!=dim(Combined_subspace_data)[1]) { rm(Combined_subspace_data) }
if (dim(Data)[2]!=dim(Combined_subspace_data)[2]) { rm(Combined_subspace_data) }

used_datas_for_clustering = list()
used_datas_for_clustering[["FullData"]] = Data
if (exists("Combined_subspace_data")) { used_datas_for_clustering[["Combined_subspace_data"]] = Combined_subspace_data}

indexUsed=2
for (indexUsed in 1:length(used_datas_for_clustering))
{#Begin
   current_data_name = names(used_datas_for_clustering)[indexUsed]
   current_data = used_datas_for_clustering[[current_data_name]]
   current_colnames = colnames(current_data)
   
   dist.function = function(x) as.dist((1-cor((x),method=correlation_method))/2)
   hclust.function = function(x) hclust(x,method="average")

   col_dist = dist.function(current_data)
   col_hc = hclust.function(col_dist)
   col_dend = as.dendrogram(col_hc)

   indexOriginalData = order.dendrogram(col_dend);
   col_labels_in_dendrogram = current_colnames[indexOriginalData]
   current_data_reordered = current_data[,indexOriginalData]

   indexBackIntoOriginalData = replicate(length(indexOriginalData),0)
   indexBackIntoOriginalData[indexOriginalData] = 1:length(indexOriginalData)
   
   labels_and_colors_list = Get_all_color_and_label_vectors(col_labels_in_dendrogram,current_task_line,summary)
   labelGroups = names(labels_and_colors_list)

   dendrogram_png_height = 16000
   dendrogram_png_bootom_par = 80
   
   generate_regular_label_dendrograms=TRUE
   if (generate_regular_label_dendrograms)
   {#Begin - generate_regular_label_dendrograms
     indexLabelGroups=1
     regular_labelGroups = labelGroups
     regular_labelGroups = regular_labelGroups[regular_labelGroups %in% labelGroups]
     for (indexLabelGroups in 1:length(regular_labelGroups))
     {#Begin
       labelGroup = regular_labelGroups[indexLabelGroups]
       current_label_group = labels_and_colors_list[[labelGroup]]
       Colors = current_label_group
       
       col_dend = color_labels(col_dend, col = Colors)
       col_dend = set_labels(col_dend,names(current_label_group))
       col_dend = set(col_dend, "labels_cex", 0.8)
       col_dend = set(col_dend,"branches_lwd", 0.7)

       dendrogram_png_height = 4400
       dendrogram_png_bootom_par = 25
              
       complete_fullData_colDend_fileName = paste(heatmap_directory,current_data_name,"Clustered_",colDend_baseFileName,"_",labelGroup,".png",sep='')
       png(complete_fullData_colDend_fileName, width = 20000, height = dendrogram_png_height, res = 2000);
       par(cex = 0.3, font = 2.3);
       par(mar = c(dendrogram_png_bootom_par, 0, 2, 0) + 0.1)
       plot(col_dend, horiz = FALSE);
       plot(col_dend, horiz = FALSE,main=labelGroup,cex.main=3);
       #colored_bars(Colors[indexBackIntoOriginalData], col_dend, rowLabels = labelGroup, sort_by_labels_order = TRUE)
       dev.off();
     }#End
   }#End - generate_regular_label_dendrograms
   
   indexKeep = which(labelGroups %in% c("Drugs"))
   labelGroups = rev(labelGroups[indexKeep])
   labelGroupsDescription = "drugs"
   
   generate_regular_label_dendrograms=TRUE
   if (generate_regular_label_dendrograms)
   {#Begin - generate_stacked_dendrograms
     indexLabelGroups=1
     color_matrix = c()
     color_label_groups = c()
     for (indexLabelGroups in 1:length(labelGroups))
     {#Begin
       labelGroup = labelGroups[indexLabelGroups]
       current_label_group = labels_and_colors_list[[labelGroup]]
       Colors = current_label_group
       if (length(color_label_groups)>0)
       {#Begin
         color_label_groups = c(color_label_groups,labelGroup)
         color_matrix = cbind(color_matrix,Colors[indexBackIntoOriginalData])
       }#End
       else
       {#Begin
         color_label_groups = labelGroup
         color_matrix = Colors[indexBackIntoOriginalData]
       }#End
     }#End
     
     dendrogram_png_height = 5000
     dendrogram_png_bootom_par = 30
     
     
     col_dend = set_labels(col_dend,replicate(length(indexBackIntoOriginalData),""))
     col_dend = set(col_dend,"branches_lwd", 0.7)
     
     color_label_groups = gsub("Cell_lines","Cell lines",color_label_groups)
     color_label_groups = gsub("SigDEGs","# DEGs",color_label_groups)
     color_label_groups = factor(color_label_groups,levels=rev(color_label_groups))
     
     complete_fullData_colDend_fileName = paste(heatmap_directory,current_data_name,"Clustered_",colDend_baseFileName,"_",labelGroupsDescription,"_asBars.png",sep='')
     png(complete_fullData_colDend_fileName, width = 20000, height = dendrogram_png_height, res = 2000);
     par(cex = 0.3, font = 2.3);
     par(mar = c(dendrogram_png_bootom_par, 2, 0, 0) + 0.1)
     #par(mar = c(15, 0, 2, 0) + 0.1)
     plot(col_dend, horiz = FALSE);
     plot(col_dend, horiz = FALSE,cex.main=3);
     colored_bars(color_matrix, col_dend, rowLabels = color_label_groups, sort_by_labels_order = TRUE,lty="none",y_scale = 0.8,cex.rowLabels = 2)
     dev.off();
   }#End - generate_stacked_dendrograms
   
         
   {#Begin - Define heatmap color map
     Ratio_min_max = abs(min(current_data)) / max(current_data)
     warm_color_count = 1000;
     cool_color_count = warm_color_count * Ratio_min_max
     
     cool = rainbow(cool_color_count, start = rgb2hsv(col2rgb('darkslategray1'))[1], end = rgb2hsv(col2rgb('midnightblue'))[1])
     warm = rainbow(warm_color_count, start = rgb2hsv(col2rgb('darkred'))[1], end = rgb2hsv(col2rgb('orange'))[1])
     cols = c(rev(cool), replicate(30, '#ffffff'), rev(warm))
     Color_map <- colorRampPalette(cols)(255)
   }#End - Define heatmap color map
   
   png_width = dim(Data)[2] * 25;
   png_height = dim(Data)[1] * 0.5
   
   complete_heatmap_fileName = paste(heatmap_directory,current_data_name,"Clustered_",reference_valueType,"_",ea_correlation_parameter,"_",heatmap_baseFileName,".png",sep='')
   png(filename = complete_heatmap_fileName, width = png_width, height = png_height, res = 100)
   par(mar = c(7, 4, 2, 5) + 0.1)
   current_data_reordered = t(current_data_reordered)
   image(current_data_reordered[length(current_data_reordered[, 1]):1,], axes = FALSE, col = Color_map);
   image.plot(current_data_reordered, col = Color_map, axes = FALSE);
   box(col = "black", lty = "solid")
   dev.off()

   complete_heatmap_fileName = paste(heatmap_directory,current_data_name,"Clustered_",reference_valueType,"_",ea_correlation_parameter,"_",heatmap_baseFileName,"_legend.png",sep='')
   png(filename = complete_heatmap_fileName, width = png_width, height = 1000, res = 100)
   par(mar=c(7,4,2,5)+0.1)
   image.plot(t(current_data_reordered),legend.only=TRUE, col= Color_map, new=TRUE);
   dev.off()
}#End

}#End - indexCoreTask
}#End - Parallel Clusters - generate all heatmaps - SVD_13
)

{#Begin - Close parallel clusters
  invisible(gc())
  parallel::stopCluster(parallel_clusters)
  invisible(gc())
  rm(parallel_clusters)
}#End - Close parallel clusters

