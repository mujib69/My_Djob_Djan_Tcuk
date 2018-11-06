namespace ISBS_New.Sales.PriceListJualApproval
{
    partial class HeaderPLJApproval
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
            this.groupPLJH = new System.Windows.Forms.GroupBox();
            this.btnCancelapproved = new System.Windows.Forms.Button();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtPljDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPLJNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnRequestApprovedByManagement = new System.Windows.Forms.Button();
            this.btnApproved = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.dgvPLJDetails = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblForm = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvCustomer = new System.Windows.Forms.DataGridView();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupPLJH.SuspendLayout();
            this.grpCustomer.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPLJDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).BeginInit();
            this.SuspendLayout();
            // 
            // groupPLJH
            // 
            this.groupPLJH.Controls.Add(this.txtNotes);
            this.groupPLJH.Controls.Add(this.label2);
            this.groupPLJH.Controls.Add(this.dgvCustomer);
            this.groupPLJH.Controls.Add(this.cmbCustomer);
            this.groupPLJH.Controls.Add(this.label1);
            this.groupPLJH.Controls.Add(this.btnCancelapproved);
            this.groupPLJH.Controls.Add(this.grpCustomer);
            this.groupPLJH.Controls.Add(this.btnRequestApprovedByManagement);
            this.groupPLJH.Controls.Add(this.btnApproved);
            this.groupPLJH.Controls.Add(this.grpDetail);
            this.groupPLJH.Controls.Add(this.btnExit);
            this.groupPLJH.Location = new System.Drawing.Point(11, 24);
            this.groupPLJH.Name = "groupPLJH";
            this.groupPLJH.Size = new System.Drawing.Size(823, 580);
            this.groupPLJH.TabIndex = 152;
            this.groupPLJH.TabStop = false;
            // 
            // btnCancelapproved
            // 
            this.btnCancelapproved.Location = new System.Drawing.Point(615, 548);
            this.btnCancelapproved.Name = "btnCancelapproved";
            this.btnCancelapproved.Size = new System.Drawing.Size(111, 23);
            this.btnCancelapproved.TabIndex = 147;
            this.btnCancelapproved.Text = "Cancel Approve";
            this.btnCancelapproved.UseVisualStyleBackColor = true;
            this.btnCancelapproved.Click += new System.EventHandler(this.btnCancelapproved_Click);
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
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "PLJ Date :";
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
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "PLJ Number :";
            // 
            // btnRequestApprovedByManagement
            // 
            this.btnRequestApprovedByManagement.Location = new System.Drawing.Point(412, 548);
            this.btnRequestApprovedByManagement.Name = "btnRequestApprovedByManagement";
            this.btnRequestApprovedByManagement.Size = new System.Drawing.Size(197, 23);
            this.btnRequestApprovedByManagement.TabIndex = 144;
            this.btnRequestApprovedByManagement.Text = "Request Approve By Management";
            this.btnRequestApprovedByManagement.UseVisualStyleBackColor = true;
            this.btnRequestApprovedByManagement.Click += new System.EventHandler(this.btnApprovedByManagement_Click);
            // 
            // btnApproved
            // 
            this.btnApproved.Location = new System.Drawing.Point(331, 549);
            this.btnApproved.Name = "btnApproved";
            this.btnApproved.Size = new System.Drawing.Size(75, 23);
            this.btnApproved.TabIndex = 143;
            this.btnApproved.Text = "Approve";
            this.btnApproved.UseVisualStyleBackColor = true;
            this.btnApproved.Click += new System.EventHandler(this.btnApproved_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.dgvPLJDetails);
            this.grpDetail.Location = new System.Drawing.Point(6, 89);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(810, 216);
            this.grpDetail.TabIndex = 142;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            // 
            // dgvPLJDetails
            // 
            this.dgvPLJDetails.AllowUserToAddRows = false;
            this.dgvPLJDetails.AllowUserToDeleteRows = false;
            this.dgvPLJDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPLJDetails.Location = new System.Drawing.Point(9, 19);
            this.dgvPLJDetails.Name = "dgvPLJDetails";
            this.dgvPLJDetails.ReadOnly = true;
            this.dgvPLJDetails.Size = new System.Drawing.Size(794, 187);
            this.dgvPLJDetails.TabIndex = 11;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(733, 548);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblForm
            // 
            this.lblForm.AutoSize = true;
            this.lblForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForm.Location = new System.Drawing.Point(12, 9);
            this.lblForm.Name = "lblForm";
            this.lblForm.Size = new System.Drawing.Size(186, 13);
            this.lblForm.TabIndex = 151;
            this.lblForm.Text = "Price List Jual Approval Header";
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(137, 462);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(255, 62);
            this.txtNotes.TabIndex = 166;
            this.txtNotes.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 483);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 165;
            this.label2.Text = "Notes";
            // 
            // dgvCustomer
            // 
            this.dgvCustomer.AllowUserToAddRows = false;
            this.dgvCustomer.AllowUserToDeleteRows = false;
            this.dgvCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomer.Location = new System.Drawing.Point(13, 341);
            this.dgvCustomer.Name = "dgvCustomer";
            this.dgvCustomer.ReadOnly = true;
            this.dgvCustomer.Size = new System.Drawing.Size(791, 110);
            this.dgvCustomer.TabIndex = 164;
            // 
            // cmbCustomer
            // 
            this.cmbCustomer.FormattingEnabled = true;
            this.cmbCustomer.Location = new System.Drawing.Point(137, 314);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(255, 21);
            this.cmbCustomer.TabIndex = 161;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 316);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 160;
            this.label1.Text = "Customer List";
            // 
            // HeaderPLJApproval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 616);
            this.Controls.Add(this.lblForm);
            this.Controls.Add(this.groupPLJH);
            this.Name = "HeaderPLJApproval";
            this.Text = "Price List Jual Approval Header";
            this.Load += new System.EventHandler(this.HeaderPLJApproval_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.HeaderPLJApproval_FormClosed);
            this.groupPLJH.ResumeLayout(false);
            this.groupPLJH.PerformLayout();
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPLJDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupPLJH;
        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.DateTimePicker dtPljDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtPLJNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Button btnRequestApprovedByManagement;
        internal System.Windows.Forms.Button btnApproved;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.DataGridView dgvPLJDetails;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblForm;
        private System.Windows.Forms.Button btnCancelapproved;
        private System.Windows.Forms.RichTextBox txtNotes;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvCustomer;
        private System.Windows.Forms.ComboBox cmbCustomer;
        internal System.Windows.Forms.Label label1;

    }
}
