namespace ISBS_New.Purchase.Retur.ReturTukarBarang
{
    partial class RTBHeader
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtVendName = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSearchGR = new System.Windows.Forms.Button();
            this.txtGRNum = new System.Windows.Forms.TextBox();
            this.txtRTBNum = new System.Windows.Forms.TextBox();
            this.dtRTB = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSiteID = new System.Windows.Forms.TextBox();
            this.txtSiteLocation = new System.Windows.Forms.TextBox();
            this.txtSiteName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtVendID = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.txtStatusName = new System.Windows.Forms.TextBox();
            this.txtApproved = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbGV = new System.Windows.Forms.GroupBox();
            this.dgvRTB = new System.Windows.Forms.DataGridView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.gbMain.SuspendLayout();
            this.gbGV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRTB)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(422, 371);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 24;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(503, 371);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 25;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtVendName
            // 
            this.txtVendName.Enabled = false;
            this.txtVendName.Location = new System.Drawing.Point(575, 19);
            this.txtVendName.Name = "txtVendName";
            this.txtVendName.Size = new System.Drawing.Size(144, 20);
            this.txtVendName.TabIndex = 17;
            // 
            // btnNew
            // 
            this.btnNew.Enabled = false;
            this.btnNew.Location = new System.Drawing.Point(9, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 9;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSearchGR
            // 
            this.btnSearchGR.Enabled = false;
            this.btnSearchGR.Location = new System.Drawing.Point(343, 78);
            this.btnSearchGR.Name = "btnSearchGR";
            this.btnSearchGR.Size = new System.Drawing.Size(23, 20);
            this.btnSearchGR.TabIndex = 1;
            this.btnSearchGR.Text = "...";
            this.btnSearchGR.UseVisualStyleBackColor = true;
            this.btnSearchGR.Click += new System.EventHandler(this.btnSearchGR_Click);
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
            // gbMain
            // 
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
            this.gbMain.Controls.Add(this.btnCancel);
            this.gbMain.Controls.Add(this.gbGV);
            this.gbMain.Controls.Add(this.btnEdit);
            this.gbMain.Controls.Add(this.btnSave);
            this.gbMain.Controls.Add(this.txtVendName);
            this.gbMain.Controls.Add(this.btnSearchGR);
            this.gbMain.Controls.Add(this.txtGRNum);
            this.gbMain.Controls.Add(this.txtRTBNum);
            this.gbMain.Controls.Add(this.dtRTB);
            this.gbMain.Controls.Add(this.label11);
            this.gbMain.Controls.Add(this.label5);
            this.gbMain.Controls.Add(this.label3);
            this.gbMain.Controls.Add(this.label2);
            this.gbMain.Location = new System.Drawing.Point(23, 55);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(766, 408);
            this.gbMain.TabIndex = 42;
            this.gbMain.TabStop = false;
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
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(584, 371);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 26;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbGV
            // 
            this.gbGV.Controls.Add(this.dgvRTB);
            this.gbGV.Controls.Add(this.btnDelete);
            this.gbGV.Controls.Add(this.btnNew);
            this.gbGV.Location = new System.Drawing.Point(17, 166);
            this.gbGV.Name = "gbGV";
            this.gbGV.Size = new System.Drawing.Size(729, 199);
            this.gbGV.TabIndex = 33;
            this.gbGV.TabStop = false;
            // 
            // dgvRTB
            // 
            this.dgvRTB.AllowUserToAddRows = false;
            this.dgvRTB.AllowUserToDeleteRows = false;
            this.dgvRTB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRTB.Location = new System.Drawing.Point(9, 41);
            this.dgvRTB.Name = "dgvRTB";
            this.dgvRTB.Size = new System.Drawing.Size(714, 155);
            this.dgvRTB.TabIndex = 36;
            this.dgvRTB.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRTB_CellEndEdit);
            this.dgvRTB.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvRTB_EditingControlShowing);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(90, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // RTBHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 475);
            this.Controls.Add(this.gbMain);
            this.Name = "RTBHeader";
            this.Text = "Header Retur Tukar Barang";
            this.Load += new System.EventHandler(this.RTBHeader_Load);
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            this.gbGV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRTB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtVendName;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSearchGR;
        private System.Windows.Forms.TextBox txtGRNum;
        private System.Windows.Forms.TextBox txtRTBNum;
        private System.Windows.Forms.DateTimePicker dtRTB;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox txtSiteID;
        private System.Windows.Forms.TextBox txtSiteLocation;
        internal System.Windows.Forms.TextBox txtSiteName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtVendID;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbGV;
        private System.Windows.Forms.DataGridView dgvRTB;
        private System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.TextBox txtStatusName;
        internal System.Windows.Forms.TextBox txtApproved;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Label label8;
    }
}