using System;
using System.Collections.Generic;
using System.Drawing;
using Network_visualization;
using Common_classes;
using Statistic;

namespace Network
{
    enum Regular_node_label_enum { E_m_p_t_y, Name1, Name2, Name1_plus_nwIndex }
    enum Regular_node_color_enum { E_m_p_t_y, Score_of_interest, Node_classification, Selected_color, Selected_color_size_by_value }
    enum Regular_node_shape_enum { E_m_p_t_y, Mbc, GO_regulation_vs_normal, Ellipse, Scps_genes, Scps_regulatoryscps_genes }

    class Visualization_of_regularNW_options : Visualization_of_nw_options
    {
        #region Fields
        public bool Report { get; set; }

        public Regular_node_color_enum Node_color { get; set; }
        public Regular_node_label_enum Node_label { get; set; }
        public Regular_node_shape_enum Node_shape { get; set; }
        public float Node_label_font_size { get; set; }
        public int Standard_node_height { get; set; }
        public int Standard_node_width{ get; set; }
        public float Factor_for_max_node_height { get; set; }
        public float Factor_for_max_node_width { get; set; }
        public float Factor_for_min_node_height { get; set; }
        public float Factor_for_min_node_width { get; set; }
        #endregion

        public Visualization_of_regularNW_options()
        {
            Report = true;
            Node_color = Regular_node_color_enum.Score_of_interest;
            Node_label = Regular_node_label_enum.Name2;
            Node_shape = Regular_node_shape_enum.Mbc;
            Standard_node_height = 50;
            Standard_node_width = 50;
            Factor_for_max_node_height = 3F;
            Factor_for_min_node_height = 0.5F;
            Factor_for_max_node_width = Factor_for_max_node_height;
            Factor_for_min_node_width = Factor_for_min_node_height;
            Node_label_font_size = 40;
        }

    }

    //////////////////////////////////////////////////////

    class Visualization_of_ontologyNW_class : Visualization_of_nw_basis
    {
        #region Fields
        Visualization_of_regularNW_options RegularOptions { get; set; }
        #endregion

        public Visualization_of_ontologyNW_class(Visualization_of_regularNW_options options)
        {
            RegularOptions = options;
            Options = options;
        }

        #region Nodes
        private string Get_hexadecimal_nodeColor_gradient(float nodeValue, float maxValue, float minValue)
        {
            if (float.IsPositiveInfinity(nodeValue))
            {
                nodeValue = float.MaxValue;
            }
            if (float.IsPositiveInfinity(maxValue))
            {
                maxValue = float.MaxValue;
            }
            float relative_node_value = (nodeValue - minValue) / (maxValue - minValue);
            int red = -1;
            int green = -1;
            int blue = -1;
            int max_blue = 100;
            int max_green = 150;
            int max_red = 255;
            float step_size = 0.33333333333333333333333333333333F;
            if (relative_node_value < step_size)  // Blue --> green
            {
                red = 0;
                green = (int)Math.Round(relative_node_value / step_size * max_green);
                blue = max_blue - (int)Math.Round(relative_node_value / step_size * max_blue);
            }
            else if ((relative_node_value >= step_size) && (relative_node_value < 2 * step_size)) // Green --> Yellow
            {
                red = (int)Math.Round((relative_node_value - step_size) / step_size * max_red);
                green = max_green;
                blue = 0;
            }
            else   // Yellow --> red
            {
                red = max_red;
                green = max_green - (int)Math.Round((relative_node_value - 2 * step_size) / step_size * max_green);
                blue = 0;
            }
            return Math_class.Get_hexadecimal_code(red, green, blue);
        }

        private void Set_nodeId_label_and_compartment(ref Visualization_of_nw_node_line visNode_line, Node_line_class un_line)
        {
            visNode_line.Id = (string)un_line.Name.Clone();
            visNode_line.Label_alignement = "center";
            visNode_line.Model_name = "sides";
            visNode_line.Model_position = "s";
            visNode_line.FontSize = RegularOptions.Node_label_font_size;
            switch (RegularOptions.Node_label)
            {
                case Regular_node_label_enum.Name1:
                    visNode_line.Label = (string)un_line.Name.Clone();
                    break;
                case Regular_node_label_enum.Name2:
                    visNode_line.Label = (string)un_line.Name2.Clone();
                    break;
                case Regular_node_label_enum.Name1_plus_nwIndex:
                    visNode_line.Label = (string)un_line.Name.Clone() + "_" + un_line.Index;
                    break;
                default:
                    throw new Exception();
            }
        }

        private void Set_nodeColor_gradient(ref Visualization_of_nw_node_line visNode_line, float nodeValue, float maxValue, float minValue)
        {
            if (nodeValue <= 0)
            {
                visNode_line.Fill_colors = new string[] { Math_class.Get_hexadecimal_code(150, 150, 150) };
            }
            else
            {
                visNode_line.Fill_colors = new string[] { Get_hexadecimal_nodeColor_gradient(nodeValue, maxValue, minValue) };
            }
        }

        private void Set_nodeColor(ref Visualization_of_nw_node_line visNode_line, Node_line_class un_line, float max_value)
        {
            switch (RegularOptions.Node_color)
            {
                case Regular_node_color_enum.Score_of_interest:
                    Set_nodeColor_gradient(ref visNode_line, un_line.Minus_log10_p_value, max_value, 0);
                    break;
                case Regular_node_color_enum.Selected_color:
                    int node_colors = un_line.Node_colors.Length;
                    if (node_colors == 0) { throw new Exception(); }
                    Color_specification_line_class node_color_specification;
                    List<Color_specification_line_class> color_specifications = new List<Color_specification_line_class>();
                    List<string> color_strings = new List<string>();
                    string color_string;
                    for (int indexNode = 0; indexNode < node_colors; indexNode++)
                    {
                        node_color_specification = new Color_specification_line_class();
                        node_color_specification.Fill_color = un_line.Node_colors[indexNode];
                        node_color_specification.Size = 1;
                        color_string = "#" + node_color_specification.Fill_color.R.ToString("X2") + node_color_specification.Fill_color.G.ToString("X2") + node_color_specification.Fill_color.B.ToString("X2");
                        color_strings.Add(color_string);
                        color_specifications.Add(node_color_specification);
                    }
                    visNode_line.Color_specifications = color_specifications.ToArray();
                    visNode_line.Fill_colors = color_strings.ToArray();
                    break;
                case Regular_node_color_enum.Selected_color_size_by_value:
                    int node_color_values = un_line.Node_color_values.Length;
                    if (node_color_values == 0) { throw new Exception(); }
                    List<Color_specification_line_class> value_color_specifications = new List<Color_specification_line_class>();
                    List<string> color_value_strings = new List<string>();
                    string color_value_string;
                    for (int indexNodeValueColors = 0; indexNodeValueColors < node_color_values; indexNodeValueColors++)
                    {
                        node_color_specification = new Color_specification_line_class();
                        node_color_specification.Fill_color = un_line.Node_color_values[indexNodeValueColors].Node_color;
                        node_color_specification.Size = un_line.Node_color_values[indexNodeValueColors].Node_color_value;
                        color_value_string = "#" + node_color_specification.Fill_color.R.ToString("X2") + node_color_specification.Fill_color.G.ToString("X2") + node_color_specification.Fill_color.B.ToString("X2");
                        color_value_strings.Add(color_value_string);
                        value_color_specifications.Add(node_color_specification);
                    }
                    visNode_line.Color_specifications = value_color_specifications.ToArray();
                    visNode_line.Fill_colors = color_value_strings.ToArray();
                    break;
                case Regular_node_color_enum.Node_classification:
                    node_color_specification = new Color_specification_line_class();
                    node_color_specification.Size = 1;
                    switch (un_line.Classification)
                    {
                        case Node_classification_enum.Intermediate:
                            node_color_specification.Fill_color = Color.LightGray;
                            break;
                        case Node_classification_enum.Seednode:
                            node_color_specification.Fill_color = Color.Orange;
                            break;
                        case Node_classification_enum.Sinknode:
                            node_color_specification.Fill_color = Color.LightBlue;
                            break;
                        case Node_classification_enum.Seedsinknode:
                            node_color_specification.Fill_color = Color.LimeGreen;
                            break;
                        default:
                            node_color_specification.Fill_color = Color.White;
                            break;
                    }
                    visNode_line.Color_specifications = new Color_specification_line_class[] { node_color_specification };
                    break;
                default:
                    throw new Exception();
            }
        }

        private void Set_nodeShape(ref Visualization_of_nw_node_line visNode_line, Node_line_class un_line)
        {
            switch (RegularOptions.Node_shape)
            {
                case Regular_node_shape_enum.Ellipse:
                    visNode_line.Shape_type = "ellipse";
                    visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                    visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                    break;
                case Regular_node_shape_enum.Mbc:
                    if (un_line.Name.Contains("Mbc:"))
                    {
                        visNode_line.Shape_type = "rectangle";
                        visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                        visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                    }
                    else
                    {
                        visNode_line.Shape_type = "ellipse";
                        visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                        visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                    }
                    break;
                case Regular_node_shape_enum.GO_regulation_vs_normal:
                    if (un_line.Name2.Contains("regulation"))
                    {
                        visNode_line.Shape_type = "rectangle";
                        visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                        visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                    }
                    else
                    {
                        visNode_line.Shape_type = "ellipse";
                        visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                        visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                    }
                    break;
                case Regular_node_shape_enum.Scps_regulatoryscps_genes:
                    switch (un_line.Classification)
                    {
                        case Node_classification_enum.Scp:
                            visNode_line.Shape_type = "ellipse";
                            visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                            visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                            break;
                        case Node_classification_enum.Regulatory_scp:
                            visNode_line.Shape_type = "rectangle";
                            visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                            visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                            break;
                        case Node_classification_enum.Gene:
                            visNode_line.Shape_type = "diamond";
                            visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                            visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                            break;
                        default:
                            throw new Exception();
                    }
                    break;
                case Regular_node_shape_enum.Scps_genes:
                    switch (un_line.Classification)
                    {
                        case Node_classification_enum.Gene:
                            visNode_line.Shape_type = "ellipse";
                            visNode_line.Geometry_heigth = (int)Math.Round(0.666*(double)RegularOptions.Standard_node_height);
                            visNode_line.Geometry_width = (int)Math.Round(0.666 *(double)RegularOptions.Standard_node_width);
                            break;
                        case Node_classification_enum.Scp:
                        default:
                            visNode_line.Shape_type = "rectangle";
                            visNode_line.Geometry_heigth = RegularOptions.Standard_node_height;
                            visNode_line.Geometry_width = RegularOptions.Standard_node_width;
                            break;
                    }
                    break;
                default:
                    throw new Exception();
            }
        }

        private void Generate_pre_nodes(Node_class uniqueNodes)
        {
            int un_length = uniqueNodes.UN.Count;
            Node_line_class un_line;
            Visualization_of_nw_node_line visNode_line;
            List<Visualization_of_nw_node_line> visNodes_list = new List<Visualization_of_nw_node_line>();
            float max_value = Options.Max_score_of_interest;
            float min_value = Options.Min_score_of_interest;
            for (int indexUN = 0; indexUN < un_length; indexUN++)
            {
                un_line = uniqueNodes.UN[indexUN];
                visNode_line = new Visualization_of_nw_node_line();
                Set_nodeId_label_and_compartment(ref visNode_line, un_line);
                Set_nodeColor(ref visNode_line, un_line, max_value);
                Set_nodeShape(ref visNode_line, un_line);
                visNodes_list.Add(visNode_line);
            }
            VisNodes = visNodes_list.ToArray();
        }

        private void Adjust_nodeSizes_if_specified()
        {
            if (RegularOptions.Node_color.Equals(Regular_node_color_enum.Selected_color_size_by_value))
            {
                float max_color_values_for_node = -1;
                float min_color_values_for_node = -1;
                int visNodes_length = this.VisNodes.Length;
                Visualization_of_nw_node_line node_line;
                float currentNode_color_values;
                for (int indexVisu=0; indexVisu<visNodes_length; indexVisu++)
                {
                    node_line = this.VisNodes[indexVisu];
                    currentNode_color_values = 0;
                    foreach (Color_specification_line_class color_specification_line in node_line.Color_specifications)
                    {
                        currentNode_color_values += color_specification_line.Size;
                    }
                    if ((max_color_values_for_node == -1)
                        || (currentNode_color_values > max_color_values_for_node))
                    {
                        max_color_values_for_node = currentNode_color_values;
                    }
                    if ((min_color_values_for_node == -1)
                        || (currentNode_color_values < min_color_values_for_node))
                    {
                        min_color_values_for_node = currentNode_color_values;
                    }
                }
                float max_node_width = (float)RegularOptions.Standard_node_width * RegularOptions.Factor_for_max_node_width;
                float min_node_width = (float)RegularOptions.Standard_node_width * RegularOptions.Factor_for_min_node_width;
                float range_node_width = max_node_width - min_node_width;
                float max_node_height = (float)RegularOptions.Standard_node_height * RegularOptions.Factor_for_max_node_height;
                float min_node_height = (float)RegularOptions.Standard_node_height * RegularOptions.Factor_for_min_node_height;
                float range_node_height = max_node_height - min_node_height;
                float scale_factor_height;
                float scale_factor_width;
                for (int indexVisu = 0; indexVisu < visNodes_length; indexVisu++)
                {
                    node_line = this.VisNodes[indexVisu];
                    currentNode_color_values = 0;
                    foreach (Color_specification_line_class color_specification_line in node_line.Color_specifications)
                    {
                        currentNode_color_values += color_specification_line.Size;
                    }
                    scale_factor_height = (currentNode_color_values - min_color_values_for_node) / (max_color_values_for_node - min_color_values_for_node);
                    scale_factor_width = (currentNode_color_values - min_color_values_for_node) / (max_color_values_for_node - min_color_values_for_node);
                    node_line.Geometry_heigth = min_node_height + scale_factor_height * range_node_height;
                    node_line.Geometry_width = min_node_width + scale_factor_width * range_node_width;
                }
            }
        }

        private void Generate_nodes(NetworkBasis_class onto_nw)
        {
            Generate_pre_nodes(onto_nw.UniqueNodes);
            Adjust_nodeSizes_if_specified();
        }
        #endregion

        #region Edges
        private void Generate_edges(NetworkBasis_class nw)
        {
            int nw_count = nw.NW.Count;
            NetworkBasis_line_class nw_line;
            nw.UniqueNodes.Order_by_index();
            Node_line_class source_node_line;
            Node_line_class target_node_line;
            int targets_count;
            int indexTarget;
            List<Visualization_of_nw_edge_line> visEdges_list = new List<Visualization_of_nw_edge_line>();
            for (int indexNW=0; indexNW<nw_count; indexNW++)
            {
                source_node_line = nw.UniqueNodes.Get_individual_node_line(indexNW);
                nw_line = nw.NW[indexNW];
                targets_count = nw_line.Targets.Count;
                for (int indexTargetIndex=0; indexTargetIndex<targets_count; indexTargetIndex++)
                {
                    indexTarget = nw_line.Targets[indexTargetIndex];
                    target_node_line = nw.UniqueNodes.Get_individual_node_line(indexTarget);
                    Visualization_of_nw_edge_line edge_line = new Visualization_of_nw_edge_line();
                    edge_line.Arrow_color = Math_class.Get_hexadecimal_black();
                    edge_line.Edge_id = source_node_line.Name + "_to_" + target_node_line.Name;
                    edge_line.Source_id = (string)source_node_line.Name.Clone();
                    edge_line.Target_id = (string)target_node_line.Name.Clone();
                    edge_line.Arrow_source_end = "none";
                    edge_line.Arrow_target_end = "standard";
                    edge_line.Arrow_type = "line";
                    edge_line.Arrow_width = "1";
                    visEdges_list.Add(edge_line);
                }
            }
            VisEdges = visEdges_list.ToArray();
        }
        #endregion

        #region Legend
        private float Get_rounded_value(float value)
        {
            return (float)Math.Round(value * 100) / 100;
        }

        private Visualization_of_nw_node_line Generate_legend_node(float value, float max_value, float min_value)
        {
            Visualization_of_nw_node_line nw_node_line = new Visualization_of_nw_node_line();
            nw_node_line.Id = "Legend_" + Get_rounded_value(value).ToString();
            nw_node_line.Compartment = Legend.Legend_label;
            nw_node_line.Label = value.ToString();
            nw_node_line.Model_name = "sides";
            nw_node_line.Model_position = "e";
            nw_node_line.Label_alignement = "east";
            nw_node_line.Shape_type = "rectangle";
            nw_node_line.Geometry_heigth = RegularOptions.Standard_node_height;
            nw_node_line.Geometry_width = RegularOptions.Standard_node_width;
            nw_node_line.Fill_colors = new string[] { Get_hexadecimal_nodeColor_gradient(value, max_value, min_value) };
            return nw_node_line;
        }

        private void Generate_legend(NetworkBasis_class nw)
        {
            if (!Options.Legend_title.Equals(Global_class.Empty_entry))
            {
                Legend.Legend_label = (string)Options.Legend_title.Clone();
            }
            float max_score_of_interest = Options.Max_score_of_interest;
            float min_score_of_interest = Options.Min_score_of_interest;
            int legend_entries_length = Options.Legend_count_of_entries;
            Legend.VisNodes = new Visualization_of_nw_node_line[legend_entries_length];
            float value;
            for (int indexN = 0; indexN < legend_entries_length; indexN++)
            {
                value = min_score_of_interest + indexN * (max_score_of_interest-min_score_of_interest) / (legend_entries_length-1);
                Legend.VisNodes[indexN] = Generate_legend_node(value, max_score_of_interest, min_score_of_interest);
            }
        }
        #endregion

        public void Generate(NetworkBasis_class nw)
        {
            Generate_nodes(nw);
            Generate_edges(nw);
            if (Options.Include_legend)
            {
                Generate_legend(nw);
            }
            Generate_resource_lines_and_connect_to_nodes();
        }
    }

    //////////////////////////////////////////////////////

    class RegularNW_generate_visualization_class : NetworkBasis_class
    {
        public Visualization_of_regularNW_options Options { get; set; }

        public RegularNW_generate_visualization_class()
        {
            Options = new Visualization_of_regularNW_options();
        }

        public Visualization_of_nw_basis Generate_visualization_instance(NetworkBasis_class nw)
        {
            base.Deep_copy_into_this(nw);
            Visualization_of_ontologyNW_class visu = new Visualization_of_ontologyNW_class(this.Options);
            visu.Generate(this);
            return visu;
        }

    }
}
