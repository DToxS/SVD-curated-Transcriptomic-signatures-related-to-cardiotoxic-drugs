using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common_classes;
using ReadWrite;
using Enrichment;

namespace Input_datasets
{
    class SVD_precision_summary_line_data
    {
        public string Side_effect { get; set; }
        public string Ontology { get; set; }
        public string Scp { get; set; }
        public string Association { get; set; }
        public DE_entry_enum Entry_type { get; set; }
        public float Cutoff_rank { get; set; }
        public float Selection_rank { get; set; }
        public string Selected { get; set; }
        public float Precision { get; set; }
        public float Recall { get; set; }
        public float ValueForSelection { get; set; }
        public string ValueTypeForSelection { get; set; }

        public SVD_precision_summary_line_data Deep_copy()
        {
            SVD_precision_summary_line_data copy = (SVD_precision_summary_line_data)this.MemberwiseClone();
            copy.Side_effect = (string)this.Side_effect.Clone();
            copy.Ontology = (string)this.Ontology.Clone();
            copy.Scp = (string)this.Scp.Clone();
            copy.Association = (string)this.Association.Clone();
            copy.Selected = (string)this.Selected.Clone();
            copy.ValueTypeForSelection = (string)this.ValueTypeForSelection.Clone();
            return copy;
        }
    }

    class SVD_precision_summary_readWriteOptions_class : ReadWriteOptions_base
    {
        public SVD_precision_summary_readWriteOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Side_effect", "Ontology", "Scp", "Association", "Entry_type", "Cutoff_rank", "Selection_rank", "Selected", "Precision", "Recall", "ValueForSelection", "ValueTypeForSelection" };
            this.Key_columnNames = this.Key_propertyNames;
            this.File_has_headline = true;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class SVD_precision_summary_class
    {
        public SVD_precision_summary_line_data[] Svd_summaries { get; set; }

        public Enrichment2018_results_class Generate_pseudo_enrichment_instance()
        {
            Enrichment2018_results_line_class enrichment_line;
            List<Enrichment2018_results_line_class> pseudo_enrichment_lines = new List<Enrichment2018_results_line_class>();
            int svd_summaries_length = Svd_summaries.Length;
            SVD_precision_summary_line_data svd_precision_line;
            for (int indexSVD = 0; indexSVD < svd_summaries_length; indexSVD++)
            {
                svd_precision_line = this.Svd_summaries[indexSVD];
                if (svd_precision_line.Selected.Equals("TRUE"))
                {
                    enrichment_line = new Enrichment2018_results_line_class();
                    enrichment_line.Scp = (string)svd_precision_line.Scp.Clone();
                    enrichment_line.Sample_name = (string)svd_precision_line.Association.Clone();
                    enrichment_line.Sample_entryType = svd_precision_line.Entry_type;
                    pseudo_enrichment_lines.Add(enrichment_line);
                }
            }
            Enrichment2018_results_class enrichment = new Enrichment2018_results_class();
            enrichment.Enrichment_results = pseudo_enrichment_lines.ToArray();
            return enrichment;
        }

        public void Generate_by_reading(string directory, string fileName)
        {
            Read(directory, fileName);
        }

        public void Read(string directory, string fileName)
        {
            SVD_precision_summary_readWriteOptions_class readWriteOptions = new SVD_precision_summary_readWriteOptions_class(directory, fileName);
            this.Svd_summaries = ReadWriteClass.ReadRawData_and_FillArray<SVD_precision_summary_line_data>(readWriteOptions);
        }
    }
}
