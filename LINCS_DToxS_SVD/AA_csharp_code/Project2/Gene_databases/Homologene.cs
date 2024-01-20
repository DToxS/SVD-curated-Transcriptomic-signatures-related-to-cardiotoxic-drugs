// Jackson Labs is missing

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ReadWrite;
using Common_classes;

namespace Gene_databases
{

    class Orthologies_directory_class : ReadWriteOptions_base
    {
        public const char Tab = '\t';
        public const char Space = ' ';

        protected string Directory { get; private set; }
        protected string Directory_downloaded_files { get; private set; }
        protected string Directory_selfmade_files { get; private set; }

        public Orthologies_directory_class()
        {
            Directory = Global_directory_class.Major_directory + "Orthologies//";
            Directory_downloaded_files = Directory + "Download//";
            Directory_selfmade_files = Directory + "Self//";
        }
    }

    /// ///////////////////////////////////////////////////////////////////////////////////////

    class Homologene_readOptions_class : ReadWriteOptions_base
    {
        public Homologene_readOptions_class(params Organism_enum[] organisms)
        {
            File = Global_directory_class.GeneDatabases_homology_download_directory + "homologene.data";
            Key_propertyNames = new string[] { "GeneNr", "Organism", "GeneID", "Symbol" };
            Key_columnNames = new string[] { };
            Key_columnIndexes = new int[4] { 0, 1, 2, 3 };

            SafeCondition_columnIndexes = new int[organisms.Length];
            SafeCondition_columnNames = new string[0];
            SafeCondition_entries = new string[organisms.Length];
            for (int i = 0; i < organisms.Length; i++)
            {
                SafeCondition_columnIndexes[i] = 1;
                SafeCondition_entries[i] = ((int)organisms[i]).ToString();
            }

            File_has_headline = false;
            RemoveFromHeadline = null;
            LineDelimiters = new char[] { Global_class.Tab };
            HeadlineDelimiters = new char[] { Global_class.Space };

            Report = ReadWrite_report_enum.Report_everything;
        }
    }

    class Homologene_line_class
    {
        public int GeneNr { get; set; }
        public Organism_enum Organism { get; set; }
        public int GeneID { get; set; }
        public string Symbol { get; set; }
    }

    class Homologene_class
    {
        public List<Homologene_line_class> Data { get; set; }
        public Organism_enum[] Organisms { get; private set; }

        public Homologene_class(params Organism_enum[] organisms)
        {
            Organisms = organisms;
        }

        public void ReadRawData()
        {
            Homologene_readOptions_class readOptions = new Homologene_readOptions_class(Organisms);
            Data = ReadWriteClass.ReadRawData_and_FillList<Homologene_line_class>(readOptions);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////

    class MGI_orthology_readOptions_class : Orthologies_directory_class
    {
        public MGI_orthology_readOptions_class(Organism_enum[] organisms)
        {
            if (organisms.Length != 2)
            {
                Report_class.WriteLine("{0}: organisms length has to be 2", typeof(MGI_orthology_readOptions_class).Name);
            }
            organisms = organisms.OrderBy(l => l).ToArray();
            //File = Directory_downloaded_files + "MGI_orthology_" + organisms[0] + "_" + organisms[1] + "_052412.txt";
            File = Directory_downloaded_files + "MGI_orthology_" + organisms[0] + "_" + organisms[1] + ".txt";
            Key_propertyNames = new string[] { "Species0_symbol", "Species0_geneID", "Species1_symbol", "Species1_geneID" };
            List<string> columnNames = new List<string>();
            for (int indexOrg = 0; indexOrg < organisms.Length; indexOrg++)
            {
                switch (organisms[indexOrg])
                {
                    case Organism_enum.Mus_musculus:
                        columnNames.Add("Mouse Symbol");
                        columnNames.Add("Mouse EntrezGene ID");
                        break;
                    case Organism_enum.Homo_sapiens:
                        columnNames.Add("Human Symbol");
                        columnNames.Add("Human EntrezGene ID");
                        break;
                    default:
                        throw new Exception();
                }
            }
            Key_columnNames = new string[columnNames.Count];
            for (int i = 0; i < Key_columnNames.Length; i++)
            {
                Key_columnNames[i] = columnNames[i];
            }
            File_has_headline = true;
            RemoveFromHeadline = null;
            LineDelimiters = new char[] { Global_class.Tab };
            HeadlineDelimiters = new char[] { Global_class.Tab };

            Report = ReadWrite_report_enum.Report_everything;
        }
    }

    class MGI_orthology_line_class
    {
        public string Species0_symbol { get; set; }
        public int Species0_geneID { get; set; }
        public Organism_enum Species0_organism { get; set; }
        public string Species1_symbol { get; set; }
        public int Species1_geneID { get; set; }
        public Organism_enum Species1_organism { get; set; }

        public MGI_orthology_line_class Deep_copy()
        {
            MGI_orthology_line_class copy = (MGI_orthology_line_class)this.MemberwiseClone();
            copy.Species0_symbol = (string)this.Species0_symbol.Clone();
            copy.Species1_symbol = (string)this.Species1_symbol.Clone();
            return copy;
        }

    }

    class MGI_orthology_class
    {
        public List<MGI_orthology_line_class> Data { get; set; }

        public MGI_orthology_class()
        {
            Data = new List<MGI_orthology_line_class>();
        }

        public MGI_orthology_class Deep_copy()
        {
            MGI_orthology_class copy = new MGI_orthology_class();
            foreach (MGI_orthology_line_class line in this.Data)
            {
                copy.Data.Add(line.Deep_copy());
            }
            return copy;
        }

        private void ReadRawData(Organism_enum[] organisms)
        {
            MGI_orthology_readOptions_class readOptions = new MGI_orthology_readOptions_class(organisms);
            Data = ReadWriteClass.ReadRawData_and_FillList<MGI_orthology_line_class>(readOptions);
        }

        private void Fill_with_organism_info(Organism_enum[] organisms)
        {
            organisms = organisms.OrderBy(l => l).ToArray();
            foreach (MGI_orthology_line_class line in Data)
            {
                line.Species0_organism = organisms[0];
                line.Species1_organism = organisms[1];
            }
        }

        public void Generate(Organism_enum[] organisms)
        {
            Report_class.WriteLine("{0}: Generate", typeof(MGI_orthology_class).Name);
            ReadRawData(organisms);
            Fill_with_organism_info(organisms);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////

    class Mgi_homologene_2018June01_line_class
    {
        public string DB_class_key { get; set; }
        public string Common_organism_name { get; set; }
        public int Ncbi_taxon_id { get; set; }
        public string Symbol { get; set; }

        public int EntrezGene_ID { get; set; }

        public Mgi_homologene_2018June01_line_class Deep_copy()
        {
            Mgi_homologene_2018June01_line_class copy = (Mgi_homologene_2018June01_line_class)this.MemberwiseClone();
            copy.DB_class_key = (string)this.DB_class_key.Clone();
            copy.Common_organism_name = (string)this.Common_organism_name.Clone();
            copy.Ncbi_taxon_id = this.Ncbi_taxon_id;
            copy.Symbol = (string)this.Symbol.Clone();
            return copy;
        }
    }

    class Mgi_homologene_2018June01_readWriteOptions_class : ReadWriteOptions_base
    {
        public static string Complete_fileName = Global_directory_class.GeneDatabases_homology_download_directory + "HOM_AllOrganism.rpt";

        public Mgi_homologene_2018June01_readWriteOptions_class()
        {
            this.File = Complete_fileName;
            this.Key_propertyNames = new string[] { "DB_class_key", "Common_organism_name", "Ncbi_taxon_id", "Symbol", "EntrezGene_ID" };
            this.Key_columnNames = new string[] { "DB Class Key", "Common Organism Name", "NCBI Taxon ID", "Symbol", "EntrezGene ID" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class Mgi_homologene_2018June_class
    {
        public Mgi_homologene_2018June01_line_class[] Mgi_homologene { get; set; }

        public void Generate()
        {
            Read();
        }

        private void Read()
        {
            Mgi_homologene_2018June01_readWriteOptions_class readWriteOptions = new Mgi_homologene_2018June01_readWriteOptions_class();
            this.Mgi_homologene = ReadWriteClass.ReadRawData_and_FillArray<Mgi_homologene_2018June01_line_class>(readWriteOptions);
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////

    interface IUse_homology<T> where T : IUse_homology<T>
    {
        string Symbol_0 { get; }
        string Symbol_1 { get; set; }
        int GeneID_1 { get; set; }
        SpeciesSwitch_enum Origin_1 { get; set; }

        T Deep_copy();
    }

    class Homology_readWriteOptions_class : ReadWriteOptions_base
    {
        public Homology_readWriteOptions_class(Homology_char_class ochar)
        {
            if (ochar.Organisms.Length != 2)
            {
                throw new Exception();
            }
            StringBuilder file = new StringBuilder();
            File = Global_directory_class.GeneDatabases_homology_self_directory + "Orthology_" + ochar.Organisms[0] + "_" + ochar.Organisms[1] + ".txt";
            Key_propertyNames = new string[] { "GeneNr", "Symbol_0", "GeneID_0", "Origin_0", "Symbol_1", "GeneID_1", "Origin_1" };
            Key_columnNames = Key_propertyNames;
            Key_columnIndexes = null;

            SafeCondition_columnNames = null;
            SafeCondition_columnIndexes = null;
            SafeCondition_entries = null;

            File_has_headline = true;
            RemoveFromHeadline = new string[0];
            LineDelimiters = new char[] { Global_class.Tab };
            HeadlineDelimiters = new char[] { Global_class.Tab };

            Report = ReadWrite_report_enum.Report_everything;
        }
    }

    class Homology_char_class
    {
        public Organism_enum[] Organisms { get; private set; }

        public Homology_char_class(Organism_enum[] organisms)
        {
            Organisms = organisms;
        }
    }

    class Homology_line_class
    {
        public int GeneNr { get; set; }
        public string Symbol_0 { get; set; }
        public int GeneID_0 { get; set; }
        public SpeciesSwitch_enum Origin_0 { get; set; }
        public string Symbol_1 { get; set; }
        public int GeneID_1 { get; set; }
        public SpeciesSwitch_enum Origin_1 { get; set; }
    }

    class Homology_class : IDisposable
    {
        public List<Homology_line_class> Ortho { get; set; }
        public Homology_char_class Char { get; set; }

        public Homology_class(Organism_enum[] organisms)
        {
            if (organisms.Length != 2) { throw new Exception(); }
            Char = new Homology_char_class(organisms);
            Ortho = new List<Homology_line_class>();
        }

        #region Generate
        private void Fill_with_homologene()
        {
            Report_class.WriteLine("{0}: Fill with Homologene from {1} and {2}", typeof(Homology_class).Name, Char.Organisms[0], Char.Organisms[1]);
            Homologene_class species_0_homo = new Homologene_class(Char.Organisms[0]);
            species_0_homo.ReadRawData();
            Homologene_class species_1_homo = new Homologene_class(Char.Organisms[1]);
            species_1_homo.ReadRawData();
            species_0_homo.Data = species_0_homo.Data.OrderBy(l => l.GeneNr).ToList();
            species_1_homo.Data = species_1_homo.Data.OrderBy(l => l.GeneNr).ToList();
            int species_0_homo_count = species_0_homo.Data.Count();
            int species_1_homo_count = species_1_homo.Data.Count();
            int index_1 = 0;
            int index_1_reset = 0;
            int intCompare;
            int lines_added = 0;
            for (int index_0 = 0; index_0 < species_0_homo_count; index_0++)
            {
                intCompare = -2;
                index_1 = index_1_reset;
                while ((index_1 < species_1_homo_count) && (intCompare <= 0))
                {
                    intCompare = species_1_homo.Data[index_1].GeneNr - species_0_homo.Data[index_0].GeneNr;
                    if (intCompare < 0)
                    {
                        index_1++;
                        index_1_reset = index_1;
                    }
                    else if (intCompare == 0)
                    {
                        Homology_line_class line = new Homology_line_class();
                        line.GeneNr = species_0_homo.Data[index_0].GeneNr;
                        line.Symbol_0 = species_0_homo.Data[index_0].Symbol;
                        line.GeneID_0 = species_0_homo.Data[index_0].GeneID;
                        line.Origin_0 = SpeciesSwitch_enum.Homologene;
                        line.GeneNr = species_1_homo.Data[index_1].GeneNr;
                        line.Symbol_1 = species_1_homo.Data[index_1].Symbol;
                        line.GeneID_1 = species_1_homo.Data[index_1].GeneID;
                        line.Origin_1 = SpeciesSwitch_enum.Homologene;
                        Ortho.Add(line);
                        lines_added++;
                        index_1++;
                    }
                }
            }
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} lines added", lines_added);
        }
        private void Fill_with_jackson_orthology_since_2018June01()
        {
            Report_class.WriteLine("{0}: Fill with MGI data from {1} and {2}", typeof(Homology_class).Name, Char.Organisms[0], Char.Organisms[1]);
            Mgi_homologene_2018June_class mgi = new Mgi_homologene_2018June_class();
            mgi.Generate();
            mgi.Mgi_homologene = mgi.Mgi_homologene.OrderBy(l => l.DB_class_key).ToArray();
            int mgi_length = mgi.Mgi_homologene.Length;
            Mgi_homologene_2018June01_line_class mgi_ortho_line;
            List<string> symbol_organism0 = new List<string>();
            List<int> geneID_organism0 = new List<int>();
            List<string> symbol_organism1 = new List<string>();
            List<int> geneID_organism1 = new List<int>();
            int lines_added = 0;
            int in_both_organisms_multiple_orthologes_count = 0;
            for (int indexMgi = 0; indexMgi < mgi_length; indexMgi++)
            {
                mgi_ortho_line = mgi.Mgi_homologene[indexMgi];
                if ((indexMgi == 0) || (!mgi_ortho_line.DB_class_key.Equals(mgi.Mgi_homologene[indexMgi - 1].DB_class_key)))
                {
                    symbol_organism0.Clear();
                    geneID_organism0.Clear();
                    symbol_organism1.Clear();
                    geneID_organism1.Clear();
                }
                if (mgi_ortho_line.Ncbi_taxon_id == (int)Char.Organisms[0])
                {
                    symbol_organism0.Add((string)mgi_ortho_line.Symbol.Clone());
                    geneID_organism0.Add(mgi_ortho_line.EntrezGene_ID);
                }
                if (mgi_ortho_line.Ncbi_taxon_id == (int)Char.Organisms[1])
                {
                    symbol_organism1.Add((string)mgi_ortho_line.Symbol.Clone());
                    geneID_organism1.Add(mgi_ortho_line.EntrezGene_ID);
                }
                if ((indexMgi == mgi_length - 1) || (!mgi_ortho_line.DB_class_key.Equals(mgi.Mgi_homologene[indexMgi + 1].DB_class_key)))
                {
                    int symbol_organism0_count = symbol_organism0.Count;
                    int symbol_organism1_count = symbol_organism1.Count;
                    if ((symbol_organism0_count > 0) && (symbol_organism1_count > 0))
                    {
                        if ((symbol_organism0_count > 1) && (symbol_organism1_count > 1))
                        {
                            in_both_organisms_multiple_orthologes_count++;
                        }
                        for (int indexS0 = 0; indexS0 < symbol_organism0_count; indexS0++)
                        {
                            for (int indexS1 = 0; indexS1 < symbol_organism1_count; indexS1++)
                            {
                                Homology_line_class newOrthoLine = new Homology_line_class();
                                newOrthoLine.GeneID_0 = geneID_organism0[indexS0];
                                newOrthoLine.GeneID_1 = geneID_organism1[indexS1];
                                newOrthoLine.Symbol_0 = (string)symbol_organism0[indexS0].Clone();
                                newOrthoLine.Symbol_1 = (string)symbol_organism1[indexS1].Clone();
                                newOrthoLine.Origin_0 = SpeciesSwitch_enum.Jackson_labs;
                                newOrthoLine.Origin_1 = SpeciesSwitch_enum.Jackson_labs;
                                Ortho.Add(newOrthoLine);
                                lines_added++;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} lines added", lines_added);
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} x more than 1 orthologes in both organism", in_both_organisms_multiple_orthologes_count);
            if (lines_added == 0)
            {
                throw new Exception();
            }
        }
        private void Remove_duplicates()
        {
            Report_class.WriteLine("{0}: Remove duplicates", typeof(Homology_class).Name);
            SpeciesSwitch_enum[] switchPriority = new SpeciesSwitch_enum[] { SpeciesSwitch_enum.Jackson_labs, SpeciesSwitch_enum.Homologene };
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("Priority: {0} > {1}", switchPriority[0], switchPriority[1]);
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("Switch direction: {0} -> {1}", Char.Organisms[0], Char.Organisms[1]);

            Ortho = Ortho.OrderBy(l => l.Symbol_0).ThenBy(l => l.Origin_0).ToList();
            int ortho_count = Ortho.Count;
            int indexFirstSymbolDup = -1;
            List<SpeciesSwitch_enum> origins = new List<SpeciesSwitch_enum>();
            SpeciesSwitch_enum keepOrigin = SpeciesSwitch_enum.E_m_p_t_y;
            int addedHomologene = 0;
            int addedJackson = 0;


            List<Homology_line_class> newOrtho = new List<Homology_line_class>();
            Homology_line_class line;
            for (int indexOrtho = 0; indexOrtho < ortho_count; indexOrtho++)
            {
                line = Ortho[indexOrtho];
                if ((indexOrtho == 0) || (!line.Symbol_0.Equals(Ortho[indexOrtho - 1].Symbol_0)))
                {
                    indexFirstSymbolDup = indexOrtho;
                    origins.Clear();
                }
                if ((indexOrtho == 0) || (!line.Symbol_0.Equals(Ortho[indexOrtho - 1].Symbol_0)) || (!line.Origin_0.Equals(Ortho[indexOrtho - 1].Origin_0)))
                {
                    origins.Add(line.Origin_0);
                }
                if ((indexOrtho == ortho_count - 1) || (!line.Symbol_0.Equals(Ortho[indexOrtho + 1].Symbol_0)))
                {
                    for (int indexP = 0; indexP < switchPriority.Length; indexP++)
                    {
                        if (origins.Contains(switchPriority[indexP]))
                        {
                            keepOrigin = switchPriority[indexP];
                            break;
                        }
                    }
                    for (int indexAdd = indexFirstSymbolDup; indexAdd <= indexOrtho; indexAdd++)
                    {
                        if (Ortho[indexAdd].Origin_0 == keepOrigin)
                        {
                            newOrtho.Add(Ortho[indexAdd]);
                            switch (keepOrigin)
                            {
                                case SpeciesSwitch_enum.Homologene:
                                    addedHomologene++;
                                    break;
                                case SpeciesSwitch_enum.Jackson_labs:
                                    addedJackson++;
                                    break;
                                default:
                                    throw new Exception();
                            }
                        }
                    }
                }
            }
            Ortho = newOrtho;
            Report_class.WriteLine("{0}: {1} of {2} lines kept", typeof(Homology_class).Name, Ortho.Count, ortho_count);
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} lines from {1}", addedJackson, SpeciesSwitch_enum.Jackson_labs);
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} lines from {1}", addedHomologene, SpeciesSwitch_enum.Homologene);
        }
        private void Remove_unfilled_symbols()
        {
            Report_class.WriteLine("{0}: Remove lines with unfilled symbols", typeof(Homology_class).Name);
            List<Homology_line_class> newOrtho = new List<Homology_line_class>();
            int emptyHomo_count = 0;
            int emptyMGI_count = 0;
            int totalHomo_count = 0;
            int totalMGI_count = 0;
            foreach (Homology_line_class line in Ortho)
            {
                switch (line.Origin_0)
                {
                    case SpeciesSwitch_enum.Homologene:
                        totalHomo_count++;
                        break;
                    case SpeciesSwitch_enum.Jackson_labs:
                        totalMGI_count++;
                        break;
                    default:
                        throw new Exception();
                }
                if ((line.Symbol_0 != null) && (line.Symbol_1 != null))
                {
                    newOrtho.Add(line);
                }
                else
                {
                    switch (line.Origin_0)
                    {
                        case SpeciesSwitch_enum.Homologene:
                            emptyHomo_count++;
                            break;
                        case SpeciesSwitch_enum.Jackson_labs:
                            emptyMGI_count++;
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }
            Report_class.WriteLine("{0}: {1} of {2} lines kept", typeof(Homology_class).Name, newOrtho.Count, Ortho.Count);
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} of {1} MGI lines with empty symbols removed", emptyMGI_count, totalMGI_count);
            for (int i = 0; i < typeof(Homology_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} of {1} homologene lines with empty symbols removed", emptyHomo_count, totalHomo_count);
            Ortho = newOrtho;
        }
        private void Set_all_symbols_0_and_1_to_upper_case()
        {
            foreach (Homology_line_class homologene_line in this.Ortho)
            {
                homologene_line.Symbol_0 = homologene_line.Symbol_0.ToUpper();
                homologene_line.Symbol_1 = homologene_line.Symbol_1.ToUpper();
            }
        }
        public void Generate_new_and_write()
        {
            Report_class.WriteLine("-------------------------------------------------------------------------------");
            Report_class.WriteLine("{0}: Generate file for switching from {1} to {2}", typeof(Homology_class).Name, Char.Organisms[0], Char.Organisms[1]);
            Report_class.WriteLine();
            Ortho.Clear();

            Organism_enum[] jackson_organisms = new Organism_enum[] { Organism_enum.Homo_sapiens, Organism_enum.Mus_musculus, Organism_enum.Rattus_norvegicus };

            if ((jackson_organisms.Contains(Char.Organisms[0])) && (jackson_organisms.Contains(Char.Organisms[1])))
            {
                Fill_with_jackson_orthology_since_2018June01();
            }
            Fill_with_homologene();
            Set_all_symbols_0_and_1_to_upper_case();
            Remove_unfilled_symbols();
            Remove_duplicates();
            Write_file();
            Report_class.WriteLine("-------------------------------------------------------------------------------");
            Report_class.WriteLine();
        }
        public void Generate_by_reading_file()
        {
            Report_class.WriteLine("{0}: Generate file for switching from {1} to {2} by reading file", typeof(Homology_class).Name, Char.Organisms[0], Char.Organisms[1]);
            Read_file();
        }
        #endregion

        #region Order
        public void Order_by_symbol_0()
        {
            Ortho = Ortho.OrderBy(l => l.Symbol_0).ToList();
        }
        #endregion

        #region Get
        public string[] Get_all_symbols_of_organism(Organism_enum organism)
        {
            int zero_or_first_symbol = -1;
            if (Char.Organisms[0] == organism)
            {
                zero_or_first_symbol = 0;
            }
            else if (Char.Organisms[1] == organism)
            {
                zero_or_first_symbol = 1;
            }
            else
            {
                throw new Exception();
            }

            int ortho_count = Ortho.Count;
            string[] all_symbols = new string[ortho_count];
            Homology_line_class line;
            for (int indexOrtho = 0; indexOrtho < ortho_count; indexOrtho++)
            {
                line = Ortho[indexOrtho];
                switch (zero_or_first_symbol)
                {
                    case 0:
                        all_symbols[indexOrtho] = (string)line.Symbol_0.Clone();
                        break;
                    case 1:
                        all_symbols[indexOrtho] = (string)line.Symbol_1.Clone();
                        break;
                    default:
                        break;
                }
            }
            return all_symbols.Distinct().ToArray();
        }
        #endregion

        #region Read write
        public void Write_file()
        {
            Homology_readWriteOptions_class ReadWrite_options = new Homology_readWriteOptions_class(Char);
            ReadWriteClass.WriteData<Homology_line_class>(Ortho, ReadWrite_options);
        }
        public void Read_file()
        {
            Homology_readWriteOptions_class ReadWriteOptions = new Homology_readWriteOptions_class(Char);
            Ortho = ReadWriteClass.ReadRawData_and_FillList<Homology_line_class>(ReadWriteOptions);
        }
        #endregion

        #region Add homologene
        private T Add_homologene_to_line<T>(T inputLine, Homology_line_class homoLine) where T : IUse_homology<T>
        {
            inputLine.Symbol_1 = homoLine.Symbol_1;
            inputLine.GeneID_1 = homoLine.GeneID_1;
            inputLine.Origin_1 = homoLine.Origin_1;
            return inputLine;
        }
        private T Add_simple_transfere_to_line<T>(T inputLine) where T : IUse_homology<T>
        {
            inputLine.Origin_1 = SpeciesSwitch_enum.Simple_transfer;
            switch (Char.Organisms[1])
            {
                case Organism_enum.Homo_sapiens:
                    inputLine.Symbol_1 = inputLine.Symbol_0.ToUpper();
                    break;
                case Organism_enum.Mus_musculus:
                    inputLine.Symbol_1 = char.ToUpper(inputLine.Symbol_0[0]) + inputLine.Symbol_0.ToLower().Substring(1);
                    break;
                default:
                    throw new Exception();
            }
            return inputLine;
        }
        public List<T> Add_homologene<T>(List<T> inputData) where T : IUse_homology<T>
        {
            Report_class.WriteLine("{0}: original Organism: {1}", typeof(T).Name, Char.Organisms[0]);
            for (int i = 0; i < typeof(T).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("Add homologues of {0}", Char.Organisms[1]);

            inputData = inputData.OrderBy(l => l.Symbol_0).ToList();
            Ortho = Ortho.OrderBy(l => l.Symbol_0).ToList();

            int inputData_count = inputData.Count;
            List<T> newInputData = new List<T>();
            int ortho_count = Ortho.Count;
            int indexOrtho = 0;
            int indexOrtho_restart = 0;
            int stringCompare = 0;
            T inputLine;
            Homology_line_class orthoLine = new Homology_line_class();
            int MGI_count = 0;
            int homo_count = 0;
            int simple_count = 0;
            int first_symbol_count = 0;
            bool first_symbol = true;
            bool inputLineAdded;
            for (int indexInput = 0; indexInput < inputData_count; indexInput++)
            {
                inputLine = inputData[indexInput];
                inputLineAdded = false;
                if (indexOrtho_restart < ortho_count)
                {
                    stringCompare = -2;
                    indexOrtho = indexOrtho_restart;
                    while ((indexOrtho < ortho_count) && (stringCompare <= 0))
                    {
                        orthoLine = Ortho[indexOrtho];
                        stringCompare = orthoLine.Symbol_0.CompareTo(inputLine.Symbol_0);
                        if (stringCompare < 0)
                        {
                            indexOrtho++;
                            indexOrtho_restart = indexOrtho;
                            first_symbol = true;
                        }
                        else if (stringCompare == 0)
                        {
                            T newLine = inputLine.Deep_copy();
                            newLine = Add_homologene_to_line<T>(newLine, orthoLine);
                            newInputData.Add(newLine);
                            inputLineAdded = true;
                            switch (orthoLine.Origin_1)
                            {
                                case SpeciesSwitch_enum.Homologene:
                                    homo_count++;
                                    break;
                                case SpeciesSwitch_enum.Jackson_labs:
                                    MGI_count++;
                                    break;
                                default:
                                    throw new Exception();
                            }
                            if (first_symbol)
                            {
                                if ((indexInput == 0) || (!inputLine.Symbol_0.Equals(inputData[indexInput - 1].Symbol_0)))
                                {
                                    first_symbol_count++;
                                }
                                first_symbol = false;
                            }
                            indexOrtho++;
                        }
                    }
                }
                else
                {
                    stringCompare = 2;
                }
                if ((stringCompare > 0) && (!inputLineAdded))
                {
                    inputLine = Add_simple_transfere_to_line<T>(inputLine);
                    newInputData.Add(inputLine);
                    simple_count++;
                    inputLineAdded = true;
                }
            }
            for (int i = 0; i < typeof(T).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} lines --> {1} lines", inputData_count, newInputData.Count);
            for (int i = 0; i < typeof(T).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} orthologes from {1}", MGI_count, SpeciesSwitch_enum.Jackson_labs);
            for (int i = 0; i < typeof(T).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} orthologes from {1}", homo_count, SpeciesSwitch_enum.Homologene);
            for (int i = 0; i < typeof(T).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} unique symbols among these 2", first_symbol_count);
            for (int i = 0; i < typeof(T).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} orthologes from {1}", simple_count, SpeciesSwitch_enum.Simple_transfer);
            if (newInputData.Count < inputData.Count)
            {
                throw new Exception();
            }
            return newInputData;
        }
        #endregion

        public void Dispose()
        {
            Ortho.Clear();
            Ortho = null;
            Char = null;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////

}
