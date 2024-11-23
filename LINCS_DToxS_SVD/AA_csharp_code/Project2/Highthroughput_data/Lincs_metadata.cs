using System;
using System.Collections.Generic;
using System.Linq;
using Common_classes;
using ReadWrite;
using Highthroughput_data;

namespace Adverse_event
{
    class Lincs_experimentalMetadata_input_line_class
    {
        public static string Get_control_drug_id() {  return "CTRL"; }

        public string Drug_name { get; set; }
        public int Sequencing_id { get; set; }
        public int Sequencing_run { get; set; }
        public string Well_number { get; set; }
        public int Investigative_units { get; set; }
        public string Drug_vehicle { get; set; }
        public string Control_vehicle { get; set; }
        public string Sample_number { get; set; }
        public string Sequencing_date { get; set; }
        public string Library_preparation { get; set; }
        public string Experiment_date { get; set; }
        public string Cell_name { get; set; }
        public string Drug_id { get; set; }
        public float Stimulation_time_in_h { get; set; }
        public float Rna_integrity_score { get; set; }
        public float Qubit { get; set; }
        public float Final_concentration { get; set; } 
        public string Final_concentration_unit { get; set; }

        public Lincs_experimentalMetadata_input_line_class Deep_copy()
        {
            Lincs_experimentalMetadata_input_line_class copy = (Lincs_experimentalMetadata_input_line_class)this.MemberwiseClone();
            copy.Drug_name = (string)this.Drug_name.Clone();
            copy.Well_number = (string)this.Well_number.Clone();
            copy.Sample_number = (string)this.Sample_number.Clone();
            copy.Sequencing_date = (string)this.Sequencing_date.Clone();
            copy.Experiment_date = (string)this.Experiment_date.Clone();
            copy.Drug_id = (string)this.Drug_id.Clone();
            copy.Library_preparation = (string)this.Library_preparation.Clone();
            copy.Final_concentration_unit = (string)this.Final_concentration_unit.Clone();
            return copy;
        }
    }

    class Lincs_experimentalMetadata_input_readWriteOptions_class : ReadWriteOptions_base
    {
        public Lincs_experimentalMetadata_input_readWriteOptions_class(string dataset)
        {
            this.File = Global_directory_class.Metadata_directory + "Lincs_experimental_metadata.txt";
            this.Key_propertyNames = new string[] { "Drug_name", "Sequencing_run", "Well_number", "Cell_name", "Drug_id", "Stimulation_time_in_h", "Rna_integrity_score", "Final_concentration", "Final_concentration_unit", "Library_preparation" };
            this.Key_columnNames = new string[] { "DrugName",    "SeqRun",         "WellNO",      "CellName",  "Drug_ID", "Time(hrs)",             "RIN",                 "FinalConc",           "FinalConc_Unit",           "Library preparation" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Lincs_experimentalMetadata_input_class
    {
        public Lincs_experimentalMetadata_input_line_class[] Exp_metadata { get; set; }

        private void Remove_all_double_drugTreatments()
        {
            int tina_length = Exp_metadata.Length;
            Lincs_experimentalMetadata_input_line_class expSummary_line;
            List<Lincs_experimentalMetadata_input_line_class> keep = new List<Lincs_experimentalMetadata_input_line_class>();
            int indexPlus;
            for (int indexT=0;indexT<tina_length;indexT++)
            {
                expSummary_line = this.Exp_metadata[indexT];
                indexPlus = expSummary_line.Drug_id.IndexOf('+');
                if ((indexPlus==-1)||(indexPlus==expSummary_line.Drug_id.Length))
                {
                    keep.Add(expSummary_line);
                }
            }
            this.Exp_metadata = keep.ToArray();
        }

        private void Convert_nM_to_uM()
        {
            foreach (Lincs_experimentalMetadata_input_line_class metadata_input_line in this.Exp_metadata)
            {
                if (metadata_input_line.Final_concentration_unit.Equals("nM"))
                {
                    metadata_input_line.Final_concentration = metadata_input_line.Final_concentration / 1000F;
                    metadata_input_line.Final_concentration_unit = "uM";
                }
            }
        }

        private void Add_plus_to_higher_concentrations()
        {
            int tina_length = Exp_metadata.Length;
            Lincs_experimentalMetadata_input_line_class expSummary_line;
            Lincs_experimentalMetadata_input_line_class inner_expSummary_line;
            this.Exp_metadata = this.Exp_metadata.OrderBy(l => l.Drug_id).ThenBy(l=>l.Sequencing_run).ThenBy(l => l.Final_concentration).ThenBy(l=>l.Final_concentration_unit).ToArray();
            int firstIndex_sameDrugId = 0;
            int current_concentration_count = 0;
            int total_concentration_count = 0;
            string final_concentration_unit = "";
            float final_concentration = -1;
            for (int indexT = 0; indexT < tina_length; indexT++)
            {
                expSummary_line = this.Exp_metadata[indexT];
                if (  (indexT == 0)
                    || (!expSummary_line.Drug_id.Equals(this.Exp_metadata[indexT - 1].Drug_id)))
                { 
                    total_concentration_count = 0;
                    firstIndex_sameDrugId = indexT;
                    final_concentration_unit = expSummary_line.Final_concentration_unit;
                    final_concentration = expSummary_line.Final_concentration;
                }
                if (!expSummary_line.Final_concentration_unit.Equals(final_concentration_unit))
                {
                    throw new Exception();
                }
                if (!expSummary_line.Final_concentration.Equals(final_concentration))
                {
                    throw new Exception();
                }
                if (  (indexT == 0)
                    || (!expSummary_line.Drug_id.Equals(this.Exp_metadata[indexT-1].Drug_id))
                    || (!expSummary_line.Final_concentration.Equals(this.Exp_metadata[indexT-1].Final_concentration))
                    || (!expSummary_line.Final_concentration_unit.Equals(this.Exp_metadata[indexT-1].Final_concentration_unit)))
                {
                    total_concentration_count++;
                }
                if ((indexT == tina_length-1)
                    || (!expSummary_line.Drug_id.Equals(this.Exp_metadata[indexT+1].Drug_id)))
                {
                    if (total_concentration_count>1)
                    {
                        current_concentration_count = -1;
                        for (int indexInner=firstIndex_sameDrugId;indexInner<=indexT;indexInner++)
                        {
                            inner_expSummary_line = this.Exp_metadata[indexInner];
                            if (  (indexInner==firstIndex_sameDrugId)
                                || (!inner_expSummary_line.Final_concentration.Equals(this.Exp_metadata[indexInner - 1].Final_concentration)))
                            {
                                current_concentration_count++;
                            }
                            for (int indexAdd=0; indexAdd<current_concentration_count;indexAdd++)
                            {
                                inner_expSummary_line.Drug_id = inner_expSummary_line.Drug_id + "+";
                            }
                        }
                    }
                }

            }
        }

        public void Generate(string dataset)
        {
            Read(dataset);
            Remove_all_double_drugTreatments();
            Convert_nM_to_uM();
            Add_plus_to_higher_concentrations();
        }

        private void Read(string dataset)
        {
            Lincs_experimentalMetadata_input_readWriteOptions_class readWriteOptions = new Lincs_experimentalMetadata_input_readWriteOptions_class(dataset);
            this.Exp_metadata = ReadWriteClass.ReadRawData_and_FillArray<Lincs_experimentalMetadata_input_line_class>(readWriteOptions);
        }

    }

    class Lincs_experimental_metadata_line_class
    {
        public string Drug_name { get; set; }
        public string Drug_id { get; set; }
        public string Drug_id_without_plus_at_end
        {
            get
            {
                string drug_id_without_plus = (string)Drug_id.Clone();
                while (drug_id_without_plus.Substring(drug_id_without_plus.Length - 1, 1).Equals("+"))
                {
                    drug_id_without_plus = drug_id_without_plus.Substring(0, drug_id_without_plus.Length - 1);
                }
                return drug_id_without_plus;
            }
        }
        public Drug_type_enum Drug_type { get; set; }
        public int Sequencing_run { get; set; }
        public string[] Experimental_dates_drug { get; set; }
        public string[] Experimental_dates_control { get; set; }
        public string Cell_name { get; set; }
        public float Stimulation_time_in_h { get; set; }
        public float[] RNA_integrity_scores_drug { get; set; }
        public float[] RNA_integrity_scores_control { get; set; }
        public float Final_drug_concentration { get; set; }
        public string Final_drug_concentration_unit { get; set; }
        public int Drug_replicates_count_initial { get; set; }
        public int Drug_replicates_count_after_computational_pipeline { get; set; }
        public int Control_replicates_count_initial { get; set; }
        public int Control_replicates_count_after_computational_pipeline { get; set; }
        public string Sequencing_platform { get; set; }
        public string Library_preparation_method { get; set; }

        public string DrugClass_for_website{ get; set; }
        public string Cellline_for_website { get; set; }
        public string Plate_for_website { get; set; }
        public string Is_cardiotoxic { get; set; }

        #region Fixed properties
        public string Plate { get { return "Plate." + Sequencing_run; } }
        public DE_entry_enum EntryType { get { return DE_entry_enum.Rna; } }
        public string Drug_concentration_plus_unit
        {
            get { return Final_drug_concentration + Final_drug_concentration_unit; }
        }
        #endregion

        public string ReadWrite_exerimental_dates_drug
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Experimental_dates_drug, Lincs_experimental_metadata_website_readWriteOptions_class.Array_delimiter); }
            set { this.Experimental_dates_drug = ReadWriteClass.Get_array_from_readLine<string>(value, Lincs_experimental_metadata_website_readWriteOptions_class.Array_delimiter); }
        }

        public string ReadWrite_experimental_dates_control
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Experimental_dates_control, Lincs_experimental_metadata_website_readWriteOptions_class.Array_delimiter); }
            set { this.Experimental_dates_control = ReadWriteClass.Get_array_from_readLine<string>(value, Lincs_experimental_metadata_website_readWriteOptions_class.Array_delimiter); }
        }

        public string ReadWrite_RNA_integrity_scores_drug
        {
            get { return ReadWriteClass.Get_writeLine_from_array(RNA_integrity_scores_drug, Lincs_experimental_metadata_website_readWriteOptions_class.Array_delimiter); }
            set { this.RNA_integrity_scores_drug = ReadWriteClass.Get_array_from_readLine<float>(value, Lincs_experimental_metadata_website_readWriteOptions_class.Array_delimiter); }
        }

        public string ReadWrite_RNA_integrity_scores_control
        {
            get { return ReadWriteClass.Get_writeLine_from_array(RNA_integrity_scores_control, Lincs_experimental_metadata_website_readWriteOptions_class.Array_delimiter); }
            set { this.RNA_integrity_scores_control = ReadWriteClass.Get_array_from_readLine<float>(value, Lincs_experimental_metadata_website_readWriteOptions_class.Array_delimiter); }
        }

        public Lincs_experimental_metadata_line_class()
        {
            Library_preparation_method = "";
            Sequencing_platform = "NovaSeq";
            Is_cardiotoxic = "";
        }

        public Lincs_experimental_metadata_line_class Deep_copy()
        {
            Lincs_experimental_metadata_line_class copy = (Lincs_experimental_metadata_line_class)this.MemberwiseClone();
            copy.Drug_id = (string)this.Drug_id.Clone();
            copy.Drug_name = (string)this.Drug_name.Clone();
            copy.Cell_name = (string)this.Cell_name.Clone();
            copy.Is_cardiotoxic = (string)this.Is_cardiotoxic.Clone();
            copy.Sequencing_platform = (string)this.Sequencing_platform.Clone();
            copy.DrugClass_for_website = (string)this.DrugClass_for_website.Clone();
            copy.Cellline_for_website = (string)this.Cellline_for_website.Clone();
            copy.Plate_for_website = (string)this.Plate_for_website.Clone();
            copy.Experimental_dates_drug = Array_class.Deep_copy_string_array(this.Experimental_dates_drug);
            copy.Experimental_dates_control = Array_class.Deep_copy_string_array(this.Experimental_dates_control);
            copy.RNA_integrity_scores_drug = Array_class.Deep_copy_array(this.RNA_integrity_scores_drug);
            copy.RNA_integrity_scores_control = Array_class.Deep_copy_array(this.RNA_integrity_scores_control);
            return copy;
        }
    }

    class Lincs_experimental_metadata_website_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Lincs_experimental_metadata_website_readWriteOptions_class(string subdirectory, string fileName)
        {
            string directory = Global_directory_class.Results_directory + subdirectory;
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Drug_name",  //2
                                                    "Is_cardiotoxic",
                                                    "DrugClass_for_website", //1
                                                    "Cellline_for_website",  //3
                                                    "Plate_for_website",  //4
                                                    "Sequencing_platform", //5
                                                    "Library_preparation_method", //6
                                                    "Stimulation_time_in_h",  //7
                                                    "Drug_concentration_plus_unit",  //8
                                                    "Drug_replicates_count_initial",  //9
                                                    "Control_replicates_count_initial",  //10
                                                    "Drug_replicates_count_after_computational_pipeline",  //11
                                                    "Control_replicates_count_after_computational_pipeline",  //12
                                                    };
            this.Key_columnNames = new string[]
            {
                Lincs_website_conversion_class.Label_drugName,  //2
                Lincs_website_conversion_class.Label_is_cardiotoxic,  //2
                Lincs_website_conversion_class.Label_drug_class,  //1
                Lincs_website_conversion_class.Label_cell_line,  //3
                Lincs_website_conversion_class.Label_plate,  //4
                "Sequencing platform",  //5
                "Library preparation", //6
                "Drug stimulation time [h]",  //7
                "Drug concentration",  //8
                "Drug replicates count",  //9
                "Control replicates count",  //10
                "Drug replicates count used for DEGs",  //11
                "Control replicates count used for DEGs",  //12
                //"Drug replicates RNA integrity scores",  //13
                //"Control replicates RNA integrity scores",  //14
                //"Date of experiment for drug treatment",
                //"Date of experiment for control treatment"
            };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }


    class Lincs_experimental_metadata_class
    {
        public Lincs_experimental_metadata_line_class[] Metadata_lines { get; set; }


        private void Generate_from_experimentalMetadata_input_data(string dataset)
        {
            Lincs_experimentalMetadata_input_class metadata_input = new Lincs_experimentalMetadata_input_class();
            metadata_input.Generate(dataset);
            Lincs_experimentalMetadata_input_line_class metadata_input_line;
            int metadata_input_length = metadata_input.Exp_metadata.Length;
            Lincs_experimental_metadata_line_class metadata_line;
            Lincs_experimental_metadata_line_class inner_metadata_line;
            List<Lincs_experimental_metadata_line_class> currentSeqRunCell_metadata_list = new List<Lincs_experimental_metadata_line_class>();
            int currentSeqRunCell_metadata_lengh;
            List<Lincs_experimental_metadata_line_class> final_metadata_list = new List<Lincs_experimental_metadata_line_class>();
            metadata_input.Exp_metadata = metadata_input.Exp_metadata.OrderBy(l => l.Sequencing_run).ThenBy(l => l.Cell_name).ThenBy(l => l.Drug_id).ToArray();

            List<float> control_integrity_scores = new List<float>();
            List<string> control_experimental_dates = new List<string>();
            int control_replicates_count = 0;
            List<float> drug_integrity_scores = new List<float>();
            List<string> drug_experimental_dates = new List<string>();
            int drug_replicates_count = 0;

            for (int indexInputM=0; indexInputM < metadata_input_length; indexInputM++)
            {
                metadata_input_line = metadata_input.Exp_metadata[indexInputM];
                if ((indexInputM == 0)
                    || (!metadata_input_line.Sequencing_run.Equals(metadata_input.Exp_metadata[indexInputM - 1].Sequencing_run))
                    || (!metadata_input_line.Cell_name.Equals(metadata_input.Exp_metadata[indexInputM - 1].Cell_name)))
                {
                    control_experimental_dates.Clear();
                    control_integrity_scores.Clear();
                    control_replicates_count = 0;
                    currentSeqRunCell_metadata_list.Clear();
                }
                if (  (indexInputM == 0)
                    || (!metadata_input_line.Sequencing_run.Equals(metadata_input.Exp_metadata[indexInputM - 1].Sequencing_run))
                    || (!metadata_input_line.Cell_name.Equals(metadata_input.Exp_metadata[indexInputM - 1].Cell_name))
                    || (!metadata_input_line.Drug_id.Equals(metadata_input.Exp_metadata[indexInputM - 1].Drug_id)))
                {
                    drug_experimental_dates.Clear();
                    drug_integrity_scores.Clear();
                    drug_replicates_count = 0;
                }
                if ((indexInputM != 0)
                    && (metadata_input_line.Sequencing_run.Equals(metadata_input.Exp_metadata[indexInputM - 1].Sequencing_run))
                    && (metadata_input_line.Cell_name.Equals(metadata_input.Exp_metadata[indexInputM - 1].Cell_name))
                    && (metadata_input_line.Drug_id.Equals(metadata_input.Exp_metadata[indexInputM - 1].Drug_id)))
                {
                    if (!metadata_input_line.Final_concentration.Equals(metadata_input.Exp_metadata[indexInputM - 1].Final_concentration))
                    {
                        throw new Exception();
                    }
                    if (!metadata_input_line.Final_concentration_unit.Equals(metadata_input.Exp_metadata[indexInputM - 1].Final_concentration_unit))
                    {
                        throw new Exception();
                    }
                    if (!metadata_input_line.Stimulation_time_in_h.Equals(metadata_input.Exp_metadata[indexInputM - 1].Stimulation_time_in_h))
                    {
                        throw new Exception();
                    }
                }

                if (metadata_input_line.Drug_id.Equals(Lincs_experimentalMetadata_input_line_class.Get_control_drug_id()))
                {
                    control_integrity_scores.Add(metadata_input_line.Rna_integrity_score);
                    control_experimental_dates.Add(metadata_input_line.Experiment_date);
                    control_replicates_count++;
                }
                else
                {
                    drug_integrity_scores.Add(metadata_input_line.Rna_integrity_score);
                    drug_experimental_dates.Add(metadata_input_line.Experiment_date);
                    drug_replicates_count++;

                    if ((indexInputM == metadata_input_length - 1)
                        || (!metadata_input_line.Sequencing_run.Equals(metadata_input.Exp_metadata[indexInputM + 1].Sequencing_run))
                        || (!metadata_input_line.Cell_name.Equals(metadata_input.Exp_metadata[indexInputM + 1].Cell_name))
                        || (!metadata_input_line.Drug_id.Equals(metadata_input.Exp_metadata[indexInputM + 1].Drug_id)))
                    {
                        metadata_line = new Lincs_experimental_metadata_line_class();
                        metadata_line.Cell_name = (string)metadata_input_line.Cell_name.Clone();
                        metadata_line.Drug_id = (string)metadata_input_line.Drug_id.Clone();
                        metadata_line.Drug_replicates_count_initial = drug_replicates_count;
                        metadata_line.Final_drug_concentration = metadata_input_line.Final_concentration;
                        metadata_line.Final_drug_concentration_unit = (string)metadata_input_line.Final_concentration_unit.Clone();
                        metadata_line.RNA_integrity_scores_drug = drug_integrity_scores.ToArray(); ;
                        metadata_line.Experimental_dates_drug = drug_experimental_dates.ToArray();
                        metadata_line.Sequencing_run = metadata_input_line.Sequencing_run;
                        metadata_line.Library_preparation_method = (string)metadata_input_line.Library_preparation.Clone();
                        metadata_line.Stimulation_time_in_h = metadata_input_line.Stimulation_time_in_h;

                        currentSeqRunCell_metadata_list.Add(metadata_line);
                    }
                }
                if ((indexInputM == metadata_input_length - 1)
                    || (!metadata_input_line.Sequencing_run.Equals(metadata_input.Exp_metadata[indexInputM + 1].Sequencing_run))
                    || (!metadata_input_line.Cell_name.Equals(metadata_input.Exp_metadata[indexInputM + 1].Cell_name)))
                {
                    currentSeqRunCell_metadata_lengh = currentSeqRunCell_metadata_list.Count;
                    for (int indexCurrent=0; indexCurrent<currentSeqRunCell_metadata_lengh; indexCurrent++)
                    {
                        inner_metadata_line = currentSeqRunCell_metadata_list[indexCurrent];
                        inner_metadata_line.RNA_integrity_scores_control = control_integrity_scores.ToArray();
                        inner_metadata_line.Experimental_dates_control = control_experimental_dates.ToArray();
                        inner_metadata_line.Control_replicates_count_initial = control_replicates_count;
                    }
                    final_metadata_list.AddRange(currentSeqRunCell_metadata_list);
                }
            }
            this.Metadata_lines = final_metadata_list.ToArray();
        }

        private void Add_drugClasses_and_drugNames()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            Deg_drug_legend_line_class drug_legend_line = new Deg_drug_legend_line_class();
            drug_legend.Generate_de_novo();
            drug_legend.Add_missing_cardiotoxicity_from_faers();
            drug_legend.Legend = drug_legend.Legend.OrderBy(l => l.Drug).ToArray();
            int legend_length = drug_legend.Legend.Length;
            int indexL = 0;

            this.Metadata_lines = this.Metadata_lines.OrderBy(l => l.Drug_id_without_plus_at_end).ToArray();
            int metadata_length = this.Metadata_lines.Length;
            Lincs_experimental_metadata_line_class metadata_line;
            int stringCompare = -2;

            for (int indexM=0; indexM<metadata_length; indexM++)
            {
                metadata_line = this.Metadata_lines[indexM];
                stringCompare = -2;
                while ((indexL<legend_length)&&(stringCompare<0))
                {
                    drug_legend_line = drug_legend.Legend[indexL];
                    stringCompare = drug_legend_line.Drug.CompareTo(metadata_line.Drug_id_without_plus_at_end);
                    if (stringCompare<0)
                    {
                        indexL++;
                    }
                }
                if (stringCompare!=0) { throw new Exception(); }
                metadata_line.Drug_name = (string)drug_legend_line.Full_name.Clone();
                metadata_line.Drug_type = drug_legend_line.Drug_type;
                metadata_line.Is_cardiotoxic = (string)drug_legend_line.Is_cardiotoxic.Clone();
            }
        }

        private void Replace_pmc_cell_names_by_cellline()
        {
            foreach (Lincs_experimental_metadata_line_class metadata_line in this.Metadata_lines)
            {
                metadata_line.Cell_name = metadata_line.Cell_name.Replace("PMC-", "Cell_line.");
            }
        }

        private void Remove_plate3()
        {
            List<Lincs_experimental_metadata_line_class> keep = new List<Lincs_experimental_metadata_line_class>();
            foreach (Lincs_experimental_metadata_line_class metadata_line in this.Metadata_lines)
            {
                if (metadata_line.Sequencing_run!=3)
                {
                    keep.Add(metadata_line);
                }
            }
            this.Metadata_lines = keep.ToArray();
        }

        public void Add_used_replicates_information(Deg_class deg)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> plateString_cellline_drug_replicates_dict = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
            int degs_length = deg.Degs_complete.Length;
            Deg_line_class deg_line;
            string plateString;
            for (int indexDeg=0; indexDeg<degs_length; indexDeg++)
            {
                deg_line = deg.Degs_complete[indexDeg];
                deg_line.Patient = deg_line.Patient.Replace("_0", "_");
                deg_line.Patient = deg_line.Patient.Replace("R", "");
                plateString = deg_line.Plate.Replace("Plate.", "");
                if (!plateString_cellline_drug_replicates_dict.ContainsKey(plateString))
                {
                    plateString_cellline_drug_replicates_dict.Add(plateString, new Dictionary<string, Dictionary<string, int>>());
                }
                if (!plateString_cellline_drug_replicates_dict[plateString].ContainsKey(deg_line.Patient))
                {
                    plateString_cellline_drug_replicates_dict[plateString].Add(deg_line.Patient, new Dictionary<string, int>());
                }
                if (!plateString_cellline_drug_replicates_dict[plateString][deg_line.Patient].ContainsKey(deg_line.Treatment))
                {
                    plateString_cellline_drug_replicates_dict[plateString][deg_line.Patient].Add(deg_line.Treatment, deg_line.Treatment_wells_count);
                }
                else if (!plateString_cellline_drug_replicates_dict[plateString][deg_line.Patient][deg_line.Treatment].Equals(deg_line.Treatment_wells_count))
                {
                    throw new Exception();
                }
                if (!plateString_cellline_drug_replicates_dict[plateString][deg_line.Patient].ContainsKey("CTRL"))
                {
                    plateString_cellline_drug_replicates_dict[plateString][deg_line.Patient].Add("CTRL", deg_line.Control_wells_count);
                }
                else if (!plateString_cellline_drug_replicates_dict[plateString][deg_line.Patient]["CTRL"].Equals(deg_line.Control_wells_count))
                {
                    throw new Exception();
                }
            }

            int metadata_length = this.Metadata_lines.Length;
            Lincs_experimental_metadata_line_class metadata_line;
            string sequencing_run_string;
            for (int indexM=0; indexM<metadata_length; indexM++)
            {
                metadata_line = this.Metadata_lines[indexM];
                metadata_line.Cell_name = metadata_line.Cell_name.Replace("-", "_");
                sequencing_run_string = metadata_line.Sequencing_run.ToString();
                if (metadata_line.Cell_name.IndexOf("Cell_line.")==-1)
                {
                    metadata_line.Cell_name = "Cell_line." + metadata_line.Cell_name;
                }
                if ((plateString_cellline_drug_replicates_dict.ContainsKey(sequencing_run_string))
                    && (plateString_cellline_drug_replicates_dict[sequencing_run_string].ContainsKey(metadata_line.Cell_name))
                    && (plateString_cellline_drug_replicates_dict[sequencing_run_string][metadata_line.Cell_name].ContainsKey(metadata_line.Drug_id)))
                {
                    metadata_line.Drug_replicates_count_after_computational_pipeline = plateString_cellline_drug_replicates_dict[sequencing_run_string][metadata_line.Cell_name][metadata_line.Drug_id];
                }
                else
                {
                    metadata_line.Drug_replicates_count_after_computational_pipeline = 0;
                //    throw new Exception();
                }
                if ((plateString_cellline_drug_replicates_dict.ContainsKey(sequencing_run_string))
                    && (plateString_cellline_drug_replicates_dict[sequencing_run_string].ContainsKey(metadata_line.Cell_name))
                    && (plateString_cellline_drug_replicates_dict[sequencing_run_string][metadata_line.Cell_name].ContainsKey("CTRL")))
                {
                    metadata_line.Control_replicates_count_after_computational_pipeline = plateString_cellline_drug_replicates_dict[sequencing_run_string][metadata_line.Cell_name]["CTRL"];
                }
                else
                {
                    metadata_line.Control_replicates_count_after_computational_pipeline = 0;
                  //  throw new Exception();
                }
            }
        }

        private void Add_properties_for_website(string cell_type)
        {
            foreach (Lincs_experimental_metadata_line_class metadata_line in Metadata_lines)
            {
                metadata_line.Cellline_for_website = Lincs_website_conversion_class.Get_cellLine_for_website(cell_type, metadata_line.Cell_name, metadata_line.Plate);
                metadata_line.DrugClass_for_website= Lincs_website_conversion_class.Get_drugClass_for_website(metadata_line.Drug_type);
                metadata_line.Plate_for_website = Lincs_website_conversion_class.Get_plate_for_website(metadata_line.Plate);
            }
        }

        public void Keep_only_selected_sequencing_run(int sequencing_run)
        {
            int metadata_length = this.Metadata_lines.Length;
            Lincs_experimental_metadata_line_class metadata_line;
            List<Lincs_experimental_metadata_line_class> keep = new List<Lincs_experimental_metadata_line_class>();
            for (int indexM = 0; indexM < metadata_length; indexM++)
            {
                metadata_line = this.Metadata_lines[indexM];
                if (metadata_line.Sequencing_run.Equals(sequencing_run))
                {
                    keep.Add(metadata_line);
                }
            }
            this.Metadata_lines = keep.ToArray();
        }

        public void Keep_only_selected_drugs(string[] selected_drugs)
        {
            Dictionary<string, bool> keep_drug_dict = new Dictionary<string, bool>();
            foreach (string selected_drug in selected_drugs)
            {
                keep_drug_dict.Add(selected_drug, false);
            }

            int metadata_length = this.Metadata_lines.Length;
            Lincs_experimental_metadata_line_class metadata_line;
            List<Lincs_experimental_metadata_line_class> keep = new List<Lincs_experimental_metadata_line_class>();
            for (int indexM = 0; indexM < metadata_length; indexM++)
            {
                metadata_line = this.Metadata_lines[indexM];
                if (keep_drug_dict.ContainsKey(metadata_line.Drug_id))
                {
                    keep.Add(metadata_line);
                    keep_drug_dict[metadata_line.Drug_id] = true;
                }
            }
            foreach (string selected_drug in selected_drugs)
            {
                if (!keep_drug_dict[selected_drug]) { throw new Exception(); }
            }
            this.Metadata_lines = keep.ToArray();
        }

        public void Generate(Deg_class degs)
        {
            string[] datasets = degs.Get_all_ordered_datasets();
            if (datasets.Length!=1) { throw new Exception(); }
            string dataset = datasets[0];
            Generate_from_experimentalMetadata_input_data(dataset);
            Replace_pmc_cell_names_by_cellline();
            Remove_plate3();
            Add_drugClasses_and_drugNames();
            Add_used_replicates_information(degs);
            string cell_type = degs.Get_cell_type_and_check_if_only_one();
            Add_properties_for_website(cell_type);
        }

        public void Write(string subdirectory, string fileName)
        {
            Lincs_experimental_metadata_website_readWriteOptions_class readWriteOptions = new Lincs_experimental_metadata_website_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(this.Metadata_lines, readWriteOptions);
        }
    }

}
