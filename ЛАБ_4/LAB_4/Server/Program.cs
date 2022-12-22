using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        const double epsilon = 1e-3;
        static int F(int n)
        {
            int f, f1 = 1, f2 = 1, m = 0;
            while (m < n - 1)
            {
                f = f1 + f2;
                f1 = f2;
                f2 = f;
                ++m;
            }
            return f1;
        }

        static double Fun(double x)
        {
            return Math.Pow(Math.E, -1) - Math.Cos(x);
        }

        static double Fib(double a, double b)
        {
            double x1, x2, _x, xf1, xf2;
            int k = 0;
            int N = 0;
            double fn1 = 1, fn2 = 1, fn, f = (b - a) / epsilon;

            while (fn1 < f)
            {
                fn = fn1 + fn2;
                fn1 = fn2;
                fn2 = fn;
                ++N;
            }
            bool bix;
            int ix = N & 1;
            if (ix == 1)
                bix = true;
            else
                bix = false;
            x1 = a + (double)F(N - 2) / F(N) * (b - a) - (bix ? -1 : 1) * epsilon / F(N);
            x2 = a + (double)F(N - 1) / F(N) * (b - a) + (bix ? -1 : 1) * epsilon / F(N);
            xf1 = Fun(x1);
            xf2 = Fun(x2);
        P:
            ++k;
            if (xf1 >= xf2)
            {
                ix = (N - k) & 1;
                if (ix == 1)
                    bix = true;
                else
                    bix = false;
                a = x1;
                x1 = x2;
                xf1 = xf2;
                x2 = a + (double)F(N - k - 1) / F(N - k) * (b - a) + (bix ? -1 : 1) * epsilon / F(N - k);
                xf2 = Fun(x2);
            }
            else
            {
                ix = (N - k) & 1;
                if (ix == 1)
                    bix = true;
                else
                    bix = false;
                b = x2;
                x2 = x1;
                xf2 = xf1;
                x1 = a + (double)F(N - k - 2) / F(N - k) * (b - a) - (bix ? -1 : 1) * epsilon / F(N - k);
                xf1 = Fun(x1);
            }
            if (Math.Abs(b - a) <= epsilon)
            {
                _x = (a + b) / 2;
                double result = Fun(_x);
                return result;
            }
            else
                goto P;
        }


        static void ThreadProc(Object obj)
        {
            Socket handler = (Socket)obj;
            string data = null;

            // Мы дождались клиента, пытающегося с нами соединиться
            byte[] bytes = new byte[sizeof(double) * 2];
            double[] numbers = new double[2];
            int bytesRec = handler.Receive(bytes);
            data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Buffer.BlockCopy(bytes, 0, numbers, 0, bytes.Length);
            double a = Convert.ToDouble(numbers[0]);
            double b = Convert.ToDouble(numbers[1]);

            Console.Write("Подсчет: ");

            double result = Fib(a, b);

            Console.WriteLine("ОК");

            // Отправляем ответ клиенту
            string reply = "Результат = " + result;
            byte[] msg = Encoding.UTF8.GetBytes(reply);
            handler.Send(msg);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку
            string host = Dns.GetHostName();

            int port = 11000;
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                //связывает объект сокет с локальной конечной точкой
                sListener.Bind(ipEndPoint);
                //начинает прослушивание ыходящих запросов
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Имя сервера: {0}", host);
                    Console.WriteLine("Порт: {0}", port);
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение

                    Socket handler = sListener.Accept();

                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), handler);
                }
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
