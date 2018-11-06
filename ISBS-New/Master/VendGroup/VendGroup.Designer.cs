namespace ISBS_New.Master.VendGroup
{
    partial class VendGroup
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
            this.txtVendGroupId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVendGroupName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtKursOrigin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDepositAmount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDepositCur = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDPAmount = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtDPCur = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCreditLimitPO = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCreditLimit = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCreditCur = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSearchDepositCur = new System.Windows.Forms.Button();
            this.btnSearchDPCur = new System.Windows.Forms.Button();
            this.btnSearchCreditCur = new System.Windows.Forms.Button();
            this.btnSearchKurs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtVendGroupId
            // 
            this.txtVendGroupId.Enabled = false;
            this.txtVendGroupId.Location = new System.Drawing.Point(175, 12);
            this.txtVendGroupId.MaxLength = 5;
            this.txtVendGroupId.Name = "txtVendGroupId";
            this.txtVendGroupId.Size = new System.Drawing.Size(80, 20);
            this.txtVendGroupId.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 93;
            this.label1.Text = "Vendor Group ID";
            // 
            // txtVendGroupName
            // 
            this.txtVendGroupName.Enabled = false;
            this.txtVendGroupName.Location = new System.Drawing.Point(175, 38);
            this.txtVendGroupName.MaxLength = 100;
            this.txtVendGroupName.Name = "txtVendGroupName";
            this.txtVendGroupName.Size = new System.Drawing.Size(219, 20);
            this.txtVendGroupName.TabIndex = 2;
            this.txtVendGroupName.Validating += new System.ComponentModel.CancelEventHandler(this.txtVendGroupName_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 95;
            this.label2.Text = "Vendor Group Name";
            // 
            // txtKursOrigin
            // 
            this.txtKursOrigin.Enabled = false;
            this.txtKursOrigin.Location = new System.Drawing.Point(175, 64);
            this.txtKursOrigin.MaxLength = 3;
            this.txtKursOrigin.Name = "txtKursOrigin";
            this.txtKursOrigin.Size = new System.Drawing.Size(50, 20);
            this.txtKursOrigin.TabIndex = 3;
            this.txtKursOrigin.Validating += new System.ComponentModel.CancelEventHandler(this.txtKursOrigin_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 97;
            this.label3.Text = "Kurs Origin";
            // 
            // txtDepositAmount
            // 
            this.txtDepositAmount.Enabled = false;
            this.txtDepositAmount.Location = new System.Drawing.Point(553, 127);
            this.txtDepositAmount.MaxLength = 20;
            this.txtDepositAmount.Name = "txtDepositAmount";
            this.txtDepositAmount.Size = new System.Drawing.Size(157, 20);
            this.txtDepositAmount.TabIndex = 11;
            this.txtDepositAmount.Validating += new System.ComponentModel.CancelEventHandler(this.txtDepositAmount_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(392, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 101;
            this.label5.Text = "Deposit Amount";
            // 
            // txtDepositCur
            // 
            this.txtDepositCur.Enabled = false;
            this.txtDepositCur.Location = new System.Drawing.Point(553, 101);
            this.txtDepositCur.MaxLength = 3;
            this.txtDepositCur.Name = "txtDepositCur";
            this.txtDepositCur.Size = new System.Drawing.Size(50, 20);
            this.txtDepositCur.TabIndex = 9;
            this.txtDepositCur.Validating += new System.ComponentModel.CancelEventHandler(this.txtDepositCur_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(392, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(141, 13);
            this.label6.TabIndex = 99;
            this.label6.Text = "Deposit Amount Currency ID";
            // 
            // txtDPAmount
            // 
            this.txtDPAmount.Enabled = false;
            this.txtDPAmount.Location = new System.Drawing.Point(553, 190);
            this.txtDPAmount.MaxLength = 20;
            this.txtDPAmount.Name = "txtDPAmount";
            this.txtDPAmount.Size = new System.Drawing.Size(157, 20);
            this.txtDPAmount.TabIndex = 14;
            this.txtDPAmount.Validating += new System.ComponentModel.CancelEventHandler(this.txtDPAmount_Validating);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(392, 193);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 107;
            this.label8.Text = "DP Amount";
            // 
            // txtDPCur
            // 
            this.txtDPCur.Enabled = false;
            this.txtDPCur.Location = new System.Drawing.Point(553, 164);
            this.txtDPCur.MaxLength = 3;
            this.txtDPCur.Name = "txtDPCur";
            this.txtDPCur.Size = new System.Drawing.Size(50, 20);
            this.txtDPCur.TabIndex = 12;
            this.txtDPCur.Validating += new System.ComponentModel.CancelEventHandler(this.txtDPCur_Validating);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(392, 167);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(120, 13);
            this.label9.TabIndex = 105;
            this.label9.Text = "DP Amount Currency ID";
            // 
            // txtCreditLimitPO
            // 
            this.txtCreditLimitPO.Enabled = false;
            this.txtCreditLimitPO.Location = new System.Drawing.Point(175, 151);
            this.txtCreditLimitPO.MaxLength = 20;
            this.txtCreditLimitPO.Name = "txtCreditLimitPO";
            this.txtCreditLimitPO.Size = new System.Drawing.Size(157, 20);
            this.txtCreditLimitPO.TabIndex = 8;
            this.txtCreditLimitPO.Validating += new System.ComponentModel.CancelEventHandler(this.txtCreditLimitPO_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 113;
            this.label4.Text = "Credit Limit Per PO";
            // 
            // txtCreditLimit
            // 
            this.txtCreditLimit.Enabled = false;
            this.txtCreditLimit.Location = new System.Drawing.Point(175, 125);
            this.txtCreditLimit.MaxLength = 20;
            this.txtCreditLimit.Name = "txtCreditLimit";
            this.txtCreditLimit.Size = new System.Drawing.Size(157, 20);
            this.txtCreditLimit.TabIndex = 7;
            this.txtCreditLimit.Validating += new System.ComponentModel.CancelEventHandler(this.txtCreditLimit_Validating);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 111;
            this.label7.Text = "Credit Limit";
            // 
            // txtCreditCur
            // 
            this.txtCreditCur.Enabled = false;
            this.txtCreditCur.Location = new System.Drawing.Point(175, 99);
            this.txtCreditCur.MaxLength = 3;
            this.txtCreditCur.Name = "txtCreditCur";
            this.txtCreditCur.Size = new System.Drawing.Size(50, 20);
            this.txtCreditCur.TabIndex = 5;
            this.txtCreditCur.Validating += new System.ComponentModel.CancelEventHandler(this.txtCreditCur_Validating);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 102);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(117, 13);
            this.label10.TabIndex = 109;
            this.label10.Text = "Credit Limit Currency ID";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(423, 219);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 17;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(342, 219);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 16;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(261, 219);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSearchDepositCur
            // 
            this.btnSearchDepositCur.Enabled = false;
            this.btnSearchDepositCur.Location = new System.Drawing.Point(609, 99);
            this.btnSearchDepositCur.Name = "btnSearchDepositCur";
            this.btnSearchDepositCur.Size = new System.Drawing.Size(36, 23);
            this.btnSearchDepositCur.TabIndex = 10;
            this.btnSearchDepositCur.Text = "...";
            this.btnSearchDepositCur.UseVisualStyleBackColor = true;
            this.btnSearchDepositCur.Click += new System.EventHandler(this.btnSearchDepositCur_Click);
            // 
            // btnSearchDPCur
            // 
            this.btnSearchDPCur.Enabled = false;
            this.btnSearchDPCur.Location = new System.Drawing.Point(613, 162);
            this.btnSearchDPCur.Name = "btnSearchDPCur";
            this.btnSearchDPCur.Size = new System.Drawing.Size(36, 23);
            this.btnSearchDPCur.TabIndex = 13;
            this.btnSearchDPCur.Text = "...";
            this.btnSearchDPCur.UseVisualStyleBackColor = true;
            this.btnSearchDPCur.Click += new System.EventHandler(this.btnSearchDPCur_Click);
            // 
            // btnSearchCreditCur
            // 
            this.btnSearchCreditCur.Enabled = false;
            this.btnSearchCreditCur.Location = new System.Drawing.Point(231, 99);
            this.btnSearchCreditCur.Name = "btnSearchCreditCur";
            this.btnSearchCreditCur.Size = new System.Drawing.Size(36, 23);
            this.btnSearchCreditCur.TabIndex = 6;
            this.btnSearchCreditCur.Text = "...";
            this.btnSearchCreditCur.UseVisualStyleBackColor = true;
            this.btnSearchCreditCur.Click += new System.EventHandler(this.btnSearchCreditCur_Click);
            // 
            // btnSearchKurs
            // 
            this.btnSearchKurs.Enabled = false;
            this.btnSearchKurs.Location = new System.Drawing.Point(231, 62);
            this.btnSearchKurs.Name = "btnSearchKurs";
            this.btnSearchKurs.Size = new System.Drawing.Size(36, 23);
            this.btnSearchKurs.TabIndex = 4;
            this.btnSearchKurs.Text = "...";
            this.btnSearchKurs.UseVisualStyleBackColor = true;
            this.btnSearchKurs.Click += new System.EventHandler(this.btnSearchKurs_Click);
            // 
            // VendGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 254);
            this.Controls.Add(this.btnSearchKurs);
            this.Controls.Add(this.btnSearchCreditCur);
            this.Controls.Add(this.btnSearchDPCur);
            this.Controls.Add(this.btnSearchDepositCur);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtCreditLimitPO);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCreditLimit);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtCreditCur);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtDPAmount);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtDPCur);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtDepositAmount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDepositCur);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtKursOrigin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtVendGroupName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtVendGroupId);
            this.Controls.Add(this.label1);
            this.Name = "VendGroup";
            this.Text = "Vendor Group";
            this.Load += new System.EventHandler(this.VendGroup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtVendGroupId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtVendGroupName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtKursOrigin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDepositAmount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDepositCur;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDPAmount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtDPCur;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCreditLimitPO;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCreditLimit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCreditCur;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.Button btnSearchDepositCur;
        internal System.Windows.Forms.Button btnSearchDPCur;
        internal System.Windows.Forms.Button btnSearchCreditCur;
        internal System.Windows.Forms.Button btnSearchKurs;

    }
}