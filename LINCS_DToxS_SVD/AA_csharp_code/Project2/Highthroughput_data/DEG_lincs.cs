using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gene_databases;
using ReadWrite;
using Common_classes;
using Statistic;
using Ontologies_and_GTEx;
using FAERS_analysis;
using System.Runtime.Remoting.Messaging;

namespace Highthroughput_data
{
    enum Replicate_data_type_enum { E_m_p_t_y, Raw_read_counts, Normalized_read_counts }
    enum Lincs_background_correction_method_enum
    {
        E_m_p_t_y, Same_plate_by_rank, Same_plate_same_cellline_by_rank, Same_plate_by_pvalue, Same_plate_same_cellline_by_pvalue,
        Same_plate_by_minusLog10pvalue_max, Same_plate_by_minusLog10pvalue_q001,
        Same_plate_by_minusLog10pvalue_q005, Same_plate_by_minusLog10pvalue_q01,
        Same_plate_by_minusLog10pvalue_q05, Same_plate_by_minusLog10pvalue_q1, Same_plate_by_minusLog10pvalue_q5
    }
    enum Drug_type_enum
    {
        Kinase_inhibitor_or_monoclonal_antibody,
        E_m_p_t_y, All, Offender, Mitigator, Offender_plus_mitigator, Nib, Antiarrhythmogenic, Antiarrhythmic, No_drug, Control, Control_median, Combined_drug_types,
        Non_nib, Kinase_inhibitor, Left_ventricle_genes, Gtex, Sympathomimetic, Anthracycline, Hormone, Other, Randomization, Nib_high_risk, Nib_medium_risk, Nib_low_risk,
        Other_chemotherapy_drug, Antiviral, Antidiabetic, Vasodilatator, Antiemetic, Bisphosphonate, Cephalosporin, Immunosuppressant, Nsaid, Secondary_bile_acid,
        Gastrointestinal_stimulant, Antidiarrheal, Antidepressant, Appetite_suppressant, Antihypertensive, Glucocorticoid, Averaged_randomization, Antiproteasome,
        Hormone_antagonist, Cytokine, Antiasthmatic, Female_sex_hormone, Endogenous_vasoconstrictor, Monoclonal_antibody,
        Cardiovascular_drug, Non_cardiovascular_drug, Eigenassay,
        Cardiac_acting_drug, Non_cardiac_acting_drug,
        Metadata
    }
    enum Lincs_molecularEntity_enum { E_m_p_t_y, Rna }
    enum Add_ncbi_symbols_origin_enum { E_m_p_t_y, Ncbi };
    enum DEG_harmonization_between_samples_enum { E_m_p_t_y, Add_missing_refSeq_genes };

    class Lincs_website_conversion_class
    {
        public static string Label_drugName = "Drug name";
        public static string Label_approval_date = "Approval data";
        public static string Label_cell_line = "Cell line";
        public static string Label_cell_type = "Cell line";
        public static string Label_signed_minus_log10pvalue = "Signed -log10(pvalue)";
        public static string Label_plate = "Sequencing experiment number";
        public static string Label_brand_name = "Brand name";
        public static string Label_drug_class = "Drug class";
        public static string Label_is_cardiotoxic = "Is cardiotoxic";
        public static string Label_human_target_proteins = "Human targets";
        public static string Label_dataset = "Dataset";
        public static string Label_gene_symbol = "NCBI gene symbol";
        public static string Label_overlap_gene_symbols = "NCBI gene symbols";
        public static string Label_log2fc = "log2(fold change)";
        public static string Label_meanLog2fc = "Mean log2(fold change)";
        public static string Label_fdr = "FDR";
        public static string Label_pvalue = "P-value";
        public static string Label_mean_fdr = "Mean FDR";
        public static string Label_no_of_cell_lines_sig_gene_expression = "No. of cell lines with significant gene expression";
        public static string Label_no_of_cell_lines_total = "No. of cell lines";
        public static string Label_sign_cutoff = "Significance cutoff (FDR)";
        public static string Label_sign_genes_count = "No. of significant genes";
        public static string Label_entryType = "Up- or downregulated";
        public static string Label_up_down_status = "Up or Down";
        public static string Label_fractional_rank = "Significance rank";

        public static string Get_drugClass_for_website(Drug_type_enum drugType)
        {
            switch (drugType)
            {
                case Drug_type_enum.Other_chemotherapy_drug:
                    return "Antineoplastic";
                case Drug_type_enum.Antiarrhythmic:
                case Drug_type_enum.Antiarrhythmogenic:
                    return "Antiarrhythmic";
                case Drug_type_enum.Antiviral:
                case Drug_type_enum.Anthracycline:
                case Drug_type_enum.Antiproteasome:
                case Drug_type_enum.Endogenous_vasoconstrictor:
                case Drug_type_enum.Female_sex_hormone:
                case Drug_type_enum.Antidiabetic:
                case Drug_type_enum.Sympathomimetic:
                case Drug_type_enum.Antidiarrheal:
                case Drug_type_enum.Antiemetic:
                case Drug_type_enum.Bisphosphonate:
                case Drug_type_enum.Gastrointestinal_stimulant:
                case Drug_type_enum.Immunosuppressant:
                case Drug_type_enum.Antidepressant:
                case Drug_type_enum.Secondary_bile_acid:
                case Drug_type_enum.Appetite_suppressant:
                case Drug_type_enum.Antiasthmatic:
                case Drug_type_enum.Antihypertensive:
                case Drug_type_enum.Hormone_antagonist:
                case Drug_type_enum.Cytokine:
                case Drug_type_enum.Glucocorticoid:
                case Drug_type_enum.Hormone:
                case Drug_type_enum.Vasodilatator:
                case Drug_type_enum.Kinase_inhibitor:
                case Drug_type_enum.Monoclonal_antibody:
                case Drug_type_enum.Cardiovascular_drug:
                case Drug_type_enum.Non_cardiovascular_drug:
                case Drug_type_enum.Eigenassay:
                    return drugType.ToString().Replace("_", " ");
                case Drug_type_enum.Cephalosporin:
                    return "Antibiotics";
                case Drug_type_enum.Nsaid:
                    return "Anti-inflammatory";
                default:
                    throw new Exception();
            }
        }

        public static string Get_relationOfGeneSymbolToDrugTargetProtein_description(Relation_of_gene_symbol_to_drug_enum relation)
        {
            Dictionary<Relation_of_gene_symbol_to_drug_enum, string> relation_relationDescriptionString_dict = new Dictionary<Relation_of_gene_symbol_to_drug_enum, string>();
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Drug_target_protein, "Drug target protein");
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Tf_regulating_drug_target_protein, "TFs regulating drug target protein");
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_drug_target_protein, "Kinase regulating drug target protein");
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Transporter, "Transporter");
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Tf_regulating_transporter, "TFs regulating transporter");
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_transporter, "Kinase regulating transporter");
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Enzyme, "Enzyme");
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Tf_regulating_enzyme, "TFs regulating enzyme");
            relation_relationDescriptionString_dict.Add(Relation_of_gene_symbol_to_drug_enum.Kinase_regulating_enzyme, "Kinase regulating enzyme");
            return relation_relationDescriptionString_dict[relation];
        }

        public static string Get_location_of_variant_description(Variant_location_refGene_enum variant_location)
        {
            Dictionary<Variant_location_refGene_enum, string> variantLocation_descriptionString_dict = new Dictionary<Variant_location_refGene_enum, string>();
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Downstream, "Downstream");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Upstream, "Upstream");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Exonic, "Exonic");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Intergenic, "Intergenic");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Intronic, "Intronic");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Ncrna_exonic, "Ncrna exonic");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Ncrna_intronic, "Ncrna intronic");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Ncrna_splicing, "Ncrna splicing");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Not_determined, "Not determined");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Splicing, "Splicing");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Utr3, "3' untranslated region");
            variantLocation_descriptionString_dict.Add(Variant_location_refGene_enum.Utr5, "5' untranslated region");
            return variantLocation_descriptionString_dict[variant_location];
        }

        public static string Get_entryType_description(DE_entry_enum entry_type)
        {
            Dictionary<DE_entry_enum, string> entryType_string_dict = new Dictionary<DE_entry_enum, string>();
            entryType_string_dict.Add(DE_entry_enum.Diffrna_up, "Up");
            entryType_string_dict.Add(DE_entry_enum.Diffrna_down, "Down");
            entryType_string_dict.Add(DE_entry_enum.Diffrna, "Expressed");
            if (!entryType_string_dict.ContainsKey(entry_type))
            {
                throw new Exception();
            }
            else { return entryType_string_dict[entry_type]; }
        }

        public static string Get_plate_for_website(string plate)
        {
            switch (plate)
            {
                case "Plate.0":
                case "Plate.1":
                case "Plate.2":
                case "Plate.4":
                case "Plate.5":
                case "Plate.6":
                case "Plate.7":
                case "Plate.1&7":
                    return plate.Replace("Plate.", "#");
                case "Plate.LFQ":
                    return plate.Replace("Plate.", "");
                case "Combined plates":
                    return "Combined plates";
                default:
                    throw new Exception();
            }
        }

        public static string Get_cellLine_for_website(string cell_type, string cellline, string plate)
        {
            switch (plate)
            {
                case "Plate.0":
                case "Plate.1":
                case "Plate.2":
                case "Plate.4":
                case "Plate.5":
                case "Plate.6":
                case "Plate.7":
                case "Plate.1&7":
                case "Plate.LFQ":
                    string[] splitStrings = cellline.Replace("Cell_line.", "").Split('_');
                    string cell_line_for_website_events = "";
                    if (splitStrings.Length==2) { cell_line_for_website_events = splitStrings[0]; }
                    else if (splitStrings.Length==3) { cell_line_for_website_events = splitStrings[0] + " " + splitStrings[2]; }
                    return cell_line_for_website_events;
                case "Combined plates":
                    return "Combined cell lines";
                default:
                    throw new Exception();
            }
        }
    }

    class Combine_individual_files_column_assignment_documentation_line_class
    {
        public string Dataset { get; set; }
        public string ColumnName { get; set; }
        public string ColumnAssignment { get; set; }
        public string Lincs_subdirectory { get; set; }
        public string File_name { get; set; }
        public string Treatment { get; set; }
        public string Cell_line { get; set; }
        public DE_entry_enum Entry_type { get; set; }
        public Timepoint_enum Timepoint { get; set; }

        public Combine_individual_files_column_assignment_documentation_line_class()
        {
            Dataset = "";
            ColumnName = "";
            ColumnAssignment = "";
            Lincs_subdirectory = "";
            File_name = "";
            Treatment = "";
            Cell_line = "";
        }

        public Combine_individual_files_column_assignment_documentation_line_class Deep_copy()
        {
            Combine_individual_files_column_assignment_documentation_line_class copy = (Combine_individual_files_column_assignment_documentation_line_class)this.MemberwiseClone();
            copy.Dataset = (string)this.Dataset.Clone();
            copy.ColumnName = (string)this.ColumnName.Clone();
            copy.ColumnAssignment = (string)this.ColumnAssignment.Clone();
            copy.Lincs_subdirectory = (string)this.Lincs_subdirectory.Clone();
            copy.File_name = (string)this.File_name.Clone();
            copy.Treatment = (string)this.Treatment.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            return copy;
        }
    }

    class Combine_individual_files_column_assignment_documentation_readWriteOptions_class : ReadWriteOptions_base
    {
        public Combine_individual_files_column_assignment_documentation_readWriteOptions_class(string fileName)
        {
            this.File = Global_directory_class.Results_directory + fileName;
            this.Key_propertyNames = new string[] { "Dataset", "Lincs_subdirectory", "File_name", "ColumnName", "ColumnAssignment", "Treatment", "Cell_line", "Entry_type", "Timepoint" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    ////////////////////////////////////////////////////////////////////

    class Lincs_extract_information_from_edgeR_files_class
    {
        private string Dataset { get; set; }

        public Lincs_extract_information_from_edgeR_files_class(string dataset)
        {
            this.Dataset = (string)dataset.Clone();
        }

        public void Get_patient_plate_treatment_entryType_timepoint(out string patient, out string plate, out string treatment, out DE_entry_enum entryType, out Timepoint_enum timepoint, string complete_file_name)
        {
            string file_name = Path.GetFileNameWithoutExtension(complete_file_name);
            string[] splitStrings = file_name.Split('-');

            patient = "";
            plate = "";
            treatment = "";
            string timepoint_string;
            timepoint = Timepoint_enum.H48;
            entryType = DE_entry_enum.Rna;
            string use_file_name;


            switch (Dataset)
            {
                case "DEGenes_iPSCdCMs_P0":
                    use_file_name = file_name.Replace(".", "-");
                    splitStrings = use_file_name.Split('-');
                    patient = "Cell_line" + "." + splitStrings[1] + "_" + splitStrings[2];
                    plate = "Plate." + splitStrings[7];
                    treatment = splitStrings[10];
                    timepoint_string = splitStrings[4] + "." + splitStrings[5];
                    entryType = DE_entry_enum.Diffrna;
                    break;
                case "DEGenes_iPSCdCMs_ECCoCulture":
                    use_file_name = file_name.Replace(".", "-");
                    splitStrings = use_file_name.Split('-');
                    patient = "Cell_line" + "." + splitStrings[1] + "_" + splitStrings[2] + "_" + splitStrings[3];
                    plate = "Plate." + splitStrings[7];
                    treatment = splitStrings[10];
                    timepoint_string = splitStrings[4] + "." + splitStrings[5];
                    entryType = DE_entry_enum.Diffrna;
                    break;
                default:
                    throw new Exception();
            }

            switch (timepoint_string)
            {
                case "Hour.48":
                case "Time.48":
                case "48hrs":
                    timepoint = Timepoint_enum.H48;
                    break;
                default:
                    throw new Exception();
            }

        }

        public string Get_full_treatment(string complete_file_name)
        {
            string patient;
            string plate;
            DE_entry_enum entryType;
            Timepoint_enum timepoint;
            string treatment;
            Get_patient_plate_treatment_entryType_timepoint(out patient, out plate, out treatment, out entryType, out timepoint, complete_file_name);
            return treatment;
        }

        public string Get_treatment_ignoring_experiment_following_dot(string complete_file_name)
        {
            string treatment = Get_full_treatment(complete_file_name);
            treatment = treatment.Split('.')[0];
            return treatment;
        }


    }

    class Lincs_column_indexes_class
    {
        public int LogFC_column_index { get; private set; }
        public int LogCPM_column_index { get; private set; }
        public int Pvalue_column_index { get; private set; }
        public int Fdr_column_index { get; private set; }
        public int Symbol_column_index { get; private set; }
        public int Regulation_column_index { get; private set; }
        public int Likelihood_ratio_index { get; private set; }
        public int[] Control_column_indexes { get; private set; }
        public int[] Control_norm_column_indexes { get; private set; }
        public int[] Treatment_column_indexes { get; private set; }
        public int[] Treatment_norm_column_indexes { get; private set; }
        public string[] Column_names_in_correct_order { get; private set; }
        public int Column_names_length { get; private set; }

        public Combine_individual_files_column_assignment_documentation_line_class[] Column_assignment_documentations { get; set; }

        public Lincs_column_indexes_class()
        {
            this.Column_assignment_documentations = new Combine_individual_files_column_assignment_documentation_line_class[0];
        }

        public void Add_to_column_assignment_documentations(Combine_individual_files_column_assignment_documentation_line_class[] add_column_assignment_documentations)
        {
            int add_length = add_column_assignment_documentations.Length;
            int this_length = this.Column_assignment_documentations.Length;
            int new_length = add_length + this_length;
            Combine_individual_files_column_assignment_documentation_line_class[] new_column_assignemnent_documentations = new Combine_individual_files_column_assignment_documentation_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_column_assignemnent_documentations[indexNew] = this.Column_assignment_documentations[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_column_assignemnent_documentations[indexNew] = add_column_assignment_documentations[indexAdd];
            }
            this.Column_assignment_documentations = new_column_assignemnent_documentations;
        }

        private void Reset_all_indexes()
        {
            LogFC_column_index = -1;
            LogCPM_column_index = -1;
            Pvalue_column_index = -1;
            Fdr_column_index = -1;
            Symbol_column_index = -1;
            Regulation_column_index = -1;
            Control_column_indexes = new int[0];
            Control_norm_column_indexes = new int[0];
            Treatment_column_indexes = new int[0];
            Treatment_norm_column_indexes = new int[0];
            Column_names_length = -1;
            Likelihood_ratio_index = -1;
        }

        public void Set_all_column_indexes_from_headline(ref string[] columnNames, string dataset, string complete_file_name)
        {
            Reset_all_indexes();
            Lincs_extract_information_from_edgeR_files_class extract_information = new Lincs_extract_information_from_edgeR_files_class(dataset);
            string cellline;
            string drug;
            string plate;
            Timepoint_enum timepoint;
            DE_entry_enum entryType;
            extract_information.Get_patient_plate_treatment_entryType_timepoint(out cellline, out plate, out drug, out entryType, out timepoint, complete_file_name);
            drug = extract_information.Get_full_treatment(complete_file_name);

            List<int> control_column_indexes_list = new List<int>();
            List<int> treatment_column_indexes_list = new List<int>();
            List<int> control_norm_column_indexes_list = new List<int>();
            List<int> treatment_norm_column_indexes_list = new List<int>();
            string columnName;
            int indexColumn_in_lines = 0;
            bool indexDesion_made;
            List<Combine_individual_files_column_assignment_documentation_line_class> column_assignments = new List<Combine_individual_files_column_assignment_documentation_line_class>();
            Combine_individual_files_column_assignment_documentation_line_class column_assignment_line;
            Combine_individual_files_column_assignment_documentation_line_class column_assignment_base_line;
            column_assignment_base_line = new Combine_individual_files_column_assignment_documentation_line_class();
            column_assignment_base_line.Dataset = (string)dataset.Clone();
            column_assignment_base_line.Cell_line = (string)cellline.Clone();
            column_assignment_base_line.File_name = (string)System.IO.Path.GetFileName(complete_file_name);
            column_assignment_base_line.Treatment = (string)drug.Clone();
            string lincs_directory = Global_directory_class.Major_directory;
            column_assignment_base_line.Lincs_subdirectory = Path.GetDirectoryName(complete_file_name).Replace(lincs_directory, "");
            column_assignment_base_line.Timepoint = timepoint;
            column_assignment_base_line.Entry_type = entryType;

            if (!columnNames[0].Equals("Symbol"))
            {
                List<string> columnNames_list = new List<string>();
                columnNames_list.Add("Symbol");
                columnNames_list.AddRange(columnNames);
                columnNames = columnNames_list.ToArray();
            }                                                        
            int columnNames_length = columnNames.Length;
            this.Column_names_length = columnNames_length;

            for (int indexC = 0; indexC < columnNames_length; indexC++)
            {
                indexColumn_in_lines = indexC;
                indexDesion_made = false;
                columnName = columnNames[indexC];
                if (   ((columnName.IndexOf("CTRL") != -1)||(columnName.IndexOf("DMSO")!=-1))
                    && (columnName.IndexOf(drug) == -1)
                    && (!columnName.Contains("Norm")))
                {
                    if (indexDesion_made) { throw new Exception(); }
                    control_column_indexes_list.Add(indexColumn_in_lines);
                    indexDesion_made = true;
                    column_assignment_line = column_assignment_base_line.Deep_copy();
                    column_assignment_line.ColumnAssignment = "Control";
                    column_assignment_line.ColumnName = (string)columnName.Clone();
                    column_assignments.Add(column_assignment_line);
                }
                if (   ((columnName.IndexOf("CTRL") != -1)|(columnName.IndexOf("DMSO")!=-1))
                    && (columnName.IndexOf(drug) == -1)
                    && (columnName.Contains("Norm")))
                {
                    if (indexDesion_made) { throw new Exception(); }
                    control_norm_column_indexes_list.Add(indexColumn_in_lines);
                    indexDesion_made = true;
                    column_assignment_line = column_assignment_base_line.Deep_copy();
                    column_assignment_line.ColumnAssignment = "Control norm";
                    column_assignment_line.ColumnName = (string)columnName.Clone();
                    column_assignments.Add(column_assignment_line);
                }
                if ((columnName.IndexOf(drug) != -1) && (!columnName.Contains("Norm")))
                {
                    if (indexDesion_made) { throw new Exception(); }
                    treatment_column_indexes_list.Add(indexColumn_in_lines);
                    indexDesion_made = true;
                    column_assignment_line = column_assignment_base_line.Deep_copy();
                    column_assignment_line.ColumnAssignment = "Treatment";
                    column_assignment_line.ColumnName = (string)columnName.Clone();
                    column_assignments.Add(column_assignment_line);
                }
                if ((columnName.IndexOf(drug) != -1) && (columnName.Contains("Norm")))
                {
                    if (indexDesion_made) { throw new Exception(); }
                    treatment_norm_column_indexes_list.Add(indexColumn_in_lines);
                    indexDesion_made = true;
                    column_assignment_line = column_assignment_base_line.Deep_copy();
                    column_assignment_line.ColumnAssignment = "Treatment norm";
                    column_assignment_line.ColumnName = (string)columnName.Clone();
                    column_assignments.Add(column_assignment_line);
                }
                switch (columnName)
                {
                    case "Symbol":
                        if (indexDesion_made) { throw new Exception(); }
                        if (indexColumn_in_lines != 0) { throw new Exception(); }
                        if (Symbol_column_index != -1) { throw new Exception(); }
                        Symbol_column_index = indexColumn_in_lines;
                        indexDesion_made = true;
                        break;
                    case "logFC":
                        if (indexDesion_made) { throw new Exception(); }
                        if (LogFC_column_index != -1) { throw new Exception(); }
                        LogFC_column_index = indexColumn_in_lines;
                        indexDesion_made = true;
                        break;
                    case "logCPM":
                        if (indexDesion_made) { throw new Exception(); }
                        if (LogCPM_column_index != -1) { throw new Exception(); }
                        LogCPM_column_index = indexColumn_in_lines;
                        indexDesion_made = true;
                        break;
                    case "PValue":
                        if (indexDesion_made) { throw new Exception(); }
                        if (Pvalue_column_index != -1) { throw new Exception(); }
                        Pvalue_column_index = indexColumn_in_lines;
                        indexDesion_made = true;
                        break;
                    case "FDR":
                        if (indexDesion_made) { throw new Exception(); }
                        if (Fdr_column_index != -1) { throw new Exception(); }
                        Fdr_column_index = indexColumn_in_lines;
                        indexDesion_made = true;
                        break;
                    case "Regulation":
                        if (indexDesion_made) { throw new Exception(); }
                        if (Regulation_column_index != -1) { throw new Exception(); }
                        Regulation_column_index = indexColumn_in_lines;
                        indexDesion_made = true;
                        break;
                    case "LR":
                        if (indexDesion_made) { throw new Exception(); }
                        if (Likelihood_ratio_index != -1) { throw new Exception(); }
                        Likelihood_ratio_index = indexColumn_in_lines;
                        indexDesion_made = true;
                        break;
                    default:
                        if (!indexDesion_made) { throw new Exception(); }
                        break;
                }
            }

            this.Column_names_in_correct_order = columnNames;


            this.Control_column_indexes = control_column_indexes_list.ToArray();
            this.Control_norm_column_indexes = control_norm_column_indexes_list.ToArray();
            this.Treatment_column_indexes = treatment_column_indexes_list.ToArray();
            this.Treatment_norm_column_indexes = treatment_norm_column_indexes_list.ToArray();

            if (this.Control_column_indexes.Length == 0) { throw new Exception(); }
            if (this.Control_norm_column_indexes.Length == 0) { throw new Exception(); }
            if (this.Treatment_column_indexes.Length == 0) { throw new Exception(); }
            if (this.Treatment_norm_column_indexes.Length == 0) { throw new Exception(); }

            if ((this.LogFC_column_index == -1)
                || (this.LogCPM_column_index == -1)
                || (this.Pvalue_column_index == -1)
                || (this.Fdr_column_index == -1)
                || (this.Symbol_column_index == -1)
                || (this.Control_column_indexes.Length == 0)
                || (this.Control_norm_column_indexes.Length == 0)
                || (this.Treatment_column_indexes.Length == 0)
                || (this.Treatment_norm_column_indexes.Length == 0)
                || (this.Column_names_length == -1)
                || (this.Column_names_in_correct_order.Length != Column_names_length))
            {
                throw new Exception();
            }

            Add_to_column_assignment_documentations(column_assignments.ToArray());
        }
    }

    class Lincs_combine_files_options_class
    {
        public string[] Directories { get; set; }
        public string Output_directory { get; set; }
        public string Combined_filename { get; set; }
        public string Combined_replicates_fileName { get; set; }
        public string Full_degs_marker { get; set; }
        public string Dataset { get; set; }
        public int Max_files_to_be_combined { get; set; }

        public Lincs_combine_files_options_class(string dataset)
        {
            Output_directory = Global_directory_class.Lincs_degs_non_binary_files_directory;
            Full_degs_marker = "Calc";
            Max_files_to_be_combined = 2000;

            Dataset = (string)dataset.Clone();
            Combined_filename = Dataset + ".txt";
            Combined_replicates_fileName = Dataset + "_replicateExpression.txt";
            Output_directory = Global_directory_class.Lincs_degs_non_binary_files_directory;
            switch (Dataset)
            {
                case "DEGenes_iPSCdCMs_P0":
                    Directories = new string[] { Global_directory_class.Initial_lincs_degs_P0_directory };
                    break;
                case "DEGenes_iPSCdCMs_ECCoCulture":
                    Directories = new string[] { Global_directory_class.Initial_lincs_degs_EC_directory };
                    break;
                default:
                    throw new Exception();
            }
        }

        public void Add_or_update_set_in_combinedFileName(int setNo)
        {
            int indexSet = this.Combined_filename.IndexOf("_set");
            if (indexSet == -1)
            {
                this.Combined_filename = System.IO.Path.GetFileNameWithoutExtension(this.Combined_filename) + "_setNo" + setNo + ".txt";
            }
            else
            {
                this.Combined_filename = this.Combined_filename.Substring(0, indexSet) + "_setNo" + setNo + ".txt";
            }
        }
    }

    ////////////////////////////////////////////////////////////////////

    class Deg_drug_legend_line_class
    {
        public string Drug { get; set; }
        public string Full_name { get; set; }
        public Drug_type_enum Drug_type { get; set; }
        public float Cardiotoxicity_frequencyGroup { get; set; }
        public string[] Cardiotoxixity_references { get; set; }
        public string ReadWrite_cardiotoxicity_references
        {
            get { return ReadWriteClass.Get_writeLine_from_array<string>(this.Cardiotoxixity_references, Deg_drug_legend_input_readOptions_class.Array_delimiter); }
            set { this.Cardiotoxixity_references = ReadWriteClass.Get_array_from_readLine<string>(value, Deg_drug_legend_input_readOptions_class.Array_delimiter); }
        }
        public bool Is_cardiotoxic_TKI
        {
            get
            {
                switch (Drug_type)
                {
                    case Drug_type_enum.Kinase_inhibitor:
                    case Drug_type_enum.Monoclonal_antibody:
                        if (Is_cardiotoxic.Equals("Yes")) { return true; }
                        else if (Is_cardiotoxic.Equals("No")) { return false; }
                        else { return false; };
                    default:
                        return false;
                }
            }
        }
        public bool Is_noncardiotoxic_TKI
        {
            get
            {
                switch (Drug_type)
                {
                    case Drug_type_enum.Kinase_inhibitor:
                    case Drug_type_enum.Monoclonal_antibody:
                        if (Is_cardiotoxic.Equals("No")) { return true; }
                        else if (Is_cardiotoxic.Equals("Yes")) { return false; }
                        else { return false; };
                    default:
                        return false;
                }
            }
        }
        public string Is_cardiotoxic { get; set; }


        public Deg_drug_legend_line_class()
        {
            this.Cardiotoxixity_references = new string[0];
            Is_cardiotoxic = "";
        }

        public Deg_drug_legend_line_class Deep_copy()
        {
            Deg_drug_legend_line_class copy = (Deg_drug_legend_line_class)this.MemberwiseClone();
            copy.Drug = (string)this.Drug.Clone();
            copy.Is_cardiotoxic = (string)this.Is_cardiotoxic.Clone();
            copy.Cardiotoxixity_references = Array_class.Deep_copy_string_array(this.Cardiotoxixity_references);
            return copy;
        }
    }

    class Deg_drug_legend_input_readOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }
        public Deg_drug_legend_input_readOptions_class()
        {
            File = Global_directory_class.Metadata_directory + "Drug_metadata.txt";
            Key_propertyNames = new string[] { "Drug", "Full_name", "Drug_type", "Cardiotoxicity_frequencyGroup", "ReadWrite_cardiotoxicity_references" };
            Key_columnNames = Key_propertyNames;
            File_has_headline = true;
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Deg_drug_legend_readWriteOptions_class : ReadWriteOptions_base
    {
        public Deg_drug_legend_readWriteOptions_class(string directory, string fileName)
        {
            File = directory + fileName;
            Key_propertyNames = new string[] { "Drug", "Full_name", "Drug_type", "Is_cardiotoxic", "Cardiotoxicity_frequencyGroup", "ReadWrite_cardiotoxicity_references" };
            Key_columnNames = new string[] { "Drug abbreviation", "Drug name", "Drug class", "Is_cardiotoxic", "Cardiotoxicity frequency", "References for cardiotoxicity" };
            File_has_headline = true;
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Deg_drug_legend_options_class
    {
        public int Minimum_cardiotoxicity_frequencyGroup_for_cardiotoxicity_definition { get; set; }
        public bool Simplify_drug_classes { get; set; }
        public bool Add_cardiotoxicity_to_tkis { get; set; }
        public float Cardiotoxicity_cutoff_odds_ratio_faers { get; set; }
        public string Cardiotoxity_sideEffect { get; set; }

        public Deg_drug_legend_options_class()
        {
            Minimum_cardiotoxicity_frequencyGroup_for_cardiotoxicity_definition = 2;
            Simplify_drug_classes = true;
            Add_cardiotoxicity_to_tkis = true;
            Cardiotoxity_sideEffect = "Cardiotoxicity";
            Cardiotoxicity_cutoff_odds_ratio_faers = 2F;
        }
    }

    class Deg_drug_legend_class
    {
        public Deg_drug_legend_line_class[] Legend { get; set; }
        public Deg_drug_legend_options_class Options { get; set; }

        private void Mark_offender_mitigator_combination()
        {
            int legend_length = Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                drug_legend_line = Legend[indexL];
                if (drug_legend_line.Drug.Contains("+"))
                {
                    drug_legend_line.Drug_type = Drug_type_enum.Offender_plus_mitigator;
                }
            }
        }

        public Deg_drug_legend_class()
        {
            this.Legend = new Deg_drug_legend_line_class[0];
            this.Options = new Deg_drug_legend_options_class();
        }

        #region Order
        public void Order_by_treatment()
        {
            Legend = Legend.OrderBy(l => l.Drug).ToArray();
        }
        #endregion

        public void Replace_drugType_by_given_drugType(Drug_type_enum old_drug_type, Drug_type_enum new_drug_type)
        {
            foreach (Deg_drug_legend_line_class drug_legend_line in Legend)
            {
                if (drug_legend_line.Drug_type.Equals(old_drug_type))
                {
                    drug_legend_line.Drug_type.Equals(new_drug_type);
                }
            }
        }
        public void Keep_only_selected_drugs(string[] drugs)
        {
            drugs = drugs.Distinct().OrderBy(l => l).ToArray();
            Dictionary<string, bool> drug_dict = new Dictionary<string, bool>();
            foreach (string drug in drugs)
            {
                drug_dict.Add(drug, true);
            }
            List<Deg_drug_legend_line_class> keep = new List<Deg_drug_legend_line_class>();
            foreach (Deg_drug_legend_line_class legend_line in this.Legend)
            {
                if (drug_dict.ContainsKey(legend_line.Drug))
                {
                    keep.Add(legend_line);
                }
            }
            this.Legend = keep.ToArray();
        }
        private void Set_all_fullNames_to_lowerCase_and_abbreviations_to_upperCase_except_eigenassays()
        {
            foreach (Deg_drug_legend_line_class legend_line in this.Legend)
            {
                legend_line.Full_name = legend_line.Full_name.ToLower();
                if (legend_line.Drug.IndexOf("igenassay") == -1)
                { legend_line.Drug = legend_line.Drug.ToUpper(); }
            }
        }
        private void Add_cardiotoxicity_info_from_clinical_frequency_groups()
        {
            foreach (Deg_drug_legend_line_class legend_line in this.Legend)
            {
                if (legend_line.Cardiotoxicity_frequencyGroup >= Options.Minimum_cardiotoxicity_frequencyGroup_for_cardiotoxicity_definition)
                {
                    legend_line.Is_cardiotoxic = "Yes";
                }
                else if (legend_line.Cardiotoxicity_frequencyGroup!=-1)
                {
                    legend_line.Is_cardiotoxic = "No";
                }
                else
                {
                    legend_line.Is_cardiotoxic = "ND";
                }
            }
        }
        public void Add_missing_cardiotoxicity_from_faers()
        {
            FAERS_class faers = new FAERS_class();
            faers.Generate_by_reading();
            Dictionary<string,float> drug_odds_ratio_dict = faers.Get_dictionary_with_drug_odds_ratio(Options.Cardiotoxity_sideEffect);
            foreach (Deg_drug_legend_line_class drug_legend_line in this.Legend)
            {
                if (  (string.IsNullOrEmpty(drug_legend_line.Is_cardiotoxic))
                    &&(drug_odds_ratio_dict.ContainsKey(drug_legend_line.Drug)))
                {
                    if (drug_legend_line.Cardiotoxixity_references.Length>0) { throw new Exception(); }
                    if (drug_odds_ratio_dict[drug_legend_line.Drug]>Options.Cardiotoxicity_cutoff_odds_ratio_faers)
                    {
                        drug_legend_line.Is_cardiotoxic = "Yes";
                        drug_legend_line.Cardiotoxixity_references = new string[] { "FAERS" };
                    }
                    else
                    {
                        drug_legend_line.Is_cardiotoxic = "No";
                        drug_legend_line.Cardiotoxixity_references = new string[] { "FAERS" };
                    }
                }
            }
        }
        private void Simplify_drugClasses()
        {
            Dictionary<Drug_type_enum, Drug_type_enum> oldDrugClass_newDrugClass_dict = new Dictionary<Drug_type_enum, Drug_type_enum>();
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antiarrhythmic, Drug_type_enum.Cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antiarrhythmogenic, Drug_type_enum.Cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antihypertensive, Drug_type_enum.Cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Endogenous_vasoconstrictor, Drug_type_enum.Cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Sympathomimetic, Drug_type_enum.Cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Vasodilatator, Drug_type_enum.Cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antiasthmatic, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antidepressant, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antidiabetic, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antidiarrheal, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antiemetic, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antiviral, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Appetite_suppressant, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Bisphosphonate, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Cephalosporin, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Cytokine, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Female_sex_hormone, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Gastrointestinal_stimulant, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Glucocorticoid, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Hormone, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Hormone_antagonist, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Immunosuppressant, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Nsaid, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Secondary_bile_acid, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Other_chemotherapy_drug, Drug_type_enum.Non_cardiovascular_drug);
            oldDrugClass_newDrugClass_dict.Add(Drug_type_enum.Antiproteasome, Drug_type_enum.Non_cardiovascular_drug);
            foreach (Deg_drug_legend_line_class drug_legend_line in Legend)
            {
                if (oldDrugClass_newDrugClass_dict.ContainsKey(drug_legend_line.Drug_type))
                {
                    drug_legend_line.Drug_type = oldDrugClass_newDrugClass_dict[drug_legend_line.Drug_type];
                }
            }
        }
        public void Generate_de_novo()
        {
            Read();
            Set_all_fullNames_to_lowerCase_and_abbreviations_to_upperCase_except_eigenassays();
            Mark_offender_mitigator_combination();
            if (Options.Simplify_drug_classes) { Simplify_drugClasses(); }
            this.Legend = this.Legend.OrderBy(l => l.Drug).ToArray();
            if (Options.Add_cardiotoxicity_to_tkis) 
            { 
                Add_cardiotoxicity_info_from_clinical_frequency_groups();
            }
        }
        public string[] Get_all_cardiotoxic_tkis()
        {
            List<string> drugs = new List<string>();
            foreach (Deg_drug_legend_line_class drug_legend_line in this.Legend)
            {
                if (drug_legend_line.Is_cardiotoxic_TKI)
                {
                    drugs.Add(drug_legend_line.Drug);
                }
            }
            return drugs.ToArray();
        }
        public string[] Get_all_non_cardiotoxic_tkis()
        {
            List<string> drugs = new List<string>();
            foreach (Deg_drug_legend_line_class drug_legend_line in this.Legend)
            {
                if (drug_legend_line.Is_noncardiotoxic_TKI)
                {
                    drugs.Add(drug_legend_line.Drug);
                }
            }
            return drugs.ToArray();
        }
        public string[] Get_all_drugs_of_indicated_types(params Drug_type_enum[] drug_types)
        {
            Dictionary<Drug_type_enum, string> drugType_isCardiotoxic_dict = new Dictionary<Drug_type_enum, string>();
            foreach (Drug_type_enum drug_type in drug_types)
            {
                drugType_isCardiotoxic_dict.Add(drug_type, ""); 
            }
            int legend_length = Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            List<string> drugs = new List<string>();
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                drug_legend_line = Legend[indexL];
                if (drugType_isCardiotoxic_dict.ContainsKey(drug_legend_line.Drug_type))
                {
                    drugs.Add(drug_legend_line.Drug);
                }
            }
            return drugs.ToArray();
        }
        public Dictionary<string, System.Drawing.Color> Get_drug_color_dictionary_based_on_drugType_cardiotoxicity_upregulated()
        {
            Dictionary<string, System.Drawing.Color> drug_color_dict = new Dictionary<string, System.Drawing.Color>();
            System.Drawing.Color drug_color;
            foreach (Deg_drug_legend_line_class drug_legend_line in this.Legend)
            {
                switch (drug_legend_line.Drug_type)
                {
                    case Drug_type_enum.Kinase_inhibitor:
                    case Drug_type_enum.Monoclonal_antibody:
                        if (drug_legend_line.Is_cardiotoxic_TKI)
                        { drug_color = System.Drawing.Color.Red; }
                        else
                        { drug_color = System.Drawing.Color.SkyBlue; }
                        break;
                    case Drug_type_enum.Anthracycline:
                        drug_color = System.Drawing.Color.Plum;
                        break;
                    case Drug_type_enum.Cardiovascular_drug:
                    case Drug_type_enum.Non_cardiovascular_drug:
                    default:
                        drug_color = System.Drawing.Color.Gray;
                        break;
                }
                drug_color_dict.Add(drug_legend_line.Drug, drug_color);
            }
            return drug_color_dict;
        }
        public Dictionary<string, System.Drawing.Color> Get_drug_color_dictionary_based_on_drugType_cardiotoxicity_downregulated()
        {
            Dictionary<string, System.Drawing.Color> drug_color_dict = new Dictionary<string, System.Drawing.Color>();
            System.Drawing.Color drug_color;
            foreach (Deg_drug_legend_line_class drug_legend_line in this.Legend)
            {
                switch (drug_legend_line.Drug_type)
                {
                    case Drug_type_enum.Kinase_inhibitor:
                    case Drug_type_enum.Monoclonal_antibody:
                        if (drug_legend_line.Is_cardiotoxic_TKI)
                        { drug_color = System.Drawing.Color.Blue; }
                        else { drug_color = System.Drawing.Color.Orange; }
                        break;
                    case Drug_type_enum.Anthracycline:
                        drug_color = System.Drawing.Color.DarkOrchid;
                        break;
                    case Drug_type_enum.Cardiovascular_drug:
                    case Drug_type_enum.Non_cardiovascular_drug:
                    default:
                        drug_color = System.Drawing.Color.Gray;
                        break;
                }
                drug_color_dict.Add(drug_legend_line.Drug, drug_color);
            }
            return drug_color_dict;
        }
        public string[] Get_all_drug_fullnames()
        {
            int legend_length = Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            List<string> drugs = new List<string>();
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                drug_legend_line = this.Legend[indexL];
                drugs.Add(drug_legend_line.Full_name);
            }
            return drugs.Distinct().OrderBy(l => l).ToArray();
        }
        public string[] Get_all_drugs()
        {
            int legend_length = Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            List<string> drugs = new List<string>();
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                drug_legend_line = this.Legend[indexL];
                drugs.Add(drug_legend_line.Drug);
            }
            return drugs.Distinct().OrderBy(l => l).ToArray();
        }
        public Dictionary<string, string> Get_drug_drugFullName_dict()
        {
            Dictionary<string, string> drug_drugFullName_dict = new Dictionary<string, string>();
            int legend_length = Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                drug_legend_line = this.Legend[indexL];
                if (!drug_drugFullName_dict.ContainsKey(drug_legend_line.Drug)) { drug_drugFullName_dict.Add((string)drug_legend_line.Drug.Clone(), (string)drug_legend_line.Full_name.Clone()); }
                else if (!drug_drugFullName_dict[drug_legend_line.Drug].Equals(drug_legend_line.Full_name)) { throw new Exception(); }
            }
            drug_drugFullName_dict.Add("cardiotoxic TKIs", "cardiotoxic TKIs");
            drug_drugFullName_dict.Add("non-cardiotoxic TKIs", "non-cardiotoxic TKIs");
            return drug_drugFullName_dict;
        }
        public Dictionary<string, bool> Get_drug_isCardiotoxicTKI_dict()
        {
            Dictionary<string, bool> drug_isCardiotoxicTKI_dict = new Dictionary<string, bool>();
            int legend_length = Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                drug_legend_line = this.Legend[indexL];
                if (drug_legend_line.Is_cardiotoxic_TKI)
                {
                    drug_isCardiotoxicTKI_dict.Add(drug_legend_line.Drug, true);
                }
            }
            return drug_isCardiotoxicTKI_dict;
        }
        public Dictionary<string, bool> Get_drug_isNonCardiotoxicTKI_dict()
        {
            Dictionary<string, bool> drug_isNonCardiotoxicTKI_dict = new Dictionary<string, bool>();
            int legend_length = Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                drug_legend_line = this.Legend[indexL];
                if (drug_legend_line.Is_noncardiotoxic_TKI)
                {
                    drug_isNonCardiotoxicTKI_dict.Add(drug_legend_line.Drug, true);
                }
            }
            return drug_isNonCardiotoxicTKI_dict;
        }
        public Dictionary<string, string> Get_drugFullName_drug_dict()
        {
            Dictionary<string, string> drugFullName_drug_dict = new Dictionary<string, string>();
            int legend_length = Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                drug_legend_line = this.Legend[indexL];
                if (!drugFullName_drug_dict.ContainsKey(drug_legend_line.Full_name)) { drugFullName_drug_dict.Add((string)drug_legend_line.Full_name.Clone(), (string)drug_legend_line.Drug.Clone()); }
                else if (!drugFullName_drug_dict[drug_legend_line.Full_name].Equals(drug_legend_line.Drug)) { throw new Exception(); }
            }
            return drugFullName_drug_dict;
        }
        public Dictionary<string, Drug_type_enum> Get_drug_drugType_dictionary()
        {
            Dictionary<string, Drug_type_enum> drug_drugType_dict = new Dictionary<string, Drug_type_enum>();
            foreach (Deg_drug_legend_line_class legend_line in this.Legend)
            {
                drug_drugType_dict.Add(legend_line.Drug, legend_line.Drug_type);
            }
            drug_drugType_dict.Add("cardiotoxic TKIs", Drug_type_enum.Kinase_inhibitor_or_monoclonal_antibody);
            drug_drugType_dict.Add("non-cardiotoxic TKIs", Drug_type_enum.Kinase_inhibitor_or_monoclonal_antibody);
            return drug_drugType_dict;
        }
        public Dictionary<string, string> Get_drug_isCardiotoxic_dictionary()
        {
            Dictionary<string, string> drug_isCardiotoxic_dict = new Dictionary<string, string>();
            foreach (Deg_drug_legend_line_class legend_line in this.Legend)
            {
                drug_isCardiotoxic_dict.Add(legend_line.Drug, legend_line.Is_cardiotoxic);
            }
            drug_isCardiotoxic_dict.Add("cardiotoxic TKIs", "Yes");
            drug_isCardiotoxic_dict.Add("non-cardiotoxic TKIs", "No");
            return drug_isCardiotoxic_dict;
        }
        public Dictionary<Drug_type_enum, string[]> Get_drugType_drugs_dictionary()
        {
            this.Legend = this.Legend.OrderBy(l => l.Drug_type).ToArray();
            Deg_drug_legend_line_class drug_legend_line;
            int legend_length = this.Legend.Length;
            List<string> current_drugs = new List<string>();
            Dictionary<Drug_type_enum, string[]> drugType_drug_dict = new Dictionary<Drug_type_enum, string[]>();
            for (int indexLegend = 0; indexLegend < legend_length; indexLegend++)
            {
                drug_legend_line = this.Legend[indexLegend];
                if ((indexLegend == 0)
                    || (!drug_legend_line.Drug_type.Equals(this.Legend[indexLegend - 1].Drug_type)))
                {
                    current_drugs.Clear();
                }
                current_drugs.Add(drug_legend_line.Drug);
                if ((indexLegend == legend_length - 1)
                    || (!drug_legend_line.Drug_type.Equals(this.Legend[indexLegend + 1].Drug_type)))
                {
                    drugType_drug_dict.Add(drug_legend_line.Drug_type, current_drugs.ToArray());
                }
            }
            return drugType_drug_dict;
        }
        public Dictionary<string, float> Get_drug_frequencyGroup_dict()
        {
            Dictionary<string, float> drug_cardiotoxicFrequency_dict = new Dictionary<string, float>();
            foreach (Deg_drug_legend_line_class drug_legend_line in this.Legend)
            {
                if (drug_legend_line.ReadWrite_cardiotoxicity_references.IndexOf("PMID:32231332") != -1)
                {
                    drug_cardiotoxicFrequency_dict.Add(drug_legend_line.Drug, drug_legend_line.Cardiotoxicity_frequencyGroup);
                }
            }
            return drug_cardiotoxicFrequency_dict;
        }
        public Dictionary<string, string> Get_drug_fullDrugName_dictionary()
        {
            Dictionary<string, string> drug_fullDrugName_dict = new Dictionary<string, string>();
            foreach (Deg_drug_legend_line_class legend_line in this.Legend)
            {
                drug_fullDrugName_dict.Add(legend_line.Drug, legend_line.Full_name);
            }
            return drug_fullDrugName_dict;
        }
        public Deg_drug_legend_class Deep_copy()
        {
            Deg_drug_legend_class copy = (Deg_drug_legend_class)this.MemberwiseClone();
            int legend_length = this.Legend.Length;
            for (int indexL = 0; indexL < legend_length; indexL++)
            {
                copy.Legend[indexL] = this.Legend[indexL].Deep_copy();
            }
            return copy;
        }
        public void Write(string directory, string fileName)
        {
            Deg_drug_legend_readWriteOptions_class readWriteOptions = new Deg_drug_legend_readWriteOptions_class(directory, fileName);
            ReadWriteClass.WriteData<Deg_drug_legend_line_class>(this.Legend, readWriteOptions);
        }
        private void Read()
        {
            Deg_drug_legend_input_readOptions_class readOptions = new Deg_drug_legend_input_readOptions_class();
            Legend = ReadWriteClass.ReadRawData_and_FillArray<Deg_drug_legend_line_class>(readOptions);
        }
    }

    ////////////////////////////////////////////////////////////////////

    class Deg_line_class : IFill_de
    {
        #region Fields
        public string Gene { get; set; }
        public string Symbol { get; set; }
        public float logFC { get; set; }
        public double PValue { get; set; }
        public double Signed_minus_log10_pvalue { get; set; }
        public float Fractional_rank { get; set; }
        public double FDR { get; set; }
        public string Patient { get; set; }
        public Timepoint_enum Timepoint { get; set; }
        public DE_entry_enum EntryType { get; set; }
        public string EntryType_description { get { return Lincs_website_conversion_class.Get_entryType_description(EntryType); } }
        public string Up_down_status_logFC { get { if (logFC < 0) { return "Down"; } else if (logFC > 0) { return "Up"; } else { return "No change"; } } }
        public string Up_down_status_signedMinusLog10 { get { if (Signed_minus_log10_pvalue < 0) { return "Down"; } else if (Signed_minus_log10_pvalue > 0) { return "Up"; } else { return "No change"; } } }
        public Drug_type_enum Drug_type { get; set; }
        public string Is_cardiotoxic { get; set; }
        public string Treatment { get; set; }
        public string Treatment_full_name { get; set; }
        public string Dataset { get; set; }
        public string Plate { get; set; }
        public static bool Check_ordering { get { return Global_class.Check_ordering; } }
        public int Treatment_wells_count { get; set; }
        public int Control_wells_count { get; set; }

        public Timepoint_enum Timepoint_for_de { get { return Timepoint; } }
        public string[] Names_for_de { get; set; }
        public string[] Symbols_for_de { get { return new string[] { Symbol }; } }
        public double Value_for_de { get; set; }
        public DE_entry_enum Entry_type_for_de { get { return EntryType; } }

        #region Variables for webesite
        public string DrugClass_for_website
        {
            get
            {
                return Lincs_website_conversion_class.Get_drugClass_for_website(Drug_type);
            }
        }

        public string Cell_line_type { get; set; }

        public string Cell_line_for_website
        {
            get
            {
                return Lincs_website_conversion_class.Get_cellLine_for_website(Cell_line_type, Patient, Plate);
            }

        }

        public string Plate_for_website
        {
            get
            {
                return Lincs_website_conversion_class.Get_plate_for_website(Plate);
            }

        }

        #endregion

        #endregion

        public Deg_line_class()
        {
            Timepoint = Timepoint_enum.E_m_p_t_y;
            this.Dataset = "";
            this.Treatment_full_name = "";
            this.Fractional_rank = -1;
            this.Cell_line_type = "";
            this.Is_cardiotoxic = "";
        }

        #region Order
        public static Deg_line_class[] Order_by_dataset_patient_drugType_treatment_plate_gene(Deg_line_class[] deg_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<Drug_type_enum, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>> dataset_patient_drugType_treatment_plate_gene_dict = new Dictionary<string, Dictionary<string, Dictionary<Drug_type_enum, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>>();
            Dictionary<string, Dictionary<Drug_type_enum, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>> patient_drugType_treatment_plate_gene_dict = new Dictionary<string, Dictionary<Drug_type_enum, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>();
            Dictionary<Drug_type_enum, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>> drugType_treatment_plate_gene_dict = new Dictionary<Drug_type_enum, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>> treatment_plate_gene_dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>();
            Dictionary<string, Dictionary<string, List<Deg_line_class>>> plate_gene_dict = new Dictionary<string, Dictionary<string, List<Deg_line_class>>>();
            Dictionary<string, List<Deg_line_class>> gene_dict = new Dictionary<string, List<Deg_line_class>>();

            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!dataset_patient_drugType_treatment_plate_gene_dict.ContainsKey(deg_line.Dataset))
                {
                    dataset_patient_drugType_treatment_plate_gene_dict.Add(deg_line.Dataset, new Dictionary<string, Dictionary<Drug_type_enum, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>());
                }
                if (!dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset].ContainsKey(deg_line.Patient))
                {
                    dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset].Add(deg_line.Patient, new Dictionary<Drug_type_enum, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>());
                }
                if (!dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient].ContainsKey(deg_line.Drug_type))
                {
                    dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient].Add(deg_line.Drug_type, new Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>());
                }
                if (!dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient][deg_line.Drug_type].ContainsKey(deg_line.Treatment))
                {
                    dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient][deg_line.Drug_type].Add(deg_line.Treatment, new Dictionary<string, Dictionary<string, List<Deg_line_class>>>());
                }
                if (!dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient][deg_line.Drug_type][deg_line.Treatment].ContainsKey(deg_line.Plate))
                {
                    dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient][deg_line.Drug_type][deg_line.Treatment].Add(deg_line.Plate, new Dictionary<string, List<Deg_line_class>>());
                }
                if (!dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient][deg_line.Drug_type][deg_line.Treatment][deg_line.Plate].ContainsKey(deg_line.Gene))
                {
                    dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient][deg_line.Drug_type][deg_line.Treatment][deg_line.Plate].Add(deg_line.Gene, new List<Deg_line_class>());
                }
                dataset_patient_drugType_treatment_plate_gene_dict[deg_line.Dataset][deg_line.Patient][deg_line.Drug_type][deg_line.Treatment][deg_line.Plate][deg_line.Gene].Add(deg_line);
            }

            string[] datasets = dataset_patient_drugType_treatment_plate_gene_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] patients;
            string patient;
            int patients_length;
            Drug_type_enum[] drug_types;
            Drug_type_enum drug_type;
            int drug_types_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            string[] plates;
            string plate;
            int plates_length;
            string[] genes;
            string gene;
            int genes_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexD = 0; indexD < datasets_length; indexD++)
            {
                dataset = datasets[indexD];
                patient_drugType_treatment_plate_gene_dict = dataset_patient_drugType_treatment_plate_gene_dict[dataset];
                patients = patient_drugType_treatment_plate_gene_dict.Keys.ToArray();
                patients_length = patients.Length;
                patients = patients.OrderBy(l => l).ToArray();
                for (int indexP = 0; indexP < patients_length; indexP++)
                {
                    patient = patients[indexP];
                    drugType_treatment_plate_gene_dict = patient_drugType_treatment_plate_gene_dict[patient];
                    drug_types = drugType_treatment_plate_gene_dict.Keys.ToArray();
                    drug_types_length = drug_types.Length;
                    drug_types = drug_types.OrderBy(l => l).ToArray();
                    for (int indexDT = 0; indexDT < drug_types_length; indexDT++)
                    {
                        drug_type = drug_types[indexDT];
                        treatment_plate_gene_dict = drugType_treatment_plate_gene_dict[drug_type];
                        treatments = treatment_plate_gene_dict.Keys.ToArray();
                        treatments_length = treatments.Length;
                        treatments = treatments.OrderBy(l => l).ToArray();
                        for (int indexT = 0; indexT < treatments_length; indexT++)
                        {
                            treatment = treatments[indexT];
                            plate_gene_dict = treatment_plate_gene_dict[treatment];
                            plates = plate_gene_dict.Keys.ToArray();
                            plates_length = plates.Length;
                            plates = plates.OrderBy(l => l).ToArray();
                            for (int indexPlate = 0; indexPlate < plates_length; indexPlate++)
                            {
                                plate = plates[indexPlate];
                                gene_dict = plate_gene_dict[plate];
                                genes = gene_dict.Keys.ToArray();
                                genes = genes.OrderBy(l => l).ToArray();
                                genes_length = genes.Length;
                                for (int indexGene = 0; indexGene < genes_length; indexGene++)
                                {
                                    gene = genes[indexGene];
                                    ordered_deg_lines.AddRange(gene_dict[gene]);
                                }
                            }
                        }
                    }
                }
            }

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = ordered_deg_lines.Count;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = ordered_deg_lines[indexOrdered - 1];
                    current_line = ordered_deg_lines[indexOrdered];
                    if (current_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Patient.CompareTo(previous_line.Patient) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Drug_type.CompareTo(previous_line.Drug_type) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Drug_type.Equals(previous_line.Drug_type))
                        && (current_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Drug_type.Equals(previous_line.Drug_type))
                        && (current_line.Treatment.Equals(previous_line.Treatment))
                        && (current_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Drug_type.Equals(previous_line.Drug_type))
                        && (current_line.Treatment.Equals(previous_line.Treatment))
                        && (current_line.Plate.Equals(previous_line.Plate))
                        && (current_line.Gene.CompareTo(previous_line.Gene) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_deg_lines.ToArray();
        }
        public static Deg_line_class[] Order_by_dataset_patient_treatment(Deg_line_class[] deg_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>> dataset_patient_treatment_dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>();
            Dictionary<string, Dictionary<string, List<Deg_line_class>>> patient_treatment_dict = new Dictionary<string, Dictionary<string, List<Deg_line_class>>>();
            Dictionary<string, List<Deg_line_class>> treatment_dict = new Dictionary<string, List<Deg_line_class>>();

            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!dataset_patient_treatment_dict.ContainsKey(deg_line.Dataset))
                {
                    dataset_patient_treatment_dict.Add(deg_line.Dataset, new Dictionary<string, Dictionary<string, List<Deg_line_class>>>());
                }
                if (!dataset_patient_treatment_dict[deg_line.Dataset].ContainsKey(deg_line.Patient))
                {
                    dataset_patient_treatment_dict[deg_line.Dataset].Add(deg_line.Patient, new Dictionary<string, List<Deg_line_class>>());
                }
                if (!dataset_patient_treatment_dict[deg_line.Dataset][deg_line.Patient].ContainsKey(deg_line.Treatment))
                {
                    dataset_patient_treatment_dict[deg_line.Dataset][deg_line.Patient].Add(deg_line.Treatment, new List<Deg_line_class>());
                }
                dataset_patient_treatment_dict[deg_line.Dataset][deg_line.Patient][deg_line.Treatment].Add(deg_line);
            }

            string[] datasets = dataset_patient_treatment_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] patients;
            string patient;
            int patients_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexD = 0; indexD < datasets_length; indexD++)
            {
                dataset = datasets[indexD];
                patient_treatment_dict = dataset_patient_treatment_dict[dataset];
                patients = patient_treatment_dict.Keys.ToArray();
                patients_length = patients.Length;
                patients = patients.OrderBy(l => l).ToArray();
                for (int indexP = 0; indexP < patients_length; indexP++)
                {
                    patient = patients[indexP];
                    treatment_dict = patient_treatment_dict[patient];
                    treatments = treatment_dict.Keys.ToArray();
                    treatments_length = treatments.Length;
                    treatments = treatments.OrderBy(l => l).ToArray();
                    for (int indexT = 0; indexT < treatments_length; indexT++)
                    {
                        treatment = treatments[indexT];
                        ordered_deg_lines.AddRange(treatment_dict[treatment]);
                    }
                }
            }

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = ordered_deg_lines.Count;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = ordered_deg_lines[indexOrdered - 1];
                    current_line = ordered_deg_lines[indexOrdered];
                    if (current_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Patient.CompareTo(previous_line.Patient) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_deg_lines.ToArray();
        }
        public static Deg_line_class[] Order_by_dataset_plate_cellline_treatment_symbol(Deg_line_class[] deg_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>> dataset_plate_cellline_treatment_symbol_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>> plate_cellline_treatment_symbol_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>> cellline_treatment_symbol_dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>();
            Dictionary<string, Dictionary<string, List<Deg_line_class>>> treatment_symbol_dict = new Dictionary<string, Dictionary<string, List<Deg_line_class>>>();
            Dictionary<string, List<Deg_line_class>> symbol_dict = new Dictionary<string, List<Deg_line_class>>();

            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!dataset_plate_cellline_treatment_symbol_dict.ContainsKey(deg_line.Dataset))
                {
                    dataset_plate_cellline_treatment_symbol_dict.Add(deg_line.Dataset, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>());
                }
                if (!dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset].ContainsKey(deg_line.Plate))
                {
                    dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset].Add(deg_line.Plate, new Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>());
                }
                if (!dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset][deg_line.Plate].ContainsKey(deg_line.Patient))
                {
                    dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset][deg_line.Plate].Add(deg_line.Patient, new Dictionary<string, Dictionary<string, List<Deg_line_class>>>());
                }
                if (!dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset][deg_line.Plate][deg_line.Patient].ContainsKey(deg_line.Treatment))
                {
                    dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset][deg_line.Plate][deg_line.Patient].Add(deg_line.Treatment, new Dictionary<string, List<Deg_line_class>>());
                }
                if (!dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset][deg_line.Plate][deg_line.Patient][deg_line.Treatment].ContainsKey(deg_line.Symbol))
                {
                    dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset][deg_line.Plate][deg_line.Patient][deg_line.Treatment].Add(deg_line.Symbol, new List<Deg_line_class>());
                }
                dataset_plate_cellline_treatment_symbol_dict[deg_line.Dataset][deg_line.Plate][deg_line.Patient][deg_line.Treatment][deg_line.Symbol].Add(deg_line);
            }

            string[] datasets = dataset_plate_cellline_treatment_symbol_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] plates;
            string plate;
            int plates_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            string[] symbols;
            string symbol;
            int symbols_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexD = 0; indexD < datasets_length; indexD++)
            {
                dataset = datasets[indexD];
                plate_cellline_treatment_symbol_dict = dataset_plate_cellline_treatment_symbol_dict[dataset];
                plates = plate_cellline_treatment_symbol_dict.Keys.ToArray();
                plates_length = plates.Length;
                plates = plates.OrderBy(l => l).ToArray();
                for (int indexP = 0; indexP < plates_length; indexP++)
                {
                    plate = plates[indexP];
                    cellline_treatment_symbol_dict = plate_cellline_treatment_symbol_dict[plate];
                    celllines = cellline_treatment_symbol_dict.Keys.ToArray();
                    celllines_length = celllines.Length;
                    celllines = celllines.OrderBy(l => l).ToArray();
                    for (int indexC = 0; indexC < celllines_length; indexC++)
                    {
                        cellline = celllines[indexC];
                        treatment_symbol_dict = cellline_treatment_symbol_dict[cellline];
                        treatments = treatment_symbol_dict.Keys.ToArray();
                        treatments_length = treatments.Length;
                        treatments = treatments.OrderBy(l => l).ToArray();
                        for (int indexT = 0; indexT < treatments_length; indexT++)
                        {
                            treatment = treatments[indexT];
                            symbol_dict = treatment_symbol_dict[treatment];
                            symbols = symbol_dict.Keys.ToArray();
                            symbols_length = symbols.Length;
                            symbols = symbols.OrderBy(l => l).ToArray();
                            for (int indexSymbol = 0; indexSymbol < symbols_length; indexSymbol++)
                            {
                                symbol = symbols[indexSymbol];
                                ordered_deg_lines.AddRange(symbol_dict[symbol]);
                            }
                        }
                    }
                }
            }

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = ordered_deg_lines.Count;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = ordered_deg_lines[indexOrdered - 1];
                    current_line = ordered_deg_lines[indexOrdered];
                    if (current_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.Equals(previous_line.Plate))
                        && (current_line.Patient.CompareTo(previous_line.Patient) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.Equals(previous_line.Plate))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.Equals(previous_line.Plate))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Treatment.Equals(previous_line.Treatment))
                        && (current_line.Symbol.CompareTo(previous_line.Symbol) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_deg_lines.ToArray();
        }
        public static Deg_line_class[] Order_by_dataset_treatment_patient_plate(Deg_line_class[] deg_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>> dataset_treatment_patient_plate_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>> treatment_patient_plate_dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>();
            Dictionary<string, Dictionary<string, List<Deg_line_class>>> patient_plate_dict = new Dictionary<string, Dictionary<string, List<Deg_line_class>>>();
            Dictionary<string, List<Deg_line_class>> plate_dict = new Dictionary<string, List<Deg_line_class>>();

            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!dataset_treatment_patient_plate_dict.ContainsKey(deg_line.Dataset))
                {
                    dataset_treatment_patient_plate_dict.Add(deg_line.Dataset, new Dictionary<string, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>());
                }
                if (!dataset_treatment_patient_plate_dict[deg_line.Dataset].ContainsKey(deg_line.Treatment))
                {
                    dataset_treatment_patient_plate_dict[deg_line.Dataset].Add(deg_line.Treatment, new Dictionary<string, Dictionary<string, List<Deg_line_class>>>());
                }
                if (!dataset_treatment_patient_plate_dict[deg_line.Dataset][deg_line.Treatment].ContainsKey(deg_line.Patient))
                {
                    dataset_treatment_patient_plate_dict[deg_line.Dataset][deg_line.Treatment].Add(deg_line.Patient, new Dictionary<string, List<Deg_line_class>>());
                }
                if (!dataset_treatment_patient_plate_dict[deg_line.Dataset][deg_line.Treatment][deg_line.Patient].ContainsKey(deg_line.Plate))
                {
                    dataset_treatment_patient_plate_dict[deg_line.Dataset][deg_line.Treatment][deg_line.Patient].Add(deg_line.Plate, new List<Deg_line_class>());
                }
                dataset_treatment_patient_plate_dict[deg_line.Dataset][deg_line.Treatment][deg_line.Patient][deg_line.Plate].Add(deg_line);
            }
            deg_lines = null;

            string[] datasets = dataset_treatment_patient_plate_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] treatments;
            string treatment;
            int treatments_length;
            string[] patients;
            string patient;
            int patients_length;
            string[] plates;
            string plate;
            int plates_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexD = 0; indexD < datasets_length; indexD++)
            {
                dataset = datasets[indexD];
                treatment_patient_plate_dict = dataset_treatment_patient_plate_dict[dataset];
                treatments = treatment_patient_plate_dict.Keys.ToArray();
                treatments_length = treatments.Length;
                treatments = treatments.OrderBy(l => l).ToArray();
                for (int indexT = 0; indexT < treatments_length; indexT++)
                {
                    treatment = treatments[indexT];
                    patient_plate_dict = treatment_patient_plate_dict[treatment];
                    patients = patient_plate_dict.Keys.ToArray();
                    patients_length = patients.Length;
                    patients = patients.OrderBy(l => l).ToArray();
                    for (int indexP = 0; indexP < patients_length; indexP++)
                    {
                        patient = patients[indexP];
                        plate_dict = patient_plate_dict[patient];
                        plates = plate_dict.Keys.ToArray();
                        plates = plates.OrderBy(l => l).ToArray();
                        plates_length = plates.Length;
                        for (int indexPL = 0; indexPL < plates_length; indexPL++)
                        {
                            plate = plates[indexPL];
                            ordered_deg_lines.AddRange(plate_dict[plate]);
                        }
                    }
                }
            }

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = ordered_deg_lines.Count;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = ordered_deg_lines[indexOrdered - 1];
                    current_line = ordered_deg_lines[indexOrdered];
                    if (current_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }

                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Treatment.Equals(previous_line.Treatment))
                        && (current_line.Patient.CompareTo(previous_line.Patient) < 0)) { throw new Exception(); }
                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Treatment.Equals(previous_line.Treatment))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }

                }
                #endregion
            }
            return ordered_deg_lines.ToArray();
        }
        public static Deg_line_class[] Order_by_group(Deg_line_class[] deg_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>> dataset_plate_timepoint_entryType_patient_treatment_dict = new Dictionary<string, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>>();
            Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>> plate_timepoint_entryType_patient_treatment_dict = new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>();
            Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>> timepoint_entryType_patient_treatment_dict = new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>();
            Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>> entryType_patient_treatment_dict = new Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>();
            Dictionary<string, Dictionary<string, List<Deg_line_class>>> patient_treatment_dict = new Dictionary<string, Dictionary<string, List<Deg_line_class>>>();
            Dictionary<string, List<Deg_line_class>> treatment_dict = new Dictionary<string, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!dataset_plate_timepoint_entryType_patient_treatment_dict.ContainsKey(deg_line.Dataset))
                {
                    dataset_plate_timepoint_entryType_patient_treatment_dict.Add(deg_line.Dataset, new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>>());
                }
                if (!dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset].ContainsKey(deg_line.Plate))
                {
                    dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset].Add(deg_line.Plate, new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>>());
                }
                if (!dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate].ContainsKey(deg_line.Timepoint))
                {
                    dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate].Add(deg_line.Timepoint, new Dictionary<DE_entry_enum, Dictionary<string, Dictionary<string, List<Deg_line_class>>>>());
                }
                if (!dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate][deg_line.Timepoint].ContainsKey(deg_line.EntryType))
                {
                    dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate][deg_line.Timepoint].Add(deg_line.EntryType, new Dictionary<string, Dictionary<string, List<Deg_line_class>>>());
                }
                if (!dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate][deg_line.Timepoint][deg_line.EntryType].ContainsKey(deg_line.Patient))
                {
                    dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate][deg_line.Timepoint][deg_line.EntryType].Add(deg_line.Patient, new Dictionary<string, List<Deg_line_class>>());
                }
                if (!dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate][deg_line.Timepoint][deg_line.EntryType][deg_line.Patient].ContainsKey(deg_line.Treatment))
                {
                    dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate][deg_line.Timepoint][deg_line.EntryType][deg_line.Patient].Add(deg_line.Treatment, new List<Deg_line_class>());
                }
                dataset_plate_timepoint_entryType_patient_treatment_dict[deg_line.Dataset][deg_line.Plate][deg_line.Timepoint][deg_line.EntryType][deg_line.Patient][deg_line.Treatment].Add(deg_line);
            }
            deg_lines = null;

            string[] datasets = dataset_plate_timepoint_entryType_patient_treatment_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] plates;
            string plate;
            int plates_length;
            Timepoint_enum[] timepoints;
            Timepoint_enum timepoint;
            int timepoints_length;
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;
            string[] patients;
            string patient;
            int patients_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexDataset = 0; indexDataset < datasets_length; indexDataset++)
            {
                dataset = datasets[indexDataset];
                plate_timepoint_entryType_patient_treatment_dict = dataset_plate_timepoint_entryType_patient_treatment_dict[dataset];
                plates = plate_timepoint_entryType_patient_treatment_dict.Keys.ToArray();
                plates_length = plates.Length;
                plates = plates.OrderBy(l => l).ToArray();
                for (int indexPlate = 0; indexPlate < plates_length; indexPlate++)
                {
                    plate = plates[indexPlate];
                    timepoint_entryType_patient_treatment_dict = plate_timepoint_entryType_patient_treatment_dict[plate];
                    timepoints = timepoint_entryType_patient_treatment_dict.Keys.ToArray();
                    timepoints_length = timepoints.Length;
                    timepoints = timepoints.OrderBy(l => l).ToArray();
                    for (int indexTimepoint = 0; indexTimepoint < timepoints_length; indexTimepoint++)
                    {
                        timepoint = timepoints[indexTimepoint];
                        entryType_patient_treatment_dict = timepoint_entryType_patient_treatment_dict[timepoint];
                        entryTypes = entryType_patient_treatment_dict.Keys.ToArray();
                        entryTypes_length = entryTypes.Length;
                        for (int indexEntryType = 0; indexEntryType < entryTypes_length; indexEntryType++)
                        {
                            entryType = entryTypes[indexEntryType];
                            patient_treatment_dict = entryType_patient_treatment_dict[entryType];
                            patients = patient_treatment_dict.Keys.ToArray();
                            patients = patients.OrderBy(l => l).ToArray();
                            patients_length = patients.Length;
                            for (int indexP = 0; indexP < patients_length; indexP++)
                            {
                                patient = patients[indexP];
                                treatment_dict = patient_treatment_dict[patient];
                                treatments = treatment_dict.Keys.ToArray();
                                treatments_length = treatments.Length;
                                treatments = treatments.OrderBy(l => l).ToArray();
                                for (int indexT = 0; indexT < treatments_length; indexT++)
                                {
                                    treatment = treatments[indexT];
                                    ordered_deg_lines.AddRange(treatment_dict[treatment]);
                                }
                            }
                        }
                    }
                }
            }

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = ordered_deg_lines.Count;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = ordered_deg_lines[indexOrdered - 1];
                    current_line = ordered_deg_lines[indexOrdered];
                    if (current_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }
                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.Equals(previous_line.Plate))
                        && (current_line.Timepoint.CompareTo(previous_line.Timepoint) < 0)) { throw new Exception(); }
                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.Equals(previous_line.Plate))
                        && (current_line.Timepoint.Equals(previous_line.Timepoint))
                        && (current_line.EntryType.CompareTo(previous_line.EntryType) < 0)) { throw new Exception(); }
                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.Equals(previous_line.Plate))
                        && (current_line.Timepoint.Equals(previous_line.Timepoint))
                        && (current_line.EntryType.Equals(previous_line.EntryType))
                        && (current_line.Patient.CompareTo(previous_line.Patient) < 0)) { throw new Exception(); }
                    if ((current_line.Dataset.Equals(previous_line.Dataset))
                        && (current_line.Plate.Equals(previous_line.Plate))
                        && (current_line.Timepoint.Equals(previous_line.Timepoint))
                        && (current_line.EntryType.Equals(previous_line.EntryType))
                        && (current_line.Patient.Equals(previous_line.Patient))
                        && (current_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }
                }
                #endregion
            }

            return ordered_deg_lines.ToArray();
        }
        public static Deg_line_class[] Order_by_group_and_pvalue(Deg_line_class[] deg_lines)
        {
            Dictionary<double, List<Deg_line_class>> pvalue_dict = new Dictionary<double, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!pvalue_dict.ContainsKey(deg_line.PValue))
                {
                    pvalue_dict.Add(deg_line.PValue, new List<Deg_line_class>());
                }
                pvalue_dict[deg_line.PValue].Add(deg_line);
            }

            double[] pvalues = pvalue_dict.Keys.ToArray();
            double pvalue;
            int pvalues_length = pvalues.Length;
            pvalues = pvalues.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexP = 0; indexP < pvalues_length; indexP++)
            {
                pvalue = pvalues[indexP];
                ordered_deg_lines.AddRange(pvalue_dict[pvalue]);
            }

            Deg_line_class[] final_deg_lines = Order_by_group(ordered_deg_lines.ToArray());


            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Equal_group(previous_line))
                        && (current_line.PValue.CompareTo(previous_line.PValue) < 0)) { throw new Exception(); }
                }
                #endregion
            }

            //degs = degs.OrderBy(l => l.Dataset).ThenBy(l => l.Plate).ThenBy(l => l.Timepoint).ThenBy(l => l.EntryType).ThenBy(l => l.Patient).ThenBy(l => l.Treatment).ThenBy(l => l.PValue).ToArray();
            return final_deg_lines;
        }
        public static Deg_line_class[] Order_by_group_pvalue_and_descendingAbsLog2foldchange(Deg_line_class[] deg_lines)
        {
            Dictionary<double, Dictionary<double, List<Deg_line_class>>> pvalue_absLog2FC_dict = new Dictionary<double, Dictionary<double, List<Deg_line_class>>>();
            Dictionary<double, List<Deg_line_class>> absLog2FC_dict = new Dictionary<double, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            double abs_logFC;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                abs_logFC = Math.Abs(deg_line.logFC);
                if (!pvalue_absLog2FC_dict.ContainsKey(deg_line.PValue))
                {
                    pvalue_absLog2FC_dict.Add(deg_line.PValue, new Dictionary<double, List<Deg_line_class>>());
                }
                if (!pvalue_absLog2FC_dict[deg_line.PValue].ContainsKey(abs_logFC))
                {
                    pvalue_absLog2FC_dict[deg_line.PValue].Add(abs_logFC, new List<Deg_line_class>());
                }
                pvalue_absLog2FC_dict[deg_line.PValue][abs_logFC].Add(deg_line);
            }

            double[] pvalues = pvalue_absLog2FC_dict.Keys.ToArray();
            double pvalue;
            int pvalues_length = pvalues.Length;
            double[] abs_logFCs;
            int abs_logFCs_length;
            pvalues = pvalues.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexP = 0; indexP < pvalues_length; indexP++)
            {
                pvalue = pvalues[indexP];
                absLog2FC_dict = pvalue_absLog2FC_dict[pvalue];
                abs_logFCs = absLog2FC_dict.Keys.ToArray();
                abs_logFCs = abs_logFCs.OrderByDescending(l => l).ToArray();
                abs_logFCs_length = abs_logFCs.Length;
                for (int indexAbs = 0; indexAbs < abs_logFCs_length; indexAbs++)
                {
                    abs_logFC = abs_logFCs[indexAbs];
                    ordered_deg_lines.AddRange(absLog2FC_dict[abs_logFC]);
                }
            }

            Deg_line_class[] final_deg_lines = Order_by_group(ordered_deg_lines.ToArray());

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Equal_group(previous_line))
                        && (current_line.PValue.CompareTo(previous_line.PValue) < 0)) { throw new Exception(); }
                    if ((current_line.Equal_group(previous_line))
                        && (current_line.PValue.Equals(previous_line.PValue))
                        && (Math.Abs(current_line.logFC).CompareTo(Math.Abs(previous_line.logFC)) > 0)) { throw new Exception(); } //descending
                }
                #endregion
            }

            //degs = degs.OrderBy(l => l.Dataset).ThenBy(l => l.Plate).ThenBy(l => l.Timepoint).ThenBy(l => l.EntryType).ThenBy(l => l.Patient).ThenBy(l => l.Treatment).ThenBy(l => l.PValue).ToArray();
            return final_deg_lines;
        }
        public static Deg_line_class[] Order_by_group_and_descendingAbsSignedMinusLog10Pvalue_and_descendingAbsLog2foldchange(Deg_line_class[] deg_lines)
        {
            Dictionary<double, Dictionary<double, List<Deg_line_class>>> absSignedMinusLog10Pvalue_absLog2FC_dict = new Dictionary<double, Dictionary<double, List<Deg_line_class>>>();
            Dictionary<double, List<Deg_line_class>> absLog2FC_dict = new Dictionary<double, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            double abs_logFC;
            double abs_signedMinusLog10Pvalue;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                abs_signedMinusLog10Pvalue = Math.Abs(deg_line.Signed_minus_log10_pvalue);
                abs_logFC = Math.Abs(deg_line.logFC);
                if (!absSignedMinusLog10Pvalue_absLog2FC_dict.ContainsKey(abs_signedMinusLog10Pvalue))
                {
                    absSignedMinusLog10Pvalue_absLog2FC_dict.Add(abs_signedMinusLog10Pvalue, new Dictionary<double, List<Deg_line_class>>());
                }
                if (!absSignedMinusLog10Pvalue_absLog2FC_dict[abs_signedMinusLog10Pvalue].ContainsKey(abs_logFC))
                {
                    absSignedMinusLog10Pvalue_absLog2FC_dict[abs_signedMinusLog10Pvalue].Add(abs_logFC, new List<Deg_line_class>());
                }
                absSignedMinusLog10Pvalue_absLog2FC_dict[abs_signedMinusLog10Pvalue][abs_logFC].Add(deg_line);
            }
            deg_lines = new Deg_line_class[0];

            double[] absSignedMinusLog10Pvalues = absSignedMinusLog10Pvalue_absLog2FC_dict.Keys.ToArray();
            double absSignedMinusLog10Pvalue;
            int absSignedMinusLog10Pvalues_length = absSignedMinusLog10Pvalues.Length;
            double[] abs_logFCs;
            int abs_logFCs_length;
            absSignedMinusLog10Pvalues = absSignedMinusLog10Pvalues.OrderByDescending(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexP = 0; indexP < absSignedMinusLog10Pvalues_length; indexP++)
            {
                absSignedMinusLog10Pvalue = absSignedMinusLog10Pvalues[indexP];
                absLog2FC_dict = absSignedMinusLog10Pvalue_absLog2FC_dict[absSignedMinusLog10Pvalue];
                abs_logFCs = absLog2FC_dict.Keys.ToArray();
                abs_logFCs = abs_logFCs.OrderByDescending(l => l).ToArray();
                abs_logFCs_length = abs_logFCs.Length;
                for (int indexAbs = 0; indexAbs < abs_logFCs_length; indexAbs++)
                {
                    abs_logFC = abs_logFCs[indexAbs];
                    ordered_deg_lines.AddRange(absLog2FC_dict[abs_logFC]);
                }
            }

            Deg_line_class[] final_deg_lines = Order_by_group(ordered_deg_lines.ToArray());

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Equal_group(previous_line))
                        && (Math.Abs(current_line.Signed_minus_log10_pvalue).CompareTo(Math.Abs(previous_line.Signed_minus_log10_pvalue)) > 0)) { throw new Exception(); }
                    if ((current_line.Equal_group(previous_line))
                        && (Math.Abs(current_line.Signed_minus_log10_pvalue).Equals(Math.Abs(previous_line.Signed_minus_log10_pvalue)))
                        && (Math.Abs(current_line.logFC).CompareTo(Math.Abs(previous_line.logFC)) > 0)) { throw new Exception(); } //descending
                }
                #endregion
            }
            return final_deg_lines;
        }
        public static Deg_line_class[] Order_by_group_and_fdr(Deg_line_class[] deg_lines)
        {
            Dictionary<double, List<Deg_line_class>> fdr_dict = new Dictionary<double, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!fdr_dict.ContainsKey(deg_line.FDR))
                {
                    fdr_dict.Add(deg_line.FDR, new List<Deg_line_class>());
                }
                fdr_dict[deg_line.FDR].Add(deg_line);
            }
            deg_lines = null;

            double[] fdrs = fdr_dict.Keys.ToArray();
            double fdr;
            int fdrs_length = fdrs.Length;
            fdrs = fdrs.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexP = 0; indexP < fdrs_length; indexP++)
            {
                fdr = fdrs[indexP];
                ordered_deg_lines.AddRange(fdr_dict[fdr]);
            }
            Deg_line_class[] final_deg_lines = Order_by_group(ordered_deg_lines.ToArray());

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Equal_group(previous_line))
                        && (current_line.FDR.CompareTo(previous_line.FDR) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return final_deg_lines;
        }
        public static Deg_line_class[] Order_by_group_and_descending_absolute_log2fc(Deg_line_class[] deg_lines)
        {
            Dictionary<double, List<Deg_line_class>> absLog2fc_dict = new Dictionary<double, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            double abs_log2fc;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                abs_log2fc = Math.Abs(deg_line.logFC);
                if (!absLog2fc_dict.ContainsKey(abs_log2fc))
                {
                    absLog2fc_dict.Add(abs_log2fc, new List<Deg_line_class>());
                }
                absLog2fc_dict[abs_log2fc].Add(deg_line);
            }
            deg_lines = null;
            double[] absLog2fcs = absLog2fc_dict.Keys.ToArray();
            double absLog2fc;
            int absLog2fcs_length = absLog2fcs.Length;
            absLog2fcs = absLog2fcs.OrderByDescending(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexP = 0; indexP < absLog2fcs_length; indexP++)
            {
                absLog2fc = absLog2fcs[indexP];
                ordered_deg_lines.AddRange(absLog2fc_dict[absLog2fc]);
            }

            Deg_line_class[] final_deg_lines = Order_by_group(ordered_deg_lines.ToArray());

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Equal_group(previous_line))
                        && (Math.Abs(current_line.logFC).CompareTo(Math.Abs(previous_line.logFC)) > 0)) { throw new Exception(); }//order by descending
                }
                #endregion
            }

            return final_deg_lines;
        }
        public static Deg_line_class[] Order_by_treatment_libraryPreparationMethod_symbol(Deg_line_class[] deg_lines)
        {
            Dictionary<string, Dictionary<string, List<Deg_line_class>>> treatment_symbol_dict = new Dictionary<string, Dictionary<string, List<Deg_line_class>>>();
            Dictionary<string, List<Deg_line_class>> symbol_dict = new Dictionary<string, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!treatment_symbol_dict.ContainsKey(deg_line.Treatment))
                {
                    treatment_symbol_dict.Add(deg_line.Treatment, new Dictionary<string, List<Deg_line_class>>());
                }
                if (!treatment_symbol_dict[deg_line.Treatment].ContainsKey(deg_line.Symbol))
                {
                    treatment_symbol_dict[deg_line.Treatment].Add(deg_line.Symbol, new List<Deg_line_class>());
                }
                treatment_symbol_dict[deg_line.Treatment][deg_line.Symbol].Add(deg_line);
            }
            deg_lines = null;

            string[] treatments = treatment_symbol_dict.Keys.ToArray();
            string treatment;
            int treatments_length = treatments.Length;
            string[] symbols;
            string symbol;
            int symbols_length;
            treatments = treatments.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexTreatment = 0; indexTreatment < treatments_length; indexTreatment++)
            {
                treatment = treatments[indexTreatment];
                symbol_dict = treatment_symbol_dict[treatment];
                symbols = symbol_dict.Keys.ToArray();
                symbols_length = symbols.Length;
                symbols = symbols.OrderBy(l => l).ToArray();
                for (int indexSymbol = 0; indexSymbol < symbols_length; indexSymbol++)
                {
                    symbol = symbols[indexSymbol];
                    ordered_deg_lines.AddRange(symbol_dict[symbol]);
                }
            }

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = ordered_deg_lines.Count;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = ordered_deg_lines[indexOrdered - 1];
                    current_line = ordered_deg_lines[indexOrdered];
                    if (current_line.Treatment.CompareTo(previous_line.Treatment) < 0) { throw new Exception(); }
                    if ((current_line.Treatment.Equals(previous_line.Treatment))
                        && (current_line.Symbol.CompareTo(previous_line.Symbol) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_deg_lines.ToArray();
        }
        public static Deg_line_class[] Order_by_treatment(Deg_line_class[] deg_lines)
        {
            Dictionary<string, List<Deg_line_class>> treatment_dict = new Dictionary<string, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!treatment_dict.ContainsKey(deg_line.Treatment))
                {
                    treatment_dict.Add(deg_line.Treatment, new List<Deg_line_class>());
                }
                treatment_dict[deg_line.Treatment].Add(deg_line);
            }

            string[] treatments = treatment_dict.Keys.ToArray();
            string treatment;
            int treatments_length = treatments.Length;
            treatments = treatments.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexP = 0; indexP < treatments_length; indexP++)
            {
                treatment = treatments[indexP];
                ordered_deg_lines.AddRange(treatment_dict[treatment]);
            }

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = ordered_deg_lines.Count;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = ordered_deg_lines[indexOrdered - 1];
                    current_line = ordered_deg_lines[indexOrdered];
                    if (current_line.Treatment.CompareTo(previous_line.Treatment) < 0) { throw new Exception(); }//order by descending
                }
                #endregion
            }
            return ordered_deg_lines.ToArray();
        }
        public static Deg_line_class[] Order_by_group_and_gene(Deg_line_class[] deg_lines)
        {
            int deg_lines_length = deg_lines.Length;
            deg_lines = Order_by_gene(deg_lines);
            Deg_line_class[] final_deg_lines = Order_by_group(deg_lines);

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Equal_group(previous_line))
                        && (current_line.Gene.CompareTo(previous_line.Gene) < 0)) { throw new Exception(); }
                }
                #endregion
            }

            //degs = degs.OrderBy(l => l.Dataset).ThenBy(l => l.Plate).ThenBy(l => l.Timepoint).ThenBy(l => l.EntryType).ThenBy(l => l.Patient).ThenBy(l => l.Treatment).ThenBy(l => l.Gene).ToArray();
            return final_deg_lines;
        }
        public static Deg_line_class[] Order_by_gene(Deg_line_class[] deg_lines)
        {
            Dictionary<string, List<Deg_line_class>> gene_dict = new Dictionary<string, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!gene_dict.ContainsKey(deg_line.Gene))
                {
                    gene_dict.Add(deg_line.Gene, new List<Deg_line_class>());
                }
                gene_dict[deg_line.Gene].Add(deg_line);
            }

            string[] genes = gene_dict.Keys.ToArray();
            string gene;
            int genes_length = genes.Length;
            genes = genes.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexP = 0; indexP < genes_length; indexP++)
            {
                gene = genes[indexP];
                ordered_deg_lines.AddRange(gene_dict[gene]);
            }

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = ordered_deg_lines.Count;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = ordered_deg_lines[indexOrdered - 1];
                    current_line = ordered_deg_lines[indexOrdered];
                    if (current_line.Gene.CompareTo(previous_line.Gene) < 0) { throw new Exception(); }
                }
                #endregion
            }

            //degs = degs.OrderBy(l => l.Dataset).ThenBy(l => l.Plate).ThenBy(l => l.Timepoint).ThenBy(l => l.EntryType).ThenBy(l => l.Patient).ThenBy(l => l.Treatment).ThenBy(l => l.Gene).ToArray();
            return ordered_deg_lines.ToArray();
        }
        public static Deg_line_class[] Order_by_symbol(Deg_line_class[] deg_lines)
        {
            Dictionary<string, List<Deg_line_class>> symbol_dict = new Dictionary<string, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!symbol_dict.ContainsKey(deg_line.Symbol))
                {
                    symbol_dict.Add(deg_line.Symbol, new List<Deg_line_class>());
                }
                symbol_dict[deg_line.Symbol].Add(deg_line);
            }

            string[] symbols = symbol_dict.Keys.ToArray();
            string symbol;
            int symbols_length = symbols.Length;
            symbols = symbols.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexP = 0; indexP < symbols_length; indexP++)
            {
                symbol = symbols[indexP];
                ordered_deg_lines.AddRange(symbol_dict[symbol]);
            }

            Deg_line_class[] final_deg_lines = ordered_deg_lines.ToArray();

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Symbol.CompareTo(previous_line.Symbol) < 0)) { throw new Exception(); }
                }
                #endregion
            }

            return final_deg_lines;
        }
        public static Deg_line_class[] Order_by_group_and_symbol(Deg_line_class[] deg_lines)
        {
            int deg_lines_length = deg_lines.Length;
            deg_lines = Order_by_symbol(deg_lines);
            Deg_line_class[] final_deg_lines = Order_by_group(deg_lines);

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Equal_group(previous_line))
                        && (current_line.Symbol.CompareTo(previous_line.Symbol) < 0)) { throw new Exception(); }
                }
                #endregion
            }

            return final_deg_lines;
        }
        public static Deg_line_class[] Order_by_group_symbol_pvalue(Deg_line_class[] deg_lines)
        {
            Dictionary<string, Dictionary<double, List<Deg_line_class>>> symbol_pvalue_dict = new Dictionary<string, Dictionary<double, List<Deg_line_class>>>();
            Dictionary<double, List<Deg_line_class>> pvalue_dict = new Dictionary<double, List<Deg_line_class>>();
            int deg_lines_length = deg_lines.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = deg_lines[indexDeg];
                if (!symbol_pvalue_dict.ContainsKey(deg_line.Symbol))
                {
                    symbol_pvalue_dict.Add(deg_line.Symbol, new Dictionary<double, List<Deg_line_class>>());
                }
                if (!symbol_pvalue_dict[deg_line.Symbol].ContainsKey(deg_line.PValue))
                {
                    symbol_pvalue_dict[deg_line.Symbol].Add(deg_line.PValue, new List<Deg_line_class>());
                }
                symbol_pvalue_dict[deg_line.Symbol][deg_line.PValue].Add(deg_line);
            }

            string[] symbols = symbol_pvalue_dict.Keys.ToArray();
            string symbol;
            int symbols_length = symbols.Length;
            double[] pvalues;
            double pvalue;
            int pvalues_length;
            symbols = symbols.OrderBy(l => l).ToArray();
            List<Deg_line_class> ordered_deg_lines = new List<Deg_line_class>();
            for (int indexS = 0; indexS < symbols_length; indexS++)
            {
                symbol = symbols[indexS];
                pvalue_dict = symbol_pvalue_dict[symbol];
                pvalues = pvalue_dict.Keys.ToArray();
                pvalues_length = pvalues.Length;
                pvalues = pvalues.OrderBy(l => l).ToArray();
                for (int indexP = 0; indexP < pvalues_length; indexP++)
                {
                    pvalue = pvalues[indexP];
                    ordered_deg_lines.AddRange(pvalue_dict[pvalue]);
                }
            }

            Deg_line_class[] final_deg_lines = Order_by_group(ordered_deg_lines.ToArray());

            if (Check_ordering)
            {
                #region Check if ordered correctly
                Deg_line_class previous_line;
                Deg_line_class current_line;
                int ordered_length = final_deg_lines.Length;
                if (ordered_length != deg_lines_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_length; indexOrdered++)
                {
                    previous_line = final_deg_lines[indexOrdered - 1];
                    current_line = final_deg_lines[indexOrdered];
                    if ((current_line.Equal_group(previous_line))
                        && (current_line.Symbol.CompareTo(previous_line.Symbol) < 0)) { throw new Exception(); }
                    if ((current_line.Equal_group(previous_line))
                        && (current_line.Symbol.Equals(previous_line.Symbol))
                        && (current_line.PValue.CompareTo(previous_line.PValue) < 0)) { throw new Exception(); }
                }
                #endregion
            }

            return final_deg_lines;
        }
        #endregion
        public bool Equal_group(Deg_line_class other)
        {
            bool equal = ((this.Dataset.Equals(other.Dataset))
                          && (this.Plate.Equals(other.Plate))
                          && (this.Timepoint.Equals(other.Timepoint))
                          && (this.EntryType.Equals(other.EntryType))
                          && (this.Patient.Equals(other.Patient))
                          && (this.Treatment.Equals(other.Treatment)));
            return equal;
        }

        public Deg_line_class Deep_copy()
        {
            Deg_line_class copy = (Deg_line_class)this.MemberwiseClone();
            copy.Gene = (string)this.Gene.Clone();
            copy.Treatment = (string)this.Treatment.Clone();
            copy.Dataset = (string)this.Dataset.Clone();
            copy.Patient = (string)this.Patient.Clone();
            copy.Treatment_full_name = (string)this.Treatment_full_name.Clone();
            copy.Plate = (string)this.Plate.Clone();
            copy.Is_cardiotoxic = (string)this.Is_cardiotoxic.Clone();
            return copy;
        }
    }

    class Deg_input_readOptions_class : ReadWriteOptions_base
    {
        public Deg_input_readOptions_class(string complete_file_name)
        {
            File = complete_file_name;
            Key_propertyNames = new string[] { "Gene", "logFC", "logCPM", "PValue", "FDR" };
            Key_columnNames = Key_propertyNames;
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            File_has_headline = true;
            Report = ReadWrite_report_enum.Report_main;
        }
    }
    class Deg_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter = ',';

        public Deg_readWriteOptions_class(string directory, string file_name)
        {
            File = directory + file_name;
            Key_propertyNames = new string[] { "Patient", "Timepoint", "EntryType", "Treatment", "Symbol", "Gene", "logFC", "PValue", "Signed_minus_log10_pvalue", "FDR", "Fractional_rank", "Plate", "Dataset", "Control_wells_count", "Treatment_wells_count" };
            Key_columnNames = Key_propertyNames;
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            File_has_headline = true;
            Report = ReadWrite_report_enum.Report_main;
        }
    }
    class Deg_website_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter = ',';

        public Deg_website_readWriteOptions_class(string subdirectory, string file_name, string cell_type)
        {
            File = Global_directory_class.Results_directory + subdirectory + file_name;
            Key_propertyNames = new string[] { "Treatment_full_name",  //1
                                                "DrugClass_for_website", //0
                                                "Is_cardiotoxic",
                                                "Cell_line_for_website", //2
                                                "Plate_for_website", //4
                                                "Symbol", //6
                                                "logFC", //7
                                                "Up_down_status_signedMinusLog10",//9
                                                "Signed_minus_log10_pvalue", //10
                                                "Fractional_rank"//11
                                                };
            Key_columnNames = new string[] {
                                                Lincs_website_conversion_class.Label_drugName,  //1
                                                Lincs_website_conversion_class.Label_drug_class,//0
                                                Lincs_website_conversion_class.Label_is_cardiotoxic,
                                                cell_type, //2
                                                Lincs_website_conversion_class.Label_plate,//4
                                                Lincs_website_conversion_class.Label_gene_symbol, //6
                                                Lincs_website_conversion_class.Label_log2fc, //7
                                                Lincs_website_conversion_class.Label_up_down_status, //9
                                                Lincs_website_conversion_class.Label_signed_minus_log10pvalue,//10
                                                Lincs_website_conversion_class.Label_fractional_rank//11
                                            };
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            File_has_headline = true;
            Report = ReadWrite_report_enum.Report_main;
        }
    }
    class Deg_preprocessed_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter = ',';

        public Deg_preprocessed_readWriteOptions_class(string directory, string file_name)
        {
            File = directory + file_name;
        }

        public Deg_preprocessed_readWriteOptions_class(string file_name) : this(Global_directory_class.Final_lincs_degs_directory, file_name)
        {
        }

        public string Get_binary_fileName()
        {
            return Path.GetDirectoryName(File) + "//" + Path.GetFileNameWithoutExtension(File) + ".bin";
        }

        public string Get_binary_bgGenes_fileName()
        {
            string complete_bgGenes_fileName = System.IO.Path.GetDirectoryName(File) + "/"
                                                + System.IO.Path.GetFileNameWithoutExtension(File)
                                                + "_bgGenes.bin";
            return complete_bgGenes_fileName;
        }

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Deg_abundancies_documentation_line_class
    {
        public string Dataset { get; set; }
        public string GeneId { get; set; }
        public string Symbol { get; set; }
        public double Rank_by_pvalue_mean { get; set; }
        public double Rank_by_pvalue_sd { get; set; }
        public double MinusLog10Pvalue_mean { get; set; }
        public double MinusLog10Pvalue_sd { get; set; }
        public double MinusLog10FDR_mean { get; set; }
        public double MinusLog10FDR_sd { get; set; }
        public double Log2FC_mean { get; set; }
        public double Log2FC_sd { get; set; }
        public double LogCPM_mean { get; set; }
        public double LogCPM_sd { get; set; }

        public Deg_abundancies_documentation_line_class Deep_copy()
        {
            Deg_abundancies_documentation_line_class copy = (Deg_abundancies_documentation_line_class)this.MemberwiseClone();
            copy.Dataset = (string)this.Dataset.Clone();
            copy.GeneId = (string)this.GeneId.Clone();
            copy.Symbol = (string)this.Symbol.Clone();
            return copy;
        }
    }
    class Combine_individual_files_fdr_documentation_line_class
    {
        public string Treatment { get; set; }
        public string Patient { get; set; }
        public string Plate { get; set; }
        public int DEGs_below_FDR01 { get; set; }
    }
    class Combine_individual_files_fdr_documentation_readWriteOptions_class : ReadWriteOptions_base
    {
        public Combine_individual_files_fdr_documentation_readWriteOptions_class(string fileName)
        {
            this.File = Global_directory_class.Results_directory + fileName;
            this.Key_propertyNames = new string[] { "Plate", "Treatment", "Patient", "DEGs_below_FDR01" };
            this.Key_columnNames = this.Key_propertyNames;
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
        }
    }
    class Combine_individual_files_to_single_degs_class : IDisposable
    {
        public Lincs_combine_files_options_class Options { get; set; }
        public Combine_individual_files_column_assignment_documentation_line_class[] Column_assignment_documentations { get; set; }
        public Lincs_extract_information_from_edgeR_files_class Extract_information { get; set; }

        Deg_line_class[] Inputs { get; set; }

        public Combine_individual_files_to_single_degs_class(string dataset)
        {
            Options = new Lincs_combine_files_options_class(dataset);
            this.Extract_information = new Lincs_extract_information_from_edgeR_files_class(Options.Dataset);
            this.Column_assignment_documentations = new Combine_individual_files_column_assignment_documentation_line_class[0];
        }
        private void Add_to_column_assignment_documentation_lines(Combine_individual_files_column_assignment_documentation_line_class[] add_column_assignment_documentations)
        {
            int add_length = add_column_assignment_documentations.Length;
            int this_length = this.Column_assignment_documentations.Length;
            int new_length = add_length + this_length;
            Combine_individual_files_column_assignment_documentation_line_class[] new_column_assignment_documentations = new Combine_individual_files_column_assignment_documentation_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_column_assignment_documentations[indexNew] = this.Column_assignment_documentations[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_column_assignment_documentations[indexNew] = add_column_assignment_documentations[indexAdd];
            }
            this.Column_assignment_documentations = new_column_assignment_documentations;
        }
        private string[] Get_all_file_names_that_are_individual_files_with_complete_degs()
        {
            string[] directories = Options.Directories;
            string directory;
            int directories_length = directories.Length;
            List<string> files_with_complete_degs = new List<string>();
            for (int indexD = 0; indexD < directories_length; indexD++)
            {
                directory = directories[indexD];
                string[] all_files = Directory.GetFiles(directory);
                string current_file;
                int all_files_length = all_files.Length;
                for (int indexA = 0; indexA < all_files_length; indexA++)
                {
                    current_file = all_files[indexA];
                    if (current_file.Contains(Options.Full_degs_marker))
                    {
                        files_with_complete_degs.Add(current_file);
                    }
                }
            }
            return files_with_complete_degs.ToArray();
        }
        private Deg_line_class[] Add_sample_specific_information(Deg_line_class[] input_lines, string complete_file_name)
        {
            string file_name = Path.GetFileNameWithoutExtension(complete_file_name);
            string[] splitStrings = file_name.Split('-');

            string patient;
            string plate;
            string treatment;
            DE_entry_enum entryType;
            Timepoint_enum timepoint;

            Extract_information.Get_patient_plate_treatment_entryType_timepoint(out patient, out plate, out treatment, out entryType, out timepoint, complete_file_name);

            int input_lines_length = input_lines.Length;
            Deg_line_class input_line;
            for (int indexInput = 0; indexInput < input_lines_length; indexInput++)
            {
                input_line = input_lines[indexInput];
                input_line.Timepoint = timepoint;
                input_line.Treatment = (string)treatment.Clone();
                input_line.EntryType = entryType;
                input_line.Patient = (string)patient.Clone();
                input_line.Dataset = (string)Options.Dataset.Clone();
                input_line.Plate = (string)plate.Clone();
            }
            return input_lines;
        }
        private string Get_well_name_form_columnName(string columnName)
        {
            switch (Options.Dataset)
            {
                case "DEGenes_iPSCdCMs_P0":
                case "DEGenes_iPSCdCMs_ECCoCulture":
                    return columnName;
                default:
                    throw new Exception();
            }
        }
        public void Dispose()
        {
            Options = null;
            Column_assignment_documentations = null;
            Extract_information = null;
        }
        private Deg_line_class[] ReadRawData_and_fill_array_local(string complete_file_name)
        {
            StreamReader reader = new StreamReader(complete_file_name);

            string cellline;
            string plate;
            string drug;
            DE_entry_enum entryType;
            Timepoint_enum timepoint;
            Extract_information.Get_patient_plate_treatment_entryType_timepoint(out cellline, out plate, out drug, out entryType, out timepoint, complete_file_name);

            int indexVs = drug.IndexOf("_vs");
            string drug_without_denomintator = (string)drug.Clone();
            if (indexVs != -1)
            {
                drug_without_denomintator = drug_without_denomintator.Substring(0, indexVs);
            }

            string headline = reader.ReadLine();
            string[] columnNames = headline.Split(Global_class.Tab);
            Lincs_column_indexes_class column_indexes = new Lincs_column_indexes_class();
            column_indexes.Set_all_column_indexes_from_headline(ref columnNames, Options.Dataset, complete_file_name);

            string inputLine;
            string[] columnEntries;
            string columnEntry;
            Deg_line_class deg_line;
            List<Deg_line_class> new_degs = new List<Deg_line_class>();
            StringBuilder sb = new StringBuilder();
            List<string> treatment_wells = new List<string>();
            List<float> treatment_counts = new List<float>();
            List<float> treatment_norm_counts = new List<float>();
            List<string> control_wells = new List<string>();
            List<float> control_counts = new List<float>();
            List<float> control_norm_counts = new List<float>();
            string columnName;
            while ((inputLine = reader.ReadLine()) != null)
            {
                columnEntries = inputLine.Split(Global_class.Tab);
                if (columnEntries.Length != column_indexes.Column_names_length) { throw new Exception(); }
                deg_line = new Deg_line_class();

                treatment_wells.Clear();
                treatment_counts.Clear();
                treatment_norm_counts.Clear();

                control_wells.Clear();
                control_counts.Clear();
                control_norm_counts.Clear();

                for (int indexC = 0; indexC < column_indexes.Column_names_length; indexC++)
                {
                    columnEntry = columnEntries[indexC];
                    if (indexC == column_indexes.Symbol_column_index)
                    {
                        deg_line.Gene = (string)columnEntry.Clone();
                    }
                    else if (indexC == column_indexes.LogCPM_column_index)
                    {
                    }
                    else if (indexC == column_indexes.LogFC_column_index)
                    {
                        deg_line.logFC = float.Parse(columnEntry);
                    }
                    else if (indexC == column_indexes.Pvalue_column_index)
                    {
                        deg_line.PValue = float.Parse(columnEntry);
                    }
                    else if (indexC == column_indexes.Fdr_column_index)
                    {
                        deg_line.FDR = float.Parse(columnEntry);
                    }
                    else if (column_indexes.Control_column_indexes.Contains(indexC))
                    {
                        control_counts.Add(float.Parse(columnEntry));
                        columnName = column_indexes.Column_names_in_correct_order[indexC];
                        control_wells.Add(Get_well_name_form_columnName(columnName));
                    }
                    else if (column_indexes.Control_norm_column_indexes.Contains(indexC))
                    {
                        control_norm_counts.Add(float.Parse(columnEntry));
                    }
                    else if (column_indexes.Treatment_norm_column_indexes.Contains(indexC))
                    {
                        treatment_norm_counts.Add(float.Parse(columnEntry));
                    }
                    else if (column_indexes.Treatment_column_indexes.Contains(indexC))
                    {
                        treatment_counts.Add(float.Parse(columnEntry));
                        columnName = column_indexes.Column_names_in_correct_order[indexC];
                        treatment_wells.Add(Get_well_name_form_columnName(columnName));
                    }
                    else if (indexC == column_indexes.Regulation_column_index)
                    {
                    }
                    else if (indexC == column_indexes.Likelihood_ratio_index)
                    {
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                deg_line.Plate = (string)plate.Clone();
                deg_line.Treatment_wells_count = treatment_wells.Count;
                deg_line.Control_wells_count = control_wells.Count;
                new_degs.Add(deg_line);
            }

            Add_to_column_assignment_documentation_lines(column_indexes.Column_assignment_documentations);

            return new_degs.ToArray();
        }
        private void Read_all_files_that_are_individual_files(string[] complete_file_names)
        {
            string complete_file_name;
            int files_length = complete_file_names.Length;
            List<Deg_line_class> input_list = new List<Deg_line_class>();
            Deg_line_class[] add_input_lines;
            for (int indexF = 0; indexF < files_length; indexF++)
            {
                complete_file_name = complete_file_names[indexF];
                Deg_input_readOptions_class readOptions = new Deg_input_readOptions_class(complete_file_name);
                add_input_lines = this.ReadRawData_and_fill_array_local(complete_file_name);
                add_input_lines = Add_sample_specific_information(add_input_lines, complete_file_name);
                input_list.AddRange(add_input_lines);
            }
            if (input_list.Count == 0) { throw new Exception(); }
            Inputs = input_list.ToArray();
        }
        public void Generate_combined_file_and_write()
        {
            this.Inputs = new Deg_line_class[0];
            string[] complete_file_names = Get_all_file_names_that_are_individual_files_with_complete_degs();
            int complete_fileNames_length = complete_file_names.Length;
            List<string> thisSet_complete_fileNames = new List<string>();
            bool add_sets = false;
            int setNo = -1;
            for (int indexC = 0; indexC < complete_fileNames_length; indexC++)
            {
                thisSet_complete_fileNames.Add(complete_file_names[indexC]);
                if ((thisSet_complete_fileNames.Count == Options.Max_files_to_be_combined)
                    || (indexC == complete_fileNames_length - 1))
                {
                    this.Inputs = new Deg_line_class[0];
                    Read_all_files_that_are_individual_files(thisSet_complete_fileNames.ToArray());
                    this.Inputs = Deg_line_class.Order_by_dataset_patient_treatment(this.Inputs);
                    if (indexC != complete_fileNames_length - 1)
                    { add_sets = true; }
                    setNo++;
                    if (add_sets) { Options.Add_or_update_set_in_combinedFileName(setNo); }
                    Write_combined_file();
                    thisSet_complete_fileNames.Clear();
                }
            }
        }
        private void Write_combined_file()
        {
            string directory = Options.Output_directory;
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            Deg_readWriteOptions_class readWriteOptions = new Deg_readWriteOptions_class(directory, Options.Combined_filename);
            ReadWriteClass.WriteData(Inputs, readWriteOptions);
        }
    }
    class Deg_options_class
    {
        public double Replace_minuslog10pvalue_by_value_if_inf { get; set; }
        public double Max_pvalue_every_greater_pvalue_will_be_set_to_before_calculation_of_minusLog10Pvalue { get; set; }
        public DEG_harmonization_between_samples_enum DEG_harmonization_between_samples { get; set; }
        public RefSeq_accepted_accessionNumberType_enum RefSeq_accepted_accessionNumberType { get; set; }
        public Add_ncbi_symbols_origin_enum Add_ncbi_symbols_origin { get; set; }

        public Deg_options_class()
        {
            DEG_harmonization_between_samples = DEG_harmonization_between_samples_enum.Add_missing_refSeq_genes;
            Add_ncbi_symbols_origin = Add_ncbi_symbols_origin_enum.Ncbi;
            RefSeq_accepted_accessionNumberType = RefSeq_accepted_accessionNumberType_enum.All_onlymRNA;

            Max_pvalue_every_greater_pvalue_will_be_set_to_before_calculation_of_minusLog10Pvalue = 0.99;
            Replace_minuslog10pvalue_by_value_if_inf = 64;
        }

        public string Get_name_of_preprocessed_degs(string original_fileName)
        {
            return original_fileName;
        }

        public void Print_options()
        {
            Report_class.WriteLine("{0}: Print options", typeof(Deg_options_class).Name);
            for (int i = 0; i < typeof(Deg_options_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("Replace_minuslog10pvalue_by_value_for_signedMinusLog10Pvalue_if_inf: {0}", Replace_minuslog10pvalue_by_value_if_inf);
            for (int i = 0; i < typeof(Deg_options_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("Max_pvalue_every_greater_pvalue_will_be_set_to_before_calculation_of_minusLog10Pvalue: {0}", Max_pvalue_every_greater_pvalue_will_be_set_to_before_calculation_of_minusLog10Pvalue);
            Report_class.WriteLine();
        }
    }
    class Deg_class : IDisposable
    {
        public Deg_line_class[] Degs_complete { get; set; }
        string[] Bg_genes { get; set; }
        public Deg_drug_legend_class Drug_legend { get; set; }
        public Deg_options_class Options { get; set; }
        public bool Averaged_data { get; private set; }

        public Deg_class()
        {
            Drug_legend = new Deg_drug_legend_class();
            Options = new Deg_options_class();
            this.Degs_complete = new Deg_line_class[0];
            this.Bg_genes = new string[0];
            Averaged_data = false;
        }

        #region Check
        public void Check_if_only_given_entrytype(DE_entry_enum entrytype)
        {
            foreach (Deg_line_class deg_line in this.Degs_complete)
            {
                if (!deg_line.EntryType.Equals(entrytype)) { throw new Exception(); }
            }
        }
        public void Check_if_equal_to_other_deg_instance_based_on_dataset_plate_cellline_treatment_geneSymbol_signedMinusLog10Pvalue(Deg_class other, bool compare_only_overlapping_genes_and_plates)
        {
            if (compare_only_overlapping_genes_and_plates)
            {
                string[] this_symbols = Get_all_symbols();
                string[] other_symbols = other.Get_all_symbols();
                this.Keep_only_lines_with_indicated_symbols(other_symbols);
                other.Keep_only_lines_with_indicated_symbols(this_symbols);
                string[] this_plates = Get_all_ordered_plates();
                string[] other_plates = other.Get_all_ordered_plates();
                this.Keep_only_indicated_plates(other_plates);
                other.Keep_only_indicated_plates(this_plates);
            }

            int this_length = this.Degs_complete.Length;
            int other_length = other.Degs_complete.Length;
            if (this_length != other_length) { throw new Exception(); }
            this.Degs_complete = Deg_line_class.Order_by_dataset_plate_cellline_treatment_symbol(this.Degs_complete);
            other.Degs_complete = Deg_line_class.Order_by_dataset_plate_cellline_treatment_symbol(other.Degs_complete);
            Deg_line_class this_line;
            Deg_line_class other_line;
            double round_start_factor = 1000000;
            double current_round_factor;
            bool values_equal = false;
            Dictionary<double, int> roundFactor_counts_dict = new Dictionary<double, int>();
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                this_line = this.Degs_complete[indexThis];
                other_line = other.Degs_complete[indexThis];
                if (!this_line.Dataset.Equals(other_line.Dataset)) { throw new Exception(); }
                if (!this_line.Plate.Equals(other_line.Plate)) { throw new Exception(); }
                if (!this_line.Patient.Equals(other_line.Patient)) { throw new Exception(); }
                if (!this_line.Treatment.Equals(other_line.Treatment)) { throw new Exception(); }
                if (!this_line.Symbol.Equals(other_line.Symbol)) { throw new Exception(); }
                values_equal = false;
                current_round_factor = round_start_factor;
                while (!values_equal)
                {
                    values_equal = Math.Round(this_line.Signed_minus_log10_pvalue * current_round_factor).Equals(Math.Round(other_line.Signed_minus_log10_pvalue * current_round_factor));
                    if (!values_equal) { current_round_factor = current_round_factor * 0.1; }
                    else
                    {
                        if (!roundFactor_counts_dict.ContainsKey(current_round_factor)) { roundFactor_counts_dict.Add(current_round_factor, 0); }
                        roundFactor_counts_dict[current_round_factor]++;
                    }
                }
            }
        }
        #endregion

        #region Order
        public void Order_by_group_fdr()
        {
            this.Degs_complete = Deg_line_class.Order_by_group_and_fdr(this.Degs_complete);
        }
        #endregion

        #region Generate
        private void Remove_empty_gene_names()
        {
            Report_class.Write("{0}: Remove empty gene names: ", typeof(Deg_class).Name);
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            List<Deg_line_class> kept_degs = new List<Deg_line_class>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                if ((!String.IsNullOrEmpty(deg_line.Gene))
                    && (!deg_line.Gene.Equals(Global_class.Empty_entry)))
                {
                    kept_degs.Add(deg_line);
                }
            }
            Report_class.WriteLine("{0} of {1} deg_lines kept", kept_degs.Count, degs_length);
            Degs_complete = kept_degs.ToArray();
        }
        private void Remove_empty_symbols()
        {
            Report_class.Write("{0}: Remove empty symbols names: ", typeof(Deg_class).Name);
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            List<Deg_line_class> kept_degs = new List<Deg_line_class>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                if ((!String.IsNullOrEmpty(deg_line.Symbol))
                    && (!deg_line.Symbol.Equals(Global_class.Empty_entry)))
                {
                    kept_degs.Add(deg_line);
                }
            }
            Report_class.WriteLine("{0} of {1} deg_lines kept", kept_degs.Count, degs_length);
            Degs_complete = kept_degs.ToArray();
        }
        private void Add_drug_type_full_name_and_is_cardiotoxic_info()
        {
            int degs_complete_length = Degs_complete.Length;
            this.Degs_complete = Deg_line_class.Order_by_treatment(this.Degs_complete);
            //Degs_complete = Degs_complete.OrderBy(l => l.Treatment).ToArray();
            Deg_line_class deg_line;
            string current_treatment;
            string previous_treatment = "error";
            Drug_legend.Order_by_treatment();
            int legend_length = Drug_legend.Legend.Length;
            int indexLegend = 0;
            int stringCompare = -2;
            Deg_drug_legend_line_class drug_legend_line;
            bool new_drug = false;
            for (int indexDegs = 0; indexDegs < degs_complete_length; indexDegs++)
            {
                deg_line = Degs_complete[indexDegs];
                current_treatment = deg_line.Treatment.Split('.')[0];
                if (deg_line.Treatment.IndexOf("randomization") != -1)
                {
                    deg_line.Drug_type = Drug_type_enum.Randomization;
                    deg_line.Treatment_full_name = (string)deg_line.Treatment.Clone();
                }
                else if (deg_line.Treatment.IndexOf("Averaged") != -1)
                {
                    deg_line.Drug_type = Drug_type_enum.Averaged_randomization;
                    deg_line.Treatment_full_name = (string)deg_line.Treatment.Clone();
                }
                else
                {
                    if ((indexDegs == 0) || (!current_treatment.Equals(previous_treatment)))
                    {
                        new_drug = true;
                    }
                    else
                    {
                        new_drug = false;
                    }
                    stringCompare = -2;
                    while ((indexLegend < legend_length) && (stringCompare < 0))
                    {
                        drug_legend_line = Drug_legend.Legend[indexLegend];
                        stringCompare = drug_legend_line.Drug.CompareTo(current_treatment);
                        if (stringCompare < 0)
                        {
                            indexLegend++;
                        }
                        else if (stringCompare == 0)
                        {
                            deg_line.Drug_type = drug_legend_line.Drug_type;
                            deg_line.Treatment_full_name = (string)drug_legend_line.Full_name.Clone();
                            deg_line.Is_cardiotoxic = (string)drug_legend_line.Is_cardiotoxic.Clone();
                        }
                    }
                    if ((new_drug) && (stringCompare != 0))
                    {
                        deg_line.Drug_type = Drug_type_enum.E_m_p_t_y;
                        deg_line.Treatment_full_name = (string)deg_line.Treatment.Clone();
                        throw new Exception();
                    }
                }
                previous_treatment = (string)current_treatment.Clone();
                if (String.IsNullOrEmpty(deg_line.Treatment_full_name)) { throw new Exception(); }
            }
        }
        private void Set_to_upper_case()
        {
            int degs_complete_length = Degs_complete.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_complete_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                deg_line.Symbol = deg_line.Symbol.ToUpper();
            }
        }
        public void Replace_human_by_cellline()
        {
            int degs_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if (deg_line.Patient.IndexOf("Human.") != -1)
                {
                    deg_line.Patient = deg_line.Patient.Replace("Human.", "Cell_line.");
                }
                else if ((deg_line.Patient.IndexOf("Cell_line.") != -1)
                         || (deg_line.Patient.IndexOf("Treatment.") != -1)
                         || (deg_line.Patient.IndexOf("Averaged") != -1))
                {
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        public void Replace_old_plate_by_new_plate(string old_plate, string new_plate)
        {
            int degs_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if (deg_line.Plate.Equals(old_plate))
                {
                    deg_line.Plate = (string)new_plate.Clone();
                }
            }
        }
        private void Set_entryType_to_diffrna()
        {
            int degs_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                deg_line.EntryType = DE_entry_enum.Diffrna;
            }
        }
        public void Check_for_deg_genes_that_are_not_part_of_ncbi_genes()
        {
            Report_class.Write("{0}: Check for deg genes that are not in ncbi reference:", typeof(Deg_class).Name);
            NcbiRefSeq_lincs_class ncbiRefSeq = new NcbiRefSeq_lincs_class();
            ncbiRefSeq.Options.RefSeq_accepted_accesionNumberType = this.Options.RefSeq_accepted_accessionNumberType;
            ncbiRefSeq.Generate_by_reading_safed_file();
            string[] geneIDs = ncbiRefSeq.Get_all_ordered_UCSCrefSeqGeneIDs();
            string geneID;
            int indexGeneID = 0;
            int geneIDs_length = geneIDs.Length;
            int stringCompare = -2;

            Degs_complete = Deg_line_class.Order_by_gene(Degs_complete);
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            bool gene_exists_in_ncbiRef = false;
            List<string> genes_not_in_ncbiRef = new List<string>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                if ((indexDeg == 0) || (!deg_line.Gene.Equals(Degs_complete[indexDeg - 1].Gene)))
                {
                    gene_exists_in_ncbiRef = false;
                    stringCompare = -2;
                    while ((indexGeneID < geneIDs_length) && (stringCompare < 0))
                    {
                        geneID = geneIDs[indexGeneID];
                        stringCompare = geneID.CompareTo(deg_line.Gene);
                        if (stringCompare < 0)
                        {
                            indexGeneID++;
                        }
                        else if (stringCompare == 0)
                        {
                            gene_exists_in_ncbiRef = true;
                        }
                    }
                    if (!gene_exists_in_ncbiRef)
                    {
                        genes_not_in_ncbiRef.Add(deg_line.Gene);
                    }
                }
            }
            if (genes_not_in_ncbiRef.Count > 0)
            {
            }
            Report_class.WriteLine(" {0} deg genes not in ncbi reference", genes_not_in_ncbiRef.Count);
        }
        public void Check_for_duplicates_with_same_patient_treatment_plate_symbols()
        {
            Report_class.Write("{0}: Check for duplicates", typeof(Deg_class).Name);
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, bool>>>> patient_treatment_plate_symbol_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, bool>>>>();

            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                if (!patient_treatment_plate_symbol_dict.ContainsKey(deg_line.Patient))
                {
                    patient_treatment_plate_symbol_dict.Add(deg_line.Patient, new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>());
                }
                if (!patient_treatment_plate_symbol_dict[deg_line.Patient].ContainsKey(deg_line.Treatment))
                {
                    patient_treatment_plate_symbol_dict[deg_line.Patient].Add(deg_line.Treatment, new Dictionary<string, Dictionary<string, bool>>());
                }
                if (!patient_treatment_plate_symbol_dict[deg_line.Patient][deg_line.Treatment].ContainsKey(deg_line.Plate))
                {
                    patient_treatment_plate_symbol_dict[deg_line.Patient][deg_line.Treatment].Add(deg_line.Plate, new Dictionary<string, bool>());
                }
                patient_treatment_plate_symbol_dict[deg_line.Patient][deg_line.Treatment][deg_line.Plate].Add(deg_line.Symbol, true);
            }
        }
        private void Add_missing_refSeq_genes_based_on_ncbi()
        {
            NcbiRefSeq_lincs_class ncbiRefSeq = new NcbiRefSeq_lincs_class();
            ncbiRefSeq.Options.RefSeq_accepted_accesionNumberType = this.Options.RefSeq_accepted_accessionNumberType;
            ncbiRefSeq.Generate_by_reading_safed_file();
            string[] geneIDs = ncbiRefSeq.Get_all_ordered_UCSCrefSeqGeneIDs();

            Add_missing_genes_to_all_lines(geneIDs);
        }
        public void Add_missing_genes_to_all_lines(string[] genes)
        {
            Report_class.Write("{0}: Add missing genes to all lines: ", typeof(Deg_class).Name);
            string[] all_genes = genes;
            string all_gene;
            all_genes = all_genes.OrderBy(l => l).ToArray();
            int all_genes_length = all_genes.Length;
            int indexAll = 0;
            int stringCompare = -2;

            this.Degs_complete = Deg_line_class.Order_by_group_and_gene(this.Degs_complete);
            int degs_length = this.Degs_complete.Length;

            Deg_line_class deg_line;
            Deg_line_class new_deg_line;
            List<Deg_line_class> new_deg_line_list = new List<Deg_line_class>();

            int new_lines_count = 0;
            int existing_lines_count = 0;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if ((indexDeg == 0) || (!deg_line.Equal_group(this.Degs_complete[indexDeg - 1])))
                {
                    indexAll = 0;
                    new_lines_count = 0;
                    existing_lines_count = 0;
                }
                existing_lines_count++;
                stringCompare = -2;
                while ((indexAll < all_genes_length) && (stringCompare <= 0))
                {
                    all_gene = all_genes[indexAll];
                    stringCompare = all_gene.ToUpper().CompareTo(deg_line.Gene.ToUpper());
                    if (stringCompare < 0)
                    {
                        new_deg_line = deg_line.Deep_copy();
                        new_deg_line.Symbol = "";
                        new_deg_line.Gene = (string)all_gene.Clone();
                        new_deg_line.logFC = 0;
                        new_deg_line.FDR = 1;
                        new_deg_line.PValue = 1;
                        new_deg_line.Fractional_rank = -1;
                        new_deg_line_list.Add(new_deg_line);
                        indexAll++;
                        new_lines_count++;
                    }
                    else if (stringCompare == 0)
                    {
                        indexAll++;
                    }
                }

                if ((indexDeg == degs_length - 1) || (!deg_line.Equal_group(this.Degs_complete[indexDeg + 1])))
                {
                    while (indexAll < all_genes_length)
                    {
                        all_gene = all_genes[indexAll];
                        new_deg_line = deg_line.Deep_copy();
                        new_deg_line.Symbol = "";
                        new_deg_line.Gene = (string)all_gene.Clone();
                        new_deg_line.logFC = 0;
                        new_deg_line.FDR = 1;
                        new_deg_line.PValue = 1;
                        new_deg_line.Fractional_rank = -1;
                        new_deg_line_list.Add(new_deg_line);
                        indexAll++;
                        new_lines_count++;
                    }
                }
                if ((indexDeg == degs_length - 1) || (!deg_line.Equal_group(this.Degs_complete[indexDeg + 1])))
                {
                    for (int i = 0; i < typeof(Deg_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                    Report_class.WriteLine("{0}-{1}-{2}: {3} lines added to {4} --> {5} lines", deg_line.Plate, deg_line.Patient, deg_line.Treatment, new_lines_count, existing_lines_count, existing_lines_count + new_lines_count);
                }
            }
            new_deg_line_list.AddRange(this.Degs_complete);
            this.Degs_complete = new_deg_line_list.ToArray();
            for (int i = 0; i < typeof(Deg_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("Summary: before addition: {0}; after addition: {1}", degs_length, this.Degs_complete.Length);
        }
        private void Add_ncbi_gene_symbols_based_on_ncbiRefSeqGeneIds()
        {
            Report_class.WriteLine("{0}: Add ncbi gene symbols based on ncbiRefSeqGeneIds", typeof(Deg_class).Name);
            NcbiRefSeq_lincs_class ncbiRefSeq = new NcbiRefSeq_lincs_class();
            ncbiRefSeq.Options.RefSeq_accepted_accesionNumberType = this.Options.RefSeq_accepted_accessionNumberType;
            ncbiRefSeq.Generate_by_reading_safed_file();
            NcbiRefSeq_lincs_line_class ncbi_line;
            ncbiRefSeq.NcbiRefSeq = ncbiRefSeq.NcbiRefSeq.OrderBy(l => l.UCSC_refSeqGeneId).ThenBy(l => l.NCBI_symbol).ToArray();
            int indexNCBI = 0;
            int indexNCBI_intern = 0;

            int ncbi_length = ncbiRefSeq.NcbiRefSeq.Length;

            this.Degs_complete = Deg_line_class.Order_by_gene(this.Degs_complete);
            int degs_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            int stringCompare = -2;

            Deg_line_class new_deg_line;
            List<Deg_line_class> new_degs = new List<Deg_line_class>();

            bool replaced;
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                deg_line = this.Degs_complete[indexDEG];
                replaced = false;
                stringCompare = -2;
                indexNCBI_intern = indexNCBI;
                while ((indexNCBI_intern < ncbi_length) && (stringCompare <= 0))
                {
                    ncbi_line = ncbiRefSeq.NcbiRefSeq[indexNCBI_intern];
                    if ((indexNCBI_intern == 0)
                        || (!ncbi_line.UCSC_refSeqGeneId.Equals(ncbiRefSeq.NcbiRefSeq[indexNCBI_intern - 1].UCSC_refSeqGeneId))
                        || (!ncbi_line.NCBI_symbol.Equals(ncbiRefSeq.NcbiRefSeq[indexNCBI_intern - 1].NCBI_symbol)))
                    {
                        stringCompare = ncbi_line.UCSC_refSeqGeneId.ToUpper().CompareTo(deg_line.Gene.ToUpper());
                        if (stringCompare < 0)
                        {
                            indexNCBI++;
                            indexNCBI_intern = indexNCBI;
                        }
                        else if (stringCompare == 0)
                        {
                            if (!replaced)
                            {
                                deg_line.Symbol = (string)ncbi_line.NCBI_symbol.Clone();
                                replaced = true;
                            }
                            else
                            {
                                new_deg_line = deg_line.Deep_copy();
                                new_deg_line.Symbol = (string)ncbi_line.NCBI_symbol.Clone();
                                new_degs.Add(new_deg_line);
                            }
                            indexNCBI_intern++;
                        }
                    }
                    else
                    {
                        indexNCBI_intern++;
                    }
                }
                if ((!replaced) || (String.IsNullOrEmpty(deg_line.Symbol)))
                {
                    //throw new Exception();
                }
            }
            Add_to_array(new_degs.ToArray());
            for (int i = 0; i < typeof(Deg_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} new deg lines added, because refSeqGeneId refered to multiple symbols", new_degs.Count);
        }
        private void Remove_duplicated_symbols_in_group_by_keeping_only_lines_with_most_significant_pvalue()
        {
            Report_class.Write("{0}: Remove_duplicated_symbols_in_group: ", typeof(Deg_class).Name);
            this.Degs_complete = Deg_line_class.Order_by_group_symbol_pvalue(this.Degs_complete);
            int degs_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            Deg_line_class sameSymbolGroup_deg_line = new Deg_line_class();
            List<Deg_line_class> new_degs = new List<Deg_line_class>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if ((indexDeg == 0)
                    || (!deg_line.Equal_group(this.Degs_complete[indexDeg - 1]))
                    || (!deg_line.Symbol.Equals(this.Degs_complete[indexDeg - 1].Symbol)))
                {
                    new_degs.Add(deg_line);
                }
            }
            Report_class.WriteLine("{0} of {1} deg_lines kept", new_degs.Count, degs_length);
            Degs_complete = new_degs.ToArray();
        }
        private void Calculate_signed_minus_log10_pvalue()
        {
            double minusLog10_pvalue;
            double pvalue;
            foreach (Deg_line_class deg_line in this.Degs_complete)
            {
                pvalue = deg_line.PValue;
                if (pvalue > Options.Max_pvalue_every_greater_pvalue_will_be_set_to_before_calculation_of_minusLog10Pvalue) { pvalue = Options.Max_pvalue_every_greater_pvalue_will_be_set_to_before_calculation_of_minusLog10Pvalue; }
                if (pvalue != 0)
                {
                    minusLog10_pvalue = -Math.Log10(pvalue);
                    if (minusLog10_pvalue >= Options.Replace_minuslog10pvalue_by_value_if_inf) { throw new Exception(); }
                    deg_line.Signed_minus_log10_pvalue = Math.Sign(deg_line.logFC) * minusLog10_pvalue;
                }
                else
                {
                    deg_line.Signed_minus_log10_pvalue = Math.Sign(deg_line.logFC) * Options.Replace_minuslog10pvalue_by_value_if_inf;
                }
            }
        }
        public void Continue_generation(Lincs_molecularEntity_enum molecularEntity)
        {
            switch (molecularEntity)
            {
                case Lincs_molecularEntity_enum.Rna:
                    if (!Options.DEG_harmonization_between_samples.Equals(DEG_harmonization_between_samples_enum.Add_missing_refSeq_genes))
                    {
                        throw new Exception();
                    }
                    Remove_empty_gene_names();
                    Check_for_deg_genes_that_are_not_part_of_ncbi_genes();
                    switch (Options.Add_ncbi_symbols_origin)
                    {
                        case Add_ncbi_symbols_origin_enum.Ncbi:
                            Add_missing_refSeq_genes_based_on_ncbi();
                            Add_ncbi_gene_symbols_based_on_ncbiRefSeqGeneIds();
                            Read_background_genes_based_on_ncbi();
                            break;
                        default:
                            throw new Exception();
                    }
                    Set_entryType_to_diffrna();
                    break;
                default:
                    throw new Exception();
            }
            Remove_empty_symbols();
            Remove_duplicated_symbols_in_group_by_keeping_only_lines_with_most_significant_pvalue();
            Replace_human_by_cellline();
            Set_to_upper_case();
            Drug_legend.Generate_de_novo();
            Add_drug_type_full_name_and_is_cardiotoxic_info();
            Calculate_signed_minus_log10_pvalue();
            Calculate_fractional_ranks_based_von_abs_signedMinusLog10Pvalues_ignoring_entryType();
            Check_for_duplicates_with_same_patient_treatment_plate_symbols();
        }
        public void Generate_from_input_array(Deg_line_class[] input_degs)
        {
            int input_degs_length = input_degs.Length;
            this.Degs_complete = new Deg_line_class[input_degs_length];
            for (int indexInput = 0; indexInput < input_degs_length; indexInput++)
            {
                this.Degs_complete[indexInput] = input_degs[indexInput].Deep_copy();
            }
            Drug_legend.Generate_de_novo();
            Add_drug_type_full_name_and_is_cardiotoxic_info();
        }
        public void Generate_by_reading_safed_files_and_process(Lincs_molecularEntity_enum molecularEntity, string filename)
        {
            this.Degs_complete = new Deg_line_class[0];
            Read_safed_file(filename);
            Continue_generation(molecularEntity);
        }
        #endregion

        #region Get
        public string[] Get_all_ordered_patients()
        {
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            List<string> patients = new List<string>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                patients.Add(deg_line.Patient);
            }
            return patients.Distinct().OrderBy(l => l).ToArray();
        }
        public string[] Get_all_ordered_plates()
        {
            this.Degs_complete = this.Degs_complete.OrderBy(l => l.Plate).ToArray();
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            List<string> plates = new List<string>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                if ((indexDeg == 0) || (!deg_line.Plate.Equals(Degs_complete[indexDeg - 1].Plate)))
                {
                    plates.Add(deg_line.Plate);
                }
            }
            return plates.ToArray();
        }
        public string[] Get_all_ordered_treatments()
        {
            this.Degs_complete = this.Degs_complete.OrderBy(l => l.Treatment).ToArray();
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            List<string> treatments = new List<string>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                if ((indexDeg == 0) || (!deg_line.Treatment.Equals(Degs_complete[indexDeg - 1].Treatment)))
                {
                    treatments.Add(deg_line.Treatment);
                }
            }
            return treatments.ToArray();
        }
        public string[] Get_all_ordered_datasets()
        {
            this.Degs_complete = this.Degs_complete.OrderBy(l => l.Dataset).ToArray();
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            List<string> datasets = new List<string>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                if ((indexDeg == 0) || (!deg_line.Dataset.Equals(Degs_complete[indexDeg - 1].Dataset)))
                {
                    datasets.Add(deg_line.Dataset);
                }
            }
            return datasets.ToArray();
        }
        #endregion

        public void Add_to_array(Deg_line_class[] add_degs_complete)
        {
            int this_length = this.Degs_complete.Length;
            int add_length = add_degs_complete.Length;
            int new_length = this_length + add_length;
            int indexNew = -1;
            Deg_line_class[] new_degs_complete = new Deg_line_class[new_length];
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_degs_complete[indexNew] = this.Degs_complete[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_degs_complete[indexNew] = add_degs_complete[indexAdd];
            }
            this.Degs_complete = new_degs_complete;
        }
        public void Keep_only_lines_with_nonZero_signedMinusLog10Pvalue()
        {
            int degs_length = Degs_complete.Length;
            Deg_line_class deg_line;
            List<Deg_line_class> kept_deg_list = new List<Deg_line_class>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = Degs_complete[indexDeg];
                if (deg_line.Signed_minus_log10_pvalue != 0)
                {
                    kept_deg_list.Add(deg_line);
                }
            }
            Degs_complete = kept_deg_list.ToArray();
        }
        private void Calculate_fractional_ranks_based_von_abs_signedMinusLog10Pvalues_ignoring_entryType()
        {
            this.Degs_complete = Deg_line_class.Order_by_group_and_descendingAbsSignedMinusLog10Pvalue_and_descendingAbsLog2foldchange(this.Degs_complete);
            int deg_lines_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            Deg_line_class inner_deg_line;
            int running_rank = 0;
            int first_line_same_absSignedMinusLog10P = -1;
            List<float> current_ranks = new List<float>();
            float fractional_rank;
            for (int indexDeg = 0; indexDeg < deg_lines_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if ((indexDeg == 0)
                    || (!deg_line.Dataset.Equals(this.Degs_complete[indexDeg - 1].Dataset))
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg - 1].Treatment))
                    || (!deg_line.Patient.Equals(this.Degs_complete[indexDeg - 1].Patient)))
                {
                    running_rank = 0;
                }
                if ((indexDeg == 0)
                    || (!deg_line.Dataset.Equals(this.Degs_complete[indexDeg - 1].Dataset))
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg - 1].Treatment))
                    || (!deg_line.Patient.Equals(this.Degs_complete[indexDeg - 1].Patient))
                    || (!Math.Abs(deg_line.Signed_minus_log10_pvalue).Equals(Math.Abs(this.Degs_complete[indexDeg - 1].Signed_minus_log10_pvalue))))
                {
                    current_ranks.Clear();
                    first_line_same_absSignedMinusLog10P = indexDeg;
                }
                running_rank++;
                current_ranks.Add(running_rank);
                if ((indexDeg == deg_lines_length - 1)
                    || (!deg_line.Dataset.Equals(this.Degs_complete[indexDeg + 1].Dataset))
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg + 1].Treatment))
                    || (!deg_line.Patient.Equals(this.Degs_complete[indexDeg + 1].Patient))
                    || (!Math.Abs(deg_line.Signed_minus_log10_pvalue).Equals(Math.Abs(this.Degs_complete[indexDeg + 1].Signed_minus_log10_pvalue))))
                {
                    if (current_ranks.Count == 1)
                    {
                        fractional_rank = current_ranks[0];
                        deg_line.Fractional_rank = fractional_rank;
                    }
                    else
                    {
                        fractional_rank = Math_class.Get_average(current_ranks.ToArray());
                        for (int indexInner = first_line_same_absSignedMinusLog10P; indexInner <= indexDeg; indexInner++)
                        {
                            inner_deg_line = this.Degs_complete[indexInner];
                            inner_deg_line.Fractional_rank = fractional_rank;
                        }
                    }
                }
            }
        }


        #region Background genes and proteins
        public string[] Get_all_bg_genes_in_upperCase_and_ordered()
        {
            int bg_genes_length = Bg_genes.Length;
            string[] symbols = new string[bg_genes_length];
            for (int indexBg = 0; indexBg < bg_genes_length; indexBg++)
            {
                symbols[indexBg] = (string)Bg_genes[indexBg].ToUpper();
            }
            return symbols.Distinct().OrderBy(l => l).ToArray();
        }
        #endregion

        #region Generate de instance
        public DE_class Generate_new_de_instance(Deg_score_of_interest_enum score_of_interest)
        {
            int degs_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            Fill_de_line_class fill_de_line;
            Fill_de_line_class[] fill_list = new Fill_de_line_class[degs_length];
            for (int indexD = 0; indexD < degs_length; indexD++)
            {
                deg_line = this.Degs_complete[indexD];
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Timepoint_for_de = deg_line.Timepoint;
                fill_de_line.Names_for_de = new string[] { deg_line.Drug_type.ToString(), deg_line.Patient, deg_line.Treatment, deg_line.Plate }; //for SVD
                fill_de_line.Symbols_for_de = new string[] { deg_line.Symbol };
                fill_de_line.Entry_type_for_de = deg_line.Entry_type_for_de;
                switch (score_of_interest)
                {
                    case Deg_score_of_interest_enum.Signed_minus_log10pvalue:
                        deg_line.Value_for_de = deg_line.Signed_minus_log10_pvalue;
                        break;
                    default:
                        throw new Exception();
                }
                if (Double.IsNaN(deg_line.Value_for_de)) { throw new Exception(); }
                fill_de_line.Value_for_de = deg_line.Value_for_de;
                fill_list[indexD] = fill_de_line;
            }
            DE_class de = new DE_class();
            de.Fill_with_data(fill_list);
            return de;
        }
        #endregion

        #region Count
        public Dictionary<string, int> Get_treatment_conditions_count_dict()
        {
            string dataset = this.Degs_complete[0].Dataset;
            Dictionary<string, int> treatment_conditions_dict = new Dictionary<string, int>();
            this.Degs_complete = Deg_line_class.Order_by_dataset_treatment_patient_plate(this.Degs_complete);
            Deg_line_class deg_line;
            int degs_length = this.Degs_complete.Length;
            int current_conditions_count = 0;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if (!deg_line.Dataset.Equals(dataset)) { throw new Exception(); }
                if ((indexDeg == 0)
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg - 1].Treatment)))
                {
                    current_conditions_count = 0;
                }
                if ((indexDeg == 0)
                    || (!deg_line.Patient.Equals(this.Degs_complete[indexDeg - 1].Patient))
                    || (!deg_line.Plate.Equals(this.Degs_complete[indexDeg - 1].Plate))
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg - 1].Treatment)))
                {
                    current_conditions_count++;
                }
                if ((indexDeg == degs_length - 1)
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg + 1].Treatment)))
                {
                    treatment_conditions_dict.Add(deg_line.Treatment, current_conditions_count);
                }
            }
            return treatment_conditions_dict;
        }
        public Dictionary<string, string[]> Get_treatment_celllines_dict()
        {
            string dataset = this.Degs_complete[0].Dataset;
            Dictionary<string, string[]> treatment_celllines_dict = new Dictionary<string, string[]>();
            this.Degs_complete = Deg_line_class.Order_by_dataset_treatment_patient_plate(this.Degs_complete);
            Deg_line_class deg_line;
            int degs_length = this.Degs_complete.Length;
            List<string> current_celllines = new List<string>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if (!deg_line.Dataset.Equals(dataset)) { throw new Exception(); }
                if ((indexDeg == 0)
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg - 1].Treatment)))
                {
                    current_celllines.Clear();
                }
                if ((indexDeg == 0)
                    || (!deg_line.Patient.Equals(this.Degs_complete[indexDeg - 1].Patient))
                    || (!deg_line.Plate.Equals(this.Degs_complete[indexDeg - 1].Plate))
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg - 1].Treatment)))
                {
                    current_celllines.Add(deg_line.Patient);
                }
                if ((indexDeg == degs_length - 1)
                    || (!deg_line.Treatment.Equals(this.Degs_complete[indexDeg + 1].Treatment)))
                {
                    if (current_celllines.Distinct().ToList().Count!=current_celllines.Count) { throw new Exception(); }
                    treatment_celllines_dict.Add(deg_line.Treatment, current_celllines.ToArray());
                }
            }
            return treatment_celllines_dict;
        }
        #endregion

        #region Get degs lines
        private Deg_line_class[] Keep_only_degs_with_indicated_plates(Deg_line_class[] degs_local, params string[] plates)
        {
            plates = plates.Distinct().OrderBy(l => l).ToArray();
            Dictionary<string, bool> keepPlates_dict = new Dictionary<string, bool>();
            foreach (string plate in plates)
            {
                keepPlates_dict.Add(plate, true);
            }
            int degs_length = degs_local.Length;
            Deg_line_class deg_line;
            List<Deg_line_class> kept_degs = new List<Deg_line_class>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = degs_local[indexDeg];
                if (keepPlates_dict.ContainsKey(deg_line.Plate))
                {
                    kept_degs.Add(deg_line);
                }
            }
            return kept_degs.ToArray();
        }
        #endregion

        #region Set
        public void Set_entryType_based_on_directionality_of_signedMinusLog10Pvalue()
        {
            int data_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                deg_line = this.Degs_complete[indexD];
                switch (deg_line.EntryType)
                {
                    case DE_entry_enum.Diffrna:
                        if (deg_line.Signed_minus_log10_pvalue >= 0)
                        {
                            deg_line.EntryType = DE_entry_enum.Diffrna_up;
                        }
                        else
                        {
                            deg_line.EntryType = DE_entry_enum.Diffrna_down;
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
        #endregion

        public string Get_cell_type_and_check_if_only_one()
        {
            string cell_type = (string)this.Degs_complete[0].Cell_line_type.Clone();
            foreach (Deg_line_class deg_line in Degs_complete)
            {
                if (!deg_line.Cell_line_type.Equals(cell_type)) { throw new Exception(); }
            }
            return cell_type;
        }
        public string[] Get_all_symbols()
        {
            int degs_complete_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            List<string> all_symbols = new List<string>();
            for (int indexDeg = 0; indexDeg < degs_complete_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                all_symbols.Add(deg_line.Symbol);
            }
            return all_symbols.Distinct().OrderBy(l => l).ToArray();
        }
        public void Remove_indicated_drugTypes(params Drug_type_enum[] remove_drugTypes)
        {
            int degs_length = this.Degs_complete.Length;
            Deg_line_class deg_line;
            List<Deg_line_class> keep_degs = new List<Deg_line_class>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if (!remove_drugTypes.Contains(deg_line.Drug_type))
                {
                    keep_degs.Add(deg_line);
                }
            }
            this.Degs_complete = keep_degs.ToArray();
        }

        #region Keep
        public void Keep_only_lines_with_indicated_symbols(params string[] symbols)
        {
            symbols = symbols.Distinct().OrderBy(l => l).ToArray();
            this.Degs_complete = this.Degs_complete.OrderBy(l => l.Symbol).ToArray();
            int degs_complete_length = this.Degs_complete.Length;
            int symbols_length = symbols.Length;
            string symbol;
            int indexS = 0;
            int stringCompare = -2;
            Report_class.Write("{0}: Keep only symbols that are part of {1} input symbols", typeof(Deg_class).Name, symbols_length);
            Deg_line_class deg_line;
            List<Deg_line_class> keep = new List<Deg_line_class>();
            List<string> removed_symbols = new List<string>();
            for (int indexDeg = 0; indexDeg < degs_complete_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                stringCompare = -2;
                while ((indexS < symbols_length) && (stringCompare < 0))
                {
                    symbol = symbols[indexS];
                    stringCompare = symbol.CompareTo(deg_line.Symbol.ToUpper());
                    if (stringCompare < 0)
                    {
                        indexS++;
                    }
                }
                if (stringCompare == 0)
                {
                    keep.Add(deg_line);
                }
                else
                {
                    removed_symbols.Add(deg_line.Symbol);
                }
            }
            removed_symbols = removed_symbols.Distinct().ToList();
            this.Degs_complete = keep.ToArray();
            for (int i = 0; i < typeof(Deg_class).Name.Length; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} of {1} lines kept", this.Degs_complete.Length, degs_complete_length);
            for (int i = 0; i < typeof(Deg_class).Name.Length; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} symbols removed", removed_symbols.Count);
        }
        public void Keep_only_indicated_plates(params string[] plates)
        {
            this.Degs_complete = this.Keep_only_degs_with_indicated_plates(this.Degs_complete, plates);
        }
        public void Keep_only_top_lines_with_existing_fractional_rank_below_cutoff(float max_fractional_rank)
        {
            List<Deg_line_class> keep = new List<Deg_line_class>();
            foreach (Deg_line_class deg_line in this.Degs_complete)
            {
                if (deg_line.Fractional_rank <= 0) { throw new Exception(); }
                else if (deg_line.Fractional_rank <= max_fractional_rank)
                {
                    keep.Add(deg_line);
                }
            }
            this.Degs_complete = keep.ToArray();
        }
        #endregion

        public void Dispose()
        {
            Degs_complete = null;
            Bg_genes = null;
            Drug_legend = null;
            Options = null;
            Averaged_data = false;
        }

        #region Read copy
        public void Write_for_paper(string directory, string fileName)
            {
                if (this.Averaged_data) { throw new Exception(); }
                Deg_website_readWriteOptions_class readWriteOptions = new Deg_website_readWriteOptions_class(directory, fileName, "Cell_line");
                readWriteOptions.HeadlineDelimiters = new char[] { Global_class.Tab };
                readWriteOptions.LineDelimiters = new char[] { Global_class.Tab };
                ReadWriteClass.WriteData<Deg_line_class>(this.Degs_complete, readWriteOptions);
            }
        public void Write_for_website(string directory, string fileName, string cell_type)
        {
            if (this.Averaged_data) { throw new Exception(); }
            Deg_website_readWriteOptions_class readWriteOptions = new Deg_website_readWriteOptions_class(directory, fileName, cell_type);
            ReadWriteClass.WriteData<Deg_line_class>(this.Degs_complete, readWriteOptions);
        }
        private void Set_bg_genes()
        {
            switch (Options.Add_ncbi_symbols_origin)
            {
                case Add_ncbi_symbols_origin_enum.Ncbi:
                    Read_background_genes_based_on_ncbi();
                    break;
                default:
                    throw new Exception();
            }
        }
        private void Write_as_binary_private(Deg_line_class[] degs_complete, string complete_fileName, FileMode fileMode)
        {
            ReadWriteClass.Create_directory_if_it_does_not_exist(complete_fileName);
            BinaryWriter binaryWriter = new BinaryWriter(File.Open(complete_fileName, fileMode));
            int degs_length = degs_complete.Length;
            Deg_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.Degs_complete[indexDeg];
                if (deg_line.Fractional_rank <= 0) { throw new Exception(); }
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Plate));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Patient));
                binaryWriter.Write(deg_line.Timepoint.ToString());
                binaryWriter.Write(deg_line.EntryType.ToString());
                binaryWriter.Write(deg_line.Drug_type.ToString());
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Treatment));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Treatment_full_name));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Is_cardiotoxic));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Gene));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Symbol));
                binaryWriter.Write(deg_line.logFC);
                binaryWriter.Write(deg_line.PValue);
                binaryWriter.Write(deg_line.Signed_minus_log10_pvalue);
                binaryWriter.Write(deg_line.FDR);
                binaryWriter.Write(deg_line.Fractional_rank);
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Plate));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_line.Dataset));
                binaryWriter.Write(deg_line.Control_wells_count);
                binaryWriter.Write(deg_line.Treatment_wells_count);
            }
            binaryWriter.Close();
        }
        private void Write_bgGenes_as_binary_private(string[] bgGenes, string complete_bgGenes_fileName, FileMode fileMode)
        {
            ReadWriteClass.Create_directory_if_it_does_not_exist(complete_bgGenes_fileName);
            BinaryWriter binaryWriter = new BinaryWriter(File.Open(complete_bgGenes_fileName, fileMode));
            int bgGenes_length = bgGenes.Length;
            string bgGene;
            for (int indexDeg = 0; indexDeg < bgGenes_length; indexDeg++)
            {
                bgGene = bgGenes[indexDeg];
                binaryWriter.Write(bgGene);
            }
            binaryWriter.Close();
        }
        public void Write_preprocessed_as_binary(string filename_before_preprocessing)
        {
            string preprocessed_filename = Options.Get_name_of_preprocessed_degs(filename_before_preprocessing);
            Deg_preprocessed_readWriteOptions_class readWriteOptions = new Deg_preprocessed_readWriteOptions_class(preprocessed_filename);
            string preprocesses_binary_fileName = readWriteOptions.Get_binary_fileName();
            Write_as_binary_private(this.Degs_complete, preprocesses_binary_fileName, FileMode.Create);
            string complete_bgGenes_fileName = readWriteOptions.Get_binary_bgGenes_fileName();
            Write_bgGenes_as_binary_private(this.Bg_genes, complete_bgGenes_fileName, FileMode.Create);
        }
        private void Read_preprocessed_as_binary(string complete_fileName, params float[] max_pvalues)
        {
            float max_pvalue = 99;
            if (max_pvalues.Length > 1) { throw new Exception(); }
            else if (max_pvalues.Length == 1)
            {
                max_pvalue = max_pvalues[0];
            }
            BinaryReader binaryReader = new BinaryReader(File.OpenRead(complete_fileName));
            Deg_line_class deg_line;
            List<Deg_line_class> deg_list = new List<Deg_line_class>();
            long baseStream_length = binaryReader.BaseStream.Length;
            int nonzero_signedMinusLog10Pvalues_count = 0;
            while (binaryReader.BaseStream.Position != baseStream_length)
            {
                deg_line = new Deg_line_class();
                deg_line.Plate = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.Patient = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.Timepoint = (Timepoint_enum)Enum.Parse(typeof(Timepoint_enum), binaryReader.ReadString());
                deg_line.EntryType = (DE_entry_enum)Enum.Parse(typeof(DE_entry_enum), binaryReader.ReadString());
                deg_line.Drug_type = (Drug_type_enum)Enum.Parse(typeof(Drug_type_enum), binaryReader.ReadString());
                deg_line.Treatment = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.Treatment_full_name = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.Is_cardiotoxic = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.Gene = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.Symbol = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.logFC = binaryReader.ReadSingle();
                deg_line.PValue = binaryReader.ReadDouble();
                deg_line.Signed_minus_log10_pvalue = binaryReader.ReadDouble();
                deg_line.FDR = binaryReader.ReadDouble();
                deg_line.Fractional_rank = binaryReader.ReadSingle();
                if (deg_line.Fractional_rank < 1) { throw new Exception(); }
                deg_line.Plate = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.Dataset = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_line.Control_wells_count = binaryReader.ReadInt32();
                deg_line.Treatment_wells_count = binaryReader.ReadInt32();
                if (deg_line.Signed_minus_log10_pvalue != 0) { nonzero_signedMinusLog10Pvalues_count++; }
                if (deg_line.PValue <= max_pvalue)
                {
                    deg_list.Add(deg_line);
                }
            }
            if ((float)nonzero_signedMinusLog10Pvalues_count < (float)deg_list.Count * 0.5F) { throw new Exception(); }
            this.Degs_complete = deg_list.ToArray();
            binaryReader.Close();
        }
        private void Read_bgGenes_as_binary(string complete_bgGenes_fileName)
        {
            BinaryReader binaryReader = new BinaryReader(File.OpenRead(complete_bgGenes_fileName));
            List<string> bgGenes_list = new List<string>();
            string bgGene;
            long baseStream_length = binaryReader.BaseStream.Length;
            while (binaryReader.BaseStream.Position != baseStream_length)
            {
                bgGene = binaryReader.ReadString();
                bgGenes_list.Add(bgGene);
            }
            this.Bg_genes = bgGenes_list.ToArray();
        }
        public void Read_preprocessed_fileName_and_bgGenes(string filename_before_preprocessing)
        {
            string preprocessed_filename;
            preprocessed_filename = Options.Get_name_of_preprocessed_degs(filename_before_preprocessing);
            Deg_preprocessed_readWriteOptions_class readWriteOptions = new Deg_preprocessed_readWriteOptions_class(Global_directory_class.Final_lincs_degs_directory, preprocessed_filename);
            Read_preprocessed_as_binary(readWriteOptions.Get_binary_fileName());
            string complete_bgGenes_fileName = readWriteOptions.Get_binary_bgGenes_fileName();
            Read_bgGenes_as_binary(complete_bgGenes_fileName);
        }
        public void Read_background_genes_based_on_ncbi()
        {
            NcbiRefSeq_lincs_class ncbi = new NcbiRefSeq_lincs_class();
            ncbi.Options.RefSeq_accepted_accesionNumberType = this.Options.RefSeq_accepted_accessionNumberType;
            ncbi.Generate_by_reading_safed_file();
            this.Bg_genes = ncbi.Get_all_ordered_ncbi_symbols();
        }
        public Deg_class Deep_copy()
        {
            Deg_class copy = (Deg_class)this.MemberwiseClone();
            int degs_length = this.Degs_complete.Length;
            copy.Degs_complete = new Deg_line_class[degs_length];
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                copy.Degs_complete[indexDEG] = this.Degs_complete[indexDEG].Deep_copy();
            }
            int bg_genes_length = this.Bg_genes.Length;
            copy.Bg_genes = new string[bg_genes_length];
            for (int indexBG = 0; indexBG < bg_genes_length; indexBG++)
            {
                copy.Bg_genes[indexBG] = (string)this.Bg_genes[indexBG].Clone();
            }
            copy.Drug_legend = this.Drug_legend.Deep_copy();
            return copy;
        }
        public void Read_safed_file(string filename)
        {
            Deg_readWriteOptions_class readWriteOptions = new Deg_readWriteOptions_class(Global_directory_class.Lincs_degs_non_binary_files_directory, filename);
            this.Degs_complete = ReadWriteClass.ReadRawData_and_FillArray<Deg_line_class>(readWriteOptions);
        }
        #endregion
    }

    ////////////////////////////////////////////////////////////////////

    class Deg_average_line_class : Deg_line_class
    {
        public int Count_upregulated_genes { get; set; }
        public int Count_downregulated_genes { get; set; }
        public int Count_total_celllines { get; set; }
        public float Percent_of_celllines_with_gene_as_DEG { get; set; }

        public static Deg_average_line_class[] Order_average_lines_by_group_descending_abs_minusLog10Pvalue_descending_abs_log2fc(Deg_average_line_class[] average_lines)
        {
            int average_lines_length = average_lines.Length;
            Deg_line_class[] up_casted_deg_lines = new Deg_line_class[average_lines_length];
            for (int indexA = 0; indexA < average_lines_length; indexA++)
            {
                up_casted_deg_lines[indexA] = (Deg_line_class)average_lines[indexA];
            }
            up_casted_deg_lines = Deg_line_class.Order_by_group_and_descendingAbsSignedMinusLog10Pvalue_and_descendingAbsLog2foldchange(up_casted_deg_lines);
            for (int indexA = 0; indexA < average_lines_length; indexA++)
            {
                average_lines[indexA] = (Deg_average_line_class)up_casted_deg_lines[indexA];
            }
            return average_lines;
        }
        public static Deg_average_line_class[] Order_average_lines_by_group(Deg_average_line_class[] average_lines)
        {
            int average_lines_length = average_lines.Length;
            Deg_line_class[] up_casted_deg_lines = new Deg_line_class[average_lines_length];
            for (int indexA = 0; indexA < average_lines_length; indexA++)
            {
                up_casted_deg_lines[indexA] = (Deg_line_class)average_lines[indexA];
            }
            up_casted_deg_lines = Deg_line_class.Order_by_group(up_casted_deg_lines);
            for (int indexA = 0; indexA < average_lines_length; indexA++)
            {
                average_lines[indexA] = (Deg_average_line_class)up_casted_deg_lines[indexA];
            }
            return average_lines;
        }

        public Deg_average_line_class Deep_copy_deg_average_line()
        {
            Deg_average_line_class copy = (Deg_average_line_class)base.Deep_copy();
            copy.Count_downregulated_genes = this.Count_downregulated_genes;
            copy.Count_downregulated_genes = this.Count_downregulated_genes;
            copy.Count_total_celllines = this.Count_total_celllines;
            copy.Percent_of_celllines_with_gene_as_DEG = this.Percent_of_celllines_with_gene_as_DEG;
            return copy;
        }
    }
    class Deg_averaged_website_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter = ',';

        public Deg_averaged_website_readWriteOptions_class(string subdirectory, string file_name, int topDEGs)
        {
            File = Global_directory_class.Results_directory + subdirectory + file_name;

            Key_propertyNames = new string[] {  "Treatment_full_name",  //1
                                                "DrugClass_for_website", //0
                                                "Is_cardiotoxic",
                                                "Symbol", //4
                                                "Up_down_status_signedMinusLog10",//7
                                                "Signed_minus_log10_pvalue", //8
                                                "Fractional_rank",//9
                                                "Count_upregulated_genes",//10
                                                "Count_downregulated_genes",//11
                                                "Count_total_celllines",//12
                                                "Percent_of_celllines_with_gene_as_DEG"//13
                                             };
            Key_columnNames = new string[] {    Lincs_website_conversion_class.Label_drugName,  //1
                                                Lincs_website_conversion_class.Label_drug_class,
                                                "Is cardiotoxic",
                                                Lincs_website_conversion_class.Label_gene_symbol, //4
                                                Lincs_website_conversion_class.Label_up_down_status,//7
                                                Lincs_website_conversion_class.Label_signed_minus_log10pvalue,//8
                                                Lincs_website_conversion_class.Label_fractional_rank,//9
                                                "No of cell lines with upregulated gene",//10
                                                "No of cell lines with downregulated gene",//11
                                                "No of cell lines",//12
                                                "% cell lines with gene among top " + topDEGs + " DEGs"//"Percentage of significant DEG"//13
                                           };

            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            File_has_headline = true;
            Report = ReadWrite_report_enum.Report_main;
        }
    }
    class Deg_average_class
    {
        public Deg_average_line_class[] Degs_average { get; set; }

        #region Generate
        private Deg_average_line_class[] Keep_only_lines_with_indicated_max_fractional_rank(Deg_average_line_class[] average_lines, int max_fractional_rank)
        {
            List<Deg_average_line_class> keep = new List<Deg_average_line_class>();
            foreach (Deg_average_line_class average_line in average_lines)
            {
                if (average_line.Fractional_rank<= max_fractional_rank)
                {
                    keep.Add(average_line);
                }
            }
            if (keep.Count==0) { throw new Exception(); }
            return keep.ToArray();
        }
        private Deg_average_line_class[] Calculate_fractional_ranks_based_on_abs_signedMinusLog10Pvalues_ignoring_entryType(Deg_average_line_class[] average_lines)
        {
            average_lines = Deg_average_line_class.Order_average_lines_by_group_descending_abs_minusLog10Pvalue_descending_abs_log2fc(average_lines);
            int average_lines_length = average_lines.Length;
            Deg_average_line_class average_line;
            Deg_average_line_class inner_average_line;
            int running_rank =0;
            int first_line_same_absSignedMinusLog10P = -1;
            List<float> current_ranks = new List<float>();
            float fractional_rank;
            for (int indexA=0; indexA<average_lines_length; indexA++)
            {
                average_line = average_lines[indexA];
                if ((indexA == 0)
                    || (!average_line.Dataset.Equals(average_lines[indexA - 1].Dataset))
                    || (!average_line.Treatment.Equals(average_lines[indexA - 1].Treatment))
                    || (!average_line.Patient.Equals(average_lines[indexA - 1].Patient)))
                {
                    running_rank = 0;
                }
                if ((indexA == 0)
                    || (!average_line.Dataset.Equals(average_lines[indexA - 1].Dataset))
                    || (!average_line.Treatment.Equals(average_lines[indexA - 1].Treatment))
                    || (!average_line.Patient.Equals(average_lines[indexA - 1].Patient))
                    || (!Math.Abs(average_line.Signed_minus_log10_pvalue).Equals(Math.Abs(average_lines[indexA - 1].Signed_minus_log10_pvalue))))
                {
                    current_ranks.Clear();
                    first_line_same_absSignedMinusLog10P = indexA;
                }
                running_rank++;
                current_ranks.Add(running_rank);
                if ((indexA == average_lines_length-1)
                    || (!average_line.Dataset.Equals(average_lines[indexA + 1].Dataset))
                    || (!average_line.Treatment.Equals(average_lines[indexA + 1].Treatment))
                    || (!average_line.Patient.Equals(average_lines[indexA + 1].Patient))
                    || (!Math.Abs(average_line.Signed_minus_log10_pvalue).Equals(Math.Abs(average_lines[indexA + 1].Signed_minus_log10_pvalue))))
                {
                    if (current_ranks.Count==1)
                    {
                        fractional_rank = current_ranks[0];
                        average_line.Fractional_rank = fractional_rank;
                    }
                    else
                    {
                        fractional_rank = Math_class.Get_average(current_ranks.ToArray());
                        for (int indexInner = first_line_same_absSignedMinusLog10P; indexInner <= indexA; indexInner++)
                        {
                            inner_average_line = average_lines[indexInner];
                            inner_average_line.Fractional_rank = fractional_rank;
                        }
                    }
                }
            }
            return average_lines;
        }
        public void Generate_from_degs_for_each_drug_and_library_prepartion_method_over_celllines_and_plates(Deg_class deg, int max_fractional_rank_for_averaged_DEGs)
        {
            Dictionary<string, int> treatment_conditions_count_dict = deg.Get_treatment_conditions_count_dict();
            Dictionary<string, string[]> treatment_celllines_dict = deg.Get_treatment_celllines_dict();

            deg.Degs_complete = Deg_line_class.Order_by_treatment_libraryPreparationMethod_symbol(deg.Degs_complete);
            int degs_length = deg.Degs_complete.Length;
            Deg_line_class deg_line;
            Deg_average_line_class average_deg_line;
            List<Deg_average_line_class> average_deg_lines = new List<Deg_average_line_class>();
            List<Deg_average_line_class> current_average_deg_lines = new List<Deg_average_line_class>();
            Deg_average_line_class[] add_average_deg_lines;
            int treatments_count = 0;
            double minusLog10FDR_mean = 0;
            double meanLog2foldchange = 0;
            double meanSignedMinusLog10pvalue = 0;
            List<float> current_minusLog10FDR = new List<float>();
            List<float> current_log2foldchange = new List<float>();
            List<double> current_signed_minusLog10pvalue = new List<double>();
            int current_count_of_upregulated_DEGs = 0;
            int current_count_of_downregulated_DEGs = 0;
            int total_cellline_counts = 0;
            string is_cardiotoxic = "";
            Dictionary<string, Dictionary<string, string>> averaged_treatment_symbols_dict = new Dictionary<string, Dictionary<string, string>>();
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                deg_line = deg.Degs_complete[indexDEG];

                if ((indexDEG == 0)
                    || (!deg_line.Plate.Equals(deg.Degs_complete[indexDEG - 1].Plate))
                    || (!deg_line.Treatment.Equals(deg.Degs_complete[indexDEG - 1].Treatment)))
                {
                current_average_deg_lines.Clear();
                }
                if ((indexDEG == 0)
                    || (!deg_line.Plate.Equals(deg.Degs_complete[indexDEG - 1].Plate))
                    || (!deg_line.Treatment.Equals(deg.Degs_complete[indexDEG - 1].Treatment))
                    || (!deg_line.Symbol.Equals(deg.Degs_complete[indexDEG - 1].Symbol)))
                {
                    current_count_of_upregulated_DEGs = 0;
                    current_count_of_downregulated_DEGs = 0;
                    total_cellline_counts = 0;
                    current_log2foldchange.Clear();
                    current_minusLog10FDR.Clear();
                    current_signed_minusLog10pvalue.Clear();
                    is_cardiotoxic = deg_line.Is_cardiotoxic;
                }
                if (!deg_line.Is_cardiotoxic.Equals(is_cardiotoxic)) { throw new Exception(); }
                current_minusLog10FDR.Add(-(float)Math.Log10(deg_line.FDR));
                current_log2foldchange.Add(deg_line.logFC);
                current_signed_minusLog10pvalue.Add(deg_line.Signed_minus_log10_pvalue);
                total_cellline_counts++;
                if (deg_line.Signed_minus_log10_pvalue > 0)
                {
                    current_count_of_upregulated_DEGs++;
                }
                else if (deg_line.Signed_minus_log10_pvalue < 0)
                {
                    current_count_of_downregulated_DEGs++;
                }
                if ((indexDEG == degs_length - 1)
                    || (!deg_line.Treatment.Equals(deg.Degs_complete[indexDEG + 1].Treatment))
                    || (!deg_line.Plate.Equals(deg.Degs_complete[indexDEG + 1].Plate))
                    || (!deg_line.Symbol.Equals(deg.Degs_complete[indexDEG + 1].Symbol)))
                {
                    treatments_count = treatment_conditions_count_dict[deg_line.Treatment];
                    while (current_minusLog10FDR.Count < treatments_count)
                    {
                        current_minusLog10FDR.Add(0);
                        current_log2foldchange.Add(0);
                        current_signed_minusLog10pvalue.Add(0);
                        total_cellline_counts++;
                    }
                    average_deg_line = new Deg_average_line_class();
                    average_deg_line.Treatment = (string)deg_line.Treatment.Clone();
                    minusLog10FDR_mean = Math_class.Get_average(current_minusLog10FDR.ToArray());
                    meanLog2foldchange = Math_class.Get_average(current_log2foldchange.ToArray());
                    meanSignedMinusLog10pvalue = Math_class.Get_average(current_signed_minusLog10pvalue.ToArray());
                    average_deg_line.FDR = (float)Math.Pow(10, -minusLog10FDR_mean);
                    average_deg_line.Signed_minus_log10_pvalue = meanSignedMinusLog10pvalue;
                    average_deg_line.Is_cardiotoxic = (string)deg_line.Is_cardiotoxic.Clone();
                    average_deg_line.Treatment_full_name = (string)deg_line.Treatment_full_name.Clone();
                    average_deg_line.Plate = (string)deg_line.Plate.Clone();
                    average_deg_line.logFC = (float)meanLog2foldchange;
                    average_deg_line.Count_upregulated_genes = current_count_of_upregulated_DEGs;
                    average_deg_line.Count_downregulated_genes = current_count_of_downregulated_DEGs;
                    average_deg_line.Count_total_celllines = total_cellline_counts;
                    average_deg_line.Percent_of_celllines_with_gene_as_DEG = 100 * (current_count_of_upregulated_DEGs + current_count_of_downregulated_DEGs) / total_cellline_counts;

                    StringBuilder sb = new StringBuilder();
                    string[] celllines = treatment_celllines_dict[average_deg_line.Treatment];
                    string cellline;
                    int celllines_length = celllines.Length;
                    for (int indexCL = 0; indexCL < celllines_length; indexCL++)
                    {
                        cellline = celllines[indexCL];
                        if (indexCL >= 1) { sb.AppendFormat(";"); }
                        sb.AppendFormat(cellline);
                    }
                    average_deg_line.Patient = sb.ToString();
                    average_deg_line.Plate = (string)average_deg_line.Plate.Clone();
                    average_deg_line.Drug_type = deg_line.Drug_type;
                    average_deg_line.EntryType = DE_entry_enum.Diffrna;
                    average_deg_line.Gene = "";
                    average_deg_line.Symbol = (string)deg_line.Symbol.Clone();

                    if (!averaged_treatment_symbols_dict.ContainsKey(deg_line.Treatment))
                    {
                        averaged_treatment_symbols_dict.Add(deg_line.Treatment, new Dictionary<string, string>());
                    }
                    if (averaged_treatment_symbols_dict[deg_line.Treatment].ContainsKey(deg_line.Symbol))
                    {
                        string gene = averaged_treatment_symbols_dict[deg_line.Treatment][deg_line.Symbol];
                        throw new Exception();
                    }
                    averaged_treatment_symbols_dict[deg_line.Treatment].Add(deg_line.Symbol, (string)deg_line.Gene.Clone());
                    average_deg_line.Fractional_rank = -1;
                    current_average_deg_lines.Add(average_deg_line);
                }
                if ((indexDEG == degs_length-1)
                    || (!deg_line.Plate.Equals(deg.Degs_complete[indexDEG + 1].Plate))
                    || (!deg_line.Treatment.Equals(deg.Degs_complete[indexDEG + 1].Treatment)))
                {
                    add_average_deg_lines = current_average_deg_lines.ToArray();
                    add_average_deg_lines = Calculate_fractional_ranks_based_on_abs_signedMinusLog10Pvalues_ignoring_entryType(add_average_deg_lines);
                    add_average_deg_lines = Keep_only_lines_with_indicated_max_fractional_rank(add_average_deg_lines,max_fractional_rank_for_averaged_DEGs);
                    average_deg_lines.AddRange(add_average_deg_lines);
                }
            }
            this.Degs_average = average_deg_lines.ToArray();
        }
        #endregion
        public void Set_entryType_based_on_directionality_of_signedMinusLog10Pvalue()
        {
            foreach (Deg_average_line_class average_line in this.Degs_average)
            {
                if (average_line.Signed_minus_log10_pvalue >= 0)
                {
                    average_line.EntryType = DE_entry_enum.Diffrna_up;
                }
                else
                {
                    average_line.EntryType = DE_entry_enum.Diffrna_down;
                }
            }
        }

        #region de
        public DE_class Generate_new_de_instance()
        {
            int degs_length = this.Degs_average.Length;
            Deg_average_line_class deg_line;
            Fill_de_line_class fill_de_line;
            Fill_de_line_class[] fill_list = new Fill_de_line_class[degs_length];
            //double pvalue;
            for (int indexD = 0; indexD < degs_length; indexD++)
            {
                deg_line = this.Degs_average[indexD];
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Timepoint_for_de = deg_line.Timepoint;
                fill_de_line.Names_for_de = new string[] { deg_line.Treatment, deg_line.Patient, deg_line.Drug_type.ToString(), deg_line.Plate }; //for SVD
                fill_de_line.Symbols_for_de = new string[] { deg_line.Symbol };
                fill_de_line.Entry_type_for_de = deg_line.Entry_type_for_de;
                deg_line.Value_for_de = deg_line.Signed_minus_log10_pvalue;
                if (Double.IsNaN(deg_line.Value_for_de)) { throw new Exception(); }
                fill_de_line.Value_for_de = deg_line.Value_for_de;
                fill_list[indexD] = fill_de_line;
            }
            DE_class de = new DE_class();
            de.Fill_with_data(fill_list);
            return de;
        }
        #endregion

        public Deg_average_class Deep_copy()
        {
            Deg_average_class copy = (Deg_average_class)this.MemberwiseClone();
            int degs_average_length = this.Degs_average.Length;
            copy.Degs_average = new Deg_average_line_class[degs_average_length];
            for (int indexA=0; indexA<degs_average_length; indexA++)
            {
                copy.Degs_average[indexA] = this.Degs_average[indexA].Deep_copy_deg_average_line();
            }
            return copy;
        }

        public void Write_average_for_website(string directory, string fileName, int topDEGsDEPs)
        {
             Deg_averaged_website_readWriteOptions_class readWriteOptions = new Deg_averaged_website_readWriteOptions_class(directory, fileName, topDEGsDEPs);
             ReadWriteClass.WriteData<Deg_average_line_class>(this.Degs_average, readWriteOptions);
        }
        public void Write_average_for_paper(string directory, string fileName, int topDEGsDEPs)
        {
            Deg_averaged_website_readWriteOptions_class readWriteOptions = new Deg_averaged_website_readWriteOptions_class(directory, fileName, topDEGsDEPs);
            readWriteOptions.HeadlineDelimiters = new char[] { Global_class.Tab };
            readWriteOptions.LineDelimiters = new char[] { Global_class.Tab };
            ReadWriteClass.WriteData<Deg_average_line_class>(this.Degs_average, readWriteOptions);
        }

    }

    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Deg_summary_line_class
    {
        #region Fields
        public int Treatment_wells_count { get; set; }
        public int Control_wells_count { get; set; }
        public string[] Treatment_subjects { get; set; }
        public string[] Control_subjects { get; set; }
        public Drug_type_enum Drug_type { get; set; }
        public string Is_cardiotoxic { get; set; }
        public string Treatment { get; set; }
        public string Treatment_full_name { get; set; }
        public string Plate { get; set; }
        public string Cell_line { get; set; }
        public int Significant_degs_based_on_FDR_count { get; set; }
        public float FDR_cutoff { get; set; }
        public int Significant_degs_based_on_pvalue_count { get; set; }
        public float Pvalue_cutoff { get; set; }
        public string Cell_line_type { get; set; }
        public int Replicate_no { get; set; }
        public string Cell_line_for_website
        {
            get
            {
                return Lincs_website_conversion_class.Get_cellLine_for_website(Cell_line_type, Cell_line, Plate);
            }
        }
        public string DrugClass_for_website
        {
            get
            {
                return Lincs_website_conversion_class.Get_drugClass_for_website(Drug_type);
            }
        }

        public string Plate_for_website
        {
            get
            {
                return Lincs_website_conversion_class.Get_plate_for_website(Plate);
            }

        }
        public string ReadWrite_treatment_subjects
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Treatment_subjects, Deg_summary_readWriteOptions_class.Array_delimiter); }
            set { this.Treatment_subjects = ReadWriteClass.Get_array_from_readLine<string>(value, Deg_summary_readWriteOptions_class.Array_delimiter); }
        }
        public string ReadWrite_control_subjects
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Control_subjects, Deg_summary_readWriteOptions_class.Array_delimiter); }
            set { this.Control_subjects = ReadWriteClass.Get_array_from_readLine<string>(value, Deg_summary_readWriteOptions_class.Array_delimiter); }
        }
        #endregion

        public Deg_summary_line_class()
        {
            Replicate_no = -1;
            Control_subjects = new string[0];
            Treatment_subjects = new string[0];
            Treatment_wells_count = -1;
            Control_wells_count = -1;
            Is_cardiotoxic = "";
        }

        public Deg_summary_line_class Deep_copy()
        {
            Deg_summary_line_class copy = (Deg_summary_line_class)this.MemberwiseClone();
            copy.Treatment = (string)this.Treatment.Clone();
            copy.Treatment_full_name = (string)this.Treatment_full_name.Clone();
            copy.Plate = (string)this.Plate.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            copy.Treatment_subjects = Array_class.Deep_copy_string_array(this.Treatment_subjects);
            copy.Control_subjects = Array_class.Deep_copy_string_array(this.Control_subjects);
            copy.Is_cardiotoxic = (string)this.Is_cardiotoxic.Clone();
            return copy;
        }
    }
    class Deg_summary_options_class
    {
        public float FDR_cutoff { get; set; }
        public float Pvalue_cutoff { get; set; }

        public Deg_summary_options_class()
        {
            FDR_cutoff = 0.1F;
            Pvalue_cutoff = 0.1F;
        }
    }
    class Deg_summary_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Deg_summary_readWriteOptions_class(string subdirectory, string file_name)
        {
            string directory;
            if (subdirectory.IndexOf(':') == -1)
            {
                directory = Global_directory_class.Results_directory + subdirectory;
            }
            else
            {
                directory = subdirectory;
            }
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + file_name;
            this.Key_propertyNames = new string[] { "Drug_type", "Plate", "Cell_line", "Treatment", "Replicate_no", "Treatment_full_name", "Treatment_wells_count", "Control_wells_count", "ReadWrite_treatment_subjects", "ReadWrite_control_subjects", "FDR_cutoff", "Pvalue_cutoff", "Significant_degs_based_on_FDR_count", "Significant_degs_based_on_pvalue_count" };

            this.Key_columnNames = this.Key_propertyNames;
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            File_has_headline = true;
        }
    }
    class Deg_summary_class
    {
        public Deg_summary_line_class[] Summaries { get; set; }
        public Deg_summary_options_class Options { get; set; }

        public Deg_summary_class()
        {
            Options = new Deg_summary_options_class();
        }

        private void Generate_from_deg_instance(Deg_class deg)
        {
            string cell_type = (string)deg.Degs_complete[0].Cell_line_type.Clone();
            int degs_length = deg.Degs_complete.Length;
            Deg_line_class deg_line;
            deg.Order_by_group_fdr();

            //deg.Degs_complete = deg.Degs_complete.OrderBy(l => l.Patient).ThenBy(l => l.Plate).ThenBy(l => l.Treatment).ThenBy(l => l.FDR).ToArray();
            int degs_above_FDR_count = 0;
            int degs_above_pvalue_count = 0;
            Deg_summary_line_class new_summary_line;
            List<Deg_summary_line_class> new_summary_lines = new List<Deg_summary_line_class>();
            for (int indexD = 0; indexD < degs_length; indexD++)
            {
                deg_line = deg.Degs_complete[indexD];
                if (!deg_line.Cell_line_type.Equals(cell_type)) { throw new Exception(); }
                if ((indexD == 0)
                    || (!deg_line.Equal_group(deg.Degs_complete[indexD - 1])))
                {
                    degs_above_FDR_count = 0;
                    degs_above_pvalue_count = 0;
                }
                if (deg_line.FDR <= Options.FDR_cutoff)
                {
                    degs_above_FDR_count++;
                }
                if (deg_line.PValue <= Options.Pvalue_cutoff)
                {
                    degs_above_pvalue_count++;
                }
                if ((indexD == degs_length - 1)
                    || (!deg_line.Equal_group(deg.Degs_complete[indexD + 1])))
                {
                    new_summary_line = new Deg_summary_line_class();
                    new_summary_line.Plate = (string)deg_line.Plate.Clone();
                    new_summary_line.Significant_degs_based_on_FDR_count = degs_above_FDR_count;
                    new_summary_line.Significant_degs_based_on_pvalue_count = degs_above_pvalue_count;
                    new_summary_line.Pvalue_cutoff = Options.Pvalue_cutoff;
                    new_summary_line.FDR_cutoff = Options.FDR_cutoff;
                    new_summary_line.Treatment_wells_count = deg_line.Treatment_wells_count;
                    new_summary_line.Cell_line_type = (string)cell_type.Clone();
                    new_summary_line.Cell_line = (string)deg_line.Patient.Clone();
                    new_summary_line.Control_wells_count = deg_line.Control_wells_count;
                    new_summary_line.Treatment = (string)deg_line.Treatment.Clone();
                    new_summary_line.Treatment_full_name = (string)deg_line.Treatment_full_name.Clone();
                    new_summary_line.Drug_type = deg_line.Drug_type;
                    new_summary_line.Is_cardiotoxic = (string)deg_line.Is_cardiotoxic.Clone();
                    new_summary_lines.Add(new_summary_line);
                }
            }
            this.Summaries = new_summary_lines.ToArray();
        }
        private void Generate_from_deg_replicate_value_instance(DEG_replicate_value_class deg_replicate_value)
        {
            string cell_type = "Unknown";
            DE_entry_enum entryType = deg_replicate_value.DEG_replicates[0].EntryType;
            Timepoint_enum timepoint = deg_replicate_value.DEG_replicates[0].Timepoint;
            int degs_length = deg_replicate_value.DEG_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;

            deg_replicate_value.Order_by_dataset_plate_cellline_treatment_replicateNo();

            Deg_summary_line_class new_summary_line;
            List<Deg_summary_line_class> new_summary_lines = new List<Deg_summary_line_class>();
            for (int indexD = 0; indexD < degs_length; indexD++)
            {
                deg_replicate_line = deg_replicate_value.DEG_replicates[indexD];
                if (!deg_replicate_line.EntryType.Equals(entryType)) { throw new Exception(); }
                if (!deg_replicate_line.Timepoint.Equals(timepoint)) { throw new Exception(); }
                if ((indexD == 0)
                    || (!deg_replicate_line.Dataset.Equals(deg_replicate_value.DEG_replicates[indexD - 1].Dataset))
                    || (!deg_replicate_line.Plate.Equals(deg_replicate_value.DEG_replicates[indexD - 1].Plate))
                    || (!deg_replicate_line.Cell_line.Equals(deg_replicate_value.DEG_replicates[indexD - 1].Cell_line))
                    || (!deg_replicate_line.Treatment.Equals(deg_replicate_value.DEG_replicates[indexD - 1].Treatment))
                    || (!deg_replicate_line.Replicate_no.Equals(deg_replicate_value.DEG_replicates[indexD - 1].Replicate_no)))
                {
                    new_summary_line = new Deg_summary_line_class();
                    new_summary_line.Plate = (string)deg_replicate_line.Plate.Clone();
                    new_summary_line.Replicate_no = deg_replicate_line.Replicate_no;
                    new_summary_line.Significant_degs_based_on_FDR_count = -1;
                    new_summary_line.Significant_degs_based_on_pvalue_count = -1;
                    new_summary_line.Pvalue_cutoff = Options.Pvalue_cutoff;
                    new_summary_line.FDR_cutoff = Options.FDR_cutoff;
                    new_summary_line.Treatment_wells_count = 0;
                    new_summary_line.Cell_line_type = (string)cell_type.Clone();
                    new_summary_line.Cell_line = (string)deg_replicate_line.Cell_line.Clone();
                    new_summary_line.Control_wells_count = 0;
                    new_summary_line.Treatment = (string)deg_replicate_line.Treatment.Clone();
                    new_summary_line.Treatment_full_name = (string)deg_replicate_line.Treatment_full_name.Clone();
                    new_summary_line.Drug_type = deg_replicate_line.Drug_type;
                    new_summary_lines.Add(new_summary_line);
                }
            }
            this.Summaries = new_summary_lines.ToArray();
        }
        public void Generate(Deg_class deg)
        {
            Generate_from_deg_instance(deg);
        }
        public void Generate(DEG_replicate_value_class deg_replicate_value)
        {
            Generate_from_deg_replicate_value_instance(deg_replicate_value);
        }
        public Dictionary<string, Dictionary<string, bool>> Get_truncatedCellline_drug_dict()
        {
            Dictionary<string, Dictionary<string, bool>> cellline_drug_dict = new Dictionary<string, Dictionary<string, bool>>();
            string truncated_cell_line;
            foreach (Deg_summary_line_class deg_summary_line in this.Summaries)
            {
                truncated_cell_line = deg_summary_line.Cell_line.Split('.')[1].Split('_')[0];
                if (!cellline_drug_dict.ContainsKey(truncated_cell_line))
                {
                    cellline_drug_dict.Add(truncated_cell_line, new Dictionary<string, bool>());
                }
                cellline_drug_dict[truncated_cell_line].Add(deg_summary_line.Treatment, true);
            }
            return cellline_drug_dict;
        }
        public void Write(string subdirectory, string fileName)
        {
            Deg_summary_readWriteOptions_class readWriteOptions = new Deg_summary_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Summaries, readWriteOptions);
        }
        public void Read(string subdirectory, string fileName)
        {
            Deg_summary_readWriteOptions_class readWriteOptions = new Deg_summary_readWriteOptions_class(subdirectory, fileName);
            this.Summaries = ReadWriteClass.ReadRawData_and_FillArray<Deg_summary_line_class>(readWriteOptions);
        }
    }

}
