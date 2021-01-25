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
        private string response2 = string.Empty;
        private string[] friends;
        private readonly string address = string.Empty;
        private readonly int port;
        private int addc = 0;
        private Form form_1;
        private Form3[] chatsArr = Array.Empty<Form3>(); 
        CancellationTokenSource ts = new CancellationTokenSource();

        // ManualResetEvent instances signal completion.  
        private ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public Form2(string usr, string fre, string adr, int port, Form obj)
        {
            InitializeComponent();
            this.Show();
            this.Activate();
            this.form_1 = obj;
            this.address = adr;
            this.port = port;
            this.user = usr;
            this.labelHello.Text = "Hello " + this.user + "!";
            makeFriendsList(fre);
            updateFriendsList();
            CancellationToken ct = this.ts.Token;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(20000);
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                    RefreshForm2("hi.  ");
                }
            }, ct);
        }
        public void RefreshForm2(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(RefreshForm2), new object[] { value });
                return;
            }
            getUpdatedList();
        }
        private void CommWithServer(string toSend, Boolean withReceive)
        {
            // Connect to a remote device.  
            try
            {
                IPAddress ipAddress = IPAddress.Parse(this.address);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, this.port);
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
                        if (this.addc == 0)
                        {
                            this.response = state.m_StringBuilder.ToString();
                        }
                        else if (this.addc == 1)
                        {
                            this.response2 = state.m_StringBuilder.ToString();
                        }
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

        private void getUpdatedList()
        {
            string dat = "log;" + this.user;
            CommWithServer(dat, true);
            this.response = this.response.Substring(4);
            int l = this.response.TakeWhile(b => b != 0).Count();
            string toPass = this.response.Substring(0, l);
            makeFriendsList(toPass);
            updateFriendsList();
        }
        private void makeFriendsList(string fre)
        {
            this.friends = fre.Split(';');
        }
        private void updateFriendsList()
        {
            this.listBoxFriends.Items.Clear();
            if (this.friends != null && this.friends[0] != "")
            {
                foreach (var fr in this.friends)
                {
                    int sep = fr.IndexOf(':');
                    string one = fr.Substring(0, sep);
                    if (fr[fr.Length - 1] == '1')
                    {
                        this.listBoxFriends.Items.Add('*' + one);
                    }
                    else
                    {
                        this.listBoxFriends.Items.Add(one);
                    }
                }
            }
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (this.textBoxAdd.Text.Length > 0 && this.listBoxFriends.FindStringExact(this.textBoxAdd.Text) == ListBox.NoMatches)
            {
                string dat = "add;" + this.user + ";" + this.textBoxAdd.Text;
                this.addc = 1;
                CommWithServer(dat, true);
                if(this.response2[0] == '1')
                {
                    if (this.friends[0] != "")
                    {
                        Array.Resize(ref friends, friends.Length + 1);
                    }
                    this.friends[this.friends.Length - 1] = this.textBoxAdd.Text + ":0";
                }
                this.textBoxAdd.Text = String.Empty;
                this.addc = 0;
                this.response2 = String.Empty;
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
                        if (chosenUser[0] == '*') { chosenUser = chosenUser.Substring(1); }
                        string dat = "get;" + this.user + ";" + chosenUser;
                        CommWithServer(dat, true);
                        int l = this.response.TakeWhile(b => b != 0).Count();
                        string toPass = this.response.Substring(0, l);
                        toPass = toPass.Replace('~', ' ');
                        Form3 chat = new Form3(this.user, chosenUser, toPass, this.address, this.port);
                        chat.Show();
                        chatsArr.Append(chat);
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
            this.ts.Cancel();
            this.Close();
            Application.Restart();
        }
        void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.ts.Cancel();
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
