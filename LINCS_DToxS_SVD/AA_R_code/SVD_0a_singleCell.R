unlink(".RData")
rm(list = ls());
delete_task_reports=TRUE
add_inFrontOf_progress_report_fileName = "SVD_1"
get_eigenassays_per_dataset=FALSE
source('SVD_global_parameter.R')

library(SingleCellExperiment)
library(Seurat)
library(ggplot2)
library(sctransform)
library(mclust)
library(dplyr)
library(beeswarm)
#library(clustree)
library(Matrix)
library(gridExtra)
library(reticulate)
library(harmony)
library(umap)
library(stringr)
library(doParallel)
library(scales)

source('Common_tools.R')
cores_count = 10;

####################### Specify directories - BEGIN

raw_filtered = "\\raw_feature_bc_matrix\\";

results_directory = paste(iPSCdCM_scRNAseq_data_directory,"iPSCd_CM_marker_genes//",sep='')
markerGenes_fileName = "ScRNAseq_iPSCdCM_schaniel_markerGenes.txt"
bgGenes_fileName = "ScRNAseq_iPSCdCM_schaniel_bgGenes.txt"
baseFileNames = c("MSN01-3_control",
                  "MSN02-4_control",
                  "MSN08-13_control",
                  "MSN09-4_control")

{#Begin - Define general parameter
  analysis_name = "iPSCdCM"
  minimum_cells = 3;

  cluster_count_string_input = "C"
  dimensionality_reduction = "pca"
  Replace_baseFileNames_by_baseFileNameGroups_as_conditions = TRUE
  minimum_feature_count_per_cell = 500;
  maximum_feature_count_per_cell = 999999;
  additional_vars_to_regress = c("percent.mt")
  pc_dimensions = 1:30;
  max_percentage_mitochondrial_genes = 50;
  top_considered_features = 2000;
  cluster_selected_resolution = 0.085;
  mitochondrial_label = "^MT-";
  only_upregulated_genes = TRUE;
  deg_logfc_thresholds = c(0);
  write_seurat_objects = TRUE;
  Visualize_clusterMapViolinPlot_for_DEGs = TRUE;
  calculate_DEGs = TRUE;
  calculate_DEGs_for_full_seurat_object = TRUE
  cluster_resolution = 0.085
  minium_cells_per_feature = 100

  dir.create(results_directory)
  downsample_read_counts_before_RNAassay_DEGs_calculation_sets = -1; #if <1, no downsampling
  ignore_mitochondrial_genes_for_downsampling_read_counts = FALSE
  
  dir.create(results_directory)
  results_documentation_directory = paste(results_directory,"Documentations\\",sep='')
  dir.create(results_documentation_directory)
  Generate_figures_for_visualization = TRUE
  indexTreatment_in_filename = 3
}#End - Define general parameter
  
options(future.globals.maxSize= 2000000*1024^2)
  
progress_report = "parameters predefined"
###################################################################################################################################

{#Begin - Document general parameters
  Col_names = c("Parameter","Value")
  Col_length = length(Col_names);
  Row_names = 1;
  Row_length= length(Row_names)
  parameter_base_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names, Col_names))
  parameter_base_line = as.data.frame(parameter_base_line)
  parameters = c()
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "Analysis";
  new_parameter_line$Value = analysis_name;
  parameters = new_parameter_line
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "cores_count";
  new_parameter_line$Value = cores_count;
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "Minimum feature count per cell";
  new_parameter_line$Value = minimum_feature_count_per_cell;
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "maximum_feature_count_per_cell";
  new_parameter_line$Value = maximum_feature_count_per_cell;
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "minimum_cells";
  new_parameter_line$Value = minimum_cells;
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "max_percentage_mitochondrial_genes";
  new_parameter_line$Value = max_percentage_mitochondrial_genes;
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "mitochondrial label";
  new_parameter_line$Value = mitochondrial_label;
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "top_considered_features";
  new_parameter_line$Value = top_considered_features;
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "additional_vars_to_regress";
  new_parameter_line$Value = paste(additional_vars_to_regress,sep='',collapse=", ");
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "Replace_baseFileNames_by_baseFileNameGroups_as_conditions"
  new_parameter_line$Value = paste(Replace_baseFileNames_by_baseFileNameGroups_as_conditions,sep='',collapse=", ");
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "PC dimensions";
  new_parameter_line$Value = paste("PCs",pc_dimensions[1],"-",pc_dimensions[length(pc_dimensions)],sep='');
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "dimensionality_reduction";
  new_parameter_line$Value = dimensionality_reduction
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "cluster_resolution";
  new_parameter_line$Value = cluster_resolution
  parameters = rbind(parameters,new_parameter_line)

  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "deg_logfc_thresholds"
  new_parameter_line$Value = paste(deg_logfc_thresholds,sep='',collapse=", ")
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "only upregulated genes";
  new_parameter_line$Value = only_upregulated_genes;
  parameters = rbind(parameters,new_parameter_line)
  
  new_parameter_line = parameter_base_line
  new_parameter_line$Parameter = "write_seurat_objects";
  new_parameter_line$Value = write_seurat_objects
  parameters = rbind(parameters,new_parameter_line)
  
  complete_fileName = paste(results_documentation_directory,"Parameter_table.txt",sep='')
  write.table(parameters,file=complete_fileName,quote=FALSE,sep='\t',row.names=FALSE,col.names=TRUE)
  
}#End - Document general parameters
  
{#Begin - Document sessionInfo
  sessionInfoSummary = Get_sessionInfo_summary_table()
  complete_fileName = paste(results_documentation_directory,"SessionInfo.txt",sep='')
  write.table(sessionInfoSummary,file=complete_fileName,quote=FALSE,sep='\t',row.names=FALSE,col.names=TRUE)
}#End - Document sessionInfo
  
{#Begin - Integrate and process  single cell RNAseq datasets
  current_cores_count = min(length(baseFileNames),cores_count)
  parallel_clusters = makeCluster(current_cores_count)
  
  Generate_SCTransformed_seurat_object = function(indexBF)
  {#Begin - Generate_SCTransformed_seurat_object = function(indexBF)
    baseFileName = baseFileNames[indexBF]
    directory = paste(iPSCdCM_scRNAseq_data_directory,baseFileName,raw_filtered,sep='');
    list.files(directory)
    Raw_data <- Read10X(data.dir=directory)
    colnames(Raw_data) = paste(colnames(Raw_data),indexBF)
    
    rowsums = rowSums(Raw_data)
    indexNonZeroGeneCounts = which(rowsums!=0)
    Raw_data = Raw_data[indexNonZeroGeneCounts,]
    
    seurat = CreateSeuratObject(Raw_data,min.cells=minimum_cells,project=baseFileName,min.features=0,names.field = 1)
    seurat$Condition = as.character(Idents(seurat))
    
    ###################################################################################################################################
    #Quality control: Removal of cells with too few and too many nFeatures and too high mitochondrial feature counts
    
    seurat <- subset(x = seurat, subset = nFeature_RNA >= 100)
    seurat[["percent.mt"]] <- PercentageFeatureSet(object = seurat, pattern = mitochondrial_label)
    
    if (Generate_figures_for_visualization)
    {#Begin - Generate violin plots
      vln_plots = list()
      plot_title = unique(seurat$Condition)
      
      # Visualize QC metrics as a violin plot
      vlnplot_nFeature_RNA = VlnPlot(object = seurat, features = c("nFeature_RNA"), ncol = 1) 
      vlnplot_nFeature_RNA = vlnplot_nFeature_RNA + geom_hline(yintercept=minimum_feature_count_per_cell,col="gray60",size=2)
      vlnplot_nFeature_RNA = vlnplot_nFeature_RNA + geom_hline(yintercept=maximum_feature_count_per_cell,col="gray60",size=2)
      vlnplot_nFeature_RNA = vlnplot_nFeature_RNA + ggtitle(plot_title)
      vlnplot_nCount_RNA = VlnPlot(object = seurat, features = c("nCount_RNA"), ncol = 1)
      vlnplot_percent_mt = VlnPlot(object = seurat, features = c("percent.mt"), ncol = 1)
      vlnplot_percent_mt = vlnplot_percent_mt + geom_hline(yintercept=max_percentage_mitochondrial_genes,col="gray60",size=2)
      vlnplot_percent_mt = vlnplot_percent_mt + ggtitle(plot_title)
      vln_plots[[length(vln_plots)+1]] = vlnplot_nFeature_RNA
      vln_plots[[length(vln_plots)+1]] = vlnplot_nCount_RNA
      vln_plots[[length(vln_plots)+1]] = vlnplot_percent_mt
      
      min_cols_count = 2;
      complete_png_fileName = paste(results_documentation_directory,"ViolinPlot_qualityControl_",baseFileName,".png",sep='')
      cols_count = min(min_cols_count,length(vln_plots))
      rows_count = ceiling(length(vln_plots)/cols_count)
      png(complete_png_fileName,width=800*cols_count,height=500*rows_count,res=75);
      do.call("grid.arrange",c(vln_plots,nrow=rows_count,ncol=cols_count))
      dev.off()
      vln_plots = list()
    }#End - Generate violin plots
    
    seurat <- subset(x = seurat, subset = nFeature_RNA <= maximum_feature_count_per_cell & nFeature_RNA >= minimum_feature_count_per_cell & percent.mt<=max_percentage_mitochondrial_genes)
    seurat <- NormalizeData(seurat, verbose = FALSE)
    seurat <- SCTransform(seurat, verbose=TRUE,variable.features.n = top_considered_features, vars.to.regress = c(additional_vars_to_regress))
    return (seurat)
  }#End - Generate_SCTransformed_seurat_object = function(indexBF)
  
  print("Generate parallel clusters")
  clusterEvalQ(parallel_clusters,{
    library(SingleCellExperiment)
    library(Seurat)
    library(ggplot2)
    library(sctransform)
    library(mclust)
    library(dplyr)
    library(beeswarm)
    library(clustree)
    library(Matrix)
    library(gridExtra)
    library(reticulate)
    library(harmony)
    library(umap)
    library(stringr)
  }
  )
  clusterExport(parallel_clusters, varlist=c("iPSCdCM_scRNAseq_data_directory","baseFileNames","raw_filtered",
                                             "Generate_figures_for_visualization",
                                             "results_documentation_directory",
                                             "minimum_cells","mitochondrial_label",
                                             "maximum_feature_count_per_cell",
                                             "minimum_feature_count_per_cell",
                                             "max_percentage_mitochondrial_genes",
                                             "top_considered_features",
                                             "additional_vars_to_regress"), envir=environment())
  
  print("Generate a seurat object for each scRNAseq dataset after QC, using parallel clusters")
  seurat_list = parLapply(parallel_clusters,1:length(baseFileNames), Generate_SCTransformed_seurat_object)
  
  registerDoParallel(parallel_clusters)
  registerDoSEQ()
  unregister <- function() {
    env <- foreach:::.foreachGlobals
    rm(list=ls(name=env), pos=env)
  }    
  unregister()
  stopCluster(parallel_clusters)
  rm(parallel_clusters)
  
  progress_report = "seurat_list generated"
  ####################### Process each dataset independently and attach to list - END
  ####################### Integrate all datasets - BEGIN
  print("Integrate seurat objects")
  {#Begin - Integrate seurat objects
    full_seurat_list = seurat_list
    repeat_integration = TRUE
    intial_list_length = length(seurat_list)
    while (repeat_integration)
    {#Begin
      if (length(seurat_list)==0) { repeat_integration = FALSE }
      if (length(seurat_list)>=2)
      {#Begin
        print("Select anchor features for integration")
        anchor_features = SelectIntegrationFeatures(object.list = seurat_list, nfeatures = top_considered_features)
        seurat_list = PrepSCTIntegration(object.list = seurat_list, anchor.features = anchor_features, verbose = FALSE)
        
        seurat_anchors = FindIntegrationAnchors(object.list = seurat_list, normalization.method = "SCT", anchor.features = anchor_features, verbose = FALSE)
        
        if (!is.null(seurat_anchors)) 
        {#Begin
          print("Integrate seurat objects based on anchor sets")
          integrated_seurat = IntegrateData(anchorset = seurat_anchors, normalization.method = "SCT", verbose = FALSE)
          
          DefaultAssay(integrated_seurat) = "integrated"
          repeat_integration = FALSE
        }#End
        else
        {#Begin
          print("Anchor features selection failed, remove seurat object with least cell counts")
          total_cellCounts = c()
          for (indexList in 1:length(seurat_list))
          {#Begin
            current_seurat = seurat_list[[indexList]]
            total_cellCounts = c(total_cellCounts,length(current_seurat$orig.ident))
          }#End
          
          indexMinCellCount = which(total_cellCounts==min(total_cellCounts))
          indexKeep = 1:length(total_cellCounts)
          indexKeep = indexKeep[!indexKeep %in% indexMinCellCount]
          seurat_list = seurat_list[indexKeep]
          repeat_integration = TRUE
        }#End
      }#End
      if (length(seurat_list)==1) 
      {#Begin
        integrated_seurat = seurat_list[[1]]
        repeat_integration = FALSE
      }#End
    }#End
  }#End - Integrate seurat objects
}#End - Integrate and process  single cell RNAseq datasets

if (Generate_figures_for_visualization)
{#Begin - Generate violin plots
  vln_plots = list()
  vlnplot_selectedGenes_list = list()
  for (indexseurat in 1:length(seurat_list))
  {#Begin
    seurat = seurat_list[[indexseurat]]
    
    plot_title = unique(seurat$Condition)
    
    # Visualize QC metrics as a violin plot
    feature_name = "nFeature_RNA"
    vlnplot_nFeature_RNA = VlnPlot(object = seurat, features = c(feature_name), ncol = 1) 
    vlnplot_nFeature_RNA = vlnplot_nFeature_RNA + geom_hline(yintercept=minimum_feature_count_per_cell,col="green",size=1)
    vlnplot_nFeature_RNA = vlnplot_nFeature_RNA + geom_hline(yintercept=maximum_feature_count_per_cell,col="green",size=1)
    vlnplot_nFeature_RNA = vlnplot_nFeature_RNA + ggtitle(paste(plot_title,"\n",feature_name,sep=''))
    feature_name = "nCount_RNA"
    vlnplot_nCount_RNA = VlnPlot(object = seurat, features = c(feature_name), ncol = 1)
    vlnplot_nCount_RNA = vlnplot_nCount_RNA + ggtitle(paste(plot_title,"\n",feature_name,sep=''))
    feature_name = "percent.mt"
    vlnplot_percent_mt = VlnPlot(object = seurat, features = c(feature_name), ncol = 1)
    vlnplot_percent_mt = vlnplot_percent_mt + geom_hline(yintercept=max_percentage_mitochondrial_genes,col="green",size=1)
    vlnplot_percent_mt = vlnplot_percent_mt + ggtitle(paste(plot_title,"\n",feature_name,sep=''))
    vln_plots[[length(vln_plots)+1]] = vlnplot_nFeature_RNA
    vln_plots[[length(vln_plots)+1]] = vlnplot_nCount_RNA
    vln_plots[[length(vln_plots)+1]] = vlnplot_percent_mt
  }#End
  
  min_cols_count=3
  complete_png_fileName = paste(results_documentation_directory,"ViolinPlot_qualityControl.png",sep='')
  png(complete_png_fileName,width=3000,height=9000,res=350);
  cols_count = min(min_cols_count,length(vln_plots))
  rows_count = ceiling(length(vln_plots)/cols_count)
  png(complete_png_fileName,width=800*cols_count,height=500*rows_count,res=75);
  do.call("grid.arrange",c(vln_plots,nrow=rows_count,ncol=cols_count))
  dev.off()
}#End - Generate violin plots

{#Begin - Write background genes
   background_genes = rownames(integrated_seurat@assays$RNA@counts)
   complete_backgroundGenes_fileName = paste(results_directory,bgGenes_fileName,sep='')
   write.table(background_genes, file=complete_backgroundGenes_fileName, row.names = FALSE, col.names = FALSE, quote = FALSE)
}#End - Write background genes
  
{#Begin - Subject integrated seurat object to PCA, UMAP, TSNE and FindNeighbors analysis
  print("Run PCA on integrated seurat object")
  integrated_seurat <- RunPCA(integrated_seurat, npcs = 30, verbose = FALSE)
  print("Run UMAP on integrated seurat object")
  integrated_seurat <- RunUMAP(integrated_seurat, reduction=dimensionality_reduction, dims = pc_dimensions, verbose = FALSE)
  print("Run TSNE on integrated seurat object")
  integrated_seurat <- RunTSNE(integrated_seurat, npcs = 30, reduction=dimensionality_reduction, verbose = FALSE)
}#End - Subject integrated seurat object to PCA, UMAP, TSNE and FindNeighbors analysis
    
{#Begin - FindNeighborhoods and clusters
  print("Run FindNeighbors in integrated seurat object")
  integrated_seurat <- FindNeighbors(object = integrated_seurat, dims = pc_dimensions)
  print("Run Clusters in integrated seurat object")
  cluster_resolution = 0.085
  integrated_seurat <- FindClusters(object = integrated_seurat, resolution = cluster_resolution)
  integrated_seurat$Cluster = paste("Cluster_",as.numeric(as.character(Idents(integrated_seurat))),sep='');
  cluster_count = length(table(integrated_seurat$Cluster))
  if (cluster_count!=6) { stop("cluster count does not equal 6")}
  cluster_count_string = paste(cluster_count_string_input,cluster_count,sep='')
  addToFileName = paste(cluster_count_string,sep='')
}#End - FindNeighborhoods and clusters

if (write_seurat_objects)
{#Begin - Write integrated seurat object
  print("Save integrated seurat object on hard drive")
  seurate_object_file = paste("Seurat_integrated.RData",sep='')
  seurat_object_directory = paste(results_directory,"Seurat_objects\\",sep='')
  dir.create(seurat_object_directory)
  complete_seurate_object_file = paste(seurat_object_directory,seurate_object_file,sep='')
  save(integrated_seurat,file=complete_seurate_object_file)
  #load(file=complete_seurate_object_file)
}#End - Write integrated seurat object

if (Generate_figures_for_visualization)
{#Begin - Visualize cell neighborhoods and clusters
  print("Visualize cell neighborhoods and clusters")
  neighborhood_plots = list()
  umap_neighborhood_plots = list()
  proposed_groups = c("Cluster")
  splitBy_variable = "Condition"
  max_cols_count=1
  groups = c()
  for (indexGroup in 1:length(proposed_groups))
  {#Begin
    proposed_group = proposed_groups[indexGroup]
    if (length(unlist(unique(integrated_seurat[[proposed_group]])))>1)
    {#Begin
      groups = c(groups,proposed_group)
    }#End
  }#End
  
  for (indexGroup in 1:length(groups))
  {#Begin
    group = groups[indexGroup]
    pt_size = 1.2;
    title_font_size = 8
    tsne_plot_cluster = DimPlot(object = integrated_seurat, dims=c(1,2), reduction = "tsne", pt.size=pt_size,label=FALSE,group.by = group, split.by = splitBy_variable)
    tsne_plot_cluster = tsne_plot_cluster + ggtitle(paste("TSNE - ",group," - Res: ",cluster_resolution,sep=''))
    tsne_plot_cluster = tsne_plot_cluster + theme(legend.text=element_text(size=title_font_size)) 
    neighborhood_plots[[length(neighborhood_plots)+1]] = tsne_plot_cluster
    pca_plot_cluster = DimPlot(object = integrated_seurat, dims=c(1,2), reduction = "pca", pt.size=pt_size,label=FALSE,group.by = group, split.by = splitBy_variable)
    pca_plot_cluster = pca_plot_cluster + ggtitle(paste("PCA - ",group," - Res: ",cluster_resolution,sep=''))
    pca_plot_cluster = pca_plot_cluster + theme(legend.text=element_text(size=title_font_size)) 
    neighborhood_plots[[length(neighborhood_plots)+1]] = pca_plot_cluster
    umap_plot_cluster = DimPlot(object = integrated_seurat, dims=c(1,2), reduction = "umap", pt.size=pt_size,label=FALSE,group.by = group, split.by = splitBy_variable)
    umap_plot_cluster = umap_plot_cluster + ggtitle(paste("UMAP - ",group," - Res: ",cluster_resolution,sep=''))
    umap_plot_cluster = umap_plot_cluster + theme(legend.text=element_text(size=title_font_size)) 
    neighborhood_plots[[length(neighborhood_plots)+1]] = umap_plot_cluster
    umap_neighborhood_plots[[length(umap_neighborhood_plots)+1]] = umap_plot_cluster
  }#End
  
  complete_png_fileName = paste(results_documentation_directory,"CellMaps_",addToFileName,".png",sep='')
  figures_count = length(neighborhood_plots)
  subfigures_count = length(unique(integrated_seurat[[splitBy_variable]][,1]))
  cols_count = min(max_cols_count,figures_count)
  rows_count = ceiling(figures_count/cols_count)
  png(complete_png_fileName,width=500*subfigures_count*cols_count,height=500*rows_count,res=75);
  do.call("grid.arrange",c(neighborhood_plots,nrow=rows_count,ncol=cols_count))
  dev.off()
}#End - Visualize cell neighborhoods and clusters

if (Generate_figures_for_visualization)
{#Begin - Document distribution of entities in each cluster and vice verse
  print("Document distribution of entities in each cluster and vice verse")
  clusters = unique(integrated_seurat$Cluster)
  clusters = clusters[order(clusters)]
  length_clusters = length(clusters)
  
  conditions = unique(integrated_seurat$Condition)
  
  Col_names = c("Cluster","EntityClass","Entity",
                "Infected_cell_count","SD_infected_cell_count",
                "Cell_count","SD_cell_count",
                "Percent_entity_in_cluster","Percent_cluster_in_entity",
                "SD_percent_entity_in_cluster","SD_percent_cluster_in_entity")
  Col_length = length(Col_names)
  Row_names = 1
  Row_length = length(Row_names)
  cell_distribution_over_clusters_base_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
  cell_distribution_over_clusters_base_line = as.data.frame(cell_distribution_over_clusters_base_line)
  cell_distribution_over_clusters = c()
  
  entityClasses = c("Condition")
  entityClass_entities_array = list(conditions)
  
  for (indexEntityClass in 1:length(entityClasses))
  {#Begin - indexCategory - Calculate cell counts for each cluster entity pair and percent of entity cell counts in each cluster
    entityClass = entityClasses[indexEntityClass]
    entityClass_entities = entityClass_entities_array[[indexEntityClass]]
    if (length(entityClass_entities)>1)
    {#Begin - if (length(entityClass_entities)>1)
      for (indexCluster in 1:length_clusters)
      {#Begin
        cluster = clusters[indexCluster]
        indexCurrentCluster = which(integrated_seurat$Cluster==cluster)
        for (indexEntity in 1:length(entityClass_entities))
        {#Begin
          entity = entityClass_entities[indexEntity]
          indexCurrentEntity = which(integrated_seurat[[entityClass]]==entity)
          cell_distribution_over_clusters_line = cell_distribution_over_clusters_base_line
          cell_distribution_over_clusters_line$Cluster = cluster
          cell_distribution_over_clusters_line$EntityClass = entityClass
          cell_distribution_over_clusters_line$Entity = entity
          cell_count = length(indexCurrentEntity[indexCurrentEntity %in% indexCurrentCluster])
          cell_distribution_over_clusters_line$Cell_count = cell_count
          cell_distribution_over_clusters_line$SD_cell_count = 0
          cell_distribution_over_clusters_line$Percent_entity_in_cluster = 100 * cell_count / length(indexCurrentCluster)
          cell_distribution_over_clusters_line$SD_percent_entity_in_cluster = 0
          if (length(cell_distribution_over_clusters)==0) { cell_distribution_over_clusters = cell_distribution_over_clusters_line}
          else { cell_distribution_over_clusters = rbind(cell_distribution_over_clusters,cell_distribution_over_clusters_line)}
        }#End
      }#End
    }#End - if (length(entityClass_entities)>1) 
  }#End - indexCategory - Calculate cell counts for each cluster entity pair and percent of entity cell counts in each cluster
  
  entities = unique(cell_distribution_over_clusters$Entity)
  for (indexEntity in 1:length(entities))
  {#Begin - Calculate percent of cluster cell counts in each entity
    entity = entities[indexEntity]
    indexCurrentEntity = which(cell_distribution_over_clusters$Entity==entity)
    currentEntity_cd = cell_distribution_over_clusters[indexCurrentEntity,]
    total_cell_count = sum(currentEntity_cd$Cell_count)
    currentEntity_cd_clusters = unique(currentEntity_cd$Cluster)
    for (indexCluster in 1:length(currentEntity_cd_clusters))
    {#Begin
      currentEntity_cd_cluster = currentEntity_cd_clusters[indexCluster]
      indexCurrentCluster = which(currentEntity_cd$Cluster==currentEntity_cd_cluster)
      currentEntity_cd$Percent_cluster_in_entity[indexCurrentCluster] = 100 * currentEntity_cd$Cell_count[indexCurrentCluster] / total_cell_count
      currentEntity_cd$SD_percent_cluster_in_entity[indexCurrentCluster] = 0
    }#End
    cell_distribution_over_clusters[indexCurrentEntity,] = currentEntity_cd
  }#End - Calculate percent of cluster counts in each entity
  
  indexDistConditions = which(cell_distribution_over_clusters$EntityClass=="Condition")
  condition_cell_distributions = cell_distribution_over_clusters[indexDistConditions,]
  condition_cell_distributions$Treatment = "error"
  condition_cell_distributions$Subject = "error"
  for (indexConditionCellDist in 1:length(condition_cell_distributions[,1]))
  {#Begin - Set treatments, Infection_status and subjects for each condition
    splitStrings = strsplit(condition_cell_distributions$Entity[indexConditionCellDist],"_")[[1]]
    condition_cell_distributions$Treatment[indexConditionCellDist] = splitStrings[indexTreatment_in_filename]  # Adopt to file name
    condition_cell_distributions$Subject[indexConditionCellDist] = splitStrings[1]
  }#End - Set treatments, Infection_status and subjects for each condition
  
  entities_list = list(conditions)
  collapse_over_entityClasses = c("Condition")
  
  collapsed_cell_distribution = c()
  for (indexC in 1:length(collapse_over_entityClasses))
  {#Begin - Calculate mean and sd for collapse_over_entityClass
    collapse_over_entityClass = collapse_over_entityClasses[indexC]
    entities = entities_list[[indexC]]
    if (length(entities)>1)
    {#Begin - if (length(entities)>0)
      for (indexEntity in 1:length(entities))
      {#Begin
        current_entity = entities[indexEntity]
        indexCurrentEntity = which(condition_cell_distributions[[collapse_over_entityClass]]==current_entity)
        for (indexCluster in 1:length(clusters))
        {#Begin
          current_cluster = clusters[indexCluster]
          indexCurrentCluster = which(condition_cell_distributions$Cluster==current_cluster)
          indexIntersection = indexCurrentCluster[indexCurrentCluster %in% indexCurrentEntity]
          if (length(indexIntersection)>1)
          {#Begin - if (length(indexIntersection)>1)
            cluster_cell_count = condition_cell_distributions$Cell_count[indexCurrentCluster]
            cell_distribution_over_clusters_line = cell_distribution_over_clusters_base_line
            cell_distribution_over_clusters_line$Cluster = current_cluster
            cell_distribution_over_clusters_line$EntityClass = paste(collapse_over_entityClass,"_mean_SD",sep='')
            cell_distribution_over_clusters_line$Entity = paste("Mean_",current_entity,sep='')
            cell_distribution_over_clusters_line$Cell_count = mean(condition_cell_distributions$Cell_count[indexIntersection])
            cell_distribution_over_clusters_line$SD_cell_count = sd(condition_cell_distributions$Cell_count[indexIntersection])
            cell_distribution_over_clusters_line$Percent_entity_in_cluster = mean(condition_cell_distributions$Percent_entity_in_cluster[indexIntersection])
            cell_distribution_over_clusters_line$SD_percent_entity_in_cluster = sd(condition_cell_distributions$Percent_entity_in_cluster[indexIntersection])
            cell_distribution_over_clusters_line$Percent_cluster_in_entity = mean(condition_cell_distributions$Percent_cluster_in_entity[indexIntersection])
            cell_distribution_over_clusters_line$SD_percent_cluster_in_entity = sd(condition_cell_distributions$Percent_cluster_in_entity[indexIntersection])
            if (length(collapsed_cell_distribution)==0) { collapsed_cell_distribution = cell_distribution_over_clusters_line}
            else { collapsed_cell_distribution = rbind(collapsed_cell_distribution,cell_distribution_over_clusters_line)}
          }#End - if (length(indexIntersection)>1)
        }#End
      }#End
    }#End - if (length(entities)>0)
  }#End - Calculate mean and sd for collapse_over_entities
  
  cell_distribution_over_clusters = rbind(cell_distribution_over_clusters,collapsed_cell_distribution)
  
  complete_cellDistribution_fileName = paste(results_directory,"CellDistributions_",addToFileName,".txt",sep='')
  write.table(cell_distribution_over_clusters,file=complete_cellDistribution_fileName,quote=FALSE,row.names=FALSE,sep='\t')
  
  if (Generate_figures_for_visualization)
  {#Begin - Plot entity distributions in clusters
    Distribution_plots = list()
    entityClasses = unique(cell_distribution_over_clusters$EntityClass)
    indexMean = grep("_mean_SD",entityClasses)
    indexKeep = 1:length(entityClasses)
    indexKeep = indexKeep[!indexKeep %in% indexMean]
    entityClasses = entityClasses[indexKeep]
    for (indexEntityClass in 1:length(entityClasses))
    {#Begin
      entityClass = entityClasses[indexEntityClass]
      indexCurentEntityClass = which(cell_distribution_over_clusters$EntityClass==entityClass)
      entityClass_cell_distribution = cell_distribution_over_clusters[indexCurentEntityClass,]
      entityClass_cell_distribution$X_label = paste(entityClass_cell_distribution$Cluster,"-",entityClass_cell_distribution$Entity,sep='')
      
      Distribution_plot = ggplot(data=entityClass_cell_distribution,aes(fill=Entity,y=Cell_count,x=Cluster))
      Distribution_plot = Distribution_plot + geom_bar(stat="identity")
      Distribution_plot = Distribution_plot + ggtitle(paste(entityClass))
      Distribution_plot = Distribution_plot + ylab("Cells in cluster")
      Distribution_plot = Distribution_plot + xlab("")
      if (entityClass=="Condition")
      { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=20)) }
      else { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=40))}
      Distribution_plot = Distribution_plot + theme(plot.title = element_text(size = 40, face = "bold", hjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.text = element_text(size = 25, face = "bold",angle=90))
      Distribution_plot = Distribution_plot + theme(axis.text.x = element_text(vjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.title = element_text(size = 30, face = "bold"))
      Distribution_plots[[length(Distribution_plots)+1]] = Distribution_plot
      
      Distribution_plot = ggplot(data=entityClass_cell_distribution,aes(fill=Entity,y=Percent_entity_in_cluster,x=Cluster))
      Distribution_plot = Distribution_plot + geom_bar(stat="identity")
      Distribution_plot = Distribution_plot + ggtitle(paste(entityClass))
      Distribution_plot = Distribution_plot + ylab("Cells in cluster [%]")
      Distribution_plot = Distribution_plot + xlab("")
      if (entityClass=="Condition")
      { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=20)) }
      else { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=40))}
      Distribution_plot = Distribution_plot + theme(legend.position = "none")
      Distribution_plot = Distribution_plot + theme(plot.title = element_text(size = 40, face = "bold", hjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.text = element_text(size = 25, face = "bold",angle=90))
      Distribution_plot = Distribution_plot + theme(axis.text.x = element_text(vjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.title = element_text(size = 30, face = "bold"))
      
      Distribution_plots[[length(Distribution_plots)+1]] = Distribution_plot
      
      Distribution_plot = ggplot(data=entityClass_cell_distribution,aes(fill=Entity,y=Percent_entity_in_cluster,x=X_label))
      Distribution_plot = Distribution_plot + geom_bar(stat="identity")
      if (max(entityClass_cell_distribution$SD_percent_entity_in_cluster)>0)
      { Distribution_plot = Distribution_plot + geom_errorbar(aes(ymin=Percent_entity_in_cluster-SD_percent_entity_in_cluster,ymax=Percent_entity_in_cluster+SD_percent_entity_in_cluster))}
      Distribution_plot = Distribution_plot + ggtitle(paste(entityClass))
      Distribution_plot = Distribution_plot + ylab("Cells in cluster [%]")
      Distribution_plot = Distribution_plot + xlab("")
      if (entityClass=="Condition")
      { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=20)) }
      else { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=40))}
      Distribution_plot = Distribution_plot + theme(legend.position = "none")
      Distribution_plot = Distribution_plot + theme(plot.title = element_text(size = 40, face = "bold", hjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.text = element_text(size = 25, face = "bold",angle=90))
      Distribution_plot = Distribution_plot + theme(axis.text.x = element_text(vjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.title = element_text(size = 30, face = "bold"))
      
      Distribution_plots[[length(Distribution_plots)+1]] = Distribution_plot
    }#End
    complete_png_fileName = paste(results_documentation_directory,"Distributions_of_entities_in_clusters_",addToFileName,".png",sep='')
    cols_count = 1
    rows_count = ceiling(length(Distribution_plots)/cols_count)
    png(complete_png_fileName,width=1000*cols_count,height=1000*rows_count,res=75);
    do.call("grid.arrange",c(Distribution_plots,nrow=rows_count,ncol=cols_count))
    dev.off()
  }#End - Plot entity distributions in clusters
  
  if (Generate_figures_for_visualization)
  {#Begin - Plot cluster distributions in entities
    Distribution_plots = list()
    entityClasses = unique(cell_distribution_over_clusters$EntityClass)
    for (indexEntityClass in 1:length(entityClasses))
    {#Begin
      entityClass = entityClasses[indexEntityClass]
      entityClass_label = entityClass
      entityClass_label = gsub("_mean_SD","",entityClass_label)
      indexCurentEntityClass = which(cell_distribution_over_clusters$EntityClass==entityClass)
      entityClass_cell_distribution = cell_distribution_over_clusters[indexCurentEntityClass,]
      entityClass_cell_distribution$X_label = paste(entityClass_cell_distribution$Entity,"-",entityClass_cell_distribution$Cluster,sep='')
      
      Distribution_plot = ggplot(data=entityClass_cell_distribution,aes(x=Entity,y=Cell_count,fill=Cluster))
      Distribution_plot = Distribution_plot + geom_bar(stat="identity")
      Distribution_plot = Distribution_plot + ggtitle(paste(entityClass))
      Distribution_plot = Distribution_plot + ylab(paste("Cells in ",entityClass_label,sep=''))
      Distribution_plot = Distribution_plot + xlab("")
      if (entityClass=="Condition")
      { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=20)) }
      else { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=40))}
      Distribution_plot = Distribution_plot + theme(plot.title = element_text(size = 40, face = "bold", hjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.text = element_text(size = 25, face = "bold",angle=90))
      Distribution_plot = Distribution_plot + theme(axis.text.x = element_text(vjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.title = element_text(size = 30, face = "bold"))
      Distribution_plots[[length(Distribution_plots)+1]] = Distribution_plot
      
      Distribution_plot = ggplot(data=entityClass_cell_distribution,aes(x=Entity,y=Percent_cluster_in_entity,fill=Cluster))
      Distribution_plot = Distribution_plot + geom_bar(stat="identity")
      Distribution_plot = Distribution_plot + theme(legend.position = "none")
      Distribution_plot = Distribution_plot + ggtitle(paste(entityClass))
      Distribution_plot = Distribution_plot + ylab(paste("Cells in ",entityClass_label," [%]",sep=''))
      Distribution_plot = Distribution_plot + xlab("")
      if (entityClass=="Condition")
      { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=2)) }
      else { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=40))}
      Distribution_plot = Distribution_plot + theme(plot.title = element_text(size = 40, face = "bold", hjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.text = element_text(size = 25, face = "bold", angle=90))
      Distribution_plot = Distribution_plot + theme(axis.text.x = element_text(vjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.title = element_text(size = 30, face = "bold"))
      Distribution_plots[[length(Distribution_plots)+1]] = Distribution_plot
      
      Distribution_plot = ggplot(data=entityClass_cell_distribution,aes(x=X_label,y=Percent_cluster_in_entity,fill=Cluster))
      Distribution_plot = Distribution_plot + geom_bar(stat="identity")
      if (max(entityClass_cell_distribution$SD_percent_cluster_in_entity)>0)
      { Distribution_plot = Distribution_plot + geom_errorbar(aes(ymin=Percent_cluster_in_entity-SD_percent_cluster_in_entity,ymax=Percent_cluster_in_entity+SD_percent_cluster_in_entity))}
      Distribution_plot = Distribution_plot + theme(legend.position = "none")
      Distribution_plot = Distribution_plot + ggtitle(paste(entityClass))
      Distribution_plot = Distribution_plot + ylab(paste("Cells in ",entityClass_label," [%]",sep=''))
      Distribution_plot = Distribution_plot + xlab("")
      if (entityClass=="Condition")
      { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=2)) }
      else { Distribution_plot = Distribution_plot + theme(legend.text=element_text(size=40))}
      Distribution_plot = Distribution_plot + theme(plot.title = element_text(size = 40, face = "bold", hjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.text = element_text(size = 25, face = "bold", angle=90))
      Distribution_plot = Distribution_plot + theme(axis.text.x = element_text(vjust=0.5))
      Distribution_plot = Distribution_plot + theme(axis.title = element_text(size = 30, face = "bold"))
      Distribution_plots[[length(Distribution_plots)+1]] = Distribution_plot
    }#End
    complete_png_fileName = paste(results_documentation_directory,"Distributions_of_cluster_in_entities_",addToFileName,".png",sep='')
    cols_count = 1
    rows_count = ceiling(length(Distribution_plots)/cols_count)
    png(complete_png_fileName,width=2000*cols_count,height=1000*rows_count,res=75);
    do.call("grid.arrange",c(Distribution_plots,nrow=rows_count,ncol=cols_count))
    dev.off()
  }#End - Plot cluster distributions in entities
}#End - Document distribution of entities in each cluster and vice verse

{#Begin - Calculate and write marker genes
   marker_genes = FindAllMarkers(integrated_seurat, assay="RNA",slot="data",logfc.threshold = deg_logfc_thresholds, only.pos = TRUE)
   complete_markerGenes_fileName = paste(results_directory,markerGenes_fileName,sep='')
   write.table(marker_genes,file=complete_markerGenes_fileName,row.names=FALSE,col.names=TRUE,quote = FALSE,sep='\t')
}#End - Calculate and write marker genes
