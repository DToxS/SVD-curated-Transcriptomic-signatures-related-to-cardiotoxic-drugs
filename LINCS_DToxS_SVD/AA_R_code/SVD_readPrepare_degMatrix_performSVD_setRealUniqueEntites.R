
####################################################################################################################
## Read and prepare DEGs matrix - BEGIN

baseName = gsub(".txt","",degs_fileName)
Data = read.csv(file=complete_degs_fileName,header=TRUE,stringsAsFactors=FALSE,sep='\t');

rownames(Data) = Data$Symbol
colNamesRemove = c("Symbol","RowSum","RowMean","RowSampleSD","RowSampleCV","Description","ReadWrite_SCPs","ReadWrite_human_symbols")
indexColRemove = which(colnames(Data) %in% colNamesRemove)
indexColKeep = 1:length(Data[1,])
indexColKeep = indexColKeep[!indexColKeep %in% indexColRemove]
Data = Data[,indexColKeep]

ColSums = colSums(Data);
indexZeroCols = which(ColSums==0);
indexColKeep = 1:length(Data[1,])
indexColKeep = indexColKeep[!indexColKeep %in% indexZeroCols]
Data = Data[,indexColKeep]

Colnames = colnames(Data)
Colnames = gsub("Diffrna_up.H48.","",Colnames)
Colnames = gsub("Diffrna_down.H48.","",Colnames)
Colnames = gsub("Diffrna.H48.","",Colnames)
Colnames = gsub("Diffprot_up.H48.","",Colnames)
Colnames = gsub("Diffprot_down.H48.","",Colnames)
Colnames = gsub("Diffprot.H48.","",Colnames)
Colnames = gsub("Cell_line.","",Colnames)
Colnames = gsub("Plate.","P",Colnames)
#Colnames = gsub("Cmap.","",Colnames)
colnames(Data) = Colnames;

if (preprocess_data=="NoPrepr") { Data = Data }
if (preprocess_data!="NoPrepr") { stop(paste(preprocess_data," is not considered",sep='')) }

## Read and prepare DEGs matrix - END
####################################################################################################################
####################################################################################################################
## Perform SVD on DEGs matrix - BEGIN
if (decomposition_method=="SVD")
{#Begin
   svd_data = svd(Data)
   length_eigenassays = length(svd_data$v[1,])

   eigenexpression = svd_data$d
   eigenexpression_fraction = eigenexpression / sum(eigenexpression)
   cum_eigensexpression_fraction = cumsum(eigenexpression_fraction)
   
   #icasso_Iq  = replicate(length(eigenexpression),1)
}#End
if (decomposition_method!="SVD")
{ stop(paste(decomposition_method," is not considered",sep='')) }

## Perform SVD on DEGs matrix - END
####################################################################################################################
####################################################################################################################
source("SVD_Set_uniqueEntries_of_entityClasses_based_on_Data.R")
