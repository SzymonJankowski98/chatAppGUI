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
        public Form1()
        {
            InitializeComponent();
        }
        private string Login(string adr, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(adr);
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

            Socket socketFd = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socketFd.Connect(endPoint);

                socketFd.Send(Encoding.ASCII.GetBytes("log;" + this.textBoxLogin.Text), this.textBoxLogin.Text.Length + 4, 0);

                byte[] buffer = new byte[1024];
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
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string friends = Login(this.textBoxAdres.Text, Int32.Parse(this.textBoxPort.Text));
                if (friends.Length > 0)
                {
                    if (friends == ";") { friends = ""; }
                    this.Hide();
                    Form2 f2 = new Form2(this.textBoxLogin.Text, friends, this.textBoxAdres.Text, Int32.Parse(this.textBoxPort.Text));
                    f2.Activate();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
            }
        }
    }
}
