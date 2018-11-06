namespace ISBS_New.Master.Convertion
{
    partial class FormConvertion
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
            this.txtRecId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtRatio = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtItemDeskripsi = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFullItemId = new System.Windows.Forms.TextBox();
            this.txtToUnit = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFromUnit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearchItem = new System.Windows.Forms.Button();
            this.grpCompanyInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(275, 227);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 43;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(113, 227);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 42;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(32, 227);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 40;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(194, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 161;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpCompanyInfo
            // 
            this.grpCompanyInfo.Controls.Add(this.btnSearchItem);
            this.grpCompanyInfo.Controls.Add(this.txtRecId);
            this.grpCompanyInfo.Controls.Add(this.label6);
            this.grpCompanyInfo.Controls.Add(this.txtRatio);
            this.grpCompanyInfo.Controls.Add(this.label5);
            this.grpCompanyInfo.Controls.Add(this.txtItemDeskripsi);
            this.grpCompanyInfo.Controls.Add(this.label4);
            this.grpCompanyInfo.Controls.Add(this.txtFullItemId);
            this.grpCompanyInfo.Controls.Add(this.txtToUnit);
            this.grpCompanyInfo.Controls.Add(this.label3);
            this.grpCompanyInfo.Controls.Add(this.txtFromUnit);
            this.grpCompanyInfo.Controls.Add(this.label2);
            this.grpCompanyInfo.Controls.Add(this.label1);
            this.grpCompanyInfo.Location = new System.Drawing.Point(12, 54);
            this.grpCompanyInfo.Name = "grpCompanyInfo";
            this.grpCompanyInfo.Size = new System.Drawing.Size(364, 160);
            this.grpCompanyInfo.TabIndex = 162;
            this.grpCompanyInfo.TabStop = false;
            // 
            // txtRecId
            // 
            this.txtRecId.Location = new System.Drawing.Point(87, 20);
            this.txtRecId.Name = "txtRecId";
            this.txtRecId.Size = new System.Drawing.Size(40, 20);
            this.txtRecId.TabIndex = 108;
            this.txtRecId.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(193, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 107;
            this.label6.Text = "%";
            // 
            // txtRatio
            // 
            this.txtRatio.Location = new System.Drawing.Point(133, 127);
            this.txtRatio.MaxLength = 30;
            this.txtRatio.Name = "txtRatio";
            this.txtRatio.Size = new System.Drawing.Size(54, 20);
            this.txtRatio.TabIndex = 106;
            this.txtRatio.Layout += new System.Windows.Forms.LayoutEventHandler(this.txtRatio_Layout);
            this.txtRatio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRatio_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 105;
            this.label5.Text = "Ratio";
            // 
            // txtItemDeskripsi
            // 
            this.txtItemDeskripsi.Location = new System.Drawing.Point(133, 48);
            this.txtItemDeskripsi.MaxLength = 5;
            this.txtItemDeskripsi.Name = "txtItemDeskripsi";
            this.txtItemDeskripsi.ReadOnly = true;
            this.txtItemDeskripsi.Size = new System.Drawing.Size(185, 20);
            this.txtItemDeskripsi.TabIndex = 104;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 102;
            this.label4.Text = "Item Deskripsi";
            // 
            // txtFullItemId
            // 
            this.txtFullItemId.Location = new System.Drawing.Point(133, 20);
            this.txtFullItemId.MaxLength = 5;
            this.txtFullItemId.Name = "txtFullItemId";
            this.txtFullItemId.ReadOnly = true;
            this.txtFullItemId.Size = new System.Drawing.Size(185, 20);
            this.txtFullItemId.TabIndex = 101;
            // 
            // txtToUnit
            // 
            this.txtToUnit.Location = new System.Drawing.Point(133, 100);
            this.txtToUnit.MaxLength = 30;
            this.txtToUnit.Name = "txtToUnit";
            this.txtToUnit.Size = new System.Drawing.Size(185, 20);
            this.txtToUnit.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "To Unit";
            // 
            // txtFromUnit
            // 
            this.txtFromUnit.Location = new System.Drawing.Point(133, 74);
            this.txtFromUnit.MaxLength = 5;
            this.txtFromUnit.Name = "txtFromUnit";
            this.txtFromUnit.Size = new System.Drawing.Size(185, 20);
            this.txtFromUnit.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "From Unit";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FullItem Id";
            // 
            // btnSearchItem
            // 
            this.btnSearchItem.Location = new System.Drawing.Point(324, 19);
            this.btnSearchItem.Name = "btnSearchItem";
            this.btnSearchItem.Size = new System.Drawing.Size(34, 23);
            this.btnSearchItem.TabIndex = 1;
            this.btnSearchItem.Text = "...";
            this.btnSearchItem.UseVisualStyleBackColor = true;
            this.btnSearchItem.Click += new System.EventHandler(this.btnSearchGroup_Click);
            // 
            // FormConvertion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 263);
            this.Controls.Add(this.grpCompanyInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Name = "FormConvertion";
            this.Text = "Form Convertion";
            this.Load += new System.EventHandler(this.FormConvertion_Load);
            this.Shown += new System.EventHandler(this.FormConvertion_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormConvertion_FormClosed);
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
        private System.Windows.Forms.TextBox txtFromUnit;
        private System.Windows.Forms.TextBox txtToUnit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFullItemId;
        private System.Windows.Forms.Button btnSearchItem;
        private System.Windows.Forms.TextBox txtItemDeskripsi;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtRatio;
        private System.Windows.Forms.TextBox txtRecId;
    }
}