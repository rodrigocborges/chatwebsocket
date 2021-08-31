using System;
using System.Collections.Generic;
using System.Text;

namespace ChatWebSocketServer.Controllers
{
    //Classe responsável por manipular a adição e remoção de um usuário e o envio de mensagem
    public class MessageController : IMessage
    {
        private readonly IList<IUser> _users;

        public MessageController()
        {
            _users = new List<IUser>();
        }

        public void Add(IUser observer)
        {
            _users.Add(observer);
        }

        public void Remove(IUser observer)
        {
            _users.Remove(observer);
        }

        public void SendMessage()
        {
            foreach (IUser user in _users)
                user.ReceptMessage();
        }
    }
}
