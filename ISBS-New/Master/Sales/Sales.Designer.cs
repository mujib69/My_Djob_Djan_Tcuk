namespace ISBS_New.Master.Sales
{
    partial class Sales
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtKode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNama = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAlamat = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTelepon = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtHandphone = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPersen = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtGroup = new System.Windows.Forms.TextBox();
            this.btnGroup = new System.Windows.Forms.Button();
            this.txtKategori = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.rdTokoProyek1 = new System.Windows.Forms.RadioButton();
            this.rdTokoProyek2 = new System.Windows.Forms.RadioButton();
            this.rdTokoProyek3 = new System.Windows.Forms.RadioButton();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvUser = new System.Windows.Forms.DataGridView();
            this.ckKunci = new System.Windows.Forms.CheckBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUser)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kode Sales";
            // 
            // txtKode
            // 
            this.txtKode.Location = new System.Drawing.Point(114, 15);
            this.txtKode.Name = "txtKode";
            this.txtKode.Size = new System.Drawing.Size(179, 20);
            this.txtKode.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nama Sales";
            // 
            // txtNama
            // 
            this.txtNama.Location = new System.Drawing.Point(114, 38);
            this.txtNama.Name = "txtNama";
            this.txtNama.Size = new System.Drawing.Size(179, 20);
            this.txtNama.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Alamat";
            // 
            // txtAlamat
            // 
            this.txtAlamat.Location = new System.Drawing.Point(114, 61);
            this.txtAlamat.Multiline = true;
            this.txtAlamat.Name = "txtAlamat";
            this.txtAlamat.Size = new System.Drawing.Size(179, 67);
            this.txtAlamat.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Telepon";
            // 
            // txtTelepon
            // 
            this.txtTelepon.Location = new System.Drawing.Point(114, 134);
            this.txtTelepon.Name = "txtTelepon";
            this.txtTelepon.Size = new System.Drawing.Size(179, 20);
            this.txtTelepon.TabIndex = 7;
            this.txtTelepon.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTelepon_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 159);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Handphone";
            // 
            // txtHandphone
            // 
            this.txtHandphone.Location = new System.Drawing.Point(114, 157);
            this.txtHandphone.Name = "txtHandphone";
            this.txtHandphone.Size = new System.Drawing.Size(179, 20);
            this.txtHandphone.TabIndex = 9;
            this.txtHandphone.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtHandphone_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Persen";
            // 
            // txtPersen
            // 
            this.txtPersen.Location = new System.Drawing.Point(114, 180);
            this.txtPersen.Name = "txtPersen";
            this.txtPersen.Size = new System.Drawing.Size(179, 20);
            this.txtPersen.TabIndex = 11;
            this.txtPersen.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Persen_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 205);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Group";
            // 
            // txtGroup
            // 
            this.txtGroup.Enabled = false;
            this.txtGroup.Location = new System.Drawing.Point(114, 203);
            this.txtGroup.Name = "txtGroup";
            this.txtGroup.ReadOnly = true;
            this.txtGroup.Size = new System.Drawing.Size(179, 20);
            this.txtGroup.TabIndex = 13;
            // 
            // btnGroup
            // 
            this.btnGroup.Location = new System.Drawing.Point(293, 202);
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Size = new System.Drawing.Size(32, 23);
            this.btnGroup.TabIndex = 143;
            this.btnGroup.Text = "...";
            this.btnGroup.UseVisualStyleBackColor = true;
            this.btnGroup.Click += new System.EventHandler(this.btnGroup_Click);
            // 
            // txtKategori
            // 
            this.txtKategori.Location = new System.Drawing.Point(114, 251);
            this.txtKategori.Name = "txtKategori";
            this.txtKategori.Size = new System.Drawing.Size(179, 20);
            this.txtKategori.TabIndex = 145;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 254);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 144;
            this.label8.Text = "Kategori";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 277);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 146;
            this.label9.Text = "Toko / Proyek";
            // 
            // rdTokoProyek1
            // 
            this.rdTokoProyek1.AutoSize = true;
            this.rdTokoProyek1.Location = new System.Drawing.Point(114, 275);
            this.rdTokoProyek1.Name = "rdTokoProyek1";
            this.rdTokoProyek1.Size = new System.Drawing.Size(50, 17);
            this.rdTokoProyek1.TabIndex = 147;
            this.rdTokoProyek1.TabStop = true;
            this.rdTokoProyek1.Text = "Toko";
            this.rdTokoProyek1.UseVisualStyleBackColor = true;
            // 
            // rdTokoProyek2
            // 
            this.rdTokoProyek2.AutoSize = true;
            this.rdTokoProyek2.Location = new System.Drawing.Point(170, 275);
            this.rdTokoProyek2.Name = "rdTokoProyek2";
            this.rdTokoProyek2.Size = new System.Drawing.Size(58, 17);
            this.rdTokoProyek2.TabIndex = 148;
            this.rdTokoProyek2.TabStop = true;
            this.rdTokoProyek2.Text = "Proyek";
            this.rdTokoProyek2.UseVisualStyleBackColor = true;
            // 
            // rdTokoProyek3
            // 
            this.rdTokoProyek3.AutoSize = true;
            this.rdTokoProyek3.Location = new System.Drawing.Point(231, 275);
            this.rdTokoProyek3.Name = "rdTokoProyek3";
            this.rdTokoProyek3.Size = new System.Drawing.Size(62, 17);
            this.rdTokoProyek3.TabIndex = 149;
            this.rdTokoProyek3.TabStop = true;
            this.rdTokoProyek3.Text = "Lainnya";
            this.rdTokoProyek3.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(114, 314);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(50, 23);
            this.btnEdit.TabIndex = 150;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(219, 314);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(50, 23);
            this.btnCancel.TabIndex = 151;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(344, 45);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(57, 23);
            this.btnAdd.TabIndex = 154;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(403, 45);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(56, 23);
            this.btnDelete.TabIndex = 155;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvUser
            // 
            this.dgvUser.AllowUserToAddRows = false;
            this.dgvUser.AllowUserToDeleteRows = false;
            this.dgvUser.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUser.Location = new System.Drawing.Point(345, 71);
            this.dgvUser.Name = "dgvUser";
            this.dgvUser.ReadOnly = true;
            this.dgvUser.Size = new System.Drawing.Size(330, 219);
            this.dgvUser.TabIndex = 156;
            // 
            // ckKunci
            // 
            this.ckKunci.AutoSize = true;
            this.ckKunci.Location = new System.Drawing.Point(344, 15);
            this.ckKunci.Name = "ckKunci";
            this.ckKunci.Size = new System.Drawing.Size(134, 17);
            this.ckKunci.TabIndex = 157;
            this.ckKunci.Text = "Kunci User ID Program";
            this.ckKunci.UseVisualStyleBackColor = true;
            this.ckKunci.Click += new System.EventHandler(this.KunciUser_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(271, 314);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(51, 23);
            this.btnExit.TabIndex = 158;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(166, 314);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 23);
            this.btnSave.TabIndex = 152;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtGroupName
            // 
            this.txtGroupName.Enabled = false;
            this.txtGroupName.Location = new System.Drawing.Point(114, 227);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.ReadOnly = true;
            this.txtGroupName.Size = new System.Drawing.Size(179, 20);
            this.txtGroupName.TabIndex = 159;
            // 
            // Sales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 362);
            this.Controls.Add(this.txtGroupName);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.ckKunci);
            this.Controls.Add(this.dgvUser);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.rdTokoProyek3);
            this.Controls.Add(this.rdTokoProyek2);
            this.Controls.Add(this.rdTokoProyek1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtKategori);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnGroup);
            this.Controls.Add(this.txtGroup);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtPersen);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtHandphone);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtTelepon);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAlamat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNama);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtKode);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Sales";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sales";
            this.Load += new System.EventHandler(this.Sales_Load);
            this.Shown += new System.EventHandler(this.Sales_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Sales_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtKode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNama;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAlamat;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTelepon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtHandphone;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPersen;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtGroup;
        private System.Windows.Forms.Button btnGroup;
        private System.Windows.Forms.TextBox txtKategori;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rdTokoProyek1;
        private System.Windows.Forms.RadioButton rdTokoProyek2;
        private System.Windows.Forms.RadioButton rdTokoProyek3;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.DataGridView dgvUser;
        private System.Windows.Forms.CheckBox ckKunci;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtGroupName;

    }
}
