using ChatWebSocketServer.Controllers;
using ChatWebSocketServer.Models;
using System;

namespace ChatWebSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IMessage message = new MessageController();

            if (message == null)
                return;

            //Cadastra usuários
            IUser user1 = new User(username: "Rodrigo");
            IUser user2 = new User(username: "Zé");

            //Adiciona os dois usuários para poder receber as mensagens
            message.Add(observer: user1);
            message.Add(observer: user2);

            //Envia mensagem
            message.SendMessage();

            Console.ReadKey();
        }
    }
}
