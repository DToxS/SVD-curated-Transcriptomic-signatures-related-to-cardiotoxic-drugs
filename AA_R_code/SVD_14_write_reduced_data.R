#stopCluster(parallel_clusters)
rm(list = ls());
#######################################################################################################################
## Generate tasks - BEGIN
get_eigenassays_per_dataset = FALSE
delete_task_reports = TRUE
add_inFrontOf_progress_report_fileName = "SVD_14"
source('SVD_global_parameter.R')

minimum_fractions_of_max_f1score = c(1)
ea_correlation_parameters = c("Ttest_pvalue")
entityClasses = c("Drug")
reference_valueTypes = c("Coefficient_of_eigenassay")

tasks = c()
task_no = 0;

bgRealExpression_values = c("Original","Decomposed_only")#,"MinusLog10pvalue_Q001","MinusLog10pvalue_Q005","MinusLog10pvalue_Q01","MinusLog10pvalue_Q05","MinusLog10pvalue_Q1",
                            #"MinusLog10pvalue_Q5","MinusLog10pvalue_Q10","MinusLog10pvalue_Q25","MinusLog10pvalue_Q50","MinusLog10pvalue_average")#"Decomposed_only","MinusLog10pvalue_max",

f1_score_weights = c(-1)#,(0:19)*0.05) #-1 indicates outlier based selection

indexAddCoCultureData = which(globally_assigned_tasks$Dataset!=globally_assigned_tasks$Dataset_coCulture)
globally_assigned_tasks$AddOrAnalyze_coCulture_data[indexAddCoCultureData] = TRUE

indexGlobalTasks = 1
for (indexGlobalTasks in 1:length(globally_assigned_tasks[,1]))
{#Begin - indexGlobalTasks
  current_global_task_line = globally_assigned_tasks[indexGlobalTasks,]
  for (index_ea_correlation_parameter in 1:length(ea_correlation_parameters))
  {#Begin
    ea_correlation_parameter = ea_correlation_parameters[index_ea_correlation_parameter]
    for (indexMinOfMaxF1Cutoff in 1:length(minimum_fractions_of_max_f1score))
    {#Begin
       minimum_fraction_of_max_f1score = minimum_fractions_of_max_f1score[indexMinOfMaxF1Cutoff]
       for (indexRV in 1:length(reference_valueTypes))
       {#Begin
          reference_valueType = reference_valueTypes[indexRV]
          for (indexE in 1:length(entityClasses))
          {#Begin
             entityClass= entityClasses[indexE]
             task_no = task_no + 1
             for (indexBG in 1:length(bgRealExpression_values))
             {#Begin
                bgRealExpression_value = bgRealExpression_values[indexBG]
                if (bgRealExpression_value=="Original") { current_f1_score_weights=-2 }
                if (bgRealExpression_value=="Decomposed_only") { current_f1_score_weights = f1_score_weights }
                for (indexF1W in 1:length(current_f1_score_weights))
                {#Begin
                   current_f1_score_weigth = current_f1_score_weights[indexF1W]
                   current_task_line = current_global_task_line
                   current_task_line$F1_score_weight = current_f1_score_weigth
                   current_task_line$EntityClass = entityClass
                   current_task_line$Task_no = task_no
                   current_task_line$BgRealExpression_value = bgRealExpression_value
                   if (current_task_line$Dataset %in% names(inputDataset_inputDatasetForSvd_list))
                   { current_task_line$Dataset_for_svd = inputDataset_inputDatasetForSvd_list[[current_task_line$Dataset]] }
                   if (!current_task_line$Dataset %in% names(inputDataset_inputDatasetForSvd_list))
                   { current_task_line$Dataset_for_svd = current_task_line$Dataset }
                   current_task_line$Reference_valueType = reference_valueType
                   current_task_line$Minimum_fraction_of_max_f1score = minimum_fraction_of_max_f1score
                   current_task_line$Ea_correlation_parameter = ea_correlation_parameter
                   current_task_line$Bg_expression_completeFileName = ""
                   if (length(tasks)==0) { tasks = current_task_line}
                   else { tasks = rbind(tasks,current_task_line)}
                }#End
             }#End
          }#End
       }#End
    }#End
  }#End
}#End

tasks$AddOrAnalyze_coCulture_data = TRUE
regular_tasks = tasks
regular_tasks$Dataset_coCulture = regular_tasks$Dataset
regular_tasks$AddOrAnalyze_coCulture_data = FALSE
tasks = rbind(tasks,regular_tasks)

## Generate tasks - End
#######################################################################################################################

length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

#core_tasks = tasks; indexCore=1;indexCoreTask=1

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

for (indexCore in 1:cores_count)
{#Begin
  startIndex = min(floor((indexCore-1) * tasks_per_core+1),length_tasks);
  endIndex = min(floor(indexCore * tasks_per_core),length_tasks)
  core_tasks = tasks[startIndex:endIndex,]
  clusterCall(parallel_clusters[indexCore], function(d) {assign('core_tasks', d, pos=.GlobalEnv)}, core_tasks)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('indexCore', d, pos=.GlobalEnv)}, indexCore)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('lincs_results_directory', d, pos=.GlobalEnv)}, lincs_results_directory)
}#End

cluster_generation_correct = TRUE;

combined_core_tasks <- do.call('rbind', clusterEvalQ(parallel_clusters, core_tasks))
if (length(combined_core_tasks[,1])!= length_tasks) { cluster_generation_correct = FALSE }
if (cluster_generation_correct)
{#Begin
  for (indexC in 1:length(combined_core_tasks[,1]))
  {#Begin
    if (combined_core_tasks$Dataset[indexC] != tasks$Dataset[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Correlation_method[indexC] != tasks$Correlation_method[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Preprocess_data[indexC] != tasks$Preprocess_data[indexC]) { cluster_generation_correct=FALSE;}
  }#End
}#End

if (!cluster_generation_correct)
{#Begin
  stopCluster(parallel_clusters) 
}#End

clusterEvalQ(parallel_clusters,
{#Begin - Parallel clusters
    
length_core_tasks = length(core_tasks[,1])
max_minusLog10Pvalue = 46

indexCoreTask = 2
for (indexCoreTask in 1:length_core_tasks)
{#Begin - indexCoreTask
   
drug_specific_gene_lines = c()
original_gene_lines = c()

current_task_line = core_tasks[indexCoreTask,]  
f1_score_weight = current_task_line$F1_score_weight
dataset = current_task_line$Dataset
minimum_fraction_of_max_f1score = current_task_line$Minimum_fraction_of_max_f1score
entityClass = current_task_line$EntityClass
ea_correlation_parameter = current_task_line$Ea_correlation_parameter
reference_valueType = current_task_line$Reference_valueType
complete_bg_expression_filenames = strsplit(current_task_line$Bg_expression_completeFileName,";")[[1]]
bgRealExpression_value = current_task_line$BgRealExpression_value
dataset_coculture = current_task_line$Dataset_coCulture
addOrAnalyze_coCulture_data = current_task_line$AddOrAnalyze_coCulture_data
addCoCulture_and_consider_only_drugs_within_that_dataset = addOrAnalyze_coCulture_data
  
############# Warning - unique element so far for only this script
#exclude_eigenassays = as.numeric(strsplit(current_task_line$Exclude_eigenassays,";")[[1]])
############# Warning - unique element so far for only this script

## Set global parameter - BEGIN
source('SVD_coreTaskSpecific_parameter.R')
## Set global parameter - END

##########################################################################################################################
## Generate colors - BEGIN
source('SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites.R')

if (addCoCulture_and_consider_only_drugs_within_that_dataset)
{#Begin
   dataset_coculture = current_task_line$Dataset_coCulture
   source('SVD_readPrepare_and_add_coCulture_degMatrix.R')
   indexesColKeep = c()
   for (indexCoCultureDrug  in 1:length(coCulture_drugs))
   {#Begin
      indexesColKeep = c(indexesColKeep,grep(coCulture_drugs[indexCoCultureDrug],colnames(Data)))
   }#End
   Data = Data[,indexesColKeep]
}#End


if (bgRealExpression_value=="Original")
{#Begin - if (bgRealExpression_value=="Original")
  Data_for_r_gene_expression = Data
  rm(Data)
  delimiter_that_separates_colnames = "[.]"
  col_splitString_indexDrug = 3
  col_splitString_indexCellline = 2 
  col_splitString_indexDrugClass = 1 
  col_splitString_indexPlate = 4
  add_inFrontOf_cellline_string = "Cell_line."
  replace_p_by_plate = TRUE
  
  source('SVD_generate_R_output_gene_expression_array.R')
  
  r_output_gene_expression_lines$F1_score_weight = -2
  r_output_gene_expression_lines$Outlier_cell_line = "F"
  
  if (length(original_gene_lines)==0) { original_gene_lines = r_output_gene_expression_lines }
  else { original_gene_lines = rbind(original_gene_lines,r_output_gene_expression_lines) }
  rm(r_output_gene_expression_lines)
}#End - if (bgRealExpression_value=="Original")
if (bgRealExpression_value=="Decomposed_only")
{#Begin - if (bgRealExpression_value=="Decomposed_only")
    collapsed_real_validations = read.table(file=complete_collapsed_entitySpecific_cluster_validation_f1_fileName,stringsAsFactors = FALSE,header = TRUE,sep='\t')
    collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Decomposition_method == decomposition_method,]
    collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Correlation_method == correlation_method,]
    collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Preprocess_data == preprocess_data,]
    collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$EA_correlation_parameter == ea_correlation_parameter,]
    collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Reference_valueType == reference_valueType,]
    collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$EntityClass==entityClass,]
    
    indexOutlier = which(collapsed_real_validations$Final_selection==finalSelection_fullData_sampleSpecific)
    outlier_collapsed = collapsed_real_validations[indexOutlier,]
    
    collapsed_real_validations = collapsed_real_validations[collapsed_real_validations$Final_selection==finalSelection_fullData,]
    if (f1_score_weight!=-1)
    { indexF1scoreWeigth = which(round(collapsed_real_validations$F1_score_weight*100)==round(f1_score_weight*100)) }
    if (f1_score_weight==-1)
    { indexF1scoreWeigth = which(collapsed_real_validations$Final_selection_for_outlier_based_decomposition==outlier_finalSelection_yes_label) }
    if (length(indexF1scoreWeigth)==0) { rm(collapsed_real_validations) }
    collapsed_real_validations = collapsed_real_validations[indexF1scoreWeigth,]

    entitySpecific_cluster_validations = read.csv(file = complete_entitySpecific_cluster_validation_f1_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
    #entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Dataset == dataset,]
    entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Decomposition_method == decomposition_method,]
    entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Correlation_method == correlation_method,]
    entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Preprocess_data == preprocess_data,]
    entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$EA_correlation_parameter == ea_correlation_parameter,]
    entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$Reference_valueType == reference_valueType,]
    entitySpecific_cluster_validations = entitySpecific_cluster_validations[entitySpecific_cluster_validations$EntityClass==entityClass,]
    
    eigenassay_statistics = read.csv(file = complete_eigenassay_correlation_statistics_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
    #eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Dataset == dataset,]
    eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Decomposition_method == decomposition_method,]
    eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Correlation_method == correlation_method,]
    eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Preprocess_data == preprocess_data,]
    eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Reference_valueType == reference_valueType,]
    indexEntityClass = which(eigenassay_statistics$EntityClass == entityClass)
    eigenassay_statistics = eigenassay_statistics[indexEntityClass,]

    if ((length(eigenassay_statistics[,1])>0)&(length(entitySpecific_cluster_validations[,1])>0))
    {#Begin - if ((length(eigenassay_statistics)>0)&(length(entitySpecific_cluster_validations)>0))
        entities = unique(eigenassay_statistics$Entity)
        indexE = 7
        for (indexE in 1:length(entities))
        {#Begin
           current_entity = entities[indexE]
        
           indexEntity_in_validations = which(entitySpecific_cluster_validations$Entity==current_entity)
           indexEntity_in_collapsed = which(collapsed_real_validations$Entity==current_entity)
           indexEntity_in_data = grep(paste(".",current_entity,".",sep=''),colnames(Data))
           if ( (length(indexEntity_in_validations)>0)
               &(length(indexEntity_in_data)>0))
           {#Begin
             if (length(indexEntity_in_collapsed)!=1) { stop("length(indexEntity_in_collapsed)!=1") }
             entity_validations = entitySpecific_cluster_validations[indexEntity_in_validations,]
             entity_collapsed = collapsed_real_validations[indexEntity_in_collapsed,]
        
             eigenassays_count = unique(entity_collapsed$Eigenassays_count)
             if (length(eigenassays_count)!=1) { stop("length(eigenassays_count)!=1") }
                
             currentEntity_eigenassay_statistics = eigenassay_statistics[eigenassay_statistics$Entity==current_entity,]
             currentEntity_eigenassay_statistics = currentEntity_eigenassay_statistics[order(currentEntity_eigenassay_statistics[[ea_correlation_parameter]]),]
             current_kept_eigenassays = currentEntity_eigenassay_statistics$Eigenassay[1:eigenassays_count]
        
             current_removed_eigenassays = 1:unique(entity_collapsed$Eigenassays_count_full_data)
             current_removed_eigenassays = current_removed_eigenassays[!current_removed_eigenassays %in% current_kept_eigenassays]
             
             if (length(current_kept_eigenassays)!=length(unique(current_kept_eigenassays))) { stop("(length(current_kept_eigenassays)!=length(unique(current_kept_eigenassays)))") }
             if (length(current_removed_eigenassays)!=length(unique(current_removed_eigenassays))) { stop("length(current_removed_eigenassays)!=length(unique(current_removed_eigenassays))") }
             
             eigenassays_list = list("Kept components" = current_kept_eigenassays,
                                     "Removed components" =  current_removed_eigenassays)
             
             indexFate=1
             for (indexFate in 1:length(eigenassays_list))
             {#Begin
                fate = names(eigenassays_list)[indexFate]
                current_eigenassays = eigenassays_list[[fate]]
               
                current_cum_eigensexpression_fraction = sum(currentEntity_eigenassay_statistics$Eigenexpression_fraction[1:eigenassays_count])
                source('SVD_generate_reduced_data_and_setColnames_to_fullDataColnames.R')
          
                colnames_data_current = colnames(Data_current)
                indexCurrentEntityColumns = grep(current_entity,colnames_data_current)

                {#Begin
                   Data_for_r_gene_expression = Data_current[,indexCurrentEntityColumns]
                   rm(Data_current)
                   delimiter_that_separates_colnames = "[.]"
                   col_splitString_indexDrug = 3
                   col_splitString_indexCellline = 2 
                   col_splitString_indexDrugClass = 1 
                   col_splitString_indexPlate = 4
                   add_inFrontOf_cellline_string = "Cell_line."
                   replace_p_by_plate = TRUE
               
                   source('SVD_generate_R_output_gene_expression_array.R')
                   rm(Data_for_r_gene_expression)
               
                   drugs = unique(r_output_gene_expression_lines$Drug)
                   indexDrug=1
                   for (indexDrug in 1:length(drugs))
                   {#Begin
                      drug = drugs[indexDrug]
                      indexCurrentDrugCollapsed = which(collapsed_real_validations$Entity==drug)
                      currentDrug_f1scoreWeight = collapsed_real_validations$F1_score_weight[indexCurrentDrugCollapsed]
                      if (length(currentDrug_f1scoreWeight)!=1) { rm(currentDrug_f1scoreWeight) }
                      
                      indexCurrentDrugROutput = which(r_output_gene_expression_lines$Drug==drug)
                      r_output_gene_expression_lines$F1_score_weight[indexCurrentDrugROutput] = currentDrug_f1scoreWeight
                      r_output_gene_expression_lines$Outlier_cell_line[indexCurrentDrugROutput] = "N"
                      
                      indexCurrentDrugCollapsedOutlier = which(outlier_collapsed$Entity==drug)
                      currentDrug_outlier_collapsed = outlier_collapsed[indexCurrentDrugCollapsedOutlier,]
                      indexCurrentF1ScoreWeight = which(currentDrug_outlier_collapsed$F1_score_weight==currentDrug_f1scoreWeight)
                      f1scoreWeigth_outlier_collapsed = currentDrug_outlier_collapsed[indexCurrentF1ScoreWeight,]
                      indexOutlierCellline = which(f1scoreWeigth_outlier_collapsed$Is_outlier==outlier_finalSelection_yes_label)
                      if (length(indexOutlierCellline)>1) { stop("length(indexOutlierCellline)>1") }
                      if (length(indexOutlierCellline)==1)
                      {#Begin
                         outlier_cellline = f1scoreWeigth_outlier_collapsed$Final_selection_for_sample[indexOutlierCellline]
                         indexOutlierInROutput = grep(outlier_cellline,r_output_gene_expression_lines$Cell_line)
                         indexOutlierInROutputDrug = indexOutlierInROutput[indexOutlierInROutput %in% indexCurrentDrugROutput]
                         r_output_gene_expression_lines$Outlier_cell_line[indexOutlierInROutputDrug] = "O"
                      }#End
                   }#End
                   
                   r_output_gene_expression_lines$Fate = fate

                   if (length(drug_specific_gene_lines)==0) { drug_specific_gene_lines = r_output_gene_expression_lines }
                   else { drug_specific_gene_lines = rbind(drug_specific_gene_lines,r_output_gene_expression_lines) }
                   rm(r_output_gene_expression_lines)
                }#End
             }#End
           }#End
        }#End
     }#End - if ((length(eigenassay_statistics)>0)&(length(entitySpecific_cluster_validations)>0))
}#End

if (!dir.exists(drugSpecificExpressionValues_directory)) { dir.create(drugSpecificExpressionValues_directory) }

if (length(original_gene_lines)>0)
{#Begin
  write.table(original_gene_lines,file=complete_originalExpressionValues_fileName,row.names=FALSE,col.names=TRUE,quote=FALSE,sep='\t')
  rm(original_gene_lines)
}#End

if (length(drug_specific_gene_lines)>0)
{#Begin
   fates = unique(drug_specific_gene_lines$Fate)
   for (indexFate in 1:length(fates))
   {#Begin
      fate = fates[indexFate]
      indexCurrentFate = which(drug_specific_gene_lines$Fate==fate)
      fate_drug_specific_gene_lines = drug_specific_gene_lines[indexCurrentFate,]
      if (fate=="Kept components")
      { write.table(fate_drug_specific_gene_lines,file=complete_drugSpecificExpressionValues_fileName,row.names=FALSE,col.names=TRUE,quote=FALSE,sep='\t') }
      if (fate=="Removed components")
      { write.table(fate_drug_specific_gene_lines,file=complete_drugRemovedExpressionValues_fileName,row.names=FALSE,col.names=TRUE,quote=FALSE,sep='\t') }
   }#End
   rm(drug_specific_gene_lines)
}#End

}#End - indexCoreTask - SVD_14 - Write reduced data

})#End - Parallel clusters - SVD_14 - Write reduced data
