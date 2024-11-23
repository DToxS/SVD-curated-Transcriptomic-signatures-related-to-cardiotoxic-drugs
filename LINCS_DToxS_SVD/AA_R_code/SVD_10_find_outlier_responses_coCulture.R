#stopCluster(parallel_clusters)
rm(list = ls());

source("SVD_Find_outlier_responses_prepare_tasks.R")

if (coCulture_data_exists)
{#Begin - if (coCulture_data_exists)

tasks$AddOrAnalyze_coCulture_data = TRUE
svd_script_name = "SVD_10_find_outlier_response_coCulture"
source("SVD_Find_outlier_responses.R")

}#End - if (coCulture_data_exists)
  


