namespace ISBS_New.AccessRight
{
    partial class UserProfile
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
            this.components = new System.ComponentModel.Container();
            this.txtAtasanKerja = new System.Windows.Forms.TextBox();
            this.lblAtasanKerja = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtUserPassOld = new System.Windows.Forms.TextBox();
            this.lblOldUserPass = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.lblUserID = new System.Windows.Forms.Label();
            this.grpChangeUserPass = new System.Windows.Forms.GroupBox();
            this.txtUserPassConfirm = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserPassNew = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpChangeEmailPass = new System.Windows.Forms.GroupBox();
            this.txtEmailPassConfirm = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEmailPassNew = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEmailPassOld = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUserEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.btnSaveUserID = new System.Windows.Forms.Button();
            this.btnCancelUserID = new System.Windows.Forms.Button();
            this.btnEditUserID = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.grpChangeUserPass.SuspendLayout();
            this.grpChangeEmailPass.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtAtasanKerja
            // 
            this.txtAtasanKerja.Location = new System.Drawing.Point(123, 113);
            this.txtAtasanKerja.MaxLength = 100;
            this.txtAtasanKerja.Name = "txtAtasanKerja";
            this.txtAtasanKerja.Size = new System.Drawing.Size(230, 20);
            this.txtAtasanKerja.TabIndex = 15;
            // 
            // lblAtasanKerja
            // 
            this.lblAtasanKerja.AutoSize = true;
            this.lblAtasanKerja.Location = new System.Drawing.Point(27, 114);
            this.lblAtasanKerja.Name = "lblAtasanKerja";
            this.lblAtasanKerja.Size = new System.Drawing.Size(67, 13);
            this.lblAtasanKerja.TabIndex = 14;
            this.lblAtasanKerja.Text = "Atasan Kerja";
            // 
            // txtFullName
            // 
            this.txtFullName.Location = new System.Drawing.Point(123, 89);
            this.txtFullName.MaxLength = 150;
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(230, 20);
            this.txtFullName.TabIndex = 13;
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(27, 89);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(54, 13);
            this.lblFullName.TabIndex = 12;
            this.lblFullName.Text = "Full Name";
            // 
            // txtUserPassOld
            // 
            this.txtUserPassOld.Location = new System.Drawing.Point(99, 28);
            this.txtUserPassOld.MaxLength = 20;
            this.txtUserPassOld.Name = "txtUserPassOld";
            this.txtUserPassOld.PasswordChar = '*';
            this.txtUserPassOld.Size = new System.Drawing.Size(230, 20);
            this.txtUserPassOld.TabIndex = 11;
            // 
            // lblOldUserPass
            // 
            this.lblOldUserPass.AutoSize = true;
            this.lblOldUserPass.Location = new System.Drawing.Point(3, 28);
            this.lblOldUserPass.Name = "lblOldUserPass";
            this.lblOldUserPass.Size = new System.Drawing.Size(72, 13);
            this.lblOldUserPass.TabIndex = 10;
            this.lblOldUserPass.Text = "Old Password";
            // 
            // txtUserID
            // 
            this.txtUserID.Location = new System.Drawing.Point(123, 63);
            this.txtUserID.MaxLength = 20;
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(230, 20);
            this.txtUserID.TabIndex = 9;
            // 
            // lblUserID
            // 
            this.lblUserID.AutoSize = true;
            this.lblUserID.Location = new System.Drawing.Point(27, 63);
            this.lblUserID.Name = "lblUserID";
            this.lblUserID.Size = new System.Drawing.Size(43, 13);
            this.lblUserID.TabIndex = 8;
            this.lblUserID.Text = "User ID";
            // 
            // grpChangeUserPass
            // 
            this.grpChangeUserPass.Controls.Add(this.txtUserPassConfirm);
            this.grpChangeUserPass.Controls.Add(this.label2);
            this.grpChangeUserPass.Controls.Add(this.txtUserPassNew);
            this.grpChangeUserPass.Controls.Add(this.label1);
            this.grpChangeUserPass.Controls.Add(this.txtUserPassOld);
            this.grpChangeUserPass.Controls.Add(this.lblOldUserPass);
            this.grpChangeUserPass.Location = new System.Drawing.Point(24, 171);
            this.grpChangeUserPass.Name = "grpChangeUserPass";
            this.grpChangeUserPass.Size = new System.Drawing.Size(347, 113);
            this.grpChangeUserPass.TabIndex = 16;
            this.grpChangeUserPass.TabStop = false;
            this.grpChangeUserPass.Text = "Change User Password";
            // 
            // txtUserPassConfirm
            // 
            this.txtUserPassConfirm.Location = new System.Drawing.Point(99, 80);
            this.txtUserPassConfirm.MaxLength = 20;
            this.txtUserPassConfirm.Name = "txtUserPassConfirm";
            this.txtUserPassConfirm.PasswordChar = '*';
            this.txtUserPassConfirm.Size = new System.Drawing.Size(230, 20);
            this.txtUserPassConfirm.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Confirm Password";
            // 
            // txtUserPassNew
            // 
            this.txtUserPassNew.Location = new System.Drawing.Point(99, 54);
            this.txtUserPassNew.MaxLength = 20;
            this.txtUserPassNew.Name = "txtUserPassNew";
            this.txtUserPassNew.PasswordChar = '*';
            this.txtUserPassNew.Size = new System.Drawing.Size(230, 20);
            this.txtUserPassNew.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "New Password";
            // 
            // grpChangeEmailPass
            // 
            this.grpChangeEmailPass.Controls.Add(this.txtEmailPassConfirm);
            this.grpChangeEmailPass.Controls.Add(this.label5);
            this.grpChangeEmailPass.Controls.Add(this.txtEmailPassNew);
            this.grpChangeEmailPass.Controls.Add(this.label4);
            this.grpChangeEmailPass.Controls.Add(this.txtEmailPassOld);
            this.grpChangeEmailPass.Controls.Add(this.label3);
            this.grpChangeEmailPass.Location = new System.Drawing.Point(377, 171);
            this.grpChangeEmailPass.Name = "grpChangeEmailPass";
            this.grpChangeEmailPass.Size = new System.Drawing.Size(347, 113);
            this.grpChangeEmailPass.TabIndex = 17;
            this.grpChangeEmailPass.TabStop = false;
            this.grpChangeEmailPass.Text = "Change Email Password";
            // 
            // txtEmailPassConfirm
            // 
            this.txtEmailPassConfirm.Location = new System.Drawing.Point(114, 77);
            this.txtEmailPassConfirm.MaxLength = 20;
            this.txtEmailPassConfirm.Name = "txtEmailPassConfirm";
            this.txtEmailPassConfirm.PasswordChar = '*';
            this.txtEmailPassConfirm.Size = new System.Drawing.Size(227, 20);
            this.txtEmailPassConfirm.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Confirm Password";
            // 
            // txtEmailPassNew
            // 
            this.txtEmailPassNew.Location = new System.Drawing.Point(114, 51);
            this.txtEmailPassNew.MaxLength = 20;
            this.txtEmailPassNew.Name = "txtEmailPassNew";
            this.txtEmailPassNew.PasswordChar = '*';
            this.txtEmailPassNew.Size = new System.Drawing.Size(227, 20);
            this.txtEmailPassNew.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "New Password";
            // 
            // txtEmailPassOld
            // 
            this.txtEmailPassOld.Location = new System.Drawing.Point(114, 25);
            this.txtEmailPassOld.MaxLength = 20;
            this.txtEmailPassOld.Name = "txtEmailPassOld";
            this.txtEmailPassOld.PasswordChar = '*';
            this.txtEmailPassOld.Size = new System.Drawing.Size(227, 20);
            this.txtEmailPassOld.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Old Password";
            // 
            // txtUserEmail
            // 
            this.txtUserEmail.Location = new System.Drawing.Point(123, 139);
            this.txtUserEmail.MaxLength = 100;
            this.txtUserEmail.Name = "txtUserEmail";
            this.txtUserEmail.Size = new System.Drawing.Size(230, 20);
            this.txtUserEmail.TabIndex = 19;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(27, 140);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(32, 13);
            this.lblEmail.TabIndex = 18;
            this.lblEmail.Text = "Email";
            // 
            // btnSaveUserID
            // 
            this.btnSaveUserID.Location = new System.Drawing.Point(129, 290);
            this.btnSaveUserID.Name = "btnSaveUserID";
            this.btnSaveUserID.Size = new System.Drawing.Size(54, 23);
            this.btnSaveUserID.TabIndex = 150;
            this.btnSaveUserID.Text = "Save";
            this.btnSaveUserID.UseVisualStyleBackColor = true;
            this.btnSaveUserID.Click += new System.EventHandler(this.btnSaveUserID_Click);
            // 
            // btnCancelUserID
            // 
            this.btnCancelUserID.Location = new System.Drawing.Point(77, 290);
            this.btnCancelUserID.Name = "btnCancelUserID";
            this.btnCancelUserID.Size = new System.Drawing.Size(50, 23);
            this.btnCancelUserID.TabIndex = 149;
            this.btnCancelUserID.Text = "Cancel";
            this.btnCancelUserID.UseVisualStyleBackColor = true;
            this.btnCancelUserID.Click += new System.EventHandler(this.btnCancelUserID_Click);
            // 
            // btnEditUserID
            // 
            this.btnEditUserID.Location = new System.Drawing.Point(27, 290);
            this.btnEditUserID.Name = "btnEditUserID";
            this.btnEditUserID.Size = new System.Drawing.Size(48, 23);
            this.btnEditUserID.TabIndex = 148;
            this.btnEditUserID.Text = "Edit";
            this.btnEditUserID.UseVisualStyleBackColor = true;
            this.btnEditUserID.Click += new System.EventHandler(this.btnEditUserID_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(189, 290);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(50, 23);
            this.btnExit.TabIndex = 155;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // UserProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 323);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSaveUserID);
            this.Controls.Add(this.btnCancelUserID);
            this.Controls.Add(this.btnEditUserID);
            this.Controls.Add(this.txtUserEmail);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.grpChangeEmailPass);
            this.Controls.Add(this.grpChangeUserPass);
            this.Controls.Add(this.txtAtasanKerja);
            this.Controls.Add(this.lblAtasanKerja);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.lblFullName);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.lblUserID);
            this.Name = "UserProfile";
            this.Resizable = false;
            this.Text = "UserProfile";
            this.Load += new System.EventHandler(this.UserProfile_Load);
            this.Shown += new System.EventHandler(this.UserProfile_Shown);
            this.grpChangeUserPass.ResumeLayout(false);
            this.grpChangeUserPass.PerformLayout();
            this.grpChangeEmailPass.ResumeLayout(false);
            this.grpChangeEmailPass.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAtasanKerja;
        private System.Windows.Forms.Label lblAtasanKerja;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtUserPassOld;
        private System.Windows.Forms.Label lblOldUserPass;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.Label lblUserID;
        private System.Windows.Forms.GroupBox grpChangeUserPass;
        private System.Windows.Forms.GroupBox grpChangeEmailPass;
        private System.Windows.Forms.TextBox txtUserEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtUserPassConfirm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUserPassNew;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEmailPassConfirm;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtEmailPassNew;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEmailPassOld;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSaveUserID;
        private System.Windows.Forms.Button btnCancelUserID;
        private System.Windows.Forms.Button btnEditUserID;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Timer timer1;
    }
}