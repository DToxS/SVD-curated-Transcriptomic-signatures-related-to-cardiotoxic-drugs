  rm(list = ls(all.names=TRUE));
  gc()

  delete_task_reports=TRUE
  add_inFrontOf_progress_report_fileName = "SVD_1"
  get_eigenassays_per_dataset=FALSE
  source('SVD_global_parameter.R')
  
  {#Begin - Open libraries and document versions - BEGIN
    Col_names = c("Library","Version")
    Col_length = length(Col_names)
    Row_names = 1
    Row_length= length(Row_names)
    version_documentation_line = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
    version_documentation_line = as.data.frame(version_documentation_line)
    version_documentations = c()
    
    libraries = c("cowplot","SingleCellExperiment","scales","RColorBrewer","Seurat","SeuratDisk",
                  "harmony","gridExtra","ggplot2","ggridges","sctransform","mclust","dplyr","beeswarm","zellkonverter",
                  "clustree","Matrix","gridExtra","reticulate","umap","stringr","rhdf5","progress","doParallel","scales"
    )
    for (indexL in 1:length(libraries))
    {#Begin
      current_library = libraries[indexL]
      library(current_library,character.only=TRUE)
      new_version_documentation_line = version_documentation_line
      new_version_documentation_line$Library = current_library
      new_version_documentation_line$Version = packageVersion(current_library)
      if (length(version_documentations)==0)
      {#Begin
        version_documentations = new_version_documentation_line
      }#End
      else
      {#Begin
        version_documentations = rbind(version_documentations,new_version_documentation_line)
      }#End
    }#End
    r_sessionInfo = sessionInfo()
  }#End - Open libraries and document versions - END
  
  hard_drive = "/windows/d/";
  hard_drive = "D:/";
  cores_count = 16
  options(future.globals.maxSize= 2000000000*1024^2)
  memory.limit(size=100000000)
  
  read_heartCellAtlas=TRUE
  if (read_heartCellAtlas)
  {#Begin - Read_and_prepare_seurat_object - SN RNAseq
    directory = downloaded_datasets_directory
    addToFileName = "Litvinukova_2020_cellsAdultHumanHeart"
    results_directory = paste(directory,"Litvinukova_2020_cellsAdultHumanHeart//",sep='')
    documentation_directory = paste(directory,"Documentation\\",sep='')
    study_baseName = "Heart cell atlas"
    h5ad_object_fileName = "Litvinukova_2020_cellsAdultHumanHeart_global_raw.h5ad"
    dir.create(results_directory)
    dir.create(documentation_directory)
    complete_h5ad_object_fileName = paste(directory,h5ad_object_fileName,sep='')
    Convert(complete_h5ad_object_fileName, dest = "h5seurat", overwrite = TRUE)
    complete_seurat_object_fileName = gsub(".h5ad",".h5Seurat",complete_h5ad_object_fileName)
    seurat_object <- LoadH5Seurat(complete_seurat_object_fileName)
    
    top_considered_features = 2000
    additional_vars_to_regress = "percent.mt"
    mitochondrial_label = "^MT"
    dimensionality_reduction = "pca"
    pc_dimensions = 1:30
    
    bgGenes = rownames(seurat_object@assays$RNA@counts)

    current_fileName = paste("BgGenes_",addToFileName,".txt",sep='')
    complete_fileName = paste(results_directory,current_fileName,sep='')
    write.table(bgGenes,file=complete_fileName,quote=FALSE,row.names=FALSE,col.names=FALSE,sep='\t')

    keep_cellTypes = unique(seurat_object$cell_type)
    keep_cellTypes = keep_cellTypes[!keep_cellTypes %in% "doublets"]
    seurat_object = subset(seurat_object, subset = cell_type %in% keep_cellTypes)

    {#Begin - Calculate marker genes and write
       seurat_object = NormalizeData(seurat_object,normalization.method = "LogNormalize", scale.factor=10000)
       data_colSums = colSums(expm1(seurat_object@assays$RNA@data))
       if (round(min(data_colSums))!=round(max(data_colSums))) { rm(seurat_object) }
      
       logfc_threshold=0
       
       Idents(seurat_object) = seurat_object$cell_type
       marker_genes = FindAllMarkers(seurat_object,logfc.threshold=logfc_threshold,test.use="wilcox",slot="data",assay="RNA",only.pos=TRUE)
       current_fileName = paste("CellType_markers_",addToFileName,".txt",sep='')
       complete_fileName = paste(results_directory,current_fileName,sep='')
       write.table(marker_genes,file=complete_fileName,quote=FALSE,row.names=FALSE,col.names = TRUE,sep='\t')
    }#End - Calculate marker genes and write
    
  }#End - Read_and_prepare_seurat_object - SN RNAseq
  
