using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Common_classes;
using ReadWrite;
using Enrichment;

namespace Ontologies_and_GTEx
{
    class Drug_bank_private_input_readOptions
    {
        #region Fields
        private string Directory { get; set; }
        public string File {get;set;}
        #endregion

        public Drug_bank_private_input_readOptions()
        {
            Directory = Global_directory_class.Enrich_libraries_download_directory;
            File = Directory + "full database.xml";
        }
    }

    class Drug_bank_self_readWriteOptions_class : ReadWriteOptions_base
    {
        const char delimiter = ',';

        public static char Delimiter { get { return delimiter; } }

        public Drug_bank_self_readWriteOptions_class() 
        {
            File = Global_directory_class.Enrich_libraries_self_directory + "Drugbank_self.txt";

            Key_propertyNames = new string[] { "Ontology","Drug_name", "Indication", "Polypeptide_entity_name", "Polypeptide_entity_symbol", "Polypeptide_entity_organism", "ReadWrite_actions" };
            Key_columnNames = Key_propertyNames;
            File_has_headline = true;
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
        }
    }

    class Drug_bank_input_target_line_class
    {
        public string Entity_name { get; set; }
        public string Entity_organism { get; set; }
        public string[] Entity_actions { get; set; }
        public string Polypeptide_entity_name { get; set; }
        public string Polypeptide_entity_gene_name { get; set; }
        public string Polypeptide_entity_organism { get; set; }
        public Ontology_type_enum Ontology { get; set; }

        public Drug_bank_input_target_line_class()
        {
            Entity_actions = new string[] { Global_class.Empty_entry };
        }
    }

    class Drug_bank_line_class
    {
        #region Fields
        public string Drug_name { get; set; }
        public string Indication { get; set; }
        public string[] Actions {get;set;}
        public string Entity_organism_string { get; set; }
        public string Entity_name { get; set; }
        public string Polypeptide_entity_name { get; set; }
        public string Polypeptide_entity_symbol { get; set; }
        public string Polypeptide_entity_organism_string { get; set; }
        public Organism_enum Polypeptide_entity_organism { get; set; }
        public Ontology_type_enum Ontology { get; set; }

        public string ReadWrite_actions
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Actions, Drug_bank_self_readWriteOptions_class.Delimiter); }
            set { Actions = ReadWriteClass.Get_array_from_readLine<string>(value, Drug_bank_self_readWriteOptions_class.Delimiter); }
        }
        #endregion

        public Drug_bank_line_class()
        {
            Drug_name = "";
            Indication = "";
            Actions = new string[0];
            Entity_name = "";
            Entity_organism_string = "";
            Polypeptide_entity_name = "";
            Polypeptide_entity_organism_string = "";
            Polypeptide_entity_symbol = "";
        }

        public Drug_bank_line_class Deep_copy()
        {
            Drug_bank_line_class copy = (Drug_bank_line_class)this.MemberwiseClone();
            copy.Indication = (string)this.Indication.Clone();
            copy.Drug_name = (string)this.Drug_name.Clone();
            copy.Entity_name = (string)this.Entity_name.Clone();
            copy.Entity_organism_string = (string)this.Entity_organism_string.Clone();
            copy.Polypeptide_entity_name = (string)this.Polypeptide_entity_name.Clone();
            copy.Polypeptide_entity_symbol = (string)this.Polypeptide_entity_symbol.Clone();
            copy.Polypeptide_entity_organism_string = (string)this.Polypeptide_entity_organism_string.Clone();
            copy.Actions = Array_class.Deep_copy_string_array(this.Actions);
            return copy;
        }
    }

    class Drug_bank_class
    {
        public Drug_bank_line_class[] Drugs { get; set; }

        public Drug_bank_class()
        {
        }

        #region Generate
        private void Set_all_symbols_to_upperCase()
        {
            Report_class.WriteLine("{0}: Set all symbols to upper case", typeof(Drug_bank_class).Name);
            foreach (Drug_bank_line_class drug_line in Drugs)
            {
                drug_line.Polypeptide_entity_symbol = drug_line.Polypeptide_entity_symbol.ToUpper();
            }
        }
        private void Keep_only_human_organism_and_set_organism_enum()
        {
            Report_class.WriteLine("{0}: Keep only human or empty organisms and set organism enum", typeof(Drug_bank_class).Name);
            List<Drug_bank_line_class> new_drugs = new List<Drug_bank_line_class>();
            this.Drugs = this.Drugs.OrderBy(l => l.Entity_organism_string).ToArray();
            int drug_length = Drugs.Length;
            Drug_bank_line_class drug_line;
            List<string> removed_organism_strings = new List<string>();
            Organism_enum organism = Organism_enum.E_m_p_t_y;
            bool keep = false;
            for (int indexD=0; indexD< drug_length; indexD++)
            {
                drug_line = Drugs[indexD];
                organism = Organism_enum.E_m_p_t_y;
                //if ((indexD==0)||(!drug_line.Target_organism_string.Equals(Drugs[indexD-1].Target_organism_string)))
                {
                    switch (drug_line.Entity_organism_string)
                    {
                        case "Human":
                        case "Humans":
                        case "human":
                        case "humans":
                            keep = true;
                            organism = Organism_enum.Homo_sapiens;
                            break;
                        case "":
                            keep = false;
                            break;
                        default:
                            keep = false;
                            break;
                    }
                }
                if (keep)
                {
                    drug_line.Polypeptide_entity_organism = organism;
                    new_drugs.Add(drug_line);
                }
                else
                {
                    removed_organism_strings.Add(drug_line.Entity_organism_string);
                }
            }
            removed_organism_strings = removed_organism_strings.Distinct().OrderBy(l => l).ToList();
            Drugs = new_drugs.ToArray();
            for (int i = 0; i < typeof(Drug_bank_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} of {1} kept", Drugs.Length, drug_length);

            int removed_organisms_length = removed_organism_strings.Count;
            int organisms_per_line = 5;
            for (int indexRem = 0; indexRem < removed_organisms_length; indexRem++)
            {
                if (indexRem % organisms_per_line == 0)
                {
                    for (int i = 0; i < typeof(Drug_bank_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                }
                else
                {
                    Report_class.Write(",");
                }
                
                Report_class.Write(" {0}", removed_organism_strings[indexRem]);

                if (((indexRem + 1) % organisms_per_line == 0)
                    || (indexRem == removed_organisms_length - 1))
                {
                    Report_class.WriteLine();
                }
            }
        }
        private void Keep_only_lines_with_non_empty_symbols()
        {
            Report_class.WriteLine("{0}: Keep only lines with non empty symbols", typeof(Drug_bank_class).Name);
            List<Drug_bank_line_class> keep_drugs = new List<Drug_bank_line_class>();
            int drugs_length = Drugs.Length;
            foreach (Drug_bank_line_class drug_line in Drugs)
            {
                if (!drug_line.Polypeptide_entity_symbol.Equals(Global_class.Empty_entry))
                {
                    keep_drugs.Add(drug_line);
                }
            }
            Drugs = keep_drugs.ToArray();
            for (int i = 0; i < typeof(Drug_bank_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} of {1} kept", Drugs.Length, drugs_length);
        }
        private void Fill_symbols_with_name_if_empty()
        {
            foreach (Drug_bank_line_class drug_line in this.Drugs)
            {
                if (String.IsNullOrEmpty(drug_line.Polypeptide_entity_symbol))
                {
                    drug_line.Polypeptide_entity_symbol = (string)drug_line.Entity_name.Clone();
                }
            }
        }
        public void Generate_de_novo_by_reading_raw_data_and_write()
        {
            Read_raw_data();
            Keep_only_human_organism_and_set_organism_enum();
            Fill_symbols_with_name_if_empty();
            Keep_only_lines_with_non_empty_symbols();
            Set_all_symbols_to_upperCase();
            this.Drugs = this.Drugs.OrderBy(l => l.Drug_name).ThenBy(l => l.Polypeptide_entity_symbol).ToArray();
            Write_own_file();
        }
        public void Keep_only_indicated_ontology(Ontology_type_enum ontology)
        {
            int drugBank_length = this.Drugs.Length;
            Drug_bank_line_class drug_bank_line;
            List<Drug_bank_line_class> keep = new List<Drug_bank_line_class>();
            for (int indexDB = 0; indexDB < drugBank_length; indexDB++)
            {
                drug_bank_line = this.Drugs[indexDB];
                if (drug_bank_line.Ontology.Equals(ontology))
                {
                    keep.Add(drug_bank_line);
                }
            }
            this.Drugs = keep.ToArray();
        }
        public Ontology_library_line_class[] Generate_ontology_library_lines()
        {
            int drugBank_length = this.Drugs.Length;
            Drug_bank_line_class drug_bank_line;
            Ontology_library_line_class library_line;
            List<Ontology_library_line_class> new_library_lines = new List<Ontology_library_line_class>();
            for (int indexDB = 0; indexDB < drugBank_length; indexDB++)
            {
                drug_bank_line = this.Drugs[indexDB];
                library_line = new Ontology_library_line_class();
                library_line.Ontology = drug_bank_line.Ontology;
                library_line.Organism = drug_bank_line.Polypeptide_entity_organism;
                library_line.Level = -1;
                library_line.References = new string[0];
                library_line.Scp = (string)drug_bank_line.Drug_name.Clone();
                library_line.Target_gene_symbol = (string)drug_bank_line.Polypeptide_entity_symbol.Clone();
                library_line.Target_gene_score = 1;
                new_library_lines.Add(library_line);
            }
            return new_library_lines.ToArray();
        }
        #endregion

        #region Read write
        private List<Drug_bank_input_target_line_class> Generate_drug_bank_input_target_list_by_extracting_information_from_charateristics(XmlNode characteristicum, Ontology_type_enum ontology)
        {
            string polypeptide_entity_organism = "";
            string polypeptide_entity_name = "";
            string polypeptide_entity_gene_name = "";
            string entity_organism = "";
            string entity_name = "";
            List<string> entity_actions = new List<string>();
            bool line_generated;
            XmlNodeList current_entities;
            current_entities = characteristicum.ChildNodes;
            Drug_bank_input_target_line_class inner_entity_line;
            Drug_bank_input_target_line_class input_entity_line;
            List<Drug_bank_input_target_line_class> input_target_list = new List<Drug_bank_input_target_line_class>();
            foreach (XmlNode current_entity in current_entities)
            {
                line_generated = false;
                polypeptide_entity_name = "";
                polypeptide_entity_organism = "";
                polypeptide_entity_gene_name = "";
                entity_name = "";
                entity_organism = "";
                entity_actions.Clear();

                foreach (XmlNode target_characteristicum in current_entity)
                {
                    #region Add target name
                    if (target_characteristicum.Name.Equals("name"))
                    {
                        if (String.IsNullOrEmpty(entity_name))
                        {
                            entity_name = target_characteristicum.InnerText;
                        }
                        else
                        {
                            throw new Exception("not null or empty");
                        }
                    }
                    #endregion

                    #region Add target organism
                    if (target_characteristicum.Name.Equals("organism"))
                    {
                        if (String.IsNullOrEmpty(entity_organism))
                        {
                            entity_organism = target_characteristicum.InnerText;
                        }
                        else
                        {
                            throw new Exception("not null or empty");
                        }
                    }
                    #endregion

                    #region Add target actions
                    if (target_characteristicum.Name.Equals("actions"))
                    {
                        foreach (XmlNode action in target_characteristicum)
                        {
                            if (action.Name.Equals("action"))
                            {
                                entity_actions.Add(action.InnerText);
                            }
                        }
                    }
                    #endregion

                    if (target_characteristicum.Name.Equals("polypeptide"))
                    {
                        polypeptide_entity_name = "";
                        polypeptide_entity_organism = "";
                        polypeptide_entity_gene_name = "";
                        foreach (XmlNode target_polypeptide_char in target_characteristicum)
                        {

                            #region Add polypeptide gene name
                            if (target_polypeptide_char.Name.Equals("gene-name"))
                            {
                                if (String.IsNullOrEmpty(polypeptide_entity_gene_name))
                                {
                                    polypeptide_entity_gene_name = target_polypeptide_char.InnerText;
                                }
                                else
                                {
                                    throw new Exception("not null or empty");
                                }
                            }
                            #endregion

                            #region Add polypeptide organism
                            else if (target_polypeptide_char.Name.Equals("organism"))
                            {
                                if (String.IsNullOrEmpty(polypeptide_entity_organism))
                                {
                                    polypeptide_entity_organism = target_polypeptide_char.InnerText;
                                }
                                else
                                {
                                    throw new Exception("not null or empty");
                                }
                            }
                            #endregion

                            #region Add polypeptide name
                            else if (target_polypeptide_char.Name.Equals("name"))
                            {
                                if (String.IsNullOrEmpty(polypeptide_entity_name))
                                {
                                    polypeptide_entity_name = target_polypeptide_char.InnerText;
                                }
                                else
                                {
                                    throw new Exception("not null or empty");
                                }
                            }
                            #endregion
                        }
                        inner_entity_line = new Drug_bank_input_target_line_class();
                        inner_entity_line.Ontology = ontology;
                        inner_entity_line.Entity_name = entity_name;
                        inner_entity_line.Entity_organism = entity_organism;
                        if (entity_actions.Count > 0)
                        {
                            inner_entity_line.Entity_actions = entity_actions.ToArray();
                        }
                        else
                        {
                            inner_entity_line.Entity_actions = new string[] { Global_class.Empty_entry };
                        }
                        inner_entity_line.Polypeptide_entity_gene_name = (string)polypeptide_entity_gene_name.Clone();
                        inner_entity_line.Polypeptide_entity_name = (string)polypeptide_entity_name.Clone();
                        inner_entity_line.Polypeptide_entity_organism = (string)polypeptide_entity_organism.Clone();
                        if (String.IsNullOrEmpty(inner_entity_line.Entity_organism))
                        {
                            inner_entity_line.Entity_organism = (string)inner_entity_line.Polypeptide_entity_organism.Clone();
                        }
                        else if (inner_entity_line.Polypeptide_entity_organism.IndexOf(inner_entity_line.Entity_organism) == -1)
                        {
                            if (inner_entity_line.Polypeptide_entity_organism.ToUpper().IndexOf("HUMAN") != -1)
                            {
                                //  throw new Exception();
                            }
                        }

                        input_target_list.Add(inner_entity_line);

                        line_generated = true;
                    }
                }
                if (!line_generated)
                {
                    input_entity_line = new Drug_bank_input_target_line_class();
                    input_entity_line.Ontology = ontology;
                    input_entity_line.Entity_name = entity_name;
                    input_entity_line.Entity_organism = entity_organism;
                    if (entity_actions.Count > 0)
                    {
                        input_entity_line.Entity_actions = entity_actions.ToArray();
                    }
                    else
                    {
                        input_entity_line.Entity_actions = new string[] { Global_class.Empty_entry };
                    }
                    input_entity_line.Polypeptide_entity_gene_name = (string)polypeptide_entity_gene_name.Clone();
                    input_entity_line.Polypeptide_entity_name = (string)polypeptide_entity_name.Clone();
                    input_entity_line.Polypeptide_entity_organism = (string)polypeptide_entity_organism.Clone();
                    input_target_list.Add(input_entity_line);
                    line_generated = true;
                }
            }
            return input_target_list;
        }
        private void Read_raw_data()
        {
            Report_class.WriteLine("{0}: Read raw data", typeof(Drug_bank_class).Name);
            Drug_bank_private_input_readOptions readOptions = new Drug_bank_private_input_readOptions();
            XmlDocument drug_bank = new XmlDocument();
            drug_bank.Load(readOptions.File);

            XmlElement root = drug_bank.DocumentElement;
            XmlNodeList all_drugs = root.ChildNodes;
            XmlNodeList drug_characteristica;
            XmlNodeList current_entities;
            List<Drug_bank_line_class> drugs_list = new List<Drug_bank_line_class>();
            Drug_bank_line_class new_line;
            string indication = Global_class.Empty_entry;

            Drug_bank_input_target_line_class input_target_line;
            List<Drug_bank_input_target_line_class> input_target_list = new List<Drug_bank_input_target_line_class>();
            int input_target_count;

            List<string> act_drug_names_list = new List<string>();
            int names_count;
            List<string> act_drug_actions = new List<string>();
            //string target_name;
            //string target_organism;
            foreach (XmlNode drug in all_drugs)
            {
                act_drug_names_list.Clear();
                input_target_list.Clear();
                drug_characteristica = drug.ChildNodes;
                indication = null;
                foreach (XmlNode characteristicum in drug_characteristica)
                {
                    #region Add name
                    if (characteristicum.Name.Equals("name"))
                    {
                        act_drug_names_list.Add(characteristicum.InnerText);
                    }
                    #endregion

                    #region Add indication
                    else if (characteristicum.Name.Equals("indication"))
                    {
                        if (String.IsNullOrEmpty(indication))
                        {
                            indication = characteristicum.InnerText;
                        }
                        else
                        {
                            throw new Exception("not null or empty");
                        }
                    }
                    #endregion

                    else if (characteristicum.Name.Equals("targets"))
                    {
                        current_entities = characteristicum.ChildNodes;
                        input_target_list.AddRange(Generate_drug_bank_input_target_list_by_extracting_information_from_charateristics(characteristicum, Ontology_type_enum.Drugbank_drug_targets));
                    }
                    else if (characteristicum.Name.Equals("transporters"))
                    {
                        current_entities = characteristicum.ChildNodes;
                        input_target_list.AddRange(Generate_drug_bank_input_target_list_by_extracting_information_from_charateristics(characteristicum, Ontology_type_enum.Drugbank_transporters));
                    }
                    else if (characteristicum.Name.Equals("enzymes"))
                    {
                        current_entities = characteristicum.ChildNodes;
                        input_target_list.AddRange(Generate_drug_bank_input_target_list_by_extracting_information_from_charateristics(characteristicum, Ontology_type_enum.Drugbank_enzymes));
                    }
                }
                names_count = act_drug_names_list.Count;
                if (names_count != 1)
                {
                    throw new Exception("names count is not 1");
                }
                input_target_count = input_target_list.Count;
                for (int indexN = 0; indexN < names_count; indexN++)
                {
                    for (int indexPolyT = 0; indexPolyT < input_target_count; indexPolyT++)
                    {
                        input_target_line = input_target_list[indexPolyT];
                        new_line = new Drug_bank_line_class();
                        new_line.Drug_name = (string)act_drug_names_list[indexN].Clone();
                        new_line.Ontology = input_target_line.Ontology;
                        new_line.Indication = (string)indication.Clone();
                        new_line.Indication = new_line.Indication.Replace("\r", " ");
                        new_line.Indication = new_line.Indication.Replace("\n", " ");
                        new_line.Indication = new_line.Indication.Replace("\t", " ");
                        new_line.Entity_name = (string)input_target_line.Entity_name.Clone();
                        new_line.Entity_organism_string = (string)input_target_line.Entity_organism.Clone();
                        new_line.Actions = input_target_line.Entity_actions.ToArray();
                        if (!String.IsNullOrEmpty(input_target_line.Polypeptide_entity_name)) { new_line.Polypeptide_entity_name = (string)input_target_line.Polypeptide_entity_name.Clone(); }
                        if (!String.IsNullOrEmpty(input_target_line.Polypeptide_entity_gene_name)) { new_line.Polypeptide_entity_symbol = (string)input_target_line.Polypeptide_entity_gene_name.Clone(); }
                        if (!String.IsNullOrEmpty(input_target_line.Polypeptide_entity_organism)) { new_line.Polypeptide_entity_organism_string = (string)input_target_line.Polypeptide_entity_organism.Clone(); }
                        drugs_list.Add(new_line);
                    }
                }
            }
            Drugs = drugs_list.ToArray();
        }
        private void Write_own_file()
        {
            Drug_bank_self_readWriteOptions_class readWriteOptions = new Drug_bank_self_readWriteOptions_class();
            ReadWriteClass.WriteData(Drugs,readWriteOptions);
        }
        #endregion
    }

    ////////////////////////////////////////////////////////

}
