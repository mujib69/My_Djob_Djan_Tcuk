namespace ISBS_New.Master.Invent.Measurement
{
    partial class FormMeasurement
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
            this.txtMeasurementId = new System.Windows.Forms.TextBox();
            this.txtMeasurementDesc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpCompanyInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(261, 149);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 43;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(99, 149);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 42;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(18, 149);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 40;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(180, 149);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 161;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpCompanyInfo
            // 
            this.grpCompanyInfo.Controls.Add(this.txtMeasurementId);
            this.grpCompanyInfo.Controls.Add(this.txtMeasurementDesc);
            this.grpCompanyInfo.Controls.Add(this.label2);
            this.grpCompanyInfo.Controls.Add(this.label1);
            this.grpCompanyInfo.Location = new System.Drawing.Point(12, 69);
            this.grpCompanyInfo.Name = "grpCompanyInfo";
            this.grpCompanyInfo.Size = new System.Drawing.Size(358, 68);
            this.grpCompanyInfo.TabIndex = 162;
            this.grpCompanyInfo.TabStop = false;
            // 
            // txtMeasurementId
            // 
            this.txtMeasurementId.Location = new System.Drawing.Point(127, 15);
            this.txtMeasurementId.MaxLength = 15;
            this.txtMeasurementId.Name = "txtMeasurementId";
            this.txtMeasurementId.Size = new System.Drawing.Size(116, 20);
            this.txtMeasurementId.TabIndex = 101;
            // 
            // txtMeasurementDesc
            // 
            this.txtMeasurementDesc.Location = new System.Drawing.Point(127, 40);
            this.txtMeasurementDesc.MaxLength = 30;
            this.txtMeasurementDesc.Name = "txtMeasurementDesc";
            this.txtMeasurementDesc.Size = new System.Drawing.Size(215, 20);
            this.txtMeasurementDesc.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Measurement ";
            // 
            // FormMeasurement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 187);
            this.Controls.Add(this.grpCompanyInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Name = "FormMeasurement";
            this.Resizable = false;
            this.Text = "Measurement";
            this.Load += new System.EventHandler(this.FormMeasurement_Load);
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
        private System.Windows.Forms.TextBox txtMeasurementDesc;
        private System.Windows.Forms.TextBox txtMeasurementId;
    }
}