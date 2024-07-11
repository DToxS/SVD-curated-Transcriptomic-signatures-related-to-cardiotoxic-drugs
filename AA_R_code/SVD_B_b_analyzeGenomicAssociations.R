#stopCluster(parallel_clusters)
library(ggplot2)
library(gridExtra)
rm(list = ls());

#######################################################################################################################
## Generate tasks - BEGIN
delete_task_reports=FALSE
get_eigenassays_per_dataset=FALSE
source('SVD_global_parameter.R')
source('SVD_colors.R')
source('Common_tools.R')

cardiotoxicity_minFrequencyGroup_cutoff = 2 #!!!!!!!!!!!!!!!!!!!!!!!!!! Has to agree with C# script !!!!!!!!!!!!!!!!!!!!!!!!!!

directory = drugTarget_directory
fileName = "Identified_variants_count_minAQ20.txt"
complete_fileName = paste(directory,fileName,sep='')
genomics = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
genomics$Relation_to_gene_symbol = gsub("_"," ",genomics$Relation_to_gene_symbol)

metadata_fileName = "Drug_metadata.txt"
complete_metadata_fileName = paste(metadata_directory,metadata_fileName,sep='')
metadata = read.csv(complete_metadata_fileName,header = TRUE,stringsAsFactors = FALSE,sep='\t')


drugs = unique(genomics$DrugOrDrugGroup)
genomics$Drug_fullName = genomics$DrugOrDrugGroup
indexDrug=1
for (indexDrug in 1:length(drugs))
{#Begin
   drug = drugs[indexDrug]
   indexCurrentDrug = which(genomics$DrugOrDrugGroup==drug)
   if (drug %in% names(full_drugNames))
   { genomics$Drug_fullName[indexCurrentDrug] = full_drugNames[[drug]] }
}#End

filter_stages = unique(genomics$Filter_stage)

indexTKIs = which(metadata$Drug_type %in% c("Kinase_inhibitor","Monoclonal_antibody"))
indexCardiotoxic = which(metadata$Cardiotoxicity_frequencyGroup >= cardiotoxicity_minFrequencyGroup_cutoff)
toxic_drugs = c(metadata$Drug[indexCardiotoxic])
summary_plots = list()
pairwise_plots = list()
relation_colors = c("orange","blue","firebrick","blue","navy","orchid","green","red","cyan","steelblue")
local_drugs_in_drugClass_specific_order = c(drugs_in_drugClass_specific_order)

indexF=1
for (indexF in 1:length(filter_stages))
{#Begin
   filter_stage = filter_stages[indexF]
   indexCurrentFilterStage = which(genomics$Filter_stage==filter_stage)
   filterGenomics = genomics[indexCurrentFilterStage,]
   entityClasses = unique(filterGenomics$Counted_entityClass)
   indexEC=1
   for (indexEC in 1:length(entityClasses))
   {#Begin
      entityClass = entityClasses[indexEC]
      indexCurrentEntityClass = which(filterGenomics$Counted_entityClass==entityClass)
      entityClass_genomics = filterGenomics[indexCurrentEntityClass,]
      indexKeep = which(entityClass_genomics$DrugOrDrugGroup %in% local_drugs_in_drugClass_specific_order)
      entityClass_genomics = entityClass_genomics[indexKeep,]
      entityClass_genomics$DrugOrDrugGroup_factor = factor(entityClass_genomics$DrugOrDrugGroup,levels=local_drugs_in_drugClass_specific_order)
      entityClass_genomics$Relation_to_gene_symbol_factor = factor(entityClass_genomics$Relation_to_gene_symbol,levels=geneSymbolToDrugRelations_in_correct_order)
      entityClass_genomics = entityClass_genomics[order(entityClass_genomics$DrugOrDrugGroup_factor),]
      entityClass_genomics$Drug_fullName_factor = factor(entityClass_genomics$Drug_fullName,levels=unique(entityClass_genomics$Drug_fullName))
      
      {#Begin - Set drug colors
        entityClass_genomics = entityClass_genomics[order(entityClass_genomics$DrugOrDrugGroup_factor),]
        drugs = unique(entityClass_genomics$DrugOrDrugGroup)
        drug_colors = replicate(length(drugs),"black")
        for (indexDrug in 1:length(drugs))
        {#Begin
          drug = drugs[indexDrug]
          if (drug %in% names(drug_drugClass))
          {#Begin
            drugClass = drug_drugClass[[drug]]
            drug_colors[indexDrug] = drugClass_colors[[drugClass]]
          }#End
        }#End
      }#End - Set drug colors
      
      {#Begin - Set relation colors
        entityClass_genomics = entityClass_genomics[order(entityClass_genomics$Relation_to_gene_symbol_factor),]
        relations = unique(entityClass_genomics$Relation_to_gene_symbol)
        relation_colors = replicate(length(drugs),"gray")
        for (indexRelation in 1:length(relations))
        {#Begin
          relation = relations[indexRelation]
          if (relation %in% names(geneSymbolToDrugRelation_color_list))
          {#Begin
            relation_colors[indexRelation] = geneSymbolToDrugRelation_color_list[[relation]]
          }#End
        }#End
      }#End - Set relation colors

      main_title = paste(gsub("_"," ",filter_stage),"\n",entityClass,sep='')
      
      summary_plot = ggplot(entityClass_genomics,aes(x=Drug_fullName_factor,y=Counts,fill=Relation_to_gene_symbol_factor))
      summary_plot = summary_plot + geom_bar(stat="identity")
      summary_plot = summary_plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
      summary_plot = summary_plot + scale_fill_manual(values=relation_colors)
      summary_plot = summary_plot + ggtitle(main_title)
      summary_plot = summary_plot + theme(axis.text.x = element_text(color=drug_colors,size=17,face=2))
      summary_plot = summary_plot + theme(axis.text.y = element_text(size=17,face=2))
      summary_plot = summary_plot + theme(axis.title.y = element_text(size=17,face=2))
      summary_plot = summary_plot + theme(plot.title = element_text(hjust=0.5,face=2))
      summary_plot = summary_plot + theme(legend.text = element_text(size=10))
      summary_plot = summary_plot + theme(legend.title = element_blank())
      summary_plot = summary_plot + theme(axis.title.x = element_blank())
      legend <- cowplot::get_legend(summary_plot)
      #summary_plot = summary_plot + theme(legend.position = "none")
      summary_plots[[length(summary_plots)+1]] = summary_plot
      
      indexToxic = which(entityClass_genomics$DrugOrDrugGroup %in% toxic_drugs)
      if (length(indexToxic)>0)
      {#Begin
          toxic_entitClass_genomics = entityClass_genomics[indexToxic,]
          
          {#Begin - Set drug colors
            toxic_entitClass_genomics = toxic_entitClass_genomics[order(toxic_entitClass_genomics$DrugOrDrugGroup_factor),]
            drugs = unique(toxic_entitClass_genomics$DrugOrDrugGroup)
            drug_colors = replicate(length(drugs),"black")
            for (indexDrug in 1:length(drugs))
            {#Begin
              drug = drugs[indexDrug]
              if (drug %in% names(drug_drugClass))
              {#Begin
                drugClass = drug_drugClass[[drug]]
                drug_colors[indexDrug] = drugClass_colors[[drugClass]]
              }#End
            }#End
          }#End - Set drug colors
    
          summary_plot = ggplot(toxic_entitClass_genomics,aes(x=Drug_fullName_factor,y=Counts,fill=Relation_to_gene_symbol_factor))
          summary_plot = summary_plot + geom_bar(stat="identity")
          summary_plot = summary_plot + theme(axis.text.x = element_text(angle = 90, vjust = 0.5, hjust=1,face=2))
          summary_plot = summary_plot + scale_fill_manual(values=relation_colors)
          summary_plot = summary_plot + ggtitle(main_title)
          summary_plot = summary_plot + theme(axis.text.x = element_text(color=drug_colors,size=17,face=2))
          summary_plot = summary_plot + theme(axis.text.y = element_text(size=17,face=2))
          summary_plot = summary_plot + theme(axis.title.y = element_text(size=17,face=2))
          summary_plot = summary_plot + theme(plot.title = element_text(hjust=0.5,face=2))
          summary_plot = summary_plot + theme(legend.text = element_text(size=10))
          summary_plot = summary_plot + theme(legend.title = element_blank())
          summary_plot = summary_plot + theme(axis.title.x = element_blank())
          legend <- cowplot::get_legend(summary_plot)
          #summary_plot = summary_plot + theme(legend.position = "none")
          summary_plots[[length(summary_plots)+1]] = summary_plot
      }#End
      
      
   }#End
}#End   
   
   
complete_pdf_fileName = gsub(".txt",".pdf",complete_fileName)
pdf(complete_pdf_fileName, width=8.5, height=11)
cols_count = 1
max_plots_per_figure= cols_count * 2
figures_count = ceiling(length(summary_plots)/max_plots_per_figure)

while ((length(summary_plots) %% max_plots_per_figure)!=0)
{#Begin
   summary_plots[[length(summary_plots)+1]] = ggplot() + theme_void()
}#End

for (indexF in 1:figures_count)
{#Begin
   startPlot = (indexF-1)*max_plots_per_figure+1
   endPlot = min(indexF*max_plots_per_figure,length(summary_plots))
   current_plots = summary_plots[startPlot:endPlot]
   length_plot_list = length(current_plots)
   rows_count = ceiling(length_plot_list / cols_count)
   png_width = 3000*cols_count;
   png_height = 500*rows_count;
   do.call("grid.arrange",c(current_plots,nrow=rows_count,ncol=cols_count))
}#End

dev.off()
