namespace ISBS_New.Master.BankGroup
{
    partial class BankGroupForm
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
            this.Label3 = new System.Windows.Forms.Label();
            this.txtBankGroupId = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.grpUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(190, 138);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 170;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(271, 138);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 169;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(109, 138);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 168;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(31, 138);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 167;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpUser
            // 
            this.grpUser.Controls.Add(this.txtBankGroupName);
            this.grpUser.Controls.Add(this.Label3);
            this.grpUser.Controls.Add(this.txtBankGroupId);
            this.grpUser.Controls.Add(this.Label1);
            this.grpUser.Location = new System.Drawing.Point(14, 55);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(342, 76);
            this.grpUser.TabIndex = 166;
            this.grpUser.TabStop = false;
            // 
            // txtBankGroupName
            // 
            this.txtBankGroupName.Enabled = false;
            this.txtBankGroupName.Location = new System.Drawing.Point(130, 43);
            this.txtBankGroupName.MaxLength = 50;
            this.txtBankGroupName.Name = "txtBankGroupName";
            this.txtBankGroupName.Size = new System.Drawing.Size(179, 20);
            this.txtBankGroupName.TabIndex = 4;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(18, 47);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(95, 13);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Bank Group Name";
            // 
            // txtBankGroupId
            // 
            this.txtBankGroupId.Enabled = false;
            this.txtBankGroupId.Location = new System.Drawing.Point(130, 15);
            this.txtBankGroupId.MaxLength = 20;
            this.txtBankGroupId.Name = "txtBankGroupId";
            this.txtBankGroupId.Size = new System.Drawing.Size(88, 20);
            this.txtBankGroupId.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(18, 18);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(76, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Bank Group Id";
            // 
            // BankGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 175);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpUser);
            this.Name = "BankGroupForm";
            this.Text = "Bank Group Form";
            this.Load += new System.EventHandler(this.BankGroupForm_Load);
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
        internal System.Windows.Forms.TextBox txtBankGroupName;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox txtBankGroupId;
        internal System.Windows.Forms.Label Label1;
    }
}