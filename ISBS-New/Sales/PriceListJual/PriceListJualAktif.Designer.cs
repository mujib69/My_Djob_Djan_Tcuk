namespace ISBS_New.Sales.PriceListJual
{
    partial class PriceListJualAktif
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
            this.lblForm = new System.Windows.Forms.Label();
            this.dgvPriceListJualAktif = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriceListJualAktif)).BeginInit();
            this.SuspendLayout();
            // 
            // lblForm
            // 
            this.lblForm.AutoSize = true;
            this.lblForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForm.Location = new System.Drawing.Point(30, 9);
            this.lblForm.Name = "lblForm";
            this.lblForm.Size = new System.Drawing.Size(135, 13);
            this.lblForm.TabIndex = 136;
            this.lblForm.Text = "Price List Jual (Active)";
            // 
            // dgvPriceListJualAktif
            // 
            this.dgvPriceListJualAktif.AllowUserToAddRows = false;
            this.dgvPriceListJualAktif.AllowUserToDeleteRows = false;
            this.dgvPriceListJualAktif.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPriceListJualAktif.Location = new System.Drawing.Point(26, 27);
            this.dgvPriceListJualAktif.MultiSelect = false;
            this.dgvPriceListJualAktif.Name = "dgvPriceListJualAktif";
            this.dgvPriceListJualAktif.ReadOnly = true;
            this.dgvPriceListJualAktif.Size = new System.Drawing.Size(703, 297);
            this.dgvPriceListJualAktif.TabIndex = 134;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(578, 332);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 135;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(654, 332);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 137;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // PriceListJualAktif
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(742, 369);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lblForm);
            this.Controls.Add(this.dgvPriceListJualAktif);
            this.Controls.Add(this.btnSelect);
            this.Name = "PriceListJualAktif";
            this.Text = "Price List Beli Aktif";
            this.Load += new System.EventHandler(this.PriceListJualAktif_Load);
            this.Shown += new System.EventHandler(this.PriceListJualAktif_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriceListJualAktif)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblForm;
        private System.Windows.Forms.Button btnSelect;
        public System.Windows.Forms.DataGridView dgvPriceListJualAktif;
        private System.Windows.Forms.Button btnExit;

    }
}
