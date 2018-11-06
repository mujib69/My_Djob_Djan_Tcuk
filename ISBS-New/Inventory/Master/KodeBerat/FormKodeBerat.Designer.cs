namespace ISBS_New.Inventory.Master.KodeBerat
{
    partial class FormKodeBerat
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
            this.grpCompanyInfo = new System.Windows.Forms.GroupBox();
            this.txtDeskripsi = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtKodeBeratId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpCompanyInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCompanyInfo
            // 
            this.grpCompanyInfo.Controls.Add(this.txtDeskripsi);
            this.grpCompanyInfo.Controls.Add(this.label2);
            this.grpCompanyInfo.Controls.Add(this.txtKodeBeratId);
            this.grpCompanyInfo.Controls.Add(this.label1);
            this.grpCompanyInfo.Location = new System.Drawing.Point(12, 55);
            this.grpCompanyInfo.Name = "grpCompanyInfo";
            this.grpCompanyInfo.Size = new System.Drawing.Size(350, 97);
            this.grpCompanyInfo.TabIndex = 167;
            this.grpCompanyInfo.TabStop = false;
            // 
            // txtDeskripsi
            // 
            this.txtDeskripsi.Location = new System.Drawing.Point(133, 56);
            this.txtDeskripsi.MaxLength = 30;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.Size = new System.Drawing.Size(185, 20);
            this.txtDeskripsi.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Deskripsi";
            // 
            // txtKodeBeratId
            // 
            this.txtKodeBeratId.Location = new System.Drawing.Point(133, 20);
            this.txtKodeBeratId.MaxLength = 5;
            this.txtKodeBeratId.Name = "txtKodeBeratId";
            this.txtKodeBeratId.Size = new System.Drawing.Size(185, 20);
            this.txtKodeBeratId.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Weight Code";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(190, 161);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 166;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(271, 161);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 165;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(109, 161);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 164;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(28, 161);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 163;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FormKodeBerat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 199);
            this.Controls.Add(this.grpCompanyInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Name = "FormKodeBerat";
            this.Resizable = false;
            this.Text = "Weight Code";
            this.Load += new System.EventHandler(this.FormKodeBerat_Load);
            this.grpCompanyInfo.ResumeLayout(false);
            this.grpCompanyInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpCompanyInfo;
        private System.Windows.Forms.TextBox txtDeskripsi;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtKodeBeratId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
    }
}