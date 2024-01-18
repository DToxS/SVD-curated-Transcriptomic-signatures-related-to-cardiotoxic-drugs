rm(list = ls());
library(ggbeeswarm)
delete_task_reports=TRUE
add_inFrontOf_progress_report_fileName = "SVD_0"
get_eigenassays_per_dataset=FALSE
source('SVD_global_parameter.R')
source('SVD_colors.R')
source('Common_tools.R')

directory = paste(lincs_results_directory,"//SVD_control_expression//",sep='')

{#Begin - Read and prepare correlations
   fileName = "Gtex_vs_DEGenes_iPSCdCMs_P0_replicateExpression_Control_expression_values.txt"
   complete_fileName = paste(directory,fileName,sep='')
   correlation = read.csv(file=complete_fileName,header=TRUE,stringsAsFactors = FALSE,sep='\t')
   correlation$ReadWrite_names0 = gsub("Control;Cell_line.","",correlation$ReadWrite_names0)
   correlation$ReadWrite_names0 = gsub(";CTRL;Plate.0_"," ",correlation$ReadWrite_names0)
   readWriteNames0s = unique(correlation$ReadWrite_names0)
   indexRWN0=1
   correlation$Cell_line = "error"
   correlation$Replicate = "error"
   for (indexRWN0 in 1:length(readWriteNames0s))
   {#Begin
      readWriteName0 = readWriteNames0s[indexRWN0]
      splitStrings = strsplit(readWriteName0,"_")[[1]]
      cellLine = splitStrings[1]
      replicate = splitStrings[2]
      indexCurrentReadWrite = which(correlation$ReadWrite_names0==readWriteName0)
      correlation$Cell_line[indexCurrentReadWrite] = cellLine
      correlation$Replicate[indexCurrentReadWrite] = replicate
   }#End
   cell_lines = unique(correlation$Cell_line)
   correlation$Cell_line_replicatesCount = "error"
   for (indexCellLine in 1:length(cell_lines))
   {#Begin
      cell_line = cell_lines[indexCellLine]
      indexCurrentCellLine = which(correlation$Cell_line==cell_line)
      replicates_count = length(unique(correlation$Replicate[indexCurrentCellLine]))
      correlation$Cell_line_replicatesCount[indexCurrentCellLine] = paste(cell_line,"\n(n=",replicates_count,")")
   }#End
}#End - Read and prepare correlations

consider_top_x_tissues = 6
correlation = correlation[order(correlation$Correlation_coefficient,decreasing = TRUE),]
top_tissues = unique(correlation$ReadWrite_names1)[1:consider_top_x_tissues]

max_correlation_coefficient = ceiling(max(correlation$Correlation_coefficient)*10)/10


Correlation_plots = list()
indexTissue=1
for (indexTissue in 1:length(top_tissues))
{#Begin
   tissue = top_tissues[indexTissue]
   indexCurrentTissue = which(correlation$ReadWrite_names1==tissue)
   tissue_correlation = correlation[indexCurrentTissue,]
   cellLines = unique(correlation$Cell_line)
   
   current_colors = replicate(length(cellLines),"gray")
   for (indexCellline in 1:length(cellLines))
   {#Begin
      cellLine = cellLines[indexCellline]
      indexCurrentColor = which(cellLines_for_highlight_colors==cellLine)
      current_colors[indexCellline] = cellLine_highlight_colors[indexCurrentColor]
   }#End

   plot_title = gsub("_"," ",tissue)
   plot_title = Split_name_over_multiple_lines(name=plot_title, max_nchar_per_line=30, max_lines=2)
   
   Correlation_plot = ggplot(tissue_correlation,aes(x=Cell_line_replicatesCount,y=Correlation_coefficient,color=Cell_line))
   Correlation_plot = Correlation_plot + geom_beeswarm(priority="ascending",size=2)
   Correlation_plot = Correlation_plot + ggtitle(plot_title)
   Correlation_plot = Correlation_plot + scale_color_manual(values=current_colors)
   Correlation_plot = Correlation_plot + ylim(c(0,max_correlation_coefficient))
   Correlation_plot = Correlation_plot + xlab("") + ylab("Pearson correlation\ncoefficient")
   Correlation_plot = Correlation_plot + theme(plot.title =  element_text(size=15,vjust=0.5,hjust=0.5,face=2))
   Correlation_plot = Correlation_plot + theme(axis.text.x = element_text(size=10))
   Correlation_plot = Correlation_plot + theme(axis.text.y = element_text(size=12))
   Correlation_plot = Correlation_plot + theme(axis.title.y = element_text(size=12))
   Correlation_plot = Correlation_plot + theme(axis.title.x = element_text(size=12))
   Correlation_plot = Correlation_plot + theme(legend.position = "none")
   Correlation_plots = append(Correlation_plots,list(Correlation_plot))
}#End

complete_pdf_fileName = paste(directory,gsub(".txt",".pdf",fileName),sep='')
Generate_plots(Correlation_plots,complete_pdf_fileName,3,2)


{#Begin - Read and prepare
  fileName = "Expression_DEGenes_iPSCdCMs_P0_replicateExpression_Control_expression_values.txt"
  complete_fileName = paste(directory,fileName,sep='')
  replicate_data = read.csv(file=complete_fileName,header=TRUE,sep='\t',stringsAsFactors = FALSE)
  rownames(replicate_data) = replicate_data$Symbol
  
  removeColNames = c("Symbol","RowSum","RowMean","RowSampleSD","RowSampleCV","Description","ReadWrite_SCPs","ReadWrite_human_symbols")
  indexKeepCol = 1:length(replicate_data[1,])
  indexRemoveCol = which(colnames(replicate_data)%in%removeColNames)
  indexKeepCol = indexKeepCol[!indexKeepCol %in% indexRemoveCol]
  replicate_data = replicate_data[,indexKeepCol]
  Colnames = colnames(replicate_data)
  Colnames = gsub("Control.Cell_line.","",Colnames)
  Colnames = gsub(".CTRL.Plate.0_"," ",Colnames)
  Colnames = gsub("_03R","",Colnames)
  Colnames = gsub("_04R","",Colnames)
  Colnames = gsub("_01R","",Colnames)
  Colnames = gsub("_13R","",Colnames)
  Colnames = gsub("_07R","",Colnames)
  Colnames = gsub("_13R","",Colnames)
  Colnames = gsub("repNo","#",Colnames)
  colnames(replicate_data) = Colnames
  
  #colsums = colSums(replicate_data)
  #newColSum = mean(colsums)
  #for (indexCol in 1:length(colsums))
  #{#Begin
  #   replicate_data[,indexCol] = newColSum/colsums[indexCol] * replicate_data[,indexCol]
  #}#End
}#End - Read and prepare


dist.function = function(x) as.dist((1-cor((x),method=correlation_method))/2)
hclust.function = function(x) hclust(x,method="average")

col_dist = dist.function(replicate_data) 
col_hc = hclust.function(col_dist)
col_dend = as.dendrogram(col_hc)
indexOriginalDataCol = order.dendrogram(col_dend);
col_labels_in_dendrogram = colnames(replicate_data)[indexOriginalDataCol]
reordered_replicate_log10data = replicate_data[,indexOriginalDataCol]
reordered_replicate_log10data = log10(reordered_replicate_log10data+0.1)

row_dist = dist.function(t(replicate_data))
row_hc = hclust.function(row_dist)
row_dend = as.dendrogram(row_hc)
indexOriginalDataRow = order.dendrogram(row_dend);
row_labels_in_dendrogram = colnames(replicate_data)[indexOriginalDataRow]
reordered_replicate_log10data = reordered_replicate_log10data[indexOriginalDataRow,]

dend_colors = replicate(length(col_labels_in_dendrogram),"gray")
for (indexCol in 1:length(dend_colors))
{#Begin
   cell_line = strsplit(col_labels_in_dendrogram[indexCol]," ")[[1]][1]
   indexCurrentColor = which(cellLines_for_highlight_colors==cell_line)
   dend_colors[indexCol] = cellLine_highlight_colors[indexCurrentColor]
}#End


col_dend = color_labels(col_dend, col = dend_colors)
col_dend = set(col_dend, "labels_cex", 0.8)
col_dend = set(col_dend,"branches_lwd", 0.7)

png_width = 5000

complete_replicateData_colDend_fileName = paste(directory,"Replicate_colDend.png",sep='')
png(complete_replicateData_colDend_fileName, width = png_width, height = 3200, res = 2000);
par(cex = 0.3, font = 2.3);
par(mar = c(9, 0, 2, 0) + 0.1)
plot(col_dend, horiz = FALSE);
dev.off();


{#Begin - Define heatmap color map
  t_reordered_replicate_log10data = t(reordered_replicate_log10data)
  
  warm_color_count = 1000;
  warm = rainbow(warm_color_count, start = rgb2hsv(col2rgb('darkred'))[1], end = rgb2hsv(col2rgb('yellow'))[1])
  Color_map <- colorRampPalette(warm)(255)
  
  complete_heatmap_fileName = paste(directory,"Heatmap_log10.png",sep='')
  png(filename = complete_heatmap_fileName, width = png_width, height = 5000, res = 100)
  par(mar = c(7, 4, 2, 5) + 0.1)
  image(t_reordered_replicate_log10data, axes = FALSE, col = Color_map);
  #image.plot(reordered_replicate_log10data, col = Color_map, axes = FALSE);
  box(col = "black", lty = "solid")
  dev.off()

  complete_heatmap_fileName = paste(directory,"Heatmap_log10_legend.png",sep='')
  png(filename = complete_heatmap_fileName, width = png_width, height = 1000, res = 100)
  par(mar=c(7,4,2,5)+0.1)
  image.plot(t_reordered_replicate_log10data,legend.only=TRUE, col= Color_map, new=TRUE);
  dev.off()
}#End - Define heatmap color map







