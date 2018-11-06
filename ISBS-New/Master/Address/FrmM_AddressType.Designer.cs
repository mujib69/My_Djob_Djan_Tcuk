namespace ISBS_New.Master.Address
{
    partial class FrmM_AddressType
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
            this.txtDeskripsi = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.txtType = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel38 = new MetroFramework.Controls.MetroLabel();
            this.btnDelete = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnEdit = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // txtDeskripsi
            // 
            this.txtDeskripsi.Lines = new string[0];
            this.txtDeskripsi.Location = new System.Drawing.Point(200, 118);
            this.txtDeskripsi.MaxLength = 50;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.PasswordChar = '\0';
            this.txtDeskripsi.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtDeskripsi.SelectedText = "";
            this.txtDeskripsi.Size = new System.Drawing.Size(238, 23);
            this.txtDeskripsi.TabIndex = 55;
            this.txtDeskripsi.UseSelectable = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(27, 120);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(60, 19);
            this.metroLabel1.TabIndex = 54;
            this.metroLabel1.Text = "Deskripsi";
            // 
            // txtType
            // 
            this.txtType.Lines = new string[0];
            this.txtType.Location = new System.Drawing.Point(200, 89);
            this.txtType.MaxLength = 10;
            this.txtType.Name = "txtType";
            this.txtType.PasswordChar = '\0';
            this.txtType.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtType.SelectedText = "";
            this.txtType.Size = new System.Drawing.Size(238, 23);
            this.txtType.TabIndex = 53;
            this.txtType.UseSelectable = true;
            // 
            // metroLabel38
            // 
            this.metroLabel38.AutoSize = true;
            this.metroLabel38.Location = new System.Drawing.Point(26, 91);
            this.metroLabel38.Name = "metroLabel38";
            this.metroLabel38.Size = new System.Drawing.Size(89, 19);
            this.metroLabel38.TabIndex = 52;
            this.metroLabel38.Text = "Purpose Type";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(270, 219);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 51;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseSelectable = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(189, 219);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 50;
            this.btnSave.Text = "Sa&ve";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(108, 219);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 49;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(27, 219);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 48;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseSelectable = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // FrmM_AddressType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 273);
            this.Controls.Add(this.txtDeskripsi);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.txtType);
            this.Controls.Add(this.metroLabel38);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEdit);
            this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmM_AddressType";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Resizable = false;
            this.Text = "Master Address Type";
            this.Load += new System.EventHandler(this.FrmM_AddressType_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox txtDeskripsi;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTextBox txtType;
        private MetroFramework.Controls.MetroLabel metroLabel38;
        private MetroFramework.Controls.MetroButton btnDelete;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnEdit;
    }
}