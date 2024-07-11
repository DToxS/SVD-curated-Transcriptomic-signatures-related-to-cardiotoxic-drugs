library(ggplot2)
library(gridExtra)
library(cowplot)
library(ggvenn)

analysis_names = c("")
get_eigenassays_per_dataset = FALSE
delete_task_reports = FALSE
add_inFrontOf_progress_report_fileName = "SVD_16"
source('SVD_global_parameter.R')
source('SVD_colors.R')
source('Common_tools.R')


topDEGs = 600

datasets = "Enrichment_maxP0.05_top600DEGs"
datasets = c( "DEGenes_iPSCdCMs_ECCoCulture_enrichment_maxP0.05_top600DEGs"
             ,"DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs"
             )

results_directory = paste(lincs_results_directory,"SVD_outlier_responses/",sep='')

ontologies = c("MBCOL1","MBCOL2","MBCOL3","MBCOL4")#,"MBCOL4")

topPredicted_pathways_for_vennDiagram = unique(c(unlist(ontology_topScps_harmfulOrProtective_forPrecsion_list),c(10,5,5)))
topPredicted_pathways_for_vennDiagram = topPredicted_pathways_for_vennDiagram[order(topPredicted_pathways_for_vennDiagram)]

topPredicted_pathways_for_overlapAnalysis_list = list( "MBCOL1" = 5
                                                      ,"MBCOL2" = 5
                                                      ,"MBCOL3" = 10
                                                      ,"MBCOL4" = 5)

drugClass_drugGroupForVennDiagram_list = list("cardiac acting" = "cardiac acting",
                                              "non-c.toxic KI" = "TKI",
                                              "c.toxic mAb" = "TKI",
                                              "c.toxic KI" = "TKI",
                                              "non-c.toxic mAb" = "TKI",
                                              "anthracycline" = "anthracycline")
group_color_list = list("cardiac acting" = "blue",
                        "anthracycline" = "mediumorchid",
                        "TKI" = "orange")
minimum_percent_of_celllines_for_SCP_overlap_vennDiagram = 66
minimum_percent_of_drugs_for_SCP_overlap_vennDiagram = 10

min_percent_celllines_scpOverlaps = 66

max_fractional_rank = 5
max_rank_for_color = 15
overall_entryTypes = c("Diffrna_up","Diffrna_down")
decomposition_statuses_in_right_order = c("complete","no1stSVD","decomposed")#,"decomposed_Q1","decomposed_Q01","decomposed_Q001")"no1stSVD",

entryTypes_in_correct_order = c("Diffrna_up","Diffrna_down")

generate_all_heatmaps_for_each_drug=TRUE
generate_counts_of_overlapping_SCPs=TRUE
large_figures=FALSE

indexDataset=2
for (indexDataset in 1:length(datasets))
{#Begin
   current_dataset = datasets[indexDataset]

   VennDiagrams=list()
   Percent_plots=list()
   Scp_count_plots=list()
   indexO=4
   for (indexO in 1:length(ontologies))
   {#Begin
      ontology = ontologies[indexO]
      indexDecompose=2
      enrich_data = c()
      for (indexDecompose in 1:length(decomposition_statuses_in_right_order))
      {#Begin - Load all enrichment files for the different decomposition statuses
         decomposition_status = decomposition_statuses_in_right_order[indexDecompose]
         current_directory = paste(results_directory,current_dataset,"_in_",decomposition_status,"_rocForFractional_rank/",sep='')
         current_fileName = ontology
         current_complete_fileName = paste(current_directory,current_fileName,".txt",sep='')
         add_data = read.csv(file=current_complete_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
         add_data$Decomposition_status = decomposition_status
         if (length(enrich_data)==0) { enrich_data = add_data }
         else { enrich_data = rbind(enrich_data,add_data) }
      }#End - Load all enrichment files for the different decomposition statuses
      
      {#Begin - Prepare uploaded enrichment for further analysis
         enrich_data$Drug = "error"
         enrich_data$Cell_line = "error"
         enrich_data$Outlier = "error"
         enrich_data$Drug_class = "error"
         indexKeep = which(enrich_data$Sample_entryType %in% overall_entryTypes)
         enrich_data = enrich_data[indexKeep,]
         sampleNames = unique(enrich_data$Sample_name)
         indexS=1
         for (indexS in 1:length(sampleNames))
         {#Begin - Assign drugs and cell lines
             sampleName = sampleNames[indexS]
             stringSplits = strsplit(sampleName,"-")[[1]]
             indexCurrentSampleName = which(enrich_data$Sample_name==sampleName)
             inner_splitStrings = strsplit(stringSplits[1],"_")[[1]]
             enrich_data$Drug[indexCurrentSampleName] = inner_splitStrings[1]
             enrich_data$Drug_class[indexCurrentSampleName] = gsub("non","non-",tolower(gsub("_"," ",stringSplits[length(stringSplits)])))
             enrich_data$Outlier[indexCurrentSampleName] = stringSplits[length(stringSplits)-1]
             cell_line_splitStrings = strsplit(stringSplits[2],"_")[[1]]
             cell_line = ""
             if (length(cell_line_splitStrings)==2)
             { cell_line = cell_line_splitStrings[1] }
             if (length(cell_line_splitStrings)==3)
             { cell_line = paste(cell_line_splitStrings[1],"-",cell_line_splitStrings[3],sep='') }
             if (nchar(cell_line)==0) { stop("Cell line not set")}
             enrich_data$Cell_line[indexCurrentSampleName] = cell_line
         }#End - Assign drugs and cell lines
         enrich_data$Drug_class = gsub("non- cardiovascular drug","not cardiac act.",enrich_data$Drug_class)
         enrich_data$Drug_class = gsub("cardiovascular drug","cardiac acting",enrich_data$Drug_class)
         enrich_data$Drug_class = gsub("kinase inhibitor","KI",enrich_data$Drug_class)
         enrich_data$Drug_class = gsub("monoclonal antibody","mAb",enrich_data$Drug_class)
         enrich_data$Drug_class = gsub("cardiotoxic","c.toxic",enrich_data$Drug_class)
         
         enrich_data$Scp_fullName = enrich_data$Scp
         enrich_data$Scp = Shorten_scp_names(enrich_data$Scp)

         scps = unique(enrich_data$Scp)
         enrich_data$Scp_for_figure = "error"
         indexScp=1
         for (indexScp in 1:length(scps))
         {#Begin
            scp = scps[indexScp]
            indexCurrentScp = which(enrich_data$Scp == scp)
            new_scp = Shorten_scp_names(scp)
            new_scp = Split_name_over_multiple_lines(name=new_scp, max_nchar_per_line=30, max_lines=2)
            enrich_data$Scp_for_figure[indexCurrentScp] = new_scp
            enrich_data$Scp[indexCurrentScp] = scp
         }#End
      }#End - Prepare uploaded enrichment for further analysis
            
      enrich_data$Decomposition_status_factor = factor(enrich_data$Decomposition_status,levels=decomposition_statuses_in_right_order)
      enrich_data = enrich_data[order(enrich_data$Decomposition_status_factor),]
      enrich_data = enrich_data[order(enrich_data$Drug),]
      enrich_drugs = unique(enrich_data$Drug)
      indexInEnrichDrugs = which(drugs_in_drugClass_specific_order %in% enrich_drugs)
      drugs = drugs_in_drugClass_specific_order[indexInEnrichDrugs]
      indexMissing = which(!enrich_drugs %in% drugs_in_drugClass_specific_order)
      if (length(indexMissing)>0) { rm(enrich_data) }
      enrich_data$Drug_decomposition_status = paste(enrich_data$Drug,"-",enrich_data$Decomposition_status,sep='')
      indexDrug=1

      if (generate_all_heatmaps_for_each_drug)
      {#Begin - if (generate_all_heatmaps_for_each_drug)
      complete_pdf_fileName = paste(results_directory,current_dataset,"_",ontology,".pdf",sep='')
      pdf(complete_pdf_fileName, width=8.5, height=11);
      for (indexDrug in 1:length(drugs))
      {#Begin - Generate, arrange and write Heatmaps for each drug
         Heatmaps = list()
         drug = drugs[indexDrug]
         fullDrugName = full_drugNames[[drug]]
         indexCurrentDrug = which(enrich_data$Drug==drug)
         drug_enrich_data = enrich_data[indexCurrentDrug,]
         drugClass = unique(drug_enrich_data$Drug_class)
         indexEntryType = 2
         plots_added=0
         indexDecom=1
         for (indexDecom in 1:length(decomposition_statuses_in_right_order))
         {#Begin
            current_decomposition_status = decomposition_statuses_in_right_order[indexDecom]
            indexCurrentDecom = which(drug_enrich_data$Decomposition_status==current_decomposition_status)
            decom_enrich_data = drug_enrich_data[indexCurrentDecom,]
            indexEntryType=1
            for (indexEntryType in 1:length(overall_entryTypes))
            {#Begin
               entryType = overall_entryTypes[indexEntryType]
               indexCurrentEntryType = which(decom_enrich_data$Sample_entryType==entryType)
               if (length(indexCurrentEntryType)==0) 
               {#Begin
                  Heatmap = ggplot()
                  Heatmap = Heatmap + geom_blank()
                  #Heatmap = Heatmap + ggtitle(figure_title)
                  Heatmap = Heatmap + theme(title = element_text(size=15,angle=0,hjust=0.5,vjust=0))
                  Heatmaps[[length(Heatmaps)+1]] = Heatmap
               }#End
               else
               {#Begin
                  entryType_enrich_data = decom_enrich_data[indexCurrentEntryType,]
                  #plot_datasets = unique(entryType_enrich_data$Dataset)
                  #indexDataset = 1
                  #for (indexDataset in 1:length(plot_datasets))
                  {#Begin
                     #plot_dataset = plot_datasets[indexDataset]
                     #indexCurrentDataset = which(entryType_enrich_data$Dataset==plot_dataset)
                     dataset_enrich_data = entryType_enrich_data
                     indexBelowRank = which(dataset_enrich_data$Fractional_rank<=max_fractional_rank)
                     if (length(indexBelowRank)==0)
                     {#Begin
                       Heatmap = ggplot()
                       Heatmap = Heatmap + geom_blank()
                       #Heatmap = Heatmap + ggtitle(figure_title)
                       Heatmap = Heatmap + theme(title = element_text(size=15,angle=0,hjust=0.5,vjust=0))
                       Heatmaps[[length(Heatmaps)+1]] = Heatmap
                     }#End
                     else
                     {#Begin
                       scps = unique(dataset_enrich_data$Scp[indexBelowRank])
                       indexKeep = which(dataset_enrich_data$Scp %in% scps)
                       dataset_enrich_data = dataset_enrich_data[indexKeep,]
     
                       dataset_enrich_data$Fractional_rank_string = as.character(round(dataset_enrich_data$Fractional_rank))
                       #dataset_enrich_data$Fractional_rank_string = gsub("[.]0","",dataset_enrich_data$Fractional_rank_string)
                       indexLarger99 = which(dataset_enrich_data$Fractional_rank>99)
                       if (length(indexLarger99)>0) { dataset_enrich_data$Fractional_rank_string[indexLarger99] = ">" }
                       #indexLarger9 = which(dataset_enrich_data$Fractional_rank>9)
                       #dataset_enrich_data$Fractional_rank_string = as.character(round(dataset_enrich_data$Fractional_rank))
                          
                       {#Begin - Set scp order and add missing scps
                          Col_names = c("SCP","Average_rank")
                          Col_length = length(Col_names)
                          Row_names = 1
                          Row_length = length(Row_names)
                          base_scp_line = as.data.frame(array(NA,c(Row_length,Col_length),dimnames = list(Row_names,Col_names)))
                          base_scp_lines = c()
                             
                          scps = unique(dataset_enrich_data$Scp)
                          current_cellines = unique(dataset_enrich_data$Cell_line)
                          length_celllines = length(current_cellines)
                          max_rank = max(dataset_enrich_data$Fractional_rank) + 1
                          scps_in_right_order = c()
                          final_dataset_enrich = c()
                          for (indexS in 1:length(scps))
                          {#Begin
                             scp = scps[indexS]
                             indexCurrentScp = which(dataset_enrich_data$Scp==scp)
                             scp_dataset_enrich = dataset_enrich_data[indexCurrentScp,]
                             missing_celllines = current_cellines[which(!current_cellines %in% scp_dataset_enrich$Cell_line)]
                             add_scp_dataset_enrich = c()
                             if (length(missing_celllines)>0)
                             {#Begin
                                for (indexMC in 1:length(missing_celllines))
                                {#Begin
                                   missing_cellline = missing_celllines[indexMC]
                                   new_dataset_enrich_line = scp_dataset_enrich[1,]
                                   new_dataset_enrich_line$Cell_line = missing_cellline
                                   new_dataset_enrich_line$Pvalue = 0;
                                   new_dataset_enrich_line$Minus_log10_pvalue=0
                                   new_dataset_enrich_line$Overlap_count=0
                                   new_dataset_enrich_line$Fractional_rank = 100
                                   new_dataset_enrich_line$Fractional_rank_string = "-"
                                   if (length(add_scp_dataset_enrich)==0) { add_scp_dataset_enrich = new_dataset_enrich_line }
                                   else { add_scp_dataset_enrich = rbind(add_scp_dataset_enrich,new_dataset_enrich_line) }
                                }#End
                             }#End
                             scp_dataset_enrich = rbind(scp_dataset_enrich,add_scp_dataset_enrich)
                             current_ranks = scp_dataset_enrich$Fractional_rank
                             if (length(final_dataset_enrich)==0) { final_dataset_enrich = scp_dataset_enrich }
                             else { final_dataset_enrich = rbind(final_dataset_enrich,scp_dataset_enrich) }
                                
                             base_scp_line$SCP = scp
                             base_scp_line$Average_rank = mean(current_ranks)
                             if (length(base_scp_lines)==0) { base_scp_lines = base_scp_line }
                             else { base_scp_lines = rbind(base_scp_lines,base_scp_line)}
                          }#End
                          base_scp_lines = base_scp_lines[order(base_scp_lines$Average_rank,decreasing=TRUE),]
                          scps_in_correct_order = base_scp_lines$SCP
                          dataset_enrich_data = final_dataset_enrich
                       }#End - Set scp order and add missing scps
                          
                       dataset_enrich_data$Scp_factor = factor(dataset_enrich_data$Scp,levels=scps_in_correct_order)
                       dataset_enrich_data = dataset_enrich_data[order(dataset_enrich_data$Scp_factor),]   
                       scp_for_figure_in_correct_order = unique(dataset_enrich_data$Scp_for_figure)
                       dataset_enrich_data$Scp_for_figure_factor = factor(dataset_enrich_data$Scp_for_figure,levels=scp_for_figure_in_correct_order)
                       
                       scps_count = length(unique(dataset_enrich_data$Scp))
                       
                       if (scps_count>19)
                       {#Begin
                          dataset_enrich_data$Scp_for_figure_factor = factor(dataset_enrich_data$Scp,levels=scps_in_correct_order)
                          scp_text_size = 6
                       }#End
                       if ((scps_count<=19)&(scps_count>15))
                       {#Begin
                          dataset_enrich_data$Scp_for_figure_factor = factor(dataset_enrich_data$Scp,levels=scps_in_correct_order)
                          scp_text_size = 6
                       }#End
                       if ((scps_count<=15)&(scps_count>11))
                       {#Begin
                          dataset_enrich_data$Scp_for_figure_factor = factor(dataset_enrich_data$Scp,levels=scps_in_correct_order)
                          scp_text_size = 7
                       }#End
                       if ((scps_count<=11)&(scps_count>8))
                       {#Begin
                          dataset_enrich_data$Scp_for_figure_factor = factor(dataset_enrich_data$Scp,levels=scps_in_correct_order)
                          scp_text_size = 8
                       }#End
                       if ((scps_count<=8)&(scps_count>7))
                       {#Begin
                          dataset_enrich_data$Scp_for_figure_factor = factor(dataset_enrich_data$Scp,levels=scps_in_correct_order)
                          scp_text_size = 8
                       }#End
                       if ((scps_count<=7))
                       {#Begin
                          scp_text_size = 10
                       }#End
                       
                       if (entryType=="Diffrna_up")
                       {#Begin
                          high_color = "orange3"
                          low_color = "yellow"
                          na_color = "tan4"
                          text_color = "black"
                       }#End
                       if (entryType=="Diffrna_down")
                       {#Begin
                          high_color = "dodgerblue3"
                          low_color = "lightskyblue"
                          na_color = "royalblue4"
                          text_color = "black"
                       }#End
                       
                       {#Begin - Set color for regular and outlier cell lines
                          dataset_enrich_data = dataset_enrich_data[order(dataset_enrich_data$Cell_line),]
                          celllines = unique(dataset_enrich_data$Cell_line)
                          cellline_color = replicate(length(celllines),"black")
                          indexOutlier = which(dataset_enrich_data$Outlier=="O")
                          outlier_cellline = unique(dataset_enrich_data$Cell_line[indexOutlier])
                          indexOutlierCellline = which(celllines==outlier_cellline)
                          cellline_color[indexOutlierCellline] = "mediumorchid"
                       }#End - Set color for regular and outlier cell lines
                             
                       Heatmap = ggplot(dataset_enrich_data, aes(x=Cell_line,y=Scp_for_figure_factor,fill=Fractional_rank))
                       Heatmap = Heatmap + geom_tile()

                       Heatmap = Heatmap + geom_text(aes(label=Fractional_rank_string),size=3,angle=0,color=text_color)  # for pdf  
                       Heatmap = Heatmap + scale_fill_gradient(low=low_color, high=high_color,limits=c(1,max_rank_for_color),na.value=na_color)
                       Heatmap = Heatmap + theme(legend.position = "none")
                       Heatmap = Heatmap + scale_x_discrete(position = "top")
                       Heatmap = Heatmap + theme(axis.title = element_blank())

                       
                       Heatmap = Heatmap + theme(axis.text.x = element_text(size=7,angle=90,hjust=0,vjust=2,face=2,color = cellline_color)) #for pdf
                       Heatmap = Heatmap + theme(axis.text.y = element_text(size=scp_text_size,angle=0,hjust=1,vjust=0.5)) #for pdf
                       Heatmaps[[length(Heatmaps)+1]] = Heatmap
                       plots_added = plots_added + 1                  
                     }#End
                  }#End
              }#End
            }#End
         }#End
         length_plots = length(Heatmaps)
         max_plot_per_figure = length(overall_entryTypes) * length(decomposition_statuses_in_right_order);
         figures_count = ceiling(length_plots / max_plot_per_figure)
         indexF=1
         for (indexF in 1:figures_count)
         {#Begin
            indexStart = (indexF-1)* max_plot_per_figure + 1
            indexEnd = min(c((indexF) * max_plot_per_figure,length_plots))
            current_plots = list()
            for (indexCurrentPlot in indexStart:indexEnd)
            {#Begin
               current_plots[[length(current_plots)+1]] = Heatmaps[[indexCurrentPlot]]
            }#End
            rows_count=3
            cols_count=2
            prefinal_figure = plot_grid(current_plots[[1]],current_plots[[2]],current_plots[[3]],current_plots[[4]],current_plots[[5]],current_plots[[6]],
                                        nrow=rows_count,ncol=cols_count,align="v")
               
            figure_title = paste(fullDrugName," (",drugClass,")",sep='')
            empty_plot = ggplot() + theme_void()
            headline = empty_plot + scale_y_continuous(limits=c(-1,1)) + scale_x_continuous(limits=c(-1,1))
            headline = headline + annotate("text",y=0.6, x=0.1, fontface=2,label=ontology,size=6)
            headline = headline + annotate("text",y=-0.1, x=0.1, fontface=2,label=fullDrugName,size=8)
            headline = headline + annotate("text",y=-0.8, x=0.1, fontface=2,label=paste("(",drugClass,")",sep=''),size=5)
            row_name1 = empty_plot + scale_y_continuous(limits=c(-1,1)) + scale_x_continuous(limits=c(-1,1))
            row_name1 = row_name1 + annotate("text",y=0, x=0, fontface=2,label=decomposition_statuses_in_right_order[1],size=6,angle=90)
            row_name2 = empty_plot + scale_y_continuous(limits=c(-1,1)) + scale_x_continuous(limits=c(-1,1))
            row_name2 = row_name2 + annotate("text",y=0, x=0, fontface=2,label=decomposition_statuses_in_right_order[2],size=6,angle=90)
            row_name3 = empty_plot + scale_y_continuous(limits=c(-1,1)) + scale_x_continuous(limits=c(-1,1))
            row_name3 = row_name3 + annotate("text",y=0, x=0, fontface=2,label=decomposition_statuses_in_right_order[3],size=6,angle=90)
            col_name1 = empty_plot + scale_y_continuous(limits=c(-1,1)) + scale_x_continuous(limits=c(-1,1))
            col_name1 = col_name1 + annotate("text",y=0, x=0.5, fontface=2,label="Upregulated",size=6,angle=0)
            col_name2 = empty_plot + scale_y_continuous(limits=c(-1,1)) + scale_x_continuous(limits=c(-1,1))
            col_name2 = col_name2 + annotate("text",y=0, x=0.5, fontface=2,label="Downregulated",size=6,angle=0)
            empty_plot = ggplot() + theme_void()
               
            Layout_matrix = rbind(c(1,2,2,2,2,2,2,2,2,2,2,2,2),
                                  c(3,7,7,7,7,7,7,8,8,8,8,8,8),
                                  c(3,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(3,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(3,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(3,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(4,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(4,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(4,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(4,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(5,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(5,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(5,6,6,6,6,6,6,6,6,6,6,6,6),
                                  c(5,6,6,6,6,6,6,6,6,6,6,6,6))
   
            grid_figures = list(empty_plot,headline,row_name1,row_name2,row_name3,prefinal_figure,col_name1,col_name2)
            fileName = paste(current_dataset,"_",ontology,"_",strsplit(drug,"-")[[1]][1],".png",sep='')
            complete_png_fileName = paste(results_directory,fileName,sep='')
            cols_count = min(length(overall_entryTypes),length(current_plots))
            rows_count = ceiling(length(current_plots)/cols_count)
            #png(complete_png_fileName,width=700*cols_count,height=400*rows_count,res=75);
            #print(final_figure)
            grid.arrange(grobs=grid_figures,layout_matrix=Layout_matrix)
            #plot_grid",c(current_plots,nrow=rows_count,ncol=cols_count,align="v"))
         }#End
      }#End - Generate, arrange and write Heatmaps for each drug
      dev.off()
      }#End - if (generate_all_heatmaps_for_each_drug)
        
      if (generate_counts_of_overlapping_SCPs)
      {#Begin - if (generate_counts_of_overlapping_SCPs)
         Col_names = c("Decomposition_status","Drug","Drug_cellinesCount","SCP","Percent_of_celllines","Count_above_minimum_percentage")
         Col_length = length(Col_names)
         Row_names = ""
         Row_length = length(Row_names)
         scp_percentCellline_ofDrug_base_line = as.data.frame(array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names)))
 
         entryTypes = unique(enrich_data$Sample_entryType)
         entryTypes = entryTypes_in_correct_order
         indexET = 1
         for (indexET in 1:length(entryTypes))
         {#Begin
            entryType = entryTypes[indexET]
            indexCurrentEntryType = which(enrich_data$Sample_entryType==entryType)
            entryType_data = enrich_data[indexCurrentEntryType,]
            indexDecomp=1
            scp_percentCelllines = c()
            for (indexDecomp in 1:length(decomposition_statuses_in_right_order))
            {#Begin
               decomposition_status = decomposition_statuses_in_right_order[indexDecomp]
               indexCurrentDecomp = which(entryType_data$Decomposition_status==decomposition_status)
               decomp_enrich_data = entryType_data[indexCurrentDecomp,]
            
               indexDrug=1
               max_rank = topPredicted_pathways_for_overlapAnalysis_list[ontology]
               for (indexDrug in 1:length(drugs_in_drugClass_specific_order))
               {#Begin
                  current_drug = drugs_in_drugClass_specific_order[indexDrug]
                  indexCurrentDrug = which(decomp_enrich_data$Drug==current_drug)
                  if (length(indexCurrentDrug)>0)
                  {#Begin
                     drug_enrich = decomp_enrich_data[indexCurrentDrug,]
                     treated_cell_lines_length = length(unique(drug_enrich$Cell_line))
                     indexBelowFractionalRank = which(drug_enrich$Fractional_rank <= max_rank)
                     if (length(indexBelowFractionalRank)==0)
                     {#Begin
                       #shared begin
                       new_scp_percentCellline_ofDrug_line = scp_percentCellline_ofDrug_base_line
                       new_scp_percentCellline_ofDrug_line$SCP = scp
                       new_scp_percentCellline_ofDrug_line$Drug = current_drug
                       new_scp_percentCellline_ofDrug_line$Drug_cellinesCount = paste(current_drug, "(",ceiling(min_percent_celllines_scpOverlaps/100 * treated_cell_lines_length),"/",treated_cell_lines_length,")",sep='')
                       new_scp_percentCellline_ofDrug_line$Decomposition_status = decomposition_status
                       #shared end

                       new_scp_percentCellline_ofDrug_line$Percent_of_celllines = 0
                     
                       #shared begin
                       if (length(scp_percentCelllines)>0) { scp_percentCelllines = rbind(scp_percentCelllines,new_scp_percentCellline_ofDrug_line) }
                       else { scp_percentCelllines = new_scp_percentCellline_ofDrug_line }
                       #shared end
                     }#End
                     else
                     {#Begin
                       keep_drug_enrich = drug_enrich[indexBelowFractionalRank,]
                       scps = unique(keep_drug_enrich$Scp)
                       indexScp=1
                       for (indexScp in 1:length(scps))
                       {#Begin
                          scp = scps[indexScp]
                          indexCurrentScp = which(keep_drug_enrich$Scp==scp)
                          scp_cell_lines_length = length(unique(keep_drug_enrich$Cell_line[indexCurrentScp]))
                          #shared begin
                          new_scp_percentCellline_ofDrug_line = scp_percentCellline_ofDrug_base_line
                          new_scp_percentCellline_ofDrug_line$SCP = scp
                          new_scp_percentCellline_ofDrug_line$Drug = current_drug
                          new_scp_percentCellline_ofDrug_line$Drug_cellinesCount = paste(current_drug, " (",ceiling(min_percent_celllines_scpOverlaps/100 * treated_cell_lines_length),"/",treated_cell_lines_length,")",sep='')
                          new_scp_percentCellline_ofDrug_line$Decomposition_status = decomposition_status
                          #shared end

                          new_scp_percentCellline_ofDrug_line$Percent_of_celllines = 100 * scp_cell_lines_length/treated_cell_lines_length

                          #shared begin
                          if (length(scp_percentCelllines)>0) { scp_percentCelllines = rbind(scp_percentCelllines,new_scp_percentCellline_ofDrug_line) }
                          else { scp_percentCelllines = new_scp_percentCellline_ofDrug_line }
                          #shared end
                       }#End
                     }#End
                  }#End
               }#End
            }#End
            
            indexDecomp=1
            indexCount =  which(scp_percentCelllines$Percent_of_celllines>=min_percent_celllines_scpOverlaps)
            scp_percentCelllines$Count_above_minimum_percentage = 0
            scp_percentCelllines$Count_above_minimum_percentage[indexCount] = 1
            scp_percentCelllines$Drug_decomposition = paste(scp_percentCelllines$Decomposition_status,"-",scp_percentCelllines$Drug,sep='')
            drug_decompositions = unique(scp_percentCelllines$Drug_decomposition)
            max_scp_count=0
            scps_count_list = list()
            for (indexDD in 1:length(drug_decompositions))
            {#Begin
               drug_decomposition = drug_decompositions[indexDD]
               indexCurrentDD = which(scp_percentCelllines$Drug_decomposition==drug_decomposition)
               decomposition_status = unique(scp_percentCelllines$Decomposition_status[indexCurrentDD])
               current_scp_count = sum(scp_percentCelllines$Count_above_minimum_percentage[indexCurrentDD])
               if (decomposition_status %in% names(scps_count_list)) { scps_count_list[[decomposition_status]] = c(scps_count_list[[decomposition_status]],current_scp_count) }
               else { scps_count_list[[decomposition_status]] = current_scp_count }
               if (current_scp_count>max_scp_count) { max_scp_count = current_scp_count }
            }#End

            for (indexDecomp in 1:length(decomposition_statuses_in_right_order))
            {#Begin
               decomposition_status = decomposition_statuses_in_right_order[indexDecomp]
               indexDecomp = which(scp_percentCelllines$Decomposition_status==decomposition_status)
               decomp_scp_percentCelllines = scp_percentCelllines[indexDecomp,]
            
              drugs = unique(decomp_scp_percentCelllines$Drug)
              drug_medians = replicate(length(drugs),-1)
              for (indexDrug in 1:length(drugs))
              {#Begin
                drug = drugs[indexDrug]
                indexCurrentDrug = which(decomp_scp_percentCelllines$Drug==drug)
                drug_medians[indexDrug] = median(decomp_scp_percentCelllines$Percent_of_celllines[indexCurrentDrug])
              }#End
              if (length(which(drug_medians==-1))!=0) { rm(drug_medians) }
              
              decomp_scp_percentCelllines$Drug_factor = factor(decomp_scp_percentCelllines$Drug, levels=drugs_in_drugClass_specific_order)
              decomp_scp_percentCelllines = decomp_scp_percentCelllines[order(decomp_scp_percentCelllines$Drug_factor),]
              drugs = unique(decomp_scp_percentCelllines$Drug)
              drug_colors = replicate(length(drugs),"gray")
              indexDrug=1
              for (indexDrug in 1:length(drugs))
              {#Begin
                drug = drugs[indexDrug]
                drugClass = drug_drugClass[[drug]]
                drug_colors[indexDrug] = drugClass_colors[[drugClass]]
              }#End
              
              headline = paste(entryType,": ",ontology," SCPs identified in % of treated cells\nin ",decomposition_status," (max rank: ",max_rank,")",sep='')
              
              percent_median = median(drug_medians)
              if (entryType=="Diffrna_up")
              {#Begin
                y_label = "% cell lines with upregulated SCP"
              }#End
              if (entryType=="Diffrna_down")
              {#Begin
                y_label = "% cell lines with downregulated SCP"
              }#End

              decomp_scp_percentCelllines = decomp_scp_percentCelllines[order(decomp_scp_percentCelllines$Drug_factor),]
              decomp_scp_percentCelllines$Drug_cellinesCount_factor = factor(decomp_scp_percentCelllines$Drug_cellinesCount,levels=unique(decomp_scp_percentCelllines$Drug_cellinesCount))
                            
              Percent_plot = ggplot(decomp_scp_percentCelllines,aes(x=Drug_factor,y=Percent_of_celllines,fill=Drug_factor,color=Drug_factor))
              Percent_plot = Percent_plot + geom_boxplot(color="black")#color=drug_colors)
              Percent_plot = Percent_plot + scale_fill_manual(values="gray45")
              Percent_plot = Percent_plot + theme(legend.position = "none")
              Percent_plot = Percent_plot + ggtitle(headline)
              Percent_plot = Percent_plot + geom_hline(yintercept=percent_median,color="black")
              Percent_plot = Percent_plot + xlab("")
              Percent_plot = Percent_plot + ylab(y_label)
              Percent_plot = Percent_plot + theme(axis.text.x = element_text(size=10,angle=90,hjust=1,vjust=0.5,face=2,color = drug_colors)) #for pdf
              Percent_plot = Percent_plot + theme(axis.text.y = element_text(size=10,angle=0,hjust=1,vjust=0.5)) #for pdf
              Percent_plot = Percent_plot + theme(plot.title = element_text(size=20,hjust=0.5)) #for pdf
              Percent_plots[[length(Percent_plots)+1]] = Percent_plot
              
              if (entryType=="Diffrna_up")
              {#Begin
                direction_of_change = "Upregulated"
              }#End
              if (entryType=="Diffrna_down")
              {#Begin
                direction_of_change = "Downregulated"
              }#End
              
              if (decomposition_status=="complete")
              { decomposition_label = "Complete data"}
              if (decomposition_status=="no1stSVD")
              { decomposition_label = "After removal of 1st EA" }
              if (decomposition_status=="decomposed")
              { decomposition_label = "Drug-selective expression profiles"}
              
              
              decomp_scp_percentCelllines = decomp_scp_percentCelllines[order(decomp_scp_percentCelllines$Drug_factor),]
              lineColors = replicate(length(decomp_scp_percentCelllines[,1]),"black")
              indexLineColor=1
              for (indexLineColor in 1:length(decomp_scp_percentCelllines[,1]))
              {#Begin
                drugClass = drug_drugClass[[decomp_scp_percentCelllines$Drug[indexLineColor]]]
                lineColors[indexLineColor] = drugClass_colors[[drugClass]]
              }#End
              
              y_label = paste("# of overlapping\n",tolower(direction_of_change)," ",gsub("MBCOL","level-",ontology)," SCPs",sep='')
              scpCount_median = median(scps_count_list[[decomposition_status]])
              
              regular_color="gray45"
              
              headline = paste(direction_of_change," ",ontology," SCPs\n",decomposition_label,sep='')
              Scp_count_plot = ggplot(decomp_scp_percentCelllines,aes(x=Drug_cellinesCount_factor,y=Count_above_minimum_percentage))
              if (large_figures)
              {#Begin
                 Scp_count_plot = Scp_count_plot + geom_bar(stat="identity",width=0.8,fill=regular_color,color=regular_color)
                 Scp_count_plot = Scp_count_plot + theme(axis.text.x = element_text(size=11,angle=90,hjust=1,vjust=0.5,face=2,color = drug_colors)) #for pdf
                 Scp_count_plot = Scp_count_plot + theme(axis.text.y = element_text(size=14,angle=0,hjust=1,vjust=0.5)) #for pdf
                 Scp_count_plot = Scp_count_plot + theme(axis.title.y = element_text(size=14,angle=90)) #for pdf
                 Scp_count_plot = Scp_count_plot + theme(plot.title = element_text(size=20,hjust=0.5)) #for pdf
                 Scp_count_plot = Scp_count_plot + geom_hline(yintercept =scpCount_median,color="dodgerblue",size=1)
              }#End
              else
              {#Begin
                Scp_count_plot = Scp_count_plot + geom_bar(stat="identity",width=0.7,fill=regular_color,color=regular_color)
                Scp_count_plot = Scp_count_plot + theme(axis.text.x = element_text(size=5,angle=90,hjust=1,vjust=0.5,face=2,color = drug_colors)) #for pdf
                Scp_count_plot = Scp_count_plot + theme(axis.text.y = element_text(size=7,angle=0,hjust=1,vjust=0.5)) #for pdf
                Scp_count_plot = Scp_count_plot + theme(axis.title.y = element_text(size=7,angle=90)) #for pdf
                Scp_count_plot = Scp_count_plot + theme(plot.title = element_text(size=10,hjust=0.5)) #for pdf
                Scp_count_plot = Scp_count_plot + geom_hline(yintercept =scpCount_median,color="dodgerblue",size=0.6)
              }#End
              Scp_count_plot = Scp_count_plot + theme(legend.position = "none")
              Scp_count_plot = Scp_count_plot + ggtitle(headline)
              Scp_count_plot = Scp_count_plot + ylim(0,max_scp_count)
              Scp_count_plot = Scp_count_plot + xlab("")
              Scp_count_plot = Scp_count_plot + ylab(y_label)
              Scp_count_plots[[length(Scp_count_plots)+1]] = Scp_count_plot
            }#End
         }#End
      }#End - if (generate_counts_of_overlapping_SCPs)
      
   }#End
   
   if (generate_counts_of_overlapping_SCPs)
   {#Begin
     if (large_figures) 
     {#Begin
        complete_pdf_fileName = paste(results_directory,"Overlapping_scp_counts_", current_dataset,"_largeFigures.pdf",sep='')
        Generate_plots(Scp_count_plots,complete_pdf_fileName,3,1)
     }#End
     else 
     {#Begin
        complete_pdf_fileName = paste(results_directory,"Overlapping_scp_counts_", current_dataset,"_smallFigures.pdf",sep='')
        resorted_scp_count_plots = list()
        old_new_indexes = c(  1, 3, 5, 2, 4, 6,
                              7, 9,11, 8,10,12,
                             13,15,17,14,16,18,
                             19,21,23,20,22,24)
        for (indexOld in 1:length(Scp_count_plots))
        {#Begin
          indexNew = old_new_indexes[indexOld]
          resorted_scp_count_plots[[indexNew]] = Scp_count_plots[[indexOld]]
        }#End
        Generate_plots(resorted_scp_count_plots,complete_pdf_fileName,6,2)
     }#End
   }#End
}#End

{#Begin - Show WNT siganling upregulated by anthracyclines

fileName = "MBCOL3.txt"
directory = paste(results_directory,"DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/",sep='')
complete_fileName = paste(directory,fileName,sep='')

enrich_data = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')

{#Begin - Prepare uploaded enrichment for further analysis
  enrich_data$Drug = "error"
  enrich_data$Cell_line = "error"
  enrich_data$Outlier = "error"
  enrich_data$Drug_class = "error"
  indexKeep = which(enrich_data$Sample_entryType %in% overall_entryTypes)
  enrich_data = enrich_data[indexKeep,]
  sampleNames = unique(enrich_data$Sample_name)
  indexS=1
  for (indexS in 1:length(sampleNames))
  {#Begin - Assign drugs and cell lines
    sampleName = sampleNames[indexS]
    stringSplits = strsplit(sampleName,"-")[[1]]
    indexCurrentSampleName = which(enrich_data$Sample_name==sampleName)
    inner_splitStrings = strsplit(stringSplits[1],"_")[[1]]
    enrich_data$Drug[indexCurrentSampleName] = inner_splitStrings[1]
    enrich_data$Drug_class[indexCurrentSampleName] = gsub("non","non-",tolower(gsub("_"," ",stringSplits[length(stringSplits)])))
    enrich_data$Outlier[indexCurrentSampleName] = stringSplits[length(stringSplits)-1]
    cell_line_splitStrings = strsplit(stringSplits[2],"_")[[1]]
    cell_line = ""
    if (length(cell_line_splitStrings)==2)
    { cell_line = cell_line_splitStrings[1] }
    if (length(cell_line_splitStrings)==3)
    { cell_line = paste(cell_line_splitStrings[1],"-",cell_line_splitStrings[3],sep='') }
    if (nchar(cell_line)==0) { stop("Cell line not set")}
    enrich_data$Cell_line[indexCurrentSampleName] = cell_line
  }#End - Assign drugs and cell lines
  enrich_data$Drug_class = gsub("non- cardiovascular drug","not cardiac act.",enrich_data$Drug_class)
  enrich_data$Drug_class = gsub("cardiovascular drug","cardiac acting",enrich_data$Drug_class)
  enrich_data$Drug_class = gsub("kinase inhibitor","KI",enrich_data$Drug_class)
  enrich_data$Drug_class = gsub("monoclonal antibody","mAb",enrich_data$Drug_class)
  enrich_data$Drug_class = gsub("cardiotoxic","c.toxic",enrich_data$Drug_class)
  
  enrich_data$Scp_fullName = enrich_data$Scp
  enrich_data$Scp = Shorten_scp_names(enrich_data$Scp)
  
  scps = unique(enrich_data$Scp)
  enrich_data$Scp_for_figure = "error"
  indexScp=1
  for (indexScp in 1:length(scps))
  {#Begin
    scp = scps[indexScp]
    indexCurrentScp = which(enrich_data$Scp == scp)
    new_scp = Shorten_scp_names(scp)
    new_scp = Split_name_over_multiple_lines(name=new_scp, max_nchar_per_line=30, max_lines=2)
    enrich_data$Scp_for_figure[indexCurrentScp] = new_scp
    enrich_data$Scp[indexCurrentScp] = scp
  }#End
}#End - Prepare uploaded enrichment for further anlysis

indexDAU = which(enrich_data$Drug %in% c("DAU"))
indexDOX = which(enrich_data$Drug %in% c("DOX"))
indexWTN = which(enrich_data$Scp=="WNT-Beta-catenin signaling pathway")
enrich_data$Drug[indexDAU] = drug_shortenedFullNames[["DAU"]]
enrich_data$Drug[indexDOX] = drug_shortenedFullNames[["DOX"]]
indexKeep = indexWTN[indexWTN %in% c(indexDAU,indexDOX)]
enrich_data = enrich_data[indexKeep,]
indexUp = which(enrich_data$Sample_entryType=="Diffrna_up")
enrich_data = enrich_data[indexUp,]
enrich_data$Fractional_rank_string = as.character(round(enrich_data$Fractional_rank))
indexLarger99 = which(enrich_data$Fractional_rank>99)
if (length(indexLarger99)>0) { enrich_data$Fractional_rank_string[indexLarger99] = ">" }



high_color = "orange3"
low_color = "yellow"
na_color = "tan4"
text_color = "black"

drug_text_size=15
cellline_text_size = 15
geom_tile_text_size = 5
text_color = "black"
cellline_color = c("black","black","black","black","black","darkorchid")

enrich_data = enrich_data[order(enrich_data$Drug),]
enrich_data$Drug = factor(enrich_data$Drug, levels = unique(enrich_data$Drug))

Heatmap = ggplot(enrich_data, aes(x=Cell_line,y=Drug,fill=Fractional_rank))
Heatmap = Heatmap + geom_tile()
Heatmap = Heatmap + geom_text(aes(label=Fractional_rank_string),size=geom_tile_text_size,face=2,angle=0,color=text_color)  # for pdf  
Heatmap = Heatmap + scale_fill_gradient(low=low_color, high=high_color,limits=c(1,max_rank_for_color),na.value=na_color)
Heatmap = Heatmap + theme(legend.position = "none")
Heatmap = Heatmap + scale_x_discrete(position = "top")
Heatmap = Heatmap + theme(axis.title = element_blank())
Heatmap = Heatmap + theme(axis.text.x = element_text(size=cellline_text_size,angle=90,hjust=0,vjust=2,face=2,color = cellline_color)) #for pdf
Heatmap = Heatmap + theme(axis.text.y = element_text(size=drug_text_size,angle=0,hjust=1,vjust=0.5,color="black",face=2)) #for pdf

Heatmaps = list(Heatmap)

complete_pdf_name = paste(results_directory,"Anthracycline_WNT_signaling.pdf",sep='')
Generate_plots(Heatmaps,complete_pdf_name,9,3)

}#End - Show WNT signaling upregulated by anthracyclines
