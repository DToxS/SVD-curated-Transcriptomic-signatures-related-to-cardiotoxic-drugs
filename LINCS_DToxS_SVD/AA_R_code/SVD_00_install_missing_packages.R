
used_packages = c( "ape","ART","beeswarm","circlize","ClassDiscovery"      
                  ,"colormap","colorspace","cowplot","dendextend"          
                  ,"dixonTest","doParallel","dplyr","fields","genefilter"          
                  ,"ggbeeswarm","ggplot2","ggridges","ggvenn"
                  ,"gplots","grid","gridExtra","harmony","lattice"             
                  ,"latticeExtra","Matrix","mclust","parallel","progress"            
                  ,"qlcMatrix","RColorBrewer","reticulate","rhdf5","Rtsne"               
                  ,"scales","sctransform","Seurat","SeuratDisk"         
                  ,"SingleCellExperiment","stringr","tools","umap","zellkonverter")

missing_packages = setdiff(used_packages, rownames(installed.packages()))
install.packages(missing_packages, dependencies = TRUE)

if (!require("BiocManager", quietly = TRUE))
  install.packages("BiocManager")
bioconductor_packages = c("SingleCellExperiment","zellkonverter")
indexBC=1
for (indexBC in 1:length(bioconductor_packages))
{#Begin
   bioconductor_package = bioconductor_packages[indexBC]
   if (bioconductor_package %in% missing_packages)
   { BiocManager::install(bioconductor_package) }
}#End

for (indexUsed in 39:length(used_packages))
{#Begin - Test, if all packages are installed
    library(used_packages[indexUsed],character.only = TRUE)
}#End - Test, if all packages are installed