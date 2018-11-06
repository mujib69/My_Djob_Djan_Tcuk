namespace ISBS_New.EFaktur
{
    partial class EFaktur
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
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbTipe = new System.Windows.Forms.ComboBox();
            this.dtSampai = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtDari = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.grpCustomer.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.label2);
            this.grpCustomer.Controls.Add(this.cmbTipe);
            this.grpCustomer.Controls.Add(this.dtSampai);
            this.grpCustomer.Controls.Add(this.label1);
            this.grpCustomer.Controls.Add(this.dtDari);
            this.grpCustomer.Controls.Add(this.label13);
            this.grpCustomer.Location = new System.Drawing.Point(23, 53);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(499, 77);
            this.grpCustomer.TabIndex = 142;
            this.grpCustomer.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 66;
            this.label2.Text = "Tipe E-Faktur";
            // 
            // cmbTipe
            // 
            this.cmbTipe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipe.FormattingEnabled = true;
            this.cmbTipe.Location = new System.Drawing.Point(128, 40);
            this.cmbTipe.Name = "cmbTipe";
            this.cmbTipe.Size = new System.Drawing.Size(121, 21);
            this.cmbTipe.TabIndex = 65;
            // 
            // dtSampai
            // 
            this.dtSampai.CustomFormat = "dd/MM/yyyy";
            this.dtSampai.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtSampai.Location = new System.Drawing.Point(375, 13);
            this.dtSampai.Name = "dtSampai";
            this.dtSampai.Size = new System.Drawing.Size(121, 20);
            this.dtSampai.TabIndex = 64;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(311, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 63;
            this.label1.Text = "Sampai";
            // 
            // dtDari
            // 
            this.dtDari.CustomFormat = "dd/MM/yyyy";
            this.dtDari.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDari.Location = new System.Drawing.Point(128, 13);
            this.dtDari.Name = "dtDari";
            this.dtDari.Size = new System.Drawing.Size(121, 20);
            this.dtDari.TabIndex = 62;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 16);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(106, 13);
            this.label13.TabIndex = 61;
            this.label13.Text = "Tanggal Invoice Dari";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(447, 136);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 144;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(366, 136);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 145;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // EFaktur
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 175);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.grpCustomer);
            this.Name = "EFaktur";
            this.Text = "EFaktur";
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpCustomer;
        private System.Windows.Forms.DateTimePicker dtDari;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DateTimePicker dtSampai;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbTipe;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnExport;

    }
}
