#pragma once
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <string>
#include <winsock2.h>
#include <iostream>

class ClientTCP
{
	public:
		ClientTCP(int port);
		~ClientTCP();
		void sendMessage(std::string message);
		std::string receiveMessage(int size);
	private:
		SOCKET sock;
		WSADATA wsaData;
		LPHOSTENT hostEnt;
		WORD ver;
		int retVal;
};

std::string to_string(char* arr, int size);
