using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class Connection
    {
        private Thread _senderThread;
        private TcpClient _user;

        private StreamReader _receiver;
        private StreamWriter _sender;

        private string _currentUser;
        private string _clientAnswer;

        public Connection(TcpClient user)
        {
            _user = user;

            _senderThread = new Thread(ValidateUser);
            _senderThread.Start();
        }

        private void ValidateUser()
        {
            _receiver = new StreamReader(_user.GetStream());
            _sender = new StreamWriter(_user.GetStream());

            _currentUser = _receiver.ReadLine();

            if (Program.Users.Contains(_currentUser))
            {
                _sender.WriteLine("0|Usuário existente.");
                _sender.Flush();
                Close();
                return;
            }

            if (_currentUser == "Administrador")
            {
                _sender.WriteLine("0|Usuário reservado.");
                _sender.Flush();
                Close();
                return;
            }

            _sender.WriteLine("1|Conectado com sucesso.");
            _sender.Flush();

            Program.AddUser(_user, _currentUser);

            try
            {
                while ((_clientAnswer = _receiver.ReadLine()) != null)
                {
                    if (_clientAnswer == null)
                    {
                        Program.RemoveUser(_user);
                    }

                    else
                    {
                        Program.SendPublicMessage(_currentUser, _clientAnswer);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void Close()
        {
            _user.Close();
            _receiver.Close();
            _sender.Close();
        }
    }
}
