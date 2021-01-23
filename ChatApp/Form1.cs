using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ChatApp
{
    public partial class Form1 : Form
    {
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

                socketFd.BeginSend(Encoding.ASCII.GetBytes(this.textBoxLogin.Text), 0, this.textBoxLogin.Text.Length, 0, new AsyncCallback(SendCallback), state);

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
                IPAddress ipaddress = System.Net.IPAddress.Parse("192.168.2.30");

                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(ipaddress, 1024);//Int32.Parse(this.textBoxPort.Text.ToString()));

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
        public Form1()
        {
            InitializeComponent();
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
                    Dns.BeginGetHostEntry("192.168.2.30", new AsyncCallback(GetHostEntryCallback), null);
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
    }
}
