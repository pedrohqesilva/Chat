using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public class Program
    {
        private static string _userName;
        private static bool _connected;

        private static IPAddress _ipAddress;
        private static TcpClient _server;

        private static StreamWriter _sender;
        private static StreamReader _receiver;

        private static Thread _messageThread;

        private static void Main(string[] args)
        {
            GetUsername();
            //GetIpAddress();

            InitializeConnection();

            if (_connected)
            {
                while (_connected)
                {
                    SendMessage();
                }
            }
            else
            {
                CloseConnection();
            }

            Console.ReadKey();
        }

        private static void GetUsername()
        {
            while (true)
            {
                Console.Write("Olá, qual o seu nome? ");
                _userName = Console.ReadLine();

                if (_userName != null)
                {
                    return;

                }
            }
        }

        private static void GetIpAddress()
        {
            while (true)
            {
                Console.Write("Informe o IP do servidor de comunicação: ");
                _ipAddress = IPAddress.Parse(Console.ReadLine());

                if (_userName != null)
                {
                    return;

                }
            }
        }

        private static void InitializeConnection()
        {
            try
            {
                Console.WriteLine("\nEstabelecendo conexão com o servidor...");

                _server = new TcpClient();
                _server.Connect(IPAddress.Parse("127.0.0.1"), 2502);
            }
            catch (Exception e)
            {
                Console.WriteLine("Não foi possível estabelecer uma conexão com o servidor.");
                return;
            }

            Console.WriteLine("Conexão estabelecida.\n");

            _sender = new StreamWriter(_server.GetStream());
            _sender.WriteLine(_userName);
            _sender.Flush();

            _connected = true;

            _messageThread = new Thread(ReceiveMessages);
            _messageThread.Start();
        }

        private static void CloseConnection()
        {
            _connected = false;

            _sender.Close();
            _receiver.Close();
            _server.Close();
        }

        private static void ReceiveMessages()
        {
            _receiver = new StreamReader(_server.GetStream());
            var answer = _receiver.ReadLine();

            if (answer[0] == '1')
            {
                Console.WriteLine("Bem vindo a sala de conversa da Take.");
            }
            else
            {
                Console.WriteLine($"Não foi possível conectar, motivo: {answer[2..]}");
            }

            while (_connected)
            {
                var message = _receiver.ReadLine();
                Console.WriteLine(message);
            }
        }

        private static void SendMessage()
        {
            var message = Console.ReadLine();

            _sender.WriteLine(message);
            _sender.Flush();
        }
    }
}
