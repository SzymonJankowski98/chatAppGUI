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
    public partial class Form2 : Form
    {
        private readonly string user = string.Empty;
        private string response = string.Empty;
        private string[] friends;
        private Socket clientSocket;

        // ManualResetEvent instances signal completion.  
        private ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public Form2(string usr, string fre)
        {
            InitializeComponent();
            this.Show();
            this.Activate();
            this.user = usr;
            this.labelHello.Text = "Hello " + this.user + "!";
            makeFriendsList(fre);
            updateFriendsList();
            StartClient();
        }
        private void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the
                // remote device is "host.contoso.com".  
                IPAddress ipAddress = IPAddress.Parse("192.168.0.74");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 1234);

                // Create a TCP/IP socket.  
                this.clientSocket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                this.clientSocket.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), this.clientSocket);
                connectDone.WaitOne();
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
                this.clientSocket.EndConnect(ar);
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void Receive()
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.m_SocketFd = this.clientSocket;

                // Begin receiving the data from the remote device.  
                this.clientSocket.BeginReceive(state.m_DataBuf, 0, StateObject.BUF_SIZE, 0,
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
                        this.response = state.m_StringBuilder.ToString();
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
        private void Send(String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            this.clientSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), this.clientSocket);
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
        private void makeFriendsList(string fre)
        {
            this.friends = fre.Split(';');
        }

        private void updateFriendsList()
        {
            this.listBoxFriends.Items.Clear();
            if (this.friends != null)
            {
                foreach (var fr in this.friends)
                {
                    this.listBoxFriends.Items.Add(fr);
                }
            }

        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (this.textBoxAdd.Text.Length > 0 && this.listBoxFriends.FindString(this.textBoxAdd.Text) == ListBox.NoMatches)
            {
                if (this.friends[0] != "") 
                { 
                    Array.Resize(ref friends, friends.Length + 1);
                }
                this.friends[this.friends.Length-1] = this.textBoxAdd.Text;
                string dat = "add;" + this.user + ";" + this.textBoxAdd.Text;
                Send(dat);
                sendDone.WaitOne();
                this.textBoxAdd.Text = String.Empty;
                updateFriendsList();
            }
            else
            {
                this.textBoxAdd.Text = String.Empty;
            }
        }

        private void listBoxFriends_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.listBoxFriends.SelectedItem != null)
                {
                    string chosenUser = this.listBoxFriends.SelectedItem.ToString();
                    if (chosenUser.Length > 0)
                    {
                        string dat = "get;" + this.user + ";" + chosenUser;
                        Send(dat);
                        sendDone.WaitOne();

                        Receive();
                        receiveDone.WaitOne();
                        int l = this.response.TakeWhile(b => b != 0).Count();
                        string toPass = this.response.Substring(0, l);
                        new Form3(this.user, chosenUser, toPass);
                        this.response = String.Empty;
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine("NullReferenceException : {0}", nre.ToString());
            }
            this.listBoxFriends.ClearSelected();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Send("out;");
            //sendDone.WaitOne();
            // Release the socket.
            this.clientSocket.Shutdown(SocketShutdown.Both);
            this.clientSocket.Close();
            this.Close();
            Application.Exit();
        }
    }
    public class StateObject
    {
        public const int BUF_SIZE = 1024;
        public byte[] m_DataBuf = new byte[BUF_SIZE];
        public StringBuilder m_StringBuilder = new StringBuilder();
        public Socket m_SocketFd = null;
    }
}
