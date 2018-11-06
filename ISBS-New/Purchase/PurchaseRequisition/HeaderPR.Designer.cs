namespace ISBS_New.Purchase.PurchaseRequisition
{
    partial class HeaderPR
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
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.txtStatusName = new System.Windows.Forms.TextBox();
            this.txtPrStatus = new System.Windows.Forms.TextBox();
            this.cmbPrType = new System.Windows.Forms.ComboBox();
            this.txtPrApproved = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dtPrDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPrNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvPrDetails = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupPRH = new System.Windows.Forms.GroupBox();
            this.grpCustomer.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrDetails)).BeginInit();
            this.groupPRH.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.txtStatusName);
            this.grpCustomer.Controls.Add(this.txtPrStatus);
            this.grpCustomer.Controls.Add(this.cmbPrType);
            this.grpCustomer.Controls.Add(this.txtPrApproved);
            this.grpCustomer.Controls.Add(this.label2);
            this.grpCustomer.Controls.Add(this.label5);
            this.grpCustomer.Controls.Add(this.label7);
            this.grpCustomer.Controls.Add(this.dtPrDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.txtPrNumber);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(809, 94);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Header";
            // 
            // txtStatusName
            // 
            this.txtStatusName.Location = new System.Drawing.Point(536, 17);
            this.txtStatusName.Name = "txtStatusName";
            this.txtStatusName.ReadOnly = true;
            this.txtStatusName.Size = new System.Drawing.Size(267, 20);
            this.txtStatusName.TabIndex = 132;
            // 
            // txtPrStatus
            // 
            this.txtPrStatus.Enabled = false;
            this.txtPrStatus.Location = new System.Drawing.Point(536, 17);
            this.txtPrStatus.Name = "txtPrStatus";
            this.txtPrStatus.ReadOnly = true;
            this.txtPrStatus.Size = new System.Drawing.Size(43, 20);
            this.txtPrStatus.TabIndex = 131;
            this.txtPrStatus.Visible = false;
            // 
            // cmbPrType
            // 
            this.cmbPrType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrType.Enabled = false;
            this.cmbPrType.FormattingEnabled = true;
            this.cmbPrType.Items.AddRange(new object[] {
            "FIX",
            "QTY",
            "AMOUNT"});
            this.cmbPrType.Location = new System.Drawing.Point(125, 61);
            this.cmbPrType.Name = "cmbPrType";
            this.cmbPrType.Size = new System.Drawing.Size(200, 21);
            this.cmbPrType.TabIndex = 130;
            this.cmbPrType.SelectedIndexChanged += new System.EventHandler(this.cmbPrType_SelectedIndexChanged);
            this.cmbPrType.Click += new System.EventHandler(this.cmbPrType_Click);
            // 
            // txtPrApproved
            // 
            this.txtPrApproved.Location = new System.Drawing.Point(536, 39);
            this.txtPrApproved.Name = "txtPrApproved";
            this.txtPrApproved.ReadOnly = true;
            this.txtPrApproved.Size = new System.Drawing.Size(267, 20);
            this.txtPrApproved.TabIndex = 129;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(421, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 128;
            this.label2.Text = "PR Approved :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(421, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 127;
            this.label5.Text = "PR Status :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 118;
            this.label7.Text = "PR Type :";
            // 
            // dtPrDate
            // 
            this.dtPrDate.CustomFormat = "dd/MM/yyyy";
            this.dtPrDate.Enabled = false;
            this.dtPrDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPrDate.Location = new System.Drawing.Point(125, 17);
            this.dtPrDate.Name = "dtPrDate";
            this.dtPrDate.Size = new System.Drawing.Size(200, 20);
            this.dtPrDate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "PR Date :";
            // 
            // txtPrNumber
            // 
            this.txtPrNumber.Location = new System.Drawing.Point(125, 39);
            this.txtPrNumber.Name = "txtPrNumber";
            this.txtPrNumber.ReadOnly = true;
            this.txtPrNumber.Size = new System.Drawing.Size(200, 20);
            this.txtPrNumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "PR No :";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(647, 329);
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
            this.btnEdit.Location = new System.Drawing.Point(476, 329);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEditH_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(561, 329);
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
            this.grpDetail.Controls.Add(this.dgvPrDetails);
            this.grpDetail.Location = new System.Drawing.Point(6, 107);
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
            // dgvPrDetails
            // 
            this.dgvPrDetails.AllowUserToAddRows = false;
            this.dgvPrDetails.AllowUserToDeleteRows = false;
            this.dgvPrDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrDetails.Location = new System.Drawing.Point(9, 44);
            this.dgvPrDetails.Name = "dgvPrDetails";
            this.dgvPrDetails.ReadOnly = true;
            this.dgvPrDetails.Size = new System.Drawing.Size(794, 162);
            this.dgvPrDetails.TabIndex = 11;
            this.dgvPrDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvPrDetails_CellBeginEdit);
            this.dgvPrDetails.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvPrDetails_CellMouseDown);
            this.dgvPrDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPrDetails_CellFormatting);
            this.dgvPrDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPrDetails_CellEndEdit);
            this.dgvPrDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPrDetails_CellClick);
            this.dgvPrDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPrDetails_EditingControlShowing);
            this.dgvPrDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvPrDetails_KeyPress);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(733, 329);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupPRH
            // 
            this.groupPRH.Controls.Add(this.grpCustomer);
            this.groupPRH.Controls.Add(this.btnCancel);
            this.groupPRH.Controls.Add(this.btnEdit);
            this.groupPRH.Controls.Add(this.btnSave);
            this.groupPRH.Controls.Add(this.grpDetail);
            this.groupPRH.Controls.Add(this.btnExit);
            this.groupPRH.Location = new System.Drawing.Point(12, 55);
            this.groupPRH.Name = "groupPRH";
            this.groupPRH.Size = new System.Drawing.Size(823, 371);
            this.groupPRH.TabIndex = 148;
            this.groupPRH.TabStop = false;
            // 
            // HeaderPR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(845, 434);
            this.Controls.Add(this.groupPRH);
            this.Name = "HeaderPR";
            this.Resizable = false;
            this.Text = "Purchase Requisition Header";
            this.Load += new System.EventHandler(this.HeaderPR2_Load);
            this.Shown += new System.EventHandler(this.HeaderPR2_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HeaderPR2_FormClosed);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrDetails)).EndInit();
            this.groupPRH.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.ComboBox cmbPrType;
        internal System.Windows.Forms.TextBox txtPrApproved;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.DateTimePicker dtPrDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtPrNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.DataGridView dgvPrDetails;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.TextBox txtPrStatus;
        private System.Windows.Forms.GroupBox groupPRH;
        internal System.Windows.Forms.TextBox txtStatusName;

    }
}