using System;
using System.Collections.Generic;
using System.Linq;
using Common_classes;
using Highthroughput_data;
using ReadWrite;
using Statistic;

namespace FAERS_analysis
{
    class FAERS_line_class
    {
        public string AdverseEvent { get; set; }
        public string Full_drug_name { get; set; }
        public Drug_type_enum Drug_type { get; set; }
        public string Date_of_approval { get; set; }
        public string Formulation { get; set; }
        public int Years_analyzed { get; set; }
        public int Total_count_adverse_events { get; set; }
        public int All_AE_count_for_drug_of_interest { get; set; }
        public int AE_of_interest_for_drug_of_interest { get; set; }
        public string Drug { get; set; }
        public float Odds_ratio { get; set; }
        public float Odds_ratio_rank { get; set; }
        public string Odds_ratio_rank_for_FDA
        {
            get { return Conversion_class.Convert_positive_float_to_string_or_nd(Odds_ratio_rank); }
        }
        public float Confidence_interval_upper { get; set; }
        public float Confidence_interval_lower { get; set; }
        public string Risk_frequency_clinical_studies { get; set; }
        public string Is_cardiotoxic { get; set; }

        public FAERS_line_class()
        {
            Risk_frequency_clinical_studies = "";
            Is_cardiotoxic = "";
        }

        public FAERS_line_class Deep_copy()
        {
            FAERS_line_class copy = (FAERS_line_class)this.MemberwiseClone();
            copy.AdverseEvent = (string)this.AdverseEvent.Clone();
            copy.Full_drug_name = (string)this.Full_drug_name.Clone();
            copy.Date_of_approval = (string)this.Date_of_approval.Clone();
            copy.Formulation = (string)this.Formulation.Clone();
            copy.Drug = (string)this.Drug.Clone();
            copy.Is_cardiotoxic = (string)this.Is_cardiotoxic.Clone();
            copy.Risk_frequency_clinical_studies = (string)this.Risk_frequency_clinical_studies.Clone();
            return copy;
        }
    }

    class FAERS_readInputOptions_class : ReadWriteOptions_base
    {
        public FAERS_readInputOptions_class()
        {
            this.File = Global_directory_class.Metadata_directory + "FAERS_risk_profiles.txt";
            this.Key_propertyNames = new string[] { "AdverseEvent", "Full_drug_name", "Date_of_approval", "Formulation", "Years_analyzed", "Total_count_adverse_events", "All_AE_count_for_drug_of_interest", "AE_of_interest_for_drug_of_interest", "Odds_ratio", "Confidence_interval_lower", "Confidence_interval_upper" };
            this.Key_columnNames = new string[] { "AdverseEvent", "Full_drug_name", "Date of Approval", "Formulation", "# of years analysed (approval-2020)", "Total # of edverse events (N)", "All AE for drug of interest (D)", "Drug of interest + AE of interest (DE)", "ROR", "CI_lower", "CI_upper" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class FAERS_writeFDAOptions_class : ReadWriteOptions_base
    {
        public FAERS_writeFDAOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Full_drug_name", "Is_cardiotoxic", "Drug_type", "Risk_frequency_clinical_studies", "Odds_ratio_rank_for_FDA", "Odds_ratio", "Date_of_approval", "Years_analyzed", "All_AE_count_for_drug_of_interest", "AE_of_interest_for_drug_of_interest", "Confidence_interval_lower", "Confidence_interval_upper" };
            this.Key_columnNames = new string[] { "Drug name", "Is cardiotoxic", "Drug class", "Risk frequency", "Odds ratio rank (FAERS)", "Odds ratio (FAERS)", "Date of approval", "# of years analysed",    "# all adverse events", "# cardiotoxic adverse events", "Lower confidence interval (FAERS)", "Upper confidence interval (FAERS)" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class FAERS_options_class
    {
        public string[] Adverse_events { get; set; }

        public FAERS_options_class()
        {
            this.Adverse_events = new string[] { "Cardiotoxicity" };
        }
    }

    class FAERS_class
    {
        public FAERS_line_class[] FAERS { get; set; }
        public FAERS_options_class Options { get; set; }

        public FAERS_class()
        {
            this.Options = new FAERS_options_class();
        }

        private void Add_drugs_from_full_drugNames_drugTypes_and_set_fullDrugNames_to_lowerCase()
        {
            Deg_drug_legend_class drug_legend = new Deg_drug_legend_class();
            drug_legend.Generate_de_novo();
            Dictionary<string, string> drugFullName_drug_dict = drug_legend.Get_drugFullName_drug_dict();
            Dictionary<string, Drug_type_enum> drug_drugType_dict = drug_legend.Get_drug_drugType_dictionary();
            foreach (FAERS_line_class faers_line in this.FAERS)
            {
                faers_line.Full_drug_name = faers_line.Full_drug_name.ToLower();
                if (drugFullName_drug_dict.ContainsKey(faers_line.Full_drug_name))
                {
                    faers_line.Drug = (string)drugFullName_drug_dict[faers_line.Full_drug_name].Clone();
                    faers_line.Drug_type = drug_drugType_dict[faers_line.Drug];
                }
                else 
                { 
                    faers_line.Drug = "Drug not found";
                }
            }
        }

        private void Keep_only_adverseEvents_of_interest()
        {
            Dictionary<string, bool> keep_adverseEvents_dict = new Dictionary<string, bool>();
            foreach (string adverseEvent in Options.Adverse_events)
            {
                keep_adverseEvents_dict.Add(adverseEvent, true);
            }
            List<FAERS_line_class> keep = new List<FAERS_line_class>();
            foreach (FAERS_line_class faers_line in this.FAERS)
            {
                if (keep_adverseEvents_dict.ContainsKey(faers_line.AdverseEvent))
                {
                    keep.Add(faers_line);
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.FAERS = keep.ToArray();
        }

        public void Keep_only_selected_drugs(string[] keep_drugs)
        {
            Dictionary<string, bool> keep_drugs_dict = new Dictionary<string, bool>();
            foreach (string drug in keep_drugs)
            {
                keep_drugs_dict.Add(drug, false);
            }
            List<FAERS_line_class> keep = new List<FAERS_line_class>();
            foreach (FAERS_line_class faers_line in this.FAERS)
            {
                if (keep_drugs_dict.ContainsKey(faers_line.Drug))
                {
                    faers_line.Odds_ratio_rank = -1;
                    keep.Add(faers_line);
                    keep_drugs_dict[faers_line.Drug] = true;
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.FAERS = keep.ToArray();
        }

        public void Keep_only_selected_drugTypes(params Drug_type_enum[] keep_drug_types)
        {
            Dictionary<Drug_type_enum, bool> keep_drugTypes_dict = new Dictionary<Drug_type_enum, bool>();
            foreach (Drug_type_enum drugType in keep_drug_types)
            {
                keep_drugTypes_dict.Add(drugType, false);
            }
            List<FAERS_line_class> keep = new List<FAERS_line_class>();
            foreach (FAERS_line_class faers_line in this.FAERS)
            {
                if (keep_drugTypes_dict.ContainsKey(faers_line.Drug_type))
                {
                    faers_line.Odds_ratio_rank = -1;
                    keep.Add(faers_line);
                    keep_drugTypes_dict[faers_line.Drug_type] = true;
                }
            }
            if (keep.Count == 0) { throw new Exception(); }
            this.FAERS = keep.ToArray();
        }

        public void Calculate_fractional_ranks_for_odds_ratios_considering_only_input_drug_types(params Drug_type_enum[] considered_drug_classes)
        {
            Dictionary<Drug_type_enum, bool> considered_drugType_dict = new Dictionary<Drug_type_enum, bool>();
            foreach (Drug_type_enum drugType in considered_drug_classes)
            {
                considered_drugType_dict.Add(drugType, true);
            }

            this.FAERS = this.FAERS.OrderByDescending(l => l.Odds_ratio).ToArray();
            List<float> current_ranks = new List<float>();
            float current_rank = -1;
            float final_rank;
            int firstIndexSameOddsRatio = -1;
            FAERS_line_class faers_line;
            int faers_length = this.FAERS.Length;
            current_rank = 0;
            for (int indexFAERS = 0; indexFAERS < faers_length; indexFAERS++)
            {
                faers_line = this.FAERS[indexFAERS];
                faers_line.Odds_ratio_rank = float.NaN;
                if (considered_drugType_dict.ContainsKey(faers_line.Drug_type))
                {
                    if ((indexFAERS == 0)
                        || (!faers_line.Drug.Equals(this.FAERS[indexFAERS - 1].Drug))
                        || (!faers_line.Drug_type.Equals(this.FAERS[indexFAERS + 1].Drug_type))
                        || (!faers_line.AdverseEvent.Equals(this.FAERS[indexFAERS - 1].AdverseEvent))
                        || (!faers_line.Odds_ratio.Equals(this.FAERS[indexFAERS - 1].Odds_ratio)))
                    {
                        current_ranks.Clear();
                        firstIndexSameOddsRatio = indexFAERS;
                    }
                    current_rank++;
                    current_ranks.Add(current_rank);
                    if ((indexFAERS == faers_length - 1)
                        || (!faers_line.Drug.Equals(this.FAERS[indexFAERS + 1].Drug))
                        || (!faers_line.Drug_type.Equals(this.FAERS[indexFAERS + 1].Drug_type))
                        || (!faers_line.AdverseEvent.Equals(this.FAERS[indexFAERS + 1].AdverseEvent))
                        || (!faers_line.Odds_ratio.Equals(this.FAERS[indexFAERS + 1].Odds_ratio)))
                    {
                        final_rank = Math_class.Get_average(current_ranks.ToArray());
                        for (int indexInner = firstIndexSameOddsRatio; indexInner <= indexFAERS; indexInner++)
                        {
                            this.FAERS[indexInner].Odds_ratio_rank = final_rank;
                        }
                    }
                }
            }
        }

        public void Generate_by_reading()
        {
            Read();
            Keep_only_adverseEvents_of_interest();
            Add_drugs_from_full_drugNames_drugTypes_and_set_fullDrugNames_to_lowerCase();
        }

        public Dictionary<string, float> Get_dictionary_with_drug_odds_ratio(string adverse_event)
        {
            Dictionary<string, float> drug_odds_ratio = new Dictionary<string, float>();
            foreach (FAERS_line_class faers_line in this.FAERS)
            {
                if (faers_line.AdverseEvent.Equals(adverse_event))

                {
                    if (!drug_odds_ratio.ContainsKey(faers_line.Drug))
                    { drug_odds_ratio.Add(faers_line.Drug, faers_line.Odds_ratio); }
                    else if (drug_odds_ratio[faers_line.Drug] != faers_line.Odds_ratio) { throw new Exception(); }
                }
            }
            return drug_odds_ratio;
        }

        public Dictionary<string, string> Get_dictionary_with_drug_drugApprovalData()
        {
            Dictionary<string, string> drug_drugApprovalDate_dict = new Dictionary<string, string>();
            foreach (FAERS_line_class faers_line in this.FAERS)
            {
                if (!drug_drugApprovalDate_dict.ContainsKey(faers_line.Drug))
                { drug_drugApprovalDate_dict.Add(faers_line.Drug, faers_line.Date_of_approval); }
                else if (drug_drugApprovalDate_dict[faers_line.Drug] != faers_line.Date_of_approval) { throw new Exception(); }
            }
            return drug_drugApprovalDate_dict;
        }

        public void Add_clinical_studies_risk_frequency(Deg_drug_legend_class drug_legend)
        {
            drug_legend.Legend = drug_legend.Legend.OrderBy(l => l.Drug).ToArray();
            Dictionary<string, float> drug_frequencyGroup_dict = drug_legend.Get_drug_frequencyGroup_dict();
            Dictionary<string, string> drug_isCardiotoxic_dict = drug_legend.Get_drug_isCardiotoxic_dictionary();
            foreach (FAERS_line_class faers_line in this.FAERS)
            {
                if (drug_frequencyGroup_dict.ContainsKey(faers_line.Drug))
                {
                    if (drug_frequencyGroup_dict[faers_line.Drug] == 0)
                    {
                        faers_line.Risk_frequency_clinical_studies = "No risk reported";
                    }
                    else if (drug_frequencyGroup_dict[faers_line.Drug] == 1)
                    {
                        faers_line.Risk_frequency_clinical_studies = "<1%";
                    }
                    else if (drug_frequencyGroup_dict[faers_line.Drug] == 2)
                    {
                        faers_line.Risk_frequency_clinical_studies = "1-10%";
                    }
                    else if (drug_frequencyGroup_dict[faers_line.Drug] == 3)
                    {
                        faers_line.Risk_frequency_clinical_studies = ">10%";
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    faers_line.Risk_frequency_clinical_studies = "";
                }
                if (drug_isCardiotoxic_dict.ContainsKey(faers_line.Drug))
                {
                    faers_line.Is_cardiotoxic = (string)drug_isCardiotoxic_dict[faers_line.Drug].Clone();
                }
            }
        }

        private void Read()
        {
            FAERS_readInputOptions_class readOptions = new FAERS_readInputOptions_class();
            this.FAERS = ReadWriteClass.ReadRawData_and_FillArray<FAERS_line_class>(readOptions);
        }

        private string[] Get_adverse_events()
        {
            Dictionary<string, bool> adverseEvent_dict = new Dictionary<string, bool>();
            foreach (FAERS_line_class faers_line in this.FAERS)
            {
                if (!adverseEvent_dict.ContainsKey(faers_line.AdverseEvent))
                {
                    adverseEvent_dict.Add(faers_line.AdverseEvent, true);
                }
            }
            return adverseEvent_dict.Keys.OrderBy(l => l).ToArray();
        }

        public void Write_for_fda(string directory, string filename)
        {
            string[] adverseEvents = Get_adverse_events();
            if (adverseEvents.Length != 1) { throw new Exception(); }
            filename = System.IO.Path.GetFileNameWithoutExtension(filename) + "_" + adverseEvents[0] + System.IO.Path.GetExtension(filename);
            FAERS_writeFDAOptions_class writeOptions = new FAERS_writeFDAOptions_class(directory, filename);
            ReadWriteClass.WriteData(this.FAERS, writeOptions);
        }
    }
}
