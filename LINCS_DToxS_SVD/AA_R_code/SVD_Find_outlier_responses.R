
#core_tasks = tasks; indexCoreTask=1; indexCore=1

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
  library("dixonTest")
}
);

indexCore=2
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
               
               print_to_console = c() 
               
               length_core_tasks = length(core_tasks[,1])  
               
               indexCoreTask = 8
               for (indexCoreTask in 1:length_core_tasks)
               {#Begin - indexCoreTask
                 current_task_line = core_tasks[indexCoreTask,]
                 addOrAnalyze_coCulture_data = current_task_line$AddOrAnalyze_coCulture_data
                 f1_score_weight_default = current_task_line$F1_score_weight_default
                 f1_score_weight=-1
                 
                 ## Set global parameter - BEGIN
                 source('SVD_coreTaskSpecific_parameter.R')
                 ## Set global parameter - END
                 
                 ##########################################################################################################################
                 ## Generate colors - BEGIN
                 source('SVD_colors.R')
                 ## Generate colors - END
                 ##########################################################################################################################
                 {#Begin - Define outlier results
                   Col_names = c("F1_score_weight","Entity","EntityClass","Pvalue","Q","Sample_count","Outlier","Significant","Final_selection","Outlier_short","Reference_valueType","Mean_F1score_without_outlier","F1_score_with_outlier","Diff_F1_score","Eigenassay_count","Mean_correlation_reduced_full_data","Median_correlation_reduced_full_data")
                   Col_length =length(Col_names)
                   Row_names = ""
                   Row_length = length(Row_names)
                   outlier_base_line = array(NA,c(Row_length,Col_length),dimnames = list(Row_names,Col_names))
                   outlier_base_line = as.data.frame(outlier_base_line)
                   
                   outliers = c()
                 }#End - Define outlier results
                 
                 significant_pvalue_cutoff = -1
                 significant_adj_pvalue_cutoff = 0.05
                 minimum_average_F1_score = 0.5
                 source('SVD_colors.R')
                 
                 if (!addOrAnalyze_coCulture_data)
                 {#Begin
                   complete_fileName = complete_collapsed_entitySpecific_cluster_validation_f1_fileName
                 }#End
                 if (addOrAnalyze_coCulture_data)
                 {#Begin
                   complete_fileName = complete_coCulture_collapsed_entitySpecific_cluster_validation_f1_fileName
                 }#End
                 
                 collapsed_cluster_validations = read.csv(file = complete_fileName,stringsAsFactors = FALSE,header=TRUE,sep='\t')
                 
                 if (length(unique(collapsed_cluster_validations$EA_correlation_parameter))!=1) { rm(collapsed_cluster_validations) }
                 
                 allEntities = unique(collapsed_cluster_validations$Entity)
                 allFullNames = allEntities
                 for (indexFullName in 1:length(allFullNames))
                 {#Begin
                   drug = allFullNames[indexFullName]
                   if (drug %in% names(full_drugNames))
                   { allFullNames[indexFullName] = full_drugNames[[drug]] }
                 }#End
                 
                 collapsed_cluster_validations$Final_selection_for_outlier_based_decomposition = outlier_finalSelection_no_label
                 collapsed_cluster_validations$Final_selection_contains_outlier = outlier_finalSelection_no_label
                 collapsed_cluster_validations$Is_outlier = outlier_finalSelection_no_label
                 
                 indexKeep = which(collapsed_cluster_validations$EntityClass=="Drug")
                 drug_collapsed_cluster_validations = collapsed_cluster_validations[indexKeep,]
                 indexKeepCollapsed = which(drug_collapsed_cluster_validations$Final_selection==finalSelection_fullData_sampleSpecific)
                 if (length(indexKeepCollapsed)>0)
                 {#Begin
                   sampleSpecific_collapsed_cluster_validations = drug_collapsed_cluster_validations[indexKeepCollapsed,]
                   
                   {#Begin - Identify total number of treatments for each cell line
                     indexOneF1ScoreWeight = which(sampleSpecific_collapsed_cluster_validations$F1_score_weight==sampleSpecific_collapsed_cluster_validations$F1_score_weight[1])
                     indexOneRValue =  which(sampleSpecific_collapsed_cluster_validations$Reference_valueType==sampleSpecific_collapsed_cluster_validations$Reference_valueType[1])
                     indexOneAnalysis = indexOneRValue[indexOneRValue %in% indexOneF1ScoreWeight]
                     treated_celllines_count_table = table(sampleSpecific_collapsed_cluster_validations$Final_selection_for_sample[indexOneAnalysis])
                     if (length(unique(sampleSpecific_collapsed_cluster_validations$Eigenassays_count_full_data))!=1) {stop("(length(unique(sampleSpecific_collapsed_cluster_validations$Eigenassays_count_full_data))!=1)")}
                     if ( (sum(treated_celllines_count_table)!=unique(sampleSpecific_collapsed_cluster_validations$Eigenassays_count_full_data))
                          &(!addOrAnalyze_coCulture_data))
                     {stop("(sum(treated_celllines_count_table)!=unique(sampleSpecific_collapsed_cluster_validations$Eigenassays_count_full_data))")}
                   }#End - Identify total number of treatments for each cell line
                   
                   reference_valueTypes = unique(sampleSpecific_collapsed_cluster_validations$Reference_valueType)
                   indexRVT=2
                   for (indexRVT in 1:length(reference_valueTypes))
                   {#Begin - Identify outlier for each reference valueType
                     reference_valueType = reference_valueTypes[indexRVT]
                     indexCurrentReferenceVT = which(sampleSpecific_collapsed_cluster_validations$Reference_valueType==reference_valueType)
                     rvt_validations = sampleSpecific_collapsed_cluster_validations[indexCurrentReferenceVT,]
                     f1_score_weights = unique(rvt_validations$F1_score_weight)
                     total_test_counts = 0
                     referenceValueType_outliers=c()
                     for (indexF1W in 1:length(f1_score_weights))
                     {#Begin - Identify outlier for each f1 score weight
                       f1_score_weight = f1_score_weights[indexF1W]
                       indexCurrentF1scoreW = which(rvt_validations$F1_score_weight==f1_score_weight)
                       f1_score_weight_validations = rvt_validations[indexCurrentF1scoreW,]
                       entities = unique(f1_score_weight_validations$Entity)
                       for (indexE in 1:length(entities))
                       {#Begin - Identify outlier for each entity
                         entity = entities[indexE]
                         indexCurrentEntity = which(f1_score_weight_validations$Entity==entity)
                         entity_validations = f1_score_weight_validations[indexCurrentEntity,]
                         if (length(unique(entity_validations$Eigenassays_count))!=1) { rm(entity_validations) }
                         total_test_counts = total_test_counts + 1
                         
                         entity_validations = entity_validations[order(entity_validations$F1_score),]
                         #if (entity=="PON") { entity_validations = entity_validations[c(1,3:length(entity_validations[,1])),]}
                         
                         dixon_results = tryCatch( { dixonTest(entity_validations$F1_score) },
                                                   error=function(cond) { return (NA) },
                                                   #warning=function(w) {},
                                                   finally = {});
                         use_dixon_test = TRUE
                         if ((use_dixon_test)&(!is.na(dixon_results[[1]])))
                         {#Begin
                           pvalue = dixon_results[[5]]
                           if (pvalue<1)
                           {#Begin
                             outlier_line = outlier_base_line
                             dixon_indexes = dixon_results[[6]]
                             indexOutlier = dixon_indexes[[1]]
                             indexesRest = 1:length(entity_validations[,1])
                             indexesRest = indexesRest[!indexesRest %in% indexOutlier]
                             outlier_line$Q = dixon_results[[3]]
                             outlier_line$Pvalue = pvalue
                             outlier_line$Sample_count = dixon_results[[4]]
                             outlier_line$F1_score_weight = f1_score_weight
                             outlier_line$Entity = entity
                             outlier_line$Reference_valueType = reference_valueType
                             outlier_line$Outlier = entity_validations$Final_selection_for_sample[indexOutlier]
                             outlier_line$EntityClass = unique(entity_validations$EntityClass)
                             outlier_line$Mean_F1score_without_outlier = mean(entity_validations$F1_score[indexesRest])
                             if (length(unique(round(entity_validations$Eigenassays_count*1E5)))>1) { stop("Duplicated entity_validations$Eigenassays_count") }
                             outlier_line$Eigenassays_count = entity_validations$Eigenassays_count[1]
                             if (length(unique(round(entity_validations$Median_correlation_reduded_full_data*1E5)))>1) { stop("Duplicated entity_validations$Mean_correlation_reduded_full_data") }
                             outlier_line$Median_correlation_reduced_full_data = entity_validations$Median_correlation_reduded_full_data[1]
                             if (length(unique(round(entity_validations$Mean_correlation_reduded_full_data*1E5)))>1) { stop("Duplicated entity_validations$Mean_correlation_reduded_full_data") }
                             outlier_line$Mean_correlation_reduced_full_data = entity_validations$Mean_correlation_reduded_full_data[1]
                             outlier_line$F1_score_outlier = entity_validations$F1_score[indexOutlier]
                             outlier_line$Diff_F1_score = outlier_line$Mean_F1score_without_outlier - outlier_line$F1_score_outlier
                             if (length(referenceValueType_outliers)>0) { referenceValueType_outliers = rbind(referenceValueType_outliers,outlier_line) }
                             else {referenceValueType_outliers = outlier_line }
                           }#End
                         }#End
                       }#End - Identify outlier for each entity
                     }#End - Identify outlier for each f1 score weight
                     if (length(referenceValueType_outliers)>0)
                     {#Begin
                       referenceValueType_outliers = referenceValueType_outliers[order(referenceValueType_outliers$Pvalue),]
                       referenceValueType_outliers$Adj_pvalue = referenceValueType_outliers$Pvalue * total_test_counts
                       indexLarger1 = which(referenceValueType_outliers$Adj_pvalue>1)
                       referenceValueType_outliers$Adj_pvalue[indexLarger1] = 1
                       if (length(outliers)>0) { outliers = rbind(outliers,referenceValueType_outliers) }
                       else { outliers = referenceValueType_outliers }
                     }#End
                   }#End - Identify outlier for same reference valueType
                   
                   drugs = unique(outliers$Entity)
                   if (length(drugs)>0)
                   {#Begin - if outliers were found for at least one drug
                     outliers$Full_entity_name="not found"
                     for (indexDrug in 1:length(drugs))
                     {#Begin - Add full drug names to outlier spreadsheet, if exists
                       drug = drugs[indexDrug]
                       indexCurrentDrug = which(outliers$Entity==drug)
                       if (drug %in% names(full_drugNames))
                       { outliers$Full_entity_name[indexCurrentDrug] = full_drugNames[[drug]] }
                     }#End - Add full drug names to outlier spreadsheet, if exists
                     
                     {#Begin - Define significance for outlier
                       outliers$Significant = outlier_significant_label
                       if (significant_adj_pvalue_cutoff>0) 
                       {#Begin
                         indexNotSignificant = which(outliers$Adj_pvalue>significant_adj_pvalue_cutoff) 
                         outliers$Significant[indexNotSignificant] = outlier_not_significant_label
                       }#End
                       if (significant_pvalue_cutoff>0) 
                       {#Begin
                         indexNotSignificant = which(outliers$Pvalue>significant_pvalue_cutoff) 
                         outliers$Significant[indexNotSignificant] = outlier_not_significant_label
                       }#End
                       if (minimum_average_F1_score>0) 
                       {#Begin
                         indexNotSignificant = which(outliers$Mean_F1score_without_outlier<minimum_average_F1_score) 
                         outliers$Significant[indexNotSignificant] = outlier_not_significant_label
                       }#End
                       outliers$Final_selection = outlier_finalSelection_no_label
                       outliers = outliers[order(outliers$F1_score_weight,decreasing=FALSE),]
                       outliers = outliers[order(outliers$Mean_F1score_without_outlier,decreasing=TRUE),]
                       #outliers = outliers[order(outliers$Significant,decreasing=TRUE),]
                       outliers = outliers[order(outliers$Adj_pvalue,decreasing=FALSE),]
                       outliers$UID_entity_outlier_referenceVT = paste(outliers$Entity," : ",outliers$Outlier," : ",outliers$Reference_valueType,sep='')
                       outliers$UID_entity_referenceVT = paste(outliers$Entity," : ",outliers$Reference_valueType,sep='')
                       outliers$UID_entity_f1scoreWeight_referenceVT = paste(outliers$Entity," : ",outliers$F1_score_weight," : ",outliers$Reference_valueType,sep='')
                       indexUID_entity_outlier_referenceVT = which(!duplicated(outliers$UID_entity_outlier_referenceVT)) #up to one outlier each outlier cell line for each drug and reference value type
                       indexUID_entity_referenceVT = which(!duplicated(outliers$UID_entity_referenceVT)) #up to one outlier each outlier cell line for each drug and reference value type
                       indexSignificant = which(outliers$Significant==outlier_significant_label)
                       indexSigUID_entity_outlier_referenceVT = indexUID_entity_outlier_referenceVT[indexUID_entity_outlier_referenceVT %in% indexSignificant]
                       indexSigUID_entity_referenceVT = indexUID_entity_referenceVT[indexUID_entity_referenceVT %in% indexSignificant]
                       outliers$Final_selection[indexSigUID_entity_referenceVT] = outlier_finalSelection_yes_label
                       if (length(indexSigUID_entity_referenceVT)<length(indexSigUID_entity_outlier_referenceVT))
                       {#Begin
                          indicesRemoved = indexSigUID_entity_outlier_referenceVT[!indexSigUID_entity_outlier_referenceVT %in% indexSigUID_entity_referenceVT]
                          indexIndexRemoved_to_be_restored = which(outliers$UID_entity_f1scoreWeight_referenceVT[indicesRemoved] %in% outliers$UID_entity_f1scoreWeight_referenceVT[indexSigUID_entity_referenceVT])
                          indicesRemoved_to_be_restored = indicesRemoved[indexIndexRemoved_to_be_restored]
                          outliers$Final_selection[indicesRemoved_to_be_restored] = outlier_finalSelection_yes_label
                          indicesRemoved = indicesRemoved[!indicesRemoved %in% indicesRemoved_to_be_restored]
                          if (length(indicesRemoved)>0)
                          {#Begin
                             print_to_console = c(print_to_console,paste(general_directory,"@",dataset,":",sep=''))
                             for (indexIndex in 1:length(indicesRemoved))
                             {#Begin
                                 indexRemoved = indicesRemoved[indexIndex]
                                 print_to_console = c(print_to_console,paste(general_directory,"@",indexIndex,") ",outliers$UID_entity_f1scoreWeight_referenceVT[indexRemoved],sep=''))
                             }#End
                          }#End
                       }#End
                       outliers$UID_entity_outlier_referenceVT = NULL
                       outliers$UID_entity_referenceVT = NULL
                       outliers$UID_entity_f1scoreWeight_referenceVT = NULL
                     }#End - Define significance for outlier
                   }#End - if outliers were found for at least one drug
                   if (!addOrAnalyze_coCulture_data)
                   {#Begin
                     complete_outlier_fileName_here = complete_outlier_fileName
                   }#End
                   if (addOrAnalyze_coCulture_data)
                   {#Begin
                     complete_outlier_fileName_here = complete_coCulture_outlier_fileName
                   }#End
                   write.table(outliers,file=complete_outlier_fileName_here,row.names=FALSE,quote=FALSE,col.names=TRUE,sep='\t')
                   
                   indexFinalSelectionOutlier = which(outliers$Final_selection==outlier_finalSelection_yes_label)
                   finalSelectionOutliers = outliers[indexFinalSelectionOutlier,]
                   if (length(finalSelectionOutliers[,1])>0)
                   {#Begin - Add final selection for outlier to collapsed_cluster_validation
                     for (indexO in 1:length(finalSelectionOutliers[,1]))
                     {#Begin
                       current_outlier_line = finalSelectionOutliers[indexO,]
                       indexCurrentDrugCollapsed = which(collapsed_cluster_validations$Entity==current_outlier_line$Entity)
                       indexCurrentReferenceVTCollapsed = which(collapsed_cluster_validations$Reference_valueType==current_outlier_line$Reference_valueType)
                       indexCurrentF1scoreWCollapsed = which(collapsed_cluster_validations$F1_score_weight==current_outlier_line$F1_score_weight)
                       outlier_celllines = current_outlier_line$Outlier
                       if (length(outlier_celllines)!=1) { stop("length(outlier_celllines)!=1") }
                       indexOutlierCellline = which(collapsed_cluster_validations$Final_selection_for_sample%in%outlier_celllines)
                       indexCollapsed = indexCurrentDrugCollapsed
                       indexCollapsed = indexCollapsed[indexCollapsed %in% indexCurrentDrugCollapsed]
                       indexCollapsed = indexCollapsed[indexCollapsed %in% indexCurrentReferenceVTCollapsed]
                       indexCollapsed = indexCollapsed[indexCollapsed %in% indexCurrentF1scoreWCollapsed]
                       indexCollapsed_outlier = indexCollapsed[indexCollapsed %in% indexOutlierCellline]
                       
                       if (length(indexCollapsed_outlier)!=1) { stop("length(indexCollapsed_outlier)!=1") }
                       
                       collapsed_cluster_validations$Final_selection_for_outlier_based_decomposition[indexCollapsed] = outlier_finalSelection_yes_label
                       collapsed_cluster_validations$Final_selection_contains_outlier[indexCollapsed] = outlier_finalSelection_yes_label
                       collapsed_cluster_validations$Is_outlier[indexCollapsed_outlier] = outlier_finalSelection_yes_label
                     }#End
                   }#End - Add final selection for outlier to collapsed_cluster_validation
                   
                   indexRVT = 1      
                   for (indexRVT in 1:length(reference_valueTypes))
                   {#Begin - Select for all drugs with no outliers the default F1 score
                     reference_valueType = reference_valueTypes[indexRVT]
                     indexCurrentRVTinOutliers = which(finalSelectionOutliers$Reference_valueType==reference_valueType)
                     rvt_outliers = finalSelectionOutliers[indexCurrentRVTinOutliers,]
                     drugsWithOutlier = unique(rvt_outliers$Entity)
                     indexDrugsWithoutOutlier = which(!collapsed_cluster_validations$Entity %in% drugsWithOutlier)
                     indexDefaultF1ScoreWeight = which(collapsed_cluster_validations$F1_score_weight==f1_score_weight_default)
                     indexReferenceValueType = which(collapsed_cluster_validations$Reference_valueType==reference_valueType)
                     indexLabel = indexDrugsWithoutOutlier
                     indexLabel = indexLabel[indexLabel %in% indexDefaultF1ScoreWeight]
                     indexLabel = indexLabel[indexLabel %in% indexReferenceValueType] 
                     collapsed_cluster_validations$Final_selection_for_outlier_based_decomposition[indexLabel] = outlier_finalSelection_yes_label
                   }#End - Select for all drugs with no outliers the default F1 score
                   
                   if (length(outliers[,1])>0)
                   {#Begin - Generate outlier figures for each reference_valueType, if outliers were found
                     indexRVT=2
                     for (indexRVT in 1:length(reference_valueTypes))
                     {#Begin - Generate outlier figures for each reference_valueType
                       reference_valueType = reference_valueTypes[indexRVT]
                       indexCurrentReferenceVT = which(outliers$Reference_valueType==reference_valueType)
                       if (length(indexCurrentReferenceVT)>0)
                       {#Begin
                         
                         plots_per_col = 6
                         plots_per_row = 9
                         plots_on_current_page = 0
                         complete_pdf_fileName = gsub(".txt",paste("_",reference_valueType,".pdf",sep=''),complete_outlier_fileName_here)
                         pdf(complete_pdf_fileName, width=8.5, height=11)
                       
                         rvt_outliers = outliers[indexCurrentReferenceVT,]
                         entities = unique(rvt_outliers$Entity)
                         indexE=1
                         Outlier_identicationDocumentation_figures = list()
                         entities_in_order = drugs_in_drugClass_specific_order[drugs_in_drugClass_specific_order %in% entities]
                         indexMissing = which(!entities %in% entities_in_order)
                         entities_in_order = c(entities_in_order,entities[indexMissing])
                         indexE=8
                         for (indexE in 1:length(entities_in_order))
                         {#Begin - Generate Outlier documentation figures for each entity
                           entity = entities_in_order[indexE]
                           fullDrugName = full_drugNames[[entity]]
                           indexCurrentEntity = which(rvt_outliers$Entity==entity)
                           entity_outliers = rvt_outliers[indexCurrentEntity,]
                           entity_outliers$Outlier = strsplit(entity_outliers$Outlier,"_")[[1]][1]
                           entity_outliers$Minus_log10pvalue = -log10(entity_outliers$Pvalue)
                           entity_outliers$Minus_log10AdjPvalue = -log10(entity_outliers$Adj_pvalue)
                           
                           title_size=7#5
                           axis_title_size=7#5
                           axis_text_size=7#5
                           legend_text_size=7
                           point_cex=0.5
                           bar_width=0.03
                           bar_frame_size=0.01
                           samples = unique(entity_outliers$Outlier)
                           samples = samples[order(samples)]
                           source('SVD_colors.R')
                           sample_colors = replicate(length(samples),"gray20")
                           for (indexSC in 1:length(sample_colors))
                           {#Begin
                             sample = samples[indexSC]
                             indexCurrentSample = which(cellLines_for_highlight_colors==sample)
                             if (length(indexCurrentSample)>0)
                             {#Begin 
                               sample_colors[indexSC] = cellLine_highlight_colors[indexCurrentSample] 
                             }#End
                           }#End
                           
                           if (significant_pvalue_cutoff>0) 
                           {#Begin
                             yaxis_name = "-log10(p-value)"
                             entity_outliers$Test_results_value = entity_outliers$Minus_log10Pvalue
                             cutoff_line_yintersect = -log10(significant_pvalue_cutoff)
                           }#End
                           if (significant_adj_pvalue_cutoff>0) 
                           {#Begin
                             yaxis_name = "-log10(adj p-value)"
                             entity_outliers$Test_results_value = entity_outliers$Minus_log10AdjPvalue
                             cutoff_line_yintersect = -log10(significant_adj_pvalue_cutoff)
                           }#End
                           
                           Plot = ggplot(entity_outliers,aes(x=F1_score_weight,y=Test_results_value,fill=Outlier))
                           Plot = Plot + geom_bar(stat="identity",width=bar_width,color = ifelse(entity_outliers$Final_selection==outlier_finalSelection_yes_label,"black",sample_colors[1]),size=bar_frame_size)
                           Plot = Plot + scale_fill_manual(values=sample_colors)
                           Plot = Plot + geom_hline(yintercept=cutoff_line_yintersect)
                           Plot = Plot + theme(legend.position = "none")
                           Plot = Plot + xlim(-0.05,1) + ylab(yaxis_name)
                           Plot = Plot + xlab("F1 score weight")
                           #Plot = Plot + ggtitle(paste(entity,"\n(",unique(entity_outliers$Sample_count)," cell lines)\n(",reference_valueType,")",sep=''))
                           Plot = Plot + ggtitle("\n")
                           Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
                           Plot = Plot + theme(axis.title = element_text(hjust=0.5,face=2,size=axis_title_size))
                           Plot = Plot + theme(axis.text.x = element_text(hjust=0.5,face=2,size=axis_text_size,angle=90))
                           Plot = Plot + theme(axis.text.y = element_text(hjust=0.5,face=2,size=axis_text_size))
                           Outlier_identicationDocumentation_figures[[length(Outlier_identicationDocumentation_figures)+1]] = Plot
                           plots_on_current_page = plots_on_current_page +1
                           
                           entity_outliers_plot = entity_outliers
                           entity_outliers_plot$F1_score = entity_outliers_plot$Mean_F1score_without_outlier - entity_outliers_plot$F1_score_outlier
                           entity_outliers_plot$Outlier = "ARest"
                           entity_outliers_plot2 = entity_outliers
                           entity_outliers_plot2$F1_score = entity_outliers_plot2$F1_score_outlier
                           entity_outliers_plot = rbind(entity_outliers_plot,entity_outliers_plot2)
                           
                           Plot = ggplot(entity_outliers_plot,aes(x=F1_score_weight,y=F1_score,fill=Outlier))
                           Plot = Plot + geom_bar(stat="identity",width=bar_width,color = ifelse(entity_outliers_plot$Final_selection==outlier_finalSelection_yes_label,"black",sample_colors[1]),size=bar_frame_size)
                           Plot = Plot + scale_fill_manual(values=c("gray",sample_colors))
                           Plot = Plot + geom_hline(yintercept=minimum_average_F1_score)
                           Plot = Plot + theme(legend.position = "none")
                           Plot = Plot + ylab("(Mean) F1 score \n+/- outlier")
                           Plot = Plot + xlab("F1 score weight")
                           Plot = Plot + xlim(-0.05,1)
                           Plot = Plot + ggtitle(paste(fullDrugName,"\n(",unique(entity_outliers$Sample_count)," cell lines, ",unique(entity_outliers$Outlier),")",sep=''))
                           Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
                           Plot = Plot + theme(axis.title = element_text(hjust=0.5,face=2,size=axis_title_size))
                           Plot = Plot + theme(axis.text.x = element_text(hjust=0.5,face=2,size=axis_text_size,angle=90))
                           Plot = Plot + theme(axis.text.y = element_text(hjust=0.5,face=2,size=axis_text_size))
                           Outlier_identicationDocumentation_figures[[length(Outlier_identicationDocumentation_figures)+1]] = Plot
                           plots_on_current_page = plots_on_current_page +1
                           
                           Plot = ggplot(entity_outliers,aes(x=F1_score_weight,y=Median_correlation_reduced_full_data,fill=Outlier))
                           Plot = Plot + geom_bar(stat="identity",width=bar_width,color = ifelse(entity_outliers$Final_selection==outlier_finalSelection_yes_label,"black",sample_colors[1]),size=bar_frame_size)
                           Plot = Plot + scale_fill_manual(values=sample_colors)
                           Plot = Plot + theme(legend.text = element_text(hjust=0.5,face=2,size=legend_text_size))
                           legend <- cowplot::get_legend(Plot)
                           Plot = Plot + theme(legend.position = "none")
                           Plot = Plot + ylab("Median cos sim")
                           Plot = Plot + xlab("F1 score weight")
                           Plot = Plot + xlim(-0.05,1)
                           Plot = Plot + ylim(0,1)
                           #Plot = Plot + ggtitle(paste(entity,"\n(",unique(entity_outliers$Sample_count)," cell lines)\n(",reference_valueType,")",sep=''))
                           Plot = Plot + ggtitle("\n")
                           Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
                           Plot = Plot + theme(axis.title = element_text(hjust=0.5,face=2,size=axis_title_size))
                           Plot = Plot + theme(axis.text.x = element_text(hjust=0.5,face=2,size=axis_text_size,angle=90))
                           Plot = Plot + theme(axis.text.y = element_text(hjust=0.5,face=2,size=axis_text_size))
                           Outlier_identicationDocumentation_figures[[length(Outlier_identicationDocumentation_figures)+1]] = Plot
                           plots_on_current_page = plots_on_current_page +1
                           
                           if (plots_on_current_page == (plots_per_row *plots_per_col-3))
                           {#Begin
                             Outlier_identicationDocumentation_figures[[length(Outlier_identicationDocumentation_figures)+1]] = ggplot() + theme_void()
                             Outlier_identicationDocumentation_figures[[length(Outlier_identicationDocumentation_figures)+1]] = ggplot() + theme_void()
                             Outlier_identicationDocumentation_figures[[length(Outlier_identicationDocumentation_figures)+1]] = ggplot() + theme_void()
                             plots_on_current_page=0
                           }#End
  
                           #Outlier_identicationDocumentation_figures[[length(Outlier_identicationDocumentation_figures)+1]] = as_ggplot(legend)
                           
                         }#End - Generate Outlier documentation figures for each entity

                         cols_count = plots_per_col
                         max_plots_per_figure= cols_count * plots_per_row
                         figures_count = ceiling(length(Outlier_identicationDocumentation_figures)/max_plots_per_figure)
                       
                         while ((length(Outlier_identicationDocumentation_figures) %% max_plots_per_figure)!=0)
                         {#Begin - Fill empty outlier documentation figures on same page
                            Outlier_identicationDocumentation_figures[[length(Outlier_identicationDocumentation_figures)+1]] = ggplot() + theme_void()
                         }#End - Fill empty outlier documentation figures on same page
                       
                         for (indexF in 1:figures_count)
                         {#Begin - Plot outlier documentation figures into open PDF
                            startPlot = (indexF-1)*max_plots_per_figure+1
                            endPlot = min(indexF*max_plots_per_figure,length(Outlier_identicationDocumentation_figures))
                            current_plots = Outlier_identicationDocumentation_figures[startPlot:endPlot]
                            length_plot_list = length(current_plots)
                            rows_count = ceiling(length_plot_list / cols_count)
                            png_width = 3000*cols_count;
                            png_height = 500*rows_count;
                            do.call("grid.arrange",c(current_plots,nrow=rows_count,ncol=cols_count))
                         }#End - Plot outlier documentation figures into open PDF
                       
                         Plots = list()
                       
                         indexSig = which(rvt_outliers$Significant==outlier_significant_label)
                         sig_rvt_outliers = rvt_outliers[indexSig,]
                         rm(rvt_outliers)
                       
                         document_outlier_counts_per_cell=FALSE
                         if (document_outlier_counts_per_cell)
                         {#Begin - Document outlier counts per cell line and generate plots
                         Col_names = c("Outlier","F1_score_weight","Significant_outlier_count","Percentage_of_significant_outlier_count")
                         Col_length = length(Col_names)
                         Row_names = ""
                         Row_length = length(Row_names)
                         outlier_summary_base_line = as.data.frame(array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names)))
                         outlier_summary = c()
                         
                         samples = unique(sig_rvt_outliers$Outlier)
                         f1_score_weights = unique(sig_rvt_outliers$F1_score_weight)
                         indexF1SW=1
                         for (indexF1SW in 1:length(f1_score_weights))
                         {#Begin
                           f1_score_weight = f1_score_weights[indexF1SW]
                           indexCurrentF1SW = which(sig_rvt_outliers$F1_score_weight==f1_score_weight)
                           indexOut=1
                           for (indexOut in 1:length(samples))
                           {#Begin
                             outlier = samples[indexOut]
                             indexCurrentOutlier = which(sig_rvt_outliers$Outlier==outlier)
                             indexCount = indexCurrentOutlier
                             indexCount = indexCount[indexCount %in% indexCurrentF1SW]
                             outlier_summary_line = outlier_summary_base_line
                             outlier_summary_line$Outlier = strsplit(outlier,"_")[[1]][1]
                             outlier_summary_line$F1_score_weight = f1_score_weight
                             outlier_summary_line$Significant_outlier_count = length(indexCount)
                             indexCurrentCellLineCount =  which(names(treated_celllines_count_table)==outlier)
                             outlier_summary_line$Percentage_of_significant_outlier_count = 100 * outlier_summary_line$Significant_outlier_count / treated_celllines_count_table[indexCurrentCellLineCount]
                             
                             if (length(outlier_summary)>0) { outlier_summary = rbind(outlier_summary,outlier_summary_line) }
                             else { outlier_summary = outlier_summary_line }
                           }#End
                         }#End
                         
                         samples = unique(outlier_summary$Outlier)
                         samples = samples[order(samples)]
                         sample_colors = replicate(length(samples),"gray")
                         for (indexSC in 1:length(sample_colors))
                         {#Begin
                           sample = samples[indexSC]
                           indexCurrentSample = which(cellLines_for_highlight_colors==sample)
                           if (length(indexCurrentSample)>0)
                           { sample_colors[indexSC] = cellLine_highlight_colors[indexCurrentSample] }
                         }#End
                         
                         title_size=10
                         point_cex=0.5
                         Plot = ggplot(outlier_summary,aes(x=F1_score_weight,y=Percentage_of_significant_outlier_count,color=Outlier))
                         Plot = Plot + geom_point(cex=point_cex)
                         Plot = Plot + geom_line()
                         Plot = Plot + ylab("Significant outlier count [%]")
                         Plot = Plot + ggtitle(paste("Number of drugs that induced an outlier response in indicated cell lines\n(pvalue<=",significant_pvalue_cutoff,")\n(",reference_valueType,")",sep=''))
                         Plot = Plot + scale_color_manual(values=sample_colors)
                         Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
                         Plots[[length(Plots)+1]] = Plot
                       }#End - Document outlier counts per cell line and generate plots
                       
                         document_how_many_drugs_of_each_drug_target_induced_outlier_response=FALSE
                         if (document_how_many_drugs_of_each_drug_target_induced_outlier_response)
                         {#Begin - Dcoument how many drug of each drug target induce outlier response
                           drugTargets = read.csv(file=complete_drugTarget_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
                           indexColRemove = which(colnames(drugTargets) %in% c("Symbol","RowSum","RowMean","RowSampleSD","RowSampleCV","Description","ReadWrite_SCPs","ReadWrite_human_symbols"))
                           rownames(drugTargets) = drugTargets$Symbol
                           indexColKeep = 1:length(drugTargets[1,])
                           indexColKeep = indexColKeep[!indexColKeep %in% indexColRemove]
                           drugTargets = drugTargets[,indexColKeep]
                           Colnames = colnames(drugTargets)
                           Colnames = gsub("Cmap.","",Colnames)
                           Colnames = gsub(".NW_min_log10_pvalue","",Colnames)
                           Colnames = gsub("X.","",Colnames)
                           colnames(drugTargets) = Colnames
                           
                           indexColKeep = which(colnames(drugTargets) %in% allEntities)
                           drugTargets = drugTargets[,indexColKeep]
                           
                           Colnames = colnames(drugTargets)         
                           
                           Col_names = c("Drug_target","Cell_line","Number_of_drugs")
                           Col_length = length(Col_names)
                           Row_names = ""
                           Row_length = length(Row_names)
                           drugTarget_outlier_summary_base_line = as.data.frame(array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names)))
                           drugTarget_outlier_summaries = c()
                           
                           indexDT=2
                           for (indexDT in 1:length(drugTargets[,1]))
                           {#Begin
                             current_drugTarget = rownames(drugTargets)[indexDT]
                             indexCurrentDrugs = which(drugTargets[indexDT,]!=0)
                             current_drugs = colnames(drugTargets)[indexCurrentDrugs]
                             indexCurrentDrugsInFullEntityNames = which(allEntities %in% current_drugs)
                             indexCurrentDrugsInOutliers = which(sig_rvt_outliers$Entity %in% current_drugs)
                             table_celllines=0
                             if (length(indexCurrentDrugsInOutliers)>0)
                             {#Begin
                               current_sig_outliers = sig_rvt_outliers[indexCurrentDrugsInOutliers,]
                               current_sig_outliers$Unique_identifier = paste(current_sig_outliers$Entity,"-",current_sig_outliers$Outlier,sep='')
                               indexUnique = which(!duplicated(current_sig_outliers$Unique_identifier))
                               current_sig_outliers = current_sig_outliers[indexUnique,]
                               
                               indexTable=1
                               for (indexTable in 1:length(current_sig_outliers[,1]))
                               {#Begin
                                 drugTarget_outlier_summary_line = drugTarget_outlier_summary_base_line
                                 drugTarget_outlier_summary_line$Drug_target = current_drugTarget
                                 drugTarget_outlier_summary_line$Cell_line = strsplit(current_sig_outliers$Outlier[indexTable],"_")[[1]][1]
                                 drugTarget_outlier_summary_line$Drugs = current_sig_outliers$Entity[indexTable]
                                 drugTarget_outlier_summary_line$Number_of_drugs = 1
                                 if (length(drugTarget_outlier_summaries)>0) { drugTarget_outlier_summaries = rbind(drugTarget_outlier_summaries,drugTarget_outlier_summary_line)}
                                 else { drugTarget_outlier_summaries = drugTarget_outlier_summary_line }
                               }#End
                               indexMissing = which(!current_drugs %in% current_sig_outliers$Entity)
                               if (length(indexMissing)>0)
                               {#Begin
                                 for (indexIndex in 1:length(indexMissing))
                                 {#Begin
                                   drugTarget_outlier_summary_line = drugTarget_outlier_summary_base_line
                                   drugTarget_outlier_summary_line$Drug_target = current_drugTarget
                                   drugTarget_outlier_summary_line$Cell_line = "None"
                                   drugTarget_outlier_summary_line$Drugs= current_drugs[indexMissing[indexIndex]]
                                   drugTarget_outlier_summary_line$Number_of_drugs = 1
                                   if (length(drugTarget_outlier_summaries)>0) { drugTarget_outlier_summaries = rbind(drugTarget_outlier_summaries,drugTarget_outlier_summary_line)}
                                   else { drugTarget_outlier_summaries = drugTarget_outlier_summary_line }
                                 }#End
                               }#End
                             }#End
                           }#End
                           
                           drugs = unique(drugTarget_outlier_summaries$Drugs)
                           indexSDrug=1
                           for (indexSDrug in 1:length(drugs))
                           {#Begin
                             drug = drugs[indexSDrug]
                             indexColumn = which(colnames(drugTargets)==full_drugNames[[drug]])
                             drugTargetsCount = length(which(drugTargets[,indexColumn]!=0))
                             indexCurrentDrug = which(drugTarget_outlier_summaries$Drugs==drug)
                             #drugTarget_outlier_summaries$Drugs[indexCurrentDrug] = paste(drugTarget_outlier_summaries$Drugs[indexCurrentDrug],"\n(",drugTargetsCount,")",sep='')
                           }#End
                           
                           
                           drugTarget_outlier_summaries = drugTarget_outlier_summaries[order(drugTarget_outlier_summaries$Drug_target),]
                           plot_counts = 3
                           drug_targets = unique(drugTarget_outlier_summaries$Drug_target)
                           total_drug_targets_count = length(drug_targets)
                           drugTargets_per_plot = ceiling(total_drug_targets_count/plot_counts)
                           indexPlot=1
                           for (indexPlot in 1:plot_counts)
                           {#Begin
                             indexStart = (indexPlot-1)*drugTargets_per_plot+1
                             indexEnd = min((indexPlot)*drugTargets_per_plot,total_drug_targets_count)
                             current_drugTargets = drug_targets[indexStart:indexEnd]
                             
                             indexCurrentDrugTargets = which(drugTarget_outlier_summaries$Drug_target %in% current_drugTargets)
                             plot_summaries = drugTarget_outlier_summaries[indexCurrentDrugTargets,]
                             
                             cell_lines = unique(plot_summaries$Cell_line)
                             samples = cell_lines[order(cell_lines)]
                             sample_colors = replicate(length(samples),"gray")
                             for (indexSC in 1:length(sample_colors))
                             {#Begin
                               sample = samples[indexSC]
                               indexCurrentSample = which(cellLines_for_highlight_colors==sample)
                               if (length(indexCurrentSample)>0)
                               { sample_colors[indexSC] = cellLine_highlight_colors[indexCurrentSample] }
                             }#End
                             
                             Plot = ggplot(plot_summaries,aes(x=Drug_target,y=Number_of_drugs,fill=Cell_line))
                             Plot = Plot + geom_bar(stat="identity")
                             Plot = Plot + geom_text(aes(label=Drugs),position=position_stack(vjust=0.5),angle=90,size=2.5)
                             Plot = Plot + labs(fill="Outlier\ncell line")
                             Plot = Plot + xlab("Drug target proteins")
                             Plot = Plot + ylab("Drugs with\ndrug target protein\n(no. of drug targets in brackets)")
                             Plot = Plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
                             Plot = Plot + scale_fill_manual(values=sample_colors)
                             
                             Plots[[length(Plots)+1]] = Plot
                           }#End
                         }#End - Document how many drug of each drug target induce outlier response
                       
                         if (length(Plots)>0)      
                         {#Begin
                            max_plots_per_figure = max_plots_per_figure / cols_count
                            while ((length(Plots) %% max_plots_per_figure)!=0)
                            {#Begin
                              Plots[[length(Plots)+1]] = ggplot() + theme_void()
                            }#End
                            do.call("grid.arrange",c(Plots,nrow=rows_count,ncol=1))
                         }#End
                       
                         dev.off()
                       }#End
                     }#End - Generate outlier figures for each reference_valueType
                   }#End - Generate outlier figures for each reference_valueType, if outliers were found
                 }#End
                 
                 if (!addOrAnalyze_coCulture_data)
                 {#Begin
                   complete_fileName = complete_collapsed_entitySpecific_cluster_validation_f1_fileName
                 }#End
                 if (addOrAnalyze_coCulture_data)
                 {#Begin
                   complete_fileName = complete_coCulture_collapsed_entitySpecific_cluster_validation_f1_fileName
                 }#End
                 write.table(collapsed_cluster_validations, file = complete_fileName,row.names=FALSE,quote=FALSE,col.names=TRUE,sep='\t')
                 
                 ##########################################################################################################################
               }#End - indexCoreTask - SVD_7
               
             }#End - Parallel clusters
             
)

{#Begin - Print reports and write to comments
  comments_fileName = paste("Comments_",svd_script_name,".txt",sep='')
  print_to_console_array <- do.call('c', clusterEvalQ(parallel_clusters, print_to_console))
  if (length(print_to_console_array)>0)
  {#Begin
     directory_message_list = list()
     headline = "The following drug-F1_score_weight-reference_value_type were removed, since only one 'Final_selection_for_outlier_based_decomposition' is allowed:"
     print(headline)
     indexPA=1
     for (indexPA in 1:length(print_to_console_array))
     {#Begin
        print_to_console = print_to_console_array[indexPA]
        splitStrings = strsplit(print_to_console,"@")[[1]]
        message = splitStrings[2]
        general_directory = splitStrings[1]
        print(message)
        directory_message_list[[general_directory]] = c(directory_message_list[[general_directory]],message)
     }#End
     print(paste("All infomation is saved in '",comments_fileName,"' files within the related results directories",sep=''))
     general_directories = names(directory_message_list)
     indexGD=1
     for (indexGD in 1:length(general_directories))
     {#Begin
        general_directory = general_directories[indexGD]
        complete_comments_fileName = paste(general_directory,comments_fileName,sep='')
        writeLines(c(headline,unlist(directory_message_list[[general_directory]])), complete_comments_fileName)
     }#End
  }#End
}#End - Print reports and write to comments


{#Begin - Close parallel clusters
  invisible(gc())
  parallel::stopCluster(parallel_clusters)
  invisible(gc())
  rm(parallel_clusters)
}#End - Close parallel clusters
