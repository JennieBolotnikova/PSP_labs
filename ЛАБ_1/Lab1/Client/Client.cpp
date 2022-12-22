#include <iostream>
#include <cmath>
#include <cstdlib>
#include "ClientTCP.h"

double function(double x) {
	return  sin(x) * exp(-x);
}

double methodTrapezoid(double x1, double x2, double y1, double y2) {
	return (y1 + y2) * (x2 - x1) / 2;
}

int main()
{
	const int PORT = 5000;

	std::string message;
	double x1, x2, y1, y2;
	double result = 0;

	ClientTCP client(PORT);
	message = client.receiveMessage(15);

	x1 = atof(message.substr(0, message.find(' ')).c_str());
	x2 = atof(message.substr(message.find(' ') + 1, message.length() - message.find(' ') - 1).c_str());
	y1 = function(x1);
	y2 = function(x2);

	std::cout << "A:    " << x1 << std::endl;
	std::cout << "B:    " << x2 << std::endl;
	std::cout << "F(A): " << y1 << std::endl;
	std::cout << "F(B): " << y2 << std::endl;

	result = methodTrapezoid(x1, x2, y1, y2);

	std::cout << "Square: " << result << std::endl;

	client.sendMessage(std::to_string(result));

	int tmp = getchar();

	return 0;
}