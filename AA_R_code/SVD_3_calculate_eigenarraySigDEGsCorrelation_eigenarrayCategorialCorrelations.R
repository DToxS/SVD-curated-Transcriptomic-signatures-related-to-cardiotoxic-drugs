#stopCluster(parallel_clusters)
rm(list = ls());
#######################################################################################################################
## Generate tasks - BEGIN

indexCore = -1
get_eigenassays_per_dataset = TRUE
add_inFrontOf_progress_report_fileName = "SVD_3"
read_finished_analyses_and_remove_from_task_list = TRUE
delete_task_reports = TRUE
delete_finished_analyses=TRUE
f1_score_weight=-1
source('SVD_global_parameter.R') #commented, so that script is not run by accident

if (delete_finished_analyses)
{#Begin
  for (indexGlobalTask in 1:length(globally_assigned_tasks[,1]))
  {#Begin - Delete existing result files
    current_task_line = globally_assigned_tasks[indexGlobalTask,]
    indexCore=0
    source('SVD_coreTaskSpecific_parameter.R')
    allFiles = list.files(statistics_enrichmentScore_directory)
    for (indexAll in 1:length(allFiles))
    {#Begin
      unlink(paste(statistics_enrichmentScore_directory,allFiles[indexAll],sep=''))
    }#End
  }#End - Delete existing result files
}#End

tasks = c()

current_task_no = 1;
indexGloballyAssignedTask=38
for (indexGloballyAssignedTask in 1:length(globally_assigned_tasks[,1]))
{#Begin - Generate local task_lines
  current_globally_assigned_task = globally_assigned_tasks[indexGloballyAssignedTask,]
  dataset = current_globally_assigned_task$Dataset
  eigenassays_count = -1
  eigenassays_count = dataset_eigenassays_count_list[[dataset]]

  Row_names = 1:eigenassays_count
  task_line_block = array(c(1,2),dim=c(eigenassays_count,Task_col_length),dimnames = list(Row_names,Task_col_names))
  task_line_block = as.data.frame(task_line_block)
  for (indexCol in 1:length(current_globally_assigned_task[1,]))
  {#Begin
    task_line_block[,indexCol] = current_globally_assigned_task[1,indexCol]
  }#End
  end_task_no = current_task_no + eigenassays_count - 1
  task_line_block$IndexEigenassay = 1:eigenassays_count
  task_line_block$Task_no = current_task_no:end_task_no
  if (length(tasks)==0) { tasks = task_line_block}
  else { tasks = rbind(tasks,task_line_block)}
  current_task_no = end_task_no + 1;
}#End - Generate local task_lines

## Generate tasks - BEGIN
#######################################################################################################################

length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

tasks = tasks[order(tasks$Dataset),]

#core_tasks = tasks; indexCore=1;indexCoreTask=1


parallel_clusters = makeCluster(cores_count)
clusterEvalQ(parallel_clusters, {
  library("ClassDiscovery")
  library("colorspace")
  library("dendextend")
  library("ape")
  library("ART")
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
  clusterCall(parallel_clusters[indexCore], function(d) {assign('add_inFrontOf_progress_report_fileName', d, pos=.GlobalEnv)}, add_inFrontOf_progress_report_fileName)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('lincs_results_directory', d, pos=.GlobalEnv)}, lincs_results_directory)
}#End


cluster_generation_correct = TRUE;

combined_core_tasks <- do.call('rbind', clusterEvalQ(parallel_clusters, core_tasks))
if (length(combined_core_tasks[,1])!= length_tasks) { cluster_generation_correct = FALSE }

combined_core_tasks = combined_core_tasks[order(combined_core_tasks$Task_no),]
tasks = tasks[order(tasks$Task_no),]

if (cluster_generation_correct)
{#Begin
  for (indexC in 1:length(combined_core_tasks[,1]))
  {#Begin
    if (combined_core_tasks$Dataset[indexC] != tasks$Dataset[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Correlation_method[indexC] != tasks$Correlation_method[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Preprocess_data[indexC] != tasks$Preprocess_data[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$IndexEigenassay[indexC] != tasks$IndexEigenassay[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Task_no[indexC] != tasks$Task_no[indexC]) { cluster_generation_correct=FALSE;}
    if (cluster_generation_correct==FALSE) { break }
  }#End
}#End

if (!cluster_generation_correct)
{#Begin
  stopCluster(parallel_clusters) 
}#End

clusterEvalQ(parallel_clusters,
{#Begin - Parallel clusters

  previous_dataset = ""
  previous_permutation_no = -999999999
  
  length_core_tasks = length(core_tasks[,1])

  {#Begin - Define progress report parameter and initiate report files
     Col_names = c("Time","Status","Core","Current_coreTask","Total_coreTasks","Permutation_no","SVD_pipeline",
                   "Cum_eigenexpression_fraction","Dataset","Correlation_method","Preprocess_data","IndexEigenassay")
     Col_length = length(Col_names)
     Row_names = 1
     Row_length = length(Row_names)
     core_progress_report_base_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
     core_progress_report_base_line = as.data.frame(core_progress_report_base_line)
     core_progress_report_base_line$SVD_pipeline = "SVD_3_calculate_eigenarraySigDEGsCorrelation_eigenarrayCategorialCorrelations"
     headline = paste(Col_names,collapse='\t')

     complete_progress_report_fileName = paste(lincs_results_directory,add_inFrontOf_progress_report_fileName,"_progress_report_core",indexCore,".txt",sep='')
     write.table(headline,file=complete_progress_report_fileName,row.names=FALSE,quote=FALSE,col.names=FALSE,sep='\t')

     core_progress_line = core_progress_report_base_line
     core_progress_line$Core = indexCore
     core_progress_line$Current_coreTask = 0
     core_progress_line$Total_coreTasks = length_core_tasks
     core_progress_line$Dataset = ""
     core_progress_line$Status = "Start analysis"
     core_progress_line$Permutation_no = 0
     core_progress_line$Correlation_method = core_tasks$Correlation_method[1]
     core_progress_line$Preprocess_data = core_tasks$Preprocess_data[1]
     core_progress_line$Decomposition_method = core_tasks$Decomposition_method[1]
     core_progress_line$Time = Sys.time()
     core_progress_line$IndexEigenassay = core_tasks$IndexEigenassay[1]
     write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,quote=FALSE,col.names=FALSE,sep='\t',append=TRUE)
     
     document_progress = unique(c(1,c(1:5000),length_core_tasks))
     document_progress = document_progress[order(document_progress)]
     indexNextDocument = 1
  }#End - Define progress report parameter and initiate report files
    
indexCoreTask=1  
for (indexCoreTask in 1:length_core_tasks)
{#Begin - indexCore

current_task_line = core_tasks[indexCoreTask,]  
indexEigenassay = current_task_line$IndexEigenassay
current_permutation_no = current_task_line$Permutation_no
dataset = current_task_line$Dataset
correlation_method = current_task_line$Correlation_method
decomposition_method = current_task_line$Decomposition_method
preprocess_data = current_task_line$Preprocess_data
calculate_faers_risk=FALSE
f1_score_weight=-1


if (indexCoreTask==document_progress[indexNextDocument])
{#Begin - Document progress
  core_progress_line = core_progress_report_base_line
  core_progress_line$Core = indexCore
  core_progress_line$Global_task_no = current_task_line$Task_no;
  core_progress_line$Current_coreTask = indexCoreTask
  core_progress_line$Total_coreTasks = length_core_tasks
  core_progress_line$Dataset = dataset
  core_progress_line$Status = "Started"
  core_progress_line$Permutation_no = current_permutation_no
  core_progress_line$Correlation_method = correlation_method
  core_progress_line$Preprocess_data = preprocess_data
  core_progress_line$Decomposition_method = decomposition_method
  core_progress_line$IndexEigenassay = indexEigenassay
  core_progress_line$Time = Sys.time()
  
  write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,quote=FALSE,col.names=FALSE,sep='\t',append=TRUE)
}#End - Document progress


## Set global parameter - BEGIN
source('SVD_coreTaskSpecific_parameter.R')
source('SVD_colors.R')
## Set global parameter - END

##########################################################################################################################

update_all_following_dataframes = FALSE
if ((dataset!=previous_dataset)|(update_all_following_dataframes))
{#Begin - Update datasets
  summary = read.table(file=complete_degs_summary_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
  if (length(unique(summary$Replicate_no))==1)
  {#Begin
    summary$Colname = paste(summary$Drug_type,gsub("Cell_line.","",summary$Cell_line),summary$Treatment,gsub("Plate.","P",summary$Plate),sep='.')
  }#End
  if (length(unique(summary$Replicate_no))>1)
  {#Begin
    summary$Colname = paste(summary$Drug_type,".",gsub("Cell_line.","",summary$Cell_line),".",summary$Treatment,".",gsub("Plate.","P",summary$Plate),"_repNo",summary$Replicate_no,sep='')
  }#End
  
  Colnames = summary$Colname
  Colnames = gsub("Cell_line_","",Colnames)
  Colnames = gsub("Plate_","P",Colnames)
  summary$Colname = Colnames

  source('SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites.R')
  previous_dataset = dataset
  update_all_following_dataframes=TRUE
}#End - Update datasets
##########################################################################################################################

##########################################################################################################################
if (update_all_following_dataframes)
{#Begin - Update eigenassay_correlations
  eigenassay_correlations = read.csv(file=complete_eigenassay_correlations_fileName,header=TRUE,stringsAsFactors = FALSE,sep=delimiter)
  update_all_following_dataframes=TRUE
}#End - Update eigenassay_correlations
##########################################################################################################################

##########################################################################################################################
##########################################################################################################################

baseName = unique(eigenassay_correlations$BaseName)
drugs = unique(eigenassay_correlations$Drug)
celllines = unique(eigenassay_correlations$Cell_line)
all_entities = list(); all_entityClasses = c()

indexCurrentEigenassay = which(eigenassay_correlations$Eigenassay==indexEigenassay)
currentEigenassay_correlations = eigenassay_correlations[indexCurrentEigenassay,]
eigenassay = indexEigenassay

if (length(unique(drugs))>1)
{#Begin
  all_entities[[length(all_entities)+1]] = drugs
  all_entityClasses = c(all_entityClasses,"Drug");
}#End
if (length(unique(celllines))>1)
{#Begin
  all_entities[[length(all_entities)+1]] = celllines
  all_entityClasses = c(all_entityClasses,"Cell_line");
}#End
not_considered_entities = c()

{#Begin - Define eigenassay_correlation_statistics
  Col_names = c("Dataset","Correlation_method","Preprocess_data",
                "Decomposition_method","Reference_valueType",
                "Eigenassay","Eigenexpression",
                "Eigenexpression_fraction","Cum_eigenexpression_fraction",
                "BaseName","Entity","EntityClass",
                "Correlation_mean","Correlation_median",
                "Correlation_sd","Correlation_coefVar",
                "Ttest_pvalue","Wilcoxtest_pvalue",
                "Ttest_minusLog10pvalue","Wilcoxtest_minusLog10pvalue")
  Col_length = length(Col_names)
  Row_names = 1;
  Row_length = length(Row_names)
  eigenassay_angles_statistic_base_line = as.data.frame(array(NA,c(Row_length,Col_length),dimnames = list(Row_names,Col_names)))
  eigenassay_correlation_statistics = c()
}#End - Define eigenassay_correlation_statistics

{#Begin - Calculate eigenassay correlation statistics
indexE=1
for (indexE in 1:length(all_entities))
{#Begin - Calculate eigenassay_correlation_statistics
  current_entities = all_entities[[indexE]]
  current_entityClass = all_entityClasses[[indexE]]
  indexEntity=1
  for (indexEntity in 1:length(current_entities))
  {#Begin
     current_entity = current_entities[indexEntity]
     if (current_entityClass=="Cell_line")
     { indexThisEntity = which(currentEigenassay_correlations$Cell_line==current_entity) }
     if (current_entityClass=="Drug")
     { indexThisEntity = which(currentEigenassay_correlations$Drug==current_entity) }
     indexOtherEntity = 1:length(currentEigenassay_correlations[,1])
     indexOtherEntity = indexOtherEntity[!indexOtherEntity %in% indexThisEntity]
     thisEntity_correlations = currentEigenassay_correlations[indexThisEntity,]
     otherEntity_correlations = currentEigenassay_correlations[indexOtherEntity,]
     if (length(thisEntity_correlations[1,])==0) { not_considered_entities = c(not_considered_entities,current_entity) }
     if (length(thisEntity_correlations[1,])>0)
     {#Begin - if  (length(thisEntity_correlations[1,])>0)
       current_coefficients = thisEntity_correlations$Coefficient
       current_correlations = thisEntity_correlations$Correlation
       current_angles = thisEntity_correlations$Angle
       current_smalles_angels = thisEntity_correlations$Angle_smallest
       other_correlations = otherEntity_correlations$Correlation
       other_coefficients = otherEntity_correlations$Coefficient
       eigenassay_angles_statistic_inner_base_line = eigenassay_angles_statistic_base_line
          
       eigenassay_angles_statistic_inner_base_line$Dataset = dataset
       eigenassay_angles_statistic_inner_base_line$Correlation_method = correlation_method
       eigenassay_angles_statistic_inner_base_line$Preprocess_data = preprocess_data
       eigenassay_angles_statistic_inner_base_line$Decomposition_method = decomposition_method
       eigenassay_angles_statistic_inner_base_line$Eigenassay = eigenassay
       eigenassay_angles_statistic_inner_base_line$Eigenexpression = eigenexpression[eigenassay]
       eigenassay_angles_statistic_inner_base_line$Eigenexpression_fraction = eigenexpression_fraction[eigenassay]
       eigenassay_angles_statistic_inner_base_line$Cum_eigenexpression_fraction = cum_eigensexpression_fraction[eigenassay]
       eigenassay_angles_statistic_inner_base_line$BaseName = baseName
       eigenassay_angles_statistic_inner_base_line$Entity = current_entity
       eigenassay_angles_statistic_inner_base_line$EntityClass = current_entityClass

       current_valueTypes = c("Correlation_with_eigenassay","Coefficient_of_eigenassay")
       current_this_values_array =  list(current_correlations,         current_coefficients)
       current_other_values_array = list(other_correlations,           other_coefficients)
       indexVT = 1
       for (indexVT in 1:length(current_valueTypes))
       {#Begin - curentValueType
          current_valueType = current_valueTypes[indexVT]
          current_this_values = current_this_values_array[[indexVT]]
          current_other_values = current_other_values_array[[indexVT]]
         
          if (length(current_this_values) + length(current_other_values)!=length(Data[1,])) { stop("(length(current_this_values) + length(current_other_values)!=length(Data[1,]))") }

          eigenassay_angles_statistic_line = eigenassay_angles_statistic_inner_base_line
          eigenassay_angles_statistic_line$Reference_valueType = current_valueType
          eigenassay_angles_statistic_line$Correlation_mean = mean(current_this_values)
          eigenassay_angles_statistic_line$Correlation_median = median(current_this_values)
          eigenassay_angles_statistic_line$Correlation_sd = sd(current_this_values)
          eigenassay_angles_statistic_line$Correlation_coefVar = eigenassay_angles_statistic_line$Correlation_sd / eigenassay_angles_statistic_line$Correlation_mean
          
          #mean_this = mean(current_this_values)
          #mean_other = mean(current_other_values)
          #sd_total = sd(c(current_this_values,current_other_values))
          #n_this = length(current_this_values)
          #n_other = length(current_other_values)
          #n_total = n_this + n_other
          #point_biserial_correlation = ((mean_this - mean_other)/sd_total) * sqrt((n_this*n_other)/n_total^2)
          #eigenassay_angles_statistic_line$Point_biserial_correlation = point_biserial_correlation
          if ((length(current_this_values)>=2)&(length(current_other_values)>=2))
          {#Begin
             t_test = t.test(current_this_values,current_other_values,alternative = "two.sided")
             wilcox_test = wilcox.test(current_this_values,current_other_values,alternative = "two.sided")
             eigenassay_angles_statistic_line$Ttest_pvalue = t_test$p.value
             eigenassay_angles_statistic_line$Wilcoxtest_pvalue = wilcox_test$p.value
          }#End
          else
          {#Begin
            eigenassay_angles_statistic_line$Ttest_pvalue = 1
            eigenassay_angles_statistic_line$Wilcoxtest_pvalue = 1
          }#End
          eigenassay_angles_statistic_line$Ttest_minusLog10pvalue = -log10(eigenassay_angles_statistic_line$Ttest_pvalue)
          eigenassay_angles_statistic_line$Wilcoxtest_minusLog10pvalue = -log10(eigenassay_angles_statistic_line$Wilcoxtest_pvalue)
          
          if (length(eigenassay_correlation_statistics)==0) {eigenassay_correlation_statistics = eigenassay_angles_statistic_line}
          else { eigenassay_correlation_statistics = rbind(eigenassay_correlation_statistics,eigenassay_angles_statistic_line)}
       }#End - curentValueType
     }#End - if  (length(thisEntity_correlations[1,])>0)
  }#End
}#End - Calculate eigenassay_correlation_statistics
}#End - Calculate eigenassay correlation statistics

{#Begin - Write eigenassay correlation statistics
  if (file.exists(core_complete_eigenassay_correlation_statistics_fileName))
  {#Begin
     write.table(eigenassay_correlation_statistics,file=core_complete_eigenassay_correlation_statistics_fileName,quote = FALSE,row.names = FALSE,col.names=FALSE,append=TRUE,sep=delimiter)
  }#End
  else 
  {#Begin
     write.table(eigenassay_correlation_statistics,file=core_complete_eigenassay_correlation_statistics_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
}#End - Write eigenassay correlation statistics

##########################################################################################################################
##########################################################################################################################

{#Begin - Define eigenassay_correlation_sigDEGs_correlation
  Col_names = c("Dataset","Correlation_method","Preprocess_data","Decomposition_method","Reference_valueType",
                "Eigenassay","BaseName","Pearson_correlation","Spearman_correlation","Kendall_correlation",
                "Pearson_correlation_abs","Spearman_correlation_abs","Kendall_correlation_abs",
                "Eigenexpression","Eigenexpression_fraction","Cum_eigenexpression_fraction")
  Col_length = length(Col_names)
  Row_names = 1;
  Row_length = length(Row_names)
  eigenassay_correlation_sigDEGs_correlation_base_line = as.data.frame(array(NA,c(Row_length,Col_length),dimnames = list(Row_names,Col_names)))
  eigenassay_correlation_sigDEGs_correlation = c()
}#End - Define eigenassay_correlation_sigDEGs_correlation

{#Begin - Calculate eigenassay_correlation_sigDEGs_correlation
  currentEigenassay_correlations = currentEigenassay_correlations[order(currentEigenassay_correlations$Columnname),]
  plate_summary = summary
  indexKeep = which(plate_summary$Colname %in% currentEigenassay_correlations$Columnname)
  indexRemove = which(!plate_summary$Colname %in% currentEigenassay_correlations$Columnname)
  if (length(indexRemove)!=0) {rm(plate_summary)}
  plate_summary = plate_summary[indexKeep,]
  plate_summary = plate_summary[order(plate_summary$Colname),]
  
  equal_columnNames = currentEigenassay_correlations$Columnname==plate_summary$Colname
  if ((length(equal_columnNames)!=1)&(equal_columnNames[1]==FALSE)) { rm(currentEigenassay_correlations) }
  
  current_correlations = currentEigenassay_correlations$Correlation
  current_coefficients = currentEigenassay_correlations$Coefficient
  signficant_degs_count = plate_summary$Significant_degs_based_on_FDR_count

  eigenassay_correlation_sigDEGs_correlation_inner_base_line = eigenassay_correlation_sigDEGs_correlation_base_line
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$Dataset = dataset
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$Correlation_method = correlation_method
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$Preprocess_data = preprocess_data
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$Decomposition_method = decomposition_method
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$Eigenassay = indexEigenassay
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$Eigenexpression = eigenexpression[indexEigenassay]
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$Eigenexpression_fraction = eigenexpression_fraction[indexEigenassay]
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$Cum_eigenexpression_fraction = cum_eigensexpression_fraction[indexEigenassay]
  baseName = unique(currentEigenassay_correlations$BaseName)
  if (length(baseName)>1) { paste(baseName,col=";",sep='') }
  eigenassay_correlation_sigDEGs_correlation_inner_base_line$BaseName = baseName;

  current_valueTypes =      c("Correlation_with_eigenassay","Coefficient_of_eigenassay")
  current_values_array = list(current_correlations,          current_coefficients)

  indexVT=1
  for (indexVT in 1:length(current_valueTypes))
  {#Begin
     current_valueType =  current_valueTypes[indexVT]
     current_values = current_values_array[[indexVT]]
  
     current_pearson_correlation = cor(signficant_degs_count,current_values,method="pearson")
     current_spearman_correlation = cor(signficant_degs_count,current_values,method="spearman")
     current_kendall_correlation = cor(signficant_degs_count,current_values,method="kendall")

     current_pearson_correlation_abs = cor(signficant_degs_count,abs(current_values),method="pearson")
     current_spearman_correlation_abs = cor(signficant_degs_count,abs(current_values),method="spearman")
     current_kendall_correlation_abs = cor(signficant_degs_count,abs(current_values),method="kendall")
  
     eigenassay_correlation_sigDEGs_correlation_line = eigenassay_correlation_sigDEGs_correlation_inner_base_line
     eigenassay_correlation_sigDEGs_correlation_line$Reference_valueType = current_valueType;
     eigenassay_correlation_sigDEGs_correlation_line$Pearson_correlation = current_pearson_correlation
     eigenassay_correlation_sigDEGs_correlation_line$Spearman_correlation = current_spearman_correlation
     eigenassay_correlation_sigDEGs_correlation_line$Kendall_correlation = current_kendall_correlation
     eigenassay_correlation_sigDEGs_correlation_line$Pearson_correlation_abs = current_pearson_correlation_abs
     eigenassay_correlation_sigDEGs_correlation_line$Spearman_correlation_abs = current_spearman_correlation_abs
     eigenassay_correlation_sigDEGs_correlation_line$Kendall_correlation_abs = current_kendall_correlation_abs
  
     if (length(eigenassay_correlation_sigDEGs_correlation)==0) {eigenassay_correlation_sigDEGs_correlation = eigenassay_correlation_sigDEGs_correlation_line}
     else { eigenassay_correlation_sigDEGs_correlation = rbind(eigenassay_correlation_sigDEGs_correlation,eigenassay_correlation_sigDEGs_correlation_line)}
  }#End
}#End - Calculate eigenassay_correlation_sigDEGs_correlation

{#Begin - Write eigenassay_correlation_sigDEGs_correlation
  if (file.exists(core_complete_eigenassay_correlation_sigDEGs_correlation_fileName))
  {#Begin
    write.table(eigenassay_correlation_sigDEGs_correlation,file=core_complete_eigenassay_correlation_sigDEGs_correlation_fileName,quote = FALSE,row.names = FALSE,col.names=FALSE,append=TRUE,sep=delimiter)
  }#End
  else 
  {#Begin
    write.table(eigenassay_correlation_sigDEGs_correlation,file=core_complete_eigenassay_correlation_sigDEGs_correlation_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
}#End - Write eigenassay_correlation_sigDEGs_correlation

## Calculate correlation with sigDEGs count - END
##########################################################################################################################

##########################################################################################################################

## Document start of analysis - BEGIN

if (indexCoreTask==document_progress[indexNextDocument])
{#Begin - Document progress
  core_progress_line = core_progress_report_base_line
  core_progress_line$Core = indexCore
  core_progress_line$Current_coreTask = indexCoreTask
  core_progress_line$Total_coreTasks = length_core_tasks
  core_progress_line$Dataset = dataset
  core_progress_line$Status = "Finished"
  core_progress_line$Correlation_method = correlation_method
  core_progress_line$Preprocess_data = preprocess_data
  core_progress_line$Decomposition_method = decomposition_method
  core_progress_line$IndexEigenassay = indexEigenassay
  core_progress_line$Time = Sys.time()
  
  write.table(core_progress_line,file=complete_progress_report_fileName,row.names=FALSE,quote=FALSE,col.names=FALSE,sep='\t',append=TRUE)
  indexNextDocument = indexNextDocument + 1
}#End - Document progress

}#End - indexCore task

{#Begin - Label progress report fileNames as finished  
  complete_finished_progress_report_fileName = gsub(".txt","_finished.txt",complete_progress_report_fileName)
  file.rename(complete_progress_report_fileName,complete_finished_progress_report_fileName)
}#End - Label progress report fileNames as finished  

}#End - Parallel clusters - SVD_3

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

  fileNames = list.files(statistics_enrichmentScore_directory)

  combined_statistics = c()
  combined_sigDEGs = c()
  combined_FAERSs = c()
  combined_ANOVA = c()
  
  for (indexBF in 1:length(fileNames))
  {#Begin
    fileName = fileNames[indexBF]
    if (grepl(eigenassay_correlation_statistics_fileName_withoutExtension,fileName))
    {#Begin
      complete_fileName = paste(statistics_enrichmentScore_directory,fileName,sep='')
      current_statistics = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep=delimiter)
      if (length(combined_statistics)==0) { combined_statistics = current_statistics }
      else { combined_statistics = rbind(combined_statistics,current_statistics)}
      unlink(complete_fileName)
    }#End
    if (grepl(eigenassay_correlation_sigDEGs_correlation_fileName_withoutExtension,fileName))
    {#Begin
      complete_fileName = paste(statistics_enrichmentScore_directory,fileName,sep='')
      current_sigDEGs = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep=delimiter)
      if (length(combined_sigDEGs)==0) { combined_sigDEGs = current_sigDEGs }
      else { combined_sigDEGs = rbind(combined_sigDEGs,current_sigDEGs)}
      unlink(complete_fileName)
    }#End
    if (grepl(eigenassay_correlation_FAERS_correlation_fileName_withoutExtension,fileName))
    {#Begin
      complete_fileName = paste(statistics_enrichmentScore_directory,fileName,sep='')
      current_FAERSs = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep=delimiter)
      if (length(combined_FAERSs)==0) { combined_FAERSs = current_FAERSs }
      else { combined_FAERSs = rbind(combined_FAERSs,current_FAERSs)}
      unlink(complete_fileName)
    }#End
    if (grepl(eigenassay_correlation_ANOVAs_fileName_withoutExtension,fileName))
    {#Begin
      complete_fileName = paste(statistics_enrichmentScore_directory,fileName,sep='')
      current_ANOVA = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep=delimiter)
      if (length(combined_ANOVA)==0) { combined_ANOVA = current_ANOVA }
      else { combined_ANOVA = rbind(combined_ANOVA,current_ANOVA)}
      unlink(complete_fileName)
    }#End
  }#End
  if (length(combined_statistics[,1]))
  {#Begin
    write.table(combined_statistics,file=complete_eigenassay_correlation_statistics_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
  if (length(combined_sigDEGs[,1]))
  {#Begin
    write.table(combined_sigDEGs,file=complete_eigenassay_correlation_sigDEGs_correlation_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
  if (length(combined_FAERSs[,1]))
  {#Begin
    write.table(combined_FAERSs,file=complete_eigenassay_correlation_FAERS_correlation_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
  if (length(combined_ANOVA[,1]))
  {#Begin
    write.table(combined_ANOVA,file=complete_eigenassay_correlation_ANOVAs_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,append=FALSE,sep=delimiter)
  }#End
}#End - Combine individual core files of each dataset analysis
