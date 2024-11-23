using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common_classes;
using ReadWrite;
using Gene_databases;

namespace Highthroughput_data
{
    enum EdgeR_replicate_value_type_enum {  E_m_p_t_y, Raw, Normalized }

    class DEG_replicate_values_line_class
    {
        public string Dataset { get; set; }
        public string Plate { get; set; }
        public float Expression_value { get; set; }
        public string Gene { get; set; }
        public string Symbol { get; set; }
        public string Replicate_name { get; set; }
        public int Replicate_no { get; set; }
        public string Cell_line { get; set; }
        public string Treatment { get; set; }
        public string Treatment_full_name { get; set; }
        public Timepoint_enum Timepoint { get; set; }
        public DE_entry_enum EntryType { get; set; }
        public Drug_type_enum Drug_type { get; set; }
        public EdgeR_replicate_value_type_enum Replicate_value_type { get; set; }

        public static DEG_replicate_values_line_class[] Order_by_gene(DEG_replicate_values_line_class[] deg_replicates)
        {
            deg_replicates = deg_replicates.OrderBy(l => l.Gene).ToArray();
            return deg_replicates;
        }

        public bool Equal_group(DEG_replicate_values_line_class other)
        {
            bool equal = ((this.Cell_line.Equals(other.Cell_line))
                          && (this.Treatment.Equals(other.Treatment))
                          && (this.EntryType.Equals(other.EntryType))
                          && (this.Timepoint.Equals(other.Timepoint))
                          && (this.Dataset.Equals(other.Timepoint))
                          && (this.Replicate_no.Equals(other.Replicate_no))
                          && (this.Plate.Equals(other.Plate)));
            return equal;
        }

        public static DEG_replicate_values_line_class[] Order_by_dataset_plate_cellline_treatment_replicateName_gene(DEG_replicate_values_line_class[] deg_replicates)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>>>> dataset_plate_cellline_treatment_replicateName_gene_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>>> plate_cellline_treatment_replicateName_gene_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>> cellline_treatment_replicateName_gene_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>> treatment_replicateName_gene_dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>();
            Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>> replicateName_gene_dict = new Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>();
            Dictionary<string, List<DEG_replicate_values_line_class>> gene_dict = new Dictionary<string, List<DEG_replicate_values_line_class>>();
            int deg_replicates_length = deg_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            for (int indexRep = 0; indexRep < deg_replicates_length; indexRep++)
            {
                deg_replicate_line = deg_replicates[indexRep];
                if (!dataset_plate_cellline_treatment_replicateName_gene_dict.ContainsKey(deg_replicate_line.Dataset))
                {
                    dataset_plate_cellline_treatment_replicateName_gene_dict.Add(deg_replicate_line.Dataset, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset].ContainsKey(deg_replicate_line.Plate))
                {
                    dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset].Add(deg_replicate_line.Plate, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].ContainsKey(deg_replicate_line.Cell_line))
                {
                    dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].Add(deg_replicate_line.Cell_line, new Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line].ContainsKey(deg_replicate_line.Treatment))
                {
                    dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line].Add(deg_replicate_line.Treatment, new Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].ContainsKey(deg_replicate_line.Replicate_name))
                {
                    dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].Add(deg_replicate_line.Replicate_name, new Dictionary<string, List<DEG_replicate_values_line_class>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_name].ContainsKey(deg_replicate_line.Gene))
                {
                    dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_name].Add(deg_replicate_line.Gene, new List<DEG_replicate_values_line_class>());
                }
                dataset_plate_cellline_treatment_replicateName_gene_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_name][deg_replicate_line.Gene].Add(deg_replicate_line);
            }

            string[] datasets = dataset_plate_cellline_treatment_replicateName_gene_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] plates;
            string plate;
            int plates_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            string[] replicateNames;
            string replicateName;
            int replicateNames_length;
            string[] genes;
            string gene;
            int genes_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<DEG_replicate_values_line_class> ordered_replicates = new List<DEG_replicate_values_line_class>();
            for (int indexData = 0; indexData < datasets_length; indexData++)
            {
                dataset = datasets[indexData];
                plate_cellline_treatment_replicateName_gene_dict = dataset_plate_cellline_treatment_replicateName_gene_dict[dataset];
                plates = plate_cellline_treatment_replicateName_gene_dict.Keys.ToArray();
                plates = plates.OrderBy(l => l).ToArray();
                plates_length = plates.Length;
                for (int indexPlate = 0; indexPlate < plates_length; indexPlate++)
                {
                    plate = plates[indexPlate];
                    cellline_treatment_replicateName_gene_dict = plate_cellline_treatment_replicateName_gene_dict[plate];
                    celllines = cellline_treatment_replicateName_gene_dict.Keys.ToArray();
                    celllines = celllines.OrderBy(l => l).ToArray();
                    celllines_length = celllines.Length;
                    for (int indexCell = 0; indexCell < celllines_length; indexCell++)
                    {
                        cellline = celllines[indexCell];
                        treatment_replicateName_gene_dict = cellline_treatment_replicateName_gene_dict[cellline];
                        treatments = treatment_replicateName_gene_dict.Keys.ToArray();
                        treatments = treatments.OrderBy(l => l).ToArray();
                        treatments_length = treatments.Length;
                        for (int indexTreatment = 0; indexTreatment < treatments_length; indexTreatment++)
                        {
                            treatment = treatments[indexTreatment];
                            replicateName_gene_dict = treatment_replicateName_gene_dict[treatment];
                            replicateNames = replicateName_gene_dict.Keys.ToArray();
                            replicateNames = replicateNames.OrderBy(l => l).ToArray();
                            replicateNames_length = replicateNames.Length;
                            for (int indexRN = 0; indexRN < replicateNames_length; indexRN++)
                            {
                                replicateName = replicateNames[indexRN];
                                gene_dict = replicateName_gene_dict[replicateName];
                                genes = gene_dict.Keys.ToArray();
                                genes_length = genes.Length;
                                genes = genes.OrderBy(l => l).ToArray();
                                for (int indexS = 0; indexS < genes_length; indexS++)
                                {
                                    gene = genes[indexS];
                                    ordered_replicates.AddRange(gene_dict[gene]);
                                }
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_replicates.Count;
                if (ordered_length != deg_replicates_length) { throw new Exception(); }
                DEG_replicate_values_line_class this_line;
                DEG_replicate_values_line_class previous_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_replicates[indexO];
                    previous_line = ordered_replicates[indexO - 1];
                    if (this_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.Equals(previous_line.Treatment))
                             && (this_line.Replicate_name.CompareTo(previous_line.Replicate_name) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.Equals(previous_line.Treatment))
                             && (this_line.Replicate_name.Equals(previous_line.Replicate_name))
                             && (this_line.Gene.CompareTo(previous_line.Gene) < 0)) { throw new Exception(); }
                }
            }

            //deg_replicates.OrderBy(l => l.Dataset).ThenBy(l => l.Plate).ThenBy(l => l.Cell_line).ThenBy(l => l.Treatment).ThenBy(l => l.Symbol).ToArray();
            return ordered_replicates.ToArray();
        }
        public static DEG_replicate_values_line_class[] Order_by_dataset_plate_cellline_treatment_replicateName_symbol_descendingExpressionValue(DEG_replicate_values_line_class[] deg_replicates)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>>>> dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>>> plate_cellline_treatment_replicateName_symbol_expressionValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>> cellline_treatment_replicateName_symbol_expressionValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>> treatment_replicateName_symbol_expressionValue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>> replicateName_symbol_expressionValue_dict = new Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>();
            Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>> symbol_expressionValue_dict = new Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>();
            Dictionary<float, List<DEG_replicate_values_line_class>> expressionValue_dict = new Dictionary<float, List<DEG_replicate_values_line_class>>();
            int deg_replicates_length = deg_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            for (int indexRep = 0; indexRep < deg_replicates_length; indexRep++)
            {
                deg_replicate_line = deg_replicates[indexRep];
                if (!dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict.ContainsKey(deg_replicate_line.Dataset))
                {
                    dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict.Add(deg_replicate_line.Dataset, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset].ContainsKey(deg_replicate_line.Plate))
                {
                    dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset].Add(deg_replicate_line.Plate, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].ContainsKey(deg_replicate_line.Cell_line))
                {
                    dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].Add(deg_replicate_line.Cell_line, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line].ContainsKey(deg_replicate_line.Treatment))
                {
                    dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line].Add(deg_replicate_line.Treatment, new Dictionary<string, Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].ContainsKey(deg_replicate_line.Replicate_name))
                {
                    dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].Add(deg_replicate_line.Replicate_name, new Dictionary<string, Dictionary<float, List<DEG_replicate_values_line_class>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_name].ContainsKey(deg_replicate_line.Symbol))
                {
                    dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_name].Add(deg_replicate_line.Symbol, new Dictionary<float, List<DEG_replicate_values_line_class>>());
                }
                if (!dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_name][deg_replicate_line.Symbol].ContainsKey(deg_replicate_line.Expression_value))
                {
                    dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_name][deg_replicate_line.Symbol].Add(deg_replicate_line.Expression_value, new List<DEG_replicate_values_line_class>());
                }
                dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_name][deg_replicate_line.Symbol][deg_replicate_line.Expression_value].Add(deg_replicate_line);
            }
            deg_replicates = null;
            string[] datasets = dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] plates;
            string plate;
            int plates_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            string[] replicateNames;
            string replicateName;
            int replicateNames_length;
            string[] symbols;
            string symbol;
            int symbols_length;
            float[] expressionValues;
            float expressionValue;
            int expressionValues_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<DEG_replicate_values_line_class> ordered_replicates = new List<DEG_replicate_values_line_class>();
            for (int indexData = 0; indexData < datasets_length; indexData++)
            {
                dataset = datasets[indexData];
                plate_cellline_treatment_replicateName_symbol_expressionValue_dict = dataset_plate_cellline_treatment_replicateName_symbol_expressionValue_dict[dataset];
                plates = plate_cellline_treatment_replicateName_symbol_expressionValue_dict.Keys.ToArray();
                plates = plates.OrderBy(l => l).ToArray();
                plates_length = plates.Length;
                for (int indexPlate = 0; indexPlate < plates_length; indexPlate++)
                {
                    plate = plates[indexPlate];
                    cellline_treatment_replicateName_symbol_expressionValue_dict = plate_cellline_treatment_replicateName_symbol_expressionValue_dict[plate];
                    celllines = cellline_treatment_replicateName_symbol_expressionValue_dict.Keys.ToArray();
                    celllines = celllines.OrderBy(l => l).ToArray();
                    celllines_length = celllines.Length;
                    for (int indexCell = 0; indexCell < celllines_length; indexCell++)
                    {
                        cellline = celllines[indexCell];
                        treatment_replicateName_symbol_expressionValue_dict = cellline_treatment_replicateName_symbol_expressionValue_dict[cellline];
                        treatments = treatment_replicateName_symbol_expressionValue_dict.Keys.ToArray();
                        treatments = treatments.OrderBy(l => l).ToArray();
                        treatments_length = treatments.Length;
                        for (int indexTreatment = 0; indexTreatment < treatments_length; indexTreatment++)
                        {
                            treatment = treatments[indexTreatment];
                            replicateName_symbol_expressionValue_dict = treatment_replicateName_symbol_expressionValue_dict[treatment];
                            replicateNames = replicateName_symbol_expressionValue_dict.Keys.ToArray();
                            replicateNames = replicateNames.OrderBy(l => l).ToArray();
                            replicateNames_length = replicateNames.Length;
                            for (int indexRN = 0; indexRN < replicateNames_length; indexRN++)
                            {
                                replicateName = replicateNames[indexRN];
                                symbol_expressionValue_dict = replicateName_symbol_expressionValue_dict[replicateName];
                                symbols = symbol_expressionValue_dict.Keys.ToArray();
                                symbols_length = symbols.Length;
                                symbols = symbols.OrderBy(l => l).ToArray();
                                for (int indexS = 0; indexS < symbols_length; indexS++)
                                {
                                    symbol = symbols[indexS];
                                    expressionValue_dict = symbol_expressionValue_dict[symbol];
                                    expressionValues = expressionValue_dict.Keys.ToArray();
                                    expressionValues = expressionValues.OrderByDescending(l=>l).ToArray();
                                    expressionValues_length = expressionValues.Length;
                                    for (int indexEx=0; indexEx<expressionValues_length; indexEx++)
                                    {
                                        expressionValue = expressionValues[indexEx];
                                        ordered_replicates.AddRange(expressionValue_dict[expressionValue]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_replicates.Count;
                if (ordered_length != deg_replicates_length) { throw new Exception(); }
                DEG_replicate_values_line_class this_line;
                DEG_replicate_values_line_class previous_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_replicates[indexO];
                    previous_line = ordered_replicates[indexO - 1];
                    if (this_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.Equals(previous_line.Treatment))
                             && (this_line.Replicate_name.CompareTo(previous_line.Replicate_name) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.Equals(previous_line.Treatment))
                             && (this_line.Replicate_name.Equals(previous_line.Replicate_name))
                             && (this_line.Symbol.Equals(previous_line.Symbol))
                             && (this_line.Expression_value.CompareTo(previous_line.Expression_value) > 0)) { throw new Exception(); }
            }
        }

            //deg_replicates.OrderBy(l => l.Dataset).ThenBy(l => l.Plate).ThenBy(l => l.Cell_line).ThenBy(l => l.Treatment).ThenBy(l => l.Symbol).ToArray();
            return ordered_replicates.ToArray();
        }

        public static DEG_replicate_values_line_class[] Order_by_dataset_cellline_treatment_replicateNo(DEG_replicate_values_line_class[] deg_replicates)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>> dataset_cellline_treatment_replicateNameNo_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>> cellline_treatment_replicateNameNo_dict = new Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>();
            Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>> treatment_replicateNameNo_dict = new Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>();
            Dictionary<int, List<DEG_replicate_values_line_class>> replicateNameNo_dict = new Dictionary<int, List<DEG_replicate_values_line_class>>();

            int deg_replicates_length = deg_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            for (int indexRep = 0; indexRep < deg_replicates_length; indexRep++)
            {
                deg_replicate_line = deg_replicates[indexRep];
                if (!dataset_cellline_treatment_replicateNameNo_dict.ContainsKey(deg_replicate_line.Dataset))
                {
                    dataset_cellline_treatment_replicateNameNo_dict.Add(deg_replicate_line.Dataset, new Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>());
                }
                if (!dataset_cellline_treatment_replicateNameNo_dict[deg_replicate_line.Dataset].ContainsKey(deg_replicate_line.Cell_line))
                {
                    dataset_cellline_treatment_replicateNameNo_dict[deg_replicate_line.Dataset].Add(deg_replicate_line.Cell_line, new Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>());
                }
                if (!dataset_cellline_treatment_replicateNameNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Cell_line].ContainsKey(deg_replicate_line.Treatment))
                {
                    dataset_cellline_treatment_replicateNameNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Cell_line].Add(deg_replicate_line.Treatment, new Dictionary<int, List<DEG_replicate_values_line_class>>());
                }
                if (!dataset_cellline_treatment_replicateNameNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].ContainsKey(deg_replicate_line.Replicate_no))
                {
                    dataset_cellline_treatment_replicateNameNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].Add(deg_replicate_line.Replicate_no, new List<DEG_replicate_values_line_class>());
                }
                dataset_cellline_treatment_replicateNameNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_no].Add(deg_replicate_line);
            }

            string[] datasets = dataset_cellline_treatment_replicateNameNo_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] celllines;
            string cellline;
            int celllines_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            int[] replicateNameNos;
            int replicateNameNo;
            int replicateNameNos_length;
            //string[] genes;
            //string gene;
            //int genes_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<DEG_replicate_values_line_class> ordered_replicates = new List<DEG_replicate_values_line_class>();
            for (int indexData = 0; indexData < datasets_length; indexData++)
            {
                dataset = datasets[indexData];
                cellline_treatment_replicateNameNo_dict = dataset_cellline_treatment_replicateNameNo_dict[dataset];
                celllines = cellline_treatment_replicateNameNo_dict.Keys.ToArray();
                celllines = celllines.OrderBy(l => l).ToArray();
                celllines_length = celllines.Length;
                for (int indexCellline = 0; indexCellline < celllines_length; indexCellline++)
                {
                    cellline = celllines[indexCellline];
                    treatment_replicateNameNo_dict = cellline_treatment_replicateNameNo_dict[cellline];
                    treatments = treatment_replicateNameNo_dict.Keys.ToArray();
                    treatments = treatments.OrderBy(l => l).ToArray();
                    treatments_length = treatments.Length;
                    for (int indexTreatment = 0; indexTreatment < treatments_length; indexTreatment++)
                    {
                        treatment = treatments[indexTreatment];
                        replicateNameNo_dict = treatment_replicateNameNo_dict[treatment];
                        replicateNameNos = replicateNameNo_dict.Keys.ToArray();
                        replicateNameNos = replicateNameNos.OrderBy(l => l).ToArray();
                        replicateNameNos_length = replicateNameNos.Length;
                        for (int indexReplicateNameNo = 0; indexReplicateNameNo < replicateNameNos_length; indexReplicateNameNo++)
                        {
                            replicateNameNo = replicateNameNos[indexReplicateNameNo];
                            ordered_replicates.AddRange(replicateNameNo_dict[replicateNameNo]);
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_replicates.Count;
                if (ordered_length != deg_replicates_length) { throw new Exception(); }
                DEG_replicate_values_line_class this_line;
                DEG_replicate_values_line_class previous_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_replicates[indexO];
                    previous_line = ordered_replicates[indexO - 1];
                    if (this_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.Equals(previous_line.Treatment))
                             && (this_line.Replicate_no.CompareTo(previous_line.Replicate_no) < 0)) { throw new Exception(); }
                }
            }
            return ordered_replicates.ToArray();
        }

        public static DEG_replicate_values_line_class[] Order_by_dataset_cellline(DEG_replicate_values_line_class[] deg_replicates)
        {
            Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>> dataset_cellline_dict = new Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>();
            Dictionary<string, List<DEG_replicate_values_line_class>> cellline_dict = new Dictionary<string, List<DEG_replicate_values_line_class>>();
            int deg_replicates_length = deg_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            for (int indexRep = 0; indexRep < deg_replicates_length; indexRep++)
            {
                deg_replicate_line = deg_replicates[indexRep];
                if (!dataset_cellline_dict.ContainsKey(deg_replicate_line.Dataset))
                {
                    dataset_cellline_dict.Add(deg_replicate_line.Dataset, new Dictionary<string, List<DEG_replicate_values_line_class>>());
                }
                if (!dataset_cellline_dict[deg_replicate_line.Dataset].ContainsKey(deg_replicate_line.Cell_line))
                {
                    dataset_cellline_dict[deg_replicate_line.Dataset].Add(deg_replicate_line.Cell_line, new List<DEG_replicate_values_line_class>());
                }
                dataset_cellline_dict[deg_replicate_line.Dataset][deg_replicate_line.Cell_line].Add(deg_replicate_line);
            }

            string[] datasets = dataset_cellline_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] celllines;
            string cellline;
            int celllines_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<DEG_replicate_values_line_class> ordered_replicates = new List<DEG_replicate_values_line_class>();
            for (int indexData = 0; indexData < datasets_length; indexData++)
            {
                dataset = datasets[indexData];
                cellline_dict = dataset_cellline_dict[dataset];
                celllines = cellline_dict.Keys.ToArray();
                celllines = celllines.OrderBy(l => l).ToArray();
                celllines_length = celllines.Length;
                for (int indexCell = 0; indexCell < celllines_length; indexCell++)
                {
                    cellline = celllines[indexCell];
                    ordered_replicates.AddRange(cellline_dict[cellline]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_replicates.Count;
                if (ordered_length != deg_replicates_length) { throw new Exception(); }
                DEG_replicate_values_line_class this_line;
                DEG_replicate_values_line_class previous_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_replicates[indexO];
                    previous_line = ordered_replicates[indexO - 1];
                    if (this_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                }
            }

            //deg_replicates.OrderBy(l => l.Dataset).ThenBy(l => l.Plate).ThenBy(l => l.Cell_line).ThenBy(l => l.Treatment).ThenBy(l => l.Symbol).ToArray();
            return ordered_replicates.ToArray();
        }

        public static DEG_replicate_values_line_class[] Order_by_dataset_plate(DEG_replicate_values_line_class[] deg_replicates)
        {
            Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>> dataset_plate_dict = new Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>();
            Dictionary<string, List<DEG_replicate_values_line_class>> plate_dict = new Dictionary<string, List<DEG_replicate_values_line_class>>();
            int deg_replicates_length = deg_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            for (int indexRep = 0; indexRep < deg_replicates_length; indexRep++)
            {
                deg_replicate_line = deg_replicates[indexRep];
                if (!dataset_plate_dict.ContainsKey(deg_replicate_line.Dataset))
                {
                    dataset_plate_dict.Add(deg_replicate_line.Dataset, new Dictionary<string, List<DEG_replicate_values_line_class>>());
                }
                if (!dataset_plate_dict[deg_replicate_line.Dataset].ContainsKey(deg_replicate_line.Cell_line))
                {
                    dataset_plate_dict[deg_replicate_line.Dataset].Add(deg_replicate_line.Plate, new List<DEG_replicate_values_line_class>());
                }
                dataset_plate_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].Add(deg_replicate_line);
            }

            string[] datasets = dataset_plate_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] plates;
            string plate;
            int plates_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<DEG_replicate_values_line_class> ordered_replicates = new List<DEG_replicate_values_line_class>();
            for (int indexData = 0; indexData < datasets_length; indexData++)
            {
                dataset = datasets[indexData];
                plate_dict = dataset_plate_dict[dataset];
                plates = plate_dict.Keys.ToArray();
                plates = plates.OrderBy(l => l).ToArray();
                plates_length = plates.Length;
                for (int indexPlate = 0; indexPlate < plates_length; indexPlate++)
                {
                    plate = plates[indexPlate];
                    ordered_replicates.AddRange(plate_dict[plate]);
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_replicates.Count;
                if (ordered_length != deg_replicates_length) { throw new Exception(); }
                DEG_replicate_values_line_class this_line;
                DEG_replicate_values_line_class previous_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_replicates[indexO];
                    previous_line = ordered_replicates[indexO - 1];
                    if (this_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }
                }
            }

            //deg_replicates.OrderBy(l => l.Dataset).ThenBy(l => l.Plate).ThenBy(l => l.Cell_line).ThenBy(l => l.Treatment).ThenBy(l => l.Symbol).ToArray();
            return ordered_replicates.ToArray();
        }

        public static DEG_replicate_values_line_class[] Order_by_dataset_plate_cellline_treatment(DEG_replicate_values_line_class[] deg_replicates)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>> dataset_plate_cellline_treatment_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>> plate_cellline_treatment_dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>();
            Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>> cellline_treatment_dict = new Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>();
            Dictionary<string, List<DEG_replicate_values_line_class>> treatment_dict = new Dictionary<string, List<DEG_replicate_values_line_class>>();
            int deg_replicates_length = deg_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            for (int indexRep = 0; indexRep < deg_replicates_length; indexRep++)
            {
                deg_replicate_line = deg_replicates[indexRep];
                if (!dataset_plate_cellline_treatment_dict.ContainsKey(deg_replicate_line.Dataset))
                {
                    dataset_plate_cellline_treatment_dict.Add(deg_replicate_line.Dataset, new Dictionary<string, Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>>());
                }
                if (!dataset_plate_cellline_treatment_dict[deg_replicate_line.Dataset].ContainsKey(deg_replicate_line.Plate))
                {
                    dataset_plate_cellline_treatment_dict[deg_replicate_line.Dataset].Add(deg_replicate_line.Plate, new Dictionary<string, Dictionary<string, List<DEG_replicate_values_line_class>>>());
                }
                if (!dataset_plate_cellline_treatment_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].ContainsKey(deg_replicate_line.Cell_line))
                {
                    dataset_plate_cellline_treatment_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].Add(deg_replicate_line.Cell_line, new Dictionary<string, List<DEG_replicate_values_line_class>>());
                }
                if (!dataset_plate_cellline_treatment_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line].ContainsKey(deg_replicate_line.Treatment))
                {
                    dataset_plate_cellline_treatment_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line].Add(deg_replicate_line.Treatment, new List<DEG_replicate_values_line_class>());
                }
                dataset_plate_cellline_treatment_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].Add(deg_replicate_line);
            }

            string[] datasets = dataset_plate_cellline_treatment_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] plates;
            string plate;
            int plates_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<DEG_replicate_values_line_class> ordered_replicates = new List<DEG_replicate_values_line_class>();
            for (int indexData = 0; indexData < datasets_length; indexData++)
            {
                dataset = datasets[indexData];
                plate_cellline_treatment_dict = dataset_plate_cellline_treatment_dict[dataset];
                plates = plate_cellline_treatment_dict.Keys.ToArray();
                plates = plates.OrderBy(l => l).ToArray();
                plates_length = plates.Length;
                for (int indexPlate = 0; indexPlate < plates_length; indexPlate++)
                {
                    plate = plates[indexPlate];
                    cellline_treatment_dict = plate_cellline_treatment_dict[plate];
                    celllines = cellline_treatment_dict.Keys.ToArray();
                    celllines = celllines.OrderBy(l => l).ToArray();
                    celllines_length = celllines.Length;
                    for (int indexCellline = 0; indexCellline < celllines_length; indexCellline++)
                    {
                        cellline = celllines[indexCellline];
                        treatment_dict = cellline_treatment_dict[cellline];
                        treatments = treatment_dict.Keys.ToArray();
                        treatments_length = treatments.Length;
                        for (int indexTreatment = 0; indexTreatment < treatments_length; indexTreatment++)
                        {
                            treatment = treatments[indexTreatment];
                            ordered_replicates.AddRange(treatment_dict[treatment]);
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_replicates.Count;
                if (ordered_length != deg_replicates_length) { throw new Exception(); }
                DEG_replicate_values_line_class this_line;
                DEG_replicate_values_line_class previous_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_replicates[indexO];
                    previous_line = ordered_replicates[indexO - 1];
                    if (this_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }
                }
            }
            return ordered_replicates.ToArray();
        }

        public static DEG_replicate_values_line_class[] Order_by_dataset_plate_cellline_treatment_replicateNo(DEG_replicate_values_line_class[] deg_replicates)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>>> dataset_plate_cellline_treatment_replicateNo_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>> plate_cellline_treatment_replicateNo_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>> cellline_treatment_replicateNo_dict = new Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>();
            Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>> treatment_replicateNo_dict = new Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>();
            Dictionary<int, List<DEG_replicate_values_line_class>> replicateNo_dict = new Dictionary<int, List<DEG_replicate_values_line_class>>();
            int deg_replicates_length = deg_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            for (int indexRep = 0; indexRep < deg_replicates_length; indexRep++)
            {
                deg_replicate_line = deg_replicates[indexRep];
                if (!dataset_plate_cellline_treatment_replicateNo_dict.ContainsKey(deg_replicate_line.Dataset))
                {
                    dataset_plate_cellline_treatment_replicateNo_dict.Add(deg_replicate_line.Dataset, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset].ContainsKey(deg_replicate_line.Plate))
                {
                    dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset].Add(deg_replicate_line.Plate, new Dictionary<string, Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].ContainsKey(deg_replicate_line.Cell_line))
                {
                    dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate].Add(deg_replicate_line.Cell_line, new Dictionary<string, Dictionary<int, List<DEG_replicate_values_line_class>>>());
                }
                if (!dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line].ContainsKey(deg_replicate_line.Treatment))
                {
                    dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line].Add(deg_replicate_line.Treatment, new Dictionary<int, List<DEG_replicate_values_line_class>>());
                }
                if (!dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].ContainsKey(deg_replicate_line.Replicate_no))
                {
                    dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment].Add(deg_replicate_line.Replicate_no, new List<DEG_replicate_values_line_class>());
                }
                dataset_plate_cellline_treatment_replicateNo_dict[deg_replicate_line.Dataset][deg_replicate_line.Plate][deg_replicate_line.Cell_line][deg_replicate_line.Treatment][deg_replicate_line.Replicate_no].Add(deg_replicate_line);
            }

            string[] datasets = dataset_plate_cellline_treatment_replicateNo_dict.Keys.ToArray();
            string dataset;
            int datasets_length = datasets.Length;
            string[] plates;
            string plate;
            int plates_length;
            string[] celllines;
            string cellline;
            int celllines_length;
            string[] treatments;
            string treatment;
            int treatments_length;
            int[] replicateNos;
            int replicateNo;
            int replicateNos_length;
            datasets = datasets.OrderBy(l => l).ToArray();
            List<DEG_replicate_values_line_class> ordered_replicates = new List<DEG_replicate_values_line_class>();
            for (int indexData = 0; indexData < datasets_length; indexData++)
            {
                dataset = datasets[indexData];
                plate_cellline_treatment_replicateNo_dict = dataset_plate_cellline_treatment_replicateNo_dict[dataset];
                plates = plate_cellline_treatment_replicateNo_dict.Keys.ToArray();
                plates = plates.OrderBy(l => l).ToArray();
                plates_length = plates.Length;
                for (int indexPlate = 0; indexPlate < plates_length; indexPlate++)
                {
                    plate = plates[indexPlate];
                    cellline_treatment_replicateNo_dict = plate_cellline_treatment_replicateNo_dict[plate];
                    celllines = cellline_treatment_replicateNo_dict.Keys.ToArray();
                    celllines = celllines.OrderBy(l => l).ToArray();
                    celllines_length = celllines.Length;
                    for (int indexCellline = 0; indexCellline < celllines_length; indexCellline++)
                    {
                        cellline = celllines[indexCellline];
                        treatment_replicateNo_dict = cellline_treatment_replicateNo_dict[cellline];
                        treatments = treatment_replicateNo_dict.Keys.ToArray();
                        treatments_length = treatments.Length;
                        treatments = treatments.OrderBy(l => l).ToArray();
                        for (int indexTreatment = 0; indexTreatment < treatments_length; indexTreatment++)
                        {
                            treatment = treatments[indexTreatment];
                            replicateNo_dict = treatment_replicateNo_dict[treatment];
                            replicateNos = replicateNo_dict.Keys.ToArray();
                            replicateNos_length = replicateNos.Length;
                            replicateNos = replicateNos.OrderBy(l => l).ToArray();
                            for (int indexReplicateNo=0; indexReplicateNo<replicateNos_length;indexReplicateNo++)
                            {
                                replicateNo = replicateNos[indexReplicateNo];
                                ordered_replicates.AddRange(replicateNo_dict[replicateNo]);
                            }
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                int ordered_length = ordered_replicates.Count;
                if (ordered_length != deg_replicates_length) { throw new Exception(); }
                DEG_replicate_values_line_class this_line;
                DEG_replicate_values_line_class previous_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    this_line = ordered_replicates[indexO];
                    previous_line = ordered_replicates[indexO - 1];
                    if (this_line.Dataset.CompareTo(previous_line.Dataset) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.CompareTo(previous_line.Plate) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.CompareTo(previous_line.Cell_line) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.CompareTo(previous_line.Treatment) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset.Equals(previous_line.Dataset))
                             && (this_line.Plate.Equals(previous_line.Plate))
                             && (this_line.Cell_line.Equals(previous_line.Cell_line))
                             && (this_line.Treatment.Equals(previous_line.Treatment))
                             && (this_line.Replicate_no.CompareTo(previous_line.Replicate_no) < 0)) { throw new Exception(); }
                }
            }
            return ordered_replicates.ToArray();
        }


        public DEG_replicate_values_line_class()
        {
            this.Dataset = "";
            this.Plate = "";
            this.Gene = "";
            this.Symbol = "";
            this.Replicate_name = "";
            this.Cell_line = "";
            this.Treatment = "";
            this.Treatment_full_name = "";
            this.Timepoint = Timepoint_enum.E_m_p_t_y;
        }

        public DEG_replicate_values_line_class Deep_copy()
        {
            DEG_replicate_values_line_class copy = (DEG_replicate_values_line_class)this.MemberwiseClone();
            copy.Dataset = (string)this.Dataset.Clone();
            copy.Plate = (string)this.Plate.Clone();
            copy.Gene = (string)this.Gene.Clone();
            copy.Symbol = (string)this.Symbol.Clone();
            copy.Replicate_name = (string)this.Replicate_name.Clone();
            copy.Cell_line = (string)this.Cell_line.Clone();
            copy.Treatment = (string)this.Treatment.Clone();
            copy.Treatment_full_name = (string)this.Treatment_full_name.Clone();
            return copy;
        }
    }

    class DEG_replicate_readWriteOptions_class : ReadWriteOptions_base
    {
        public DEG_replicate_readWriteOptions_class(string directory, string fileName)
        {
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Dataset", "Plate", "Expression_value", "Gene","Symbol", "Replicate_no", "Replicate_name", "Cell_line", "Treatment", "Treatment_full_name","Drug_type", "Replicate_value_type","Timepoint","EntryType" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class DEG_replicate_preprocessed_readWriteOptions_class : ReadWriteOptions_base
    {
        public DEG_replicate_preprocessed_readWriteOptions_class(string directory, string fileName)
        {
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Dataset", "Plate", "Expression_value", "Gene", "Symbol", "Replicate_no", "Replicate_name", "Cell_line", "Treatment", "Treatment_full_name", "Drug_type", "Replicate_value_type", "Timepoint", "EntryType" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }

        public DEG_replicate_preprocessed_readWriteOptions_class(string file_name) : this(Global_directory_class.Final_lincs_degs_directory, file_name)
        {
        }

        public string Get_binary_fileName()
        {
            return Path.GetDirectoryName(File) + "//" + Path.GetFileNameWithoutExtension(File) + ".bin";
        }

    }


    class Combine_DEG_replicate_value_class : IDisposable
    {
        public DEG_replicate_values_line_class[] Deg_replicates { get; set; }
        public Lincs_combine_files_options_class Options { get; set; }
        public Lincs_extract_information_from_edgeR_files_class Extract_information { get; set; }

        public Combine_DEG_replicate_value_class(string dataset)
        {
            this.Options = new Lincs_combine_files_options_class(dataset);
            this.Extract_information = new Lincs_extract_information_from_edgeR_files_class(this.Options.Dataset);
        }

        private DEG_replicate_values_line_class[] Generate_by_reading_indicated_file(string complete_file_name)
        {
            StreamReader reader = new StreamReader(complete_file_name);
            string input_line;
            char delimiter = Global_class.Tab;

            string cellline;
            string plate;
            string drug;
            DE_entry_enum entryType;
            Timepoint_enum timepoint;
            Extract_information.Get_patient_plate_treatment_entryType_timepoint(out cellline, out plate, out drug, out entryType, out timepoint, complete_file_name);

            string headline = reader.ReadLine();
            string[] columnNames = headline.Split(delimiter);
            Lincs_column_indexes_class column_indexes = new Lincs_column_indexes_class();
            column_indexes.Set_all_column_indexes_from_headline(ref columnNames, this.Options.Dataset, complete_file_name);

            int controlIndexes_length = column_indexes.Control_column_indexes.Length;
            int controlNormIndexes_length = column_indexes.Control_norm_column_indexes.Length;
            int treatmentIndexes_length = column_indexes.Treatment_column_indexes.Length;
            int treatmentNormIndexes_length = column_indexes.Treatment_norm_column_indexes.Length;

            DEG_replicate_values_line_class replicate_line;
            DEG_replicate_values_line_class base_replicate_line = new DEG_replicate_values_line_class();
            base_replicate_line = new DEG_replicate_values_line_class();
            base_replicate_line.Cell_line = (string)cellline.Clone();
            base_replicate_line.Replicate_value_type = EdgeR_replicate_value_type_enum.Raw;
            base_replicate_line.Dataset = (string)this.Options.Dataset.Clone();
            base_replicate_line.Plate = (string)plate.Clone();
            base_replicate_line.Timepoint = Timepoint_enum.E_m_p_t_y;

            List<DEG_replicate_values_line_class> replicates = new List<DEG_replicate_values_line_class>();
            string[] columnEntries;
            string columnEntry;
            string columnName;
            string gene;
            int indexControl;
            int indexControlNorm;
            int indexTreatment;
            int indexTreatmentNorm;
            string[] columnEntry_splitStrings;
            while ((input_line=reader.ReadLine())!=null)
            {
                columnEntries = input_line.Split(delimiter);
                if (columnEntries.Length!=column_indexes.Column_names_length) { throw new Exception(); }
                gene = columnEntries[column_indexes.Symbol_column_index];
                for (int indexIndex = 0; indexIndex < controlIndexes_length; indexIndex++)
                {
                    indexControl = column_indexes.Control_column_indexes[indexIndex];
                    columnEntry = columnEntries[indexControl];
                    columnName = columnNames[indexControl];
                    columnEntry_splitStrings = columnName.Split('-')[0].Split('.');
                    replicate_line = base_replicate_line.Deep_copy();
                    replicate_line.Gene = (string)gene.Clone();
                    replicate_line.Replicate_name = (string)columnName.Clone();
                    if (replicate_line.Cell_line.IndexOf(columnEntry_splitStrings[1]) == -1) { throw new Exception(); }
                    replicate_line.Treatment = (string)columnEntry_splitStrings[0].Clone();
                    replicate_line.Replicate_no = indexIndex + 1;
                    replicate_line.Replicate_value_type = EdgeR_replicate_value_type_enum.Raw;
                    replicate_line.Expression_value = float.Parse(columnEntry);
                    replicates.Add(replicate_line);
                }
                for (int indexIndex = 0; indexIndex < controlNormIndexes_length; indexIndex++)
                {
                    indexControlNorm = column_indexes.Control_norm_column_indexes[indexIndex];
                    columnEntry = columnEntries[indexControlNorm];
                    columnName = columnNames[indexControlNorm];
                    columnEntry_splitStrings = columnName.Split('-')[0].Split('.');
                    replicate_line = base_replicate_line.Deep_copy();
                    replicate_line.Gene = (string)gene.Clone();
                    replicate_line.Replicate_name = (string)columnName.Clone();
                    if (replicate_line.Cell_line.IndexOf(columnEntry_splitStrings[1]) == -1) { throw new Exception(); }
                    replicate_line.Treatment = (string)columnEntry_splitStrings[0].Clone();
                    replicate_line.Replicate_no = indexIndex + 1;
                    replicate_line.Replicate_value_type = EdgeR_replicate_value_type_enum.Normalized;
                    replicate_line.Expression_value = float.Parse(columnEntry);
                    //replicates.Add(replicate_line);
                }
                for (int indexIndex = 0; indexIndex < treatmentIndexes_length; indexIndex++)
                {
                    indexTreatment = column_indexes.Treatment_column_indexes[indexIndex];
                    columnEntry = columnEntries[indexTreatment];
                    columnName = columnNames[indexTreatment];
                    columnEntry_splitStrings = columnName.Split('-')[0].Split('.');
                    replicate_line = base_replicate_line.Deep_copy();
                    replicate_line.Gene = (string)gene.Clone();
                    replicate_line.Replicate_name = (string)columnName.Clone();
                    if (replicate_line.Cell_line.IndexOf(columnEntry_splitStrings[1]) == -1) { throw new Exception(); }
                    replicate_line.Treatment = (string)columnEntry_splitStrings[0].Clone();
                    replicate_line.Replicate_no = indexIndex + 1;
                    replicate_line.Replicate_value_type = EdgeR_replicate_value_type_enum.Raw;
                    replicate_line.Expression_value = float.Parse(columnEntry);
                    replicates.Add(replicate_line);
                }
                for (int indexIndex = 0; indexIndex < treatmentNormIndexes_length; indexIndex++)
                {
                    indexTreatmentNorm = column_indexes.Treatment_norm_column_indexes[indexIndex];
                    columnEntry = columnEntries[indexTreatmentNorm];
                    columnName = columnNames[indexTreatmentNorm];
                    columnEntry_splitStrings = columnName.Split('-')[0].Split('.');
                    replicate_line = base_replicate_line.Deep_copy();
                    replicate_line.Gene = (string)gene.Clone();
                    replicate_line.Replicate_name = (string)columnName.Clone();
                    if (replicate_line.Cell_line.IndexOf(columnEntry_splitStrings[1]) == -1) { throw new Exception(); }
                    replicate_line.Treatment = (string)columnEntry_splitStrings[0].Clone();
                    replicate_line.Replicate_no = indexIndex + 1;
                    replicate_line.Replicate_value_type = EdgeR_replicate_value_type_enum.Normalized;
                    replicate_line.Expression_value = float.Parse(columnEntry);
                    //replicates.Add(replicate_line);
                }
            }
            return replicates.ToArray();
        }

        private void Generate_by_reading_allFiles()
        {
            string deg_fileName_label = (string)Options.Full_degs_marker.Clone();
            string[] add_allFiles;
            List<string> allFiles_list = new List<string>();
            for (int indexD=0; indexD<Options.Directories.Length;indexD++)
            {
                add_allFiles = Directory.GetFiles(Options.Directories[indexD]);
                allFiles_list.AddRange(add_allFiles);
            }
            string[] allFiles = allFiles_list.Distinct().OrderBy(l => l).ToArray();
            string current_file;
            int allFiles_length = allFiles.Length;
            DEG_replicate_values_line_class[] add_value_lines;
            List<DEG_replicate_values_line_class> add_values_list = new List<DEG_replicate_values_line_class>();
            for (int indexFile = 0; indexFile<allFiles_length;indexFile++)
            {
                current_file = allFiles[indexFile];
                if (current_file.IndexOf(deg_fileName_label) !=-1)
                {
                    add_value_lines = Generate_by_reading_indicated_file(current_file);
                    add_values_list.AddRange(add_value_lines);
                }
            }
            this.Deg_replicates = add_values_list.ToArray();
        }

        private void Add_drugTypes_and_fullDrug_names()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string,Drug_type_enum> drug_drugType_dict = drug_legend.Get_drug_drugType_dictionary();
            Dictionary<string, string> drug_fullDrugName_dict = drug_legend.Get_drug_fullDrugName_dictionary();
            int replicates_length = this.Deg_replicates.Length;
            DEG_replicate_values_line_class replicate_line;
            for (int indexR=0; indexR<replicates_length;indexR++)
            {
                replicate_line = this.Deg_replicates[indexR];
                if (replicate_line.Treatment.Equals("CTRL"))
                {
                    replicate_line.Drug_type = Drug_type_enum.Control;
                    replicate_line.Treatment = "CTRL";
                }
                else
                {
                    replicate_line.Drug_type = drug_drugType_dict[replicate_line.Treatment];
                    replicate_line.Treatment_full_name = drug_fullDrugName_dict[replicate_line.Treatment];
                }
            }
        }

        private void Remove_duplicates_and_check_if_all_values_are_equal_between_duplicates()
        {
            int replicates_length = this.Deg_replicates.Length;
            this.Deg_replicates = DEG_replicate_values_line_class.Order_by_dataset_plate_cellline_treatment_replicateName_gene(this.Deg_replicates);
            DEG_replicate_values_line_class replicate_line;
            List<DEG_replicate_values_line_class> keep = new List<DEG_replicate_values_line_class>();
            for (int indexRep=0; indexRep<replicates_length; indexRep++)
            {
                replicate_line = this.Deg_replicates[indexRep];
                if (  (indexRep==0)
                    || (!replicate_line.Dataset.Equals(this.Deg_replicates[indexRep - 1].Dataset))
                    || (!replicate_line.Cell_line.Equals(this.Deg_replicates[indexRep - 1].Cell_line))
                    || (!replicate_line.Treatment.Equals(this.Deg_replicates[indexRep - 1].Treatment))
                    || (!replicate_line.Gene.Equals(this.Deg_replicates[indexRep - 1].Gene))
                    || (!replicate_line.Plate.Equals(this.Deg_replicates[indexRep - 1].Plate))
                    || (!replicate_line.Replicate_name.Equals(this.Deg_replicates[indexRep - 1].Replicate_name)))
                {
                    keep.Add(replicate_line);
                }
                else if (   (!replicate_line.Expression_value.Equals(this.Deg_replicates[indexRep-1].Expression_value))
                         || (!replicate_line.Drug_type.Equals(this.Deg_replicates[indexRep - 1].Drug_type))
                         || (!replicate_line.Symbol.Equals(this.Deg_replicates[indexRep - 1].Symbol))
                         || (!replicate_line.Replicate_no.Equals(this.Deg_replicates[indexRep - 1].Replicate_no))
                         || (!replicate_line.Timepoint.Equals(this.Deg_replicates[indexRep - 1].Timepoint))
                         || (!replicate_line.EntryType.Equals(this.Deg_replicates[indexRep - 1].EntryType))
                         || (!replicate_line.Replicate_value_type.Equals(this.Deg_replicates[indexRep - 1].Replicate_value_type))
                         || (!replicate_line.Treatment_full_name.Equals(this.Deg_replicates[indexRep - 1].Treatment_full_name)))
                {
                    throw new Exception();
                }
            }
            this.Deg_replicates = keep.ToArray();
        }

        public void Generate_de_novo_and_write()
        {
            Generate_by_reading_allFiles();
            Add_drugTypes_and_fullDrug_names();
            Remove_duplicates_and_check_if_all_values_are_equal_between_duplicates();
            Write();
        }
        public void Dispose()
        {
            Deg_replicates = null;
            Options = null;
            Extract_information = null;
        }
        private void Write()
        {
            string directory = this.Options.Output_directory;
            DEG_replicate_readWriteOptions_class readWriteOptions = new DEG_replicate_readWriteOptions_class(directory, this.Options.Combined_replicates_fileName);
            ReadWriteClass.WriteData(this.Deg_replicates, readWriteOptions);
        }
    }

    class DEG_replicate_value_options_class
    {
        public RefSeq_accepted_accessionNumberType_enum RefSeq_accepted_accessionNumberType { get; set; }

        public string Get_name_of_preprocessed_degs(string original_fileName)
        {
            string original_file_name_withoutExtension = Path.GetFileNameWithoutExtension(original_fileName);
            string extension = Path.GetExtension(original_fileName);
            string name_of_preprocessed_degs = original_file_name_withoutExtension + "_" + RefSeq_accepted_accessionNumberType + extension;
            return name_of_preprocessed_degs;
        }

    }

    class DEG_replicate_value_class
    {
        public DEG_replicate_values_line_class[] DEG_replicates { get; set; }
        public DEG_replicate_value_options_class Options { get; set; }
        public string[] Bg_symbols_in_upperCase { get; set; }

        public DEG_replicate_value_class()
        {
            this.DEG_replicates = new DEG_replicate_values_line_class[0];
            this.Options = new DEG_replicate_value_options_class();
        }

        private void Add_to_array(DEG_replicate_values_line_class[] add_deg_replicates)
        {
            int add_length = add_deg_replicates.Length;
            int this_length = this.DEG_replicates.Length;
            int new_length = add_length + this_length;
            DEG_replicate_values_line_class[] new_deg_replicates = new DEG_replicate_values_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_deg_replicates[indexNew] = this.DEG_replicates[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_deg_replicates[indexNew] = add_deg_replicates[indexAdd];
            }
            this.DEG_replicates = new_deg_replicates;
        }

        #region Order
        public void Order_by_dataset_plate_cellline_treatment_replicateNo()
        {
            this.DEG_replicates = DEG_replicate_values_line_class.Order_by_dataset_plate_cellline_treatment_replicateNo(this.DEG_replicates);
        }
        #endregion

        public void Keep_only_lines_matching_with_score_of_interest(Deg_score_of_interest_enum deg_score_of_interest)
        {
            List<DEG_replicate_values_line_class> keep = new List<DEG_replicate_values_line_class>();
            int replicate_length = this.DEG_replicates.Length;
            DEG_replicate_values_line_class replicate_value_line;
            for (int indexR=0; indexR<replicate_length;indexR++)
            {
                replicate_value_line = this.DEG_replicates[indexR];
                switch (deg_score_of_interest)
                {
                    case Deg_score_of_interest_enum.Control_expression_values:
                        if (replicate_value_line.Drug_type.Equals(Drug_type_enum.Control))
                        {
                            keep.Add(replicate_value_line);
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }
            this.DEG_replicates = keep.ToArray();
        }

        #region Bg symbols
        public string[] Get_all_bgSymbols_in_upperCase()
        {
            return Array_class.Deep_copy_string_array(this.Bg_symbols_in_upperCase);
        }
        private void Set_bg_symbols_in_upperCase()
        {
            Dictionary<string, bool> bg_symbols_in_upperCase = new Dictionary<string, bool>();
            foreach (DEG_replicate_values_line_class replicate_value_line in this.DEG_replicates)
            {
                if (bg_symbols_in_upperCase.ContainsKey(replicate_value_line.Symbol))
                {
                    bg_symbols_in_upperCase.Add(replicate_value_line.Symbol, true);
                }
            }
            this.Bg_symbols_in_upperCase = bg_symbols_in_upperCase.Keys.ToArray();
        }
        #endregion

        #region Generate and preprocess
        private void Add_ncbi_gene_symbols_based_on_ncbiRefSeqGeneIds()
        {
            Report_class.WriteLine("{0}: Add ncbi gene symbols based on ncbiRefSeqGeneIds", typeof(Deg_class).Name);
            using (NcbiRefSeq_lincs_class ncbiRefSeq = new NcbiRefSeq_lincs_class())
            {
                ncbiRefSeq.Options.RefSeq_accepted_accesionNumberType = this.Options.RefSeq_accepted_accessionNumberType;
                ncbiRefSeq.Generate_by_reading_safed_file();
                NcbiRefSeq_lincs_line_class ncbi_line;
                ncbiRefSeq.NcbiRefSeq = ncbiRefSeq.NcbiRefSeq.OrderBy(l => l.UCSC_refSeqGeneId).ThenBy(l => l.NCBI_symbol).ToArray();
                int indexNCBI = 0;
                int indexNCBI_intern = 0;

                int ncbi_length = ncbiRefSeq.NcbiRefSeq.Length;

                this.DEG_replicates = DEG_replicate_values_line_class.Order_by_gene(this.DEG_replicates);
                int degs_length = this.DEG_replicates.Length;
                DEG_replicate_values_line_class deg_replicate_value_line;
                int stringCompare = -2;

                DEG_replicate_values_line_class new_deg_value_line;
                List<DEG_replicate_values_line_class> new_deg_value_lines = new List<DEG_replicate_values_line_class>();

                bool replaced;
                for (int indexDEG = 0; indexDEG < degs_length; indexDEG++)
                {
                    deg_replicate_value_line = this.DEG_replicates[indexDEG];
                    replaced = false;
                    stringCompare = -2;
                    indexNCBI_intern = indexNCBI;
                    while ((indexNCBI_intern < ncbi_length) && (stringCompare <= 0))
                    {
                        ncbi_line = ncbiRefSeq.NcbiRefSeq[indexNCBI_intern];
                        if ((indexNCBI_intern == 0)
                            || (!ncbi_line.UCSC_refSeqGeneId.Equals(ncbiRefSeq.NcbiRefSeq[indexNCBI_intern - 1].UCSC_refSeqGeneId))
                            || (!ncbi_line.NCBI_symbol.Equals(ncbiRefSeq.NcbiRefSeq[indexNCBI_intern - 1].NCBI_symbol)))
                        {
                            stringCompare = ncbi_line.UCSC_refSeqGeneId.ToUpper().CompareTo(deg_replicate_value_line.Gene.ToUpper());
                            if (stringCompare < 0)
                            {
                                indexNCBI++;
                                indexNCBI_intern = indexNCBI;
                            }
                            else if (stringCompare == 0)
                            {
                                if (!replaced)
                                {
                                    deg_replicate_value_line.Symbol = (string)ncbi_line.NCBI_symbol.Clone();
                                    replaced = true;
                                }
                                else
                                {
                                    new_deg_value_line = deg_replicate_value_line.Deep_copy();
                                    new_deg_value_line.Symbol = (string)ncbi_line.NCBI_symbol.Clone();
                                    new_deg_value_lines.Add(new_deg_value_line);
                                }
                                indexNCBI_intern++;
                            }
                        }
                        else
                        {
                            indexNCBI_intern++;
                        }
                    }
                    if ((!replaced) || (String.IsNullOrEmpty(deg_replicate_value_line.Symbol)))
                    {
                        //throw new Exception();
                    }
                }
                Add_to_array(new_deg_value_lines.ToArray());
                for (int i = 0; i < typeof(DEG_replicate_value_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("{0} new deg lines added, because refSeqGeneId refered to multiple symbols", new_deg_value_lines.Count);
            }
        }
        private void Remove_empty_symbols()
        {
            Report_class.Write("{0}: Remove empty symbols names: ", typeof(Deg_class).Name);
            int degs_length = DEG_replicates.Length;
            DEG_replicate_values_line_class deg_line;
            List<DEG_replicate_values_line_class> kept_degs = new List<DEG_replicate_values_line_class>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = DEG_replicates[indexDeg];
                if ((!String.IsNullOrEmpty(deg_line.Symbol))
                    && (!deg_line.Symbol.Equals(Global_class.Empty_entry)))
                {
                    kept_degs.Add(deg_line);
                }
            }
            Report_class.WriteLine("{0} of {1} deg_lines kept", kept_degs.Count, degs_length);
            DEG_replicates = kept_degs.ToArray();
        }
        private void Remove_duplicated_lines_by_keeping_the_highest_expression_value()
        {
            DEG_replicates= DEG_replicate_values_line_class.Order_by_dataset_plate_cellline_treatment_replicateName_symbol_descendingExpressionValue(DEG_replicates);

            Report_class.Write("{0}: Remove duplicated lines: ", typeof(Deg_class).Name);
            int degs_length = DEG_replicates.Length;
            DEG_replicate_values_line_class deg_line;
            List<DEG_replicate_values_line_class> kept_degs = new List<DEG_replicate_values_line_class>();
            for (int indexDeg = 0; indexDeg < degs_length; indexDeg++)
            {
                deg_line = DEG_replicates[indexDeg];
                if (   (indexDeg==0)
                    || (!deg_line.Dataset.Equals(DEG_replicates[indexDeg-1].Dataset))
                    || (!deg_line.Plate.Equals(DEG_replicates[indexDeg - 1].Plate))
                    || (!deg_line.Cell_line.Equals(DEG_replicates[indexDeg - 1].Cell_line))
                    || (!deg_line.Treatment.Equals(DEG_replicates[indexDeg - 1].Treatment))
                    || (!deg_line.Replicate_name.Equals(DEG_replicates[indexDeg - 1].Replicate_name))
                    || (!deg_line.Symbol.Equals(DEG_replicates[indexDeg - 1].Symbol)))
                {
                    kept_degs.Add(deg_line);
                }
            }
            Report_class.WriteLine("{0} of {1} deg_lines kept", kept_degs.Count, degs_length);
            DEG_replicates = kept_degs.ToArray();
        }
        public DE_class Generate_new_de_instance(Deg_score_of_interest_enum score_of_interest)
        {
            int degs_length = this.DEG_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_value_line;
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_list = new List<Fill_de_line_class>();
            bool generate_fill_de_line = false;
            List<string> allTreatments = new List<string>();
            for (int indexD = 0; indexD < degs_length; indexD++)
            {
                deg_replicate_value_line = this.DEG_replicates[indexD];
                allTreatments.Add(deg_replicate_value_line.Treatment);
                generate_fill_de_line = false;
                switch (score_of_interest)
                {
                    case Deg_score_of_interest_enum.Control_expression_values:
                        if (deg_replicate_value_line.Treatment.Equals("CTRL")) { generate_fill_de_line = true; }
                        break;
                    default:
                        throw new Exception();
                }
                if (generate_fill_de_line)
                {
                    fill_de_line = new Fill_de_line_class();
                    fill_de_line.Timepoint_for_de = deg_replicate_value_line.Timepoint;

                    switch (score_of_interest)
                    {
                        case Deg_score_of_interest_enum.Control_expression_values:
                            fill_de_line.Names_for_de = new string[] { deg_replicate_value_line.Drug_type.ToString(), deg_replicate_value_line.Cell_line, deg_replicate_value_line.Treatment, deg_replicate_value_line.Plate + "_repNo" + deg_replicate_value_line.Replicate_no }; 
                            break;
                            //fill_de_line.Names_for_de = new string[] { deg_replicate_value_line.Drug_type.ToString(), deg_replicate_value_line.Cell_line, deg_replicate_value_line.Treatment + "_repNO" + deg_replicate_value_line.Replicate_no, deg_replicate_value_line.Plate + "_repNO" + deg_replicate_value_line.Replicate_no };
                            //break;
                        default:
                            throw new Exception();
                    }
                    fill_de_line.Symbols_for_de = new string[] { deg_replicate_value_line.Symbol };
                    fill_de_line.Entry_type_for_de = deg_replicate_value_line.EntryType;
                    switch (score_of_interest)
                    {
                        case Deg_score_of_interest_enum.Control_expression_values:
                            fill_de_line.Value_for_de = deg_replicate_value_line.Expression_value;
                            break;
                        default:
                            throw new Exception();
                    }
                    fill_list.Add(fill_de_line);
                }
            }
            allTreatments = allTreatments.Distinct().ToList();
            DE_class de = new DE_class();
            de.Fill_with_data(fill_list.ToArray());
            return de;
        }
        public void Generate_by_reading_safed_files_and_process(string fileName)
        {
            Read_saved_file(fileName);
            Add_ncbi_gene_symbols_based_on_ncbiRefSeqGeneIds();
            Remove_empty_symbols();
            Remove_duplicated_lines_by_keeping_the_highest_expression_value();
        }
        private void Read_saved_file(string fileName)
        {
            string directory = Global_directory_class.Lincs_degs_non_binary_files_directory;
            DEG_replicate_readWriteOptions_class readWriteOptions = new DEG_replicate_readWriteOptions_class(directory, fileName);
            this.DEG_replicates = ReadWriteClass.ReadRawData_and_FillArray<DEG_replicate_values_line_class>(readWriteOptions);
        }
        #endregion

        #region Read, write
        public void Write_preprocessed_as_binary(string filename_before_preprocessing)
        {
            string preprocessed_filename = Options.Get_name_of_preprocessed_degs(filename_before_preprocessing);
            DEG_replicate_preprocessed_readWriteOptions_class readWriteOptions = new DEG_replicate_preprocessed_readWriteOptions_class(preprocessed_filename);
            string preprocesses_binary_fileName = readWriteOptions.Get_binary_fileName();
            BinaryWriter binaryWriter = new BinaryWriter(File.Open(preprocesses_binary_fileName, FileMode.Create));
            int degs_replicates_length = this.DEG_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            for (int indexDeg = 0; indexDeg < degs_replicates_length; indexDeg++)
            {
                deg_replicate_line = this.DEG_replicates[indexDeg];
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_replicate_line.Cell_line));
                binaryWriter.Write(deg_replicate_line.Timepoint.ToString());
                binaryWriter.Write(deg_replicate_line.EntryType.ToString());
                binaryWriter.Write(deg_replicate_line.Drug_type.ToString());
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_replicate_line.Replicate_name));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_replicate_line.Dataset));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_replicate_line.Plate));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_replicate_line.Treatment));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(deg_replicate_line.Treatment_full_name));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_replicate_line.Gene));
                binaryWriter.Write(ReadWriteBinary_class.Get_string_to_write_for_binaryWriter(deg_replicate_line.Symbol));
                binaryWriter.Write(deg_replicate_line.Expression_value);
                binaryWriter.Write(deg_replicate_line.Replicate_no);
            }
            binaryWriter.Close();
        }
        public void Read_preprocessed_as_binary_add_to_array_and_set_bg_genes(string filename_before_preprocessing)
        {
            string preprocessed_filename = Options.Get_name_of_preprocessed_degs(filename_before_preprocessing);
            Deg_preprocessed_readWriteOptions_class readWriteOptions = new Deg_preprocessed_readWriteOptions_class(preprocessed_filename);
            string preprocesses_binary_fileName = readWriteOptions.Get_binary_fileName();
            BinaryReader binaryReader = new BinaryReader(File.OpenRead(preprocesses_binary_fileName));
            int degs_length = this.DEG_replicates.Length;
            DEG_replicate_values_line_class deg_replicate_line;
            List<DEG_replicate_values_line_class> deg_replicate_list = new List<DEG_replicate_values_line_class>();
            long baseStream_length = binaryReader.BaseStream.Length;
            while (binaryReader.BaseStream.Position != baseStream_length)
            {
                deg_replicate_line = new DEG_replicate_values_line_class();
                deg_replicate_line.Cell_line = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_replicate_line.Timepoint = (Timepoint_enum)Enum.Parse(typeof(Timepoint_enum), binaryReader.ReadString());
                deg_replicate_line.EntryType = (DE_entry_enum)Enum.Parse(typeof(DE_entry_enum), binaryReader.ReadString());
                deg_replicate_line.Drug_type = (Drug_type_enum)Enum.Parse(typeof(Drug_type_enum), binaryReader.ReadString());

                deg_replicate_line.Replicate_name = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_replicate_line.Dataset = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_replicate_line.Plate = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_replicate_line.Treatment = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_replicate_line.Treatment_full_name = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_replicate_line.Gene = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_replicate_line.Symbol = ReadWriteBinary_class.Get_string_to_assign_after_reading_via_binaryReader(binaryReader.ReadString());
                deg_replicate_line.Expression_value = binaryReader.ReadSingle();
                deg_replicate_line.Replicate_no = binaryReader.ReadInt32();
                deg_replicate_list.Add(deg_replicate_line);
            }
            Add_to_array(deg_replicate_list.ToArray());
            binaryReader.Close();
            Set_bg_symbols_in_upperCase();
        }
        #endregion
    }

}
