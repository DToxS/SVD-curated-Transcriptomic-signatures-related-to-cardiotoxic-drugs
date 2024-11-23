using System;
using System.Collections.Generic;
using System.Linq;
using Common_classes;
using Highthroughput_data;
using ReadWrite;

namespace Statistic
{
    enum Correlation_type_enum { E_m_p_t_y, Pearson };

    class Correlation_line_class
    {
        public string[] Names0 {get;set;}
        public Timepoint_enum Timepoint0 { get; set; }
        public DE_entry_enum Entry_type0 { get; set; }
        public string[] Names1 {get;set;}
        public Timepoint_enum Timepoint1 { get; set; }
        public DE_entry_enum Entry_type1 { get; set; }
        public int Zeros_count0 { get; set; }
        public int Zeros_count1 { get; set; }
        public int Compared_values_length { get; set; }
        public double Correlation_coefficient { get; set; }
        public Correlation_type_enum Correlation_type { get; set; }
        public float Fractional_rank_for_group0 { get; set; }

        public string ReadWrite_names0
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Names0, Correlation_readWriteOptions_class.Delimiter); }
            set { this.Names0 = ReadWriteClass.Get_array_from_readLine<string>(value, Correlation_readWriteOptions_class.Delimiter); }
        }

        public string ReadWrite_names1
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Names1, Correlation_readWriteOptions_class.Delimiter); }
            set { this.Names1 = ReadWriteClass.Get_array_from_readLine<string>(value, Correlation_readWriteOptions_class.Delimiter); }
        }

        public Correlation_line_class Deep_copy()
        {
            Correlation_line_class correlation_line = (Correlation_line_class)this.MemberwiseClone();
            correlation_line.Names0 = Array_class.Deep_copy_string_array(this.Names0);
            correlation_line.Names1 = Array_class.Deep_copy_string_array(this.Names1);
            return correlation_line;
        }
    }

    class Correlation_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Delimiter { get { return ';'; } }

        public Correlation_readWriteOptions_class(string results_subdirectory, string file_name)
        {
            File = Global_directory_class.Results_directory + results_subdirectory + file_name;
            Key_propertyNames = new string[] { "Entry_type0", "Timepoint0", "ReadWrite_names0", "Entry_type1", "Timepoint1", "ReadWrite_names1", "Correlation_coefficient", "Correlation_type","Zeros_count0","Zeros_count1","Compared_values_length", "Fractional_rank_for_group0" };
            Key_columnNames = Key_propertyNames;
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            Report = ReadWrite_report_enum.Report_main;
            File_has_headline = true;
        }
    }

    class Correlation_options_class
    {
        public bool Consider_only_non_zero_values_in_each_pairwise_comparison { get; set; }

        public Correlation_options_class()
        {
            Consider_only_non_zero_values_in_each_pairwise_comparison = false;
        }
    }

    class Correlation_class
    {
        Correlation_line_class[] Correlations { get; set; }
        public Correlation_options_class Options { get; set; }

        public Correlation_class()
        {
            Correlations = new Correlation_line_class[0];
            Options = new Correlation_options_class();
        }

        private void Add_to_array(params Correlation_line_class[] add_correlations)
        {
            int add_correlations_length = add_correlations.Length;
            int this_correaltions_length = this.Correlations.Length;
            int new_correlations_length = add_correlations_length + this_correaltions_length;
            Correlation_line_class[] new_correlations = new Correlation_line_class[new_correlations_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_correaltions_length; indexThis++)
            {
                indexNew++;
                new_correlations[indexNew] = this.Correlations[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_correlations_length; indexAdd++)
            {
                indexNew++;
                new_correlations[indexNew] = add_correlations[indexAdd];
            }
            this.Correlations = new_correlations;
        }

        private int Count_zeros(double[] values)
        {
            int zeros_count = 0;
            foreach (double value in values)
            {
                if (value == 0) { zeros_count++; }
            }
            return zeros_count;
        }

        private void Remove_all_datapoint_pairs_that_are_zero_in_at_least_one_array(ref double[] de0_expression_values, ref double[] de1_expression_values)
        {
            int de0_length = de0_expression_values.Length;
            int de1_length = de1_expression_values.Length;

            List<double> de0_kept_values = new List<double>();
            List<double> de1_kept_values = new List<double>();

            double current_de0_expression_value;
            double current_de1_expression_value;

            for (int indexDe=0; indexDe<de0_length; indexDe++)
            {
                current_de0_expression_value = de0_expression_values[indexDe];
                current_de1_expression_value = de1_expression_values[indexDe];
                if (  (current_de0_expression_value != 0)
                    &&(current_de1_expression_value != 0))
                {
                    de0_kept_values.Add(current_de0_expression_value);
                    de1_kept_values.Add(current_de1_expression_value);
                }
            }
            de0_expression_values = de0_kept_values.ToArray();
            de1_expression_values = de1_kept_values.ToArray();
        }

        public void Generate_pairwise_correlations_between_de_instances_and_add_to_array(DE_class de0, DE_class de1)
        {
            string[] de0_symbols = de0.Get_all_symbols_in_current_order();
            string[] de1_symbols = de1.Get_all_symbols_in_current_order();
            string[] de0_symbols_missing_in_de1 = Overlap_class.Get_part_of_list1_but_not_of_list2(de0_symbols, de1_symbols);
            string[] de1_symbols_missing_in_de0 = Overlap_class.Get_part_of_list1_but_not_of_list2(de1_symbols, de0_symbols);
            de0.Fill_with_symbols(Organism_enum.Homo_sapiens, de0.ColChar.Columns[0].EntryType, de0.ColChar.Columns[0].Timepoint, de0.ColChar.Columns[0].Names, 0, de1_symbols_missing_in_de0);
            de1.Fill_with_symbols(Organism_enum.Homo_sapiens, de1.ColChar.Columns[0].EntryType, de1.ColChar.Columns[0].Timepoint, de1.ColChar.Columns[0].Names, 0, de0_symbols_missing_in_de1);

            de0.Order_by_symbol();
            de1.Order_by_symbol();
            int de0_col_length = de0.ColChar.Columns.Count;
            int de1_col_length = de1.ColChar.Columns.Count;
            double[] de0_expression_values;
            double[] de1_expression_values;
            double[] input_de0_expression_values;
            double[] input_de1_expression_values;
            Correlation_line_class new_correlation_line;
            List<Correlation_line_class> correlation_list = new List<Correlation_line_class>();
            Correlation_coefficient_class correlation_coefficient = new Correlation_coefficient_class();
            for (int indexCol0 = 0; indexCol0 < de0_col_length; indexCol0++)
            {
                input_de0_expression_values = de0.Get_all_values_of_indicated_column_in_given_order(indexCol0);
                for (int indexCol1 = 0; indexCol1 < de1_col_length; indexCol1++)
                {
                    input_de1_expression_values = de1.Get_all_values_of_indicated_column_in_given_order(indexCol1);

                    if (Options.Consider_only_non_zero_values_in_each_pairwise_comparison)
                    {
                        de0_expression_values = Array_class.Deep_copy_array(input_de0_expression_values);
                        de1_expression_values = Array_class.Deep_copy_array(input_de1_expression_values);
                        Remove_all_datapoint_pairs_that_are_zero_in_at_least_one_array(ref de0_expression_values, ref de1_expression_values);
                    }
                    else
                    {
                        de1_expression_values = input_de1_expression_values;
                        de0_expression_values = input_de0_expression_values;
                    }

                    if (de0_expression_values.Length != de1_expression_values.Length) { throw new Exception(); }
                    new_correlation_line = new Correlation_line_class();
                    new_correlation_line.Correlation_type = Correlation_type_enum.Pearson;
                    new_correlation_line.Entry_type0 = de0.ColChar.Columns[indexCol0].EntryType;
                    new_correlation_line.Timepoint0 = de0.ColChar.Columns[indexCol0].Timepoint;
                    new_correlation_line.Names0 = Array_class.Deep_copy_string_array(de0.ColChar.Columns[indexCol0].Names);
                    new_correlation_line.Entry_type1 = de1.ColChar.Columns[indexCol1].EntryType;
                    new_correlation_line.Timepoint1 = de1.ColChar.Columns[indexCol1].Timepoint;
                    new_correlation_line.Names1 = Array_class.Deep_copy_string_array(de1.ColChar.Columns[indexCol1].Names);

                    new_correlation_line.Correlation_coefficient = correlation_coefficient.Get_pearson_correlation_coefficient(de0_expression_values, de1_expression_values);
                 //   if (double.IsNaN(new_correlation_line.Correlation_coefficient)) { throw new Exception(); }
                    new_correlation_line.Zeros_count0 = Count_zeros(de0_expression_values);
                    new_correlation_line.Zeros_count1 = Count_zeros(de1_expression_values);
                    new_correlation_line.Compared_values_length = de0_expression_values.Length;
                    
                    correlation_list.Add(new_correlation_line);
                }
            }
            Add_to_array(correlation_list.ToArray());
        }

        public void Calculate_fractional_rank_with_same_group0_based_on_descending_correlation()
        {
            this.Correlations = this.Correlations.OrderBy(l => l.ReadWrite_names0).ThenByDescending(l=>l.Entry_type0).ThenBy(l=>l.Timepoint0).ThenBy(l=>l.Correlation_type).ThenByDescending(l => l.Correlation_coefficient).ToArray();
            int correlations_length = this.Correlations.Length;
            Correlation_line_class correlation_line;
            Correlation_line_class inner_correlation_line;
            int current_rank = 0;
            float fractional_rank;
            int firstIndexSameGroup0 = -1;
            List<float> current_ranks_sameCorrelation = new List<float>();
            for (int indexC = 0; indexC < correlations_length; indexC++)
            {
                correlation_line = this.Correlations[indexC];
                if (  (indexC == 0)
                    || (!correlation_line.Entry_type0.Equals(this.Correlations[indexC - 1].Entry_type0))
                    || (!correlation_line.Timepoint0.Equals(this.Correlations[indexC - 1].Timepoint0))
                    || (!correlation_line.ReadWrite_names0.Equals(this.Correlations[indexC - 1].ReadWrite_names0))
                    || (!correlation_line.Correlation_type.Equals(this.Correlations[indexC - 1].Correlation_type)))
                {
                    current_rank = 0;
                }
                if ((indexC == 0)
                    || (!correlation_line.Entry_type0.Equals(this.Correlations[indexC - 1].Entry_type0))
                    || (!correlation_line.Timepoint0.Equals(this.Correlations[indexC - 1].Timepoint0))
                    || (!correlation_line.ReadWrite_names0.Equals(this.Correlations[indexC - 1].ReadWrite_names0))
                    || (!correlation_line.Correlation_type.Equals(this.Correlations[indexC - 1].Correlation_type))
                    || (!correlation_line.Correlation_coefficient.Equals(this.Correlations[indexC - 1].Correlation_coefficient)))
                {
                    current_ranks_sameCorrelation.Clear();
                    firstIndexSameGroup0 = indexC;
                }
                current_rank++;
                current_ranks_sameCorrelation.Add(current_rank);
                if ((indexC == correlations_length-1)
                    || (!correlation_line.Entry_type0.Equals(this.Correlations[indexC + 1].Entry_type0))
                    || (!correlation_line.Timepoint0.Equals(this.Correlations[indexC + 1].Timepoint0))
                    || (!correlation_line.ReadWrite_names0.Equals(this.Correlations[indexC + 1].ReadWrite_names0))
                    || (!correlation_line.Correlation_type.Equals(this.Correlations[indexC + 1].Correlation_type))
                    || (!correlation_line.Correlation_coefficient.Equals(this.Correlations[indexC + 1].Correlation_coefficient)))
                {
                    fractional_rank = Math_class.Get_average(current_ranks_sameCorrelation.ToArray());
                    for (int indexInner=firstIndexSameGroup0; indexInner <= indexC;indexInner++)
                    {
                        inner_correlation_line = this.Correlations[indexInner];
                        inner_correlation_line.Fractional_rank_for_group0 = fractional_rank;
                    }
                }
            }
        }

        public void Generate_pairwise_correlations_between_all_columns_and_add_to_array(DE_class de)
        {
            int de_col_length = de.ColChar.Columns.Count;
            double[] de0_expression_values;
            double[] de1_expression_values;
            Correlation_line_class new_correlation_line;
            List<Correlation_line_class> correlation_list = new List<Correlation_line_class>();
            Correlation_coefficient_class correlation_coefficient = new Correlation_coefficient_class();
            for (int indexCol0 = 0; indexCol0 < de_col_length-1; indexCol0++)
            {
                de0_expression_values = de.Get_all_values_of_indicated_column_in_given_order(indexCol0);
                for (int indexCol1 = indexCol0+1; indexCol1 < de_col_length; indexCol1++)
                {
                    de1_expression_values = de.Get_all_values_of_indicated_column_in_given_order(indexCol1);
                    if (de0_expression_values.Length != de1_expression_values.Length) { throw new Exception(); }

                    new_correlation_line = new Correlation_line_class();
                    new_correlation_line.Correlation_type = Correlation_type_enum.Pearson;
                    new_correlation_line.Entry_type0 = de.ColChar.Columns[indexCol0].EntryType;
                    new_correlation_line.Timepoint0 = de.ColChar.Columns[indexCol0].Timepoint;
                    new_correlation_line.Names0 = Array_class.Deep_copy_string_array(de.ColChar.Columns[indexCol0].Names);
                    new_correlation_line.Entry_type1 = de.ColChar.Columns[indexCol1].EntryType;
                    new_correlation_line.Timepoint1 = de.ColChar.Columns[indexCol1].Timepoint;
                    new_correlation_line.Names1 = Array_class.Deep_copy_string_array(de.ColChar.Columns[indexCol1].Names);

                    new_correlation_line.Correlation_coefficient = correlation_coefficient.Get_pearson_correlation_coefficient(de0_expression_values, de1_expression_values);
                    new_correlation_line.Zeros_count0 = Count_zeros(de0_expression_values);
                    new_correlation_line.Zeros_count1 = Count_zeros(de1_expression_values);
                    new_correlation_line.Compared_values_length = de0_expression_values.Length;

                    correlation_list.Add(new_correlation_line);
                }
            }
            Add_to_array(correlation_list.ToArray());
        }

        public void Generate_pairwise_correlation_between_arrays_and_add_to_array(double[] array0, double[] array1, string name0, string name1)
        {
            Generate_pairwise_correlation_between_arrays_add_to_array_and_return_correlation_coefficient(array0, array1, name0, name1);
        }

        public double Generate_pairwise_correlation_between_arrays_add_to_array_and_return_correlation_coefficient(double[] array0, double[] array1, string name0, string name1)
        {
            if (array0.Length != array1.Length) { throw new Exception(); }

            Correlation_coefficient_class correlation_coefficient = new Correlation_coefficient_class();
            Correlation_line_class new_correlation_line = new Correlation_line_class();
            new_correlation_line.Correlation_type = Correlation_type_enum.Pearson;
            new_correlation_line.Names0 = new string[] { (string)name0.Clone() };
            new_correlation_line.Names1 = new string[] { (string)name1.Clone() };

            new_correlation_line.Correlation_coefficient = correlation_coefficient.Get_pearson_correlation_coefficient(array0, array1);
            new_correlation_line.Zeros_count0 = Count_zeros(array0);
            new_correlation_line.Zeros_count1 = Count_zeros(array1);
            new_correlation_line.Compared_values_length = array0.Length;

            Add_to_array(new_correlation_line);
            return new_correlation_line.Correlation_coefficient;
        }


        public void Write(string results_subdirectory, string file_name)
        {
            Correlation_readWriteOptions_class correlation_readWriteOptions = new Correlation_readWriteOptions_class(results_subdirectory, file_name);
            ReadWriteClass.WriteData(this.Correlations, correlation_readWriteOptions);
        }


    }
}
