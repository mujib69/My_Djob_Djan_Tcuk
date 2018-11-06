namespace ISBS_New.AccountAssignment
{
    partial class FormFQA
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnInActive = new MetroFramework.Controls.MetroButton();
            this.btnEdit = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnExit = new MetroFramework.Controls.MetroButton();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSearchCOA = new MetroFramework.Controls.MetroButton();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.txtCOAID = new System.Windows.Forms.TextBox();
            this.chkInActive = new System.Windows.Forms.CheckBox();
            this.txtFQADesc = new System.Windows.Forms.TextBox();
            this.txtFQAID = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnInActive);
            this.groupBox1.Controls.Add(this.btnEdit);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Location = new System.Drawing.Point(23, 217);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 47);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnInActive
            // 
            this.btnInActive.Location = new System.Drawing.Point(10, 14);
            this.btnInActive.Name = "btnInActive";
            this.btnInActive.Size = new System.Drawing.Size(75, 23);
            this.btnInActive.TabIndex = 6;
            this.btnInActive.Text = "InActive";
            this.btnInActive.UseSelectable = true;
            this.btnInActive.Click += new System.EventHandler(this.btnInActive_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(152, 14);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseSelectable = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(233, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(314, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(395, 14);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseSelectable = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(16, 48);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(70, 19);
            this.metroLabel1.TabIndex = 3;
            this.metroLabel1.Text = "FQA 1 ID :";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(16, 83);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(89, 19);
            this.metroLabel2.TabIndex = 4;
            this.metroLabel2.Text = "FQA 1 Desc  :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSearchCOA);
            this.groupBox2.Controls.Add(this.metroLabel3);
            this.groupBox2.Controls.Add(this.txtCOAID);
            this.groupBox2.Controls.Add(this.chkInActive);
            this.groupBox2.Controls.Add(this.txtFQADesc);
            this.groupBox2.Controls.Add(this.txtFQAID);
            this.groupBox2.Controls.Add(this.metroLabel1);
            this.groupBox2.Controls.Add(this.metroLabel2);
            this.groupBox2.Location = new System.Drawing.Point(23, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 158);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btnSearchCOA
            // 
            this.btnSearchCOA.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchCOA.Location = new System.Drawing.Point(319, 17);
            this.btnSearchCOA.Name = "btnSearchCOA";
            this.btnSearchCOA.Size = new System.Drawing.Size(47, 23);
            this.btnSearchCOA.TabIndex = 7;
            this.btnSearchCOA.Text = "...";
            this.btnSearchCOA.UseSelectable = true;
            this.btnSearchCOA.Click += new System.EventHandler(this.btnSearchCOA_Click);
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(16, 20);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(61, 19);
            this.metroLabel3.TabIndex = 9;
            this.metroLabel3.Text = "COA ID :";
            // 
            // txtCOAID
            // 
            this.txtCOAID.Location = new System.Drawing.Point(111, 19);
            this.txtCOAID.Name = "txtCOAID";
            this.txtCOAID.ReadOnly = true;
            this.txtCOAID.Size = new System.Drawing.Size(202, 20);
            this.txtCOAID.TabIndex = 8;
            // 
            // chkInActive
            // 
            this.chkInActive.AutoSize = true;
            this.chkInActive.Enabled = false;
            this.chkInActive.Location = new System.Drawing.Point(395, 21);
            this.chkInActive.Name = "chkInActive";
            this.chkInActive.Size = new System.Drawing.Size(68, 17);
            this.chkInActive.TabIndex = 7;
            this.chkInActive.Text = "In Active";
            this.chkInActive.UseVisualStyleBackColor = true;
            // 
            // txtFQADesc
            // 
            this.txtFQADesc.Location = new System.Drawing.Point(111, 83);
            this.txtFQADesc.Multiline = true;
            this.txtFQADesc.Name = "txtFQADesc";
            this.txtFQADesc.Size = new System.Drawing.Size(255, 61);
            this.txtFQADesc.TabIndex = 6;
            // 
            // txtFQAID
            // 
            this.txtFQAID.Location = new System.Drawing.Point(111, 48);
            this.txtFQAID.Name = "txtFQAID";
            this.txtFQAID.ReadOnly = true;
            this.txtFQAID.Size = new System.Drawing.Size(255, 20);
            this.txtFQAID.TabIndex = 5;
            // 
            // FormFQA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 283);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormFQA";
            this.Text = "Form FQA 1";
            this.Load += new System.EventHandler(this.FormFQA_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private MetroFramework.Controls.MetroButton btnExit;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private MetroFramework.Controls.MetroButton btnInActive;
        private MetroFramework.Controls.MetroButton btnEdit;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private System.Windows.Forms.CheckBox chkInActive;
        private System.Windows.Forms.TextBox txtFQADesc;
        private System.Windows.Forms.TextBox txtFQAID;
        private MetroFramework.Controls.MetroButton btnSearchCOA;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private System.Windows.Forms.TextBox txtCOAID;
    }
}