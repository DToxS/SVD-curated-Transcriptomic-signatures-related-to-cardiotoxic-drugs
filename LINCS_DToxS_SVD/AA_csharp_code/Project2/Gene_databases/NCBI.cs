using Accord;
using Common_classes;
using ReadWrite;
using Statistic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gene_databases
{
    enum RefSeq_accepted_accessionNumberType_enum { E_m_p_t_y, OnlyCurated_allTypes, OnlyCurated_onlymRNA, Accept_everything, All_onlymRNA }
    enum RefSeq_rnaType_type_enum { E_m_p_t_y, Mrna, Non_coding_rna, Protein }


    enum Origin_of_ncbi_official_symbol_annotation_enum { E_m_p_t_y, Ncbi_symbol_was_set_based_on_gtf_refseqgeneid_matching_ncbi_refseqgeneid, Ncbi_symbol_was_set_based_on_gtf_refseqgeneid_matching_ncbi_synonym, Ncbi_symbol_is_copy_of_gtf_refseqgeneid, No_match_found }

    class Gene2RefSeq_readOptions_class : ReadWriteOptions_base
    {
        public Gene2RefSeq_readOptions_class(params Organism_enum[] organisms)
        {
            File = Global_directory_class.GeneDatabases_homology_download_directory + "gene2refseq.txt";
            Key_propertyNames = new string[] { "TaxID", "GeneID", "RNA_nucleotide_accession_status", "RNA_nucleotide_accession", "RefSeqGene" };
            Key_columnNames = new string[] { "#tax_id", "GeneID", "status", "RNA_nucleotide_accession.version", "Symbol" };
            Key_columnIndexes = null;

            SafeCondition_columnIndexes = null;
            SafeCondition_columnNames = new string[organisms.Length];
            SafeCondition_entries = new string[organisms.Length];
            for (int i = 0; i < organisms.Length; i++)
            {
                SafeCondition_columnNames[i] = "#tax_id";
                SafeCondition_entries[i] = ((int)organisms[i]).ToString();
            }

            File_has_headline = true;
            RemoveFromHeadline = new string[0];// { "#Format:" };
            LineDelimiters = new char[] { Global_class.Tab };
            HeadlineDelimiters = new char[] { Global_class.Tab };

            Report = ReadWrite_report_enum.Report_everything;
        }

    }

    class Gene2RefSeq_line_class
    {
        public Organism_enum TaxID { get; set; }
        public int GeneID { get; set; }
        public Gene2RefSeq_accession_enum RNA_nucleotide_accession_status { get; set; }
        public string RNA_nucleotide_accession { get; set; }
        public string RefSeqGene { get; set; }

        public Gene2RefSeq_line_class()
        {
        }
    }

    class Gene2RefSeq_class
    {
        public List<Gene2RefSeq_line_class> Data { get; set; }
        public Organism_enum[] Organisms { get; set; }

        public Gene2RefSeq_class(params Organism_enum[] organisms)
        {
            Organisms = organisms;
        }
        
        public Dictionary<string, List<int>> Get_refSeqGene_geneIds_dict()
        {
            Dictionary<string, List<int>> refSeqGene_geneIds_dict = new Dictionary<string, List<int>>();
            foreach (Gene2RefSeq_line_class gene2RefSeq_line in Data)
            {
                if (!refSeqGene_geneIds_dict.ContainsKey(gene2RefSeq_line.RefSeqGene))
                {
                    refSeqGene_geneIds_dict.Add(gene2RefSeq_line.RefSeqGene, new List<int>());
                }
                refSeqGene_geneIds_dict[gene2RefSeq_line.RefSeqGene].Add(gene2RefSeq_line.GeneID);
                refSeqGene_geneIds_dict[gene2RefSeq_line.RefSeqGene] = refSeqGene_geneIds_dict[gene2RefSeq_line.RefSeqGene].Distinct().ToList();
            }
            return refSeqGene_geneIds_dict;
        }

        public void ReadRawData()
        {
            Gene2RefSeq_readOptions_class ReadOptions = new Gene2RefSeq_readOptions_class(Organisms);
            Data = ReadWriteClass.ReadRawData_and_FillList<Gene2RefSeq_line_class>(ReadOptions);
        }

    }

    //////////////////////////////////////////////////////////////////////////////////////

    class GeneInfo_readOptions_class : ReadWriteOptions_base
    {
        const char synonym_delimiter = '|';

        public static char Synonym_delimiter { get { return synonym_delimiter; } }

        public GeneInfo_readOptions_class(params Organism_enum[] organisms)
        {
            File = Global_directory_class.GeneDatabases_homology_download_directory + "All_Data_gene_info.txt";
            Key_propertyNames = new string[] { "TaxID", "GeneID", "Symbol", "LocusTag", "Synonyms", "Description", "dbXrefs" };
            Key_columnNames = new string[] { "#tax_id", "GeneID", "Symbol", "LocusTag", "Synonyms", "description", "dbXrefs" };
            Key_columnIndexes = null;

            File_has_headline = true;
            RemoveFromHeadline = new string[0];// { "#", "Format:" };

            SafeCondition_columnNames = new string[organisms.Length];
            SafeCondition_columnIndexes = null;
            SafeCondition_entries = new string[organisms.Length];
            for (int i = 0; i < organisms.Length; i++)
            {
                SafeCondition_columnNames[i] = "#tax_id";
                SafeCondition_entries[i] = ((int)organisms[i]).ToString();
            }

            LineDelimiters = new char[] { Global_class.Tab };
            HeadlineDelimiters = new char[] { Global_class.Tab };

            Report = ReadWrite_report_enum.Report_everything;
        }
    }

    class GeneInfo_line_class
    {
        public Organism_enum TaxID { get; set; }
        public int GeneID { get; set; }
        public string Synonyms { get; set; }
        public string Symbol { get; set; }
        public string LocusTag { get; set; }
        public string Description { get; set; }
        public string Ensemble { get; set; }
        public string dbXrefs { get; set; }

        public GeneInfo_line_class()
        {
        }

    }

    class GeneInfo_class
    {
        const char separator = '|';
        const string empty_sign = "-";
        public List<GeneInfo_line_class> Data { get; set; }
        Organism_enum[] Organisms { get; set; }
        public char Separator { get { return separator; } }
        public string Empty_sign { get { return empty_sign; } }

        public GeneInfo_class(params Organism_enum[] organisms)
        {
            Organisms = organisms;
        }

        #region Order
        protected void Order_by_locusTag()
        {
            Data = Data.OrderBy(l => l.LocusTag).ToList();
        }
        #endregion

        public void ReadRawData()
        {
            GeneInfo_readOptions_class ReadOptions = new GeneInfo_readOptions_class(Organisms);
            Data = ReadWriteClass.ReadRawData_and_FillList<GeneInfo_line_class>(ReadOptions);
        }

        public Dictionary<int,string> Get_geneID_ncbiGeneSymbol_dict()
        {
            Dictionary<int, string> geneID_ncbiGeneSymbol_dict = new Dictionary<int, string>();
            Dictionary<string, int> refSeqGene_geneId_dict = new Dictionary<string, int>();
            foreach (GeneInfo_line_class geneInfo_line in Data)
            {
                if (!geneID_ncbiGeneSymbol_dict.ContainsKey(geneInfo_line.GeneID))
                {
                    geneID_ncbiGeneSymbol_dict.Add(geneInfo_line.GeneID, geneInfo_line.Symbol);
                }
                else if (!geneID_ncbiGeneSymbol_dict[geneInfo_line.GeneID].Equals(geneInfo_line.Symbol)) { throw new Exception(); }
            }
            return geneID_ncbiGeneSymbol_dict;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////
    class Synonym_readWriteOptions_class : ReadWriteOptions_base
    {
        public Synonym_readWriteOptions_class(Organism_enum organism)
        {
            File = Global_directory_class.GeneDatabases_homology_self_directory + "gene_synonym_" + organism + "_" + ".txt";
            Key_propertyNames = new string[] { "Symbol", "Description", "GeneId", "Synonym", "Organism", "Synonym_is_also_symbol" };
            Key_columnNames = Key_propertyNames;
            Key_columnIndexes = null;

            SafeCondition_columnIndexes = null;
            SafeCondition_columnNames = null;
            SafeCondition_entries = null;

            File_has_headline = true;
            RemoveFromHeadline = null;
            LineDelimiters = new char[] { Global_class.Tab };
            HeadlineDelimiters = new char[] { Global_class.Tab };

            Report = ReadWrite_report_enum.Report_everything;
        }
    }

    class Synonym_line_class
    {
        #region
        public string Symbol { get; set; }
        public int GeneId { get; set; }
        public string Synonym { get; set; }
        public string Description { get; set; }
        public Organism_enum Organism { get; set; }
        public bool Synonym_is_also_symbol { get; set; }
        #endregion

        public Synonym_line_class()
        {
            GeneId = -1;
        }

        public Synonym_line_class Deep_copy()
        {
            Synonym_line_class copy = (Synonym_line_class)this.MemberwiseClone();
            copy.Symbol = (string)this.Symbol.Clone();
            copy.Synonym = (string)this.Synonym.Clone();
            copy.Description = (string)this.Description.Clone();
            return copy;
        }
    }

    class Synonym_class : IDisposable
    {
        public Synonym_line_class[] Sdata { get; set; }
        public Organism_enum Organism { get; set; }

        public Synonym_class(Organism_enum organism)
        {
            Organism = organism;
        }

        #region Generate de novo and write
        private void Fill_synonyms_array(GeneInfo_class geneInfo)
        {
            int geneInfo_length = geneInfo.Data.Count;
            List<Synonym_line_class> sdata_list = new List<Synonym_line_class>();
            Synonym_line_class sdata_line;
            GeneInfo_line_class geneInfo_line;
            string[] synonyms;
            int synonyms_length;
            for (int indexG = 0; indexG < geneInfo_length; indexG++)
            {
                geneInfo_line = geneInfo.Data[indexG];
                synonyms = geneInfo_line.Synonyms.Split(GeneInfo_readOptions_class.Synonym_delimiter);
                synonyms_length = synonyms.Length;
                for (int indexS = 0; indexS < synonyms_length; indexS++)
                {
                    sdata_line = new Synonym_line_class();
                    sdata_line.Symbol = (string)geneInfo_line.Symbol.Clone();
                    sdata_line.Synonym = (string)synonyms[indexS].Clone();
                    sdata_line.Description = (string)geneInfo_line.Description.Clone();
                    sdata_line.GeneId = geneInfo_line.GeneID;
                    sdata_line.Organism = Organism;
                    sdata_list.Add(sdata_line);
                }
            }
            Sdata = sdata_list.ToArray();
        }
        private void Mark_synonyms_which_are_also_symbols()
        {
            string[] all_symbols = Get_all_symbols_ordered();
            int all_symbols_length = all_symbols.Length;
            int stringCompare;
            int sdata_length = Sdata.Length;
            int indexAS = 0;
            Order_by_synonym();
            Synonym_line_class sd_line;
            for (int indexSD = 0; indexSD < sdata_length; indexSD++)
            {
                sd_line = Sdata[indexSD];
                stringCompare = -2;
                while ((indexAS < all_symbols_length) && (stringCompare < 0))
                {
                    stringCompare = all_symbols[indexAS].CompareTo(sd_line.Synonym);
                    if (stringCompare < 0)
                    {
                        indexAS++;
                    }
                    else if (stringCompare == 0)
                    {
                        sd_line.Synonym_is_also_symbol = true;
                    }
                }
            }
        }
        private void Write()
        {
            Synonym_readWriteOptions_class readWriteOpitons = new Synonym_readWriteOptions_class(Organism);
            ReadWriteClass.WriteData<Synonym_line_class>(Sdata, readWriteOpitons);
        }
        public void Generate_de_novo_and_write()
        {
            GeneInfo_class geneInfo = new GeneInfo_class(Organism);
            geneInfo.ReadRawData();
            Fill_synonyms_array(geneInfo);
            Mark_synonyms_which_are_also_symbols();
            Write();
        }
        #endregion

        public void Generate_by_reading_safed_file()
        {
            Synonym_readWriteOptions_class readWriteOptions = new Synonym_readWriteOptions_class(Organism);
            Sdata = ReadWriteClass.ReadRawData_and_FillArray<Synonym_line_class>(readWriteOptions);
        }

        #region Get
        public string[] Get_all_symbols_ordered()
        {
            List<string> all_symbols_list = new List<string>();
            Order_by_symbol();
            int sdata_length = Sdata.Length;
            Synonym_line_class sd_line;
            for (int indexSD = 0; indexSD < sdata_length; indexSD++)
            {
                sd_line = Sdata[indexSD];
                if ((indexSD == 0) || (!sd_line.Symbol.Equals(Sdata[indexSD - 1].Symbol)))
                {
                    all_symbols_list.Add(sd_line.Symbol);
                }
            }
            return all_symbols_list.OrderBy(l => l).ToArray();
        }
        #endregion

        #region Order
        public void Order_by_synonym_symbol()
        {
            Sdata = Sdata.OrderBy(l => l.Synonym).ThenBy(l => l.Symbol).ToArray();
        }
        public void Order_by_symbol()
        {
            Sdata = Sdata.OrderBy(l => l.Symbol).ToArray();
        }
        public void Order_by_synonym()
        {
            Sdata = Sdata.OrderBy(l => l.Synonym).ToArray();
        }
        #endregion

        public void Dispose()
        {
            this.Sdata = null;
            this.Organism = Organism_enum.E_m_p_t_y;
        }

        #region Copy
        public Synonym_class Deep_copy()
        {
            Synonym_class copy = (Synonym_class)this.MemberwiseClone();
            int data_length = this.Sdata.Length;
            copy.Sdata = new Synonym_line_class[data_length];
            for (int indexS = 0; indexS < data_length; indexS++)
            {
                copy.Sdata[indexS] = this.Sdata[indexS].Deep_copy();
            }
            return copy;
        }
        #endregion

    }

    //////////////////////////////////////////////////////////////////////////////////////


    class NcbiRefSeq_lincs_line_class
    {
        public string UCSC_refSeqGeneId { get; set; }
        public string UCSC_geneName { get; set; }
        public int GeneId { get; set; }
        public string NCBI_refSeqGeneId { get; set; }
        public string NCBI_symbol { get; set; }

        public bool More_than_one_NCBI_symbol_per_ucsc_refSeqGeneID { get; set; }
        public string[] Transcript_ids { get; set; }

        public RefSeq_rnaType_type_enum RefSeq_rnaType { get; set; }

        public int Count_of_matches_for_particular_symbol { get; set; }

        public Origin_of_ncbi_official_symbol_annotation_enum Origin_of_ncbi_official_symbol_annotation { get; set; }

        public string ReadWrite_transcript_ids
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Transcript_ids, NcbiRefSeq_lincs_readWriteOptions_class.Array_delimiter); }
            set { Transcript_ids = ReadWriteClass.Get_array_from_readLine<string>(value, NcbiRefSeq_lincs_readWriteOptions_class.Array_delimiter); }
        }

        public NcbiRefSeq_lincs_line_class()
        {
            UCSC_geneName = "";
            UCSC_refSeqGeneId = "";
            NCBI_refSeqGeneId = "";
            NCBI_symbol = "";
            Transcript_ids = new string[0];
            More_than_one_NCBI_symbol_per_ucsc_refSeqGeneID = false;
            Origin_of_ncbi_official_symbol_annotation = Origin_of_ncbi_official_symbol_annotation_enum.No_match_found;
        }

        public NcbiRefSeq_lincs_line_class Deep_copy()
        {
            NcbiRefSeq_lincs_line_class copy = (NcbiRefSeq_lincs_line_class)this.MemberwiseClone();
            copy.UCSC_refSeqGeneId = (string)this.UCSC_refSeqGeneId.Clone();
            copy.NCBI_refSeqGeneId = (string)this.NCBI_refSeqGeneId.Clone();
            copy.NCBI_symbol = (string)this.NCBI_symbol.Clone();
            copy.Transcript_ids = Array_class.Deep_copy_string_array(this.Transcript_ids);
            return copy;
        }
    }

    class NcbiRefSeq_lincs_readWriteOptions_class : ReadWriteOptions_base
    {
        public static char Array_delimiter { get { return ';'; } }

        public NcbiRefSeq_lincs_readWriteOptions_class()
        {
            this.File = Global_directory_class.GeneDatabases_homology_self_directory + "ncbiRefSeq.All.txt";
            this.Key_propertyNames = new string[] { "ReadWrite_transcript_ids", "UCSC_refSeqGeneId", "NCBI_refSeqGeneId", "UCSC_geneName", "GeneId", "NCBI_symbol", "RefSeq_rnaType", "Origin_of_ncbi_official_symbol_annotation", "More_than_one_NCBI_symbol_per_ucsc_refSeqGeneID", "Count_of_matches_for_particular_symbol" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class NcbiRefSeq_lincs_download_readWriteOptions_class : ReadWriteOptions_base
    {
        public int Gene_id_columnIndex { get; set; }
        public int Transcript_id_columnIndex { get; set; }
        public char Intra_column_delimiter { get; set; }

        public NcbiRefSeq_lincs_download_readWriteOptions_class()
        {
            this.File = Global_directory_class.GeneDatabases_homology_download_directory + "ncbiRefSeq.All.hg38.gtf";
            this.Report = ReadWrite_report_enum.Report_main;
            Gene_id_columnIndex = 8;
            Intra_column_delimiter = ';';
            this.LineDelimiters = new char[] { Global_class.Tab };
        }

    }

    class NcbiRefSeq_lincs_options_class
    {
        public RefSeq_accepted_accessionNumberType_enum RefSeq_accepted_accesionNumberType { get; set; }

        public NcbiRefSeq_lincs_options_class()
        {
            RefSeq_accepted_accesionNumberType = RefSeq_accepted_accessionNumberType_enum.All_onlymRNA;
        }
    }

    class NcbiRefSeq_lincs_class : IDisposable
    {
        #region Fields
        public NcbiRefSeq_lincs_line_class[] NcbiRefSeq { get; set; }
        public NcbiRefSeq_lincs_options_class Options { get; set; }
        #endregion
        public NcbiRefSeq_lincs_class()
        {
            Options = new NcbiRefSeq_lincs_options_class();
        }

        private void Add_to_array(NcbiRefSeq_lincs_line_class[] add_ncbiRefSeq)
        {
            int this_length = this.NcbiRefSeq.Length;
            int add_length = add_ncbiRefSeq.Length;
            int new_length = this_length + add_length;
            NcbiRefSeq_lincs_line_class[] new_ncbiRefSeq = new NcbiRefSeq_lincs_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_ncbiRefSeq[indexNew] = this.NcbiRefSeq[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_ncbiRefSeq[indexNew] = add_ncbiRefSeq[indexAdd];
            }
            this.NcbiRefSeq = new_ncbiRefSeq;
        }

        #region Generate de novo and write
        private void Remove_duplicates_based_on_ucsc_refSeqGeneId_and_rnaType_and_combine_transcriptIDs()
        {
            Report_class.WriteLine("{0}: Remove duplicates based on UCSC refSeqGeneIDs and combine transcriptIDs", typeof(NcbiRefSeq_lincs_class).Name);
            NcbiRefSeq_lincs_line_class ncbi_line;
            List<NcbiRefSeq_lincs_line_class> keep = new List<NcbiRefSeq_lincs_line_class>();
            List<string> new_transcript_ids = new List<string>();
            int ncbi_length = NcbiRefSeq.Length;
            NcbiRefSeq = NcbiRefSeq.OrderBy(l => l.UCSC_refSeqGeneId).ThenBy(l => l.RefSeq_rnaType).ThenBy(l => l.Transcript_ids[0]).ToArray();
            for (int indexNCBI = 0; indexNCBI < ncbi_length; indexNCBI++)
            {
                ncbi_line = this.NcbiRefSeq[indexNCBI];
                if ((indexNCBI == 0)
                    || (!ncbi_line.RefSeq_rnaType.Equals(NcbiRefSeq[indexNCBI - 1].RefSeq_rnaType))
                    || (!ncbi_line.UCSC_refSeqGeneId.Equals(NcbiRefSeq[indexNCBI - 1].UCSC_refSeqGeneId)))
                {
                    new_transcript_ids.Clear();
                }
                if ((indexNCBI != 0)
                    && (ncbi_line.Transcript_ids[0].Equals(this.NcbiRefSeq[indexNCBI - 1].Transcript_ids[0])))
                {
                    throw new Exception();
                }
                if (ncbi_line.Transcript_ids.Length != 1)
                {
                    throw new Exception();
                }
                new_transcript_ids.AddRange(ncbi_line.Transcript_ids);
                if ((indexNCBI == ncbi_length - 1)
                    || (!ncbi_line.RefSeq_rnaType.Equals(NcbiRefSeq[indexNCBI + 1].RefSeq_rnaType))
                    || (!ncbi_line.UCSC_refSeqGeneId.Equals(NcbiRefSeq[indexNCBI + 1].UCSC_refSeqGeneId)))
                {
                    ncbi_line.Transcript_ids = new_transcript_ids.Distinct().OrderBy(l => l).ToArray();
                    keep.Add(ncbi_line);
                }

            }
            this.NcbiRefSeq = keep.ToArray();
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} of {1} lines kept", keep.Count, ncbi_length);
        }
        private void Add_refSeq_rnaType_and_check_if_all_transcriptIDs_have_same_refSeq_rnaType()
        {
            NcbiRefSeq_lincs_line_class refSeq_line;
            int ncbi_length = this.NcbiRefSeq.Length;
            string[] transcriptIDs;
            string transcriptID;
            int transcriptIDs_length;
            RefSeq_rnaType_type_enum rnaType;
            RefSeq_rnaType_type_enum current_transcript_rnaType;
            for (int indexN = 0; indexN < ncbi_length; indexN++)
            {
                refSeq_line = this.NcbiRefSeq[indexN];
                transcriptIDs = refSeq_line.Transcript_ids;
                transcriptIDs_length = transcriptIDs.Length;
                rnaType = RefSeq_rnaType_type_enum.E_m_p_t_y;
                for (int indexT = 0; indexT < transcriptIDs_length; indexT++)
                {
                    transcriptID = transcriptIDs[indexT];
                    if ((transcriptID.IndexOf("XM") == 0) || (transcriptID.IndexOf("NM") == 0))
                    {
                        current_transcript_rnaType = RefSeq_rnaType_type_enum.Mrna;
                        if (rnaType.Equals(RefSeq_rnaType_type_enum.E_m_p_t_y)) { rnaType = current_transcript_rnaType; }
                        else if (!rnaType.Equals(current_transcript_rnaType)) { throw new Exception(); }
                    }
                    else if ((transcriptID.IndexOf("XR") == 0) || (transcriptID.IndexOf("NR") == 0))
                    {
                        current_transcript_rnaType = RefSeq_rnaType_type_enum.Non_coding_rna;
                        if (rnaType.Equals(RefSeq_rnaType_type_enum.E_m_p_t_y)) { rnaType = current_transcript_rnaType; }
                        else if (!rnaType.Equals(current_transcript_rnaType)) { throw new Exception(); }
                    }
                    else if ((transcriptID.IndexOf("XP") == 0) || (transcriptID.IndexOf("NP") == 0) || (transcriptID.IndexOf("YP") == 0))
                    {
                        current_transcript_rnaType = RefSeq_rnaType_type_enum.Protein;
                        if (rnaType.Equals(RefSeq_rnaType_type_enum.E_m_p_t_y)) { rnaType = current_transcript_rnaType; }
                        else if (!rnaType.Equals(current_transcript_rnaType)) { throw new Exception(); }
                    }
                    else { throw new Exception(); }
                }
                refSeq_line.RefSeq_rnaType = rnaType;
            }
        }
        private void Remove_complete_duplicates_based_on_ucsc_geneName_ucsc_refSeqGeneId_first_transcriptId()
        {
            this.NcbiRefSeq = this.NcbiRefSeq.OrderBy(l => l.UCSC_geneName).ThenBy(l => l.UCSC_refSeqGeneId).ThenBy(l => l.Transcript_ids[0]).ToArray();
            int ncbiRefSeq_length = this.NcbiRefSeq.Length;
            NcbiRefSeq_lincs_line_class ncbiRefSeq_line;
            List<NcbiRefSeq_lincs_line_class> keep = new List<NcbiRefSeq_lincs_line_class>();
            for (int indexNcbi = 0; indexNcbi < ncbiRefSeq_length; indexNcbi++)
            {
                ncbiRefSeq_line = this.NcbiRefSeq[indexNcbi];
                if (ncbiRefSeq_line.Transcript_ids.Length != 1) { throw new Exception(); }
                if ((indexNcbi == 0)
                    || (!ncbiRefSeq_line.UCSC_geneName.Equals(this.NcbiRefSeq[indexNcbi - 1].UCSC_geneName))
                    || (!ncbiRefSeq_line.UCSC_refSeqGeneId.Equals(this.NcbiRefSeq[indexNcbi - 1].UCSC_refSeqGeneId))
                    || (!ncbiRefSeq_line.Transcript_ids[0].Equals(this.NcbiRefSeq[indexNcbi - 1].Transcript_ids[0])))
                {
                    keep.Add(ncbiRefSeq_line);
                }
            }
            this.NcbiRefSeq = keep.ToArray();
        }
        private void Add_geneIDs_by_matching_refGenes_from_gene2RefSeq()
        {
            Gene2RefSeq_class gene2RefSeq = new Gene2RefSeq_class(Organism_enum.Homo_sapiens);
            gene2RefSeq.ReadRawData();
            Dictionary<string, List<int>> refGene_geneID_dict = gene2RefSeq.Get_refSeqGene_geneIds_dict();
            int[] geneIDs;
            NcbiRefSeq_lincs_line_class new_refGene_lincs_line = new NcbiRefSeq_lincs_line_class();
            List<NcbiRefSeq_lincs_line_class> new_refGene_lincs_lines = new List<NcbiRefSeq_lincs_line_class>();
            foreach (NcbiRefSeq_lincs_line_class ncbi_line in this.NcbiRefSeq)
            {
                if (refGene_geneID_dict.ContainsKey(ncbi_line.UCSC_refSeqGeneId))
                {
                    geneIDs = refGene_geneID_dict[ncbi_line.UCSC_refSeqGeneId].ToArray();
                    ncbi_line.GeneId = geneIDs[0];
                    for (int indexGID =1;indexGID<geneIDs.Length;indexGID++)
                    {
                        new_refGene_lincs_line = ncbi_line.Deep_copy();
                        new_refGene_lincs_line.GeneId = geneIDs[indexGID];
                        new_refGene_lincs_lines.Add(new_refGene_lincs_line);
                    }
                }
                else
                {
                    ncbi_line.GeneId = -1;
                }
            }
            Add_to_array(new_refGene_lincs_lines.ToArray());
        }
        private void Add_ncbi_symbols_by_matching_geneIds_from_all_data_gene_info()
        {
            GeneInfo_class geneInfo = new GeneInfo_class(Organism_enum.Homo_sapiens);
            geneInfo.ReadRawData();
            Dictionary<int, string> geneID_ncbiGeneSymbol_dict = geneInfo.Get_geneID_ncbiGeneSymbol_dict();
            foreach (NcbiRefSeq_lincs_line_class ncbi_line in this.NcbiRefSeq)
            {
                if (geneID_ncbiGeneSymbol_dict.ContainsKey(ncbi_line.GeneId))
                {
                    ncbi_line.NCBI_symbol = geneID_ncbiGeneSymbol_dict[ncbi_line.GeneId];
                    ncbi_line.Origin_of_ncbi_official_symbol_annotation = Origin_of_ncbi_official_symbol_annotation_enum.Ncbi_symbol_was_set_based_on_gtf_refseqgeneid_matching_ncbi_refseqgeneid;
                }
                else
                {
                    ncbi_line.NCBI_symbol = Global_class.Empty_entry;
                    ncbi_line.Origin_of_ncbi_official_symbol_annotation = Origin_of_ncbi_official_symbol_annotation_enum.No_match_found;
                }
            }

        }
        private void Set_ucsc_refGeneId_as_geneSymbol_if_missing()
        {
            foreach (NcbiRefSeq_lincs_line_class ncbi_lincs_line in this.NcbiRefSeq)
            {
                if (ncbi_lincs_line.NCBI_symbol.Equals(Global_class.Empty_entry))
                {
                    ncbi_lincs_line.NCBI_symbol = (string)ncbi_lincs_line.UCSC_refSeqGeneId.Clone();
                }
            }
        }
        private void Add_NCBI_symbols_using_synonym_as_reference()
        {
            Report_class.WriteLine("{0}: Add NCBI symbols using synonym as reference, if synonym is only assigned to ONE symbol", typeof(NcbiRefSeq_lincs_class).Name);
            Synonym_class synonym = new Synonym_class(Organism_enum.Homo_sapiens);
            synonym.Generate_by_reading_safed_file();
            synonym.Order_by_synonym_symbol();

            int synonym_length = synonym.Sdata.Length;
            Synonym_line_class synonym_line;
            int indexS = 0;
            int stringCompare = -2;

            NcbiRefSeq = NcbiRefSeq.OrderBy(l => l.UCSC_refSeqGeneId).ToArray();
            int ncbiRefSeq_length = NcbiRefSeq.Length;

            int ncbi_refSeq_length = NcbiRefSeq.Length;
            NcbiRefSeq_lincs_line_class ncbi_lincs_line;
            int symbols_for_current_synonym_count = 0;
            bool symbol_added = false;
            List<string> no_unique_synonym_found = new List<string>();
            int symbol_added_count = 0;
            int total_tested_count = 0;
            for (int indexNCBI = 0; indexNCBI < ncbi_refSeq_length; indexNCBI++)
            {
                ncbi_lincs_line = NcbiRefSeq[indexNCBI];
                if ((indexNCBI == 0)
                    || (!ncbi_lincs_line.UCSC_refSeqGeneId.Equals(NcbiRefSeq[indexNCBI - 1].UCSC_refSeqGeneId)))
                {
                    if (ncbi_lincs_line.Origin_of_ncbi_official_symbol_annotation.Equals(Origin_of_ncbi_official_symbol_annotation_enum.No_match_found))
                    {
                        total_tested_count++;
                        symbol_added = false;
                        stringCompare = -2;
                        while ((indexS < synonym_length) && (stringCompare < 0))
                        {
                            synonym_line = synonym.Sdata[indexS];
                            symbols_for_current_synonym_count = 1;
                            while ((indexS < synonym_length - 1) && (synonym_line.Synonym.Equals(synonym.Sdata[indexS + 1].Synonym)))
                            {
                                symbols_for_current_synonym_count++;
                                indexS++;
                                synonym_line = synonym.Sdata[indexS];
                            }
                            if (symbols_for_current_synonym_count == 1)
                            {
                                stringCompare = synonym_line.Synonym.CompareTo(ncbi_lincs_line.UCSC_refSeqGeneId);
                                if (stringCompare < 0)
                                {
                                    indexS++;
                                }
                                else if (stringCompare == 0)
                                {
                                    ncbi_lincs_line.NCBI_symbol = (string)synonym_line.Symbol.Clone();
                                    ncbi_lincs_line.Origin_of_ncbi_official_symbol_annotation = Origin_of_ncbi_official_symbol_annotation_enum.Ncbi_symbol_was_set_based_on_gtf_refseqgeneid_matching_ncbi_synonym;
                                    symbol_added = true;
                                    symbol_added_count++;
                                }
                            }
                        }
                        if (!symbol_added) { no_unique_synonym_found.Add(ncbi_lincs_line.UCSC_refSeqGeneId); }
                    }
                }
            }
        }
        public void Generate_de_novo_and_write()
        {
            Read_download_data();
            Remove_complete_duplicates_based_on_ucsc_geneName_ucsc_refSeqGeneId_first_transcriptId();
            Add_refSeq_rnaType_and_check_if_all_transcriptIDs_have_same_refSeq_rnaType();
            Remove_duplicates_based_on_ucsc_refSeqGeneId_and_rnaType_and_combine_transcriptIDs();
            Add_geneIDs_by_matching_refGenes_from_gene2RefSeq();
            Add_ncbi_symbols_by_matching_geneIds_from_all_data_gene_info();
            Add_NCBI_symbols_using_synonym_as_reference();
            //Set_ucsc_refGeneId_as_geneSymbol_if_missing();
            Print_summary();
            Write();
        }
        #endregion

        #region Generate by reading saved file
        private void Remove_not_accepted_accessionNumberTypes()
        {
            Report_class.WriteLine("{0}: Remove not accepted accessionNumberTypes: Order: keep {0}", this.Options.RefSeq_accepted_accesionNumberType);
            int ncbi_length = NcbiRefSeq.Length;
            NcbiRefSeq_lincs_line_class ncbi_line;
            List<NcbiRefSeq_lincs_line_class> keep_ncbi_lines = new List<NcbiRefSeq_lincs_line_class>();
            int transcript_ids_length;
            string current_transcript_id;
            List<string> keep_transcript_ids = new List<string>();
            int input_transcript_ids_count = 0;
            int kept_transcript_ids_count = 0;
            for (int indexN = 0; indexN < ncbi_length; indexN++)
            {
                ncbi_line = this.NcbiRefSeq[indexN];
                transcript_ids_length = ncbi_line.Transcript_ids.Length;
                keep_transcript_ids.Clear();
                for (int indexT = 0; indexT < transcript_ids_length; indexT++)
                {
                    current_transcript_id = ncbi_line.Transcript_ids[indexT];
                    input_transcript_ids_count++;
                    switch (Options.RefSeq_accepted_accesionNumberType)
                    {
                        case RefSeq_accepted_accessionNumberType_enum.All_onlymRNA:
                            if ((current_transcript_id.IndexOf("NM") == 0)
                                || (current_transcript_id.IndexOf("XM") == 0))
                            {
                                keep_transcript_ids.Add(current_transcript_id);
                            }
                            else if (current_transcript_id.IndexOf("NP") == 0) { }
                            else if (current_transcript_id.IndexOf("NR") == 0) { }
                            else if (current_transcript_id.IndexOf("XR") == 0) { }
                            else if (current_transcript_id.IndexOf("YP") == 0) { }
                            else
                            {
                                throw new Exception();
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                }
                if (keep_transcript_ids.Count > 0)
                {
                    kept_transcript_ids_count += keep_transcript_ids.Count;
                    ncbi_line.Transcript_ids = keep_transcript_ids.ToArray();
                    keep_ncbi_lines.Add(ncbi_line);
                }
            }
            this.NcbiRefSeq = keep_ncbi_lines.ToArray();
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} of {1} transcriptIds kept", kept_transcript_ids_count, input_transcript_ids_count);
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} of {1} ncbi lines kept", this.NcbiRefSeq.Length, ncbi_length);
        }
        public void Generate_by_reading_safed_file()
        {
            Read();
            Remove_not_accepted_accessionNumberTypes();
        }
        #endregion

        #region Get
        public string[] Get_all_ordered_UCSCrefSeqGeneIDs()
        {
            int ncbi_length = this.NcbiRefSeq.Length;
            NcbiRefSeq_lincs_line_class ncbi_line;
            this.NcbiRefSeq = this.NcbiRefSeq.OrderBy(l => l.UCSC_refSeqGeneId).ToArray();
            List<string> all_ucsc_refSeqGeneIds = new List<string>();
            for (int indexNCBI = 0; indexNCBI < ncbi_length; indexNCBI++)
            {
                ncbi_line = this.NcbiRefSeq[indexNCBI];
                if ((indexNCBI == 0)
                    || (!ncbi_line.UCSC_refSeqGeneId.Equals(this.NcbiRefSeq[indexNCBI - 1].UCSC_refSeqGeneId)))
                {
                    all_ucsc_refSeqGeneIds.Add(ncbi_line.UCSC_refSeqGeneId);
                }
            }
            return all_ucsc_refSeqGeneIds.ToArray();
        }
        public string[] Get_all_ordered_ncbi_symbols()
        {
            int ncbi_length = this.NcbiRefSeq.Length;
            NcbiRefSeq_lincs_line_class ncbi_line;
            this.NcbiRefSeq = this.NcbiRefSeq.OrderBy(l => l.NCBI_symbol).ToArray();
            List<string> all_ncbi_symbols = new List<string>();
            for (int indexNCBI = 0; indexNCBI < ncbi_length; indexNCBI++)
            {
                ncbi_line = this.NcbiRefSeq[indexNCBI];
                if ((indexNCBI == 0)
                    || (!ncbi_line.NCBI_symbol.Equals(this.NcbiRefSeq[indexNCBI - 1].NCBI_symbol)))
                {
                    all_ncbi_symbols.Add(ncbi_line.NCBI_symbol);
                }
            }
            return all_ncbi_symbols.ToArray();
        }
        public string[] Get_all_ordered_ncbi_symbols_of_input_refSeq_annotation(RefSeq_rnaType_type_enum rnaType)
        {
            int ncbi_length = this.NcbiRefSeq.Length;
            NcbiRefSeq_lincs_line_class ncbi_line;
            this.NcbiRefSeq = this.NcbiRefSeq.OrderBy(l => l.NCBI_symbol).ToArray();
            List<string> all_ncbi_symbols = new List<string>();
            for (int indexNCBI = 0; indexNCBI < ncbi_length; indexNCBI++)
            {
                ncbi_line = this.NcbiRefSeq[indexNCBI];
                if ((indexNCBI == 0)
                    || (!ncbi_line.NCBI_symbol.Equals(this.NcbiRefSeq[indexNCBI - 1].NCBI_symbol)))
                {
                    if (ncbi_line.RefSeq_rnaType.Equals(rnaType))
                    {
                        all_ncbi_symbols.Add(ncbi_line.NCBI_symbol);
                    }
                }
            }
            return all_ncbi_symbols.ToArray();
        }

        public string[] Get_all_ordered_ncbi_symbols_that_are_only_associated_with_input_refSeq_annotation(RefSeq_rnaType_type_enum rnaType)
        {
            int ncbi_length = this.NcbiRefSeq.Length;
            NcbiRefSeq_lincs_line_class ncbi_line;
            this.NcbiRefSeq = this.NcbiRefSeq.OrderBy(l => l.NCBI_symbol).ToArray();
            List<RefSeq_rnaType_type_enum> rnaTypes_of_current_symbol = new List<RefSeq_rnaType_type_enum>();
            List<string> symbols = new List<string>();
            for (int indexNCBI = 0; indexNCBI < ncbi_length; indexNCBI++)
            {
                ncbi_line = this.NcbiRefSeq[indexNCBI];
                if ((indexNCBI == 0)
                    || (!ncbi_line.NCBI_symbol.Equals(this.NcbiRefSeq[indexNCBI - 1].NCBI_symbol)))
                {
                    rnaTypes_of_current_symbol.Clear();
                }
                rnaTypes_of_current_symbol.Add(ncbi_line.RefSeq_rnaType);
                if ((indexNCBI == ncbi_length - 1)
                    || (!ncbi_line.NCBI_symbol.Equals(this.NcbiRefSeq[indexNCBI + 1].NCBI_symbol)))
                {
                    if ((rnaTypes_of_current_symbol.Distinct().ToArray().Length == 1) && (ncbi_line.RefSeq_rnaType.Equals(rnaType)))
                    {
                        symbols.Add(ncbi_line.NCBI_symbol);
                    }
                }
            }
            return symbols.ToArray();
        }
        #endregion

        public void Dispose()
        {
            this.NcbiRefSeq = null;
            this.Options = null;
        }

        #region Read write copy print summary
        private void Print_summary()
        {
            Report_class.WriteLine("{0}: Print summary", typeof(NcbiRefSeq_lincs_class).Name);

            #region Count and print origin of ncbi official symbol annotations
            int gtf_refSeqGeneId_matches_with_annotated_ncbi_refSeqGeneId_count = 0;
            int gtf_refSeqGeneId_matches_with_ncbi_synonym_count = 0;
            int gtf_refSeqGeenId_has_no_match_count = 0;
            int ncbi_symbol_is_copy_of_gtf_refSeqGeneId_count = 0;
            this.NcbiRefSeq = this.NcbiRefSeq.OrderBy(l => l.NCBI_symbol).ThenBy(l => l.UCSC_refSeqGeneId).ToArray();
            int ncbiRefSeq_length = this.NcbiRefSeq.Length;
            NcbiRefSeq_lincs_line_class line;
            NcbiRefSeq_lincs_line_class inner_line;
            int max_considered_counts_of_geneIDs_per_symbol = 1000;
            int[] count_of_symbols_with_array_position_geneIDs = new int[max_considered_counts_of_geneIDs_per_symbol];
            int geneIDs_of_currentsymbol_count = 0;
            int firstIndex_current_ncbi_symbol = 0;
            for (int indexNcbi = 0; indexNcbi < ncbiRefSeq_length; indexNcbi++)
            {
                line = this.NcbiRefSeq[indexNcbi];
                if ((indexNcbi == 0)
                    || (!line.NCBI_symbol.Equals(this.NcbiRefSeq[indexNcbi - 1].NCBI_symbol)))
                {
                    geneIDs_of_currentsymbol_count = 0;
                    firstIndex_current_ncbi_symbol = indexNcbi;
                }
                geneIDs_of_currentsymbol_count++;
                if ((indexNcbi != 0)
                    && (line.NCBI_symbol.Equals(this.NcbiRefSeq[indexNcbi - 1].NCBI_symbol))
                    && (line.UCSC_refSeqGeneId.Equals(this.NcbiRefSeq[indexNcbi - 1].UCSC_refSeqGeneId))
                    && (!String.IsNullOrEmpty(line.NCBI_symbol))
                    && (line.Origin_of_ncbi_official_symbol_annotation.Equals(this.NcbiRefSeq[indexNcbi - 1].Origin_of_ncbi_official_symbol_annotation)))
                {
                    // throw new Exception();
                }


                switch (line.Origin_of_ncbi_official_symbol_annotation)
                {
                    case Origin_of_ncbi_official_symbol_annotation_enum.No_match_found:
                        gtf_refSeqGeenId_has_no_match_count++;
                        break;
                    case Origin_of_ncbi_official_symbol_annotation_enum.Ncbi_symbol_is_copy_of_gtf_refseqgeneid:
                        ncbi_symbol_is_copy_of_gtf_refSeqGeneId_count++;
                        break;
                    case Origin_of_ncbi_official_symbol_annotation_enum.Ncbi_symbol_was_set_based_on_gtf_refseqgeneid_matching_ncbi_refseqgeneid:
                        gtf_refSeqGeneId_matches_with_annotated_ncbi_refSeqGeneId_count++;
                        break;
                    case Origin_of_ncbi_official_symbol_annotation_enum.Ncbi_symbol_was_set_based_on_gtf_refseqgeneid_matching_ncbi_synonym:
                        gtf_refSeqGeneId_matches_with_ncbi_synonym_count++;
                        break;
                    default:
                        throw new Exception();
                }
                if ((indexNcbi == ncbiRefSeq_length - 1)
                    || (!line.NCBI_symbol.Equals(this.NcbiRefSeq[indexNcbi + 1].NCBI_symbol)))
                {
                    if (!line.Origin_of_ncbi_official_symbol_annotation.Equals(Origin_of_ncbi_official_symbol_annotation_enum.No_match_found))
                    {
                        count_of_symbols_with_array_position_geneIDs[geneIDs_of_currentsymbol_count]++;
                        for (int indexInner = firstIndex_current_ncbi_symbol; indexInner <= indexNcbi; indexInner++)
                        {
                            inner_line = NcbiRefSeq[indexInner];
                            inner_line.Count_of_matches_for_particular_symbol = geneIDs_of_currentsymbol_count;
                        }
                    }
                }
            }
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0}: {1}", Origin_of_ncbi_official_symbol_annotation_enum.Ncbi_symbol_was_set_based_on_gtf_refseqgeneid_matching_ncbi_refseqgeneid, gtf_refSeqGeneId_matches_with_annotated_ncbi_refSeqGeneId_count);
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0}: {1}", Origin_of_ncbi_official_symbol_annotation_enum.Ncbi_symbol_was_set_based_on_gtf_refseqgeneid_matching_ncbi_synonym, gtf_refSeqGeneId_matches_with_ncbi_synonym_count);
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0}: {1}", Origin_of_ncbi_official_symbol_annotation_enum.Ncbi_symbol_is_copy_of_gtf_refseqgeneid, ncbi_symbol_is_copy_of_gtf_refSeqGeneId_count);
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0}: {1}", Origin_of_ncbi_official_symbol_annotation_enum.No_match_found, gtf_refSeqGeenId_has_no_match_count);
            for (int indexC = 1; indexC < max_considered_counts_of_geneIDs_per_symbol; indexC++)
            {
                if (count_of_symbols_with_array_position_geneIDs[indexC] != 0)
                {
                    for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                    Report_class.WriteLine("{0} NCBI symbols have {1} NCBI gene ids", count_of_symbols_with_array_position_geneIDs[indexC], indexC);
                }
            }
            #endregion

            #region Count how many RefSeq geneIDs are associated with multiple rnaTypes
            this.NcbiRefSeq = this.NcbiRefSeq.OrderBy(l => l.UCSC_refSeqGeneId).ThenBy(l => l.RefSeq_rnaType).ToArray();
            int[] count_of_rnaTypes = new int[4];
            int count_of_rnaTypes_of_current_refSeqGeneId = 0;
            for (int indexNcbi = 0; indexNcbi < ncbiRefSeq_length; indexNcbi++)
            {
                line = this.NcbiRefSeq[indexNcbi];
                if ((indexNcbi == 0)
                    || (!line.UCSC_refSeqGeneId.Equals(this.NcbiRefSeq[indexNcbi - 1].UCSC_refSeqGeneId)))
                {
                    count_of_rnaTypes_of_current_refSeqGeneId = 0;
                }
                if ((indexNcbi != 0)
                    && (line.UCSC_refSeqGeneId.Equals(this.NcbiRefSeq[indexNcbi - 1].UCSC_refSeqGeneId))
                    && (line.RefSeq_rnaType.Equals(this.NcbiRefSeq[indexNcbi - 1].RefSeq_rnaType)))
                {
                    throw new Exception();
                }
                count_of_rnaTypes_of_current_refSeqGeneId++;
                if ((indexNcbi == ncbiRefSeq_length - 1)
                    || (!line.UCSC_refSeqGeneId.Equals(this.NcbiRefSeq[indexNcbi + 1].UCSC_refSeqGeneId)))
                {
                    count_of_rnaTypes[count_of_rnaTypes_of_current_refSeqGeneId]++;
                }
            }

            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} UCSC refSeqGeneIds are associated with {1} refSeq rna types", count_of_rnaTypes[1], 1);
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} UCSC refSeqGeneIds are associated with {1} refSeq rna types", count_of_rnaTypes[2], 2);
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} UCSC refSeqGeneIds are associated with {1} refSeq rna types", count_of_rnaTypes[3], 3);
            #endregion
        }

        private void Read_download_data()
        {
            Report_class.WriteLine("{0}: Read download data", typeof(NcbiRefSeq_lincs_class).Name);
            NcbiRefSeq_lincs_download_readWriteOptions_class readWriteOptions = new NcbiRefSeq_lincs_download_readWriteOptions_class();
            string completeFileName = readWriteOptions.File;
            int indexGeneId = readWriteOptions.Gene_id_columnIndex;
            StreamReader reader = new StreamReader(completeFileName);
            char[] lineDelimiters = readWriteOptions.LineDelimiters;
            char intraColumnDelimiter = readWriteOptions.Intra_column_delimiter;
            string inputLine;
            string[] columnEntries;
            string[] ID_splitStrings;
            string geneID;
            string transcriptID;
            string geneName;
            string transcriptID_entry;
            string geneID_entry;
            string geneName_entry;
            string transcript_id_label = "transcript_id";
            string gene_id_label = "gene_id";
            string gene_name_label = "gene_name";
            NcbiRefSeq_lincs_line_class new_line;
            List<NcbiRefSeq_lincs_line_class> new_lines_list = new List<NcbiRefSeq_lincs_line_class>();
            int geneIDs_added_count = 0;
            int transript_IDs_added_count = 0;
            int geneNames_added_count = 0;
            while ((inputLine = reader.ReadLine()) != null)
            {
                columnEntries = inputLine.Split(lineDelimiters);
                new_line = new NcbiRefSeq_lincs_line_class();

                ID_splitStrings = columnEntries[indexGeneId].Split(intraColumnDelimiter);
                foreach (string ID_splitString in ID_splitStrings)
                {
                    if (ID_splitString.IndexOf(gene_id_label) != -1)
                    {
                        geneID_entry = ID_splitString;
                        geneID = geneID_entry.Replace("gene_id \"", "").Replace("\"", "");
                        new_line.UCSC_refSeqGeneId = Text2_class.Remove_space_comma_semicolon_colon_underline_from_end_and_beginning_of_text(geneID);
                        geneIDs_added_count++;
                    }
                    else if (ID_splitString.IndexOf(transcript_id_label) != -1)
                    {
                        transcriptID_entry = ID_splitString;
                        transcriptID = transcriptID_entry.Replace(" transcript_id \"", "").Replace("\"", "").Split('.')[0];
                        new_line.Transcript_ids = new string[] { Text2_class.Remove_space_comma_semicolon_colon_underline_from_end_and_beginning_of_text(transcriptID) };
                        transript_IDs_added_count++;
                    }
                    else if (ID_splitString.IndexOf(gene_name_label) != -1)
                    {
                        geneName_entry = ID_splitString;
                        geneName = geneName_entry.Replace(" gene_name \"", "").Replace("\"", "");
                        new_line.UCSC_geneName = Text2_class.Remove_space_comma_semicolon_colon_underline_from_end_and_beginning_of_text(geneName);
                        geneNames_added_count++;
                    }
                }
                new_lines_list.Add(new_line);
            }
            this.NcbiRefSeq = new_lines_list.ToArray();
            for (int i = 0; i < typeof(NcbiRefSeq_lincs_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} transcript IDs added, {1} geneIDs added, {2} geneNames_added, {3} total lines screened", transript_IDs_added_count, geneIDs_added_count, geneNames_added_count, this.NcbiRefSeq.Length);
        }

        private void Write()
        {
            NcbiRefSeq_lincs_readWriteOptions_class readWriteOptions = new NcbiRefSeq_lincs_readWriteOptions_class();
            ReadWriteClass.WriteData(this.NcbiRefSeq, readWriteOptions);
        }

        private void Read()
        {
            NcbiRefSeq_lincs_readWriteOptions_class readWriteOptions = new NcbiRefSeq_lincs_readWriteOptions_class();
            this.NcbiRefSeq = ReadWriteClass.ReadRawData_and_FillArray<NcbiRefSeq_lincs_line_class>(readWriteOptions);
        }
        #endregion
    }
}
