#Copyright 2023. The uploaded code was written by Jens Hansen, working for the Ravi Iyengar lab.
#It is made available under the apache 2 license:
#  The copyright holders for the code are the authors/funders.
#Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
#http://www.apache.org/licenses/LICENSE-2.0
#Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
#
#Please cite our preprint, if you use any of our code:
#
#Multiscale mapping of transcriptomic signatures for cardiotoxic drugs.
#Jens Hansen, Yuguang Xiong, Mustafa  Siddiq, Priyanka Dhanan, Bin Hu, Bhavana Shewali, Arjun  S. Yadaw, Gomathi Jayaraman,
#Rosa Tolentino, Yibang Chen, Pedro Martinez, Kristin G. Beaumont, Robert Sebra, Dusica Vidovic, Stephan C. Schürer,
#Joseph Goldfarb, James Gallo, Marc R. Birtwistle, Eric A. Sobie, Evren U. Azeloglu, Seth Berger, Angel Chan,
#Christoph Schaniel, Nicole C. Dubois,* Ravi Iyengar*.
#Initial version: bioRxiv 2021.11.02.466774; doi: https://doi.org/10.1101/2021.11.02.466774

working_directory = "D:/LINCS_DToxS_SVD/AA_R_code/"
setwd(working_directory)


source("SVD_00_install_missing_packages.R")
#source('SVD_0a_singleCell.R') #Results are supplied to avoid time consumptive sc RNAseq analysis
#source('SVD_0b_HeartCellAtlas_singleCell.R) #Results are supplied to avoid time consumptive sc RNAseq analysis
source('SVD_0___wait_for_C#_script_to_finish.R')
memory_larger_than_16GB = TRUE
if (memory_larger_than_16GB)
{ source('SVD_0c_Correlation_with_GTEx.R') }
source('SVD_0d_sigDEGS.R')
source('SVD_1_subtractFirstSVD_from_DEGs.R')
source('SVD_2_calculate_angles_and_correlations_with_eigenarrays.R')
source('SVD_3_calculate_eigenarraySigDEGsCorrelation_eigenarrayCategorialCorrelations.R')
source('SVD_4_visualize_eigenarraySigDEGsCorrelation_eigenarrayCategorialCorrelations.R')
source('SVD_5_validate_clusters_by_calculating_f1_scores_basedOnDrugSpecificEigenarrays_for_test_sets.R')
source('SVD_6_collapse_validation_F1_scores_and_calculate_pvalues.R')
source('SVD_7_find_outlier_responses.R')
source('SVD_8_validate_clusters_including_coCulture_by_calculating_f1_scores_basedOn_selected_DrugSpecificEigenarrays.R')
source('SVD_9_collapse_validation_F1_scores_and_calculate_pvalues_for_coCulture.R')
source('SVD_10_find_outlier_responses_coCulture.R')
source('SVD_11_visualize_collapsed_F1_scores_basedOnDrugSpecificEigenarray.R')
source('SVD_12_cluster_reduced_data.R')
source('SVD_13_generate_all_heatmaps.R')
source('SVD_14_write_reduced_data.R') 
source('SVD_15_add_regular_drug_treatments_to_coCulture_treatments.R')
source('SVD_16_generate_all_parameterFilesForCsharp.R')
source('SVD_17_report_finished.R')
source('SVD_A___wait_for_C#_script_to_finish.R')
source('SVD_A_A_crossCompare_enrichmentResults.R')
source('SVD_A_B_rocAuc_analysis.R')
source('SVD_A_C_Barplots_singleCell.R')
source('SVD_A_D_report_finished.R')
source('SVD_B___wait_for_C#_script_to_finish.R')
source('SVD_B_a_genomicSCPVariants_perSCP_drug.R')
source('SVD_B_b_analyzeGenomicAssociations.R')
source('SVD_B_c_genomicSCPVariant_counts_per_drug.R')
source('SVD_B_d_compare_with_GWAS.R')
source('SVD_B_e_report_finished.R')





