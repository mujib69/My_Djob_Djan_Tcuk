namespace ISBS_New.AccountPayable
{
    partial class InquiryAccountsPayable
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnEmail = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.cmbStatusCode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.btnMNext = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.dgvInquiry = new System.Windows.Forms.DataGridView();
            this.btnCompleted = new System.Windows.Forms.Button();
            this.btnOnProgress = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dtToDate = new System.Windows.Forms.DateTimePicker();
            this.dtFromDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCriteria = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInquiry)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnEmail);
            this.groupBox1.Controls.Add(this.btnPreview);
            this.groupBox1.Controls.Add(this.cmbStatusCode);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Controls.Add(this.lblTotal);
            this.groupBox1.Controls.Add(this.cmbShow);
            this.groupBox1.Controls.Add(this.btnNew);
            this.groupBox1.Controls.Add(this.txtPage);
            this.groupBox1.Controls.Add(this.lblPage);
            this.groupBox1.Controls.Add(this.btnMNext);
            this.groupBox1.Controls.Add(this.btnNext);
            this.groupBox1.Controls.Add(this.btnMPrev);
            this.groupBox1.Controls.Add(this.btnPrev);
            this.groupBox1.Controls.Add(this.dgvInquiry);
            this.groupBox1.Controls.Add(this.btnCompleted);
            this.groupBox1.Controls.Add(this.btnOnProgress);
            this.groupBox1.Controls.Add(this.btnReset);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.dtToDate);
            this.groupBox1.Controls.Add(this.dtFromDate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbCriteria);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(807, 529);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnEmail
            // 
            this.btnEmail.Location = new System.Drawing.Point(95, 479);
            this.btnEmail.Name = "btnEmail";
            this.btnEmail.Size = new System.Drawing.Size(75, 23);
            this.btnEmail.TabIndex = 88;
            this.btnEmail.Text = "Send Email";
            this.btnEmail.UseVisualStyleBackColor = true;
            this.btnEmail.Click += new System.EventHandler(this.btnEmail_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(14, 479);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 87;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Visible = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // cmbStatusCode
            // 
            this.cmbStatusCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusCode.FormattingEnabled = true;
            this.cmbStatusCode.Location = new System.Drawing.Point(87, 72);
            this.cmbStatusCode.Name = "cmbStatusCode";
            this.cmbStatusCode.Size = new System.Drawing.Size(271, 21);
            this.cmbStatusCode.TabIndex = 86;
            this.cmbStatusCode.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 85;
            this.label5.Text = "Status Name";
            this.label5.Visible = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(718, 479);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 34;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(637, 479);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 33;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(638, 447);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 32;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // cmbShow
            // 
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(745, 444);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 31;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            this.cmbShow.TextChanged += new System.EventHandler(this.cmbShow_TextChanged);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(556, 479);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 23;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(76, 448);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 20;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(113, 452);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(15, 13);
            this.lblPage.TabIndex = 17;
            this.lblPage.Text = "/ ";
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(171, 447);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 23);
            this.btnMNext.TabIndex = 22;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(145, 447);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 23);
            this.btnNext.TabIndex = 21;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(14, 447);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(31, 23);
            this.btnMPrev.TabIndex = 18;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(50, 447);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(20, 23);
            this.btnPrev.TabIndex = 19;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // dgvInquiry
            // 
            this.dgvInquiry.AllowUserToAddRows = false;
            this.dgvInquiry.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInquiry.Location = new System.Drawing.Point(14, 135);
            this.dgvInquiry.Name = "dgvInquiry";
            this.dgvInquiry.Size = new System.Drawing.Size(777, 304);
            this.dgvInquiry.TabIndex = 12;
            this.dgvInquiry.DoubleClick += new System.EventHandler(this.dgvInquiry_DoubleClick);
            // 
            // btnCompleted
            // 
            this.btnCompleted.Location = new System.Drawing.Point(407, 106);
            this.btnCompleted.Name = "btnCompleted";
            this.btnCompleted.Size = new System.Drawing.Size(384, 23);
            this.btnCompleted.TabIndex = 11;
            this.btnCompleted.Text = "Completed";
            this.btnCompleted.UseVisualStyleBackColor = true;
            this.btnCompleted.Click += new System.EventHandler(this.btnCompleted_Click);
            // 
            // btnOnProgress
            // 
            this.btnOnProgress.Location = new System.Drawing.Point(14, 106);
            this.btnOnProgress.Name = "btnOnProgress";
            this.btnOnProgress.Size = new System.Drawing.Size(395, 23);
            this.btnOnProgress.TabIndex = 10;
            this.btnOnProgress.Text = "On Progress";
            this.btnOnProgress.UseVisualStyleBackColor = true;
            this.btnOnProgress.Click += new System.EventHandler(this.btnOnProgress_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(721, 42);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(721, 15);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dtToDate
            // 
            this.dtToDate.CustomFormat = "dd/MM/yyyy";
            this.dtToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtToDate.Location = new System.Drawing.Point(556, 45);
            this.dtToDate.Name = "dtToDate";
            this.dtToDate.Size = new System.Drawing.Size(159, 20);
            this.dtToDate.TabIndex = 7;
            // 
            // dtFromDate
            // 
            this.dtFromDate.CustomFormat = "dd/MM/yyyy";
            this.dtFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFromDate.Location = new System.Drawing.Point(556, 19);
            this.dtFromDate.Name = "dtFromDate";
            this.dtFromDate.Size = new System.Drawing.Size(159, 20);
            this.dtFromDate.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(473, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "To Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(473, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "From Date";
            // 
            // cmbCriteria
            // 
            this.cmbCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCriteria.FormattingEnabled = true;
            this.cmbCriteria.Location = new System.Drawing.Point(85, 45);
            this.cmbCriteria.Name = "cmbCriteria";
            this.cmbCriteria.Size = new System.Drawing.Size(273, 21);
            this.cmbCriteria.TabIndex = 3;
            this.cmbCriteria.SelectedIndexChanged += new System.EventHandler(this.cmbCriteria_SelectedIndexChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(85, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(273, 20);
            this.txtSearch.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Criteria";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search";
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 45000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // InquiryAccountsPayable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 614);
            this.Controls.Add(this.groupBox1);
            this.Name = "InquiryAccountsPayable";
            this.Text = "InquiryAccountsPayable";
            this.Load += new System.EventHandler(this.InquiryAccountsPayable_Load);
            this.Shown += new System.EventHandler(this.InquiryAccountsPayable_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InquiryAccountsPayable_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInquiry)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCriteria;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtFromDate;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DateTimePicker dtToDate;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnOnProgress;
        private System.Windows.Forms.Button btnCompleted;
        private System.Windows.Forms.DataGridView dgvInquiry;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.Button btnMNext;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.ComboBox cmbShow;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Timer timerRefresh;
        internal System.Windows.Forms.ComboBox cmbStatusCode;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnEmail;
        private System.Windows.Forms.Button btnPreview;
    }
}