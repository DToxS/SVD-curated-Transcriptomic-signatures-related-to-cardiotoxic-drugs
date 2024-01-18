using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Common_classes
{
    class Global_class
    {
        #region Const
        public static bool Memory_larger_than_16GB = true;
        private const Input_data_enum input_data = Input_data_enum.Lincs;
        private const Organism_enum organism = Organism_enum.Homo_sapiens;
        private const string empty_entry = "E_m_p_t_y";  //check enums, Empty has to be the same!!
        private const string auto_entry = "Auto";
        private const string no_nodeName = "N_o_n_o_d_e";
        private const char tab = '\t';
        private const char space = ' ';
        private const char comma = ',';
        private const string separation_text = "S_e_p_a_r_a_t_i_o_n";
        private const string space_text = "S_p_a_c_e";
        private const int inf_number_abs = 99999999;
        public const bool Check_ordering = true;
        #endregion

        #region cellular components
        private const string go_plasma_membrane = "GO:0005886";
        private const string go_integral_to_plasma_membrane = "GO:0005887";

        public static string GO_plasma_membrane { get { return go_plasma_membrane; } }
        public static string GO_integral_to_plasma_membrane { get { return go_integral_to_plasma_membrane; } }
        #endregion

        private const string no_node_text = "N_o_n_o_d_e";
        public static Encoding Get_streamWriter_encoding() { return Encoding.UTF8; }

        public static Input_data_enum Input_data
        {
            get
            {
                Input_data_enum Input_data = input_data;
                return Input_data;
            }
        }

        public static Organism_enum Organism
        {
            get { return organism; }
        }

        public static int Inf_number_abs
        { get { return inf_number_abs; } }

        public static int Inf_number_pos
        { get { return Inf_number_abs; } }

        public static int Inf_number_neg
        { get { return -Inf_number_abs; } }

        public static string No_nodeName
        {
            get { return no_nodeName; }
        }

        public static string Empty_entry
        { get { return empty_entry; } }

        public static string Auto_entry
        { get { return auto_entry; } }

        public static char Comma
        { get { return comma; } }

        public static char Tab
        { get { return tab; } }

        public static char Space
        { get { return space; } }

        public static string Separation_text
        { get { return separation_text; } }

        public static string Space_text
        { get { return space_text; } }

        public static string No_node_text
        { get { return no_node_text; } }

    }
    public class Color_conversion_class
    {
        System.Drawing.Color[] All_csharp_colors { get; set; }
        public Color_conversion_class()
        {
            All_csharp_colors = Get_all_csharp_colors();
        }


        ///Inset was written by Chat GPT
        public (int R, int G, int B) ContinuousToRGB(double value)
        {
            if (value < 0 || value > 1)
                throw new ArgumentOutOfRangeException("Value must be in the range [0, 1]");

            double hue = value;
            double saturation = 1.0;
            double brightness = 1.0;

            return HSVToRGB(hue * 360, saturation, brightness);
        }

        public string ContinuousToHexadecimalColor(double value)
        {
            int red; int green; int blue;
            (red, green, blue) = ContinuousToRGB(value);
            return RGBToHex(red, green, blue);
        }

        public string RGBToHex(int r, int g, int b)
        {
            // Ensure the RGB values are within the valid range
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
            {
                throw new ArgumentException("RGB values must be in the range 0-255.");
            }

            // Convert each component to hexadecimal and concatenate
            return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        private (int R, int G, int B) HSVToRGB(double h, double s, double v)
        {
            double r = 0, g = 0, b = 0;

            int i = (int)(h / 60.0) % 6;
            double f = (h / 60.0) - Math.Floor(h / 60.0);
            double p = v * (1.0 - s);
            double q = v * (1.0 - f * s);
            double t = v * (1.0 - (1.0 - f) * s);

            switch (i)
            {
                case 0:
                    r = v; g = t; b = p; break;
                case 1:
                    r = q; g = v; b = p; break;
                case 2:
                    r = p; g = v; b = t; break;
                case 3:
                    r = p; g = q; b = v; break;
                case 4:
                    r = t; g = p; b = v; break;
                case 5:
                    r = v; g = p; b = q; break;
            }

            return ((int)(r * 255.0), (int)(g * 255.0), (int)(b * 255.0));
        }
        ///Inset was written by Chat GPT

        #region Static functions for call in property fields
        public static string Get_color_string(System.Drawing.Color color)
        {
            string color_string = color.ToString().Replace("Color ", "").Replace("[", "").Replace("]", "");
            return color_string;
        }

        public static System.Drawing.Color Set_color_from_string(string color_string)
        {
            System.Drawing.Color return_color;
            return_color = System.Drawing.Color.FromName(color_string);
            return return_color;
        }
        #endregion

        private System.Drawing.Color[] Get_all_csharp_colors()
        {
            Dictionary<System.Drawing.Color, bool> selectable_colors_dict = new Dictionary<System.Drawing.Color, bool>();
            System.Drawing.Color add_color;
            foreach (System.Reflection.PropertyInfo property in typeof(System.Drawing.Color).GetProperties())
            {
                if (property.PropertyType == typeof(System.Drawing.Color))
                {
                    add_color = (System.Drawing.Color)property.GetValue(null, null);
                    if ((!selectable_colors_dict.ContainsKey(add_color))
                        && (!add_color.Equals(System.Drawing.Color.Transparent))
                        && (!add_color.Equals(System.Drawing.Color.White)))
                    {
                        selectable_colors_dict.Add(add_color, false);
                    }
                }
            }
            return selectable_colors_dict.Keys.ToArray();
        }

        private System.Drawing.Color Get_closest_csharp_color(int input_red, int input_green, int input_blue)
        {
            int all_colors_length = All_csharp_colors.Length;
            System.Drawing.Color current_color;
            int csharp_red = -1;
            int csharp_green = -1;
            int csharp_blue = -1;
            float current_distance;
            float minimum_distance = 999999999;
            System.Drawing.Color selected_csharp_color = System.Drawing.Color.Gray;
            for (int indexColor = 0; indexColor < all_colors_length; indexColor++)
            {
                current_color = All_csharp_colors[indexColor];
                csharp_blue = int.Parse(current_color.B.ToString());
                csharp_red = int.Parse(current_color.R.ToString());
                csharp_green = int.Parse(current_color.G.ToString());
                current_distance = (float)Math.Sqrt(Math.Pow(input_red - csharp_red, 2) + Math.Pow(input_blue - csharp_blue, 2) + Math.Pow(input_green - csharp_green, 2));
                if (current_distance < minimum_distance)
                {
                    minimum_distance = current_distance;
                    selected_csharp_color = current_color;
                }
            }
            return selected_csharp_color;
        }

        public System.Drawing.Color Get_closest_csharp_color_for_hexadecimal_color_if_exists(string color_string)
        {
            System.Drawing.Color closest_color = System.Drawing.Color.FromName(color_string);
            if ((color_string.Substring(0, 1).Equals("#"))
                && (color_string.Length == 7))
            {
                try
                {
                    int red = int.Parse(color_string.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    int green = int.Parse(color_string.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    int blue = int.Parse(color_string.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                    closest_color = Get_closest_csharp_color(red, green, blue);

                }
                catch { }
            }
            return closest_color;
        }
    }

    public class Number2string_conversion_class
    {
        private Dictionary<string, int> NumberString_number_dict { get; set; }
        private Dictionary<int, string> Number_numberString_dict { get; set; }

        public Number2string_conversion_class()
        {
            NumberString_number_dict = new Dictionary<string, int>();
            NumberString_number_dict.Add("One", 1);
            NumberString_number_dict.Add("Two", 2);
            NumberString_number_dict.Add("Three", 3);
            NumberString_number_dict.Add("Four", 4);
            NumberString_number_dict.Add("Five", 5);
            NumberString_number_dict.Add("Six", 6);
            NumberString_number_dict.Add("Seven", 7);
            NumberString_number_dict.Add("Eight", 8);
            NumberString_number_dict.Add("Nine", 9);
            NumberString_number_dict.Add("Ten", 10);
            NumberString_number_dict.Add("Eleven", 11);
            NumberString_number_dict.Add("Twelve", 12);
            NumberString_number_dict.Add("Thirteen", 13);
            NumberString_number_dict.Add("Fourteen", 14);
            NumberString_number_dict.Add("Fifteen", 15);
            NumberString_number_dict.Add("Sixteen", 16);
            NumberString_number_dict.Add("Seventeen", 17);
            NumberString_number_dict.Add("Eightteen", 18);
            NumberString_number_dict.Add("Nineteen", 19);
            NumberString_number_dict.Add("Twenty", 20);
            NumberString_number_dict.Add("Twentyone", 21);

            Number_numberString_dict = new Dictionary<int, string>();
            string[] numberStrings = NumberString_number_dict.Keys.ToArray();
            foreach (string numberString in numberStrings)
            {
                Number_numberString_dict.Add(NumberString_number_dict[numberString], numberString);
            }
        }

        public int Get_number_for_numberString(string numberString)
        {
            return NumberString_number_dict[numberString];
        }

        public string Get_numberString_for_number(int number)
        {
            return Number_numberString_dict[number];
        }
    }
    class Global_directory_class
    {
        private const string hard_drive = "D:/";
        private const string major_directory = hard_drive + "Lincs_DToxS_SVD/";
        private const string experimental_data_directory_add = "Experimental_data/";
        private const string downloaded_data_subdirectory = "Downloaded_datasets/";
        private const string lincs_final_deg_directory_add = "Degs_final/";
        private const string lincs_nonbinary_deg_directory_add = "Degs_preliminary/";
        private const string lincs_initial_deg_P0_directory_add = "Degs_initial_iPSCdCMs_P0/";
        private const string lincs_initial_deg_EC_directory_add = "Degs_initial_iPSCdCMs_EC/";
        private const string lincs_metadata_subdirectory = "Metadata/";
        private const string mbco_desktop_application_subdirectory_base = "MBCO_desktop_application";
        private const string lincs_directory = major_directory;
        private const string ontology_directory = major_directory + "Ontologies/";
        private const string enrich_libraries_directory = major_directory + "Libraries_for_enrichment/";
        private const string geneDatabases_homology_directory = major_directory + "GeneDatabases_homology/";
        private const string download_subdirectory = "Download/";
        private const string self_subdirectory = "Self/";
        private const string results_subdirectory = "Results/";
        private const string scRNAseq_schaniel_marker_subdirectory = "iPSCdCM_scRNAseq/iPSCd_CM_marker_genes/";
        private const string scRNAseq_litvinkukova_marker_subdirectory = "Litvinukova_2020_cellsAdultHumanHeart/";
        private const string resultsSCSNEnrich_subdirectory = "ScSnRNAseq_enrichment/";
        private const string published_cardiotoxic_variants_subdirectory = "RARG_variant/";
        private const string svd_enrichment_subdirectory = "SVD_outlier_responses/";
        private const string drug_pk_pd_subdirectory = "SVD_drug_target_proteins/";
        private const string supplTables_subdirectory = "SVD_manuscript_supplTables/";
        private const string figures_supplFigures_subdirectory = "SVD_manuscript_figures_supplFigures/";
        private const string genomics_filtered_subdirectory = "Genomics_filtered/";
        private const string whole_genome_sequencing_subdirectory = "Whole_genome_sequencing/";
        private const string eigenarray_subdirectory = "SVD_eigenarrays/";
        private const string results_metadata_subdirectory = "Metadata/";
        public static string Major_directory
        { get { return major_directory; } }
        public static string Ontology_directory
        { get { return ontology_directory; } }
        public static string Lincs_degs_non_binary_files_directory
        { get { return Experimental_data_directory + lincs_nonbinary_deg_directory_add; } }
        public static string Experimental_data_directory
        {
            get
            {
                string directory = lincs_directory + experimental_data_directory_add;
                return directory;
            }
        }
        public static string Experimental_data_whole_genome_sequencing_directory
        { get { return Experimental_data_directory + whole_genome_sequencing_subdirectory; } }
        public static string Published_cardiotoxic_variants_directory
        { get { return Experimental_data_directory + published_cardiotoxic_variants_subdirectory; } }
        public static string Downloaded_data_directory
        {
            get
            {
                string directory = Major_directory + downloaded_data_subdirectory;
                return directory;
            }
        }
        public static string Final_lincs_degs_directory
        {
            get
            {
                return Experimental_data_directory + lincs_final_deg_directory_add;
            }
        }
        public static string Eigenarray_subdirectory
        {
            get { return eigenarray_subdirectory; }
        }
        public static string Metadata_directory
        {
            get
            {
                return Experimental_data_directory + lincs_metadata_subdirectory;
            }
        }
        public static string Get_MBCO_desktop_application_directory(string dataset)
        {
            return Results_directory + mbco_desktop_application_subdirectory_base + "_" + dataset + "/";
        }
        public static string Initial_lincs_degs_P0_directory
        {
            get
            {
                return Experimental_data_directory + lincs_initial_deg_P0_directory_add;
            }
        }
        public static string Initial_lincs_degs_EC_directory
        {
            get
            {
                return Experimental_data_directory + lincs_initial_deg_EC_directory_add;
            }
        }
        public static string Input_networks_directory
        {
            get { return Experimental_data_directory + "Input_networks//"; }
        }
        public static string Enrich_libraries_download_directory
        {
            get { return enrich_libraries_directory + download_subdirectory; }
        }
        public static string Enrich_libraries_self_directory
        {
            get { return enrich_libraries_directory + self_subdirectory; }
        }
        public static string Results_directory
        { get { return lincs_directory + results_subdirectory; } }
        public static string Results_metadata_directory
        { get { return Results_directory + results_metadata_subdirectory; } }
        public static string Results_svd_enrichment_directory
        { get { return Results_directory + svd_enrichment_subdirectory; } }
        public static string Drug_pk_pd_subdirectory
        { get { return drug_pk_pd_subdirectory; } }
        public static string Results_genomics_filtered_subdirectory
        { get { return Drug_pk_pd_subdirectory + genomics_filtered_subdirectory; } }
        public static string SupplTables_subdirectory
        { get { return supplTables_subdirectory; } }
        public static string Figures_supplFigures_subdirectory
        { get { return figures_supplFigures_subdirectory; } }
        public static string Svd_enrichment_subdirectory
        { get { return svd_enrichment_subdirectory; } }
        public static string ScRNAseq_litvinukova_markerGenes_directory
        { get { return lincs_directory + downloaded_data_subdirectory + scRNAseq_litvinkukova_marker_subdirectory; } }
        public static string ScRNAseq_schaniel_markerGenes_directory
        { get { return lincs_directory + scRNAseq_schaniel_marker_subdirectory; } }
        public static string ScSnRNAseq_enrichment_results_directory
        { get { return Results_directory + resultsSCSNEnrich_subdirectory; } }
        public static string GeneDatabases_homology_download_directory { get { return geneDatabases_homology_directory + download_subdirectory; } }
        public static string GeneDatabases_homology_self_directory { get { return geneDatabases_homology_directory + self_subdirectory; } }
    }

    class Report_finished_and_wait_for_r_script_class
    {
        private string report_finished_1st_part_fileName = "Report_finished_1st_part_by_Csharp.txt";
        private string report_finished_2nd_part_fileName = "Report_finished_2nd_part_by_R.txt";
        private string report_finished_3rd_part_fileName = "Report_finished_3rd_part_by_Csharp.txt";
        private string report_finished_4th_part_fileName = "Report_finished_4th_part_by_R.txt";
        private string report_finished_5th_part_fileName = "Report_finished_5th_part_by_Csharp.txt";
        private string report_finished_6th_part_fileName = "Report_finished_6th_part_by_R.txt";
        private int Waiting_time_in_min = 30;

        public void Report_finish_of_first_part()
        {
            string completeFileName = Global_directory_class.Results_directory + report_finished_1st_part_fileName;
            StreamWriter writer = new StreamWriter(completeFileName);
            writer.WriteLine("First part finished by C#.");
            writer.Close();
        }

        public void Wait_for_R_to_finish_second_part()
        {
            string completeFileName = Global_directory_class.Results_directory + report_finished_2nd_part_fileName;
            while (!File.Exists(completeFileName))
            {
                System.Threading.Thread.Sleep(Waiting_time_in_min * 60 * 1000);
            }
        }

        public void Report_finish_of_third_part()
        {
            string completeFileName = Global_directory_class.Results_directory + report_finished_3rd_part_fileName;
            StreamWriter writer = new StreamWriter(completeFileName);
            writer.WriteLine("Third part finished by C#.");
        }

        public void Wait_for_R_to_finish_fourth_part()
        {
            string completeFileName = Global_directory_class.Results_directory + report_finished_4th_part_fileName;
            Stopwatch stopwatch = new Stopwatch();
            while (!File.Exists(completeFileName))
            {
                Report_class.WriteLine_in_progress_report("Waiting for R to finish (waited already for " + stopwatch.Elapsed + ")");
                System.Threading.Thread.Sleep(Waiting_time_in_min * 60 * 1000);
            }
            stopwatch.Stop();
        }
        public void Wait_for_R_to_finish_sixth_part()
        {
            string completeFileName = Global_directory_class.Results_directory + report_finished_6th_part_fileName;
            Stopwatch stopwatch = new Stopwatch();
            while (!File.Exists(completeFileName))
            {
                Report_class.WriteLine_in_progress_report("Waiting for R to finish (waited already for " + stopwatch.Elapsed + ")");
                System.Threading.Thread.Sleep(Waiting_time_in_min * 60 * 1000);
            }
            stopwatch.Stop();
        }
        public void Report_finish_of_fifth_part()
        {
            string completeFileName = Global_directory_class.Results_directory + report_finished_5th_part_fileName;
            StreamWriter writer = new StreamWriter(completeFileName);
            writer.WriteLine("Fifth part finished by C#.");
        }



    }

    public class Conversion_class
    {
        public static string UpDownStatus_columnName { get { return "Up or down"; } }
        public static string No_value_string { get { return "ND"; } }
        public static string Get_upDown_status_from_entryType(DE_entry_enum entryType)
        {
            switch (entryType)
            {
                case DE_entry_enum.Diffrna_up:
                    return "Up";
                case DE_entry_enum.Diffrna_down:
                    return "Down";
                default:
                    throw new Exception();
            }
        }
        public static DE_entry_enum Get_entryType_from_upDown_status(string upDown_status)
        {
            switch (upDown_status)
            {
                case "Up":
                    return DE_entry_enum.Diffrna_up;
                case "Down":
                    return DE_entry_enum.Diffrna_down;
                default:
                    throw new Exception();
            }
        }
        public static string Convert_positive_float_to_string_or_nd(float float_value)
        {
            int max_number_letters = 2;
            string number_string = "";
            int indexPoint = -1;
            int letters_of_full_number = 0;
            if (  (float_value != -1)
                &&(!float.IsNaN(float_value)))
            {
                number_string = float_value.ToString();
                indexPoint = number_string.IndexOf('.');
                if (indexPoint==-1) { letters_of_full_number = number_string.Length; }
                else 
                { 
                    letters_of_full_number = indexPoint; 
                }
                while (letters_of_full_number < max_number_letters)
                {
                    number_string = "0" + number_string;
                    letters_of_full_number++;
                }
            }
            else
            {
                number_string = float.NaN.ToString();//   (string)No_value_string.Clone();
            }
            return number_string;
        }
        public static float Convert_string_or_nd_to_float(string string_value)
        {
            if (string_value.Equals("ND"))
            {
                return float.NaN;
            }
            else
            {
                return float.Parse(string_value);
            }

        }
    }

    public enum Input_data_enum { E_m_p_t_y, Lincs }
    public enum Organism_enum { E_m_p_t_y, Homo_sapiens = 9606, Mus_musculus = 10090, Rattus_norvegicus = 10116 }
    public enum Timepoint_enum {  E_m_p_t_y, H0, H48 }
    public enum SpeciesSwitch_enum { E_m_p_t_y, Homologene, Jackson_labs, Simple_transfer }
    public enum Gene2RefSeq_accession_enum { E_m_p_t_y, Provisional, Validated, Reviewed, Model, Predicted, Inferred, Suppressed, Na }
    public enum Reference_annotation_enum { E_m_p_t_y, Refseq, Ensembl }
    public enum Reference_genome_enum { E_m_p_t_y, GRCh38hg38 }
    public enum yED_colored_property_enum { E_m_p_t_y, NW_classification, Classification, Timepoints, DE, DE_expression, None }
    public enum ReadWrite_report_enum { E_m_p_t_y = 0, Report_main = 1, Report_everything = 2, Report_nothing }
    public enum Node_classification_enum { E_m_p_t_y, Seednode, Nonseednode, Intermediate, Sinknode, Seedsinknode, Regulatory_scp, Scp, Gene, Level_0, Level_1, Level_2, Level_3, Level_4, Level_5 }
    public enum Node_nw_classification_enum { E_m_p_t_y = 0, Bipartite_source = 1, Bipartite_target = 2 }
    public enum Node_de_enum { E_m_p_t_y, Not_DE, Upregulated, Downregulated, Upanddownregulated }
    public enum yED_node_label_enum { E_m_p_t_y, Id, Symbol, Name1, Name2, Name2_plus_integer, Name1_plus_name2, None, Symbol_input }
    public enum Ontology_type_enum
    {
        E_m_p_t_y,
        Molecular_biology_of_the_cell,
        Mbco_level1, Mbco_level2, Mbco_level3, Mbco_level4,
        Chea_2016, Chea_2022, Encode_tf_chip_seq_2015, Transfac_and_jaspar_pwms,Trrust_transcription_factors_2019, Kea_2015,
        Drugbank_drug_targets, Drugbank_enzymes, Drugbank_transporters,
        Tucker_2020_adult_heart_cell_type_marker_genes, Asp_2019_developing_heart_cell_type_marker_genes
    };
    public enum Network_direction_type_enum { E_m_p_t_y, Directed_forward, Undirected_double }
    public enum Network_interaction_type_enum { E_m_p_t_y, Interaction, Activation }
    public enum Ensembl_gene_area_enum { E_m_p_t_y, Exon, Stop_codon, Cds, Start_codon }

    class Ontology_abbreviation_class
    {
        public static Dictionary<Ontology_type_enum, string> Get_ontology_abbreviation_dict()
        {
            string delimiter = "";
            Dictionary<Ontology_type_enum, string> ontology_abbreviation_dict = new Dictionary<Ontology_type_enum, string>();
            ontology_abbreviation_dict.Add(Ontology_type_enum.Mbco_level1,"MBCO" + delimiter + "L1");
            ontology_abbreviation_dict.Add(Ontology_type_enum.Mbco_level2,"MBCO" + delimiter + "L2");
            ontology_abbreviation_dict.Add(Ontology_type_enum.Mbco_level3,"MBCO" + delimiter + "L3");
            ontology_abbreviation_dict.Add(Ontology_type_enum.Mbco_level4,"MBCO" + delimiter + "L4");
            return ontology_abbreviation_dict;
        }
        public static Dictionary<string, Ontology_type_enum> Get_abbreviation_ontology_dict()
        {
            Dictionary<Ontology_type_enum, string> ontology_abbreviation_dict = Get_ontology_abbreviation_dict();
            Ontology_type_enum[] ontologies = ontology_abbreviation_dict.Keys.ToArray();
            Dictionary<string, Ontology_type_enum> abbreviation_ontology_dict = new Dictionary<string, Ontology_type_enum>();
            foreach (Ontology_type_enum ontology in ontologies)
            {
                abbreviation_ontology_dict.Add(ontology_abbreviation_dict[ontology], ontology);
            }
            return abbreviation_ontology_dict;
        }
        public static Ontology_type_enum Get_ontology_of_abbreviation(string abbreviation)
        {
            Dictionary<string, Ontology_type_enum> abbreviation_ontology_dict = Get_abbreviation_ontology_dict();
            return abbreviation_ontology_dict[abbreviation];
        }
        public static string Get_abbreviation_of_ontology(Ontology_type_enum ontology)
        {
            Dictionary<Ontology_type_enum, string> ontology_abbreviation_dict = Get_ontology_abbreviation_dict();
            if (ontology_abbreviation_dict.ContainsKey(ontology))
            { return ontology_abbreviation_dict[ontology]; }
            else { return ontology.ToString(); }
        }
    }

    public enum Deg_score_of_interest_enum
    {
        E_m_p_t_y,
        Signed_minus_log10pvalue,
        Signed_minus_log10pvalue_no1stSVD,
        Control_expression_values
    }


    public enum DE_entry_enum
    {
        E_m_p_t_y, Rna, Diffrna, Diffrna_up, Diffrna_down, Snp, Indel, Various
    }
    public enum Data_value_signs_of_interest_enum { E_m_p_t_y, Upregulated, Downregulated, Combined }
    class Deep_copy_class
    {
        public static string[] Deep_copy_string_array(string[] input)
        {
            int length = input.Length;
            string[] copy = new string[length];
            for (int indexS = 0; indexS < length; indexS++)
            {
                copy[indexS] = (string)input[indexS].Clone();
            }
            return copy;
        }
    }
    class Text2_class
    {
        public static void Set_first_letter_to_uppercase(ref string text)
        {
            int text_length = text.Length;
            text = char.ToUpper(text[0]) + text.Substring(1);
        }
        public static string Remove_space_comma_semicolon_colon_underline_from_end_and_beginning_of_text(string text)
        {
            int text_length = text.Length;
            bool space_comma_semicolon_colon_at_beginning = true;
            bool space_comma_semicolon_colon_at_end = true;
            while (((space_comma_semicolon_colon_at_beginning) || (space_comma_semicolon_colon_at_end)) && ((!String.IsNullOrEmpty(text)) && (text_length >= 2)))
            {
                text_length = text.Length;
                space_comma_semicolon_colon_at_beginning = text[0].Equals(' ') || (text[0].Equals(',')) || (text[0].Equals('_')) || (text[0].Equals(';')) || (text[0].Equals(':'));
                space_comma_semicolon_colon_at_end = (text[text_length - 1].Equals(' ')) || (text[text_length - 1].Equals(',')) || (text[text_length - 1].Equals('_')) || (text[text_length - 1].Equals(';')) || (text[text_length - 1].Equals(':'));
                if (space_comma_semicolon_colon_at_beginning && space_comma_semicolon_colon_at_end)
                {
                    text = text.Substring(1, text_length - 2);
                }
                else if (space_comma_semicolon_colon_at_beginning)
                {
                    text = text.Substring(1, text_length - 1);
                }
                else if (space_comma_semicolon_colon_at_end)
                {
                    text = text.Substring(0, text_length - 1);
                }
            }
            return text;
        }
    }

    class Array_class
    {
        public static bool Array_order_dependent_equal<T>(T[] array1, T[] array2)
        {
            bool equal = true;
            int array1_length = array1.Length;
            int array2_length = array2.Length;
            if (array1_length != array2_length)
            {
                equal = false;
            }
            else
            {
                for (int indexA = 0; indexA < array1_length; indexA++)
                {
                    if (!array1[indexA].Equals(array2[indexA]))
                    {
                        equal = false;
                        break;
                    }
                }
            }
            return equal;
        }
        public static string[] Deep_copy_string_array(string[] array)
        {
            int array_length = array.Length;
            string[] copy = new string[0];
            if (array_length > 0)
            {
                copy = new string[array_length];
                for (int indexA = 0; indexA < array_length; indexA++)
                {
                    copy[indexA] = (string)array[indexA].Clone();
                }
            }
            return copy;
        }
        public static T[] Deep_copy_array<T>(T[] array)
        {
            T[] copy = new T[0];
            if (typeof(T) == typeof(string))
            {
                throw new Exception();
            }
            else if (array != null)
            {
                int array_length = array.Length;
                copy = new T[array_length];
                for (int indexA = 0; indexA < array_length; indexA++)
                {
                    copy[indexA] = (T)array[indexA];
                }
            }
            else
            {
                copy = array;
            }
            return copy;
        }
        public static bool Equal_arrays<T>(T[] array1, T[] array2) where T : IComparable<T>
        {
            int array1_length = array1.Length;
            int array2_length = array2.Length;
            bool equal = true;
            if (array1_length != array2_length)
            {
                equal = false;
            }
            else
            {
                for (int indexA = 0; indexA < array1_length; indexA++)
                {
                    if (!array1[indexA].Equals(array2[indexA]))
                    {
                        equal = false;
                        break;
                    }
                }
            }
            return equal;
        }
    }

    class Report_class
    {
        static string File_name = Global_directory_class.Results_directory + "Report_output.txt";
        static string Progress_report_fileName = Global_directory_class.Results_directory + "Progress_report.txt";
        static string Error_file_name = Global_directory_class.Results_directory + "Report_output.txt";
        public static void Write_analysis_start()
        {
            Report_class.WriteLine("-------------------------------------------------------------------------------");
            Report_class.WriteLine("-------------------------------------------------------------------------------");
            Report_class.WriteLine("Start analysis, {0}, {1}", DateTime.Now.ToString("yyyy.MM.dd"), string.Format("{0:HH:mm:ss tt}", DateTime.Now));
        }
        public static void Write_analysis_end()
        {
            Report_class.WriteLine("Analysis finished, {0}, {1}", DateTime.Now.ToString("yyyy.MM.dd"), string.Format("{0:HH:mm:ss tt}", DateTime.Now));
            Report_class.WriteLine("-------------------------------------------------------------------------------");
            Report_class.WriteLine("-------------------------------------------------------------------------------");
        }
        private static string Generate_singleString(string original_string, params object[] add)
        {
            StringBuilder sb = new StringBuilder();
            int add_length = add.Length;
            int firstIndex_openCurley_bracket = original_string.IndexOf("{"); 
            int firstIndex_closedCurley_bracket = original_string.IndexOf("}");
            if (firstIndex_openCurley_bracket != -1)
            {
                string number_string_between_curleyBrackets;
                int array_index;
                string string_before_first_openCurleyBracket;
                string string_after_first_closedCurleyBracket = "error";
                while (firstIndex_openCurley_bracket != -1)
                {
                    number_string_between_curleyBrackets = original_string.Substring(firstIndex_openCurley_bracket + 1, firstIndex_closedCurley_bracket - firstIndex_openCurley_bracket - 1);
                    array_index = int.Parse(number_string_between_curleyBrackets);
                    string_before_first_openCurleyBracket = original_string.Substring(0, firstIndex_openCurley_bracket);
                    string_after_first_closedCurleyBracket = original_string.Substring(firstIndex_closedCurley_bracket + 1, original_string.Length - firstIndex_closedCurley_bracket - 1);
                    sb.AppendFormat("{0}{1}", string_before_first_openCurleyBracket, (string)add[array_index]);
                    original_string = string_after_first_closedCurleyBracket;
                    firstIndex_openCurley_bracket = original_string.IndexOf("{");
                    firstIndex_closedCurley_bracket = original_string.IndexOf("}");
                }
                sb.AppendFormat("{0}", string_after_first_closedCurleyBracket);
                return sb.ToString();
            }
            else
            {
                return original_string;
            }
        }
        public static void Write(string text, params string[] add)
        {
            StreamWriter writer = new StreamWriter(File_name, true);
            writer.Write(text, add);
            string writeString = Generate_singleString(text, add);
            System.Diagnostics.Debug.Write(writeString);
            writer.Close();
        }
        public static void Write(params object[] add)
        {
            StreamWriter writer = new StreamWriter(File_name, true);
            writer.Write(add);
            System.Diagnostics.Debug.Write(add);
            writer.Close();
        }
        public static void Write_major_line(string text, params object[] add)
        {
            Report_class.WriteLine();
            Report_class.WriteLine("*******************************************************************************");
            Report_class.WriteLine(text, add);
            int object_length = 0;
            foreach (object add_object in add)
            {
                object_length = +add_object.ToString().Length;
            }
            for (int i = 0; i < text.Length + object_length; i++) { Report_class.Write("*"); }
            Report_class.WriteLine();
        }
        public static void Write_major_line(string text)
        {
            Report_class.WriteLine();
            Report_class.WriteLine("*******************************************************************************");
            Report_class.WriteLine(text);
            for (int i = 0; i < text.Length; i++) { Report_class.Write("*"); }
            Report_class.WriteLine();
        }
        public static void Write_notation(string text, params object[] add)
        {
            StreamWriter writer = new StreamWriter(File_name, true);
            writer.Write(text);
            writer.Write(add);
            System.Diagnostics.Debug.Write(add);
            writer.Close();
        }
        public static void WriteLine()
        {
            System.Diagnostics.Debug.WriteLine("");
        }
        public static void WriteLine(string text, params object[] add)
        {
            StreamWriter writer = new StreamWriter(File_name, true);
            if ((add != null) && (add.Length > 0))
            {
                writer.WriteLine(text, add);
                //System.Diagnostics.Debug.WriteLine(text, add);
            }
            else
            {
                writer.WriteLine(text);
                //System.Diagnostics.Debug.WriteLine(text);
            }
            writer.Close();
        }
        public static void WriteLine_in_progress_report(string text, params object[] add)
        {
            string time = DateTime.Now.ToString();
            string write_text = time + ": " + text;
            StreamWriter writer = new StreamWriter(Progress_report_fileName, true);
            if ((add != null) && (add.Length > 0))
            {
                writer.WriteLine(write_text, add);
            }
            else
            {
                writer.WriteLine(write_text);
            }
            writer.Close();
        }
        public static void WriteLine(params object[] add)
        {
            StreamWriter writer = new StreamWriter(File_name, true);
            writer.WriteLine(add);
            System.Diagnostics.Debug.WriteLine(add);
            writer.Close();
        }
    }
    class Hexadecimal_color_class
    {
        public static string Get_hexadecimal_code_for_color(System.Drawing.Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }



}
