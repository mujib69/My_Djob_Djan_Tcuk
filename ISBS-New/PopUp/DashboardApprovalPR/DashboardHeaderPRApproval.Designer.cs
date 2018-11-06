namespace ISBS_New.PopUp.DashboardApprovalPR
{
    partial class DashboardHeaderPRApproval
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
            this.txtTransStatus = new System.Windows.Forms.TextBox();
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
            this.dgvPrDetails = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.grpGelombang = new System.Windows.Forms.GroupBox();
            this.dgvGelombang = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.tabDgvControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.grpPR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrDetails)).BeginInit();
            this.grpGelombang.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGelombang)).BeginInit();
            this.grpDetail.SuspendLayout();
            this.tabDgvControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPR
            // 
            this.grpPR.Controls.Add(this.txtTransStatus);
            this.grpPR.Controls.Add(this.txtPrStatus);
            this.grpPR.Controls.Add(this.cmbPrType);
            this.grpPR.Controls.Add(this.txtPrApproved);
            this.grpPR.Controls.Add(this.label2);
            this.grpPR.Controls.Add(this.label5);
            this.grpPR.Controls.Add(this.label7);
            this.grpPR.Controls.Add(this.dtPrDate);
            this.grpPR.Controls.Add(this.label8);
            this.grpPR.Controls.Add(this.txtPrNumber);
            this.grpPR.Controls.Add(this.label9);
            this.grpPR.Location = new System.Drawing.Point(12, 12);
            this.grpPR.Name = "grpPR";
            this.grpPR.Size = new System.Drawing.Size(809, 121);
            this.grpPR.TabIndex = 148;
            this.grpPR.TabStop = false;
            this.grpPR.Text = "Header";
            // 
            // txtTransStatus
            // 
            this.txtTransStatus.Enabled = false;
            this.txtTransStatus.Location = new System.Drawing.Point(397, 17);
            this.txtTransStatus.Name = "txtTransStatus";
            this.txtTransStatus.Size = new System.Drawing.Size(42, 20);
            this.txtTransStatus.TabIndex = 132;
            this.txtTransStatus.Visible = false;
            // 
            // txtPrStatus
            // 
            this.txtPrStatus.Enabled = false;
            this.txtPrStatus.Location = new System.Drawing.Point(560, 18);
            this.txtPrStatus.Name = "txtPrStatus";
            this.txtPrStatus.ReadOnly = true;
            this.txtPrStatus.Size = new System.Drawing.Size(219, 20);
            this.txtPrStatus.TabIndex = 131;
            // 
            // cmbPrType
            // 
            this.cmbPrType.Enabled = false;
            this.cmbPrType.FormattingEnabled = true;
            this.cmbPrType.Location = new System.Drawing.Point(132, 85);
            this.cmbPrType.Name = "cmbPrType";
            this.cmbPrType.Size = new System.Drawing.Size(200, 21);
            this.cmbPrType.TabIndex = 130;
            // 
            // txtPrApproved
            // 
            this.txtPrApproved.Enabled = false;
            this.txtPrApproved.Location = new System.Drawing.Point(560, 51);
            this.txtPrApproved.Name = "txtPrApproved";
            this.txtPrApproved.ReadOnly = true;
            this.txtPrApproved.Size = new System.Drawing.Size(219, 20);
            this.txtPrApproved.TabIndex = 129;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(445, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 128;
            this.label2.Text = "PR Approved :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(445, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 127;
            this.label5.Text = "PR Status :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 118;
            this.label7.Text = "PR Type :";
            // 
            // dtPrDate
            // 
            this.dtPrDate.CustomFormat = "dd-MM-yyyy";
            this.dtPrDate.Enabled = false;
            this.dtPrDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPrDate.Location = new System.Drawing.Point(132, 18);
            this.dtPrDate.Name = "dtPrDate";
            this.dtPrDate.Size = new System.Drawing.Size(200, 20);
            this.dtPrDate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "PR Date :";
            // 
            // txtPrNumber
            // 
            this.txtPrNumber.Enabled = false;
            this.txtPrNumber.Location = new System.Drawing.Point(132, 51);
            this.txtPrNumber.Name = "txtPrNumber";
            this.txtPrNumber.ReadOnly = true;
            this.txtPrNumber.Size = new System.Drawing.Size(200, 20);
            this.txtPrNumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "PR Number :";
            // 
            // dgvPrDetails
            // 
            this.dgvPrDetails.AllowUserToAddRows = false;
            this.dgvPrDetails.AllowUserToDeleteRows = false;
            this.dgvPrDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrDetails.Location = new System.Drawing.Point(8, 51);
            this.dgvPrDetails.Name = "dgvPrDetails";
            this.dgvPrDetails.Size = new System.Drawing.Size(794, 168);
            this.dgvPrDetails.TabIndex = 11;
            this.dgvPrDetails.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvPrDetails_CellMouseDown);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(744, 513);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 153;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // grpGelombang
            // 
            this.grpGelombang.Controls.Add(this.dgvGelombang);
            this.grpGelombang.Location = new System.Drawing.Point(12, 371);
            this.grpGelombang.Name = "grpGelombang";
            this.grpGelombang.Size = new System.Drawing.Size(809, 127);
            this.grpGelombang.TabIndex = 154;
            this.grpGelombang.TabStop = false;
            this.grpGelombang.Text = "Gelombang";
            this.grpGelombang.Visible = false;
            // 
            // dgvGelombang
            // 
            this.dgvGelombang.AllowUserToAddRows = false;
            this.dgvGelombang.AllowUserToDeleteRows = false;
            this.dgvGelombang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGelombang.Location = new System.Drawing.Point(13, 19);
            this.dgvGelombang.Name = "dgvGelombang";
            this.dgvGelombang.ReadOnly = true;
            this.dgvGelombang.Size = new System.Drawing.Size(794, 93);
            this.dgvGelombang.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(663, 513);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 150;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.dgvPrDetails);
            this.grpDetail.Controls.Add(this.tabDgvControl);
            this.grpDetail.Location = new System.Drawing.Point(12, 139);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(810, 226);
            this.grpDetail.TabIndex = 149;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            // 
            // tabDgvControl
            // 
            this.tabDgvControl.Controls.Add(this.tabPage1);
            this.tabDgvControl.Controls.Add(this.tabPage2);
            this.tabDgvControl.Location = new System.Drawing.Point(8, 29);
            this.tabDgvControl.Name = "tabDgvControl";
            this.tabDgvControl.SelectedIndex = 0;
            this.tabDgvControl.Size = new System.Drawing.Size(795, 30);
            this.tabDgvControl.TabIndex = 151;
            this.tabDgvControl.SelectedIndexChanged += new System.EventHandler(this.tabDgvControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(787, 4);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Approval";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(787, 4);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Detail";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // HeaderPRApproval
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 548);
            this.Controls.Add(this.grpPR);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.grpGelombang);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpDetail);
            this.Name = "HeaderPRApproval";
            this.Text = "HeaderPRApproval";
            this.Load += new System.EventHandler(this.DashboardHeaderPRApproval_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DashboardHeaderPRApproval_FormClosed);
            this.grpPR.ResumeLayout(false);
            this.grpPR.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrDetails)).EndInit();
            this.grpGelombang.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGelombang)).EndInit();
            this.grpDetail.ResumeLayout(false);
            this.tabDgvControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpPR;
        internal System.Windows.Forms.TextBox txtPrStatus;
        internal System.Windows.Forms.ComboBox cmbPrType;
        internal System.Windows.Forms.TextBox txtPrApproved;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.DateTimePicker dtPrDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtPrNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.DataGridView dgvPrDetails;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox grpGelombang;
        private System.Windows.Forms.DataGridView dgvGelombang;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpDetail;
        private System.Windows.Forms.TextBox txtTransStatus;
        private System.Windows.Forms.TabControl tabDgvControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}