namespace ISBS_New.Inventory.NotaAdjustment
{
    partial class HeaderNotaAdjust
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
            this.groupPLJH = new System.Windows.Forms.GroupBox();
            this.btnUnapprove = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.btnSearchW = new System.Windows.Forms.Button();
            this.txtInventSiteName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbActionCode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInventSiteID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
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
            this.dgvNADetails = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRevision = new System.Windows.Forms.Button();
            this.groupPLJH.SuspendLayout();
            this.grpCustomer.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNADetails)).BeginInit();
            this.SuspendLayout();
            // 
            // groupPLJH
            // 
            this.groupPLJH.Controls.Add(this.btnRevision);
            this.groupPLJH.Controls.Add(this.btnUnapprove);
            this.groupPLJH.Controls.Add(this.btnApprove);
            this.groupPLJH.Controls.Add(this.grpCustomer);
            this.groupPLJH.Controls.Add(this.btnCancel);
            this.groupPLJH.Controls.Add(this.btnEdit);
            this.groupPLJH.Controls.Add(this.btnSave);
            this.groupPLJH.Controls.Add(this.grpDetail);
            this.groupPLJH.Controls.Add(this.btnExit);
            this.groupPLJH.Location = new System.Drawing.Point(10, 56);
            this.groupPLJH.Name = "groupPLJH";
            this.groupPLJH.Size = new System.Drawing.Size(823, 393);
            this.groupPLJH.TabIndex = 153;
            this.groupPLJH.TabStop = false;
            // 
            // btnUnapprove
            // 
            this.btnUnapprove.Location = new System.Drawing.Point(87, 359);
            this.btnUnapprove.Name = "btnUnapprove";
            this.btnUnapprove.Size = new System.Drawing.Size(75, 23);
            this.btnUnapprove.TabIndex = 148;
            this.btnUnapprove.Text = "Unapprove";
            this.btnUnapprove.UseVisualStyleBackColor = true;
            this.btnUnapprove.Click += new System.EventHandler(this.btnUnapprove_Click);
            // 
            // btnApprove
            // 
            this.btnApprove.Location = new System.Drawing.Point(6, 359);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(75, 23);
            this.btnApprove.TabIndex = 147;
            this.btnApprove.Text = "Approve";
            this.btnApprove.UseVisualStyleBackColor = true;
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.btnSearchW);
            this.grpCustomer.Controls.Add(this.txtInventSiteName);
            this.grpCustomer.Controls.Add(this.label3);
            this.grpCustomer.Controls.Add(this.cmbActionCode);
            this.grpCustomer.Controls.Add(this.label2);
            this.grpCustomer.Controls.Add(this.txtInventSiteID);
            this.grpCustomer.Controls.Add(this.label1);
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
            // btnSearchW
            // 
            this.btnSearchW.Location = new System.Drawing.Point(177, 59);
            this.btnSearchW.Name = "btnSearchW";
            this.btnSearchW.Size = new System.Drawing.Size(34, 23);
            this.btnSearchW.TabIndex = 151;
            this.btnSearchW.Text = "...";
            this.btnSearchW.UseVisualStyleBackColor = true;
            this.btnSearchW.Click += new System.EventHandler(this.btnSearchW_Click);
            // 
            // txtInventSiteName
            // 
            this.txtInventSiteName.Enabled = false;
            this.txtInventSiteName.Location = new System.Drawing.Point(233, 61);
            this.txtInventSiteName.Name = "txtInventSiteName";
            this.txtInventSiteName.ReadOnly = true;
            this.txtInventSiteName.Size = new System.Drawing.Size(200, 20);
            this.txtInventSiteName.TabIndex = 127;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(217, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 126;
            this.label3.Text = "-";
            // 
            // cmbActionCode
            // 
            this.cmbActionCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActionCode.FormattingEnabled = true;
            this.cmbActionCode.Location = new System.Drawing.Point(139, 84);
            this.cmbActionCode.Name = "cmbActionCode";
            this.cmbActionCode.Size = new System.Drawing.Size(200, 21);
            this.cmbActionCode.TabIndex = 125;
            this.cmbActionCode.SelectedIndexChanged += new System.EventHandler(this.cmbActionCode_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 124;
            this.label2.Text = "Action Code";
            // 
            // txtInventSiteID
            // 
            this.txtInventSiteID.Enabled = false;
            this.txtInventSiteID.Location = new System.Drawing.Point(139, 61);
            this.txtInventSiteID.Name = "txtInventSiteID";
            this.txtInventSiteID.ReadOnly = true;
            this.txtInventSiteID.Size = new System.Drawing.Size(32, 20);
            this.txtInventSiteID.TabIndex = 123;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 122;
            this.label1.Text = "Warehouse";
            // 
            // dtNotaDate
            // 
            this.dtNotaDate.CustomFormat = "dd/MM/yyyy";
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
            this.label8.Size = new System.Drawing.Size(111, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "Nota Adjustment Date";
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
            this.label9.Location = new System.Drawing.Point(9, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(125, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Nota Adjustment Number";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(654, 359);
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
            this.btnEdit.Location = new System.Drawing.Point(483, 359);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(568, 359);
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
            this.grpDetail.Controls.Add(this.dgvNADetails);
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
            // dgvNADetails
            // 
            this.dgvNADetails.AllowUserToAddRows = false;
            this.dgvNADetails.AllowUserToDeleteRows = false;
            this.dgvNADetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNADetails.Location = new System.Drawing.Point(9, 44);
            this.dgvNADetails.Name = "dgvNADetails";
            this.dgvNADetails.Size = new System.Drawing.Size(794, 162);
            this.dgvNADetails.TabIndex = 11;
            this.dgvNADetails.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNADetails_CellValueChanged);
            this.dgvNADetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvNADetails_CellBeginEdit);
            this.dgvNADetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNADetails_CellEndEdit);
            this.dgvNADetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNADetails_CellClick);
            this.dgvNADetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvNADetails_EditingControlShowing);
            this.dgvNADetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvNADetails_KeyPress);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(740, 359);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRevision
            // 
            this.btnRevision.Location = new System.Drawing.Point(168, 359);
            this.btnRevision.Name = "btnRevision";
            this.btnRevision.Size = new System.Drawing.Size(75, 23);
            this.btnRevision.TabIndex = 149;
            this.btnRevision.Text = "Revision";
            this.btnRevision.UseVisualStyleBackColor = true;
            this.btnRevision.Click += new System.EventHandler(this.btnRevision_Click);
            // 
            // HeaderNotaAdjust
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 459);
            this.Controls.Add(this.groupPLJH);
            this.Name = "HeaderNotaAdjust";
            this.Text = "HeaderNotaAdjust";
            this.Load += new System.EventHandler(this.HeaderNotaAdjust_Load);
            this.groupPLJH.ResumeLayout(false);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNADetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupPLJH;
        internal System.Windows.Forms.GroupBox grpCustomer;
        private System.Windows.Forms.ComboBox cmbActionCode;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox txtInventSiteID;
        internal System.Windows.Forms.Label label1;
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
        internal System.Windows.Forms.DataGridView dgvNADetails;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.TextBox txtInventSiteName;
        internal System.Windows.Forms.Button btnSearchW;
        internal System.Windows.Forms.Button btnApprove;
        internal System.Windows.Forms.Button btnUnapprove;
        internal System.Windows.Forms.Button btnRevision;
    }
}