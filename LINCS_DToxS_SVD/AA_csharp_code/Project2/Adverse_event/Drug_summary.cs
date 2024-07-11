using System;
using System.Collections.Generic;
using System.Linq;
using Common_classes;
using Highthroughput_data;
using ReadWrite;
using Ontologies_and_GTEx;
using Enrichment;
using FAERS_analysis;
using System.Text;

namespace Adverse_event
{
    class Drug_summary_line_class
    {
        public string DrugName { get; set; }
        public string Drug_ID { get; set; }
        public string BrandName { get; set; }
        public string ApprovalDate { get; set; }
        public Drug_type_enum Drug_type { get; set; }
        public string[] DrugHumanTargetProteins { get; set; }
        public string Is_cardiotoxic { get; set; }

        public string ReadWrite_drugHumanTargetProteins
        {
            get { return ReadWriteClass.Get_writeLine_from_array<string>(DrugHumanTargetProteins, Drug_summary_readWriteOptions_class.Array_delimiter); }
            set { DrugHumanTargetProteins = ReadWriteClass.Get_array_from_readLine<string>(value, Drug_summary_readWriteOptions_class.Array_delimiter); }
        }

        public string Drug_class_for_website
        {
            get
            {
                return Lincs_website_conversion_class.Get_drugClass_for_website(Drug_type);
            }
        }

        public Drug_summary_line_class()
        {
            this.DrugHumanTargetProteins = new string[0];
            Is_cardiotoxic = "";
        }

        public Drug_summary_line_class Deep_copy()
        {
            Drug_summary_line_class copy = (Drug_summary_line_class)this.MemberwiseClone();
            copy.DrugName = (string)this.DrugName.Clone();
            copy.BrandName = (string)this.BrandName.Clone();
            copy.ApprovalDate = (string)this.ApprovalDate.Clone();
            copy.Drug_ID = (string)this.Drug_ID.Clone();
            copy.Is_cardiotoxic = (string)this.Is_cardiotoxic.Clone();
            return copy;
        }
    }

    class Drug_summary_inputReadOptions_class : ReadWriteOptions_base
    {
        public Drug_summary_inputReadOptions_class()
        {
            this.File = Global_directory_class.Metadata_directory + "LincsDrugDosage_harmonized_names.txt";
            this.Key_propertyNames = new string[] { "DrugName", "Drug_ID", "BrandName" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Drug_summary_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Drug_summary_readWriteOptions_class(string subdirectory, string file_name)
        {
            string directory = Global_directory_class.Results_directory + subdirectory;
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + file_name;
            this.Key_propertyNames = new string[] { "DrugName", "BrandName", "ApprovalDate", "Drug_type", "ReadWrite_drugHumanTargetProteins" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Comma };
            this.LineDelimiters = new char[] { Global_class.Comma };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Drug_summary_adverseEvent_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Drug_summary_adverseEvent_readWriteOptions_class(string subdirectory, string file_name)
        {
            string directory = Global_directory_class.Results_directory + subdirectory;
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + file_name;
            this.Key_propertyNames = new string[] { "DrugName", "Is_cardiotoxic", "Drug_class_for_website", "BrandName", "ApprovalDate", "ReadWrite_drugHumanTargetProteins" };
            this.Key_columnNames = new string[] { Lincs_website_conversion_class.Label_drugName,
                                                  Lincs_website_conversion_class.Label_is_cardiotoxic,
                                                  Lincs_website_conversion_class.Label_drug_class,
                                                  Lincs_website_conversion_class.Label_brand_name,
                                                  Lincs_website_conversion_class.Label_approval_date,
                                                  Lincs_website_conversion_class.Label_human_target_proteins };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Drug_summary_class
    {
        public Drug_summary_line_class[] Drug_summaries { get; set; }

        private void Add_drugType()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            Deg_drug_legend_line_class drug_legend_line = new Deg_drug_legend_line_class();
            drug_legend.Generate_de_novo();
            drug_legend.Add_missing_cardiotoxicity_from_faers();
            drug_legend.Legend = drug_legend.Legend.OrderBy(l => l.Drug).ToArray();
            int legend_length = drug_legend.Legend.Length;
            int indexL = 0;

            this.Drug_summaries = this.Drug_summaries.OrderBy(l => l.Drug_ID).ToArray();
            int drug_summaries_length = Drug_summaries.Length;
            Drug_summary_line_class drug_summary_line;
            int stringCompare = -2;
            for (int indexD=0; indexD<drug_summaries_length;indexD++)
            {
                drug_summary_line = this.Drug_summaries[indexD];
                stringCompare = -2;
                while ((indexL < legend_length)&&(stringCompare<0))
                {
                    drug_legend_line = drug_legend.Legend[indexL];
                    stringCompare = drug_legend_line.Drug.CompareTo(drug_summary_line.Drug_ID);
                    if (stringCompare<0)
                    {
                        indexL++;
                    }
                }
                if (stringCompare!=0) { throw new Exception(); }
                drug_summary_line.Drug_type = drug_legend_line.Drug_type;
                drug_summary_line.Is_cardiotoxic = (string)drug_legend_line.Is_cardiotoxic.Clone();
                drug_summary_line.DrugName = (string)drug_legend_line.Full_name.Clone();
            }
        }

        private void Add_human_drug_target_proteins()
        {
            Drug_bank_class drug_bank = new Drug_bank_class();
            drug_bank.Generate_de_novo_by_reading_raw_data_and_write();
            int drug_bank_length = drug_bank.Drugs.Length;
            Drug_bank_line_class drug_bank_line;
            int indexDrugBank = 0;
            int stringCompare = -2;

            int drug_summaries_length = this.Drug_summaries.Length;
            Drug_summary_line_class drug_summary_line;
            List<string> current_human_target_proteins = new List<string>();

            for (int indexD=0; indexD<drug_summaries_length; indexD++)
            {
                drug_summary_line = this.Drug_summaries[indexD];
                stringCompare = -2;
                while ((indexDrugBank < drug_bank_length) && (stringCompare <= 0))
                {
                    drug_bank_line = drug_bank.Drugs[indexDrugBank];
                    if ((indexDrugBank == 0) || (!drug_bank_line.Drug_name.Equals(drug_bank.Drugs[indexDrugBank - 1].Drug_name)))
                    {
                        current_human_target_proteins.Clear();
                    }
                    if (drug_bank_line.Polypeptide_entity_organism.Equals(Organism_enum.Homo_sapiens))
                    {
                        current_human_target_proteins.Add(drug_bank_line.Entity_name);
                    }
                    stringCompare = drug_bank_line.Drug_name.CompareTo(drug_summary_line.DrugName);
                    if (stringCompare <= 0)
                    {
                        indexDrugBank++;
                    }
                    if ((indexDrugBank == drug_bank_length - 1) || (!drug_bank_line.Drug_name.Equals(drug_bank.Drugs[indexDrugBank + 1].Drug_name)))
                    {
                        if (stringCompare == 0)
                        {
                            drug_summary_line.DrugHumanTargetProteins = current_human_target_proteins.ToArray();
                        }
                    }
                }
            }
        }

        private void Add_human_drug_target_proteins_using_onto_association()
        {
            Ontology_library_class drugBank = new Ontology_library_class();
            drugBank.Generate_by_reading(Ontology_type_enum.Drugbank_drug_targets);

            int drug_summaries_length = this.Drug_summaries.Length;
            Drug_summary_line_class drug_summary_line;
            List<string> current_human_target_proteins = new List<string>();
            string drug_name_for_drugbank;

            for (int indexD = 0; indexD < drug_summaries_length; indexD++)
            {
                drug_summary_line = this.Drug_summaries[indexD];
                drug_name_for_drugbank = drug_summary_line.DrugName;
                switch (drug_name_for_drugbank)
                {
                    case "cefuroxime":
                    case "delavirdine":
                    case "endothelin-1":
                    case "entecavir":
                    case "IGF-1":
                    case "rituximab":
                    case "SB431542 (TGF-beta antagonist)":
                    case "sb431542 (tgf-beta antagonist)":
                    case "TNF-alpha":
                    case "tnf-alpha":
                    case "insulin-like growth factor 1":
                        drug_summary_line.DrugHumanTargetProteins = new string[] { "No human drug target proteins in drug bank" };
                        break;
                    case "cyclosporine":
                        drug_name_for_drugbank = "Ciclosporin";
                        break;
                    default:
                        drug_summary_line.DrugHumanTargetProteins = drugBank.Get_all_ordered_unique_gene_symbols_of_input_scps(new string[] { drug_name_for_drugbank.ToUpper() });
                        break;
                }
            }
        }



        public void Generate()
        {
            Read();
            Add_drugType();
            Add_drugApproval_dates();
            Add_human_drug_target_proteins_using_onto_association();
        }

        private void Add_drugApproval_dates()
        {
            FAERS_class faers = new FAERS_class();
            faers.Generate_by_reading();
            Dictionary<string,string> drug_drugApprovalDate_dict = faers.Get_dictionary_with_drug_drugApprovalData();
            foreach (Drug_summary_line_class summary_line in this.Drug_summaries)
            {
                if (drug_drugApprovalDate_dict.ContainsKey(summary_line.Drug_ID))
                {
                    summary_line.ApprovalDate = (string)drug_drugApprovalDate_dict[summary_line.Drug_ID].Clone();
                }
            }
        }

        public void Keep_only_indicated_drug_IDs(string[] keep_drug_IDs)
        {
            keep_drug_IDs = keep_drug_IDs.Distinct().OrderBy(l => l).ToArray();
            int keep_drug_IDs_length = keep_drug_IDs.Length;
            string keep_drug_ID;
            int indexKeep = 0;
            Drug_summary_line_class drug_summary_line;
            List<Drug_summary_line_class> keep_drug_summaries = new List<Drug_summary_line_class>();
            int drug_summaries_length = this.Drug_summaries.Length;
            int stringCompare = -2;
            for (int indexD=0; indexD<drug_summaries_length; indexD++)
            {
                drug_summary_line = this.Drug_summaries[indexD];
                stringCompare = -2;
                while ((indexKeep<keep_drug_IDs_length)&&(stringCompare<0))
                {
                    keep_drug_ID = keep_drug_IDs[indexKeep];
                    stringCompare = keep_drug_ID.CompareTo(drug_summary_line.Drug_ID);
                    if (stringCompare<0)
                    {
                        indexKeep++;
                    }
                    else if (stringCompare==0)
                    {
                        keep_drug_summaries.Add(drug_summary_line);
                    }
                }
            }
            this.Drug_summaries = keep_drug_summaries.ToArray();
        }

        public void Write(string subdirectory, string fileName)
        {
            Drug_summary_readWriteOptions_class readWriteOptions = new Drug_summary_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Drug_summaries, readWriteOptions);
        }

        public void Write_for_adverseEvent(string subdirectory, string fileName)
        {
            Drug_summary_adverseEvent_readWriteOptions_class readWriteOptions = new Drug_summary_adverseEvent_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Drug_summaries, readWriteOptions);
        }

        private void Read()
        {
            Drug_summary_inputReadOptions_class readOptions = new Drug_summary_inputReadOptions_class();
            this.Drug_summaries = ReadWriteClass.ReadRawData_and_FillArray<Drug_summary_line_class>(readOptions);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Scp_summary_line_class
    {
        public string Side_effect { get; set; }
        public Ontology_type_enum Ontology { get; set; }
        public string Scp { get; set; }
        public string Association { get; set; }
        public DE_entry_enum Entry_type { get; set; }
        public float Cutoff_rank { get; set; }
        public int Max_cutoff_rank_for_AUC { get; set; }
        public float Selection_rank { get; set; }
        public string Selected { get; set; }
        public float Precision { get; set; }
        public float Recall { get; set; }
        public string ValueForSelection { get; set; }

        public Scp_summary_line_class Deep_copy()
        {
            Scp_summary_line_class copy = (Scp_summary_line_class)this.MemberwiseClone();
            copy.Side_effect = (string)this.Side_effect.Clone();
            copy.Scp = (string)this.Scp.Clone();
            copy.Association = (string)this.Association.Clone();
            copy.Selected = (string)this.Selected.Clone();
            copy.ValueForSelection = (string)this.ValueForSelection.Clone();
            return copy;
        }
    }

    class Scp_summary_readWriteOptions_class : ReadWriteOptions_base
    {
        public Scp_summary_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Side_effect", "Ontology", "Scp",              "Association", "Entry_type", "Cutoff_rank", "Max_cutoff_rank_for_AUC", "Selection_rank", "Selected", "Precision", "Recall", "ValueForSelection" };
            this.Key_columnNames   = new string[] { "Side_effect", "Ontology", "Scp_completeName", "Association", "Entry_type", "Cutoff_rank", "Max_cutoff_rank_for_AUC", "Selection_rank", "Selected", "Precision", "Recall", "ValueForSelection" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Scp_summary_class
    {
        public Scp_summary_line_class[] Scp_summaries { get; set; }

        private void Keep_only_selected_scps()
        {
            List<Scp_summary_line_class> keep = new List<Scp_summary_line_class>();
            foreach (Scp_summary_line_class scp_summary_line in Scp_summaries)
            {
                if (scp_summary_line.Selected.Equals("TRUE"))
                {
                    keep.Add(scp_summary_line);
                }
            }
            this.Scp_summaries = keep.ToArray();
        }

        public void Generate_by_reading(string subdirectory, string fileName)
        {
            Read(subdirectory, fileName);
            Keep_only_selected_scps();
        }

        private void Read(string subdirectory, string fileName)
        {
            Scp_summary_readWriteOptions_class readWriteOptions = new Scp_summary_readWriteOptions_class(subdirectory, fileName);
            this.Scp_summaries = ReadWriteClass.ReadRawData_and_FillArray<Scp_summary_line_class>(readWriteOptions);
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Pathway_for_adverseEvent_line_class
    {
        public Ontology_type_enum Ontology { get; set; }
        public string Drug { get; set; }
        public Drug_type_enum Drug_class { get; set; }
        public string Sample_name { get; set; }
        public DE_entry_enum Sample_entryType { get; set; }
        public string Entry_type_description { get { return Lincs_website_conversion_class.Get_entryType_description(Sample_entryType); } }
        public string Cell_line { get; set; }
        public string Scp { get; set; }
        public double Pvalue { get; set; }
        public float Fractional_rank { get; set; }
        public string Scp_classification { get; set; }
        public string[] Overlap_symbols { get; set; }
        public string ReadWrite_overlap_symbols
        { 
            get { return ReadWriteClass.Get_writeLine_from_array(this.Overlap_symbols, Pathway_for_adverseEvent_input_readWriteOptions_class.Delimiter); }
            set { this.Overlap_symbols = ReadWriteClass.Get_array_from_readLine<string>(value, Pathway_for_adverseEvent_input_readWriteOptions_class.Delimiter); }
        }

        public Pathway_for_adverseEvent_line_class Deep_copy()
        {
            Pathway_for_adverseEvent_line_class copy = (Pathway_for_adverseEvent_line_class)this.MemberwiseClone();
            copy.Drug = (string)this.Drug.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            copy.Scp_classification = (string)this.Scp_classification.Clone();
            copy.Scp = (string)this.Scp.Clone();
            copy.Sample_name = (string)this.Sample_name.Clone();
            int symbols_length = this.Overlap_symbols.Length;
            copy.Overlap_symbols = new string[symbols_length];
            for (int indexNCBI=0; indexNCBI<symbols_length; indexNCBI++)
            {
                copy.Overlap_symbols[indexNCBI] = (string)this.Overlap_symbols[indexNCBI].Clone();
            }
            return copy;
        }
    }

    class Pathway_for_adverseEvent_input_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter { get { return ','; } }

        public Pathway_for_adverseEvent_input_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;

            this.Key_propertyNames = new string[] { "Ontology", "Sample_entryType", "Sample_name", "Scp", "Pvalue", "Fractional_rank","ReadWrite_overlap_symbols" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Pathway_for_adverseEvent_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter {  get { return ','; } }

        public Pathway_for_adverseEvent_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Ontology", //0
                                                    "Drug", //1
                                                    "Drug_class", //2
                                                    "Cell_line", //3
                                                    "Entry_type_description",  //4
                                                    "Scp", //5
                                                    "Scp_classification",//6
                                                    "Pvalue",  //7
                                                    "Fractional_rank", //8
                                                    "ReadWrite_overlap_symbols" };//9

            this.Key_columnNames = new string[]   { "MBCO SCP level",//0
                                                    Lincs_website_conversion_class.Label_drugName,//1
                                                    Lincs_website_conversion_class.Label_drug_class,//2
                                                    Lincs_website_conversion_class.Label_cell_line,//3
                                                    Lincs_website_conversion_class.Label_entryType,//4
                                                    "SCP",//5
                                                    "SCP classification", //6
                                                    Lincs_website_conversion_class.Label_pvalue, //7
                                                    "Fractional rank",//8
                                                    "Drug-induced SCP genes" };//9
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Pathway_for_adverseEvent_options_class
    {
        public float Max_pvalue { get; set; }
        public int TopDEGs_count { get; set; }
        public float F1score_beta { get; set; }
        public float Penalty { get; set; }

        public Pathway_for_adverseEvent_options_class()
        {
            Max_pvalue = 0.05F;
            TopDEGs_count = 600;
            F1score_beta = 0.2F;
            Penalty = 0.5F;
        }

        public string Get_scp_subdirectory()
        {
            return "Enrichment_maxP" + Max_pvalue + "_top" + TopDEGs_count + "DEGs_in_decomposed_rocForFractional_rank/";
        }
        public string Get_scp_summary_fileName()
        {
            return "SCP_summaries_betaF1_beta" + F1score_beta + "_penalty" + Penalty + ".txt";
        }

    }

    class Pathway_for_adverseEvent_class
    {
        public Pathway_for_adverseEvent_line_class[] Scps { get; set; }
        public Pathway_for_adverseEvent_options_class Options { get; set; }

        public Pathway_for_adverseEvent_class()
        {
            this.Options = new Pathway_for_adverseEvent_options_class();
        }

        private void Add_drugClasses_and_replace_drugName_by_fullDrugName()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, Drug_type_enum> drug_drugClass_dict = drug_legend.Get_drug_drugType_dictionary();
            Dictionary<string, string> drug_drugFullName_dict = drug_legend.Get_drug_drugFullName_dict();
            foreach (Pathway_for_adverseEvent_line_class scp_line in Scps)
            {
                scp_line.Drug_class = drug_drugClass_dict[scp_line.Drug];
                scp_line.Drug = (string)drug_drugFullName_dict[scp_line.Drug].Clone();
            }
        }

        public void Generate_by_reading_and_complete(string overall_input_subdirectory, string[] fileNames)
        {
            Read(overall_input_subdirectory, fileNames);
            Complete_pathway_instance(overall_input_subdirectory);
        }

        private void Add_hyphen_to_cell_type()
        {
            foreach (Pathway_for_adverseEvent_line_class pathway_line in this.Scps)
            {
                pathway_line.Cell_line = pathway_line.Cell_line.Replace("iPSC derived", "iPSC-derived");
            }
        }

        private void Add_pathway_classification_and_check_that_only_one_sideEffect(string overall_input_subdirectory)
        {
            string input_subdirectory = overall_input_subdirectory + Options.Get_scp_subdirectory();
            string scp_summary_fileName = Options.Get_scp_summary_fileName();
            Scp_summary_class scp_summary = new Scp_summary_class();
            Scp_summary_line_class scp_summary_line;
            scp_summary.Generate_by_reading(input_subdirectory, scp_summary_fileName);
            scp_summary.Scp_summaries = scp_summary.Scp_summaries.OrderBy(l=>l.Association).ThenBy(l => l.Ontology).ThenBy(l => l.Scp).ThenBy(l => l.Entry_type).ToArray();
            int summaries_length = scp_summary.Scp_summaries.Length;
            string sideEffect = (string)scp_summary.Scp_summaries[0].Side_effect.Clone();
            int stringCompare;

            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, bool> drug_isCardiotoxicTKI_dict = drug_legend.Get_drug_isCardiotoxicTKI_dict();
            Dictionary<string, bool> drug_isNonCardiotoxicTKI_dict = drug_legend.Get_drug_isNonCardiotoxicTKI_dict();

            int scps_length = this.Scps.Length;
            int indexScp = 0;
            Pathway_for_adverseEvent_line_class scp_line;
            this.Scps = this.Scps.OrderBy(l => l.Ontology).ThenBy(l => l.Scp).ThenBy(l => l.Sample_entryType).ThenBy(l => l.Drug).ToArray();
            string add_scp_classification = "";
            for (int indexSummaries=0; indexSummaries < summaries_length; indexSummaries++)
            {
                scp_summary_line = scp_summary.Scp_summaries[indexSummaries];
                if (!scp_summary_line.Side_effect.Equals(sideEffect)) { throw new Exception(); }
                if ((indexSummaries==0)||(!scp_summary_line.Association.Equals(scp_summary.Scp_summaries[indexSummaries-1].Association)))
                {
                    indexScp = 0;
                }
                if (scp_summary_line.Selected.Equals("TRUE"))
                {
                    stringCompare = -2;
                    while ((indexScp < scps_length) && (stringCompare <= 0))
                    {
                        scp_line = this.Scps[indexScp];
                        stringCompare = scp_line.Ontology.CompareTo(scp_summary_line.Ontology);
                        if (stringCompare == 0)
                        {
                            stringCompare = scp_line.Scp.CompareTo(scp_summary_line.Scp);
                        }
                        if (stringCompare == 0)
                        {
                            stringCompare = scp_line.Sample_entryType.CompareTo(scp_summary_line.Entry_type);
                        }
                        if (stringCompare < 0)
                        {
                            indexScp++;
                        }
                        else if (stringCompare == 0)
                        {
                            add_scp_classification = "";
                            if (scp_line.Fractional_rank <= scp_summary_line.Max_cutoff_rank_for_AUC)
                            {
                                switch (scp_summary_line.Association)
                                {
                                    case Lincs_toxicity_classifications_definition_class.Cardiotoxic_tkis:
                                        if (drug_isCardiotoxicTKI_dict.ContainsKey(scp_line.Drug))
                                        {
                                            add_scp_classification = Lincs_toxicity_classifications_definition_class.Get_scp_classification_for_cardiotoxic_TKIs(scp_summary_line.Entry_type);
                                        }
                                        break;
                                    case Lincs_toxicity_classifications_definition_class.Noncardiotoxic_tkis:
                                        if (drug_isNonCardiotoxicTKI_dict.ContainsKey(scp_line.Drug))
                                        {
                                            add_scp_classification = Lincs_toxicity_classifications_definition_class.Get_scp_classification_for_noncardiotoxic_TKIs(scp_summary_line.Entry_type);
                                        }
                                        break;
                                    default:
                                        throw new Exception();
                                }
                            }
                            if (String.IsNullOrEmpty(scp_line.Scp_classification))
                            {
                                scp_line.Scp_classification = (string)add_scp_classification.Clone();
                            }
                            else if (!String.IsNullOrEmpty(add_scp_classification)) { throw new Exception(); }
                            indexScp++;
                        }
                    }
                }
            }
        }

        private void Fill_drug_and_cell_line()
        {
            string[] splitStrings;
            foreach (Pathway_for_adverseEvent_line_class pathway_line in this.Scps)
            {
                splitStrings = pathway_line.Sample_name.Split('-');
                pathway_line.Cell_line = (string)splitStrings[1].Clone();
                pathway_line.Drug = (string)splitStrings[0].Clone();
            }
        }

        private void Remove_not_significant_pvalue()
        {
            List<Pathway_for_adverseEvent_line_class> keep = new List<Pathway_for_adverseEvent_line_class>();
            foreach (Pathway_for_adverseEvent_line_class pathway_line in this.Scps)
            {
                if (pathway_line.Pvalue==1) { }
                else if (pathway_line.Pvalue>Options.Max_pvalue) { throw new Exception(); }
                else
                {
                    keep.Add(pathway_line);
                }
            }
            this.Scps = keep.ToArray();
        }

        public void Complete_pathway_instance(string overall_input_subdirectory)
        {
            Remove_not_significant_pvalue();
            Fill_drug_and_cell_line();
            Add_hyphen_to_cell_type();
            Add_pathway_classification_and_check_that_only_one_sideEffect(overall_input_subdirectory);
            Add_drugClasses_and_replace_drugName_by_fullDrugName();
        }

        private void Read(string overall_input_subdirectory, string[] fileNames)
        {
            string input_subdirectory = overall_input_subdirectory + Options.Get_scp_subdirectory();
            List<Pathway_for_adverseEvent_line_class> combined_read_lines = new List<Pathway_for_adverseEvent_line_class>();
            foreach (string fileName in fileNames)
            {
                Pathway_for_adverseEvent_input_readWriteOptions_class readWriteOptions = new Pathway_for_adverseEvent_input_readWriteOptions_class(input_subdirectory, fileName);
                combined_read_lines.AddRange(ReadWriteClass.ReadRawData_and_FillArray<Pathway_for_adverseEvent_line_class>(readWriteOptions));
            }
            this.Scps = combined_read_lines.ToArray();
        }

        public void Write(string directory, string fileName)
        {
            Pathway_for_adverseEvent_readWriteOptions_class readWriteOptions = new Pathway_for_adverseEvent_readWriteOptions_class(directory, fileName);
            ReadWriteClass.WriteData(this.Scps, readWriteOptions);
        }

        public void Write_after_adding_parameter_adjusted_subdirectory_specified_in_options(string subdirectory, string fileName)
        {
            string final_subdirectory = subdirectory + Options.Get_scp_subdirectory();
            Pathway_for_adverseEvent_readWriteOptions_class readWriteOptions = new Pathway_for_adverseEvent_readWriteOptions_class(final_subdirectory, fileName);
            ReadWriteClass.WriteData(this.Scps, readWriteOptions);
        }


    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    class Pathway_for_adverseEvent_singleCell_expression_line_class
    {
        public string Scp { get; set; }
        public string Scp_classification { get; set; }
        public float Average_expression_atrial_cardiomyocyte { get; set; }
        public float Average_expression_ventriculuar_cardiomyocyte { get; set; }
        public float Average_expression_cardiac_fibroblast { get; set; }
        public float Average_expression_endothelial_cell { get; set; }

        public Pathway_for_adverseEvent_singleCell_expression_line_class()
        {
        }

        public Pathway_for_adverseEvent_singleCell_expression_line_class Deep_copy()
        {
            Pathway_for_adverseEvent_singleCell_expression_line_class copy = (Pathway_for_adverseEvent_singleCell_expression_line_class)this.MemberwiseClone();
            copy.Scp = (string)this.Scp.Clone();
            copy.Scp_classification = (string)this.Scp_classification.Clone();
            return copy;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Drug_cards_line_class
    {
        public string Drug { get; set; }
        public string Drug_fullName { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public Drug_type_enum Drug_type { get; set; }
        public int Order { get; set; }
        public int Column_no { get; set; }

        public string Drug_class_for_adverse_events
        {
            get { return Lincs_website_conversion_class.Get_drugClass_for_website(Drug_type); }
        }

        public string[] Entities { get; set; }

        public string ReadWrite_entities
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Entities, Drug_cards_readWriteOptions_class.Delimiter); }
            set { this.Entities = ReadWriteClass.Get_array_from_readLine<string>(value, Drug_cards_readWriteOptions_class.Delimiter); }
        }

        public Drug_cards_line_class Deep_copy()
        {
            Drug_cards_line_class copy = (Drug_cards_line_class)this.MemberwiseClone();
            copy.Entities = Array_class.Deep_copy_string_array(this.Entities);
            copy.Drug = (string)this.Drug.Clone();
            copy.Title = (string)this.Title.Clone();
            copy.Subtitle = (string)this.Subtitle.Clone();
            return copy;
        }

    }

    class Drug_cards_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter { get { return ';'; } }
        public Drug_cards_readWriteOptions_class(string subdirectory, string fileName)
        {
            if (subdirectory.IndexOf(":")!=-1)
            {
                this.File = subdirectory + fileName;
            }
            else
            {
                this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            }
            this.Key_propertyNames = new string[] { "Order","Drug_type", "Drug", "Drug_fullName", "Title","Subtitle",  "ReadWrite_entities" };
            this.Key_columnNames = this.Key_propertyNames;
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Drug_cards_for_website_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter { get { return ';'; } }
        public Drug_cards_for_website_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Drug_fullName", "Order", "Title", "Subtitle",  "ReadWrite_entities" };
            this.Key_columnNames = new string[] { "Drug name", "Order", "Title","Subtitle", "Values" };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Drug_cards_for_FDA_singleDrugFile_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter { get { return ';'; } }
        public Drug_cards_for_FDA_singleDrugFile_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Order", "Title", "Subtitle", "ReadWrite_entities" };
            this.Key_columnNames = new string[] { "Order", "Title", "Subtitle", "Values" };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Drug_cards_options_class
    {
        public int Top_ranked_degs { get; set; }

        public int Top_ranked_up_or_down_scps { get; set; }
        public int Top_considered_cardiotoxic_scps { get; set; }
        public int Max_enrichmentRank_cardiotoxic_mbco_level3_scps { get; set; }
        public int Max_shown_variants_within_each_category_for_each_drug { get; set; }

        public Drug_cards_options_class()
        {
            Top_ranked_degs = 10;
            Top_ranked_up_or_down_scps = 5;
            Top_considered_cardiotoxic_scps = 5;
            Max_enrichmentRank_cardiotoxic_mbco_level3_scps = 30;
            Max_shown_variants_within_each_category_for_each_drug = 100;
        }
    }

    class Drug_cards_class
    {
        const int OrderNo_drug_name = 0;
        const int OrderNo_drug_class = 1;
        const int OrderNo_isCardiotoxic = 2;
        const int OrderNo_cardiotoxicFrequency = 3;
        const int OrderNo_cardiotoxicFaers = 4;
        const int OrderNo_treatedCellLines = 5;
        const int OrderNo_upRegulatedGenes = 6;
        const int OrderNo_downRegulatedGenes = 7;
        const int OrderNo_upRegulatedPathways = 8;
        const int OrderNo_downRegulatedPathways = 9;
        const int OrderNo_upDownCardiotoxicPathwaysCount = 10;
        const int OrderNo_upCardiotoxicPathways = 11;
        const int OrderNo_downCardiotoxicPathways = 12;
        const int OrderNo_genomicVariantsCount_drugTarget = 13;
        const int OrderNo_genomicVariantsCount_regulatorsDrugTarget = 14;
        const int OrderNo_genomicVariantsCount_cardiotoxic_SCPs = 15;
        const int OrderNo_genomicVariants_drugTarget = 16;
        const int OrderNo_genomicVariants_regulatorsDrugTarget = 17;
        const int OrderNo_genomicVariants_cardiotoxicSCPs = 18;

        public Drug_cards_line_class[] Drug_cards { get; set; }
        public Drug_cards_options_class Options { get; set; }
        public Dictionary<string,Dictionary<string, bool>> EntityClass_already_considered_DEGs_dict { get; set; }
        public int Top_degs_for_calcualtions { get; set; }
        public Drug_cards_class()
        {
            this.Drug_cards = new Drug_cards_line_class[0];
            this.Options = new Drug_cards_options_class();
            EntityClass_already_considered_DEGs_dict = new Dictionary<string, Dictionary<string, bool>>();
            Top_degs_for_calcualtions = -1;
        }

        private void Set_top_degs_for_calculation_or_check_if_the_same(int top_degs_for_calculations)
        {
            if (Top_degs_for_calcualtions==-1) { Top_degs_for_calcualtions = top_degs_for_calculations; }
            else if (Top_degs_for_calcualtions!=top_degs_for_calculations) { throw new Exception(); }
        }

        private void Add_to_array(Drug_cards_line_class[] add_drug_cards)
        {
            int this_length = this.Drug_cards.Length;
            int add_length = add_drug_cards.Length;
            int new_length = this_length + add_length;
            int indexNew = -1;
            Drug_cards_line_class[] new_drug_cards = new Drug_cards_line_class[new_length];
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_drug_cards[indexNew] = this.Drug_cards[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_drug_cards[indexNew] = add_drug_cards[indexAdd];
            }
            this.Drug_cards = new_drug_cards;
        }

        public void Generate_from_genomic_variants_mapping_to_pharmakodynamic_mechnisms(Lincs_vcf_genomic_data_class genomic_data)
        {
            genomic_data.Genomic_data = genomic_data.Genomic_data.OrderBy(l => l.Cell_line_drug_with_outlier_response).ThenBy(l => l.Rs_identifier).ThenBy(l=>l.Gene_symbol).ThenBy(l=>l.Drug_target_symbol).ThenBy(l=>l.Relation_of_gene_symbol_to_drug).ToArray();
            int genomic_data_length = genomic_data.Genomic_data.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            int rs_count_at_drug_target = 0;
            int rs_count_regulating_drug_target = 0;
            Drug_cards_line_class new_drug_cards_line;
            List<Drug_cards_line_class> new_drug_cards = new List<Drug_cards_line_class>();
            List<string> rsIdentifiers_mapping_to_drugTargets = new List<string>();
            List<string> rsIdentifiers_mapping_to_regulatorsOfDrugTargets = new List<string>();
            for (int indexG=0; indexG<genomic_data_length; indexG++)
            {
                genomic_data_line = genomic_data.Genomic_data[indexG];
                if ((indexG == 0)
                    || (!genomic_data_line.Cell_line_drug_with_outlier_response.Equals(genomic_data.Genomic_data[indexG - 1].Cell_line_drug_with_outlier_response)))
                {
                    rs_count_at_drug_target = 0;
                    rs_count_regulating_drug_target = 0;
                    rsIdentifiers_mapping_to_drugTargets.Clear();
                    rsIdentifiers_mapping_to_regulatorsOfDrugTargets.Clear();
                }
                if ((indexG != 0)
                    && (genomic_data_line.Cell_line_drug_with_outlier_response.Equals(genomic_data.Genomic_data[indexG - 1].Cell_line_drug_with_outlier_response))
                    && (genomic_data_line.Gene_symbol.Equals(genomic_data.Genomic_data[indexG - 1].Gene_symbol))
                    && (genomic_data_line.Drug_target_symbol.Equals(genomic_data.Genomic_data[indexG - 1].Drug_target_symbol))
                    && (genomic_data_line.Relation_of_gene_symbol_to_drug.Equals(genomic_data.Genomic_data[indexG - 1].Relation_of_gene_symbol_to_drug))
                    && (genomic_data_line.Rs_identifier.Equals(genomic_data.Genomic_data[indexG - 1].Rs_identifier)))
                {
                    throw new Exception();
                }
                if ((indexG == 0)
                    || (!genomic_data_line.Cell_line_drug_with_outlier_response.Equals(genomic_data.Genomic_data[indexG - 1].Cell_line_drug_with_outlier_response))
                    || (!genomic_data_line.Rs_identifier.Equals(genomic_data.Genomic_data[indexG - 1].Rs_identifier)))
                {
                    switch (genomic_data_line.Relation_of_gene_symbol_to_drug)
                    {
                        case Relation_of_gene_symbol_to_drug_enum.Drug_target_protein:
                            rs_count_at_drug_target++;
                            rsIdentifiers_mapping_to_drugTargets.Add(genomic_data_line.Rs_identifier);
                            break;
                        case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein:
                        case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein:
                            rs_count_regulating_drug_target++;
                            rsIdentifiers_mapping_to_regulatorsOfDrugTargets.Add(genomic_data_line.Rs_identifier);
                            break;
                        case Relation_of_gene_symbol_to_drug_enum.Enzyme:
                        case Relation_of_gene_symbol_to_drug_enum.Transporter:
                        case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme:
                        case Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter:
                        case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme:
                        case Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter:
                            break;
                        default:
                            throw new Exception();
                    }
                }
                if ((indexG == genomic_data_length - 1)
                    || (!genomic_data_line.Cell_line_drug_with_outlier_response.Equals(genomic_data.Genomic_data[indexG + 1].Cell_line_drug_with_outlier_response)))
                {
                    //if (rsIdentifiers_mapping_to_drugTargets.Count > 0)
                    {
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Predicted genomic variants";
                        new_drug_cards_line.Subtitle = "# predicted genomic variants mapping to drug targets:";
                        new_drug_cards_line.Entities = new string[] { rs_count_at_drug_target.ToString() };
                        new_drug_cards_line.Drug_fullName = (string)genomic_data_line.Cell_line_drug_with_outlier_response.Clone();
                        new_drug_cards_line.Order = OrderNo_genomicVariantsCount_drugTarget;
                        new_drug_cards.Add(new_drug_cards_line);
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Predicted genomic variants";
                        new_drug_cards_line.Subtitle = "List of predicted genomic variants mapping to drug targets:";
                        if (rsIdentifiers_mapping_to_drugTargets.Distinct().ToArray().Length != rsIdentifiers_mapping_to_drugTargets.Count) { throw new Exception(); }
                        if (rsIdentifiers_mapping_to_drugTargets.Count <= Options.Max_shown_variants_within_each_category_for_each_drug)
                        {
                            new_drug_cards_line.Entities = rsIdentifiers_mapping_to_drugTargets.Distinct().OrderBy(l => l).ToArray();
                        }
                        else
                        {
                            new_drug_cards_line.Entities = new string[] { "See 'Datasets'" };
                        }
                        new_drug_cards_line.Drug_fullName = (string)genomic_data_line.Cell_line_drug_with_outlier_response.Clone();
                        new_drug_cards_line.Order = OrderNo_genomicVariants_drugTarget;
                        new_drug_cards.Add(new_drug_cards_line);
                    }
                    //if (rs_count_regulating_drug_target > 0)
                    {
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Predicted genomic variants";
                        new_drug_cards_line.Subtitle = "# predicted genomic variants mapping to drug action pathways:";
                        new_drug_cards_line.Entities = new string[] { rs_count_regulating_drug_target.ToString() };
                        new_drug_cards_line.Drug_fullName = (string)genomic_data_line.Cell_line_drug_with_outlier_response.Clone();
                        new_drug_cards_line.Order = OrderNo_genomicVariantsCount_regulatorsDrugTarget;
                        new_drug_cards.Add(new_drug_cards_line);
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Predicted genomic variants";
                        new_drug_cards_line.Subtitle = "List of predicted genomic variants mapping to drug action pathways:";
                        if (rsIdentifiers_mapping_to_regulatorsOfDrugTargets.Distinct().ToArray().Length != rsIdentifiers_mapping_to_regulatorsOfDrugTargets.Count) { throw new Exception(); }
                        if (rsIdentifiers_mapping_to_regulatorsOfDrugTargets.Count <= Options.Max_shown_variants_within_each_category_for_each_drug)
                        {
                            new_drug_cards_line.Entities = rsIdentifiers_mapping_to_regulatorsOfDrugTargets.Distinct().OrderBy(l => l).ToArray();
                        }
                        else
                        {
                            new_drug_cards_line.Entities = new string[] { "See 'Datasets'" };
                        }
                        new_drug_cards_line.Drug_fullName = (string)genomic_data_line.Cell_line_drug_with_outlier_response.Clone();
                        new_drug_cards_line.Order = OrderNo_genomicVariants_regulatorsDrugTarget;
                        new_drug_cards.Add(new_drug_cards_line);
                    }
                }
            }
            Add_to_array(new_drug_cards.ToArray());
        }

        public void Generate_from_genomic_variants_mapping_to_cardiotoxic_pathways_if_cardiotoxic_drug(Lincs_genomic_variant_counts_foreach_drug_class genomic_data)
        {
            genomic_data.Variants_count = genomic_data.Variants_count.OrderBy(l => l.Drug).ThenBy(l=>l.Ontology).ToArray();
            int genomic_data_variants_count_length = genomic_data.Variants_count.Length;
            Lincs_genomic_variant_count_foreach_drug_line_class genomic_data_line;
            Drug_cards_line_class new_drug_cards_line;
            List<Drug_cards_line_class> new_drug_cards = new List<Drug_cards_line_class>();
            List<string> currentGenomicVariants = new List<string>();
            for (int indexGDV = 0; indexGDV < genomic_data_variants_count_length; indexGDV++)
            {
                genomic_data_line = genomic_data.Variants_count[indexGDV];
                if (  (indexGDV != 0)
                    && (genomic_data_line.Ontology.Equals(genomic_data.Variants_count[indexGDV - 1].Ontology))
                    && (genomic_data_line.Drug.Equals(genomic_data.Variants_count[indexGDV - 1].Drug)))
                {
                    throw new Exception();
                }
                if (  (genomic_data_line.Drug_is_cardiotoxic.Equals("Yes"))
                    &&(genomic_data_line.Ontology.Equals(Ontology_type_enum.Mbco_level3)))
                {
                    new_drug_cards_line = new Drug_cards_line_class();
                    new_drug_cards_line.Title = "Predicted genomic variants";
                    new_drug_cards_line.Subtitle = "# counted predicted variants mapping to cardiotoxic pathways affected by cardiotoxic drug:";
                    new_drug_cards_line.Entities = new string[] { genomic_data_line.Count_of_all_variants_mapping_to_sameLevel_scps.ToString() };
                    new_drug_cards_line.Drug = "";
                    new_drug_cards_line.Drug_fullName = (string)genomic_data_line.Drug.Clone();
                    new_drug_cards_line.Order = OrderNo_genomicVariantsCount_cardiotoxic_SCPs;
                    new_drug_cards.Add(new_drug_cards_line);
                }
            }
            Add_to_array(new_drug_cards.ToArray());
        }

        public void Generate_from_genomic_variants_mapping_to_cardiotoxic_pathways_if_cardiotoxic_drug_including_variants(Lincs_genomic_variant_counts_foreach_drug_class genomic_data)
        {
            genomic_data.Selected_variants = genomic_data.Selected_variants.OrderBy(l => l.Drug_regulating_scp).ThenBy(l => l.Rs_identifier).ThenBy(l => l.Scp_gene).ThenBy(l => l.Scp).ToArray();
            int genomic_data_length = genomic_data.Selected_variants.Length;
            Lincs_vcf_genomic_data_line_class genomic_data_line;
            Drug_cards_line_class new_drug_cards_line;
            List<Drug_cards_line_class> new_drug_cards = new List<Drug_cards_line_class>();
            List<string> rsIdentifiers_mapping_to_drugAffectedSCPs = new List<string>();
            for (int indexG = 0; indexG < genomic_data_length; indexG++)
            {
                genomic_data_line = genomic_data.Selected_variants[indexG];
                if ((indexG == 0)
                    || (!genomic_data_line.Drug_regulating_scp.Equals(genomic_data.Selected_variants[indexG - 1].Drug_regulating_scp)))
                {
                    rsIdentifiers_mapping_to_drugAffectedSCPs.Clear();
                }
                if ((indexG != 0)
                    && (genomic_data_line.Drug_regulating_scp.Equals(genomic_data.Selected_variants[indexG - 1].Drug_regulating_scp))
                    && (genomic_data_line.Scp_gene.Equals(genomic_data.Selected_variants[indexG - 1].Scp_gene))
                    && (genomic_data_line.Scp.Equals(genomic_data.Selected_variants[indexG - 1].Scp))
                    && (genomic_data_line.Rs_identifier.Equals(genomic_data.Selected_variants[indexG - 1].Rs_identifier)))
                {
                    throw new Exception();
                }
                if ((indexG == 0)
                    || (!genomic_data_line.Drug_regulating_scp.Equals(genomic_data.Selected_variants[indexG - 1].Drug_regulating_scp))
                    || (!genomic_data_line.Rs_identifier.Equals(genomic_data.Selected_variants[indexG - 1].Rs_identifier)))
                {
                    rsIdentifiers_mapping_to_drugAffectedSCPs.Add(genomic_data_line.Rs_identifier);
                }
                if ((indexG == genomic_data_length - 1)
                    || (!genomic_data_line.Drug_regulating_scp.Equals(genomic_data.Selected_variants[indexG + 1].Drug_regulating_scp)))
                {
                    //if (rsIdentifiers_mapping_to_drugTargets.Count > 0)
                    {
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Predicted genomic variants";
                        new_drug_cards_line.Subtitle = "# predicted variants mapping to drug-affected cardiotoxic pathways:";
                        if (genomic_data_line.Drug_is_cardiotoxic.Equals("Yes"))
                        {
                            new_drug_cards_line.Entities = new string[] { rsIdentifiers_mapping_to_drugAffectedSCPs.Distinct().OrderBy(l => l).ToArray().Length.ToString() };
                        }
                        else
                        {
                            new_drug_cards_line.Entities = new string[] { Conversion_class.UpDownStatus_columnName };
                        }
                        new_drug_cards_line.Drug_fullName = (string)genomic_data_line.Cell_line_drug_with_outlier_response.Clone();
                        new_drug_cards_line.Order = OrderNo_genomicVariantsCount_cardiotoxic_SCPs;
                        new_drug_cards.Add(new_drug_cards_line);
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Predicted genomic variants";
                        new_drug_cards_line.Subtitle = "List of predicted variants mapping to drug-affected cardiotoxic pathways:";
                        if (rsIdentifiers_mapping_to_drugAffectedSCPs.Distinct().ToArray().Length!=rsIdentifiers_mapping_to_drugAffectedSCPs.Count) { throw new Exception(); }
                        if (genomic_data_line.Drug_is_cardiotoxic.Equals("Yes"))
                        {
                            if (rsIdentifiers_mapping_to_drugAffectedSCPs.Count<=Options.Max_shown_variants_within_each_category_for_each_drug)
                            {
                                new_drug_cards_line.Entities = rsIdentifiers_mapping_to_drugAffectedSCPs.Distinct().OrderBy(l => l).ToArray();
                            }
                            else
                            {
                                new_drug_cards_line.Entities = new string[] { "See 'Datasets'" };
                            }
                        }
                        else
                        {
                            new_drug_cards_line.Entities = new string[] { (string)Conversion_class.No_value_string.Clone() };
                        }
                        new_drug_cards_line.Drug_fullName = (string)genomic_data_line.Cell_line_drug_with_outlier_response.Clone();
                        new_drug_cards_line.Order = OrderNo_genomicVariants_cardiotoxicSCPs;
                        new_drug_cards.Add(new_drug_cards_line);
                    }
                }
            }
            Add_to_array(new_drug_cards.ToArray());
        }

        public void Generate_from_average_degs_and_add_to_array(Deg_average_class average_degs, int top_degs_for_calculations)
        {
            Set_top_degs_for_calculation_or_check_if_the_same(top_degs_for_calculations);
            average_degs.Degs_average = Deg_average_line_class.Order_average_lines_by_group(average_degs.Degs_average);
            Deg_average_line_class average_deg_line;
            int degs_complete_length = average_degs.Degs_average.Length;
            List<string> up_genes = new List<string>();
            List<string> down_genes = new List<string>();
            Drug_cards_line_class new_drug_cards_line;
            List<Drug_cards_line_class> new_drug_cards = new List<Drug_cards_line_class>();
            string entityClass;
            string[] patients;
            string patient;
            int patients_length;
            for (int indexDeg = 0; indexDeg < degs_complete_length; indexDeg++)
            {
                average_deg_line = average_degs.Degs_average[indexDeg];
                if (  (indexDeg!=0)
                    && (average_deg_line.Equal_group(average_degs.Degs_average[indexDeg - 1]))
                    && (average_deg_line.Patient.Equals(average_degs.Degs_average[indexDeg-1]))) {  throw new Exception(); }
                if ((indexDeg == 0)
                    || (!average_deg_line.Equal_group(average_degs.Degs_average[indexDeg - 1])))
                {
                    up_genes.Clear();
                    down_genes.Clear();
                }
                if ((average_deg_line.Up_down_status_signedMinusLog10.Equals("Up")
                    && (up_genes.Count <= Options.Top_ranked_degs)))
                {
                    up_genes.Add((string)average_deg_line.Symbol.Clone());
                }
                if ((average_deg_line.Up_down_status_signedMinusLog10.Equals("Down")
                    && (down_genes.Count <= Options.Top_ranked_degs)))
                {
                    down_genes.Add((string)average_deg_line.Symbol.Clone());
                }
                if ((indexDeg == degs_complete_length - 1)
                    || (!average_deg_line.Equal_group(average_degs.Degs_average[indexDeg + 1])))
                {
                    if (up_genes.Count > 0)
                    {
                        entityClass = "Top upregulated genes:";
                        if (!EntityClass_already_considered_DEGs_dict.ContainsKey(entityClass)) { EntityClass_already_considered_DEGs_dict.Add(entityClass, new Dictionary<string, bool>()); }
                        EntityClass_already_considered_DEGs_dict[entityClass].Add(average_deg_line.Treatment, true);
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Differentially expressed genes (in human iPSC cardiomyoctyes)";
                        new_drug_cards_line.Subtitle = entityClass;
                        new_drug_cards_line.Entities = up_genes.ToArray();
                        new_drug_cards_line.Drug = (string)average_deg_line.Treatment.Clone();
                        new_drug_cards_line.Order = OrderNo_upRegulatedGenes;
                        new_drug_cards.Add(new_drug_cards_line);
                    }
                    if (down_genes.Count > 0)
                    {
                        entityClass = "Top downregulated genes:";
                        if (!EntityClass_already_considered_DEGs_dict.ContainsKey(entityClass)) { EntityClass_already_considered_DEGs_dict.Add(entityClass, new Dictionary<string, bool>()); }
                        EntityClass_already_considered_DEGs_dict[entityClass].Add(average_deg_line.Treatment, true);
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Differentially expressed genes (in human iPSC cardiomyoctyes)";
                        new_drug_cards_line.Subtitle = entityClass;
                        new_drug_cards_line.Entities = down_genes.ToArray();
                        new_drug_cards_line.Drug = (string)average_deg_line.Treatment.Clone();
                        new_drug_cards_line.Order = OrderNo_downRegulatedGenes;
                        new_drug_cards.Add(new_drug_cards_line);
                    }
                    entityClass = "Treated cell lines";
                    if (!EntityClass_already_considered_DEGs_dict.ContainsKey(entityClass)) { EntityClass_already_considered_DEGs_dict.Add(entityClass, new Dictionary<string, bool>()); }
                    new_drug_cards_line = new Drug_cards_line_class();
                    new_drug_cards_line.Title = "Treated human iPSC cardiomyocyte lines";
                    new_drug_cards_line.Subtitle = "";
                    patients = average_deg_line.Patient.Split(';');
                    patients = patients.OrderBy(l => l).ToArray();
                    patients_length = patients.Length;
                    for (int indexP =0; indexP<patients_length; indexP++)
                    {
                        patient = patients[indexP];
                        patient = patient.Replace("Cell_line.", "");
                        patient = patient.Split('_')[0];
                        patients[indexP] = patient;
                    }
                    new_drug_cards_line.Entities = patients;
                    new_drug_cards_line.Drug = (string)average_deg_line.Treatment.Clone();
                    new_drug_cards_line.Order = OrderNo_treatedCellLines;
                    new_drug_cards.Add(new_drug_cards_line);

                    entityClass = "Drug class";
                    if (!EntityClass_already_considered_DEGs_dict.ContainsKey(entityClass)) { EntityClass_already_considered_DEGs_dict.Add(entityClass, new Dictionary<string, bool>()); }
                    new_drug_cards_line = new Drug_cards_line_class();
                    new_drug_cards_line.Title = (string)entityClass.Clone();
                    new_drug_cards_line.Subtitle = "";
                    new_drug_cards_line.Entities = new string[] { average_deg_line.Drug_type.ToString() };
                    new_drug_cards_line.Drug = (string)average_deg_line.Treatment.Clone();
                    new_drug_cards_line.Order = OrderNo_drug_class;
                    new_drug_cards.Add(new_drug_cards_line);
                }
            }
            this.Add_to_array(new_drug_cards.ToArray());
        }

        public void Generate_from_average_scps_and_add_to_array(Enrichment2018_results_class enrichment, int top_degs_for_calculations)
        {
            Set_top_degs_for_calculation_or_check_if_the_same(top_degs_for_calculations);
            enrichment.Order_by_ontology_sampleName_timepoint_entryType_pvalue();
            Enrichment2018_results_line_class enrichment_results_line;
            int enrich_length = enrichment.Enrichment_results.Length;
            List<string> up_scps = new List<string>();
            List<string> down_scps = new List<string>();
            Drug_cards_line_class new_drug_cards_line;
            List<Drug_cards_line_class> new_drug_cards = new List<Drug_cards_line_class>();
            string entityClass;
            for (int indexE = 0; indexE < enrich_length; indexE++)
            {
                enrichment_results_line = enrichment.Enrichment_results[indexE];
                if ((indexE == 0)
                    || (!enrichment_results_line.Sample_name.Equals(enrichment.Enrichment_results[indexE - 1].Sample_name))
                    || (!enrichment_results_line.Sample_timepoint.Equals(enrichment.Enrichment_results[indexE - 1].Sample_timepoint)))
                {
                    up_scps.Clear();
                    down_scps.Clear();
                }
                if (enrichment_results_line.Fractional_rank <= Options.Top_ranked_up_or_down_scps)
                {
                    switch (enrichment_results_line.Sample_entryType)
                    {
                        case DE_entry_enum.Diffrna_up:
                            up_scps.Add((string)enrichment_results_line.Scp.Clone());
                            break;
                        case DE_entry_enum.Diffrna_down:
                            down_scps.Add((string)enrichment_results_line.Scp.Clone());
                            break;
                        default:
                            throw new Exception();
                    }
                }
                if ((indexE == enrich_length - 1)
                    || (!enrichment_results_line.Sample_name.Equals(enrichment.Enrichment_results[indexE + 1].Sample_name))
                    || (!enrichment_results_line.Sample_timepoint.Equals(enrichment.Enrichment_results[indexE + 1].Sample_timepoint)))
                {
                    //if (up_scps.Count > 0)
                    {
                        entityClass = "Top upregulated pathways:$";// + enrichment_results_line.Ontology.ToString().Replace("_","") + " SCPs";
                        if (!EntityClass_already_considered_DEGs_dict.ContainsKey(entityClass)) { EntityClass_already_considered_DEGs_dict.Add(entityClass, new Dictionary<string, bool>()); }
                        EntityClass_already_considered_DEGs_dict[entityClass].Add(enrichment_results_line.Lincs_drug, true);
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Differentially expressed pathways (in human iPSC cardiomyoctyes)";
                        new_drug_cards_line.Subtitle = (string)entityClass.Clone();
                        new_drug_cards_line.Entities = up_scps.ToArray();
                        new_drug_cards_line.Drug = (string)enrichment_results_line.Lincs_drug.Clone();
                        new_drug_cards_line.Order = OrderNo_upRegulatedPathways;
                        new_drug_cards.Add(new_drug_cards_line);
                    }
                    //if (down_scps.Count > 0)
                    {
                        entityClass = "Top downregulated pathways:$";// + enrichment_results_line.Ontology.ToString().Replace("_", "") + " SCPs";
                        if (!EntityClass_already_considered_DEGs_dict.ContainsKey(entityClass)) { EntityClass_already_considered_DEGs_dict.Add(entityClass, new Dictionary<string, bool>()); }
                        EntityClass_already_considered_DEGs_dict[entityClass].Add(enrichment_results_line.Lincs_drug, true);
                        new_drug_cards_line = new Drug_cards_line_class();
                        new_drug_cards_line.Title = "Differentially expressed pathways (in human iPSC cardiomyoctyes)";
                        new_drug_cards_line.Subtitle = (string)entityClass.Clone();
                        new_drug_cards_line.Entities = down_scps.ToArray();
                        new_drug_cards_line.Drug = (string)enrichment_results_line.Lincs_drug.Clone();
                        new_drug_cards_line.Order = OrderNo_downRegulatedPathways;
                        new_drug_cards.Add(new_drug_cards_line);
                    }
                }
            }
            Add_to_array(new_drug_cards.ToArray());
        }

        public void Generate_from_published_risk_profiles_and_add_to_array(Deg_drug_legend_class drug_legend)
        {
            int drug_legend_length = drug_legend.Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            Drug_cards_line_class new_drug_card_line;
            List<Drug_cards_line_class> new_drug_lines = new List<Drug_cards_line_class>();
            string entityClass;
            for (int indexDC=0; indexDC<drug_legend_length;indexDC++)
            {
                drug_legend_line = drug_legend.Legend[indexDC];
                {
                    entityClass = "Clinical frequency (PMID:32231332):";
                    if (!EntityClass_already_considered_DEGs_dict.ContainsKey(entityClass)) { EntityClass_already_considered_DEGs_dict.Add(entityClass, new Dictionary<string, bool>()); }
                    EntityClass_already_considered_DEGs_dict[entityClass].Add(drug_legend_line.Drug, true);

                    new_drug_card_line = new Drug_cards_line_class();
                    new_drug_card_line.Drug = (string)drug_legend_line.Drug.Clone();
                    new_drug_card_line.Title = "Cardiotoxicity";
                    new_drug_card_line.Subtitle = (string)entityClass.Clone();
                    new_drug_card_line.Order = OrderNo_cardiotoxicFrequency;
                    if (drug_legend_line.ReadWrite_cardiotoxicity_references.IndexOf("32231332") != -1)
                    {
                        switch (drug_legend_line.Cardiotoxicity_frequencyGroup)
                        {
                            case 0:
                                new_drug_card_line.Entities = new string[] { "not reported" };
                                break;
                            case 1:
                                new_drug_card_line.Entities = new string[] { "<1%" };
                                break;
                            case 2:
                                new_drug_card_line.Entities = new string[] { "1-10%" };
                                break;
                            case 3:
                                new_drug_card_line.Entities = new string[] { ">10%" };
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else
                    {
                        new_drug_card_line.Entities = new string[] { };
                    }
                    new_drug_lines.Add(new_drug_card_line);
                }
                {
                    entityClass = "Cardiotoxic:";
                    if (!EntityClass_already_considered_DEGs_dict.ContainsKey(entityClass)) { EntityClass_already_considered_DEGs_dict.Add(entityClass, new Dictionary<string, bool>()); }
                    new_drug_card_line = new Drug_cards_line_class();
                    new_drug_card_line.Drug = (string)drug_legend_line.Drug.Clone();
                    new_drug_card_line.Title = "Cardiotoxicity";
                    new_drug_card_line.Subtitle = (string)entityClass.Clone();
                    if (!String.IsNullOrEmpty(drug_legend_line.Is_cardiotoxic))
                    {
                        new_drug_card_line.Entities = new string[] { (string)drug_legend_line.Is_cardiotoxic.Clone() };
                    }
                    else
                    {
                        new_drug_card_line.Entities = new string[0];
                    }
                    new_drug_card_line.Order = OrderNo_isCardiotoxic;
                    new_drug_lines.Add(new_drug_card_line);
                }
            }
            Add_to_array(new_drug_lines.ToArray());
        }

        public void Generate_from_faers_risk_profiles_and_add_to_array(FAERS_class faers)
        {
            int faers_length = faers.FAERS.Length;
            faers.FAERS = faers.FAERS.OrderBy(l => l.AdverseEvent).ToArray();
            FAERS_line_class faers_line;
            Drug_cards_line_class new_drug_card_line;
            List<Drug_cards_line_class> new_drug_lines = new List<Drug_cards_line_class>();
            for (int indexFaers = 0; indexFaers < faers_length; indexFaers++)
            {
                faers_line = faers.FAERS[indexFaers];
                {
                    new_drug_card_line = new Drug_cards_line_class();
                    new_drug_card_line.Drug_fullName = faers_line.Full_drug_name.ToLower();
                    new_drug_card_line.Drug = (string)faers_line.Drug.Clone();
                    new_drug_card_line.Title = "Cardiotoxicity";
                    new_drug_card_line.Subtitle = "FAERS rank:";
                    if (faers_line.Odds_ratio_rank != -1)
                    {
                        new_drug_card_line.Entities = new string[] { faers_line.Odds_ratio_rank.ToString() };
                    }
                    else
                    {
                        new_drug_card_line.Entities = new string[] { };
                    }
                    new_drug_card_line.Order = OrderNo_cardiotoxicFaers;
                    new_drug_lines.Add(new_drug_card_line);
                }
            }
            Add_to_array(new_drug_lines.ToArray());
        }

        public void Add_drug_types_and_fullNames(Deg_drug_legend_class drug_legend)
        {
            Dictionary<string, Drug_type_enum> drug_drugType_dict = drug_legend.Get_drug_drugType_dictionary();
            Dictionary<string, string> drug_drugFullName_dict = drug_legend.Get_drug_drugFullName_dict();
            foreach (Drug_cards_line_class drug_line in this.Drug_cards)
            {
                drug_line.Drug_type = drug_drugType_dict[drug_line.Drug];
                drug_line.Drug_fullName = (string)drug_drugFullName_dict[drug_line.Drug].Clone();
            }
        }

        public void Add_drug_name()
        {
            int drug_cards_length = this.Drug_cards.Length;
            this.Drug_cards = this.Drug_cards.OrderBy(l => l.Drug_fullName).ToArray();
            Drug_cards_line_class drug_cards_line;
            Drug_cards_line_class new_drug_cards_line;
            List<Drug_cards_line_class> new_drug_cards = new List<Drug_cards_line_class>();
            for (int indexDC=0; indexDC<drug_cards_length; indexDC++)
            {
                drug_cards_line = this.Drug_cards[indexDC];
                if (  (indexDC==0)
                    || (!drug_cards_line.Drug_fullName.Equals(this.Drug_cards[indexDC-1].Drug_fullName)))
                {
                    new_drug_cards_line = new Drug_cards_line_class();
                    new_drug_cards_line.Order = OrderNo_drug_name;
                    new_drug_cards_line.Drug_fullName = (string)drug_cards_line.Drug_fullName.Clone();
                    new_drug_cards_line.Subtitle = "";
                    new_drug_cards_line.Title = "Drug name";
                    new_drug_cards_line.Drug_type = drug_cards_line.Drug_type;
                    new_drug_cards_line.Entities = new string[] { (string)new_drug_cards_line.Drug_fullName.Clone() };
                    new_drug_cards.Add(new_drug_cards_line);
                }
            }
            Add_to_array(new_drug_cards.ToArray());
        }

        public void Add_top_cardiotoxic_SCPs(Lincs_website_enrichment_class fda_enrichment)
        {
            fda_enrichment.Enrich = fda_enrichment.Enrich.OrderBy(l => l.Drug_fullName).ThenBy(l => l.Enrichment_rank).ToArray();
            int enrich_length = fda_enrichment.Enrich.Length;
            Drug_cards_line_class drug_card_line;
            List<Drug_cards_line_class> add_drug_cards = new List<Drug_cards_line_class>();
            Lincs_website_enrichment_line_class fda_enrichment_line;
            fda_enrichment.Enrich = fda_enrichment.Enrich.OrderBy(l => l.Drug_fullName).ThenBy(l => l.Enrichment_rank).ToArray();
            int scps_added = 0;
            Dictionary<DE_entry_enum, List<Lincs_website_enrichment_line_class>> entrytype_cardiotoxicScps_dict = new Dictionary<DE_entry_enum, List<Lincs_website_enrichment_line_class>>();
            Dictionary<DE_entry_enum, int> entrytype_cardiotoxicScpsCount_dict = new Dictionary<DE_entry_enum, int>();
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;
            StringBuilder sb_entityClass = new StringBuilder();
            StringBuilder sb_entity = new StringBuilder();
            List<string> entities = new List<string>();
            float max_rank = -1;
            for (int indexFDA=0; indexFDA<enrich_length; indexFDA++)
            {
                fda_enrichment_line = fda_enrichment.Enrich[indexFDA];
                if (  (indexFDA==0)
                    ||(!fda_enrichment_line.Drug_fullName.Equals(fda_enrichment.Enrich[indexFDA-1].Drug_fullName)))
                {
                    scps_added = 0;
                    entrytype_cardiotoxicScps_dict.Clear();
                    entrytype_cardiotoxicScpsCount_dict.Clear();
                }
                if (  (!String.IsNullOrEmpty(fda_enrichment_line.SCP_cardiotoxicity))
                    &&(fda_enrichment_line.Enrichment_rank<=Options.Max_enrichmentRank_cardiotoxic_mbco_level3_scps))
                {
                    if (!entrytype_cardiotoxicScpsCount_dict.ContainsKey(fda_enrichment_line.Entry_type))
                    { entrytype_cardiotoxicScpsCount_dict.Add(fda_enrichment_line.Entry_type, 0); }
                    entrytype_cardiotoxicScpsCount_dict[fda_enrichment_line.Entry_type]++;
                }
                if (fda_enrichment_line.Enrichment_rank > max_rank)
                {
                    max_rank = fda_enrichment_line.Enrichment_rank;
                }

                if (   (!String.IsNullOrEmpty(fda_enrichment_line.SCP_cardiotoxicity))
                    && (scps_added < Options.Top_considered_cardiotoxic_scps))
                {
                    scps_added++;
                    if (!entrytype_cardiotoxicScps_dict.ContainsKey(fda_enrichment_line.Entry_type))
                    { entrytype_cardiotoxicScps_dict.Add(fda_enrichment_line.Entry_type, new List<Lincs_website_enrichment_line_class>()); }
                    entrytype_cardiotoxicScps_dict[fda_enrichment_line.Entry_type].Add(fda_enrichment_line);
                }
                if ((indexFDA == enrich_length-1)
                    || (!fda_enrichment_line.Drug_fullName.Equals(fda_enrichment.Enrich[indexFDA + 1].Drug_fullName)))
                {
                    entryTypes = entrytype_cardiotoxicScpsCount_dict.Keys.ToArray();
                    entryTypes = entryTypes.OrderBy(l => l).ToArray();
                    entryTypes_length = entryTypes.Length;
                    if (entryTypes_length > 0)
                    {
                        drug_card_line = new Drug_cards_line_class();
                        drug_card_line.Drug_fullName = (string)fda_enrichment_line.Drug_fullName.Clone();
                        drug_card_line.Drug_type = fda_enrichment_line.Drug_type;
                        sb_entityClass.Clear();
                        sb_entityClass.AppendFormat("# ");
                        sb_entity.Clear();
                        for (int indexET = 0; indexET < entryTypes_length; indexET++)
                        {
                            entryType = entryTypes[indexET];
                            if (indexET != 0)
                            {
                                sb_entityClass.AppendFormat("- and ");
                                sb_entity.AppendFormat(" and ");
                            }
                            switch (entryType)
                            {
                                case DE_entry_enum.Diffrna_up:
                                    sb_entityClass.AppendFormat("up");
                                    break;
                                case DE_entry_enum.Diffrna_down:
                                    sb_entityClass.AppendFormat("down");
                                    break;
                                default:
                                    throw new Exception();
                            }
                            sb_entity.AppendFormat("{0}", entrytype_cardiotoxicScpsCount_dict[entryType]);
                        }
                        if (entryTypes_length > 1) { sb_entity.AppendFormat(", respectively"); }
                        sb_entityClass.AppendFormat("regulated cardiotoxic pathways:");
                        drug_card_line.Title = "Pathways associated with TKI cardiotoxicity (in human iPSC cardiomyoctyes):$";
                        drug_card_line.Subtitle = sb_entityClass.ToString();
                        drug_card_line.Entities = new string[] { sb_entity.ToString() };
                        drug_card_line.Order = OrderNo_upDownCardiotoxicPathwaysCount;
                        add_drug_cards.Add(drug_card_line);
                    }
                    entryTypes = entrytype_cardiotoxicScps_dict.Keys.ToArray();
                    entryTypes_length = entryTypes.Length;
                    for (int indexET = 0; indexET < entryTypes.Length; indexET++)
                    {
                        entryType = entryTypes[indexET];
                        drug_card_line = new Drug_cards_line_class();
                        drug_card_line.Drug_fullName = (string)fda_enrichment_line.Drug_fullName.Clone();
                        drug_card_line.Drug_type = fda_enrichment_line.Drug_type;
                        drug_card_line.Title = "Pathways associated with TKI cardiotoxicity (in human iPSC cardiomyoctyes):$";
                        switch (entryType)
                        {
                            case DE_entry_enum.Diffrna_up:
                                drug_card_line.Subtitle = "Top upregulated pathways:";
                                drug_card_line.Order = OrderNo_upCardiotoxicPathways;
                                break;
                            case DE_entry_enum.Diffrna_down:
                                drug_card_line.Subtitle = "Top downregulated pathways:";
                                drug_card_line.Order = OrderNo_downCardiotoxicPathways;
                                break;
                            default:
                                throw new Exception();
                        }
                        entities.Clear();
                        foreach (Lincs_website_enrichment_line_class selected_line in entrytype_cardiotoxicScps_dict[entryType])
                        {
                            entities.Add(selected_line.MBCO_subcellular_process + " (enrichment rank " + selected_line.Enrichment_rank + ")");
                        }
                        drug_card_line.Entities = entities.ToArray();
                        add_drug_cards.Add(drug_card_line);
                    }
                }
            }
            if (max_rank != Options.Max_enrichmentRank_cardiotoxic_mbco_level3_scps) { throw new Exception(); }

            Add_to_array(add_drug_cards.ToArray());
        }

        public void Replace_empyt_entities_by_NAs()
        {
            List<string> newEntities = new List<string>();
            foreach (Drug_cards_line_class drug_cards_line in this.Drug_cards)
            {
                newEntities.Clear();
                foreach (string entity in drug_cards_line.Entities)
                {
                    if (!String.IsNullOrEmpty(entity))
                    {
                        newEntities.Add(entity);
                    }
                }
                drug_cards_line.Entities = newEntities.ToArray();
                if (drug_cards_line.Entities.Length == 0)
                {
                    drug_cards_line.Entities = new string[] { (string)Conversion_class.No_value_string.Clone() };
                }
            }
        }

        private Drug_cards_line_class Generate_new_drug_cards_line_line_with_empty_entries(Drug_cards_line_class reference_drug_card_line, string drugName, Dictionary<string, string> drugFullName_drug_dict, Dictionary<string, Drug_type_enum> drug_drugType_dict, Dictionary<string,string> drug_isCardiotoxic_dict)
        {
            Drug_cards_line_class new_drug_card_line = new Drug_cards_line_class();
            new_drug_card_line.Subtitle = (string)reference_drug_card_line.Subtitle.Clone();
            new_drug_card_line.Title = (string)reference_drug_card_line.Title.Clone();
            new_drug_card_line.Order = reference_drug_card_line.Order;
            new_drug_card_line.Drug_fullName = (string)drugName.Clone();
            new_drug_card_line.Drug = (string)drugFullName_drug_dict[new_drug_card_line.Drug_fullName];
            new_drug_card_line.Drug_type = drug_drugType_dict[new_drug_card_line.Drug];
            new_drug_card_line.Column_no = reference_drug_card_line.Column_no;
            new_drug_card_line.Entities = new string[] { };
            if (  (  (new_drug_card_line.Order.Equals(OrderNo_upDownCardiotoxicPathwaysCount))
                   ||(new_drug_card_line.Order.Equals(OrderNo_genomicVariantsCount_cardiotoxic_SCPs))
                   ||(new_drug_card_line.Order.Equals(OrderNo_genomicVariants_cardiotoxicSCPs))
                   )
                && (  (!drug_isCardiotoxic_dict[new_drug_card_line.Drug].Equals("Yes")))
                    ||(  (!new_drug_card_line.Drug_type.Equals(Drug_type_enum.Kinase_inhibitor))
                       && (!new_drug_card_line.Drug_type.Equals(Drug_type_enum.Monoclonal_antibody))))
            {
                new_drug_card_line.Entities = new string[] { (string)Conversion_class.No_value_string.Clone() };
            }
            return new_drug_card_line;
        }


        public void Add_missing_entries_for_each_drug_based_on_order_nos()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            drug_legend.Add_missing_cardiotoxicity_from_faers();
            Dictionary<string,string> drugFullName_drug_dict = drug_legend.Get_drugFullName_drug_dict();
            Dictionary<string, Drug_type_enum> drug_drugType_dict = drug_legend.Get_drug_drugType_dictionary();
            Dictionary<string,string> drug_isCardiotoxic_dict = drug_legend.Get_drug_isCardiotoxic_dictionary();

            string[] drugNames = Get_all_drugNames();
            string drugName;
            int drugNames_length = drugNames.Length;
            int indexDrug = -1;
            int drugCompare;
            this.Drug_cards = this.Drug_cards.OrderBy(l => l.Order).ThenBy(l=>l.Drug_fullName).ToArray();
            int drug_cards_length = this.Drug_cards.Length;
            Drug_cards_line_class drug_card_line;
            Drug_cards_line_class new_drug_card_line;
            List<Drug_cards_line_class> new_drug_cards = new List<Drug_cards_line_class>();
            for (int indexDC=0; indexDC<drug_cards_length; indexDC++)
            {
                drug_card_line = this.Drug_cards[indexDC];
                if (  (indexDC==0)
                    ||(!drug_card_line.Order.Equals(this.Drug_cards[indexDC-1].Order)))
                {
                    indexDrug = 0;
                }
                drugCompare = -2;
                while ((indexDrug<drugNames_length)&&(drugCompare<0))
                {
                    drugName = drugNames[indexDrug];
                    drugCompare = drugName.CompareTo(drug_card_line.Drug_fullName);
                    if (drugCompare<0)
                    {
                        new_drug_card_line = Generate_new_drug_cards_line_line_with_empty_entries(drug_card_line, drugName, drugFullName_drug_dict, drug_drugType_dict, drug_isCardiotoxic_dict);
                        new_drug_cards.Add(new_drug_card_line);
                        indexDrug++;
                    }
                    else if (drugCompare==0)
                    {
                        indexDrug++;
                    }
                }
                if ((indexDC == drug_cards_length-1)
                    || (!drug_card_line.Order.Equals(this.Drug_cards[indexDC + 1].Order)))
                {
                    while (indexDrug<drugNames_length)
                    {
                        drugName = drugNames[indexDrug];
                        new_drug_card_line = Generate_new_drug_cards_line_line_with_empty_entries(drug_card_line, drugName, drugFullName_drug_dict, drug_drugType_dict, drug_isCardiotoxic_dict);
                        new_drug_cards.Add(new_drug_card_line);
                        indexDrug++;
                    }
                }
            }
            this.Add_to_array(new_drug_cards.ToArray());
        }

        public void Remove_selected_drugFullNames(string[] remove_drugFullNames)
        {
            Dictionary<string, bool> remove_drugFullName_dict = new Dictionary<string, bool>();
            foreach (string remove_drugFullName in remove_drugFullNames)
            {
                remove_drugFullName_dict.Add(remove_drugFullName, true);
            }
            List<Drug_cards_line_class> keep = new List<Drug_cards_line_class>();
            foreach (Drug_cards_line_class drugCards_line in this.Drug_cards)
            {
                if (!remove_drugFullName_dict.ContainsKey(drugCards_line.Drug_fullName))
                {
                    keep.Add(drugCards_line);
                }
            }
            this.Drug_cards = keep.ToArray();
        }

        private string[] Get_all_drugNames()
        {
            Dictionary<string, bool> drugName_dict = new Dictionary<string, bool>();
            foreach (Drug_cards_line_class drugCards_line in this.Drug_cards)
            {
                if (!drugName_dict.ContainsKey(drugCards_line.Drug_fullName))
                {
                    drugName_dict.Add(drugCards_line.Drug_fullName, true);
                }
            }
            return drugName_dict.Keys.ToArray();
        }

        public void Write(string subdirectory, string fileName, int topDEGs_for_calculations)
        {
            Set_top_degs_for_calculation_or_check_if_the_same(topDEGs_for_calculations);
            Drug_cards_readWriteOptions_class readWriteOptions = new Drug_cards_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Drug_cards, readWriteOptions);
        }

        public void Write_one_file_foreach_drug(string subdirectory, int topDEGs_for_calculations)
        {
            Set_top_degs_for_calculation_or_check_if_the_same(topDEGs_for_calculations);
            this.Drug_cards =  this.Drug_cards.OrderBy(l=>l.Drug_fullName).ThenBy(l=>l.Order).ToArray();
            List<Drug_cards_line_class> currentDrug_lines = new List<Drug_cards_line_class>();
            Drug_cards_line_class drug_card_line;
            int drug_cards_length = this.Drug_cards.Length;
            for (int indexDC =0; indexDC<drug_cards_length; indexDC++)
            {
                drug_card_line = this.Drug_cards[indexDC];
                if (  (indexDC==0)
                    || (!drug_card_line.Drug_fullName.Equals(this.Drug_cards[indexDC - 1].Drug_fullName)))
                {
                    currentDrug_lines.Clear();
                }
                currentDrug_lines.Add(drug_card_line);
                if ((indexDC == drug_cards_length-1)
                    || (!drug_card_line.Drug_fullName.Equals(this.Drug_cards[indexDC + 1].Drug_fullName)))
                {
                    string fileName = drug_card_line.Drug_fullName + ".txt";
                    Drug_cards_for_FDA_singleDrugFile_readWriteOptions_class readWriteOptions = new Drug_cards_for_FDA_singleDrugFile_readWriteOptions_class(subdirectory, fileName);
                    ReadWriteClass.WriteData(currentDrug_lines.ToArray(), readWriteOptions);
                }
            }
        }

        public void Write_for_website_linear(string subdirectory, string fileName, int topDEGs_for_calculations)
        {
            Set_top_degs_for_calculation_or_check_if_the_same(topDEGs_for_calculations);
            Drug_cards_for_website_readWriteOptions_class readWriteOptions = new Drug_cards_for_website_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Drug_cards, readWriteOptions);
        }

        public void Write_for_website(string subdirectory, string fileName, int topDEGs_for_calculations)
        {
            Set_top_degs_for_calculation_or_check_if_the_same(topDEGs_for_calculations);

            List<Drug_cards_line_class> write_drugCards_list = new List<Drug_cards_line_class>();
            Drug_cards_line_class drugCards_line;
            Drug_cards_line_class new_drugCards_line;
            int drug_cards_length = this.Drug_cards.Length;
            string[] drugNames = Get_all_drugNames();
            string drugName;
            int drugNames_length = drugNames.Length;
            this.Drug_cards = this.Drug_cards.OrderBy(l => l.Order).ToArray();
            int indexDrug = 0;
            int drugCompare = -2;
            for (int indexDC=0; indexDC< drug_cards_length; indexDC++)
            {
                drugCards_line = this.Drug_cards[indexDC];
                if (  (indexDC==0)
                    || (!drugCards_line.Order.Equals(this.Drug_cards[indexDC-1].Order)))
                {
                    indexDrug = 0;
                }
                drugCompare = -2;
                while (  (indexDrug<drugNames_length)&&(drugCompare<0))
                {
                    drugName = drugNames[indexDrug];
                    drugCompare = drugName.CompareTo(drugCards_line.Drug_fullName);
                    if (drugCompare<0) 
                    { 
                        new_drugCards_line = new Drug_cards_line_class();
                        new_drugCards_line.Title = (string)drugCards_line.Title.Clone();
                        new_drugCards_line.Subtitle = (string)drugCards_line.Subtitle.Clone();
                        new_drugCards_line.Drug_fullName = (string)drugName.Clone();
                        new_drugCards_line.Drug_type = Drug_type_enum.E_m_p_t_y;
                        new_drugCards_line.Entities = new string[0];
                        new_drugCards_line.Order = drugCards_line.Order;
                        write_drugCards_list.Add(new_drugCards_line);
                        indexDrug++;
                    }
                    else if (drugCompare==0) { indexDrug++; }
                }
                if ((indexDC == drug_cards_length - 1)
                    || (!drugCards_line.Order.Equals(this.Drug_cards[indexDC + 1].Order)))
                {
                    while (indexDrug<drugNames_length)
                    {
                        drugName = drugNames[indexDrug];
                        new_drugCards_line = new Drug_cards_line_class();
                        new_drugCards_line.Title = (string)drugCards_line.Title.Clone();
                        new_drugCards_line.Subtitle = (string)drugCards_line.Subtitle.Clone();
                        new_drugCards_line.Drug_fullName = (string)drugName.Clone();
                        new_drugCards_line.Drug_type = Drug_type_enum.E_m_p_t_y;
                        new_drugCards_line.Entities = new string[] {"No data"};
                        write_drugCards_list.Add(new_drugCards_line);
                        indexDrug++;
                    }
                }
                write_drugCards_list.Add(drugCards_line);
            }
            Drug_cards_line_class[] write_drugCards = write_drugCards_list.ToArray();


            #region Add column nos
            write_drugCards = write_drugCards.OrderBy(l => l.Order).ToArray();
            int write_drugCards_length = write_drugCards.Length;
            int columnNo = -1;
            for (int indexDC=0; indexDC< write_drugCards_length; indexDC++)
            {
                drugCards_line = write_drugCards[indexDC];
                if (  (indexDC==0)
                    ||(!drugCards_line.Title.Equals(write_drugCards[indexDC-1].Title)))
                {
                    columnNo++;
                }
                drugCards_line.Column_no = columnNo;
            }
            #endregion

            #region Write headline
            char delimiter = Global_class.Tab;
            write_drugCards = write_drugCards.OrderBy(l => l.Drug_fullName).ThenBy(l => l.Order).ToArray();
            string complete_fileName = Global_directory_class.Results_directory + subdirectory + fileName;
            System.IO.StreamWriter writer = new System.IO.StreamWriter(complete_fileName);
            for (int indexDC = 0; indexDC < write_drugCards_length; indexDC++)
            {
                drugCards_line = write_drugCards[indexDC];
                if (  (indexDC==0)
                    || (!drugCards_line.Title.Equals(write_drugCards[(indexDC-1)].Title)))
                {
                    if (indexDC!=0) { writer.Write(delimiter); }
                    writer.Write(drugCards_line.Title);
                }
                if (  (indexDC==write_drugCards_length-1)
                    ||(!drugCards_line.Drug_fullName.Equals(write_drugCards[indexDC+1].Drug_fullName)))
                {
                    writer.WriteLine();
                    break;
                }
            }
            #endregion

            #region Write all rows
            write_drugCards = write_drugCards.OrderBy(l=>l.Drug_fullName).ThenBy(l => l.Order).ToArray();
            StringBuilder sb = new StringBuilder();
            char subtitle_delimiter = '$';
            for (int indexDC = 0; indexDC < write_drugCards_length; indexDC++)
            {
                drugCards_line = write_drugCards[indexDC];
                if ((indexDC == 0)
                    || (!drugCards_line.Drug_fullName.Equals(write_drugCards[indexDC - 1].Drug_fullName)))
                {
                }
                if ((indexDC == 0)
                    || (!drugCards_line.Drug_fullName.Equals(write_drugCards[indexDC - 1].Drug_fullName))
                    || (!drugCards_line.Title.Equals(write_drugCards[indexDC - 1].Title)))
                {
                    sb.Clear();
                }
                if (sb.Length > 0) { sb.AppendFormat("{0}", subtitle_delimiter); }
                if (!String.IsNullOrEmpty(drugCards_line.Subtitle))
                {
                    if (drugCards_line.Subtitle.IndexOf('$') == drugCards_line.Subtitle.Length - 1) { sb.AppendFormat("{0}", drugCards_line.Subtitle); }
                    else { sb.AppendFormat("{0} ", drugCards_line.Subtitle); }
                }
                sb.AppendFormat("{0}", drugCards_line.ReadWrite_entities);
                if ((indexDC == write_drugCards_length - 1)
                    || (!drugCards_line.Drug_fullName.Equals(write_drugCards[indexDC + 1].Drug_fullName))
                    || (!drugCards_line.Title.Equals(write_drugCards[indexDC + 1].Title)))
                {
                    if (drugCards_line.Column_no != 0) { writer.Write("{0}", delimiter); }
                    writer.Write("{0}", sb.ToString());
                }
                if ((indexDC == write_drugCards_length - 1)
                    || (!drugCards_line.Drug_fullName.Equals(write_drugCards[indexDC + 1].Drug_fullName)))
                {
                    writer.WriteLine();
                }
                #endregion
            }
            writer.Close();
        }

        public void Read_and_add_to_array(string subdirectory, string fileName)
        {
            Drug_cards_readWriteOptions_class readWriteOptions = new Drug_cards_readWriteOptions_class(subdirectory, fileName);
            Drug_cards_line_class[] add_drug_cards = ReadWriteClass.ReadRawData_and_FillArray<Drug_cards_line_class>(readWriteOptions);
            Add_to_array(add_drug_cards);
        }
    }

}
