using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatWebSocketServer
{
    public class Client
    {
        public string ID { get; private set; }
        public Socket Socket { get; private set; }
        public Thread Thread { get; private set; }

        public Client(Socket socket, Thread thread)
        {
            Socket = socket;
            Thread = thread;
            ID = Guid.NewGuid().ToString();
        }
    }
}
