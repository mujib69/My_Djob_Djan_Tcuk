namespace ISBS_New.Master.Customer
{
    partial class FrmPop_Address
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPop_Address));
            this.txtKode_Pos = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel12 = new MetroFramework.Controls.MetroLabel();
            this.txtProvinsi = new MetroFramework.Controls.MetroTextBox();
            this.txtKota = new MetroFramework.Controls.MetroTextBox();
            this.txtAddress = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnEdit = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.chkPrimaryC = new MetroFramework.Controls.MetroCheckBox();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.Cmb_Purpose = new MetroFramework.Controls.MetroComboBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.txtRT = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.txtRW = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.txtNama_Address = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel9 = new MetroFramework.Controls.MetroLabel();
            this.txtNama_Prsh = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel10 = new MetroFramework.Controls.MetroLabel();
            this.txtKode_Prsh = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel11 = new MetroFramework.Controls.MetroLabel();
            this.btnDelete = new MetroFramework.Controls.MetroButton();
            this.btntxttblCustomer_Kota = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtKode_Pos
            // 
            this.txtKode_Pos.Lines = new string[0];
            this.txtKode_Pos.Location = new System.Drawing.Point(161, 221);
            this.txtKode_Pos.MaxLength = 5;
            this.txtKode_Pos.Name = "txtKode_Pos";
            this.txtKode_Pos.PasswordChar = '\0';
            this.txtKode_Pos.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtKode_Pos.SelectedText = "";
            this.txtKode_Pos.Size = new System.Drawing.Size(61, 23);
            this.txtKode_Pos.TabIndex = 41;
            this.txtKode_Pos.UseSelectable = true;
            // 
            // metroLabel12
            // 
            this.metroLabel12.AutoSize = true;
            this.metroLabel12.Location = new System.Drawing.Point(25, 223);
            this.metroLabel12.Name = "metroLabel12";
            this.metroLabel12.Size = new System.Drawing.Size(73, 19);
            this.metroLabel12.TabIndex = 40;
            this.metroLabel12.Text = "Area Code";
            // 
            // txtProvinsi
            // 
            this.txtProvinsi.Lines = new string[0];
            this.txtProvinsi.Location = new System.Drawing.Point(161, 250);
            this.txtProvinsi.MaxLength = 20;
            this.txtProvinsi.Name = "txtProvinsi";
            this.txtProvinsi.PasswordChar = '\0';
            this.txtProvinsi.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtProvinsi.SelectedText = "";
            this.txtProvinsi.Size = new System.Drawing.Size(179, 23);
            this.txtProvinsi.TabIndex = 39;
            this.txtProvinsi.UseSelectable = true;
            // 
            // txtKota
            // 
            this.txtKota.Lines = new string[0];
            this.txtKota.Location = new System.Drawing.Point(161, 192);
            this.txtKota.MaxLength = 30;
            this.txtKota.Name = "txtKota";
            this.txtKota.PasswordChar = '\0';
            this.txtKota.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtKota.SelectedText = "";
            this.txtKota.Size = new System.Drawing.Size(179, 23);
            this.txtKota.TabIndex = 38;
            this.txtKota.UseSelectable = true;
            this.txtKota.Leave += new System.EventHandler(this.txttblCustomer_Kota_Leave);
            this.txtKota.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txttblCustomer_Kota_KeyPress);
            // 
            // txtAddress
            // 
            this.txtAddress.Lines = new string[0];
            this.txtAddress.Location = new System.Drawing.Point(161, 133);
            this.txtAddress.MaxLength = 150;
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.PasswordChar = '\0';
            this.txtAddress.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtAddress.SelectedText = "";
            this.txtAddress.Size = new System.Drawing.Size(410, 53);
            this.txtAddress.TabIndex = 37;
            this.txtAddress.UseSelectable = true;
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.Location = new System.Drawing.Point(25, 252);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(59, 19);
            this.metroLabel8.TabIndex = 36;
            this.metroLabel8.Text = "Province";
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.Location = new System.Drawing.Point(25, 193);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(31, 19);
            this.metroLabel7.TabIndex = 35;
            this.metroLabel7.Text = "City";
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(25, 135);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(56, 19);
            this.metroLabel6.TabIndex = 34;
            this.metroLabel6.Text = "Address";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(319, 364);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 44;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(240, 364);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 43;
            this.btnEdit.Text = "&Edit";
            this.btnEdit.UseSelectable = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(398, 364);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 45;
            this.btnSave.Text = "Sa&ve";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkPrimaryC
            // 
            this.chkPrimaryC.AutoSize = true;
            this.chkPrimaryC.Location = new System.Drawing.Point(161, 314);
            this.chkPrimaryC.Name = "chkPrimaryC";
            this.chkPrimaryC.Size = new System.Drawing.Size(41, 15);
            this.chkPrimaryC.TabIndex = 47;
            this.chkPrimaryC.Text = "Yes";
            this.chkPrimaryC.UseSelectable = true;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(25, 312);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(55, 19);
            this.metroLabel5.TabIndex = 46;
            this.metroLabel5.Text = "Primary";
            // 
            // Cmb_Purpose
            // 
            this.Cmb_Purpose.FormattingEnabled = true;
            this.Cmb_Purpose.ItemHeight = 23;
            this.Cmb_Purpose.Location = new System.Drawing.Point(161, 279);
            this.Cmb_Purpose.Name = "Cmb_Purpose";
            this.Cmb_Purpose.Size = new System.Drawing.Size(179, 29);
            this.Cmb_Purpose.TabIndex = 48;
            this.Cmb_Purpose.UseSelectable = true;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(25, 279);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(57, 19);
            this.metroLabel1.TabIndex = 49;
            this.metroLabel1.Text = "Purpose";
            // 
            // txtRT
            // 
            this.txtRT.Lines = new string[0];
            this.txtRT.Location = new System.Drawing.Point(263, 221);
            this.txtRT.MaxLength = 5;
            this.txtRT.Name = "txtRT";
            this.txtRT.PasswordChar = '\0';
            this.txtRT.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtRT.SelectedText = "";
            this.txtRT.Size = new System.Drawing.Size(61, 23);
            this.txtRT.TabIndex = 51;
            this.txtRT.UseSelectable = true;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(234, 223);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(24, 19);
            this.metroLabel2.TabIndex = 50;
            this.metroLabel2.Text = "RT";
            // 
            // txtRW
            // 
            this.txtRW.Lines = new string[0];
            this.txtRW.Location = new System.Drawing.Point(382, 221);
            this.txtRW.MaxLength = 5;
            this.txtRW.Name = "txtRW";
            this.txtRW.PasswordChar = '\0';
            this.txtRW.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtRW.SelectedText = "";
            this.txtRW.Size = new System.Drawing.Size(61, 23);
            this.txtRW.TabIndex = 53;
            this.txtRW.UseSelectable = true;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(349, 223);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(30, 19);
            this.metroLabel3.TabIndex = 52;
            this.metroLabel3.Text = "RW";
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(330, 223);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(14, 19);
            this.metroLabel4.TabIndex = 54;
            this.metroLabel4.Text = "/";
            // 
            // txtNama_Address
            // 
            this.txtNama_Address.Lines = new string[0];
            this.txtNama_Address.Location = new System.Drawing.Point(161, 104);
            this.txtNama_Address.MaxLength = 100;
            this.txtNama_Address.Name = "txtNama_Address";
            this.txtNama_Address.PasswordChar = '\0';
            this.txtNama_Address.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtNama_Address.SelectedText = "";
            this.txtNama_Address.Size = new System.Drawing.Size(210, 23);
            this.txtNama_Address.TabIndex = 56;
            this.txtNama_Address.UseSelectable = true;
            // 
            // metroLabel9
            // 
            this.metroLabel9.AutoSize = true;
            this.metroLabel9.Location = new System.Drawing.Point(25, 106);
            this.metroLabel9.Name = "metroLabel9";
            this.metroLabel9.Size = new System.Drawing.Size(94, 19);
            this.metroLabel9.TabIndex = 55;
            this.metroLabel9.Text = "Contact Name";
            // 
            // txtNama_Prsh
            // 
            this.txtNama_Prsh.Lines = new string[0];
            this.txtNama_Prsh.Location = new System.Drawing.Point(249, 75);
            this.txtNama_Prsh.MaxLength = 30;
            this.txtNama_Prsh.Name = "txtNama_Prsh";
            this.txtNama_Prsh.PasswordChar = '\0';
            this.txtNama_Prsh.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtNama_Prsh.SelectedText = "";
            this.txtNama_Prsh.Size = new System.Drawing.Size(322, 23);
            this.txtNama_Prsh.TabIndex = 58;
            this.txtNama_Prsh.UseSelectable = true;
            // 
            // metroLabel10
            // 
            this.metroLabel10.AutoSize = true;
            this.metroLabel10.Location = new System.Drawing.Point(25, 77);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(68, 19);
            this.metroLabel10.TabIndex = 57;
            this.metroLabel10.Text = "Kode Prsh";
            // 
            // txtKode_Prsh
            // 
            this.txtKode_Prsh.Lines = new string[0];
            this.txtKode_Prsh.Location = new System.Drawing.Point(161, 75);
            this.txtKode_Prsh.MaxLength = 30;
            this.txtKode_Prsh.Name = "txtKode_Prsh";
            this.txtKode_Prsh.PasswordChar = '\0';
            this.txtKode_Prsh.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtKode_Prsh.SelectedText = "";
            this.txtKode_Prsh.Size = new System.Drawing.Size(62, 23);
            this.txtKode_Prsh.TabIndex = 59;
            this.txtKode_Prsh.UseSelectable = true;
            // 
            // metroLabel11
            // 
            this.metroLabel11.AutoSize = true;
            this.metroLabel11.Location = new System.Drawing.Point(229, 77);
            this.metroLabel11.Name = "metroLabel11";
            this.metroLabel11.Size = new System.Drawing.Size(15, 19);
            this.metroLabel11.TabIndex = 60;
            this.metroLabel11.Text = "-";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(477, 364);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 61;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseSelectable = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btntxttblCustomer_Kota
            // 
            this.btntxttblCustomer_Kota.Image = ((System.Drawing.Image)(resources.GetObject("btntxttblCustomer_Kota.Image")));
            this.btntxttblCustomer_Kota.Location = new System.Drawing.Point(346, 192);
            this.btntxttblCustomer_Kota.Name = "btntxttblCustomer_Kota";
            this.btntxttblCustomer_Kota.Size = new System.Drawing.Size(25, 23);
            this.btntxttblCustomer_Kota.TabIndex = 42;
            this.btntxttblCustomer_Kota.UseVisualStyleBackColor = true;
            this.btntxttblCustomer_Kota.Click += new System.EventHandler(this.btntxttblCustomer_Kota_Click);
            // 
            // FrmPop_Address
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 398);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.metroLabel11);
            this.Controls.Add(this.txtKode_Prsh);
            this.Controls.Add(this.txtNama_Prsh);
            this.Controls.Add(this.metroLabel10);
            this.Controls.Add(this.txtNama_Address);
            this.Controls.Add(this.metroLabel9);
            this.Controls.Add(this.metroLabel4);
            this.Controls.Add(this.txtRW);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.txtRT);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.Cmb_Purpose);
            this.Controls.Add(this.chkPrimaryC);
            this.Controls.Add(this.metroLabel5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btntxttblCustomer_Kota);
            this.Controls.Add(this.txtKode_Pos);
            this.Controls.Add(this.metroLabel12);
            this.Controls.Add(this.txtProvinsi);
            this.Controls.Add(this.txtKota);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.metroLabel8);
            this.Controls.Add(this.metroLabel7);
            this.Controls.Add(this.metroLabel6);
            this.Name = "FrmPop_Address";
            this.Resizable = false;
            this.Text = "Description";
            this.Load += new System.EventHandler(this.FrmPop_Address_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btntxttblCustomer_Kota;
        private MetroFramework.Controls.MetroTextBox txtKode_Pos;
        private MetroFramework.Controls.MetroLabel metroLabel12;
        private MetroFramework.Controls.MetroTextBox txtProvinsi;
        private MetroFramework.Controls.MetroTextBox txtKota;
        private MetroFramework.Controls.MetroTextBox txtAddress;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnEdit;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroCheckBox chkPrimaryC;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroComboBox Cmb_Purpose;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTextBox txtRT;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroTextBox txtRW;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroTextBox txtNama_Address;
        private MetroFramework.Controls.MetroLabel metroLabel9;
        private MetroFramework.Controls.MetroTextBox txtNama_Prsh;
        private MetroFramework.Controls.MetroLabel metroLabel10;
        private MetroFramework.Controls.MetroTextBox txtKode_Prsh;
        private MetroFramework.Controls.MetroLabel metroLabel11;
        private MetroFramework.Controls.MetroButton btnDelete;
    }
}