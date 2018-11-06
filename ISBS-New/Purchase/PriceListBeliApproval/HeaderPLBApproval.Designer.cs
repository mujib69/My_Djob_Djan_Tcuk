namespace ISBS_New.Purchase.PriceListBeliApproval
{
    partial class HeaderPLBApproval
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
            this.groupPLJH = new System.Windows.Forms.GroupBox();
            this.btnCancelapproved = new System.Windows.Forms.Button();
            this.btnRequestApprovedByManagement = new System.Windows.Forms.Button();
            this.btnApproved = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.txtNotes = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvVendorReference = new System.Windows.Forms.DataGridView();
            this.cmbVendorReference = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpDetail = new System.Windows.Forms.GroupBox();
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
            this.grpCustomer.Text = "Header";
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtFrom.Enabled = false;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(525, 15);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(200, 20);
            this.dtFrom.TabIndex = 121;
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dtTo.Enabled = false;
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
            this.dgvPLBDetails.Location = new System.Drawing.Point(9, 19);
            this.dgvPLBDetails.Name = "dgvPLBDetails";
            this.dgvPLBDetails.ReadOnly = true;
            this.dgvPLBDetails.Size = new System.Drawing.Size(794, 162);
            this.dgvPLBDetails.TabIndex = 11;
            // 
            // groupPLJH
            // 
            this.groupPLJH.Controls.Add(this.btnCancelapproved);
            this.groupPLJH.Controls.Add(this.btnRequestApprovedByManagement);
            this.groupPLJH.Controls.Add(this.btnApproved);
            this.groupPLJH.Controls.Add(this.btnExit);
            this.groupPLJH.Controls.Add(this.txtNotes);
            this.groupPLJH.Controls.Add(this.label2);
            this.groupPLJH.Controls.Add(this.dgvVendorReference);
            this.groupPLJH.Controls.Add(this.cmbVendorReference);
            this.groupPLJH.Controls.Add(this.label1);
            this.groupPLJH.Controls.Add(this.grpCustomer);
            this.groupPLJH.Controls.Add(this.grpDetail);
            this.groupPLJH.Location = new System.Drawing.Point(12, 58);
            this.groupPLJH.Name = "groupPLJH";
            this.groupPLJH.Size = new System.Drawing.Size(823, 531);
            this.groupPLJH.TabIndex = 150;
            this.groupPLJH.TabStop = false;
            // 
            // btnCancelapproved
            // 
            this.btnCancelapproved.Location = new System.Drawing.Point(623, 501);
            this.btnCancelapproved.Name = "btnCancelapproved";
            this.btnCancelapproved.Size = new System.Drawing.Size(111, 23);
            this.btnCancelapproved.TabIndex = 156;
            this.btnCancelapproved.Text = "Cancel Approve";
            this.btnCancelapproved.UseVisualStyleBackColor = true;
            this.btnCancelapproved.Click += new System.EventHandler(this.btnCancelapproved_Click);
            // 
            // btnRequestApprovedByManagement
            // 
            this.btnRequestApprovedByManagement.Location = new System.Drawing.Point(420, 501);
            this.btnRequestApprovedByManagement.Name = "btnRequestApprovedByManagement";
            this.btnRequestApprovedByManagement.Size = new System.Drawing.Size(197, 23);
            this.btnRequestApprovedByManagement.TabIndex = 154;
            this.btnRequestApprovedByManagement.Text = "Request Approve By Management";
            this.btnRequestApprovedByManagement.UseVisualStyleBackColor = true;
            this.btnRequestApprovedByManagement.Click += new System.EventHandler(this.btnRequestApprovedByManagement_Click);
            // 
            // btnApproved
            // 
            this.btnApproved.Location = new System.Drawing.Point(339, 502);
            this.btnApproved.Name = "btnApproved";
            this.btnApproved.Size = new System.Drawing.Size(75, 23);
            this.btnApproved.TabIndex = 153;
            this.btnApproved.Text = "Approve";
            this.btnApproved.UseVisualStyleBackColor = true;
            this.btnApproved.Click += new System.EventHandler(this.btnApproved_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(741, 501);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 155;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(142, 430);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(255, 62);
            this.txtNotes.TabIndex = 152;
            this.txtNotes.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 451);
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
            this.dgvVendorReference.Location = new System.Drawing.Point(18, 309);
            this.dgvVendorReference.Name = "dgvVendorReference";
            this.dgvVendorReference.ReadOnly = true;
            this.dgvVendorReference.Size = new System.Drawing.Size(791, 110);
            this.dgvVendorReference.TabIndex = 150;
            // 
            // cmbVendorReference
            // 
            this.cmbVendorReference.FormattingEnabled = true;
            this.cmbVendorReference.Location = new System.Drawing.Point(142, 282);
            this.cmbVendorReference.Name = "cmbVendorReference";
            this.cmbVendorReference.Size = new System.Drawing.Size(255, 21);
            this.cmbVendorReference.TabIndex = 147;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 284);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 122;
            this.label1.Text = "Vendor Reference Type";
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.dgvPLBDetails);
            this.grpDetail.Location = new System.Drawing.Point(6, 89);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(810, 190);
            this.grpDetail.TabIndex = 142;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            // 
            // HeaderPLBApproval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(854, 603);
            this.Controls.Add(this.groupPLJH);
            this.Name = "HeaderPLBApproval";
            this.Resizable = false;
            this.Text = "Price List Jual Header";
            this.Load += new System.EventHandler(this.HeaderPLBApproval_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HeaderPLBApproval_FormClosed);
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
        private System.Windows.Forms.GroupBox groupPLJH;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbVendorReference;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtNotes;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvVendorReference;
        private System.Windows.Forms.Button btnCancelapproved;
        internal System.Windows.Forms.Button btnRequestApprovedByManagement;
        internal System.Windows.Forms.Button btnApproved;
        internal System.Windows.Forms.Button btnExit;

    }
}
