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


cardiomyopathy_genomics_directory = paste(overall_lincs_directory,"//Cardiomyopathy_genomics//",sep='')

mbco_levels_list = list("2,3,4" = c(2,3,4))

indexMBCOLevels = 1

max_pvalue = 0.05
top_DEGs = 600
beta = 0.25
penalty = 0.5

predicted_directory = paste(enrichment_results_directory,"DEGenes_iPSCdCMs_P0_enrichment_maxP",max_pvalue,"_top",top_DEGs,"DEGs_in_decomposed_rocForFractional_rank\\",sep='')
predicted_fileName = paste("Cardiotoxicity_SCP_variants_foreach_drug_beta",beta,"_penalty",penalty,"_minAQ0.txt",sep='')
predictedSCPs_fileName = paste("Cardiotoxicity_SCP_summaries_betaF1_beta",beta,"_penalty",penalty,".txt",sep='')

Col_names = c("MBCO_levels","Dataset","DrugTypeGroup","Published_geneType","Trait_group","PublishedGenes_count","MBCOGenes_count","PublishedGenesInMbco_count","IdentifiedSCPgenes_count","PublishedGenes_in_identifiedSCPgenes_count","Pvalue")
Col_length = length(Col_names)
Row_names = ""
Row_length = length(Row_names)
gene_overlap_base_line = as.data.frame(array(0,c(Row_length,Col_length),dimnames = list(Row_names,Col_names)))
gene_overlaps = c()

drugTypeGroup_drugTypes_list = list("Tox and nontox TKIs" = c("Cardiotoxic_TKI","Noncardiotoxic_TKI"))
                     
drugTypeGroups = names(drugTypeGroup_drugTypes_list)

use_allSCPgenes_as_predictedSCPgenes = TRUE

association_drugType_list = list( "non-cardiotoxic TKIs" = "Noncardiotoxic_TKI"
                                 ,"cardiotoxic TKIs" = "Cardiotoxic_TKI")


indexMBCOLevels=1
traitGroup_levels=c()
for (indexMBCOLevels in 1:length(mbco_levels_list))
{#Begin - Compare published with identified genes
  mbco_levels = mbco_levels_list[[indexMBCOLevels]]
  mbco_ontologies = paste("Mbco_level",mbco_levels,sep='')
  indexDrugTypeGroup=1
  for (indexDrugTypeGroup in 1:length(drugTypeGroups))
  {#Begin
     drugTypeGroup = drugTypeGroups[indexDrugTypeGroup]
    
    {#Begin - Load and prepare Pirruccello 2020
      pirruccello2020_fileName = "Pirruccello_2020_suppl_dataFile_3.txt"
      pirruccello2020_complete_fileName = paste(cardiomyopathy_genomics_directory,pirruccello2020_fileName,sep='')
      pirruccello2020 = read.table(file=pirruccello2020_complete_fileName,header=TRUE,sep='\t',stringsAsFactors = FALSE)
      
      pirrucello2020_traits = unique(pirruccello2020$Trait)
      pirrucello2020_dataset_name = "DCM GWAS"

      pirrucello2020_traitGroupings_list = list("DCM" = c("lvef","lvesv","lvesvi"))
    }#End - Load and prepare Pirruccello 2020
    
    {#Begin - Load and prepare Harper 2021
      harper2021_fileName = "Harper_2021_suppl_table_2.txt"
      harper2021_complete_fileName = paste(cardiomyopathy_genomics_directory,harper2021_fileName,sep='')
      harper2021 = read.table(file=harper2021_complete_fileName,header=TRUE,sep='\t',stringsAsFactors = FALSE)
      harper2021$Trait = harper2021$Study      
      
      harper2021_dataset_name = "HCM GWAS"
      
      harper2021$Locus.Name = gsub("[*]","",harper2021$Locus.Name)
      indexesDouble = grep(",",harper2021$Locus.Name)
      indexIndex=1
      for (indexIndex in 1:length(indexesDouble))
      {#Begin
         indexDouble = indexesDouble[indexIndex]
         locus_name = harper2021$Locus.Name[indexDouble]
         splitStrings = strsplit(locus_name,",")[[1]]
         harper2021$Locus.Name[indexDouble] = splitStrings[1]
         for (indexSplit in 2:length(splitStrings))
         {#Begin
            new_line = harper2021[indexDouble,]
            new_line$Locus.Name = splitStrings[indexSplit]
            harper2021 = rbind(harper2021,new_line)
         }#End
      }#End

      harper2021_traits = unique(harper2021$Trait)
      
      harper2021_traitGroupings_list = list("All" = harper2021_traits)
    }#End - Load and prepare Harper 2021
    
    {#Begin - Load and prepare MBCO
      mbco_directory = libraries_for_enrichment_self_directory
      mbco = c()
      indexMBCOL = 1
      for (indexMBCOL in 1:length(mbco_ontologies))
      {#Begin
         mbco_ontology = mbco_ontologies[indexMBCOL]
         mbco_completeFileName = paste(mbco_directory,mbco_ontology,"_Homo_sapiens.txt",sep='')
         add_mbco = read.csv(file=mbco_completeFileName,header=TRUE,sep='\t',stringsAsFactors = FALSE)
         indexNotBGgenes = which(add_mbco$Scp!="Background genes")
         add_mbco = add_mbco[indexNotBGgenes,]
         if (length(mbco)>0) { mbco = rbind(mbco, add_mbco) }
         else { mbco = add_mbco }
      }#End
    }#End - Load and prepare MBCO
    
    {#Begin - Load and prepare HuGe Phenopedia
      huGe_directory = libraries_for_enrichment_download_directory
      huGe_fileName = "HuGE_phenopedia.txt"
      huGe_completeFileName = paste(huGe_directory,huGe_fileName,sep='')
      huGe = read.csv(file=huGe_completeFileName,header=TRUE,sep='\t',stringsAsFactors = FALSE)

      indexKeep = which(huGe$hpo_name %in% c("Hypertrophic cardiomyopathy","Dilated cardiomyopathy"))
      huGe = huGe[indexKeep,]
      huGe$Trait = huGe$hpo_name
      huGe$Gene_type = ""
      
      huGe_traitGroupings_list = list( "HCM" = c("Hypertrophic cardiomyopathy")
                                      ,"DCM" = c("Dilated cardiomyopathy"))
                                       
      huGe_dataset_name = "HuGE"
    }#End - Load and prepare HuGe Phenopedia
    
    if (!use_allSCPgenes_as_predictedSCPgenes)
    {#Begin - Load and prepare predicted SCP genes
      predicted_completeFileName = paste(predicted_directory,predicted_fileName,sep='')
      
      predicted = read.csv(file=predicted_completeFileName,header=TRUE,sep='\t',stringsAsFactors = FALSE)
      indexKeep = which(predicted$Ontology%in%mbco_ontologies)
      predicted = predicted[indexKeep,]
    }#End - Load and prepare predicted SCP genes
    
    if (use_allSCPgenes_as_predictedSCPgenes)
    {#Begin
       Col_names = c("Scp_gene","Drug_type","Ontology","Scp","Entry_type")
       Col_length = length(Col_names)
       Row_names = ""
       Row_length = length(Row_names)
       predicted_base_line = as.data.frame(array(0,c(Row_length,Col_length),dimnames = list(Row_names,Col_names)))
       predicted = c()
      
       predictedSCPs_completeFileName = paste(predicted_directory,predictedSCPs_fileName,sep='')
      
       predictedSCPs = read.csv(file=predictedSCPs_completeFileName,header=TRUE,sep='\t',stringsAsFactors = FALSE)
       indexKeep = which(predictedSCPs$Ontology%in%mbco_ontologies)
       predictedSCPs = predictedSCPs[indexKeep,]
       indexKeep = which(predictedSCPs$Selected==TRUE)
       predictedSCPs = predictedSCPs[indexKeep,]
       predictedSCPs$UniqueIdentifier = paste(predictedSCPs$Scp,"-",predictedSCPs$Entry_type,"-",predictedSCPs$Association,sep='')
       uniqueIdentifiers = unique(predictedSCPs$UniqueIdentifier)
       indexUnique=1
       for (indexUnique in 1:length(uniqueIdentifiers))
       {#Begin
          uniqueIdentifier = uniqueIdentifiers[indexUnique]
          indexCurrentIdentifier = which(predictedSCPs$UniqueIdentifier == uniqueIdentifier)
          uniqueId_predictedSCPs = predictedSCPs[indexCurrentIdentifier,]
          scp = unique(uniqueId_predictedSCPs$Scp_completeName)
          association = unique(uniqueId_predictedSCPs$Association)
          entryType = unique(uniqueId_predictedSCPs$Entry_type)
          ontology = unique(uniqueId_predictedSCPs$Ontology)
          indexCurrentScp_in_mbco = which(mbco$Scp==scp)
          mbco_scpGenes = unique(mbco$Target_gene_symbol[indexCurrentScp_in_mbco])
          indexMbcoGene=1
          for (indexMbcoGene in 1:length(mbco_scpGenes))
          {#Begin
             mbco_scpGene = mbco_scpGenes[indexMbcoGene]
             predicted_line = predicted_base_line
             predicted_line$Scp_gene = mbco_scpGene
             predicted_line$Ontology = ontology
             predicted_line$Scp = scp
             predicted_line$Entry_type = entryType
             predicted_line$Drug_type = association_drugType_list[[association]]
             if (length(predicted)>0) { predicted = rbind(predicted,predicted_line)}
             else { predicted = predicted_line }
          }#End
       }#End
       
       indexMissing = which(!predictedSCPs$Scp_completeName %in% predicted$Scp)
       if (length(indexMissing)>0) { stop("indexMissing = which(!predictedSCPs$Scp_completeName %in% predicted$Scp)") }
       indexMissing = which(!predicted$Scp %in%predictedSCPs$Scp_completeName)
       if (length(indexMissing)>0) { stop("indexMissing = which(!predicted$Scp %in%predictedSCPs$Scp_completeName)") }
    }#End

    drugTypes = drugTypeGroup_drugTypes_list[[drugTypeGroup]]
    indexKeep = which(predicted$Drug_type %in% drugTypes)
    predicted = predicted[indexKeep,]

    trait_groupings_list_lists = list()
    trait_groupings_list_lists[[pirrucello2020_dataset_name]] = pirrucello2020_traitGroupings_list
    trait_groupings_list_lists[[harper2021_dataset_name]] = harper2021_traitGroupings_list
    trait_groupings_list_lists[[huGe_dataset_name]] = huGe_traitGroupings_list
    
    genomics_list = list()
    genomics_list[[pirrucello2020_dataset_name]] = pirruccello2020
    genomics_list[[harper2021_dataset_name]] = harper2021
    genomics_list[[huGe_dataset_name]] = huGe
    
    geneTypes_list = list()
    geneTypes_list[[pirrucello2020_dataset_name]] = c("Nearest.Gene","TWAS.Gene")
    geneTypes_list[[harper2021_dataset_name]] = c("Locus.Name")
    geneTypes_list[[huGe_dataset_name]] = c("gene_symbol")

    indexDataset=1    
    for (indexDataset in 1:length(genomics_list))
    {#Begin
       current_datasetName = names(genomics_list)[indexDataset]
       geneTypes = geneTypes_list[[current_datasetName]]
       current_trait_groupings_list = trait_groupings_list_lists[[current_datasetName]]
       current_genomics = genomics_list[[current_datasetName]]

       indexGT=1
      for (indexGT in 1:length(geneTypes))
      {#Begin
        current_geneType = geneTypes[indexGT]
        indexTGL=1
        for (indexTGL in 1:length(current_trait_groupings_list))
        {#Begin
          trait_group = names(current_trait_groupings_list)[indexTGL]
          traits = current_trait_groupings_list[[trait_group]]
          indexCurrentTraits = which(current_genomics$Trait%in%traits)
          traitGroup_genomics = current_genomics[indexCurrentTraits,]
          publishedGenes = unique(traitGroup_genomics[[current_geneType]])
          publishedGenes = publishedGenes[publishedGenes!=""]
          indexPublishedInMBCO = which(publishedGenes%in%mbco$Target_gene_symbol)
          
          publishedMBCOGenes = publishedGenes[indexPublishedInMBCO]
          predictedSCPgenes = unique(predicted$Scp_gene)
          indexPublishedInIdentified = which(publishedGenes%in%predictedSCPgenes)
          publishedInIdentified = publishedGenes[indexPublishedInIdentified]
          if (length(which(!predictedSCPgenes %in% mbco$Target_gene_symbol)>0)) { rm(predictedSCPgenes); rm(publishedGenes); rm(gene_overlap_base_line) }
          if (length(unique(publishedGenes))!=length(publishedGenes))  { rm(predictedSCPgenes); rm(publishedGenes); rm(gene_overlap_base_line) }
          if (length(unique(predictedSCPgenes))!=length(predictedSCPgenes))  { rm(predictedSCPgenes); rm(publishedGenes); rm(gene_overlap_base_line) }
          if (length(unique(publishedMBCOGenes))!=length(publishedMBCOGenes))  { rm(predictedSCPgenes); rm(publishedGenes); rm(gene_overlap_base_line) }
          if (length(unique(publishedInIdentified))!=length(publishedInIdentified))  { rm(predictedSCPgenes); rm(publishedGenes); rm(gene_overlap_base_line) }
          
          gene_overlap_line = gene_overlap_base_line
          gene_overlap_line$DrugTypeGroup = drugTypeGroup
          gene_overlap_line$MBCO_levels = paste(mbco_levels,collapse=';')
          gene_overlap_line$Dataset = current_datasetName
          gene_overlap_line$Published_geneType = current_geneType
          gene_overlap_line$Trait_group = trait_group
          gene_overlap_line$PublishedGenes_count = length(publishedGenes)
          gene_overlap_line$MBCOGenes_count = length(unique(mbco$Target_gene_symbol))
          gene_overlap_line$PublishedGenesInMbco_count = length(publishedMBCOGenes)
          gene_overlap_line$IdentifiedSCPgenes_count = length(predictedSCPgenes)
          gene_overlap_line$PublishedGenes_in_identifiedSCPgenes_count = length(publishedInIdentified)
          
          Array = array(0,c(2,2))
          Array[1,1] = gene_overlap_line$PublishedGenes_in_identifiedSCPgenes_count
          Array[1,2] = gene_overlap_line$IdentifiedSCPgenes_count - Array[1,1]
          Array[2,1] = gene_overlap_line$PublishedGenesInMbco_count - Array[1,1]
          Array[2,2] = gene_overlap_line$MBCOGenes_count - Array[1,1] - Array[1,2] - Array[2,1]
          if (sum(Array)!=length(unique(mbco$Target_gene_symbol))) { rm(Array) }
          
          fisher_results = fisher.test(Array,alternative="greater")
          gene_overlap_line$Pvalue = fisher_results$p.value
          
          if (length(gene_overlaps)>0) { gene_overlaps = rbind(gene_overlaps,gene_overlap_line)}
          else { gene_overlaps = gene_overlap_line }
        }#End
      }#End
    }#End
  }#End
}#End - Compare published with identified genes

{#Begin - Process gene overlaps
  gene_overlaps$Published_geneType = gsub("Nearest.Gene","Nearest gene",gene_overlaps$Published_geneType)
  gene_overlaps$Published_geneType = gsub("TWAS.Gene","TWAS gene",gene_overlaps$Published_geneType)
  gene_overlaps$Published_geneType = gsub("Locus.Name","Locus name",gene_overlaps$Published_geneType)
  
  gene_overlaps$FinalName = gene_overlaps$Trait_group
  indexPublishedGeneTypeExists = which(gene_overlaps$Published_geneType!="")
  gene_overlaps$FinalName[indexPublishedGeneTypeExists] = paste(gene_overlaps$Trait_group[indexPublishedGeneTypeExists]," (",gene_overlaps$Published_geneType[indexPublishedGeneTypeExists],")",sep='')
  gene_overlaps$Minus_log10pvalue = -log10(gene_overlaps$Pvalue)
  
  geneType_levels = c("Nearest gene","TWAS gene","Locus name")

  gene_overlaps$Published_geneType_factor = factor(gene_overlaps$Published_geneType,levels=geneType_levels)
  gene_overlaps = gene_overlaps[order(gene_overlaps$Trait_group),]
  gene_overlaps$Trait_group_factor = factor(gene_overlaps$Trait_group,levels=unique(gene_overlaps$Trait_group))
  
  gene_overlaps = gene_overlaps[order(gene_overlaps$Published_geneType_factor),]
  gene_overlaps = gene_overlaps[order(gene_overlaps$Trait_group_factor),]
  finalName_levels = unique(gene_overlaps$FinalName)
}#End - Process gene overlaps

{#Begin - Add gene overlaps for combined data
   dataset_finalName_list = list()
   dataset_finalName_list[[pirrucello2020_dataset_name]] = c("DCM (Nearest gene)","DCM (TWAS gene)")
   dataset_finalName_list[[harper2021_dataset_name]] = c("All (Locus name)")
   #dataset_finalName_list[[huGe_dataset_name]] = c("DCM (Symbol)","HCM (Symbol)")
   
   datasets = names(dataset_finalName_list)
   indexD=1
   for (indexD in 1:length(datasets))
   {#Begin
      dataset = datasets[indexD]
      finalNames = dataset_finalName_list[[dataset]]
      indexCurrentDataset = which(gene_overlaps$Dataset==dataset)
      dataset_geneOverlaps = gene_overlaps[indexCurrentDataset,]
      indexKeep = which(dataset_geneOverlaps$FinalName %in% finalNames)
      dataset_geneOverlaps = dataset_geneOverlaps[indexKeep,]
      dataset_geneOverlaps$FinalName = paste(dataset_geneOverlaps$Dataset, "\n",dataset_geneOverlaps$FinalName,sep='')
      dataset_geneOverlaps$Dataset = "All"
      dataset_geneOverlaps$Published_geneType = gsub("Locus name","Nearest gene",dataset_geneOverlaps$Published_geneType)
      dataset_geneOverlaps = dataset_geneOverlaps[order(dataset_geneOverlaps$FinalName),]
      finalName_levels = c(finalName_levels,unique(dataset_geneOverlaps$FinalName))
      gene_overlaps = rbind(gene_overlaps,dataset_geneOverlaps)
   }#End
}#End - Add gene overlaps for combined data

number_of_bars = 10

Plots = list()
datasetNames = unique(gene_overlaps$Dataset)

indexHuge = which(gene_overlaps$Dataset=="HuGE")
gene_overlaps$Published_geneType

indexDN=3

for (indexDN in 1:length(datasetNames))
{#Begin
   dataset = datasetNames[indexDN]
   indexCurrentDataset = which(gene_overlaps$Dataset==dataset)
   dataset_geneOverlaps = gene_overlaps[indexCurrentDataset,]
   drugTypeGroups = unique(dataset_geneOverlaps$DrugTypeGroup)
   indexDrugTypeGroup = 1
   for (indexDrugTypeGroup in 1:length(drugTypeGroups))
   {#Begin
      drugTypeGroup = drugTypeGroups[indexDrugTypeGroup]
      indexCurrentDrugTypeGroup = which(dataset_geneOverlaps$DrugTypeGroup==drugTypeGroup)
      drugTypeGroup_geneOverlaps = dataset_geneOverlaps[indexCurrentDrugTypeGroup,]

      considered_mbco_levels = unique(drugTypeGroup_geneOverlaps$MBCO_levels)
      indexLevel=1
      for (indexLevel in 1:length(considered_mbco_levels))
      {#Begin
         considered_mbco_level = considered_mbco_levels[indexLevel]
         indexCurrentLevel = which(drugTypeGroup_geneOverlaps$MBCO_levels==considered_mbco_level)
         level_geneOverlaps = drugTypeGroup_geneOverlaps[indexCurrentLevel,]
         trait_colors_list = list( "Nearest gene" = "gray45"
                                  ,"TWAS gene" = "black"
                                  ,"Locus name" = "gray45"
                                  ,"gene_symbol" = "gray75")
           
         missing_bars_count = number_of_bars - length(level_geneOverlaps[,1])
         added_finalName_levels = c()
         if (missing_bars_count>0)
         {#Begin
            for (indexMissing in 1:missing_bars_count)
            {#Begin
               new_line = level_geneOverlaps[1,]
               new_line$FinalName = paste("empty ",indexMissing,sep='')
               added_finalName_levels = c(added_finalName_levels,new_line$FinalName)
               new_line$Minus_log10pvalue=0
               new_line$MBCOGenes_count=0
               new_line$PublishedGenesInMbco_count=0
               new_line$IdentifiedSCPgenes_count=0
               new_line$PublishedGenes_count=0
               new_line$PublishedGenes_in_identifiedSCPgenes_count=0
               level_geneOverlaps = rbind(level_geneOverlaps,new_line)
            }#End
         }#End
        
         level_geneOverlaps$FinalName_factor = factor(level_geneOverlaps$FinalName,levels=c(finalName_levels,added_finalName_levels))
        
         if (use_allSCPgenes_as_predictedSCPgenes)
         {#Begin
            usedGenes_string = "all SCP genes"
         }#End
         else
         {#Begin
            usedGenes_string = "only SCP genes with variants"
         }#End
         headline = paste(dataset," ",drugTypeGroup,"\n(MBCO SCPs of levels ",paste(considered_mbco_level,collapse=','),")\n",usedGenes_string,sep='')

         
         
         Plot = ggplot(level_geneOverlaps,aes(x=FinalName_factor,y=Minus_log10pvalue,fill=Published_geneType))
         Plot = Plot + geom_bar(stat = "identity")
         Plot = Plot + ylab("-log10(p)")
         Plot = Plot + xlab("")
         Plot = Plot + scale_fill_manual(values=trait_colors_list)
         Plot = Plot + theme(axis.text.x = element_text(size=15))
         Plot = Plot + theme(axis.text.y = element_text(size=15))
         Plot = Plot + geom_hline(yintercept = -log10(0.05))
         #Plot = Plot + geom_hline(yintercept = -log10(0.1))
         Plot = Plot + scale_y_continuous(breaks=seq(0,12,by=2))
         Plot = Plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
         Plot = Plot + theme(axis.title.x = element_blank())
         Plot = Plot + theme(axis.title.y = element_text(size=15,color="black"))
         Plot = Plot + ggtitle(headline)
         Plot = Plot + theme(plot.title = element_text(hjust=0.5,face=2,size=15))
         Plot = Plot + theme(legend.position = "none")
         Plots[[length(Plots)+1]] = Plot
      }#End
   }#End
}#End

{#Begin - Write figures
  if (use_allSCPgenes_as_predictedSCPgenes)
  { complete_pdf_fileName = paste(predicted_directory,gsub(".txt","_allSCPGenes_literature.pdf",predicted_fileName),sep='') }
  else
  { complete_pdf_fileName = paste(predicted_directory,gsub(".txt","_SCPGenesWithVariants_literature.pdf",predicted_fileName),sep='') }
  Generate_plots(Plots,complete_pdf_fileName,2,1)
}#End - Write figures
  
