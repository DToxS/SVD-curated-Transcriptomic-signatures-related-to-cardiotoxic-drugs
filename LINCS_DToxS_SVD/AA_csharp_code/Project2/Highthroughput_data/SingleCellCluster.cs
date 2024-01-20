using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReadWrite;
using Common_classes;
using Statistic;
using Enrichment;

namespace Highthroughput_data
{
    class SingleCellCluster_line_class
    {
        public string Dataset_name { get; set; }
        public string Ensembl { get; set; }
        public string Cluster_name { get; set; }
        public string Gene_symbol { get; set; }
        public double P_value { get; set; }
        public double Average_log2_foldchange { get; set; }
        public double Average_expression { get; set; }
        public double Minus_log10_pvalue { get; set; }
        public float Pct_1 { get; set; }
        public float Pct_2 { get; set; }
        public double P_value_adj { get; set; }
        public float Fractional_rank { get; set; }

        #region Order
        public static SingleCellCluster_line_class[] Order_by_datasetName_clusterName(SingleCellCluster_line_class[] singleCellClusters)
        {
            //singleCellClusters = singleCellClusters.OrderBy(l => l.Dataset_name).ThenBy(l => l.Cluster_name).ToArray();

            Dictionary<string, Dictionary<string, List<SingleCellCluster_line_class>>> datasetName_clusterName_dict = new Dictionary<string, Dictionary<string, List<SingleCellCluster_line_class>>>();
            Dictionary<string, List<SingleCellCluster_line_class>> clusterName_dict = new Dictionary<string, List<SingleCellCluster_line_class>>();
            int singleCellClusters_length = singleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            for (int indexSCC = 0; indexSCC < singleCellClusters_length; indexSCC++)
            {
                singleCellCluster_line = singleCellClusters[indexSCC];
                if (!datasetName_clusterName_dict.ContainsKey(singleCellCluster_line.Dataset_name))
                {
                    datasetName_clusterName_dict.Add(singleCellCluster_line.Dataset_name, new Dictionary<string, List<SingleCellCluster_line_class>>());
                }
                if (!datasetName_clusterName_dict[singleCellCluster_line.Dataset_name].ContainsKey(singleCellCluster_line.Cluster_name))
                {
                    datasetName_clusterName_dict[singleCellCluster_line.Dataset_name].Add(singleCellCluster_line.Cluster_name, new List<SingleCellCluster_line_class>());
                }
                datasetName_clusterName_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name].Add(singleCellCluster_line);
            }
            singleCellClusters = new SingleCellCluster_line_class[0];

            List<SingleCellCluster_line_class> ordered_singleCellClusters = new List<SingleCellCluster_line_class>();
            string[] datasetNames = datasetName_clusterName_dict.Keys.ToArray();
            string datasetName;
            int datasetNames_length = datasetNames.Length;
            string[] cluster_names;
            string cluster_name;
            int cluster_names_length;
            datasetNames = datasetNames.OrderBy(l => l).ToArray();
            for (int indexDN=0; indexDN<datasetNames_length;indexDN++)
            {
                datasetName = datasetNames[indexDN];
                clusterName_dict = datasetName_clusterName_dict[datasetName];
                cluster_names = clusterName_dict.Keys.ToArray();
                cluster_names_length = cluster_names.Length;
                cluster_names = cluster_names.OrderBy(l => l).ToArray();
                for (int indexCN=0; indexCN<cluster_names_length;indexCN++)
                {
                    cluster_name = cluster_names[indexCN];
                    ordered_singleCellClusters.AddRange(clusterName_dict[cluster_name]);
                }
            }

            if (Global_class.Check_ordering)
            {
                #region Check ordering
                int ordered_singleCellClusters_length = ordered_singleCellClusters.Count;
                SingleCellCluster_line_class this_line;
                SingleCellCluster_line_class previous_line;
                if (ordered_singleCellClusters_length!=singleCellClusters_length) { throw new Exception(); }
                for (int indexOrdered=1;indexOrdered<ordered_singleCellClusters_length;indexOrdered++)
                {
                    this_line = ordered_singleCellClusters[indexOrdered];
                    previous_line = ordered_singleCellClusters[indexOrdered - 1];
                    if (this_line.Dataset_name.CompareTo(previous_line.Dataset_name) < 0) { throw new Exception(); }
                    else if (  (this_line.Dataset_name.CompareTo(previous_line.Dataset_name)==0)
                             &&(this_line.Cluster_name.CompareTo(previous_line.Cluster_name) < 0)) { throw new Exception(); }
                }
                #endregion
            }

            return ordered_singleCellClusters.ToArray();
        }
        public static SingleCellCluster_line_class[] Order_by_datasetName_clusterName_geneSymbol_pvalue(SingleCellCluster_line_class[] singleCellClusters)
        {
            //singleCellClusters = singleCellClusters.OrderBy(l => l.Dataset_name).ThenBy(l => l.Cluster_name).ThenBy(l => l.Gene_symbol).ThenBy(l => l.P_value).ToArray();
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>>> datasetName_clusterName_geneSymbol_pvalue_dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>>>();
            Dictionary<string, Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>> clusterName_geneSymbol_pvalue_dict = new Dictionary<string, Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>>();
            Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>> geneSymbol_pvalue_dict = new Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>();
            Dictionary<double, List<SingleCellCluster_line_class>> pvalue_dict = new Dictionary<double, List<SingleCellCluster_line_class>>();
            int singleCellClusters_length = singleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            for (int indexSCC = 0; indexSCC < singleCellClusters_length; indexSCC++)
            {
                singleCellCluster_line = singleCellClusters[indexSCC];
                if (!datasetName_clusterName_geneSymbol_pvalue_dict.ContainsKey(singleCellCluster_line.Dataset_name))
                {
                    datasetName_clusterName_geneSymbol_pvalue_dict.Add(singleCellCluster_line.Dataset_name, new Dictionary<string, Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>>());
                }
                if (!datasetName_clusterName_geneSymbol_pvalue_dict[singleCellCluster_line.Dataset_name].ContainsKey(singleCellCluster_line.Cluster_name))
                {
                    datasetName_clusterName_geneSymbol_pvalue_dict[singleCellCluster_line.Dataset_name].Add(singleCellCluster_line.Cluster_name, new Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>());
                }
                if (!datasetName_clusterName_geneSymbol_pvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name].ContainsKey(singleCellCluster_line.Gene_symbol))
                {
                    datasetName_clusterName_geneSymbol_pvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name].Add(singleCellCluster_line.Gene_symbol, new Dictionary<double, List<SingleCellCluster_line_class>>());
                }
                if (!datasetName_clusterName_geneSymbol_pvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name][singleCellCluster_line.Gene_symbol].ContainsKey(singleCellCluster_line.P_value))
                {
                    datasetName_clusterName_geneSymbol_pvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name][singleCellCluster_line.Gene_symbol].Add(singleCellCluster_line.P_value, new List<SingleCellCluster_line_class>());
                }
                datasetName_clusterName_geneSymbol_pvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name][singleCellCluster_line.Gene_symbol][singleCellCluster_line.P_value].Add(singleCellCluster_line);
            }
            singleCellClusters = new SingleCellCluster_line_class[0];

            List<SingleCellCluster_line_class> ordered_singleCellClusters = new List<SingleCellCluster_line_class>();
            string[] datasetNames = datasetName_clusterName_geneSymbol_pvalue_dict.Keys.ToArray();
            string datasetName;
            int datasetNames_length = datasetNames.Length;
            string[] cluster_names;
            string cluster_name;
            int cluster_names_length;
            string[] geneSymbols;
            string geneSymbol;
            int geneSymbols_length;
            double[] pvalues;
            double pvalue;
            int pvalues_length;

            datasetNames = datasetNames.OrderBy(l => l).ToArray();
            for (int indexDN = 0; indexDN < datasetNames_length; indexDN++)
            {
                datasetName = datasetNames[indexDN];
                clusterName_geneSymbol_pvalue_dict = datasetName_clusterName_geneSymbol_pvalue_dict[datasetName];
                cluster_names = clusterName_geneSymbol_pvalue_dict.Keys.ToArray();
                cluster_names_length = cluster_names.Length;
                cluster_names = cluster_names.OrderBy(l => l).ToArray();
                for (int indexCN = 0; indexCN < cluster_names_length; indexCN++)
                {
                    cluster_name = cluster_names[indexCN];
                    geneSymbol_pvalue_dict = clusterName_geneSymbol_pvalue_dict[cluster_name];
                    geneSymbols = geneSymbol_pvalue_dict.Keys.ToArray();
                    geneSymbols_length = geneSymbols.Length;
                    geneSymbols = geneSymbols.OrderBy(l => l).ToArray();
                    for (int indexGS=0; indexGS<geneSymbols_length;indexGS++)
                    {
                        geneSymbol = geneSymbols[indexGS];
                        pvalue_dict = geneSymbol_pvalue_dict[geneSymbol];
                        pvalues = pvalue_dict.Keys.ToArray();
                        pvalues_length = pvalues.Length;
                        pvalues = pvalues.OrderBy(l => l).ToArray();
                        for (int indexP=0; indexP<pvalues_length;indexP++)
                        {
                            pvalue = pvalues[indexP];
                            ordered_singleCellClusters.AddRange(pvalue_dict[pvalue]);
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                #region Check ordering
                int ordered_singleCellClusters_length = ordered_singleCellClusters.Count;
                SingleCellCluster_line_class this_line;
                SingleCellCluster_line_class previous_line;
                if (ordered_singleCellClusters_length != singleCellClusters_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_singleCellClusters_length; indexOrdered++)
                {
                    this_line = ordered_singleCellClusters[indexOrdered];
                    previous_line = ordered_singleCellClusters[indexOrdered - 1];
                    if (this_line.Dataset_name.CompareTo(previous_line.Dataset_name) < 0) { throw new Exception(); }
                    else if (   (this_line.Dataset_name.Equals(previous_line.Dataset_name))
                             && (this_line.Cluster_name.CompareTo(previous_line.Cluster_name) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset_name.Equals(previous_line.Dataset_name))
                             && (this_line.Cluster_name.Equals(previous_line.Cluster_name))
                             && (this_line.Gene_symbol.CompareTo(previous_line.Gene_symbol) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset_name.Equals(previous_line.Dataset_name))
                             && (this_line.Cluster_name.Equals(previous_line.Cluster_name))
                             && (this_line.Gene_symbol.Equals(previous_line.Gene_symbol))
                             && (this_line.P_value.CompareTo(previous_line.P_value) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_singleCellClusters.ToArray();
        }
        public static SingleCellCluster_line_class[] Order_by_datasetName_clusterName_pvalue(SingleCellCluster_line_class[] singleCellClusters)
        {
            Dictionary<string, Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>> datasetName_clusterName_pvalue_dict = new Dictionary<string, Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>>();
            Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>> clusterName_pvalue_dict = new Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>();
            Dictionary<double, List<SingleCellCluster_line_class>> pvalue_dict = new Dictionary<double, List<SingleCellCluster_line_class>>();
            int singleCellClusters_length = singleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            for (int indexSCC = 0; indexSCC < singleCellClusters_length; indexSCC++)
            {
                singleCellCluster_line = singleCellClusters[indexSCC];
                if (!datasetName_clusterName_pvalue_dict.ContainsKey(singleCellCluster_line.Dataset_name))
                {
                    datasetName_clusterName_pvalue_dict.Add(singleCellCluster_line.Dataset_name, new Dictionary<string, Dictionary<double, List<SingleCellCluster_line_class>>>());
                }
                if (!datasetName_clusterName_pvalue_dict[singleCellCluster_line.Dataset_name].ContainsKey(singleCellCluster_line.Cluster_name))
                {
                    datasetName_clusterName_pvalue_dict[singleCellCluster_line.Dataset_name].Add(singleCellCluster_line.Cluster_name, new Dictionary<double, List<SingleCellCluster_line_class>>());
                }
                if (!datasetName_clusterName_pvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name].ContainsKey(singleCellCluster_line.P_value))
                {
                    datasetName_clusterName_pvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name].Add(singleCellCluster_line.P_value, new List<SingleCellCluster_line_class>());
                }
                datasetName_clusterName_pvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name][singleCellCluster_line.P_value].Add(singleCellCluster_line);
            }
            singleCellClusters = new SingleCellCluster_line_class[0];

            List<SingleCellCluster_line_class> ordered_singleCellClusters = new List<SingleCellCluster_line_class>();
            string[] datasetNames = datasetName_clusterName_pvalue_dict.Keys.ToArray();
            string datasetName;
            int datasetNames_length = datasetNames.Length;
            string[] cluster_names;
            string cluster_name;
            int cluster_names_length;
            double[] pvalues;
            double pvalue;
            int pvalues_length;

            datasetNames = datasetNames.OrderBy(l => l).ToArray();
            for (int indexDN = 0; indexDN < datasetNames_length; indexDN++)
            {
                datasetName = datasetNames[indexDN];
                clusterName_pvalue_dict = datasetName_clusterName_pvalue_dict[datasetName];
                cluster_names = clusterName_pvalue_dict.Keys.ToArray();
                cluster_names_length = cluster_names.Length;
                cluster_names = cluster_names.OrderBy(l => l).ToArray();
                for (int indexCN = 0; indexCN < cluster_names_length; indexCN++)
                {
                    cluster_name = cluster_names[indexCN];
                    pvalue_dict = clusterName_pvalue_dict[cluster_name];
                    pvalues = pvalue_dict.Keys.ToArray();
                    pvalues_length = pvalues.Length;
                    pvalues = pvalues.OrderBy(l => l).ToArray();
                    for (int indexP = 0; indexP < pvalues_length; indexP++)
                    {
                        pvalue = pvalues[indexP];
                        ordered_singleCellClusters.AddRange(pvalue_dict[pvalue]);
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                #region Check ordering
                int ordered_singleCellClusters_length = ordered_singleCellClusters.Count;
                SingleCellCluster_line_class this_line;
                SingleCellCluster_line_class previous_line;
                if (ordered_singleCellClusters_length != singleCellClusters_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_singleCellClusters_length; indexOrdered++)
                {
                    this_line = ordered_singleCellClusters[indexOrdered];
                    previous_line = ordered_singleCellClusters[indexOrdered - 1];
                    if (this_line.Dataset_name.CompareTo(previous_line.Dataset_name) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset_name.Equals(previous_line.Dataset_name))
                             && (this_line.Cluster_name.CompareTo(previous_line.Cluster_name) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset_name.Equals(previous_line.Dataset_name))
                             && (this_line.Cluster_name.Equals(previous_line.Cluster_name))
                             && (this_line.P_value.CompareTo(previous_line.P_value) < 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_singleCellClusters.ToArray();
        }
        public static SingleCellCluster_line_class[] Order_by_datasetName_clusterName_adjPvalue_descending_absAvgLogFC(SingleCellCluster_line_class[] singleCellClusters)
        {
            //singleCellClusters = singleCellClusters.OrderBy(l => l.Dataset_name).ThenBy(l => l.Cluster_name).ThenBy(l => l.P_value).ThenByDescending(l=>l.Average_log2_foldchange).ToArray();
            Dictionary<string, Dictionary<string, Dictionary<double, Dictionary<double, List<SingleCellCluster_line_class>>>>> datasetName_clusterName_adjPvalue_absAvgLogFc_dict = new Dictionary<string, Dictionary<string, Dictionary<double, Dictionary<double, List<SingleCellCluster_line_class>>>>>();
            Dictionary<string, Dictionary<double, Dictionary<double, List<SingleCellCluster_line_class>>>> clusterName_adjPvalue_absAvgLogFc_dict = new Dictionary<string, Dictionary<double, Dictionary<double, List<SingleCellCluster_line_class>>>>();
            Dictionary<double, Dictionary<double, List<SingleCellCluster_line_class>>> adjPvalue_absAvgLogFc_dict = new Dictionary<double, Dictionary<double, List<SingleCellCluster_line_class>>>();
            Dictionary<double, List<SingleCellCluster_line_class>> absAvgLogFc_dict = new Dictionary<double, List<SingleCellCluster_line_class>>();
            int singleCellClusters_length = singleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            for (int indexSCC = 0; indexSCC < singleCellClusters_length; indexSCC++)
            {
                singleCellCluster_line = singleCellClusters[indexSCC];
                if (!datasetName_clusterName_adjPvalue_absAvgLogFc_dict.ContainsKey(singleCellCluster_line.Dataset_name))
                {
                    datasetName_clusterName_adjPvalue_absAvgLogFc_dict.Add(singleCellCluster_line.Dataset_name, new Dictionary<string, Dictionary<double, Dictionary<double, List<SingleCellCluster_line_class>>>>());
                }
                if (!datasetName_clusterName_adjPvalue_absAvgLogFc_dict[singleCellCluster_line.Dataset_name].ContainsKey(singleCellCluster_line.Cluster_name))
                {
                    datasetName_clusterName_adjPvalue_absAvgLogFc_dict[singleCellCluster_line.Dataset_name].Add(singleCellCluster_line.Cluster_name, new Dictionary<double, Dictionary<double, List<SingleCellCluster_line_class>>>());
                }
                if (!datasetName_clusterName_adjPvalue_absAvgLogFc_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name].ContainsKey(singleCellCluster_line.P_value_adj))
                {
                    datasetName_clusterName_adjPvalue_absAvgLogFc_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name].Add(singleCellCluster_line.P_value_adj, new Dictionary<double, List<SingleCellCluster_line_class>>());
                }
                if (!datasetName_clusterName_adjPvalue_absAvgLogFc_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name][singleCellCluster_line.P_value_adj].ContainsKey(Math.Abs(singleCellCluster_line.Average_log2_foldchange)))
                {
                    datasetName_clusterName_adjPvalue_absAvgLogFc_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name][singleCellCluster_line.P_value_adj].Add(Math.Abs(singleCellCluster_line.Average_log2_foldchange), new List<SingleCellCluster_line_class>());
                }
                datasetName_clusterName_adjPvalue_absAvgLogFc_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name][singleCellCluster_line.P_value_adj][singleCellCluster_line.Average_log2_foldchange].Add(singleCellCluster_line);
            }
            singleCellClusters = new SingleCellCluster_line_class[0];

            List<SingleCellCluster_line_class> ordered_singleCellClusters = new List<SingleCellCluster_line_class>();
            string[] datasetNames = datasetName_clusterName_adjPvalue_absAvgLogFc_dict.Keys.ToArray();
            string datasetName;
            int datasetNames_length = datasetNames.Length;
            string[] cluster_names;
            string cluster_name;
            int cluster_names_length;
            double[] adjPvalues;
            double adjPvalue;
            int adjPvalues_length;
            double[] absAvgLogFCs;
            double absAvgLogFC;
            int absAvgLogFCs_length;

            datasetNames = datasetNames.OrderBy(l => l).ToArray();
            for (int indexDN = 0; indexDN < datasetNames_length; indexDN++)
            {
                datasetName = datasetNames[indexDN];
                clusterName_adjPvalue_absAvgLogFc_dict = datasetName_clusterName_adjPvalue_absAvgLogFc_dict[datasetName];
                cluster_names = clusterName_adjPvalue_absAvgLogFc_dict.Keys.ToArray();
                cluster_names_length = cluster_names.Length;
                cluster_names = cluster_names.OrderBy(l => l).ToArray();
                for (int indexCN = 0; indexCN < cluster_names_length; indexCN++)
                {
                    cluster_name = cluster_names[indexCN];
                    adjPvalue_absAvgLogFc_dict = clusterName_adjPvalue_absAvgLogFc_dict[cluster_name];
                    adjPvalues = adjPvalue_absAvgLogFc_dict.Keys.ToArray();
                    adjPvalues_length = adjPvalues.Length;
                    adjPvalues = adjPvalues.OrderBy(l => l).ToArray();
                    for (int indexP = 0; indexP < adjPvalues_length; indexP++)
                    {
                        adjPvalue = adjPvalues[indexP];
                        absAvgLogFc_dict = adjPvalue_absAvgLogFc_dict[adjPvalue];
                        absAvgLogFCs = absAvgLogFc_dict.Keys.ToArray();
                        absAvgLogFCs_length = absAvgLogFCs.Length;
                        absAvgLogFCs = absAvgLogFCs.OrderByDescending(l => l).ToArray();
                        for (int indexAvg = 0; indexAvg < absAvgLogFCs_length; indexAvg++)
                        {
                            absAvgLogFC = absAvgLogFCs[indexAvg];
                            ordered_singleCellClusters.AddRange(absAvgLogFc_dict[absAvgLogFC]);
                        }
                    }
                }
            }

            if (Global_class.Check_ordering)
            {
                #region Check ordering
                int ordered_singleCellClusters_length = ordered_singleCellClusters.Count;
                SingleCellCluster_line_class this_line;
                SingleCellCluster_line_class previous_line;
                if (ordered_singleCellClusters_length != singleCellClusters_length) { throw new Exception(); }
                for (int indexOrdered = 1; indexOrdered < ordered_singleCellClusters_length; indexOrdered++)
                {
                    this_line = ordered_singleCellClusters[indexOrdered];
                    previous_line = ordered_singleCellClusters[indexOrdered - 1];
                    if (this_line.Dataset_name.CompareTo(previous_line.Dataset_name) < 0) { throw new Exception(); }
                    else if ((this_line.Dataset_name.Equals(previous_line.Dataset_name))
                             && (this_line.Cluster_name.CompareTo(previous_line.Cluster_name) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset_name.Equals(previous_line.Dataset_name))
                             && (this_line.Cluster_name.Equals(previous_line.Cluster_name))
                             && (this_line.P_value_adj.CompareTo(previous_line.P_value_adj) < 0)) { throw new Exception(); }
                    else if ((this_line.Dataset_name.Equals(previous_line.Dataset_name))
                             && (this_line.Cluster_name.Equals(previous_line.Cluster_name))
                             && (this_line.P_value_adj.Equals(previous_line.P_value_adj))
                             && (Math.Abs(this_line.Average_log2_foldchange).CompareTo(Math.Abs(previous_line.Average_log2_foldchange)) > 0)) { throw new Exception(); }
                }
                #endregion
            }
            return ordered_singleCellClusters.ToArray();
        }
        #endregion

        public SingleCellCluster_line_class Deep_copy()
        {
            SingleCellCluster_line_class copy = (SingleCellCluster_line_class)this.MemberwiseClone();
            copy.Gene_symbol = (string)this.Gene_symbol.Clone();
            return copy;
        }
    }

    class SingleCellCluster_readWriteOptions_class : ReadWriteOptions_base
    {
        public static string Get_bgGenesInUpperCase_completeFileName(string directory_or_subdirectory, string bgGenesFileName)
        {
            string complete_directory = "";
            if (directory_or_subdirectory.IndexOf(":")!=-1)
            {
                complete_directory = directory_or_subdirectory;
            }
            else
            {
                complete_directory = Global_directory_class.Experimental_data_directory + directory_or_subdirectory;
            }
            string complete_fileName = complete_directory + bgGenesFileName;
            return complete_fileName;
        }

        public SingleCellCluster_readWriteOptions_class(string directory, string fileName)
        {
            this.File = Global_directory_class.Experimental_data_directory + "SingleCellCluster/" + fileName;
            this.Key_propertyNames = new string[] { "Cluster_name", "Gene_symbol", "P_value", "Average_log2_foldchange", "Pct_1", "Pct_2", "P_value_adj" };
            this.Key_columnNames = new string[] { "Cluster_name", "Gene_symbol", "p_val", "avg_logFC", "pct.1", "pct.2", "p_val_adj" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }

    }

    class SingleCellCluster_results_readWriteOptions_class : ReadWriteOptions_base
    {
        public SingleCellCluster_results_readWriteOptions_class(string complete_directory, string fileName)
        {
            this.File = complete_directory + fileName;
            this.Key_propertyNames = new string[] { "Cluster_name", "Gene_symbol", "P_value", "Average_log2_foldchange", "Pct_1", "Pct_2", "P_value_adj","Fractional_rank" };
            this.Key_columnNames = new string[] { "Cluster_name", "Gene_symbol", "p_val", "avg_logFC", "pct.1", "pct.2", "p_val_adj", "Fractional_rank" };
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }

    }

    class SingleCellCluster_r_output_readOptions_class : ReadWriteOptions_base
    {
        public SingleCellCluster_r_output_readOptions_class(string directory, string fileName)
        {
            this.File = directory + fileName;
            this.Key_propertyNames = new string[] { "Cluster_name", "Gene_symbol", "P_value", "Average_log2_foldchange", "Pct_1", "Pct_2", "P_value_adj"};
            this.Key_columnNames = new string[] { "cluster", "gene", "p_val", "avg_log2FC", "pct.1", "pct.2", "p_val_adj" }; //RM1
            this.HeadlineDelimiters = new char[] { Global_class.Tab };
            this.LineDelimiters = new char[] { Global_class.Tab };
            this.File_has_headline = true;
            this.Report = ReadWrite_report_enum.Report_main;
        }
    }

    class SingleCellCluster_class
    {
        public SingleCellCluster_line_class[] SingleCellClusters { get; set; }
        public string[] Bg_genes_inUpperCase { get; set; }

        public SingleCellCluster_class()
        {
            this.SingleCellClusters = new SingleCellCluster_line_class[0];
            this.Bg_genes_inUpperCase = new string[0];
        }

        private void Add_to_array(SingleCellCluster_line_class[] add_singleCellClusters)
        {
            int this_length = this.SingleCellClusters.Length;
            int add_length = add_singleCellClusters.Length;
            int new_length = this_length + add_length;
            SingleCellCluster_line_class[] new_singleCellClusters = new SingleCellCluster_line_class[new_length];
            int indexNew = -1;
            for (int indexThis = 0; indexThis < this_length; indexThis++)
            {
                indexNew++;
                new_singleCellClusters[indexNew] = this.SingleCellClusters[indexThis];
            }
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                indexNew++;
                new_singleCellClusters[indexNew] = add_singleCellClusters[indexAdd];
            }
            this.SingleCellClusters = new_singleCellClusters;
        }

        public void Keep_only_significant_genes_after_pvalue_adjustment_and_write_no_data_conditions(double adj_p_value_cutoff, string noDEGs_subdirectory, string current_analysis_name)
        {
            if (adj_p_value_cutoff<0) { throw new Exception(); }
            int single_length = SingleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            SingleCellCluster_line_class noDEGs_singleCellCluster_line;
            List<SingleCellCluster_line_class> keep_singleCellCluster_list = new List<SingleCellCluster_line_class>();
            //this.SingleCellClusters = this.SingleCellClusters.OrderBy(l => l.Dataset_name).ThenBy(l => l.Cluster_name).ToArray();
            this.SingleCellClusters = SingleCellCluster_line_class.Order_by_datasetName_clusterName(this.SingleCellClusters);
            int current_kept_cluster_count = 0;
            List<SingleCellCluster_line_class> noDEGs_list = new List<SingleCellCluster_line_class>();
            for (int indexS = 0; indexS < single_length; indexS++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexS];
                if ((indexS == 0)
                    || (!singleCellCluster_line.Dataset_name.Equals(this.SingleCellClusters[indexS - 1].Dataset_name))
                    || (!singleCellCluster_line.Cluster_name.Equals(this.SingleCellClusters[indexS - 1].Cluster_name)))
                {
                    current_kept_cluster_count = 0;
                }

                if (singleCellCluster_line.P_value_adj <= adj_p_value_cutoff)
                {
                    keep_singleCellCluster_list.Add(singleCellCluster_line);
                    current_kept_cluster_count++;
                }
                if ((indexS == single_length-1)
                    || (!singleCellCluster_line.Dataset_name.Equals(this.SingleCellClusters[indexS + 1].Dataset_name))
                    || (!singleCellCluster_line.Cluster_name.Equals(this.SingleCellClusters[indexS + 1].Cluster_name)))
                {
                    if (current_kept_cluster_count ==0)
                    {
                        noDEGs_singleCellCluster_line = singleCellCluster_line.Deep_copy();
                        noDEGs_singleCellCluster_line.Average_expression = 0;
                        noDEGs_singleCellCluster_line.Average_log2_foldchange = 0;
                        noDEGs_singleCellCluster_line.Ensembl = "noDEGs";
                        noDEGs_singleCellCluster_line.Fractional_rank = -1;
                        noDEGs_singleCellCluster_line.Gene_symbol = "noDEGs";
                        noDEGs_singleCellCluster_line.Minus_log10_pvalue = 0;
                        noDEGs_singleCellCluster_line.Pct_1 = 0;
                        noDEGs_singleCellCluster_line.Pct_2 = 0;
                        noDEGs_singleCellCluster_line.P_value = 1;
                        noDEGs_singleCellCluster_line.P_value_adj = 1;
                        keep_singleCellCluster_list.Add(noDEGs_singleCellCluster_line);
                        noDEGs_list.Add(noDEGs_singleCellCluster_line);
                    }
                }
            }
            this.SingleCellClusters = keep_singleCellCluster_list.ToArray();

            SingleCellCluster_readWriteOptions_class readWriteOptions = new SingleCellCluster_readWriteOptions_class(noDEGs_subdirectory, current_analysis_name + "_noDEGsCluster.txt");
            ReadWriteClass.WriteData(noDEGs_list, readWriteOptions);
        }
        public void Keep_only_top_x_predictions_based_on_adjPvalue_and_decreasing_log2FC_per_cluster_and_datasetName_after_calculation_of_fractional_ranks_and_check_if_at_least_one_kept_line_per_condition_has_nonZeroPvalue_or_nonInfinitylogFC(int keep_top_x)
        {
            Calculate_fractinal_ranks_based_on_adjPvalues_and_decreasing_log2fc();
            int single_length = SingleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            List<SingleCellCluster_line_class> keep_singleCellCluster_list = new List<SingleCellCluster_line_class>();
            //this.SingleCellClusters = this.SingleCellClusters.OrderBy(l => l.Dataset_name).ThenBy(l => l.Cluster_name).ThenBy(l => l.P_value).ToArray();
            this.SingleCellClusters = SingleCellCluster_line_class.Order_by_datasetName_clusterName_pvalue(this.SingleCellClusters);
            int kept_lines_per_cluster = 0;
            Dictionary<string, Dictionary<string, bool>> dataset_cluster_atLeastOneSignificantNonZeroPvalue_dict = new Dictionary<string, Dictionary<string, bool>>();
            for (int indexS = 0; indexS < single_length; indexS++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexS];
                if (singleCellCluster_line.Fractional_rank <= keep_top_x)
                {
                    kept_lines_per_cluster++;
                    keep_singleCellCluster_list.Add(singleCellCluster_line);
                    if ((singleCellCluster_line.P_value > 0) || (!double.IsInfinity(singleCellCluster_line.Average_log2_foldchange)))
                    {
                        if (!dataset_cluster_atLeastOneSignificantNonZeroPvalue_dict.ContainsKey(singleCellCluster_line.Dataset_name))
                        {
                            dataset_cluster_atLeastOneSignificantNonZeroPvalue_dict.Add(singleCellCluster_line.Dataset_name, new Dictionary<string, bool>());
                        }
                        if (!dataset_cluster_atLeastOneSignificantNonZeroPvalue_dict[singleCellCluster_line.Dataset_name].ContainsKey(singleCellCluster_line.Cluster_name))
                        {
                            dataset_cluster_atLeastOneSignificantNonZeroPvalue_dict[singleCellCluster_line.Dataset_name].Add(singleCellCluster_line.Cluster_name, true);
                        }
                    }
                }
            }
            this.SingleCellClusters = keep_singleCellCluster_list.ToArray();
            single_length = this.SingleCellClusters.Length;
            for (int indexS = 0; indexS < single_length; indexS++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexS];
                if (!dataset_cluster_atLeastOneSignificantNonZeroPvalue_dict[singleCellCluster_line.Dataset_name][singleCellCluster_line.Cluster_name].Equals(true))
                {
                    throw new Exception(); //will throw an exception if not in dictionary
                }
            }
        }
        public void Calculate_fractinal_ranks_based_on_adjPvalues_and_decreasing_log2fc()
        {
            int cluster_length = this.SingleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            SingleCellCluster_line_class inner_singleCellCluster_line;
            //this.SingleCellClusters = this.SingleCellClusters.OrderBy(l => l.Dataset_name).ThenBy(l => l.Cluster_name).ThenBy(l => l.P_value).ThenByDescending(l=>l.Average_log2_foldchange).ToArray();
            this.SingleCellClusters = SingleCellCluster_line_class.Order_by_datasetName_clusterName_adjPvalue_descending_absAvgLogFC(this.SingleCellClusters);
            int current_ordinal_rank = -1;
            int firstIndexSamePvalue = -1;
            float current_fractional_rank;
            List<double> current_ordinal_ranks = new List<double>();
            float max_fractional_rank = -1;
            int cluster_count = 0;
            for (int indexCluster = 0; indexCluster < cluster_length; indexCluster++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexCluster];
                if ((indexCluster == 0)
                    || (!singleCellCluster_line.Dataset_name.Equals(this.SingleCellClusters[indexCluster - 1].Dataset_name))
                    || (!singleCellCluster_line.Cluster_name.Equals(this.SingleCellClusters[indexCluster - 1].Cluster_name)))
                {
                    current_ordinal_rank = 0;
                    cluster_count++;
                }
                if ((indexCluster == 0)
                    || (!singleCellCluster_line.Dataset_name.Equals(this.SingleCellClusters[indexCluster - 1].Dataset_name))
                    || (!singleCellCluster_line.Cluster_name.Equals(this.SingleCellClusters[indexCluster - 1].Cluster_name))
                    || (!singleCellCluster_line.P_value_adj.Equals(this.SingleCellClusters[indexCluster - 1].P_value_adj))
                    || (!singleCellCluster_line.Average_log2_foldchange.Equals(this.SingleCellClusters[indexCluster - 1].Average_log2_foldchange)))
                {
                    current_ordinal_ranks.Clear();
                    firstIndexSamePvalue = indexCluster;
                }
                current_ordinal_rank++;
                current_ordinal_ranks.Add(current_ordinal_rank);
                if ((indexCluster == cluster_length - 1)
                    || (!singleCellCluster_line.Dataset_name.Equals(this.SingleCellClusters[indexCluster + 1].Dataset_name))
                    || (!singleCellCluster_line.Cluster_name.Equals(this.SingleCellClusters[indexCluster + 1].Cluster_name))
                    || (!singleCellCluster_line.P_value_adj.Equals(this.SingleCellClusters[indexCluster + 1].P_value_adj))
                    || (!singleCellCluster_line.Average_log2_foldchange.Equals(this.SingleCellClusters[indexCluster + 1].Average_log2_foldchange)))
                {
                    current_fractional_rank = (float)Math_class.Get_average(current_ordinal_ranks.ToArray());
                    if (current_fractional_rank > max_fractional_rank) { max_fractional_rank = current_fractional_rank; }
                    for (int indexInner = firstIndexSamePvalue; indexInner <= indexCluster; indexInner++)
                    {
                        inner_singleCellCluster_line = this.SingleCellClusters[indexInner];
                        inner_singleCellCluster_line.Fractional_rank = current_fractional_rank;
                    }
                }
            }
        }



        #region Generate DE instances and bgGenes 
        public DE_class Generate_data_instance()
        {
            Fill_de_line_class add_to_data_line;
            List<Fill_de_line_class> add_to_data_list = new List<Fill_de_line_class>();
            int singleCluster_length = this.SingleCellClusters.Length;
            SingleCellCluster_line_class singleCluster_line;
            for (int indexS = 0; indexS < singleCluster_length; indexS++)
            {
                singleCluster_line = this.SingleCellClusters[indexS];
                add_to_data_line = new Fill_de_line_class();
                add_to_data_line.Symbols_for_de = new string[] { (string)singleCluster_line.Gene_symbol.Clone() };
                add_to_data_line.Entry_type_for_de = DE_entry_enum.Diffrna;
                add_to_data_line.Timepoint_for_de = Timepoint_enum.H0;
                //add_to_data_line.Names_for_de = new string[] { (string)singleCluster_line.Dataset_name.Clone(), (string)singleCluster_line.Cluster_name.Clone() };
                add_to_data_line.Names_for_de = new string[] { (string)singleCluster_line.Cluster_name.Clone() };
                add_to_data_line.Value_for_de = singleCluster_line.Average_log2_foldchange;
                add_to_data_list.Add(add_to_data_line);
            }
            //add_to_data_list = add_to_data_list.OrderBy(l => l.Symbols_for_de[0]).ThenBy(l => l.Entry_type_for_de).ThenBy(l => l.Names_for_de[0]).ThenBy(l => l.Names_for_de[1]).ToList();
            add_to_data_list = add_to_data_list.OrderBy(l => l.Symbols_for_de[0]).ThenBy(l => l.Entry_type_for_de).ThenBy(l => l.Names_for_de[0]).ToList();
            int add_count = add_to_data_list.Count;
            Fill_de_line_class add_to_line;
            for (int indexAdd = 0; indexAdd < add_count; indexAdd++)
            {
                add_to_line = add_to_data_list[indexAdd];
                if ((indexAdd != 0)
                    && (add_to_line.Symbols_for_de[0].Equals(add_to_data_list[indexAdd - 1].Symbols_for_de[0]))
                    && (add_to_line.Timepoint_for_de.Equals(add_to_data_list[indexAdd - 1].Timepoint_for_de))
                    && (add_to_line.Entry_type_for_de.Equals(add_to_data_list[indexAdd - 1].Entry_type_for_de))
                    && (add_to_line.Names_for_de[0].Equals(add_to_data_list[indexAdd - 1].Names_for_de[0]))
                    //&& (add_to_line.Names_for_de[1].Equals(add_to_data_list[indexAdd - 1].Names_for_de[1]))
                    )
                {
                    throw new Exception();
                }
            }

            DE_class data = new DE_class();
            data.Fill_with_data(add_to_data_list.ToArray());
            return data;
        }
        public string[] Get_bgGenesInUpperCase()
        {
            return Array_class.Deep_copy_string_array(this.Bg_genes_inUpperCase);
        }
        #endregion

        #region Generate
        public void Keep_only_upregulated_genes()
        {
            //this.SingleCellClusters = this.SingleCellClusters.OrderBy(l => l.Dataset_name).ThenBy(l => l.Cluster_name).ThenBy(l => l.Gene_symbol).ThenBy(l => l.P_value).ToArray();
            this.SingleCellClusters = SingleCellCluster_line_class.Order_by_datasetName_clusterName_geneSymbol_pvalue(this.SingleCellClusters);
            int single_length = this.SingleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            List<SingleCellCluster_line_class> keep = new List<SingleCellCluster_line_class>();
            for (int indexS = 0; indexS < single_length; indexS++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexS];
                if (singleCellCluster_line.Average_log2_foldchange > 0)
                {
                    keep.Add(singleCellCluster_line);
                }
            }
            this.SingleCellClusters = keep.ToArray();
        }
        private void Remove_duplicates_by_keeping_most_significant_gene()
        {
            //this.SingleCellClusters = this.SingleCellClusters.OrderBy(l => l.Dataset_name).ThenBy(l => l.Cluster_name).ThenBy(l => l.Gene_symbol).ThenBy(l=>l.P_value).ToArray();
            this.SingleCellClusters = SingleCellCluster_line_class.Order_by_datasetName_clusterName_geneSymbol_pvalue(this.SingleCellClusters);
            int single_length = this.SingleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            List<SingleCellCluster_line_class> keep = new List<SingleCellCluster_line_class>();
            for (int indexS = 0; indexS < single_length; indexS++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexS];
                if ((indexS == 0)
                    || (!singleCellCluster_line.Dataset_name.Equals(this.SingleCellClusters[indexS - 1].Dataset_name))
                    || (!singleCellCluster_line.Cluster_name.Equals(this.SingleCellClusters[indexS - 1].Cluster_name))
                    || (!singleCellCluster_line.Gene_symbol.Equals(this.SingleCellClusters[indexS - 1].Gene_symbol)))
                {
                    keep.Add(singleCellCluster_line);
                }
                else
                {
                    throw new Exception();
                }
            }
            this.SingleCellClusters = keep.ToArray();
        }
        private void Set_all_gene_symbols_to_upperCase()
        {
            int single_length = SingleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            for (int indexS = 0; indexS < single_length; indexS++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexS];
                singleCellCluster_line.Gene_symbol = singleCellCluster_line.Gene_symbol.ToUpper();
            }
        }
        private void Calculate_minusLog10Pvalue()
        {
            int single_length = SingleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            for (int indexS = 0; indexS < single_length; indexS++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexS];
                singleCellCluster_line.Minus_log10_pvalue = -Math.Log10(singleCellCluster_line.P_value);
            }
        }
        private void Replace_positive_and_negative_infinity_log2foldchanges_and_minusLog10Pvalues_by_postive_and_negative_max_double()
        {
            int single_length = SingleCellClusters.Length;
            SingleCellCluster_line_class singleCellCluster_line;
            double max_double = Double.MaxValue * 0.1;
            for (int indexS = 0; indexS < single_length; indexS++)
            {
                singleCellCluster_line = this.SingleCellClusters[indexS];
                if (singleCellCluster_line.Average_log2_foldchange > max_double)
                {
                    singleCellCluster_line.Average_log2_foldchange = max_double;
                }
                else if (singleCellCluster_line.Average_log2_foldchange < (-max_double))
                {
                    singleCellCluster_line.Average_log2_foldchange = (-max_double);
                }
                if (singleCellCluster_line.Minus_log10_pvalue > max_double)
                {
                    singleCellCluster_line.Minus_log10_pvalue = max_double;
                }
                else if (singleCellCluster_line.Minus_log10_pvalue < (-max_double))
                {
                    singleCellCluster_line.Minus_log10_pvalue = (-max_double);
                }
            }
        }
        public void Generate_by_reading_r_outputs_and_add_to_array(string completeDirectory, string sc_fileName, string bgGenes_directory, string bgGenes_fileName)
        {
            Read_and_add_cluster_name_and_add_to_array(completeDirectory, sc_fileName);
            Set_all_gene_symbols_to_upperCase();
            Read_bg_genes_in_upperCase(bgGenes_directory, bgGenes_fileName);
            Calculate_minusLog10Pvalue();
            Replace_positive_and_negative_infinity_log2foldchanges_and_minusLog10Pvalues_by_postive_and_negative_max_double();
            Remove_duplicates_by_keeping_most_significant_gene();
        }
        #endregion

        #region Read, copy
        private void Read_and_add_cluster_name_and_add_to_array(string subdirectory, string fileName)
        {
            SingleCellCluster_r_output_readOptions_class readWriteOptions = new SingleCellCluster_r_output_readOptions_class(subdirectory, fileName);
            SingleCellCluster_line_class[] add_cluster_lines = ReadWriteClass.ReadRawData_and_FillArray<SingleCellCluster_line_class>(readWriteOptions);
            int add_length = add_cluster_lines.Length;
            SingleCellCluster_line_class single_cell_cluster_line;
            string dataset_name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            for (int indexAdd = 0; indexAdd < add_length; indexAdd++)
            {
                single_cell_cluster_line = add_cluster_lines[indexAdd];
                single_cell_cluster_line.Dataset_name = (string)dataset_name.Clone();
            }
            this.SingleCellClusters = add_cluster_lines;
        }
        private void Read_bg_genes_in_upperCase(string directory, string bgGenes_file_name)
        {
            if (!String.IsNullOrEmpty(bgGenes_file_name))
            {
                string complete_bgGenesDirectory = directory + bgGenes_file_name;
                this.Bg_genes_inUpperCase = ReadWriteClass.Read_string_array(complete_bgGenesDirectory);
                this.Bg_genes_inUpperCase = this.Bg_genes_inUpperCase.Distinct().OrderBy(l => l).ToArray();
            }
            else
            {
                this.Bg_genes_inUpperCase = new string[0];
            }
        }
        private void Get_columnIndexes_from_headline_for_mssm_output(string headline, char delimiter, out int indexFeature_id, out int indexFeature_name, out int[] indexesMeanCounts, out int[] indexesLog2FoldChange, out int[] indexesPvalues, out int[] indexesAdjustedPvalues, out int[][] clusterNo_indexes)
        {
            indexFeature_name = -1;
            indexFeature_id = -1;
            string[] columnEntries = headline.Split(delimiter);
            string columnEntry;
            int clusterNo;
            int columnEntries_length = columnEntries.Length;
            List<int> indexes_mean_count_list = new List<int>();
            List<int> indexes_log2_fold_change_list = new List<int>();
            List<int> indexes_adjusted_pvalues_list = new List<int>();
            List<int> indexes_pvalues_list = new List<int>();
            List<List<int>> clusterNo_indexes_list = new List<List<int>>();
            for (int indexC=0; indexC<columnEntries_length; indexC++)
            {
                columnEntry = columnEntries[indexC];
                switch (columnEntry)
                {
                    case "Feature ID":
                    case "FeatureID":
                        if (indexFeature_id != -1) { throw new Exception(); }
                        indexFeature_id = indexC;
                        break;
                    case "Feature Name":
                    case "FeatureName":
                        if (indexFeature_name != -1) { throw new Exception(); }
                        indexFeature_name = indexC;
                        break;
                    default:
                        clusterNo = int.Parse(columnEntry.Split(' ')[1]);
                        while (clusterNo_indexes_list.Count < clusterNo + 1)
                        {
                            clusterNo_indexes_list.Add(new List<int>());
                        }
                        clusterNo_indexes_list[clusterNo].Add(indexC);
                        if (  (columnEntry.IndexOf("Mean Counts")!=-1)
                            ||(columnEntry.IndexOf("Average")!=-1))
                        {
                            indexes_mean_count_list.Add(indexC);
                        }
                        else if (  (columnEntry.IndexOf("Log2 fold change")!=-1)
                                 ||(columnEntry.IndexOf("Log2 Fold Change") != -1))
                        {
                            indexes_log2_fold_change_list.Add(indexC);
                        }
                        else if (columnEntry.IndexOf("Adjusted p value") != -1)
                        {
                            indexes_adjusted_pvalues_list.Add(indexC);
                        }
                        else if (columnEntry.IndexOf("P-Value") != -1)
                        {
                            indexes_pvalues_list.Add(indexC);
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                }
            }
            indexesMeanCounts = indexes_mean_count_list.ToArray();
            indexesLog2FoldChange = indexes_log2_fold_change_list.ToArray();
            indexesPvalues = indexes_pvalues_list.ToArray();
            indexesAdjustedPvalues = indexes_adjusted_pvalues_list.ToArray();
            int clusterNo_count = clusterNo_indexes_list.Count;
            clusterNo_indexes = new int[clusterNo_count][];
            for (int indexClusterNO = 0; indexClusterNO < clusterNo_count; indexClusterNO++)
            {
                clusterNo_indexes[indexClusterNO] = clusterNo_indexes_list[indexClusterNO].ToArray();
            }
        }
        public void Write(string directory, string fileName)
        {
            SingleCellCluster_results_readWriteOptions_class readWriteOptions = new SingleCellCluster_results_readWriteOptions_class(directory, fileName);
            ReadWriteClass.WriteData(this.SingleCellClusters,readWriteOptions);
        }
        public SingleCellCluster_class Deep_copy()
        {
            SingleCellCluster_class copy = (SingleCellCluster_class)this.MemberwiseClone();
            int data_length = this.SingleCellClusters.Length;
            copy.SingleCellClusters = new SingleCellCluster_line_class[data_length];
            for (int indexD=0; indexD<data_length;indexD++)
            {
                copy.SingleCellClusters[indexD] = this.SingleCellClusters[indexD].Deep_copy();
            }
            copy.Bg_genes_inUpperCase = Array_class.Deep_copy_string_array(this.Bg_genes_inUpperCase);
            return copy;
        }
        #endregion

    }
}
