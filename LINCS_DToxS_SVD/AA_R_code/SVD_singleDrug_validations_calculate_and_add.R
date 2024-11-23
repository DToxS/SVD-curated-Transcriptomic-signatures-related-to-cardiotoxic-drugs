if (length(current_eigenassays)>=3)
{#Begin
  current_total_eigenexpression_fraction = sum(eigenexpression_fraction[current_eigenassays])

  source('SVD_generate_reduced_data_and_setColnames_to_fullDataColnames.R')
  
  dist.function = function(x) as.dist((1-cor((x),method=correlation_method))/2)
  hclust.function = function(x) hclust(x,method="average")
  
  col_dist = dist.function(Data_current) 
  col_hc = hclust.function(col_dist)
  col_dend = as.dendrogram(col_hc)
  
  clusters = partition_leaves(col_dend)
  length_clusters = length(clusters)
  clusters_with_more_than2_members = 0
  
  colnames_data = colnames(Data_current)
  if (length(unique(colnames_data==real_colnames_in_columnOrder))!=1) { stop("(length(unique(colnames_data==real_colnames_in_columnOrder))!=1)") }
  equal_names = unique(real_colnames_in_columnOrder==colnames_data)
  if ((length(equal_names)!=1)|(equal_names[1]!=TRUE)) { stop("((length(equal_names)!=1)|(equal_names[1]!=TRUE))") }
  
  indexCluster=435
  for (indexCluster in 1:length_clusters)
  {#Begin
    current_cluster_members = clusters[[indexCluster]]
    cluster_members_count = length(current_cluster_members)
    if (cluster_members_count>=2)
    {#Begin
      clusters_with_more_than2_members = clusters_with_more_than2_members + 1
      indexColnames_in_cluster = which(real_colnames_in_columnOrder %in% current_cluster_members)
      current_colnames = real_colnames_in_columnOrder[indexColnames_in_cluster]
      current_drugs = real_drugs_in_columnOrder[indexColnames_in_cluster]
      current_celllines = real_celllines_in_columnOrder[indexColnames_in_cluster]
      current_all_colNames = real_colnames_in_columnOrder
      current_entities = c()
      if (currentCoreTask_Entity %in% current_drugs)
      {#Begin
         entityClass = c("Drug")
         current_entities = current_drugs
         current_samples = current_celllines
         full_data_entities = real_drugs_in_columnOrder
      }#End
      if (currentCoreTask_Entity %in% current_celllines)
      {#Begin
        entityClass = c("Cell_line")
        current_entities = current_celllines
        current_samples = current_drugs
        full_data_entities = real_celllines_in_columnOrder
      }#End
      if ((length(unique(current_entities))>0)&(length(unique(current_entities))<cluster_members_count))#2nd condition states that there need to be  at least 2 samples from any entity in that cluster, below it says at least 2 sample of entity of interest, save time
      {#Begin
        table_full_data_entities = table(full_data_entities)
        table_current_entities = table(current_entities)
        indexCurrentTable = which(names(table_current_entities)==currentCoreTask_Entity)
        if ((table_current_entities[indexCurrentTable]>=2)|(table_full_data_entities[currentCoreTask_Entity]==1))
        {#Begin
          indexDataTable = which(names(table_full_data_entities)==currentCoreTask_Entity)
          entity_samples_count = table_full_data_entities[indexDataTable]
          entity_samples_in_cluster_count = table_current_entities[indexCurrentTable]
          precision = entity_samples_in_cluster_count / cluster_members_count
          recall = entity_samples_in_cluster_count / entity_samples_count
          
          cluster_validation_line = current_singleDrug_cluster_validation_base_line
          cluster_validation_line$IndexCluster = indexCluster
          cluster_validation_line$Eigenexpression_fraction_total = current_total_eigenexpression_fraction
          cluster_validation_line$Eigenassays_count = length(current_eigenassays)
          cluster_validation_line$CumEigenexpression_cutoff = cum_eigenexpression_fraction_cutoff
          cluster_validation_line$Eigen_entityClass = currentCoreTask_Entity
          cluster_validation_line$EntityClass = entityClass
          cluster_validation_line$Entity = currentCoreTask_Entity
          cluster_validation_line$Entity_samples_count = entity_samples_count
          cluster_validation_line$Cluster_samples_count = cluster_members_count
          cluster_validation_line$Entity_in_cluster_samples_count = entity_samples_in_cluster_count
          cluster_validation_line$Recall = recall
          cluster_validation_line$Precision = precision
          cluster_validation_line$F1_score = 2 * precision * recall / (precision + recall)
          indexCurrentEntity_in_cluster = which(current_entities==currentCoreTask_Entity)
          if (length(indexCurrentEntity_in_cluster)>=2) 
          { cluster_validation_line$Samples_in_cluster = paste(unique(current_samples[indexCurrentEntity_in_cluster]),collapse=";") }
          if (length(indexCurrentEntity_in_cluster)==1) 
          {#Begin 
            cluster_validation_line$Samples_in_cluster = current_samples[indexCurrentEntity_in_cluster[1]]
          }#End

          indexCurrentEntityColumns = which(full_data_entities==currentCoreTask_Entity)
          length_currentEntityColumns = length(indexCurrentEntityColumns)
          current_correlations = c()
          indexIndex=1
          for (indexIndex in 1:length_currentEntityColumns)
          {#Begin - Calculate cosine similarities
            current_index_col = indexCurrentEntityColumns[indexIndex]
            current_fullData_columnName = colnames(Data)[current_index_col]
            if (!grepl(paste("[.]",currentCoreTask_Entity,"[.]",sep=''),current_fullData_columnName)) { stop("currentCoreTask_Entity is missing in full data column names") }
            data_col = Data[,current_index_col]
            magnitude_data_col = sqrt(sum(data_col^2))
            unit_data_col = data_col / magnitude_data_col
            
            current_reducedData_columnName = colnames(Data)[current_index_col]
            if (!grepl(paste("[.]",currentCoreTask_Entity,"[.]",sep=''),current_reducedData_columnName)) { stop("currentCoreTask_Entity is missing in reduced data column names") }
            if (current_reducedData_columnName!=current_fullData_columnName) { stop("(current_reducedData_columnName!=current_fullData_columnName)") }
            reduced_data_col = Data_current[,current_index_col]
            magnitude_reduced_data_col = sqrt(sum(reduced_data_col^2))
            unit_reduced_data_col = reduced_data_col / magnitude_reduced_data_col
            
            current_correlation = sum(unit_data_col * unit_reduced_data_col)
            current_correlations = c(current_correlations,current_correlation)
          }#End - Calculate cosine similarities
          
          cluster_validation_line$Median_correlation_reduded_full_data = median(current_correlations)
          cluster_validation_line$Mean_correlation_reduded_full_data = mean(current_correlations)
          cluster_validation_line$Correlations_reduced_full_data = paste(current_correlations,collapse=";")
          if (length(current_correlations)>1)
          {#Begin
            cluster_validation_line$SD_correlation_reduded_full_data = sd(current_correlations)
          }#End
          if (length(current_correlations)==1)
          {#Begin
            cluster_validation_line$SD_correlation_reduded_full_data = 0
          }#End
          
          if (length(singleDrug_cluster_validations)==0) { singleDrug_cluster_validations = cluster_validation_line }
          else { singleDrug_cluster_validations = rbind(singleDrug_cluster_validations,cluster_validation_line)}
        }#End
      }#End
    }#End
  }#End
}#End 
