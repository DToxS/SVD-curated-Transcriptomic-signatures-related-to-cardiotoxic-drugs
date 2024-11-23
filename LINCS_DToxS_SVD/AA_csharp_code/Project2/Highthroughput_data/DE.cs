using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReadWrite;
using Common_classes;

namespace Highthroughput_data
{
    interface IFill_de
    {
        Timepoint_enum Timepoint_for_de { get; }
        string[] Symbols_for_de { get; }
        string[] Names_for_de { get; }
        double Value_for_de { get; }
        DE_entry_enum Entry_type_for_de { get; }
    }

    class Fill_de_line_class : IFill_de
    {
        public Timepoint_enum Timepoint_for_de { get; set; }
        public string[] Symbols_for_de { get; set; }
        public string[] Names_for_de { get; set; }
        public double Value_for_de { get; set; }
        public DE_entry_enum Entry_type_for_de { get; set; }

        public static bool Check_if_ordered { get { return true; } }

        public int Column { get; set; }

        public string Combined_names { get; set; }

        public string Combined_symbols { get; set; }

        public string ReadWrite_symbols_for_de
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Symbols_for_de, Fill_de_readWriteOptions.Delimiter); }
            set { this.Symbols_for_de = ReadWriteClass.Get_array_from_readLine<string>(value, Fill_de_readWriteOptions.Delimiter); }
        }

        public string ReadWrite_names_for_de
        {
            get { return ReadWriteClass.Get_writeLine_from_array(Names_for_de, Fill_de_readWriteOptions.Delimiter); }
            set { this.Names_for_de = ReadWriteClass.Get_array_from_readLine<string>(value, Fill_de_readWriteOptions.Delimiter); }
        }

        public Fill_de_line_class()
        {
            Symbols_for_de = new string[] { "" };
            Names_for_de = new string[] { "" };
            Combined_names = "";
            Combined_symbols = "";
        }

        #region Order
        public static Fill_de_line_class[] Order_by_entryType_timepoint_combinedNames(Fill_de_line_class[] fill_de_lines)
        {
            Dictionary<DE_entry_enum, Dictionary<Timepoint_enum, Dictionary<string, List<Fill_de_line_class>>>> entryType_timepoint_combinedNames_dict = new Dictionary<DE_entry_enum, Dictionary<Timepoint_enum, Dictionary<string, List<Fill_de_line_class>>>>();
            Dictionary<Timepoint_enum, Dictionary<string, List<Fill_de_line_class>>> timepoint_combinedNames_dict = new Dictionary<Timepoint_enum, Dictionary<string, List<Fill_de_line_class>>>();
            Dictionary<string, List<Fill_de_line_class>> combinedNames_dict = new Dictionary<string, List<Fill_de_line_class>>();
            int fill_de_lines_length = fill_de_lines.Length;
            Fill_de_line_class fill_de_line;
            for (int indexFill = 0; indexFill < fill_de_lines_length; indexFill++)
            {
                fill_de_line = fill_de_lines[indexFill];
                if (!entryType_timepoint_combinedNames_dict.ContainsKey(fill_de_line.Entry_type_for_de))
                {
                    entryType_timepoint_combinedNames_dict.Add(fill_de_line.Entry_type_for_de, new Dictionary<Timepoint_enum, Dictionary<string, List<Fill_de_line_class>>>());
                }
                if (!entryType_timepoint_combinedNames_dict[fill_de_line.Entry_type_for_de].ContainsKey(fill_de_line.Timepoint_for_de))
                {
                    entryType_timepoint_combinedNames_dict[fill_de_line.Entry_type_for_de].Add(fill_de_line.Timepoint_for_de, new Dictionary<string, List<Fill_de_line_class>>());
                }
                if (!entryType_timepoint_combinedNames_dict[fill_de_line.Entry_type_for_de][fill_de_line.Timepoint_for_de].ContainsKey(fill_de_line.Combined_names))
                {
                    entryType_timepoint_combinedNames_dict[fill_de_line.Entry_type_for_de][fill_de_line.Timepoint_for_de].Add(fill_de_line.Combined_names, new List<Fill_de_line_class>());
                }
                entryType_timepoint_combinedNames_dict[fill_de_line.Entry_type_for_de][fill_de_line.Timepoint_for_de][fill_de_line.Combined_names].Add(fill_de_line);
            }
            DE_entry_enum[] entryTypes = entryType_timepoint_combinedNames_dict.Keys.ToArray();
            DE_entry_enum entryType;
            int entryTypes_length = entryTypes.Length;
            Timepoint_enum[] timepoints;
            Timepoint_enum timepoint;
            int timepoints_length;
            string[] combinedNames;
            string combinedName;
            int combinedNames_length;
            List<Fill_de_line_class> ordered_fill_de_list = new List<Fill_de_line_class>();
            entryTypes = entryTypes.OrderBy(l => l).ToArray();
            for (int indexE = 0; indexE < entryTypes_length; indexE++)
            {
                entryType = entryTypes[indexE];
                timepoint_combinedNames_dict = entryType_timepoint_combinedNames_dict[entryType];
                timepoints = timepoint_combinedNames_dict.Keys.OrderBy(l => l).ToArray();
                timepoints = timepoints.OrderBy(l => l).ToArray();
                timepoints_length = timepoints.Length;
                for (int indexT = 0; indexT < timepoints_length; indexT++)
                {
                    timepoint = timepoints[indexT];
                    combinedNames_dict = timepoint_combinedNames_dict[timepoint];
                    combinedNames = combinedNames_dict.Keys.ToArray();
                    combinedNames = combinedNames.OrderBy(l => l).ToArray();
                    combinedNames_length = combinedNames.Length;
                    for (int indexC = 0; indexC < combinedNames_length; indexC++)
                    {
                        combinedName = combinedNames[indexC];
                        ordered_fill_de_list.AddRange(combinedNames_dict[combinedName]);
                    }
                }
            }

            if (Check_if_ordered)
            {
                #region Check if ordered
                int ordered_length = ordered_fill_de_list.Count;
                Fill_de_line_class previous_line;
                Fill_de_line_class current_line;
                for (int indexO = 1; indexO < ordered_length; indexO++)
                {
                    previous_line = ordered_fill_de_list[indexO - 1];
                    current_line = ordered_fill_de_list[indexO];
                    if (current_line.Entry_type_for_de.CompareTo(previous_line.Entry_type_for_de) < 0) { throw new Exception(); }
                    if ((current_line.Entry_type_for_de.Equals(previous_line.Entry_type_for_de))
                        && (current_line.Timepoint_for_de.CompareTo(previous_line.Timepoint_for_de) < 0)) { throw new Exception(); }
                    if ((current_line.Entry_type_for_de.Equals(previous_line.Entry_type_for_de))
                        && (current_line.Timepoint_for_de.Equals(previous_line.Timepoint_for_de))
                        && (current_line.Combined_names.CompareTo(previous_line.Combined_names) < 0)) { throw new Exception(); }
                }
            }
            #endregion
            return ordered_fill_de_list.ToArray();
        }
        #endregion


        private bool Equal_symbols(string[] other_symbols_for_de)
        {
            bool equals = true;
            int this_symbols_length = this.Symbols_for_de.Length;
            int other_symbols_length = other_symbols_for_de.Length;
            if (this_symbols_length != other_symbols_length)
            {
                equals = false;
            }
            else
            {
                for (int indexS = 0; indexS < this_symbols_length; indexS++)
                {
                    if (this.Symbols_for_de[indexS].Equals(other_symbols_for_de[indexS]))
                    {
                        equals = false;
                        break;
                    }
                }
            }
            return equals;
        }

        public void Set_combined_symbols_and_names()
        {
            int symbols_length = this.Symbols_for_de.Length;
            int names_length = this.Names_for_de.Length;
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            for (int indexS = 0; indexS < symbols_length; indexS++)
            {
                if (indexS != 0) { sb.AppendFormat("@"); }
                sb.AppendFormat("{0}", Symbols_for_de[indexS]);
            }
            this.Combined_symbols = sb.ToString();
            sb.Clear();
            for (int indexN = 0; indexN < names_length; indexN++)
            {
                if (indexN != 0) { sb.AppendFormat("@"); }
                sb.AppendFormat("{0}", Names_for_de[indexN]);
            }
            this.Combined_names = sb.ToString();
        }

        public bool Equals(Fill_de_line_class other)
        {
            return Equal_symbols(other.Symbols_for_de)
                    && this.Entry_type_for_de.Equals(other.Entry_type_for_de)
                    && this.Timepoint_for_de.Equals(other.Timepoint_for_de);
        }

        public Fill_de_line_class Deep_copy()
        {
            Fill_de_line_class copy = (Fill_de_line_class)this.MemberwiseClone();
            copy.Names_for_de = Array_class.Deep_copy_string_array(this.Names_for_de);
            copy.Symbols_for_de = Array_class.Deep_copy_string_array(this.Symbols_for_de);
            copy.Combined_names = (string)this.Combined_names.Clone();
            copy.Combined_symbols = (string)this.Combined_symbols.Clone();
            return copy;
        }
    }

    class Fill_de_readWriteOptions : ReadWriteOptions_base
    {
        public static char Delimiter { get { return ';'; } }

        public Fill_de_readWriteOptions(string subdirectory, string fileName)
        {
            string directory = Global_directory_class.Results_directory + subdirectory;
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "ReadWrite_names_for_de", "Entry_type_for_de", "Timepoint_for_de", "ReadWrite_symbols_for_de", "Value_for_de" };
            this.Key_columnNames = this.Key_propertyNames;
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.Report = ReadWrite_report_enum.Report_main;
            this.File_has_headline = true;
        }
    }

    class DE_columns_line_class
    {
        public double Value { get; set; }

        public DE_columns_line_class Deep_copy()
        {
            DE_columns_line_class copy = (DE_columns_line_class)this.MemberwiseClone();
            return copy;
        }
    }

    class DE_column_characterization_line_class
    {
        #region Fields
        public int Index { get; set; }
        public int IndexOld { get; set; }
        public string[] Names { get; set; }
        public Timepoint_enum Timepoint { get; set; }
        public DE_entry_enum EntryType { get; set; }

        public string Combined_names
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string name in Names)
                {
                    sb.AppendFormat(name);
                }
                return sb.ToString();
            }
        }

        public string ReadWrite_names
        {
            get { return ReadWriteClass.Get_writeLine_from_array(this.Names, ';'); }
            set { this.Names = ReadWriteClass.Get_array_from_readLine<string>(value, ';'); }
        }

        public string Full_column_label
        {
            get
            {
                StringBuilder name = new StringBuilder();
                bool first_add = true;
                if (!EntryType.Equals(DE_entry_enum.E_m_p_t_y))
                {
                    if (first_add)
                    {
                        first_add = false;
                        name.AppendFormat("{0}", EntryType);
                    }
                    else
                    {
                        name.AppendFormat("${0}", EntryType);
                    }
                }
                if (!Timepoint.Equals(Timepoint_enum.E_m_p_t_y))
                {
                    if (first_add)
                    {
                        first_add = false;
                        name.AppendFormat("{0}", Timepoint);
                    }
                    else
                    {
                        name.AppendFormat("${0}", Timepoint);
                    }
                }
                int names_length = Names.Length;
                for (int indexN = 0; indexN < names_length; indexN++)
                {
                    if (first_add)
                    {
                        name.AppendFormat("{0}", Names[indexN]);
                        first_add = false;
                    }
                    else
                    {
                        name.AppendFormat("${0}", Names[indexN]);
                    }
                }
                return name.ToString();
            }
            set
            {
                string[] splitStrings = value.Split('$');
                string splitString;
                int splitStrings_length = splitStrings.Length;
                splitString = splitStrings[0];
                EntryType = (DE_entry_enum)Enum.Parse(typeof(DE_entry_enum), splitString);
                splitString = splitStrings[1];
                Timepoint = (Timepoint_enum)Enum.Parse(typeof(Timepoint_enum), splitString);
                for (int indexS = 2; indexS < splitStrings_length; indexS++)
                {
                    splitString = splitStrings[indexS];
                    Names[indexS - 2] = splitString;
                }
            }

        }
        #endregion

        #region Lincs fields
        public string DrugType { get { return this.Names[0]; } }
        public string CellType { get { return this.Names[1]; } }
        public string Drug { get { return this.Names[2]; } }
        public string Plate { get { return this.Names[3]; } }
        #endregion

        public DE_column_characterization_line_class()
        {
            Names = new string[] { "" };
        }

        public Timepoint_enum Get_timepoint()
        {
            return Timepoint;
        }

        public DE_entry_enum Get_entryType()
        {
            return EntryType;
        }

        public string[] Get_names()
        {
            int names_length = Names.Length;
            string[] copy = new string[names_length];
            for (int indexN = 0; indexN < names_length; indexN++)
            {
                copy[indexN] = (string)Names[indexN].Clone();
            }
            return copy;
        }

        public string Get_full_column_name()
        {
            int names_length = Names.Length;
            StringBuilder name = new StringBuilder();
            for (int indexN = 0; indexN < names_length; indexN++)
            {
                if (indexN == 0)
                {
                    name.AppendFormat("{0}", Names[indexN]);
                }
                else
                {
                    name.AppendFormat("-{0}", Names[indexN]);
                }
            }
            return name.ToString();
        }

        public string Get_full_column_label()
        {
            StringBuilder name = new StringBuilder();
            bool first_add = true;
            if (EntryType != DE_entry_enum.E_m_p_t_y)
            {
                if (first_add)
                {
                    first_add = false;
                    name.AppendFormat("{0}", EntryType);
                }
                else
                {
                    name.AppendFormat("-{0}", EntryType);
                }
            }
            if (Timepoint != Timepoint_enum.E_m_p_t_y)
            {
                if (first_add)
                {
                    first_add = false;
                    name.AppendFormat("{0}", Timepoint);
                }
                else
                {
                    name.AppendFormat("-{0}", Timepoint);
                }
            }
            int names_length = Names.Length;
            if (first_add)
            {
                name.AppendFormat("{0}", Get_full_column_name());
            }
            else
            {
                name.AppendFormat("-{0}", Get_full_column_name());
            }
            return name.ToString();
        }

        public void Add_new_names(params string[] add_names)
        {
            int this_length = Names.Length;
            int add_length = add_names.Length;
            int new_length = this_length + add_length;
            string[] newNames = new string[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                newNames[indexNew] = (string)Names[indexThis].Clone();
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                newNames[indexNew] = (string)add_names[indexAdd].Clone();
            }
            this.Names = newNames;
        }

        public static DE_column_characterization_line_class[] Order_in_standard_way_for_equal_comparison(DE_column_characterization_line_class[] column_char_lines)
        {
            return column_char_lines.OrderBy(l => l.Timepoint).ThenBy(l => l.EntryType).ThenBy(l => l.Combined_names).ToArray();
        }

        public bool Equals_other(DE_column_characterization_line_class other)
        {
            bool equal = true;
            int this_names_length = this.Names.Length;
            int other_names_length = other.Names.Length;
            if ((!this.EntryType.Equals(other.EntryType))
                || (!this.Timepoint.Equals(other.Timepoint)))
            {
                equal = false;
            }
            else if (this_names_length != other_names_length)
            {
                equal = false;
            }
            else
            {
                for (int indexN = 0; indexN < this_names_length; indexN++)
                {
                    if (!this.Names[indexN].Equals(other.Names[indexN]))
                    {
                        equal = false;
                        break;
                    }
                }
            }
            return equal;
        }

        public DE_column_characterization_line_class Deep_copy()
        {
            DE_column_characterization_line_class copy = (DE_column_characterization_line_class)this.MemberwiseClone();
            copy.Names = Deep_copy_class.Deep_copy_string_array(this.Names);
            return copy;
        }
    }

    class DE_column_characterization_class
    {
        #region Fields
        public List<DE_column_characterization_line_class> Columns { get; set; }
        public bool Index_changes_adopted { get; set; }
        public bool Report { get; set; }
        #endregion

        #region Constructors
        public DE_column_characterization_class()
        {
            Columns = new List<DE_column_characterization_line_class>();
            Index_changes_adopted = true;
            Report = true;
        }
        #endregion

        #region Check
        public bool Small_correctness_check()
        {
            bool everything_correct = true;
            Report_class.Write("{0}: Small correctness check: ", typeof(DE_column_characterization_class).Name);
            if (!Index_changes_adopted)
            {
                everything_correct = false;
                throw new Exception();
            }
            if (everything_correct) { Report_class.WriteLine("OK"); }
            return everything_correct;
        }

        public bool Check_if_columns_are_differential_expression()
        {
            bool ok = true;
            foreach (DE_column_characterization_line_class line in Columns)
            {
                switch (line.EntryType)
                {
                    case DE_entry_enum.Diffrna:
                    case DE_entry_enum.Diffrna_up:
                    case DE_entry_enum.Diffrna_down:
                        break;
                    default:
                        throw new Exception();
                }
            }
            return ok;
        }
        #endregion

        #region bool
        public bool Does_column_already_exist(DE_column_characterization_line_class other_colChar_line)
        {
            bool other_column_already_exists = false;
            int this_count = this.Columns.Count;
            DE_column_characterization_line_class this_colChar_line;
            for (int indexThis = 0; indexThis < this_count; indexThis++)
            {
                this_colChar_line = Columns[indexThis];
                if (other_colChar_line.Equals_other(this_colChar_line))
                {
                    other_column_already_exists = true;
                    break;
                }
            }
            return other_column_already_exists;
        }
        #endregion


        #region Get colIndexes
        public int[] Get_colIndexes_of_timepoints(params Timepoint_enum[] timepoints_ofInterest)
        {
            List<int> indexes = new List<int>();
            int columns_count = Columns.Count;
            for (int i = 0; i < columns_count; i++)
            {
                if (timepoints_ofInterest.Contains(Columns[i].Timepoint))
                {
                    indexes.Add(Columns[i].Index);
                }
            }
            return indexes.ToArray();
        }
        public int[] Get_colIndexes_of_deEntries(params DE_entry_enum[] deEntries_ofInterest)
        {
            List<int> indexes = new List<int>();
            int columns_count = Columns.Count;
            for (int i = 0; i < columns_count; i++)
            {
                if (deEntries_ofInterest.Contains(Columns[i].EntryType))
                {
                    indexes.Add(Columns[i].Index);
                }
            }
            return indexes.ToArray();
        }
        public int[] Get_colIndexes_of_exact_names(params string[][] names_of_interest)
        {
            List<int> indexes = new List<int>();
            int columns_count = Columns.Count;
            int names_group_length = names_of_interest.Length;
            for (int indexCol = 0; indexCol < columns_count; indexCol++)
            {
                for (int indexG = 0; indexG < names_group_length; indexG++)
                {
                    if (Array_class.Array_order_dependent_equal<string>(Columns[indexCol].Names, names_of_interest[indexG]))
                    {
                        indexes.Add(Columns[indexCol].Index);
                    }
                }
            }
            return indexes.ToArray();
        }
        public int[] Get_colIndexes_that_countain_at_least_one_name(params string[] names)
        {
            List<int> indexes = new List<int>();
            int columns_count = Columns.Count;
            int names_length = names.Length;
            for (int indexCol = 0; indexCol < columns_count; indexCol++)
            {
                for (int indexN = 0; indexN < names_length; indexN++)
                {
                    if (Columns[indexCol].Names.Contains(names[indexN]))
                    {
                        indexes.Add(indexCol);
                        break;
                    }
                }
            }
            return indexes.ToArray();
        }
        public int[] Get_colIndexes_of_zeroth_names(params string[] first_names_ofInterest)
        {
            List<int> indexes = new List<int>();
            int columns_count = Columns.Count;
            for (int i = 0; i < columns_count; i++)
            {
                if (first_names_ofInterest.Contains(Columns[i].Names[0]))
                {
                    indexes.Add(Columns[i].Index);
                }
            }
            return indexes.ToArray();
        }
        public int[] Get_colIndexes_that_contain_selected_name_among_their_names(string selected_name)
        {
            List<int> indexes = new List<int>();
            int columns_count = Columns.Count;
            for (int i = 0; i < columns_count; i++)
            {
                foreach (string name in Columns[i].Names)
                {
                    if (name.Equals(selected_name))
                    {
                        indexes.Add(Columns[i].Index);
                        break;
                    }
                }
            }
            return indexes.ToArray();
        }
        public int[] Get_colIndexes_of_intersection_of_timepoints_deEntries_and_exact_names(Timepoint_enum[] timepoints_ofInterest, DE_entry_enum[] deEntries_ofInterest, params string[][] names_ofInterest)
        {
            List<int> finalIndexes = new List<int>();
            int[] deEntriesIndexes = Get_colIndexes_of_deEntries(deEntries_ofInterest);
            int[] timepointsIndexes = Get_colIndexes_of_timepoints(timepoints_ofInterest);
            int[] namesIndexes = Get_colIndexes_of_exact_names(names_ofInterest);
            foreach (int index in timepointsIndexes)
            {
                if ((deEntriesIndexes.Contains(index)) && (namesIndexes.Contains(index)))
                {
                    finalIndexes.Add(index);
                }
            }
            return finalIndexes.ToArray();
        }
        public int Get_max_index()
        {
            int maxIndex = -1;
            foreach (DE_column_characterization_line_class line in Columns)
            {
                if (line.Index > maxIndex) { maxIndex = line.Index; }
            }
            if (maxIndex != Columns.Count - 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Report_class.WriteLine("{0}: max index ({1}) != Columns.Count ({2}) -1", typeof(DE_column_characterization_class).Name, maxIndex, Columns.Count);
                Console.ResetColor();
            }
            return maxIndex;
        }
        #endregion

        public int Get_columns_count()
        {
            return Columns.Count;
        }

        #region Keep remove columns
        public void Keep_columns(int[] keepColIndexes)
        {
            if (Report) { Report_class.WriteLine("{0}: Keep {1} of {2} columns:", typeof(DE_column_characterization_class).Name, keepColIndexes.Length, Columns.Count); }
            Small_correctness_check();
            Index_changes_adopted = false;
            Array.Sort(keepColIndexes);
            int col_count = Columns.Count;
            List<DE_column_characterization_line_class> newColumns = new List<DE_column_characterization_line_class>();
            int newIndex = -1;
            foreach (DE_column_characterization_line_class line in Columns)
            {
                if (keepColIndexes.Contains(line.Index))
                {
                    newIndex++;
                    line.Index = newIndex;
                    newColumns.Add(line);
                }
            }
            Columns = newColumns;
            if (Report)
            {
                //foreach (DE_column_characterization_line_class line in Columns)
                //{
                //    for (int i = 0; i < typeof(DE_column_characterization_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                //    Report_class.WriteLine("{0}-{1}", line.EntryType, line.Timepoint);
                //}
            }
        }
        #endregion

        #region Get column labels
        public Timepoint_enum[] Get_all_timepoints_in_left_right_order()
        {
            Timepoint_enum[] allTimepoints = new Timepoint_enum[Columns.Count];
            for (int i = 0; i < Columns.Count; i++)
            {
                allTimepoints[i] = Columns[i].Timepoint;
            }
            return allTimepoints;
        }
        public DE_entry_enum[] Get_all_entry_types()
        {
            List<DE_entry_enum> entryTypes = new List<DE_entry_enum>();
            foreach (DE_column_characterization_line_class line in this.Columns)
            {
                entryTypes.Add(line.EntryType);
            }
            return entryTypes.ToArray();
        }
        public DE_entry_enum[] Get_all_distinct_entry_types()
        {
            return Get_all_entry_types().Distinct().ToArray();
        }
        public string[] Get_all_zeroth_names()
        {
            List<string> names = new List<string>();
            foreach (DE_column_characterization_line_class line in this.Columns)
            {
                if (!string.IsNullOrEmpty(line.Names[0]))
                {
                    names.Add(line.Names[0]);
                }
            }
            return names.ToArray();
        }
        public string Get_complete_column_label(int indexColumn)
        {
            StringBuilder sb = new StringBuilder();
            DE_column_characterization_line_class colChar_line = Columns[indexColumn];
            if (!colChar_line.EntryType.Equals(DE_entry_enum.E_m_p_t_y))
            {
                if (sb.Length > 0) { sb.AppendFormat("_"); }
                sb.AppendFormat("{0}", colChar_line.EntryType);
            }
            if (!colChar_line.Timepoint.Equals(Timepoint_enum.E_m_p_t_y))
            {
                if (sb.Length > 0) { sb.AppendFormat("_"); }
                sb.AppendFormat("{0}", colChar_line.Timepoint);
            }
            int names_length = colChar_line.Names.Length;
            for (int indexN = 0; indexN < names_length; indexN++)
            {
                if (sb.Length > 0) { sb.AppendFormat("-"); }
                sb.AppendFormat("{0}", colChar_line.Names[indexN]);
            }
            return sb.ToString();
        }
        #endregion

        public void Add_names(int indexCol, params string[] add_names)
        {
            int add_names_length = add_names.Length;
            int old_names_length = Columns[indexCol].Names.Length;
            int new_names_length = old_names_length + add_names_length;
            string[] names_new = new string[new_names_length];
            for (int indexN = 0; indexN < old_names_length; indexN++)
            {
                names_new[indexN] = Columns[indexCol].Names[indexN];
            }
            for (int indexN = old_names_length; indexN < new_names_length; indexN++)
            {
                names_new[indexN] = (string)add_names[indexN - old_names_length].Clone();
            }
            Columns[indexCol].Names = names_new;
        }

        #region Order
        private void Reset_indexes()
        {
            int col_count = Columns.Count;
            for (int indexCol = 0; indexCol < col_count; indexCol++)
            {
                Columns[indexCol].IndexOld = Columns[indexCol].Index;
                Columns[indexCol].Index = indexCol;
            }
        }
        public void Order_columns_by_names_entryType_timepoint()
        {
            foreach (DE_column_characterization_line_class col_line in Columns)
            {
                col_line.IndexOld = col_line.Index;
            }
            Index_changes_adopted = false;
            Columns = Columns.OrderBy(l => l.EntryType).ThenBy(l => l.Timepoint).ToList();
            int names_length = Columns[0].Names.Length;
            for (int indexN = names_length - 1; indexN >= 0; indexN--)
            {
                Columns = Columns.OrderBy(l => l.Names[indexN]).ToList();
            }
            Reset_indexes();
        }
        #endregion
        public void Mark_columns_as_upregulated()
        {
            foreach (DE_column_characterization_line_class line in Columns)
            {
                switch (line.EntryType)
                {
                    case DE_entry_enum.Diffrna:
                        line.EntryType = DE_entry_enum.Diffrna_up;
                        break;
                    default:
                        throw new Exception();
                        throw new Exception();
                }
            }
        }
        public void Mark_columns_as_downregulated()
        {
            foreach (DE_column_characterization_line_class line in Columns)
            {
                switch (line.EntryType)
                {
                    case DE_entry_enum.Diffrna:
                        line.EntryType = DE_entry_enum.Diffrna_down;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        #region Extend
        public int Add_columns_and_count_added_columns(params DE_column_characterization_line_class[] addColchar_lines)
        {
            int col_count = Columns.Count;
            bool line_already_exists;
            int actIndex = Get_max_index();
            int added_cols = 0;
            foreach (DE_column_characterization_line_class line in addColchar_lines)
            {
                line_already_exists = false;
                for (int colIndex = 0; colIndex < col_count; colIndex++)
                {
                    if (line.Equals_other(Columns[colIndex]))
                    {
                        line_already_exists = true;
                        break;
                    }
                }
                if (line_already_exists)
                {
                    //Report_class.Write_error_line("{0}: Timepoint \"{1}\", EntryType \"{2}\" and Name \"{3}\" already exist", typeof(DE_column_characterization_class).Name, line.Timepoint, line.EntryType, line.Name);
                    //Report_class.Write_error_line("{0}: line will not be added", typeof(DE_column_characterization_class).Name, line.Timepoint, line.EntryType);
                }
                else
                {
                    actIndex++;
                    added_cols++;
                    line.Index = actIndex;
                    Columns.Add(line.Deep_copy());
                }
            }
            return added_cols;
        }
        #endregion

        #region Copy
        public DE_column_characterization_class Deep_copy_without_columns()
        {
            DE_column_characterization_class copy = (DE_column_characterization_class)this.MemberwiseClone();
            copy.Columns = new List<DE_column_characterization_line_class>();
            return copy;
        }
        public DE_column_characterization_class Deep_copy()
        {
            DE_column_characterization_class copy = Deep_copy_without_columns();
            foreach (DE_column_characterization_line_class line in this.Columns)
            {
                copy.Columns.Add(line.Deep_copy());
            }
            return copy;
        }
        #endregion
    }

    ///////////////////////////////////////////////////////////////////////////

    class DE_readWriteOptions_class : ReadWriteOptions_base
    {
        #region Fields
        public const char array_delimiter = ';';
        public const char de_array_delimiter = '\t';
        public const char de_symbols_delimiter = ';';

        public static char Array_delimiter { get { return array_delimiter; } }
        public static char DE_array_delimiter { get { return de_array_delimiter; } }
        public static char DE_symbols_delimiter { get { return de_symbols_delimiter; } }
        #endregion

        public static string Get_directory()
        {
            return Global_directory_class.Results_directory;
        }

        public DE_readWriteOptions_class(string subdirectory, string file_name, DE_column_characterization_class colChar)
        {
            string directory = "";
            if (subdirectory.IndexOf(":") != -1)
            {
                directory = subdirectory;
            }
            else
            {
                directory = Get_directory() + subdirectory;
            }
            ReadWriteClass.Create_directory_if_it_does_not_exist(directory);
            File = directory + file_name;
            File_has_headline = true;
            LineDelimiters = new char[] { Global_class.Tab };
            HeadlineDelimiters = new char[] { Global_class.Tab };

            StringBuilder de_headline = new StringBuilder();
            de_headline.AppendFormat("{0}", colChar.Columns[0].Get_full_column_label());
            for (int i = 1; i < colChar.Columns.Count; i++)
            {
                de_headline.AppendFormat("{0}{1}", DE_array_delimiter, colChar.Columns[i].Get_full_column_label());
            }

            Key_propertyNames = new string[] { "ReadWrite_symbols", "ReadWrite_columns" };
            Key_columnNames = new string[] { "Symbol", de_headline.ToString() };
            Report = ReadWrite_report_enum.Report_main;
        }

    }

    class DE_line_class
    {
        #region Fields
        public string[] Symbols { get; set; }
        public DE_columns_line_class[] Columns { get; set; }
        public string Gene_symbol { get { return Symbols[0]; } set { Symbols[0] = value; } }
        public string ReadWrite_symbols
        {
            get { return ReadWriteClass.Get_writeLine_from_array<string>(Symbols, DE_readWriteOptions_class.DE_symbols_delimiter); }
            set { Symbols = ReadWriteClass.Get_array_from_readLine<string>(value, DE_readWriteOptions_class.DE_symbols_delimiter); }
        }
        public string ReadWrite_columns
        {
            get
            {
                StringBuilder write_columns = new StringBuilder();
                int columns_count = Columns.Length;
                write_columns.AppendFormat("{0}", Columns[0].Value);
                for (int indexCol = 1; indexCol < columns_count; indexCol++)
                {
                    write_columns.AppendFormat("{0}{1}", DE_readWriteOptions_class.DE_array_delimiter, Columns[indexCol].Value);
                }
                return write_columns.ToString();
            }
            set
            {
                string[] entries = value.Split(DE_readWriteOptions_class.DE_array_delimiter);
                int entries_length = entries.Length;
                Columns = new DE_columns_line_class[entries_length];
                for (int indexE = 0; indexE < entries_length; indexE++)
                {
                    Columns[indexE] = new DE_columns_line_class();
                    Columns[indexE].Value = Convert.ToDouble(entries[indexE]);
                }
            }
        }
        #endregion

        #region Constructor
        private DE_line_class()
        {
            Symbols = new string[1];
            Columns = new DE_columns_line_class[0];
            Gene_symbol = Global_class.Empty_entry;
        }

        public DE_line_class(int col_count)
            : this()
        {
            Extend_columns(col_count);
        }
        #endregion

        #region operations on columns
        public void Extend_columns(int additional_column_count)
        {
            DE_columns_line_class[] add = new DE_columns_line_class[additional_column_count];
            int this_length = this.Columns.Length;
            int add_length = additional_column_count;
            int new_length = this_length + add_length;
            DE_columns_line_class[] new_columns = new DE_columns_line_class[new_length];
            DE_columns_line_class new_column;
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_columns[indexNew] = this.Columns[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_column = new DE_columns_line_class();
                new_column.Value = 0;
                new_columns[indexNew] = new_column;
            }
            this.Columns = new_columns;
        }
        public void Keep_columns(Dictionary<int, bool> keepCols_dict, int kept_columns_length)
        {
            DE_columns_line_class[] newColumns = new DE_columns_line_class[kept_columns_length];
            int indexNew = -1;
            int col_count = Columns.Length;
            for (int indexCol = 0; indexCol < col_count; indexCol++)
            {
                if (keepCols_dict.ContainsKey(indexCol))
                {
                    indexNew++;
                    newColumns[indexNew] = Columns[indexCol];
                }
            }
            if (indexNew != kept_columns_length - 1) { throw new Exception(); }
            Columns = newColumns;
        }
        public void Keep_columns(int[] keep_columns)
        {
            Dictionary<int, bool> keepCols_dict = new Dictionary<int, bool>();
            foreach (int keep_column in keep_columns)
            {
                keepCols_dict.Add(keep_column, true);
            }
            Keep_columns(keepCols_dict, keep_columns.Length);
        }
        #endregion

        #region Compare equals
        public int Compare_this_de_line_symbols_with_other_de_line_symbols(DE_line_class other)
        {
            int de_line_symbols_length = this.Symbols.Length;
            int data_line_symbols_length = other.Symbols.Length;
            if (de_line_symbols_length != data_line_symbols_length)
            {
                throw new Exception();
            }
            int stringCompare = 1;
            for (int indexS = 0; indexS < de_line_symbols_length; indexS++)
            {
                stringCompare = this.Symbols[indexS].CompareTo(other.Symbols[indexS]);
                if (stringCompare != 0)
                {
                    break;
                }
            }
            return stringCompare;

        }
        #endregion

        #region Copy
        public string[] Deep_copy_symbols()
        {
            int symbols_length = this.Symbols.Length;
            string[] copy = new string[symbols_length];
            for (int indexS = 0; indexS < symbols_length; indexS++)
            {
                copy[indexS] = (string)this.Symbols[indexS].Clone();
            }
            return copy;
        }

        public DE_line_class Deep_copy_without_columns()
        {
            DE_line_class newLine = (DE_line_class)this.MemberwiseClone();
            int symbols_length = this.Symbols.Length;
            newLine.Symbols = new string[symbols_length];
            for (int indexS = 0; indexS < symbols_length; indexS++)
            {
                newLine.Symbols[indexS] = (string)this.Symbols[indexS].Clone();
            }
            return newLine;
        }

        public DE_line_class Deep_copy()
        {
            DE_line_class newLine = this.Deep_copy_without_columns();
            int columns_length = this.Columns.Length;
            newLine.Columns = new DE_columns_line_class[columns_length];
            for (int indexC = 0; indexC < columns_length; indexC++)
            {
                newLine.Columns[indexC] = this.Columns[indexC].Deep_copy();
            }
            return newLine;
        }
        #endregion
    }

    class DE_class
    {
        #region Fields
        public List<DE_line_class> DE { get; set; }
        public int Symbols_length { get; set; }
        public DE_line_class ColSums { get; set; }
        public DE_line_class ColSums_abs { get; set; }
        public DE_line_class ColMeans { get; set; }
        public DE_line_class SampleColSDs { get; set; }
        public DE_line_class PopulationColSDs { get; set; }
        public DE_line_class Non_zero_entry_count_in_cols { get; set; }
        public DE_column_characterization_class ColChar { get; set; }
        #endregion

        #region Constructor
        public DE_class()
        {
            DE = new List<DE_line_class>();
            ColChar = new DE_column_characterization_class();
            ColMeans = new DE_line_class(0);
            ColSums = new DE_line_class(0);
            ColSums_abs = new DE_line_class(0);
            SampleColSDs = new DE_line_class(0);
            PopulationColSDs = new DE_line_class(0);
        }
        #endregion

        #region Order
        public void Order_by_symbol()
        {
            if (DE.Count > 0)
            {
                int symbols_length = DE[0].Symbols.Length;
                for (int indexS = symbols_length - 1; indexS >= 0; indexS--)
                {
                    DE = DE.OrderBy(l => l.Symbols[indexS]).ToList();
                }
            }
        }
        public void Order_by_descending_column_values(int indexCol)
        {
            DE = DE.OrderByDescending(l => l.Columns[indexCol].Value).ToList();
        }
        #endregion

        #region Order columns
        private void Order_columns()
        {
            DE_column_characterization_line_class current_columnChar_line;
            List<DE_line_class> de_new = new List<DE_line_class>();
            DE_line_class new_de_line;
            int col_count = ColChar.Columns.Count;
            int indexOld;
            int indexNew;
            DE_columns_line_class new_empty_columns_line;
            foreach (DE_line_class de_line in DE)
            {
                new_de_line = de_line.Deep_copy_without_columns();
                new_de_line.Columns = new DE_columns_line_class[col_count];
                for (int indexAddZero = 0; indexAddZero < col_count; indexAddZero++)
                {
                    new_empty_columns_line = new DE_columns_line_class();
                    new_empty_columns_line.Value = 0;
                    new_de_line.Columns[indexAddZero] = new_empty_columns_line;
                }
                for (int indexCol = 0; indexCol < col_count; indexCol++)
                {
                    current_columnChar_line = this.ColChar.Columns[indexCol];
                    indexOld = current_columnChar_line.IndexOld;
                    indexNew = current_columnChar_line.Index;
                    new_de_line.Columns[indexNew].Value = de_line.Columns[indexOld].Value;
                }
                de_new.Add(new_de_line);
            }
            DE = de_new;
            ColChar.Index_changes_adopted = true;
        }

        public void Order_columns_by_column_names()
        {
            ColChar.Order_columns_by_names_entryType_timepoint();
            Order_columns();
        }
        #endregion

        #region order T[] inputData
        public List<T> Order_inputData_by_names<T>(List<T> inputData) where T : IFill_de
        {
            int names_length = inputData[0].Names_for_de.Length;
            for (int indexNames = names_length - 1; indexNames >= 0; indexNames--)
            {
                inputData = inputData.OrderBy(l => l.Names_for_de[indexNames]).ToList();
            }
            return inputData;
        }

        public List<T> Order_inputData_by_entryType_timepoint_names<T>(List<T> inputData) where T : IFill_de
        {
            inputData = Order_inputData_by_names<T>(inputData);
            inputData = inputData.OrderBy(l => l.Entry_type_for_de).ThenBy(l => l.Timepoint_for_de).ToList();
            return inputData;
        }
        #endregion

        #region Fill with data
        private void Extend_columns(params DE_column_characterization_line_class[] addColChar)
        {
            int additional_column_count = ColChar.Add_columns_and_count_added_columns(addColChar);
            foreach (DE_line_class line in DE)
            {
                line.Extend_columns(additional_column_count);
            }
        }

        private void Check_if_indexDECol_length_is_one(int[] indexDECol)
        {
            if (indexDECol.Length > 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Report_class.WriteLine("{0}: colIndex length > 1", typeof(DE_class).Name);
                Console.ResetColor();
            }
        }

        private void Get_all_timepoints_and_entrytypes_and_names_combinations_in_data<T>(List<T> inputData, out Timepoint_enum[] timepoints, out DE_entry_enum[] entry_types, out string[][] names) where T : IFill_de
        {
            inputData = Order_inputData_by_entryType_timepoint_names(inputData);
            int inputData_count = inputData.Count;
            List<Timepoint_enum> allTimepoints = new List<Timepoint_enum>();
            List<DE_entry_enum> allEntry_types = new List<DE_entry_enum>();
            List<string[]> allNames = new List<string[]>();
            T inputLine;
            for (int indexInput = 0; indexInput < inputData_count; indexInput++)
            {
                inputLine = inputData[indexInput];
                if ((indexInput == inputData_count - 1)
                    || (!inputLine.Timepoint_for_de.Equals(inputData[indexInput + 1].Timepoint_for_de))
                    || (!inputLine.Entry_type_for_de.Equals(inputData[indexInput + 1].Entry_type_for_de))
                    || (!Array_class.Array_order_dependent_equal<string>(inputLine.Names_for_de, inputData[indexInput + 1].Names_for_de)))
                {
                    allTimepoints.Add(inputLine.Timepoint_for_de);
                    allEntry_types.Add(inputLine.Entry_type_for_de);
                    allNames.Add(Deep_copy_class.Deep_copy_string_array(inputLine.Names_for_de));
                }
            }
            timepoints = allTimepoints.ToArray();
            entry_types = allEntry_types.ToArray();
            names = allNames.ToArray();

        }

        #region Fill with data
        private List<T> Order_input_data_by_symbols_for_de<T>(List<T> inputData) where T : IFill_de
        {
            int symbols_for_de_length = inputData[0].Symbols_for_de.Length;
            int inputData_count = inputData.Count;
            for (int indexI = 0; indexI < inputData_count; indexI++)
            {
                if (inputData[indexI].Symbols_for_de.Length != symbols_for_de_length)
                {
                    throw new Exception();
                }
            }
            for (int indexS = symbols_for_de_length - 1; indexS >= 0; indexS--)
            {
                inputData = inputData.OrderBy(l => l.Symbols_for_de[indexS]).ToList();
            }
            return inputData;
        }

        private bool Equal_inputData_line_Symbols<T>(T dataLine1, T dataLine2) where T : IFill_de
        {
            int symbols_length1 = dataLine1.Symbols_for_de.Length;
            int symbols_length2 = dataLine2.Symbols_for_de.Length;
            bool equal = true;
            if (symbols_length1 != symbols_length2)
            {
                equal = false;
            }
            else
            {
                for (int indexS = 0; indexS < symbols_length1; indexS++)
                {
                    if (!dataLine1.Symbols_for_de[indexS].Equals(dataLine2.Symbols_for_de[indexS]))
                    {
                        equal = false;
                        break;
                    }
                }
            }
            return equal;
        }

        private Fill_de_line_class[] Set_combined_symbols_and_combined_names(Fill_de_line_class[] inputData)
        {
            int inputData_length = inputData.Length;
            Fill_de_line_class fill_de_line;
            StringBuilder sb = new StringBuilder();
            for (int indexInput = 0; indexInput < inputData_length; indexInput++)
            {
                fill_de_line = inputData[indexInput];
                fill_de_line.Set_combined_symbols_and_names();
            }
            return inputData;
        }

        public int Fill_with_data(Fill_de_line_class[] inputData)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            if (ColChar.Report) { Report_class.WriteLine("{0}: Fill with data alternatively", typeof(DE_class).Name); }


            inputData = Set_combined_symbols_and_combined_names(inputData);

            List<DE_column_characterization_line_class> colChars = new List<DE_column_characterization_line_class>();
            DE_column_characterization_line_class new_colChar_line;
            Fill_de_line_class fill_de_line;
            int inputData_length = inputData.Length;

            int indexCol = -1;
            int currentCol = -1;

            //Report_class.Write_code_imperfect_line("{0}: Fill de lines not sorted", typeof(DE_class).Name);
            inputData = Fill_de_line_class.Order_by_entryType_timepoint_combinedNames(inputData);

            //inputData = inputData.OrderBy(l => l.Entry_type_for_de).ThenBy(l => l.Timepoint_for_de).ThenBy(l => l.Combined_names).ToArray();
            for (int indexInput = 0; indexInput < inputData_length; indexInput++)
            {
                fill_de_line = inputData[indexInput];
                if ((indexInput == 0)
                    || (!fill_de_line.Entry_type_for_de.Equals(inputData[indexInput - 1].Entry_type_for_de))
                    || (!fill_de_line.Timepoint_for_de.Equals(inputData[indexInput - 1].Timepoint_for_de))
                    || (!fill_de_line.Combined_names.Equals(inputData[indexInput - 1].Combined_names)))
                {
                    new_colChar_line = new DE_column_characterization_line_class();
                    new_colChar_line.EntryType = fill_de_line.Entry_type_for_de;
                    new_colChar_line.Timepoint = fill_de_line.Timepoint_for_de;
                    new_colChar_line.Names = Array_class.Deep_copy_string_array(fill_de_line.Names_for_de);
                    indexCol++;
                    new_colChar_line.Index = indexCol;
                    colChars.Add(new_colChar_line);
                    currentCol++;
                }
                fill_de_line.Column = currentCol;
            }
            this.ColChar.Columns = colChars;
            Report_class.WriteLine("duration: {0}", stopwatch.Elapsed);

            int col_count = this.ColChar.Columns.Count;
            indexCol = 0;

            Dictionary<string, int> symbol_rowIndex_dict = new Dictionary<string, int>();
            //inputData = inputData.OrderBy(l=>l.Combined_symbols).ToArray();

            DE_line_class new_de_line = new DE_line_class(col_count);
            List<DE_line_class> new_de_list = new List<DE_line_class>();
            int current_lastIndex_of_new_de_list = -1;
            int row_of_symbol;
            for (int indexInput = 0; indexInput < inputData_length; indexInput++)
            {
                fill_de_line = inputData[indexInput];
                if (!symbol_rowIndex_dict.ContainsKey(fill_de_line.Combined_symbols))
                {
                    new_de_line = new DE_line_class(col_count);
                    new_de_line.Symbols = Array_class.Deep_copy_string_array(fill_de_line.Symbols_for_de);
                    current_lastIndex_of_new_de_list++;
                    symbol_rowIndex_dict.Add(fill_de_line.Combined_symbols, current_lastIndex_of_new_de_list);
                    new_de_list.Add(new_de_line);
                }
                row_of_symbol = symbol_rowIndex_dict[fill_de_line.Combined_symbols];
                if ((row_of_symbol==4418)&&(fill_de_line.Column==0))
                {
                    string lol = "";
                }
                if (new_de_list[row_of_symbol].Columns[fill_de_line.Column].Value!=0) { throw new Exception(); }
                new_de_list[row_of_symbol].Columns[fill_de_line.Column].Value = fill_de_line.Value_for_de;
            }
            this.DE = new_de_list;
            Remove_empty_rows_and_columns();
            stopwatch.Stop();
            if (ColChar.Report)
            {
                Report_class.WriteLine("{0}: Job finished, {1} lines in DE instance", typeof(DE_class).Name, DE.Count);
                for (int i = 0; i < typeof(DE_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("duration: {0}", stopwatch.Elapsed);
            }
            return col_count;
        }
        #endregion

        public void Fill_with_other_de(DE_class other)
        {
            if (ColChar.Report)
            {
                Report_class.WriteLine("{0}: Fill with {1} other de lines ", typeof(DE_class).Name, other.DE.Count);
            }
            //ColChar.Compare_organism_or_add_if_empty(other.ColChar.Organism);
            int other_col_count = other.ColChar.Columns.Count;
            List<DE_column_characterization_line_class> addColChar_list = new List<DE_column_characterization_line_class>();
            DE_column_characterization_line_class putative_addColChar;
            for (int i = 0; i < other_col_count; i++)
            {
                putative_addColChar = other.ColChar.Columns[i].Deep_copy();
                if (!this.ColChar.Does_column_already_exist(putative_addColChar))
                {
                    addColChar_list.Add(putative_addColChar);
                }
            }
            DE_column_characterization_line_class[] addColChar = addColChar_list.ToArray();
            //    if (this.ColChar.Check_if_inputColumns_do_not_already_exist(addColChar))
            {
                Extend_columns(addColChar);
                int this_col_count = ColChar.Get_columns_count();
                DE_line_class otherLine;
                other.Order_by_symbol();
                int other_count = other.DE.Count;
                List<DE_line_class> addDE = new List<DE_line_class>();
                DE_line_class addLine = new DE_line_class(this_col_count);
                this.Order_by_symbol();
                int this_count = this.DE.Count;
                int indexThis = 0;
                int stringCompare;
                int[] indexThisCol;
                int added_count = 0;
                for (int indexOther = 0; indexOther < other_count; indexOther++)
                {
                    otherLine = other.DE[indexOther];
                    stringCompare = -2;
                    if (indexThis < this_count)
                    {
                        while ((indexThis < this_count) && (stringCompare < 0))
                        {
                            stringCompare = DE[indexThis].Compare_this_de_line_symbols_with_other_de_line_symbols(otherLine);
                            if (stringCompare < 0) { indexThis++; }
                        }
                    }
                    else
                    {
                        stringCompare = 2;
                    }
                    if (stringCompare == 0)
                    {
                        for (int indexOtherCol = 0; indexOtherCol < other_col_count; indexOtherCol++)
                        {
                            indexThisCol = this.ColChar.Get_colIndexes_of_intersection_of_timepoints_deEntries_and_exact_names(new Timepoint_enum[] { other.ColChar.Columns[indexOtherCol].Timepoint }, new DE_entry_enum[] { other.ColChar.Columns[indexOtherCol].EntryType }, other.ColChar.Columns[indexOtherCol].Names);
                            Check_if_indexDECol_length_is_one(indexThisCol);
                            if (other.DE[indexOther].Columns[indexOtherCol].Value != 0)
                            {
                                if (this.DE[indexThis].Columns[indexThisCol[0]].Value != 0)
                                {
                                    throw new Exception();
                                }
                                this.DE[indexThis].Columns[indexThisCol[0]] = other.DE[indexOther].Columns[indexOtherCol];
                            }
                        }
                        added_count++;
                    }
                    else if ((indexThis == this_count) || (stringCompare > 0))
                    {
                        addLine = new DE_line_class(this_col_count);
                        addLine.Symbols = otherLine.Deep_copy_symbols();
                        for (int indexOtherCol = 0; indexOtherCol < other_col_count; indexOtherCol++)
                        {
                            indexThisCol = this.ColChar.Get_colIndexes_of_intersection_of_timepoints_deEntries_and_exact_names(new Timepoint_enum[] { other.ColChar.Columns[indexOtherCol].Timepoint }, new DE_entry_enum[] { other.ColChar.Columns[indexOtherCol].EntryType }, other.ColChar.Columns[indexOtherCol].Names);
                            Check_if_indexDECol_length_is_one(indexThisCol);
                            addLine.Columns[indexThisCol[0]] = other.DE[indexOther].Columns[indexOtherCol];
                        }
                        addDE.Add(addLine);
                        added_count++;
                    }
                }
                if (added_count != other_count)
                {
                    throw new Exception();
                }
                if (ColChar.Report)
                {
                    for (int i = 0; i < typeof(DE_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                    Report_class.WriteLine("{0} new lines added to {1} existing lines", addDE.Count, DE.Count);
                }
                DE.AddRange(addDE);
            }
        }
        private Fill_de_line_class[] Generate_fill_de_lines()
        {
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            int col_length = this.ColChar.Columns.Count;
            int de_length = this.DE.Count;
            DE_line_class de_line;
            DE_column_characterization_line_class colChar_line;
            for (int indexDE = 0; indexDE < de_length; indexDE++)
            {
                de_line = this.DE[indexDE];
                for (int indexCol = 0; indexCol < col_length; indexCol++)
                {
                    if (de_line.Columns[indexCol].Value != 0)
                    {
                        colChar_line = this.ColChar.Columns[indexCol];
                        fill_de_line = new Fill_de_line_class();
                        fill_de_line.Symbols_for_de = Array_class.Deep_copy_string_array(de_line.Symbols);
                        fill_de_line.Names_for_de = Array_class.Deep_copy_string_array(colChar_line.Names);
                        fill_de_line.Entry_type_for_de = colChar_line.EntryType;
                        fill_de_line.Timepoint_for_de = colChar_line.Timepoint;
                        fill_de_line.Value_for_de = de_line.Columns[indexCol].Value;
                        fill_de_list.Add(fill_de_line);
                    }
                }
            }
            return fill_de_list.ToArray();
        }
        public void Fill_with_other_de_alternativly(DE_class other)
        {
            Fill_de_line_class[] this_fill_de_lines = Generate_fill_de_lines();
            Fill_de_line_class[] other_fill_de_lines = other.Generate_fill_de_lines();
            List<Fill_de_line_class> combined_fill_de_lines = new List<Fill_de_line_class>();
            combined_fill_de_lines.AddRange(this_fill_de_lines);
            combined_fill_de_lines.AddRange(other_fill_de_lines);
            DE_class de_combined = new DE_class();
            de_combined.Fill_with_data(combined_fill_de_lines.ToArray());
            Deep_copy_into_this(de_combined);
        }
        #endregion

        #region Modify symbols
        public void Set_symbols_to_upper_case_letters()
        {
            Report_class.Write("{0}: Set symbols to upper case letters", typeof(DE_class).Name);
            foreach (DE_line_class line in DE)
            {
                line.Gene_symbol = line.Gene_symbol.ToUpper();
            }

        }
        #endregion
        public int[] Get_non_zero_counts_of_each_column_in_indexOrder()
        {
            int col_count = ColChar.Columns.Count;
            DE_line_class de_line;
            DE_columns_line_class column_line;
            int de_count = DE.Count;
            int[] non_zero_counts = new int[col_count];
            for (int indexDE = 0; indexDE < de_count; indexDE++)
            {
                de_line = DE[indexDE];
                col_count = de_line.Columns.Length;
                for (int indexCol = 0; indexCol < col_count; indexCol++)
                {
                    column_line = de_line.Columns[indexCol];
                    if (column_line.Value != 0)
                    {
                        non_zero_counts[indexCol]++;
                    }
                }
            }
            return non_zero_counts;
        }

        #region Keep and remove
        public void Remove_empty_rows_and_columns()
        {
            if (ColChar.Report) { Report_class.WriteLine("{0}: Remove empty rows and columns", typeof(DE_class).Name); }
            int column_count = ColChar.Columns.Count;
            List<int> keepColumns = new List<int>();
            bool keep_row;
            List<DE_line_class> newDE = new List<DE_line_class>();
            int de_length = DE.Count;
            DE_line_class line;
            for (int indexDE = 0; indexDE < de_length; indexDE++)
            {
                line = DE[indexDE];
                keep_row = false;
                for (int indexCol = 0; indexCol < column_count; indexCol++)
                {
                    if (line.Columns[indexCol].Value != 0)
                    {
                        keepColumns.Add(indexCol);
                        keep_row = true;
                    }
                }
                if (keep_row)
                {
                    newDE.Add(line);
                }
            }
            keepColumns = keepColumns.Distinct().OrderBy(l => l).ToList();
            int keepColumns_length = keepColumns.Count;
            Dictionary<int, bool> keepCols_dict = new Dictionary<int, bool>();
            foreach (int keepColumn in keepColumns)
            {
                keepCols_dict.Add(keepColumn, true);
            }

            if (keepColumns.Count() < ColChar.Columns.Count)
            {
                ColChar.Keep_columns(keepColumns.ToArray());
                de_length = newDE.Count;
                for (int indexDE = 0; indexDE < de_length; indexDE++)
                {
                    line = newDE[indexDE];
                    line.Keep_columns(keepCols_dict, keepColumns_length);
                    newDE[indexDE] = line;
                }
            }
            if (ColChar.Report) { Report_class.WriteLine("{0}: {1} of {2} rows and {3} of {4} columns kept", typeof(DE_class).Name, newDE.Count, DE.Count, ColChar.Columns.Count, column_count); }
            DE = newDE;
            this.ColChar.Index_changes_adopted = true;
        }
        public void Keep_only_columns_with_stated_zeroth_names(params string[] names)
        {
            int names_length = names.Length;
            if (ColChar.Report)
            {
                Report_class.Write("{0}: Keep only columns of first names", typeof(DE_class).Name);
                for (int i = 0; i < names_length; i++) { Report_class.Write(" {0}", names[i]); }
                Report_class.WriteLine();
            }
            int[] keepCols = ColChar.Get_colIndexes_of_zeroth_names(names);
            Keep_only_stated_columns(keepCols);
        }
        public void Keep_only_columns_with_selected_name_among_any_names(string name)
        {
            int[] keepCols = ColChar.Get_colIndexes_that_contain_selected_name_among_their_names(name);
            Keep_only_stated_columns(keepCols);
        }
        public void Keep_only_stated_columns(params int[] keepCols)
        {
            foreach (DE_line_class line in DE)
            {
                line.Keep_columns(keepCols);
            }
            ColChar.Keep_columns(keepCols);
            Remove_empty_rows_and_columns();
            ColChar.Index_changes_adopted = true;
        }
        public void Keep_only_stated_symbols(string[] keepSymbols)
        {
            keepSymbols = keepSymbols.Distinct().OrderBy(l => l).ToArray();
            Dictionary<string, bool> keep_symbols_dict = new Dictionary<string, bool>();
            foreach (string symbol in keepSymbols)
            {
                keep_symbols_dict.Add(symbol, true);
            }
            if (ColChar.Report) { Report_class.WriteLine("{0}: Keep only symbols which are part of input symbol list (length: {1})", typeof(DE_class).Name, keepSymbols.Length); }
            int de_symbol_count = DE.Count;
            DE_line_class de_line;
            List<DE_line_class> newDE = new List<DE_line_class>();
            for (int indexDE=0; indexDE<de_symbol_count;indexDE++)
            {
                de_line = DE[indexDE];
                if (keep_symbols_dict.ContainsKey(de_line.Gene_symbol))
                {
                    newDE.Add(de_line);
                }
            }

            if (ColChar.Report)
            {
                for (int i = 0; i < typeof(DE_class).Name.Length; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("{0} of {1} symbols kept", newDE.Count, de_symbol_count);
            }
            DE = newDE;
            Remove_empty_rows_and_columns();
        }
        #endregion

        #region Get indexes and symbols
        public double[] Get_all_values_of_indicated_column_in_given_order(int indexCol)
        {
            int de_length = this.DE.Count;
            double[] values = new double[de_length];
            DE_line_class de_line;
            for (int indexDe = 0; indexDe < de_length; indexDe++)
            {
                de_line = DE[indexDe];
                values[indexDe] = de_line.Columns[indexCol].Value;
            }
            return values;
        }
        public int[] Get_rowIndexes_of_non_zero_entries_in_all_stated_colIndexes(params int[] colIndexes)
        {
            int de_count = DE.Count;
            int colIndex_length = colIndexes.Length;
            List<int> rowIndexes = new List<int>();
            bool add;
            for (int deIndex = 0; deIndex < de_count; deIndex++)
            {
                add = true;
                for (int cIndexIndex = 0; cIndexIndex < colIndex_length; cIndexIndex++)
                {
                    if (DE[deIndex].Columns[colIndexes[cIndexIndex]].Value == 0)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    rowIndexes.Add(deIndex);
                }
            }
            return rowIndexes.ToArray();
        }
        private int[] Get_rowIndexes_of_non_zero_entries_in_at_least_one_of_stated_colIndexes(params int[] colIndexes)
        {
            int de_count = DE.Count;
            int colIndex_length = colIndexes.Length;
            List<int> rowIndexes = new List<int>();
            bool add;
            for (int deIndex = 0; deIndex < de_count; deIndex++)
            {
                add = false;
                for (int cIndexIndex = 0; cIndexIndex < colIndex_length; cIndexIndex++)
                {
                    if (DE[deIndex].Columns[colIndexes[cIndexIndex]].Value != 0)
                    {
                        add = true;
                        break;
                    }
                }
                if (add)
                {
                    rowIndexes.Add(deIndex);
                }
            }
            return rowIndexes.ToArray();
        }
        public int[] Get_rowIndexes_of_non_zero_in_at_least_one_column_of_each_stated_deEntry(params DE_entry_enum[] deEntries)
        {
            int deEntries_length = deEntries.Length;
            if (ColChar.Report)
            {
                Report_class.WriteLine("{0}: Get rowIndexes of non zero in at least one columns", typeof(DE_class).Name);
                for (int i = 0; i < typeof(DE_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                for (int i = 0; i < deEntries_length; i++) { Report_class.Write(" {0}", deEntries[i]); }
                Report_class.WriteLine();
            }
            List<int> finalIndexes = new List<int>();
            int[][] rowIndexes = new int[deEntries_length][];
            for (int i = 0; i < deEntries_length; i++)
            {
                int[] colIndexes = ColChar.Get_colIndexes_of_deEntries(deEntries[i]);
                rowIndexes[i] = Get_rowIndexes_of_non_zero_entries_in_at_least_one_of_stated_colIndexes(colIndexes);
            }
            int[] indexIndex = new int[deEntries_length];
            int[] rI_length = new int[deEntries_length];
            for (int i = 0; i < deEntries_length; i++)
            {
                indexIndex[i] = 0;
                rI_length[i] = rowIndexes[i].Length;
            }
            bool repeat = true;
            int smallest;
            while (repeat)
            {
                smallest = -1;
                for (int i = 1; i < deEntries_length; i++)
                {
                    if (smallest == -1)
                    {
                        if (rowIndexes[i][indexIndex[i]] < rowIndexes[i - 1][indexIndex[i - 1]]) { smallest = i; }
                        else if (rowIndexes[i][indexIndex[i]] > rowIndexes[i - 1][indexIndex[i - 1]]) { smallest = i - 1; }
                    }
                    else
                    {
                        if (rowIndexes[i][indexIndex[i]] < rowIndexes[smallest][indexIndex[smallest]]) { smallest = i; }
                    }
                }
                if (smallest == -1)
                {
                    finalIndexes.Add(rowIndexes[0][indexIndex[0]]);
                    for (int i = 0; i < deEntries_length; i++) { indexIndex[i]++; }
                }
                else
                {
                    indexIndex[smallest]++;
                }
                for (int i = 0; i < deEntries_length; i++)
                {
                    if (indexIndex[i] == rI_length[i]) { repeat = false; }
                }
            }
            return finalIndexes.ToArray();
        }
        public void Fill_with_symbols(Organism_enum organism, DE_entry_enum entryType, Timepoint_enum timepoint, string[] names, double value, params string[] symbols)
        {
            if (ColChar.Report)
            {
                Report_class.WriteLine("{0}: Fill with {1} symbols ", typeof(DE_class).Name, symbols.Length);
            }
            //ColChar.Compare_organism_or_add_if_empty(organism);
            DE_column_characterization_line_class[] addColChar = new DE_column_characterization_line_class[1];
            addColChar[0] = new DE_column_characterization_line_class();
            addColChar[0].EntryType = entryType;
            addColChar[0].Timepoint = timepoint;
            addColChar[0].Names = Deep_copy_class.Deep_copy_string_array(names);
            int[] columns = ColChar.Get_colIndexes_of_intersection_of_timepoints_deEntries_and_exact_names(new Timepoint_enum[] { timepoint }, new DE_entry_enum[] { entryType }, names);

            if (columns.Length == 0) { Extend_columns(addColChar); }
            int this_col_count = ColChar.Get_columns_count();
            int other_col_count = 1;
            symbols = symbols.OrderBy(l => l).ToArray();
            int symbols_length = symbols.Length;
            List<DE_line_class> addDE = new List<DE_line_class>();
            DE_line_class addLine = new DE_line_class(this_col_count);
            Order_by_symbol();
            int this_count = this.DE.Count;
            int indexThis = 0;
            int stringCompare;
            int[] indexThisCol;
            int added_count = 0;
            for (int indexSymbol = 0; indexSymbol < symbols_length; indexSymbol++)
            {
                stringCompare = -2;
                if (indexThis < this_count)
                {
                    while ((indexThis < this_count) && (stringCompare < 0))
                    {
                        stringCompare = DE[indexThis].Gene_symbol.CompareTo(symbols[indexSymbol]);
                        if (stringCompare < 0) { indexThis++; }
                    }
                }
                else
                {
                    stringCompare = 2;
                }
                if (stringCompare == 0)
                {
                    for (int indexOtherCol = 0; indexOtherCol < other_col_count; indexOtherCol++)
                    {
                        indexThisCol = this.ColChar.Get_colIndexes_of_intersection_of_timepoints_deEntries_and_exact_names(new Timepoint_enum[] { timepoint }, new DE_entry_enum[] { entryType }, names);
                        Check_if_indexDECol_length_is_one(indexThisCol);
                        this.DE[indexThis].Columns[indexThisCol[0]].Value = value;
                        added_count++;
                    }
                }
                else if ((indexThis == this_count) || (stringCompare > 0))
                {
                    addLine = new DE_line_class(this_col_count);
                    addLine.Gene_symbol = (string)symbols[indexSymbol].Clone();
                    for (int indexOtherCol = 0; indexOtherCol < other_col_count; indexOtherCol++)
                    {
                        indexThisCol = this.ColChar.Get_colIndexes_of_intersection_of_timepoints_deEntries_and_exact_names(new Timepoint_enum[] { timepoint }, new DE_entry_enum[] { entryType }, names);
                        Check_if_indexDECol_length_is_one(indexThisCol);
                        addLine.Columns[indexThisCol[0]].Value = value;
                    }
                    addDE.Add(addLine);
                    added_count++;
                }
            }
            if (added_count != symbols_length)
            {
                throw new Exception();
            }
            if (ColChar.Report)
            {
                for (int i = 0; i < typeof(DE_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("{0} new lines added to {1} existing lines", addDE.Count, DE.Count);
            }
            DE.AddRange(addDE);
        }
        #endregion

        public string[] Get_all_symbols_with_non_zero_entries_at_stated_columns(params int[] stated_columns)
        {
            stated_columns = stated_columns.Distinct().OrderBy(l => l).ToArray();
            int stated_col_length = stated_columns.Length;
            List<string> allSymbols = new List<string>();
            bool keep;
            foreach (DE_line_class de_line in DE)
            {
                keep = true;
                for (int indexIndex = 0; indexIndex < stated_col_length; indexIndex++)
                {
                    if (de_line.Columns[stated_columns[indexIndex]].Value == 0)
                    {
                        keep = false;
                        break;
                    }
                }
                if (keep)
                {
                    allSymbols.Add(de_line.Gene_symbol);
                }
            }
            return allSymbols.ToArray();
        }

        public void Print_summary()
        {
            Report_class.WriteLine("{0}: Print differentially expressed genes", typeof(DE_class).Name);
            int col_count = ColChar.Get_columns_count();
            DE_line_class up_count = new DE_line_class(col_count);
            DE_line_class down_count = new DE_line_class(col_count);
            int total_up = 0;
            int total_down = 0;
            bool up_added = false;
            bool down_added = false;

            foreach (DE_line_class line in DE)
            {
                up_added = false;
                down_added = false;
                int column_count = line.Columns.Length;
                for (int columnIndex = 0; columnIndex < column_count; columnIndex++)
                {
                    if (line.Columns[columnIndex].Value > 0)
                    {
                        up_count.Columns[columnIndex].Value++;
                        if (!up_added)
                        {
                            up_added = true;
                            total_up++;
                        }
                    }
                    if (line.Columns[columnIndex].Value < 0)
                    {
                        down_count.Columns[columnIndex].Value++;
                        if (!down_added)
                        {
                            down_added = true;
                            total_down++;
                        }
                    }
                }
            }
            for (int indexCol = 0; indexCol < col_count; indexCol++)
            {
                for (int i = 0; i < typeof(DE_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("{0}:\tup: {1}\t down: {2}\t total: {3}", ColChar.Get_complete_column_label(indexCol), up_count.Columns[indexCol].Value, down_count.Columns[indexCol].Value, up_count.Columns[indexCol].Value + down_count.Columns[indexCol].Value);
            }
            for (int i = 0; i < typeof(DE_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0}:\tup: {1}\t down: {2}\t total: {3}", "Total", total_up, total_down, DE.Count());
            Report_class.WriteLine();
        }

        #region Get sub symbols
        public string[] Get_all_symbols_in_current_order()
        {
            string[] allSymbols = new string[DE.Count];
            int de_count = DE.Count;
            for (int indexDE = 0; indexDE < de_count; indexDE++)
            {
                allSymbols[indexDE] = DE[indexDE].Gene_symbol;
            }
            return allSymbols;
        }
        #endregion


        #region Get sub DE_classes
        public DE_class Get_DE_class_with_expression_over_cutoff(double min_fc, int smaller0_abs1_larger2)
        {
            if (ColChar.Report)
            {
                Report_class.WriteLine("{0}: Get DE_class", typeof(DE_class).Name);
                for (int i = 0; i < typeof(DE_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                switch (smaller0_abs1_larger2)
                {
                    case 0:
                        Report_class.WriteLine("with expression <= -log2({0})", min_fc);
                        break;
                    case 1:
                        Report_class.WriteLine("with abs expression >= {0}", min_fc);
                        break;
                    case 2:
                        Report_class.WriteLine("with expression >= log2({0})", min_fc);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Report_class.WriteLine("{0} is not considered for smaller0_abs1_larger2", smaller0_abs1_larger2);
                        Console.ResetColor();
                        break;
                }
            }
            DE_class newDE = new DE_class();
            if (min_fc < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Report_class.WriteLine("{0}: min_fc ({1}) has to be >= 1", typeof(DE_class).Name, min_fc);
                Console.ResetColor();
            }
            else
            {
                newDE = this.Deep_copy();
                int col_count = ColChar.Columns.Count;
                int kept_rows_count = 0;
                bool row_kept;
                int kept_values_count = 0;
                foreach (DE_line_class line in newDE.DE)
                {
                    row_kept = false;
                    for (int indexCol = 0; indexCol < col_count; indexCol++)
                    {
                        switch (smaller0_abs1_larger2)
                        {
                            case 0:
                                if (line.Columns[indexCol].Value > (-Math.Log(min_fc, 2))) { line.Columns[indexCol].Value = 0; }
                                else if (line.Columns[indexCol].Value != 0)
                                {
                                    kept_values_count++;
                                    if (!row_kept)
                                    {
                                        kept_rows_count++;
                                        row_kept = true;
                                    }
                                }
                                break;
                            case 1:
                                if (Math.Abs(line.Columns[indexCol].Value) <= Math.Log(min_fc, 2)) { line.Columns[indexCol].Value = 0; }
                                else if (line.Columns[indexCol].Value != 0)
                                {
                                    kept_values_count++;
                                    if (!row_kept)
                                    {
                                        kept_rows_count++;
                                        row_kept = true;
                                    }
                                }
                                break;
                            case 2:
                                if (line.Columns[indexCol].Value < (Math.Log(min_fc, 2))) { line.Columns[indexCol].Value = 0; }
                                else if (line.Columns[indexCol].Value != 0)
                                {
                                    kept_values_count++;
                                    if (!row_kept)
                                    {
                                        kept_rows_count++;
                                        row_kept = true;
                                    }
                                }
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Report_class.WriteLine("{0} is not considered for smaller0_abs1_larger2", smaller0_abs1_larger2);
                                Console.ResetColor();
                                break;
                        }
                    }
                }
                newDE.Remove_empty_rows_and_columns();
                if (ColChar.Report) { Report_class.WriteLine("{0}: {1} values within {2} rows are kept for new class", typeof(DE_class).Name, kept_values_count, kept_rows_count); }
                newDE.Print_summary();
            }
            return newDE;
        }
        public DE_class Get_DE_class_with_only_upregulated_entries()
        {
            Report_class.Write("{0}: Get de class with only upregulated entries", typeof(DE_class).Name);
            DE_class de_up = new DE_class();
            if (ColChar.Check_if_columns_are_differential_expression())
            {
                de_up = Get_DE_class_with_expression_over_cutoff(1, 2);
                de_up.ColChar.Mark_columns_as_upregulated();
            }
            return de_up;
        }
        public DE_class Get_DE_class_with_only_downregulated_entries()
        {
            Report_class.Write("{0}: Get de class with only downregulated entries", typeof(DE_class).Name);
            DE_class de_down = new DE_class();
            if (ColChar.Check_if_columns_are_differential_expression())
            {
                de_down = Get_DE_class_with_expression_over_cutoff(1, 0);
                de_down.ColChar.Mark_columns_as_downregulated();
            }
            return de_down;
        }
        public DE_class Get_DE_class_with_separated_up_and_downregulated_entries()
        {
            Report_class.Write("{0}: Get de class with separated up and downregulated entries", typeof(DE_class).Name);
            DE_class de = Get_DE_class_with_only_upregulated_entries();
            DE_class de_down = Get_DE_class_with_only_downregulated_entries();
            de.Fill_with_other_de_alternativly(de_down);
            return de;
        }
        #endregion

        public Dictionary<string, double> Generate_geneSymbol_valueOf1stColumn_dict()
        {
            Dictionary<string, double> geneSymbol_valueOf1stColumn_dict = new Dictionary<string, double>();
            DE_line_class de_line;
            int de_length = this.DE.Count;
            for (int indexDE = 0; indexDE < de_length; indexDE++)
            {
                de_line = this.DE[indexDE];
                geneSymbol_valueOf1stColumn_dict.Add(de_line.Gene_symbol, de_line.Columns[0].Value);
            }
            return geneSymbol_valueOf1stColumn_dict;
        }

        #region Write Deep copy
        public void Write_file(string subdirectory, string file_name)
        {
            DE_readWriteOptions_class options = new DE_readWriteOptions_class(subdirectory, file_name, ColChar);
            ReadWriteClass.WriteData(DE, options);
        }
        protected void Deep_copy_into_this(DE_class other)
        {
            this.ColChar = other.ColChar.Deep_copy();
            this.DE = new List<DE_line_class>();
            foreach (DE_line_class line in other.DE)
            {
                this.DE.Add(line.Deep_copy());
            }
        }
        public DE_class Deep_copy_without_DE_lines()
        {
            DE_class copy = (DE_class)this.MemberwiseClone();
            copy.ColChar = this.ColChar.Deep_copy();
            copy.DE = new List<DE_line_class>();
            return copy;
        }
        public DE_class Deep_copy()
        {
            DE_class copy = Deep_copy_without_DE_lines();
            copy.ColSums = this.ColSums.Deep_copy();
            copy.ColSums_abs = this.ColSums_abs.Deep_copy();
            copy.ColMeans = this.ColMeans.Deep_copy();
            copy.SampleColSDs = this.SampleColSDs.Deep_copy();
            copy.PopulationColSDs = this.PopulationColSDs.Deep_copy();
            foreach (DE_line_class line in DE)
            {
                copy.DE.Add(line.Deep_copy());
            }
            return copy;
        }
        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////
}
