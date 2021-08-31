using System;
using System.Collections.Generic;
using System.Text;

namespace ChatWebSocketServer
{
    //Interface responsável por registrar, remover e enviar a mensagem para os usuários conectados no chat (Observable)
    public interface IMessage
    {
        void Add(IUser observer);
        void Remove(IUser observer);
        void SendMessage();
    }
}
