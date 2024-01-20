####################################################################################################################
#Open libraries and document versions - BEGIN
Col_names = c("Library","Version")
Col_length = length(Col_names)
Row_names = 1
Row_length= length(Row_names)
version_documentation_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
version_documentation_line = as.data.frame(version_documentation_line)
version_documentations = c()

libraries = c("ClassDiscovery","colorspace","dendextend","circlize","dixonTest","ape","fields","colormap","beeswarm","gplots","grid","gridExtra","ggplot2","genefilter","doParallel")
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

Col_names = c("Dataset","Correlation_method","Preprocess_data")
Col_length = length(Col_names)
Row_names = 1
Row_length = length(Row_names)
task_base_line = array(NA,dim=c(Row_length,Col_length),dimnames = list(Row_names,Col_names))
task_base_line = as.data.frame(task_base_line)

tasks = c()

delete_task_reports=TRUE
add_inFrontOf_progress_report_fileName="SVD7"
get_eigenassays_per_dataset=FALSE
f1_score_weight=-1
source('SVD_global_parameter.R')

indexCV = grep("_CVno",globally_assigned_tasks$Dataset)
indexNonCV = 1:length(globally_assigned_tasks[,1])
indexNonCV = indexNonCV[!indexNonCV %in% indexCV]
tasks = globally_assigned_tasks[indexNonCV,]
tasks$Add_coCulture_data = FALSE



## Generate tasks - BEGIN
#######################################################################################################################

length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count
