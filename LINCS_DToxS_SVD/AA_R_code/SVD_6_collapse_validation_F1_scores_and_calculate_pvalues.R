#stopCluster(parallel_clusters)
rm(list = ls());

#######################################################################################################################
## Generate tasks - BEGIN

get_eigenassays_per_dataset = FALSE
indexCore = -1
add_inFrontOf_progress_report_fileName = "SVD_6"
delete_task_reports = TRUE
f1_score_weight=-1
source('SVD_global_parameter.R')
tasks = c()

globally_assigned_tasks$Unique_identifier = paste(globally_assigned_tasks$Dataset,globally_assigned_tasks$Correlation_method,globally_assigned_tasks$Preprocess_data,globally_assigned_tasks$Decomposition_method,sep='-')

f1_score_weights = c((0:19)/20)
tasks = c()
for (indexF1 in 1:length(f1_score_weights))
{#Begin
  add_tasks = globally_assigned_tasks
  add_tasks$F1_score_weight = f1_score_weights[indexF1]
  if (length(tasks)==0) { tasks = add_tasks }
  else { tasks = rbind(tasks,add_tasks) }
}#End
tasks$AddOrAnalyze_coCulture_data = FALSE

indexDocument = which(tasks$F1_score_weight == 0.95)
tasks$Document_F1_scores_cosine_similarites=FALSE
tasks$Document_F1_scores_cosine_similarites[indexDocument] = TRUE

## Generate tasks - END
#######################################################################################################################

source("SVD_collapse_validation_F1_scores_and_calculate_pvalues_shared.R")
