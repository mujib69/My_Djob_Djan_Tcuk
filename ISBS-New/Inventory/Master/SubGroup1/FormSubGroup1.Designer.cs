namespace ISBS_New.Master.Invent.SubGroup1
{
    partial class FormSubGroup1
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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpCompanyInfo = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtGroupDeskripsi = new System.Windows.Forms.TextBox();
            this.btnSearchGroup = new System.Windows.Forms.Button();
            this.txtGroupId = new System.Windows.Forms.TextBox();
            this.txtDeskripsi = new System.Windows.Forms.TextBox();
            this.txtSubGroup1Id = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpCompanyInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(275, 175);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 43;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(113, 175);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 42;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(32, 175);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 40;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(194, 175);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 161;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpCompanyInfo
            // 
            this.grpCompanyInfo.Controls.Add(this.label4);
            this.grpCompanyInfo.Controls.Add(this.label3);
            this.grpCompanyInfo.Controls.Add(this.txtGroupDeskripsi);
            this.grpCompanyInfo.Controls.Add(this.btnSearchGroup);
            this.grpCompanyInfo.Controls.Add(this.txtGroupId);
            this.grpCompanyInfo.Controls.Add(this.txtDeskripsi);
            this.grpCompanyInfo.Controls.Add(this.txtSubGroup1Id);
            this.grpCompanyInfo.Controls.Add(this.label2);
            this.grpCompanyInfo.Controls.Add(this.label1);
            this.grpCompanyInfo.Location = new System.Drawing.Point(11, 56);
            this.grpCompanyInfo.Name = "grpCompanyInfo";
            this.grpCompanyInfo.Size = new System.Drawing.Size(364, 107);
            this.grpCompanyInfo.TabIndex = 162;
            this.grpCompanyInfo.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(153, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 167;
            this.label4.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(153, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 166;
            this.label3.Text = "-";
            // 
            // txtGroupDeskripsi
            // 
            this.txtGroupDeskripsi.Location = new System.Drawing.Point(168, 23);
            this.txtGroupDeskripsi.MaxLength = 30;
            this.txtGroupDeskripsi.Name = "txtGroupDeskripsi";
            this.txtGroupDeskripsi.Size = new System.Drawing.Size(123, 20);
            this.txtGroupDeskripsi.TabIndex = 102;
            // 
            // btnSearchGroup
            // 
            this.btnSearchGroup.Location = new System.Drawing.Point(294, 22);
            this.btnSearchGroup.Name = "btnSearchGroup";
            this.btnSearchGroup.Size = new System.Drawing.Size(34, 23);
            this.btnSearchGroup.TabIndex = 1;
            this.btnSearchGroup.Text = "...";
            this.btnSearchGroup.UseVisualStyleBackColor = true;
            this.btnSearchGroup.Click += new System.EventHandler(this.btnSearchGroup_Click);
            // 
            // txtGroupId
            // 
            this.txtGroupId.Location = new System.Drawing.Point(87, 23);
            this.txtGroupId.MaxLength = 5;
            this.txtGroupId.Name = "txtGroupId";
            this.txtGroupId.ReadOnly = true;
            this.txtGroupId.Size = new System.Drawing.Size(60, 20);
            this.txtGroupId.TabIndex = 101;
            // 
            // txtDeskripsi
            // 
            this.txtDeskripsi.Location = new System.Drawing.Point(168, 49);
            this.txtDeskripsi.MaxLength = 30;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.Size = new System.Drawing.Size(123, 20);
            this.txtDeskripsi.TabIndex = 4;
            // 
            // txtSubGroup1Id
            // 
            this.txtSubGroup1Id.Location = new System.Drawing.Point(87, 49);
            this.txtSubGroup1Id.MaxLength = 5;
            this.txtSubGroup1Id.Name = "txtSubGroup1Id";
            this.txtSubGroup1Id.Size = new System.Drawing.Size(60, 20);
            this.txtSubGroup1Id.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sub Group ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Group Id";
            // 
            // FormSubGroup1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 223);
            this.Controls.Add(this.grpCompanyInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.MaximizeBox = false;
            this.Name = "FormSubGroup1";
            this.Resizable = false;
            this.Text = "Sub Group Item";
            this.Load += new System.EventHandler(this.FormSubGroup1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSubGroup1_FormClosed_1);
            this.grpCompanyInfo.ResumeLayout(false);
            this.grpCompanyInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpCompanyInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSubGroup1Id;
        private System.Windows.Forms.TextBox txtDeskripsi;
        private System.Windows.Forms.TextBox txtGroupId;
        private System.Windows.Forms.Button btnSearchGroup;
        private System.Windows.Forms.TextBox txtGroupDeskripsi;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}