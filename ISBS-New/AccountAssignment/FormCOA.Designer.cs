namespace ISBS_New.AccountAssignment
{
    partial class FormCOA
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
            this.txtCOADeskripsi = new System.Windows.Forms.TextBox();
            this.btnSearchMCOA = new System.Windows.Forms.Button();
            this.chkInActive = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtEndCoa = new System.Windows.Forms.TextBox();
            this.txtStartCoa = new System.Windows.Forms.TextBox();
            this.txtCOAID = new System.Windows.Forms.TextBox();
            this.txtMCOADeskripsi = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExit = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnEdit = new MetroFramework.Controls.MetroButton();
            this.btnInActive = new MetroFramework.Controls.MetroButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCOADeskripsi);
            this.groupBox1.Controls.Add(this.btnSearchMCOA);
            this.groupBox1.Controls.Add(this.chkInActive);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtEndCoa);
            this.groupBox1.Controls.Add(this.txtStartCoa);
            this.groupBox1.Controls.Add(this.txtCOAID);
            this.groupBox1.Controls.Add(this.txtMCOADeskripsi);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(23, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(472, 130);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // txtCOADeskripsi
            // 
            this.txtCOADeskripsi.Location = new System.Drawing.Point(106, 68);
            this.txtCOADeskripsi.Name = "txtCOADeskripsi";
            this.txtCOADeskripsi.Size = new System.Drawing.Size(180, 20);
            this.txtCOADeskripsi.TabIndex = 32;
            // 
            // btnSearchMCOA
            // 
            this.btnSearchMCOA.AutoSize = true;
            this.btnSearchMCOA.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchMCOA.Location = new System.Drawing.Point(260, 12);
            this.btnSearchMCOA.Name = "btnSearchMCOA";
            this.btnSearchMCOA.Size = new System.Drawing.Size(26, 23);
            this.btnSearchMCOA.TabIndex = 31;
            this.btnSearchMCOA.Text = "...";
            this.btnSearchMCOA.UseVisualStyleBackColor = true;
            this.btnSearchMCOA.Click += new System.EventHandler(this.btnSearchMCOA_Click);
            // 
            // chkInActive
            // 
            this.chkInActive.AutoSize = true;
            this.chkInActive.Location = new System.Drawing.Point(394, 95);
            this.chkInActive.Name = "chkInActive";
            this.chkInActive.Size = new System.Drawing.Size(56, 17);
            this.chkInActive.TabIndex = 30;
            this.chkInActive.Text = "Active";
            this.chkInActive.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(191, 97);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(10, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "-";
            // 
            // txtEndCoa
            // 
            this.txtEndCoa.Location = new System.Drawing.Point(213, 94);
            this.txtEndCoa.Name = "txtEndCoa";
            this.txtEndCoa.ReadOnly = true;
            this.txtEndCoa.Size = new System.Drawing.Size(73, 20);
            this.txtEndCoa.TabIndex = 27;
            this.txtEndCoa.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEndFQA_KeyPress);
            // 
            // txtStartCoa
            // 
            this.txtStartCoa.AcceptsReturn = true;
            this.txtStartCoa.Location = new System.Drawing.Point(106, 94);
            this.txtStartCoa.Name = "txtStartCoa";
            this.txtStartCoa.ReadOnly = true;
            this.txtStartCoa.Size = new System.Drawing.Size(73, 20);
            this.txtStartCoa.TabIndex = 26;
            this.txtStartCoa.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStartFQA_KeyPress);
            // 
            // txtCOAID
            // 
            this.txtCOAID.Location = new System.Drawing.Point(106, 40);
            this.txtCOAID.Name = "txtCOAID";
            this.txtCOAID.Size = new System.Drawing.Size(180, 20);
            this.txtCOAID.TabIndex = 25;
            this.txtCOAID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCOAID_KeyPress_1);
            // 
            // txtMCOADeskripsi
            // 
            this.txtMCOADeskripsi.Location = new System.Drawing.Point(106, 14);
            this.txtMCOADeskripsi.Name = "txtMCOADeskripsi";
            this.txtMCOADeskripsi.ReadOnly = true;
            this.txtMCOADeskripsi.Size = new System.Drawing.Size(148, 20);
            this.txtMCOADeskripsi.TabIndex = 24;
            this.txtMCOADeskripsi.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCOAID_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(89, 97);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(10, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = ":";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(89, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(10, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = ":";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(89, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = ":";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(89, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = ":";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Range COA";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "COA Deskripsi";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "COA ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "MCOA Deskripsi";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnExit);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.btnEdit);
            this.groupBox2.Controls.Add(this.btnInActive);
            this.groupBox2.Location = new System.Drawing.Point(24, 199);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(471, 44);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(374, 15);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseSelectable = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(293, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(212, 15);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(131, 15);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseSelectable = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnInActive
            // 
            this.btnInActive.Location = new System.Drawing.Point(6, 15);
            this.btnInActive.Name = "btnInActive";
            this.btnInActive.Size = new System.Drawing.Size(75, 23);
            this.btnInActive.TabIndex = 0;
            this.btnInActive.Text = "InActive";
            this.btnInActive.UseSelectable = true;
            this.btnInActive.Click += new System.EventHandler(this.btnInActive_Click);
            // 
            // FormCOA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 267);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormCOA";
            this.Text = "Form Coa";
            this.Load += new System.EventHandler(this.FormCOA_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkInActive;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtEndCoa;
        private System.Windows.Forms.TextBox txtStartCoa;
        private System.Windows.Forms.TextBox txtCOAID;
        private System.Windows.Forms.TextBox txtMCOADeskripsi;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private MetroFramework.Controls.MetroButton btnExit;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnEdit;
        private MetroFramework.Controls.MetroButton btnInActive;
        private System.Windows.Forms.Button btnSearchMCOA;
        private System.Windows.Forms.TextBox txtCOADeskripsi;

    }
}