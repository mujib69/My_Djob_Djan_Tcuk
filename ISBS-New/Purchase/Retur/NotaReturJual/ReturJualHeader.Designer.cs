namespace ISBS_New.Purchase.Retur.NotaReturJual
{
    partial class ReturJualHeader
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
            this.dgvNRJ = new System.Windows.Forms.DataGridView();
            this.txtStatusName = new System.Windows.Forms.TextBox();
            this.txtApproved = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.gbApprove = new System.Windows.Forms.GroupBox();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.cbSame = new System.Windows.Forms.CheckBox();
            this.btnSearchInventSite = new System.Windows.Forms.Button();
            this.btnSearchVehicleOwner = new System.Windows.Forms.Button();
            this.txtInventSiteID = new System.Windows.Forms.TextBox();
            this.txtDriverName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtInventSiteName = new System.Windows.Forms.TextBox();
            this.txtVehicleNumber = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtVehicleType = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.dtExpectedReturnDate = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCustID = new System.Windows.Forms.TextBox();
            this.txtCustName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtBBK = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.txtVehicleOwnerID = new System.Windows.Forms.TextBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbGV = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtVehicleOwnerName = new System.Windows.Forms.TextBox();
            this.btnSearchBBK = new System.Windows.Forms.Button();
            this.txtBBKNum = new System.Windows.Forms.TextBox();
            this.txtNRJNum = new System.Windows.Forms.TextBox();
            this.dtNRJ = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNRJ)).BeginInit();
            this.gbMain.SuspendLayout();
            this.gbApprove.SuspendLayout();
            this.gbGV.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvNRJ
            // 
            this.dgvNRJ.AllowUserToAddRows = false;
            this.dgvNRJ.AllowUserToDeleteRows = false;
            this.dgvNRJ.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNRJ.Location = new System.Drawing.Point(9, 41);
            this.dgvNRJ.Name = "dgvNRJ";
            this.dgvNRJ.Size = new System.Drawing.Size(815, 155);
            this.dgvNRJ.TabIndex = 36;
            this.dgvNRJ.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNRJ_CellEndEdit);
            this.dgvNRJ.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvNRJ_EditingControlShowing);
            // 
            // txtStatusName
            // 
            this.txtStatusName.Enabled = false;
            this.txtStatusName.Location = new System.Drawing.Point(519, 153);
            this.txtStatusName.Name = "txtStatusName";
            this.txtStatusName.ReadOnly = true;
            this.txtStatusName.Size = new System.Drawing.Size(200, 20);
            this.txtStatusName.TabIndex = 166;
            // 
            // txtApproved
            // 
            this.txtApproved.Enabled = false;
            this.txtApproved.Location = new System.Drawing.Point(519, 176);
            this.txtApproved.Name = "txtApproved";
            this.txtApproved.ReadOnly = true;
            this.txtApproved.Size = new System.Drawing.Size(200, 20);
            this.txtApproved.TabIndex = 165;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(419, 179);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 164;
            this.label7.Text = "NRJ Approved :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(419, 156);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 163;
            this.label8.Text = "NRJ Status :";
            // 
            // gbMain
            // 
            this.gbMain.Controls.Add(this.gbApprove);
            this.gbMain.Controls.Add(this.cbSame);
            this.gbMain.Controls.Add(this.btnSearchInventSite);
            this.gbMain.Controls.Add(this.btnSearchVehicleOwner);
            this.gbMain.Controls.Add(this.txtInventSiteID);
            this.gbMain.Controls.Add(this.txtDriverName);
            this.gbMain.Controls.Add(this.label13);
            this.gbMain.Controls.Add(this.txtInventSiteName);
            this.gbMain.Controls.Add(this.txtVehicleNumber);
            this.gbMain.Controls.Add(this.label12);
            this.gbMain.Controls.Add(this.label14);
            this.gbMain.Controls.Add(this.txtVehicleType);
            this.gbMain.Controls.Add(this.label10);
            this.gbMain.Controls.Add(this.dtExpectedReturnDate);
            this.gbMain.Controls.Add(this.label9);
            this.gbMain.Controls.Add(this.txtCustID);
            this.gbMain.Controls.Add(this.txtCustName);
            this.gbMain.Controls.Add(this.label6);
            this.gbMain.Controls.Add(this.dtBBK);
            this.gbMain.Controls.Add(this.label4);
            this.gbMain.Controls.Add(this.txtStatusName);
            this.gbMain.Controls.Add(this.txtApproved);
            this.gbMain.Controls.Add(this.label7);
            this.gbMain.Controls.Add(this.label8);
            this.gbMain.Controls.Add(this.txtVehicleOwnerID);
            this.gbMain.Controls.Add(this.txtNotes);
            this.gbMain.Controls.Add(this.label20);
            this.gbMain.Controls.Add(this.btnExit);
            this.gbMain.Controls.Add(this.btnCancel);
            this.gbMain.Controls.Add(this.gbGV);
            this.gbMain.Controls.Add(this.btnEdit);
            this.gbMain.Controls.Add(this.btnSave);
            this.gbMain.Controls.Add(this.txtVehicleOwnerName);
            this.gbMain.Controls.Add(this.btnSearchBBK);
            this.gbMain.Controls.Add(this.txtBBKNum);
            this.gbMain.Controls.Add(this.txtNRJNum);
            this.gbMain.Controls.Add(this.dtNRJ);
            this.gbMain.Controls.Add(this.label11);
            this.gbMain.Controls.Add(this.label5);
            this.gbMain.Controls.Add(this.label3);
            this.gbMain.Controls.Add(this.label2);
            this.gbMain.Location = new System.Drawing.Point(12, 59);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(861, 510);
            this.gbMain.TabIndex = 44;
            this.gbMain.TabStop = false;
            // 
            // gbApprove
            // 
            this.gbApprove.Controls.Add(this.btnReject);
            this.gbApprove.Controls.Add(this.btnApprove);
            this.gbApprove.Location = new System.Drawing.Point(343, 468);
            this.gbApprove.Name = "gbApprove";
            this.gbApprove.Size = new System.Drawing.Size(173, 42);
            this.gbApprove.TabIndex = 186;
            this.gbApprove.TabStop = false;
            this.gbApprove.Visible = false;
            // 
            // btnReject
            // 
            this.btnReject.Location = new System.Drawing.Point(92, 13);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(75, 23);
            this.btnReject.TabIndex = 188;
            this.btnReject.Text = "Reject";
            this.btnReject.UseVisualStyleBackColor = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnApprove
            // 
            this.btnApprove.Location = new System.Drawing.Point(6, 13);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(75, 23);
            this.btnApprove.TabIndex = 187;
            this.btnApprove.Text = "Approve";
            this.btnApprove.UseVisualStyleBackColor = true;
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // cbSame
            // 
            this.cbSame.AutoSize = true;
            this.cbSame.Enabled = false;
            this.cbSame.Location = new System.Drawing.Point(755, 21);
            this.cbSame.Name = "cbSame";
            this.cbSame.Size = new System.Drawing.Size(105, 17);
            this.cbSame.TabIndex = 185;
            this.cbSame.Text = "Same as Cust ID";
            this.cbSame.UseVisualStyleBackColor = true;
            this.cbSame.CheckedChanged += new System.EventHandler(this.cbSame_CheckedChanged);
            // 
            // btnSearchInventSite
            // 
            this.btnSearchInventSite.Enabled = false;
            this.btnSearchInventSite.Location = new System.Drawing.Point(725, 125);
            this.btnSearchInventSite.Name = "btnSearchInventSite";
            this.btnSearchInventSite.Size = new System.Drawing.Size(23, 20);
            this.btnSearchInventSite.TabIndex = 184;
            this.btnSearchInventSite.Text = "...";
            this.btnSearchInventSite.UseVisualStyleBackColor = true;
            this.btnSearchInventSite.Click += new System.EventHandler(this.btnSearchInventSite_Click);
            // 
            // btnSearchVehicleOwner
            // 
            this.btnSearchVehicleOwner.Enabled = false;
            this.btnSearchVehicleOwner.Location = new System.Drawing.Point(725, 19);
            this.btnSearchVehicleOwner.Name = "btnSearchVehicleOwner";
            this.btnSearchVehicleOwner.Size = new System.Drawing.Size(23, 20);
            this.btnSearchVehicleOwner.TabIndex = 183;
            this.btnSearchVehicleOwner.Text = "...";
            this.btnSearchVehicleOwner.UseVisualStyleBackColor = true;
            this.btnSearchVehicleOwner.Click += new System.EventHandler(this.btnSearchVehicleOwner_Click);
            // 
            // txtInventSiteID
            // 
            this.txtInventSiteID.Enabled = false;
            this.txtInventSiteID.Location = new System.Drawing.Point(519, 125);
            this.txtInventSiteID.Name = "txtInventSiteID";
            this.txtInventSiteID.Size = new System.Drawing.Size(50, 20);
            this.txtInventSiteID.TabIndex = 182;
            // 
            // txtDriverName
            // 
            this.txtDriverName.Enabled = false;
            this.txtDriverName.Location = new System.Drawing.Point(519, 98);
            this.txtDriverName.Name = "txtDriverName";
            this.txtDriverName.Size = new System.Drawing.Size(200, 20);
            this.txtDriverName.TabIndex = 179;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(418, 101);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(66, 13);
            this.label13.TabIndex = 178;
            this.label13.Text = "Driver Name";
            // 
            // txtInventSiteName
            // 
            this.txtInventSiteName.Enabled = false;
            this.txtInventSiteName.Location = new System.Drawing.Point(575, 125);
            this.txtInventSiteName.Name = "txtInventSiteName";
            this.txtInventSiteName.Size = new System.Drawing.Size(144, 20);
            this.txtInventSiteName.TabIndex = 181;
            // 
            // txtVehicleNumber
            // 
            this.txtVehicleNumber.Enabled = false;
            this.txtVehicleNumber.Location = new System.Drawing.Point(519, 73);
            this.txtVehicleNumber.Name = "txtVehicleNumber";
            this.txtVehicleNumber.Size = new System.Drawing.Size(200, 20);
            this.txtVehicleNumber.TabIndex = 177;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(418, 76);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 13);
            this.label12.TabIndex = 176;
            this.label12.Text = "Vehicle Number";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(418, 131);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 13);
            this.label14.TabIndex = 180;
            this.label14.Text = "Warehouse";
            // 
            // txtVehicleType
            // 
            this.txtVehicleType.Enabled = false;
            this.txtVehicleType.Location = new System.Drawing.Point(519, 48);
            this.txtVehicleType.Name = "txtVehicleType";
            this.txtVehicleType.Size = new System.Drawing.Size(200, 20);
            this.txtVehicleType.TabIndex = 175;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(418, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 13);
            this.label10.TabIndex = 174;
            this.label10.Text = "Vehicle Type";
            // 
            // dtExpectedReturnDate
            // 
            this.dtExpectedReturnDate.CustomFormat = "dd/MM/yyyy";
            this.dtExpectedReturnDate.Enabled = false;
            this.dtExpectedReturnDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtExpectedReturnDate.Location = new System.Drawing.Point(139, 152);
            this.dtExpectedReturnDate.Name = "dtExpectedReturnDate";
            this.dtExpectedReturnDate.Size = new System.Drawing.Size(200, 20);
            this.dtExpectedReturnDate.TabIndex = 173;
            this.dtExpectedReturnDate.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 13);
            this.label9.TabIndex = 172;
            this.label9.Text = "Expected Return Date";
            // 
            // txtCustID
            // 
            this.txtCustID.Enabled = false;
            this.txtCustID.Location = new System.Drawing.Point(139, 124);
            this.txtCustID.Name = "txtCustID";
            this.txtCustID.Size = new System.Drawing.Size(50, 20);
            this.txtCustID.TabIndex = 171;
            // 
            // txtCustName
            // 
            this.txtCustName.Enabled = false;
            this.txtCustName.Location = new System.Drawing.Point(195, 124);
            this.txtCustName.Name = "txtCustName";
            this.txtCustName.Size = new System.Drawing.Size(144, 20);
            this.txtCustName.TabIndex = 170;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 169;
            this.label6.Text = "Customer";
            // 
            // dtBBK
            // 
            this.dtBBK.CustomFormat = "dd/MM/yyyy";
            this.dtBBK.Enabled = false;
            this.dtBBK.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtBBK.Location = new System.Drawing.Point(139, 76);
            this.dtBBK.Name = "dtBBK";
            this.dtBBK.Size = new System.Drawing.Size(200, 20);
            this.dtBBK.TabIndex = 168;
            this.dtBBK.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 167;
            this.label4.Text = "BBK Date";
            // 
            // txtVehicleOwnerID
            // 
            this.txtVehicleOwnerID.Enabled = false;
            this.txtVehicleOwnerID.Location = new System.Drawing.Point(519, 19);
            this.txtVehicleOwnerID.Name = "txtVehicleOwnerID";
            this.txtVehicleOwnerID.Size = new System.Drawing.Size(50, 20);
            this.txtVehicleOwnerID.TabIndex = 40;
            // 
            // txtNotes
            // 
            this.txtNotes.Enabled = false;
            this.txtNotes.Location = new System.Drawing.Point(116, 417);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(214, 87);
            this.txtNotes.TabIndex = 8;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(28, 420);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(35, 13);
            this.label20.TabIndex = 38;
            this.label20.Text = "Notes";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(765, 481);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 27;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(684, 481);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 26;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbGV
            // 
            this.gbGV.Controls.Add(this.dgvNRJ);
            this.gbGV.Controls.Add(this.btnDelete);
            this.gbGV.Controls.Add(this.btnNew);
            this.gbGV.Location = new System.Drawing.Point(16, 212);
            this.gbGV.Name = "gbGV";
            this.gbGV.Size = new System.Drawing.Size(839, 199);
            this.gbGV.TabIndex = 33;
            this.gbGV.TabStop = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(90, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.Enabled = false;
            this.btnNew.Location = new System.Drawing.Point(9, 12);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 9;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(522, 481);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 24;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(603, 481);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 25;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtVehicleOwnerName
            // 
            this.txtVehicleOwnerName.Enabled = false;
            this.txtVehicleOwnerName.Location = new System.Drawing.Point(575, 19);
            this.txtVehicleOwnerName.Name = "txtVehicleOwnerName";
            this.txtVehicleOwnerName.Size = new System.Drawing.Size(144, 20);
            this.txtVehicleOwnerName.TabIndex = 17;
            // 
            // btnSearchBBK
            // 
            this.btnSearchBBK.Enabled = false;
            this.btnSearchBBK.Location = new System.Drawing.Point(343, 102);
            this.btnSearchBBK.Name = "btnSearchBBK";
            this.btnSearchBBK.Size = new System.Drawing.Size(23, 20);
            this.btnSearchBBK.TabIndex = 1;
            this.btnSearchBBK.Text = "...";
            this.btnSearchBBK.UseVisualStyleBackColor = true;
            this.btnSearchBBK.Click += new System.EventHandler(this.btnSearchBBK_Click);
            // 
            // txtBBKNum
            // 
            this.txtBBKNum.Enabled = false;
            this.txtBBKNum.Location = new System.Drawing.Point(139, 102);
            this.txtBBKNum.Name = "txtBBKNum";
            this.txtBBKNum.ReadOnly = true;
            this.txtBBKNum.Size = new System.Drawing.Size(200, 20);
            this.txtBBKNum.TabIndex = 13;
            // 
            // txtNRJNum
            // 
            this.txtNRJNum.Enabled = false;
            this.txtNRJNum.Location = new System.Drawing.Point(139, 48);
            this.txtNRJNum.Name = "txtNRJNum";
            this.txtNRJNum.Size = new System.Drawing.Size(200, 20);
            this.txtNRJNum.TabIndex = 11;
            // 
            // dtNRJ
            // 
            this.dtNRJ.CustomFormat = "dd/MM/yyyy";
            this.dtNRJ.Enabled = false;
            this.dtNRJ.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtNRJ.Location = new System.Drawing.Point(139, 22);
            this.dtNRJ.Name = "dtNRJ";
            this.dtNRJ.Size = new System.Drawing.Size(200, 20);
            this.dtNRJ.TabIndex = 10;
            this.dtNRJ.Value = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 52);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "NRJ Number";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "BBK Number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(418, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Vehicle Owner";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "NRJ Date";
            // 
            // ReturJualHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 581);
            this.Controls.Add(this.gbMain);
            this.Name = "ReturJualHeader";
            this.Text = "ReturJualHeader";
            this.Load += new System.EventHandler(this.ReturJualHeader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNRJ)).EndInit();
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            this.gbApprove.ResumeLayout(false);
            this.gbGV.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvNRJ;
        internal System.Windows.Forms.TextBox txtStatusName;
        internal System.Windows.Forms.TextBox txtApproved;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.TextBox txtVehicleOwnerID;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbGV;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtVehicleOwnerName;
        private System.Windows.Forms.Button btnSearchBBK;
        private System.Windows.Forms.TextBox txtBBKNum;
        private System.Windows.Forms.TextBox txtNRJNum;
        private System.Windows.Forms.DateTimePicker dtNRJ;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtBBK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtExpectedReturnDate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCustID;
        private System.Windows.Forms.TextBox txtCustName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDriverName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtVehicleNumber;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtVehicleType;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtInventSiteID;
        private System.Windows.Forms.TextBox txtInventSiteName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cbSame;
        private System.Windows.Forms.Button btnSearchInventSite;
        private System.Windows.Forms.Button btnSearchVehicleOwner;
        private System.Windows.Forms.GroupBox gbApprove;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnApprove;
    }
}