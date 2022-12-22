using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string host = "127.0.0.1";

                // Буфер для входящих данных
                byte[] bytes = new byte[1024];

                // Устанавливаем удаленную точку для сокета
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(host), 11000);

                // Создание сокета отправителя, который привязывается к удаленной токче с протоколом TCP
                Socket client_socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Соединяем сокет с удаленной точкой
                client_socket.Connect(ipEndPoint);

                Console.WriteLine("Сокет соединяется с {0} ", client_socket.RemoteEndPoint.ToString());

                Console.Write("Введите a: ");
                double a = Convert.ToDouble(Console.ReadLine());

                Console.Write("Введите b: ");
                double b = Convert.ToDouble(Console.ReadLine());

                double[] numbers = new double[2];
                numbers[0] = a;
                numbers[1] = b;

                byte[] msg;

                msg = new byte[numbers.Length * sizeof(double)];
                Buffer.BlockCopy(numbers, 0, msg, 0, msg.Length);

                // Отправляем данные через сокет
                client_socket.Send(msg);

                Console.WriteLine("Отправка: ОК");
                // Получаем ответ от сервера
                int bytesReceive = client_socket.Receive(bytes);

                Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesReceive));

                // Освобождаем сокет
                client_socket.Shutdown(SocketShutdown.Both);
                client_socket.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}

