namespace ISBS_New.Purchase.GoodsReceipt.Resize
{
    public partial class FormResizeGR
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblposted = new System.Windows.Forms.Label();
            this.chkPosted = new System.Windows.Forms.CheckBox();
            this.txtReffTransID = new System.Windows.Forms.TextBox();
            this.dtTransDate = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTransNo = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.dgvPrDetails = new System.Windows.Forms.DataGridView();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnCancel);
            this.groupBox3.Controls.Add(this.btnEdit);
            this.groupBox3.Controls.Add(this.btnSave);
            this.groupBox3.Controls.Add(this.btnExit);
            this.groupBox3.Location = new System.Drawing.Point(12, 374);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(860, 46);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Footer";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(681, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 148;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(510, 12);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 149;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(595, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 147;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(767, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 150;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblposted);
            this.groupBox1.Controls.Add(this.chkPosted);
            this.groupBox1.Controls.Add(this.txtReffTransID);
            this.groupBox1.Controls.Add(this.dtTransDate);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtTransNo);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Location = new System.Drawing.Point(12, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(860, 90);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Header";
            // 
            // lblposted
            // 
            this.lblposted.AutoSize = true;
            this.lblposted.Location = new System.Drawing.Point(722, 18);
            this.lblposted.Name = "lblposted";
            this.lblposted.Size = new System.Drawing.Size(80, 13);
            this.lblposted.TabIndex = 145;
            this.lblposted.Text = "Current Status :";
            // 
            // chkPosted
            // 
            this.chkPosted.AutoSize = true;
            this.chkPosted.Location = new System.Drawing.Point(783, 40);
            this.chkPosted.Name = "chkPosted";
            this.chkPosted.Size = new System.Drawing.Size(59, 17);
            this.chkPosted.TabIndex = 144;
            this.chkPosted.Text = "Posted";
            this.chkPosted.UseVisualStyleBackColor = true;
            // 
            // txtReffTransID
            // 
            this.txtReffTransID.Location = new System.Drawing.Point(127, 60);
            this.txtReffTransID.Name = "txtReffTransID";
            this.txtReffTransID.ReadOnly = true;
            this.txtReffTransID.Size = new System.Drawing.Size(200, 20);
            this.txtReffTransID.TabIndex = 143;
            // 
            // dtTransDate
            // 
            this.dtTransDate.CustomFormat = "dd-MM-yyyy";
            this.dtTransDate.Enabled = false;
            this.dtTransDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTransDate.Location = new System.Drawing.Point(127, 15);
            this.dtTransDate.Name = "dtTransDate";
            this.dtTransDate.Size = new System.Drawing.Size(200, 20);
            this.dtTransDate.TabIndex = 136;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 133;
            this.label9.Text = "Trans No :";
            // 
            // txtTransNo
            // 
            this.txtTransNo.Location = new System.Drawing.Point(127, 37);
            this.txtTransNo.Name = "txtTransNo";
            this.txtTransNo.ReadOnly = true;
            this.txtTransNo.Size = new System.Drawing.Size(200, 20);
            this.txtTransNo.TabIndex = 134;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 13);
            this.label8.TabIndex = 135;
            this.label8.Text = "Trans Date :";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 63);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 137;
            this.label7.Text = "Ref Trans Id :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnNew);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.dgvPrDetails);
            this.groupBox2.Location = new System.Drawing.Point(12, 149);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(860, 219);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Detail";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(6, 18);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(59, 23);
            this.btnNew.TabIndex = 152;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(72, 18);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(59, 23);
            this.btnDelete.TabIndex = 151;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // dgvPrDetails
            // 
            this.dgvPrDetails.AllowUserToAddRows = false;
            this.dgvPrDetails.AllowUserToDeleteRows = false;
            this.dgvPrDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrDetails.Location = new System.Drawing.Point(6, 47);
            this.dgvPrDetails.Name = "dgvPrDetails";
            this.dgvPrDetails.ReadOnly = true;
            this.dgvPrDetails.Size = new System.Drawing.Size(847, 162);
            this.dgvPrDetails.TabIndex = 150;
            this.dgvPrDetails.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvPrDetails_CellMouseClick);
            this.dgvPrDetails.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPrDetails_CellEndEdit);
            this.dgvPrDetails.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPrDetails_EditingControlShowing);
            this.dgvPrDetails.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvPrDetails_KeyPress);
            // 
            // FormResizeGR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 441);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "FormResizeGR";
            this.Text = "FormPurchaseRequisition";
            this.Load += new System.EventHandler(this.FormResizeGR_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnEdit;
        internal System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.TextBox txtReffTransID;
        internal System.Windows.Forms.DateTimePicker dtTransDate;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.TextBox txtTransNo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        internal System.Windows.Forms.Button btnNew;
        internal System.Windows.Forms.Button btnDelete;
        internal System.Windows.Forms.DataGridView dgvPrDetails;
        //private ISBS_New.Inventory.GoodReceiptNT.CachedRpt_TiketBongkarMuatNT cachedRpt_TiketBongkarMuatNT1;
        private System.Windows.Forms.CheckBox chkPosted;
        private System.Windows.Forms.Label lblposted;

    }
}