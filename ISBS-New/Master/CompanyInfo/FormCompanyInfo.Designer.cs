namespace ISBS_New.Master.CompanyInfo
{
    partial class FormCompanyInfo
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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpCompanyInfo = new System.Windows.Forms.GroupBox();
            this.txtSignatoryName = new System.Windows.Forms.TextBox();
            this.txtTaxAddress = new System.Windows.Forms.TextBox();
            this.txtCompanyFax = new System.Windows.Forms.TextBox();
            this.txtCompanyPhone = new System.Windows.Forms.TextBox();
            this.txtCompanyAddress = new System.Windows.Forms.TextBox();
            this.txtTaxName = new System.Windows.Forms.TextBox();
            this.txtCompanyName = new System.Windows.Forms.TextBox();
            this.txtNpwp = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCompanyId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpCompanyInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(519, 260);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 43;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(234, 260);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 42;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(86, 260);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 40;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(392, 260);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 161;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpCompanyInfo
            // 
            this.grpCompanyInfo.Controls.Add(this.txtSignatoryName);
            this.grpCompanyInfo.Controls.Add(this.txtTaxAddress);
            this.grpCompanyInfo.Controls.Add(this.txtCompanyFax);
            this.grpCompanyInfo.Controls.Add(this.txtCompanyPhone);
            this.grpCompanyInfo.Controls.Add(this.txtCompanyAddress);
            this.grpCompanyInfo.Controls.Add(this.txtTaxName);
            this.grpCompanyInfo.Controls.Add(this.txtCompanyName);
            this.grpCompanyInfo.Controls.Add(this.txtNpwp);
            this.grpCompanyInfo.Controls.Add(this.label9);
            this.grpCompanyInfo.Controls.Add(this.label8);
            this.grpCompanyInfo.Controls.Add(this.label7);
            this.grpCompanyInfo.Controls.Add(this.label6);
            this.grpCompanyInfo.Controls.Add(this.label5);
            this.grpCompanyInfo.Controls.Add(this.label4);
            this.grpCompanyInfo.Controls.Add(this.label3);
            this.grpCompanyInfo.Controls.Add(this.label2);
            this.grpCompanyInfo.Controls.Add(this.txtCompanyId);
            this.grpCompanyInfo.Controls.Add(this.label1);
            this.grpCompanyInfo.Location = new System.Drawing.Point(12, 59);
            this.grpCompanyInfo.Name = "grpCompanyInfo";
            this.grpCompanyInfo.Size = new System.Drawing.Size(666, 195);
            this.grpCompanyInfo.TabIndex = 162;
            this.grpCompanyInfo.TabStop = false;
            // 
            // txtSignatoryName
            // 
            this.txtSignatoryName.Location = new System.Drawing.Point(445, 133);
            this.txtSignatoryName.MaxLength = 20;
            this.txtSignatoryName.Name = "txtSignatoryName";
            this.txtSignatoryName.Size = new System.Drawing.Size(202, 20);
            this.txtSignatoryName.TabIndex = 9;
            // 
            // txtTaxAddress
            // 
            this.txtTaxAddress.Location = new System.Drawing.Point(445, 76);
            this.txtTaxAddress.MaxLength = 150;
            this.txtTaxAddress.Multiline = true;
            this.txtTaxAddress.Name = "txtTaxAddress";
            this.txtTaxAddress.Size = new System.Drawing.Size(202, 51);
            this.txtTaxAddress.TabIndex = 8;
            // 
            // txtCompanyFax
            // 
            this.txtCompanyFax.Location = new System.Drawing.Point(133, 160);
            this.txtCompanyFax.MaxLength = 50;
            this.txtCompanyFax.Name = "txtCompanyFax";
            this.txtCompanyFax.Size = new System.Drawing.Size(185, 20);
            this.txtCompanyFax.TabIndex = 5;
            this.txtCompanyFax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCompanyFax_KeyPress);
            // 
            // txtCompanyPhone
            // 
            this.txtCompanyPhone.Location = new System.Drawing.Point(133, 133);
            this.txtCompanyPhone.MaxLength = 50;
            this.txtCompanyPhone.Name = "txtCompanyPhone";
            this.txtCompanyPhone.Size = new System.Drawing.Size(185, 20);
            this.txtCompanyPhone.TabIndex = 4;
            this.txtCompanyPhone.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCompanyPhone_KeyPress);
            // 
            // txtCompanyAddress
            // 
            this.txtCompanyAddress.Location = new System.Drawing.Point(133, 76);
            this.txtCompanyAddress.MaxLength = 150;
            this.txtCompanyAddress.Multiline = true;
            this.txtCompanyAddress.Name = "txtCompanyAddress";
            this.txtCompanyAddress.Size = new System.Drawing.Size(185, 51);
            this.txtCompanyAddress.TabIndex = 3;
            // 
            // txtTaxName
            // 
            this.txtTaxName.Location = new System.Drawing.Point(445, 48);
            this.txtTaxName.MaxLength = 100;
            this.txtTaxName.Name = "txtTaxName";
            this.txtTaxName.Size = new System.Drawing.Size(202, 20);
            this.txtTaxName.TabIndex = 7;
            // 
            // txtCompanyName
            // 
            this.txtCompanyName.Location = new System.Drawing.Point(133, 48);
            this.txtCompanyName.MaxLength = 100;
            this.txtCompanyName.Name = "txtCompanyName";
            this.txtCompanyName.Size = new System.Drawing.Size(185, 20);
            this.txtCompanyName.TabIndex = 2;
            // 
            // txtNpwp
            // 
            this.txtNpwp.Location = new System.Drawing.Point(445, 20);
            this.txtNpwp.MaxLength = 50;
            this.txtNpwp.Name = "txtNpwp";
            this.txtNpwp.Size = new System.Drawing.Size(202, 20);
            this.txtNpwp.TabIndex = 6;
            this.txtNpwp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNpwp_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(346, 136);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Signatory Name";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(346, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Tax Address";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(346, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Tax Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(346, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "NPWP";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Company Fax";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Company Phone";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Company Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Company Name";
            // 
            // txtCompanyId
            // 
            this.txtCompanyId.Location = new System.Drawing.Point(133, 20);
            this.txtCompanyId.MaxLength = 5;
            this.txtCompanyId.Name = "txtCompanyId";
            this.txtCompanyId.Size = new System.Drawing.Size(185, 20);
            this.txtCompanyId.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Company Id";
            // 
            // FormCompanyInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 298);
            this.Controls.Add(this.grpCompanyInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Name = "FormCompanyInfo";
            this.Text = "Form Company Info";
            this.Load += new System.EventHandler(this.FormCompanyInfo_Load);
            this.grpCompanyInfo.ResumeLayout(false);
            this.grpCompanyInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpCompanyInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCompanyId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTaxName;
        private System.Windows.Forms.TextBox txtCompanyName;
        private System.Windows.Forms.TextBox txtNpwp;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSignatoryName;
        private System.Windows.Forms.TextBox txtTaxAddress;
        private System.Windows.Forms.TextBox txtCompanyFax;
        private System.Windows.Forms.TextBox txtCompanyPhone;
        private System.Windows.Forms.TextBox txtCompanyAddress;
    }
}