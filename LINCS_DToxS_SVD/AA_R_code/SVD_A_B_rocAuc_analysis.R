rm(list = ls());
rm(list = ls(all.names = TRUE));
gc();
library(ggplot2)
library(gridExtra)
library(ggbeeswarm)

get_eigenassays_per_dataset = FALSE
delete_task_reports = FALSE
add_inFrontOf_progress_report_fileName = "SVD_16"
source('SVD_global_parameter.R')

baseDirectory = overall_lincs_directory

topDEGs=600
scp_pvalue = 0.05

generate_small_plots=FALSE
test_reversed_f1score=TRUE

overall_dataset = "DEGenes_iPSCdCMs_P0"

drugLegendDirectory = paste(baseDirectory,"Experimental_data//Metadata//",sep='')
drugLegendFileName = "Drug_metadata.txt"
complete_drugLegendFileName = paste(drugLegendDirectory,drugLegendFileName,sep='')
directory = paste(enrichment_results_directory,overall_dataset,"_enrichment_maxP",scp_pvalue,"_top",topDEGs,"DEGs_in_decomposed_rocForFractional_rank//",sep='')
ontologies = c("MBCOL1","MBCOL2","MBCOL3","MBCOL4")

ontology_ontology_library_fileName = list()
ontology_ontology_library_fileName[["MBCOL1"]] = "Mbco_level1_Homo_sapiens.txt"
ontology_ontology_library_fileName[["MBCOL2"]] = "Mbco_level2_Homo_sapiens.txt"
ontology_ontology_library_fileName[["MBCOL3"]] = "Mbco_level3_Homo_sapiens.txt"
ontology_ontology_library_fileName[["MBCOL4"]] = "Mbco_level4_Homo_sapiens.txt"

source('SVD_colors.R')
source('Common_tools.R')

ontology_minRankCutoff_list = list(  "MBCOL1"=0
                                    ,"MBCOL2"=0
                                    ,"MBCOL3"=0
                                    ,"MBCOL4"=0
                                  )

risk_assessment_maximize_valueTypes = c("F1score_thisAUC_and_diffAUC")
weight_of_penalty_AUC = 0.5; #(>0 and <1)
outlier_statuses = c("O","N")
drugTypes_of_interest = c("Kinase_inhibitor","Monoclonal_antibody","Anthracycline")
drugToxicity_color_list = list("diffrna_up-Toxic" = "navy",
                               "1" = "royalblue4",
                               "2" = "orange",
                               "3" = "darkred")
drugToxicity_2ncolor_list = list("0" = "lightsteelblue3",
                                 "1" = "lightskyblue1",
                                 "2" = "lightsalmon",
                                 "3" = "salmon3")

empty_scp_name = c("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz empty ")


ontology_maxRankCutoff_list = ontology_maxRank_list

beta_f1score = 0.25 #Define beta_f1score range
minimum_precision_dashed_line = 0.9

minimum_recall_protective_toBeConsidered_for_precision = 0.2
minimum_recall_harmful_toBeConsidered_for_precision = 0.10
precision_minimum_true_positives_count=5

generate_precisionRecallF1score_vs_cutoffRank_plots_rowsCount = 5
generate_precisionRecallF1score_vs_cutoffRank_plots_colsCount = 3

add_toEnrichmentFile_for_published_data = "_publishedData"
add_toEnrichmentFile = "_maxP0.05.txt"
add_toEnrichmentFile = ".txt"
auc_base_fileName = "Auc_of_roc_"
roc_base_fileName = "Roc_"
end_fileName = paste("_top",topDEGs,"_Minus_log10_pvalue.txt",sep='')
end_fileName = paste("_top",topDEGs,"_Fractional_rank.txt",sep='')
roc_minimum_true_positives_count=0

top_x_scps = 1000
bottom_x_scps = 1000
if (generate_small_plots) { max_scps_per_spreadsheet_figure=60 }
if (!generate_small_plots) { max_scps_per_spreadsheet_figure=47 }


color_2nd_list = list("Diffrna" = "aquamarine",
                      "Diffrna_up" = "gold",
                      "Diffrna_down" = "lightskyblue",
                      "Diffrna_upminusdown" = "orchid",
                      "E_m_p_t_y" = "black")


color_list = list("Diffrna" = "forestgreen",
                  "Diffrna_up" = "orangered",
                  "Diffrna_down" = "navy",
                  "Diffrna_upminusdown" = "darkorchid",
                  "Diffrna_up;Diffrna_down" = "aquamarine",
                  "Diffrna_down;Diffrna_up" = "gray",
                  "precison_line" = "black",
                  "E_m_p_t_y" = "black")

precisionRecall_color_list = list()                         
precisionRecall_color_list[[paste("Diffrna_up-",tki_harmful_label,sep='')]] = spreadsheet_color_list[[paste("Diffrna_up-",tki_harmful_label,sep='')]]
precisionRecall_color_list[[paste("Diffrna_down-",tki_protective_label,sep='')]] = spreadsheet_color_list[[paste("Diffrna_down-",tki_protective_label,sep='')]]
precisionRecall_color_list[[paste("Diffrna_down-",tki_harmful_label,sep='')]] = spreadsheet_color_list[[paste("Diffrna_down-",tki_harmful_label,sep='')]]
precisionRecall_color_list[[paste("Diffrna_up-",tki_protective_label,sep='')]] = spreadsheet_color_list[[paste("Diffrna_up-",tki_protective_label,sep='')]]
precisionRecall_color_list[[paste("Diffrna_up-",anthracycline_label,sep='')]] = spreadsheet_color_list[[paste("Diffrna_up-",anthracycline_label,sep='')]]
precisionRecall_color_list[[paste("Diffrna_down-",high_tox_TKI_label,sep='')]] = spreadsheet_color_list[[paste("Diffrna_down-",high_tox_TKI_label,sep='')]]
precisionRecall_color_list[[paste("Diffrna_down-",anthracycline_label,sep='')]] = spreadsheet_color_list[[paste("Diffrna_down-",anthracycline_label,sep='')]]
precisionRecall_color_list[[paste("Diffrna_up-",high_tox_TKI_label,sep='')]] = spreadsheet_color_list[[paste("Diffrna_up-",high_tox_TKI_label,sep='')]]

indexO=3

side_effect_groups_list = list( "Cardiotoxicity" = "Cardiotoxicity",
                                "ATC vs high tox. TKI" = "ATC vs high tox. TKI")

#Part of manuscript
generate_comparison_with_clusterMarkerGenes= TRUE
generate_precisionRecallF1score_vs_cutoffRank_plots = TRUE
generate_comparison_with_adultDCMHCM_data = TRUE
generate_comparison_with_coCuluture_data = TRUE

na_color = "white"
high_color_down = "blue"
low_color_down = "cyan"
high_color_up = "orange"
low_color_up = "yellow"
text_color="black"
cellLine_full_text_color="gray"
cellLine_outlier_text_color="purple"
cellLine_nonOutlier_text_color="black"

entryTypes_of_interest = c("Diffrna_up","Diffrna_down")

SideEffect_precisionRecallF1_vs_rankCutoff_plots = list()
SideEffect_spreadsheet_plots = list()
SideEffect_collapsed_spreadsheet_plots=list()
SideEffect_toxic_collapsed_spreadsheet_plots=list()
SideEffect_comparison_collapsed_spreadheet_plots_array=list()
SideEffect_toxic_comparison_collapsed_spreadheet_plots_array=list()

{#Begin - Read drug legend
   drugLegend = read.csv(file=complete_drugLegendFileName,header=TRUE,sep='\t')
   indexKeep = which(drugLegend$Drug_type %in% drugTypes_of_interest)
   drugLegend = drugLegend[indexKeep,]
}#End - Read drug legend

Split_names_over_multiple_lines = function(names, max_nchar_per_line, max_lines)
{#Begin
   unique_names = unique(names)
   for (indexUnique in 1:length(unique_names))
   {#Begin
      uniqueName = unique_names[indexUnique]
      indexCurrentName = which(names==uniqueName)
      new_uniqueName = Split_name_over_multiple_lines(uniqueName,max_nchar_per_line,max_lines)
      names[indexCurrentName] = new_uniqueName
   }#End
   return (names)
}#End

Duplicate_roc_to_generate_harmful_and_protective_roc = function(roc,harmful_label,protective_label)
{#Begin
   harmful_sideEffect_roc = roc
   harmful_sideEffect_roc$Precision = harmful_sideEffect_roc$Precision_harmful
   harmful_sideEffect_roc$Recall = harmful_sideEffect_roc$Recall_harmful
   harmful_sideEffect_roc$F1score = harmful_sideEffect_roc$F1score_harmful
   harmful_sideEffect_roc$Association = harmful_label
   indexKeep = which(harmful_sideEffect_roc$F1score>0)
   harmful_sideEffect_roc = harmful_sideEffect_roc[indexKeep,]
   
   protective_sideEffect_roc = roc
   protective_sideEffect_roc$Precision = protective_sideEffect_roc$Precision_protective
   protective_sideEffect_roc$Recall = protective_sideEffect_roc$Recall_protective
   protective_sideEffect_roc$Association = protective_label
   protective_sideEffect_roc$F1score = protective_sideEffect_roc$F1score_protective
   indexKeep = which(protective_sideEffect_roc$F1score>0)
   protective_sideEffect_roc = protective_sideEffect_roc[indexKeep,]
   
   plot_sideEffect = rbind(harmful_sideEffect_roc,protective_sideEffect_roc)
   plot_sideEffect$EntryType_association = paste(plot_sideEffect$Entry_type,"-",plot_sideEffect$Association,sep='')
   return (plot_sideEffect)
}#End

Add_maxF1ScoreAbsDifference_and_F1ScoreAUCs = function(harmProt_roc,last_considered_rocRankCutoff,weight_of_penalty_AUC)
{#Begin
  harmProt_roc$F1score_thisAUC_and_diffAUC = -1
  harmProt_roc$Max_cutoff_rank_for_AUC = last_considered_rocRankCutoff
  
  associations = unique(harmProt_roc$Association)
  indexAsso=1
  for (indexAsso in 1:length(associations))
  {#Begin
    association = associations[indexAsso]
    indexCurrentAssociation = which(harmProt_roc$Association==association)
    asso_harmProt_roc = harmProt_roc[indexCurrentAssociation,]
    scps = unique(asso_harmProt_roc$ReadWrite_disease_neighborhoods)
    indexScp=1
    for (indexScp in 1:length(scps))
    {#Begin
      scp = scps[indexScp]
      indexCurrentScp = which(asso_harmProt_roc$ReadWrite_disease_neighborhoods==scp)
      scp_roc = asso_harmProt_roc[indexCurrentScp,]
      scp_roc = scp_roc[order(scp_roc$Roc_cutoff),]
      maxF1score_difference=0
      upAUC=0
      downAUC=0
      lastUpRocCutoff=1
      lastDownRocCutoff=1
      upF1Score_atLastUpCutoff=0
      downF1Score_atLastDownCutoff=0
      indexScpRoc=1
      indexScpRoc=indexScpRoc+1
      for (indexScpRoc in 1:length(scp_roc[,1]))
      {#Begin
        current_scp_roc_line = scp_roc[indexScpRoc,]
        if (current_scp_roc_line$Roc_cutoff<=last_considered_rocRankCutoff)
        {#Begin
          if (current_scp_roc_line$Entry_type=="Diffrna_up")
          {#Begin - Add to upAUC
            upAUC = upAUC + (current_scp_roc_line$Roc_cutoff - lastUpRocCutoff) * upF1Score_atLastUpCutoff
            upF1Score_atLastUpCutoff = current_scp_roc_line$F1score
            lastUpRocCutoff = current_scp_roc_line$Roc_cutoff
          }#End - Add to upAUC
          if (current_scp_roc_line$Entry_type=="Diffrna_down")
          {#Begin - Add to downAUC
            downAUC = downAUC + (current_scp_roc_line$Roc_cutoff - lastDownRocCutoff) * downF1Score_atLastDownCutoff
            downF1Score_atLastDownCutoff = current_scp_roc_line$F1score
            lastDownRocCutoff = current_scp_roc_line$Roc_cutoff
          }#End - Add to downAUC
        }#End
      }#End
      upAUC = upAUC + (last_considered_rocRankCutoff - lastUpRocCutoff) * upF1Score_atLastUpCutoff
      downAUC = downAUC + (last_considered_rocRankCutoff - lastDownRocCutoff) * downF1Score_atLastDownCutoff
      upAUC = upAUC * 100 /  ((last_considered_rocRankCutoff-1) * 1)
      downAUC = downAUC * 100 /  ((last_considered_rocRankCutoff-1) * 1)
      indexUp = which(scp_roc$Entry_type=="Diffrna_up")
      indexDown = which(scp_roc$Entry_type=="Diffrna_down")
      scp_roc$F1score_thisAUC_and_diffAUC[indexUp]   = upAUC   - weight_of_penalty_AUC * downAUC
      scp_roc$F1score_thisAUC_and_diffAUC[indexDown] = downAUC - weight_of_penalty_AUC * upAUC
      if (length(which(!colnames(scp_roc)%in%colnames(asso_harmProt_roc)))!=0) { rm(scp_rco);rm(asso_harmProt_roc) }
      if (length(which(!colnames(asso_harmProt_roc)%in%colnames(scp_roc)))!=0) { rm(scp_rco);rm(asso_harmProt_roc) }
      asso_harmProt_roc[indexCurrentScp,] = scp_roc
    }#End
    harmProt_roc[indexCurrentAssociation,] = asso_harmProt_roc
  }#End
  return (harmProt_roc)
}#End

Add_maxF1ScoreAbsDifference_and_F1ScoreAUCs_reverse = function(harmProt_roc,last_considered_rocRankCutoff,weight_of_penalty_AUC)
{#Begin
  harmProt_roc$F1score_thisAUC_and_diffAUC = -1
  harmProt_roc$Max_cutoff_rank_for_AUC = last_considered_rocRankCutoff
  
  associations = unique(harmProt_roc$Association)
  indexAsso=1
  for (indexAsso in 1:length(associations))
  {#Begin
    association = associations[indexAsso]
    indexCurrentAssociation = which(harmProt_roc$Association==association)
    asso_harmProt_roc = harmProt_roc[indexCurrentAssociation,]
    scps = unique(asso_harmProt_roc$ReadWrite_disease_neighborhoods)
    indexScp=1
    for (indexScp in 1:length(scps))
    {#Begin
      scp = scps[indexScp]
      indexCurrentScp = which(asso_harmProt_roc$ReadWrite_disease_neighborhoods==scp)
      scp_roc = asso_harmProt_roc[indexCurrentScp,]
      scp_roc = scp_roc[order(scp_roc$Roc_cutoff),]
      maxF1score_difference=0
      upAUC=0
      downAUC=0
      lastUpRocCutoff=1
      lastDownRocCutoff=1
      upF1Score_atLastUpCutoff=1  #only difference to regular function, rest is copy and paste
      downF1Score_atLastDownCutoff=1  #only difference to regular function, rest is copy and paste
      indexScpRoc=1
      indexScpRoc=indexScpRoc+1
      for (indexScpRoc in 1:length(scp_roc[,1]))
      {#Begin
        current_scp_roc_line = scp_roc[indexScpRoc,]
        if (current_scp_roc_line$Roc_cutoff<=last_considered_rocRankCutoff)
        {#Begin
          if (current_scp_roc_line$Entry_type=="Diffrna_up")
          {#Begin - Add to upAUC
            upAUC = upAUC + (current_scp_roc_line$Roc_cutoff - lastUpRocCutoff) * upF1Score_atLastUpCutoff
            upF1Score_atLastUpCutoff = current_scp_roc_line$F1score
            lastUpRocCutoff = current_scp_roc_line$Roc_cutoff
          }#End - Add to upAUC
          if (current_scp_roc_line$Entry_type=="Diffrna_down")
          {#Begin - Add to downAUC
            downAUC = downAUC + (current_scp_roc_line$Roc_cutoff - lastDownRocCutoff) * downF1Score_atLastDownCutoff
            downF1Score_atLastDownCutoff = current_scp_roc_line$F1score
            lastDownRocCutoff = current_scp_roc_line$Roc_cutoff
          }#End - Add to downAUC
        }#End
      }#End
      upAUC = upAUC + (last_considered_rocRankCutoff - lastUpRocCutoff) * upF1Score_atLastUpCutoff
      downAUC = downAUC + (last_considered_rocRankCutoff - lastDownRocCutoff) * downF1Score_atLastDownCutoff
      upAUC = upAUC * 100 /  ((last_considered_rocRankCutoff-1) * 1)
      downAUC = downAUC * 100 /  ((last_considered_rocRankCutoff-1) * 1)
      indexUp = which(scp_roc$Entry_type=="Diffrna_up")
      indexDown = which(scp_roc$Entry_type=="Diffrna_down")
      scp_roc$F1score_thisAUC_and_diffAUC[indexUp]   = upAUC   - weight_of_penalty_AUC * downAUC
      scp_roc$F1score_thisAUC_and_diffAUC[indexDown] = downAUC - weight_of_penalty_AUC * upAUC
      if (length(which(!colnames(scp_roc)%in%colnames(asso_harmProt_roc)))!=0) { rm(scp_rco);rm(asso_harmProt_roc) }
      if (length(which(!colnames(asso_harmProt_roc)%in%colnames(scp_roc)))!=0) { rm(scp_rco);rm(asso_harmProt_roc) }
      asso_harmProt_roc[indexCurrentScp,] = scp_roc
    }#End
    harmProt_roc[indexCurrentAssociation,] = asso_harmProt_roc
  }#End
  return (harmProt_roc)
}#End

Add_opposing_maxF1ScoreAbsDifference_and_F1ScoreAUCs = function(harmProt_roc,first_considered_rocRankCutoff,last_considered_rocRankCutoff,single_and_diff_weight)
{#Begin
   harmProt_roc$F1score_thisAUC_and_diffAUC = -1
   
   up_associations = unique(harmProt_roc$Association)
   indexAsso=1
   for (indexAsso in 1:length(up_associations))
   {#Begin
      up_association = up_associations[indexAsso]
      indexCurrentAssociation_up = which((harmProt_roc$Association==up_association)&(harmProt_roc$Entry_type=="Diffrna_up"))
      indexOtherAssociation_down = which((harmProt_roc$Association!=up_association)&(harmProt_roc$Entry_type=="Diffrna_down"))
      indexCurrentAssociations = c(indexCurrentAssociation_up,indexOtherAssociation_down)
      if (length(which(duplicated(indexCurrentAssociations)))>0) { rm (harmProt_roc) }
      asso_harmProt_roc = harmProt_roc[indexCurrentAssociations,]
      scps = unique(asso_harmProt_roc$ReadWrite_disease_neighborhoods)
      indexScp=1
      for (indexScp in 1:length(scps))
      {#Begin
         scp = scps[indexScp]
         indexCurrentScp = which(asso_harmProt_roc$ReadWrite_disease_neighborhoods==scp)
         scp_roc = asso_harmProt_roc[indexCurrentScp,]
         scp_roc = scp_roc[order(scp_roc$Roc_cutoff),]
         maxF1score_difference=0
         upAUC=0
         opposingEffect_downAUC=0
         lastUpRocCutoff=1
         lastDownRocCutoff=1
         upF1Score_atLastUpCutoff=0
         downF1Score_atLastDownCutoff=0
         indexScpRoc=1
         for (indexScpRoc in 1:length(scp_roc[,1]))
         {#Begin
            current_scp_roc_line = scp_roc[indexScpRoc,]
            if (current_scp_roc_line$Roc_cutoff<=last_considered_rocRankCutoff)
            {#Begin
               if (current_scp_roc_line$Entry_type=="Diffrna_up")
               {#Begin - Add to upAUC
                  upAUC = upAUC + (current_scp_roc_line$Roc_cutoff - lastUpRocCutoff) * upF1Score_atLastUpCutoff
                  upF1Score_atLastUpCutoff = current_scp_roc_line$F1score
                  lastUpRocCutoff = current_scp_roc_line$Roc_cutoff
               }#End - Add to upAUC
               if (current_scp_roc_line$Entry_type=="Diffrna_down")
               {#Begin - Add to opposingEffect_downAUC
                  opposingEffect_downAUC = opposingEffect_downAUC + (current_scp_roc_line$Roc_cutoff - lastDownRocCutoff) * downF1Score_atLastDownCutoff
                  downF1Score_atLastDownCutoff = current_scp_roc_line$F1score
                  lastDownRocCutoff = current_scp_roc_line$Roc_cutoff
               }#End - Add to opposingEffect_downAUC
            }#End
         }#End
         upAUC = upAUC + (last_considered_rocRankCutoff - lastUpRocCutoff) * upF1Score_atLastUpCutoff
         opposingEffect_downAUC = opposingEffect_downAUC + (last_considered_rocRankCutoff - lastDownRocCutoff) * downF1Score_atLastDownCutoff
         upAUC = upAUC * 100 /  (last_considered_rocRankCutoff * 1)
         opposingEffect_downAUC = opposingEffect_downAUC * 100 /  (last_considered_rocRankCutoff * 1)
         indexUp = which(scp_roc$Entry_type=="Diffrna_up")
         indexDown = which(scp_roc$Entry_type=="Diffrna_down")
         scp_roc$F1score_thisAUC_and_diffAUC[indexUp]   = single_and_diff_weight * upAUC   + (1-single_and_diff_weight) * (opposingEffect_downAUC)
         scp_roc$F1score_thisAUC_and_diffAUC[indexDown] = single_and_diff_weight * opposingEffect_downAUC + (1-single_and_diff_weight) * (upAUC)
         if (length(which(!colnames(scp_roc)%in%colnames(asso_harmProt_roc)))!=0) { rm(scp_rco);rm(asso_harmProt_roc) }
         if (length(which(!colnames(asso_harmProt_roc)%in%colnames(scp_roc)))!=0) { rm(scp_rco);rm(asso_harmProt_roc) }
         asso_harmProt_roc[indexCurrentScp,] = scp_roc
      }#End
      harmProt_roc[indexCurrentAssociations,] = asso_harmProt_roc
   }#End
   return (harmProt_roc)
}#End

AddEmptyLinesCountIfNecessary_setScpFactor_and_setBlackOrWhiteScpColors = function(local_precision_summaries,scps_in_correct_spreadsheet_order,max_scps_per_spreadsheet_figure)
{#Begin
   showScps = unique(local_precision_summaries$Scp)
   add_empty_lines_count = max_scps_per_spreadsheet_figure-length(showScps)
   label_colors = replicate(length(showScps),"black")
   associations = unique(local_precision_summaries$Association)
   if (add_empty_lines_count>0)
   {#Begin
     for (indexAdd in 1:add_empty_lines_count)
     {#Begin
        new_add_line = local_precision_summaries[1:length(associations),]
        new_add_line$Association = associations
        new_add_line$Scp = paste("zzzzzz empty ",indexAdd,sep='')
        new_add_line$Recall=0
        new_add_line$Selected=FALSE
        new_add_line$Field_label = ""
        new_add_line$Entry_type = "E_m_p_t_y"
        new_add_line$Entry_type_association = paste(new_add_line$Entry_type,"-",new_add_line$Association,sep='')
        local_precision_summaries = rbind(new_add_line,local_precision_summaries)
        label_colors=c("white",label_colors)
      }#End
   }#End
   indexMissing = which(!local_precision_summaries$Scp %in% scps_in_correct_spreadsheet_order)
   scps_in_correct_spreadsheet_order_current = c(unique(local_precision_summaries$Scp[indexMissing]),scps_in_correct_spreadsheet_order)
   local_precision_summaries$Scp_factor = factor(local_precision_summaries$Scp,levels=scps_in_correct_spreadsheet_order_current)
   
   return_list = list()
   return_list[["precision_summaries"]] = local_precision_summaries
   return_list[["label_colors"]] = label_colors
   return (return_list)
}#End

Collapse_scps_that_are_both_up_and_downregulated_for_the_same_association_in_precision_summaries = function(local_precision_summaries)
{#Begin
   local_precision_summaries$Association_scp = paste(local_precision_summaries$Association,"-",local_precision_summaries$Scp,sep='')
   indexesDuplicated = which(duplicated(local_precision_summaries$Association_scp))
   if (length(indexesDuplicated)!=0)
   {#Begin - Collapse scps that are up- and downregulated
      indexUnique = which(!duplicated(local_precision_summaries$Association_scp))
      indexesDuplicated = indexesDuplicated[order(indexesDuplicated,decreasing = TRUE)]
      for (indexIndex in 1:length(indexesDuplicated))
      {#Begin
        indexSecondLine = indexesDuplicated[indexIndex]
        indexFirstLine = indexSecondLine - 1
        local_precision_summaries$Field_label[indexFirstLine] =
          paste(local_precision_summaries$Field_label[indexFirstLine],"\n",local_precision_summaries$Field_label[indexSecondLine],sep='')
      }#End
      local_precision_summaries = local_precision_summaries[indexUnique,]
   }#End - Collapse scps that are up- and downregulated
   local_precision_summaries$Association_scp = NULL
   return (local_precision_summaries)
}#End

comparison_enrichments=list()
comparison_sampleNames_in_rightOrders=list()

empty_rank_entry = 9999
emptyRank_field_label = "X"

{#Begin - Set scRNAseq labels and upDown colors
  scRNASeq_adjPvalue = 0.05
  scRNASeq_topDEGs = 500
  
  foundBycardiotoxicDrugs_label = "Tox TKI"
  foundByNoncardiotoxicDrugs_label = "Not tox TKI"
  own_scRNAseq_label = "iPSCd "
  aH_scRNAseq_label = "adult "
  own_vcm_label = paste(own_scRNAseq_label,"VCM",sep='')
  own_epc_label = paste(own_scRNAseq_label,"EPC",sep='')
  own_cNCC_label = paste(own_scRNAseq_label,"cNCC",sep='')
  own_acm_label = paste(own_scRNAseq_label,"CM I",sep='')
  own_mcm_label = paste(own_scRNAseq_label,"CM II",sep='')
  aH_vcm_label = paste(aH_scRNAseq_label,"VCM",sep='')
  aH_acm_label = paste(aH_scRNAseq_label,"ACM",sep='')
  aH_fb_label = paste(aH_scRNAseq_label,"CFB",sep='')
  aH_smc_label = paste(aH_scRNAseq_label,"SMC",sep='')
  aH_lym_label = paste(aH_scRNAseq_label,"LYM",sep='')
  aH_mes_label = paste(aH_scRNAseq_label,"MES",sep='')
  aH_myl_label = paste(aH_scRNAseq_label,"MYL",sep='')
  aH_neu_label = paste(aH_scRNAseq_label,"NEU",sep='')
  aH_per_label = paste(aH_scRNAseq_label,"PER",sep='')
  aH_ap_label = paste(aH_scRNAseq_label,"AP",sep='')
  aH_ec_label = paste(aH_scRNAseq_label,"EC",sep='')
  
  upDown_color_list = list()
  upDown_color_list[["Diffrna_up"]]="firebrick"
  upDown_color_list[["Diffrna_down"]]="dodgerblue4"
  upDown_color_list[["Diffrna_up;Diffrna_down"]]="firebrick1"
  upDown_color_list[["Diffrna_down;Diffrna_up"]]="dodgerblue1"
  upDown_color_list[["E_m_p_t_y"]] = "gray"
  upDown_color_list[[own_vcm_label]] = "indianred"
  upDown_color_list[[own_epc_label]] = "goldenrod4"
  upDown_color_list[[own_cNCC_label]] = "mediumseagreen"
  upDown_color_list[[own_acm_label]] = "cadetblue3"
  upDown_color_list[[own_mcm_label]] = "cornflowerblue"

  upDown_color_list[[aH_vcm_label]] = "indianred"
  upDown_color_list[[aH_acm_label]] = "cadetblue3"
  upDown_color_list[[aH_fb_label]] = "goldenrod4"
  upDown_color_list[[aH_smc_label]] = "goldenrod4"
  upDown_color_list[[aH_lym_label]] = "gray"
  upDown_color_list[[aH_mes_label]] = "gray"
  upDown_color_list[[aH_myl_label]] = "gray"
  upDown_color_list[[aH_neu_label]] = "gray"
  upDown_color_list[[aH_per_label]] = "gray"
  upDown_color_list[[aH_ap_label]] = "gray"
  upDown_color_list[[aH_ec_label]] = "gray"

  upDown_color_list[[foundBycardiotoxicDrugs_label]] = "gray50"
  upDown_color_list[[foundByNoncardiotoxicDrugs_label]] = "gray50"
  
  upDown_color_list[["DCM"]] = "orange"
  upDown_color_list[["HCM"]] = "blue"
  upDown_color_list[["RCM"]] = "orchid"
}#End - Set scRNAseq labels and upDown colors

if (generate_comparison_with_clusterMarkerGenes)
{#Begin
  iPSCdCM_sc_cluster_cellType_list = list(  "0" = paste(own_scRNAseq_label,"VCM",sep='')
                                    ,"3" = paste(own_scRNAseq_label,"CM I",sep='')
                                    ,"4" = paste(own_scRNAseq_label,"CM II",sep='')
                                    ,"1" = paste(own_scRNAseq_label,"EPC",sep='')
                                    ,"2" = paste(own_scRNAseq_label,"cNCC",sep='')
                                    #,"Cluster_5_vs_Rest" = paste(own_scRNAseq_label,"EP",sep='')
  )
  
  aH_cluster_cellType_list = list( 
                                   "Ventricular_Cardiomyocyte" = paste(aH_scRNAseq_label,"VCM",sep='')
                                  ,"Atrial_Cardiomyocyte" = paste(aH_scRNAseq_label,"ACM",sep='')
                                  ,"Fibroblast" = paste(aH_scRNAseq_label,"CFB",sep='')
                                  ,"Smooth_muscle_cells" = paste(aH_scRNAseq_label,"SMC",sep='')
                                  #,"Endothelial" = paste(aH_scRNAseq_label,"EC",sep='')
                                  #,"Lymphoid" = paste(aH_scRNAseq_label,"LYM",sep='')                 
                                  #,"Mesothelial" = paste(aH_scRNAseq_label,"MES",sep='')
                                  #,"Myeloid" = paste(aH_scRNAseq_label,"MYL",sep='')
                                  #,"Neuronal" = paste(aH_scRNAseq_label,"NEU",sep='')
                                  #,"NotAssigned" = paste(aH_scRNAseq_label,"NA",sep='')
                                  #,"Pericytes" = paste(aH_scRNAseq_label,"PER",sep='')                
                                  #,"Adipocytes" = paste(aH_scRNAseq_label,"AP",sep='')
  )
}#End

if (generate_comparison_with_adultDCMHCM_data)
{#Begin
   dataset_fileNameAddition_list = list()
   dataset_fileNameAddition_list[["SC adult"]] = "_ScRNASeq_koenig_DCM_filtered"
   dataset_fileNameAddition_list[["SN adult"]] = "_SnRNASeq_chaffin_DCM_HCM_filtered"
   dataset_fileNameAddition_list[["SC iPSCd"]] = "_ScRNASeq_chun_DCM_filtered"

   dataset_keepSampleNames_list = list()
   dataset_keepSampleNames_list[["SC adult"]] = c("DCM_vs_healthy_in_cardiomyocytes","DCM_vs_healthy_in_fibroblasts")
   dataset_keepSampleNames_list[["SN adult"]] = c("DCM_vs_NF_in_Cardiomyocyte","DCM_vs_NF_in_Fibroblast","HCM_vs_NF_in_Cardiomyocyte","HCM_vs_NF_in_Fibroblast")
   dataset_keepSampleNames_list[["SC iPSCd"]] = c("DCM_vs_healthy_in_iPSCdCMs")
   
   datasets = names(dataset_fileNameAddition_list)
   indexD=2
   published_enrichment = c()
   for (indexD in 1:length(datasets))
   {#Begin
      dataset = datasets[indexD]
      fileNameAddition = dataset_fileNameAddition_list[[dataset]]
      indexO = 1
      dataset_enrich = c()
      for (indexO in 1:length(ontologies))
      {#Begin
         ontology = ontologies[indexO]
         complete_fileName = paste(scSnRnaSeq_enrichment_directory,ontology,fileNameAddition,".txt",sep='')
         add_enrich = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
         add_enrich$Ontology = ontology
         if (length(dataset_enrich)>0) { dataset_enrich = rbind(dataset_enrich,add_enrich) }
         else { dataset_enrich = add_enrich }
      }#End
      keepSampleNames = dataset_keepSampleNames_list[[dataset]]
      indexKeep = which(dataset_enrich$Sample_name %in% keepSampleNames)
      dataset_enrich = dataset_enrich[indexKeep,]
      dataset_enrich$Sample_name = paste(dataset," ",dataset_enrich$Sample_name,sep='')
      dataset_enrich$Sample_name = gsub("_vs_healthy_in_"," ",dataset_enrich$Sample_name)
      dataset_enrich$Sample_name = gsub("_vs_NF_in_"," ",dataset_enrich$Sample_name)
      dataset_enrich$Sample_name = gsub("Cardiomyocyte","CM",dataset_enrich$Sample_name)
      dataset_enrich$Sample_name = gsub("Fibroblast","FB",dataset_enrich$Sample_name)
      dataset_enrich$Sample_name = gsub("cardiomyocytes","CM",dataset_enrich$Sample_name)
      dataset_enrich$Sample_name = gsub("fibroblasts","FB",dataset_enrich$Sample_name)
      dataset_enrich$Sample_name = gsub("iPSCdCMs","CM",dataset_enrich$Sample_name)
      if (length(published_enrichment)>0) { published_enrichment = rbind(published_enrichment,dataset_enrich) }
      else { published_enrichment = dataset_enrich }
   }#End
   sampleNames = unique(published_enrichment$Sample_name)
   indexSN=1
   for (indexSN in 1:length(sampleNames))
   {#Begin
      sampleName = sampleNames[indexSN]
      splitStrings = strsplit(sampleName," ")[[1]]
      rearranged_sampleName=""
      for (indexSplit in 1:(length(splitStrings)-2))
      {#Begin
         splitString = splitStrings[indexSplit]
         if (indexSplit>1) { rearranged_sampleName = paste(rearranged_sampleName," ",splitString,sep='') }
         else {rearranged_sampleName = splitString }
      }#End
      rearranged_sampleName = paste(rearranged_sampleName," ",splitStrings[length(splitStrings)]," ",splitStrings[length(splitStrings)-1],sep='')
      indexCurrentSampleName = which(published_enrichment$Sample_name==sampleName)
      published_enrichment$Sample_name[indexCurrentSampleName] = rearranged_sampleName
   }#End

   published_sampleNames_in_rightOrder = c("SC iPSCd CM DCM","SC adult CM DCM","SN adult CM DCM","SN adult CM HCM","SC adult FB DCM","SN adult FB DCM","SN adult FB HCM")
   indexMissing = which(!published_enrichment$Sample_name %in% published_sampleNames_in_rightOrder)
   if (length(indexMissing)>0) { stop("Not all sample names are in published_sampleNames_in_rightOrder") }

   comparison_enrichments[["Published"]] = published_enrichment
   comparison_sampleNames_in_rightOrders[["Published"]] = c(published_sampleNames_in_rightOrder,foundBycardiotoxicDrugs_label,foundByNoncardiotoxicDrugs_label)
}#End

if (generate_comparison_with_coCuluture_data)
{#Begin
  coCulture_directory = gsub("_P0_","_ECCoCulture_",directory)
  coCulture_ontologies = c("MBCOL1","MBCOL2","MBCOL3","MBCOL4")
  coCulture_enrich = c()
  indexO=1
  for (indexO in 1:length(coCulture_ontologies))
  {#Begin
     coCulture_ontology = coCulture_ontologies[indexO]
     complete_fileName = paste(coCulture_directory,coCulture_ontology,".txt",sep='')
     add_enrich = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
     max_rank = ontology_maxRankCutoff_list[[coCulture_ontology]]
     indexKeep = which(add_enrich$Pvalue<=scp_pvalue)
     add_enrich = add_enrich[indexKeep,]
     indexKeep = which(add_enrich$Fractional_rank<=max_rank)
     add_enrich = add_enrich[indexKeep,]
     if (length(coCulture_enrich)>0) { coCulture_enrich = rbind(coCulture_enrich,add_enrich) }
     else { coCulture_enrich = add_enrich }
  }#End
  
  sampleNames = unique(coCulture_enrich$Sample_name)
  indexS=1
  for (indexS in 1:length(sampleNames))
  {#Begin
     sampleName = sampleNames[indexS]
     splitStrings = strsplit(sampleName,'-')[[1]]
     drug = splitStrings[1]
     cellLine_splitStrings = strsplit(splitStrings[2],'_')[[1]]
     cellline = "error"
     if (length(cellLine_splitStrings)==2) { cellline = cellLine_splitStrings[1] }
     if (length(cellLine_splitStrings)==3) { cellline = paste(cellLine_splitStrings[1]," ",cellLine_splitStrings[3],sep='') }
     indexCurrentSampleName = which(coCulture_enrich$Sample_name==sampleName)
     coCulture_enrich$Sample_name[indexCurrentSampleName] = paste(drug,"-",cellline,sep='')
  }#End

  indexDab = grep("DAB",coCulture_enrich$Sample_name)
  dab_coCulture_enrich = coCulture_enrich[indexDab,]
  dab_coCulture_enrich$Sample_name = gsub("DAB-","",dab_coCulture_enrich$Sample_name)
  indexPaz = grep("PAZ",coCulture_enrich$Sample_name)
  paz_coCulture_enrich = coCulture_enrich[indexPaz,]
  paz_coCulture_enrich$Sample_name = gsub("PAZ-","",paz_coCulture_enrich$Sample_name)
  
  dab_coCulture_enrich = dab_coCulture_enrich[order(dab_coCulture_enrich$Sample_name),]
  comparison_enrichments[["Dabrafenib"]] = dab_coCulture_enrich
  comparison_sampleNames_in_rightOrders[["Dabrafenib"]] = c(unique(dab_coCulture_enrich$Sample_name),foundBycardiotoxicDrugs_label,foundByNoncardiotoxicDrugs_label)

  paz_coCulture_enrich = paz_coCulture_enrich[order(paz_coCulture_enrich$Sample_name),]
  comparison_enrichments[["Pazopanib"]] = paz_coCulture_enrich
  comparison_sampleNames_in_rightOrders[["Pazopanib"]] = c(unique(paz_coCulture_enrich$Sample_name),foundBycardiotoxicDrugs_label,foundByNoncardiotoxicDrugs_label)
}#End

{#Begin - Initiate precision summaries
  Col_names = c("Side_effect","Ontology","Scp","Scp_completeName","Association","Entry_type","Cutoff_rank","Max_cutoff_rank_for_AUC","Selection_rank","Selected","Precision","Recall","F1_score","ValueForSelection","ValueTypeForSelection","ReadWrite_drugs")
  Col_length = length(Col_names)
  Row_names = ""
  Row_length = length(Row_names)
  precision_summary_base_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
  precision_summary_base_line = as.data.frame(precision_summary_base_line)
  precision_summaries = c()
}#End - Initiate precision summaries

missing_scps_in_scps_in_right_order = c()

indexO=1
for (indexO in 1:length(ontologies))
{#Begin
   ontology = ontologies[indexO]
   print(paste("Analyze and generate figures for ",ontology,sep=''))
   sideEffect_scps_forVisuRoc_list = list()
   sideEffect_scps_forVisuPrecision_list = list()
   
   {#Begin - Read and prepare ROC
      roc_fileName = paste(roc_base_fileName,ontology,end_fileName,sep='')
      complete_roc_fileName = paste(directory,roc_fileName,sep='')
      roc = read.csv(complete_roc_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
      indexKeepRoc = which(roc$Entry_type %in% entryTypes_of_interest)
      roc = roc[indexKeepRoc,]
      indexKeepRoc = which(roc$Side_effect %in% unlist(side_effect_groups_list))
      roc = roc[indexKeepRoc,]
      sideEffects = unique(roc$Side_effect)
      final_roc = c()
      indexSideEffect=1
      for (indexSideEffect in 1:length(sideEffects))
      {#Begin - Prepare side effect roc and check for correctness
         sideEffect = sideEffects[indexSideEffect]
         indexCurrentSideEffect = which(roc$Side_effect==sideEffect)
         se_roc = roc[indexCurrentSideEffect,]
      
         total_positives = se_roc$True_positives_length + se_roc$False_negatives_length
         total_negatives = se_roc$False_positives_length + se_roc$True_negatives_length
         if (length(unique(total_positives))!=1) { stop("(length(unique(total_positives))!=1)") }
         if (length(unique(total_negatives))!=1) { stop("(length(unique(total_negatives))!=1)") }
         if (unique(total_positives+total_negatives)!=unique(se_roc$Total_samples_count)) { stop("unique(total_positives+total_negatives)!=unique(se_roc$Total_samples_count)") }
        
         check_scps=unique(se_roc$ReadWrite_disease_neighborhoods)
         indexCheckScp=1
         for (indexCheckScp in 1:length(check_scps))
         {#Begin
            check_scp = check_scps[indexCheckScp]
            indexCurrentCheckScp = which(se_roc$ReadWrite_disease_neighborhoods==check_scp)
            if (max(se_roc$False_positive_rate[indexCurrentCheckScp])!=1) { stop("(max(se_roc$False_positive_rate[indexCurrentCheckScp])!=1)") }
            if (max(se_roc$True_positive_rate[indexCurrentCheckScp])!=1) { stop("(max(se_roc$True_positive_rate[indexCurrentCheckScp])!=1)") }
            if (max(se_roc$True_positives_length[indexCurrentCheckScp]+se_roc$False_positives_length[indexCurrentCheckScp])!=unique(se_roc$Total_samples_count[indexCurrentCheckScp])) { stop("max(se_roc$True_positives_length[indexCurrentCheckScp]+se_roc$False_positives_length[indexCurrentCheckScp])!=unique(se_roc$Total_samples_count[indexCurrentCheckScp])") }
         }#End
        
         if (min(se_roc$True_positives_length)<0) { stop("(min(se_roc$True_positives_length)<0)") }
         if (min(se_roc$False_positives_length)<0) { stop("(min(se_roc$False_positives_length)<0)") }
         se_roc$Scp_completeName = se_roc$ReadWrite_disease_neighborhoods
         se_roc$ReadWrite_disease_neighborhoods = Shorten_scp_names(se_roc$ReadWrite_disease_neighborhoods)
         se_roc$Precision_harmful = se_roc$True_positives_length / (se_roc$True_positives_length + se_roc$False_positives_length)
         se_roc$Recall_harmful = se_roc$True_positive_rate
         se_roc$F1score_harmful = (1+beta_f1score^2)*se_roc$Precision_harmful*se_roc$Recall_harmful/( (beta_f1score^2*se_roc$Precision_harmful) + se_roc$Recall_harmful)
         indexZero = which((se_roc$Precision_harmful+se_roc$Recall_harmful)==0)
         se_roc$F1score_harmful[indexZero]=0
         se_roc$Precision_protective = se_roc$False_positives_length / (se_roc$True_positives_length+se_roc$False_positives_length)
         se_roc$Recall_protective = se_roc$False_positive_rate
         se_roc$F1score_protective = (1+beta_f1score^2)*se_roc$Precision_protective*se_roc$Recall_protective/( (beta_f1score^2*se_roc$Precision_protective) + se_roc$Recall_protective)
         indexZero = which((se_roc$Precision_protective+se_roc$Recall_protective)==0)
         se_roc$F1score_protective[indexZero] = 0
         total_samples = (se_roc$True_positives_length/se_roc$True_positive_rate + se_roc$False_positives_length/se_roc$False_positive_rate)
         indexNotNaN = which(!is.nan(total_samples))
         if (length(unique(floor(total_samples[indexNotNaN]*10000+0.5)/10000))!=1) { stop("Unequal sample counts in se_roc")  }
         if (length(unique(se_roc$Total_samples_count))!=1) { stop("Unequal sample counts in se_roc") }
         if (unique(floor(total_samples[indexNotNaN]*10000+0.5)/10000)!=unique(se_roc$Total_samples_count)) { stop("Calculation error - check C# script")  }
         if (unique(se_roc$Precision_harmful+se_roc$Precision_protective)!=1) { stop("(unique(se_roc$Precision_harmful+se_roc$Precision_protective)!=1)") }
         if (length(which(se_roc$Recall_harmful!=se_roc$Recall))!=0) { stop("(length(which(se_roc$Recall_harmful!=se_roc$Recall))!=0)") }
         round_factor = 1E5
         if (length(which(round(round_factor*se_roc$Precision_harmful)!=round(round_factor*se_roc$Precision)))!=0) { stop("length(which(se_roc$Precision_harmful!=se_roc$Precision)!=0)") }
         if (length(final_roc)>0) { final_roc = rbind(final_roc,se_roc) }
         else { final_roc = se_roc }
      }#End - Prepare side effect roc and check for correctness
      roc = final_roc
      rm(final_roc)
   }#End - Read and prepare ROC

   if (generate_comparison_with_clusterMarkerGenes)
   {#Begin - Read and prepare single cell enrichment of RM1 cardiomyocytes and adult heart single cell RNAseq
     {#Begin - Read and prepare single cell enrichment of iPSCd cardiomyocytes
         fileName = paste(ontology,"_ScRNAseq_iPSCdCM_schaniel_filtered.txt",sep='')
         complete_fileName = paste(scSnRnaSeq_enrichment_directory,fileName,sep='')
         iPSCdCM_sc_enrich = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
         iPSCdCM_sc_enrich$Sample_name = as.character(iPSCdCM_sc_enrich$Sample_name)
         
         if (length(unique(iPSCdCM_sc_enrich$Sample_entryType))!=1) { stop("(length(unique(iPSCdCM_sc_enrich$Sample_entryType))!=1)") }
         
         sampleNames = unique(iPSCdCM_sc_enrich$Sample_name)
         indexS=1
         for (indexS in 1:length(sampleNames))
         {#Begin
            sampleName = sampleNames[indexS]
            indexCurrentSN = which(iPSCdCM_sc_enrich$Sample_name==sampleName)
            if (sampleName %in% names(iPSCdCM_sc_cluster_cellType_list))
            { iPSCdCM_sc_enrich$Sample_name[indexCurrentSN] = iPSCdCM_sc_cluster_cellType_list[[sampleName]] }
         }#End
         indexKeep = which(iPSCdCM_sc_enrich$Sample_name %in% iPSCdCM_sc_cluster_cellType_list)
         iPSCdCM_sc_enrich = iPSCdCM_sc_enrich[indexKeep,]
     }#End - Read and prepare single cell enrichment of iPSCd cardiomyocytes

     {#Begin - Read and prepare single cell enrichment of adult heart cells
       fileName = paste(ontology,"_ScRNAseq_litvinukova_adultHeart_filtered.txt",sep='')
       complete_fileName = paste(scSnRnaSeq_enrichment_directory,fileName,sep='')
       
       aH_sc_enrich = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')

       if (length(unique(aH_sc_enrich$Sample_entryType))!=1) { stop("(length(unique(aH_sc_enrich$Sample_entryType))!=1)") }

       sampleNames = unique(aH_sc_enrich$Sample_name)
       indexS=1
       for (indexS in 1:length(sampleNames))
       {#Begin
         sampleName = sampleNames[indexS]
         indexCurrentSN = which(aH_sc_enrich$Sample_name==sampleName)
         if (sampleName %in% names(aH_cluster_cellType_list))
         { aH_sc_enrich$Sample_name[indexCurrentSN] = aH_cluster_cellType_list[[sampleName]] }
       }#End
       indexKeep = which(aH_sc_enrich$Sample_name %in% aH_cluster_cellType_list)
       aH_sc_enrich = aH_sc_enrich[indexKeep,]
     }#End - Read and prepare single cell enrichment of adult heart cells

     if (generate_small_plots) { scRNAseq_enrich = aH_sc_enrich }
     if (!generate_small_plots) { scRNAseq_enrich = rbind(iPSCdCM_sc_enrich,aH_sc_enrich) }
     
     if (length(unique(scRNAseq_enrich$Sample_entryType))>1) { stop("More than one entry type in clusterMarkerPathways")}
     scRNAseq_enrich$Sample_entryType = scRNAseq_enrich$Sample_name
     
     {#Begin - Process combined enrichment
         indexKeep = which(scRNAseq_enrich$Pvalue<=0.05)
         scRNAseq_enrich = scRNAseq_enrich[indexKeep,]

         comparison_enrichments[["ScRNASeq_DEGs"]] = scRNAseq_enrich
         if (generate_small_plots) { comparison_sampleNames_in_rightOrders[["ScRNASeq_DEGs"]] = c(unlist(aH_cluster_cellType_list),foundBycardiotoxicDrugs_label,foundByNoncardiotoxicDrugs_label) }
         if (!generate_small_plots) { comparison_sampleNames_in_rightOrders[["ScRNASeq_DEGs"]] = c(unlist(iPSCdCM_sc_cluster_cellType_list),unlist(aH_cluster_cellType_list),foundBycardiotoxicDrugs_label,foundByNoncardiotoxicDrugs_label) }
     }#End - Process combined enrichment
           
   }#End - Read and prepare single cell enrichment of RM1 cardiomyocytes and adult heart single cell RNAseq

   side_effects = unique(roc$Side_effect)
   indexSide=2
   for (indexSide in 1:length(side_effects))
   {#Begin - Generate precision summaries, fill visualize scps and missing_scps_in_scps_in_right_order
      side_effect = side_effects[indexSide]
      harmful_label = rocComparison_harmfulLabel_list[[side_effect]]
      protective_label = rocComparison_protectiveLabel_list[[side_effect]]
      sideEffect_precision_cutoff_ranks = c()
      indexCurrentSideEffect = which(roc$Side_effect==side_effect)
      sideEffect_roc = roc[indexCurrentSideEffect,]
     
      {#Begin - Generate harmProt_roc with auc and f1 score parameter within min and max rank cutoffs
         indexMaxRankCutoff = which(sideEffect_roc$Roc_cutoff<=ontology_maxRankCutoff_list[[ontology]])
         maxRankCutoff_roc = sideEffect_roc[indexMaxRankCutoff,]
         harmProt_roc = Duplicate_roc_to_generate_harmful_and_protective_roc(roc=maxRankCutoff_roc,harmful_label,protective_label)  
         harmProt_roc = Add_maxF1ScoreAbsDifference_and_F1ScoreAUCs( harmProt_roc = harmProt_roc
                                                                    ,last_considered_rocRankCutoff = ontology_maxRankCutoff_list[[ontology]]
                                                                    ,weight_of_penalty_AUC = weight_of_penalty_AUC)
        
         if (test_reversed_f1score)
         {#Begin
            harmProt_roc_regular = harmProt_roc
            harmProt_roc_regular = Add_maxF1ScoreAbsDifference_and_F1ScoreAUCs( harmProt_roc = harmProt_roc_regular
                                                                               ,last_considered_rocRankCutoff = ontology_maxRankCutoff_list[[ontology]]
                                                                               ,weight_of_penalty_AUC = 0)
            harmProt_roc_reversed = harmProt_roc
            harmProt_roc_reversed$F1score = 1-harmProt_roc_reversed$F1score
            harmProt_roc_reversed = Add_maxF1ScoreAbsDifference_and_F1ScoreAUCs_reverse( harmProt_roc = harmProt_roc_reversed
                                                                                        ,last_considered_rocRankCutoff = ontology_maxRankCutoff_list[[ontology]]
                                                                                        ,weight_of_penalty_AUC = 0)
          
            round_factor=1000
            AUC_sums = harmProt_roc_regular$F1score_thisAUC_and_diffAUC+harmProt_roc_reversed$F1score_thisAUC_and_diffAUC
            indexNot100 = which(round(round_factor*AUC_sums)/round_factor!=round(round_factor*100)/round_factor)
            if (length(indexNot100)>0) { stop("test_reversed_f1score failed") }
            else { rm(harmProt_roc_regular); rm(harmProt_roc_reversed) }
        }#End
        
         indexMinRank = which(harmProt_roc$Roc_cutoff>=ontology_minRankCutoff_list[[ontology]])
         harmProt_roc = harmProt_roc[indexMinRank,]
         rm(sideEffect_roc)
     }#End - Generate harmProt_roc with auc and f1 score parameter within min and max rank cutoffs
     
      {#Begin - Generate evaluate rocs with only highest risk assessment value for each sideEffect-scp-entryType-association
         harmProt_roc$UniqueIdentifier = paste(harmProt_roc$Side_effect,harmProt_roc$ReadWrite_disease_neighborhoods,harmProt_roc$Association,harmProt_roc$Entry_type,sep=" ")
         harmProt_roc$Value_for_selection = 0
         harmProt_roc$Value_type_for_selection = "error"
         indexRiskAssesment=1
         for (indexRiskAssesment in 1:length(risk_assessment_maximize_valueTypes))
         {#Begin
            current_riskAssesment_valueType = risk_assessment_maximize_valueTypes[indexRiskAssesment]
            indexHarmProt=1
            for (indexHarmProt in 1:length(harmProt_roc[,1]))
            {#Begin 
               if (harmProt_roc$Value_for_selection[indexHarmProt]<0) { rm(harmProt_roc) }
               if (harmProt_roc[[current_riskAssesment_valueType]][indexHarmProt] > harmProt_roc$Value_for_selection[indexHarmProt])
               {#Begin
                  harmProt_roc$Value_for_selection[indexHarmProt] = harmProt_roc[[current_riskAssesment_valueType]][indexHarmProt]
                  harmProt_roc$Value_type_for_selection = current_riskAssesment_valueType
               }#End
            }#End
         }#End
         harmProt_roc = harmProt_roc[order(harmProt_roc$Value_for_selection,decreasing=TRUE),]
         indexKeep = which(!duplicated(harmProt_roc$UniqueIdentifier))
         harmProt_roc$UniqueIdentifier=NULL
         evaluate_rocs = harmProt_roc[indexKeep,]
      }#End - Generate evaluate rocs with only highest risk assessment value for each sideEffect-scp-entryType-association
     
      {#Begin - Generate selected rocs
         evaluate_rocs$Selected = FALSE
         evaluate_rocs$Selection_rank = 999999
         evaluate_rocs$UID_to_complement = paste(evaluate_rocs$Side_effect,evaluate_rocs$ReadWrite_disease_neighborhoods,sep=" ")
         sideEffects = unique(evaluate_rocs$Side_effect)
         indexSEffect=1
         for (indexSEffect in 1:length(sideEffects))
         {#Begin
            sideEffect = sideEffects[indexSEffect]
            indexCurrentSideEffect = which(evaluate_rocs$Side_effect==sideEffect)
            sE_evaluate_rocs = evaluate_rocs[indexCurrentSideEffect,]
            associations = unique(sE_evaluate_rocs$Association)
            indexAssociation=1
            for (indexAssociation in 1:length(associations))
            {#Begin
               association = associations[indexAssociation]
               indexCurrentAssociation = which(sE_evaluate_rocs$Association==association)
               association_evaluate_rocs = sE_evaluate_rocs[indexCurrentAssociation,]
               association_evaluate_rocs$Selection_rank = rank(-(association_evaluate_rocs$Value_for_selection))
               indexSelected = which(association_evaluate_rocs$Selection_rank <= ontology_topScps_harmfulOrProtective_forPrecsion_list[[ontology]])
               association_evaluate_rocs$Selected[indexSelected] = TRUE
               sE_evaluate_rocs[indexCurrentAssociation,] = association_evaluate_rocs
            }#End
            evaluate_rocs[indexCurrentSideEffect,] = sE_evaluate_rocs
        }#End
         indexSelected = which(evaluate_rocs$Selected==TRUE)
         selectedUniqueIDs = unique(evaluate_rocs$UID_to_complement[indexSelected])
         indexKeep = which(evaluate_rocs$UID_to_complement %in% selectedUniqueIDs)
         evaluate_rocs$UID_to_complement=NULL
         selected_rocs = evaluate_rocs[indexKeep,]
      }#End - Generate selected rocs
     
      if (length(selected_rocs)>0)
      {#Begin - if roc list exists - extend precision summaries ans scps selected for visualization
        indexSelected=1
        visualize_scps=c()
        for (indexSelected in 1:length(selected_rocs[,1]))
        {#Begin - indexSelected
           current_selected_association_roc = selected_rocs[indexSelected,]
           visualize_scps = c(visualize_scps,current_selected_association_roc$ReadWrite_disease_neighborhoods)
           precision_summary_line = precision_summary_base_line
           precision_summary_line$Side_effect = current_selected_association_roc$Side_effect
           precision_summary_line$Ontology = ontologyAbbreviation_ontologyEnum_list[[ontology]]
           precision_summary_line$Selected = current_selected_association_roc$Selected
           precision_summary_line$Scp = current_selected_association_roc$ReadWrite_disease_neighborhoods
           precision_summary_line$Scp_completeName = current_selected_association_roc$Scp_completeName
           precision_summary_line$Entry_type = current_selected_association_roc$Entry_type
           precision_summary_line$Precision = current_selected_association_roc$Precision
           precision_summary_line$Recall = current_selected_association_roc$Recall
           precision_summary_line$Association = current_selected_association_roc$Association
           precision_summary_line$Cutoff_rank = current_selected_association_roc$Roc_cutoff
           precision_summary_line$Max_cutoff_rank_for_AUC = current_selected_association_roc$Max_cutoff_rank_for_AUC
           precision_summary_line$F1_score = current_selected_association_roc$F1_score
           precision_summary_line$ValueForSelection = current_selected_association_roc$Value_for_selection 
           precision_summary_line$ValueTypeForSelection = current_selected_association_roc$Value_type_for_selection
           precision_summary_line$Selection_rank = current_selected_association_roc$Selection_rank
           if (length(precision_summaries)>0) { precision_summaries = rbind(precision_summaries,precision_summary_line) }
           else { precision_summaries = precision_summary_line }
        }#End - indexSelected
        sideEffect_scps_forVisuPrecision_list[[side_effect]] = unique(visualize_scps[order(visualize_scps)])
     }#End - if roc list exists - extend precision summaries ans scps selected for visualization
      
      {#Begin - Check precision summaries for duplicated values
        precision_summaries$Unique_identifier = paste(precision_summaries$Side_effect,"-",precision_summaries$Scp_completeName,"-",precision_summaries$Entry_type,"-",precision_summaries$Association,sep='')
        precision_summaries = precision_summaries[order(precision_summaries$Unique_identifier),]
        indexDuplicated = which(duplicated(precision_summaries$Unique_identifier))
        if (length(indexDuplicated)>0) { stop("Precision summary contains duplicated values") }
        precision_summaries$Unique_identifier = NULL
      }#End - Check precision summaries for duplicated values
      
      precision_summaries$Scp = Shorten_scp_names(precision_summaries$Scp_completeName)
      indexSelected = which(precision_summaries$Selected==TRUE)
      indexIndexMissing = which(!precision_summaries$Scp[indexSelected] %in% scps_in_correct_spreadsheet_order)
      missing_scps_in_scps_in_right_order = c(missing_scps_in_scps_in_right_order,unique(precision_summaries$Scp[indexSelected[indexIndexMissing]]))
   }#End - Generate precision summaries, fill visualize scps and missing_scps_in_scps_in_right_order
   
   {#Begin - Read and prepare enrichment file
     fileName = paste(ontology,add_toEnrichmentFile,sep='')
     complete_fileName = paste(directory,fileName,sep='')
     enrichment = read.table(complete_fileName,header=TRUE,stringsAsFactor=FALSE,sep='\t')
     indexKeep = which(enrichment$Pvalue<=scp_pvalue)
     indexRemove = which(enrichment$Pvalue>scp_pvalue)
     enrichment = enrichment[indexKeep,]
     indexKeep = which(enrichment$Sample_entryType %in% entryTypes_of_interest)
     enrichment = enrichment[indexKeep,]
     
     enrichment = Add_drugs_celllines_f1ScoreWeights_drugTypes_to_enrichment_dataframe(enrichment)
     
     indexKeep = which(enrichment$Outlier %in% outlier_statuses)
     enrichment = enrichment[indexKeep,]
     indexKeep = which(enrichment$Drug_type %in% drugTypes_of_interest)
     enrichment = enrichment[indexKeep,]
     enrichment$Scp_completeName = enrichment$Scp
     enrichment$Scp = Shorten_scp_names(enrichment$Scp)
   }#End - Read and prepare enrichment file
   
   {#Begin - Read and prepare library
      complete_ontology_library_fileName = paste(ontology_library_directory,ontology_ontology_library_fileName[[ontology]],sep='')
      ontology_library = read.csv(file=complete_ontology_library_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
   }#End - Read and prepare library
   
   sideEffect_scps_forVisuRoc_list = sideEffect_scps_forVisuPrecision_list
   
   indexSG=2
   for (indexSG in 1:length(side_effect_groups_list))
   {#Begin - plots for same side effect group
      side_effect_group = names(side_effect_groups_list)[[indexSG]]
      side_effects = side_effect_groups_list[[side_effect_group]]
      harmful_label = rocComparison_harmfulLabel_list[[side_effect]]
      protective_label = rocComparison_protectiveLabel_list[[side_effect]]

      PrecisionRecallF1_vs_rankCutoff_plots = list()

      if (generate_precisionRecallF1score_vs_cutoffRank_plots)
      {#Begin - generate_precisionRecallF1score_vs_cutoffRank_plots
         max_roc_cutoff_rank = ontology_maxRank_list[[ontology]]
         indexV=1
         visualize_scps = unique(unlist(sideEffect_scps_forVisuPrecision_list))
         visualize_scps = visualize_scps[order(visualize_scps)]
         if (length(visualize_scps)>0)
         {#Begin - (length(visualize_scps)>0)
            indexVisualizeScps = 1
            for (indexVisualizeScps in 1:length(visualize_scps))
            {#Begin - same SCP precision plots
               visualize_scp = visualize_scps[indexVisualizeScps]
               indexCurrentScp = which(roc$ReadWrite_disease_neighborhoods==visualize_scp)
               if (length(indexCurrentScp)>0)
               {#Begin - if (length(indexCurrentScp)>0)
                  scp_roc = roc[indexCurrentScp,]
                  indexSideEffect=1
                  for (indexSideEffect in 1:length(side_effects))
                  {#Begin - same side effect precision plots
                     side_effect = side_effects[indexSideEffect]
                     harmful_label = rocComparison_harmfulLabel_list[[side_effect]]
                     protective_label = rocComparison_protectiveLabel_list[[side_effect]]
                     
                     indexCurrentSideEffect=c()
                     if (visualize_scp %in% sideEffect_scps_forVisuPrecision_list[[side_effect]])
                     { indexCurrentSideEffect = which(scp_roc$Side_effect==side_effect) }
                     if (length(indexCurrentSideEffect)>0)
                     {#Begin - if (length(indexCurrentSideEffect)>0)
                        sideEffect_roc = scp_roc[indexCurrentSideEffect,]
                        
                        sideEffect_roc$Precision = sideEffect_roc$Precision_harmful
                        sideEffect_roc$Percentages_of_samples = floor(100*sideEffect_roc$True_positives_length/(sideEffect_roc$True_positives_length+sideEffect_roc$False_negatives_length)+0.5)
                        indexTrueNegatives = which(sideEffect_roc$Precision<0.5)
                        sideEffect_roc$Percentages_of_samples[indexTrueNegatives] = floor(100*abs(sideEffect_roc$False_positives_length[indexTrueNegatives])/(abs(sideEffect_roc$False_positives_length[indexTrueNegatives])+abs(sideEffect_roc$True_negatives_length[indexTrueNegatives]))+0.5)
                        sideEffect_roc$Percentages_of_samples = paste(sideEffect_roc$Percentages_of_samples,"%",sep='')
                        
                        color_list = color_list[order(names(color_list))]
                        indexCurrentColors = which(names(color_list)%in%sideEffect_roc$Entry_type)
                        current_colors = unlist(color_list[indexCurrentColors])
                        
                        harmProt_sideEffect_roc = Duplicate_roc_to_generate_harmful_and_protective_roc(sideEffect_roc,harmful_label,protective_label)
                        entryType_associations = unique(harmProt_sideEffect_roc$EntryType_association)
                        entryType_associations = entryType_associations[order(entryType_associations)]
                        current_colors = c()
                        for (indexETA in 1:length(entryType_associations))
                        {#Begin
                           entryType_association = entryType_associations[indexETA]
                           if (!entryType_association %in% names(precisionRecall_color_list))
                           { stop(paste(entryType_association," is missing in names(precisionRecall_color_list)",sep='')) }
                           current_colors = c(current_colors,precisionRecall_color_list[[entryType_associations[indexETA]]])
                        }#End
                        maxRankCutoff_in_figure = ontology_maxRank_list[[ontology]]
                        
                        {#Begin - Add 0 values and maxRankcuttoff + 1 as start points
                           scps = unique(harmProt_sideEffect_roc$ReadWrite_disease_neighborhoods)
                           indexScp=1
                           zero_rocs = c()
                           for (indexScp in 1:length(scps))
                           {#Begin
                              scp = scps[indexScp]
                              indexCurrentScp = which(harmProt_sideEffect_roc$ReadWrite_disease_neighborhoods==scp)
                              scp_harmProt_sideEffect_roc = harmProt_sideEffect_roc[indexCurrentScp,]
                              entryType_associations = unique(scp_harmProt_sideEffect_roc$EntryType_association)
                              indexETA=1
                              for (indexETA in 1:length(entryType_associations))
                              {#Begin
                                 entryType_association = entryType_associations[indexETA]
                                 indexCurrentEntryTypeAssociation = which(scp_harmProt_sideEffect_roc$EntryType_association==entryType_association)
                                 ETA_roc = scp_harmProt_sideEffect_roc[indexCurrentEntryTypeAssociation,]
                                 ETA_roc$Distance_to_edge = maxRankCutoff_in_figure-ETA_roc$Roc_cutoff
                                 indexLeft = which(ETA_roc$Distance_to_edge>=0)
                                 if (length(indexLeft)>0)
                                 { indexLefClosestToEdge = indexLeft[which(ETA_roc$Distance_to_edge[indexLeft]==min(ETA_roc$Distance_to_edge[indexLeft]))] }
                                 else { indexLefClosestToEdge = c() }
                                 ETA_roc$Distance_to_edge=NULL
                                 indexCurrentMaxRankCutoff = which(ETA_roc$Roc_cutoff==max(ETA_roc$Roc_cutoff))
                                 zero_ETA_scp_roc = ETA_roc[1,]
                                 zero_ETA_scp_roc$Roc_cutoff=0
                                 zero_ETA_scp_roc$Recall=0
                                 zero_ETA_scp_roc$Precision=0
                                 zero_ETA_scp_roc$F1score=0
                                 if (length(zero_rocs)>0) { zero_rocs = rbind(zero_rocs,zero_ETA_scp_roc) }
                                 else { zero_rocs = zero_ETA_scp_roc }
                                 if (length(indexLefClosestToEdge)==0)
                                 {#Begin
                                    maxRocRank_ETA_scp_roc = ETA_roc[1,]
                                    maxRocRank_ETA_scp_roc$Roc_cutoff=maxRankCutoff_in_figure
                                    maxRocRank_ETA_scp_roc$Recall=0
                                    maxRocRank_ETA_scp_roc$Precision=0
                                    maxRocRank_ETA_scp_roc$F1score=0
                                    if (length(zero_rocs)>0) { zero_rocs = rbind(zero_rocs,maxRocRank_ETA_scp_roc) }
                                    else { zero_rocs = maxRocRank_ETA_scp_roc }
                                 }#End
                                 else
                                 {#Begin
                                    maxRocRank_ETA_scp_roc = ETA_roc[indexLefClosestToEdge,]
                                    maxRocRank_ETA_scp_roc$Roc_cutoff=maxRankCutoff_in_figure
                                    maxRocRank_ETA_scp_roc$Recall=ETA_roc$Recall[indexLefClosestToEdge]
                                    maxRocRank_ETA_scp_roc$Precision=ETA_roc$Precision[indexLefClosestToEdge]
                                    maxRocRank_ETA_scp_roc$F1score=ETA_roc$F1score[indexLefClosestToEdge]
                                    if (length(zero_rocs)>0) { zero_rocs = rbind(zero_rocs,maxRocRank_ETA_scp_roc) }
                                    else { zero_rocs = maxRocRank_ETA_scp_roc }
                                 }#End
                              }#End
                           }#End
                           harmProt_sideEffect_roc = rbind(harmProt_sideEffect_roc,zero_rocs)
                        }#End - Add 0 values and maxRankcuttoff + 1 

                        visualize_scp_title = Shorten_scp_names(visualize_scp)

                        if (generate_small_plots)
                        {#Begin
                          points_size=0.2
                          line_width = 0.3
                          title_size = 5
                          axis_text_size = 5
                          axis_title_size = 5
                          visualize_scp_title = Split_name_over_multiple_lines(name=visualize_scp_title,max_nchar_per_line=35,max_lines=2)
                        }#End
                        else
                        {#Begin
                          points_size=0.5
                          line_width = 1
                          title_size = 10
                          axis_text_size = 8
                          axis_title_size = 8
                          visualize_scp_title = Split_name_over_multiple_lines( name=visualize_scp_title,max_nchar_per_line=30,max_lines=5)
                        }#End
                        
                        shared_title = paste(ontology," - ",side_effect,"\n",visualize_scp_title,sep='')
                        
                        Precision_plot = ggplot(harmProt_sideEffect_roc,aes(x=Roc_cutoff,y=Precision,fill=EntryType_association,linetype=Association,color=EntryType_association))
                        Precision_plot = Precision_plot  + geom_point(size=points_size) + geom_step(size=line_width)
                        Precision_plot = Precision_plot + scale_linetype_manual(values=c("solid","dashed"))
                        Precision_plot = Precision_plot + ylim(0,1)
                        Precision_plot = Precision_plot + xlim(0,maxRankCutoff_in_figure)
                        #Precision_plot = Precision_plot + geom_text(label=harmProt_sideEffect_roc$Percentages_of_samples,size=2)
                        Precision_plot = Precision_plot + xlab("Significance rank cutoff");
                        Precision_plot = Precision_plot + scale_color_manual(values=current_colors)
                        #Precision_plot = Precision_plot + ggtitle(paste(side_effect,"\n",ontology,"\n",visualize_scp_title,sep=''))
                        Precision_plot = Precision_plot + ggtitle(shared_title)
                        Precision_plot = Precision_plot + geom_hline(yintercept = 0.5)
                        maximum_precision_dashed_line = 1 - minimum_precision_dashed_line
                        Precision_plot = Precision_plot + geom_hline(yintercept = minimum_precision_dashed_line, linetype="dashed")
                        #Precision_plot = Precision_plot + geom_hline(yintercept = maximum_precision_dashed_line, linetype="dashed")
                        Precision_plot = Precision_plot + theme(plot.title =  element_text(size=title_size,vjust=0.5,hjust=0.5))
                        Precision_plot = Precision_plot + theme(axis.text.x = element_text(size=axis_text_size))
                        Precision_plot = Precision_plot + theme(axis.text.y = element_text(size=axis_text_size))
                        Precision_plot = Precision_plot + theme(axis.title.y = element_text(size=axis_title_size))
                        Precision_plot = Precision_plot + theme(axis.title.x = element_text(size=axis_title_size))
                        Precision_plot = Precision_plot + theme(legend.position = "none")
                        PrecisionRecallF1_vs_rankCutoff_plots[[length(PrecisionRecallF1_vs_rankCutoff_plots)+1]] = Precision_plot
                        
                        harmProt_sideEffect_roc$Figure_text = paste(floor(harmProt_sideEffect_roc$Precision*100+0.5),"%",sep='')
                        Recall_plot = ggplot(harmProt_sideEffect_roc,aes(x=Roc_cutoff,y=Recall,fill=EntryType_association,linetype=Association,color=EntryType_association))
                        Recall_plot = Recall_plot + geom_step(size=line_width) + geom_point(size=points_size)
                        Recall_plot = Recall_plot + scale_linetype_manual(values=c("solid","dashed"))
                        #Recall_plot = Recall_plot+ geom_text(label=harmProt_sideEffect_roc$Figure_text,size=2)
                        Recall_plot = Recall_plot + scale_color_manual(values=current_colors)
                        #Recall_plot = Recall_plot + ggtitle(paste(side_effect,"\n",ontology,"\n",visualize_scp_title,sep=''))
                        Recall_plot = Recall_plot + ggtitle(shared_title)
                        Recall_plot = Recall_plot + ylim(0,1)
                        Recall_plot = Recall_plot + xlim(0,ontology_maxRank_list[[ontology]])
                        Recall_plot = Recall_plot + xlab("Significance rank cutoff");
                        # Recall_plot = Recall_plot + geom_hline(yintercept = 0.5)
                        Recall_plot = Recall_plot + geom_hline(yintercept = minimum_precision_dashed_line, linetype="dashed")
                        Recall_plot = Recall_plot + theme(plot.title =  element_text(size=title_size,vjust=0.5,hjust=0.5))
                        Recall_plot = Recall_plot + theme(axis.text.x = element_text(size=axis_text_size))
                        Recall_plot = Recall_plot + theme(axis.text.y = element_text(size=axis_text_size))
                        Recall_plot = Recall_plot + theme(axis.title.y = element_text(size=axis_title_size))
                        Recall_plot = Recall_plot + theme(axis.title.x = element_text(size=axis_title_size))
                        Recall_plot = Recall_plot + theme(legend.position = "none")
                        PrecisionRecallF1_vs_rankCutoff_plots[[length(PrecisionRecallF1_vs_rankCutoff_plots)+1]] = Recall_plot
                        
                        harmProt_sideEffect_roc$Figure_text = paste(floor(harmProt_sideEffect_roc$Precision*100+0.5),"%",sep='')
                        F1score_plot = ggplot(harmProt_sideEffect_roc,aes(x=Roc_cutoff,y=F1score,fill=EntryType_association,linetype=Association,color=EntryType_association))
                        F1score_plot = F1score_plot + geom_step(size=line_width) + geom_point(size=points_size)
                        Recall_plot = Recall_plot + scale_linetype_manual(values=c("solid","dashed"))
                        #F1score_plot = F1score_plot+ geom_text(label=harmProt_sideEffect_roc$Figure_text,size=2)
                        F1score_plot = F1score_plot + xlab("Significance rank cutoff");
                        F1score_plot = F1score_plot + ylab("F1 score");
                        F1score_plot = F1score_plot + scale_color_manual(values=current_colors)
                        #F1score_plot = F1score_plot + ggtitle(paste(side_effect,"\n",ontology,"\n",visualize_scp_title,sep=''))
                        F1score_plot = F1score_plot + ggtitle(shared_title)
                        F1score_plot = F1score_plot + ylim(0,1)
                        F1score_plot = F1score_plot + xlim(0,ontology_maxRank_list[[ontology]])
                        # F1score_plot = F1score_plot + geom_hline(yintercept = 0.5)
                        F1score_plot = F1score_plot + geom_hline(yintercept = minimum_precision_dashed_line, linetype="dashed")
                        F1score_plot = F1score_plot + theme(plot.title =  element_text(size=title_size,vjust=0.5,hjust=0.5))
                        F1score_plot = F1score_plot + theme(axis.text.x = element_text(size=axis_text_size))
                        F1score_plot = F1score_plot + theme(axis.text.y = element_text(size=axis_text_size))
                        F1score_plot = F1score_plot + theme(axis.title.y = element_text(size=axis_title_size))
                        F1score_plot = F1score_plot + theme(axis.title.x = element_text(size=axis_title_size))
                        F1score_plot = F1score_plot + theme(legend.position = "none")
                        PrecisionRecallF1_vs_rankCutoff_plots[[length(PrecisionRecallF1_vs_rankCutoff_plots)+1]] = F1score_plot
                     }#End - if (length(indexCurrentSideEffect)>0)
                  }#End - same side effect precision plots
               }#End - if (length(indexCurrentScp)>0)
            }#End - same SCP precision plots
         }#End - (length(visualize_scps)>0)
      }#End - Generate generate_precisionRecallF1score_vs_cutoffRank_plots
      
      SideEffect_precisionRecallF1_vs_rankCutoff_plots[[side_effect_group]] = c(SideEffect_precisionRecallF1_vs_rankCutoff_plots[[side_effect_group]],PrecisionRecallF1_vs_rankCutoff_plots)
   }#End - plots for same side effect group
   
   indexSG=1
   for (indexSG in 1:length(side_effect_groups_list))
   {#Begin - Generate spreadsheet summary figure
      side_effect_group = names(side_effect_groups_list)[[indexSG]]
      side_effects = side_effect_groups_list[[side_effect_group]]
      harmful_label = rocComparison_harmfulLabel_list[[side_effect_group]]
      protective_label = rocComparison_protectiveLabel_list[[side_effect_group]]
      
      Spreadsheet_plots = list()
      Collapsed_spreadsheet_plots = list()
      Toxic_collapsed_spreadsheet_plots = list()

      {#Begin - Generate spreadsheet summary figure for precision plots
         indexCurrentSideEffects = which(precision_summaries$Side_effect%in%side_effects)
         indexCurrentOntology = which(precision_summaries$Ontology==ontologyAbbreviation_ontologyEnum_list[[ontology]])
         indexCurrentFigure = indexCurrentSideEffects[indexCurrentSideEffects %in% indexCurrentOntology]
         if (length(indexCurrentFigure)>0)
         {#Begin - Generate spreadsheet summary figure for precision plots
            sE_precision_cutoff_ranks = precision_summaries[indexCurrentFigure,]
            sE_precision_cutoff_ranks$SideEffect_association_entryType = paste(sE_precision_cutoff_ranks$Entry_type," at higher ranks by\n",sE_precision_cutoff_ranks$Association,sep='')
            sE_precision_cutoff_ranks$SideEffect_association_entryType = gsub("Diffrna_up","upregulated",sE_precision_cutoff_ranks$SideEffect_association_entryType)
            sE_precision_cutoff_ranks$SideEffect_association_entryType = gsub("Diffrna_down","downregulated",sE_precision_cutoff_ranks$SideEffect_association_entryType)
            sE_precision_cutoff_ranks$Unique_identifier = paste(sE_precision_cutoff_ranks$Association,"-",sE_precision_cutoff_ranks$Entry_type,"-",sE_precision_cutoff_ranks$Scp,sep='')
            sE_precision_cutoff_ranks = sE_precision_cutoff_ranks[order(sE_precision_cutoff_ranks$Unique_identifier),]

            sE_precision_cutoff_ranks$Field_label = sE_precision_cutoff_ranks$Selection_rank
            
            sE_precision_cutoff_ranks = sE_precision_cutoff_ranks[order(sE_precision_cutoff_ranks$Entry_type,decreasing=TRUE),]
            indexDuplicated = which(duplicated(sE_precision_cutoff_ranks$Unique_identifier))
            if (length(indexDuplicated)>0) { stop("which(duplicated(sE_precision_cutoff_ranks$Unique_identifier))") }
            
            for (indexSe in 1:length(sE_precision_cutoff_ranks[,1]))
            {#Begin
               sE_precision_cutoff_ranks$Scp[indexSe] = Split_name_over_multiple_lines(name=sE_precision_cutoff_ranks$Scp[indexSe],max_lines=2,max_nchar_per_line=200)
            }#End
            
            sE_precision_cutoff_ranks = sE_precision_cutoff_ranks[order(sE_precision_cutoff_ranks$Scp,decreasing=TRUE),]
            sE_precision_cutoff_ranks$Scp_factor = factor(sE_precision_cutoff_ranks$Scp,levels=unique(sE_precision_cutoff_ranks$Scp))
            
            while (length(sE_precision_cutoff_ranks[,1])>0)
            {#Begin -  while (length(sE_precision_cutoff_ranks[,1])>0)
               leftOverScps = unique(sE_precision_cutoff_ranks$Scp)
               leftOverScps = leftOverScps[order(leftOverScps)]
               showScps = leftOverScps[1:min(length(leftOverScps),max_scps_per_spreadsheet_figure)]
               indexCurrentShowScps = which(sE_precision_cutoff_ranks$Scp%in%showScps)
               indexLeftOverScps = which(!sE_precision_cutoff_ranks$Scp%in%showScps)
               current_sE_precision_cutoff_ranks = sE_precision_cutoff_ranks[indexCurrentShowScps,]
               sE_precision_cutoff_ranks = sE_precision_cutoff_ranks[indexLeftOverScps,]
               current_sE_precision_cutoff_ranks = current_sE_precision_cutoff_ranks[order(current_sE_precision_cutoff_ranks$Scp,decreasing=TRUE),]

               indexHarmful = grep(harmful_label,current_sE_precision_cutoff_ranks$Association)
               indexProtective = grep(protective_label,current_sE_precision_cutoff_ranks$Association)
               indexHarmful = indexHarmful[!indexHarmful %in% indexProtective]
               current_sE_precision_cutoff_ranks$Entry_type_association = "error"
               if (length(indexHarmful)>0)
               { current_sE_precision_cutoff_ranks$Entry_type_association[indexHarmful] = paste(current_sE_precision_cutoff_ranks$Entry_type[indexHarmful],"-",harmful_label,sep='') }
               if (length(indexProtective)>0)
               { current_sE_precision_cutoff_ranks$Entry_type_association[indexProtective] = paste(current_sE_precision_cutoff_ranks$Entry_type[indexProtective],"-",protective_label,sep='') }

               return_list = AddEmptyLinesCountIfNecessary_setScpFactor_and_setBlackOrWhiteScpColors(current_sE_precision_cutoff_ranks,scps_in_correct_spreadsheet_order,max_scps_per_spreadsheet_figure)
               current_sE_precision_cutoff_ranks = return_list$precision_summaries
               label_colors = return_list$label_colors

               indexEntryTypeAssociation_noColor = which(!current_sE_precision_cutoff_ranks$Entry_type_association %in% names(spreadsheet_color_list))
               if (length(indexEntryTypeAssociation_noColor)>0) 
               {#Begin
                  entryTypeAssociation_with_no_color = unique(current_sE_precision_cutoff_ranks$Entry_type_association[indexEntryTypeAssociation_noColor])
                  stop("entryTypeAssociation_with_no_color exists")
               }#End
               
               current_sE_precision_cutoff_ranks = current_sE_precision_cutoff_ranks[order(current_sE_precision_cutoff_ranks$Entry_type,decreasing=TRUE),]
               current_sE_precision_cutoff_ranks = current_sE_precision_cutoff_ranks[order(current_sE_precision_cutoff_ranks$Association),]
               sideEffect_association_entryTypes = unique(current_sE_precision_cutoff_ranks$SideEffect_association_entryType)
               current_sE_precision_cutoff_ranks$SideEffect_association_entryType_factor = factor(current_sE_precision_cutoff_ranks$SideEffect_association_entryType,levels=sideEffect_association_entryTypes)
               current_sE_precision_cutoff_ranks$VarX=1
               for (indexVarX in 1:length(sideEffect_association_entryTypes))
               {#Begin
                  sideEffect_association_entryType = sideEffect_association_entryTypes[indexVarX]
                  indexCurrentVarX = which(current_sE_precision_cutoff_ranks$SideEffect_association_entryType==sideEffect_association_entryType)
                  current_sE_precision_cutoff_ranks$VarX[indexCurrentVarX] = indexVarX
               }#End
               current_sE_precision_cutoff_ranks = current_sE_precision_cutoff_ranks[order(current_sE_precision_cutoff_ranks$Scp_factor,decreasing = FALSE),]
               scps = unique(current_sE_precision_cutoff_ranks$Scp)
               current_sE_precision_cutoff_ranks$VarY=1
               for (indexVarY in 1:length(scps))
               {#Begin
                  scp = scps[indexVarY]
                  indexCurrentVarY = which(current_sE_precision_cutoff_ranks$Scp==scp)
                  current_sE_precision_cutoff_ranks$VarY[indexCurrentVarY] = indexVarY
               }#End
               
               indexSelected = which(current_sE_precision_cutoff_ranks$Selected==TRUE)
               selected_current_sE_precision_cutoff_ranks = current_sE_precision_cutoff_ranks[indexSelected,]

               shared_title = paste(side_effect_group,"\n",ontology," SCPs up/down at higher ranks by",sep='')

               Spreadsheet_plot = ggplot(current_sE_precision_cutoff_ranks,aes(x=SideEffect_association_entryType_factor,y=Scp_factor,fill=Entry_type_association))
               Spreadsheet_plot = Spreadsheet_plot + geom_tile() + coord_equal()
               Spreadsheet_plot = Spreadsheet_plot + geom_rect(data=selected_current_sE_precision_cutoff_ranks, size=0.75, fill=NA, colour=spreadsheet_selected_field_frame_color,
                                                               aes(xmin=VarX - 0.5, xmax=VarX + 0.5, ymin=VarY - 0.5, ymax=VarY + 0.5))
               Spreadsheet_plot = Spreadsheet_plot + ggtitle(shared_title)
               Spreadsheet_plot = Spreadsheet_plot + xlab("") + ylab("")
               Spreadsheet_plot = Spreadsheet_plot + scale_x_discrete(position = "top")
               Spreadsheet_plot = Spreadsheet_plot + scale_fill_manual(values=spreadsheet_color_list)
               Spreadsheet_plot = Spreadsheet_plot + geom_text(label=current_sE_precision_cutoff_ranks$Field_label,size=4,color=spreadsheet_field_label_color)
               Spreadsheet_plot = Spreadsheet_plot + theme(axis.ticks.x = element_blank(), axis.ticks.y = element_blank())
               Spreadsheet_plot = Spreadsheet_plot + theme(plot.title = element_text(size=15,vjust=0.5,hjust=0.5))
               Spreadsheet_plot = Spreadsheet_plot + theme(axis.text.x = element_text(size=13,angle=90,vjust=0.5,hjust=0,color="black"))
               Spreadsheet_plot = Spreadsheet_plot + theme(axis.text.y = element_text(size=13,color=label_colors))
               Spreadsheet_plot = Spreadsheet_plot + theme(axis.title.y = element_text(size=10))
               Spreadsheet_plot = Spreadsheet_plot + theme(axis.title.x = element_text(size=10))
               Spreadsheet_plot = Spreadsheet_plot + theme(legend.position = "none")
               Spreadsheet_plots[[length(Spreadsheet_plots)+1]] = Spreadsheet_plot
               
               {#Begin - Collapse selected
                 selected_current_sE_precision_cutoff_ranks = Collapse_scps_that_are_both_up_and_downregulated_for_the_same_association_in_precision_summaries(selected_current_sE_precision_cutoff_ranks)
                 
                 return_list = AddEmptyLinesCountIfNecessary_setScpFactor_and_setBlackOrWhiteScpColors(selected_current_sE_precision_cutoff_ranks,scps_in_correct_spreadsheet_order,max_scps_per_spreadsheet_figure)
                 selected_current_sE_precision_cutoff_ranks = return_list$precision_summaries
                 label_colors = return_list$label_colors

                 selected_current_sE_precision_cutoff_ranks$SideEffect_association = selected_current_sE_precision_cutoff_ranks$Association

                 geom_text_size = 4
                 if (length(grep("\n",selected_current_sE_precision_cutoff_ranks$Field_label))>0)
                 { geom_text_size = 3.3 }

                 Collapsed_spreadsheet_plot = ggplot(selected_current_sE_precision_cutoff_ranks,aes(x=Association,y=Scp_factor,fill=Entry_type_association,label=Field_label))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + geom_tile() + coord_equal()
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + ggtitle(shared_title)
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + xlab("") + ylab("") 
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + scale_x_discrete(position = "top")
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + scale_fill_manual(values=spreadsheet_color_list)
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + geom_text(size=geom_text_size,color=spreadsheet_field_label_color,fontface="bold",lineheight=0.5)
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.ticks.x = element_blank(), axis.ticks.y = element_blank())
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(plot.title =  element_text(size=15,vjust=0.5,hjust=0.5))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.text.x = element_text(size=13,angle=90,vjust=0.5,hjust=0,color="black"))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.text.y = element_text(size=13,color=label_colors))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.title.y = element_text(size=10))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.title.x = element_text(size=10))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(legend.position = "none")
                 Collapsed_spreadsheet_plots[[length(Collapsed_spreadsheet_plots)+1]] = Collapsed_spreadsheet_plot
               }#End - Collapse selected

               indexCardiotoxic = which(selected_current_sE_precision_cutoff_ranks$Association==harmful_label)
               if (length(indexCardiotoxic)>0)
               {#Begin - Collapse selected - only cardiotoxic drugs
                 toxic_current_sE_precision_cutoff_ranks = selected_current_sE_precision_cutoff_ranks[indexCardiotoxic,]
                 indexEmpty_scp = grep("empty",toxic_current_sE_precision_cutoff_ranks$Scp)
                 indexNotEmpty_scp = 1:length(toxic_current_sE_precision_cutoff_ranks[,1])
                 indexNotEmpty_scp = indexNotEmpty_scp[!indexNotEmpty_scp %in% indexEmpty_scp]
                 toxic_current_sE_precision_cutoff_ranks = toxic_current_sE_precision_cutoff_ranks[indexNotEmpty_scp,]
                 
                 toxic_current_sE_precision_cutoff_ranks = Collapse_scps_that_are_both_up_and_downregulated_for_the_same_association_in_precision_summaries(toxic_current_sE_precision_cutoff_ranks)

                 return_list = AddEmptyLinesCountIfNecessary_setScpFactor_and_setBlackOrWhiteScpColors(toxic_current_sE_precision_cutoff_ranks,scps_in_correct_spreadsheet_order,max_scps_per_spreadsheet_figure)
                 toxic_current_sE_precision_cutoff_ranks = return_list$precision_summaries
                 label_colors = return_list$label_colors

                 geom_text_size = 4
                 if (length(grep("\n",toxic_current_sE_precision_cutoff_ranks$Field_label))>0)
                 { geom_text_size = 3.3 }

                 Collapsed_spreadsheet_plot = ggplot(toxic_current_sE_precision_cutoff_ranks,aes(x=Association,y=Scp_factor,fill=Entry_type_association,label=Field_label))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + geom_tile() + coord_equal()
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + ggtitle(shared_title)
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + xlab("") + ylab("") 
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + scale_x_discrete(position = "top")
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + scale_fill_manual(values=spreadsheet_color_list)
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + geom_text(size=4,color=spreadsheet_field_label_color,fontface="bold")
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.ticks.x = element_blank(), axis.ticks.y = element_blank())
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(plot.title =  element_text(size=15,vjust=0.5,hjust=0.5))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.text.x = element_text(size=13,angle=90,vjust=0.5,hjust=0,color="black"))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.text.y = element_text(size=13,color=label_colors))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.title.y = element_text(size=10))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(axis.title.x = element_text(size=10))
                 Collapsed_spreadsheet_plot = Collapsed_spreadsheet_plot + theme(legend.position = "none")
                 
                 print(Collapsed_spreadsheet_plot)
                 Toxic_collapsed_spreadsheet_plots[[length(Toxic_collapsed_spreadsheet_plots)+1]] = Collapsed_spreadsheet_plot
               }#End - Collapse selected - only cardiotoxic drugs
            }#End -  while (length(sE_precision_cutoff_ranks[,1])>0)
         }#End - Generate spreadsheet summary figure for precision plots
      }#End - Generate spreadsheet summary figure for precision plots

      SideEffect_spreadsheet_plots[[side_effect_group]] = c(SideEffect_spreadsheet_plots[[side_effect_group]],Spreadsheet_plots)
      SideEffect_collapsed_spreadsheet_plots[[side_effect_group]] = c(SideEffect_collapsed_spreadsheet_plots[[side_effect_group]],Collapsed_spreadsheet_plots)
      SideEffect_toxic_collapsed_spreadsheet_plots[[side_effect_group]] = c(SideEffect_toxic_collapsed_spreadsheet_plots[[side_effect_group]],Toxic_collapsed_spreadsheet_plots)

      comparison_collapsed_spreadheet_plots_array = SideEffect_comparison_collapsed_spreadheet_plots_array[[side_effect_group]]
      toxic_comparison_collapsed_spreadheet_plots_array = SideEffect_toxic_comparison_collapsed_spreadheet_plots_array[[side_effect_group]]

      {#Begin - Generate comparison with comparison_enrichments
         comparison_references = names(comparison_enrichments)

         if (length(comparison_references)>0)
         {#Begin - if (length(comparison_references)>0)
         indexCR=1
         for (indexCR in 1:length(comparison_references))
         {#Begin - Compare marker SCPs from RM1 with current genes
           comparison_reference = comparison_references[indexCR]
           comparison_enrichment = comparison_enrichments[[comparison_reference]]
           comparison_sampleNames_in_rightOrder = comparison_sampleNames_in_rightOrders[[comparison_reference]]
           comparison_collapsed_spreadheet_plots = comparison_collapsed_spreadheet_plots_array[[comparison_reference]]
           toxic_comparison_collapsed_spreadheet_plots = toxic_comparison_collapsed_spreadheet_plots_array[[comparison_reference]]
           
           sE_precision_cutoff_ranks = precision_summaries[indexCurrentFigure,]
           sE_precision_cutoff_ranks$SideEffect_association_entryType = paste(sE_precision_cutoff_ranks$Side_effect,"\n",sE_precision_cutoff_ranks$Association," - ",sE_precision_cutoff_ranks$Entry_type,sep='')
           sE_precision_cutoff_ranks$Unique_identifier = paste(sE_precision_cutoff_ranks$Association,"-",sE_precision_cutoff_ranks$Entry_type,"-",sE_precision_cutoff_ranks$Scp,sep='')
           sE_precision_cutoff_ranks = sE_precision_cutoff_ranks[order(sE_precision_cutoff_ranks$Unique_identifier),]
           
           relevant_scps = unique(sE_precision_cutoff_ranks$Scp_completeName)
           
           indexRelevant = which(comparison_enrichment$Scp %in% relevant_scps)
           if (length(indexRelevant)>0)
           {#Begin - if (length(indexRelevant)>0)
             level_published_enrichment = comparison_enrichment[indexRelevant,]

             level_published_enrichment$Field_label = floor(level_published_enrichment$Fractional_rank+0.5)
             {#Begin - Collapse duplicated SCPs and combine ranks of collapsed scps in field label
               sample_names = unique(level_published_enrichment$Sample_name)
               indexSN=1
               collapsed_published_enrichment = c()
               for (indexSN in 1:length(sample_names))
               {#Begin
                 sample_name = sample_names[indexSN]
                 indexCurrentSampleName = which(level_published_enrichment$Sample_name==sample_name)
                 sampleName_published_enrichment = level_published_enrichment[indexCurrentSampleName,]
                 scps = unique(sampleName_published_enrichment$Scp)
                 indexSCP=1
                 for (indexSCP in 1:length(scps))
                 {#Begin
                    scp = scps[indexSCP]
                    indexCurrentScp = which(sampleName_published_enrichment$Scp==scp)
                    scp_published = sampleName_published_enrichment[indexCurrentScp,]
                    if (length(scp_published)>1)
                    {#Begin
                       scp_published = scp_published[order(scp_published$Fractional_rank,decreasing=FALSE),]
                       scp_published$Sample_entryType[1] = paste(scp_published$Sample_entryType,collapse=";")
                       scp_published$Field_label[1] = paste(scp_published$Field_label,collapse="\n")
                    }#End
                    if (length(collapsed_published_enrichment)>0) { collapsed_published_enrichment = rbind(collapsed_published_enrichment,scp_published[1,])}
                    else {collapsed_published_enrichment = scp_published }
                 }#End
               }#End
               level_published_enrichment = collapsed_published_enrichment
             }#End - Collapse duplicated SCPs and combine ranks of collapsed scps in field label
  
             indexMissingCluster = which(!comparison_sampleNames_in_rightOrder %in% level_published_enrichment$Sample_name)
             missingClusters = comparison_sampleNames_in_rightOrder[indexMissingCluster]
             if (length(missingClusters)>0)
             {#Begin- Add missing clusters
               indexMissing=1
               for (indexMissing in 1:length(missingClusters))
               {#Begin - Add missing relevant SCPs
                 new_enrichment_line = level_published_enrichment[1,]
                 new_enrichment_line$Sample_name = missingClusters[indexMissing]
                 new_enrichment_line$Scp = empty_scp_name
                 new_enrichment_line$Sample_entryType = "E_m_p_t_y"
                 new_enrichment_line$Minus_log10_pvalue = 0;
                 new_enrichment_line$Pvalue = 1;
                 new_enrichment_line$Overlap_count=0
                 new_enrichment_line$ReadWrite_overlap_symbols = ""
                 new_enrichment_line$Fractional_rank = empty_rank_entry
                 level_published_enrichment = rbind(level_published_enrichment,new_enrichment_line)
               }#End- Add missing relevant SCPS
             }#End- Add missing clusters
             indexEmptyRank = which(level_published_enrichment$Fractional_rank==empty_rank_entry)
             level_published_enrichment$Field_label[indexEmptyRank] = emptyRank_field_label
             
             {#Begin - Add found by toxic or non toxic drugs
                 indexToxic = which(precision_summaries$Association==harmful_label)
                 indexNotToxic = which(precision_summaries$Association==protective_label)
                 indexSelected = which(precision_summaries$Selected==TRUE)
                 indexToxic_selected = indexToxic[indexToxic %in% indexSelected]
                 indexNotToxic_selected = indexNotToxic[indexNotToxic %in% indexSelected]
                 toxic_scps = precision_summaries$Scp_completeName[indexToxic_selected]
                 toxic_scps = toxic_scps[toxic_scps %in% level_published_enrichment$Scp]
                 not_toxic_scps = precision_summaries$Scp_completeName[indexNotToxic_selected]
                 not_toxic_scps = not_toxic_scps[not_toxic_scps %in% level_published_enrichment$Scp]
                 if (length(toxic_scps)>0)
                 {#Begin - Add found by toxic drugs
                    indexToxic=1
                    for (indexToxic in 1:length(toxic_scps))
                    {#Begin - Add found by toxic drugs
                       new_enrichment_line = level_published_enrichment[1,]
                       new_enrichment_line$Sample_name = foundBycardiotoxicDrugs_label
                       new_enrichment_line$Scp = toxic_scps[indexToxic]
                       new_enrichment_line$Sample_entryType = foundBycardiotoxicDrugs_label
                       new_enrichment_line$Minus_log10_pvalue = 0;
                       new_enrichment_line$Pvalue = 1;
                       new_enrichment_line$Overlap_count=0
                       new_enrichment_line$ReadWrite_overlap_symbols = ""
                       new_enrichment_line$Fractional_rank = empty_rank_entry
                       level_published_enrichment = rbind(level_published_enrichment,new_enrichment_line)
                    }#End - Add found by toxic drugs
                 }#End - Add found by toxic drugs
                 if (length(not_toxic_scps)>0)
                 {#Begin - Add found by not toxic drugs
                    for (indexNotToxic in 1:length(not_toxic_scps))
                    {#Begin - Add found by not toxic drugs
                       new_enrichment_line = level_published_enrichment[1,]
                       new_enrichment_line$Sample_name = foundByNoncardiotoxicDrugs_label
                       new_enrichment_line$Scp = not_toxic_scps[indexNotToxic]
                       new_enrichment_line$Sample_entryType = foundByNoncardiotoxicDrugs_label
                       new_enrichment_line$Minus_log10_pvalue = 0;
                       new_enrichment_line$Pvalue = 1;
                       new_enrichment_line$Overlap_count=0
                       new_enrichment_line$ReadWrite_overlap_symbols = ""
                       new_enrichment_line$Fractional_rank = empty_rank_entry
                       level_published_enrichment = rbind(level_published_enrichment,new_enrichment_line)
                     }#End - Add found by not toxic drugs
                 }#End - Add found by not toxic drugs
             }#End - Add found by toxic or non toxic drugs

             level_published_enrichment$Scp_short = Shorten_scp_names(level_published_enrichment$Scp)

             {#Begin - Add scp effects and (combined) ranks as field labels
               level_published_enrichment$Scp_effect = "E_m_p_t_y"
               indexScp=17
               for (indexScp in 1:length(relevant_scps))
               {#Begin
                 relevant_scp = relevant_scps[indexScp]
                 indexCurrentScp = which(precision_summaries$Scp_completeName==relevant_scp)
                 indexCurrentScpEnrich = which(level_published_enrichment$Scp == relevant_scp)
                 if ( (length(indexCurrentScp)>0)
                     &(length(indexCurrentScpEnrich)>0))
                 {#Begin
                   scp_summaries = precision_summaries[indexCurrentScp,]
                   indexSelected = which((scp_summaries$Selected==TRUE)&(scp_summaries$Side_effect %in% side_effects))
                   if (length(indexSelected)>0)
                   {#Begin
                       selected_scp_summaries = scp_summaries[indexSelected,]
                       scpEffects = c()
                       indexSelectedScp=1
                       for (indexSelectedScp in 1:length(selected_scp_summaries[,1]))
                       {#Begin
                         entrytype = selected_scp_summaries$Entry_type[indexSelectedScp]
                         association = selected_scp_summaries$Association[indexSelectedScp]
                         add_scpEffect = entryTypeAssociation_scpEffect[[paste(entrytype,"-",association,sep='')]]
                         scpEffects=c(scpEffects,add_scpEffect)
                       }#End
                       scpEffects = unique(scpEffects)
                       if (length(scpEffects)>1) { scpEffects = paste(scpEffects,collapse=";") }
                       level_published_enrichment$Scp_effect[indexCurrentScpEnrich] = scpEffects
                   }#End
                 }#End
               }#End
             }#End - Add scp effects and (combined) ranks as field labels
  
             level_published_enrichment$Scp_short_factor = factor(level_published_enrichment$Scp_short,levels=scps_in_correct_spreadsheet_order)
             level_published_enrichment = level_published_enrichment[order(level_published_enrichment$Scp_short_factor),]
             
             while (length(level_published_enrichment[,1])>0)
             {#Begin -  while (length(sE_precision_cutoff_ranks[,1])>0)
               leftOverScps = unique(level_published_enrichment$Scp)
               leftOverScps = leftOverScps[order(leftOverScps)]
               showScps = leftOverScps[1:min(length(leftOverScps),max_scps_per_spreadsheet_figure)]
               indexCurrentShowScps = which(level_published_enrichment$Scp%in%showScps)
               indexLeftOverScps = which(!level_published_enrichment$Scp%in%showScps)
               current_published_enrichment = level_published_enrichment[indexCurrentShowScps,]
               level_published_enrichment = level_published_enrichment[indexLeftOverScps,]
               current_published_enrichment = current_published_enrichment[order(current_published_enrichment$Scp,decreasing=TRUE),]
               add_empty_lines_count = max_scps_per_spreadsheet_figure-length(showScps)
               label_colors = replicate(length(unique(current_published_enrichment$Scp)),"black")
               sampleNames = unique(current_published_enrichment$Sample_name)
               if (add_empty_lines_count>0)
               {#Begin
                 indexAdd=1
                 for (indexAdd in 1:add_empty_lines_count)
                 {#Begin
                   new_add_line = current_published_enrichment[1:length(sampleNames),]
                   new_add_line$Sample_name = sampleNames
                   new_add_line$Scp = paste(empty_scp_name,indexAdd,sep='')
                   new_add_line$Scp_short = new_add_line$Scp
                   new_add_line$Pvalue=0
                   new_add_line$Minus_log10_pvalue=1
                   new_add_line$Fractional_rank=empty_rank_entry
                   new_add_line$Scp_effect="E_m_p_t_y"
                   new_add_line$Overlap_count=0
                   new_add_line$ReadWrite_overlap_symbols=""
                   new_add_line$Sample_entryType = "E_m_p_t_y"
                   current_published_enrichment = rbind(new_add_line,current_published_enrichment)
                   label_colors=c("white",label_colors)
                 }#End
               }#End
               
               indexEmptyRank = which(current_published_enrichment$Fractional_rank==empty_rank_entry)
               current_published_enrichment$Field_label[indexEmptyRank] = emptyRank_field_label
               
               indexMissing = which(!current_published_enrichment$Scp_short %in% scps_in_correct_spreadsheet_order)
               scps_in_correct_spreadsheet_order_current = c(unique(current_published_enrichment$Scp_short[indexMissing]),scps_in_correct_spreadsheet_order)
               current_published_enrichment$Scp_short_factor = factor(current_published_enrichment$Scp_short,levels=scps_in_correct_spreadsheet_order_current)
               current_published_enrichment = current_published_enrichment[order(current_published_enrichment$Scp_short_factor),]
               
               scps = unique(current_published_enrichment$Scp)
               scp_colors = replicate(length(scps),"gray60")
               indexScp = 42
               for (indexScp in 1:length(scps))
               {#Begin - Set scp_colors
                 scp = scps[indexScp]
                 indexCurrentScp = which(current_published_enrichment$Scp==scp)
                 if (length(indexCurrentScp)>0)
                 {#Begin
                   scp_current_published = current_published_enrichment[indexCurrentScp,]
                   scp_effect = unique(scp_current_published$Scp_effect)
                   scp_effect = scp_effect[scp_effect!="E_m_p_t_y"]
                   if (length(scp_effect)>1) { stop("(length(scp_effect)>1)") }
                   if (length(scp_effect)==1)
                   {#Begin
                     if (scp_effect %in% names(scpEffect_color))
                     {#Begin
                       scp_colors[indexScp] = scpEffect_color[[scp_effect]]
                     }#End
                   }#End
                 }#End
               }#End - Set scp_colors
               
               indexMissing = which(!current_published_enrichment$Sample_name %in% comparison_sampleNames_in_rightOrder)
               missingSampleNames = unique(current_published_enrichment$Sample_name[indexMissing])
               current_published_enrichment$Sample_name_factor = factor(current_published_enrichment$Sample_name,levels=c(comparison_sampleNames_in_rightOrder,missingSampleNames))
  
               {#Begin - Set upDown colors             
                 entryTypes = unique(current_published_enrichment$Sample_entryType)
                 entryTypes = entryTypes[order(entryTypes)]
                 upDown_colors = replicate(length(entryTypes),"gray60")
                 for (indexET in 1:length(entryTypes))
                 {#Begin
                    entryType = entryTypes[indexET]
                    if (entryType %in% names(upDown_color_list))
                    { upDown_colors[indexET] = upDown_color_list[[entryType]] }
                 }#End
               }#End - Set upDown colors             
  
               shared_title = paste(side_effect_group,"\n",ontology," SCPs up/down at higher ranks by",sep='')

               Comparison_spreadsheet_plot = ggplot(current_published_enrichment,aes(x=Sample_name_factor,y=Scp_short_factor,fill=Sample_entryType,label=Field_label))
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + geom_tile() + coord_equal()
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + ggtitle(shared_title)
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + xlab("") + ylab("") 
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + scale_x_discrete(position = "top")
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + scale_fill_manual(values=upDown_colors)
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + geom_text(size=3.3,fontface="bold",color="white",lineheight=0.5)#,color=spreadsheet_field_label_color)
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + theme(axis.ticks.x = element_blank(), axis.ticks.y = element_blank())
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + theme(plot.title =  element_text(size=15,vjust=0.5,hjust=0.5))
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + theme(axis.text.x = element_text(size=13,angle=90,vjust=0.5,hjust=0,color="black"))
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + theme(axis.text.y = element_text(size=13,color=scp_colors))
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + theme(axis.title.y = element_text(size=10))
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + theme(axis.title.x = element_text(size=10))
               Comparison_spreadsheet_plot = Comparison_spreadsheet_plot + theme(legend.position = "none")
               comparison_collapsed_spreadheet_plots[[length(comparison_collapsed_spreadheet_plots)+1]] = Comparison_spreadsheet_plot
               
               rm(scp_colors)
               
               indexToxicScp = which(current_published_enrichment$Sample_name==foundBycardiotoxicDrugs_label)
               toxic_scps = current_published_enrichment$Scp[indexToxicScp]
               indexToxic = which(current_published_enrichment$Scp %in% toxic_scps)
               if (length(indexToxic)>0)
               {#Begin - if (length(indexToxic)>0)
                   current_published_enrichment = current_published_enrichment[indexToxic,]
                   
                   add_empty_lines_count = max_scps_per_spreadsheet_figure-length(unique(current_published_enrichment$Scp))
                   label_colors = replicate(length(unique(current_published_enrichment$Scp)),"black")
                   datasetNames = unique(current_published_enrichment$Dataset.name)
                   if (add_empty_lines_count>0)
                   {#Begin
                     for (indexAdd in 1:add_empty_lines_count)
                     {#Begin
                       new_add_line = current_published_enrichment[1:length(datasetNames),]
                       new_add_line$Dataset.name = datasetNames
                       new_add_line$Scp = paste("zzzzzz empty ",indexAdd,sep='')
                       new_add_line$Scp_short = paste("zzzzzz empty ",indexAdd,sep='')
                       new_add_line$Pvalue=0
                       new_add_line$Minus_log10_pvalue=1
                       new_add_line$Fractional_rank=empty_rank_entry
                       new_add_line$Scp_effect="E_m_p_t_y"
                       new_add_line$Overlap_count=0
                       new_add_line$ReadWrite_overlap_symbols=""
                       new_add_line$Sample_entryType = "E_m_p_t_y"
                       current_published_enrichment = rbind(new_add_line,current_published_enrichment)
                       label_colors=c("white",label_colors)
                     }#End
                   }#End
                   indexEmptyRank = which(current_published_enrichment$Fractional.rank==empty_rank_entry)
                   current_published_enrichment$Field_label[indexEmptyRank] = emptyRank_field_label
                   
                   indexMissing = which(!current_published_enrichment$Scp_short %in% scps_in_correct_spreadsheet_order)
                   scps_in_correct_spreadsheet_order_current = c(unique(current_published_enrichment$Scp[indexMissing]),scps_in_correct_spreadsheet_order)
                   current_published_enrichment$Scp_factor = factor(current_published_enrichment$Scp,levels=scps_in_correct_spreadsheet_order_current)
                   current_published_enrichment$Scp_short = Shorten_scp_names(current_published_enrichment$Scp)
                   current_published_enrichment = current_published_enrichment[order(current_published_enrichment$Scp_factor),]
                   current_published_enrichment$Scp_short_factor = factor(current_published_enrichment$Scp_short,levels=scps_in_correct_spreadsheet_order_current)
                   
                   current_published_enrichment = current_published_enrichment[order(current_published_enrichment$Scp_factor),]
                   scps = unique(current_published_enrichment$Scp)
                   scp_colors = replicate(length(scps),"gray60")
                   for (indexScp in 1:length(scps))
                   {#Begin - Set scp_colors
                     scp = scps[indexScp]
                     indexCurrentScp = which(current_published_enrichment$Scp==scp)
                     if (length(indexCurrentScp)>0)
                     {#Begin
                       clusterAvgExpr = current_published_enrichment[indexCurrentScp,]
                       scp_effect = unique(clusterAvgExpr$Scp_effect)
                       scp_effect = scp_effect[scp_effect!="E_m_p_t_Y"]
                       if (length(scp_effect)>1) { stop("(length(scp_effect)>1)") }
                       if (length(scp_effect)==1)
                       {#Begin
                          if (scp_effect %in% names(scpEffect_color))
                          {#Begin
                            scp_colors[indexScp] = scpEffect_color[[scp_effect]]
                          }#End
                       }#End
                     }#End
                   }#End - Set scp_colors
                   
                   {#Begin - Set upDown colors             
                     entryTypes = unique(current_published_enrichment$Sample_entryType)
                     entryTypes = entryTypes[order(entryTypes)]
                     upDown_colors = replicate(length(entryTypes),"gray60")
                     for (indexET in 1:length(entryTypes))
                     {#Begin
                       entryType = entryTypes[indexET]
                       if (entryType %in% names(upDown_color_list))
                       { upDown_colors[indexET] = upDown_color_list[[entryType]] }
                     }#End
                   }#End - Set upDown colors             
      
                   Toxic_collapsed_spreadsheet_plot = ggplot(current_published_enrichment,aes(x=Sample_name_factor,y=Scp_short_factor,fill=Sample_entryType,label=Field_label))
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + geom_tile() + coord_equal()
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + xlab("") + ylab("") 
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + scale_x_discrete(position = "top")
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + ggtitle(shared_title)
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + scale_fill_manual(values=upDown_colors)
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + geom_text(size=3.3,fontface="bold",color="white",lineheight=0.5)#,color=spreadsheet_field_label_color)
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + theme(axis.ticks.x = element_blank(), axis.ticks.y = element_blank())
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + theme(plot.title =  element_text(size=15,vjust=0.5,hjust=0.5))
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + theme(axis.text.x = element_text(size=13,angle=90,vjust=0.5,hjust=0,color="black"))
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + theme(axis.text.y = element_text(size=13,color=scp_colors))
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + theme(axis.title.y = element_text(size=10))
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + theme(axis.title.x = element_text(size=10))
                   Toxic_collapsed_spreadsheet_plot = Toxic_collapsed_spreadsheet_plot + theme(legend.position = "none")
                   rm(scp_colors)
                   
                   toxic_comparison_collapsed_spreadheet_plots[[length(toxic_comparison_collapsed_spreadheet_plots)+1]] = Toxic_collapsed_spreadsheet_plot
               }#End - if (length(indexToxic)>0)
             }#End -  while (length(sE_precision_cutoff_ranks[,1])>0)
           
             comparison_collapsed_spreadheet_plots_array[[comparison_reference]] = comparison_collapsed_spreadheet_plots
             toxic_comparison_collapsed_spreadheet_plots_array[[comparison_reference]] = toxic_comparison_collapsed_spreadheet_plots
           }#End - if (length(indexRelevant)>0)
           
         }#End - Generate subcluster specific expression for SCP genes
         }#End - if (length(comparison_references)>0)
         
      }#End - Generate comparison with comparison_enrichments
      
      SideEffect_comparison_collapsed_spreadheet_plots_array[[side_effect_group]] = comparison_collapsed_spreadheet_plots_array
      SideEffect_toxic_comparison_collapsed_spreadheet_plots_array[[side_effect_group]] = toxic_comparison_collapsed_spreadheet_plots_array
      rm(comparison_collapsed_spreadheet_plots_array)
      rm(toxic_comparison_collapsed_spreadheet_plots_array)
      
   }#End - Generate spreadsheet summary figure
}#End

addToFileName = paste("beta",beta_f1score,"_penalty",weight_of_penalty_AUC,sep='')

{#Begin - Generate AUC side effect plot
   SelectionScore_plots = list()
   side_effects = unique(precision_summaries$Side_effect)
   indexSE=2
   for (indexSE in 1:length(side_effects))
   {#Begin
      side_effect = side_effects[indexSE]
      indexCurrentSideEffect = which(precision_summaries$Side_effect==side_effect)
      sE_precision_summaries = precision_summaries[indexCurrentSideEffect,]
      indexSelected = which(sE_precision_summaries$Selected==TRUE)
      sE_precision_summaries = sE_precision_summaries[indexSelected,]

      oldAssociation_newAssociation_list = list()
      oldAssociation_newAssociation_list[["non-cardiotoxic TKIs"]] = "Nontox TKIs";
      oldAssociation_newAssociation_list[["cardiotoxic TKIs"]] = "Tox TKIs";
      oldAssociations = names(oldAssociation_newAssociation_list)
      here_spreadsheet_color_list = spreadsheet_color_list
      for (indexOld in 1:length(oldAssociations))
      {#Begin
         oldAssociation = oldAssociations[indexOld]
         newAssociation = oldAssociation_newAssociation_list[[oldAssociation]]
         indexCurrentAssociation = which(sE_precision_summaries$Association==oldAssociation)
         sE_precision_summaries$Association[indexCurrentAssociation] = newAssociation
         names(here_spreadsheet_color_list) = gsub(oldAssociation,newAssociation,names(here_spreadsheet_color_list))
      }#End

      sE_precision_summaries$Entry_type_association = paste(sE_precision_summaries$Entry_type,"-",sE_precision_summaries$Association,sep='')
      oldETA_newETA_list = list()
      oldETA_newETA_list[["Diffrna_down-ATCs"]] = "ATCs Down"
      oldETA_newETA_list[["Diffrna_up-ATCs"]] = "ATCs Up"
      oldETA_newETA_list[["Diffrna_down-HT TKIs"]] = "HT TKIs Down"
      oldETA_newETA_list[["Diffrna_up-HT TKIs"]] = "HT TKIs Up"
      oldETA_newETA_list[["Diffrna_down-Tox TKIs"]] = "Tox TKIs Down"
      oldETA_newETA_list[["Diffrna_up-Tox TKIs"]] = "Tox TKIs Up"
      oldETA_newETA_list[["Diffrna_down-Nontox TKIs"]] = "Non tox TKIs Down"
      oldETA_newETA_list[["Diffrna_up-Nontox TKIs"]] = "Non tox TKIs Up"

      oldETAs = names(oldETA_newETA_list)
      for (indexOld in 1:length(oldETAs))
      {#Begin
         oldETA = oldETAs[indexOld]
         newETA = oldETA_newETA_list[[oldETA]]
         indexCurrentOld = which(sE_precision_summaries$Entry_type_association==oldETA)
         if (length(indexCurrentOld)>0)
         { sE_precision_summaries$Entry_type_association[indexCurrentOld] = newETA }
         indexCurrentOld = which(names(here_spreadsheet_color_list)==oldETA)
         names(here_spreadsheet_color_list)[indexCurrentOld] = newETA
      }#End

      sE_precision_summaries$Unique_identifier = paste(sE_precision_summaries$Association,"\n",sE_precision_summaries$Ontology,sep='')
      sE_precision_summaries$Unique_identifier = gsub("_","\n",sE_precision_summaries$Unique_identifier)
      SelectionScore_plot = ggplot(sE_precision_summaries,aes(x=Unique_identifier,y=ValueForSelection,label=Selection_rank,color=Entry_type_association))
      SelectionScore_plot = SelectionScore_plot + geom_text()
      SelectionScore_plot = SelectionScore_plot + scale_colour_manual("",values=here_spreadsheet_color_list)
      SelectionScore_plot = SelectionScore_plot + ggtitle(side_effect)
      SelectionScore_plot = SelectionScore_plot + coord_cartesian(ylim=c(0,100)) 
      SelectionScore_plot = SelectionScore_plot + theme(plot.title = element_text(hjust = 0.5))
      SelectionScore_plot = SelectionScore_plot + xlab("") + ylab("corrected AUC")
      SelectionScore_plots[[length(SelectionScore_plots)+1]] = SelectionScore_plot
   }#End
   complete_pdf_fileName = paste(directory,"AUCcorrected_",addToFileName,".pdf",sep='')
   Generate_plots(SelectionScore_plots,complete_pdf_fileName,2,1,0)
}#End - Generate AUC side effect plot

side_effects = unique(precision_summaries$Side_effect)
side_effects = side_effects[order(side_effects)]

indexSideEffect=1
for (indexSideEffect in 1:length(side_effects))
{#Begin - Generate all side effect selective figures and text files
   side_effect = side_effects[indexSideEffect]
   fileName_start = side_effect
   
   indexCurrentSideEffect = which(precision_summaries$Side_effect==side_effect)
   sE_precision_summaries = precision_summaries[indexCurrentSideEffect,]
   complete_precision_summary_fileName = paste(directory,fileName_start,"_SCP_summaries_betaF1_",addToFileName,".txt",sep='')
   write.table(file=complete_precision_summary_fileName,sE_precision_summaries,row.names=FALSE,quote=FALSE,col.names=TRUE,sep='\t')

   Spreadsheet_plots = SideEffect_spreadsheet_plots[[side_effect]]
   complete_pdf_fileName = paste(directory,fileName_start,"_SCP_summary_",addToFileName,".pdf",sep='')
   Generate_plots(Spreadsheet_plots,complete_pdf_fileName,1,1,0)

   Collapsed_spreadsheet_plots = SideEffect_collapsed_spreadsheet_plots[[side_effect]]
   complete_pdf_fileName = paste(directory,fileName_start,"_SCP_summary_",addToFileName,"_collapsed.pdf",sep='')
   Generate_plots(Collapsed_spreadsheet_plots,complete_pdf_fileName,1,1,0)

   Toxic_collapsed_spreadsheet_plots = SideEffect_toxic_collapsed_spreadsheet_plots[[side_effect]]
   complete_pdf_fileName = paste(directory,fileName_start,"_SCP_summary_",addToFileName,"_onlyToxic_collapsed.pdf",sep='')
   Generate_plots(Toxic_collapsed_spreadsheet_plots,complete_pdf_fileName,1,1,0)

   comparison_collapsed_spreadheet_plots_array = SideEffect_comparison_collapsed_spreadheet_plots_array[[side_effect]]
   if (length(comparison_collapsed_spreadheet_plots_array)>0)
   {#Begin
     indexComp=1
     for (indexComp in 1:length(comparison_collapsed_spreadheet_plots_array))
     {#Begin
        referenceData = names(comparison_collapsed_spreadheet_plots_array)[indexComp]
        if (referenceData=="Published")
        { complete_pdf_fileName = paste(directory,fileName_start,"_compareWith_",referenceData,"_",addToFileName,".pdf",sep='') }
        else
        { complete_pdf_fileName = paste(directory,fileName_start,"_compareWith_",referenceData,"_adjP",scRNASeq_adjPvalue,"_top",scRNASeq_topDEGs,"DEGs_",addToFileName,".pdf",sep='') }
        Generate_plots(comparison_collapsed_spreadheet_plots_array[[referenceData]],complete_pdf_fileName,1,1,0)
     }#End
   }#End

   toxic_comparison_collapsed_spreadheet_plots_array = SideEffect_toxic_comparison_collapsed_spreadheet_plots_array[[side_effect]]
   if (length(toxic_comparison_collapsed_spreadheet_plots_array)>0)
   {#Begin
      indexComp=1
      for (indexComp in 1:length(toxic_comparison_collapsed_spreadheet_plots_array))
      {#Begin
        referenceData = names(toxic_comparison_collapsed_spreadheet_plots_array)[indexComp]
        if (referenceData=="Published")
        { complete_pdf_fileName = paste(directory,fileName_start,"_compareWith_",referenceData,"_",addToFileName,"_toxicOnly.pdf",sep='') }
        else
        { complete_pdf_fileName = paste(directory,fileName_start,"_compareWith_",referenceData,"_adjP",scRNASeq_adjPvalue,"_top",scRNASeq_topDEGs,"DEGs_",addToFileName,"_toxicOnly.pdf",sep='') }
        Generate_plots(toxic_comparison_collapsed_spreadheet_plots_array[[referenceData]],complete_pdf_fileName,1,1,0)
      }#End
   }#End

   if (generate_precisionRecallF1score_vs_cutoffRank_plots)
   {#Begin
      PrecisionRecallF1_vs_rankCutoff_plots = SideEffect_precisionRecallF1_vs_rankCutoff_plots[[side_effect]]
      complete_pdf_fileName = paste(directory,fileName_start,"_precisionRecallF1score_betaF1_",addToFileName,".pdf",sep='')
      if (generate_small_plots) { Generate_plots(PrecisionRecallF1_vs_rankCutoff_plots,complete_pdf_fileName,11,6,1) }
      else { Generate_plots(PrecisionRecallF1_vs_rankCutoff_plots,complete_pdf_fileName,generate_precisionRecallF1score_vs_cutoffRank_plots_rowsCount,generate_precisionRecallF1score_vs_cutoffRank_plots_colsCount,0) }
   }#End
}#End - Generate all side effect selective figures and text files
