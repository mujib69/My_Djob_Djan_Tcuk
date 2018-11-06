namespace ISBS_New
{
    partial class SearchQueryV1
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.cmbMask = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFilter1 = new System.Windows.Forms.ComboBox();
            this.txtFilter1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgvSearch = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnMNext = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnReset);
            this.groupBox1.Controls.Add(this.cmbMask);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbFilter1);
            this.groupBox1.Controls.Add(this.txtFilter1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Location = new System.Drawing.Point(10, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(638, 75);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(379, 45);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(80, 25);
            this.btnReset.TabIndex = 13;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // cmbMask
            // 
            this.cmbMask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMask.FormattingEnabled = true;
            this.cmbMask.Location = new System.Drawing.Point(57, 40);
            this.cmbMask.Name = "cmbMask";
            this.cmbMask.Size = new System.Drawing.Size(200, 21);
            this.cmbMask.TabIndex = 12;
            this.cmbMask.SelectedIndexChanged += new System.EventHandler(this.cmbMask_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Criteria";
            // 
            // cmbFilter1
            // 
            this.cmbFilter1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilter1.FormattingEnabled = true;
            this.cmbFilter1.Location = new System.Drawing.Point(57, 40);
            this.cmbFilter1.Name = "cmbFilter1";
            this.cmbFilter1.Size = new System.Drawing.Size(200, 21);
            this.cmbFilter1.TabIndex = 10;
            // 
            // txtFilter1
            // 
            this.txtFilter1.Location = new System.Drawing.Point(57, 14);
            this.txtFilter1.Name = "txtFilter1";
            this.txtFilter1.Size = new System.Drawing.Size(316, 20);
            this.txtFilter1.TabIndex = 5;
            this.txtFilter1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFilter1_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Search";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(379, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 25);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dgvSearch
            // 
            this.dgvSearch.AllowUserToAddRows = false;
            this.dgvSearch.AllowUserToDeleteRows = false;
            this.dgvSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearch.Location = new System.Drawing.Point(9, 140);
            this.dgvSearch.Name = "dgvSearch";
            this.dgvSearch.ReadOnly = true;
            this.dgvSearch.Size = new System.Drawing.Size(639, 280);
            this.dgvSearch.TabIndex = 1;
            this.dgvSearch.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSearch_CellDoubleClick);
            this.dgvSearch.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvSearch_CellFormatting);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(472, 459);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(80, 25);
            this.btnSelect.TabIndex = 92;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(552, 459);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(80, 25);
            this.btnExit.TabIndex = 93;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(469, 432);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 91;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(92, 450);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 86;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(128, 454);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(15, 13);
            this.lblPage.TabIndex = 90;
            this.lblPage.Text = "/ ";
            // 
            // cmbShow
            // 
            this.cmbShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(585, 428);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(42, 21);
            this.cmbShow.TabIndex = 89;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(204, 448);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 25);
            this.btnMNext.TabIndex = 88;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(168, 448);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(30, 25);
            this.btnNext.TabIndex = 87;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(20, 448);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(30, 25);
            this.btnMPrev.TabIndex = 84;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(56, 448);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(30, 25);
            this.btnPrev.TabIndex = 85;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // SearchQueryV1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 499);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.txtPage);
            this.Controls.Add(this.lblPage);
            this.Controls.Add(this.cmbShow);
            this.Controls.Add(this.btnMNext);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnMPrev);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.dgvSearch);
            this.Controls.Add(this.groupBox1);
            this.Name = "SearchQueryV1";
            this.Resizable = false;
            this.Text = "Search Query";
            this.Load += new System.EventHandler(this.SearchQueryV1_Load);
            this.Shown += new System.EventHandler(this.SearchQueryV1_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbFilter1;
        private System.Windows.Forms.TextBox txtFilter1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvSearch;
        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.Button btnMNext;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMask;
        private System.Windows.Forms.Button btnReset;
    }
}