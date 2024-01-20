using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReadWrite;
using Common_classes;
using Gene_databases;
using Highthroughput_data;
using Statistic;
using System.IO;

namespace Highthroughput_data
{
    enum Single_cell_normalization_type_enum { E_m_p_t_y, Total_reads_normalization, No_normalization, Read_removal }
    enum Read_status_enum { E_m_p_t_y, Assigned, Unassigned_ambiguity, Unassigned_multimapping, Unassigned_nofeatures, Unassigned_unmapped, Unassigned_mappingquality, Unassigned_fragmentlength, Unassigned_chimera, Unassigned_secondary, Unassigned_nonjunction, Unassigned_duplicate, Total, Fraction_of_assigned_reads, Fraction_of_assignedReads_assignedToMitoChrom, Assigned_to_mitoChrom };

    class Single_cell_sequencing_summary_line_class
    {
        public string Sample { get; set; }
        public int Sample_no { get; set; }
        public Read_status_enum Status { get; set; }
        public float Value { get; set; }
        public bool Microscope_positive { get; set; }
        public bool Kept { get; set; }
        public float ExpressionValue_of_outgrowthMarker { get; set; }
        public int Nog_sample_no { get; set; }
        public string Nog_sample { get; set; }

        public Single_cell_sequencing_summary_line_class()
        {
            this.Kept = true;
        }

        public Single_cell_sequencing_summary_line_class Deep_copy()
        {
            Single_cell_sequencing_summary_line_class copy = (Single_cell_sequencing_summary_line_class)this.MemberwiseClone();
            copy.Sample = (string)this.Sample.Clone();
            return copy;
        }
    }

    class Single_cell_sequencing_summary_readWriteOptions_class : ReadWriteOptions_base
    {
        public Single_cell_sequencing_summary_readWriteOptions_class(string date)
        {
            this.File = Global_directory_class.Results_directory + "Summary_" + date + ".txt";
            Key_propertyNames = new string[] { "Sample", "Sample_no", "Nog_sample", "Nog_sample_no", "Status", "Value", "Microscope_positive", "Kept", "ExpressionValue_of_outgrowthMarker" };
            Key_columnNames = Key_propertyNames;
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }
     
    ///////////////////////////////////////////////////////////////////////

    class Single_cell_sequencing_gene_summary_line_class
    {
        public string GeneId { get; set; }
        public string Symbol { get; set; }
        public bool Kept { get; set; }
        public double Average_read_counts { get; set; }
        public double Log10_average_read_counts { get; set; }
        public int Expressing_samples_count { get; set; }
        public int Total_samples_count { get; set; }
    }

    class Single_cell_sequencing_gene_summary_readWriteOptions_class : ReadWriteOptions_base
    {
        public Single_cell_sequencing_gene_summary_readWriteOptions_class(string date)
        {
            this.File = Global_directory_class.Results_directory + "Summary_genes_" + date + ".txt";
            this.Key_propertyNames = new string[] { "GeneId", "Symbol", "Average_read_counts", "Log10_average_read_counts", "Expressing_samples_count", "Total_samples_count", "Kept" };
            this.Key_columnNames = this.Key_propertyNames;
            this.File_has_headline = true;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    ///////////////////////////////////////////////////////////////////////
    
    class Single_cell_sequencing_line_class : IFill_de
    {
        public string GeneId { get; set; }
        public string Symbol { get; set; }
        public string Chromosome { get; set; }
        public float Expression_value { get; set; }
        public string Sample { get; set; }
        public int Sample_no { get; set; }
        public string Nog_sample { get; set; }
        public int Nog_sample_no { get; set; }
        public bool Is_official_gene_symbol { get; set; }
        public float Neurite_outgrowth_marker_expression { get; set; } 

        public Timepoint_enum Timepoint_for_de { get { return Timepoint_enum.H0; } }
        public string[] Symbols_for_de { get { return new string[] { (string)Symbol.Clone() }; } }
        public string[] Names_for_de 
        { 
            get 
            {
                //string first_name = "";
                //if (Neurite_outgrowth_marker_expression >= 10000)
                //{
                //    first_name = Neurite_outgrowth_marker_expression.ToString();
                //}
                //else if (Neurite_outgrowth_marker_expression >= 1000)
                //{
                //    first_name = "0" + Neurite_outgrowth_marker_expression.ToString();
                //}
                //else if (Neurite_outgrowth_marker_expression >= 100)
                //{
                //    first_name = "00" + Neurite_outgrowth_marker_expression.ToString();
                //}
                //else if (Neurite_outgrowth_marker_expression >= 10)
                //{
                //    first_name = "000" + Neurite_outgrowth_marker_expression.ToString();
                //}
                //else
                //{
                //    first_name = "0000" + Neurite_outgrowth_marker_expression.ToString();
                //}
                //return new string[] { first_name, (string)Sample.Clone() };
                return new string[] { (string)Nog_sample.Clone() };
            } 
        }
        public double Value_for_de { get { return Expression_value; } }
        public DE_entry_enum Entry_type_for_de { get { return DE_entry_enum.Rna; } }

        public Single_cell_sequencing_line_class()
        {
            this.Symbol = Global_class.Empty_entry.ToString();
            this.Expression_value = -1;
        }

        public Single_cell_sequencing_line_class Deep_copy()
        {
            Single_cell_sequencing_line_class copy = (Single_cell_sequencing_line_class)this.MemberwiseClone();
            copy.GeneId = (string)this.GeneId.Clone();
            copy.Symbol = (string)this.Symbol.Clone();
            copy.Sample = (string)this.Sample.Clone();
            return copy;
        }
    }

    class Single_cell_sequencing_options_class
    {
        public int Minimum_reads_per_cell { get; set; }
        public float Max_fraction_of_mitochondrial_reads_per_sample { get; set; }
        public float Min_fraction_of_assigned_reads_per_sample { get; set; }
        public Single_cell_normalization_type_enum Normalization_method { get; set; }
        public bool Keep_only_microscopically_selected_samples { get; set; }
        public bool Apply_filters { get; set; }
        public string[] Keep_cells { get; set; }
        public string[] Symbols_of_expressionMarker { get; set; }
        public bool Normalize_per_gene_length { get; set; }
        public bool Rename_samples_based_on_nog_marker { get; set; }

        public Single_cell_sequencing_options_class()
        {
            Apply_filters = true;
            Minimum_reads_per_cell = 3 * 10^6;
            Max_fraction_of_mitochondrial_reads_per_sample = 0.05F;
            Min_fraction_of_assigned_reads_per_sample = 0.3F;
            Normalization_method = Single_cell_normalization_type_enum.Total_reads_normalization;
            //Keep_cellNos = new int[] { 1, 2, 4, 7, 10, 13, 14, 15, 16, 19, 20, 21, 22, 25, 26, 27, 28, 31, 32, 33, 34, 35, 36, 37, 39, 44, 45, 48, 50, 51, 55, 57, 61, 63, 64, 66, 67, 69, 74, 75, 80, 81, 84, 86, 87, 88, 90, 91, 92 };
            //microscopic classification
            Keep_cells = new string[] { "Sample_01", "Sample_02", "Sample_04", "Sample_07", "Sample_10", "Sample_13", "Sample_14", "Sample_15", "Sample_16", "Sample_19", "Sample_21", "Sample_22", "Sample_25", "Sample_26", "Sample_27", "Sample_28", "Sample_31", "Sample_33", "Sample_34", "Sample_35", "Sample_36", "Sample_37", "Sample_39", "Sample_44", "Sample_45", "Sample_48", "Sample_50", "Sample_51", "Sample_55", "Sample_57", "Sample_61", "Sample_63", "Sample_64", "Sample_66", "Sample_67", "Sample_69", "Sample_74", "Sample_75", "Sample_81", "Sample_84", "Sample_86", "Sample_87", "Sample_88", "Sample_90", "Sample_91" };
            //SVM classification
            Keep_cells = new string[] { "Sample_01", "Sample_02", "Sample_04", "Sample_07", "Sample_10", "Sample_13", "Sample_14", "Sample_15", "Sample_16", "Sample_19", "Sample_21", "Sample_22", "Sample_25", "Sample_26", "Sample_27", "Sample_28", "Sample_31", "Sample_33", "Sample_34", "Sample_35", "Sample_36", "Sample_37", "Sample_39",              "Sample_45", "Sample_48", "Sample_50", "Sample_51", "Sample_55", "Sample_57", "Sample_61", "Sample_63",              "Sample_66", "Sample_67", "Sample_69", "Sample_74", "Sample_75", "Sample_81", "Sample_84", "Sample_86", "Sample_87",              "Sample_90", "Sample_91" };
            //Keep_cells = new string[] { "Sample_02", "Sample_04", "Sample_07", "Sample_10", "Sample_13", "Sample_14", "Sample_15", "Sample_16", "Sample_19", "Sample_21", "Sample_22", "Sample_25", "Sample_26", "Sample_27", "Sample_28", "Sample_31", "Sample_33", "Sample_34", "Sample_35", "Sample_36", "Sample_37", "Sample_39", "Sample_44", "Sample_45", "Sample_48", "Sample_50", "Sample_51", "Sample_55", "Sample_57", "Sample_61", "Sample_63", "Sample_64", "Sample_66", "Sample_67", "Sample_69", "Sample_74", "Sample_75", "Sample_81", "Sample_84", "Sample_86", "Sample_87", "Sample_88", "Sample_90", "Sample_91" };
            //Keep_cellNos = new int[] { 2, 4, 7, 13, 14, 15, 16, 19, 21, 22, 25, 26, 27, 28, 31, 33, 34, 35, 36, 37, 39, 44, 45, 48, 50, 51, 55, 57, 61, 63, 64, 66, 67, 69, 74, 75, 81, 84, 86, 87, 88, 90, 91 };
            Symbols_of_expressionMarker = new string[] { "Tubb3" };
            //Symbols_of_expressionMarker = new string[] { "Tuba1a", "Tuba1b", "Tuba1c", "Tubb2a", "Tubb2b", "Tubb3", "Tubb4a", "Tubb4b", "Tubb5", "Tube1", "Tubg1", "Tubg2" };
            Normalize_per_gene_length = true;
            Keep_only_microscopically_selected_samples = true;
            Rename_samples_based_on_nog_marker = true;
        }


    }


    ///////////////////////////////////////////////////////////////////////


    ///////////////////////////////////////////////////////////////////////


    class SingleCellCluster_gene_enrichment_line_class
    {
        public string Gene { get; set; }
        public float Pc1 { get; set; }
        public float Pc2 { get; set; }
        public float Average_expression1 { get; set; }
        public float Average_expression2 { get; set; }
        public int Cluster_no { get; set; }
        public string Cluster_name { get; set; }

        public SingleCellCluster_gene_enrichment_line_class Deep_copy()
        {
            SingleCellCluster_gene_enrichment_line_class copy = (SingleCellCluster_gene_enrichment_line_class)this.MemberwiseClone();
            copy.Gene = (string)this.Gene.Clone();
            copy.Cluster_name = (string)this.Cluster_name.Clone();
            return copy;
        }
    }

    class Cluster_specificMarkerGene_discovery_class
    {

    }

}
