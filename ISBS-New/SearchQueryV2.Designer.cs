namespace ISBS_New
{
    partial class SearchQueryV2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>s
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFilter1 = new System.Windows.Forms.ComboBox();
            this.txtFilter1 = new System.Windows.Forms.TextBox();
            this.dgvSearch = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grpHeader = new System.Windows.Forms.GroupBox();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPage = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnMNext = new System.Windows.Forms.Button();
            this.grpFooter = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.ToolTipNotes = new System.Windows.Forms.ToolTip(this.components);
            this.cmbMask = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).BeginInit();
            this.grpHeader.SuspendLayout();
            this.grpDetail.SuspendLayout();
            this.grpFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Criteria";
            // 
            // cmbFilter1
            // 
            this.cmbFilter1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilter1.FormattingEnabled = true;
            this.cmbFilter1.Location = new System.Drawing.Point(54, 35);
            this.cmbFilter1.Name = "cmbFilter1";
            this.cmbFilter1.Size = new System.Drawing.Size(200, 21);
            this.cmbFilter1.TabIndex = 10;
            // 
            // txtFilter1
            // 
            this.txtFilter1.Location = new System.Drawing.Point(53, 12);
            this.txtFilter1.Name = "txtFilter1";
            this.txtFilter1.Size = new System.Drawing.Size(316, 20);
            this.txtFilter1.TabIndex = 5;
            this.txtFilter1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFilter1_KeyPress);
            // 
            // dgvSearch
            // 
            this.dgvSearch.AllowUserToAddRows = false;
            this.dgvSearch.AllowUserToDeleteRows = false;
            this.dgvSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearch.Location = new System.Drawing.Point(6, 12);
            this.dgvSearch.Name = "dgvSearch";
            this.dgvSearch.ReadOnly = true;
            this.dgvSearch.Size = new System.Drawing.Size(689, 244);
            this.dgvSearch.TabIndex = 95;
            this.dgvSearch.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearch_CellDoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Search";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(617, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 25);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grpHeader
            // 
            this.grpHeader.Controls.Add(this.cmbMask);
            this.grpHeader.Controls.Add(this.chkAll);
            this.grpHeader.Controls.Add(this.btnReset);
            this.grpHeader.Controls.Add(this.label1);
            this.grpHeader.Controls.Add(this.cmbFilter1);
            this.grpHeader.Controls.Add(this.txtFilter1);
            this.grpHeader.Controls.Add(this.label2);
            this.grpHeader.Controls.Add(this.btnSearch);
            this.grpHeader.Location = new System.Drawing.Point(5, 61);
            this.grpHeader.Name = "grpHeader";
            this.grpHeader.Size = new System.Drawing.Size(702, 65);
            this.grpHeader.TabIndex = 94;
            this.grpHeader.TabStop = false;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(273, 38);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(71, 17);
            this.chkAll.TabIndex = 125;
            this.chkAll.Text = "Check All";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // btnReset
            // 
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(617, 36);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(80, 25);
            this.btnReset.TabIndex = 124;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.label5);
            this.grpDetail.Controls.Add(this.lblPage);
            this.grpDetail.Controls.Add(this.lblTotal);
            this.grpDetail.Controls.Add(this.dgvSearch);
            this.grpDetail.Controls.Add(this.txtPage);
            this.grpDetail.Controls.Add(this.btnNext);
            this.grpDetail.Controls.Add(this.btnPrev);
            this.grpDetail.Controls.Add(this.cmbShow);
            this.grpDetail.Controls.Add(this.btnMPrev);
            this.grpDetail.Controls.Add(this.btnMNext);
            this.grpDetail.Location = new System.Drawing.Point(5, 125);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(702, 293);
            this.grpDetail.TabIndex = 106;
            this.grpDetail.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(102, 267);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 13);
            this.label5.TabIndex = 125;
            this.label5.Text = "/";
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(124, 266);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(0, 13);
            this.lblPage.TabIndex = 124;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(539, 267);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 123;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(69, 264);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 118;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPage.Leave += new System.EventHandler(this.txtPage_Leave);
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(154, 262);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 23);
            this.btnNext.TabIndex = 119;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(43, 262);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(20, 23);
            this.btnPrev.TabIndex = 117;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // cmbShow
            // 
            this.cmbShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(653, 264);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 121;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(6, 262);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(31, 23);
            this.btnMPrev.TabIndex = 116;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(180, 262);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 23);
            this.btnMNext.TabIndex = 120;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // grpFooter
            // 
            this.grpFooter.Controls.Add(this.pictureBox1);
            this.grpFooter.Controls.Add(this.btnSelect);
            this.grpFooter.Controls.Add(this.btnExit);
            this.grpFooter.Location = new System.Drawing.Point(5, 416);
            this.grpFooter.Name = "grpFooter";
            this.grpFooter.Size = new System.Drawing.Size(702, 40);
            this.grpFooter.TabIndex = 107;
            this.grpFooter.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ISBS_New.Properties.Resources.Question;
            this.pictureBox1.Location = new System.Drawing.Point(7, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 30);
            this.pictureBox1.TabIndex = 126;
            this.pictureBox1.TabStop = false;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(531, 11);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(80, 25);
            this.btnSelect.TabIndex = 114;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(617, 11);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(80, 25);
            this.btnExit.TabIndex = 115;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cmbMask
            // 
            this.cmbMask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMask.FormattingEnabled = true;
            this.cmbMask.Location = new System.Drawing.Point(54, 34);
            this.cmbMask.Name = "cmbMask";
            this.cmbMask.Size = new System.Drawing.Size(200, 21);
            this.cmbMask.TabIndex = 126;
            this.cmbMask.SelectedIndexChanged += new System.EventHandler(this.cmbMask_SelectedIndexChanged);
            // 
            // SearchQueryV2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 479);
            this.Controls.Add(this.grpFooter);
            this.Controls.Add(this.grpDetail);
            this.Controls.Add(this.grpHeader);
            this.Name = "SearchQueryV2";
            this.Resizable = false;
            this.Text = "SearchV1";
            this.Load += new System.EventHandler(this.SearchV1_Load);
            this.Shown += new System.EventHandler(this.SearchQueryV2_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).EndInit();
            this.grpHeader.ResumeLayout(false);
            this.grpHeader.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            this.grpDetail.PerformLayout();
            this.grpFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFilter1;
        private System.Windows.Forms.TextBox txtFilter1;
        private System.Windows.Forms.DataGridView dgvSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grpHeader;
        private System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnPrev;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnMNext;
        private System.Windows.Forms.GroupBox grpFooter;
        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnReset;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip ToolTipNotes;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.ComboBox cmbMask;


    }
}