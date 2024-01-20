using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Common_classes;
using Statistic;
using ReadWrite;

namespace Network_visualization
{
    class Color_specification_line_class
    {
        public Color Fill_color { get; set; }
        public float Size { get; set; }

        public Color_specification_line_class()
        {
            Size = 1;
        }

        public bool Equals_other(Color_specification_line_class other)
        {
            bool equals_other = this.Fill_color.Equals(other.Fill_color) && (this.Size.Equals(other.Size));
            return equals_other;
        }

        public Color_specification_line_class Deep_copy()
        {
            Color_specification_line_class copy = (Color_specification_line_class)this.MemberwiseClone();
            return copy;
        }
    }

    class Visualization_of_nw_node_line
    {
        #region Fields
        public string Id { get; set; }
        public string Label { get; set; }
        public string Label_intern { get; set; }
        public float Geometry_heigth { get; set; }
        public float Geometry_width { get; set; }
        public string[] Fill_colors { get; set; }
        public float Weight_for_multiple_datasets { get; set; }
        public Color_specification_line_class[] Color_specifications { get; set; }
        public float FontSize { get; set; }
        public string FontStyle { get; set; }
        public string LabelHasLineColor { get; set; }
        public string TextColor { get; set; }
        public string Model_name { get; set; }
        public string Model_position { get; set; }
        public string Shape_type { get; set; }
        public string Transparent { get; set; }
        public string Border_style_color { get; set; }
        public float Border_style_width { get; set; }
        public string Label_alignement { get; set; }
        public string Compartment { get; set; }
        public string Cell_type { get; set; }
        public string Resource_id { get; set; }
        public bool Merge_by_generating_pie_chart { get; set; }
        #endregion

        public bool SameColorSpecification_in_same_order(Visualization_of_nw_node_line other)
        {
            int this_fill_colors_csharp_length = this.Color_specifications.Length;
            int other_fill_colors_csharp_length = other.Color_specifications.Length;
            bool sameFillColors = true;
            if (this_fill_colors_csharp_length != other_fill_colors_csharp_length)
            {
                sameFillColors = false;
            }
            else
            {
                for (int indexC = 0; indexC < this_fill_colors_csharp_length; indexC++)
                {
                    if ((!this.Color_specifications[indexC].Equals_other(other.Color_specifications[indexC])))
                    {
                        sameFillColors = false;
                        break;
                    }
                }
            }
            return sameFillColors;
        }

        public Visualization_of_nw_node_line()
        {
            Resource_id = "";
            Geometry_heigth = 30;
            Geometry_width = 30;
            Fill_colors = new string[] { Math_class.Get_hexadecimal_code(166, 166, 166) };
            FontSize = 14;
            FontStyle = "plain";
            Model_name = "internal";
            Model_position = "s";
            Shape_type = "ellipse";
            Transparent = "false";
            LabelHasLineColor = "false";
            TextColor = "#000000";
            Border_style_color = "#000000";
            Border_style_width = 1.0F;
            Label_alignement = "center";
            Label_intern = "";
        }

        public Visualization_of_nw_node_line Deep_copy()
        {
            Visualization_of_nw_node_line copy = (Visualization_of_nw_node_line)this.MemberwiseClone();
            copy.Id = (string) this.Id.Clone();
            copy.Label = (string) this.Label.Clone();
            copy.Label_intern = (string)this.Label_intern.Clone();
            copy.Fill_colors = Array_class.Deep_copy_string_array(this.Fill_colors);
            int color_specifications_length = this.Color_specifications.Length;
            copy.Color_specifications = new Color_specification_line_class[color_specifications_length];
            for (int indexCS=0; indexCS<color_specifications_length; indexCS++)
            {
                copy.Color_specifications[indexCS] = this.Color_specifications[indexCS].Deep_copy();
            }
            copy.FontStyle = (string) this.FontStyle.Clone();
            copy.LabelHasLineColor = (string) this.LabelHasLineColor.Clone();
            copy.TextColor = (string) this.TextColor.Clone();
            copy.Model_name = (string) this.Model_name.Clone();
            copy.Shape_type = (string) this.Shape_type.Clone();
            copy.Transparent = (string) this.Transparent.Clone();
            copy.Border_style_color = (string) this.Border_style_color.Clone();
            copy.Label_alignement = (string) this.Label_alignement.Clone();
            if (!string.IsNullOrEmpty(this.Compartment)) { copy.Compartment = (string)this.Compartment.Clone(); }
            if (!string.IsNullOrEmpty(this.Cell_type)) { copy.Cell_type = (string)this.Cell_type.Clone(); }
            return copy;
        }
    }

    class Visualization_of_nw_edge_line
    {
        #region Fields
        public string Target_id { get; set; }
        public string Edge_id { get; set; }
        public string Reaction_summary_index { get; set; }
        public string Source_id { get; set; }
        public string Arrow_source_end { get; set; }
        public string Arrow_target_end { get; set; }
        public string Arrow_color { get; set; }
        public string Arrow_type { get; set; }
        public string Arrow_width { get; set; }
        public string Arrow_label { get; set; }
        public string Arrow_label_font_size { get; set; }
        #endregion

        public Visualization_of_nw_edge_line()
        {
            Arrow_source_end = "";
            Arrow_target_end = "";
            Arrow_color = "#000000";
            Arrow_type = "line";
            Arrow_width = "1.0";
            Arrow_label = "";
            Arrow_label_font_size = "15";
        }

        public Visualization_of_nw_edge_line Deep_copy()
        {
            Visualization_of_nw_edge_line copy = (Visualization_of_nw_edge_line)this.MemberwiseClone();
            copy.Target_id = (string)this.Target_id.Clone();
            copy.Source_id = (string)this.Source_id.Clone();
            copy.Edge_id = (string)this.Edge_id.Clone();
            copy.Arrow_source_end = (string) this.Arrow_source_end.Clone();
            copy.Arrow_color = (string)this.Arrow_color.Clone();
            copy.Arrow_target_end = (string)this.Arrow_target_end.Clone();
            copy.Arrow_type = (string)this.Arrow_type.Clone();
            copy.Arrow_width = (string)this.Arrow_width.Clone();
            copy.Arrow_label = (string)this.Arrow_label.Clone();
            copy.Arrow_label_font_size = (string)this.Arrow_label_font_size.Clone();
            return copy;
        }
    }

    class Visualization_of_nw_resource_line_class
    {
        public string Resource_id { get; set; }
        public string Base64String { get; set; }

        public Visualization_of_nw_resource_line_class Deep_copy()
        {
            Visualization_of_nw_resource_line_class copy = (Visualization_of_nw_resource_line_class)this.MemberwiseClone();
            copy.Resource_id = (string)this.Resource_id.Clone();
            copy.Base64String = (string)this.Base64String.Clone();
            return copy;
        }
    }

    class Visualization_of_nw_legend_line_class
    {
        public float Value {get;set;}
        public string Label {get;set;}
    }

    class Visualization_of_nw_legend_class
    {
        public Visualization_of_nw_node_line[] VisNodes { get; set; }
        public Visualization_of_nw_edge_line[] VisEdges { get; set; }
        public string Legend_label { get; set; }

        public Visualization_of_nw_legend_class()
        {
            Legend_label = "Legend";
            VisNodes = new Visualization_of_nw_node_line[0];
            VisEdges = new Visualization_of_nw_edge_line[0];
        }

        public void Order_nodes_by_compartment_and_id()
        {
            VisNodes = VisNodes.OrderBy(l => l.Compartment).ThenBy(l => l.Id).ToArray();
        }

        public void Order_edges_by_id()
        {
            VisEdges = VisEdges.OrderBy(l => l.Edge_id).ToArray();
        }

    }

    class Visualization_of_nw_options
    {
        public bool Consider_node_groups { get; set; }
        public string Headline { get; set; }
        public bool Include_legend { get; set; }
        public int Legend_count_of_entries { get; set; }
        public string Legend_title { get; set; }
        public float Max_score_of_interest { get; set; }
        public float Min_score_of_interest { get; set; }
        public Color[] Cell_type_colors { get; set; }

        public Visualization_of_nw_options()
        {
            Consider_node_groups = false;
            Headline = null;
            Include_legend = false;
            Legend_count_of_entries = 5;
            Max_score_of_interest = -1;
            Min_score_of_interest = -1;
            Legend_title = Global_class.Empty_entry;
            Cell_type_colors = new Color[] { Color.Blue, Color.Red, Color.OrangeRed, Color.Orchid, Color.MediumPurple, Color.Green, Color.Black, Color.White, Color.AntiqueWhite, Color.Blue, Color.DodgerBlue, Color.OrangeRed,Color.DarkRed, Color.Gray, Color.Gray, Color.RoyalBlue, Color.LightSlateGray, Color.DarkOliveGreen, Color.DarkOrange, Color.Blue, Color.Red, Color.OrangeRed, Color.Orchid, Color.MediumPurple, Color.Green, Color.Black, Color.White, Color.AntiqueWhite, Color.Blue, Color.DodgerBlue, Color.OrangeRed, Color.DarkRed, Color.Gray, Color.Gray, Color.RoyalBlue, Color.LightSlateGray, Color.DarkOliveGreen, Color.DarkOrange };
        }
    }

    class Visualization_of_nw_basis
    {
        #region Fields
        public Visualization_of_nw_node_line[] VisNodes { get; set; }
        public Visualization_of_nw_edge_line[] VisEdges { get; set; }
        public Visualization_of_nw_resource_line_class[] VisResources { get; set; }
        public Visualization_of_nw_legend_class Legend { get; set; }
        public Visualization_of_nw_options Options { get; set; }
        public int VisNodes_length { get { return VisNodes.Length; } }
        public int VisEdges_length { get { return VisEdges.Length; } }
        #endregion

        #region Order
        public void Order_nodes_by_cell_type_compartment_and_id()
        {
            VisNodes = VisNodes.OrderBy(l=>l.Cell_type).ThenBy(l => l.Compartment).ThenBy(l => l.Id).ToArray();
        }

        public void Order_nodes_by_id()
        {
            VisNodes = VisNodes.OrderBy(l => l.Id).ToArray();
        }

        public void Order_edges_by_id()
        {
            VisEdges = VisEdges.OrderBy(l => l.Edge_id).ToArray();
        }
        #endregion

        public Visualization_of_nw_basis()
        {
            Legend = new Visualization_of_nw_legend_class();
            this.VisResources = new Visualization_of_nw_resource_line_class[0];
            this.VisNodes = new Visualization_of_nw_node_line[0];
            this.VisEdges = new Visualization_of_nw_edge_line[0];
        }

        private bool Check_if_all_nodes_are_conncected_to_at_least_one_edge()
        {
            bool ok = true;
            int visNodes_length = VisNodes_length;
            int visEdges_length = VisEdges_length;
            Order_edges_by_id();
            Order_nodes_by_id();
            List<Visualization_of_nw_node_line> node_not_in_edges = new List<Visualization_of_nw_node_line>();
            int stringCompare = -2;
            Visualization_of_nw_edge_line edge_line;
            Visualization_of_nw_node_line node_line;
            int indexEdges = 0;
            for (int indexN = 0; indexN < visNodes_length; indexN++)
            {
                node_line = VisNodes[indexN];
                stringCompare = -2;
                while ((indexEdges < visEdges_length) && (stringCompare < 0))
                {
                    edge_line = VisEdges[indexEdges];
                    stringCompare = edge_line.Edge_id.CompareTo(node_line.Id);
                    if (stringCompare < 0)
                    {
                        indexEdges++;
                    }
                }
                if (stringCompare != 0)
                {
                    node_not_in_edges.Add(node_line);
                }
            }
            int node_not_in_edges_count = node_not_in_edges.Count;
            string[] node_ids = new string[node_not_in_edges_count];
            for (int indexN = 0; indexN < node_not_in_edges_count; indexN++)
            {
                node_line = node_not_in_edges[indexN];
                node_ids[indexN] = (string)node_line.Id.Clone();
            }
            if (node_not_in_edges_count > 0)
            {
                throw new Exception();
            }
            return ok;
        }

        private bool Check_if_all_edgeIds_have_nodeIds()
        {
            bool ok = true;
            int edges_length = VisEdges.Length;
            int nodes_length = VisNodes.Length;
            Visualization_of_nw_node_line visu_node_line;
            Visualization_of_nw_edge_line visu_edge_line;
            List<string> allEdgeNodeId_list = new List<string>();
            List<string> allNodeId_list = new List<string>();
            for (int indexEdge = 0; indexEdge < edges_length; indexEdge++)
            {
                visu_edge_line = this.VisEdges[indexEdge];
                allEdgeNodeId_list.Add(visu_edge_line.Source_id);
                allEdgeNodeId_list.Add(visu_edge_line.Target_id);
            }
            string[] allEdgeNodeIds = allEdgeNodeId_list.Distinct().OrderBy(l => l).ToArray();
            for (int indexNode = 0; indexNode < nodes_length; indexNode++)
            {
                visu_node_line = this.VisNodes[indexNode];
                allNodeId_list.Add(visu_node_line.Id);
            }
            string[] allNodeIds = allNodeId_list.Distinct().OrderBy(l=>l).ToArray();

            int allNodes_length = allNodeIds.Length;
            int allEdgeNodes_length = allEdgeNodeIds.Length;
            if (allNodes_length < allEdgeNodes_length)
            {
                throw new Exception();
            }

            int indexN = 0;
            int stringCompare;
            for (int indexE = 0; indexE < allEdgeNodes_length; indexE++)
            {
                stringCompare = -2;
                while ((indexN < nodes_length) && (stringCompare < 0))
                {
                    stringCompare = allNodeIds[indexN].CompareTo(allEdgeNodeIds[indexE]);
                    if (stringCompare < 0)
                    {
                        indexN++;
                    }
                }
                if (stringCompare!=0)
                {
                    throw new Exception();
                }
            }
            return ok;
        }

        public bool Correctness_check()
        {
            bool final_ok = true;
            bool ok;
           // ok = Check_if_all_nodes_are_conncected_to_at_least_one_edge();
           // if (!ok) { final_ok = ok; }
            ok = Check_if_all_edgeIds_have_nodeIds();
            if (!ok) { final_ok = ok; }
            return final_ok;
        }

        #region Resources
        private Visualization_of_nw_resource_line_class[] Generate_resource_lines_with_pieChart_and_connect_to_nodes_for_same_color_specification_length_length(ref List<Visualization_of_nw_node_line> sameFillColorsLength_nodes)
        {
            int colors_length = sameFillColorsLength_nodes[0].Color_specifications.Length;
            int sameFillColorsLength_nodes_length = sameFillColorsLength_nodes.Count;
            Visualization_of_nw_resource_line_class resource_line;
            List<Visualization_of_nw_resource_line_class> new_resources = new List<Visualization_of_nw_resource_line_class>();
            Visualization_of_nw_node_line visualization_node_line;
            Visualization_of_nw_node_line inner_visualization_node_line;
            int pieChart_no = -1;
            string pieChart_id = "";
            for (int indexColor = colors_length - 1; indexColor < 0; indexColor--)
            {
                sameFillColorsLength_nodes = sameFillColorsLength_nodes.OrderBy(l => l.Color_specifications[indexColor]).ToList();
            }
            int firstIndexSameResource = -1;
            for (int indexSFN = 0; indexSFN < sameFillColorsLength_nodes_length; indexSFN++)
            {
                visualization_node_line = sameFillColorsLength_nodes[indexSFN];
                if ((indexSFN == 0) || (!visualization_node_line.SameColorSpecification_in_same_order(sameFillColorsLength_nodes[indexSFN - 1])))
                {
                    firstIndexSameResource = indexSFN;
                }
                if ((indexSFN == sameFillColorsLength_nodes_length - 1) || (!visualization_node_line.SameColorSpecification_in_same_order(sameFillColorsLength_nodes[indexSFN + 1])))
                {
                    pieChart_no++;
                    pieChart_id = "PieChart" + colors_length + "_no" + pieChart_no;
                    for (int indexInner = firstIndexSameResource; indexInner <= indexSFN; indexInner++)
                    {
                        inner_visualization_node_line = sameFillColorsLength_nodes[indexInner];
                        inner_visualization_node_line.Resource_id = (string)pieChart_id.Clone();
                    }
                    resource_line = new Visualization_of_nw_resource_line_class();
                    resource_line.Resource_id = (string)pieChart_id.Clone();

                    Chart pie_chart = new Chart();
                    /* Add a chart area to the chart */
                    ChartArea CA = pie_chart.ChartAreas.Add("A1");
                    CA.Area3DStyle.Enable3D = true;
                    CA.Area3DStyle.Rotation = -180;
                    CA.Area3DStyle.Inclination = 0;
                    CA.BorderColor = Color.Black;
                    CA.BorderWidth = 1;
                    /* Add data series to the chart and specify the type of Series */
                    Series S1 = pie_chart.Series.Add("S1");
                    S1.LabelBorderWidth = 0;
                    S1.LabelBorderColor = Color.Black;
                    S1.BorderColor = Color.Black;
                    S1.ChartType = SeriesChartType.Pie;
                    Color_specification_line_class color_specification_line;
                    //double sum_of_yvalues = 0;
                    for (int indexColor = 0; indexColor < colors_length; indexColor++)
                    {
                        color_specification_line = visualization_node_line.Color_specifications[indexColor];
                        S1.Points.AddXY(indexColor, 1);
                        S1.Points[indexColor].Color = color_specification_line.Fill_color;
                        S1.Points[indexColor].YValues = new double[] { color_specification_line.Size };
                        //sum_of_yvalues += color_specification_line.Size;
                    }
                    S1.BorderWidth = 30;// (int)Math.Round((double)60 / sum_of_yvalues);

                    /* Set chart color and other settings as required */
                    pie_chart.BackColor = Color.Transparent;
                    pie_chart.BorderlineColor = Color.Black;
                    pie_chart.BorderlineWidth = 2;
                    CA.BackColor = pie_chart.BackColor;
                    CA.Area3DStyle.Enable3D = false;

                    /*Assign AntiAliasing to Graphics style for smooth edges*/
                    pie_chart.AntiAliasing = AntiAliasingStyles.Graphics;

                    /* Set the image path and save the image as PNG format*/
                    pie_chart.Height = 2000;
                    pie_chart.Width = 2000;

                    MemoryStream memory = new MemoryStream();
                    pie_chart.SaveImage(memory, ChartImageFormat.Png);

                    byte[] imageBytes = memory.ToArray();

                    string base64String = Convert.ToBase64String(imageBytes);

                    //StreamWriter writer = new StreamWriter(Global_directory_and_file_class.Results_directory + "base64string.txt");
                    //writer.WriteLine(base64String);
                    //writer.Close();

                    resource_line.Base64String = (string)base64String.Clone();
                    new_resources.Add(resource_line);
                }
            }
            return new_resources.ToArray();
        }

        protected void Generate_resource_lines_and_connect_to_nodes()
        {
            List<Visualization_of_nw_resource_line_class> resources = new List<Visualization_of_nw_resource_line_class>();
            this.VisNodes = this.VisNodes.OrderBy(l => l.Color_specifications.Length).ToArray();
            int nodes_length = this.VisNodes.Length;
            Visualization_of_nw_node_line node_line;
            List<Visualization_of_nw_node_line> sameColorLength_node_lines = new List<Visualization_of_nw_node_line>();

            for (int indexN = 0; indexN < nodes_length; indexN++)
            {
                node_line = this.VisNodes[indexN];
                if (node_line.Color_specifications.Length > 1)
                {
                    if ((indexN == 0) || (!node_line.Color_specifications.Length.Equals(this.VisNodes[indexN - 1].Color_specifications.Length)))
                    {
                        sameColorLength_node_lines.Clear();
                    }
                    sameColorLength_node_lines.Add(node_line);
                    if ((indexN == nodes_length - 1) || (!node_line.Color_specifications.Length.Equals(this.VisNodes[indexN + 1].Color_specifications.Length)))
                    {
                        resources.AddRange(Generate_resource_lines_with_pieChart_and_connect_to_nodes_for_same_color_specification_length_length(ref sameColorLength_node_lines));
                    }
                }
            }
            VisResources = resources.ToArray();
        }
        #endregion

        public Visualization_of_nw_basis Deep_copy()
        {
            Visualization_of_nw_basis copy = (Visualization_of_nw_basis)this.MemberwiseClone();
            int visNodes_length = VisNodes_length;
            copy.VisNodes = new Visualization_of_nw_node_line[visNodes_length];
            for (int indexN = 0; indexN < visNodes_length; indexN++)
            {
                copy.VisNodes[indexN] = this.VisNodes[indexN].Deep_copy();
            }
            int visEdges_length = VisEdges_length;
            copy.VisEdges = new Visualization_of_nw_edge_line[visEdges_length];
            for (int indexE = 0; indexE < visEdges_length; indexE++)
            {
                copy.VisEdges[indexE] = this.VisEdges[indexE].Deep_copy();
            }
            int visResources_length = VisResources.Length;
            copy.VisResources = new Visualization_of_nw_resource_line_class[visResources_length];
            for (int indexR = 0; indexR < visResources_length; indexR++)
            {
                copy.VisResources[indexR] = this.VisResources[indexR].Deep_copy();
            }
            return copy;
        }
    }

    //////////////////////////////////////////////////////////
    
    class yED_key_id_class
    {
        #region Fields
        public string Graphml { get; private set; }
        public string Portgraphics { get; private set; }
        public string Portgeometry { get; private set; }
        public string Portuserdata { get; private set; }
        public string Node_url { get; private set; }
        public string Node_description { get; private set; }
        public string Node_graphics { get; private set; }
        public string Resource_graphml { get; set; }
        public string Edge_url { get; private set; }
        public string Edge_description { get; private set; }
        public string Edge_graphics { get; private set; }
        #endregion

        public yED_key_id_class()
        {
            Portgraphics = "d1";
            Portgeometry = "d2";
            Portuserdata = "d3";
            Node_url = "d4";
            Node_description = "d5";
            Node_graphics = "d6";
            Edge_url = "d7";
            Edge_description = "d8";
            Edge_graphics = "d9";
            Resource_graphml = "d10";
        }
    }

    class yED_class
    {
        #region Fields
        private StreamWriter Writer { get; set; }
        private int Shift_text_right { get; set; }
        private Visualization_of_nw_basis Visu_nodeEdges { get; set; }
        private yED_key_id_class Key { get; set; }
        #endregion

        public yED_class()
        {
            Shift_text_right = 0;
            Key = new yED_key_id_class();
        }

        private void Generate()
        {
        }

        #region Head and botton
        private void Write_file_head()
        {
            string file_head =
                  "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\n"
                + "<graphml xmlns=\"http://graphml.graphdrawing.org/xmlns\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:y=\"http://www.yworks.com/xml/graphml\" xmlns:yed=\"http://www.yworks.com/xml/yed/3\" xsi:schemaLocation=\"http://graphml.graphdrawing.org/xmlns http://www.yworks.com/xml/schema/graphml/1.1/ygraphml.xsd\">\n"
                + "  <key for=\"graphml\" id=\"" + Key.Graphml + "\" yfiles.type=\"resources\"/>\n"
                + "  <key attr.name=\"url\" attr.type=\"string\" for=\"node\" id=\"" + Key.Node_url + "\"/>\n"
                + "  <key attr.name=\"description\" attr.type=\"string\" for=\"node\" id=\"" + Key.Node_description + "\"/>\n"
                + "  <key for=\"node\" id=\"" + Key.Node_graphics + "\" yfiles.type=\"nodegraphics\"/>\n"
                + "  <key attr.name=\"url\" attr.type=\"string\" for=\"edge\" id=\"" + Key.Edge_url + "\"/>\n"
                + "  <key attr.name=\"description\" attr.type=\"string\" for=\"edge\" id=\"" + Key.Edge_description + "\"/>\n"
                + "  <key for=\"edge\" id=\"" + Key.Edge_graphics + "\" yfiles.type=\"edgegraphics\"/>\n"
                + "  <key for=\"graphml\" id=\"" + Key.Resource_graphml + "\" yfiles.type=\"resources\"/>\n"
                + "  <graph id=\"G\" edgedefault=\"undirected\">";
            Writer.WriteLine(file_head);
            Shift_text_right += 4;
        }

        private void Write_file_bottom_for_nodes_and_edges()
        {
            Shift_text_right -= 2;
            string file_bottom =
                  "</graph>\n";
            Writer.Write(file_bottom);
        }

        private void Write_final_file_bottom()
        {
            Shift_text_right -= 2;
            string file_bottom =
//                  "  </graph>\n"
//                + "  <data key=\"d0\">\n"
//                + "    <y:Resources/>\n"
//                + "  </data>\n"
                 "</graphml>";
            Writer.Write(file_bottom);
            if (Shift_text_right != 0)
            {
                throw new Exception();
            }
        }
        #endregion

        #region Headline and Legend
        private void Write_headline()
        {
            Visualization_of_nw_node_line node = new Visualization_of_nw_node_line();

            node.Id = "Headline";
            node.Label = Visu_nodeEdges.Options.Headline;
            node.Geometry_heigth = 50;
            node.FontSize = 30;
            node.FontStyle = "bold";
            node.TextColor = Math_class.Get_hexadecimal_code(255, 0, 0);
            node.Geometry_width = node.Label.Length * 20;
            Color_specification_line_class color_specification_line = new Color_specification_line_class();
            color_specification_line.Fill_color = Color.White;
            node.Color_specifications = new Color_specification_line_class[] { color_specification_line };
            node.Model_name = "internal";
            node.Shape_type = "rectangle";
            Write_single_node(node);
        }
        #endregion

        #region Cell type
        private void Write_cell_type_group_start_and_group_boxes(string cell_type)
        {
            string group_id = cell_type.ToString() + "_cell_type_group";
            string graph_id = cell_type.ToString() + "_graph_id";
            string name_open_box = cell_type.ToString();
            string name_closed_box = cell_type.ToString();

            string spaces = Get_spaces_string();
            string text_group_start = spaces + "<node id=\"" + group_id + "\" yfiles.foldertype=\"group\">\n";
            Writer.Write(text_group_start);

            Shift_text_right += 2;
            spaces = Get_spaces_string();
            string text_group_boxes =
                spaces + "<data key=\"" + Key.Node_url + "\"/>\n"
              + spaces + "<data key=\"" + Key.Node_graphics + "\">\n"
              + spaces + "  <y:ProxyAutoBoundsNode>\n"
              + spaces + "    <y:Realizers active=\"0\">\n"
              + spaces + "      <y:GroupNode>\n"
              + spaces + "        <y:Geometry height=\"191.2364196849619\" width=\"353.5269841269842\" x=\"431.6801587301587\" y=\"101.4834899974619\"/>\n"
              + spaces + "        <y:Fill color=\"#F5F5F5\" transparent=\"false\"/>\n"
              + spaces + "        <y:BorderStyle color=\"#000000\" type=\"dashed\" width=\"1.0\"/>\n"
              + spaces + "        <y:NodeLabel alignment=\"right\" autoSizePolicy=\"node_width\" backgroundColor=\"#EBEBEB\" borderDistance=\"0.0\" fontFamily=\"Dialog\" fontSize=\"15\" fontStyle=\"plain\" hasLineColor=\"false\" height=\"22.37646484375\" modelName=\"internal\" modelPosition=\"t\" textColor=\"#000000\" visible=\"true\" width=\"353.5269841269842\" x=\"0.0\" y=\"0.0\">" + name_open_box + "</y:NodeLabel>\n"
              + spaces + "        <y:Shape type=\"roundrectangle\"/>\n"
              + spaces + "        <y:State closed=\"false\" closedHeight=\"50.0\" closedWidth=\"50.0\" innerGraphDisplayEnabled=\"false\"/>\n"
              + spaces + "        <y:Insets bottom=\"15\" bottomF=\"15.0\" left=\"15\" leftF=\"15.0\" right=\"15\" rightF=\"15.0\" top=\"15\" topF=\"15.0\"/>\n"
              + spaces + "        <y:BorderInsets bottom=\"9\" bottomF=\"8.57568359375\" left=\"112\" leftF=\"112.0976190476191\" right=\"104\" rightF=\"103.7509765625\" top=\"0\" topF=\"0.0\"/>\n"
              + spaces + "      </y:GroupNode>\n"
              + spaces + "      <y:GroupNode>\n"
              + spaces + "        <y:Geometry height=\"50.0\" width=\"50.0\" x=\"431.6801587301587\" y=\"101.4834899974619\"/>\n"
              + spaces + "        <y:Fill color=\"#F2F0D8\" transparent=\"false\"/>\n"
              + spaces + "        <y:BorderStyle color=\"#000000\" type=\"line\" width=\"1.0\"/>\n"
              + spaces + "        <y:NodeLabel alignment=\"right\" autoSizePolicy=\"node_width\" backgroundColor=\"#B7B69E\" borderDistance=\"0.0\" fontFamily=\"Dialog\" fontSize=\"15\" fontStyle=\"plain\" hasLineColor=\"false\" height=\"22.37646484375\" modelName=\"internal\" modelPosition=\"t\" textColor=\"#000000\" visible=\"true\" width=\"75.69677734375\" x=\"-12.848388671875\" y=\"0.0\">" + name_closed_box + "</y:NodeLabel>\n"
              + spaces + "        <y:Shape type=\"rectangle\"/>\n"
              + spaces + "        <y:DropShadow color=\"#D2D2D2\" offsetX=\"4\" offsetY=\"4\"/>\n"
              + spaces + "        <y:State closed=\"true\" closedHeight=\"50.0\" closedWidth=\"50.0\" innerGraphDisplayEnabled=\"false\"/>\n"
              + spaces + "        <y:Insets bottom=\"5\" bottomF=\"5.0\" left=\"5\" leftF=\"5.0\" right=\"5\" rightF=\"5.0\" top=\"5\" topF=\"5.0\"/>\n"
              + spaces + "        <y:BorderInsets bottom=\"0\" bottomF=\"0.0\" left=\"0\" leftF=\"0.0\" right=\"0\" rightF=\"0.0\" top=\"0\" topF=\"0.0\"/>\n"
              + spaces + "      </y:GroupNode>\n"
              + spaces + "    </y:Realizers>\n"
              + spaces + "</y:ProxyAutoBoundsNode>\n"
              + spaces + "</data>\n"
              + spaces + "<graph edgedefault=\"directed\" id=\"" + graph_id + ":\">";
            Writer.WriteLine(text_group_boxes);
        }

        private void Write_cell_type_group_end()
        {
            Shift_text_right -= 2;
            string space = Get_spaces_string();
            string text_group_end =
                  space + "  </graph>"
                + space + "</node>\n";
            Writer.WriteLine(text_group_end);
        }
        #endregion

        #region Compartment
        private void Write_compartment_group_start_and_group_boxes(string cell_type, string compartment)
        {
            string group_id = compartment.ToString() + "_compartment_group";
            string graph_id = compartment.ToString() + "_graph_id";
            string name_open_box = compartment.ToString();
            string name_closed_box = compartment.ToString();
            if (!String.IsNullOrEmpty(cell_type))
            {
                group_id = cell_type + "_" + compartment.ToString() + "_compartment_group";
                graph_id = cell_type + "_" + compartment.ToString() + "_graph_id";
                name_open_box = cell_type + "_" + compartment.ToString();
                name_closed_box = cell_type + "_" + compartment.ToString();
            }

            string spaces = Get_spaces_string();
            string text_group_start = spaces + "<node id=\"" + group_id + "\" yfiles.foldertype=\"group\">\n";
            Writer.Write(text_group_start);

            Shift_text_right += 2;
            spaces = Get_spaces_string();
            string text_group_boxes =
                spaces + "<data key=\"" + Key.Node_url + "\"/>\n"
              + spaces + "<data key=\"" + Key.Node_graphics + "\">\n"
              + spaces + "  <y:ProxyAutoBoundsNode>\n"
              + spaces + "    <y:Realizers active=\"0\">\n"
              + spaces + "      <y:GroupNode>\n"
              + spaces + "        <y:Geometry height=\"191.2364196849619\" width=\"353.5269841269842\" x=\"431.6801587301587\" y=\"101.4834899974619\"/>\n"
              + spaces + "        <y:Fill color=\"#F5F5F5\" transparent=\"false\"/>\n"
              + spaces + "        <y:BorderStyle color=\"#000000\" type=\"dashed\" width=\"1.0\"/>\n"
              + spaces + "        <y:NodeLabel alignment=\"right\" autoSizePolicy=\"node_width\" backgroundColor=\"#EBEBEB\" borderDistance=\"0.0\" fontFamily=\"Dialog\" fontSize=\"15\" fontStyle=\"plain\" hasLineColor=\"false\" height=\"22.37646484375\" modelName=\"internal\" modelPosition=\"t\" textColor=\"#000000\" visible=\"true\" width=\"353.5269841269842\" x=\"0.0\" y=\"0.0\">" + name_open_box + "</y:NodeLabel>\n"
              + spaces + "        <y:Shape type=\"roundrectangle\"/>\n"
              + spaces + "        <y:State closed=\"false\" closedHeight=\"50.0\" closedWidth=\"50.0\" innerGraphDisplayEnabled=\"false\"/>\n"
              + spaces + "        <y:Insets bottom=\"15\" bottomF=\"15.0\" left=\"15\" leftF=\"15.0\" right=\"15\" rightF=\"15.0\" top=\"15\" topF=\"15.0\"/>\n"
              + spaces + "        <y:BorderInsets bottom=\"9\" bottomF=\"8.57568359375\" left=\"112\" leftF=\"112.0976190476191\" right=\"104\" rightF=\"103.7509765625\" top=\"0\" topF=\"0.0\"/>\n"
              + spaces + "      </y:GroupNode>\n"
              + spaces + "      <y:GroupNode>\n"
              + spaces + "        <y:Geometry height=\"50.0\" width=\"50.0\" x=\"431.6801587301587\" y=\"101.4834899974619\"/>\n"
              + spaces + "        <y:Fill color=\"#F2F0D8\" transparent=\"false\"/>\n"
              + spaces + "        <y:BorderStyle color=\"#000000\" type=\"line\" width=\"1.0\"/>\n"
              + spaces + "        <y:NodeLabel alignment=\"right\" autoSizePolicy=\"node_width\" backgroundColor=\"#B7B69E\" borderDistance=\"0.0\" fontFamily=\"Dialog\" fontSize=\"15\" fontStyle=\"plain\" hasLineColor=\"false\" height=\"22.37646484375\" modelName=\"internal\" modelPosition=\"t\" textColor=\"#000000\" visible=\"true\" width=\"75.69677734375\" x=\"-12.848388671875\" y=\"0.0\">" + name_closed_box + "</y:NodeLabel>\n"
              + spaces + "        <y:Shape type=\"rectangle\"/>\n"
              + spaces + "        <y:DropShadow color=\"#D2D2D2\" offsetX=\"4\" offsetY=\"4\"/>\n"
              + spaces + "        <y:State closed=\"true\" closedHeight=\"50.0\" closedWidth=\"50.0\" innerGraphDisplayEnabled=\"false\"/>\n"
              + spaces + "        <y:Insets bottom=\"5\" bottomF=\"5.0\" left=\"5\" leftF=\"5.0\" right=\"5\" rightF=\"5.0\" top=\"5\" topF=\"5.0\"/>\n"
              + spaces + "        <y:BorderInsets bottom=\"0\" bottomF=\"0.0\" left=\"0\" leftF=\"0.0\" right=\"0\" rightF=\"0.0\" top=\"0\" topF=\"0.0\"/>\n"
              + spaces + "      </y:GroupNode>\n"
              + spaces + "    </y:Realizers>\n"
              + spaces + "</y:ProxyAutoBoundsNode>\n"
              + spaces + "</data>\n"
              + spaces + "<graph edgedefault=\"directed\" id=\"" + graph_id + ":\">";
            Writer.WriteLine(text_group_boxes);
        }

        private void Write_compartment_group_end()
        {
            Shift_text_right -= 2;
            string space = Get_spaces_string();
            string text_group_end =
                  space + "  </graph>"
                + space + "</node>\n";
            Writer.WriteLine(text_group_end);
        }
        #endregion

        #region Nodes
        private void Write_single_node(Visualization_of_nw_node_line node)
        {
            StringBuilder sb_spaces = new StringBuilder();
            string spaces = Get_spaces_string();

            StringBuilder node_label = new StringBuilder();
            string[] nodeLabel_words = node.Label.Split(' ');
            string current_nodeLabel_word;
            int indexN;
            int nodeLabel_words_length = nodeLabel_words.Length;
            node_label.AppendFormat("{0}", nodeLabel_words[0]);
            int max_signs_per_line_in_nodeName = 25;
            if (nodeLabel_words_length > 1)
            {
                int length_of_current_line = node_label.Length;
                for (int indexNW = 1; indexNW < nodeLabel_words_length; indexNW++)
                {
                    current_nodeLabel_word = nodeLabel_words[indexNW];
                    if (current_nodeLabel_word.IndexOf('@') != -1)
                    {
                        while ((indexN = current_nodeLabel_word.IndexOf('@')) != -1)
                        {
                            node_label.AppendFormat("\n{0}", (string)current_nodeLabel_word.Substring(0, indexN));
                            current_nodeLabel_word = current_nodeLabel_word.Substring(indexN + 1, current_nodeLabel_word.Length - indexN - 1);
                        }
                        length_of_current_line = current_nodeLabel_word.Length;
                    }
                    if (length_of_current_line + 1 + current_nodeLabel_word.Length <= max_signs_per_line_in_nodeName)
                    {
                        node_label.AppendFormat(" {0}", (string)current_nodeLabel_word.Clone());
                        length_of_current_line += 1 + current_nodeLabel_word.Length;
                    }
                    else
                    {
                        node_label.AppendFormat("\n{0}", (string)current_nodeLabel_word.Clone());
                        length_of_current_line = current_nodeLabel_word.Length;
                    }
                }
            }

            string text_node = "";
            if (node.Color_specifications.Length == 1)
            {
                string hexadecimal_color = Hexadecimal_color_class.Get_hexadecimal_code_for_color(node.Color_specifications[0].Fill_color);
                text_node =
                      spaces + "<node id=\"" + node.Id + "\">\n"
                    + spaces + "  <data key=\"" + Key.Node_url + "\"/>\n"
                    + spaces + "  <data key=\"" + Key.Node_graphics + "\">\n"
                    + spaces + "    <y:ShapeNode>\n"
                    + spaces + "      <y:Geometry height=\"" + node.Geometry_heigth + "\" width=\"" + node.Geometry_width + "\" x=\"94.3427734375\" y=\"208.0\"/>\n"
                    + spaces + "      <y:Fill color=\"" + hexadecimal_color + "\" transparent=\"" + node.Transparent + "\"/>\n"
                    + spaces + "      <y:BorderStyle color=\"" + node.Border_style_color + "\" type=\"line\" width=\"" + node.Border_style_width + "\"/>\n"
                    + spaces + "      <y:NodeLabel alignment=\"" + node.Label_alignement + "\" autoSizePolicy=\"content\" fontFamily=\"Dialog\" fontSize=\"" + node.FontSize + "\" fontStyle=\"" + node.FontStyle + "\" hasBackgroundColor=\"false\" hasLineColor=\"" + node.LabelHasLineColor + "\" height=\"18.701171875\" modelName=\"" + node.Model_name + "\" modelPosition=\"" + node.Model_position + "\" textColor=\"" + node.TextColor + "\" visible=\"true\" width=\"64.703125\" x=\"-17.3515625\" y=\"5.6494140625\">" + node_label.ToString() + "</y:NodeLabel>\n"
                    + spaces + "      <y:Shape type=\"" + node.Shape_type + "\"/>\n"
                    + spaces + "    </y:ShapeNode>\n"
                    + spaces + "  </data>\n"
                    + spaces + "</node>";
            }
            else
            {
                if (String.IsNullOrEmpty(node.Resource_id)) { throw new Exception(); }
                text_node =
                  spaces + "<node id=\"" + node.Id + "\">\n"
                + spaces + "  <data key=\"" + Key.Node_url + "\"/>\n"
                + spaces + "  <data key=\"" + Key.Node_graphics + "\">\n"
                + spaces + "    <y:ImageNode>\n"
                + spaces + "      <y:Geometry height=\"" + node.Geometry_heigth * 166F/150F + "\" width=\"" + node.Geometry_width * 166F/150F + "\" x=\"812.0\" y=\"187.0\"/>\n"
                + spaces + "      <y:Fill color=\"#CCCCFF\" transparent=\"" + node.Transparent + "\"/>\n"
                + spaces + "      <y:BorderStyle color=\"" + node.Border_style_color + "\" type=\"line\" width=\"" + node.Border_style_width + "\" />\n"
                + spaces + "      <y:NodeLabel alignment=\"" + node.Label_alignement + "\" autoSizePolicy=\"content\" fontFamily=\"Dialog\" fontSize=\"" + node.FontSize + "\" fontStyle=\"" + node.FontStyle + "\" hasLineColor=\"" + node.LabelHasLineColor + "\" height=\"18.701171875\" horizontalTextPosition=\"center\" iconTextGap=\"4\" modelName=\"" + node.Model_name + "\" modelPosition=\"" + node.Model_position + "\" textColor=\"" + node.TextColor + "\" verticalTextPosition=\"bottom\" visible=\"true\" width=\"41.34765625\" x=\"129.326171875\" xml:space=\"preserve\" y=\"304.0\">" + node_label.ToString() + "</y:NodeLabel>\n"
                + spaces + "      <y:Image alphaImage=\"true\" refid=\"" + node.Resource_id + "\"/>\n"
                + spaces + "    </y:ImageNode>\n"
                + spaces + "  </data>\n"
                + spaces + "</node>";
            }
            Writer.WriteLine(text_node);
        }

        private void Write_all_nodes()
        {
            int all_nodes_length = Visu_nodeEdges.VisNodes_length;
            bool consider_node_groups = Visu_nodeEdges.Options.Consider_node_groups;
            if (consider_node_groups) { Visu_nodeEdges.Order_nodes_by_cell_type_compartment_and_id(); }
            else { Visu_nodeEdges.Order_nodes_by_id(); }
            Visualization_of_nw_node_line visu_node_line;
            if (consider_node_groups)
            {
                Visu_nodeEdges.Order_nodes_by_cell_type_compartment_and_id();
                for (int indexN = 0; indexN < all_nodes_length; indexN++)
                {
                    visu_node_line = Visu_nodeEdges.VisNodes[indexN];
                    if ((indexN == 0) || (!visu_node_line.Cell_type.Equals(Visu_nodeEdges.VisNodes[indexN - 1].Cell_type)))
                    {
                        Write_cell_type_group_start_and_group_boxes(visu_node_line.Cell_type);
                    }
                    if (   (indexN == 0)
                        || (!visu_node_line.Compartment.Equals(Visu_nodeEdges.VisNodes[indexN - 1].Compartment))
                        || (!visu_node_line.Cell_type.Equals(Visu_nodeEdges.VisNodes[indexN - 1].Cell_type)))
                    {
                        Write_compartment_group_start_and_group_boxes(visu_node_line.Cell_type, visu_node_line.Compartment);
                    }
                    Write_single_node(visu_node_line);
                    if (   (indexN == all_nodes_length - 1) 
                        || (!visu_node_line.Compartment.Equals(Visu_nodeEdges.VisNodes[indexN + 1].Compartment))
                        || (!visu_node_line.Cell_type.Equals(Visu_nodeEdges.VisNodes[indexN + 1].Cell_type)))
                    {
                        Write_compartment_group_end();
                    }
                    if ((indexN == all_nodes_length - 1) || (!visu_node_line.Cell_type.Equals(Visu_nodeEdges.VisNodes[indexN + 1].Cell_type)))
                    {
                        Write_cell_type_group_end();
                    }
                }
            }
            else
            {
                Visu_nodeEdges.Order_nodes_by_id();
                for (int indexN = 0; indexN < all_nodes_length; indexN++)
                {
                    visu_node_line = Visu_nodeEdges.VisNodes[indexN];
                    Write_single_node(visu_node_line);
                }
            }
        }
        #endregion

        #region Write Resources
        private void Write_individual_resource(Visualization_of_nw_resource_line_class resource)
        {
            StringBuilder sb_spaces = new StringBuilder();
            string spaces = Get_spaces_string();

            string text_resource = "error";
            //text_resource = spaces + "<data key=\"" + Key.Resource_graphml + "\">\n"
            text_resource = spaces + "  <y:Resource id=\"" + resource.Resource_id + "\" type=\"java.awt.image.BufferedImage\" xml:space=\"preserve\">" + resource.Base64String
                                     + "</y:Resource>\n";
            Writer.WriteLine(text_resource);
        }

        private void Write_all_resources()
        {
            Visualization_of_nw_resource_line_class[] resources = Visu_nodeEdges.VisResources;
            int resources_length = resources.Length;
            Visualization_of_nw_resource_line_class resource_line;
            resources = resources.OrderBy(l => l.Resource_id).ToArray();
            string text = "<data key=\"" + Key.Resource_graphml + "\">\n"
                          + "<y:Resources>\n";
            Writer.WriteLine(text);
            Shift_text_right = Shift_text_right + 2;

            for (int indexN = 0; indexN < resources_length; indexN++)
            {
                resource_line = resources[indexN];
                Write_individual_resource(resource_line);
            }
            text = "</y:Resources>\n"
                   + "</data>";
            Shift_text_right = Shift_text_right - 2;
            Writer.WriteLine(text);
        }
        #endregion

        #region Edges
        private void Write_single_edge(Visualization_of_nw_edge_line edge)
        {
            string spaces = Get_spaces_string();
            string text_edge =
                   spaces + "<edge id=\"" + edge.Edge_id + "\" source=\"" + edge.Source_id + "\" target=\"" + edge.Target_id + "\">\n"
                 + spaces + "  <data key=\"" + Key.Edge_description + "\"/>\n"
                 + spaces + "  <data key=\"" + Key.Edge_graphics + "\">\n"
                 + spaces + "    <y:PolyLineEdge>\n"
                 + spaces + "      <y:Path sx=\"0.0\" sy=\"0.0\" tx=\"0.0\" ty=\"0.0\"/>\n"
                 + spaces + "      <y:LineStyle color=\"" + edge.Arrow_color + "\" type=\"" + edge.Arrow_type + "\" width=\"" + edge.Arrow_width + "\"/>\n"
                 + spaces + "      <y:Arrows source=\"" + edge.Arrow_source_end + "\" target=\"" + edge.Arrow_target_end + "\"/>\n"
                 + spaces + "      <y:EdgeLabel alignment=\"center\" configuration=\"AutoFlippingLabel\" distance=\"2.0\" fontFamily=\"Dialog\" fontSize=\"" +  edge.Arrow_label_font_size +"\" fontStyle=\"plain\" hasBackgroundColor=\"false\" hasLineColor=\"false\" height=\"28.501953125\" modelName=\"two_pos\" modelPosition=\"head\" preferredPlacement=\"anywhere\" ratio=\"0.5\" textColor=\"#000000\" visible=\"true\" width=\"25.123046875\" x=\"-72.580322265625\" y=\"-84.35603932425425\">"+edge.Arrow_label+"<y:PreferredPlacementDescriptor angle=\"0.0\" angleOffsetOnRightSide=\"0\" angleReference=\"absolute\" angleRotationOnRightSide=\"co\" distance=\"-1.0\" frozen=\"true\" placement=\"anywhere\" side=\"anywhere\" sideReference=\"relative_to_edge_flow\"/>"
                 + spaces + "      </y:EdgeLabel>"
                 + spaces + "      <y:BendStyle smoothed=\"false\"/>\n"
                 + spaces + "    </y:PolyLineEdge>\n"
                 + spaces + "  </data>\n"
                 + spaces + "</edge>";
            Writer.WriteLine(text_edge);
        }

        private void Write_all_edges()
        {
            int all_edges_length = Visu_nodeEdges.VisEdges_length;
            Visu_nodeEdges.Order_edges_by_id();
            Visualization_of_nw_edge_line visu_edge_line;
            for (int indexE = 0; indexE < all_edges_length; indexE++)
            {
                visu_edge_line = Visu_nodeEdges.VisEdges[indexE];
                Write_single_edge(visu_edge_line);
            }
        }
        #endregion

        #region Legend
        private void Write_all_legend_nodes()
        {
            int all_nodes_length = Visu_nodeEdges.Legend.VisNodes.Length;
            Visualization_of_nw_node_line visu_node_line;
            Visu_nodeEdges.Legend.Order_nodes_by_compartment_and_id();
            Write_compartment_group_start_and_group_boxes("", Visu_nodeEdges.Legend.Legend_label);
            for (int indexN = 0; indexN < all_nodes_length; indexN++)
            {
                visu_node_line = Visu_nodeEdges.Legend.VisNodes[indexN];
                Write_single_node(visu_node_line);
            }
            Write_compartment_group_end();
        }

        private void Write_all_legend_edges()
        {
            int all_edges_length = Visu_nodeEdges.Legend.VisEdges.Length;
            Visu_nodeEdges.Legend.Order_edges_by_id();
            Visualization_of_nw_edge_line visu_edge_line;
            for (int indexE = 0; indexE < all_edges_length; indexE++)
            {
                visu_edge_line = Visu_nodeEdges.VisEdges[indexE];
                Write_single_edge(visu_edge_line);
            }
        }

        private void Write_legend()
        {
            Write_all_legend_nodes();
            Write_all_legend_edges();
        }
        #endregion

        public void Write_yED_file(Visualization_of_nw_basis visu_nodeEdges, string file_name)
        {
            if (visu_nodeEdges.Correctness_check())
            {
                string complete_file_name = "";
                if (file_name.IndexOf(":")!=-1)
                {
                    complete_file_name = file_name + ".graphml";
                }
                else
                {
                    complete_file_name = Global_directory_class.Results_directory + "yed_networks//" + file_name + ".graphml";
                }
                string directory = Path.GetDirectoryName(complete_file_name);
                ReadWriteClass.Create_directory_if_it_does_not_exist(directory + "//");
                
                Visu_nodeEdges = visu_nodeEdges.Deep_copy();
                Writer = new StreamWriter(complete_file_name, false);

                Write_file_head();
                if (!String.IsNullOrEmpty(Visu_nodeEdges.Options.Headline)) { Write_headline(); }
                //Shift_text_right += 2;
                Write_all_nodes();
                Write_all_edges();
                Write_all_resources();
                if (Visu_nodeEdges.Options.Include_legend)
                {
                    Write_legend();
                }
                //Shift_text_right -= 2;
                Write_file_bottom_for_nodes_and_edges();
                if (Visu_nodeEdges.VisResources.Length > 0) { Write_all_resources(); }
                Write_final_file_bottom();
                Writer.Close();
            }
        }

        public void Write_yED_file(Visualization_of_nw_basis visu_nodeEdges, string directoryOrSubdirectory, string file_name)
        {
            file_name = directoryOrSubdirectory + "/" + file_name;
            Write_yED_file(visu_nodeEdges, file_name);
        }

        #region Write_file
        private string Get_spaces_string()
        {
            StringBuilder sb_spaces = new StringBuilder();
            for (int i = 0; i < Shift_text_right; i++) { sb_spaces.Append(" "); }
            return sb_spaces.ToString();
        }
        #endregion

    }
}
