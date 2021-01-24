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
    public partial class Form3 : Form
    {
        private string user;
        private string chosenOne;
        public Form3(string usr, string chosen)
        {
            InitializeComponent();
            this.user = usr;
            this.chosenOne = chosen;
            this.Show();
            this.Activate();
            this.labelUser.Text = this.chosenOne;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Font bold = new Font(richTextBoxMessages.Font, FontStyle.Bold);
            Font regular = new Font(richTextBoxMessages.Font, FontStyle.Regular);
            if (this.richTextBox1.Text.Length > 0)
            {
                this.richTextBoxMessages.AppendText(Environment.NewLine);
                this.richTextBoxMessages.SelectionFont = bold;
                this.richTextBoxMessages.SelectionColor = Color.Blue;
                this.richTextBoxMessages.AppendText(this.user.ToUpper() + ": ");
                this.richTextBoxMessages.SelectionColor = this.richTextBoxMessages.ForeColor;
                this.richTextBoxMessages.SelectionFont = regular;
                this.richTextBoxMessages.AppendText(this.richTextBox1.Text);
                this.richTextBoxMessages.AppendText(Environment.NewLine);
                this.richTextBox1.ResetText();

                this.richTextBoxMessages.SelectionStart = this.richTextBoxMessages.Text.Length;
                this.richTextBoxMessages.ScrollToCaret();
            }
        }
    }
}
