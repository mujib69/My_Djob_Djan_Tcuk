namespace ISBS_New.Purchase.RFQ
{
    partial class RFQForm
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
            this.btnDeleteV = new System.Windows.Forms.Button();
            this.btnNewV = new System.Windows.Forms.Button();
            this.dgvVendor = new System.Windows.Forms.DataGridView();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.txtTransType = new System.Windows.Forms.TextBox();
            this.btnSearchPurchReqID = new System.Windows.Forms.Button();
            this.txtPurchReqID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearchVendor = new System.Windows.Forms.Button();
            this.txtVendorID = new System.Windows.Forms.TextBox();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dtRFQDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRFQID = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.grp = new System.Windows.Forms.GroupBox();
            this.dgvDetails = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
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
            this.groupRFQ.Controls.Add(this.btnCancel);
            this.groupRFQ.Controls.Add(this.btnEdit);
            this.groupRFQ.Controls.Add(this.grp);
            this.groupRFQ.Controls.Add(this.btnExit);
            this.groupRFQ.Controls.Add(this.btnSave);
            this.groupRFQ.Location = new System.Drawing.Point(3, 55);
            this.groupRFQ.Name = "groupRFQ";
            this.groupRFQ.Size = new System.Drawing.Size(834, 571);
            this.groupRFQ.TabIndex = 149;
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
            this.grpVendor.Controls.Add(this.btnDeleteV);
            this.grpVendor.Controls.Add(this.btnNewV);
            this.grpVendor.Controls.Add(this.dgvVendor);
            this.grpVendor.Location = new System.Drawing.Point(6, 9);
            this.grpVendor.Name = "grpVendor";
            this.grpVendor.Size = new System.Drawing.Size(445, 161);
            this.grpVendor.TabIndex = 147;
            this.grpVendor.TabStop = false;
            this.grpVendor.Text = "Vendor";
            // 
            // btnDeleteV
            // 
            this.btnDeleteV.Enabled = false;
            this.btnDeleteV.Location = new System.Drawing.Point(74, 16);
            this.btnDeleteV.Name = "btnDeleteV";
            this.btnDeleteV.Size = new System.Drawing.Size(59, 23);
            this.btnDeleteV.TabIndex = 8;
            this.btnDeleteV.Text = "Delete";
            this.btnDeleteV.UseVisualStyleBackColor = true;
            this.btnDeleteV.Click += new System.EventHandler(this.btnDeleteV_Click);
            // 
            // btnNewV
            // 
            this.btnNewV.Enabled = false;
            this.btnNewV.Location = new System.Drawing.Point(8, 16);
            this.btnNewV.Name = "btnNewV";
            this.btnNewV.Size = new System.Drawing.Size(59, 23);
            this.btnNewV.TabIndex = 7;
            this.btnNewV.Text = "New";
            this.btnNewV.UseVisualStyleBackColor = true;
            this.btnNewV.Click += new System.EventHandler(this.btnNewV_Click);
            // 
            // dgvVendor
            // 
            this.dgvVendor.AllowUserToAddRows = false;
            this.dgvVendor.AllowUserToDeleteRows = false;
            this.dgvVendor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVendor.Location = new System.Drawing.Point(8, 44);
            this.dgvVendor.Name = "dgvVendor";
            this.dgvVendor.ReadOnly = true;
            this.dgvVendor.Size = new System.Drawing.Size(427, 104);
            this.dgvVendor.TabIndex = 0;
            this.dgvVendor.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvVendor_CellMouseDown);
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.txtTransType);
            this.grpCustomer.Controls.Add(this.btnSearchPurchReqID);
            this.grpCustomer.Controls.Add(this.txtPurchReqID);
            this.grpCustomer.Controls.Add(this.label1);
            this.grpCustomer.Controls.Add(this.btnSearchVendor);
            this.grpCustomer.Controls.Add(this.txtVendorID);
            this.grpCustomer.Controls.Add(this.txtVendorName);
            this.grpCustomer.Controls.Add(this.label5);
            this.grpCustomer.Controls.Add(this.label7);
            this.grpCustomer.Controls.Add(this.dtRFQDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.txtRFQID);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(809, 91);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Header";
            // 
            // txtTransType
            // 
            this.txtTransType.Location = new System.Drawing.Point(348, 38);
            this.txtTransType.Name = "txtTransType";
            this.txtTransType.ReadOnly = true;
            this.txtTransType.Size = new System.Drawing.Size(100, 20);
            this.txtTransType.TabIndex = 136;
            this.txtTransType.Visible = false;
            // 
            // btnSearchPurchReqID
            // 
            this.btnSearchPurchReqID.Enabled = false;
            this.btnSearchPurchReqID.Location = new System.Drawing.Point(346, 62);
            this.btnSearchPurchReqID.Name = "btnSearchPurchReqID";
            this.btnSearchPurchReqID.Size = new System.Drawing.Size(36, 23);
            this.btnSearchPurchReqID.TabIndex = 134;
            this.btnSearchPurchReqID.Text = "...";
            this.btnSearchPurchReqID.UseVisualStyleBackColor = true;
            this.btnSearchPurchReqID.Click += new System.EventHandler(this.btnSearchPurchReqID_Click);
            // 
            // txtPurchReqID
            // 
            this.txtPurchReqID.Enabled = false;
            this.txtPurchReqID.Location = new System.Drawing.Point(139, 63);
            this.txtPurchReqID.MaxLength = 25;
            this.txtPurchReqID.Name = "txtPurchReqID";
            this.txtPurchReqID.ReadOnly = true;
            this.txtPurchReqID.Size = new System.Drawing.Size(201, 20);
            this.txtPurchReqID.TabIndex = 133;
            this.txtPurchReqID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtPurchReqID_MouseDown);
            this.txtPurchReqID.Validating += new System.ComponentModel.CancelEventHandler(this.txtPurchReqID_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 135;
            this.label1.Text = "PR No";
            // 
            // btnSearchVendor
            // 
            this.btnSearchVendor.Enabled = false;
            this.btnSearchVendor.Location = new System.Drawing.Point(764, 12);
            this.btnSearchVendor.Name = "btnSearchVendor";
            this.btnSearchVendor.Size = new System.Drawing.Size(36, 23);
            this.btnSearchVendor.TabIndex = 3;
            this.btnSearchVendor.Text = "...";
            this.btnSearchVendor.UseVisualStyleBackColor = true;
            this.btnSearchVendor.Click += new System.EventHandler(this.btnSearchVendor_Click);
            // 
            // txtVendorID
            // 
            this.txtVendorID.Enabled = false;
            this.txtVendorID.Location = new System.Drawing.Point(558, 13);
            this.txtVendorID.MaxLength = 8;
            this.txtVendorID.Name = "txtVendorID";
            this.txtVendorID.ReadOnly = true;
            this.txtVendorID.Size = new System.Drawing.Size(200, 20);
            this.txtVendorID.TabIndex = 2;
            this.txtVendorID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtVendorID_MouseDown);
            this.txtVendorID.Validating += new System.ComponentModel.CancelEventHandler(this.txtVendorID_Validating);
            // 
            // txtVendorName
            // 
            this.txtVendorName.Enabled = false;
            this.txtVendorName.Location = new System.Drawing.Point(558, 38);
            this.txtVendorName.MaxLength = 100;
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(246, 20);
            this.txtVendorName.TabIndex = 4;
            this.txtVendorName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtVendorName_MouseDown);
            this.txtVendorName.Validating += new System.ComponentModel.CancelEventHandler(this.txtVendorName_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(454, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 127;
            this.label5.Text = "Vendor Name:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(454, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 118;
            this.label7.Text = "Vendor ID:";
            // 
            // dtRFQDate
            // 
            this.dtRFQDate.CustomFormat = "dd-MM-yyyy";
            this.dtRFQDate.Enabled = false;
            this.dtRFQDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtRFQDate.Location = new System.Drawing.Point(139, 13);
            this.dtRFQDate.Name = "dtRFQDate";
            this.dtRFQDate.Size = new System.Drawing.Size(200, 20);
            this.dtRFQDate.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "RFQ No:";
            // 
            // txtRFQID
            // 
            this.txtRFQID.Enabled = false;
            this.txtRFQID.Location = new System.Drawing.Point(139, 38);
            this.txtRFQID.Name = "txtRFQID";
            this.txtRFQID.ReadOnly = true;
            this.txtRFQID.Size = new System.Drawing.Size(200, 20);
            this.txtRFQID.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "RFQ Date";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(658, 535);
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
            this.btnEdit.Location = new System.Drawing.Point(494, 535);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
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
            this.dgvDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvDetails_CellBeginEdit);
            this.dgvDetails.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvDetails_MouseClick);
            this.dgvDetails.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvDetails_CellMouseDown);
            this.dgvDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvDetails_CellFormatting);
            this.dgvDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetails_CellEndEdit);
            this.dgvDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetails_CellClick);
            this.dgvDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvDetails_EditingControlShowing);
            this.dgvDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvDetails_KeyPress);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(744, 535);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(577, 535);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 143;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // RFQForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 634);
            this.Controls.Add(this.groupRFQ);
            this.Name = "RFQForm";
            this.Resizable = false;
            this.Text = "Form Request For Quotation";
            this.Load += new System.EventHandler(this.RFQForm_Load);
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
        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.TextBox txtVendorName;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.DateTimePicker dtRFQDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtRFQID;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpVendor;
        private System.Windows.Forms.DataGridView dgvVendor;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grp;
        internal System.Windows.Forms.DataGridView dgvDetails;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.TextBox txtVendorID;
        internal System.Windows.Forms.Button btnNewV;
        internal System.Windows.Forms.Button btnDeleteV;
        internal System.Windows.Forms.Button btnSearchVendor;
        private System.Windows.Forms.GroupBox grpVN;
        internal System.Windows.Forms.Button btnSearchPurchReqID;
        internal System.Windows.Forms.TextBox txtPurchReqID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTransType;
        private System.Windows.Forms.GroupBox grpNotes;
        private System.Windows.Forms.RichTextBox rtxtNotes;
    }
}