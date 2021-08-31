using System;
using System.Collections.Generic;
using System.Text;

namespace ChatWebSocketServer.Util
{
    public enum MessageType
    {
        INFO,
        ERROR,
        WARNING
    }
    //Classe de utilidade para escrever coisas na tela de forma simples e personalizável
    public static class ConsoleLog
    {
        public static void Write(string message, ConsoleColor color, MessageType type)
        {
            ConsoleColor typeColor = ConsoleColor.Blue;
            switch (type)
            {
                case MessageType.ERROR:
                    typeColor = ConsoleColor.Red;
                    break;
                case MessageType.INFO:
                    typeColor = ConsoleColor.Blue;
                    break;
                case MessageType.WARNING:
                    typeColor = ConsoleColor.DarkYellow;
                    break;
            }

            Console.ForegroundColor = typeColor;
            string text = type.ToString();
            text += ":";
            Console.Write(text);
            Console.ForegroundColor = color;
            text = " " + message;
            Console.Write(text + "\n");            
        }
    }
}
