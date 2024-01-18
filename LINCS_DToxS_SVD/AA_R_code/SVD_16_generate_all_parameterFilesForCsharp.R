#stopCluster(parallel_clusters)
rm(list = ls());
#######################################################################################################################
## Generate tasks - BEGIN
get_eigenassays_per_dataset = FALSE
delete_task_reports = TRUE
add_inFrontOf_progress_report_fileName = "SVD_16"
source('SVD_global_parameter.R')
source('SVD_colors.R')
library(dendextend)

minimum_fractions_of_max_f1score = c(1)
ea_correlation_parameters = c("Ttest_pvalue")
entityClasses = c("Drug")
reference_valueTypes = c("Coefficient_of_eigenassay")

globally_assigned_tasks$AddOrAnalyze_coCulture_data = FALSE
cc_tasks = globally_assigned_tasks
cc_tasks$AddOrAnalyze_coCulture_data = TRUE
tasks = rbind(globally_assigned_tasks,cc_tasks)

f1_score_weight = -1;#-1 indicates outlier based f1soceweight selection, 0.95

parameter_completeRColors_fileNames = c()
parameter_completeAUCCutoffRansk_fileNames = c()

indexTasks=1
for (indexTasks in 1:length(tasks[,1]))
{#Begin - indexGlobalTasks
   current_task_line = tasks[indexTasks,]
   indexCore=1
   source('SVD_coreTaskSpecific_parameter.R')
   parameter_completeRColors_fileNames = c(parameter_completeRColors_fileNames,complete_rcolores_fileName)
   parameter_completeAUCCutoffRansk_fileNames = c(parameter_completeAUCCutoffRansk_fileNames,complete_aucCutoffRank_fileName)
}#End

parameter_completeRColors_fileNames = unique(parameter_completeRColors_fileNames)
for (indexPC in 1:length(parameter_completeRColors_fileNames))
{#Begin
  parameter_completeRColors_fileName = parameter_completeRColors_fileNames[indexPC]
  write.table(entity_rColors,file=parameter_completeRColors_fileName,col.names=TRUE,row.names=FALSE,quote=FALSE,sep='\t')
}#End

parameter_completeAUCCutoffRansk_fileNames = unique(parameter_completeAUCCutoffRansk_fileNames)
for (indexPC in 1:length(parameter_completeAUCCutoffRansk_fileNames))
{#Begin
  parameter_completeAUCCutoffRansk_fileName = parameter_completeAUCCutoffRansk_fileNames[indexPC]
  write.table(ontology_AUC_cutoff_ranks,file=parameter_completeAUCCutoffRansk_fileName,col.names=TRUE,row.names=FALSE,quote=FALSE,sep='\t')
}#End

## Generate tasks - End
#######################################################################################################################

