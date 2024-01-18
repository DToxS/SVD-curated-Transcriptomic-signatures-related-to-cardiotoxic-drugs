data_col = Data[,indexColumn]
magnitude_data_col = sqrt(sum(data_col^2))
unit_data_col = data_col / magnitude_data_col

##################################################################################################
if (decomposition_method=="SVD")
{#Begin - SVD
  coefficient = svd_data$v[indexColumn,indexEigenassay] #coefficient = how much contributes an eigenarray to a sample, columns of v contain right singular vectors 
  eigenassay = svd_data$u[,indexEigenassay]
  magnitude_eigenassay = 1;#sqrt(sum(eigenassay^2)) svd_data$u %*% diag(svd_data$d) %*% t(current_v)
  unit_eigenassay = eigenassay
  current_correlation = sum(unit_data_col * unit_eigenassay)
  current_angle = acos(current_correlation/1)
  current_smallest_angle = current_angle
  if (current_smallest_angle > pi/2) { current_smallest_angle = pi - current_smallest_angle}
}#End - SVD
##################################################################################################


##################################################################################################
if (decomposition_method=="ICA")
{#Begin - SVD
  coefficient = icasso_A[indexEigenassay,indexColumn]
  eigenassay = icasso_uncentered_S[indexEigenassay,]
  magnitude_eigenassay = sqrt(sum(eigenassay^2))
  unit_eigenassay = eigenassay / magnitude_eigenassay
  current_correlation = sum(unit_data_col * unit_eigenassay)
  current_angle = acos(current_correlation/1)
  current_smallest_angle = current_angle
  if (current_smallest_angle > pi/2) { current_smallest_angle = pi - current_smallest_angle}
}#End - SVD
##################################################################################################
