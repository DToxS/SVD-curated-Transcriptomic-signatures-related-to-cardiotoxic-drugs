library(ggplot2)
library(gridExtra)


Generate_plots = function(plots,complete_pdf_fileName,rows_count,cols_count,empty_rows_at_bottom_count=0)
{#Begin - Generate plots function
  empty_plot = ggplot() + theme_void()
  max_plots_per_figure = cols_count*rows_count;
  if (length(plots)==0) { plots[[length(plots)+1]] = empty_plot }
  if ((empty_rows_at_bottom_count>0)&(empty_rows_at_bottom_count<rows_count))
  {#Begin
     new_plots = list()
     plots_per_page = (rows_count-empty_rows_at_bottom_count)*cols_count
     plots_count_on_current_page=0
     for (indexPlot in 1:length(plots))
     {#Begin
        new_plots[[length(new_plots)+1]] = plots[[indexPlot]]
        plots_count_on_current_page = plots_count_on_current_page + 1
        if (plots_count_on_current_page==plots_per_page)
        {#Begin
           for (indexCols in 1:cols_count)
           {#Begin
             new_plots[[length(new_plots)+1]] = empty_plot
           }#End
           plots_count_on_current_page=0
        }#End
     }#End
     plots = new_plots
  }#End
  
  
  while ((length(plots)%%max_plots_per_figure)!=0)
  {#Begin
    plots[[length(plots)+1]] = empty_plot
  }#End
  
  pdf(complete_pdf_fileName,width=8.27,height=11.69);
  
  figures_count = ceiling(length(plots)/max_plots_per_figure)
  indexF=1
  for (indexF in 1:figures_count)
  {#Begin
    startPlot = (indexF-1)*max_plots_per_figure+1
    endPlot = min(indexF*max_plots_per_figure,length(plots))
    current_plots = plots[startPlot:endPlot]
    length_plot_list = length(current_plots)
    png_width = 3000*cols_count;
    png_height = 500*rows_count;
    png_resolution=250
    do.call("grid.arrange",c(current_plots,nrow=rows_count,ncol=cols_count))
  }#End
  dev.off()
}#End - Generate plots function

Split_name_over_multiple_lines = function(name, max_nchar_per_line, max_lines)
{#Begin
  name_length = nchar(name)
  if (name_length > max_nchar_per_line)
  {#Begin
    new_lines_count = min(max_lines,ceiling(name_length / max_nchar_per_line))
    remaining_char_length = name_length
    remaining_lines_length = new_lines_count - 1
    new_name = ""
    nchar_current_line=0
    splitStrings = strsplit(name," ")[[1]]
    indexS=4
    lines_count=1
    for (indexS in 1:length(splitStrings))
    {#Begin
      splitString=splitStrings[indexS]
      if ((remaining_lines_length==0)|(nchar_current_line+1+nchar(splitString)<=(remaining_char_length/remaining_lines_length)))
      {#Begin
        if (indexS>1) 
        { 
          new_name = paste(new_name," ",splitString,sep='') 
          nchar_current_line = nchar_current_line + nchar(splitString) + 1
          remaining_char_length = remaining_char_length - nchar(splitString) - 1
        }
        else 
        { 
          new_name = splitString 
          nchar_current_line = nchar_current_line + nchar(splitString) + 1
          remaining_char_length = remaining_char_length - nchar(splitString)
        }
      }#End
      else
      {#Begin
        new_name = paste(new_name,"\n",splitString,sep='')
        nchar_current_line = nchar(splitString)
        remaining_char_length = remaining_char_length - nchar(splitString) - 1
        remaining_lines_length = remaining_lines_length - 1
      }#End
    }#End
    if (remaining_char_length!=0) { throw_an_excetption }
    if (remaining_lines_length!=0) { throw_an_excetption }
  }#End
  else { new_name = name }
  return (new_name)
}#End

Split_names_over_multiple_lines = function(names, max_nchar_per_line, max_lines)
{#Begin
  for (indexN in 1:length(names))
  {#Begin
     names[indexN] = Split_name_over_multiple_lines(names[indexN], max_nchar_per_line, max_lines)
  }#End
  return (names)
}#End

Shorten_scp_names = function(scps)
{#Begin
   scps = gsub("Transmembrane","TM",scps)
   scps = gsub("Membrane transport","TMT",scps)
   scps = gsub("small molecules","small mol.",scps)
   #scps = gsub("Energy generation and metabolism of cellular monomers","Energy & metabolism of cellular monomers")
   scps = gsub("Cardiomyocyte repolarization during action potential and hyperpolarization","CM repolarization during AP & hyperpol.",scps)
   scps = gsub("Extracellular matrix breakdown by heparanases and sulfatases","ECM breakdown by heparanases & sulfatases",scps)
   scps = gsub("Peroxisome proliferator-activated receptor alpha signaling","PPAR alpha signaling",scps)
   scps = gsub("Coagulation, fibrinolysis, complement system and blood protein dynamics","Coag., fib., compl. system and blood protein dynamics",scps)
   scps = gsub("CCN intercellular signaling protein family receptor signaling","CCN family receptor signaling",scps)
   scps = gsub("mRNA degradation, storing and translational repression by cytoplasmic processing bodies","mRNA deg., storing and translational repr. by cyt. processing bodies",scps)
   scps = gsub("Neuronal membrane repolarization during action potential and hyperpolarization","Neuronal membrane repolarization during AP and hyperpolarization",scps)
   scps = gsub("Extracellular matrix breakdown and membrane shedding by adamalysins","ECM breakdown & membrane shedding by adamalysins",scps)
   scps = gsub("Inhibition of amyloid aggregation, amyloid degradation and uptake","Amyloid degradation, uptake and aggregation inhibition",scps)
   scps = gsub("Signaling pathways that control cell proliferation and differentiation","Sig. pathways that control cell prol. and diff.",scps)
   scps = gsub("Apoptosis initator caspase cascade","Apoptosis initiator caspase cascade",scps)
   scps = gsub("Phosphatidylcholine and phosphatidylethanolamine biosynthesis via Kennedy pathway", "PtdCho and PtdEth biosynthesis via Kennedy pathway", scps)
   scps = gsub("Transmembrane water and ion transport not involved in membrane potential generatation", "TM H2O/ion transport not involved in membrane potential generation", scps)
   scps = gsub("Extracellular matrix breakdown and membrane shedding by adamalysins", "ECM breakdown and membrane shedding by adamalysins", scps)
   scps = gsub("Neuronal membrane repolarization during action potential and hyperpolarization", "Neuronal membrane repolarization and hyperpolarization", scps)
   scps = gsub("Cardiomyocyte repolarization during action potential and hyperpolarization","Cardiomyocyte repolarization and hyperpolarization", scps)
   scps = gsub("Coagulation, fibrinolysis, complement system and blood protein dynamics", "Coagulation/fibrinolysis/complement system/blood protein dynamics", scps)
   scps = gsub("Clathrin-mediated vesicle traffic from TGN to endosomal lysosomal system", "Clathrin-mediated vesicle traffic from TGN to endolysosomal system", scps)
   scps = gsub("Transmembrane ion transport involved in membrane potential generation", "TM ion transport involved in membrane potential generation", scps)
   scps = gsub("Membrane transport of small molecules and electrical properties of membranes", "TM transport of small molecules & electrical properties of membranes", scps)
   scps = gsub("Granulocyte macrophage colony-stimulating factor receptor signaling","GM-CSF receptor signaling", scps)
   scps = gsub("Granulocyte-colony stimulating factor receptor signaling","GCSF receptor signaling",scps)
   indexGCSF1 = grep("Granulocyte",scps)
   indexGCSF2 = grep("colony stimulating factor receptor signaling",scps)
   indexGCSF = indexGCSF1[indexGCSF1 %in% indexGCSF2]
   scps[indexGCSF] = "GSCF receptor signaling"
   scps = gsub("transmembrane transport","TM transport", scps)
   scps = gsub("Transmembrane transport","TM transport", scps)
   scps = gsub("Platelet−derived growth factor receptor signaling","PDGF receptor signaling", scps)
   scps = gsub("Vascular endothelial growth factor receptor signaling","VEGF receptor signaling", scps)
   scps = gsub("Platelet-derived growth factor receptor signaling","PDGF receptor signaling", scps)
   
   indexQC = grep("translational protein modification and quality control during biosynthetic",scps)
   scps[indexQC] = "PT protein modification and QC during secretory pathway"
   indexQC = grep("ost-translational protein modification in Mitochondria",scps)
   scps[indexQC] = "PT protein modification in mitochondria"
   scps = gsub("Extracellular matrix","ECM",scps)
   return (scps)
}#End

Add_drugs_celllines_f1ScoreWeights_drugTypes_to_enrichment_dataframe = function(enrichment)
{#Begin
    sampleNames = unique(enrichment$Sample_name)
    indexSN=1
    enrichment$Drug="error"
    enrichment$Outlier="error"
    enrichment$Cell_line = "error"
    enrichment$F1_score_weight="error"
    enrichment$Drug_type ="error"
    for (indexSN in 1:length(sampleNames))
    {#Begin
      sampleName = sampleNames[indexSN]
      splitStrings = strsplit(sampleName,"-")[[1]]
      indexCurrentSampleName = which(enrichment$Sample_name==sampleName)
      enrichment$Drug[indexCurrentSampleName]=splitStrings[1]
      enrichment$Outlier[indexCurrentSampleName]=splitStrings[5]
      enrichment$Cell_line[indexCurrentSampleName] =splitStrings[2]
      enrichment$F1_score_weight[indexCurrentSampleName]=splitStrings[3]
      enrichment$Drug_type[indexCurrentSampleName] =splitStrings[6]
    }#End
    return (enrichment)
}#End

Get_sessionInfo_summary_table = function()
{#Begin
  Col_names = c("Group","Library","Field","Entry")
  Col_length = length(Col_names)
  Row_names = 1
  Row_length = length(Row_names)
  sessionInfo_summary_baseLine = as.data.frame(array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names)))
  sessionInfo_summary = c();
  
  sessionInfos_fields_list = list( "R.version" = "combined"
                                   ,"platform" = "combined"
                                   ,"locale" = "combined"
                                   ,"running" = "combined"
                                   ,"RNGkind" = "combined"
                                   ,"basePkgs" = "combined"
                                   ,"otherPkgs" = c("Version","URL","Date/Publication","Built")
                                   ,"loadedOnly" = c("Version","URL","Date/Publication","Built")
                                   ,"matprod" = "combined"
                                   ,"BLAS" = "combined"
                                   ,"LAPACK" = "combined"
                                   ,"system.codepage" = "combined"
                                   ,"codepage" = "combined")
  
  r_sessionInfo = sessionInfo()
  r_sessionInfo_length = length(r_sessionInfo)
  indexSession=1
  for (indexSession in 1:r_sessionInfo_length)
  {#Begin
     current_infoName = names(r_sessionInfo)[indexSession]
     current_infos = r_sessionInfo[[current_infoName]]
     indexInfo=1
     fields_of_interest = sessionInfos_fields_list[[current_infoName]]
     if ((!is.null(fields_of_interest))&(length(fields_of_interest)>0))
     {#Begin
       for (indexInfo in 1:length(current_infos))
       {#Begin
          current_info = current_infos[indexInfo]
          if (fields_of_interest[1]=="combined")
          {#Begin
             sessionInfo_summary_line = sessionInfo_summary_baseLine
             sessionInfo_summary_line$Group = current_infoName
             if (!is.null(names(current_info)[1])){ sessionInfo_summary_line$Field = names(current_info)[1] }
             else { sessionInfo_summary_line$Field = "No given name" }
             sessionInfo_summary_line$Library = current_info[[1]]
             if (length(sessionInfo_summary_line$Library)>1) { rm(sessionInfos_fields_list) }
             if (length(sessionInfo_summary)>0) { sessionInfo_summary = rbind(sessionInfo_summary,sessionInfo_summary_line)}
             else { sessionInfo_summary = sessionInfo_summary_line }
          }#End
          else
          {#Begin
            for (indexField in 1:length(fields_of_interest))
            {#Begin
               field_of_interest = fields_of_interest[indexField]
               sessionInfo_summary_line = sessionInfo_summary_baseLine
               sessionInfo_summary_line$Group = current_infoName
               sessionInfo_summary_line$Field = field_of_interest
               sessionInfo_summary_line$Library = current_info[[1]][["Package"]]
               if (length(sessionInfo_summary_line$Library)>1) { rm(sessionInfos_fields_list) }
               if (!is.null(current_info[[1]][[field_of_interest]]))
               { sessionInfo_summary_line$Entry = current_info[[1]][[field_of_interest]] }
               if (length(sessionInfo_summary)>0) { sessionInfo_summary = rbind(sessionInfo_summary,sessionInfo_summary_line)}
               else { sessionInfo_summary = sessionInfo_summary_line }
            }#End
          }#End
       }#End
     }#End
  }#End
  sessionInfo_summary$Group = gsub("\t"," ",sessionInfo_summary$Group)
  sessionInfo_summary$Library = gsub("\t"," ",sessionInfo_summary$Library)
  sessionInfo_summary$Field = gsub("\t"," ",sessionInfo_summary$Field)
  sessionInfo_summary$Entry = gsub("\t"," ",sessionInfo_summary$Entry)
  sessionInfo_summary$Group = gsub("\r\n"," ",sessionInfo_summary$Group)
  sessionInfo_summary$Library = gsub("\r\n"," ",sessionInfo_summary$Library)
  sessionInfo_summary$Field = gsub("\r\n"," ",sessionInfo_summary$Field)
  sessionInfo_summary$Entry = gsub("\r\n"," ",sessionInfo_summary$Entry)
  sessionInfo_summary$Group = gsub("\n"," ",sessionInfo_summary$Group)
  sessionInfo_summary$Library = gsub("\n"," ",sessionInfo_summary$Library)
  sessionInfo_summary$Field = gsub("\n"," ",sessionInfo_summary$Field)
  sessionInfo_summary$Entry = gsub("\n"," ",sessionInfo_summary$Entry)
  return (sessionInfo_summary)
}#End

Add_to_dataframe_and_extend_dataframe_is_necessary = function(dataframe_toBeAddedTo,add_dataframe,number_of_added_lines_if_missing)
{#Begin
   indexesNextAvailableSpots = which(is.na(dataframe_toBeAddedTo[,1]))
   if (length(indexesNextAvailableSpots)==0)
   {#Begin
     indexesNextAvailableSpots = length(dataframe_toBeAddedTo[,1])+1
   }#End
   length_add_dataframe = length(add_dataframe[,1])
   length_dataframe_toBeAddedTo = length(dataframe_toBeAddedTo[,1])
   indexNextAvailableSpot = indexesNextAvailableSpots[1]
   while (indexNextAvailableSpot - 1 + length_add_dataframe > length_dataframe_toBeAddedTo)
   {#Begin - Extend dataframe_toBeAddedTo if necessary
      Col_names = colnames(dataframe_toBeAddedTo)
      Col_length = length(Col_names)
      Row_names = replicate(number_of_added_lines_if_missing,"")
      Row_length = length(Row_names)
      new_dataframe = array(NA,c(Row_length,Col_length),dimnames=list(Row_names,Col_names))
      dataframe_toBeAddedTo = rbind(dataframe_toBeAddedTo,new_dataframe)
      length_dataframe_toBeAddedTo = length(dataframe_toBeAddedTo[,1])
   }#End - Extend dataframe_toBeAddedTo if necessary
   dataframe_toBeAddedTo[indexNextAvailableSpot:(indexNextAvailableSpot+length_add_dataframe-1),] = add_dataframe
   return (dataframe_toBeAddedTo)
}#End