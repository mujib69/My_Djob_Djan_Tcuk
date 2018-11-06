namespace ISBS_New.Utility
{
    partial class Frm_Login
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
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.ChkRemember = new System.Windows.Forms.CheckBox();
            this.mPanel_Password = new MetroFramework.Controls.MetroPanel();
            this.metroPanel7 = new MetroFramework.Controls.MetroPanel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.mPanel_UserID = new MetroFramework.Controls.MetroPanel();
            this.metroPanel6 = new MetroFramework.Controls.MetroPanel();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.mBtnLogin = new MetroFramework.Controls.MetroButton();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.metroPanel2.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.mPanel_Password.SuspendLayout();
            this.mPanel_UserID.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.metroPanel3);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(181, 74);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(320, 170);
            this.metroPanel2.TabIndex = 2;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.ChkRemember);
            this.metroPanel3.Controls.Add(this.mPanel_Password);
            this.metroPanel3.Controls.Add(this.mPanel_UserID);
            this.metroPanel3.Controls.Add(this.mBtnLogin);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(0, 0);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Size = new System.Drawing.Size(320, 170);
            this.metroPanel3.TabIndex = 3;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // ChkRemember
            // 
            this.ChkRemember.AutoSize = true;
            this.ChkRemember.Location = new System.Drawing.Point(7, 99);
            this.ChkRemember.Name = "ChkRemember";
            this.ChkRemember.Size = new System.Drawing.Size(115, 20);
            this.ChkRemember.TabIndex = 10;
            this.ChkRemember.Text = "Remember Me";
            this.ChkRemember.UseVisualStyleBackColor = true;
            // 
            // mPanel_Password
            // 
            this.mPanel_Password.Controls.Add(this.metroPanel7);
            this.mPanel_Password.Controls.Add(this.txtPassword);
            this.mPanel_Password.HorizontalScrollbarBarColor = true;
            this.mPanel_Password.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_Password.HorizontalScrollbarSize = 10;
            this.mPanel_Password.Location = new System.Drawing.Point(6, 52);
            this.mPanel_Password.Name = "mPanel_Password";
            this.mPanel_Password.Size = new System.Drawing.Size(299, 24);
            this.mPanel_Password.TabIndex = 9;
            this.mPanel_Password.VerticalScrollbarBarColor = true;
            this.mPanel_Password.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Password.VerticalScrollbarSize = 10;
            // 
            // metroPanel7
            // 
            this.metroPanel7.BackgroundImage = global::ISBS_New.Properties.Resources.Lock;
            this.metroPanel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.metroPanel7.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroPanel7.HorizontalScrollbarBarColor = true;
            this.metroPanel7.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel7.HorizontalScrollbarSize = 10;
            this.metroPanel7.Location = new System.Drawing.Point(0, 0);
            this.metroPanel7.Name = "metroPanel7";
            this.metroPanel7.Size = new System.Drawing.Size(27, 24);
            this.metroPanel7.TabIndex = 2;
            this.metroPanel7.VerticalScrollbarBarColor = true;
            this.metroPanel7.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel7.VerticalScrollbarSize = 10;
            // 
            // txtPassword
            // 
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtPassword.Location = new System.Drawing.Point(27, 0);
            this.txtPassword.MaxLength = 20;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(272, 22);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.Tag = "test";
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            // 
            // mPanel_UserID
            // 
            this.mPanel_UserID.Controls.Add(this.metroPanel6);
            this.mPanel_UserID.Controls.Add(this.txtUserID);
            this.mPanel_UserID.HorizontalScrollbarBarColor = true;
            this.mPanel_UserID.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_UserID.HorizontalScrollbarSize = 10;
            this.mPanel_UserID.Location = new System.Drawing.Point(6, 21);
            this.mPanel_UserID.Name = "mPanel_UserID";
            this.mPanel_UserID.Size = new System.Drawing.Size(299, 24);
            this.mPanel_UserID.TabIndex = 8;
            this.mPanel_UserID.VerticalScrollbarBarColor = true;
            this.mPanel_UserID.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_UserID.VerticalScrollbarSize = 10;
            // 
            // metroPanel6
            // 
            this.metroPanel6.BackgroundImage = global::ISBS_New.Properties.Resources.User;
            this.metroPanel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.metroPanel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroPanel6.HorizontalScrollbarBarColor = true;
            this.metroPanel6.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel6.HorizontalScrollbarSize = 10;
            this.metroPanel6.Location = new System.Drawing.Point(0, 0);
            this.metroPanel6.Name = "metroPanel6";
            this.metroPanel6.Size = new System.Drawing.Size(27, 24);
            this.metroPanel6.TabIndex = 2;
            this.metroPanel6.VerticalScrollbarBarColor = true;
            this.metroPanel6.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel6.VerticalScrollbarSize = 10;
            // 
            // txtUserID
            // 
            this.txtUserID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUserID.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtUserID.Location = new System.Drawing.Point(27, 0);
            this.txtUserID.MaxLength = 20;
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(272, 22);
            this.txtUserID.TabIndex = 1;
            this.txtUserID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUserID_KeyPress);
            // 
            // mBtnLogin
            // 
            this.mBtnLogin.Location = new System.Drawing.Point(216, 128);
            this.mBtnLogin.Name = "mBtnLogin";
            this.mBtnLogin.Size = new System.Drawing.Size(89, 23);
            this.mBtnLogin.TabIndex = 4;
            this.mBtnLogin.Text = "LOGIN";
            this.mBtnLogin.UseSelectable = true;
            this.mBtnLogin.Click += new System.EventHandler(this.mBtnLogin_Click);
            // 
            // metroPanel1
            // 
            this.metroPanel1.BackgroundImage = global::ISBS_New.Properties.Resources.Login;
            this.metroPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(27, 74);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(154, 170);
            this.metroPanel1.TabIndex = 1;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // Frm_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 269);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.metroPanel1);
            this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Frm_Login";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Green;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Frm_Login_Load);
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel3.PerformLayout();
            this.mPanel_Password.ResumeLayout(false);
            this.mPanel_Password.PerformLayout();
            this.mPanel_UserID.ResumeLayout(false);
            this.mPanel_UserID.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserID;
        private MetroFramework.Controls.MetroButton mBtnLogin;
        private MetroFramework.Controls.MetroPanel mPanel_Password;
        private MetroFramework.Controls.MetroPanel metroPanel7;
        private MetroFramework.Controls.MetroPanel mPanel_UserID;
        private MetroFramework.Controls.MetroPanel metroPanel6;
        private System.Windows.Forms.CheckBox ChkRemember;

    }
}