namespace ISBS_New.Master.Bank
{
    partial class BankForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpUser = new System.Windows.Forms.GroupBox();
            this.txtBankGroupName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtTxt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAccountNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSearchBank = new System.Windows.Forms.Button();
            this.txtBankGroupId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAccountId = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.grpUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(301, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 175;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(382, 227);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 174;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(222, 227);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 173;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(142, 227);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 172;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpUser
            // 
            this.grpUser.Controls.Add(this.txtBankGroupName);
            this.grpUser.Controls.Add(this.label7);
            this.grpUser.Controls.Add(this.txtTxt);
            this.grpUser.Controls.Add(this.label6);
            this.grpUser.Controls.Add(this.txtName);
            this.grpUser.Controls.Add(this.label5);
            this.grpUser.Controls.Add(this.txtAccountNo);
            this.grpUser.Controls.Add(this.label4);
            this.grpUser.Controls.Add(this.btnSearchBank);
            this.grpUser.Controls.Add(this.txtBankGroupId);
            this.grpUser.Controls.Add(this.label2);
            this.grpUser.Controls.Add(this.txtAccountId);
            this.grpUser.Controls.Add(this.Label3);
            this.grpUser.Location = new System.Drawing.Point(12, 58);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(442, 154);
            this.grpUser.TabIndex = 171;
            this.grpUser.TabStop = false;
            // 
            // txtBankGroupName
            // 
            this.txtBankGroupName.Enabled = false;
            this.txtBankGroupName.Location = new System.Drawing.Point(255, 40);
            this.txtBankGroupName.MaxLength = 50;
            this.txtBankGroupName.Name = "txtBankGroupName";
            this.txtBankGroupName.Size = new System.Drawing.Size(179, 20);
            this.txtBankGroupName.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 13);
            this.label7.TabIndex = 23;
            // 
            // txtTxt
            // 
            this.txtTxt.Enabled = false;
            this.txtTxt.Location = new System.Drawing.Point(131, 117);
            this.txtTxt.MaxLength = 15;
            this.txtTxt.Name = "txtTxt";
            this.txtTxt.Size = new System.Drawing.Size(233, 20);
            this.txtTxt.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Notes";
            // 
            // txtName
            // 
            this.txtName.Enabled = false;
            this.txtName.Location = new System.Drawing.Point(131, 91);
            this.txtName.MaxLength = 150;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(233, 20);
            this.txtName.TabIndex = 18;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Account Name";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Enabled = false;
            this.txtAccountNo.Location = new System.Drawing.Point(130, 65);
            this.txtAccountNo.MaxLength = 25;
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.Size = new System.Drawing.Size(140, 20);
            this.txtAccountNo.TabIndex = 16;
            this.txtAccountNo.TextChanged += new System.EventHandler(this.txtAccountNo_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Account No";
            // 
            // btnSearchBank
            // 
            this.btnSearchBank.Enabled = false;
            this.btnSearchBank.Location = new System.Drawing.Point(213, 38);
            this.btnSearchBank.Name = "btnSearchBank";
            this.btnSearchBank.Size = new System.Drawing.Size(36, 23);
            this.btnSearchBank.TabIndex = 15;
            this.btnSearchBank.Text = "...";
            this.btnSearchBank.UseVisualStyleBackColor = true;
            this.btnSearchBank.Click += new System.EventHandler(this.btnSearchBank_Click);
            // 
            // txtBankGroupId
            // 
            this.txtBankGroupId.Enabled = false;
            this.txtBankGroupId.Location = new System.Drawing.Point(130, 39);
            this.txtBankGroupId.MaxLength = 15;
            this.txtBankGroupId.Name = "txtBankGroupId";
            this.txtBankGroupId.Size = new System.Drawing.Size(80, 20);
            this.txtBankGroupId.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Group Id";
            // 
            // txtAccountId
            // 
            this.txtAccountId.Enabled = false;
            this.txtAccountId.Location = new System.Drawing.Point(130, 13);
            this.txtAccountId.MaxLength = 50;
            this.txtAccountId.Name = "txtAccountId";
            this.txtAccountId.Size = new System.Drawing.Size(80, 20);
            this.txtAccountId.TabIndex = 4;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(18, 16);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(44, 13);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Bank Id";
            // 
            // BankForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 266);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpUser);
            this.Name = "BankForm";
            this.Text = "Bank Form";
            this.Load += new System.EventHandler(this.BankForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BankForm_FormClosed);
            this.grpUser.ResumeLayout(false);
            this.grpUser.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpUser;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox txtBankGroupId;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox txtAccountId;
        internal System.Windows.Forms.TextBox txtAccountNo;
        internal System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button btnSearchBank;
        internal System.Windows.Forms.TextBox txtName;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox txtTxt;
        internal System.Windows.Forms.Label label6;
        internal System.Windows.Forms.TextBox txtBankGroupName;
        internal System.Windows.Forms.Label label7;
    }
}