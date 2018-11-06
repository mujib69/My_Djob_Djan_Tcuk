namespace ISBS_New.Sales.MoUCustomer
{
    partial class HeaderMoUCustomer
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvMoUCustomerDetails = new System.Windows.Forms.DataGridView();
            this.dtValidTo = new System.Windows.Forms.DateTimePicker();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.dtMoUDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.groupPLJH = new System.Windows.Forms.GroupBox();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.btnLookUpBank = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtBankName = new System.Windows.Forms.TextBox();
            this.txtBankID = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.dtJatuhTempo = new System.Windows.Forms.DateTimePicker();
            this.textCustGroupID = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtLC = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.textCustGroupName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textLimitCredit = new System.Windows.Forms.TextBox();
            this.txtLCType = new System.Windows.Forms.TextBox();
            this.btnLookUpCustGroup = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLCNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbBankGuarantee = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMoUNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoUCustomerDetails)).BeginInit();
            this.groupPLJH.SuspendLayout();
            this.grpCustomer.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(476, 435);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.btnNew);
            this.grpDetail.Controls.Add(this.btnDelete);
            this.grpDetail.Controls.Add(this.dgvMoUCustomerDetails);
            this.grpDetail.Location = new System.Drawing.Point(6, 212);
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
            // dgvMoUCustomerDetails
            // 
            this.dgvMoUCustomerDetails.AllowUserToAddRows = false;
            this.dgvMoUCustomerDetails.AllowUserToDeleteRows = false;
            this.dgvMoUCustomerDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMoUCustomerDetails.Location = new System.Drawing.Point(9, 44);
            this.dgvMoUCustomerDetails.Name = "dgvMoUCustomerDetails";
            this.dgvMoUCustomerDetails.ReadOnly = true;
            this.dgvMoUCustomerDetails.Size = new System.Drawing.Size(794, 162);
            this.dgvMoUCustomerDetails.TabIndex = 11;
            this.dgvMoUCustomerDetails.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMoUCustomerDetails_CellMouseDown);
            this.dgvMoUCustomerDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvMoUCustomerDetails_CellFormatting);
            this.dgvMoUCustomerDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvMoUCustomerDetails_EditingControlShowing);
            this.dgvMoUCustomerDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvMoUCustomerDetails_KeyPress);
            // 
            // dtValidTo
            // 
            this.dtValidTo.CustomFormat = "dd/MM/yyyy";
            this.dtValidTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtValidTo.Location = new System.Drawing.Point(125, 64);
            this.dtValidTo.Name = "dtValidTo";
            this.dtValidTo.Size = new System.Drawing.Size(200, 20);
            this.dtValidTo.TabIndex = 120;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(647, 435);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 144;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 119;
            this.label3.Text = "Valid To ";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(561, 435);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 143;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtMoUDate
            // 
            this.dtMoUDate.CustomFormat = "dd/MM/yyyy";
            this.dtMoUDate.Enabled = false;
            this.dtMoUDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMoUDate.Location = new System.Drawing.Point(125, 13);
            this.dtMoUDate.Name = "dtMoUDate";
            this.dtMoUDate.Size = new System.Drawing.Size(200, 20);
            this.dtMoUDate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "MoU Date ";
            // 
            // groupPLJH
            // 
            this.groupPLJH.Controls.Add(this.grpCustomer);
            this.groupPLJH.Controls.Add(this.btnCancel);
            this.groupPLJH.Controls.Add(this.btnEdit);
            this.groupPLJH.Controls.Add(this.btnSave);
            this.groupPLJH.Controls.Add(this.grpDetail);
            this.groupPLJH.Controls.Add(this.btnExit);
            this.groupPLJH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupPLJH.Location = new System.Drawing.Point(20, 60);
            this.groupPLJH.Name = "groupPLJH";
            this.groupPLJH.Size = new System.Drawing.Size(816, 479);
            this.groupPLJH.TabIndex = 152;
            this.groupPLJH.TabStop = false;
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.btnLookUpBank);
            this.grpCustomer.Controls.Add(this.label12);
            this.grpCustomer.Controls.Add(this.txtBankName);
            this.grpCustomer.Controls.Add(this.txtBankID);
            this.grpCustomer.Controls.Add(this.label13);
            this.grpCustomer.Controls.Add(this.dtJatuhTempo);
            this.grpCustomer.Controls.Add(this.textCustGroupID);
            this.grpCustomer.Controls.Add(this.label11);
            this.grpCustomer.Controls.Add(this.label1);
            this.grpCustomer.Controls.Add(this.dtLC);
            this.grpCustomer.Controls.Add(this.label2);
            this.grpCustomer.Controls.Add(this.textCustGroupName);
            this.grpCustomer.Controls.Add(this.label10);
            this.grpCustomer.Controls.Add(this.textLimitCredit);
            this.grpCustomer.Controls.Add(this.txtLCType);
            this.grpCustomer.Controls.Add(this.btnLookUpCustGroup);
            this.grpCustomer.Controls.Add(this.label7);
            this.grpCustomer.Controls.Add(this.label4);
            this.grpCustomer.Controls.Add(this.txtLCNo);
            this.grpCustomer.Controls.Add(this.label6);
            this.grpCustomer.Controls.Add(this.cmbBankGuarantee);
            this.grpCustomer.Controls.Add(this.label5);
            this.grpCustomer.Controls.Add(this.dtValidTo);
            this.grpCustomer.Controls.Add(this.label3);
            this.grpCustomer.Controls.Add(this.dtMoUDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.txtMoUNumber);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(809, 198);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Header";
            // 
            // btnLookUpBank
            // 
            this.btnLookUpBank.Location = new System.Drawing.Point(727, 138);
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
            this.label12.Location = new System.Drawing.Point(411, 168);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 13);
            this.label12.TabIndex = 141;
            this.label12.Text = "Bank Name";
            // 
            // txtBankName
            // 
            this.txtBankName.Enabled = false;
            this.txtBankName.Location = new System.Drawing.Point(526, 164);
            this.txtBankName.Name = "txtBankName";
            this.txtBankName.ReadOnly = true;
            this.txtBankName.Size = new System.Drawing.Size(200, 20);
            this.txtBankName.TabIndex = 140;
            // 
            // txtBankID
            // 
            this.txtBankID.Enabled = false;
            this.txtBankID.Location = new System.Drawing.Point(526, 139);
            this.txtBankID.Name = "txtBankID";
            this.txtBankID.ReadOnly = true;
            this.txtBankID.Size = new System.Drawing.Size(200, 20);
            this.txtBankID.TabIndex = 139;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(411, 143);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 138;
            this.label13.Text = "Bank ID";
            // 
            // dtJatuhTempo
            // 
            this.dtJatuhTempo.CustomFormat = "dd/MM/yyyy";
            this.dtJatuhTempo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtJatuhTempo.Location = new System.Drawing.Point(526, 114);
            this.dtJatuhTempo.Name = "dtJatuhTempo";
            this.dtJatuhTempo.Size = new System.Drawing.Size(200, 20);
            this.dtJatuhTempo.TabIndex = 137;
            // 
            // textCustGroupID
            // 
            this.textCustGroupID.Enabled = false;
            this.textCustGroupID.Location = new System.Drawing.Point(125, 89);
            this.textCustGroupID.Name = "textCustGroupID";
            this.textCustGroupID.ReadOnly = true;
            this.textCustGroupID.Size = new System.Drawing.Size(200, 20);
            this.textCustGroupID.TabIndex = 122;
            this.textCustGroupID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textCustGroupID_MouseDown);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(411, 118);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 13);
            this.label11.TabIndex = 136;
            this.label11.Text = "Jatuh Tempo";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 121;
            this.label1.Text = "Cust ID ";
            // 
            // dtLC
            // 
            this.dtLC.CustomFormat = "dd/MM/yyyy";
            this.dtLC.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtLC.Location = new System.Drawing.Point(526, 89);
            this.dtLC.Name = "dtLC";
            this.dtLC.Size = new System.Drawing.Size(200, 20);
            this.dtLC.TabIndex = 135;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 123;
            this.label2.Text = "Limit Credit";
            // 
            // textCustGroupName
            // 
            this.textCustGroupName.Enabled = false;
            this.textCustGroupName.Location = new System.Drawing.Point(125, 114);
            this.textCustGroupName.Name = "textCustGroupName";
            this.textCustGroupName.ReadOnly = true;
            this.textCustGroupName.Size = new System.Drawing.Size(200, 20);
            this.textCustGroupName.TabIndex = 124;
           // this.textCustGroupName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textCustGroupName_MouseDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(411, 93);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 134;
            this.label10.Text = "LC Date";
            // 
            // textLimitCredit
            // 
            this.textLimitCredit.Location = new System.Drawing.Point(125, 139);
            this.textLimitCredit.Name = "textLimitCredit";
            this.textLimitCredit.Size = new System.Drawing.Size(200, 20);
            this.textLimitCredit.TabIndex = 125;
            this.textLimitCredit.Text = "0.0000";
            this.textLimitCredit.Leave += new System.EventHandler(this.textLimitCredit_Leave);
            this.textLimitCredit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textLimitCredit_KeyPress);
            // 
            // txtLCType
            // 
            this.txtLCType.Location = new System.Drawing.Point(526, 64);
            this.txtLCType.Name = "txtLCType";
            this.txtLCType.Size = new System.Drawing.Size(200, 20);
            this.txtLCType.TabIndex = 133;
            // 
            // btnLookUpCustGroup
            // 
            this.btnLookUpCustGroup.Location = new System.Drawing.Point(328, 88);
            this.btnLookUpCustGroup.Name = "btnLookUpCustGroup";
            this.btnLookUpCustGroup.Size = new System.Drawing.Size(32, 23);
            this.btnLookUpCustGroup.TabIndex = 126;
            this.btnLookUpCustGroup.Text = "...";
            this.btnLookUpCustGroup.UseVisualStyleBackColor = true;
            this.btnLookUpCustGroup.Click += new System.EventHandler(this.btnLookUpCustGroup_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(411, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 132;
            this.label7.Text = "LC Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 127;
            this.label4.Text = "Cust Name";
            // 
            // txtLCNo
            // 
            this.txtLCNo.Location = new System.Drawing.Point(526, 39);
            this.txtLCNo.Name = "txtLCNo";
            this.txtLCNo.Size = new System.Drawing.Size(200, 20);
            this.txtLCNo.TabIndex = 131;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(411, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 130;
            this.label6.Text = "LC No";
            // 
            // cmbBankGuarantee
            // 
            this.cmbBankGuarantee.FormattingEnabled = true;
            this.cmbBankGuarantee.Location = new System.Drawing.Point(526, 13);
            this.cmbBankGuarantee.Name = "cmbBankGuarantee";
            this.cmbBankGuarantee.Size = new System.Drawing.Size(200, 21);
            this.cmbBankGuarantee.TabIndex = 129;
            this.cmbBankGuarantee.SelectedIndexChanged += new System.EventHandler(this.BankGuarantee_SelectedIndexChange);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(411, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 128;
            this.label5.Text = "Bank Guarantee";
            // 
            // txtMoUNumber
            // 
            this.txtMoUNumber.Enabled = false;
            this.txtMoUNumber.Location = new System.Drawing.Point(125, 39);
            this.txtMoUNumber.Name = "txtMoUNumber";
            this.txtMoUNumber.ReadOnly = true;
            this.txtMoUNumber.Size = new System.Drawing.Size(200, 20);
            this.txtMoUNumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "MoU Number ";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(733, 434);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // HeaderMoUCustomer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(856, 559);
            this.Controls.Add(this.groupPLJH);
            this.Name = "HeaderMoUCustomer";
            this.Resizable = false;
            this.Text = "MoU Customer";
            this.Load += new System.EventHandler(this.HeaderMoUCustomer2_Load);
            this.Shown += new System.EventHandler(this.HeaderMoUCustomer2_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HeaderMoUCustomer2_FormClosed);
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoUCustomerDetails)).EndInit();
            this.groupPLJH.ResumeLayout(false);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.DataGridView dgvMoUCustomerDetails;
        internal System.Windows.Forms.DateTimePicker dtValidTo;
        internal System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.DateTimePicker dtMoUDate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupPLJH;
        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.TextBox txtMoUNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox textCustGroupID;
        internal System.Windows.Forms.TextBox textCustGroupName;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox textLimitCredit;
        private System.Windows.Forms.Button btnLookUpCustGroup;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.DateTimePicker dtLC;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtLCType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLCNo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbBankGuarantee;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label12;
        internal System.Windows.Forms.TextBox txtBankName;
        internal System.Windows.Forms.TextBox txtBankID;
        private System.Windows.Forms.Label label13;
        internal System.Windows.Forms.DateTimePicker dtJatuhTempo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnLookUpBank;

    }
}
