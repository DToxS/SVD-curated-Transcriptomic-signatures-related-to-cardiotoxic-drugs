// private NetworkBasis_class Generate_subnetwork(int[] subnetwork_indexes_old, List<Node_line_class> seed_nodes_old)
// does not consider duplicated seed_nodes_old


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;
using Highthroughput_data;
using Common_classes;
using Statistic;
using ReadWrite;

namespace Network
{
    class Node_color_value_line_class
    {
        public Color Node_color { get; set; }
        public float Node_color_value { get; set; }

        public Node_color_value_line_class Deep_copy()
        {
            Node_color_value_line_class copy = (Node_color_value_line_class)this.MemberwiseClone();
            return copy;
        }
    }

    class Node_line_class : IFill_de
    {
        #region Fields
        public string Name { get; internal set; }
        public string Name2 { get; internal set; }
        public Color[] Node_colors { get; set; }
        public Node_color_value_line_class[] Node_color_values { get; set; }
        public int Index { get; internal set; }
        public Node_classification_enum Classification { get; internal set; }
        public Node_nw_classification_enum NW_classification { get; internal set; }
        public Node_de_enum DE { get; set; }
        public Color C_sharp_color { get; set; }
        //public Node_GO_MF_effect GO_mf_final_effect { get; internal set; }
        public int Degree { get; set; }
        public int Integer { get; set; }
        public string String { get; set; }
        public int IndexOld { get; internal set; }
        public float Seed_node_neighbor_ratio { get; set; }
        //public float Z_score { get; internal set; }
        public float Minus_log10_p_value { get; internal set; }
        //public float Module_distance_score { get; internal set; } 
        public List<Timepoint_enum> Timepoints { get; set; }
        public List<double> Log2_fc_changes { get; set; }
        #endregion

        #region readWrite Fields
        public string ReadWrite_dataset_colors
        {
            get
            { return ReadWriteClass.Get_writeLine_from_array<Color>(Node_colors, Node_writeOptions_class.Get_array_delimiter()); }
            set
            { Node_colors = ReadWriteClass.Get_array_from_readLine<Color>(value, Node_writeOptions_class.Get_array_delimiter()); }
        }

        public string ReadWrite_timepoints
        {
            get
            { return ReadWriteClass.Get_writeLine_from_array<Timepoint_enum>(Timepoints.ToArray(), Node_writeOptions_class.Get_array_delimiter()); }
            set
            { Timepoints = ReadWriteClass.Get_array_from_readLine<Timepoint_enum>(value, Node_writeOptions_class.Get_array_delimiter()).ToList(); }
        }

        public string ReadWrite_log2_fc_changes
        {
            get
            { return ReadWriteClass.Get_writeLine_from_array<double>(Log2_fc_changes.ToArray(), Node_writeOptions_class.Get_array_delimiter()); }
            set
            { Log2_fc_changes = ReadWriteClass.Get_array_from_readLine<double>(value, Node_writeOptions_class.Get_array_delimiter()).ToList(); }
        }
        #endregion

        #region Fields for de
        public string[] Symbols_for_de { get { return new string[] { Name }; } }
        public Timepoint_enum Timepoint_for_de { get; set; }
        public double Value_for_de { get; set; }
        public DE_entry_enum Entry_type_for_de { get; set; }
        public string[] Names_for_de { get; set; }
        #endregion

        public Node_line_class()
        {
            Name = Global_class.Empty_entry;
            Name2 = Global_class.Empty_entry;
            Index = -1;
            IndexOld = -1;
            Minus_log10_p_value = 0;
            Degree = -1;
            Integer = -1;
            String = "";
            Names_for_de = new string[] { "" };
            Classification = Node_classification_enum.E_m_p_t_y;
            NW_classification = Node_nw_classification_enum.E_m_p_t_y;
            Node_color_values = new Node_color_value_line_class[0];
            DE = Node_de_enum.E_m_p_t_y;
            Timepoints = new List<Timepoint_enum>();
            Log2_fc_changes = new List<double>();
            Node_colors = new Color[0];
        }

        public bool Equal_lines(Node_line_class other, string[] Node_line_compare_props)
        {
            bool equals = true;
            PropertyInfo[] propInfo = typeof(Node_line_class).GetProperties();
            foreach (PropertyInfo prop in propInfo)
            {
                if ((Node_line_compare_props.Length == 0) || (Node_line_compare_props.Contains(prop.Name)))
                {
                    if (!prop.GetValue(this, null).Equals(prop.GetValue(other, null))) { equals = false; }
                }
            }
            return equals;
        }

        public bool Equal_name_name2_and_index(Node_line_class other)
        {
            bool equals = true;
            if (!this.Name.Equals(other.Name)) { equals = false; }
            if (!this.Name2.Equals(other.Name2)) { equals = false; }
            if (!this.Index.Equals(other.Index)) { equals = false; }
            return equals;
        }

        public Node_line_class Deep_copy()
        {
            Node_line_class newLine = (Node_line_class)this.MemberwiseClone();
            newLine.Timepoints = new List<Timepoint_enum>();
            int timepoints_count = Timepoints.Count;
            for (int indexTime = 0; indexTime < timepoints_count; indexTime++)
            {
                newLine.Timepoints.Add(this.Timepoints[indexTime]);
            }
            newLine.Name = (string)Name.Clone();
            newLine.String = (string)String.Clone();
            if (this.Name2 != null) { newLine.Name2 = (string)Name2.Clone(); }
            int node_color_values_length = this.Node_color_values.Length;
            newLine.Node_color_values = new Node_color_value_line_class[node_color_values_length];
            for (int indexCV = 0; indexCV < node_color_values_length; indexCV++)
            {
                newLine.Node_color_values[indexCV] = this.Node_color_values[indexCV].Deep_copy();
            }
            int colors_length = this.Node_colors.Length;
            newLine.Node_colors = new Color[colors_length];
            for (int indexC = 0; indexC < colors_length; indexC++)
            {
                newLine.Node_colors[indexC] = this.Node_colors[indexC];
            }
            return newLine;
        }
    }

    class Node_writeOptions_class : ReadWriteOptions_base
    {
        const char array_delimiter = ',';
        public string Directory_results { get; set; }

        public static char Get_array_delimiter()
        {
            return array_delimiter;
        }

        public Node_writeOptions_class(string subdirectory, string fileName, bool report)
        {
            Directory_results = Global_directory_class.Results_directory;
            File = Directory_results + subdirectory + fileName;
            Key_propertyNames = new string[] { "Index", "Name", "Name2", "Classification", "NW_classification", "DE", "ReadWrite_timepoints", "ReadWrite_log2_fc_changes", "Minus_log10_p_value", "Seed_node_neighbor_ratio", "Degree", "Node_colors" };
            Key_columnNames = Key_propertyNames;
            Key_columnIndexes = null;

            SafeCondition_columnIndexes = null;
            SafeCondition_columnNames = null;
            SafeCondition_entries = null;

            File_has_headline = true;
            RemoveFromHeadline = null;
            LineDelimiters = new char[] { Global_class.Tab };
            HeadlineDelimiters = new char[] { Global_class.Tab };

            if (report) { Report = ReadWrite_report_enum.Report_main; }
            else { Report = ReadWrite_report_enum.E_m_p_t_y; }
        }
    }

    class Node_class
    {
        #region Fields
        const float seednode_minus_log10_pvalue = 9999;

        private List<Node_line_class> un;
        public List<Node_line_class> UN { get { return un; } set { un = value; } }
        //   public string NetworkName { get; set; }
        public bool Report { get; set; }
        public bool Index_changes_adopted { get; set; }
        public bool Includes_unique_nodes { get; set; }
        private Network_direction_type_enum network_direction;
        public Network_direction_type_enum Network_direction
        {
            get
            {
                return network_direction;
            }
            set
            {
                // if (network_direction == Network_direction_type_enum.E_m_p_t_y)
                // {
                network_direction = value;
                // }
                // else
                // {
                //     Report_class.Write_error_line("{0}: current network direction: {1}, change from outside not allowed", typeof(Node_class).Name, Network_direction);
                // }
            }
        }
        public Organism_enum Organism { get; set; }

        public float SeedNode_minus_log10_pvalue { get { return seednode_minus_log10_pvalue; } }
        #endregion

        #region Constructors
        public Node_class()
        {
            Generate();
        }
        #endregion

        #region Generate
        public void Generate()
        {
            un = new List<Node_line_class>();
            Report = true;
            Includes_unique_nodes = true;
            Index_changes_adopted = false;
        }
        #endregion

        public Dictionary<int, int> Get_oldIndex_index_dict()
        {
            Dictionary<int, int> oldIndex_newIndex_dict = new Dictionary<int, int>();
            int un_count = this.UN.Count;
            Node_line_class un_line;
            for (int indexUN = 0; indexUN < un_count; indexUN++)
            {
                un_line = this.UN[indexUN];
                if (un_line.IndexOld != -1)
                {
                    oldIndex_newIndex_dict.Add(un_line.IndexOld, un_line.Index);
                }
            }
            return oldIndex_newIndex_dict;
        }

        public int Get_max_index()
        {
            int un_count = this.UN.Count;
            int max_index = -1;
            Node_line_class un_line;
            for (int indexUN = 0; indexUN < un_count; indexUN++)
            {
                un_line = this.UN[indexUN];
                if (un_line.Index > max_index)
                {
                    max_index = un_line.Index;
                }
            }
            return max_index;
        }

        #region Order Nodes
        public void Order_by_name()
        {
            Dictionary<string, List<Node_line_class>> name_dict = new Dictionary<string, List<Node_line_class>>();
            int un_length = un.Count;
            Node_line_class node_line;
            for (int indexNode = 0; indexNode < un_length; indexNode++)
            {
                node_line = this.un[indexNode];
                if (!name_dict.ContainsKey(node_line.Name))
                {
                    name_dict.Add(node_line.Name, new List<Node_line_class>());
                }
                name_dict[node_line.Name].Add(node_line);
            }
            string[] names = name_dict.Keys.ToArray();
            string name;
            int names_length = names.Length;
            names = names.OrderBy(l => l).ToArray();
            List<Node_line_class> ordered_nodes = new List<Node_line_class>();
            for (int indexName = 0; indexName < names_length; indexName++)
            {
                name = names[indexName];
                ordered_nodes.AddRange(name_dict[name]);
            }
            un = ordered_nodes;
        }

        public void Order_by_name2()
        {
            un = un.OrderBy(l => l.Name2).ToList();
        }

        public void Order_by_indexOld()
        {
            un = un.OrderBy(l => l.IndexOld).ToList();
        }

        public void Order_by_index()
        {
            un = un.OrderBy(l => l.Index).ToList();
        }

        public void Order_by_degree_and_then_by_index()
        {
            un = un.OrderBy(l => l.Degree).ThenBy(l => l.Index).ToList();
        }

        public int Order_by_index_and_get_last_source_node_index_plus_one()
        {
            int pos;
            int lastSourceNodeIndex;
            Order_by_index_and_get_lastSourceIndex_and_firstTargetIndex(out lastSourceNodeIndex, out pos);
            return lastSourceNodeIndex + 1;
        }

        public void Order_by_index_and_get_lastSourceIndex_and_firstTargetIndex(out int lastSourceIndex, out int firstTargetIndex)
        {
            Order_by_index();
            lastSourceIndex = un.Count - 1;
            firstTargetIndex = 0;
        }
        #endregion

        #region Clear
        public void Clear_unique_nodes()
        {
            un.Clear();
            Index_changes_adopted = false;
            Includes_unique_nodes = false;
        }
        public void Clear_node_classification()
        {
            Node_classification_enum classification = Node_classification_enum.E_m_p_t_y;
            foreach (Node_line_class node_line in un)
            {
                node_line.Classification = classification;
            }
        }
        #endregion

        #region Set nodes
        private void Set_sorted_unique_nodes_add(List<string> nodes, Node_nw_classification_enum NW_classification)
        {
            nodes = nodes.Distinct().ToList();
            nodes = nodes.OrderBy(l => l).ToList();
            int nodes_count = nodes.Count;
            for (int i = 0; i < nodes_count; i++)
            {
                if ((i == nodes_count - 1) || (nodes[i] != nodes[i + 1]))
                {
                    if (nodes[i] != null)
                    {
                        Node_line_class line = new Node_line_class();
                        line.Name = nodes[i];
                        line.NW_classification = NW_classification;
                        un.Add(line);
                    }
                }
            }
        }
        public void Set_sorted_unique_nodes_and_index(List<string> sources, List<string> targets)
        {
            un.Clear();
            Index_changes_adopted = true;
            List<string> nodes = new List<string>();
            nodes.AddRange(sources);
            nodes.AddRange(targets);
            Set_sorted_unique_nodes_add(nodes, Node_nw_classification_enum.E_m_p_t_y);
            Index_nodes_and_set_index_old();
        }
        #endregion

        #region Change node characteristics
        public void Add_selected_color_to_nodes(string[] seedNodes, Color node_color)
        {
            seedNodes = seedNodes.Distinct().OrderBy(l => l).ToArray();
            Order_by_name();
            int un_count = un.Count;
            int seedNodes_length = seedNodes.Length;
            int indexSeed = 0;
            int intCompare;
            Node_line_class un_line;
            List<Color> new_colors = new List<Color>();
            for (int indexUN = 0; indexUN < un_count; indexUN++)
            {
                intCompare = -2;
                un_line = un[indexUN];
                if (indexSeed < seedNodes_length)
                {
                    while ((indexSeed < seedNodes_length) && (intCompare < 0))
                    {
                        intCompare = seedNodes[indexSeed].CompareTo(un_line.Name);
                        if (intCompare < 0) { indexSeed++; }
                        if (intCompare == 0)
                        {
                            new_colors.Clear();
                            new_colors.AddRange(un_line.Node_colors);
                            new_colors.Add(node_color);
                            un_line.Node_colors = new_colors.ToArray();
                        }
                    }
                }
            }
        }
        public void Add_selected_color_to_nodes(Dictionary<string, List<Color>> seedNodes_color_dict)
        {
            Order_by_name();
            int un_count = un.Count;
            Node_line_class un_line;
            List<Color> new_colors = new List<Color>();
            for (int indexUN = 0; indexUN < un_count; indexUN++)
            {
                un_line = un[indexUN];
                if (seedNodes_color_dict.ContainsKey(un_line.Name))
                {
                    new_colors.Clear();
                    new_colors.AddRange(un_line.Node_colors);
                    new_colors.AddRange(seedNodes_color_dict[un_line.Name]);
                    un_line.Node_colors = new_colors.ToArray();
                }
            }
        }
        public void Add_color_to_all_nodes_with_no_colors(Color empty_color)
        {
            foreach (Node_line_class un_line in un)
            {
                if (un_line.Node_colors.Length == 0)
                {
                    un_line.Node_colors = new Color[] { empty_color };
                }
            }
        }
        #endregion

        #region Get
        public Node_line_class Get_individual_node_line(int indexUN)
        {
            Node_line_class un_line = UN[indexUN];
            if (un_line.Index != indexUN)
            {
                throw new Exception();
            }
            else
            {
                return un_line; ;
            }
        }
        #endregion

        #region Check
        public bool Small_correctness_check()
        {
            bool everything_correct = true;
            if (Includes_unique_nodes == true)
            {
                if (Report) { Report_class.Write("{0}: Small correctness check: ", typeof(Node_class).Name); }
                if (un.Count == 0)
                {
                    everything_correct = false;
                    throw new Exception();
                }
                if ((Index_changes_adopted == false))
                {
                    everything_correct = false;
                    throw new Exception();
                }
                if (Network_direction == Network_direction_type_enum.E_m_p_t_y)
                {
                    everything_correct = false;
                    throw new Exception();
                }
                //Test for duplicated nodes
                Order_by_name();
                int un_count = UN.Count;
                Node_line_class un_line;
                for (int indexUN = 0; indexUN < un_count; indexUN++)
                {
                    un_line = un[indexUN];
                    if ((indexUN != 0) && (un_line.Name.Equals(un[indexUN - 1].Name)))
                    {
                        everything_correct = false;
                        throw new Exception();
                    }
                }
                if ((Report) && (everything_correct)) { Report_class.WriteLine("OK"); }
                if (!everything_correct) { throw new Exception("see console"); }
            }
            return everything_correct;
        }
        public bool Complete_correctness_check()
        {
            bool everything_correct = Small_correctness_check();
            if (Includes_unique_nodes)
            {
                if (Report) { Report_class.Write("{0}: Complete correctness check:", typeof(Node_class).Name); }
                if ((Report) && (everything_correct)) { Report_class.WriteLine("OK"); }
                if (!everything_correct) { throw new Exception("see console"); }
            }
            return everything_correct;
        }
        public bool Check_if_only_unique_nodes()
        {
            bool only_unique_nodes = true;
            Order_by_name();
            int un_count = un.Count;
            for (int indexUN = 1; indexUN < un_count; indexUN++)
            {
                if (un[indexUN - 1].Name.Equals(un[indexUN].Name))
                {
                    only_unique_nodes = false;
                }
            }
            if (!only_unique_nodes)
            {
                throw new Exception();
            }
            return only_unique_nodes;
        }
        #endregion

        #region Set_nodes (including index changes)
        public void Index_nodes_and_set_index_old()
        {
            if (Report) { Report_class.WriteLine("{0}: Index nodes", typeof(Node_class).Name); }
            if (un.Count > 0)
            {
                Small_correctness_check();
                un = un.OrderBy(l => l.Name).ToList();
                int newUN_count = un.Count;
                for (int i = 0; i < newUN_count; i++)
                {
                    un[i].IndexOld = un[i].Index;
                    un[i].Index = i;
                }
            }
            Index_changes_adopted = false;
        }
        public void Add_nodes_and_reindex(params string[] addNodes)
        {
            if (Report) { Report_class.WriteLine("{0}: Add nodes and reindex", typeof(Node_class).Name); }
            if (Small_correctness_check())
            {
                addNodes = addNodes.Distinct().OrderBy(l => l).ToArray();
                int add_length = addNodes.Length;
                Order_by_name();
                int un_count = un.Count;
                int indexUN = 0;
                int stringCompare;
                Node_line_class un_line;
                for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
                {
                    stringCompare = -2;
                    while ((indexUN < un_count) && (stringCompare < 0))
                    {
                        un_line = un[indexUN];
                        stringCompare = un_line.Name.CompareTo(addNodes[indexAdd]);
                        if (stringCompare < 0) { indexUN++; }
                    }
                    if (stringCompare != 0)
                    {
                        Node_line_class new_un_line = new Node_line_class();
                        new_un_line.Name = addNodes[indexAdd];
                        un.Add(new_un_line);
                    }
                }
                Check_if_only_unique_nodes();
                Index_nodes_and_set_index_old();
            }
        }
        #endregion

        #region Copy and output
        public void Print_number_of_unique_nodes(string propName)
        {
            Report_class.WriteLine("{0}: node counts based on {1}", typeof(Node_class).Name, propName);
            PropertyInfo[] propInfo = typeof(Node_line_class).GetProperties();
            int propIndex = -1;
            for (int i = 0; i < propInfo.Length; i++)
            {
                if (propInfo[i].Name == propName) { propIndex = i; }
            }
            if (propIndex == -1)
            {
                throw new Exception();
            }
            switch (propName)
            {
                case "NW_classification":
                    un = un.OrderBy(l => l.NW_classification).ToList();
                    break;
                case "Classification":
                    un = un.OrderBy(l => l.Classification).ToList();
                    break;
                case "DE":
                    un = un.OrderBy(l => l.DE).ToList();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Report_class.WriteLine("{0}: propName not considered for orderBy, data unsorted", typeof(Node_class).Name);
                    Console.ResetColor();
                    break;
            }
            int uniqueNodes_count = un.Count();
            int count = 1;
            for (int indexNodes = 1; indexNodes < uniqueNodes_count; indexNodes++)
            {
                if ((indexNodes < uniqueNodes_count) && (propInfo[propIndex].GetValue(un[indexNodes], null).Equals(propInfo[propIndex].GetValue(un[indexNodes - 1], null))))
                {
                    count = count + 1;
                }
                else
                {
                    for (int i = 0; i < typeof(Node_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                    Report_class.WriteLine("{0} unique nodes {1}", count, propInfo[propIndex].GetValue(un[indexNodes - 1], null));
                    count = 1;
                }
            }
            for (int i = 0; i < typeof(Node_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("{0} unique nodes {1}", count, propInfo[propIndex].GetValue(un[uniqueNodes_count - 1], null));
        }
        public void Print_report()
        {
            Report_class.WriteLine("{0}: Print report", typeof(Node_class).Name);
            for (int i = 0; i < typeof(Node_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("Network direction: {0}", Network_direction);
            for (int i = 0; i < typeof(Node_class).Name.Length + 2; i++) { Report_class.Write(" "); }
            Report_class.WriteLine("Organism: {0}", Organism);
            Print_number_of_unique_nodes("Classification");
        }
        public void Shallow_copy_into_this(Node_class other)
        {
            this.un = other.un;
            this.Report = other.Report;
            this.Index_changes_adopted = other.Index_changes_adopted;
            this.Includes_unique_nodes = other.Includes_unique_nodes;
            this.network_direction = other.network_direction;
        }
        public void Deep_copy_into_this_without_UN(Node_class other)
        {
            Report = other.Report;
            Index_changes_adopted = other.Index_changes_adopted;
            Includes_unique_nodes = other.Includes_unique_nodes;
            network_direction = other.Network_direction;
            Organism = other.Organism;
            un = new List<Node_line_class>();
        }
        public void Deep_copy_into_this(Node_class other)
        {
            Deep_copy_into_this_without_UN(other);
            int un_count = other.UN.Count;
            for (int indexUN = 0; indexUN < un_count; indexUN++)
            {
                this.UN.Add(other.UN[indexUN].Deep_copy());
            }
        }
        public Node_class Deep_copy_without_UN()
        {
            Node_class newUN = (Node_class)this.MemberwiseClone();
            newUN.un = new List<Node_line_class>();
            return newUN;
        }
        public Node_class Deep_copy()
        {
            Small_correctness_check();
            Node_class newUN = Deep_copy_without_UN();
            foreach (Node_line_class this_line in this.un)
            {
                newUN.un.Add(this_line.Deep_copy());
            }
            return newUN;
        }
        public void Write_unique_nodes(string subdirectory, string fileName)
        {
            Small_correctness_check();
            Node_writeOptions_class writeOptions = new Node_writeOptions_class(subdirectory, fileName, Report);
            ReadWriteClass.WriteData<Node_line_class>(un, writeOptions);
        }
        public void Write_unique_nodes(string fileName)
        {
            Write_unique_nodes("", fileName);
        }
        #endregion
    }

    ////////////////////////////////////////////////////////////////

    class NetworkBasis_line_class
    {
        #region Fields
        public List<int> Targets { get; set; }
        #endregion

        #region Constructor
        public NetworkBasis_line_class()
        {
            Targets = new List<int>();
        }
        #endregion

        public void Add_not_existing_targets_and_order(List<int> addTargets)
        {
            List<int> rmTargets = new List<int>();
            this.Targets.AddRange(addTargets);
            this.Targets = this.Targets.Distinct().OrderBy(l => l).ToList();
        }
        public void Add_not_existing_targets_remove_stated_targets_and_order(List<int> addTargets, List<int> rmTargets)
        {
            //throw new Exception();////Check code
            List<int> newTargets = new List<int>();
            addTargets = addTargets.OrderBy(l => l).ToList();
            this.Targets = this.Targets.OrderBy(l => l).ToList();
            rmTargets = rmTargets.OrderBy(l => l).ToList();
            int indexAdd = 0;
            int indexThis = 0;
            int indexRM = 0;
            int this_count = Targets.Count;
            int add_count = addTargets.Count;
            int rm_count = rmTargets.Count;
            int addCompare = -2;
            int rmCompare = -2;
            while ((indexAdd < add_count) || (indexThis < this_count))
            {
                if ((indexAdd < add_count) && (indexThis < this_count)) { addCompare = addTargets[indexAdd] - this.Targets[indexThis]; }
                if (indexThis == this_count) { addCompare = -1; }
                if (indexAdd == add_count) { addCompare = 1; }

                //Check if target to be added is part of rmTargets
                if ((indexRM < rm_count) && (addCompare < 0))
                {
                    rmCompare = -2;
                    while ((indexRM < rm_count) && (rmCompare < 0))
                    {
                        rmCompare = rmTargets[indexRM] - addTargets[indexAdd];
                        if (rmCompare < 0) { indexRM++; }
                    }
                }
                else if ((indexRM < rm_count) && (addCompare >= 0))
                {
                    rmCompare = -2;
                    while ((indexRM < rm_count) && (rmCompare < 0))
                    {
                        rmCompare = rmTargets[indexRM] - this.Targets[indexThis];
                        if (rmCompare < 0) { indexRM++; }
                    }
                }
                else
                {
                    rmCompare = 2;
                }

                //Add target to be added, if not part of to be removed targets
                if (addCompare < 0)
                {
                    if ((rmCompare > 0) || (indexRM == rm_count)) { newTargets.Add(addTargets[indexAdd]); }
                    indexAdd++;
                }
                else if (addCompare > 0)
                {
                    if ((rmCompare > 0) || (indexRM == rm_count)) { newTargets.Add(this.Targets[indexThis]); }
                    indexThis++;
                }
                else if (addCompare == 0)
                {
                    if ((rmCompare > 0) || (indexRM == rm_count)) { newTargets.Add(this.Targets[indexThis]); }
                    indexAdd++;
                    indexThis++;
                }
            }
            this.Targets = newTargets;
        }
        public bool Remove_targets_and_report(params int[] rmTargets)
        {
            rmTargets = rmTargets.OrderBy(l=>l).ToArray();
            this.Targets = this.Targets.OrderBy(l => l).ToList();
            int this_target;
            int rmTarget;
            int rm_length = rmTargets.Length;
            int indexRm = 0;
            int target_compare;
            int this_length = this.Targets.Count;
            List<int> newTargets = new List<int>();
            bool target_removed = false;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                this_target = this.Targets[indexThis];
                target_compare = -2;
                while ((indexRm < rm_length) && (target_compare < 0))
                {
                    rmTarget = rmTargets[indexRm];
                    target_compare = rmTarget.CompareTo(this_target);
                    if (target_compare < 0)
                    {
                        indexRm++;
                    }
                    else if (target_compare == 0)
                    {
                        target_removed = true;
                    }
                }
                if (target_compare != 0)
                {
                    newTargets.Add(this_target);
                }

            }
            Targets = newTargets;
            return target_removed;
        }
        public void Clear_targets()
        {
            Targets.Clear();
        }
        public bool Contains_at_least_one_target_of_input_targets(int[] input_targets)
        {
            input_targets = input_targets.OrderBy(l => l).ToArray();
            int input_length = input_targets.Length;
            int targets_count = Targets.Count;
            int intCompare;
            int indexInput=0;
            int target;
            bool contains_at_least_one_input_target = false;
            for (int indexT = 0; indexT < targets_count; indexT++)
            {
                intCompare = -2;
                target = Targets[indexT];
                while ((indexInput < input_length) && (intCompare < 0))
                {
                    intCompare = input_targets[indexInput] - target;
                    if (intCompare < 0)
                    {
                        indexInput++;
                    }
                    else if (intCompare == 0)
                    {
                        contains_at_least_one_input_target = true;
                        break;
                    }
                }
            }
            return contains_at_least_one_input_target;
        }
        public int Count_overlapping_targets(NetworkBasis_line_class other)
        {
            int indexThis = 0;
            int indexOther = 0;
            int this_count = this.Targets.Count;
            int other_count = other.Targets.Count;
            int intCompare = -2;
            int overlap_count = 0;
            while ((indexThis < this_count) && (indexOther < other_count))
            {
                intCompare = this.Targets[indexThis] - other.Targets[indexOther];
                if (intCompare < 0) { indexThis++; }
                if (intCompare > 0) { indexOther++; }
                if (intCompare == 0)
                {
                    overlap_count++;
                    indexThis++;
                    indexOther++;
                }
            }
            return overlap_count;
        }
        public int Count_non_self_targets(int sourceIndex)
        {
            int non_self_target_count = 0;
            foreach (int target in Targets)
            {
                if (target != sourceIndex)
                {
                    non_self_target_count++;
                }
            }
            return non_self_target_count;
        }
        public float Calculate_jaccard_index_for_targets_excluding_sources(NetworkBasis_line_class other, int thisSourceIndex, int otherSourceIndex)
        {
            int this_count = this.Targets.Count;
            int other_count = other.Targets.Count;

            //Count number of overlapping targets and check, if otherSourceIndex in this.Target and vice verse
            int indexThis = 0;
            int indexOther = 0;
            int intCompare = -2;
            int overlap_count = 0;
            int otherSource_in_thisTargets = 0;
            int thisSource_in_otherTargets = 0;
            while ((indexThis < this_count) || (indexOther < other_count))
            {
                if ((indexThis < this_count) && (indexOther < other_count))
                {
                    intCompare = this.Targets[indexThis] - other.Targets[indexOther];
                }
                else if (indexThis < this_count)
                {
                    intCompare = -2;
                }
                else // indexOther < other_count
                {
                    intCompare = 2;
                }

                if (intCompare < 0)
                {
                    if (this.Targets[indexThis] == otherSourceIndex)
                    {
                        overlap_count++;
                    }
                    else if (this.Targets[indexThis] == thisSourceIndex)
                    {
                        Report_class.WriteLine("{0}: Self interacting edges were not removed for calculation of lc jaccard index", typeof(NetworkBasis_line_class).Name);
                        throw new Exception();
                    }
                    indexThis++;
                }
                if (intCompare > 0)
                {
                    if (other.Targets[indexOther] == thisSourceIndex)
                    {
                        overlap_count++;
                    }
                    else if (other.Targets[indexOther] == otherSourceIndex)
                    {
                        Report_class.WriteLine("{0}: Self interacting edges were not removed for calculation of lc jaccard index", typeof(NetworkBasis_line_class).Name);
                        throw new Exception();
                    }
                    indexOther++;
                }
                if (intCompare == 0)
                {
                    overlap_count++;
                    indexThis++;
                    indexOther++;
                }
            }

            if ((this_count != 0) || (other_count != 0))
            {
                float jaccard = ((float)overlap_count + thisSource_in_otherTargets + otherSource_in_thisTargets) / (float)(this_count + other_count + 2 - overlap_count); //+2 for the sourceNodes
                return jaccard;
            }
            else { return 0; }
        }
        public void Increase_all_targetIndexes_by_stated_number(int increase_by)
        {
            int target_count = Targets.Count;
            for (int indexTarget = 0; indexTarget < target_count; indexTarget++)
            {
                Targets[indexTarget] = +increase_by;
            }
        }
        public NetworkBasis_line_class Deep_copy()
        {
            NetworkBasis_line_class newLine = (NetworkBasis_line_class)this.MemberwiseClone();
            newLine.Targets = new List<int>();
            newLine.Targets.AddRange(this.Targets);
            return newLine;
        }
    }

    class NetworkBasis_class
    {
        #region Fields
        public List<NetworkBasis_line_class> NW { get; protected set; }
        public Node_class UniqueNodes { get; private set; }
        #endregion

        #region Constructors
        public NetworkBasis_class()
        {
            UniqueNodes = new Node_class();
            NW = new List<NetworkBasis_line_class>();
        }
        public NetworkBasis_class(SigNWBasis_class<SigNWBasis_line_class> sig) : this()
        {
             Generate_network_from_sigNW(sig);
        }
        #endregion

        #region Generate
        public void Generate_network_from_sigNW(SigNWBasis_class<SigNWBasis_line_class> sig)
        {
            if (UniqueNodes.Report)
            {
                Report_class.WriteLine("-------------------------------------------------------------------------------");
                Report_class.WriteLine("{0}: Generate {1} Network from {2}", typeof(NetworkBasis_class).Name, sig.UniqueNodes.Network_direction, typeof(SigNWBasis_class<SigNWBasis_line_class>).Name);
                Report_class.WriteLine();

                Report_class.WriteLine("{0}: Generate sources and fill with targets", typeof(NetworkBasis_class).Name);
            }
            sig.UniqueNodes.Complete_correctness_check();
            UniqueNodes = sig.UniqueNodes.Deep_copy();

            int sigNW_count = sig.SigNW.Count;
            NetworkBasis_line_class nwLine;
            int target_negative_index_count = 0;
            int targetName_no_node_count = 0;
            int indexSig = 0;
            SigNWBasis_line_class sigNW_line;
            Node_line_class un_line;
            int sourceIndexCompare;
            int un_lastSourceNode_index = UniqueNodes.Order_by_index_and_get_last_source_node_index_plus_one();
            sig.Order_sigNW_by_sourceIndex_and_targetIndex();
            NetworkBasis_line_class[] nw_array = new NetworkBasis_line_class[un_lastSourceNode_index];
            for (int indexUN = 0; indexUN < un_lastSourceNode_index; indexUN++)
            {
                un_line = UniqueNodes.UN[indexUN];
                nwLine = new NetworkBasis_line_class();
                sourceIndexCompare = -2;
                while ((indexSig < sigNW_count) && (sourceIndexCompare < 0))
                {
                    sigNW_line = sig.SigNW[indexSig];
                    sourceIndexCompare = sigNW_line.Source_index - un_line.Index;
                    if (sourceIndexCompare < 0)
                    {
                        if ((sigNW_line.Source_index == -1)||(string.IsNullOrEmpty(sigNW_line.Source))||(sigNW_line.Source.Equals(Global_class.No_node_text)))
                        { 
                            throw new Exception();
                        }
                        indexSig++;
                    }
                    else if (sourceIndexCompare == 0)
                    {
                        while ((indexSig < sigNW_count) && (un_line.Index == sig.SigNW[indexSig].Source_index))
                        {
                            sigNW_line = sig.SigNW[indexSig];
                            if (sigNW_line.Target_index == -1) 
                            { 
                                target_negative_index_count++; 
                            }
                            else 
                            {
                                if (   (!UniqueNodes.UN[sigNW_line.Target_index].Index.Equals(sigNW_line.Target_index))
                                    || (!UniqueNodes.UN[sigNW_line.Target_index].Name.Equals(sigNW_line.Target)))
                                {
                                    throw new Exception();
                                }
                                nwLine.Targets.Add(sigNW_line.Target_index); 
                            }
                            if (sigNW_line.Target.Equals(Global_class.No_node_text)) { targetName_no_node_count++; }
                            if (string.IsNullOrEmpty(sigNW_line.Target)) 
                            {
                                throw new Exception();
                            }
                            indexSig++;
                        }
                        if (!un_line.Name.Equals(sigNW_line.Source))
                        {
                            throw new Exception();
                        }
                    }
                }
                nw_array[un_line.Index] = nwLine;
            }
            NW = nw_array.ToList();
            if (indexSig < sigNW_count)
            {
                throw new Exception();
            }

            if (UniqueNodes.Report)
            {
                Report_class.WriteLine("{0}: {1} targetIndexes = -1 and {2} targets = {3}", typeof(NetworkBasis_class).Name, target_negative_index_count, targetName_no_node_count, Global_class.No_node_text);
            }
            if (targetName_no_node_count != target_negative_index_count) 
            {
                throw new Exception();
            }
            if (UniqueNodes.Report)
            {
                Report_class.WriteLine("-------------------------------------------------------------------------------");
                Report_class.WriteLine();
            }
        }
        #endregion

        #region add nodes
        public void Add_nodes(params string[] addNodes)
        {
            UniqueNodes.Add_nodes_and_reindex(addNodes);
            Reindex_network();
        }
        #endregion

        #region change network
        private void Reindex_network()
        {
            if (UniqueNodes.Report) { Report_class.WriteLine("{0}: Reindex network", typeof(NetworkBasis_class).Name); }
            if (UniqueNodes.UN.Count > 0)
            {
                int nw_count = NW.Count;
                int un_count = UniqueNodes.UN.Count;
                List<int> transformIndex = new List<int>();
                Dictionary<int, int> oldIndex_newIndex_dict = this.UniqueNodes.Get_oldIndex_index_dict();
                int max_index = this.UniqueNodes.Get_max_index();
                NetworkBasis_line_class nw_line;
                List<NetworkBasis_line_class> new_nw = new List<NetworkBasis_line_class>();
                for (int indexUN =0; indexUN<= max_index; indexUN++)
                {
                    new_nw.Add(new NetworkBasis_line_class());
                }
                int targets_count;
                List<int> new_targets = new List<int>();
                int target;
                for (int indexOldNW=0; indexOldNW<nw_count; indexOldNW++)
                {
                    nw_line = this.NW[indexOldNW];
                    if (oldIndex_newIndex_dict.ContainsKey(indexOldNW))
                    {
                        targets_count = nw_line.Targets.Count;
                        new_targets = new List<int>();
                        for (int indexT = 0; indexT < targets_count; indexT++)
                        {
                            target = nw_line.Targets[indexT];
                            if (oldIndex_newIndex_dict.ContainsKey(target))
                            {
                                new_targets.Add(oldIndex_newIndex_dict[target]);
                            }
                        }
                        nw_line.Targets = new_targets.OrderBy(l => l).ToList();
                        if (new_nw[oldIndex_newIndex_dict[indexOldNW]].Targets.Count>0) { throw new Exception(); }
                        new_nw[oldIndex_newIndex_dict[indexOldNW]] = nw_line;
                    }
                }
                NW = new_nw;
                UniqueNodes.Index_changes_adopted = true;
            }
            else
            {
                NW.Clear();
            }
            if (UniqueNodes.Report) { Report_class.WriteLine(); }
        }
        private void Converge_network_over_rmNodes(int[] rmNodeIndexes, bool repeat)
        {
            if (repeat)
            {
                repeat = false;
                Array.Sort(rmNodeIndexes);
                int rm_length = rmNodeIndexes.Length;
                int indexRM2 = 0;
                NetworkBasis_line_class actNW_line;
                List<int> addTargets = new List<int>();
                List<int> rmTargets = new List<int>();
                int intCompare = -2;
                for (int indexRM = 0; indexRM < rm_length; indexRM++)
                {
                    actNW_line = NW[rmNodeIndexes[indexRM]];
                    indexRM2 = 0;
                    foreach (int target in actNW_line.Targets)
                    {
                        intCompare = -2;
                        while ((indexRM2 < rm_length) && (intCompare < 0))
                        {
                            intCompare = rmNodeIndexes[indexRM2] - target;
                            if (intCompare < 0) { indexRM2++; }
                        }
                        if (intCompare == 0)
                        {
                            addTargets = NW[target].Targets;
                            rmTargets.Clear();
                            rmTargets.Add(target);
                            actNW_line.Add_not_existing_targets_remove_stated_targets_and_order(addTargets, rmTargets);
                            repeat = true;
                        }
                    }
                }
                Converge_network_over_rmNodes(rmNodeIndexes, repeat);
            }
        }
        public void Remove_self_interacting_edges()
        {
            if (UniqueNodes.Report) { Report_class.WriteLine("{0}: Remove self interacting edges", typeof(NetworkBasis_class).Name); }
            int nw_count = NW.Count;
            int edges_removed = 0;
            for (int indexSource = 0;indexSource<nw_count;indexSource++)
            {
                if (NW[indexSource].Remove_targets_and_report(indexSource))
                {
                    edges_removed++;
                }
            }
            if (UniqueNodes.Report)
            {
                for (int i = 0; i < typeof(NetworkBasis_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("{0} edges removed", edges_removed);
            }
        }
        #endregion

        #region Copy, read, write
        public void Deep_copy_into_this(NetworkBasis_class other)
        {
            UniqueNodes.Deep_copy_into_this(other.UniqueNodes);
            int nw_count = other.NW.Count;
            NW = new List<NetworkBasis_line_class>();
            for (int indexNW = 0; indexNW < nw_count; indexNW++)
            {
                this.NW.Add(other.NW[indexNW].Deep_copy());
            }
        }
        #endregion
    }
}
