using System;
using System.Collections.Generic;
using System.Linq;
using Common_classes;
using ReadWrite;
using Gene_databases;
using Ontologies_and_GTEx;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Enrichment
{
    class ScRNAseq_marker_gene_line_class
    {
        public int Cluster { get; set; }
        public string Cell_type { get; set; }
        public string Gene { get; set; }
        public int Marker { get; set; }

        public ScRNAseq_marker_gene_line_class Deep_copy()
        {
            ScRNAseq_marker_gene_line_class copy = (ScRNAseq_marker_gene_line_class)this.MemberwiseClone();
            copy.Cell_type = (string)this.Cell_type.Clone();
            copy.Gene = (string)this.Gene.Clone();
            return copy;
        }
    }
    class ScRNAseq_marker_gene_readWriteOptions_class : ReadWriteOptions_base
    {
        public ScRNAseq_marker_gene_readWriteOptions_class(Ontology_type_enum ontology)
        {
            switch (ontology)
            {
                case Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes:
                    this.File = Global_directory_class.Enrich_libraries_download_directory + "Asp_2019_PMID_31835037_heartDevelopmentalCells.txt";
                    this.Key_propertyNames = new string[] { "Cluster", "Gene" };
                    this.Key_columnNames = new string[] { "cluster", "gene" };
                    break;
                case Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes:
                    this.File = Global_directory_class.Enrich_libraries_download_directory + "Tucker_2020_marker_genes_PMID 32403949 _suppl_table4.txt";
                    this.Key_propertyNames = new string[] { "Cell_type", "Gene", "Marker" };
                    this.Key_columnNames = new string[] { "Cell Type", "Gene", "Marker" };
                    break;
                default:
                    throw new Exception();
            }
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }
    class ScRNAseq_marker_gene_class
    {
        public ScRNAseq_marker_gene_line_class[] Marker_genes { get; set; }

        private void Add_cell_types_for_asp(Ontology_type_enum ontology)
        {
            if (!ontology.Equals(Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes)) { throw new Exception(); }
            Dictionary<int, string> cluster_cellType_dict = new Dictionary<int, string>();
            cluster_cellType_dict.Add(0, "Capillary endothelium");
            cluster_cellType_dict.Add(1, "Ventricular cardiomyocytes");
            cluster_cellType_dict.Add(2, "Fibroblast - like(related to cardiac skeleton connective tissue)");
            cluster_cellType_dict.Add(3, "Epicardium - derived cells");
            cluster_cellType_dict.Add(4, "Fibroblast - like(related to smaller vascular development)");
            cluster_cellType_dict.Add(5, "Smooth muscle cells / fibroblast - like");
            cluster_cellType_dict.Add(6, "Erythrocytes");
            cluster_cellType_dict.Add(7, "Atrial cardiomyocytes");
            cluster_cellType_dict.Add(8, "Fibroblast - like(related to larger vascular development)");
            cluster_cellType_dict.Add(9, "Epicardial cells");
            cluster_cellType_dict.Add(10, "Endothelium / pericytes / adventitia");
            cluster_cellType_dict.Add(11, "Erythrocytes");
            cluster_cellType_dict.Add(12, "Myoz2 - enriched cardiomyocytes");
            cluster_cellType_dict.Add(13, "Immune cells");
            cluster_cellType_dict.Add(14, "Cardiac neural crest cells &Schwann progenitor cells");
            foreach (ScRNAseq_marker_gene_line_class marker_gene_line in this.Marker_genes)
            {
                marker_gene_line.Cell_type = cluster_cellType_dict[marker_gene_line.Cluster];
            }
        }

        private void Keep_only_lines_with_marker_equals_1(Ontology_type_enum ontology)
        {
            if (!ontology.Equals(Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes)) { throw new Exception(); }
            List<ScRNAseq_marker_gene_line_class> keep = new List<ScRNAseq_marker_gene_line_class>();
            foreach (ScRNAseq_marker_gene_line_class scRNA_seq_line in this.Marker_genes)
            {
                if (scRNA_seq_line.Marker==1)
                {
                    keep.Add(scRNA_seq_line);
                }
            }
            this.Marker_genes = keep.ToArray();
        }

        public void Generate_by_reading(Ontology_type_enum ontology)
        {
            Read(ontology);
            switch (ontology)
            {
                case Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes:
                    Add_cell_types_for_asp(ontology);
                    break;
                case Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes:
                    Keep_only_lines_with_marker_equals_1(ontology);
                    break;
                default:
                    throw new Exception();
            }
        }
        private void Read(Ontology_type_enum ontology)
        {
            ScRNAseq_marker_gene_readWriteOptions_class readWriteOptions = new ScRNAseq_marker_gene_readWriteOptions_class(ontology);
            this.Marker_genes = ReadWriteClass.ReadRawData_and_FillArray<ScRNAseq_marker_gene_line_class>(readWriteOptions);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Ontology_library_line_class
    {
        public int Level { get; set; }
        public string Scp { get; set; }
        public string Target_gene_symbol { get; set; }
        public string Organism_string { get; set; }

        public string Additional_information { get; set; }
        public float Target_gene_score { get; set; }
        public Organism_enum Organism { get; set; }
        public Organism_enum Organism_initial { get; set; }

        public Ontology_type_enum Ontology { get; set; }
        public string[] References { get; set; }
        public string ReadWrite_references
        {
            get { return ReadWriteClass.Get_writeLine_from_array(References, Ontology_libary_readWriteOptions_class.Array_delimiter); }
            set {  References = ReadWriteClass.Get_array_from_readLine<string>(value, Ontology_libary_readWriteOptions_class.Array_delimiter); }
        }

        public static bool Check_if_ordered_correctly { get { return Global_class.Check_ordering; } }


        public Ontology_library_line_class()
        {
            this.Level = -1;
            this.Scp = "";
            this.Target_gene_symbol = "";
            this.Organism_string = "";
            this.Additional_information = "";
            this.Target_gene_score = -1;
            this.References = new string[0];
        }

        #region Order
        public static Ontology_library_line_class[] Order_by_scp_targetGeneSymbol(Ontology_library_line_class[] lines)
        {
            Dictionary<string, Dictionary<string, List<Ontology_library_line_class>>> scp_targetGeneSymbol_dict = new Dictionary<string, Dictionary<string, List<Ontology_library_line_class>>>();
            Dictionary<string, List<Ontology_library_line_class>> targetGeneSymbol_dict = new Dictionary<string, List<Ontology_library_line_class>>();
            int lines_length = lines.Length;
            Ontology_library_line_class library_line;
            for (int indexL = 0; indexL < lines_length; indexL++)
            {
                library_line = lines[indexL];
                if (!scp_targetGeneSymbol_dict.ContainsKey(library_line.Scp))
                {
                    scp_targetGeneSymbol_dict.Add(library_line.Scp, new Dictionary<string, List<Ontology_library_line_class>>());
                }
                if (!scp_targetGeneSymbol_dict[library_line.Scp].ContainsKey(library_line.Target_gene_symbol))
                {
                    scp_targetGeneSymbol_dict[library_line.Scp].Add(library_line.Target_gene_symbol, new List<Ontology_library_line_class>());
                }
                scp_targetGeneSymbol_dict[library_line.Scp][library_line.Target_gene_symbol].Add(library_line);
            }

            string[] scps = scp_targetGeneSymbol_dict.Keys.ToArray();
            string scp;
            int scps_length = scps.Length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            scps = scps.OrderBy(l => l).ToArray();
            List<Ontology_library_line_class> ordered_lines = new List<Ontology_library_line_class>();
            for (int indexScp = 0; indexScp < scps_length; indexScp++)
            {
                scp = scps[indexScp];
                targetGeneSymbol_dict = scp_targetGeneSymbol_dict[scp];
                geneSymbols = targetGeneSymbol_dict.Keys.ToArray();
                geneSymbols_length = geneSymbols.Length;
                geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
                {
                    geneSymbol = geneSymbols[indexGS];
                    ordered_lines.AddRange(targetGeneSymbol_dict[geneSymbol]);
                }
            }

            if (Check_if_ordered_correctly)
            {
                #region Check if ordered correctly
                int ordered_length = ordered_lines.Count;
                Ontology_library_line_class previous_line;
                Ontology_library_line_class current_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_lines[indexO - 1];
                    current_line = ordered_lines[indexO];
                    if ((current_line.Scp.CompareTo(previous_line.Scp) < 0)) { throw new Exception(); }
                    if ((current_line.Scp.Equals(previous_line.Scp))
                        && (current_line.Target_gene_symbol.CompareTo(previous_line.Target_gene_symbol) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_lines.ToArray();
        }

        public static Ontology_library_line_class[] Order_by_targetGeneSymbol_scp(Ontology_library_line_class[] lines)
        {
            Dictionary<string, Dictionary<string, List<Ontology_library_line_class>>> targetGeneSymbol_scp_dict = new Dictionary<string, Dictionary<string, List<Ontology_library_line_class>>>();
            Dictionary<string, List<Ontology_library_line_class>> scp_dict = new Dictionary<string, List<Ontology_library_line_class>>();
            int lines_length = lines.Length;
            Ontology_library_line_class library_line;
            for (int indexL = 0; indexL < lines_length; indexL++)
            {
                library_line = lines[indexL];
                if (!targetGeneSymbol_scp_dict.ContainsKey(library_line.Target_gene_symbol))
                {
                    targetGeneSymbol_scp_dict.Add(library_line.Target_gene_symbol, new Dictionary<string, List<Ontology_library_line_class>>());
                }
                if (!targetGeneSymbol_scp_dict[library_line.Target_gene_symbol].ContainsKey(library_line.Scp))
                {
                    targetGeneSymbol_scp_dict[library_line.Target_gene_symbol].Add(library_line.Scp, new List<Ontology_library_line_class>());
                }
                targetGeneSymbol_scp_dict[library_line.Target_gene_symbol][library_line.Scp].Add(library_line);
            }

            string[] geneSymbols = targetGeneSymbol_scp_dict.Keys.ToArray();
            string geneSymbol;
            int geneSymbols_length = geneSymbols.Length; ;
            string[] scps;
            string scp;
            int scps_length;
            geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
            List<Ontology_library_line_class> ordered_lines = new List<Ontology_library_line_class>();
            for (int indexGS = 0; indexGS < geneSymbols_length; indexGS++)
            {
                geneSymbol = geneSymbols[indexGS];
                scp_dict = targetGeneSymbol_scp_dict[geneSymbol];
                scps = scp_dict.Keys.ToArray();
                scps_length = scps.Length;
                scps = scps.OrderBy(l => l).ToArray();
                for (int indexScp = 0; indexScp < scps_length; indexScp++)
                {
                    scp = scps[indexScp];
                    ordered_lines.AddRange(scp_dict[scp]);
                }
            }

            if (Check_if_ordered_correctly)
            {
                #region Check if ordered correctly
                int ordered_length = ordered_lines.Count;
                Ontology_library_line_class previous_line;
                Ontology_library_line_class current_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_lines[indexO - 1];
                    current_line = ordered_lines[indexO];
                    if ((current_line.Target_gene_symbol.CompareTo(previous_line.Target_gene_symbol) < 0)) { throw new Exception(); }
                    if ((current_line.Target_gene_symbol.Equals(previous_line.Target_gene_symbol))
                        && (current_line.Scp.CompareTo(previous_line.Scp) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_lines.ToArray();
        }


        #endregion

        public Ontology_library_line_class Deep_copy()
        {
            Ontology_library_line_class copy = (Ontology_library_line_class)this.MemberwiseClone();
            copy.Target_gene_symbol = (string)this.Target_gene_symbol.Clone();
            copy.Organism_string = (string)this.Organism_string.Clone();
            copy.Scp = (string)this.Scp.Clone();
            copy.Additional_information = (string)this.Additional_information.Clone();
            copy.References = Array_class.Deep_copy_string_array(this.References);
            return copy;
        }
    }

    class Ontology_libary_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public Ontology_libary_readWriteOptions_class(Ontology_type_enum ontology, Organism_enum organism)
        {
            this.File = Global_directory_class.Enrich_libraries_self_directory + ontology + "_" + organism + ".txt";
            this.Key_propertyNames = new string[] { "Ontology", "Scp", "Target_gene_symbol", "Organism", "ReadWrite_references" };
            this.Key_propertyNames = new string[] { "Ontology", "Scp", "Target_gene_symbol" };
            this.Key_columnNames = this.Key_propertyNames;
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Ontology_libary_resultsDirectory_readWriteOptions_class : ReadWriteOptions_base
    {
        public Ontology_libary_resultsDirectory_readWriteOptions_class(string subdirectory, string fileName)
        {
            this.File = Global_directory_class.Results_directory + subdirectory + fileName;
            this.Key_propertyNames = new string[] { "Ontology", "Level", "Scp", "Target_gene_symbol", "Organism_string", "Target_gene_score", "Organism", "Additional_information" };
            this.Key_propertyNames = new string[] { "Ontology", "Scp", "Target_gene_symbol", "Organism", "ReadWrite_references" };
            this.Key_columnNames = this.Key_propertyNames;
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Ontology_library_mbco_readOptions_class : ReadWriteOptions_base
    {
        public Ontology_library_mbco_readOptions_class(Ontology_type_enum ontology)
        {
            switch (ontology)
            {
                case Ontology_type_enum.Molecular_biology_of_the_cell:
                    this.File = Global_directory_class.Enrich_libraries_download_directory + "MBCO_v1.1_gene-SCP_associations_human.txt";
                    break;
                default:
                    throw new Exception();
            }
            this.Key_propertyNames = new string[] { "Level", "Scp", "Target_gene_symbol" };
            this.Key_columnNames = new string[] { "ProcessLevel", "ProcessName", "Symbol" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class Ontology_library_generation_class : IDisposable
    {
        public Ontology_library_line_class[] Library { get; set; }
        public Homology_class Homology_mouse_to_human { get; set; }
        public Homology_class Homology_rat_to_human { get; set; }

        public Ontology_library_generation_class()
        {
            Homology_mouse_to_human = new Homology_class(new Organism_enum[] { Organism_enum.Mus_musculus, Organism_enum.Homo_sapiens });
            Homology_mouse_to_human.Generate_by_reading_file();
            Homology_rat_to_human = new Homology_class(new Organism_enum[] { Organism_enum.Rattus_norvegicus, Organism_enum.Homo_sapiens });
            Homology_rat_to_human.Generate_by_reading_file();
            this.Library = new Ontology_library_line_class[0];
        }

        public void Remove_duplicates()
        {
            int library_length = this.Library.Length;
            Ontology_library_line_class onto_line;
            List<Ontology_library_line_class> keep = new List<Ontology_library_line_class>();
            this.Library = this.Library.OrderBy(l => l.Organism).ThenBy(l => l.Scp).ThenBy(l => l.Target_gene_symbol).ToArray();
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                onto_line = this.Library[indexL];
                if ((indexL == 0)
                    || (!onto_line.Scp.Equals(this.Library[indexL - 1].Scp))
                    || (!onto_line.Target_gene_symbol.Equals(this.Library[indexL - 1].Target_gene_symbol))
                    || (!onto_line.Organism.Equals(this.Library[indexL - 1].Organism)))
                {
                    keep.Add(onto_line);
                }
            }
            this.Library = keep.ToArray();
        }
        public void Check_for_duplicates()
        {
            int library_length = this.Library.Length;
            Ontology_library_line_class onto_line;
            this.Library = this.Library.OrderBy(l=>l.Organism).ThenBy(l => l.Scp).ThenBy(l => l.Target_gene_symbol).ToArray();
            for (int indexL=0; indexL<library_length;indexL++)
            {
                onto_line = this.Library[indexL];
                if (  (indexL!=0)
                    && (onto_line.Scp.Equals(this.Library[indexL - 1].Scp))
                    && (onto_line.Target_gene_symbol.Equals(this.Library[indexL - 1].Target_gene_symbol))
                    && (onto_line.Organism.Equals(this.Library[indexL - 1].Organism)))
                {
                    throw new Exception();
                }
            }
        }
        public void Remove_spaces_at_beginning_or_end_of_scp_and_gene_symbol()
        {
            int library_length = this.Library.Length;
            Ontology_library_line_class onto_line;
            this.Library = this.Library.OrderBy(l => l.Organism).ThenBy(l => l.Scp).ThenBy(l => l.Target_gene_symbol).ToArray();
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                onto_line = this.Library[indexL];
                onto_line.Scp = Text2_class.Remove_space_comma_semicolon_colon_underline_from_end_and_beginning_of_text(onto_line.Scp);
                onto_line.Target_gene_symbol = Text2_class.Remove_space_comma_semicolon_colon_underline_from_end_and_beginning_of_text(onto_line.Target_gene_symbol);
                if (onto_line.Scp[0].Equals(' ')) { throw new Exception(); }
                if (onto_line.Scp[onto_line.Scp.Length - 1].Equals(' ')) { throw new Exception(); }
                if (onto_line.Target_gene_symbol[0].Equals(' ')) { throw new Exception(); }
                if (onto_line.Target_gene_symbol[onto_line.Target_gene_symbol.Length - 1].Equals(' ')) { throw new Exception(); }
            }
        }
        public void Check_if_all_values_assigned()
        {
            foreach (Ontology_library_line_class library_line in this.Library)
            {
                if (library_line.Ontology.Equals(Organism_enum.E_m_p_t_y)) { throw new Exception(); }
                if (library_line.Organism.Equals(Organism_enum.E_m_p_t_y)) { throw new Exception(); }
                if (String.IsNullOrEmpty(library_line.Scp)) { throw new Exception(); }
                if (String.IsNullOrEmpty(library_line.Target_gene_symbol)) { throw new Exception(); }
            }
        }
        public void Add_to_array(Ontology_library_line_class[] add_library)
        {
            int this_length = this.Library.Length;
            int add_length = add_library.Length;
            int new_length = this_length + add_length;
            Ontology_library_line_class[] new_library = new Ontology_library_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_library[indexNew] = this.Library[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_library[indexNew] = add_library[indexAdd];
            }
            this.Library = new_library;
        }
        private Ontology_type_enum Get_ontology_and_check_if_only_one(Ontology_library_line_class[] library)
        {
            Ontology_type_enum ontology = library[0].Ontology;
            foreach (Ontology_library_line_class library_line in library)
            {
                if (!library_line.Ontology.Equals(ontology))
                {
                    throw new Exception();
                }
            }
            return ontology;
        }
        private void Set_human_as_default_organism()
        {
            foreach (Ontology_library_line_class library_line in Library)
            {
                if ((!library_line.Organism.Equals(Organism_enum.Homo_sapiens)) && (!library_line.Organism.Equals(Organism_enum.E_m_p_t_y))) { throw new Exception(); }
                library_line.Organism = Organism_enum.Homo_sapiens;
                library_line.Organism_initial = library_line.Organism;
                library_line.Organism_string = "Human as default - no info available";
            }
        }
        private void Set_organism_and_organism_initial_if_english_and_separated_from_scp_name_by_brackets()
        {
            int library_length = this.Library.Length;
            Ontology_library_line_class libary_line;
            int indexHuman;
            int indexMouse;
            int indexRat;
            bool organism_assigned = false;
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                libary_line = this.Library[indexL];
                organism_assigned = false;

                #region Identify index of organism label
                indexHuman = -1;
                if (indexHuman == -1)
                {
                    indexHuman = libary_line.Scp.IndexOf(" (Human)");
                }
                if (indexHuman == -1)
                {
                    indexHuman = libary_line.Scp.IndexOf(" (human)");
                }
                indexMouse = -1;
                if (indexMouse == -1)
                {
                    indexMouse = libary_line.Scp.IndexOf(" (Mouse)");
                }
                if (indexMouse == -1)
                {
                    indexMouse = libary_line.Scp.IndexOf(" (mouse)");
                }
                indexRat = -1;
                if (indexRat == -1)
                {
                    indexRat = libary_line.Scp.IndexOf(" (Rat)");
                }
                if (indexRat == -1)
                {
                    indexRat = libary_line.Scp.IndexOf(" (rat)");
                }
                #endregion

                if (indexHuman != -1)
                {
                    if (organism_assigned) { throw new Exception(); }
                    if ((!libary_line.Organism.Equals(Organism_enum.E_m_p_t_y)) && (!libary_line.Organism.Equals(Organism_enum.Homo_sapiens))) { throw new Exception(); }
                    libary_line.Organism = Organism_enum.Homo_sapiens;
                    organism_assigned = true;
                }
                if (indexMouse != -1)
                {
                    if (organism_assigned) { throw new Exception(); }
                    if ((!libary_line.Organism.Equals(Organism_enum.E_m_p_t_y)) && (!libary_line.Organism.Equals(Organism_enum.Mus_musculus))) { throw new Exception(); }
                    libary_line.Organism = Organism_enum.Mus_musculus;
                    organism_assigned = true;
                }
                libary_line.Organism_initial = libary_line.Organism;
                if (!organism_assigned)
                {
                    throw new Exception();
                }
            }
        }
        private void Set_organism_and_organism_initial_if_english_and_separated_from_scp_name_by_space_or_underline()
        {
            int library_length = this.Library.Length;
            Ontology_library_line_class libary_line;
            int indexHuman;
            int indexMouse;
            int indexRat;
            int indexHuman_final;
            int indexMouse_final;
            int indexRat_final;
            bool organism_assigned = false;
            string organism_string;
            string final_organism_string = "error";
            List<string> notAssigned_scps = new List<string>();
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                libary_line = this.Library[indexL];
                organism_assigned = false;
                indexHuman_final = -1;
                indexMouse_final = -1;
                indexRat_final = -1;

                #region Identify index of organism label
                organism_string = " Human";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = "_Human";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = " HUMAN";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = " human";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = "_21211035_ChIP-Seq_LN229_Gbm";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = " 21211035 ChIP-Seq LN229 Gbm"; //LN229 is a human glioblastoma cell line, https://altogenlabs.com/xenograft-models/brain-cancer-xenograft/ln-229-xenograft-model/#:~:text=LN229%20is%20a%20human%20glioblastoma%20cell%20line%20commonly,of%20brain%20tumors%20and%20test%20potential%20cancer%20therapies.
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = "_K562_Hela";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = " K562 Hela";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = "_human";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = " hg19";
                indexHuman = libary_line.Scp.IndexOf(organism_string);
                if (indexHuman != -1)
                {
                    indexHuman_final = indexHuman;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = " Mouse";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.Clone();
                }
                organism_string = " MOUSE";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper();
                }
                organism_string = " FMOUSE";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = "_Mouse";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = " mouse";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = "_mouse";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = "_20523734_ChIP-Seq_CORTICAL_Neurons";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = " 20523734 ChIP-Seq CORTICAL Neurons";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = "_21621532_ChIP-ChIP_FETAL_Ovary";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = " 21621532 ChIP-ChIP FETAL Ovary";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = " mm9";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = " mm10";
                indexMouse = libary_line.Scp.IndexOf(organism_string);
                if (indexMouse != -1)
                {
                    indexMouse_final = indexMouse;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = " Rat";
                indexRat = libary_line.Scp.IndexOf(organism_string);
                if (indexRat != -1)
                {
                    indexRat_final = indexRat;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = "_Rat";
                indexRat = libary_line.Scp.IndexOf(organism_string);
                if (indexRat != -1)
                {
                    indexRat_final = indexRat;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = " rat";
                indexRat = libary_line.Scp.IndexOf(organism_string);
                if (indexRat != -1)
                {
                    indexRat_final = indexRat;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = "_rat";
                indexRat = libary_line.Scp.IndexOf(organism_string);
                if (indexRat != -1)
                {
                    indexRat_final = indexRat;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                organism_string = " 34362910 ChIP-Seq WistarRat Hippocampus";
                indexRat = libary_line.Scp.IndexOf(organism_string);
                if (indexRat != -1)
                {
                    indexRat_final = indexRat;
                    final_organism_string = (string)organism_string.ToUpper().Clone();
                }
                #endregion

                if (indexHuman_final != -1)
                {
                    if (indexMouse_final != -1) { throw new Exception(); }
                    if (indexRat_final != -1) { throw new Exception(); }
                    if (organism_assigned) { throw new Exception(); }
                    if ((!libary_line.Organism.Equals(Organism_enum.E_m_p_t_y)) && (!libary_line.Organism.Equals(Organism_enum.Homo_sapiens))) { throw new Exception(); }
                    libary_line.Organism_string = (string)final_organism_string.Clone();
                    libary_line.Organism = Organism_enum.Homo_sapiens;
                    organism_assigned = true;
                }
                if (indexMouse_final != -1)
                {
                    if (indexHuman_final != -1) { throw new Exception(); }
                    if (indexRat_final != -1) { throw new Exception(); }
                    if (organism_assigned) { throw new Exception(); }
                    if ((!libary_line.Organism.Equals(Organism_enum.E_m_p_t_y)) && (!libary_line.Organism.Equals(Organism_enum.Mus_musculus))) { throw new Exception(); }
                    libary_line.Organism_string = (string)final_organism_string.Clone();
                    libary_line.Organism = Organism_enum.Mus_musculus;
                    organism_assigned = true;
                }
                if (indexRat_final != -1)
                {
                    if (indexHuman_final != -1) { throw new Exception(); }
                    if (indexMouse_final != -1) { throw new Exception(); }
                    if (organism_assigned) { throw new Exception(); }
                    if ((!libary_line.Organism.Equals(Organism_enum.E_m_p_t_y)) && (!libary_line.Organism.Equals(Organism_enum.Mus_musculus))) { throw new Exception(); }
                    libary_line.Organism_string = (string)final_organism_string.Clone();
                    libary_line.Organism = Organism_enum.Rattus_norvegicus;
                    organism_assigned = true;
                }
                libary_line.Organism_initial = libary_line.Organism;
                if (!organism_assigned)
                {
                    notAssigned_scps.Add(libary_line.Scp);
                }
            }
            notAssigned_scps = notAssigned_scps.Distinct().OrderBy(l => l).ToList();
            if (notAssigned_scps.Count > 0)
            {
                ReadWriteClass.WriteArray_into_directory(notAssigned_scps.ToArray(), Global_directory_class.Results_directory, "No_organism_assigned.txt");
                throw new Exception();
            }
        }
        private void Set_organism(Ontology_type_enum ontology)
        {
            Report_class.WriteLine("{0}: {1}: Set organism", typeof(Ontology_library_generation_class).Name, ontology);
            switch (ontology)
            {
                case Ontology_type_enum.Kea_2015:
                case Ontology_type_enum.Molecular_biology_of_the_cell:
                case Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes:
                case Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes:
                    Set_human_as_default_organism();
                    break;
                case Ontology_type_enum.Transfac_and_jaspar_pwms:
                    Set_organism_and_organism_initial_if_english_and_separated_from_scp_name_by_brackets();
                    break;
                case Ontology_type_enum.Encode_tf_chip_seq_2015:
                case Ontology_type_enum.Chea_2016:
                case Ontology_type_enum.Chea_2022:
                case Ontology_type_enum.Trrust_transcription_factors_2019:
                    Set_organism_and_organism_initial_if_english_and_separated_from_scp_name_by_space_or_underline();
                    break;
                case Ontology_type_enum.Drugbank_drug_targets:
                case Ontology_type_enum.Drugbank_enzymes:
                case Ontology_type_enum.Drugbank_transporters:
                    break;
                default:
                    throw new Exception();
            }
        }
        private void Remove_experiment_information_from_SCP_names_separated_by_space_or_underline_if_TF_or_kinase_library(Ontology_type_enum ontology)
        {
            switch (ontology)
            {
                case Ontology_type_enum.Trrust_transcription_factors_2019:
                case Ontology_type_enum.Chea_2016:
                case Ontology_type_enum.Chea_2022:
                case Ontology_type_enum.Kea_2015:
                case Ontology_type_enum.Transfac_and_jaspar_pwms:
                case Ontology_type_enum.Encode_tf_chip_seq_2015:
                    break;
                default:
                    throw new Exception();
            }
            Ontology_library_line_class library_line;
            int ontology_library_length = this.Library.Length;
            for (int indexOL=0; indexOL<ontology_library_length;indexOL++)
            {
                library_line = this.Library[indexOL];
                library_line.Scp = library_line.Scp.Split(' ', '_')[0];
                if (String.IsNullOrEmpty(library_line.Scp)) { throw new Exception(); }
            }
        }
        private void Set_orthologes_for_species0_scps_based_to_species1_scps_if_organism_matches(Homology_class homology_species0_to_species1, Ontology_library_line_class[] current_library_lines)
        {
            Ontology_type_enum ontology = Get_ontology_and_check_if_only_one(current_library_lines);
            Report_class.WriteLine("{0}: {1}: Replace SCP genes of {2} by {3}", typeof(Ontology_library_generation_class).Name, ontology, homology_species0_to_species1.Char.Organisms[0], homology_species0_to_species1.Char.Organisms[1]);
            for (int i = 0; i < typeof(Ontology_library_generation_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("If no orthologues found, keep original line");

            Organism_enum species0_organism = homology_species0_to_species1.Char.Organisms[0];
            Organism_enum species1_organism = homology_species0_to_species1.Char.Organisms[1];

            homology_species0_to_species1.Order_by_symbol_0();
            int current_library_length = current_library_lines.Length;
            int ortho_count = homology_species0_to_species1.Ortho.Count;
            int indexHomology = 0;
            int indexHomology_initial = 0;
            int stringCompare;
            Homology_line_class homology_line;
            Ontology_library_line_class library_line;
            Ontology_library_line_class new_library_line;
            List<Ontology_library_line_class> new_library_lines = new List<Ontology_library_line_class>();
            string official_gene_symbol_of_current_scp;
            current_library_lines = current_library_lines.OrderBy(l => l.Scp).ThenBy(l=>l.Organism).ThenBy(l=>l.Organism_initial).ToArray();
            int symbols_with_orthologue_found_count = 0;
            bool orthologue_found_for_species0_symbol = false;
            bool orthologue_found_for_this_library_line;
            List<string> symbols_with_no_found_orthologues = new List<string>();
            int current_species_0_line_counts = 0;
            int current_checked_scp_line_counts = 0;
            Dictionary<string, List<string>> symbol0_sumbol1s_dict = new Dictionary<string, List<string>>();
            for (int indexLibrary = 0; indexLibrary < current_library_length; indexLibrary++)
            {
                library_line = current_library_lines[indexLibrary];
                if (library_line.Organism.Equals(Organism_enum.E_m_p_t_y)) { throw new Exception(); }
                if (  (library_line.Organism_initial.Equals(species0_organism))
                    &&(library_line.Organism.Equals(library_line.Organism_initial)))
                {
                    current_species_0_line_counts++;
                }
                if (  (library_line.Organism_initial.Equals(species0_organism))
                    &&(library_line.Organism.Equals(species1_organism)))
                {
                    orthologue_found_for_this_library_line = false;
                    current_checked_scp_line_counts++;
                    if ((indexLibrary == 0)
                        || (!library_line.Scp.Equals(current_library_lines[indexLibrary - 1].Scp))
                        || (!library_line.Organism.Equals(current_library_lines[indexLibrary - 1].Organism))
                        || (!library_line.Organism_initial.Equals(current_library_lines[indexLibrary - 1].Organism_initial)))
                    {
                        orthologue_found_for_species0_symbol = false;
                    }
                    official_gene_symbol_of_current_scp = (string)library_line.Scp.Clone();
                    stringCompare = -2;
                    indexHomology = indexHomology_initial;
                    while ((indexHomology < ortho_count) && (stringCompare <= 0))
                    {
                        homology_line = homology_species0_to_species1.Ortho[indexHomology];
                        stringCompare = homology_line.Symbol_0.CompareTo(official_gene_symbol_of_current_scp);
                        if (stringCompare < 0)
                        {
                            indexHomology++;
                            indexHomology_initial = indexHomology;
                        }
                        else if (stringCompare == 0)
                        {
                            if (!library_line.Organism.Equals(species1_organism)) { throw new Exception(); }
                            if (!orthologue_found_for_this_library_line)
                            {
                                library_line.Scp = (string)homology_line.Symbol_1.Clone();
                                orthologue_found_for_this_library_line = true;
                                orthologue_found_for_species0_symbol = true;
                            }
                            else
                            {
                                new_library_line = library_line.Deep_copy();
                                new_library_line.Scp = (string)homology_line.Symbol_1.Clone();
                                new_library_lines.Add(new_library_line);
                                if (!symbol0_sumbol1s_dict.ContainsKey(homology_line.Symbol_0))
                                {
                                    symbol0_sumbol1s_dict.Add(homology_line.Symbol_0, new List<string>());
                                }
                                symbol0_sumbol1s_dict[homology_line.Symbol_0].Add(library_line.Scp);
                                symbol0_sumbol1s_dict[homology_line.Symbol_0].Add(new_library_line.Scp);
                                symbol0_sumbol1s_dict[homology_line.Symbol_0] = symbol0_sumbol1s_dict[homology_line.Symbol_0].Distinct().ToList();
                            }
                            indexHomology++;
                        }
                    }
                    if (  (indexLibrary == current_library_length - 1)
                        || (!library_line.Scp.Equals(current_library_lines[indexLibrary + 1].Scp))
                        || (!library_line.Organism.Equals(current_library_lines[indexLibrary + 1].Organism))
                        || (!library_line.Organism_initial.Equals(current_library_lines[indexLibrary + 1].Organism_initial)))
                    {
                        if (orthologue_found_for_species0_symbol)
                        {
                            symbols_with_orthologue_found_count++;
                        }
                        else
                        {
                            symbols_with_no_found_orthologues.Add(library_line.Scp);
                        }
                    }
                }
            }
            Add_to_array(new_library_lines.ToArray());
            if (current_checked_scp_line_counts < current_species_0_line_counts) { throw new Exception(); }
            if (symbols_with_orthologue_found_count < symbols_with_no_found_orthologues.Count) {  throw new Exception(); }
        }
        private Ontology_library_line_class[] Get_orthologes_for_species0_symbols_in_species1_symbols_based_on_input_orthology(Homology_class homology_species0_to_species1, Ontology_library_line_class[] current_library_lines)
        {
            Ontology_type_enum ontology = Get_ontology_and_check_if_only_one(current_library_lines);
            homology_species0_to_species1.Order_by_symbol_0();
            int current_library_length = current_library_lines.Length;
            int ortho_count = homology_species0_to_species1.Ortho.Count;
            int indexHomo = 0;
            int indexHomo_initial = 0;
            int stringCompare;
            Homology_line_class ortho_line;
            Ontology_library_line_class library_line;
            Ontology_library_line_class new_library_line;
            List<Ontology_library_line_class> homologue_library_list = new List<Ontology_library_line_class>();

            Organism_enum species0_organism = homology_species0_to_species1.Char.Organisms[0];
            Organism_enum species1_organism = homology_species0_to_species1.Char.Organisms[1];

            current_library_lines = current_library_lines.OrderBy(l => l.Organism).ThenBy(l => l.Target_gene_symbol).ToArray();
            bool homologue_found_for_gene = false;
            bool homologue_found_for_line = false;
            int unique_target_genes_count = 0;
            int unique_target_genes_with_homologue_count = 0;
            List<string> target_genes_with_no_homologues = new List<string>();
            for (int indexLibrary = 0; indexLibrary < current_library_length; indexLibrary++)
            {
                library_line = current_library_lines[indexLibrary];
                if (       (indexLibrary!=0)
                    &&  (  (library_line.Organism.CompareTo(current_library_lines[indexLibrary - 1].Organism)<0)
                         ||(  (  (library_line.Organism.Equals(current_library_lines[indexLibrary - 1].Organism))
                               &&(library_line.Organism.CompareTo(current_library_lines[indexLibrary - 1].Organism)<0)))))
                    { throw new Exception(); }
                if (library_line.Organism.Equals(Organism_enum.E_m_p_t_y)) { throw new Exception(); }
                if (library_line.Organism.Equals(species0_organism))
                {
                    if (!library_line.Organism.Equals(library_line.Organism_initial)) {  throw new Exception(); }
                    if ((indexLibrary == 0)
                        || (!library_line.Target_gene_symbol.Equals(current_library_lines[indexLibrary - 1].Target_gene_symbol))
                        || (!library_line.Organism.Equals(current_library_lines[indexLibrary - 1].Organism)))
                    {
                        unique_target_genes_count++;
                        homologue_found_for_gene = false;
                    }
                    homologue_found_for_line = false;
                    stringCompare = -2;
                    indexHomo = indexHomo_initial;
                    while ((indexHomo < ortho_count) && (stringCompare <= 0))
                    {
                        ortho_line = homology_species0_to_species1.Ortho[indexHomo];
                        stringCompare = ortho_line.Symbol_0.CompareTo(library_line.Target_gene_symbol);
                        if (stringCompare < 0)
                        {
                            indexHomo++;
                            indexHomo_initial = indexHomo;
                        }
                        else if ((stringCompare == 0)
                                 && (library_line.Organism.Equals(species0_organism)))
                        {
                            homologue_found_for_gene = true;
                            homologue_found_for_line = true;
                            new_library_line = library_line.Deep_copy();
                            new_library_line.Target_gene_symbol = (string)ortho_line.Symbol_1.Clone();
                            new_library_line.Organism_string = "Homologue";
                            new_library_line.Organism = species1_organism;
                            new_library_line.Target_gene_score = -1;
                            homologue_library_list.Add(new_library_line);
                            indexHomo++;
                        }
                        else if (stringCompare == 0)
                        {
                            indexHomo++;
                        }
                    }
                    if ((indexLibrary == current_library_length - 1)
                        || (!library_line.Target_gene_symbol.Equals(current_library_lines[indexLibrary + 1].Target_gene_symbol))
                        || (!library_line.Organism.Equals(current_library_lines[indexLibrary + 1].Organism)))
                    {
                        if (homologue_found_for_gene)
                        {
                            unique_target_genes_with_homologue_count++;
                        }
                        else
                        {
                            target_genes_with_no_homologues.Add(library_line.Target_gene_symbol);
                        }
                    }
                    if (!homologue_found_for_line)
                    {
                        if (homologue_found_for_gene) { throw new Exception(); }
                        new_library_line = library_line.Deep_copy();
                        new_library_line.Organism = species1_organism;
                        new_library_line.Target_gene_score = -1;
                        homologue_library_list.Add(new_library_line);
                    }
                }
            }
            return homologue_library_list.ToArray();
        }
        private void Add_species_homologues(Ontology_type_enum ontology)
        {
            Ontology_library_line_class[] new_human_library;
            List<Ontology_library_line_class> new_library_lines = new List<Ontology_library_line_class>();
            new_human_library = Get_orthologes_for_species0_symbols_in_species1_symbols_based_on_input_orthology(Homology_mouse_to_human, this.Library);
            new_human_library = Set_all_target_genes_to_upper_case(new_human_library);
            new_library_lines.AddRange(new_human_library);
            new_human_library = Get_orthologes_for_species0_symbols_in_species1_symbols_based_on_input_orthology(Homology_rat_to_human, this.Library);
            new_human_library = Set_all_target_genes_to_upper_case(new_human_library);
            new_library_lines.AddRange(new_human_library);
            this.Add_to_array(new_library_lines.ToArray());

            switch (ontology)
            {
                case Ontology_type_enum.Kea_2015:
                case Ontology_type_enum.Drugbank_drug_targets:
                case Ontology_type_enum.Drugbank_enzymes:
                case Ontology_type_enum.Drugbank_transporters:
                case Ontology_type_enum.Molecular_biology_of_the_cell:
                case Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes:
                case Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes:
                    break;
                case Ontology_type_enum.Chea_2016:
                case Ontology_type_enum.Chea_2022:
                case Ontology_type_enum.Encode_tf_chip_seq_2015:
                case Ontology_type_enum.Transfac_and_jaspar_pwms:
                case Ontology_type_enum.Trrust_transcription_factors_2019:
                    Set_orthologes_for_species0_scps_based_to_species1_scps_if_organism_matches(Homology_mouse_to_human, this.Library);
                    Set_orthologes_for_species0_scps_based_to_species1_scps_if_organism_matches(Homology_rat_to_human, this.Library);
                    break;
                default:
                    throw new Exception();
            }
        }
        private Ontology_library_line_class[] Set_all_target_genes_to_upper_case(Ontology_library_line_class[] library_lines)
        {
            foreach (Ontology_library_line_class library_line in library_lines)
            {
                library_line.Target_gene_symbol = library_line.Target_gene_symbol.ToUpper();
            }
            return library_lines;
        }
        private void Set_all_target_genes_to_upper_case()
        {
            this.Library = Set_all_target_genes_to_upper_case(this.Library);
        }
        private void Duplicate_MBCO_bgGenes_for_each_level_and_add_level_specific_ontology(Ontology_type_enum ontology)
        {
            if (!ontology.Equals(Ontology_type_enum.Molecular_biology_of_the_cell))
            {
                throw new Exception();
            }

            int library_length = this.Library.Length;
            Ontology_library_line_class library_line;
            this.Library = this.Library.OrderBy(l => l.Level).ToArray();
            List<Ontology_library_line_class> background_lines = new List<Ontology_library_line_class>();
            List<Ontology_library_line_class> keep_lines = new List<Ontology_library_line_class>();
            Ontology_library_line_class level_specific_bg_line;
            Ontology_type_enum level_specific_ontology = Ontology_type_enum.E_m_p_t_y;
            bool bg_genes_finished = false;
            for (int indexL=0; indexL<library_length; indexL++)
            {
                library_line = Library[indexL];
                if ((indexL==0)&&(!library_line.Scp.Equals("Background genes")))
                {
                    throw new Exception();
                }
                if (library_line.Scp.Equals("Background genes"))
                {
                    if (bg_genes_finished) { throw new Exception(); }
                    background_lines.Add(library_line);
                }
                else
                {
                    bg_genes_finished = true;
                    switch (library_line.Level)
                    {
                        case 1:
                            level_specific_ontology = Ontology_type_enum.Mbco_level1;
                            break;
                        case 2:
                            level_specific_ontology = Ontology_type_enum.Mbco_level2;
                            break;
                        case 3:
                            level_specific_ontology = Ontology_type_enum.Mbco_level3;
                            break;
                        case 4:
                            level_specific_ontology = Ontology_type_enum.Mbco_level4;
                            break;
                        default:
                            throw new Exception();
                    }
                    library_line.Ontology = level_specific_ontology;
                    keep_lines.Add(library_line);
                    if ((indexL == library_length - 1) || (library_line.Level != this.Library[indexL + 1].Level))
                    {
                        foreach (Ontology_library_line_class bg_line in background_lines)
                        {
                            level_specific_bg_line = bg_line.Deep_copy();
                            level_specific_bg_line.Level = library_line.Level;
                            level_specific_bg_line.Ontology = level_specific_ontology;
                            keep_lines.Add(level_specific_bg_line);
                        }
                    }
                }
            }
            this.Library = keep_lines.ToArray();
        }
        private void Set_scps_to_upper_case_if_gene_symbol(Ontology_type_enum ontology)
        {
            switch (ontology)
            {
                case Ontology_type_enum.Kea_2015:
                case Ontology_type_enum.Chea_2022:
                case Ontology_type_enum.Chea_2016:
                case Ontology_type_enum.Encode_tf_chip_seq_2015:
                case Ontology_type_enum.Transfac_and_jaspar_pwms:
                case Ontology_type_enum.Trrust_transcription_factors_2019:
                    foreach (Ontology_library_line_class library_line in Library)
                    {
                        library_line.Scp = library_line.Scp.ToUpper();
                    }
                    break;
                case Ontology_type_enum.Molecular_biology_of_the_cell:
                case Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes:
                case Ontology_type_enum.Drugbank_drug_targets:
                case Ontology_type_enum.Drugbank_enzymes:
                case Ontology_type_enum.Drugbank_transporters:
                case Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes:
                    break;
                default:
                    throw new Exception();
            }
        }
        private void Keep_only_homo_sapiens()
        {
            List<Ontology_library_line_class> keep = new List<Ontology_library_line_class>();
            foreach (Ontology_library_line_class ontology_library_line in this.Library)
            {
                switch (ontology_library_line.Organism)
                {
                    case Organism_enum.Homo_sapiens:
                        keep.Add(ontology_library_line);
                        break;
                    case Organism_enum.Mus_musculus:
                    case Organism_enum.Rattus_norvegicus:
                        break;
                    default:
                        throw new Exception();
                }
            }
            this.Library = keep.ToArray();
        }
        public void Add_background_genes_from_indicated_ontologies_and_own(Ontology_type_enum this_ontology, params Ontology_type_enum[] bgGene_ontologies)
        {
            Dictionary<string, bool> bgGenes_dict = new Dictionary<string, bool>();
            foreach (Ontology_library_line_class library_line in this.Library)
            {
                if (!bgGenes_dict.ContainsKey(library_line.Target_gene_symbol))
                {
                    bgGenes_dict.Add(library_line.Target_gene_symbol, true);
                }
            }

            foreach (Ontology_type_enum bgGene_ontology in bgGene_ontologies)
            {
                Ontology_library_line_class[] ontology_library_lines = new Ontology_library_line_class[0];
                switch (bgGene_ontology)
                {
                    case Ontology_type_enum.Molecular_biology_of_the_cell:
                        ontology_library_lines = Read_mbco_input_library_and_return_library_array(bgGene_ontology);
                        break;
                    case Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes:
                    case Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes:
                        ontology_library_lines = Read_tucker_or_asp_marker_genes_and_return_library_array(bgGene_ontology);
                        break;
                    default:
                        throw new Exception();
                }
                int ontology_library_lines_length = ontology_library_lines.Length;
                Ontology_library_line_class bg_library_line;
                for (int indexO = 0; indexO < ontology_library_lines_length; indexO++)
                {
                    bg_library_line = ontology_library_lines[indexO];
                    if (!bgGenes_dict.ContainsKey(bg_library_line.Target_gene_symbol))
                    {
                        bgGenes_dict.Add(bg_library_line.Target_gene_symbol, true);
                    }
                }
            }
            string[] bgGenes = bgGenes_dict.Keys.ToArray();
            Ontology_library_line_class new_library_line;
            List<Ontology_library_line_class> new_libarary_lines = new List<Ontology_library_line_class>();
            foreach (string bgGene in bgGenes)
            {
                new_library_line = new Ontology_library_line_class();
                new_library_line.Level = -1;
                new_library_line.Ontology = this_ontology;
                new_library_line.Organism = Organism_enum.E_m_p_t_y;
                new_library_line.Scp = "Background genes";
                new_library_line.Target_gene_score = -1;
                new_library_line.Target_gene_symbol = (string)bgGene.Clone();
                new_library_line.Additional_information = "";
                new_libarary_lines.Add(new_library_line);
            }
            Add_to_array(new_libarary_lines.ToArray());
        }
        public void Generate_ontologies(params Ontology_type_enum[] ontologies)
        {
            int ontologies_length = ontologies.Length;
            Ontology_type_enum ontology;
            for (int indexO = 0; indexO < ontologies_length; indexO++)
            {
                ontology = ontologies[indexO];
                Read_input_library_and_override_library_array(ontology);
                Ontology_type_enum[] shared_bgGene_ontologies = new Ontology_type_enum[] { Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes, Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes };
                if (shared_bgGene_ontologies.Contains(ontology))
                {
                    Add_background_genes_from_indicated_ontologies_and_own(ontology, shared_bgGene_ontologies);
                }
                Set_organism(ontology);
                switch (ontology)
                {
                    case Ontology_type_enum.Trrust_transcription_factors_2019:
                    case Ontology_type_enum.Chea_2016:
                    case Ontology_type_enum.Chea_2022:
                    case Ontology_type_enum.Kea_2015:
                    case Ontology_type_enum.Transfac_and_jaspar_pwms:
                    case Ontology_type_enum.Encode_tf_chip_seq_2015:
                        Remove_experiment_information_from_SCP_names_separated_by_space_or_underline_if_TF_or_kinase_library(ontology);
                        break;
                    case Ontology_type_enum.Molecular_biology_of_the_cell:
                    case Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes:
                    case Ontology_type_enum.Drugbank_drug_targets:
                    case Ontology_type_enum.Drugbank_enzymes:
                    case Ontology_type_enum.Drugbank_transporters:
                    case Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes:
                        break;
                    default:
                        throw new Exception();
                }
                Set_all_target_genes_to_upper_case();
                Set_scps_to_upper_case_if_gene_symbol(ontology);

                Add_species_homologues(ontology);
                Keep_only_homo_sapiens();


                Remove_spaces_at_beginning_or_end_of_scp_and_gene_symbol();
                Remove_duplicates();
                Check_for_duplicates();
                Check_if_all_values_assigned();

                Write_one_library_for_each_included_ontology();
                if (ontology.Equals(Ontology_type_enum.Molecular_biology_of_the_cell))
                {
                    Duplicate_MBCO_bgGenes_for_each_level_and_add_level_specific_ontology(ontology);
                    Write_libraries_for_each_level(ontology);
                }
            }
        }

        public void Dispose()
        {
            this.Library = null;
            this.Homology_mouse_to_human.Dispose();
            this.Homology_mouse_to_human = null;
        }

        #region Read ontologies
        private void Read_input_library_and_override_library_array(Ontology_type_enum ontology)
        {
            Report_class.WriteLine("{0}: Read input library and override library array: {0}", ontology);
            this.Library = new Ontology_library_line_class[0];
            switch (ontology)
            {
                case Ontology_type_enum.Kea_2015:
                case Ontology_type_enum.Encode_tf_chip_seq_2015:
                    Read_enrichR_input_library_and_override_library_array(ontology);
                    break;
                case Ontology_type_enum.Drugbank_drug_targets:
                case Ontology_type_enum.Drugbank_enzymes:
                case Ontology_type_enum.Drugbank_transporters:
                    Read_drugBank_xml_and_generate_library_lines(ontology);
                    break;
                case Ontology_type_enum.Transfac_and_jaspar_pwms:
                case Ontology_type_enum.Trrust_transcription_factors_2019:
                case Ontology_type_enum.Chea_2016:
                case Ontology_type_enum.Chea_2022:
                    Read_enrichR_input_library_set_organism_and_override_library_array(ontology);
                    break;
                case Ontology_type_enum.Molecular_biology_of_the_cell:
                    Read_mbco_input_library_and_override_library(ontology);
                    break;
                case Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes:
                case Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes:
                    Read_tucker_or_asp_marker_genes_and_override_library(ontology);
                    break;
                default:
                    throw new Exception();
            }
        }
        private void Read_drugBank_xml_and_generate_library_lines(Ontology_type_enum ontology)
        {
            Drug_bank_class drug_bank = new Drug_bank_class();
            drug_bank.Generate_de_novo_by_reading_raw_data_and_write();
            drug_bank.Keep_only_indicated_ontology(ontology);
            this.Library = drug_bank.Generate_ontology_library_lines();
        }
        private Ontology_library_line_class[] Read_enrichR_input_library(Ontology_type_enum ontology)
        {
            string directory = Global_directory_class.Enrich_libraries_download_directory;
            string ontology_string = ontology.ToString();
            string complete_fileName = directory + ontology_string + ".txt";
            StreamReader reader = new StreamReader(complete_fileName);
            string inputLine;
            string[] splitStrings;
            string gene_splitString;
            string[] gene_splitString_splitStrings;
            string gene;
            string gene_score_string;
            string scp;
            int splitStrings_length;

            List<Ontology_library_line_class> ontology_library_list = new List<Ontology_library_line_class>();
            Ontology_library_line_class new_ontology_line;

            while ((inputLine = reader.ReadLine()) != null)
            {
                splitStrings = inputLine.Split(Global_class.Tab);
                splitStrings_length = splitStrings.Length;
                scp = splitStrings[0];
                for (int indexSP = 1; indexSP < splitStrings_length; indexSP++)
                {
                    gene_splitString = splitStrings[indexSP];
                    if (String.IsNullOrEmpty(gene_splitString))
                    {
                        if ((indexSP != splitStrings_length - 1)
                            && (indexSP != 1))
                        {
                            //throw new Exception();
                        }
                    }
                    else
                    {
                        if (indexSP == 1) { throw new Exception(); }
                        gene_splitString_splitStrings = gene_splitString.Split(',');
                        gene = gene_splitString_splitStrings[0];

                        new_ontology_line = new Ontology_library_line_class();
                        new_ontology_line.Scp = (string)scp.Clone();
                        new_ontology_line.Ontology = ontology;
                        new_ontology_line.Target_gene_symbol = (string)gene.Clone();

                        if (gene_splitString_splitStrings.Length == 1)
                        {
                        }
                        else if (gene_splitString_splitStrings.Length == 2)
                        {
                            gene_score_string = gene_splitString_splitStrings[1];
                            new_ontology_line.Target_gene_score = float.Parse(gene_score_string);
                        }
                        else
                        {
                            throw new Exception();
                        }

                        ontology_library_list.Add(new_ontology_line);
                    }
                }
            }
            return ontology_library_list.ToArray();
        }
        private void Read_enrichR_input_library_and_override_library_array(Ontology_type_enum ontology)
        {
            this.Library = Read_enrichR_input_library(ontology);
        }
        private void Read_enrichR_input_library_set_organism_and_override_library_array(Ontology_type_enum ontology)
        {
            string directory = Global_directory_class.Enrich_libraries_download_directory;
            string[] all_files_in_directory = Directory.GetFiles(directory);
            string complete_fileName;
            string fileName_without_extension;
            int all_files_in_directory_length = all_files_in_directory.Length;
            bool is_mouse_data_set;
            bool is_human_data_set;
            string inputLine;
            string[] splitStrings;
            int splitStrings_length;
            string scp;
            string target_gene;
            string additional_information;
            Ontology_library_line_class new_ontology_line;
            List<Ontology_library_line_class> ontology_library_list = new List<Ontology_library_line_class>();
            int files_found_count = 0;
            for (int indexF = 0; indexF < all_files_in_directory_length; indexF++)
            {
                complete_fileName = all_files_in_directory[indexF];
                fileName_without_extension = Path.GetFileNameWithoutExtension(complete_fileName);
                if (fileName_without_extension.ToUpper().Replace("-","_").IndexOf(ontology.ToString().ToUpper()) != -1)
                {
                    files_found_count++;
                    is_mouse_data_set = false;
                    is_human_data_set = false;
                    StreamReader reader = new StreamReader(complete_fileName);
                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        splitStrings = inputLine.Split(Global_class.Tab);
                        splitStrings_length = splitStrings.Length;
                        scp = splitStrings[0];
                        if (scp.ToLower().IndexOf("human") != -1) { is_human_data_set = true; }
                        if (scp.ToLower().IndexOf("mouse") != -1) { is_mouse_data_set = true; }
                        if (!is_mouse_data_set && !is_human_data_set) { throw new Exception(); }
                        additional_information = splitStrings[1];
                        for (int indexSP = 2; indexSP < splitStrings_length; indexSP++)
                        {
                            target_gene = splitStrings[indexSP];
                            if (String.IsNullOrEmpty(target_gene))
                            {
                            }
                            else
                            {
                                new_ontology_line = new Ontology_library_line_class();
                                new_ontology_line.Scp = (string)scp.Clone();
                                new_ontology_line.Additional_information = (string)additional_information.Clone();
                                new_ontology_line.Ontology = ontology;
                                new_ontology_line.Target_gene_symbol = target_gene.Split(',')[0];
                                if ((is_human_data_set) && (is_mouse_data_set))
                                {
                                    new_ontology_line.Organism_string = "Human-mouse";
                                    new_ontology_line.Organism = Organism_enum.E_m_p_t_y;
                                }
                                else if (is_human_data_set)
                                { 
                                    new_ontology_line.Organism_string = "Human";
                                    new_ontology_line.Organism = Organism_enum.Homo_sapiens;
                                }
                                else if (is_mouse_data_set)
                                {
                                    new_ontology_line.Organism_string = "Mouse";
                                    new_ontology_line.Organism = Organism_enum.Mus_musculus;
                                }
                                else
                                {
                                  //  string lol = "";
                                }
                                ontology_library_list.Add(new_ontology_line);
                            }
                        }
                    }
                }
            }
            if (files_found_count!=1) { throw new Exception(); }
            this.Library = ontology_library_list.ToArray();
        }
        private void Keep_only_first_word_of_scp_if_transcriptionFactor_ontology(Ontology_type_enum ontology)
        {
            switch (ontology)
            {
                case Ontology_type_enum.Chea_2016:
                case Ontology_type_enum.Trrust_transcription_factors_2019:
                case Ontology_type_enum.Encode_tf_chip_seq_2015:
                    break;
                default:
                    throw new Exception();
                    
            }
            foreach (Ontology_library_line_class library_line in this.Library)
            {
                library_line.Scp = library_line.Scp.Split(' ', '_')[0];
            }
        }
        private Ontology_library_line_class[] Read_mbco_input_library_and_return_library_array(Ontology_type_enum ontology)
        {
            if (!ontology.Equals(Ontology_type_enum.Molecular_biology_of_the_cell)) { throw new Exception(); }
            Ontology_library_mbco_readOptions_class readOptions = new Ontology_library_mbco_readOptions_class(ontology);
            Ontology_library_line_class[] library_lines = ReadWriteClass.ReadRawData_and_FillArray<Ontology_library_line_class>(readOptions);
            foreach (Ontology_library_line_class library_line in library_lines)
            {
                library_line.Ontology = ontology;
            }
            return library_lines;
        }
        private void Read_mbco_input_library_and_override_library(Ontology_type_enum ontology)
        {
            if (!ontology.Equals(Ontology_type_enum.Molecular_biology_of_the_cell)) { throw new Exception(); }
            this.Library = Read_mbco_input_library_and_return_library_array(ontology);
        }
        private Ontology_library_line_class[] Read_tucker_or_asp_marker_genes_and_return_library_array(Ontology_type_enum ontology)
        {
            ScRNAseq_marker_gene_class scRNAseq_markers = new ScRNAseq_marker_gene_class();
            scRNAseq_markers.Generate_by_reading(ontology);
            int marker_genes_length = scRNAseq_markers.Marker_genes.Length;
            ScRNAseq_marker_gene_line_class tucker_line;
            Ontology_library_line_class new_library_line;
            List<Ontology_library_line_class> new_library_lines = new List<Ontology_library_line_class>();
            for (int indexMarker = 0; indexMarker < marker_genes_length; indexMarker++)
            {
                tucker_line = scRNAseq_markers.Marker_genes[indexMarker];
                new_library_line = new Ontology_library_line_class();
                new_library_line.Ontology = ontology;
                new_library_line.Target_gene_symbol = (string)tucker_line.Gene.Clone();
                new_library_line.Scp = (string)tucker_line.Cell_type.Clone();
                new_library_lines.Add(new_library_line);
            }
            return new_library_lines.ToArray();
        }
        private void Read_tucker_or_asp_marker_genes_and_override_library(Ontology_type_enum ontology)
        {
            this.Library = Read_tucker_or_asp_marker_genes_and_return_library_array(ontology);
        }
        #endregion
        public void Write_one_library_for_each_included_ontology()
        {
            int library_length = this.Library.Length;
            Ontology_library_line_class library_line;
            Library = Library.OrderBy(l=>l.Ontology).ThenBy(l => l.Organism).ThenBy(l => l.Scp).ThenBy(l => l.Target_gene_symbol).ToArray();
            List<Ontology_library_line_class> sameOrganism_library = new List<Ontology_library_line_class>();
            for (int indexL=0; indexL<library_length; indexL++)
            {
                library_line = Library[indexL];
                if (  (indexL==0)
                    || (!library_line.Ontology.Equals(Library[indexL - 1].Ontology))
                    || (!library_line.Organism.Equals(Library[indexL - 1].Organism)))
                {
                    sameOrganism_library.Clear();
                }
                sameOrganism_library.Add(library_line);
                if ((indexL == library_length-1)
                    || (!library_line.Ontology.Equals(Library[indexL + 1].Ontology))
                    || (!library_line.Organism.Equals(Library[indexL + 1].Organism)))
                {
                    Ontology_libary_readWriteOptions_class readWriteOptions = new Ontology_libary_readWriteOptions_class(library_line.Ontology, library_line.Organism);
                    ReadWriteClass.WriteData(sameOrganism_library.ToArray(), readWriteOptions);
                }
            }
        }
        private void Write_libraries_for_each_level(Ontology_type_enum ontology)
        {
            if (!ontology.Equals(Ontology_type_enum.Molecular_biology_of_the_cell)) { throw new Exception(); }
            int library_length = this.Library.Length;
            Ontology_library_line_class library_line;
            Ontology_type_enum write_ontology = Ontology_type_enum.E_m_p_t_y;
            Library = Library.OrderBy(l => l.Organism).ThenBy(l=>l.Level).ThenBy(l => l.Scp).ThenBy(l => l.Target_gene_symbol).ToArray();
            List<Ontology_library_line_class> sameOrganism_sameLevel_library = new List<Ontology_library_line_class>();
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = Library[indexL];
                if (  (indexL == 0)
                    || (!library_line.Organism.Equals(Library[indexL - 1].Organism))
                    || (!library_line.Level.Equals(Library[indexL - 1].Level)))
                {
                    sameOrganism_sameLevel_library.Clear();
                }
                sameOrganism_sameLevel_library.Add(library_line);
                if ((indexL == library_length-1)
                    || (!library_line.Organism.Equals(Library[indexL + 1].Organism))
                    || (!library_line.Level.Equals(Library[indexL + 1].Level)))
                {
                    switch (ontology)
                    {
                        case Ontology_type_enum.Molecular_biology_of_the_cell:
                            switch (library_line.Level)
                            {
                                case 1:
                                    write_ontology = Ontology_type_enum.Mbco_level1;
                                    break;
                                case 2:
                                    write_ontology = Ontology_type_enum.Mbco_level2;
                                    break;
                                case 3:
                                    write_ontology = Ontology_type_enum.Mbco_level3;
                                    break;
                                case 4:
                                    write_ontology = Ontology_type_enum.Mbco_level4;
                                    break;
                                default:
                                    throw new Exception();
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                    Ontology_libary_readWriteOptions_class readWriteOptions = new Ontology_libary_readWriteOptions_class(write_ontology, library_line.Organism);
                    ReadWriteClass.WriteData(sameOrganism_sameLevel_library.ToArray(), readWriteOptions);
                }
            }
        }
    }

    class Ontology_library_class
    {
        public Ontology_library_line_class[] Library { get; set; }

        public Ontology_library_class()
        {
            this.Library = new Ontology_library_line_class[0];
        }

        #region Order
        public void Order_by_scp_target_gene_symbol()
        {
            this.Library = Ontology_library_line_class.Order_by_scp_targetGeneSymbol(this.Library);
            //this.Library = this.Library.OrderBy(l => l.Scp).ThenBy(l => l.Target_gene_symbol).ToArray();
        }
        #endregion

        public void Add_to_array(Ontology_library_line_class[] add_library)
        {
            int this_length = this.Library.Length;
            int add_length = add_library.Length;
            int new_length = this_length + add_length;
            int indexNew = -1;
            Ontology_library_line_class[] new_library = new Ontology_library_line_class[new_length];
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_library[indexNew] = this.Library[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_library[indexNew] = add_library[indexAdd];
            }
            this.Library = new_library;
        }

        #region Generate by reading
        public void Remove_duplicates_based_on_scp_gene_target_symbols()
        {
            this.Library = this.Library.OrderBy(l => l.Scp).ThenBy(l => l.Target_gene_symbol).ToArray();
            int library_length = this.Library.Length;
            Ontology_library_line_class library_line;
            List<Ontology_library_line_class> library_list = new List<Ontology_library_line_class>();
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = this.Library[indexL];
                if ((indexL == 0)
                    || (!library_line.Scp.Equals(this.Library[indexL - 1].Scp))
                    || (!library_line.Target_gene_symbol.Equals(this.Library[indexL - 1].Target_gene_symbol)))
                {
                    library_line.Target_gene_score = -1;
                    library_line.Additional_information = "only unique lines are kept";
                    library_list.Add(library_line);
                }
            }
            this.Library = library_list.ToArray();
        }

        public void Generate_by_reading(Ontology_type_enum ontology, Organism_enum organism, params string[] bg_genes_in_upperCase)
        {
            Read_and_override_library(ontology, organism);
            Remove_duplicates_based_on_scp_gene_target_symbols();
            if (bg_genes_in_upperCase.Length > 0) { Keep_only_indicated_genes(bg_genes_in_upperCase); }
        }

        public void Generate_by_reading(Ontology_type_enum ontology, params string[] bg_genes_in_upperCase)
        {
            Generate_by_reading(ontology, Global_class.Organism, bg_genes_in_upperCase);
        }

        private void Read_and_override_library(Ontology_type_enum ontology, Organism_enum organism)
        {
            Ontology_libary_readWriteOptions_class readWriteOptions = new Ontology_libary_readWriteOptions_class(ontology, organism);
            this.Library = ReadWriteClass.ReadRawData_and_FillArray<Ontology_library_line_class>(readWriteOptions);
        }
        #endregion

        public void Check_if_all_scps_are_single_terms_using_space_and_underline_delimiter()
        {
            foreach (Ontology_library_line_class library_line in this.Library)
            {
                if (library_line.Scp.Split('_',' ').Length!=1) { throw new Exception(); };
            }
            Remove_duplicates_based_on_scp_gene_target_symbols();
        }

        #region Keep
        public void Keep_only_indicated_genes(string[] keep_genes_in_upperCase)
        {
            keep_genes_in_upperCase = keep_genes_in_upperCase.Distinct().OrderBy(l => l).ToArray();
            int keep_genes_length = keep_genes_in_upperCase.Length;
            int indexKeep = 0;
            string keep_gene;

            int stringCompare = -2;
            Ontology_library_line_class library_line;
            int library_length = this.Library.Length;
            List<Ontology_library_line_class> keep = new List<Ontology_library_line_class>();
            Library = Library.OrderBy(l => l.Target_gene_symbol).ToArray();

            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = Library[indexL];
                stringCompare = -2;
                while ((indexKeep < keep_genes_length) && (stringCompare < 0))
                {
                    keep_gene = keep_genes_in_upperCase[indexKeep];
                    stringCompare = keep_gene.CompareTo(library_line.Target_gene_symbol);
                    if (stringCompare < 0)
                    {
                        indexKeep++;
                    }
                    else if (stringCompare == 0)
                    {
                        keep.Add(library_line);
                    }
                }
            }
            this.Library = keep.ToArray();
        }
        public void Keep_only_indicated_scps(string[] keep_scps)
        {
            keep_scps = keep_scps.Distinct().OrderBy(l => l).ToArray();
            int keep_genes_length = keep_scps.Length;
            int indexKeep = 0;
            string keep_scp;

            int stringCompare = -2;
            Ontology_library_line_class library_line;
            int library_length = this.Library.Length;
            List<Ontology_library_line_class> keep = new List<Ontology_library_line_class>();
            Library = Library.OrderBy(l => l.Scp).ToArray();
            bool scp_found = false;
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = Library[indexL];
                stringCompare = -2;
                while ((indexKeep < keep_genes_length) && (stringCompare < 0))
                {
                    keep_scp = keep_scps[indexKeep];
                    stringCompare = keep_scp.CompareTo(library_line.Scp);
                    if (stringCompare < 0)
                    {
                        if (!scp_found) { throw new Exception(); }
                        indexKeep++;
                        scp_found = false;
                    }
                    else if (stringCompare == 0)
                    {
                        keep.Add(library_line);
                        scp_found = true;
                    }
                }
            }
            this.Library = keep.ToArray();
        }
        public void Keep_only_indicated_gene_symbols_if_existing(string[] keep_symbols)
        {
            keep_symbols = keep_symbols.Distinct().OrderBy(l => l).ToArray();
            Dictionary<string, bool> keep_symbol_dict = new Dictionary<string, bool>();
            int keep_symbols_length = keep_symbols.Length;
            for (int indexKeep=0; indexKeep<keep_symbols_length; indexKeep++)
            {
                keep_symbol_dict.Add(keep_symbols[indexKeep],true);
            }

            Ontology_library_line_class library_line;
            int library_length = this.Library.Length;
            List<Ontology_library_line_class> keep = new List<Ontology_library_line_class>();

            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = Library[indexL];
                if (keep_symbol_dict.ContainsKey(library_line.Target_gene_symbol))
                { 
                    keep.Add(library_line);
                }
            }
            this.Library = keep.ToArray();
        }
        #endregion

        #region Get 
        public Ontology_type_enum Get_current_ontology_and_check_if_only_one()
        {
            Ontology_type_enum ontology = this.Library[0].Ontology;
            foreach (Ontology_library_line_class ontology_library_line in this.Library)
            {
                if (!ontology_library_line.Ontology.Equals(ontology))
                {
                    throw new Exception();
                }
            }
            return ontology;
        }
        public string[] Get_all_ordered_unique_scps_targeting_inputSymbol(string inputTargetGeneSymbol)
        {
            Ontology_library_line_class library_line;
            List<string> all_scps = new List<string>();
            int library_length = this.Library.Length;
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = this.Library[indexL];
                if (library_line.Target_gene_symbol.Equals(inputTargetGeneSymbol))
                {
                    all_scps.Add(library_line.Scp);
                }
            }
            return all_scps.Distinct().ToArray();
        }
        public string[] Get_all_ordered_unique_scps_targeting_inputSymbols(params string[] geneSymbols)
        {
            geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
            Dictionary<string, bool> geneSymbols_dict = new Dictionary<string, bool>();
            foreach (string geneSymbol in geneSymbols)
            {
                geneSymbols_dict.Add(geneSymbol, true);
            }

            Ontology_library_line_class library_line;
            List<string> all_scps = new List<string>();
            int library_length = this.Library.Length;
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = this.Library[indexL];
                if (geneSymbols_dict.ContainsKey(library_line.Target_gene_symbol))
                {
                    all_scps.Add(library_line.Scp);
                }
            }
            return all_scps.Distinct().ToArray();
        }
        public string[] Get_all_ordered_unique_gene_symbols()
        {
            int library_length = this.Library.Length;
            Ontology_library_line_class library_line;
            List<string> all_symbols = new List<string>();
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = this.Library[indexL];
                all_symbols.Add(library_line.Target_gene_symbol);
            }
            return all_symbols.Distinct().OrderBy(l=>l).ToArray();
        }
        public string[] Get_all_ordered_unique_gene_symbols_of_input_scps(params string[] input_scps)
        {
            input_scps = input_scps.Distinct().OrderBy(l => l).ToArray();
            string input_scp;
            int indexInput = 0;
            int input_scps_length = input_scps.Length;

            this.Library = Ontology_library_line_class.Order_by_scp_targetGeneSymbol(this.Library);
            int library_length = this.Library.Length;
            Ontology_library_line_class library_line;
            List<string> all_symbols = new List<string>();
            int stringCompare = -2;
            bool input_scp_exists = false;
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = this.Library[indexL];
                stringCompare = -2;
                while ((indexInput < input_scps_length) && (stringCompare < 0))
                {
                    input_scp = input_scps[indexInput];
                    stringCompare = input_scp.ToUpper().CompareTo(library_line.Scp.ToUpper());
                    if (stringCompare < 0)
                    {
                        if (!input_scp_exists) { throw new Exception(); }
                        indexInput++;
                        input_scp_exists = false;
                    }
                    else if (stringCompare == 0)
                    {
                        all_symbols.Add(library_line.Target_gene_symbol);
                        input_scp_exists = true;
                    }
                }
            }
            return all_symbols.Distinct().OrderBy(l => l).ToArray();
        }
        public string[] Get_all_ordered_unique_gene_symbols_of_input_scps_if_they_exist(params string[] input_scps)
        {
            input_scps = input_scps.Distinct().OrderBy(l => l).ToArray();
            string input_scp;
            int indexInput = 0;
            int input_scps_length = input_scps.Length;

            this.Library = Ontology_library_line_class.Order_by_scp_targetGeneSymbol(this.Library);
            int library_length = this.Library.Length;
            Ontology_library_line_class library_line;
            List<string> all_symbols = new List<string>();
            int stringCompare = -2;
            for (int indexL = 0; indexL < library_length; indexL++)
            {
                library_line = this.Library[indexL];
                stringCompare = -2;
                while ((indexInput < input_scps_length) && (stringCompare < 0))
                {
                    input_scp = input_scps[indexInput];
                    stringCompare = input_scp.ToUpper().CompareTo(library_line.Scp.ToUpper());
                    if (stringCompare < 0)
                    {
                        indexInput++;
                    }
                    else if (stringCompare == 0)
                    {
                        all_symbols.Add(library_line.Target_gene_symbol);
                    }
                }
            }
            return all_symbols.Distinct().OrderBy(l => l).ToArray();
        }
        public Dictionary<string,int> Get_scp_geneCount_dictionary()
        {
            this.Library = Ontology_library_line_class.Order_by_scp_targetGeneSymbol(this.Library);
            int library_length = Library.Length;
            Ontology_library_line_class ontology_library_line;
            Dictionary<string, int> scp_geneCount_dict = new Dictionary<string, int>();
            int gene_count_of_current_scp = 0;
            for (int indexO=0; indexO<library_length; indexO++)
            {
                ontology_library_line = this.Library[indexO];
                if ((indexO==0) ||(!ontology_library_line.Scp.Equals(this.Library[indexO-1].Scp)))
                {
                    gene_count_of_current_scp = 0;
                }
                gene_count_of_current_scp++;
                if ((indexO == library_length-1) || (!ontology_library_line.Scp.Equals(this.Library[indexO + 1].Scp)))
                {
                    if (!ontology_library_line.Scp.Equals("Background genes"))
                    {
                        scp_geneCount_dict.Add(ontology_library_line.Scp, gene_count_of_current_scp);
                    }
                }
            }
            return scp_geneCount_dict;
        }
        public Dictionary<string, string[]> Get_scp_genes_dictionary()
        {
            this.Library = Ontology_library_line_class.Order_by_scp_targetGeneSymbol(this.Library);
            int library_length = Library.Length;
            Ontology_library_line_class ontology_library_line;
            Dictionary<string, string[]> scp_targetSymbols_dict = new Dictionary<string, string[]>();
            List<string> current_targetSymbols = new List<string>();
            for (int indexO = 0; indexO < library_length; indexO++)
            {
                ontology_library_line = this.Library[indexO];
                if ((indexO == 0) || (!ontology_library_line.Scp.Equals(this.Library[indexO - 1].Scp)))
                {
                    current_targetSymbols.Clear();
                }
                current_targetSymbols.Add(ontology_library_line.Target_gene_symbol);
                if ((indexO == library_length - 1) || (!ontology_library_line.Scp.Equals(this.Library[indexO + 1].Scp)))
                {
                    if (!ontology_library_line.Scp.Equals("Background genes"))
                    {
                        scp_targetSymbols_dict.Add(ontology_library_line.Scp, current_targetSymbols.ToArray());
                    }
                }
            }
            return scp_targetSymbols_dict;
        }
        public Dictionary<string, string[]> Get_gene_scps_dictionary()
        {
            this.Library = Ontology_library_line_class.Order_by_targetGeneSymbol_scp(this.Library);
            int library_length = Library.Length;
            Ontology_library_line_class ontology_library_line;
            Dictionary<string, string[]> targetSymbol_scp_dict = new Dictionary<string, string[]>();
            List<string> current_scps= new List<string>();
            for (int indexO = 0; indexO < library_length; indexO++)
            {
                ontology_library_line = this.Library[indexO];
                if ((indexO == 0) || (!ontology_library_line.Target_gene_symbol.Equals(this.Library[indexO - 1].Target_gene_symbol)))
                {
                    current_scps.Clear();
                }
                current_scps.Add(ontology_library_line.Scp);
                if ((indexO == library_length - 1) || (!ontology_library_line.Target_gene_symbol.Equals(this.Library[indexO + 1].Target_gene_symbol)))
                {
                    if (!ontology_library_line.Scp.Equals("Background genes"))
                    {
                        targetSymbol_scp_dict.Add(ontology_library_line.Target_gene_symbol, current_scps.ToArray());
                    }
                }
            }
            return targetSymbol_scp_dict;
        }
        #endregion

        public void Add_other(Ontology_library_class other)
        {
            this.Add_to_array(other.Library);
        }
        public void Write_to_results_directory(string subdirectory, string fileName)
        {
            Ontology_libary_resultsDirectory_readWriteOptions_class readWriteOptions = new Ontology_libary_resultsDirectory_readWriteOptions_class(subdirectory, fileName);
            ReadWriteClass.WriteData(Library, readWriteOptions);
        }
        public Ontology_library_class Deep_copy()
        {
            Ontology_library_class copy = (Ontology_library_class)this.MemberwiseClone();
            int library_length = this.Library.Length;
            copy.Library = new Ontology_library_line_class[library_length];
            for (int indexL=0; indexL<library_length; indexL++)
            {
                copy.Library[indexL] = this.Library[indexL].Deep_copy();
            }
            return copy;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Ontology_functions
    {
        public static void Generate_all_enrichR_libraries()
        {
            Ontology_type_enum[] ontologies = new Ontology_type_enum[]
            {
               Ontology_type_enum.Drugbank_drug_targets,
               Ontology_type_enum.Drugbank_enzymes,
               Ontology_type_enum.Drugbank_transporters,
               Ontology_type_enum.Molecular_biology_of_the_cell,
               Ontology_type_enum.Asp_2019_developing_heart_cell_type_marker_genes,
               Ontology_type_enum.Tucker_2020_adult_heart_cell_type_marker_genes,
               //Ontology_type_enum.Chea_2016,
               Ontology_type_enum.Chea_2022,
               Ontology_type_enum.Encode_tf_chip_seq_2015,
               Ontology_type_enum.Kea_2015,
               Ontology_type_enum.Transfac_and_jaspar_pwms,
               Ontology_type_enum.Trrust_transcription_factors_2019
           };
           ontologies = ontologies.Distinct().OrderBy(l => l).ToArray();
           using (Ontology_library_generation_class library_generation = new Ontology_library_generation_class())
           { library_generation.Generate_ontologies(ontologies); }
        }
    }

}
