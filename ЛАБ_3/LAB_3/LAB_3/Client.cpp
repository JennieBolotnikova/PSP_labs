#pragma comment(lib, "ws2_32.lib")
#include <winsock2.h>
#include <windows.h>
#include <iostream>
#include <vector>
#include <sstream>
#pragma warning(disable: 4996)



int main(int argc, char* argv[])
{

    //загрузка версии библиотеки для вызова сетевых функций
    WSAData wsaData;

    //запрашиваемая версия для библиотеки winsock
    WORD DLLVersion = MAKEWORD(2, 1);
    //фуекция для загрузки библиотеки
    if (WSAStartup(DLLVersion, &wsaData) != 0) {
        std::cout << "Error lib\n";
        exit(1);
    }

    char ip_server[15] = "127.0.0.1";


    //std::vector<int> size_segment(2);


    //структура для хранения адреса (для интернет протоколов)
    SOCKADDR_IN socket_addres;
    int sizeof_socket_addres = sizeof(socket_addres);
    //для хранения ip-адреса
    socket_addres.sin_addr.s_addr = inet_addr(ip_server);
    //для хранения порта
    socket_addres.sin_port = htons(5000);
    //указание семейства протоколов - af_inet -сокет интернета
    socket_addres.sin_family = AF_INET;

    //создаем сокет (параметры: протокол, тип сокета - потоковый или асинхронный
    SOCKET client_Socket = socket(AF_INET, SOCK_STREAM, 0);

    if (connect(client_Socket, (SOCKADDR*)&socket_addres, sizeof(socket_addres)) != 0) {
        std::cout << "Error connection to Server\n";
        return 1;
    }

    std::cout << "Connected to Server\n";

    double a, b;

    std::cout << "Input a\n";
    std::cin >> a;

    std::cout << "Input b\n";
    std::cin >> b;



    std::stringstream s;
    s << a;
    s << " ";
    s << b;
    std::string str = s.str();
    const char* buff2 = str.c_str();
    send(client_Socket, buff2, std::strlen(buff2) + 1, 0);

    double result, s2;
    char buff[2048];

    recv(client_Socket, buff, sizeof(double) * 3 + sizeof(char) * 2 + 1, 0);
    result = strtod(buff, NULL);


    printf("Result from server = %f\n", result);

    system("pause");
    closesocket(client_Socket);
    WSACleanup();

    return(0);
}




