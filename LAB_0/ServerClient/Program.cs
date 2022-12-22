using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ServerClient
{
    class Program
    {
        private static string ReadFile(string path)
        {
            string text = "";
            using (var reader = new StreamReader(path))
            {
                text = reader.ReadToEnd();

            }
            return text;

        }

        static string[] connectStr = Program.ReadFile("txt.txt").Split("\r\n");
        static int port = Convert.ToInt32(connectStr[1]); // порт для приема входящих запросов
        static void Main(string[] args)
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(connectStr[0]), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер решатель");

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
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());
                    MatrixWork.CreateMatrix(builder.ToString(), out int step, out double[,] matrix);
                    var result = MatrixWork.ResultForMainServer(MatrixWork.PowMatrix(matrix, step));
                    // отправляем ответ
                    string message = result;
                    data = Encoding.Unicode.GetBytes(message);
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