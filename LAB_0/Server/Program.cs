using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    double[,] matrix = new double[,]
        //        {
        //        { 1, 2 },
        //        { 3, 4 }
        //        };
        //    double[,] result = CalculateMatrix.PowMatrix(matrix, 2);
        //    double[,] result1 = CalculateMatrix.PowMatrix(matrix, 3);
        //    double[,] result2 = CalculateMatrix.TwoMatrix(result, result1);

        //    PrintMatrix(result2);

        //    Console.ReadKey();
        //}

        private static void PrintMatrix(double[,] matrix)
        {
            Console.WriteLine("Matrix:");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write("\t");
                    Console.Write(matrix[i, j]);
                }
                Console.WriteLine();
            }
        }

        private static void ResultPortsAndAddress(string[] masData,out string[] address, out int[] ports) 
        {
            var p = new int[masData.Length/2];
            var a = new string[masData.Length/2];
            var j = 0;
            var k = 0;
            for (int i = 0; i < masData.Length; i++) 
            {
                if (i % 2 == 0)
                {
                    a[j] = masData[i];
                    j++;
                }
                else
                {
                    p[k] = Convert.ToInt32(masData[i]);
                    k++;
                }
            }
            address = a;
            ports = p;
        }

        private static string ReadFile(string path)
        {
            string text = "";
            using (var reader = new StreamReader(path))
            {
                text = reader.ReadToEnd();

            }
            return text;

        }

        static string[] connectServersStr = Program.ReadFile("servers.txt").Split("\r\n");

        static string[] connectStr = Program.ReadFile("txt.txt").Split("\r\n");

        static int port = Convert.ToInt32(connectStr[connectStr.Length - 1]); // порт для приема входящих запросов
        public static void Main(string[] args)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(connectStr[0]), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);
                    ResultPortsAndAddress(connectServersStr, out string[] address, out int[] ports);
                    var getMatrixsList = ClientWork.ClientConnect(address, ports, builder.ToString(), ports.Length - 1);
                    var allMatr = new List<double[,]>();
                    //CalculateMatrix.CreateMatrixWithoutStep(getMatrixsList[0], out double[,] matrFirst);
                    for (int i = 0; i < getMatrixsList.Count;i++) 
                    {
                        CalculateMatrix.CreateMatrixWithoutStep(getMatrixsList[i], out double[,] nextMatr);
                        allMatr.Add(nextMatr);
                    }

                    var resultMatr = CalculateMatrix.MultiAllMatrix(allMatr);

                    //Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());
                    var res = CalculateMatrix.ResultForMainServer(resultMatr);

                    // отправляем ответ
                    string message = res;
                    data = Encoding.UTF8.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
