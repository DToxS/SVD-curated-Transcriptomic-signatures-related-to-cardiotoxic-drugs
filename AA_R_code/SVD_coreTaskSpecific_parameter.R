####################################################################################################################
####################################################################################################################
## Define directories and fileNames - BEGIN

{#Begin - Define global labels
  full_dataType_label = "Full"
  finalSelection_trainingSetSubspace_clusterWithTestSample_label ="Subspace selected only based on training set, cluster includes test sample"
  finalSelection_trainingAndTestSetSubspace_clusterWithTestSample_label = "Subspace selected after projection of test set, cluster includes test sample"
  finalSelection_fullData = "Final selection in full data"
  finalSelection_fullData_sampleSpecific = "Final selection for sample in full data"
  finalSelection_notSelected ="Not selected"
  noLeftOutSample_label = "No left out sample for this drug"
  outlier_significant_label="Yes"
  outlier_not_significant_label="No"
  outlier_finalSelection_yes_label="Yes"
  outlier_finalSelection_no_label="No"
  coCulture_label_following_underline = "_coCulture"
}#End - Define global labels

{#Begin - Extract information from current tasks line
   preprocess_data = current_task_line$Preprocess_data
   dataset = current_task_line$Dataset
   correlation_method = current_task_line$Correlation_method
   decomposition_method = current_task_line$Decomposition_method
   dataset_coCulture = current_task_line$Dataset_coCulture
   addOrAnalyze_coCulture_data = current_task_line$AddOrAnalyze_coCulture_data
   
   if (!exists("generate_large_plots")) { generate_large_plots = FALSE }
   
}#End - Extract information from current tasks line

{#Begin - Set general directories
   general_directory = paste(lincs_results_directory,dataset,"/",sep='')
   general_coCulture_directory = paste(lincs_results_directory,dataset_coCulture,"/",sep='')
   degs_coCulture_directory = paste(general_coCulture_directory,"1_DEGs/",sep='')
   degs_directory = paste(general_directory,"1_DEGs/",sep='')
   icasso_directory = paste(general_directory,"2_Icasso/",sep='')
   angle_base_directory = paste(general_directory,"3_correlations_with_eigenassays/",sep='')
   if (!dir.exists(angle_base_directory)) { dir.create(angle_base_directory) } 
   statistics_enrichmentScore_base_directory = paste(general_directory,"4_Eigenassay_statistics_and_enrichmentScores/",sep='')
   if (!dir.exists(statistics_enrichmentScore_base_directory)) { dir.create(statistics_enrichmentScore_base_directory) }
   cluster_validation_f1_base_directory = paste(general_directory,"/5_Cluster_validation_f1_scores/",sep='')
   if (!dir.exists(cluster_validation_f1_base_directory)) { dir.create(cluster_validation_f1_base_directory) }
   coCulture_cluster_validation_f1_base_directory = paste(general_coCulture_directory,"/5_Cluster_validation_f1_scores/",sep='')
   if (!dir.exists(coCulture_cluster_validation_f1_base_directory)) { dir.create(coCulture_cluster_validation_f1_base_directory) }

   if (!addOrAnalyze_coCulture_data)
   {#Begin 
      visualization_of_entitySpecific_clusterDendrograms_base_directory = paste(general_directory,"7_Visualize_entitySpecific_cluster_dendrograms/",sep='')
      visualization_of_overall_clusterDendrograms_base_directory = paste(general_directory,"8_Visualize_overall_cluster_dendrograms/",sep='')
      visualization_of_cluster_validations_base_directory = paste(general_directory,"6_Visualize_cluster_validations/",sep='')
   }#End
   if (addOrAnalyze_coCulture_data)
   {#Begin 
      visualization_of_entitySpecific_clusterDendrograms_base_directory = paste(general_coCulture_directory,"7_Visualize_entitySpecific_cluster_dendrograms/",sep='')
      visualization_of_overall_clusterDendrograms_base_directory = paste(general_coCulture_directory,"8_Visualize_overall_cluster_dendrograms/",sep='')
      visualization_of_cluster_validations_base_directory = paste(general_coCulture_directory,"6_Visualize_cluster_validations/",sep='')
   }#End

   if (!dir.exists(visualization_of_cluster_validations_base_directory)) { dir.create(visualization_of_cluster_validations_base_directory) }

   if (!addOrAnalyze_coCulture_data)
   { drugSpecificExpressionValues_base_directory = paste(general_directory,"9_Drug_specific_expression_values/",sep='') }
   if (addOrAnalyze_coCulture_data)
   { drugSpecificExpressionValues_base_directory = paste(general_coCulture_directory,"9_Drug_specific_expression_values/",sep='') }
   if (!dir.exists(drugSpecificExpressionValues_base_directory)) { dir.create(drugSpecificExpressionValues_base_directory) }
   if (!addOrAnalyze_coCulture_data)
   { heatmap_directory = paste(general_directory,"10_Heatmaps/",sep='') }
   if (addOrAnalyze_coCulture_data)
   { heatmap_directory = paste(general_coCulture_directory,"10_Heatmaps/",sep='') }
   if (!dir.exists(heatmap_directory)) { dir.create(heatmap_directory) }
   crossValidationResults_directory = paste(general_directory,"12_CrossValidationResuls/",sep='')
   if (!dir.exists(crossValidationResults_directory)) { dir.create(crossValidationResults_directory) }
   outlier_directory = cluster_validation_f1_base_directory
   if (!dir.exists(outlier_directory)) { dir.create(outlier_directory) }
   outlier_coCulture_directory = coCulture_cluster_validation_f1_base_directory
   if (!dir.exists(outlier_coCulture_directory)) { dir.create(outlier_directory) }
   
   if (!addOrAnalyze_coCulture_data)
   { parameterExchangeWithCsharp_directory = paste(general_directory,"13_ParameterExchangeCsharp/",sep='') }
   if (addOrAnalyze_coCulture_data)
   { parameterExchangeWithCsharp_directory = paste(general_coCulture_directory,"13_ParameterExchangeCsharp/",sep='') }
   if (!dir.exists(parameterExchangeWithCsharp_directory)) { dir.create(parameterExchangeWithCsharp_directory) }
   doubleCheck_directory = paste(general_directory,"14_crosscheck/",sep='')
   if (!dir.exists(doubleCheck_directory)) { dir.create(doubleCheck_directory) }
}#End - Set general directories

{#Begin - Set actual result directories and update add_to_fileName, if permuation
  add_fileName = paste(preprocess_data,"_",correlation_method,"_",decomposition_method,sep='')
  angle_directory = angle_base_directory
  statistics_enrichmentScore_directory = statistics_enrichmentScore_base_directory
  cluster_validation_f1_directory = cluster_validation_f1_base_directory
  coCulture_cluster_validation_f1_directory = coCulture_cluster_validation_f1_base_directory
  visualization_of_cluster_validations_directory = visualization_of_cluster_validations_base_directory
  visualization_of_entitySpecific_clusterDendrograms_directory = visualization_of_entitySpecific_clusterDendrograms_base_directory
  visualization_of_overall_clusterDendrograms_directory = visualization_of_overall_clusterDendrograms_base_directory
  drugSpecificExpressionValues_directory = drugSpecificExpressionValues_base_directory
}#End - Set actual result directories and update add_to_fileName, if permuation

{#Begin - FEARS risks
   faers_directory = degs_directory
   faers_fileName = "FAERS_risks_original_and_permuted.txt"
   complete_faers_fileName = paste(faers_directory,faers_fileName,sep='')
}#End - FEARS risks

{#Begin - Directory_1: Set degs, degs_summary fileNames and drug target fileNames
  dataset_for_fileName = paste(dataset,"_topall",sep='')
  if ((grepl("no1stSVD",general_directory))&(!grepl("_no1stSVD_mA",general_directory))&(!grepl("_no1stSVD_CVno",general_directory)))
  {#Begin
     dataset_for_fileName = gsub("_no1stSVD","_no1stSVD_topall",dataset)
  }#End
  dataset_coCulture_for_fileName = paste(dataset_coCulture,"_topall",sep='')
  if ((grepl("no1stSVD",general_coCulture_directory))&(!grepl("_no1stSVD_mA",general_coCulture_directory))&(!grepl("_no1stSVD_CVno",general_coCulture_directory)))
  {#Begin
    dataset_coCulture_for_fileName = gsub("_no1stSVD","_no1stSVD_topall",dataset_coCulture)
  }#End
  
  degs_fileName = paste(dataset_for_fileName,".txt",sep='')
  degs_coCulture_fileName = paste(dataset_coCulture_for_fileName,".txt",sep='')
  degs_testSet_fileName = gsub("_topall","_test_topall",degs_fileName);
  degs_matrixMatlab_fileName = paste(dataset_for_fileName,"_matrix.txt",sep='')
  degs_colNamesMatlab_fileName = paste(dataset_for_fileName,"_colNames.txt",sep='')
  degs_rowNamesMatlab_fileName = paste(dataset_for_fileName,"_rowNames.txt",sep='')
  drugTarget_fileName = "Drug_target_proteins.txt"
  drugTargetSignalingPathway_fileName = "Drug_target_protein_signaling_pathways.txt"
  
  degs_coCulture_fileName = paste(dataset_coCulture_for_fileName,".txt",sep='')
  
  sig_input_directory = degs_directory
  sig_input_coCulture_directory = degs_coCulture_directory
  complete_degs_fileName = paste(degs_directory,degs_fileName,sep='')
  complete_degs_matrixMatlab_fileName = paste(degs_directory,degs_matrixMatlab_fileName,sep='')
  complete_degs_colNamesMatlab_fileName = paste(degs_directory,degs_colNamesMatlab_fileName,sep='')
  complete_degs_rowNamesMatlab_fileName = paste(degs_directory,degs_rowNamesMatlab_fileName,sep='')
  complete_degs_summary_fileName = paste(sig_input_directory,"Deg_summary.txt",sep='')
  complete_degs_summary_coCulture_fileName = paste(sig_input_coCulture_directory,"Deg_summary.txt",sep='')
  complete_drugTarget_fileName = paste(degs_directory,drugTarget_fileName,sep='')
  complete_drugTargetSignalingPathway_fileName = paste(degs_directory,drugTargetSignalingPathway_fileName,sep='')
}#End - Directory_1: Set degs, degs_summary fileNames and drug target fileNames

{#Begin - Directory_3: Set correlation fileNames
  eigenassay_correlations_fileName_withoutExtension = paste("Correlations_with_eigenassays_",add_fileName,sep='')
  complete_eigenassay_correlations_fileName = paste(angle_directory,eigenassay_correlations_fileName_withoutExtension,".txt",sep='')
  core_complete_eigenassay_correlations_fileName = paste(angle_directory,eigenassay_correlations_fileName_withoutExtension,"_core",indexCore,".txt",sep='')
}#End - Directory_3: Set correlation fileNames

{#Begin - Directory 4: Set statistics and enrichmentScores fileNames
  eigenassay_correlation_statistics_fileName_withoutExtension = paste("Eigenassay_correlations_statistics_",add_fileName,sep='')
  eigenassay_correlation_sigDEGs_correlation_fileName_withoutExtension = paste("Eigenassay_correlation_with_sigDEGs_",add_fileName,sep='')
  eigenassay_correlation_FAERS_correlation_fileName_withoutExtension = paste("Eigenassay_correlation_with_FAERS_",add_fileName,sep='')
  eigenassay_correlation_ANOVAs_fileName_withoutExtension = paste("Eigenassay_ANOVA_with_categories_",add_fileName,sep='')
  subspace_FAERS_correlations_fileName_withoutExtension = paste("Subspace_correlation_with_FAERS_",add_fileName,sep='')
  
  complete_eigenassay_correlation_statistics_fileName = paste(statistics_enrichmentScore_directory,eigenassay_correlation_statistics_fileName_withoutExtension,".txt",sep='')
  complete_eigenassay_correlation_sigDEGs_correlation_fileName = paste(statistics_enrichmentScore_directory,eigenassay_correlation_sigDEGs_correlation_fileName_withoutExtension,".txt",sep='')
  complete_eigenassay_correlation_FAERS_correlation_fileName = paste(statistics_enrichmentScore_directory,eigenassay_correlation_FAERS_correlation_fileName_withoutExtension,".txt",sep='')
  complete_eigenassay_correlation_ANOVAs_fileName = paste(statistics_enrichmentScore_directory,eigenassay_correlation_ANOVAs_fileName_withoutExtension,".txt",sep='')
  complete_subspace_FAERS_correlations_fileName = paste(statistics_enrichmentScore_directory,subspace_FAERS_correlations_fileName_withoutExtension,".txt",sep='')
  
  core_complete_eigenassay_correlation_statistics_fileName = paste(statistics_enrichmentScore_directory,eigenassay_correlation_statistics_fileName_withoutExtension,"_core",indexCore,".txt",sep='')
  core_complete_eigenassay_correlation_sigDEGs_correlation_fileName = paste(statistics_enrichmentScore_directory,eigenassay_correlation_sigDEGs_correlation_fileName_withoutExtension,"_core",indexCore,".txt",sep='')
  core_complete_eigenassay_correlation_FAERS_correlation_fileName = paste(statistics_enrichmentScore_directory,eigenassay_correlation_FAERS_correlation_fileName_withoutExtension,"_core",indexCore,".txt",sep='')
  core_complete_eigenassay_correlation_ANOVAs_fileName = paste(statistics_enrichmentScore_directory,eigenassay_correlation_ANOVAs_fileName_withoutExtension,"_core",indexCore,".txt",sep='')
  core_complete_subspace_FAERS_correlations_fileName = paste(statistics_enrichmentScore_directory,subspace_FAERS_correlations_fileName_withoutExtension,"_core",indexCore,".txt",sep='')
}#End - Directory 4: Set statistics and enrichmentScores fileNames

{#Begin - Directory 5: Set validation fileNames
  cluster_validation_f1_fileName_withoutExtension = paste("Cluster_validation_",add_fileName,sep='')
  entitySpecific_cluster_validation_f1_fileName = paste("EntitySpecific_validation_",add_fileName,sep='')
  complete_cluster_validation_f1_fileName = paste(cluster_validation_f1_directory,cluster_validation_f1_fileName_withoutExtension,".txt",sep='')
  
  complete_entitySpecific_cluster_validation_f1_fileName = paste(cluster_validation_f1_directory,entitySpecific_cluster_validation_f1_fileName,".txt",sep='')
  core_complete_entitySpecific_cluster_validation_f1_fileName = paste(cluster_validation_f1_directory,entitySpecific_cluster_validation_f1_fileName,"_core",indexCore,".txt",sep='')
  
  complete_coCulture_entitySpecific_cluster_validation_f1_fileName = paste(coCulture_cluster_validation_f1_directory,entitySpecific_cluster_validation_f1_fileName,".txt",sep='')
  core_complete_coCulture_entitySpecific_cluster_validation_f1_fileName = paste(coCulture_cluster_validation_f1_directory,entitySpecific_cluster_validation_f1_fileName,"_core",indexCore,".txt",sep='')
  
  complete_collapsed_entitySpecific_cluster_validation_f1_fileName = paste(cluster_validation_f1_directory,"EntitySpecific_cluster_validation_collapsed_",add_fileName,"_no1stSVD.txt",sep='')
  complete_coCulture_collapsed_entitySpecific_cluster_validation_f1_fileName = paste(coCulture_cluster_validation_f1_directory,"EntitySpecific_cluster_validation_collapsed_",add_fileName,"_no1stSVD.txt",sep='')
  complete_real_F1Scores_png_base_fileName  = paste(cluster_validation_f1_directory,"F1_allReal_cosineSim_",add_fileName,sep='')

  exclude_eigenassay_string = ""
  if (exists("exclude_eigenassays"))
  {#Begin
     if (length(exclude_eigenassays)==1) { exclude_eigenassay_string = paste("_no",exclude_eigenassays,"EA",sep='')}
     if (length(exclude_eigenassays)>1)  { exclude_eigenassay_string = paste("_no",paste(exclude_eigenassays,collapse="_",sep=''),"EA",sep='')}
  }#Enn
  
  original_term = "full"
  if (grepl("no1stSVD",dataset)) { original_term = "no1stSVD" }
  
  complete_originalExpressionValues_fileName = paste(drugSpecificExpressionValues_directory,"Drug_specific_expression_",original_term,".txt",sep='')
  
  if (f1_score_weight!=-1) { f1_score_weight_text = f1_score_weight }
  if (f1_score_weight==-1) { f1_score_weight_text = "byOutlier" }
  complete_eigenassay_fileName = paste(drugSpecificExpressionValues_directory,"All_eigenassays_including_removed_ones.txt",sep='')
  complete_drugSpecificExpressionValues_fileName = paste(drugSpecificExpressionValues_directory,"Drug_specific_expression_decomposed",exclude_eigenassay_string,"_F1SW",f1_score_weight_text,".txt",sep='')
  complete_drugRemovedExpressionValues_fileName = paste(drugSpecificExpressionValues_directory,"Drug_removed_expression_decomposed",exclude_eigenassay_string,"_F1SW",f1_score_weight_text,".txt",sep='')
  complete_drugSpecificExpressionValues_after_bg_removal_fileName = paste(drugSpecificExpressionValues_directory,"Drug_specific_expression_decomposed_after_bgRemoval",exclude_eigenassay_string,".txt",sep='')
  complete_drugSpecificExpressionValues_after_bg_removal_equal_length_fileName = paste(drugSpecificExpressionValues_directory,"Drug_specific_expression_decomposed_after_bgRemovalEqualLength",exclude_eigenassay_string,".txt",sep='')
  complete_fullExpressionValues_fileName = paste(drugSpecificExpressionValues_directory,"Drug_specific_expression_full.txt",sep='')
}#End - Directory 5: Set validation fileNames

{#Begin - Directory 6: Set visualization of cluster validations fileNames
   complete_visualization_of_entitySpecific_clusterDendrograms_directory = visualization_of_entitySpecific_clusterDendrograms_base_directory
}#End - Directory 6: Set visualization of cluster validations fileNames

{#Begin - Directory 10: Set heatmap fileNames
  heatmap_baseFileName = paste(add_fileName,"heatmap",sep='')
  colDend_baseFileName = paste("SVD","_colDend",sep='')
  if (addOrAnalyze_coCulture_data)
  {#Begin
     colDend_baseFileName = paste(colDend_baseFileName,coCulture_label_following_underline,sep='')
     heatmap_baseFileName = paste(heatmap_baseFileName,coCulture_label_following_underline,sep='')
  }#End

  complete_heatmap_baseFileName = paste(heatmap_directory,heatmap_baseFileName,sep='')
  complete_colDend_baseFileName = paste(heatmap_directory,colDend_baseFileName,sep='')
}#End - Directory 10: Set heatmap fileNames

{#Begin - Directory 12: Set cross validation results file names
  rcolors_fileName = "RColors.txt"
  complete_rcolores_fileName = paste(parameterExchangeWithCsharp_directory,rcolors_fileName,sep='')
  aucCutoffRank_fileName = "AUC_cutoff_ranks.txt"
  complete_aucCutoffRank_fileName = paste(parameterExchangeWithCsharp_directory,aucCutoffRank_fileName,sep='')
}#End - Directory 12: Set cross validation results file names

{#Begin - Directory 14 - Set cross check directory
   crosscheck_fileName = "Cross_check.txt"
   complete_crosscheck_fileName = paste(doubleCheck_directory,crosscheck_fileName,sep='')
}#End - Directory 14 - Set cross check directory

{#Begin - Directory 7: Set outlier responses file names
  outlier_fileName = "Outlier_responses.txt"
  complete_outlier_fileName = paste(outlier_directory,outlier_fileName,sep='')
  complete_coCulture_outlier_fileName = paste(outlier_coCulture_directory,outlier_fileName,sep='')
}#End - Directory 7: Set outlier responses file names

{#Begin - Define indexes of entityClasses in columnNames
  indexDrug_in_colname = 3
  indexDrugType_in_colname = 1
  indexCellLine_in_colname = 2
  indexPlate_in_colname = 4
}#End - Define indexes of entityClasses in columnNames
 
{#Begin - Define drugs with similar targets 
  drugs_with_similar_targets = list()
  indexList=0
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("PIO","ROS","AMI","FLE","ISO","VRP","DOB","PHP")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("CER","CRI")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("BOS","TRA")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("DAB","VEM")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("CAB","PON","REG","SOR","AXI","PAZ","SUN","DAS","IMA","NIL")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("EST")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("BEV","TRS","VAN","AFA","LAP","ERL","GEF");
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("BOR","CAR");
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("RUX","TOF");
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("EPI","DOX","IDA","AZA","DEC")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("SAX")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("RTX")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("PRE")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("MIL")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("OLM")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("TNF")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("EDN")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("IGF")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("CYC")
  indexList=indexList+1
  drugs_with_similar_targets[[indexList]] = c("CTX")

  defined_drugTargetClasses = c()
  defined_drugs_in_drugTargetClasses = c()
  for (indexDST in 1:length(drugs_with_similar_targets))
  {#Begin
    current_drugs = drugs_with_similar_targets[[indexDST]]
    current_drugTargetClass = paste("DrugTargetClass",indexDST,sep='')
    defined_drugTargetClasses = c(defined_drugTargetClasses,c(replicate(length(current_drugs),current_drugTargetClass)))
    defined_drugs_in_drugTargetClasses = c(defined_drugs_in_drugTargetClasses,current_drugs)                     
  }#End
}#End - Define drugs with similar targets 
  
delimiter = '\t'
