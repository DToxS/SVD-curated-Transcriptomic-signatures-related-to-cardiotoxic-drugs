rm(list = ls());
library(tools);
library("gplots");
library("ggplot2");
library("beeswarm")
library("gridExtra")
library("scales")
get_eigenassays_per_dataset = FALSE
indexCore = -1
add_inFrontOf_progress_report_fileName = "SVD_6"
delete_task_reports = TRUE
f1_score_weight=-1
source('SVD_global_parameter.R')
source('SVD_colors.R')

cores_count = 20;

Col_names = c("File_name","Overall_directory","Complete_directory","Complete_fileName")
Col_length = length(Col_names);
Row_names = 1;
Row_length = length(Row_names)
core_tasks_base_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
core_tasks_base_line = as.data.frame(core_tasks_base_line)
core_tasks = c()

max_figures_per_page = c(6)
max_fractional_ranks = c(5)
distances_from_left_edge = c(50)

indexOverall=1


enrichment_file_extension = ".txt";
filtered_enrichment_label_plus_extension = paste("filtered", enrichment_file_extension, sep = '');
iPSCd_CM_schaniel_label = "schaniel"
pdf_file_extension = ".pdf";

all_fileNames_in_directory = list.files(scSnRnaSeq_enrichment_directory);
indexF=2
for (indexF in 1:length(all_fileNames_in_directory))
{#Begin - Generate all tasks
    current_fileName = all_fileNames_in_directory[indexF];
    current_complete_fileName = paste(scSnRnaSeq_enrichment_directory,current_fileName, sep = '');
    if ( (grepl(filtered_enrichment_label_plus_extension, current_complete_fileName))
        &(grepl(iPSCd_CM_schaniel_label,current_complete_fileName)))
    {#Begin
       for (indexMaxRank in 1:length(max_fractional_ranks))
       {#Begin
          core_tasks_line = core_tasks_base_line
          core_tasks_line$File_name = current_fileName
          core_tasks_line$Complete_directory = scSnRnaSeq_enrichment_directory
          core_tasks_line$Complete_fileName = current_complete_fileName
          core_tasks_line$Max_figures_per_page = max_figures_per_page[indexMaxRank]
          core_tasks_line$Max_fractional_rank = max_fractional_ranks[indexMaxRank]
          core_tasks_line$Distance_from_left_edge = distances_from_left_edge[indexMaxRank]
          if (length(core_tasks)==0) { core_tasks = core_tasks_line }
          else { core_tasks = rbind(core_tasks,core_tasks_line)}
       }#End
    }#End
}#End - Generate all tasks

indexTask=1
for (indexTask in 1:length(core_tasks[,1]))
{#Begin - Generate_pdf = function(indexTask)
  color_up = "orange";
  highlight_color_up = "chocolate1"
  color_down = "cornflowerblue";
  highlight_color_down = "chartreuse3";
  color_combined = "green"
  drug_color = "darkorchid4";
  Cex_main = 1;
  Cex_label = 1.4;#0.75;
  Cex_axis = 0.75;
  plotted_figures = 0;
  rotating_entryTypes = c("Diffrna", "Diffrna_up", "Diffrna_down")
  rotating_entryType_detected = c(0, 0, 0)
  
  current_complete_fileName = core_tasks$Complete_fileName[indexTask]
  
  max_figures_per_page = core_tasks$Max_figures_per_page[indexTask];
  max_fractional_rank = core_tasks$Max_fractional_rank[indexTask]
  considered_scps_count = max_fractional_rank
  distance_from_left_edge = core_tasks$Distance_from_left_edge[indexTask]
  
  Data = read.table(current_complete_fileName, header = TRUE, sep = '	', quote = "", stringsAsFactors = FALSE)
  indexPvalue0 = which(Data$Pvalue==0)
  Data$Minus_log10_pvalue[indexPvalue0] = 350
  Data$Minus_log10_pvalue = as.numeric(Data$Minus_log10_pvalue)
  indexKeep = which(Data$Scp != "Background genes")
  Data = Data[indexKeep,]
  indexKeep = which(Data$Fractional_rank<=max_fractional_rank)
  Data = Data[indexKeep,]
  indexKeep = which(Data$Sample_entryType %in% c("Diffrna_up","Diffrna_down","Diffrna"))#
  Data = Data[indexKeep,]
  Data$Sample_entryType = gsub("Diffrna_","",Data$Sample_entryType)
  Data$Cluster_no = -1
  Data$Cluster = "error"
      
  {#Begin - Define cluster specific colors
    unique_sample_names = unique(Data$Sample_name)
    unique_sample_names = gsub("_vs_Rest","",unique_sample_names)
    unique_sample_names = unique_sample_names[order(unique_sample_names)]
    clusters = c()
    cluster_nos = c()
    indexU=1
    for (indexU in 1:length(unique_sample_names))
    {#Begin
       unique_sample_name = unique_sample_names[indexU]
       cluster_no = as.numeric(unique_sample_name)
       indexCurrent_sample_name = which(Data$Sample_name==unique_sample_name)
       Data$Cluster_no[indexCurrent_sample_name] = cluster_no
       Data$Cluster[indexCurrent_sample_name] = paste("Cluster ",unique_sample_name,sep='')
       cluster_nos = c(cluster_nos,cluster_no)
    }#End
    max_cluster_no = max(cluster_nos)
    color_values = list()
    if (length(cluster_nos)>0)
    {#Begin
      cluster_nos = cluster_nos[order(cluster_nos)]
      length_clusters = length(cluster_nos)
      my_color_palette <- hue_pal()(max_cluster_no+1)
      indexCluster = 6
      for (indexCluster in 1:length_clusters)
      {#Begin
         cluster_no = cluster_nos[indexCluster]
         color_values[[as.character(cluster_no)]] = my_color_palette[cluster_no+1]
      }#End
    }#End
    bg_color = "gray60"
  }#End - Define cluster specific colors
      
  Data$SampleName_timepoint = paste(Data$Sample_name)#,Data$Sample_timepoint,sep='-')
  entryTypes = unique(Data$Sample_entryType)
  sampleNameTimepoints = unique(Data$SampleName_timepoint)
  for (indexSNT in 1:length(sampleNameTimepoints))
  {#Begin
    sampleName_timepoint = sampleNameTimepoints[indexSNT]
    indexSampleNameTimepoint = which(Data$SampleName_timepoint == sampleName_timepoint)
    current_data = Data[indexSampleNameTimepoint,]
    Data[indexSampleNameTimepoint,] = current_data
    current_entryTypes = unique(current_data$Sample_entryType)
    indexMissing = which(!entryTypes %in% current_entryTypes)
    if (length(indexMissing) > 0)
    {#Begin
      for (indexIndex in 1:length(indexMissing))
      {#Begin
        new_data_line = current_data[1,]
        new_data_line$Minus_log10_pvalue = 0;
        new_data_line$Sample_name = current_data$Sample_name[1]
        new_data_line$Sample_timepoint = current_data$Sample_timepoint[1]
        new_data_line$Sample_entryType = entryTypes[indexMissing[indexIndex]]
        new_data_line$Scp = "No DEGs in ontology"
        new_data_line$Fractional_rank = -1;
        Data = rbind(Data, new_data_line)
      }#End
    }#End
  }#End

  results_directory = paste(dirname(current_complete_fileName),"\\Top",max_fractional_rank,"SCPs\\",sep='')
  dir.create(results_directory)
  current_complete_pdf_file_name = paste(results_directory,gsub(enrichment_file_extension, pdf_file_extension,basename(current_complete_fileName)),sep='')
  
  #current_complete_pdf_file_name = gsub(enrichment_file_extension, pdf_file_extension, current_complete_fileName);
  #current_complete_pdf_file_name = gsub("Aortic_cell_marker_genesAverageGeneExpression_","Avg_", pdf_file_extension, current_complete_fileName);
  #current_complete_pdf_file_name = gsub("Aortic_cell_marker_genesAverageGeneExpression_","Avg_", pdf_file_extension, current_complete_fileName);
  pdf(file = current_complete_pdf_file_name, width = 8.5, height = 11);
  indexEmpty = which(Data$Sample_timepoint=="E_m_p_t_y")
  if (length(indexEmpty)>0) { Data$Sample_timepoint[indexEmpty] = "" }
  Data$Complete_sample_label = paste(Data$Ontology, "\n", Data$Sample_timepoint, " ", Data$Sample_name, " ", Data$Sample_entryType, sep = '');
  par(mfrow = c(max_figures_per_page, 1));
  Data = Data[order(Data$Sample_entryType,decreasing=TRUE),]
  Data = Data[order(Data$Sample_name,decreasing=TRUE),]
  complete_sample_labels = unique(Data$Complete_sample_label);
  indexS=1
  for (indexS in 1:max(length(complete_sample_labels)))
  {#Begin
    current_sample_label = complete_sample_labels[indexS];
    indexCurrent = which(Data$Complete_sample_label == current_sample_label)
    current_data = Data[indexCurrent,]
    title_sample_label = paste(unique(current_data$Ontology),"\n",unique(current_data$Sample_name),"\n",unique(current_data$Sample_entryType),sep='')
    title_sample_label = gsub(" ","\n",title_sample_label)
    while (length(current_data[, 1]) < considered_scps_count)
    {#Begin
        new_col = current_data[1,]
        new_col$Scp = "";
        new_col$Minus_log10_pvalue = 0;
        new_col$Fractional_rank = considered_scps_count + 1;
        current_data = rbind(current_data, new_col)
    }#End
    current_data = current_data[order(current_data$Fractional_rank),]
    Title = gsub("_", " ", title_sample_label);
    bar_names = current_data$Scp;
    ################## Plot all results - BEGIN
    Colors = c()
    if (length(cluster_nos)>0)
    {#Begin
       for (indexCluster in 1:length(color_values))
       {#Begin
          cluster_no_string = names(color_values)[indexCluster]
          if (grepl(cluster_no_string,unique(current_data$Sample_name))) { Colors = rep(color_values[[cluster_no_string]], times = length(current_data[,1]))} 
       }#End
    }#End
    bg_color = "gray"
    if (length(Colors)==0) { Colors = rep(bg_color,times=length(current_data[,1]))}
        
    par( mar=c(3, distance_from_left_edge, 5, 1));
    Cex_names = 1;
    Cex_label = 1;
    Cex_axis = 1;
    Shading=FALSE
    if (length(bar_names)==1) {Cex_names = 1.2}
    if (length(bar_names)==20) {Cex_names = 1.1}
    if (length(bar_names)==10) {Cex_names = 1.5}
    if (length(bar_names)==4) {Cex_names = 1.2}
    if (length(bar_names)==5) {Cex_names = 1.2}
    if (length(bar_names)==6) {Cex_names = 1.0}
    if (grepl("down",Title)) { Shading = TRUE}
    
    if (length(indexCurrent)>0)
    {#Begin
        barplot(rev(current_data$Minus_log10_pvalue), border = rev(Colors), col=rev(Colors), names.arg=rev(bar_names),horiz=TRUE,las=1,xlab="-log10(p-value)", cex.axis = Cex_axis, cex.names = Cex_names, cex.main=Cex_main)
    }#End
    if (length(indexCurrent)==0)
    {#Begin
        barplot(replicate(topSCPs_count,0), xlim=c(0,1),border = rev(Colors), col=rev(Colors), names.arg=replicate(topSCPs_count,"No DEGs"),horiz=TRUE,las=1,xlab="-log10(p-value)", cex.axis = Cex_axis, cex.names = Cex_names, cex.main=Cex_main)
    }#End
    if (Shading)
    {#Begin
        barplot(rev(current_data$Minus_log10_pvalue),names.arg=NA,horiz=TRUE,density=11,add=T,col="white",border=rev(Colors))
    }#End
        
    Cex_names = 0.9*Cex_names;
    for (indexData in 1:length(current_data[,1]))
    {#Begin
        if ((indexData<=length(current_data$Overlap_count))&(!is.null(current_data$Overlap_count[indexData]))&(current_data$Minus_log10_pvalue[indexData]!=0))
        { text(max(current_data$Minus_log10_pvalue)/1800,0.65 + (length(current_data[,1])-indexData)* 1.198,paste(current_data$Overlap_count[indexData]," DEGs",sep=''),pos=4,cex=Cex_names);}
    }#End
    par( mar=c(5, 10, 5, 1));
    title(main=Title,cex.main=Cex_main);
  }#End
  dev.off()
}#End - Generate_pdf = function(indexTask)

