namespace ISBS_New.Master.DeliveryMethod
{
    partial class DeliveryForm
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
            this.txtDeliveryMethod = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdBtnFranco = new System.Windows.Forms.RadioButton();
            this.rdBtnLoco = new System.Windows.Forms.RadioButton();
            this.grpUser.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(184, 217);
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
            this.btnExit.Location = new System.Drawing.Point(265, 217);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 169;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(103, 217);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 168;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(22, 217);
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
            this.grpUser.Controls.Add(this.groupBox1);
            this.grpUser.Controls.Add(this.txtDeskripsi);
            this.grpUser.Controls.Add(this.Label3);
            this.grpUser.Controls.Add(this.txtDeliveryMethod);
            this.grpUser.Controls.Add(this.Label1);
            this.grpUser.Location = new System.Drawing.Point(12, 55);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(350, 156);
            this.grpUser.TabIndex = 166;
            this.grpUser.TabStop = false;
            // 
            // txtDeskripsi
            // 
            this.txtDeskripsi.Enabled = false;
            this.txtDeskripsi.Location = new System.Drawing.Point(133, 103);
            this.txtDeskripsi.MaxLength = 50;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.Size = new System.Drawing.Size(185, 20);
            this.txtDeskripsi.TabIndex = 4;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(19, 106);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(50, 13);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Deskripsi";
            // 
            // txtDeliveryMethod
            // 
            this.txtDeliveryMethod.Enabled = false;
            this.txtDeliveryMethod.Location = new System.Drawing.Point(133, 20);
            this.txtDeliveryMethod.MaxLength = 20;
            this.txtDeliveryMethod.Name = "txtDeliveryMethod";
            this.txtDeliveryMethod.Size = new System.Drawing.Size(185, 20);
            this.txtDeliveryMethod.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(19, 23);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(84, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Delivery Method";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdBtnLoco);
            this.groupBox1.Controls.Add(this.rdBtnFranco);
            this.groupBox1.Location = new System.Drawing.Point(133, 47);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(185, 48);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Franco / Loco";
            // 
            // rdBtnFranco
            // 
            this.rdBtnFranco.AutoSize = true;
            this.rdBtnFranco.Location = new System.Drawing.Point(7, 20);
            this.rdBtnFranco.Name = "rdBtnFranco";
            this.rdBtnFranco.Size = new System.Drawing.Size(58, 17);
            this.rdBtnFranco.TabIndex = 0;
            this.rdBtnFranco.Text = "Franco";
            this.rdBtnFranco.UseVisualStyleBackColor = true;
            // 
            // rdBtnLoco
            // 
            this.rdBtnLoco.AutoSize = true;
            this.rdBtnLoco.Location = new System.Drawing.Point(88, 20);
            this.rdBtnLoco.Name = "rdBtnLoco";
            this.rdBtnLoco.Size = new System.Drawing.Size(49, 17);
            this.rdBtnLoco.TabIndex = 1;
            this.rdBtnLoco.TabStop = true;
            this.rdBtnLoco.Text = "Loco";
            this.rdBtnLoco.UseVisualStyleBackColor = true;
            // 
            // DeliveryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 276);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpUser);
            this.Name = "DeliveryForm";
            this.Resizable = false;
            this.Text = "Delivery Form";
            this.Load += new System.EventHandler(this.DeliveryForm_Load);
            this.grpUser.ResumeLayout(false);
            this.grpUser.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        internal System.Windows.Forms.TextBox txtDeliveryMethod;
        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdBtnLoco;
        private System.Windows.Forms.RadioButton rdBtnFranco;
    }
}