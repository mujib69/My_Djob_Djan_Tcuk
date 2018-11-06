namespace ISBS_New
{
    partial class InquiryV2
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
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabDgv = new System.Windows.Forms.TabControl();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnMNext = new System.Windows.Forms.Button();
            this.lblPage = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.dgvInquiry = new System.Windows.Forms.DataGridView();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbFilter = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grpFooter = new System.Windows.Forms.GroupBox();
            this.btnApproval = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.grpHeader = new System.Windows.Forms.GroupBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.ToolTipNotes = new System.Windows.Forms.ToolTip(this.components);
            this.btnPreview = new System.Windows.Forms.Button();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInquiry)).BeginInit();
            this.grpFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 45000;
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy";
            this.dtFrom.Enabled = false;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(492, 16);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(200, 20);
            this.dtFrom.TabIndex = 81;
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.label5);
            this.grpDetail.Controls.Add(this.tabDgv);
            this.grpDetail.Controls.Add(this.cmbShow);
            this.grpDetail.Controls.Add(this.btnPrev);
            this.grpDetail.Controls.Add(this.btnMPrev);
            this.grpDetail.Controls.Add(this.btnNext);
            this.grpDetail.Controls.Add(this.btnMNext);
            this.grpDetail.Controls.Add(this.lblPage);
            this.grpDetail.Controls.Add(this.txtPage);
            this.grpDetail.Controls.Add(this.lblTotal);
            this.grpDetail.Controls.Add(this.dgvInquiry);
            this.grpDetail.Location = new System.Drawing.Point(9, 136);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(799, 323);
            this.grpDetail.TabIndex = 83;
            this.grpDetail.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(117, 297);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "/";
            // 
            // tabDgv
            // 
            this.tabDgv.Location = new System.Drawing.Point(4, 9);
            this.tabDgv.Name = "tabDgv";
            this.tabDgv.SelectedIndex = 0;
            this.tabDgv.Size = new System.Drawing.Size(783, 24);
            this.tabDgv.TabIndex = 32;
            this.tabDgv.Click += new System.EventHandler(this.tabDgv_Click);
            // 
            // cmbShow
            // 
            this.cmbShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(743, 291);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 17;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(45, 291);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(20, 23);
            this.btnPrev.TabIndex = 13;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(8, 291);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(31, 23);
            this.btnMPrev.TabIndex = 12;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(167, 291);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 23);
            this.btnNext.TabIndex = 15;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(193, 291);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 23);
            this.btnMNext.TabIndex = 16;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(139, 296);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(0, 13);
            this.lblPage.TabIndex = 9;
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(71, 293);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(40, 20);
            this.txtPage.TabIndex = 14;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPage.Leave += new System.EventHandler(this.txtPage_Leave);
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(624, 296);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 30;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // dgvInquiry
            // 
            this.dgvInquiry.AllowUserToAddRows = false;
            this.dgvInquiry.AllowUserToDeleteRows = false;
            this.dgvInquiry.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInquiry.Location = new System.Drawing.Point(4, 33);
            this.dgvInquiry.Name = "dgvInquiry";
            this.dgvInquiry.ReadOnly = true;
            this.dgvInquiry.Size = new System.Drawing.Size(782, 254);
            this.dgvInquiry.TabIndex = 5;
            this.dgvInquiry.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvInquiry_CellDoubleClick);
            // 
            // btnReset
            // 
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(710, 39);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(623, 15);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 78;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Enabled = false;
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(492, 42);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(200, 20);
            this.dtTo.TabIndex = 80;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(140, 15);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 79;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(427, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 79;
            this.label3.Text = "To Date ";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(710, 15);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 81;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(417, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 78;
            this.label4.Text = "From Date ";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(54, 15);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 77;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(710, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbFilter
            // 
            this.cmbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilter.FormattingEnabled = true;
            this.cmbFilter.Location = new System.Drawing.Point(62, 41);
            this.cmbFilter.Name = "cmbFilter";
            this.cmbFilter.Size = new System.Drawing.Size(271, 21);
            this.cmbFilter.TabIndex = 2;
            this.cmbFilter.SelectedIndexChanged += new System.EventHandler(this.cmbFilter_SelectedIndexChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(5, 41);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(39, 13);
            this.Label1.TabIndex = 37;
            this.Label1.Text = "Criteria";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(62, 16);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(271, 20);
            this.txtFilter.TabIndex = 1;
            this.txtFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFilter_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Search";
            // 
            // grpFooter
            // 
            this.grpFooter.Controls.Add(this.btnPreview);
            this.grpFooter.Controls.Add(this.btnApproval);
            this.grpFooter.Controls.Add(this.pictureBox1);
            this.grpFooter.Controls.Add(this.btnSelect);
            this.grpFooter.Controls.Add(this.btnDelete);
            this.grpFooter.Controls.Add(this.btnExit);
            this.grpFooter.Controls.Add(this.btnNew);
            this.grpFooter.Location = new System.Drawing.Point(9, 460);
            this.grpFooter.Name = "grpFooter";
            this.grpFooter.Size = new System.Drawing.Size(799, 50);
            this.grpFooter.TabIndex = 84;
            this.grpFooter.TabStop = false;
            // 
            // btnApproval
            // 
            this.btnApproval.Location = new System.Drawing.Point(542, 15);
            this.btnApproval.Name = "btnApproval";
            this.btnApproval.Size = new System.Drawing.Size(75, 23);
            this.btnApproval.TabIndex = 85;
            this.btnApproval.Text = "Approval";
            this.btnApproval.UseVisualStyleBackColor = true;
            this.btnApproval.Click += new System.EventHandler(this.btnApproval_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ISBS_New.Properties.Resources.Question;
            this.pictureBox1.Location = new System.Drawing.Point(8, 14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 30);
            this.pictureBox1.TabIndex = 82;
            this.pictureBox1.TabStop = false;
            // 
            // grpHeader
            // 
            this.grpHeader.Controls.Add(this.label2);
            this.grpHeader.Controls.Add(this.dtFrom);
            this.grpHeader.Controls.Add(this.txtFilter);
            this.grpHeader.Controls.Add(this.Label1);
            this.grpHeader.Controls.Add(this.btnReset);
            this.grpHeader.Controls.Add(this.cmbFilter);
            this.grpHeader.Controls.Add(this.dtTo);
            this.grpHeader.Controls.Add(this.btnSearch);
            this.grpHeader.Controls.Add(this.label4);
            this.grpHeader.Controls.Add(this.label3);
            this.grpHeader.Location = new System.Drawing.Point(9, 63);
            this.grpHeader.Name = "grpHeader";
            this.grpHeader.Size = new System.Drawing.Size(799, 72);
            this.grpHeader.TabIndex = 85;
            this.grpHeader.TabStop = false;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(229, 15);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(75, 23);
            this.btnPreview.TabIndex = 86;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // InquiryV2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 512);
            this.Controls.Add(this.grpHeader);
            this.Controls.Add(this.grpFooter);
            this.Controls.Add(this.grpDetail);
            this.Name = "InquiryV2";
            this.Text = "InquiryV2";
            this.Load += new System.EventHandler(this.InquiryV2_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InquiryV2_FormClosed);
            this.grpDetail.ResumeLayout(false);
            this.grpDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInquiry)).EndInit();
            this.grpFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grpHeader.ResumeLayout(false);
            this.grpHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerRefresh;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.Button btnPrev;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnMNext;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.DataGridView dgvInquiry;
        internal System.Windows.Forms.Button btnReset;
        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.DateTimePicker dtTo;
        internal System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.ComboBox cmbFilter;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox txtFilter;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grpFooter;
        private System.Windows.Forms.GroupBox grpHeader;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip ToolTipNotes;
        private System.Windows.Forms.TabControl tabDgv;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.Button btnApproval;
        internal System.Windows.Forms.Button btnPreview;
    }
}