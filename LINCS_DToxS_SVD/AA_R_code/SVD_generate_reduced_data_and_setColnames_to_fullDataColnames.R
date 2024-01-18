if (decomposition_method=="SVD")
{#Begin
   if (!current_task_line$AddOrAnalyze_coCulture_data)
   {#Begin - Use original algorithm
     current_weights = replicate(length(svd_data$d),0)
     current_weights[current_eigenassays] = 1
     diag_weights = diag(length(current_weights))
     diag(diag_weights) = current_weights
     current_v = svd_data$v %*% diag_weights
     Data_current = svd_data$u %*% diag(svd_data$d) %*% t(current_v)
   }#End - Use original algorithm 
   if (current_task_line$AddOrAnalyze_coCulture_data)
   {#Begin - Use projection algorithm
     current_d_inverse = replicate(length(svd_data$d),0)
     current_d_inverse[current_eigenassays] = 1/svd_data$d[current_eigenassays]
     current_v_coCulture = t(Data) %*% svd_data$u %*% diag(current_d_inverse)
     Data_current = svd_data$u %*% diag(svd_data$d) %*% t(current_v_coCulture)
   }#End - Use projection algorithm
}#END
if (decomposition_method!="SVD")
{#Begin
   stop(paste(decomposition_method," is not accepted",sep=''))
}#End


rownames(Data_current) = rownames(Data)
colnames(Data_current) = colnames(Data)

