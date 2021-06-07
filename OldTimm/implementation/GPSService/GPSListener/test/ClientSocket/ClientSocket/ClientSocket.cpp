// ClientSocket.cpp : main project file.
#include "stdafx.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Threading;
using namespace System::Text;
using namespace System::IO;
using namespace System::Configuration;

Socket^ ConnectSocket( String^ server, int port )
{
	Socket^ s = nullptr;
	IPHostEntry^ hostEntry = nullptr;
	IPAddress^ address = nullptr;
	// Get host related information.
	try
	{
		if (! IPAddress::TryParse(server, address))
		{
			hostEntry = Dns::Resolve( server );
			address = nullptr;
			// Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
			// an exception that occurs when the host IP Address is not compatible with the address family
			// (typical in the IPv6 case).
			IEnumerator^ myEnum = hostEntry->AddressList->GetEnumerator();
			while ( myEnum->MoveNext() )
			{
				address = safe_cast<IPAddress^>(myEnum->Current);
				IPEndPoint^ endPoint = gcnew IPEndPoint( address,port );
				Socket^ tmpS = gcnew Socket( endPoint->AddressFamily,SocketType::Stream,ProtocolType::Tcp );
				tmpS->Connect( endPoint );
				if ( tmpS->Connected )
				{
					s = tmpS;
					break;
				}
				else
				{
					continue;
				}
			}
		}
		else
		{
			IPEndPoint^ endPoint = gcnew IPEndPoint( address,port );
			Socket^ tmpS = gcnew Socket( endPoint->AddressFamily,SocketType::Stream,ProtocolType::Tcp );
			tmpS->Connect( endPoint );
			if ( tmpS->Connected )
			{
				s = tmpS;
			}
		}
	}
	catch(...)
	{
	}
	return s;
}

// This method requests the home page content for the specified server.
String^ SocketSendReceive( Socket^ s, String^ request)
{
   array<Byte>^bytesSent = Encoding::ASCII->GetBytes( request );
   array<Byte>^bytesReceived = gcnew array<Byte>(1024);
   int nLength = bytesSent->Length;
   Array::Resize(bytesSent,nLength +1) ;
   bytesSent[nLength] = '\0';

   // Send request to the server.
   s->Send( bytesSent, bytesSent->Length, static_cast<SocketFlags>(0) );

   // Receive the server home page content.
   int bytes = 0;
   String^ strRetPage = "";
   do 
   {
	   if( s->Available)
	   {
		bytes = s->Receive( bytesReceived);
	   }
	   strRetPage = String::Concat( strRetPage, Encoding::ASCII->GetString( bytesReceived, 0, bytes ) );
   }while( s->Available > 0);

   return strRetPage;
}



int main()
{
	array<String^>^args = Environment::GetCommandLineArgs();
	int port;
	int nInterval;
	bool isLoop = false;
	String^ sExcutableName = Path::GetFileNameWithoutExtension(args[0]);  

/*	Stream^ logFile = File::Create( sExcutableName + ".log" );
	System::Diagnostics::TextWriterTraceListener^ Listener =  gcnew System::Diagnostics::TextWriterTraceListener(logFile);
	System::Diagnostics::Trace::Listeners->Add(Listener);
*/
	String^ host = ConfigurationManager::AppSettings["host"];
	try
	{
		port = int::Parse(ConfigurationManager::AppSettings["port"]);
		nInterval = int::Parse(ConfigurationManager::AppSettings["interval"]) * 1000;
		isLoop = Boolean::Parse(ConfigurationManager::AppSettings["IsLoop"]);
	}
	catch (...)
	{
		Console::WriteLine(L"[port] [interval]  must be numbers");
		return -1;
	}
	// Create a socket connection with the specified server and port.
	Socket^ s = ConnectSocket( host, port );
	if ( s == nullptr )
	{
		Console::WriteLine(L"Connection to {0} {1} failed ", host, port );
		return -1;
	}

	FileStream^ fileStream = nullptr;
	StreamReader^ streamReader = nullptr;
	do
	{
		try
		{
			fileStream = gcnew FileStream("pos.txt", FileMode::Open);
			streamReader = gcnew StreamReader(fileStream);
			while (true)
			{
				String^ line = streamReader->ReadLine();
				if (String::IsNullOrEmpty(line))
					break;
				SocketSendReceive(s, line);
				Console::WriteLine(L"Sending to  {0}  ", line );
				Thread::Sleep(nInterval); 

			}
		}
		catch (...)
		{
			Console::WriteLine(L"Failed to reading pos.txt");
			return -1;
		}
		finally
		{
			if (streamReader != nullptr)
				streamReader->Close();
			if (fileStream != nullptr)
				fileStream->Close();
		}
	}
	while(isLoop);

	System::Diagnostics::Trace::Flush(); 
	return 0;
}
