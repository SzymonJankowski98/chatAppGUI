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
    public partial class Form1 : Form
    {
        private int ready = 0;
        private Form obj;

        public Form1()
        {
            InitializeComponent();
            this.obj = this;
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* send data */
                int size = socketFd.EndSend(ar);
                if (size > 0)
                {
                    //setThreadedStatusLabel("Index number sent.");
                    //setThreadedButton(true);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                //setThreadedStatusLabel("Check \"Server Info\" and try again!");
                //setThreadedButton(true);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndReceive(ar);


                if (size > 0)
                {
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                    /* get the rest of the data */
                    socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    /* all the data has arrived */
                    if (state.m_StringBuilder.Length > 1)
                    {
                        Console.Out.Write(state.m_StringBuilder);
                        //setThreadedTextBox(state.m_StringBuilder.ToString());
                        //setThreadedStatusLabel("Done.");
                        //setThreadedButton(true);

                        /* shutdown and close socket */
                        
                        socketFd.Shutdown(SocketShutdown.Both);
                        socketFd.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                //setThreadedStatusLabel("Check \"Server Info\" and try again!");
                //setThreadedButton(true);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the socket from the state object */
                Socket socketFd = (Socket)ar.AsyncState;

                /* complete the connection */
                socketFd.EndConnect(ar);

                /* create the SocketStateObject */
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                //setThreadedStatusLabel("Wait! Sending...");

                socketFd.BeginSend(Encoding.ASCII.GetBytes("log" + this.textBoxLogin.Text), 0, this.textBoxLogin.Text.Length, 0, new AsyncCallback(SendCallback), state);

                //setThreadedStatusLabel("Wait! Reading...");

                /* begin receiving the data */
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                //setThreadedStatusLabel("Check \"Server Info\" and try again!");
                //setThreadedButton(true);
            }
        }

        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                Socket socketFd = null;
                IPEndPoint endPoint = null;

                /* complete the DNS query */
                //hostEntry = Dns.EndGetHostEntry(ar);
                //addresses = hostEntry.AddressList;
                IPAddress ipaddress = System.Net.IPAddress.Parse("192.168.0.74");

                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(ipaddress, 1234);//Int32.Parse(this.textBoxPort.Text.ToString()));

                //setThreadedStatusLabel("Wait! Connecting...");

                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                //setThreadedStatusLabel("Check \"Server Info\" and try again!");
                //setThreadedButton(true);
            }
        }
        private string Login()
        {
            //IPHostEntry ipHostInfo = Dns.GetHostEntry("192.168.0.74");
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("192.168.0.74");
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 1234);

            Socket socketFd = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socketFd.Connect(endPoint);

                socketFd.Send(Encoding.ASCII.GetBytes("log;" + this.textBoxLogin.Text), this.textBoxLogin.Text.Length + 4, 0);

                byte[] buffer = new byte[1024];
                // Receive the response from the remote device.
                int howManyBytes = 0;
                int msg = 1; 
                while (msg > 0)
                {
                    int bytesRec = socketFd.Receive(buffer, howManyBytes, buffer.Length - howManyBytes, 0);
                    howManyBytes += bytesRec;
                    if (bytesRec < 1)
                    {
                        msg = 0;
                    }
                }
                // Release the socket.  
                socketFd.Shutdown(SocketShutdown.Both);
                socketFd.Close();
                byte[] friends = new byte[howManyBytes];
                if (string.Compare(Encoding.UTF8.GetString(buffer), 0, "lst;", 0, 4) == 0)
                {
                    Array.Copy(buffer, 4, friends, 0, howManyBytes - 4);
                    int l = friends.TakeWhile(b => b != 0).Count();
                    return Encoding.UTF8.GetString(friends, 0, l);
                }
                else { return ";"; }
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            return "";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //setThreadedButton(false);
                //setThreadedTextBox("");
                //setThreadedStatusLabel("Wait! DNS query...");

                if (true)//this.textBoxAddr.Text.Length > 0 && this.textBoxPort.Text.Length > 0 && this.textBoxIndex.Text.Length > 0)
                {
                    /* get DNS host information */
                    //Dns.BeginGetHostEntry("192.168.0.74", new AsyncCallback(GetHostEntryCallback), null);
                    string friends = Login();
                    if (friends.Length > 0)
                    {
                        if (friends == ";") { friends = ""; }
                        this.Hide();
                        Form2 f2 = new Form2(this.textBoxLogin.Text, friends);
                        f2.Activate();
                    }
                }
                else
                {
                    //if (this.textBoxAddr.Text.Length <= 0) MessageBox.Show("No server address!");
                    //else if (this.textBoxPort.Text.Length <= 0) MessageBox.Show("No server port number!");
                    //else if (this.textBoxIndex.Text.Length <= 0) MessageBox.Show("No index number!");
                    //setThreadedButton(true);
                    //setThreadedStatusLabel("Check \"Server Info\" and try again!");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                //setThreadedStatusLabel("Check \"Server Info\" and try again!");
                //setThreadedButton(true);
            }
        }
    }

    public class SocketStateObject
    {
        public const int BUF_SIZE = 1024;
        public byte[] m_DataBuf = new byte[BUF_SIZE];
        public StringBuilder m_StringBuilder = new StringBuilder();
        public Socket m_SocketFd = null;
        public int ready = 0;
    }
}
