namespace ISBS_New.PopUp.DeliveryOrder
{
    partial class Deliveryorder
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.rtbNotes = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dgvItemOverview = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvItemDetail = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dgvTransferNote = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dgvRetur = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtDate = new System.Windows.Forms.DateTimePicker();
            this.dtDeliveryOrderDate = new System.Windows.Forms.DateTimePicker();
            this.dtSODate = new System.Windows.Forms.DateTimePicker();
            this.lblDriverName = new System.Windows.Forms.Label();
            this.lblVehicleNumber = new System.Windows.Forms.Label();
            this.lblVehicleType = new System.Windows.Forms.Label();
            this.lblVehicleOwner = new System.Windows.Forms.Label();
            this.lblWarehouse = new System.Windows.Forms.Label();
            this.lblDOStatus = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.lblSONo = new System.Windows.Forms.Label();
            this.lblDONo = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LblWarehouseName = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemOverview)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemDetail)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransferNote)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRetur)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.rtbNotes);
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(23, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(668, 603);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 478);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Notes";
            // 
            // rtbNotes
            // 
            this.rtbNotes.Location = new System.Drawing.Point(65, 475);
            this.rtbNotes.Name = "rtbNotes";
            this.rtbNotes.Size = new System.Drawing.Size(309, 96);
            this.rtbNotes.TabIndex = 4;
            this.rtbNotes.Text = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(6, 244);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(644, 225);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvItemOverview);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(636, 199);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Item Overview";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvItemOverview
            // 
            this.dgvItemOverview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItemOverview.Location = new System.Drawing.Point(3, 3);
            this.dgvItemOverview.Name = "dgvItemOverview";
            this.dgvItemOverview.Size = new System.Drawing.Size(630, 193);
            this.dgvItemOverview.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvItemDetail);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(733, 199);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Item Detail";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvItemDetail
            // 
            this.dgvItemDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItemDetail.Location = new System.Drawing.Point(3, 3);
            this.dgvItemDetail.Name = "dgvItemDetail";
            this.dgvItemDetail.Size = new System.Drawing.Size(727, 196);
            this.dgvItemDetail.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgvTransferNote);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(733, 199);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Transfer Note";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvTransferNote
            // 
            this.dgvTransferNote.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTransferNote.Location = new System.Drawing.Point(3, 3);
            this.dgvTransferNote.Name = "dgvTransferNote";
            this.dgvTransferNote.Size = new System.Drawing.Size(727, 193);
            this.dgvTransferNote.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.dgvRetur);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(733, 199);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Retur";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // dgvRetur
            // 
            this.dgvRetur.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRetur.Location = new System.Drawing.Point(3, 3);
            this.dgvRetur.Name = "dgvRetur";
            this.dgvRetur.Size = new System.Drawing.Size(727, 193);
            this.dgvRetur.TabIndex = 0;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(575, 567);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LblWarehouseName);
            this.groupBox2.Controls.Add(this.dtDate);
            this.groupBox2.Controls.Add(this.dtDeliveryOrderDate);
            this.groupBox2.Controls.Add(this.dtSODate);
            this.groupBox2.Controls.Add(this.lblDriverName);
            this.groupBox2.Controls.Add(this.lblVehicleNumber);
            this.groupBox2.Controls.Add(this.lblVehicleType);
            this.groupBox2.Controls.Add(this.lblVehicleOwner);
            this.groupBox2.Controls.Add(this.lblWarehouse);
            this.groupBox2.Controls.Add(this.lblDOStatus);
            this.groupBox2.Controls.Add(this.lblName);
            this.groupBox2.Controls.Add(this.lblCustomer);
            this.groupBox2.Controls.Add(this.lblSONo);
            this.groupBox2.Controls.Add(this.lblDONo);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(6, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(644, 227);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // dtDate
            // 
            this.dtDate.CustomFormat = "dd/MM/yyyy";
            this.dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDate.Location = new System.Drawing.Point(416, 18);
            this.dtDate.Name = "dtDate";
            this.dtDate.Size = new System.Drawing.Size(127, 20);
            this.dtDate.TabIndex = 27;
            // 
            // dtDeliveryOrderDate
            // 
            this.dtDeliveryOrderDate.CustomFormat = "dd/MM/yyyy";
            this.dtDeliveryOrderDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDeliveryOrderDate.Location = new System.Drawing.Point(109, 98);
            this.dtDeliveryOrderDate.Name = "dtDeliveryOrderDate";
            this.dtDeliveryOrderDate.Size = new System.Drawing.Size(127, 20);
            this.dtDeliveryOrderDate.TabIndex = 26;
            // 
            // dtSODate
            // 
            this.dtSODate.CustomFormat = "dd/MM/yyyy";
            this.dtSODate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtSODate.Location = new System.Drawing.Point(109, 71);
            this.dtSODate.Name = "dtSODate";
            this.dtSODate.Size = new System.Drawing.Size(127, 20);
            this.dtSODate.TabIndex = 25;
            // 
            // lblDriverName
            // 
            this.lblDriverName.AutoSize = true;
            this.lblDriverName.Location = new System.Drawing.Point(415, 183);
            this.lblDriverName.Name = "lblDriverName";
            this.lblDriverName.Size = new System.Drawing.Size(73, 13);
            this.lblDriverName.TabIndex = 24;
            this.lblDriverName.Text = "lblDriverName";
            // 
            // lblVehicleNumber
            // 
            this.lblVehicleNumber.AutoSize = true;
            this.lblVehicleNumber.Location = new System.Drawing.Point(415, 156);
            this.lblVehicleNumber.Name = "lblVehicleNumber";
            this.lblVehicleNumber.Size = new System.Drawing.Size(89, 13);
            this.lblVehicleNumber.TabIndex = 23;
            this.lblVehicleNumber.Text = "lblVehicleNumber";
            // 
            // lblVehicleType
            // 
            this.lblVehicleType.AutoSize = true;
            this.lblVehicleType.Location = new System.Drawing.Point(415, 132);
            this.lblVehicleType.Name = "lblVehicleType";
            this.lblVehicleType.Size = new System.Drawing.Size(76, 13);
            this.lblVehicleType.TabIndex = 22;
            this.lblVehicleType.Text = "lblVehicleType";
            // 
            // lblVehicleOwner
            // 
            this.lblVehicleOwner.AutoSize = true;
            this.lblVehicleOwner.Location = new System.Drawing.Point(415, 104);
            this.lblVehicleOwner.Name = "lblVehicleOwner";
            this.lblVehicleOwner.Size = new System.Drawing.Size(83, 13);
            this.lblVehicleOwner.TabIndex = 21;
            this.lblVehicleOwner.Text = "lblVehicleOwner";
            // 
            // lblWarehouse
            // 
            this.lblWarehouse.AutoSize = true;
            this.lblWarehouse.Location = new System.Drawing.Point(415, 78);
            this.lblWarehouse.Name = "lblWarehouse";
            this.lblWarehouse.Size = new System.Drawing.Size(72, 13);
            this.lblWarehouse.TabIndex = 20;
            this.lblWarehouse.Text = "lblWarehouse";
            // 
            // lblDOStatus
            // 
            this.lblDOStatus.AutoSize = true;
            this.lblDOStatus.Location = new System.Drawing.Point(415, 50);
            this.lblDOStatus.Name = "lblDOStatus";
            this.lblDOStatus.Size = new System.Drawing.Size(63, 13);
            this.lblDOStatus.TabIndex = 19;
            this.lblDOStatus.Text = "lblDOStatus";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(106, 156);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(45, 13);
            this.lblName.TabIndex = 18;
            this.lblName.Text = "lblName";
            // 
            // lblCustomer
            // 
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(106, 132);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(61, 13);
            this.lblCustomer.TabIndex = 17;
            this.lblCustomer.Text = "lblCustomer";
            // 
            // lblSONo
            // 
            this.lblSONo.AutoSize = true;
            this.lblSONo.Location = new System.Drawing.Point(106, 50);
            this.lblSONo.Name = "lblSONo";
            this.lblSONo.Size = new System.Drawing.Size(46, 13);
            this.lblSONo.TabIndex = 14;
            this.lblSONo.Text = "lblSONo";
            // 
            // lblDONo
            // 
            this.lblDONo.AutoSize = true;
            this.lblDONo.Location = new System.Drawing.Point(106, 25);
            this.lblDONo.Name = "lblDONo";
            this.lblDONo.Size = new System.Drawing.Size(47, 13);
            this.lblDONo.TabIndex = 13;
            this.lblDONo.Text = "lblDONo";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(314, 183);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(93, 13);
            this.label13.TabIndex = 12;
            this.label13.Text = "Driver Name        :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(314, 156);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(94, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Vehicle Number   :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(314, 132);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(93, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Vehicle Type       :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(314, 104);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Vehicle Owner     :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(314, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Warehouse          :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(314, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "DO Status            :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(314, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Date                     :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 156);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Name              :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Customer         :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Delivery Date  :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "SO Date          :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "SO No             :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "DO No             :";
            // 
            // LblWarehouseName
            // 
            this.LblWarehouseName.AutoSize = true;
            this.LblWarehouseName.Location = new System.Drawing.Point(493, 78);
            this.LblWarehouseName.Name = "LblWarehouseName";
            this.LblWarehouseName.Size = new System.Drawing.Size(104, 13);
            this.LblWarehouseName.TabIndex = 28;
            this.LblWarehouseName.Text = "LblWarehouseName";
            // 
            // Deliveryorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 681);
            this.Controls.Add(this.groupBox1);
            this.Name = "Deliveryorder";
            this.Text = "Delivery Order";
            this.Load += new System.EventHandler(this.Deliveryorder_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemOverview)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItemDetail)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransferNote)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRetur)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.Label lblSONo;
        private System.Windows.Forms.Label lblDONo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblDOStatus;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.DateTimePicker dtDeliveryOrderDate;
        private System.Windows.Forms.DateTimePicker dtSODate;
        private System.Windows.Forms.Label lblDriverName;
        private System.Windows.Forms.Label lblVehicleNumber;
        private System.Windows.Forms.Label lblVehicleType;
        private System.Windows.Forms.Label lblVehicleOwner;
        private System.Windows.Forms.Label lblWarehouse;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RichTextBox rtbNotes;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dgvItemOverview;
        private System.Windows.Forms.DataGridView dgvItemDetail;
        private System.Windows.Forms.DataGridView dgvTransferNote;
        private System.Windows.Forms.DataGridView dgvRetur;
        private System.Windows.Forms.Label LblWarehouseName;
    }
}