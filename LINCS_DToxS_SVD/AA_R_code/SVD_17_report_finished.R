#stopCluster(parallel_clusters)
rm(list = ls());
#######################################################################################################################
## Generate tasks - BEGIN
get_eigenassays_per_dataset = FALSE
delete_task_reports = TRUE
add_inFrontOf_progress_report_fileName = "SVD_17"
source('SVD_global_parameter.R')

finished = "2nd part finished by R"
write.table(finished,file=complete_report_finished_2nd_part_fileName,quote=FALSE,sep='\t')
