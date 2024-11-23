#stopCluster(parallel_clusters)
rm(list = ls());

#######################################################################################################################
## Generate tasks - BEGIN

get_eigenassays_per_dataset = FALSE
indexCore = -1
#remove_accomplished_tasks_from_task_list = TRUE
delete_finished_analyses = TRUE
delete_task_reports = TRUE
add_inFrontOf_progress_report_fileName = "SVD_5c"
f1_score_weight=-1
source('SVD_global_parameter.R')
tasks = c()

sigDEGs_correlation_methods = c("Pearson")
max_sigDEGs_abs_correlations = c(1)
cum_eigenexpression_fractions = c(1)

ea_correlation_parameters = c("Ttest_pvalue")
cutoff_ranks = paste(c(3:325),collapse=' ')


Col_names = c("Task_no","Dataset","Correlation_method","Preprocess_data",
              "Decomposition_method")

current_task_no = 1;
tasks = c()
indexGlobal = 1
for (indexGlobal in 1:length(globally_assigned_tasks[,1]))
{#Begin
  current_global_task_line = globally_assigned_tasks[indexGlobal,]
  current_task_line = current_global_task_line
  source('SVD_coreTaskSpecific_parameter.R')
  summary_local = read.table(file=complete_degs_summary_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
  coreTask_Entities = c()
  if (length(unique(summary_local$Treatment))>1)
  { coreTask_Entities = c(coreTask_Entities,unique(summary_local$Treatment)) }
  if (length(unique(summary_local$Cell_line))>1)
  { coreTask_Entities = c(coreTask_Entities,unique(summary_local$Cell_line)) }
  coreTask_Entities = gsub("Cell_line.","",coreTask_Entities)
  for (indexCorPar in 1:length(ea_correlation_parameters))
  {#Begin
     ea_correlation_parameter = ea_correlation_parameters[indexCorPar]
     sigDEGs_correlation_method = sigDEGs_correlation_methods[1]
     max_sigDEGs_abs_correlation = max_sigDEGs_abs_correlations[1]
     for (indexCum in 1:length(cum_eigenexpression_fractions))
     {#Begin
        cum_eigenexpression_fraction = cum_eigenexpression_fractions[indexCum]
   
        Row_names = 1:length(coreTask_Entities)
        task_line_block = array(c(1,2),dim=c(length(coreTask_Entities),Task_col_length),dimnames = list(Row_names,Task_col_names))
        task_line_block = as.data.frame(task_line_block)
        for (indexCol in 1:length(current_global_task_line[1,]))
        {#Begin
          task_line_block[,indexCol] = current_global_task_line[1,indexCol]
        }#End
        end_task_no = current_task_no + length(coreTask_Entities) - 1
        task_line_block$Task_no = current_task_no:end_task_no
        task_line_block$Cum_eigenexpression_fraction = cum_eigenexpression_fraction
        task_line_block$Max_sigDEGs_abs_correlation = max_sigDEGs_abs_correlation
        task_line_block$SigDEGs_correlation_method = sigDEGs_correlation_method
        task_line_block$EA_correlation_parameter = ea_correlation_parameter
        task_line_block$CoreTask_Entitity = coreTask_Entities
        task_line_block$Cutoff_ranks = paste(cutoff_ranks,collapse=' ')
        if (length(tasks)==0) { tasks = task_line_block}
        else { tasks = rbind(tasks,task_line_block)}
        current_task_no = end_task_no + 1;
     }#End
  }#End
}#End

tasks$AddOrAnalyze_coCulture_data = FALSE

## Generate tasks - END
#######################################################################################################################

if (delete_finished_analyses)
{#Begin - Delete finished analyses
  for (indexGlobalTask in 1:length(globally_assigned_tasks[,1]))
  {#Begin - Delete existing result files
    current_task_line = globally_assigned_tasks[indexGlobalTask,]
    indexCore=0
    source('SVD_coreTaskSpecific_parameter.R')
    allFiles = list.files(cluster_validation_f1_directory)
    for (indexAll in 1:length(allFiles))
    {#Begin
      unlink(paste(cluster_validation_f1_directory,allFiles[indexAll],sep=''))
    }#End
  }#End - Delete existing result files
}#End - Delete finished analyses

#Remove accomplished tasks - End

length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

#core_tasks = tasks; indexCore=1;indexCoreTask=1034



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

indexCoreTask=3
for (indexCoreTask in 1:length_core_tasks)
{#Begin - indexCore

progressReport = paste("core task no ",indexCoreTask," initiated",sep='')  
   
current_task_line = core_tasks[indexCoreTask,]  
f1_score_weight=-1

##########################################################################################################################
## Set global parameter - BEGIN
source('SVD_coreTaskSpecific_parameter.R')
## Set global parameter - END
##########################################################################################################################

dataset = current_task_line$Dataset
currentCoreTask_Entity = current_task_line$CoreTask_Entitity
max_sigDEGs_abs_correlation = current_task_line$Max_sigDEGs_abs_correlation
correlation_method = current_task_line$Correlation_method
sigDEGs_correlation_method = current_task_line$SigDEGs_correlation_method
cum_eigenexpression_fraction_cutoff = current_task_line$Cum_eigenexpression_fraction
ea_correlation_parameter = current_task_line$EA_correlation_parameter
cutoff_ranks = as.numeric(strsplit(current_task_line$Cutoff_ranks," ")[[1]])

##########################################################################################################################
{#Begin -Document start of analysis - BEGIN
  
  core_progress_line = core_progress_report_base_line
  core_progress_line$Task_no = current_task_line$Task_no
  core_progress_line$Core = indexCore
  core_progress_line$Current_coreTask = indexCoreTask
  core_progress_line$Total_coreTasks = length_core_tasks
  core_progress_line$Dataset = dataset
  core_progress_line$Correlation_method = correlation_method
  core_progress_line$Preprocess_data = preprocess_data
  core_progress_line$Time = Sys.time()
  core_progress_line$Cum_eigenexpression_fraction = cum_eigenexpression_fraction_cutoff
  core_progress_line$Status = "Started"
  #if (!save_and_delete_cluster_validations) { core_progress_line$Status = "Finished" }
  #if (save_and_delete_cluster_validations) { core_progress_line$Status = "Finished and saved" }
  core_progress_line$Time = Sys.time()
  
  write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,sep='\t',quote=FALSE,col.names=FALSE,append=TRUE)  
  
}#End - Document start of analysis - END
##########################################################################################################################
update_all_following_dataframes = FALSE
if ((previous_dataset!=dataset)|(update_all_following_dataframes))
{#Begin - SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites
  source('SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites.R')
  previous_dataset=dataset
  projected_data_types  = full_dataType_label
  update_all_following_dataframes = TRUE
}#End - SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites

if ((add_fileName!=previous_add_fileName)|(update_all_following_dataframes))
{#Begin - Read eigenassay_correlation files
  eigenassay_correlation_statistics = read.table(file=complete_eigenassay_correlation_statistics_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
  update_all_following_dataframes=TRUE
  previous_add_fileName = add_fileName
}#End - Read eigenassay_correlation files
##########################################################################################################################

progressReport = paste("core task no ",indexCoreTask," saved files read",sep='')  

#### Read angle analysis data - End
##########################################################################################################################
reference_valueTypes = unique(eigenassay_correlation_statistics$Reference_valueType)

source("SVD_singleDrug_validations_define_and_initiate_singleDrug_validations.R")

indexProjected_data=1
for (indexProjected_data in 1:length(projected_data_types))
{#Begin - Projected data type
projected_data_type = projected_data_types[indexProjected_data]
if (projected_data_type==full_dataType_label)
{#Begin
   Data_combined = Data
}#End
if (projected_data_type!=full_dataType_label)
{ stop(paste(projected_data_type," is not considered",sep='')) }
  
indexReferenceValueType = 1
for (indexReferenceValueType in 1:length(reference_valueTypes))
{#Begin - indexReferenceValueType
  reference_valueType = reference_valueTypes[indexReferenceValueType]
  indexCurrent = which(eigenassay_correlation_statistics$Reference_valueType==reference_valueType);
  referenceValueType_eigenassay_correlation_statistics = eigenassay_correlation_statistics[indexCurrent,]

##########################################################################################################################
#### Validate cluster by calculating F1 score - BEGIN
source("SVD_singleDrug_validations_set current_singleDrug_cluster_validation_base_line.R")

indexCurrentEntity = which(referenceValueType_eigenassay_correlation_statistics$Entity == currentCoreTask_Entity)
currentEntity_eigenassay_correlation_statistics = referenceValueType_eigenassay_correlation_statistics[indexCurrentEntity,]

currentEntity_eigenassay_correlation_statistics$Rank = rank(currentEntity_eigenassay_correlation_statistics[[ea_correlation_parameter]])

{#Begin - Identify max rank in data and remove all cutoff ranks above max rank (add max rank)
max_rank_in_data = max(currentEntity_eigenassay_correlation_statistics$Rank)
indexCutoffsAboveMax = which(cutoff_ranks > max_rank_in_data)
indexCutoffsEqualMax = which(cutoff_ranks == max_rank_in_data)
if (length(indexCutoffsAboveMax)>0)
{#Begin
  indexKeepCutoffs = which(cutoff_ranks <= max_rank_in_data)
  cutoff_ranks = cutoff_ranks[indexKeepCutoffs]
  if (length(indexCutoffsEqualMax)==0)
  { cutoff_ranks = c(cutoff_ranks,max_rank_in_data) }
}#End
}#End - Identify max rank in data and remove all cutoff ranks above max rank (add max rank)

number_of_reports = 3
report_cutoff_ranks = cutoff_ranks[(1:number_of_reports)*(length(cutoff_ranks)/number_of_reports)]

length_cutoff_ranks = length(cutoff_ranks)
indexRankCutoff=1
for (indexRankCutoff in 1:length_cutoff_ranks)
{#Begin - rank cutoff
   rank_cutoff = cutoff_ranks[indexRankCutoff]
   if (rank_cutoff %in% report_cutoff_ranks)
   {#Begin - Document start of report cutoff rank
     
     core_progress_line = core_progress_report_base_line
     core_progress_line$Reference_valueType = reference_valueType
     core_progress_line$Cutoff_rank = rank_cutoff
     core_progress_line$Core = indexCore
     core_progress_line$Current_coreTask = indexCoreTask
     core_progress_line$Task_no = current_task_line$Task_no
     core_progress_line$Total_coreTasks = length_core_tasks
     core_progress_line$Dataset = dataset
     core_progress_line$Correlation_method = correlation_method
     core_progress_line$Preprocess_data = preprocess_data
     core_progress_line$Time = Sys.time()
     core_progress_line$Cum_eigenexpression_fraction = cum_eigenexpression_fraction_cutoff
     core_progress_line$Status = "Started"
     #if (!save_and_delete_cluster_validations) { core_progress_line$Status = "Finished" }
     #if (save_and_delete_cluster_validations) { core_progress_line$Status = "Finished and saved" }
     core_progress_line$Time = Sys.time()
     
     write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,sep='\t',quote=FALSE,col.names=FALSE,append=TRUE)  
     
   }#End - Document start of report cutoff rank

   indexEqualBelowCutoff = which(currentEntity_eigenassay_correlation_statistics$Rank<=rank_cutoff)
   entity_eigenassays = currentEntity_eigenassay_correlation_statistics$Eigenassay[indexEqualBelowCutoff]
   round_factor = 1E12
   keep_eigenassays = which((round(cum_eigensexpression_fraction*round_factor)/round_factor)<=cum_eigenexpression_fraction_cutoff)
   current_eigenassays = unique(keep_eigenassays[keep_eigenassays %in% entity_eigenassays])
   #current_eigenassays = current_eigenassays[!current_eigenassays %in% sigDEGs_eigenassays]
   if (length(current_eigenassays)>=3)
   {#Begin
      source("SVD_singleDrug_validations_calculate_and_add.R")
   }#End 
}#End

}#End - indexReferenceValueType

}#End - Projected data type
  

## Write results - BEGIN
if (length(singleDrug_cluster_validations)!=0)
{#Begin - Write results
  if (file.exists(core_complete_entitySpecific_cluster_validation_f1_fileName))
  {#Begin
    write.table(singleDrug_cluster_validations,file=core_complete_entitySpecific_cluster_validation_f1_fileName,quote = FALSE,row.names = FALSE,col.names=FALSE,append=TRUE,sep=delimiter)
  }#End
  else 
  {#Begin
    write.table(singleDrug_cluster_validations,file=core_complete_entitySpecific_cluster_validation_f1_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
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
core_progress_line$Cum_eigenexpression_fraction = cum_eigenexpression_fraction_cutoff
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

}#End parallel clusters - SVD_5
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
  
  fileNames = list.files(cluster_validation_f1_directory)
  combined_singleDrug_cluster_validations = c()
  for (indexBF in 1:length(fileNames))
  {#Begin
    fileName = fileNames[indexBF]
    if (grepl(entitySpecific_cluster_validation_f1_fileName,fileName))
    {#Begin
      complete_fileName = paste(cluster_validation_f1_directory,fileName,sep='')
      current_singleDrug_cluster_validations = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep=delimiter)
      if (length(combined_singleDrug_cluster_validations)==0) { combined_singleDrug_cluster_validations = current_singleDrug_cluster_validations }
      else { combined_singleDrug_cluster_validations = rbind(combined_singleDrug_cluster_validations,current_singleDrug_cluster_validations)}
      unlink(complete_fileName)
    }#End
  }#End
  if (length(combined_singleDrug_cluster_validations[,1]))
  {#Begin
    write.table(combined_singleDrug_cluster_validations,file=complete_entitySpecific_cluster_validation_f1_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
}#End - Combine individual core files of each dataset analysis



