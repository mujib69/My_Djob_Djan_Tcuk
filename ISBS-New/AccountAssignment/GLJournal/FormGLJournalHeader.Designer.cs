namespace ISBS_New.AccountAssignment.GLJournal
{
    partial class FormGLJournalHeader
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
            this.btnNew = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dtRefDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.dtJournalDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearchReference = new System.Windows.Forms.Button();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSearchJournalType = new System.Windows.Forms.Button();
            this.txtJournalType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkpost = new System.Windows.Forms.CheckBox();
            this.txtTotalKredit = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTotalDebet = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dgvJournalHeader = new System.Windows.Forms.DataGridView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtGLJournalCode = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnPosting = new System.Windows.Forms.Button();
            this.btnInActive = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJournalHeader)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(28, 89);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "GL Journal Code :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.dtRefDate);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.dtJournalDate);
            this.groupBox1.Controls.Add(this.btnSearchReference);
            this.groupBox1.Controls.Add(this.txtReference);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnSearchJournalType);
            this.groupBox1.Controls.Add(this.txtJournalType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtNotes);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkpost);
            this.groupBox1.Controls.Add(this.txtTotalKredit);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtTotalDebet);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dgvJournalHeader);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.txtGLJournalCode);
            this.groupBox1.Controls.Add(this.btnNew);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(23, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(743, 371);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(386, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Ref Date :";
            this.label8.Visible = false;
            // 
            // dtRefDate
            // 
            this.dtRefDate.CustomFormat = "dd/MM/yyyy";
            this.dtRefDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtRefDate.Location = new System.Drawing.Point(462, 83);
            this.dtRefDate.Name = "dtRefDate";
            this.dtRefDate.Size = new System.Drawing.Size(187, 20);
            this.dtRefDate.TabIndex = 26;
            this.dtRefDate.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(386, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Journal Date :";
            // 
            // dtJournalDate
            // 
            this.dtJournalDate.CustomFormat = "dd/MM/yyyy";
            this.dtJournalDate.Enabled = false;
            this.dtJournalDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtJournalDate.Location = new System.Drawing.Point(462, 54);
            this.dtJournalDate.Name = "dtJournalDate";
            this.dtJournalDate.Size = new System.Drawing.Size(187, 20);
            this.dtJournalDate.TabIndex = 24;
            // 
            // btnSearchReference
            // 
            this.btnSearchReference.AutoSize = true;
            this.btnSearchReference.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchReference.Location = new System.Drawing.Point(623, 22);
            this.btnSearchReference.Name = "btnSearchReference";
            this.btnSearchReference.Size = new System.Drawing.Size(26, 23);
            this.btnSearchReference.TabIndex = 23;
            this.btnSearchReference.Text = "...";
            this.btnSearchReference.UseVisualStyleBackColor = true;
            this.btnSearchReference.Click += new System.EventHandler(this.btnSearchReference_Click);
            // 
            // txtReference
            // 
            this.txtReference.Location = new System.Drawing.Point(462, 24);
            this.txtReference.Name = "txtReference";
            this.txtReference.ReadOnly = true;
            this.txtReference.Size = new System.Drawing.Size(155, 20);
            this.txtReference.TabIndex = 22;
            this.txtReference.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtReference_MouseDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(386, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Reference :";
            // 
            // btnSearchJournalType
            // 
            this.btnSearchJournalType.AutoSize = true;
            this.btnSearchJournalType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchJournalType.Location = new System.Drawing.Point(285, 52);
            this.btnSearchJournalType.Name = "btnSearchJournalType";
            this.btnSearchJournalType.Size = new System.Drawing.Size(26, 23);
            this.btnSearchJournalType.TabIndex = 20;
            this.btnSearchJournalType.Text = "...";
            this.btnSearchJournalType.UseVisualStyleBackColor = true;
            this.btnSearchJournalType.Click += new System.EventHandler(this.btnSearchJournalType_Click);
            // 
            // txtJournalType
            // 
            this.txtJournalType.Location = new System.Drawing.Point(123, 54);
            this.txtJournalType.Name = "txtJournalType";
            this.txtJournalType.ReadOnly = true;
            this.txtJournalType.Size = new System.Drawing.Size(156, 20);
            this.txtJournalType.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Journal Type :";
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(123, 307);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(188, 58);
            this.txtNotes.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 310);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Notes";
            // 
            // chkpost
            // 
            this.chkpost.AutoSize = true;
            this.chkpost.Enabled = false;
            this.chkpost.Location = new System.Drawing.Point(675, 26);
            this.chkpost.Name = "chkpost";
            this.chkpost.Size = new System.Drawing.Size(47, 17);
            this.chkpost.TabIndex = 17;
            this.chkpost.Text = "Post";
            this.chkpost.UseVisualStyleBackColor = true;
            this.chkpost.CheckedChanged += new System.EventHandler(this.chkpost_CheckedChanged);
            // 
            // txtTotalKredit
            // 
            this.txtTotalKredit.Location = new System.Drawing.Point(537, 333);
            this.txtTotalKredit.Name = "txtTotalKredit";
            this.txtTotalKredit.ReadOnly = true;
            this.txtTotalKredit.Size = new System.Drawing.Size(188, 20);
            this.txtTotalKredit.TabIndex = 16;
            this.txtTotalKredit.Text = "0.0000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(438, 336);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Total Kredit :";
            // 
            // txtTotalDebet
            // 
            this.txtTotalDebet.Location = new System.Drawing.Point(537, 307);
            this.txtTotalDebet.Name = "txtTotalDebet";
            this.txtTotalDebet.ReadOnly = true;
            this.txtTotalDebet.Size = new System.Drawing.Size(188, 20);
            this.txtTotalDebet.TabIndex = 14;
            this.txtTotalDebet.Text = "0.0000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(438, 310);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Total Debet :";
            // 
            // dgvJournalHeader
            // 
            this.dgvJournalHeader.AllowUserToAddRows = false;
            this.dgvJournalHeader.AllowUserToDeleteRows = false;
            this.dgvJournalHeader.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvJournalHeader.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvJournalHeader.Location = new System.Drawing.Point(19, 118);
            this.dgvJournalHeader.Name = "dgvJournalHeader";
            this.dgvJournalHeader.ReadOnly = true;
            this.dgvJournalHeader.Size = new System.Drawing.Size(706, 183);
            this.dgvJournalHeader.TabIndex = 10;
            this.dgvJournalHeader.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvJournalHeader_RowsAdded);
            this.dgvJournalHeader.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvJournalHeader_CellEndEdit);
            this.dgvJournalHeader.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvJournalHeader_EditingControlShowing);
            this.dgvJournalHeader.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvJournalHeader_RowsRemoved);
            this.dgvJournalHeader.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvJournalHeader_KeyPress);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(109, 89);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtGLJournalCode
            // 
            this.txtGLJournalCode.Location = new System.Drawing.Point(123, 24);
            this.txtGLJournalCode.Name = "txtGLJournalCode";
            this.txtGLJournalCode.ReadOnly = true;
            this.txtGLJournalCode.Size = new System.Drawing.Size(188, 20);
            this.txtGLJournalCode.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnPosting);
            this.groupBox2.Controls.Add(this.btnInActive);
            this.groupBox2.Controls.Add(this.btnEdit);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnExit);
            this.groupBox2.Location = new System.Drawing.Point(23, 439);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(743, 44);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // btnPosting
            // 
            this.btnPosting.Location = new System.Drawing.Point(100, 13);
            this.btnPosting.Name = "btnPosting";
            this.btnPosting.Size = new System.Drawing.Size(75, 23);
            this.btnPosting.TabIndex = 22;
            this.btnPosting.Text = "Posting";
            this.btnPosting.UseVisualStyleBackColor = true;
            this.btnPosting.Click += new System.EventHandler(this.btnPosting_Click);
            // 
            // btnInActive
            // 
            this.btnInActive.Location = new System.Drawing.Point(19, 13);
            this.btnInActive.Name = "btnInActive";
            this.btnInActive.Size = new System.Drawing.Size(75, 23);
            this.btnInActive.TabIndex = 21;
            this.btnInActive.Text = "InActive";
            this.btnInActive.UseVisualStyleBackColor = true;
            this.btnInActive.Click += new System.EventHandler(this.btnInActive_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(408, 13);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 20;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(488, 13);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(569, 13);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(650, 13);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 17;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // FormGLJournalHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 504);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormGLJournalHeader";
            this.Text = "GL Journal Header";
            this.Load += new System.EventHandler(this.FormGLJournalHeader_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvJournalHeader)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtGLJournalCode;
        private System.Windows.Forms.DataGridView dgvJournalHeader;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtTotalKredit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTotalDebet;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnPosting;
        private System.Windows.Forms.Button btnInActive;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.CheckBox chkpost;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtJournalType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearchJournalType;
        private System.Windows.Forms.Button btnSearchReference;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtJournalDate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtRefDate;
    }
}