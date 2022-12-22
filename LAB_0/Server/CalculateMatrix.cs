using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class CalculateMatrix
    {
        public static string ResultForMainServer(double[,] matr)
        {
            var resStr = "";
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                for (int j = 0; j < matr.GetLength(1); j++)
                {
                    resStr += matr[i, j] + " ";
                }
            }
            return resStr;
        }
        public static string ResultForServer(double[,] matr, int step)
        {
            var resStr = "";
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                for (int j = 0; j < matr.GetLength(1); j++)
                {
                    resStr += matr[i, j] + " ";
                }
            }
            return resStr + step;
        }
        public static string ResultForClient(string str)
        {
            CreateMatrix(str, out int step, out double[,] matr);
            var resMatr = PowMatrix(matr, step);
            var resStr = "";
            for (int i = 0; i < resMatr.GetLength(0); i++)
            {
                for (int j = 0; j < resMatr.GetLength(1); j++)
                {
                    resStr += resMatr[i, j] + " ";
                }
            }
            return resStr;
        }
        public static double[,] PowMatrix(double[,] m, int power)
        {
            if (m.GetLength(0) != m.GetLength(1))
                throw new ArgumentException("Матрица не квадратная");
            if (power < 1)
                throw new ArgumentException("Степень должна быть натуральным числом");

            int size = m.GetLength(0);
            double[,] result = m;
            for (int p = 1; p < power; p++)
            {
                double[,] temp = new double[size, size];
                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                    {
                        double sum = 0.0;
                        for (int k = 0; k < size; k++)
                            sum += result[i, k] * m[k, j];
                        temp[i, j] = sum;
                    }
                result = temp;
            }
            return result;
        }
        
        public static double[,] TwoMatrix(double[,] m, double[,] m2)
        {
            int size = m.GetLength(0);
            double[,] temp = new double[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    double sum = 0.0;
                    for (int k = 0; k < size; k++)
                        sum += m2[i, k] * m[k, j];
                    temp[i, j] = sum;
                }
            m2 = temp;
            return m2;
        }

        public static double[,] MultiAllMatrix(List<double[,]> matrixs) 
        {
            var result = matrixs[0];
            for (int i = 1; i< matrixs.Count; i++) 
            {
                result = TwoMatrix(result, matrixs[i]);
            }
            return result;
        }

        public static string[] GetClientMatrix(string matrStr, int serverCount) 
        {
            var resultStr = new List<string>();

            CreateMatrix(matrStr, out int step, out double[,] matr);
            if (matr.GetLength(0) != matr.GetLength(1))
                throw new Exception("Матрица не квадратная.");
            var xStep = step / serverCount;
            var sends = 0;
            var sendStep = 0;
            for (int i = 0; i < serverCount; i++)
            {
                if (i == serverCount-1)
                {
                    sendStep = step - xStep * (serverCount - 1);
                    resultStr.Add(ResultForServer(matr, sendStep));
                }
                else 
                {
                    sendStep = xStep;
                    resultStr.Add(ResultForServer(matr, sendStep));
                }
                sends += xStep;
            }
            return resultStr.ToArray();
        }

        public static void CreateMatrix(string matrixStr, out int step, out double[,] matrix)
        {
            var matrixArray = matrixStr.Split(' ');
            step = Convert.ToInt32(matrixArray.Last());
            matrixArray = matrixArray.Take(matrixArray.Length - 1).ToArray();
            var size = matrixArray.Length/ 2;
            matrix = new double[size, size];
            var col = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = Convert.ToDouble(matrixArray[col]);
                    col += 1;
                }
            }
        }
        public static void CreateMatrixWithoutStep(string matrixStr, out double[,] matrix)
        {
            var matrixArray = matrixStr.Split(' ');
            var size = matrixArray.Length/ 2;
            matrix = new double[size, size];
            var col = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = Convert.ToDouble(matrixArray[col]);
                    col += 1;
                }
            }
        }
    }
}
