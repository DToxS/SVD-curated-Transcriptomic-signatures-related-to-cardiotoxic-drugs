using System;
using System.Collections.Generic;
using System.Linq;
using Highthroughput_data;
using Enrichment;
using Common_classes;
using ReadWrite;
using Statistic;

namespace SingelCellNucleus_RNAseq
{
    class Published_singleCellNucleusRNAseq_line_class
    {
        public string Comparison { get; set; }
        public string Cell_type { get; set; }
        public double Pvalue { get; set; }
        public double Adj_pvalue { get; set; }
        public string Nominator { get; set; }
        public string Denominator { get; set; }
        public double Log2FC { get; set; }
        public string Gene_symbol { get; set; }
        public float Fractional_rank { get; set; }
    }
    class Published_singleCellNucleusRNAseq_chaffin_readOptions_class : ReadWriteOptions_base
    {
        public Published_singleCellNucleusRNAseq_chaffin_readOptions_class(string fileName)
        {
            this.File = Global_directory_class.Downloaded_data_directory + fileName;
            this.Key_propertyNames = new string[] { "Cell_type", "Nominator", "Denominator", "Gene_symbol", "Adj_pvalue", "Log2FC" };
            this.Key_columnNames = new string[] { "Cell Type", "Test Group", "Reference Group", "Gene", "CellRanger:Adjusted P-Value","CellRanger:logFC" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }
    class Published_singleCellNucleusRNAseq_koenig_readOptions_class : ReadWriteOptions_base
    {
        public Published_singleCellNucleusRNAseq_koenig_readOptions_class(string fileName)
        {
            this.File = Global_directory_class.Downloaded_data_directory + fileName;
            this.Key_propertyNames = new string[] { "Comparison", "Gene_symbol", "Adj_pvalue", "Log2FC" };
            this.Key_columnNames = new string[] { "Cluster", "Gene_symbol", "padj", "log2FoldChange" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }
    class Published_singleCellNucleusRNAseq_chun_readOptions_class : ReadWriteOptions_base
    {
        public Published_singleCellNucleusRNAseq_chun_readOptions_class(string fileName)
        {
            this.File = Global_directory_class.Downloaded_data_directory + fileName;
            this.Key_propertyNames = new string[] { "Gene_symbol", "Adj_pvalue", "Log2FC" };
            this.Key_columnNames = new string[] { "gene", "p_val_adj", "avg_logFC" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }
    class Published_singleCellNulceusRNAseq_readWriteOptions_class : ReadWriteOptions_base
    {
        public Published_singleCellNulceusRNAseq_readWriteOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Comparison", "Gene_symbol", "Pvalue", "Adj_pvalue", "Log2FC", "Fractional_rank", "Cell_type", "Nominator", "Denominator" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }
    class Published_singleCellNucleusRNAseq_class
    {
        public Published_singleCellNucleusRNAseq_line_class[] Published_data { get; set; }

        public Published_singleCellNucleusRNAseq_class()
        {
            this.Published_data = new Published_singleCellNucleusRNAseq_line_class[0];
        }

        public void Add_to_array(Published_singleCellNucleusRNAseq_line_class[] add_published_data)
        {
            int this_length = this.Published_data.Length;
            int add_length = add_published_data.Length;
            int new_length = add_length + this_length;
            Published_singleCellNucleusRNAseq_line_class[] new_published_data = new Published_singleCellNucleusRNAseq_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_published_data[indexNew] = this.Published_data[indexThis];
            }
            for (int indexAdd = 0; indexAdd< add_length; indexAdd++)
            {
                indexNew++;
                new_published_data[indexNew] = add_published_data[indexAdd];
            }
            this.Published_data = new_published_data;
        }

        #region Filter
        public void Calculate_fractional_ranks_based_on_adjpvalues_and_absLog2FC()
        {
            this.Published_data = this.Published_data.OrderBy(l => l.Comparison).ThenBy(l => l.Adj_pvalue).ThenByDescending(l => Math.Abs(l.Log2FC)).ToArray();
            float running_rank=-9999;
            float current_rank;
            List<float> current_ranks = new List<float>();
            Published_singleCellNucleusRNAseq_line_class published_line;
            Published_singleCellNucleusRNAseq_line_class inner_published_line;
            int firstIndexSameValues = -1;
            int published_length = this.Published_data.Length;
            for (int indexPub=0; indexPub< published_length;indexPub++)
            {
                published_line = this.Published_data[indexPub];
                if ((indexPub == 0)
                    || (!published_line.Comparison.Equals(this.Published_data[indexPub - 1].Comparison)))
                {
                    running_rank = 0;
                }
                if ((indexPub == 0)
                    || (!published_line.Comparison.Equals(this.Published_data[indexPub - 1].Comparison))
                    || (!published_line.Adj_pvalue.Equals(this.Published_data[indexPub - 1].Adj_pvalue))
                    || (!Math.Abs(published_line.Log2FC).Equals(Math.Abs(this.Published_data[indexPub - 1].Log2FC))))
                {
                    current_ranks.Clear();
                    firstIndexSameValues = indexPub;
                }
                running_rank++;
                current_ranks.Add(running_rank);
                if ((indexPub == published_length-1)
                    || (!published_line.Comparison.Equals(this.Published_data[indexPub + 1].Comparison))
                    || (!published_line.Adj_pvalue.Equals(this.Published_data[indexPub + 1].Adj_pvalue))
                    || (!Math.Abs(published_line.Log2FC).Equals(Math.Abs(this.Published_data[indexPub + 1].Log2FC))))
                {
                    if (current_ranks.Count==1)
                    {
                        current_rank = current_ranks[0];
                    }
                    else
                    {
                        current_rank = Math_class.Get_average(current_ranks.ToArray());
                    }
                    for (int indexInner=firstIndexSameValues;indexInner<=indexPub;indexInner++)
                    {
                        inner_published_line = this.Published_data[indexInner];
                        inner_published_line.Fractional_rank = current_rank;
                    }
                }
            }
        }
        public void Keep_only_lines_belowOrEqual_adj_pvalue_cutoff(float adj_pvalue_cutoff)
        {
            List<Published_singleCellNucleusRNAseq_line_class> keep = new List<Published_singleCellNucleusRNAseq_line_class>();
            Published_singleCellNucleusRNAseq_line_class published_line;
            int published_length = this.Published_data.Length;
            for (int indexPub=0; indexPub<published_length; indexPub++)
            {
                published_line = this.Published_data[indexPub];
                if (published_line.Adj_pvalue <= adj_pvalue_cutoff)
                {
                    keep.Add(published_line);
                }
            }
            this.Published_data = keep.ToArray();
        }
        public void Keep_only_lines_belowOrEqual_fractionalRankCutoff_after_calculating_ranks_based_on_adjpvalue_absLog2FC(float max_rank_cutoff)
        {
            Calculate_fractional_ranks_based_on_adjpvalues_and_absLog2FC();
            List<Published_singleCellNucleusRNAseq_line_class> keep = new List<Published_singleCellNucleusRNAseq_line_class>();
            Published_singleCellNucleusRNAseq_line_class published_line;
            int published_length = this.Published_data.Length;
            for (int indexPub = 0; indexPub < published_length; indexPub++)
            {
                published_line = this.Published_data[indexPub];
                if (published_line.Fractional_rank <= max_rank_cutoff)
                {
                    keep.Add(published_line);
                }
            }
            this.Published_data = keep.ToArray();
        }
        #endregion

        public DE_class Generate_de_instance()
        {
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            foreach (Published_singleCellNucleusRNAseq_line_class published_line in this.Published_data)
            {
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Entry_type_for_de = DE_entry_enum.Diffrna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_line.Symbols_for_de = new string[] { (string)published_line.Gene_symbol.Clone() };
                fill_de_line.Value_for_de = published_line.Log2FC;
                fill_de_line.Names_for_de = new string[] { (string)published_line.Comparison.Clone() };
                fill_de_list.Add(fill_de_line);
            }
            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list.ToArray());
            return de;
        }

        #region Read
        public void Read_and_add_to_array(string fileName)
        {
            if (fileName.IndexOf("Koenig") == 0) { Read_koenig_and_add_to_array(fileName); }
            else if (fileName.IndexOf("Chaffin") == 0) { Read_chaffin_and_add_to_array(fileName); }
            else if (fileName.IndexOf("Chun") == 0) { Read_chun_and_add_to_array(fileName); }
            else { throw new Exception(); }
        }
        private void Read_koenig_and_add_to_array(string fileName)
        {
            Published_singleCellNucleusRNAseq_koenig_readOptions_class readWriteOptions = new Published_singleCellNucleusRNAseq_koenig_readOptions_class(fileName);
            Published_singleCellNucleusRNAseq_line_class[] add_published_data = ReadWriteClass.ReadRawData_and_FillArray<Published_singleCellNucleusRNAseq_line_class>(readWriteOptions);
            this.Add_to_array(add_published_data);
        }
        private void Read_chaffin_and_add_to_array(string fileName)
        {
            Published_singleCellNucleusRNAseq_chaffin_readOptions_class readWriteOptions = new Published_singleCellNucleusRNAseq_chaffin_readOptions_class(fileName);
            Published_singleCellNucleusRNAseq_line_class[] add_published_data = ReadWriteClass.ReadRawData_and_FillArray<Published_singleCellNucleusRNAseq_line_class>(readWriteOptions);
            foreach (Published_singleCellNucleusRNAseq_line_class published_line in add_published_data)
            {
                published_line.Comparison = published_line.Nominator + "_vs_" + published_line.Denominator + "_in_" + published_line.Cell_type;
            }
            this.Add_to_array(add_published_data);
        }
        private void Read_chun_and_add_to_array(string fileName)
        {
            Published_singleCellNucleusRNAseq_chun_readOptions_class readWriteOptions = new Published_singleCellNucleusRNAseq_chun_readOptions_class(fileName);
            Published_singleCellNucleusRNAseq_line_class[] add_published_data = ReadWriteClass.ReadRawData_and_FillArray<Published_singleCellNucleusRNAseq_line_class>(readWriteOptions);
            bool is_downregulated = fileName.IndexOf("downregulated") != -1;
            bool is_upregulated = fileName.IndexOf("upregulated") != -1;
            foreach (Published_singleCellNucleusRNAseq_line_class published_line in add_published_data)
            {
                published_line.Comparison = "DCM_vs_healthy_in_iPSCdCMs";
                if (is_downregulated)
                {
                    published_line.Log2FC = -(Math.Abs(published_line.Log2FC));
                }
                else if (is_upregulated)
                {
                    published_line.Log2FC = (Math.Abs(published_line.Log2FC));
                }
                else { throw new Exception(); }
            }
            this.Add_to_array(add_published_data);
        }

        public void Write(string directory, string fileName)
        {
            Published_singleCellNulceusRNAseq_readWriteOptions_class readWriteOptions = new Published_singleCellNulceusRNAseq_readWriteOptions_class(directory, fileName);
            ReadWriteClass.WriteData(this.Published_data, readWriteOptions);
        }
        #endregion
    }


    class SingleCellNucleusRNAseq_analysis_class
    {
        public void Do_enrichment_analysis_for_Schaniel_iPSCdCM_singleCell_cardiomyocyte()
        {
            bool only_up = true;
            float max_adj_pvalue = 0.05F;
            int top_DEGs = 500;
            string enrich_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory;
            string enrich_addToFileName = "ScRNAseq_iPSCdCM_schaniel";
            string singleCellRNAseq_markers_fileName = "ScRNAseq_iPSCdCM_schaniel_markerGenes.txt";
            string singleCellRNAseq_directory = Global_directory_class.ScRNAseq_schaniel_markerGenes_directory;
            string singleCellRNAseq_bgGenes_fileName = "ScRNAseq_iPSCdCM_schaniel_bgGenes.txt";

            SingleCellCluster_class single_cell_cluster = new SingleCellCluster_class();
            single_cell_cluster.Generate_by_reading_r_outputs_and_add_to_array(singleCellRNAseq_directory, singleCellRNAseq_markers_fileName, singleCellRNAseq_directory, singleCellRNAseq_bgGenes_fileName);
            single_cell_cluster.Keep_only_significant_genes_after_pvalue_adjustment_and_write_no_data_conditions(max_adj_pvalue, "", "NoDEGs");
            if (only_up) { single_cell_cluster.Keep_only_upregulated_genes(); }
            single_cell_cluster.Keep_only_top_x_predictions_based_on_adjPvalue_and_decreasing_log2FC_per_cluster_and_datasetName_after_calculation_of_fractional_ranks_and_check_if_at_least_one_kept_line_per_condition_has_nonZeroPvalue_or_nonInfinitylogFC(top_DEGs);
            single_cell_cluster.Write(enrich_directory, "DEGs_" + enrich_addToFileName + ".txt");
            DE_class de_single_cell = single_cell_cluster.Generate_data_instance();
            string[] bg_genes = single_cell_cluster.Get_bgGenesInUpperCase();

            Dictionary<string, Ontology_type_enum[]> analysis_ontologies_dict = new Dictionary<string, Ontology_type_enum[]>();
            analysis_ontologies_dict.Add("CompareWithScRNAseq", new Ontology_type_enum[] { Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes, Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes });
            analysis_ontologies_dict.Add("MBCO", new Ontology_type_enum[] { Ontology_type_enum.Mbco_level1, Ontology_type_enum.Mbco_level2, Ontology_type_enum.Mbco_level3, Ontology_type_enum.Mbco_level4 });
            Dictionary<string, float> analysis_maxPvalues_dict = new Dictionary<string, float>();
            analysis_maxPvalues_dict.Add("CompareWithScRNAseq", 1);
            analysis_maxPvalues_dict.Add("MBCO", 0.05F);
            Dictionary<string, int> analysis_TopXPredictions_dict = new Dictionary<string, int>();
            analysis_TopXPredictions_dict.Add("CompareWithScRNAseq", 5);
            analysis_TopXPredictions_dict.Add("MBCO", 999999);

            string[] analyses = analysis_maxPvalues_dict.Keys.ToArray();
            foreach (string analysis in analyses)
            {
                Downstream_analysis_2020_class downstream = new Downstream_analysis_2020_class();
                downstream.Options.Data_value_signs_of_interest = new Data_value_signs_of_interest_enum[] { Data_value_signs_of_interest_enum.Upregulated };
                downstream.Options.Ontologies = analysis_ontologies_dict[analysis];
                downstream.Options.Top_top_x_predictions = analysis_TopXPredictions_dict[analysis];
                downstream.Options.Max_pvalue = analysis_TopXPredictions_dict[analysis];
                downstream.Generate(bg_genes);
                downstream.Analyse_de_instance_and_return_unfiltered_enrichment_results(de_single_cell, enrich_directory, enrich_addToFileName);
            }
        }
        public void Do_enrichment_analysis_for_Litvinukova_2020_cellsAdultHumanHeart()
        { 
            float max_adj_pvalue = 0.05F;
            int top_DEGs = 500;
            bool only_up = true;

            string sc_marker_directory = Global_directory_class.ScRNAseq_litvinukova_markerGenes_directory;
            string sc_marker_fileName = "CellType_markers_Litvinukova_2020_cellsAdultHumanHeart.txt";
            string sc_marker_bgGenes = "BgGenes_Litvinukova_2020_cellsAdultHumanHeart.txt";
            string results_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory;
            string enrich_addTofileName = "ScRNAseq_litvinukova_adultHeart";

            SingleCellCluster_class single_cell_cluster = new SingleCellCluster_class();
            single_cell_cluster.Generate_by_reading_r_outputs_and_add_to_array(sc_marker_directory + "/", sc_marker_fileName, sc_marker_directory, sc_marker_bgGenes);
            single_cell_cluster.Keep_only_significant_genes_after_pvalue_adjustment_and_write_no_data_conditions(max_adj_pvalue, "", "NoDEGs");
            if (only_up) { single_cell_cluster.Keep_only_upregulated_genes(); }
            single_cell_cluster.Keep_only_top_x_predictions_based_on_adjPvalue_and_decreasing_log2FC_per_cluster_and_datasetName_after_calculation_of_fractional_ranks_and_check_if_at_least_one_kept_line_per_condition_has_nonZeroPvalue_or_nonInfinitylogFC(top_DEGs);
            single_cell_cluster.Write(results_directory, "DEGs_" + enrich_addTofileName + ".txt");

            string[] bgGenes = single_cell_cluster.Get_bgGenesInUpperCase();

            DE_class de_singleCell = single_cell_cluster.Generate_data_instance();

            Downstream_analysis_2020_class downstream = new Downstream_analysis_2020_class();
            downstream.Options.Data_value_signs_of_interest = new Data_value_signs_of_interest_enum[] { Data_value_signs_of_interest_enum.Upregulated };
            downstream.Options.Ontologies = new Ontology_type_enum[] { Ontology_type_enum.Mbco_level1, Ontology_type_enum.Mbco_level2, Ontology_type_enum.Mbco_level3, Ontology_type_enum.Mbco_level4 };
            downstream.Options.Top_top_x_predictions = 999999;
            downstream.Options.Max_pvalue = 0.05F;
            downstream.Generate(bgGenes);
            downstream.Analyse_de_instance_and_return_unfiltered_enrichment_results(de_singleCell, results_directory, enrich_addTofileName);
        }
        public void Do_enrichment_analysis_for_chaffin_koenig_and_chun_HCM_DCM_vs_NF()
        {
            float max_adj_pvalue = 0.05F;// 0.05F;
            int top_DEGs = 600;

            string sc_marker_directory = Global_directory_class.Downloaded_data_directory;
            Dictionary<string, string[]> author_fileNames_dict = new Dictionary<string, string[]>();
            author_fileNames_dict.Add("SnRNASeq_chaffin_DCM_HCM", new string[] { "Chaffin_2022_singleNucleus_DCMvsNF.txt", "Chaffin_2022_singleNucleus_HCMvsNF.txt" });
            author_fileNames_dict.Add("ScRNASeq_koenig_DCM", new string[] { "Koenig_2022_singleCell_DCMvsHealthy.txt" });
            author_fileNames_dict.Add("ScRNASeq_chun_DCM", new string[] { "Chun_2023_upregulated_in_LVIP.txt", "Chun_2023_downregulated_in_LVIP.txt" });

            string results_directory = Global_directory_class.ScSnRNAseq_enrichment_results_directory;
            string[] authors = author_fileNames_dict.Keys.ToArray();
            string author;
            int authors_length = authors.Length;
            string[] fileNames;

            Downstream_analysis_2020_class downstream = new Downstream_analysis_2020_class();
            downstream.Options.Data_value_signs_of_interest = new Data_value_signs_of_interest_enum[] { Data_value_signs_of_interest_enum.Upregulated, Data_value_signs_of_interest_enum.Downregulated };
            downstream.Options.Ontologies = new Ontology_type_enum[] { Ontology_type_enum.Mbco_level1, Ontology_type_enum.Mbco_level2, Ontology_type_enum.Mbco_level3, Ontology_type_enum.Mbco_level4 };
            downstream.Options.Top_top_x_predictions = 999999;
            downstream.Options.Max_pvalue = 0.05F;
            downstream.Generate();

            for (int indexA=0; indexA<authors_length; indexA++)
            {
                author = authors[indexA];
                fileNames = author_fileNames_dict[author];
                Published_singleCellNucleusRNAseq_class published = new Published_singleCellNucleusRNAseq_class();
                foreach (string fileName in fileNames)
                {
                    published.Read_and_add_to_array(fileName);
                }
                published.Keep_only_lines_belowOrEqual_adj_pvalue_cutoff(max_adj_pvalue);
                published.Keep_only_lines_belowOrEqual_fractionalRankCutoff_after_calculating_ranks_based_on_adjpvalue_absLog2FC(top_DEGs);
                published.Write(results_directory, "DEGs_" + author + ".txt");
                DE_class de = published.Generate_de_instance();
                downstream.Analyse_de_instance_and_return_unfiltered_enrichment_results(de, results_directory, author);
            }
        }

    }
}
