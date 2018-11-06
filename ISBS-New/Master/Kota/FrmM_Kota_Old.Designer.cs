namespace ISBS_New.Master.Kota
{
    partial class FrmM_Kota_Old
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmM_Kota_Old));
            this.txtkota = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel38 = new MetroFramework.Controls.MetroLabel();
            this.btnDelete = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnEdit = new MetroFramework.Controls.MetroButton();
            this.Btntxtprovinsi = new System.Windows.Forms.Button();
            this.txtprovinsi = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.txtdaerah = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // txtkota
            // 
            this.txtkota.Lines = new string[0];
            this.txtkota.Location = new System.Drawing.Point(197, 83);
            this.txtkota.MaxLength = 30;
            this.txtkota.Name = "txtkota";
            this.txtkota.PasswordChar = '\0';
            this.txtkota.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtkota.SelectedText = "";
            this.txtkota.Size = new System.Drawing.Size(238, 23);
            this.txtkota.TabIndex = 44;
            this.txtkota.UseSelectable = true;
            // 
            // metroLabel38
            // 
            this.metroLabel38.AutoSize = true;
            this.metroLabel38.Location = new System.Drawing.Point(23, 85);
            this.metroLabel38.Name = "metroLabel38";
            this.metroLabel38.Size = new System.Drawing.Size(35, 19);
            this.metroLabel38.TabIndex = 43;
            this.metroLabel38.Text = "Kota";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(267, 213);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 42;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseSelectable = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(186, 213);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 41;
            this.btnSave.Text = "Sa&ve";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(105, 213);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 40;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(24, 213);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 39;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseSelectable = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // Btntxtprovinsi
            // 
            this.Btntxtprovinsi.Image = ((System.Drawing.Image)(resources.GetObject("Btntxtprovinsi.Image")));
            this.Btntxtprovinsi.Location = new System.Drawing.Point(443, 112);
            this.Btntxtprovinsi.Name = "Btntxtprovinsi";
            this.Btntxtprovinsi.Size = new System.Drawing.Size(25, 23);
            this.Btntxtprovinsi.TabIndex = 48;
            this.Btntxtprovinsi.UseVisualStyleBackColor = true;
            this.Btntxtprovinsi.Click += new System.EventHandler(this.Btnprovinsi_Click);
            // 
            // txtprovinsi
            // 
            this.txtprovinsi.Lines = new string[0];
            this.txtprovinsi.Location = new System.Drawing.Point(197, 112);
            this.txtprovinsi.MaxLength = 20;
            this.txtprovinsi.Name = "txtprovinsi";
            this.txtprovinsi.PasswordChar = '\0';
            this.txtprovinsi.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtprovinsi.SelectedText = "";
            this.txtprovinsi.Size = new System.Drawing.Size(238, 23);
            this.txtprovinsi.TabIndex = 47;
            this.txtprovinsi.UseSelectable = true;
            this.txtprovinsi.Leave += new System.EventHandler(this.txtprovinsi_Leave);
            this.txtprovinsi.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtprovinsi_KeyPress);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(24, 114);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(54, 19);
            this.metroLabel1.TabIndex = 46;
            this.metroLabel1.Text = "Provinsi";
            // 
            // txtdaerah
            // 
            this.txtdaerah.Lines = new string[0];
            this.txtdaerah.Location = new System.Drawing.Point(197, 143);
            this.txtdaerah.MaxLength = 30;
            this.txtdaerah.Name = "txtdaerah";
            this.txtdaerah.PasswordChar = '\0';
            this.txtdaerah.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtdaerah.SelectedText = "";
            this.txtdaerah.Size = new System.Drawing.Size(238, 23);
            this.txtdaerah.TabIndex = 50;
            this.txtdaerah.UseSelectable = true;
            //this.txtdaerah.Leave += new System.EventHandler(this.txtdaerah_Leave);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(24, 145);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(161, 19);
            this.metroLabel2.TabIndex = 49;
            this.metroLabel2.Text = "Daerah (Dalam/Luar Kota)";
            // 
            // FrmM_Kota
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 268);
            this.Controls.Add(this.txtdaerah);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.Btntxtprovinsi);
            this.Controls.Add(this.txtprovinsi);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.txtkota);
            this.Controls.Add(this.metroLabel38);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEdit);
            this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmM_Kota";
            this.Resizable = false;
            this.Text = "Master Kota";
            this.Load += new System.EventHandler(this.FrmM_Kota_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox txtkota;
        private MetroFramework.Controls.MetroLabel metroLabel38;
        private MetroFramework.Controls.MetroButton btnDelete;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnEdit;
        internal System.Windows.Forms.Button Btntxtprovinsi;
        private MetroFramework.Controls.MetroTextBox txtprovinsi;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTextBox txtdaerah;
        private MetroFramework.Controls.MetroLabel metroLabel2;
    }
}