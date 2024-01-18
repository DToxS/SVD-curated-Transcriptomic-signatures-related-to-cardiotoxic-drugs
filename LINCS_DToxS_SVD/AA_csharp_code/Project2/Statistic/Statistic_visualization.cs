using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common_classes;
using ReadWrite;
using Highthroughput_data;

namespace Statistic
{
    class PiePiece_line_class
    {
        public string SampleName { get; set; }
        public string Piepiece_name { get; set; }
        public int Absolute_value { get; set; }
        public float Relative_value { get; set; }
        public string[] Gene_symbols { get; set; }
        
        public string ReadWrite_gene_symbols
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Gene_symbols, Pie_chart_readWriteOptions_class.Array_delimiter); }
            set { this.Gene_symbols = ReadWriteClass.Get_array_from_readLine<string>(value, Pie_chart_readWriteOptions_class.Array_delimiter); }
        }

        public PiePiece_line_class Deep_copy()
        {
            PiePiece_line_class copy = (PiePiece_line_class)this.MemberwiseClone();
            copy.Piepiece_name = (string)this.Piepiece_name.Clone();
            copy.SampleName = (string)this.SampleName.Clone();
            return copy;
        }
    }

    class Pie_chart_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ','; } }

        public Pie_chart_readWriteOptions_class(string fileName)
        {
            this.File = Global_directory_class.Results_directory + fileName;
            this.Key_propertyNames = new string[] { "SampleName", "Piepiece_name", "Absolute_value", "Relative_value","ReadWrite_gene_symbols" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Pie_chart_class
    {
        public PiePiece_line_class[] Pie_chart_lines { get; set; }

        public Pie_chart_class()
        {
            this.Pie_chart_lines = new PiePiece_line_class[0];
        }

        private void Add_to_array(PiePiece_line_class[] add_pie_chart_lines)
        {
            int this_length = this.Pie_chart_lines.Length;
            int add_length = add_pie_chart_lines.Length;
            int new_length = this_length + add_length;
            PiePiece_line_class[] new_pie_chart_lines = new PiePiece_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_pie_chart_lines[indexNew] = this.Pie_chart_lines[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_pie_chart_lines[indexNew] = add_pie_chart_lines[indexAdd];
            }
            this.Pie_chart_lines = new_pie_chart_lines;
        }

        private PiePiece_line_class Generate_piePiece_line(string[] geneSymbols_of_current_piePiece, string current_piePiece_name, string current_sampleName)
        {
            geneSymbols_of_current_piePiece = geneSymbols_of_current_piePiece.Distinct().ToArray();
            PiePiece_line_class new_pie_chart_line = new PiePiece_line_class();
            new_pie_chart_line.Gene_symbols = Array_class.Deep_copy_string_array(geneSymbols_of_current_piePiece);
            new_pie_chart_line.SampleName = (string)current_sampleName.Clone();
            new_pie_chart_line.Piepiece_name = (string)current_piePiece_name.Clone();
            new_pie_chart_line.Absolute_value = new_pie_chart_line.Gene_symbols.Length;
            return new_pie_chart_line;
        }

        private void Generate_piePiece_lines_and_add_to_array(Dictionary<string, List<string>> piePieceName_geneSymbols_dict, string current_sampleName)
        {
            string[] piePieceNames = piePieceName_geneSymbols_dict.Keys.ToArray();
            string piePieceName;
            int piePieceNames_length = piePieceNames.Length;
            PiePiece_line_class new_piePiece_line;
            List<PiePiece_line_class> complete_pie_list = new List<PiePiece_line_class>();
            int total_symbols_count = 0;
            for (int indexPiePiece = 0; indexPiePiece < piePieceNames_length; indexPiePiece++)
            {
                piePieceName = piePieceNames[indexPiePiece];
                new_piePiece_line = Generate_piePiece_line(piePieceName_geneSymbols_dict[piePieceName].ToArray(), piePieceName, current_sampleName);
                total_symbols_count += new_piePiece_line.Absolute_value;
                complete_pie_list.Add(new_piePiece_line);
            }

            foreach (PiePiece_line_class piePiece_line in complete_pie_list)
            {
                piePiece_line.Relative_value = (float)piePiece_line.Absolute_value / (float)total_symbols_count;
            }
            Add_to_array(complete_pie_list.ToArray());
        }

        private Dictionary<string, string[]> Generate_geneSymbols_piePieceName_dict(Dictionary<string, string[]> piePieceName_geneSymbols_dict)
        {
            string[] piePieceNames = piePieceName_geneSymbols_dict.Keys.ToArray();
            string piePieceName;
            int piePieceNames_length = piePieceNames.Length;

            string[] current_geneSymbols;
            string current_geneSymbol;
            int current_geneSymbols_length;

            Dictionary<string, string[]> geneSymbols_piePieceName_dict = new Dictionary<string, string[]>();
            for (int indexPiePiece = 0; indexPiePiece < piePieceNames_length; indexPiePiece++)
            {
                piePieceName = piePieceNames[indexPiePiece];
                current_geneSymbols = piePieceName_geneSymbols_dict[piePieceName];
                current_geneSymbols_length = current_geneSymbols.Length;
                for (int indexC = 0; indexC < current_geneSymbols_length; indexC++)
                {
                    current_geneSymbol = current_geneSymbols[indexC];
                    if (!geneSymbols_piePieceName_dict.ContainsKey(current_geneSymbol))
                    {
                        geneSymbols_piePieceName_dict.Add(current_geneSymbol, new string[] { piePieceName });
                    }
                    else
                    {
                        geneSymbols_piePieceName_dict[current_geneSymbol] = Overlap_class.Get_union(geneSymbols_piePieceName_dict[current_geneSymbol], piePieceName);
                    }
                }
            }
            return geneSymbols_piePieceName_dict;
        }

        public void Generate_one_pie_for_each_column(DE_class de, Dictionary<string, string[]> piePieceName_geneSymbols_dict)
        {
            Dictionary<string, string[]> geneSymbols_pieChartNames_dict = Generate_geneSymbols_piePieceName_dict(piePieceName_geneSymbols_dict);
            Dictionary<string, List<string>> pieChartName_current_symbols_dict = new Dictionary<string, List<string>>();
            int col_count = de.ColChar.Columns.Count;
            int de_length = de.DE.Count;
            DE_line_class de_line;
            string current_sampleName;

            string[] pieChartNames_of_current_symbol;
            string pieChartName_of_current_symbol;
            int pieChartNames_of_current_symbol_length;
            StringBuilder sb = new StringBuilder();
            string combined_pieChartName;

            List<PiePiece_line_class> piePiece_list = new List<PiePiece_line_class>();
            for (int indexCol = 0; indexCol < col_count; indexCol++)
            {
                current_sampleName = de.ColChar.Get_complete_column_label(indexCol);
                pieChartName_current_symbols_dict.Clear();
                for (int indexDe = 0; indexDe < de_length; indexDe++)
                {
                    de_line = de.DE[indexDe];
                    if (de_line.Columns[indexCol].Value != 0)
                    {
                        if (geneSymbols_pieChartNames_dict.ContainsKey(de_line.Gene_symbol))
                        {
                            pieChartNames_of_current_symbol = geneSymbols_pieChartNames_dict[de_line.Gene_symbol].OrderBy(l => l).ToArray();
                            pieChartNames_of_current_symbol_length = pieChartNames_of_current_symbol.Length;
                            sb.Clear();
                            pieChartName_of_current_symbol = pieChartNames_of_current_symbol[0];
                            sb.AppendFormat(pieChartName_of_current_symbol);
                            for (int indexPieChart = 1; indexPieChart < pieChartNames_of_current_symbol_length; indexPieChart++)
                            {
                                pieChartName_of_current_symbol = pieChartNames_of_current_symbol[indexPieChart];
                                sb.AppendFormat("-{0}", (string)pieChartName_of_current_symbol.Clone());
                            }
                            combined_pieChartName = sb.ToString();
                            if (!pieChartName_current_symbols_dict.ContainsKey(combined_pieChartName))
                            {
                                pieChartName_current_symbols_dict.Add(combined_pieChartName, new List<string>());
                            }
                            pieChartName_current_symbols_dict[combined_pieChartName].Add(de_line.Gene_symbol);
                        }
                        else
                        {
                           // if (!pieChartName_current_symbols_dict.ContainsKey("Other")) { pieChartName_current_symbols_dict.Add("Other", new List<string>()); }
                           // pieChartName_current_symbols_dict["Other"].Add(de_line.Gene_symbol);
                        }
                    }
                }
                Generate_piePiece_lines_and_add_to_array(pieChartName_current_symbols_dict, current_sampleName);
            }
        }

        public void Write_pie(string fileName)
        {
            Pie_chart_readWriteOptions_class readWriteOptions = new Pie_chart_readWriteOptions_class(fileName);
            ReadWriteClass.WriteData(this.Pie_chart_lines, readWriteOptions);
        }
    }

}
