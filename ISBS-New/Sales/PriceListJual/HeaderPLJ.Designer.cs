namespace ISBS_New.Sales.PriceListJual
{
    partial class HeaderPLJ
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
            this.dtPljDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPLJNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dgvPLJDetails = new System.Windows.Forms.DataGridView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupPLJH = new System.Windows.Forms.GroupBox();
            this.txtNotes = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvCustomer = new System.Windows.Forms.DataGridView();
            this.btnDeleteCustomer = new System.Windows.Forms.Button();
            this.btnAddCustomer = new System.Windows.Forms.Button();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.lblForm = new System.Windows.Forms.Label();
            this.grpCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPLJDetails)).BeginInit();
            this.groupPLJH.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).BeginInit();
            this.grpDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.dtFrom);
            this.grpCustomer.Controls.Add(this.dtTo);
            this.grpCustomer.Controls.Add(this.label3);
            this.grpCustomer.Controls.Add(this.label4);
            this.grpCustomer.Controls.Add(this.dtPljDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.txtPLJNumber);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(809, 75);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Header";
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
            // dtPljDate
            // 
            this.dtPljDate.CustomFormat = "dd-MM-yyyy";
            this.dtPljDate.Enabled = false;
            this.dtPljDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPljDate.Location = new System.Drawing.Point(125, 17);
            this.dtPljDate.Name = "dtPljDate";
            this.dtPljDate.Size = new System.Drawing.Size(200, 20);
            this.dtPljDate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "PLJ Date";
            // 
            // txtPLJNumber
            // 
            this.txtPLJNumber.Enabled = false;
            this.txtPLJNumber.Location = new System.Drawing.Point(125, 39);
            this.txtPLJNumber.Name = "txtPLJNumber";
            this.txtPLJNumber.ReadOnly = true;
            this.txtPLJNumber.Size = new System.Drawing.Size(200, 20);
            this.txtPLJNumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "PLJ Number";
            // 
            // dgvPLJDetails
            // 
            this.dgvPLJDetails.AllowUserToAddRows = false;
            this.dgvPLJDetails.AllowUserToDeleteRows = false;
            this.dgvPLJDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPLJDetails.Location = new System.Drawing.Point(9, 44);
            this.dgvPLJDetails.Name = "dgvPLJDetails";
            this.dgvPLJDetails.ReadOnly = true;
            this.dgvPLJDetails.Size = new System.Drawing.Size(794, 162);
            this.dgvPLJDetails.TabIndex = 11;
            this.dgvPLJDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvPLJDetails_CellBeginEdit);
            this.dgvPLJDetails.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvPLJDetails_CellMouseDown);
            this.dgvPLJDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPLJDetails_CellFormatting);
            this.dgvPLJDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPLJDetails_CellEndEdit);
            this.dgvPLJDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPLJDetails_EditingControlShowing);
            this.dgvPLJDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvPLJDetails_KeyPress);
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
            this.btnExit.Location = new System.Drawing.Point(733, 528);
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
            this.groupPLJH.Controls.Add(this.dgvCustomer);
            this.groupPLJH.Controls.Add(this.btnDeleteCustomer);
            this.groupPLJH.Controls.Add(this.btnAddCustomer);
            this.groupPLJH.Controls.Add(this.cmbCustomer);
            this.groupPLJH.Controls.Add(this.label1);
            this.groupPLJH.Controls.Add(this.grpCustomer);
            this.groupPLJH.Controls.Add(this.btnCancel);
            this.groupPLJH.Controls.Add(this.btnEdit);
            this.groupPLJH.Controls.Add(this.btnSave);
            this.groupPLJH.Controls.Add(this.grpDetail);
            this.groupPLJH.Controls.Add(this.btnExit);
            this.groupPLJH.Location = new System.Drawing.Point(12, 28);
            this.groupPLJH.Name = "groupPLJH";
            this.groupPLJH.Size = new System.Drawing.Size(823, 560);
            this.groupPLJH.TabIndex = 150;
            this.groupPLJH.TabStop = false;
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(141, 459);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(255, 62);
            this.txtNotes.TabIndex = 159;
            this.txtNotes.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 480);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 158;
            this.label2.Text = "Notes";
            // 
            // dgvCustomer
            // 
            this.dgvCustomer.AllowUserToAddRows = false;
            this.dgvCustomer.AllowUserToDeleteRows = false;
            this.dgvCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomer.Location = new System.Drawing.Point(17, 338);
            this.dgvCustomer.Name = "dgvCustomer";
            this.dgvCustomer.ReadOnly = true;
            this.dgvCustomer.Size = new System.Drawing.Size(791, 110);
            this.dgvCustomer.TabIndex = 157;
            // 
            // btnDeleteCustomer
            // 
            this.btnDeleteCustomer.Location = new System.Drawing.Point(483, 309);
            this.btnDeleteCustomer.Name = "btnDeleteCustomer";
            this.btnDeleteCustomer.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteCustomer.TabIndex = 156;
            this.btnDeleteCustomer.Text = "Delete";
            this.btnDeleteCustomer.UseVisualStyleBackColor = true;
            this.btnDeleteCustomer.Click += new System.EventHandler(this.btnDeleteCustomer_Click);
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.Location = new System.Drawing.Point(402, 309);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(75, 23);
            this.btnAddCustomer.TabIndex = 155;
            this.btnAddCustomer.Text = "Add";
            this.btnAddCustomer.UseVisualStyleBackColor = true;
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            // 
            // cmbCustomer
            // 
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(141, 311);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(255, 21);
            this.cmbCustomer.TabIndex = 154;
            this.cmbCustomer.SelectedIndexChanged += new System.EventHandler(this.Customer_SelectedIndexChange);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 313);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 153;
            this.label1.Text = "Customer List";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(647, 528);
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
            this.btnEdit.Location = new System.Drawing.Point(476, 528);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(561, 528);
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
            this.grpDetail.Controls.Add(this.dgvPLJDetails);
            this.grpDetail.Location = new System.Drawing.Point(6, 89);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(810, 216);
            this.grpDetail.TabIndex = 142;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            this.grpDetail.Enter += new System.EventHandler(this.grpDetail_Enter);
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
            // lblForm
            // 
            this.lblForm.AutoSize = true;
            this.lblForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForm.Location = new System.Drawing.Point(13, 13);
            this.lblForm.Name = "lblForm";
            this.lblForm.Size = new System.Drawing.Size(132, 13);
            this.lblForm.TabIndex = 149;
            this.lblForm.Text = "Price List Jual Header";
            // 
            // HeaderPLJ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(850, 598);
            this.Controls.Add(this.lblForm);
            this.Controls.Add(this.groupPLJH);
            this.Name = "HeaderPLJ";
            this.Resizable = false;
            this.Text = "Price List Jual Header";
            this.Load += new System.EventHandler(this.HeaderPLJ2_Load);
            this.Shown += new System.EventHandler(this.HeaderPLJ2_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HeaderPLJ2_FormClosed);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPLJDetails)).EndInit();
            this.groupPLJH.ResumeLayout(false);
            this.groupPLJH.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).EndInit();
            this.grpDetail.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.DateTimePicker dtPljDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtPLJNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.DataGridView dgvPLJDetails;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupPLJH;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label lblForm;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox txtNotes;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvCustomer;
        internal System.Windows.Forms.Button btnDeleteCustomer;
        internal System.Windows.Forms.Button btnAddCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        internal System.Windows.Forms.Label label1;

    }
}
