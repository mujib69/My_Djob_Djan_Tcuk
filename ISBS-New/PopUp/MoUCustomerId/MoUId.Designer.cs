namespace ISBS_New.PopUp.MoUCustomerId
{
    partial class MoUId
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
            this.buttonExit = new System.Windows.Forms.Button();
            this.dgvMoU = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtJatuhTempo = new System.Windows.Forms.DateTimePicker();
            this.dtLCDate = new System.Windows.Forms.DateTimePicker();
            this.lblBankName = new System.Windows.Forms.Label();
            this.lblBankID = new System.Windows.Forms.Label();
            this.lblLCType = new System.Windows.Forms.Label();
            this.lblLCNo = new System.Windows.Forms.Label();
            this.lblBankGuarantee = new System.Windows.Forms.Label();
            this.lblLimitCredit = new System.Windows.Forms.Label();
            this.lblCustName = new System.Windows.Forms.Label();
            this.lblCustID = new System.Windows.Forms.Label();
            this.dtValidTo = new System.Windows.Forms.DateTimePicker();
            this.lblMoUNumber = new System.Windows.Forms.Label();
            this.dtMouDate = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
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
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoU)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonExit);
            this.groupBox1.Controls.Add(this.dgvMoU);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(23, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(667, 447);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(586, 400);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 2;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // dgvMoU
            // 
            this.dgvMoU.AllowUserToAddRows = false;
            this.dgvMoU.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMoU.Location = new System.Drawing.Point(6, 209);
            this.dgvMoU.Name = "dgvMoU";
            this.dgvMoU.Size = new System.Drawing.Size(655, 185);
            this.dgvMoU.TabIndex = 1;
            this.dgvMoU.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMoU_CellMouseDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dtJatuhTempo);
            this.groupBox2.Controls.Add(this.dtLCDate);
            this.groupBox2.Controls.Add(this.lblBankName);
            this.groupBox2.Controls.Add(this.lblBankID);
            this.groupBox2.Controls.Add(this.lblLCType);
            this.groupBox2.Controls.Add(this.lblLCNo);
            this.groupBox2.Controls.Add(this.lblBankGuarantee);
            this.groupBox2.Controls.Add(this.lblLimitCredit);
            this.groupBox2.Controls.Add(this.lblCustName);
            this.groupBox2.Controls.Add(this.lblCustID);
            this.groupBox2.Controls.Add(this.dtValidTo);
            this.groupBox2.Controls.Add(this.lblMoUNumber);
            this.groupBox2.Controls.Add(this.dtMouDate);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
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
            this.groupBox2.Location = new System.Drawing.Point(6, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(655, 193);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // dtJatuhTempo
            // 
            this.dtJatuhTempo.CustomFormat = "dd/MM/yyyy";
            this.dtJatuhTempo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtJatuhTempo.Location = new System.Drawing.Point(425, 113);
            this.dtJatuhTempo.Name = "dtJatuhTempo";
            this.dtJatuhTempo.Size = new System.Drawing.Size(185, 20);
            this.dtJatuhTempo.TabIndex = 155;
            // 
            // dtLCDate
            // 
            this.dtLCDate.CustomFormat = "dd/MM/yyyy";
            this.dtLCDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtLCDate.Location = new System.Drawing.Point(425, 87);
            this.dtLCDate.Name = "dtLCDate";
            this.dtLCDate.Size = new System.Drawing.Size(185, 20);
            this.dtLCDate.TabIndex = 154;
            // 
            // lblBankName
            // 
            this.lblBankName.AutoSize = true;
            this.lblBankName.Location = new System.Drawing.Point(422, 165);
            this.lblBankName.Name = "lblBankName";
            this.lblBankName.Size = new System.Drawing.Size(70, 13);
            this.lblBankName.TabIndex = 153;
            this.lblBankName.Text = "lblBankName";
            // 
            // lblBankID
            // 
            this.lblBankID.AutoSize = true;
            this.lblBankID.Location = new System.Drawing.Point(422, 140);
            this.lblBankID.Name = "lblBankID";
            this.lblBankID.Size = new System.Drawing.Size(53, 13);
            this.lblBankID.TabIndex = 152;
            this.lblBankID.Text = "lblBankID";
            // 
            // lblLCType
            // 
            this.lblLCType.AutoSize = true;
            this.lblLCType.Location = new System.Drawing.Point(422, 67);
            this.lblLCType.Name = "lblLCType";
            this.lblLCType.Size = new System.Drawing.Size(54, 13);
            this.lblLCType.TabIndex = 151;
            this.lblLCType.Text = "lblLCType";
            // 
            // lblLCNo
            // 
            this.lblLCNo.AutoSize = true;
            this.lblLCNo.Location = new System.Drawing.Point(422, 43);
            this.lblLCNo.Name = "lblLCNo";
            this.lblLCNo.Size = new System.Drawing.Size(44, 13);
            this.lblLCNo.TabIndex = 150;
            this.lblLCNo.Text = "lblLCNo";
            // 
            // lblBankGuarantee
            // 
            this.lblBankGuarantee.AutoSize = true;
            this.lblBankGuarantee.Location = new System.Drawing.Point(422, 20);
            this.lblBankGuarantee.Name = "lblBankGuarantee";
            this.lblBankGuarantee.Size = new System.Drawing.Size(92, 13);
            this.lblBankGuarantee.TabIndex = 149;
            this.lblBankGuarantee.Text = "lblBankGuarantee";
            // 
            // lblLimitCredit
            // 
            this.lblLimitCredit.AutoSize = true;
            this.lblLimitCredit.Location = new System.Drawing.Point(96, 140);
            this.lblLimitCredit.Name = "lblLimitCredit";
            this.lblLimitCredit.Size = new System.Drawing.Size(65, 13);
            this.lblLimitCredit.TabIndex = 148;
            this.lblLimitCredit.Text = "lblLimitCredit";
            // 
            // lblCustName
            // 
            this.lblCustName.AutoSize = true;
            this.lblCustName.Location = new System.Drawing.Point(96, 116);
            this.lblCustName.Name = "lblCustName";
            this.lblCustName.Size = new System.Drawing.Size(66, 13);
            this.lblCustName.TabIndex = 147;
            this.lblCustName.Text = "lblCustName";
            this.lblCustName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblCustName_MouseDown);
            // 
            // lblCustID
            // 
            this.lblCustID.AutoSize = true;
            this.lblCustID.Location = new System.Drawing.Point(97, 93);
            this.lblCustID.Name = "lblCustID";
            this.lblCustID.Size = new System.Drawing.Size(49, 13);
            this.lblCustID.TabIndex = 146;
            this.lblCustID.Text = "lblCustID";
            this.lblCustID.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblCustID_MouseDown);
            // 
            // dtValidTo
            // 
            this.dtValidTo.CustomFormat = "dd/MM/yyyy";
            this.dtValidTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtValidTo.Location = new System.Drawing.Point(99, 61);
            this.dtValidTo.Name = "dtValidTo";
            this.dtValidTo.Size = new System.Drawing.Size(185, 20);
            this.dtValidTo.TabIndex = 145;
            // 
            // lblMoUNumber
            // 
            this.lblMoUNumber.AutoSize = true;
            this.lblMoUNumber.Location = new System.Drawing.Point(96, 43);
            this.lblMoUNumber.Name = "lblMoUNumber";
            this.lblMoUNumber.Size = new System.Drawing.Size(77, 13);
            this.lblMoUNumber.TabIndex = 144;
            this.lblMoUNumber.Text = "lblMoUNumber";
            // 
            // dtMouDate
            // 
            this.dtMouDate.CustomFormat = "dd/MM/yyyy";
            this.dtMouDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtMouDate.Location = new System.Drawing.Point(99, 13);
            this.dtMouDate.Name = "dtMouDate";
            this.dtMouDate.Size = new System.Drawing.Size(185, 20);
            this.dtMouDate.TabIndex = 143;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(319, 165);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(99, 13);
            this.label12.TabIndex = 142;
            this.label12.Text = "Bank Name          : ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(319, 140);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(97, 13);
            this.label13.TabIndex = 139;
            this.label13.Text = "Bank ID                :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(319, 116);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(96, 13);
            this.label11.TabIndex = 137;
            this.label11.Text = "Jatuh Tempo        :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(319, 93);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(97, 13);
            this.label10.TabIndex = 135;
            this.label10.Text = "LC Date                :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(319, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 13);
            this.label9.TabIndex = 133;
            this.label9.Text = "LC Type                :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(319, 43);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 131;
            this.label8.Text = "LC No                   :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(319, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 13);
            this.label7.TabIndex = 129;
            this.label7.Text = "Bank Guarantee   :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 140);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Limit Credit      :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Cust Name      :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Cust ID            :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Valid To           :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "MoU Number   :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "MoU Date        :";
            // 
            // MoUId
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 533);
            this.Controls.Add(this.groupBox1);
            this.Name = "MoUId";
            this.Text = "MoU Customer";
            this.Load += new System.EventHandler(this.MoUId_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoU)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DateTimePicker dtMouDate;
        private System.Windows.Forms.Label lblMoUNumber;
        private System.Windows.Forms.DateTimePicker dtValidTo;
        private System.Windows.Forms.Label lblBankID;
        private System.Windows.Forms.Label lblLCType;
        private System.Windows.Forms.Label lblLCNo;
        private System.Windows.Forms.Label lblBankGuarantee;
        private System.Windows.Forms.Label lblLimitCredit;
        private System.Windows.Forms.Label lblCustName;
        private System.Windows.Forms.Label lblCustID;
        private System.Windows.Forms.DateTimePicker dtLCDate;
        private System.Windows.Forms.Label lblBankName;
        private System.Windows.Forms.DateTimePicker dtJatuhTempo;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.DataGridView dgvMoU;
    }
}