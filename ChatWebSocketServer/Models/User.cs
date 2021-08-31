using ChatWebSocketServer.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatWebSocketServer.Models
{
    public class User : IUser
    {
        private string _username;

        public User(string username)
        {
            _username = username;
        }

        public void ReceptMessage()
        {
            ConsoleLog.Write(message:string.Format("{0} recebeu a mensagem!", _username), 
                            color: ConsoleColor.Green, 
                            type: MessageType.INFO);
        }
    }
}
