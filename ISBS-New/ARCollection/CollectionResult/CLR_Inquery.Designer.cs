namespace ISBS_New.ARCollection.CollectionResult
{
    partial class CLR_Inquery
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
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.btnExit = new MetroFramework.Controls.MetroButton();
            this.btnSearch = new MetroFramework.Controls.MetroButton();
            this.txtSearch = new MetroFramework.Controls.MetroTextBox();
            this.btnSelect = new MetroFramework.Controls.MetroButton();
            this.btnMPrev = new MetroFramework.Controls.MetroButton();
            this.btnPrev = new MetroFramework.Controls.MetroButton();
            this.btnNext = new MetroFramework.Controls.MetroButton();
            this.btnMNext = new MetroFramework.Controls.MetroButton();
            this.btnReset = new MetroFramework.Controls.MetroButton();
            this.grpCustomer = new System.Windows.Forms.GroupBox();
            this.btnCompleted = new System.Windows.Forms.Button();
            this.dgvCLResult = new System.Windows.Forms.DataGridView();
            this.btnOnProgress = new System.Windows.Forms.Button();
            this.lblTotal = new MetroFramework.Controls.MetroLabel();
            this.lblPage = new MetroFramework.Controls.MetroLabel();
            this.txtPage = new MetroFramework.Controls.MetroTextBox();
            this.cmbShow = new System.Windows.Forms.ComboBox();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.cmbStatusCode = new System.Windows.Forms.ComboBox();
            this.cmbCriteria = new System.Windows.Forms.ComboBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.grpCustomer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCLResult)).BeginInit();
            this.SuspendLayout();
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(10, 75);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(43, 19);
            this.metroLabel3.TabIndex = 2;
            this.metroLabel3.Text = "Status";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(10, 48);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(52, 19);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "Criteria";
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(10, 21);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(48, 19);
            this.metroLabel1.TabIndex = 0;
            this.metroLabel1.Text = "Search";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(765, 485);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 24;
            this.btnExit.Text = "Exit";
            this.btnExit.UseSelectable = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(725, 21);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 15;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseSelectable = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtSearch.Lines = new string[0];
            this.txtSearch.Location = new System.Drawing.Point(86, 21);
            this.txtSearch.MaxLength = 32767;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PasswordChar = '\0';
            this.txtSearch.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtSearch.SelectedText = "";
            this.txtSearch.Size = new System.Drawing.Size(270, 20);
            this.txtSearch.TabIndex = 10;
            this.txtSearch.UseSelectable = true;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(684, 485);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 24;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseSelectable = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnMPrev
            // 
            this.btnMPrev.Location = new System.Drawing.Point(10, 437);
            this.btnMPrev.Name = "btnMPrev";
            this.btnMPrev.Size = new System.Drawing.Size(30, 23);
            this.btnMPrev.TabIndex = 19;
            this.btnMPrev.Text = "<<";
            this.btnMPrev.UseSelectable = true;
            this.btnMPrev.Click += new System.EventHandler(this.btnMPrev_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(49, 437);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(30, 23);
            this.btnPrev.TabIndex = 20;
            this.btnPrev.Text = "<";
            this.btnPrev.UseSelectable = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(160, 437);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(30, 23);
            this.btnNext.TabIndex = 21;
            this.btnNext.Text = ">";
            this.btnNext.UseSelectable = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnMNext
            // 
            this.btnMNext.Location = new System.Drawing.Point(199, 437);
            this.btnMNext.Name = "btnMNext";
            this.btnMNext.Size = new System.Drawing.Size(30, 23);
            this.btnMNext.TabIndex = 22;
            this.btnMNext.Text = ">>";
            this.btnMNext.UseSelectable = true;
            this.btnMNext.Click += new System.EventHandler(this.btnMNext_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(725, 50);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 16;
            this.btnReset.Text = "Reset";
            this.btnReset.UseSelectable = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // grpCustomer
            // 
            this.grpCustomer.Controls.Add(this.btnCompleted);
            this.grpCustomer.Controls.Add(this.dgvCLResult);
            this.grpCustomer.Controls.Add(this.btnOnProgress);
            this.grpCustomer.Controls.Add(this.metroLabel3);
            this.grpCustomer.Controls.Add(this.lblTotal);
            this.grpCustomer.Controls.Add(this.metroLabel2);
            this.grpCustomer.Controls.Add(this.lblPage);
            this.grpCustomer.Controls.Add(this.metroLabel1);
            this.grpCustomer.Controls.Add(this.txtPage);
            this.grpCustomer.Controls.Add(this.cmbShow);
            this.grpCustomer.Controls.Add(this.dtFrom);
            this.grpCustomer.Controls.Add(this.dtTo);
            this.grpCustomer.Controls.Add(this.cmbStatusCode);
            this.grpCustomer.Controls.Add(this.cmbCriteria);
            this.grpCustomer.Controls.Add(this.btnExit);
            this.grpCustomer.Controls.Add(this.metroLabel4);
            this.grpCustomer.Controls.Add(this.metroLabel5);
            this.grpCustomer.Controls.Add(this.btnSelect);
            this.grpCustomer.Controls.Add(this.btnSearch);
            this.grpCustomer.Controls.Add(this.btnMPrev);
            this.grpCustomer.Controls.Add(this.txtSearch);
            this.grpCustomer.Controls.Add(this.btnPrev);
            this.grpCustomer.Controls.Add(this.btnReset);
            this.grpCustomer.Controls.Add(this.btnNext);
            this.grpCustomer.Controls.Add(this.btnMNext);
            this.grpCustomer.Location = new System.Drawing.Point(23, 63);
            this.grpCustomer.Name = "grpCustomer";
            this.grpCustomer.Size = new System.Drawing.Size(854, 514);
            this.grpCustomer.TabIndex = 28;
            this.grpCustomer.TabStop = false;
            // 
            // btnCompleted
            // 
            this.btnCompleted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompleted.Location = new System.Drawing.Point(428, 108);
            this.btnCompleted.Name = "btnCompleted";
            this.btnCompleted.Size = new System.Drawing.Size(420, 23);
            this.btnCompleted.TabIndex = 91;
            this.btnCompleted.Text = "Completed";
            this.btnCompleted.UseVisualStyleBackColor = true;
            this.btnCompleted.Click += new System.EventHandler(this.btnCompleted_Click);
            // 
            // dgvCLResult
            // 
            this.dgvCLResult.AllowUserToAddRows = false;
            this.dgvCLResult.AllowUserToDeleteRows = false;
            this.dgvCLResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCLResult.Location = new System.Drawing.Point(6, 133);
            this.dgvCLResult.Name = "dgvCLResult";
            this.dgvCLResult.ReadOnly = true;
            this.dgvCLResult.Size = new System.Drawing.Size(842, 300);
            this.dgvCLResult.TabIndex = 102;
            this.dgvCLResult.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCLResult_CellDoubleClick);
            // 
            // btnOnProgress
            // 
            this.btnOnProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOnProgress.Location = new System.Drawing.Point(6, 108);
            this.btnOnProgress.Name = "btnOnProgress";
            this.btnOnProgress.Size = new System.Drawing.Size(420, 23);
            this.btnOnProgress.TabIndex = 90;
            this.btnOnProgress.Text = "On Progress";
            this.btnOnProgress.UseVisualStyleBackColor = true;
            this.btnOnProgress.Click += new System.EventHandler(this.btnOnProgress_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(669, 438);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(83, 19);
            this.lblTotal.TabIndex = 9;
            this.lblTotal.Text = "Total Rows : ";
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(118, 438);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(14, 19);
            this.lblPage.TabIndex = 8;
            this.lblPage.Text = "/";
            // 
            // txtPage
            // 
            this.txtPage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtPage.Lines = new string[0];
            this.txtPage.Location = new System.Drawing.Point(87, 438);
            this.txtPage.MaxLength = 32767;
            this.txtPage.Name = "txtPage";
            this.txtPage.PasswordChar = '\0';
            this.txtPage.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtPage.SelectedText = "";
            this.txtPage.Size = new System.Drawing.Size(30, 20);
            this.txtPage.TabIndex = 23;
            this.txtPage.UseSelectable = true;
            this.txtPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPage_KeyPress);
            // 
            // cmbShow
            // 
            this.cmbShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShow.FormattingEnabled = true;
            this.cmbShow.Location = new System.Drawing.Point(790, 437);
            this.cmbShow.Name = "cmbShow";
            this.cmbShow.Size = new System.Drawing.Size(50, 21);
            this.cmbShow.TabIndex = 24;
            this.cmbShow.SelectedIndexChanged += new System.EventHandler(this.cmbShow_SelectedIndexChanged);
            this.cmbShow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbShow_KeyPress);
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(506, 21);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(200, 20);
            this.dtFrom.TabIndex = 13;
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(506, 48);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(200, 20);
            this.dtTo.TabIndex = 14;
            // 
            // cmbStatusCode
            // 
            this.cmbStatusCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusCode.FormattingEnabled = true;
            this.cmbStatusCode.Location = new System.Drawing.Point(86, 75);
            this.cmbStatusCode.Name = "cmbStatusCode";
            this.cmbStatusCode.Size = new System.Drawing.Size(270, 21);
            this.cmbStatusCode.TabIndex = 12;
            // 
            // cmbCriteria
            // 
            this.cmbCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCriteria.FormattingEnabled = true;
            this.cmbCriteria.Location = new System.Drawing.Point(86, 48);
            this.cmbCriteria.Name = "cmbCriteria";
            this.cmbCriteria.Size = new System.Drawing.Size(270, 21);
            this.cmbCriteria.TabIndex = 11;
            this.cmbCriteria.SelectedIndexChanged += new System.EventHandler(this.cmbCriteria_SelectedIndexChanged);
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(418, 21);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(72, 19);
            this.metroLabel4.TabIndex = 6;
            this.metroLabel4.Text = "From Date";
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(418, 48);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(55, 19);
            this.metroLabel5.TabIndex = 7;
            this.metroLabel5.Text = "To Date";
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 45000;
            // 
            // CLR_Inquery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.grpCustomer);
            this.Name = "CLR_Inquery";
            this.Text = "Collection Result Inquery";
            this.Load += new System.EventHandler(this.CLR_Inquery_Load);
            this.grpCustomer.ResumeLayout(false);
            this.grpCustomer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCLResult)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroButton btnExit;
        private MetroFramework.Controls.MetroButton btnSearch;
        private MetroFramework.Controls.MetroTextBox txtSearch;
        private MetroFramework.Controls.MetroButton btnSelect;
        private MetroFramework.Controls.MetroButton btnMPrev;
        private MetroFramework.Controls.MetroButton btnPrev;
        private MetroFramework.Controls.MetroButton btnNext;
        private MetroFramework.Controls.MetroButton btnMNext;
        private MetroFramework.Controls.MetroButton btnReset;
        private System.Windows.Forms.GroupBox grpCustomer;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        internal System.Windows.Forms.ComboBox cmbStatusCode;
        internal System.Windows.Forms.ComboBox cmbCriteria;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.DateTimePicker dtTo;
        internal System.Windows.Forms.ComboBox cmbShow;
        private MetroFramework.Controls.MetroTextBox txtPage;
        private MetroFramework.Controls.MetroLabel lblPage;
        private MetroFramework.Controls.MetroLabel lblTotal;
        private System.Windows.Forms.Timer timerRefresh;
        internal System.Windows.Forms.DataGridView dgvCLResult;
        private System.Windows.Forms.Button btnCompleted;
        private System.Windows.Forms.Button btnOnProgress;

    }
}