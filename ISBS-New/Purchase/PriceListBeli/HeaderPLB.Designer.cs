namespace ISBS_New.Purchase.PriceListBeli
{
    partial class HeaderPLB
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
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtPlbDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPLBNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dgvPLBDetails = new System.Windows.Forms.DataGridView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupPLJH = new System.Windows.Forms.GroupBox();
            this.txtNotes = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvVendorReference = new System.Windows.Forms.DataGridView();
            this.btnDeleteVendor = new System.Windows.Forms.Button();
            this.btnAddVendor = new System.Windows.Forms.Button();
            this.cmbVendorReference = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.grpCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPLBDetails)).BeginInit();
            this.groupPLJH.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendorReference)).BeginInit();
            this.grpDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.dtFrom);
            this.grpCustomer.Controls.Add(this.dtTo);
            this.grpCustomer.Controls.Add(this.label3);
            this.grpCustomer.Controls.Add(this.label4);
            this.grpCustomer.Controls.Add(this.dtPlbDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.txtPLBNumber);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(809, 75);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(525, 15);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(200, 20);
            this.dtFrom.TabIndex = 121;
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(525, 41);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(200, 20);
            this.dtTo.TabIndex = 120;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(451, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 119;
            this.label3.Text = "Valid To";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(450, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 118;
            this.label4.Text = "Valid From ";
            // 
            // dtPlbDate
            // 
            this.dtPlbDate.CustomFormat = "dd-MM-yyyy";
            this.dtPlbDate.Enabled = false;
            this.dtPlbDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPlbDate.Location = new System.Drawing.Point(125, 17);
            this.dtPlbDate.Name = "dtPlbDate";
            this.dtPlbDate.Size = new System.Drawing.Size(200, 20);
            this.dtPlbDate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "PLB Date";
            // 
            // txtPLBNumber
            // 
            this.txtPLBNumber.Enabled = false;
            this.txtPLBNumber.Location = new System.Drawing.Point(125, 39);
            this.txtPLBNumber.Name = "txtPLBNumber";
            this.txtPLBNumber.ReadOnly = true;
            this.txtPLBNumber.Size = new System.Drawing.Size(200, 20);
            this.txtPLBNumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "PLB Number";
            // 
            // dgvPLBDetails
            // 
            this.dgvPLBDetails.AllowUserToAddRows = false;
            this.dgvPLBDetails.AllowUserToDeleteRows = false;
            this.dgvPLBDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPLBDetails.Location = new System.Drawing.Point(9, 44);
            this.dgvPLBDetails.Name = "dgvPLBDetails";
            this.dgvPLBDetails.ReadOnly = true;
            this.dgvPLBDetails.Size = new System.Drawing.Size(794, 162);
            this.dgvPLBDetails.TabIndex = 11;
            this.dgvPLBDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvPLBDetails_CellBeginEdit);
            this.dgvPLBDetails.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvPLBDetails_CellMouseDown);
            this.dgvPLBDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPLBDetails_CellFormatting);
            this.dgvPLBDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPLBDetails_CellEndEdit);
            this.dgvPLBDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPLBDetails_EditingControlShowing);
            this.dgvPLBDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvPLBDetails_KeyPress);
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
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(733, 526);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupPLJH
            // 
            this.groupPLJH.Controls.Add(this.txtNotes);
            this.groupPLJH.Controls.Add(this.label2);
            this.groupPLJH.Controls.Add(this.dgvVendorReference);
            this.groupPLJH.Controls.Add(this.btnDeleteVendor);
            this.groupPLJH.Controls.Add(this.btnAddVendor);
            this.groupPLJH.Controls.Add(this.cmbVendorReference);
            this.groupPLJH.Controls.Add(this.label1);
            this.groupPLJH.Controls.Add(this.grpCustomer);
            this.groupPLJH.Controls.Add(this.btnCancel);
            this.groupPLJH.Controls.Add(this.btnEdit);
            this.groupPLJH.Controls.Add(this.btnSave);
            this.groupPLJH.Controls.Add(this.grpDetail);
            this.groupPLJH.Controls.Add(this.btnExit);
            this.groupPLJH.Location = new System.Drawing.Point(12, 54);
            this.groupPLJH.Name = "groupPLJH";
            this.groupPLJH.Size = new System.Drawing.Size(823, 557);
            this.groupPLJH.TabIndex = 150;
            this.groupPLJH.TabStop = false;
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(142, 460);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(255, 62);
            this.txtNotes.TabIndex = 152;
            this.txtNotes.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 481);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 151;
            this.label2.Text = "Notes";
            // 
            // dgvVendorReference
            // 
            this.dgvVendorReference.AllowUserToAddRows = false;
            this.dgvVendorReference.AllowUserToDeleteRows = false;
            this.dgvVendorReference.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVendorReference.Location = new System.Drawing.Point(18, 339);
            this.dgvVendorReference.Name = "dgvVendorReference";
            this.dgvVendorReference.ReadOnly = true;
            this.dgvVendorReference.Size = new System.Drawing.Size(791, 110);
            this.dgvVendorReference.TabIndex = 150;
            // 
            // btnDeleteVendor
            // 
            this.btnDeleteVendor.Location = new System.Drawing.Point(484, 310);
            this.btnDeleteVendor.Name = "btnDeleteVendor";
            this.btnDeleteVendor.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteVendor.TabIndex = 149;
            this.btnDeleteVendor.Text = "Delete";
            this.btnDeleteVendor.UseVisualStyleBackColor = true;
            this.btnDeleteVendor.Click += new System.EventHandler(this.btnDeleteVendor_Click);
            // 
            // btnAddVendor
            // 
            this.btnAddVendor.Location = new System.Drawing.Point(403, 310);
            this.btnAddVendor.Name = "btnAddVendor";
            this.btnAddVendor.Size = new System.Drawing.Size(75, 23);
            this.btnAddVendor.TabIndex = 148;
            this.btnAddVendor.Text = "Add";
            this.btnAddVendor.UseVisualStyleBackColor = true;
            this.btnAddVendor.Click += new System.EventHandler(this.btnAddVendor_Click);
            // 
            // cmbVendorReference
            // 
            this.cmbVendorReference.FormattingEnabled = true;
            this.cmbVendorReference.Location = new System.Drawing.Point(142, 312);
            this.cmbVendorReference.Name = "cmbVendorReference";
            this.cmbVendorReference.Size = new System.Drawing.Size(255, 21);
            this.cmbVendorReference.TabIndex = 147;
            this.cmbVendorReference.SelectedIndexChanged += new System.EventHandler(this.VendorReference_SelectedIndexChange);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 314);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 122;
            this.label1.Text = "Vendor Reference Type";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(647, 526);
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
            this.btnEdit.Location = new System.Drawing.Point(476, 526);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(561, 526);
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
            this.grpDetail.Controls.Add(this.dgvPLBDetails);
            this.grpDetail.Location = new System.Drawing.Point(6, 89);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(810, 216);
            this.grpDetail.TabIndex = 142;
            this.grpDetail.TabStop = false;
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
            // HeaderPLB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(846, 618);
            this.Controls.Add(this.groupPLJH);
            this.Name = "HeaderPLB";
            this.Resizable = false;
            this.Text = "Purchase Price List";
            this.Load += new System.EventHandler(this.HeaderPLB2_Load);
            this.Shown += new System.EventHandler(this.HeaderPLB2_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HeaderPLB2_FormClosed);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPLBDetails)).EndInit();
            this.groupPLJH.ResumeLayout(false);
            this.groupPLJH.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendorReference)).EndInit();
            this.grpDetail.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.DateTimePicker dtPlbDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtPLBNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.DataGridView dgvPLBDetails;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupPLJH;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button btnDeleteVendor;
        internal System.Windows.Forms.Button btnAddVendor;
        private System.Windows.Forms.ComboBox cmbVendorReference;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtNotes;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvVendorReference;

    }
}
