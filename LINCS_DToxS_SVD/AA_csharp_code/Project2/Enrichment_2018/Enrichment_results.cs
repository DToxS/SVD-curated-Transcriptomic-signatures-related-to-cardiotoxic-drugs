using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Highthroughput_data;
using Common_classes;
using ReadWrite;
using Statistic;

namespace Enrichment
{
    enum Enrichment2018_color_specification_enum { E_m_p_t_y, Regular, Highlight };
    enum Enrichment2018_value_type_enum {  E_m_p_t_y, Minuslog10pvalue, Fractional_rank }
    enum Entity_class_enum {  E_m_p_t_y, DEG, SCP }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Lincs_website_enrichment_line_class
    {
        public Drug_type_enum Drug_type { get; set; }
        public string Drug { get; set; }
        public string Drug_fullName { get; set; }
        public string Cell_line { get; set; }
        public DE_entry_enum Entry_type { get; set; }
        public string UpDownStatus
        {
            get { return Conversion_class.Get_upDown_status_from_entryType(Entry_type); }
            set { Entry_type = Conversion_class.Get_entryType_from_upDown_status(value); }
        }
        public string MBCO_subcellular_process { get; set; }
        public string SCP_cardiotoxicity { get; set; }
        public float SCP_cardiotoxicity_AUC_rank { get; set; }
        public float F1_score_beta { get; set; }
        public string P_value {get;set;}
        public float Enrichment_rank { get; set; }
        public string[] NCBI_gene_symbols { get; set; }
        public string Is_cardiotoxic { get; set; }
        public string SCP_cardiotoxicity_AUC_rank_for_FDA
        {
            get { return Conversion_class.Convert_positive_float_to_string_or_nd(SCP_cardiotoxicity_AUC_rank); }
        }

        public Lincs_website_enrichment_line_class()
        {
            Is_cardiotoxic = "";
        }
        public string ReadWrite_ncbi_gene_symbols
        {
            get { return ReadWriteClass.Get_writeLine_from_array(NCBI_gene_symbols, Lincs_website_enrichment_readWriteOptions.Array_delimiter); }
            set { this.NCBI_gene_symbols = ReadWriteClass.Get_array_from_readLine<string>(value, Lincs_website_enrichment_readWriteOptions.Array_delimiter); }
        }

        public Lincs_website_enrichment_line_class Deep_copy()
        {
            Lincs_website_enrichment_line_class copy = (Lincs_website_enrichment_line_class)this.MemberwiseClone();
            copy.Is_cardiotoxic = (string)this.Is_cardiotoxic.Clone();
            copy.Drug_fullName = (string)this.Drug_fullName.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            copy.SCP_cardiotoxicity = (string)this.SCP_cardiotoxicity.Clone();
            copy.MBCO_subcellular_process = (string)this.MBCO_subcellular_process.Clone();
            copy.NCBI_gene_symbols = Array_class.Deep_copy_string_array(this.NCBI_gene_symbols);
            return copy;
        }
    }
    class Lincs_website_enrichment_input_readOptions : ReadWriteOptions_base
    {
        public Lincs_website_enrichment_input_readOptions(string directory, string fileName, string cellType)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Drug", "Cell_line", "UpDownStatus", "MBCO_subcellular_process", "P_value", "Enrichment_rank", "ReadWrite_ncbi_gene_symbols" };
            this.Key_columnNames = new string[] { "Drug", cellType, Conversion_class.UpDownStatus_columnName, "MBCO subcellular process (SCP)", "Enrichment p-value", "Enrichment rank", "NCBI gene symbols" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }
    class Lincs_website_enrichment_readWriteOptions : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }
        public Lincs_website_enrichment_readWriteOptions(string directory, string fileName, string cellType, bool is_average)
        {
            this.File = directory + fileName;
            if (!is_average)
            {
                this.Key_propertyNames = new string[] { "Drug_fullName", "Is_cardiotoxic", "Drug_type", "Cell_line", "UpDownStatus", "MBCO_subcellular_process", "P_value", "Enrichment_rank", "SCP_cardiotoxicity_AUC_rank_for_FDA", "SCP_cardiotoxicity", "ReadWrite_ncbi_gene_symbols" };
                this.Key_columnNames = new string[] { "Drug", "Is_cardiotoxic", "Drug class", cellType, Conversion_class.UpDownStatus_columnName, "Pathway", "Enrichment p-value", "Enrichment rank", "Pathway F1 score AUC rank", "Cardiotoxicity of pathway",  "Pathway genes" };
            }
            else
            {
                this.Key_propertyNames = new string[] { "Drug_fullName", "Is_cardiotoxic", "Drug_type", "UpDownStatus", "MBCO_subcellular_process", "P_value", "Enrichment_rank", "SCP_cardiotoxicity_AUC_rank_for_FDA", "SCP_cardiotoxicity",  "ReadWrite_ncbi_gene_symbols" };
                this.Key_columnNames = new string[] { "Drug", "Is_cardiotoxic", "Drug class", Conversion_class.UpDownStatus_columnName, "Pathway", "Enrichment p-value", "Enrichment rank", "Pathway F1 score AUC rank", "Cardiotoxicity of pathway",  "Pathway genes" };
            }
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }
    class Lincs_website_enrichment_class
    {
        public Lincs_website_enrichment_line_class[] Enrich { get; set; }

        private void Add_drug_types_and_replace_drugAbbreviations_by_fullNames()
        {
            Deg_drug_legend_class legend = new Deg_drug_legend_class();
            legend.Generate_de_novo();
            legend.Add_missing_cardiotoxicity_from_faers();
            Dictionary<string, Drug_type_enum> drug_drugType_dict = legend.Get_drug_drugType_dictionary();
            Dictionary<string, string> drug_drugFullName_dict = legend.Get_drug_drugFullName_dict();
            Dictionary<string, string> drug_isCardiotoxic_dict = legend.Get_drug_isCardiotoxic_dictionary();
            foreach (Lincs_website_enrichment_line_class enrich_line in this.Enrich)
            {
                enrich_line.Drug_type = drug_drugType_dict[enrich_line.Drug];
                enrich_line.Drug_fullName = (string)drug_drugFullName_dict[enrich_line.Drug].Clone();
                enrich_line.Is_cardiotoxic = (string)drug_isCardiotoxic_dict[enrich_line.Drug].Clone();
            }
        }

        public void Generate_by_reading(string directory, string fileName, string cellType)
        {
            Read(directory, fileName, cellType);
            Add_drug_types_and_replace_drugAbbreviations_by_fullNames();
        }

        public void Add_scp_cardiotoxicity(Lincs_scp_summary_after_roc_class scp_summaries)
        {
            Lincs_scp_summary_valueType_for_selection_enum valueTypeForSelection = scp_summaries.Scp_summaries[0].ValueTypeForSelection;
            scp_summaries.Scp_summaries = scp_summaries.Scp_summaries.OrderBy(l => l.Scp_completeName).ThenBy(l => l.Entry_type).ThenBy(l => l.Association).ToArray();
            this.Enrich = this.Enrich.OrderBy(l => l.MBCO_subcellular_process).ThenBy(l => l.Entry_type).ToArray();
            string association = (string)scp_summaries.Scp_summaries[0].Association.Clone();
            string sideEffect = (string)scp_summaries.Scp_summaries[0].Side_effect.Clone();
            int scp_summaries_length = scp_summaries.Scp_summaries.Length;
            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            int indexSCPSummary_start = 0;
            int indexSCPSummary = 0;
            int enrich_length = this.Enrich.Length;
            int stringCompare;
            Lincs_website_enrichment_line_class enrich_line;
            for (int indexEnrich=0; indexEnrich<enrich_length; indexEnrich++)
            {
                enrich_line = this.Enrich[indexEnrich];
                enrich_line.SCP_cardiotoxicity = "";
                enrich_line.SCP_cardiotoxicity_AUC_rank = float.NaN;
                stringCompare = -2;
                indexSCPSummary = indexSCPSummary_start;
                while ((indexSCPSummary < scp_summaries_length) && (stringCompare < 0))
                {
                    scp_summary_line = scp_summaries.Scp_summaries[indexSCPSummary];
                    if (!scp_summary_line.Side_effect.Equals(sideEffect)) { throw new Exception(); }
                    if (!scp_summary_line.Association.Equals(association)) { throw new Exception(); }
                    if (!scp_summary_line.ValueTypeForSelection.Equals(valueTypeForSelection)) { throw new Exception(); }
                    stringCompare = scp_summary_line.Scp_completeName.CompareTo(enrich_line.MBCO_subcellular_process);
                    if (stringCompare==0)
                    {
                        stringCompare = scp_summary_line.Entry_type.CompareTo(enrich_line.Entry_type);
                    }
                    if (stringCompare<0)
                    {
                        indexSCPSummary_start++;
                        indexSCPSummary = indexSCPSummary_start;
                    }
                    else if (stringCompare==0)
                    {
                        if (  (enrich_line.Is_cardiotoxic.Equals("Yes"))
                            &&(  (enrich_line.Drug_type.Equals(Drug_type_enum.Kinase_inhibitor))
                               ||(enrich_line.Drug_type.Equals(Drug_type_enum.Monoclonal_antibody)))
                           )
                        {
                            enrich_line.SCP_cardiotoxicity = enrich_line.UpDownStatus + " by " + (string)scp_summary_line.Association.Clone();
                            enrich_line.SCP_cardiotoxicity_AUC_rank = scp_summary_line.Selection_rank;
                        }
                    }
                }
            }
        }

        private void Read(string directory, string fileName, string cellType)
        {
            Lincs_website_enrichment_input_readOptions input_readOptions = new Lincs_website_enrichment_input_readOptions(directory, fileName, cellType);
            this.Enrich = ReadWriteClass.ReadRawData_and_FillArray<Lincs_website_enrichment_line_class>(input_readOptions);
        }

        public void Write(string directory, string fileName, string cellType, bool is_average)
        {
            Lincs_website_enrichment_readWriteOptions input_readOptions = new Lincs_website_enrichment_readWriteOptions(directory, fileName, cellType, is_average);
            ReadWriteClass.WriteData(this.Enrich, input_readOptions);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Lincs_website_scp_genes_line_class
    {
        public string Scp { get; set; }
        public string UpDown { get; set; }
        public string Gene { get; set; }
        public string Selected { get; set; }
        public float Selection_rank { get; set; }

        public Lincs_website_scp_genes_line_class()
        {
            Gene = "";
            Selection_rank = -1;
        }

        public Lincs_website_scp_genes_line_class Deep_copy()
        {
            Lincs_website_scp_genes_line_class copy = (Lincs_website_scp_genes_line_class)this.MemberwiseClone();
            copy.Scp = (string)this.Scp.Clone();
            copy.UpDown = (string)this.UpDown.Clone();
            copy.Gene = (string)this.Gene.Clone();
            copy.Selected = (string)this.Selected.Clone();
            return copy;
        }
    }
    class Lincs_website_scp_genes_readWriteOptions_class : ReadWriteOptions_base
    { 
        public Lincs_website_scp_genes_readWriteOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Scp", "UpDown", "Gene", "Selection_rank" };
            this.Key_columnNames = new string[] { "Cardiotoxic pathway", "Up or down", "Gene", "Pathway F1 score AUC rank" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }
    class Lincs_website_scp_genes_class
    {
        public Lincs_website_scp_genes_line_class[] Scp_genes { get; set; }

        public void Generate_from_Lincs_scp_summary_after_roc(Lincs_scp_summary_after_roc_class summary)
        {
            List<Lincs_website_scp_genes_line_class> add = new List<Lincs_website_scp_genes_line_class>();
            Lincs_website_scp_genes_line_class add_line;
            foreach (Lincs_scp_summary_after_roc_line_class summary_line in summary.Scp_summaries)
            {
                add_line = new Lincs_website_scp_genes_line_class();
                add_line.Scp = (string)summary_line.Scp.Clone();
                add_line.Selected = summary_line.Selected;
                add_line.UpDown = (string)summary_line.Up_down_status.Clone();
                add_line.Selection_rank = summary_line.Selection_rank;
                add.Add(add_line);
            }
            this.Scp_genes = add.ToArray();
        }

        public void Add_genes(Ontology_library_class library)
        {
            Dictionary<string,string[]> scp_genes_dict = library.Get_scp_genes_dictionary();
            List<Lincs_website_scp_genes_line_class> new_scp_genes = new List<Lincs_website_scp_genes_line_class>();
            int scp_genes_length = this.Scp_genes.Length;
            Lincs_website_scp_genes_line_class scp_genes_line;
            Lincs_website_scp_genes_line_class new_scp_genes_line;
            string[] genes;
            string gene;
            int genes_length;
            for (int indexS=0; indexS<scp_genes_length; indexS++)
            {
                scp_genes_line = this.Scp_genes[indexS];
                if (scp_genes_dict.ContainsKey(scp_genes_line.Scp))
                {
                    if (!String.IsNullOrEmpty(scp_genes_line.Gene)) { throw new Exception(); }
                    genes = scp_genes_dict[scp_genes_line.Scp];
                    genes_length = genes.Length;
                    for (int indexG=0; indexG<genes_length; indexG++)
                    {
                        gene = genes[indexG];
                        new_scp_genes_line = scp_genes_line.Deep_copy();
                        new_scp_genes_line.Gene = (string)gene.Clone();
                        new_scp_genes.Add(new_scp_genes_line);
                    }
                }
                else
                {
                    new_scp_genes.Add(scp_genes_line);
                }
            }
            this.Scp_genes = new_scp_genes.ToArray();
        }

        public void Write(string directory, string fileName)
        {
            Lincs_website_scp_genes_readWriteOptions_class readWriteOptions = new Lincs_website_scp_genes_readWriteOptions_class(directory, fileName);
            ReadWriteClass.WriteData(this.Scp_genes, readWriteOptions);
        }

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Enrichment2018_results_line_class
    {
        public Ontology_type_enum  Ontology { get; set; }
        public string Scp { get; set; }
        public Timepoint_enum Sample_timepoint { get; set; }
        public DE_entry_enum Sample_entryType { get; set; }
        public string Sample_entryType_string 
        { 
            get {  return Conversion_class.Get_upDown_status_from_entryType(Sample_entryType); } 
            set { this.Sample_entryType = Conversion_class.Get_entryType_from_upDown_status(value); }
        }
        public string Sample_name { get; set; }
        public double Pvalue { get; set; }
        public string Lincs_plate
        {
            get
            {
                string[] splitStrings = this.Sample_name.Split('-');
                string cellline = (string)splitStrings[3].Clone();
                cellline = cellline.Replace(".", " ");
                return cellline;
            }
            set
            {
                string[] splitStrings = this.Sample_name.Split('-');
                splitStrings[3] = value;
                StringBuilder sb = new StringBuilder();
                foreach (string splitString in splitStrings)
                {
                    if (sb.Length > 0) { sb.AppendFormat("-"); }
                    sb.AppendFormat(splitString);
                }
                this.Sample_name = sb.ToString();
            }
        }
        public string Lincs_cell_line
        {
            get
            {
                string[] splitStrings = this.Sample_name.Split('-');
                string cellline = splitStrings[1];//.Replace("_", " ");
                //cellline = cellline.Replace(".", " ");
                return cellline;
            }
            set
            {
                string[] splitStrings = this.Sample_name.Split('-');
                splitStrings[1] = value;
                StringBuilder sb = new StringBuilder();
                foreach (string splitString in splitStrings)
                {
                    if (sb.Length > 0) { sb.AppendFormat("-"); }
                    sb.AppendFormat(splitString);
                }
                this.Sample_name = sb.ToString();
            }
        }
        public string Lincs_drug
        {
            get
            {
                string[] splitStrings = this.Sample_name.Split('-');
                return splitStrings[0];
            }
            set
            {
                string[] splitStrings = this.Sample_name.Split('-');
                splitStrings[0] = value;
                StringBuilder sb = new StringBuilder();
                foreach (string splitString in splitStrings)
                {
                    if (sb.Length>0) { sb.AppendFormat("-"); }
                    sb.AppendFormat(splitString);
                }
                this.Sample_name = sb.ToString();
            }
        }
        public string Lincs_outlier
        {
            get
            {
                string[] splitStrings = this.Sample_name.Split('-');
                return splitStrings[4];
            }
        }
        public float Fractional_rank { get; set; }
        public float Minus_log10_pvalue { get; set; }
        public int Overlap_count { get; set; }
        public int Scp_genes_count { get; set; }
        public int Experimental_genes_count { get; set; }
        public Enrichment2018_color_specification_enum Color_specification { get; set; }
        public int Bg_genes_count { get; set; }
        public string[] Overlap_symbols { get; set; }

        public static bool Check_for_correct_ordering {  get { return Global_class.Check_ordering; } }
        public string ReadWrite_overlap_symbols
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Overlap_symbols, Enrichment2018_results_readWriteOptions_class.Array_delimiter); }
            set { this.Overlap_symbols = ReadWriteClass.Get_array_from_readLine<string>(value, Enrichment2018_results_readWriteOptions_class.Array_delimiter); }
        }

        #region Order
        public static Enrichment2018_results_line_class[] Order_by_pvalue(Enrichment2018_results_line_class[] lines)
        {
            Dictionary<double, List<Enrichment2018_results_line_class>> pvalue_dict = new Dictionary<double, List<Enrichment2018_results_line_class>>();
            int lines_length = lines.Length;
            Enrichment2018_results_line_class line;
            for (int indexL = 0; indexL < lines_length; indexL++)
            {
                line = lines[indexL];
                if (!pvalue_dict.ContainsKey(line.Pvalue))
                {
                    pvalue_dict.Add(line.Pvalue, new List<Enrichment2018_results_line_class>());
                }
                pvalue_dict[line.Pvalue].Add(line);
            }

            double[] pvalues = pvalue_dict.Keys.ToArray();
            double pvalue;
            int pvalues_length = pvalues.Length;
            List<Enrichment2018_results_line_class> ordered_enrichment_results = new List<Enrichment2018_results_line_class>();
            pvalues = pvalues.OrderBy(l => l).ToArray();
            for (int indexP = 0; indexP < pvalues_length; indexP++)
            {
                pvalue = pvalues[indexP];
                ordered_enrichment_results.AddRange(pvalue_dict[pvalue]);
            }

            if (Check_for_correct_ordering)
            {
                #region Check for correct ordering

                int ordered_length = ordered_enrichment_results.Count;
                Enrichment2018_results_line_class previous_line;
                Enrichment2018_results_line_class current_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_enrichment_results[indexO - 1];
                    current_line = ordered_enrichment_results[indexO];
                    if (current_line.Pvalue.CompareTo(previous_line.Pvalue) < 0) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_enrichment_results.ToArray();
        }
        public static Enrichment2018_results_line_class[] Order_by_scp(Enrichment2018_results_line_class[] lines)
        {
            Dictionary<string, List<Enrichment2018_results_line_class>> scp_dict = new Dictionary<string, List<Enrichment2018_results_line_class>>();
            int lines_length = lines.Length;
            Enrichment2018_results_line_class line;
            for (int indexL = 0; indexL < lines_length; indexL++)
            {
                line = lines[indexL];
                if (!scp_dict.ContainsKey(line.Scp))
                {
                    scp_dict.Add(line.Scp, new List<Enrichment2018_results_line_class>());
                }
                scp_dict[line.Scp].Add(line);
            }

            string[] scps = scp_dict.Keys.ToArray();
            string scp;
            int scps_length = scps.Length;
            List<Enrichment2018_results_line_class> ordered_enrichment_results = new List<Enrichment2018_results_line_class>();
            scps = scps.OrderBy(l => l).ToArray();
            for (int indexP = 0; indexP < scps_length; indexP++)
            {
                scp = scps[indexP];
                ordered_enrichment_results.AddRange(scp_dict[scp]);
            }

            if (Check_for_correct_ordering)
            {
                #region Check for correct ordering

                int ordered_length = ordered_enrichment_results.Count;
                Enrichment2018_results_line_class previous_line;
                Enrichment2018_results_line_class current_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_enrichment_results[indexO - 1];
                    current_line = ordered_enrichment_results[indexO];
                    if (current_line.Scp.CompareTo(previous_line.Scp) < 0) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_enrichment_results.ToArray();
        }
        public static Enrichment2018_results_line_class[] Order_by_ontology_sampleName_timepoint_entryType_pvalue(Enrichment2018_results_line_class[] lines)
        {
            Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>>> ontology_sampleName_timepoint_entryType_pvalue_dict = new Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>>>();
            Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>> sampleName_timepoint_entryType_pvalue_dict = new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>>();
            Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>> timepoint_entryType_pvalue_dict = new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>();
            Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>> entryType_pvalue_dict = new Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>();
            Dictionary<double, List<Enrichment2018_results_line_class>> pvalue_dict = new Dictionary<double, List<Enrichment2018_results_line_class>>();
            int lines_length = lines.Length;
            Enrichment2018_results_line_class line;
            for (int indexL = 0; indexL < lines_length; indexL++)
            {
                line = lines[indexL];
                if (!ontology_sampleName_timepoint_entryType_pvalue_dict.ContainsKey(line.Ontology))
                {
                    ontology_sampleName_timepoint_entryType_pvalue_dict.Add(line.Ontology, new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>>());
                }
                if (!ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology].ContainsKey(line.Sample_name))
                {
                    ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology].Add(line.Sample_name, new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>());
                }
                if (!ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology][line.Sample_name].ContainsKey(line.Sample_timepoint))
                {
                    ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology][line.Sample_name].Add(line.Sample_timepoint, new Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>());
                }
                if (!ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint].ContainsKey(line.Sample_entryType))
                {
                    ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint].Add(line.Sample_entryType, new Dictionary<double, List<Enrichment2018_results_line_class>>());
                }
                if (!ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint][line.Sample_entryType].ContainsKey(line.Pvalue))
                {
                    ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint][line.Sample_entryType].Add(line.Pvalue, new List<Enrichment2018_results_line_class>());
                }
                ontology_sampleName_timepoint_entryType_pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint][line.Sample_entryType][line.Pvalue].Add(line);
            }

            Ontology_type_enum[] ontologies = ontology_sampleName_timepoint_entryType_pvalue_dict.Keys.ToArray();
            Ontology_type_enum ontology;
            int ontologies_length = ontologies.Length;
            string[] sampleNames;
            string sampleName;
            int sampleNames_length;
            Timepoint_enum[] timepoints;
            Timepoint_enum timepoint;
            int timepoints_length;
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;
            double[] pvalues;
            double pvalue;
            int pvalues_length;

            ontologies = ontologies.OrderBy(l => l).ToArray();
            List<Enrichment2018_results_line_class> ordered_enrichment_results = new List<Enrichment2018_results_line_class>();
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                ontology = ontologies[indexO];
                sampleName_timepoint_entryType_pvalue_dict = ontology_sampleName_timepoint_entryType_pvalue_dict[ontology];
                sampleNames = sampleName_timepoint_entryType_pvalue_dict.Keys.ToArray();
                sampleNames_length = sampleNames.Length;
                sampleNames = sampleNames.OrderBy(l => l).ToArray();
                for (int indexSN = 0; indexSN < sampleNames_length; indexSN++)
                {
                    sampleName = sampleNames[indexSN];
                    timepoint_entryType_pvalue_dict = sampleName_timepoint_entryType_pvalue_dict[sampleName];
                    timepoints = timepoint_entryType_pvalue_dict.Keys.ToArray();
                    timepoints_length = timepoints.Length;
                    timepoints = timepoints.OrderBy(l => l).ToArray();
                    for (int indexT = 0; indexT < timepoints_length; indexT++)
                    {
                        timepoint = timepoints[indexT];
                        entryType_pvalue_dict = timepoint_entryType_pvalue_dict[timepoint];
                        entryTypes = entryType_pvalue_dict.Keys.ToArray();
                        entryTypes_length = entryTypes.Length;
                        entryTypes = entryTypes.OrderBy(l => l).ToArray();
                        for (int indexE = 0; indexE < entryTypes_length; indexE++)
                        {
                            entryType = entryTypes[indexE];
                            pvalue_dict = entryType_pvalue_dict[entryType];
                            pvalues = pvalue_dict.Keys.ToArray();
                            pvalues_length = pvalues.Length;
                            pvalues = pvalues.OrderBy(l => l).ToArray();
                            for (int indexP = 0; indexP < pvalues_length; indexP++)
                            {
                                pvalue = pvalues[indexP];
                                ordered_enrichment_results.AddRange(pvalue_dict[pvalue]);
                            }
                        }
                    }
                }
            }

            if (Check_for_correct_ordering)
            {
                #region Check for correct ordering
                int ordered_length = ordered_enrichment_results.Count;
                if (ordered_length != lines_length) { throw new Exception(); }
                Enrichment2018_results_line_class previous_line;
                Enrichment2018_results_line_class current_line;
                for (int indexOrder = 1; indexOrder < ordered_length; indexOrder++)
                {
                    previous_line = ordered_enrichment_results[indexOrder - 1];
                    current_line = ordered_enrichment_results[indexOrder];
                    if (current_line.Ontology.CompareTo(previous_line.Ontology) < 0) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Sample_name.CompareTo(previous_line.Sample_name) < 0)) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Sample_name.Equals(previous_line.Sample_name))
                        && (current_line.Sample_timepoint.CompareTo(previous_line.Sample_timepoint) < 0)) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Sample_name.Equals(previous_line.Sample_name))
                        && (current_line.Sample_timepoint.Equals(previous_line.Sample_timepoint))
                        && (current_line.Sample_entryType.CompareTo(previous_line.Sample_entryType) < 0)) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Sample_name.Equals(previous_line.Sample_name))
                        && (current_line.Sample_timepoint.Equals(previous_line.Sample_timepoint))
                        && (current_line.Sample_entryType.Equals(previous_line.Sample_entryType))
                        && (current_line.Pvalue.CompareTo(previous_line.Pvalue) < 0)) { throw new Exception(); }
                }
            }
            #endregion

            return ordered_enrichment_results.ToArray();
        }
        public static Enrichment2018_results_line_class[] Order_by_ontology_sampleName_timepoint_entryType_descendingMinusLog10Pvalue(Enrichment2018_results_line_class[] lines)
        {
            Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>>> ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict = new Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>>>();
            Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>> sampleName_timepoint_entryType_minusLog10Pvalue_dict = new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>>();
            Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>> timepoint_entryType_minusLog10Pvalue_dict = new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>();
            Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>> entryType_minusLog10Pvalue_dict = new Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>();
            Dictionary<double, List<Enrichment2018_results_line_class>> minusLog10Pvalue_dict = new Dictionary<double, List<Enrichment2018_results_line_class>>();
            int lines_length = lines.Length;
            Enrichment2018_results_line_class line;
            for (int indexL = 0; indexL < lines_length; indexL++)
            {
                line = lines[indexL];
                if (!ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict.ContainsKey(line.Ontology))
                {
                    ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict.Add(line.Ontology, new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>>());
                }
                if (!ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology].ContainsKey(line.Sample_name))
                {
                    ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology].Add(line.Sample_name, new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>>());
                }
                if (!ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology][line.Sample_name].ContainsKey(line.Sample_timepoint))
                {
                    ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology][line.Sample_name].Add(line.Sample_timepoint, new Dictionary<DE_entry_enum, Dictionary<double, List<Enrichment2018_results_line_class>>>());
                }
                if (!ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint].ContainsKey(line.Sample_entryType))
                {
                    ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint].Add(line.Sample_entryType, new Dictionary<double, List<Enrichment2018_results_line_class>>());
                }
                if (!ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint][line.Sample_entryType].ContainsKey(line.Minus_log10_pvalue))
                {
                    ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint][line.Sample_entryType].Add(line.Minus_log10_pvalue, new List<Enrichment2018_results_line_class>());
                }
                ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[line.Ontology][line.Sample_name][line.Sample_timepoint][line.Sample_entryType][line.Minus_log10_pvalue].Add(line);
            }

            Ontology_type_enum[] ontologies = ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict.Keys.ToArray();
            Ontology_type_enum ontology;
            int ontologies_length = ontologies.Length;
            string[] sampleNames;
            string sampleName;
            int sampleNames_length;
            Timepoint_enum[] timepoints;
            Timepoint_enum timepoint;
            int timepoints_length;
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;
            double[] minusLog10Pvalues;
            double minusLog10Pvalue;
            int minusLog10Pvalues_length;

            ontologies = ontologies.OrderBy(l => l).ToArray();
            List<Enrichment2018_results_line_class> ordered_enrichment_results = new List<Enrichment2018_results_line_class>();
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                ontology = ontologies[indexO];
                sampleName_timepoint_entryType_minusLog10Pvalue_dict = ontology_sampleName_timepoint_entryType_minusLog10Pvalue_dict[ontology];
                sampleNames = sampleName_timepoint_entryType_minusLog10Pvalue_dict.Keys.ToArray();
                sampleNames_length = sampleNames.Length;
                sampleNames = sampleNames.OrderBy(l => l).ToArray();
                for (int indexSN = 0; indexSN < sampleNames_length; indexSN++)
                {
                    sampleName = sampleNames[indexSN];
                    timepoint_entryType_minusLog10Pvalue_dict = sampleName_timepoint_entryType_minusLog10Pvalue_dict[sampleName];
                    timepoints = timepoint_entryType_minusLog10Pvalue_dict.Keys.ToArray();
                    timepoints_length = timepoints.Length;
                    timepoints = timepoints.OrderBy(l => l).ToArray();
                    for (int indexT = 0; indexT < timepoints_length; indexT++)
                    {
                        timepoint = timepoints[indexT];
                        entryType_minusLog10Pvalue_dict = timepoint_entryType_minusLog10Pvalue_dict[timepoint];
                        entryTypes = entryType_minusLog10Pvalue_dict.Keys.ToArray();
                        entryTypes_length = entryTypes.Length;
                        entryTypes = entryTypes.OrderBy(l => l).ToArray();
                        for (int indexE = 0; indexE < entryTypes_length; indexE++)
                        {
                            entryType = entryTypes[indexE];
                            minusLog10Pvalue_dict = entryType_minusLog10Pvalue_dict[entryType];
                            minusLog10Pvalues = minusLog10Pvalue_dict.Keys.ToArray();
                            minusLog10Pvalues_length = minusLog10Pvalues.Length;
                            minusLog10Pvalues = minusLog10Pvalues.OrderByDescending(l => l).ToArray();
                            for (int indexM = 0; indexM < minusLog10Pvalues_length; indexM++)
                            {
                                minusLog10Pvalue = minusLog10Pvalues[indexM];
                                ordered_enrichment_results.AddRange(minusLog10Pvalue_dict[minusLog10Pvalue]);
                            }
                        }
                    }
                }
            }

            if (Check_for_correct_ordering)
            {
                #region Check for correct ordering
                int ordered_length = ordered_enrichment_results.Count;
                if (ordered_length != lines_length) { throw new Exception(); }
                Enrichment2018_results_line_class previous_line;
                Enrichment2018_results_line_class current_line;
                for (int indexOrder = 1; indexOrder < ordered_length; indexOrder++)
                {
                    previous_line = ordered_enrichment_results[indexOrder - 1];
                    current_line = ordered_enrichment_results[indexOrder];
                    if (current_line.Ontology.CompareTo(previous_line.Ontology) < 0) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Sample_name.CompareTo(previous_line.Sample_name) < 0)) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Sample_name.Equals(previous_line.Sample_name))
                        && (current_line.Sample_timepoint.CompareTo(previous_line.Sample_timepoint) < 0)) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Sample_name.Equals(previous_line.Sample_name))
                        && (current_line.Sample_timepoint.Equals(previous_line.Sample_timepoint))
                        && (current_line.Sample_entryType.CompareTo(previous_line.Sample_entryType) < 0)) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Sample_name.Equals(previous_line.Sample_name))
                        && (current_line.Sample_timepoint.Equals(previous_line.Sample_timepoint))
                        && (current_line.Sample_entryType.Equals(previous_line.Sample_entryType))
                        && (current_line.Minus_log10_pvalue.CompareTo(previous_line.Minus_log10_pvalue) > 0)) { throw new Exception(); }  //descending
                }
            }
            #endregion

            return ordered_enrichment_results.ToArray();
        }
        public static Enrichment2018_results_line_class[] Order_by_ontology_scpName_sampleName_timepoint_entryType(Enrichment2018_results_line_class[] lines)
        {
            Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>>>> ontology_scpName_sampleName_timepoint_entryType_dict = new Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>>> scpName_sampleName_timepoint_entryType_dict = new Dictionary<string, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>>>();
            Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>> sampleName_timepoint_entryType_dict = new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>>();
            Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>> timepoint_entryType_dict = new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>();
            Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>> entryType_dict = new Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>();
            int lines_length = lines.Length;
            Enrichment2018_results_line_class line;
            for (int indexL = 0; indexL < lines_length; indexL++)
            {
                line = lines[indexL];
                if (!ontology_scpName_sampleName_timepoint_entryType_dict.ContainsKey(line.Ontology))
                {
                    ontology_scpName_sampleName_timepoint_entryType_dict.Add(line.Ontology, new Dictionary<string, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>>>());
                }
                if (!ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology].ContainsKey(line.Scp))
                {
                    ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology].Add(line.Scp, new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>>());
                }
                if (!ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology][line.Scp].ContainsKey(line.Sample_name))
                {
                    ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology][line.Scp].Add(line.Sample_name, new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>>());
                }
                if (!ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology][line.Scp][line.Sample_name].ContainsKey(line.Sample_timepoint))
                {
                    ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology][line.Scp][line.Sample_name].Add(line.Sample_timepoint, new Dictionary<DE_entry_enum, List<Enrichment2018_results_line_class>>());
                }
                if (!ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology][line.Scp][line.Sample_name][line.Sample_timepoint].ContainsKey(line.Sample_entryType))
                {
                    ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology][line.Scp][line.Sample_name][line.Sample_timepoint].Add(line.Sample_entryType, new List<Enrichment2018_results_line_class>());
                }
                ontology_scpName_sampleName_timepoint_entryType_dict[line.Ontology][line.Scp][line.Sample_name][line.Sample_timepoint][line.Sample_entryType].Add(line);
            }

            Ontology_type_enum[] ontologies = ontology_scpName_sampleName_timepoint_entryType_dict.Keys.ToArray();
            Ontology_type_enum ontology;
            int ontologies_length = ontologies.Length;
            string[] scpNames;
            string scpName;
            int scpNames_length;
            string[] sampleNames;
            string sampleName;
            int sampleNames_length;
            Timepoint_enum[] timepoints;
            Timepoint_enum timepoint;
            int timepoints_length;
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;

            ontologies = ontologies.OrderBy(l => l).ToArray();
            List<Enrichment2018_results_line_class> ordered_enrichment_results = new List<Enrichment2018_results_line_class>();
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                ontology = ontologies[indexO];
                scpName_sampleName_timepoint_entryType_dict = ontology_scpName_sampleName_timepoint_entryType_dict[ontology];
                scpNames = scpName_sampleName_timepoint_entryType_dict.Keys.ToArray();
                scpNames_length = scpNames.Length;
                scpNames = scpNames.OrderBy(l => l).ToArray();
                for (int indexSCP = 0; indexSCP < scpNames_length; indexSCP++)
                {
                    scpName = scpNames[indexSCP];
                    sampleName_timepoint_entryType_dict = scpName_sampleName_timepoint_entryType_dict[scpName];
                    sampleNames = sampleName_timepoint_entryType_dict.Keys.ToArray();
                    sampleNames_length = sampleNames.Length;
                    sampleNames = sampleNames.OrderBy(l => l).ToArray();
                    for (int indexSN = 0; indexSN < sampleNames_length; indexSN++)
                    {
                        sampleName = sampleNames[indexSN];
                        timepoint_entryType_dict = sampleName_timepoint_entryType_dict[sampleName];
                        timepoints = timepoint_entryType_dict.Keys.ToArray();
                        timepoints_length = timepoints.Length;
                        timepoints = timepoints.OrderBy(l => l).ToArray();
                        for (int indexT = 0; indexT < timepoints_length; indexT++)
                        {
                            timepoint = timepoints[indexT];
                            entryType_dict = timepoint_entryType_dict[timepoint];
                            entryTypes = entryType_dict.Keys.ToArray();
                            entryTypes_length = entryTypes.Length;
                            entryTypes = entryTypes.OrderBy(l => l).ToArray();
                            for (int indexE = 0; indexE < entryTypes_length; indexE++)
                            {
                                entryType = entryTypes[indexE];
                                ordered_enrichment_results.AddRange(entryType_dict[entryType]);
                            }
                        }
                    }
                }
            }

            if (Check_for_correct_ordering)
            {
                #region Check for correct ordering
                int ordered_length = ordered_enrichment_results.Count;
                if (ordered_length != lines_length) { throw new Exception(); }
                Enrichment2018_results_line_class previous_line;
                Enrichment2018_results_line_class current_line;
                for (int indexOrder = 1; indexOrder < ordered_length; indexOrder++)
                {
                    previous_line = ordered_enrichment_results[indexOrder - 1];
                    current_line = ordered_enrichment_results[indexOrder];
                    if (current_line.Ontology.CompareTo(previous_line.Ontology) < 0) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Scp.CompareTo(previous_line.Scp) < 0)) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Scp.Equals(previous_line.Scp))
                        && (current_line.Sample_name.CompareTo(previous_line.Sample_name) < 0)) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Scp.Equals(previous_line.Scp))
                        && (current_line.Sample_name.Equals(previous_line.Sample_name))
                        && (current_line.Sample_timepoint.Equals(previous_line.Sample_timepoint))) { throw new Exception(); }
                    if ((current_line.Ontology.Equals(previous_line.Ontology))
                        && (current_line.Scp.Equals(previous_line.Scp))
                        && (current_line.Sample_name.Equals(previous_line.Sample_name))
                        && (current_line.Sample_timepoint.Equals(previous_line.Sample_timepoint))
                        && (current_line.Sample_entryType.CompareTo(previous_line.Sample_entryType) < 0)) { throw new Exception(); }
                }
            }
            #endregion

            return ordered_enrichment_results.ToArray();
        }
        #endregion

        public Enrichment2018_results_line_class()
        {
            this.Fractional_rank = -1;
            Color_specification = Enrichment2018_color_specification_enum.Regular;
        }
        public Enrichment2018_results_line_class Deep_copy()
        {
            Enrichment2018_results_line_class copy = (Enrichment2018_results_line_class)this.MemberwiseClone();
            copy.Scp = (string)this.Scp.Clone();
            copy.Sample_name = (string)this.Sample_name.Clone();
            int symbols_length = this.Overlap_symbols.Length;
            copy.Overlap_symbols = new string[symbols_length];
            for (int indexS=0; indexS<symbols_length; indexS++)
            {
                copy.Overlap_symbols[indexS] = (string)this.Overlap_symbols[indexS].Clone();
            }
            return copy;
        }
    }

    class Enrichment2018_results_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Enrichment2018_results_readWriteOptions_class(string completeDirectory_or_subdirectory, string fileName)
        {
            string directory = "";
            if (completeDirectory_or_subdirectory.IndexOf(":")!=-1)
            {
                directory = completeDirectory_or_subdirectory;
            }
            else
            {
                directory = Global_directory_class.Results_directory + completeDirectory_or_subdirectory;
            }
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Ontology", "Sample_timepoint", "Sample_entryType", "Sample_name", "Scp", "Pvalue", "Minus_log10_pvalue", "Fractional_rank", "Overlap_count", "Scp_genes_count", "Experimental_genes_count", "ReadWrite_overlap_symbols" };
            //this.Key_propertyNames = new string[] { "Ontology", "Sample_name", "Scp", "Pvalue", "Minus_log10_pvalue", "Ordinal_rank", "Overlap_count", "Scp_genes_count", "Experimental_genes_count", "ReadWrite_overlap_symbols" };
            this.Key_propertyNames = new string[] { "Ontology", "Sample_entryType", "Sample_name", "Scp", "Pvalue", "Minus_log10_pvalue", "Fractional_rank", "Overlap_count", "Scp_genes_count", "Experimental_genes_count", "ReadWrite_overlap_symbols" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Enrichment2018_results_for_website_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ','; } }

        public Enrichment2018_results_for_website_readWriteOptions_class(string subdirectory, string fileName, string cellType)
        {
            string directory = Global_directory_class.Results_directory + subdirectory;
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Lincs_drug", "Lincs_cell_line", "Sample_entryType_string", "Scp", "Pvalue", "Fractional_rank", "ReadWrite_overlap_symbols" };
            this.Key_columnNames = new string[] { "Drug", cellType, Conversion_class.UpDownStatus_columnName, "MBCO subcellular process (SCP)", "Enrichment p-value", "Enrichment rank", Lincs_website_conversion_class.Label_overlap_gene_symbols }; //C-path
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Enrichment2018_results_readOptions_for_r_class : ReadWriteOptions_base
    {
        public Enrichment2018_results_readOptions_for_r_class(string completeDirectory_or_subdirectory, string fileName)
        {
            string directory = "";
            if (completeDirectory_or_subdirectory.IndexOf(":")!=-1)
            {
                directory = completeDirectory_or_subdirectory;
            }
            else
            {
                directory = Global_directory_class.Results_directory + completeDirectory_or_subdirectory;
            }
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Ontology", "Sample_timepoint","Sample_entryType", "Sample_name", "Scp", "Experimental_genes_count", "Scp_genes_count", "Bg_genes_count", "Overlap_count", "Pvalue", "Minus_log10_pvalue", "Fractional_rank", "ReadWrite_overlap_symbols" };
            this.Key_propertyNames = new string[] { "Ontology", "Sample_timepoint", "Sample_entryType", "Sample_name", "Scp", "Experimental_genes_count", "Scp_genes_count", "Bg_genes_count", "Overlap_count", "Pvalue", "Minus_log10_pvalue", "Fractional_rank", "ReadWrite_overlap_symbols" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Enrichment2018_results_class
    {
        public Enrichment2018_results_line_class[] Enrichment_results { get; set; }

        #region Order
        public void Order_by_pvalue()
        {
            this.Enrichment_results = Enrichment2018_results_line_class.Order_by_pvalue(this.Enrichment_results);
        }

        public void Order_by_group()
        {
            this.Enrichment_results = Enrichment2018_results_line_class.Order_by_pvalue(this.Enrichment_results);
        }

        public void Order_by_ontology_sampleName_timepoint_entryType_pvalue()
        {
            this.Enrichment_results = Enrichment2018_results_line_class.Order_by_ontology_sampleName_timepoint_entryType_pvalue(this.Enrichment_results);
        }

        public void Order_by_ontology_scpName_sampleName_timepoint_entryType()
        {
            this.Enrichment_results = Enrichment2018_results_line_class.Order_by_ontology_scpName_sampleName_timepoint_entryType(this.Enrichment_results);
        }
        #endregion

        public Enrichment2018_results_class()
        {
            this.Enrichment_results = new Enrichment2018_results_line_class[0];
        }

        private void Add_to_array(Enrichment2018_results_line_class[] add_enrichment_results)
        {
            int this_enrichment_results_length = this.Enrichment_results.Length;
            int add_enrichment_results_length = add_enrichment_results.Length;
            int new_enrichment_results_length = this_enrichment_results_length + add_enrichment_results_length;
            Enrichment2018_results_line_class[] new_enrichment_results = new Enrichment2018_results_line_class[new_enrichment_results_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_enrichment_results_length; indexThis++)
            {
                indexNew++;
                new_enrichment_results[indexNew] = this.Enrichment_results[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_enrichment_results_length; indexAdd++)
            {
                indexNew++;
                new_enrichment_results[indexNew] = add_enrichment_results[indexAdd];
            }
            this.Enrichment_results = new_enrichment_results;
        }

        private void Check_if_not_empty()
        {
            if (Enrichment_results.Length==0) { throw new Exception(); }
        }
        public void Check_for_duplicates()
        {
            int enrichment_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class enrichment_line;
            Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Enrichment2018_results_line_class>>>> sampleName_timepoint_entryType_scp_dic = new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Enrichment2018_results_line_class>>>>();
            for (int indexE=0; indexE<enrichment_length;indexE++)
            {
                enrichment_line = this.Enrichment_results[indexE];
                if (!sampleName_timepoint_entryType_scp_dic.ContainsKey(enrichment_line.Sample_name))
                {
                    sampleName_timepoint_entryType_scp_dic.Add(enrichment_line.Sample_name, new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, Dictionary<string, Enrichment2018_results_line_class>>>());
                }
                if (!sampleName_timepoint_entryType_scp_dic[enrichment_line.Sample_name].ContainsKey(enrichment_line.Sample_timepoint))
                {
                    sampleName_timepoint_entryType_scp_dic[enrichment_line.Sample_name].Add(enrichment_line.Sample_timepoint, new Dictionary<DE_entry_enum, Dictionary<string, Enrichment2018_results_line_class>>());
                }
                if (!sampleName_timepoint_entryType_scp_dic[enrichment_line.Sample_name][enrichment_line.Sample_timepoint].ContainsKey(enrichment_line.Sample_entryType))
                {
                    sampleName_timepoint_entryType_scp_dic[enrichment_line.Sample_name][enrichment_line.Sample_timepoint].Add(enrichment_line.Sample_entryType, new Dictionary<string, Enrichment2018_results_line_class>());
                }
                sampleName_timepoint_entryType_scp_dic[enrichment_line.Sample_name][enrichment_line.Sample_timepoint][enrichment_line.Sample_entryType].Add(enrichment_line.Scp,enrichment_line);
            }
        }
        public void Check_if_all_lines_have_non_empty_entryTypes()
        {
            foreach (Enrichment2018_results_line_class enrichment_results_line in this.Enrichment_results)
            {
                if (enrichment_results_line.Sample_entryType.Equals(DE_entry_enum.E_m_p_t_y))
                {
                    throw new Exception();
                }
            }
        }
        public Ontology_type_enum Get_ontology_and_check_if_only_one()
        {
            Ontology_type_enum ontology = this.Enrichment_results[0].Ontology;
            foreach (Enrichment2018_results_line_class enrichment_results_line in Enrichment_results)
            {
                if (!ontology.Equals(enrichment_results_line.Ontology))
                {
                    throw new Exception();
                }
            }
            return ontology;
        }
        public void Calculate_fractional_ranks_for_scps_based_on_selected_valuetype(Enrichment2018_value_type_enum selected_valueType)
        {
            int enrichment_results_length = Enrichment_results.Length;
            Enrichment2018_results_line_class results_line;
            Enrichment2018_results_line_class inner_results_line;
            int firstIndex_sameCondition = -1;
            switch (selected_valueType)
            {
                case Enrichment2018_value_type_enum.Minuslog10pvalue:
                    this.Enrichment_results = Enrichment2018_results_line_class.Order_by_ontology_sampleName_timepoint_entryType_descendingMinusLog10Pvalue(this.Enrichment_results);
                    break;
                default:
                    throw new Exception();
            }
            //this.Enrichment_results = this.Enrichment_results.OrderBy(l => l.Ontology).ThenBy(l => l.Sample_timepoint).ThenBy(l => l.Sample_entryType).ThenBy(l => l.Sample_name).ThenByDescending(l => l.Minus_log10_pvalue).ToArray();
            int ordinal_rank = 0;
            List<float> current_ordinal_ranks = new List<float>();
            float fractional_rank;
            bool do_calculate_rank = false;
            for (int indexE = 0; indexE < enrichment_results_length; indexE++)
            {
                results_line = this.Enrichment_results[indexE];
                if ((indexE == 0)
                    || (!results_line.Ontology.Equals(this.Enrichment_results[indexE - 1].Ontology))
                    || (!results_line.Sample_timepoint.Equals(this.Enrichment_results[indexE - 1].Sample_timepoint))
                    || (!results_line.Sample_entryType.Equals(this.Enrichment_results[indexE - 1].Sample_entryType))
                    || (!results_line.Sample_name.Equals(this.Enrichment_results[indexE - 1].Sample_name)))
                {
                    ordinal_rank = 0;
                }
                switch (selected_valueType)
                {
                    case Enrichment2018_value_type_enum.Minuslog10pvalue:
                        if ((indexE == 0)
                            || (!results_line.Ontology.Equals(this.Enrichment_results[indexE - 1].Ontology))
                            || (!results_line.Sample_timepoint.Equals(this.Enrichment_results[indexE - 1].Sample_timepoint))
                            || (!results_line.Sample_entryType.Equals(this.Enrichment_results[indexE - 1].Sample_entryType))
                            || (!results_line.Sample_name.Equals(this.Enrichment_results[indexE - 1].Sample_name))
                            || (!results_line.Minus_log10_pvalue.Equals(this.Enrichment_results[indexE - 1].Minus_log10_pvalue)))
                        {
                            current_ordinal_ranks.Clear();
                            firstIndex_sameCondition = indexE;
                        }
                        break;
                    default:
                        throw new Exception();
                }
                ordinal_rank++;
                current_ordinal_ranks.Add(ordinal_rank);
                do_calculate_rank = false;
                switch (selected_valueType)
                {
                    case Enrichment2018_value_type_enum.Minuslog10pvalue:
                        if ((indexE == enrichment_results_length - 1)
                            || (!results_line.Ontology.Equals(this.Enrichment_results[indexE + 1].Ontology))
                            || (!results_line.Sample_timepoint.Equals(this.Enrichment_results[indexE + 1].Sample_timepoint))
                            || (!results_line.Sample_entryType.Equals(this.Enrichment_results[indexE + 1].Sample_entryType))
                            || (!results_line.Sample_name.Equals(this.Enrichment_results[indexE + 1].Sample_name))
                            || (!results_line.Minus_log10_pvalue.Equals(this.Enrichment_results[indexE + 1].Minus_log10_pvalue)))
                        {
                            do_calculate_rank = true;
                        }
                        break;
                    default:
                        throw new Exception();
                }
                if (do_calculate_rank)
                { 
                    if (current_ordinal_ranks.Count > 1)
                    {
                        fractional_rank = Math_class.Get_average(current_ordinal_ranks.ToArray());
                        for (int indexInner = firstIndex_sameCondition; indexInner<=indexE;indexInner++)
                        {
                            inner_results_line = this.Enrichment_results[indexInner];
                            inner_results_line.Fractional_rank = fractional_rank;
                        }
                    }
                    else if (current_ordinal_ranks.Count==1)
                    {
                        if (firstIndex_sameCondition!=indexE) { throw new Exception(); }
                        fractional_rank = current_ordinal_ranks[0];
                        results_line.Fractional_rank = fractional_rank;
                    }
                    else { throw new Exception(); }
                }
            }
        }
        public void Calculate_fractional_ranks_for_scps_based_on_descending_absolute_minusLog10Pvalue()
        {
            int enrichment_results_length = Enrichment_results.Length;
            Enrichment2018_results_line_class results_line;
            Enrichment2018_results_line_class inner_results_line;
            int firstIndex_sameCondition = -1;
            this.Enrichment_results = Enrichment2018_results_line_class.Order_by_ontology_sampleName_timepoint_entryType_descendingMinusLog10Pvalue(this.Enrichment_results);
            //this.Enrichment_results = this.Enrichment_results.OrderBy(l => l.Ontology).ThenBy(l => l.Sample_timepoint).ThenBy(l => l.Sample_entryType).ThenBy(l => l.Sample_name).ThenByDescending(l => l.Minus_log10_pvalue).ToArray();
            int ordinal_rank = 0;
            List<float> current_ordinal_ranks = new List<float>();
            float fractional_rank;
            for (int indexE = 0; indexE < enrichment_results_length; indexE++)
            {
                results_line = this.Enrichment_results[indexE];
                if ((indexE == 0)
                    || (!results_line.Ontology.Equals(this.Enrichment_results[indexE - 1].Ontology))
                    || (!results_line.Sample_timepoint.Equals(this.Enrichment_results[indexE - 1].Sample_timepoint))
                    || (!results_line.Sample_entryType.Equals(this.Enrichment_results[indexE - 1].Sample_entryType))
                    || (!results_line.Sample_name.Equals(this.Enrichment_results[indexE - 1].Sample_name)))
                {
                    ordinal_rank = 0;
                }
                if ((indexE == 0)
                    || (!results_line.Ontology.Equals(this.Enrichment_results[indexE - 1].Ontology))
                    || (!results_line.Sample_timepoint.Equals(this.Enrichment_results[indexE - 1].Sample_timepoint))
                    || (!results_line.Sample_entryType.Equals(this.Enrichment_results[indexE - 1].Sample_entryType))
                    || (!results_line.Sample_name.Equals(this.Enrichment_results[indexE - 1].Sample_name))
                    || (!results_line.Minus_log10_pvalue.Equals(this.Enrichment_results[indexE - 1].Minus_log10_pvalue)))
                {
                    current_ordinal_ranks.Clear();
                    firstIndex_sameCondition = indexE;
                }
                ordinal_rank++;
                current_ordinal_ranks.Add(ordinal_rank);
                if ((indexE == enrichment_results_length - 1)
                    || (!results_line.Ontology.Equals(this.Enrichment_results[indexE + 1].Ontology))
                    || (!results_line.Sample_timepoint.Equals(this.Enrichment_results[indexE + 1].Sample_timepoint))
                    || (!results_line.Sample_entryType.Equals(this.Enrichment_results[indexE + 1].Sample_entryType))
                    || (!results_line.Sample_name.Equals(this.Enrichment_results[indexE + 1].Sample_name))
                    || (!results_line.Minus_log10_pvalue.Equals(this.Enrichment_results[indexE + 1].Minus_log10_pvalue)))
                {
                    if (current_ordinal_ranks.Count > 1)
                    {
                        fractional_rank = Math_class.Get_average(current_ordinal_ranks.ToArray());
                        for (int indexInner = firstIndex_sameCondition; indexInner <= indexE; indexInner++)
                        {
                            inner_results_line = this.Enrichment_results[indexInner];
                            inner_results_line.Fractional_rank = fractional_rank;
                        }
                    }
                    else if (current_ordinal_ranks.Count == 1)
                    {
                        if (firstIndex_sameCondition != indexE) { throw new Exception(); }
                        fractional_rank = current_ordinal_ranks[0];
                        results_line.Fractional_rank = fractional_rank;
                    }
                    else { throw new Exception(); }
                }
            }
        }

        #region Get conditions, groups, entry types
        public Ontology_type_enum[] Get_all_unique_ontologies()
        {
            Dictionary<Ontology_type_enum, bool> ontology_dict = new Dictionary<Ontology_type_enum, bool>();
            foreach (Enrichment2018_results_line_class enrichment_line in this.Enrichment_results)
            {
                if (!ontology_dict.ContainsKey(enrichment_line.Ontology))
                {
                    ontology_dict.Add(enrichment_line.Ontology, true);
                }
            }
            return ontology_dict.Keys.ToArray();
        }
        #endregion

        #region Highlight selected SCPs
        public void Highlight_selected_scps(string[] highlight_scps)
        {
            highlight_scps = highlight_scps.Distinct().OrderBy(l => l).ToArray();
            string highlight_scp;
            int indexHighlight = 0;
            int highlight_scps_length = highlight_scps.Length;
            this.Enrichment_results = this.Enrichment_results.OrderBy(l => l.Scp).ToArray();
            Enrichment2018_results_line_class enrichment_resuls_line;
            int enrichment_length = this.Enrichment_results.Length;
            int stringCompare = -2;
            for (int indexE=0; indexE < enrichment_length; indexE++)
            {
                enrichment_resuls_line = this.Enrichment_results[indexE];
                stringCompare = -2;
                while ((indexHighlight<highlight_scps_length)&&(stringCompare<0))
                {
                    highlight_scp = highlight_scps[indexHighlight];
                    stringCompare = highlight_scp.CompareTo(enrichment_resuls_line.Scp);
                    if (stringCompare<0)
                    {
                        indexHighlight++;
                    }
                    else if (stringCompare==0)
                    {
                        enrichment_resuls_line.Color_specification = Enrichment2018_color_specification_enum.Highlight;
                    }
                }
            }
        }
        #endregion

        public string[] Get_all_distinct_ordered_lincs_drugs()
        {
            int enrichment_results_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class enrichment_results_line;
            List<string> drugs = new List<string>();
            for (int indexE = 0; indexE < enrichment_results_length; indexE++)
            {
                enrichment_results_line = this.Enrichment_results[indexE];
                drugs.Add(enrichment_results_line.Lincs_drug);
            }
            return drugs.Distinct().OrderBy(l => l).ToArray();
        }

        #region Keep
        public void Keep_only_lincs_drugs(params string[] keep_lincsDrugs)
        {
            Dictionary<string, bool> keep_lincsDrugs_dict = new Dictionary<string, bool>();
            foreach (string keep_lincsDrug in keep_lincsDrugs)
            {
                keep_lincsDrugs_dict.Add(keep_lincsDrug, true);
            }
            int enrichment_length = Enrichment_results.Length;
            Enrichment2018_results_line_class enrichment_results_line;
            List<Enrichment2018_results_line_class> keep = new List<Enrichment2018_results_line_class>();
            for (int indexE = 0; indexE < enrichment_length; indexE++)
            {
                enrichment_results_line = this.Enrichment_results[indexE];
                if (keep_lincsDrugs_dict.ContainsKey(enrichment_results_line.Lincs_drug))
                {
                    keep.Add(enrichment_results_line);

                }
            }
            this.Enrichment_results = keep.ToArray();
            Check_if_not_empty();
        }
        public void Keep_only_indicated_ontologies_if_existent(params Ontology_type_enum[] keep_ontolgies)
        {
            Dictionary<Ontology_type_enum, bool> keep_ontologies_kept_dict = new Dictionary<Ontology_type_enum, bool>();
            foreach (Ontology_type_enum ontology in keep_ontolgies)
            {
                if (!keep_ontologies_kept_dict.ContainsKey(ontology))
                {
                    keep_ontologies_kept_dict.Add(ontology, false);
                }
            }

            int enrichment_length = Enrichment_results.Length;
            Enrichment2018_results_line_class enrichment_results_line;
            List<Enrichment2018_results_line_class> keep = new List<Enrichment2018_results_line_class>();
            for (int indexE = 0; indexE < enrichment_length; indexE++)
            {
                enrichment_results_line = this.Enrichment_results[indexE];
                if (keep_ontologies_kept_dict.ContainsKey(enrichment_results_line.Ontology))
                {
                    keep_ontologies_kept_dict[enrichment_results_line.Ontology] = true;
                    keep.Add(enrichment_results_line);
                }
            }
            this.Enrichment_results = keep.ToArray();
        }
        public void Remove_indicated_scps(params string[] remove_scps)
        {
            remove_scps = remove_scps.Distinct().OrderBy(l => l).ToArray();
            string remove_scp;
            int remove_scps_length = remove_scps.Length;
            int indexRemove = 0;
            int stringCompare = -2;

            this.Enrichment_results = this.Enrichment_results.OrderBy(l => l.Scp).ToArray();

            int enrichment_length = Enrichment_results.Length;
            Enrichment2018_results_line_class enrichment_results_line;
            List<Enrichment2018_results_line_class> keep = new List<Enrichment2018_results_line_class>();
            for (int indexE = 0; indexE < enrichment_length; indexE++)
            {
                enrichment_results_line = this.Enrichment_results[indexE];
                stringCompare = -2;
                while ((indexRemove < remove_scps_length) && (stringCompare < 0))
                {
                    remove_scp = remove_scps[indexRemove];
                    stringCompare = remove_scp.CompareTo(enrichment_results_line.Scp);
                    if (stringCompare < 0)
                    {
                        indexRemove++;
                    }
                }
                if (stringCompare != 0)
                {
                    keep.Add(enrichment_results_line);
                }
            }
            this.Enrichment_results = keep.ToArray();
            //Check_if_not_empty();
        }
        public void Keep_only_top_x_ranked_scps_per_condition(float top_x)
        {
            this.Order_by_ontology_sampleName_timepoint_entryType_pvalue();
            int results_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class results_line;
            List<Enrichment2018_results_line_class> keep_enrichment = new List<Enrichment2018_results_line_class>();
            int kept_per_condition = 0;
            for (int indexE = 0; indexE < results_length; indexE++)
            {
                results_line = this.Enrichment_results[indexE];
                if ((indexE == 0)
                    || (!results_line.Ontology.Equals(this.Enrichment_results[indexE - 1].Ontology))
                    || (!results_line.Sample_entryType.Equals(this.Enrichment_results[indexE - 1].Sample_entryType))
                    || (!results_line.Sample_name.Equals(this.Enrichment_results[indexE - 1].Sample_name))
                    || (!results_line.Sample_timepoint.Equals(this.Enrichment_results[indexE - 1].Sample_timepoint)))
                {
                    kept_per_condition = 0;
                }
                if (kept_per_condition < top_x)
                {
                    kept_per_condition++;
                    keep_enrichment.Add(results_line);
                }
            }
            this.Enrichment_results = keep_enrichment.ToArray();
        }
        public void Keep_only_top_x_ranked_scps_based_on_existing_fractional_ranks(Dictionary<Ontology_type_enum,float> ontology_maxFractionalRank_dict)
        {
            int results_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class results_line;
            List<Enrichment2018_results_line_class> keep_enrichment = new List<Enrichment2018_results_line_class>();
            List<Enrichment2018_results_line_class> remove_enrichment = new List<Enrichment2018_results_line_class>();
            Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, bool>>>> ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict = new Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, bool>>>>();
            Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, int>>>> ontology_sampleName_timepoint_entryType_keptSCPsCount_dict = new Dictionary<Ontology_type_enum, Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, int>>>>();
            for (int indexE = 0; indexE < results_length; indexE++)
            {
                results_line = this.Enrichment_results[indexE];
                if (results_line.Fractional_rank == -1) { throw new Exception(); }
                if (results_line.Fractional_rank <= ontology_maxFractionalRank_dict[results_line.Ontology])
                {
                    keep_enrichment.Add(results_line);
                    if (!float.IsInfinity(results_line.Minus_log10_pvalue))
                    {
                        if (!ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict.ContainsKey(results_line.Ontology))
                        {
                            ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict.Add(results_line.Ontology, new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, bool>>>());
                        }
                        if (!ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict[results_line.Ontology].ContainsKey(results_line.Sample_name))
                        {
                            ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict[results_line.Ontology].Add(results_line.Sample_name, new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, bool>>());
                        }
                        if (!ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict[results_line.Ontology][results_line.Sample_name].ContainsKey(results_line.Sample_timepoint))
                        {
                            ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict[results_line.Ontology][results_line.Sample_name].Add(results_line.Sample_timepoint, new Dictionary<DE_entry_enum, bool>());
                        }
                        if (!ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict[results_line.Ontology][results_line.Sample_name][results_line.Sample_timepoint].ContainsKey(results_line.Sample_entryType))
                        {
                            ontology_sampleName_timepoint_entryType_atLeastOneKeptMinusLog10PvalueNotInfinity_dict[results_line.Ontology][results_line.Sample_name][results_line.Sample_timepoint].Add(results_line.Sample_entryType, true);
                        }
                    }
                    if (!ontology_sampleName_timepoint_entryType_keptSCPsCount_dict.ContainsKey(results_line.Ontology))
                    {
                        ontology_sampleName_timepoint_entryType_keptSCPsCount_dict.Add(results_line.Ontology, new Dictionary<string, Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, int>>>());
                    }
                    if (!ontology_sampleName_timepoint_entryType_keptSCPsCount_dict[results_line.Ontology].ContainsKey(results_line.Sample_name))
                    {
                        ontology_sampleName_timepoint_entryType_keptSCPsCount_dict[results_line.Ontology].Add(results_line.Sample_name, new Dictionary<Timepoint_enum, Dictionary<DE_entry_enum, int>>());
                    }
                    if (!ontology_sampleName_timepoint_entryType_keptSCPsCount_dict[results_line.Ontology][results_line.Sample_name].ContainsKey(results_line.Sample_timepoint))
                    {
                        ontology_sampleName_timepoint_entryType_keptSCPsCount_dict[results_line.Ontology][results_line.Sample_name].Add(results_line.Sample_timepoint, new Dictionary<DE_entry_enum, int>());
                    }
                    if (!ontology_sampleName_timepoint_entryType_keptSCPsCount_dict[results_line.Ontology][results_line.Sample_name][results_line.Sample_timepoint].ContainsKey(results_line.Sample_entryType))
                    {
                        ontology_sampleName_timepoint_entryType_keptSCPsCount_dict[results_line.Ontology][results_line.Sample_name][results_line.Sample_timepoint].Add(results_line.Sample_entryType, 0);
                    }
                    ontology_sampleName_timepoint_entryType_keptSCPsCount_dict[results_line.Ontology][results_line.Sample_name][results_line.Sample_timepoint][results_line.Sample_entryType]++;
                }
            }
            this.Enrichment_results = keep_enrichment.ToArray();
        }
        public void Keep_only_top_x_ranked_scps_based_on_existing_fractional_ranks_specified_for_each_ontology(Dictionary<Ontology_type_enum,int> ontology_maxRank_dict)
        {
            int results_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class results_line;
            List<Enrichment2018_results_line_class> keep_enrichment = new List<Enrichment2018_results_line_class>();
            for (int indexE = 0; indexE < results_length; indexE++)
            {
                results_line = this.Enrichment_results[indexE];
                if (results_line.Fractional_rank == -1) { throw new Exception(); }
                if (results_line.Fractional_rank<=ontology_maxRank_dict[results_line.Ontology])
                {
                    keep_enrichment.Add(results_line);
                }
            }
            this.Enrichment_results = keep_enrichment.ToArray();
        }
        public void Keep_only_scps_with_maximium_pvalue(float max_pvalue)
        {
            List<Enrichment2018_results_line_class> keep = new List<Enrichment2018_results_line_class>();
            foreach (Enrichment2018_results_line_class enrichment_results_line in this.Enrichment_results)
            {
                if (enrichment_results_line.Pvalue <= max_pvalue)
                {
                    keep.Add(enrichment_results_line);
                }
            }
            this.Enrichment_results = keep.ToArray();
        }
        #endregion

        #region Add other
        public void Add_other(Enrichment2018_results_class add_enrich)
        {
            this.Add_to_array(add_enrich.Enrichment_results);
        }
        #endregion

        #region Add missing scps
        public void Add_missing_scps_that_were_detected_at_least_once_with_pvalue_one_and_indicated_rank(float rank, DE_class de, params DE_entry_enum[] entryTypes_that_are_always_added)
        {
            Dictionary<string, Dictionary<DE_entry_enum, Dictionary<Timepoint_enum, bool>>> sampleName_entryType_timepoint_dict = new Dictionary<string, Dictionary<DE_entry_enum, Dictionary<Timepoint_enum, bool>>>();
            Dictionary<DE_entry_enum, Dictionary<Timepoint_enum, bool>> entryType_timepoint_dict = new Dictionary<DE_entry_enum, Dictionary<Timepoint_enum, bool>>();
            Dictionary<Timepoint_enum, bool> timepoint_dict = new  Dictionary<Timepoint_enum, bool>();
            int enrichment_results_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class enrichment_results_line;
            Enrichment2018_results_line_class add_enrichment_results_line;
            List<Enrichment2018_results_line_class> add_enrichment_result_lines = new List<Enrichment2018_results_line_class>();
            DE_column_characterization_class colChar = de.ColChar;
            DE_column_characterization_line_class colChar_line;
            int columns_length = colChar.Columns.Count;
            string full_column_name;
            for (int indexC =0; indexC<columns_length;indexC++)
            {
                colChar_line = colChar.Columns[indexC];
                full_column_name = colChar_line.Get_full_column_name();
                if (!sampleName_entryType_timepoint_dict.ContainsKey(full_column_name))
                { sampleName_entryType_timepoint_dict.Add(full_column_name, new Dictionary<DE_entry_enum, Dictionary<Timepoint_enum, bool>>()); }
                if (!sampleName_entryType_timepoint_dict[full_column_name].ContainsKey(colChar_line.EntryType))
                { sampleName_entryType_timepoint_dict[full_column_name].Add(colChar_line.EntryType, new Dictionary<Timepoint_enum, bool>()); }
                if (!sampleName_entryType_timepoint_dict[full_column_name][colChar_line.EntryType].ContainsKey(colChar_line.Timepoint))
                { sampleName_entryType_timepoint_dict[full_column_name][colChar_line.EntryType].Add(colChar_line.Timepoint, true); }
                foreach (DE_entry_enum always_add_entryType in entryTypes_that_are_always_added)
                {
                    if (!sampleName_entryType_timepoint_dict[full_column_name].ContainsKey(always_add_entryType))
                    { sampleName_entryType_timepoint_dict[full_column_name].Add(always_add_entryType, new Dictionary<Timepoint_enum, bool>()); }
                    if (!sampleName_entryType_timepoint_dict[full_column_name][always_add_entryType].ContainsKey(colChar_line.Timepoint))
                    { sampleName_entryType_timepoint_dict[full_column_name][always_add_entryType].Add(colChar_line.Timepoint, true); }
                }
            }

            for (int indexE = 0; indexE < enrichment_results_length; indexE++)
            {
                enrichment_results_line = this.Enrichment_results[indexE];
                if (!sampleName_entryType_timepoint_dict.ContainsKey(enrichment_results_line.Sample_name))
                { sampleName_entryType_timepoint_dict.Add(enrichment_results_line.Sample_name, new Dictionary<DE_entry_enum, Dictionary<Timepoint_enum, bool>>()); }
                if (!sampleName_entryType_timepoint_dict[enrichment_results_line.Sample_name].ContainsKey(enrichment_results_line.Sample_entryType))
                { sampleName_entryType_timepoint_dict[enrichment_results_line.Sample_name].Add(enrichment_results_line.Sample_entryType, new Dictionary<Timepoint_enum, bool>()); }
                if (!sampleName_entryType_timepoint_dict[enrichment_results_line.Sample_name][enrichment_results_line.Sample_entryType].ContainsKey(enrichment_results_line.Sample_timepoint))
                { sampleName_entryType_timepoint_dict[enrichment_results_line.Sample_name][enrichment_results_line.Sample_entryType].Add(enrichment_results_line.Sample_timepoint, true); }
            }

            string[] sampleNames;
            string sampleName;
            int sampleNames_length;
            DE_entry_enum[] entryTypes;
            DE_entry_enum entryType;
            int entryTypes_length;
            Timepoint_enum[] timepoints;
            Timepoint_enum timepoint;
            int timepoints_length;
            this.Enrichment_results = Enrichment2018_results_line_class.Order_by_scp(this.Enrichment_results);
            for (int indexE = 0; indexE < enrichment_results_length; indexE++)
            {
                enrichment_results_line = this.Enrichment_results[indexE];
                if ((indexE == 0)
                    || (!enrichment_results_line.Scp.Equals(this.Enrichment_results[indexE - 1].Scp)))
                {
                    sampleNames = sampleName_entryType_timepoint_dict.Keys.ToArray();
                    sampleNames_length = sampleNames.Length;
                    for (int indexSN = 0; indexSN < sampleNames_length; indexSN++)
                    {
                        sampleName = sampleNames[indexSN];
                        entryType_timepoint_dict = sampleName_entryType_timepoint_dict[sampleName];
                        entryTypes = entryType_timepoint_dict.Keys.ToArray();
                        entryTypes_length = entryTypes.Length;
                        for (int indexEntryType = 0; indexEntryType < entryTypes_length; indexEntryType++)
                        {
                            entryType = entryTypes[indexEntryType];
                            if (entryType.Equals(DE_entry_enum.E_m_p_t_y)) { throw new Exception(); }
                            timepoint_dict = entryType_timepoint_dict[entryType];
                            timepoints = timepoint_dict.Keys.ToArray();
                            timepoints_length = timepoints.Length;
                            for (int indexTimepoint = 0; indexTimepoint < timepoints_length; indexTimepoint++)
                            {
                                timepoint = timepoints[indexTimepoint];
                                timepoint_dict[timepoint] = false;
                            }
                            entryType_timepoint_dict[entryType] = timepoint_dict;
                        }
                        sampleName_entryType_timepoint_dict[sampleName] = entryType_timepoint_dict;
                    }
                }
                if (sampleName_entryType_timepoint_dict[enrichment_results_line.Sample_name][enrichment_results_line.Sample_entryType][enrichment_results_line.Sample_timepoint]==true) { throw new Exception(); }
                sampleName_entryType_timepoint_dict[enrichment_results_line.Sample_name][enrichment_results_line.Sample_entryType][enrichment_results_line.Sample_timepoint] = true;
                if ((indexE == enrichment_results_length - 1)
                    || (!enrichment_results_line.Scp.Equals(this.Enrichment_results[indexE + 1].Scp)))
                {
                    sampleNames = sampleName_entryType_timepoint_dict.Keys.ToArray();
                    sampleNames_length = sampleNames.Length;
                    for (int indexSN = 0; indexSN < sampleNames_length; indexSN++)
                    {
                        sampleName = sampleNames[indexSN];
                        entryType_timepoint_dict = sampleName_entryType_timepoint_dict[sampleName];
                        entryTypes = entryType_timepoint_dict.Keys.ToArray();
                        entryTypes_length = entryTypes.Length;
                        for (int indexEntryType = 0; indexEntryType < entryTypes_length; indexEntryType++)
                        {
                            entryType = entryTypes[indexEntryType];
                            if (entryType.Equals(DE_entry_enum.E_m_p_t_y)) { throw new Exception(); }
                            timepoint_dict = entryType_timepoint_dict[entryType];
                            timepoints = timepoint_dict.Keys.ToArray();
                            timepoints_length = timepoints.Length;
                            for (int indexTimepoint = 0; indexTimepoint < timepoints_length; indexTimepoint++)
                            {
                                timepoint = timepoints[indexTimepoint];
                                if (timepoint_dict[timepoint] == false)
                                {
                                    add_enrichment_results_line = new Enrichment2018_results_line_class();
                                    add_enrichment_results_line.Ontology = enrichment_results_line.Ontology;
                                    add_enrichment_results_line.Sample_entryType = entryType;
                                    add_enrichment_results_line.Sample_name = (string)sampleName.Clone();
                                    add_enrichment_results_line.Sample_timepoint = timepoint;
                                    add_enrichment_results_line.Scp = (string)enrichment_results_line.Scp.Clone();
                                    add_enrichment_results_line.Overlap_count = 0;
                                    add_enrichment_results_line.Scp_genes_count = -1;
                                    add_enrichment_results_line.Pvalue = 1;
                                    add_enrichment_results_line.Minus_log10_pvalue = 0;
                                    add_enrichment_results_line.Fractional_rank = rank;
                                    add_enrichment_results_line.Color_specification = Enrichment2018_color_specification_enum.Regular;
                                    add_enrichment_results_line.Overlap_symbols = new string[0];
                                    add_enrichment_result_lines.Add(add_enrichment_results_line);
                                }
                            }
                            entryType_timepoint_dict[entryType] = timepoint_dict;
                        }
                        sampleName_entryType_timepoint_dict[sampleName] = entryType_timepoint_dict;
                    }
                }
            }
            Add_to_array(add_enrichment_result_lines.ToArray());
        }
        #endregion

        #region Filter
        public void Filter_by_keeping_only_input_scps(params string[] input_scps)
        {
            input_scps = input_scps.Distinct().OrderBy(l => l).ToArray();
            int input_length = input_scps.Length;
            string input_scp;
            int indexInput = 0;

            this.Enrichment_results = this.Enrichment_results.OrderBy(l => l.Scp).ToArray();
            int enrichment_results_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class results_line;
            List<Enrichment2018_results_line_class> keep_results = new List<Enrichment2018_results_line_class>();
            int stringCompare = -2;
            for (int indexE=0; indexE<enrichment_results_length;indexE++)
            {
                results_line = this.Enrichment_results[indexE];
                stringCompare = -2;
                while ((indexInput<input_length)&&(stringCompare<0))
                {
                    input_scp = input_scps[indexInput];
                    stringCompare = input_scp.CompareTo(results_line.Scp);
                    if (stringCompare<0)
                    {
                        indexInput++;
                    }
                    else if (stringCompare==0)
                    {
                        keep_results.Add(results_line);
                    }
                }
            }
            this.Enrichment_results = keep_results.ToArray();
        }
        public void Filter_by_keeping_only_lines_with_indicated_lincs_drugs(string[] drugs)
        {
            drugs = drugs.Distinct().OrderBy(l => l).ToArray();
            Dictionary<string, bool> keep_drugs_dict = new Dictionary<string, bool>();
            foreach (string drug in drugs)
            {
                keep_drugs_dict.Add(drug, true);
            }
            int enrichment_results_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class results_line;
            List<Enrichment2018_results_line_class> keep_results = new List<Enrichment2018_results_line_class>();
            for (int indexE = 0; indexE < enrichment_results_length; indexE++)
            {
                results_line = this.Enrichment_results[indexE];
                if (keep_drugs_dict.ContainsKey(results_line.Lincs_drug))
                {
                    keep_results.Add(results_line);
                }
            }
            this.Enrichment_results = keep_results.ToArray();
        }
        public void Filter_by_keeping_only_lines_with_indicated_entryType(DE_entry_enum keep_entryType)
        {
            int enrichment_results_length = this.Enrichment_results.Length;
            Enrichment2018_results_line_class results_line;
            List<Enrichment2018_results_line_class> keep_results = new List<Enrichment2018_results_line_class>();
            for (int indexE = 0; indexE < enrichment_results_length; indexE++)
            {
                results_line = this.Enrichment_results[indexE];
                if (results_line.Sample_entryType.Equals(keep_entryType))
                {
                    keep_results.Add(results_line);
                }
            }
            this.Enrichment_results = keep_results.ToArray();
        }
        #endregion

        #region Copy read write
        public Enrichment2018_results_class Deep_copy()
        {
            Enrichment2018_results_class copy = (Enrichment2018_results_class)this.MemberwiseClone();
            int enrichment_length = this.Enrichment_results.Length;
            copy.Enrichment_results = new Enrichment2018_results_line_class[enrichment_length];
            for (int indexE=0; indexE<enrichment_length; indexE++)
            {
                copy.Enrichment_results[indexE] = this.Enrichment_results[indexE].Deep_copy();
            }
            return copy;
        }
        public void Read(string subdirectory, params string[] fileNames)
        {
            List<Enrichment2018_results_line_class> new_enrichment_results = new List<Enrichment2018_results_line_class>();
            Enrichment2018_results_line_class[] add_enrichment_results;
            foreach (string fileName in fileNames)
            {
                Enrichment2018_results_readWriteOptions_class readWriteOptions = new Enrichment2018_results_readWriteOptions_class(subdirectory, fileName);
                add_enrichment_results = ReadWriteClass.ReadRawData_and_FillArray<Enrichment2018_results_line_class>(readWriteOptions);
                new_enrichment_results.AddRange(add_enrichment_results);
            }
            Enrichment_results = new_enrichment_results.ToArray();
        }
        public void Write_for_website(string subdirectory, string fileName, string cellType)
        {
            Enrichment2018_results_for_website_readWriteOptions_class readWriteOptions = new Enrichment2018_results_for_website_readWriteOptions_class(subdirectory, fileName, cellType);
            ReadWriteClass.WriteData(Enrichment_results, readWriteOptions);
        }
        public void Write_for_r(string subdirectory, string fileName)
        {
            Enrichment2018_results_readOptions_for_r_class readWriteOptions = new Enrichment2018_results_readOptions_for_r_class(subdirectory, fileName);
            ReadWriteClass.WriteData(Enrichment_results, readWriteOptions);
        }
        #endregion
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

}
