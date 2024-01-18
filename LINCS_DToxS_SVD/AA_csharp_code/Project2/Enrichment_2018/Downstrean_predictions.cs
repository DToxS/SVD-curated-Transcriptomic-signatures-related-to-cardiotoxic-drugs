using Common_classes;
using Highthroughput_data;
using Statistic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enrichment
{
    enum Enrichment_algorithm_enum { E_m_p_t_y, Fishers_exact_test }

    class Downstream_analysis_2020_options_class
    {
        public Ontology_type_enum[] Ontologies { get; set; }
        public Data_value_signs_of_interest_enum[] Data_value_signs_of_interest { get; set; }
        public float Max_pvalue { get; set; }
        public int Top_top_x_predictions { get; set; }
        public string[] Highlight_scps { get; set; }
        public Organism_enum Organism { get; set; }
        public bool Write_results { get; set; }
        public bool Only_keep_filtered_results { get; set; }
        public bool Add_missing_scps_identified_in_other_conditions { get; set; }
        public bool Use_ontology_abbreviations_for_directories_and_files { get; set; }
        public Enrichment_algorithm_enum Enrichment_algorithm { get; set; }

        public Downstream_analysis_2020_options_class()
        {
            Organism = Global_class.Organism;
            Ontologies = new Ontology_type_enum[] { Ontology_type_enum.Mbco_level1, Ontology_type_enum.Mbco_level2, Ontology_type_enum.Mbco_level3, Ontology_type_enum.Mbco_level4 };
            Highlight_scps = new string[0];
            Data_value_signs_of_interest = new Data_value_signs_of_interest_enum[] { Data_value_signs_of_interest_enum.Combined, Data_value_signs_of_interest_enum.Upregulated, Data_value_signs_of_interest_enum.Downregulated };
            Top_top_x_predictions = 15;
            Write_results = true;
            Only_keep_filtered_results = false;
            Add_missing_scps_identified_in_other_conditions = false;
            Use_ontology_abbreviations_for_directories_and_files = true;
            Enrichment_algorithm = Enrichment_algorithm_enum.Fishers_exact_test;
            Max_pvalue = 1;
        }
    }

    class Downstream_analysis_2020_class
    {
        public Fisher_exact_test_class Fisher { get; set; }
        public Ontology_library_class[] Ontology_libraries { get; set; }
        string[] Experimental_bg_genes { get; set; }
        public Downstream_analysis_2020_options_class Options { get; set; }

        public Downstream_analysis_2020_class()
        {
            Options = new Downstream_analysis_2020_options_class();
        }

        #region Generate for all runs
        private void Generate_ontology_libraries()
        {
            Ontology_type_enum[] ontologies = this.Options.Ontologies.Distinct().ToArray();
            Ontology_type_enum ontology;
            int ontologies_length = ontologies.Length;
            Ontology_libraries = new Ontology_library_class[ontologies_length];
            Ontology_library_class ontology_library;
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                ontology = ontologies[indexO];
                ontology_library = new Ontology_library_class();
                ontology_library.Generate_by_reading(ontology, Options.Organism, this.Experimental_bg_genes);
                Ontology_libraries[indexO] = ontology_library;
            }
        }
        public void Generate(params string[] bg_genes)
        {
            if (bg_genes.Length > 0)
            {
                this.Experimental_bg_genes = Array_class.Deep_copy_string_array(bg_genes);
            }
            else
            {
                // throw new Exception();
                this.Experimental_bg_genes = new string[0];
            }
            Generate_ontology_libraries();

            Fisher = new Fisher_exact_test_class(50000, false);
        }
        #endregion

        private Enrichment2018_results_class Do_enrichment_analysis_with_single_ontology_on_all_columns(Ontology_library_class ontology_library, DE_class de_input)
        {
            Ontology_type_enum current_ontology = ontology_library.Get_current_ontology_and_check_if_only_one();
            DE_class de = de_input.Deep_copy();
            string[] all_ontology_genes = ontology_library.Get_all_ordered_unique_gene_symbols();
            de.Keep_only_stated_symbols(all_ontology_genes);
            Dictionary<string, DE_line_class> deSymbol_deLine_dict = new Dictionary<string, DE_line_class>();
            DE_line_class de_line;
            int de_length = de.DE.Count;
            int de_colCount = de.ColChar.Columns.Count;
            for (int indexDe = 0; indexDe < de_length; indexDe++)
            {
                de_line = de.DE[indexDe];
                deSymbol_deLine_dict.Add(de_line.Gene_symbol, de_line);
            }

            List<Enrichment2018_results_line_class> enrichment_results_list = new List<Enrichment2018_results_line_class>();
            Dictionary<string, List<string>[]> pathway_deColumnsOverlapSymbols_dict = new Dictionary<string, List<string>[]>();
            Dictionary<string, int> pathway_pathwaySymbolsCount_dict = new Dictionary<string, int>();
            int library_length = ontology_library.Library.Length;
            Ontology_library_line_class ontology_library_line;
            DE_line_class current_line;
            string current_scp;
            string current_symbol;

            DE_line_class de_line_copy;
            DE_class de_copy = new DE_class();
            Dictionary<string, bool> ontology_bgSymbols_dict = new Dictionary<string, bool>();
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                ontology_library_line = ontology_library.Library[indexL];
                current_scp = ontology_library_line.Scp;
                current_symbol = ontology_library_line.Target_gene_symbol;
                if (!ontology_bgSymbols_dict.ContainsKey(current_symbol))
                {
                    ontology_bgSymbols_dict.Add(current_symbol, true);
                }
                if (!pathway_pathwaySymbolsCount_dict.ContainsKey(current_scp))
                {
                    pathway_pathwaySymbolsCount_dict.Add(current_scp, 0);
                }
                pathway_pathwaySymbolsCount_dict[current_scp]++;
                if (deSymbol_deLine_dict.ContainsKey(current_symbol))
                {
                    current_line = deSymbol_deLine_dict[current_symbol];
                    de_line_copy = current_line.Deep_copy();
                    de_copy.DE.Add(de_line_copy);
                    if (!pathway_deColumnsOverlapSymbols_dict.ContainsKey(ontology_library_line.Scp))
                    {
                        pathway_deColumnsOverlapSymbols_dict.Add(current_scp, new List<string>[de_colCount]);
                        for (int indexCol = 0; indexCol < de_colCount; indexCol++)
                        {
                            pathway_deColumnsOverlapSymbols_dict[current_scp][indexCol] = new List<string>();
                        }
                    }
                    for (int indexCol = 0; indexCol < de_colCount; indexCol++)
                    {
                        if (current_line.Columns[indexCol].Value != 0)
                        {
                            pathway_deColumnsOverlapSymbols_dict[current_scp][indexCol].Add(current_symbol);
                        }
                    }
                }
            }

            int[] experimental_symbols_in_columns_count = de.Get_non_zero_counts_of_each_column_in_indexOrder();
            string[] scps = pathway_deColumnsOverlapSymbols_dict.Keys.ToArray();
            int scps_length = scps.Length;
            string scp;
            List<string>[] currentScp_overlap_symbols_of_columns;
            List<string> currentScpColumn_overlap_symbols;
            Enrichment2018_results_line_class new_enrichment_results_line;
            List<Enrichment2018_results_line_class> new_enrichment_results_list = new List<Enrichment2018_results_line_class>();
            List<DE_column_characterization_line_class> colChar_columns = de.ColChar.Columns;
            string[] complete_sampleNames_of_columns = new string[de_colCount];
            Timepoint_enum[] timepoints_of_columns = new Timepoint_enum[de_colCount];
            DE_entry_enum[] entryTypes_of_columns = new DE_entry_enum[de_colCount];
            int bg_genes_count = ontology_bgSymbols_dict.Keys.ToArray().Length;
            for (int indexCol = 0; indexCol < de_colCount; indexCol++)
            {
                complete_sampleNames_of_columns[indexCol] = de.ColChar.Columns[indexCol].Get_full_column_name();
                timepoints_of_columns[indexCol] = de.ColChar.Columns[indexCol].Timepoint;
                entryTypes_of_columns[indexCol] = de.ColChar.Columns[indexCol].EntryType;
            }
            int a; int b; int c; int d; double p_value;
            for (int indexScp = 0; indexScp < scps_length; indexScp++)
            {
                scp = scps[indexScp];
                currentScp_overlap_symbols_of_columns = pathway_deColumnsOverlapSymbols_dict[scp];
                for (int indexCol = 0; indexCol < de_colCount; indexCol++)
                {
                    currentScpColumn_overlap_symbols = currentScp_overlap_symbols_of_columns[indexCol];
                    if (currentScpColumn_overlap_symbols.Count > 0)
                    {
                        new_enrichment_results_line = new Enrichment2018_results_line_class();
                        new_enrichment_results_line.Scp = (string)scp.Clone();
                        new_enrichment_results_line.Overlap_symbols = currentScpColumn_overlap_symbols.Distinct().OrderBy(l => l).ToArray();
                        if (new_enrichment_results_line.Overlap_symbols.Length != currentScpColumn_overlap_symbols.Count) { throw new Exception(); }
                        new_enrichment_results_line.Overlap_count = new_enrichment_results_line.Overlap_symbols.Length;
                        new_enrichment_results_line.Ontology = current_ontology;
                        new_enrichment_results_line.Experimental_genes_count = experimental_symbols_in_columns_count[indexCol];
                        new_enrichment_results_line.Scp_genes_count = pathway_pathwaySymbolsCount_dict[scp];
                        new_enrichment_results_line.Sample_name = (string)complete_sampleNames_of_columns[indexCol].Clone();
                        new_enrichment_results_line.Sample_timepoint = timepoints_of_columns[indexCol];
                        new_enrichment_results_line.Sample_entryType = entryTypes_of_columns[indexCol];
                        new_enrichment_results_line.Bg_genes_count = bg_genes_count;

                        a = new_enrichment_results_line.Overlap_count;
                        b = new_enrichment_results_line.Scp_genes_count - a;
                        c = new_enrichment_results_line.Experimental_genes_count - a;
                        d = new_enrichment_results_line.Bg_genes_count - a - b - c;

                        if ((a < 0) || (b < 0) || (c < 0) || (d < 0)) { throw new Exception(); }

                        p_value = Fisher.Get_rightTailed_p_value(a, b, c, d);
                        new_enrichment_results_line.Pvalue = p_value;
                        new_enrichment_results_line.Minus_log10_pvalue = (float)-Math.Log(p_value, 10);
                        new_enrichment_results_list.Add(new_enrichment_results_line);
                    }
                }
            }
            Enrichment2018_results_class enrichment_results = new Enrichment2018_results_class();
            enrichment_results.Enrichment_results = new_enrichment_results_list.ToArray();
            return enrichment_results;
        }
        public DE_class Get_de_instance_with_separated_entries_based_on_specified_signs_of_interest(DE_class de_input)
        {
            DE_class de = new DE_class();
            if (Options.Data_value_signs_of_interest.Contains(Data_value_signs_of_interest_enum.Upregulated))
            {
                DE_class de_up = de_input.Get_DE_class_with_only_upregulated_entries();
                if (de.DE.Count == 0) { de = de_up; }
                else
                {
                    de.Fill_with_other_de_alternativly(de_up);
                }
            }
            if (Options.Data_value_signs_of_interest.Contains(Data_value_signs_of_interest_enum.Downregulated))
            {
                DE_class de_down = de_input.Get_DE_class_with_only_downregulated_entries();
                if (de.DE.Count == 0) { de = de_down; }
                else
                {
                    de.Fill_with_other_de_alternativly(de_down);
                }
            }
            if (Options.Data_value_signs_of_interest.Contains(Data_value_signs_of_interest_enum.Combined))
            {
                DE_class de_combined = de_input.Deep_copy();
                if (de.DE.Count == 0) { de = de_combined; }
                else
                {
                    de.Fill_with_other_de_alternativly(de_combined);
                }
            }
            return de;
        }
        private Enrichment2018_results_class[] Generate_filtered_enrichment_results(Enrichment2018_results_class[] enrichment_results_array)
        {
            int enrichment2018_results_length = enrichment_results_array.Length;
            Enrichment2018_results_class current_enrichment_results;
            Enrichment2018_results_class filtered_enrichment_results;
            Enrichment2018_results_class[] filtered_enrichment_results_array = new Enrichment2018_results_class[enrichment2018_results_length];
            for (int indexE = 0; indexE < enrichment2018_results_length; indexE++)
            {
                current_enrichment_results = enrichment_results_array[indexE];
                filtered_enrichment_results = current_enrichment_results.Deep_copy();
                filtered_enrichment_results.Keep_only_scps_with_maximium_pvalue(this.Options.Max_pvalue);
                filtered_enrichment_results.Keep_only_top_x_ranked_scps_per_condition(this.Options.Top_top_x_predictions);
                filtered_enrichment_results_array[indexE] = filtered_enrichment_results;
            }
            return filtered_enrichment_results_array;
        }
        public Enrichment2018_results_class[] Analyse_de_instance_and_return_unfiltered_enrichment_results(DE_class de_input, string subdirectory, string add_results_file_name)
        {
            DE_class de = Get_de_instance_with_separated_entries_based_on_specified_signs_of_interest(de_input);
            if (this.Experimental_bg_genes.Length > 0)
            {
                de.Keep_only_stated_symbols(this.Experimental_bg_genes); //Non ontology genes will be removed for each ontology separately
            }
            Ontology_library_class current_ontology_library;
            int ontologies_length = Ontology_libraries.Length;
            Enrichment2018_results_class new_enrichment_results;
            Enrichment2018_results_class[] enrichment_results = new Enrichment2018_results_class[ontologies_length];
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                current_ontology_library = Ontology_libraries[indexO];
                switch (Options.Enrichment_algorithm)
                {
                    case Enrichment_algorithm_enum.Fishers_exact_test:
                        new_enrichment_results = Do_enrichment_analysis_with_single_ontology_on_all_columns(current_ontology_library, de);
                        break;
                    default:
                        throw new Exception();
                }
                new_enrichment_results.Order_by_ontology_sampleName_timepoint_entryType_pvalue();
                new_enrichment_results.Remove_indicated_scps("Background genes");
                if (this.Options.Highlight_scps.Length > 0)
                {
                    new_enrichment_results.Highlight_selected_scps(this.Options.Highlight_scps);
                }
                if (Options.Add_missing_scps_identified_in_other_conditions)
                {
                    new_enrichment_results.Add_missing_scps_that_were_detected_at_least_once_with_pvalue_one_and_indicated_rank(99999, de);
                }
                enrichment_results[indexO] = new_enrichment_results;
                new_enrichment_results.Calculate_fractional_ranks_for_scps_based_on_selected_valuetype(Enrichment2018_value_type_enum.Minuslog10pvalue);
            }
            if (Options.Write_results)
            {
                 string add_results_file_name_internal = (string)add_results_file_name.Clone();
                 if (add_results_file_name_internal.Length>0)
                 { add_results_file_name_internal = "_" + add_results_file_name_internal; }
                 Write_enrichment_results_for_R(enrichment_results, subdirectory, add_results_file_name_internal + "_all");
                 Enrichment2018_results_class[] filtered_enrichment_results = Generate_filtered_enrichment_results(enrichment_results);
                 Write_enrichment_results_for_R(filtered_enrichment_results, subdirectory, add_results_file_name_internal + "_filtered");
            }
            return enrichment_results;
        }
        public void Write_enrichment_results_for_R(Enrichment2018_results_class[] enrichment_results, string subdirectory, string addition_at_end_of_file)
        {
            subdirectory = subdirectory + "//";
            Ontology_type_enum ontology;
            string ontology_string = "error";
            foreach (Enrichment2018_results_class enrichment_result in enrichment_results)
            {
                if (enrichment_result.Enrichment_results.Length > 0)
                {
                    ontology = enrichment_result.Get_ontology_and_check_if_only_one();
                    if (Options.Use_ontology_abbreviations_for_directories_and_files)
                    {
                        ontology_string = Ontology_abbreviation_class.Get_abbreviation_of_ontology(ontology);
                    }
                    else
                    {
                        ontology_string = ontology.ToString();
                    }
                    enrichment_result.Write_for_r(subdirectory, ontology_string + addition_at_end_of_file + ".txt");
                }
            }
        }

        public Downstream_analysis_2020_class Deep_copy()
        {
            Downstream_analysis_2020_class copy = (Downstream_analysis_2020_class)this.MemberwiseClone();
            copy.Fisher = this.Fisher.Deep_copy();
            int libraries_length = this.Ontology_libraries.Length;
            copy.Ontology_libraries = new Ontology_library_class[libraries_length];
            for (int indexOL=0; indexOL<libraries_length; indexOL++)
            {
                copy.Ontology_libraries[indexOL] = this.Ontology_libraries[indexOL].Deep_copy();
            }
            int bgGenes_length = this.Experimental_bg_genes.Length;
            copy.Experimental_bg_genes = new string[bgGenes_length];
            for (int indexBG=0; indexBG<bgGenes_length; indexBG++)
            {
                copy.Experimental_bg_genes[indexBG] = (string)this.Experimental_bg_genes[indexBG].Clone();
            }
            return copy;
        }
    }


}