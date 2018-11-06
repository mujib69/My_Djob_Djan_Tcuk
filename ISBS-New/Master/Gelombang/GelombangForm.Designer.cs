namespace ISBS_New.Master.Gelombang
{
    partial class GelombangForm
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
            this.grpUser = new System.Windows.Forms.GroupBox();
            this.rtxtDesc = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBracketDesc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddVendor = new System.Windows.Forms.Button();
            this.lBoxVendor = new System.Windows.Forms.ListBox();
            this.txtBracketId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtGelombangId = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnMNext = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvGelombangD = new System.Windows.Forms.DataGridView();
            this.grpUser.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGelombangD)).BeginInit();
            this.SuspendLayout();
            // 
            // grpUser
            // 
            this.grpUser.Controls.Add(this.rtxtDesc);
            this.grpUser.Controls.Add(this.label5);
            this.grpUser.Controls.Add(this.txtBracketDesc);
            this.grpUser.Controls.Add(this.label3);
            this.grpUser.Controls.Add(this.btnRemove);
            this.grpUser.Controls.Add(this.btnAddVendor);
            this.grpUser.Controls.Add(this.lBoxVendor);
            this.grpUser.Controls.Add(this.txtBracketId);
            this.grpUser.Controls.Add(this.label4);
            this.grpUser.Controls.Add(this.dtDate);
            this.grpUser.Controls.Add(this.Label2);
            this.grpUser.Controls.Add(this.txtGelombangId);
            this.grpUser.Controls.Add(this.Label1);
            this.grpUser.Location = new System.Drawing.Point(5, 58);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(833, 112);
            this.grpUser.TabIndex = 75;
            this.grpUser.TabStop = false;
            // 
            // rtxtDesc
            // 
            this.rtxtDesc.Enabled = false;
            this.rtxtDesc.Location = new System.Drawing.Point(390, 51);
            this.rtxtDesc.MaxLength = 150;
            this.rtxtDesc.Name = "rtxtDesc";
            this.rtxtDesc.Size = new System.Drawing.Size(165, 47);
            this.rtxtDesc.TabIndex = 31;
            this.rtxtDesc.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(271, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Description";
            // 
            // txtBracketDesc
            // 
            this.txtBracketDesc.Enabled = false;
            this.txtBracketDesc.Location = new System.Drawing.Point(390, 22);
            this.txtBracketDesc.MaxLength = 150;
            this.txtBracketDesc.Name = "txtBracketDesc";
            this.txtBracketDesc.Size = new System.Drawing.Size(165, 20);
            this.txtBracketDesc.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(271, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Bracket Description";
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(726, 41);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 27;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAddVendor
            // 
            this.btnAddVendor.Enabled = false;
            this.btnAddVendor.Location = new System.Drawing.Point(726, 12);
            this.btnAddVendor.Name = "btnAddVendor";
            this.btnAddVendor.Size = new System.Drawing.Size(75, 23);
            this.btnAddVendor.TabIndex = 26;
            this.btnAddVendor.Text = "Add";
            this.btnAddVendor.UseVisualStyleBackColor = true;
            this.btnAddVendor.Click += new System.EventHandler(this.btnAddVendor_Click);
            // 
            // lBoxVendor
            // 
            this.lBoxVendor.FormattingEnabled = true;
            this.lBoxVendor.Location = new System.Drawing.Point(600, 12);
            this.lBoxVendor.Name = "lBoxVendor";
            this.lBoxVendor.Size = new System.Drawing.Size(120, 95);
            this.lBoxVendor.TabIndex = 25;
            this.lBoxVendor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lBoxVendor_MouseDown);
            // 
            // txtBracketId
            // 
            this.txtBracketId.Enabled = false;
            this.txtBracketId.Location = new System.Drawing.Point(130, 51);
            this.txtBracketId.MaxLength = 50;
            this.txtBracketId.Name = "txtBracketId";
            this.txtBracketId.Size = new System.Drawing.Size(110, 20);
            this.txtBracketId.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Bracket Id";
            // 
            // dtDate
            // 
            this.dtDate.CustomFormat = "dd/MM/yyyy";
            this.dtDate.Enabled = false;
            this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDate.Location = new System.Drawing.Point(130, 81);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(110, 20);
            this.dtDate.TabIndex = 14;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(18, 85);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(30, 13);
            this.Label2.TabIndex = 9;
            this.Label2.Text = "Date";
            // 
            // txtGelombangId
            // 
            this.txtGelombangId.Enabled = false;
            this.txtGelombangId.Location = new System.Drawing.Point(130, 22);
            this.txtGelombangId.MaxLength = 50;
            this.txtGelombangId.Name = "txtGelombangId";
            this.txtGelombangId.Size = new System.Drawing.Size(110, 20);
            this.txtGelombangId.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(18, 25);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(48, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Wave Id";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(659, 439);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 77;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(740, 439);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 78;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(497, 439);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 76;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(578, 439);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 79;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.lblTotal);
            this.grpDetail.Controls.Add(this.txtPage);
            this.grpDetail.Controls.Add(this.lblPage);
            this.grpDetail.Controls.Add(this.cmbShow);
            this.grpDetail.Controls.Add(this.btnMNext);
            this.grpDetail.Controls.Add(this.btnNext);
            this.grpDetail.Controls.Add(this.btnMPrev);
            this.grpDetail.Controls.Add(this.btnPrev);
            this.grpDetail.Controls.Add(this.btnAdd);
            this.grpDetail.Controls.Add(this.btnDelete);
            this.grpDetail.Controls.Add(this.dgvGelombangD);
            this.grpDetail.Location = new System.Drawing.Point(5, 176);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(810, 257);
            this.grpDetail.TabIndex = 143;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(641, 225);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 157;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(69, 218);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 153;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(104, 221);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(15, 13);
            this.lblPage.TabIndex = 150;
            this.lblPage.Text = "/ ";
            // 
            // cmbShow
            // 
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(761, 221);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 156;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            this.cmbShow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbShow_KeyPress);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(162, 216);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 23);
            this.btnMNext.TabIndex = 155;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(136, 216);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 23);
            this.btnNext.TabIndex = 154;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(6, 216);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(31, 23);
            this.btnMPrev.TabIndex = 151;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(43, 216);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(20, 23);
            this.btnPrev.TabIndex = 152;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(9, 19);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(59, 23);
            this.btnAdd.TabIndex = 149;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(75, 19);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(59, 23);
            this.btnDelete.TabIndex = 148;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvGelombangD
            // 
            this.dgvGelombangD.AllowUserToAddRows = false;
            this.dgvGelombangD.AllowUserToDeleteRows = false;
            this.dgvGelombangD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGelombangD.Location = new System.Drawing.Point(9, 50);
            this.dgvGelombangD.Name = "dgvGelombangD";
            this.dgvGelombangD.ReadOnly = true;
            this.dgvGelombangD.Size = new System.Drawing.Size(794, 162);
            this.dgvGelombangD.TabIndex = 11;
            this.dgvGelombangD.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGelombangD_CellValueChanged);
            this.dgvGelombangD.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvGelombangD_CellMouseDown);
            this.dgvGelombangD.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            // 
            // GelombangForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 479);
            this.Controls.Add(this.grpDetail);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.grpUser);
            this.Name = "GelombangForm";
            this.Text = "Gelombang Form";
            this.Load += new System.EventHandler(this.GelombangForm_Load);
            this.grpUser.ResumeLayout(false);
            this.grpUser.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            this.grpDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGelombangD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpUser;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox txtGelombangId;
        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.DateTimePicker dtDate;
        internal System.Windows.Forms.TextBox txtBracketId;
        internal System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.Button btnAdd;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.DataGridView dgvGelombangD;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.Button btnMNext;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.ListBox lBoxVendor;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAddVendor;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox txtBracketDesc;
        internal System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rtxtDesc;
    }
}