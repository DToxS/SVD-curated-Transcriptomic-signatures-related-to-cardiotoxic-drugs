using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common_classes;
using ReadWrite;
using Statistic;
using Network;

namespace Highthroughput_data
{
    enum Variant_location_refGene_enum { E_m_p_t_y, Not_determined, Intergenic, Ncrna_exonic, Ncrna_intronic, Ncrna_splicing, Upstream, Downstream, Exonic, Intronic, Splicing, Utr3, Utr5 }
    enum Variant_cardiotoxic_effect_enum { E_m_p_t_y, Protective_variant, Risk_variant, Variant_with_effect, Not_specified, Variant_with_no_effect }
    enum Relation_of_gene_symbol_to_drug_enum
    {
        E_m_p_t_y, No_relation_identified,
        Drugs_not_considered_yet,
        No_specified_mechanism,
        Drug_target_protein, Tf_regulating_drug_target_protein, Kinase_regulating_drug_target_protein, 
        Enzyme, Tf_regulating_enzyme, Kinase_regulating_enzyme, 
        Transporter, Tf_regulating_transporter, Kinase_regulating_transporter, 
    }
    enum Lincs_genomics_clinical_effect_of_variant_enum { E_m_p_t_y, Not_significant, Significant_by_nominal_pvalue, Significant_by_adjusted_pvalue };
    enum Lincs_genomics_analysis_stage_enum {  E_m_p_t_y, Before_quality_control, After_quality_control, After_quality_control_biological_relevance, 
                                               After_quality_control_biological_relevance_overrepresented_variants, 
                                               After_quality_control_biological_relevance_overrepresented_variants_in_outlier,
                                               Only_drug_related_mechanisms, Only_drug_related_mechanisms_outlier }

    class Lincs_genomics_drugBank_names_class
    {
        public static Dictionary<Relation_of_gene_symbol_to_drug_enum, System.Drawing.Color> Get_relation_color_dict()
        {
            Dictionary<Relation_of_gene_symbol_to_drug_enum, System.Drawing.Color> relation_color_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, System.Drawing.Color>();
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Drug_target_protein, System.Drawing.Color.Chocolate);
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein, System.Drawing.Color.DarkGoldenrod);
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein, System.Drawing.Color.Orange);
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Enzyme, System.Drawing.Color.Purple);
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme, System.Drawing.Color.MediumOrchid);
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme, System.Drawing.Color.PaleVioletRed);
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Transporter, System.Drawing.Color.Blue);
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter, System.Drawing.Color.DodgerBlue);
            relation_color_dict.Add(Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter, System.Drawing.Color.SkyBlue);
            return relation_color_dict;
        }

        public static Relation_of_gene_symbol_to_drug_enum Get_corresponding_relationship_for_selected_ontology(Relation_of_gene_symbol_to_drug_enum relation, Ontology_type_enum ontology)
        {
            Relation_of_gene_symbol_to_drug_enum corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.E_m_p_t_y;
            switch (relation)
            {
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                    switch (ontology)
                    {
                        case Ontology_type_enum.Drugbank_drug_targets:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein;
                            break;
                        case Ontology_type_enum.Drugbank_enzymes:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme;
                            break;
                        case Ontology_type_enum.Drugbank_transporters:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                    switch (ontology)
                    {
                        case Ontology_type_enum.Drugbank_drug_targets:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein;
                            break;
                        case Ontology_type_enum.Drugbank_enzymes:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme;
                            break;
                        case Ontology_type_enum.Drugbank_transporters:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Transporter:
                case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                    switch (ontology)
                    {
                        case Ontology_type_enum.Drugbank_drug_targets:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Drug_target_protein;
                            break;
                        case Ontology_type_enum.Drugbank_enzymes:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Enzyme;
                            break;
                        case Ontology_type_enum.Drugbank_transporters:
                            corresponding_relationship = Relation_of_gene_symbol_to_drug_enum.Transporter;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                default:
                    throw new Exception();
            }
            return corresponding_relationship;
        }

        public static Relation_of_gene_symbol_to_drug_enum Get_corresponding_relationship_for_direct_interactor_of_gene(Relation_of_gene_symbol_to_drug_enum input_relationship)
        {
            switch (input_relationship)
            {
                case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                    return Relation_of_gene_symbol_to_drug_enum.Drug_target_protein;
                case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                    return Relation_of_gene_symbol_to_drug_enum.Enzyme;
                case Relation_of_gene_symbol_to_drug_enum.Transporter:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                    return Relation_of_gene_symbol_to_drug_enum.Transporter;
                default:
                    throw new Exception();
            }
        }

        private static string Get_genomics_complete_directory()
        {
            return Global_directory_class.Experimental_data_whole_genome_sequencing_directory;
        }

        public static string Get_complete_fileName_of_genomic_input_file()
        {
            string fileName = "annotated_combined.hg38_multianno_reheadered_withGWAS_withRSids.txt";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }
        public static string Get_complete_fileName_of_use_genomic_input_file()
        {
            string fileName = "annotated_combined.hg38_multianno_reheadered_withGWAS_withRSids_use.txt";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }
        public static string Get_complete_fileName_of_short_genomic_input_file()
        {
            string fileName = "annotated_combined.hg38_multianno_reheadered_withGWAS_withRSids_short.txt";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }
        public static string Get_complete_fileName_of_genomics_binary_file_before_quality_control()
        {
            string fileName = "Cellline_specific_minor_allels.bin";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }
        public static string Get_complete_fileName_of_genomics_binary_file_after_quality_control(int minimum_qualityAQ)
        {
            string fileName = "Cellline_specific_minor_allels_minAQ" + minimum_qualityAQ + ".bin";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }
        public static string Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance(int minimum_qualityAQ)
        {
            string fileName = "Cellline_specific_minor_allels_minAQ" + minimum_qualityAQ + "_bR.bin";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }
        public static string Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance_considering_only_variants_overrepresented_in_one_cellline(int minimum_qualityAQ)
        {
            string fileName = "Cellline_specific_minor_allels_minAQ" + minimum_qualityAQ + "_bR_orV.bin";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }
        public static string Get_complete_fileName_of_genomics_binary_file_after_quality_control_with_biological_relevance_considering_only_variants_overrepresented_in_one_cellline_that_is_outlier(int minimum_qualityAQ)
        {
            string fileName = "Cellline_specific_minor_allels_minAQ" + minimum_qualityAQ + "_bR_orV_outlier.bin";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }
        public static string Get_complete_fileName_of_publishedGenes_genomics_binary_file_after_after_QC_BR_and_orV(int minimum_qualityAQ)
        {
            string fileName = "Cellline_specific_minor_allels_minAQ" + minimum_qualityAQ + "_bR_orV_publishedGenes.bin";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }

        public static string Get_complete_fileName_of_drugMoA_genomics_binary_file(Relation_of_gene_symbol_to_drug_enum relation, Ontology_type_enum ontology, int minimum_qualityAQ)
        {
            string fileName = "Cellline_genomics_minAQ" + minimum_qualityAQ + "_bR_orV_" + ontology + "_" + Get_corresponding_relationship_for_selected_ontology(relation, ontology) + ".bin";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }

        public static string Get_complete_fileName_of_drugMoA_genomics_binary_file(Relation_of_gene_symbol_to_drug_enum relation, int minimum_qualityAQ)
        {
            Ontology_type_enum ontology = Get_corresponding_ontology(relation);
            return Get_complete_fileName_of_drugMoA_genomics_binary_file(relation, ontology, minimum_qualityAQ);
        }

        public static string Get_complete_fileName_of_drugMoA_genomics_outlier_binary_file(Relation_of_gene_symbol_to_drug_enum relation, Ontology_type_enum ontology, int minimum_qualityAQ)
        {
            string fileName = "Cellline_genomics_minAQ" + minimum_qualityAQ + ontology + "_bR_orV_" + Get_corresponding_relationship_for_selected_ontology(relation, ontology) + "_outlier.bin";
            string complete_directory = Get_genomics_complete_directory() + fileName;
            return complete_directory;
        }

        public static string Get_complete_fileName_of_drugMoA_genomics_outlier_binary_file(Relation_of_gene_symbol_to_drug_enum relation, int minimum_qualityAQ)
        {
            Ontology_type_enum ontology = Get_corresponding_ontology(relation);
            return Get_complete_fileName_of_drugMoA_genomics_outlier_binary_file(relation, ontology, minimum_qualityAQ);
        }

        public static Ontology_type_enum Get_corresponding_ontology(Relation_of_gene_symbol_to_drug_enum relation)
        {
            switch (relation)
            {
                case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                    return Ontology_type_enum.Drugbank_drug_targets;
                case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                    return Ontology_type_enum.Drugbank_enzymes;
                case Relation_of_gene_symbol_to_drug_enum.Transporter:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                    return Ontology_type_enum.Drugbank_transporters;
                default:
                    throw new Exception();
            }
        }

        public static Relation_of_gene_symbol_to_drug_enum[] Keep_only_relationships_mapping_to_selected_ontology(Relation_of_gene_symbol_to_drug_enum[] potential_relationships, Ontology_type_enum ontology)
        {
            List<Relation_of_gene_symbol_to_drug_enum> keep = new List<Relation_of_gene_symbol_to_drug_enum>();
            foreach (Relation_of_gene_symbol_to_drug_enum potential_relationship in potential_relationships)
            {
                switch (ontology)
                {
                    case Ontology_type_enum.Drugbank_drug_targets:
                        switch (potential_relationship)
                        {
                            case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                            case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                            case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                                keep.Add(potential_relationship);
                                break;
                            default:
                                break;
                        }
                        break;
                    case Ontology_type_enum.Drugbank_enzymes:
                        switch (potential_relationship)
                        {
                            case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                            case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                            case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                                keep.Add(potential_relationship);
                                break;
                            default:
                                break;
                        }
                        break;
                    case Ontology_type_enum.Drugbank_transporters:
                        switch (potential_relationship)
                        {
                            case Relation_of_gene_symbol_to_drug_enum.Transporter:
                            case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                            case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                                keep.Add(potential_relationship);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }
            return keep.ToArray();
        }
        public static bool Does_analysis_stage_only_contain_outlier_celllines(Lincs_genomics_analysis_stage_enum analysis_stage)
        {
            switch (analysis_stage)
            {
                case Lincs_genomics_analysis_stage_enum.After_quality_control:
                case Lincs_genomics_analysis_stage_enum.Before_quality_control:
                case Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms:
                case Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance:
                case Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance_overrepresented_variants:
                case Lincs_genomics_analysis_stage_enum.After_quality_control_biological_relevance_overrepresented_variants_in_outlier:
                    return false;
                case Lincs_genomics_analysis_stage_enum.Only_drug_related_mechanisms_outlier:
                    return true;
                default:
                    throw new Exception();
            }
        }
    }


    class Lincs_vcf_genomic_readOptions_class : ReadWriteOptions_base
    {
        public Lincs_vcf_genomic_readOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "CHROM", "POS", "ID", "REF", "ALT", "QUAL", "FILTER", "INFO", "FORMAT", "MSN01", "MSN02", "MSN05", "MSN06", "MSN08", "MSN09" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_everything;
        }
    }

    class Lincs_vcf_genomic_input_data_line_class
    {
        public string rsID { get; set; }
        public string Chr { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string Ref { get; set; }
        public string Alt { get; set; }
        public string Func_refGene { get; set; }
        public string Gene_refGene { get; set; }
        public string CADD_phred { get; set; }
        public string AQ { get; set; }
        public int AQ_integer { get; set; }
        public string AF { get; set; }
        public string AF_raw { get; set; }
        public string AF_male { get; set; }
        public string AF_female { get; set; }
        public string AF_afr { get; set; }
        public string AF_ami { get; set; }
        public string AF_amr { get; set; }
        public string AF_asj { get; set; }
        public string AF_eas { get; set; }
        public string AF_fin { get; set; }
        public string AF_nfe { get; set; }
        public string AF_oth { get; set; }
        public string AF_sas { get; set; }
        public string spAI_DS_snv_raw { get; set; }
        public string spAI_DS_indel_raw { get; set; }
        public string CLNSIG { get; set; }
        public string gtex_atria_egene { get; set; }
        public string gtex_atria_sgene { get; set; }
        public string gtex_vent_egene { get; set; }
        public string gtex_vent_sgene { get; set; }
        public string Publication { get; set; }
        public string Cell_line_AF { get; set; }
        public string MSN01_GT { get; set; }
        public string MSN01_DP { get; set; }
        public string MSN01_AD { get; set; }
        public string MSN01_GQ { get; set; }
        public string MSN02_GT { get; set; }
        public string MSN02_DP { get; set; }
        public string MSN02_AD { get; set; }
        public string MSN02_GQ { get; set; }
        public string MSN05_GT { get; set; }
        public string MSN05_DP { get; set; }
        public string MSN05_AD { get; set; }
        public string MSN05_GQ { get; set; }
        public string MSN06_GT { get; set; }
        public string MSN06_DP { get; set; }
        public string MSN06_AD { get; set; }
        public string MSN06_GQ { get; set; }
        public string MSN08_GT { get; set; }
        public string MSN08_DP { get; set; }
        public string MSN08_AD { get; set; }
        public string MSN08_GQ { get; set; }
        public string MSN09_GT { get; set; }
        public string MSN09_DP { get; set; }
        public string MSN09_AD { get; set; }
        public string MSN09_GQ { get; set; }

        public static Lincs_vcf_genomic_input_data_line_class[] Order_by_rsID_chr_start_end_ref_alt_descendingAQ(Lincs_vcf_genomic_input_data_line_class[] input_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>>>> rsID_chr_start_end_ref_alt_aq_dict = new Dictionary<string, Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>>>>();
            Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>>> chr_start_end_ref_alt_aq_dict = new Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>>>();
            Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>> start_end_ref_alt_aq_dict = new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>>();
            Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>> end_ref_alt_aq_dict = new Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>> ref_alt_aq_dict = new Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>();
            Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>> alt_aq_dict = new Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>();
            Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>> aq_dict = new Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>();
            int input_lines_length = input_lines.Length;
            Lincs_vcf_genomic_input_data_line_class input_line;
            for (int indexInput=0; indexInput<input_lines_length; indexInput++)
            {
                input_line = input_lines[indexInput];
                if (!rsID_chr_start_end_ref_alt_aq_dict.ContainsKey(input_line.rsID))
                {
                    rsID_chr_start_end_ref_alt_aq_dict.Add(input_line.rsID, new Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>>>());
                }
                if (!rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID].ContainsKey(input_line.Chr))
                {
                    rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID].Add(input_line.Chr, new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>>());
                }
                if (!rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr].ContainsKey(input_line.Start))
                {
                    rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr].Add(input_line.Start, new Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>>());
                }
                if (!rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start].ContainsKey(input_line.End))
                {
                    rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start].Add(input_line.End, new Dictionary<string, Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>>());
                }
                if (!rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start][input_line.End].ContainsKey(input_line.Ref))
                {
                    rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start][input_line.End].Add(input_line.Ref, new Dictionary<string, Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>>());
                }
                if (!rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start][input_line.End][input_line.Ref].ContainsKey(input_line.Alt))
                {
                    rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start][input_line.End][input_line.Ref].Add(input_line.Alt, new Dictionary<int, List<Lincs_vcf_genomic_input_data_line_class>>());
                }
                if (!rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start][input_line.End][input_line.Ref][input_line.Alt].ContainsKey(input_line.AQ_integer))
                {
                    rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start][input_line.End][input_line.Ref][input_line.Alt].Add(input_line.AQ_integer, new List<Lincs_vcf_genomic_input_data_line_class>());
                }
                rsID_chr_start_end_ref_alt_aq_dict[input_line.rsID][input_line.Chr][input_line.Start][input_line.End][input_line.Ref][input_line.Alt][input_line.AQ_integer].Add(input_line);
            }
            string[] rsIDs;
            string rsID;
            int rsIDs_length;
            string[] chrs;
            string chr;
            int chrs_length;
            int[] starts;
            int start;
            int starts_length;
            int[] ends;
            int end;
            int ends_length;
            string[] refs;
            string _ref;
            int refs_length;
            string[] alts;
            string alt;
            int alts_length;
            int[] aqs;
            int aq;
            int aqs_length;
            List<Lincs_vcf_genomic_input_data_line_class> ordered_list = new List<Lincs_vcf_genomic_input_data_line_class>();
            rsIDs = rsID_chr_start_end_ref_alt_aq_dict.Keys.ToArray();
            rsIDs_length = rsIDs.Length;
            rsIDs = rsIDs.OrderBy(l => l).ToArray();
            for (int indexRsID=0; indexRsID<rsIDs_length; indexRsID++)
            {
                rsID = rsIDs[indexRsID];
                chr_start_end_ref_alt_aq_dict = rsID_chr_start_end_ref_alt_aq_dict[rsID];
                chrs = chr_start_end_ref_alt_aq_dict.Keys.ToArray();
                chrs_length = chrs.Length;
                chrs = chrs.OrderBy(l => l).ToArray();
                for (int indexChr=0; indexChr<chrs_length;indexChr++)
                {
                    chr = chrs[indexChr];
                    start_end_ref_alt_aq_dict = chr_start_end_ref_alt_aq_dict[chr];
                    starts = start_end_ref_alt_aq_dict.Keys.ToArray();
                    starts_length = starts.Length;
                    starts = starts.OrderBy(l => l).ToArray();
                    for (int indexStart=0; indexStart<starts_length;indexStart++)
                    {
                        start = starts[indexStart];
                        end_ref_alt_aq_dict = start_end_ref_alt_aq_dict[start];
                        ends = end_ref_alt_aq_dict.Keys.ToArray();
                        ends_length = ends.Length;
                        ends = ends.OrderBy(l => l).ToArray();
                        for (int indexEnd=0; indexEnd<ends_length;indexEnd++)
                        {
                            end = ends[indexEnd];
                            ref_alt_aq_dict = end_ref_alt_aq_dict[end];
                            refs = ref_alt_aq_dict.Keys.ToArray();
                            refs = refs.OrderBy(l => l).ToArray();
                            refs_length = refs.Length;
                            for (int indexRef=0; indexRef<refs_length;indexRef++)
                            {
                                _ref = refs[indexRef];
                                alt_aq_dict = ref_alt_aq_dict[_ref];
                                alts = alt_aq_dict.Keys.ToArray();
                                alts = alts.OrderBy(l => l).ToArray();
                                alts_length = alts.Length;
                                for (int indexAlt=0; indexAlt<alts_length;indexAlt++)
                                {
                                    alt = alts[indexAlt];
                                    aq_dict = alt_aq_dict[alt];
                                    aqs = aq_dict.Keys.ToArray();
                                    aqs = aqs.OrderByDescending(l => l).ToArray();
                                    aqs_length = aqs.Length;
                                    for (int indexAQ=0; indexAQ<aqs_length; indexAQ++)
                                    {
                                        aq = aqs[indexAQ];
                                        ordered_list.AddRange(aq_dict[aq]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_list_length = ordered_list.Count;
                if (ordered_list_length!= input_lines_length) { throw new Exception(); }
                Lincs_vcf_genomic_input_data_line_class this_line;
                Lincs_vcf_genomic_input_data_line_class previous_line;
                for (int indexOrdered=1;indexOrdered<ordered_list_length;indexOrdered++)
                {
                    this_line = ordered_list[indexOrdered];
                    previous_line = ordered_list[indexOrdered - 1];
                    if (this_line.rsID.CompareTo(previous_line.rsID) < 0) { throw new Exception(); }
                    else if (this_line.rsID.Equals(previous_line.rsID)
                        && this_line.Chr.CompareTo(previous_line.Chr) < 0) { throw new Exception(); }
                    else if (this_line.rsID.Equals(previous_line.rsID)
                        && this_line.Chr.Equals(previous_line.Chr)
                        && this_line.Start.CompareTo(previous_line.Start) < 0) { throw new Exception(); }
                    else if (this_line.rsID.Equals(previous_line.rsID)
                        && this_line.Chr.Equals(previous_line.Chr)
                        && this_line.Start.Equals(previous_line.Start)
                        && this_line.End.CompareTo(previous_line.End) < 0) { throw new Exception(); }
                    else if (this_line.rsID.Equals(previous_line.rsID)
                        && this_line.Chr.Equals(previous_line.Chr)
                        && this_line.Start.Equals(previous_line.Start)
                        && this_line.End.Equals(previous_line.End)
                        && this_line.Alt.CompareTo(previous_line.Alt) < 0) { throw new Exception(); }
                    else if (this_line.rsID.Equals(previous_line.rsID)
                        && this_line.Chr.Equals(previous_line.Chr)
                        && this_line.Start.Equals(previous_line.Start)
                        && this_line.End.Equals(previous_line.End)
                        && this_line.Alt.Equals(previous_line.Alt)
                        && this_line.Ref.CompareTo(previous_line.Ref) < 0) { throw new Exception(); }
                    else if (this_line.rsID.Equals(previous_line.rsID)
                        && this_line.Chr.Equals(previous_line.Chr)
                        && this_line.Start.Equals(previous_line.Start)
                        && this_line.End.Equals(previous_line.End)
                        && this_line.Alt.Equals(previous_line.Alt)
                        && this_line.Ref.Equals(previous_line.Ref)
                        && this_line.AQ_integer.CompareTo(previous_line.AQ_integer) > 0) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }
    }

    class Lincs_vcf_genomic_input_readOptions_class : ReadWriteOptions_base
    {
        public static string Genomics_data_directory = Global_directory_class.Experimental_data_whole_genome_sequencing_directory;

        public Lincs_vcf_genomic_input_readOptions_class(string complete_fileName, string chromosome)
        {
            this.File = complete_fileName;
            this.Key_propertyNames = new string[] {"rsID","Chr","Start","End","Ref","Alt","Func_refGene","Gene_refGene","AQ","AF","AF_raw","AF_male","AF_female","AF_afr","AF_ami","AF_amr","AF_asj","AF_eas","AF_fin","AF_nfe","AF_oth","AF_sas","CADD_phred",
                   "gtex_atria_egene","gtex_atria_sgene","gtex_vent_egene","gtex_vent_sgene","MSN01_GT","MSN01_DP","MSN01_AD","MSN01_GQ","MSN02_GT","MSN02_DP","MSN02_AD","MSN02_GQ","MSN05_GT","MSN05_DP",
                   "MSN05_AD","MSN05_GQ","MSN06_GT","MSN06_DP","MSN06_AD","MSN06_GQ","MSN08_GT","MSN08_DP","MSN08_AD","MSN08_GQ","MSN09_GT","MSN09_DP","MSN09_AD","MSN09_GQ","Cell_line_AF","spAI_DS_snv_raw","spAI_DS_indel_raw","CLNSIG","Publication"
            };
            this.Key_columnNames = new string[] {"rsID", "Chr","Start","End","Ref","Alt","Func_refGene","Gene_refGene","[7]AQ","AF","AF_raw","AF_male","AF_female","AF_afr","AF_ami","AF_amr","AF_asj","AF_eas","AF_fin","AF_nfe","AF_oth","AF_sas","CADD_phred",
                   "gtex_atria_egene","gtex_atria_sgene","gtex_vent_egene","gtex_vent_sgene","[9]MSN01:GT", "[10]MSN01:DP","[11]MSN01:AD","[12]MSN01:GQ",
                                                                                             "[14]MSN02:GT","[15]MSN02:DP","[16]MSN02:AD","[17]MSN02:GQ",
                                                                                             "[19]MSN05:GT","[20]MSN05:DP","[21]MSN05:AD","[22]MSN05:GQ",
                                                                                             "[24]MSN06:GT","[25]MSN06:DP","[26]MSN06:AD","[27]MSN06:GQ",
                                                                                             "[29]MSN08:GT","[30]MSN08:DP","[31]MSN08:AD","[32]MSN08:GQ",
                                                                                             "[34]MSN09:GT","[35]MSN09:DP","[36]MSN09:AD","[37]MSN09:GQ","[6]AF","spAI_DS_snv_raw","spAI_DS_indel_raw","CLNSIG","Publication"
            };
            this.SafeCondition_columnNames = new string[] { "Chr" };
            this.SafeCondition_entries = new string[] { (string)chromosome.Clone() };

            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Lincs_vcf_genomic_data_chromosomeOnly_line_class
    {
        public string Chr { get; set; }
    }
    class Lincs_vcf_genomic_data_chromosomeOnly_readWriteOptions_class : ReadWriteOptions_base
    {
        public Lincs_vcf_genomic_data_chromosomeOnly_readWriteOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Chr" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }


    class Lincs_vcf_genomic_data_line_class
    {
        public Ontology_type_enum  Ontology { get; set; }
        public string Chrom { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string Gene_symbol { get; set; }
        public string Rs_identifier { get; set; }
        public Relation_of_gene_symbol_to_drug_enum Relation_of_gene_symbol_to_drug { get; set; }
        public string Relation_of_gene_symbol_to_drug_description {  get { return Lincs_website_conversion_class.Get_relationOfGeneSymbolToDrugTargetProtein_description(Relation_of_gene_symbol_to_drug); } }
        public string Drug_target_symbol { get; set; }
        public float Rank_of_gene_symbol_within_relation_for_drug { get; set; }
        public Variant_location_refGene_enum Variant_location { get; set; }
        public string Variant_location_description {  get { return Lincs_website_conversion_class.Get_location_of_variant_description(Variant_location); } }
        public string Minor_allele { get; set; }
        public string[] Major_alleles { get; set; }
        public float Minor_allel_frequency { get; set; }
        public float Cadd_phred { get; set; }
        public float SpAI_DS_snv_raw { get; set; }
        public float SpAI_DS_indel_raw { get; set; }
        public int Successfully_sequenced_celllines_count { get; set; }
        public int Quality_aq { get; set; }
        public string Clnsig { get; set; }
        public string Cell_line { get; set; }
        public string Gtex_atrial_eQTL { get; set; }
        public string Gtex_atrial_sQTL { get; set; }
        public string Gtex_ventricular_eQTL { get; set; }
        public string Gtex_ventricular_sQTL { get; set; }
        public float Cell_line_outlier_adj_pvalue { get; set; }
        public float Cell_line_outlier_pvalue { get; set; }
        public float Cell_line_outlier_mean_f1_score_without_outlier { get; set; }
        public float Cell_line_outlier_F1_score_weight { get; set; }
        public string Cell_line_drug_with_outlier_response { get; set; }
        public Drug_type_enum Drug_type { get; set; }
        public string Drug_is_cardiotoxic { get; set; }
        public int Cell_line_drug_sample_count { get; set; }
        public float[] F1_score_weigths_with_outlier_responses { get; set; }
        public bool Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments { get; set; }
        public int Cell_line_minor_allele_count { get; set; }
        public int[] Other_cell_lines_minor_allele_counts { get; set; }
        public string Cell_line_genotype_GT { get; set; }
        public int Cell_line_allele_depth_AD { get; set; }
        public int Cell_line_approximate_read_depth_DP { get; set; }
        public int Cell_line_phred_scale_genotype_quality_GQ { get; set; }
        public float Cell_line_conditional_genotype_quality { get; set; }
        public float Cell_line_frequency_of_cell_line_minor_allele { get; set; }
        public string Publication { get; set; }
        public string ReadWrite_other_cell_lines_minor_allele_counts 
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Other_cell_lines_minor_allele_counts, ';'); }
            set { Other_cell_lines_minor_allele_counts = ReadWriteClass.Get_array_from_readLine<int>(value, ';'); }
        }
        public string ReadWrite_major_alleles
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Major_alleles, ';'); }
            set { Major_alleles = ReadWriteClass.Get_array_from_readLine<string>(value, ';'); }
        }
        public string ReadWrite_F1_score_weigths_with_outlier_responses
        {
            get { return ReadWriteClass.Get_writeLine_from_array(F1_score_weigths_with_outlier_responses, ';'); }
            set { F1_score_weigths_with_outlier_responses = ReadWriteClass.Get_array_from_readLine<float>(value, ';'); }
        }

        #region For variant mapping to SCPs
        public string Scp { get { return Cell_line; } set { Cell_line = value; } }
        public string Side_effect { get { return Drug_target_symbol; } set { Drug_target_symbol = value; } }
        public string Toxicity_association { get { return Publication; } set { Publication = value; } }
        public DE_entry_enum Entry_type { get; set; }
        public string Entry_type_description {  get { return Lincs_website_conversion_class.Get_entryType_description(Entry_type); } }
        public string Drug_regulating_scp { get { return Cell_line_drug_with_outlier_response; } set { Cell_line_drug_with_outlier_response = value; } }
        public string Scp_gene {  get { return Gene_symbol; } set { Gene_symbol = value; } }
        #endregion

        #region Order
        public static Lincs_vcf_genomic_data_line_class[] Order_by_geneSymbol_cellline(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> geneSymbol_cellline_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> cellline_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!geneSymbol_cellline_dict.ContainsKey(data_line.Gene_symbol))
                {
                    geneSymbol_cellline_dict.Add(data_line.Gene_symbol, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!geneSymbol_cellline_dict[data_line.Gene_symbol].ContainsKey(data_line.Cell_line))
                {
                    geneSymbol_cellline_dict[data_line.Gene_symbol].Add(data_line.Cell_line, new List<Lincs_vcf_genomic_data_line_class>());
                }
                geneSymbol_cellline_dict[data_line.Gene_symbol][data_line.Cell_line].Add(data_line);
            }
            data = null;
            string[] geneSymbols = geneSymbol_cellline_dict.Keys.ToArray();
            string geneSymbol;
            int geneSymbols_length = geneSymbols.Length;
            string[] celllines;
            string cellline;
            int celllines_length;
            geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
            for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
            {
                geneSymbol = geneSymbols[indexGS];
                cellline_dict = geneSymbol_cellline_dict[geneSymbol];
                celllines = cellline_dict.Keys.ToArray();
                celllines = celllines.OrderBy(l => l).ToArray();
                celllines_length = celllines.Length;
                for (int indexCL = 0; indexCL < celllines_length; indexCL++)
                {
                    cellline = celllines[indexCL];
                    ordered_list.AddRange(cellline_dict[cellline]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0) { throw new Exception(); }
                    else if ((this_line.Gene_symbol.Equals(previous_line.Gene_symbol))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_geneSymbol_rsIdentifier(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> geneSymbol_rsIdentifier_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> rsIdentifier_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!geneSymbol_rsIdentifier_dict.ContainsKey(data_line.Gene_symbol))
                {
                    geneSymbol_rsIdentifier_dict.Add(data_line.Gene_symbol, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!geneSymbol_rsIdentifier_dict[data_line.Gene_symbol].ContainsKey(data_line.Rs_identifier))
                {
                    geneSymbol_rsIdentifier_dict[data_line.Gene_symbol].Add(data_line.Rs_identifier, new List<Lincs_vcf_genomic_data_line_class>());
                }
                geneSymbol_rsIdentifier_dict[data_line.Gene_symbol][data_line.Rs_identifier].Add(data_line);
            }
            data = null;
            string[] geneSymbols = geneSymbol_rsIdentifier_dict.Keys.ToArray();
            string geneSymbol;
            int geneSymbols_length = geneSymbols.Length;
            string[] rs_identifiers;
            string rs_identifier;
            int rs_identifiers_length;
            geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
            for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
            {
                geneSymbol = geneSymbols[indexGS];
                rsIdentifier_dict = geneSymbol_rsIdentifier_dict[geneSymbol];
                rs_identifiers = rsIdentifier_dict.Keys.ToArray();
                rs_identifiers = rs_identifiers.OrderBy(l => l).ToArray();
                rs_identifiers_length = rs_identifiers.Length;
                for (int indexCL = 0; indexCL < rs_identifiers_length; indexCL++)
                {
                    rs_identifier = rs_identifiers[indexCL];
                    ordered_list.AddRange(rsIdentifier_dict[rs_identifier]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0) { throw new Exception(); }
                    else if ((this_line.Gene_symbol.Equals(previous_line.Gene_symbol))
                             && (this_line.Rs_identifier.CompareTo(previous_line.Rs_identifier) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_geneSymbol_rsIdentifier_cellline(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>> geneSymbol_rsIdentifier_cellline_dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>();
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> rsIdentifier_cellline_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> cellline_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!geneSymbol_rsIdentifier_cellline_dict.ContainsKey(data_line.Gene_symbol))
                {
                    geneSymbol_rsIdentifier_cellline_dict.Add(data_line.Gene_symbol, new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>());
                }
                if (!geneSymbol_rsIdentifier_cellline_dict[data_line.Gene_symbol].ContainsKey(data_line.Rs_identifier))
                {
                    geneSymbol_rsIdentifier_cellline_dict[data_line.Gene_symbol].Add(data_line.Rs_identifier, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!geneSymbol_rsIdentifier_cellline_dict[data_line.Gene_symbol][data_line.Rs_identifier].ContainsKey(data_line.Cell_line))
                {
                    geneSymbol_rsIdentifier_cellline_dict[data_line.Gene_symbol][data_line.Rs_identifier].Add(data_line.Cell_line, new List<Lincs_vcf_genomic_data_line_class>());
                }
                geneSymbol_rsIdentifier_cellline_dict[data_line.Gene_symbol][data_line.Rs_identifier][data_line.Cell_line].Add(data_line);
            }
            string[] geneSymbols = geneSymbol_rsIdentifier_cellline_dict.Keys.ToArray();
            string geneSymbol;
            int geneSymbols_length = geneSymbols.Length;
            string[] rs_identifiers;
            string rs_identifier;
            int rs_identifiers_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
            for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
            {
                geneSymbol = geneSymbols[indexGS];
                rsIdentifier_cellline_dict = geneSymbol_rsIdentifier_cellline_dict[geneSymbol];
                rs_identifiers = rsIdentifier_cellline_dict.Keys.ToArray();
                rs_identifiers = rs_identifiers.OrderBy(l => l).ToArray();
                rs_identifiers_length = rs_identifiers.Length;
                for (int indexCL = 0; indexCL < rs_identifiers_length; indexCL++)
                {
                    rs_identifier = rs_identifiers[indexCL];
                    cellline_dict = rsIdentifier_cellline_dict[rs_identifier];
                    celllines = cellline_dict.Keys.ToArray();
                    celllines_length = celllines.Length;
                    celllines = celllines.OrderBy(l => l).ToArray();
                    for (int indexCelline=0; indexCelline<celllines_length; indexCelline++)
                    {
                        cellline = celllines[indexCelline];
                        ordered_list.AddRange(cellline_dict[cellline]);
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0) { throw new Exception(); }
                    else if ((this_line.Gene_symbol.Equals(previous_line.Gene_symbol))
                             && (this_line.Rs_identifier.CompareTo(previous_line.Rs_identifier) < 0)) { throw new Exception(); }
                    else if (   (this_line.Gene_symbol.Equals(previous_line.Gene_symbol))
                             && (this_line.Rs_identifier.Equals(previous_line.Rs_identifier))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_cellline_celllineDrugWithOutlierResponse(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> cellline_drug_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> drug_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!cellline_drug_dict.ContainsKey(data_line.Cell_line))
                {
                    cellline_drug_dict.Add(data_line.Cell_line, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!cellline_drug_dict[data_line.Cell_line].ContainsKey(data_line.Cell_line_drug_with_outlier_response))
                {
                    cellline_drug_dict[data_line.Cell_line].Add(data_line.Cell_line_drug_with_outlier_response, new List<Lincs_vcf_genomic_data_line_class>());
                }
                cellline_drug_dict[data_line.Cell_line][data_line.Cell_line_drug_with_outlier_response].Add(data_line);
            }
            string[] celllines = cellline_drug_dict.Keys.ToArray();
            string cellline;
            int celllines_length = celllines.Length;
            celllines = celllines.OrderBy(l => l).ToArray();
            string[] drugs;
            string drug;
            int drugs_length = celllines.Length;
            for (int indexC = 0; indexC < celllines_length; indexC++)
            {
                cellline = celllines[indexC];
                drug_dict = cellline_drug_dict[cellline];
                drugs = drug_dict.Keys.ToArray();
                drugs = drugs.OrderBy(l => l).ToArray();
                drugs_length = drugs.Length;
                for (int indexD = 0; indexD < drugs_length; indexD++)
                {
                    drug = drugs[indexD];
                    ordered_list.AddRange(drug_dict[drug]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0) { throw new Exception(); }
                    else if ((this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Cell_line_drug_with_outlier_response.CompareTo(previous_line.Cell_line_drug_with_outlier_response) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_sideEffect_drugRegulatingScp_ontology_entryType(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, Dictionary<Ontology_type_enum, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>>> sideEffect_drugRegulatingScp_ontology_entryType_dict = new Dictionary<string, Dictionary<string, Dictionary<Ontology_type_enum, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>>>();
            Dictionary<string, Dictionary<Ontology_type_enum, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>> drugRegulatingScp_ontology_entryType_dict = new Dictionary<string, Dictionary<Ontology_type_enum, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>>();
            Dictionary<Ontology_type_enum, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>> ontology_entryType_dict = new Dictionary<Ontology_type_enum, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>> entryType_dict = new Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!sideEffect_drugRegulatingScp_ontology_entryType_dict.ContainsKey(data_line.Side_effect))
                {
                    sideEffect_drugRegulatingScp_ontology_entryType_dict.Add(data_line.Side_effect, new Dictionary<string, Dictionary<Ontology_type_enum, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>>());
                }
                if (!sideEffect_drugRegulatingScp_ontology_entryType_dict[data_line.Side_effect].ContainsKey(data_line.Drug_regulating_scp))
                {
                    sideEffect_drugRegulatingScp_ontology_entryType_dict[data_line.Side_effect].Add(data_line.Drug_regulating_scp, new Dictionary<Ontology_type_enum, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>());
                }
                if (!sideEffect_drugRegulatingScp_ontology_entryType_dict[data_line.Side_effect][data_line.Drug_regulating_scp].ContainsKey(data_line.Ontology))
                {
                    sideEffect_drugRegulatingScp_ontology_entryType_dict[data_line.Side_effect][data_line.Drug_regulating_scp].Add(data_line.Ontology, new Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!sideEffect_drugRegulatingScp_ontology_entryType_dict[data_line.Side_effect][data_line.Drug_regulating_scp][data_line.Ontology].ContainsKey(data_line.Entry_type))
                {
                    sideEffect_drugRegulatingScp_ontology_entryType_dict[data_line.Side_effect][data_line.Drug_regulating_scp][data_line.Ontology].Add(data_line.Entry_type, new List<Lincs_vcf_genomic_data_line_class>());
                }
                sideEffect_drugRegulatingScp_ontology_entryType_dict[data_line.Side_effect][data_line.Drug_regulating_scp][data_line.Ontology][data_line.Entry_type].Add(data_line);
            }
            string[] sideEffects = sideEffect_drugRegulatingScp_ontology_entryType_dict.Keys.ToArray();
            string sideEffect;
            int sideEffects_length = sideEffects.Length;
            string[] drugs;
            string drug;
            int drugs_length;
            Ontology_type_enum[] ontologies;
            Ontology_type_enum ontology;
            int ontologies_length;
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;
            for (int indexSideEffect = 0; indexSideEffect < sideEffects_length; indexSideEffect++)
            {
                sideEffect = sideEffects[indexSideEffect];
                drugRegulatingScp_ontology_entryType_dict = sideEffect_drugRegulatingScp_ontology_entryType_dict[sideEffect];
                drugs = drugRegulatingScp_ontology_entryType_dict.Keys.ToArray();
                drugs_length = drugs.Length;
                drugs = drugs.OrderBy(l => l).ToArray();
                for (int indexDrug = 0; indexDrug < drugs_length; indexDrug++)
                {
                    drug = drugs[indexDrug];
                    ontology_entryType_dict = drugRegulatingScp_ontology_entryType_dict[drug];
                    ontologies = ontology_entryType_dict.Keys.ToArray();
                    ontologies_length = ontologies.Length;
                    ontologies = ontologies.OrderBy(l => l).ToArray();
                    for (int indexOntologies = 0; indexOntologies < ontologies_length; indexOntologies++)
                    {
                        ontology = ontologies[indexOntologies];
                        entryType_dict = ontology_entryType_dict[ontology];
                        entryTypes = entryType_dict.Keys.ToArray();
                        entryTypes = entryTypes.OrderBy(l => l).ToArray();
                        entryTypes_length = entryTypes.Length;
                        for (int indexET = 0; indexET < entryTypes_length; indexET++)
                        {
                            entryType = entryTypes[indexET];
                            ordered_list.AddRange(entryType_dict[entryType]);
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Side_effect.CompareTo(previous_line.Side_effect) < 0) { throw new Exception(); }
                    else if (   (this_line.Side_effect.Equals(previous_line.Side_effect))
                             && (this_line.Drug_regulating_scp.CompareTo(previous_line.Drug_regulating_scp) < 0)
                             ) { throw new Exception(); }
                    else if (   (this_line.Side_effect.Equals(previous_line.Side_effect))
                             && (this_line.Drug_regulating_scp.Equals(previous_line.Drug_regulating_scp))
                             && (this_line.Ontology.CompareTo(previous_line.Ontology) < 0)
                             ) { throw new Exception(); }
                    else if (   (this_line.Side_effect.Equals(previous_line.Side_effect))
                             && (this_line.Drug_regulating_scp.Equals(previous_line.Drug_regulating_scp))
                             && (this_line.Ontology.Equals(previous_line.Ontology))
                             && (this_line.Entry_type.CompareTo(previous_line.Entry_type) < 0)
                             ) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_sideEffect_toxicityAssociation_drugRegulatingScp_entryType(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>>> sideEffect_toxicityAssociation_drug_entryType_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>> toxicityAssociation_drugRegulatingScp_entryType_dict = new Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>>();
            Dictionary<string, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>> drugRegulatingScp_entryType_dict = new Dictionary<string, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>> entryType_dict = new Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!sideEffect_toxicityAssociation_drug_entryType_dict.ContainsKey(data_line.Side_effect))
                {
                    sideEffect_toxicityAssociation_drug_entryType_dict.Add(data_line.Side_effect, new Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>>());
                }
                if (!sideEffect_toxicityAssociation_drug_entryType_dict[data_line.Side_effect].ContainsKey(data_line.Toxicity_association))
                {
                    sideEffect_toxicityAssociation_drug_entryType_dict[data_line.Side_effect].Add(data_line.Toxicity_association, new Dictionary<string, Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>>());
                }
                if (!sideEffect_toxicityAssociation_drug_entryType_dict[data_line.Side_effect][data_line.Toxicity_association].ContainsKey(data_line.Drug_regulating_scp))
                {
                    sideEffect_toxicityAssociation_drug_entryType_dict[data_line.Side_effect][data_line.Toxicity_association].Add(data_line.Drug_regulating_scp, new Dictionary<DE_entry_enum, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!sideEffect_toxicityAssociation_drug_entryType_dict[data_line.Side_effect][data_line.Toxicity_association][data_line.Drug_regulating_scp].ContainsKey(data_line.Entry_type))
                {
                    sideEffect_toxicityAssociation_drug_entryType_dict[data_line.Side_effect][data_line.Toxicity_association][data_line.Drug_regulating_scp].Add(data_line.Entry_type, new List<Lincs_vcf_genomic_data_line_class>());
                }
                sideEffect_toxicityAssociation_drug_entryType_dict[data_line.Side_effect][data_line.Toxicity_association][data_line.Drug_regulating_scp][data_line.Entry_type].Add(data_line);
            }
            string[] sideEffects = sideEffect_toxicityAssociation_drug_entryType_dict.Keys.ToArray();
            string sideEffect;
            int sideEffects_length = sideEffects.Length;
            string[] toxicityAssociations;
            string toxicityAssociation;
            int toxicityAssociations_length;
            string[] drugs;
            string drug;
            int drugs_length;
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;
            for (int indexSideEffect = 0; indexSideEffect < sideEffects_length; indexSideEffect++)
            {
                sideEffect = sideEffects[indexSideEffect];
                toxicityAssociation_drugRegulatingScp_entryType_dict = sideEffect_toxicityAssociation_drug_entryType_dict[sideEffect];
                toxicityAssociations = toxicityAssociation_drugRegulatingScp_entryType_dict.Keys.ToArray();
                toxicityAssociations_length = toxicityAssociations.Length;
                toxicityAssociations = toxicityAssociations.OrderBy(l => l).ToArray();
                for (int indexToxicity = 0; indexToxicity < toxicityAssociations_length; indexToxicity++)
                {
                    toxicityAssociation = toxicityAssociations[indexToxicity];
                    drugRegulatingScp_entryType_dict = toxicityAssociation_drugRegulatingScp_entryType_dict[toxicityAssociation];
                    drugs = drugRegulatingScp_entryType_dict.Keys.ToArray();
                    drugs_length = drugs.Length;
                    drugs = drugs.OrderBy(l => l).ToArray();
                    for (int indexDrug = 0; indexDrug < drugs_length; indexDrug++)
                    {
                        drug = drugs[indexDrug];
                        entryType_dict = drugRegulatingScp_entryType_dict[drug];
                        entryTypes = entryType_dict.Keys.ToArray();
                        entryTypes = entryTypes.OrderBy(l => l).ToArray();
                        entryTypes_length = entryTypes.Length;
                        for (int indexET = 0; indexET < entryTypes_length; indexET++)
                        {
                            entryType = entryTypes[indexET];
                            ordered_list.AddRange(entryType_dict[entryType]);
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Side_effect.CompareTo(previous_line.Side_effect) < 0) { throw new Exception(); }
                    else if ((this_line.Side_effect.Equals(previous_line.Side_effect))
                             && (this_line.Toxicity_association.CompareTo(previous_line.Toxicity_association) < 0)
                             ) { throw new Exception(); }
                    else if ((this_line.Side_effect.Equals(previous_line.Side_effect))
                             && (this_line.Toxicity_association.Equals(previous_line.Toxicity_association))
                             && (this_line.Drug_regulating_scp.CompareTo(previous_line.Drug_regulating_scp) < 0)
                             ) { throw new Exception(); }
                    else if (   (this_line.Side_effect.Equals(previous_line.Side_effect))
                             && (this_line.Toxicity_association.Equals(previous_line.Toxicity_association))
                             && (this_line.Drug_regulating_scp.Equals((previous_line.Drug_regulating_scp)))
                             && (this_line.Entry_type.CompareTo(previous_line.Entry_type) < 0)
                             ) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_drug_drugTargetSymbol(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> drug_drugTargetSymbol_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> drugTargetSymbol_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!drug_drugTargetSymbol_dict.ContainsKey(data_line.Cell_line_drug_with_outlier_response))
                {
                    drug_drugTargetSymbol_dict.Add(data_line.Cell_line_drug_with_outlier_response, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!drug_drugTargetSymbol_dict[data_line.Cell_line_drug_with_outlier_response].ContainsKey(data_line.Drug_target_symbol))
                {
                    drug_drugTargetSymbol_dict[data_line.Cell_line_drug_with_outlier_response].Add(data_line.Drug_target_symbol, new List<Lincs_vcf_genomic_data_line_class>());
                }
                drug_drugTargetSymbol_dict[data_line.Cell_line_drug_with_outlier_response][data_line.Drug_target_symbol].Add(data_line);
            }
            string[] drugs = drug_drugTargetSymbol_dict.Keys.ToArray();
            string drug;
            int drugs_length = drugs.Length;
            string[] drugTargetSymbols;
            string drugTargetSymbol;
            int drugTargetSymbols_length;
            drugs = drugs.OrderBy(l => l).ToArray();
            for (int indexDrug = 0; indexDrug < drugs_length; indexDrug++)
            {
                drug = drugs[indexDrug];
                drugTargetSymbol_dict = drug_drugTargetSymbol_dict[drug];
                drugTargetSymbols = drugTargetSymbol_dict.Keys.ToArray();
                drugTargetSymbols = drugTargetSymbols.OrderBy(l => l).ToArray();
                drugTargetSymbols_length = drugTargetSymbols.Length;
                for (int indexDrugTargetSymbol = 0; indexDrugTargetSymbol < drugTargetSymbols_length; indexDrugTargetSymbol++)
                {
                    drugTargetSymbol = drugTargetSymbols[indexDrugTargetSymbol];
                    ordered_list.AddRange(drugTargetSymbol_dict[drugTargetSymbol]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Cell_line_drug_with_outlier_response.CompareTo(previous_line.Cell_line_drug_with_outlier_response) < 0) { throw new Exception(); }
                    else if ((this_line.Cell_line_drug_with_outlier_response.Equals(previous_line.Cell_line_drug_with_outlier_response))
                             && (this_line.Drug_target_symbol.CompareTo(previous_line.Drug_target_symbol) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_drug_geneSymbol(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> drug_geneSymbol_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> geneSymbol_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!drug_geneSymbol_dict.ContainsKey(data_line.Cell_line_drug_with_outlier_response))
                {
                    drug_geneSymbol_dict.Add(data_line.Cell_line_drug_with_outlier_response, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!drug_geneSymbol_dict[data_line.Cell_line_drug_with_outlier_response].ContainsKey(data_line.Gene_symbol))
                {
                    drug_geneSymbol_dict[data_line.Cell_line_drug_with_outlier_response].Add(data_line.Gene_symbol, new List<Lincs_vcf_genomic_data_line_class>());
                }
                drug_geneSymbol_dict[data_line.Cell_line_drug_with_outlier_response][data_line.Gene_symbol].Add(data_line);
            }
            string[] drugs = drug_geneSymbol_dict.Keys.ToArray();
            string drug;
            int drugs_length = drugs.Length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            drugs = drugs.OrderBy(l => l).ToArray();
            for (int indexDrug = 0; indexDrug < drugs_length; indexDrug++)
            {
                drug = drugs[indexDrug];
                geneSymbol_dict = drug_geneSymbol_dict[drug];
                geneSymbols = geneSymbol_dict.Keys.ToArray();
                geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                geneSymbols_length = geneSymbols.Length;
                for (int indexDrugTargetSymbol = 0; indexDrugTargetSymbol < geneSymbols_length; indexDrugTargetSymbol++)
                {
                    geneSymbol = geneSymbols[indexDrugTargetSymbol];
                    ordered_list.AddRange(geneSymbol_dict[geneSymbol]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Cell_line_drug_with_outlier_response.CompareTo(previous_line.Cell_line_drug_with_outlier_response) < 0) { throw new Exception(); }
                    else if ((this_line.Cell_line_drug_with_outlier_response.Equals(previous_line.Cell_line_drug_with_outlier_response))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_drug_rsIdentifier(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> drug_rsIdentifier_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> rsIdentifier_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!drug_rsIdentifier_dict.ContainsKey(data_line.Cell_line_drug_with_outlier_response))
                {
                    drug_rsIdentifier_dict.Add(data_line.Cell_line_drug_with_outlier_response, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!drug_rsIdentifier_dict[data_line.Cell_line_drug_with_outlier_response].ContainsKey(data_line.Rs_identifier))
                {
                    drug_rsIdentifier_dict[data_line.Cell_line_drug_with_outlier_response].Add(data_line.Rs_identifier, new List<Lincs_vcf_genomic_data_line_class>());
                }
                drug_rsIdentifier_dict[data_line.Cell_line_drug_with_outlier_response][data_line.Rs_identifier].Add(data_line);
            }
            string[] drugs = drug_rsIdentifier_dict.Keys.ToArray();
            string drug;
            int drugs_length = drugs.Length;
            string[] rsIdentifiers;
            string rsIdentifier;
            int rsIdentifiers_length;
            drugs = drugs.OrderBy(l => l).ToArray();
            for (int indexDrug = 0; indexDrug < drugs_length; indexDrug++)
            {
                drug = drugs[indexDrug];
                rsIdentifier_dict = drug_rsIdentifier_dict[drug];
                rsIdentifiers = rsIdentifier_dict.Keys.ToArray();
                rsIdentifiers = rsIdentifiers.OrderBy(l => l).ToArray();
                rsIdentifiers_length = rsIdentifiers.Length;
                for (int indexRSIdentifier = 0; indexRSIdentifier < rsIdentifiers_length; indexRSIdentifier++)
                {
                    rsIdentifier = rsIdentifiers[indexRSIdentifier];
                    ordered_list.AddRange(rsIdentifier_dict[rsIdentifier]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Cell_line_drug_with_outlier_response.CompareTo(previous_line.Cell_line_drug_with_outlier_response) < 0) { throw new Exception(); }
                    else if ((this_line.Cell_line_drug_with_outlier_response.Equals(previous_line.Cell_line_drug_with_outlier_response))
                             && (this_line.Rs_identifier.CompareTo(previous_line.Rs_identifier) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_chromosome_start_end_cellline(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>> chr_start_end_cellline_dict = new Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>();
            Dictionary<int, Dictionary<int, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>> start_end_cellline_dict = new Dictionary<int, Dictionary<int, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>();
            Dictionary<int, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> end_cellline_dict = new Dictionary<int, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> cellline_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!chr_start_end_cellline_dict.ContainsKey(data_line.Chrom))
                {
                    chr_start_end_cellline_dict.Add(data_line.Chrom, new Dictionary<int, Dictionary<int, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>());
                }
                if (!chr_start_end_cellline_dict[data_line.Chrom].ContainsKey(data_line.Start))
                {
                    chr_start_end_cellline_dict[data_line.Chrom].Add(data_line.Start, new Dictionary<int, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>());
                }
                if (!chr_start_end_cellline_dict[data_line.Chrom][data_line.Start].ContainsKey(data_line.End))
                {
                    chr_start_end_cellline_dict[data_line.Chrom][data_line.Start].Add(data_line.End, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!chr_start_end_cellline_dict[data_line.Chrom][data_line.Start][data_line.End].ContainsKey(data_line.Cell_line))
                {
                    chr_start_end_cellline_dict[data_line.Chrom][data_line.Start][data_line.End].Add(data_line.Cell_line, new List<Lincs_vcf_genomic_data_line_class>());
                }
                chr_start_end_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line].Add(data_line);
            }
            string[] chroms = chr_start_end_cellline_dict.Keys.ToArray();
            string chrom;
            int chroms_length = chroms.Length;
            int[] starts;
            int start;
            int starts_length;
            int[] ends;
            int end;
            int ends_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            chroms = chroms.OrderBy(l => l).ToArray();
            for (int indexC = 0; indexC < chroms_length; indexC++)
            {
                chrom = chroms[indexC];
                start_end_cellline_dict = chr_start_end_cellline_dict[chrom];
                starts = start_end_cellline_dict.Keys.ToArray();
                starts = starts.OrderBy(l => l).ToArray();
                starts_length = starts.Length;
                for (int indexS = 0; indexS < starts_length; indexS++)
                {
                    start = starts[indexS];
                    end_cellline_dict = start_end_cellline_dict[start];
                    ends = end_cellline_dict.Keys.ToArray();
                    ends = ends.OrderBy(l => l).ToArray();
                    ends_length = ends.Length;
                    for (int indexE = 0; indexE < ends_length; indexE++)
                    {
                        end = ends[indexE];
                        cellline_dict = end_cellline_dict[end];
                        celllines = cellline_dict.Keys.ToArray();
                        celllines = celllines.OrderBy(l => l).ToArray();
                        celllines_length = celllines.Length;
                        for (int indexCL = 0; indexCL < celllines_length; indexCL++)
                        {
                            cellline = celllines[indexCL];
                            ordered_list.AddRange(cellline_dict[cellline]);
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Chrom.CompareTo(previous_line.Chrom) < 0) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.CompareTo(previous_line.Start) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.CompareTo(previous_line.End) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.Equals(previous_line.End))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_chromosome_start_end_geneSymbol_cellline(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>> chr_start_end_geneSymbol_cellline_dict = new Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>();
            Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>> start_end_geneSymbol_cellline_dict = new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>();
            Dictionary<int, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>> end_geneSymbol_cellline_dict = new Dictionary<int, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>();
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> geneSymbol_cellline_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> cellline_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!chr_start_end_geneSymbol_cellline_dict.ContainsKey(data_line.Chrom))
                {
                    chr_start_end_geneSymbol_cellline_dict.Add(data_line.Chrom, new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>());
                }
                if (!chr_start_end_geneSymbol_cellline_dict[data_line.Chrom].ContainsKey(data_line.Start))
                {
                    chr_start_end_geneSymbol_cellline_dict[data_line.Chrom].Add(data_line.Start, new Dictionary<int, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>());
                }
                if (!chr_start_end_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start].ContainsKey(data_line.End))
                {
                    chr_start_end_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start].Add(data_line.End, new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>());
                }
                if (!chr_start_end_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End].ContainsKey(data_line.Gene_symbol))
                {
                    chr_start_end_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End].Add(data_line.Gene_symbol, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!chr_start_end_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Gene_symbol].ContainsKey(data_line.Cell_line))
                {
                    chr_start_end_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Gene_symbol].Add(data_line.Cell_line, new List<Lincs_vcf_genomic_data_line_class>());
                }
                chr_start_end_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Gene_symbol][data_line.Cell_line].Add(data_line);
            }
            string[] chroms = chr_start_end_geneSymbol_cellline_dict.Keys.ToArray();
            string chrom;
            int chroms_length = chroms.Length;
            int[] starts;
            int start;
            int starts_length;
            int[] ends;
            int end;
            int ends_length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            chroms = chroms.OrderBy(l => l).ToArray();
            for (int indexC = 0; indexC < chroms_length; indexC++)
            {
                chrom = chroms[indexC];
                start_end_geneSymbol_cellline_dict = chr_start_end_geneSymbol_cellline_dict[chrom];
                starts = start_end_geneSymbol_cellline_dict.Keys.ToArray();
                starts = starts.OrderBy(l => l).ToArray();
                starts_length = starts.Length;
                for (int indexS = 0; indexS < starts_length; indexS++)
                {
                    start = starts[indexS];
                    end_geneSymbol_cellline_dict = start_end_geneSymbol_cellline_dict[start];
                    ends = end_geneSymbol_cellline_dict.Keys.ToArray();
                    ends = ends.OrderBy(l => l).ToArray();
                    ends_length = ends.Length;
                    for (int indexE = 0; indexE < ends_length; indexE++)
                    {
                        end = ends[indexE];
                        geneSymbol_cellline_dict = end_geneSymbol_cellline_dict[end];
                        geneSymbols = geneSymbol_cellline_dict.Keys.ToArray();
                        geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                        geneSymbols_length = geneSymbols.Length;
                        for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
                        {
                            geneSymbol = geneSymbols[indexGS];
                            cellline_dict = geneSymbol_cellline_dict[geneSymbol];
                            celllines = cellline_dict.Keys.ToArray();
                            celllines = celllines.OrderBy(l => l).ToArray();
                            celllines_length = celllines.Length;
                            for (int indexCL = 0; indexCL < celllines_length; indexCL++)
                            {
                                cellline = celllines[indexCL];
                                ordered_list.AddRange(cellline_dict[cellline]);
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Chrom.CompareTo(previous_line.Chrom) < 0) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.CompareTo(previous_line.Start) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.CompareTo(previous_line.End) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.Equals(previous_line.End))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.Equals(previous_line.End))
                             && (this_line.Gene_symbol.Equals(previous_line.Gene_symbol))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_chromosome_start_end_drug_drugTargetSymbol_relationToDrug_geneSymbol_cellline(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>>>> chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = new Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>>>>();
            Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>>> start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>>>();
            Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>> end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = new Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>> drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = new Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>();
            Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>> drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = new Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>();
            Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>> relationToDrug_geneSymbol_cellline_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>();
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> geneSymbol_cellline_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> cellline_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();



            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict.ContainsKey(data_line.Chrom))
                {
                    chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict.Add(data_line.Chrom, new Dictionary<int, Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>>>());
                }
                if (!chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom].ContainsKey(data_line.Start))
                {
                    chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom].Add(data_line.Start, new Dictionary<int, Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>>());
                }
                if (!chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start].ContainsKey(data_line.End))
                {
                    chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start].Add(data_line.End, new Dictionary<string, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>>());
                }
                if (!chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End].ContainsKey(data_line.Cell_line_drug_with_outlier_response))
                {
                    chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End].Add(data_line.Cell_line_drug_with_outlier_response, new Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>>());
                }
                if (!chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response].ContainsKey(data_line.Drug_target_symbol))
                {
                    chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response].Add(data_line.Drug_target_symbol, new Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>>());
                }
                if (!chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response][data_line.Drug_target_symbol].ContainsKey(data_line.Relation_of_gene_symbol_to_drug))
                {
                    chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response][data_line.Drug_target_symbol].Add(data_line.Relation_of_gene_symbol_to_drug, new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>());
                }
                if (!chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response][data_line.Drug_target_symbol][data_line.Relation_of_gene_symbol_to_drug].ContainsKey(data_line.Gene_symbol))
                {
                    chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response][data_line.Drug_target_symbol][data_line.Relation_of_gene_symbol_to_drug].Add(data_line.Gene_symbol, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response][data_line.Drug_target_symbol][data_line.Relation_of_gene_symbol_to_drug][data_line.Gene_symbol].ContainsKey(data_line.Cell_line))
                {
                    chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response][data_line.Drug_target_symbol][data_line.Relation_of_gene_symbol_to_drug][data_line.Gene_symbol].Add(data_line.Cell_line, new List<Lincs_vcf_genomic_data_line_class>());
                }
                chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[data_line.Chrom][data_line.Start][data_line.End][data_line.Cell_line_drug_with_outlier_response][data_line.Drug_target_symbol][data_line.Relation_of_gene_symbol_to_drug][data_line.Gene_symbol][data_line.Cell_line].Add(data_line);
            }
            string[] chroms = chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict.Keys.ToArray();
            string chrom;
            int chroms_length = chroms.Length;
            int[] starts;
            int start;
            int starts_length;
            int[] ends;
            int end;
            int ends_length;
            string[] drugs;
            string drug;
            int drugs_length;
            string[] drugTargetSymbols;
            string drugTargetSymbol;
            int drugTargetSymbols_length;
            Relation_of_gene_symbol_to_drug_enum[] relationOfGeneSymboltoDrugs;
            Relation_of_gene_symbol_to_drug_enum relationOfGeneSymboltoDrug;
            int relationOfGeneSymboltoDrugs_length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            chroms = chroms.OrderBy(l => l).ToArray();
            for (int indexC = 0; indexC < chroms_length; indexC++)
            {
                chrom = chroms[indexC];
                start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = chromosome_start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[chrom];
                starts = start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict.Keys.ToArray();
                starts = starts.OrderBy(l => l).ToArray();
                starts_length = starts.Length;
                for (int indexS = 0; indexS < starts_length; indexS++)
                {
                    start = starts[indexS];
                    end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = start_end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[start];
                    ends = end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict.Keys.ToArray();
                    ends = ends.OrderBy(l => l).ToArray();
                    ends_length = ends.Length;
                    for (int indexE = 0; indexE < ends_length; indexE++)
                    {
                        end = ends[indexE];
                        drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = end_drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[end];
                        drugs = drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict.Keys.ToArray();
                        drugs = drugs.OrderBy(l => l).ToArray();
                        drugs_length = drugs.Length;
                        for (int indexDrug = 0; indexDrug < drugs_length; indexDrug++)
                        {
                            drug = drugs[indexDrug];
                            drugTargetProtein_relationToDrug_geneSymbol_cellline_dict = drug_drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[drug];
                            drugTargetSymbols = drugTargetProtein_relationToDrug_geneSymbol_cellline_dict.Keys.ToArray();
                            drugTargetSymbols = drugTargetSymbols.OrderBy(l => l).ToArray();
                            drugTargetSymbols_length = drugTargetSymbols.Length;
                            for (int indexCL = 0; indexCL < drugTargetSymbols_length; indexCL++)
                            {
                                drugTargetSymbol = drugTargetSymbols[indexCL];
                                relationToDrug_geneSymbol_cellline_dict = drugTargetProtein_relationToDrug_geneSymbol_cellline_dict[drugTargetSymbol];
                                relationOfGeneSymboltoDrugs = relationToDrug_geneSymbol_cellline_dict.Keys.ToArray();
                                relationOfGeneSymboltoDrugs = relationOfGeneSymboltoDrugs.OrderBy(l => l).ToArray();
                                relationOfGeneSymboltoDrugs_length = relationOfGeneSymboltoDrugs.Length;
                                for (int indexRel=0; indexRel<relationOfGeneSymboltoDrugs_length;indexRel++)
                                {
                                    relationOfGeneSymboltoDrug = relationOfGeneSymboltoDrugs[indexRel];
                                    geneSymbol_cellline_dict = relationToDrug_geneSymbol_cellline_dict[relationOfGeneSymboltoDrug];
                                    geneSymbols = geneSymbol_cellline_dict.Keys.ToArray();
                                    geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                                    geneSymbols_length = geneSymbols.Length;
                                    for (int indexGeneSymbol=0; indexGeneSymbol<geneSymbols_length;indexGeneSymbol++)
                                    {
                                        geneSymbol = geneSymbols[indexGeneSymbol];
                                        cellline_dict = geneSymbol_cellline_dict[geneSymbol];
                                        celllines = cellline_dict.Keys.ToArray();
                                        celllines = celllines.OrderBy(l => l).ToArray();
                                        celllines_length = celllines.Length;
                                        for (int indexCellline=0;indexCellline<celllines_length;indexCellline++)
                                        {
                                            cellline = celllines[indexCellline];
                                            ordered_list.AddRange(cellline_dict[cellline]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Chrom.CompareTo(previous_line.Chrom) < 0) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.CompareTo(previous_line.Start) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.CompareTo(previous_line.End) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.Equals(previous_line.End))
                             && (this_line.Cell_line_drug_with_outlier_response.CompareTo(previous_line.Cell_line_drug_with_outlier_response) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.Equals(previous_line.End))
                             && (this_line.Cell_line_drug_with_outlier_response.Equals(previous_line.Cell_line_drug_with_outlier_response))
                             && (this_line.Drug_target_symbol.CompareTo(previous_line.Drug_target_symbol) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.Equals(previous_line.End))
                             && (this_line.Cell_line_drug_with_outlier_response.Equals(previous_line.Cell_line_drug_with_outlier_response))
                             && (this_line.Drug_target_symbol.Equals(previous_line.Drug_target_symbol))
                             && (this_line.Relation_of_gene_symbol_to_drug.CompareTo(previous_line.Relation_of_gene_symbol_to_drug) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.Equals(previous_line.End))
                             && (this_line.Cell_line_drug_with_outlier_response.Equals(previous_line.Cell_line_drug_with_outlier_response))
                             && (this_line.Drug_target_symbol.Equals(previous_line.Drug_target_symbol))
                             && (this_line.Relation_of_gene_symbol_to_drug.Equals(previous_line.Relation_of_gene_symbol_to_drug))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                    else if ((this_line.Chrom.Equals(previous_line.Chrom))
                             && (this_line.Start.Equals(previous_line.Start))
                             && (this_line.End.Equals(previous_line.End))
                             && (this_line.Cell_line_drug_with_outlier_response.Equals(previous_line.Cell_line_drug_with_outlier_response))
                             && (this_line.Drug_target_symbol.Equals(previous_line.Drug_target_symbol))
                             && (this_line.Relation_of_gene_symbol_to_drug.Equals(previous_line.Relation_of_gene_symbol_to_drug))
                             && (this_line.Gene_symbol.Equals(previous_line.Gene_symbol))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_cellline(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> cellline_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!cellline_dict.ContainsKey(data_line.Cell_line))
                {
                    cellline_dict.Add(data_line.Cell_line, new List<Lincs_vcf_genomic_data_line_class>());
                }
                cellline_dict[data_line.Cell_line].Add(data_line);
            }
            string[] celllines = cellline_dict.Keys.ToArray();
            string cellline;
            celllines = celllines.OrderBy(l => l).ToArray();
            int celllines_length = celllines.Length;
            for (int indexCL = 0; indexCL < celllines_length; indexCL++)
            {
                cellline = celllines[indexCL];
                ordered_list.AddRange(cellline_dict[cellline]);
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_ontology(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<Ontology_type_enum, List<Lincs_vcf_genomic_data_line_class>> ontology_dict = new Dictionary<Ontology_type_enum, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!ontology_dict.ContainsKey(data_line.Ontology))
                {
                    ontology_dict.Add(data_line.Ontology, new List<Lincs_vcf_genomic_data_line_class>());
                }
                ontology_dict[data_line.Ontology].Add(data_line);
            }
            Ontology_type_enum[] ontologies = ontology_dict.Keys.ToArray();
            Ontology_type_enum ontology;
            ontologies = ontologies.OrderBy(l => l).ToArray();
            int ontologies_length = ontologies.Length;
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                ontology = ontologies[indexO];
                ordered_list.AddRange(ontology_dict[ontology]);
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Ontology.CompareTo(previous_line.Ontology) < 0) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_geneSymbol(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> symbol_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!symbol_dict.ContainsKey(data_line.Gene_symbol))
                {
                    symbol_dict.Add(data_line.Gene_symbol, new List<Lincs_vcf_genomic_data_line_class>());
                }
                symbol_dict[data_line.Gene_symbol].Add(data_line);
            }
            string[] symbols = symbol_dict.Keys.ToArray();
            string symbol;
            symbols = symbols.OrderBy(l => l).ToArray();
            int symbols_length = symbols.Length;
            for (int indexCL = 0; indexCL < symbols_length; indexCL++)
            {
                symbol = symbols[indexCL];
                ordered_list.AddRange(symbol_dict[symbol]);
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_rsIdentifer(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> rsIdentifier_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!rsIdentifier_dict.ContainsKey(data_line.Rs_identifier))
                {
                    rsIdentifier_dict.Add(data_line.Rs_identifier, new List<Lincs_vcf_genomic_data_line_class>());
                }
                rsIdentifier_dict[data_line.Rs_identifier].Add(data_line);
            }
            string[] rsIdentifers = rsIdentifier_dict.Keys.ToArray();
            string rsIdentifer;
            rsIdentifers = rsIdentifers.OrderBy(l => l).ToArray();
            int rsIdentifers_length = rsIdentifers.Length;
            for (int indexRs = 0; indexRs < rsIdentifers_length; indexRs++)
            {
                rsIdentifer = rsIdentifers[indexRs];
                ordered_list.AddRange(rsIdentifier_dict[rsIdentifer]);
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Rs_identifier.CompareTo(previous_line.Rs_identifier) < 0) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }

        public static Lincs_vcf_genomic_data_line_class[] Order_by_rsIdentifier_geneSymbol(Lincs_vcf_genomic_data_line_class[] data)
        {
            Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>> rsIdentifier_geneSymbol_dict = new Dictionary<string, Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>>();
            Dictionary<string, List<Lincs_vcf_genomic_data_line_class>> geneSymbol_dict = new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>();
            int data_length = data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> ordered_list = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = data[indexData];
                if (!rsIdentifier_geneSymbol_dict.ContainsKey(data_line.Rs_identifier))
                {
                    rsIdentifier_geneSymbol_dict.Add(data_line.Rs_identifier, new Dictionary<string, List<Lincs_vcf_genomic_data_line_class>>());
                }
                if (!rsIdentifier_geneSymbol_dict[data_line.Rs_identifier].ContainsKey(data_line.Gene_symbol))
                {
                    rsIdentifier_geneSymbol_dict[data_line.Rs_identifier].Add(data_line.Gene_symbol, new List<Lincs_vcf_genomic_data_line_class>());
                }
                rsIdentifier_geneSymbol_dict[data_line.Rs_identifier][data_line.Gene_symbol].Add(data_line);
            }
            string[] rsIdentifiers = rsIdentifier_geneSymbol_dict.Keys.ToArray();
            string rsIdentifier;
            int rsIdentifiers_length = rsIdentifiers.Length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            rsIdentifiers = rsIdentifiers.OrderBy(l => l).ToArray();
            for (int indexRsIDs = 0; indexRsIDs < rsIdentifiers_length; indexRsIDs++)
            {
                rsIdentifier = rsIdentifiers[indexRsIDs];
                geneSymbol_dict = rsIdentifier_geneSymbol_dict[rsIdentifier];
                geneSymbols = geneSymbol_dict.Keys.ToArray();
                geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                geneSymbols_length = geneSymbols.Length;
                for (int indexDrugTargetSymbol = 0; indexDrugTargetSymbol < geneSymbols_length; indexDrugTargetSymbol++)
                {
                    geneSymbol = geneSymbols[indexDrugTargetSymbol];
                    ordered_list.AddRange(geneSymbol_dict[geneSymbol]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_list.Count;
                if (ordered_length != data_length) { throw new Exception(); }
                Lincs_vcf_genomic_data_line_class previous_line;
                Lincs_vcf_genomic_data_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_list[indexO];
                    previous_line = ordered_list[indexO - 1];
                    if (this_line.Rs_identifier.CompareTo(previous_line.Rs_identifier) < 0) { throw new Exception(); }
                    else if ((this_line.Rs_identifier.Equals(previous_line.Rs_identifier))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                }
            }
            return ordered_list.ToArray();
        }
        #endregion

        public Lincs_vcf_genomic_data_line_class()
        {
            this.Other_cell_lines_minor_allele_counts = new int[0];
            this.Chrom = "";
            this.Gene_symbol = "";
            this.Minor_allele = "";
            this.Drug_is_cardiotoxic = "";
            this.Drug_target_symbol = "";
            this.Major_alleles = new string[0];
            this.Cell_line = "";
            this.Cell_line_genotype_GT = "";
            this.F1_score_weigths_with_outlier_responses = new float[0];
            this.Cell_line_drug_with_outlier_response = "";
            this.Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments = false;
            this.Rs_identifier = "";
            this.Publication = "";
        }

        public Lincs_vcf_genomic_data_line_class Deep_copy()
        {
            Lincs_vcf_genomic_data_line_class copy = (Lincs_vcf_genomic_data_line_class)this.MemberwiseClone();
            copy.Chrom = (string)this.Chrom.Clone();
            copy.Gene_symbol = (string)this.Gene_symbol.Clone();
            copy.Drug_is_cardiotoxic = (string)this.Drug_is_cardiotoxic.Clone();
            copy.Rs_identifier = (string)this.Rs_identifier.Clone();
            copy.Drug_target_symbol = (string)this.Drug_target_symbol.Clone();
            copy.Minor_allele = (string)this.Minor_allele.Clone();
            copy.Major_alleles = Array_class.Deep_copy_string_array(this.Major_alleles);
            copy.Clnsig = (string)this.Clnsig.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            copy.Gtex_atrial_eQTL = (string)this.Gtex_atrial_eQTL.Clone();
            copy.Gtex_atrial_sQTL = (string)this.Gtex_atrial_sQTL.Clone();
            copy.Gtex_ventricular_eQTL = (string)this.Gtex_ventricular_eQTL.Clone();
            copy.Gtex_ventricular_sQTL = (string)this.Gtex_ventricular_sQTL.Clone();
            copy.Cell_line_drug_with_outlier_response = (string)this.Cell_line_drug_with_outlier_response.Clone();
            copy.Cell_line_genotype_GT = (string)this.Cell_line_genotype_GT.Clone();
            copy.Publication = (string)this.Publication.Clone();
            copy.F1_score_weigths_with_outlier_responses = Array_class.Deep_copy_array(this.F1_score_weigths_with_outlier_responses);
            return copy;
        }
    }

    class Lincs_vcf_genomic_data_readWriteOptions_class : ReadWriteOptions_base
    {
        public Lincs_vcf_genomic_data_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Ontology","Cell_line","Cell_line_drug_with_outlier_response","Rs_identifier","Chrom","Start","End","Gene_symbol","Drug_target_symbol","Relation_of_gene_symbol_to_drug","Rank_of_gene_symbol_within_relation_for_drug","Variant_location","Minor_allele","Minor_allel_frequency","Cadd_phred","SpAI_DS_snv_raw","SpAI_DS_indel_raw","Quality_aq","Clnsig","Gtex_atrial_eQTL","Gtex_atrial_sQTL","Gtex_ventricular_eQTL","Gtex_ventricular_sQTL",
                                                    "Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments","Cell_line_minor_allele_count","Cell_line_genotype_GT","Cell_line_allele_depth_AD","Cell_line_approximate_read_depth_DP","Cell_line_phred_scale_genotype_quality_GQ",
                                                    "Successfully_sequenced_celllines_count",
                                                    "Cell_line_outlier_adj_pvalue","Cell_line_outlier_pvalue","Cell_line_outlier_mean_f1_score_without_outlier","Cell_line_outlier_F1_score_weight",
                                                    "Cell_line_conditional_genotype_quality","Cell_line_frequency_of_cell_line_minor_allele","Publication",
                                                    "ReadWrite_other_cell_lines_minor_allele_counts",
                                                    "ReadWrite_major_alleles","ReadWrite_F1_score_weigths_with_outlier_responses" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_vcf_genomic_data_mapped_to_scps_readWriteOptions_class : ReadWriteOptions_base
    {
        public Lincs_vcf_genomic_data_mapped_to_scps_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;

            this.Key_propertyNames = new string[] { "Chrom", //0
                                                    "Start", //1
                                                    "End", //2
                                                    "Rs_identifier", //3
                                                    "Scp_gene", //4
                                                    "Drug_regulating_scp",//5
                                                    "Drug_is_cardiotoxic",
                                                    "Drug_type", //6
                                                    "Ontology",//7
                                                    "Scp",//8
                                                    "Entry_type", //9
                                                  };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_vcf_genomic_data_mapped_to_scps_supplTable_readWriteOptions_class : ReadWriteOptions_base
    {
        public Lincs_vcf_genomic_data_mapped_to_scps_supplTable_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;

            this.Key_propertyNames = new string[] { "Chrom", //0
                                                    "Start", //1
                                                    "End", //2
                                                    "Rs_identifier", //3
                                                    "Scp_gene", //4
                                                    "Variant_location",
                                                    "Drug_regulating_scp",//5
                                                    "Drug_is_cardiotoxic",
                                                    "Drug_type", //6
                                                    "Ontology",//7
                                                    "Scp",//8
                                                    "Entry_type_description", //9
                                                  };
            this.Key_columnNames = new string[] {   "Chromosome", //0
                                                    "Start position", //1
                                                    "End position", //2
                                                    "Variant RS ID", //3
                                                    Lincs_website_conversion_class.Label_gene_symbol, //4
                                                    "Location of variant",
                                                    "Potentially affected drug", //5
                                                    Lincs_website_conversion_class.Label_is_cardiotoxic,
                                                    Lincs_website_conversion_class.Label_drug_class, //6
                                                    "MBCO SCP level",//7
                                                    "SCP gene is annotated to", //8
                                                    "Up- or downregulated SCP" //9
                                                  };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_vcf_genomic_data_mapped_to_scps_forWebsite_readWriteOptions_class : ReadWriteOptions_base
    {
        public Lincs_vcf_genomic_data_mapped_to_scps_forWebsite_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;

            this.Key_propertyNames = new string[] { "Rs_identifier", //3
                                                    "Scp_gene", //2
                                                    "Scp",//0
                                                    "Entry_type_description", //1
                                                    "Variant_location",//4
                                                    "Chrom", //5
                                                    "Start", //6
                                                    "End", //7
                                                    "Drug_regulating_scp"//8
                                                    //"Drug_is_cardiotoxic",
                                                    //"Drug_type", //6
                                                    //"Ontology",//7
                                                    
                                                  };
            this.Key_columnNames = new string[] {   "Variant RS ID",
                                                    "Variant gene", //2
                                                    "Pathway", //0
                                                    "Up- or down", //1
                                                    "Location of variant", //4
                                                    "Chromosome", //5
                                                    "Start position", //6
                                                    "End position", //7
                                                    "Potentially affected cardiotoxic TKI(s)" //8
                                                    //Lincs_adverseEvent_conversion_class.Label_is_cardiotoxic,
                                                    //Lincs_adverseEvent_conversion_class.Label_drug_class, //6
                                                    //"MBCO SCP level",//7
                                                    
                                                  };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_vcf_genomic_data_supplTable_readWriteOptions_class : ReadWriteOptions_base
    {
        public Lincs_vcf_genomic_data_supplTable_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;


            this.Key_propertyNames = new string[] { "Rs_identifier", //3
                                                    "Gene_symbol", //6
                                                    "Variant_location_description", //10
                                                    "Relation_of_gene_symbol_to_drug_description", //7
                                                    "Drug_target_symbol", //9
                                                    "Cell_line_drug_with_outlier_response",//4
                                                    "Drug_is_cardiotoxic",
                                                    "Drug_type", //5
                                                    "Chrom", //0
                                                    "Start", //1
                                                    "End", //2
                                                    "Minor_allele", //11
                                                    "Minor_allel_frequency", //12
                                                    "ReadWrite_major_alleles", //13
                                                    "Cell_line", //14
                                                    "Cell_line_genotype_GT" //15
                                                  };
            this.Key_columnNames = new string[] {   "Variant RS ID", //3
                                                    "Variant gene", //6
                                                    "Location of variant", //10
                                                    "Relation of variant gene to drug target gene", //7
                                                    "Potentially affected drug target protein", //9
                                                    "Potentially affected drug", //4
                                                    Lincs_website_conversion_class.Label_is_cardiotoxic,
                                                    Lincs_website_conversion_class.Label_drug_class, //5
                                                    "Chromosome", //0
                                                    "Start position", //1
                                                    "End position", //2
                                                    "Least frequent allele in population", //11
                                                    "Population-wide frequency", //12
                                                    "Other allele(s)", //13
                                                    "Cardiomyocyte cell line with deviating response", //14
                                                    "Genotype of cardiomyocyte cell line" //15
                                                  };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_vcf_genomic_data_options_class
    {
        public int Aq_minimum { get; set; }
        public float Minor_allel_frequency_maximum { get; set; }
        public Lincs_vcf_genomic_data_options_class()
        {
            Aq_minimum = 20;
            Minor_allel_frequency_maximum = 0.1F; //0 - 1
        }
    }

    class Lincs_vcf_genomic_data_class
    {
        public Lincs_vcf_genomic_data_line_class[] Genomic_data { get; set; }
        public Lincs_vcf_genomic_data_options_class Options { get; set; }

        public Lincs_vcf_genomic_data_class()
        {
            this.Genomic_data = new Lincs_vcf_genomic_data_line_class[0];
            Options = new Lincs_vcf_genomic_data_options_class();
        }

        private void Add_to_array(Lincs_vcf_genomic_data_line_class[] add_genomic_data)
        {
            int add_length = add_genomic_data.Length;
            int this_length = this.Genomic_data.Length;
            int new_length = add_length + this_length;
            Lincs_vcf_genomic_data_line_class[] new_genomic_data = new Lincs_vcf_genomic_data_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_genomic_data[indexNew] = this.Genomic_data[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_genomic_data[indexNew] = add_genomic_data[indexAdd];
            }
            this.Genomic_data = new_genomic_data;
        }

        public void Check_for_correct_drugTargetProtein_drug_assignment(DE_class de_drugTarget)
        {
            DE_class de_drugTarget_currentDrug;
            this.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_drug_drugTargetSymbol(this.Genomic_data);
            int genomic_dat_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomics_line;
            string[] drugTargetGeneSymbols;
            Dictionary<string, bool> currentDrugTargetSymbols_dict = new Dictionary<string, bool>();
            for (int indexG = 0; indexG < genomic_dat_length; indexG++)
            {
                genomics_line = this.Genomic_data[indexG];
                if ((indexG == 0)
                    || (!genomics_line.Cell_line_drug_with_outlier_response.Equals(this.Genomic_data[indexG - 1].Cell_line_drug_with_outlier_response)))
                {
                    de_drugTarget_currentDrug = de_drugTarget.Deep_copy();
                    de_drugTarget_currentDrug.Keep_only_columns_with_selected_name_among_any_names(genomics_line.Cell_line_drug_with_outlier_response);
                    drugTargetGeneSymbols = de_drugTarget_currentDrug.Get_all_symbols_in_current_order();
                    currentDrugTargetSymbols_dict.Clear();
                    foreach (string drugTargetGeneSymbol in drugTargetGeneSymbols)
                    {
                        currentDrugTargetSymbols_dict.Add(drugTargetGeneSymbol, true);
                    }
                }
                switch (genomics_line.Relation_of_gene_symbol_to_drug)
                {
                    case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                    case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                    case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                    case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                    case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                    case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                    case Relation_of_gene_symbol_to_drug_enum.Transporter:
                    case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                    case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                        if (!currentDrugTargetSymbols_dict.ContainsKey(genomics_line.Drug_target_symbol)) { throw new Exception(); }
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        public void Check_if_equal_with_other(Lincs_vcf_genomic_data_class other)
        {
            int this_length = this.Genomic_data.Length;
            int other_length = other.Genomic_data.Length;
            //if (this_length!=other_length) { throw new Exception(); }
            this.Genomic_data = this.Genomic_data.OrderBy(l => l.Rs_identifier).ThenBy(l=>l.Cell_line_drug_with_outlier_response).ThenBy(l=>l.Relation_of_gene_symbol_to_drug).ThenBy(l=>l.Gene_symbol).ToArray();
            other.Genomic_data = other.Genomic_data.OrderBy(l => l.Rs_identifier).ThenBy(l => l.Cell_line_drug_with_outlier_response).ThenBy(l => l.Relation_of_gene_symbol_to_drug).ThenBy(l => l.Gene_symbol).ToArray();
            Lincs_vcf_genomic_data_line_class this_genomic_data_line;
            Lincs_vcf_genomic_data_line_class other_genomic_data_line;
            int indexOther = -1;
            for (int indexThis=0; indexThis<this_length;indexThis++)
            {
                this_genomic_data_line = this.Genomic_data[indexThis];
                if ((indexThis == 0)
                    || (!this_genomic_data_line.Rs_identifier.Equals(this.Genomic_data[indexThis - 1].Rs_identifier))
                    || (!this_genomic_data_line.Cell_line_drug_with_outlier_response.Equals(this.Genomic_data[indexThis - 1].Cell_line_drug_with_outlier_response))
                    || (!this_genomic_data_line.Relation_of_gene_symbol_to_drug.Equals(this.Genomic_data[indexThis - 1].Relation_of_gene_symbol_to_drug))
                    || (!this_genomic_data_line.Gene_symbol.Equals(this.Genomic_data[indexThis - 1].Gene_symbol)))
                {
                    indexOther++;
                    other_genomic_data_line = other.Genomic_data[indexOther];
                    if (!this_genomic_data_line.Rs_identifier.Equals(other_genomic_data_line.Rs_identifier)) { throw new Exception(); }
                    if (!this_genomic_data_line.Cell_line_drug_with_outlier_response.Equals(other_genomic_data_line.Cell_line_drug_with_outlier_response)) { throw new Exception(); }
                    if (!this_genomic_data_line.Relation_of_gene_symbol_to_drug.Equals(other_genomic_data_line.Relation_of_gene_symbol_to_drug)) { throw new Exception(); }
                    if (!this_genomic_data_line.Gene_symbol.Equals(other_genomic_data_line.Gene_symbol)) { throw new Exception(); }
                }
            }
            if (indexOther!=other_length-1) { throw new Exception(); }
        }

        public void Check_for_duplicates(string results_subdirectory)
        {
            Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>>>> relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>>>>();
            int genomic_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            Lincs_vcf_genomic_data_line_class duplicated_genomic_data_line;
            List<Lincs_vcf_genomic_data_line_class> duplicated_lines = new List<Lincs_vcf_genomic_data_line_class>();
            int duplicated_indexG;
            for (int indexG=0;indexG<genomic_length;indexG++)
            {
                genomic_data_line = this.Genomic_data[indexG];
                if (!relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict.ContainsKey(genomic_data_line.Relation_of_gene_symbol_to_drug))
                {
                    relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict.Add(genomic_data_line.Relation_of_gene_symbol_to_drug, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>>>());
                }
                if (!relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug].ContainsKey(genomic_data_line.Cell_line))
                {
                    relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug].Add(genomic_data_line.Cell_line, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>>());
                }
                if (!relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line].ContainsKey(genomic_data_line.Cell_line_drug_with_outlier_response))
                {
                    relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line].Add(genomic_data_line.Cell_line_drug_with_outlier_response, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, int>>>>());
                }
                if (!relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response].ContainsKey(genomic_data_line.Drug_target_symbol))
                {
                    relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response].Add(genomic_data_line.Drug_target_symbol, new Dictionary<string, Dictionary<string, Dictionary<string, int>>>());
                }
                if (!relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response][genomic_data_line.Drug_target_symbol].ContainsKey(genomic_data_line.Gene_symbol))
                {
                    relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response][genomic_data_line.Drug_target_symbol].Add(genomic_data_line.Gene_symbol, new Dictionary<string, Dictionary<string, int>>());
                }
                if (!relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response][genomic_data_line.Drug_target_symbol][genomic_data_line.Gene_symbol].ContainsKey(genomic_data_line.Rs_identifier))
                {
                    relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response][genomic_data_line.Drug_target_symbol][genomic_data_line.Gene_symbol].Add(genomic_data_line.Rs_identifier, new Dictionary<string, int>());
                }
                if (!relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response][genomic_data_line.Drug_target_symbol][genomic_data_line.Gene_symbol][genomic_data_line.Rs_identifier].ContainsKey(genomic_data_line.Cell_line_genotype_GT))
                {
                    relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response][genomic_data_line.Drug_target_symbol][genomic_data_line.Gene_symbol][genomic_data_line.Rs_identifier].Add(genomic_data_line.Cell_line_genotype_GT, indexG);
                }
                else
                {
                    duplicated_indexG = relationGeneSymbolToDrug_cellline_drug_drugTargetSymbol_geneSymbol_rsIdentifier_genotype_dict[genomic_data_line.Relation_of_gene_symbol_to_drug][genomic_data_line.Cell_line][genomic_data_line.Cell_line_drug_with_outlier_response][genomic_data_line.Drug_target_symbol][genomic_data_line.Gene_symbol][genomic_data_line.Rs_identifier][genomic_data_line.Cell_line_genotype_GT];
                    duplicated_genomic_data_line = this.Genomic_data[duplicated_indexG];
                    duplicated_lines.Add(duplicated_genomic_data_line);
                    duplicated_lines.Add(genomic_data_line);
                }
            }
            if (duplicated_lines.Count>0)
            {
                Lincs_vcf_genomic_data_readWriteOptions_class readWriteOptions = new Lincs_vcf_genomic_data_readWriteOptions_class(results_subdirectory, "Duplicated_lines.txt");
                ReadWriteClass.WriteData(duplicated_lines.ToArray(), readWriteOptions);
                throw new Exception();
            }
        }

        public void Generate_after_reading_and_write_binary(string complete_fileName, string complete_binary_fileName, string results_subdirectory)
        {
            Lincs_vcf_genomic_data_chromosomeOnly_readWriteOptions_class chromsomeOnly_readWriteOptions = new Lincs_vcf_genomic_data_chromosomeOnly_readWriteOptions_class("", complete_fileName);
            Lincs_vcf_genomic_data_chromosomeOnly_line_class[] chromosomeOnly_lines = ReadWriteClass.ReadRawData_and_FillArray<Lincs_vcf_genomic_data_chromosomeOnly_line_class>(chromsomeOnly_readWriteOptions);
            Dictionary<string, bool> chromosome_dict = new Dictionary<string, bool>();
            foreach (Lincs_vcf_genomic_data_chromosomeOnly_line_class chromosomeOnly_line in chromosomeOnly_lines)
            {
                if (!chromosome_dict.ContainsKey(chromosomeOnly_line.Chr))
                {
                    chromosome_dict.Add(chromosomeOnly_line.Chr, true);
                }
            }
            string[] chromosomes = chromosome_dict.Keys.ToArray();
            string chromosome;
            int chromosomes_length = chromosomes.Length;
            if (System.IO.File.Exists(complete_binary_fileName)) { System.IO.File.Delete(complete_binary_fileName); }
            for (int indexC=0; indexC<chromosomes_length; indexC++)
            {
                chromosome = chromosomes[indexC];
                Read_all_lines_of_given_chromosome(complete_fileName, chromosome);
                Check_for_duplicates(results_subdirectory);
                Write_binary(complete_binary_fileName, System.IO.FileMode.Append);
                this.Genomic_data = null;
            }
        }

        public DE_class Generate_de_instance()
        {
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> new_fill_de_lines = new List<Fill_de_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class vcf_genomic_line in this.Genomic_data)
            {
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Symbols_for_de = new string[] { (string)vcf_genomic_line.Gene_symbol.Clone() };
                fill_de_line.Value_for_de = 1;
                fill_de_line.Names_for_de = new string[] { (string)vcf_genomic_line.Cell_line.Clone() };
                new_fill_de_lines.Add(fill_de_line);
            }
            DE_class de = new DE_class();
            de.Fill_with_data(new_fill_de_lines.ToArray());
            return de;
        }

        public void Filter_data_by_quality_control_cutoffs()
        {
            int genomic_data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexGD = 0; indexGD < genomic_data_length; indexGD++)
            {
                genomic_data_line = this.Genomic_data[indexGD];
                if (genomic_data_line.Quality_aq >= Options.Aq_minimum)
                {
                    keep.Add(genomic_data_line);
                }
            }
            if ((Options.Aq_minimum!=0)&&(keep.Count==genomic_data_length)) { throw new Exception(); }
            this.Genomic_data = keep.ToArray();
        }

        public void Filter_data_by_biologicalRelevance_cutoffs()
        {
            int genomic_data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            float max_minor_allele_frequency = -1;
            bool lines_removed = false;
            for (int indexGD = 0; indexGD < genomic_data_length; indexGD++)
            {
                genomic_data_line = this.Genomic_data[indexGD];
                if (max_minor_allele_frequency > genomic_data_line.Minor_allel_frequency) { max_minor_allele_frequency = genomic_data_line.Minor_allel_frequency; }
                if (       (   (!genomic_data_line.Gtex_atrial_eQTL.Equals("."))
                            || (!genomic_data_line.Gtex_atrial_sQTL.Equals("."))
                            || (!genomic_data_line.Gtex_ventricular_eQTL.Equals("."))
                            || (!genomic_data_line.Gtex_ventricular_sQTL.Equals("."))
                            || (genomic_data_line.Variant_location.Equals(Variant_location_refGene_enum.Exonic)))
                        && ( genomic_data_line.Minor_allel_frequency <= Options.Minor_allel_frequency_maximum))
                {
                    keep.Add(genomic_data_line);
                }
                else
                {
                    lines_removed = true;
                }
            }
            if (!lines_removed) { throw new Exception(); }
            this.Genomic_data = keep.ToArray();
        }

        public void Filter_data_by_keeping_only_variants_that_are_overrepresented_in_only_one_cell_line()
        {
            this.Identify_variants_that_are_overrepresented_in_only_one_cell_line();
            int genomic_data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexGD = 0; indexGD < genomic_data_length; indexGD++)
            {
                genomic_data_line = this.Genomic_data[indexGD];
                if (genomic_data_line.Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments)
                {
                    keep.Add(genomic_data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Keep_only_gtex_qtls_or_exonic_variants()
        {
            int genomic_data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexGD = 0; indexGD < genomic_data_length; indexGD++)
            {
                genomic_data_line = this.Genomic_data[indexGD];
                if (((!String.IsNullOrEmpty(genomic_data_line.Gtex_atrial_eQTL))
                        && (!genomic_data_line.Gtex_atrial_eQTL.Equals(".")))
                    || ((!String.IsNullOrEmpty(genomic_data_line.Gtex_atrial_sQTL))
                        && (!genomic_data_line.Gtex_atrial_sQTL.Equals(".")))
                    || ((!String.IsNullOrEmpty(genomic_data_line.Gtex_ventricular_eQTL))
                        && (!genomic_data_line.Gtex_ventricular_eQTL.Equals(".")))
                    || ((!String.IsNullOrEmpty(genomic_data_line.Gtex_ventricular_sQTL))
                        && (!genomic_data_line.Gtex_ventricular_sQTL.Equals(".")))
                    || (genomic_data_line.Variant_location.Equals(Variant_location_refGene_enum.Exonic))
                    || (genomic_data_line.Variant_location.Equals(Variant_location_refGene_enum.Ncrna_exonic))
                    || (genomic_data_line.Variant_location.Equals(Variant_location_refGene_enum.Splicing))
                    || (genomic_data_line.Variant_location.Equals(Variant_location_refGene_enum.Ncrna_splicing)))
                {
                    keep.Add(genomic_data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Write_binary(string completeFileName, FileMode fileMode)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(completeFileName, fileMode));
            int genomic_data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            for (int indexGD = 0; indexGD < genomic_data_length; indexGD++)
            {
                genomic_data_line = this.Genomic_data[indexGD];
                writer.Write(genomic_data_line.Rs_identifier);
                writer.Write(genomic_data_line.Cell_line);
                writer.Write(genomic_data_line.Cell_line_allele_depth_AD);
                writer.Write(genomic_data_line.Cell_line_approximate_read_depth_DP);
                writer.Write(genomic_data_line.Cell_line_conditional_genotype_quality);
                writer.Write(genomic_data_line.Cell_line_genotype_GT);
                writer.Write(genomic_data_line.Cell_line_minor_allele_count);
                writer.Write(genomic_data_line.Cell_line_phred_scale_genotype_quality_GQ);
                writer.Write(genomic_data_line.Cell_line_drug_with_outlier_response);
                writer.Write(genomic_data_line.ReadWrite_F1_score_weigths_with_outlier_responses);
                writer.Write(genomic_data_line.Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments);
                writer.Write(genomic_data_line.Cell_line_frequency_of_cell_line_minor_allele);
                writer.Write(genomic_data_line.Cell_line_outlier_adj_pvalue);
                writer.Write(genomic_data_line.Cell_line_outlier_pvalue);
                writer.Write(genomic_data_line.Cell_line_outlier_mean_f1_score_without_outlier);
                writer.Write(genomic_data_line.Cell_line_outlier_F1_score_weight);
                writer.Write(genomic_data_line.Cell_line_drug_sample_count);
                writer.Write(genomic_data_line.Cadd_phred);
                writer.Write(genomic_data_line.SpAI_DS_snv_raw);
                writer.Write(genomic_data_line.SpAI_DS_indel_raw);
                writer.Write(genomic_data_line.Clnsig);
                writer.Write(genomic_data_line.Publication);
                writer.Write(genomic_data_line.Quality_aq);
                writer.Write(genomic_data_line.Gtex_atrial_eQTL);
                writer.Write(genomic_data_line.Gtex_atrial_sQTL);
                writer.Write(genomic_data_line.Gtex_ventricular_eQTL);
                writer.Write(genomic_data_line.Gtex_ventricular_sQTL);
                writer.Write(genomic_data_line.Successfully_sequenced_celllines_count);
                writer.Write(genomic_data_line.Chrom);
                writer.Write(genomic_data_line.Relation_of_gene_symbol_to_drug.ToString());
                writer.Write(genomic_data_line.Ontology.ToString());
                writer.Write(genomic_data_line.Rank_of_gene_symbol_within_relation_for_drug);
                writer.Write(genomic_data_line.End);
                writer.Write(genomic_data_line.Gene_symbol);
                writer.Write(genomic_data_line.Drug_target_symbol);
                writer.Write(genomic_data_line.Minor_allele);
                writer.Write(genomic_data_line.Minor_allel_frequency);
                writer.Write(genomic_data_line.ReadWrite_major_alleles);
                writer.Write(genomic_data_line.ReadWrite_other_cell_lines_minor_allele_counts);
                writer.Write(genomic_data_line.Start);
                writer.Write(genomic_data_line.Variant_location.ToString());
            }
            writer.Close();
        }

        public void Write_as_supplemental_table_and_for_website(string subdirectory, string fileName)
        {
            Lincs_vcf_genomic_data_supplTable_readWriteOptions_class readWriteOptions = new Lincs_vcf_genomic_data_supplTable_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Genomic_data, readWriteOptions);
        }
        public void Write(string subdirectory, string fileName)
        {
            Lincs_vcf_genomic_data_readWriteOptions_class readWriteOptions = new Lincs_vcf_genomic_data_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Genomic_data, readWriteOptions);
        }

        public void Read(string subdirectory, string fileName)
        {
            Lincs_vcf_genomic_data_readWriteOptions_class readWriteOptions = new Lincs_vcf_genomic_data_readWriteOptions_class(subdirectory, fileName);
            this.Genomic_data = ReadWriteClass.ReadRawData_and_FillArray<Lincs_vcf_genomic_data_line_class>(readWriteOptions);
        }

        private void Split_multiple_geneSymbols()
        {
            List<Lincs_vcf_genomic_data_line_class> new_data = new List<Lincs_vcf_genomic_data_line_class>();
            int data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            Lincs_vcf_genomic_data_line_class new_data_line;
            string[] geneSymbols;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                data_line = this.Genomic_data[indexD];
                geneSymbols = data_line.Gene_symbol.Split(';');
                foreach (string geneSymbol in geneSymbols)
                {
                    new_data_line = data_line.Deep_copy();
                    new_data_line.Gene_symbol = (string)geneSymbol.Clone();
                    new_data.Add(new_data_line);
                }
            }
            this.Genomic_data = new_data.ToArray();
        }

        public void Read_binary_and_process(string fileName)
        {
            Read_binary_either_allLines_or_lines_with_specified_chromosomes(fileName);
            Remove_uncompletely_sequenced_cellline_alleles();
            Identify_variants_that_are_overrepresented_in_only_one_cell_line();
            Keep_only_lines_with_variants_overrepresented_in_on_cell_line();
        }

        public string[] Get_all_drugs()
        {
            Dictionary<string, bool> drug_dict = new Dictionary<string, bool>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                if (!drug_dict.ContainsKey(genomic_line.Cell_line_drug_with_outlier_response))
                {
                    drug_dict.Add(genomic_line.Cell_line_drug_with_outlier_response, true);
                }
            }
            return drug_dict.Keys.OrderBy(l => l).ToArray();
        }

        public Dictionary<string, string> Get_drug_outlierCellline_dict()
        {
            Dictionary<string, string> drug_cellline_dict = new Dictionary<string, string>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                if (!drug_cellline_dict.ContainsKey(genomic_line.Cell_line_drug_with_outlier_response))
                {
                    drug_cellline_dict.Add(genomic_line.Cell_line_drug_with_outlier_response, genomic_line.Cell_line);
                }
                else if (!drug_cellline_dict[genomic_line.Cell_line_drug_with_outlier_response].Equals(genomic_line.Cell_line)) { throw new Exception(); }
            }
            return drug_cellline_dict;
        }

        public Dictionary<string, int> Generate_drugGroupOrSingleDrug_geneSymbolCount_dictionary(params string[][] drugs_with_groupedCounts_array)
        {
            int drugs_with_groupedCounts_array_length = drugs_with_groupedCounts_array.Length;
            string[] currentGroup_drugs;
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> drug_drugGroup_dict = new Dictionary<string, string>();
            Dictionary<string, string[]> drugGroup_drugs_dict = new Dictionary<string, string[]>();
            for (int indexGroup = 0; indexGroup < drugs_with_groupedCounts_array_length; indexGroup++)
            {
                currentGroup_drugs = drugs_with_groupedCounts_array[indexGroup].ToArray();
                sb.Clear();
                foreach (string currentGroup_drug in currentGroup_drugs)
                {
                    if (sb.Length > 0) { sb.AppendFormat(";"); }
                    sb.AppendFormat("{0}", currentGroup_drug);
                }
                foreach (string currentGroup_drug in currentGroup_drugs)
                {
                    drug_drugGroup_dict.Add(currentGroup_drug, sb.ToString());
                }
                drugGroup_drugs_dict.Add(sb.ToString(), currentGroup_drugs);
            }
            string currentGroup;
            Dictionary<string, Dictionary<string, bool>> drugGroup_genes_dict = new Dictionary<string, Dictionary<string, bool>>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                if (!drug_drugGroup_dict.ContainsKey(genomic_line.Cell_line_drug_with_outlier_response))
                { drug_drugGroup_dict.Add(genomic_line.Cell_line_drug_with_outlier_response, genomic_line.Cell_line_drug_with_outlier_response); }
                currentGroup = drug_drugGroup_dict[genomic_line.Cell_line_drug_with_outlier_response];
                if (!drugGroup_genes_dict.ContainsKey(currentGroup))
                {
                    drugGroup_genes_dict.Add(currentGroup, new Dictionary<string, bool>());
                }
                if (!drugGroup_genes_dict[currentGroup].ContainsKey(genomic_line.Gene_symbol))
                {
                    drugGroup_genes_dict[currentGroup].Add(genomic_line.Gene_symbol, true);
                }
            }
            Dictionary<string, int> drugGroupOrSingleDrug_genesCount_dict = new Dictionary<string, int>();
            string[] drugGroups = drugGroup_genes_dict.Keys.ToArray();
            foreach (string drugGroup in drugGroups)
            {
                drugGroupOrSingleDrug_genesCount_dict.Add(drugGroup, drugGroup_genes_dict[drugGroup].Keys.ToArray().Length);
            }
            return drugGroupOrSingleDrug_genesCount_dict;
        }

        public Dictionary<string, int> Generate_drugGroupOrSingleDrug_rsIdentifierCount_dictionary(params string[][] drugs_with_groupedCounts_array)
        {
            int drugs_with_groupedCounts_array_length = drugs_with_groupedCounts_array.Length;
            string[] currentGroup_drugs;
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> drug_drugGroup_dict = new Dictionary<string, string>();
            Dictionary<string, string[]> drugGroup_drugs_dict = new Dictionary<string, string[]>();
            for (int indexGroup = 0; indexGroup < drugs_with_groupedCounts_array_length; indexGroup++)
            {
                currentGroup_drugs = drugs_with_groupedCounts_array[indexGroup].ToArray();
                sb.Clear();
                foreach (string currentGroup_drug in currentGroup_drugs)
                {
                    if (sb.Length > 0) { sb.AppendFormat(";"); }
                    sb.AppendFormat("{0}", currentGroup_drug);
                }
                foreach (string currentGroup_drug in currentGroup_drugs)
                {
                    drug_drugGroup_dict.Add(currentGroup_drug, sb.ToString());
                }
                drugGroup_drugs_dict.Add(sb.ToString(), currentGroup_drugs);
            }
            string currentGroup;
            Dictionary<string, Dictionary<string, bool>> drugGroup_genes_dict = new Dictionary<string, Dictionary<string, bool>>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                if (!drug_drugGroup_dict.ContainsKey(genomic_line.Cell_line_drug_with_outlier_response))
                { drug_drugGroup_dict.Add(genomic_line.Cell_line_drug_with_outlier_response, genomic_line.Cell_line_drug_with_outlier_response); }
                currentGroup = drug_drugGroup_dict[genomic_line.Cell_line_drug_with_outlier_response];
                if (!drugGroup_genes_dict.ContainsKey(currentGroup))
                {
                    drugGroup_genes_dict.Add(currentGroup, new Dictionary<string, bool>());
                }
                if (!drugGroup_genes_dict[currentGroup].ContainsKey(genomic_line.Rs_identifier))
                {
                    drugGroup_genes_dict[currentGroup].Add(genomic_line.Rs_identifier, true);
                }
            }
            Dictionary<string, int> drugGroupOrSingleDrug_rsIdentifierCount_dict = new Dictionary<string, int>();
            string[] drugGroups = drugGroup_genes_dict.Keys.ToArray();
            foreach (string drugGroup in drugGroups)
            {
                drugGroupOrSingleDrug_rsIdentifierCount_dict.Add(drugGroup, drugGroup_genes_dict[drugGroup].Keys.ToArray().Length);
            }
            return drugGroupOrSingleDrug_rsIdentifierCount_dict;
        }

        public string[] Get_all_geneSymbols()
        {
            Dictionary<string, bool> geneSymbol_dict = new Dictionary<string, bool>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                if (!geneSymbol_dict.ContainsKey(genomic_line.Gene_symbol))
                {
                    geneSymbol_dict.Add(genomic_line.Gene_symbol, true);
                }
            }
            return geneSymbol_dict.Keys.OrderBy(l => l).ToArray();
        }

        public string[] Get_all_rsIdentifiers()
        {
            Dictionary<string, bool> rsIdentifier_dict = new Dictionary<string, bool>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                if (!rsIdentifier_dict.ContainsKey(genomic_line.Rs_identifier))
                {
                    rsIdentifier_dict.Add(genomic_line.Rs_identifier, true);
                }
            }
            return rsIdentifier_dict.Keys.OrderBy(l => l).ToArray();
        }

        public void Keep_only_lines_with_indicated_drugs_ignoring_case(params string[] drugs)
        {
            drugs = drugs.Distinct().ToArray();
            Dictionary<string, bool> keep_drug_dict = new Dictionary<string, bool>();
            foreach (string drug in drugs)
            {
                keep_drug_dict.Add(drug.ToLower(), true);
            }
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class data_line in this.Genomic_data)
            {
                if (keep_drug_dict.ContainsKey(data_line.Cell_line_drug_with_outlier_response.ToLower()))
                {
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Keep_only_lines_with_indicated_drug_types(params Drug_type_enum[] keep_drugTypes)
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            string[] drugs = drug_legend.Get_all_drugs_of_indicated_types(keep_drugTypes);
            Keep_only_lines_with_indicated_drugs_ignoring_case(drugs);
        }

        public void Keep_only_lines_with_TKIs_and_indicated_cardiotoxicity(bool is_cardiotoxic_TKIs)
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            string[] drugs = new string[0];
            if (is_cardiotoxic_TKIs)
            {
                drugs = drug_legend.Get_all_cardiotoxic_tkis();
            }
            else
            {
                drugs = drug_legend.Get_all_cardiotoxic_tkis();
            }
            Keep_only_lines_with_indicated_drugs_ignoring_case(drugs);
        }

        public void Replace_drugAbbreviation_by_fullDrugName_and_add_drugType()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            drug_legend.Add_missing_cardiotoxicity_from_faers();
            Dictionary<string, Drug_type_enum> drug_drugType_dict = drug_legend.Get_drug_drugType_dictionary();
            Dictionary<string, string> drug_fullDrugName_dict = drug_legend.Get_drug_drugFullName_dict();
            Dictionary<string, string> drug_isCardiotoxic_dict = drug_legend.Get_drug_isCardiotoxic_dictionary();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                genomic_line.Drug_type = drug_drugType_dict[genomic_line.Cell_line_drug_with_outlier_response];
                genomic_line.Drug_is_cardiotoxic = drug_isCardiotoxic_dict[genomic_line.Cell_line_drug_with_outlier_response];
                genomic_line.Cell_line_drug_with_outlier_response = (string)drug_fullDrugName_dict[genomic_line.Cell_line_drug_with_outlier_response].Clone();
            }
        }

        public void Add_drug_drugTarget_ontology_and_relation_to_drug_for_selected_geneSymbol_after_checking_if_all_empty(string geneSymbol, string drugTarget_symbol, Ontology_type_enum ontology, Relation_of_gene_symbol_to_drug_enum relation, string drug)
        {
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                if (genomic_line.Gene_symbol.Equals(geneSymbol))
                {
                    if (  (!genomic_line.Relation_of_gene_symbol_to_drug.Equals(Relation_of_gene_symbol_to_drug_enum.E_m_p_t_y))
                        &&(!genomic_line.Relation_of_gene_symbol_to_drug.Equals(Relation_of_gene_symbol_to_drug_enum.Drugs_not_considered_yet))) { throw new Exception(); }
                    if (!String.IsNullOrEmpty(genomic_line.Cell_line_drug_with_outlier_response)) { throw new Exception(); }
                    if (!String.IsNullOrEmpty(genomic_line.Drug_target_symbol)) { throw new Exception(); }
                    genomic_line.Ontology = ontology;
                    genomic_line.Cell_line_drug_with_outlier_response = (string)drug.Clone();
                    genomic_line.Relation_of_gene_symbol_to_drug = relation;
                    genomic_line.Drug_target_symbol = (string)drugTarget_symbol.Clone();
                }
            }
        }

        public void Add_ranks_in_first_de_column_to_all_genes_ignoring_any_other_specification(DE_class de_ranks)
        {
            Dictionary<string, double> geneSymbol_rank_dict = de_ranks.Generate_geneSymbol_valueOf1stColumn_dict();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Genomic_data)
            {
                genomic_line.Rank_of_gene_symbol_within_relation_for_drug = (float)geneSymbol_rank_dict[genomic_line.Gene_symbol];
            }
        }
        public void Keep_only_lines_with_indicated_tfs_or_kinases_as_symbols_and_add_drug_scp_ontology_and_relationToGeneSymbol(string drug, string tfOrKinase_targetGene, Ontology_type_enum ontology, Relation_of_gene_symbol_to_drug_enum relation_of_gene_symbol_to_drug, string[] tfs_or_kinases)
        {
            switch (relation_of_gene_symbol_to_drug)
            {
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                    break;
                default:
                    throw new Exception();
            }
            tfs_or_kinases = tfs_or_kinases.Distinct().ToArray();
            Dictionary<string, bool> tfsOrKinases_dict = new Dictionary<string, bool>();
            foreach (string tf_or_kinase in tfs_or_kinases)
            {
                tfsOrKinases_dict.Add(tf_or_kinase, true);
            }
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class data_line in this.Genomic_data)
            {
                if (tfsOrKinases_dict.ContainsKey(data_line.Gene_symbol))
                {
                    data_line.Ontology = ontology;
                    data_line.Cell_line_drug_with_outlier_response = (string)drug.Clone();
                    data_line.Drug_target_symbol = (string)tfOrKinase_targetGene.Clone();
                    data_line.Relation_of_gene_symbol_to_drug = relation_of_gene_symbol_to_drug;
                    data_line.Rank_of_gene_symbol_within_relation_for_drug = -1;
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Keep_only_lines_with_indicated_drugTargetProteins_as_symbols_and_add_drug_ontology_and_relationToGeneSymbol(string drug, Ontology_type_enum ontology, Relation_of_gene_symbol_to_drug_enum relation_of_geneSybmol_to_drug, params string[] drugTarget_proteins)
        {
            switch (relation_of_geneSybmol_to_drug)
            {
                case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                case Relation_of_gene_symbol_to_drug_enum.Transporter:
                    break;
                default:
                    throw new Exception();
            }
            drugTarget_proteins = drugTarget_proteins.Distinct().ToArray();
            Dictionary<string, bool> keep_symbol_dict = new Dictionary<string, bool>();
            foreach (string symbol in drugTarget_proteins)
            {
                keep_symbol_dict.Add(symbol, true);
            }
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class data_line in this.Genomic_data)
            {
                if (keep_symbol_dict.ContainsKey(data_line.Gene_symbol))
                {
                    data_line.Ontology = ontology;
                    data_line.Cell_line_drug_with_outlier_response = (string)drug.Clone();
                    data_line.Drug_target_symbol = (string)data_line.Gene_symbol.Clone();
                    data_line.Relation_of_gene_symbol_to_drug = relation_of_geneSybmol_to_drug;
                    data_line.Rank_of_gene_symbol_within_relation_for_drug = -1;
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Keep_only_lines_with_indicated_symbols(params string[] symbols)
        {
            symbols = symbols.Distinct().ToArray();
            Dictionary<string, bool> keep_symbol_dict = new Dictionary<string, bool>();
            foreach (string symbol in symbols)
            {
                keep_symbol_dict.Add(symbol, true);
            }
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class data_line in this.Genomic_data)
            {
                if (keep_symbol_dict.ContainsKey(data_line.Gene_symbol))
                {
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Keep_only_lines_with_indicated_rsIdentifiers(params string[] rsIdentifiers)
        {
            rsIdentifiers = rsIdentifiers.Distinct().ToArray();
            Dictionary<string, bool> rsIdentifier_dict = new Dictionary<string, bool>();
            foreach (string rsIdentifier in rsIdentifiers)
            {
                rsIdentifier_dict.Add(rsIdentifier, true);
            }
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class data_line in this.Genomic_data)
            {
                if (rsIdentifier_dict.ContainsKey(data_line.Rs_identifier))
                {
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Keep_only_lines_with_indicated_cell_lines(params string[] cell_lines)
        {
            cell_lines = cell_lines.Distinct().ToArray();
            Dictionary<string, bool> keep_cell_line_dict = new Dictionary<string, bool>();
            foreach (string cell_line in cell_lines)
            {
                keep_cell_line_dict.Add(cell_line, true);
            }
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class data_line in this.Genomic_data)
            {
                if (keep_cell_line_dict.ContainsKey(data_line.Cell_line))
                {
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Keep_only_lines_with_indicated_relationsGeneSymbolsToDrugTargets(params Relation_of_gene_symbol_to_drug_enum[] relations)
        {
            relations = relations.Distinct().ToArray();
            Dictionary<Relation_of_gene_symbol_to_drug_enum, bool> keep_relation_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, bool>();
            foreach (Relation_of_gene_symbol_to_drug_enum relation in relations)
            {
                keep_relation_dict.Add(relation, true);
            }
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class data_line in this.Genomic_data)
            {
                if (keep_relation_dict.ContainsKey(data_line.Relation_of_gene_symbol_to_drug))
                {
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Read_binary_either_allLines_or_lines_with_specified_chromosomes(string complete_fileName, params string[] read_only_lines_with_selected_chromosomes)
        {
            this.Genomic_data = new Lincs_vcf_genomic_data_line_class[0];
            Read_binary_and_add_to_array(complete_fileName, read_only_lines_with_selected_chromosomes);
        }

        public NetworkBasis_class Generate_network_considering_all_lines()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, string> drug_drugFullName_dict = drug_legend.Get_drug_drugFullName_dict();

            this.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_geneSymbol_rsIdentifier(this.Genomic_data);
            Dictionary<Relation_of_gene_symbol_to_drug_enum, System.Drawing.Color> relation_color_dict = Lincs_genomics_drugBank_names_class.Get_relation_color_dict();

            Lincs_vcf_genomic_data_line_class genomic_data_line;
            int genomics_length = Genomic_data.Length;
            SigNWBasis_line_class new_sigNW_line;
            List<SigNWBasis_line_class> sigNW_list = new List<SigNWBasis_line_class>();
            Dictionary<Relation_of_gene_symbol_to_drug_enum, List<string>> relationOfGeneSymboltoDrug_nodes_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, List<string>>();
            Dictionary<string, Dictionary<string, bool>> drug_drugTargetSymbols_dict = new Dictionary<string, Dictionary<string, bool>>();
            string drugFullName;
            string geneSymbol_node_name = "";
            string drugTargetSymbol_for_nw;
            string[] drugTargetSymbols;
            Dictionary<string, List<Variant_cardiotoxic_effect_enum>> current_rsIdentifier_effect_dict = new Dictionary<string, List<Variant_cardiotoxic_effect_enum>>();
            StringBuilder geneSymbol_node_name_sb = new StringBuilder();
            for (int indexG = 0; indexG < genomics_length; indexG++)
            {
                genomic_data_line = Genomic_data[indexG];
                if ((indexG == 0)
                    || (!genomic_data_line.Gene_symbol.Equals(Genomic_data[indexG - 1].Gene_symbol)))
                {
                    current_rsIdentifier_effect_dict.Clear();
                }
                if (!current_rsIdentifier_effect_dict.ContainsKey(genomic_data_line.Rs_identifier))
                {
                    current_rsIdentifier_effect_dict.Add(genomic_data_line.Rs_identifier, new List<Variant_cardiotoxic_effect_enum>());
                }
                //else { throw new Exception(); }
                if ((indexG == genomics_length - 1)
                    || (!genomic_data_line.Gene_symbol.Equals(Genomic_data[indexG + 1].Gene_symbol)))
                {
                    if (String.IsNullOrEmpty(genomic_data_line.Drug_target_symbol)) { throw new Exception(); }
                    if (String.IsNullOrEmpty(genomic_data_line.Gene_symbol)) { throw new Exception(); }

                    drugTargetSymbols = genomic_data_line.Drug_target_symbol.Split(';');
                    geneSymbol_node_name = (string)genomic_data_line.Gene_symbol.Clone() + " (" + current_rsIdentifier_effect_dict.Keys.Count + ")";
                    foreach (string drugTargetSymbol in drugTargetSymbols)
                    {
                        if (String.IsNullOrEmpty(drugTargetSymbol)) { throw new Exception(); }
                        if (String.IsNullOrEmpty(geneSymbol_node_name)) { throw new Exception(); }
                        new_sigNW_line = new SigNWBasis_line_class();
                        if (drugTargetSymbol.Equals(genomic_data_line.Gene_symbol)) { drugTargetSymbol_for_nw = drugTargetSymbol + " (" + current_rsIdentifier_effect_dict.Keys.Count + ")"; }
                        else { drugTargetSymbol_for_nw = drugTargetSymbol; }
                        new_sigNW_line.Target = (string)drugTargetSymbol_for_nw.Clone();
                        new_sigNW_line.Source = (string)geneSymbol_node_name.Clone();

                        if (!relationOfGeneSymboltoDrug_nodes_dict.ContainsKey(genomic_data_line.Relation_of_gene_symbol_to_drug))
                        {
                            relationOfGeneSymboltoDrug_nodes_dict.Add(genomic_data_line.Relation_of_gene_symbol_to_drug, new List<string>());
                        }
                        relationOfGeneSymboltoDrug_nodes_dict[genomic_data_line.Relation_of_gene_symbol_to_drug].Add(geneSymbol_node_name);
                        sigNW_list.Add(new_sigNW_line);
                        drugFullName = drug_drugFullName_dict[genomic_data_line.Cell_line_drug_with_outlier_response];
                        if (!drug_drugTargetSymbols_dict.ContainsKey(drugFullName))
                        {
                            drug_drugTargetSymbols_dict.Add(drugFullName, new Dictionary<string, bool>());
                        }
                        if (!drug_drugTargetSymbols_dict[drugFullName].ContainsKey(drugTargetSymbol_for_nw))
                        {
                            drug_drugTargetSymbols_dict[drugFullName].Add(drugTargetSymbol_for_nw, true);
                            new_sigNW_line = new SigNWBasis_line_class();
                            switch (genomic_data_line.Relation_of_gene_symbol_to_drug)
                            {
                                case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                                case Relation_of_gene_symbol_to_drug_enum.Transporter:
                                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                                    new_sigNW_line.Source = (string)drugTargetSymbol_for_nw.Clone();
                                    new_sigNW_line.Target = (string)drugFullName.Clone();
                                    break;
                                case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                                case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                                case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                                    new_sigNW_line.Source = (string)drugFullName.Clone();
                                    new_sigNW_line.Target = (string)drugTargetSymbol_for_nw.Clone();
                                    break;
                                default:
                                    throw new Exception();
                            }
                            if (String.IsNullOrEmpty(new_sigNW_line.Source)) { throw new Exception(); }
                            if (String.IsNullOrEmpty(new_sigNW_line.Target)) { throw new Exception(); }
                            sigNW_list.Add(new_sigNW_line);
                        }
                    }
                }
            }

            NetworkBasis_class nw = new NetworkBasis_class();
            if (sigNW_list.Count > 0)
            {
                SigNWBasis_class<SigNWBasis_line_class> sigNW = new SigNWBasis_class<SigNWBasis_line_class>();
                sigNW.Generate_sigNW_from_sigNW_list(sigNW_list, Network_direction_type_enum.Directed_forward);
                nw.Generate_network_from_sigNW(sigNW);
                nw.UniqueNodes.Clear_node_classification();

                Relation_of_gene_symbol_to_drug_enum[] relationOfGeneSymbolsToDrugs = relationOfGeneSymboltoDrug_nodes_dict.Keys.ToArray();
                Relation_of_gene_symbol_to_drug_enum relationOfGeneSymbolsToDrug;
                int relation_length = relationOfGeneSymbolsToDrugs.Length;

                string legend_relation;
                string[] current_nodes;
                System.Drawing.Color selected_color;
                for (int indexR = 0; indexR < relation_length; indexR++)
                {
                    relationOfGeneSymbolsToDrug = relationOfGeneSymbolsToDrugs[indexR];
                    current_nodes = relationOfGeneSymboltoDrug_nodes_dict[relationOfGeneSymbolsToDrug].ToArray();
                    selected_color = relation_color_dict[relationOfGeneSymbolsToDrug];
                    nw.UniqueNodes.Add_selected_color_to_nodes(current_nodes, selected_color);

                    legend_relation = relationOfGeneSymbolsToDrug.ToString().Replace("_", " ");
                    nw.Add_nodes(legend_relation);
                    nw.UniqueNodes.Add_selected_color_to_nodes(new string[] { legend_relation }, relation_color_dict[relationOfGeneSymbolsToDrug]);
                }
                nw.UniqueNodes.Add_color_to_all_nodes_with_no_colors(System.Drawing.Color.White);
                nw.Remove_self_interacting_edges();
            }
            return nw;
        }

        public void Read_binary_and_add_to_array(string completeFileName, params string[] read_only_lines_with_given_chromosomes)
        {
            Dictionary<string, bool> readOnlyChromosome_dict = new Dictionary<string, bool>();
            read_only_lines_with_given_chromosomes = read_only_lines_with_given_chromosomes.Distinct().ToArray();
            foreach (string chromosome in read_only_lines_with_given_chromosomes)
            {
                readOnlyChromosome_dict.Add(chromosome, true);
            }
            bool filter_by_chromosome = readOnlyChromosome_dict.Keys.Count > 0;

            BinaryReader reader = new BinaryReader(File.OpenRead(completeFileName));
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            List<Lincs_vcf_genomic_data_line_class> genomic_lines_list = new List<Lincs_vcf_genomic_data_line_class>();
            long last_index = reader.BaseStream.Length;
            while (reader.BaseStream.Position != last_index)
            {
                genomic_data_line = new Lincs_vcf_genomic_data_line_class();
                genomic_data_line.Rs_identifier = reader.ReadString();
                genomic_data_line.Cell_line = reader.ReadString();
                genomic_data_line.Cell_line_allele_depth_AD = reader.ReadInt32();
                genomic_data_line.Cell_line_approximate_read_depth_DP = reader.ReadInt32();
                genomic_data_line.Cell_line_conditional_genotype_quality = reader.ReadSingle();
                genomic_data_line.Cell_line_genotype_GT = reader.ReadString();
                genomic_data_line.Cell_line_minor_allele_count = reader.ReadInt32();
                genomic_data_line.Cell_line_phred_scale_genotype_quality_GQ = reader.ReadInt32();
                genomic_data_line.Cell_line_drug_with_outlier_response = reader.ReadString();
                genomic_data_line.ReadWrite_F1_score_weigths_with_outlier_responses = reader.ReadString();
                genomic_data_line.Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments = reader.ReadBoolean();
                genomic_data_line.Cell_line_frequency_of_cell_line_minor_allele = reader.ReadSingle();
                genomic_data_line.Cell_line_outlier_adj_pvalue = reader.ReadSingle();
                genomic_data_line.Cell_line_outlier_pvalue = reader.ReadSingle();
                genomic_data_line.Cell_line_outlier_mean_f1_score_without_outlier = reader.ReadSingle();
                genomic_data_line.Cell_line_outlier_F1_score_weight = reader.ReadSingle();
                genomic_data_line.Cell_line_drug_sample_count = reader.ReadInt32();
                genomic_data_line.Cadd_phred = reader.ReadSingle();
                genomic_data_line.SpAI_DS_snv_raw = reader.ReadSingle();
                genomic_data_line.SpAI_DS_indel_raw = reader.ReadSingle();
                genomic_data_line.Clnsig = reader.ReadString();
                genomic_data_line.Publication = reader.ReadString();
                genomic_data_line.Quality_aq = reader.ReadInt32();
                genomic_data_line.Gtex_atrial_eQTL = reader.ReadString();
                genomic_data_line.Gtex_atrial_sQTL = reader.ReadString();
                genomic_data_line.Gtex_ventricular_eQTL = reader.ReadString();
                genomic_data_line.Gtex_ventricular_sQTL = reader.ReadString();
                genomic_data_line.Successfully_sequenced_celllines_count = reader.ReadInt32();
                genomic_data_line.Chrom = reader.ReadString();
                genomic_data_line.Relation_of_gene_symbol_to_drug = (Relation_of_gene_symbol_to_drug_enum)Enum.Parse(typeof(Relation_of_gene_symbol_to_drug_enum), reader.ReadString());
                genomic_data_line.Ontology = (Ontology_type_enum)Enum.Parse(typeof(Ontology_type_enum), reader.ReadString());
                genomic_data_line.Rank_of_gene_symbol_within_relation_for_drug = reader.ReadSingle();
                genomic_data_line.End = reader.ReadInt32();
                genomic_data_line.Gene_symbol = reader.ReadString();
                genomic_data_line.Drug_target_symbol = reader.ReadString();
                genomic_data_line.Minor_allele = reader.ReadString();
                genomic_data_line.Minor_allel_frequency = reader.ReadSingle();
                genomic_data_line.ReadWrite_major_alleles = reader.ReadString();
                genomic_data_line.ReadWrite_other_cell_lines_minor_allele_counts = reader.ReadString();
                genomic_data_line.Start = reader.ReadInt32();
                genomic_data_line.Variant_location = (Variant_location_refGene_enum)Enum.Parse(typeof(Variant_location_refGene_enum), reader.ReadString());
                if (  (!filter_by_chromosome)
                    ||(readOnlyChromosome_dict.ContainsKey(genomic_data_line.Chrom)))
                {
                    genomic_lines_list.Add(genomic_data_line);
                }
            }
            Add_to_array(genomic_lines_list.ToArray());
        }

        public string[] Get_all_chromsomes_from_binary(string completeFileName)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(completeFileName));
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            long last_index = reader.BaseStream.Length;
            Dictionary<string, bool> chromosome_dict = new Dictionary<string, bool>();
            while (reader.BaseStream.Position != last_index)
            {
                genomic_data_line = new Lincs_vcf_genomic_data_line_class();
                genomic_data_line.Rs_identifier = reader.ReadString();
                genomic_data_line.Cell_line = reader.ReadString();
                genomic_data_line.Cell_line_allele_depth_AD = reader.ReadInt32();
                genomic_data_line.Cell_line_approximate_read_depth_DP = reader.ReadInt32();
                genomic_data_line.Cell_line_conditional_genotype_quality = reader.ReadSingle();
                genomic_data_line.Cell_line_genotype_GT = reader.ReadString();
                genomic_data_line.Cell_line_minor_allele_count = reader.ReadInt32();
                genomic_data_line.Cell_line_phred_scale_genotype_quality_GQ = reader.ReadInt32();
                genomic_data_line.Cell_line_drug_with_outlier_response = reader.ReadString();
                genomic_data_line.ReadWrite_F1_score_weigths_with_outlier_responses = reader.ReadString();
                genomic_data_line.Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments = reader.ReadBoolean();
                genomic_data_line.Cell_line_frequency_of_cell_line_minor_allele = reader.ReadSingle();
                genomic_data_line.Cell_line_outlier_adj_pvalue = reader.ReadSingle();
                genomic_data_line.Cell_line_outlier_pvalue = reader.ReadSingle();
                genomic_data_line.Cell_line_outlier_mean_f1_score_without_outlier = reader.ReadSingle();
                genomic_data_line.Cell_line_outlier_F1_score_weight = reader.ReadSingle();
                genomic_data_line.Cell_line_drug_sample_count = reader.ReadInt32();
                genomic_data_line.Cadd_phred = reader.ReadSingle();
                genomic_data_line.SpAI_DS_snv_raw = reader.ReadSingle();
                genomic_data_line.SpAI_DS_indel_raw = reader.ReadSingle();
                genomic_data_line.Clnsig = reader.ReadString();
                genomic_data_line.Publication = reader.ReadString();
                genomic_data_line.Quality_aq = reader.ReadInt32();
                genomic_data_line.Gtex_atrial_eQTL = reader.ReadString();
                genomic_data_line.Gtex_atrial_sQTL = reader.ReadString();
                genomic_data_line.Gtex_ventricular_eQTL = reader.ReadString();
                genomic_data_line.Gtex_ventricular_sQTL = reader.ReadString();
                genomic_data_line.Successfully_sequenced_celllines_count = reader.ReadInt32();
                genomic_data_line.Chrom = reader.ReadString();
                genomic_data_line.Relation_of_gene_symbol_to_drug = (Relation_of_gene_symbol_to_drug_enum)Enum.Parse(typeof(Relation_of_gene_symbol_to_drug_enum), reader.ReadString());
                genomic_data_line.Ontology = (Ontology_type_enum)Enum.Parse(typeof(Ontology_type_enum), reader.ReadString());
                genomic_data_line.Rank_of_gene_symbol_within_relation_for_drug = reader.ReadSingle();
                genomic_data_line.End = reader.ReadInt32();
                genomic_data_line.Gene_symbol = reader.ReadString();
                genomic_data_line.Drug_target_symbol = reader.ReadString();
                genomic_data_line.Minor_allele = reader.ReadString();
                genomic_data_line.Minor_allel_frequency = reader.ReadSingle();
                genomic_data_line.ReadWrite_major_alleles = reader.ReadString();
                genomic_data_line.ReadWrite_other_cell_lines_minor_allele_counts = reader.ReadString();
                genomic_data_line.Start = reader.ReadInt32();
                genomic_data_line.Variant_location = (Variant_location_refGene_enum)Enum.Parse(typeof(Variant_location_refGene_enum), reader.ReadString());
                if (!chromosome_dict.ContainsKey(genomic_data_line.Chrom))
                {
                    chromosome_dict.Add(genomic_data_line.Chrom,true);
                }
            }
            return chromosome_dict.Keys.ToArray();
        }

        private Lincs_vcf_genomic_data_line_class Add_cell_line_specific_information(string cell_line, string genotype_GT, string phred_scale_genotype_quality_GQ, string allele_depth_AD, string approximate_read_depth_DP, Lincs_vcf_genomic_data_line_class genomic_data_line)
        {
            genomic_data_line.Cell_line = (string)cell_line.Clone();
            genomic_data_line.Cell_line_genotype_GT = (string)genotype_GT.Clone();
            int integer;
            if (int.TryParse(phred_scale_genotype_quality_GQ, out integer))
            { genomic_data_line.Cell_line_phred_scale_genotype_quality_GQ = integer; }
            else { genomic_data_line.Cell_line_phred_scale_genotype_quality_GQ = -1; }
            if (int.TryParse(allele_depth_AD, out integer))
            { genomic_data_line.Cell_line_allele_depth_AD = integer; }
            else { genomic_data_line.Cell_line_allele_depth_AD = -1; }
            if (int.TryParse(approximate_read_depth_DP, out integer))
            { genomic_data_line.Cell_line_approximate_read_depth_DP = integer; }
            else { genomic_data_line.Cell_line_approximate_read_depth_DP = -1; }
            string[] genotype_alleles = genomic_data_line.Cell_line_genotype_GT.Split('/');
            if (genotype_alleles.Length != 2) { throw new Exception(); }
            int minor_allel_count = 0;
            for (int indexGA = 0; indexGA < 2; indexGA++)
            {
                if (genotype_alleles[indexGA].Equals(genomic_data_line.Minor_allele))
                {
                    minor_allel_count++;
                }
            }
            genomic_data_line.Cell_line_minor_allele_count = minor_allel_count;
            return genomic_data_line;
        }

        private void Remove_uncompletely_sequenced_cellline_alleles()
        {
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            int data_length = this.Genomic_data.Length;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                data_line = this.Genomic_data[indexD];
                if (data_line.Cell_line_genotype_GT.IndexOf(".") == -1)
                {
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        private void Identify_variants_that_are_overrepresented_in_only_one_cell_line()
        {
            int data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            Lincs_vcf_genomic_data_line_class inner_data_line;
            Lincs_vcf_genomic_data_line_class second_inner_data_line;
            this.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_chromosome_start_end_drug_drugTargetSymbol_relationToDrug_geneSymbol_cellline(this.Genomic_data);
            int highest_minor_allele_counts = 0;
            int firstIndexSameGene = -1;
            string minor_allel_cellline;
            List<int> minor_alleles_count_of_other_celllines = new List<int>();
            Dictionary<string, bool> sequenced_celllines_dict = new Dictionary<string, bool>();
            int successfully_sequenced_celllines_count;
            List<string> celllines_with_highest_minor_allele_count = new List<string>();
            for (int indexData = 0; indexData < data_length; indexData++)
            {
                data_line = this.Genomic_data[indexData];
                if ((indexData == 0)
                    || (!data_line.Chrom.Equals(this.Genomic_data[indexData - 1].Chrom))
                    || (!data_line.Start.Equals(this.Genomic_data[indexData - 1].Start))
                    || (!data_line.End.Equals(this.Genomic_data[indexData - 1].End))
                    || (!data_line.Cell_line_drug_with_outlier_response.Equals(this.Genomic_data[indexData - 1].Cell_line_drug_with_outlier_response))
                    || (!data_line.Drug_target_symbol.Equals(this.Genomic_data[indexData - 1].Drug_target_symbol))
                    || (!data_line.Relation_of_gene_symbol_to_drug.Equals(this.Genomic_data[indexData - 1].Relation_of_gene_symbol_to_drug))
                    || (!data_line.Gene_symbol.Equals(this.Genomic_data[indexData - 1].Gene_symbol)))
                {
                    highest_minor_allele_counts = -1;
                    sequenced_celllines_dict.Clear();
                    celllines_with_highest_minor_allele_count.Clear();
                    firstIndexSameGene = indexData;
                }
                sequenced_celllines_dict.Add(data_line.Cell_line, true);
                if (data_line.Cell_line_minor_allele_count > highest_minor_allele_counts)
                {
                    highest_minor_allele_counts = data_line.Cell_line_minor_allele_count;
                    celllines_with_highest_minor_allele_count.Clear();
                    celllines_with_highest_minor_allele_count.Add(data_line.Cell_line);
                }
                else if (data_line.Cell_line_minor_allele_count == highest_minor_allele_counts)
                {
                    celllines_with_highest_minor_allele_count.Add(data_line.Cell_line);
                }
                if ((indexData == data_length - 1)
                    || (!data_line.Chrom.Equals(this.Genomic_data[indexData + 1].Chrom))
                    || (!data_line.Start.Equals(this.Genomic_data[indexData + 1].Start))
                    || (!data_line.End.Equals(this.Genomic_data[indexData + 1].End))
                    || (!data_line.Cell_line_drug_with_outlier_response.Equals(this.Genomic_data[indexData + 1].Cell_line_drug_with_outlier_response))
                    || (!data_line.Drug_target_symbol.Equals(this.Genomic_data[indexData + 1].Drug_target_symbol))
                    || (!data_line.Relation_of_gene_symbol_to_drug.Equals(this.Genomic_data[indexData + 1].Relation_of_gene_symbol_to_drug))
                    || (!data_line.Gene_symbol.Equals(this.Genomic_data[indexData + 1].Gene_symbol)))
                {
                    minor_allel_cellline = "none";
                    if (celllines_with_highest_minor_allele_count.Distinct().ToList().Count != celllines_with_highest_minor_allele_count.Count) { throw new Exception(); }
                    if (celllines_with_highest_minor_allele_count.Count == 1)
                    {
                        minor_allel_cellline = celllines_with_highest_minor_allele_count[0];
                    }
                    successfully_sequenced_celllines_count = sequenced_celllines_dict.Keys.ToArray().Length;
                    for (int indexInner = firstIndexSameGene; indexInner <= indexData; indexInner++)
                    {
                        inner_data_line = this.Genomic_data[indexInner];
                        inner_data_line.Successfully_sequenced_celllines_count = successfully_sequenced_celllines_count;
                        minor_alleles_count_of_other_celllines.Clear();
                        for (int indexInner_second = firstIndexSameGene; indexInner_second <= indexData; indexInner_second++)
                        {
                            if (indexInner != indexInner_second)
                            {
                                second_inner_data_line = this.Genomic_data[indexInner_second];
                                minor_alleles_count_of_other_celllines.Add(second_inner_data_line.Cell_line_minor_allele_count);
                            }
                        }
                        inner_data_line.Other_cell_lines_minor_allele_counts = minor_alleles_count_of_other_celllines.OrderBy(l=>l).ToArray();
                        if (inner_data_line.Cell_line.Equals(minor_allel_cellline))
                        {
                            inner_data_line.Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments = true;
                        }
                        else
                        {
                            inner_data_line.Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments = false;
                        }
                    }
                }
            }
        }

        private void Keep_only_lines_with_variants_overrepresented_in_on_cell_line()
        {
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class data_line in this.Genomic_data)
            {
                if (data_line.Cell_line_minor_allele_only_in_this_cell_line_compared_to_all_other_same_drug_treatments)
                {
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Reset_cell_line_drug_with_outlier_response_and_delete_duplicates()
        {
            this.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_chromosome_start_end_geneSymbol_cellline(this.Genomic_data);
            int data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                data_line = this.Genomic_data[indexD];
                if ((indexD == 0)
                    || (data_line.Chrom.Equals(this.Genomic_data[indexD - 1].Chrom))
                    || (data_line.Start.Equals(this.Genomic_data[indexD - 1].Start))
                    || (data_line.End.Equals(this.Genomic_data[indexD - 1].End))
                    || (data_line.Gene_symbol.Equals(this.Genomic_data[indexD - 1].Gene_symbol))
                    || (data_line.Cell_line.Equals(this.Genomic_data[indexD - 1].Cell_line)))
                {
                    data_line.Cell_line_drug_with_outlier_response = "";
                    keep.Add(data_line);
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Add_outliers_and_keep_only_lines_with_outlier(Outlier_class outlier)
        {
            int data_length = this.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class data_line;
            Lincs_vcf_genomic_data_line_class outlier_data_line;
            this.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_cellline_celllineDrugWithOutlierResponse(this.Genomic_data);
            outlier.Outliers = outlier.Outliers.OrderBy(l => l.Outlier).ThenBy(l => l.Entity).ToArray();
            Outlier_line_class outlier_line;
            int outlier_length = outlier.Outliers.Length;
            int indexOutlier = 0;
            int stringCompare = -2;
            StringBuilder outlier_drugs = new StringBuilder();
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                data_line = this.Genomic_data[indexD];
                if (   (indexD != 0)
                    && (data_line.Cell_line.CompareTo(this.Genomic_data[indexD - 1].Cell_line)<0)
                    && (data_line.Cell_line_drug_with_outlier_response.CompareTo(this.Genomic_data[indexD - 1].Cell_line_drug_with_outlier_response)<0))
                { throw new Exception(); }
                if (String.IsNullOrEmpty(data_line.Cell_line_drug_with_outlier_response)) { throw new Exception(); }
                stringCompare = -2;
                outlier_drugs.Clear();
                while ((indexOutlier < outlier_length) && (stringCompare < 0))
                {
                    outlier_line = outlier.Outliers[indexOutlier];
                    if (   (indexOutlier != 0)
                        && (outlier_line.Outlier.Equals(outlier.Outliers[indexOutlier - 1].Outlier))
                        && (outlier_line.Entity.Equals(outlier.Outliers[indexOutlier - 1].Entity)))
                    { throw new Exception(); }

                    stringCompare = outlier_line.Outlier.CompareTo(data_line.Cell_line);
                    if (stringCompare==0)
                    {
                        stringCompare = outlier_line.Entity.CompareTo(data_line.Cell_line_drug_with_outlier_response);
                    }
                    if (stringCompare < 0)
                    {
                        indexOutlier++;
                        outlier_drugs.Clear();
                    }
                    else if (stringCompare == 0)
                    {
                        outlier_data_line = data_line.Deep_copy();
                        outlier_data_line.Cell_line_outlier_adj_pvalue = outlier_line.Adj_pvalue;
                        outlier_data_line.Cell_line_outlier_pvalue = outlier_line.Pvalue;
                        outlier_data_line.Cell_line_outlier_F1_score_weight = outlier_line.F1_score_weight;
                        outlier_data_line.Cell_line_drug_sample_count = outlier_line.Sample_count;
                        outlier_data_line.Cell_line_outlier_mean_f1_score_without_outlier = outlier_line.Mean_F1score_without_outlier;
                        keep.Add(outlier_data_line); 
                    }
                }
            }
            this.Genomic_data = keep.ToArray();
        }

        public void Add_deep_copy_of_other(Lincs_vcf_genomic_data_class other)
        {
            Lincs_vcf_genomic_data_class other_deep_copy = other.Deep_copy();
            Add_to_array(other_deep_copy.Genomic_data);
        }

        private Lincs_vcf_genomic_input_data_line_class[] Add_aq_integer(Lincs_vcf_genomic_input_data_line_class[] read_lines)
        {
            int read_lines_length = read_lines.Length;
            Lincs_vcf_genomic_input_data_line_class read_line;
            int integer;
            for (int indexR = 0; indexR < read_lines_length; indexR++)
            {
                read_line = read_lines[indexR];
                if (int.TryParse(read_line.AQ, out integer))
                {
                    read_line.AQ_integer = integer;
                }
                else
                {
                    read_line.AQ_integer = -1;
                }
            }
            return read_lines;
        }

        private Lincs_vcf_genomic_input_data_line_class[] Remove_duplicates(Lincs_vcf_genomic_input_data_line_class[] read_lines)
        {
            read_lines = Lincs_vcf_genomic_input_data_line_class.Order_by_rsID_chr_start_end_ref_alt_descendingAQ(read_lines);
            List<Lincs_vcf_genomic_input_data_line_class> keep = new List<Lincs_vcf_genomic_input_data_line_class>();
            Lincs_vcf_genomic_input_data_line_class read_line;
            int read_lines_length = read_lines.Length;
            bool at_least_one_aq_integer_not_zero = false;
            for (int indexR = 0; indexR < read_lines_length; indexR++)
            {
                read_line = read_lines[indexR];
                if (read_line.AQ_integer > 0) { at_least_one_aq_integer_not_zero = true; }
                if ((indexR == 0)
                    || (!read_line.rsID.Equals(read_lines[indexR - 1].rsID))
                    || (!read_line.Chr.Equals(read_lines[indexR - 1].Chr))
                    || (!read_line.Start.Equals(read_lines[indexR - 1].Start))
                    || (!read_line.End.Equals(read_lines[indexR - 1].End))
                    || (!read_line.Ref.Equals(read_lines[indexR - 1].Ref))
                    || (!read_line.Alt.Equals(read_lines[indexR - 1].Alt)))
                {
                    keep.Add(read_line);
                }
            }
            if (!at_least_one_aq_integer_not_zero) { throw new Exception(); }
            return keep.ToArray();
        }

        private void Investigate_selected_read_lines(Lincs_vcf_genomic_input_data_line_class[] read_lines)
        {
            read_lines = Lincs_vcf_genomic_input_data_line_class.Order_by_rsID_chr_start_end_ref_alt_descendingAQ(read_lines);
            List<Lincs_vcf_genomic_input_data_line_class> investigate = new List<Lincs_vcf_genomic_input_data_line_class>();
            Lincs_vcf_genomic_input_data_line_class read_line;
            int read_lines_length = read_lines.Length;
            for (int indexR = 0; indexR < read_lines_length; indexR++)
            {
                read_line = read_lines[indexR];
                if ((read_line.Chr.Equals("chr1"))
                    && (read_line.Start.Equals(121789295))
                    && (read_line.End.Equals(121789295)))
                {
                    investigate.Add(read_line);
                }
            }
        }

        private Lincs_vcf_genomic_input_data_line_class[] Add_own_rs_if_missing(Lincs_vcf_genomic_input_data_line_class[] read_lines)
        {
            Lincs_vcf_genomic_input_data_line_class read_line;
            int read_lines_length = read_lines.Length;
            for (int indexR = 0; indexR < read_lines_length; indexR++)
            {
                read_line = read_lines[indexR];
                if (read_line.rsID.Equals("."))
                {
                    read_line.rsID = read_line.Chr + ":" + read_line.Start;
                }
            }
            return read_lines;
        }

        private void Read_all_lines_of_given_chromosome(string complete_fileName, string chromosome)
        {
            Lincs_vcf_genomic_input_readOptions_class readOptions = new Lincs_vcf_genomic_input_readOptions_class(complete_fileName, chromosome);
            Lincs_vcf_genomic_input_data_line_class[] read_lines = ReadWriteClass.ReadRawData_and_FillArray<Lincs_vcf_genomic_input_data_line_class>(readOptions);
            read_lines = Add_aq_integer(read_lines);
            read_lines = Remove_duplicates(read_lines);
            read_lines = Add_own_rs_if_missing(read_lines);
            int read_lines_length = read_lines.Length;
            Lincs_vcf_genomic_input_data_line_class read_line;
            Lincs_vcf_genomic_data_line_class new_genomic_data_line;
            Lincs_vcf_genomic_data_line_class new_genomic_data_base_line;
            bool add_potential_lines;
            List<Lincs_vcf_genomic_data_line_class> potential_add_genomic_lines = new List<Lincs_vcf_genomic_data_line_class>();
            List<Lincs_vcf_genomic_data_line_class> genomic_lines = new List<Lincs_vcf_genomic_data_line_class>();
            List<float> allel_frequencies_list = new List<float>();
            List<string> allels_list = new List<string>();
            float current_allel_frequency;
            string[] splitStrings;
            int indexMinorAllel;
            float minimum_frequency;
            int allels_count;
            string variant_location_string = "";
            float float_value;
            StringBuilder major_allel_sb = new StringBuilder();
            bool add_line;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            string[] variant_locations;
            bool at_least_one_aq_integer_larger_than_zero = false;
            int total_rsIDs_with_variants_with_frequencies = 0;
            int rsIDs_with_more_than_2_variants = 0;
            int rsIDs_with_more_than_1_variant_with_frequency_below_equal_01 = 0;
            int rsIDs_with_1_variant_with_frequency_below_equal_01 = 0;
            for (int indexR = 0; indexR < read_lines_length; indexR++)
            {
                read_line = read_lines[indexR];
                if (read_line.AQ_integer>0) { at_least_one_aq_integer_larger_than_zero = true; }
                geneSymbols = read_line.Gene_refGene.Split(';');
                geneSymbols = Overlap_class.Get_part_of_list1_but_not_of_list2(geneSymbols, "NONE");
                geneSymbols_length = geneSymbols.Length;
                if (geneSymbols.Distinct().ToArray().Length!=geneSymbols_length) { throw new Exception(); }
                variant_locations = read_line.Func_refGene.Split(';');
                for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
                {
                    geneSymbol = geneSymbols[indexGS];
                    variant_location_string = "";
                    if (indexGS >= variant_locations.Length) 
                    { 
                       // if (variant_locations.Length!=1) { throw new Exception(); }
                        variant_location_string = variant_locations[variant_locations.Length - 1]; 
                    }
                    else { variant_location_string = variant_locations[indexGS]; }

                    add_line = true;

                    new_genomic_data_base_line = new Lincs_vcf_genomic_data_line_class();
                    new_genomic_data_base_line.Relation_of_gene_symbol_to_drug = Relation_of_gene_symbol_to_drug_enum.Drugs_not_considered_yet;
                    new_genomic_data_base_line.Rs_identifier = (string)read_line.rsID.Clone();
                    new_genomic_data_base_line.Chrom = (string)read_line.Chr.Clone();
                    new_genomic_data_base_line.Start = read_line.Start;
                    new_genomic_data_base_line.End = read_line.End;
                    new_genomic_data_base_line.Gene_symbol = (string)geneSymbol.Clone();
                    if (float.TryParse(read_line.spAI_DS_indel_raw, out float_value))
                    { new_genomic_data_base_line.SpAI_DS_indel_raw = float_value; }
                    else { new_genomic_data_base_line.SpAI_DS_indel_raw = -1; }
                    if (float.TryParse(read_line.spAI_DS_snv_raw, out float_value))
                    { new_genomic_data_base_line.SpAI_DS_snv_raw = float_value; }
                    else { new_genomic_data_base_line.SpAI_DS_snv_raw = -1; }
                    new_genomic_data_base_line.Clnsig = (string)read_line.CLNSIG.Clone();
                    new_genomic_data_base_line.Publication = (string)read_line.Publication.Clone();
                    new_genomic_data_base_line.Gtex_atrial_eQTL = (string)read_line.gtex_atria_egene.Clone();
                    new_genomic_data_base_line.Gtex_atrial_sQTL = (string)read_line.gtex_atria_sgene.Clone();
                    new_genomic_data_base_line.Gtex_ventricular_eQTL = (string)read_line.gtex_vent_egene.Clone();
                    new_genomic_data_base_line.Gtex_ventricular_sQTL = (string)read_line.gtex_vent_sgene.Clone();
                    if (  (  (!new_genomic_data_base_line.Gtex_atrial_eQTL.Equals("."))
                           ||(!new_genomic_data_base_line.Gtex_atrial_sQTL.Equals("."))
                           ||(!new_genomic_data_base_line.Gtex_ventricular_eQTL.Equals("."))
                           ||(!new_genomic_data_base_line.Gtex_ventricular_sQTL.Equals(".")))
                        &&(geneSymbols_length!=1))
                    {
                   //     throw new Exception();
                    }


                    new_genomic_data_base_line.Cell_line_frequency_of_cell_line_minor_allele = -1;//float.Parse(read_line.Cell_line_AF);
                    if (float.TryParse(read_line.CADD_phred, out float_value))
                    { new_genomic_data_base_line.Cadd_phred = float_value; }
                    else { new_genomic_data_base_line.Cadd_phred = -1; }
                    new_genomic_data_base_line.Quality_aq = read_line.AQ_integer;


                    Text_class.Set_first_letter_to_uppercase_and_rest_to_lowercase(ref variant_location_string);
                    if (variant_location_string.Equals("."))
                    {
                        new_genomic_data_base_line.Variant_location = Variant_location_refGene_enum.Not_determined;
                    }
                    else
                    {
                        new_genomic_data_base_line.Variant_location = (Variant_location_refGene_enum)Enum.Parse(typeof(Variant_location_refGene_enum), variant_location_string);
                    }

                    #region Identify and add minor allel and minor allel frequency
                    allels_list.Clear();
                    allel_frequencies_list.Clear();
                    allels_list.Add(read_line.Ref);
                    allel_frequencies_list.Add(1);
                    allels_list.AddRange(read_line.Alt.Split(','));
                    splitStrings = read_line.AF.Split(',');
                    foreach (string splitString in splitStrings)
                    {
                        current_allel_frequency = 0;
                        if (!splitString.Equals("."))
                        {
                            current_allel_frequency = float.Parse(splitString);
                            allel_frequencies_list.Add(current_allel_frequency);
                            allel_frequencies_list[0] -= current_allel_frequency;
                        }
                        else
                        {
                            add_line = false;
                        }
                    }
                    allels_count = allels_list.Count;
                    minimum_frequency = 1;
                    indexMinorAllel = -1;
                    if (add_line)
                    {
                        int frequency_below_or_equal_01 = 0;
                        for (int indexAllelCount = 0; indexAllelCount < allels_count; indexAllelCount++)
                        {
                            if (allel_frequencies_list[indexAllelCount] > 1) { throw new Exception(); }
                            if (allel_frequencies_list[indexAllelCount] <= 0.1) { frequency_below_or_equal_01++; }
                            if (allel_frequencies_list[indexAllelCount] < minimum_frequency)
                            {
                                minimum_frequency = allel_frequencies_list[indexAllelCount];
                                indexMinorAllel = indexAllelCount;
                            }
                        }
                        total_rsIDs_with_variants_with_frequencies++;
                        if (allel_frequencies_list.Count>2) { rsIDs_with_more_than_2_variants++; }
                        if (frequency_below_or_equal_01 > 1) { rsIDs_with_more_than_1_variant_with_frequency_below_equal_01++; }
                        if (frequency_below_or_equal_01 == 1) { rsIDs_with_1_variant_with_frequency_below_equal_01++; }
                        new_genomic_data_base_line.Minor_allele = (string)allels_list[indexMinorAllel].Clone();
                        new_genomic_data_base_line.Major_alleles = Overlap_class.Get_part_of_list1_but_not_of_list2(allels_list.ToArray(), new string[] { new_genomic_data_base_line.Minor_allele });
                        new_genomic_data_base_line.Minor_allel_frequency = allel_frequencies_list[indexMinorAllel];
                    }
                    #endregion

                    if (add_line)
                    {
                        add_potential_lines = true;
                        potential_add_genomic_lines.Clear();

                        if (add_potential_lines)
                        {
                            new_genomic_data_line = new_genomic_data_base_line.Deep_copy();
                            new_genomic_data_line = Add_cell_line_specific_information("MSN01", read_line.MSN01_GT, read_line.MSN01_GQ, read_line.MSN01_AD, read_line.MSN01_DP, new_genomic_data_line);
                            if (new_genomic_data_line.Cell_line_genotype_GT.IndexOf(".") != -1)
                            {
                                add_potential_lines = false;
                            }
                            potential_add_genomic_lines.Add(new_genomic_data_line);
                        }

                        if (add_potential_lines)
                        {
                            new_genomic_data_line = new_genomic_data_base_line.Deep_copy();
                            new_genomic_data_line = Add_cell_line_specific_information("MSN02", read_line.MSN02_GT, read_line.MSN02_GQ, read_line.MSN02_AD, read_line.MSN02_DP, new_genomic_data_line);
                            if (new_genomic_data_line.Cell_line_genotype_GT.IndexOf(".") != -1)
                            {
                                add_potential_lines = false;
                            }
                            potential_add_genomic_lines.Add(new_genomic_data_line);
                        }

                        if (add_potential_lines)
                        {
                            new_genomic_data_line = new_genomic_data_base_line.Deep_copy();
                            new_genomic_data_line = Add_cell_line_specific_information("MSN05", read_line.MSN05_GT, read_line.MSN05_GQ, read_line.MSN05_AD, read_line.MSN05_DP, new_genomic_data_line);
                            if (new_genomic_data_line.Cell_line_genotype_GT.IndexOf(".") != -1)
                            {
                                add_potential_lines = false;
                            }
                            potential_add_genomic_lines.Add(new_genomic_data_line);
                        }

                        if (add_potential_lines)
                        {
                            new_genomic_data_line = new_genomic_data_base_line.Deep_copy();
                            new_genomic_data_line = Add_cell_line_specific_information("MSN06", read_line.MSN06_GT, read_line.MSN06_GQ, read_line.MSN06_AD, read_line.MSN06_DP, new_genomic_data_line);
                            if (new_genomic_data_line.Cell_line_genotype_GT.IndexOf(".") != -1)
                            {
                                add_potential_lines = false;
                            }
                            potential_add_genomic_lines.Add(new_genomic_data_line);
                        }

                        if (add_potential_lines)
                        {
                            new_genomic_data_line = new_genomic_data_base_line.Deep_copy();
                            new_genomic_data_line = Add_cell_line_specific_information("MSN08", read_line.MSN08_GT, read_line.MSN08_GQ, read_line.MSN08_AD, read_line.MSN08_DP, new_genomic_data_line);
                            if (new_genomic_data_line.Cell_line_genotype_GT.IndexOf(".") != -1)
                            {
                                add_potential_lines = false;
                            }
                            potential_add_genomic_lines.Add(new_genomic_data_line);
                        }

                        if (add_potential_lines)
                        {
                            new_genomic_data_line = new_genomic_data_base_line.Deep_copy();
                            new_genomic_data_line = Add_cell_line_specific_information("MSN09", read_line.MSN09_GT, read_line.MSN09_GQ, read_line.MSN09_AD, read_line.MSN09_DP, new_genomic_data_line);
                            if (new_genomic_data_line.Cell_line_genotype_GT.IndexOf(".") != -1)
                            {
                                add_potential_lines = false;
                            }
                            potential_add_genomic_lines.Add(new_genomic_data_line);
                        }

                        if (add_potential_lines)
                        {
                            genomic_lines.AddRange(potential_add_genomic_lines);
                        }
                    }
                }
                read_lines[indexR] = null;
            }
            if (!at_least_one_aq_integer_larger_than_zero) { throw new Exception(); }
            if (rsIDs_with_more_than_1_variant_with_frequency_below_equal_01 != 0) { throw new Exception(); }
            //if (rsIDs_with_1_variant_with_frequency_below_equal_01 == 0) { throw new Exception(); } //aplies for chrM
            this.Genomic_data = genomic_lines.ToArray();
        }

        public Lincs_vcf_genomic_data_class Deep_copy()
        {
            Lincs_vcf_genomic_data_class copy = (Lincs_vcf_genomic_data_class)this.MemberwiseClone();
            int data_length = this.Genomic_data.Length;
            copy.Genomic_data = new Lincs_vcf_genomic_data_line_class[data_length];
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                copy.Genomic_data[indexD] = this.Genomic_data[indexD].Deep_copy();
            }
            return copy;
        }

        public Lincs_vcf_genomic_data_class Shallow_copy()
        {
            Lincs_vcf_genomic_data_class copy = (Lincs_vcf_genomic_data_class)this.MemberwiseClone();
            int data_length = this.Genomic_data.Length;
            copy.Genomic_data = new Lincs_vcf_genomic_data_line_class[data_length];
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                copy.Genomic_data[indexD] = this.Genomic_data[indexD];
            }
            return copy;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Lincs_vcf_genomic_data_summary_line_class
    {
        public Lincs_genomics_analysis_stage_enum Filter_stage { get; set; }
        public Relation_of_gene_symbol_to_drug_enum Relation_to_gene_symbol { get; set; }
        public string DrugOrDrugGroup { get; set; }
        public string Counted_entityClass { get; set; }
        public float Counts { get; set; }
        
        public Lincs_vcf_genomic_data_summary_line_class Deep_copy()
        {
            Lincs_vcf_genomic_data_summary_line_class copy = (Lincs_vcf_genomic_data_summary_line_class)this.MemberwiseClone();
            copy.DrugOrDrugGroup = (string)this.DrugOrDrugGroup.Clone();
            copy.Counted_entityClass = (string)this.Counted_entityClass.Clone();
            return copy;
        }
    }

    class Lincs_vcf_genomic_data_summary_readWriteOptions : ReadWriteOptions_base
    {
        public Lincs_vcf_genomic_data_summary_readWriteOptions(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Filter_stage", "DrugOrDrugGroup", "Relation_to_gene_symbol","Counted_entityClass", "Counts" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_vcf_genomic_data_summary_class
    {
        public Lincs_vcf_genomic_data_summary_line_class[] Summary { get; set; }

        public Lincs_vcf_genomic_data_summary_class()
        {
            this.Summary = new Lincs_vcf_genomic_data_summary_line_class[0];
        }

        private void Add_to_array(Lincs_vcf_genomic_data_summary_line_class[] add_summary)
        {
            int this_summary_length = this.Summary.Length;
            int add_summary_length = add_summary.Length;
            int new_summary_length = this_summary_length + add_summary_length;
            int indexNew = -1;
            Lincs_vcf_genomic_data_summary_line_class[] new_summary = new Lincs_vcf_genomic_data_summary_line_class[new_summary_length];
            for (int indexThis = 0; indexThis < this_summary_length; indexThis++)
            {
                indexNew++;
                new_summary[indexNew] = this.Summary[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_summary_length; indexAdd++)
            {
                indexNew++;
                new_summary[indexNew] = add_summary[indexAdd];
            }
            this.Summary = new_summary.ToArray();
        }

        private Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>> Add_counts_to_drugOrDrugGroup_relationToGeneSymbol_counts_dict(Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>> singleDrugOrDrugGroup_relation_entity_dict, Dictionary<string,string[]> drug_singleDrugOrDrugGroup_dict, Lincs_vcf_genomic_data_line_class vcf_genomic_line, string entity)
        {
            string[] add_to_singleDrugOrDrugGroups = new string[] { vcf_genomic_line.Cell_line_drug_with_outlier_response };
            if (drug_singleDrugOrDrugGroup_dict.ContainsKey(vcf_genomic_line.Cell_line_drug_with_outlier_response))
            { add_to_singleDrugOrDrugGroups = Overlap_class.Get_union(drug_singleDrugOrDrugGroup_dict[vcf_genomic_line.Cell_line_drug_with_outlier_response], add_to_singleDrugOrDrugGroups); }
            string add_to_singleDrugOrDrugGroup;
            int add_to_singleDrugOrDrugGroups_length = add_to_singleDrugOrDrugGroups.Length;
            for (int indexAdd=0; indexAdd<add_to_singleDrugOrDrugGroups_length;indexAdd++)
            {
                add_to_singleDrugOrDrugGroup = add_to_singleDrugOrDrugGroups[indexAdd];
                if (!singleDrugOrDrugGroup_relation_entity_dict.ContainsKey(add_to_singleDrugOrDrugGroup))
                {
                    singleDrugOrDrugGroup_relation_entity_dict.Add(add_to_singleDrugOrDrugGroup, new Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>());
                }
                if (!singleDrugOrDrugGroup_relation_entity_dict[add_to_singleDrugOrDrugGroup].ContainsKey(vcf_genomic_line.Relation_of_gene_symbol_to_drug))
                {
                    singleDrugOrDrugGroup_relation_entity_dict[add_to_singleDrugOrDrugGroup].Add(vcf_genomic_line.Relation_of_gene_symbol_to_drug, new Dictionary<string, bool>());
                }
                if (!singleDrugOrDrugGroup_relation_entity_dict[add_to_singleDrugOrDrugGroup][vcf_genomic_line.Relation_of_gene_symbol_to_drug].ContainsKey(entity))
                {
                    singleDrugOrDrugGroup_relation_entity_dict[add_to_singleDrugOrDrugGroup][vcf_genomic_line.Relation_of_gene_symbol_to_drug].Add(entity,true);
                }
            }
            return singleDrugOrDrugGroup_relation_entity_dict;
        }

        public Dictionary<string,Dictionary<Relation_of_gene_symbol_to_drug_enum,float>> Generate_drugOrDrugGroup_relation_count_dict(Dictionary<string,Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>> singleDrugOrDrugGroup_relation_entity_dict)
        {
            Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, float>> drugOrDrugGroup_relation_count_dict = new Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, float>>();
            Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string,bool>> relation_entity_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>();
            Dictionary<string, List<Relation_of_gene_symbol_to_drug_enum>> entity_relations_dict = new Dictionary<string, List<Relation_of_gene_symbol_to_drug_enum>>();
            Relation_of_gene_symbol_to_drug_enum[] relations;
            Relation_of_gene_symbol_to_drug_enum relation;
            int relations_length;
            string[] entities;
            string entity;
            int entities_length;
            string[] singleDrugsOrDrugGroups = singleDrugOrDrugGroup_relation_entity_dict.Keys.ToArray();
            string singleDrugOrDrugGroup;
            int singleDrugsOrDrugGroups_length = singleDrugsOrDrugGroups.Length;
            float add_value_for_current_gene;
            for (int indexSD=0; indexSD<singleDrugsOrDrugGroups_length; indexSD++)
            {
                singleDrugOrDrugGroup = singleDrugsOrDrugGroups[indexSD];

                entity_relations_dict.Clear();
                relation_entity_dict = singleDrugOrDrugGroup_relation_entity_dict[singleDrugOrDrugGroup];
                relations = relation_entity_dict.Keys.ToArray();
                relations_length = relations.Length;
                for (int indexR=0; indexR<relations_length; indexR++)
                {
                    relation = relations[indexR];
                    entities = relation_entity_dict[relation].Keys.ToArray();
                    entities_length = entities.Length;
                    for (int indexGS=0; indexGS< entities_length; indexGS++)
                    {
                        entity = entities[indexGS];
                        if (!entity_relations_dict.ContainsKey(entity))
                        {
                            entity_relations_dict.Add(entity, new List<Relation_of_gene_symbol_to_drug_enum>());
                        }
                        entity_relations_dict[entity].Add(relation);
                    }
                }

                entities = entity_relations_dict.Keys.Distinct().ToArray();
                entities_length = entities.Length;
                for (int indexE=0; indexE<entities_length; indexE++)
                {
                    entity = entities[indexE];
                    relations = entity_relations_dict[entity].Distinct().ToArray();
                    relations_length = relations.Length;
                    add_value_for_current_gene = 1F / relations_length;
                    for (int indexR=0; indexR<relations_length; indexR++)
                    {
                        relation = relations[indexR];
                        if (!drugOrDrugGroup_relation_count_dict.ContainsKey(singleDrugOrDrugGroup))
                        {
                            drugOrDrugGroup_relation_count_dict.Add(singleDrugOrDrugGroup, new Dictionary<Relation_of_gene_symbol_to_drug_enum, float>());
                        }
                        if (!drugOrDrugGroup_relation_count_dict[singleDrugOrDrugGroup].ContainsKey(relation))
                        {
                            drugOrDrugGroup_relation_count_dict[singleDrugOrDrugGroup].Add(relation, 0);
                        }
                        drugOrDrugGroup_relation_count_dict[singleDrugOrDrugGroup][relation] += add_value_for_current_gene;
                    }
                }
            }
            return drugOrDrugGroup_relation_count_dict;
        }

        public void Generate_new_summary_lines_and_add_to_array(Lincs_genomics_analysis_stage_enum filter_stage, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, float>> singleDrugOrDrugGroup_relation_count_dict, string entityClass)
        {
            string[] singleDrugOrDrugGroups = singleDrugOrDrugGroup_relation_count_dict.Keys.ToArray();
            string singleDrugOrDrugGroup;
            int singleDrugOrDrugGroups_length = singleDrugOrDrugGroups.Length;
            Dictionary<Relation_of_gene_symbol_to_drug_enum, float> relation_count_dict;
            Relation_of_gene_symbol_to_drug_enum[] relations;
            Relation_of_gene_symbol_to_drug_enum relation;
            int relations_length;
            Lincs_vcf_genomic_data_summary_line_class summary_line;
            List<Lincs_vcf_genomic_data_summary_line_class> new_summary_lines = new List<Lincs_vcf_genomic_data_summary_line_class>();
            for (int indexDrug=0; indexDrug< singleDrugOrDrugGroups_length; indexDrug++)
            {
                singleDrugOrDrugGroup = singleDrugOrDrugGroups[indexDrug];
                relation_count_dict = singleDrugOrDrugGroup_relation_count_dict[singleDrugOrDrugGroup];
                relations = relation_count_dict.Keys.ToArray();
                relations_length = relations.Length;
                for (int indexRelation=0; indexRelation<relations_length;indexRelation++)
                {
                    relation = relations[indexRelation];
                    summary_line = new Lincs_vcf_genomic_data_summary_line_class();
                    summary_line.DrugOrDrugGroup = (string)singleDrugOrDrugGroup.Clone();
                    summary_line.Filter_stage = filter_stage;
                    summary_line.Counted_entityClass = (string)entityClass.Clone();
                    summary_line.Relation_to_gene_symbol = relation;
                    summary_line.Counts = relation_count_dict[relation];
                    new_summary_lines.Add(summary_line);
                }
            }
            Add_to_array(new_summary_lines.ToArray());
        }

        public void Generate_from_genomic_data_and_add_to_array(Lincs_vcf_genomic_data_class genomic_data, Dictionary<string,string[]> drug_drugGroup_dict, Lincs_genomics_analysis_stage_enum filter_stage)
        {
            Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>> singleDrugOrDrugGroup_relation_rsIdentifier_dict = new Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>>();
            Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>> singleDrugOrDrugGroup_relation_geneSymbol_dict = new Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, bool>>>();
            int genomic_data_length = genomic_data.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class vcf_genomic_line;

            Dictionary<string, List<Relation_of_gene_symbol_to_drug_enum>> rsIdentifier_relationToGeneSymbol_dict = new Dictionary<string, List<Relation_of_gene_symbol_to_drug_enum>>();
            Dictionary<string, List<Relation_of_gene_symbol_to_drug_enum>> geneSymbol_relationToGeneSymbol_dict = new Dictionary<string, List<Relation_of_gene_symbol_to_drug_enum>>();
            for (int indexGD = 0; indexGD < genomic_data_length; indexGD++)
            {
                vcf_genomic_line = genomic_data.Genomic_data[indexGD];
                singleDrugOrDrugGroup_relation_geneSymbol_dict = Add_counts_to_drugOrDrugGroup_relationToGeneSymbol_counts_dict(singleDrugOrDrugGroup_relation_geneSymbol_dict, drug_drugGroup_dict, vcf_genomic_line, vcf_genomic_line.Gene_symbol);
                singleDrugOrDrugGroup_relation_rsIdentifier_dict = Add_counts_to_drugOrDrugGroup_relationToGeneSymbol_counts_dict(singleDrugOrDrugGroup_relation_rsIdentifier_dict, drug_drugGroup_dict, vcf_genomic_line, vcf_genomic_line.Rs_identifier);
            }
            Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, float>> drugOrDrugGroup_relation_geneSymbolCounts_dict = Generate_drugOrDrugGroup_relation_count_dict(singleDrugOrDrugGroup_relation_geneSymbol_dict);
            Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, float>> drugOrDrugGroup_relation_rsIdentifier_dict = Generate_drugOrDrugGroup_relation_count_dict(singleDrugOrDrugGroup_relation_rsIdentifier_dict);
            Generate_new_summary_lines_and_add_to_array(filter_stage, drugOrDrugGroup_relation_geneSymbolCounts_dict, "Gene symbol");
            Generate_new_summary_lines_and_add_to_array(filter_stage, drugOrDrugGroup_relation_rsIdentifier_dict, "rsID");
        }

        public void Write(string subdirectory, string fileName)
        {
            Lincs_vcf_genomic_data_summary_readWriteOptions readWrite_options = new Lincs_vcf_genomic_data_summary_readWriteOptions(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Summary, readWrite_options);
        }


        public Lincs_vcf_genomic_data_summary_class Deep_copy()
        {
            Lincs_vcf_genomic_data_summary_class copy = (Lincs_vcf_genomic_data_summary_class)this.MemberwiseClone();
            int summary_length = this.Summary.Length;
            copy.Summary = new Lincs_vcf_genomic_data_summary_line_class[summary_length];
            for (int indexS=0; indexS<summary_length; indexS++)
            {
                copy.Summary[indexS] = this.Summary[indexS].Deep_copy();
            }
            return copy;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Lincs_cardiotoxic_variant_line_class
    {
        public Lincs_genomics_analysis_stage_enum Filter_stage { get; set; }
        public string Rs_identifier { get; set; }
        public string Gene_symbol { get; set; }
        public string Cell_line_rs_identifier { get; set; }
        public string Cell_line { get; set; }
        public int Data_quality_aq { get; set; }
        public float Count_value_for_summary { get; set; }
        public bool Cell_line_treated_with_at_least_one_clinical_effect_drug { get; set; }
        public bool Cell_line_shows_outlier_response { get; set; }
        public string Drug_with_outlier_response_in_this_cell_line { get; set; }
        public string Drug_target_symbol { get; set; }
        public Relation_of_gene_symbol_to_drug_enum Relation_of_gene_symbol_to_drug { get; set; }
        public string Cell_line_genotype { get; set; }
        public string Population_minor_allele { get; set; }
        public int Cell_line_minor_alleles_count { get; set; }
        public string Variant_allele_with_clinical_effect { get; set; }
        public string[] Drugs_with_clinical_effect_documentation { get; set; }
        public string P_values_string { get; set; }
        public float P_value { get; set; }
        public float Adj_p_value { get; set; }
        public string Multiple_hypothesis_correction_method { get; set; }
        public string Genotype_information { get; set; }
        public int Total_geneSymbols_count_for_indicated_drugs { get; set; }
        public int Total_rsIdentifiers_count_for_indicated_drugs { get; set; }
        public string[] Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count { get; set; }
        public Variant_cardiotoxic_effect_enum[] Effects { get; set; }
        public int Total_number_of_published_significant_rsIdentifiers_for_drugs_with_clinical_effect { get; set; }
        public int Total_number_of_published_significant_geneSymbols_for_drugs_with_clinical_effect  { get; set; }
        public string[] References { get; set; }
        public string SummaryName_for_drugs_with_clinical_effect_documentation { get; set; }
        public string ReadWrite_drugs_with_clinical_effect_documentation
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Drugs_with_clinical_effect_documentation, Lincs_cardiotoxic_variant_input_readOptions.ReadWRite_array_delimiter); }
            set { this.Drugs_with_clinical_effect_documentation = ReadWriteClass.Get_array_from_readLine<string>(value, Lincs_cardiotoxic_variant_input_readOptions.ReadWRite_array_delimiter); }
        }
        public string ReadWrite_rugs_referring_to_total_geneSymbols_and_rsIdentifiers_count
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count, Lincs_cardiotoxic_variant_input_readOptions.ReadWRite_array_delimiter); }
            set { this.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count = ReadWriteClass.Get_array_from_readLine<string>(value, Lincs_cardiotoxic_variant_input_readOptions.ReadWRite_array_delimiter); }
        }
        public string ReadWrite_references
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.References, Lincs_cardiotoxic_variant_input_readOptions.ReadWRite_array_delimiter); }
            set { this.References = ReadWriteClass.Get_array_from_readLine<string>(value, Lincs_cardiotoxic_variant_input_readOptions.ReadWRite_array_delimiter); }
        }
        public string ReadWrite_effects
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Effects, Lincs_cardiotoxic_variant_input_readOptions.ReadWRite_array_delimiter); }
            set 
            { 
                string[] effect_strings = ReadWriteClass.Get_array_from_readLine<string>(value, Lincs_cardiotoxic_variant_input_readOptions.ReadWRite_array_delimiter);
                string effect_string;
                int effect_strings_length = effect_strings.Length;
                this.Effects = new Variant_cardiotoxic_effect_enum[effect_strings_length];
                for (int indexE=0; indexE<effect_strings_length;indexE++)
                {
                    effect_string = effect_strings[indexE];
                    this.Effects[indexE] = (Variant_cardiotoxic_effect_enum)Enum.Parse(typeof(Variant_cardiotoxic_effect_enum), effect_string);
                }
            }
        }

        public static Lincs_cardiotoxic_variant_line_class[] Order_by_identity(Lincs_cardiotoxic_variant_line_class[] variants)
        {
            variants = variants.OrderBy(l => l.ReadWrite_effects).ThenBy(l=>l.ReadWrite_drugs_with_clinical_effect_documentation).ThenBy(l => l.Cell_line).ThenBy(l => l.Cell_line_genotype).ThenBy(l => l.Cell_line_rs_identifier).ThenBy(l => l.Data_quality_aq).ThenBy(l => l.Drug_with_outlier_response_in_this_cell_line).ThenBy(l => l.Gene_symbol).ThenBy(l => l.Relation_of_gene_symbol_to_drug).ThenBy(l => l.Rs_identifier).ThenBy(l => l.Variant_allele_with_clinical_effect).ToArray();
            return variants;
        }

        public bool Equal_identity(Lincs_cardiotoxic_variant_line_class other_line)
        {
            bool equal_identiy =    this.ReadWrite_effects.Equals(other_line.ReadWrite_effects)
                                 && this.ReadWrite_drugs_with_clinical_effect_documentation.Equals(other_line.ReadWrite_drugs_with_clinical_effect_documentation)
                                 && this.Filter_stage.Equals(other_line.Filter_stage)
                                 && this.Cell_line.Equals(other_line.Cell_line)
                                 && this.Cell_line_genotype.Equals(other_line.Cell_line_genotype)
                                 && this.Cell_line_rs_identifier.Equals(other_line.Cell_line_rs_identifier)
                                 && this.Data_quality_aq.Equals(other_line.Data_quality_aq)
                                 && this.Drug_with_outlier_response_in_this_cell_line.Equals(other_line.Drug_with_outlier_response_in_this_cell_line)
                                 && this.Gene_symbol.Equals(other_line.Gene_symbol)
                                 && this.Relation_of_gene_symbol_to_drug.Equals(other_line.Relation_of_gene_symbol_to_drug)
                                 && this.Rs_identifier.Equals(other_line.Rs_identifier)
                                 && this.Variant_allele_with_clinical_effect.Equals(other_line.Variant_allele_with_clinical_effect);
            return equal_identiy;
        }

        public Lincs_cardiotoxic_variant_line_class()
        {
            this.Cell_line_treated_with_at_least_one_clinical_effect_drug = false;
            this.Rs_identifier = "";
            this.Gene_symbol = "";
            this.Cell_line = "";
            this.Genotype_information = "";
            this.Cell_line_rs_identifier = "";
            this.Multiple_hypothesis_correction_method = "";
            this.Drug_with_outlier_response_in_this_cell_line = "";
            this.SummaryName_for_drugs_with_clinical_effect_documentation = "";
            this.Cell_line_genotype = "";
            this.Population_minor_allele = "";
            this.Variant_allele_with_clinical_effect = "";
            this.References = new string[0];
            this.Effects = new Variant_cardiotoxic_effect_enum[0];
            this.Drug_target_symbol = "";
            this.Drugs_with_clinical_effect_documentation = new string[0];
            this.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count = new string[0];
        }

        public Lincs_cardiotoxic_variant_line_class Deep_copy()
        {
            Lincs_cardiotoxic_variant_line_class copy = (Lincs_cardiotoxic_variant_line_class)this.MemberwiseClone();
            copy.Rs_identifier = (string)this.Rs_identifier.Clone();
            copy.Gene_symbol = (string)this.Gene_symbol.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            copy.SummaryName_for_drugs_with_clinical_effect_documentation = (string)this.SummaryName_for_drugs_with_clinical_effect_documentation.Clone();
            copy.Cell_line_genotype = (string)this.Cell_line_genotype.Clone();
            copy.Drug_target_symbol = (string)this.Drug_target_symbol.Clone();
            copy.Multiple_hypothesis_correction_method = (string)this.Multiple_hypothesis_correction_method.Clone();
            copy.Genotype_information = (string)this.Genotype_information.Clone();
            copy.Drug_with_outlier_response_in_this_cell_line = (string)this.Drug_with_outlier_response_in_this_cell_line.Clone();
            int references_length = this.References.Length;
            copy.References = new string[references_length];
            for (int indexRef = 0; indexRef < references_length; indexRef++)
            {
                copy.References[indexRef] = (string)this.References[indexRef].Clone();
            }
            int effects_length = this.Effects.Length;
            copy.Effects = new Variant_cardiotoxic_effect_enum[effects_length];
            for (int indexEffect = 0; indexEffect < effects_length; indexEffect++)
            {
                copy.Effects[indexEffect] = this.Effects[indexEffect];
            }
            int drugs_length = this.Drugs_with_clinical_effect_documentation.Length;
            copy.Drugs_with_clinical_effect_documentation = new string[drugs_length];
            for (int indexDrug = 0; indexDrug < drugs_length; indexDrug++)
            {
                copy.Drugs_with_clinical_effect_documentation[indexDrug] = (string)this.Drugs_with_clinical_effect_documentation[indexDrug].Clone();
            }
            int indicated_drugs_length = this.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count.Length;
            copy.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count = new string[indicated_drugs_length];
            for (int indexIndicatedDrug = 0; indexIndicatedDrug < indicated_drugs_length; indexIndicatedDrug++)
            {
                copy.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count[indexIndicatedDrug] = (string)this.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count[indexIndicatedDrug].Clone();
            }
            return copy;
        }
    }

    class Lincs_cardiotoxic_variant_input_readOptions : ReadWriteOptions_base
    {
        public static char ReadWRite_array_delimiter { get { return ';'; } }
        public Lincs_cardiotoxic_variant_input_readOptions()
        {
            this.File = Global_directory_class.Published_cardiotoxic_variants_directory + "RARG_variant.txt";
            this.Key_propertyNames = new string[] { "Rs_identifier", "Gene_symbol", "Variant_allele_with_clinical_effect","ReadWrite_drugs_with_clinical_effect_documentation", "ReadWrite_effects", "P_value", "Adj_p_value", "Multiple_hypothesis_correction_method", "ReadWrite_references" };
            this.Key_columnNames = this.Key_propertyNames;
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_cardiotoxic_variant_readWriteOptions : ReadWriteOptions_base
    {
        public static char ReadWRite_array_delimiter { get { return ';'; } }
        public Lincs_cardiotoxic_variant_readWriteOptions(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Filter_stage",
                                                    "Rs_identifier",
                                                    "Gene_symbol", 
                                                    "ReadWrite_drugs_with_clinical_effect_documentation",
                                                    "Cell_line_treated_with_at_least_one_clinical_effect_drug",
                                                    "ReadWrite_effects",
                                                    "Variant_allele_with_clinical_effect",
                                                    "Population_minor_allele",
                                                    "Cell_line_rs_identifier",
                                                    "P_value",
                                                    "Adj_p_value",
                                                    "P_values_string",
                                                    "Multiple_hypothesis_correction_method",
                                                    "Genotype_information",
                                                    "Cell_line",
                                                    "Cell_line_shows_outlier_response",
                                                    "Drug_target_symbol",
                                                    "Drug_with_outlier_response_in_this_cell_line",
                                                    "Total_rsIdentifiers_count_for_indicated_drugs","Total_geneSymbols_count_for_indicated_drugs",
                                                    "Total_number_of_published_significant_rsIdentifiers_for_drugs_with_clinical_effect","Total_number_of_published_significant_geneSymbols_for_drugs_with_clinical_effect",
                                                    "ReadWrite_rugs_referring_to_total_geneSymbols_and_rsIdentifiers_count",
                                                    "Relation_of_gene_symbol_to_drug",
                                                    "Cell_line_genotype",
                                                    "Cell_line_minor_alleles_count",
                                                    "Data_quality_aq",
                                                    "SummaryName_for_drugs_with_clinical_effect_documentation",
                                                    "Count_value_for_summary",
                                                    "ReadWrite_references" };

            this.Key_columnNames = this.Key_propertyNames;
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_cardiotoxic_variant_options_class
    {
        public float Maximum_pvalue { get; set; }

        public Lincs_cardiotoxic_variant_options_class()
        {
            Maximum_pvalue = 0.05F;
        }
    }

    class Lincs_cardiotoxic_variant_class
    {
        public Lincs_cardiotoxic_variant_line_class[] Cardiotoxic_variants { get; set; }
        public Lincs_cardiotoxic_variant_options_class Options { get; set; }

        public Lincs_cardiotoxic_variant_class()
        {
            this.Cardiotoxic_variants = new Lincs_cardiotoxic_variant_line_class[0];
            this.Options = new Lincs_cardiotoxic_variant_options_class();
        }

        private void Add_to_array(Lincs_cardiotoxic_variant_line_class[] add_cardiotoxic_variants)
        {
            int this_length = this.Cardiotoxic_variants.Length;
            int add_length = add_cardiotoxic_variants.Length;
            int new_length = this_length + add_length;
            Lincs_cardiotoxic_variant_line_class[] new_cardiotoxic_variants = new Lincs_cardiotoxic_variant_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_cardiotoxic_variants[indexNew] = this.Cardiotoxic_variants[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_cardiotoxic_variants[indexNew] = add_cardiotoxic_variants[indexAdd];
            }
            this.Cardiotoxic_variants = new_cardiotoxic_variants.ToArray();
        }

        private void Check_for_duplicates_basedon_rsIdentifier_and_geneSymbol()
        {
            Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>> rsIdentifier_geneSymbol_dict = new Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>();
            Lincs_cardiotoxic_variant_line_class duplicted_variant_line;
            foreach (Lincs_cardiotoxic_variant_line_class variant_line in Cardiotoxic_variants)
            {
                if (!rsIdentifier_geneSymbol_dict.ContainsKey(variant_line.Rs_identifier))
                {
                    rsIdentifier_geneSymbol_dict.Add(variant_line.Rs_identifier, new Dictionary<string, Lincs_cardiotoxic_variant_line_class>());
                }
                if (!rsIdentifier_geneSymbol_dict[variant_line.Rs_identifier].ContainsKey(variant_line.Gene_symbol))
                {
                    rsIdentifier_geneSymbol_dict[variant_line.Rs_identifier].Add(variant_line.Gene_symbol, variant_line);
                }
                else
                {
                    duplicted_variant_line = rsIdentifier_geneSymbol_dict[variant_line.Rs_identifier][variant_line.Gene_symbol];
                    throw new Exception();
                }
            }
        }

        private void Check_that_all_rs_labeled_rsIdentifiers_are_rs_plus_numbers()
        {
            long integer;
            string rs_number_string;
            foreach (Lincs_cardiotoxic_variant_line_class variant_line in this.Cardiotoxic_variants)
            {
                if (variant_line.Rs_identifier.IndexOf("rs")==0)
                {
                    rs_number_string = variant_line.Rs_identifier.Replace("rs", "");
                    if (!long.TryParse(rs_number_string,out integer))
                    {
                        throw new Exception();
                    }
                }
            }
        }

        public void Check_for_duplicates_after_population_with_genomics_results(Lincs_cardiotoxic_variant_line_class[] cardiotoxic_variants, string error_report_subdirectory)
        {
            Dictionary<string, Dictionary<Lincs_genomics_analysis_stage_enum, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>>>>>>>> drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict = new Dictionary<string, Dictionary<Lincs_genomics_analysis_stage_enum, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>>>>>>>>();
            Lincs_cardiotoxic_variant_line_class duplicted_variant_line;
            List<Lincs_cardiotoxic_variant_line_class> duplicated_lines = new List<Lincs_cardiotoxic_variant_line_class>();
            string cellLine_string;
            foreach (Lincs_cardiotoxic_variant_line_class variant_line in cardiotoxic_variants)
            {
                if (Lincs_genomics_drugBank_names_class.Does_analysis_stage_only_contain_outlier_celllines(variant_line.Filter_stage))
                { cellLine_string = ""; }
                else { cellLine_string = variant_line.Cell_line; }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict.ContainsKey(variant_line.Drug_target_symbol))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict.Add(variant_line.Drug_target_symbol, new Dictionary<Lincs_genomics_analysis_stage_enum, Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>>>>>>>());
                }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol].ContainsKey(variant_line.Filter_stage))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol].Add(variant_line.Filter_stage, new Dictionary<string, Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>>>>>>());
                }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage].ContainsKey(cellLine_string))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage].Add(cellLine_string, new Dictionary<Relation_of_gene_symbol_to_drug_enum, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>>>>>());
                }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string].ContainsKey(variant_line.Relation_of_gene_symbol_to_drug))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string].Add(variant_line.Relation_of_gene_symbol_to_drug, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>>>>());
                }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug].ContainsKey(variant_line.Cell_line_rs_identifier))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug].Add(variant_line.Cell_line_rs_identifier, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>>>());
                }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier].ContainsKey(variant_line.Drug_with_outlier_response_in_this_cell_line))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier].Add(variant_line.Drug_with_outlier_response_in_this_cell_line, new Dictionary<string, Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>>());
                }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier][variant_line.Drug_with_outlier_response_in_this_cell_line].ContainsKey(variant_line.Cell_line_genotype))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier][variant_line.Drug_with_outlier_response_in_this_cell_line].Add(variant_line.Cell_line_genotype, new Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class>>());
                }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier][variant_line.Drug_with_outlier_response_in_this_cell_line][variant_line.Cell_line_genotype].ContainsKey(variant_line.Rs_identifier))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier][variant_line.Drug_with_outlier_response_in_this_cell_line][variant_line.Cell_line_genotype].Add(variant_line.Rs_identifier, new Dictionary<string, Lincs_cardiotoxic_variant_line_class>());
                }
                if (!drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier][variant_line.Drug_with_outlier_response_in_this_cell_line][variant_line.Cell_line_genotype][variant_line.Rs_identifier].ContainsKey(variant_line.Gene_symbol))
                {
                    drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier][variant_line.Drug_with_outlier_response_in_this_cell_line][variant_line.Cell_line_genotype][variant_line.Rs_identifier].Add(variant_line.Gene_symbol, variant_line);
                }
                else
                {
                    duplicted_variant_line = drugTargetSymbol_stage_cellline_relation_celllineRsIdentifier_drug_celllineGenotype_rsIdentifier_geneSymbol_dict[variant_line.Drug_target_symbol][variant_line.Filter_stage][cellLine_string][variant_line.Relation_of_gene_symbol_to_drug][variant_line.Cell_line_rs_identifier][variant_line.Drug_with_outlier_response_in_this_cell_line][variant_line.Cell_line_genotype][variant_line.Rs_identifier][variant_line.Gene_symbol];
                    duplicated_lines.Add(duplicted_variant_line);
                    duplicated_lines.Add(variant_line);
                }
            }
            if (duplicated_lines.Count > 0)
            {
                Lincs_cardiotoxic_variant_readWriteOptions readWriteOptions = new Lincs_cardiotoxic_variant_readWriteOptions(error_report_subdirectory, "Duplicated_cardiotoxic_variants.txt");
                ReadWriteClass.WriteData(duplicated_lines.ToArray(), readWriteOptions);
                throw new Exception();
            }
        }

        public void Check_for_duplicates_after_population_with_genomics_results(string error_report_subdirectory)
        {
            Check_for_duplicates_after_population_with_genomics_results(this.Cardiotoxic_variants, error_report_subdirectory);
        }

        public void Set_count_values_for_summary()
        {
            this.Cardiotoxic_variants = this.Cardiotoxic_variants.OrderBy(l => l.Filter_stage).ThenBy(l => l.Gene_symbol).ThenBy(l => l.Rs_identifier).ThenBy(l => l.Cell_line).ThenBy(l => l.Drug_with_outlier_response_in_this_cell_line).ToArray();
            Lincs_cardiotoxic_variant_line_class cardiotoxic_variant_line;
            Lincs_cardiotoxic_variant_line_class inner_cardiotoxic_variant_line;
            Dictionary<Relation_of_gene_symbol_to_drug_enum, bool> current_relations_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, bool>();
            int firstIndex_currentGroup = -1;
            int cardiotoxic_variants_length = this.Cardiotoxic_variants.Length;
            float count_value;
            for (int indexCT=0; indexCT<cardiotoxic_variants_length; indexCT++)
            {
                cardiotoxic_variant_line = this.Cardiotoxic_variants[indexCT];
                if (  (indexCT==0)
                    || (cardiotoxic_variant_line.Filter_stage.Equals(this.Cardiotoxic_variants[indexCT-1].Filter_stage))
                    || (cardiotoxic_variant_line.Gene_symbol.Equals(this.Cardiotoxic_variants[indexCT-1].Gene_symbol))
                    || (cardiotoxic_variant_line.Rs_identifier.Equals(this.Cardiotoxic_variants[indexCT-1].Rs_identifier))
                    || (cardiotoxic_variant_line.Cell_line.Equals(this.Cardiotoxic_variants[indexCT-1].Cell_line))
                    || (cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line.Equals(this.Cardiotoxic_variants[indexCT-1].Drug_with_outlier_response_in_this_cell_line)))
                {
                    firstIndex_currentGroup = indexCT;
                    current_relations_dict.Clear();
                }
                current_relations_dict.Add(cardiotoxic_variant_line.Relation_of_gene_symbol_to_drug, true);
                if ((indexCT == cardiotoxic_variants_length-1)
                    || (cardiotoxic_variant_line.Filter_stage.Equals(this.Cardiotoxic_variants[indexCT+1].Filter_stage))
                    || (cardiotoxic_variant_line.Gene_symbol.Equals(this.Cardiotoxic_variants[indexCT + 1].Gene_symbol))
                    || (cardiotoxic_variant_line.Rs_identifier.Equals(this.Cardiotoxic_variants[indexCT + 1].Rs_identifier))
                    || (cardiotoxic_variant_line.Cell_line.Equals(this.Cardiotoxic_variants[indexCT + 1].Cell_line))
                    || (cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line.Equals(this.Cardiotoxic_variants[indexCT + 1].Drug_with_outlier_response_in_this_cell_line)))
                {
                    count_value = 1F / current_relations_dict.Keys.ToArray().Length;
                    for (int indexInner=firstIndex_currentGroup; indexInner<=indexCT;indexInner++)
                    {
                        inner_cardiotoxic_variant_line = this.Cardiotoxic_variants[indexInner];
                        inner_cardiotoxic_variant_line.Count_value_for_summary = count_value;
                    }
                }
            }
        }

        public void Set_cellline_treated_with_at_least_one_clinical_effect_drug(Deg_summary_class deg_summary)
        {
            Dictionary<string, Dictionary<string, bool>> cellline_drug_dict = deg_summary.Get_truncatedCellline_drug_dict();
            foreach (Lincs_cardiotoxic_variant_line_class cardiotoxic_variant_line in this.Cardiotoxic_variants)
            {
                cardiotoxic_variant_line.Cell_line_treated_with_at_least_one_clinical_effect_drug = false;
                if (cellline_drug_dict.ContainsKey(cardiotoxic_variant_line.Cell_line))
                { 
                    foreach (string drug in cardiotoxic_variant_line.Drugs_with_clinical_effect_documentation)
                    {
                        if (cellline_drug_dict[cardiotoxic_variant_line.Cell_line].ContainsKey(drug))
                        {
                            cardiotoxic_variant_line.Cell_line_treated_with_at_least_one_clinical_effect_drug = true;
                            break;
                        }
                    }
                }
            }
        }

        private void Remove_intergenic_geneSymbol()
        {
            int variants_length = this.Cardiotoxic_variants.Length;
            List<Lincs_cardiotoxic_variant_line_class> keep = new List<Lincs_cardiotoxic_variant_line_class>();
            Lincs_cardiotoxic_variant_line_class variant_line;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if (!variant_line.Gene_symbol.ToLower().Equals("intergenic"))
                {
                    keep.Add(variant_line);
                }
            }
            this.Cardiotoxic_variants = keep.ToArray();
        }

        private void Count_significant_rsIdentifiers_and_geneSymbols()
        {
            this.Cardiotoxic_variants = this.Cardiotoxic_variants.OrderBy(l => l.ReadWrite_drugs_with_clinical_effect_documentation).ToArray();
            Dictionary<string, bool> geneSymbol_dict = new Dictionary<string, bool>();
            Dictionary<string, bool> rsIdentifier_dict = new Dictionary<string, bool>();
            int cardiotoxic_variants_length = this.Cardiotoxic_variants.Length;
            Lincs_cardiotoxic_variant_line_class variant_line;
            Lincs_cardiotoxic_variant_line_class inner_variant_line;
            int firstIndex_sameClinicalDrugs = 0;
            int unique_geneSymbols_count;
            int unique_rsIdentifiers_count;
            for (int indexV = 0; indexV < cardiotoxic_variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if (  (indexV==0)
                    ||(!variant_line.ReadWrite_drugs_with_clinical_effect_documentation.Equals(this.Cardiotoxic_variants[indexV-1].ReadWrite_drugs_with_clinical_effect_documentation)))
                {
                    geneSymbol_dict.Clear();
                    rsIdentifier_dict.Clear();
                    firstIndex_sameClinicalDrugs = indexV;
                }
                if (variant_line.P_value <= Options.Maximum_pvalue)
                {
                    if (!geneSymbol_dict.ContainsKey(variant_line.Gene_symbol)) { geneSymbol_dict.Add(variant_line.Gene_symbol, true); }
                    if (!rsIdentifier_dict.ContainsKey(variant_line.Rs_identifier)) { rsIdentifier_dict.Add(variant_line.Rs_identifier, true); }
                }
                if ((indexV == cardiotoxic_variants_length-1)
                    || (!variant_line.ReadWrite_drugs_with_clinical_effect_documentation.Equals(this.Cardiotoxic_variants[indexV + 1].ReadWrite_drugs_with_clinical_effect_documentation)))
                {
                    unique_geneSymbols_count = geneSymbol_dict.Keys.ToArray().Length;
                    unique_rsIdentifiers_count = rsIdentifier_dict.Keys.ToArray().Length;
                    for (int indexInner = firstIndex_sameClinicalDrugs; indexInner <= indexV; indexInner++)
                    {
                        inner_variant_line = this.Cardiotoxic_variants[indexInner];
                        inner_variant_line.Total_number_of_published_significant_geneSymbols_for_drugs_with_clinical_effect = unique_geneSymbols_count;
                        inner_variant_line.Total_number_of_published_significant_rsIdentifiers_for_drugs_with_clinical_effect = unique_rsIdentifiers_count;
                    }
                }
            }
        }

        public void Set_simplied_variant_effect(float maximum_nominal_pvalue)
        {
            foreach (Lincs_cardiotoxic_variant_line_class genomic_line in this.Cardiotoxic_variants)
            {
                if (genomic_line.P_value>Options.Maximum_pvalue)
                {
                    genomic_line.Effects = new Variant_cardiotoxic_effect_enum[] { Variant_cardiotoxic_effect_enum.Variant_with_no_effect };
                }
                else
                {
                    genomic_line.Effects = new Variant_cardiotoxic_effect_enum[] { Variant_cardiotoxic_effect_enum.Variant_with_effect };
                }
            }
        }

        private Lincs_cardiotoxic_variant_line_class Generate_new_cardiotoxic_line_with_vcf_information(Lincs_cardiotoxic_variant_line_class cardiotoxic_variant_line, Lincs_vcf_genomic_data_line_class vcf_genomic_line, Lincs_genomics_analysis_stage_enum filter_stage, Dictionary<string, int> drugGroupOrSingleDrug_totalNumberRsIdentifiers_dict, Dictionary<string, int> drugGroupOrSingleDrug_totalNumberGeneSymbols_dict)
        {
            Lincs_cardiotoxic_variant_line_class final_cardiotoxic_variant_line = cardiotoxic_variant_line.Deep_copy();
            final_cardiotoxic_variant_line.Filter_stage = filter_stage;
            final_cardiotoxic_variant_line.Cell_line = (string)vcf_genomic_line.Cell_line.Clone();
            final_cardiotoxic_variant_line.Data_quality_aq = vcf_genomic_line.Quality_aq;
            final_cardiotoxic_variant_line.Cell_line_genotype = (string)vcf_genomic_line.Cell_line_genotype_GT.Clone();
            final_cardiotoxic_variant_line.Cell_line_rs_identifier = (string)vcf_genomic_line.Rs_identifier.Clone();
            final_cardiotoxic_variant_line.Population_minor_allele = (string)vcf_genomic_line.Minor_allele.Clone();
            final_cardiotoxic_variant_line.Cell_line_minor_alleles_count = vcf_genomic_line.Cell_line_minor_allele_count;
            final_cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line = (string)vcf_genomic_line.Cell_line_drug_with_outlier_response.Clone();
            final_cardiotoxic_variant_line.Drug_target_symbol = (string)vcf_genomic_line.Drug_target_symbol.Clone();
            final_cardiotoxic_variant_line.Relation_of_gene_symbol_to_drug = vcf_genomic_line.Relation_of_gene_symbol_to_drug;
            if ((drugGroupOrSingleDrug_totalNumberGeneSymbols_dict.ContainsKey(final_cardiotoxic_variant_line.ReadWrite_drugs_with_clinical_effect_documentation))
                && (final_cardiotoxic_variant_line.Drugs_with_clinical_effect_documentation.Contains(final_cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line)))
            {
                final_cardiotoxic_variant_line.Total_geneSymbols_count_for_indicated_drugs = drugGroupOrSingleDrug_totalNumberGeneSymbols_dict[final_cardiotoxic_variant_line.ReadWrite_drugs_with_clinical_effect_documentation];
                final_cardiotoxic_variant_line.Total_rsIdentifiers_count_for_indicated_drugs = drugGroupOrSingleDrug_totalNumberRsIdentifiers_dict[final_cardiotoxic_variant_line.ReadWrite_drugs_with_clinical_effect_documentation];
                final_cardiotoxic_variant_line.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count = Array_class.Deep_copy_array(final_cardiotoxic_variant_line.Drugs_with_clinical_effect_documentation);
            }
            else if (drugGroupOrSingleDrug_totalNumberGeneSymbols_dict.ContainsKey(final_cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line))
            {
                final_cardiotoxic_variant_line.Total_geneSymbols_count_for_indicated_drugs = drugGroupOrSingleDrug_totalNumberGeneSymbols_dict[final_cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line];
                final_cardiotoxic_variant_line.Total_rsIdentifiers_count_for_indicated_drugs = drugGroupOrSingleDrug_totalNumberRsIdentifiers_dict[final_cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line];
                final_cardiotoxic_variant_line.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count = new string[] { (string)final_cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line.Clone() };
            }
            else
            {
                final_cardiotoxic_variant_line.Total_geneSymbols_count_for_indicated_drugs = 0;
                final_cardiotoxic_variant_line.Total_rsIdentifiers_count_for_indicated_drugs = 0;
                final_cardiotoxic_variant_line.Drugs_referring_to_total_geneSymbols_and_rsIdentifiers_count = new string[] { (string)final_cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line.Clone() };
            }
            if (final_cardiotoxic_variant_line.Total_geneSymbols_count_for_indicated_drugs > final_cardiotoxic_variant_line.Total_rsIdentifiers_count_for_indicated_drugs) { throw new Exception(); }
            return final_cardiotoxic_variant_line;
        }

        private Lincs_cardiotoxic_variant_line_class[] Combine_carditoxic_variants_with_vcf_genomimcs_line_for_same_geneSymbol(Lincs_genomics_analysis_stage_enum filter_stage, Lincs_vcf_genomic_data_line_class[] vcf_genomic_lines, Lincs_cardiotoxic_variant_line_class[] cardiotoxic_lines, Dictionary<string, int> drugGroupOrSingleDrug_totalNumberRsIdentifiers_dict, Dictionary<string, int> drugGroupOrSingleDrug_totalNumberGeneSymbols_dict, string error_report_directory)
        {
            string geneSymbol = (string)cardiotoxic_lines[0].Gene_symbol.Clone();
            //Relation_of_gene_symbol_to_drug_enum relation = vcf_genomic_lines[0].Relation_of_gene_symbol_to_drug;
            vcf_genomic_lines = Lincs_vcf_genomic_data_line_class.Order_by_rsIdentifer(vcf_genomic_lines);
            cardiotoxic_lines = cardiotoxic_lines.OrderBy(l => l.Rs_identifier).ToArray();
            int cardiotoxic_lines_length = cardiotoxic_lines.Length;
            int vcf_genomic_lines_length = vcf_genomic_lines.Length;
            Lincs_vcf_genomic_data_line_class vcf_genomic_line;
            Lincs_cardiotoxic_variant_line_class cardiotoxic_variant_line;
            Lincs_cardiotoxic_variant_line_class final_cardiotoxic_variant_line;
            List<Lincs_cardiotoxic_variant_line_class> merged_matched_cardiotoxic_lines = new List<Lincs_cardiotoxic_variant_line_class>();
            List<Lincs_cardiotoxic_variant_line_class> merged_unmatched_cardiotoxic_lines = new List<Lincs_cardiotoxic_variant_line_class>();
            int indexVcfGenomic = 0;
            int stringCompare = -2;
            bool line_included;
            for (int indexCardiotoxic=0; indexCardiotoxic<cardiotoxic_lines_length; indexCardiotoxic++)
            {
                cardiotoxic_variant_line = cardiotoxic_lines[indexCardiotoxic];
                if (!cardiotoxic_variant_line.Gene_symbol.Equals(geneSymbol)) { throw new Exception(); }
                line_included = false;
                stringCompare = -2;
                while ((stringCompare<=0)&&(indexVcfGenomic<vcf_genomic_lines_length))
                {
                    vcf_genomic_line = vcf_genomic_lines[indexVcfGenomic];
                    if (!vcf_genomic_line.Gene_symbol.Equals(geneSymbol)) { throw new Exception(); }
                    stringCompare = vcf_genomic_line.Rs_identifier.CompareTo(cardiotoxic_variant_line.Rs_identifier);
                    if (stringCompare < 0)
                    {
                        final_cardiotoxic_variant_line = Generate_new_cardiotoxic_line_with_vcf_information(cardiotoxic_variant_line, vcf_genomic_line, filter_stage, drugGroupOrSingleDrug_totalNumberRsIdentifiers_dict, drugGroupOrSingleDrug_totalNumberGeneSymbols_dict);
                        merged_matched_cardiotoxic_lines.Add(final_cardiotoxic_variant_line);
                        line_included = true;
                        indexVcfGenomic++;
                    }
                    else if (stringCompare==0)
                    {
                        final_cardiotoxic_variant_line = Generate_new_cardiotoxic_line_with_vcf_information(cardiotoxic_variant_line, vcf_genomic_line, filter_stage, drugGroupOrSingleDrug_totalNumberRsIdentifiers_dict, drugGroupOrSingleDrug_totalNumberGeneSymbols_dict);
                        merged_matched_cardiotoxic_lines.Add(final_cardiotoxic_variant_line);
                        line_included = true;
                        indexVcfGenomic++;
                    }
                }
                if (!line_included) { throw new Exception(); }
                if (indexCardiotoxic == cardiotoxic_lines_length - 1)
                {
                    while (indexVcfGenomic < vcf_genomic_lines_length)
                    {
                        vcf_genomic_line = vcf_genomic_lines[indexVcfGenomic];
                        if (!vcf_genomic_line.Gene_symbol.Equals(geneSymbol)) { throw new Exception(); }
                        final_cardiotoxic_variant_line = Generate_new_cardiotoxic_line_with_vcf_information(cardiotoxic_variant_line, vcf_genomic_line, filter_stage, drugGroupOrSingleDrug_totalNumberRsIdentifiers_dict, drugGroupOrSingleDrug_totalNumberGeneSymbols_dict);
                        merged_matched_cardiotoxic_lines.Add(final_cardiotoxic_variant_line);
                        indexVcfGenomic++;
                    }
                }
            }
            if (merged_matched_cardiotoxic_lines.Count!=vcf_genomic_lines_length) { throw new Exception(); }
            Check_for_duplicates_after_population_with_genomics_results(merged_matched_cardiotoxic_lines.ToArray(), error_report_directory);
            merged_matched_cardiotoxic_lines.AddRange(merged_unmatched_cardiotoxic_lines);
            return merged_matched_cardiotoxic_lines.ToArray();
        }

        private string[][] Get_all_grouped_drugs()
        {
            Dictionary<string, bool> readWriteDrugs_dict = new Dictionary<string, bool>();
            List<string[]> grouped_drugs = new List<string[]>();
            foreach (Lincs_cardiotoxic_variant_line_class variant_line in this.Cardiotoxic_variants)
            {
                if (!readWriteDrugs_dict.ContainsKey(variant_line.ReadWrite_drugs_with_clinical_effect_documentation))
                {
                    readWriteDrugs_dict.Add(variant_line.ReadWrite_drugs_with_clinical_effect_documentation, true);
                    grouped_drugs.Add(variant_line.Drugs_with_clinical_effect_documentation);
                }
            }
            return grouped_drugs.ToArray();
        }

        public void Add_and_override_cellline_specific_variants(Lincs_genomics_analysis_stage_enum filter_stage, Lincs_vcf_genomic_data_class genomic_data, string error_report_subdirectory)
        {
            //string[][] grouped_drugs = Get_all_grouped_drugs();
            Dictionary<string, int> drugGroupOrSingleDrug_geneSymbolCount_dict = genomic_data.Generate_drugGroupOrSingleDrug_geneSymbolCount_dictionary();
            Dictionary<string, int> drugGroupOrSingleDrug_rsIdentifierCount_dict = genomic_data.Generate_drugGroupOrSingleDrug_rsIdentifierCount_dictionary();
            genomic_data.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_geneSymbol_rsIdentifier(genomic_data.Genomic_data);
            int genomic_data_length = genomic_data.Genomic_data.Length;
            int cardiotoxic_variants_length = Cardiotoxic_variants.Length;
            this.Cardiotoxic_variants = this.Cardiotoxic_variants.OrderBy(l => l.Gene_symbol).ThenBy(l => l.Rs_identifier).ToArray();
            Lincs_cardiotoxic_variant_line_class cardiotoxic_variant_line;
            List<Lincs_cardiotoxic_variant_line_class> final_cardiotoxic_variant_lines = new List<Lincs_cardiotoxic_variant_line_class>();
            Lincs_cardiotoxic_variant_line_class[] add_cardiotoxic_variants;
            Lincs_vcf_genomic_data_line_class vcf_genomic_data_line;
            int indexGenomicData = 0;
            int geneSymbolStringCompare;
            List<Lincs_cardiotoxic_variant_line_class> sameGeneSymbol_cardiotoxic_variants = new List<Lincs_cardiotoxic_variant_line_class>();
            List<Lincs_vcf_genomic_data_line_class> sameGeneSymbol_genomic_data = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexCardioTox = 0; indexCardioTox < cardiotoxic_variants_length; indexCardioTox++)
            {
                cardiotoxic_variant_line = this.Cardiotoxic_variants[indexCardioTox];
                if ((indexCardioTox == 0)
                    || (!cardiotoxic_variant_line.Gene_symbol.Equals(this.Cardiotoxic_variants[indexCardioTox - 1].Gene_symbol)))
                {
                    sameGeneSymbol_cardiotoxic_variants.Clear();
                    sameGeneSymbol_genomic_data.Clear();
                }
                sameGeneSymbol_cardiotoxic_variants.Add(cardiotoxic_variant_line);
                if ((indexCardioTox == cardiotoxic_variants_length - 1)
                    || (!cardiotoxic_variant_line.Gene_symbol.Equals(this.Cardiotoxic_variants[indexCardioTox + 1].Gene_symbol)))
                {
                    geneSymbolStringCompare = -2;
                    while ((geneSymbolStringCompare <= 0) && (indexGenomicData < genomic_data_length))
                    {
                        vcf_genomic_data_line = genomic_data.Genomic_data[indexGenomicData];
                        geneSymbolStringCompare = vcf_genomic_data_line.Gene_symbol.CompareTo(cardiotoxic_variant_line.Gene_symbol);
                        if (geneSymbolStringCompare < 0)
                        {
                            indexGenomicData++;
                        }
                        else if (geneSymbolStringCompare == 0)
                        {
                            sameGeneSymbol_genomic_data.Add(vcf_genomic_data_line);
                            indexGenomicData++;
                        }
                    }
                    add_cardiotoxic_variants = Combine_carditoxic_variants_with_vcf_genomimcs_line_for_same_geneSymbol(filter_stage, sameGeneSymbol_genomic_data.ToArray(), sameGeneSymbol_cardiotoxic_variants.ToArray(), drugGroupOrSingleDrug_rsIdentifierCount_dict, drugGroupOrSingleDrug_geneSymbolCount_dict, error_report_subdirectory);
                    final_cardiotoxic_variant_lines.AddRange(add_cardiotoxic_variants);
                }
            }
            this.Cardiotoxic_variants = final_cardiotoxic_variant_lines.ToArray();
            Check_for_duplicates_after_population_with_genomics_results(error_report_subdirectory);
        }

        public string[] Get_all_geneSymbols()
        {
            Dictionary<string, bool> geneSymbol_dict = new Dictionary<string, bool>();
            foreach (Lincs_cardiotoxic_variant_line_class cardiotoxic_line in this.Cardiotoxic_variants)
            {
                if (!geneSymbol_dict.ContainsKey(cardiotoxic_line.Gene_symbol))
                {
                    geneSymbol_dict.Add(cardiotoxic_line.Gene_symbol, true);
                }
            }
            return geneSymbol_dict.Keys.OrderBy(l => l).ToArray();
        }

        public Relation_of_gene_symbol_to_drug_enum[] Get_all_relationships_and_add_corresponding_relationship_of_direct_drug_interactor()
        {
            Dictionary<Relation_of_gene_symbol_to_drug_enum, bool> relationShip_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, bool>();
            Relation_of_gene_symbol_to_drug_enum corresponding_relation_for_direct_drug_interactor;
            foreach (Lincs_cardiotoxic_variant_line_class cardiotoxic_line in this.Cardiotoxic_variants)
            {
                corresponding_relation_for_direct_drug_interactor = Lincs_genomics_drugBank_names_class.Get_corresponding_relationship_for_direct_interactor_of_gene(cardiotoxic_line.Relation_of_gene_symbol_to_drug);
                if (!relationShip_dict.ContainsKey(cardiotoxic_line.Relation_of_gene_symbol_to_drug))
                {
                    relationShip_dict.Add(cardiotoxic_line.Relation_of_gene_symbol_to_drug, true);
                }
                if (!relationShip_dict.ContainsKey(corresponding_relation_for_direct_drug_interactor))
                {
                    relationShip_dict.Add(corresponding_relation_for_direct_drug_interactor, true);
                }
            }
            return relationShip_dict.Keys.OrderBy(l => l).ToArray();
        }

        public Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class[]>> Get_drug_geneSymbol_cardioToxicityLines_dictionary()
        {
            Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class[]>> drug_geneSymbol_rsIdentifierEffect_dictionary = new Dictionary<string, Dictionary<string, Lincs_cardiotoxic_variant_line_class[]>>();
            int variants_length = this.Cardiotoxic_variants.Length;
            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> variant_lines = new List<Lincs_cardiotoxic_variant_line_class>();
            List<string> rsIdenfier_effects = new List<string>();
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                foreach (string drug in variant_line.Drugs_with_clinical_effect_documentation)
                {
                    if (!drug_geneSymbol_rsIdentifierEffect_dictionary.ContainsKey(drug))
                    {
                        drug_geneSymbol_rsIdentifierEffect_dictionary.Add(drug, new Dictionary<string, Lincs_cardiotoxic_variant_line_class[]>());
                    }
                    if (!drug_geneSymbol_rsIdentifierEffect_dictionary[drug].ContainsKey(variant_line.Gene_symbol))
                    {
                        drug_geneSymbol_rsIdentifierEffect_dictionary[drug].Add(variant_line.Gene_symbol, new Lincs_cardiotoxic_variant_line_class[0]);
                    }
                    variant_lines.Clear();
                    variant_lines.AddRange(drug_geneSymbol_rsIdentifierEffect_dictionary[drug][variant_line.Gene_symbol]);
                    variant_lines.Add(variant_line);
                    drug_geneSymbol_rsIdentifierEffect_dictionary[drug][variant_line.Gene_symbol] = variant_lines.ToArray();
                }
            }
            return drug_geneSymbol_rsIdentifierEffect_dictionary;
        }

        private void Split_gene_symbols_and_add_as_new_line()
        {
            Lincs_cardiotoxic_variant_line_class variant_line;
            Lincs_cardiotoxic_variant_line_class new_variant_line;
            List<Lincs_cardiotoxic_variant_line_class> new_variant_lines = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            string[] splitStrings;
            char[] potential_delimiters = new char[] { '_', ' ', ',' };
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            List<string> new_geneSymbols = new List<string>();
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                geneSymbols = new string[] { variant_line.Gene_symbol };
                foreach (char potential_delimiter in potential_delimiters)
                {
                    new_geneSymbols.Clear();
                    geneSymbols_length = geneSymbols.Length;
                    for (int indexGS=0; indexGS<geneSymbols_length; indexGS++)
                    {
                        geneSymbol = geneSymbols[indexGS];
                        splitStrings = geneSymbol.Split(potential_delimiter);
                        new_geneSymbols.AddRange(splitStrings);
                    }
                    geneSymbols = new_geneSymbols.Distinct().ToArray();
                }
                variant_line.Gene_symbol = (string)geneSymbols[0].Clone();
                new_variant_lines.Add(variant_line);
                geneSymbols_length = geneSymbols.Length;
                for (int indexGeneSymbol = 1; indexGeneSymbol < geneSymbols_length; indexGeneSymbol++)
                {
                    geneSymbol = geneSymbols[indexGeneSymbol];
                    new_variant_line = variant_line.Deep_copy();
                    new_variant_line.Gene_symbol = (string)geneSymbol.Clone();
                    new_variant_lines.Add(new_variant_line);
                }
            }
            this.Cardiotoxic_variants = new_variant_lines.ToArray();
        }

        private void Merge_all_lines_with_same_rs_and_geneSymbol()
        {
            this.Cardiotoxic_variants = this.Cardiotoxic_variants.OrderBy(l => l.Gene_symbol).ThenBy(l => l.Rs_identifier).ToArray();
            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> merged = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            List<string> current_references = new List<string>();
            List<Variant_cardiotoxic_effect_enum> current_effects = new List<Variant_cardiotoxic_effect_enum>();
            List<string> current_drugs = new List<string>();
            float final_adj_pvalue = -1;
            List<float> pvalues = new List<float>();
            StringBuilder sb = new StringBuilder();
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if (  (indexV==0)
                    || (!variant_line.Gene_symbol.Equals(this.Cardiotoxic_variants[indexV - 1].Gene_symbol))
                    || (!variant_line.Rs_identifier.Equals(this.Cardiotoxic_variants[indexV - 1].Rs_identifier)))
                {
                    current_references.Clear();
                    current_effects.Clear();
                    current_drugs.Clear();
                    pvalues.Clear();
                    final_adj_pvalue = -1;
                }
                current_references.AddRange(variant_line.References);
                current_effects.AddRange(variant_line.Effects);
                current_drugs.AddRange(variant_line.Drugs_with_clinical_effect_documentation);
                pvalues.Add(variant_line.P_value);
                if ((final_adj_pvalue == -1)
                    || ((variant_line.Adj_p_value < final_adj_pvalue)
                       && (variant_line.Adj_p_value >= 0)))
                { final_adj_pvalue = variant_line.Adj_p_value; }
                if ((indexV == variants_length-1)
                    || (!variant_line.Gene_symbol.Equals(this.Cardiotoxic_variants[indexV + 1].Gene_symbol))
                    || (!variant_line.Rs_identifier.Equals(this.Cardiotoxic_variants[indexV + 1].Rs_identifier)))
                {
                    pvalues = pvalues.OrderBy(l => l).ToList();
                    sb.Clear();
                    foreach (float pvalue in pvalues)
                    {
                        if (sb.Length>0) { sb.AppendFormat(";"); }
                        sb.AppendFormat("{0}", pvalue);
                    }
                    variant_line.P_values_string = sb.ToString();
                    variant_line.P_value = pvalues[0];
                    variant_line.Adj_p_value = final_adj_pvalue;
                    variant_line.Effects = current_effects.Distinct().OrderBy(l => l).ToArray();
                    variant_line.References = current_references.Distinct().OrderBy(l => l).ToArray();
                    variant_line.Drugs_with_clinical_effect_documentation = current_drugs.Distinct().OrderBy(l => l).ToArray();
                    merged.Add(variant_line);
                }
            }
            this.Cardiotoxic_variants = merged.ToArray();
        }

        private void Keep_only_variants_with_maximum_pvalue()
        {
            int variants_length = this.Cardiotoxic_variants.Length;
            List<Lincs_cardiotoxic_variant_line_class> keep = new List<Lincs_cardiotoxic_variant_line_class>();
            Lincs_cardiotoxic_variant_line_class variant_line;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if (variant_line.P_value <= Options.Maximum_pvalue)
                {
                    keep.Add(variant_line);
                }
            }
            this.Cardiotoxic_variants = keep.ToArray();
        }

        private void Set_summaryName_for_drugs_with_clinical_effect_documentation()
        {
            foreach (Lincs_cardiotoxic_variant_line_class cardiotoxic_variant_line in this.Cardiotoxic_variants)
            {
                switch (cardiotoxic_variant_line.ReadWrite_drugs_with_clinical_effect_documentation)
                {
                    case "DAU;DOX;EPI;IDA":
                        cardiotoxic_variant_line.SummaryName_for_drugs_with_clinical_effect_documentation = "Anthracyclines";
                        break;
                    default:
                        cardiotoxic_variant_line.SummaryName_for_drugs_with_clinical_effect_documentation = (string)cardiotoxic_variant_line.ReadWrite_drugs_with_clinical_effect_documentation.Clone();
                        break;
                }
            }
        }
        public void Generate_by_reading_input()
        {
            Read_input_data();
            Set_summaryName_for_drugs_with_clinical_effect_documentation();
            Remove_intergenic_geneSymbol();
            Split_gene_symbols_and_add_as_new_line();
            Merge_all_lines_with_same_rs_and_geneSymbol();
            Count_significant_rsIdentifiers_and_geneSymbols();
            Keep_only_variants_with_maximum_pvalue();
            Check_for_duplicates_basedon_rsIdentifier_and_geneSymbol();
            Check_that_all_rs_labeled_rsIdentifiers_are_rs_plus_numbers();
        }

        public DE_class Generate_de_instance()
        {
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> merged = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Symbols_for_de = new string[] { (string)variant_line.Gene_symbol.Clone() };
                fill_de_line.Names_for_de = new string[] { variant_line.SummaryName_for_drugs_with_clinical_effect_documentation };
                fill_de_line.Value_for_de = 1;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Various;
                fill_de_list.Add(fill_de_line);
            }
            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list.ToArray());
            return de;
        }

        public DE_class Generate_cellline_and_outlierDrug_de_instance()
        {
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> merged = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Symbols_for_de = new string[] { (string)variant_line.Gene_symbol.Clone() };
                fill_de_line.Names_for_de = new string[] { variant_line.Drug_with_outlier_response_in_this_cell_line, variant_line.Cell_line };
                fill_de_line.Value_for_de = 1;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Various;
                fill_de_list.Add(fill_de_line);
            }
            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list.ToArray());
            return de;
        }

        private Dictionary<string, string> Get_geneSymbol_geneSymbolPublishedVariant_dictionary()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, string> drug_drugFullName_dict = drug_legend.Get_drug_drugFullName_dict();

            Dictionary<string, string> geneSymbol_geneSymbolVariant_dict = new Dictionary<string, string>();
            int variants_length = this.Cardiotoxic_variants.Length;
            Lincs_cardiotoxic_variant_line_class variant_line;
            this.Cardiotoxic_variants = this.Cardiotoxic_variants.OrderBy(l => l.Gene_symbol).ToArray();
            List<string> sameGeneSymbol_variants_list = new List<string>();
            int sameGeneSymbol_variants_list_count;
            string variant;
            StringBuilder sb = new StringBuilder();
            string clinical_effect_drugName;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if (  (indexV==0)
                    ||(!variant_line.Gene_symbol.Equals(this.Cardiotoxic_variants[indexV-1].Gene_symbol)))
                {
                    sameGeneSymbol_variants_list.Clear();
                }
                clinical_effect_drugName = variant_line.SummaryName_for_drugs_with_clinical_effect_documentation;
                if (drug_drugFullName_dict.ContainsKey(clinical_effect_drugName))
                {
                    clinical_effect_drugName = drug_drugFullName_dict[clinical_effect_drugName];
                }
                sameGeneSymbol_variants_list.Add(variant_line.Rs_identifier + " for " + clinical_effect_drugName);
                if ((indexV == variants_length-1)
                    || (!variant_line.Gene_symbol.Equals(this.Cardiotoxic_variants[indexV+1].Gene_symbol)))
                {
                    sameGeneSymbol_variants_list = sameGeneSymbol_variants_list.Distinct().OrderBy(l => l).ToList();
                    sameGeneSymbol_variants_list_count = sameGeneSymbol_variants_list.Count;
                    sb.Clear();
                    sb.AppendFormat("{0}", variant_line.Gene_symbol);
                    if (sameGeneSymbol_variants_list_count > 0)
                    {
                        sb.AppendFormat("\n(");
                        for (int indexSGV = 0; indexSGV < sameGeneSymbol_variants_list_count; indexSGV++)
                        {
                            variant = sameGeneSymbol_variants_list[indexSGV];
                            if (indexSGV>0) { sb.AppendFormat(","); }
                            sb.AppendFormat("{0}", variant);
                        }
                        sb.AppendFormat(")");
                    }
                    geneSymbol_geneSymbolVariant_dict.Add(variant_line.Gene_symbol, sb.ToString());
                }
            }
            return geneSymbol_geneSymbolVariant_dict;
        }

        public void Keep_only_variants_that_are_identical_in_cellline_and_published()
        {
            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> keep = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if (variant_line.Rs_identifier.Equals(variant_line.Cell_line_rs_identifier))
                {
                    keep.Add(variant_line);
                }
            }
            this.Cardiotoxic_variants = keep.ToArray();
        }

        public void Keep_only_selected_population_rsIdentifiers(string[] selected_population_rsIdentifiers)
        {
            Dictionary<string, bool> selected_variants_dict = new Dictionary<string, bool>();
            foreach (string selected_variant in selected_population_rsIdentifiers)
            {
                selected_variants_dict.Add(selected_variant, true);
            }

            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> keep = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if (selected_variants_dict.ContainsKey(variant_line.Rs_identifier))
                {
                    keep.Add(variant_line);
                }
            }
            this.Cardiotoxic_variants = keep.ToArray();
        }

        public void Keep_only_variants_that_are_related_to_at_least_one_indicated_clinical_risks_drugs(string[] drugs)
        {
            Dictionary<string, bool> keep_drug = new Dictionary<string, bool>();
            foreach (string drug in drugs)
            {
                keep_drug.Add(drug, true);
            }
            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> keep = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                foreach (string clinical_drug in variant_line.Drugs_with_clinical_effect_documentation)
                {
                    if (keep_drug.ContainsKey(clinical_drug))
                    {
                        keep.Add(variant_line);
                        break;
                    }
                }
            }
            this.Cardiotoxic_variants = keep.ToArray();
        }

        public NetworkBasis_class Generate_network_connecting_drugs_to_identified_variants()
        {
            Relation_of_gene_symbol_to_drug_enum[] existing_relationships = Get_all_relationships_and_add_corresponding_relationship_of_direct_drug_interactor();

            Dictionary<string, string> geneSymbol_geneSymbolPublishedVariant_dict = Get_geneSymbol_geneSymbolPublishedVariant_dictionary();
            string[] drugTargetProteins;
            string geneSymbol_name;
            string drugTargetProtein_name;
            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> keep = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            SigNWBasis_line_class sigNW_line;
            List<SigNWBasis_line_class> sigNW_list = new List<SigNWBasis_line_class>();
            Dictionary<string, List<System.Drawing.Color>> nw_node_color_dict = new Dictionary<string, List<System.Drawing.Color>>();
            System.Drawing.Color current_color = new System.Drawing.Color();
            bool add_color_for_geneSymbol_name = false;
            Dictionary<string, Dictionary<string, bool>> drug_consideredDrugTargetProteins_dict = new Dictionary<string, Dictionary<string, bool>>();

            Deg_drug_legend_class drugLegend = new Deg_drug_legend_class();
            drugLegend.Generate_de_novo();
            Dictionary<string, string> drug_drugFullName_dict = drugLegend.Get_drug_drugFullName_dict();
            Dictionary<string, Drug_type_enum> drug_drugType_dict = drugLegend.Get_drug_drugType_dictionary();
            Dictionary<Relation_of_gene_symbol_to_drug_enum, System.Drawing.Color> relation_color_dict = Lincs_genomics_drugBank_names_class.Get_relation_color_dict();
            string drugFullName;
            string drugFullName_withDrugType;
            for (int indexV = 0; indexV < variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                drugFullName = drug_drugFullName_dict[variant_line.Drug_with_outlier_response_in_this_cell_line];
                drugFullName_withDrugType = drugFullName + "\n(" + drug_drugType_dict[variant_line.Drug_with_outlier_response_in_this_cell_line].ToString().Replace("_"," " ) + ")";
                if (!nw_node_color_dict.ContainsKey(drugFullName_withDrugType))
                {
                    nw_node_color_dict.Add(drugFullName_withDrugType, new List<System.Drawing.Color>());
                    nw_node_color_dict[drugFullName_withDrugType].Add(System.Drawing.Color.Black);
                }
                geneSymbol_name = variant_line.Gene_symbol;
                if (geneSymbol_geneSymbolPublishedVariant_dict.ContainsKey(geneSymbol_name))
                {
                    geneSymbol_name = geneSymbol_geneSymbolPublishedVariant_dict[geneSymbol_name];
                }
                switch (variant_line.Relation_of_gene_symbol_to_drug)
                {
                    case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                    case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                    case Relation_of_gene_symbol_to_drug_enum.Transporter:
                        add_color_for_geneSymbol_name = false;
                        break;
                    case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                    case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                    case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                    case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                    case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                    case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                        add_color_for_geneSymbol_name = true;
                        break;
                    default:
                        throw new Exception();

                }
                if (add_color_for_geneSymbol_name)
                {
                    if (!nw_node_color_dict.ContainsKey(geneSymbol_name))
                    {
                        nw_node_color_dict.Add(geneSymbol_name, new List<System.Drawing.Color>());
                    }
                    current_color = relation_color_dict[variant_line.Relation_of_gene_symbol_to_drug];
                    nw_node_color_dict[geneSymbol_name].Add(current_color);
                    nw_node_color_dict[geneSymbol_name] = nw_node_color_dict[geneSymbol_name].Distinct().ToList();
                }
                //geneSymbol_color_dict[geneSymbol_name] = geneSymbol_color_dict[geneSymbol_name].Distinct().ToList();
                drugTargetProteins = variant_line.Drug_target_symbol.Split(';');
                foreach (string drugTargetProtein in drugTargetProteins)
                {
                    drugTargetProtein_name = drugTargetProtein;
                    if (geneSymbol_geneSymbolPublishedVariant_dict.ContainsKey(drugTargetProtein_name))
                    {
                        drugTargetProtein_name = geneSymbol_geneSymbolPublishedVariant_dict[drugTargetProtein_name];
                    }
                    if (!variant_line.Gene_symbol.Equals(variant_line.Drug_target_symbol))
                    {
                        sigNW_line = new SigNWBasis_line_class();
                        sigNW_line.Source = (string)geneSymbol_name.Clone();
                        sigNW_line.Target = (string)drugTargetProtein_name.Clone();
                        sigNW_list.Add(sigNW_line);
                    }
                    sigNW_line = new SigNWBasis_line_class();
                    switch (variant_line.Relation_of_gene_symbol_to_drug)
                    {
                        case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                        case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                        case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                            sigNW_line.Target = (string)drugTargetProtein_name.Clone();
                            sigNW_line.Source = (string)drugFullName_withDrugType.Clone();
                            current_color = relation_color_dict[Relation_of_gene_symbol_to_drug_enum.Drug_target_protein];
                            break;
                        case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                        case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                        case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                            sigNW_line.Source = (string)drugTargetProtein_name.Clone();
                            sigNW_line.Target = (string)drugFullName_withDrugType.Clone();
                            current_color = relation_color_dict[Relation_of_gene_symbol_to_drug_enum.Enzyme];
                            break;
                        case Relation_of_gene_symbol_to_drug_enum.Transporter:
                        case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                        case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                            sigNW_line.Source = (string)drugTargetProtein_name.Clone();
                            sigNW_line.Target = (string)drugFullName_withDrugType.Clone();
                            current_color = relation_color_dict[Relation_of_gene_symbol_to_drug_enum.Transporter];
                            break;
                        default:
                            throw new Exception();
                    }
                    sigNW_list.Add(sigNW_line);
                    if (!nw_node_color_dict.ContainsKey(drugTargetProtein_name))
                    {
                        nw_node_color_dict.Add(drugTargetProtein_name, new List<System.Drawing.Color>());
                    }
                    if (!drug_consideredDrugTargetProteins_dict.ContainsKey(drugFullName))
                    {
                        drug_consideredDrugTargetProteins_dict.Add(drugFullName, new Dictionary<string, bool>());
                    }
                    if (!drug_consideredDrugTargetProteins_dict[drugFullName].ContainsKey(drugTargetProtein_name))
                    {
                        nw_node_color_dict[drugTargetProtein_name].Add(current_color);
                        nw_node_color_dict[drugTargetProtein_name] = nw_node_color_dict[drugTargetProtein_name].Distinct().ToList();
                        drug_consideredDrugTargetProteins_dict[drugFullName].Add(drugTargetProtein_name, true);
                    }
                }
            }
            List<SigNWBasis_line_class> keep_sigNW = new List<SigNWBasis_line_class>();
            sigNW_list = sigNW_list.OrderBy(l => l.Source).ThenBy(l => l.Target).ToList();
            int sigNW_list_count = sigNW_list.Count;
            for (int indexSig=0; indexSig<sigNW_list_count;indexSig++)
            {
                sigNW_line = sigNW_list[indexSig];
                if (  (indexSig==0)
                    || (!sigNW_line.Source.Equals(sigNW_list[indexSig - 1].Source))
                    || (!sigNW_line.Target.Equals(sigNW_list[indexSig - 1].Target)))
                {
                    keep_sigNW.Add(sigNW_line);
                }
            }


            SigNWBasis_class<SigNWBasis_line_class> sigNW = new SigNWBasis_class<SigNWBasis_line_class>();
            sigNW.Generate_sigNW_from_sigNW_list(keep_sigNW, Network_direction_type_enum.Directed_forward);

            NetworkBasis_class nw = new NetworkBasis_class();
            nw.Generate_network_from_sigNW(sigNW);

            string legend_relation;
            foreach (Relation_of_gene_symbol_to_drug_enum relation in existing_relationships)
            {
                legend_relation = relation.ToString().Replace("_", " ");
                nw.Add_nodes(legend_relation);
                nw.UniqueNodes.Add_selected_color_to_nodes(new string[] { legend_relation }, relation_color_dict[relation]);
            }
            nw.UniqueNodes.Add_selected_color_to_nodes(nw_node_color_dict);
            nw.UniqueNodes.Add_color_to_all_nodes_with_no_colors(System.Drawing.Color.White);
            return nw;
        }


        public void Keep_only_selected_relationships_of_gene_symbol_to_drugTarget(Relation_of_gene_symbol_to_drug_enum[] keep_relations)
        {
            Dictionary<Relation_of_gene_symbol_to_drug_enum, bool> keep_relations_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, bool>();
            foreach (Relation_of_gene_symbol_to_drug_enum keep_relation in keep_relations)
            {
                keep_relations_dict.Add(keep_relation, true);
            }

            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> keep = new List<Lincs_cardiotoxic_variant_line_class>();
            List<Relation_of_gene_symbol_to_drug_enum> removed_relations = new List<Relation_of_gene_symbol_to_drug_enum>();
            List<Relation_of_gene_symbol_to_drug_enum> kept_relations = new List<Relation_of_gene_symbol_to_drug_enum>();
            int variants_length = this.Cardiotoxic_variants.Length;
            for (int indexV=0; indexV<variants_length; indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if (keep_relations_dict.ContainsKey(variant_line.Relation_of_gene_symbol_to_drug))
                {
                    keep.Add(variant_line);
                    kept_relations.Add(variant_line.Relation_of_gene_symbol_to_drug);
                }
                else
                {
                    removed_relations.Add(variant_line.Relation_of_gene_symbol_to_drug);
                }
            }
            removed_relations = removed_relations.Distinct().ToList();
            kept_relations = kept_relations.Distinct().ToList();
            this.Cardiotoxic_variants = keep.ToArray();
        }

        private void Read_input_data()
        {
            Lincs_cardiotoxic_variant_input_readOptions readOptions = new Lincs_cardiotoxic_variant_input_readOptions();
            this.Cardiotoxic_variants = ReadWriteClass.ReadRawData_and_FillArray<Lincs_cardiotoxic_variant_line_class>(readOptions);
        }

        private void Remove_duplicates()
        {
            this.Cardiotoxic_variants = Lincs_cardiotoxic_variant_line_class.Order_by_identity(this.Cardiotoxic_variants);
            Lincs_cardiotoxic_variant_line_class variant_line;
            List<Lincs_cardiotoxic_variant_line_class> keep = new List<Lincs_cardiotoxic_variant_line_class>();
            int variants_length = this.Cardiotoxic_variants.Length;
            for (int indexV=0; indexV<variants_length;indexV++)
            {
                variant_line = this.Cardiotoxic_variants[indexV];
                if ((indexV==0)||(!variant_line.Equals(this.Cardiotoxic_variants[indexV-1])))
                {
                    keep.Add(variant_line);
                }
            }
            this.Cardiotoxic_variants = keep.ToArray();
        }

        public void Erase_data_with_too_low_data_quality_and_remove_duplicates(int minimum_data_aq)
        { 
            foreach (Lincs_cardiotoxic_variant_line_class cardiotoxic_variant_line in this.Cardiotoxic_variants)
            {
                if (cardiotoxic_variant_line.Data_quality_aq< minimum_data_aq)
                {
                    cardiotoxic_variant_line.Cell_line_genotype = "";
                    cardiotoxic_variant_line.Cell_line = "";
                    cardiotoxic_variant_line.Cell_line_rs_identifier = "";
                    cardiotoxic_variant_line.Data_quality_aq = 0;
                    cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line = "";
                    cardiotoxic_variant_line.Relation_of_gene_symbol_to_drug = Relation_of_gene_symbol_to_drug_enum.E_m_p_t_y;
                }
            }
            Remove_duplicates();
        }

        public void Add_deep_copy_of_other(Lincs_cardiotoxic_variant_class other)
        {
            Lincs_cardiotoxic_variant_class deep_copy_of_other = other.Deep_copy();
            this.Add_to_array(deep_copy_of_other.Cardiotoxic_variants);
        }

        public void Write(string subdirectory, string fileName)
        {
            Lincs_cardiotoxic_variant_readWriteOptions readWriteOptions = new Lincs_cardiotoxic_variant_readWriteOptions(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Cardiotoxic_variants, readWriteOptions);
        }

        public void Read(string subdirectory, string fileName)
        {
            Lincs_cardiotoxic_variant_readWriteOptions readWriteOptions = new Lincs_cardiotoxic_variant_readWriteOptions(subdirectory, fileName);
            this.Cardiotoxic_variants = ReadWriteClass.ReadRawData_and_FillArray<Lincs_cardiotoxic_variant_line_class>(readWriteOptions);
        }

        public Lincs_cardiotoxic_variant_class Deep_copy()
        {
            Lincs_cardiotoxic_variant_class copy = (Lincs_cardiotoxic_variant_class)this.MemberwiseClone();
            int variants_length = this.Cardiotoxic_variants.Length;
            copy.Cardiotoxic_variants = new Lincs_cardiotoxic_variant_line_class[variants_length];
            for (int indexV=0; indexV<variants_length; indexV++)
            {
                copy.Cardiotoxic_variants[indexV] = this.Cardiotoxic_variants[indexV].Deep_copy();
            }
            return copy;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Lincs_cardiotoxic_variant_summary_line_class
    {
        public Lincs_genomics_analysis_stage_enum Dataprocessing_stage { get; set; }
        public Variant_cardiotoxic_effect_enum[] Clinical_effects { get; set; }
        public Relation_of_gene_symbol_to_drug_enum Relation_of_gene_symbol_to_drug { get; set; }
        public Variant_cardiotoxic_effect_enum Variant_effect { get; set; }
        public int Total_geneSymbols_count_for_indicated_drugs { get; set; }
        public int Total_rsIdentifiers_count_for_indicated_drugs { get; set; }
        public string[] Drugs_with_clinical_effect { get; set; }
        public float ClinicalEffectDrug_sameGeneSymbol_sameRsIdentifier_count { get; set; }
        public float ClinicalEffectDrug_sameGeneSymbol_differentRsIdentifier_count { get; set; }
        public float OtherDrug_sameGeneSymbol_sameRsIdentifier_count { get; set; }
        public float OtherDrug_sameGeneSymbol_differentRsIdentifier_count { get; set; }
        public string ReadWrite_drugs_with_clinical_effect
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Drugs_with_clinical_effect, Lincs_cardiotoxic_variant_summary_readWriteOptions_class.Array_delimiter); }
            set { this.Drugs_with_clinical_effect = ReadWriteClass.Get_array_from_readLine<string>(value, Lincs_cardiotoxic_variant_summary_readWriteOptions_class.Array_delimiter); }
        }
        public string ReadWrite_clinical_effects
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Clinical_effects, Lincs_cardiotoxic_variant_summary_readWriteOptions_class.Array_delimiter); }
            set { this.Clinical_effects = ReadWriteClass.Get_array_from_readLine<Variant_cardiotoxic_effect_enum>(value, Lincs_cardiotoxic_variant_summary_readWriteOptions_class.Array_delimiter); }
        }

        public Lincs_cardiotoxic_variant_summary_line_class Deep_copy()
        {
            Lincs_cardiotoxic_variant_summary_line_class copy = (Lincs_cardiotoxic_variant_summary_line_class)this.MemberwiseClone();
            int drugs_length = this.Drugs_with_clinical_effect.Length;
            copy.Drugs_with_clinical_effect = new string[drugs_length];
            for (int indexD = 0; indexD < drugs_length; indexD++)
            {
                copy.Drugs_with_clinical_effect[indexD] = (string)this.Drugs_with_clinical_effect[indexD].Clone();
            }
            int clinical_effects_length = this.Clinical_effects.Length;
            copy.Clinical_effects = new Variant_cardiotoxic_effect_enum[clinical_effects_length];
            for (int indexCE = 0; indexCE < clinical_effects_length; indexCE++)
            {
                copy.Clinical_effects[indexCE] = this.Clinical_effects[indexCE];
            }
            return copy;
        }
    }

    class Lincs_cardiotoxic_variant_summary_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter {  get { return ';'; } }
        public Lincs_cardiotoxic_variant_summary_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Dataprocessing_stage", "Relation_of_gene_symbol_to_drug", "ClinicalEffectDrug_sameGeneSymbol_sameRsIdentifier_count", "ClinicalEffectDrug_sameGeneSymbol_differentRsIdentifier_count","OtherDrug_sameGeneSymbol_sameRsIdentifier_count", "OtherDrug_sameGeneSymbol_differentRsIdentifier_count", "Total_rsIdentifiers_count_for_drugs_with_clinical_effect", "Total_geneSymbols_count_for_drugs_with_clinical_effect", "Variant_effect", "ReadWrite_clinical_effects" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_cardiotoxic_variant_summary_class
    {
        public Lincs_cardiotoxic_variant_summary_line_class[] Summary_lines { get; set; }

        private void Generate_from_lincs_cardiotoxic_variants_private(Lincs_cardiotoxic_variant_class cardiotoxic_variant)
        {
            int cardiotoxic_variants_length = cardiotoxic_variant.Cardiotoxic_variants.Length;
            cardiotoxic_variant.Cardiotoxic_variants = cardiotoxic_variant.Cardiotoxic_variants.OrderBy(l=>l.Filter_stage).ThenBy(l=>l.Relation_of_gene_symbol_to_drug).ThenBy(l=>l.Effects[0]).ThenBy(l => l.Gene_symbol).ThenBy(l => l.Rs_identifier).ToArray();
            Lincs_cardiotoxic_variant_line_class cardiotoxic_variant_line;
            Lincs_cardiotoxic_variant_summary_line_class variant_summary_line;
            List<Lincs_cardiotoxic_variant_summary_line_class> variant_summaries = new List<Lincs_cardiotoxic_variant_summary_line_class>();
            //float atleastOne_correctDrug_sameGeneSymbol_sameRsIdentifier_count = 0;
            //float atleastOne_correctDrug_sameGeneSymbol_differentRsIdentifier_count = 0;
            //float atleastOne_differentDrug_sameGeneSymbol_sameRsIdentifier_count = 0;
            //float atleastOne_differentDrug_sameGeneSymbol_differentRsIdentifier_count = 0;
            List<string> correctDrug_sameGeneSymbol_sameRsIdentifier_variants = new List<string>();
            List<string> correctDrug_sameGeneSymbol_differentRsIdentifier_variants = new List<string>();
            List<string> differentDrug_sameGeneSymbol_sameRsIdentifier_variants = new List<string>();
            List<string> differentDrug_sameGeneSymbol_differentRsIdentifier_variants = new List<string>();
            for (int indexCT=0; indexCT<cardiotoxic_variants_length;indexCT++)
            {
                cardiotoxic_variant_line = cardiotoxic_variant.Cardiotoxic_variants[indexCT];
                if (cardiotoxic_variant_line.Effects.Length!=1) { throw new Exception(); }
                if ((indexCT==0)
                    || (!cardiotoxic_variant_line.Filter_stage.Equals(cardiotoxic_variant.Cardiotoxic_variants[indexCT - 1].Filter_stage))
                    || (!cardiotoxic_variant_line.Effects[0].Equals(cardiotoxic_variant.Cardiotoxic_variants[indexCT - 1].Effects[0]))
                    || (!cardiotoxic_variant_line.Relation_of_gene_symbol_to_drug.Equals(cardiotoxic_variant.Cardiotoxic_variants[indexCT - 1].Relation_of_gene_symbol_to_drug)))
                {
                correctDrug_sameGeneSymbol_sameRsIdentifier_variants.Clear();
                    correctDrug_sameGeneSymbol_differentRsIdentifier_variants.Clear();
                    differentDrug_sameGeneSymbol_sameRsIdentifier_variants.Clear();
                    differentDrug_sameGeneSymbol_differentRsIdentifier_variants.Clear();
                }
                if (cardiotoxic_variant_line.Cell_line_treated_with_at_least_one_clinical_effect_drug)
                {
                    if (cardiotoxic_variant_line.Drugs_with_clinical_effect_documentation.Contains(cardiotoxic_variant_line.Drug_with_outlier_response_in_this_cell_line))
                    {
                        if (cardiotoxic_variant_line.Rs_identifier.Equals(cardiotoxic_variant_line.Cell_line_rs_identifier))
                        {
                            correctDrug_sameGeneSymbol_sameRsIdentifier_variants.Add(cardiotoxic_variant_line.Rs_identifier);
                        }
                        else
                        {
                            correctDrug_sameGeneSymbol_differentRsIdentifier_variants.Add(cardiotoxic_variant_line.Rs_identifier);
                        }
                    }
                    else
                    {
                        if (cardiotoxic_variant_line.Rs_identifier.Equals(cardiotoxic_variant_line.Cell_line_rs_identifier))
                        {
                            differentDrug_sameGeneSymbol_sameRsIdentifier_variants.Add(cardiotoxic_variant_line.Rs_identifier);
                        }
                        else
                        {
                            differentDrug_sameGeneSymbol_differentRsIdentifier_variants.Add(cardiotoxic_variant_line.Rs_identifier);
                        }
                    }
                }
                if (  (indexCT == cardiotoxic_variants_length-1)
                    || (!cardiotoxic_variant_line.Filter_stage.Equals(cardiotoxic_variant.Cardiotoxic_variants[indexCT + 1].Filter_stage))
                    || (!cardiotoxic_variant_line.Effects[0].Equals(cardiotoxic_variant.Cardiotoxic_variants[indexCT + 1].Effects[0]))
                    || (!cardiotoxic_variant_line.Relation_of_gene_symbol_to_drug.Equals(cardiotoxic_variant.Cardiotoxic_variants[indexCT + 1].Relation_of_gene_symbol_to_drug)))
                {
                    variant_summary_line = new Lincs_cardiotoxic_variant_summary_line_class();
                    variant_summary_line.Dataprocessing_stage = cardiotoxic_variant_line.Filter_stage;
                    variant_summary_line.Drugs_with_clinical_effect = Array_class.Deep_copy_string_array(cardiotoxic_variant_line.Drugs_with_clinical_effect_documentation);
                    variant_summary_line.Relation_of_gene_symbol_to_drug = cardiotoxic_variant_line.Relation_of_gene_symbol_to_drug;
                    variant_summary_line.ClinicalEffectDrug_sameGeneSymbol_differentRsIdentifier_count = correctDrug_sameGeneSymbol_differentRsIdentifier_variants.Distinct().ToArray().Length;
                    variant_summary_line.ClinicalEffectDrug_sameGeneSymbol_sameRsIdentifier_count = correctDrug_sameGeneSymbol_sameRsIdentifier_variants.Distinct().ToArray().Length;
                    variant_summary_line.OtherDrug_sameGeneSymbol_differentRsIdentifier_count = differentDrug_sameGeneSymbol_differentRsIdentifier_variants.Distinct().ToArray().Length;
                    variant_summary_line.OtherDrug_sameGeneSymbol_sameRsIdentifier_count = differentDrug_sameGeneSymbol_sameRsIdentifier_variants.Distinct().ToArray().Length;
                    variant_summary_line.Total_geneSymbols_count_for_indicated_drugs = cardiotoxic_variant_line.Total_geneSymbols_count_for_indicated_drugs;
                    variant_summary_line.Total_rsIdentifiers_count_for_indicated_drugs = cardiotoxic_variant_line.Total_rsIdentifiers_count_for_indicated_drugs;
                    variant_summary_line.Clinical_effects = Array_class.Deep_copy_array(cardiotoxic_variant_line.Effects);
                    variant_summaries.Add(variant_summary_line);
                }
            }
            this.Summary_lines = variant_summaries.ToArray();
        }

        public void Generate_from_lincs_cardiotoxic_variants(Lincs_cardiotoxic_variant_class cardiotoxic_variant)
        {
            Generate_from_lincs_cardiotoxic_variants_private(cardiotoxic_variant);
        }

        public void Write(string subdirectory, string fileName)
        {
            Lincs_cardiotoxic_variant_summary_readWriteOptions_class readWriteOptions = new Lincs_cardiotoxic_variant_summary_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Summary_lines, readWriteOptions);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Outlier_line_class
    {
        public float F1_score_weight { get; set; }
        public string Entity { get; set; }
        public string EntityClass { get; set; }
        public float Pvalue { get; set; }
        public float Q { get; set; }
        public int Sample_count { get; set; }
        public string Outlier { get; set; }
        public string Reference_valueType { get; set; }
        public float Mean_F1score_without_outlier { get; set; }
        public float F1_score_with_outlier { get; set; }
        public float Diff_F1_score { get; set; }
        public int Eigenassay_count { get; set; }
        public float Mean_correlation_reduced_full_data { get; set; }
        public float Median_correlation_reduced_full_data { get; set; }
        public int Eigenassays_count { get; set; }
        public float Adj_pvalue { get; set; }
        public string Full_entity_name { get; set; }
        public string Significant { get; set; }
        public string Final_selection { get; set; }

        public Outlier_line_class()
        {
            this.Entity = "";
            this.EntityClass = "";
            this.Outlier = "";
            this.Reference_valueType = "";
            this.Full_entity_name = "";
            this.Significant = "";
            this.Final_selection = "";
        }

        public Outlier_line_class Deep_copy()
        {
            Outlier_line_class copy = (Outlier_line_class)this.MemberwiseClone();
            copy.Entity = (string)this.Entity.Clone();
            copy.EntityClass = (string)this.EntityClass.Clone();
            copy.Outlier = (string)this.Outlier.Clone();
            copy.Reference_valueType = (string)this.Reference_valueType.Clone();
            copy.Full_entity_name = (string)this.Full_entity_name.Clone();
            copy.Significant = (string)this.Significant.Clone();
            return copy;
        }
    }

    class Outlier_readWriteOptions_class : ReadWriteOptions_base
    {
        public Outlier_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "F1_score_weight", "Entity", "EntityClass", "Pvalue", "Q", "Sample_count", "Outlier", "Reference_valueType", "Mean_F1score_without_outlier", "F1_score_with_outlier", "Diff_F1_score", "Eigenassay_count", "Mean_correlation_reduced_full_data", "Median_correlation_reduced_full_data", "Eigenassays_count", "Adj_pvalue", "Full_entity_name","Significant","Final_selection" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Outlier_options_class
    {
        public float Max_adj_pvalue { get; set; }
        public float Minimum_f1score_without_outlier { get; set; }
        public float Max_pvalue { get; set; }
        public int Minimum_total_sample_size { get; set; }

        public Outlier_options_class()
        {
            Max_adj_pvalue = 1F;
            Minimum_f1score_without_outlier = 0.5F;
            Max_pvalue = 1F;
            Minimum_total_sample_size = 3;
        }

    }

    class Outlier_class
    {
        const string finalSelection_yes_label = "Yes";
        const string significant_label = "Yes";
        const string coefficient_reference_valueType = "Coefficient_of_eigenassay";
        public Outlier_line_class[] Outliers { get; set; }
        public Outlier_options_class Options { get; set; }

        public Outlier_class()
        {
            Options = new Outlier_options_class();
        }

        private void Filter_by_options()
        {
            List<Outlier_line_class> keep = new List<Outlier_line_class>();
            foreach (Outlier_line_class outlier_line in this.Outliers)
            {
                if (((Options.Max_pvalue == -1) || (outlier_line.Pvalue <= Options.Max_pvalue))
                    && ((Options.Max_adj_pvalue == -1) || (outlier_line.Adj_pvalue <= Options.Max_adj_pvalue))
                    && (outlier_line.Mean_F1score_without_outlier >= Options.Minimum_f1score_without_outlier)
                    && (outlier_line.Sample_count >= Options.Minimum_total_sample_size)
                    && (outlier_line.Reference_valueType.Equals("Coefficient_of_eigenassay")))
                {
                    keep.Add(outlier_line);
                }
            }
            this.Outliers = keep.ToArray();
            throw new Exception(); //Not allowed, significance defined in R-script to prevent divergent cutoffs
        }

        private void Keep_only_significant_outliers()
        {
            List<Outlier_line_class> keep = new List<Outlier_line_class>();
            foreach (Outlier_line_class outlier_line in this.Outliers)
            {
                if (outlier_line.Significant.Equals(significant_label))
                {
                    keep.Add(outlier_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.Outliers = keep.ToArray();
        }

        private void Keep_only_final_selections()
        {
            List<Outlier_line_class> keep = new List<Outlier_line_class>();
            foreach (Outlier_line_class outlier_line in this.Outliers)
            {
                if (outlier_line.Final_selection.Equals(finalSelection_yes_label))
                {
                    if (!outlier_line.Significant.Equals(significant_label)) { throw new Exception(); }
                    keep.Add(outlier_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.Outliers = keep.ToArray();
        }

        private void Keep_only_coefficient_reference_value_type()
        {
            List<Outlier_line_class> keep = new List<Outlier_line_class>();
            foreach (Outlier_line_class outlier_line in this.Outliers)
            {
                if (outlier_line.Reference_valueType.Equals(coefficient_reference_valueType))
                {
                    keep.Add(outlier_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.Outliers = keep.ToArray();
        }

        private void Remove_clone_information_from_cellline()
        {
            foreach (Outlier_line_class outlier_line in Outliers)
            {
                outlier_line.Outlier = outlier_line.Outlier.Split('_')[0];
            }
        }

        public void Keep_only_one_line_per_cellline_drug_combination()
        {
            this.Outliers = this.Outliers.OrderBy(l => l.Outlier).ThenBy(l => l.Entity).ThenBy(l => l.Adj_pvalue).ThenByDescending(l => l.Mean_F1score_without_outlier).ThenByDescending(l => l.Mean_correlation_reduced_full_data).ThenBy(l => l.F1_score_weight).ToArray();
            List<Outlier_line_class> keep = new List<Outlier_line_class>();
            int outliers_length = this.Outliers.Length;
            Outlier_line_class outlier_line;
            for (int indexO = 0; indexO < outliers_length; indexO++)
            {
                outlier_line = this.Outliers[indexO];
                if ((indexO == 0)
                    || (!outlier_line.Outlier.Equals(this.Outliers[indexO - 1].Outlier))
                    || (!outlier_line.Entity.Equals(this.Outliers[indexO - 1].Entity)))
                {
                    keep.Add(outlier_line);
                }
            }
            this.Outliers = keep.ToArray();
        }

        public void Generate_by_reading(string subdirectory, string fileName)
        {
            Read(subdirectory, fileName);
            Remove_clone_information_from_cellline();
            Keep_only_final_selections();
            Keep_only_coefficient_reference_value_type();
        }

        public Dictionary<string, Dictionary<float, string>> Generate_drug_f1scoreWeigth_outlierCellline_dict()
        {
            Dictionary<string, Dictionary<float, string>> drug_f1scoreWeigth_outlierCellline = new Dictionary<string, Dictionary<float, string>>();
            int outliers_length = this.Outliers.Length;
            Outlier_line_class outlier_line;
            List<Outlier_line_class> keep = new List<Outlier_line_class>();
            for (int indexO = 0; indexO < outliers_length; indexO++)
            {
                outlier_line = this.Outliers[indexO];
                if (!drug_f1scoreWeigth_outlierCellline.ContainsKey(outlier_line.Entity))
                {
                    drug_f1scoreWeigth_outlierCellline.Add((string)outlier_line.Entity.Clone(), new Dictionary<float, string>());
                }
                if (!drug_f1scoreWeigth_outlierCellline[outlier_line.Entity].ContainsKey(outlier_line.F1_score_weight))
                {
                    drug_f1scoreWeigth_outlierCellline[outlier_line.Entity].Add(outlier_line.F1_score_weight, (string)outlier_line.Outlier.Clone());
                }
                else if (drug_f1scoreWeigth_outlierCellline[outlier_line.Entity][outlier_line.F1_score_weight].Equals(outlier_line.Outlier))
                { throw new Exception(); }
            }
            return drug_f1scoreWeigth_outlierCellline;
        }

        public float[] Get_all_ordered_f1_score_weights()
        {
            List<float> f1_scores = new List<float>();
            int outliers_length = Outliers.Length;
            Outlier_line_class outlier_line;
            for (int indexO = 0; indexO < outliers_length; indexO++)
            {
                outlier_line = Outliers[indexO];
                f1_scores.Add(outlier_line.F1_score_weight);
            }
            return f1_scores.Distinct().OrderBy(l => l).ToArray();
        }

        public string[] Get_all_entities()
        {
            List<string> drugs = new List<string>();
            int outliers_length = Outliers.Length;
            Outlier_line_class outlier_line;
            for (int indexO = 0; indexO < outliers_length; indexO++)
            {
                outlier_line = Outliers[indexO];
                drugs.Add(outlier_line.Entity);
            }
            return drugs.Distinct().OrderBy(l => l).ToArray();
        }

        public string[] Get_all_outlier()
        {
            List<string> drugs = new List<string>();
            int outliers_length = Outliers.Length;
            Outlier_line_class outlier_line;
            for (int indexO = 0; indexO < outliers_length; indexO++)
            {
                outlier_line = Outliers[indexO];
                drugs.Add(outlier_line.Outlier);
            }
            return drugs.Distinct().OrderBy(l => l).ToArray();
        }

        public Dictionary<string, string[]> Get_outlier_entities_dictionary()
        {
            Outliers = Outliers.OrderBy(l => l.Outlier).ThenBy(l => l.Entity).ToArray();
            int outliers_length = Outliers.Length;
            Outlier_line_class outlier_line;
            Dictionary<string, string[]> outlier_entities_dict = new Dictionary<string, string[]>();
            List<string> current_entities = new List<string>();
            for (int indexO = 0; indexO < outliers_length; indexO++)
            {
                outlier_line = Outliers[indexO];
                if ((indexO == 0) || (!outlier_line.Outlier.Equals(Outliers[indexO - 1].Outlier)))
                {
                    current_entities.Clear();
                }
                current_entities.Add(outlier_line.Entity);
                if ((indexO == outliers_length - 1) || (!outlier_line.Outlier.Equals(Outliers[indexO + 1].Outlier)))
                {
                    outlier_entities_dict.Add(outlier_line.Outlier.Split('_')[0], current_entities.Distinct().OrderBy(l => l).ToArray());
                }
            }
            return outlier_entities_dict;
        }

        private void Read(string subdirectory, string fileName)
        {
            Outlier_readWriteOptions_class readWriteOptions = new Outlier_readWriteOptions_class(subdirectory, fileName);
            this.Outliers = ReadWriteClass.ReadRawData_and_FillArray<Outlier_line_class>(readWriteOptions);
        }


    }
}
