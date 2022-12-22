#pragma comment(lib, "ws2_32.lib")
#include <winsock2.h>
#include <windows.h>
#include "Poco\ThreadPool.h"
#include <iostream>
#include <vector>
#include <sstream>
#include <chrono>
#pragma warning(disable: 4996)

#define eps 1e-3

//прототип функции 
DWORD WINAPI ThreadFunc(LPVOID client_socket);
SOCKET server_socket;

#define eps 1e-3

double Fun(double x)
{
    return (2 * x * x * x - 6 * x + 3);
}
int F(int n)
{
    int f;
    int f1 = 1;
    int f2 = 1;
    int m = 0;
    while (m < n - 1)
    {
        f = f1 + f2;
        f1 = f2;
        f2 = f;
        ++m;
    }
    return f1;
}
double Fib(double a, double b)
{
    double x1, x2, _x, xf1, xf2;
    int k = 0;
    int N = 0;
    double fn1 = 1;
    double fn2 = 1;
    double fn;
    double f = (b - a) / eps;
    while (fn1 < f)
    {
        fn = fn1 + fn2;
        fn1 = fn2;
        fn2 = fn;
        ++N;
    }
    x1 = a + (double)F(N - 2) / F(N) * (b - a) - (N & 1 ? -1 : 1) * eps / F(N);
    x2 = a + (double)F(N - 1) / F(N) * (b - a) + (N & 1 ? -1 : 1) * eps / F(N);
    xf1 = Fun(x1);
    xf2 = Fun(x2);
P:
    ++k;
    if (xf1 >= xf2)
    {
        a = x1;
        x1 = x2;
        xf1 = xf2;
        x2 = a + (double)F(N - k - 1) / F(N - k) * (b - a) + ((N - k) & 1 ? -1 : 1) * eps / F(N - k);
        xf2 = Fun(x2);
    }
    else
    {
        b = x2;
        x2 = x1;
        xf2 = xf1;
        x1 = a + (double)F(N - k - 2) / F(N - k) * (b - a) - ((N - k) & 1 ? -1 : 1) * eps / F(N - k);
        xf1 = Fun(x1);
    }

    if (fabs(b - a) <= eps)
    {
        _x = (a + b) / 2;
        double result = Fun(_x);
        return result;

    }
    else
        goto P;
}

DWORD WINAPI ThreadFunc(LPVOID client_socket)
{
    double result, a, b;

    SOCKET mySock;
    mySock = ((SOCKET*)client_socket)[0];


    char buff2[2048];
    char* a_end;
    char* b_end;
    recv(mySock, buff2, sizeof(double) * 3 + sizeof(char) * 2 + 1, 0);
    a = strtod(buff2, &a_end);
    b = strtod(a_end, &b_end);

    result = Fib(a, b);

    std::stringstream s;
    s << result;
    std::string str = s.str();
    const char* buff = str.c_str();
    send(mySock, buff, std::strlen(buff) + 1, 0);

    return 0;
}




int main(int argc, char* argv[])
{
    //загрузка версии библиотеки для вызова сетевых функций
    WSAData wsaData;

    //запрашиваемая версия для библиотеки winsock
    WORD DLLVersion = MAKEWORD(2, 1);
    //функция для загрузки библиотеки
    if (WSAStartup(DLLVersion, &wsaData) != 0) {
        std::cout << "Error lib\n";
        exit(1);
    }

    //структура для хранения адреса (для интернет протоколов)
    SOCKADDR_IN server_socket_addres;

    //для хранения ip-адреса
    server_socket_addres.sin_addr.s_addr = 0;
    //для хранения порта
    server_socket_addres.sin_port = htons(5000);
    //указание семейства протоколов - af_inet -сокет интернета
    server_socket_addres.sin_family = AF_INET;

    //создаем сокет (параметры: протокол, тип сокета - потоковый
    server_socket = socket(AF_INET, SOCK_STREAM, 0);

    //связываем сокет с его локальным адресом
    bind(server_socket, (SOCKADDR*)&server_socket_addres, sizeof(server_socket_addres));

    //прослушиваем порт(какой порт + количество клиентов в очереди)
    listen(server_socket, SOMAXCONN);



    //принемаем соединение, создавая новый сокет для клиента
    SOCKET client_socket;
    SOCKADDR_IN client_socket_addres;
    int sizeof_client_socket_addres = sizeof(server_socket_addres);


    while ((client_socket = accept(server_socket, (sockaddr*)&client_socket_addres, &sizeof_client_socket_addres)))
    {
        if (client_socket == 0) {
            std::cout << "Error with client connection\n";
        }
        else {
            std::cout << "Client connected\n";
        }

        QueueUserWorkItem(ThreadFunc, &client_socket, WT_EXECUTEDEFAULT);
    }

    closesocket(server_socket);
    system("pause");
    WSACleanup();
    return(0);
}


