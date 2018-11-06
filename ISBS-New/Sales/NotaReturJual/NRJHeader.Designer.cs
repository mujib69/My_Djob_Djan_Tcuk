namespace ISBS_New.Sales.NotaReturJual
{
    partial class NRJHeader
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
            this.txtSONum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearchWH = new System.Windows.Forms.Button();
            this.txtStatusName = new System.Windows.Forms.TextBox();
            this.txtApproved = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSiteID = new System.Windows.Forms.TextBox();
            this.txtSiteLocation = new System.Windows.Forms.TextBox();
            this.txtSiteName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtCustID = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbGV = new System.Windows.Forms.GroupBox();
            this.dgvNRJ = new System.Windows.Forms.DataGridView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtCustName = new System.Windows.Forms.TextBox();
            this.btnSearchGI = new System.Windows.Forms.Button();
            this.txtNRJNum = new System.Windows.Forms.TextBox();
            this.dtNRJ = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtGI = new System.Windows.Forms.DateTimePicker();
            this.txtGINum = new System.Windows.Forms.TextBox();
            this.gbMain.SuspendLayout();
            this.gbGV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNRJ)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMain
            // 
            this.gbMain.Controls.Add(this.cmbJenisRetur);
            this.gbMain.Controls.Add(this.label4);
            this.gbMain.Controls.Add(this.txtSONum);
            this.gbMain.Controls.Add(this.label1);
            this.gbMain.Controls.Add(this.btnSearchWH);
            this.gbMain.Controls.Add(this.txtStatusName);
            this.gbMain.Controls.Add(this.txtApproved);
            this.gbMain.Controls.Add(this.label7);
            this.gbMain.Controls.Add(this.label8);
            this.gbMain.Controls.Add(this.txtSiteID);
            this.gbMain.Controls.Add(this.txtSiteLocation);
            this.gbMain.Controls.Add(this.txtSiteName);
            this.gbMain.Controls.Add(this.label21);
            this.gbMain.Controls.Add(this.txtCustID);
            this.gbMain.Controls.Add(this.txtNotes);
            this.gbMain.Controls.Add(this.label20);
            this.gbMain.Controls.Add(this.btnExit);
            this.gbMain.Controls.Add(this.btnCancel);
            this.gbMain.Controls.Add(this.gbGV);
            this.gbMain.Controls.Add(this.btnEdit);
            this.gbMain.Controls.Add(this.btnSave);
            this.gbMain.Controls.Add(this.txtCustName);
            this.gbMain.Controls.Add(this.btnSearchGI);
            this.gbMain.Controls.Add(this.txtNRJNum);
            this.gbMain.Controls.Add(this.dtNRJ);
            this.gbMain.Controls.Add(this.label11);
            this.gbMain.Controls.Add(this.label5);
            this.gbMain.Controls.Add(this.label3);
            this.gbMain.Controls.Add(this.label2);
            this.gbMain.Controls.Add(this.dtGI);
            this.gbMain.Controls.Add(this.txtGINum);
            this.gbMain.Location = new System.Drawing.Point(23, 54);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(766, 433);
            this.gbMain.TabIndex = 44;
            this.gbMain.TabStop = false;
            // 
            // cmbJenisRetur
            // 
            this.cmbJenisRetur.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJenisRetur.FormattingEnabled = true;
            this.cmbJenisRetur.Location = new System.Drawing.Point(519, 149);
            this.cmbJenisRetur.Name = "cmbJenisRetur";
            this.cmbJenisRetur.Size = new System.Drawing.Size(200, 21);
            this.cmbJenisRetur.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(431, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Jenis Retur";
            // 
            // txtSONum
            // 
            this.txtSONum.Enabled = false;
            this.txtSONum.Location = new System.Drawing.Point(139, 100);
            this.txtSONum.Name = "txtSONum";
            this.txtSONum.ReadOnly = true;
            this.txtSONum.Size = new System.Drawing.Size(200, 20);
            this.txtSONum.TabIndex = 5;
            this.txtSONum.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtSONum_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SO Number";
            // 
            // btnSearchWH
            // 
            this.btnSearchWH.Enabled = false;
            this.btnSearchWH.Location = new System.Drawing.Point(725, 48);
            this.btnSearchWH.Name = "btnSearchWH";
            this.btnSearchWH.Size = new System.Drawing.Size(23, 20);
            this.btnSearchWH.TabIndex = 10;
            this.btnSearchWH.Text = "...";
            this.btnSearchWH.UseVisualStyleBackColor = true;
            this.btnSearchWH.Click += new System.EventHandler(this.btnSearchWH_Click);
            // 
            // txtStatusName
            // 
            this.txtStatusName.Enabled = false;
            this.txtStatusName.Location = new System.Drawing.Point(139, 126);
            this.txtStatusName.Name = "txtStatusName";
            this.txtStatusName.ReadOnly = true;
            this.txtStatusName.Size = new System.Drawing.Size(200, 20);
            this.txtStatusName.TabIndex = 6;
            // 
            // txtApproved
            // 
            this.txtApproved.Enabled = false;
            this.txtApproved.Location = new System.Drawing.Point(139, 152);
            this.txtApproved.Name = "txtApproved";
            this.txtApproved.ReadOnly = true;
            this.txtApproved.Size = new System.Drawing.Size(200, 20);
            this.txtApproved.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "RJ Approved :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 129);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "RJ Status :";
            // 
            // txtSiteID
            // 
            this.txtSiteID.Enabled = false;
            this.txtSiteID.Location = new System.Drawing.Point(519, 48);
            this.txtSiteID.Name = "txtSiteID";
            this.txtSiteID.ReadOnly = true;
            this.txtSiteID.Size = new System.Drawing.Size(36, 20);
            this.txtSiteID.TabIndex = 11;
            this.txtSiteID.Visible = false;
            // 
            // txtSiteLocation
            // 
            this.txtSiteLocation.Enabled = false;
            this.txtSiteLocation.Location = new System.Drawing.Point(519, 74);
            this.txtSiteLocation.Name = "txtSiteLocation";
            this.txtSiteLocation.Size = new System.Drawing.Size(200, 20);
            this.txtSiteLocation.TabIndex = 13;
            // 
            // txtSiteName
            // 
            this.txtSiteName.Enabled = false;
            this.txtSiteName.Location = new System.Drawing.Point(519, 48);
            this.txtSiteName.Name = "txtSiteName";
            this.txtSiteName.Size = new System.Drawing.Size(200, 20);
            this.txtSiteName.TabIndex = 12;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(431, 52);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(62, 13);
            this.label21.TabIndex = 0;
            this.label21.Text = "Warehouse";
            // 
            // txtCustID
            // 
            this.txtCustID.Enabled = false;
            this.txtCustID.Location = new System.Drawing.Point(519, 22);
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.Size = new System.Drawing.Size(50, 20);
            this.txtCustID.TabIndex = 8;
            this.txtCustID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtCustID_MouseDown);
            // 
            // txtNotes
            // 
            this.txtNotes.Enabled = false;
            this.txtNotes.Location = new System.Drawing.Point(519, 100);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(200, 43);
            this.txtNotes.TabIndex = 14;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(431, 100);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(35, 13);
            this.label20.TabIndex = 0;
            this.label20.Text = "Notes";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(665, 384);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 19;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(584, 384);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbGV
            // 
            this.gbGV.Controls.Add(this.dgvNRJ);
            this.gbGV.Controls.Add(this.btnDelete);
            this.gbGV.Controls.Add(this.btnNew);
            this.gbGV.Location = new System.Drawing.Point(17, 179);
            this.gbGV.Name = "gbGV";
            this.gbGV.Size = new System.Drawing.Size(729, 199);
            this.gbGV.TabIndex = 15;
            this.gbGV.TabStop = false;
            // 
            // dgvNRJ
            // 
            this.dgvNRJ.AllowUserToAddRows = false;
            this.dgvNRJ.AllowUserToDeleteRows = false;
            this.dgvNRJ.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNRJ.Location = new System.Drawing.Point(9, 41);
            this.dgvNRJ.Name = "dgvNRJ";
            this.dgvNRJ.Size = new System.Drawing.Size(714, 155);
            this.dgvNRJ.TabIndex = 2;
            this.dgvNRJ.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvNRJ_CellMouseDown);
            this.dgvNRJ.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNRJ_CellEndEdit);
            this.dgvNRJ.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvNRJ_EditingControlShowing);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(90, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.Enabled = false;
            this.btnNew.Location = new System.Drawing.Point(9, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(422, 384);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 16;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(503, 384);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtCustName
            // 
            this.txtCustName.Enabled = false;
            this.txtCustName.Location = new System.Drawing.Point(575, 22);
            this.txtCustName.Name = "txtCustName";
            this.txtCustName.Size = new System.Drawing.Size(144, 20);
            this.txtCustName.TabIndex = 9;
            this.txtCustName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtCustName_MouseDown);
            // 
            // btnSearchGI
            // 
            this.btnSearchGI.Enabled = false;
            this.btnSearchGI.Location = new System.Drawing.Point(343, 74);
            this.btnSearchGI.Name = "btnSearchGI";
            this.btnSearchGI.Size = new System.Drawing.Size(23, 20);
            this.btnSearchGI.TabIndex = 3;
            this.btnSearchGI.Text = "...";
            this.btnSearchGI.UseVisualStyleBackColor = true;
            this.btnSearchGI.Click += new System.EventHandler(this.btnSearchGI_Click);
            // 
            // txtNRJNum
            // 
            this.txtNRJNum.Enabled = false;
            this.txtNRJNum.Location = new System.Drawing.Point(139, 48);
            this.txtNRJNum.Name = "txtNRJNum";
            this.txtNRJNum.Size = new System.Drawing.Size(200, 20);
            this.txtNRJNum.TabIndex = 2;
            // 
            // dtNRJ
            // 
            this.dtNRJ.CustomFormat = "dd/MM/yyyy";
            this.dtNRJ.Enabled = false;
            this.dtNRJ.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNRJ.Location = new System.Drawing.Point(139, 22);
            this.dtNRJ.Name = "dtNRJ";
            this.dtNRJ.Size = new System.Drawing.Size(200, 20);
            this.dtNRJ.TabIndex = 1;
            this.dtNRJ.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "RJ Number";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "BBK Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(431, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Customer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "RJ Date";
            // 
            // dtGI
            // 
            this.dtGI.CustomFormat = "dd/MM/yyyy";
            this.dtGI.Enabled = false;
            this.dtGI.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtGI.Location = new System.Drawing.Point(139, 74);
            this.dtGI.Name = "dtGI";
            this.dtGI.Size = new System.Drawing.Size(90, 20);
            this.dtGI.TabIndex = 20;
            this.dtGI.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtGI.Visible = false;
            // 
            // txtGINum
            // 
            this.txtGINum.Enabled = false;
            this.txtGINum.Location = new System.Drawing.Point(139, 74);
            this.txtGINum.Name = "txtGINum";
            this.txtGINum.ReadOnly = true;
            this.txtGINum.Size = new System.Drawing.Size(200, 20);
            this.txtGINum.TabIndex = 4;
            this.txtGINum.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtGINum_MouseDown);
            // 
            // NRJHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 510);
            this.Controls.Add(this.gbMain);
            this.Name = "NRJHeader";
            this.Text = "Retur Jual Header";
            this.Load += new System.EventHandler(this.NRJHeader_Load);
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            this.gbGV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNRJ)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.TextBox txtSONum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearchWH;
        internal System.Windows.Forms.TextBox txtStatusName;
        internal System.Windows.Forms.TextBox txtApproved;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtSiteID;
        private System.Windows.Forms.TextBox txtSiteLocation;
        internal System.Windows.Forms.TextBox txtSiteName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtCustID;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbGV;
        private System.Windows.Forms.DataGridView dgvNRJ;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtCustName;
        private System.Windows.Forms.Button btnSearchGI;
        private System.Windows.Forms.TextBox txtNRJNum;
        private System.Windows.Forms.DateTimePicker dtNRJ;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtGI;
        private System.Windows.Forms.TextBox txtGINum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbJenisRetur;
    }
}