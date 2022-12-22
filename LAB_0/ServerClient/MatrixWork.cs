using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerClient
{
    internal class MatrixWork
    {
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
        public static void CreateMatrix(string matrixStr, out int step, out double[,] matrix)
        {
            var matrixArray = matrixStr.Split(' ');
            step = Convert.ToInt32(matrixArray.Last());
            matrixArray = matrixArray.Take(matrixArray.Length - 1).ToArray();
            var size = matrixArray.Length / 2;
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
