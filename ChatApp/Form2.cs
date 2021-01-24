using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatApp
{
    public partial class Form2 : Form
    {
        string user;
        string[] friends;
        
        public Form2(string usr, string fre)
        {
            InitializeComponent();
            this.Show();
            this.Activate();
            this.user = usr;
            this.labelHello.Text = "Hello " + this.user + "!";
            makeFriendsList(fre);
            updateFriendsList();
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (this.textBoxAdd.Text.Length > 0)
            {
                if (this.friends[0] != "") 
                { 
                    Array.Resize(ref friends, friends.Length + 1);
                }
                this.friends[this.friends.Length-1] = this.textBoxAdd.Text;
                this.textBoxAdd.Text = "";
                updateFriendsList();
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
                        Form3 f3 = new Form3(this.user, chosenUser);
                        f3.Activate();
                    }
                }
            }
            catch (NullReferenceException nre)
            {
                Console.WriteLine("NullReferenceException : {0}", nre.ToString());
            }
        }
    }
}
