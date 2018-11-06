namespace ISBS_New.CashAndBank.BankReconcile
{
    partial class BankReconcile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.btnReset = new System.Windows.Forms.Button();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbCriteria = new System.Windows.Forms.ComboBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grpReconcile = new System.Windows.Forms.GroupBox();
            this.grpBankStatment = new System.Windows.Forms.GroupBox();
            this.dgvBankStatement = new System.Windows.Forms.DataGridView();
            this.grpBankSystem = new System.Windows.Forms.GroupBox();
            this.dgvBankSystem = new System.Windows.Forms.DataGridView();
            this.cmbBankGroup = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grpSearch.SuspendLayout();
            this.grpReconcile.SuspendLayout();
            this.grpBankStatment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankStatement)).BeginInit();
            this.grpBankSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankSystem)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSearch
            // 
            this.grpSearch.Controls.Add(this.dtFrom);
            this.grpSearch.Controls.Add(this.btnReset);
            this.grpSearch.Controls.Add(this.dtTo);
            this.grpSearch.Controls.Add(this.label3);
            this.grpSearch.Controls.Add(this.label4);
            this.grpSearch.Controls.Add(this.btnSearch);
            this.grpSearch.Controls.Add(this.cmbCriteria);
            this.grpSearch.Controls.Add(this.Label1);
            this.grpSearch.Controls.Add(this.txtSearch);
            this.grpSearch.Controls.Add(this.label2);
            this.grpSearch.Location = new System.Drawing.Point(10, 64);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(514, 76);
            this.grpSearch.TabIndex = 0;
            this.grpSearch.TabStop = false;
            this.grpSearch.Text = "Search";
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "dd/MM/yyyy";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(325, 19);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(101, 20);
            this.dtFrom.TabIndex = 91;
            // 
            // btnReset
            // 
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(432, 46);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 85;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "dd/MM/yyyy";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(325, 45);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(101, 20);
            this.dtTo.TabIndex = 90;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(260, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 89;
            this.label3.Text = "To Date ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(250, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 88;
            this.label4.Text = "From Date ";
            // 
            // btnSearch
            // 
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(432, 19);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 84;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbCriteria
            // 
            this.cmbCriteria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCriteria.FormattingEnabled = true;
            this.cmbCriteria.Location = new System.Drawing.Point(111, 44);
            this.cmbCriteria.Name = "cmbCriteria";
            this.cmbCriteria.Size = new System.Drawing.Size(118, 21);
            this.cmbCriteria.TabIndex = 83;
            this.cmbCriteria.SelectedIndexChanged += new System.EventHandler(this.cmbCriteria_SelectedIndexChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(6, 48);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(39, 13);
            this.Label1.TabIndex = 87;
            this.Label1.Text = "Criteria";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(111, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(118, 20);
            this.txtSearch.TabIndex = 82;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 86;
            this.label2.Text = "Search";
            // 
            // grpReconcile
            // 
            this.grpReconcile.Controls.Add(this.cmbType);
            this.grpReconcile.Controls.Add(this.label6);
            this.grpReconcile.Controls.Add(this.grpBankStatment);
            this.grpReconcile.Controls.Add(this.grpBankSystem);
            this.grpReconcile.Controls.Add(this.cmbBankGroup);
            this.grpReconcile.Controls.Add(this.label5);
            this.grpReconcile.Location = new System.Drawing.Point(10, 146);
            this.grpReconcile.Name = "grpReconcile";
            this.grpReconcile.Size = new System.Drawing.Size(1032, 355);
            this.grpReconcile.TabIndex = 1;
            this.grpReconcile.TabStop = false;
            this.grpReconcile.Text = "Reconcile";
            // 
            // grpBankStatment
            // 
            this.grpBankStatment.Controls.Add(this.dgvBankStatement);
            this.grpBankStatment.Location = new System.Drawing.Point(520, 52);
            this.grpBankStatment.Name = "grpBankStatment";
            this.grpBankStatment.Size = new System.Drawing.Size(506, 297);
            this.grpBankStatment.TabIndex = 94;
            this.grpBankStatment.TabStop = false;
            this.grpBankStatment.Text = "Bank Statement";
            // 
            // dgvBankStatement
            // 
            this.dgvBankStatement.AllowUserToAddRows = false;
            this.dgvBankStatement.AllowUserToDeleteRows = false;
            this.dgvBankStatement.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBankStatement.Location = new System.Drawing.Point(8, 19);
            this.dgvBankStatement.Name = "dgvBankStatement";
            this.dgvBankStatement.ReadOnly = true;
            this.dgvBankStatement.Size = new System.Drawing.Size(491, 272);
            this.dgvBankStatement.TabIndex = 6;
            this.dgvBankStatement.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBankStatement_CellContentClick);
            // 
            // grpBankSystem
            // 
            this.grpBankSystem.Controls.Add(this.dgvBankSystem);
            this.grpBankSystem.Location = new System.Drawing.Point(10, 52);
            this.grpBankSystem.Name = "grpBankSystem";
            this.grpBankSystem.Size = new System.Drawing.Size(504, 297);
            this.grpBankSystem.TabIndex = 93;
            this.grpBankSystem.TabStop = false;
            this.grpBankSystem.Text = "Bank System";
            // 
            // dgvBankSystem
            // 
            this.dgvBankSystem.AllowUserToAddRows = false;
            this.dgvBankSystem.AllowUserToDeleteRows = false;
            this.dgvBankSystem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBankSystem.Location = new System.Drawing.Point(6, 19);
            this.dgvBankSystem.Name = "dgvBankSystem";
            this.dgvBankSystem.ReadOnly = true;
            this.dgvBankSystem.Size = new System.Drawing.Size(491, 272);
            this.dgvBankSystem.TabIndex = 7;
            this.dgvBankSystem.SelectionChanged += new System.EventHandler(this.cmbCriteria_SelectedIndexChanged);
            this.dgvBankSystem.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBankSystem_CellContentClick);
            // 
            // cmbBankGroup
            // 
            this.cmbBankGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBankGroup.FormattingEnabled = true;
            this.cmbBankGroup.Location = new System.Drawing.Point(111, 19);
            this.cmbBankGroup.Name = "cmbBankGroup";
            this.cmbBankGroup.Size = new System.Drawing.Size(118, 21);
            this.cmbBankGroup.TabIndex = 92;
            this.cmbBankGroup.SelectedIndexChanged += new System.EventHandler(this.cmbBankGroup_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 92;
            this.label5.Text = "Bank Code";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(886, 507);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(967, 507);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(325, 19);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(101, 21);
            this.cmbType.TabIndex = 96;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(269, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 95;
            this.label6.Text = "Type";
            // 
            // BankReconcile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(1049, 535);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpReconcile);
            this.Controls.Add(this.grpSearch);
            this.Name = "BankReconcile";
            this.Text = "Bank Reconcile";
            this.Load += new System.EventHandler(this.BankReconcile_Load);
            this.Shown += new System.EventHandler(this.BankReconcile_Shown);
            this.grpSearch.ResumeLayout(false);
            this.grpSearch.PerformLayout();
            this.grpReconcile.ResumeLayout(false);
            this.grpReconcile.PerformLayout();
            this.grpBankStatment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankStatement)).EndInit();
            this.grpBankSystem.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBankSystem)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSearch;
        internal System.Windows.Forms.DateTimePicker dtFrom;
        internal System.Windows.Forms.Button btnReset;
        internal System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button btnSearch;
        internal System.Windows.Forms.ComboBox cmbCriteria;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox txtSearch;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox grpReconcile;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExit;
        internal System.Windows.Forms.ComboBox cmbBankGroup;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grpBankStatment;
        private System.Windows.Forms.GroupBox grpBankSystem;
        internal System.Windows.Forms.DataGridView dgvBankStatement;
        internal System.Windows.Forms.DataGridView dgvBankSystem;
        internal System.Windows.Forms.ComboBox cmbType;
        internal System.Windows.Forms.Label label6;

    }
}
