####################################################################################################################
####################################################################################################################
##!!!!!!!!!!!!!!!!!!!!!!!! specify overall directory

overall_lincs_directory = "D:/LINCS_DToxS_SVD/" #ensure that the selected directory ends with '/'
fraction_of_used_cores = 0.40
library("parallel")
available_cores = detectCores(all.tests = FALSE, logical = TRUE)
cores_count = ceiling(fraction_of_used_cores*available_cores)

##!!!!!!!!!!!!!!!!!!!!!!!! specify overall directory
####################################################################################################################
####################################################################################################################
##Define user-supplied dataset related parameter

#These datasets will be subjected to SVD to identify drug-selective subspaces
#The subdirectories need to match the names
datasets = c( "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue"
             ,"SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD"
)

#These datasets (keys) will be projected into the subspaces generated from the specified datasets (values)
#Leave the list empty, if such key value pairs do not exist
#The subdirectories need to match the names (keys)
inputDataset_inputDatasetForSvd_list = list( 
  "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue" = "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue",
  "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue_no1stSVD" = "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD"
)

if (any(names(inputDataset_inputDatasetForSvd_list) %in% datasets)) { stop("keys in inputDataset_inputDatasetForSvd_list are not allowed to be listed in datasets") }

#The stated eigenarrays will be subtracted from the stated dataset to generate the "_no1stSVD" datasets
#any eigenarray(s) can be selected, the name "_no1stSVD" was just initially selected
#all datasets in substract_svds need to be assigned to an SVD-defining dataset in inputDataset_inputSVDDataset_list (e.g., itself)
#Leave list empty for no subtraction
substract_svds = list( 
  "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue" = c(1),
  "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue" = c(1)          
)

#For the given datasets (keys) the stated datasets (values) will define the SVD space to remove the eigenarrays selected above
#Used in SVD_1_subtractFirstSVD_from_DEGs.R
#This list will only be used for those datasets that are defined in substract_svds
#In the regular case, the keys and values should be the same.
#If you want to project a second dataset into the subspaces identified by a first dataset, enter the first dataset as the value
#and the second dataset as the key, as currently implemented for the second dataset "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue".
inputDataset_inputSVDDataset_list =
  list("SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue" = "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue",
       "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue" = "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue")

##Define user-supplied dataset related parameter
####################################################################################################################

coCulture_data_exists = FALSE
if (length(inputDataset_inputDatasetForSvd_list)>0) { coCulture_data_exists = TRUE }

#Open libraries and document versions - BEGIN
Col_names = c("Library","Version")
Col_length = length(Col_names)
Row_names = 1
Row_length= length(Row_names)
version_documentation_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
version_documentation_line = as.data.frame(version_documentation_line)
version_documentations = c()

libraries = c("parallel","ClassDiscovery","lattice","latticeExtra","colorspace","ART","dendextend","ape","fields","colormap","beeswarm","gplots","grid","gridExtra","ggplot2","doParallel","progress")#"genefilter",
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

source('Common_tools.R')

r_sessionInfo = Get_sessionInfo_summary_table()

#Open libraries and document versions - END

lincs_results_directory = paste(overall_lincs_directory,"Results/",sep='')
metadata_directory = paste(overall_lincs_directory,"Experimental_data/Metadata/",sep='')
ontology_library_directory = paste(overall_lincs_directory,"Libraries_for_enrichment/Self/",sep='')
iPSCdCM_scRNAseq_data_directory = paste(overall_lincs_directory,"iPSCdCM_scRNAseq/",sep='')
downloaded_datasets_directory = paste(overall_lincs_directory,"Downloaded_datasets/",sep='')
scSnRnaSeq_enrichment_directory = paste(lincs_results_directory,"ScSnRNAseq_enrichment/",sep='')
enrichment_results_directory = paste(lincs_results_directory,"SVD_outlier_responses/",sep='')
drugTarget_directory = paste(lincs_results_directory,"SVD_drug_target_proteins/",sep='')
libraries_for_enrichment_self_directory = paste(overall_lincs_directory,"Libraries_for_enrichment/Self/",sep='')
libraries_for_enrichment_download_directory = paste(overall_lincs_directory,"Libraries_for_enrichment/Download/",sep='')

complete_report_finished_1st_part_fileName = paste(lincs_results_directory,"Report_finished_1st_part_by_Csharp.txt",sep='')
complete_report_finished_2nd_part_fileName = paste(lincs_results_directory,"Report_finished_2nd_part_by_R.txt",sep='')
complete_report_finished_3rd_part_fileName = paste(lincs_results_directory,"Report_finished_3rd_part_by_Csharp.txt",sep='')
complete_report_finished_4th_part_fileName = paste(lincs_results_directory,"Report_finished_4th_part_by_R.txt",sep='')
complete_report_finished_5th_part_fileName = paste(lincs_results_directory,"Report_finished_5th_part_by_Csharp.txt",sep='')
complete_report_finished_6th_part_fileName = paste(lincs_results_directory,"Report_finished_6th_part_by_R.txt",sep='')
waiting_time_in_minutes_if_csharp_not_finished = 30

dataset_cocultures_list = list()
if (length(inputDataset_inputDatasetForSvd_list)>0)
{#Begin
  for (indexInputDataset in 1:length(inputDataset_inputDatasetForSvd_list))
  {#Begin - Fill dataset cocultures list
     inputDataset = names(inputDataset_inputDatasetForSvd_list)[indexInputDataset]
     regular_dataset = inputDataset_inputDatasetForSvd_list[[inputDataset]]
     coCulture_dataset = inputDataset
     dataset_cocultures_list[[regular_dataset]] = coCulture_dataset
  }#End - Fill dataset cocultures list
}#End

correlation_methods = c("pearson");#c("pearson")#,"spearman","kendall")
preprocess_datas = c("NoPrepr")#,"Remove_first_svd_eigenassay")#,"Center")#,"Center_and_scale","Center"),"NoPrepr","Remove_first_svd_eigenassay",
decomposition_methods = c("SVD")
icasso_permuations = 100;
f1_score_weight_default=0.95

Task_col_names = c("Task_no","Dataset","Correlation_method","Preprocess_data","Decomposition_method","Dataset_coCulture","AddOrAnalyze_coCulture_data")
Task_col_length = length(Task_col_names)
Task_row_names = 1
Task_row_length = length(Task_row_names)
task_base_line = array(NA,dim=c(Task_row_length,Task_col_length),dimnames = list(Task_row_names,Task_col_names))
task_base_line = as.data.frame(task_base_line)

globally_assigned_tasks = c()
task_no = 0;

for (indexDataset in 1:length(datasets))
{#Begin - indexDataset
  dataset = datasets[indexDataset]
  dataset_coculture = dataset_cocultures_list[[dataset]]
  
  for (indexCorrelationMethod in 1:length(correlation_methods))
  {#Begin - indexCorrelationMethod
    correlation_method = correlation_methods[indexCorrelationMethod]
    
    for (indexPreprocessData in 1:length(preprocess_datas))
    {#Begin = preprocessData
      preprocess_data = preprocess_datas[indexPreprocessData]
      
      for (indexDecomposition in 1:length(decomposition_methods))
      {#Begin - indexDecomposition
         decomposition_method = decomposition_methods[indexDecomposition]
         task_no = task_no + 1
        
         task_line = task_base_line
         task_line$Task_no = task_no
         task_line$Dataset = dataset
         task_line$Dataset_coCulture = dataset_coculture
         task_line$Correlation_method = correlation_method
         task_line$Preprocess_data = preprocess_data
         task_line$Decomposition_method = decomposition_method
         task_line$AddOrAnalyze_coCulture_data = FALSE
         task_line$F1_score_weight_default = f1_score_weight_default
         if (length(globally_assigned_tasks)==0) { globally_assigned_tasks = task_line}
         else { globally_assigned_tasks = rbind(globally_assigned_tasks,task_line)}
      }#End - indexDecomposition
    }#End - preprocessData
  }#End - indexCorrelationMethod
  
  sessionInfo_directory = paste(lincs_results_directory,dataset,"/",sep='')
  if (dir.exists(sessionInfo_directory))
  {#Begin
     complete_sessionInfo_fileName = paste(sessionInfo_directory,"SessionInfo.txt",sep='')
     write.table(r_sessionInfo,file=complete_sessionInfo_fileName,col.names=TRUE,row.names=FALSE,sep='\t',quote=FALSE)
  }#End
}#End - indexDataset

###############################################################################################

if (delete_task_reports)
{#Begin - Delete task reports
   for (indexReport in 1:150)
   {#Begin
     complete_fileName = paste(lincs_results_directory,add_inFrontOf_progress_report_fileName,"_progress_report_core",indexReport,".txt",sep='')
     if (file.exists(complete_fileName)) { unlink(complete_fileName) }
     complete_fileName = paste(lincs_results_directory,add_inFrontOf_progress_report_fileName,"_progress_report_core",indexReport,"_finished.txt",sep='')
     if (file.exists(complete_fileName)) { unlink(complete_fileName) }
   }#End
}#End - Delete task reports
  
###############################################################################################
if (get_eigenassays_per_dataset)
{#Begin - Calculate eigenassays per dataset

dataset_eigenassays_count_list = list()

indexGlobalTask=1
for (indexGlobalTask in 1:length(globally_assigned_tasks[,1]))
{#Begin
  current_global_task_line = globally_assigned_tasks[indexGlobalTask,]
  dataset = current_global_task_line$Dataset
  if (!dataset %in% names(dataset_eigenassays_count_list))
  {#Begin
    datasets = c(datasets,dataset)
    current_task_line = current_global_task_line
    source('SVD_coreTaskSpecific_parameter.R')
    Data = read.table(file=complete_degs_fileName,sep='\t',header=TRUE)
    colNamesRemove = c("Symbol","RowSum","RowMean","RowSampleSD","RowSampleCV","Description","ReadWrite_SCPs","ReadWrite_human_symbols")
    indexColRemove = which(colnames(Data) %in% colNamesRemove)
    indexColKeep = 1:length(Data[1,])
    indexColKeep = indexColKeep[!indexColKeep %in% indexColRemove]
    indexRowKeep = 1:length(Data[,1])
    dataset_eigenassays_count_list[[dataset]] = min(length(indexColKeep),length(indexRowKeep))
  }#End
}#End

}#End - Calculate eigenassays per dataset