get_eigenassays_per_dataset = FALSE
delete_task_reports = TRUE
add_inFrontOf_progress_report_fileName = "SVD_A"
source('SVD_global_parameter.R')
indexIteration=0
while (!file.exists(complete_report_finished_3rd_part_fileName))
{#Begin
  print(paste("Waiting for C# script to finish 3rd part (already waited for ",waiting_time_in_minutes_if_csharp_not_finished * indexIteration," min)",sep=''))
  indexIteration=indexIteration+1
  Sys.sleep(waiting_time_in_minutes_if_csharp_not_finished*60)
}#End