#stopCluster(parallel_clusters)
rm(list = ls());

get_eigenassays_per_dataset = TRUE
indexCore = -1
add_inFrontOf_progress_report_fileName = "SVD_2"
delete_task_reports = TRUE
delete_finished_analyses=TRUE
f1_score_weight=-1
source('SVD_global_parameter.R')

datasets = c()

if (delete_finished_analyses)
{#Begin - delete_finished_analyses
   for (indexGlobalTask in 1:length(globally_assigned_tasks[,1]))
   {#Begin - Delete existing result files
      current_task_line = globally_assigned_tasks[indexGlobalTask,]
      indexCore=0
      source('SVD_coreTaskSpecific_parameter.R')
      allFiles = list.files(angle_directory)
      for (indexAll in 1:length(allFiles))
      {#Begin
         unlink(paste(angle_directory,allFiles[indexAll],sep=''))
      }#End
   }#End - Delete existing result files
}#End - delete_finished_analyses

tasks = c()
current_task_no = 1

for (indexGlobalTask in 1:length(globally_assigned_tasks[,1]))
{#Begin - indexGlobalTask
  current_global_task_line = globally_assigned_tasks[indexGlobalTask,]
  dataset = current_global_task_line$Dataset
  eigenassay_count = dataset_eigenassays_count_list[[dataset]]
  Row_names = 1:eigenassay_count
  task_line_block = array(c(1,2),dim=c(eigenassay_count,Task_col_length),dimnames = list(Row_names,Task_col_names))
  task_line_block = as.data.frame(task_line_block)
  for (indexCol in 1:length(current_global_task_line[1,]))
  {#Begin
    task_line_block[,indexCol] = current_global_task_line[1,indexCol]
  }#End
  task_line_block$IndexEigenassay = 1:eigenassay_count
  end_task_no = current_task_no + eigenassay_count - 1
  task_line_block$Task_no = current_task_no:end_task_no
  current_task_no = end_task_no + 1;
  if (length(tasks)==0) { tasks = task_line_block}
  else { tasks = rbind(tasks,task_line_block)}
}#End - indexGlobalTask

tasks = tasks[order(tasks$Dataset),]
#core_tasks = tasks;indexCore=1;indexCoreTask=1

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

length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

for (indexCore in 1:cores_count)
{#Begin
  startIndex = min(floor((indexCore-1) * tasks_per_core+1),length_tasks);
  endIndex = min(floor(indexCore * tasks_per_core),length_tasks)
  core_tasks = tasks[startIndex:endIndex,]
  clusterCall(parallel_clusters[indexCore], function(d) {assign('core_tasks', d, pos=.GlobalEnv)}, core_tasks)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('indexCore', d, pos=.GlobalEnv)}, indexCore)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('add_inFrontOf_progress_report_fileName', d, pos=.GlobalEnv)}, add_inFrontOf_progress_report_fileName)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('lincs_results_directory', d, pos=.GlobalEnv)}, lincs_results_directory)
}#End

cluster_generation_correct = TRUE;

combined_core_tasks <- do.call('rbind', clusterEvalQ(parallel_clusters, core_tasks))
if (length(combined_core_tasks[,1]) != length_tasks) { cluster_generation_correct = FALSE }
if (cluster_generation_correct)
{#Begin
  for (indexC in 1:length(combined_core_tasks[,1]))
  {#Begin
    if (combined_core_tasks$Dataset[indexC] != tasks$Dataset[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Correlation_method [indexC] != tasks$Correlation_method[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Preprocess_data[indexC] != tasks$Preprocess_data[indexC]) { cluster_generation_correct=FALSE;}
  }#End
}#End

if (!cluster_generation_correct)
{#Begin
  stopCluster(parallel_clusters) 
}#End

clusterEvalQ(parallel_clusters,
{#Begin - Parallel clusters

  Col_names = c("Time","Status","Global_task_no","Core","Current_coreTask","Total_coreTasks","SVD_pipeline","Dataset","Preprocess_data","Correlation_method","Decomposition_method","Eigenassay","Total_eigenassay_count")
  Col_length = length(Col_names)
  Row_names = 1
  Row_length = length(Row_names)
  core_progress_report_base_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
  core_progress_report_base_line = as.data.frame(core_progress_report_base_line)
  core_progress_report_base_line$SVD_pipeline = "SVD_2_calculate_angels_and_correlations"
  headline = paste(Col_names,collapse='\t')

  complete_progress_report_fileName = paste(lincs_results_directory,add_inFrontOf_progress_report_fileName,"_progress_report_core",indexCore,".txt",sep='')
  write.table(headline,file=complete_progress_report_fileName,row.names=FALSE,quote=FALSE,col.names=FALSE,sep='\t')
  
  previous_dataset = "" 

  document_progress = unique(c(1,c(1:500)*10))
  indexNextDocument = 1

  length_core_tasks = length(core_tasks[,1])
  indexCoreTask=1
for (indexCoreTask in 1:length_core_tasks)
{#Begin
  
current_task_line = core_tasks[indexCoreTask,]
current_dataset = current_task_line$Dataset 
indexEigenassay = current_task_line$IndexEigenassay
f1_score_weight=-1


if (indexCoreTask==document_progress[indexNextDocument])
{#Begin - Document progress
  core_progress_line = core_progress_report_base_line
  core_progress_line$Core = indexCore
  core_progress_line$Global_task_no =  current_task_line$Task_no
  core_progress_line$Current_coreTask = indexCoreTask
  core_progress_line$Total_coreTasks = length_core_tasks
  core_progress_line$Dataset = current_dataset
  core_progress_line$Status = "Started"
  core_progress_line$Eigenassay = indexEigenassay;
  core_progress_line$Total_eigenassay_count = -1
  core_progress_line$Correlation_method = current_task_line$Correlation_method
  core_progress_line$Preprocess_data = current_task_line$Preprocess_data
  core_progress_line$Decomposition_method = current_task_line$Decomposition_method
  core_progress_line$Time = Sys.time()
  
  write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,quote=FALSE,col.names=FALSE,sep='\t',append=TRUE)
}#End - Document progress

## Set global parameter - BEGIN
source('SVD_coreTaskSpecific_parameter.R')
## Set global parameter - END

##########################################################################################################################
##########################################################################################################################
#Initiate angel colnames - BEGIN
Col_names = c("Eigenassay","BaseName","Columnname","Drug","DrugTargetClass","Cell_line","Correlation","Coefficient")

Col_length = length(Col_names)
Row_names = 1
Row_length = length(Row_names)
eigenassay_correlation_base_line = array(NA,dim=c(Row_length,Col_length),dimnames = list(Row_names,Col_names))
eigenassay_correlation_base_line = as.data.frame(eigenassay_correlation_base_line)
eigenassay_correlations = c();
#Initiate angel colnames - End
update_all_following_dataframes = FALSE
####################################################################################################################
if ((previous_dataset!=current_dataset)|(update_all_following_dataframes))
{#Begin - Read data and do dimensionality reduction
   sig_input_directory = degs_directory
   complete_degs_summary_fileName = paste(sig_input_directory,"DEG_summary.txt",sep='')
   source('SVD_colors.R')
   source('SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites.R')
   previous_dataset = current_dataset
   update_all_following_dataframes=TRUE
}#End - Read data and do dimensionality reduction
####################################################################################################################

## Cluster DEGs matrix and calculate angels and correlations with eigenassays - BEGIN

colnames_data = colnames(Data)
rownames_data = rownames(Data)

data_sum = c();
length_cols_data = length(Data[1,])

##########################################################################################################################
##Begin - Cluster DEGs matrix and calculate correlations and coefficients with eigenassaysindexColumn=1
for (indexColumn in 1:length_cols_data)
{#Begin - indexColumn
   data_col = Data[,indexColumn]
   source('SVD_get_current_coefficient_and_correlation_with_eigenassay.R')

   eigenassay_correlation_line = eigenassay_correlation_base_line
   eigenassay_correlation_line$Eigenassay = indexEigenassay
   eigenassay_correlation_line$BaseName = baseName
   current_columnname = colnames(Data)[indexColumn]
   eigenassay_correlation_line$Columnname = current_columnname
   splitStrings = strsplit(current_columnname,"[.]")[[1]]
   eigenassay_correlation_line$Drug = splitStrings[3]
   eigenassay_correlation_line$DrugTargetClass = ""
   eigenassay_correlation_line$Cell_line = splitStrings[2]
   eigenassay_correlation_line$Correlation = current_correlation
   eigenassay_correlation_line$Coefficient = coefficient
     
   if (length(eigenassay_correlations)==0) { eigenassay_correlations = eigenassay_correlation_line}
   else { eigenassay_correlations = rbind(eigenassay_correlations,eigenassay_correlation_line)}
}#End - indexColumn
##End - Cluster DEGs matrix and calculate correlations and coefficients with eigenassays
####################################################################################################################
if (indexCoreTask==document_progress[indexNextDocument])
{#Begin - Document progress
  core_progress_line = core_progress_report_base_line
  core_progress_line$Core = indexCore
  core_progress_line$Global_task_no =  current_task_line$Task_no
  core_progress_line$Current_coreTask = indexCoreTask
  core_progress_line$Total_coreTasks = length_core_tasks
  core_progress_line$Dataset = current_dataset
  core_progress_line$Status = "Finished"
  core_progress_line$Eigenassay = indexEigenassay
  core_progress_line$Total_eigenassay_count = length_eigenassays
  core_progress_line$Correlation_method = correlation_method
  core_progress_line$Preprocess_data = preprocess_data
  core_progress_line$Decomposition_method = decomposition_method
  core_progress_line$Time = Sys.time()
  
  write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,quote=FALSE,col.names=FALSE,sep='\t',append=TRUE)
  indexNextDocument = indexNextDocument + 1
}#End - Document progress
####################################################################################################################
## Write results - BEGIN
if (file.exists(core_complete_eigenassay_correlations_fileName))
{#Begin
  write.table(eigenassay_correlations,file=core_complete_eigenassay_correlations_fileName,quote = FALSE,row.names = FALSE,col.names=FALSE,append=TRUE,sep=delimiter)
}#End
else 
{#Begin
  write.table(eigenassay_correlations,file=core_complete_eigenassay_correlations_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
}#End
## Write results - END
####################################################################################################################

}#End - indexCoreTask

{#Begin - Label progress report fileNames as finished  
  complete_finished_progress_report_fileName = gsub(".txt","_finished.txt",complete_progress_report_fileName)
  file.rename(complete_progress_report_fileName,complete_finished_progress_report_fileName)
}#End - Label progress report fileNames as finished  
     
}#End - Parallel clusters - SVD_2
)

##########################################################################################################################

{#Begin - Close parallel clusters
   invisible(gc())
   parallel::stopCluster(parallel_clusters)
   invisible(gc())
   rm(parallel_clusters)
}#End - Close parallel clusters

##########################################################################################################################
##########################################################################################################################

indexGlobalTasks=2
for (indexGlobalTasks in 1:length(globally_assigned_tasks[,1]))
{#Begin - Combine individual core files of each dataset analysis
  current_global_task_line = globally_assigned_tasks[indexGlobalTasks,]
  current_task_line = current_global_task_line
  indexCore=0
  source('SVD_coreTaskSpecific_parameter.R')
  
  fileNames = list.files(angle_directory)
  combined_eigenassay_correlations = c()
  for (indexBF in 1:length(fileNames))
  {#Begin
     fileName = fileNames[indexBF]
     if (grepl(eigenassay_correlations_fileName_withoutExtension,fileName))
     {#Begin
        complete_fileName = paste(angle_directory,fileName,sep='')
        current_eigenassay_correlations = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep=delimiter)
        if (length(combined_eigenassay_correlations)==0) { combined_eigenassay_correlations = current_eigenassay_correlations }
        else { combined_eigenassay_correlations = rbind(combined_eigenassay_correlations,current_eigenassay_correlations)}
        unlink(complete_fileName)
     }#End
  }#End
  if (length(combined_eigenassay_correlations[,1]))
  {#Begin
    write.table(combined_eigenassay_correlations,file=complete_eigenassay_correlations_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
}#End - Combine individual core files of each dataset analysis

