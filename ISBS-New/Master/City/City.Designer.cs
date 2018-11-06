namespace ISBS_New.Master.City
{
    partial class City
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.grpProvince = new System.Windows.Forms.GroupBox();
            this.btnSearchProvince = new System.Windows.Forms.Button();
            this.txtProvinceName = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtProvinceId = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCityName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCityId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.grpProvince.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(164, 238);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 83;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(53, 238);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 85;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // grpProvince
            // 
            this.grpProvince.Controls.Add(this.btnSearchProvince);
            this.grpProvince.Controls.Add(this.txtProvinceName);
            this.grpProvince.Controls.Add(this.Label2);
            this.grpProvince.Controls.Add(this.txtProvinceId);
            this.grpProvince.Controls.Add(this.Label1);
            this.grpProvince.Location = new System.Drawing.Point(13, 55);
            this.grpProvince.Name = "grpProvince";
            this.grpProvince.Size = new System.Drawing.Size(420, 84);
            this.grpProvince.TabIndex = 81;
            this.grpProvince.TabStop = false;
            // 
            // btnSearchProvince
            // 
            this.btnSearchProvince.Enabled = false;
            this.btnSearchProvince.Location = new System.Drawing.Point(361, 17);
            this.btnSearchProvince.Name = "btnSearchProvince";
            this.btnSearchProvince.Size = new System.Drawing.Size(36, 23);
            this.btnSearchProvince.TabIndex = 28;
            this.btnSearchProvince.Text = "...";
            this.btnSearchProvince.UseVisualStyleBackColor = true;
            this.btnSearchProvince.Click += new System.EventHandler(this.btnSearchProvince_Click);
            // 
            // txtProvinceName
            // 
            this.txtProvinceName.Enabled = false;
            this.txtProvinceName.Location = new System.Drawing.Point(132, 45);
            this.txtProvinceName.MaxLength = 50;
            this.txtProvinceName.Name = "txtProvinceName";
            this.txtProvinceName.Size = new System.Drawing.Size(218, 20);
            this.txtProvinceName.TabIndex = 2;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(17, 48);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(80, 13);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Province Name";
            // 
            // txtProvinceId
            // 
            this.txtProvinceId.Enabled = false;
            this.txtProvinceId.Location = new System.Drawing.Point(132, 19);
            this.txtProvinceId.MaxLength = 5;
            this.txtProvinceId.Name = "txtProvinceId";
            this.txtProvinceId.Size = new System.Drawing.Size(94, 20);
            this.txtProvinceId.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(17, 22);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(63, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Province ID";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(288, 238);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 86;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCityName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtCityId);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(13, 145);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(420, 84);
            this.groupBox1.TabIndex = 82;
            this.groupBox1.TabStop = false;
            // 
            // txtCityName
            // 
            this.txtCityName.Enabled = false;
            this.txtCityName.Location = new System.Drawing.Point(132, 45);
            this.txtCityName.MaxLength = 50;
            this.txtCityName.Name = "txtCityName";
            this.txtCityName.Size = new System.Drawing.Size(218, 20);
            this.txtCityName.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "City Name";
            // 
            // txtCityId
            // 
            this.txtCityId.Enabled = false;
            this.txtCityId.Location = new System.Drawing.Point(132, 19);
            this.txtCityId.MaxLength = 5;
            this.txtCityId.Name = "txtCityId";
            this.txtCityId.Size = new System.Drawing.Size(94, 20);
            this.txtCityId.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "City ID";
            // 
            // City
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 274);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.grpProvince);
            this.Controls.Add(this.btnExit);
            this.Name = "City";
            this.Text = "City";
            this.Load += new System.EventHandler(this.City_Load);
            this.grpProvince.ResumeLayout(false);
            this.grpProvince.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.GroupBox grpProvince;
        internal System.Windows.Forms.TextBox txtProvinceName;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox txtProvinceId;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnSearchProvince;
        internal System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.TextBox txtCityName;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.TextBox txtCityId;
        internal System.Windows.Forms.Label label4;

    }
}