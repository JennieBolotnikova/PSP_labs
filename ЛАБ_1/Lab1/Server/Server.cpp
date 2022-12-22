#include "ServerTCP.h"

int main()
{
	const int PORT = 5000;
	int clientCount = 0;
	std::string input;
	std::string a;
	std::string b;

	do
	{
		std::cout << "Enter clients count:" << std::endl;
		std::cin >> input;
	} while (input[0] < '0' || input[0] > '9');

	clientCount = stoi(input);

	Server server(PORT, clientCount);

	do
	{
		std::cout << "Enter A:" << std::endl;
		std::cin >> a;
	} while (a[0] == '-');

	do
	{
		std::cout << "Enter B:" << std::endl;
		std::cin >> b;
	} while (b[0] == '-');

	std::thread* threads = server.listenClients(stod(a), stod(b));

	while (server.clientFinished != clientCount);

	std::cout << "Square: " << server.getSquare() << std::endl;

	int tmp = getchar();

	return 0;
}
