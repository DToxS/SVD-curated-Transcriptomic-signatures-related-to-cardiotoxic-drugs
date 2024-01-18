rm(list = ls());
rm(list = ls(all.names = TRUE));
gc();
library(ggplot2)
library(gridExtra)
library(qlcMatrix)

delete_task_reports=FALSE
get_eigenassays_per_dataset=FALSE
source('SVD_global_parameter.R')
source('SVD_colors.R')
source('Common_tools.R')

directory = paste(enrichment_results_directory,"DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank//",sep='')
variant_counts_fileName = "Cardiotoxicity_SCP_variant_counts_foreach_drug_beta0.25_penalty0.5_minAQ0.txt"
complete_variant_counts_fileName = paste(directory,variant_counts_fileName,sep='')
variant_counts = read.table(file=complete_variant_counts_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')

mbco_color_list = list(#"Mbco_level1" = "black",
                       "Mbco_level2" = "gray63",
                       "Mbco_level3" = "black", 
                       "Mbco_level4" = "gray63"
                       )


Plots=list()
sideEffects = unique(variant_counts$Side_effect)
indexSE=1
for (indexSE in 1:length(sideEffects))
{#Begin
   sideEffect = sideEffects[indexSE]
   indexCurrentSideEffect = which(variant_counts$Side_effect==sideEffect)
   sideEffect_variant_counts = variant_counts[indexCurrentSideEffect,]
   associations = unique(sideEffect_variant_counts$Toxicity_association)
   indexAsso=1
   for (indexAsso in 1:length(associations))
   {#Begin
      association = associations[indexAsso]
      indexCurrentAssociation = which(sideEffect_variant_counts$Toxicity_association==association)
      association_variant_counts = sideEffect_variant_counts[indexCurrentAssociation,]
      indexKeep = which(association_variant_counts$O %in% names(mbco_color_list))
      association_variant_counts = association_variant_counts[indexKeep,]

      indexUp = which(association_variant_counts$EntryType=="Diffrna_up")
      indexDown = which(association_variant_counts$EntryType=="Diffrna_down")
      fullDrugNames = unique(association_variant_counts$Drug)
      association_variant_counts$Drug_abbreviation = association_variant_counts$Drug
      drugs = unique(association_variant_counts$Drug)
      for (indexDrug in 1:length(drugs))
      {#Begin
         drug = drugs[indexDrug]
         if (drug %in% names(abbreviation_drugs))
         {#Begin
            indexCurrentDrug = which(association_variant_counts$Drug_abbreviation==drug)
            association_variant_counts$Drug_abbreviation[indexCurrentDrug] = abbreviation_drugs[[drug]]
         }#End
      }#End

      association_variant_counts$Drug_direction = association_variant_counts$Drug
      association_variant_counts$Drug_direction[indexUp] = paste(association_variant_counts$Drug_direction[indexUp]," - Up",sep='')
      association_variant_counts$Drug_direction[indexDown] = paste(association_variant_counts$Drug_direction[indexDown]," - Down",sep='')
      
      {#Begin - Set drug colors and factorize drug_labels
        indexMissing = which(!association_variant_counts$Drug_abbreviation %in% drugs_in_drugClass_specific_order)
        drugs_in_drugClass_specific_order_current = c(drugs_in_drugClass_specific_order,unique(association_variant_counts$Drug_abbreviation[indexMissing]))
        association_variant_counts$Drug_abbreviation_factor = factor(association_variant_counts$Drug_abbreviation,levels=drugs_in_drugClass_specific_order_current)
        association_variant_counts = association_variant_counts[order(association_variant_counts$Drug_abbreviation_factor),]
        association_variant_counts$Drug_direction_factor = factor(association_variant_counts$Drug_direction,levels=unique(association_variant_counts$Drug_direction))
        
        drugs = unique(association_variant_counts$Drug)
        drugColors = replicate(length(drugs),"black")
        indexDrug=17
        for (indexDrug in 1:length(drugs))
        {#Begin
          drug = drugs[indexDrug]
          drug_abbreviation = drug
          if (drug_abbreviation %in% names(abbreviation_drugs))
          { drug_abbreviation = abbreviation_drugs[[drug]] }
          if (drug_abbreviation %in% names(drug_drugClass))
          {#Begin
            drugClass = drug_drugClass[[drug_abbreviation]]
            #drugColors[indexDrug*2-1] = drugClass_colors[[drugClass]]
            drugColors[indexDrug] = drugClass_colors[[drugClass]]
          }#End
        }#End
      }#End - Set drug colors and factorize drug_labels
      
      drug_x_label_size=13
      title_size=20
      Plot = ggplot(association_variant_counts,aes(y=Count_of_variants_that_are_not_covered_by_lower_level_scps,x=Drug_direction_factor,fill=Ontology))
      Plot = Plot + geom_bar(stat="identity")
      Plot = Plot + ylab("# Variants")
      Plot = Plot + xlab("")
      Plot = Plot + scale_fill_manual(values=mbco_color_list)
      Plot = Plot + theme(axis.text.x = element_text(size=drug_x_label_size,color=drugColors))
      Plot = Plot + theme(axis.text.y = element_text(size=drug_x_label_size,color="black"))
      Plot = Plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
      Plot = Plot + theme(axis.title.x = element_blank())
      Plot = Plot + theme(axis.title.y = element_text(size=drug_x_label_size,color="black"))
      Plot = Plot + ggtitle(paste(sideEffect," - ",association,sep=''))
      Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=title_size))
      Plot = Plot + theme(legend.position = "none")
      Plots[[length(Plots)+1]] = Plot
   }#End
}#End

complete_pdf_fileName = paste(directory,gsub(".txt",".pdf",variant_counts_fileName),sep='')
Generate_plots(Plots,complete_pdf_fileName,2,1)


