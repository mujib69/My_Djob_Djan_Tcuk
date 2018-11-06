namespace ISBS_New.Inventory.StockView
{
    partial class StockViewHeader
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
            this.tabHeader = new MetroFramework.Controls.MetroTabControl();
            this.tabOnHand = new MetroFramework.Controls.MetroTabPage();
            this.gvOnHand = new System.Windows.Forms.DataGridView();
            this.tabPurchase = new MetroFramework.Controls.MetroTabPage();
            this.gvPurchase = new System.Windows.Forms.DataGridView();
            this.tabSales = new MetroFramework.Controls.MetroTabPage();
            this.gvSales = new System.Windows.Forms.DataGridView();
            this.tabMovement = new System.Windows.Forms.TabPage();
            this.gvMovement = new System.Windows.Forms.DataGridView();
            this.tabOnHand2 = new MetroFramework.Controls.MetroTabPage();
            this.gvOnHand2 = new System.Windows.Forms.DataGridView();
            this.tabDetail = new MetroFramework.Controls.MetroTabControl();
            this.tabOnHandDetail = new MetroFramework.Controls.MetroTabPage();
            this.gvOnHandDetail = new System.Windows.Forms.DataGridView();
            this.tabPurchaseDetail = new MetroFramework.Controls.MetroTabPage();
            this.gvPurchaseDetail = new System.Windows.Forms.DataGridView();
            this.tabSalesDetail = new MetroFramework.Controls.MetroTabPage();
            this.gvSalesDetail = new System.Windows.Forms.DataGridView();
            this.tabMovementDetail = new MetroFramework.Controls.MetroTabPage();
            this.gvMovementDetail = new System.Windows.Forms.DataGridView();
            this.tabOnHand2Detail = new System.Windows.Forms.TabPage();
            this.gvOnHand2Detail = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.tabHeader.SuspendLayout();
            this.tabOnHand.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvOnHand)).BeginInit();
            this.tabPurchase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvPurchase)).BeginInit();
            this.tabSales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvSales)).BeginInit();
            this.tabMovement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvMovement)).BeginInit();
            this.tabOnHand2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvOnHand2)).BeginInit();
            this.tabDetail.SuspendLayout();
            this.tabOnHandDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvOnHandDetail)).BeginInit();
            this.tabPurchaseDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvPurchaseDetail)).BeginInit();
            this.tabSalesDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvSalesDetail)).BeginInit();
            this.tabMovementDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvMovementDetail)).BeginInit();
            this.tabOnHand2Detail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvOnHand2Detail)).BeginInit();
            this.SuspendLayout();
            // 
            // tabHeader
            // 
            this.tabHeader.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabHeader.Controls.Add(this.tabOnHand);
            this.tabHeader.Controls.Add(this.tabPurchase);
            this.tabHeader.Controls.Add(this.tabSales);
            this.tabHeader.Controls.Add(this.tabMovement);
            this.tabHeader.Controls.Add(this.tabOnHand2);
            this.tabHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabHeader.Location = new System.Drawing.Point(20, 60);
            this.tabHeader.Name = "tabHeader";
            this.tabHeader.SelectedIndex = 0;
            this.tabHeader.Size = new System.Drawing.Size(953, 618);
            this.tabHeader.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabHeader.TabIndex = 0;
            this.tabHeader.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tabHeader.UseSelectable = true;
            this.tabHeader.SelectedIndexChanged += new System.EventHandler(this.tabHeader_SelectedIndexChanged);
            // 
            // tabOnHand
            // 
            this.tabOnHand.BackColor = System.Drawing.Color.White;
            this.tabOnHand.Controls.Add(this.gvOnHand);
            this.tabOnHand.HorizontalScrollbarBarColor = true;
            this.tabOnHand.HorizontalScrollbarHighlightOnWheel = false;
            this.tabOnHand.HorizontalScrollbarSize = 10;
            this.tabOnHand.Location = new System.Drawing.Point(4, 41);
            this.tabOnHand.Name = "tabOnHand";
            this.tabOnHand.Size = new System.Drawing.Size(945, 573);
            this.tabOnHand.TabIndex = 0;
            this.tabOnHand.Text = "OnHand";
            this.tabOnHand.VerticalScrollbarBarColor = true;
            this.tabOnHand.VerticalScrollbarHighlightOnWheel = false;
            this.tabOnHand.VerticalScrollbarSize = 10;
            // 
            // gvOnHand
            // 
            this.gvOnHand.AllowUserToAddRows = false;
            this.gvOnHand.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvOnHand.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvOnHand.Location = new System.Drawing.Point(3, 3);
            this.gvOnHand.Name = "gvOnHand";
            this.gvOnHand.ReadOnly = true;
            this.gvOnHand.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvOnHand.Size = new System.Drawing.Size(933, 256);
            this.gvOnHand.TabIndex = 2;
            this.gvOnHand.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvOnHand_CellFormatting);
            this.gvOnHand.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvOnHand_CellClick);
            // 
            // tabPurchase
            // 
            this.tabPurchase.BackColor = System.Drawing.Color.White;
            this.tabPurchase.Controls.Add(this.gvPurchase);
            this.tabPurchase.HorizontalScrollbarBarColor = true;
            this.tabPurchase.HorizontalScrollbarHighlightOnWheel = false;
            this.tabPurchase.HorizontalScrollbarSize = 10;
            this.tabPurchase.Location = new System.Drawing.Point(4, 41);
            this.tabPurchase.Name = "tabPurchase";
            this.tabPurchase.Size = new System.Drawing.Size(945, 573);
            this.tabPurchase.TabIndex = 1;
            this.tabPurchase.Text = "| Purchase";
            this.tabPurchase.VerticalScrollbarBarColor = true;
            this.tabPurchase.VerticalScrollbarHighlightOnWheel = false;
            this.tabPurchase.VerticalScrollbarSize = 10;
            // 
            // gvPurchase
            // 
            this.gvPurchase.AllowUserToAddRows = false;
            this.gvPurchase.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvPurchase.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvPurchase.Location = new System.Drawing.Point(0, 3);
            this.gvPurchase.Name = "gvPurchase";
            this.gvPurchase.ReadOnly = true;
            this.gvPurchase.Size = new System.Drawing.Size(939, 259);
            this.gvPurchase.TabIndex = 3;
            this.gvPurchase.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvPurchase_CellFormatting);
            this.gvPurchase.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvPurchase_CellClick);
            // 
            // tabSales
            // 
            this.tabSales.BackColor = System.Drawing.Color.White;
            this.tabSales.Controls.Add(this.gvSales);
            this.tabSales.HorizontalScrollbarBarColor = true;
            this.tabSales.HorizontalScrollbarHighlightOnWheel = false;
            this.tabSales.HorizontalScrollbarSize = 10;
            this.tabSales.Location = new System.Drawing.Point(4, 41);
            this.tabSales.Name = "tabSales";
            this.tabSales.Size = new System.Drawing.Size(945, 573);
            this.tabSales.TabIndex = 2;
            this.tabSales.Text = "| Sales";
            this.tabSales.VerticalScrollbarBarColor = true;
            this.tabSales.VerticalScrollbarHighlightOnWheel = false;
            this.tabSales.VerticalScrollbarSize = 10;
            // 
            // gvSales
            // 
            this.gvSales.AllowUserToAddRows = false;
            this.gvSales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvSales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSales.Location = new System.Drawing.Point(3, 3);
            this.gvSales.Name = "gvSales";
            this.gvSales.ReadOnly = true;
            this.gvSales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvSales.Size = new System.Drawing.Size(933, 256);
            this.gvSales.TabIndex = 3;
            this.gvSales.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvSales_CellFormatting);
            // 
            // tabMovement
            // 
            this.tabMovement.BackColor = System.Drawing.Color.White;
            this.tabMovement.Controls.Add(this.gvMovement);
            this.tabMovement.Location = new System.Drawing.Point(4, 41);
            this.tabMovement.Name = "tabMovement";
            this.tabMovement.Size = new System.Drawing.Size(945, 573);
            this.tabMovement.TabIndex = 3;
            this.tabMovement.Text = "| Movement";
            // 
            // gvMovement
            // 
            this.gvMovement.AllowUserToAddRows = false;
            this.gvMovement.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvMovement.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvMovement.Location = new System.Drawing.Point(3, 3);
            this.gvMovement.Name = "gvMovement";
            this.gvMovement.ReadOnly = true;
            this.gvMovement.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvMovement.Size = new System.Drawing.Size(933, 256);
            this.gvMovement.TabIndex = 3;
            this.gvMovement.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvMovement_CellFormatting);
            // 
            // tabOnHand2
            // 
            this.tabOnHand2.Controls.Add(this.gvOnHand2);
            this.tabOnHand2.HorizontalScrollbarBarColor = true;
            this.tabOnHand2.HorizontalScrollbarHighlightOnWheel = false;
            this.tabOnHand2.HorizontalScrollbarSize = 10;
            this.tabOnHand2.Location = new System.Drawing.Point(4, 41);
            this.tabOnHand2.Name = "tabOnHand2";
            this.tabOnHand2.Size = new System.Drawing.Size(945, 573);
            this.tabOnHand2.TabIndex = 4;
            this.tabOnHand2.Text = "| OnHand2";
            this.tabOnHand2.VerticalScrollbarBarColor = true;
            this.tabOnHand2.VerticalScrollbarHighlightOnWheel = false;
            this.tabOnHand2.VerticalScrollbarSize = 10;
            // 
            // gvOnHand2
            // 
            this.gvOnHand2.AllowUserToAddRows = false;
            this.gvOnHand2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvOnHand2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvOnHand2.Location = new System.Drawing.Point(3, 3);
            this.gvOnHand2.Name = "gvOnHand2";
            this.gvOnHand2.ReadOnly = true;
            this.gvOnHand2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvOnHand2.Size = new System.Drawing.Size(933, 256);
            this.gvOnHand2.TabIndex = 3;
            this.gvOnHand2.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvOnHand2_CellClick);
            // 
            // tabDetail
            // 
            this.tabDetail.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabDetail.Controls.Add(this.tabOnHandDetail);
            this.tabDetail.Controls.Add(this.tabPurchaseDetail);
            this.tabDetail.Controls.Add(this.tabSalesDetail);
            this.tabDetail.Controls.Add(this.tabMovementDetail);
            this.tabDetail.Controls.Add(this.tabOnHand2Detail);
            this.tabDetail.Location = new System.Drawing.Point(23, 373);
            this.tabDetail.Name = "tabDetail";
            this.tabDetail.SelectedIndex = 4;
            this.tabDetail.Size = new System.Drawing.Size(947, 302);
            this.tabDetail.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabDetail.TabIndex = 1;
            this.tabDetail.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tabDetail.UseSelectable = true;
            // 
            // tabOnHandDetail
            // 
            this.tabOnHandDetail.BackColor = System.Drawing.Color.White;
            this.tabOnHandDetail.Controls.Add(this.gvOnHandDetail);
            this.tabOnHandDetail.HorizontalScrollbarBarColor = true;
            this.tabOnHandDetail.HorizontalScrollbarHighlightOnWheel = false;
            this.tabOnHandDetail.HorizontalScrollbarSize = 10;
            this.tabOnHandDetail.Location = new System.Drawing.Point(4, 41);
            this.tabOnHandDetail.Name = "tabOnHandDetail";
            this.tabOnHandDetail.Size = new System.Drawing.Size(939, 257);
            this.tabOnHandDetail.TabIndex = 0;
            this.tabOnHandDetail.Text = "OnHand Detail";
            this.tabOnHandDetail.VerticalScrollbarBarColor = true;
            this.tabOnHandDetail.VerticalScrollbarHighlightOnWheel = false;
            this.tabOnHandDetail.VerticalScrollbarSize = 10;
            // 
            // gvOnHandDetail
            // 
            this.gvOnHandDetail.AllowUserToAddRows = false;
            this.gvOnHandDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvOnHandDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvOnHandDetail.Location = new System.Drawing.Point(3, 7);
            this.gvOnHandDetail.Name = "gvOnHandDetail";
            this.gvOnHandDetail.ReadOnly = true;
            this.gvOnHandDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvOnHandDetail.Size = new System.Drawing.Size(933, 247);
            this.gvOnHandDetail.TabIndex = 2;
            this.gvOnHandDetail.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvOnHandDetail_CellFormatting);
            // 
            // tabPurchaseDetail
            // 
            this.tabPurchaseDetail.BackColor = System.Drawing.Color.White;
            this.tabPurchaseDetail.Controls.Add(this.gvPurchaseDetail);
            this.tabPurchaseDetail.HorizontalScrollbarBarColor = true;
            this.tabPurchaseDetail.HorizontalScrollbarHighlightOnWheel = false;
            this.tabPurchaseDetail.HorizontalScrollbarSize = 10;
            this.tabPurchaseDetail.Location = new System.Drawing.Point(4, 41);
            this.tabPurchaseDetail.Name = "tabPurchaseDetail";
            this.tabPurchaseDetail.Size = new System.Drawing.Size(939, 257);
            this.tabPurchaseDetail.TabIndex = 1;
            this.tabPurchaseDetail.Text = "| Purchase Detail";
            this.tabPurchaseDetail.VerticalScrollbarBarColor = true;
            this.tabPurchaseDetail.VerticalScrollbarHighlightOnWheel = false;
            this.tabPurchaseDetail.VerticalScrollbarSize = 10;
            // 
            // gvPurchaseDetail
            // 
            this.gvPurchaseDetail.AllowUserToAddRows = false;
            this.gvPurchaseDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvPurchaseDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvPurchaseDetail.Location = new System.Drawing.Point(4, 4);
            this.gvPurchaseDetail.Name = "gvPurchaseDetail";
            this.gvPurchaseDetail.ReadOnly = true;
            this.gvPurchaseDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvPurchaseDetail.Size = new System.Drawing.Size(933, 247);
            this.gvPurchaseDetail.TabIndex = 3;
            this.gvPurchaseDetail.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvPurchaseDetail_CellFormatting);
            // 
            // tabSalesDetail
            // 
            this.tabSalesDetail.BackColor = System.Drawing.Color.White;
            this.tabSalesDetail.Controls.Add(this.gvSalesDetail);
            this.tabSalesDetail.HorizontalScrollbarBarColor = true;
            this.tabSalesDetail.HorizontalScrollbarHighlightOnWheel = false;
            this.tabSalesDetail.HorizontalScrollbarSize = 10;
            this.tabSalesDetail.Location = new System.Drawing.Point(4, 41);
            this.tabSalesDetail.Name = "tabSalesDetail";
            this.tabSalesDetail.Size = new System.Drawing.Size(939, 257);
            this.tabSalesDetail.TabIndex = 2;
            this.tabSalesDetail.Text = "| Sales Detail";
            this.tabSalesDetail.VerticalScrollbarBarColor = true;
            this.tabSalesDetail.VerticalScrollbarHighlightOnWheel = false;
            this.tabSalesDetail.VerticalScrollbarSize = 10;
            // 
            // gvSalesDetail
            // 
            this.gvSalesDetail.AllowUserToAddRows = false;
            this.gvSalesDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvSalesDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSalesDetail.Location = new System.Drawing.Point(3, 3);
            this.gvSalesDetail.Name = "gvSalesDetail";
            this.gvSalesDetail.ReadOnly = true;
            this.gvSalesDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvSalesDetail.Size = new System.Drawing.Size(933, 247);
            this.gvSalesDetail.TabIndex = 3;
            // 
            // tabMovementDetail
            // 
            this.tabMovementDetail.BackColor = System.Drawing.Color.White;
            this.tabMovementDetail.Controls.Add(this.gvMovementDetail);
            this.tabMovementDetail.HorizontalScrollbarBarColor = true;
            this.tabMovementDetail.HorizontalScrollbarHighlightOnWheel = false;
            this.tabMovementDetail.HorizontalScrollbarSize = 10;
            this.tabMovementDetail.Location = new System.Drawing.Point(4, 41);
            this.tabMovementDetail.Name = "tabMovementDetail";
            this.tabMovementDetail.Size = new System.Drawing.Size(939, 257);
            this.tabMovementDetail.TabIndex = 3;
            this.tabMovementDetail.Text = "| Movement Detail";
            this.tabMovementDetail.VerticalScrollbarBarColor = true;
            this.tabMovementDetail.VerticalScrollbarHighlightOnWheel = false;
            this.tabMovementDetail.VerticalScrollbarSize = 10;
            // 
            // gvMovementDetail
            // 
            this.gvMovementDetail.AllowUserToAddRows = false;
            this.gvMovementDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvMovementDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvMovementDetail.Location = new System.Drawing.Point(3, 3);
            this.gvMovementDetail.Name = "gvMovementDetail";
            this.gvMovementDetail.ReadOnly = true;
            this.gvMovementDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvMovementDetail.Size = new System.Drawing.Size(933, 247);
            this.gvMovementDetail.TabIndex = 3;
            // 
            // tabOnHand2Detail
            // 
            this.tabOnHand2Detail.Controls.Add(this.gvOnHand2Detail);
            this.tabOnHand2Detail.Location = new System.Drawing.Point(4, 41);
            this.tabOnHand2Detail.Name = "tabOnHand2Detail";
            this.tabOnHand2Detail.Size = new System.Drawing.Size(939, 257);
            this.tabOnHand2Detail.TabIndex = 4;
            this.tabOnHand2Detail.Text = "| OnHand2 Detail";
            // 
            // gvOnHand2Detail
            // 
            this.gvOnHand2Detail.AllowUserToAddRows = false;
            this.gvOnHand2Detail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvOnHand2Detail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvOnHand2Detail.Location = new System.Drawing.Point(3, 5);
            this.gvOnHand2Detail.Name = "gvOnHand2Detail";
            this.gvOnHand2Detail.ReadOnly = true;
            this.gvOnHand2Detail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvOnHand2Detail.Size = new System.Drawing.Size(933, 247);
            this.gvOnHand2Detail.TabIndex = 4;
            this.gvOnHand2Detail.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvOnHand2Detail_CellClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(765, 31);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // StockViewHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(993, 698);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.tabDetail);
            this.Controls.Add(this.tabHeader);
            this.Name = "StockViewHeader";
            this.Resizable = false;
            this.Text = "Stock View";
            this.Load += new System.EventHandler(this.StockViewHeader_Load);
            this.tabHeader.ResumeLayout(false);
            this.tabOnHand.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvOnHand)).EndInit();
            this.tabPurchase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvPurchase)).EndInit();
            this.tabSales.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvSales)).EndInit();
            this.tabMovement.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvMovement)).EndInit();
            this.tabOnHand2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvOnHand2)).EndInit();
            this.tabDetail.ResumeLayout(false);
            this.tabOnHandDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvOnHandDetail)).EndInit();
            this.tabPurchaseDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvPurchaseDetail)).EndInit();
            this.tabSalesDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvSalesDetail)).EndInit();
            this.tabMovementDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvMovementDetail)).EndInit();
            this.tabOnHand2Detail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvOnHand2Detail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTabControl tabHeader;
        private MetroFramework.Controls.MetroTabPage tabOnHand;
        private MetroFramework.Controls.MetroTabPage tabPurchase;
        private MetroFramework.Controls.MetroTabPage tabSales;
        private System.Windows.Forms.TabPage tabMovement;
        private MetroFramework.Controls.MetroTabControl tabDetail;
        private MetroFramework.Controls.MetroTabPage tabOnHandDetail;
        private MetroFramework.Controls.MetroTabPage tabPurchaseDetail;
        private MetroFramework.Controls.MetroTabPage tabSalesDetail;
        private MetroFramework.Controls.MetroTabPage tabMovementDetail;
        private System.Windows.Forms.DataGridView gvOnHand;
        private System.Windows.Forms.DataGridView gvOnHandDetail;
        private System.Windows.Forms.DataGridView gvPurchase;
        private System.Windows.Forms.DataGridView gvSales;
        private System.Windows.Forms.DataGridView gvMovement;
        private System.Windows.Forms.DataGridView gvPurchaseDetail;
        private System.Windows.Forms.DataGridView gvSalesDetail;
        private System.Windows.Forms.DataGridView gvMovementDetail;
        private MetroFramework.Controls.MetroTabPage tabOnHand2;
        private System.Windows.Forms.DataGridView gvOnHand2;
        private System.Windows.Forms.TabPage tabOnHand2Detail;
        private System.Windows.Forms.DataGridView gvOnHand2Detail;
        private System.Windows.Forms.Button btnRefresh;
    }
}