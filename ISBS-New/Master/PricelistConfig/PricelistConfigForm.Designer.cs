namespace ISBS_New.Master.PricelistConfig
{
    partial class PricelistConfigForm
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
            this.cmbPriceType = new System.Windows.Forms.ComboBox();
            this.txtRecId = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSubGroup2ID = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFactor = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSearchSubGroup2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSubGroup2 = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.grpUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(202, 204);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 175;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(277, 204);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 174;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(127, 204);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 173;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(52, 204);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 172;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpUser
            // 
            this.grpUser.Controls.Add(this.cmbPriceType);
            this.grpUser.Controls.Add(this.txtRecId);
            this.grpUser.Controls.Add(this.label9);
            this.grpUser.Controls.Add(this.txtSubGroup2ID);
            this.grpUser.Controls.Add(this.label8);
            this.grpUser.Controls.Add(this.label5);
            this.grpUser.Controls.Add(this.label1);
            this.grpUser.Controls.Add(this.txtFactor);
            this.grpUser.Controls.Add(this.label7);
            this.grpUser.Controls.Add(this.btnSearchSubGroup2);
            this.grpUser.Controls.Add(this.label2);
            this.grpUser.Controls.Add(this.txtSubGroup2);
            this.grpUser.Controls.Add(this.Label3);
            this.grpUser.Location = new System.Drawing.Point(12, 58);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(406, 140);
            this.grpUser.TabIndex = 171;
            this.grpUser.TabStop = false;
            // 
            // cmbPriceType
            // 
            this.cmbPriceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPriceType.FormattingEnabled = true;
            this.cmbPriceType.Items.AddRange(new object[] {
            "2",
            "3",
            "7",
            "14",
            "21",
            "30",
            "40",
            "45",
            "60",
            "75",
            "90",
            "120",
            "150",
            "180"});
            this.cmbPriceType.Location = new System.Drawing.Point(130, 46);
            this.cmbPriceType.Name = "cmbPriceType";
            this.cmbPriceType.Size = new System.Drawing.Size(179, 21);
            this.cmbPriceType.TabIndex = 31;
            // 
            // txtRecId
            // 
            this.txtRecId.Location = new System.Drawing.Point(279, 99);
            this.txtRecId.MaxLength = 25;
            this.txtRecId.Name = "txtRecId";
            this.txtRecId.Size = new System.Drawing.Size(30, 20);
            this.txtRecId.TabIndex = 29;
            this.txtRecId.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(233, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 30;
            this.label9.Text = "RecId";
            this.label9.Visible = false;
            // 
            // txtSubGroup2ID
            // 
            this.txtSubGroup2ID.Location = new System.Drawing.Point(130, 99);
            this.txtSubGroup2ID.MaxLength = 25;
            this.txtSubGroup2ID.Name = "txtSubGroup2ID";
            this.txtSubGroup2ID.Size = new System.Drawing.Size(30, 20);
            this.txtSubGroup2ID.TabIndex = 27;
            this.txtSubGroup2ID.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 103);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Sub Group 2 ID";
            this.label8.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(314, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "%";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(314, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Days";
            // 
            // txtFactor
            // 
            this.txtFactor.Location = new System.Drawing.Point(130, 73);
            this.txtFactor.MaxLength = 50;
            this.txtFactor.Name = "txtFactor";
            this.txtFactor.Size = new System.Drawing.Size(179, 20);
            this.txtFactor.TabIndex = 22;
            this.txtFactor.Text = "0.00";
            this.txtFactor.Leave += new System.EventHandler(this.txtFactor_Leave);
            this.txtFactor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFactor_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Factor";
            // 
            // btnSearchSubGroup2
            // 
            this.btnSearchSubGroup2.Enabled = false;
            this.btnSearchSubGroup2.Location = new System.Drawing.Point(314, 18);
            this.btnSearchSubGroup2.Name = "btnSearchSubGroup2";
            this.btnSearchSubGroup2.Size = new System.Drawing.Size(36, 23);
            this.btnSearchSubGroup2.TabIndex = 15;
            this.btnSearchSubGroup2.Text = "...";
            this.btnSearchSubGroup2.UseVisualStyleBackColor = true;
            this.btnSearchSubGroup2.Click += new System.EventHandler(this.btnSearchSubGroup2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Price Type";
            // 
            // txtSubGroup2
            // 
            this.txtSubGroup2.Enabled = false;
            this.txtSubGroup2.Location = new System.Drawing.Point(130, 19);
            this.txtSubGroup2.MaxLength = 50;
            this.txtSubGroup2.Name = "txtSubGroup2";
            this.txtSubGroup2.Size = new System.Drawing.Size(179, 20);
            this.txtSubGroup2.TabIndex = 4;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(18, 23);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(67, 13);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Sub Group 2";
            // 
            // PricelistConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 248);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpUser);
            this.Name = "PricelistConfigForm";
            this.Resizable = false;
            this.Text = "Pricelist Config";
            this.Load += new System.EventHandler(this.PricelistConfigForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PricelistConfigForm_FormClosed);
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
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox txtSubGroup2;
        internal System.Windows.Forms.Button btnSearchSubGroup2;
        internal System.Windows.Forms.TextBox txtFactor;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox txtSubGroup2ID;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtRecId;
        internal System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbPriceType;
    }
}