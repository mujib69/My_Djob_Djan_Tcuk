namespace ISBS_New.Sales.BBK
{
    partial class UbahReceiptDate
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
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.dtReceiptDate = new System.Windows.Forms.DateTimePicker();
            this.dtGIDate = new System.Windows.Forms.DateTimePicker();
            this.txtGINo = new System.Windows.Forms.TextBox();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(24, 64);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(52, 19);
            this.metroLabel1.TabIndex = 0;
            this.metroLabel1.Text = "GI Date";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(23, 93);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(43, 19);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "GI No";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(24, 141);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(83, 19);
            this.metroLabel3.TabIndex = 2;
            this.metroLabel3.Text = "Receipt Date";
            // 
            // dtReceiptDate
            // 
            this.dtReceiptDate.CustomFormat = "dd/MM/yyyy";
            this.dtReceiptDate.Enabled = false;
            this.dtReceiptDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtReceiptDate.Location = new System.Drawing.Point(113, 141);
            this.dtReceiptDate.Name = "dtReceiptDate";
            this.dtReceiptDate.Size = new System.Drawing.Size(164, 20);
            this.dtReceiptDate.TabIndex = 82;
            // 
            // dtGIDate
            // 
            this.dtGIDate.CustomFormat = "dd/MM/yyyy";
            this.dtGIDate.Enabled = false;
            this.dtGIDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtGIDate.Location = new System.Drawing.Point(113, 64);
            this.dtGIDate.Name = "dtGIDate";
            this.dtGIDate.Size = new System.Drawing.Size(164, 20);
            this.dtGIDate.TabIndex = 83;
            // 
            // txtGINo
            // 
            this.txtGINo.Location = new System.Drawing.Point(113, 93);
            this.txtGINo.Name = "txtGINo";
            this.txtGINo.Size = new System.Drawing.Size(164, 20);
            this.txtGINo.TabIndex = 84;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(202, 181);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 86;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(121, 181);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 87;
            this.btnSave.Text = "Save";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // UbahReceiptDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 250);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtGINo);
            this.Controls.Add(this.dtGIDate);
            this.Controls.Add(this.dtReceiptDate);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Name = "UbahReceiptDate";
            this.Text = "Ubah Receipt Date";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        internal System.Windows.Forms.DateTimePicker dtReceiptDate;
        internal System.Windows.Forms.DateTimePicker dtGIDate;
        private System.Windows.Forms.TextBox txtGINo;
        internal MetroFramework.Controls.MetroButton btnCancel;
        internal MetroFramework.Controls.MetroButton btnSave;
    }
}