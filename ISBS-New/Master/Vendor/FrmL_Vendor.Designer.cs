namespace ISBS_New.Master.Vendor
{
    partial class FrmL_Vendor
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
            this.DtGridView_Vendor = new System.Windows.Forms.DataGridView();
            this.BtnNew = new MetroFramework.Controls.MetroButton();
            this.BtnEdit = new MetroFramework.Controls.MetroButton();
            this.BtnExit = new MetroFramework.Controls.MetroButton();
            this.BtnDelete = new MetroFramework.Controls.MetroButton();
            this.BtnReset = new MetroFramework.Controls.MetroButton();
            this.BtnSearch = new MetroFramework.Controls.MetroButton();
            this.CboCriteria = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DtGridView_Vendor)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DtGridView_Vendor);
            this.groupBox1.Controls.Add(this.BtnNew);
            this.groupBox1.Controls.Add(this.BtnEdit);
            this.groupBox1.Controls.Add(this.BtnExit);
            this.groupBox1.Controls.Add(this.BtnDelete);
            this.groupBox1.Controls.Add(this.BtnReset);
            this.groupBox1.Controls.Add(this.BtnSearch);
            this.groupBox1.Controls.Add(this.CboCriteria);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.Label1);
            this.groupBox1.Controls.Add(this.Label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(27, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(817, 478);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // DtGridView_Vendor
            // 
            this.DtGridView_Vendor.AllowUserToAddRows = false;
            this.DtGridView_Vendor.AllowUserToDeleteRows = false;
            this.DtGridView_Vendor.AllowUserToOrderColumns = true;
            this.DtGridView_Vendor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DtGridView_Vendor.Location = new System.Drawing.Point(10, 97);
            this.DtGridView_Vendor.Margin = new System.Windows.Forms.Padding(4);
            this.DtGridView_Vendor.Name = "DtGridView_Vendor";
            this.DtGridView_Vendor.ReadOnly = true;
            this.DtGridView_Vendor.RowHeadersWidth = 60;
            this.DtGridView_Vendor.ShowCellToolTips = false;
            this.DtGridView_Vendor.Size = new System.Drawing.Size(800, 333);
            this.DtGridView_Vendor.TabIndex = 133;
            this.DtGridView_Vendor.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DtGridView_Vendor_CellDoubleClick);
            // 
            // BtnNew
            // 
            this.BtnNew.Location = new System.Drawing.Point(459, 449);
            this.BtnNew.Name = "BtnNew";
            this.BtnNew.Size = new System.Drawing.Size(75, 23);
            this.BtnNew.TabIndex = 132;
            this.BtnNew.Text = "&New";
            this.BtnNew.UseSelectable = true;
            this.BtnNew.Click += new System.EventHandler(this.BtnNew_Click);
            // 
            // BtnEdit
            // 
            this.BtnEdit.Location = new System.Drawing.Point(540, 449);
            this.BtnEdit.Name = "BtnEdit";
            this.BtnEdit.Size = new System.Drawing.Size(75, 23);
            this.BtnEdit.TabIndex = 131;
            this.BtnEdit.Text = "&Edit";
            this.BtnEdit.UseSelectable = true;
            this.BtnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // BtnExit
            // 
            this.BtnExit.Location = new System.Drawing.Point(736, 449);
            this.BtnExit.Name = "BtnExit";
            this.BtnExit.Size = new System.Drawing.Size(75, 23);
            this.BtnExit.TabIndex = 130;
            this.BtnExit.Text = "&Exit";
            this.BtnExit.UseSelectable = true;
            this.BtnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(621, 449);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(75, 23);
            this.BtnDelete.TabIndex = 129;
            this.BtnDelete.Text = "&Delete";
            this.BtnDelete.UseSelectable = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // BtnReset
            // 
            this.BtnReset.Location = new System.Drawing.Point(554, 53);
            this.BtnReset.Name = "BtnReset";
            this.BtnReset.Size = new System.Drawing.Size(75, 23);
            this.BtnReset.TabIndex = 127;
            this.BtnReset.Text = "Reset";
            this.BtnReset.UseSelectable = true;
            this.BtnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // BtnSearch
            // 
            this.BtnSearch.Location = new System.Drawing.Point(554, 22);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(75, 23);
            this.BtnSearch.TabIndex = 126;
            this.BtnSearch.Text = "Search";
            this.BtnSearch.UseSelectable = true;
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // CboCriteria
            // 
            this.CboCriteria.FormattingEnabled = true;
            this.CboCriteria.Location = new System.Drawing.Point(103, 52);
            this.CboCriteria.Margin = new System.Windows.Forms.Padding(4);
            this.CboCriteria.Name = "CboCriteria";
            this.CboCriteria.Size = new System.Drawing.Size(443, 24);
            this.CboCriteria.TabIndex = 125;
            // 
            // txtSearch
            // 
            this.txtSearch.AcceptsReturn = true;
            this.txtSearch.Location = new System.Drawing.Point(103, 22);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(443, 22);
            this.txtSearch.TabIndex = 124;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(7, 56);
            this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(72, 16);
            this.Label1.TabIndex = 123;
            this.Label1.Text = "Criteria";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(7, 25);
            this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(56, 16);
            this.Label2.TabIndex = 122;
            this.Label2.Text = "Search";
            // 
            // FrmL_Vendor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 577);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmL_Vendor";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Resizable = false;
            this.Text = "List Vendor";
            this.Load += new System.EventHandler(this.FrmL_Vendor_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmL_Vendor_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DtGridView_Vendor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.DataGridView DtGridView_Vendor;
        private MetroFramework.Controls.MetroButton BtnNew;
        private MetroFramework.Controls.MetroButton BtnEdit;
        private MetroFramework.Controls.MetroButton BtnExit;
        private MetroFramework.Controls.MetroButton BtnDelete;
        private MetroFramework.Controls.MetroButton BtnReset;
        private MetroFramework.Controls.MetroButton BtnSearch;
        internal System.Windows.Forms.ComboBox CboCriteria;
        internal System.Windows.Forms.TextBox txtSearch;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label Label2;
    }
}