namespace ISBS_New.Inventory.GoodReceiptNT
{
    partial class PopUpGRNT
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.grpGrid = new System.Windows.Forms.GroupBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.dgvSearch = new System.Windows.Forms.DataGridView();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.lblPage = new System.Windows.Forms.Label();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnMNext = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.grpGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(340, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 34;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grpGrid
            // 
            this.grpGrid.Controls.Add(this.btnSearch);
            this.grpGrid.Controls.Add(this.btnSelect);
            this.grpGrid.Controls.Add(this.btnExit);
            this.grpGrid.Controls.Add(this.lblTotal);
            this.grpGrid.Controls.Add(this.txtPage);
            this.grpGrid.Controls.Add(this.dgvSearch);
            this.grpGrid.Controls.Add(this.txtSearch);
            this.grpGrid.Controls.Add(this.Label5);
            this.grpGrid.Controls.Add(this.lblPage);
            this.grpGrid.Controls.Add(this.cmbShow);
            this.grpGrid.Controls.Add(this.btnMNext);
            this.grpGrid.Controls.Add(this.btnNext);
            this.grpGrid.Controls.Add(this.btnMPrev);
            this.grpGrid.Controls.Add(this.btnPrev);
            this.grpGrid.Location = new System.Drawing.Point(23, 57);
            this.grpGrid.Name = "grpGrid";
            this.grpGrid.Size = new System.Drawing.Size(700, 368);
            this.grpGrid.TabIndex = 86;
            this.grpGrid.TabStop = false;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(541, 332);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 82;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(621, 332);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 83;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(538, 309);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 30;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(71, 307);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 5;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // dgvSearch
            // 
            this.dgvSearch.AllowUserToAddRows = false;
            this.dgvSearch.AllowUserToDeleteRows = false;
            this.dgvSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearch.Location = new System.Drawing.Point(7, 40);
            this.dgvSearch.Name = "dgvSearch";
            this.dgvSearch.ReadOnly = true;
            this.dgvSearch.Size = new System.Drawing.Size(687, 259);
            this.dgvSearch.TabIndex = 2;
            this.dgvSearch.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearch_CellDoubleClick);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(63, 13);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(271, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(6, 16);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(41, 13);
            this.Label5.TabIndex = 14;
            this.Label5.Text = "Search";
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(106, 310);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(15, 13);
            this.lblPage.TabIndex = 9;
            this.lblPage.Text = "/ ";
            // 
            // cmbShow
            // 
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(652, 306);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 8;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(182, 305);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 23);
            this.btnMNext.TabIndex = 7;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(156, 305);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(20, 23);
            this.btnNext.TabIndex = 6;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(8, 305);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(31, 22);
            this.btnMPrev.TabIndex = 3;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(45, 305);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(20, 23);
            this.btnPrev.TabIndex = 4;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // PopUpGRNT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 433);
            this.Controls.Add(this.grpGrid);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(751, 433);
            this.MinimumSize = new System.Drawing.Size(751, 433);
            this.Name = "PopUpGRNT";
            this.Text = "Good Receipt Nota Transfer";
            this.Load += new System.EventHandler(this.PopUpGRNT_Load);
            this.grpGrid.ResumeLayout(false);
            this.grpGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.GroupBox grpGrid;
        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.DataGridView dgvSearch;
        internal System.Windows.Forms.TextBox txtSearch;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.Button btnMNext;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnPrev;
    }
}