{#Begin - Define singleDrug_cluster_validations
  
  Col_names = c("Dataset","IndexCluster","Correlation_method","Preprocess_data","Decomposition_method","Reference_valueType",
                "CumEigenexpression_cutoff",
                "EntityClass","Entity",
                "Eigenassays_count","Eigenexpression_fraction_total",
                "Mean_correlation_reduded_full_data","SD_correlation_reduded_full_data","Median_correlation_reduded_full_data",
                "SigDEGs_correlation_method","Max_sigDEGs_abs_correlation",
                "EA_correlation_parameter",
                "Entity_samples_count","Cluster_samples_count","Entity_in_cluster_samples_count",
                "Recall","Precision","F1_score","Projected_data_type")
  Col_length = length(Col_names)
  Row_names = 1
  Row_length = length(Row_names)
  singleDrug_cluster_validation_base_line = array(NA,dim=c(Row_length,Col_length),dimnames = list(Row_names,Col_names))
  singleDrug_cluster_validation_base_line = as.data.frame(singleDrug_cluster_validation_base_line)
  singleDrug_cluster_validations = c()
  
}#End - Define singleDrug_cluster_validations
