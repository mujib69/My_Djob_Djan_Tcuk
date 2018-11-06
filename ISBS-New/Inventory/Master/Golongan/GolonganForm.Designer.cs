namespace ISBS_New.Master.Golongan
{
    partial class GolonganForm
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
            this.grpUser = new System.Windows.Forms.GroupBox();
            this.txtDeskripsi = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtGolonganId = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpUser
            // 
            this.grpUser.Controls.Add(this.txtDeskripsi);
            this.grpUser.Controls.Add(this.Label3);
            this.grpUser.Controls.Add(this.txtGolonganId);
            this.grpUser.Controls.Add(this.Label1);
            this.grpUser.Location = new System.Drawing.Point(13, 54);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(342, 76);
            this.grpUser.TabIndex = 76;
            this.grpUser.TabStop = false;
            // 
            // txtDeskripsi
            // 
            this.txtDeskripsi.Enabled = false;
            this.txtDeskripsi.Location = new System.Drawing.Point(130, 43);
            this.txtDeskripsi.MaxLength = 30;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.Size = new System.Drawing.Size(206, 20);
            this.txtDeskripsi.TabIndex = 4;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(18, 47);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(56, 13);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Deskripsi  ";
            // 
            // txtGolonganId
            // 
            this.txtGolonganId.Enabled = false;
            this.txtGolonganId.Location = new System.Drawing.Point(130, 16);
            this.txtGolonganId.MaxLength = 15;
            this.txtGolonganId.Name = "txtGolonganId";
            this.txtGolonganId.Size = new System.Drawing.Size(119, 20);
            this.txtGolonganId.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(18, 20);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(56, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Golongan ";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(187, 140);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 165;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(268, 140);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 164;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(106, 140);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 163;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(28, 140);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 162;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // GolonganForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 174);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpUser);
            this.Name = "GolonganForm";
            this.Resizable = false;
            this.Text = "Golongan ";
            this.Load += new System.EventHandler(this.GologanForm_Load);
            this.grpUser.ResumeLayout(false);
            this.grpUser.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpUser;
        internal System.Windows.Forms.TextBox txtDeskripsi;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox txtGolonganId;
        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
    }
}