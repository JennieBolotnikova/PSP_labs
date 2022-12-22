using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ClientWork
    {
        private static List<string> results = new List<string>();

        public static List<string> ClientConnect(string[] address, int[] port, string matrStr, int serverCount)
        {
            var answerForServer = CalculateMatrix.GetClientMatrix(matrStr, serverCount);
            var masThread = new List<Thread>();
            for (int i=0;i< serverCount; i++) 
            {
                var threadData = new ThreadData(address[i], port[i], answerForServer[i]);
                var thread = new Thread(new ParameterizedThreadStart(DoWork));
                masThread.Add(thread);

                thread.Start(threadData);
            }

            foreach (var v in masThread) 
            {
                v.Join();
            }
            return results;

        }

        static void DoWork(object data)
        {
            if (data is ThreadData threadData)
            {
                 SendMatrix(threadData.Address, threadData.Port, threadData.MatrixForSolve);
            }
        }
        public static void SendMatrix(string address, int port, string answerForServer) 
        {
            string result;
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);


                string message = answerForServer;
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                // получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                result = (builder.ToString());

                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

            }
            catch (Exception ex)
            {
                result = ex.Message;
                Console.WriteLine(ex.Message);
            }

            results.Add(result);
        }
    }
}
