using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common_classes;
using ReadWrite;
using Enrichment;
using Statistic;
using Network;

namespace Highthroughput_data
{
    enum Lincs_scp_summary_valueType_for_selection_enum { E_m_p_t_y, F1score_thisauc_and_diffauc }

    class Lincs_scp_summary_after_roc_line_class
    {
        public string Side_effect { get; set; }
        public Ontology_type_enum Ontology { get; set; }
        public string Scp { get; set; }
        public string Scp_completeName { get; set; }
        public string Association { get; set; }
        public DE_entry_enum Entry_type { get; set; }
        public float Cutoff_rank { get; set; }
        public int Max_cutoff_rank_for_AUC { get; set; }
        public float Selection_rank { get; set; }
        public string Selected { get; set; }
        public float Precision { get; set; }
        public float Recall { get; set; }
        public Lincs_scp_summary_valueType_for_selection_enum ValueTypeForSelection { get; set; }
        public float ValueForSelection { get; set; }
        public int Number_of_variants { get; set; }

        public string Up_down_status
        {
            get { return Conversion_class.Get_upDown_status_from_entryType(Entry_type); }
            set { Entry_type = Conversion_class.Get_entryType_from_upDown_status(value); }
        }

        public Lincs_scp_summary_after_roc_line_class()
        {
        }

        public Lincs_scp_summary_after_roc_line_class Deep_copy()
        {
            Lincs_scp_summary_after_roc_line_class copy = (Lincs_scp_summary_after_roc_line_class)this.MemberwiseClone();
            copy.Side_effect = (string)this.Side_effect.Clone();
            copy.Scp = (string)this.Scp.Clone();
            copy.Association = (string)this.Association.Clone();
            return copy;
        }
    }

    class Lincs_scp_summary_after_roc_readFromROptions_class : ReadWriteOptions_base
    {
        public Lincs_scp_summary_after_roc_readFromROptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Side_effect", "Ontology", "Scp", "Scp_completeName", "Association", "Entry_type", "Cutoff_rank", "Max_cutoff_rank_for_AUC", "Selection_rank", "Selected", "Precision", "Recall", "ValueForSelection", "ValueTypeForSelection" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }
    class Lincs_scp_summary_after_roc_readWriteOptions_class : Lincs_scp_summary_after_roc_readFromROptions_class
    {
        public Lincs_scp_summary_after_roc_readWriteOptions_class(string directory, string fileName) : base(directory, fileName)
        {
            this.File = directory + fileName;
            List<string> extended_property_names = new List<string>();
            extended_property_names.AddRange(base.Key_propertyNames);
            extended_property_names.Add("Number_of_variants");
            this.Key_propertyNames = extended_property_names.ToArray();
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_scp_summary_after_roc_for_website_readWriteOptions_class : Lincs_scp_summary_after_roc_readFromROptions_class
    {
        public Lincs_scp_summary_after_roc_for_website_readWriteOptions_class(string directory, string fileName) : base(directory, fileName)
        {
            this.File = directory + fileName;
            List<string> extended_property_names = new List<string>();
            this.Key_propertyNames = new string[] { "Scp_completeName",   "Up_down_status", "Selection_rank" };
            this.Key_columnNames = new string[] { "Pathway", Conversion_class.UpDownStatus_columnName, "F1 score AUC rank" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_scp_summary_after_roc_class
    {
        public Lincs_scp_summary_after_roc_line_class[] Scp_summaries { get; set; }

        public Lincs_scp_summary_after_roc_class()
        {
            this.Scp_summaries = new Lincs_scp_summary_after_roc_line_class[0];
        }

        private void Add_to_array(Lincs_scp_summary_after_roc_line_class[] add_scp_summaries)
        {
            int this_length = this.Scp_summaries.Length;
            int add_length = add_scp_summaries.Length;
            int new_length = this_length + add_length;
            Lincs_scp_summary_after_roc_line_class[] new_scp_summaries = new Lincs_scp_summary_after_roc_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_scp_summaries[indexNew] = this.Scp_summaries[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_scp_summaries[indexNew] = add_scp_summaries[indexAdd];
            }
            this.Scp_summaries = new_scp_summaries;
        }

        public void Generate_by_reading(string directory, string fileName)
        {
            Read_r_output(directory, fileName);
        }

        public string[] Get_all_toxicity_associations()
        {
            Dictionary<string, bool> association_dict = new Dictionary<string, bool>();
            foreach (Lincs_scp_summary_after_roc_line_class summary_line in this.Scp_summaries)
            {
                if (!association_dict.ContainsKey(summary_line.Association))
                {
                    association_dict.Add(summary_line.Association, true);
                }
            }
            return association_dict.Keys.OrderBy(l => l).ToArray();
        }

        public Ontology_type_enum[] Get_all_ontologies()
        {
            Dictionary<Ontology_type_enum, bool> ontology_dict = new Dictionary<Ontology_type_enum, bool>();
            foreach (Lincs_scp_summary_after_roc_line_class summary_line in this.Scp_summaries)
            {
                if (!ontology_dict.ContainsKey(summary_line.Ontology))
                {
                    ontology_dict.Add(summary_line.Ontology, true);
                }
            }
            return ontology_dict.Keys.OrderBy(l => l).ToArray();
        }

        public string[] Get_all_sideEffects()
        {
            Dictionary<string, bool> sideEffect_dict = new Dictionary<string, bool>();
            foreach (Lincs_scp_summary_after_roc_line_class summary_line in this.Scp_summaries)
            {
                if (!sideEffect_dict.ContainsKey(summary_line.Side_effect))
                {
                    sideEffect_dict.Add(summary_line.Side_effect, true);
                }
            }
            return sideEffect_dict.Keys.OrderBy(l => l).ToArray();
        }

        public DE_entry_enum[] Get_all_entryTypes()
        {
            Dictionary<DE_entry_enum, bool> entryType_dict = new Dictionary<DE_entry_enum, bool>();
            foreach (Lincs_scp_summary_after_roc_line_class summary_line in this.Scp_summaries)
            {
                if (!entryType_dict.ContainsKey(summary_line.Entry_type))
                {
                    entryType_dict.Add(summary_line.Entry_type, true);
                }
            }
            return entryType_dict.Keys.OrderBy(l => l).ToArray();
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, float>>>> Get_scp_sideEffect_association_entryType_rank_dict()
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, float>>>> scp_sideEffect_association_entryType_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, float>>>>();
            foreach (Lincs_scp_summary_after_roc_line_class scp_summary_line in this.Scp_summaries)
            {
                if (!scp_sideEffect_association_entryType_dict.ContainsKey(scp_summary_line.Scp))
                {
                    scp_sideEffect_association_entryType_dict.Add(scp_summary_line.Scp, new Dictionary<string, Dictionary<string, Dictionary<DE_entry_enum, float>>>());
                }
                if (!scp_sideEffect_association_entryType_dict[scp_summary_line.Scp].ContainsKey(scp_summary_line.Side_effect))
                {
                    scp_sideEffect_association_entryType_dict[scp_summary_line.Scp].Add(scp_summary_line.Side_effect, new Dictionary<string, Dictionary<DE_entry_enum, float>>());
                }
                if (!scp_sideEffect_association_entryType_dict[scp_summary_line.Scp][scp_summary_line.Side_effect].ContainsKey(scp_summary_line.Association))
                {
                    scp_sideEffect_association_entryType_dict[scp_summary_line.Scp][scp_summary_line.Side_effect].Add(scp_summary_line.Association, new Dictionary<DE_entry_enum, float>());
                }
                scp_sideEffect_association_entryType_dict[scp_summary_line.Scp][scp_summary_line.Side_effect][scp_summary_line.Association].Add(scp_summary_line.Entry_type, scp_summary_line.Selection_rank);
            }
            return scp_sideEffect_association_entryType_dict;
        }

        public Dictionary<Ontology_type_enum,int> Get_ontology_maxCutoffRankForAuc_dictionary_and_check_if_only_one_cutoff_for_each_ontology()
        {
            Dictionary<Ontology_type_enum, int> ontology_maxCutoffRankForAuc_dict = new Dictionary<Ontology_type_enum, int>();
            int scp_summaries_length = Scp_summaries.Length;
            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            Ontology_type_enum ontology;
            for (int indexScp=0; indexScp<scp_summaries_length; indexScp++)
            {
                scp_summary_line = this.Scp_summaries[indexScp];
                ontology = scp_summary_line.Ontology;
                if (!ontology_maxCutoffRankForAuc_dict.ContainsKey(ontology))
                {
                    ontology_maxCutoffRankForAuc_dict.Add(ontology, scp_summary_line.Max_cutoff_rank_for_AUC);
                }
                else if (ontology_maxCutoffRankForAuc_dict[ontology] !=scp_summary_line.Max_cutoff_rank_for_AUC)
                {
                    throw new Exception();
                }
            }
            return ontology_maxCutoffRankForAuc_dict;
        }

        public void Add_count_of_genomic_variants_per_pathway_and_count_of_variants_per_sideEffect_entryType_association(Lincs_vcf_genomic_data_class vcf_genomic_data, Ontology_library_class ontology_library)
        {
            Dictionary<string, Dictionary<string,bool>> scp_variant_dict = new Dictionary<string, Dictionary<string, bool>>();

            #region Fill scp_variantCount_dict and sideEffect_entryType_variantCount_dict
            vcf_genomic_data.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_rsIdentifer(vcf_genomic_data.Genomic_data);
            Dictionary<string, string[]> gene_scps_dict = ontology_library.Get_gene_scps_dictionary();
            int vcf_length = vcf_genomic_data.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class vcf_genomic_data_line;
            string[] scps;
            string scp;
            int scps_length;
            for (int indexVCF=0; indexVCF<vcf_length;indexVCF++)
            {
                vcf_genomic_data_line = vcf_genomic_data.Genomic_data[indexVCF];
                if (gene_scps_dict.ContainsKey(vcf_genomic_data_line.Gene_symbol))
                {
                    scps = gene_scps_dict[vcf_genomic_data_line.Gene_symbol];
                    scps_length = scps.Length;
                    for (int indexScp = 0; indexScp < scps_length; indexScp++)
                    {
                        scp = scps[indexScp];

                        #region Fill scp_variantCount_dict
                        if (!scp_variant_dict.ContainsKey(scp))
                        {
                            scp_variant_dict.Add(scp, new Dictionary<string, bool>());
                        }
                        if (!scp_variant_dict[scp].ContainsKey(vcf_genomic_data_line.Rs_identifier))
                        {
                            scp_variant_dict[scp].Add(vcf_genomic_data_line.Rs_identifier,true);
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region Add variant counts to scp summaries
            int scp_summaries_length = this.Scp_summaries.Length;
            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            for (int indexScp=0; indexScp<scp_summaries_length; indexScp++)
            {
                scp_summary_line = this.Scp_summaries[indexScp];
                if (scp_variant_dict.ContainsKey(scp_summary_line.Scp_completeName))
                {
                    scp_summary_line.Number_of_variants = scp_variant_dict[scp_summary_line.Scp_completeName].Count;
                }
            }
            #endregion
        }

        public string[] Get_all_scps_if_selected_is_true()
        {
            Dictionary<string, bool> scp_dict = new Dictionary<string, bool>();
            foreach (Lincs_scp_summary_after_roc_line_class scp_summary_line in this.Scp_summaries)
            {
                if ((scp_summary_line.Selected.Equals("TRUE"))&&(!scp_dict.ContainsKey(scp_summary_line.Scp_completeName)))
                {
                    scp_dict.Add(scp_summary_line.Scp_completeName, true);
                }
            }
            return scp_dict.Keys.ToArray();
        }

        public void Keep_only_selected_side_effect(string side_effect)
        {
            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            int scp_summaries_length = this.Scp_summaries.Length;
            List<Lincs_scp_summary_after_roc_line_class> keep = new List<Lincs_scp_summary_after_roc_line_class>();
            for (int indexScp = 0; indexScp < scp_summaries_length; indexScp++)
            {
                scp_summary_line = this.Scp_summaries[indexScp];
                if (scp_summary_line.Side_effect.Equals(side_effect))
                {
                    keep.Add(scp_summary_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.Scp_summaries = keep.ToArray();
        }

        public void Keep_only_selected_association(string association)
        {
            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            int scp_summaries_length = this.Scp_summaries.Length;
            List<Lincs_scp_summary_after_roc_line_class> keep = new List<Lincs_scp_summary_after_roc_line_class>();
            for (int indexScp = 0; indexScp < scp_summaries_length; indexScp++)
            {
                scp_summary_line = this.Scp_summaries[indexScp];
                if (scp_summary_line.Association.Equals(association))
                {
                    keep.Add(scp_summary_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.Scp_summaries = keep.ToArray();
        }
        public void Keep_only_selected_ontologies(params Ontology_type_enum[] keep_ontologies)
        {
            Dictionary<Ontology_type_enum, bool> keep_ontology_dict = new Dictionary<Ontology_type_enum, bool>();
            foreach (Ontology_type_enum ontology in keep_ontologies)
            {
                keep_ontology_dict.Add(ontology, true);
            }

            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            int scp_summaries_length = this.Scp_summaries.Length;
            List<Lincs_scp_summary_after_roc_line_class> keep = new List<Lincs_scp_summary_after_roc_line_class>();
            for (int indexScp = 0; indexScp < scp_summaries_length; indexScp++)
            {
                scp_summary_line = this.Scp_summaries[indexScp];
                if (keep_ontology_dict.ContainsKey(scp_summary_line.Ontology))
                {
                    keep.Add(scp_summary_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.Scp_summaries = keep.ToArray();
        }

        public void Keep_only_scps_that_passed_selection_criteria()
        {
            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            int scp_summaries_length = this.Scp_summaries.Length;
            List<Lincs_scp_summary_after_roc_line_class> keep = new List<Lincs_scp_summary_after_roc_line_class>();
            for (int indexScp = 0; indexScp < scp_summaries_length; indexScp++)
            {
                scp_summary_line = this.Scp_summaries[indexScp];
                if (scp_summary_line.Selected.Equals("TRUE"))
                {
                    keep.Add(scp_summary_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.Scp_summaries = keep.ToArray();
        }

        public void Keep_only_selected_entryType(DE_entry_enum entryType)
        {
            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            int scp_summaries_length = this.Scp_summaries.Length;
            List<Lincs_scp_summary_after_roc_line_class> keep = new List<Lincs_scp_summary_after_roc_line_class>();
            for (int indexScp = 0; indexScp < scp_summaries_length; indexScp++)
            {
                scp_summary_line = this.Scp_summaries[indexScp];
                if (scp_summary_line.Entry_type.Equals(entryType))
                {
                    keep.Add(scp_summary_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.Scp_summaries = keep.ToArray();
        }

        private void Read_r_output(string directory, string fileName)
        {
            Lincs_scp_summary_after_roc_readFromROptions_class readWriteOptions = new Lincs_scp_summary_after_roc_readFromROptions_class(directory, fileName);
            this.Scp_summaries = ReadWriteClass.ReadRawData_and_FillArray<Lincs_scp_summary_after_roc_line_class>(readWriteOptions);
        }

        public void Write(string directory, string fileName)
        {
            Lincs_scp_summary_after_roc_readWriteOptions_class readWriteOptions = new Lincs_scp_summary_after_roc_readWriteOptions_class(directory, fileName);
            ReadWriteClass.WriteData(this.Scp_summaries, readWriteOptions);
        }

        public void Write_for_website(string directory, string fileName)
        {
            string[] associations = this.Get_all_toxicity_associations();
            string[] sideEffects = this.Get_all_sideEffects();
            Ontology_type_enum[] ontologies = this.Get_all_ontologies();
            if (associations.Length != 1) { throw new Exception(); }
            if (sideEffects.Length != 1) { throw new Exception(); }
            if (ontologies.Length != 1) { throw new Exception(); }

            Lincs_scp_summary_after_roc_for_website_readWriteOptions_class readWriteOptions = new Lincs_scp_summary_after_roc_for_website_readWriteOptions_class(directory, fileName);
            ReadWriteClass.WriteData(this.Scp_summaries, readWriteOptions);
        }

        public void Read_from_fda(string directory, string fileName)
        {
            Lincs_scp_summary_after_roc_for_website_readWriteOptions_class readWriteOptions = new Lincs_scp_summary_after_roc_for_website_readWriteOptions_class(directory, fileName);
            this.Scp_summaries = ReadWriteClass.ReadRawData_and_FillArray<Lincs_scp_summary_after_roc_line_class>(readWriteOptions);
        }

        public Lincs_scp_summary_after_roc_class Deep_copy()
        {
            Lincs_scp_summary_after_roc_class copy = (Lincs_scp_summary_after_roc_class)this.MemberwiseClone();
            int scp_summaries_length = this.Scp_summaries.Length;
            copy.Scp_summaries = new Lincs_scp_summary_after_roc_line_class[scp_summaries_length];
            for (int indexSvd=0; indexSvd<scp_summaries_length;indexSvd++)
            {
                copy.Scp_summaries[indexSvd] = this.Scp_summaries[indexSvd].Deep_copy();
            }
            return copy;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    class Lincs_genomic_variant_count_foreach_drug_line_class
    {
        public string Drug { get; set; }
        public string Drug_is_cardiotoxic { get; set; }
        public Drug_type_enum Drug_type { get; set; }
        public Ontology_type_enum Ontology {get;set;}
        public string Side_effect { get; set; }
        public string Toxicity_association { get; set; }
        public int Count_of_all_variants_mapping_to_sameLevel_scps { get; set; }
        public int Count_of_variants_that_are_not_covered_by_lower_level_scps { get; set; }
        public int SCPs_count { get; set; }
        public DE_entry_enum EntryType { get; set; }

        public Lincs_genomic_variant_count_foreach_drug_line_class Deep_copy()
        {
            Lincs_genomic_variant_count_foreach_drug_line_class copy = (Lincs_genomic_variant_count_foreach_drug_line_class)this.MemberwiseClone();
            copy.Drug = (string)this.Drug.Clone();
            copy.Side_effect = (string)this.Side_effect.Clone();
            copy.Toxicity_association = (string)this.Toxicity_association.Clone();
            copy.Drug_is_cardiotoxic = (string)this.Drug_is_cardiotoxic.Clone();
            return copy;
        }
    }
    class Lincs_genomics_variant_count_foreach_drug_readWriteOptions : ReadWriteOptions_base
    {
        public Lincs_genomics_variant_count_foreach_drug_readWriteOptions(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Drug", "Drug_is_cardiotoxic", "Drug_type", "Side_effect", "Toxicity_association", "Ontology", "EntryType", "Count_of_variants_that_are_not_covered_by_lower_level_scps","Count_of_all_variants_mapping_to_sameLevel_scps", "SCPs_count" };
            this.Key_columnNames = this.Key_propertyNames;
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Lincs_toxicity_classifications_definition_class
    {
        public const string Cardiotoxic_tkis = "cardiotoxic TKIs";
        public const string Noncardiotoxic_tkis = "non-cardiotoxic TKIs";

        public static string Get_scp_classification_for_cardiotoxic_TKIs(DE_entry_enum entryType)
        {
            switch (entryType)
            {
                case DE_entry_enum.Diffrna_up:
                    return "SCP upregulated at higher ranks by cardiotoxic TKIs";
                case DE_entry_enum.Diffrna_down:
                    return "SCP downregulated at higher ranks by cardiotoxic TKIs";
                default:
                    throw new Exception();
            }
        }

        public static string Get_scp_classification_for_noncardiotoxic_TKIs(DE_entry_enum entryType)
        {
            switch (entryType)
            {
                case DE_entry_enum.Diffrna_up:
                    return "SCP upregulated at higher ranks by noncardiotoxic TKIs";
                case DE_entry_enum.Diffrna_down:
                    return "SCP downregulated at higher ranks by noncardiotoxic TKIs";
                default:
                    throw new Exception();
            }
        }
    }


    class Lincs_genomic_variant_counts_options_class
    {
        public Ontology_type_enum[] Ontology_consideration_hierarchy { get; set; }

        public Lincs_genomic_variant_counts_options_class()
        {
            Ontology_consideration_hierarchy = new Ontology_type_enum[] { Ontology_type_enum.Mbco_level4, Ontology_type_enum.Mbco_level3, Ontology_type_enum.Mbco_level2, Ontology_type_enum.Mbco_level1 };
        }
    }

    class Lincs_genomic_variant_counts_foreach_drug_class
    {
        public Lincs_genomic_variant_count_foreach_drug_line_class[] Variants_count { get; set; }
        public Lincs_vcf_genomic_data_line_class[] Selected_variants { get; set; }
        public Lincs_genomic_variant_counts_options_class Options { get; set; }

        public Lincs_genomic_variant_counts_foreach_drug_class()
        {
            this.Options = new Lincs_genomic_variant_counts_options_class();
        }

        private Ontology_type_enum[] Get_ontologies_in_correct_order(Ontology_type_enum[] ontologies)
        {
            List<Ontology_type_enum> ontologies_in_correct_order = new List<Ontology_type_enum>();
            int ontologies_length = Options.Ontology_consideration_hierarchy.Length;
            Ontology_type_enum[] options_ontologies = new Ontology_type_enum[ontologies_length];
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                options_ontologies[indexO] = Options.Ontology_consideration_hierarchy[indexO];
            }

            foreach (Ontology_type_enum ontology in options_ontologies)
            {
                if (ontologies.Contains(ontology))
                {
                    ontologies_in_correct_order.Add(ontology);
                }
            }
            ontologies = Overlap_class.Get_part_of_list1_but_not_of_list2(ontologies, options_ontologies);
            ontologies_in_correct_order.AddRange(ontologies);
            return ontologies_in_correct_order.ToArray();
        }

        #region Count variants old code
        private int Count_all_variants_mapping_to_selected_genes(Lincs_vcf_genomic_data_class genomic_data, string[] selected_genes)
        {
            Dictionary<string, bool> selected_genes_dict = new Dictionary<string, bool>();
            foreach (string selected_gene in selected_genes)
            {
                selected_genes_dict.Add(selected_gene,true);
            }

            int genomic_length = genomic_data.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            int variants_count = 0;
            int stringCompare = -1;
            for (int indexGenomic=0; indexGenomic<genomic_length; indexGenomic++)
            {
                genomic_data_line = genomic_data.Genomic_data[indexGenomic];
                if (indexGenomic == 0) { if (selected_genes_dict.ContainsKey(genomic_data_line.Gene_symbol)) { variants_count++; } }
                else
                {
                    stringCompare = genomic_data_line.Rs_identifier.CompareTo(genomic_data.Genomic_data[indexGenomic - 1].Rs_identifier);
                    if (stringCompare < 0) { throw new Exception(); }
                    else if ((stringCompare > 0) && (selected_genes_dict.ContainsKey(genomic_data_line.Gene_symbol))) { variants_count++; }
                }
            }
            return variants_count;
        }

        private Lincs_genomic_variant_count_foreach_drug_line_class[] Count_all_variants_mapping_to_each_drug_and_ontology(Lincs_vcf_genomic_data_class genomic_data, Enrichment2018_results_class enrichment_results, string association, string sideEffect)
        {
            DE_entry_enum entrytype = enrichment_results.Enrichment_results[0].Sample_entryType;
            Dictionary<string, Dictionary<string, bool>> drug_consideredRSVariants_dict = new Dictionary<string, Dictionary<string, bool>>();
            Ontology_type_enum[] ontologies = enrichment_results.Get_all_unique_ontologies();
            Ontology_type_enum[] ontologies_in_correct_order = Get_ontologies_in_correct_order(ontologies);
            int ontologies_length = ontologies_in_correct_order.Length;
            Ontology_type_enum ontology;
            Enrichment2018_results_class current_enrichment_results;
            Enrichment2018_results_class drug_enrichment_results;
            int enrichment_length;
            Enrichment2018_results_line_class enrichment_results_line;
            Ontology_library_class ontology_library;
            List<string> current_scps = new List<string>();
            string[] current_genes;
            int variants_count = 0;
            Lincs_genomic_variant_count_foreach_drug_line_class new_genomic_variant_count_line;
            List<Lincs_genomic_variant_count_foreach_drug_line_class> add_genomic_variant_counts = new List<Lincs_genomic_variant_count_foreach_drug_line_class>();
            string[] drugs = enrichment_results.Get_all_distinct_ordered_lincs_drugs();
            string drug;
            int drugs_length = drugs.Length;

            Dictionary<string, bool> considered_genes = new Dictionary<string, bool>();
            List<string> final_genes = new List<string>();

            for (int indexDrug=0; indexDrug < drugs_length; indexDrug++)
            {
                drug = drugs[indexDrug];
                drug_enrichment_results = enrichment_results.Deep_copy();
                drug_enrichment_results.Keep_only_lincs_drugs(drug);
                considered_genes.Clear();
                for (int indexO = 0; indexO < ontologies_length; indexO++)
                {
                    ontology = ontologies_in_correct_order[indexO];
                    current_enrichment_results = drug_enrichment_results.Deep_copy();
                    current_enrichment_results.Keep_only_indicated_ontologies_if_existent(ontology);
                    ontology_library = new Ontology_library_class();
                    ontology_library.Generate_by_reading(ontology);
                    enrichment_length = current_enrichment_results.Enrichment_results.Length;
                    current_scps.Clear();
                    for (int indexE = 0; indexE < enrichment_length; indexE++)
                    {
                        enrichment_results_line = current_enrichment_results.Enrichment_results[indexE];
                        if (!enrichment_results_line.Lincs_drug.Equals(drug)) { throw new Exception(); }
                        if (!enrichment_results_line.Sample_entryType.Equals(entrytype)) { throw new Exception(); }
                        current_scps.Add(enrichment_results_line.Scp);
                    }
                    current_scps = current_scps.Distinct().OrderBy(l => l).ToList();
                    current_genes = ontology_library.Get_all_ordered_unique_gene_symbols_of_input_scps(current_scps.ToArray());
                    final_genes.Clear();
                    foreach (string current_gene in current_genes)
                    {
                        if (!considered_genes.ContainsKey(current_gene))
                        {
                            considered_genes.Add(current_gene, true);
                            final_genes.Add(current_gene);
                        }
                    }
                    variants_count = Count_all_variants_mapping_to_selected_genes(genomic_data, final_genes.ToArray());
                    new_genomic_variant_count_line = new Lincs_genomic_variant_count_foreach_drug_line_class();
                    new_genomic_variant_count_line.Drug = (string)drug.Clone();
                    new_genomic_variant_count_line.EntryType = entrytype;
                    new_genomic_variant_count_line.Ontology = ontology;
                    new_genomic_variant_count_line.Toxicity_association = (string)association.Clone();
                    new_genomic_variant_count_line.Side_effect = (string)sideEffect.Clone();
                    new_genomic_variant_count_line.Count_of_variants_that_are_not_covered_by_lower_level_scps = variants_count;
                    new_genomic_variant_count_line.SCPs_count = current_scps.Count;
                    add_genomic_variant_counts.Add(new_genomic_variant_count_line);
                }
            }
            return add_genomic_variant_counts.ToArray();
        }

        private void Count_variants_per_pathway(Lincs_vcf_genomic_data_class genomic_data, Enrichment2018_results_class enrichment_results, Lincs_scp_summary_after_roc_class scp_summary)
        {
            Dictionary<Ontology_type_enum, int> ontology_maxCutoffRankForAuc_dict = scp_summary.Get_ontology_maxCutoffRankForAuc_dictionary_and_check_if_only_one_cutoff_for_each_ontology();
            enrichment_results.Keep_only_top_x_ranked_scps_based_on_existing_fractional_ranks_specified_for_each_ontology(ontology_maxCutoffRankForAuc_dict);

            genomic_data.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_rsIdentifer(genomic_data.Genomic_data);
            DE_entry_enum[] entryTypes = scp_summary.Get_all_entryTypes();
            DE_entry_enum entryType;
            int entryTypes_length = entryTypes.Length;
            string[] associations = scp_summary.Get_all_toxicity_associations();
            string association;
            int associations_length = associations.Length;
            string[] sideEffects = scp_summary.Get_all_sideEffects();
            string sideEffect;
            int sideEffects_length = sideEffects.Length;
            Enrichment2018_results_class current_enrichment_results;
            Lincs_scp_summary_after_roc_class sideEffect_scp_summary;
            Lincs_scp_summary_after_roc_class association_scp_summary;
            Lincs_scp_summary_after_roc_class entryType_scp_summary;
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            string[] drugs;
            string merged_drugs_name;
            string[] scps;
            Lincs_genomic_variant_count_foreach_drug_line_class[] new_genomic_variant_count_lines;
            List<Lincs_genomic_variant_count_foreach_drug_line_class> genomic_variant_counts = new List<Lincs_genomic_variant_count_foreach_drug_line_class>();
            for (int indexSideEffect = 0; indexSideEffect < sideEffects_length; indexSideEffect++)
            {
                sideEffect = sideEffects[indexSideEffect];
                sideEffect_scp_summary = scp_summary.Deep_copy();
                sideEffect_scp_summary.Keep_only_selected_side_effect(sideEffect);
                for (int indexAssociation = 0; indexAssociation < associations_length; indexAssociation++)
                {
                    association = associations[indexAssociation];
                    switch (association)
                    {
                        case "cardiotoxic TKIs":
                            drugs = drug_legend.Get_all_cardiotoxic_tkis();
                            merged_drugs_name = "All cardiotoxic TKIs";
                            break;
                        case "non-cardiotoxic TKIs":
                            drugs = drug_legend.Get_all_non_cardiotoxic_tkis();
                            merged_drugs_name = "All non cardiotoxic TKIs";
                            break;
                        default:
                            throw new Exception();
                    }
                    association_scp_summary = sideEffect_scp_summary.Deep_copy();
                    association_scp_summary.Keep_only_selected_association(association);
                    for (int indexEntry = 0; indexEntry < entryTypes_length; indexEntry++)
                    {
                        entryType = entryTypes[indexEntry];
                        entryType_scp_summary = association_scp_summary.Deep_copy();
                        entryType_scp_summary.Keep_only_selected_entryType(entryType);
                        scps = entryType_scp_summary.Get_all_scps_if_selected_is_true();
                        current_enrichment_results = enrichment_results.Deep_copy();
                        current_enrichment_results.Filter_by_keeping_only_lines_with_indicated_entryType(entryType);
                        current_enrichment_results.Filter_by_keeping_only_lines_with_indicated_lincs_drugs(drugs);
                        current_enrichment_results.Filter_by_keeping_only_input_scps(scps);
                        new_genomic_variant_count_lines = Count_all_variants_mapping_to_each_drug_and_ontology(genomic_data, current_enrichment_results, association, sideEffect);
                        genomic_variant_counts.AddRange(new_genomic_variant_count_lines);
                        foreach (Enrichment2018_results_line_class results_line in current_enrichment_results.Enrichment_results)
                        {
                            results_line.Lincs_drug = (string)merged_drugs_name.Clone();
                        }
                        new_genomic_variant_count_lines = Count_all_variants_mapping_to_each_drug_and_ontology(genomic_data, current_enrichment_results, association, sideEffect);
                        genomic_variant_counts.AddRange(new_genomic_variant_count_lines);
                    }
                }
            }
            this.Variants_count = genomic_variant_counts.ToArray();
        }
        #endregion

        private Ontology_type_enum[] Get_all_ontologies_from_selected_variants(Lincs_vcf_genomic_data_line_class[] sameSideEffect_selected_genomic_lines)
        {
            Dictionary<Ontology_type_enum, bool> ontology_dict = new Dictionary<Ontology_type_enum, bool>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in sameSideEffect_selected_genomic_lines)
            {
                if (!ontology_dict.ContainsKey(genomic_line.Ontology))
                {
                    ontology_dict.Add(genomic_line.Ontology, true);
                }
            }
            return ontology_dict.Keys.ToArray();
        }

        private Lincs_vcf_genomic_data_line_class[] Get_all_genomic_data_lines_with_given_ontology(Lincs_vcf_genomic_data_line_class[] genomic_lines, Ontology_type_enum ontology)
        {
            List<Lincs_vcf_genomic_data_line_class> givenOntology_lines = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_data_line in genomic_lines)
            {
                if (genomic_data_line.Ontology.Equals(ontology))
                {
                    givenOntology_lines.Add(genomic_data_line);
                }
            }
            return givenOntology_lines.ToArray();
        }

        private Lincs_vcf_genomic_data_line_class[] Return_all_variant_lines_mapping_to_selected_genes(Lincs_vcf_genomic_data_class genomic_data, Ontology_type_enum ontology, string drug, DE_entry_enum entryType, string scp, string[] selected_genes, string toxicity_association, string sideEffect)
        {
            List<Lincs_vcf_genomic_data_line_class> variant_lines = new List<Lincs_vcf_genomic_data_line_class>();
            Dictionary<string, bool> selected_genes_dict = new Dictionary<string, bool>();
            foreach (string selected_gene in selected_genes)
            {
                selected_genes_dict.Add(selected_gene, true);
            }
            int genomic_length = genomic_data.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            Lincs_vcf_genomic_data_line_class mapping_genomic_data_line;
            StringBuilder sb = new StringBuilder();
            for (int indexGenomic = 0; indexGenomic < genomic_length; indexGenomic++)
            {
                genomic_data_line = genomic_data.Genomic_data[indexGenomic];
                if (   (indexGenomic != 0)
                    && (genomic_data_line.Rs_identifier.CompareTo(genomic_data.Genomic_data[indexGenomic - 1].Rs_identifier) <= 0)
                    && (genomic_data_line.Gene_symbol.CompareTo(genomic_data.Genomic_data[indexGenomic - 1].Gene_symbol) <= 0)
                    && (genomic_data_line.Cell_line.CompareTo(genomic_data.Genomic_data[indexGenomic - 1].Cell_line) <= 0)
                    ) { throw new Exception(); }
                if (  (indexGenomic==0)
                    || (!genomic_data_line.Gene_symbol.Equals(genomic_data.Genomic_data[indexGenomic - 1].Gene_symbol))
                    || (!genomic_data_line.Rs_identifier.Equals(genomic_data.Genomic_data[indexGenomic - 1].Rs_identifier)))
                {
                    if (selected_genes_dict.ContainsKey(genomic_data_line.Gene_symbol))
                    {
                        mapping_genomic_data_line = genomic_data_line.Deep_copy();
                        mapping_genomic_data_line.Ontology = ontology;
                        mapping_genomic_data_line.Scp_gene = (string)mapping_genomic_data_line.Gene_symbol.Clone();
                        mapping_genomic_data_line.Drug_regulating_scp = (string)drug.Clone(); 
                        mapping_genomic_data_line.Entry_type = entryType;
                        mapping_genomic_data_line.Scp = (string)scp.Clone();
                        mapping_genomic_data_line.Ontology = ontology;
                        mapping_genomic_data_line.Toxicity_association = (string)toxicity_association.Clone();
                        mapping_genomic_data_line.Side_effect = (string)sideEffect.Clone();
                        variant_lines.Add(mapping_genomic_data_line);
                    }
                }
            }
            return variant_lines.ToArray();
        }

        private Lincs_vcf_genomic_data_line_class[] Return_all_variant_lines_mapping_to_each_drug_and_ontology(Lincs_vcf_genomic_data_class genomic_data, Enrichment2018_results_class enrichment_results, string toxicity_association, string sideEffect)
        {
            DE_entry_enum entrytype = enrichment_results.Enrichment_results[0].Sample_entryType;
            Dictionary<string, Dictionary<string, bool>> drug_consideredRSVariants_dict = new Dictionary<string, Dictionary<string, bool>>();
            Ontology_type_enum[] ontologies = enrichment_results.Get_all_unique_ontologies();
            Ontology_type_enum[] ontologies_in_correct_order = Get_ontologies_in_correct_order(ontologies);
            int ontologies_length = ontologies_in_correct_order.Length;
            Ontology_type_enum ontology;
            Enrichment2018_results_class ontology_enrichment_results;
            Enrichment2018_results_class drug_enrichment_results;
            int enrichment_length;
            Enrichment2018_results_line_class enrichment_results_line;
            Ontology_library_class ontology_library;
            List<string> current_scps_list = new List<string>();
            string[] current_genes;
            Lincs_vcf_genomic_data_line_class[] mapping_variant_lines;
            List<Lincs_vcf_genomic_data_line_class> return_mapping_variant_lines = new List<Lincs_vcf_genomic_data_line_class>();
            string[] drugs = enrichment_results.Get_all_distinct_ordered_lincs_drugs();
            string drug;
            int drugs_length = drugs.Length;

            for (int indexDrug = 0; indexDrug < drugs_length; indexDrug++)
            {
                drug = drugs[indexDrug];
                drug_enrichment_results = enrichment_results.Deep_copy();
                drug_enrichment_results.Keep_only_lincs_drugs(drug);
                for (int indexO = 0; indexO < ontologies_length; indexO++)
                {
                    ontology = ontologies_in_correct_order[indexO];
                    ontology_enrichment_results = drug_enrichment_results.Deep_copy();
                    ontology_enrichment_results.Keep_only_indicated_ontologies_if_existent(ontology);
                    ontology_library = new Ontology_library_class();
                    ontology_library.Generate_by_reading(ontology);
                    enrichment_length = ontology_enrichment_results.Enrichment_results.Length;
                    ontology_enrichment_results.Enrichment_results = ontology_enrichment_results.Enrichment_results.OrderBy(l => l.Scp).ToArray();
                    for (int indexE = 0; indexE < enrichment_length; indexE++)
                    {
                        enrichment_results_line = ontology_enrichment_results.Enrichment_results[indexE];
                        if (!enrichment_results_line.Lincs_drug.Equals(drug)) { throw new Exception(); }
                        if (!enrichment_results_line.Sample_entryType.Equals(entrytype)) { throw new Exception(); }
                        if (!enrichment_results_line.Ontology.Equals(ontology)) { throw new Exception(); }
                        if (  (indexE==0)
                            ||(!enrichment_results_line.Scp.Equals(ontology_enrichment_results.Enrichment_results[indexE-1].Scp)))
                        {
                            current_genes = ontology_library.Get_all_ordered_unique_gene_symbols_of_input_scps(enrichment_results_line.Scp);
                            mapping_variant_lines = Return_all_variant_lines_mapping_to_selected_genes(genomic_data, ontology, drug, entrytype, enrichment_results_line.Scp, current_genes, toxicity_association, sideEffect);
                            return_mapping_variant_lines.AddRange(mapping_variant_lines);
                        }
                    }
                }
            }
            return return_mapping_variant_lines.ToArray();
        }

        private void Collect_variant_lines_per_pathway(Lincs_vcf_genomic_data_class genomic_data, Enrichment2018_results_class enrichment_results, Lincs_scp_summary_after_roc_class scp_summary)
        {
            Dictionary<Ontology_type_enum, int> ontology_maxCutoffRankForAuc_dict = scp_summary.Get_ontology_maxCutoffRankForAuc_dictionary_and_check_if_only_one_cutoff_for_each_ontology();
            enrichment_results.Keep_only_top_x_ranked_scps_based_on_existing_fractional_ranks_specified_for_each_ontology(ontology_maxCutoffRankForAuc_dict);

            genomic_data.Genomic_data = Lincs_vcf_genomic_data_line_class.Order_by_geneSymbol_rsIdentifier_cellline(genomic_data.Genomic_data);
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;
            string[] associations;
            string association;
            int associations_length;
            string[] sideEffects = scp_summary.Get_all_sideEffects();
            string sideEffect;
            int sideEffects_length = sideEffects.Length;
            Enrichment2018_results_class current_enrichment_results;
            Lincs_scp_summary_after_roc_class sideEffect_scp_summary;
            Lincs_scp_summary_after_roc_class association_scp_summary;
            Lincs_scp_summary_after_roc_class entryType_scp_summary;
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            string[] drugs;
            string[] scps;
            Lincs_vcf_genomic_data_line_class[] new_genomic_variant_lines;
            List<Lincs_vcf_genomic_data_line_class> selected_genomic_variant_lines = new List<Lincs_vcf_genomic_data_line_class>();
            for (int indexSideEffect = 0; indexSideEffect < sideEffects_length; indexSideEffect++)
            {
                sideEffect = sideEffects[indexSideEffect];
                sideEffect_scp_summary = scp_summary.Deep_copy();
                sideEffect_scp_summary.Keep_only_selected_side_effect(sideEffect);
                associations = sideEffect_scp_summary.Get_all_toxicity_associations();
                associations_length = associations.Length;
                for (int indexAssociation = 0; indexAssociation < associations_length; indexAssociation++)
                {
                    association = associations[indexAssociation];
                    switch (association)
                    {
                        case Lincs_toxicity_classifications_definition_class.Cardiotoxic_tkis:
                            drugs = drug_legend.Get_all_cardiotoxic_tkis();
                            break;
                        case Lincs_toxicity_classifications_definition_class.Noncardiotoxic_tkis:
                            drugs = drug_legend.Get_all_non_cardiotoxic_tkis();
                            break;
                        default:
                            throw new Exception();
                    }
                    association_scp_summary = sideEffect_scp_summary.Deep_copy();
                    association_scp_summary.Keep_only_selected_association(association);
                    entryTypes = association_scp_summary.Get_all_entryTypes();
                    entryTypes_length = entryTypes.Length;
                    for (int indexEntry = 0; indexEntry < entryTypes_length; indexEntry++)
                    {
                        entryType = entryTypes[indexEntry];
                        entryType_scp_summary = association_scp_summary.Deep_copy();
                        entryType_scp_summary.Keep_only_selected_entryType(entryType);
                        scps = entryType_scp_summary.Get_all_scps_if_selected_is_true();
                        current_enrichment_results = enrichment_results.Deep_copy();
                        current_enrichment_results.Filter_by_keeping_only_lines_with_indicated_entryType(entryType);
                        current_enrichment_results.Filter_by_keeping_only_lines_with_indicated_lincs_drugs(drugs);
                        current_enrichment_results.Filter_by_keeping_only_input_scps(scps);
                        new_genomic_variant_lines = Return_all_variant_lines_mapping_to_each_drug_and_ontology(genomic_data, current_enrichment_results, association, sideEffect);
                        selected_genomic_variant_lines.AddRange(new_genomic_variant_lines);
                    }
                }
            }
            this.Selected_variants = selected_genomic_variant_lines.ToArray();
        }

        private Lincs_genomic_variant_count_foreach_drug_line_class[] Count_selected_variants_from_sameSideEffect_sameToxicityAssociation_sameDrugRegulatingScp_sameEntryType_selected_variants(Lincs_vcf_genomic_data_line_class[] sameSideEffect_sameDrug_selected_genomic_lines)
        {
            string sideEffect = sameSideEffect_sameDrug_selected_genomic_lines[0].Side_effect;
            string drug = sameSideEffect_sameDrug_selected_genomic_lines[0].Drug_regulating_scp;
            DE_entry_enum entryType = sameSideEffect_sameDrug_selected_genomic_lines[0].Entry_type;
            string toxicity_association = sameSideEffect_sameDrug_selected_genomic_lines[0].Toxicity_association;
            Ontology_type_enum[] ontologies = Get_all_ontologies_from_selected_variants(sameSideEffect_sameDrug_selected_genomic_lines);
            Ontology_type_enum ontology;
            ontologies = Get_ontologies_in_correct_order(ontologies);
            int ontologies_length = ontologies.Length;
            Lincs_vcf_genomic_data_line_class[] ontology_selected_genomic_lines;
            List<Lincs_vcf_genomic_data_line_class> current_ontology_selected_genomic_lines = new List<Lincs_vcf_genomic_data_line_class>();
            Lincs_vcf_genomic_data_line_class ontology_selected_genomic_line;
            int ontology_selected_lines_length;
            sameSideEffect_sameDrug_selected_genomic_lines = Lincs_vcf_genomic_data_line_class.Order_by_ontology(sameSideEffect_sameDrug_selected_genomic_lines);
            Dictionary<string, bool> rsIdentifier_counted_dict = new Dictionary<string, bool>();
            Dictionary<string, bool> current_scps_dict = new Dictionary<string, bool>();
            int current_all_variants_count;
            int current_variants_counts_if_not_covered_by_lower_level_scps;
            Lincs_genomic_variant_count_foreach_drug_line_class genomic_variant_count_line;
            List<Lincs_genomic_variant_count_foreach_drug_line_class> genomic_variant_count_list = new List<Lincs_genomic_variant_count_foreach_drug_line_class>();
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                ontology = ontologies[indexO];
                ontology_selected_genomic_lines = Get_all_genomic_data_lines_with_given_ontology(sameSideEffect_sameDrug_selected_genomic_lines, ontology);
                ontology_selected_lines_length = ontology_selected_genomic_lines.Length;
                ontology_selected_genomic_lines = Lincs_vcf_genomic_data_line_class.Order_by_rsIdentifer(ontology_selected_genomic_lines);
                current_all_variants_count = 0;
                current_variants_counts_if_not_covered_by_lower_level_scps = 0;
                current_scps_dict.Clear();
                for (int indexOSelected = 0; indexOSelected < ontology_selected_lines_length; indexOSelected++)
                {
                    ontology_selected_genomic_line = ontology_selected_genomic_lines[indexOSelected];
                    if (!ontology_selected_genomic_line.Side_effect.Equals(sideEffect)) { throw new Exception(); }
                    if (!ontology_selected_genomic_line.Toxicity_association.Equals(toxicity_association)) { throw new Exception(); }
                    if (!ontology_selected_genomic_line.Drug_regulating_scp.Equals(drug)) { throw new Exception(); }
                    if (!ontology_selected_genomic_line.Entry_type.Equals(entryType)) { throw new Exception(); }
                    if ((indexOSelected == 0)
                        || (!ontology_selected_genomic_line.Rs_identifier.Equals(ontology_selected_genomic_lines[indexOSelected - 1].Rs_identifier)))
                    {
                        if (!rsIdentifier_counted_dict.ContainsKey(ontology_selected_genomic_line.Rs_identifier))
                        {
                            rsIdentifier_counted_dict.Add(ontology_selected_genomic_line.Rs_identifier, true);
                            current_variants_counts_if_not_covered_by_lower_level_scps++;
                        }
                        current_all_variants_count++;
                    }
                    if (!current_scps_dict.ContainsKey(ontology_selected_genomic_line.Scp))
                    {
                        current_scps_dict.Add(ontology_selected_genomic_line.Scp, true);
                    }
                }
                genomic_variant_count_line = new Lincs_genomic_variant_count_foreach_drug_line_class();
                genomic_variant_count_line.Ontology = ontology;
                genomic_variant_count_line.Drug = (string)drug.Clone();
                genomic_variant_count_line.EntryType = entryType;
                genomic_variant_count_line.SCPs_count = current_scps_dict.Keys.Count;
                genomic_variant_count_line.Count_of_all_variants_mapping_to_sameLevel_scps = current_all_variants_count;
                genomic_variant_count_line.Count_of_variants_that_are_not_covered_by_lower_level_scps = current_variants_counts_if_not_covered_by_lower_level_scps;
                genomic_variant_count_line.Side_effect = (string)sideEffect.Clone();
                genomic_variant_count_line.Toxicity_association = (string)toxicity_association.Clone();
                genomic_variant_count_list.Add(genomic_variant_count_line);
            }
            return genomic_variant_count_list.ToArray();
        }

        private Lincs_genomic_variant_count_foreach_drug_line_class[] Count_selected_variants_in_input_array(Lincs_vcf_genomic_data_line_class[] genomic_variants)
        {
            genomic_variants = Lincs_vcf_genomic_data_line_class.Order_by_sideEffect_toxicityAssociation_drugRegulatingScp_entryType(genomic_variants);
            int selected_variants_length = genomic_variants.Length;
            Lincs_vcf_genomic_data_line_class selected_variant_line;
            List<Lincs_vcf_genomic_data_line_class> sameSideEffect_sameToxicityAssociation_sameDrugRegulatingScp_sameEntryType_variants = new List<Lincs_vcf_genomic_data_line_class>();
            Lincs_genomic_variant_count_foreach_drug_line_class[] new_variant_count_lines;
            List<Lincs_genomic_variant_count_foreach_drug_line_class> variant_counts = new List<Lincs_genomic_variant_count_foreach_drug_line_class>();
            for (int indexSelected =0; indexSelected<selected_variants_length;indexSelected++)
            {
                selected_variant_line = genomic_variants[indexSelected];
                if (  (indexSelected==0)
                    || (!selected_variant_line.Side_effect.Equals(genomic_variants[indexSelected - 1].Side_effect))
                    || (!selected_variant_line.Toxicity_association.Equals(genomic_variants[indexSelected - 1].Toxicity_association))
                    || (!selected_variant_line.Drug_regulating_scp.Equals(genomic_variants[indexSelected - 1].Drug_regulating_scp))
                    || (!selected_variant_line.Entry_type.Equals(genomic_variants[indexSelected - 1].Entry_type)))
                {
                    sameSideEffect_sameToxicityAssociation_sameDrugRegulatingScp_sameEntryType_variants.Clear();
                }
                sameSideEffect_sameToxicityAssociation_sameDrugRegulatingScp_sameEntryType_variants.Add(selected_variant_line);
                if ((indexSelected == selected_variants_length-1)
                    || (!selected_variant_line.Side_effect.Equals(genomic_variants[indexSelected + 1].Side_effect))
                    || (!selected_variant_line.Toxicity_association.Equals(genomic_variants[indexSelected + 1].Toxicity_association))
                    || (!selected_variant_line.Drug_regulating_scp.Equals(genomic_variants[indexSelected + 1].Drug_regulating_scp))
                    || (!selected_variant_line.Entry_type.Equals(genomic_variants[indexSelected + 1].Entry_type)))
                {
                    new_variant_count_lines = Count_selected_variants_from_sameSideEffect_sameToxicityAssociation_sameDrugRegulatingScp_sameEntryType_selected_variants(sameSideEffect_sameToxicityAssociation_sameDrugRegulatingScp_sameEntryType_variants.ToArray());
                    variant_counts.AddRange(new_variant_count_lines);
                }
            }
            return variant_counts.ToArray();
        }

        private Lincs_vcf_genomic_data_line_class[] Get_selected_variants_assigned_to_cardiotoxic_and_noncardiotoxic_tks()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, bool> cardiotoxic_tki_dict = drug_legend.Get_drug_isCardiotoxicTKI_dict();
            Dictionary<string, bool> noncardiotoxic_tki_dict = drug_legend.Get_drug_isNonCardiotoxicTKI_dict();
            Lincs_vcf_genomic_data_line_class add_genomic_variant_line;
            List<Lincs_vcf_genomic_data_line_class> tki_genomic_variants = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_line in this.Selected_variants)
            {
                if (cardiotoxic_tki_dict.ContainsKey(genomic_line.Drug_regulating_scp))
                {
                    add_genomic_variant_line = genomic_line.Deep_copy();
                    add_genomic_variant_line.Drug_regulating_scp = (string)Lincs_toxicity_classifications_definition_class.Cardiotoxic_tkis.Clone();
                    tki_genomic_variants.Add(add_genomic_variant_line);
                }
                if (noncardiotoxic_tki_dict.ContainsKey(genomic_line.Drug_regulating_scp))
                {
                    add_genomic_variant_line = genomic_line.Deep_copy();
                    add_genomic_variant_line.Drug_regulating_scp = (string)Lincs_toxicity_classifications_definition_class.Noncardiotoxic_tkis.Clone();
                    tki_genomic_variants.Add(add_genomic_variant_line);
                }
            }
            return tki_genomic_variants.ToArray();
        }

        private Lincs_vcf_genomic_data_line_class[] Deep_copy_and_set_input_entryTypes_to_diffrna(Lincs_vcf_genomic_data_line_class[] input_genomic_variants)
        {
            int input_length = input_genomic_variants.Length;
            Lincs_vcf_genomic_data_line_class genomic_variant_line;
            Lincs_vcf_genomic_data_line_class[] deep_copy_variants = new Lincs_vcf_genomic_data_line_class[input_length];
            for (int indexInput=0; indexInput<input_length; indexInput++)
            {
                genomic_variant_line = input_genomic_variants[indexInput].Deep_copy();
                genomic_variant_line.Entry_type = DE_entry_enum.Diffrna;
                deep_copy_variants[indexInput] = genomic_variant_line;
            }
            return deep_copy_variants;
        }


        private void Count_selected_variants()
        {
            Lincs_vcf_genomic_data_line_class[] submit_variants;
            List<Lincs_genomic_variant_count_foreach_drug_line_class> final_variant_counts = new List<Lincs_genomic_variant_count_foreach_drug_line_class>();
            Lincs_genomic_variant_count_foreach_drug_line_class[] add_variants;
            submit_variants = Deep_copy_and_set_input_entryTypes_to_diffrna(this.Selected_variants);
            add_variants = Count_selected_variants_in_input_array(submit_variants);
            final_variant_counts.AddRange(add_variants);
            Lincs_vcf_genomic_data_line_class[] tki_genomic_lines = Get_selected_variants_assigned_to_cardiotoxic_and_noncardiotoxic_tks();
            submit_variants = Deep_copy_and_set_input_entryTypes_to_diffrna(tki_genomic_lines);
            add_variants = Count_selected_variants_in_input_array(submit_variants);
            final_variant_counts.AddRange(add_variants);
            this.Variants_count = final_variant_counts.ToArray();
        }

        private void Replace_drug_by_fullDrugName_and_add_drug_type()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, string> drug_fullName_dict = drug_legend.Get_drug_drugFullName_dict();
            Dictionary<string, Drug_type_enum> drug_drugType_dict = drug_legend.Get_drug_drugType_dictionary();
            Dictionary<string, string> drug_isCardiotoxic_dict = drug_legend.Get_drug_isCardiotoxic_dictionary();
            foreach (Lincs_vcf_genomic_data_line_class genomics_data_line in this.Selected_variants)
            {
                genomics_data_line.Drug_type = drug_drugType_dict[genomics_data_line.Drug_regulating_scp];
                genomics_data_line.Drug_is_cardiotoxic = (string)drug_isCardiotoxic_dict[genomics_data_line.Drug_regulating_scp].Clone();
                genomics_data_line.Drug_regulating_scp = (string)drug_fullName_dict[genomics_data_line.Drug_regulating_scp].Clone();
            }
            //foreach (Lincs_genomic_variant_count_foreach_drug_line_class variant_count_line in this.Variants_count)
            //{
            //    variant_count_line.Drug_type = drug_drugType_dict[variant_count_line.Drug];
            //    variant_count_line.Drug = (string)drug_fullName_dict[variant_count_line.Drug].Clone();
            //}
        }

        public void Replace_drugAbbreviation_by_fullDrugName_and_add_drugType()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            drug_legend.Add_missing_cardiotoxicity_from_faers();
            Dictionary<string,string> fullDrugName_drug_dict = drug_legend.Get_drugFullName_drug_dict();
            Dictionary<string, Drug_type_enum> drug_drugType_dict = drug_legend.Get_drug_drugType_dictionary();
            Dictionary<string, string> drug_fullDrugName_dict = drug_legend.Get_drug_drugFullName_dict();
            Dictionary<string, string> drug_isCardiotoxic_dict = drug_legend.Get_drug_isCardiotoxic_dictionary();
            string drug;
            foreach (Lincs_genomic_variant_count_foreach_drug_line_class genomic_line in this.Variants_count)
            {
                drug = genomic_line.Drug;
                if (!drug_isCardiotoxic_dict.ContainsKey(drug))
                {
                    drug = fullDrugName_drug_dict[genomic_line.Drug];
                }
                genomic_line.Drug_type = drug_drugType_dict[drug];
                genomic_line.Drug_is_cardiotoxic = drug_isCardiotoxic_dict[drug];
                genomic_line.Drug = (string)drug_fullDrugName_dict[drug].Clone();
            }
        }

        public void Generate(Lincs_vcf_genomic_data_class genomic_data, Enrichment2018_results_class enrichment_results, Lincs_scp_summary_after_roc_class scp_summary)
        {
            Collect_variant_lines_per_pathway(genomic_data, enrichment_results, scp_summary);
            Count_selected_variants();
            Replace_drug_by_fullDrugName_and_add_drug_type();
        }

        public void Keep_only_selected_ontology_in_selected_variants(Ontology_type_enum ontology)
        {
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_data_line in this.Selected_variants)
            {
                if (genomic_data_line.Ontology.Equals(ontology))
                {
                    keep.Add(genomic_data_line);
                }
            }
            this.Selected_variants = keep.ToArray();
        }

        public void Keep_only_cardiotoxic_drugs()
        {
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            foreach (Lincs_vcf_genomic_data_line_class genomic_data_line in this.Selected_variants)
            {
                if (genomic_data_line.Drug_is_cardiotoxic.Equals("Yes"))
                {
                    keep.Add(genomic_data_line);
                }
            }
            this.Selected_variants = keep.ToArray();
        }
        public void Merge_same_lines_with_different_drugs()
        {
            int genomic_lines_length = this.Selected_variants.Length;
            Lincs_vcf_genomic_data_line_class genomic_line;
            this.Selected_variants = this.Selected_variants.OrderBy(l => l.Scp).ThenBy(l=>l.Drug_target_symbol).ThenBy(l=>l.Entry_type).ThenBy(l => l.Scp_gene).ThenBy(l=>l.Rs_identifier).ThenBy(l => l.Drug_regulating_scp).ToArray();
            List<Lincs_vcf_genomic_data_line_class> keep = new List<Lincs_vcf_genomic_data_line_class>();
            StringBuilder drug_sb = new StringBuilder();
            for (int indexG=0; indexG<genomic_lines_length;indexG++)
            {
                genomic_line = this.Selected_variants[indexG];
                if (  (indexG==0)
                    || (!genomic_line.Scp.Equals(this.Selected_variants[indexG - 1].Scp))
                    || (!genomic_line.Drug_target_symbol.Equals(this.Selected_variants[indexG-1].Drug_target_symbol))
                    || (!genomic_line.Scp_gene.Equals(this.Selected_variants[indexG - 1].Scp_gene))
                    || (!genomic_line.Rs_identifier.Equals(this.Selected_variants[indexG - 1].Rs_identifier))
                    || (!genomic_line.Entry_type.Equals(this.Selected_variants[indexG - 1].Entry_type))
                    || (!genomic_line.Scp_gene.Equals(this.Selected_variants[indexG - 1].Scp_gene)))
                {
                    drug_sb.Clear();
                }
                if (drug_sb.Length > 0) { drug_sb.AppendFormat(";"); }
                drug_sb.AppendFormat(genomic_line.Drug_regulating_scp);
                if    (   (indexG != 0)
                       && (genomic_line.Scp.Equals(this.Selected_variants[indexG - 1].Scp))
                       && (genomic_line.Drug_target_symbol.Equals(this.Selected_variants[indexG - 1].Drug_target_symbol))
                       && (genomic_line.Scp_gene.Equals(this.Selected_variants[indexG - 1].Scp_gene))
                       && (genomic_line.Rs_identifier.Equals(this.Selected_variants[indexG - 1].Rs_identifier))
                       && (genomic_line.Entry_type.Equals(this.Selected_variants[indexG - 1].Entry_type))
                       && (genomic_line.Gene_symbol.Equals(this.Selected_variants[indexG - 1].Gene_symbol))
                       && (   (!genomic_line.Gtex_atrial_eQTL.Equals(this.Selected_variants[indexG - 1].Gtex_atrial_eQTL))
                           || (!genomic_line.Gtex_atrial_sQTL.Equals(this.Selected_variants[indexG - 1].Gtex_atrial_sQTL))
                           || (!genomic_line.Gtex_ventricular_sQTL.Equals(this.Selected_variants[indexG - 1].Gtex_ventricular_sQTL))
                           || (!genomic_line.Quality_aq.Equals(this.Selected_variants[indexG - 1].Quality_aq))
                           || (!genomic_line.Cadd_phred.Equals(this.Selected_variants[indexG - 1].Cadd_phred))
                           || (!genomic_line.Variant_location.Equals(this.Selected_variants[indexG - 1].Variant_location))
                           || (!genomic_line.Variant_location_description.Equals(this.Selected_variants[indexG - 1].Variant_location_description))
                           || (!genomic_line.Start.Equals(this.Selected_variants[indexG - 1].Start))
                           || (!genomic_line.End.Equals(this.Selected_variants[indexG - 1].End))
                           || (!genomic_line.Chrom.Equals(this.Selected_variants[indexG - 1].Chrom))
                           || (!genomic_line.Cell_line_allele_depth_AD.Equals(this.Selected_variants[indexG - 1].Cell_line_allele_depth_AD))
                           || (!genomic_line.Cell_line_approximate_read_depth_DP.Equals(this.Selected_variants[indexG - 1].Cell_line_approximate_read_depth_DP))
                           || (!genomic_line.Cell_line_conditional_genotype_quality.Equals(this.Selected_variants[indexG - 1].Cell_line_conditional_genotype_quality))
                           || (!genomic_line.Cell_line_genotype_GT.Equals(this.Selected_variants[indexG - 1].Cell_line_genotype_GT))
                          )
                      )
                {
                    throw new Exception();
                }
                if ((indexG == genomic_lines_length-1)
                    || (!genomic_line.Scp.Equals(this.Selected_variants[indexG + 1].Scp))
                    || (!genomic_line.Drug_target_symbol.Equals(this.Selected_variants[indexG + 1].Drug_target_symbol))
                    || (!genomic_line.Scp_gene.Equals(this.Selected_variants[indexG + 1].Scp_gene))
                    || (!genomic_line.Rs_identifier.Equals(this.Selected_variants[indexG + 1].Rs_identifier))
                    || (!genomic_line.Entry_type.Equals(this.Selected_variants[indexG + 1].Entry_type))
                    || (!genomic_line.Scp_gene.Equals(this.Selected_variants[indexG + 1].Scp_gene)))
                {
                    genomic_line.Drug_regulating_scp = drug_sb.ToString();
                    keep.Add(genomic_line);
                }
            }
            this.Selected_variants = keep.ToArray();
        }

        public NetworkBasis_class Generate_drug_scp_gene_network(params string[] drugs)
        {
            this.Selected_variants = this.Selected_variants.OrderBy(l => l.Entry_type).ThenBy(l => l.Gene_symbol).ThenBy(l => l.Rs_identifier).ToArray();
            drugs = drugs.Distinct().ToArray();
            Dictionary<string, bool> keep_drug_dict = new Dictionary<string, bool>();
            foreach (string drug in drugs)
            {
                keep_drug_dict.Add(drug, true);
            }

            SigNWBasis_line_class sigNW_line;
            List<SigNWBasis_line_class> sigNW_list = new List<SigNWBasis_line_class>();
            int selected_variants_length = this.Selected_variants.Length;
            Lincs_vcf_genomic_data_line_class selected_variant_line;
            int current_rsIdentifier_count = -1;
            Dictionary<string, Dictionary<string, bool>> current_drug_scp_dict = new Dictionary<string, Dictionary<string, bool>>();
            Dictionary<string, bool> scp_dict = new Dictionary<string, bool>();
            string[] currentGene_drugs;
            string currentGene_drug;
            int currentGene_drugs_length;
            string[] currentGeneDrugs_scps;
            string currentGeneDrugs_scp;
            int currentGeneDrugs_scps_length;
            Dictionary<string, List<System.Drawing.Color>> node_colors_dict = new Dictionary<string, List<System.Drawing.Color>>();
            for (int indexSV = 0; indexSV < selected_variants_length; indexSV++)
            {
                selected_variant_line = this.Selected_variants[indexSV];
                if ((indexSV == 0)
                    || (!selected_variant_line.Entry_type.Equals(this.Selected_variants[indexSV - 1].Entry_type))
                    || (!selected_variant_line.Gene_symbol.Equals(this.Selected_variants[indexSV - 1].Gene_symbol)))
                {
                    current_rsIdentifier_count = 0;
                    current_drug_scp_dict.Clear();
                }
                if ((indexSV == 0)
                    || (!selected_variant_line.Rs_identifier.Equals(this.Selected_variants[indexSV - 1].Rs_identifier))
                    || (!selected_variant_line.Entry_type.Equals(this.Selected_variants[indexSV - 1].Entry_type))
                    || (!selected_variant_line.Gene_symbol.Equals(this.Selected_variants[indexSV - 1].Gene_symbol)))
                {
                    current_rsIdentifier_count++;
                }
                if (!current_drug_scp_dict.ContainsKey(selected_variant_line.Drug_regulating_scp))
                {
                    current_drug_scp_dict.Add(selected_variant_line.Drug_regulating_scp, new Dictionary<string, bool>());
                }
                if (!current_drug_scp_dict[selected_variant_line.Drug_regulating_scp].ContainsKey(selected_variant_line.Scp))
                {
                    current_drug_scp_dict[selected_variant_line.Drug_regulating_scp].Add(selected_variant_line.Scp, true);
                }
                if (((indexSV == selected_variants_length - 1)
                        || (!selected_variant_line.Entry_type.Equals(this.Selected_variants[indexSV + 1].Entry_type))
                        || (!selected_variant_line.Gene_symbol.Equals(this.Selected_variants[indexSV + 1].Gene_symbol))))
                {
                    currentGene_drugs = current_drug_scp_dict.Keys.ToArray();
                    currentGene_drugs_length = currentGene_drugs.Length;
                    for (int indexCGD = 0; indexCGD < currentGene_drugs_length; indexCGD++)
                    {
                        currentGene_drug = currentGene_drugs[indexCGD];
                        if (keep_drug_dict.ContainsKey(currentGene_drug))
                        {
                            scp_dict = current_drug_scp_dict[currentGene_drug];
                            currentGeneDrugs_scps = scp_dict.Keys.ToArray();
                            currentGeneDrugs_scps_length = currentGeneDrugs_scps.Length;
                            for (int indexGDS = 0; indexGDS < currentGeneDrugs_scps_length; indexGDS++)
                            {
                                currentGeneDrugs_scp = currentGeneDrugs_scps[indexGDS];
                                sigNW_line = new SigNWBasis_line_class();
                                sigNW_line.Source = (string)currentGene_drug.Clone();
                                sigNW_line.Target = (string)currentGeneDrugs_scp.Clone();
                                sigNW_list.Add(sigNW_line);
                                sigNW_line = new SigNWBasis_line_class();
                                sigNW_line.Source = (string)currentGeneDrugs_scp.Clone();
                                sigNW_line.Target = (string)selected_variant_line.Gene_symbol + " (" + current_rsIdentifier_count + ")";
                                sigNW_list.Add(sigNW_line);
                                if (!node_colors_dict.ContainsKey(currentGene_drug))
                                {
                                    node_colors_dict.Add(currentGene_drug, new List<System.Drawing.Color>());
                                    node_colors_dict[currentGene_drug].Add(System.Drawing.Color.White);
                                }
                                if (!node_colors_dict.ContainsKey(currentGeneDrugs_scp))
                                {
                                    node_colors_dict.Add(currentGeneDrugs_scp, new List<System.Drawing.Color>());
                                }
                                if (!node_colors_dict.ContainsKey(sigNW_line.Target))
                                {
                                    node_colors_dict.Add(sigNW_line.Target, new List<System.Drawing.Color>());
                                }
                                switch (selected_variant_line.Entry_type)
                                {
                                    case DE_entry_enum.Diffrna_up:
                                        node_colors_dict[currentGeneDrugs_scp].Add(System.Drawing.Color.Orange);
                                        node_colors_dict[sigNW_line.Target].Add(System.Drawing.Color.Orange);
                                        break;
                                    case DE_entry_enum.Diffrna_down:
                                        node_colors_dict[currentGeneDrugs_scp].Add(System.Drawing.Color.CornflowerBlue);
                                        node_colors_dict[sigNW_line.Target].Add(System.Drawing.Color.CornflowerBlue);
                                        break;
                                    default:
                                        throw new Exception();
                                }
                                node_colors_dict[currentGeneDrugs_scp] = node_colors_dict[currentGeneDrugs_scp].Distinct().ToList();
                                node_colors_dict[sigNW_line.Target] = node_colors_dict[sigNW_line.Target].Distinct().ToList();
                            }
                        }
                    }
                }
            }
            if (sigNW_list.Count == 0) { throw new Exception(); }
            SigNWBasis_class<SigNWBasis_line_class> sigNW = new Network.SigNWBasis_class<SigNWBasis_line_class>();
            sigNW.Generate_sigNW_from_sigNW_list(sigNW_list, Network_direction_type_enum.Directed_forward);
            NetworkBasis_class nw = new NetworkBasis_class();
            nw.Generate_network_from_sigNW(sigNW);
            nw.UniqueNodes.Add_selected_color_to_nodes(node_colors_dict);
            return nw;
        }

        public NetworkBasis_class Generate_drug_scp_gene_network_for_selected_scps(DE_entry_enum[] entryTypes, params string[] scps)
        {
            this.Selected_variants = this.Selected_variants.OrderBy(l => l.Entry_type).ThenBy(l => l.Gene_symbol).ThenBy(l => l.Rs_identifier).ToArray();
            scps = scps.Distinct().ToArray();
            Dictionary<string, bool> keep_scp_dict = new Dictionary<string, bool>();
            foreach (string scp in scps)
            {
                keep_scp_dict.Add(scp, true);
            }
            entryTypes = entryTypes.Distinct().ToArray();
            Dictionary<DE_entry_enum, bool> keep_entryType_dict = new Dictionary<DE_entry_enum, bool>();
            foreach (DE_entry_enum entryType in entryTypes)
            {
                keep_entryType_dict.Add(entryType, true);
            }

            SigNWBasis_line_class sigNW_line;
            List<SigNWBasis_line_class> sigNW_list = new List<SigNWBasis_line_class>();
            int selected_variants_length = this.Selected_variants.Length;
            Lincs_vcf_genomic_data_line_class selected_variant_line;
            int current_rsIdentifier_count = -1;
            Dictionary<string, Dictionary<string, bool>> current_scp_drug_dict = new Dictionary<string, Dictionary<string, bool>>();
            Dictionary<string, bool> drug_dict = new Dictionary<string, bool>();
            string[] currentGene_scps;
            string currentGene_scp;
            int currentGene_scps_length;
            string[] currentGeneScp_drugs;
            string currentGeneScp_drug;
            int currentGeneScp_drugs_length;
            Dictionary<string, List<System.Drawing.Color>> node_colors_dict = new Dictionary<string, List<System.Drawing.Color>>();
            Dictionary<string, Dictionary<string, bool>> drug_scp_added_dict = new Dictionary<string, Dictionary<string, bool>>();
            for (int indexSV = 0; indexSV < selected_variants_length; indexSV++)
            {
                selected_variant_line = this.Selected_variants[indexSV];
                if ((indexSV == 0)
                    || (!selected_variant_line.Entry_type.Equals(this.Selected_variants[indexSV - 1].Entry_type))
                    || (!selected_variant_line.Gene_symbol.Equals(this.Selected_variants[indexSV - 1].Gene_symbol)))
                {
                    current_rsIdentifier_count = 0;
                    current_scp_drug_dict.Clear();
                }
                if ((indexSV == 0)
                    || (!selected_variant_line.Rs_identifier.Equals(this.Selected_variants[indexSV - 1].Rs_identifier))
                    || (!selected_variant_line.Entry_type.Equals(this.Selected_variants[indexSV - 1].Entry_type))
                    || (!selected_variant_line.Gene_symbol.Equals(this.Selected_variants[indexSV - 1].Gene_symbol)))
                {
                    current_rsIdentifier_count++;
                }
                if (!current_scp_drug_dict.ContainsKey(selected_variant_line.Scp))
                {
                    current_scp_drug_dict.Add(selected_variant_line.Scp, new Dictionary<string, bool>());
                }
                if (!current_scp_drug_dict[selected_variant_line.Scp].ContainsKey(selected_variant_line.Drug_regulating_scp))
                {
                    current_scp_drug_dict[selected_variant_line.Scp].Add(selected_variant_line.Drug_regulating_scp, true);
                }
                if (   (   (indexSV == selected_variants_length - 1)
                        || (!selected_variant_line.Entry_type.Equals(this.Selected_variants[indexSV + 1].Entry_type))
                        || (!selected_variant_line.Gene_symbol.Equals(this.Selected_variants[indexSV + 1].Gene_symbol)))
                    && (keep_entryType_dict.ContainsKey(selected_variant_line.Entry_type))
                   )
                {
                    currentGene_scps = current_scp_drug_dict.Keys.ToArray();
                    currentGene_scps_length = currentGene_scps.Length;
                    for (int indexCGScp = 0; indexCGScp < currentGene_scps_length; indexCGScp++)
                    {
                        currentGene_scp = currentGene_scps[indexCGScp];
                        if (keep_scp_dict.ContainsKey(currentGene_scp))
                        {
                            drug_dict = current_scp_drug_dict[currentGene_scp];
                            currentGeneScp_drugs = drug_dict.Keys.ToArray();
                            currentGeneScp_drugs_length = currentGeneScp_drugs.Length;
                            for (int indexGDS = 0; indexGDS < currentGeneScp_drugs_length; indexGDS++)
                            {
                                currentGeneScp_drug = currentGeneScp_drugs[indexGDS];
                                if (!drug_scp_added_dict.ContainsKey(currentGeneScp_drug))
                                {
                                    drug_scp_added_dict.Add(currentGeneScp_drug, new Dictionary<string, bool>());
                                }
                                if (!drug_scp_added_dict[currentGeneScp_drug].ContainsKey(currentGene_scp))
                                {
                                    sigNW_line = new SigNWBasis_line_class();
                                    sigNW_line.Source = (string)currentGeneScp_drug.Clone();
                                    sigNW_line.Target = (string)currentGene_scp.Clone();
                                    sigNW_list.Add(sigNW_line);
                                    drug_scp_added_dict[currentGeneScp_drug].Add(currentGene_scp, true);
                                }
                                if (!node_colors_dict.ContainsKey(currentGeneScp_drug))
                                {
                                    node_colors_dict.Add(currentGeneScp_drug, new List<System.Drawing.Color>());
                                    node_colors_dict[currentGeneScp_drug].Add(System.Drawing.Color.White);
                                }
                            }
                            sigNW_line = new SigNWBasis_line_class();
                                sigNW_line.Source = (string)currentGene_scp.Clone();
                                sigNW_line.Target = (string)selected_variant_line.Gene_symbol + " (" + current_rsIdentifier_count + ")";
                                sigNW_list.Add(sigNW_line);
                                if (!node_colors_dict.ContainsKey(currentGene_scp))
                                {
                                    node_colors_dict.Add(currentGene_scp, new List<System.Drawing.Color>());
                                }
                                if (!node_colors_dict.ContainsKey(sigNW_line.Target))
                                {
                                    node_colors_dict.Add(sigNW_line.Target, new List<System.Drawing.Color>());
                                }
                                switch (selected_variant_line.Entry_type)
                                {
                                    case DE_entry_enum.Diffrna_up:
                                        node_colors_dict[currentGene_scp].Add(System.Drawing.Color.Orange);
                                        node_colors_dict[sigNW_line.Target].Add(System.Drawing.Color.Orange);
                                        break;
                                    case DE_entry_enum.Diffrna_down:
                                        node_colors_dict[currentGene_scp].Add(System.Drawing.Color.CornflowerBlue);
                                        node_colors_dict[sigNW_line.Target].Add(System.Drawing.Color.CornflowerBlue);
                                        break;
                                    default:
                                        throw new Exception();
                                }
                                node_colors_dict[currentGene_scp] = node_colors_dict[currentGene_scp].Distinct().ToList();
                                node_colors_dict[sigNW_line.Target] = node_colors_dict[sigNW_line.Target].Distinct().ToList();
                            
                        }
                    }
                }
            }
            if (sigNW_list.Count == 0) { throw new Exception(); }
            SigNWBasis_class<SigNWBasis_line_class> sigNW = new Network.SigNWBasis_class<SigNWBasis_line_class>();
            sigNW.Generate_sigNW_from_sigNW_list(sigNW_list, Network_direction_type_enum.Directed_forward);
            NetworkBasis_class nw = new NetworkBasis_class();
            nw.Generate_network_from_sigNW(sigNW);
            nw.UniqueNodes.Add_selected_color_to_nodes(node_colors_dict);
            return nw;
        }

        public void Write_scp_genomic_variants_supplTable(string subdirectory, string fileName)
        {
            Lincs_vcf_genomic_data_mapped_to_scps_supplTable_readWriteOptions_class scp_readWriteOptions = new Lincs_vcf_genomic_data_mapped_to_scps_supplTable_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Selected_variants, scp_readWriteOptions);
        }
        public void Write_scp_genomic_variants_for_website(string subdirectory, string fileName)
        {
            Lincs_vcf_genomic_data_mapped_to_scps_forWebsite_readWriteOptions_class scp_readWriteOptions = new Lincs_vcf_genomic_data_mapped_to_scps_forWebsite_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Selected_variants, scp_readWriteOptions);
        }
        public void Write_scp_genomic_variants(string subdirectory, string fileName)
        {
            Lincs_vcf_genomic_data_mapped_to_scps_readWriteOptions_class scp_readWriteOptions = new Lincs_vcf_genomic_data_mapped_to_scps_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Selected_variants, scp_readWriteOptions);
        }

        public void Write_variant_counts(string subdirectory, string fileName)
        {
            Lincs_genomics_variant_count_foreach_drug_readWriteOptions readWriteOptions = new Lincs_genomics_variant_count_foreach_drug_readWriteOptions(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Variants_count, readWriteOptions);
        }
        public void Read_variant_counts(string subdirectory, string fileName)
        {
            Lincs_genomics_variant_count_foreach_drug_readWriteOptions readWriteOptions = new Lincs_genomics_variant_count_foreach_drug_readWriteOptions(subdirectory, fileName);
            this.Variants_count = ReadWriteClass.ReadRawData_and_FillArray<Lincs_genomic_variant_count_foreach_drug_line_class>(readWriteOptions);
        }
        public void Read_scp_genomic_variants(string subdirectory, string fileName)
        {
            Lincs_vcf_genomic_data_mapped_to_scps_readWriteOptions_class scp_readWriteOptions = new Lincs_vcf_genomic_data_mapped_to_scps_readWriteOptions_class(subdirectory, fileName);
            this.Selected_variants = ReadWriteClass.ReadRawData_and_FillArray<Lincs_vcf_genomic_data_line_class>(scp_readWriteOptions);
        }

        public Lincs_genomic_variant_counts_foreach_drug_class Deep_copy()
        {
            Lincs_genomic_variant_counts_foreach_drug_class copy = (Lincs_genomic_variant_counts_foreach_drug_class)this.MemberwiseClone();
            int variant_counts_length = this.Variants_count.Length;
            copy.Variants_count = new Lincs_genomic_variant_count_foreach_drug_line_class[variant_counts_length];
            for (int indexV=0; indexV<variant_counts_length; indexV++)
            {
                copy.Variants_count[indexV] = this.Variants_count[indexV].Deep_copy();
            }
            int selected_variants_length = this.Selected_variants.Length;
            copy.Selected_variants = new Lincs_vcf_genomic_data_line_class[selected_variants_length];
            for (int indexS =0; indexS<selected_variants_length; indexS++)
            {
                copy.Selected_variants[indexS] = this.Selected_variants[indexS].Deep_copy();
            }
            return copy;
        }
    }
}
