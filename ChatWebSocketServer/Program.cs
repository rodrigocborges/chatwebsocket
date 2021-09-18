using ChatWebSocketServer.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatWebSocketServer
{
    class Program
    { 
        static void Main(string[] args)
        {
            Console.Title = "Servidor";
            Server server = new Server(9999);
            server.Start();

            ConsoleLog.Write(message: "Pressione 'enter' para fechar o console.", color: ConsoleColor.Blue, type: MessageType.INFO);
            Console.ReadKey();
        }
    }
}
