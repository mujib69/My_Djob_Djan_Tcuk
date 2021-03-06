﻿namespace ISBS_New
{
    partial class GlobalInquiry
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblForm = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelete = new MetroFramework.Controls.MetroButton();
            this.btnUbahReceiptDate = new MetroFramework.Controls.MetroButton();
            this.btnPrev = new MetroFramework.Controls.MetroButton();
            this.btnNext = new MetroFramework.Controls.MetroButton();
            this.btnMNext = new MetroFramework.Controls.MetroButton();
            this.btnMPrev = new MetroFramework.Controls.MetroButton();
            this.btnCompleted = new MetroFramework.Controls.MetroButton();
            this.btnOnProgress = new MetroFramework.Controls.MetroButton();
            this.btnSearch = new MetroFramework.Controls.MetroButton();
            this.btnReset = new MetroFramework.Controls.MetroButton();
            this.btnSelect = new MetroFramework.Controls.MetroButton();
            this.btnExit = new MetroFramework.Controls.MetroButton();
            this.btnClosed = new MetroFramework.Controls.MetroButton();
            this.btnNew = new MetroFramework.Controls.MetroButton();
            this.cmbCriteria = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.metroGrid1 = new MetroFramework.Controls.MetroGrid();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.cachedReportRFQ1 = new ISBS_New.Purchase.PurchaseQuotation.CachedReportRFQ();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.metroGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy";
            this.dtFrom.Enabled = false;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(569, 11);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(200, 20);
            this.dtFrom.TabIndex = 81;
            // 
            // lblForm
            // 
            this.lblForm.AutoSize = true;
            this.lblForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForm.Location = new System.Drawing.Point(20, 14);
            this.lblForm.Name = "lblForm";
            this.lblForm.Size = new System.Drawing.Size(97, 13);
            this.lblForm.TabIndex = 90;
            this.lblForm.Text = "Sales Quotation";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(13, 40);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(39, 13);
            this.Label1.TabIndex = 37;
            this.Label1.Text = "Criteria";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(70, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(271, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Search";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(785, 469);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 30;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(76, 466);
            this.txtPage.MaxLength = 6;
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 14;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPage.Leave += new System.EventHandler(this.txtPage_Leave);
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(111, 469);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(15, 13);
            this.lblPage.TabIndex = 9;
            this.lblPage.Text = "/ ";
            // 
            // cmbShow
            // 
            this.cmbShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(898, 465);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 17;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Enabled = false;
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(569, 38);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(200, 20);
            this.dtTo.TabIndex = 80;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(517, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 79;
            this.label3.Text = "To Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(507, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 78;
            this.label4.Text = "From Date";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnUbahReceiptDate);
            this.groupBox1.Controls.Add(this.btnPrev);
            this.groupBox1.Controls.Add(this.btnNext);
            this.groupBox1.Controls.Add(this.btnMNext);
            this.groupBox1.Controls.Add(this.btnMPrev);
            this.groupBox1.Controls.Add(this.btnCompleted);
            this.groupBox1.Controls.Add(this.btnOnProgress);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.btnReset);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.btnClosed);
            this.groupBox1.Controls.Add(this.btnNew);
            this.groupBox1.Controls.Add(this.dtFrom);
            this.groupBox1.Controls.Add(this.dtTo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmbCriteria);
            this.groupBox1.Controls.Add(this.Label1);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblTotal);
            this.groupBox1.Controls.Add(this.txtPage);
            this.groupBox1.Controls.Add(this.lblPage);
            this.groupBox1.Controls.Add(this.cmbShow);
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Controls.Add(this.metroGrid1);
            this.groupBox1.Location = new System.Drawing.Point(23, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(946, 522);
            this.groupBox1.TabIndex = 89;
            this.groupBox1.TabStop = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(500, 491);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 99;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseSelectable = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click_1);
            // 
            // btnUbahReceiptDate
            // 
            this.btnUbahReceiptDate.Location = new System.Drawing.Point(366, 492);
            this.btnUbahReceiptDate.Name = "btnUbahReceiptDate";
            this.btnUbahReceiptDate.Size = new System.Drawing.Size(127, 23);
            this.btnUbahReceiptDate.TabIndex = 98;
            this.btnUbahReceiptDate.Text = "Ubah Receipt Date";
            this.btnUbahReceiptDate.UseSelectable = true;
            this.btnUbahReceiptDate.Click += new System.EventHandler(this.btnUbahReceiptDate_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(41, 465);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(25, 23);
            this.btnPrev.TabIndex = 96;
            this.btnPrev.Text = "<";
            this.btnPrev.UseSelectable = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(138, 465);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(25, 23);
            this.btnNext.TabIndex = 95;
            this.btnNext.Text = ">";
            this.btnNext.UseSelectable = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(169, 465);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(25, 23);
            this.btnMNext.TabIndex = 94;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseSelectable = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(10, 465);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(25, 23);
            this.btnMPrev.TabIndex = 93;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseSelectable = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnCompleted
            // 
            this.btnCompleted.Location = new System.Drawing.Point(489, 67);
            this.btnCompleted.Name = "btnCompleted";
            this.btnCompleted.Size = new System.Drawing.Size(451, 23);
            this.btnCompleted.TabIndex = 92;
            this.btnCompleted.Text = "Completed";
            this.btnCompleted.UseCustomBackColor = true;
            this.btnCompleted.UseCustomForeColor = true;
            this.btnCompleted.UseSelectable = true;
            this.btnCompleted.Click += new System.EventHandler(this.btnCompleted_Click);
            // 
            // btnOnProgress
            // 
            this.btnOnProgress.BackColor = System.Drawing.Color.White;
            this.btnOnProgress.Location = new System.Drawing.Point(10, 67);
            this.btnOnProgress.Name = "btnOnProgress";
            this.btnOnProgress.Size = new System.Drawing.Size(473, 23);
            this.btnOnProgress.TabIndex = 91;
            this.btnOnProgress.Text = "On Progress";
            this.btnOnProgress.UseCustomBackColor = true;
            this.btnOnProgress.UseCustomForeColor = true;
            this.btnOnProgress.UseSelectable = true;
            this.btnOnProgress.Click += new System.EventHandler(this.btnOnProgress_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(865, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 90;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseSelectable = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(865, 38);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 89;
            this.btnReset.Text = "Reset";
            this.btnReset.UseSelectable = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(784, 492);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 88;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseSelectable = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(865, 492);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 87;
            this.btnExit.Text = "Exit";
            this.btnExit.UseSelectable = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnClosed
            // 
            this.btnClosed.Location = new System.Drawing.Point(284, 492);
            this.btnClosed.Name = "btnClosed";
            this.btnClosed.Size = new System.Drawing.Size(75, 23);
            this.btnClosed.TabIndex = 86;
            this.btnClosed.Text = "Closed";
            this.btnClosed.UseSelectable = true;
            this.btnClosed.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(203, 492);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 85;
            this.btnNew.Text = "New";
            this.btnNew.UseSelectable = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // cmbCriteria
            // 
            this.cmbCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCriteria.FormattingEnabled = true;
            this.cmbCriteria.Location = new System.Drawing.Point(70, 40);
            this.cmbCriteria.Name = "cmbCriteria";
            this.cmbCriteria.Size = new System.Drawing.Size(271, 21);
            this.cmbCriteria.TabIndex = 2;
            this.cmbCriteria.TextChanged += new System.EventHandler(this.cmbCriteria_TextChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(10, 99);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(930, 360);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // metroGrid1
            // 
            this.metroGrid1.AllowUserToAddRows = false;
            this.metroGrid1.AllowUserToResizeColumns = false;
            this.metroGrid1.AllowUserToResizeRows = false;
            this.metroGrid1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.metroGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.metroGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metroGrid1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.metroGrid1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.metroGrid1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.metroGrid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.metroGrid1.DefaultCellStyle = dataGridViewCellStyle2;
            this.metroGrid1.EnableHeadersVisualStyles = false;
            this.metroGrid1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.metroGrid1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.metroGrid1.Location = new System.Drawing.Point(10, 96);
            this.metroGrid1.Name = "metroGrid1";
            this.metroGrid1.ReadOnly = true;
            this.metroGrid1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.metroGrid1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.metroGrid1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.metroGrid1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.metroGrid1.Size = new System.Drawing.Size(671, 275);
            this.metroGrid1.TabIndex = 97;
            this.metroGrid1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroGrid1.Visible = false;
            this.metroGrid1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.metroGrid1_CellFormatting);
            // 
            // GlobalInquiry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 573);
            this.Controls.Add(this.lblForm);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "GlobalInquiry";
            this.Resizable = false;
            this.Text = "GlobalInquiry";
            this.Load += new System.EventHandler(this.GlobalInquiry_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.metroGrid1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label lblForm;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox txtSearch;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.ComboBox cmbCriteria;
        internal System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Timer timerRefresh;
        internal MetroFramework.Controls.MetroButton btnClosed;
        internal MetroFramework.Controls.MetroButton btnNew;
        internal MetroFramework.Controls.MetroButton btnSelect;
        internal MetroFramework.Controls.MetroButton btnExit;
        internal MetroFramework.Controls.MetroButton btnPrev;
        internal MetroFramework.Controls.MetroButton btnNext;
        internal MetroFramework.Controls.MetroButton btnMNext;
        internal MetroFramework.Controls.MetroButton btnMPrev;
        internal MetroFramework.Controls.MetroButton btnCompleted;
        internal MetroFramework.Controls.MetroButton btnOnProgress;
        internal MetroFramework.Controls.MetroButton btnSearch;
        internal MetroFramework.Controls.MetroButton btnReset;
        private ISBS_New.Purchase.PurchaseQuotation.CachedReportRFQ cachedReportRFQ1;
        private MetroFramework.Controls.MetroGrid metroGrid1;
        private MetroFramework.Controls.MetroButton btnUbahReceiptDate;
        private MetroFramework.Controls.MetroButton btnDelete;
    }
}