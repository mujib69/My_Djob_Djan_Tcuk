﻿namespace ISBS_New.TaskList.Purchase.NotaPurchaseParked
{
    partial class TaskListNotaPurchaseParked
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
            this.components = new System.ComponentModel.Container();
            this.btnCompleted = new System.Windows.Forms.Button();
            this.btnOnProgress = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.btnReset = new System.Windows.Forms.Button();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.cmbCriteria = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblPage = new System.Windows.Forms.Label();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.btnMNext = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnMPrev = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.dgvNotaPurchaseParked = new System.Windows.Forms.DataGridView();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.grpCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotaPurchaseParked)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCompleted
            // 
            this.btnCompleted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompleted.Location = new System.Drawing.Point(428, 91);
            this.btnCompleted.Name = "btnCompleted";
            this.btnCompleted.Size = new System.Drawing.Size(420, 23);
            this.btnCompleted.TabIndex = 89;
            this.btnCompleted.Text = "Completed";
            this.btnCompleted.UseVisualStyleBackColor = true;
            this.btnCompleted.Click += new System.EventHandler(this.btnCompleted_Click);
            // 
            // btnOnProgress
            // 
            this.btnOnProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOnProgress.Location = new System.Drawing.Point(6, 91);
            this.btnOnProgress.Name = "btnOnProgress";
            this.btnOnProgress.Size = new System.Drawing.Size(420, 23);
            this.btnOnProgress.TabIndex = 88;
            this.btnOnProgress.Text = "On Progress";
            this.btnOnProgress.UseVisualStyleBackColor = true;
            this.btnOnProgress.Click += new System.EventHandler(this.btnOnProgress_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(682, 483);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(80, 25);
            this.btnSelect.TabIndex = 82;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(489, 15);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(200, 20);
            this.dtFrom.TabIndex = 81;
            // 
            // btnReset
            // 
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(709, 38);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(80, 25);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(489, 40);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(200, 20);
            this.dtTo.TabIndex = 80;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(424, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 79;
            this.label3.Text = "To Date ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(414, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 78;
            this.label4.Text = "From Date ";
            // 
            // btnSearch
            // 
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(709, 13);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 25);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.btnCompleted);
            this.grpCustomer.Controls.Add(this.btnOnProgress);
            this.grpCustomer.Controls.Add(this.btnSelect);
            this.grpCustomer.Controls.Add(this.dtFrom);
            this.grpCustomer.Controls.Add(this.btnReset);
            this.grpCustomer.Controls.Add(this.dtTo);
            this.grpCustomer.Controls.Add(this.label3);
            this.grpCustomer.Controls.Add(this.btnExit);
            this.grpCustomer.Controls.Add(this.label4);
            this.grpCustomer.Controls.Add(this.btnSearch);
            this.grpCustomer.Controls.Add(this.cmbCriteria);
            this.grpCustomer.Controls.Add(this.Label1);
            this.grpCustomer.Controls.Add(this.txtSearch);
            this.grpCustomer.Controls.Add(this.label2);
            this.grpCustomer.Controls.Add(this.lblTotal);
            this.grpCustomer.Controls.Add(this.txtPage);
            this.grpCustomer.Controls.Add(this.lblPage);
            this.grpCustomer.Controls.Add(this.cmbShow);
            this.grpCustomer.Controls.Add(this.btnMNext);
            this.grpCustomer.Controls.Add(this.btnNext);
            this.grpCustomer.Controls.Add(this.btnMPrev);
            this.grpCustomer.Controls.Add(this.btnPrev);
            this.grpCustomer.Controls.Add(this.dgvNotaPurchaseParked);
            this.grpCustomer.Location = new System.Drawing.Point(23, 63);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(854, 514);
            this.grpCustomer.TabIndex = 88;
            this.grpCustomer.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(768, 483);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(80, 25);
            this.btnExit.TabIndex = 81;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cmbCriteria
            // 
            this.cmbCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCriteria.FormattingEnabled = true;
            this.cmbCriteria.Location = new System.Drawing.Point(70, 40);
            this.cmbCriteria.Name = "cmbCriteria";
            this.cmbCriteria.Size = new System.Drawing.Size(270, 21);
            this.cmbCriteria.TabIndex = 2;
            this.cmbCriteria.SelectedIndexChanged += new System.EventHandler(this.cmbCriteria_SelectedIndexChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(13, 44);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(39, 13);
            this.Label1.TabIndex = 37;
            this.Label1.Text = "Criteria";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(70, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(270, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 36;
            this.label2.Text = "Search";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(679, 430);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 30;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(78, 426);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 14;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(114, 430);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(15, 13);
            this.lblPage.TabIndex = 9;
            this.lblPage.Text = "/ ";
            // 
            // cmbShow
            // 
            this.cmbShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(798, 426);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(50, 21);
            this.cmbShow.TabIndex = 17;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            this.cmbShow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbShow_KeyPress);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(200, 424);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 25);
            this.btnMNext.TabIndex = 16;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseVisualStyleBackColor = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(164, 424);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(30, 25);
            this.btnNext.TabIndex = 15;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(6, 424);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(30, 25);
            this.btnMPrev.TabIndex = 12;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseVisualStyleBackColor = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(42, 424);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(30, 25);
            this.btnPrev.TabIndex = 13;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // dgvNotaPurchaseParked
            // 
            this.dgvNotaPurchaseParked.AllowUserToAddRows = false;
            this.dgvNotaPurchaseParked.AllowUserToDeleteRows = false;
            this.dgvNotaPurchaseParked.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNotaPurchaseParked.Location = new System.Drawing.Point(6, 120);
            this.dgvNotaPurchaseParked.Name = "dgvNotaPurchaseParked";
            this.dgvNotaPurchaseParked.ReadOnly = true;
            this.dgvNotaPurchaseParked.Size = new System.Drawing.Size(842, 300);
            this.dgvNotaPurchaseParked.TabIndex = 5;
            this.dgvNotaPurchaseParked.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNotaPurchaseParked_CellDoubleClick);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 45000;
            // 
            // TaskListNotaPurchaseParked
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 589);
            this.Controls.Add(this.grpCustomer);
            this.Name = "TaskListNotaPurchaseParked";
            this.Text = "TaskListNotaPurchaseParked";
            this.Load += new System.EventHandler(this.TaskListNotaPurchaseParked_Load);
            this.Shown += new System.EventHandler(this.TaskListNotaPurchaseParked_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TaskListNotaPurchaseParked_FormClosed);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotaPurchaseParked)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCompleted;
        private System.Windows.Forms.Button btnOnProgress;
        internal System.Windows.Forms.Button btnSelect;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.Button btnReset;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.GroupBox grpCustomer;
        internal System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.ComboBox cmbCriteria;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox txtSearch;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label lblTotal;
        internal System.Windows.Forms.TextBox txtPage;
        internal System.Windows.Forms.Label lblPage;
        internal System.Windows.Forms.ComboBox cmbShow;
        internal System.Windows.Forms.Button btnMNext;
        internal System.Windows.Forms.Button btnNext;
        internal System.Windows.Forms.Button btnMPrev;
        internal System.Windows.Forms.Button btnPrev;
        internal System.Windows.Forms.DataGridView dgvNotaPurchaseParked;
        private System.Windows.Forms.Timer timerRefresh;
    }
}