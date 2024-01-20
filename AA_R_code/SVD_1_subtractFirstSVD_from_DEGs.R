#library(parallel)
#stopCluster(parallel_clusters)
rm(list = ls());
delete_task_reports=TRUE
add_inFrontOf_progress_report_fileName = "SVD_1"
get_eigenassays_per_dataset=FALSE
source('SVD_global_parameter.R')

preprocess_data = "Remove_first_svd_eigenassay"
decomposition_method = "None"
#source('SVD_coreTaskspecific_parameter.R')
input_datasets = c(
             "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue",
             "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue"
)

substract_svds = list( 
  "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue" = c(1),
  "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue" = c(1)          
)

inputDataset_inputSVDDataset_list =
list("SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue" = "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue",
     "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue" = "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue")

no1stSVD_label = "_no1stSVD"
only1stSVD_label = "_only1stSVD"
f1_score_weight=0

indexDataset = 2
for (indexDataset in 1:length(input_datasets))
{#Begin
   input_dataset = input_datasets[indexDataset]
   if (!input_dataset %in% names(substract_svds)) { stop("!input_dataset %in% names(substract_svds)") }
   if (!input_dataset %in% names(inputDataset_inputSVDDataset_list)) { stop("!input_dataset %in% names(inputDataset_inputSVDDataset_list)") }
   input_svd_dataset = inputDataset_inputSVDDataset_list[[input_dataset]]
   svd_to_be_removed = substract_svds[[input_dataset]]
   if (!grepl(no1stSVD_label,input_dataset))
   {#Begin
      output_dataset = paste(input_dataset,no1stSVD_label,sep='')
      output_only1stSVD = paste(input_dataset,only1stSVD_label,sep='')
      
      input_degs_directory = paste(lincs_results_directory,input_dataset,"\\","1_DEGs\\",sep='')
      output_directory = paste(lincs_results_directory,output_dataset,"\\",sep='')
      dir.create(output_directory)
      output_1stSVD_directory = paste(lincs_results_directory,output_only1stSVD,"\\",sep='')
      output_degs_directory = paste(output_directory,"\\","1_DEGs\\",sep='')
      output_1stSVD_degs_directory = paste(output_1stSVD_directory,"\\","1_DEGs\\",sep='')
      ouput_r_gene_expression_directory = paste(output_directory,"\\","9_Drug_specific_expression_values\\",sep='')
      dir.create(ouput_r_gene_expression_directory)
      
      input_degs_fileName = paste(input_dataset,"_topall.txt",sep='')
      input_degsSummary_fileName = "DEG_summary.txt"
      output_degsSummary_fileName = "DEG_summary.txt"
      output_degsSummary_1stSVD_fileName = "DEG_summary.txt"
      output_removed_SVDs_fileName = "Removed_SVDs.txt"
      output_degs_fileName = paste(output_dataset,"_topall.txt",sep='')
      output_eigenassay_fileName = "All_eigenassays_including_removed_ones.txt"
      output_degs_firstSVD_fileName = paste(output_only1stSVD,"_topall.txt",sep='')
      output_r_output_gene_expression = paste("Drug_specific_expression_full.txt",sep='')
      output_r_output_gene_expression_no1SVD = paste("Drug_specific_expression_SVD",paste(svd_to_be_removed,collapse='_'),"removed.txt",sep='');
      complete_input_degs_fileName = paste(input_degs_directory,input_degs_fileName,sep='')
      complete_output_degs_fileName = paste(output_degs_directory,output_degs_fileName,sep='')
      complete_output_degs_1stSVD_fileName = paste(output_1stSVD_degs_directory,output_degs_firstSVD_fileName,sep='')
      complete_output_removed_SVDs_fileName = paste(output_degs_directory,output_removed_SVDs_fileName,sep='')
      complete_input_degsSummary_fileName = paste(input_degs_directory,input_degsSummary_fileName,sep='')
      complete_output_degsSummary_fileName = paste(output_degs_directory,output_degsSummary_fileName,sep='')
      complete_output_eigenassays_fileName = paste(output_degs_directory,output_eigenassay_fileName,sep='')
      complete_r_output_gene_expression_fileName = paste(ouput_r_gene_expression_directory,output_r_output_gene_expression,sep='')
      complete_r_output_eigenassay_expression_fileName = paste(ouput_r_gene_expression_directory,output_eigenassay_fileName,sep='')
      complete_r_output_gene_expression_no1SVD_fileName = paste(ouput_r_gene_expression_directory,output_r_output_gene_expression_no1SVD,sep='')
      complete_output_degsSummary_1stSVD_fileName = paste(output_1stSVD_directory,output_degsSummary_1stSVD_fileName,sep='')
      
      Summary = read.csv(file=complete_input_degsSummary_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');

      perform_svd=TRUE
      if (perform_svd)
      {#Perform SVD and save
      Data = read.csv(file=complete_input_degs_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');
      rownames(Data) = Data$Symbol
      
      colNamesRemove = c("Symbol","RowSum","RowMean","RowSampleSD","RowSampleCV","Description","ReadWrite_SCPs","ReadWrite_human_symbols")
      indexColRemove = which(colnames(Data) %in% colNamesRemove)
      indexColKeep = 1:length(Data[1,])
      indexColKeep = indexColKeep[!indexColKeep %in% indexColRemove]
      Data = Data[,indexColKeep]
      
      if (input_dataset %in% names(inputDataset_inputDatasetForSvd_list))
      {#Begin
         input_svd_dataset = inputDataset_inputDatasetForSvd_list[[input_dataset]]
      }#End
      if (!input_dataset %in% names(inputDataset_inputDatasetForSvd_list))
      {#Begin
         input_svd_dataset = input_dataset
      }#End

      if (input_svd_dataset==input_dataset)
      {#Begin
         data_for_svd = Data
      }#End
      if (input_svd_dataset!=input_dataset)
      {#Begin
         svd_degs_fileName = paste(input_svd_dataset,"_topall.txt",sep='')
         input_svd_degs_directory = paste(lincs_results_directory,input_svd_dataset,"\\","1_DEGs\\",sep='')
         complete_input_degs_fileName = paste(input_svd_degs_directory,svd_degs_fileName,sep='')
         data_for_svd = read.csv(file=complete_input_degs_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');
         
         colNamesRemove = c("Symbol","RowSum","RowMean","RowSampleSD","RowSampleCV","Description","ReadWrite_SCPs","ReadWrite_human_symbols")
         rownames(data_for_svd) = data_for_svd$Symbol
         indexColRemove = which(colnames(data_for_svd) %in% colNamesRemove)
         indexColKeep = 1:length(data_for_svd[1,])
         indexColKeep = indexColKeep[!indexColKeep %in% indexColRemove]
         data_for_svd = data_for_svd[,indexColKeep]
         
         indexRowKeep = which(rownames(Data) %in% rownames(data_for_svd))
         Data = Data[indexRowKeep,]
         indexAddRows = which(!rownames(data_for_svd) %in% rownames(Data))
         geneSymbols_add = rownames(data_for_svd)[indexAddRows]
         
         Col_names = colnames(Data)
         Col_length = length(Col_names)
         Row_names = geneSymbols_add
         Row_length = length(Row_names)
         add_data = as.matrix(array(0,c(Row_length,Col_length),dimnames = list(Row_names,Col_names)))
         Data = rbind(Data,add_data)
         Data = Data[order(rownames(Data)),]
         data_for_svd = data_for_svd[order(rownames(data_for_svd)),]
         
         indexMismatch = which(rownames(Data)!=rownames(data_for_svd))
         if (length(indexMismatch)>0) { stop("Rownames between Data and Data for svd don't match")}
      }#End
      
      colnames_data = colnames(Data)
      rownames_data = rownames(Data)
      
      colnames_data_for_svd = colnames(data_for_svd)
      rownames_data_for_svd = rownames(data_for_svd)
      svd_data = svd(data_for_svd)
      
      eigenassay_matrix = svd_data$u
      rownames(eigenassay_matrix) = rownames(data_for_svd)
      colnames(eigenassay_matrix) = 1:length(eigenassay_matrix[1,])
      colnames(eigenassay_matrix) = paste("Eigenassay_",colnames(eigenassay_matrix),sep='')
      
      first_diagonals = svd_data$d[svd_to_be_removed]
      svd_data$d[svd_to_be_removed]=0
      new_diagonal = replicate(length(svd_data$d),0)
      new_diagonal[svd_to_be_removed] = first_diagonals
      
      if (input_svd_dataset==input_dataset)
      {#Begin
         Data_afterSVDsRemoval = svd_data$u %*% diag(svd_data$d) %*% t(svd_data$v)
         Data_removedSVDs = svd_data$u %*% diag(new_diagonal) %*% t(svd_data$v)
      }#End
      if (input_svd_dataset!=input_dataset)
      {#Begin
         current_d_inverse = 1/svd_data$d
         current_d_inverse[svd_to_be_removed] = 0
         current_v_afterSVDsRemoval = t(Data) %*% svd_data$u %*% diag(current_d_inverse)
         Data_afterSVDsRemoval = svd_data$u %*% diag(svd_data$d) %*% t(current_v_afterSVDsRemoval)

         current_new_d_inverse = 1/new_diagonal
         current_v_removedSVDs = t(Data) %*% svd_data$u %*% diag(current_new_d_inverse)
         Data_removedSVDs = svd_data$u %*% diag(new_diagonal) %*% t(current_v_removedSVDs)
      }#End
      
      colnames(Data_afterSVDsRemoval) = colnames_data
      rownames(Data_afterSVDsRemoval) = rownames_data
      colnames(Data_removedSVDs) = colnames_data
      rownames(Data_removedSVDs) = rownames_data
      
      Colnames = colnames(Data_afterSVDsRemoval)
      Colnames = gsub("[.]","-",Colnames)
      Colnames = gsub("Plate-","Plate.",Colnames)
      Colnames = gsub("Cell_line-","Cell_line.",Colnames)
      colnames(Data_afterSVDsRemoval) = Colnames
      
      Colnames = colnames(Data_removedSVDs)
      Colnames = gsub("[.]","-",Colnames)
      Colnames = gsub("Plate-","Plate.",Colnames)
      Colnames = gsub("Cell_line-","Cell_line.",Colnames)
      colnames(Data_removedSVDs) = Colnames
      
      dir.create(output_directory)
      dir.create(output_degs_directory)
      dataset = output_dataset
      write.table(Summary,file=complete_output_degsSummary_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')
      write.table(svd_to_be_removed,file=complete_output_removed_SVDs_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')

      if (length(svd_to_be_removed)>0)
      {#Begin
         dir.create(output_1stSVD_directory)
         dir.create(output_1stSVD_degs_directory)
         
         write.table(Summary,file=complete_output_degsSummary_1stSVD_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')
         
         #complete_output_degs_1stSVD_matrix = gsub(".txt","_matrix.txt",complete_output_degs_1stSVD_fileName)
         #complete_output_degs_1stSVD_rowNames = gsub(".txt","_rowNames.txt",complete_output_degs_1stSVD_fileName)
         #complete_output_degs_1stSVD_colNames = gsub(".txt","_colNames.txt",complete_output_degs_1stSVD_fileName)
         #write.table(Data_afterSVDsRemoval,file=complete_output_degs_1stSVD_matrix,quote = FALSE,col.names=FALSE,row.names=FALSE,sep='\t')
         #write.table(t(colnames(Data_afterSVDsRemoval)),file=complete_output_degs_1stSVD_colNames,quote = FALSE,col.names=FALSE,row.names=FALSE,sep='\t')
         #write.table(rownames(Data_afterSVDsRemoval),file=complete_output_degs_1stSVD_rowNames,quote = FALSE,col.names=FALSE,row.names=FALSE,sep='\t')
         
         Data_removedSVDs = as.data.frame(Data_removedSVDs)
         Data_removedSVDs$Symbol = rownames(Data_removedSVDs)
         write.table(Data_removedSVDs,file=complete_output_degs_1stSVD_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')
      }#End

      #complete_output_degs_matrix = gsub(".txt","_matrix.txt",complete_output_degs_fileName)
      #complete_output_degs_rowNames = gsub(".txt","_rowNames.txt",complete_output_degs_fileName)
      #complete_output_degs_colNames = gsub(".txt","_colNames.txt",complete_output_degs_fileName)
      #write.table(Data_afterSVDsRemoval,file=complete_output_degs_matrix,quote = FALSE,col.names=FALSE,row.names=FALSE,sep='\t')
      #write.table(t(colnames(Data_afterSVDsRemoval)),file=complete_output_degs_colNames,quote = FALSE,col.names=FALSE,row.names=FALSE,sep='\t')
      #write.table(rownames(Data_afterSVDsRemoval),file=complete_output_degs_rowNames,quote = FALSE,col.names=FALSE,row.names=FALSE,sep='\t')
      
      Data_afterSVDsRemoval = as.data.frame(Data_afterSVDsRemoval)
      Data_afterSVDsRemoval$Symbol = rownames(Data_afterSVDsRemoval)
      write.table(Data_afterSVDsRemoval,file=complete_output_degs_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')

      data_colnames = colnames(Data)
      indexDataCol=1
      celllines = c()
      plates = c()
      for (indexDataCol in 1:length(data_colnames))
      {#Begin
         data_colname = data_colnames[indexDataCol]
         splitStrings = strsplit(data_colname,"[.]")[[1]]
         plates = c(plates,splitStrings[8])
         celllines = c(celllines,splitStrings[5])
      }#End
      celllines = unique(celllines)
      plates = unique(plates)
      if (length(plates)==1) { plate_string = plates}
      if (length(plates)!=1) { plate_string = paste(plates,collapse="_") }
      
      Data_for_r_gene_expression = eigenassay_matrix
      colnames(Data_for_r_gene_expression) = paste(colnames(Data_for_r_gene_expression),".","All",".","Eigenassay",".",plate_string,sep='')
      delimiter_that_separates_colnames = "[.]"
      col_splitString_indexDrug = 1
      col_splitString_indexCellline = 2 
      col_splitString_indexDrugClass = 3 
      col_splitString_indexPlate = 4
      add_inFrontOf_cellline_string = "Cell_line."
      add_inFrontOf_plate_string = "Plate."
      replace_p_by_plate = FALSE
      f1_score_weight = -1
      entityClass="Eigenassay"
      source('SVD_generate_R_output_gene_expression_array.R')
      write.table(r_output_gene_expression_lines,file=complete_r_output_eigenassay_expression_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')

      Data_for_r_gene_expression = Data
      delimiter_that_separates_colnames = "[.]"
      col_splitString_indexDrug = 6
      col_splitString_indexCellline = 5 
      col_splitString_indexDrugClass = 3 
      col_splitString_indexPlate = 8
      add_inFrontOf_cellline_string = "Cell_line."
      add_inFrontOf_plate_string = "Plate."
      replace_p_by_plate = FALSE
      entityClass="Drug"
      source('SVD_generate_R_output_gene_expression_array.R')
      write.table(r_output_gene_expression_lines,file=complete_r_output_gene_expression_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')
      
      indexKeepCol = which(!colnames(Data_afterSVDsRemoval)%in%c("Symbol"))
      Data_for_r_gene_expression = Data_afterSVDsRemoval[,indexKeepCol]
      delimiter_that_separates_colnames = "-"
      col_splitString_indexDrug = 5
      col_splitString_indexCellline = 4 
      col_splitString_indexDrugClass = 3 
      col_splitString_indexPlate = 6
      add_inFrontOf_cellline_string = ""
      add_inFrontOf_plate_string = ""
      replace_p_by_plate = FALSE
      entityClass="Drug"
      source('SVD_generate_R_output_gene_expression_array.R')
      
      write.table(r_output_gene_expression_lines,file=complete_r_output_gene_expression_no1SVD_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')

      Data_for_r_gene_expression = eigenassay_matrix
      delimiter_that_separates_colnames = "[.]"
      col_splitString_indexDrug = 1
      col_splitString_indexCellline = 2 
      col_splitString_indexDrugClass = 3 
      col_splitString_indexPlate = 4
      add_inFrontOf_cellline_string = "Cell_line."
      add_inFrontOf_plate_string = "Plate."
      replace_p_by_plate = FALSE
      entityClass="Eigenassay"
      source('SVD_generate_R_output_gene_expression_array.R')
      r_output_gene_expression_lines$Drug_class="Kinase_inhibitor"
      
      write.table(r_output_gene_expression_lines,file=complete_output_eigenassays_fileName,quote = FALSE,col.names=TRUE,row.names=FALSE,sep='\t')
      
      }#Perform SVD and save
   }#End
}#End
