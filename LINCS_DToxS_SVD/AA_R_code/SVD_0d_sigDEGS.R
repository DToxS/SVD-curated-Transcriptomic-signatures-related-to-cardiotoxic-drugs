library(beeswarm)
delete_task_reports=TRUE
add_inFrontOf_progress_report_fileName = "SVD_0"
get_eigenassays_per_dataset=FALSE
source('SVD_global_parameter.R')

directory = paste(lincs_results_directory,"//SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue//1_DEGs//",sep='')
fileName = "Deg_summary.txt"
metadata_fileName = "Drug_metadata.txt"
complete_metadata_fileName = paste(metadata_directory,metadata_fileName,sep='')
complete_degs_summary_fileName = paste(directory,fileName,sep='')

## Generate colors - BEGIN
source('SVD_colors.R')
## Generate colors - END

Summary = summary_local

{#Begin - Adjust drugClasses
   for (indexS in 1:length(Summary[,1]))
   {#Begin
      current_drug = Summary$Treatment[indexS]
      Summary$Drug_type[indexS] = drug_drugClass[[current_drug]]
   }#End
}#end


Summary$Drug_type_factor = factor(Summary$Drug_type,levels = drugClassOrder)
Summary = Summary[order(Summary$Drug_type_factor),]
Summary$Treatment_factor=factor(Summary$Treatment,levels=unique(Summary$Treatment))

Summary = Summary[order(Summary$Treatment_factor),]
all_entities = unique(Summary$Treatment)
entity_colors = replicate(length(all_entities),"gray40")
for (indexAllEntities in 1:length(all_entities))
{#Begin
  current_entity = all_entities[indexAllEntities]
  drugClass = drug_drugClass[[current_entity]]
  entity_colors[indexAllEntities] = drugClass_colors[[drugClass]]
}#End

drug_table = table(Summary$Treatment)

Summary$Treatment_full_name = "error"
Colors = replicate(length(Summary[,1]),"gray40")
for (indexRow in 1:length(Summary[,1]))
{#begin
  current_drug = Summary$Treatment[indexRow]
  current_drugClass = Summary$Drug_type[indexRow]
  #indexColor = which(drugs_for_highlight_colors==current_drug)
  #Colors[indexRow] = drugClass_colors[[current_drugClass]]
  drugClass = drug_drugClass[[current_drug]]
  Colors[indexRow] = drugClass_colors[[drugClass]]
  Summary$Treatment_full_name[indexRow] = paste(full_drugNames[[current_drug]]," (",drug_table[[current_drug]],") ",sep='')
}#End


png_fileName = "Sign_degs.png"
complete_png_fileName = paste(directory,png_fileName,sep='')
matrix_png_width = 3500; matrix_png_height = 2000
png(complete_png_fileName,width=matrix_png_width,height=matrix_png_height,res=350);
par(mar=c(10,4,1,1))
beeswarm(Significant_degs_based_on_FDR_count ~ Treatment_factor,data=Summary,las=2,pwcol=Colors,pch=20,xlab="",ylab="",method="swarm",cex.axis=1,xaxt="n")
mtext(unique(Summary$Treatment_full_name),at=1:length(unique(Summary$Treatment_full_name)),col=entity_colors,cex=0.8,las=2,side=1,font=2)
dev.off()


