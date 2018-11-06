namespace ISBS_New.ARCollection.Collection
{
    partial class CollectionTaskList
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnCollector = new MetroFramework.Controls.MetroButton();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.TabInvoice = new MetroFramework.Controls.MetroTabPage();
            this.btnInvoiceCompleted = new MetroFramework.Controls.MetroButton();
            this.btnInvoiceOnProgress = new MetroFramework.Controls.MetroButton();
            this.chkDueDate = new MetroFramework.Controls.MetroCheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkInvoiceDate = new MetroFramework.Controls.MetroCheckBox();
            this.tbxInvoiceTo = new System.Windows.Forms.TextBox();
            this.btnReset = new MetroFramework.Controls.MetroButton();
            this.to = new System.Windows.Forms.Label();
            this.btnSearch = new MetroFramework.Controls.MetroButton();
            this.btnCustSearch = new System.Windows.Forms.Button();
            this.tbxInvoiceFrom = new System.Windows.Forms.TextBox();
            this.tbxCustId = new System.Windows.Forms.TextBox();
            this.dtDueTo = new System.Windows.Forms.DateTimePicker();
            this.dtDueFrom = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxCustName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Customer = new System.Windows.Forms.Label();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.TabCollectionLog = new MetroFramework.Controls.MetroTabPage();
            this.btnCollectionCompleted = new MetroFramework.Controls.MetroButton();
            this.btnCollectionOnProgress = new MetroFramework.Controls.MetroButton();
            this.label9 = new System.Windows.Forms.Label();
            this.dtToLog = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.dtFromLog = new System.Windows.Forms.DateTimePicker();
            this.btnReset2 = new MetroFramework.Controls.MetroButton();
            this.btnSearch2 = new MetroFramework.Controls.MetroButton();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbCrit = new System.Windows.Forms.ComboBox();
            this.tbxSearhFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.btnExit = new MetroFramework.Controls.MetroButton();
            this.btnSelect = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.metroTabControl1.SuspendLayout();
            this.TabInvoice.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.TabCollectionLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 144);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(940, 250);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // btnCollector
            // 
            this.btnCollector.Location = new System.Drawing.Point(17, 19);
            this.btnCollector.Name = "btnCollector";
            this.btnCollector.Size = new System.Drawing.Size(130, 23);
            this.btnCollector.TabIndex = 1;
            this.btnCollector.Text = "Send to Collection";
            this.btnCollector.UseSelectable = true;
            this.btnCollector.Click += new System.EventHandler(this.btnCollector_Click);
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.metroTabControl1.Controls.Add(this.TabInvoice);
            this.metroTabControl1.Controls.Add(this.TabCollectionLog);
            this.metroTabControl1.Location = new System.Drawing.Point(23, 63);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 1;
            this.metroTabControl1.Size = new System.Drawing.Size(954, 439);
            this.metroTabControl1.TabIndex = 0;
            this.metroTabControl1.UseSelectable = true;
            this.metroTabControl1.SelectedIndexChanged += new System.EventHandler(this.metroTabControl1_SelectedIndexChanged);
            // 
            // TabInvoice
            // 
            this.TabInvoice.BackColor = System.Drawing.SystemColors.Window;
            this.TabInvoice.Controls.Add(this.btnInvoiceCompleted);
            this.TabInvoice.Controls.Add(this.btnInvoiceOnProgress);
            this.TabInvoice.Controls.Add(this.chkDueDate);
            this.TabInvoice.Controls.Add(this.label1);
            this.TabInvoice.Controls.Add(this.label3);
            this.TabInvoice.Controls.Add(this.chkInvoiceDate);
            this.TabInvoice.Controls.Add(this.tbxInvoiceTo);
            this.TabInvoice.Controls.Add(this.btnReset);
            this.TabInvoice.Controls.Add(this.to);
            this.TabInvoice.Controls.Add(this.btnSearch);
            this.TabInvoice.Controls.Add(this.btnCustSearch);
            this.TabInvoice.Controls.Add(this.tbxInvoiceFrom);
            this.TabInvoice.Controls.Add(this.tbxCustId);
            this.TabInvoice.Controls.Add(this.dtDueTo);
            this.TabInvoice.Controls.Add(this.dtDueFrom);
            this.TabInvoice.Controls.Add(this.label5);
            this.TabInvoice.Controls.Add(this.tbxCustName);
            this.TabInvoice.Controls.Add(this.groupBox1);
            this.TabInvoice.Controls.Add(this.label6);
            this.TabInvoice.Controls.Add(this.dataGridView1);
            this.TabInvoice.Controls.Add(this.Customer);
            this.TabInvoice.Controls.Add(this.dtTo);
            this.TabInvoice.Controls.Add(this.label4);
            this.TabInvoice.Controls.Add(this.dtFrom);
            this.TabInvoice.HorizontalScrollbarBarColor = true;
            this.TabInvoice.HorizontalScrollbarHighlightOnWheel = false;
            this.TabInvoice.HorizontalScrollbarSize = 10;
            this.TabInvoice.Location = new System.Drawing.Point(4, 41);
            this.TabInvoice.Name = "TabInvoice";
            this.TabInvoice.Size = new System.Drawing.Size(946, 394);
            this.TabInvoice.TabIndex = 0;
            this.TabInvoice.Text = "Invoice";
            this.TabInvoice.VerticalScrollbarBarColor = true;
            this.TabInvoice.VerticalScrollbarHighlightOnWheel = false;
            this.TabInvoice.VerticalScrollbarSize = 10;
            // 
            // btnInvoiceCompleted
            // 
            this.btnInvoiceCompleted.Location = new System.Drawing.Point(473, 115);
            this.btnInvoiceCompleted.Name = "btnInvoiceCompleted";
            this.btnInvoiceCompleted.Size = new System.Drawing.Size(470, 23);
            this.btnInvoiceCompleted.TabIndex = 29;
            this.btnInvoiceCompleted.Text = "Invoice Completed";
            this.btnInvoiceCompleted.UseCustomBackColor = true;
            this.btnInvoiceCompleted.UseCustomForeColor = true;
            this.btnInvoiceCompleted.UseSelectable = true;
            this.btnInvoiceCompleted.Click += new System.EventHandler(this.btnInvoiceCompleted_Click);
            // 
            // btnInvoiceOnProgress
            // 
            this.btnInvoiceOnProgress.Location = new System.Drawing.Point(3, 115);
            this.btnInvoiceOnProgress.Name = "btnInvoiceOnProgress";
            this.btnInvoiceOnProgress.Size = new System.Drawing.Size(464, 23);
            this.btnInvoiceOnProgress.TabIndex = 28;
            this.btnInvoiceOnProgress.Text = "Invoice On Progress";
            this.btnInvoiceOnProgress.UseCustomBackColor = true;
            this.btnInvoiceOnProgress.UseCustomForeColor = true;
            this.btnInvoiceOnProgress.UseSelectable = true;
            this.btnInvoiceOnProgress.Click += new System.EventHandler(this.btnInvoiceOnProgress_Click);
            // 
            // chkDueDate
            // 
            this.chkDueDate.AutoSize = true;
            this.chkDueDate.Location = new System.Drawing.Point(398, 89);
            this.chkDueDate.Name = "chkDueDate";
            this.chkDueDate.Size = new System.Drawing.Size(69, 15);
            this.chkDueDate.TabIndex = 27;
            this.chkDueDate.Text = "Use Date";
            this.chkDueDate.UseSelectable = true;
            this.chkDueDate.CheckedChanged += new System.EventHandler(this.chkDueDate_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "to";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(227, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "to";
            // 
            // chkInvoiceDate
            // 
            this.chkInvoiceDate.AutoSize = true;
            this.chkInvoiceDate.Location = new System.Drawing.Point(398, 64);
            this.chkInvoiceDate.Name = "chkInvoiceDate";
            this.chkInvoiceDate.Size = new System.Drawing.Size(69, 15);
            this.chkInvoiceDate.TabIndex = 4;
            this.chkInvoiceDate.Text = "Use Date";
            this.chkInvoiceDate.UseSelectable = true;
            this.chkInvoiceDate.CheckedChanged += new System.EventHandler(this.chkInvoiceDate_CheckedChanged);
            // 
            // tbxInvoiceTo
            // 
            this.tbxInvoiceTo.Location = new System.Drawing.Point(249, 35);
            this.tbxInvoiceTo.Name = "tbxInvoiceTo";
            this.tbxInvoiceTo.Size = new System.Drawing.Size(140, 20);
            this.tbxInvoiceTo.TabIndex = 21;
            this.tbxInvoiceTo.Leave += new System.EventHandler(this.tbxInvoiceTo_Leave);
            this.tbxInvoiceTo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxInvoiceTo_KeyPress);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(398, 36);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 14;
            this.btnReset.Text = "Reset";
            this.btnReset.UseSelectable = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // to
            // 
            this.to.AutoSize = true;
            this.to.Location = new System.Drawing.Point(227, 65);
            this.to.Name = "to";
            this.to.Size = new System.Drawing.Size(16, 13);
            this.to.TabIndex = 21;
            this.to.Text = "to";
            this.to.Click += new System.EventHandler(this.to_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(398, 7);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseSelectable = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCustSearch
            // 
            this.btnCustSearch.AutoSize = true;
            this.btnCustSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCustSearch.Location = new System.Drawing.Point(363, 7);
            this.btnCustSearch.Name = "btnCustSearch";
            this.btnCustSearch.Size = new System.Drawing.Size(26, 23);
            this.btnCustSearch.TabIndex = 21;
            this.btnCustSearch.Text = "...";
            this.btnCustSearch.UseVisualStyleBackColor = true;
            this.btnCustSearch.Click += new System.EventHandler(this.btnCustSearch_Click);
            // 
            // tbxInvoiceFrom
            // 
            this.tbxInvoiceFrom.Location = new System.Drawing.Point(81, 35);
            this.tbxInvoiceFrom.Name = "tbxInvoiceFrom";
            this.tbxInvoiceFrom.Size = new System.Drawing.Size(140, 20);
            this.tbxInvoiceFrom.TabIndex = 19;
            this.tbxInvoiceFrom.Leave += new System.EventHandler(this.tbxInvoiceFrom_Leave);
            this.tbxInvoiceFrom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxInvoiceFrom_KeyPress);
            // 
            // tbxCustId
            // 
            this.tbxCustId.Location = new System.Drawing.Point(81, 9);
            this.tbxCustId.Name = "tbxCustId";
            this.tbxCustId.ReadOnly = true;
            this.tbxCustId.Size = new System.Drawing.Size(80, 20);
            this.tbxCustId.TabIndex = 9;
            // 
            // dtDueTo
            // 
            this.dtDueTo.CustomFormat = "dd/MM/yyyy";
            this.dtDueTo.Enabled = false;
            this.dtDueTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDueTo.Location = new System.Drawing.Point(249, 87);
            this.dtDueTo.Name = "dtDueTo";
            this.dtDueTo.Size = new System.Drawing.Size(140, 20);
            this.dtDueTo.TabIndex = 23;
            // 
            // dtDueFrom
            // 
            this.dtDueFrom.CustomFormat = "dd/MM/yyyy";
            this.dtDueFrom.Enabled = false;
            this.dtDueFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtDueFrom.Location = new System.Drawing.Point(81, 87);
            this.dtDueFrom.Name = "dtDueFrom";
            this.dtDueFrom.Size = new System.Drawing.Size(140, 20);
            this.dtDueFrom.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Invoice No";
            // 
            // tbxCustName
            // 
            this.tbxCustName.Location = new System.Drawing.Point(167, 9);
            this.tbxCustName.Name = "tbxCustName";
            this.tbxCustName.ReadOnly = true;
            this.tbxCustName.Size = new System.Drawing.Size(190, 20);
            this.tbxCustName.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCollector);
            this.groupBox1.Location = new System.Drawing.Point(508, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 55);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected Invoice";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Due Date";
            // 
            // Customer
            // 
            this.Customer.AutoSize = true;
            this.Customer.Location = new System.Drawing.Point(12, 12);
            this.Customer.Name = "Customer";
            this.Customer.Size = new System.Drawing.Size(51, 13);
            this.Customer.TabIndex = 5;
            this.Customer.Text = "Customer";
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Enabled = false;
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(249, 61);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(140, 20);
            this.dtTo.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Invoice Date";
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy";
            this.dtFrom.Enabled = false;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(81, 61);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(140, 20);
            this.dtFrom.TabIndex = 10;
            // 
            // TabCollectionLog
            // 
            this.TabCollectionLog.Controls.Add(this.btnCollectionCompleted);
            this.TabCollectionLog.Controls.Add(this.btnCollectionOnProgress);
            this.TabCollectionLog.Controls.Add(this.label9);
            this.TabCollectionLog.Controls.Add(this.dtToLog);
            this.TabCollectionLog.Controls.Add(this.label8);
            this.TabCollectionLog.Controls.Add(this.dtFromLog);
            this.TabCollectionLog.Controls.Add(this.btnReset2);
            this.TabCollectionLog.Controls.Add(this.btnSearch2);
            this.TabCollectionLog.Controls.Add(this.label7);
            this.TabCollectionLog.Controls.Add(this.cmbCrit);
            this.TabCollectionLog.Controls.Add(this.tbxSearhFilter);
            this.TabCollectionLog.Controls.Add(this.label2);
            this.TabCollectionLog.Controls.Add(this.dataGridView2);
            this.TabCollectionLog.HorizontalScrollbarBarColor = true;
            this.TabCollectionLog.HorizontalScrollbarHighlightOnWheel = false;
            this.TabCollectionLog.HorizontalScrollbarSize = 10;
            this.TabCollectionLog.Location = new System.Drawing.Point(4, 41);
            this.TabCollectionLog.Name = "TabCollectionLog";
            this.TabCollectionLog.Size = new System.Drawing.Size(946, 394);
            this.TabCollectionLog.TabIndex = 1;
            this.TabCollectionLog.Text = "| Collection Log";
            this.TabCollectionLog.VerticalScrollbarBarColor = true;
            this.TabCollectionLog.VerticalScrollbarHighlightOnWheel = false;
            this.TabCollectionLog.VerticalScrollbarSize = 10;
            // 
            // btnCollectionCompleted
            // 
            this.btnCollectionCompleted.Location = new System.Drawing.Point(473, 68);
            this.btnCollectionCompleted.Name = "btnCollectionCompleted";
            this.btnCollectionCompleted.Size = new System.Drawing.Size(470, 23);
            this.btnCollectionCompleted.TabIndex = 31;
            this.btnCollectionCompleted.Text = "Collection Completed";
            this.btnCollectionCompleted.UseCustomBackColor = true;
            this.btnCollectionCompleted.UseCustomForeColor = true;
            this.btnCollectionCompleted.UseSelectable = true;
            this.btnCollectionCompleted.Click += new System.EventHandler(this.btnCollectionCompleted_Click);
            // 
            // btnCollectionOnProgress
            // 
            this.btnCollectionOnProgress.Location = new System.Drawing.Point(3, 68);
            this.btnCollectionOnProgress.Name = "btnCollectionOnProgress";
            this.btnCollectionOnProgress.Size = new System.Drawing.Size(464, 23);
            this.btnCollectionOnProgress.TabIndex = 30;
            this.btnCollectionOnProgress.Text = "Collection On Progress";
            this.btnCollectionOnProgress.UseCustomBackColor = true;
            this.btnCollectionOnProgress.UseCustomForeColor = true;
            this.btnCollectionOnProgress.UseSelectable = true;
            this.btnCollectionOnProgress.Click += new System.EventHandler(this.btnCollectionOnProgress_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Location = new System.Drawing.Point(330, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "Date To";
            // 
            // dtToLog
            // 
            this.dtToLog.CustomFormat = "dd/MM/yyyy";
            this.dtToLog.Enabled = false;
            this.dtToLog.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtToLog.Location = new System.Drawing.Point(399, 37);
            this.dtToLog.Name = "dtToLog";
            this.dtToLog.Size = new System.Drawing.Size(223, 20);
            this.dtToLog.TabIndex = 28;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.Location = new System.Drawing.Point(330, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Date From";
            // 
            // dtFromLog
            // 
            this.dtFromLog.CustomFormat = "dd/MM/yyyy";
            this.dtFromLog.Enabled = false;
            this.dtFromLog.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFromLog.Location = new System.Drawing.Point(399, 8);
            this.dtFromLog.Name = "dtFromLog";
            this.dtFromLog.Size = new System.Drawing.Size(223, 20);
            this.dtFromLog.TabIndex = 26;
            // 
            // btnReset2
            // 
            this.btnReset2.Location = new System.Drawing.Point(641, 36);
            this.btnReset2.Name = "btnReset2";
            this.btnReset2.Size = new System.Drawing.Size(75, 23);
            this.btnReset2.TabIndex = 25;
            this.btnReset2.Text = "Reset";
            this.btnReset2.UseSelectable = true;
            this.btnReset2.Click += new System.EventHandler(this.btnReset2_Click);
            // 
            // btnSearch2
            // 
            this.btnSearch2.Location = new System.Drawing.Point(641, 7);
            this.btnSearch2.Name = "btnSearch2";
            this.btnSearch2.Size = new System.Drawing.Size(75, 23);
            this.btnSearch2.TabIndex = 24;
            this.btnSearch2.Text = "Search";
            this.btnSearch2.UseSelectable = true;
            this.btnSearch2.Click += new System.EventHandler(this.metroButton2_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label7.Location = new System.Drawing.Point(12, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Criteria";
            // 
            // cmbCrit
            // 
            this.cmbCrit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCrit.FormattingEnabled = true;
            this.cmbCrit.Location = new System.Drawing.Point(81, 35);
            this.cmbCrit.Name = "cmbCrit";
            this.cmbCrit.Size = new System.Drawing.Size(223, 21);
            this.cmbCrit.TabIndex = 22;
            this.cmbCrit.SelectedIndexChanged += new System.EventHandler(this.cmbCrit_SelectedIndexChanged);
            // 
            // tbxSearhFilter
            // 
            this.tbxSearhFilter.Location = new System.Drawing.Point(81, 9);
            this.tbxSearhFilter.Name = "tbxSearhFilter";
            this.tbxSearhFilter.Size = new System.Drawing.Size(223, 20);
            this.tbxSearhFilter.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(12, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Search";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(3, 97);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.Size = new System.Drawing.Size(940, 297);
            this.dataGridView2.TabIndex = 2;
            this.dataGridView2.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellDoubleClick);
            this.dataGridView2.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView2_CellFormatting);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(895, 508);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseSelectable = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(814, 508);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 5;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseSelectable = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // CollectionTaskList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 544);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.metroTabControl1);
            this.Name = "CollectionTaskList";
            this.Resizable = false;
            this.Text = "Task List AR Outstanding";
            this.Load += new System.EventHandler(this.CollectionTaskList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.metroTabControl1.ResumeLayout(false);
            this.TabInvoice.ResumeLayout(false);
            this.TabInvoice.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.TabCollectionLog.ResumeLayout(false);
            this.TabCollectionLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage TabInvoice;
        private MetroFramework.Controls.MetroTabPage TabCollectionLog;
        private MetroFramework.Controls.MetroButton btnCollector;
        private MetroFramework.Controls.MetroButton btnExit;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Label Customer;
        private MetroFramework.Controls.MetroButton btnSearch;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtTo;
        private MetroFramework.Controls.MetroButton btnReset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbxInvoiceFrom;
        private System.Windows.Forms.TextBox tbxCustName;
        private System.Windows.Forms.TextBox tbxCustId;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbxInvoiceTo;
        private System.Windows.Forms.DateTimePicker dtDueTo;
        private System.Windows.Forms.DateTimePicker dtDueFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label to;
        private System.Windows.Forms.Button btnCustSearch;
        private System.Windows.Forms.Label label3;
        private MetroFramework.Controls.MetroCheckBox chkDueDate;
        private MetroFramework.Controls.MetroCheckBox chkInvoiceDate;
        private System.Windows.Forms.TextBox tbxSearhFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbCrit;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dtToLog;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtFromLog;
        private MetroFramework.Controls.MetroButton btnReset2;
        private MetroFramework.Controls.MetroButton btnSearch2;
        private MetroFramework.Controls.MetroButton btnSelect;
        private MetroFramework.Controls.MetroButton btnInvoiceCompleted;
        private MetroFramework.Controls.MetroButton btnInvoiceOnProgress;
        private MetroFramework.Controls.MetroButton btnCollectionCompleted;
        private MetroFramework.Controls.MetroButton btnCollectionOnProgress;
    }
}