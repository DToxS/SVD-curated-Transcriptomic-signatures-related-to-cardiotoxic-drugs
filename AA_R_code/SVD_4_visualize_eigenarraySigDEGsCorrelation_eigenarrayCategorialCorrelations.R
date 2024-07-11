#stopCluster(parallel_clusters)
rm(list = ls());
#######################################################################################################################
## Generate tasks - BEGIN

indexCore = -1
get_eigenassays_per_dataset = FALSE
add_inFrontOf_progress_report_fileName = "SVD_4"
delete_task_reports = TRUE
f1_score_weight=-1
source('SVD_global_parameter.R')

tasks = globally_assigned_tasks

## Generate tasks - BEGIN
#######################################################################################################################

length_tasks = length(tasks[,1])
if (length_tasks<cores_count) { cores_count = length_tasks }
tasks_per_core = length_tasks/cores_count

#core_tasks = tasks; indexCore=1;indexCoreTask=1



parallel_clusters = makeCluster(cores_count)
clusterEvalQ(parallel_clusters, {
  library("ClassDiscovery")
  library("colorspace")
  library("dendextend")
  library("ape")
  library("ART")
  library("lattice")
  library("latticeExtra")
  library("fields")
  library("colormap")
  library("beeswarm")
  library("gplots")
  library("grid")
  library("gridExtra")
  library("ggplot2")
  library("genefilter")
  library("doParallel")
}
);

for (indexCore in 1:cores_count)
{#Begin
  startIndex = min(floor((indexCore-1) * tasks_per_core+1),length_tasks);
  endIndex = min(floor(indexCore * tasks_per_core),length_tasks)
  core_tasks = tasks[startIndex:endIndex,]
  clusterCall(parallel_clusters[indexCore], function(d) {assign('core_tasks', d, pos=.GlobalEnv)}, core_tasks)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('indexCore', d, pos=.GlobalEnv)}, indexCore)
  clusterCall(parallel_clusters[indexCore], function(d) {assign('lincs_results_directory', d, pos=.GlobalEnv)}, lincs_results_directory)
}#End

cluster_generation_correct = TRUE;

combined_core_tasks <- do.call('rbind', clusterEvalQ(parallel_clusters, core_tasks))
if (length(combined_core_tasks[,1])!= length_tasks) { cluster_generation_correct = FALSE }
if (cluster_generation_correct)
{#Begin
  for (indexC in 1:length(combined_core_tasks[,1]))
  {#Begin
    if (combined_core_tasks$Dataset[indexC] != tasks$Dataset[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Correlation_method[indexC] != tasks$Correlation_method[indexC]) { cluster_generation_correct=FALSE;}
    if (combined_core_tasks$Preprocess_data[indexC] != tasks$Preprocess_data[indexC]) { cluster_generation_correct=FALSE;}
  }#End
}#End

if (!cluster_generation_correct)
{#Begin
  stopCluster(parallel_clusters) 
}#End


clusterEvalQ(parallel_clusters,
{#Begin - Parallel clusters

length_core_tasks = length(core_tasks[,1])
  
for (indexCoreTask in 1:length_core_tasks)
{#Begin - indexCoreTask
  
  current_task_line = core_tasks[indexCoreTask,]  
  f1_score_weight=-1
  
  ## Set global parameter - BEGIN
  source('SVD_coreTaskSpecific_parameter.R')
  ## Set global parameter - END
  eigenassay_correlation_statistics = read.table(file=complete_eigenassay_correlation_statistics_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
  eigenassay_correlation_sigDEGs_correlation = read.table(file=complete_eigenassay_correlation_sigDEGs_correlation_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')

  if (file.exists(complete_eigenassay_correlation_ANOVAs_fileName))
  {#Begin
     eigenassay_correlation_ANOVAs = read.table(file=complete_eigenassay_correlation_ANOVAs_fileName,header=TRUE,stringsAsFactors = FALSE, sep='\t')
     plot_anova=TRUE
  }#End

  ##########################################################################################################################
  ## Generate date from saved R-scripts - BEGIN
  source('SVD_readPrepare_degMatrix_performSVD_setRealUniqueEntites.R')
  ## Generate date from saved R-scripts - END
  ##########################################################################################################################
  ##########################################################################################################################
  ## Visualize categorical correlations - BEGIN

  reference_valueTypes = unique(eigenassay_correlation_sigDEGs_correlation$Reference_valueType)

  for (indexReference in 1:length(reference_valueTypes))
  {#Begin - indexReferenceValue
    current_reference_valueType = reference_valueTypes[indexReference]
    add_fileName_internal = paste(add_fileName,"_",current_reference_valueType,sep='');
    indexCurrent = which(eigenassay_correlation_sigDEGs_correlation$Reference_valueType==current_reference_valueType)
    referenceValueType_eigenassay_correlation_sigDEGs_correlation = eigenassay_correlation_sigDEGs_correlation[indexCurrent,]
    #indexCurrent = which(eigenassay_correlation_ANOVAs$Reference_valueType == current_reference_valueType)
    #referenceValueType_eigenassay_correlation_ANOVAs = eigenassay_correlation_ANOVAs[indexCurrent,]
    indexCurrent = which(eigenassay_correlation_statistics$Reference_valueType == current_reference_valueType)  
    referenceValueType_eigenassay_correlation_statistics = eigenassay_correlation_statistics[indexCurrent,]
    
    plot_list = list()
  
    #Pearson correlation - eigenassay ~ # Sig DEGs - BEGIN
    Plot = ggplot(data=referenceValueType_eigenassay_correlation_sigDEGs_correlation,aes(x=Eigenassay,y=Pearson_correlation))
    Plot = Plot + geom_text(aes(label=Eigenassay,size=2,fontface=2))
    Plot = Plot + ggtitle("Pearson correlation between correlations with eigenassay and # of sig. DEGs (FDR=10%)")
    plot_list[[length(plot_list)+1]] = Plot
    #Pearson correlation - eigenassay ~ # Sig DEGs - END
  
    #Spearman correlation - eigenassay ~ # Sig DEGs - BEGIN
    Plot = ggplot(data=referenceValueType_eigenassay_correlation_sigDEGs_correlation,aes(x=Eigenassay,y=Spearman_correlation))
    Plot = Plot + geom_text(aes(label=Eigenassay,size=2,fontface=2))
    Plot = Plot + ggtitle("Spearman correlation between correlations with eigenassay and # of sig. DEGs (FDR=10%)")
    plot_list[[length(plot_list)+1]] = Plot
    #Spearman correlation - eigenassay ~ # Sig DEGs - END
    
    #Kendall correlation - eigenassay ~ # Sig DEGs - BEGIN
    Plot = ggplot(data=referenceValueType_eigenassay_correlation_sigDEGs_correlation,aes(x=Eigenassay,y=Kendall_correlation))
    Plot = Plot + geom_text(aes(label=Eigenassay,size=2,fontface=2))
    Plot = Plot + ggtitle("Kendall correlation between correlations with eigenassay and # of sig. DEGs (FDR=10%)")
    plot_list[[length(plot_list)+1]] = Plot
    #Kendall correlation - eigenassay ~ # Sig DEGs - END
    
    #Eigenexpression fraction of eigenassay - BEGIN
    eigenexpression_df = cbind(1:length(eigenexpression_fraction),eigenexpression_fraction)
    colnames(eigenexpression_df) = c("Eigenarray","Eigenexpression_fraction")
    eigenexpression_df = as.data.frame(eigenexpression_df)
    eigenexpression_df$Eigenarray = factor(eigenexpression_df$Eigenarray)
    Plot = ggplot(data=eigenexpression_df,aes(x=Eigenarray,y=Eigenexpression_fraction))
    Plot = Plot + ggtitle("Relative contribution of each eigenassay")
    Plot = Plot + geom_bar(stat="identity")
    plot_list[[length(plot_list)+1]] = Plot
    #Eigenexpression fraction of eigenassay - END
  
    #Write plots - BEGIN
    complete_png_fileName = paste(statistics_enrichmentScore_directory,"Correlations_",add_fileName_internal,".png",sep='')
    length_plot_list = length(plot_list)
    cols_count = 2
    rows_count = ceiling(length_plot_list / cols_count)
    png_width = 2000*cols_count;
    png_height = 2000*rows_count;
    png_resolution=250
    png(complete_png_fileName,width=png_width,height=png_height,res=png_resolution);
    do.call("grid.arrange",c(plot_list,nrow=rows_count,ncol=cols_count))
    dev.off()
    #Write plots - END
  
  
    plot_list = list()
    correlation_method = "pearson"
    dist.function = function(x) as.dist((1-cor((x),method=correlation_method))/2)
    hclust.function = function(x) hclust(x,method="average")
    #Plot minuslog10(p) of ttest
  
    Col_names = c("Eigenassay","Eigenexpression","Eigenexpression_fraction","Cum_eigenexpression_fraction","BaseName","Entity","EntityClass","Correlation_mean","Correlation_median","Correlation_sd","Correlation_coefVar","Angle_mean","Angle_median","Angle_sd","Angle_coefVar","Point_biserial_correlation","Ttest_pvalue","Wilcoxtest_pvalue","Ttest_minusLog10pvalue","Wilcoxtest_minusLog10pvalue")
   
    Col_length = length(Col_names)
    Row_names = 1;
    Row_length = length(Row_names)
    eigenassay_angles_statistic_base_line = as.data.frame(array(NA,c(Row_length,Col_length),dimnames = list(Row_names,Col_names)))
  
    cluster_minCutoffs = c(99999999,99999999,99999999,25,25)
    clusterValues = c("Ttest_minusLog10pvalue","Wilcoxtest_minusLog10pvalue")
    clusterEntityClasses = c("Drug","Cell_line","both")
  
    for (indexV in 1:length(clusterValues))
    {#Begin
      clusterValue = clusterValues[indexV]
      cluster_minCutoff = cluster_minCutoffs[indexV]
      for (indexEC in 1:length(clusterEntityClasses))
      {#Begin
        clusterEntityClass = clusterEntityClasses[indexEC]
        if (clusterEntityClass=="both") { entityClass_correlation_statistics = referenceValueType_eigenassay_correlation_statistics }
        else
        {#Begin
           indexCurrentEntity = which(referenceValueType_eigenassay_correlation_statistics$EntityClass==clusterEntityClass)
           entityClass_correlation_statistics = referenceValueType_eigenassay_correlation_statistics[indexCurrentEntity,]
        }#End
        if (length(entityClass_correlation_statistics[,1])>2)
        {#Begin
          entityClass_correlation_statistics = entityClass_correlation_statistics[order(entityClass_correlation_statistics$Entity,entityClass_correlation_statistics$Eigenassay),]
          
          indexNotNa = which(!is.na(entityClass_correlation_statistics[[clusterValue]]))
          entityClass_correlation_statistics = entityClass_correlation_statistics[indexNotNa,]
          
          Col_names = unique(entityClass_correlation_statistics$Entity)
          Col_length = length(Col_names)
          Row_names = unique(entityClass_correlation_statistics$Eigenassay)
          Row_length = length(Row_names)
          cluster_data = array(entityClass_correlation_statistics[[clusterValue]],c(Row_length,Col_length),dimnames = list(Row_names,Col_names))
          cluster_data = as.matrix(cluster_data)
        
          indexAboveCutoff = which(cluster_data>cluster_minCutoff,arr.ind=TRUE)
          if (length(indexAboveCutoff[,1])>0){ cluster_data[indexAboveCutoff] = 0 }
          
          row_sds = rowSds(cluster_data)
          col_sds = rowSds(t(cluster_data))
          indexNonZeroRows=which(row_sds!=0)
          indexNonZeroCols=which(col_sds!=0)

          if ((length(indexNonZeroCols)>0)&(length(indexNonZeroRows)>0))
          {#Begin
            cluster_data = cluster_data[indexNonZeroRows,]
            cluster_data = cluster_data[,indexNonZeroCols]

            col_dist = dist.function(cluster_data) 
            col_hc = hclust.function(col_dist)
            col_dend = as.dendrogram(col_hc)
            col_ord = order.dendrogram(col_dend)
          
            row_dist = dist.function(t(cluster_data))
            row_hc = hclust.function(row_dist)
            row_dend = as.dendrogram(row_hc)
            row_ord = order.dendrogram(row_dend)
          
            max_cluster_data = max(cluster_data)
            min_cluster_data = min(cluster_data)
            
            if (min_cluster_data<0)
            {#Begin
              Ratio_min_max = abs(min_cluster_data)/max_cluster_data
              warm_color_count = 2000;
              cool_color_count = warm_color_count * Ratio_min_max
              cool = rainbow(cool_color_count, start=rgb2hsv(col2rgb('cyan'))[1], end=rgb2hsv(col2rgb('navy'))[1])
              warm = rainbow(warm_color_count, start=rgb2hsv(col2rgb('red'))[1], end=rgb2hsv(col2rgb('yellow'))[1])
              cols = c(rev(cool), replicate(50,'#ffffff'),rev(warm))
            }#End
            else
            {#Begin
              warm_color_count = 1000;
              warm = rainbow(warm_color_count, start=rgb2hsv(col2rgb('darkorchid'))[1], end=rgb2hsv(col2rgb('yellow'))[1])
              cols = c('#ffffff',rev(warm))
            }#End
    
            myTheme <- modifyList(custom.theme(region=cols),
                                  list(strip.background=list(col='gray'),panel.background=list(col='gray')))
            
            Plot = levelplot(cluster_data[row_ord, col_ord],par.settings=myTheme,
                             aspect = "fill",
                             scales = list(x = list(rot = 90)),
                             colorkey = list(space = "left"),
                             ylab=list(cex=.5),
                             legend = list(right = list(fun = dendrogramGrob, args = list(x = col_dend, ord = col_ord, side = "right", size = 4)),
                                           top = list(fun = dendrogramGrob, args = list(x = row_dend, side = "top", size = 3))),
                             main = paste(clusterValue," - ",clusterEntityClass,sep=''))
            plot_list[[length(plot_list)+1]] = Plot
          }#End
        }#End
      }#End
    }#End
  
    #Write plots - BEGIN
    complete_png_fileName = paste(statistics_enrichmentScore_directory,"Correlation_statisticCluster_",add_fileName_internal,".png",sep='')
    length_plot_list = length(plot_list)
    cols_count = 3
    rows_count = ceiling(length_plot_list / cols_count)
    png_width = 5000*cols_count;
    png_height = 3000*rows_count;
    png_resolution=250
    png(complete_png_fileName,width=png_width,height=png_height,res=png_resolution);
    do.call("grid.arrange",c(plot_list,nrow=rows_count,ncol=cols_count))
    dev.off()
    #Write plots - END
    
    eigenassays = unique(referenceValueType_eigenassay_correlation_statistics$Eigenassay)
    eigenassays = eigenassays[order(eigenassays)]
    Col_names = c("Eigenassay","Median_ttest_rank","Mean_ttest_rank","SD_ttest_rank","Eigenexpression_fraction")
    Col_length = length(Col_names)
    Row_names = eigenassays
    Row_length = length(Row_names)
    eigenassay_abundancies = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
    eigenassay_abundancies = as.data.frame(eigenassay_abundancies)
    
    entities = unique(referenceValueType_eigenassay_correlation_statistics$Entity)
    referenceValueType_eigenassay_correlation_statistics = referenceValueType_eigenassay_correlation_statistics[order(referenceValueType_eigenassay_correlation_statistics$Ttest_pvalue),]
    referenceValueType_eigenassay_correlation_statistics$Ttest_rank = -1
    for (indexEntity in 1:length(entities))
    {#Begin
       entity = entities[indexEntity]
       indexCurrentEntity = which(referenceValueType_eigenassay_correlation_statistics$Entity==entity)
       referenceValueType_eigenassay_correlation_statistics$Ttest_rank[indexCurrentEntity] = rank(referenceValueType_eigenassay_correlation_statistics$Ttest_pvalue[indexCurrentEntity])
    }#End
    
    
    for (indexEigen in 1:length(eigenassays))    
    {#Begin
       eigenassay = eigenassays[indexEigen]
       indexCurrentEigenassay = which(referenceValueType_eigenassay_correlation_statistics$Eigenassay==eigenassay)
       
       eigenassay_abundancies$Eigenassay[indexEigen] = eigenassay
       eigenassay_abundancies$Eigenexpression_fraction[indexEigen] = unique(referenceValueType_eigenassay_correlation_statistics$Eigenexpression_fraction[indexCurrentEigenassay])
       eigenassay_abundancies$Median_ttest_rank[indexEigen] = median(referenceValueType_eigenassay_correlation_statistics$Ttest_rank[indexCurrentEigenassay])
       eigenassay_abundancies$Mean_ttest_rank[indexEigen] = mean(referenceValueType_eigenassay_correlation_statistics$Ttest_rank[indexCurrentEigenassay])
       eigenassay_abundancies$SD_ttest_rank[indexEigen] = sd(referenceValueType_eigenassay_correlation_statistics$TTest_rank[indexCurrentEigenassay])
    }#End
    
  }#End - indexReferenceValue

  ## Visualize categorical correlations - END
  ##########################################################################################################################

}#End - indexCore task
}#End - Parallel clusters - SVD_4

)

{#Begin - Close parallel clusters
  invisible(gc())
  parallel::stopCluster(parallel_clusters)
  invisible(gc())
  rm(parallel_clusters)
}#End - Close parallel clusters
