using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Common_classes;
using ReadWrite;
using Enrichment;

namespace MBCO
{
    enum Timeunit_enum { E_m_p_t_y = 0, sec = 1, min = 2, hrs = 3, days = 4, weeks = 5, months = 6, years = 7 }
    enum Significance_value_type_enum {  E_m_p_t_y, Minus_log10_pvalue, Minus_log10_adjusted_pvalue, Signed_minus_log10_pvalue }

    #region Copy paste from MBCO windows form
    class Timeunit_conversion_class
    {
        public static void Get_lowest_and_highest_timeunit(Timeunit_enum[] timeunits, out Timeunit_enum lowest_timeunit, out Timeunit_enum highest_timeunit)
        {
            timeunits = timeunits.OrderBy(l => l).ToArray();
            lowest_timeunit = timeunits[0];
            highest_timeunit = timeunits[timeunits.Length - 1];
        }

        public static Timeunit_enum Get_max_timeunit_with_timepoint_larger_1(int timepoint, Timeunit_enum timeunit)
        {
            switch (timeunit)
            {
                case Timeunit_enum.min:
                    if (timepoint >= 60 * 24 * 7) { return Timeunit_enum.weeks; }
                    else if (timepoint >= 60 * 24) { return Timeunit_enum.days; }
                    else if (timepoint >= 60) { return Timeunit_enum.hrs; }
                    else { return timeunit; }
                case Timeunit_enum.hrs:
                    if (timepoint >= 24 * 7) { return Timeunit_enum.weeks; }
                    else if (timepoint >= 24) { return Timeunit_enum.days; }
                    else { return timeunit; }
                case Timeunit_enum.days:
                    if (timepoint >= 365) { return Timeunit_enum.years; }
                    else if (timepoint >= 7) { return Timeunit_enum.weeks; }
                    else { return timeunit; }
                case Timeunit_enum.weeks:
                    return timeunit;
                case Timeunit_enum.months:
                    return timeunit;
                case Timeunit_enum.years:
                    return timeunit;
                default:
                    throw new Exception();
            }
        }

        public static float Convert_timepoint_from_old_unit_to_new_unit(float old_timepoint, Timeunit_enum old_timeunit, Timeunit_enum new_timeunit)
        {
            if ((old_timeunit.Equals(Timeunit_enum.sec)) && (new_timeunit.CompareTo(old_timeunit) > 0))
            {
                old_timepoint = old_timepoint / 60;
                old_timeunit = Timeunit_enum.min;
            }
            int compare = new_timeunit.CompareTo(old_timeunit);
            if ((old_timeunit.Equals(Timeunit_enum.min)) && (new_timeunit.CompareTo(old_timeunit) > 0))
            {
                old_timepoint = old_timepoint / 60;
                old_timeunit = Timeunit_enum.hrs;
            }
            if ((old_timeunit.Equals(Timeunit_enum.hrs)) && (new_timeunit.CompareTo(old_timeunit) > 0))
            {
                old_timepoint = old_timepoint / 24;
                old_timeunit = Timeunit_enum.days;
            }
            if ((old_timeunit.Equals(Timeunit_enum.days)) && (new_timeunit.CompareTo(old_timeunit) > 0))
            {
                old_timepoint = old_timepoint / 7;
                old_timeunit = Timeunit_enum.weeks;
            }

            if ((old_timeunit.Equals(Timeunit_enum.weeks)) && (new_timeunit.CompareTo(old_timeunit) < 0))
            {
                old_timepoint = old_timepoint * 7;
                old_timeunit = Timeunit_enum.days;
            }
            if ((old_timeunit.Equals(Timeunit_enum.days)) && (new_timeunit.CompareTo(old_timeunit) < 0))
            {
                old_timepoint = old_timepoint * 24;
                old_timeunit = Timeunit_enum.hrs;
            }
            if ((old_timeunit.Equals(Timeunit_enum.hrs)) && (new_timeunit.CompareTo(old_timeunit) < 0))
            {
                old_timepoint = old_timepoint * 60;
                old_timeunit = Timeunit_enum.min;
            }
            if ((old_timeunit.Equals(Timeunit_enum.min)) && (new_timeunit.CompareTo(old_timeunit) < 0))
            {
                old_timepoint = old_timepoint * 60;
                old_timeunit = Timeunit_enum.sec;
            }
            return old_timepoint;
        }

        public static float Get_timepoint_in_days(float timepoint, Timeunit_enum timeunit)
        {
            float timepoint_in_days = -1;
            switch (timeunit)
            {
                case Timeunit_enum.sec:
                    timepoint_in_days = timepoint / (60 * 60 * 24);
                    break;
                case Timeunit_enum.min:
                    timepoint_in_days = timepoint / (60 * 24);
                    break;
                case Timeunit_enum.hrs:
                    timepoint_in_days = timepoint / 24;
                    break;
                case Timeunit_enum.days:
                    timepoint_in_days = timepoint;
                    break;
                case Timeunit_enum.weeks:
                    timepoint_in_days = timepoint * 7;
                    break;
                case Timeunit_enum.months:
                    timepoint_in_days = timepoint * 30;
                    break;
                case Timeunit_enum.years:
                    timepoint_in_days = timepoint * 365;
                    break;
                default:
                    timepoint_in_days = timepoint;
                    break;
            }
            return timepoint_in_days;
        }
    }
    #endregion

    class MBCO_windows_form_input_line_class
    {
        public string Dataset_name { get; set; }
        public string Integration_group { get; set; }
        public string NCBI_official_gene_symbol { get; set; }
        public double Minus_log10_pval_or_adj_pval { get; set; } 
        public Significance_value_type_enum Significance_value_type { get; set; }
        public double Log2_fold_change { get; set; }
        public float Fractional_rank { get; set; }
        public float Timepoint { get; set; }
        public Timeunit_enum Timeunit { get; set; }
        public float TimepointInDays {  get { return Timeunit_conversion_class.Get_timepoint_in_days(Timepoint, Timeunit); } }
        public Color Dataset_color_struct { get; set; }
        public string Dataset_color
        {
            get { return Color_conversion_class.Get_color_string(Dataset_color_struct); }
            set { Dataset_color_struct = Color_conversion_class.Set_color_from_string(value); }
        }

        public MBCO_windows_form_input_line_class()
        {
            this.Integration_group = "";
            Fractional_rank = -1;
        }

        public MBCO_windows_form_input_line_class Deep_copy()
        {
            MBCO_windows_form_input_line_class copy = (MBCO_windows_form_input_line_class)this.MemberwiseClone();
            copy.Dataset_name = (string)this.Dataset_name.Clone();
            copy.Integration_group = (string)this.Integration_group.Clone();
            return copy;
        }
    }

    class MBCO_windows_form_input_readWriteOptions_class : ReadWriteOptions_base
    {
        public MBCO_windows_form_input_readWriteOptions_class(string directory, string fileName, bool add_timepoints, bool add_integrationGroups)
        {
            this.File = directory + fileName;
            List<string> key_properties_list = new List<string>();
            if (add_integrationGroups) { key_properties_list.Add("Integration_group"); }
            key_properties_list.Add("Dataset_name");
            if (add_timepoints)
            {
                key_properties_list.Add("Timepoint");
                key_properties_list.Add("Timeunit");
            }
            key_properties_list.Add("Dataset_color"); 
            key_properties_list.Add("NCBI_official_gene_symbol");
            key_properties_list.Add("Minus_log10_pval_or_adj_pval");
            key_properties_list.Add("Significance_value_type");
            key_properties_list.Add("Log2_fold_change");
            key_properties_list.Add("Fractional_rank");
            this.Key_propertyNames = key_properties_list.ToArray();
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }

    }

    class MBCO_windows_form_input_class
    {
        public MBCO_windows_form_input_line_class[] MBCO_input_lines { get; set; }
        public Dictionary<string, string[]> Scp_integrationGroups_dict { get; set; }
        public Dictionary<string,string[]> BgGenes_dict { get; set; }
        public MBCO_windows_form_input_class(Dictionary<string, string[]> scp_integrationGroups_dict)
        {
            this.MBCO_input_lines = new MBCO_windows_form_input_line_class[0];
            this.BgGenes_dict = new Dictionary<string, string[]>();
            Scp_integrationGroups_dict = new Dictionary<string, string[]>();
            string[] scps = scp_integrationGroups_dict.Keys.ToArray();
            foreach (string scp in scps)
            {
                Scp_integrationGroups_dict.Add(scp, scp_integrationGroups_dict[scp]);
            }
        }
        public MBCO_windows_form_input_class() : this(new Dictionary<string, string[]>())
        {
        }

        private void Add_to_array(MBCO_windows_form_input_line_class[] add_mbco_input_lines)
        {
            int this_length = this.MBCO_input_lines.Length;
            int add_length = add_mbco_input_lines.Length;
            int new_length = this_length + add_length;
            MBCO_windows_form_input_line_class[] new_mbco_input_lines = new MBCO_windows_form_input_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_mbco_input_lines[indexNew] = this.MBCO_input_lines[indexNew];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_mbco_input_lines[indexNew] = add_mbco_input_lines[indexAdd];
            }
            this.MBCO_input_lines = new_mbco_input_lines;
        }

        private void Add_to_bgGenes_dict(string bgGenes_name, string[] bgGenes)
        {
         //   if (BgGenes_dict.Keys.ToArray().Length == 1) { throw new Exception(); }
            if (!BgGenes_dict.ContainsKey(bgGenes_name))
            {
                BgGenes_dict.Add((string)bgGenes_name.Clone(), Array_class.Deep_copy_string_array(bgGenes));
            }
            else
            {
                foreach (string bgGene in bgGenes)
                {
                    if (!Array_class.Equal_arrays(BgGenes_dict[bgGenes_name], bgGenes)) { throw new Exception(); }
                }
            }
        }

        public void Remove_duplicates()
        {
            this.MBCO_input_lines = this.MBCO_input_lines.OrderBy(l => l.Dataset_name).ThenBy(l => l.Integration_group).ThenBy(l => l.TimepointInDays).ThenBy(l => Math.Sign(l.Log2_fold_change)).ThenBy(l => l.NCBI_official_gene_symbol).ToArray();
            int mboc_length = this.MBCO_input_lines.Length;
            MBCO_windows_form_input_line_class windows_form_input_line;
            List<MBCO_windows_form_input_line_class> keep = new List<MBCO_windows_form_input_line_class>();
            for (int indexMBCO=0; indexMBCO<mboc_length;indexMBCO++)
            {
                windows_form_input_line = this.MBCO_input_lines[indexMBCO];
                if ((indexMBCO == 0)
                    || (!windows_form_input_line.Dataset_name.Equals(this.MBCO_input_lines[indexMBCO - 1].Dataset_name))
                    || (!windows_form_input_line.Integration_group.Equals(this.MBCO_input_lines[indexMBCO - 1].Integration_group))
                    || (!windows_form_input_line.TimepointInDays.Equals(this.MBCO_input_lines[indexMBCO - 1].TimepointInDays))
                    || (!Math.Sign(windows_form_input_line.Log2_fold_change).Equals(Math.Sign(this.MBCO_input_lines[indexMBCO - 1].Log2_fold_change)))
                    || (!windows_form_input_line.NCBI_official_gene_symbol.Equals(this.MBCO_input_lines[indexMBCO - 1].NCBI_official_gene_symbol)))
                {
                    keep.Add(windows_form_input_line);
                }
                else if (  (!windows_form_input_line.Log2_fold_change.Equals(this.MBCO_input_lines[indexMBCO-1].Log2_fold_change))
                        || (!windows_form_input_line.Significance_value_type.Equals(this.MBCO_input_lines[indexMBCO - 1].Significance_value_type))
                        || (!windows_form_input_line.Minus_log10_pval_or_adj_pval.Equals(this.MBCO_input_lines[indexMBCO - 1].Minus_log10_pval_or_adj_pval)))
                {
                    throw new Exception();
                }
            }
            this.MBCO_input_lines = keep.ToArray();
        }

        #region Generate from input datasets
        public void GenerateFromEnrichmentData_addIntegrationGroups_and_addToArray(Enrichment2018_results_class enrich_result, string[] bgGenes, Dictionary<string, Color> upregulated_drug_color_dict, Dictionary<string, Color> downregulated_drug_color_dict, ref Dictionary<string, bool> added_scps_dict)
        {
            int enrichment_length = enrich_result.Enrichment_results.Length;
            Enrichment2018_results_line_class enrich_line;
            MBCO_windows_form_input_line_class new_mbco_windows_line;
            List<MBCO_windows_form_input_line_class> new_mbco_windows_lines = new List<MBCO_windows_form_input_line_class>();
            string cellline;
            string drug;
            //Drug_type_enum drugType;
            string[] integrationGroups;
            string integrationGroup;
            int integrationGroups_length;
            string[] overlap_symbols;
            string overlap_symbol;
            int overlap_symbols_length;
            System.Drawing.Color current_color;
            for (int indexEnrich = 0; indexEnrich < enrichment_length; indexEnrich++)
            {
                enrich_line = enrich_result.Enrichment_results[indexEnrich];
                if (Scp_integrationGroups_dict.ContainsKey(enrich_line.Scp))
                {
                    if (!added_scps_dict.ContainsKey(enrich_line.Scp)) { added_scps_dict.Add(enrich_line.Scp, true); }
                    cellline = enrich_line.Lincs_cell_line;
                    drug = enrich_line.Lincs_drug;
                    overlap_symbols = enrich_line.Overlap_symbols;
                    overlap_symbols_length = overlap_symbols.Length;

                    integrationGroups = Scp_integrationGroups_dict[enrich_line.Scp];
                    integrationGroups_length = integrationGroups.Length;
                    for (int indexIG = 0; indexIG < integrationGroups_length; indexIG++)
                    {
                        integrationGroup = integrationGroups[indexIG];
                        for (int indexOS = 0; indexOS < overlap_symbols_length; indexOS++)
                        {
                            overlap_symbol = overlap_symbols[indexOS];
                            new_mbco_windows_line = new MBCO_windows_form_input_line_class();
                            new_mbco_windows_line.NCBI_official_gene_symbol = (string)overlap_symbol.Clone();
                            current_color = System.Drawing.Color.Gray;
                            switch (enrich_line.Sample_entryType)
                            {
                                case DE_entry_enum.Diffrna_up:
                                    new_mbco_windows_line.Log2_fold_change = 1;
                                    if (upregulated_drug_color_dict.ContainsKey(drug))
                                    {
                                        current_color = upregulated_drug_color_dict[drug];
                                    }
                                    break;
                                case DE_entry_enum.Diffrna_down:
                                    new_mbco_windows_line.Log2_fold_change = -1;
                                    if (downregulated_drug_color_dict.ContainsKey(drug))
                                    {
                                        current_color = downregulated_drug_color_dict[drug];
                                    }
                                    break;
                                default:
                                    throw new Exception();
                            }
                            new_mbco_windows_line.Minus_log10_pval_or_adj_pval = 0;

                            new_mbco_windows_line.Dataset_name = drug + " - " + cellline;
                            new_mbco_windows_line.Significance_value_type = Significance_value_type_enum.Signed_minus_log10_pvalue;
                            new_mbco_windows_line.Integration_group = (string)integrationGroup.Clone();
                            new_mbco_windows_line.Dataset_color_struct = current_color;
                            new_mbco_windows_lines.Add(new_mbco_windows_line);
                        }
                    }
                }
            }
            Add_to_bgGenes_dict("BgGenes", bgGenes);
            Add_to_array(new_mbco_windows_lines.ToArray());
        }
        #endregion
        private float[] Get_all_different_timepointsInDays()
        {
            Dictionary<float, bool> timepointsInDays_dict = new Dictionary<float, bool>();
            foreach (MBCO_windows_form_input_line_class mbco_line in this.MBCO_input_lines)
            {
                if (!timepointsInDays_dict.ContainsKey(mbco_line.TimepointInDays))
                {
                    timepointsInDays_dict.Add(mbco_line.TimepointInDays, true);
                }
            }
            return timepointsInDays_dict.Keys.ToArray();
        }

        public string[] Get_all_different_integrationGroups()
        {
            Dictionary<string, bool> integrationGroups_dict = new Dictionary<string, bool>();
            foreach (MBCO_windows_form_input_line_class mbco_line in this.MBCO_input_lines)
            {
                if (!integrationGroups_dict.ContainsKey(mbco_line.Integration_group))
                {
                    integrationGroups_dict.Add(mbco_line.Integration_group, true);
                }
            }
            return integrationGroups_dict.Keys.ToArray();
        }

        public void Write(string completeDirectory, string fileName)
        {
            bool add_timepoints = Get_all_different_timepointsInDays().Length > 1;
            bool add_integrationGroups = Get_all_different_integrationGroups().Length > 1;
            MBCO_windows_form_input_readWriteOptions_class readWriteOptions = new MBCO_windows_form_input_readWriteOptions_class(completeDirectory, fileName, add_timepoints, add_integrationGroups);
            ReadWriteClass.WriteData(this.MBCO_input_lines, readWriteOptions);


            string[] bgGeneList_names = this.BgGenes_dict.Keys.ToArray();
            string fileName_without_extension = System.IO.Path.GetFileNameWithoutExtension(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            if (bgGeneList_names.Length>1) { throw new Exception(); }
            foreach (string bgGeneList_name in bgGeneList_names)
            {
                ReadWriteClass.WriteArray_into_directory(this.BgGenes_dict[bgGeneList_name], completeDirectory, fileName_without_extension + "_bgGenes" + extension);
            }
        }
        public void Write_scpIntegrationGroups_as_mbcoApp_parameter_settings(string completeDirectory)
        {
            Dictionary<string, List<string>> overallFunction_scps_dict = new Dictionary<string, List<string>>();
            #region Fill overallFunction_scps_dict
            string[] scps = Scp_integrationGroups_dict.Keys.ToArray();
            string scp;
            int scps_length = scps.Length;
            string[] overallFunctions;
            string overallFunction;
            int overallFunctions_length;
            for (int indexScp=0; indexScp<scps_length;indexScp++)
            {
                scp = scps[indexScp];
                overallFunctions = Scp_integrationGroups_dict[scp];
                overallFunctions_length = overallFunctions.Length;
                for (int indexOverall=0; indexOverall<overallFunctions_length;indexOverall++)
                {
                    overallFunction = overallFunctions[indexOverall];
                    if (!overallFunction_scps_dict.ContainsKey(overallFunction))
                    {
                        overallFunction_scps_dict[overallFunction] = new List<string>();
                    }
                    overallFunction_scps_dict[overallFunction].Add(scp);
                }
            }
            #endregion

            string complete_fileName = completeDirectory + "MBCOApp_parameter_settings.txt";
            System.IO.StreamWriter writer = new System.IO.StreamWriter(complete_fileName);
            char delimiter = Global_class.Tab;
            writer.WriteLine("#MBCO parameter settings for re-import into application");
            writer.WriteLine("User_data_options_class{0}All_genes_significant{0}True", delimiter);
            writer.WriteLine("Bardiagram_options_class{0}Generate_bardiagrams{0}False", delimiter);
            writer.WriteLine("Heatmap_options_class{0}Generate_bardiagrams{0}False", delimiter);
            writer.WriteLine("Timeline_options_class{0}Generate_bardiagrams{0}False", delimiter);
            writer.WriteLine("MBCO_enrichment_pipeline_options_class{0}Keep_top_standard_SCPs{0}Mbco_human{0}0;999;5;10;5", delimiter);
            writer.WriteLine("MBCO_enrichment_pipeline_options_class{0}Max_pvalue_standard_enrichment{0}Mbco_human{0}1", delimiter);
            writer.WriteLine("MBCO_network_based_integration_options_class{0}Add_parent_child_relationships_to_standard_SCP_networks{0}Mbco_human{0}True", delimiter);
            writer.WriteLine("MBCO_network_based_integration_options_class{0}Add_genes_to_standard_networks{0}Mbco_human{0}True", delimiter);
            writer.WriteLine("MBCO_network_based_integration_options_class{0}Add_edges_that_connect_standard_scps{0}Mbco_human{0}False", delimiter);
            writer.WriteLine("MBCO_network_based_integration_options_class{0}Node_size_determinant{0}Mbco_human{0}Standard", delimiter);
            writer.WriteLine("MBCO_network_based_integration_options_class{0}Box_sameLevel_scps_for_standard_enrichment{0}Mbco_human{0}False", delimiter);
            overallFunctions = overallFunction_scps_dict.Keys.ToArray();
            overallFunctions_length = overallFunctions.Length;

            for (int indexOF = 0; indexOF < overallFunctions_length; indexOF++)
            {
                overallFunction = overallFunctions[indexOF];
                writer.Write("MBCO_enrichment_pipeline_options_class{0}User_selected_scps{0}Mbco_human{0}{1}{0}", delimiter, overallFunction);
                scps = overallFunction_scps_dict[overallFunction].ToArray();
                scps_length = scps.Length;
                for (int indexScp = 0; indexScp < scps_length; indexScp++)
                {
                    if (indexScp > 0) { writer.Write(";"); }
                    writer.Write("{0}", scps[indexScp]);
                }
                writer.WriteLine();
            }
            writer.Close();
        }

        public MBCO_windows_form_input_class Deep_copy()
        {
            MBCO_windows_form_input_class copy = (MBCO_windows_form_input_class)this.MemberwiseClone();
            int mbco_length = this.MBCO_input_lines.Length;
            copy.MBCO_input_lines = new MBCO_windows_form_input_line_class[mbco_length];
            for (int indexMBCO=0; indexMBCO < mbco_length; indexMBCO++)
            {
                copy.MBCO_input_lines[indexMBCO] = this.MBCO_input_lines[indexMBCO].Deep_copy();
            }
            string[] bgGeneLists = BgGenes_dict.Keys.ToArray();
            copy.BgGenes_dict = new Dictionary<string, string[]>();
            foreach (string bgGeneList in bgGeneLists)
            {
                copy.BgGenes_dict.Add(bgGeneList, Array_class.Deep_copy_string_array(this.BgGenes_dict[bgGeneList]));
            }
            return copy;
        }
    }
}
