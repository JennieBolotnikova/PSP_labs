using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Lab6PSP
{
    class Server
    {
        readonly TcpListener Listener;

        static X509Certificate2 serverCertificate = null;

        public Server(int Port)
        {
            string s = IPAddress.Any.ToString();
            serverCertificate = new X509Certificate2("E:/studies/4/ПСП/ЛАБ_6/lab_5_6/RootCA.pfx", "password");
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start();
            Console.WriteLine("Server is running. https://localhost:" + Port);

            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());
            }
        }

        static void ClientThread(object StateInfo)
        {
            TcpClient Client = (TcpClient)StateInfo;

            SslStream sslStream = new SslStream(Client.GetStream(), false);

            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls12, false);

                string Request = "";
                byte[] Buffer = new byte[1024];
                int Count;
                while ((Count = sslStream.Read(Buffer, 0, Buffer.Length)) > 0)
                {
                    Request += Encoding.UTF8.GetString(Buffer, 0, Count);
                    if (Request.IndexOf("\r\n\r\n") >= 0 || Request.Length > 4096)
                    {
                        break;
                    }
                }

                Match ReqMatch = Regex.Match(Request, @"^\w+\s+([^\s]+)[^\s]*\s+HTTP/.*|");

                if (ReqMatch == Match.Empty)
                {
                    SendError(Client, 400);
                    return;
                }

                string RequestUri = ReqMatch.Groups[1].Value;

                RequestUri = Uri.UnescapeDataString(RequestUri);

                if (RequestUri.IndexOf("..") >= 0)
                {
                    SendError(Client, 400);
                    return;
                }

                string Headers;
                byte[] HeadersBuffer;

                if (RequestUri.EndsWith("/") ||
                    RequestUri.EndsWith(".css") ||
                    RequestUri.EndsWith(".html") ||
                    RequestUri.EndsWith(".js") ||
                    RequestUri.EndsWith(".svg") ||
                    RequestUri.EndsWith(".ico") ||
                    RequestUri.EndsWith(".htm"))
                {
                    if (RequestUri.EndsWith("/"))
                    {
                        RequestUri += "index.html";
                    }

                    string FilePath = "Resources/" + RequestUri;

                    if (!File.Exists(FilePath))
                    {
                        SendError(Client, 404);
                        return;
                    }

                    string Extension = RequestUri.Substring(RequestUri.LastIndexOf('.'));

                    string ContentType = "";

                    switch (Extension)
                    {
                        case ".htm":
                        case ".html":
                            ContentType = "text/html";
                            break;
                        case ".css":
                            ContentType = "text/stylesheet";
                            break;
                        case ".js":
                            ContentType = "text/javascript";
                            break;
                        case ".jpg":
                            ContentType = "image/jpeg";
                            break;
                        case ".svg":
                            ContentType = "image/svg+xml";
                            break;
                        case ".jpeg":
                        case ".png":
                        case ".gif":
                            ContentType = "image/" + Extension.Substring(1);
                            break;
                        default:
                            if (Extension.Length > 1)
                            {
                                ContentType = "application/" + Extension.Substring(1);
                            }
                            else
                            {
                                ContentType = "application/unknown";
                            }
                            break;
                    }

                    FileStream FS;
                    try
                    {
                        FS = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    catch (Exception)
                    {
                        SendError(Client, 500);
                        return;
                    }

                    Headers = "HTTP/1.1 200 OK\nContent-Type: " + ContentType + "; charset=utf-8" + "\nContent-Length: " + FS.Length + "\n\n";
                    HeadersBuffer = Encoding.UTF8.GetBytes(Headers);
                    sslStream.Write(HeadersBuffer, 0, HeadersBuffer.Length);

                    while (FS.Position < FS.Length)
                    {
                        Count = FS.Read(Buffer, 0, Buffer.Length);
                        sslStream.Write(Buffer, 0, Count);
                    }

                    FS.Close();
                    Client.Close();
                }
                else if (RequestUri.Length > 1)
                {
                    string text = RequestUri.Remove(0, 1);

                    string emails = "";
                    string emailsRegex = "([a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+)";
                    MatchCollection matches = Regex.Matches(text, emailsRegex);
                    foreach (Match match in matches)
                    {
                        emails += match.Value + "<br/>";
                    }

                    byte[] message = Encoding.UTF8.GetBytes(emails);

                    Headers = "HTTP/1.1 200 OK\nContent-Type: " + "text" + "\nContent-Length: " + message.Length + "\n\n";
                    HeadersBuffer = Encoding.UTF8.GetBytes(Headers);
                    sslStream.Write(HeadersBuffer, 0, HeadersBuffer.Length);
                    sslStream.Write(message, 0, message.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                sslStream.Close();
                Client.Close();
            }

        }

        private static void SendError(TcpClient Client, int Code)
        {
            string CodeStr = Code.ToString() + " " + ((HttpStatusCode)Code).ToString();
            string Html = "<html><body><h1>" + CodeStr + "</h1></body></html>";
            string Str = "HTTP/1.1 " + CodeStr + "\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            byte[] Buffer = Encoding.UTF8.GetBytes(Str);
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            Client.Close();
        }

        ~Server()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }
    }
}
