using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReadWrite;
using Highthroughput_data;
using Common_classes;
using Special_functions;
using MBCO;

namespace Statistic
{
    enum Basic_statistic_measure_of_interest_enum { E_m_p_t_y, Mean, Median, Max }
    enum Mean_type_enum { E_m_p_t_y, Arithmetic_mean, Geometric_mean, Harmonic_mean }

    class Fisher_exact_test_class
    {
        private double[] log_factorials;
        private int max_size;
        public bool Report;

        public Fisher_exact_test_class(int input_max_size, bool report)
        {
            max_size = input_max_size;
            Report = report;
            if (Report)
            {
                Report_class.WriteLine("{0}: Initialize array of factorials with max_size = {1}", typeof(Fisher_exact_test_class).Name, max_size);
            }
            log_factorials = new double[max_size+1];
            log_factorials[0] = 0;
            for (int i = 1; i < max_size+1; i++)
            {
                log_factorials[i] = log_factorials[i - 1] + Math.Log(i);
            }
        }
        private bool Check_if_n_not_larger_than_max_size(int a, int b, int c, int d)
        {
            bool smaller = true;
            int n = a+b+c+d;
            if (n > max_size + 1)
            {
                throw new Exception();
            }
            return smaller;
        }
        private double Get_specific_log_p_value(int a, int b, int c, int d)
        {
            int n = a + b + c + d;
            double log_p = log_factorials[a + b] + log_factorials[c + d] + log_factorials[a + c] + log_factorials[b + d] - log_factorials[n] - log_factorials[a] - log_factorials[b] - log_factorials[c] - log_factorials[d];
            return log_p;
        }
        private double Get_specific_p_value(int a, int b, int c, int d)
        {
            double log_p = Get_specific_log_p_value(a, b, c, d);
            return Math.Exp(log_p);
        }
        public double Get_rightTailed_p_value(int a, int b, int c, int d)
        {
            if (Report) { Report_class.WriteLine("{0}: Get right tailed p-value", typeof(Fisher_exact_test_class).Name); }
            double p;
            if (Check_if_n_not_larger_than_max_size(a, b, c, d))
            {
                p = Get_specific_p_value(a, b, c, d);
                int min = (c < b) ? c : b;
                for (int i = 0; i < min; i++)
                {
                    p += Get_specific_p_value(++a, --b, --c, ++d);
                }
            }
            else { p = -1; };
            if (Report)
            {
                for (int i = 0; i < typeof(Fisher_exact_test_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("p_value: {0}", p);
                for (int i = 0; i < typeof(Fisher_exact_test_class).Name.Length + 2; i++) { Report_class.Write(" "); }
                Report_class.WriteLine("log10_p_value: {0}", Math.Log10(p));
            }
            if (p > 1) { p = 1; }
            return p;
        }
        public Fisher_exact_test_class Deep_copy()
        {
            Fisher_exact_test_class copy = (Fisher_exact_test_class)this.MemberwiseClone();
            int log_factorials_length = this.log_factorials.Length;
            copy.log_factorials = new double[log_factorials_length];
            for (int indexLF=0; indexLF<log_factorials_length; indexLF++)
            {
                copy.log_factorials[indexLF] = this.log_factorials[indexLF];
            }
            return copy;
        }
    }

    class Overlap_class
    {
        public static string[] Get_intersection(string[] list1, string[] list2)
        {
            list1 = list1.Distinct().OrderBy(l => l).ToArray();
            list2 = list2.Distinct().OrderBy(l => l).ToArray();
            int list1_length = list1.Length;
            int list2_length = list2.Length;
            int index1 = 0;
            int index2 = 0;
            int stringCompare;
            List<string> intersection = new List<string>();
            while ((index1 < list1_length) && (index2 < list2_length))
            {
                stringCompare = list2[index2].CompareTo(list1[index1]);
                if (stringCompare < 0) { index2++; }
                else if (stringCompare > 0) { index1++; }
                else
                {
                    intersection.Add(list1[index1]);
                    index1++;
                    index2++;
                }
            }
            return intersection.ToArray();
        }
        public static string[] Get_union(string[] list1, params string[] list2)
        {
            string[] union = new string[0];
            if ((list1 != null) && (list2 != null))
            {
                list1 = list1.Distinct().OrderBy(l => l).ToArray();
                list2 = list2.Distinct().OrderBy(l => l).ToArray();
                int list1_length = list1.Length;
                int list2_length = list2.Length;
                int index1 = 0;
                int index2 = 0;
                int stringCompare;
                List<string> union_list = new List<string>();
                while ((index1 < list1_length) || (index2 < list2_length))
                {
                    if ((index1 < list1_length) && (index2 < list2_length))
                    {
                        stringCompare = list2[index2].CompareTo(list1[index1]);
                        if (stringCompare < 0)
                        {
                            union_list.Add(list2[index2]);
                            index2++;
                        }
                        else if (stringCompare > 0)
                        {
                            union_list.Add(list1[index1]);
                            index1++;
                        }
                        else
                        {
                            union_list.Add(list1[index1]);
                            index1++;
                            index2++;
                        }
                    }
                    else if (index1 < list1_length)
                    {
                        union_list.Add(list1[index1]);
                        index1++;
                    }
                    else if (index2 < list2_length)
                    {
                        union_list.Add(list2[index2]);
                        index2++;
                    }
                }
                union = union_list.ToArray();
            }
            else if (list1 != null)
            {
                union = list1;
            }
            else if (list2 != null)
            {
                union = list2;
            }
            return union;
        }
        public static string[] Get_part_of_list1_but_not_of_list2(string[] list1, params string[] list2)
        {
            list1 = list1.Distinct().OrderBy(l => l).ToArray();
            list2 = list2.Distinct().OrderBy(l => l).ToArray();
            List<string> not = new List<string>();
            int list1_length = list1.Length;
            int list2_length = list2.Length;
            int index2=0;
            int stringCompare;
            for (int index1 = 0; index1 < list1_length; index1++)
            {
                stringCompare = -2;
                while ((index2 < list2_length) && (stringCompare < 0))
                {
                    stringCompare = list2[index2].CompareTo(list1[index1]);
                    if (stringCompare < 0) { index2++; }
                }
                if ((stringCompare > 0) || (index2 == list2_length))
                {
                    not.Add(list1[index1]);
                }
            }
            return not.ToArray();
        }
        public static T[] Get_part_of_list1_but_not_of_list2<T>(T[] list1, T[] list2) where T:IComparable
        {
            list1 = list1.Distinct().OrderBy(l => l).ToArray();
            list2 = list2.Distinct().OrderBy(l => l).ToArray();
            List<T> not = new List<T>();
            int list1_length = list1.Length;
            int list2_length = list2.Length;
            int index2 = 0;
            int stringCompare;
            for (int index1 = 0; index1 < list1_length; index1++)
            {
                stringCompare = -2;
                while ((index2 < list2_length) && (stringCompare < 0))
                {
                    stringCompare = list2[index2].CompareTo(list1[index1]);
                    if (stringCompare < 0) { index2++; }
                }
                if ((stringCompare > 0) || (index2 == list2_length))
                {
                    not.Add(list1[index1]);
                }
            }
            return not.ToArray();
        }
    }

    class Text_class
    {
        public static void Set_first_letter_to_uppercase_and_rest_to_lowercase(ref string text)
        {
            int text_length = text.Length;
            text = char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }

    class Math_class
    {
        private static string Get_hexadecimal_sign(int number)
        {
            string sign = "no value";
            switch (number)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    sign = number.ToString();
                    break;
                case 10:
                    sign = "A";
                    break;
                case 11:
                    sign = "B";
                    break;
                case 12:
                    sign = "C";
                    break;
                case 13:
                    sign = "D";
                    break;
                case 14:
                    sign = "E";
                    break;
                case 15:
                    sign = "F";
                    break;
                default:
                    throw new Exception();
            }
            return sign;
        }
        public static string Convert_into_two_digit_hexadecimal(int number)
        {
            if ((number > 255) || (number < 0))
            {
                throw new Exception();
            }
            else
            {
                int multiples_of_16 = (int)Math.Floor((double)number / (double)16);
                int modulus = number % 16;
                return Get_hexadecimal_sign(multiples_of_16) + Get_hexadecimal_sign(modulus);
            }
        }
        public static string Get_hexadecimal_code(int red, int green, int blue)
        {
            return "#" + Convert_into_two_digit_hexadecimal(red) + Convert_into_two_digit_hexadecimal(green) + Convert_into_two_digit_hexadecimal(blue);
        }
        public static string Get_hexadecimal_black()
        {
            return Get_hexadecimal_code(0, 0, 0);
        }
        public static float Get_average(float[] values)
        {
            int values_length = values.Length;
            float sum = 0;
            for (int indexV = 0; indexV < values_length; indexV++)
            {
                sum += values[indexV];
            }
            return sum / (float)values_length;
        }
        public static double Get_average(double[] values)
        {
            int values_length = values.Length;
            double sum = 0;
            for (int indexV = 0; indexV < values_length; indexV++)
            {
                sum += values[indexV];
            }
            return sum / (double)values_length;
        }
    }

    class Correlation_coefficient_class
    {
        public double Get_pearson_correlation_coefficient(double[] array_x, double[] array_y)
        {
            int array_x_length = array_x.Length;
            int array_y_length = array_y.Length;
            if (array_x_length != array_y_length)
            {
                throw new Exception();
            }

            double[] array_xy = new double[array_x_length];
            double[] array_xp2 = new double[array_x_length];
            double[] array_yp2 = new double[array_x_length];
            double sum_x = 0;
            double sum_y = 0;
            double sum_xy = 0;
            double sum_xpow2 = 0;
            double sum_ypow2 = 0;
            for (int indexXY = 0; indexXY < array_x_length; indexXY++)
            {
                array_xy[indexXY] = array_x[indexXY] * array_y[indexXY];
            }
            for (int indexXP2 = 0; indexXP2 < array_x_length; indexXP2++)
            {
                array_xp2[indexXP2] = Math.Pow(array_x[indexXP2], 2.0);
            }
            for (int indexYP2 = 0; indexYP2 < array_x_length; indexYP2++)
            {
                array_yp2[indexYP2] = Math.Pow(array_y[indexYP2], 2.0);
            }
            for (int indexYP2 = 0; indexYP2 < array_y_length; indexYP2++)
            {
                array_yp2[indexYP2] = Math.Pow(array_y[indexYP2], 2.0);
            }
            for (int index_x = 0; index_x < array_x_length; index_x++)
            {
                sum_x += array_x[index_x];
            }
            for (int index_y = 0; index_y < array_y_length; index_y++)
            {
                sum_y += array_y[index_y];
            }
            for (int index_xy = 0; index_xy < array_x_length; index_xy++)
            {
                sum_xy += array_xy[index_xy];
            }
            for (int indexXpow2 = 0; indexXpow2 < array_x_length; indexXpow2++)
            {
                sum_xpow2 += array_xp2[indexXpow2];
            }
            for (int indexYpow2 = 0; indexYpow2 < array_y_length; indexYpow2++)
            {
                sum_ypow2 += array_yp2[indexYpow2];
            }
            double Ex2 = Math.Pow(sum_x, 2.00);
            double Ey2 = Math.Pow(sum_y, 2.00);
            double Correl = (array_x_length * sum_xy - sum_x * sum_y) / Math.Sqrt((array_x_length * sum_xpow2 - Ex2) * (array_x_length * sum_ypow2 - Ey2));
            return Correl;
        }
    }
}
