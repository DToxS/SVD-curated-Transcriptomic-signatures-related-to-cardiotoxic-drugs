This folder contains C# and R code that was used to do the main analyses in our manuscript "Multiscale mapping of transcriptomic signatures for cardiotoxic drugs" that is currently available as a preprint (10.1101/2021.11.02.466774).<br>
<br>
Currently under construction<br>
<br>
--------------------------------------------------------------------------------<br>
<br>
Below is a documentation of the directory structure and all needed experimental and databases files.
The downloaded zip-folder from github contains the first two directories 'AA_csharp_code' and 'AA_R_code'. If unpacking this folder generates a warning about too long file names, rename the zip folder to "S" and retry the unpacking.<br>
All of our datasets can be downloaded from 'https://iyengarlab.org/dtoxs/datasets.php' after selection of 'Datasets used for prediction of transcriptomic and genomic signatures for TKI-induced cardiotoxicity'. The downloaded dataset zip-folder contains all other directories listed below (rename the directory 'GeneDatabases_orthology' to 'GeneDatabases_homology').<br>
Both downloads contain one folder called 'LINCS_DToxS_SVD'. Simply merge these folders and copy the merged folder to your hard drive (e.g. to 'D:' generating 'D:/LINCs_DToxS_SVD/').<br>
The merged folder will contain the following directory structure, missing files have to be added, as described below.<br>
<br>
--Your directory (e.g., 'D:/LINCs_DToxS_SVD/')<br>
----AA_csharp_code<br>
------1 C# solution file ('LINCS_DToxS_SVD.sln') and 1 subdirectory<br>
----AA_R_code<br>
------52 R-code files<br>
<br>
----Cardiomyopathy_genomics<br>
------Harper_2021_suppl_table_2.txt<br>
------Pirruccello_2020_suppl_dataFile_3.txt<br>
<br>
----Downloaded_datasets<br>
------Chaffin_2022_singleNucleus_DCMvsNF.txt (optional, see below)<br>
------Chaffin_2022_singleNucleus_HCMvsNF.txt (optional, see below)<br>
------Chun_2023_downregulated_in_LVIP.txt (optional, see below)<br>
------Chun_2023_upregulated_in_LVIP.txt (optional, see below)<br>
------Koenig_2022_singleCell_DCMvsHealthy.txt (optional, see below)<br>
------Litvinukova_2020_cellsAdultHumanHeart_global_raw.h5ad (optional, see below)<br>
<br>
----Experimental_data (all datasets are part of zip-folder at 'https://iyengarlab.org/dtoxs/datasets.php')<br>
------Degs_initial_iPSCdCMs_EC<br>
--------8 files with edgeR DEGs<br>
------Degs_initial_iPSCdCMs_P0<br>
--------266 files with edgeR DEGs<br>
------Metadata<br>
--------Drug_metadata.txt<br>
--------FAERS_risk_profiles.txt<br>
--------Lincs_experimental_metadata.txt<br>
--------Lincs_experimental_metadata_EC_cocultures.txt<br>
--------LincsDrugDosage_harmonized_names.txt<br>
------RARG_variant<br>
--------RARG_variant.txt<br>
------Whole_genome_sequencing<br>
--------annotated_combined.hg38_multianno_reheadered_withGWAS_withRSids.txt<br>
<br>
----GeneDatabases_homology<br>
------Download<br>
--------All_Data_gene_info.txt<br>
--------gene2refseq.txt<br>
--------homologene.data<br>
--------HOM_AllOrganism.rpt<br>
--------ncbiRefSeq.All.hg38.gtf<br>
<br>
----iPSCdCM_scRNAseq (all datasets are part of zip-folder at 'https://iyengarlab.org/dtoxs/datasets.php')<br>
------MSN01-3_control<br>
--------raw_feature_bc_matrix<br>
----------barcodes.tsv.gz<br>
----------features.tsv.gz<br>
----------matrix.mtx.gz<br>
------MSN02-4_control<br>
--------raw_feature_bc_matrix<br>
----------barcodes.tsv.gz<br>
----------features.tsv.gz<br>
----------matrix.mtx.gz<br>
------MSN08-13_control<br>
--------raw_feature_bc_matrix<br>
----------barcodes.tsv.gz<br>
----------features.tsv.gz<br>
----------matrix.mtx.gz<br>
------MSN09-4_control<br>
--------raw_feature_bc_matrix<br>
----------barcodes.tsv.gz<br>
----------features.tsv.gz<br>
----------matrix.mtx.gz<br>
<br>
------Libraries_for_enrichment<br>
--------Download<br>
----------Asp_2019_PMID_31835037_heartDevelopmentalCells.txt<br>
----------ChEA_2022.txt<br>
----------ENCODE_TF_ChIP_seq_2015.txt<br>
----------full database.xml<br>
----------GTEx_Analysis_v6p_RNA-seq_RNA-SeQCv1.1.8_gene_median_rpkm_mod.txt<br>
----------HuGE_phenopedia.txt<br>
----------KEA_2015.txt<br>
----------MBCO_v1.1_gene-SCP_associations_human.txt<br>
----------MBCO_v1.1_SCP_hierarchy.txt<br>
----------TRANSFAC_and_JASPAR_PWMs.txt<br>
----------TRRUST_Transcription_Factors_2019.txt<br>
----------Tucker_2020_marker_genes_PMID 32403949 _suppl_table4.txt<br>
<br>
------Results<br>
--------ScSnRNAseq_enrichment (all datasets are part of zip-folder at 'https://iyengarlab.org/dtoxs/datasets.php')<br>
----------44 files with enrichment results, 1 file with references<br>
<br>
------Results_to_run_R_SVD1to17_in_isolation<br>
--------SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue<br>
----------1_DEGs<br>
------------Deg_summary.txt<br>
------------SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue_topall.txt<br>
--------SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue<br>
----------1_DEGs<br>
------------Deg_summary.txt<br>
------------SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_topall.txt<br>
--------Read_me.txt<br>
--------Report_finished_1st_part_by_Csharp.txt<br>
<br>
The indicated files can be downloaded from the following links. Since some files might have been updated and modified after we downloaded them, we also indicate our download dates.<br>
<br>
---------------------------------------------<br>
<br>
Initial LINCs datasets processed by pipeline:<br>
Select 'Datasets used for prediction of transcriptomic and genomic signatures for TKI-induced cardiotoxicity' at 'https://iyengarlab.org/dtoxs/datasets.php' and unzip the downloaded file. Copy the directory 'LINCs_DToxS_SVD' to a your hard drive. Copy the code directories 'AA_csharp_code' and 'AA_R_code' obtained from this gitHub page into that directory.<br>
<br>
---------------------------------------------<br>
<br>
Subdirectory: ../Libraries_for_enrichment/Download/<br>
https://go.drugbank.com/releases/latest: Download 'All drugs', unzip and copy the file 'full database.xml' into the subdirectory specified above.<br>
(our download date: 2024 January 15)<br>
https://maayanlab.cloud/Enrichr/#libraries: Download the libraries 'ChEA_2022.txt', 'ENCODE_TF_ChIP-seq_2015.txt', 'KEA_2015.txt', 'TRANSFAC_and_JASPAR_PWMs.txt', 'TRRUST_Transcription_Factors_2019.txt' and copy the files into the subdirectory specified above.<br>
(our download date: 2024 January 15)<br>
Rename the 'ENCODE_TF_ChIP-seq_2015.txt' into 'ENCODE_TF_ChIP_seq_2015.txt' (i.e., replace the hyphen by underline).<br>
https://github.com/SBCNY/Molecular-Biology-of-the-Cell/tree/master/MBCO_datasets: Download the datasets 'MBCO v1.1 gene-SCP associations human.txt' and 'MBCO v1.1 SCP hierarchy.txt' into the subdirectory specified above.<br>
(our download date: 2024 January 15)<br>
https://www.gtexportal.org/home/downloads/adult-gtex#bulk_tissue_expression: Select 'Adult GTEx' and download the 'GTEx Analysis V6p' dataset 'GTEx_Analysis_v6p_RNA-seq_RNA-SeQCv1.1.8_gene_median_rpkm.gct.gz'. Unzip the file and delete the first two lines in that dataset using a text editor such as JujuEdit or NotePad and copy the new file with a new name 'GTEx_Analysis_v6p_RNA-seq_RNA-SeQCv1.1.8_gene_median_rpkm_mod.txt' (i.e., after addition of '_mod' and change of the file extension to '.txt') into the subdirectory specified above.<br>
(our download date: 2016 August 26)<br>
https://doi.org/10.1161/CIRCULATIONAHA.119.045401 or PMID:32403949: Download the 'supplemental tables (2).xlsx', copy paste the content of the sheet 'Table 4 IV MarkerGene' into a new document and delete the last line (1559). Delete all empty columns to the right of the last filled column (e.g., columns K - AG). If this step is skipped, the C# script might throw an exception due to existing columns that have the same column names (i.e. an empty string, ""). Save the new document as 'text (tab-delimited) (\*.txt)' with the new name 'Tucker_2020_marker_genes_PMID 32403949 _suppl_table4.txt' into the subdirectory specified above.<br>
https://doi.org/10.1016/j.cell.2019.11.025 or PMID:31835037: Download the supplemental table file '1-s2.0-S0092867419312826-mmc3.xlsx'. Merge the content of all sheets ('Cluster 1 - 14') into a new document. Ensure that the new document contains the headline only in the first line and does not contain any empty lines (including at the bottom of the file).  Delete all empty columns to the right of the last filled column (e.g., columns H - AG). If this step is skipped, the C# script might throw an exception due to existing columns that have the same column names (i.e. an empty string, ""). Save the new document as 'text (tab-delimited) (\*.txt)' with the new name 'Asp_2019_PMID_31835037_heartDevelopmentalCells.txt' into the subdirectory specified above.<br>
HuGE_phenopedia.txt: Download 'GENES TO PHENOTYPE' from 'https://hpo.jax.org/app/data/annotations', rename the file to 'HuGE_phenopedia.txt' and copy the file into the subdirectory specified above.<br>
(our file was generated after downloading "Disease-GeneID.txt" from the HuGe Phenopedia website (download: 2020 June 04))<br>
<br>
---------------------------------------------<br>
<br>
Subdirectory: ../GeneDatabases_homology/<br>
https://ftp.ncbi.nlm.nih.gov/gene/DATA/: Download the file 'gene2refseq.gz', unzip, add the extension '.txt' and copy the file into the specified folder.<br>
(our download: 2018June01)<br>
https://ftp.ncbi.nlm.nih.gov/gene/DATA/GENE_INFO/: Download the file 'All_Data.gene_info.gz', unzip, rename the file to 'All_Data_gene_info.txt' (i.e. replace the dots within the file name by underlines and add the extension '.txt') and copy into the specified subdirectory.<br>
(our download: 2018June01)<br>
https://ftp.ncbi.nih.gov/pub/HomoloGene/build68/: Download the file 'homologene.data' (right click, select 'Save link as') and copy into the specified subdirectory.<br>
(our download: 2024 January 15)<br>
ncbiRefSeq.All.hg38.gtf: Go to https://genome.ucsc.edu, select Table Browser, make the following selections: "clade: Mammal", "genome: Human", "assembly: Dec. 2013 (GRCh38/hg38)", "group: Genes and Gene Predictions", "track: NCBI RefSeq", "table: RefSeq All (ncbiRefSeq)", "region: genome", "output format: GTF - gene transfer format (limited)", "output filename: ncbiRefSeq.All.hg38.txt", followed by pressing "get output". Copy the downloaded file into the specified folder.<br>
(our download: 2017 March 06)<br>
HOM_AllOrganism.rpt: Go to https://www.informatics.jax.org/orthology.shtml, select 'Download', then 'Vertebrate Homology'. Download the file 'HOM_AllOrganism.rpt' (right click, select 'Save link as'). Copy the file into the subdiretory specified above.<br>
(our download: 2024 January 15)<br>
<br>
---------------------------------------------<br>
<br>
Subdirectory: ../Downloaded_datasets/<br>
The files 'Chun_2023_downregulated_in_LVIP.txt' and 'Chun_2023_upregulated_in_LVIP.txt' were obtained by contacting the authors Chun et al. 2023 (PMID: 36970983).<br>
PMID:35732739 or Chaffin et al 2022: Download the supplemental tables, open the files '2021-02-03277C-ST6.DCMvsNF.xlsx' and '2021-02-03277C-ST7.HCMvsNF.xlsx' and save them as text "(tab-delimited) (\*.txt)" with the new names 'Chaffin_2022_singleNucleus_DCMvsNF.txt' and 'Chaffin_2022_singleNucleus_HCMvsNF.txt' into the specified folder.<br>
PMID: 35959412 or https://www.nature.com/articles/s44161-022-00028-6: Download 'Supplementary Table 21 Healthy_DCM_Pseudo_DE'. On the first sheet delete all lines that do not contain an adjusted p-value (starting with line 22487). Generate a new column 'Cluster' and add 'DCM_vs_healthy_in_cardiomyocytes' to each line. On the sheet 'Fibroblasts' delete all lines with no adjusted pvalue (starting with 21199), add 'DCM_vs_healthy_in_fibroblasts' to each line in the new column 'Cluster'. Copy all fibroblast lines to the first sheet (excluding the header).  Delete all empty columns to the right of the last filled column (e.g., columns E - AG). If this step is skipped, the C# script might throw an exception due to existing columns that have the same column names (i.e. an empty string, ""). Save the document as in 'text (tab-delimited) (\*.txt)' format with the name 'Koenig_2022_singleCell_DCMvsHealthy.txt'. Copy the file into the folder specified above.<br>
https://www.heartcellatlas.org/: Select 'Version 1' in the upper right corner. Download the h5ad object 'Heart_global' and rename it into 'Litvinukova_2020_cellsAdultHumanHeart_global_raw.h5ad' into this folder.<br>
(our download: 2022 August 10)<br>
<br>
These downloads can be skipped, see below.<br>
<br>
---------------------------------------------<br>
<br>
Subdirectory: '../Cardiomyopathy_genomics/'<br>
PMID: 33495597 or Harper et al. Nature Genetics volume 53, pages135–142 (2021): Download the file '41588_2020_764_MOESM3_ESM.xlsx'. Copy the spreadsheet on sheet S2 without the initial description (i.e., rows 5-155) into a new document and save it in 'text (tab-delimited) (\*.txt)' format under the name 'Harper_2021_suppl_table_2.txt' into the specified folder.<br>
PMID: 32382064 or Pirruccello Nature Communications volume 11, Article number: 2254 (2020): Download 'Supplementary Dataset 3', delete the lines with headline functions (i.e. lines 2, 25, 40, 63, 96, 125 and 138) and save it in 'text (tab-delimieted) (\*.txt)' format under the name 'Pirruccello_2020_suppl_dataFile_3.txt' into the specified folder.<br>
<br>
------------------------------------------------------------------------------------------------------<br>
------------------------------------------------------------------------------------------------------<br>
------------------------------------------------------------------------------------------------------<br>
<br>
Run the analysis<br>
<br>
C# script:<br>
Download the Visual Studio Installer (https://visualstudio.microsoft.com/downloads/). Follow the instructions and install Visual Studio and the package 'NET desktop development'.<br>
Open the file 'LINCS_DToxS_SVD.sln' in the 'AA_csharp_code' folder with Visual Studio.<br>
Using the solution explorer (right side of the screen, or if hidden, menu 'View' - 'Solution explorer') open 'LINCS_DToxS_SVD', right click on 'References' and select 'Manage NuGet packages..:'. Select 'Restore' in the upper right corner. After the downloads have been finished, restart Visual Studio.<br>
In the solution explorer select the folder 'Common_classes'. Select the C# script 'Common_classes.cs'. Open the class 'Global_directory_class" and define hard_drive (e.g., "D:/") and major_directory (e.g., "LINCs_DToxS_SVD/").<br>
Open the C# script 'Code_file.cs'.<br>
Depending on the available memory, set "Global_class.Memory_larger_than_16GB = " to "true" or "false". If set to false, algorithms generating suppl. figures 1A and B will be skipped.<br>
Start debugging in the menu 'Debug' or by pressing F5.<br>
<br>
R-scripts:<br>
Install R (https://www.r-project.org/).<br>
Install Rtools (https://cran.r-project.org/bin/windows/Rtools/).<br>
Open the file 'SVD_0000000_main_Run_pipeline.R' with any text or R code editor.<br>
Depending on the available memory, set "memory_larget_than_16GB = " to "TRUE" or "FALSE". If set to false, algorithms generating Suppl. Figures 1A and B will be skipped.
Set the working directory that contains all R code files (e.g, working_directory = "D:/LINCs_DToxS_SVD/AA_R_code/").<br>
Save your changes.<br>
Open the file 'SVD_global_parameter.R' and specify the overall_lincs_directory that contains all subdirectories
(e.g., overall_lincs_directory = "D:/LINCs_DToxS_SVD/").<br>
Next, set the number of available cores for parallel processing in the same file. Save your changes.<br>
Start running the file 'SVD_0000000_main_Run_pipeline.R', by coping it into the R user interface or using the functionalities of any suited R code editor.<br>
R libraries used by our pipeline will be installed by the script 'SVD_00_install_missing_packages.R'.<br>
<br>
C# and R-script process the data in a successive order that is documented in the 'Code_file.sc' of the C# solution. Whenever one script finishes its current analysis part, it will write a file into the results folder. The other script will wait with the next analysis step until it can detect that file. Both scripts check the results directory every 30 min.<br>
<br>
The C# script writes a progress report file into the 'Results' directory. Overall progress information about the running R pipeline can be obtained from the R-console. Additionally, some R-scripts write progress report files into the results folder as well.<br>
<br>
------------------------------------------------------------------------------------------------------<br>
------------------------------------------------------------------------------------------------------<br>
------------------------------------------------------------------------------------------------------<br>
<br>
Output:
- The results folder "./Results/SVD_manuscript_supplTables/" will contain all supplemental tables that contain results generated from the data (Suppl. Tables 3 - 32).<br>
- The results folder "./Results/SVD_manuscript_figures/" will contain PDFs, image files and graphml-files that were used to generate main and supplemental figures.
  Graphml files can be visualized using yED graph editor (https://www.yworks.com/products/yed/download).<br>
<br>
------------------------------------------------------------------------------------------------------<br>
------------------------------------------------------------------------------------------------------<br>
------------------------------------------------------------------------------------------------------<br>
<br>
SKIP of single cell RNAseq analysis:<br>
We assume that most users want to skip the analysis of the sc/sn RNAseq datasets that are very time consumptive. Therefore, we provide the enrichment results for all single cell RNAseq datasets as described under 'Subdirectory: ../Results/ScSnRNAseq_enrichment/'<br>
If you want to include these steps, you have to uncomment the related C# code by deleting '\\' at the beginning of each of the following lines:<br>
\\SingleCellNucleusRNAseq_analysis_class scSnRNAseq = new SingleCellNucleusRNAseq_analysis_class();<br>
\\scSnRNAseq.Do_enrichment_analysis_for_Schaniel_iPSCdCM_singleCell_cardiomyocyte();<br>
\\scSnRNAseq.Do_enrichment_analysis_for_Litvinukova_2020_cellsAdultHumanHeart();<br>
\\scSnRNAseq.Do_enrichment_analysis_for_chaffin_koenig_and_chun_HCM_DCM_vs_NF();<br>
<br>
Similarly, you have to uncomment the related R code in 'SVD_0000000_main_Run_pipeline.R' by deleting '#' at the beginning of each of the following lines:<br>
#source('SVD_0a_singleCell.R')<br>
#source('SVD_0b_HeartCellAtlas_singleCell.R)<br>
<br>
-------------------------------------------------------------------------------------------------------<br>
------------------------------------------------------------------------------------------------------<br>
------------------------------------------------------------------------------------------------------<br>
<br>
System requirements/Performance:<br>
High-performance computer<br>
C# and R-code were developed on a high-performance computer with 40 cores and 384 GB RAM that used Windows 10 Pro as an operating system.<br>
C# code was developed using Microsoft Visual Studio Community 2022 version 17.5.5 (https://visualstudio.microsoft.com/downloads/), R code using R version 4.1.0 and Rstudio 2023.06.1 Build 524.<br>
On this system, the whole pipeline finishes within less than 10 hours.<br>
<br>
Laptop with 16GB memory<br>
Code was optimized to also run on a laptop with 16GB memory and one core. All main and almost all supplemental figures can be reproduced on a similar computer (except Suppl. Figures 1A and B).<br>
Number of cores was set to 1 in the R-file 'SVD_global_parameter.R', "Global_class.Memory_larger_than_16GB = " to "false".<br>
Running the file 'SVD_5_validate_clusters_by_calculating_f1_scores_basedOnDrugSpecificEigenarrays_for_test_sets.R' with only one core can take a few days, extending the time taken by the whole pipeline to finish.<br>
