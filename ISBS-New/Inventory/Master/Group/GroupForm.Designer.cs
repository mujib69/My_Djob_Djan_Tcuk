namespace ISBS_New.Master.Group
{
    partial class GroupForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpUser = new System.Windows.Forms.GroupBox();
            this.txtDeskripsi = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtGroupId = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInventTypeId = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grpUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(208, 171);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 170;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(289, 171);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 169;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(127, 171);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 168;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(49, 171);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 167;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpUser
            // 
            this.grpUser.Controls.Add(this.btnSearch);
            this.grpUser.Controls.Add(this.txtInventTypeId);
            this.grpUser.Controls.Add(this.label2);
            this.grpUser.Controls.Add(this.txtDeskripsi);
            this.grpUser.Controls.Add(this.Label3);
            this.grpUser.Controls.Add(this.txtGroupId);
            this.grpUser.Controls.Add(this.Label1);
            this.grpUser.Location = new System.Drawing.Point(11, 55);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(368, 110);
            this.grpUser.TabIndex = 166;
            this.grpUser.TabStop = false;
            // 
            // txtDeskripsi
            // 
            this.txtDeskripsi.Enabled = false;
            this.txtDeskripsi.Location = new System.Drawing.Point(130, 45);
            this.txtDeskripsi.MaxLength = 30;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.Size = new System.Drawing.Size(223, 20);
            this.txtDeskripsi.TabIndex = 4;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(18, 49);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(67, 13);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Group Name";
            // 
            // txtGroupId
            // 
            this.txtGroupId.Enabled = false;
            this.txtGroupId.Location = new System.Drawing.Point(130, 16);
            this.txtGroupId.MaxLength = 5;
            this.txtGroupId.Name = "txtGroupId";
            this.txtGroupId.Size = new System.Drawing.Size(88, 20);
            this.txtGroupId.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(18, 20);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(48, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Group Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Invent Type Id";
            this.label2.Visible = false;
            // 
            // txtInventTypeId
            // 
            this.txtInventTypeId.Enabled = false;
            this.txtInventTypeId.Location = new System.Drawing.Point(130, 74);
            this.txtInventTypeId.MaxLength = 5;
            this.txtInventTypeId.Name = "txtInventTypeId";
            this.txtInventTypeId.Size = new System.Drawing.Size(61, 20);
            this.txtInventTypeId.TabIndex = 13;
            this.txtInventTypeId.Visible = false;
            this.txtInventTypeId.Leave += new System.EventHandler(this.txtInventTypeId_Leave);
            this.txtInventTypeId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInventTypeId_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(197, 73);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(36, 23);
            this.btnSearch.TabIndex = 14;
            this.btnSearch.Text = "...";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Visible = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // GroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 211);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpUser);
            this.MaximizeBox = false;
            this.Name = "GroupForm";
            this.Resizable = false;
            this.Text = "Group Item";
            this.Load += new System.EventHandler(this.GroupForm_Load);
            this.grpUser.ResumeLayout(false);
            this.grpUser.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpUser;
        internal System.Windows.Forms.TextBox txtDeskripsi;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox txtGroupId;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.TextBox txtInventTypeId;
        internal System.Windows.Forms.Label label2;
    }
}