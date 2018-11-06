namespace ISBS_New.Inventory.Master.Bentuk
{
    partial class FrmM_Bentuk
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
            this.txtBentuk = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel38 = new MetroFramework.Controls.MetroLabel();
            this.btnDelete = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnEdit = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // txtBentuk
            // 
            this.txtBentuk.Lines = new string[0];
            this.txtBentuk.Location = new System.Drawing.Point(129, 77);
            this.txtBentuk.MaxLength = 30;
            this.txtBentuk.Name = "txtBentuk";
            this.txtBentuk.PasswordChar = '\0';
            this.txtBentuk.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtBentuk.SelectedText = "";
            this.txtBentuk.Size = new System.Drawing.Size(242, 23);
            this.txtBentuk.TabIndex = 56;
            this.txtBentuk.UseSelectable = true;
            // 
            // metroLabel38
            // 
            this.metroLabel38.AutoSize = true;
            this.metroLabel38.Location = new System.Drawing.Point(29, 79);
            this.metroLabel38.Name = "metroLabel38";
            this.metroLabel38.Size = new System.Drawing.Size(48, 19);
            this.metroLabel38.TabIndex = 55;
            this.metroLabel38.Text = "Bentuk";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(284, 162);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 54;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseSelectable = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(203, 162);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 53;
            this.btnSave.Text = "Sa&ve";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(122, 162);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 52;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(41, 162);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 51;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseSelectable = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // FrmM_Bentuk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 223);
            this.Controls.Add(this.txtBentuk);
            this.Controls.Add(this.metroLabel38);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEdit);
            this.Font = new System.Drawing.Font("Courier New", 10F);
            this.Name = "FrmM_Bentuk";
            this.Padding = new System.Windows.Forms.Padding(26, 74, 26, 25);
            this.Resizable = false;
            this.Text = "Bentuk";
            this.Load += new System.EventHandler(this.FrmM_Bentuk_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox txtBentuk;
        private MetroFramework.Controls.MetroLabel metroLabel38;
        private MetroFramework.Controls.MetroButton btnDelete;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnEdit;

    }
}