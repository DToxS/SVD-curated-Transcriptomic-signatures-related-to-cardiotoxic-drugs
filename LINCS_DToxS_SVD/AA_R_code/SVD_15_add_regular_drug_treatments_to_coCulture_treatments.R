#library(parallel)
#stopCluster(parallel_clusters)
rm(list = ls());
delete_task_reports=TRUE
add_inFrontOf_progress_report_fileName = "SVD_1"
get_eigenassays_per_dataset=FALSE
source('SVD_global_parameter.R')
tasks = globally_assigned_tasks

full_data_fileName = "Drug_specific_expression_full.txt" #Has to match with SVD_1_subtractFirstSVD_from_DEGs
svd1_removed_fileName = "Drug_specific_expression_SVD1removed.txt" #Has to match with SVD_1_subtractFirstSVD_from_DEGs
no1stSVD_fileName = "Drug_specific_expression_no1stSVD.txt" #Has to match with SVD_1_subtractFirstSVD_from_DEGs
fileNames = c(svd1_removed_fileName,no1stSVD_fileName,full_data_fileName)


indexT=1
indexCore=3
for (indexT in 1:length(tasks[,1]))
{#Begin
   current_task_line = tasks[indexT,]
   for (indexFN in 1:length(fileNames))
   {#Begin
      fileName = fileNames[indexFN]
      f1_score_weight = -1
      current_task_line$AddOrAnalyze_coCulture_data = FALSE
      source('SVD_coreTaskSpecific_parameter.R')
      complete_fileName = paste(drugSpecificExpressionValues_directory,fileName,sep='')
      if (file.exists(complete_fileName))
      {#Begin
        regular_data = read.csv(file=complete_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
        current_task_line$AddOrAnalyze_coCulture_data = TRUE
        source('SVD_coreTaskSpecific_parameter.R')
        complete_fileName = paste(drugSpecificExpressionValues_directory,fileName,sep='')
        coCulture_data = read.csv(file=complete_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
        complete_saveFileName = gsub(".txt","_cellCultureOnly.txt",complete_fileName)
        write.table(coCulture_data,file=complete_saveFileName,quote=FALSE,col.names=TRUE,row.names=FALSE,sep='\t')
        keep_drugs = unique(coCulture_data$Drug)
        indexKeep = which(regular_data$Drug %in% keep_drugs)
        regular_data = regular_data[indexKeep,]
        coCulture_data = rbind(coCulture_data,regular_data)
        write.table(coCulture_data,file=complete_fileName,quote=FALSE,col.names=TRUE,row.names=FALSE,sep='\t')
        rm(coCulture_data);rm(regular_data)
      }#End
   }#End
}#End