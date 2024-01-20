/*
Copyright 2023. The uploaded code was written by Jens Hansen, working for the Ravi Iyengar lab.
It is made available under the apache 2 license:
The copyright holders for the code are the authors/funders.
Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

Please cite our preprint, if you use any of our code:

Multiscale mapping of transcriptomic signatures for cardiotoxic drugs.
Jens Hansen, Yuguang Xiong, Mustafa  Siddiq, Priyanka Dhanan, Bin Hu, Bhavana Shewali, Arjun  S. Yadaw, Gomathi Jayaraman,
Rosa Tolentino, Yibang Chen, Pedro Martinez, Kristin G. Beaumont, Robert Sebra, Dusica Vidovic, Stephan C. Schürer,
Joseph Goldfarb, James Gallo, Marc R. Birtwistle, Eric A. Sobie, Evren U. Azeloglu, Seth Berger, Angel Chan,
Christoph Schaniel, Nicole C. Dubois,* Ravi Iyengar*.
Initial version: bioRxiv 2021.11.02.466774; doi: https://doi.org/10.1101/2021.11.02.466774
*/

using System;
using Enrichment;
using Highthroughput_data;
using Common_classes;
using Lincs;
using SingelCellNucleus_RNAseq;
using Gene_databases;
using Ontologies_and_GTEx;
using System.Collections.Generic;

class RNASeq_analysis
{
    static void Main_lincs()
    {
        #region User input needed
        Global_class.Memory_larger_than_16GB = true; //if false, ~ 8GB - 10GB free memory should be available before staring the C# pipeline
                                                     //skipped analysis steps do not produce results that are needed for follow-up algorithms
                                                     //See /Results/Progress_report.txt
        #endregion

        Dictionary<int, bool> run_csharp_parts_dict = new Dictionary<int, bool>();
        run_csharp_parts_dict.Add(0, true); //C#: Onetime preparations of data and libraries for SVD pipeline
        run_csharp_parts_dict.Add(1, true); //C#: LINCs pipeline: 1st part: Generate input files for SVD and additional analyses
                                            //R:  LINCs pipeline 2nd part - R scripts SVD_0 - SVD_17
        run_csharp_parts_dict.Add(2, true); //C#: Process genomics data (two parts: independent and dependent on results of R-SVD part)
        run_csharp_parts_dict.Add(3, true); //C#: LINCs pipeline 3rd part: Enrichment analysis of DEG datasets with/without SVD (Run after Rscript with numbers)
                                            //R:  LINCs pipeline 4th part: R scripts with capital letters: SVD_A_A, SVD_A_B, SVD_A_C will run
        run_csharp_parts_dict.Add(5, true); //C#: LINCs pipeline 5th part: Integrate genomics analysis with outlier responses and identified SCPs
                                            //C#: LINCs pipeline 5th part: Finalize tables for website
                                            //R:  LINCs pipeline 6th part: R scripts with capital letters: SVD_B_a, SVD_B_b, SVD_B_c will run
        run_csharp_parts_dict.Add(7, true); //C#: Collection of Figures, Suppl. Figures and Suppl. Tables

        Report_finished_and_wait_for_r_script_class reportFinishedAndWait = new Report_finished_and_wait_for_r_script_class();

        if (run_csharp_parts_dict.ContainsKey(0))
        {
            #region Onetime preparations of data and libraries for SVD pipeline
            using (Homology_class homology = new Homology_class(new Organism_enum[] { Organism_enum.Mus_musculus, Organism_enum.Homo_sapiens }))
            {
                homology.Generate_new_and_write();
            }
            using (Homology_class homology = new Homology_class(new Organism_enum[] { Organism_enum.Rattus_norvegicus, Organism_enum.Homo_sapiens }))
            {
                homology.Generate_new_and_write();
            }
            Ontology_functions.Generate_all_enrichR_libraries();
            Report_class.WriteLine_in_progress_report("SVD one time preparations - C#: Ontology libraries finished");

            using (Synonym_class synonym = new Synonym_class(Organism_enum.Homo_sapiens))
            { synonym.Generate_de_novo_and_write(); }
            Report_class.WriteLine_in_progress_report("SVD one time preparations - C#: Synonym finished");

            using (NcbiRefSeq_lincs_class ncbi = new NcbiRefSeq_lincs_class())
            { ncbi.Generate_de_novo_and_write(); }
            Report_class.WriteLine_in_progress_report("SVD one time preparations - C#: NcbiRefSeq finished");

            using (Combine_individual_files_to_single_degs_class combine = new Combine_individual_files_to_single_degs_class("DEGenes_iPSCdCMs_P0"))
            { combine.Generate_combined_file_and_write(); }
            using (Combine_individual_files_to_single_degs_class combine_ec = new Combine_individual_files_to_single_degs_class("DEGenes_iPSCdCMs_ECCoCulture"))
            { combine_ec.Generate_combined_file_and_write(); }
            Report_class.WriteLine_in_progress_report("SVD one time preparations - C#: Generation of preliminary DEGs finished");
            Lincs_svd_manuscript_class.Preprocess_degs_for_all_further_analyses_and_write_as_binary();
            Report_class.WriteLine_in_progress_report("SVD one time preparations - C#: Generation of DEGs finished");

            if (Global_class.Memory_larger_than_16GB)
            {
                using (Combine_DEG_replicate_value_class deg_replicates = new Combine_DEG_replicate_value_class("DEGenes_iPSCdCMs_P0"))
                { deg_replicates.Generate_de_novo_and_write(); }
                Lincs_svd_manuscript_class.Preprocess_degs_replicates_for_all_further_analyses_and_write_as_binary();
                Report_class.WriteLine_in_progress_report("SVD one time preparations - C#: Generation of replicate gene lists finished");
            }
            #endregion
        }

        if (run_csharp_parts_dict.ContainsKey(1))
        {
            #region LINCs pipeline: 1st part: Generate input files for SVD and additional analyses
            Lincs_svd_manuscript_class.Write_drugLegend_for_manuscript("DEGenes_iPSCdCMs_P0");
            if (Global_class.Memory_larger_than_16GB)
            {
                string replicate_dataset = "DEGenes_iPSCdCMs_P0_replicateExpression";
                Lincs_svd_manuscript_class.Calculate_overlap_of_gtexGenes_with_absExpression_and_write_instance_for_replicate_clustering(Deg_score_of_interest_enum.Control_expression_values, replicate_dataset);
            }

            string[] datasets = new string[] { "DEGenes_iPSCdCMs_ECCoCulture", "DEGenes_iPSCdCMs_P0" };// "DEGenes_promoCells";//"DEGenes_iPSCdCMs_P0"
            Deg_score_of_interest_enum[] deg_score_of_interests = new Deg_score_of_interest_enum[] { Deg_score_of_interest_enum.Signed_minus_log10pvalue, Deg_score_of_interest_enum.Signed_minus_log10pvalue_no1stSVD };
            foreach (string dataset in datasets)
            {
                foreach (Deg_score_of_interest_enum deg_score_of_interest in deg_score_of_interests)
                {
                    Lincs_svd_manuscript_class.Create_svd_directory(dataset, deg_score_of_interest);
                    if (deg_score_of_interest.Equals(Deg_score_of_interest_enum.Signed_minus_log10pvalue))
                    { Lincs_svd_manuscript_class.Write_degs_for_supplementary_tables_and_svd_eigenarray_removal(dataset, deg_score_of_interest); }
                }
            }
            #endregion

            Report_class.WriteLine_in_progress_report("SVD part 1 - C#: DEGs written for R-SVD finished");

            reportFinishedAndWait.Report_finish_of_first_part();
        }

        #region LINCs pipeline 2nd part - R scripts SVD_0 - SVD_17
        //LINCs pipeline 2nd part: Rscript with numbers: SVD_0 - SVD_15 wil run
        #endregion

        int[] minimum_genomic_qualityAQs = new int[] { 0, 20 };
        if (run_csharp_parts_dict.ContainsKey(2))
        {
            if (!Global_class.Memory_larger_than_16GB) { reportFinishedAndWait.Wait_for_R_to_finish_second_part(); }

            #region LINCs pipeline 2nd part II (runs in parallel to R script mentioned above, if enough memory)
            Lincs_svd_manuscript_class.Generate_shorter_and_use_genomic_vcf_file();
            Report_class.WriteLine_in_progress_report("SVD part 2 II - C#: Generation of initial genomic file finished");
            Lincs_svd_manuscript_class.Prepare_genomic_input_data_and_write_as_cell_line_specific_minor_allel_binary(); // time consumptive
            Report_class.WriteLine_in_progress_report("SVD part 2 II - C#: Generation of minor allel finished");
            Lincs_svd_manuscript_class.Filter_cell_line_specific_minor_allel_binary_by_quality_control_measures(minimum_genomic_qualityAQs);
            Report_class.WriteLine_in_progress_report("SVD part 2 II - C#: Filtering of genomic data by quality control finished");
            Lincs_svd_manuscript_class.Filter_cell_line_specific_minor_allel_binary_by_biological_relevance(minimum_genomic_qualityAQs);
            Report_class.WriteLine_in_progress_report("SVD part 2 II - C#: Filtering of genomic data by biological relevance finished");
            #endregion

            reportFinishedAndWait.Wait_for_R_to_finish_second_part();

            #region LINCs pipeline 2nd part III (needs R results)
            Lincs_svd_manuscript_class.Filter_cell_line_specific_minor_allel_binary_by_keeping_only_celllines_with_highest_minor_allel_frequency(minimum_genomic_qualityAQs);
            Report_class.WriteLine_in_progress_report("SVD part 2 III - C#: Filtering of genomic data by keeping cell lines with highest minor allel frequency finished");
            Lincs_svd_manuscript_class.Generate_genomics_related_to_published_variants_from_filtered_cell_line_specific_minor_allel_binary_after_QC_BR_and_orV(minimum_genomic_qualityAQs);
            Report_class.WriteLine_in_progress_report("SVD part 2 III - C#: Generation of genomics data with published variants finished");
            #endregion
        }

        if (run_csharp_parts_dict.ContainsKey(3))
        {
            #region LINCs pipeline 3rd part: Enrichment analysis of DEG datasets with/without SVD (Run after Rscript with numbers)
            int topDEGs = 600;

            Lincs_svd_manuscript_class.Do_enrichment_on_degs_from_svd_focussing_on_outlier_responses("DEGenes_iPSCdCMs_ECCoCulture", "SVD", "decomposed", topDEGs, Deg_score_of_interest_enum.Signed_minus_log10pvalue_no1stSVD, Roc.DownstreanEnrichment_score_of_interest_enum.Fractional_rank, "decomposed");
            Report_class.WriteLine_in_progress_report("SVD part 3 - C#: Decomposed EC coculture DEGs enrichment finished");
            Lincs_svd_manuscript_class.Do_enrichment_on_degs_from_svd_focussing_on_outlier_responses("DEGenes_iPSCdCMs_ECCoCulture", "SVD", "decomposed", topDEGs, Deg_score_of_interest_enum.Signed_minus_log10pvalue_no1stSVD, Roc.DownstreanEnrichment_score_of_interest_enum.Fractional_rank, "no1stSVD");
            Report_class.WriteLine_in_progress_report("SVD part 3 - C#: No 1st SVD EC coculture DEGs enrichment finished");
            Lincs_svd_manuscript_class.Do_enrichment_on_degs_from_svd_focussing_on_outlier_responses("DEGenes_iPSCdCMs_ECCoCulture", "SVD", "decomposed", topDEGs, Deg_score_of_interest_enum.Signed_minus_log10pvalue_no1stSVD, Roc.DownstreanEnrichment_score_of_interest_enum.Fractional_rank, "complete");
            Report_class.WriteLine_in_progress_report("SVD part 3 - C#: Complete EC coculture DEGs enrichment finished");
            Lincs_svd_manuscript_class.Do_enrichment_on_degs_from_svd_focussing_on_outlier_responses("DEGenes_iPSCdCMs_P0", "SVD", "decomposed", topDEGs, Deg_score_of_interest_enum.Signed_minus_log10pvalue_no1stSVD, Roc.DownstreanEnrichment_score_of_interest_enum.Fractional_rank, "decomposed");
            Report_class.WriteLine_in_progress_report("SVD part 3 - C#: Decomposed iPSCdCM P0 DEGs enrichment finished");
            Lincs_svd_manuscript_class.Do_enrichment_on_degs_from_svd_focussing_on_outlier_responses("DEGenes_iPSCdCMs_P0", "SVD", "decomposed", topDEGs, Deg_score_of_interest_enum.Signed_minus_log10pvalue_no1stSVD, Roc.DownstreanEnrichment_score_of_interest_enum.Fractional_rank, "complete");
            Report_class.WriteLine_in_progress_report("SVD part 3 - C#: Complete iPSCdCM P0 DEGs enrichment finished");
            Lincs_svd_manuscript_class.Do_enrichment_on_degs_from_svd_focussing_on_outlier_responses("DEGenes_iPSCdCMs_P0", "SVD", "decomposed", topDEGs, Deg_score_of_interest_enum.Signed_minus_log10pvalue_no1stSVD, Roc.DownstreanEnrichment_score_of_interest_enum.Fractional_rank, "no1stSVD");
            Report_class.WriteLine_in_progress_report("SVD part 3 - C#: No 1st SVD iPSCdCM P0 DEGs enrichment finished");
            Lincs_svd_manuscript_class.Do_enrichment_of_eigenassays("DEGenes_iPSCdCMs_P0", "SVD", "decomposed", topDEGs, Deg_score_of_interest_enum.Signed_minus_log10pvalue_no1stSVD);

            //SingleCellNucleusRNAseq_analysis_class scSnRNAseq = new SingleCellNucleusRNAseq_analysis_class();
            //scSnRNAseq.Do_enrichment_analysis_for_Schaniel_iPSCdCM_singleCell_cardiomyocyte(); //Results are supplied to avoid time consumptive sc RNAseq analysis
            //scSnRNAseq.Do_enrichment_analysis_for_Litvinukova_2020_cellsAdultHumanHeart(); //Results are supplied to avoid time consumptive sc RNAseq analysis
            //scSnRNAseq.Do_enrichment_analysis_for_chaffin_koenig_and_chun_HCM_DCM_vs_NF(); //Results are supplied to avoid time consumptive sc RNAseq analysis
            #endregion

            reportFinishedAndWait.Report_finish_of_third_part();
        }

        #region LINCs pipeline 4th part - R scripts SVD_A_A, SVD_A_B, SVD_A_C
        //LINCs pipeline 4th part: R scripts with capital letters: SVD_A_A, SVD_A_B, SVD_A_C will run
        #endregion

        reportFinishedAndWait.Wait_for_R_to_finish_fourth_part();

        if (run_csharp_parts_dict.ContainsKey(5))
        {
            #region LINCs pipeline 5th part: Integrate genomics analysis with outlier responses and identified SCPs
            string svd_subdirectory = "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/";
            string side_effect = "Cardiotoxicity";
            int minimum_qualityAQ_outlier = minimum_genomic_qualityAQs[1];
            int minimum_qualityAQ_SCP = minimum_genomic_qualityAQs[0];

            Ontology_type_enum[] drugBank_ontologies = new Ontology_type_enum[] { Ontology_type_enum.Drugbank_drug_targets, Ontology_type_enum.Drugbank_transporters, Ontology_type_enum.Drugbank_enzymes };
            Relation_of_gene_symbol_to_drug_enum[] considered_relations_of_gene_symbol_to_drug = new Relation_of_gene_symbol_to_drug_enum[] {  Relation_of_gene_symbol_to_drug_enum.Drug_target_protein
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Enzyme
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Transporter
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter };

            foreach (Ontology_type_enum drugBank_ontology in drugBank_ontologies)
            {
                Lincs_svd_manuscript_class.Map_drugBank_targets_to_potential_TF_and_kinase_regulators(drugBank_ontology, minimum_qualityAQ_outlier);
                Lincs_svd_manuscript_class.Map_genomic_variants_to_outliers_and_write_results(considered_relations_of_gene_symbol_to_drug, drugBank_ontology, minimum_qualityAQ_outlier, svd_subdirectory);
            }
            Report_class.WriteLine_in_progress_report("SVD part 5 - C#: Mapping of genomic variants to PK/PD finished");

            Lincs_svd_manuscript_class.Extract_variants_associated_with_pharmacokinetics_pharmacodynamics_and_count_number_of_identified_variants(considered_relations_of_gene_symbol_to_drug, minimum_qualityAQ_outlier);
            Lincs_svd_manuscript_class.Compare_identified_mechanisms_with_published_cardiotoxic_and_noncardiotoxic_variants(minimum_qualityAQ_outlier, svd_subdirectory, considered_relations_of_gene_symbol_to_drug);
            Relation_of_gene_symbol_to_drug_enum[] considered_relations_of_gene_symbol_to_drug_for_network = new Relation_of_gene_symbol_to_drug_enum[] {  Relation_of_gene_symbol_to_drug_enum.Drug_target_protein
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Enzyme
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Transporter
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter
                                                                                                                                          ,Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter
        };
            Lincs_svd_manuscript_class.Generate_network_containing_published_variants_involved_in_clinical_drug_toxicity(considered_relations_of_gene_symbol_to_drug_for_network, minimum_qualityAQ_outlier);
            Lincs_svd_manuscript_class.Generate_network_containing_predicted_variants_involved_in_clinical_drug_toxicity_of_selected_drugs_or_drugClasses(new Relation_of_gene_symbol_to_drug_enum[] { Relation_of_gene_symbol_to_drug_enum.Drug_target_protein }, minimum_qualityAQ_outlier, new string[0], new Drug_type_enum[] { Drug_type_enum.Kinase_inhibitor, Drug_type_enum.Monoclonal_antibody }, true);
            Lincs_svd_manuscript_class.Count_genomic_variants_that_map_to_identified_scps(minimum_qualityAQ_SCP, "decomposed", side_effect);
            Lincs_svd_manuscript_class.Generate_drug_scp_with_variants_networks_for_selected_scps(side_effect, minimum_qualityAQ_SCP, "decomposed", Ontology_type_enum.Mbco_level3, new DE_entry_enum[] { DE_entry_enum.Diffrna_down }, new string[] { "Thin myofilament organization" });
            Report_class.WriteLine_in_progress_report("SVD part 5 - C#: Analysis of genomic variants finished");
            #endregion

            #region LINCs pipeline 5th part: Finalize tables for website
            Lincs_svd_manuscript_class.Add_additional_information_to_files_for_website("decomposed");
            Report_class.WriteLine_in_progress_report("SVD part 5 - C#: Website files finished");
            #endregion

            reportFinishedAndWait.Report_finish_of_fifth_part();
        }

        #region LINCs pipeline 6th part - R scripts SVD_B_a, SVD_B_b, SVD_B_c
        //LINCs pipeline 6th part: R scripts with capital letters: SVD_B_a, SVD_B_b, SVD_B_c will run
        #endregion

        reportFinishedAndWait.Wait_for_R_to_finish_sixth_part();

        if (run_csharp_parts_dict.ContainsKey(7))
        {
            #region LINCs pipeline 7th part: Collection of Figures, Suppl. Figures and Suppl. Tables
            Lincs_svd_manuscript_class.Organize_figures_and_supplemental_figures();
            Lincs_svd_manuscript_class.Organize_supplTables();
            Report_class.WriteLine_in_progress_report("SVD part 7 - C#: Collection of Figures, Suppl. Figures and Suppl. Tables finished");
            #endregion
        }
    }

    static void Main()
    {
        Report_class.Write_analysis_start();
        switch (Global_class.Input_data)
        {
            case Input_data_enum.Lincs:
                Main_lincs();
                break;
            default:
                throw new Exception("input data is not considered");
        }
        Report_class.Write_analysis_end();
    }
}
