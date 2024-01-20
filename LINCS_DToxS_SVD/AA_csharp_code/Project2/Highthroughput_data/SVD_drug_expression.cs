using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReadWrite;
using Common_classes;
using Statistic;
using Enrichment;
using System.Web;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Highthroughput_data
{
    enum Read_svd_oneEntity_of_entityClass_eachTime_enum {  E_m_p_t_y, Drug, Cell_line }

    class Lincs_rcolor_line_class
    {
        public string EntityClass { get; set; }
        public string Entity { get; set; }
        public string Rcolor { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public string Hex { get; set; }
        public System.Drawing.Color Csharp_color { get; set; }
        public Lincs_rcolor_line_class Deep_copy()
        {
            Lincs_rcolor_line_class copy = (Lincs_rcolor_line_class)this.MemberwiseClone();
            copy.EntityClass = (string)this.EntityClass.Clone();
            copy.Entity = (string)this.Entity.Clone();
            copy.Rcolor = (string)this.Rcolor.Clone();
            copy.Hex = (string)this.Hex.Clone();
            return copy;
        }
    }

    class Lincs_rcolor_input_readOptions_class : ReadWriteOptions_base
    {
        public Lincs_rcolor_input_readOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "EntityClass", "Entity", "Rcolor", "Red", "Green", "Blue", "Hex" };
            this.Key_columnNames = this.Key_propertyNames;
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Lincs_rcolor_readWriteOptions_class : ReadWriteOptions_base
    {
        public Lincs_rcolor_readWriteOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "EntityClass", "Entity", "Csharp_color","Rcolor", "Red", "Green", "Blue", "Hex" };
            this.Key_columnNames = this.Key_propertyNames;
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Lincs_rcolor_class
    {
        public Lincs_rcolor_line_class[] Rcolors { get; set; }
        public System.Drawing.Color[] All_csharp_colors { get; set; }

        public Lincs_rcolor_class()
        {
            Dictionary<System.Drawing.Color, bool> selectable_colors_dict = new Dictionary<System.Drawing.Color, bool>();

            System.Drawing.Color add_color;
            foreach (System.Reflection.PropertyInfo property in typeof(System.Drawing.Color).GetProperties())
            {
                if (property.PropertyType == typeof(System.Drawing.Color))
                {
                    add_color = (System.Drawing.Color)property.GetValue(null,null);
                    if ((!selectable_colors_dict.ContainsKey(add_color))
                        && (!add_color.Equals(System.Drawing.Color.White))
                        && (!add_color.Equals(System.Drawing.Color.Transparent)))
                    {
                        selectable_colors_dict.Add(add_color, false);
                    }
                }
            }
            All_csharp_colors = selectable_colors_dict.Keys.ToArray();
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
            for (int indexColor=0; indexColor< all_colors_length;indexColor++)
            {
                current_color = All_csharp_colors[indexColor];
                csharp_blue = int.Parse(current_color.B.ToString());
                csharp_red = int.Parse(current_color.R.ToString());
                csharp_green = int.Parse(current_color.G.ToString());
                current_distance = (float)Math.Sqrt(Math.Pow(input_red - csharp_red,2) + Math.Pow(input_blue - csharp_blue,2) + Math.Pow(input_green - csharp_green,2));
                if (current_distance<minimum_distance)
                {
                    minimum_distance = current_distance;
                    selected_csharp_color = current_color;
                }
            }
            return selected_csharp_color;
        }


        private void Add_closet_csharp_color()
        {
            int rcolors_length = this.Rcolors.Length;
            Lincs_rcolor_line_class rcolor_line;
            for (int indexRC=0; indexRC<rcolors_length; indexRC++)
            {
                rcolor_line = this.Rcolors[indexRC];
                rcolor_line.Csharp_color = Get_closest_csharp_color(rcolor_line.Red, rcolor_line.Green, rcolor_line.Blue);
            }
        }

        public void Generate_after_reading(string directory, string fileName)
        {
            Read(directory, fileName);
            Add_closet_csharp_color();
        }

        public Dictionary<string,System.Drawing.Color> Get_entity_color_dict()
        {
            Dictionary<string, System.Drawing.Color> entity_color_dict = new Dictionary<string, System.Drawing.Color>();
            foreach (Lincs_rcolor_line_class rcolor_line in this.Rcolors)
            {
                entity_color_dict.Add(rcolor_line.Entity, rcolor_line.Csharp_color);
            }
            return entity_color_dict;
        }

        private void Read(string directory, string fileName)
        {
            Lincs_rcolor_input_readOptions_class readWriteOptions = new Lincs_rcolor_input_readOptions_class(directory, fileName);
            this.Rcolors = ReadWriteClass.ReadRawData_and_FillArray<Lincs_rcolor_line_class>(readWriteOptions);
        }
        public void Write(string directory, string fileName)
        {
            Lincs_rcolor_readWriteOptions_class readWriteOptions = new Lincs_rcolor_readWriteOptions_class(directory, fileName);
            ReadWriteClass.WriteData<Lincs_rcolor_line_class>(this.Rcolors,readWriteOptions);
        }
    }

    class Lincs_auc_cutoff_ranks_line_class
    {
        public string Ontology_R_name { get; set; }
        public Ontology_type_enum Ontology { get; set; }
        public float EnrichmentCutoffRank_for_AUC { get; set; }

        public Lincs_auc_cutoff_ranks_line_class Deep_copy()
        {
            Lincs_auc_cutoff_ranks_line_class copy = (Lincs_auc_cutoff_ranks_line_class)this.MemberwiseClone();
            copy.Ontology_R_name = (string)this.Ontology_R_name.Clone();
            return copy;
        }
    }

    class Lincs_auc_cutoff_ranks_input_readOptions_class : ReadWriteOptions_base
    {
        public Lincs_auc_cutoff_ranks_input_readOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Ontology_R_name", "Ontology", "EnrichmentCutoffRank_for_AUC" };
            this.Key_columnNames = new string[] { "Ontology_R_name", "Ontology", "AUC_cutoff_rank" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_auc_cutoff_ranks_class
    {
        public Lincs_auc_cutoff_ranks_line_class[] AUC_cutoff_ranks { get; set; }

        public void Generate_by_reading(string directory, string fileName)
        {
            Read(directory, fileName);
        }

        public Dictionary<Ontology_type_enum,float> Get_ontology_enrichmentRankCutoffForAUC_dict()
        {
            Dictionary<Ontology_type_enum, float> ontology_enrichmentRankCutoffForAUC_dict = new Dictionary<Ontology_type_enum, float>();
            foreach (Lincs_auc_cutoff_ranks_line_class auc_line in AUC_cutoff_ranks)
            {
                ontology_enrichmentRankCutoffForAUC_dict.Add(auc_line.Ontology, auc_line.EnrichmentCutoffRank_for_AUC);
            }
            return ontology_enrichmentRankCutoffForAUC_dict;
        }

        private void Read(string directory, string fileName)
        {
            Lincs_auc_cutoff_ranks_input_readOptions_class readOptions = new Lincs_auc_cutoff_ranks_input_readOptions_class(directory,fileName);
            this.AUC_cutoff_ranks = ReadWriteClass.ReadRawData_and_FillArray<Lincs_auc_cutoff_ranks_line_class>(readOptions);
        }
    }



    class SVD_drug_specific_expression_line_class
    {
        public string EntityClass { get; set; }
        public string Drug { get; set; }
        public string Drug_fullName { get; set; }
        public string Experiment { get; set; }
        public string Cell_line { get; set; }
        public string Outlier_cell_line { get; set; }
        public string Plate { get; set; }
        public Timepoint_enum Timepoint { get; set; }
        public DE_entry_enum Entry_type { get; set; }
        public Drug_type_enum Drug_class { get; set; }
        public string Gene_symbol { get; set; }
        public string DEG_dataset { get; set; }
        public float Value { get; set; }
        public float Cardiotoxixity_frequencyGroup { get; set; }
        public string Is_cardiotoxic { get; set; }
        public string Reference_valueType { get; set; }
        public string Dataset { get; set; }
        public string Correlation_method { get; set; }
        public string Preprocess_data { get; set; }
        public string Decomposition_method { get; set; }
        public float F1_score_weight { get; set; }
        public float Fractional_rank { get; set; }
        public string Cardiotoxicity_of_interest { get; set; }

        public string Processing_method { get; set; }

        public static SVD_drug_specific_expression_line_class[] Order_by_experiment_plate_drug_cellline_descending_value(SVD_drug_specific_expression_line_class[] expression_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>> experiment_plate_drug_cellline_descendingValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>> plate_drug_cellline_descendingValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>> drug_cellline_descendingValue_dict = new Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>();
            Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>> cellline_descendingValue_dict = new Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>();
            Dictionary<float, List<SVD_drug_specific_expression_line_class>> descendingValue_dict = new Dictionary<float, List<SVD_drug_specific_expression_line_class>>();

            int expression_lines_length = expression_lines.Length;
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexE = 0; indexE < expression_lines_length; indexE++)
            {
                expression_line = expression_lines[indexE];
                if (!experiment_plate_drug_cellline_descendingValue_dict.ContainsKey(expression_line.Experiment))
                {
                    experiment_plate_drug_cellline_descendingValue_dict.Add(expression_line.Experiment, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>());
                }
                if (!experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment].ContainsKey(expression_line.Plate))
                {
                    experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment].Add(expression_line.Plate, new Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>());
                }
                if (!experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment][expression_line.Plate].ContainsKey(expression_line.Drug))
                {
                    experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment][expression_line.Plate].Add(expression_line.Drug, new Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>());
                }
                if (!experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment][expression_line.Plate][expression_line.Drug].ContainsKey(expression_line.Cell_line))
                {
                    experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment][expression_line.Plate][expression_line.Drug].Add(expression_line.Cell_line, new Dictionary<float, List<SVD_drug_specific_expression_line_class>>());
                }
                if (!experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment][expression_line.Plate][expression_line.Drug][expression_line.Cell_line].ContainsKey(expression_line.Value))
                {
                    experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment][expression_line.Plate][expression_line.Drug][expression_line.Cell_line].Add(expression_line.Value, new List<SVD_drug_specific_expression_line_class>());
                }
                experiment_plate_drug_cellline_descendingValue_dict[expression_line.Experiment][expression_line.Plate][expression_line.Drug][expression_line.Cell_line][expression_line.Value].Add(expression_line);
            }

            string[] experiments;
            string experiment;
            int experiments_length;
            string[] plates;
            string plate;
            int plates_length;
            string[] drugs;
            string drug;
            int drugs_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            float[] descendingValues;
            float descendingValue;
            int descendingValues_length;

            List<SVD_drug_specific_expression_line_class> ordered_expression_list = new List<SVD_drug_specific_expression_line_class>();

            experiments = experiment_plate_drug_cellline_descendingValue_dict.Keys.ToArray();
            experiments_length = experiments.Length;
            experiments = experiments.OrderBy(l => l).ToArray();
            for (int indexEx = 0; indexEx < experiments_length; indexEx++)
            {
                experiment = experiments[indexEx];
                plate_drug_cellline_descendingValue_dict = experiment_plate_drug_cellline_descendingValue_dict[experiment];
                plates = plate_drug_cellline_descendingValue_dict.Keys.ToArray();
                plates = plates.OrderBy(l => l).ToArray();
                plates_length = plates.Length;
                for (int indexP = 0; indexP < plates_length; indexP++)
                {
                    plate = plates[indexP];
                    drug_cellline_descendingValue_dict = plate_drug_cellline_descendingValue_dict[plate];
                    drugs = drug_cellline_descendingValue_dict.Keys.ToArray();
                    drugs = drugs.OrderBy(l => l).ToArray();
                    drugs_length = drugs.Length;
                    for (int indexD = 0; indexD < drugs_length; indexD++)
                    {
                        drug = drugs[indexD];
                        cellline_descendingValue_dict = drug_cellline_descendingValue_dict[drug];
                        celllines = cellline_descendingValue_dict.Keys.ToArray();
                        celllines = celllines.OrderBy(l => l).ToArray();
                        celllines_length = celllines.Length;
                        for (int indexC = 0; indexC < celllines_length; indexC++)
                        {
                            cellline = celllines[indexC];
                            descendingValue_dict = cellline_descendingValue_dict[cellline];
                            descendingValues = descendingValue_dict.Keys.ToArray();
                            descendingValues = descendingValues.OrderByDescending(l => l).ToArray();
                            descendingValues_length = descendingValues.Length;
                            for (int indexDV = 0; indexDV < descendingValues_length; indexDV++)
                            {
                                descendingValue = descendingValues[indexDV];
                                ordered_expression_list.AddRange(descendingValue_dict[descendingValue]);
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_expression_list.Count;
                if (ordered_length != expression_lines_length) { throw new Exception(); }
                SVD_drug_specific_expression_line_class previous_line;
                SVD_drug_specific_expression_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_expression_list[indexO - 1];
                    this_line = ordered_expression_list[indexO];
                    if (this_line.Experiment.CompareTo(previous_line.Experiment) < 0) { throw new Exception(); }
                    else if ((this_line.Experiment.Equals(previous_line.Experiment))
                             && (this_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }
                    else if ((this_line.Experiment.Equals(previous_line.Experiment))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Drug.CompareTo(previous_line.Drug) < 0)) { throw new Exception(); }
                    else if ((this_line.Experiment.Equals(previous_line.Experiment))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Experiment.Equals(previous_line.Experiment))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Value.CompareTo(previous_line.Value) > 0)) { throw new Exception(); }
                }
            }

            return ordered_expression_list.ToArray();
        }

        public static SVD_drug_specific_expression_line_class[] Order_by_drug_cellLine_f1ScoreWeigth_geneSymbol(SVD_drug_specific_expression_line_class[] expression_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>> drug_cellLine_f1ScoreWeight_geneSymbol_dict = new Dictionary<string, Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>>();
            Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>> cellLine_f1ScoreWeight_geneSymbol_dict = new Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>();
            Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>> f1ScoreWeight_geneSymbol_dict = new Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>();
            Dictionary<string, List<SVD_drug_specific_expression_line_class>> geneSymbol_dict = new Dictionary<string, List<SVD_drug_specific_expression_line_class>>();

            int expression_lines_length = expression_lines.Length;
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexE = 0; indexE < expression_lines_length; indexE++)
            {
                expression_line = expression_lines[indexE];
                if (!drug_cellLine_f1ScoreWeight_geneSymbol_dict.ContainsKey(expression_line.Drug))
                {
                    drug_cellLine_f1ScoreWeight_geneSymbol_dict.Add(expression_line.Drug, new Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>());
                }
                if (!drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Drug].ContainsKey(expression_line.Cell_line))
                {
                    drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Drug].Add(expression_line.Cell_line, new Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>());
                }
                if (!drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Drug][expression_line.Cell_line].ContainsKey(expression_line.F1_score_weight))
                {
                    drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Drug][expression_line.Cell_line].Add(expression_line.F1_score_weight, new Dictionary<string, List<SVD_drug_specific_expression_line_class>>());
                }
                if (!drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Drug][expression_line.Cell_line][expression_line.F1_score_weight].ContainsKey(expression_line.Gene_symbol))
                {
                    drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Drug][expression_line.Cell_line][expression_line.F1_score_weight].Add(expression_line.Gene_symbol, new List<SVD_drug_specific_expression_line_class>());
                }
                drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Drug][expression_line.Cell_line][expression_line.F1_score_weight][expression_line.Gene_symbol].Add(expression_line);
            }

            string[] drugs;
            string drug;
            int drugs_length;
            string[] cellLines;
            string cellLine;
            int cellLines_length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            float[] f1_score_weights;
            float f1_score_weight;
            int f1_score_weights_length;

            List<SVD_drug_specific_expression_line_class> ordered_expression_list = new List<SVD_drug_specific_expression_line_class>();


            drugs = drug_cellLine_f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
            drugs = drugs.OrderBy(l => l).ToArray();
            drugs_length = drugs.Length;
            for (int indexD = 0; indexD < drugs_length; indexD++)
            {
                drug = drugs[indexD];
                cellLine_f1ScoreWeight_geneSymbol_dict = drug_cellLine_f1ScoreWeight_geneSymbol_dict[drug];
                cellLines = cellLine_f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
                cellLines = cellLines.OrderBy(l => l).ToArray();
                cellLines_length = cellLines.Length;
                for (int indexCell = 0; indexCell < cellLines_length; indexCell++)
                {
                    cellLine = cellLines[indexCell];
                    f1ScoreWeight_geneSymbol_dict = cellLine_f1ScoreWeight_geneSymbol_dict[cellLine];
                    f1_score_weights = f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
                    f1_score_weights = f1_score_weights.OrderBy(l => l).ToArray();
                    f1_score_weights_length = f1_score_weights.Length;
                    for (int indexF1 = 0; indexF1 < f1_score_weights_length; indexF1++)
                    {
                        f1_score_weight = f1_score_weights[indexF1];
                        geneSymbol_dict = f1ScoreWeight_geneSymbol_dict[f1_score_weight];
                        geneSymbols = geneSymbol_dict.Keys.ToArray();
                        geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                        geneSymbols_length = geneSymbols.Length;
                        for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
                        {
                            geneSymbol = geneSymbols[indexGS];
                            ordered_expression_list.AddRange(geneSymbol_dict[geneSymbol]);
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_expression_list.Count;
                if (ordered_length != expression_lines_length) { throw new Exception(); }
                SVD_drug_specific_expression_line_class previous_line;
                SVD_drug_specific_expression_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_expression_list[indexO - 1];
                    this_line = ordered_expression_list[indexO];
                    if (this_line.Drug.CompareTo(previous_line.Drug) < 0) { throw new Exception(); }
                    else if ((this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.F1_score_weight.CompareTo(previous_line.F1_score_weight) < 0)) { throw new Exception(); }
                    else if ((this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.F1_score_weight.Equals(previous_line.F1_score_weight))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                }
            }

            return ordered_expression_list.ToArray();
        }

        public static SVD_drug_specific_expression_line_class[] Order_by_cardiotoxicityOfInterest_drug_cellLine_f1ScoreWeigth_geneSymbol(SVD_drug_specific_expression_line_class[] expression_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>>> cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>> drug_cellLine_f1ScoreWeight_geneSymbol_dict = new Dictionary<string, Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>>();
            Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>> cellLine_f1ScoreWeight_geneSymbol_dict = new Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>();
            Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>> f1ScoreWeight_geneSymbol_dict = new Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>();
            Dictionary<string, List<SVD_drug_specific_expression_line_class>> geneSymbol_dict = new Dictionary<string, List<SVD_drug_specific_expression_line_class>>();

            int expression_lines_length = expression_lines.Length;
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexE = 0; indexE < expression_lines_length; indexE++)
            {
                expression_line = expression_lines[indexE];
                if (!cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict.ContainsKey(expression_line.Cardiotoxicity_of_interest))
                {
                    cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict.Add(expression_line.Cardiotoxicity_of_interest, new Dictionary<string, Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>>());
                }
                if (!cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest].ContainsKey(expression_line.Drug))
                {
                    cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest].Add(expression_line.Drug, new Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>());
                }
                if (!cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest][expression_line.Drug].ContainsKey(expression_line.Cell_line))
                {
                    cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest][expression_line.Drug].Add(expression_line.Cell_line, new Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>());
                }
                if (!cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest][expression_line.Drug][expression_line.Cell_line].ContainsKey(expression_line.F1_score_weight))
                {
                    cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest][expression_line.Drug][expression_line.Cell_line].Add(expression_line.F1_score_weight, new Dictionary<string, List<SVD_drug_specific_expression_line_class>>());
                }
                if (!cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest][expression_line.Drug][expression_line.Cell_line][expression_line.F1_score_weight].ContainsKey(expression_line.Gene_symbol))
                {
                    cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest][expression_line.Drug][expression_line.Cell_line][expression_line.F1_score_weight].Add(expression_line.Gene_symbol, new List<SVD_drug_specific_expression_line_class>());
                }
                cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[expression_line.Cardiotoxicity_of_interest][expression_line.Drug][expression_line.Cell_line][expression_line.F1_score_weight][expression_line.Gene_symbol].Add(expression_line);
            }

            string[] cardiotoxicities;
            string cardiotoxicity;
            int cardiotoxicities_length;
            string[] drugs;
            string drug;
            int drugs_length;
            string[] cellLines;
            string cellLine;
            int cellLines_length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            float[] f1_score_weights;
            float f1_score_weight;
            int f1_score_weights_length;

            List<SVD_drug_specific_expression_line_class> ordered_expression_list = new List<SVD_drug_specific_expression_line_class>();
            cardiotoxicities = cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
            cardiotoxicities = cardiotoxicities.OrderBy(l => l).ToArray();
            cardiotoxicities_length = cardiotoxicities.Length;
            for (int indexCardioTox = 0; indexCardioTox < cardiotoxicities_length; indexCardioTox++)
            {
                cardiotoxicity = cardiotoxicities[indexCardioTox];
                drug_cellLine_f1ScoreWeight_geneSymbol_dict = cardiotoicty_drug_cellLine_f1ScoreWeight_geneSymbol_dict[cardiotoxicity];
                drugs = drug_cellLine_f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
                drugs = drugs.OrderBy(l => l).ToArray();
                drugs_length = drugs.Length;
                for (int indexD = 0; indexD < drugs_length; indexD++)
                {
                    drug = drugs[indexD];
                    cellLine_f1ScoreWeight_geneSymbol_dict = drug_cellLine_f1ScoreWeight_geneSymbol_dict[drug];
                    cellLines = cellLine_f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
                    cellLines = cellLines.OrderBy(l => l).ToArray();
                    cellLines_length = cellLines.Length;
                    for (int indexCell = 0; indexCell < cellLines_length; indexCell++)
                    {
                        cellLine = cellLines[indexCell];
                        f1ScoreWeight_geneSymbol_dict = cellLine_f1ScoreWeight_geneSymbol_dict[cellLine];
                        f1_score_weights = f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
                        f1_score_weights = f1_score_weights.OrderBy(l => l).ToArray();
                        f1_score_weights_length = f1_score_weights.Length;
                        for (int indexF1 = 0; indexF1 < f1_score_weights_length; indexF1++)
                        {
                            f1_score_weight = f1_score_weights[indexF1];
                            geneSymbol_dict = f1ScoreWeight_geneSymbol_dict[f1_score_weight];
                            geneSymbols = geneSymbol_dict.Keys.ToArray();
                            geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                            geneSymbols_length = geneSymbols.Length;
                            for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
                            {
                                geneSymbol = geneSymbols[indexGS];
                                ordered_expression_list.AddRange(geneSymbol_dict[geneSymbol]);
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_expression_list.Count;
                if (ordered_length != expression_lines_length) { throw new Exception(); }
                SVD_drug_specific_expression_line_class previous_line;
                SVD_drug_specific_expression_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_expression_list[indexO - 1];
                    this_line = ordered_expression_list[indexO];
                    if (this_line.Cardiotoxicity_of_interest.CompareTo(previous_line.Cardiotoxicity_of_interest) < 0) { throw new Exception(); }
                    else if ((this_line.Cardiotoxicity_of_interest.Equals(previous_line.Cardiotoxicity_of_interest))
                             && (this_line.Drug.CompareTo(previous_line.Drug) < 0)) { throw new Exception(); }
                    else if ((this_line.Cardiotoxicity_of_interest.Equals(previous_line.Cardiotoxicity_of_interest))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Cardiotoxicity_of_interest.Equals(previous_line.Cardiotoxicity_of_interest))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.F1_score_weight.CompareTo(previous_line.F1_score_weight) < 0)) { throw new Exception(); }
                    else if ((this_line.Cardiotoxicity_of_interest.Equals(previous_line.Cardiotoxicity_of_interest))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.F1_score_weight.Equals(previous_line.F1_score_weight))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                }
            }

            return ordered_expression_list.ToArray();
        }

        public static SVD_drug_specific_expression_line_class[] Order_by_drug_f1ScoreWeigth_geneSymbol(SVD_drug_specific_expression_line_class[] expression_lines)
        {
            Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>> drug_f1ScoreWeight_geneSymbol_dict = new Dictionary<string, Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>();
            Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>> f1ScoreWeight_geneSymbol_dict = new Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>();
            Dictionary<string, List<SVD_drug_specific_expression_line_class>> geneSymbol_dict = new Dictionary<string, List<SVD_drug_specific_expression_line_class>>();

            int expression_lines_length = expression_lines.Length;
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexE = 0; indexE < expression_lines_length; indexE++)
            {
                expression_line = expression_lines[indexE];
                if (!drug_f1ScoreWeight_geneSymbol_dict.ContainsKey(expression_line.Drug))
                {
                    drug_f1ScoreWeight_geneSymbol_dict.Add(expression_line.Drug, new Dictionary<float, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>());
                }
                if (!drug_f1ScoreWeight_geneSymbol_dict[expression_line.Drug].ContainsKey(expression_line.F1_score_weight))
                {
                    drug_f1ScoreWeight_geneSymbol_dict[expression_line.Drug].Add(expression_line.F1_score_weight, new Dictionary<string, List<SVD_drug_specific_expression_line_class>>());
                }
                if (!drug_f1ScoreWeight_geneSymbol_dict[expression_line.Drug][expression_line.F1_score_weight].ContainsKey(expression_line.Gene_symbol))
                {
                    drug_f1ScoreWeight_geneSymbol_dict[expression_line.Drug][expression_line.F1_score_weight].Add(expression_line.Gene_symbol, new List<SVD_drug_specific_expression_line_class>());
                }
                drug_f1ScoreWeight_geneSymbol_dict[expression_line.Drug][expression_line.F1_score_weight][expression_line.Gene_symbol].Add(expression_line);
            }

            string[] drugs;
            string drug;
            int drugs_length;
            //string[] cellLines;
            //string cellLine;
            //int cellLines_length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            float[] f1_score_weights;
            float f1_score_weight;
            int f1_score_weights_length;

            List<SVD_drug_specific_expression_line_class> ordered_expression_list = new List<SVD_drug_specific_expression_line_class>();


            drugs = drug_f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
            drugs = drugs.OrderBy(l => l).ToArray();
            drugs_length = drugs.Length;
            for (int indexD = 0; indexD < drugs_length; indexD++)
            {
                drug = drugs[indexD];
                f1ScoreWeight_geneSymbol_dict = drug_f1ScoreWeight_geneSymbol_dict[drug];
                f1_score_weights = f1ScoreWeight_geneSymbol_dict.Keys.ToArray();
                f1_score_weights = f1_score_weights.OrderBy(l => l).ToArray();
                f1_score_weights_length = f1_score_weights.Length;
                for (int indexF1 = 0; indexF1 < f1_score_weights_length; indexF1++)
                {
                    f1_score_weight = f1_score_weights[indexF1];
                    geneSymbol_dict = f1ScoreWeight_geneSymbol_dict[f1_score_weight];
                    geneSymbols = geneSymbol_dict.Keys.ToArray();
                    geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                    geneSymbols_length = geneSymbols.Length;
                    for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
                    {
                        geneSymbol = geneSymbols[indexGS];
                        ordered_expression_list.AddRange(geneSymbol_dict[geneSymbol]);
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_expression_list.Count;
                if (ordered_length != expression_lines_length) { throw new Exception(); }
                SVD_drug_specific_expression_line_class previous_line;
                SVD_drug_specific_expression_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_expression_list[indexO - 1];
                    this_line = ordered_expression_list[indexO];
                    if (this_line.Drug.CompareTo(previous_line.Drug) < 0) { throw new Exception(); }
                    else if ((this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.F1_score_weight.CompareTo(previous_line.F1_score_weight) < 0)) { throw new Exception(); }
                    else if ((this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.F1_score_weight.Equals(previous_line.F1_score_weight))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                }
            }

            return ordered_expression_list.ToArray();
        }

        public static SVD_drug_specific_expression_line_class[] Order_by_drug_cellLine_geneSymbol(SVD_drug_specific_expression_line_class[] expression_lines)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>> drug_cellLine_geneSymbol_dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>>();
            Dictionary<string, Dictionary<string, List<SVD_drug_specific_expression_line_class>>> cellLine_geneSymbol_dict = new Dictionary<string, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>();
            Dictionary<string, List<SVD_drug_specific_expression_line_class>> geneSymbol_dict = new Dictionary<string, List<SVD_drug_specific_expression_line_class>>();

            int expression_lines_length = expression_lines.Length;
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexE = 0; indexE < expression_lines_length; indexE++)
            {
                expression_line = expression_lines[indexE];
                if (!drug_cellLine_geneSymbol_dict.ContainsKey(expression_line.Drug))
                {
                    drug_cellLine_geneSymbol_dict.Add(expression_line.Drug, new Dictionary<string, Dictionary<string, List<SVD_drug_specific_expression_line_class>>>());
                }
                if (!drug_cellLine_geneSymbol_dict[expression_line.Drug].ContainsKey(expression_line.Cell_line))
                {
                    drug_cellLine_geneSymbol_dict[expression_line.Drug].Add(expression_line.Cell_line, new Dictionary<string, List<SVD_drug_specific_expression_line_class>>());
                }
                if (!drug_cellLine_geneSymbol_dict[expression_line.Drug][expression_line.Cell_line].ContainsKey(expression_line.Gene_symbol))
                {
                    drug_cellLine_geneSymbol_dict[expression_line.Drug][expression_line.Cell_line].Add(expression_line.Gene_symbol, new List<SVD_drug_specific_expression_line_class>());
                }
                drug_cellLine_geneSymbol_dict[expression_line.Drug][expression_line.Cell_line][expression_line.Gene_symbol].Add(expression_line);
            }

            string[] drugs;
            string drug;
            int drugs_length;
            string[] cellLines;
            string cellLine;
            int cellLines_length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;

            List<SVD_drug_specific_expression_line_class> ordered_expression_list = new List<SVD_drug_specific_expression_line_class>();


            drugs = drug_cellLine_geneSymbol_dict.Keys.ToArray();
            drugs = drugs.OrderBy(l => l).ToArray();
            drugs_length = drugs.Length;
            for (int indexD = 0; indexD < drugs_length; indexD++)
            {
                drug = drugs[indexD];
                cellLine_geneSymbol_dict = drug_cellLine_geneSymbol_dict[drug];
                cellLines = cellLine_geneSymbol_dict.Keys.ToArray();
                cellLines = cellLines.OrderBy(l => l).ToArray();
                cellLines_length = cellLines.Length;
                for (int indexCell = 0; indexCell < cellLines_length; indexCell++)
                {
                    cellLine = cellLines[indexCell];
                    geneSymbol_dict = cellLine_geneSymbol_dict[cellLine];
                    geneSymbols = geneSymbol_dict.Keys.ToArray();
                    geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                    geneSymbols_length = geneSymbols.Length;
                    for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
                    {
                        geneSymbol = geneSymbols[indexGS];
                        ordered_expression_list.AddRange(geneSymbol_dict[geneSymbol]);
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_expression_list.Count;
                if (ordered_length != expression_lines_length) { throw new Exception(); }
                SVD_drug_specific_expression_line_class previous_line;
                SVD_drug_specific_expression_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_expression_list[indexO - 1];
                    this_line = ordered_expression_list[indexO];
                    if (this_line.Drug.CompareTo(previous_line.Drug) < 0) { throw new Exception(); }
                    else if ((this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                }
            }

            return ordered_expression_list.ToArray();
        }

        public static SVD_drug_specific_expression_line_class[] Order_by_drug(SVD_drug_specific_expression_line_class[] expression_lines)
        {
            Dictionary<string, List<SVD_drug_specific_expression_line_class>> drug_dict = new Dictionary<string, List<SVD_drug_specific_expression_line_class>>();

            int expression_lines_length = expression_lines.Length;
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexE = 0; indexE < expression_lines_length; indexE++)
            {
                expression_line = expression_lines[indexE];
                if (!drug_dict.ContainsKey(expression_line.Drug))
                {
                    drug_dict.Add(expression_line.Drug, new List<SVD_drug_specific_expression_line_class>());
                }
                drug_dict[expression_line.Drug].Add(expression_line);
            }

            string[] drugs;
            string drug;
            int drugs_length;

            List<SVD_drug_specific_expression_line_class> ordered_expression_list = new List<SVD_drug_specific_expression_line_class>();
            drugs = drug_dict.Keys.ToArray();
            drugs = drugs.OrderBy(l => l).ToArray();
            drugs_length = drugs.Length;
            for (int indexD = 0; indexD < drugs_length; indexD++)
            {
                drug = drugs[indexD];
                ordered_expression_list.AddRange(drug_dict[drug]);
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_expression_list.Count;
                if (ordered_length != expression_lines_length) { throw new Exception(); }
                SVD_drug_specific_expression_line_class previous_line;
                SVD_drug_specific_expression_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_expression_list[indexO - 1];
                    this_line = ordered_expression_list[indexO];
                    if (this_line.Drug.CompareTo(previous_line.Drug) < 0) { throw new Exception(); }
                }
            }

            return ordered_expression_list.ToArray();
        }

        public static SVD_drug_specific_expression_line_class[] Order_by_geneSymbol(SVD_drug_specific_expression_line_class[] expression_lines)
        {
            Dictionary<string, List<SVD_drug_specific_expression_line_class>> geneSymbol_dict = new Dictionary<string, List<SVD_drug_specific_expression_line_class>>();

            int expression_lines_length = expression_lines.Length;
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexE = 0; indexE < expression_lines_length; indexE++)
            {
                expression_line = expression_lines[indexE];
                if (!geneSymbol_dict.ContainsKey(expression_line.Gene_symbol))
                {
                    geneSymbol_dict.Add(expression_line.Gene_symbol, new List<SVD_drug_specific_expression_line_class>());
                }
                geneSymbol_dict[expression_line.Gene_symbol].Add(expression_line);
            }

            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;

            List<SVD_drug_specific_expression_line_class> ordered_expression_list = new List<SVD_drug_specific_expression_line_class>();
            geneSymbols = geneSymbol_dict.Keys.ToArray();
            geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
            geneSymbols_length = geneSymbols.Length;
            for (int indexD = 0; indexD < geneSymbols_length; indexD++)
            {
                geneSymbol = geneSymbols[indexD];
                ordered_expression_list.AddRange(geneSymbol_dict[geneSymbol]);
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_expression_list.Count;
                if (ordered_length != expression_lines_length) { throw new Exception(); }
                SVD_drug_specific_expression_line_class previous_line;
                SVD_drug_specific_expression_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_expression_list[indexO - 1];
                    this_line = ordered_expression_list[indexO];
                    if (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0) { throw new Exception(); }
                }
            }
            return ordered_expression_list.ToArray();
        }

        public static SVD_drug_specific_expression_line_class[] Order_by_dataset_drug_cellLine_processingMethod_plate_descendingAbsValue(SVD_drug_specific_expression_line_class[] expression_lines)
        {
            //this.DEGs = this.DEGs.OrderBy(l => l.Dataset).ThenBy(l => l.Drug).ThenBy(l => l.Cell_line).ThenBy(l => l.Decomposition_method).ThenBy(l => l.Plate).ThenByDescending(l => Math.Abs(l.Value)).ToArray();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>>> dataset_drug_cellline_processingMethod_plate_descendingValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>> drug_cellline_processingMethod_plate_descendingValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>> cellline_processingMethod_plate_descendingValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>> processingMethod_plate_descendingValue_dict = new Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>();
            Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>> plate_descendingValue_dict = new Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>();
            Dictionary<float, List<SVD_drug_specific_expression_line_class>> descendingValue_dict = new Dictionary<float, List<SVD_drug_specific_expression_line_class>>();

            int expression_lines_length = expression_lines.Length;
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexE = 0; indexE < expression_lines_length; indexE++)
            {
                expression_line = expression_lines[indexE];
                if (!dataset_drug_cellline_processingMethod_plate_descendingValue_dict.ContainsKey(expression_line.Dataset))
                {
                    dataset_drug_cellline_processingMethod_plate_descendingValue_dict.Add(expression_line.Dataset, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>>());
                }
                if (!dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset].ContainsKey(expression_line.Drug))
                {
                    dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset].Add(expression_line.Drug, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>>());
                }
                if (!dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug].ContainsKey(expression_line.Cell_line))
                {
                    dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug].Add(expression_line.Cell_line, new Dictionary<string, Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>>());
                }
                if (!dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line].ContainsKey(expression_line.Processing_method))
                {
                    dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line].Add(expression_line.Processing_method, new Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>());
                }
                if (!dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line].ContainsKey(expression_line.Processing_method))
                {
                    dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line].Add(expression_line.Processing_method, new Dictionary<string, Dictionary<float, List<SVD_drug_specific_expression_line_class>>>());
                }
                if (!dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line][expression_line.Processing_method].ContainsKey(expression_line.Plate))
                {
                    dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line][expression_line.Processing_method].Add(expression_line.Plate, new Dictionary<float, List<SVD_drug_specific_expression_line_class>>());
                }
                if (!dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line][expression_line.Processing_method][expression_line.Plate].ContainsKey(expression_line.Value))
                {
                    dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line][expression_line.Processing_method][expression_line.Plate].Add(expression_line.Value, new List<SVD_drug_specific_expression_line_class>());
                }
                dataset_drug_cellline_processingMethod_plate_descendingValue_dict[expression_line.Dataset][expression_line.Drug][expression_line.Cell_line][expression_line.Processing_method][expression_line.Plate][expression_line.Value].Add(expression_line);
            }

            string[] datasets;
            string dataset;
            int datasets_length;
            string[] plates;
            string plate;
            int plates_length;
            string[] processingMethods;
            string processingMethod;
            int processingMethods_length;
            string[] drugs;
            string drug;
            int drugs_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            float[] descendingValues;
            float descendingValue;
            int descendingValues_length;

            List<SVD_drug_specific_expression_line_class> ordered_expression_list = new List<SVD_drug_specific_expression_line_class>();

            datasets = dataset_drug_cellline_processingMethod_plate_descendingValue_dict.Keys.ToArray();
            datasets_length = datasets.Length;
            datasets = datasets.OrderBy(l => l).ToArray();
            for (int indexData = 0; indexData < datasets_length; indexData++)
            {
                dataset = datasets[indexData];
                drug_cellline_processingMethod_plate_descendingValue_dict = dataset_drug_cellline_processingMethod_plate_descendingValue_dict[dataset];
                drugs = drug_cellline_processingMethod_plate_descendingValue_dict.Keys.ToArray();
                drugs = drugs.OrderBy(l => l).ToArray();
                drugs_length = drugs.Length;
                for (int indexD = 0; indexD < drugs_length; indexD++)
                {
                    drug = drugs[indexD];
                    cellline_processingMethod_plate_descendingValue_dict = drug_cellline_processingMethod_plate_descendingValue_dict[drug];
                    celllines = cellline_processingMethod_plate_descendingValue_dict.Keys.ToArray();
                    celllines = celllines.OrderBy(l => l).ToArray();
                    celllines_length = celllines.Length;
                    for (int indexCell = 0; indexCell < celllines_length; indexCell++)
                    {
                        cellline = celllines[indexCell];
                        processingMethod_plate_descendingValue_dict = cellline_processingMethod_plate_descendingValue_dict[cellline];
                        processingMethods = processingMethod_plate_descendingValue_dict.Keys.ToArray();
                        processingMethods = processingMethods.OrderBy(l => l).ToArray();
                        processingMethods_length = processingMethods.Length;
                        for (int indexDC = 0; indexDC < processingMethods_length; indexDC++)
                        {
                            processingMethod = processingMethods[indexDC];
                            plate_descendingValue_dict = processingMethod_plate_descendingValue_dict[processingMethod];
                            plates = plate_descendingValue_dict.Keys.ToArray();
                            plates = plates.OrderBy(l => l).ToArray();
                            plates_length = plates.Length;
                            for (int indexPlate = 0; indexPlate < plates_length; indexPlate++)
                            {
                                plate = plates[indexPlate];
                                descendingValue_dict = plate_descendingValue_dict[plate];
                                descendingValues = descendingValue_dict.Keys.ToArray();
                                descendingValues_length = descendingValues.Length;
                                descendingValues = descendingValues.OrderByDescending(l => Math.Abs(l)).ToArray();
                                for (int indexDes = 0; indexDes < descendingValues_length; indexDes++)
                                {
                                    descendingValue = descendingValues[indexDes];
                                    ordered_expression_list.AddRange(descendingValue_dict[descendingValue]);
                                }
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_expression_list.Count;
                if (ordered_length != expression_lines_length) { throw new Exception(); }
                SVD_drug_specific_expression_line_class previous_line;
                SVD_drug_specific_expression_line_class this_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_expression_list[indexO - 1];
                    this_line = ordered_expression_list[indexO];
                    if (this_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Drug.CompareTo(previous_line.Drug) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Processing_method.CompareTo(previous_line.Processing_method) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Processing_method.Equals(previous_line.Processing_method))
                             && (this_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Drug.Equals(previous_line.Drug))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Processing_method.Equals(previous_line.Processing_method))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (Math.Abs(this_line.Value).CompareTo(Math.Abs(previous_line.Value)) > 0)) { throw new Exception(); }
                }
            }

            return ordered_expression_list.ToArray();

        }

        public SVD_drug_specific_expression_line_class()
        {
            Cardiotoxicity_of_interest = "";
            Is_cardiotoxic = "";
            Gene_symbol = "";
            DEG_dataset = "";
            EntityClass = "";
            Drug = "";
            Experiment = "";
            Cell_line = "";
            Plate = "";
            Reference_valueType = "";
            Outlier_cell_line = "";
            Dataset = "";
            Correlation_method = "";
            Preprocess_data = "";
            Timepoint = Timepoint_enum.E_m_p_t_y;
        }
    
        public SVD_drug_specific_expression_line_class Deep_copy()
        {
            SVD_drug_specific_expression_line_class copy = (SVD_drug_specific_expression_line_class)this.MemberwiseClone();
            copy.EntityClass = (string)this.EntityClass.Clone();
            copy.Drug = (string)this.Drug.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            copy.Outlier_cell_line = (string)this.Outlier_cell_line.Clone();
            copy.DEG_dataset = (string)this.DEG_dataset.Clone();
            copy.Gene_symbol = (string)this.Gene_symbol.Clone();
            copy.Reference_valueType = (string)this.Reference_valueType.Clone();
            copy.Dataset = (string)this.Dataset.Clone();
            copy.Correlation_method = (string)this.Correlation_method.Clone();
            copy.Preprocess_data = (string)this.Preprocess_data.Clone();
            copy.Processing_method = (string)this.Processing_method.Clone();
            return copy;
        }
    }
    class SVD_drug_specific_expression_inputFromR_readOptions_class : ReadWriteOptions_base
    {
        public SVD_drug_specific_expression_inputFromR_readOptions_class(string subdirectory, string fileName, Read_svd_oneEntity_of_entityClass_eachTime_enum read_oneEntity_ofEntityClass_each_time, string save_only_drug_or_cellLine)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "EntityClass", "Drug", "Cell_line", "Drug_class", "Plate", "Gene_symbol", "Value", "Reference_valueType", "Dataset", "Correlation_method", "Preprocess_data", "Decomposition_method", "F1_score_weight", "Outlier_cell_line" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
            switch (read_oneEntity_ofEntityClass_each_time)
            {
                case Read_svd_oneEntity_of_entityClass_eachTime_enum.Drug:
                    this.SafeCondition_columnNames = new string[] { "Drug" };
                    this.SafeCondition_entries = new string[] { (string)save_only_drug_or_cellLine.Clone() };
                    break;
                case Read_svd_oneEntity_of_entityClass_eachTime_enum.Cell_line:
                    this.SafeCondition_columnNames = new string[] { "Cell_line" };
                    this.SafeCondition_entries = new string[] { (string)save_only_drug_or_cellLine.Clone() };
                    break;
                default:
                    throw new Exception();
            }
        }
    }
    class SVD_drug_specific_expression_drugCellLineOnly_line_class
    {
        public string Cell_line { get; set; }
        public string Drug { get; set; }
    }

    class SVD_cell_line_specific_expression_drugCellLIneOnly_inputFromR_readOptions_class : ReadWriteOptions_base
    {
        public SVD_cell_line_specific_expression_drugCellLIneOnly_inputFromR_readOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Cell_line", "Drug" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class SVD_drug_specific_expression_readWriteOptions_class : ReadWriteOptions_base
    {
        public SVD_drug_specific_expression_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "EntityClass", "Drug", "Drug_fullName", "Cell_line", "Outlier_cell_line", "Drug_class", "Plate", "Gene_symbol", "Fractional_rank", "Value", "Minimum_fraction_of_max_f1score", "EA_correlation_parameter", "Reference_valueType", "Dataset", "Correlation_method", "Preprocess_data", "Decomposition_method", "Icasso_permuations_count", "Processing_method", "Entry_type", "F1_score_weight", "Lv_dysfunction", "Heart_failure", "QT_interval_prolongation", "Arrhythmia", "Torsades_de_points" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class SVD_drug_specific_expression_class
    {
        public SVD_drug_specific_expression_line_class[] DEGs { get; set; }
        public string[] BgGenes { get; set; }

        public SVD_drug_specific_expression_class()
        {
            this.DEGs = new SVD_drug_specific_expression_line_class[0];
            this.BgGenes = new string[0];
        }

        public void Check_for_duplicated_drug_cellline_f1Score_geneSymbol_combination()
        {
            this.DEGs = SVD_drug_specific_expression_line_class.Order_by_drug_cellLine_f1ScoreWeigth_geneSymbol(this.DEGs);
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            SVD_drug_specific_expression_line_class previous_svd_drug_specific_line;
            for (int indexDEG = 1; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                previous_svd_drug_specific_line = this.DEGs[indexDEG - 1];
                if ((svd_drug_specific_line.Drug.Equals(previous_svd_drug_specific_line.Drug))
                    && (svd_drug_specific_line.Cell_line.Equals(previous_svd_drug_specific_line.Cell_line))
                    && (svd_drug_specific_line.F1_score_weight.Equals(previous_svd_drug_specific_line.F1_score_weight))
                    && (svd_drug_specific_line.Gene_symbol.Equals(previous_svd_drug_specific_line.Gene_symbol)))
                {
                    throw new Exception();
                }
            }
        }

        public void Check_for_duplicated_drug_cellline_geneSymbol_combination()
        {
            this.DEGs = SVD_drug_specific_expression_line_class.Order_by_drug_cellLine_geneSymbol(this.DEGs);
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            SVD_drug_specific_expression_line_class previous_svd_drug_specific_line;
            for (int indexDEG = 1; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                previous_svd_drug_specific_line = this.DEGs[indexDEG - 1];
                if ((svd_drug_specific_line.Drug.Equals(previous_svd_drug_specific_line.Drug))
                    && (svd_drug_specific_line.Cell_line.Equals(previous_svd_drug_specific_line.Cell_line))
                    && (svd_drug_specific_line.Gene_symbol.Equals(previous_svd_drug_specific_line.Gene_symbol)))
                {
                    throw new Exception();
                }
            }
        }

        private void Add_to_array(SVD_drug_specific_expression_line_class[] add_DEGs)
        {
            int this_count = this.DEGs.Length;
            int add_count = add_DEGs.Length;
            int new_count = this_count + add_count;
            SVD_drug_specific_expression_line_class[] new_DEGs = new SVD_drug_specific_expression_line_class[new_count];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_count; indexThis++)
            {
                indexNew++;
                new_DEGs[indexNew] = this.DEGs[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_count; indexAdd++)
            {
                indexNew++;
                new_DEGs[indexNew] = add_DEGs[indexAdd];
            }
            this.DEGs = new_DEGs;
        }

        public void Replace_drugClasses_as_selected(Dictionary<Drug_type_enum, Drug_type_enum> oldDrugClass_newDrugClass_dict)
        {
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                if (oldDrugClass_newDrugClass_dict.ContainsKey(svd_drug_specific_line.Drug_class))
                {
                    svd_drug_specific_line.Drug_class = oldDrugClass_newDrugClass_dict[svd_drug_specific_line.Drug_class];
                }
            }
        }

        private SVD_drug_specific_expression_line_class[] Calculate_fractional_ranks_based_on_absolute_values(SVD_drug_specific_expression_line_class[] degs)
        {
            int degs_length = degs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            SVD_drug_specific_expression_line_class inner_svd_drug_specific_line;
            degs = SVD_drug_specific_expression_line_class.Order_by_dataset_drug_cellLine_processingMethod_plate_descendingAbsValue(degs);
            int firstIndex_sameGroupValue = -1;
            int current_rank = -1;
            float final_rank;
            List<float> current_ranks = new List<float>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                svd_drug_specific_line = degs[indexDeg];
                if ((indexDeg == 0)
                   || (!svd_drug_specific_line.Dataset.Equals(degs[indexDeg - 1].Dataset))
                   || (!svd_drug_specific_line.Drug.Equals(degs[indexDeg - 1].Drug))
                   || (!svd_drug_specific_line.Cell_line.Equals(degs[indexDeg - 1].Cell_line))
                   || (!svd_drug_specific_line.Processing_method.Equals(degs[indexDeg - 1].Processing_method))
                   || (!svd_drug_specific_line.Plate.Equals(degs[indexDeg - 1].Plate)))
                {
                    current_rank = 0;
                }
                if ((indexDeg == 0)
                   || (!svd_drug_specific_line.Dataset.Equals(degs[indexDeg - 1].Dataset))
                   || (!svd_drug_specific_line.Drug.Equals(degs[indexDeg - 1].Drug))
                   || (!svd_drug_specific_line.Cell_line.Equals(degs[indexDeg - 1].Cell_line))
                   || (!svd_drug_specific_line.Processing_method.Equals(degs[indexDeg - 1].Processing_method))
                   || (!svd_drug_specific_line.Plate.Equals(degs[indexDeg - 1].Plate))
                   || (!Math.Abs(svd_drug_specific_line.Value).Equals(Math.Abs(degs[indexDeg - 1].Value))))
                {
                    firstIndex_sameGroupValue = indexDeg;
                    current_ranks.Clear();
                }
                current_rank++;
                current_ranks.Add(current_rank);
                if ((indexDeg == degs_length-1)
                   || (!svd_drug_specific_line.Dataset.Equals(degs[indexDeg + 1].Dataset))
                   || (!svd_drug_specific_line.Drug.Equals(degs[indexDeg + 1].Drug))
                   || (!svd_drug_specific_line.Cell_line.Equals(degs[indexDeg + 1].Cell_line))
                   || (!svd_drug_specific_line.Processing_method.Equals(degs[indexDeg + 1].Processing_method))
                   || (!svd_drug_specific_line.Plate.Equals(degs[indexDeg + 1].Plate))
                   || (!Math.Abs(svd_drug_specific_line.Value).Equals(Math.Abs(degs[indexDeg + 1].Value))))
                {
                    if (current_ranks.Count==1)
                    {
                        svd_drug_specific_line.Fractional_rank = current_ranks[0];
                    }
                    else
                    {
                        final_rank = Math_class.Get_average(current_ranks.ToArray());
                        for (int indexInner = firstIndex_sameGroupValue;indexInner<=indexDeg;indexInner++)
                        {
                            inner_svd_drug_specific_line = degs[indexInner];
                            inner_svd_drug_specific_line.Fractional_rank = final_rank;
                        }
                    }
                }
            }
            return degs;
        }

        public void Calculate_fractional_ranks_based_on_absolute_values()
        {
            this.DEGs = Calculate_fractional_ranks_based_on_absolute_values(this.DEGs);
        }

        private SVD_drug_specific_expression_line_class[] Keep_only_lines_with_fractionalRanks_equalOrSmaller_than_selected_rank(SVD_drug_specific_expression_line_class[] degs, float max_fractional_rank)
        {
            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = degs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = degs[indexDEG];
                if (svd_drug_specific_line.Fractional_rank == -1) { throw new Exception(); }
                else if (svd_drug_specific_line.Fractional_rank <= max_fractional_rank)
                {
                    keep.Add(svd_drug_specific_line);
                }
            }
            return keep.ToArray();
        }

        public void Keep_only_lines_with_fractionalRanks_equalOrSmaller_than_selected_rank(float max_fractional_rank)
        {
            this.DEGs = Keep_only_lines_with_fractionalRanks_equalOrSmaller_than_selected_rank(this.DEGs,max_fractional_rank);
        }
        public void Keep_only_lines_with_genes_that_have_at_least_one_fractionalRanks_equalOrSmaller_than_selected_rank_for_the_same_drug_and_entryType(float max_fractional_rank)
        {
            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            Dictionary<string, Dictionary<DE_entry_enum, Dictionary<string, bool>>> drug_geneSymbol_keep_dict = new Dictionary<string, Dictionary<DE_entry_enum, Dictionary<string, bool>>>();
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                if (svd_drug_specific_line.Fractional_rank <= max_fractional_rank)
                {
                    if (!drug_geneSymbol_keep_dict.ContainsKey(svd_drug_specific_line.Drug))
                    {
                        drug_geneSymbol_keep_dict.Add(svd_drug_specific_line.Drug, new Dictionary<DE_entry_enum, Dictionary<string, bool>>());
                    }
                    if (!drug_geneSymbol_keep_dict[svd_drug_specific_line.Drug].ContainsKey(svd_drug_specific_line.Entry_type))
                    {
                        drug_geneSymbol_keep_dict[svd_drug_specific_line.Drug].Add(svd_drug_specific_line.Entry_type, new Dictionary<string, bool>());
                    }
                    if (!drug_geneSymbol_keep_dict[svd_drug_specific_line.Drug][svd_drug_specific_line.Entry_type].ContainsKey(svd_drug_specific_line.Gene_symbol))
                    {
                        drug_geneSymbol_keep_dict[svd_drug_specific_line.Drug][svd_drug_specific_line.Entry_type].Add(svd_drug_specific_line.Gene_symbol, true);
                    }
                }
            }
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                if (  (drug_geneSymbol_keep_dict.ContainsKey(svd_drug_specific_line.Drug))
                    && (drug_geneSymbol_keep_dict[svd_drug_specific_line.Drug].ContainsKey(svd_drug_specific_line.Entry_type))
                    && (drug_geneSymbol_keep_dict[svd_drug_specific_line.Drug][svd_drug_specific_line.Entry_type].ContainsKey(svd_drug_specific_line.Gene_symbol)))
                {
                    keep.Add(svd_drug_specific_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Keep_only_lines_with_selected_drug(string drug)
        {
            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                if (svd_drug_specific_line.Drug.Equals(drug))
                {
                    keep.Add(svd_drug_specific_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Keep_only_lines_with_selected_processingMethod(string processing_method)
        {
            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                if (svd_drug_specific_line.Processing_method.Equals(processing_method))
                {
                    keep.Add(svd_drug_specific_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Keep_only_lines_with_selected_geneSymbols(string[] geneSymbols)
        {
            Dictionary<string, bool> geneSymbol_dict = new Dictionary<string, bool>();
            foreach (string geneSymbol in geneSymbols)
            {
                geneSymbol_dict.Add(geneSymbol, true);
            }

            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                if (geneSymbol_dict.ContainsKey(svd_drug_specific_line.Gene_symbol))
                {
                    keep.Add(svd_drug_specific_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public SVD_drug_specific_expression_class Generate_svd_drug_specific_expression_instance_containing_only_indicated_geneSymbols(string[] geneSymbols)
        {
            Dictionary<string, bool> geneSymbol_dict = new Dictionary<string, bool>();
            foreach (string geneSymbol in geneSymbols)
            {
                geneSymbol_dict.Add(geneSymbol, true);
            }

            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                if (geneSymbol_dict.ContainsKey(svd_drug_specific_line.Gene_symbol))
                {
                    keep.Add(svd_drug_specific_line.Deep_copy());
                }
            }
            SVD_drug_specific_expression_class geneSymbol_svd = new SVD_drug_specific_expression_class();
            geneSymbol_svd.Add_to_array(keep.ToArray());
            return geneSymbol_svd;
        }

        public void Keep_degsdeps_with_minimum_absolute_value(float min_abs_value)
        {
            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_specific_line;
            for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
            {
                svd_drug_specific_line = this.DEGs[indexDEG];
                if ((float)Math.Abs(svd_drug_specific_line.Value) >= min_abs_value)
                {
                    keep.Add(svd_drug_specific_line);
                }
            }
            this.DEGs = keep.ToArray();

        }

        private void Set_entrytype()
        {
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            //string[] splitStrings;
            //string degs_score_of_interest_string;
            StringBuilder sb = new StringBuilder();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                deg_line.Entry_type = DE_entry_enum.Diffrna;
            }
        }

        public Deg_class Generate_new_deg_instance_assuming_values_are_signed_minusLog10Pvalues_and_plate0(string dataset)
        {
            int data_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_expression_line;
            Deg_line_class deg_line;
            List<Deg_line_class> degs = new List<Deg_line_class>();
            for (int indexDeg=0; indexDeg<data_length; indexDeg++)
            {
                svd_drug_expression_line = this.DEGs[indexDeg];
                deg_line = new Deg_line_class();
                deg_line.Dataset = (string)dataset.Clone();
                deg_line.Patient = (string)svd_drug_expression_line.Cell_line.Clone();
                deg_line.Drug_type = svd_drug_expression_line.Drug_class;
                deg_line.Symbol = (string)svd_drug_expression_line.Gene_symbol.Clone();
                deg_line.Gene = "";
                deg_line.Treatment = (string)svd_drug_expression_line.Drug.Clone();
                deg_line.Treatment_full_name = (string)svd_drug_expression_line.Drug_fullName.Clone();
                deg_line.Signed_minus_log10_pvalue = svd_drug_expression_line.Value;
                deg_line.EntryType = DE_entry_enum.Diffrna; 
                deg_line.Fractional_rank = svd_drug_expression_line.Fractional_rank;
                deg_line.Plate = svd_drug_expression_line.Plate;
                deg_line.Is_cardiotoxic = (string)svd_drug_expression_line.Is_cardiotoxic.Clone();
                degs.Add(deg_line);
            }
            Deg_class deg_instance = new Deg_class();
            deg_instance.Generate_from_input_array(degs.ToArray());
            return deg_instance;
        }

        public SVD_drug_specific_expression_class Generate_new_svd_instance_containing_only_geneExpVector_differences_from_regular_to_outlier()
        {
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            SVD_drug_specific_expression_line_class inner_deg_line;
            SVD_drug_specific_expression_line_class outlier_deg_line;
            SVD_drug_specific_expression_line_class difference_deg_line;
            List<SVD_drug_specific_expression_line_class> difference_vectors = new List<SVD_drug_specific_expression_line_class>();
            int firstIndexSameGroup = -1;
            int outlierCellLineIndex = -1;
            this.DEGs = SVD_drug_specific_expression_line_class.Order_by_drug_f1ScoreWeigth_geneSymbol(this.DEGs);
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (  (indexDeg==0)
                    || (!deg_line.Drug.Equals(this.DEGs[indexDeg - 1].Drug))
                    || (!deg_line.F1_score_weight.Equals(this.DEGs[indexDeg - 1].F1_score_weight))
                    || (!deg_line.Gene_symbol.Equals(this.DEGs[indexDeg - 1].Gene_symbol)))
                {
                    firstIndexSameGroup = indexDeg;
                    outlierCellLineIndex = -1;
                }
                if (deg_line.Outlier_cell_line.Equals("O"))
                {
                    if (outlierCellLineIndex!=-1) { throw new Exception(); }
                    outlierCellLineIndex = indexDeg;
                }
                if ((indexDeg == degs_length-1)
                    || (!deg_line.Drug.Equals(this.DEGs[indexDeg + 1].Drug))
                    || (!deg_line.F1_score_weight.Equals(this.DEGs[indexDeg + 1].F1_score_weight))
                    || (!deg_line.Gene_symbol.Equals(this.DEGs[indexDeg + 1].Gene_symbol)))
                {
                    if (outlierCellLineIndex!=-1)
                    {
                        outlier_deg_line = this.DEGs[outlierCellLineIndex];
                        for (int indexInner = firstIndexSameGroup; indexInner <= indexDeg; indexInner++)
                        {
                            if (indexInner != outlierCellLineIndex)
                            {
                                inner_deg_line = this.DEGs[indexInner];
                                difference_deg_line = inner_deg_line.Deep_copy();
                                difference_deg_line.Value = outlier_deg_line.Value - inner_deg_line.Value;
                                difference_deg_line.Outlier_cell_line = "Diff";
                                difference_deg_line.Cell_line = outlier_deg_line.Cell_line + "_minus_" + inner_deg_line.Cell_line;
                                difference_deg_line.Fractional_rank = -1;
                                difference_vectors.Add(difference_deg_line);
                            }
                        }
                    }
                }
            }
            SVD_drug_specific_expression_class vector_expression_svd_degs = new SVD_drug_specific_expression_class();
            vector_expression_svd_degs.DEGs = difference_vectors.ToArray();
            return vector_expression_svd_degs;
        }

        public void Keep_only_genes_that_are_part_of_all_deg_datasets()
        {
            Dictionary<string, Dictionary<string, bool>> symbol_degDataset_dict = new Dictionary<string, Dictionary<string, bool>>();
            Dictionary<string, bool> deg_dataset_dict = new Dictionary<string, bool>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (!symbol_degDataset_dict.ContainsKey(deg_line.Gene_symbol))
                {
                    symbol_degDataset_dict.Add(deg_line.Gene_symbol, new Dictionary<string, bool>());
                }
                if (!symbol_degDataset_dict[deg_line.Gene_symbol].ContainsKey(deg_line.DEG_dataset))
                {
                    symbol_degDataset_dict[deg_line.Gene_symbol].Add(deg_line.DEG_dataset, true);
                }
                if (!deg_dataset_dict.ContainsKey(deg_line.DEG_dataset))
                {
                    deg_dataset_dict.Add(deg_line.DEG_dataset, true);
                }
            }

            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int deg_dataset_length = deg_dataset_dict.Keys.ToArray().Length;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (symbol_degDataset_dict[deg_line.Gene_symbol].ToArray().Length == deg_dataset_length)
                {
                    keep.Add(deg_line); 
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Keep_only_lines_that_are_in_outlier_file_and_label_outlier(Outlier_class outlier)
        {
            Dictionary<string, Dictionary<float, string>> drug_f1scoreWeigth_outlierCellline = outlier.Generate_drug_f1scoreWeigth_outlierCellline_dict();
            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if ((drug_f1scoreWeigth_outlierCellline.ContainsKey(deg_line.Drug))
                    && (drug_f1scoreWeigth_outlierCellline[deg_line.Drug].ContainsKey(deg_line.F1_score_weight)))
                {
                    if (deg_line.Cell_line.Equals(drug_f1scoreWeigth_outlierCellline[deg_line.Drug][deg_line.F1_score_weight]))
                    {
                        deg_line.Outlier_cell_line = "O";
                    }
                    else
                    {
                        deg_line.Outlier_cell_line = "N";
                    }
                    keep.Add(deg_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Label_outlier(Outlier_class outlier)
        {
            Dictionary<string, Dictionary<float, string>> drug_f1scoreWeigth_outlierCellline = outlier.Generate_drug_f1scoreWeigth_outlierCellline_dict();
            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (deg_line.F1_score_weight == 10) { deg_line.Outlier_cell_line = "F"; }
                else { deg_line.Outlier_cell_line = "N"; }
                if ((drug_f1scoreWeigth_outlierCellline.ContainsKey(deg_line.Drug))
                    && (drug_f1scoreWeigth_outlierCellline[deg_line.Drug].ContainsKey(deg_line.F1_score_weight)))
                {
                    if (deg_line.Cell_line.Equals(drug_f1scoreWeigth_outlierCellline[deg_line.Drug][deg_line.F1_score_weight]))
                    {
                        deg_line.Outlier_cell_line = "O";
                    }
                }
            }
        }

        public void Label_all_outlierCellLine_as_fullData()
        {
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                deg_line.Outlier_cell_line = "F";
            }
        }

        public void Label_all_outlierCellLine_as_non()
        {
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                deg_line.Outlier_cell_line = "N";
            }
        }

        public void Keep_only_drugs_that_are_part_of_all_deg_datasets()
        {
            Dictionary<string, Dictionary<string, bool>> drug_degDataset_dict = new Dictionary<string, Dictionary<string, bool>>();
            Dictionary<string, bool> deg_dataset_dict = new Dictionary<string, bool>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (!drug_degDataset_dict.ContainsKey(deg_line.Drug))
                {
                    drug_degDataset_dict.Add(deg_line.Drug, new Dictionary<string, bool>());
                }
                if (!drug_degDataset_dict[deg_line.Drug].ContainsKey(deg_line.DEG_dataset))
                {
                    drug_degDataset_dict[deg_line.Drug].Add(deg_line.DEG_dataset, true);
                }
                if (!deg_dataset_dict.ContainsKey(deg_line.DEG_dataset))
                {
                    deg_dataset_dict.Add(deg_line.DEG_dataset, true);
                }
            }

            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int deg_dataset_length = deg_dataset_dict.Keys.ToArray().Length;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (drug_degDataset_dict[deg_line.Drug].ToArray().Length == deg_dataset_length)
                {
                    keep.Add(deg_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Keep_only_selected_outlier_statusses(string[] outlier_statuses)
        {
            outlier_statuses = outlier_statuses.Distinct().OrderBy(l => l).ToArray();
            Dictionary<string, bool> outlier_statuses_dict = new Dictionary<string, bool>();
            foreach (string outlier in outlier_statuses)
            {
                outlier_statuses_dict.Add(outlier, true);
            }

            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int deg_dataset_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_dataset_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (outlier_statuses_dict.ContainsKey(deg_line.Outlier_cell_line))
                {
                    keep.Add(deg_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Keep_only_selected_drugs(string[] selected_drugs)
        {
            selected_drugs = selected_drugs.Distinct().OrderBy(l => l).ToArray();
            Dictionary<string, bool> selected_drugs_dict = new Dictionary<string, bool>();
            foreach (string drug in selected_drugs)
            {
                selected_drugs_dict.Add(drug, true);
            }

            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int deg_dataset_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_dataset_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (selected_drugs_dict.ContainsKey(deg_line.Drug))
                {
                    keep.Add(deg_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Keep_only_selected_drugTypes(params Drug_type_enum[] selected_drugTypes)
        {
            selected_drugTypes = selected_drugTypes.Distinct().OrderBy(l => l).ToArray();
            Dictionary<Drug_type_enum, bool> selected_drugTypes_dict = new Dictionary<Drug_type_enum, bool>();
            foreach (Drug_type_enum drugType in selected_drugTypes)
            {
                selected_drugTypes_dict.Add(drugType, true);
            }

            List<SVD_drug_specific_expression_line_class> keep = new List<SVD_drug_specific_expression_line_class>();
            int deg_dataset_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_dataset_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (selected_drugTypes_dict.ContainsKey(deg_line.Drug_class))
                {
                    keep.Add(deg_line);
                }
            }
            this.DEGs = keep.ToArray();
        }

        public void Set_all_f1score_weights_to_input(float f1_score_weight)
        {
            int deg_dataset_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_dataset_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                deg_line.F1_score_weight = f1_score_weight;
            }
        }

        public void Adjust_expression_of_each_gene_in_set0_with_regard_to_set1(string to_be_adjusted_set0, string reference_set1)
        {
            Dictionary<string, Dictionary<string, Dictionary<string,float>>> current_degDataset_experiment_cellLIne_value_dict = new Dictionary<string, Dictionary<string, Dictionary<string, float>>>();

            int degs_length = this.DEGs.Length;
            this.DEGs = this.DEGs.OrderBy(l => l.Gene_symbol).ThenBy(l=>l.Drug).ThenBy(l=>l.DEG_dataset).ThenBy(l=>l.Experiment).ToArray();
            SVD_drug_specific_expression_line_class deg_line;
            SVD_drug_specific_expression_line_class inner_deg_line;
            int indexFirstSameDrugSymbol = -1;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if ((indexDeg == 0)
                    || (!deg_line.Gene_symbol.Equals(this.DEGs[indexDeg - 1].Gene_symbol))
                    || (!deg_line.Drug.Equals(this.DEGs[indexDeg - 1].Drug)))
                {
                    current_degDataset_experiment_cellLIne_value_dict.Clear();
                    indexFirstSameDrugSymbol = indexDeg;
                }
                if (!current_degDataset_experiment_cellLIne_value_dict.ContainsKey(deg_line.DEG_dataset))
                {
                    current_degDataset_experiment_cellLIne_value_dict.Add(deg_line.DEG_dataset, new Dictionary<string, Dictionary<string, float>>());
                }
                if (!current_degDataset_experiment_cellLIne_value_dict[deg_line.DEG_dataset].ContainsKey(deg_line.Experiment))
                {
                    current_degDataset_experiment_cellLIne_value_dict[deg_line.DEG_dataset].Add(deg_line.Experiment, new Dictionary<string, float>());
                }
                current_degDataset_experiment_cellLIne_value_dict[deg_line.DEG_dataset][deg_line.Experiment].Add(deg_line.Cell_line, deg_line.Value);
                if ((indexDeg == degs_length - 1)
                    || (!deg_line.Gene_symbol.Equals(this.DEGs[indexDeg + 1].Gene_symbol))
                    || (!deg_line.Drug.Equals(this.DEGs[indexDeg + 1].Drug)))
                {
                    Dictionary<string, float> reference_cellline_value_dict = current_degDataset_experiment_cellLIne_value_dict[reference_set1]["same"];
                    Dictionary<string, Dictionary<string, float>> experiment_cellline_value_dict = current_degDataset_experiment_cellLIne_value_dict[to_be_adjusted_set0];
                    string[] experiments = experiment_cellline_value_dict.Keys.ToArray();
                    string experiment;
                    int experiments_length = experiments.Length;
                    for (int indexE = 0; indexE < experiments_length; indexE++)
                    {
                        experiment = experiments[indexE];
                        Dictionary<string, float> cellline_value_dict = experiment_cellline_value_dict[experiment];
                        string[] celllines = cellline_value_dict.Keys.ToArray();
                        string cellline;
                        int celllines_length = celllines.Length;
                        List<float> value_differences = new List<float>();
                        for (int indexC = 0; indexC < celllines_length; indexC++)
                        {
                            cellline = celllines[indexC];
                            value_differences.Add(cellline_value_dict[cellline] - reference_cellline_value_dict[cellline.Replace("_" + experiment, "")]);
                        }
                        float mean_diff = Math_class.Get_average(value_differences.ToArray());
                        for (int indexInner = indexFirstSameDrugSymbol; indexInner <= indexDeg; indexInner++)
                        {
                            inner_deg_line = this.DEGs[indexInner];
                            if ((inner_deg_line.DEG_dataset.Equals(to_be_adjusted_set0))
                                && (inner_deg_line.Experiment.Equals(experiment)))
                            {
                                inner_deg_line.Value -= mean_diff;
                            }
                        }
                    }
                }
            }
        }

        public void Add_drug_fullNames()
        {
            Deg_drug_legend_class legend = new Deg_drug_legend_class();
            legend.Generate_de_novo();
            Dictionary<string, string> drug_drugFullName_dict = legend.Get_drug_drugFullName_dict();
            int deg_dataset_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            string drug;
            for (int indexDeg = 0; indexDeg < deg_dataset_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                drug = (string)deg_line.Drug.Clone();
                Text2_class.Set_first_letter_to_uppercase(ref drug);
                if (drug_drugFullName_dict.ContainsKey(drug))
                {
                    deg_line.Drug_fullName = (string)drug_drugFullName_dict[deg_line.Drug];
                }
                else
                {
                    deg_line.Drug_fullName = "No full name found";
                }
            }
        }

        public void Check_if_equals_other(SVD_drug_specific_expression_class other)
        {
            int this_length = this.DEGs.Length;
            int other_length = other.DEGs.Length;
            if (this_length!=other_length) { throw new Exception(); }
            this.DEGs = SVD_drug_specific_expression_line_class.Order_by_drug_cellLine_geneSymbol(this.DEGs);
            other.DEGs = SVD_drug_specific_expression_line_class.Order_by_drug_cellLine_geneSymbol(other.DEGs);
            SVD_drug_specific_expression_line_class this_line;
            SVD_drug_specific_expression_line_class other_line;
            for (int indexThis=0; indexThis<this_length;indexThis++)
            {
                this_line = this.DEGs[indexThis];
                other_line = other.DEGs[indexThis];
                if (  (indexThis!=0)
                    && (!this_line.Drug.Equals(this.DEGs[indexThis - 1].Drug))
                    && (!this_line.Cell_line.Equals(this.DEGs[indexThis - 1].Cell_line))
                    && (!this_line.Gene_symbol.Equals(this.DEGs[indexThis - 1].Gene_symbol)))
                {
                    throw new Exception();
                }
                if ((indexThis != 0)
                    && (!other_line.Drug.Equals(other.DEGs[indexThis - 1].Drug))
                    && (!other_line.Cell_line.Equals(other.DEGs[indexThis - 1].Cell_line))
                    && (!other_line.Gene_symbol.Equals(other.DEGs[indexThis - 1].Gene_symbol)))
                {
                    throw new Exception();
                }
                if (!this_line.Drug.Equals(other_line.Drug)) { throw new Exception(); }
                if (!this_line.Cell_line.Equals(other_line.Cell_line)) { throw new Exception(); }
                if (!this_line.Gene_symbol.Equals(other_line.Gene_symbol)) { throw new Exception(); }
                if (!this_line.Value.Equals(other_line.Value)) { throw new Exception(); }
            }
        }

        public void Add_drug_types()
        {
            Deg_drug_legend_class legend = new Deg_drug_legend_class();
            legend.Generate_de_novo();
            Dictionary<string, Drug_type_enum> drug_drugType_dict = legend.Get_drug_drugType_dictionary();
            int deg_dataset_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < deg_dataset_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                if (drug_drugType_dict.ContainsKey(deg_line.Drug))
                {
                    deg_line.Drug_class = drug_drugType_dict[deg_line.Drug];
                }
            }
        }

        public void Generate_by_readingROutput_calculateFractionalRanks_keepOnlyTopDEGs_and_set_bgGenes(string[] datasets, string processing_method, string[] fileNames, string decomposition_method, Deg_score_of_interest_enum[] deg_score_of_interests, Read_svd_oneEntity_of_entityClass_eachTime_enum read_oneEntity_of_entityClass_eachTime, int topDEGs, params string[] onlyReadEntities)
        {
            Read_r_outputs_and_set_bgGenes(datasets, processing_method, fileNames, deg_score_of_interests, decomposition_method, topDEGs, read_oneEntity_of_entityClass_eachTime, onlyReadEntities);
            Add_drug_fullNames();
            Set_entrytype();
        }

        public string[] Get_all_drugs()
        {
            List<string> drugs = new List<string>();
            foreach (SVD_drug_specific_expression_line_class svd_line in this.DEGs)
            {
                drugs.Add(svd_line.Drug);
            }
            return drugs.Distinct().OrderBy(l => l).ToArray();
        }

        public string[] Get_all_celllines()
        {
            List<string> celllines = new List<string>();
            foreach (SVD_drug_specific_expression_line_class svd_line in this.DEGs)
            {
                celllines.Add(svd_line.Cell_line);
            }
            return celllines.Distinct().OrderBy(l => l).ToArray();
        }

        public int Get_number_of_samples_defined_by_cellline_plus_drug_of_selected_drugTypes(params Drug_type_enum[] selected_drugClasses)
        {
            Dictionary<string, bool> sample_dict = new Dictionary<string, bool>();
            string sample;
            foreach (SVD_drug_specific_expression_line_class svd_line in this.DEGs)
            {
                sample = svd_line.Drug + svd_line.Cell_line;
                if ((selected_drugClasses.Contains(svd_line.Drug_class))&&(!sample_dict.ContainsKey(sample)))
                {
                    sample_dict.Add(sample, true);
                }
            }
            return sample_dict.Keys.ToArray().Length;
        }

        public string[] Get_all_processing_methods()
        {
            Dictionary<string, bool> processingMethod_dict = new Dictionary<string, bool>();
            foreach (SVD_drug_specific_expression_line_class svd_line in this.DEGs)
            {
                if (!processingMethod_dict.ContainsKey(svd_line.Processing_method))
                {
                    processingMethod_dict.Add(svd_line.Processing_method, true);
                }
            }
            return processingMethod_dict.Keys.Distinct().OrderBy(l => l).ToArray();
        }

        public string[] Get_deep_copy_of_bgGenes()
        {
            int bgGenes_length = this.BgGenes.Length;
            string[] bgGenes = new string[bgGenes_length];
            for (int indexBG=0; indexBG<bgGenes_length; indexBG++)
            {
                bgGenes[indexBG] = this.BgGenes[indexBG].ToString();
            }
            return bgGenes;
        }

        public DE_class Generate_de_instance()
        {
            SVD_drug_specific_expression_line_class first_deg_line = this.DEGs[0];
            string reference_value_type = (string)first_deg_line.Reference_valueType.Clone();

            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                //if (!deg_line.Minimum_fraction_of_max_f1score.Equals(first_deg_line.Minimum_fraction_of_max_f1score)) { throw new Exception(); }
                //if (!deg_line.Preprocess_data.Equals(first_deg_line.Preprocess_data)) { throw new Exception(); }
                //if (!deg_line.Icasso_permuations_count.Equals(first_deg_line.Icasso_permuations_count)) { throw new Exception(); }
                //if (!deg_line.Deg_score_of_interest.Equals(first_deg_line.Deg_score_of_interest)) { throw new Exception(); }
                //if (!deg_line.Decomposition_method.Equals(first_deg_line.Decomposition_method)) { throw new Exception(); }
                //if (!deg_line.Correlation_method.Equals(first_deg_line.Correlation_method)) { throw new Exception(); }

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { (string)deg_line.Drug.Clone(), deg_line.Cell_line.Replace("Cell_line.", ""), "F1SW" + deg_line.F1_score_weight, "CMfr" + deg_line.Cardiotoxixity_frequencyGroup, deg_line.Outlier_cell_line, deg_line.Drug_class.ToString(), "Is cardiotoxic: " + deg_line.Is_cardiotoxic };
                fill_de_line.Symbols_for_de = new string[] { (string)deg_line.Gene_symbol.Clone() };
                fill_de_line.Entry_type_for_de = deg_line.Entry_type;
                fill_de_line.Timepoint_for_de = deg_line.Timepoint;
                fill_de_line.Value_for_de = deg_line.Value;
                fill_de_list.Add(fill_de_line);
            }
            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list.ToArray());
            return de;
        }

        private void Read_r_outputs_and_set_bgGenes(string[] datasets, string processing_method, string[] fileNames, Deg_score_of_interest_enum[] deg_score_of_interests, string decomposition_method, int topDEGs, Read_svd_oneEntity_of_entityClass_eachTime_enum read_oneEntity_of_entityClass_eachTime, params string[] onlyReadEntities)
        {
            List<SVD_drug_specific_expression_line_class> degs_list = new List<SVD_drug_specific_expression_line_class>();
            Dictionary<string, bool> bgGenes_dict = new Dictionary<string, bool>();
            int subdirectories_length = datasets.Length;
            string subdirectory;
            string fileName;
            Deg_score_of_interest_enum deg_score_of_interest;
            for (int indexS = 0; indexS < subdirectories_length; indexS++)
            {
                deg_score_of_interest = deg_score_of_interests[indexS];
                subdirectory = decomposition_method + "_" + datasets[indexS] + "_" + deg_score_of_interest + "/9_Drug_specific_expression_values/";
                fileName = fileNames[indexS];
                string[] drugs_or_cellLines;
                if (onlyReadEntities.Length == 0)
                {
                    SVD_cell_line_specific_expression_drugCellLIneOnly_inputFromR_readOptions_class drugOnly_readOptions = new SVD_cell_line_specific_expression_drugCellLIneOnly_inputFromR_readOptions_class(subdirectory, fileName); ;
                    SVD_drug_specific_expression_drugCellLineOnly_line_class[] celllineOnly_lines = ReadWriteClass.ReadRawData_and_FillArray<SVD_drug_specific_expression_drugCellLineOnly_line_class>(drugOnly_readOptions);
                    Dictionary<string, bool> drugOrCellline_dict = new Dictionary<string, bool>();
                    foreach (SVD_drug_specific_expression_drugCellLineOnly_line_class celllineOnly_line in celllineOnly_lines)
                    {
                        switch (read_oneEntity_of_entityClass_eachTime)
                        {
                            case Read_svd_oneEntity_of_entityClass_eachTime_enum.Cell_line:
                                if (!drugOrCellline_dict.ContainsKey(celllineOnly_line.Cell_line))
                                {
                                    drugOrCellline_dict.Add(celllineOnly_line.Cell_line, true);
                                }
                                break;
                            case Read_svd_oneEntity_of_entityClass_eachTime_enum.Drug:
                                if (!drugOrCellline_dict.ContainsKey(celllineOnly_line.Drug))
                                {
                                    drugOrCellline_dict.Add(celllineOnly_line.Drug, true);
                                }
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    drugs_or_cellLines = drugOrCellline_dict.Keys.ToArray();
                }
                else
                {
                    drugs_or_cellLines = onlyReadEntities;
                }
                foreach (string drug_or_cellLine in drugs_or_cellLines)
                {
                    SVD_drug_specific_expression_inputFromR_readOptions_class readOptions = new SVD_drug_specific_expression_inputFromR_readOptions_class(subdirectory, fileName, read_oneEntity_of_entityClass_eachTime, drug_or_cellLine);
                    SVD_drug_specific_expression_line_class[] drugOrCellline_specific_expression_lines = ReadWriteClass.ReadRawData_and_FillArray<SVD_drug_specific_expression_line_class>(readOptions);
                    int drugOrCellline_specific_length = drugOrCellline_specific_expression_lines.Length;
                    SVD_drug_specific_expression_line_class expression_line;
                    for (int indexSVD = 0; indexSVD < drugOrCellline_specific_length; indexSVD++)
                    {
                        expression_line = drugOrCellline_specific_expression_lines[indexSVD];
                        expression_line.Processing_method = (string)processing_method.Clone();
                        if (!bgGenes_dict.ContainsKey(expression_line.Gene_symbol))
                        {
                            bgGenes_dict.Add(expression_line.Gene_symbol, true);
                        }
                    }
                    drugOrCellline_specific_expression_lines = Calculate_fractional_ranks_based_on_absolute_values(drugOrCellline_specific_expression_lines);
                    drugOrCellline_specific_expression_lines = Keep_only_lines_with_fractionalRanks_equalOrSmaller_than_selected_rank(drugOrCellline_specific_expression_lines, topDEGs);
                    degs_list.AddRange(drugOrCellline_specific_expression_lines);
                }
            }
            this.DEGs = degs_list.ToArray();
            this.BgGenes = bgGenes_dict.Keys.OrderBy(l => l).ToArray();
        }

        public void Add_and_overrride_entryType(DE_entry_enum entryType)
        {
            foreach (SVD_drug_specific_expression_line_class expression_line in DEGs)
            {
                expression_line.Entry_type = entryType;
            }
        }
        public void Add_cardiotoxicity()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            drug_legend.Add_missing_cardiotoxicity_from_faers();
            drug_legend.Legend = drug_legend.Legend.OrderBy(l => l.Drug).ToArray();
            int legend_length = drug_legend.Legend.Length;
            Deg_drug_legend_line_class legend_line = new Deg_drug_legend_line_class();
            int indexL = 0;
            int stringCompare = -2;
            int degs_length = this.DEGs.Length;
            this.DEGs = SVD_drug_specific_expression_line_class.Order_by_drug(this.DEGs);
            SVD_drug_specific_expression_line_class expression_line;
            for (int indexDEG=0; indexDEG<degs_length;indexDEG++)
            {
                expression_line = this.DEGs[indexDEG];
                stringCompare = -2;
                while ((indexDEG<degs_length)&&(stringCompare<0))
                {
                    legend_line = drug_legend.Legend[indexL];
                    stringCompare = legend_line.Drug.CompareTo(expression_line.Drug);
                    if (stringCompare<0)
                    {
                        indexL++;
                    }
                }
                if (stringCompare!=0) { throw new Exception(); }
                expression_line.Cardiotoxixity_frequencyGroup = legend_line.Cardiotoxicity_frequencyGroup;
                expression_line.Is_cardiotoxic = (string)legend_line.Is_cardiotoxic.Clone();
            }
        }

        public void Add_lines_with_zero_value_for_missing_geneSymbol_cellLine_drug_processingMethod_combination(string[] geneSymbols)
        {
            string geneSymbol;
            int indexGS = 0;
            List<string> missingSymbols = new List<string>();
            int stringCompare = -2;
            int geneSymbols_length = geneSymbols.Length;
            geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
            this.DEGs = this.DEGs.OrderBy(l=>l.Dataset).ThenBy(l => l.Cell_line).ThenBy(l => l.Drug).ThenBy(l => l.Processing_method).ThenBy(l=>l.Gene_symbol).ToArray();
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class svd_drug_line;
            SVD_drug_specific_expression_line_class new_svd_drug_line;
            List<SVD_drug_specific_expression_line_class> add_svd_lines = new List<SVD_drug_specific_expression_line_class>();
            for (int indexDEGs=0; indexDEGs<degs_length;indexDEGs++)
            {
                svd_drug_line = this.DEGs[indexDEGs];
                if ((indexDEGs==0)
                    || (!svd_drug_line.Dataset.Equals(this.DEGs[indexDEGs - 1].Dataset))
                    || (!svd_drug_line.Cell_line.Equals(this.DEGs[indexDEGs - 1].Cell_line))
                    || (!svd_drug_line.Preprocess_data.Equals(this.DEGs[indexDEGs - 1].Preprocess_data))
                    || (!svd_drug_line.Drug.Equals(this.DEGs[indexDEGs - 1].Drug)))
                {
                    indexGS = 0;
                    missingSymbols.Clear();
                }
                stringCompare = -2;
                while ((indexGS < geneSymbols_length)&&(stringCompare<0))
                {
                    geneSymbol = geneSymbols[indexGS];
                    stringCompare = geneSymbol.CompareTo(svd_drug_line.Gene_symbol);
                    if (stringCompare<0)
                    {
                        missingSymbols.Add(geneSymbol);
                        indexGS++;
                    }
                    else if (stringCompare==0) { indexGS++; }
                }
                if (stringCompare>0) { throw new Exception(); }
                if ((indexDEGs == degs_length-1)
                    || (!svd_drug_line.Dataset.Equals(this.DEGs[indexDEGs + 1].Dataset))
                    || (!svd_drug_line.Cell_line.Equals(this.DEGs[indexDEGs + 1].Cell_line))
                    || (!svd_drug_line.Preprocess_data.Equals(this.DEGs[indexDEGs + 1].Preprocess_data))
                    || (!svd_drug_line.Drug.Equals(this.DEGs[indexDEGs + 1].Drug)))
                {
                    while (indexGS < geneSymbols_length)
                    {
                        geneSymbol = geneSymbols[indexGS];
                        missingSymbols.Add(geneSymbol);
                        indexGS++;
                    }
                    foreach (string missing_geneSymbol in missingSymbols)
                    {
                        new_svd_drug_line = new SVD_drug_specific_expression_line_class();
                        new_svd_drug_line.Value = 0;
                        new_svd_drug_line.Gene_symbol = (string)missing_geneSymbol.Clone();
                        new_svd_drug_line.Cell_line = (string)svd_drug_line.Cell_line.Clone();
                        new_svd_drug_line.Dataset = (string)svd_drug_line.Dataset.Clone();
                        new_svd_drug_line.Drug = (string)svd_drug_line.Drug.Clone();
                        new_svd_drug_line.Drug_fullName = (string)svd_drug_line.Drug_fullName.Clone();
                        new_svd_drug_line.Entry_type = svd_drug_line.Entry_type;
                        add_svd_lines.Add(new_svd_drug_line);
                    }
                }
            }
            this.Add_to_array(add_svd_lines.ToArray());
        }

        public void Add_string_inFrontOf_all_geneSymbols(string add_inFront)
        {
            int degs_length = this.DEGs.Length;
            SVD_drug_specific_expression_line_class deg_line;
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = this.DEGs[indexDeg];
                deg_line.Gene_symbol = add_inFront + " - " + deg_line.Gene_symbol;
            }
        }

        public void Add_other(SVD_drug_specific_expression_class other)
        {
            this.Add_to_array(other.DEGs);
        }

        public void Write(string subdirectory, string fileName)
        {
            SVD_drug_specific_expression_readWriteOptions_class writeOptions = new SVD_drug_specific_expression_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.DEGs, writeOptions);
        }

        public void Write_for_paper(string subdirectory, string fileName)
        {
            SVD_drug_specific_expression_readWriteOptions_class writeOptions = new SVD_drug_specific_expression_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.DEGs, writeOptions);
        }

        public void Read(string subdirectory, string fileName)
        {
            SVD_drug_specific_expression_readWriteOptions_class readOptions = new SVD_drug_specific_expression_readWriteOptions_class(subdirectory, fileName);
            this.DEGs = ReadWriteClass.ReadRawData_and_FillArray<SVD_drug_specific_expression_line_class>(readOptions);
        }

        public SVD_drug_specific_expression_class Deep_copy()
        {
            SVD_drug_specific_expression_class copy = (SVD_drug_specific_expression_class)this.MemberwiseClone();
            int this_length = this.DEGs.Length;
            copy.DEGs = new SVD_drug_specific_expression_line_class[this_length];
            for (int indexThis=0; indexThis<this_length;indexThis++)
            {
                copy.DEGs[indexThis] = this.DEGs[indexThis].Deep_copy();
            }
            return copy;
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class SVD_collapsed_drugType_expression_summary_line_class
    {
        public string Drug_class_string { get; set; }
        public int Samples_with_drug_class { get; set; }
        public int Samples_with_drug_class_expressing_gene { get; set; }
        public float Percent_of_samples_with_drug_class_expressing_gene { get; set; }
        public string Gene_symbol { get; set; }

        public SVD_collapsed_drugType_expression_summary_line_class Deep_copy()
        {
            SVD_collapsed_drugType_expression_summary_line_class copy = (SVD_collapsed_drugType_expression_summary_line_class)this.MemberwiseClone();
            copy.Gene_symbol = (string)this.Gene_symbol.Clone();
            copy.Drug_class_string = (string)this.Drug_class_string.Clone();
            return copy;
        }
    }

    class SVD_collapsed_drugType_expression_summary_readWriteOptions : ReadWriteOptions_base
    {
        public SVD_collapsed_drugType_expression_summary_readWriteOptions(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Drug_class_string","Gene_symbol","Samples_with_drug_class", "Samples_with_drug_class_expressing_gene", "Percent_of_samples_with_drug_class_expressing_gene" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class SVD_collapsed_drugType_expression_summary_class
    {
        public SVD_collapsed_drugType_expression_summary_line_class[] Collapsed { get; set; }

        public SVD_collapsed_drugType_expression_summary_class()
        {
            Collapsed = new SVD_collapsed_drugType_expression_summary_line_class[0];
        }

        public void Add_to_array(SVD_collapsed_drugType_expression_summary_line_class[] add_collapsed)
        {
            int add_length = add_collapsed.Length;
            int this_length = this.Collapsed.Length;
            int new_length = add_length + this_length;
            SVD_collapsed_drugType_expression_summary_line_class[] new_collapsed = new SVD_collapsed_drugType_expression_summary_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_collapsed[indexNew] = this.Collapsed[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_collapsed[indexNew] = add_collapsed[indexAdd];
            }
            this.Collapsed = new_collapsed;
        }

        public void Generate_from_svd_drug_specific_expression_instance_separating_between_cardiotoxic_and_non_cardiotoxic_and_add_to_array(SVD_drug_specific_expression_class svd_expression)
        {
            int cardiotoxic_samples_length = -1;// svd_expression.Get_number_of_samples_defined_by_cellline_plus_drug_of_selected_drugTypes(Drug_type_enum.Cardiotoxic_kinase_inhibitor, Drug_type_enum.Cardiotoxic_monoclonal_antibody);
            int non_cardiotoxic_samples_length = -1;// svd_expression.Get_number_of_samples_defined_by_cellline_plus_drug_of_selected_drugTypes(Drug_type_enum.Noncardiotoxic_kinase_inhibitor, Drug_type_enum.Noncardiotoxic_monoclonal_antibody);
            svd_expression.DEGs = SVD_drug_specific_expression_line_class.Order_by_geneSymbol(svd_expression.DEGs);
            int degs_length = svd_expression.DEGs.Length;
            int currentGene_upregulated_cardiotoxic_samples_count = -1;
            int currentGene_upregulated_noncardiotoxic_samples_count = -1;
            int currentGene_downregulated_cardiotoxic_samples_count = -1;
            int currentGene_downregulated_noncardiotoxic_samples_count = -1;
            SVD_drug_specific_expression_line_class svd_drug_expression_line;
            SVD_collapsed_drugType_expression_summary_line_class new_collapsed_line;
            List<SVD_collapsed_drugType_expression_summary_line_class> new_collapsed_list = new List<SVD_collapsed_drugType_expression_summary_line_class>();
            for (int indexDEG=0; indexDEG<degs_length; indexDEG++)
            {
                svd_drug_expression_line = svd_expression.DEGs[indexDEG];
                if (  (indexDEG==0)
                    ||(!svd_drug_expression_line.Gene_symbol.Equals(svd_expression.DEGs[indexDEG-1].Gene_symbol)))
                {
                    currentGene_upregulated_cardiotoxic_samples_count = 0;
                    currentGene_upregulated_noncardiotoxic_samples_count = 0;
                    currentGene_downregulated_cardiotoxic_samples_count = 0;
                    currentGene_downregulated_noncardiotoxic_samples_count = 0;
                }
                switch (svd_drug_expression_line.Drug_class)
                {
                    case Drug_type_enum.Kinase_inhibitor:
                    case Drug_type_enum.Monoclonal_antibody:
                        if (svd_drug_expression_line.Value < 0)
                        {
                            if (svd_drug_expression_line.Is_cardiotoxic.Equals("Yes"))
                            { currentGene_downregulated_cardiotoxic_samples_count++; }
                            else
                            { currentGene_downregulated_noncardiotoxic_samples_count++; }
                        }
                        else if (svd_drug_expression_line.Value > 0)
                        {
                            if (svd_drug_expression_line.Is_cardiotoxic.Equals("Yes"))
                            { currentGene_upregulated_cardiotoxic_samples_count++; }
                            else
                            { currentGene_upregulated_noncardiotoxic_samples_count++; }
                        }
                        break;
                }
                if ((indexDEG == degs_length-1)
                    || (!svd_drug_expression_line.Gene_symbol.Equals(svd_expression.DEGs[indexDEG + 1].Gene_symbol)))
                {
                    if (currentGene_upregulated_cardiotoxic_samples_count > 0)
                    {
                        new_collapsed_line = new SVD_collapsed_drugType_expression_summary_line_class();
                        new_collapsed_line.Drug_class_string = "Cardiotoxic - Up";
                        new_collapsed_line.Gene_symbol = (string)svd_drug_expression_line.Gene_symbol.Clone();
                        new_collapsed_line.Percent_of_samples_with_drug_class_expressing_gene = 100 * currentGene_upregulated_cardiotoxic_samples_count / cardiotoxic_samples_length;
                        new_collapsed_line.Samples_with_drug_class_expressing_gene = currentGene_upregulated_cardiotoxic_samples_count;
                        new_collapsed_line.Samples_with_drug_class = cardiotoxic_samples_length;
                        new_collapsed_list.Add(new_collapsed_line);
                    }
                    if (currentGene_downregulated_cardiotoxic_samples_count > 0)
                    {
                        new_collapsed_line = new SVD_collapsed_drugType_expression_summary_line_class();
                        new_collapsed_line.Drug_class_string = "Cardiotoxic - Down";
                        new_collapsed_line.Gene_symbol = (string)svd_drug_expression_line.Gene_symbol.Clone();
                        new_collapsed_line.Percent_of_samples_with_drug_class_expressing_gene = 100 * currentGene_downregulated_cardiotoxic_samples_count / cardiotoxic_samples_length;
                        new_collapsed_line.Samples_with_drug_class_expressing_gene = currentGene_downregulated_cardiotoxic_samples_count;
                        new_collapsed_line.Samples_with_drug_class = cardiotoxic_samples_length;
                        new_collapsed_list.Add(new_collapsed_line);
                    }
                    if (currentGene_upregulated_noncardiotoxic_samples_count > 0)
                    {
                        new_collapsed_line = new SVD_collapsed_drugType_expression_summary_line_class();
                        new_collapsed_line.Drug_class_string = "Noncardiotoxic - Up";
                        new_collapsed_line.Gene_symbol = (string)svd_drug_expression_line.Gene_symbol.Clone();
                        new_collapsed_line.Percent_of_samples_with_drug_class_expressing_gene = 100 * currentGene_upregulated_noncardiotoxic_samples_count / non_cardiotoxic_samples_length;
                        new_collapsed_line.Samples_with_drug_class_expressing_gene = currentGene_upregulated_noncardiotoxic_samples_count;
                        new_collapsed_line.Samples_with_drug_class = non_cardiotoxic_samples_length;
                        new_collapsed_list.Add(new_collapsed_line);
                    }
                    if (currentGene_downregulated_noncardiotoxic_samples_count > 0)
                    {
                        new_collapsed_line = new SVD_collapsed_drugType_expression_summary_line_class();
                        new_collapsed_line.Drug_class_string = "Noncardiotoxic - Down";
                        new_collapsed_line.Gene_symbol = (string)svd_drug_expression_line.Gene_symbol.Clone();
                        new_collapsed_line.Percent_of_samples_with_drug_class_expressing_gene = 100 * currentGene_downregulated_noncardiotoxic_samples_count / non_cardiotoxic_samples_length;
                        new_collapsed_line.Samples_with_drug_class_expressing_gene = currentGene_downregulated_noncardiotoxic_samples_count;
                        new_collapsed_line.Samples_with_drug_class = non_cardiotoxic_samples_length;
                        new_collapsed_list.Add(new_collapsed_line);
                    }
                }
            }
            Add_to_array(new_collapsed_list.ToArray());
        }

        public void Generate_from_inputGene_list_and_add_to_array(string[] geneSymbols, string drug_class_string)
        {
            geneSymbols = geneSymbols.Distinct().OrderBy(l => l).ToArray();
            SVD_collapsed_drugType_expression_summary_line_class new_collapsed_line;
            List<SVD_collapsed_drugType_expression_summary_line_class> new_collapsed_list = new List<SVD_collapsed_drugType_expression_summary_line_class>();
            foreach (string geneSymbol in geneSymbols)
            {
                new_collapsed_line = new SVD_collapsed_drugType_expression_summary_line_class();
                new_collapsed_line.Drug_class_string = (string)drug_class_string.Clone();
                new_collapsed_line.Gene_symbol = (string)geneSymbol.Clone();
                new_collapsed_line.Percent_of_samples_with_drug_class_expressing_gene = 100;
                new_collapsed_line.Samples_with_drug_class = 1;
                new_collapsed_line.Samples_with_drug_class_expressing_gene = 1;
                new_collapsed_list.Add(new_collapsed_line);
            }
            Add_to_array(new_collapsed_list.ToArray());
        }

        public void Keep_only_inputGeneSymbols(string[] keep_symbols)
        {
            keep_symbols = keep_symbols.Distinct().OrderBy(l => l).ToArray();
            Dictionary<string, bool> keep_symbols_dict = new Dictionary<string, bool>();
            foreach (string keep_symbol in keep_symbols)
            {
                keep_symbols_dict.Add(keep_symbol, true);
            }
            List<SVD_collapsed_drugType_expression_summary_line_class> keep = new List<SVD_collapsed_drugType_expression_summary_line_class>();
            foreach (SVD_collapsed_drugType_expression_summary_line_class collapsed_line in this.Collapsed)
            {
                if (keep_symbols_dict.ContainsKey(collapsed_line.Gene_symbol))
                {
                    keep.Add(collapsed_line);
                }
            }
            this.Collapsed = keep.ToArray();
        }

        public SVD_collapsed_drugType_expression_summary_class Deep_copy()
        {
            SVD_collapsed_drugType_expression_summary_class copy = (SVD_collapsed_drugType_expression_summary_class)this.MemberwiseClone();
            int collapsed_length = this.Collapsed.Length;
            copy.Collapsed = new SVD_collapsed_drugType_expression_summary_line_class[collapsed_length];
            for (int indexC=0; indexC<collapsed_length; indexC++)
            {
                copy.Collapsed[indexC] = this.Collapsed[indexC].Deep_copy();
            }
            return copy;
        }

        public void Write(string subdirectory, string fileName)
        {
            SVD_collapsed_drugType_expression_summary_readWriteOptions readWriteOptions = new SVD_collapsed_drugType_expression_summary_readWriteOptions(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Collapsed, readWriteOptions);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class SVD_drug_specific_expression_summary_line_class
    {
        public string Drug { get; set; }
        public string Cell_line { get; set; }
        public string Experiment { get; set; }
        public string Plate { get; set; }
        public float Pvalue_cutoff { get; set; }
        public int Number_of_significant_genes { get; set; }

    }

    class SVD_drug_specific_expression_summary_readWriteOptions_class : ReadWriteOptions_base
    {
        public SVD_drug_specific_expression_summary_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Experiment", "Plate", "Drug", "Cell_line", "Pvalue_cutoff", "Number_of_significant_genes" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class SVD_drug_specific_expression_summary_class
    {
        public SVD_drug_specific_expression_summary_line_class[] SVD_degs_summaries { get; set; }

        public void Generate_from_svd_drug_specific_expression_instance(SVD_drug_specific_expression_class svd_degs, float[] minus_10toPower_cutoffs)
        {
            svd_degs.DEGs = SVD_drug_specific_expression_line_class.Order_by_experiment_plate_drug_cellline_descending_value(svd_degs.DEGs);
            SVD_drug_specific_expression_summary_line_class new_summary_line;
            List<SVD_drug_specific_expression_summary_line_class> summaries = new List<SVD_drug_specific_expression_summary_line_class>();
            int degs_lenght = svd_degs.DEGs.Length;
            SVD_drug_specific_expression_line_class expression_line;


            int cutoffs_length = minus_10toPower_cutoffs.Length;
            int[] genes_above_cutoff = new int[0];
            float[] cutoffs = new float[cutoffs_length];
            for (int indexC=0; indexC<cutoffs_length;indexC++)
            {
                cutoffs[indexC] = -(float)Math.Log10(minus_10toPower_cutoffs[indexC]);
            }
            for (int indexDEG=0; indexDEG<degs_lenght;indexDEG++)
            {
                expression_line = svd_degs.DEGs[indexDEG];
                if (  (indexDEG==0)
                    || (!expression_line.Experiment.Equals(svd_degs.DEGs[indexDEG - 1].Experiment))
                    || (!expression_line.Plate.Equals(svd_degs.DEGs[indexDEG - 1].Plate))
                    || (!expression_line.Drug.Equals(svd_degs.DEGs[indexDEG - 1].Drug))
                    || (!expression_line.Cell_line.Equals(svd_degs.DEGs[indexDEG - 1].Cell_line)))
                {
                    genes_above_cutoff = new int[cutoffs_length];
                }
                for (int indexC=0; indexC<cutoffs_length;indexC++)
                {
                    if (expression_line.Value>=cutoffs[indexC])
                    {
                        genes_above_cutoff[indexC]++;
                    }
                }
                if ((indexDEG == degs_lenght-1)
                    || (!expression_line.Experiment.Equals(svd_degs.DEGs[indexDEG + 1].Experiment))
                    || (!expression_line.Plate.Equals(svd_degs.DEGs[indexDEG + 1].Plate))
                    || (!expression_line.Drug.Equals(svd_degs.DEGs[indexDEG + 1].Drug))
                    || (!expression_line.Cell_line.Equals(svd_degs.DEGs[indexDEG + 1].Cell_line)))
                {
                    for (int indexC=0; indexC<cutoffs_length;indexC++)
                    {
                        new_summary_line = new SVD_drug_specific_expression_summary_line_class();
                        new_summary_line.Cell_line = (string)expression_line.Cell_line.Clone();
                        new_summary_line.Drug = (string)expression_line.Drug.Clone();
                        new_summary_line.Pvalue_cutoff = minus_10toPower_cutoffs[indexC];
                        new_summary_line.Plate = (string)expression_line.Plate.Clone();
                        new_summary_line.Experiment = (string)expression_line.Experiment.Clone();
                        new_summary_line.Number_of_significant_genes = genes_above_cutoff[indexC];
                        summaries.Add(new_summary_line);
                    }
                }
            }

            this.SVD_degs_summaries = summaries.ToArray();
        }

        public void Write(string subdirectory, string fileName)
        {
            SVD_drug_specific_expression_summary_readWriteOptions_class readWriteOptions = new SVD_drug_specific_expression_summary_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.SVD_degs_summaries, readWriteOptions);
        }


    }

}
