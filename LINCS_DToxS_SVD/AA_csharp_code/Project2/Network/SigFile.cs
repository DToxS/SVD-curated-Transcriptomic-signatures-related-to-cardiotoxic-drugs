using System;
using System.Collections.Generic;
using System.Linq;
using Common_classes;
using ReadWrite;
using Highthroughput_data;

namespace Network
{
    class SigNWBasis_writeOptions_class : ReadWriteOptions_base
    {
        public string Directory_results { get; set; }

        public SigNWBasis_writeOptions_class(string subdirectory, string file_name)
        {
            Directory_results = Global_directory_class.Results_directory;
            File = Directory_results + subdirectory + file_name;
            Key_propertyNames = new string[] { "Source_name", "Source", "Target" };
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

    class SigNWBasis_readOptions_class : ReadWriteOptions_base
    {
        public string Directory_results { get; set; }

        public SigNWBasis_readOptions_class(string directory, string file_name)
        {
            Directory_results = directory;
            File = Directory_results + file_name;
            Key_propertyNames = new string[] { "Source", "Target" };
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

    class SigNWBasis_line_class : IFill_de
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public Network_interaction_type_enum Type { get; set; }
        public Network_direction_type_enum Interaction_direction { get; set; }
        public int Source_index { get; set; }
        public int Target_index { get; set; }

        #region Var for de
        public Timepoint_enum Timepoint_for_de
        { get; set; }
        public string[] Symbols_for_de
        { get { return new string[] { Target }; } }
        public string[] Names_for_de
        { get; set; }
        public double Value_for_de { get { return 1; } }
        public DE_entry_enum Entry_type_for_de { get; set; }
        #endregion

        public SigNWBasis_line_class()
        {
            Source_index = -1;
            Target_index = -1;
        }

        public bool Same_source_and_target(SigNWBasis_line_class other)
        {
            return ((this.Source.Equals(other.Source)) &&
                    (this.Target.Equals(other.Target)));
        }

        public virtual SigNWBasis_line_class Deep_copy()
        {
            SigNWBasis_line_class newLine = (SigNWBasis_line_class)this.MemberwiseClone();
            newLine.Source = (string)this.Source;
            newLine.Target = (string)this.Target;
            newLine.Source_index = -1;
            newLine.Target_index = -1;
            return newLine;
        }
    }

    class SigNWBasis_class<T> where T : SigNWBasis_line_class
    {
        #region Fields
        protected List<T> sigNW;
        public Node_class UniqueNodes { get; private set; }
        public List<T> SigNW
        {
            get
            {
                return sigNW;
            }
            set
            {
                sigNW = value;
                Clear_uniqueNodes_and_source_target_indexes();
            }
        }
        #endregion

        #region Constructors
        public SigNWBasis_class()
        {
            sigNW = new List<T>();
            UniqueNodes = new Node_class();
        }
        #endregion

        #region Check
        private bool Check_for_lines_with_duplicated_sources_and_targets()
        {
            int sigNW_count = sigNW.Count;
            T sigNW_line;
            List<T> newSigNW = new List<T>();
            Order_sigNW_by_source_and_target();
            bool ok = true;
            for (int indexSig = 0; indexSig < sigNW_count; indexSig++)
            {
                sigNW_line = sigNW[indexSig];
                if ((indexSig != 0) && (sigNW_line.Source.Equals(sigNW[indexSig - 1].Source)) && (sigNW_line.Target.Equals(sigNW[indexSig - 1].Target)))
                {
                    throw new Exception();
                }
            }
            return ok;
        }

        public void Check_if_specified_source_target_connection_exists()
        {
            bool exists = false;
            int sigNW_count = sigNW.Count;
            T sigNW_line;
            List<T> newSigNW = new List<T>();
            for (int indexSig = 0; indexSig < sigNW_count; indexSig++)
            {
                sigNW_line = sigNW[indexSig];
                if ((sigNW_line.Source.Equals("ZDHHC3") && (sigNW_line.Target.Equals("UBQLN4"))))
                {
                    exists = true;
                }
            }
            if (!exists) { throw new Exception(); }
        }

        #endregion

        #region Generate new
        private void Fill_types()
        {
            switch (UniqueNodes.Network_direction)
            {
                case Network_direction_type_enum.Undirected_double:
                    foreach (T line in sigNW)
                    {
                        line.Type = Network_interaction_type_enum.Interaction;
                    }
                    break;
                case Network_direction_type_enum.Directed_forward:
                    foreach (T line in sigNW)
                    {
                        line.Type = Network_interaction_type_enum.Activation;
                    }
                    break;
                default:
                    throw new Exception();
            }
        }
        public void Generate_sigNW_from_sigNW_list(List<T> sigNW_list, Network_direction_type_enum network_direction)
        {
            this.UniqueNodes.Network_direction = network_direction;
            this.sigNW = sigNW_list;
            Finish_sigNW_generation();
        }
        public void Finish_sigNW_generation()
        {
            UniqueNodes.Includes_unique_nodes = false;
            Set_unique_nodes_and_fill_NW_with_sourceIndexes_and_targetIndexes();
            Fill_types();
            //Set_all_nodeNames_sources_and_targets_to_upperCaseLetters_except_no_node();
            UniqueNodes.Complete_correctness_check();
            if (!Check_for_lines_with_duplicated_sources_and_targets())
            {
                throw new Exception();
            }
        }
        #endregion

        #region OrderBy
        public void Order_sigNW_by_sourceIndex_and_targetIndex()
        {
            sigNW = sigNW.OrderBy(l => l.Source_index).ThenBy(l => l.Target_index).ToList();
        }
        public void Order_sigNW_by_source_and_target()
        {
            sigNW = sigNW.OrderBy(l => l.Source).ThenBy(l => l.Target).ToList();
        }
        #endregion

        public void Remove_all_lines_with_duplicated_sources_and_targets()
        {
            int sigNW_count = sigNW.Count;
            T sigNW_line;
            List<T> newSigNW = new List<T>();
            Order_sigNW_by_source_and_target();
            for (int indexSig = 0; indexSig < sigNW_count; indexSig++)
            {
                sigNW_line = sigNW[indexSig];
                if ((indexSig == 0)
                    || (!sigNW_line.Source.Equals(sigNW[indexSig - 1].Source))
                    || (!sigNW_line.Target.Equals(sigNW[indexSig - 1].Target)))
                {
                    newSigNW.Add(sigNW_line);
                }
            }
            sigNW = newSigNW;
        }

        #region Fill_name or index with regard to UniqueNodes
        protected void Fill_index_of_sources_and_targets_based_on_names()
        {
            if (UniqueNodes.Report)
            {
                Report_class.WriteLine("{0}: Fill source indexes based source names", typeof(T).Name);
            }
            int firstTargetIndex;
            int lastSourceIndex;
            UniqueNodes.Order_by_index_and_get_lastSourceIndex_and_firstTargetIndex(out lastSourceIndex, out firstTargetIndex);
            int indexUN_source = 0;
            int NW_count = sigNW.Count;
            int uniqueNodes_count = UniqueNodes.UN.Count();
            int sourceStringCompare = -2;
            SigNWBasis_line_class nw_line;
            Node_line_class act_un;
            sigNW = sigNW.OrderBy(l => l.Source).ToList();
            for (int indexNW = 0; indexNW < NW_count; indexNW++)
            {
                nw_line = sigNW[indexNW];
                sourceStringCompare = -2;
                while (sourceStringCompare < 0)  // if indexUN_source > lastSourceIndex something is wrong
                {
                    act_un = UniqueNodes.UN[indexUN_source];
                    if ((indexUN_source > 0) && (act_un.Name.CompareTo(UniqueNodes.UN[indexUN_source - 1].Name) <= 0))
                    {
                        throw new Exception();
                    }
                    sourceStringCompare = act_un.Name.CompareTo(nw_line.Source);
                    if (sourceStringCompare < 0)
                    {
                        indexUN_source++;
                    }
                    else if (sourceStringCompare == 0)
                    {
                        nw_line.Source_index = act_un.Index;
                    }
                    else // sourceStringCompare > 0
                    {
                        throw new Exception();
                    }
                }
            }

            if (UniqueNodes.Report)
            {
                for (int i = 0; i < typeof(T).Name.Length + 2; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("Fill target indexes based target names", typeof(T).Name);
            }
            sigNW = sigNW.OrderBy(l => l.Target).ToList();
            int targetStringCompare = -2;
            int indexUN_target = firstTargetIndex;
            for (int indexNW = 0; indexNW < NW_count; indexNW++)
            {
                nw_line = sigNW[indexNW];
                if (!string.IsNullOrEmpty(nw_line.Target))
                {
                    targetStringCompare = -2;
                    while ((targetStringCompare < 0) && (indexUN_target < uniqueNodes_count)) // if indexUN_target == un_count something is wrong
                    {
                        act_un = UniqueNodes.UN[indexUN_target];
                        if ((indexUN_target > firstTargetIndex) && (act_un.Name.CompareTo(UniqueNodes.UN[indexUN_target - 1].Name) <= 0))
                        {
                            throw new Exception();
                        }
                        targetStringCompare = act_un.Name.CompareTo(nw_line.Target);
                        if (targetStringCompare < 0)
                        {
                            indexUN_target++;
                        }
                        else if (targetStringCompare == 0)
                        {
                            nw_line.Target_index = act_un.Index;
                        }
                        else if (!nw_line.Target.Equals(Global_class.No_node_text))// targetStringCompare > 0
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            UniqueNodes.Index_changes_adopted = true;
        }
        #endregion

        #region Operations on UniqueNodes
        protected void Clear_uniqueNodes_and_source_target_indexes()
        {
            if (UniqueNodes.UN.Count > 0) { UniqueNodes.Clear_unique_nodes(); }
            if ((sigNW.Count > 0) && ((sigNW[0].Source_index != -1) || (sigNW[0].Target_index != -1)))
            {
                int sigNW_count = sigNW.Count;
                for (int i = 0; i < sigNW_count; i++)
                {
                    sigNW[i].Source_index = -1;
                    sigNW[i].Target_index = -1;
                }
            }
        }
        protected void Set_unique_nodes()
        {
            sigNW = sigNW.OrderBy(l => l.Source).ThenBy(l => l.Target).ToList();
            int sigNW_length = sigNW.Count;
            List<string> sources = new List<string>();
            List<string> targets = new List<string>();
            T sigNW_line;
            for (int indexSig = 0; indexSig < sigNW_length; indexSig++)
            {
                sigNW_line = sigNW[indexSig];
                if ((!string.IsNullOrEmpty(sigNW_line.Source)) && (!sigNW_line.Source.Equals(Global_class.No_node_text))) { sources.Add(sigNW_line.Source); }
                if ((!string.IsNullOrEmpty(sigNW_line.Target)) && (!sigNW_line.Target.Equals(Global_class.No_node_text))) { targets.Add(sigNW_line.Target); }
            }
            UniqueNodes.Set_sorted_unique_nodes_and_index(sources, targets);
            UniqueNodes.Includes_unique_nodes = true;
        }
        public void Set_unique_nodes_and_fill_NW_with_sourceIndexes_and_targetIndexes()
        {
            Set_unique_nodes();
            Fill_index_of_sources_and_targets_based_on_names();
        }
        #endregion

        #region Input, Output, deep_copy, upcast, compare
        public void Write_file(string subdirectory, string file_name)
        {
            UniqueNodes.Small_correctness_check();
            SigNWBasis_writeOptions_class write_options = new SigNWBasis_writeOptions_class(subdirectory, file_name);
            if (file_name.Length > 0)
            {
                write_options.File = write_options.Directory_results + file_name;
            }
            ReadWriteClass.WriteData<T>(SigNW, write_options);
            file_name.Replace(".sig", "");
            file_name = file_name + "_UN.jns";
            UniqueNodes.Write_unique_nodes(file_name);
        }
        public void Write_file(string file_name)
        {
            Write_file("", file_name);
        }
        public SigNWBasis_class<T> Deep_copy_without_uniqueNodes(bool report)
        {
            SigNWBasis_class<T> new_sigNW = (SigNWBasis_class<T>)this.MemberwiseClone();
            new_sigNW.sigNW = new List<T>();
            foreach (SigNWBasis_line_class line in sigNW)
            {
                new_sigNW.sigNW.Add((T)line.Deep_copy());
            }
            new_sigNW.UniqueNodes = new Node_class();
            new_sigNW.Clear_uniqueNodes_and_source_target_indexes();
            new_sigNW.UniqueNodes = new Node_class();
            return new_sigNW;
        }
        public SigNWBasis_class<T> Deep_copy(bool report)
        {
            if (report) { Report_class.WriteLine("{0}: Create a deep copy of network", typeof(NetworkBasis_class).Name); }
            UniqueNodes.Small_correctness_check();
            SigNWBasis_class<T> new_sigNW = Deep_copy_without_uniqueNodes(report);
            new_sigNW.UniqueNodes = this.UniqueNodes.Deep_copy();
            if (UniqueNodes.Includes_unique_nodes)
            {
                bool report_old = new_sigNW.UniqueNodes.Report;
                new_sigNW.UniqueNodes.Report = false;
                new_sigNW.Fill_index_of_sources_and_targets_based_on_names();
                new_sigNW.UniqueNodes.Report = report_old;
            }
            return new_sigNW;
        }
        public void Deep_copy_input_into_this(SigNWBasis_class<T> input)
        {
            input.UniqueNodes.Small_correctness_check();
            sigNW = new List<T>();
            foreach (T line in input.sigNW)
            {
                T newLine = (T)line.Deep_copy();
                sigNW.Add(newLine);
            }
            UniqueNodes = input.UniqueNodes.Deep_copy();
        }
        #endregion

    }

    //////////////////////////////////////////////////////////////////////////////////////

}
