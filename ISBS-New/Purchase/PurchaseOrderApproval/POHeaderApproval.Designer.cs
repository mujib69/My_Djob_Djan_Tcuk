namespace ISBS_New.Purchase.PurchaseOrderApproval
{
    partial class POHeaderApproval
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
            this.grpPR = new System.Windows.Forms.GroupBox();
            this.txtPOStatus = new System.Windows.Forms.TextBox();
            this.cmbPOType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dtPODate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPONumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.dgvPODetails1 = new System.Windows.Forms.DataGridView();
            this.grpPR.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPODetails1)).BeginInit();
            this.SuspendLayout();
            // 
            // grpPR
            // 
            this.grpPR.Controls.Add(this.txtPOStatus);
            this.grpPR.Controls.Add(this.cmbPOType);
            this.grpPR.Controls.Add(this.label5);
            this.grpPR.Controls.Add(this.label7);
            this.grpPR.Controls.Add(this.dtPODate);
            this.grpPR.Controls.Add(this.label8);
            this.grpPR.Controls.Add(this.txtPONumber);
            this.grpPR.Controls.Add(this.label9);
            this.grpPR.Location = new System.Drawing.Point(7, 57);
            this.grpPR.Name = "grpPR";
            this.grpPR.Size = new System.Drawing.Size(865, 121);
            this.grpPR.TabIndex = 154;
            this.grpPR.TabStop = false;
            this.grpPR.Text = "Header";
            // 
            // txtPOStatus
            // 
            this.txtPOStatus.Enabled = false;
            this.txtPOStatus.Location = new System.Drawing.Point(626, 21);
            this.txtPOStatus.Name = "txtPOStatus";
            this.txtPOStatus.ReadOnly = true;
            this.txtPOStatus.Size = new System.Drawing.Size(219, 20);
            this.txtPOStatus.TabIndex = 131;
            // 
            // cmbPOType
            // 
            this.cmbPOType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPOType.Enabled = false;
            this.cmbPOType.FormattingEnabled = true;
            this.cmbPOType.Items.AddRange(new object[] {
            "QTY",
            "AMOUNT"});
            this.cmbPOType.Location = new System.Drawing.Point(132, 85);
            this.cmbPOType.Name = "cmbPOType";
            this.cmbPOType.Size = new System.Drawing.Size(200, 21);
            this.cmbPOType.TabIndex = 130;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(511, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 127;
            this.label5.Text = "PO Status :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 118;
            this.label7.Text = "PO Type :";
            // 
            // dtPODate
            // 
            this.dtPODate.CustomFormat = "dd/MM/yyyy";
            this.dtPODate.Enabled = false;
            this.dtPODate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPODate.Location = new System.Drawing.Point(132, 18);
            this.dtPODate.Name = "dtPODate";
            this.dtPODate.Size = new System.Drawing.Size(200, 20);
            this.dtPODate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "PO Date :";
            // 
            // txtPONumber
            // 
            this.txtPONumber.Enabled = false;
            this.txtPONumber.Location = new System.Drawing.Point(132, 51);
            this.txtPONumber.Name = "txtPONumber";
            this.txtPONumber.ReadOnly = true;
            this.txtPONumber.Size = new System.Drawing.Size(200, 20);
            this.txtPONumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "PO Number :";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(803, 509);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 157;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnApprove
            // 
            this.btnApprove.Location = new System.Drawing.Point(722, 509);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(75, 23);
            this.btnApprove.TabIndex = 156;
            this.btnApprove.Text = "Approve";
            this.btnApprove.UseVisualStyleBackColor = true;
            this.btnApprove.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.dgvPODetails1);
            this.grpDetail.Location = new System.Drawing.Point(7, 184);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(871, 319);
            this.grpDetail.TabIndex = 155;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            // 
            // dgvPODetails1
            // 
            this.dgvPODetails1.AllowUserToAddRows = false;
            this.dgvPODetails1.AllowUserToDeleteRows = false;
            this.dgvPODetails1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPODetails1.Location = new System.Drawing.Point(10, 19);
            this.dgvPODetails1.Name = "dgvPODetails1";
            this.dgvPODetails1.Size = new System.Drawing.Size(855, 294);
            this.dgvPODetails1.TabIndex = 11;
            this.dgvPODetails1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvPODetails1_CellMouseDown);
            this.dgvPODetails1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPODetails1_CellFormatting);
            // 
            // POHeaderApproval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 546);
            this.Controls.Add(this.grpPR);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnApprove);
            this.Controls.Add(this.grpDetail);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(901, 546);
            this.Name = "POHeaderApproval";
            this.Text = "PO Approval";
            this.Load += new System.EventHandler(this.POHeaderApproval_Load);
            this.grpPR.ResumeLayout(false);
            this.grpPR.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPODetails1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpPR;
        internal System.Windows.Forms.TextBox txtPOStatus;
        internal System.Windows.Forms.ComboBox cmbPOType;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.DateTimePicker dtPODate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtPONumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnApprove;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.DataGridView dgvPODetails1;
    }
}