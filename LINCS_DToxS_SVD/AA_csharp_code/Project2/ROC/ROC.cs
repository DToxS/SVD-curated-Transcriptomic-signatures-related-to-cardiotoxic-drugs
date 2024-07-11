using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Highthroughput_data;
using Common_classes;
using ReadWrite;
using Enrichment;

namespace Roc
{
    enum Pairwise_enrichment_score_of_interest_enum { E_m_p_t_y, Minus_log10_pvalue_at_fixed_position, Sum_of_minuslog10_pvalues, Sum_of_singlegene_minuslog10_pvalues, Normalized_sum_of_minuslog10_pvalues, Rank_sum_of_minuslog10_pvalues, Percent_of_max_sum_of_minuslog10_pvalues };
    enum DownstreanEnrichment_score_of_interest_enum { E_m_p_t_y, Minus_log10_pvalue, Fractional_rank }

    ///////////////////////////////////////////////////////////////////////////////////////

    class Roc_line_class
    {
        public Pairwise_enrichment_score_of_interest_enum Pairwise_score_of_interest { get; set; }
        public string Side_effect { get; set; }
        public string[] Disease_neighborhoods { get; set; }
        public double Roc_cutoff { get; set; }
        public int Pairwise_cutoff { get; set; }
        public int DEGs_minimum { get; set; }
        public int True_positives_length { get; set; }
        public int False_positives_length { get; set; }
        public int True_negatives_length { get; set; }
        public int False_negatives_length { get; set; }
        public float True_positive_rate { get; set; }
        public float False_positive_rate { get; set; }
        public DE_entry_enum Entry_type { get; set; }
        public string[] New_true_positives { get; set; }
        public string[] New_false_positives { get; set; }
        public int Total_samples_count { get; set; }
        public float Precision { get; set; }
        public float Recall { get; set; }
        public float F1_score { get; set; }
        public float F1_score_beta { get; set; }
        public float F1_score_AUC { get; set; }
        public string F1_score_AUC_for_FDA
        {
            get { return Conversion_class.Convert_positive_float_to_string_or_nd(F1_score_AUC); }
            set { F1_score_AUC = Conversion_class.Convert_string_or_nd_to_float(value); }
        }
        public float F1_score_AUC_rank { get; set; }
        public string F1_score_AUC_rank_for_FDA
        {
            get { return Conversion_class.Convert_positive_float_to_string_or_nd(F1_score_AUC_rank); }
            set { F1_score_AUC_rank = Conversion_class.Convert_string_or_nd_to_float(value); }
        }

        public string Up_down_status
        {
            get { return Conversion_class.Get_upDown_status_from_entryType(Entry_type); }
            set { Entry_type = Conversion_class.Get_entryType_from_upDown_status(value); }
        }

        public string ReadWrite_new_true_positives
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.New_true_positives, Roc_readWriteOptions.Array_delimiter); }
            set { this.New_true_positives = ReadWriteClass.Get_array_from_readLine<string>(value, Roc_readWriteOptions.Array_delimiter); }
        }

        public string ReadWrite_new_false_positives
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.New_false_positives, Roc_readWriteOptions.Array_delimiter); }
            set { this.New_false_positives = ReadWriteClass.Get_array_from_readLine<string>(value, Roc_readWriteOptions.Array_delimiter); }
        }

        public string ReadWrite_disease_neighborhoods
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Disease_neighborhoods, Roc_readWriteOptions.Array_delimiter); }
            set { this.Disease_neighborhoods = ReadWriteClass.Get_array_from_readLine<string>(value, Roc_readWriteOptions.Array_delimiter); }
        }

        public Roc_line_class()
        {
            this.Precision = -1;
            this.Recall = -1;
            this.F1_score = -1;
            this.F1_score_AUC = -1;
            this.F1_score_beta = -1;
        }

        public Roc_line_class Deep_copy()
        {
            Roc_line_class copy = (Roc_line_class)this.MemberwiseClone();
            copy.New_true_positives = Array_class.Deep_copy_string_array(this.New_true_positives);
            copy.New_false_positives = Array_class.Deep_copy_string_array(this.New_false_positives);
            copy.Disease_neighborhoods = Array_class.Deep_copy_string_array(this.Disease_neighborhoods);
            return copy;
        }
    }

    class Roc_readWriteOptions : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Roc_readWriteOptions(string subdirectory, string filename)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + filename;
            this.Key_propertyNames = new string[] { "Side_effect", "ReadWrite_disease_neighborhoods", "Entry_type", "Pairwise_score_of_interest", "Roc_cutoff", "Pairwise_cutoff", "DEGs_minimum", "True_positives_length", "False_positives_length", "True_negatives_length", "False_negatives_length", "False_positive_rate", "True_positive_rate", "Precision", "Recall", "Total_samples_count", "ReadWrite_new_true_positives", "ReadWrite_new_false_positives" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Roc_for_website_readWriteOptions : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Roc_for_website_readWriteOptions(string subdirectory, string filename, float beta)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + filename;
            this.Key_propertyNames = new string[] { "ReadWrite_disease_neighborhoods", "Up_down_status", "Roc_cutoff", "Precision", "Recall", "F1_score", "F1_score_AUC_for_FDA", "F1_score_AUC_rank_for_FDA", "ReadWrite_new_true_positives", "ReadWrite_new_false_positives" };
            this.Key_columnNames = new string[] { "Pathway", "Up or down", "Enrichment rank cutoff for predicted positives", "Precision", "Recall", "F1 score (beta=" + beta + ")", "F1 score AUC (corrected)", "F1 score AUC rank", "Cardiotoxic TKIs added at rank cutoff", "Non cardiotoxic TKIs added at rank cutoff" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Drug_leaflet_line_class
    {
        public string Side_effect { get; set; }
        public string Drug { get; set; }
        public bool Creates_side_effect { get; set; }
        public float Risk_score { get; set; }

        public Drug_leaflet_line_class Deep_copy()
        {
            Drug_leaflet_line_class copy = (Drug_leaflet_line_class)this.MemberwiseClone();
            copy.Side_effect = (string)this.Side_effect.Clone();
            copy.Drug = (string)this.Drug.Clone();
            return copy;
        }
    }

    class DiseaseNeighborhood_sideEffect_line_class
    {
        public string Side_effect { get; set; }
        public string Disease_neighorhood { get; set; }

        public DiseaseNeighborhood_sideEffect_line_class Deep_copy()
        {
            DiseaseNeighborhood_sideEffect_line_class copy = (DiseaseNeighborhood_sideEffect_line_class)this.MemberwiseClone();
            copy.Side_effect = (string)this.Side_effect.Clone();
            copy.Disease_neighorhood = (string)this.Disease_neighorhood.Clone();
            return copy;
        }
    }

    class Drug_leaflet_readWriteOptions : ReadWriteOptions_base
    {
        public Drug_leaflet_readWriteOptions(string filename)
        {
            this.File = Global_directory_class.Results_directory + filename;
            this.Key_propertyNames = new string[] { "Side_effect", "Drug", "Creates_side_effect", "Risk_score" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Drug_leaflet_class
    {
        public Drug_leaflet_line_class[] Leaflets { get; set; }

        public Drug_leaflet_class()
        {
        }

        private void Add_to_array(Drug_leaflet_line_class[] add_leaflets)
        {
            int add_length = add_leaflets.Length;
            int this_length = this.Leaflets.Length;
            int new_length = add_length + this_length;
            Drug_leaflet_line_class[] new_leaflets = new Drug_leaflet_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_leaflets[indexNew] = this.Leaflets[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_leaflets[indexNew] = add_leaflets[indexAdd];
            }
            this.Leaflets = new_leaflets;
        }

        public string[] Get_all_drugs()
        {
            int leaflets_length = this.Leaflets.Length;
            Drug_leaflet_line_class drug_leaflet_line;
            List<string> drugs = new List<string>();
            this.Leaflets = this.Leaflets.OrderBy(l => l.Drug).ToArray();
            for (int indexL = 0; indexL < leaflets_length; indexL++)
            {
                drug_leaflet_line = this.Leaflets[indexL];
                if ((indexL == 0) || (!drug_leaflet_line.Drug.Equals(this.Leaflets[indexL - 1].Drug)))
                {
                    drugs.Add(drug_leaflet_line.Drug);
                }
            }
            return drugs.ToArray();
        }

        private void Check_for_duplicates(Drug_leaflet_line_class[] leaflets)
        {
            leaflets = leaflets.OrderBy(l => l.Side_effect).ThenBy(l => l.Drug).ToArray();
            int leaflets_length = leaflets.Length;
            Drug_leaflet_line_class leaflet_line;
            for (int indexL = 0; indexL < leaflets_length; indexL++)
            {
                leaflet_line = leaflets[indexL];
                if ((indexL != 0)
                    && (leaflet_line.Side_effect.Equals(leaflets[indexL - 1].Side_effect))
                    && (leaflet_line.Drug.Equals(leaflets[indexL - 1].Drug)))
                {
                    throw new Exception();
                }
            }
        }

        public void Generate()
        {
            List<Drug_leaflet_line_class> leaflet_list = new List<Drug_leaflet_line_class>();
            Drug_leaflet_line_class leaflet_line;

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "DAS";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "VAN";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "DAB";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "SOR";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "PAZ";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TRA";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "AFA";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "PON";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "SUN";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TRS";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "LAP";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "IMA";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "ERL";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "BOS";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "NIL";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "RUX";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "VEM";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "CER";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "CAB";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "REG";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TOF";
            leaflet_line.Side_effect = "Cardiomyopathy - jens";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);




            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "DAS";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "VAN";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "DAB";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "SOR";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "PAZ";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TRA";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "AFA";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "PON";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "SUN";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TRS";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "LAP";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "IMA";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "ERL";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "BOS";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "NIL";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "RUX";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "VEM";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "CER";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "REG";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TOF";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "GEF";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor, Gharwan
            leaflet_line.Drug = "AXI";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor, Gharwan
            leaflet_line.Drug = "CAB";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor, Gharwan
            leaflet_line.Drug = "CRI";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor, Gharwan
            leaflet_line.Drug = "IBR";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor, Gharwan
            leaflet_line.Drug = "LEN";
            leaflet_line.Side_effect = "Cardiomyopathy - jaehee";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);




            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "AFA";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "AXI";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "BOS";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "DAS";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "ERL";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "GEF";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "IMA";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "LAP";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "PON";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "REG";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "RUX";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "SOR";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "SUN";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TOF";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TRA";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "VAN";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "VEM";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            #region compare cardiomyopathy messinis and jaehee
            List<Drug_leaflet_line_class> combined_jaehee_messinis_leaflet_list = new List<Drug_leaflet_line_class>();
            foreach (Drug_leaflet_line_class leaflet_line_to_be_combined in leaflet_list)
            {
                if ((leaflet_line_to_be_combined.Side_effect.Equals("Cardiomyopathy - messinis"))
                    || (leaflet_line_to_be_combined.Side_effect.Equals("Cardiomyopathy - jaehee")))
                {
                    combined_jaehee_messinis_leaflet_list.Add(leaflet_line_to_be_combined.Deep_copy());
                }
            }
            combined_jaehee_messinis_leaflet_list = combined_jaehee_messinis_leaflet_list.OrderBy(l => l.Drug).ToList();
            List<Drug_leaflet_line_class> final_jaehee_messinis_leaflet_list = new List<Drug_leaflet_line_class>();
            Drug_leaflet_line_class new_leaflet_line;
            int combined_length = combined_jaehee_messinis_leaflet_list.Count;
            for (int indexC = 0; indexC < combined_length; indexC++)
            {
                leaflet_line = combined_jaehee_messinis_leaflet_list[indexC];
                if ((indexC == combined_length - 1)
                    || (!leaflet_line.Drug.Equals(combined_jaehee_messinis_leaflet_list[indexC + 1].Drug)))
                {
                    new_leaflet_line = leaflet_line.Deep_copy();
                    new_leaflet_line.Side_effect = "Cardiomyopathy - jaehee + messinis";
                    final_jaehee_messinis_leaflet_list.Add(new_leaflet_line);
                }
                else if (leaflet_line.Creates_side_effect)
                {
                    combined_jaehee_messinis_leaflet_list[indexC + 1].Creates_side_effect = true;
                }
            }
            #endregion

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "AFA";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "AXI";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "BOS";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "DAS";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "ERL";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "GEF";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "IMA";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "LAP";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "PON";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "REG";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "RUX";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "SOR";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "SUN";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TOF";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TRA";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "VAN";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "VEM";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "CAB";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "CER";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "DAB";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "NIL";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "PAZ";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "TRS";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = true;
            leaflet_list.Add(leaflet_line);

            leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            leaflet_line.Drug = "CRI";
            leaflet_line.Side_effect = "Cardiomyopathy - messinis expected after extension";
            leaflet_line.Creates_side_effect = false;
            leaflet_list.Add(leaflet_line);



            Check_for_duplicates(final_jaehee_messinis_leaflet_list.ToArray());
            leaflet_list.AddRange(final_jaehee_messinis_leaflet_list);
            Check_for_duplicates(leaflet_list.ToArray());
            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "DAS";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "VAN";//no by Shim et al
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "DAB";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "SOR";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "PAZ";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "TRA";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "AFA";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "PON";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "SUN";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "TRS";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "LAP"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed"; //
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "IMA"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "DAS"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "VAN"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "SOR"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "BOS"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "NIL"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "PAZ"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "VEM"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "PON"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "SUN"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "Prolonged QT - dailymed";
            //leaflet_list.Add(leaflet_line);





            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "DAS";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "VAN";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "DAB";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "SOR";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "PAZ";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "TRA";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "AFA";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "PON";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "SUN";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "TRS";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "LAP"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "IMA"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "ERL";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "BOS";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "NIL";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "RUX";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "VEM";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "CER"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "CAB";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "REG";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "TOF"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);
            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "DAS";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "VAN";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "DAB";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "SOR";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "PAZ";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "TRA";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "AFA";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "PON";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "SUN";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "TRS";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "LAP"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "IMA"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "ERL";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "BOS";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "NIL";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "RUX";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "VEM";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "CER"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "CAB";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "REG";
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //kinase inhibitor
            //leaflet_line.Drug = "TOF"; //missing in excel spreadsheet
            //leaflet_line.Side_effect = "All KIs";
            //leaflet_list.Add(leaflet_line);



            //leaflet_line = new Drug_leaflet_line_class(); //control drug
            //leaflet_line.Drug = "EST";
            //leaflet_line.Side_effect = "No cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //control drug
            //leaflet_line.Drug = "PRE";
            //leaflet_line.Side_effect = "No cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //control drug
            //leaflet_line.Drug = "IGF";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed"; //reference publications
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //control drug
            //leaflet_line.Drug = "TGF";
            //leaflet_line.Side_effect = "No cardiomyopathy - dailymed";
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //control drug
            //leaflet_line.Drug = "END";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed"; //reference publications
            //leaflet_list.Add(leaflet_line);

            //leaflet_line = new Drug_leaflet_line_class(); //control drug
            //leaflet_line.Drug = "TNF";
            //leaflet_line.Side_effect = "Cardiomyopathy - dailymed"; //reference publications
            //leaflet_list.Add(leaflet_line);

            this.Leaflets = leaflet_list.ToArray();
            int leaflets_length = this.Leaflets.Length;
            for (int indexL = 0; indexL < leaflets_length; indexL++)
            {
                leaflet_line = this.Leaflets[indexL];
                if (leaflet_line.Creates_side_effect)
                {
                    leaflet_line.Risk_score = 10;
                }
                else
                {
                    leaflet_line.Risk_score = -10;
                }
            }
        }

        public void Generate_from_drug_legend(Deg_drug_legend_class drug_legend)
        {
            int drug_legend_length = drug_legend.Legend.Length;
            Deg_drug_legend_line_class drug_legend_line;
            Drug_leaflet_line_class drug_leaflet_line;
            List<Drug_leaflet_line_class> drug_leaflets = new List<Drug_leaflet_line_class>();
            for (int indexDrug = 0; indexDrug < drug_legend_length; indexDrug++)
            {
                drug_legend_line = drug_legend.Legend[indexDrug];
                drug_leaflet_line = new Drug_leaflet_line_class();
                drug_leaflet_line.Drug = (string)drug_legend_line.Drug.Clone();
                drug_leaflet_line.Side_effect = Drug_type_enum.Anthracycline.ToString();
                drug_leaflet_line.Creates_side_effect = drug_legend_line.Drug_type.Equals(Drug_type_enum.Anthracycline);
                drug_leaflets.Add(drug_leaflet_line);
                drug_legend_line = drug_legend.Legend[indexDrug];
                drug_leaflet_line = new Drug_leaflet_line_class();
                drug_leaflet_line.Drug = (string)drug_legend_line.Drug.Clone();
                drug_leaflet_line.Side_effect = Drug_type_enum.Kinase_inhibitor.ToString() + " or " + Drug_type_enum.Monoclonal_antibody.ToString();
                drug_leaflet_line.Creates_side_effect = drug_legend_line.Drug_type.Equals(Drug_type_enum.Kinase_inhibitor) || drug_legend_line.Drug_type.Equals(Drug_type_enum.Monoclonal_antibody);
                drug_leaflets.Add(drug_leaflet_line);
                drug_leaflet_line = new Drug_leaflet_line_class();
                drug_leaflet_line.Drug = (string)drug_legend_line.Drug.Clone();
                drug_leaflet_line.Side_effect = Drug_type_enum.Kinase_inhibitor.ToString();
                drug_leaflet_line.Creates_side_effect = drug_legend_line.Drug_type.Equals(Drug_type_enum.Kinase_inhibitor);
                drug_leaflets.Add(drug_leaflet_line);
                drug_leaflet_line = new Drug_leaflet_line_class();
                drug_leaflet_line.Drug = (string)drug_legend_line.Drug.Clone();
                drug_leaflet_line.Side_effect = Drug_type_enum.Monoclonal_antibody.ToString();
                drug_leaflet_line.Creates_side_effect = drug_legend_line.Drug_type.Equals(Drug_type_enum.Monoclonal_antibody);
                drug_leaflets.Add(drug_leaflet_line);

                if (drug_legend_line.Drug_type.Equals(Drug_type_enum.Kinase_inhibitor)
                    || drug_legend_line.Drug_type.Equals(Drug_type_enum.Monoclonal_antibody))
                {
                    drug_leaflet_line = new Drug_leaflet_line_class();
                    drug_leaflet_line.Drug = (string)drug_legend_line.Drug.Clone();
                    drug_leaflet_line.Side_effect = "Cardiotoxicity";
                    if (drug_legend_line.Is_cardiotoxic_TKI)
                    {
                        drug_leaflet_line.Creates_side_effect = true;
                    }
                    else if (drug_legend_line.Is_noncardiotoxic_TKI)
                    {
                        drug_leaflet_line.Creates_side_effect = false;
                    }
                    else { throw new Exception(); }
                    drug_leaflets.Add(drug_leaflet_line);
                }
                if (   drug_legend_line.Drug_type.Equals(Drug_type_enum.Kinase_inhibitor)
                    || drug_legend_line.Drug_type.Equals(Drug_type_enum.Monoclonal_antibody)
                    || drug_legend_line.Drug_type.Equals(Drug_type_enum.Anthracycline))
                {
                    drug_leaflet_line = new Drug_leaflet_line_class();
                    drug_leaflet_line.Drug = (string)drug_legend_line.Drug.Clone();
                    drug_leaflet_line.Side_effect = "ATC vs high tox. TKI";
                    if (  (  drug_legend_line.Drug_type.Equals(Drug_type_enum.Kinase_inhibitor)
                           ||drug_legend_line.Drug_type.Equals(Drug_type_enum.Monoclonal_antibody))
                        &&(drug_legend_line.Cardiotoxicity_frequencyGroup==3))

                    {
                        drug_leaflet_line.Creates_side_effect = false;
                    }
                    else if (drug_legend_line.Drug_type.Equals(Drug_type_enum.Anthracycline))
                    {
                        drug_leaflet_line.Creates_side_effect = true;
                    }
                    //else { throw new Exception(); }
                    drug_leaflets.Add(drug_leaflet_line);
                }
            }
            this.Leaflets = drug_leaflets.ToArray();
        }
        private string[] Get_all_unique_sideEffects()
        {
            List<string> sideEffects_list = new List<string>();
            int leaflets_length = this.Leaflets.Length;
            this.Leaflets = this.Leaflets.OrderBy(l => l.Side_effect).ToArray();
            Drug_leaflet_line_class leaflet_line;
            for (int indexL = 0; indexL < leaflets_length; indexL++)
            {
                leaflet_line = this.Leaflets[indexL];
                if ((indexL == 0) || (!leaflet_line.Side_effect.Equals(this.Leaflets[indexL - 1].Side_effect)))
                {
                    sideEffects_list.Add(leaflet_line.Side_effect);
                }
            }
            return sideEffects_list.ToArray();
        }

        public void Write_leaflets(string fileName)
        {
            Drug_leaflet_readWriteOptions readWriteOptions = new Drug_leaflet_readWriteOptions(fileName);
            ReadWriteClass.WriteData(this.Leaflets, readWriteOptions);

            string complete_fileName = Global_directory_class.Results_directory + Path.GetFileNameWithoutExtension(fileName) + "_table.txt";
            StreamWriter writer = new StreamWriter(complete_fileName);
            string[] sideEffects = Get_all_unique_sideEffects();
            string sideEffect;
            sideEffects = sideEffects.OrderBy(l => l).ToArray();
            int sideEffects_length = sideEffects.Length;
            writer.Write("Drug");
            for (int indexSE2 = 0; indexSE2 < sideEffects_length; indexSE2++)
            {
                writer.Write("{0}{1}", Global_class.Tab, sideEffects[indexSE2]);
            }
            writer.WriteLine();
            int indexSE = 0;
            int leaflets_length = this.Leaflets.Length;
            this.Leaflets = this.Leaflets.OrderBy(l => l.Drug).ThenBy(l => l.Side_effect).ToArray();
            Drug_leaflet_line_class leaflet_line;
            int stringCompare = -2;
            for (int indexL = 0; indexL < leaflets_length; indexL++)
            {
                leaflet_line = this.Leaflets[indexL];
                if ((indexL == 0) || (!leaflet_line.Drug.Equals(this.Leaflets[indexL - 1].Drug)))
                {
                    indexSE = 0;
                    writer.Write(leaflet_line.Drug);
                }
                stringCompare = -2;
                while ((stringCompare < 0))
                {
                    sideEffect = sideEffects[indexSE];
                    stringCompare = sideEffect.CompareTo(leaflet_line.Side_effect);
                    if (stringCompare < 0)
                    {
                        indexSE++;
                        writer.Write("{0}Not annotated", Global_class.Tab);
                    }
                }
                if (stringCompare != 0) { throw new Exception(); }
                writer.Write("{0}{1}", Global_class.Tab, leaflet_line.Risk_score);
                indexSE++;
                if ((indexL == leaflets_length - 1) || (!leaflet_line.Drug.Equals(this.Leaflets[indexL + 1].Drug)))
                {
                    while (indexSE < sideEffects_length)
                    {
                        indexSE++;
                        writer.Write("{0}Not annotated", Global_class.Tab);
                    }
                    writer.WriteLine();
                }
            }
            writer.Close();
        }
    }

    class Roc_options_class
    {
        public Pairwise_enrichment_score_of_interest_enum Pairwise_score_of_interest { get; set; }
        public DownstreanEnrichment_score_of_interest_enum DownstreamEnrichment_score_of_interest { get; set; }

        public Roc_options_class()
        {
            Pairwise_score_of_interest = Pairwise_enrichment_score_of_interest_enum.Rank_sum_of_minuslog10_pvalues;
            DownstreamEnrichment_score_of_interest = DownstreanEnrichment_score_of_interest_enum.Fractional_rank;
        }

    }

    class Roc_screen_line_class
    {
        public string Drug { get; set; }
        public string Cell_line { get; set; }
        public double Value { get; set; }
        public bool True_positive { get; set; }

        public Roc_screen_line_class Deep_copy()
        {
            Roc_screen_line_class copy = (Roc_screen_line_class)this.MemberwiseClone();
            copy.Drug = (string)this.Drug.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            return copy;
        }
    }

    class Roc_class
    {
        public Drug_leaflet_class Leaflet { get; set; }
        public Roc_line_class[] Roc { get; set; }
        public Roc_options_class Options { get; set; }

        public Roc_class()
        {
            Leaflet = new Drug_leaflet_class();
            this.Options = new Roc_options_class();
            this.Roc = new Roc_line_class[0];
        }

        public void Generate(string[] diseaseNeighborhoods)
        {
            Leaflet.Generate();
        }

        private void Add_to_array(Roc_line_class[] add_roc)
        {
            int this_length = this.Roc.Length;
            int add_length = add_roc.Length;
            int new_length = this_length + add_length;
            Roc_line_class[] new_roc = new Roc_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_roc[indexNew] = this.Roc[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_roc[indexNew] = add_roc[indexAdd];
            }
            this.Roc = new_roc;
        }

        #region Generate from enrichment results
        private string[] Get_all_ordered_distinct_scps(Enrichment2018_results_line_class[] sameScpEntryType_enrichment_result_lines)
        {
            Dictionary<string, bool> scps = new Dictionary<string, bool>();
            foreach (Enrichment2018_results_line_class enrichment_line in sameScpEntryType_enrichment_result_lines)
            {
                if (!scps.ContainsKey(enrichment_line.Scp)) { scps.Add(enrichment_line.Scp, true); }
            }
            return scps.Keys.ToArray();
        }
        private Roc_screen_line_class[] Generate_sorted_screen_lines_from_downstreamEnrichmentResults_and_total_positive_count(Enrichment2018_results_line_class[] sameScpEntryType_enrichment_result_lines, Dictionary<string, bool> sameSideEffect_drugs_dict, out int total_positive_count)
        {
            DE_entry_enum entryType = sameScpEntryType_enrichment_result_lines[0].Sample_entryType;
            DownstreanEnrichment_score_of_interest_enum enrichment_score_of_interest = Options.DownstreamEnrichment_score_of_interest;
            int significant_length = sameScpEntryType_enrichment_result_lines.Length;
            Enrichment2018_results_line_class enrichment_results_line;

            switch (enrichment_score_of_interest)
            {
                case DownstreanEnrichment_score_of_interest_enum.Minus_log10_pvalue:
                    sameScpEntryType_enrichment_result_lines = sameScpEntryType_enrichment_result_lines.OrderByDescending(l => l.Minus_log10_pvalue).ToArray();
                    break;
                case DownstreanEnrichment_score_of_interest_enum.Fractional_rank:
                    sameScpEntryType_enrichment_result_lines = sameScpEntryType_enrichment_result_lines.OrderBy(l => l.Fractional_rank).ToArray();
                    break;
                default:
                    throw new Exception();
            }

            Roc_screen_line_class roc_screen_line;
            List<Roc_screen_line_class> roc_screen_list = new List<Roc_screen_line_class>();
            Dictionary<string, Dictionary<string, int>> already_considered_drugs_subjects_dict = new Dictionary<string, Dictionary<string, int>>();
            total_positive_count = 0;
            Dictionary<string, bool> drug1_considered_dict = new Dictionary<string, bool>();
            for (int indexSig = 0; indexSig < significant_length; indexSig++)
            {
                enrichment_results_line = sameScpEntryType_enrichment_result_lines[indexSig];
                if (!enrichment_results_line.Sample_entryType.Equals(entryType)) { throw new Exception(); }
                if (sameSideEffect_drugs_dict.ContainsKey(enrichment_results_line.Lincs_drug))
                {
                    if (!already_considered_drugs_subjects_dict.ContainsKey(enrichment_results_line.Lincs_drug))
                    {
                        already_considered_drugs_subjects_dict.Add(enrichment_results_line.Lincs_drug, new Dictionary<string, int>());
                    }
                    if (!already_considered_drugs_subjects_dict[enrichment_results_line.Lincs_drug].ContainsKey(enrichment_results_line.Lincs_cell_line))
                    {
                        already_considered_drugs_subjects_dict[enrichment_results_line.Lincs_drug].Add(enrichment_results_line.Lincs_cell_line, indexSig);
                    }
                    else
                    {
                        Enrichment2018_results_line_class other_enrichment_results_line = sameScpEntryType_enrichment_result_lines[already_considered_drugs_subjects_dict[enrichment_results_line.Lincs_drug][enrichment_results_line.Lincs_cell_line]];
                        throw new Exception();
                    }
                    roc_screen_line = new Roc_screen_line_class();
                    roc_screen_line.Drug = (string)enrichment_results_line.Lincs_drug.Clone();
                    roc_screen_line.Cell_line = (string)enrichment_results_line.Lincs_cell_line.Clone();
                    switch (enrichment_score_of_interest)
                    {
                        case DownstreanEnrichment_score_of_interest_enum.Minus_log10_pvalue:
                            roc_screen_line.Value = enrichment_results_line.Minus_log10_pvalue;
                            break;
                        case DownstreanEnrichment_score_of_interest_enum.Fractional_rank:
                            roc_screen_line.Value = enrichment_results_line.Fractional_rank;
                            break;
                        default:
                            throw new Exception();
                    }
                    roc_screen_line.True_positive = sameSideEffect_drugs_dict[enrichment_results_line.Lincs_drug];
                    if (roc_screen_line.True_positive)
                    {
                        total_positive_count++;
                    }
                    roc_screen_list.Add(roc_screen_line);
                }
            }
            return roc_screen_list.ToArray();
        }
        private Roc_line_class[] Generate_from_downstreamEnrichmentResults_for_one_sideEffect_entryType_and_scp(Enrichment2018_results_line_class[] sameSideEffectScpEntryType_enrichment_result_lines, Dictionary<string, bool> sameSideEffect_drugs_dict, string sideEffect)
        {
            DE_entry_enum entryType = sameSideEffectScpEntryType_enrichment_result_lines[0].Sample_entryType;
            int total_positive_count;
            Roc_screen_line_class[] roc_screen_lines = Generate_sorted_screen_lines_from_downstreamEnrichmentResults_and_total_positive_count(sameSideEffectScpEntryType_enrichment_result_lines, sameSideEffect_drugs_dict, out total_positive_count);
            int total_drugSubjectPlateCombinations_in_significant_count = roc_screen_lines.Length;
            string[] scps = Get_all_ordered_distinct_scps(sameSideEffectScpEntryType_enrichment_result_lines);

            int running_sum_of_positive_count = 0;
            int running_sum_of_true_positive_count = 0;

            Roc_screen_line_class roc_screen_line;
            int roc_screen_length = roc_screen_lines.Length;
            List<string> new_true_positive_drugs_list = new List<string>();
            List<string> new_false_positive_drugs_list = new List<string>();

            Roc_line_class roc_results_line;
            List<Roc_line_class> roc_results_list = new List<Roc_line_class>();

            for (int indexRoc = 0; indexRoc < roc_screen_length; indexRoc++)
            {
                roc_screen_line = roc_screen_lines[indexRoc];
                if ((indexRoc == 0) || (!roc_screen_line.Value.Equals(roc_screen_lines[indexRoc - 1].Value)))
                {
                    new_true_positive_drugs_list.Clear();
                    new_false_positive_drugs_list.Clear();
                }
                running_sum_of_positive_count++;
                if (roc_screen_line.True_positive)
                {
                    running_sum_of_true_positive_count++;
                    new_true_positive_drugs_list.Add(roc_screen_line.Drug + " (" + roc_screen_line.Cell_line + ")");
                }
                else
                {
                    new_false_positive_drugs_list.Add(roc_screen_line.Drug + " (" + roc_screen_line.Cell_line + ")");
                }
                if ((indexRoc == roc_screen_length - 1) || (!roc_screen_line.Value.Equals(roc_screen_lines[indexRoc + 1].Value)))
                {
                    roc_results_line = new Roc_line_class();
                    roc_results_line.Side_effect = (string)sideEffect.Clone();
                    roc_results_line.True_positives_length = running_sum_of_true_positive_count;
                    roc_results_line.False_positives_length = running_sum_of_positive_count - running_sum_of_true_positive_count;
                    roc_results_line.False_negatives_length = total_positive_count - running_sum_of_true_positive_count;
                    roc_results_line.True_negatives_length = (total_drugSubjectPlateCombinations_in_significant_count - total_positive_count) - roc_results_line.False_positives_length;
                    roc_results_line.True_positive_rate = (float)roc_results_line.True_positives_length / (float)total_positive_count;
                    roc_results_line.False_positive_rate = (float)roc_results_line.False_positives_length / (float)(total_drugSubjectPlateCombinations_in_significant_count - total_positive_count);
                    roc_results_line.New_true_positives = new_true_positive_drugs_list.ToArray();
                    roc_results_line.New_false_positives = new_false_positive_drugs_list.ToArray();
                    roc_results_line.Disease_neighborhoods = Array_class.Deep_copy_string_array(scps);
                    roc_results_line.Roc_cutoff = roc_screen_line.Value;
                    roc_results_line.Entry_type = entryType;
                    roc_results_line.Pairwise_score_of_interest = this.Options.Pairwise_score_of_interest;
                    roc_results_line.Pairwise_cutoff = -1;
                    roc_results_line.Precision = (float)roc_results_line.True_positives_length / (float)running_sum_of_positive_count;
                    roc_results_line.Recall = roc_results_line.True_positive_rate;
                    roc_results_line.Total_samples_count = total_drugSubjectPlateCombinations_in_significant_count;
                    if (roc_results_line.Entry_type.Equals(DE_entry_enum.E_m_p_t_y)) { throw new Exception(); }
                    roc_results_list.Add(roc_results_line);
                }
            }
            return roc_results_list.ToArray();
        }
        private void Generate_from_downstreamEnrichmentResults_for_one_sideEffeect_and_add_to_array(Enrichment2018_results_line_class[] enrichment_result_lines, Dictionary<string, bool> sameSideEffect_drugs_dict, string sideEffect)
        {
            enrichment_result_lines = enrichment_result_lines.OrderBy(l => l.Scp).ThenBy(l => l.Sample_entryType).ThenBy(l => l.Fractional_rank).ToArray();
            int sameSideEffect_enrichment_length = enrichment_result_lines.Length;
            Enrichment2018_results_line_class enrichment_results_line;
            List<Enrichment2018_results_line_class> sameSideEffectEntryTypeScp_list = new List<Enrichment2018_results_line_class>();
            List<Roc_line_class> roc_list = new List<Roc_line_class>();
            for (int indexEnrich = 0; indexEnrich < sameSideEffect_enrichment_length; indexEnrich++)
            {
                enrichment_results_line = enrichment_result_lines[indexEnrich];
                if (enrichment_results_line.Sample_entryType.Equals(DE_entry_enum.E_m_p_t_y)) { throw new Exception(); }
                if ((indexEnrich == 0)
                    || (!enrichment_results_line.Sample_entryType.Equals(enrichment_result_lines[indexEnrich - 1].Sample_entryType))
                    || (!enrichment_results_line.Scp.Equals(enrichment_result_lines[indexEnrich - 1].Scp)))
                {
                    sameSideEffectEntryTypeScp_list.Clear();
                }
                sameSideEffectEntryTypeScp_list.Add(enrichment_results_line);
                if ((indexEnrich == sameSideEffect_enrichment_length - 1)
                    || (!enrichment_results_line.Sample_entryType.Equals(enrichment_result_lines[indexEnrich + 1].Sample_entryType))
                    || (!enrichment_results_line.Scp.Equals(enrichment_result_lines[indexEnrich + 1].Scp)))
                {
                    roc_list.AddRange(Generate_from_downstreamEnrichmentResults_for_one_sideEffect_entryType_and_scp(sameSideEffectEntryTypeScp_list.ToArray(), sameSideEffect_drugs_dict, sideEffect));
                }
            }
            Add_to_array(roc_list.ToArray());
        }
        public void Generate_from_downstreamEnrichmentResults_significant_and_add_to_array(Enrichment2018_results_class enrichment)
        {
            Drug_leaflet_line_class[] drug_leaflets = this.Leaflet.Leaflets;
            Drug_leaflet_line_class drug_leaflet_line;
            drug_leaflets = drug_leaflets.OrderBy(l => l.Side_effect).ThenBy(l => l.Drug).ToArray();
            int drug_leaflets_length = drug_leaflets.Length;
            Dictionary<string, bool> sameSideEffect_drugs_dict = new Dictionary<string, bool>();
            for (int indexDrugLeaflet = 0; indexDrugLeaflet < drug_leaflets_length; indexDrugLeaflet++)
            {
                drug_leaflet_line = drug_leaflets[indexDrugLeaflet];
                if ((indexDrugLeaflet == 0) || (!drug_leaflet_line.Side_effect.Equals(drug_leaflets[indexDrugLeaflet - 1].Side_effect)))
                {
                    sameSideEffect_drugs_dict.Clear();
                }
                sameSideEffect_drugs_dict.Add(drug_leaflet_line.Drug, drug_leaflet_line.Creates_side_effect);
                if ((indexDrugLeaflet == drug_leaflets_length - 1) || (!drug_leaflet_line.Side_effect.Equals(drug_leaflets[indexDrugLeaflet + 1].Side_effect)))
                {
                    Generate_from_downstreamEnrichmentResults_for_one_sideEffeect_and_add_to_array(enrichment.Enrichment_results, sameSideEffect_drugs_dict, drug_leaflet_line.Side_effect);
                }
            }
        }
        #endregion

        public void Clear_rocs()
        {
            this.Roc = new Roc_line_class[0];
        }

        private string[] Replace_terms_by_unique_terms_plus_term_count(string[] terms)
        {
            Dictionary<string, int> term_count_dict = new Dictionary<string, int>();
            foreach (string term in terms)
            {
                if (!term_count_dict.ContainsKey(term)) { term_count_dict.Add(term, 0); }
                term_count_dict[term]++;
            }
            string[] unique_terms = term_count_dict.Keys.ToArray();
            List<string> terms_plus_term_count = new List<string>();
            foreach (string unique_term in unique_terms)
            {
                if (term_count_dict[unique_term] == 1) { terms_plus_term_count.Add(unique_term); }
                else { terms_plus_term_count.Add(unique_term + " (" + term_count_dict[unique_term] + "x)"); }
            }
            return terms_plus_term_count.ToArray();
        }

        public void Replace_new_true_and_false_positives_by_unique_terms_plus_term_count()
        {
            foreach (Roc_line_class roc_line in this.Roc)
            {
                roc_line.New_true_positives = Replace_terms_by_unique_terms_plus_term_count(roc_line.New_true_positives);
                roc_line.New_false_positives = Replace_terms_by_unique_terms_plus_term_count(roc_line.New_false_positives);
            }
        }

        public void Keep_only_sideEffect_of_interest(string keep_sideEffect)
        {
            List<Roc_line_class> keep = new List<Roc_line_class>();
            foreach (Roc_line_class roc_line in this.Roc)
            {
                if (roc_line.Side_effect.Equals(keep_sideEffect))
                {
                    keep.Add(roc_line);
                }
            }
            this.Roc = keep.ToArray();
        }

        private string[] Merge_drugs_plus_cell_lines_and_shorten_cell_line_names(string[] drugs_plus_celllines)
        {
            drugs_plus_celllines = drugs_plus_celllines.OrderBy(l => l).ToArray();
            int drugs_length = drugs_plus_celllines.Length;
            string drug_plus_cellline;
            Dictionary<string, List<string>> drug_celllines_dict = new Dictionary<string, List<string>>();
            string[] splitStrings;
            string drug;
            string cellline;
            for (int indexD = 0; indexD < drugs_length; indexD++)
            {
                drug_plus_cellline = drugs_plus_celllines[indexD];
                splitStrings = drug_plus_cellline.Split(' ');
                drug = splitStrings[0];
                cellline = splitStrings[1].Replace("(", "").Replace(")", "");
                if (!drug_celllines_dict.ContainsKey(drug))
                {
                    drug_celllines_dict.Add(drug, new List<string>());
                }
                drug_celllines_dict[drug].Add(cellline.Split('_')[0]);
            }
            string[] drugs = drug_celllines_dict.Keys.ToArray();
            drugs_length = drugs.Length;
            List<string> merged_drugs_plus_celllines = new List<string>();
            string[] celllines;
            int celllines_length;
            StringBuilder sb = new StringBuilder();
            for (int indexD = 0; indexD < drugs_length; indexD++)
            {
                drug = drugs[indexD];
                celllines = drug_celllines_dict[drug].OrderBy(l => l).ToArray();
                sb.Clear();
                sb.AppendFormat("{0}", drug);
                sb.AppendFormat(" (");
                celllines_length = celllines.Length;
                for (int indexCellline = 0; indexCellline < celllines_length; indexCellline++)
                {
                    cellline = celllines[indexCellline];
                    if (indexCellline > 0) { sb.AppendFormat(";"); }
                    sb.AppendFormat("{0}", cellline);
                }
                sb.AppendFormat(")");
                merged_drugs_plus_celllines.Add(sb.ToString());
            }
            return merged_drugs_plus_celllines.ToArray();
        }
        public void Add_all_previous_drugs_to_added_drugs_and_shorten_cell_line_names()
        {
            this.Roc = this.Roc.OrderBy(l => l.ReadWrite_disease_neighborhoods).ThenBy(l => l.Entry_type).ThenBy(l => l.Roc_cutoff).ToArray();
            int roc_length = this.Roc.Length;
            Roc_line_class roc_line;
            List<string> true_positives_drugs = new List<string>();
            List<string> false_positives_drugs = new List<string>();
            string[] merged_true_positives;
            string[] merged_false_positives;
            for (int indexRoc = 0; indexRoc < roc_length; indexRoc++)
            {
                roc_line = this.Roc[indexRoc];
                if ((indexRoc == 0)
                    || (!roc_line.ReadWrite_disease_neighborhoods.Equals(this.Roc[indexRoc - 1].ReadWrite_disease_neighborhoods))
                    || (!roc_line.Entry_type.Equals(this.Roc[indexRoc - 1].Entry_type)))
                {
                    true_positives_drugs.Clear();
                    false_positives_drugs.Clear();
                }
                true_positives_drugs.AddRange(roc_line.New_true_positives);
                merged_true_positives = Merge_drugs_plus_cell_lines_and_shorten_cell_line_names(true_positives_drugs.ToArray());
                false_positives_drugs.AddRange(roc_line.New_false_positives);
                merged_false_positives = Merge_drugs_plus_cell_lines_and_shorten_cell_line_names(false_positives_drugs.ToArray());
                roc_line.New_true_positives = merged_true_positives;
                roc_line.New_false_positives = merged_false_positives;
            }
        }


        public void Keep_only_max_rocCutoff(float max_enrichmentRank_cutoff)
        {
            List<Roc_line_class> keep = new List<Roc_line_class>();
            foreach (Roc_line_class roc_line in this.Roc)
            {
                if (roc_line.Roc_cutoff <= max_enrichmentRank_cutoff)
                {
                    keep.Add(roc_line);
                }
            }
            this.Roc = keep.ToArray();
        }

        public string[] Get_all_side_effects()
        {
            Dictionary<string, bool> sideEffect_dict = new Dictionary<string, bool>();
            foreach (Roc_line_class roc_line in this.Roc)
            {
                if (!sideEffect_dict.ContainsKey(roc_line.Side_effect))
                {
                    sideEffect_dict.Add(roc_line.Side_effect, true);
                }
            }
            return sideEffect_dict.Keys.ToArray();
        }

        private float[] Get_all_f1_score_betas()
        {
            Dictionary<float, bool> beta_dict = new Dictionary<float, bool>();
            foreach (Roc_line_class roc_line in this.Roc)
            {
                if (!beta_dict.ContainsKey(roc_line.F1_score_beta))
                {
                    beta_dict.Add(roc_line.F1_score_beta, true);
                }
            }
            return beta_dict.Keys.ToArray();
        }

        public void Add_F1_score_AUCs(Lincs_scp_summary_after_roc_class scp_summaries, float f1_score_beta)
        {
            Lincs_scp_summary_valueType_for_selection_enum valueTypeForSelection = scp_summaries.Scp_summaries[0].ValueTypeForSelection;
            string association = (string)scp_summaries.Scp_summaries[0].Association.Clone();
            string sideEffect = (string)scp_summaries.Scp_summaries[0].Side_effect.Clone();
            this.Roc = this.Roc.OrderBy(l => l.ReadWrite_disease_neighborhoods).ThenBy(l => l.Entry_type).ToArray();
            scp_summaries.Scp_summaries = scp_summaries.Scp_summaries.OrderBy(l => l.Scp_completeName).ThenBy(l => l.Entry_type).ThenBy(l => l.Association).ToArray();
            int scp_summaries_length = scp_summaries.Scp_summaries.Length;
            Lincs_scp_summary_after_roc_line_class scp_summary_line;
            int indexSCPSummary_start = 0;
            int indexSCPSummary = 0;
            int roc_length = this.Roc.Length;
            int stringCompare;
            Roc_line_class roc_line;
            for (int indexRoc = 0; indexRoc < roc_length; indexRoc++)
            {
                roc_line = this.Roc[indexRoc];
                roc_line.F1_score = (1F + (float)Math.Pow(f1_score_beta, 2)) * (roc_line.Precision * roc_line.Recall) / ((float)Math.Pow(f1_score_beta, 2) * roc_line.Precision + roc_line.Recall);
                roc_line.F1_score_AUC = float.NaN;
                roc_line.F1_score_AUC_rank = float.NaN;
                roc_line.F1_score_beta = f1_score_beta;
                stringCompare = -2;
                indexSCPSummary = indexSCPSummary_start;
                while ((indexSCPSummary < scp_summaries_length) && (stringCompare < 0))
                {
                    scp_summary_line = scp_summaries.Scp_summaries[indexSCPSummary];
                    if (!scp_summary_line.Side_effect.Equals(sideEffect)) { throw new Exception(); }
                    if (!scp_summary_line.Association.Equals(association)) { throw new Exception(); }
                    if (!scp_summary_line.ValueTypeForSelection.Equals(valueTypeForSelection)) { throw new Exception(); }
                    stringCompare = scp_summary_line.Scp_completeName.CompareTo(roc_line.ReadWrite_disease_neighborhoods);
                    if (stringCompare == 0)
                    {
                        stringCompare = scp_summary_line.Entry_type.CompareTo(roc_line.Entry_type);
                    }
                    if (stringCompare < 0)
                    {
                        indexSCPSummary_start++;
                        indexSCPSummary = indexSCPSummary_start;
                    }
                    else if (stringCompare == 0)
                    {
                        roc_line.F1_score_AUC = scp_summary_line.ValueForSelection;
                        roc_line.F1_score_AUC_rank = scp_summary_line.Selection_rank;
                    }
                }
            }
        }

        public void Keep_only_cardiotoxic_pathways_with_f1_score_AUC_ranks()
        {
            int roc_length = this.Roc.Length;
            Roc_line_class roc_line;
            List<Roc_line_class> roc_list = new List<Roc_line_class>();
            for (int indexRoc = 0; indexRoc < roc_length; indexRoc++)
            {
                roc_line = this.Roc[indexRoc];
                if (!float.IsNaN(roc_line.F1_score_AUC))
                {
                    roc_list.Add(roc_line);
                }
            }
            this.Roc = roc_list.ToArray();
        }

        public void Write(string subdirectory, string fileName)
        {
            Roc_readWriteOptions readWriteOptions = new Roc_readWriteOptions(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Roc, readWriteOptions);
        }

        public void Write_for_website(string subdirectory, string fileName)
        {
            float[] f1_score_betas = Get_all_f1_score_betas();
            if (f1_score_betas.Length != 1) { throw new Exception(); }
            Roc_for_website_readWriteOptions readWriteOptions = new Roc_for_website_readWriteOptions(subdirectory, fileName, f1_score_betas[0]);
            ReadWriteClass.WriteData(this.Roc, readWriteOptions);
        }

        public void Read_from_website(string subdirectory, string fileName, float f1_score_beta)
        {
            Roc_for_website_readWriteOptions readWriteOptions = new Roc_for_website_readWriteOptions(subdirectory, fileName, f1_score_beta);
            this.Roc = ReadWriteClass.ReadRawData_and_FillArray<Roc_line_class>(readWriteOptions);
        }

        public void Read(string subdirectory, string fileName)
        {
            Roc_readWriteOptions readWriteOptions = new Roc_readWriteOptions(subdirectory, fileName);
            this.Roc = ReadWriteClass.ReadRawData_and_FillArray<Roc_line_class>(readWriteOptions);
        }

    }

    ///////////////////////////////////////////////////////////////////////////////////////

    class Pairwise_enrichment_roc_auc_line_class
    {
        public Pairwise_enrichment_score_of_interest_enum Pairwise_score_of_interest { get; set; }
        public string Side_effect { get; set; }
        public DE_entry_enum Entry_type { get; set; }
        public double Lowest_diseaseNeighborhood_rank { get; set; }
        public string[] Disease_neighborhoods { get; set; }
        public double Area_under_the_curve { get; set; }
        public int Ordinal_rank_auc { get; set; }
        public int Pairwise_cutoff { get; set; }
        public int DEGs_minimum { get; set; }

        public string ReadWrite_disease_neighborhoods
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Disease_neighborhoods, Roc_readWriteOptions.Array_delimiter); }
            set { this.Disease_neighborhoods = ReadWriteClass.Get_array_from_readLine<string>(value, Roc_readWriteOptions.Array_delimiter); }
        }

        public Roc_line_class Deep_copy()
        {
            Roc_line_class copy = (Roc_line_class)this.MemberwiseClone();
            copy.Disease_neighborhoods = Array_class.Deep_copy_string_array(this.Disease_neighborhoods);
            copy.Side_effect = (string)this.Side_effect.Clone();
            return copy;
        }


    }

    class Pairwise_enrichment_roc_auc_readWriteOptions : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Pairwise_enrichment_roc_auc_readWriteOptions(string subdirectory, string filename)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + filename;
            this.Key_propertyNames = new string[] { "Side_effect", "ReadWrite_disease_neighborhoods", "Entry_type", "Area_under_the_curve", "Pairwise_cutoff", "Lowest_diseaseNeighborhood_rank", "Ordinal_rank_auc", "DEGs_minimum" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Pairwise_enrichment_roc_auc_options_class
    {
        public float Riemann_bars_width { get; set; }

        public Pairwise_enrichment_roc_auc_options_class()
        {
            Riemann_bars_width = 0.01F;
        }

    }

    class Pairwise_enrichment_roc_auc_class
    {
        public Pairwise_enrichment_roc_auc_line_class[] Auc { get; set; }
        public Pairwise_enrichment_roc_auc_options_class Options { get; set; }

        public Pairwise_enrichment_roc_auc_class()
        {
            Options = new Pairwise_enrichment_roc_auc_options_class();
        }

        private Pairwise_enrichment_roc_auc_line_class Generate_from_one_block(Roc_line_class[] sameBloc_roc_lines)
        {
            throw new Exception();
            //sameBloc_roc_lines = sameBloc_roc_lines.OrderBy(l => l.False_positive_rate).ToArray();
            //Pairwise_enrichment_roc_line_class first_roc_line = sameBloc_roc_lines[0];
            //Pairwise_enrichment_roc_line_class left_roc_line;
            //Pairwise_enrichment_roc_line_class right_roc_line;
            //int roc_lines_length = sameBloc_roc_lines.Length;
            //float width_of_bar;
            //float height_of_bar;
            //float auc = 0;

            //double lowest_dieseseNeighorhood_cutoff_rank = 99999999999F;

            //for (int indexRoc = 0; indexRoc < roc_lines_length - 1; indexRoc++)
            //{
            //    left_roc_line = sameBloc_roc_lines[indexRoc];
            //    right_roc_line = sameBloc_roc_lines[indexRoc + 1];

            //    if ((left_roc_line.Roc_cutoff != -1)
            //       && (left_roc_line.Roc_cutoff < lowest_dieseseNeighorhood_cutoff_rank))
            //    {
            //        lowest_dieseseNeighorhood_cutoff_rank = left_roc_line.Roc_cutoff;
            //    }
            //    if ((right_roc_line.Roc_cutoff != -1)
            //        && (right_roc_line.Roc_cutoff < lowest_dieseseNeighorhood_cutoff_rank))
            //    {
            //        lowest_dieseseNeighorhood_cutoff_rank = right_roc_line.Roc_cutoff;
            //    }

            //    if ((!right_roc_line.Side_effect.Equals(left_roc_line.Side_effect))
            //        || (!right_roc_line.ReadWrite_disease_neighborhoods.Equals(left_roc_line.ReadWrite_disease_neighborhoods))
            //        || (!right_roc_line.Pairwise_score_of_interest.Equals(left_roc_line.Pairwise_score_of_interest))
            //        || (!right_roc_line.Entry_type.Equals(left_roc_line.Entry_type))
            //        || (!right_roc_line.Side_effect.Equals(left_roc_line.Side_effect)))
            //    {
            //        throw new Exception();
            //    }

            //    width_of_bar = right_roc_line.False_positive_rate - left_roc_line.False_positive_rate;
            //    height_of_bar = left_roc_line.True_positive_rate;// + (float)(right_roc_line.True_positive_rate - left_roc_line.True_positive_rate) / 2F;
            //    auc += width_of_bar * height_of_bar;
            //}
            //if (auc == 0)
            //{
            //    Console.WriteLine();
            //}
            //Pairwise_enrichment_roc_auc_line_class new_roc_auc_line = new Pairwise_enrichment_roc_auc_line_class();
            //new_roc_auc_line.Area_under_the_curve = auc;
            //new_roc_auc_line.DEGs_minimum = first_roc_line.DEGs_minimum;
            //new_roc_auc_line.Disease_neighborhoods = Array_class.Deep_copy_string_array(first_roc_line.Disease_neighborhoods);
            //new_roc_auc_line.Lowest_diseaseNeighborhood_rank = lowest_dieseseNeighorhood_cutoff_rank;
            //new_roc_auc_line.Pairwise_cutoff = first_roc_line.Pairwise_cutoff;
            //new_roc_auc_line.Pairwise_score_of_interest = first_roc_line.Pairwise_score_of_interest;
            //new_roc_auc_line.Side_effect = (string)first_roc_line.Side_effect;
            //new_roc_auc_line.Entry_type = first_roc_line.Entry_type;
            //return new_roc_auc_line;
        }

        public void Generate_from_roc_instance(Roc_class roc)
        {
            roc.Roc = roc.Roc.OrderBy(l => l.Pairwise_score_of_interest).ThenBy(l => l.Pairwise_cutoff).ThenBy(l => l.ReadWrite_disease_neighborhoods).ThenBy(l => l.Entry_type).ThenBy(l => l.Side_effect).ToArray();
            int roc_length = roc.Roc.Length;
            Roc_line_class roc_line;
            List<Roc_line_class> sameBlock_roc_list = new List<Roc_line_class>();
            Pairwise_enrichment_roc_auc_line_class new_roc_auc_line = new Pairwise_enrichment_roc_auc_line_class();
            List<Pairwise_enrichment_roc_auc_line_class> new_roc_auc_list = new List<Pairwise_enrichment_roc_auc_line_class>();
            for (int indexRoc = 0; indexRoc < roc_length; indexRoc++)
            {
                roc_line = roc.Roc[indexRoc];
                if ((indexRoc == 0)
                    || (!roc_line.Pairwise_score_of_interest.Equals(roc.Roc[indexRoc - 1].Pairwise_score_of_interest))
                    || (!roc_line.Pairwise_cutoff.Equals(roc.Roc[indexRoc - 1].Pairwise_cutoff))
                    || (!roc_line.ReadWrite_disease_neighborhoods.Equals(roc.Roc[indexRoc - 1].ReadWrite_disease_neighborhoods))
                    || (!roc_line.Entry_type.Equals(roc.Roc[indexRoc - 1].Entry_type))
                    || (!roc_line.Side_effect.Equals(roc.Roc[indexRoc - 1].Side_effect)))
                {
                    sameBlock_roc_list.Clear();
                }
                sameBlock_roc_list.Add(roc_line);
                if ((indexRoc == roc_length - 1)
                    || (!roc_line.Pairwise_score_of_interest.Equals(roc.Roc[indexRoc + 1].Pairwise_score_of_interest))
                    || (!roc_line.Pairwise_cutoff.Equals(roc.Roc[indexRoc + 1].Pairwise_cutoff))
                    || (!roc_line.ReadWrite_disease_neighborhoods.Equals(roc.Roc[indexRoc + 1].ReadWrite_disease_neighborhoods))
                    || (!roc_line.Entry_type.Equals(roc.Roc[indexRoc + 1].Entry_type))
                    || (!roc_line.Side_effect.Equals(roc.Roc[indexRoc + 1].Side_effect)))
                {
                    new_roc_auc_line = Generate_from_one_block(sameBlock_roc_list.ToArray());
                    new_roc_auc_list.Add(new_roc_auc_line);
                }
            }
            this.Auc = new_roc_auc_list.ToArray();
        }

        public void Rank_by_decreasing_auc()
        {
            this.Auc = this.Auc.OrderBy(l => l.Pairwise_score_of_interest).ThenBy(l => l.Side_effect).ThenBy(l => l.Pairwise_cutoff).ThenByDescending(l => l.Area_under_the_curve).ToArray();
            int auc_length = this.Auc.Length;
            Pairwise_enrichment_roc_auc_line_class roc_line;
            int current_rank = 0;
            for (int indexAuc = 0; indexAuc < auc_length; indexAuc++)
            {
                roc_line = this.Auc[indexAuc];
                if ((indexAuc == 0)
                    || (!roc_line.Pairwise_score_of_interest.Equals(this.Auc[indexAuc - 1].Pairwise_score_of_interest))
                    || (!roc_line.Side_effect.Equals(this.Auc[indexAuc - 1].Side_effect))
                    || (!roc_line.Pairwise_cutoff.Equals(this.Auc[indexAuc - 1].Pairwise_cutoff)))
                {
                    current_rank = 0;
                }
                current_rank++;
                roc_line.Ordinal_rank_auc = current_rank;
            }
        }

        public void Write(string subdirectory, string fileName)
        {
            Pairwise_enrichment_roc_auc_readWriteOptions readWriteOptions = new Pairwise_enrichment_roc_auc_readWriteOptions(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Auc, readWriteOptions);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////

}
