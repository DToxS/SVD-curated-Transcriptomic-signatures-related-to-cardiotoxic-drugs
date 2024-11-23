if (!exists("projected_data_type")) { projected_data_type = "" }
if ((exists("Data_test"))&(projected_data_type==training_and_test_dataType_label))
{#Begin - if exists(Data_test)
   if (decomposition_method=="SVD")
   {#Begin
      Data_test_current = array(NA,dim(Data_test))
      for (indexTestCol in 1:length(Data_test[1,]))
      {#Begin
         current_test_vector = Data_test[,indexTestCol]
         final_projection=replicate(length(current_test_vector),0)
         for (indexIndexEigenassay in 1:length(current_eigenassays))
         {#Begin
            current_eigenassay = current_eigenassays[indexIndexEigenassay]
            unit_eigenassay = svd_data$u[,current_eigenassay]
            current_projection_length = sum(current_test_vector * unit_eigenassay)
            current_projection = current_projection_length * unit_eigenassay
            final_projection = final_projection + current_projection
         }#End
         Data_test_current[,indexTestCol] = final_projection
      }#End
   }#END
   if (decomposition_method=="ICA")
   {#Begin
      rm(Data_current)
   }#eND
   colnames(Data_test_current) = colnames(Data_test)
   combined_colnames_in_columnOrder = c(real_colnames_in_columnOrder,colnames(Data_test_current))
   if (length(which(!combined_colnames_in_columnOrder %in% colnames(Data_combined)))>0) { rm(Data_combined); rm(Data_test); rm(Data) }
   if (unique(combined_colnames_in_columnOrder %in% colnames(Data_combined))!=TRUE) { rm(Data_combined); rm(Data_test); rm(Data) }
   
   real_combined_trainingTestSet_in_columnOrder = c(replicate(length(real_celllines_in_columnOrder),"Training"),replicate(length(real_test_celllines_in_columnOrder),"Test"))
   real_combined_celllines_in_columnOrder = c(real_celllines_in_columnOrder,real_test_celllines_in_columnOrder)
   real_combined_drugs_in_columnOrder = c(real_drugs_in_columnOrder,real_test_drugs_in_columnOrder)
   real_combined_plates_in_columnOrder = c(real_plates_in_columnOrder,real_test_plates_in_columnOrder)
   real_combined_drugClasses_in_columnOrder = c(real_drugClasses_in_columnOrder,real_test_drugClasses_in_columnOrder)
   
}#End - if exists(Data_test)
if ((!exists("Data_test"))|(projected_data_type!=training_and_test_dataType_label))
{#Begin
   Data_test_current = c()

}#End


combined_colnames_in_columnOrder = real_colnames_in_columnOrder
if (length(which(!combined_colnames_in_columnOrder %in% colnames(Data_combined)))>0) { rm(Data_combined); rm(Data_test); rm(Data) }
equal_names = unique(combined_colnames_in_columnOrder == colnames(Data_combined))
if ((length(equal_names)!=1)|(equal_names[1]!=TRUE)) { rm(Data_combined); rm(Data_test); rm(Data) }

real_combined_trainingTestSet_in_columnOrder = c(replicate(length(real_celllines_in_columnOrder),"Training"))
real_combined_celllines_in_columnOrder = real_celllines_in_columnOrder
real_combined_drugs_in_columnOrder = real_drugs_in_columnOrder
real_combined_plates_in_columnOrder = real_plates_in_columnOrder
real_combined_drugClasses_in_columnOrder = real_drugClasses_in_columnOrder

real_combined_trainingTestSets = unique(real_combined_trainingTestSet_in_columnOrder)
real_combined_celllines = unique(real_combined_celllines_in_columnOrder)
real_combined_drugs = unique(real_combined_drugs_in_columnOrder)
real_combined_plates = unique(real_combined_plates_in_columnOrder)
real_combined_drugClasses = unique(real_combined_drugClasses_in_columnOrder)

Data_combined_current = cbind(Data_current,Data_test_current)


