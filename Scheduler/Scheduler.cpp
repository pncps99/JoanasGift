#include <iostream>
#include <filesystem>
#include <string>
#include <io.h>
#include <fcntl.h>
#include <stdio.h>


int main()
{
	_setmode(_fileno(stdout), _O_U16TEXT);

	std::string path, start_time, start_date, command;

	std::wcout << "Ol� Joana! Antes de mais, muitos parab�ns, n�o � todos os dias que se fazem 13 anos (uau!)\n";
	std::wcout << "Espero que d�s o devido uso a esta prenda, e que gostes tamb�m. Beijoca :)\n";
	std::wcout << "--------------------------------------\n";
	std::wcout << "Instru��es:\n" << "O seguinte programa serve apenas para definires as horas e o dia a que queres\n";
	std::wcout << "que o bot (o programa que efetua a compra) corra. N�o � necess�rio interagires com o outro programa,\n";
	std::wcout << "apenas com este!\n";
	std::wcout << "Quando te pedir, insere a hora e dia que queres que a compra seja efetuada, e � isso!\n";
	std::wcout << "--------------------------------------\n";

	std::wcout << "Data (dd/mm/yyyy)? ";
	std::cin >> start_date;

	std::wcout << "Hora (HH:mm)? ";
	std::cin >> start_time;

	command = "schtasks /Create /SC ONCE /TN shopbot /TR \"%cd%/run.bat\" /ST " + start_time + " /SD " + start_date;
	
	system(command.c_str());

	std::string dump;

	std::wcout << "Feito! O bot foi agendado para dia " << start_date.c_str() << " �s " << start_time.c_str() << "!\n";
	std::wcout << "Presiona \"q\" + ENTER para fechar...";
	
	std::cin >> dump;

	return 0;
}