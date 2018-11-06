namespace ISBS_New.Purchase.Retur.RTBApproval
{
    partial class RTBApproval
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
            this.dgvRTB = new System.Windows.Forms.DataGridView();
            this.txtStatusName = new System.Windows.Forms.TextBox();
            this.txtApproved = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.gbApprove = new System.Windows.Forms.GroupBox();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.btnRevision = new System.Windows.Forms.Button();
            this.txtSiteID = new System.Windows.Forms.TextBox();
            this.txtSiteLocation = new System.Windows.Forms.TextBox();
            this.txtSiteName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtVendID = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.gbGV = new System.Windows.Forms.GroupBox();
            this.txtVendName = new System.Windows.Forms.TextBox();
            this.txtGRNum = new System.Windows.Forms.TextBox();
            this.txtRTBNum = new System.Windows.Forms.TextBox();
            this.dtRTB = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRTB)).BeginInit();
            this.gbMain.SuspendLayout();
            this.gbApprove.SuspendLayout();
            this.gbGV.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvRTB
            // 
            this.dgvRTB.AllowUserToAddRows = false;
            this.dgvRTB.AllowUserToDeleteRows = false;
            this.dgvRTB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRTB.Location = new System.Drawing.Point(9, 19);
            this.dgvRTB.Name = "dgvRTB";
            this.dgvRTB.Size = new System.Drawing.Size(714, 155);
            this.dgvRTB.TabIndex = 36;
            // 
            // txtStatusName
            // 
            this.txtStatusName.Enabled = false;
            this.txtStatusName.Location = new System.Drawing.Point(139, 107);
            this.txtStatusName.Name = "txtStatusName";
            this.txtStatusName.ReadOnly = true;
            this.txtStatusName.Size = new System.Drawing.Size(200, 20);
            this.txtStatusName.TabIndex = 166;
            // 
            // txtApproved
            // 
            this.txtApproved.Enabled = false;
            this.txtApproved.Location = new System.Drawing.Point(139, 130);
            this.txtApproved.Name = "txtApproved";
            this.txtApproved.ReadOnly = true;
            this.txtApproved.Size = new System.Drawing.Size(200, 20);
            this.txtApproved.TabIndex = 165;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 164;
            this.label7.Text = "RTB Approved :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 163;
            this.label8.Text = "RTB Status :";
            // 
            // gbMain
            // 
            this.gbMain.Controls.Add(this.gbApprove);
            this.gbMain.Controls.Add(this.txtStatusName);
            this.gbMain.Controls.Add(this.txtApproved);
            this.gbMain.Controls.Add(this.label7);
            this.gbMain.Controls.Add(this.label8);
            this.gbMain.Controls.Add(this.txtSiteID);
            this.gbMain.Controls.Add(this.txtSiteLocation);
            this.gbMain.Controls.Add(this.txtSiteName);
            this.gbMain.Controls.Add(this.label21);
            this.gbMain.Controls.Add(this.txtVendID);
            this.gbMain.Controls.Add(this.txtNotes);
            this.gbMain.Controls.Add(this.label20);
            this.gbMain.Controls.Add(this.btnExit);
            this.gbMain.Controls.Add(this.gbGV);
            this.gbMain.Controls.Add(this.txtVendName);
            this.gbMain.Controls.Add(this.txtGRNum);
            this.gbMain.Controls.Add(this.txtRTBNum);
            this.gbMain.Controls.Add(this.dtRTB);
            this.gbMain.Controls.Add(this.label11);
            this.gbMain.Controls.Add(this.label5);
            this.gbMain.Controls.Add(this.label3);
            this.gbMain.Controls.Add(this.label2);
            this.gbMain.Location = new System.Drawing.Point(12, 53);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(766, 408);
            this.gbMain.TabIndex = 44;
            this.gbMain.TabStop = false;
            // 
            // gbApprove
            // 
            this.gbApprove.Controls.Add(this.btnReject);
            this.gbApprove.Controls.Add(this.btnApprove);
            this.gbApprove.Controls.Add(this.btnRevision);
            this.gbApprove.Location = new System.Drawing.Point(412, 368);
            this.gbApprove.Name = "gbApprove";
            this.gbApprove.Size = new System.Drawing.Size(247, 30);
            this.gbApprove.TabIndex = 167;
            this.gbApprove.TabStop = false;
            // 
            // btnReject
            // 
            this.btnReject.Location = new System.Drawing.Point(87, 3);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(75, 23);
            this.btnReject.TabIndex = 25;
            this.btnReject.Text = "Reject";
            this.btnReject.UseVisualStyleBackColor = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnApprove
            // 
            this.btnApprove.Location = new System.Drawing.Point(6, 3);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(75, 23);
            this.btnApprove.TabIndex = 24;
            this.btnApprove.Text = "Approve";
            this.btnApprove.UseVisualStyleBackColor = true;
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // btnRevision
            // 
            this.btnRevision.Location = new System.Drawing.Point(168, 3);
            this.btnRevision.Name = "btnRevision";
            this.btnRevision.Size = new System.Drawing.Size(75, 23);
            this.btnRevision.TabIndex = 26;
            this.btnRevision.Text = "Revision";
            this.btnRevision.UseVisualStyleBackColor = true;
            this.btnRevision.Click += new System.EventHandler(this.btnRevision_Click);
            // 
            // txtSiteID
            // 
            this.txtSiteID.Enabled = false;
            this.txtSiteID.Location = new System.Drawing.Point(519, 51);
            this.txtSiteID.Name = "txtSiteID";
            this.txtSiteID.ReadOnly = true;
            this.txtSiteID.Size = new System.Drawing.Size(36, 20);
            this.txtSiteID.TabIndex = 160;
            this.txtSiteID.Visible = false;
            // 
            // txtSiteLocation
            // 
            this.txtSiteLocation.Enabled = false;
            this.txtSiteLocation.Location = new System.Drawing.Point(519, 79);
            this.txtSiteLocation.Name = "txtSiteLocation";
            this.txtSiteLocation.Size = new System.Drawing.Size(200, 20);
            this.txtSiteLocation.TabIndex = 159;
            // 
            // txtSiteName
            // 
            this.txtSiteName.Enabled = false;
            this.txtSiteName.Location = new System.Drawing.Point(519, 52);
            this.txtSiteName.Name = "txtSiteName";
            this.txtSiteName.Size = new System.Drawing.Size(200, 20);
            this.txtSiteName.TabIndex = 158;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(431, 55);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(62, 13);
            this.label21.TabIndex = 42;
            this.label21.Text = "Warehouse";
            // 
            // txtVendID
            // 
            this.txtVendID.Enabled = false;
            this.txtVendID.Location = new System.Drawing.Point(519, 19);
            this.txtVendID.Name = "txtVendID";
            this.txtVendID.Size = new System.Drawing.Size(50, 20);
            this.txtVendID.TabIndex = 40;
            // 
            // txtNotes
            // 
            this.txtNotes.Enabled = false;
            this.txtNotes.Location = new System.Drawing.Point(519, 106);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(200, 43);
            this.txtNotes.TabIndex = 8;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(431, 109);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(35, 13);
            this.label20.TabIndex = 38;
            this.label20.Text = "Notes";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(665, 371);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 27;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // gbGV
            // 
            this.gbGV.Controls.Add(this.dgvRTB);
            this.gbGV.Location = new System.Drawing.Point(17, 166);
            this.gbGV.Name = "gbGV";
            this.gbGV.Size = new System.Drawing.Size(729, 184);
            this.gbGV.TabIndex = 33;
            this.gbGV.TabStop = false;
            // 
            // txtVendName
            // 
            this.txtVendName.Enabled = false;
            this.txtVendName.Location = new System.Drawing.Point(575, 19);
            this.txtVendName.Name = "txtVendName";
            this.txtVendName.Size = new System.Drawing.Size(144, 20);
            this.txtVendName.TabIndex = 17;
            // 
            // txtGRNum
            // 
            this.txtGRNum.Enabled = false;
            this.txtGRNum.Location = new System.Drawing.Point(139, 78);
            this.txtGRNum.Name = "txtGRNum";
            this.txtGRNum.ReadOnly = true;
            this.txtGRNum.Size = new System.Drawing.Size(200, 20);
            this.txtGRNum.TabIndex = 13;
            // 
            // txtRTBNum
            // 
            this.txtRTBNum.Enabled = false;
            this.txtRTBNum.Location = new System.Drawing.Point(139, 48);
            this.txtRTBNum.Name = "txtRTBNum";
            this.txtRTBNum.Size = new System.Drawing.Size(200, 20);
            this.txtRTBNum.TabIndex = 11;
            // 
            // dtRTB
            // 
            this.dtRTB.CustomFormat = "dd/MM/yyyy";
            this.dtRTB.Enabled = false;
            this.dtRTB.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtRTB.Location = new System.Drawing.Point(139, 22);
            this.dtRTB.Name = "dtRTB";
            this.dtRTB.Size = new System.Drawing.Size(200, 20);
            this.dtRTB.TabIndex = 10;
            this.dtRTB.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "RTB Number";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Goods Receipt Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(431, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Vendor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "RTB Date";
            // 
            // RTBApproval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 481);
            this.Controls.Add(this.gbMain);
            this.Name = "RTBApproval";
            this.Text = "Retur Tukar Barang Approval";
            this.Load += new System.EventHandler(this.RTBApproval_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRTB)).EndInit();
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            this.gbApprove.ResumeLayout(false);
            this.gbGV.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRTB;
        internal System.Windows.Forms.TextBox txtStatusName;
        internal System.Windows.Forms.TextBox txtApproved;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox gbMain;
        internal System.Windows.Forms.TextBox txtSiteID;
        private System.Windows.Forms.TextBox txtSiteLocation;
        internal System.Windows.Forms.TextBox txtSiteName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtVendID;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRevision;
        private System.Windows.Forms.GroupBox gbGV;
        private System.Windows.Forms.Button btnApprove;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.TextBox txtVendName;
        private System.Windows.Forms.TextBox txtGRNum;
        private System.Windows.Forms.TextBox txtRTBNum;
        private System.Windows.Forms.DateTimePicker dtRTB;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbApprove;
    }
}