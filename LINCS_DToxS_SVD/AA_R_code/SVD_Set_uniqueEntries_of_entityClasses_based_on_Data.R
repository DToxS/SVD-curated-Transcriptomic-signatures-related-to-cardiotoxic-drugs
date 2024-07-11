####################################################################################################################
## Update unique entities of entiyClasses - BEGIN

Colnames = colnames(Data)
real_celllines_in_columnOrder = c()
real_plates_in_columnOrder = c()
real_genders_in_columnOrder = c()
real_drugClasses_in_columnOrder = c()
real_drugs_in_columnOrder = c()
real_drugTargetClasses_in_columnOrder = c()
real_colnames_in_columnOrder = Colnames
indexCol=1
for (indexCol in 1:length(real_colnames_in_columnOrder))
{#Begin
  splitStrings = strsplit(real_colnames_in_columnOrder[indexCol],'[.]')[[1]]
  current_cellline = splitStrings[2]
  real_celllines_in_columnOrder = c(real_celllines_in_columnOrder,current_cellline)
  current_drugClass = splitStrings[1]
  real_drugClasses_in_columnOrder = c(real_drugClasses_in_columnOrder,current_drugClass)
  current_drug = splitStrings[3]
  real_drugs_in_columnOrder = c(real_drugs_in_columnOrder,current_drug)
  current_plate = splitStrings[4]
  real_plates_in_columnOrder = c(real_plates_in_columnOrder,current_plate)
}#End

real_celllines = unique(real_celllines_in_columnOrder)
real_drugs = unique(real_drugs_in_columnOrder)
real_plates = unique(real_plates_in_columnOrder)
real_drugClasses = unique(real_drugClasses_in_columnOrder)
####################################################################################################################
