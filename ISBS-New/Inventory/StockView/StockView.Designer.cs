namespace ISBS_New.Inventory.StockView
{
    partial class StockView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.dgvInvent_OnHand = new System.Windows.Forms.DataGridView();
            this.metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.metroTabPage3 = new MetroFramework.Controls.MetroTabPage();
            this.metroTabPage4 = new MetroFramework.Controls.MetroTabPage();
            this.dgvInvent_Trans = new System.Windows.Forms.DataGridView();
            this.dataSet1 = new ISBS_New.DataSet1();
            this.view_Stock_OnHandBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.view_Stock_OnHandTableAdapter = new ISBS_New.DataSet1TableAdapters.View_Stock_OnHandTableAdapter();
            this.tableAdapterManager = new ISBS_New.DataSet1TableAdapters.TableAdapterManager();
            this.fullItemIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemDeskripsiDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.warehouseDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unitDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.warehouseStockDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.availableForSaleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reservationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.metroTabControl1.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvent_OnHand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvent_Trans)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_Stock_OnHandBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.metroTabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvInvent_Trans, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 60);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1011, 580);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.metroTabControl1.Controls.Add(this.metroTabPage1);
            this.metroTabControl1.Controls.Add(this.metroTabPage2);
            this.metroTabControl1.Controls.Add(this.metroTabPage3);
            this.metroTabControl1.Controls.Add(this.metroTabPage4);
            this.metroTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroTabControl1.Location = new System.Drawing.Point(3, 3);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(1005, 284);
            this.metroTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.metroTabControl1.TabIndex = 0;
            this.metroTabControl1.UseSelectable = true;
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.Controls.Add(this.dgvInvent_OnHand);
            this.metroTabPage1.HorizontalScrollbar = true;
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.HorizontalScrollbarSize = 10;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 41);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(997, 239);
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "OnHand";
            this.metroTabPage1.VerticalScrollbar = true;
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            this.metroTabPage1.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage1.VerticalScrollbarSize = 10;
            // 
            // dgvInvent_OnHand
            // 
            this.dgvInvent_OnHand.AllowUserToAddRows = false;
            this.dgvInvent_OnHand.AllowUserToDeleteRows = false;
            this.dgvInvent_OnHand.AutoGenerateColumns = false;
            this.dgvInvent_OnHand.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvent_OnHand.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fullItemIdDataGridViewTextBoxColumn,
            this.itemDeskripsiDataGridViewTextBoxColumn,
            this.warehouseDataGridViewTextBoxColumn,
            this.unitDataGridViewTextBoxColumn,
            this.warehouseStockDataGridViewTextBoxColumn,
            this.availableForSaleDataGridViewTextBoxColumn,
            this.reservationDataGridViewTextBoxColumn});
            this.dgvInvent_OnHand.DataSource = this.view_Stock_OnHandBindingSource;
            this.dgvInvent_OnHand.Location = new System.Drawing.Point(0, 28);
            this.dgvInvent_OnHand.Name = "dgvInvent_OnHand";
            this.dgvInvent_OnHand.ReadOnly = true;
            this.dgvInvent_OnHand.Size = new System.Drawing.Size(994, 208);
            this.dgvInvent_OnHand.TabIndex = 2;
            // 
            // metroTabPage2
            // 
            this.metroTabPage2.HorizontalScrollbarBarColor = true;
            this.metroTabPage2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.HorizontalScrollbarSize = 10;
            this.metroTabPage2.Location = new System.Drawing.Point(4, 41);
            this.metroTabPage2.Name = "metroTabPage2";
            this.metroTabPage2.Size = new System.Drawing.Size(997, 239);
            this.metroTabPage2.TabIndex = 1;
            this.metroTabPage2.Text = "| Purchase";
            this.metroTabPage2.VerticalScrollbarBarColor = true;
            this.metroTabPage2.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage2.VerticalScrollbarSize = 10;
            // 
            // metroTabPage3
            // 
            this.metroTabPage3.HorizontalScrollbarBarColor = true;
            this.metroTabPage3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.HorizontalScrollbarSize = 10;
            this.metroTabPage3.Location = new System.Drawing.Point(4, 41);
            this.metroTabPage3.Name = "metroTabPage3";
            this.metroTabPage3.Size = new System.Drawing.Size(997, 239);
            this.metroTabPage3.TabIndex = 2;
            this.metroTabPage3.Text = "| Sales";
            this.metroTabPage3.VerticalScrollbarBarColor = true;
            this.metroTabPage3.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage3.VerticalScrollbarSize = 10;
            // 
            // metroTabPage4
            // 
            this.metroTabPage4.HorizontalScrollbarBarColor = true;
            this.metroTabPage4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroTabPage4.HorizontalScrollbarSize = 10;
            this.metroTabPage4.Location = new System.Drawing.Point(4, 41);
            this.metroTabPage4.Name = "metroTabPage4";
            this.metroTabPage4.Size = new System.Drawing.Size(997, 239);
            this.metroTabPage4.TabIndex = 3;
            this.metroTabPage4.Text = "| Movement";
            this.metroTabPage4.VerticalScrollbarBarColor = true;
            this.metroTabPage4.VerticalScrollbarHighlightOnWheel = false;
            this.metroTabPage4.VerticalScrollbarSize = 10;
            // 
            // dgvInvent_Trans
            // 
            this.dgvInvent_Trans.AllowUserToAddRows = false;
            this.dgvInvent_Trans.AllowUserToDeleteRows = false;
            this.dgvInvent_Trans.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvent_Trans.Location = new System.Drawing.Point(3, 293);
            this.dgvInvent_Trans.Name = "dgvInvent_Trans";
            this.dgvInvent_Trans.ReadOnly = true;
            this.dgvInvent_Trans.Size = new System.Drawing.Size(1005, 284);
            this.dgvInvent_Trans.TabIndex = 1;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "DataSet1";
            this.dataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // view_Stock_OnHandBindingSource
            // 
            this.view_Stock_OnHandBindingSource.DataMember = "View_Stock_OnHand";
            this.view_Stock_OnHandBindingSource.DataSource = this.dataSet1;
            // 
            // view_Stock_OnHandTableAdapter
            // 
            this.view_Stock_OnHandTableAdapter.ClearBeforeFill = true;
            // 
            // tableAdapterManager
            // 
            this.tableAdapterManager.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager.Connection = null;
            this.tableAdapterManager.UpdateOrder = ISBS_New.DataSet1TableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // fullItemIdDataGridViewTextBoxColumn
            // 
            this.fullItemIdDataGridViewTextBoxColumn.DataPropertyName = "FullItemId";
            this.fullItemIdDataGridViewTextBoxColumn.HeaderText = "FullItemId";
            this.fullItemIdDataGridViewTextBoxColumn.Name = "fullItemIdDataGridViewTextBoxColumn";
            this.fullItemIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // itemDeskripsiDataGridViewTextBoxColumn
            // 
            this.itemDeskripsiDataGridViewTextBoxColumn.DataPropertyName = "ItemDeskripsi";
            this.itemDeskripsiDataGridViewTextBoxColumn.HeaderText = "ItemDeskripsi";
            this.itemDeskripsiDataGridViewTextBoxColumn.Name = "itemDeskripsiDataGridViewTextBoxColumn";
            this.itemDeskripsiDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // warehouseDataGridViewTextBoxColumn
            // 
            this.warehouseDataGridViewTextBoxColumn.DataPropertyName = "Warehouse";
            this.warehouseDataGridViewTextBoxColumn.HeaderText = "Warehouse";
            this.warehouseDataGridViewTextBoxColumn.Name = "warehouseDataGridViewTextBoxColumn";
            this.warehouseDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // unitDataGridViewTextBoxColumn
            // 
            this.unitDataGridViewTextBoxColumn.DataPropertyName = "Unit";
            this.unitDataGridViewTextBoxColumn.HeaderText = "Unit";
            this.unitDataGridViewTextBoxColumn.Name = "unitDataGridViewTextBoxColumn";
            this.unitDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // warehouseStockDataGridViewTextBoxColumn
            // 
            this.warehouseStockDataGridViewTextBoxColumn.DataPropertyName = "Warehouse Stock";
            this.warehouseStockDataGridViewTextBoxColumn.HeaderText = "Warehouse Stock";
            this.warehouseStockDataGridViewTextBoxColumn.Name = "warehouseStockDataGridViewTextBoxColumn";
            this.warehouseStockDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // availableForSaleDataGridViewTextBoxColumn
            // 
            this.availableForSaleDataGridViewTextBoxColumn.DataPropertyName = "Available For Sale";
            this.availableForSaleDataGridViewTextBoxColumn.HeaderText = "Available For Sale";
            this.availableForSaleDataGridViewTextBoxColumn.Name = "availableForSaleDataGridViewTextBoxColumn";
            this.availableForSaleDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // reservationDataGridViewTextBoxColumn
            // 
            this.reservationDataGridViewTextBoxColumn.DataPropertyName = "Reservation";
            this.reservationDataGridViewTextBoxColumn.HeaderText = "Reservation";
            this.reservationDataGridViewTextBoxColumn.Name = "reservationDataGridViewTextBoxColumn";
            this.reservationDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // StockView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1051, 660);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StockView";
            this.Resizable = false;
            this.Text = "Stock View";
            this.Load += new System.EventHandler(this.StockView_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.metroTabControl1.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvent_OnHand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvent_Trans)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.view_Stock_OnHandBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage metroTabPage2;
        private MetroFramework.Controls.MetroTabPage metroTabPage3;
        private MetroFramework.Controls.MetroTabPage metroTabPage4;
        private MetroFramework.Controls.MetroTabPage metroTabPage1;
        private System.Windows.Forms.DataGridView dgvInvent_OnHand;
        private System.Windows.Forms.DataGridView dgvInvent_Trans;
        private DataSet1 dataSet1;
        private System.Windows.Forms.BindingSource view_Stock_OnHandBindingSource;
        private ISBS_New.DataSet1TableAdapters.View_Stock_OnHandTableAdapter view_Stock_OnHandTableAdapter;
        private ISBS_New.DataSet1TableAdapters.TableAdapterManager tableAdapterManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn fullItemIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemDeskripsiDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn warehouseDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn warehouseStockDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn availableForSaleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn reservationDataGridViewTextBoxColumn;

    }
}