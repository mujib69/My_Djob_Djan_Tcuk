namespace ISBS_New.CashAndBank.BankStatement
{
    partial class HeaderBankStatement
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.groupImport = new System.Windows.Forms.GroupBox();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.txtAttachment = new System.Windows.Forms.TextBox();
            this.btnAttachment = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAccountName = new System.Windows.Forms.TextBox();
            this.btnLookUpBank = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAccountNo = new System.Windows.Forms.TextBox();
            this.txtBankCode = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.dgvBankStatement = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupImport.SuspendLayout();
            this.grpCustomer.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankStatement)).BeginInit();
            this.SuspendLayout();
            // 
            // groupImport
            // 
            this.groupImport.Controls.Add(this.grpCustomer);
            this.groupImport.Controls.Add(this.btnImport);
            this.groupImport.Controls.Add(this.grpDetail);
            this.groupImport.Controls.Add(this.btnExit);
            this.groupImport.Location = new System.Drawing.Point(12, 49);
            this.groupImport.Name = "groupImport";
            this.groupImport.Size = new System.Drawing.Size(721, 482);
            this.groupImport.TabIndex = 153;
            this.groupImport.TabStop = false;
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.txtAttachment);
            this.grpCustomer.Controls.Add(this.btnAttachment);
            this.grpCustomer.Controls.Add(this.label1);
            this.grpCustomer.Controls.Add(this.txtAccountName);
            this.grpCustomer.Controls.Add(this.btnLookUpBank);
            this.grpCustomer.Controls.Add(this.label12);
            this.grpCustomer.Controls.Add(this.txtAccountNo);
            this.grpCustomer.Controls.Add(this.txtBankCode);
            this.grpCustomer.Controls.Add(this.label13);
            this.grpCustomer.Location = new System.Drawing.Point(6, 11);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(701, 105);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            // 
            // txtAttachment
            // 
            this.txtAttachment.Enabled = false;
            this.txtAttachment.Location = new System.Drawing.Point(480, 17);
            this.txtAttachment.Multiline = true;
            this.txtAttachment.Name = "txtAttachment";
            this.txtAttachment.ReadOnly = true;
            this.txtAttachment.Size = new System.Drawing.Size(200, 68);
            this.txtAttachment.TabIndex = 146;
            // 
            // btnAttachment
            // 
            this.btnAttachment.Enabled = false;
            this.btnAttachment.Location = new System.Drawing.Point(394, 17);
            this.btnAttachment.Name = "btnAttachment";
            this.btnAttachment.Size = new System.Drawing.Size(69, 23);
            this.btnAttachment.TabIndex = 145;
            this.btnAttachment.Text = "Attachment";
            this.btnAttachment.UseVisualStyleBackColor = true;
            this.btnAttachment.Click += new System.EventHandler(this.btnAttachment_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 144;
            this.label1.Text = "AccountName";
            // 
            // txtAccountName
            // 
            this.txtAccountName.Enabled = false;
            this.txtAccountName.Location = new System.Drawing.Point(94, 65);
            this.txtAccountName.Name = "txtAccountName";
            this.txtAccountName.ReadOnly = true;
            this.txtAccountName.Size = new System.Drawing.Size(200, 20);
            this.txtAccountName.TabIndex = 143;
            // 
            // btnLookUpBank
            // 
            this.btnLookUpBank.Location = new System.Drawing.Point(295, 15);
            this.btnLookUpBank.Name = "btnLookUpBank";
            this.btnLookUpBank.Size = new System.Drawing.Size(32, 23);
            this.btnLookUpBank.TabIndex = 142;
            this.btnLookUpBank.Text = "...";
            this.btnLookUpBank.UseVisualStyleBackColor = true;
            this.btnLookUpBank.Click += new System.EventHandler(this.btnLookUpBank_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 13);
            this.label12.TabIndex = 141;
            this.label12.Text = "AccountNo";
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Enabled = false;
            this.txtAccountNo.Location = new System.Drawing.Point(94, 41);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.ReadOnly = true;
            this.txtAccountNo.Size = new System.Drawing.Size(200, 20);
            this.txtAccountNo.TabIndex = 140;
            // 
            // txtBankCode
            // 
            this.txtBankCode.Enabled = false;
            this.txtBankCode.Location = new System.Drawing.Point(94, 17);
            this.txtBankCode.Name = "txtBankCode";
            this.txtBankCode.ReadOnly = true;
            this.txtBankCode.Size = new System.Drawing.Size(200, 20);
            this.txtBankCode.TabIndex = 139;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 20);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 13);
            this.label13.TabIndex = 138;
            this.label13.Text = "Bank Code";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(555, 452);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 143;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.dgvBankStatement);
            this.grpDetail.Location = new System.Drawing.Point(6, 118);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(701, 322);
            this.grpDetail.TabIndex = 142;
            this.grpDetail.TabStop = false;
            // 
            // dgvBankStatement
            // 
            this.dgvBankStatement.AllowUserToAddRows = false;
            this.dgvBankStatement.AllowUserToDeleteRows = false;
            this.dgvBankStatement.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBankStatement.Location = new System.Drawing.Point(9, 13);
            this.dgvBankStatement.Name = "dgvBankStatement";
            this.dgvBankStatement.ReadOnly = true;
            this.dgvBankStatement.Size = new System.Drawing.Size(686, 297);
            this.dgvBankStatement.TabIndex = 11;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(633, 451);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // HeaderBankStatement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(748, 543);
            this.Controls.Add(this.groupImport);
            this.Name = "HeaderBankStatement";
            this.Resizable = false;
            this.Text = "Import Bank Statement";
            this.Load += new System.EventHandler(this.HeaderBankStatement_Load);
            this.Shown += new System.EventHandler(this.HeaderBankStatement_Shown);
            this.groupImport.ResumeLayout(false);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankStatement)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupImport;
        internal System.Windows.Forms.GroupBox grpCustomer;
        private System.Windows.Forms.Button btnLookUpBank;
        internal System.Windows.Forms.TextBox txtBankCode;
        internal System.Windows.Forms.Button btnImport;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.DataGridView dgvBankStatement;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label12;
        internal System.Windows.Forms.TextBox txtAccountNo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txtAccountName;
        internal System.Windows.Forms.TextBox txtAttachment;
        private System.Windows.Forms.Button btnAttachment;


    }
}
