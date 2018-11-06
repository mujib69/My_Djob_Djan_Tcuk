namespace ISBS_New.PopUp.PurchaseRequisition
{
    partial class PRNo
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dgvApproval = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvDetail = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblPRApproved = new System.Windows.Forms.Label();
            this.lblPRStatus = new System.Windows.Forms.Label();
            this.lblPRType = new System.Windows.Forms.Label();
            this.dtPRDate = new System.Windows.Forms.DateTimePicker();
            this.lblPRNumber = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApproval)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetail)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonExit);
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(23, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(658, 405);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(572, 345);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 2;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(6, 134);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(645, 205);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvApproval);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(637, 179);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Approval";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvApproval
            // 
            this.dgvApproval.AllowUserToAddRows = false;
            this.dgvApproval.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvApproval.Location = new System.Drawing.Point(0, 0);
            this.dgvApproval.Name = "dgvApproval";
            this.dgvApproval.Size = new System.Drawing.Size(637, 179);
            this.dgvApproval.TabIndex = 0;
            this.dgvApproval.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvApproval_CellMouseDown);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvDetail);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(637, 179);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Detail";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvDetail
            // 
            this.dgvDetail.AllowUserToAddRows = false;
            this.dgvDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetail.Location = new System.Drawing.Point(0, 0);
            this.dgvDetail.Name = "dgvDetail";
            this.dgvDetail.Size = new System.Drawing.Size(637, 179);
            this.dgvDetail.TabIndex = 0;
            this.dgvDetail.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvDetail_CellMouseDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblPRApproved);
            this.groupBox2.Controls.Add(this.lblPRStatus);
            this.groupBox2.Controls.Add(this.lblPRType);
            this.groupBox2.Controls.Add(this.dtPRDate);
            this.groupBox2.Controls.Add(this.lblPRNumber);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(6, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(645, 119);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // lblPRApproved
            // 
            this.lblPRApproved.AutoSize = true;
            this.lblPRApproved.Location = new System.Drawing.Point(423, 52);
            this.lblPRApproved.Name = "lblPRApproved";
            this.lblPRApproved.Size = new System.Drawing.Size(78, 13);
            this.lblPRApproved.TabIndex = 9;
            this.lblPRApproved.Text = "lblPRApproved";
            // 
            // lblPRStatus
            // 
            this.lblPRStatus.AutoSize = true;
            this.lblPRStatus.Location = new System.Drawing.Point(423, 26);
            this.lblPRStatus.Name = "lblPRStatus";
            this.lblPRStatus.Size = new System.Drawing.Size(62, 13);
            this.lblPRStatus.TabIndex = 8;
            this.lblPRStatus.Text = "lblPRStatus";
            // 
            // lblPRType
            // 
            this.lblPRType.AutoSize = true;
            this.lblPRType.Location = new System.Drawing.Point(86, 83);
            this.lblPRType.Name = "lblPRType";
            this.lblPRType.Size = new System.Drawing.Size(56, 13);
            this.lblPRType.TabIndex = 7;
            this.lblPRType.Text = "lblPRType";
            // 
            // dtPRDate
            // 
            this.dtPRDate.CustomFormat = "dd/MM/yyyy";
            this.dtPRDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPRDate.Location = new System.Drawing.Point(87, 20);
            this.dtPRDate.Name = "dtPRDate";
            this.dtPRDate.Size = new System.Drawing.Size(181, 20);
            this.dtPRDate.TabIndex = 6;
            // 
            // lblPRNumber
            // 
            this.lblPRNumber.AutoSize = true;
            this.lblPRNumber.Location = new System.Drawing.Point(85, 52);
            this.lblPRNumber.Name = "lblPRNumber";
            this.lblPRNumber.Size = new System.Drawing.Size(69, 13);
            this.lblPRNumber.TabIndex = 5;
            this.lblPRNumber.Text = "lblPRNumber";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(335, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "PR Approved   :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(335, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "PR Status        :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "PR Type       :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "PR Number   :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "PR Date        :";
            // 
            // PRNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 493);
            this.Controls.Add(this.groupBox1);
            this.Name = "PRNo";
            this.Text = "Purchase Requisition";
            this.Load += new System.EventHandler(this.PRNo_Load);
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvApproval)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetail)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblPRApproved;
        private System.Windows.Forms.Label lblPRStatus;
        private System.Windows.Forms.Label lblPRType;
        private System.Windows.Forms.DateTimePicker dtPRDate;
        private System.Windows.Forms.Label lblPRNumber;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dgvApproval;
        private System.Windows.Forms.DataGridView dgvDetail;
    }
}