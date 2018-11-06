namespace ISBS_New.Purchase.CanvasSheet
{
    partial class FormCanvasSheet2
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
            this.label4 = new System.Windows.Forms.Label();
            this.txtPrType = new System.Windows.Forms.TextBox();
            this.txtTransStatusDesc = new System.Windows.Forms.TextBox();
            this.txtTransStatusCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearchPR = new System.Windows.Forms.Button();
            this.txtPrNumber = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtCanvasDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCSNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.btnDeleteQuotation = new System.Windows.Forms.Button();
            this.btnAddQuotation = new System.Windows.Forms.Button();
            this.dgvPqDetails = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupPRH = new System.Windows.Forms.GroupBox();
            this.lblForm = new System.Windows.Forms.Label();
            this.grpCustomer.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPqDetails)).BeginInit();
            this.groupPRH.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.label4);
            this.grpCustomer.Controls.Add(this.txtPrType);
            this.grpCustomer.Controls.Add(this.txtTransStatusDesc);
            this.grpCustomer.Controls.Add(this.txtTransStatusCode);
            this.grpCustomer.Controls.Add(this.label1);
            this.grpCustomer.Controls.Add(this.btnSearchPR);
            this.grpCustomer.Controls.Add(this.txtPrNumber);
            this.grpCustomer.Controls.Add(this.label6);
            this.grpCustomer.Controls.Add(this.dtCanvasDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.txtCSNumber);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(1149, 110);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Header";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 150;
            this.label4.Text = "PR Type :";
            // 
            // txtPrType
            // 
            this.txtPrType.Enabled = false;
            this.txtPrType.Location = new System.Drawing.Point(125, 83);
            this.txtPrType.Name = "txtPrType";
            this.txtPrType.ReadOnly = true;
            this.txtPrType.Size = new System.Drawing.Size(200, 20);
            this.txtPrType.TabIndex = 149;
            // 
            // txtTransStatusDesc
            // 
            this.txtTransStatusDesc.Enabled = false;
            this.txtTransStatusDesc.Location = new System.Drawing.Point(485, 17);
            this.txtTransStatusDesc.Name = "txtTransStatusDesc";
            this.txtTransStatusDesc.ReadOnly = true;
            this.txtTransStatusDesc.Size = new System.Drawing.Size(239, 20);
            this.txtTransStatusDesc.TabIndex = 144;
            // 
            // txtTransStatusCode
            // 
            this.txtTransStatusCode.Enabled = false;
            this.txtTransStatusCode.Location = new System.Drawing.Point(602, 17);
            this.txtTransStatusCode.Name = "txtTransStatusCode";
            this.txtTransStatusCode.ReadOnly = true;
            this.txtTransStatusCode.Size = new System.Drawing.Size(58, 20);
            this.txtTransStatusCode.TabIndex = 143;
            this.txtTransStatusCode.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(417, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 142;
            this.label1.Text = "Status :";
            // 
            // btnSearchPR
            // 
            this.btnSearchPR.Location = new System.Drawing.Point(331, 59);
            this.btnSearchPR.Name = "btnSearchPR";
            this.btnSearchPR.Size = new System.Drawing.Size(32, 23);
            this.btnSearchPR.TabIndex = 141;
            this.btnSearchPR.Text = "...";
            this.btnSearchPR.UseVisualStyleBackColor = true;
            this.btnSearchPR.Click += new System.EventHandler(this.btnSearchPR_Click);
            // 
            // txtPrNumber
            // 
            this.txtPrNumber.Enabled = false;
            this.txtPrNumber.Location = new System.Drawing.Point(125, 61);
            this.txtPrNumber.Name = "txtPrNumber";
            this.txtPrNumber.ReadOnly = true;
            this.txtPrNumber.Size = new System.Drawing.Size(200, 20);
            this.txtPrNumber.TabIndex = 140;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 139;
            this.label6.Text = "PR Number :";
            // 
            // dtCanvasDate
            // 
            this.dtCanvasDate.CustomFormat = "dd-MM-yyyy";
            this.dtCanvasDate.Enabled = false;
            this.dtCanvasDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtCanvasDate.Location = new System.Drawing.Point(125, 17);
            this.dtCanvasDate.Name = "dtCanvasDate";
            this.dtCanvasDate.Size = new System.Drawing.Size(200, 20);
            this.dtCanvasDate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "Canvas Date :";
            // 
            // txtCSNumber
            // 
            this.txtCSNumber.Enabled = false;
            this.txtCSNumber.Location = new System.Drawing.Point(125, 39);
            this.txtCSNumber.Name = "txtCSNumber";
            this.txtCSNumber.ReadOnly = true;
            this.txtCSNumber.Size = new System.Drawing.Size(200, 20);
            this.txtCSNumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Canvas Number :";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(985, 636);
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
            this.btnEdit.Location = new System.Drawing.Point(814, 636);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(904, 636);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 143;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.btnDeleteQuotation);
            this.grpDetail.Controls.Add(this.btnAddQuotation);
            this.grpDetail.Controls.Add(this.dgvPqDetails);
            this.grpDetail.Controls.Add(this.label3);
            this.grpDetail.Location = new System.Drawing.Point(6, 122);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(1149, 508);
            this.grpDetail.TabIndex = 142;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            // 
            // btnDeleteQuotation
            // 
            this.btnDeleteQuotation.Location = new System.Drawing.Point(144, 19);
            this.btnDeleteQuotation.Name = "btnDeleteQuotation";
            this.btnDeleteQuotation.Size = new System.Drawing.Size(62, 23);
            this.btnDeleteQuotation.TabIndex = 150;
            this.btnDeleteQuotation.Text = "Delete";
            this.btnDeleteQuotation.UseVisualStyleBackColor = true;
            this.btnDeleteQuotation.Click += new System.EventHandler(this.btnDeleteQuotation_Click);
            // 
            // btnAddQuotation
            // 
            this.btnAddQuotation.Location = new System.Drawing.Point(76, 19);
            this.btnAddQuotation.Name = "btnAddQuotation";
            this.btnAddQuotation.Size = new System.Drawing.Size(62, 23);
            this.btnAddQuotation.TabIndex = 149;
            this.btnAddQuotation.Text = "Add";
            this.btnAddQuotation.UseVisualStyleBackColor = true;
            this.btnAddQuotation.Click += new System.EventHandler(this.btnAddQuotation_Click);
            // 
            // dgvPqDetails
            // 
            this.dgvPqDetails.AllowUserToAddRows = false;
            this.dgvPqDetails.AllowUserToDeleteRows = false;
            this.dgvPqDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPqDetails.Location = new System.Drawing.Point(7, 48);
            this.dgvPqDetails.Name = "dgvPqDetails";
            this.dgvPqDetails.ReadOnly = true;
            this.dgvPqDetails.Size = new System.Drawing.Size(1136, 454);
            this.dgvPqDetails.TabIndex = 11;
            this.dgvPqDetails.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvPqDetails_CellMouseDown);
            this.dgvPqDetails.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPqDetails_CellFormatting);
            this.dgvPqDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPqDetails_CellEndEdit);
            this.dgvPqDetails.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvPqDetails_CellPainting);
            this.dgvPqDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPqDetails_EditingControlShowing);
            this.dgvPqDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvPqDetails_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Quotation :";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1071, 636);
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
            this.groupPRH.Controls.Add(this.grpDetail);
            this.groupPRH.Controls.Add(this.btnExit);
            this.groupPRH.Controls.Add(this.btnEdit);
            this.groupPRH.Controls.Add(this.btnCancel);
            this.groupPRH.Controls.Add(this.btnSave);
            this.groupPRH.Location = new System.Drawing.Point(12, 27);
            this.groupPRH.Name = "groupPRH";
            this.groupPRH.Size = new System.Drawing.Size(1161, 674);
            this.groupPRH.TabIndex = 148;
            this.groupPRH.TabStop = false;
            // 
            // lblForm
            // 
            this.lblForm.AutoSize = true;
            this.lblForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForm.Location = new System.Drawing.Point(15, 14);
            this.lblForm.Name = "lblForm";
            this.lblForm.Size = new System.Drawing.Size(117, 13);
            this.lblForm.TabIndex = 132;
            this.lblForm.Text = "Form Canvas Sheet";
            // 
            // FormCanvasSheet2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(1181, 709);
            this.Controls.Add(this.lblForm);
            this.Controls.Add(this.groupPRH);
            this.Name = "FormCanvasSheet2";
            this.Resizable = false;
            this.Text = "Purchase Quotation Header";
            this.Load += new System.EventHandler(this.FormCanvasSheet2_Load);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            this.grpDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPqDetails)).EndInit();
            this.groupPRH.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.DateTimePicker dtCanvasDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtCSNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.DataGridView dgvPqDetails;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupPRH;
        private System.Windows.Forms.Label lblForm;
        private System.Windows.Forms.Button btnSearchPR;
        internal System.Windows.Forms.TextBox txtPrNumber;
        internal System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox txtTransStatusCode;
        private System.Windows.Forms.TextBox txtTransStatusDesc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDeleteQuotation;
        private System.Windows.Forms.Button btnAddQuotation;
        internal System.Windows.Forms.TextBox txtPrType;
        internal System.Windows.Forms.Label label4;

    }
}