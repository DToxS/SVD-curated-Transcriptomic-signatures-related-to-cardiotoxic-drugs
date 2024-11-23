all_ordered_symbols = rownames(Data_for_r_gene_expression)
all_ordered_symbols = all_ordered_symbols[order(all_ordered_symbols)]

Col_names = c("EntityClass","Drug_class","Drug","Cell_line","Plate","Gene_symbol","F1_score_weight","Value","Correlation_method","Minimum_fraction_of_max_f1score","EA_correlation_parameter","Reference_valueType","Dataset","Preprocess_data","Decomposition_method")
Col_length = length(Col_names)
Row_names = all_ordered_symbols;
Row_length = length(Row_names)
r_output_gene_expression_base_lines = array(NA,c(Row_length,Col_length),dimnames = list(Row_names,Col_names))
r_output_gene_expression_base_lines = as.data.frame(r_output_gene_expression_base_lines)
r_output_gene_expression_base_lines$Gene_symbol = all_ordered_symbols
r_output_gene_expression_lines = c()

colnames_data_current = colnames(Data_for_r_gene_expression)
Data_for_r_gene_expression = Data_for_r_gene_expression[order(rownames(Data_for_r_gene_expression)),,drop=FALSE]

if (!exists("delimiter_that_separates_colnames"))
{ delimiter_that_separates_colnames = "-" }
if (!exists("col_splitString_indexDrug"))
{ col_splitString_indexDrug = 5 }
if (!exists("col_splitString_indexCellline"))
{ col_splitString_indexCellline = 4 }
if (!exists("col_splitString_indexDrugClass"))
{ col_splitString_indexDrugClass = 3 }
if (!exists("col_splitString_indexPlate"))
{ col_splitString_indexPlate = 6 }
if (!exists("add_inFrontOf_cellline_string"))
{ add_inFrontOf_cellline_string = "" }
if (!exists("replace_p_by_plate"))
{ replace_p_by_plate = FALSE }
if (!exists("add_inFrontOf_plate_string"))
{ add_inFrontOf_plate_string = "" }


unique_equal = unique(r_output_gene_expression_base_lines$Gene_symbol==rownames(Data_for_r_gene_expression))
stop = FALSE
if ("FALSE" %in% unique_equal) { stop = TRUE }
length_cols = length(Data_for_r_gene_expression[1,])
if (!stop)
{#Begin
indexCol=1
for (indexCol in 1:length_cols)
{#Begin
  colname = colnames(Data_for_r_gene_expression)[indexCol]
  if (colname!="POS")
  {#Begin
     splitStrings = strsplit(colname,delimiter_that_separates_colnames)[[1]]
     current_r_output_gene_expression_lines = r_output_gene_expression_base_lines
     if (exists("entityClass"))
     { current_r_output_gene_expression_lines$EntityClass = entityClass }
     current_r_output_gene_expression_lines$F1_score_weight = -1
     current_r_output_gene_expression_lines$Outlier_cell_line = "no Info"
     current_r_output_gene_expression_lines$Drug = splitStrings[col_splitString_indexDrug]
     current_r_output_gene_expression_lines$Cell_line = paste(add_inFrontOf_cellline_string,splitStrings[col_splitString_indexCellline],sep='')
     current_r_output_gene_expression_lines$Drug_class = splitStrings[col_splitString_indexDrugClass]
     current_r_output_gene_expression_lines$Plate = paste(add_inFrontOf_plate_string,splitStrings[col_splitString_indexPlate],sep='')
     if (replace_p_by_plate)
     { current_r_output_gene_expression_lines$Plate = gsub("P","Plate.",current_r_output_gene_expression_lines$Plate) }

     current_r_output_gene_expression_lines$Value = Data_for_r_gene_expression[,indexCol]
     current_r_output_gene_expression_lines$Dataset = dataset
     if (exists("minimum_fraction_of_max_f1score"))
     { if (!is.null(minimum_fraction_of_max_f1score)) { current_r_output_gene_expression_lines$Minimum_fraction_of_max_f1score = minimum_fraction_of_max_f1score } }
     if (exists("ea_correlation_parameter"))
     { if (!is.null(ea_correlation_parameter)) { current_r_output_gene_expression_lines$EA_correlation_parameter = ea_correlation_parameter } }
     if (exists("reference_valueType"))
     { if (!is.null(reference_valueType)) { current_r_output_gene_expression_lines$Reference_valueType = reference_valueType } }
     if (exists("correlation_method"))
     { if (!is.null(correlation_method)) { current_r_output_gene_expression_lines$Correlation_method = correlation_method } }
     if (exists("preprocess_data"))
     { if (!is.null(preprocess_data)) { current_r_output_gene_expression_lines$Preprocess_data = preprocess_data } }
     if (exists("decomposition_method"))
     { if (!is.null(decomposition_method)) { current_r_output_gene_expression_lines$Decomposition_method = decomposition_method } }
     if (exists("icasso_permuations_count"))
     { if (!is.null(icasso_permuations_count)) { current_r_output_gene_expression_lines$Icasso_permuations_count = icasso_permuations_count } }
     if (length(r_output_gene_expression_lines)==0) { r_output_gene_expression_lines = current_r_output_gene_expression_lines }
     else { r_output_gene_expression_lines = rbind(r_output_gene_expression_lines,current_r_output_gene_expression_lines) }
  }#End
}#End
}#End

rm(col_splitString_indexDrug)
rm(col_splitString_indexCellline)
rm(col_splitString_indexDrugClass)
rm(col_splitString_indexPlate)
rm(add_inFrontOf_cellline_string)

