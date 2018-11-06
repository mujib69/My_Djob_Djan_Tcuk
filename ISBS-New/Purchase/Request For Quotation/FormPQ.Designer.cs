namespace ISBS_New.Purchase.PurchaseQuotation
{
    partial class FormPQ
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
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.dtVendorPqDate = new System.Windows.Forms.DateTimePicker();
            this.btnSearchPR = new System.Windows.Forms.Button();
            this.txtPrNumber = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbPaymentMode = new System.Windows.Forms.ComboBox();
            this.btnSearchVendor = new System.Windows.Forms.Button();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtVendorId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVendorPqNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtPqDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPqNumber = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvPqDetails = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupPRH = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.txtPPH = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtPPN = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtTermOfPayment = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtDeskripsi = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtTotalPPH = new System.Windows.Forms.TextBox();
            this.txtTotalPPN = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.txtCashBackScheme = new System.Windows.Forms.TextBox();
            this.txtBonusScheme = new System.Windows.Forms.TextBox();
            this.txtDiscScheme = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblForm = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.grpCustomer.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPqDetails)).BeginInit();
            this.groupPRH.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.dtVendorPqDate);
            this.grpCustomer.Controls.Add(this.btnSearchPR);
            this.grpCustomer.Controls.Add(this.txtPrNumber);
            this.grpCustomer.Controls.Add(this.label6);
            this.grpCustomer.Controls.Add(this.cmbPaymentMode);
            this.grpCustomer.Controls.Add(this.btnSearchVendor);
            this.grpCustomer.Controls.Add(this.txtVendorName);
            this.grpCustomer.Controls.Add(this.label15);
            this.grpCustomer.Controls.Add(this.label3);
            this.grpCustomer.Controls.Add(this.txtVendorId);
            this.grpCustomer.Controls.Add(this.label1);
            this.grpCustomer.Controls.Add(this.txtVendorPqNumber);
            this.grpCustomer.Controls.Add(this.label2);
            this.grpCustomer.Controls.Add(this.label5);
            this.grpCustomer.Controls.Add(this.dtPqDate);
            this.grpCustomer.Controls.Add(this.label8);
            this.grpCustomer.Controls.Add(this.txtPqNumber);
            this.grpCustomer.Controls.Add(this.label9);
            this.grpCustomer.Location = new System.Drawing.Point(6, 10);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(809, 116);
            this.grpCustomer.TabIndex = 141;
            this.grpCustomer.TabStop = false;
            this.grpCustomer.Text = "Header";
            // 
            // dtVendorPqDate
            // 
            this.dtVendorPqDate.CustomFormat = "dd-MM-yyyy";
            this.dtVendorPqDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtVendorPqDate.Location = new System.Drawing.Point(536, 40);
            this.dtVendorPqDate.Name = "dtVendorPqDate";
            this.dtVendorPqDate.Size = new System.Drawing.Size(200, 20);
            this.dtVendorPqDate.TabIndex = 142;
            // 
            // btnSearchPR
            // 
            this.btnSearchPR.Location = new System.Drawing.Point(331, 59);
            this.btnSearchPR.Name = "btnSearchPR";
            this.btnSearchPR.Size = new System.Drawing.Size(32, 23);
            this.btnSearchPR.TabIndex = 141;
            this.btnSearchPR.Text = "...";
            this.btnSearchPR.UseVisualStyleBackColor = true;
            this.btnSearchPR.Click += new System.EventHandler(this.btnSearchPR_Click);
            // 
            // txtPrNumber
            // 
            this.txtPrNumber.Enabled = false;
            this.txtPrNumber.Location = new System.Drawing.Point(125, 61);
            this.txtPrNumber.Name = "txtPrNumber";
            this.txtPrNumber.ReadOnly = true;
            this.txtPrNumber.Size = new System.Drawing.Size(200, 20);
            this.txtPrNumber.TabIndex = 140;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 139;
            this.label6.Text = "PR Number :";
            // 
            // cmbPaymentMode
            // 
            this.cmbPaymentMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMode.FormattingEnabled = true;
            this.cmbPaymentMode.Items.AddRange(new object[] {
            "LOCO",
            "FRANCO"});
            this.cmbPaymentMode.Location = new System.Drawing.Point(125, 84);
            this.cmbPaymentMode.Name = "cmbPaymentMode";
            this.cmbPaymentMode.Size = new System.Drawing.Size(200, 21);
            this.cmbPaymentMode.TabIndex = 165;
            // 
            // btnSearchVendor
            // 
            this.btnSearchVendor.Location = new System.Drawing.Point(742, 61);
            this.btnSearchVendor.Name = "btnSearchVendor";
            this.btnSearchVendor.Size = new System.Drawing.Size(32, 23);
            this.btnSearchVendor.TabIndex = 138;
            this.btnSearchVendor.Text = "...";
            this.btnSearchVendor.UseVisualStyleBackColor = true;
            this.btnSearchVendor.Click += new System.EventHandler(this.btnSearchVendor_Click);
            // 
            // txtVendorName
            // 
            this.txtVendorName.Enabled = false;
            this.txtVendorName.Location = new System.Drawing.Point(536, 87);
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(200, 20);
            this.txtVendorName.TabIndex = 135;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 87);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(84, 13);
            this.label15.TabIndex = 161;
            this.label15.Text = "Payment Mode :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(413, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 134;
            this.label3.Text = "Vendor Name :";
            // 
            // txtVendorId
            // 
            this.txtVendorId.Enabled = false;
            this.txtVendorId.Location = new System.Drawing.Point(536, 63);
            this.txtVendorId.Name = "txtVendorId";
            this.txtVendorId.ReadOnly = true;
            this.txtVendorId.Size = new System.Drawing.Size(200, 20);
            this.txtVendorId.TabIndex = 133;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(413, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 132;
            this.label1.Text = "Vendor Id :";
            // 
            // txtVendorPqNumber
            // 
            this.txtVendorPqNumber.Location = new System.Drawing.Point(536, 17);
            this.txtVendorPqNumber.Name = "txtVendorPqNumber";
            this.txtVendorPqNumber.Size = new System.Drawing.Size(200, 20);
            this.txtVendorPqNumber.TabIndex = 131;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(413, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 128;
            this.label2.Text = "Vendor Quote Date :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(413, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 13);
            this.label5.TabIndex = 127;
            this.label5.Text = "Vendor Quote Number :";
            // 
            // dtPqDate
            // 
            this.dtPqDate.CustomFormat = "dd-MM-yyyy";
            this.dtPqDate.Enabled = false;
            this.dtPqDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPqDate.Location = new System.Drawing.Point(125, 17);
            this.dtPqDate.Name = "dtPqDate";
            this.dtPqDate.Size = new System.Drawing.Size(200, 20);
            this.dtPqDate.TabIndex = 117;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 116;
            this.label8.Text = "Quote Date :";
            // 
            // txtPqNumber
            // 
            this.txtPqNumber.Enabled = false;
            this.txtPqNumber.Location = new System.Drawing.Point(125, 39);
            this.txtPqNumber.Name = "txtPqNumber";
            this.txtPqNumber.ReadOnly = true;
            this.txtPqNumber.Size = new System.Drawing.Size(200, 20);
            this.txtPqNumber.TabIndex = 100;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Quote Number :";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(648, 487);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 144;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(477, 487);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 145;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEditH_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(562, 487);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 143;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.btnNew);
            this.grpDetail.Controls.Add(this.btnDelete);
            this.grpDetail.Controls.Add(this.dgvPqDetails);
            this.grpDetail.Location = new System.Drawing.Point(6, 132);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(810, 216);
            this.grpDetail.TabIndex = 142;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Details";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(9, 15);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(59, 23);
            this.btnNew.TabIndex = 149;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(75, 15);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(59, 23);
            this.btnDelete.TabIndex = 148;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvPqDetails
            // 
            this.dgvPqDetails.AllowUserToAddRows = false;
            this.dgvPqDetails.AllowUserToDeleteRows = false;
            this.dgvPqDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPqDetails.Location = new System.Drawing.Point(9, 43);
            this.dgvPqDetails.Name = "dgvPqDetails";
            this.dgvPqDetails.ReadOnly = true;
            this.dgvPqDetails.Size = new System.Drawing.Size(794, 162);
            this.dgvPqDetails.TabIndex = 11;
            this.dgvPqDetails.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPqDetails_CellValueChanged);
            this.dgvPqDetails.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvPrDetails_CellBeginEdit);
            this.dgvPqDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPrDetails_CellEndEdit);
            this.dgvPqDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPrDetails_CellClick);
            this.dgvPqDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPrDetails_EditingControlShowing);
            this.dgvPqDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvPrDetails_KeyPress);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(734, 487);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 146;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupPRH
            // 
            this.groupPRH.Controls.Add(this.groupBox1);
            this.groupPRH.Controls.Add(this.grpCustomer);
            this.groupPRH.Controls.Add(this.grpDetail);
            this.groupPRH.Controls.Add(this.btnExit);
            this.groupPRH.Controls.Add(this.btnEdit);
            this.groupPRH.Controls.Add(this.btnCancel);
            this.groupPRH.Controls.Add(this.btnSave);
            this.groupPRH.Location = new System.Drawing.Point(12, 27);
            this.groupPRH.Name = "groupPRH";
            this.groupPRH.Size = new System.Drawing.Size(823, 520);
            this.groupPRH.TabIndex = 148;
            this.groupPRH.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.txtPPH);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.txtPPN);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.txtTermOfPayment);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txtDeskripsi);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtTotalPPH);
            this.groupBox1.Controls.Add(this.txtTotalPPN);
            this.groupBox1.Controls.Add(this.txtTotal);
            this.groupBox1.Controls.Add(this.txtCashBackScheme);
            this.groupBox1.Controls.Add(this.txtBonusScheme);
            this.groupBox1.Controls.Add(this.txtDiscScheme);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(6, 354);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(809, 127);
            this.groupBox1.TabIndex = 147;
            this.groupBox1.TabStop = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(221, 36);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(15, 13);
            this.label20.TabIndex = 171;
            this.label20.Text = "%";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(221, 17);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(15, 13);
            this.label19.TabIndex = 170;
            this.label19.Text = "%";
            // 
            // txtPPH
            // 
            this.txtPPH.Location = new System.Drawing.Point(125, 33);
            this.txtPPH.Name = "txtPPH";
            this.txtPPH.Size = new System.Drawing.Size(89, 20);
            this.txtPPH.TabIndex = 169;
            this.txtPPH.TextChanged += new System.EventHandler(this.txtPPH_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 36);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(35, 13);
            this.label18.TabIndex = 168;
            this.label18.Text = "PPH :";
            // 
            // txtPPN
            // 
            this.txtPPN.Location = new System.Drawing.Point(125, 11);
            this.txtPPN.Name = "txtPPN";
            this.txtPPN.Size = new System.Drawing.Size(89, 20);
            this.txtPPN.TabIndex = 167;
            this.txtPPN.TextChanged += new System.EventHandler(this.txtPPN_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 14);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(35, 13);
            this.label17.TabIndex = 166;
            this.label17.Text = "PPN :";
            // 
            // txtTermOfPayment
            // 
            this.txtTermOfPayment.Location = new System.Drawing.Point(125, 55);
            this.txtTermOfPayment.Name = "txtTermOfPayment";
            this.txtTermOfPayment.Size = new System.Drawing.Size(89, 20);
            this.txtTermOfPayment.TabIndex = 164;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 58);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(95, 13);
            this.label16.TabIndex = 163;
            this.label16.Text = "Term Of Payment :";
            // 
            // txtDeskripsi
            // 
            this.txtDeskripsi.Location = new System.Drawing.Point(536, 77);
            this.txtDeskripsi.Multiline = true;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.Size = new System.Drawing.Size(210, 41);
            this.txtDeskripsi.TabIndex = 160;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(413, 77);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 13);
            this.label14.TabIndex = 159;
            this.label14.Text = "Deskripsi :";
            // 
            // txtTotalPPH
            // 
            this.txtTotalPPH.Location = new System.Drawing.Point(536, 55);
            this.txtTotalPPH.Name = "txtTotalPPH";
            this.txtTotalPPH.Size = new System.Drawing.Size(210, 20);
            this.txtTotalPPH.TabIndex = 158;
            // 
            // txtTotalPPN
            // 
            this.txtTotalPPN.Location = new System.Drawing.Point(536, 33);
            this.txtTotalPPN.Name = "txtTotalPPN";
            this.txtTotalPPN.Size = new System.Drawing.Size(210, 20);
            this.txtTotalPPN.TabIndex = 157;
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(536, 11);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(210, 20);
            this.txtTotal.TabIndex = 156;
            // 
            // txtCashBackScheme
            // 
            this.txtCashBackScheme.Location = new System.Drawing.Point(125, 122);
            this.txtCashBackScheme.Name = "txtCashBackScheme";
            this.txtCashBackScheme.Size = new System.Drawing.Size(210, 20);
            this.txtCashBackScheme.TabIndex = 155;
            this.txtCashBackScheme.Visible = false;
            // 
            // txtBonusScheme
            // 
            this.txtBonusScheme.Location = new System.Drawing.Point(125, 100);
            this.txtBonusScheme.Name = "txtBonusScheme";
            this.txtBonusScheme.Size = new System.Drawing.Size(210, 20);
            this.txtBonusScheme.TabIndex = 154;
            this.txtBonusScheme.Visible = false;
            // 
            // txtDiscScheme
            // 
            this.txtDiscScheme.Location = new System.Drawing.Point(125, 77);
            this.txtDiscScheme.Name = "txtDiscScheme";
            this.txtDiscScheme.Size = new System.Drawing.Size(210, 20);
            this.txtDiscScheme.TabIndex = 153;
            this.txtDiscScheme.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(413, 58);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 13);
            this.label13.TabIndex = 152;
            this.label13.Text = "Total PPH :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(413, 36);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(62, 13);
            this.label12.TabIndex = 151;
            this.label12.Text = "Total PPN :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(413, 14);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 150;
            this.label11.Text = "Total :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 125);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 13);
            this.label10.TabIndex = 149;
            this.label10.Text = "CashBack Scheme :";
            this.label10.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 148;
            this.label7.Text = "Bonus Scheme :";
            this.label7.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 147;
            this.label4.Text = "Disc Scheme :";
            this.label4.Visible = false;
            // 
            // lblForm
            // 
            this.lblForm.AutoSize = true;
            this.lblForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForm.Location = new System.Drawing.Point(15, 14);
            this.lblForm.Name = "lblForm";
            this.lblForm.Size = new System.Drawing.Size(164, 13);
            this.lblForm.TabIndex = 132;
            this.lblForm.Text = "Purchase Quotation Header";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(221, 58);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(29, 13);
            this.label21.TabIndex = 172;
            this.label21.Text = "days";
            // 
            // FormPQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(845, 582);
            this.Controls.Add(this.lblForm);
            this.Controls.Add(this.groupPRH);
            this.Name = "FormPQ";
            this.Resizable = false;
            this.Text = "Purchase Quotation Header";
            this.Load += new System.EventHandler(this.FormPQ_Load);
            this.Shown += new System.EventHandler(this.FormPQ_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormPQ_FormClosed);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPqDetails)).EndInit();
            this.groupPRH.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.DateTimePicker dtPqDate;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.TextBox txtPqNumber;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.GroupBox grpDetail;
        internal System.Windows.Forms.DataGridView dgvPqDetails;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.TextBox txtVendorPqNumber;
        private System.Windows.Forms.GroupBox groupPRH;
        private System.Windows.Forms.Label lblForm;
        internal System.Windows.Forms.TextBox txtVendorName;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.TextBox txtVendorId;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearchVendor;
        private System.Windows.Forms.Button btnSearchPR;
        internal System.Windows.Forms.TextBox txtPrNumber;
        internal System.Windows.Forms.Label label6;
        internal System.Windows.Forms.DateTimePicker dtVendorPqDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtTotalPPH;
        private System.Windows.Forms.TextBox txtTotalPPN;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.TextBox txtCashBackScheme;
        private System.Windows.Forms.TextBox txtBonusScheme;
        private System.Windows.Forms.TextBox txtDiscScheme;
        private System.Windows.Forms.TextBox txtDeskripsi;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtTermOfPayment;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbPaymentMode;
        private System.Windows.Forms.TextBox txtPPH;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtPPN;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label21;

    }
}