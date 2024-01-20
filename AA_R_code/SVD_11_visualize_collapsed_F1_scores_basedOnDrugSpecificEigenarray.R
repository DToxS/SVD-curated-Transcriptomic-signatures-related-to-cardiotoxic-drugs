#stopCluster(parallel_clusters)
rm(list = ls());

####################################################################################################################
#Open libraries and document versions - BEGIN
Col_names = c("Library","Version")
Col_length = length(Col_names)
Row_names = 1
Row_length= length(Row_names)
version_documentation_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
version_documentation_line = as.data.frame(version_documentation_line)
version_documentations = c()

libraries = c("ClassDiscovery","colorspace","dendextend","ape","fields","colormap","beeswarm","gplots","grid","gridExtra","ggplot2","genefilter","doParallel")
for (indexL in 1:length(libraries))
{#Begin
  current_library = libraries[indexL]
  library(current_library,character.only=TRUE)
  new_version_documentation_line = version_documentation_line
  new_version_documentation_line$Library = current_library
  new_version_documentation_line$Version = packageVersion(current_library)
  if (length(version_documentations)==0)
  {#Begin
    version_documentations = new_version_documentation_line
  }#End
  else
  {#Begin
    version_documentations = rbind(version_documentations,new_version_documentation_line)
  }#End
}#End
r_sessionInfo = sessionInfo()
## Open libraries and document versions - END
##########################################################################################################################
#######################################################################################################################
## Generate tasks - BEGIN

tasks = c()

get_eigenassays_per_dataset=FALSE
delete_task_reports=TRUE
add_inFrontOf_progress_report_fileName ="SVD_11"
f1_score_weight=-1
source('SVD_global_parameter.R')

tasks = globally_assigned_tasks

tasks$AddOrAnalyze_coCulture_data_order = "No"
tasks$AddOrAnalyze_coCulture_data = FALSE
cc_tasks = tasks
cc_tasks$AddOrAnalyze_coCulture_data_order= "Yes"
cc_tasks$AddOrAnalyze_coCulture_data = TRUE
compare_tasks = tasks
compare_tasks$AddOrAnalyze_coCulture_data_order= "Compare"
compare_tasks$AddOrAnalyze_coCulture_data = TRUE
tasks = rbind(cc_tasks,tasks,compare_tasks)
#tasks = compare_tasks

## Generate tasks - BEGIN
#######################################################################################################################

length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

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
f1_score_weight_default = current_task_line$F1_score_weight_default
addOrAnalyze_coCulture_data = current_task_line$AddOrAnalyze_coCulture_data
addOrAnalyze_coCulture_data_order = current_task_line$AddOrAnalyze_coCulture_data_order
f1_score_weight=-1
dataset = current_task_line$Dataset


top_eigenassay_count = 30


## Set global parameter - BEGIN
source('SVD_coreTaskSpecific_parameter.R')
## Set global parameter - END

if (!dir.exists(visualization_of_cluster_validations_directory)) {dir.create(visualization_of_cluster_validations_directory)}

##########################################################################################################################
## Generate colors - BEGIN
source('SVD_colors.R')
source('Common_tools.R')
## Generate colors - END
##########################################################################################################################
#### Visualize F1 scores - Begin
if (addOrAnalyze_coCulture_data_order=="No")
{#Begin
   complete_fileName = complete_collapsed_entitySpecific_cluster_validation_f1_fileName
   cluster_validations = read.csv(file = complete_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
   indexKeep = which(cluster_validations$EntityClass=="Drug")
   cluster_validations = cluster_validations[indexKeep,]
   cluster_validations$Subtract = FALSE
}#End
if (addOrAnalyze_coCulture_data_order=="Yes")
{#Begin
  complete_fileName = complete_coCulture_collapsed_entitySpecific_cluster_validation_f1_fileName
  cluster_validations = read.csv(file = complete_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
  indexKeep = which(cluster_validations$EntityClass=="Drug")
  cluster_validations = cluster_validations[indexKeep,]
  cluster_validations$Subtract = FALSE
}#End
if (addOrAnalyze_coCulture_data_order=="Compare")
{#Begin
  complete_fileName = complete_collapsed_entitySpecific_cluster_validation_f1_fileName
  original_cluster_validations = read.csv(file = complete_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
  complete_fileName = complete_coCulture_collapsed_entitySpecific_cluster_validation_f1_fileName
  coCulture_cluster_validations = read.csv(file = complete_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
  original_referenceValueTypes = unique(original_cluster_validations$Reference_valueType)
  coCulture_referenceValueTypes = unique(coCulture_cluster_validations$Reference_valueType)
  keep_referenceValueTypes = original_referenceValueTypes[original_referenceValueTypes %in% coCulture_referenceValueTypes]
  validations_list = list(original_cluster_validations,coCulture_cluster_validations)
  indexVL=1
  for (indexVL in 1:length(validations_list))
  {#Begin
     vl_validations = validations_list[[indexVL]]
     indexKeep = which(vl_validations$EntityClass=="Drug")
     vl_validations = vl_validations[indexKeep,]
     indexKeep = which( (vl_validations$Final_selection==finalSelection_fullData)
                       |(vl_validations$Final_selection==finalSelection_fullData_sampleSpecific))
     vl_validations = vl_validations[indexKeep,]
     indexKeep = which(vl_validations$Reference_valueType %in% keep_referenceValueTypes)
     vl_validations = vl_validations[indexKeep,]
     validations_list[[indexVL]] = vl_validations
  }#End
  original_cluster_validations = validations_list[[1]]
  coCulture_cluster_validations = validations_list[[2]]
  indexSameCellLines = which(coCulture_cluster_validations$Final_selection_for_sample %in% original_cluster_validations$Final_selection_for_sample)
  if (dim(original_cluster_validations)[1]!=dim(coCulture_cluster_validations[indexSameCellLines,])[1]) { stop("Dimensions of original and coCulture cluster validations not equal")}
  if (dim(original_cluster_validations)[2]!=dim(coCulture_cluster_validations[indexSameCellLines,])[2]) { stop("Dimensions of original and coCulture cluster validations not equal")}
  coCulture_cluster_validations$Subtract = TRUE
  original_cluster_validations$Subtract = FALSE
  cluster_validations = rbind(original_cluster_validations,coCulture_cluster_validations)
}#End

if (length(unique(cluster_validations$Dataset))!=1) { stop("") }
if (length(unique(cluster_validations$Correlation_method))!=1) { stop("") }
if (length(unique(cluster_validations$Preprocess_data))!=1) { stop("") }
if (length(unique(cluster_validations$Decomposition_method))!=1) { stop("") }
if (length(unique(cluster_validations$CumEigenexpression_cutoff))!=1) { stop("") }
referenceValue_types = unique(cluster_validations$Reference_valueType)

indexFinalTestSets=c()

Generate_figure_set = function(current_validations,dataset_description,drugs_in_drugClass_specific_order)
{#Begin - Generate figure set
   Plots=list()
   
   regular_color = "gray45"
   regular_with_outlier_color = "skyblue4"
   outlier_color = "gray65"
   median_line_color = "navy"
   
   regular_with_outlier_label = "Has_outlier"
   
   colors_for_outlier_plots = c(regular_color,regular_with_outlier_color,outlier_color)
   names(colors_for_outlier_plots) = c(outlier_not_significant_label,regular_with_outlier_label,outlier_significant_label)
   
   entities = unique(current_validations$Entity)

   current_validations$Shortened_fullName = "error"   
   for (indexEntity in 1:length(entities))
   {#Begin
      entity = entities[indexEntity]
      indexCurrentEntity = which(current_validations$Entity==entity)
      if (entity %in% names(drug_shortenedFullNames))
      {#Begin
         current_validations$Shortened_fullName[indexCurrentEntity] = drug_shortenedFullNames[[entity]]
      }#End
      else
      {#Begin
        current_validations$Shortened_fullName[indexCurrentEntity] = entity
      }#End
   }#End

   current_validations$Entity_factor = factor(current_validations$Entity,levels=drugs_in_drugClass_specific_order)
   current_validations = current_validations[order(current_validations$Entity_factor),]
   current_validations$Shortened_fullName_factor = factor(current_validations$Shortened_fullName,levels=unique(current_validations$Shortened_fullName))
   
   indexFullData = which(current_validations$Final_selection==finalSelection_fullData)
   current_validations_fullData = current_validations[indexFullData,]
   min_y = 0
   max_y = 1

   if (TRUE %in% current_validations_fullData$Subtract)
   {#Begin
      min_y = -max_y
      indexSubtrahend = which(current_validations_fullData$Subtract)
      subtrahend_validations = current_validations_fullData[indexSubtrahend,]
      indexMinuend = which(!current_validations_fullData$Subtract)
      minuend_validations = current_validations_fullData[indexMinuend,]
      subtrahend_validations = subtrahend_validations[order(subtrahend_validations$Entity),]
      minuend_validations = minuend_validations[order(minuend_validations$Entity),]
      indexMismatch = which(subtrahend_validations$Entity!=minuend_validations$Entity)
      if (length(indexMismatch)>0) {stop("Subtrahend and minuend datasets have not the same entities")}
      minuend_validations$F1_score = -(minuend_validations$F1_score - subtrahend_validations$F1_score)
      minuend_validations$F1_score_full_data = -(minuend_validations$F1_score_full_data - subtrahend_validations$F1_score_full_data)
      minuend_validations$Median_correlation_reduded_full_data = -(minuend_validations$Median_correlation_reduded_full_data - subtrahend_validations$Median_correlation_reduded_full_data)
      minuend_validations$Entity_samples_count = -(minuend_validations$Entity_samples_count - subtrahend_validations$Entity_samples_count)
      current_validations_fullData = minuend_validations
   }#End

   indexSampleData = which(current_validations$Final_selection==finalSelection_fullData_sampleSpecific)
   sample_validations = current_validations[indexSampleData,]

   current_validations_fullData = current_validations_fullData[order(current_validations_fullData$Entity_factor),]
   all_entities = unique(current_validations_fullData$Entity)
   entity_colors = replicate(length(all_entities),"gray40")
   for (indexAllEntities in 1:length(all_entities))
   {#Begin
      current_entity = all_entities[indexAllEntities]
      drugClass = drug_drugClass[[current_entity]]
      entity_colors[indexAllEntities] = drugClass_colors[[drugClass]]
   }#End
   
   median_F1_full_data = median(current_validations_fullData$F1_score_full_data)
   top_median_F1 = median(current_validations_fullData$F1_score)
   top_median_cosine = median(current_validations_fullData$Median_correlation_reduded_full_data)
   top_median_samples_count = median(current_validations_fullData$Entity_samples_count)

   title_size = 12
   drug_x_label_size = 12
   bar_width = 0.85
   
   Plot = ggplot(current_validations_fullData,aes(x=Shortened_fullName_factor,y=F1_score_full_data,group=EntityClass,fill=Is_outlier))
   Plot = Plot + geom_bar(stat="identity",width=bar_width)
   if (!TRUE %in% current_validations$Subtract)
   { 
     Plot = Plot + geom_hline(yintercept = median_F1_full_data,color=median_line_color,size=1)
     if (grepl("_no1stSVD",dataset))
     { Plot = Plot + ggtitle(paste("F1 score after removal of 1st eigenarray",sep='')) }
     else
     { Plot = Plot + ggtitle(paste("F1 score in full dataset",sep='')) }
   }
   if (TRUE %in% current_validations$Subtract)
   { 
     if (grepl("_no1stSVD",dataset))
     { Plot = Plot + ggtitle(paste("Changes in F1 scores after removal of 1st eigenarray",sep='')) }
     else
     { Plot = Plot + ggtitle(paste("Changes in F1 scores in full dataset",sep='')) }
   }
   Plot = Plot + scale_fill_manual(values=c(regular_color))
   Plot = Plot + theme(axis.text.x = element_text(size=drug_x_label_size,colour=entity_colors))
   Plot = Plot + theme(axis.text.y = element_text(size=drug_x_label_size))
   Plot = Plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
   Plot = Plot + theme(axis.title.y = element_blank())
   Plot = Plot + theme(axis.title.x = element_blank())
   Plot = Plot + coord_cartesian(ylim=c(min_y,max_y))
   Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
   Plot = Plot + theme(legend.position = "none")
   Plots[[length(Plots)+1]] = Plot
   
   Plot = ggplot(current_validations_fullData,aes(x=Shortened_fullName_factor,y=F1_score,group=EntityClass,fill=Is_outlier))
   Plot = Plot + geom_bar(stat="identity",width=bar_width)
   if (!TRUE %in% current_validations$Subtract)
   { 
     Plot = Plot + geom_hline(yintercept = top_median_F1,color=median_line_color,size=1)
     Plot = Plot + ggtitle(paste("F1 score - top selections (",dataset_description,")",sep=''))
   }
   if (TRUE %in% current_validations$Subtract)
   { 
     Plot = Plot + ggtitle(paste("Changes in F1 scores - top selections (",dataset_description,")",sep=''))
   }
   Plot = Plot + scale_fill_manual(values=c(regular_color))
   Plot = Plot + theme(axis.text.x = element_text(size=drug_x_label_size,colour=entity_colors))
   Plot = Plot + theme(axis.text.y = element_text(size=drug_x_label_size))
   Plot = Plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
   Plot = Plot + theme(axis.title.y = element_blank())
   Plot = Plot + theme(axis.title.x = element_blank())
   Plot = Plot + coord_cartesian(ylim=c(min_y,max_y))
   Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
   Plot = Plot + theme(legend.position = "none")
   Plots[[length(Plots)+1]] = Plot
   
   subtracts = unique(sample_validations$Subtract)
   plot_validations = c()
   indexSub=2
   for (indexSub in 1:length(subtracts))
   {#Begin
      subtract = subtracts[indexSub]
      indexCurrentSub = which(sample_validations$Subtract==subtract)
      s_sample_validations = sample_validations[indexCurrentSub,]
      drugs = unique(s_sample_validations$Entity)
      mean_f1_scores_without_outlier = c()
      indexDrug=1
      for (indexDrug in 1:length(drugs))
      {#Begin
        drug = drugs[indexDrug]
        indexCurrentDrug = which(s_sample_validations$Entity==drug)
        drug_validations = s_sample_validations[indexCurrentDrug,]
        indexNoOutlier = which(drug_validations$Is_outlier==outlier_finalSelection_no_label)
        indexOutlier = which(drug_validations$Is_outlier==outlier_finalSelection_yes_label)
        drug_validations$Is_outlier = outlier_not_significant_label
        drug_validations$Mean_f1_score=-1
        drug_validations$Sd_f1_score=-1
        drug_validations$Mean_cosine_similarity=-1
        drug_validations$Sd_cosine_similarity=-1
        drug_validations$Mean_f1_score_plot = -1
        drug_validations$Mean_f1_score[indexNoOutlier] = mean(drug_validations$F1_score[indexNoOutlier])
        drug_validations$Mean_f1_score_plot[indexNoOutlier] = drug_validations$Mean_f1_score[indexNoOutlier]
        drug_validations$Sd_f1_score[indexNoOutlier] = sd(drug_validations$F1_score[indexNoOutlier])
        #drug_validations$Mean_cosine_similarity[indexNoOutlier] = mean(drug_validations$F1_score[indexNoOutlier])
        #drug_validations$Sd_cosine_similarity[indexNoOutlier] = sd(drug_validations$Mean_cosine_similarity[indexNoOutlier])
        if (length(indexOutlier)>0)
        {#Begin
           if (length(indexOutlier)!=1) { rm(drug_validations)}
           drug_validations$Is_outlier = regular_with_outlier_label
           drug_validations$Is_outlier[indexOutlier] = outlier_significant_label
           drug_validations$Mean_f1_score[indexOutlier] = mean(drug_validations$F1_score[indexOutlier])
           drug_validations$Mean_f1_score_plot[indexOutlier] = drug_validations$Mean_f1_score[indexOutlier]
           drug_validations$Sd_f1_score[indexOutlier] = sd(drug_validations$F1_score[indexOutlier])
           drug_validations$Mean_f1_score_plot[indexNoOutlier] = drug_validations$Mean_f1_score[indexNoOutlier] - drug_validations$Mean_f1_score[indexOutlier]
           if (length(plot_validations)>0) { plot_validations = rbind(plot_validations,drug_validations[indexOutlier,]) }
           else { plot_validations = drug_validations[indexOutlier,] }
        }#End
        if (length(plot_validations)>0) { plot_validations = rbind(plot_validations,drug_validations[indexNoOutlier[1],]) }
        else { plot_validations = drug_validations[indexNoOutlier[1],] }
     }#End
   }#End
   
   if (TRUE %in% plot_validations$Subtract)
   {#Begin
      indexNoOutlier = which( (plot_validations$Is_outlier==outlier_not_significant_label)
                             |(plot_validations$Is_outlier==regular_with_outlier_label))
      plot_validations = plot_validations[indexNoOutlier,]
      indexMinuend = which(!plot_validations$Subtract)
      indexSubtrahend = which(plot_validations$Subtract)
      minuend_plot = plot_validations[indexMinuend,]
      subtrahend_plot = plot_validations[indexSubtrahend,]
      minuend_plot = minuend_plot[order(minuend_plot$Is_outlier),]
      minuend_plot = minuend_plot[order(minuend_plot$Entity),]
      subtrahend_plot = subtrahend_plot[order(subtrahend_plot$Is_outlier),]
      subtrahend_plot = subtrahend_plot[order(subtrahend_plot$Entity),]
      indexMismatch = which(subtrahend_plot$Entity!=minuend_plot$Entity)
      if (length(indexMismatch)>0) { stop(paste("sample validations plot have mismatching entries: ",dataset_description,sep='')) }
      minuend_plot$Mean_f1_score_plot = -(minuend_plot$Mean_f1_score_plot - subtrahend_plot$Mean_f1_score_plot)
      minuend_plot$Mean_f1_score = -(minuend_plot$Mean_f1_score - subtrahend_plot$Mean_f1_score)
      minuend_plot$Sd_f1_score = 0
      plot_validations = minuend_plot
   }#End
   
   
   indexNoOutlier = which( (plot_validations$Is_outlier==outlier_finalSelection_no_label)
                          |(plot_validations$Is_outlier==regular_with_outlier_label))
   median_mean_sample_F1_score = median(plot_validations$Mean_f1_score[indexNoOutlier])
   
   Plot = ggplot(plot_validations,aes(x=Shortened_fullName_factor,y=Mean_f1_score_plot,group=EntityClass,fill=Is_outlier))
   Plot = Plot + geom_bar(stat="identity",width=bar_width)
   if (!TRUE %in% current_validations$Subtract)
   { 
     Plot = Plot + geom_hline(yintercept = median_mean_sample_F1_score,color=median_line_color,size=1)
     Plot = Plot + geom_errorbar(aes(ymin=Mean_f1_score - Sd_f1_score, max=Mean_f1_score + Sd_f1_score))
     Plot = Plot + ggtitle(paste("Drug-selective F1 scores for each cell line",sep=''))
     Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
   }
   if (TRUE %in% current_validations$Subtract)
   {
     Plot = Plot + ggtitle(paste("Change in drug-selective F1 scores for each cell line (excluding outlier)",sep=''))
     Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=0.8*title_size))
   }
   Plot = Plot + scale_fill_manual(values=c(colors_for_outlier_plots))
   Plot = Plot + theme(axis.text.x = element_text(size=drug_x_label_size,colour=entity_colors))
   Plot = Plot + theme(axis.text.y = element_text(size=drug_x_label_size))
   Plot = Plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
   Plot = Plot + theme(axis.title.y = element_blank())
   Plot = Plot + theme(axis.title.x = element_blank())
   Plot = Plot + theme(legend.position = "none")
   Plot = Plot + coord_cartesian(ylim=c(min_y,max_y))
   Plots[[length(Plots)+1]] = Plot

   Plot = ggplot(current_validations_fullData,aes(x=Shortened_fullName_factor,y=Median_correlation_reduded_full_data,group=EntityClass,fill=Is_outlier))
   Plot = Plot + geom_bar(stat="identity",width=bar_width)
   if (!TRUE %in% current_validations$Subtract)
   { 
     Plot = Plot + geom_hline(yintercept = top_median_cosine,color=median_line_color,size=1)
     Plot = Plot + ggtitle(paste("Median cosine similarity - top selections  (",dataset_description,")",sep=''))
     Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
   }
   if (TRUE %in% current_validations$Subtract)
   { 
     Plot = Plot + ggtitle(paste("Changes in median cosine similarities - top selections  (",dataset_description,")",sep=''))
     Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=0.8*title_size))
   }
   Plot = Plot + scale_fill_manual(values=c(regular_color))
   Plot = Plot + theme(axis.text.x = element_text(size=drug_x_label_size,colour=entity_colors))
   Plot = Plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
   Plot = Plot + theme(axis.text.y = element_text(size=drug_x_label_size))
   Plot = Plot + theme(axis.title.y = element_blank())
   Plot = Plot + theme(axis.title.x = element_blank())
   Plot = Plot + coord_cartesian(ylim=c(min_y,max_y))
   Plot = Plot + theme(legend.position = "none")
   Plots[[length(Plots)+1]] = Plot
   
   Plot = ggplot(current_validations_fullData,aes(x=Shortened_fullName_factor,y=Entity_samples_count,group=EntityClass,fill=Is_outlier))
   Plot = Plot + geom_bar(stat="identity",width=bar_width)
   if (!TRUE %in% current_validations$Subtract)
   { 
     Plot = Plot + geom_hline(yintercept = top_median_samples_count,color=median_line_color,size=1)
     Plot = Plot + ggtitle(paste("# of drug-treated samples",sep=''))
     Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
   }
   if (TRUE %in% current_validations$Subtract)
   { 
     Plot = Plot + ggtitle(paste("Changes in # of drug-treated samples",sep=''))
     Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=0.8*title_size))
   }
   Plot = Plot + scale_fill_manual(values=c(regular_color))
   Plot = Plot + theme(axis.text.x = element_text(size=drug_x_label_size,colour=entity_colors))
   Plot = Plot + theme(axis.text.y = element_text(size=drug_x_label_size))
   Plot = Plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
   Plot = Plot + theme(axis.title.y = element_blank())
   Plot = Plot + theme(axis.title.x = element_blank())
   Plot = Plot + theme(legend.position = "none")
   Plots[[length(Plots)+1]] = Plot
   
   return (Plots)
}#End - Generate figure set

indexRef=1
for (indexRef in 1:length(referenceValue_types))
{#Begin
   referenceValue_type = referenceValue_types[indexRef]
   add_to_png_fileName = referenceValue_type
   indexCurrentReferenceValueType = which(cluster_validations$Reference_valueType==referenceValue_type)
   referenceValue_validations = cluster_validations[indexCurrentReferenceValueType,]
   f1_score_weights = unique(referenceValue_validations$F1_score_weight)
   indexF1_score_weigth=4
   for (indexF1_score_weigth in 1:length(f1_score_weights))
   {#Begin - Plot F1 scores group by same f1_score_weight
      f1_score_weight = f1_score_weights[indexF1_score_weigth]
      indexCurrentF1ScoreWeight = which(referenceValue_validations$F1_score_weight==f1_score_weight)
      f1_validations = referenceValue_validations[indexCurrentF1ScoreWeight,]

      Plots = list()
      Boxplots = list()
      Collapsed_plots = list()
   
      dataset_description = paste("F1 score weight = ",f1_score_weight,sep='')
      Plots = Generate_figure_set(f1_validations,dataset_description,drugs_in_drugClass_specific_order)
      
      complete_pdf_fileName = paste(visualization_of_cluster_validations_directory,"EntitySpecific_F1_",add_to_png_fileName,"_F1SW",f1_score_weight,".pdf",sep='')
      if (addOrAnalyze_coCulture_data_order=="Compare")
      { complete_pdf_fileName = gsub(".pdf","_comparison.pdf",complete_pdf_fileName) }
      rows_count = 5
      cols_count = 1
      Generate_plots(plots=Plots,complete_pdf_fileName,rows_count,cols_count)
   }#End - Plot F1 scores group by same f1_score_weight
   
   indexOutlier = which(referenceValue_validations$Final_selection_for_outlier_based_decomposition==outlier_finalSelection_yes_label)
   outlier_validations = referenceValue_validations[indexOutlier,]
   
   dataset_description = paste("F1 score weight selected by outlier",sep='')
   Plots = Generate_figure_set(outlier_validations,dataset_description,drugs_in_drugClass_specific_order)
   
   complete_pdf_fileName = paste(visualization_of_cluster_validations_directory,"EntitySpecific_F1_",add_to_png_fileName,"_F1SWByOutlier",".pdf",sep='')
   if (addOrAnalyze_coCulture_data_order=="Compare")
   { complete_pdf_fileName = gsub(".pdf","_comparison.pdf",complete_pdf_fileName) }
   rows_count = 5
   cols_count = 1
   Generate_plots(plots=Plots,complete_pdf_fileName,rows_count,cols_count)
}#End

#### Visualize F1 scores - End - SVD_7d
##########################################################################################################################
}#End - indexCoreTask
  
}#End - Parallel clusters

)# SVD_11

{#Begin - Close parallel clusters
  invisible(gc())
  parallel::stopCluster(parallel_clusters)
  invisible(gc())
  rm(parallel_clusters)
}#End - Close parallel clusters



