namespace ISBS_New.Purchase.NotaDebet
{
    partial class FrmT_NotaDebet
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvDetail = new System.Windows.Forms.DataGridView();
            this.TabControl = new MetroFramework.Controls.MetroTabControl();
            this.TabPage_General = new MetroFramework.Controls.MetroTabPage();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSearchKurs = new System.Windows.Forms.Button();
            this.btnSearchAcc = new System.Windows.Forms.Button();
            this.DueDate = new System.Windows.Forms.DateTimePicker();
            this.metroLabel15 = new MetroFramework.Controls.MetroLabel();
            this.txtPPN = new System.Windows.Forms.TextBox();
            this.metroLabel12 = new MetroFramework.Controls.MetroLabel();
            this.txtPPH = new System.Windows.Forms.TextBox();
            this.txtKurs = new System.Windows.Forms.TextBox();
            this.txtMataUang = new System.Windows.Forms.TextBox();
            this.txtAccName = new System.Windows.Forms.TextBox();
            this.txtAccId = new System.Windows.Forms.TextBox();
            this.DNDate = new System.Windows.Forms.DateTimePicker();
            this.txtDNNo = new System.Windows.Forms.TextBox();
            this.metroLabel14 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel13 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel11 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel10 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel9 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.TabPage_Tax = new MetroFramework.Controls.MetroTabPage();
            this.txtTaxAddess = new System.Windows.Forms.TextBox();
            this.txtTaxName = new System.Windows.Forms.TextBox();
            this.txtTaxNumber = new System.Windows.Forms.TextBox();
            this.dateTax = new System.Windows.Forms.DateTimePicker();
            this.txtNPWP = new System.Windows.Forms.TextBox();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.Label20z = new System.Windows.Forms.Label();
            this.BtnSaveHeader_NotaDebet = new System.Windows.Forms.Button();
            this.BtnExitHeader_NotaDebet = new System.Windows.Forms.Button();
            this.txttblDOPabrikH_Ket1 = new System.Windows.Forms.TextBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.BtnCancelHeader_NotaDebet = new System.Windows.Forms.Button();
            this.BtnEditHeader_NotaDebet = new System.Windows.Forms.Button();
            this.txtTotalNett = new System.Windows.Forms.TextBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.txtPPHbwh = new System.Windows.Forms.TextBox();
            this.txtPPNbwh = new System.Windows.Forms.TextBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetail)).BeginInit();
            this.TabControl.SuspendLayout();
            this.TabPage_General.SuspendLayout();
            this.TabPage_Tax.SuspendLayout();
            this.metroPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.dgvDetail, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TabControl, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.metroPanel1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(27, 74);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(849, 555);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dgvDetail
            // 
            this.dgvDetail.AllowUserToAddRows = false;
            this.dgvDetail.AllowUserToDeleteRows = false;
            this.dgvDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetail.Location = new System.Drawing.Point(3, 225);
            this.dgvDetail.Name = "dgvDetail";
            this.dgvDetail.ReadOnly = true;
            this.dgvDetail.Size = new System.Drawing.Size(843, 160);
            this.dgvDetail.TabIndex = 0;
            this.dgvDetail.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvDetail_CellMouseDown);
            this.dgvDetail.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvDetail_RowsAdded);
            this.dgvDetail.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvDetail_RowsRemoved);
            // 
            // TabControl
            // 
            this.TabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TabControl.Controls.Add(this.TabPage_General);
            this.TabControl.Controls.Add(this.TabPage_Tax);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.Location = new System.Drawing.Point(3, 3);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(843, 216);
            this.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControl.TabIndex = 1;
            this.TabControl.UseSelectable = true;
            // 
            // TabPage_General
            // 
            this.TabPage_General.Controls.Add(this.btnDelete);
            this.TabPage_General.Controls.Add(this.btnNew);
            this.TabPage_General.Controls.Add(this.btnSearchKurs);
            this.TabPage_General.Controls.Add(this.btnSearchAcc);
            this.TabPage_General.Controls.Add(this.DueDate);
            this.TabPage_General.Controls.Add(this.metroLabel15);
            this.TabPage_General.Controls.Add(this.txtPPN);
            this.TabPage_General.Controls.Add(this.metroLabel12);
            this.TabPage_General.Controls.Add(this.txtPPH);
            this.TabPage_General.Controls.Add(this.txtKurs);
            this.TabPage_General.Controls.Add(this.txtMataUang);
            this.TabPage_General.Controls.Add(this.txtAccName);
            this.TabPage_General.Controls.Add(this.txtAccId);
            this.TabPage_General.Controls.Add(this.DNDate);
            this.TabPage_General.Controls.Add(this.txtDNNo);
            this.TabPage_General.Controls.Add(this.metroLabel14);
            this.TabPage_General.Controls.Add(this.metroLabel13);
            this.TabPage_General.Controls.Add(this.metroLabel11);
            this.TabPage_General.Controls.Add(this.metroLabel10);
            this.TabPage_General.Controls.Add(this.metroLabel9);
            this.TabPage_General.Controls.Add(this.metroLabel7);
            this.TabPage_General.Controls.Add(this.metroLabel6);
            this.TabPage_General.HorizontalScrollbarBarColor = true;
            this.TabPage_General.HorizontalScrollbarHighlightOnWheel = false;
            this.TabPage_General.HorizontalScrollbarSize = 2;
            this.TabPage_General.Location = new System.Drawing.Point(4, 41);
            this.TabPage_General.Name = "TabPage_General";
            this.TabPage_General.Size = new System.Drawing.Size(835, 171);
            this.TabPage_General.TabIndex = 0;
            this.TabPage_General.Text = "GENERAL";
            this.TabPage_General.VerticalScrollbarBarColor = true;
            this.TabPage_General.VerticalScrollbarHighlightOnWheel = false;
            this.TabPage_General.VerticalScrollbarSize = 2;
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(87, 148);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 119;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.Enabled = false;
            this.btnNew.Location = new System.Drawing.Point(8, 148);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 118;
            this.btnNew.Text = "&New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSearchKurs
            // 
            this.btnSearchKurs.AutoSize = true;
            this.btnSearchKurs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchKurs.Enabled = false;
            this.btnSearchKurs.Location = new System.Drawing.Point(790, 115);
            this.btnSearchKurs.Name = "btnSearchKurs";
            this.btnSearchKurs.Size = new System.Drawing.Size(42, 26);
            this.btnSearchKurs.TabIndex = 117;
            this.btnSearchKurs.Text = "...";
            this.btnSearchKurs.UseVisualStyleBackColor = true;
            this.btnSearchKurs.Click += new System.EventHandler(this.btnSearchKurs_Click);
            // 
            // btnSearchAcc
            // 
            this.btnSearchAcc.AutoSize = true;
            this.btnSearchAcc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchAcc.Enabled = false;
            this.btnSearchAcc.Location = new System.Drawing.Point(537, 14);
            this.btnSearchAcc.Name = "btnSearchAcc";
            this.btnSearchAcc.Size = new System.Drawing.Size(42, 26);
            this.btnSearchAcc.TabIndex = 116;
            this.btnSearchAcc.Text = "...";
            this.btnSearchAcc.UseVisualStyleBackColor = true;
            this.btnSearchAcc.Click += new System.EventHandler(this.btnSearchAcc_Click);
            // 
            // DueDate
            // 
            this.DueDate.CustomFormat = "dd/MM/yyyy";
            this.DueDate.Enabled = false;
            this.DueDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DueDate.Location = new System.Drawing.Point(87, 70);
            this.DueDate.Name = "DueDate";
            this.DueDate.Size = new System.Drawing.Size(150, 22);
            this.DueDate.TabIndex = 115;
            this.DueDate.Value = new System.DateTime(2018, 4, 23, 0, 0, 0, 0);
            // 
            // metroLabel15
            // 
            this.metroLabel15.AutoSize = true;
            this.metroLabel15.Location = new System.Drawing.Point(3, 72);
            this.metroLabel15.Name = "metroLabel15";
            this.metroLabel15.Size = new System.Drawing.Size(63, 19);
            this.metroLabel15.TabIndex = 114;
            this.metroLabel15.Text = "Due Date";
            // 
            // txtPPN
            // 
            this.txtPPN.Enabled = false;
            this.txtPPN.Location = new System.Drawing.Point(731, 67);
            this.txtPPN.MaxLength = 5;
            this.txtPPN.Name = "txtPPN";
            this.txtPPN.Size = new System.Drawing.Size(50, 22);
            this.txtPPN.TabIndex = 113;
            this.txtPPN.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPPN.TextChanged += new System.EventHandler(this.txtPPN_TextChanged);
            this.txtPPN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPPN_KeyPress);
            // 
            // metroLabel12
            // 
            this.metroLabel12.AutoSize = true;
            this.metroLabel12.Location = new System.Drawing.Point(656, 69);
            this.metroLabel12.Name = "metroLabel12";
            this.metroLabel12.Size = new System.Drawing.Size(35, 19);
            this.metroLabel12.TabIndex = 112;
            this.metroLabel12.Text = "PPN";
            // 
            // txtPPH
            // 
            this.txtPPH.Enabled = false;
            this.txtPPH.Location = new System.Drawing.Point(731, 92);
            this.txtPPH.MaxLength = 5;
            this.txtPPH.Name = "txtPPH";
            this.txtPPH.Size = new System.Drawing.Size(50, 22);
            this.txtPPH.TabIndex = 109;
            this.txtPPH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPPH.TextChanged += new System.EventHandler(this.txtPPH_TextChanged);
            this.txtPPH.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPPH_KeyPress);
            // 
            // txtKurs
            // 
            this.txtKurs.Enabled = false;
            this.txtKurs.Location = new System.Drawing.Point(731, 142);
            this.txtKurs.MaxLength = 15;
            this.txtKurs.Name = "txtKurs";
            this.txtKurs.Size = new System.Drawing.Size(100, 22);
            this.txtKurs.TabIndex = 111;
            this.txtKurs.Text = "0.00";
            this.txtKurs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtKurs.TextChanged += new System.EventHandler(this.txtKurs_TextChanged);
            this.txtKurs.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtKurs_KeyPress);
            // 
            // txtMataUang
            // 
            this.txtMataUang.Enabled = false;
            this.txtMataUang.Location = new System.Drawing.Point(731, 117);
            this.txtMataUang.MaxLength = 3;
            this.txtMataUang.Name = "txtMataUang";
            this.txtMataUang.Size = new System.Drawing.Size(53, 22);
            this.txtMataUang.TabIndex = 110;
            this.txtMataUang.TextChanged += new System.EventHandler(this.txtMataUang_TextChanged);
            // 
            // txtAccName
            // 
            this.txtAccName.Enabled = false;
            this.txtAccName.Location = new System.Drawing.Point(432, 42);
            this.txtAccName.Name = "txtAccName";
            this.txtAccName.Size = new System.Drawing.Size(400, 22);
            this.txtAccName.TabIndex = 108;
            this.txtAccName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtAccName_MouseDown);
            // 
            // txtAccId
            // 
            this.txtAccId.Enabled = false;
            this.txtAccId.Location = new System.Drawing.Point(432, 17);
            this.txtAccId.MaxLength = 8;
            this.txtAccId.Name = "txtAccId";
            this.txtAccId.Size = new System.Drawing.Size(100, 22);
            this.txtAccId.TabIndex = 105;
            this.txtAccId.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtAccId_MouseDown);
            // 
            // DNDate
            // 
            this.DNDate.CustomFormat = "dd/MM/yyyy";
            this.DNDate.Enabled = false;
            this.DNDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DNDate.Location = new System.Drawing.Point(87, 17);
            this.DNDate.Name = "DNDate";
            this.DNDate.Size = new System.Drawing.Size(150, 22);
            this.DNDate.TabIndex = 17;
            this.DNDate.Value = new System.DateTime(2018, 4, 23, 0, 0, 0, 0);
            // 
            // txtDNNo
            // 
            this.txtDNNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDNNo.Enabled = false;
            this.txtDNNo.Location = new System.Drawing.Point(87, 42);
            this.txtDNNo.MaxLength = 19;
            this.txtDNNo.Name = "txtDNNo";
            this.txtDNNo.Size = new System.Drawing.Size(200, 22);
            this.txtDNNo.TabIndex = 18;
            // 
            // metroLabel14
            // 
            this.metroLabel14.AutoSize = true;
            this.metroLabel14.Location = new System.Drawing.Point(656, 145);
            this.metroLabel14.Name = "metroLabel14";
            this.metroLabel14.Size = new System.Drawing.Size(33, 19);
            this.metroLabel14.TabIndex = 13;
            this.metroLabel14.Text = "Kurs";
            // 
            // metroLabel13
            // 
            this.metroLabel13.AutoSize = true;
            this.metroLabel13.Location = new System.Drawing.Point(656, 120);
            this.metroLabel13.Name = "metroLabel13";
            this.metroLabel13.Size = new System.Drawing.Size(72, 19);
            this.metroLabel13.TabIndex = 12;
            this.metroLabel13.Text = "Mata uang";
            // 
            // metroLabel11
            // 
            this.metroLabel11.AutoSize = true;
            this.metroLabel11.Location = new System.Drawing.Point(656, 94);
            this.metroLabel11.Name = "metroLabel11";
            this.metroLabel11.Size = new System.Drawing.Size(34, 19);
            this.metroLabel11.TabIndex = 10;
            this.metroLabel11.Text = "PPH";
            // 
            // metroLabel10
            // 
            this.metroLabel10.AutoSize = true;
            this.metroLabel10.Location = new System.Drawing.Point(330, 44);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(96, 19);
            this.metroLabel10.TabIndex = 9;
            this.metroLabel10.Text = "Account Name";
            // 
            // metroLabel9
            // 
            this.metroLabel9.AutoSize = true;
            this.metroLabel9.Location = new System.Drawing.Point(355, 19);
            this.metroLabel9.Name = "metroLabel9";
            this.metroLabel9.Size = new System.Drawing.Size(71, 19);
            this.metroLabel9.TabIndex = 8;
            this.metroLabel9.Text = "Account Id";
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.Location = new System.Drawing.Point(3, 44);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(50, 19);
            this.metroLabel7.TabIndex = 6;
            this.metroLabel7.Text = "DN No";
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(3, 19);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(59, 19);
            this.metroLabel6.TabIndex = 5;
            this.metroLabel6.Text = "DN Date";
            // 
            // TabPage_Tax
            // 
            this.TabPage_Tax.Controls.Add(this.txtTaxAddess);
            this.TabPage_Tax.Controls.Add(this.txtTaxName);
            this.TabPage_Tax.Controls.Add(this.txtTaxNumber);
            this.TabPage_Tax.Controls.Add(this.dateTax);
            this.TabPage_Tax.Controls.Add(this.txtNPWP);
            this.TabPage_Tax.Controls.Add(this.metroLabel5);
            this.TabPage_Tax.Controls.Add(this.metroLabel4);
            this.TabPage_Tax.Controls.Add(this.metroLabel3);
            this.TabPage_Tax.Controls.Add(this.metroLabel2);
            this.TabPage_Tax.Controls.Add(this.metroLabel1);
            this.TabPage_Tax.HorizontalScrollbarBarColor = true;
            this.TabPage_Tax.HorizontalScrollbarHighlightOnWheel = false;
            this.TabPage_Tax.HorizontalScrollbarSize = 2;
            this.TabPage_Tax.Location = new System.Drawing.Point(4, 41);
            this.TabPage_Tax.Name = "TabPage_Tax";
            this.TabPage_Tax.Size = new System.Drawing.Size(835, 171);
            this.TabPage_Tax.TabIndex = 1;
            this.TabPage_Tax.Text = "TAX";
            this.TabPage_Tax.VerticalScrollbarBarColor = true;
            this.TabPage_Tax.VerticalScrollbarHighlightOnWheel = false;
            this.TabPage_Tax.VerticalScrollbarSize = 2;
            // 
            // txtTaxAddess
            // 
            this.txtTaxAddess.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTaxAddess.Location = new System.Drawing.Point(104, 130);
            this.txtTaxAddess.MaxLength = 19;
            this.txtTaxAddess.Name = "txtTaxAddess";
            this.txtTaxAddess.Size = new System.Drawing.Size(200, 22);
            this.txtTaxAddess.TabIndex = 23;
            this.txtTaxAddess.TabStop = false;
            // 
            // txtTaxName
            // 
            this.txtTaxName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTaxName.Location = new System.Drawing.Point(104, 102);
            this.txtTaxName.MaxLength = 19;
            this.txtTaxName.Name = "txtTaxName";
            this.txtTaxName.Size = new System.Drawing.Size(200, 22);
            this.txtTaxName.TabIndex = 22;
            this.txtTaxName.TabStop = false;
            // 
            // txtTaxNumber
            // 
            this.txtTaxNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTaxNumber.Location = new System.Drawing.Point(104, 74);
            this.txtTaxNumber.MaxLength = 19;
            this.txtTaxNumber.Name = "txtTaxNumber";
            this.txtTaxNumber.Size = new System.Drawing.Size(200, 22);
            this.txtTaxNumber.TabIndex = 21;
            this.txtTaxNumber.TabStop = false;
            // 
            // dateTax
            // 
            this.dateTax.CustomFormat = "dd/MM/yyyy";
            this.dateTax.Enabled = false;
            this.dateTax.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTax.Location = new System.Drawing.Point(104, 46);
            this.dateTax.Name = "dateTax";
            this.dateTax.Size = new System.Drawing.Size(150, 22);
            this.dateTax.TabIndex = 19;
            this.dateTax.TabStop = false;
            this.dateTax.Value = new System.DateTime(2018, 4, 23, 0, 0, 0, 0);
            // 
            // txtNPWP
            // 
            this.txtNPWP.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNPWP.Enabled = false;
            this.txtNPWP.Location = new System.Drawing.Point(104, 18);
            this.txtNPWP.MaxLength = 19;
            this.txtNPWP.Name = "txtNPWP";
            this.txtNPWP.Size = new System.Drawing.Size(200, 22);
            this.txtNPWP.TabIndex = 20;
            this.txtNPWP.TabStop = false;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(3, 132);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(80, 19);
            this.metroLabel5.TabIndex = 6;
            this.metroLabel5.Text = "Tax Address";
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(3, 104);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(69, 19);
            this.metroLabel4.TabIndex = 5;
            this.metroLabel4.Text = "Tax Name";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(3, 20);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(48, 19);
            this.metroLabel3.TabIndex = 4;
            this.metroLabel3.Text = "NPWP";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(3, 76);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(82, 19);
            this.metroLabel2.TabIndex = 3;
            this.metroLabel2.Text = "Tax Number";
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(3, 48);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(60, 19);
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Text = "Tax Date";
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.Label20z);
            this.metroPanel1.Controls.Add(this.BtnSaveHeader_NotaDebet);
            this.metroPanel1.Controls.Add(this.BtnExitHeader_NotaDebet);
            this.metroPanel1.Controls.Add(this.txttblDOPabrikH_Ket1);
            this.metroPanel1.Controls.Add(this.Label14);
            this.metroPanel1.Controls.Add(this.BtnCancelHeader_NotaDebet);
            this.metroPanel1.Controls.Add(this.BtnEditHeader_NotaDebet);
            this.metroPanel1.Controls.Add(this.txtTotalNett);
            this.metroPanel1.Controls.Add(this.Label15);
            this.metroPanel1.Controls.Add(this.txtPPHbwh);
            this.metroPanel1.Controls.Add(this.txtPPNbwh);
            this.metroPanel1.Controls.Add(this.Label12);
            this.metroPanel1.Controls.Add(this.Label13);
            this.metroPanel1.Controls.Add(this.txtTotal);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(3, 391);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(843, 161);
            this.metroPanel1.TabIndex = 2;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // Label20z
            // 
            this.Label20z.AutoSize = true;
            this.Label20z.Location = new System.Drawing.Point(9, 39);
            this.Label20z.Name = "Label20z";
            this.Label20z.Size = new System.Drawing.Size(88, 16);
            this.Label20z.TabIndex = 117;
            this.Label20z.Text = "Keterangan";
            // 
            // BtnSaveHeader_NotaDebet
            // 
            this.BtnSaveHeader_NotaDebet.Enabled = false;
            this.BtnSaveHeader_NotaDebet.Location = new System.Drawing.Point(677, 130);
            this.BtnSaveHeader_NotaDebet.Name = "BtnSaveHeader_NotaDebet";
            this.BtnSaveHeader_NotaDebet.Size = new System.Drawing.Size(75, 23);
            this.BtnSaveHeader_NotaDebet.TabIndex = 114;
            this.BtnSaveHeader_NotaDebet.Text = "Sa&ve";
            this.BtnSaveHeader_NotaDebet.UseVisualStyleBackColor = true;
            this.BtnSaveHeader_NotaDebet.Click += new System.EventHandler(this.BtnSaveHeader_NotaDebet_Click);
            // 
            // BtnExitHeader_NotaDebet
            // 
            this.BtnExitHeader_NotaDebet.Enabled = false;
            this.BtnExitHeader_NotaDebet.Location = new System.Drawing.Point(757, 130);
            this.BtnExitHeader_NotaDebet.Name = "BtnExitHeader_NotaDebet";
            this.BtnExitHeader_NotaDebet.Size = new System.Drawing.Size(75, 23);
            this.BtnExitHeader_NotaDebet.TabIndex = 115;
            this.BtnExitHeader_NotaDebet.Text = "&Exit";
            this.BtnExitHeader_NotaDebet.UseVisualStyleBackColor = true;
            this.BtnExitHeader_NotaDebet.Click += new System.EventHandler(this.BtnExitHeader_NotaDebet_Click);
            // 
            // txttblDOPabrikH_Ket1
            // 
            this.txttblDOPabrikH_Ket1.Enabled = false;
            this.txttblDOPabrikH_Ket1.Location = new System.Drawing.Point(103, 39);
            this.txttblDOPabrikH_Ket1.MaxLength = 40;
            this.txttblDOPabrikH_Ket1.Multiline = true;
            this.txttblDOPabrikH_Ket1.Name = "txttblDOPabrikH_Ket1";
            this.txttblDOPabrikH_Ket1.Size = new System.Drawing.Size(305, 76);
            this.txttblDOPabrikH_Ket1.TabIndex = 116;
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Location = new System.Drawing.Point(614, 6);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(40, 16);
            this.Label14.TabIndex = 107;
            this.Label14.Text = "NETT";
            // 
            // BtnCancelHeader_NotaDebet
            // 
            this.BtnCancelHeader_NotaDebet.Enabled = false;
            this.BtnCancelHeader_NotaDebet.Location = new System.Drawing.Point(597, 130);
            this.BtnCancelHeader_NotaDebet.Name = "BtnCancelHeader_NotaDebet";
            this.BtnCancelHeader_NotaDebet.Size = new System.Drawing.Size(75, 23);
            this.BtnCancelHeader_NotaDebet.TabIndex = 113;
            this.BtnCancelHeader_NotaDebet.Text = "&Cancel";
            this.BtnCancelHeader_NotaDebet.UseVisualStyleBackColor = true;
            this.BtnCancelHeader_NotaDebet.Click += new System.EventHandler(this.BtnCancelHeader_NotaDebet_Click);
            // 
            // BtnEditHeader_NotaDebet
            // 
            this.BtnEditHeader_NotaDebet.Enabled = false;
            this.BtnEditHeader_NotaDebet.Location = new System.Drawing.Point(517, 130);
            this.BtnEditHeader_NotaDebet.Name = "BtnEditHeader_NotaDebet";
            this.BtnEditHeader_NotaDebet.Size = new System.Drawing.Size(75, 23);
            this.BtnEditHeader_NotaDebet.TabIndex = 112;
            this.BtnEditHeader_NotaDebet.Text = "&Edit";
            this.BtnEditHeader_NotaDebet.UseVisualStyleBackColor = true;
            this.BtnEditHeader_NotaDebet.Click += new System.EventHandler(this.BtnEditHeader_NotaDebet_Click);
            // 
            // txtTotalNett
            // 
            this.txtTotalNett.Enabled = false;
            this.txtTotalNett.Location = new System.Drawing.Point(655, 3);
            this.txtTotalNett.Name = "txtTotalNett";
            this.txtTotalNett.Size = new System.Drawing.Size(170, 22);
            this.txtTotalNett.TabIndex = 111;
            this.txtTotalNett.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Location = new System.Drawing.Point(422, 6);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(32, 16);
            this.Label15.TabIndex = 106;
            this.Label15.Text = "PPH";
            // 
            // txtPPHbwh
            // 
            this.txtPPHbwh.Enabled = false;
            this.txtPPHbwh.Location = new System.Drawing.Point(455, 3);
            this.txtPPHbwh.Name = "txtPPHbwh";
            this.txtPPHbwh.Size = new System.Drawing.Size(155, 22);
            this.txtPPHbwh.TabIndex = 110;
            this.txtPPHbwh.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtPPNbwh
            // 
            this.txtPPNbwh.Enabled = false;
            this.txtPPNbwh.Location = new System.Drawing.Point(265, 3);
            this.txtPPNbwh.Name = "txtPPNbwh";
            this.txtPPNbwh.Size = new System.Drawing.Size(155, 22);
            this.txtPPNbwh.TabIndex = 109;
            this.txtPPNbwh.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Location = new System.Drawing.Point(5, 6);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(48, 16);
            this.Label12.TabIndex = 104;
            this.Label12.Text = "Total";
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Location = new System.Drawing.Point(227, 6);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(32, 16);
            this.Label13.TabIndex = 105;
            this.Label13.Text = "PPN";
            // 
            // txtTotal
            // 
            this.txtTotal.Enabled = false;
            this.txtTotal.Location = new System.Drawing.Point(53, 3);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(170, 22);
            this.txtTotal.TabIndex = 108;
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // FrmT_NotaDebet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 654);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmT_NotaDebet";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Resizable = false;
            this.Text = "Nota Debet";
            this.Load += new System.EventHandler(this.FrmT_NotaDebet_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmT_NotaDebet_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetail)).EndInit();
            this.TabControl.ResumeLayout(false);
            this.TabPage_General.ResumeLayout(false);
            this.TabPage_General.PerformLayout();
            this.TabPage_Tax.ResumeLayout(false);
            this.TabPage_Tax.PerformLayout();
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgvDetail;
        private MetroFramework.Controls.MetroTabControl TabControl;
        private MetroFramework.Controls.MetroTabPage TabPage_General;
        private MetroFramework.Controls.MetroTabPage TabPage_Tax;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        internal System.Windows.Forms.TextBox txtAccName;
        internal System.Windows.Forms.TextBox txtAccId;
        internal System.Windows.Forms.DateTimePicker DNDate;
        internal System.Windows.Forms.TextBox txtDNNo;
        private MetroFramework.Controls.MetroLabel metroLabel14;
        private MetroFramework.Controls.MetroLabel metroLabel13;
        private MetroFramework.Controls.MetroLabel metroLabel11;
        private MetroFramework.Controls.MetroLabel metroLabel10;
        private MetroFramework.Controls.MetroLabel metroLabel9;
        internal System.Windows.Forms.TextBox txtTaxAddess;
        internal System.Windows.Forms.TextBox txtTaxName;
        internal System.Windows.Forms.TextBox txtTaxNumber;
        internal System.Windows.Forms.DateTimePicker dateTax;
        internal System.Windows.Forms.TextBox txtNPWP;
        internal System.Windows.Forms.TextBox txtPPN;
        private MetroFramework.Controls.MetroLabel metroLabel12;
        internal System.Windows.Forms.TextBox txtPPH;
        internal System.Windows.Forms.TextBox txtKurs;
        internal System.Windows.Forms.TextBox txtMataUang;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        internal System.Windows.Forms.Label Label14;
        internal System.Windows.Forms.TextBox txtTotalNett;
        internal System.Windows.Forms.Label Label15;
        internal System.Windows.Forms.TextBox txtPPHbwh;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.TextBox txtTotal;
        internal System.Windows.Forms.Label Label20z;
        internal System.Windows.Forms.TextBox txttblDOPabrikH_Ket1;
        internal System.Windows.Forms.Button BtnExitHeader_NotaDebet;
        internal System.Windows.Forms.Button BtnSaveHeader_NotaDebet;
        internal System.Windows.Forms.Button BtnCancelHeader_NotaDebet;
        internal System.Windows.Forms.Button BtnEditHeader_NotaDebet;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.TextBox txtPPNbwh;
        internal System.Windows.Forms.Button btnSearchKurs;
        internal System.Windows.Forms.Button btnSearchAcc;
        internal System.Windows.Forms.DateTimePicker DueDate;
        private MetroFramework.Controls.MetroLabel metroLabel15;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.Button btnNew;
    }
}