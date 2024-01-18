using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ontologies_and_GTEx;
using Gene_databases;
using Highthroughput_data;
using Common_classes;
using ReadWrite;
using Statistic;
using Network;
using Network_visualization;
using Enrichment;
using Adverse_event;
using MBCO;
using FAERS_analysis;
using Roc;
using System.Windows.Forms.VisualStyles;

namespace Lincs
{
    enum Lincs_diseaseNeighborhoods_order_enum { Simple_seed_nodes, Single_seed_nodes, Weighted_seed_nodes_log10p, Weighted_seed_nodes_p };

    class Lincs_svd_manuscript_class
    {
        #region DEGs
        private static Deg_class Get_preprocessed_lincs_degs(out string[] bg_genes, string plate)
        {
            Deg_class deg = new Deg_class();
            deg.Options.DEG_harmonization_between_samples = DEG_harmonization_between_samples_enum.Add_missing_refSeq_genes;
            deg.Options.Add_ncbi_symbols_origin = Add_ncbi_symbols_origin_enum.Ncbi;
            deg.Options.RefSeq_accepted_accessionNumberType = RefSeq_accepted_accessionNumberType_enum.All_onlymRNA;
            deg.Options.Print_options();
            deg.Read_preprocessed_fileName_and_bgGenes(plate);
            bg_genes = deg.Get_all_bg_genes_in_upperCase_and_ordered();
            return deg;
        }
        private static DEG_replicate_value_class Get_preprocessed_lincs_deg_repliate_value_instance(out string[] bg_genes, bool readBinary, params string[] fileNames)
        {
            DEG_replicate_value_class deg = new DEG_replicate_value_class();
            deg.Options.RefSeq_accepted_accessionNumberType = RefSeq_accepted_accessionNumberType_enum.All_onlymRNA;
            if (readBinary)
            {
                foreach (string fileName in fileNames)
                {
                    deg.Read_preprocessed_as_binary_add_to_array_and_set_bg_genes(fileName);
                }
            }
            else
            {
                throw new Exception();
            }
            //deg.Print_summary();
            bg_genes = deg.Get_all_bgSymbols_in_upperCase();
            return deg;

        }
        #endregion

        public static void Write_drugLegend_for_manuscript(string dataset)
        {
            string directory = Global_directory_class.Results_metadata_directory;
            string fileName = "Suppl_table_Drug_metadata.txt";
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();

            Deg_class degs = Get_deg_instance_for_svd(dataset, out _);
            string[] drugs = degs.Get_all_ordered_treatments();

            drug_legend.Keep_only_selected_drugs(drugs);
            drug_legend.Replace_drugType_by_given_drugType(Drug_type_enum.Cardiovascular_drug, Drug_type_enum.Cardiac_acting_drug);
            drug_legend.Replace_drugType_by_given_drugType(Drug_type_enum.Non_cardiovascular_drug, Drug_type_enum.Non_cardiac_acting_drug);
            drug_legend.Write(directory, fileName);

        }
        private static Dictionary<Lincs_genomics_analysis_stage_enum, string[]> Get_stage_referenceCelllineSpecificFileName_dictionary(Lincs_genomics_analysis_stage_enum[] genomic_analysis_stages, Relation_of_gene_symbol_to_drug_enum[] considered_relationships, int minimum_qualityAq)
        {
            Dictionary<Lincs_genomics_analysis_stage_enum, string[]> stage_referenceCelllineSpecificFileName_dict = new Dictionary<Lincs_genomics_analysis_stage_enum, string[]>();
            if (genomic_analysis_stages.Contains(Lincs_genomics_analysis_stage_enum.After_quality_control))
            {
                stage_referenceCelllineSpecificFileName_dict.Add(Lincs_genomics_analysis_stage_enum.After_quality_control, new string[] { Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control(minimum_qualityAq) });
            }
            if (genomic_analysis_stages.Contains(Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance))
            {
                stage_referenceCelllineSpecificFileName_dict.Add(Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance, new string[] { Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance(minimum_qualityAq) });
            }
            if (genomic_analysis_stages.Contains(Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance_overrepresented_variants))
            {
                stage_referenceCelllineSpecificFileName_dict.Add(Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance_overrepresented_variants, new string[] { Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance_considering_only_variants_overrepresented_in_one_cellline(minimum_qualityAq) });
            }
            if (genomic_analysis_stages.Contains(Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance_overrepresented_variants_in_outlier))
            {
                stage_referenceCelllineSpecificFileName_dict.Add(Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance_overrepresented_variants_in_outlier, new string[] { Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance_considering_only_variants_overrepresented_in_one_cellline_that_is_outlier(minimum_qualityAq) });
            }
            List<string> drugMoA_fileNames = new List<string>();
            if (genomic_analysis_stages.Contains(Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms))
            {
                drugMoA_fileNames.Clear();
                foreach (Relation_of_gene_symbol_to_drug_enum considered_relationship in considered_relationships)
                {
                    drugMoA_fileNames.Add(Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_drugMoA_genomics_binary_file(considered_relationship, minimum_qualityAq));
                }
                stage_referenceCelllineSpecificFileName_dict.Add(Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms, drugMoA_fileNames.ToArray());
            }
            if (genomic_analysis_stages.Contains(Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms_outlier))
            {
                drugMoA_fileNames.Clear();
                foreach (Relation_of_gene_symbol_to_drug_enum considered_relationship in considered_relationships)
                {
                    drugMoA_fileNames.Add(Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_drugMoA_genomics_outlier_binary_file(considered_relationship, minimum_qualityAq));
                }
                stage_referenceCelllineSpecificFileName_dict.Add(Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms_outlier, drugMoA_fileNames.ToArray());
            }
            return stage_referenceCelllineSpecificFileName_dict;
        }

        public static void Extract_variants_associated_with_pharmacokinetics_pharmacodynamics_and_count_number_of_identified_variants(Relation_of_gene_symbol_to_drug_enum[] considered_relationships, int minimum_qualityAq)
        {
            Lincs_genomics_analysis_stage_enum[] considered_stages = new Lincs_genomics_analysis_stage_enum[]
            {
              Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms_outlier
            };
            Dictionary<Lincs_genomics_analysis_stage_enum, bool> stage_writeSuppl_dict = new Dictionary<Lincs_genomics_analysis_stage_enum, bool>
            {
                { Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms_outlier, true }
            };
            Dictionary<Lincs_genomics_analysis_stage_enum, string[]> stage_referenceCelllineSpecificFileName_dict = Get_stage_referenceCelllineSpecificFileName_dictionary(considered_stages, considered_relationships, minimum_qualityAq);

            Lincs_vcf_genomic_data_summary_class summary = new Lincs_vcf_genomic_data_summary_class();

            Lincs_vcf_genomic_data_class genomic_data;
            string[] completeFileNames;
            string completeFileName;
            int completeFileNames_length;

            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, string[]> drugGroup_drugs_dict = new Dictionary<string, string[]>();
            Drug_type_enum[] current_drugTypes;
            string[] drugs;
            string results_subdirectory = Global_directory_class.Drug_pk_pd_subdirectory;
            string website_subdirectory = "Website_DEGenes_iPSCdCMs_P0_decomposed_p0.05_Fractional_rank_top600DEGs/";
            drugGroup_drugs_dict.Add("Anthracyclines", drug_legend.Get_all_drugs_of_indicated_types(Drug_type_enum.Anthracycline));
            drugGroup_drugs_dict.Add("cardiotoxic TKIs", drug_legend.Get_all_cardiotoxic_tkis());
            drugGroup_drugs_dict.Add("non-cardiotoxic TKIs", drug_legend.Get_all_non_cardiotoxic_tkis());
            string[] drugGroups = drugGroup_drugs_dict.Keys.ToArray();
            string drugGroup;
            int drugGroups_length = drugGroups.Length;
            Dictionary<string, string[]> drug_drugGroups_dict = new Dictionary<string, string[]>();
            for (int indexDrug = 0; indexDrug < drugGroups_length; indexDrug++)
            {
                drugGroup = drugGroups[indexDrug];
                drugs = drugGroup_drugs_dict[drugGroup];
                if (drugs.Length == 0) { throw new Exception(); }
                foreach (string drug in drugs)
                {
                    if (!drug_drugGroups_dict.ContainsKey(drug))
                    {
                        drug_drugGroups_dict.Add(drug, new string[0]);
                    }
                    drug_drugGroups_dict[drug] = Overlap_class.Get_union(drug_drugGroups_dict[drug], drugGroup);
                }
            }
            string[] all_drugs = drug_legend.Get_all_drugs();
            foreach (string drug in all_drugs)
            {
                if (!drug_drugGroups_dict.ContainsKey(drug)) { drug_drugGroups_dict.Add(drug, new string[0]); }
                drug_drugGroups_dict[drug] = Overlap_class.Get_union(drug_drugGroups_dict[drug], "Total");
            }

            Drug_cards_class drug_cards = new Drug_cards_class();

            foreach (Lincs_genomics_analysis_stage_enum considered_stage in considered_stages)
            {
                completeFileNames = stage_referenceCelllineSpecificFileName_dict[considered_stage];
                completeFileNames_length = completeFileNames.Length;
                genomic_data = new Lincs_vcf_genomic_data_class();
                for (int indexCF = 0; indexCF < completeFileNames_length; indexCF++)
                {
                    completeFileName = completeFileNames[indexCF];
                    genomic_data.Read_binary_and_add_to_array(completeFileName);
                }
                summary.Generate_from_genomic_data_and_add_to_array(genomic_data, drug_drugGroups_dict, considered_stage);
                if (stage_writeSuppl_dict.ContainsKey(considered_stage))
                {
                    genomic_data.Replace_drugAbbreviation_by_fullDrugName_and_add_drugType();
                    genomic_data.Write_as_supplemental_table_and_for_website(results_subdirectory, "Genomic_variants_" + considered_stage + "_supplTable.txt");
                    if (considered_stage.Equals(Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms_outlier))
                    {
                        genomic_data.Write_as_supplemental_table_and_for_website(website_subdirectory, "Potential genomic variants influencing drug PK or PD.txt");
                        drug_cards.Generate_from_genomic_variants_mapping_to_pharmakodynamic_mechnisms(genomic_data);
                    }
                }
            }
            drug_cards.Write(website_subdirectory, "DrugCards_genomicVariants_pd.txt", 600);
            summary.Write(results_subdirectory, "Identified_variants_count_minAQ" + minimum_qualityAq + ".txt");
        }

        public static void Compare_identified_mechanisms_with_published_cardiotoxic_and_noncardiotoxic_variants(int minimum_qualityAq, string svd_subdirectory, Relation_of_gene_symbol_to_drug_enum[] considered_relationships)
        {
            Lincs_cardiotoxic_variant_class cardiotoxic_variants_mapped_to_all_genomics = new Lincs_cardiotoxic_variant_class();
            cardiotoxic_variants_mapped_to_all_genomics.Generate_by_reading_input();

            Deg_summary_class deg_summary = new Deg_summary_class();
            deg_summary.Read(svd_subdirectory + "1_DEGs/", "DEG_summary.txt");
            string results_subdirectory = "SVD_drug_target_proteins/";
            string results_singleDrug_files_subdirectory = results_subdirectory + "Genomics_filtered/";
            Lincs_genomics_analysis_stage_enum[] considered_stages = new Lincs_genomics_analysis_stage_enum[] { Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms_outlier };
            Dictionary<Lincs_genomics_analysis_stage_enum, string[]> stage_referenceCelllineSpecificFileName_dict = Get_stage_referenceCelllineSpecificFileName_dictionary(considered_stages, considered_relationships, minimum_qualityAq);

            Lincs_cardiotoxic_variant_class stageSpecific_cardiotoxic_variants_mapped_to_all_genomics;
            Lincs_cardiotoxic_variant_class combined_cardiotoxic_variants = new Lincs_cardiotoxic_variant_class();

            Lincs_genomics_analysis_stage_enum[] documentation_stages = stage_referenceCelllineSpecificFileName_dict.Keys.ToArray();
            Lincs_genomics_analysis_stage_enum documentation_stage;
            int documentation_stages_length = documentation_stages.Length;
            string[] fileNames;
            Lincs_vcf_genomic_data_class genomics;
            for (int indexDS = 0; indexDS < documentation_stages_length; indexDS++)
            {
                documentation_stage = documentation_stages[indexDS];
                fileNames = stage_referenceCelllineSpecificFileName_dict[documentation_stage];
                genomics = new Lincs_vcf_genomic_data_class();
                foreach (string fileName in fileNames)
                {
                    genomics.Read_binary_and_add_to_array(fileName);
                }
                genomics.Check_for_duplicates(results_subdirectory);

                stageSpecific_cardiotoxic_variants_mapped_to_all_genomics = cardiotoxic_variants_mapped_to_all_genomics.Deep_copy();

                stageSpecific_cardiotoxic_variants_mapped_to_all_genomics.Add_and_override_cellline_specific_variants(documentation_stage, genomics, results_singleDrug_files_subdirectory);
                combined_cardiotoxic_variants.Add_deep_copy_of_other(stageSpecific_cardiotoxic_variants_mapped_to_all_genomics);
                stageSpecific_cardiotoxic_variants_mapped_to_all_genomics.Write(results_singleDrug_files_subdirectory, "Cardiotoxic_variants_own_cell_line_variants_minAQ" + minimum_qualityAq + "_" + documentation_stage + ".txt");
            }
            combined_cardiotoxic_variants.Check_for_duplicates_after_population_with_genomics_results(results_singleDrug_files_subdirectory);
            combined_cardiotoxic_variants.Set_count_values_for_summary();
            combined_cardiotoxic_variants.Set_cellline_treated_with_at_least_one_clinical_effect_drug(deg_summary);
            combined_cardiotoxic_variants.Write(results_singleDrug_files_subdirectory, "Cardiotoxic_variants_own_cell_line_variants_minAQ" + minimum_qualityAq + "_allStages.txt");
        }

        public static void Generate_network_containing_published_variants_involved_in_clinical_drug_toxicity(Relation_of_gene_symbol_to_drug_enum[] considered_relationships, int minimum_qualityAq)
        {
            string results_singleDrug_files_subdirectory = Global_directory_class.Results_genomics_filtered_subdirectory;
            Lincs_cardiotoxic_variant_class combined_cardiotoxic_variants = new Lincs_cardiotoxic_variant_class();
            combined_cardiotoxic_variants.Read(results_singleDrug_files_subdirectory, "Cardiotoxic_variants_own_cell_line_variants_minAQ" + minimum_qualityAq + "_Only_drug_related_mechanisms_outlier.txt");
            combined_cardiotoxic_variants.Keep_only_selected_relationships_of_gene_symbol_to_drugTarget(considered_relationships);
            combined_cardiotoxic_variants.Keep_only_variants_that_are_identical_in_cellline_and_published();
            combined_cardiotoxic_variants.Keep_only_variants_that_are_related_to_at_least_one_indicated_clinical_risks_drugs(new string[] { "DOX", "DAU", "EPI", "IDA", "TRS" });
            combined_cardiotoxic_variants.Keep_only_selected_population_rsIdentifiers(new string[] { "rs2229774", "rs17863783", "rs7853758" }); //RARG: "rs2229774", UGT1A: "rs17863783", SLC28A3: "rs7853758"
            NetworkBasis_class nw = combined_cardiotoxic_variants.Generate_network_connecting_drugs_to_identified_variants();

            RegularNW_generate_visualization_class visu_make = new RegularNW_generate_visualization_class();
            visu_make.Options.Include_legend = false;
            visu_make.Options.Node_label = Regular_node_label_enum.Name1;
            visu_make.Options.Node_shape = Regular_node_shape_enum.Ellipse;
            visu_make.Options.Node_color = Regular_node_color_enum.Selected_color;

            Visualization_of_nw_basis visu = visu_make.Generate_visualization_instance(nw);
            yED_class yed = new yED_class();
            yed.Write_yED_file(visu, "Regulatory_nw_minAQ" + minimum_qualityAq);
        }

        public static void Generate_network_containing_predicted_variants_involved_in_clinical_drug_toxicity_of_selected_drugs_or_drugClasses(Relation_of_gene_symbol_to_drug_enum[] considered_relationships, int minimum_qualityAq, string[] drugs, Drug_type_enum[] drug_types, bool cardiotoxic_drugs)
        {
            string results_subdirectory = Global_directory_class.Drug_pk_pd_subdirectory;
            Lincs_genomics_analysis_stage_enum[] considered_stages = new Lincs_genomics_analysis_stage_enum[] { Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms_outlier
                                                                                                              };
            Dictionary<Lincs_genomics_analysis_stage_enum, string[]> stage_referenceCelllineSpecificFileName_dict = Get_stage_referenceCelllineSpecificFileName_dictionary(considered_stages, considered_relationships, minimum_qualityAq);

            NetworkBasis_class network;

            Lincs_genomics_analysis_stage_enum[] documentation_stages = stage_referenceCelllineSpecificFileName_dict.Keys.ToArray();
            Lincs_genomics_analysis_stage_enum documentation_stage;
            int documentation_stages_length = documentation_stages.Length;
            string[] fileNames;
            Lincs_vcf_genomic_data_class genomics;

            StringBuilder sb = new StringBuilder();
            for (int indexDS = 0; indexDS < documentation_stages_length; indexDS++)
            {
                documentation_stage = documentation_stages[indexDS];
                fileNames = stage_referenceCelllineSpecificFileName_dict[documentation_stage];
                genomics = new Lincs_vcf_genomic_data_class();
                foreach (string fileName in fileNames)
                {
                    genomics.Read_binary_and_add_to_array(fileName);
                }
                genomics.Check_for_duplicates(results_subdirectory);
                genomics.Keep_only_lines_with_indicated_relationsGeneSymbolsToDrugTargets(considered_relationships);
                sb.Clear();
                if (drugs.Length > 0)
                {
                    genomics.Keep_only_lines_with_indicated_drugs_ignoring_case(drugs);
                    foreach (string drug in drugs)
                    {
                        if (sb.Length > 0) { sb.AppendFormat("_"); }
                        sb.AppendFormat(drug);
                    }
                }
                else
                {
                    genomics.Keep_only_lines_with_indicated_drug_types(drug_types);
                    genomics.Keep_only_lines_with_TKIs_and_indicated_cardiotoxicity(cardiotoxic_drugs);
                    foreach (Drug_type_enum drugType in drug_types)
                    {
                        if (sb.Length > 0) { sb.AppendFormat("_"); }
                        sb.AppendFormat(drugType.ToString());
                    }
                }

                network = genomics.Generate_network_considering_all_lines();

                RegularNW_generate_visualization_class visu_make = new RegularNW_generate_visualization_class();
                visu_make.Options.Include_legend = false;
                visu_make.Options.Node_label = Regular_node_label_enum.Name1;
                visu_make.Options.Node_shape = Regular_node_shape_enum.Ellipse;
                visu_make.Options.Node_color = Regular_node_color_enum.Selected_color;


                Visualization_of_nw_basis visu = visu_make.Generate_visualization_instance(network);
                yED_class yed = new yED_class();
                yed.Write_yED_file(visu, "Regulatory_nw_" + sb.ToString() + "_" + documentation_stage + "_minAQ" + minimum_qualityAq);
            }
        }
        public static DE_class Get_all_drug_targets_for_Lincs_drugs_with_fullNameAtColIndex0_abbreviationAtColIndex1(Ontology_type_enum ontology)
        {
            #region Read drug legend and generate drug_drugFullName_dict
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, string> drugFullName_drug_dict = drug_legend.Get_drugFullName_drug_dict();
            #endregion

            Deg_drug_legend_class drug_legends = new Deg_drug_legend_class();
            drug_legends.Generate_de_novo();

            string[] all_fullDrugNames = drug_legends.Get_all_drug_fullnames();
            string drug_fullName;
            int all_fullDrugNames_length = all_fullDrugNames.Length;

            Ontology_library_class drugBank_library = new Ontology_library_class();
            drugBank_library.Generate_by_reading(ontology);

            string[] current_drugTargets;
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();

            List<string> drug_not_in_drugBank = new List<string>();

            for (int indexF = 0; indexF < all_fullDrugNames_length; indexF++)
            {
                drug_fullName = all_fullDrugNames[indexF];
                if (drugFullName_drug_dict.ContainsKey(drug_fullName))
                {
                    switch (drug_fullName)
                    {
                        case "delavirdine":
                            drug_fullName = "Delavirdine";
                            break;
                        case "cyclosporine":
                            drug_fullName = "Cyclosporine";
                            break;
                        case "cefuroxime":
                            drug_fullName = "Cefuroxime";
                            break;
                        default:
                            break;
                    }
                    current_drugTargets = drugBank_library.Get_all_ordered_unique_gene_symbols_of_input_scps_if_they_exist(drug_fullName);
                    if ((current_drugTargets.Length == 0) && (drug_fullName.Equals("endothelin-1")) && (ontology.Equals(Ontology_type_enum.Drugbank_drug_targets)))
                    {
                        current_drugTargets = new string[] { "EDNRA", "EDNRB" };
                    }
                    else if ((current_drugTargets.Length == 0) && (drug_fullName.Equals("Cyclosporine")) && (ontology.Equals(Ontology_type_enum.Drugbank_drug_targets)))
                    {
                        current_drugTargets = new string[] { "CAMLG", "PPP3R2", "PPIA", "PPIF" };
                    }
                    if (current_drugTargets.Length == 0)
                    {
                        drug_not_in_drugBank.Add(drug_fullName); //Adds all drugs that are part of the legend, not only those of the current iPSCd CM data.
                    }
                    foreach (string current_drugTarget in current_drugTargets)
                    {
                        fill_de_line = new Fill_de_line_class
                        {
                            Names_for_de = new string[] { (string)drug_fullName.ToLower() },
                            Symbols_for_de = new string[] { (string)current_drugTarget.Clone() },
                            Value_for_de = 1,
                            Timepoint_for_de = Timepoint_enum.E_m_p_t_y,
                            Entry_type_for_de = DE_entry_enum.E_m_p_t_y
                        };
                        fill_de_list.Add(fill_de_line);
                    }
                }
            }

            //ReadWriteClass.WriteArray_into_directory(drug_not_in_drugBank.ToArray(), Global_directory_class.Results_directory, "Drugs_not_found_in_" + ontology + ".txt");
            //Writes all drugs in the legend, not only those of the current iPSCd CM data.

            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list.ToArray());

            int col_count = de.ColChar.Columns.Count;
            string[] new_names;
            for (int indexCol = 0; indexCol < col_count; indexCol++)
            {
                new_names = new string[2];
                new_names[0] = de.ColChar.Columns[indexCol].Names[0].ToLower();
                new_names[1] = (string)drugFullName_drug_dict[new_names[0]].Clone();
                de.ColChar.Columns[indexCol].Names = new_names;
            }
            return de;
        }


        #region One time preparations
        public static void Preprocess_degs_for_all_further_analyses_and_write_as_binary()
        {
            string[] plates = new string[] { "DEGenes_iPSCdCMs_ECCoCulture.txt",
                                             "DEGenes_iPSCdCMs_P0.txt"
                                           };
            string plate;
            int plates_length = plates.Length;
            for (int indexP = 0; indexP < plates_length; indexP++)
            {
                plate = plates[indexP];
                using (Deg_class deg = new Deg_class())
                {
                    deg.Options.RefSeq_accepted_accessionNumberType = RefSeq_accepted_accessionNumberType_enum.All_onlymRNA;
                    deg.Options.DEG_harmonization_between_samples = DEG_harmonization_between_samples_enum.Add_missing_refSeq_genes;
                    deg.Options.Add_ncbi_symbols_origin = Add_ncbi_symbols_origin_enum.Ncbi;
                    deg.Options.Print_options();
                    deg.Generate_by_reading_safed_files_and_process(Lincs_molecularEntity_enum.Rna, plate);
                    deg.Write_preprocessed_as_binary(plate);
                }
            }
        }
        public static void Preprocess_degs_replicates_for_all_further_analyses_and_write_as_binary()
        {
            string[] plates = new string[] { "DEGenes_iPSCdCMs_P0_replicateExpression.txt"};
            string plate;
            int plates_length = plates.Length;
            for (int indexP = 0; indexP < plates_length; indexP++)
            {
                plate = plates[indexP];
                DEG_replicate_value_class deg = new DEG_replicate_value_class();
                deg.Options.RefSeq_accepted_accessionNumberType = RefSeq_accepted_accessionNumberType_enum.All_onlymRNA;
                deg.Generate_by_reading_safed_files_and_process(plate);
                deg.Write_preprocessed_as_binary(plate);
            }
        }
        #endregion

        #region Genomic data
        public static void Generate_shorter_and_use_genomic_vcf_file()
        {
            string complete_inputFileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomic_input_file();
            string complete_shorter_outputFileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_short_genomic_input_file();
            string complete_outputFileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_use_genomic_input_file();
            System.IO.StreamWriter writer = new System.IO.StreamWriter(complete_outputFileName);
            System.IO.StreamWriter writer_shorter = new System.IO.StreamWriter(complete_shorter_outputFileName);
            System.IO.StreamReader reader = new System.IO.StreamReader(complete_inputFileName);
            string inputLine;
            int readLines_count = 0;
            string headline = reader.ReadLine();
            headline = headline.Replace(".", "_");
            headline = headline.Substring(0, headline.Length - 1) + "Publication";
            writer.WriteLine(headline);
            writer_shorter.WriteLine(headline);
            string[] columns;
            string previous_input_line = "";
            int columns_count = -1;
            while ((inputLine = reader.ReadLine()) != null)
            {
                columns = inputLine.Split(Global_class.Tab);
                if (columns.Length == 1)
                {
                    previous_input_line += ";" + inputLine;
                }
                else
                {
                    if (columns_count == -1) { columns_count = columns.Length; }
                    else if (columns_count != columns.Length)
                    {
                        writer_shorter.WriteLine(previous_input_line);
                        throw new Exception();
                    }
                    readLines_count++;
                    writer.WriteLine(previous_input_line);
                    if (readLines_count < 10000)
                    {
                        writer_shorter.WriteLine(previous_input_line);
                    }
                    previous_input_line = (string)inputLine.Clone();
                }
            }
            writer.WriteLine(previous_input_line);
            writer.Close();
            writer_shorter.Close();
            reader.Close();
        }
        public static void Prepare_genomic_input_data_and_write_as_cell_line_specific_minor_allel_binary()
        {
            Lincs_vcf_genomic_data_class genomics = new Lincs_vcf_genomic_data_class();
            string seth_input_file = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_use_genomic_input_file();
            string cellline_genomics_completeFileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_before_quality_control();
            genomics.Generate_after_reading_and_write_binary(seth_input_file, cellline_genomics_completeFileName, Global_directory_class.Drug_pk_pd_subdirectory);
        }
        public static void Filter_cell_line_specific_minor_allel_binary_by_quality_control_measures(int[] minimum_quality_aqs)
        {
            string complete_destination_fileName;
            int minimum_quality_aqs_length = minimum_quality_aqs.Length;
            int minimum_quality_aq;
            for (int indexMinimumQuality = 0; indexMinimumQuality < minimum_quality_aqs_length; indexMinimumQuality++)
            {
                minimum_quality_aq = minimum_quality_aqs[indexMinimumQuality];
                complete_destination_fileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control(minimum_quality_aq);
                if (System.IO.File.Exists(complete_destination_fileName)) {  System.IO.File.Delete(complete_destination_fileName); }
            }
            string complete_fileName_before_qc = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_before_quality_control();

            Lincs_vcf_genomic_data_class genomics = new Lincs_vcf_genomic_data_class();
            string[] chromosomes = genomics.Get_all_chromsomes_from_binary(complete_fileName_before_qc);
            string chromosome;
            int chromosomes_length = chromosomes.Length;
            for (int indexC=0; indexC<chromosomes_length; indexC++)
            {
                chromosome = chromosomes[indexC];
                genomics.Read_binary_either_allLines_or_lines_with_specified_chromosomes(complete_fileName_before_qc, chromosome);
                minimum_quality_aqs = minimum_quality_aqs.Distinct().OrderBy(l => l).ToArray();
                for (int indexMinimumQuality = 0; indexMinimumQuality < minimum_quality_aqs_length; indexMinimumQuality++)
                {
                    minimum_quality_aq = minimum_quality_aqs[indexMinimumQuality];
                    genomics.Options.Aq_minimum = minimum_quality_aq;
                    genomics.Filter_data_by_quality_control_cutoffs();
                    genomics.Write_binary(Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control(minimum_quality_aq), System.IO.FileMode.Append);
                }
            }
        }
        public static void Filter_cell_line_specific_minor_allel_binary_by_biological_relevance(int[] minimum_quality_aqs)
        {
            Lincs_vcf_genomic_data_class genomics;
            minimum_quality_aqs = minimum_quality_aqs.Distinct().OrderByDescending(l => l).ToArray();
            int minimum_quality_aqs_length = minimum_quality_aqs.Length;
            int minimum_quality_aq;
            string input_complete_fileName;
            string output_complete_fileName;
            string[] chromosomes;
            string chromosome;
            int chromosomes_length;

            for (int indexMinimumQuality = 0; indexMinimumQuality < minimum_quality_aqs_length; indexMinimumQuality++)
            {
                minimum_quality_aq = minimum_quality_aqs[indexMinimumQuality];

                input_complete_fileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control(minimum_quality_aq);
                output_complete_fileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance(minimum_quality_aq);
                if (System.IO.File.Exists(output_complete_fileName)) { System.IO.File.Delete(output_complete_fileName); }
                genomics = new Lincs_vcf_genomic_data_class();
                chromosomes = genomics.Get_all_chromsomes_from_binary(input_complete_fileName);
                chromosomes_length = chromosomes.Length;
                for (int indexChrom=0; indexChrom<chromosomes_length; indexChrom++)
                {
                    chromosome = chromosomes[indexChrom];
                    genomics.Read_binary_either_allLines_or_lines_with_specified_chromosomes(input_complete_fileName,chromosome);
                    genomics.Filter_data_by_biologicalRelevance_cutoffs();
                    genomics.Write_binary(output_complete_fileName, System.IO.FileMode.Append);
                }
            }
        }
        public static void Filter_cell_line_specific_minor_allel_binary_by_keeping_only_celllines_with_highest_minor_allel_frequency(int[] minimum_quality_aqs)
        {
            Lincs_vcf_genomic_data_class genomics;
            minimum_quality_aqs = minimum_quality_aqs.Distinct().OrderByDescending(l => l).ToArray();
            int minimum_quality_aqs_length = minimum_quality_aqs.Length;
            int minimum_quality_aq;

            Outlier_class outlier = new Outlier_class();
            outlier.Options.Max_adj_pvalue = -1F;
            outlier.Options.Max_pvalue = 0.1F;
            outlier.Options.Minimum_f1score_without_outlier = 0.5F;
            outlier.Generate_by_reading("SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/5_Cluster_validation_f1_scores/", "Outlier_responses.txt");

            string input_complete_fileName;
            string overrepresented_output_complete_fileName;
            string outlier_output_complete_fileName;
            string[] chromosomes;
            string chromosome;
            int chromosomes_length;

            string[] outlier_celllines = outlier.Get_all_outlier();

            for (int indexMinimumQuality = 0; indexMinimumQuality < minimum_quality_aqs_length; indexMinimumQuality++)
            {
                minimum_quality_aq = minimum_quality_aqs[indexMinimumQuality];
                genomics = new Lincs_vcf_genomic_data_class();
                input_complete_fileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance(minimum_quality_aq);
                chromosomes = genomics.Get_all_chromsomes_from_binary(input_complete_fileName);
                chromosomes_length = chromosomes.Length;
                overrepresented_output_complete_fileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance_considering_only_variants_overrepresented_in_one_cellline(minimum_quality_aq);
                outlier_output_complete_fileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance_considering_only_variants_overrepresented_in_one_cellline_that_is_outlier(minimum_quality_aq);
                if (System.IO.File.Exists(overrepresented_output_complete_fileName)) { System.IO.File.Delete(overrepresented_output_complete_fileName); }
                if (System.IO.File.Exists(outlier_output_complete_fileName)) { System.IO.File.Delete(outlier_output_complete_fileName); }
                for (int indexChrom=0; indexChrom< chromosomes_length; indexChrom++)
                {
                    chromosome = chromosomes[indexChrom];
                    genomics.Read_binary_either_allLines_or_lines_with_specified_chromosomes(input_complete_fileName, chromosome);
                    genomics.Filter_data_by_keeping_only_variants_that_are_overrepresented_in_only_one_cell_line();
                    genomics.Write_binary(overrepresented_output_complete_fileName, System.IO.FileMode.Append);
                    genomics.Keep_only_lines_with_indicated_cell_lines(outlier_celllines);
                    genomics.Write_binary(outlier_output_complete_fileName, System.IO.FileMode.Append);
                }

            }
        }
        public static void Generate_genomics_related_to_published_variants_from_filtered_cell_line_specific_minor_allel_binary_after_QC_BR_and_orV(int[] minimum_quality_aqs)
        {
            Lincs_cardiotoxic_variant_class cardiotoxic_variants = new Lincs_cardiotoxic_variant_class();
            cardiotoxic_variants.Generate_by_reading_input();
            string[] all_cardiotoxic_genes = cardiotoxic_variants.Get_all_geneSymbols();

            Lincs_vcf_genomic_data_class genomics = new Lincs_vcf_genomic_data_class();
            minimum_quality_aqs = minimum_quality_aqs.Distinct().OrderByDescending(l => l).ToArray();
            int minimum_quality_aqs_length = minimum_quality_aqs.Length;
            int minimum_quality_aq;

            Outlier_class outlier = new Outlier_class();
            outlier.Options.Max_adj_pvalue = -1F;
            outlier.Options.Max_pvalue = 0.1F;
            outlier.Options.Minimum_f1score_without_outlier = 0.5F;
            outlier.Generate_by_reading("SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/5_Cluster_validation_f1_scores/", "Outlier_responses.txt");

            for (int indexMinimumQuality = 0; indexMinimumQuality < minimum_quality_aqs_length; indexMinimumQuality++)
            {
                minimum_quality_aq = minimum_quality_aqs[indexMinimumQuality];
                genomics = new Lincs_vcf_genomic_data_class();
                genomics.Read_binary_either_allLines_or_lines_with_specified_chromosomes(Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance_considering_only_variants_overrepresented_in_one_cellline(minimum_quality_aq));
                genomics.Keep_only_lines_with_indicated_symbols(all_cardiotoxic_genes);
                genomics.Write_binary(Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_publishedGenes_genomics_binary_file_after_after_QC_BR_and_orV(minimum_quality_aq), System.IO.FileMode.Create);
                genomics = new Lincs_vcf_genomic_data_class();
            }
        }
        public static void Map_drugBank_targets_to_potential_TF_and_kinase_regulators(Ontology_type_enum drugBank_ontology, int minimum_quality_aq)
        {
            string genomics_after_qcAndbr_completeFileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance_considering_only_variants_overrepresented_in_one_cellline(minimum_quality_aq);
            //bool search_for_mfpt_neighbors = true;
            string results_subdirectory = Global_directory_class.Drug_pk_pd_subdirectory;

            Outlier_class outlier = new Outlier_class();
            outlier.Options.Max_adj_pvalue = 1000;
            outlier.Options.Max_pvalue = 1000;
            outlier.Options.Minimum_f1score_without_outlier = 0;
            outlier.Generate_by_reading("SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/5_Cluster_validation_f1_scores/", "Outlier_responses.txt");
            string[] all_drugs = outlier.Get_all_entities();

            Ontology_type_enum[] transcription_factor_ontologies = new Ontology_type_enum[] { Ontology_type_enum.Chea_2022,
                                                                                              Ontology_type_enum.Encode_tf_chip_seq_2015,
                                                                                              Ontology_type_enum.Transfac_and_jaspar_pwms,
                                                                                              Ontology_type_enum.Trrust_transcription_factors_2019 };
            //Double TF 'SCPs' are ignored


            Ontology_type_enum[] kinase_ontologies = new Ontology_type_enum[] { Ontology_type_enum.Kea_2015 };

            Ontology_library_class combined_tf = new Ontology_library_class();
            foreach (Ontology_type_enum tf_ontology_type in transcription_factor_ontologies)
            {
                Ontology_library_class tf_ontology = new Ontology_library_class();
                tf_ontology.Generate_by_reading(tf_ontology_type);
                tf_ontology.Check_if_all_scps_are_single_terms_using_space_and_underline_delimiter();
                combined_tf.Add_other(tf_ontology);
            }
            combined_tf.Remove_duplicates_based_on_scp_gene_target_symbols();
            combined_tf.Write_to_results_directory(results_subdirectory, "Transcription_factors_used.txt");

            Ontology_library_class combined_kinase = new Ontology_library_class();
            foreach (Ontology_type_enum kinase_ontology_type in kinase_ontologies)
            {
                Ontology_library_class kinase_ontology = new Ontology_library_class();
                kinase_ontology.Generate_by_reading(kinase_ontology_type);
                combined_kinase.Add_other(kinase_ontology);
            }
            combined_kinase.Write_to_results_directory(results_subdirectory, "Kinases_used.txt");

            #region Read drug legend and generate drug_drugFullName_dict
            int indexDrugFullName_in_de_instances = 0;
            int indexDrugAbbreviation_in_de_instance = 1;
            string drug;
            //string fullDrug_name;
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, string> drug_drugFullName_dict = drug_legend.Get_drug_drugFullName_dict();
            #endregion

            #region Read and filter lincs genomics data
            Lincs_vcf_genomic_data_class genomics = new Lincs_vcf_genomic_data_class();
            genomics.Read_binary_either_allLines_or_lines_with_specified_chromosomes(genomics_after_qcAndbr_completeFileName);
            int all_drugs_length = all_drugs.Length;
            string[] all_drug_fullNames = new string[all_drugs_length];
            for (int indexAll = 0; indexAll < all_drugs_length; indexAll++)
            {
                drug = all_drugs[indexAll];
                all_drug_fullNames[indexAll] = (string)drug_drugFullName_dict[drug].Clone();
            }
            Lincs_vcf_genomic_data_class current_genomics;
            #endregion

            #region Define relation to drug enums
            Relation_of_gene_symbol_to_drug_enum relation_to_drug_tf;
            Relation_of_gene_symbol_to_drug_enum relation_to_drug_kinase;
            Relation_of_gene_symbol_to_drug_enum relation_to_drug_itself;
            switch (drugBank_ontology)
            {
                case Ontology_type_enum.Drugbank_enzymes:
                    relation_to_drug_tf = Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme;
                    relation_to_drug_kinase = Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme;
                    relation_to_drug_itself = Relation_of_gene_symbol_to_drug_enum.Enzyme;
                    break;
                case Ontology_type_enum.Drugbank_drug_targets:
                    relation_to_drug_tf = Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein;
                    relation_to_drug_kinase = Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein;
                    relation_to_drug_itself = Relation_of_gene_symbol_to_drug_enum.Drug_target_protein;
                    break;
                case Ontology_type_enum.Drugbank_transporters:
                    relation_to_drug_tf = Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter;
                    relation_to_drug_kinase = Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter;
                    relation_to_drug_itself = Relation_of_gene_symbol_to_drug_enum.Transporter;
                    break;
                default:
                    throw new Exception();
            }

            #endregion

            #region Get de drugTargets and add drug abbreviations
            DE_class de_drugTargets = Get_all_drug_targets_for_Lincs_drugs_with_fullNameAtColIndex0_abbreviationAtColIndex1(drugBank_ontology);
            de_drugTargets.Write_file(results_subdirectory, "Drug_target_proteins.txt");
            if (indexDrugFullName_in_de_instances != 0) { throw new Exception(); }
            if (indexDrugAbbreviation_in_de_instance != 1) { throw new Exception(); }
            de_drugTargets.Keep_only_columns_with_stated_zeroth_names(all_drug_fullNames);
            de_drugTargets.Write_file(results_subdirectory, "Drug_target_proteins_of_drugs_in_genomics.txt");
            string[] drugTarget_fullDrugNames = de_drugTargets.ColChar.Get_all_zeroth_names();
            string[] missing_drug_fullNames = Overlap_class.Get_part_of_list1_but_not_of_list2(all_drug_fullNames, drugTarget_fullDrugNames);
            ReadWriteClass.WriteArray<string>(missing_drug_fullNames, Global_directory_class.Results_directory + results_subdirectory + "Drugs_in_genomics_missing_in_" + drugBank_ontology + ".txt");
            #endregion

            string[] columnNames;
            int col_count = de_drugTargets.ColChar.Columns.Count;
            string[] drugTargetProteins;

            Lincs_vcf_genomic_data_class currentDrug_genomics;
            Lincs_vcf_genomic_data_class combined_genomics_drugTargets = new Lincs_vcf_genomic_data_class();
            Lincs_vcf_genomic_data_class combined_genomics_TFs = new Lincs_vcf_genomic_data_class();
            Lincs_vcf_genomic_data_class combined_genomics_kinases = new Lincs_vcf_genomic_data_class();

            for (int indexCol = 0; indexCol < col_count; indexCol++)
            {
                drugTargetProteins = de_drugTargets.Get_all_symbols_with_non_zero_entries_at_stated_columns(indexCol);
                columnNames = de_drugTargets.ColChar.Columns[indexCol].Get_names();
                drug = columnNames[indexDrugAbbreviation_in_de_instance];
                currentDrug_genomics = genomics.Shallow_copy();

                #region Find drug target proteins with genomic variations
                current_genomics = currentDrug_genomics.Shallow_copy();
                current_genomics.Keep_only_lines_with_indicated_symbols(drugTargetProteins);
                current_genomics = current_genomics.Deep_copy();
                current_genomics.Keep_only_lines_with_indicated_drugTargetProteins_as_symbols_and_add_drug_ontology_and_relationToGeneSymbol(drug, drugBank_ontology, relation_to_drug_itself, drugTargetProteins);
                if (current_genomics.Genomic_data.Length > 0)
                {
                    current_genomics.Check_for_correct_drugTargetProtein_drug_assignment(de_drugTargets);
                    combined_genomics_drugTargets.Add_deep_copy_of_other(current_genomics);
                }
                #endregion

                #region Find transcription factors with genomic variation that regulate drug target proteins
                foreach (string drugTargetProtein in drugTargetProteins)
                {
                    string[] tfs = combined_tf.Get_all_ordered_unique_scps_targeting_inputSymbol(drugTargetProtein);
                    current_genomics = currentDrug_genomics.Shallow_copy();
                    current_genomics.Keep_only_lines_with_indicated_symbols(tfs);
                    current_genomics = current_genomics.Deep_copy();
                    current_genomics.Keep_only_lines_with_indicated_tfs_or_kinases_as_symbols_and_add_drug_scp_ontology_and_relationToGeneSymbol(drug, drugTargetProtein, drugBank_ontology, relation_to_drug_tf, tfs);
                    if (current_genomics.Genomic_data.Length > 0)
                    {
                        current_genomics.Check_for_correct_drugTargetProtein_drug_assignment(de_drugTargets);
                        combined_genomics_TFs.Add_deep_copy_of_other(current_genomics);
                    }
                }
                #endregion

                #region Find kinases with genomic variation that regulate drug target proteins
                foreach (string drugTargetProtein in drugTargetProteins)
                {
                    string[] kinases = combined_kinase.Get_all_ordered_unique_scps_targeting_inputSymbol(drugTargetProtein);
                    current_genomics = currentDrug_genomics.Shallow_copy();
                    current_genomics.Keep_only_lines_with_indicated_symbols(kinases);
                    current_genomics = current_genomics.Deep_copy();
                    current_genomics.Keep_only_lines_with_indicated_tfs_or_kinases_as_symbols_and_add_drug_scp_ontology_and_relationToGeneSymbol(drug, drugTargetProtein, drugBank_ontology, relation_to_drug_kinase, kinases);
                    if (current_genomics.Genomic_data.Length > 0)
                    {
                        current_genomics.Check_for_correct_drugTargetProtein_drug_assignment(de_drugTargets);
                        combined_genomics_TFs.Add_deep_copy_of_other(current_genomics);
                    }
                }
                #endregion
            }

            combined_genomics_drugTargets.Check_for_correct_drugTargetProtein_drug_assignment(de_drugTargets);
            combined_genomics_drugTargets.Write_binary(Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_drugMoA_genomics_binary_file(Relation_of_gene_symbol_to_drug_enum.Drug_target_protein, drugBank_ontology, minimum_quality_aq), System.IO.FileMode.Create);
            combined_genomics_TFs.Check_for_correct_drugTargetProtein_drug_assignment(de_drugTargets);
            combined_genomics_TFs.Write_binary(Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_drugMoA_genomics_binary_file(Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein, drugBank_ontology, minimum_quality_aq), System.IO.FileMode.Create);
            combined_genomics_kinases.Check_for_correct_drugTargetProtein_drug_assignment(de_drugTargets);
            combined_genomics_kinases.Write_binary(Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_drugMoA_genomics_binary_file(Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein, drugBank_ontology, minimum_quality_aq), System.IO.FileMode.Create);

        }
        public static void Count_genomic_variants_that_map_to_identified_scps(int minimum_quality_aq, string dataType, string sideEffect)
        {
            string genomics_after_qcAndbr_completeFileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance(minimum_quality_aq);
            float max_pvalue = 0.05F;
            int topDEGs = 600;
            float penalty = 0.5F;
            float f1_beta = 0.25F;

            string website_results_subdirectory = "Website_DEGenes_iPSCdCMs_P0_decomposed_p" + max_pvalue + "_Fractional_rank_top" + topDEGs + "DEGs/";

            string overall_enrichment_subdirectory = Global_directory_class.Svd_enrichment_subdirectory + "DEGenes_iPSCdCMs_P0_enrichment_maxP" + max_pvalue + "_top" + topDEGs + "DEGs_in_" + dataType + "_rocForFractional_rank/";
            string overall_enrichment_directory = Global_directory_class.Results_directory + overall_enrichment_subdirectory;
            string variant_counts_subdirectory = overall_enrichment_subdirectory;
            string[] enrichment_fileNames = new string[] { Ontology_abbreviation_class.Get_abbreviation_of_ontology(Ontology_type_enum.Mbco_level1) + ".txt",
                                                           Ontology_abbreviation_class.Get_abbreviation_of_ontology(Ontology_type_enum.Mbco_level2) + ".txt",
                                                           Ontology_abbreviation_class.Get_abbreviation_of_ontology(Ontology_type_enum.Mbco_level3) + ".txt",
                                                           Ontology_abbreviation_class.Get_abbreviation_of_ontology(Ontology_type_enum.Mbco_level4) + ".txt"};
            string scp_summary_fileName = sideEffect + "_SCP_summaries_betaF1_beta" + f1_beta + "_penalty" + penalty + ".txt";
            string scp_summary_with_variantCounts_fileName = sideEffect + "_SCP_summaries_betaF1_beta" + f1_beta + "_penalty" + penalty + "_plusVariantCounts_minAQ" + minimum_quality_aq + ".txt";
            string variantCounts_foreach_drug_fileName = sideEffect + "_SCP_variant_counts_foreach_drug_beta" + f1_beta + "_penalty" + penalty + "_minAQ" + minimum_quality_aq + ".txt";
            string variants_foreach_drug_supplTable_fileName = "SupplTable_" + sideEffect + "_SCP_variants_foreach_drug_beta" + f1_beta + "_penalty" + penalty + "_minAQ" + minimum_quality_aq + ".txt";
            string variants_foreach_drug_fileName = sideEffect + "_SCP_variants_foreach_drug_beta" + f1_beta + "_penalty" + penalty + "_minAQ" + minimum_quality_aq + ".txt";
            Lincs_scp_summary_after_roc_class scp_summary = new Lincs_scp_summary_after_roc_class();
            scp_summary.Generate_by_reading(overall_enrichment_directory, scp_summary_fileName);
            scp_summary.Keep_only_selected_side_effect(sideEffect);
            scp_summary.Keep_only_scps_that_passed_selection_criteria();

            Ontology_library_class mbco_ontology = new Ontology_library_class();
            mbco_ontology.Generate_by_reading(Ontology_type_enum.Molecular_biology_of_the_cell);

            Enrichment2018_results_class enrichment_results = new Enrichment2018_results_class();
            enrichment_results.Read(overall_enrichment_subdirectory, enrichment_fileNames);
            enrichment_results.Check_for_duplicates();

            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();

            Lincs_vcf_genomic_data_class genomics = new Lincs_vcf_genomic_data_class();
            genomics.Read_binary_either_allLines_or_lines_with_specified_chromosomes(genomics_after_qcAndbr_completeFileName);

            Drug_cards_class drug_cards = new Drug_cards_class();

            Lincs_genomic_variant_counts_foreach_drug_class genomic_variants = new Lincs_genomic_variant_counts_foreach_drug_class();
            genomic_variants.Generate(genomics, enrichment_results, scp_summary);
            genomic_variants.Replace_drugAbbreviation_by_fullDrugName_and_add_drugType();
            genomic_variants.Write_scp_genomic_variants(variant_counts_subdirectory, variants_foreach_drug_fileName);
            genomic_variants.Write_scp_genomic_variants_supplTable(variant_counts_subdirectory, variants_foreach_drug_supplTable_fileName);
            genomic_variants.Write_variant_counts(variant_counts_subdirectory, variantCounts_foreach_drug_fileName);

            drug_cards.Generate_from_genomic_variants_mapping_to_cardiotoxic_pathways_if_cardiotoxic_drug(genomic_variants);
            drug_cards.Generate_from_genomic_variants_mapping_to_cardiotoxic_pathways_if_cardiotoxic_drug_including_variants(genomic_variants);

            Lincs_genomic_variant_counts_foreach_drug_class website_genomic_variants = genomic_variants.Deep_copy();
            website_genomic_variants.Keep_only_selected_ontology_in_selected_variants(Ontology_type_enum.Mbco_level3);
            website_genomic_variants.Merge_same_lines_with_different_drugs();
            website_genomic_variants.Write_scp_genomic_variants_for_website(website_results_subdirectory, "Potential genomic variants influencing " + sideEffect + "_pathways.txt");

            scp_summary.Add_count_of_genomic_variants_per_pathway_and_count_of_variants_per_sideEffect_entryType_association(genomics, mbco_ontology);
            scp_summary.Write(overall_enrichment_directory, scp_summary_with_variantCounts_fileName);
            Ontology_type_enum[] ontologies = scp_summary.Get_all_ontologies();
            foreach (Ontology_type_enum ontology in ontologies)
            {
                Lincs_scp_summary_after_roc_class ontology_scp_summary = scp_summary.Deep_copy();
                ontology_scp_summary.Keep_only_selected_ontologies(ontology);
                scp_summary.Write(overall_enrichment_directory, scp_summary_with_variantCounts_fileName.Replace(sideEffect, "ST_" + sideEffect + "_" + ontology));
            }


            drug_cards.Write(website_results_subdirectory, "DrugCards_genomicVariants_scps_" + sideEffect + ".txt", topDEGs);
        }
        public static void Generate_drug_scp_with_variants_networks_for_selected_scps(string side_effect, int minimum_quality_aq, string dataType, Ontology_type_enum ontology, DE_entry_enum[] entryTypes, params string[] scps)
        {
            float max_pvalue = 0.05F;
            int topDEGs = 600;
            float penalty = 0.5F;
            float f1_beta = 0.25F;

            string overall_enrichment_subdirectory = "SVD_outlier_responses/DEGenes_iPSCdCMs_P0_enrichment_maxP" + max_pvalue + "_top" + topDEGs + "DEGs_in_" + dataType + "_rocForFractional_rank/";
            string variant_counts_subdirectory = overall_enrichment_subdirectory;
            string variantCounts_foreach_drug_fileName = side_effect + "_SCP_variants_foreach_drug_beta" + f1_beta + "_penalty" + penalty + "_minAQ" + minimum_quality_aq + ".txt";
            Lincs_genomic_variant_counts_foreach_drug_class genomic_variants = new Lincs_genomic_variant_counts_foreach_drug_class();
            genomic_variants.Read_scp_genomic_variants(variant_counts_subdirectory, variantCounts_foreach_drug_fileName);
            genomic_variants.Keep_only_selected_ontology_in_selected_variants(ontology);

            NetworkBasis_class nw = genomic_variants.Generate_drug_scp_gene_network_for_selected_scps(entryTypes, scps);

            RegularNW_generate_visualization_class visu_make = new RegularNW_generate_visualization_class();
            visu_make.Options.Include_legend = false;
            visu_make.Options.Node_label = Regular_node_label_enum.Name1;
            visu_make.Options.Node_shape = Regular_node_shape_enum.Ellipse;
            visu_make.Options.Node_color = Regular_node_color_enum.Selected_color;

            StringBuilder sb = new StringBuilder();
            foreach (string scp in scps)
            {
                if (sb.Length > 0) { sb.AppendFormat("_"); }
                else { sb.AppendFormat(scp); }
            }
            foreach (DE_entry_enum entryType in entryTypes)
            {
                if (sb.Length > 0) { sb.AppendFormat("_"); }
                else { sb.AppendFormat(entryType.ToString()); }
            }
            Visualization_of_nw_basis visu = visu_make.Generate_visualization_instance(nw);
            yED_class yed = new yED_class();
            yed.Write_yED_file(visu, side_effect + "_" + ontology + "_SCP_gene_nw_beta" + f1_beta + "_penalty" + penalty + sb.ToString() + "_minAQ" + minimum_quality_aq);

        }
        public static void Map_genomic_variants_to_outliers_and_write_results(Relation_of_gene_symbol_to_drug_enum[] considered_relationships, Ontology_type_enum drugBank_ontology, int minimum_qualityAQ, string svd_subdirectory)
        {
            considered_relationships = Lincs_genomics_drugBank_names_class.Keep_only_relationships_mapping_to_selected_ontology(considered_relationships, drugBank_ontology);
            DE_class de_drugTargets = Get_all_drug_targets_for_Lincs_drugs_with_fullNameAtColIndex0_abbreviationAtColIndex1(drugBank_ontology);
            string inputFileName;
            string outputFileName;
            Lincs_vcf_genomic_data_class genomics;
            foreach (Relation_of_gene_symbol_to_drug_enum considered_relationship in considered_relationships)
            {
                inputFileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_drugMoA_genomics_binary_file(considered_relationship, drugBank_ontology, minimum_qualityAQ);
                outputFileName = Lincs_genomics_drugBank_names_class.Get_complete_fileName_of_drugMoA_genomics_outlier_binary_file(considered_relationship, drugBank_ontology, minimum_qualityAQ);
                genomics = new Lincs_vcf_genomic_data_class();
                genomics.Read_binary_and_process(inputFileName);
                genomics.Check_for_correct_drugTargetProtein_drug_assignment(de_drugTargets);
                //genomics.Check_for_duplicates();

                Outlier_class outlier = new Outlier_class();
                outlier.Options.Max_adj_pvalue = -1F;
                outlier.Options.Max_pvalue = 0.1F;
                outlier.Options.Minimum_f1score_without_outlier = 0.5F;
                outlier.Generate_by_reading(svd_subdirectory + "5_Cluster_validation_f1_scores/", "Outlier_responses.txt");

                genomics.Add_outliers_and_keep_only_lines_with_outlier(outlier);
                //genomics.Check_for_duplicates();
                genomics.Check_for_correct_drugTargetProtein_drug_assignment(de_drugTargets);
                genomics.Write_binary(outputFileName, System.IO.FileMode.Create);
            }
        }
        #endregion

        #region Compare base expression with GTEx
        public static void Calculate_overlap_of_gtexGenes_with_absExpression_and_write_instance_for_replicate_clustering(Deg_score_of_interest_enum deg_score_of_interest, string dataset)
        {
            DEG_replicate_value_class deg_replicate_value = Get_deg_replicate_value_instance_for_svd(dataset, out _);
            deg_replicate_value.Keep_only_lines_matching_with_score_of_interest(deg_score_of_interest);

            string subdirectory = "SVD_control_expression/";

            DE_class de_replicate = deg_replicate_value.Generate_new_de_instance(deg_score_of_interest);
            de_replicate.Write_file(subdirectory, "Expression_" + dataset + "_" + deg_score_of_interest + ".txt");
            string[] replicate_symbols = de_replicate.Get_all_symbols_in_current_order();

            GTEx_class gtx = new GTEx_class();
            gtx.Generate();

            DE_class de_gtex = gtx.Generate_de_instance_after_summing_up_duplicates();
            string[] gtex_symbols = de_gtex.Get_all_symbols_in_current_order();
            string[] overlap_symbols = Overlap_class.Get_intersection(replicate_symbols, gtex_symbols);

            de_replicate.Keep_only_stated_symbols(overlap_symbols);
            de_gtex.Keep_only_stated_symbols(overlap_symbols);

            Correlation_class correlation = new Correlation_class();
            correlation.Generate_pairwise_correlations_between_de_instances_and_add_to_array(de_replicate, de_gtex);
            correlation.Calculate_fractional_rank_with_same_group0_based_on_descending_correlation();
            correlation.Write(subdirectory, "Gtex_vs_" + dataset + "_" + deg_score_of_interest + ".txt");
        }
        #endregion

        #region Regular pathway enrichment
        private static Deg_class Get_deg_instance_for_svd(string dataset, out string[] bg_genes)
        {
            string dataset_input = dataset + ".txt";
            Deg_class deg_lincs = Get_preprocessed_lincs_degs(out bg_genes, dataset_input);
            return deg_lincs;
        }

        private static DEG_replicate_value_class Get_deg_replicate_value_instance_for_svd(string dataset, out string[] bg_genes)
        {
            string dataset_input = dataset + ".txt";
            DEG_replicate_value_class deg_replicate_lincs = Get_preprocessed_lincs_deg_repliate_value_instance(out bg_genes, true, dataset_input);
            return deg_replicate_lincs;
        }

        private static void Get_baseName_and_deg_subdirectory(out string baseName, out string deg_subdirectory, string dataset, Deg_score_of_interest_enum deg_score_of_interest)
        {
            baseName = "SVD_" + System.IO.Path.GetFileNameWithoutExtension(dataset) + "_" + deg_score_of_interest;
            deg_subdirectory = baseName + "/1_DEGs/";
        }

        public static void Create_svd_directory(string dataset, Deg_score_of_interest_enum deg_score_of_interest)
        {
            Get_baseName_and_deg_subdirectory(out _, out string subdirectory, dataset, deg_score_of_interest);
            string complete_degdirectory = Global_directory_class.Results_directory + subdirectory;
            ReadWriteClass.Create_directory_if_it_does_not_exist(complete_degdirectory);
        }
        public static void Write_degs_for_supplementary_tables_and_svd_eigenarray_removal(string dataset, Deg_score_of_interest_enum deg_score_of_interest)
        {
            Deg_class deg_lincs = new Deg_class();
            Deg_summary_class deg_summary = new Deg_summary_class();

            DEG_replicate_value_class deg_replicate_value = new DEG_replicate_value_class();
            int degs0_replicates1;

            switch (dataset)
            {
                case "DEGenes_iPSCdCMs_ECCoCulture":
                case "DEGenes_iPSCdCMs_P0":
                    deg_lincs = Get_deg_instance_for_svd(dataset, out _);
                    deg_lincs.Remove_indicated_drugTypes(Drug_type_enum.Offender_plus_mitigator);
                    deg_summary.Generate(deg_lincs);
                    degs0_replicates1 = 0;
                    break;
                case "DEGenes_iPSCdCMs_replicateExpression_P0":
                    deg_replicate_value = Get_deg_replicate_value_instance_for_svd(dataset, out _);
                    deg_replicate_value.Keep_only_lines_matching_with_score_of_interest(deg_score_of_interest);
                    degs0_replicates1 = 1;
                    deg_summary.Generate(deg_replicate_value);
                    break;
                default:
                    throw new Exception();
            }
            Get_baseName_and_deg_subdirectory(out string baseName, out string subdirectory, dataset, deg_score_of_interest);
            DE_class de_all_drugs_unfiltered;
            switch (degs0_replicates1)
            {
                case 0:
                    de_all_drugs_unfiltered = deg_lincs.Generate_new_de_instance(deg_score_of_interest);
                    break;
                case 1:
                    de_all_drugs_unfiltered = deg_replicate_value.Generate_new_de_instance(deg_score_of_interest);
                    break;
                default:
                    throw new Exception();
            }
            de_all_drugs_unfiltered.Order_by_symbol();
            de_all_drugs_unfiltered.Write_file(subdirectory, baseName + "_topall.txt");
            deg_summary.Write(subdirectory, "Deg_summary.txt");

            string svd_initial_data_subdirectory = "SVD_" + dataset + "_supplemental_DEGs/";
            string fda_label = "";
            int max_fractional_rank = 600;
            switch (degs0_replicates1)
            {
                case 0:
                    Deg_average_class deg_average = new Deg_average_class();
                    deg_average.Generate_from_degs_for_each_drug_and_library_prepartion_method_over_celllines_and_plates(deg_lincs, max_fractional_rank);
                    deg_average.Set_entryType_based_on_directionality_of_signedMinusLog10Pvalue();
                    deg_average.Write_average_for_paper(svd_initial_data_subdirectory, "Averaged_drug-induced_DEGs" + fda_label + "_counting_top" + max_fractional_rank + "DEG.txt", max_fractional_rank);
                    deg_lincs.Keep_only_top_lines_with_existing_fractional_rank_below_cutoff(max_fractional_rank);
                    deg_lincs.Set_entryType_based_on_directionality_of_signedMinusLog10Pvalue();
                    deg_lincs.Write_for_paper(svd_initial_data_subdirectory, "Top_" + max_fractional_rank + "_drug-induced_DEGs_per_cell_line" + fda_label + ".txt");
                    break;
                default:
                    throw new Exception();
            }
        }
        public static void Do_enrichment_of_eigenassays(string dataset, string decomposition_method, string processing_method, int topDEGs, Deg_score_of_interest_enum deg_score_of_interest)
        {
            Read_svd_oneEntity_of_entityClass_eachTime_enum read_oneEntityEachTime_of_entityClass = Read_svd_oneEntity_of_entityClass_eachTime_enum.Drug;
            SVD_drug_specific_expression_class svd_drug_expression = new SVD_drug_specific_expression_class();
            svd_drug_expression.Generate_by_readingROutput_calculateFractionalRanks_keepOnlyTopDEGs_and_set_bgGenes(new string[] { dataset }, processing_method, new string[] { "All_eigenassays_including_removed_ones.txt" }, decomposition_method, new Deg_score_of_interest_enum[] { deg_score_of_interest }, read_oneEntityEachTime_of_entityClass, topDEGs, "Eigenassay_1");
            string[] bg_genes = svd_drug_expression.Get_deep_copy_of_bgGenes();

            Downstream_analysis_2020_class enrichment = new Downstream_analysis_2020_class();
            enrichment.Options.Data_value_signs_of_interest = new Data_value_signs_of_interest_enum[] { Data_value_signs_of_interest_enum.Combined };
            enrichment.Options.Ontologies = new Ontology_type_enum[] { Ontology_type_enum.Mbco_level1, Ontology_type_enum.Mbco_level2, Ontology_type_enum.Mbco_level3, Ontology_type_enum.Mbco_level4 };
            enrichment.Options.Write_results = true;
            enrichment.Options.Add_missing_scps_identified_in_other_conditions = false;
            enrichment.Generate(bg_genes);

            string subdirectory = Global_directory_class.Eigenarray_subdirectory;

            svd_drug_expression.Keep_only_lines_with_selected_drug("Eigenassay_1");
            Deg_class deg_eigenassay = svd_drug_expression.Generate_new_deg_instance_assuming_values_are_signed_minusLog10Pvalues_and_plate0(dataset);
            deg_eigenassay.Replace_old_plate_by_new_plate("Plate.0", "Plate.7");
            deg_eigenassay.Write_for_paper(subdirectory, "Supplemental_Table_4 - Top" + topDEGs + "genes_of_first_eigenarray.txt");
            DE_class de = svd_drug_expression.Generate_de_instance();
            Enrichment2018_results_class[] enrich_results = enrichment.Analyse_de_instance_and_return_unfiltered_enrichment_results(de, subdirectory, "_top" + topDEGs + "genes.txt");
            foreach (Enrichment2018_results_class enrich_result in enrich_results)
            {
                Ontology_type_enum ontology = enrich_result.Get_ontology_and_check_if_only_one();
                enrich_result.Write_for_r(subdirectory, "Supplemental_table_5 - " + Ontology_abbreviation_class.Get_abbreviation_of_ontology(ontology) + " for 1st eigenassay.txt");
            }
        }
        public static void Do_enrichment_on_degs_from_svd_focussing_on_outlier_responses(string dataset, string decomposition_method, string processing_method, int topDEGs, Deg_score_of_interest_enum deg_score_of_interest, DownstreanEnrichment_score_of_interest_enum rocScoreOfInterest, string dataType)
        {
            const string dataType_decomposed = "decomposed";
            const string dataType_no1stSVD = "no1stSVD";
            const string dataType_complete = "complete";
            string cell_type = "Cell line";
            switch (dataset)
            {
                case "DEGenes_iPSCdCMs_P0":
                case "DEGenes_iPSCdCMs_ECCoCulture":
                    cell_type = "Cardiomyocyte line";
                    break;
                default:
                    throw new Exception();
            }

            Lincs_rcolor_class rcolor = new Lincs_rcolor_class();
            rcolor.Generate_after_reading(Global_directory_class.Results_directory + decomposition_method + "_" + dataset + "_" + deg_score_of_interest + "/" + "13_ParameterExchangeCsharp/", "RColors.txt");
            rcolor.Write(Global_directory_class.Results_directory, "Mapped_r2csharp_colors.txt");

            Lincs_auc_cutoff_ranks_class auc_cutoff_ranks = new Lincs_auc_cutoff_ranks_class();
            auc_cutoff_ranks.Generate_by_reading(Global_directory_class.Results_directory + decomposition_method + "_" + dataset + "_" + deg_score_of_interest + "/" + "13_ParameterExchangeCsharp/", "AUC_cutoff_ranks.txt");

            float max_pvalue = 0.05F;
            Dictionary<Ontology_type_enum, float> ontology_enrichmentRankCutoffForAuc_dict = auc_cutoff_ranks.Get_ontology_enrichmentRankCutoffForAUC_dict();

            string website_label = "Website";
            string results_subdirectory = Global_directory_class.Svd_enrichment_subdirectory;
            string website_results_subdirectory_base = website_label + "_" + dataset + "_" + dataType + "_p" + max_pvalue + "_" + rocScoreOfInterest;
            string website_results_subdirectory;

            #region Read, prepare svd_drug_expression instance and get drugs, cell lines, bgGenes and max_topDEGsDEPs
            SVD_drug_specific_expression_class svd_drug_expression = new SVD_drug_specific_expression_class();
            int keep_initial_DEGs = -1;
            if (Global_class.Memory_larger_than_16GB) { keep_initial_DEGs = 9999999; }
            else { keep_initial_DEGs = topDEGs; }


            Read_svd_oneEntity_of_entityClass_eachTime_enum readSVD_oneEntityEachTime_of_entityClass = Read_svd_oneEntity_of_entityClass_eachTime_enum.Cell_line;
            switch (dataType)
            {
                case dataType_complete:
                    svd_drug_expression.Generate_by_readingROutput_calculateFractionalRanks_keepOnlyTopDEGs_and_set_bgGenes(new string[] { dataset }, processing_method, new string[] { "Drug_specific_expression_full.txt" }, decomposition_method, new Deg_score_of_interest_enum[] { deg_score_of_interest }, readSVD_oneEntityEachTime_of_entityClass, keep_initial_DEGs);
                    svd_drug_expression.Label_all_outlierCellLine_as_fullData();
                    website_label = "";
                    break;
                case dataType_no1stSVD:
                    svd_drug_expression.Generate_by_readingROutput_calculateFractionalRanks_keepOnlyTopDEGs_and_set_bgGenes(new string[] { dataset }, processing_method, new string[] { "Drug_specific_expression_SVD1removed.txt" }, decomposition_method, new Deg_score_of_interest_enum[] { deg_score_of_interest }, readSVD_oneEntityEachTime_of_entityClass, keep_initial_DEGs);
                    svd_drug_expression.Label_all_outlierCellLine_as_fullData();
                    website_label = " after removal of 1st eigenarray";
                    break;
                case dataType_decomposed:
                    svd_drug_expression.Generate_by_readingROutput_calculateFractionalRanks_keepOnlyTopDEGs_and_set_bgGenes(new string[] { dataset }, processing_method, new string[] { "Drug_specific_expression_" + processing_method + "_F1SW" + "byOutlier" + ".txt" }, decomposition_method, new Deg_score_of_interest_enum[] { deg_score_of_interest }, readSVD_oneEntityEachTime_of_entityClass, keep_initial_DEGs);
                    website_label = " after projection into drug-selective subspaces";
                    break;
                default:
                    throw new Exception();
            }
            string[] bg_genes = svd_drug_expression.Get_deep_copy_of_bgGenes();
            string[] drugs = svd_drug_expression.Get_all_drugs();
            string[] cell_lines = svd_drug_expression.Get_all_celllines();
            svd_drug_expression.Calculate_fractional_ranks_based_on_absolute_values();
            svd_drug_expression.Add_drug_fullNames();
            svd_drug_expression.Add_drug_types();
            svd_drug_expression.Add_cardiotoxicity();
            #endregion

            #region Read and prepare drug_legend and faers instances
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Options.Simplify_drug_classes = false;
            drug_legend.Generate_de_novo();
            drug_legend.Add_missing_cardiotoxicity_from_faers();
            drug_legend.Keep_only_selected_drugs(drugs);
            drug_legend.Write(Global_directory_class.Results_directory + results_subdirectory, "Drug_cardiotoxicities.txt");
            drug_legend.Write(Global_directory_class.Results_directory + results_subdirectory, "Drug_legend_for_leaflets.txt");

            FAERS_class faers = new FAERS_class();
            faers.Generate_by_reading();
            faers.Add_clinical_studies_risk_frequency(drug_legend);
            faers.Keep_only_selected_drugs(drugs);
            //faers.Keep_only_selected_drugTypes(Drug_type_enum.Cardiotoxic_kinase_inhibitor, Drug_type_enum.Noncardiotoxic_kinase_inhibitor, Drug_type_enum.Cardiotoxic_monoclonal_antibody, Drug_type_enum.Noncardiotoxic_monoclonal_antibody);
            faers.Calculate_fractional_ranks_for_odds_ratios_considering_only_input_drug_types(Drug_type_enum.Kinase_inhibitor, Drug_type_enum.Monoclonal_antibody, Drug_type_enum.Anthracycline, Drug_type_enum.Antiproteasome);
            #endregion

            Dictionary<string, string> drug_drugFullName_dict = drug_legend.Get_drug_drugFullName_dict();
            string[] tki_drugs = drug_legend.Get_all_drugs_of_indicated_types(Drug_type_enum.Kinase_inhibitor, Drug_type_enum.Monoclonal_antibody);
            Dictionary<string, System.Drawing.Color> upregulated_drug_color_dict = drug_legend.Get_drug_color_dictionary_based_on_drugType_cardiotoxicity_upregulated();
            Dictionary<string, System.Drawing.Color> downregulated_drug_color_dict = drug_legend.Get_drug_color_dictionary_based_on_drugType_cardiotoxicity_downregulated();

            #region Generate scp_integrationGroups_dict
            Dictionary<string, string[]> integrationGroup_scps_dict = new Dictionary<string, string[]>
            {
                { "Potassium", new string[] { "Potassium transmembrane transport", "Gap junction organization", "Chloride transmembrane transport" } },
                { "DNA", new string[] { "DNA replication, recombination and repair" } },
                { "Contraction", new string[] { "Cellular contraction", "Myofibril formation and organization", "Thin myofilament organization", "Myofibril formation", "Actin filament depolymerization", "Myoglobin synthesis" } },
                { "Redox", new string[] { "Cellular redox homeostasis", "Cellular antioxidant systems" } },
                { "Radiation", new string[] { "Cellular response to radiation" } },
                { "Intermediate_filaments", new string[] { "Epithelial intermediate filament dynamics", "Axonal intermediate filament dynamics", "Neutral and basic keratin dynamics" } },
                { "Adhesion", new string[] { "Cellular adhesion" } },
                { "Cell_cycle", new string[] { "Cell cycle and cell division", "Restriction point" } },
                { "Amino_acids", new string[] { "Amino acid metabolism", "Metabolism of non-essential amino acids", "Serine and glycine metabolism", "Aspartate and arginine metabolism", "Transamination pathways", "GABA metabolism" } },
                {
                    "Signaling",
                    new string[] { "Cellular communication",
                                                                       "Pattern recognition signaling", "Signaling pathways regulating water homeostasis", "Matricellular protein signaling", "Interleukin receptor signaling", "Interferon signaling",
                                                                       "Natriuretic peptide receptor signaling","Adrenergic receptor signaling","Prostaglandin E2 receptor signaling","Hippo signaling","Thyroid hormone receptor signaling",
                                                                       "Granulocyte-colony stimulating factor receptor signaling","Vascular endothelial growth factor receptor signaling","Leptin receptor signaling","Oncostatin-M receptor signaling","HIF-1 receptor signaling pathway","Platelet-derived growth factor receptor signaling","Thyroid hormone receptor signaling",
                                                                       "Erk signaling pathway","C-type natriuretic peptide receptor signaling","Brain natriuretic peptide receptor signaling","Atrial natriuretic peptide receptor signaling","Catecholamine inactivation" }
                },
                { "Degradation_by_lysosomal_enzymes", new string[] { "Degradation by lysosomal enzymes", "Lysosomal glycoprotein degradation" } },
                { "Water_TM", new string[] { "Water transmembrane transport" } },
                { "PT protein modification", new string[] { "Posttranslational protein modification", "Post-translational protein modification and quality control during biosynthetic-secretory pathway" } },
                { "Nucleotide_metabolism", new string[] { "Nucleotide metabolism" } },
                { "Amino_acid_metabolism", new string[] { "Amino acid metabolism" } },
                { "Lipid_metabolism", new string[] { "Lipid metabolism" } },
                { "Energy generation and metabolism of cellular monomers", new string[] { "Energy generation and metabolism of cellular monomers" } },
                { "Regulated_cell_death", new string[] { "Regulated cell death" } },
                { "ECM", new string[] { "Extracellular matrix homeostasis", "Coagulation, fibrinolysis, complement system and blood protein dynamics" } },
                { "Iron", new string[] { "Cellular iron uptake and export" } },
                { "Anthracycline - Iron", new string[] { "Iron, heme and hemoglobin homeostasis", "Cellular iron storage", "Heme degradation to bilirubin" } },
                { "Anthracycline - Mitochondria", new string[] { "Organelle organization", "Mitochondrial dynamics" } },
                { "Anthracycline - Turn over", new string[] { "Posttranslational protein modification", "Cytosolic post-translational modification", "Protein prenylation", "Protein acylation", "Cytosolic protein folding", "Intracellular degradation pathways", "Degradation by lysosomal enzymes", "Lysosomal lipid degradation", "Ubiquitin-mediated proteasomal degradation", "Autophagy", "Chaperone-mediated autophagy", "Protein polyubiquitination", "Proteasomal regulatory particle organization", "Ubiquitin activation", "Ubiquitin ligation", "Ubiquitin conjugation", "Autophagosome elongation" } },
                { "Anthracycline - Cell death", new string[] { "Regulated cell death", "Extrinsic apoptosis pathway", "Necroptosis induction" } },
                { "Anthracycline - DNA damage", new string[] { "Chromatin remodeling", "Nucleotide excision repair", "Nucleosome activation", "RNA editing" } }
            };

            Dictionary<string, bool> addedSCPs_dict = new Dictionary<string, bool>();

            Dictionary<string, List<string>> scp_integrationGroupsList_dict = new Dictionary<string, List<string>>();
            string[] integrationGroups = integrationGroup_scps_dict.Keys.ToArray();
            string[] scps;
            foreach (string integrationGroup in integrationGroups)
            {
                scps = integrationGroup_scps_dict[integrationGroup];
                foreach (string scp in scps)
                {
                    if (!scp_integrationGroupsList_dict.ContainsKey(scp))
                    {
                        scp_integrationGroupsList_dict.Add(scp, new List<string>());
                    }
                    scp_integrationGroupsList_dict[scp].Add(integrationGroup);
                }
            }
            Dictionary<string, string[]> scp_integrationGroups_dict = new Dictionary<string, string[]>();
            scps = scp_integrationGroupsList_dict.Keys.ToArray();
            foreach (string scp in scps)
            {
                scp_integrationGroups_dict.Add(scp, scp_integrationGroupsList_dict[scp].Distinct().OrderBy(l => l).ToArray());
            }
            #endregion

            #region Generate degs_after_decomposition and degs_average_after_decomposition from svd_drug_expression for adverse event
            string supplemental_results_subdirectory = "SVD_" + dataset + "_supplemental_DEGs/";
            Deg_class degs_after_decomposition = svd_drug_expression.Generate_new_deg_instance_assuming_values_are_signed_minusLog10Pvalues_and_plate0(dataset);
            switch (dataset)
            {
                case "DEGenes_iPSCdCMs_ECCoCulture":
                    degs_after_decomposition.Replace_old_plate_by_new_plate("Plate.0", "Plate.1&7");
                    degs_after_decomposition.Replace_old_plate_by_new_plate("Plate.1", "Plate.1&7");
                    break;
                case "DEGenes_iPSCdCMs_P0":
                    degs_after_decomposition.Replace_old_plate_by_new_plate("Plate.0", "Plate.7");
                    break;
                default:
                    throw new Exception();
            }
            degs_after_decomposition.Check_if_only_given_entrytype(DE_entry_enum.Diffrna);

            Deg_average_class degs_average_after_decomposition = new Deg_average_class();
            Deg_average_class degs_average_after_decomposition_website_write = new Deg_average_class();
            if (Global_class.Memory_larger_than_16GB)
            {
                degs_average_after_decomposition.Generate_from_degs_for_each_drug_and_library_prepartion_method_over_celllines_and_plates(degs_after_decomposition, topDEGs);
                degs_average_after_decomposition_website_write = degs_average_after_decomposition.Deep_copy();
                degs_average_after_decomposition_website_write.Set_entryType_based_on_directionality_of_signedMinusLog10Pvalue();
            }

            degs_after_decomposition.Set_entryType_based_on_directionality_of_signedMinusLog10Pvalue();
            degs_after_decomposition.Keep_only_lines_with_nonZero_signedMinusLog10Pvalue();
            degs_after_decomposition.Keep_only_top_lines_with_existing_fractional_rank_below_cutoff(topDEGs);

            if (((dataType.Equals(dataType_decomposed))
                   || (dataType.Equals(dataType_no1stSVD))))
            {
                degs_after_decomposition.Write_for_paper(supplemental_results_subdirectory, "Top_" + topDEGs + "_drug-induced_DEGs_per_cell_line" + website_label + ".txt");
                website_results_subdirectory = website_results_subdirectory_base + "_top" + topDEGs + "DEGs/";
                degs_after_decomposition.Write_for_website(website_results_subdirectory, "Drug-induced DEGs in each cell line.txt", cell_type);

                if (Global_class.Memory_larger_than_16GB)
                {
                    degs_average_after_decomposition_website_write.Write_average_for_paper(supplemental_results_subdirectory, "Averaged_drug-induced_DEGs" + website_label + "_counting_top" + topDEGs + "DEG.txt", topDEGs);

                    Deg_class degs = Get_deg_instance_for_svd(dataset, out string[] intial_bgGenes);
                    degs.Replace_old_plate_by_new_plate("Plate.0", "Plate.7");

                    Lincs_experimental_metadata_class experimental_metadata = new Lincs_experimental_metadata_class();
                    experimental_metadata.Generate(degs_after_decomposition);
                    experimental_metadata.Keep_only_selected_sequencing_run(7);
                    experimental_metadata.Keep_only_selected_drugs(drugs);
                    experimental_metadata.Add_used_replicates_information(degs);
                    experimental_metadata.Write(website_results_subdirectory, "Experimental_metadata.txt");

                    Drug_summary_class drug_summary = new Drug_summary_class();
                    drug_summary.Generate();
                    drug_summary.Keep_only_indicated_drug_IDs(drugs);
                    drug_summary.Write_for_adverseEvent(website_results_subdirectory, "Drug_metadata.txt");

                    degs.Dispose();
                }
            }
            #endregion

            #region Generate svd_drug_expression_enrichment from svd_drug_expression
            SVD_drug_specific_expression_class svd_drug_expression_enrichment = svd_drug_expression.Deep_copy();
            svd_drug_expression_enrichment.Keep_only_lines_with_fractionalRanks_equalOrSmaller_than_selected_rank(topDEGs);
            #endregion

            #region Initialize downstream analysis 2020 instance
            Downstream_analysis_2020_class downstream = new Downstream_analysis_2020_class();
            downstream.Options.Data_value_signs_of_interest = new Data_value_signs_of_interest_enum[] { Data_value_signs_of_interest_enum.Upregulated, Data_value_signs_of_interest_enum.Downregulated };
            downstream.Options.Ontologies = new Ontology_type_enum[] { Ontology_type_enum.Mbco_level1, Ontology_type_enum.Mbco_level2, Ontology_type_enum.Mbco_level3,Ontology_type_enum.Mbco_level4 };
            downstream.Options.Write_results = false;
            downstream.Options.Max_pvalue = max_pvalue;
            downstream.Options.Add_missing_scps_identified_in_other_conditions = false;
            downstream.Generate(bg_genes);
            #endregion

            #region Initialize roc
            Roc_class pairwise_roc = new Roc_class();
            pairwise_roc.Leaflet.Generate_from_drug_legend(drug_legend);
            pairwise_roc.Options.DownstreamEnrichment_score_of_interest = rocScoreOfInterest;
            #endregion

            MBCO_windows_form_input_class mbco_integrationGroups_input = new MBCO_windows_form_input_class(scp_integrationGroups_dict);

            #region Prepare and subject svd_drug_expression_enrichment to enrichment analysis
            svd_drug_expression_enrichment.Keep_only_lines_with_fractionalRanks_equalOrSmaller_than_selected_rank(topDEGs);

            DE_class de = svd_drug_expression_enrichment.Generate_de_instance();

            string enrichment_subdirectory = results_subdirectory + dataset + "_enrichment_maxP" + max_pvalue + "_top" + topDEGs + "DEGs_in_" + dataType + "_rocFor" + pairwise_roc.Options.DownstreamEnrichment_score_of_interest + "/";
            Ontology_type_enum ontology;
            string ontology_string;

            Enrichment2018_results_class[] enrichment_results = downstream.Analyse_de_instance_and_return_unfiltered_enrichment_results(de, enrichment_subdirectory, "");
            #endregion

            foreach (Enrichment2018_results_class enrichment_result in enrichment_results)
            {
                #region Prepare enrichment results for further analysis
                if (max_pvalue != -1)
                { enrichment_result.Keep_only_scps_with_maximium_pvalue(max_pvalue); }
                enrichment_result.Calculate_fractional_ranks_for_scps_based_on_descending_absolute_minusLog10Pvalue();
                enrichment_result.Check_if_all_lines_have_non_empty_entryTypes();
                enrichment_result.Check_for_duplicates();
                #endregion

                #region Write enrichment results for website and for following r-script
                ontology = enrichment_result.Get_ontology_and_check_if_only_one();
                ontology_string = Ontology_abbreviation_class.Get_abbreviation_of_ontology(ontology);
                enrichment_result.Write_for_r(enrichment_subdirectory, "Supplemental_table_" + ontology_string + ".txt");
                website_results_subdirectory = website_results_subdirectory_base + "_top" + topDEGs + "DEGs/";
                Enrichment2018_results_class enrichment_fda = enrichment_result.Deep_copy();
                enrichment_fda.Keep_only_top_x_ranked_scps_based_on_existing_fractional_ranks(ontology_enrichmentRankCutoffForAuc_dict);
                enrichment_fda.Write_for_website(website_results_subdirectory, ontology_string + ".txt", cell_type);
                enrichment_result.Add_missing_scps_that_were_detected_at_least_once_with_pvalue_one_and_indicated_rank(99999, de.Get_DE_class_with_separated_up_and_downregulated_entries(), DE_entry_enum.Diffrna_down, DE_entry_enum.Diffrna_up);
                enrichment_result.Write_for_r(enrichment_subdirectory, ontology_string + ".txt");
                #endregion

                #region Add enrichment results to mbco_integrationGroups_input
                if (dataType.Equals(dataType_decomposed))
                {
                    Enrichment2018_results_class integrationGroup_enrichment = enrichment_result.Deep_copy();
                    integrationGroup_enrichment.Keep_only_top_x_ranked_scps_per_condition(ontology_enrichmentRankCutoffForAuc_dict[ontology]);
                    mbco_integrationGroups_input.GenerateFromEnrichmentData_addIntegrationGroups_and_addToArray(integrationGroup_enrichment, bg_genes, upregulated_drug_color_dict, downregulated_drug_color_dict, ref addedSCPs_dict);
                }
                #endregion

                #region Reset and generate ROCs within pairwise roc and write for R
                Enrichment2018_results_class outlier_enrichment_result;
                if (dataType.Equals("decomposed"))
                {
                    outlier_enrichment_result = enrichment_result.Deep_copy();
                    foreach (Enrichment2018_results_line_class enrichment2018_results_line in outlier_enrichment_result.Enrichment_results)
                    {
                        enrichment2018_results_line.Lincs_drug = enrichment2018_results_line.Lincs_drug + "_" + enrichment2018_results_line.Lincs_outlier;
                    }
                    enrichment_result.Add_other(outlier_enrichment_result);
                }

                pairwise_roc.Clear_rocs();
                pairwise_roc.Generate_from_downstreamEnrichmentResults_significant_and_add_to_array(enrichment_result);
                pairwise_roc.Write(enrichment_subdirectory, "Roc_" + ontology_string + "_top" + topDEGs + "_" + pairwise_roc.Options.DownstreamEnrichment_score_of_interest + ".txt");
                #endregion

                #region Adjust and write pairwise_rocs for website
                string[] splitStrings;
                foreach (Roc_line_class auc_line in pairwise_roc.Roc)
                {
                    int drugs_length = auc_line.New_false_positives.Length;
                    for (int indexD = 0; indexD < drugs_length; indexD++)
                    {
                        splitStrings = auc_line.New_false_positives[indexD].Split(' ');
                        auc_line.New_false_positives[indexD] = drug_drugFullName_dict[splitStrings[0]] + " " + splitStrings[1];
                    }
                    drugs_length = auc_line.New_true_positives.Length;
                    for (int indexD = 0; indexD < drugs_length; indexD++)
                    {
                        splitStrings = auc_line.New_true_positives[indexD].Split(' ');
                        auc_line.New_true_positives[indexD] = drug_drugFullName_dict[splitStrings[0]] + " " + splitStrings[1];
                    }
                }
                pairwise_roc.Keep_only_sideEffect_of_interest("Cardiotoxicity");
                string[] sideEffects = pairwise_roc.Get_all_side_effects();
                if (sideEffects.Length != 1) { throw new Exception(); }
                pairwise_roc.Replace_new_true_and_false_positives_by_unique_terms_plus_term_count();
                pairwise_roc.Keep_only_max_rocCutoff(1000F);
                pairwise_roc.Add_all_previous_drugs_to_added_drugs_and_shorten_cell_line_names();
                pairwise_roc.Write_for_website(website_results_subdirectory, sideEffects[0] + "_of_" + ontology_string + "_SCPs.txt");
                #endregion
            }

            #region Write mbco_integrationGroups
            if (dataType.Equals(dataType_decomposed))
            {
                mbco_integrationGroups_input.Remove_duplicates();
                mbco_integrationGroups_input.Write(Global_directory_class.Get_MBCO_desktop_application_directory(dataset), "SVD_" + dataType + "_" + topDEGs + "topDEGs_dataBasedIntegrationGroups.txt");
                mbco_integrationGroups_input.Write_scpIntegrationGroups_as_mbcoApp_parameter_settings(Global_directory_class.Get_MBCO_desktop_application_directory(dataset));
                downstream.Options.Write_results = false;
            }
            #endregion

            if (Global_class.Memory_larger_than_16GB)
            {
                #region Do enrichment on averaged degs after decomposition and generate and fill preliminary drug cards
                website_results_subdirectory = website_results_subdirectory_base + "_top" + topDEGs + "DEGs/";
                degs_average_after_decomposition.Write_average_for_website(website_results_subdirectory, "Drug-induced DEGs across all cell lines.txt", topDEGs);
                Drug_cards_class drugCards = new Drug_cards_class();
                drugCards.Generate_from_published_risk_profiles_and_add_to_array(drug_legend);
                drugCards.Generate_from_faers_risk_profiles_and_add_to_array(faers);
                drugCards.Generate_from_average_degs_and_add_to_array(degs_average_after_decomposition, topDEGs);
                DE_class de_average = degs_average_after_decomposition.Generate_new_de_instance();
                Enrichment2018_results_class[] website_enrichment_average_results = downstream.Analyse_de_instance_and_return_unfiltered_enrichment_results(de_average, "", "");

                foreach (Enrichment2018_results_class website_enrichment_average_result in website_enrichment_average_results)
                {
                    if (website_enrichment_average_result.Get_ontology_and_check_if_only_one().Equals(Ontology_type_enum.Mbco_level3))
                    {
                        if (max_pvalue != -1)
                        { website_enrichment_average_result.Keep_only_scps_with_maximium_pvalue(max_pvalue); }
                        website_enrichment_average_result.Calculate_fractional_ranks_for_scps_based_on_descending_absolute_minusLog10Pvalue();
                        website_enrichment_average_result.Keep_only_top_x_ranked_scps_based_on_existing_fractional_ranks(ontology_enrichmentRankCutoffForAuc_dict);

                        website_enrichment_average_result.Check_if_all_lines_have_non_empty_entryTypes();
                        website_enrichment_average_result.Check_for_duplicates();
                        ontology = website_enrichment_average_result.Get_ontology_and_check_if_only_one();
                        ontology_string = Ontology_abbreviation_class.Get_abbreviation_of_ontology(ontology);
                        faers.Write_for_fda(Global_directory_class.Results_directory + website_results_subdirectory, "FAERS_risk_profiles.txt");
                        drugCards.Generate_from_average_scps_and_add_to_array(website_enrichment_average_result, topDEGs);
                        drugCards.Add_drug_types_and_fullNames(drug_legend);
                        website_enrichment_average_result.Write_for_website(website_results_subdirectory, "Averaged_drug-induced_" + ontology_string + ".txt", cell_type);
                    }
                }
                drugCards.Write(website_results_subdirectory, "DrugCards_preliminary.txt", topDEGs);
                #endregion
            }
        }
        public static void Add_additional_information_to_files_for_website(string dataType)
        {
            float max_pvalue = 0.05F;
            int topDEGs = 600;
            string cellType = "Cardiomyocyte line";

            string website_subdirectory = "Website_DEGenes_iPSCdCMs_P0_" + dataType + "_p" + max_pvalue + "_Fractional_rank_top" + topDEGs + "DEGs/";
            string website_directory = Global_directory_class.Results_directory + website_subdirectory;
            string supplTables_subdirectory = Global_directory_class.SupplTables_subdirectory;
            string supplTables_directory = Global_directory_class.Results_directory + supplTables_subdirectory;

            float penalty = 0.5F;
            float f1_beta = 0.25F;

            string overall_enrichment_subdirectory = "SVD_outlier_responses/DEGenes_iPSCdCMs_P0_enrichment_maxP" + max_pvalue + "_top" + topDEGs + "DEGs_in_" + dataType + "_rocForFractional_rank/";
            string overall_enrichment_directory = Global_directory_class.Results_directory + overall_enrichment_subdirectory;
            string scp_summary_fileName = "Cardiotoxicity_SCP_summaries_betaF1_beta" + f1_beta + "_penalty" + penalty + "_plusVariantCounts_minAQ0.txt";
            Lincs_scp_summary_after_roc_class scp_summary_website = new Lincs_scp_summary_after_roc_class();
            scp_summary_website.Generate_by_reading(overall_enrichment_directory, scp_summary_fileName);
            scp_summary_website.Keep_only_selected_side_effect("Cardiotoxicity");
            scp_summary_website.Keep_only_selected_association("cardiotoxic TKIs");
            scp_summary_website.Keep_only_selected_ontologies(Ontology_type_enum.Mbco_level3);
            scp_summary_website.Write_for_website(website_directory, "Summary_of_cardiotoxic_pathways.txt");

            Lincs_website_enrichment_class enrichment_results = new Lincs_website_enrichment_class();
            enrichment_results.Generate_by_reading(website_directory, Ontology_abbreviation_class.Get_abbreviation_of_ontology(Ontology_type_enum.Mbco_level3) + ".txt", cellType);
            enrichment_results.Add_scp_cardiotoxicity(scp_summary_website);
            enrichment_results.Write(website_directory, "Drug-induced pathways in each cell line.txt", cellType, false);

            Lincs_website_enrichment_class average_enrichment_results = new Lincs_website_enrichment_class();
            average_enrichment_results.Generate_by_reading(website_directory, "Averaged_drug-induced_" + Ontology_abbreviation_class.Get_abbreviation_of_ontology(Ontology_type_enum.Mbco_level3) + ".txt", cellType);
            average_enrichment_results.Add_scp_cardiotoxicity(scp_summary_website);
            average_enrichment_results.Write(website_directory, "Drug-induced pathways across all cell lines.txt", cellType, true);

            Ontology_library_class mbcoL3 = new Ontology_library_class();
            mbcoL3.Generate_by_reading(Ontology_type_enum.Mbco_level3);

            Lincs_website_scp_genes_class scp_genes = new Lincs_website_scp_genes_class();
            scp_genes.Generate_from_Lincs_scp_summary_after_roc(scp_summary_website);
            scp_genes.Add_genes(mbcoL3);
            scp_genes.Write(website_directory, "Genes of cardiotoxic pathways.txt");

            Drug_cards_class drugCards = new Drug_cards_class();
            drugCards.Read_and_add_to_array(website_subdirectory, "DrugCards_preliminary.txt");
            drugCards.Read_and_add_to_array(website_subdirectory, "DrugCards_genomicVariants_pd.txt");
            drugCards.Read_and_add_to_array(website_subdirectory, "DrugCards_genomicVariants_scps_Cardiotoxicity.txt");
            drugCards.Remove_selected_drugFullNames(new string[] { "cardiotoxic TKIs", "noncardiotoxic TKIs" });
            drugCards.Add_top_cardiotoxic_SCPs(average_enrichment_results);
            drugCards.Add_drug_name();
            drugCards.Write_one_file_foreach_drug(website_subdirectory + "DrugCards/", topDEGs);
            drugCards.Add_missing_entries_for_each_drug_based_on_order_nos();
            drugCards.Drug_cards = drugCards.Drug_cards.OrderBy(l => l.Drug_fullName).ThenBy(l => l.Order).ToArray();
            drugCards.Replace_empyt_entities_by_NAs();
            drugCards.Write_for_website_linear(website_subdirectory, "DrugToxCards_linear.txt", topDEGs);
            drugCards.Write_for_website(website_subdirectory, "DrugToxCards.txt", topDEGs);
        }

        private static void Copy_sourceFiles_to_targetDestination(Dictionary<string,string> sourceFileName_targetFileName_dict, string source_directory, string target_directory)
        {
            string[] sourceFileNames;
            int sourceFileNames_length;
            string sourceFileName;
            string targetFileName;
            string complete_sourceFileName;
            string complete_targetFileName;
            sourceFileNames = sourceFileName_targetFileName_dict.Keys.ToArray();
            sourceFileNames_length = sourceFileNames.Length;
            for (int indexS = 0; indexS < sourceFileNames_length; indexS++)
            {
                sourceFileName = sourceFileNames[indexS];
                targetFileName = sourceFileName_targetFileName_dict[sourceFileName];
                complete_sourceFileName = source_directory + sourceFileName;
                if (System.IO.File.Exists(complete_sourceFileName))
                {
                    complete_targetFileName = target_directory + targetFileName;
                    System.IO.File.Copy(complete_sourceFileName, complete_targetFileName, true);
                }
            }
        }

        public static void Organize_supplTables()
        {
            string target_directory = Global_directory_class.Results_directory + Global_directory_class.SupplTables_subdirectory;
            string source_directory;
            Dictionary<string, string> sourceFileName_targetFileName_dict = new Dictionary<string, string>();

            ReadWriteClass.Create_directory_if_it_does_not_exist(target_directory);

            source_directory = Global_directory_class.Results_metadata_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Suppl_table_Drug_metadata.txt", "Suppl Table 04 - Cardiotoxicity of used drugs.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_" + "DEGenes_iPSCdCMs_P0" + "_supplemental_DEGs/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Top_600_drug-induced_DEGs_per_cell_line.txt", "Suppl Table 05 - DEGs in each cell line.txt");
            sourceFileName_targetFileName_dict.Add("Averaged_drug-induced_DEGs_counting_top600DEG.txt", "Suppl Table 06 - Averaged DEGs across all cell lines.txt");
            sourceFileName_targetFileName_dict.Add("Top_600_drug-induced_DEGs_per_cell_line after removal of 1st eigenarray.txt", "Suppl Table 09 - DEGs no 1st EA in each cell line.txt");
            sourceFileName_targetFileName_dict.Add("Top_600_drug-induced_DEGs_per_cell_line after projection into drug-selective subspaces.txt", "Suppl Table 10 - Drug-selective DEGs in each cell line.txt");
            sourceFileName_targetFileName_dict.Add("Averaged_drug-induced_DEGs after projection into drug-selective subspaces_counting_top600DEG.txt", "Suppl Table 11 - Averaged drug-selective DEGs across all cell lines.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + Global_directory_class.Eigenarray_subdirectory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Supplemental_Table_4 - Top600genes_of_first_eigenarray.txt", "Suppl Table 07 - Top600genes_of_first_eigenarray.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_5 - MBCOL2 for 1st eigenassay.txt", "Suppl Table 08 - MBCOL2 for 1st eigenassay.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL1.txt", "Suppl Table 12A - drug-selective DEGs - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL2.txt", "Suppl Table 12B - drug-selective DEGs - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL3.txt", "Suppl Table 12C - drug-selective DEGs - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL4.txt", "Suppl Table 12D - drug-selective DEGs - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_complete_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL1.txt", "Suppl Table 13A - complete DEGs - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL2.txt", "Suppl Table 13B - complete DEGs - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL3.txt", "Suppl Table 13C - complete DEGs - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL4.txt", "Suppl Table 13D - complete DEGs - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_no1stSVD_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL1.txt", "Suppl Table 14A - DEGs no 1st EA - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL2.txt", "Suppl Table 14B - DEGs no 1st EA - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL3.txt", "Suppl Table 14C - DEGs no 1st EA - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL4.txt", "Suppl Table 14D - DEGs no 1st EA - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("ST_Cardiotoxicity_Mbco_level1_SCP_summaries_betaF1_beta0.25_penalty0.5_plusVariantCounts_minAQ0.txt", "Suppl Table 15A - Potential cardiotoxic MBCOL1 SCPs.txt");
            sourceFileName_targetFileName_dict.Add("ST_Cardiotoxicity_Mbco_level2_SCP_summaries_betaF1_beta0.25_penalty0.5_plusVariantCounts_minAQ0.txt", "Suppl Table 15B - Potential cardiotoxic MBCOL2 SCPs.txt");
            sourceFileName_targetFileName_dict.Add("ST_Cardiotoxicity_Mbco_level3_SCP_summaries_betaF1_beta0.25_penalty0.5_plusVariantCounts_minAQ0.txt", "Suppl Table 15C - Potential cardiotoxic MBCOL3 SCPs.txt");
            sourceFileName_targetFileName_dict.Add("ST_Cardiotoxicity_Mbco_level4_SCP_summaries_betaF1_beta0.25_penalty0.5_plusVariantCounts_minAQ0.txt", "Suppl Table 15D - Potential cardiotoxic MBCOL4 SCPs.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScRNAseq_schaniel_markerGenes_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("ScRNAseq_iPSCdCM_schaniel_markerGenes.txt", "Suppl Table 16 - Cluster marker genes hiPSCd CMs.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Tucker_2020_adult_heart_cell_type_marker_genes_ScRNAseq_iPSCdCM_schaniel_all.txt", "Suppl Table 17A - Cluster marker genes iPSCd CMs - adult heart.txt");
            sourceFileName_targetFileName_dict.Add("Asp_2019_developing_heart_cell_type_marker_genes_ScRNAseq_iPSCdCM_schaniel_filtered.txt", "Suppl Table 17B - Cluster marker genes iPSCd CMs - developing heart.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL1_ScRNAseq_iPSCdCM_schaniel_filtered.txt", "Suppl Table 17C - Cluster marker genes hiPSCd CMs - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL2_ScRNAseq_iPSCdCM_schaniel_filtered.txt", "Suppl Table 17D - Cluster marker genes hiPSCd CMs - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL3_ScRNAseq_iPSCdCM_schaniel_filtered.txt", "Suppl Table 17E - Cluster marker genes hiPSCd CMs - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL4_ScRNAseq_iPSCdCM_schaniel_filtered.txt", "Suppl Table 17F - Cluster marker genes hiPSCd CMs - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScRNAseq_litvinukova_markerGenes_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("CellType_markers_Litvinukova_2020_cellsAdultHumanHeart.txt", "Suppl Table 18 - Cluster marker genes adult heart.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("MBCOL1_ScRNAseq_litvinukova_adultHeart_filtered.txt", "Suppl Table 19A - Cluster marker genes adult heart - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL2_ScRNAseq_litvinukova_adultHeart_filtered.txt", "Suppl Table 19B - Cluster marker genes adult heart - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL3_ScRNAseq_litvinukova_adultHeart_filtered.txt", "Suppl Table 19C - Cluster marker genes adult heart - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL4_ScRNAseq_litvinukova_adultHeart_filtered.txt", "Suppl Table 19D - Cluster marker genes adult heart - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("MBCOL1_ScRNASeq_chun_DCM_filtered.txt", "Suppl Table 20A - juvenile DCM hiPSCdCMs scRNAseq - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL2_ScRNASeq_chun_DCM_filtered.txt", "Suppl Table 20B - juvenile DCM hiPSCdCMs scRNAseq - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL3_ScRNASeq_chun_DCM_filtered.txt", "Suppl Table 20C - juvenile DCM hiPSCdCMs scRNAseq - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL4_ScRNASeq_chun_DCM_filtered.txt", "Suppl Table 20D - juvenile DCM hiPSCdCMs scRNAseq - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("MBCOL1_ScRNASeq_koenig_DCM_filtered.txt", "Suppl Table 21A - DCM heart scRNAseq - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL2_ScRNASeq_koenig_DCM_filtered.txt", "Suppl Table 21B - DCM heart scRNAseq - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL3_ScRNASeq_koenig_DCM_filtered.txt", "Suppl Table 21C - DCM heart scRNAseq - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL4_ScRNASeq_koenig_DCM_filtered.txt", "Suppl Table 21D - DCM heart scRNAseq - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("MBCOL1_SnRNASeq_chaffin_DCM_HCM_filtered.txt", "Suppl Table 22A - DCM HCM heart snRNAseq - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL2_SnRNASeq_chaffin_DCM_HCM_filtered.txt", "Suppl Table 22B - DCM HCM heart snRNAseq - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL3_SnRNASeq_chaffin_DCM_HCM_filtered.txt", "Suppl Table 22C - DCM HCM heart snRNAseq - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("MBCOL4_SnRNASeq_chaffin_DCM_HCM_filtered.txt", "Suppl Table 22D - DCM HCM heart snRNAseq - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_" + "DEGenes_iPSCdCMs_ECCoCulture" + "_supplemental_DEGs/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Top_600_drug-induced_DEGs_per_cell_line.txt", "Suppl Table 23A - DEGs EC coculture.txt");
            sourceFileName_targetFileName_dict.Add("Top_600_drug-induced_DEGs_per_cell_line after removal of 1st eigenarray.txt", "Suppl Table 23B - DEGs EC coculture no 1st EA.txt");
            sourceFileName_targetFileName_dict.Add("Top_600_drug-induced_DEGs_per_cell_line after projection into drug-selective subspaces.txt", "Suppl Table 23C - drug-selective DEGs EC coculture.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_ECCoCulture_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL1.txt", "Suppl Table 24A - drug-selective DEGs EC coculture - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL2.txt", "Suppl Table 24B - drug-selective DEGs EC coculture - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL3.txt", "Suppl Table 24C - drug-selective DEGs EC coculture - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL4.txt", "Suppl Table 24D - drug-selective DEGs EC coculture - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_ECCoCulture_enrichment_maxP0.05_top600DEGs_in_no1stSVD_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL1.txt", "Suppl Table 25A - complete DEGs EC coculture - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL2.txt", "Suppl Table 25B - complete DEGs EC coculture - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL3.txt", "Suppl Table 25C - complete DEGs EC coculture - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL4.txt", "Suppl Table 25D - complete DEGs EC coculture - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_ECCoCulture_enrichment_maxP0.05_top600DEGs_in_complete_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL1.txt", "Suppl Table 26A - DEGs no 1st EA EC coculture - MBCOL1.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL2.txt", "Suppl Table 26B - DEGs no 1st EA EC coculture - MBCOL2.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL3.txt", "Suppl Table 26C - DEGs no 1st EA EC coculture - MBCOL3.txt");
            sourceFileName_targetFileName_dict.Add("Supplemental_table_MBCOL4.txt", "Suppl Table 26D - DEGs no 1st EA EC coculture - MBCOL4.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + Global_directory_class.Drug_pk_pd_subdirectory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Genomic_variants_Only_drug_related_mechanisms_outlier_supplTable.txt", "Suppl Table 27 - Genomic variants PK PD.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + Global_directory_class.Svd_enrichment_subdirectory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("SupplTable_Cardiotoxicity_SCP_variants_foreach_drug_beta0.25_penalty0.5_minAQ0.txt", "Suppl Table 28 - Genomic variants SCPs.txt");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);
        }

        public static void Organize_figures_and_supplemental_figures()
        {
            string target_directory = Global_directory_class.Results_directory + Global_directory_class.Figures_supplFigures_subdirectory;
            string source_directory;
            Dictionary<string, string> sourceFileName_targetFileName_dict = new Dictionary<string, string>();

            ReadWriteClass.Create_directory_if_it_does_not_exist(target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_SVD_colDend_drugs_asBars.png", "Figure 1B - Complete DEG clusters.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/6_Visualize_cluster_validations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("EntitySpecific_F1_Coefficient_of_eigenassay_F1SWByOutlier.pdf", "Figure 1C - Complete data F1 scores.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/6_Visualize_cluster_validations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("EntitySpecific_F1_Coefficient_of_eigenassay_F1SWByOutlier.pdf", "Figure 1D - Combined subspace data F1 scores.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_drugs_asBars.png", "Figure 1E - subspace DEG clusters.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_SCP_summary_beta0.25_penalty0.5_onlyToxic_collapsed.pdf", "Figure 2B - SCPs associated with cardiotoxicity only tox TKIs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_compareWith_ScRNASeq_DEGs_adjP0.05_top500DEGs_beta0.25_penalty0.5_toxicOnly.pdf", "Figure 3A - iPSCdCM adult CM cardiotoxic SCPs only tox TKIs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_compareWith_Published_beta0.25_penalty0.5_toxicOnly.pdf", "Figure 3B - DCM HCM vs healty cardiotoxic SCPs only tox TKIs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/7_Visualize_entitySpecific_cluster_dendrograms/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Entity_specific_subspaces_Coefficient_of_eigenassay_Drug_F1SWByOutlier_large.pdf", "Figure 4B - Clustering results for single drugs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "yed_networks/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Regulatory_nw_minAQ20.graphml", "Figure 4C - RARG genomic variant.graphml");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Anthracycline_WNT_signaling.pdf", "Figure 4D - Anthracyclines WNT signaling.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + Global_directory_class.Drug_pk_pd_subdirectory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Identified_variants_count_minAQ20.pdf", "Figure 4E - genomic variants PK PD.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_SCP_variants_foreach_drug_beta0.25_penalty0.5_minAQ0_allSCPGenes_literature.pdf", "Figure 4F - SCPs associated with cardiotoxicity HuGE GWAS.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_SCP_variant_counts_foreach_drug_beta0.25_penalty0.5_minAQ0.pdf", "Figure 4G - genomic variants SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "yed_networks/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_Mbco_level3_SCP_gene_nw_beta0.25_penalty0.5Thin myofilament organization__minAQ0.graphml", "Figure 4H - genomic variants thin myofilament.graphml");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            if (Global_class.Memory_larger_than_16GB)
            {
                source_directory = Global_directory_class.Results_directory + "SVD_control_expression/";
                sourceFileName_targetFileName_dict.Clear();
                sourceFileName_targetFileName_dict.Add("Gtex_vs_DEGenes_iPSCdCMs_P0_replicateExpression_Control_expression_values.pdf", "Suppl Figure 1A - Control Gtex.pdf");
                Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

                source_directory = Global_directory_class.Results_directory + "SVD_control_expression/";
                sourceFileName_targetFileName_dict.Clear();
                sourceFileName_targetFileName_dict.Add("Heatmap_log10.png", "Suppl Figure 1B - Correlation control replicates - heatmap.png");
                Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

                source_directory = Global_directory_class.Results_directory + "SVD_control_expression/";
                sourceFileName_targetFileName_dict.Clear();
                sourceFileName_targetFileName_dict.Add("Heatmap_log10_legend.png", "Suppl Figure 1B - Correlation control replicates - legend.png");
                Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

                source_directory = Global_directory_class.Results_directory + "SVD_control_expression/";
                sourceFileName_targetFileName_dict.Clear();
                sourceFileName_targetFileName_dict.Add("Replicate_colDend.png", "Suppl Figure 1B - Correlation control replicates - colDend.png");
                Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);
            }

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/1_DEGs/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Sign_degs.png", "Suppl Figure 3A - No of sig DEGs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_Coefficient_of_eigenassay_Ttest_pvalue_NoPrepr_pearson_SVDheatmap.png", "Suppl Figure 3B - DEG clusters complete data - heatmap.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_Coefficient_of_eigenassay_Ttest_pvalue_NoPrepr_pearson_SVDheatmap_legend.png", "Suppl Figure 3B - DEG clusters complete data - legend.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_SVD_colDend_Drugs.png", "Suppl Figure 3B 5A - DEG clusters complete data - drugs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_SVD_colDend_Cell_lines.png", "Suppl Figure 3B 5A - DEG clusters complete data - cell lines.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_SVD_colDend_SigDEGs.png", "Suppl Figure 3B 5A - DEG clusters complete data - sig DEGs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/4_Eigenassay_statistics_and_enrichmentScores/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Correlations_NoPrepr_pearson_SVD_Coefficient_of_eigenassay.png", "Suppl Figure 6BD - EA eigenexpression correlation.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_SVD_colDend_Drugs.png", "Suppl Figure 7 - DEG clusters no 1st EA - drugs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_SVD_colDend_Cell_lines.png", "Suppl Figure 7 - DEG clusters no 1st EA - cell lines.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("FullDataClustered_SVD_colDend_SigDEGs.png", "Suppl Figure 7 - DEG clusters no 1st EA - sig DEGs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue/6_Visualize_cluster_validations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("EntitySpecific_F1_Coefficient_of_eigenassay_F1SWByOutlier.pdf", "Suppl Figure 8 top - F1 scores complete data.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/6_Visualize_cluster_validations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("EntitySpecific_F1_Coefficient_of_eigenassay_F1SWByOutlier.pdf", "Suppl Figure 8 bottom - F1 scores no 1st EA.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/4_Eigenassay_statistics_and_enrichmentScores/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Correlation_statisticCluster_NoPrepr_pearson_SVD_Coefficient_of_eigenassay.png", "Suppl Figure 9B - EAs related to cell line and drugs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/6_Visualize_cluster_validations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("EntitySpecific_F1_Coefficient_of_eigenassay_F1SW0.95.pdf", "Suppl Figure 10BC - F1 scores F1S weight 095.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/6_Visualize_cluster_validations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("EntitySpecific_F1_Coefficient_of_eigenassay_F1SWByOutlier.pdf", "Suppl Figure 10EF - F1 scores final subspaces.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/7_Visualize_entitySpecific_cluster_dendrograms/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Entity_specific_subspaces_Coefficient_of_eigenassay_Drug_F1SWByOutlier_large.pdf", "Suppl Figure 11 - Clustering results for single drugs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/5_Cluster_validation_f1_scores/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Outlier_responses_Coefficient_of_eigenassay.pdf", "Suppl Figure 12 - Identification of outlier responses.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_Drugs.png", "Suppl Figure 13 - Combined subspace DEG clusters - drugs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_Cell_lines.png", "Suppl Figure 13 - Combined subspace DEG clusters - cell lines.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_SigDEGs.png", "Suppl Figure 13 - Combined subspace DEG clusters - sig DEGs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_MBCOL1.pdf", "Suppl Figure 14B - Top MBCOL1 SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_MBCOL2.pdf", "Suppl Figure 14C - Top MBCOL2 SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_MBCOL3.pdf", "Suppl Figure 14D - Top MBCOL3 SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_MBCOL4.pdf", "Suppl Figure 14E - Top MBCOL4 SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Overlapping_scp_counts_DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_smallFigures.pdf", "Suppl Figure 15 - Overalapping SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_precisionRecallF1score_betaF1_beta0.25_penalty0.5.pdf", "Suppl Figure 17 - F1 score and Area Under the Curve statistics.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_SCP_summary_beta0.25_penalty0.5_collapsed.pdf", "Suppl Figure 18 - SCPs associated with TKI cardiotoxicity.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_SCP_summaries_betaF1_beta0.25_penalty0.5_plusVariantCounts_minAQ0.pdf", "Suppl Figure 20 21 - SCPs associated with TKI cardiotoxicity.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScRNAseq_schaniel_markerGenes_directory + "Documentations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("CellMaps_C6.png", "Suppl Figure 22A - SC iPSCdCM UMAPs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScRNAseq_schaniel_markerGenes_directory + "Documentations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Distributions_of_cluster_in_entities_C6.png", "Suppl Figure 22B - SC iPSCdCM cell type distributions.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory + "Top5SCPs/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Tucker_2020_adult_heart_cell_type_marker_genes_ScRNAseq_iPSCdCM_schaniel_filtered.pdf", "Suppl Figure 22C - SC iPSCdCM enrichment adult heart.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory + "Top5SCPs/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Asp_2019_developing_heart_cell_type_marker_genes_ScRNAseq_iPSCdCM_schaniel_filtered.pdf", "Suppl Figure 22D - SC iPSCdCM enrichment developing heart.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);


            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_compareWith_ScRNASeq_DEGs_adjP0.05_top500DEGs_beta0.25_penalty0.5.pdf", "Suppl Figure 23 - iPSCdCM adult CM cardiotoxic SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_compareWith_Published_beta0.25_penalty0.5.pdf", "Suppl Figure 24 - DCM HCM vs healty cardiotoxic SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("ATC vs high tox. TKI_SCP_summary_beta0.25_penalty0.5_collapsed.pdf", "Suppl Figure 25 - SCPs associated with anthracyclines.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue_no1stSVD/7_Visualize_entitySpecific_cluster_dendrograms/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Entity_specific_subspaces_Coefficient_of_eigenassay_Drug_coCulture_F1SWByOutlier_large.pdf", "Suppl Figure 28A - Clustering results for single drugs with coculture.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue_no1stSVD/6_Visualize_cluster_validations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("EntitySpecific_F1_Coefficient_of_eigenassay_F1SWByOutlier.pdf", "Suppl Figure 28B bottom - F1 scores final subspaces with coculture data.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/6_Visualize_cluster_validations/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("EntitySpecific_F1_Coefficient_of_eigenassay_F1SWByOutlier.pdf", "Suppl Figure 28B top - F1 scores final subspaces original.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_P0_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_drugs_asBars.png", "Suppl Figure 28C top - Combined subspace DEGs original clustered bars.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_coCulture_drugs_asBars.png", "Suppl Figure 28C bottom - Combined subspace DEGs with coculture data clustered bars.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_coCulture_Drugs.png", "Suppl Figure 28D - Combined subspace DEGs with coculture data clustered - drugs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_coCulture_Cell_lines.png", "Suppl Figure 28D - Combined subspace DEGs with coculture data clustered - cell lines.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "SVD_DEGenes_iPSCdCMs_ECCoCulture_Signed_minus_log10pvalue_no1stSVD/10_Heatmaps/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Combined_subspace_dataClustered_SVD_colDend_coCulture_SigDEGs.png", "Suppl Figure 28D - Combined subspace DEGs with coculture data clustered - SigDEGs.png");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_MBCOL1.pdf", "Suppl Figure 29A - EC coculture - Top MBCOL1 SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_MBCOL2.pdf", "Suppl Figure 29B - EC coculture - Top MBCOL2 SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_MBCOL3.pdf", "Suppl Figure 29C - EC coculture - Top MBCOL3 SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_MBCOL4.pdf", "Suppl Figure 29D - EC coculture - Top MBCOL4 SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_compareWith_Pazopanib_adjP0.05_top500DEGs_beta0.25_penalty0.5_toxicOnly.pdf", "Suppl Figure 30A - pazopanib cardiotoxic SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_compareWith_Dabrafenib_adjP0.05_top500DEGs_beta0.25_penalty0.5_toxicOnly.pdf", "Suppl Figure 30B - dabrafenib cardiotoxic SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + "yed_networks/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Regulatory_nw_Kinase_inhibitor_Monoclonal_antibody_Only_drug_related_mechanisms_outlier_minAQ20.graphml", "Suppl Figure 32B - genomic variants TKI target proteins.graphml");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_directory + Global_directory_class.Drug_pk_pd_subdirectory;
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Identified_variants_count_minAQ20.pdf", "Suppl Figure 32 AC - genomic variants PK PD.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

            source_directory = Global_directory_class.Results_svd_enrichment_directory + "DEGenes_iPSCdCMs_P0_enrichment_maxP0.05_top600DEGs_in_decomposed_rocForFractional_rank/";
            sourceFileName_targetFileName_dict.Clear();
            sourceFileName_targetFileName_dict.Add("Cardiotoxicity_SCP_variant_counts_foreach_drug_beta0.25_penalty0.5_minAQ0.pdf", "Suppl Figure 32D - genomic variants SCPs.pdf");
            Copy_sourceFiles_to_targetDestination(sourceFileName_targetFileName_dict, source_directory, target_directory);

        }
        #endregion
    }

}
