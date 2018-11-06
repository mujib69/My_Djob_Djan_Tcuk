namespace ISBS_New.Purchase.PriceListBeli
{
    partial class PriceListBeliAktif
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
            this.dgvPriceListBeliAktif = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriceListBeliAktif)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPriceListBeliAktif
            // 
            this.dgvPriceListBeliAktif.AllowUserToAddRows = false;
            this.dgvPriceListBeliAktif.AllowUserToDeleteRows = false;
            this.dgvPriceListBeliAktif.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPriceListBeliAktif.Location = new System.Drawing.Point(26, 62);
            this.dgvPriceListBeliAktif.MultiSelect = false;
            this.dgvPriceListBeliAktif.Name = "dgvPriceListBeliAktif";
            this.dgvPriceListBeliAktif.ReadOnly = true;
            this.dgvPriceListBeliAktif.Size = new System.Drawing.Size(703, 297);
            this.dgvPriceListBeliAktif.TabIndex = 134;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(578, 367);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 135;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(654, 367);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 137;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // PriceListBeliAktif
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(742, 406);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.dgvPriceListBeliAktif);
            this.Controls.Add(this.btnSelect);
            this.Name = "PriceListBeliAktif";
            this.Text = "Price List Beli Aktif";
            this.Load += new System.EventHandler(this.PriceListBeliAktif_Load);
            this.Shown += new System.EventHandler(this.PriceListBeliAktif_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriceListBeliAktif)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        public System.Windows.Forms.DataGridView dgvPriceListBeliAktif;
        private System.Windows.Forms.Button btnExit;

    }
}
