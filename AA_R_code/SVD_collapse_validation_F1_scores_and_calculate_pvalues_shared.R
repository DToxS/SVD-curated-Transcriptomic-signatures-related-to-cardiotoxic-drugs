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
               
               indexCoreTask=1
               for (indexCoreTask in 1:length_core_tasks)
               {#Begin - indexCore
                 
                 progressReport = paste("core task no ",indexCoreTask," initiated",sep='')  
                 
                 current_task_line = core_tasks[indexCoreTask,]
                 addOrAnalyze_coCulture_data = current_task_line$AddOrAnalyze_coCulture_data
                 considered_permutations=NULL
                 if (length(current_task_line$Considered_permutations)>0)
                 { considered_permutations = strsplit(current_task_line$Considered_permutations,";")[[1]] }
                 f1_score_weight = current_task_line$F1_score_weight
                 
                 document_F1_scores_cosine_similarites = current_task_line$Document_F1_scores_cosine_similarites
                 
                 ##########################################################################################################################
                 ## Set global parameter - BEGIN
                 source('SVD_coreTaskSpecific_parameter.R')
                 source('SVD_colors.R')
                 ## Set global parameter - END
                 ##########################################################################################################################
                 
                 error_message = "";
                 
                 if (!addOrAnalyze_coCulture_data)
                 {#Begin
                   real_validations = read.csv(file=complete_entitySpecific_cluster_validation_f1_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');
                 }#End
                 if (addOrAnalyze_coCulture_data)
                 {#Begin
                   real_validations = read.csv(file=complete_coCulture_entitySpecific_cluster_validation_f1_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');
                 }#End
                 
                 if (length(unique(real_validations$Correlation_method))!=1) { real_validations = c() }
                 if (length(unique(real_validations$Preprocess_data))!=1) { real_validations = c() }
                 if (length(unique(real_validations$Decomposition_method))!=1) { real_validations = c() }
                 if (length(unique(real_validations$CumEigenexpression_cutoff))!=1) { real_validations = c() }
                 if (length(unique(real_validations$SigDEGs_correlation_method))!=1) { real_validations = c() }
                 
                 collapsed_real_validations = c()
                 entities = unique(real_validations$Entity)
                 indexKeep = which(drugs_in_drugClass_specific_order %in% entities)
                 indexMissing = which(!entities %in% drugs_in_drugClass_specific_order)
                 entities = c(drugs_in_drugClass_specific_order[indexKeep],entities[indexMissing])
                 referenceValueTypes = unique(real_validations$Reference_valueType)
                 EA_correlation_parameters = unique(real_validations$EA_correlation_parameter)
                 
                 document_differences_between_mean_and_median_cosine_similarity=FALSE
                 if (document_differences_between_mean_and_median_cosine_similarity)
                 {#Begin - Document difference between mean and median cosine similarity
                   indexReferenceValue=2
                   for (indexReferenceValue in 1:length(referenceValueTypes))
                   {#Begin - indexReferenceValue
                     referenceValue = referenceValueTypes[indexReferenceValue]
                     indexCurrentReferenceValue = which(real_validations$Reference_valueType==referenceValue)
                     current_referenceValue_validations = real_validations[indexCurrentReferenceValue,]
                     indexEA=1
                     for (indexEA in 1:length(EA_correlation_parameters))
                     {#Begin - indexEA 
                       EA_correlation_parameter = EA_correlation_parameters[indexEA]
                       complete_pdf_fileName = paste(complete_real_F1Scores_png_base_fileName,referenceValue,"_",EA_correlation_parameter,"_diffMeanMedian.pdf",sep='')
                       pdf(complete_pdf_fileName, width=8.5, height=11);
                       plots_row_count = 3
                       plots_col_count = 2
                       plots_per_page = plots_row_count * plots_col_count
                       F1_cosine_plots = list()
                       indexCurrentEA_correlation_parameter = which(real_validations$EA_correlation_parameter==EA_correlation_parameter)
                       current_EA_validations = current_referenceValue_validations[indexCurrentEA_correlation_parameter,]
                       indexEntity=1
                       diff_cosine_plots = list()
                       for (indexEntity in 1:length(entities))
                       {#Begin - indexEntity
                         entity = entities[indexEntity]
                         indexCurrentEntity = which(current_EA_validations$Entity==entity)
                         entity_real = current_EA_validations[indexCurrentEntity,]
                         figure_title = paste(gsub("_"," ",drug_drugClass[entity]),": ",full_drugNames[entity],sep='')
                         diff_cosine_plot = ggplot(entity_real,aes(x=Mean_correlation_reduded_full_data,y=Median_correlation_reduded_full_data))
                         diff_cosine_plot = diff_cosine_plot + geom_point()
                         diff_cosine_plot = diff_cosine_plot + geom_abline(intercept = 0, slope = 1, color="black", linetype="solid", size=1.5)
                         diff_cosine_plot = diff_cosine_plot + xlab("Mean cosine similarity") + ylab("Mediane cosine similarity")
                         diff_cosine_plot = diff_cosine_plot + theme(legend.position="none")
                         diff_cosine_plot = diff_cosine_plot + scale_colour_manual(values=c("blue"))
                         diff_cosine_plot = diff_cosine_plot + ggtitle(figure_title)
                         diff_cosine_plot = diff_cosine_plot + xlim(0,1) + ylim(0,1)
                         diff_cosine_plot = diff_cosine_plot + theme(plot.title = element_text(face="bold", size=15,hjust=0.5))
                         diff_cosine_plot = diff_cosine_plot + theme(axis.text = element_text(face="bold", size=10,hjust=0.5))
                         diff_cosine_plot = diff_cosine_plot + theme(axis.title = element_text(face="bold", size=10,hjust=0.5))
                         diff_cosine_plots[[length(diff_cosine_plots)+ 1]] = diff_cosine_plot
                         if ((length(diff_cosine_plots)==plots_per_page)|(indexEntity == length(entities)))
                         {#Begin
                           empty_plot = ggplot() + theme_void()
                           while (length(diff_cosine_plots)<plots_per_page)
                           { diff_cosine_plots[[length(diff_cosine_plots)+ 1]] = empty_plot }
                           do.call("grid.arrange",c(diff_cosine_plots,nrow=plots_row_count,ncol=plots_col_count))
                           diff_cosine_plots = list()
                         }#End
                       }#End - indexEntity
                       dev.off()
                     }#End - indexEA
                   }#End - indexReferenceValue
                 }#End - Document difference between mean and median cosine similarity
                 
                 eigenassays_count_full_data = -1
                 indexReferenceValue=1
                 for (indexReferenceValue in 1:length(referenceValueTypes))
                 {#Begin - indexReferenceValue: Calculate selection score and identify overall F1score and sample specific F1 scores
                   referenceValue = referenceValueTypes[indexReferenceValue]
                   indexCurrentReferenceValue = which(real_validations$Reference_valueType==referenceValue)
                   current_referenceValue_validations = real_validations[indexCurrentReferenceValue,]
                   indexEA=1
                   for (indexEA in 1:length(EA_correlation_parameters))
                   {#Begin - indexEA 
                     EA_correlation_parameter = EA_correlation_parameters[indexEA]
                     if (document_F1_scores_cosine_similarites)
                     {#Begin - Start PDF
                       complete_pdf_fileName = paste(complete_real_F1Scores_png_base_fileName,"_",referenceValue,"_",EA_correlation_parameter,"_F1ScoreWeight",f1_score_weight,".pdf",sep='')
                       pdf(complete_pdf_fileName, width=8.5, height=11);
                     }#End - Start PDF
                     plots_row_count = 3
                     plots_col_count = 2
                     plots_per_page = plots_row_count * plots_col_count
                     F1_cosine_plots = list()
                     indexCurrentEA_correlation_parameter = which(current_referenceValue_validations$EA_correlation_parameter==EA_correlation_parameter)
                     current_EA_validations = current_referenceValue_validations[indexCurrentEA_correlation_parameter,]

                     indexEntity = 3
                     for (indexEntity in 1:length(entities))
                     {#Begin - indexEntity
                       entity = entities[indexEntity]
                       indexCurrentEntity = which(current_EA_validations$Entity==entity)
                       entity_real = current_EA_validations[indexCurrentEntity,]

                       {#Begin - Select highestF1_highestMedianCorrelationReducedFullData for each subject
                         collapsed_cluster_validation_lines = c()
                         indexCollapsed = c()
                         
                         entity_real$Selection_value = (f1_score_weight * entity_real$F1_score + (1-f1_score_weight) * entity_real$Median_correlation_reduded_full_data)
                         entity_real$F1_score_weight = f1_score_weight
                         entity_real$Final_selection_for_sample = ""
                         
                         indexMaxEigenassay = which(entity_real$Eigenassays_count==max(entity_real$Eigenassays_count))
                         entity_real$F1_score_full_data = max(entity_real$F1_score[indexMaxEigenassay])
                         entity_real$Eigenassays_count_full_data = max(entity_real$Eigenassays_count)
                         
                         entity_real = entity_real[order(entity_real$F1_score,decreasing = TRUE),]
                         entity_real = entity_real[order(entity_real$Selection_value,decreasing = TRUE),]
                         
                         entity_real$Final_selection = finalSelection_notSelected
                         indexFullData = which(entity_real$Projected_data_type==full_dataType_label)
                         
                         add_collapsed_entity = c()
                         
                         if (length(indexFullData)>0)
                         {#Begin - Get highest F1 score for each sample in subspace that gives final selection
                           final_selection_entity_real = entity_real[1,]
                           final_selection_entity_real$Final_selection = finalSelection_fullData
                           entity_real[1,] = final_selection_entity_real
                           if (length(add_collapsed_entity)>0)  { add_collapsed_entity = rbind(add_collapsed_entity,final_selection_entity_real) }
                           if (length(add_collapsed_entity)==0) { add_collapsed_entity = final_selection_entity_real }
                           finalSelection_eigenassay_count = final_selection_entity_real$Eigenassays_count
                           
                           all_sample_groups = unique(entity_real$Samples_in_cluster)
                           all_samples = c()
                           for (indexG in 1:length(all_sample_groups))
                           {#Begin
                             all_samples = c(all_samples,strsplit(all_sample_groups[indexG],";")[[1]])
                           }#End
                           all_samples = unique(all_samples)
                           indexS=1
                           for (indexS in 1:length(all_samples))
                           {#Begin
                             sample = all_samples[indexS]
                             indexCurrentSample = grep(sample,entity_real$Samples_in_cluster)
                             indexCorrectEigenassayCount = which(entity_real$Eigenassays_count==finalSelection_eigenassay_count)
                             indexCurrentSample = indexCurrentSample[indexCurrentSample %in% indexCorrectEigenassayCount]
                             final_selection_entity_real = entity_real[indexCurrentSample[1],]
                             final_selection_entity_real$Final_selection_for_sample = sample
                             final_selection_entity_real$Final_selection = finalSelection_fullData_sampleSpecific
                             entity_real[indexCurrentSample[1],] = final_selection_entity_real
                             if (length(add_collapsed_entity)>0)  { add_collapsed_entity = rbind(add_collapsed_entity,final_selection_entity_real)  }
                             if (length(add_collapsed_entity)==0) { add_collapsed_entity = final_selection_entity_real }
                           }#End
                           Colors = c("orange3","firebrick2","slategray2")
                           text_sizes = c(5,5,3)
                           fontFaces = c("bold","bold","plain")
                         }#End - Get highest F1 score for each sample in subspace that gives final selection
                         
                         if (length(indexFullData)==0) { stop("No full data found") }
                         
                         if ((document_F1_scores_cosine_similarites)&(entity_real$EntityClass[1]=="Drug"))
                         {#Begin
                           entity_real$Selection_value_string = as.character(round(entity_real$Selection_value*100)/100)
                           entity_real$Selection_value_string = gsub("0.",".",entity_real$Selection_value_string)
                           entity_real$Visualization_category = paste(entity_real$Projected_data_type,"-",entity_real$Final_selection,sep='')
                           figure_title = paste(gsub("_"," ",drug_drugClass[entity]),": ",full_drugNames[entity],"\n(",entity_real$Entity_samples_count[1]," cell lines)\n(F1_score_weight: ",f1_score_weight,")",sep='')
                           
                           visualization_categories = unique(entity_real$Visualization_category)
                           visualization_categories = visualization_categories[order(visualization_categories)]
                           indexVF=1
                           for (indexVC in 1:length(visualization_categories))
                           {#Begin
                             visualization_category = visualization_categories[indexVC]
                             indexCurrentVisualization = which(entity_real$Visualization_category==visualization_category)
                             visu_entity = entity_real[indexCurrentVisualization,]
                             visu_color = Colors[indexVC]
                             if (indexVC==1)
                             {#Begin 
                               F1_cosine_plot = ggplot(visu_entity,aes(x=F1_score,y=Median_correlation_reduded_full_data,color=Visualization_category)) 
                               F1_cosine_plot = F1_cosine_plot + geom_text(aes(label=Selection_value_string),size=text_sizes[indexVC],fontface=fontFaces[indexVC])
                             }#End
                             else
                             {#Begin
                               F1_cosine_plot = F1_cosine_plot + geom_text(data = visu_entity, aes(x=F1_score,y=Median_correlation_reduded_full_data,label=Selection_value_string),size=text_sizes[indexVC],fontface=fontFaces[indexVC])
                             }#End
                           }#End
                           
                           F1_cosine_plot = F1_cosine_plot + scale_colour_manual(values=Colors)
                           #F1_cosine_plot = F1_cosine_plot + geom_point()
                           F1_cosine_plot = F1_cosine_plot + xlab("F1 score") + ylab("Mediane cosine similarity")
                           F1_cosine_plot = F1_cosine_plot + theme(legend.position="none")
                           F1_cosine_plot = F1_cosine_plot + ggtitle(figure_title)
                           F1_cosine_plot = F1_cosine_plot + xlim(0,1) + ylim(0,1)
                           F1_cosine_plot = F1_cosine_plot + theme(plot.title = element_text(face="bold", size=15,hjust=0.5))
                           F1_cosine_plot = F1_cosine_plot + theme(axis.text = element_text(face="bold", size=10,hjust=0.5))
                           F1_cosine_plot = F1_cosine_plot + theme(axis.title = element_text(face="bold", size=10,hjust=0.5))
                           F1_cosine_plots[[length(F1_cosine_plots)+ 1]] = F1_cosine_plot
                           
                           if ((length(F1_cosine_plots)==plots_per_page)|(indexEntity == length(entities)))
                           {#Begin
                             empty_plot = ggplot() + theme_void()
                             while (length(F1_cosine_plots)<plots_per_page)
                             { F1_cosine_plots[[length(F1_cosine_plots)+ 1]] = empty_plot }
                             do.call("grid.arrange",c(F1_cosine_plots,nrow=plots_row_count,ncol=plots_col_count))
                             F1_cosine_plots = list()
                           }#End
                           entity_real$Selection_value_string = NULL
                         }#End
                         
                         if (length(collapsed_real_validations)==0) { collapsed_real_validations = add_collapsed_entity }
                         else { collapsed_real_validations = rbind(collapsed_real_validations,add_collapsed_entity) }
                         
                       }#End - Select highestF1_highestMedianCorrelationReducedFullData for each subject
                     }#End - indexEntity
                     if (document_F1_scores_cosine_similarites) { dev.off() }#Close PDF
                   }#End - indexEA
                 }#End - indexReferenceValue: Calculate selection score and identify overall F1score and sample specific F1 scores
                 
                 entities = unique(collapsed_real_validations$Entity)
                 referenceValueTypes = unique(collapsed_real_validations$Reference_valueType)
                 EA_correlation_parameters = unique(collapsed_real_validations$EA_correlation_parameter)
                 
                 indexReferenceValue=1
                 for (indexReferenceValue in 1:length(referenceValueTypes))
                 {#Begin - indexReferenceValue
                   referenceValue = referenceValueTypes[indexReferenceValue]
                   
                   indexCurrentReferenceValue_colReal = which(collapsed_real_validations$Reference_valueType==referenceValue)
                   realCol_referenceValue_validations = collapsed_real_validations[indexCurrentReferenceValue_colReal,]
                   
                   indexEA=1
                   for (indexEA in 1:length(EA_correlation_parameters))
                   {#Begin - indexEA 
                     EA_correlation_parameter = EA_correlation_parameters[indexEA]
                     
                     indexCurrentEA_correlation_parameter_colReal = which(realCol_referenceValue_validations$EA_correlation_parameter==EA_correlation_parameter)
                     realCol_EAcorrelation_validations = realCol_referenceValue_validations[indexCurrentEA_correlation_parameter_colReal,]
                     
                     indexEntity=2
                     for (indexEntity in 1:length(entities))
                     {#Begin - indexEntity
                       entity = entities[indexEntity]
                       
                       indexCurrentEntity_realCol = which(realCol_EAcorrelation_validations$Entity==entity)
                       realCol_entity_validations = realCol_EAcorrelation_validations[indexCurrentEntity_realCol,]
                       
                       if (length(realCol_entity_validations[,1])!=1) { if (error_message=="") { error_message=paste(indexEntity,": ",entity,": (length(realCol_entity_validations)!=1)",sep='') }} #collapsed_real_validations = c(); 
                       
                     }#End - indexEntity
                     realCol_referenceValue_validations[indexCurrentEA_correlation_parameter_colReal,] = realCol_EAcorrelation_validations
                   }#End - indexEA
                   collapsed_real_validations[indexCurrentReferenceValue_colReal,] = realCol_referenceValue_validations
                 }#End - indexReferenceValue
                 
                 if (!addOrAnalyze_coCulture_data)
                 {#Begin
                   current_complete_fileName = complete_collapsed_entitySpecific_cluster_validation_f1_fileName
                 }#End
                 if (addOrAnalyze_coCulture_data)
                 {#Begin
                   current_complete_fileName = complete_coCulture_entitySpecific_cluster_validation_f1_fileName
                 }#End
                 
                 if (length(unique(collapsed_real_validations$Eigenassays_count_full_data))!=1) { stop("Multipe eigenassay count for full data") }
                 
                 current_complete_collapsed_entitySpecific_cluster_validation_f1_fileName = gsub(".txt",paste("_F1scoreWeight",current_task_line$F1_score_weight,".txt",sep=''),current_complete_fileName)
                 write.table(collapsed_real_validations,file=current_complete_collapsed_entitySpecific_cluster_validation_f1_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,sep='\t')
                 
               }#End - indexCoreTask
               
               ##########################################################################################################################
               
             })#End - Parallel clusters

{#Begin - Close parallel clusters
  invisible(gc())
  parallel::stopCluster(parallel_clusters)
  invisible(gc())
  rm(parallel_clusters)
}#End - Close parallel clusters



{#Begin - Combine individual files and write
  datasets = unique(tasks$Dataset)
  indexDataset=1
  for (indexDataset in 1:length(datasets))
  {#Begin
    dataset = datasets[indexDataset]
    indexCurrentDataset = which(tasks$Dataset==dataset)
    currentDataset_tasks = tasks[indexCurrentDataset,]
    combined_collapsed = c()
    indexTask=1
    for (indexTask in 1:length(currentDataset_tasks[,1]))
    {#Begin
      current_task_line = currentDataset_tasks[indexTask,]
      source('SVD_coreTaskSpecific_parameter.R')
      if (!addOrAnalyze_coCulture_data)
      {#Begin
         current_complete_fileName = complete_collapsed_entitySpecific_cluster_validation_f1_fileName
      }#End
      if (addOrAnalyze_coCulture_data)
      {#Begin
         current_complete_fileName = complete_coCulture_entitySpecific_cluster_validation_f1_fileName
      }#End
      current_complete_collapsed_entitySpecific_cluster_validation_f1_fileName = gsub(".txt",paste("_F1scoreWeight",current_task_line$F1_score_weight,".txt",sep=''),current_complete_fileName)
      add_collapsed = read.csv(file=current_complete_collapsed_entitySpecific_cluster_validation_f1_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
      if (length(combined_collapsed)==0) { combined_collapsed = add_collapsed }
      else { combined_collapsed = rbind(combined_collapsed,add_collapsed) }
      file.remove(current_complete_collapsed_entitySpecific_cluster_validation_f1_fileName)
    }#End
    if (!addOrAnalyze_coCulture_data)
    {#Begin
      complete_combined_fileName = complete_collapsed_entitySpecific_cluster_validation_f1_fileName
    }#End
    if (addOrAnalyze_coCulture_data)
    {#Begin
      complete_combined_fileName = complete_coCulture_collapsed_entitySpecific_cluster_validation_f1_fileName
    }#End
    
    write.table(combined_collapsed,file=complete_combined_fileName,quote = FALSE,row.names = FALSE,col.names=TRUE,sep='\t')
  }#End
}#End - Combine individual files and write


