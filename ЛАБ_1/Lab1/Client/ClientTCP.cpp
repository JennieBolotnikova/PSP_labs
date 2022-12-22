#include "ClientTCP.h"



ClientTCP::ClientTCP(int port)
{
    std::cout << "Trying to connect to port " << port << std::endl;

    ver = MAKEWORD(2, 2);

    retVal = WSAStartup(ver, (LPWSADATA)&wsaData);

    hostEnt = gethostbyname("localhost");

    if (!hostEnt)
    {
        std::cout << "Unable to collect gethostbyname" << std::endl;
        WSACleanup();
    }
    //создание сокета (семейство протокола, потоковый сокет, протокол)
    sock = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);

    if (sock == SOCKET_ERROR)
    {
        std::cout << "Unable to create socket" << std::endl;
        WSACleanup();//Деинициаоизация бибилиотеки Winsoock
    }

    SOCKADDR_IN serverInfo;

    serverInfo.sin_family = PF_INET;
    if (hostEnt != nullptr)
    {
        serverInfo.sin_addr = *((LPIN_ADDR)*hostEnt->h_addr_list);
    }
    serverInfo.sin_port = htons(port);

    retVal = connect(sock, (LPSOCKADDR)&serverInfo, sizeof(serverInfo));

    if (retVal == SOCKET_ERROR)
    {
        std::cout << "Unable to connect" << std::endl;
        WSACleanup();
    } else {
        std::cout << "Connected!" << std::endl;
    }
}

ClientTCP::~ClientTCP()
{
	closesocket(sock);
	WSACleanup();
}

void ClientTCP::sendMessage(std::string message)
{
    retVal = send(sock, message.c_str(), message.length(), 0);

    if (retVal == SOCKET_ERROR)
    {
        std::cout << "Unable to send" << std::endl;
        WSACleanup();
    }
}

std::string ClientTCP::receiveMessage(int size)
{
    char *szResponse = new char[size];
    retVal = recv(sock, szResponse, size, 0);

    if (retVal == SOCKET_ERROR)
    {
        std::cout << "Unable to receive" << std::endl;
        WSACleanup();
    }

    return to_string(szResponse, size);
}

std::string to_string(char* arr, int size) {
    std::string s = "";
    for (int i = 0; i < size; i++) {
        s = s + arr[i];
    }
    return s;
}