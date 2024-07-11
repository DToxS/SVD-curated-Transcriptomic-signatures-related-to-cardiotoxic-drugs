#stopCluster(parallel_clusters)
rm(list = ls());

#######################################################################################################################
## Generate tasks - BEGIN

get_eigenassays_per_dataset = FALSE
indexCore = -1
delete_finished_analyses=FALSE
delete_task_reports = TRUE
add_inFrontOf_progress_report_fileName = "SVD_8"
f1_score_weight=-1
source('SVD_global_parameter.R') #commented, so that script is not run by accident
tasks = c()

sigDEGs_correlation_methods = c("Pearson")
max_sigDEGs_abs_correlations = c(1)
cum_eigenexpression_fractions = c(1)

correlation_parameters = c("Ttest_pvalue")

Col_names = c("Task_no","Dataset","Correlation_method","Preprocess_data",
              "Decomposition_method","Icasso_permutations_count")

validate_all_drugs = TRUE

current_task_no = 1;
tasks = c()
indexGlobal = 1
for (indexGlobal in 1:length(globally_assigned_tasks[,1]))
{#Begin
  current_global_task_line = globally_assigned_tasks[indexGlobal,]
  current_task_line = current_global_task_line
  source('SVD_coreTaskSpecific_parameter.R')
  if (validate_all_drugs)
  { summary_local = read.table(file=complete_degs_summary_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t') }
  if (!validate_all_drugs)
  { summary_local = read.table(file=complete_degs_summary_coCulture_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t') }
  coreTask_Entities = c()
  if (length(unique(summary_local$Treatment))>1)
  { coreTask_Entities = c(coreTask_Entities,unique(summary_local$Treatment)) }

  Row_names = 1:length(coreTask_Entities)
  task_line_block = array(c(1,2),dim=c(length(coreTask_Entities),Task_col_length),dimnames = list(Row_names,Task_col_names))
  task_line_block = as.data.frame(task_line_block)
  for (indexCol in 1:length(current_global_task_line[1,]))
  {#Begin
     task_line_block[,indexCol] = current_global_task_line[1,indexCol]
  }#End
  end_task_no = current_task_no + length(coreTask_Entities) - 1
  task_line_block$Task_no = current_task_no:end_task_no
  task_line_block$CoreTask_Entitity = coreTask_Entities
  if (length(tasks)==0) { tasks = task_line_block}
  else { tasks = rbind(tasks,task_line_block)}
}#End

tasks$AddOrAnalyze_coCulture_data = TRUE
tasks$Ea_correlation_parameter = "Ttest_pvalue"
tasks$F1_score_weight = -1
tasks$Reference_valueType = "Coefficient_of_eigenassay"
tasks$EntityClass = "Drug"
tasks$Cum_eigenexpression_fraction = 1
tasks$SigDEGs_correlation_method = "Pearson"
tasks$Max_sigDEGs_abs_correlation = 1



## Generate tasks - END
#######################################################################################################################


if (delete_finished_analyses)
{#Begin - Delete finished analyses
  for (indexGlobalTask in 1:length(globally_assigned_tasks[,1]))
  {#Begin - Delete existing result files
    current_task_line = globally_assigned_tasks[indexGlobalTask,]
    indexCore=0
    source('SVD_coreTaskSpecific_parameter.R')
    allFiles = list.files(coCulture_cluster_validation_f1_directory)
    for (indexAll in 1:length(allFiles))
    {#Begin
      unlink(paste(coCulture_cluster_validation_f1_directory,allFiles[indexAll],sep=''))
    }#End
  }#End - Delete existing result files
}#End - Delete finished analyses

#Remove accomplished tasks - End

length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

#indexPermuted = sample(1:length_tasks,length_tasks,replace=FALSE)
#tasks = tasks[indexPermuted,]


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

tasks$IndexCore = -1

for (indexCore in 1:cores_count)
{#Begin
  startIndex = min(floor((indexCore-1) * tasks_per_core+1),length_tasks);
  endIndex = min(floor(indexCore * tasks_per_core),length_tasks)
  tasks$IndexCore[startIndex:endIndex] = indexCore
  core_tasks = tasks[startIndex:endIndex,]
  clusterCall(parallel_clusters[indexCore], function(d) {assign('core_tasks', d, pos=.GlobalEnv)}, core_tasks)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('indexCore', d, pos=.GlobalEnv)}, indexCore)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('add_inFrontOf_progress_report_fileName', d, pos=.GlobalEnv)}, add_inFrontOf_progress_report_fileName)
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
    if (combined_core_tasks$Max_sigDEGs_abs_correlation[indexC] != tasks$Max_sigDEGs_abs_correlation[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$SigDEGs_correlation_method[indexC] != tasks$SigDEGs_correlation_method[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Cum_eigenexpression_fraction[indexC] != tasks$Cum_eigenexpression_fraction[indexC]) { cluster_generation_correct=FALSE;}
  }#End
}#End

if (!cluster_generation_correct)
{#Begin
  stopCluster(parallel_clusters) 
}#End


clusterEvalQ(parallel_clusters,
{#Begin - Parallel clusters

previous_dataset = "";
previous_permutation_no = -9999999
previous_add_fileName = "";
length_core_tasks = length(core_tasks[,1])

{#Begin - Initialize progress report
  complete_progress_report_fileName = paste(lincs_results_directory,add_inFrontOf_progress_report_fileName,"_progress_report_core",indexCore,".txt",sep='')
  
  Col_names = c("Time","Status","Core","Current_coreTask","Task_no","Total_coreTasks","Reference_valueType","Cutoff_rank","Permutation_no","SVD_pipeline",
                "Minimum_ratio_drug_cellline_minusLog10pvalue","Cum_eigenexpression_fraction",
                "Dataset","Correlation_method","Preprocess_data")
  Col_length = length(Col_names)
  Row_names = 1
  Row_length = length(Row_names)
  core_progress_report_base_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
  core_progress_report_base_line = as.data.frame(core_progress_report_base_line)
  core_progress_report_base_line$SVD_pipeline = "SVD_5b_validate_clusters_by_calculating_f1_scores"
  headline = paste(Col_names,collapse='\t')

  core_progress_line = core_progress_report_base_line
  core_progress_line$Core = indexCore
  core_progress_line$Current_coreTask = ""
  core_progress_line$Task_no = -1
  core_progress_line$Total_coreTasks = length_core_tasks
  core_progress_line$Dataset = ""
  core_progress_line$Reference_valueType = "NA"
  core_progress_line$Cutoff_rank = -1
  core_progress_line$Permutation_no = ""
  core_progress_line$Status = "Started"
  core_progress_line$Correlation_method = ""
  core_progress_line$Preprocess_data = ""
  core_progress_line$Time = Sys.time()
  core_progress_line$Cum_eigenexpression_fraction = ""
  core_progress_line$Time = Sys.time()

  write.table(headline,file=complete_progress_report_fileName,row.names=FALSE,sep='\t',quote=FALSE,col.names=FALSE,append=FALSE)  
  write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,sep='\t',quote=FALSE,col.names=FALSE,append=TRUE)  
}#End - Initialize progress report

## Document start of analysis - END

complete_progress_report_fileName = paste(lincs_results_directory,add_inFrontOf_progress_report_fileName,"_progress_report_core",indexCore,".txt",sep='')
write.table(headline,file=complete_progress_report_fileName,row.names=FALSE,quote=FALSE,col.names=FALSE,sep='\t')

previous_dataset=""
previous_dataset_coCulture=""
indexCoreTask=61
for (indexCoreTask in 1:length_core_tasks)
{#Begin - indexCore
  current_task_line = core_tasks[indexCoreTask,]
  dataset = current_task_line$Dataset
  dataset_coCulture =  current_task_line$Dataset_coCulture
  ea_correlation_parameter = current_task_line$Ea_correlation_parameter
  currentCoreTask_Entity = current_task_line$CoreTask_Entitity
  preprocess_data = current_task_line$Preprocess_data
  reference_valueType = current_task_line$Reference_valueType
  f1_score_weight = current_task_line$F1_score_weight
  permutation_no = current_task_line$Permutation_no
  addOrAnalyze_coCulture_data = current_task_line$AddOrAnalyze_coCulture_data
  cum_eigenexpression_fraction = current_task_line$Cum_eigenexpression_fraction
  sigDEGs_correlation_method = current_task_line$SigDEGs_correlation_method
  max_sigDEGs_abs_correlation = current_task_line$Max_sigDEGs_abs_correlation
  
  ## Set global parameter - BEGIN
  source('SVD_coreTaskSpecific_parameter.R')
  ## Set global parameter - END
  
  update_all_following_dataframes = FALSE
  if ((previous_dataset!=dataset)|(dataset_coCulture!=previous_dataset_coCulture)|(update_all_following_dataframes))
  {#Begin - SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites
     source('SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites.R')
     previous_dataset=dataset
     update_all_following_dataframes = TRUE
  }#End - SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites
  
  if ((dataset_coCulture!=previous_dataset_coCulture)|(update_all_following_dataframes))
  {#Begin - SVD_readPrepare_and_add_coCulture_degMatrix.R
     source('SVD_readPrepare_and_add_coCulture_degMatrix.R')
     previous_dataset_coCulture=dataset_coCulture
     update_all_following_dataframes = TRUE
  }#End - SVD_readPrepare_and_add_coCulture_degMatrix.R
  
  projected_data_type = full_dataType_label
  source("SVD_singleDrug_validations_define_and_initiate_singleDrug_validations.R")
  source("SVD_singleDrug_validations_set current_singleDrug_cluster_validation_base_line.R")

  if (projected_data_type==full_dataType_label)
  {#Begin
    Data_combined = Data
  }#End
  if (projected_data_type!=full_dataType_label)
  {#Begin
     stop(paste(projected_data_type," is not considered",sep=''))
  }#End
    
  {#Begin - Read collapsed_real_validations and eigenassay_statistics
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
  }#End - Read collapsed_real_validations and eigenassay_statistics
  
  {#Begin - Project dataWithCoCulture into selected subspaces and calculate F1 score statistics
    indexCurrentEntityCollapsed = which(collapsed_real_validations$Entity==currentCoreTask_Entity)
    current_collapsed_real_validation = collapsed_real_validations[indexCurrentEntityCollapsed,]
    eigenassays_count = unique(current_collapsed_real_validation$Eigenassays_count)
    if (length(eigenassays_count)!=1) { stop("(length(eigenassays_count)!=1)") }
    indexOutlier = which(current_collapsed_real_validation$Is_outlier==outlier_finalSelection_yes_label)
    outlierCellLine = current_collapsed_real_validation$Final_selection_for_sample[indexOutlier]
    
    currentEntity_eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Entity==currentCoreTask_Entity,]
    currentEntity_eigenassay_statistics = currentEntity_eigenassay_statistics[order(currentEntity_eigenassay_statistics[[ea_correlation_parameter]]),]
    current_eigenassays = currentEntity_eigenassay_statistics$Eigenassay[1:eigenassays_count]
    if (length(current_eigenassays)!=length(unique(current_eigenassays))) { stop("length(current_eigenassays)!=length(unique(current_eigenassays)") }
    #current_cum_eigensexpression_fraction = sum(currentEntity_eigenassay_statistics$Eigenexpression_fraction[1:eigenassays_count])
    if (length(current_eigenassays)>=3)
    {#Begin
       source("SVD_singleDrug_validations_calculate_and_add.R")
    }#End
    current_eigenassays = 1:unique(current_collapsed_real_validation$Eigenassays_count_full_data)
    if (length(current_eigenassays)>=3)
    {#Begin
      source("SVD_singleDrug_validations_calculate_and_add.R")
    }#End
  }#End - Project dataWithCoCulture into selected subspaces and calculate F1 score statistics

  ## Write results - BEGIN
  if (length(singleDrug_cluster_validations)!=0)
  {#Begin - Write results
    if (file.exists(core_complete_coCulture_entitySpecific_cluster_validation_f1_fileName))
    {#Begin
      write.table(singleDrug_cluster_validations,file=core_complete_coCulture_entitySpecific_cluster_validation_f1_fileName,quote = FALSE,row.names = FALSE,col.names=FALSE,append=TRUE,sep=delimiter)
    }#End
    else 
    {#Begin
      write.table(singleDrug_cluster_validations,file=core_complete_coCulture_entitySpecific_cluster_validation_f1_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
    }#End
  }#End - Write results
    
#### Validate cluster by calculating F1 score 4b - BEGIN##########################################################################################################################
##########################################################################################################################
##########################################################################################################################
{#Begin -Document finish of analysis - BEGIN

core_progress_line = core_progress_report_base_line
core_progress_line$Core = indexCore
core_progress_line$Current_coreTask = indexCoreTask
core_progress_line$Total_coreTasks = length_core_tasks
core_progress_line$Dataset = dataset
core_progress_line$Reference_valueType = "NA"
core_progress_line$Task_no = current_task_line$Task_no
core_progress_line$Cutoff_rank = -1
core_progress_line$Correlation_method = correlation_method
core_progress_line$Preprocess_data = preprocess_data
core_progress_line$Time = Sys.time()
core_progress_line$Cum_eigenexpression_fraction = cum_eigenexpression_fraction
core_progress_line$Status = "Finished"
#if (!save_and_delete_cluster_validations) { core_progress_line$Status = "Finished" }
#if (save_and_delete_cluster_validations) { core_progress_line$Status = "Finished and saved" }
core_progress_line$Time = Sys.time()

write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,sep='\t',quote=FALSE,col.names=FALSE,append=TRUE)  

}#End - Document finish of analysis - END
##########################################################################################################################
}#End - indexCoreTask

complete_finished_progress_report_fileName = gsub(".txt","_finished.txt",complete_progress_report_fileName)
file.rename(complete_progress_report_fileName,complete_finished_progress_report_fileName)

}#End parallel clusters - SVD_8
)

{#Begin - Close parallel clusters
  invisible(gc())
  parallel::stopCluster(parallel_clusters)
  invisible(gc())
  rm(parallel_clusters)
}#End - Close parallel clusters



for (indexGlobalTasks in 1:length(globally_assigned_tasks[,1]))
{#Begin - Combine individual core files of each dataset analysis
  current_global_task_line = globally_assigned_tasks[indexGlobalTasks,]
  current_task_line = current_global_task_line
  indexCore=0
  source('SVD_coreTaskSpecific_parameter.R')
  
  fileNames = list.files(coCulture_cluster_validation_f1_directory)
  combined_singleDrug_cluster_validations = c()
  for (indexBF in 1:length(fileNames))
  {#Begin
    fileName = fileNames[indexBF]
    if (grepl(entitySpecific_cluster_validation_f1_fileName,fileName))
    {#Begin
      complete_fileName = paste(coCulture_cluster_validation_f1_directory,fileName,sep='')
      current_singleDrug_cluster_validations = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep=delimiter)
      if (length(combined_singleDrug_cluster_validations)==0) { combined_singleDrug_cluster_validations = current_singleDrug_cluster_validations }
      else { combined_singleDrug_cluster_validations = rbind(combined_singleDrug_cluster_validations,current_singleDrug_cluster_validations)}
      #unlink(complete_fileName)
    }#End
  }#End
  if (length(combined_singleDrug_cluster_validations[,1]))
  {#Begin
    write.table(combined_singleDrug_cluster_validations,file=complete_coCulture_entitySpecific_cluster_validation_f1_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
}#End - Combine individual core files of each dataset analysis



