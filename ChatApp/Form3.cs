using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace ChatApp
{
    public partial class Form3 : Form
    {
        private string user;
        private string chosenOne;
        private string data;
        private string answer = string.Empty;
        private int dataLength;

        // ManualResetEvent instances signal completion.  
        private ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public Form3(string usr, string chosen, string msgs)
        {
            InitializeComponent();
            this.user = usr;
            this.chosenOne = chosen;
            this.data = msgs;
            this.dataLength = this.data.Length;
            this.Show();
            this.Activate();
            this.labelUser.Text = this.chosenOne;
            PrintMessages();

        }
        private void PrintMessages()
        {
            Font bold = new Font(richTextBoxMessages.Font, FontStyle.Bold);
            Font regular = new Font(richTextBoxMessages.Font, FontStyle.Regular);
            var msges = this.data.Split(';');
            foreach (var part in msges)
            {
                if (part != "")
                {
                    var sep = part.IndexOf(':');
                    if (this.richTextBoxMessages.Text.Length > 0)
                    {
                        this.richTextBoxMessages.AppendText(Environment.NewLine);
                    }
                    this.richTextBoxMessages.SelectionFont = bold;
                    this.richTextBoxMessages.SelectionColor = Color.Blue;
                    this.richTextBoxMessages.AppendText(part.Substring(0, sep).ToUpper() + ": ");
                    this.richTextBoxMessages.SelectionColor = this.richTextBoxMessages.ForeColor;
                    this.richTextBoxMessages.SelectionFont = regular;
                    this.richTextBoxMessages.AppendText(part.Substring(sep + 1));
                    this.richTextBox1.ResetText();
                }
            }

            this.richTextBoxMessages.SelectionStart = this.richTextBoxMessages.Text.Length;
            this.richTextBoxMessages.ScrollToCaret();
        }
        private void UpdateMessages()
        {
            string dat = "get;" + this.user + ";" + this.chosenOne;
            CommWithServer(dat, true);
            int l = this.answer.TakeWhile(b => b != 0).Count();
            string newAnswer = this.answer.Substring(0, l);
            if (newAnswer.Length > this.dataLength)
            {
                this.data = newAnswer;
                this.dataLength = newAnswer.Length; 
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Font bold = new Font(richTextBoxMessages.Font, FontStyle.Bold);
            Font regular = new Font(richTextBoxMessages.Font, FontStyle.Regular);
            if (this.richTextBox1.Text.Length > 0)
            {
                if (this.richTextBoxMessages.Text.Length > 0)
                {
                    this.richTextBoxMessages.AppendText(Environment.NewLine);
                }
                this.richTextBoxMessages.SelectionFont = bold;
                this.richTextBoxMessages.SelectionColor = Color.Blue;
                this.richTextBoxMessages.AppendText(this.user.ToUpper() + ": ");
                this.richTextBoxMessages.SelectionColor = this.richTextBoxMessages.ForeColor;
                this.richTextBoxMessages.SelectionFont = regular;
                this.richTextBoxMessages.AppendText(this.richTextBox1.Text);

                string dat = "snd;" + this.user + ";" + this.chosenOne + ";" + this.richTextBox1.Text;
                CommWithServer(dat, false);

                this.richTextBoxMessages.SelectionStart = this.richTextBoxMessages.Text.Length;
                this.richTextBoxMessages.ScrollToCaret();
                this.dataLength += this.richTextBox1.Text.Length + this.user.Length + 1;
                this.richTextBox1.ResetText();
            }
        }

        private void CommWithServer(string toSend, Boolean withReceive)
        {
            // Connect to a remote device.  
            try
            {
                IPAddress ipAddress = IPAddress.Parse("192.168.0.74");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 1234);
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                Send(client, toSend);
                sendDone.WaitOne();

                if (withReceive)
                {
                    Receive(client);
                    receiveDone.WaitOne();
                }
                client.Shutdown(SocketShutdown.Both);
                client.Close();

                this.connectDone.Reset();
                this.sendDone.Reset();
                this.receiveDone.Reset();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.m_SocketFd = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.m_DataBuf, 0, StateObject.BUF_SIZE, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.m_SocketFd;
                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.m_DataBuf, 0, StateObject.BUF_SIZE, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.m_StringBuilder.Length > 1)
                    {
                        this.answer = state.m_StringBuilder.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
