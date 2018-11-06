namespace ISBS_New.Purchase.ParkedItem
{
    partial class InquiryParkedItem
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
            this.components = new System.ComponentModel.Container();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.grpParkedItem = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbCriteria = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnMNext = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.dgvParkedItemH = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.Detail = new System.Windows.Forms.GroupBox();
            this.dgvParkedItemD = new System.Windows.Forms.DataGridView();
            this.lblTotalD = new System.Windows.Forms.Label();
            this.txtPageD = new System.Windows.Forms.TextBox();
            this.lblPageD = new System.Windows.Forms.Label();
            this.cmbShowD = new System.Windows.Forms.ComboBox();
            this.btnMNextD = new System.Windows.Forms.Button();
            this.btnNextD = new System.Windows.Forms.Button();
            this.btnMPrevD = new System.Windows.Forms.Button();
            this.btnPrevD = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.grpParkedItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParkedItemH)).BeginInit();
            this.Detail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParkedItemD)).BeginInit();
            this.SuspendLayout();
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
            // grpParkedItem
            // 
            this.grpParkedItem.Controls.Add(this.dtFrom);
            this.grpParkedItem.Controls.Add(this.btnReset);
            this.grpParkedItem.Controls.Add(this.dtTo);
            this.grpParkedItem.Controls.Add(this.label3);
            this.grpParkedItem.Controls.Add(this.label4);
            this.grpParkedItem.Controls.Add(this.btnSearch);
            this.grpParkedItem.Controls.Add(this.cmbCriteria);
            this.grpParkedItem.Controls.Add(this.Label1);
            this.grpParkedItem.Controls.Add(this.txtSearch);
            this.grpParkedItem.Controls.Add(this.label2);
            this.grpParkedItem.Controls.Add(this.lblTotal);
            this.grpParkedItem.Controls.Add(this.txtPage);
            this.grpParkedItem.Controls.Add(this.lblPage);
            this.grpParkedItem.Controls.Add(this.cmbShow);
            this.grpParkedItem.Controls.Add(this.btnMNext);
            this.grpParkedItem.Controls.Add(this.btnNext);
            this.grpParkedItem.Controls.Add(this.btnMPrev);
            this.grpParkedItem.Controls.Add(this.btnPrev);
            this.grpParkedItem.Controls.Add(this.dgvParkedItemH);
            this.grpParkedItem.Location = new System.Drawing.Point(11, 63);
            this.grpParkedItem.Name = "grpParkedItem";
            this.grpParkedItem.Size = new System.Drawing.Size(866, 505);
            this.grpParkedItem.TabIndex = 87;
            this.grpParkedItem.TabStop = false;
            this.grpParkedItem.Text = "Header";
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
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(439, 41);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(200, 20);
            this.dtTo.TabIndex = 80;
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(364, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 78;
            this.label4.Text = "From Date ";
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
            this.lblTotal.Location = new System.Drawing.Point(629, 313);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 30;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(76, 310);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 14;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(111, 313);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(15, 13);
            this.lblPage.TabIndex = 9;
            this.lblPage.Text = "/ ";
            // 
            // cmbShow
            // 
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(742, 309);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 17;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            this.cmbShow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbShow_KeyPress);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(169, 308);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 23);
            this.btnMNext.TabIndex = 16;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(143, 308);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 23);
            this.btnNext.TabIndex = 15;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(13, 308);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(31, 23);
            this.btnMPrev.TabIndex = 12;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(50, 308);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(20, 23);
            this.btnPrev.TabIndex = 13;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // dgvParkedItemH
            // 
            this.dgvParkedItemH.AllowUserToAddRows = false;
            this.dgvParkedItemH.AllowUserToDeleteRows = false;
            this.dgvParkedItemH.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParkedItemH.Location = new System.Drawing.Point(11, 67);
            this.dgvParkedItemH.Name = "dgvParkedItemH";
            this.dgvParkedItemH.Size = new System.Drawing.Size(778, 237);
            this.dgvParkedItemH.TabIndex = 5;
            this.dgvParkedItemH.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParkedItemH_CellClick);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(741, 399);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 81;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 45000;
            // 
            // Detail
            // 
            this.Detail.Controls.Add(this.dgvParkedItemD);
            this.Detail.Controls.Add(this.lblTotalD);
            this.Detail.Controls.Add(this.txtPageD);
            this.Detail.Controls.Add(this.lblPageD);
            this.Detail.Controls.Add(this.cmbShowD);
            this.Detail.Controls.Add(this.btnMNextD);
            this.Detail.Controls.Add(this.btnNextD);
            this.Detail.Controls.Add(this.btnMPrevD);
            this.Detail.Controls.Add(this.btnPrevD);
            this.Detail.Location = new System.Drawing.Point(11, 437);
            this.Detail.Name = "Detail";
            this.Detail.Size = new System.Drawing.Size(805, 191);
            this.Detail.TabIndex = 89;
            this.Detail.TabStop = false;
            this.Detail.Text = "Detail";
            // 
            // dgvParkedItemD
            // 
            this.dgvParkedItemD.AllowUserToAddRows = false;
            this.dgvParkedItemD.AllowUserToDeleteRows = false;
            this.dgvParkedItemD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParkedItemD.Location = new System.Drawing.Point(11, 19);
            this.dgvParkedItemD.Name = "dgvParkedItemD";
            this.dgvParkedItemD.ReadOnly = true;
            this.dgvParkedItemD.Size = new System.Drawing.Size(778, 136);
            this.dgvParkedItemD.TabIndex = 82;
            // 
            // lblTotalD
            // 
            this.lblTotalD.AutoSize = true;
            this.lblTotalD.Location = new System.Drawing.Point(629, 165);
            this.lblTotalD.Name = "lblTotalD";
            this.lblTotalD.Size = new System.Drawing.Size(70, 13);
            this.lblTotalD.TabIndex = 38;
            this.lblTotalD.Text = "Total Rows : ";
            // 
            // txtPageD
            // 
            this.txtPageD.Location = new System.Drawing.Point(76, 162);
            this.txtPageD.Name = "txtPageD";
            this.txtPageD.Size = new System.Drawing.Size(30, 20);
            this.txtPageD.TabIndex = 34;
            this.txtPageD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPageD.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPageD_KeyPress);
            // 
            // lblPageD
            // 
            this.lblPageD.AutoSize = true;
            this.lblPageD.Location = new System.Drawing.Point(111, 165);
            this.lblPageD.Name = "lblPageD";
            this.lblPageD.Size = new System.Drawing.Size(15, 13);
            this.lblPageD.TabIndex = 31;
            this.lblPageD.Text = "/ ";
            // 
            // cmbShowD
            // 
            this.cmbShowD.FormattingEnabled = true;
            this.cmbShowD.Location = new System.Drawing.Point(742, 161);
            this.cmbShowD.Name = "cmbShowD";
            this.cmbShowD.Size = new System.Drawing.Size(42, 21);
            this.cmbShowD.TabIndex = 37;
            this.cmbShowD.SelectedIndexChanged += new System.EventHandler(this.cmbShowD_SelectedIndexChanged);
            this.cmbShowD.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbShowD_KeyPress);
            // 
            // btnMNextD
            // 
            this.btnMNextD.Location = new System.Drawing.Point(169, 160);
            this.btnMNextD.Name = "btnMNextD";
            this.btnMNextD.Size = new System.Drawing.Size(30, 23);
            this.btnMNextD.TabIndex = 36;
            this.btnMNextD.Text = ">>";
            this.btnMNextD.UseVisualStyleBackColor = true;
            this.btnMNextD.Click += new System.EventHandler(this.btnMNextD_Click);
            // 
            // btnNextD
            // 
            this.btnNextD.Location = new System.Drawing.Point(143, 160);
            this.btnNextD.Name = "btnNextD";
            this.btnNextD.Size = new System.Drawing.Size(20, 23);
            this.btnNextD.TabIndex = 35;
            this.btnNextD.Text = ">";
            this.btnNextD.UseVisualStyleBackColor = true;
            this.btnNextD.Click += new System.EventHandler(this.btnNextD_Click);
            // 
            // btnMPrevD
            // 
            this.btnMPrevD.Location = new System.Drawing.Point(13, 160);
            this.btnMPrevD.Name = "btnMPrevD";
            this.btnMPrevD.Size = new System.Drawing.Size(31, 23);
            this.btnMPrevD.TabIndex = 32;
            this.btnMPrevD.Text = "<<";
            this.btnMPrevD.UseVisualStyleBackColor = true;
            this.btnMPrevD.Click += new System.EventHandler(this.btnMPrevD_Click);
            // 
            // btnPrevD
            // 
            this.btnPrevD.Location = new System.Drawing.Point(50, 160);
            this.btnPrevD.Name = "btnPrevD";
            this.btnPrevD.Size = new System.Drawing.Size(20, 23);
            this.btnPrevD.TabIndex = 33;
            this.btnPrevD.Text = "<";
            this.btnPrevD.UseVisualStyleBackColor = true;
            this.btnPrevD.Click += new System.EventHandler(this.btnPrevD_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(660, 400);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 90;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // InquiryParkedItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.Detail);
            this.Controls.Add(this.grpParkedItem);
            this.Controls.Add(this.btnExit);
            this.Name = "InquiryParkedItem";
            this.Resizable = false;
            this.Text = "Inquiry Parked Item";
            this.Load += new System.EventHandler(this.InquiryParkedItem_Load);
            this.Shown += new System.EventHandler(this.InquiryParkedItem_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InquiryParkedItem_FormClosed);
            this.grpParkedItem.ResumeLayout(false);
            this.grpParkedItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParkedItemH)).EndInit();
            this.Detail.ResumeLayout(false);
            this.Detail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParkedItemD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.GroupBox grpParkedItem;
        internal System.Windows.Forms.Button btnReset;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.ComboBox cmbCriteria;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox txtSearch;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.Button btnMNext;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnPrev;
        internal System.Windows.Forms.DataGridView dgvParkedItemH;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.GroupBox Detail;
        internal System.Windows.Forms.DataGridView dgvParkedItemD;
        internal System.Windows.Forms.Label lblTotalD;
        internal System.Windows.Forms.TextBox txtPageD;
        internal System.Windows.Forms.Label lblPageD;
        internal System.Windows.Forms.ComboBox cmbShowD;
        internal System.Windows.Forms.Button btnMNextD;
        internal System.Windows.Forms.Button btnNextD;
        internal System.Windows.Forms.Button btnMPrevD;
        internal System.Windows.Forms.Button btnPrevD;
        internal System.Windows.Forms.Button btnSelect;
    }
}
