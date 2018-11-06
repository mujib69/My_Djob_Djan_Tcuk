namespace ISBS_New.Master.Bank
{
    partial class FrmM_Bank
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
            this.mPanel_Bottom = new MetroFramework.Controls.MetroPanel();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnEdit = new System.Windows.Forms.Button();
            this.mPanel_Body = new MetroFramework.Controls.MetroPanel();
            this.mTabCtrl_Bank = new MetroFramework.Controls.MetroTabControl();
            this.mTabPage_General = new MetroFramework.Controls.MetroTabPage();
            this.txttblBank_Cabang = new System.Windows.Forms.TextBox();
            this.metroLabel11 = new MetroFramework.Controls.MetroLabel();
            this.cbxtblBank_Kode_Pos = new System.Windows.Forms.ComboBox();
            this.metroLabel10 = new MetroFramework.Controls.MetroLabel();
            this.cbxtblBank_Desa_Kelurahan = new System.Windows.Forms.ComboBox();
            this.metroLabel9 = new MetroFramework.Controls.MetroLabel();
            this.cbxtblBank_Kecamatan = new System.Windows.Forms.ComboBox();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.cbxtblBank_Provinsi = new System.Windows.Forms.ComboBox();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.cbxtblBank_Kota_Kabupaten = new System.Windows.Forms.ComboBox();
            this.cbxtblBank_Group = new System.Windows.Forms.ComboBox();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.txttblBank_Alamat2 = new System.Windows.Forms.TextBox();
            this.txttblBank_Alamat1 = new System.Windows.Forms.TextBox();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.txttblBank_Atas_Nama = new System.Windows.Forms.TextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.txttblBank_No_Rek = new System.Windows.Forms.TextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.txttblBank_Group_Nama = new System.Windows.Forms.TextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.txttblBank_Kode_Bank = new System.Windows.Forms.TextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.mTabPage_Contact = new MetroFramework.Controls.MetroTabPage();
            this.mPanel_Contact = new MetroFramework.Controls.MetroPanel();
            this.DtGridView_Contact = new System.Windows.Forms.DataGridView();
            this.mPanel_Ct_Bottom = new MetroFramework.Controls.MetroPanel();
            this.btnDelete_Ct = new System.Windows.Forms.Button();
            this.btnEdit_Ct = new System.Windows.Forms.Button();
            this.btnNew_Ct = new System.Windows.Forms.Button();
            this.mTabPage_Notes = new MetroFramework.Controls.MetroTabPage();
            this.txttblBank_Ket = new System.Windows.Forms.TextBox();
            this.metroLabel12 = new MetroFramework.Controls.MetroLabel();
            this.mPanel_Bottom.SuspendLayout();
            this.mPanel_Body.SuspendLayout();
            this.mTabCtrl_Bank.SuspendLayout();
            this.mTabPage_General.SuspendLayout();
            this.mTabPage_Contact.SuspendLayout();
            this.mPanel_Contact.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DtGridView_Contact)).BeginInit();
            this.mPanel_Ct_Bottom.SuspendLayout();
            this.mTabPage_Notes.SuspendLayout();
            this.SuspendLayout();
            // 
            // mPanel_Bottom
            // 
            this.mPanel_Bottom.Controls.Add(this.BtnDelete);
            this.mPanel_Bottom.Controls.Add(this.BtnCancel);
            this.mPanel_Bottom.Controls.Add(this.BtnSave);
            this.mPanel_Bottom.Controls.Add(this.BtnEdit);
            this.mPanel_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mPanel_Bottom.HorizontalScrollbarBarColor = true;
            this.mPanel_Bottom.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_Bottom.HorizontalScrollbarSize = 10;
            this.mPanel_Bottom.Location = new System.Drawing.Point(20, 476);
            this.mPanel_Bottom.Name = "mPanel_Bottom";
            this.mPanel_Bottom.Size = new System.Drawing.Size(643, 32);
            this.mPanel_Bottom.TabIndex = 0;
            this.mPanel_Bottom.VerticalScrollbarBarColor = true;
            this.mPanel_Bottom.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Bottom.VerticalScrollbarSize = 10;
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(561, 5);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(75, 23);
            this.BtnDelete.TabIndex = 18;
            this.BtnDelete.Text = "&Delete";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(399, 5);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 16;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(480, 5);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 17;
            this.BtnSave.Text = "Sa&ve";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnEdit
            // 
            this.BtnEdit.Location = new System.Drawing.Point(321, 5);
            this.BtnEdit.Name = "BtnEdit";
            this.BtnEdit.Size = new System.Drawing.Size(75, 23);
            this.BtnEdit.TabIndex = 15;
            this.BtnEdit.Text = "&Edit";
            this.BtnEdit.UseVisualStyleBackColor = true;
            this.BtnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // mPanel_Body
            // 
            this.mPanel_Body.Controls.Add(this.mTabCtrl_Bank);
            this.mPanel_Body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mPanel_Body.HorizontalScrollbarBarColor = true;
            this.mPanel_Body.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_Body.HorizontalScrollbarSize = 10;
            this.mPanel_Body.Location = new System.Drawing.Point(20, 60);
            this.mPanel_Body.Name = "mPanel_Body";
            this.mPanel_Body.Size = new System.Drawing.Size(643, 416);
            this.mPanel_Body.TabIndex = 1;
            this.mPanel_Body.VerticalScrollbarBarColor = true;
            this.mPanel_Body.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Body.VerticalScrollbarSize = 10;
            // 
            // mTabCtrl_Bank
            // 
            this.mTabCtrl_Bank.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.mTabCtrl_Bank.Controls.Add(this.mTabPage_General);
            this.mTabCtrl_Bank.Controls.Add(this.mTabPage_Contact);
            this.mTabCtrl_Bank.Controls.Add(this.mTabPage_Notes);
            this.mTabCtrl_Bank.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTabCtrl_Bank.Location = new System.Drawing.Point(0, 0);
            this.mTabCtrl_Bank.Name = "mTabCtrl_Bank";
            this.mTabCtrl_Bank.SelectedIndex = 0;
            this.mTabCtrl_Bank.Size = new System.Drawing.Size(643, 416);
            this.mTabCtrl_Bank.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mTabCtrl_Bank.TabIndex = 46;
            this.mTabCtrl_Bank.UseSelectable = true;
            // 
            // mTabPage_General
            // 
            this.mTabPage_General.Controls.Add(this.txttblBank_Cabang);
            this.mTabPage_General.Controls.Add(this.metroLabel11);
            this.mTabPage_General.Controls.Add(this.cbxtblBank_Kode_Pos);
            this.mTabPage_General.Controls.Add(this.metroLabel10);
            this.mTabPage_General.Controls.Add(this.cbxtblBank_Desa_Kelurahan);
            this.mTabPage_General.Controls.Add(this.metroLabel9);
            this.mTabPage_General.Controls.Add(this.cbxtblBank_Kecamatan);
            this.mTabPage_General.Controls.Add(this.metroLabel8);
            this.mTabPage_General.Controls.Add(this.cbxtblBank_Provinsi);
            this.mTabPage_General.Controls.Add(this.metroLabel7);
            this.mTabPage_General.Controls.Add(this.cbxtblBank_Kota_Kabupaten);
            this.mTabPage_General.Controls.Add(this.cbxtblBank_Group);
            this.mTabPage_General.Controls.Add(this.metroLabel6);
            this.mTabPage_General.Controls.Add(this.txttblBank_Alamat2);
            this.mTabPage_General.Controls.Add(this.txttblBank_Alamat1);
            this.mTabPage_General.Controls.Add(this.metroLabel5);
            this.mTabPage_General.Controls.Add(this.txttblBank_Atas_Nama);
            this.mTabPage_General.Controls.Add(this.metroLabel4);
            this.mTabPage_General.Controls.Add(this.txttblBank_No_Rek);
            this.mTabPage_General.Controls.Add(this.metroLabel2);
            this.mTabPage_General.Controls.Add(this.txttblBank_Group_Nama);
            this.mTabPage_General.Controls.Add(this.metroLabel1);
            this.mTabPage_General.Controls.Add(this.txttblBank_Kode_Bank);
            this.mTabPage_General.Controls.Add(this.metroLabel3);
            this.mTabPage_General.HorizontalScrollbarBarColor = true;
            this.mTabPage_General.HorizontalScrollbarHighlightOnWheel = false;
            this.mTabPage_General.HorizontalScrollbarSize = 3;
            this.mTabPage_General.Location = new System.Drawing.Point(4, 41);
            this.mTabPage_General.Name = "mTabPage_General";
            this.mTabPage_General.Size = new System.Drawing.Size(635, 371);
            this.mTabPage_General.TabIndex = 0;
            this.mTabPage_General.Text = "General";
            this.mTabPage_General.VerticalScrollbarBarColor = true;
            this.mTabPage_General.VerticalScrollbarHighlightOnWheel = false;
            this.mTabPage_General.VerticalScrollbarSize = 2;
            // 
            // txttblBank_Cabang
            // 
            this.txttblBank_Cabang.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Cabang.Location = new System.Drawing.Point(126, 124);
            this.txttblBank_Cabang.MaxLength = 50;
            this.txttblBank_Cabang.Name = "txttblBank_Cabang";
            this.txttblBank_Cabang.Size = new System.Drawing.Size(503, 23);
            this.txttblBank_Cabang.TabIndex = 6;
            // 
            // metroLabel11
            // 
            this.metroLabel11.AutoSize = true;
            this.metroLabel11.Location = new System.Drawing.Point(7, 126);
            this.metroLabel11.Name = "metroLabel11";
            this.metroLabel11.Size = new System.Drawing.Size(55, 19);
            this.metroLabel11.TabIndex = 69;
            this.metroLabel11.Text = "Cabang";
            // 
            // cbxtblBank_Kode_Pos
            // 
            this.cbxtblBank_Kode_Pos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxtblBank_Kode_Pos.FormattingEnabled = true;
            this.cbxtblBank_Kode_Pos.Location = new System.Drawing.Point(126, 324);
            this.cbxtblBank_Kode_Pos.Name = "cbxtblBank_Kode_Pos";
            this.cbxtblBank_Kode_Pos.Size = new System.Drawing.Size(78, 24);
            this.cbxtblBank_Kode_Pos.TabIndex = 13;
            this.cbxtblBank_Kode_Pos.SelectedIndexChanged += new System.EventHandler(this.cbxtblBank_Kode_Pos_SelectedIndexChanged);
            // 
            // metroLabel10
            // 
            this.metroLabel10.AutoSize = true;
            this.metroLabel10.Location = new System.Drawing.Point(7, 327);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(64, 19);
            this.metroLabel10.TabIndex = 66;
            this.metroLabel10.Text = "Kode Pos";
            // 
            // cbxtblBank_Desa_Kelurahan
            // 
            this.cbxtblBank_Desa_Kelurahan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxtblBank_Desa_Kelurahan.FormattingEnabled = true;
            this.cbxtblBank_Desa_Kelurahan.Items.AddRange(new object[] {
            " "});
            this.cbxtblBank_Desa_Kelurahan.Location = new System.Drawing.Point(126, 295);
            this.cbxtblBank_Desa_Kelurahan.Name = "cbxtblBank_Desa_Kelurahan";
            this.cbxtblBank_Desa_Kelurahan.Size = new System.Drawing.Size(264, 24);
            this.cbxtblBank_Desa_Kelurahan.TabIndex = 12;
            this.cbxtblBank_Desa_Kelurahan.SelectedIndexChanged += new System.EventHandler(this.cbxtblBank_Desa_Kelurahan_SelectedIndexChanged);
            // 
            // metroLabel9
            // 
            this.metroLabel9.AutoSize = true;
            this.metroLabel9.Location = new System.Drawing.Point(7, 298);
            this.metroLabel9.Name = "metroLabel9";
            this.metroLabel9.Size = new System.Drawing.Size(107, 19);
            this.metroLabel9.TabIndex = 64;
            this.metroLabel9.Text = "Desa / Kelurahan";
            // 
            // cbxtblBank_Kecamatan
            // 
            this.cbxtblBank_Kecamatan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxtblBank_Kecamatan.FormattingEnabled = true;
            this.cbxtblBank_Kecamatan.Items.AddRange(new object[] {
            " "});
            this.cbxtblBank_Kecamatan.Location = new System.Drawing.Point(126, 266);
            this.cbxtblBank_Kecamatan.Name = "cbxtblBank_Kecamatan";
            this.cbxtblBank_Kecamatan.Size = new System.Drawing.Size(264, 24);
            this.cbxtblBank_Kecamatan.TabIndex = 11;
            this.cbxtblBank_Kecamatan.SelectedIndexChanged += new System.EventHandler(this.cbxtblBank_Kecamatan_SelectedIndexChanged);
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.Location = new System.Drawing.Point(7, 269);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(73, 19);
            this.metroLabel8.TabIndex = 62;
            this.metroLabel8.Text = "Kecamatan";
            // 
            // cbxtblBank_Provinsi
            // 
            this.cbxtblBank_Provinsi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxtblBank_Provinsi.FormattingEnabled = true;
            this.cbxtblBank_Provinsi.Items.AddRange(new object[] {
            " "});
            this.cbxtblBank_Provinsi.Location = new System.Drawing.Point(126, 208);
            this.cbxtblBank_Provinsi.Name = "cbxtblBank_Provinsi";
            this.cbxtblBank_Provinsi.Size = new System.Drawing.Size(264, 24);
            this.cbxtblBank_Provinsi.TabIndex = 9;
            this.cbxtblBank_Provinsi.SelectedIndexChanged += new System.EventHandler(this.cbxtblBank_Provinsi_SelectedIndexChanged);
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.Location = new System.Drawing.Point(7, 211);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(54, 19);
            this.metroLabel7.TabIndex = 60;
            this.metroLabel7.Text = "Provinsi";
            // 
            // cbxtblBank_Kota_Kabupaten
            // 
            this.cbxtblBank_Kota_Kabupaten.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxtblBank_Kota_Kabupaten.FormattingEnabled = true;
            this.cbxtblBank_Kota_Kabupaten.Items.AddRange(new object[] {
            " "});
            this.cbxtblBank_Kota_Kabupaten.Location = new System.Drawing.Point(126, 237);
            this.cbxtblBank_Kota_Kabupaten.Name = "cbxtblBank_Kota_Kabupaten";
            this.cbxtblBank_Kota_Kabupaten.Size = new System.Drawing.Size(264, 24);
            this.cbxtblBank_Kota_Kabupaten.TabIndex = 10;
            this.cbxtblBank_Kota_Kabupaten.SelectedIndexChanged += new System.EventHandler(this.cbxtblBank_Kota_Kabupaten_SelectedIndexChanged);
            // 
            // cbxtblBank_Group
            // 
            this.cbxtblBank_Group.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxtblBank_Group.FormattingEnabled = true;
            this.cbxtblBank_Group.Location = new System.Drawing.Point(126, 39);
            this.cbxtblBank_Group.Name = "cbxtblBank_Group";
            this.cbxtblBank_Group.Size = new System.Drawing.Size(78, 24);
            this.cbxtblBank_Group.TabIndex = 2;
            this.cbxtblBank_Group.SelectedIndexChanged += new System.EventHandler(this.cbxtblBank_Group_SelectedIndexChanged);
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(7, 240);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(110, 19);
            this.metroLabel6.TabIndex = 57;
            this.metroLabel6.Text = "Kota / Kabupaten";
            // 
            // txttblBank_Alamat2
            // 
            this.txttblBank_Alamat2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Alamat2.Location = new System.Drawing.Point(126, 180);
            this.txttblBank_Alamat2.MaxLength = 50;
            this.txttblBank_Alamat2.Name = "txttblBank_Alamat2";
            this.txttblBank_Alamat2.Size = new System.Drawing.Size(503, 23);
            this.txttblBank_Alamat2.TabIndex = 8;
            // 
            // txttblBank_Alamat1
            // 
            this.txttblBank_Alamat1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Alamat1.Location = new System.Drawing.Point(126, 152);
            this.txttblBank_Alamat1.MaxLength = 50;
            this.txttblBank_Alamat1.Name = "txttblBank_Alamat1";
            this.txttblBank_Alamat1.Size = new System.Drawing.Size(503, 23);
            this.txttblBank_Alamat1.TabIndex = 7;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(7, 154);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(51, 19);
            this.metroLabel5.TabIndex = 55;
            this.metroLabel5.Text = "Alamat";
            // 
            // txttblBank_Atas_Nama
            // 
            this.txttblBank_Atas_Nama.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Atas_Nama.Location = new System.Drawing.Point(126, 96);
            this.txttblBank_Atas_Nama.MaxLength = 50;
            this.txttblBank_Atas_Nama.Name = "txttblBank_Atas_Nama";
            this.txttblBank_Atas_Nama.Size = new System.Drawing.Size(503, 23);
            this.txttblBank_Atas_Nama.TabIndex = 5;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(7, 98);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(74, 19);
            this.metroLabel4.TabIndex = 53;
            this.metroLabel4.Text = "Atas Nama";
            // 
            // txttblBank_No_Rek
            // 
            this.txttblBank_No_Rek.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_No_Rek.Location = new System.Drawing.Point(126, 68);
            this.txttblBank_No_Rek.MaxLength = 25;
            this.txttblBank_No_Rek.Name = "txttblBank_No_Rek";
            this.txttblBank_No_Rek.Size = new System.Drawing.Size(264, 23);
            this.txttblBank_No_Rek.TabIndex = 4;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(7, 70);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(87, 19);
            this.metroLabel2.TabIndex = 51;
            this.metroLabel2.Text = "No. Rekening";
            // 
            // txttblBank_Group_Nama
            // 
            this.txttblBank_Group_Nama.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Group_Nama.Location = new System.Drawing.Point(207, 40);
            this.txttblBank_Group_Nama.MaxLength = 50;
            this.txttblBank_Group_Nama.Name = "txttblBank_Group_Nama";
            this.txttblBank_Group_Nama.ReadOnly = true;
            this.txttblBank_Group_Nama.Size = new System.Drawing.Size(422, 23);
            this.txttblBank_Group_Nama.TabIndex = 3;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(7, 42);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(77, 19);
            this.metroLabel1.TabIndex = 49;
            this.metroLabel1.Text = "Nama Bank";
            // 
            // txttblBank_Kode_Bank
            // 
            this.txttblBank_Kode_Bank.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Kode_Bank.Location = new System.Drawing.Point(126, 11);
            this.txttblBank_Kode_Bank.MaxLength = 5;
            this.txttblBank_Kode_Bank.Name = "txttblBank_Kode_Bank";
            this.txttblBank_Kode_Bank.ReadOnly = true;
            this.txttblBank_Kode_Bank.Size = new System.Drawing.Size(78, 23);
            this.txttblBank_Kode_Bank.TabIndex = 1;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(7, 13);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(71, 19);
            this.metroLabel3.TabIndex = 47;
            this.metroLabel3.Text = "Kode Bank";
            // 
            // mTabPage_Contact
            // 
            this.mTabPage_Contact.Controls.Add(this.mPanel_Contact);
            this.mTabPage_Contact.HorizontalScrollbarBarColor = true;
            this.mTabPage_Contact.HorizontalScrollbarHighlightOnWheel = false;
            this.mTabPage_Contact.HorizontalScrollbarSize = 3;
            this.mTabPage_Contact.Location = new System.Drawing.Point(4, 41);
            this.mTabPage_Contact.Name = "mTabPage_Contact";
            this.mTabPage_Contact.Size = new System.Drawing.Size(635, 371);
            this.mTabPage_Contact.TabIndex = 1;
            this.mTabPage_Contact.Text = "| Contact";
            this.mTabPage_Contact.VerticalScrollbarBarColor = true;
            this.mTabPage_Contact.VerticalScrollbarHighlightOnWheel = false;
            this.mTabPage_Contact.VerticalScrollbarSize = 2;
            // 
            // mPanel_Contact
            // 
            this.mPanel_Contact.Controls.Add(this.DtGridView_Contact);
            this.mPanel_Contact.Controls.Add(this.mPanel_Ct_Bottom);
            this.mPanel_Contact.Dock = System.Windows.Forms.DockStyle.Top;
            this.mPanel_Contact.HorizontalScrollbarBarColor = true;
            this.mPanel_Contact.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_Contact.HorizontalScrollbarSize = 6;
            this.mPanel_Contact.Location = new System.Drawing.Point(0, 0);
            this.mPanel_Contact.Name = "mPanel_Contact";
            this.mPanel_Contact.Size = new System.Drawing.Size(635, 236);
            this.mPanel_Contact.TabIndex = 2;
            this.mPanel_Contact.VerticalScrollbarBarColor = true;
            this.mPanel_Contact.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Contact.VerticalScrollbarSize = 6;
            // 
            // DtGridView_Contact
            // 
            this.DtGridView_Contact.AllowUserToAddRows = false;
            this.DtGridView_Contact.AllowUserToDeleteRows = false;
            this.DtGridView_Contact.AllowUserToOrderColumns = true;
            this.DtGridView_Contact.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DtGridView_Contact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DtGridView_Contact.Location = new System.Drawing.Point(0, 0);
            this.DtGridView_Contact.Name = "DtGridView_Contact";
            this.DtGridView_Contact.ReadOnly = true;
            this.DtGridView_Contact.Size = new System.Drawing.Size(635, 207);
            this.DtGridView_Contact.TabIndex = 3;
            // 
            // mPanel_Ct_Bottom
            // 
            this.mPanel_Ct_Bottom.Controls.Add(this.btnDelete_Ct);
            this.mPanel_Ct_Bottom.Controls.Add(this.btnEdit_Ct);
            this.mPanel_Ct_Bottom.Controls.Add(this.btnNew_Ct);
            this.mPanel_Ct_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mPanel_Ct_Bottom.HorizontalScrollbarBarColor = true;
            this.mPanel_Ct_Bottom.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_Ct_Bottom.HorizontalScrollbarSize = 6;
            this.mPanel_Ct_Bottom.Location = new System.Drawing.Point(0, 207);
            this.mPanel_Ct_Bottom.Name = "mPanel_Ct_Bottom";
            this.mPanel_Ct_Bottom.Size = new System.Drawing.Size(635, 29);
            this.mPanel_Ct_Bottom.TabIndex = 2;
            this.mPanel_Ct_Bottom.VerticalScrollbarBarColor = true;
            this.mPanel_Ct_Bottom.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Ct_Bottom.VerticalScrollbarSize = 6;
            // 
            // btnDelete_Ct
            // 
            this.btnDelete_Ct.Location = new System.Drawing.Point(167, 3);
            this.btnDelete_Ct.Name = "btnDelete_Ct";
            this.btnDelete_Ct.Size = new System.Drawing.Size(75, 23);
            this.btnDelete_Ct.TabIndex = 11;
            this.btnDelete_Ct.Text = "&Delete";
            this.btnDelete_Ct.UseVisualStyleBackColor = true;
            this.btnDelete_Ct.Click += new System.EventHandler(this.btnDelete_Ct_Click);
            // 
            // btnEdit_Ct
            // 
            this.btnEdit_Ct.Location = new System.Drawing.Point(86, 3);
            this.btnEdit_Ct.Name = "btnEdit_Ct";
            this.btnEdit_Ct.Size = new System.Drawing.Size(75, 23);
            this.btnEdit_Ct.TabIndex = 10;
            this.btnEdit_Ct.Text = "&Edit";
            this.btnEdit_Ct.UseVisualStyleBackColor = true;
            this.btnEdit_Ct.Click += new System.EventHandler(this.btnEdit_Ct_Click);
            // 
            // btnNew_Ct
            // 
            this.btnNew_Ct.Location = new System.Drawing.Point(5, 3);
            this.btnNew_Ct.Name = "btnNew_Ct";
            this.btnNew_Ct.Size = new System.Drawing.Size(75, 23);
            this.btnNew_Ct.TabIndex = 9;
            this.btnNew_Ct.Text = "&New";
            this.btnNew_Ct.UseVisualStyleBackColor = true;
            this.btnNew_Ct.Click += new System.EventHandler(this.btnNew_Ct_Click);
            // 
            // mTabPage_Notes
            // 
            this.mTabPage_Notes.Controls.Add(this.txttblBank_Ket);
            this.mTabPage_Notes.Controls.Add(this.metroLabel12);
            this.mTabPage_Notes.HorizontalScrollbarBarColor = true;
            this.mTabPage_Notes.HorizontalScrollbarHighlightOnWheel = false;
            this.mTabPage_Notes.HorizontalScrollbarSize = 8;
            this.mTabPage_Notes.Location = new System.Drawing.Point(4, 41);
            this.mTabPage_Notes.Name = "mTabPage_Notes";
            this.mTabPage_Notes.Size = new System.Drawing.Size(635, 371);
            this.mTabPage_Notes.TabIndex = 2;
            this.mTabPage_Notes.Text = "| Notes";
            this.mTabPage_Notes.VerticalScrollbarBarColor = true;
            this.mTabPage_Notes.VerticalScrollbarHighlightOnWheel = false;
            this.mTabPage_Notes.VerticalScrollbarSize = 8;
            // 
            // txttblBank_Ket
            // 
            this.txttblBank_Ket.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Ket.Location = new System.Drawing.Point(111, 30);
            this.txttblBank_Ket.MaxLength = 50;
            this.txttblBank_Ket.Multiline = true;
            this.txttblBank_Ket.Name = "txttblBank_Ket";
            this.txttblBank_Ket.Size = new System.Drawing.Size(521, 267);
            this.txttblBank_Ket.TabIndex = 71;
            // 
            // metroLabel12
            // 
            this.metroLabel12.AutoSize = true;
            this.metroLabel12.Location = new System.Drawing.Point(10, 30);
            this.metroLabel12.Name = "metroLabel12";
            this.metroLabel12.Size = new System.Drawing.Size(75, 19);
            this.metroLabel12.TabIndex = 72;
            this.metroLabel12.Text = "Keterangan";
            // 
            // FrmM_Bank
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 528);
            this.Controls.Add(this.mPanel_Body);
            this.Controls.Add(this.mPanel_Bottom);
            this.Font = new System.Drawing.Font("Courier New", 10F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmM_Bank";
            this.Resizable = false;
            this.Text = "Master Bank";
            this.Load += new System.EventHandler(this.FrmM_Bank_Load);
            this.Shown += new System.EventHandler(this.FrmM_Bank_Shown);
            this.mPanel_Bottom.ResumeLayout(false);
            this.mPanel_Body.ResumeLayout(false);
            this.mTabCtrl_Bank.ResumeLayout(false);
            this.mTabPage_General.ResumeLayout(false);
            this.mTabPage_General.PerformLayout();
            this.mTabPage_Contact.ResumeLayout(false);
            this.mPanel_Contact.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DtGridView_Contact)).EndInit();
            this.mPanel_Ct_Bottom.ResumeLayout(false);
            this.mTabPage_Notes.ResumeLayout(false);
            this.mTabPage_Notes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel mPanel_Bottom;
        private MetroFramework.Controls.MetroPanel mPanel_Body;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnEdit;
        private MetroFramework.Controls.MetroTabControl mTabCtrl_Bank;
        private MetroFramework.Controls.MetroTabPage mTabPage_General;
        private System.Windows.Forms.TextBox txttblBank_Cabang;
        private MetroFramework.Controls.MetroLabel metroLabel11;
        private System.Windows.Forms.ComboBox cbxtblBank_Kode_Pos;
        private MetroFramework.Controls.MetroLabel metroLabel10;
        private System.Windows.Forms.ComboBox cbxtblBank_Desa_Kelurahan;
        private MetroFramework.Controls.MetroLabel metroLabel9;
        private System.Windows.Forms.ComboBox cbxtblBank_Kecamatan;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private System.Windows.Forms.ComboBox cbxtblBank_Provinsi;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private System.Windows.Forms.ComboBox cbxtblBank_Kota_Kabupaten;
        private System.Windows.Forms.ComboBox cbxtblBank_Group;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private System.Windows.Forms.TextBox txttblBank_Alamat2;
        private System.Windows.Forms.TextBox txttblBank_Alamat1;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private System.Windows.Forms.TextBox txttblBank_Atas_Nama;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private System.Windows.Forms.TextBox txttblBank_No_Rek;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private System.Windows.Forms.TextBox txttblBank_Group_Nama;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.TextBox txttblBank_Kode_Bank;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroTabPage mTabPage_Contact;
        private MetroFramework.Controls.MetroPanel mPanel_Contact;
        private System.Windows.Forms.DataGridView DtGridView_Contact;
        private MetroFramework.Controls.MetroPanel mPanel_Ct_Bottom;
        private System.Windows.Forms.Button btnDelete_Ct;
        private System.Windows.Forms.Button btnEdit_Ct;
        private System.Windows.Forms.Button btnNew_Ct;
        private MetroFramework.Controls.MetroTabPage mTabPage_Notes;
        private System.Windows.Forms.TextBox txttblBank_Ket;
        private MetroFramework.Controls.MetroLabel metroLabel12;
    }
}