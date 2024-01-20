rm(list = ls());
rm(list = ls(all.names = TRUE));
gc();
library(ggplot2)
library(gridExtra)
library(qlcMatrix)

cardiotoxicity_minFrequencyGroup_cutoff = 2 #!!!!!!!!!!!!!!!!!!!!!!!!!! Has to agree with C# script !!!!!!!!!!!!!!!!!!!!!!!!!!

delete_task_reports=FALSE
get_eigenassays_per_dataset=FALSE
source('SVD_global_parameter.R')
source('SVD_colors.R')
source('Common_tools.R')

baseDirectory = overall_lincs_directory
metadata_fileName = "Drug_metadata.txt"
complete_metadata_fileName = paste(metadata_directory,metadata_fileName,sep='')
directory = paste(enrichment_results_directory,"DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank//",sep='')
scp_summaries_fileName = "Cardiotoxicity_SCP_summaries_betaF1_beta0.25_penalty0.5_plusVariantCounts_minAQ0.txt"
complete_scp_summaries_fileName = paste(directory,scp_summaries_fileName,sep='')
scp_summaries = read.table(file=complete_scp_summaries_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
ontologies = unique(scp_summaries$Ontology)
ontologies = ontologies[!ontologies%in%c("All combined")]
ontologies = ontologies[order(ontologies)]
indexO=1

metadata = read.csv(complete_metadata_fileName,header = TRUE,stringsAsFactors = FALSE,sep='\t')

indexTKIs = which(metadata$Drug_type %in% c("Kinase_inhibitor","Monoclonal_antibody"))
indexCardiotoxic = which(metadata$Cardiotoxicity_frequencyGroup >= cardiotoxicity_minFrequencyGroup_cutoff)
indexNonCardiotoxic = which(metadata$Cardiotoxicity_frequencyGroup < cardiotoxicity_minFrequencyGroup_cutoff)
indexCardiotoxicTKIs = indexTKIs[indexTKIs %in% indexCardiotoxic]
indexNonCardiotoxicTKIs = indexTKIs[indexTKIs %in% indexNonCardiotoxic]

cardiotoxic_tkis = metadata$Drug[indexCardiotoxicTKIs]
noncardiotoxic_tkis = metadata$Drug[indexNonCardiotoxicTKIs]

associations_list = list()
associations_list[[tki_harmful_label]] = cardiotoxic_tkis
associations_list[[tki_protective_label]] = noncardiotoxic_tkis
associations = names(associations_list)


Plots = list()

max_scps_per_spreadsheet_figure = 25

indexAssociation=1
for (indexAssociation in 1:length(associations))
{#Begin
  association = associations[indexAssociation]
  effectType=association
  indexCurrentAssociation = which(scp_summaries$Association==association)
  association_scp_summaries = scp_summaries[indexCurrentAssociation,]
  current_drugs = associations_list[[association]]
  indexO=1
  for (indexO in 1:length(ontologies))
  {#Begin
     ontology = ontologies[indexO]
     ontology_abbreviation = ontologyEnum_ontologyAbbreviation_list[[ontology]]
     indexCurrentOntology = which(association_scp_summaries$Ontology==ontology)
     current_scp_summaries = association_scp_summaries[indexCurrentOntology,]
  
     max_selection_rank = ontology_topScps_harmfulOrProtective_forPrecsion_list[[ontology_abbreviation]]
     indexKeep = which(current_scp_summaries$Selection_rank<=max_selection_rank)
     current_scp_summaries = current_scp_summaries[indexKeep,]
     indexUp = which(current_scp_summaries$Entry_type=="Diffrna_up")
     indexDown= which(current_scp_summaries$Entry_type=="Diffrna_down")
     up_scps = current_scp_summaries$Scp_completeName[indexUp]
     down_scps = current_scp_summaries$Scp_completeName[indexDown]

     {#Begin - Read and prepare enrichment
         enrichment_fileName = paste(ontology_abbreviation,".txt",sep='')
         complete_enrichment_fileName = paste(directory,enrichment_fileName,sep='')
         enrichment = read.table(file=complete_enrichment_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
         max_rank = ontology_maxRank_list[[ontology_abbreviation]]
         enrichment = Add_drugs_celllines_f1ScoreWeights_drugTypes_to_enrichment_dataframe(enrichment)
         indexKeep = which(enrichment$Drug %in% current_drugs)
         enrichment = enrichment[indexKeep,]
         indexEnrichUp = which(enrichment$Sample_entryType=="Diffrna_up")
         indexEnrichDown = which(enrichment$Sample_entryType=="Diffrna_down")
         indexEnrichUpScps = which(enrichment$Scp %in% up_scps)
         indexEnrichDownScps = which(enrichment$Scp %in% down_scps)
         indexEnrichUpScps_up = indexEnrichUpScps[indexEnrichUpScps %in% indexEnrichUp]
         indexEnrichUpScps_down = indexEnrichDownScps[indexEnrichDownScps %in% indexEnrichDown]
         indexKeep = unique(c(indexEnrichUpScps_up,indexEnrichUpScps_down))
         enrichment = enrichment[indexKeep,]
     }#End - Read and prepare enrichment
     
     label_color = "white"
     label_size = 3
     label_lineheight = 0.9
     max_label_lines = 2
     if (association==tki_protective_label) { label_color = "black"; label_size = 2.75; label_lineheight = 0.8; max_label_lines=3 }

     {#Begin - Calculate drug average enrichment
       drug_average_enrichments = c()
       entryTypes = unique(enrichment$Sample_entryType)
       indexET=1
       for (indexET in 1:length(entryTypes))
       {#Begin
         entryType = entryTypes[indexET]
         indexCurrentEntryType = which(enrichment$Sample_entryType==entryType)
         entryType_enrichment = enrichment[indexCurrentEntryType,]
         scps = unique(entryType_enrichment$Scp)
         indexScp=1
         for (indexScp in 1:length(scps))
         {#Begin
           scp = scps[indexScp]
           indexCurrentScps = which(entryType_enrichment$Scp==scp)
           scp_enrichment = entryType_enrichment[indexCurrentScps,]
           drugs = unique(scp_enrichment$Drug)
           indexDrug=1
           for (indexDrug in 1:length(drugs))
           {#Begin
             drug = drugs[indexDrug]
             indexCurrentDrug = which(scp_enrichment$Drug ==drug)
             drug_enrichment = scp_enrichment[indexCurrentDrug,]
             drug_enrichment = drug_enrichment[order(drug_enrichment$Fractional_rank),]
             if (min(drug_enrichment$Fractional_rank)<=max_rank)
             {#Begin
               total_treated_celllines_count = length(drug_enrichment[,1])
               indexKeep = which(drug_enrichment$Fractional_rank<=max_rank)
               drug_enrichment = drug_enrichment[indexKeep,]
               label = paste(floor(drug_enrichment$Fractional_rank+0.5),collapse=" ")
               label = Split_name_over_multiple_lines(label,3,max_label_lines)
               label = gsub(" ",";",label)
               average_enrichment = drug_enrichment[1,]
               average_enrichment$Entity_label = paste(full_drugNames[drug]," (n=",total_treated_celllines_count,")",sep='')
               average_enrichment$Label = label
               average_enrichment$Mean_fractional_rank = mean(drug_enrichment$Fractional_rank)
               average_enrichment$Sd_fractional_rank = sd(drug_enrichment$Fractional_rank)
               average_enrichment$Median_fractional_rank = sd(drug_enrichment$Fractional_rank)
               if (length(drug_average_enrichments)>0) { drug_average_enrichments = rbind(drug_average_enrichments,average_enrichment)}
               else { drug_average_enrichments = average_enrichment }
             }#End
           }#End
         }#End
       }#End
       round_factor=10
       drug_average_enrichments$Scp = Shorten_scp_names(drug_average_enrichments$Scp)
       drug_average_enrichments$Entity = drug_average_enrichments$Drug

       indexUp = which(drug_average_enrichments$Sample_entryType=="Diffrna_up")
       indexDown = which(drug_average_enrichments$Sample_entryType=="Diffrna_down")
       drug_average_enrichments$Fractional_rank[indexDown] = -drug_average_enrichments$Fractional_rank[indexDown]
       #indexKeep = which(average_enrichments$Mean_fractional_rank<=max_rank)
       #average_enrichments = average_enrichments[indexKeep,]
     }#End - Calculate drug average enrichment
     
     {#Begin - Set scps_in_correct_spreadsheet_order_current for drugs
       indexCurrentScps = which(scps_in_correct_spreadsheet_order %in% current_scp_summaries$Scp)
       scps_in_correct_spreadsheet_order_current = scps_in_correct_spreadsheet_order[indexCurrentScps]
       indexMissing = which(!current_scp_summaries$Scp %in% scps_in_correct_spreadsheet_order_current)
       scps_in_correct_spreadsheet_order_current = c(scps_in_correct_spreadsheet_order_current,current_scp_summaries$Scp[indexMissing])
       drug_average_enrichments$Scp_factor = factor(drug_average_enrichments$Scp,levels=scps_in_correct_spreadsheet_order_current)
     }#End - Set scps_in_correct_spreadsheet_order_current for drugs

     {#Begin - Set drug colors as drugClass colors
       drugs = drug_average_enrichments$Entity
       drug_color_list = list()
       for (indexD in 1:length(drugs))
       {#Begin
         drug = drugs[indexD]
         if (drug %in% names(drug_drugClass))
         {#Begin
           drugClass = drug_drugClass[[drug]]
           drug_color_list[[drug]] = drugClass_colors[[drugClass]]
         }#End
         else
         {#Begin
           drug_color_list[[drug]] = "gray"
         }#End
       }#End
     }#End - Set drug colors as drugClass colors

     average_enrichment_list = list("Drug" = drug_average_enrichments)

     entities_in_correct_order_list = list("Drug" = drugs_in_drugClass_specific_order)
     entity_color_list_list = list("Drug" = drug_color_list)
     
     indexAEL=2
     for (indexAEL in 1:length(average_enrichment_list))
     {#Begin
        entityClass = names(average_enrichment_list)[indexAEL]
        entities_in_correct_order = entities_in_correct_order_list[[entityClass]]
        entity_color_list = entity_color_list_list[[entityClass]]
        average_enrichments = average_enrichment_list[[entityClass]]
        while (length(average_enrichments[,1])>0)
        {#Begin -  while (length(sE_precision_cutoff_ranks[,1])>0)
          leftOverScps = unique(average_enrichments$Scp_factor)
          leftOverScps = leftOverScps[order(leftOverScps)]
          showScps = leftOverScps[1:min(length(leftOverScps),max_scps_per_spreadsheet_figure)]
          indexCurrentShowScps = which(average_enrichments$Scp_factor%in%showScps)
          indexLeftOverScps = which(!average_enrichments$Scp_factor%in%showScps)
          current_average_enrichments = average_enrichments[indexCurrentShowScps,]
          average_enrichments = average_enrichments[indexLeftOverScps,]
          current_average_enrichments = current_average_enrichments[order(current_average_enrichments$Scp_factor,decreasing=TRUE),]
          add_empty_lines_count = max_scps_per_spreadsheet_figure-length(showScps) + 1
          label_colors = replicate(length(unique(current_average_enrichments$Scp_factor)),"black")
          entities = unique(current_average_enrichments$Entity)
          entity_labels = unique(current_average_enrichments$Entity_label)
          if (add_empty_lines_count>0)
          {#Begin
             for (indexAdd in 1:add_empty_lines_count)
             {#Begin
               new_add_line = current_average_enrichments[1:length(entities),]
               new_add_line$Entity = entities
               new_add_line$Entity_label = entity_labels
               add_string = indexAdd
               while (nchar(add_string)<2) { add_string = paste(add_string,"L",sep='') }
               new_add_line$Scp = paste("zzzzzz empty long text long text longgg",add_string,sep='')
               new_add_line$Label=""
               new_add_line$Sample_entryType = "E_m_p_t_y"
               current_average_enrichments = rbind(new_add_line,current_average_enrichments)
               label_colors=c("white",label_colors)
             }#End
         }#End
         
         {#Begin - Add variant counts to each SCP
           variant_count_columnName = "# variants"
           for (indexSummary in 1:length(current_scp_summaries[,1]))
           {#Begin
             new_enrichment_line = current_average_enrichments[1,]
             new_enrichment_line$Scp = current_scp_summaries$Scp[indexSummary]
             new_enrichment_line$Sample_entryType = variantCount_entryType
             new_enrichment_line$Entity = variant_count_columnName
             new_enrichment_line$Entity_label = variant_count_columnName
             new_enrichment_line$Label = current_scp_summaries$Number_of_variants[indexSummary]
             current_average_enrichments = rbind(current_average_enrichments,new_enrichment_line)
           }#End
         }#End - Add variant counts to each SCP

         {#Begin - Reset scps_in_correct_spreadsheet_order_current after addition of empty SCPs
            indexCurrentScps = which(scps_in_correct_spreadsheet_order %in% current_average_enrichments$Scp)
            scps_in_correct_spreadsheet_order_current = scps_in_correct_spreadsheet_order[indexCurrentScps]
            indexMissing = which(!current_average_enrichments$Scp %in% scps_in_correct_spreadsheet_order_current)
            scps_in_correct_spreadsheet_order_current = c(unique(current_average_enrichments$Scp[indexMissing]),scps_in_correct_spreadsheet_order_current)
            current_average_enrichments$Scp_factor = factor(current_average_enrichments$Scp,levels=scps_in_correct_spreadsheet_order_current)
            current_average_enrichments$Scps_lineBreak = Split_names_over_multiple_lines(current_average_enrichments$Scp,15,2)
            indexZZZZZ = grep("zzzzzz",current_average_enrichments$Scp)
            current_average_enrichments$Scps_lineBreak[indexZZZZZ] = current_average_enrichments$Scp[indexZZZZZ]
            current_average_enrichments = current_average_enrichments[order(current_average_enrichments$Scp_factor,decreasing=FALSE),]
            current_average_enrichments$Scps_lineBreak_factor = factor(current_average_enrichments$Scps_lineBreak,levels=unique(current_average_enrichments$Scps_lineBreak))
         }#End - Reset scps_in_correct_spreadsheet_order_current after addition of empty SCPs

         current_average_enrichments$EntryType_effect = paste(current_average_enrichments$Sample_entryType,"-",effectType,sep='')
         indexVariantCount = which(current_average_enrichments$Sample_entryType==variantCount_entryType)
         current_average_enrichments$EntryType_effect[indexVariantCount] = variantCount_entryType

         {#Begin - Set entity colors and factorize entity_labels
            current_average_enrichments$Entity_factor = factor(current_average_enrichments$Entity,levels=c(entities_in_correct_order,variant_count_columnName))
            current_average_enrichments = current_average_enrichments[order(current_average_enrichments$Entity_factor),]
            current_average_enrichments$Entity_label_factor = factor(current_average_enrichments$Entity_label,levels=unique(c(current_average_enrichments$Entity_label,variant_count_columnName)))
            
            entities = unique(current_average_enrichments$Entity)
            entityColors = replicate(length(entities),"black")
            for (indexEntity in 1:length(entities))
            {#Begin
               entity = entities[indexEntity]
               if (entity %in% names(entity_color_list))
               { entityColors[indexEntity] = entity_color_list[[entity]] }
            }#End
         }#End - Set entity colors and factorize entity_labels
         
         plot_title = paste("Ranks of ",ontology," SCPs\ninduced by ",association,sep='')
         
         Plot = ggplot(current_average_enrichments,aes(y=Scps_lineBreak_factor,x=Entity_label_factor,label=Label,fill=EntryType_effect))
         Plot = Plot + geom_tile(color="white")
         Plot = Plot + ggtitle(plot_title)
         Plot = Plot + scale_fill_manual(values=spreadsheet_color_list)#scale_fill_gradient(low="blue",high="orange")
         Plot = Plot + geom_text(color=label_color,size=label_size,lineheight=label_lineheight)
         Plot = Plot + scale_x_discrete(position = "top")
         Plot = Plot + xlab("") + ylab("")
         Plot = Plot + theme(axis.ticks.x = element_blank(), axis.ticks.y = element_blank())
         Plot = Plot + theme(plot.title =  element_text(size=15,face=2,vjust=0.5,hjust=0.5))
         Plot = Plot + theme(axis.text.x = element_text(size=13,angle=90,vjust=0.5,hjust=0,face=2,color=entityColors))
         Plot = Plot + theme(axis.text.y = element_text(size=9,face=2,color = label_colors))
         Plot = Plot + theme(axis.title.y = element_text(size=10))
         Plot = Plot + theme(axis.title.x = element_text(size=10))
         Plot = Plot + theme(legend.position = "none")
         Plots[[length(Plots)+1]] = Plot
        }#End
     }#End
  }#End
}#End

complete_pdf_fileName = paste(directory,gsub(".txt",".pdf",scp_summaries_fileName),sep='')
Generate_plots(Plots,complete_pdf_fileName,1,1)


