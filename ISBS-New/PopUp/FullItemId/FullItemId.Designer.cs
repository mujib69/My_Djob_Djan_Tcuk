namespace ISBS_New.PopUp.FullItemId
{
    partial class FullItemId
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
            this.btnExit = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabStock = new System.Windows.Forms.TabPage();
            this.dgvStockItem = new System.Windows.Forms.DataGridView();
            this.tabPagePrice = new System.Windows.Forms.TabPage();
            this.dgvPriceItem = new System.Windows.Forms.DataGridView();
            this.tabPageVendPreference = new System.Windows.Forms.TabPage();
            this.dgvVendPreference = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LblSubGroup2Desk = new System.Windows.Forms.Label();
            this.LblSubGroup1Desk = new System.Windows.Forms.Label();
            this.LblGroupDesk = new System.Windows.Forms.Label();
            this.LblUoMAlt = new System.Windows.Forms.Label();
            this.LblUoM = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.LblFullItemId = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LblItemID = new System.Windows.Forms.Label();
            this.lblSpek = new System.Windows.Forms.Label();
            this.lblQuality = new System.Windows.Forms.Label();
            this.lblGolongan = new System.Windows.Forms.Label();
            this.lblMerek = new System.Windows.Forms.Label();
            this.lblManufacturer = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockItem)).BeginInit();
            this.tabPagePrice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriceItem)).BeginInit();
            this.tabPageVendPreference.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendPreference)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(536, 488);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 42;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabStock);
            this.tabControl1.Controls.Add(this.tabPagePrice);
            this.tabControl1.Controls.Add(this.tabPageVendPreference);
            this.tabControl1.Location = new System.Drawing.Point(20, 260);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(595, 222);
            this.tabControl1.TabIndex = 50;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabStock
            // 
            this.tabStock.Controls.Add(this.dgvStockItem);
            this.tabStock.Location = new System.Drawing.Point(4, 22);
            this.tabStock.Name = "tabStock";
            this.tabStock.Padding = new System.Windows.Forms.Padding(3);
            this.tabStock.Size = new System.Drawing.Size(587, 196);
            this.tabStock.TabIndex = 0;
            this.tabStock.Text = "Stock";
            this.tabStock.UseVisualStyleBackColor = true;
            // 
            // dgvStockItem
            // 
            this.dgvStockItem.AllowUserToAddRows = false;
            this.dgvStockItem.AllowUserToDeleteRows = false;
            this.dgvStockItem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStockItem.Location = new System.Drawing.Point(7, 6);
            this.dgvStockItem.Name = "dgvStockItem";
            this.dgvStockItem.ReadOnly = true;
            this.dgvStockItem.Size = new System.Drawing.Size(574, 184);
            this.dgvStockItem.TabIndex = 0;
            this.dgvStockItem.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvStockItem_CellMouseDown);
            // 
            // tabPagePrice
            // 
            this.tabPagePrice.Controls.Add(this.dgvPriceItem);
            this.tabPagePrice.Location = new System.Drawing.Point(4, 22);
            this.tabPagePrice.Name = "tabPagePrice";
            this.tabPagePrice.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePrice.Size = new System.Drawing.Size(587, 196);
            this.tabPagePrice.TabIndex = 1;
            this.tabPagePrice.Text = "Price";
            this.tabPagePrice.UseVisualStyleBackColor = true;
            // 
            // dgvPriceItem
            // 
            this.dgvPriceItem.AllowUserToAddRows = false;
            this.dgvPriceItem.AllowUserToDeleteRows = false;
            this.dgvPriceItem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPriceItem.Location = new System.Drawing.Point(7, 6);
            this.dgvPriceItem.Name = "dgvPriceItem";
            this.dgvPriceItem.ReadOnly = true;
            this.dgvPriceItem.Size = new System.Drawing.Size(574, 184);
            this.dgvPriceItem.TabIndex = 0;
            // 
            // tabPageVendPreference
            // 
            this.tabPageVendPreference.Controls.Add(this.dgvVendPreference);
            this.tabPageVendPreference.Location = new System.Drawing.Point(4, 22);
            this.tabPageVendPreference.Name = "tabPageVendPreference";
            this.tabPageVendPreference.Size = new System.Drawing.Size(587, 196);
            this.tabPageVendPreference.TabIndex = 2;
            this.tabPageVendPreference.Text = "Vendor Preference";
            this.tabPageVendPreference.UseVisualStyleBackColor = true;
            // 
            // dgvVendPreference
            // 
            this.dgvVendPreference.AllowUserToAddRows = false;
            this.dgvVendPreference.AllowUserToDeleteRows = false;
            this.dgvVendPreference.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVendPreference.Location = new System.Drawing.Point(7, 12);
            this.dgvVendPreference.Name = "dgvVendPreference";
            this.dgvVendPreference.ReadOnly = true;
            this.dgvVendPreference.Size = new System.Drawing.Size(577, 167);
            this.dgvVendPreference.TabIndex = 0;
            this.dgvVendPreference.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvVendPreference_CellMouseDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LblSubGroup2Desk);
            this.groupBox1.Controls.Add(this.LblSubGroup1Desk);
            this.groupBox1.Controls.Add(this.LblGroupDesk);
            this.groupBox1.Controls.Add(this.LblUoMAlt);
            this.groupBox1.Controls.Add(this.LblUoM);
            this.groupBox1.Controls.Add(this.lblItemName);
            this.groupBox1.Controls.Add(this.LblFullItemId);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.LblItemID);
            this.groupBox1.Controls.Add(this.lblSpek);
            this.groupBox1.Controls.Add(this.lblQuality);
            this.groupBox1.Controls.Add(this.lblGolongan);
            this.groupBox1.Controls.Add(this.lblMerek);
            this.groupBox1.Controls.Add(this.lblManufacturer);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(24, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(591, 200);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            // 
            // LblSubGroup2Desk
            // 
            this.LblSubGroup2Desk.AutoSize = true;
            this.LblSubGroup2Desk.Location = new System.Drawing.Point(117, 162);
            this.LblSubGroup2Desk.Name = "LblSubGroup2Desk";
            this.LblSubGroup2Desk.Size = new System.Drawing.Size(100, 13);
            this.LblSubGroup2Desk.TabIndex = 82;
            this.LblSubGroup2Desk.Text = "LblSubGroup2Desk";
            // 
            // LblSubGroup1Desk
            // 
            this.LblSubGroup1Desk.AutoSize = true;
            this.LblSubGroup1Desk.Location = new System.Drawing.Point(118, 138);
            this.LblSubGroup1Desk.Name = "LblSubGroup1Desk";
            this.LblSubGroup1Desk.Size = new System.Drawing.Size(100, 13);
            this.LblSubGroup1Desk.TabIndex = 81;
            this.LblSubGroup1Desk.Text = "LblSubGroup1Desk";
            // 
            // LblGroupDesk
            // 
            this.LblGroupDesk.AutoSize = true;
            this.LblGroupDesk.Location = new System.Drawing.Point(118, 114);
            this.LblGroupDesk.Name = "LblGroupDesk";
            this.LblGroupDesk.Size = new System.Drawing.Size(75, 13);
            this.LblGroupDesk.TabIndex = 80;
            this.LblGroupDesk.Text = "LblGroupDesk";
            // 
            // LblUoMAlt
            // 
            this.LblUoMAlt.AutoSize = true;
            this.LblUoMAlt.Location = new System.Drawing.Point(117, 93);
            this.LblUoMAlt.Name = "LblUoMAlt";
            this.LblUoMAlt.Size = new System.Drawing.Size(56, 13);
            this.LblUoMAlt.TabIndex = 79;
            this.LblUoMAlt.Text = "LblUoMAlt";
            // 
            // LblUoM
            // 
            this.LblUoM.AutoSize = true;
            this.LblUoM.Location = new System.Drawing.Point(117, 70);
            this.LblUoM.Name = "LblUoM";
            this.LblUoM.Size = new System.Drawing.Size(44, 13);
            this.LblUoM.TabIndex = 78;
            this.LblUoM.Text = "LblUoM";
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Location = new System.Drawing.Point(117, 48);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(65, 13);
            this.lblItemName.TabIndex = 77;
            this.lblItemName.Text = "lblItemName";
            // 
            // LblFullItemId
            // 
            this.LblFullItemId.AutoSize = true;
            this.LblFullItemId.Location = new System.Drawing.Point(117, 26);
            this.LblFullItemId.Name = "LblFullItemId";
            this.LblFullItemId.Size = new System.Drawing.Size(66, 13);
            this.LblFullItemId.TabIndex = 76;
            this.LblFullItemId.Text = "LblFullItemId";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 48);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(101, 13);
            this.label13.TabIndex = 75;
            this.label13.Text = "Full Item Name       :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 162);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(104, 13);
            this.label12.TabIndex = 74;
            this.label12.Text = "SubGroup 2 Desk   :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 138);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(104, 13);
            this.label11.TabIndex = 73;
            this.label11.Text = "SubGroup 1 Desk   :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 13);
            this.label7.TabIndex = 72;
            this.label7.Text = "Group Desk            :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 71;
            this.label3.Text = "UoM Alt                  :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 70;
            this.label2.Text = "UoM                       :";
            // 
            // LblItemID
            // 
            this.LblItemID.AutoSize = true;
            this.LblItemID.Location = new System.Drawing.Point(12, 26);
            this.LblItemID.Name = "LblItemID";
            this.LblItemID.Size = new System.Drawing.Size(102, 13);
            this.LblItemID.TabIndex = 69;
            this.LblItemID.Text = "Full Item ID             :";
            // 
            // lblSpek
            // 
            this.lblSpek.AutoSize = true;
            this.lblSpek.Location = new System.Drawing.Point(425, 117);
            this.lblSpek.Name = "lblSpek";
            this.lblSpek.Size = new System.Drawing.Size(42, 13);
            this.lblSpek.TabIndex = 68;
            this.lblSpek.Text = "lblSpek";
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(425, 93);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(49, 13);
            this.lblQuality.TabIndex = 67;
            this.lblQuality.Text = "lblQuality";
            // 
            // lblGolongan
            // 
            this.lblGolongan.AutoSize = true;
            this.lblGolongan.Location = new System.Drawing.Point(426, 70);
            this.lblGolongan.Name = "lblGolongan";
            this.lblGolongan.Size = new System.Drawing.Size(63, 13);
            this.lblGolongan.TabIndex = 66;
            this.lblGolongan.Text = "lblGolongan";
            // 
            // lblMerek
            // 
            this.lblMerek.AutoSize = true;
            this.lblMerek.Location = new System.Drawing.Point(425, 48);
            this.lblMerek.Name = "lblMerek";
            this.lblMerek.Size = new System.Drawing.Size(47, 13);
            this.lblMerek.TabIndex = 65;
            this.lblMerek.Text = "lblMerek";
            // 
            // lblManufacturer
            // 
            this.lblManufacturer.AutoSize = true;
            this.lblManufacturer.Location = new System.Drawing.Point(425, 26);
            this.lblManufacturer.Name = "lblManufacturer";
            this.lblManufacturer.Size = new System.Drawing.Size(80, 13);
            this.lblManufacturer.TabIndex = 64;
            this.lblManufacturer.Text = "lblManufacturer";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(331, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 13);
            this.label9.TabIndex = 63;
            this.label9.Text = "Merek                :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(331, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 13);
            this.label8.TabIndex = 62;
            this.label8.Text = "Golongan           :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(331, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 61;
            this.label6.Text = "Spek                  :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(331, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 60;
            this.label5.Text = "Quality                :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(331, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 59;
            this.label4.Text = "Manufacturer     :";
            // 
            // FullItemId
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 543);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnExit);
            this.Name = "FullItemId";
            this.Text = "FullItemId";
            this.Load += new System.EventHandler(this.FullItemId_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabStock.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStockItem)).EndInit();
            this.tabPagePrice.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriceItem)).EndInit();
            this.tabPageVendPreference.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVendPreference)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabStock;
        private System.Windows.Forms.TabPage tabPagePrice;
        private System.Windows.Forms.TabPage tabPageVendPreference;
        private System.Windows.Forms.DataGridView dgvStockItem;
        private System.Windows.Forms.DataGridView dgvPriceItem;
        private System.Windows.Forms.DataGridView dgvVendPreference;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LblSubGroup2Desk;
        private System.Windows.Forms.Label LblSubGroup1Desk;
        private System.Windows.Forms.Label LblGroupDesk;
        private System.Windows.Forms.Label LblUoMAlt;
        private System.Windows.Forms.Label LblUoM;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label LblFullItemId;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LblItemID;
        private System.Windows.Forms.Label lblSpek;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.Label lblGolongan;
        private System.Windows.Forms.Label lblMerek;
        private System.Windows.Forms.Label lblManufacturer;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
    }
}