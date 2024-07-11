
####################################################################################################################
## Read and prepare coculture DEGs matrix - BEGIN

complete_degs_fileName = paste(degs_coCulture_directory,degs_coCulture_fileName,sep='')

baseName = gsub(".txt","",degs_fileName)
Data_coCulture = read.csv(file=complete_degs_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');

compare_icasso_centeredMatrixes = FALSE
if (compare_icasso_centeredMatrixes)
{#Begin
  icasso_centered = read.csv(file=complete_icasso_icassoCenteredMatrix_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');
  icasso_colCentered = read.csv(file=complete_icasso_jensColCenteredMatrix_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');
  icasso_rowCentered = read.csv(file=complete_icasso_jensRowCenteredMatrix_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');
}#End

rownames(Data_coCulture) = Data_coCulture$Symbol
colNamesRemove = c("Symbol","RowSum","RowMean","RowSampleSD","RowSampleCV","Description","ReadWrite_SCPs","ReadWrite_human_symbols")
indexColRemove = which(colnames(Data_coCulture) %in% colNamesRemove)
indexColKeep = 1:length(Data_coCulture[1,])
indexColKeep = indexColKeep[!indexColKeep %in% indexColRemove]
Data_coCulture = Data_coCulture[,indexColKeep]

ColSums = colSums(Data_coCulture);
indexZeroCols = which(ColSums==0);
indexColKeep = 1:length(Data_coCulture[1,])
indexColKeep = indexColKeep[!indexColKeep %in% indexZeroCols]
Data_coCulture = Data_coCulture[,indexColKeep]

Colnames = colnames(Data_coCulture)
Colnames = gsub("Diffrna_up.H48.","",Colnames)
Colnames = gsub("Diffrna_down.H48.","",Colnames)
Colnames = gsub("Diffrna.H48.","",Colnames)
Colnames = gsub("Diffprot_up.H48.","",Colnames)
Colnames = gsub("Diffprot_down.H48.","",Colnames)
Colnames = gsub("Diffprot.H48.","",Colnames)
Colnames = gsub("Cell_line.","",Colnames)
Colnames = gsub("Plate.","P",Colnames)
#Colnames = gsub("Cmap.","",Colnames)
colnames(Data_coCulture) = Colnames;

if (preprocess_data=="NoPrepr") { Data_coCulture = Data_coCulture }
if (preprocess_data=="Center") { Data_coCulture = t(scale(t(Data_coCulture), scale=FALSE)) }
if (preprocess_data=="Center_and_scale") { Data_coCulture = t(scale(t(Data_coCulture), scale=TRUE)) }
if (preprocess_data=="Remove_first_svd_eigenassay")
{#Begin
    colnames_data = colnames(Data_coCulture)
    rownames_data = rownames(Data_coCulture)
    svd_data = svd(Data_coCulture)
    svd_data$d[1]=0
    Data_coCulture = svd_data$u %*% diag(svd_data$d) %*% t(svd_data$v)
    colnames(Data_coCulture) = colnames_data
    rownames(Data_coCulture) = rownames_data
}#End
## Read and prepare coculture DEGs matrix - END
####################################################################################################################
####################################################################################################################
## Add data coCulture to data - Begin

indexKeepRows_in_coculture = which(rownames(Data_coCulture) %in% rownames(Data))
Data_coCulture = Data_coCulture[indexKeepRows_in_coculture,]

{#Begin - Extract coCulture drugs
   coCulture_columnNames = colnames(Data_coCulture)
   indexCoCulture = 1
   coCulture_drugs = c()
   for (indexCoCulture in 1:length(coCulture_columnNames))
   {#Begin
      coCulture_columnName = coCulture_columnNames[indexCoCulture]
      splitStrings = strsplit(coCulture_columnName,"[.]")[[1]]
      coCulture_drugs = c(coCulture_drugs,splitStrings[3])
   }#End
   coCulture_drugs = unique(coCulture_drugs)
}#End - Extract coCulture drugs
    
indexAddRows_to_coculture = which(!rownames(Data) %in% rownames(Data_coCulture))
geneSymbols_add_to_coculture = rownames(Data)[indexAddRows_to_coculture]

Col_names = colnames(Data_coCulture)
Col_length = length(Col_names)
Row_names = geneSymbols_add_to_coculture
Row_length = length(Row_names)
add_data_coCulture = as.matrix(array(0,c(Row_length,Col_length),dimnames = list(Row_names,Col_names)))
Data_coCulture = rbind(Data_coCulture,add_data_coCulture)
Data_coCulture = Data_coCulture[order(rownames(Data_coCulture)),]
Data = Data[order(rownames(Data)),]

indexMismatch = which(rownames(Data)!=rownames(Data_coCulture))
if (length(indexMismatch)>0) { stop("Rownames between Data and Data_coCulture don't match")}


Data = cbind(Data,Data_coCulture)

## Add data coCulture to data - End
####################################################################################################################
source("SVD_Set_uniqueEntries_of_entityClasses_based_on_Data.R")
