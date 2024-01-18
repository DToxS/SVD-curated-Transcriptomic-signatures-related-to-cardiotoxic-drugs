using System.Collections.Generic;
using System.Linq;
using ReadWrite;
using Highthroughput_data;
using Common_classes;

namespace Ontologies_and_GTEx
{
    class GTEx_line_class
    {
        public string EnsemblGene { get; set; }
        public string Description { get; set; }
        public float Adipose_subcutaneous_rpkm { get; set; }
        public float Adipose_visceral_omentum_rpkm { get; set; }
        public float Adrenal_gland_rpkm { get; set; }
        public float Artery_aorta_rpkm { get; set; }
        public float Artery_coronary_rpkm { get; set; }
        public float Artery_tibial_rpkm { get; set; }
        public float Bladder_rpkm { get; set; }
        public float Brain_amygdala_rpkm { get; set; }
        public float Brain_anterior_cingulate_cortex_BA24_rpkm { get; set; }
        public float Brain_caudate_basal_ganglia_rpkm { get; set; }
        public float Brain_cerebellar_hemisphere_rpkm { get; set; }
        public float Brain_cerebellum_rpkm { get; set; }
        public float Brain_cortex_rpkm { get; set; }
        public float Brain_frontal_cortex_BA9_rpkm { get; set; }
        public float Brain_hippocampus_rpkm { get; set; }
        public float Brain_hypothalamus_rpkm { get; set; }
        public float Brain_nucleus_accumbens_basal_ganglia_rpkm { get; set; }
        public float Brain_putamen_basal_ganglia_rpkm { get; set; }
        public float Brain_spinal_cord_cervical_c1_rpkm { get; set; }
        public float Brain_substantia_nigra_rpkm { get; set; }
        public float Breast_mammary_tissue_rpkm { get; set; }
        public float Cells_EBV_transformed_lymphocytes_rpkm { get; set; }
        public float Cells_transformed_fibroblasts_rpkm { get; set; }
        public float Cervix_ectocervix_rpkm { get; set; }
        public float Cervix_endocervix_rpkm { get; set; }
        public float Colon_sigmoid_rpkm { get; set; }
        public float Colon_transverse_rpkm { get; set; }
        public float Esophagus_gastroesophageal_junction_rpkm { get; set; }
        public float Esophagus_mucosa_rpkm { get; set; }
        public float Esophagus_muscularis_rpkm { get; set; }
        public float Fallopian_tube_rpkm { get; set; }
        public float Heart_atrial_appendage_rpkm { get; set; }
        public float Heart_left_ventricle_rpkm { get; set; }
        public float Kidney_cortex_rpkm { get; set; }
        public float Liver_rpkm { get; set; }
        public float Lung_rpkm { get; set; }
        public float Minor_salivary_gland_rpkm { get; set; }
        public float Muscle_skeletal_rpkm { get; set; }
        public float Nerve_tibial_rpkm { get; set; }
        public float Ovary_rpkm { get; set; }
        public float Pancreas_rpkm { get; set; }
        public float Pituitary_rpkm { get; set; }
        public float Prostate_rpkm { get; set; }
        public float Skin_not_sun_exposed_suprapubic_rpkm { get; set; }
        public float Skin_sun_exposed_lower_leg_rpkm { get; set; }
        public float Small_intestine_terminal_ileum_rpkm { get; set; }
        public float Spleen_rpkm { get; set; }
        public float Stomach_rpkm { get; set; }
        public float Testis_rpkm { get; set; }
        public float Thyroid_rpkm { get; set; }
        public float Uterus_rpkm { get; set; }
        public float Vagina_rpkm { get; set; }
        public float Whole_blood_rpkm { get; set; }

        public GTEx_line_class Deep_copy()
        {
            GTEx_line_class copy = (GTEx_line_class)this.MemberwiseClone();
            copy.EnsemblGene = (string)this.EnsemblGene.Clone();
            copy.Description = (string)this.Description.Clone();
            return copy;
        }
    }

    class GTEx_readOptions : ReadWriteOptions_base
    {
        public GTEx_readOptions()
        {
            File = Global_directory_class.Enrich_libraries_download_directory + "GTEx_Analysis_v6p_RNA-seq_RNA-SeQCv1.1.8_gene_median_rpkm_mod.txt";
            Key_propertyNames = new string[] { "EnsemblGene", "Description","Adipose_subcutaneous_rpkm","Adipose_visceral_omentum_rpkm","Adrenal_gland_rpkm","Artery_aorta_rpkm","Artery_coronary_rpkm","Artery_tibial_rpkm","Bladder_rpkm","Brain_amygdala_rpkm","Brain_anterior_cingulate_cortex_BA24_rpkm","Brain_caudate_basal_ganglia_rpkm","Brain_cerebellar_hemisphere_rpkm","Brain_cerebellum_rpkm","Brain_cortex_rpkm","Brain_frontal_cortex_BA9_rpkm","Brain_hippocampus_rpkm","Brain_hypothalamus_rpkm","Brain_nucleus_accumbens_basal_ganglia_rpkm","Brain_putamen_basal_ganglia_rpkm","Brain_spinal_cord_cervical_c1_rpkm","Brain_substantia_nigra_rpkm","Breast_mammary_tissue_rpkm","Cells_EBV_transformed_lymphocytes_rpkm","Cells_transformed_fibroblasts_rpkm","Cervix_ectocervix_rpkm","Cervix_endocervix_rpkm","Colon_sigmoid_rpkm","Colon_transverse_rpkm","Esophagus_gastroesophageal_junction_rpkm","Esophagus_mucosa_rpkm","Esophagus_muscularis_rpkm","Fallopian_tube_rpkm","Heart_atrial_appendage_rpkm","Heart_left_ventricle_rpkm","Kidney_cortex_rpkm","Liver_rpkm","Lung_rpkm","Minor_salivary_gland_rpkm","Muscle_skeletal_rpkm","Nerve_tibial_rpkm","Ovary_rpkm","Pancreas_rpkm","Pituitary_rpkm","Prostate_rpkm","Skin_not_sun_exposed_suprapubic_rpkm","Skin_sun_exposed_lower_leg_rpkm","Small_intestine_terminal_ileum_rpkm","Spleen_rpkm","Stomach_rpkm","Testis_rpkm","Thyroid_rpkm","Uterus_rpkm","Vagina_rpkm","Whole_blood_rpkm" };
            Key_columnNames = new string[]   { "Name",        "Description","Adipose - Subcutaneous",   "Adipose - Visceral (Omentum)", "Adrenal Gland",     "Artery - Aorta",   "Artery - Coronary",   "Artery - Tibial",   "Bladder",     "Brain - Amygdala",   "Brain - Anterior cingulate cortex (BA24)", "Brain - Caudate (basal ganglia)", "Brain - Cerebellar Hemisphere",   "Brain - Cerebellum",   "Brain - Cortex",   "Brain - Frontal Cortex (BA9)", "Brain - Hippocampus",   "Brain - Hypothalamus",   "Brain - Nucleus accumbens (basal ganglia)", "Brain - Putamen (basal ganglia)", "Brain - Spinal cord (cervical c-1)","Brain - Substantia nigra",   "Breast - Mammary Tissue",   "Cells - EBV-transformed lymphocytes",   "Cells - Transformed fibroblasts",   "Cervix - Ectocervix",   "Cervix - Endocervix",   "Colon - Sigmoid",   "Colon - Transverse",   "Esophagus - Gastroesophageal Junction",   "Esophagus - Mucosa",   "Esophagus - Muscularis",   "Fallopian Tube",     "Heart - Atrial Appendage",   "Heart - Left Ventricle",   "Kidney - Cortex",   "Liver",     "Lung",     "Minor Salivary Gland",     "Muscle - Skeletal",   "Nerve - Tibial",   "Ovary",     "Pancreas",     "Pituitary",     "Prostate",     "Skin - Not Sun Exposed (Suprapubic)", "Skin - Sun Exposed (Lower leg)", "Small Intestine - Terminal Ileum",   "Spleen",     "Stomach",     "Testis",     "Thyroid",     "Uterus",     "Vagina",     "Whole Blood" };
            HeadlineDelimiters = new char[] { Global_class.Tab };
            LineDelimiters = new char[] { Global_class.Tab };
            Report = ReadWrite_report_enum.Report_main;
            File_has_headline = true;
        }
    }

    class GTEx_class
    {
        public GTEx_line_class[] GTEx_lines { get; set; }

        public void Generate()
        {
            Read();
            //Sum_up_rpkm_of_same_descriptions();
            //Keep_only_genes_with_non_zero_hear_counts();
            Add_missing_symbols_and_set_all_entries_smaller_than_minimum_to_half_minimum_value(Get_all_symbols());
            Set_symbols_to_upperCase();
        }

        private void Set_symbols_to_upperCase()
        {
            foreach (GTEx_line_class gtex_line in this.GTEx_lines)
            {
                gtex_line.Description = gtex_line.Description.ToUpper();
            }
        }

        public string[] Get_all_symbols()
        {
            List<string> all_symbols = new List<string>();
            foreach (GTEx_line_class gtex_line in GTEx_lines)
            {
                all_symbols.Add(gtex_line.Description);
            }
            return all_symbols.ToArray();
        }

        public void Add_missing_symbols_and_set_all_entries_smaller_than_minimum_to_half_minimum_value(string[] symbols)
        {
            symbols = symbols.Distinct().OrderBy(l => l).ToArray();
            int symbols_length = symbols.Length;
            string symbol;
            int stringCompare=-2;

            GTEx_lines = GTEx_lines.OrderBy(l => l.Description).ToArray();
            int gtex_lines_length = this.GTEx_lines.Length;
            GTEx_line_class gtex_line;
            int indexGtex=0;

            GTEx_line_class new_gtex_line;
            List<GTEx_line_class> new_gtex_lines_list = new List<GTEx_line_class>();

            for (int indexSymbol = 0; indexSymbol < symbols_length; indexSymbol++)
            {
                symbol = symbols[indexSymbol];
                stringCompare = -2;
                while ((indexGtex < gtex_lines_length) && (stringCompare < 0))
                {
                    gtex_line = this.GTEx_lines[indexGtex];
                    stringCompare = gtex_line.Description.CompareTo(symbol);
                    if (stringCompare < 0)
                    {
                        indexGtex++;
                    }
                }
                if (stringCompare != 0)
                {
                    new_gtex_line = new GTEx_line_class();
                    new_gtex_line.EnsemblGene = "";
                    new_gtex_line.Description = (string)symbol.Clone();
                    new_gtex_lines_list.Add(new_gtex_line);
                }
            }

            new_gtex_lines_list.AddRange(this.GTEx_lines);
            this.GTEx_lines = new_gtex_lines_list.ToArray();
            float minimum_value = 10000F;

            foreach (GTEx_line_class gtex_line2 in this.GTEx_lines)
            {
                if ((gtex_line2.Adipose_subcutaneous_rpkm < minimum_value)
                    && (gtex_line2.Adipose_subcutaneous_rpkm > 0))
                {
                    minimum_value = gtex_line2.Adipose_subcutaneous_rpkm;
                }
                if ((gtex_line2.Adipose_visceral_omentum_rpkm < minimum_value)
                    && (gtex_line2.Adipose_visceral_omentum_rpkm > 0))
                {
                    minimum_value = gtex_line2.Adipose_visceral_omentum_rpkm;
                }
                if ((gtex_line2.Adrenal_gland_rpkm < minimum_value)
                    && (gtex_line2.Adrenal_gland_rpkm > 0))
                {
                    minimum_value = gtex_line2.Adrenal_gland_rpkm;
                }
                if ((gtex_line2.Artery_aorta_rpkm < minimum_value)
                    && (gtex_line2.Artery_aorta_rpkm > 0))
                {
                    minimum_value = gtex_line2.Artery_aorta_rpkm;
                }
                if ((gtex_line2.Artery_coronary_rpkm < minimum_value)
                    && (gtex_line2.Artery_coronary_rpkm > 0))
                {
                    minimum_value = gtex_line2.Artery_coronary_rpkm;
                }
                if ((gtex_line2.Artery_tibial_rpkm < minimum_value)
                    && (gtex_line2.Artery_tibial_rpkm > 0))
                {
                    minimum_value = gtex_line2.Artery_tibial_rpkm;
                }
                if ((gtex_line2.Bladder_rpkm < minimum_value)
                    && (gtex_line2.Bladder_rpkm > 0))
                {
                    minimum_value = gtex_line2.Bladder_rpkm;
                }
                if ((gtex_line2.Brain_amygdala_rpkm < minimum_value)
                    && (gtex_line2.Brain_amygdala_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_amygdala_rpkm;
                }
                if ((gtex_line2.Brain_anterior_cingulate_cortex_BA24_rpkm < minimum_value)
                    && (gtex_line2.Brain_anterior_cingulate_cortex_BA24_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_anterior_cingulate_cortex_BA24_rpkm;
                }
                if ((gtex_line2.Brain_caudate_basal_ganglia_rpkm < minimum_value)
                    && (gtex_line2.Brain_caudate_basal_ganglia_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_caudate_basal_ganglia_rpkm;
                }
                if ((gtex_line2.Brain_cerebellar_hemisphere_rpkm < minimum_value)
                    && (gtex_line2.Brain_cerebellar_hemisphere_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_cerebellar_hemisphere_rpkm;
                }
                if ((gtex_line2.Brain_cerebellum_rpkm < minimum_value)
                    && (gtex_line2.Brain_cerebellum_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_cerebellum_rpkm;
                }
                if ((gtex_line2.Brain_cortex_rpkm < minimum_value)
                    && (gtex_line2.Brain_cortex_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_cortex_rpkm;
                }
                if ((gtex_line2.Brain_frontal_cortex_BA9_rpkm < minimum_value)
                    && (gtex_line2.Brain_frontal_cortex_BA9_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_frontal_cortex_BA9_rpkm;
                }
                if ((gtex_line2.Brain_hippocampus_rpkm < minimum_value)
                    && (gtex_line2.Brain_hippocampus_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_hippocampus_rpkm;
                }
                if ((gtex_line2.Brain_hypothalamus_rpkm < minimum_value)
                    && (gtex_line2.Brain_hypothalamus_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_hypothalamus_rpkm;
                }
                if ((gtex_line2.Brain_nucleus_accumbens_basal_ganglia_rpkm < minimum_value)
                    && (gtex_line2.Brain_nucleus_accumbens_basal_ganglia_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_nucleus_accumbens_basal_ganglia_rpkm;
                }
                if ((gtex_line2.Brain_putamen_basal_ganglia_rpkm < minimum_value)
                    && (gtex_line2.Brain_putamen_basal_ganglia_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_putamen_basal_ganglia_rpkm;
                }
                if ((gtex_line2.Brain_spinal_cord_cervical_c1_rpkm < minimum_value)
                    && (gtex_line2.Brain_spinal_cord_cervical_c1_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_spinal_cord_cervical_c1_rpkm;
                }
                if ((gtex_line2.Brain_substantia_nigra_rpkm < minimum_value)
                    && (gtex_line2.Brain_substantia_nigra_rpkm > 0))
                {
                    minimum_value = gtex_line2.Brain_substantia_nigra_rpkm;
                }
                if ((gtex_line2.Breast_mammary_tissue_rpkm < minimum_value)
                    && (gtex_line2.Breast_mammary_tissue_rpkm > 0))
                {
                    minimum_value = gtex_line2.Breast_mammary_tissue_rpkm;
                }
                if ((gtex_line2.Cells_EBV_transformed_lymphocytes_rpkm < minimum_value)
                    && (gtex_line2.Cells_EBV_transformed_lymphocytes_rpkm > 0))
                {
                    minimum_value = gtex_line2.Cells_EBV_transformed_lymphocytes_rpkm;
                }
                if ((gtex_line2.Cells_transformed_fibroblasts_rpkm < minimum_value)
                    && (gtex_line2.Cells_transformed_fibroblasts_rpkm > 0))
                {
                    minimum_value = gtex_line2.Cells_transformed_fibroblasts_rpkm;
                }
                if ((gtex_line2.Cervix_ectocervix_rpkm < minimum_value)
                    && (gtex_line2.Cervix_ectocervix_rpkm > 0))
                {
                    minimum_value = gtex_line2.Cervix_ectocervix_rpkm;
                }
                if ((gtex_line2.Cervix_endocervix_rpkm < minimum_value)
                    && (gtex_line2.Cervix_endocervix_rpkm > 0))
                {
                    minimum_value = gtex_line2.Cervix_endocervix_rpkm;
                }
                if ((gtex_line2.Colon_sigmoid_rpkm < minimum_value)
                    && (gtex_line2.Colon_sigmoid_rpkm > 0))
                {
                    minimum_value = gtex_line2.Colon_sigmoid_rpkm;
                }
                if ((gtex_line2.Colon_transverse_rpkm < minimum_value)
                    && (gtex_line2.Colon_transverse_rpkm > 0))
                {
                    minimum_value = gtex_line2.Colon_transverse_rpkm;
                }
                if ((gtex_line2.Esophagus_gastroesophageal_junction_rpkm < minimum_value)
                    && (gtex_line2.Esophagus_gastroesophageal_junction_rpkm > 0))
                {
                    minimum_value = gtex_line2.Esophagus_gastroesophageal_junction_rpkm;
                }
                if ((gtex_line2.Esophagus_mucosa_rpkm < minimum_value)
                    && (gtex_line2.Esophagus_mucosa_rpkm > 0))
                {
                    minimum_value = gtex_line2.Esophagus_mucosa_rpkm;
                }
                if ((gtex_line2.Esophagus_muscularis_rpkm < minimum_value)
                    && (gtex_line2.Esophagus_muscularis_rpkm > 0))
                {
                    minimum_value = gtex_line2.Esophagus_muscularis_rpkm;
                }
                if ((gtex_line2.Fallopian_tube_rpkm < minimum_value)
                    && (gtex_line2.Fallopian_tube_rpkm > 0))
                {
                    minimum_value = gtex_line2.Fallopian_tube_rpkm;
                }
                if ((gtex_line2.Heart_atrial_appendage_rpkm < minimum_value)
                    && (gtex_line2.Heart_atrial_appendage_rpkm > 0))
                {
                    minimum_value = gtex_line2.Heart_atrial_appendage_rpkm;
                }
                if ((gtex_line2.Heart_left_ventricle_rpkm < minimum_value)
                    && (gtex_line2.Heart_left_ventricle_rpkm > 0))
                {
                    minimum_value = gtex_line2.Heart_left_ventricle_rpkm;
                }
                if ((gtex_line2.Kidney_cortex_rpkm < minimum_value)
                    && (gtex_line2.Kidney_cortex_rpkm > 0))
                {
                    minimum_value = gtex_line2.Kidney_cortex_rpkm;
                }
                if ((gtex_line2.Liver_rpkm < minimum_value)
                    && (gtex_line2.Liver_rpkm > 0))
                {
                    minimum_value = gtex_line2.Liver_rpkm;
                }
                if ((gtex_line2.Lung_rpkm < minimum_value)
                    && (gtex_line2.Lung_rpkm > 0))
                {
                    minimum_value = gtex_line2.Lung_rpkm;
                }
                if ((gtex_line2.Minor_salivary_gland_rpkm < minimum_value)
                    && (gtex_line2.Minor_salivary_gland_rpkm > 0))
                {
                    minimum_value = gtex_line2.Minor_salivary_gland_rpkm;
                }
                if ((gtex_line2.Muscle_skeletal_rpkm < minimum_value)
                    && (gtex_line2.Muscle_skeletal_rpkm > 0))
                {
                    minimum_value = gtex_line2.Muscle_skeletal_rpkm;
                }
                if ((gtex_line2.Nerve_tibial_rpkm < minimum_value)
                    && (gtex_line2.Nerve_tibial_rpkm > 0))
                {
                    minimum_value = gtex_line2.Nerve_tibial_rpkm;
                }
                if ((gtex_line2.Ovary_rpkm < minimum_value)
                    && (gtex_line2.Ovary_rpkm > 0))
                {
                    minimum_value = gtex_line2.Ovary_rpkm;
                }
                if ((gtex_line2.Pancreas_rpkm < minimum_value)
                    && (gtex_line2.Pancreas_rpkm > 0))
                {
                    minimum_value = gtex_line2.Pancreas_rpkm;
                }
                if ((gtex_line2.Pituitary_rpkm < minimum_value)
                    && (gtex_line2.Pituitary_rpkm > 0))
                {
                    minimum_value = gtex_line2.Pituitary_rpkm;
                }
                if ((gtex_line2.Prostate_rpkm < minimum_value)
                    && (gtex_line2.Prostate_rpkm > 0))
                {
                    minimum_value = gtex_line2.Prostate_rpkm;
                }
                if ((gtex_line2.Skin_not_sun_exposed_suprapubic_rpkm < minimum_value)
                    && (gtex_line2.Skin_not_sun_exposed_suprapubic_rpkm > 0))
                {
                    minimum_value = gtex_line2.Skin_not_sun_exposed_suprapubic_rpkm;
                }
                if ((gtex_line2.Skin_sun_exposed_lower_leg_rpkm < minimum_value)
                    && (gtex_line2.Skin_sun_exposed_lower_leg_rpkm > 0))
                {
                    minimum_value = gtex_line2.Skin_sun_exposed_lower_leg_rpkm;
                }
                if ((gtex_line2.Small_intestine_terminal_ileum_rpkm < minimum_value)
                    && (gtex_line2.Small_intestine_terminal_ileum_rpkm > 0))
                {
                    minimum_value = gtex_line2.Small_intestine_terminal_ileum_rpkm;
                }
                if ((gtex_line2.Spleen_rpkm < minimum_value)
                    && (gtex_line2.Spleen_rpkm > 0))
                {
                    minimum_value = gtex_line2.Spleen_rpkm;
                }
                if ((gtex_line2.Stomach_rpkm < minimum_value)
                    && (gtex_line2.Stomach_rpkm > 0))
                {
                    minimum_value = gtex_line2.Stomach_rpkm;
                }
                if ((gtex_line2.Testis_rpkm < minimum_value)
                    && (gtex_line2.Testis_rpkm > 0))
                {
                    minimum_value = gtex_line2.Testis_rpkm;
                }
                if ((gtex_line2.Thyroid_rpkm < minimum_value)
                    && (gtex_line2.Thyroid_rpkm > 0))
                {
                    minimum_value = gtex_line2.Thyroid_rpkm;
                }
                if ((gtex_line2.Uterus_rpkm < minimum_value)
                    && (gtex_line2.Uterus_rpkm > 0))
                {
                    minimum_value = gtex_line2.Uterus_rpkm;
                }
                if ((gtex_line2.Vagina_rpkm < minimum_value)
                    && (gtex_line2.Vagina_rpkm > 0))
                {
                    minimum_value = gtex_line2.Vagina_rpkm;
                }
                if ((gtex_line2.Whole_blood_rpkm < minimum_value)
                    && (gtex_line2.Whole_blood_rpkm > 0))
                {
                    minimum_value = gtex_line2.Whole_blood_rpkm;
                }
            }

            minimum_value = minimum_value * 0.5F;

            foreach (GTEx_line_class gtex_line2 in this.GTEx_lines)
            {
                if (gtex_line2.Adipose_subcutaneous_rpkm < minimum_value) { gtex_line2.Adipose_subcutaneous_rpkm = minimum_value; }
                if (gtex_line2.Adipose_visceral_omentum_rpkm < minimum_value) { gtex_line2.Adipose_visceral_omentum_rpkm = minimum_value; }
                if (gtex_line2.Adrenal_gland_rpkm < minimum_value) { gtex_line2.Adrenal_gland_rpkm = minimum_value; }
                if (gtex_line2.Artery_aorta_rpkm < minimum_value) { gtex_line2.Artery_aorta_rpkm = minimum_value; }
                if (gtex_line2.Artery_coronary_rpkm < minimum_value) { gtex_line2.Artery_coronary_rpkm = minimum_value; }
                if (gtex_line2.Artery_tibial_rpkm < minimum_value) { gtex_line2.Artery_tibial_rpkm = minimum_value; }
                if (gtex_line2.Bladder_rpkm < minimum_value) { gtex_line2.Bladder_rpkm = minimum_value; }
                if (gtex_line2.Brain_amygdala_rpkm < minimum_value) { gtex_line2.Brain_amygdala_rpkm = minimum_value; }
                if (gtex_line2.Brain_anterior_cingulate_cortex_BA24_rpkm < minimum_value) { gtex_line2.Brain_anterior_cingulate_cortex_BA24_rpkm = minimum_value; }
                if (gtex_line2.Brain_caudate_basal_ganglia_rpkm < minimum_value) { gtex_line2.Brain_caudate_basal_ganglia_rpkm = minimum_value; }
                if (gtex_line2.Brain_cerebellar_hemisphere_rpkm < minimum_value) { gtex_line2.Brain_cerebellar_hemisphere_rpkm = minimum_value; }
                if (gtex_line2.Brain_cerebellum_rpkm < minimum_value) { gtex_line2.Brain_cerebellum_rpkm = minimum_value; }
                if (gtex_line2.Brain_cortex_rpkm < minimum_value) { gtex_line2.Brain_cortex_rpkm = minimum_value; }
                if (gtex_line2.Brain_frontal_cortex_BA9_rpkm < minimum_value) { gtex_line2.Brain_frontal_cortex_BA9_rpkm = minimum_value; }
                if (gtex_line2.Brain_hippocampus_rpkm < minimum_value) { gtex_line2.Brain_hippocampus_rpkm = minimum_value; }
                if (gtex_line2.Brain_hypothalamus_rpkm < minimum_value) { gtex_line2.Brain_hypothalamus_rpkm = minimum_value; }
                if (gtex_line2.Brain_nucleus_accumbens_basal_ganglia_rpkm < minimum_value) { gtex_line2.Brain_nucleus_accumbens_basal_ganglia_rpkm = minimum_value; }
                if (gtex_line2.Brain_putamen_basal_ganglia_rpkm < minimum_value) { gtex_line2.Brain_putamen_basal_ganglia_rpkm = minimum_value; }
                if (gtex_line2.Brain_spinal_cord_cervical_c1_rpkm < minimum_value) { gtex_line2.Brain_spinal_cord_cervical_c1_rpkm = minimum_value; }
                if (gtex_line2.Brain_substantia_nigra_rpkm < minimum_value) { gtex_line2.Brain_substantia_nigra_rpkm = minimum_value; }
                if (gtex_line2.Breast_mammary_tissue_rpkm < minimum_value) { gtex_line2.Breast_mammary_tissue_rpkm = minimum_value; }
                if (gtex_line2.Cells_EBV_transformed_lymphocytes_rpkm < minimum_value) { gtex_line2.Cells_EBV_transformed_lymphocytes_rpkm = minimum_value; }
                if (gtex_line2.Cells_transformed_fibroblasts_rpkm < minimum_value) { gtex_line2.Cells_transformed_fibroblasts_rpkm = minimum_value; }
                if (gtex_line2.Cervix_ectocervix_rpkm < minimum_value) { gtex_line2.Cervix_ectocervix_rpkm = minimum_value; }
                if (gtex_line2.Cervix_endocervix_rpkm < minimum_value) { gtex_line2.Cervix_endocervix_rpkm = minimum_value; }
                if (gtex_line2.Colon_sigmoid_rpkm< minimum_value) { gtex_line2.Colon_sigmoid_rpkm = minimum_value; }
                if (gtex_line2.Colon_transverse_rpkm < minimum_value) { gtex_line2.Colon_transverse_rpkm = minimum_value; }
                if (gtex_line2.Esophagus_gastroesophageal_junction_rpkm < minimum_value) { gtex_line2.Esophagus_gastroesophageal_junction_rpkm = minimum_value; }
                if (gtex_line2.Esophagus_mucosa_rpkm < minimum_value) { gtex_line2.Esophagus_mucosa_rpkm = minimum_value; }
                if (gtex_line2.Esophagus_muscularis_rpkm < minimum_value) { gtex_line2.Esophagus_muscularis_rpkm = minimum_value; }
                if (gtex_line2.Fallopian_tube_rpkm < minimum_value) { gtex_line2.Fallopian_tube_rpkm = minimum_value; }
                if (gtex_line2.Heart_atrial_appendage_rpkm < minimum_value) { gtex_line2.Heart_atrial_appendage_rpkm = minimum_value; }
                if (gtex_line2.Heart_left_ventricle_rpkm < minimum_value) { gtex_line2.Heart_left_ventricle_rpkm = minimum_value; }
                if (gtex_line2.Kidney_cortex_rpkm < minimum_value) { gtex_line2.Kidney_cortex_rpkm = minimum_value; }
                if (gtex_line2.Liver_rpkm < minimum_value) { gtex_line2.Liver_rpkm = minimum_value; }
                if (gtex_line2.Lung_rpkm < minimum_value) { gtex_line2.Lung_rpkm = minimum_value; }
                if (gtex_line2.Minor_salivary_gland_rpkm < minimum_value) { gtex_line2.Minor_salivary_gland_rpkm = minimum_value; }
                if (gtex_line2.Muscle_skeletal_rpkm < minimum_value) { gtex_line2.Muscle_skeletal_rpkm = minimum_value; }
                if (gtex_line2.Nerve_tibial_rpkm < minimum_value) { gtex_line2.Nerve_tibial_rpkm = minimum_value; }
                if (gtex_line2.Ovary_rpkm < minimum_value) { gtex_line2.Ovary_rpkm = minimum_value; }
                if (gtex_line2.Pancreas_rpkm < minimum_value) { gtex_line2.Pancreas_rpkm = minimum_value; }
                if (gtex_line2.Pituitary_rpkm < minimum_value) { gtex_line2.Pituitary_rpkm = minimum_value; }
                if (gtex_line2.Prostate_rpkm < minimum_value) { gtex_line2.Prostate_rpkm = minimum_value; }
                if (gtex_line2.Skin_not_sun_exposed_suprapubic_rpkm < minimum_value) { gtex_line2.Skin_not_sun_exposed_suprapubic_rpkm = minimum_value; }
                if (gtex_line2.Skin_sun_exposed_lower_leg_rpkm < minimum_value) { gtex_line2.Skin_sun_exposed_lower_leg_rpkm = minimum_value; }
                if (gtex_line2.Small_intestine_terminal_ileum_rpkm < minimum_value) { gtex_line2.Small_intestine_terminal_ileum_rpkm = minimum_value; }
                if (gtex_line2.Spleen_rpkm < minimum_value) { gtex_line2.Spleen_rpkm = minimum_value; }
                if (gtex_line2.Stomach_rpkm < minimum_value) { gtex_line2.Stomach_rpkm = minimum_value; }
                if (gtex_line2.Testis_rpkm < minimum_value) { gtex_line2.Testis_rpkm = minimum_value; }
                if (gtex_line2.Thyroid_rpkm < minimum_value) { gtex_line2.Thyroid_rpkm = minimum_value; }
                if (gtex_line2.Uterus_rpkm < minimum_value) { gtex_line2.Uterus_rpkm = minimum_value; }
                if (gtex_line2.Vagina_rpkm < minimum_value) { gtex_line2.Vagina_rpkm = minimum_value; }
                if (gtex_line2.Whole_blood_rpkm < minimum_value) { gtex_line2.Whole_blood_rpkm= minimum_value; }
            }
        }

        public DE_class Generate_de_instance_after_summing_up_duplicates()
        {
            GTEx_line_class gtex_line;
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            int gtex_length = GTEx_lines.Length;
            for (int indexGTEx = 0; indexGTEx < gtex_length; indexGTEx++)
            {
                gtex_line = this.GTEx_lines[indexGTEx];
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Adipose_subcutaneous" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Adipose_subcutaneous_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Adipose_visceral_omentum" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Adipose_visceral_omentum_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Adrenal_gland" };//, "GTEx" };//, "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Adrenal_gland_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Artery_aorta" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Artery_aorta_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Artery_coronary" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Artery_coronary_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Artery_tibial" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Artery_tibial_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Bladder" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Bladder_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_amygdala" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_amygdala_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_anterior_cingulate_cortex_BA24" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_anterior_cingulate_cortex_BA24_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_caudate_basal_ganglia" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_caudate_basal_ganglia_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_cerebellar_hemisphere" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_cerebellar_hemisphere_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_cerebellum" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_cerebellum_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_cortex" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_cortex_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_frontal_cortex_BA9" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_frontal_cortex_BA9_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_hippocampus" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_hippocampus_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_hypothalamus" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_hypothalamus_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_nucleus_accumbens_basal_ganglia" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_nucleus_accumbens_basal_ganglia_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_putamen_basal_ganglia" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_putamen_basal_ganglia_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_spinal_cord_cervical_c1" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_spinal_cord_cervical_c1_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_substantia_nigra" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_substantia_nigra_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Breast_mammary_tissue" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Breast_mammary_tissue_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Cells_EBV_transformed_lymphocytes" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Cells_EBV_transformed_lymphocytes_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Cells_transformed_fibroblasts" };//,"GTEx","Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Cells_transformed_fibroblasts_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Cervix_ectocervix" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Cervix_ectocervix_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Cervix_endocervix" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Cervix_endocervix_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Colon_sigmoid" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Colon_sigmoid_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Colon_transverse" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Colon_transverse_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Esophagus_gastroesophageal_junction" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Esophagus_gastroesophageal_junction_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Esophagus_mucosa" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Esophagus_mucosa_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Esophagus_muscularis" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Esophagus_muscularis_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Fallopian_tube" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Fallopian_tube_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Heart_atrial_appendage" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Heart_atrial_appendage_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Heart_left_ventricle" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Heart_left_ventricle_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Kidney_cortex" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Kidney_cortex_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Liver" };//, "GTEx" };//, "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Liver_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Lung" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Lung_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Minor_salivary_gland" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Minor_salivary_gland_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Muscle_skeletal" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Muscle_skeletal_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Nerve_tibial" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Nerve_tibial_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Ovary" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Ovary_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Pancreas" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Pancreas_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Pituitary" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Pituitary_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Prostate" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Prostate_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Skin_not_sun_exposed_suprapubic" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Skin_not_sun_exposed_suprapubic_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Skin_sun_exposed_lower_leg" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Skin_sun_exposed_lower_leg_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Small_intestine_terminal_ileum" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Small_intestine_terminal_ileum_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Spleen" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Spleen_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Stomach" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Stomach_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Testis" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Testis_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Thyroid" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Thyroid_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Uterus" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Uterus_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Vagina" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Vagina_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Whole_blood" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Whole_blood_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);
            }

            fill_de_list = fill_de_list.OrderBy(l => l.Symbols_for_de[0]).ThenBy(l => l.Names_for_de[0]).ThenBy(l=>l.Entry_type_for_de).ThenBy(l=>l.Timepoint_for_de).ToList();
            List<Fill_de_line_class> fill_de_list_without_duplicates = new List<Fill_de_line_class>();
            int fill_de_list_count = fill_de_list.Count;
            for (int indexList = 0; indexList < fill_de_list_count; indexList++)
            {
                fill_de_line = fill_de_list[indexList];
                if ((indexList == fill_de_list_count-1)
                    || (!fill_de_line.Entry_type_for_de.Equals(fill_de_list[indexList + 1].Entry_type_for_de))
                    || (!fill_de_line.Timepoint_for_de.Equals(fill_de_list[indexList + 1].Timepoint_for_de))
                    || (!fill_de_line.Symbols_for_de[0].Equals(fill_de_list[indexList + 1].Symbols_for_de[0]))
                    || (!fill_de_line.Names_for_de[0].Equals(fill_de_list[indexList + 1].Names_for_de[0])))
                {
                    fill_de_list_without_duplicates.Add(fill_de_line);
                }
                else
                {
                    fill_de_list[indexList + 1].Value_for_de += fill_de_line.Value_for_de;
                }
            }

            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list_without_duplicates.ToArray());
            return de;
        }

        public DE_class Generate_de_instance_for_selected_tissues_after_removing_duplicates()
        {
            GTEx_line_class gtex_line;
            Fill_de_line_class fill_de_line;
            List<Fill_de_line_class> fill_de_list = new List<Fill_de_line_class>();
            int gtex_length = GTEx_lines.Length;
            for (int indexGTEx = 0; indexGTEx < gtex_length; indexGTEx++)
            {
                gtex_line = this.GTEx_lines[indexGTEx];
                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Brain_hippocampus" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Brain_hippocampus_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Colon_transverse" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Colon_transverse_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Esophagus_mucosa" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Esophagus_mucosa_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Heart_atrial_appendage" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Heart_atrial_appendage_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Heart_left_ventricle" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Heart_left_ventricle_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Kidney_cortex" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Kidney_cortex_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Liver" };//, "GTEx" };//, "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Liver_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Lung" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Lung_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Muscle_skeletal" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Muscle_skeletal_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);

                fill_de_line = new Fill_de_line_class();
                fill_de_line.Names_for_de = new string[] { "Whole_blood" };//, "GTEx", "Median rpkm" };
                fill_de_line.Symbols_for_de = new string[] { gtex_line.Description };
                fill_de_line.Value_for_de = gtex_line.Whole_blood_rpkm;
                fill_de_line.Entry_type_for_de = DE_entry_enum.Rna;
                fill_de_line.Timepoint_for_de = Timepoint_enum.H0;
                fill_de_list.Add(fill_de_line);
            }

            fill_de_list = fill_de_list.OrderBy(l => l.Combined_symbols).ThenBy(l => l.Combined_names).ToList();
            List<Fill_de_line_class> fill_de_list_without_duplicates = new List<Fill_de_line_class>();
            int fill_de_list_count = fill_de_list.Count;
            for (int indexList = 0; indexList < fill_de_list_count; indexList++)
            {
                fill_de_line = fill_de_list[indexList];
                if ((indexList == fill_de_list_count - 1)
                    || (!fill_de_line.Entry_type_for_de.Equals(fill_de_list[indexList + 1].Entry_type_for_de))
                    || (!fill_de_line.Timepoint_for_de.Equals(fill_de_list[indexList + 1].Timepoint_for_de))
                    || (!fill_de_line.Combined_symbols.Equals(fill_de_list[indexList + 1].Combined_symbols))
                    || (!fill_de_line.Combined_names.Equals(fill_de_list[indexList + 1].Combined_names)))
                {
                    fill_de_list_without_duplicates.Add(fill_de_line);
                }
                else
                {
                    fill_de_list[indexList + 1].Value_for_de += fill_de_line.Value_for_de;
                }
            }

            DE_class de = new DE_class();
            de.Fill_with_data(fill_de_list_without_duplicates.ToArray());
            return de;
        }

        private void Sum_up_rpkm_of_same_descriptions()
        {
            this.GTEx_lines = this.GTEx_lines.OrderBy(l => l.Description).ToArray();
            int gtex_length = this.GTEx_lines.Length;
            GTEx_line_class gtex_line;
            List<GTEx_line_class> keep_gtext = new List<GTEx_line_class>();
            float heart_leftVentricle_rpkm = 0;
            float heart_atrial_appendage_rpkm = 0;
            for (int indexGTEx = 0; indexGTEx < gtex_length; indexGTEx++)
            {
                gtex_line = this.GTEx_lines[indexGTEx];
                if ((indexGTEx == 0) || (!gtex_line.Description.Equals(this.GTEx_lines[indexGTEx - 1].Description)))
                {
                    heart_atrial_appendage_rpkm = 0;
                    heart_leftVentricle_rpkm = 0;
                }
                heart_leftVentricle_rpkm += gtex_line.Heart_left_ventricle_rpkm;
                heart_atrial_appendage_rpkm += gtex_line.Heart_atrial_appendage_rpkm;
                if ((indexGTEx == gtex_length-1) || (!gtex_line.Description.Equals(this.GTEx_lines[indexGTEx + 1].Description)))
                {
                    gtex_line.Heart_atrial_appendage_rpkm = heart_atrial_appendage_rpkm;
                    gtex_line.Heart_left_ventricle_rpkm = heart_leftVentricle_rpkm;
                    gtex_line.EnsemblGene = "Combined";
                    keep_gtext.Add(gtex_line);
                }
            }
            this.GTEx_lines = keep_gtext.ToArray();
        }

        private void Keep_only_genes_with_non_zero_hear_counts()
        {
            List<GTEx_line_class> keep = new List<GTEx_line_class>();
            foreach (GTEx_line_class gtex_line in this.GTEx_lines)
            {
                if ((gtex_line.Heart_left_ventricle_rpkm >= 1) || (gtex_line.Heart_atrial_appendage_rpkm >= 1))
                {
                    keep.Add(gtex_line);
                }
            }
            this.GTEx_lines = keep.ToArray();
        }

        private void Read()
        {
            GTEx_readOptions readOptions = new GTEx_readOptions();
            this.GTEx_lines = ReadWriteClass.ReadRawData_and_FillArray<GTEx_line_class>(readOptions);
        }


    }
}
