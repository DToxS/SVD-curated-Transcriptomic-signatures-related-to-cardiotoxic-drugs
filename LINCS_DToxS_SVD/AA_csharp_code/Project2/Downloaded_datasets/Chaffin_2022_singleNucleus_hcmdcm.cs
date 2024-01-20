using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReadWrite;
using Common_classes;
using Enrichment;
using Statistic;
using Highthroughput_data;

namespace Downloaded_datasets
{
    class Chaffin2022_line_class
    {
        public string Gene { get; set; }
        public string Ensembl_ID { get; set; }
        public string Gene_type { get; set; }
        public string Cell_type { get; set; }
        public string Sub_cluster { get; set; }
        public int N_nuclei { get; set; }
        public float Fractional_rank { get; set; }
        public float PCT_target { get; set; }
        public float PCT_non_target { get; set; }
        public float Avg_exp_target { get; set; }
        public float Avg_exp_nonTargetEXPR { get; set; }
        public float Non_target_AUC { get; set; }
        public float Limma_voom_logFC { get; set; }
        public float Limma_voom_p_value { get; set; }
        public float Limma_voom_adjusted_p_value { get; set; }

        public Chaffin2022_line_class()
        {
            Fractional_rank = -1;
        }

        public Chaffin2022_line_class Deep_copy()
        {
            Chaffin2022_line_class copy = (Chaffin2022_line_class)this.MemberwiseClone();
            copy.Gene = (string)this.Gene.Clone();
            copy.Ensembl_ID = (string)this.Ensembl_ID.Clone();
            copy.Gene_type = (string)this.Gene_type.Clone();
            copy.Sub_cluster = (string)this.Sub_cluster.Clone();
            return copy;
        }
    }

    class Chaffin2022_input_readOptions_class : ReadWriteOptions_base
    {
        public Chaffin2022_input_readOptions_class()
        {
            this.File = Global_directory_class.Downloaded_data_directory + "Chaffin_2022_singleNucleus_subclusterMarker_genes.txt";
            this.Key_propertyNames = new string[] { "Gene", "Ensembl_ID", "Gene_type", "Cell_type", "Sub_cluster", "N_nuclei", "PCT_target", "PCT_non_target", "Avg_exp_target", "Avg_exp_nonTargetEXPR", "Non_target_AUC", "Limma_voom_logFC", "Limma_voom_p_value", "Limma_voom_adjusted_p_value" };
            this.Key_columnNames = new string[] { "Gene", "Ensembl ID", "Gene Type", "Cell Type", "Sub-Cluster", "N Nuclei", "PCT > 0: Target", "PCT > 0: Non-target", "AVG EXPR: Target", "AVG EXPR: Non-Target", "AUC", "limma-voom: logFC", "limma-voom: P-Value", "limma-voom: Adjusted P-Value" };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }

    }

    class Chaffin2022_class
    {
        public Chaffin2022_line_class[] Data { get; set; }

        public void Generate_by_reading()
        {
            Read();
            Calculate_fractional_ranks();
        }

        public void Calculate_fractional_ranks()
        {
            this.Data = this.Data.OrderBy(l => l.Cell_type).ThenBy(l => l.Limma_voom_p_value).ThenByDescending(l=>Math.Abs(l.Limma_voom_logFC)).ToArray();
            int data_length = this.Data.Length;
            Chaffin2022_line_class chaffin2022_line;
            Chaffin2022_line_class inner_chaffin2022_line;
            int running_rank = 0;
            int first_rank_sameCellType = -1;
            List<float> current_running_ranks = new List<float>();
            float final_rank;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                chaffin2022_line = this.Data[indexD];
                if (  (indexD==0)
                    ||(!chaffin2022_line.Cell_type.Equals(this.Data[indexD-1].Cell_type)))
                {
                    running_rank = 0;
                }
                if ((indexD == 0)
                    || (!chaffin2022_line.Cell_type.Equals(this.Data[indexD - 1].Cell_type))
                    || (!chaffin2022_line.Limma_voom_p_value.Equals(this.Data[indexD - 1].Limma_voom_p_value))
                    || (!chaffin2022_line.Limma_voom_logFC.Equals(this.Data[indexD - 1].Limma_voom_logFC)))
                {
                    current_running_ranks.Clear();
                    first_rank_sameCellType = indexD;
                }
                running_rank++;
                current_running_ranks.Add(running_rank);
                if ((indexD == data_length-1)
                    || (!chaffin2022_line.Cell_type.Equals(this.Data[indexD + 1].Cell_type))
                    || (!chaffin2022_line.Limma_voom_p_value.Equals(this.Data[indexD + 1].Limma_voom_p_value))
                    || (!chaffin2022_line.Limma_voom_logFC.Equals(this.Data[indexD + 1].Limma_voom_logFC)))
                {
                if (current_running_ranks.Count==1) { final_rank = current_running_ranks[0]; }
                    else { final_rank = Math_class.Get_average(current_running_ranks.ToArray()); }
                    for (int indexInner=first_rank_sameCellType; indexInner<=indexD;indexInner++)
                    {
                        inner_chaffin2022_line = this.Data[indexInner];
                        inner_chaffin2022_line.Fractional_rank = final_rank;
                    }
                }
            }
        }

        public void Keep_only_significant_lines()
        {
            List<Chaffin2022_line_class> keep = new List<Chaffin2022_line_class>();
            int data_length = this.Data.Length;
            Chaffin2022_line_class chaffin2022_line;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                chaffin2022_line = this.Data[indexD];
                if ((!float.IsNaN(chaffin2022_line.Limma_voom_adjusted_p_value))
                     && (chaffin2022_line.Limma_voom_adjusted_p_value <= 0.05))
                {
                    keep.Add(chaffin2022_line);
                }
            }
            this.Data = keep.ToArray();
        }

        public DE_class Generate_new_de_instance()
        {
            int data_length = this.Data.Length;
            Chaffin2022_line_class chaffin2022_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            Fill_de_line_class fill_de_line;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                chaffin2022_line = this.Data[indexD];
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { chaffin2022_line.Cell_type, chaffin2022_line.Sub_cluster };
                fill_de_line.Symbols_for_de = new string[] { chaffin2022_line.Gene };
                fill_de_line.Value_for_de = chaffin2022_line.Limma_voom_logFC;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Diffrna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.E_m_p_t_y;
                fill_de_list.Add(fill_de_line);
            }
            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list.ToArray());
            return de;
        }

        public void Keep_only_lines_with_maxium_fractional_rank(float max_rank)
        {
            List<Chaffin2022_line_class> keep = new List<Chaffin2022_line_class>();
            int data_length = this.Data.Length;
            Chaffin2022_line_class chaffin2022_line;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                chaffin2022_line = this.Data[indexD];
                if (chaffin2022_line.Fractional_rank==-1) { throw new Exception(); }
                if (chaffin2022_line.Fractional_rank<=max_rank)
                {
                    keep.Add(chaffin2022_line);
                }
            }
            this.Data = keep.ToArray();
        }

        private void Read()
        {
            Chaffin2022_input_readOptions_class readOptions = new Chaffin2022_input_readOptions_class();
            this.Data = ReadWriteClass.ReadRawData_and_FillArray<Chaffin2022_line_class>(readOptions);
        }
    }

    ////////////////////////////////////////////////////////////////////////
    
    class Chaffin2022_pseudobulk_line_class
    {
        public string Gene { get; set; }
        public string Cell_type { get; set; }
        public string Test_group { get; set; }
        public string Reference_group { get; set; }
        public string Dataset_name { get; set; }
        public float CellBender_logFC { get; set; }
        public float CellBender_p_value { get; set; }
        public float CellBender_adjusted_p_value { get; set; }
        public float Fractional_rank { get; set; }

        public Chaffin2022_pseudobulk_line_class Deep_copy()
        {
            Chaffin2022_pseudobulk_line_class copy = (Chaffin2022_pseudobulk_line_class)this.MemberwiseClone();
            copy.Gene = (string)this.Gene.Clone();
            copy.Cell_type = (string)this.Cell_type.Clone();
            copy.Test_group = (string)this.Test_group.Clone();
            copy.Reference_group = (string)this.Reference_group.Clone();
            copy.Dataset_name = (string)this.Dataset_name.Clone();
            return copy;
        }
    }

    class Chaffin2022_pseudobulk_readWriteOptions_class : ReadWriteOptions_base
    {
        public Chaffin2022_pseudobulk_readWriteOptions_class(string fileName)
        {
            this.File = Global_directory_class.Downloaded_data_directory + fileName;
            this.Key_propertyNames = new string[] { "Gene", "Cell_type", "Test_group", "Reference_group", "Dataset_name", "CellBender_logFC", "CellBender_p_value", "CellBender_adjusted_p_value" };
            this.Key_columnNames = new string[] { "Gene", "Cell Type", "Test Group", "Reference Group", "Dataset_name", "CellBender:logFC", "CellBender:P-Value", "CellBender:Adjusted P-Value" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Chaffin2022_pseudobulk_class
    {
        public Chaffin2022_pseudobulk_line_class[] Data { get; set; }

        public void Generate_by_reading(string[] fileNames)
        {
            Read(fileNames);
            Calculate_fractional_ranks();
        }

        public void Calculate_fractional_ranks()
        {
            this.Data = this.Data.OrderBy(l => l.Cell_type).ThenBy(l=>l.Reference_group).ThenBy(l=>l.Test_group).ThenBy(l => l.CellBender_p_value).ThenByDescending(l => Math.Abs(l.CellBender_logFC)).ToArray();
            int data_length = this.Data.Length;
            Chaffin2022_pseudobulk_line_class chaffin2022_line;
            Chaffin2022_pseudobulk_line_class inner_chaffin2022_line;
            int running_rank = 0;
            int first_rank_sameCellType = -1;
            List<float> current_running_ranks = new List<float>();
            float final_rank;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                chaffin2022_line = this.Data[indexD];
                if ((indexD == 0)
                    || (!chaffin2022_line.Cell_type.Equals(this.Data[indexD - 1].Cell_type))
                    || (!chaffin2022_line.Reference_group.Equals(this.Data[indexD - 1].Reference_group))
                    || (!chaffin2022_line.Test_group.Equals(this.Data[indexD - 1].Test_group)))
                {
                    running_rank = 0;
                }
                if ((indexD == 0)
                    || (!chaffin2022_line.Cell_type.Equals(this.Data[indexD - 1].Cell_type))
                    || (!chaffin2022_line.Reference_group.Equals(this.Data[indexD - 1].Reference_group))
                    || (!chaffin2022_line.Test_group.Equals(this.Data[indexD - 1].Test_group))
                    || (!chaffin2022_line.CellBender_p_value.Equals(this.Data[indexD - 1].CellBender_p_value))
                    || (!chaffin2022_line.CellBender_logFC.Equals(this.Data[indexD - 1].CellBender_logFC)))
                {
                    current_running_ranks.Clear();
                    first_rank_sameCellType = indexD;
                }
                running_rank++;
                current_running_ranks.Add(running_rank);
                if ((indexD == data_length - 1)
                    || (!chaffin2022_line.Cell_type.Equals(this.Data[indexD + 1].Cell_type))
                    || (!chaffin2022_line.Reference_group.Equals(this.Data[indexD + 1].Reference_group))
                    || (!chaffin2022_line.Test_group.Equals(this.Data[indexD + 1].Test_group))
                    || (!chaffin2022_line.CellBender_p_value.Equals(this.Data[indexD + 1].CellBender_p_value))
                    || (!chaffin2022_line.CellBender_logFC.Equals(this.Data[indexD + 1].CellBender_logFC)))
                {
                    if (current_running_ranks.Count == 1) { final_rank = current_running_ranks[0]; }
                    else { final_rank = Math_class.Get_average(current_running_ranks.ToArray()); }
                    for (int indexInner = first_rank_sameCellType; indexInner <= indexD; indexInner++)
                    {
                        inner_chaffin2022_line = this.Data[indexInner];
                        inner_chaffin2022_line.Fractional_rank = final_rank;
                    }
                }
            }
        }

        public void Keep_only_significant_lines()
        {
            List<Chaffin2022_pseudobulk_line_class> keep = new List<Chaffin2022_pseudobulk_line_class>();
            int data_length = this.Data.Length;
            Chaffin2022_pseudobulk_line_class chaffin2022_line;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                chaffin2022_line = this.Data[indexD];
                if ((!float.IsNaN(chaffin2022_line.CellBender_adjusted_p_value))
                     && (chaffin2022_line.CellBender_adjusted_p_value <= 0.05))
                {
                    keep.Add(chaffin2022_line);
                }
            }
            this.Data = keep.ToArray();
        }

        public DE_class Generate_new_de_instance()
        {
            int data_length = this.Data.Length;
            Chaffin2022_pseudobulk_line_class chaffin2022_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            Fill_de_line_class fill_de_line;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                chaffin2022_line = this.Data[indexD];
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { chaffin2022_line.Cell_type, chaffin2022_line.Test_group + "vs" + chaffin2022_line.Reference_group };
                fill_de_line.Symbols_for_de = new string[] { chaffin2022_line.Gene };
                fill_de_line.Value_for_de = chaffin2022_line.CellBender_logFC;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Diffrna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.E_m_p_t_y;
                fill_de_list.Add(fill_de_line);
            }
            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list.ToArray());
            return de;
        }

        public void Keep_only_lines_with_maxium_fractional_rank(float max_rank)
        {
            List<Chaffin2022_pseudobulk_line_class> keep = new List<Chaffin2022_pseudobulk_line_class>();
            int data_length = this.Data.Length;
            Chaffin2022_pseudobulk_line_class chaffin2022_line;
            for (int indexD = 0; indexD < data_length; indexD++)
            {
                chaffin2022_line = this.Data[indexD];
                if (chaffin2022_line.Fractional_rank == -1) { throw new Exception(); }
                if (chaffin2022_line.Fractional_rank <= max_rank)
                {
                    keep.Add(chaffin2022_line);
                }
            }
            this.Data = keep.ToArray();
        }

        private void Read(string[] fileNames)
        {
            Chaffin2022_pseudobulk_line_class[] add_pseudobulk;
            List<Chaffin2022_pseudobulk_line_class> new_data = new List<Chaffin2022_pseudobulk_line_class>();
            foreach (string fileName in fileNames)
            {
                Chaffin2022_pseudobulk_readWriteOptions_class readOptions = new Chaffin2022_pseudobulk_readWriteOptions_class(fileName);
                add_pseudobulk = ReadWriteClass.ReadRawData_and_FillArray<Chaffin2022_pseudobulk_line_class>(readOptions);
                new_data.AddRange(add_pseudobulk);
            }
            this.Data = new_data.ToArray();
        }
    }
}
