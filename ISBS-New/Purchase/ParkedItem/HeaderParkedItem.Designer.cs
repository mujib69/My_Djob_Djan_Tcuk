namespace ISBS_New.Purchase.ParkedItem
{
    partial class HeaderParkedItem
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
            this.groupPLJH = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvAttachment = new System.Windows.Forms.DataGridView();
            this.btnDeleteFile = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.txtVendorID = new System.Windows.Forms.TextBox();
            this.cmbAction = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtGoodsReceivedNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtNotaDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtNotaNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvParkedItemDetails = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupPLJH.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttachment)).BeginInit();
            this.grpCustomer.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParkedItemDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // groupPLJH
            // 
            this.groupPLJH.Controls.Add(this.groupBox1);
            this.groupPLJH.Controls.Add(this.grpCustomer);
            this.groupPLJH.Controls.Add(this.btnCancel);
            this.groupPLJH.Controls.Add(this.btnEdit);
            this.groupPLJH.Controls.Add(this.btnSave);
            this.groupPLJH.Controls.Add(this.grpDetail);
            this.groupPLJH.Controls.Add(this.btnExit);
            this.groupPLJH.Location = new System.Drawing.Point(11, 57);
            this.groupPLJH.Name = "groupPLJH";
            this.groupPLJH.Size = new System.Drawing.Size(823, 586);
            this.groupPLJH.TabIndex = 152;
            this.groupPLJH.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvAttachment);
            this.groupBox1.Controls.Add(this.btnDeleteFile);
            this.groupBox1.Controls.Add(this.btnDownload);
            this.groupBox1.Controls.Add(this.btnUpload);
            this.groupBox1.Location = new System.Drawing.Point(6, 360);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(809, 165);
            this.groupBox1.TabIndex = 153;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Attached";
            // 
            // dgvAttachment
            // 
            this.dgvAttachment.AllowUserToAddRows = false;
            this.dgvAttachment.AllowUserToDeleteRows = false;
            this.dgvAttachment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttachment.Location = new System.Drawing.Point(9, 19);
            this.dgvAttachment.Name = "dgvAttachment";
            this.dgvAttachment.ReadOnly = true;
            this.dgvAttachment.Size = new System.Drawing.Size(794, 110);
            this.dgvAttachment.TabIndex = 150;
            // 
            // btnDeleteFile
            // 
            this.btnDeleteFile.Location = new System.Drawing.Point(731, 135);
            this.btnDeleteFile.Name = "btnDeleteFile";
            this.btnDeleteFile.Size = new System.Drawing.Size(72, 23);
            this.btnDeleteFile.TabIndex = 152;
            this.btnDeleteFile.Text = "Delete";
            this.btnDeleteFile.UseVisualStyleBackColor = true;
            this.btnDeleteFile.Click += new System.EventHandler(this.btnDeleteFile_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(654, 135);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(68, 23);
            this.btnDownload.TabIndex = 151;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(575, 135);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(70, 23);
            this.btnUpload.TabIndex = 150;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.txtVendorName);
            this.grpCustomer.Controls.Add(this.txtVendorID);
            this.grpCustomer.Controls.Add(this.cmbAction);
            this.grpCustomer.Controls.Add(this.label2);
            this.grpCustomer.Controls.Add(this.txtGoodsReceivedNumber);
            this.grpCustomer.Controls.Add(this.label1);
            this.grpCustomer.Controls.Add(this.label3);
            this.grpCustomer.Controls.Add(this.label4);
            this.grpCustomer.Controls.Add(this.dtNotaDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.txtNotaNumber);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(809, 121);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Header";
            // 
            // txtVendorName
            // 
            this.txtVendorName.Enabled = false;
            this.txtVendorName.Location = new System.Drawing.Point(552, 39);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(200, 20);
            this.txtVendorName.TabIndex = 127;
            // 
            // txtVendorID
            // 
            this.txtVendorID.Enabled = false;
            this.txtVendorID.Location = new System.Drawing.Point(552, 16);
            this.txtVendorID.Name = "txtVendorID";
            this.txtVendorID.ReadOnly = true;
            this.txtVendorID.Size = new System.Drawing.Size(200, 20);
            this.txtVendorID.TabIndex = 126;
            // 
            // cmbAction
            // 
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Location = new System.Drawing.Point(139, 84);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(200, 21);
            this.cmbAction.TabIndex = 125;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 124;
            this.label2.Text = "Action";
            // 
            // txtGoodsReceivedNumber
            // 
            this.txtGoodsReceivedNumber.Enabled = false;
            this.txtGoodsReceivedNumber.Location = new System.Drawing.Point(139, 61);
            this.txtGoodsReceivedNumber.Name = "txtGoodsReceivedNumber";
            this.txtGoodsReceivedNumber.ReadOnly = true;
            this.txtGoodsReceivedNumber.Size = new System.Drawing.Size(200, 20);
            this.txtGoodsReceivedNumber.TabIndex = 123;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 122;
            this.label1.Text = "Goods Received Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(451, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 119;
            this.label3.Text = "Vendor Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(450, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 118;
            this.label4.Text = "Vendor ID";
            // 
            // dtNotaDate
            // 
            this.dtNotaDate.CustomFormat = "dd-MM-yyyy";
            this.dtNotaDate.Enabled = false;
            this.dtNotaDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNotaDate.Location = new System.Drawing.Point(139, 17);
            this.dtNotaDate.Name = "dtNotaDate";
            this.dtNotaDate.Size = new System.Drawing.Size(200, 20);
            this.dtNotaDate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "Nota Date";
            // 
            // txtNotaNumber
            // 
            this.txtNotaNumber.Enabled = false;
            this.txtNotaNumber.Location = new System.Drawing.Point(139, 39);
            this.txtNotaNumber.Name = "txtNotaNumber";
            this.txtNotaNumber.ReadOnly = true;
            this.txtNotaNumber.Size = new System.Drawing.Size(200, 20);
            this.txtNotaNumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Nota Number";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(649, 554);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 144;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(478, 554);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(563, 554);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 143;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.btnNew);
            this.grpDetail.Controls.Add(this.btnDelete);
            this.grpDetail.Controls.Add(this.dgvParkedItemDetails);
            this.grpDetail.Location = new System.Drawing.Point(6, 137);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(810, 216);
            this.grpDetail.TabIndex = 142;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(9, 15);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(59, 23);
            this.btnNew.TabIndex = 149;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(75, 15);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(59, 23);
            this.btnDelete.TabIndex = 148;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvParkedItemDetails
            // 
            this.dgvParkedItemDetails.AllowUserToAddRows = false;
            this.dgvParkedItemDetails.AllowUserToDeleteRows = false;
            this.dgvParkedItemDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParkedItemDetails.Location = new System.Drawing.Point(9, 44);
            this.dgvParkedItemDetails.Name = "dgvParkedItemDetails";
            this.dgvParkedItemDetails.ReadOnly = true;
            this.dgvParkedItemDetails.Size = new System.Drawing.Size(794, 162);
            this.dgvParkedItemDetails.TabIndex = 11;
            this.dgvParkedItemDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvParkedItemDetails_CellFormatting);
            this.dgvParkedItemDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvParkedItemDetails_EditingControlShowing);
            this.dgvParkedItemDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvParkedItemDetails_KeyPress);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(735, 554);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // HeaderParkedItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(854, 655);
            this.Controls.Add(this.groupPLJH);
            this.Name = "HeaderParkedItem";
            this.Resizable = false;
            this.Text = "Header Parked Item";
            this.Load += new System.EventHandler(this.HeaderParkedItem_Load);
            this.Shown += new System.EventHandler(this.HeaderParkedItem_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HeaderParkedItem_FormClosed);
            this.groupPLJH.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttachment)).EndInit();
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParkedItemDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupPLJH;
        internal System.Windows.Forms.GroupBox grpCustomer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.DateTimePicker dtNotaDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtNotaNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.DataGridView dgvParkedItemDetails;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ComboBox cmbAction;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox txtGoodsReceivedNumber;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txtVendorName;
        internal System.Windows.Forms.TextBox txtVendorID;
        internal System.Windows.Forms.Button btnDownload;
        internal System.Windows.Forms.Button btnUpload;
        internal System.Windows.Forms.Button btnDeleteFile;
        private System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.DataGridView dgvAttachment;

    }
}
