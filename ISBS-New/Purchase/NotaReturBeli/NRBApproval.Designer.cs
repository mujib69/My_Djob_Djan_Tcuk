namespace ISBS_New.Purchase.NotaReturBeli
{
    partial class NRBApproval
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
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.cmbJenisRetur = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.txtPONum = new System.Windows.Forms.TextBox();
            this.btnRevision = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtStatusName = new System.Windows.Forms.TextBox();
            this.txtApproved = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSiteID = new System.Windows.Forms.TextBox();
            this.txtSiteLocation = new System.Windows.Forms.TextBox();
            this.txtSiteName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtVendID = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtVendName = new System.Windows.Forms.TextBox();
            this.txtNRBNum = new System.Windows.Forms.TextBox();
            this.dtNRB = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtGR = new System.Windows.Forms.DateTimePicker();
            this.txtGRNum = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.gbGV = new System.Windows.Forms.GroupBox();
            this.dgvNRB = new System.Windows.Forms.DataGridView();
            this.gbMain.SuspendLayout();
            this.gbGV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNRB)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMain
            // 
            this.gbMain.Controls.Add(this.cmbJenisRetur);
            this.gbMain.Controls.Add(this.label4);
            this.gbMain.Controls.Add(this.btnReject);
            this.gbMain.Controls.Add(this.btnApprove);
            this.gbMain.Controls.Add(this.txtPONum);
            this.gbMain.Controls.Add(this.btnRevision);
            this.gbMain.Controls.Add(this.label1);
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
            this.gbMain.Controls.Add(this.txtVendName);
            this.gbMain.Controls.Add(this.txtNRBNum);
            this.gbMain.Controls.Add(this.dtNRB);
            this.gbMain.Controls.Add(this.label11);
            this.gbMain.Controls.Add(this.label5);
            this.gbMain.Controls.Add(this.label3);
            this.gbMain.Controls.Add(this.label2);
            this.gbMain.Controls.Add(this.dtGR);
            this.gbMain.Controls.Add(this.txtGRNum);
            this.gbMain.Controls.Add(this.btnExit);
            this.gbMain.Controls.Add(this.gbGV);
            this.gbMain.Location = new System.Drawing.Point(12, 53);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(766, 408);
            this.gbMain.TabIndex = 45;
            this.gbMain.TabStop = false;
            // 
            // cmbJenisRetur
            // 
            this.cmbJenisRetur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJenisRetur.FormattingEnabled = true;
            this.cmbJenisRetur.Location = new System.Drawing.Point(521, 143);
            this.cmbJenisRetur.Name = "cmbJenisRetur";
            this.cmbJenisRetur.Size = new System.Drawing.Size(200, 21);
            this.cmbJenisRetur.TabIndex = 193;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(433, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 192;
            this.label4.Text = "Jenis Retur";
            // 
            // btnReject
            // 
            this.btnReject.Location = new System.Drawing.Point(596, 361);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(75, 23);
            this.btnReject.TabIndex = 25;
            this.btnReject.Text = "Reject";
            this.btnReject.UseVisualStyleBackColor = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnApprove
            // 
            this.btnApprove.Location = new System.Drawing.Point(527, 361);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(75, 23);
            this.btnApprove.TabIndex = 24;
            this.btnApprove.Text = "Approve";
            this.btnApprove.UseVisualStyleBackColor = true;
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // txtPONum
            // 
            this.txtPONum.Enabled = false;
            this.txtPONum.Location = new System.Drawing.Point(141, 95);
            this.txtPONum.Name = "txtPONum";
            this.txtPONum.ReadOnly = true;
            this.txtPONum.Size = new System.Drawing.Size(200, 20);
            this.txtPONum.TabIndex = 181;
            this.txtPONum.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtPONum_MouseDown);
            // 
            // btnRevision
            // 
            this.btnRevision.Location = new System.Drawing.Point(446, 361);
            this.btnRevision.Name = "btnRevision";
            this.btnRevision.Size = new System.Drawing.Size(75, 23);
            this.btnRevision.TabIndex = 26;
            this.btnRevision.Text = "Revision";
            this.btnRevision.UseVisualStyleBackColor = true;
            this.btnRevision.Click += new System.EventHandler(this.btnRevision_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 173;
            this.label1.Text = "PO Number";
            // 
            // txtStatusName
            // 
            this.txtStatusName.Enabled = false;
            this.txtStatusName.Location = new System.Drawing.Point(141, 121);
            this.txtStatusName.Name = "txtStatusName";
            this.txtStatusName.ReadOnly = true;
            this.txtStatusName.Size = new System.Drawing.Size(200, 20);
            this.txtStatusName.TabIndex = 182;
            // 
            // txtApproved
            // 
            this.txtApproved.Enabled = false;
            this.txtApproved.Location = new System.Drawing.Point(141, 147);
            this.txtApproved.Name = "txtApproved";
            this.txtApproved.ReadOnly = true;
            this.txtApproved.Size = new System.Drawing.Size(200, 20);
            this.txtApproved.TabIndex = 183;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 169;
            this.label7.Text = "RB Approved :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 125);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 174;
            this.label8.Text = "RB Status :";
            // 
            // txtSiteID
            // 
            this.txtSiteID.Enabled = false;
            this.txtSiteID.Location = new System.Drawing.Point(521, 43);
            this.txtSiteID.Name = "txtSiteID";
            this.txtSiteID.ReadOnly = true;
            this.txtSiteID.Size = new System.Drawing.Size(36, 20);
            this.txtSiteID.TabIndex = 187;
            this.txtSiteID.Visible = false;
            // 
            // txtSiteLocation
            // 
            this.txtSiteLocation.Enabled = false;
            this.txtSiteLocation.Location = new System.Drawing.Point(521, 69);
            this.txtSiteLocation.Name = "txtSiteLocation";
            this.txtSiteLocation.Size = new System.Drawing.Size(200, 20);
            this.txtSiteLocation.TabIndex = 189;
            // 
            // txtSiteName
            // 
            this.txtSiteName.Enabled = false;
            this.txtSiteName.Location = new System.Drawing.Point(521, 43);
            this.txtSiteName.Name = "txtSiteName";
            this.txtSiteName.Size = new System.Drawing.Size(200, 20);
            this.txtSiteName.TabIndex = 188;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(433, 47);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(62, 13);
            this.label21.TabIndex = 170;
            this.label21.Text = "Warehouse";
            // 
            // txtVendID
            // 
            this.txtVendID.Enabled = false;
            this.txtVendID.Location = new System.Drawing.Point(521, 17);
            this.txtVendID.Name = "txtVendID";
            this.txtVendID.Size = new System.Drawing.Size(50, 20);
            this.txtVendID.TabIndex = 184;
            this.txtVendID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtVendID_MouseDown);
            // 
            // txtNotes
            // 
            this.txtNotes.Enabled = false;
            this.txtNotes.Location = new System.Drawing.Point(521, 95);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(200, 43);
            this.txtNotes.TabIndex = 190;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(433, 95);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(35, 13);
            this.label20.TabIndex = 168;
            this.label20.Text = "Notes";
            // 
            // txtVendName
            // 
            this.txtVendName.Enabled = false;
            this.txtVendName.Location = new System.Drawing.Point(577, 17);
            this.txtVendName.Name = "txtVendName";
            this.txtVendName.Size = new System.Drawing.Size(144, 20);
            this.txtVendName.TabIndex = 185;
            this.txtVendName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtVendName_MouseDown);
            // 
            // txtNRBNum
            // 
            this.txtNRBNum.Enabled = false;
            this.txtNRBNum.Location = new System.Drawing.Point(141, 43);
            this.txtNRBNum.Name = "txtNRBNum";
            this.txtNRBNum.Size = new System.Drawing.Size(200, 20);
            this.txtNRBNum.TabIndex = 178;
            // 
            // dtNRB
            // 
            this.dtNRB.CustomFormat = "dd/MM/yyyy";
            this.dtNRB.Enabled = false;
            this.dtNRB.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNRB.Location = new System.Drawing.Point(141, 17);
            this.dtNRB.Name = "dtNRB";
            this.dtNRB.Size = new System.Drawing.Size(200, 20);
            this.dtNRB.TabIndex = 177;
            this.dtNRB.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 13);
            this.label11.TabIndex = 171;
            this.label11.Text = "RB Number";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 13);
            this.label5.TabIndex = 172;
            this.label5.Text = "Goods Receipt Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(433, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 175;
            this.label3.Text = "Vendor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 176;
            this.label2.Text = "RB Date";
            // 
            // dtGR
            // 
            this.dtGR.CustomFormat = "dd/MM/yyyy";
            this.dtGR.Enabled = false;
            this.dtGR.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtGR.Location = new System.Drawing.Point(141, 69);
            this.dtGR.Name = "dtGR";
            this.dtGR.Size = new System.Drawing.Size(90, 20);
            this.dtGR.TabIndex = 191;
            this.dtGR.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtGR.Visible = false;
            // 
            // txtGRNum
            // 
            this.txtGRNum.Enabled = false;
            this.txtGRNum.Location = new System.Drawing.Point(141, 69);
            this.txtGRNum.Name = "txtGRNum";
            this.txtGRNum.ReadOnly = true;
            this.txtGRNum.Size = new System.Drawing.Size(200, 20);
            this.txtGRNum.TabIndex = 180;
            this.txtGRNum.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtGRNum_MouseDown);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(671, 361);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 27;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // gbGV
            // 
            this.gbGV.Controls.Add(this.dgvNRB);
            this.gbGV.Location = new System.Drawing.Point(17, 171);
            this.gbGV.Name = "gbGV";
            this.gbGV.Size = new System.Drawing.Size(729, 184);
            this.gbGV.TabIndex = 33;
            this.gbGV.TabStop = false;
            // 
            // dgvNRB
            // 
            this.dgvNRB.AllowUserToAddRows = false;
            this.dgvNRB.AllowUserToDeleteRows = false;
            this.dgvNRB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNRB.Location = new System.Drawing.Point(9, 19);
            this.dgvNRB.Name = "dgvNRB";
            this.dgvNRB.Size = new System.Drawing.Size(714, 155);
            this.dgvNRB.TabIndex = 36;
            this.dgvNRB.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvNRB_CellMouseDown);
            // 
            // NRBApproval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 481);
            this.Controls.Add(this.gbMain);
            this.Name = "NRBApproval";
            this.Text = "Nota Retur Beli Approval";
            this.Load += new System.EventHandler(this.NRBApproval_Load);
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            this.gbGV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNRB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnApprove;
        private System.Windows.Forms.Button btnRevision;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox gbGV;
        private System.Windows.Forms.DataGridView dgvNRB;
        private System.Windows.Forms.TextBox txtPONum;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txtStatusName;
        internal System.Windows.Forms.TextBox txtApproved;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtSiteID;
        private System.Windows.Forms.TextBox txtSiteLocation;
        internal System.Windows.Forms.TextBox txtSiteName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtVendID;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtVendName;
        private System.Windows.Forms.TextBox txtNRBNum;
        private System.Windows.Forms.DateTimePicker dtNRB;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtGR;
        private System.Windows.Forms.TextBox txtGRNum;
        private System.Windows.Forms.ComboBox cmbJenisRetur;
        private System.Windows.Forms.Label label4;
    }
}