namespace ChatApp
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxFriends = new System.Windows.Forms.ListBox();
            this.labelHello = new System.Windows.Forms.Label();
            this.labelAdd = new System.Windows.Forms.Label();
            this.textBoxAdd = new System.Windows.Forms.TextBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxFriends
            // 
            this.listBoxFriends.BackColor = System.Drawing.SystemColors.HighlightText;
            this.listBoxFriends.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listBoxFriends.ForeColor = System.Drawing.SystemColors.ControlText;
            this.listBoxFriends.FormattingEnabled = true;
            this.listBoxFriends.ItemHeight = 25;
            this.listBoxFriends.Location = new System.Drawing.Point(12, 51);
            this.listBoxFriends.Name = "listBoxFriends";
            this.listBoxFriends.ScrollAlwaysVisible = true;
            this.listBoxFriends.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxFriends.Size = new System.Drawing.Size(426, 379);
            this.listBoxFriends.TabIndex = 0;
            this.listBoxFriends.SelectedIndexChanged += new System.EventHandler(this.listBoxFriends_SelectedIndexChanged);
            // 
            // labelHello
            // 
            this.labelHello.AutoSize = true;
            this.labelHello.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelHello.Location = new System.Drawing.Point(9, 19);
            this.labelHello.Name = "labelHello";
            this.labelHello.Size = new System.Drawing.Size(70, 26);
            this.labelHello.TabIndex = 1;
            this.labelHello.Text = "label3";
            // 
            // labelAdd
            // 
            this.labelAdd.AutoSize = true;
            this.labelAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelAdd.Location = new System.Drawing.Point(9, 456);
            this.labelAdd.Name = "labelAdd";
            this.labelAdd.Size = new System.Drawing.Size(143, 24);
            this.labelAdd.TabIndex = 2;
            this.labelAdd.Text = "Add new friend:";
            // 
            // textBoxAdd
            // 
            this.textBoxAdd.Location = new System.Drawing.Point(12, 485);
            this.textBoxAdd.Name = "textBoxAdd";
            this.textBoxAdd.Size = new System.Drawing.Size(294, 22);
            this.textBoxAdd.TabIndex = 3;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonAdd.Location = new System.Drawing.Point(327, 482);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(111, 29);
            this.buttonAdd.TabIndex = 4;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.buttonLogout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonLogout.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonLogout.Location = new System.Drawing.Point(343, 19);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(95, 26);
            this.buttonLogout.TabIndex = 5;
            this.buttonLogout.Text = "Logout";
            this.buttonLogout.UseVisualStyleBackColor = false;
            this.buttonLogout.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form2
            // 
            this.ClientSize = new System.Drawing.Size(450, 529);
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.textBoxAdd);
            this.Controls.Add(this.labelAdd);
            this.Controls.Add(this.labelHello);
            this.Controls.Add(this.listBoxFriends);
            this.Name = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxFriends;
        private System.Windows.Forms.Label labelHello;
        private System.Windows.Forms.Label labelAdd;
        private System.Windows.Forms.TextBox textBoxAdd;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonLogout;
    }
}