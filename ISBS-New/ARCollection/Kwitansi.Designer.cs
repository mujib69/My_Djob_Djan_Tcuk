namespace ISBS_New.ARCollection
{
    partial class Kwitansi
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
            this.btnSearchCustomer = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtKwitansiDate = new System.Windows.Forms.DateTimePicker();
            this.txtCustName = new System.Windows.Forms.TextBox();
            this.txtCustId = new System.Windows.Forms.TextBox();
            this.txtKwitansiId = new System.Windows.Forms.TextBox();
            this.btnAddAR = new MetroFramework.Controls.MetroButton();
            this.btnDeleteAR = new MetroFramework.Controls.MetroButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExit = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnEdit = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.dgvDetail = new System.Windows.Forms.DataGridView();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSearchCustomer);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtKwitansiDate);
            this.groupBox1.Controls.Add(this.txtCustName);
            this.groupBox1.Controls.Add(this.txtCustId);
            this.groupBox1.Controls.Add(this.txtKwitansiId);
            this.groupBox1.Location = new System.Drawing.Point(24, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(707, 149);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnSearchCustomer
            // 
            this.btnSearchCustomer.Location = new System.Drawing.Point(300, 73);
            this.btnSearchCustomer.Name = "btnSearchCustomer";
            this.btnSearchCustomer.Size = new System.Drawing.Size(27, 20);
            this.btnSearchCustomer.TabIndex = 8;
            this.btnSearchCustomer.Text = "...";
            this.btnSearchCustomer.UseVisualStyleBackColor = true;
            this.btnSearchCustomer.Click += new System.EventHandler(this.btnSearchCustomer_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Customer Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Customer Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Tanda Terima Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tanda Terima Id";
            // 
            // dtKwitansiDate
            // 
            this.dtKwitansiDate.CustomFormat = "dd/MM/yyyy";
            this.dtKwitansiDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtKwitansiDate.Location = new System.Drawing.Point(140, 47);
            this.dtKwitansiDate.Name = "dtKwitansiDate";
            this.dtKwitansiDate.Size = new System.Drawing.Size(154, 20);
            this.dtKwitansiDate.TabIndex = 3;
            // 
            // txtCustName
            // 
            this.txtCustName.Location = new System.Drawing.Point(140, 100);
            this.txtCustName.Name = "txtCustName";
            this.txtCustName.Size = new System.Drawing.Size(284, 20);
            this.txtCustName.TabIndex = 2;
            this.txtCustName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtCustName_MouseDown);
            // 
            // txtCustId
            // 
            this.txtCustId.Location = new System.Drawing.Point(140, 73);
            this.txtCustId.Name = "txtCustId";
            this.txtCustId.Size = new System.Drawing.Size(154, 20);
            this.txtCustId.TabIndex = 1;
            this.txtCustId.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtCustId_MouseDown);
            // 
            // txtKwitansiId
            // 
            this.txtKwitansiId.Location = new System.Drawing.Point(140, 20);
            this.txtKwitansiId.Name = "txtKwitansiId";
            this.txtKwitansiId.Size = new System.Drawing.Size(154, 20);
            this.txtKwitansiId.TabIndex = 0;
            // 
            // btnAddAR
            // 
            this.btnAddAR.Location = new System.Drawing.Point(16, 19);
            this.btnAddAR.Name = "btnAddAR";
            this.btnAddAR.Size = new System.Drawing.Size(75, 23);
            this.btnAddAR.TabIndex = 1;
            this.btnAddAR.Text = "Add";
            this.btnAddAR.UseSelectable = true;
            this.btnAddAR.Click += new System.EventHandler(this.btnAddAR_Click);
            // 
            // btnDeleteAR
            // 
            this.btnDeleteAR.Location = new System.Drawing.Point(97, 19);
            this.btnDeleteAR.Name = "btnDeleteAR";
            this.btnDeleteAR.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteAR.TabIndex = 2;
            this.btnDeleteAR.Text = "Delete";
            this.btnDeleteAR.UseSelectable = true;
            this.btnDeleteAR.Click += new System.EventHandler(this.btnDeleteAR_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnExit);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnEdit);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.dgvDetail);
            this.groupBox2.Controls.Add(this.btnAddAR);
            this.groupBox2.Controls.Add(this.btnDeleteAR);
            this.groupBox2.Location = new System.Drawing.Point(24, 219);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(707, 297);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(622, 258);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseSelectable = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(460, 258);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(379, 258);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseSelectable = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(541, 258);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgvDetail
            // 
            this.dgvDetail.AllowUserToAddRows = false;
            this.dgvDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetail.Location = new System.Drawing.Point(16, 49);
            this.dgvDetail.Name = "dgvDetail";
            this.dgvDetail.Size = new System.Drawing.Size(681, 203);
            this.dgvDetail.TabIndex = 3;
            this.dgvDetail.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvDetail_CellMouseDown);
            // 
            // Kwitansi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 539);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Kwitansi";
            this.Text = "Tanda Terima";
            this.Load += new System.EventHandler(this.Kwitansi_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtCustName;
        private System.Windows.Forms.TextBox txtCustId;
        private System.Windows.Forms.TextBox txtKwitansiId;
        private System.Windows.Forms.DateTimePicker dtKwitansiDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearchCustomer;
        private MetroFramework.Controls.MetroButton btnAddAR;
        private MetroFramework.Controls.MetroButton btnDeleteAR;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvDetail;
        private MetroFramework.Controls.MetroButton btnEdit;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnExit;
    }
}