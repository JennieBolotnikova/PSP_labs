#include "ServerTCP.h"

Server::Server(int port, int clientCount)
{
	square = 0.0;
	clientFinished = 0;
	this->clientCount = clientCount;
	clients = new SOCKET[clientCount];
	clientThreads = new std::thread[clientCount];
	sockVer = MAKEWORD(2, 2);

	retVal = WSAStartup(sockVer, &wsaData);
	//создание сокета (семейство протокола, потоковый сокет, протокол)
	sock = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (sock == INVALID_SOCKET)
	{
		//ошибка создания сокета
		std::cout << "Unable to create socket" << std::endl;
		WSACleanup();//Деинициаоизация бибилиотеки Winsoock
	}
	//описывает сокет для работы с протоколом 
	SOCKADDR_IN sin;
	sin.sin_family = PF_INET;
	sin.sin_port = htons(port);
	sin.sin_addr.s_addr = INADDR_ANY;
	//вызываем bind для связывания
	retVal = bind(sock, (LPSOCKADDR)&sin, sizeof(sin));
	if (retVal == SOCKET_ERROR)
	{		
		std::cout << "Unable to bind" << std::endl;
		WSACleanup();//Деинициаоизация бибилиотеки Winsoock
	}
}

std::thread* Server::listenClients(double start, double end)
{
	for (int i = 0; i < clientCount; i++)
	{
		int retVal = listen(sock, 10);
		if (retVal == SOCKET_ERROR)
		{
			std::cout << "Unable to listen" << std::endl;
		}

		clients[i] = accept(sock, NULL, NULL);

		if (clients[i] == INVALID_SOCKET)
		{
			std::cout << "Unable to accept" << std::endl;
		}
		
		clientThreads[i] = std::thread(ProcessingClients, clients[i], clientCount, i, &clientFinished, start, end, &square);
		clientThreads[i].detach();
	}

	return clientThreads;
}

double Server::getSquare()
{
	return square;
}

void ProcessingClients(SOCKET client, int count, int index, int* clientFinished, double start, double end, double* square) {
	const int RESVBUF = 32;
	double step = (end - start) / count;
	double x1 = step * (double)index + start;
	double x2 = step * (1 + (double)index) + start;

	std::stringstream stream_x1;
	stream_x1 << std::fixed << std::setprecision(3) << x1;

	std::stringstream stream_x2;
	stream_x2 << std::fixed << std::setprecision(3) << x2;

	std::string sendMessage = stream_x1.str() + " " + stream_x2.str();

	std::cout << "Connection with " << index << " client established successfully. Sent data: " << sendMessage << std::endl;

	int err = 0;
	//
	err = send(client, sendMessage.c_str(), sendMessage.length(), 0);

	char receiveMessageBuf[RESVBUF];
	//
	err = recv(client, receiveMessageBuf, RESVBUF, 0);

	std::string receiveMessage = "";
	for (int i = 0; i < RESVBUF; i++)
	{
		receiveMessage += receiveMessageBuf[i];
	}

	double receive = atof(receiveMessage.c_str());

	std::cout << "Answer from " << index << " client. Recieved data: " << receive << std::endl;

	*square += receive;
	*clientFinished += 1;
}

Server::~Server()
{
	for (int i = 0; i < clientCount; i++)
	{
		closesocket(clients[i]);
	}

	delete[] clientThreads;
	delete[] clients;
	closesocket(sock);
	WSACleanup();
}
