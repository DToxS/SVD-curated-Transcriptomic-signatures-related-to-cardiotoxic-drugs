#stopCluster(parallel_clusters)
rm(list = ls());
#######################################################################################################################
## Generate tasks - BEGIN
indexCore = -1
get_eigenassays_per_dataset = FALSE
add_inFrontOf_progress_report_fileName = "SVD_12"
delete_task_reports = TRUE

source('SVD_global_parameter.R')

minimum_fractions_of_max_f1score = c(0,1)
ea_correlation_parameters = c("Ttest_pvalue")
entityClasses = c("Drug")
reference_valueTypes = c("Coefficient_of_eigenassay")

all_tasks = c()
task_no = 0;

f1_score_weights = c(-1,(0:19)/20) # f1_score = -1 indicates cluster by outlier

for (indexGlobalTasks in 1:length(globally_assigned_tasks[,1]))
{#Begin - indexGlobalTasks
  current_global_task_line = globally_assigned_tasks[indexGlobalTasks,]
  for (index_ea_correlation_parameter in 1:length(ea_correlation_parameters))
  {#Begin
    ea_correlation_parameter = ea_correlation_parameters[index_ea_correlation_parameter]
    for (indexRV in 1:length(reference_valueTypes))
    {#Begin
        reference_valueType = reference_valueTypes[indexRV]
        for (indexE in 1:length(entityClasses))
        {#Begin
            entityClass= entityClasses[indexE]
            for (indexF1scoreWeight in 1:length(f1_score_weights))
            {#Begin
               f1_score_weight = f1_score_weights[indexF1scoreWeight]
               task_no = task_no + 1
               current_task_line = current_global_task_line
               current_task_line$EntityClass = entityClass
               current_task_line$Task_no = task_no
               current_task_line$F1_score_weight = f1_score_weight
               current_task_line$Reference_valueType = reference_valueType
               current_task_line$Ea_correlation_parameter = ea_correlation_parameter
               if (length(all_tasks)==0) { all_tasks = current_task_line}
               else { all_tasks = rbind(all_tasks,current_task_line)}
            }#End
         }#End
    }#End
  }#End
}#End
## Generate all_tasks - End
#######################################################################################################################

length_tasks = length(all_tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

all_tasks$AddOrAnalyze_coCulture_data = FALSE
cc_tasks = all_tasks
cc_tasks$AddOrAnalyze_coCulture_data = TRUE
all_tasks = rbind(all_tasks,cc_tasks)
generate_large_plots_orders = c(TRUE,FALSE)
tasks = all_tasks
all_tasks = c()
for (indexGL in 1:length(generate_large_plots_orders))
{#Begin
   generate_large_plots = generate_large_plots_orders[indexGL]
   tasks$Generate_large_plots = generate_large_plots
   if (length(all_tasks)>0) { all_tasks = rbind(all_tasks,tasks)}
   else { all_tasks = tasks }
}#End

addOrAnalyze_coCulture_datas = unique(all_tasks$AddOrAnalyze_coCulture_data)

indexAddCoCulture=1
for (indexAddCoCulture in 1:length(addOrAnalyze_coCulture_datas))
{#Begin - Unique add_coCulture_data
   addOrAnalyze_coCulture_data = addOrAnalyze_coCulture_datas[indexAddCoCulture]
   
   print(paste("Analyzed data is coCulture data: ",addOrAnalyze_coCulture_data,sep=''))
   indexCurrentAdd = which(all_tasks$AddOrAnalyze_coCulture_data==addOrAnalyze_coCulture_data)
   tasks = all_tasks[indexCurrentAdd,]

   #core_tasks = tasks; indexCore=1;indexCoreTask=1
   
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
generate_large_plots = current_task_line$Generate_large_plots

entityClass = current_task_line$EntityClass
ea_correlation_parameter = current_task_line$Ea_correlation_parameter
reference_valueType = current_task_line$Reference_valueType
f1_score_weight = current_task_line$F1_score_weight
addOrAnalyze_coCulture_data = current_task_line$AddOrAnalyze_coCulture_data

## Set global parameter - BEGIN
source('SVD_coreTaskSpecific_parameter.R')
## Set global parameter - END

if (!dir.exists(visualization_of_entitySpecific_clusterDendrograms_base_directory)) { dir.create(visualization_of_entitySpecific_clusterDendrograms_base_directory) }
if (!dir.exists(visualization_of_overall_clusterDendrograms_base_directory)) { dir.create(visualization_of_overall_clusterDendrograms_base_directory) }


##########################################################################################################################
## Generate colors - BEGIN
source('SVD_colors.R')
source('Common_tools.R')
source('SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites.R')
if (addOrAnalyze_coCulture_data)
{#Begin - Read, prepare and add coCulture DEGs and summary
  dataset_coculture = current_task_line$Dataset_coCulture
  source('SVD_readPrepare_and_add_coCulture_degMatrix.R')
}#End - Read, prepare and add coCulture DEGs and summary

projected_data_type = full_dataType_label
finalSelection_criteria = finalSelection_fullData 

if (!addOrAnalyze_coCulture_data)
{#Begin
  complete_fileName = complete_collapsed_entitySpecific_cluster_validation_f1_fileName
}#End
if (addOrAnalyze_coCulture_data)
{#Begin
  complete_fileName = complete_coCulture_collapsed_entitySpecific_cluster_validation_f1_fileName
}#End

collapsed_real_validations = read.table(file=complete_fileName,stringsAsFactors = FALSE,header = TRUE,sep='\t')
collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Decomposition_method == decomposition_method,]
collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Correlation_method == correlation_method,]
collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Preprocess_data == preprocess_data,]
collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$EA_correlation_parameter == ea_correlation_parameter,]
collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Reference_valueType == reference_valueType,]
collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$EntityClass==entityClass,]

fullData_eigenassay_count = unique(collapsed_real_validations$Eigenassays_count_full_data)
if (length(fullData_eigenassay_count)!=1) { stop(paste(indexCoreTasks,": ",entityClass," ",addOrAnalyze_coCulture_data,"(length(fullData_eigenassay_count)!=1)",sep='')) }

indexAllEigenassays = which(collapsed_real_validations$Eigenassays_count==fullData_eigenassay_count)
indexF1Score0 = which(collapsed_real_validations$F1_score_weight==0)
indexCompleteData = indexAllEigenassays[indexAllEigenassays %in% indexF1Score0]
fullData_collapsed_real_validations = collapsed_real_validations[indexCompleteData,]
if (unique(fullData_collapsed_real_validations$Eigenassays_count)!=unique(fullData_collapsed_real_validations$Eigenassays_count_full_data))
{ stop("Wrong fullData_collapsed_real_validations selected") }
if ( (length(unique(fullData_collapsed_real_validations$Entity))!=length(unique(collapsed_real_validations$Entity)))
    &(!addOrAnalyze_coCulture_data))
   { stop(paste(indexCoreTasks,": ",entityClass," ",addOrAnalyze_coCulture_data,"(length(unique(fullData_collapsed_real_validations$Entity))!=length(unique(collapsed_real_validations$Entity)))",sep='')) }

if (f1_score_weight!=-1)
{ indexF1ScoreWeigth = which(collapsed_real_validations$F1_score_weight==f1_score_weight) }
if (f1_score_weight==-1)
{ indexF1ScoreWeigth = which(collapsed_real_validations$Final_selection_for_outlier_based_decomposition==outlier_finalSelection_yes_label) }
collapsed_real_validations = collapsed_real_validations[indexF1ScoreWeigth,]

indexOutlier = which(collapsed_real_validations$Is_outlier==outlier_finalSelection_yes_label)
outlier_collapsed_real_validations = collapsed_real_validations[indexOutlier,]
   
indexSelect = which(collapsed_real_validations$Final_selection==finalSelection_criteria)
collapsed_real_validations = collapsed_real_validations[indexSelect,]

entitySpecific_cluster_validations = read.csv(file = complete_entitySpecific_cluster_validation_f1_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Dataset == dataset,]
entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Decomposition_method == decomposition_method,]
entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Correlation_method == correlation_method,]
entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Preprocess_data == preprocess_data,]
entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$EA_correlation_parameter == ea_correlation_parameter,]
entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Reference_valueType == reference_valueType,]

eigenassay_statistics = read.csv(file = complete_eigenassay_correlation_statistics_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Dataset == dataset,]
eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Decomposition_method == decomposition_method,]
eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Correlation_method == correlation_method,]
eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Preprocess_data == preprocess_data,]
eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Reference_valueType == reference_valueType,]
indexEntityClass = which(eigenassay_statistics$EntityClass == entityClass)
eigenassay_statistics = eigenassay_statistics[indexEntityClass,]
## Generate colors - END
##########################################################################################################################
#### Visualize F1 scores - Begin

all_entities = c(unique(collapsed_real_validations$Entity))
indexKeep = which(all_entities %in% c(entitySpecific_cluster_validations$Entity))
all_entities = c(all_entities[indexKeep])

indexAllEntities=1

dist.function = function(x) as.dist((1-cor((x),method=correlation_method))/2)
hclust.function = function(x) hclust(x,method="average")

full_col_dist = dist.function(Data)
full_col_hc = hclust.function(full_col_dist)
full_col_dend = as.dendrogram(full_col_hc)

indexOriginalData_col_full = order.dendrogram(full_col_dend);
col_labels_in_dendrogram_full = Colnames[indexOriginalData_col_full]
Full_data_reordered = Data[,indexOriginalData_col_full]

add_coCulture_label="";
if (addOrAnalyze_coCulture_data) { add_coCulture_label = coCulture_label_following_underline }

if (f1_score_weight!=-1)
{ complete_pdf_fileName = paste(visualization_of_entitySpecific_clusterDendrograms_directory,"Entity_specific_subspaces_",reference_valueType,"_",entityClass,add_coCulture_label,"_F1SW",f1_score_weight,".pdf",sep='') }
if (f1_score_weight==-1)
{ complete_pdf_fileName = paste(visualization_of_entitySpecific_clusterDendrograms_directory,"Entity_specific_subspaces_",reference_valueType,"_",entityClass,add_coCulture_label,"_F1SWByOutlier.pdf",sep='') }
if (generate_large_plots)
{ complete_pdf_fileName = gsub(".pdf","_large.pdf",complete_pdf_fileName) }
pdf(complete_pdf_fileName, width=8.5, height=11);

if (!generate_large_plots)
{ par(mfrow=c(11,2)) }
if (generate_large_plots)
{ par(mfrow=c(8,1)) }

source('SVD_colors.R')

indexInAllEntities = which(drugs_in_drugClass_specific_order %in% all_entities)
indexMissing = which(!all_entities %in% drugs_in_drugClass_specific_order)
if (length(indexMissing)>0) { rm(drugs_in_drugClass_specific_order) }
all_entities = drugs_in_drugClass_specific_order[indexInAllEntities] 

#all_entities = all_entities[order(all_entities)]

indexAllEntities=4
for (indexAllEntities in 1:length(all_entities))
{#Begin
   current_entity = all_entities[indexAllEntities]

   {#Begin - Generate reduced data
      currentEntity_structure = structuralGroups[[current_entity]]
      if (is.null(currentEntity_structure)) { currentEntity_structure = "noStructure"}

      indexFullData = which(fullData_collapsed_real_validations$Entity==current_entity)
      full_entity_collapsed_validations = fullData_collapsed_real_validations[indexFullData,]
      indexFinalSelection = which(full_entity_collapsed_validations$Final_selection==finalSelection_criteria)
      fs_full_entity_collapsed_validations = full_entity_collapsed_validations[indexFinalSelection,]
      
      indexEntity_in_collapsed = which(collapsed_real_validations$Entity==current_entity)
      entity_collapsed_validations = collapsed_real_validations[indexEntity_in_collapsed,]

      indexFinalSelections = which(entity_collapsed_validations$Final_selection==finalSelection_criteria)
      fS_entity_collapsed_validations = entity_collapsed_validations[indexFinalSelections,]

      if (length(fS_entity_collapsed_validations[,1])!=1) { stop("(length(fS_entity_collapsed_validations[,1])!=1)") }
      if (length(fs_full_entity_collapsed_validations[,1])!=1) { stop("(length(fs_full_entity_collapsed_validations[,1])!=1)") }
      
      eigenassays_count = fS_entity_collapsed_validations$Eigenassays_count

      currentEntity_eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Entity==current_entity,]
      currentEntity_eigenassay_statistics = currentEntity_eigenassay_statistics[order(currentEntity_eigenassay_statistics[[ea_correlation_parameter]]),]
      current_eigenassays = currentEntity_eigenassay_statistics$Eigenassay[1:eigenassays_count]
      if (length(current_eigenassays)!=length(unique(current_eigenassays))) { current_cum_eigensexpression_fraction = NA }
      current_cum_eigensexpression_fraction = sum(currentEntity_eigenassay_statistics$Eigenexpression_fraction[1:eigenassays_count])
      source('SVD_generate_reduced_data_and_setColnames_to_fullDataColnames.R')
   }#End - Generate reduced data

   reduced_col_dist = dist.function(Data_current)
   reduced_col_hc = hclust.function(reduced_col_dist)
   reduced_col_dend = as.dendrogram(reduced_col_hc)
   
   indexOriginalData_col_reduced = order.dendrogram(reduced_col_dend);
   col_labels_in_dendrogram_reduced = Colnames[indexOriginalData_col_reduced]
   Reduced_data_reordered = Data_current[,indexOriginalData_col_reduced]
   
   data_for_visualization_list = list("Full" = Full_data_reordered,
                                      "Decomposed" = Reduced_data_reordered)
   col_dends_list = list("Full" = full_col_dend,
                         "Decomposed" = reduced_col_dend)
   cluster_indexes = list("Full" = fs_full_entity_collapsed_validations$IndexCluster,
                          "Decomposed" = fS_entity_collapsed_validations$IndexCluster)
   
   indexVisualize=1
   for (indexVisualize in 1:length(names(data_for_visualization_list)))
   {#Begin
      visualization_data_name = names(data_for_visualization_list)[indexVisualize]
      visualization_data = data_for_visualization_list[[visualization_data_name]]
      visualization_col_dend = col_dends_list[[visualization_data_name]]
      cluster_index = cluster_indexes[[indexVisualize]]
      
      clusters = partition_leaves(visualization_col_dend)
      selected_cluster_colnames = clusters[[cluster_index]]
      
      current_colnames = colnames(visualization_data)

      background_color = "gray90"
      background_dendrogram_color = "gray60"
      selectedCluster_color = "black"
      current_drug_colors = replicate(length(current_colnames),background_color)
      current_drugClass_colors = replicate(length(current_colnames),background_color)
      current_cellline_colors = replicate(length(current_colnames),background_color)
      current_drug_cellline_colors = replicate(length(current_colnames),background_color)
      current_selectedBranches_colors = replicate(length(current_colnames),background_dendrogram_color)
      current_drugTarget_colors = replicate(length(current_colnames),background_color)
      current_drugStructure_colors = replicate(length(current_colnames),background_color)
      current_sigDEGs_color = replicate(length(current_colnames),background_color)
      current_drug_labels = replicate(length(current_colnames),"error")
      current_cellline_labels = replicate(length(current_colnames),"error")
      current_drug_cellline_lables = replicate(length(current_colnames),"error")
      current_drugClass_labels = replicate(length(current_colnames),"error")
      indexColNames=267
      for (indexColNames in 1:length(current_colnames))
      {#Begin - Generate colors for single entity dendrograms
         current_colname = current_colnames[indexColNames]
         splitStrings = strsplit(current_colname,"[.]")[[1]]
         drug = splitStrings[3]
         drugStructure = structuralGroups[[drug]]
         cellline = splitStrings[2]
         drugClass = splitStrings[1]
         
         current_drug_labels[indexColNames] = full_drugNames[[drug]]
         if (drug==current_entity)
         {#Begin
            indexOutlierDrug = which(outlier_collapsed_real_validations$Entity==current_entity)
            indexOutlierCellline = which(outlier_collapsed_real_validations$Final_selection_for_sample==cellline)
            indexOutlierDrugCellline = indexOutlierDrug[indexOutlierDrug %in% indexOutlierCellline]
            if (length(indexOutlierDrugCellline)>0)
            {#Begin
               current_drug_labels[indexColNames] = paste("Outlier - ",current_drug_labels[indexColNames],sep='')
            }#End
         }#End

         splitStrings = strsplit(cellline,"_")[[1]]
         if (length(splitStrings)==2)
         {#Begin
            current_cellline_labels[indexColNames] = splitStrings[1]
            current_drug_cellline_lables[indexColNames] = paste(current_drug_labels[indexColNames],": ",current_cellline_labels[indexColNames],sep='')
         }#End

         if (length(splitStrings)==3)
         {#Begin
            current_cellline_labels[indexColNames] = paste(splitStrings[3]," ",splitStrings[1],sep='')
            current_drug_cellline_lables[indexColNames] = paste(splitStrings[3]," - ",current_drug_labels[indexColNames],": ",splitStrings[1],sep='')
         }#End
         current_drugClass_labels[indexColNames] = drugClass
         if ((current_entity=="All")||(drugStructure==currentEntity_structure))
         {#Begin
            current_drugStructure_colors[indexColNames] = structuralGroup_colors[[drug]]
         }#End
         if ((current_task_line$EntityClass=="Drug")&(drug==current_entity))
         {#Begin
            indexColor = which(drugs_for_highlight_colors==drug) 
            if (length(indexColor)==1)
            {#Begin
               current_drug_cellline_colors[indexColNames] = drugClass_colors[[drug_drugClass[[drug]]]]
            }#End
         }#End
         if ((current_task_line$EntityClass=="Cell_line")&(cellline==current_entity))
         {#Begin
            indexColor = which(cellLines_for_highlight_colors==cellline) 
            if (length(indexColor)==1)
            {#Begin
               current_drug_cellline_colors[indexColNames] = cellLine_highlight_colors[indexColor]
            }#End
         }#End
         if (current_colname %in% selected_cluster_colnames)
         {#Begin
            current_selectedBranches_colors[indexColNames] = selectedCluster_color
         }#End
      }#End - Generate colors for single entity dendrograms

      {#Begin - Set colors and labels for single entity dendrogram
         v_entityClasses = c("Cell_line","Drug_cell_line","Drug","DrugStructure");
         v_colors = list(current_cellline_colors,current_drug_cellline_colors,current_drug_colors,current_drugStructure_colors) 
         v_labels = list(current_cellline_labels,current_drug_cellline_lables,current_drug_labels,current_drug_labels)
         current_complete_visualization_of_clusterDendrograms_directory = visualization_of_entitySpecific_clusterDendrograms_base_directory
   
         v_entityClasses = c("Drug_cell_line");
         v_label_colors = list(current_drug_cellline_colors) 
         v_dendrogram_colors = list(current_selectedBranches_colors)
         v_labels = list(current_drug_cellline_lables)
         current_complete_visualization_of_clusterDendrograms_directory = visualization_of_entitySpecific_clusterDendrograms_base_directory
      }#End - Set colors and labels for single entity dendrogram

      if (!generate_large_plots) 
      {#Begin
         dendrogram_label_size = 0.215 
         dendrogram_title_size = 0.6
      }#End
      if (generate_large_plots) 
      {#Begin
         dendrogram_label_size = 0.35 
         dendrogram_title_size = 1
      }#End
      
      indexEC=1
      for (indexEC in 1:length(v_entityClasses))
      {#Begin - Add single entity dendrogram to PDF
          v_entityClass = v_entityClasses[indexEC]
          current_label_colors = v_label_colors[[indexEC]]
          current_dendrogram_colors = v_dendrogram_colors[[indexEC]]
          if (length(unique(current_label_colors))<=1) { stop("(length(unique(current_label_colors))<=1)") }
          if (length(unique(current_label_colors))>1)
          {#Begin
             current_labels = v_labels[[indexEC]]
             visualization_col_dend <- set(visualization_col_dend, "labels_cex", dendrogram_label_size)    
             visualization_col_dend = color_labels(visualization_col_dend,col=current_label_colors)
             visualization_col_dend = set_labels(visualization_col_dend,current_labels)
             visualization_col_dend = color_branches(visualization_col_dend,k=1,col=background_dendrogram_color)
             visualization_col_dend = color_branches(visualization_col_dend,k=length(current_dendrogram_colors),col=current_dendrogram_colors)
             #visualization_col_dend = set(visualization_col_dend, "labels_font",2)
             #dendrogram_title = paste("\n",ea_correlation_parameter," (F1 <= ",
             #                         round(minimumF1Score*100)/100," = ",round(minimum_fraction_of_max_f1score*100)/100," * ",round(maxF1Score*100)/100,
             #                         "\n",eigenassays_count," Eigenassays (",round(current_cum_eigensexpression_fraction*100)/100," cum eigenexpression fraction)",sep='')
             drugName = entity_collapsed_validations$Entity
             if (drugName %in% names(full_drugNames))
             { drugName = full_drugNames[[drugName]] }
             if (visualization_data_name=="Full")
             {#Begin
                if (grepl("no1stSVD",current_task_line$Dataset)) { dataset_info = " after removal of 1st eigenarray" }
                else { dataset_info = " in complete data" }
                dendrogram_title = paste("\n\n",drugName,dataset_info,
                                         ", F1: ", round(fs_full_entity_collapsed_validations$F1_score*100)/100,
                                         ", Precision: ", round(fs_full_entity_collapsed_validations$Precision*100)/100,
                                         ", Recall: ",round(fs_full_entity_collapsed_validations$Recall*100)/100 
                                         #", mediane cosine similarity: ",round(fs_full_entity_collapsed_validations$Median_correlation_reduded_full_data*100)/100,
                                         #", top ",fs_full_entity_collapsed_validations$Eigenassays_count," eigenassays)",sep='')
                                         )
             }#End
             if (visualization_data_name=="Decomposed")
             {#Begin
                dendrogram_title = paste("\n",drugName," in decomposed data (F1 score weight: ",entity_collapsed_validations$F1_score_weight," ",
                                         ", F1: ", round(entity_collapsed_validations$F1_score*100)/100,
                                         ", Precision: ", round(entity_collapsed_validations$Precision*100)/100,
                                         ", Recall: ",round(entity_collapsed_validations$Recall*100)/100, 
                                         ",\nmediane cosine similarity: ",round(entity_collapsed_validations$Median_correlation_reduded_full_data*100)/100,
                                         ", top ",entity_collapsed_validations$Eigenassays_count," eigenarrays)",sep='')
             }#End
             if (!generate_large_plots)
             {#Begin
                par(mar=c(4, 1, 2, 1)) 
             }#End
             if (generate_large_plots)
             {#Begin
                par(mar=c(6, 1, 2, 1)) 
             }#End
             plot(visualization_col_dend,horiz=FALSE,las=1,axes=FALSE);
             par(mar=c(1, 1, 1, 1))
             title(main=dendrogram_title,cex.main=dendrogram_title_size,font=2)
             #dendrogram_title = "";
          }#End
      }#End - Add single entity dendrogram to PDF
    }#End
}#End#if (dim(All_reduced_data)[1]!=dim(Data_combined)[1]) { rm(All_reduced_data) }
#if (dim(All_reduced_data)[2]!=dim(Data_combined)[2]) { rm(All_reduced_data) }
}#End - indexCoreTask

dev.off()

##########################################################################################################################
}#End - Parallel clusters

)# SVD_12
}#End - Unique add_coCulture_data within SVD 12

{#Begin - Close parallel clusters
  invisible(gc())
  parallel::stopCluster(parallel_clusters)
  invisible(gc())
  rm(parallel_clusters)
}#End - Close parallel clusters
