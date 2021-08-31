using System;
using System.Collections.Generic;
using System.Text;

namespace ChatWebSocketServer
{
    //Interface responsável por receber a mensagem enviada para o usuário (Observer)
    public interface IUser
    {
        void ReceptMessage();
    }
}
