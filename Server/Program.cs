using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Program
    {
        public static Hashtable Users = new Hashtable(64);
        public static Hashtable Connections = new Hashtable(64);

        private static IPAddress _ipAddress;
        private static TcpClient _server;

        private static TcpListener _tcpListener;
        private static Thread _listenerThread;

        private static StreamWriter _sender;

        private static bool _running;


        private static void Main(string[] args)
        {
            InitializeServer();
            Console.ReadKey();
        }

        private static void InitializeServer()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 2502);
                _tcpListener.Start();

                _running = true;

                _listenerThread = new Thread(KeepRunning);
                _listenerThread.Start();

                Console.WriteLine("\nAguardando conexões...");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void KeepRunning()
        {
            while (_running)
            {
                _server = _tcpListener.AcceptTcpClient();
                var connection = new Connection(_server);
            }
        }

        public static void AddUser(TcpClient client, string username)
        {
            Users.Add(username, client);
            Connections.Add(client, username);

            SendPublicMessage("Server", $"{username} entrou na sala.");
        }

        public static void RemoveUser(TcpClient client)
        {
            if (Connections[_server] != null)
            {
                SendPublicMessage("Server", $"{Connections[client]} saiu da sala.");

                Users.Remove(Connections[client]);
                Connections.Remove(client);
            }
        }

        public static void SendPublicMessage(string source, string message)
        {
            var users = new TcpClient[Users.Count];
            Users.Values.CopyTo(users, 0);

            foreach (var user in users)
            {
                try
                {
                    SendMessage($"> {source}: {message}", user);
                }
                catch
                {
                    //RemoveUsuario(tcpClientes[i]);
                }
            }
        }

        public static void SendPrivateMessage(string source, string destination, string message)
        {
            //    var user = Users.Values.Cast<string>();
            //    var x = user.Where(w => w == usern)

            //    try
            //    {
            //        SendMessage($"> {source}: {message}", user);
            //    }
            //    catch
            //    {
            //        //RemoveUsuario(tcpClientes[i]);
            //    }
        }

        private static void SendMessage(string message, TcpClient user)
        {
            _sender = new StreamWriter(user.GetStream());
            _sender.WriteLine(message);
            _sender.Flush();
            _sender = null;
        }
    }
}
