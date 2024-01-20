#stopCluster(parallel_clusters)
rm(list = ls());
#######################################################################################################################
## Generate tasks - BEGIN
get_eigenassays_per_dataset = FALSE
delete_task_reports = TRUE
add_inFrontOf_progress_report_fileName = "SVD_B_e"
source('SVD_global_parameter.R')
finished = "6th part finished by R"
write.table(finished,file=complete_report_finished_6th_part_fileName,quote=FALSE,sep='\t')
