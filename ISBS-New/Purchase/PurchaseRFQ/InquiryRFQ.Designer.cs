﻿namespace ISBS_New.Purchase.PurchaseQuotation
{
    partial class InquiryPQ
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
            this.btnReset = new System.Windows.Forms.Button();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnMNext = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.cmbCriteria = new System.Windows.Forms.ComboBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.btnProcess = new System.Windows.Forms.Button();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.dgvPQ = new System.Windows.Forms.DataGridView();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.lblForm = new System.Windows.Forms.Label();
            this.grpCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPQ)).BeginInit();
            this.SuspendLayout();
            // 
            // btnReset
            // 
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(709, 38);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(76, 312);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 14;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(111, 315);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(15, 13);
            this.lblPage.TabIndex = 9;
            this.lblPage.Text = "/ ";
            // 
            // cmbShow
            // 
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(742, 311);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 17;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            this.cmbShow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbShow_KeyPress);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(169, 310);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 23);
            this.btnMNext.TabIndex = 16;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(143, 310);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 23);
            this.btnNext.TabIndex = 15;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(13, 310);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(31, 23);
            this.btnMPrev.TabIndex = 12;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(50, 310);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(20, 23);
            this.btnPrev.TabIndex = 13;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(709, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(203, 338);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 77;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
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
            this.cmbCriteria.SelectedIndexChanged += new System.EventHandler(this.cmbCriteria_SelectedIndexChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(627, 338);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 78;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
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
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.btnProcess);
            this.grpCustomer.Controls.Add(this.dtFrom);
            this.grpCustomer.Controls.Add(this.btnReset);
            this.grpCustomer.Controls.Add(this.btnSelect);
            this.grpCustomer.Controls.Add(this.dtTo);
            this.grpCustomer.Controls.Add(this.btnDelete);
            this.grpCustomer.Controls.Add(this.label3);
            this.grpCustomer.Controls.Add(this.btnExit);
            this.grpCustomer.Controls.Add(this.label4);
            this.grpCustomer.Controls.Add(this.btnNew);
            this.grpCustomer.Controls.Add(this.btnSearch);
            this.grpCustomer.Controls.Add(this.cmbCriteria);
            this.grpCustomer.Controls.Add(this.Label1);
            this.grpCustomer.Controls.Add(this.txtSearch);
            this.grpCustomer.Controls.Add(this.label2);
            this.grpCustomer.Controls.Add(this.lblTotal);
            this.grpCustomer.Controls.Add(this.txtPage);
            this.grpCustomer.Controls.Add(this.lblPage);
            this.grpCustomer.Controls.Add(this.cmbShow);
            this.grpCustomer.Controls.Add(this.btnMNext);
            this.grpCustomer.Controls.Add(this.btnNext);
            this.grpCustomer.Controls.Add(this.btnMPrev);
            this.grpCustomer.Controls.Add(this.btnPrev);
            this.grpCustomer.Controls.Add(this.dgvPQ);
            this.grpCustomer.Location = new System.Drawing.Point(12, 27);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(805, 375);
            this.grpCustomer.TabIndex = 78;
            this.grpCustomer.TabStop = false;
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(534, 338);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 82;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(439, 15);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(200, 20);
            this.dtFrom.TabIndex = 81;
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(439, 41);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(200, 20);
            this.dtTo.TabIndex = 80;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(296, 338);
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
            this.label3.Location = new System.Drawing.Point(374, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 79;
            this.label3.Text = "To Date ";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(714, 338);
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
            this.label4.Location = new System.Drawing.Point(364, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 78;
            this.label4.Text = "From Date ";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(70, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(271, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
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
            this.lblTotal.Location = new System.Drawing.Point(629, 315);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 30;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // dgvPQ
            // 
            this.dgvPQ.AllowUserToAddRows = false;
            this.dgvPQ.AllowUserToDeleteRows = false;
            this.dgvPQ.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPQ.Location = new System.Drawing.Point(11, 67);
            this.dgvPQ.Name = "dgvPQ";
            this.dgvPQ.ReadOnly = true;
            this.dgvPQ.Size = new System.Drawing.Size(778, 238);
            this.dgvPQ.TabIndex = 5;
            this.dgvPQ.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPR_CellDoubleClick);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 45000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // lblForm
            // 
            this.lblForm.AutoSize = true;
            this.lblForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForm.Location = new System.Drawing.Point(13, 11);
            this.lblForm.Name = "lblForm";
            this.lblForm.Size = new System.Drawing.Size(161, 13);
            this.lblForm.TabIndex = 82;
            this.lblForm.Text = "Purchase Quotation Inquiry";
            // 
            // InquiryPQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 422);
            this.Controls.Add(this.lblForm);
            this.Controls.Add(this.grpCustomer);
            this.Name = "InquiryPQ";
            this.Resizable = false;
            this.Text = "Purchase Quotation Inquiry";
            this.Load += new System.EventHandler(this.InquiryPR_Load);
            this.Shown += new System.EventHandler(this.InquiryPR_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InquiryPR_FormClosed);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPQ)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnReset;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.Button btnMNext;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnPrev;
        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.ComboBox cmbCriteria;
        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.TextBox txtSearch;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Timer timerRefresh;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.DataGridView dgvPQ;
        private System.Windows.Forms.Label lblForm;
        internal System.Windows.Forms.Button btnProcess;
    }
}