using ChatWebSocketServer.Util;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatWebSocketServer
{
    //Classe responsável por gerenciar o servidor
    public class Server
    {
        private int _port;
        private TcpListener _serverListener = null;

        private List<Client> _connectedClients = null;
        private Thread _clientThread = null;

        //Construtor onde vai criar o objeto socket com as informações do servidor e instanciar a lista de clientes
        public Server(int port)
        {
            _port = port;

            _serverListener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
            _connectedClients = new List<Client>();
        }

        //Método responsável por iniciar o servidor após ter definido as configurações iniciais no construtor
        public void Start()
        {
            try
            {
                if(_serverListener == null)
                {
                    ConsoleLog.Write("TcpListener está nulo!", ConsoleColor.Green, MessageType.ERROR);
                    return;
                }

                _serverListener.Start();

                ConsoleLog.Write("Servidor ligado!", ConsoleColor.Green, MessageType.INFO);

                while (true)
                {
                    //Aceita uma conexão pendente e cria um thread para o cliente atual
                    Socket clientSocket = _serverListener.AcceptSocket();
                    _clientThread = new Thread(ProcessClientConnection);
                    _clientThread.IsBackground = true;
                    _clientThread.Start(clientSocket);
                }
            }
            catch(Exception ex)
            {
                ConsoleLog.Write(ex.Message, ConsoleColor.Green, MessageType.ERROR);
            }
        }

        //Processa conexão do cliente e cria um objeto cliente com essas informações para adicionar em uma lista
        private void ProcessClientConnection(object socket)
        {
            Socket clientSocket = (Socket)socket;
            bool keepAlive = true;

            ConsoleLog.Write(string.Format("Cliente `{0}` conectado!", clientSocket.RemoteEndPoint), ConsoleColor.Yellow, MessageType.INFO);

            Client clientConnected = new Client(clientSocket, _clientThread);
            _connectedClients.Add(clientConnected);

            while (keepAlive)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    //Recebe dados e coloca dentro da variável buffer
                    int bytesReceived = clientSocket.Receive(buffer);

                    if (bytesReceived > 0)
                    {
                        string clientMessage = Encoding.UTF8.GetString(buffer);
                        string[] clientInfo = clientMessage.Split(':');

                        string username = clientInfo[0];
                        string message = clientInfo[1];

                        ConsoleLog.Write(string.Format("Cliente `{0}` enviou: {1}", username, message), ConsoleColor.Green, MessageType.CHAT);

                        Broadcast(clientMessage);

                        if (message.Equals("quit"))
                        {
                            DisconnectClient(clientConnected);
                            keepAlive = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ConsoleLog.Write(ex.Message, ConsoleColor.Green, MessageType.ERROR);
                    DisconnectClient(clientConnected);
                }

            }
        }

        //Método responsável por enviar mensagem para o cliente especificado
        private void SendToClient(Client client, string message)
        {
            try
            {
                //Converte string para um array de bytes
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                client.Socket.Send(buffer, buffer.Length, 0);
            }
            catch (Exception ex)
            {
                ConsoleLog.Write(ex.Message, ConsoleColor.Green, MessageType.ERROR);
                DisconnectClient(client);
            }
        }

        //Método responsável por enviar mensagem para todos os clientes conectados
        private void Broadcast(string message)
        {
            try
            {
                foreach (Client client in _connectedClients)
                {
                    //Se por algum problema o cliente não estiver conectado não tenta enviar a mensagem para ele e passa pro próximo da lista
                    if (!client.Socket.Connected)
                    {
                        DisconnectClient(client);
                        continue;
                    }

                    SendToClient(client, message);
                }
            }
            catch(Exception ex)
            {
                ConsoleLog.Write(ex.Message, ConsoleColor.Green, MessageType.ERROR);
            }
        }

        //Método responsável por desconectar um cliente
        private void DisconnectClient(Client client)
        {
            try
            {
                if (client == null)
                    return;

                client.Socket.Close();
                _connectedClients.Remove(client);
            }
            catch(Exception ex)
            {
                ConsoleLog.Write(ex.Message, ConsoleColor.Green, MessageType.ERROR);
            }
        }

    }
}
