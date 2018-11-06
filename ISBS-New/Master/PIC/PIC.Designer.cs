namespace ISBS_New.Master.PIC
{
    partial class PIC
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtPICId = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtPICName = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.cmbUserId = new System.Windows.Forms.ComboBox();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.grpUser = new System.Windows.Forms.GroupBox();
            this.grpUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(246, 195);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 78;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(153, 195);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 75;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(327, 195);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 77;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(234, 195);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 76;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(18, 25);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(36, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "PIC Id";
            // 
            // txtPICId
            // 
            this.txtPICId.Enabled = false;
            this.txtPICId.Location = new System.Drawing.Point(118, 22);
            this.txtPICId.Name = "txtPICId";
            this.txtPICId.Size = new System.Drawing.Size(68, 20);
            this.txtPICId.TabIndex = 1;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(18, 61);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(55, 13);
            this.Label2.TabIndex = 77;
            this.Label2.Text = "PIC Name";
            // 
            // txtPICName
            // 
            this.txtPICName.Enabled = false;
            this.txtPICName.Location = new System.Drawing.Point(118, 58);
            this.txtPICName.MaxLength = 50;
            this.txtPICName.Name = "txtPICName";
            this.txtPICName.Size = new System.Drawing.Size(234, 20);
            this.txtPICName.TabIndex = 5;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(18, 94);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(57, 13);
            this.Label3.TabIndex = 66;
            this.Label3.Text = "UserId";
            // 
            // cmbUserId
            // 
            this.cmbUserId.Enabled = false;
            this.cmbUserId.FormattingEnabled = true;
            this.cmbUserId.Location = new System.Drawing.Point(118, 94);
            this.cmbUserId.Name = "cmbUserId";
            this.cmbUserId.Size = new System.Drawing.Size(234, 21);
            this.cmbUserId.TabIndex = 8;
            this.cmbUserId.SelectedIndexChanged += new System.EventHandler(this.cmbUserId_SelectedIndexChanged);
            // 
            // txtUserId
            // 
            this.txtUserId.Enabled = false;
            this.txtUserId.Location = new System.Drawing.Point(118, 94);
            this.txtUserId.MaxLength = 50;
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(107, 20);
            this.txtUserId.TabIndex = 4;
            this.txtUserId.Visible = false;
            
            // 
            // grpUser
            // 
            this.grpUser.Controls.Add(this.txtUserId);
            this.grpUser.Controls.Add(this.cmbUserId);
            this.grpUser.Controls.Add(this.Label3);
            this.grpUser.Controls.Add(this.txtPICName);
            this.grpUser.Controls.Add(this.Label2);
            this.grpUser.Controls.Add(this.txtPICId);
            this.grpUser.Controls.Add(this.Label1);
            this.grpUser.Location = new System.Drawing.Point(12, 12);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(389, 177);
            this.grpUser.TabIndex = 50;
            this.grpUser.TabStop = false;
            // 
            // PIC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 235);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.grpUser);
            this.Name = "PIC";
            this.Text = "PIC";
            this.Load += new System.EventHandler(this.PIC_Load);
            this.grpUser.ResumeLayout(false);
            this.grpUser.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox txtPICId;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox txtPICName;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.ComboBox cmbUserId;
        internal System.Windows.Forms.TextBox txtUserId;
        internal System.Windows.Forms.GroupBox grpUser;
    }
}