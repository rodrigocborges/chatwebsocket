using ChatWebSocketServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatWebSocketClient
{
    public partial class MainForm : Form
    {

        private TcpClient _clientSocket = null;
        private bool _connected = false;
        private Thread _receiveThread = null;
        private NetworkStream _networkStream = null;
        private string _localUsername = "";

        public MainForm()
        {
            InitializeComponent();
        }

        //Ao carregar o form principal definir a label de status com o texto padrão
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                statusLabel.Text = "DESCONECTADO";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                statusLabel.Text = "DESCONECTADO";
            }
        }

        //Ao fechar form cancela conexão com o socket
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SendMessage("quit");

                if(_clientSocket != null)
                    _clientSocket.Close();

                if (_networkStream != null)
                    _networkStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Método responsável por ficar lendo o servidor e colocando na tela as mensagens
        private void ChatMessages()
        {
            bool keepAlive = true;

            try
            {
                while (keepAlive)
                {
                    byte[] buffer = new byte[2048];
                    _networkStream.Read(buffer, 0, buffer.Length);

                    string message = Environment.NewLine + Encoding.UTF8.GetString(buffer);

                    //Essa condição serve para mostrar a cor "verde" para quando a mensagem for sua e "ciano" quando for de outros usuários
                    if (message.Split(':')[0].Trim().ToLower().Equals(_localUsername.Trim().ToLower()))
                    {
                        ChatLogAppendText(message, Color.LightGreen);
                    }
                    else
                    {
                        ChatLogAppendText(message, Color.Cyan);
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Método para processar o cliente, ou seja, envia mensagem de boas vindas pro servidor e o servidor envia para todos
        // Após isso, cria um thread para ficar recebendo e mostrando na tela as mensagens
        private void ProcessClient()
        {
            try {
                if (_connected)
                {
                    SendMessage(string.Format("Bem vindo, {0}!", _localUsername));
                    _receiveThread = new Thread(ChatMessages);
                    //Definindo como background para que quando fechar a aplicação termine automaticamente esse thread
                    _receiveThread.IsBackground = true;
                    _receiveThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Botão para enviar mensagem
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (_clientSocket == null)
                    return;

                SendMessage(tbMessage.Text);
                tbMessage.Clear();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Método responsável por enviar o que foi digitado para o servidor
        private void SendMessage(string message)
        {
            try
            {
                if (_networkStream == null)
                    return;

                if (string.IsNullOrEmpty(message))
                    return;

                string usernameAndMessage = string.Format("{0}: {1}", _localUsername, message);

                byte[] dataToSend = Encoding.UTF8.GetBytes(usernameAndMessage);
                _networkStream.Write(dataToSend, 0, dataToSend.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Método responsável por mostrar mensagem na tela e definir uma cor personalizada caso seja solicitado
        private void ChatLogAppendText(string text, Color color)
        {
            if (chatLog.InvokeRequired)
            {
                chatLog.Invoke(new Action(() => {
                    chatLog.SelectionStart = chatLog.TextLength;
                    chatLog.SelectionLength = 0;

                    chatLog.SelectionColor = color;
                    chatLog.AppendText(text + Environment.NewLine);
                    chatLog.SelectionColor = chatLog.ForeColor;
                }));
            }
            else
            {
                chatLog.SelectionStart = chatLog.TextLength;
                chatLog.SelectionLength = 0;

                chatLog.SelectionColor = color;
                chatLog.AppendText(text + Environment.NewLine);
                chatLog.SelectionColor = chatLog.ForeColor;
            }
        }

        //Botão de conectar, inicia conexão com o servidor e processa o cliente
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(tbUsername.Text))
                {
                    MessageBox.Show("Informe um nome de usuário!");
                    return;
                }

                _localUsername = tbUsername.Text;

                //Conecta com o servidor
                _clientSocket = new TcpClient();
                _clientSocket.Connect("127.0.0.1", 9999);

                //Obtem stream da conexão para envio e recimento de dados
                _networkStream = _clientSocket.GetStream();
                _connected = true;
                statusLabel.Text = "CONECTADO";

                ProcessClient();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                statusLabel.Text = "DESCONECTADO";
            }
        }

        //Ao teclar "enter" vai enviar mensagem da mesma forma que ao clicar no botão de enviar
        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                    btnSendMessage_Click(sender, e);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
