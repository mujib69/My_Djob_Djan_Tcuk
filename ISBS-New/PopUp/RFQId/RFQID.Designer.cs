namespace ISBS_New.PopUp.RFQId
{
    partial class RFQID
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
            this.groupRFQ = new System.Windows.Forms.GroupBox();
            this.grpVN = new System.Windows.Forms.GroupBox();
            this.grpNotes = new System.Windows.Forms.GroupBox();
            this.rtxtNotes = new System.Windows.Forms.RichTextBox();
            this.grpVendor = new System.Windows.Forms.GroupBox();
            this.dgvVendor = new System.Windows.Forms.DataGridView();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.lblVendName = new System.Windows.Forms.Label();
            this.lblVendorId = new System.Windows.Forms.Label();
            this.lblPRNo = new System.Windows.Forms.Label();
            this.lblRFQId = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dtRFQDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.grp = new System.Windows.Forms.GroupBox();
            this.dgvDetails = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupRFQ.SuspendLayout();
            this.grpVN.SuspendLayout();
            this.grpNotes.SuspendLayout();
            this.grpVendor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendor)).BeginInit();
            this.grpCustomer.SuspendLayout();
            this.grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // groupRFQ
            // 
            this.groupRFQ.Controls.Add(this.grpVN);
            this.groupRFQ.Controls.Add(this.grpCustomer);
            this.groupRFQ.Controls.Add(this.grp);
            this.groupRFQ.Location = new System.Drawing.Point(23, 63);
            this.groupRFQ.Name = "groupRFQ";
            this.groupRFQ.Size = new System.Drawing.Size(834, 542);
            this.groupRFQ.TabIndex = 150;
            this.groupRFQ.TabStop = false;
            // 
            // grpVN
            // 
            this.grpVN.Controls.Add(this.grpNotes);
            this.grpVN.Controls.Add(this.grpVendor);
            this.grpVN.Location = new System.Drawing.Point(6, 353);
            this.grpVN.Name = "grpVN";
            this.grpVN.Size = new System.Drawing.Size(809, 176);
            this.grpVN.TabIndex = 150;
            this.grpVN.TabStop = false;
            // 
            // grpNotes
            // 
            this.grpNotes.Controls.Add(this.rtxtNotes);
            this.grpNotes.Location = new System.Drawing.Point(470, 20);
            this.grpNotes.Name = "grpNotes";
            this.grpNotes.Size = new System.Drawing.Size(329, 136);
            this.grpNotes.TabIndex = 148;
            this.grpNotes.TabStop = false;
            this.grpNotes.Text = "Notes";
            // 
            // rtxtNotes
            // 
            this.rtxtNotes.Enabled = false;
            this.rtxtNotes.Location = new System.Drawing.Point(18, 19);
            this.rtxtNotes.MaxLength = 250;
            this.rtxtNotes.Name = "rtxtNotes";
            this.rtxtNotes.Size = new System.Drawing.Size(293, 98);
            this.rtxtNotes.TabIndex = 0;
            this.rtxtNotes.Text = "";
            // 
            // grpVendor
            // 
            this.grpVendor.Controls.Add(this.dgvVendor);
            this.grpVendor.Location = new System.Drawing.Point(6, 9);
            this.grpVendor.Name = "grpVendor";
            this.grpVendor.Size = new System.Drawing.Size(445, 161);
            this.grpVendor.TabIndex = 147;
            this.grpVendor.TabStop = false;
            this.grpVendor.Text = "Vendor";
            // 
            // dgvVendor
            // 
            this.dgvVendor.AllowUserToAddRows = false;
            this.dgvVendor.AllowUserToDeleteRows = false;
            this.dgvVendor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVendor.Location = new System.Drawing.Point(8, 19);
            this.dgvVendor.Name = "dgvVendor";
            this.dgvVendor.ReadOnly = true;
            this.dgvVendor.Size = new System.Drawing.Size(427, 129);
            this.dgvVendor.TabIndex = 0;
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.lblVendName);
            this.grpCustomer.Controls.Add(this.lblVendorId);
            this.grpCustomer.Controls.Add(this.lblPRNo);
            this.grpCustomer.Controls.Add(this.lblRFQId);
            this.grpCustomer.Controls.Add(this.label1);
            this.grpCustomer.Controls.Add(this.label5);
            this.grpCustomer.Controls.Add(this.label7);
            this.grpCustomer.Controls.Add(this.dtRFQDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(809, 91);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Header";
            // 
            // lblVendName
            // 
            this.lblVendName.AutoSize = true;
            this.lblVendName.Location = new System.Drawing.Point(537, 42);
            this.lblVendName.Name = "lblVendName";
            this.lblVendName.Size = new System.Drawing.Size(70, 13);
            this.lblVendName.TabIndex = 139;
            this.lblVendName.Text = "lblVendName";
            // 
            // lblVendorId
            // 
            this.lblVendorId.AutoSize = true;
            this.lblVendorId.Location = new System.Drawing.Point(537, 18);
            this.lblVendorId.Name = "lblVendorId";
            this.lblVendorId.Size = new System.Drawing.Size(60, 13);
            this.lblVendorId.TabIndex = 138;
            this.lblVendorId.Text = "lblVendorId";
            // 
            // lblPRNo
            // 
            this.lblPRNo.AutoSize = true;
            this.lblPRNo.Location = new System.Drawing.Point(136, 67);
            this.lblPRNo.Name = "lblPRNo";
            this.lblPRNo.Size = new System.Drawing.Size(46, 13);
            this.lblPRNo.TabIndex = 137;
            this.lblPRNo.Text = "lblPRNo";
            // 
            // lblRFQId
            // 
            this.lblRFQId.AutoSize = true;
            this.lblRFQId.Location = new System.Drawing.Point(136, 42);
            this.lblRFQId.Name = "lblRFQId";
            this.lblRFQId.Size = new System.Drawing.Size(48, 13);
            this.lblRFQId.TabIndex = 136;
            this.lblRFQId.Text = "lblRFQId";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 135;
            this.label1.Text = "PR No                          :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(419, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 13);
            this.label5.TabIndex = 127;
            this.label5.Text = "Vendor Name             :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(419, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 13);
            this.label7.TabIndex = 118;
            this.label7.Text = "Vendor ID                  :";
            // 
            // dtRFQDate
            // 
            this.dtRFQDate.CustomFormat = "dd-MM-yyyy";
            this.dtRFQDate.Enabled = false;
            this.dtRFQDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtRFQDate.Location = new System.Drawing.Point(139, 11);
            this.dtRFQDate.Name = "dtRFQDate";
            this.dtRFQDate.Size = new System.Drawing.Size(200, 20);
            this.dtRFQDate.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(121, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "RFQ No                        :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(121, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "RFQ Date                     :";
            // 
            // grp
            // 
            this.grp.Controls.Add(this.dgvDetails);
            this.grp.Location = new System.Drawing.Point(7, 107);
            this.grp.Name = "grp";
            this.grp.Size = new System.Drawing.Size(810, 216);
            this.grp.TabIndex = 142;
            this.grp.TabStop = false;
            this.grp.Text = "Details";
            // 
            // dgvDetails
            // 
            this.dgvDetails.AllowUserToAddRows = false;
            this.dgvDetails.AllowUserToDeleteRows = false;
            this.dgvDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetails.Location = new System.Drawing.Point(9, 19);
            this.dgvDetails.Name = "dgvDetails";
            this.dgvDetails.ReadOnly = true;
            this.dgvDetails.Size = new System.Drawing.Size(794, 186);
            this.dgvDetails.TabIndex = 11;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(782, 611);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 151;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // RFQID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 650);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupRFQ);
            this.Name = "RFQID";
            this.Text = "Request For Quotation";
            this.groupRFQ.ResumeLayout(false);
            this.grpVN.ResumeLayout(false);
            this.grpNotes.ResumeLayout(false);
            this.grpVendor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendor)).EndInit();
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.grp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupRFQ;
        private System.Windows.Forms.GroupBox grpVN;
        private System.Windows.Forms.GroupBox grpNotes;
        private System.Windows.Forms.RichTextBox rtxtNotes;
        private System.Windows.Forms.GroupBox grpVendor;
        private System.Windows.Forms.DataGridView dgvVendor;
        internal System.Windows.Forms.GroupBox grpCustomer;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.DateTimePicker dtRFQDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.GroupBox grp;
        internal System.Windows.Forms.DataGridView dgvDetails;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblVendName;
        private System.Windows.Forms.Label lblVendorId;
        private System.Windows.Forms.Label lblPRNo;
        private System.Windows.Forms.Label lblRFQId;

    }
}