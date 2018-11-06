namespace ISBS_New.Master.Sales
{
    partial class FrmM_Sales
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
            this.mPanel_Bottom = new MetroFramework.Controls.MetroPanel();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnEdit = new System.Windows.Forms.Button();
            this.mTabCtrl_Sales = new MetroFramework.Controls.MetroTabControl();
            this.mTabPage_General = new MetroFramework.Controls.MetroTabPage();
            this.txttblSales_Persen = new System.Windows.Forms.TextBox();
            this.txttblSales_Group = new System.Windows.Forms.TextBox();
            this.cbxtblSales_Toko_Proyek = new System.Windows.Forms.ComboBox();
            this.cbxtblSales_Counter_Sls = new System.Windows.Forms.ComboBox();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.txttblSales_Nama_Sales = new System.Windows.Forms.TextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.txttblSales_Kode_Sls = new System.Windows.Forms.TextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.mTabPage_Contact = new MetroFramework.Controls.MetroTabPage();
            this.mPanel_Contact = new MetroFramework.Controls.MetroPanel();
            this.DtGridView_Contact = new System.Windows.Forms.DataGridView();
            this.mPanel_Contact_Bottom = new MetroFramework.Controls.MetroPanel();
            this.btnDelete_Ct = new System.Windows.Forms.Button();
            this.btnEdit_Ct = new System.Windows.Forms.Button();
            this.btnNew_Ct = new System.Windows.Forms.Button();
            this.mPanel_Bottom.SuspendLayout();
            this.mTabCtrl_Sales.SuspendLayout();
            this.mTabPage_General.SuspendLayout();
            this.mTabPage_Contact.SuspendLayout();
            this.mPanel_Contact.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DtGridView_Contact)).BeginInit();
            this.mPanel_Contact_Bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // mPanel_Bottom
            // 
            this.mPanel_Bottom.Controls.Add(this.BtnDelete);
            this.mPanel_Bottom.Controls.Add(this.BtnCancel);
            this.mPanel_Bottom.Controls.Add(this.BtnSave);
            this.mPanel_Bottom.Controls.Add(this.BtnEdit);
            this.mPanel_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mPanel_Bottom.HorizontalScrollbarBarColor = true;
            this.mPanel_Bottom.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_Bottom.HorizontalScrollbarSize = 10;
            this.mPanel_Bottom.Location = new System.Drawing.Point(27, 422);
            this.mPanel_Bottom.Name = "mPanel_Bottom";
            this.mPanel_Bottom.Size = new System.Drawing.Size(636, 32);
            this.mPanel_Bottom.TabIndex = 63;
            this.mPanel_Bottom.VerticalScrollbarBarColor = true;
            this.mPanel_Bottom.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Bottom.VerticalScrollbarSize = 10;
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(556, 5);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(75, 23);
            this.BtnDelete.TabIndex = 18;
            this.BtnDelete.Text = "&Delete";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(394, 5);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 16;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(475, 5);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 17;
            this.BtnSave.Text = "Sa&ve";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnEdit
            // 
            this.BtnEdit.Location = new System.Drawing.Point(316, 5);
            this.BtnEdit.Name = "BtnEdit";
            this.BtnEdit.Size = new System.Drawing.Size(75, 23);
            this.BtnEdit.TabIndex = 15;
            this.BtnEdit.Text = "&Edit";
            this.BtnEdit.UseVisualStyleBackColor = true;
            this.BtnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // mTabCtrl_Sales
            // 
            this.mTabCtrl_Sales.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.mTabCtrl_Sales.Controls.Add(this.mTabPage_General);
            this.mTabCtrl_Sales.Controls.Add(this.mTabPage_Contact);
            this.mTabCtrl_Sales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTabCtrl_Sales.Location = new System.Drawing.Point(27, 74);
            this.mTabCtrl_Sales.Name = "mTabCtrl_Sales";
            this.mTabCtrl_Sales.SelectedIndex = 0;
            this.mTabCtrl_Sales.Size = new System.Drawing.Size(636, 348);
            this.mTabCtrl_Sales.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mTabCtrl_Sales.TabIndex = 72;
            this.mTabCtrl_Sales.UseSelectable = true;
            // 
            // mTabPage_General
            // 
            this.mTabPage_General.Controls.Add(this.txttblSales_Persen);
            this.mTabPage_General.Controls.Add(this.txttblSales_Group);
            this.mTabPage_General.Controls.Add(this.cbxtblSales_Toko_Proyek);
            this.mTabPage_General.Controls.Add(this.cbxtblSales_Counter_Sls);
            this.mTabPage_General.Controls.Add(this.metroLabel6);
            this.mTabPage_General.Controls.Add(this.metroLabel5);
            this.mTabPage_General.Controls.Add(this.metroLabel3);
            this.mTabPage_General.Controls.Add(this.metroLabel2);
            this.mTabPage_General.Controls.Add(this.txttblSales_Nama_Sales);
            this.mTabPage_General.Controls.Add(this.metroLabel1);
            this.mTabPage_General.Controls.Add(this.txttblSales_Kode_Sls);
            this.mTabPage_General.Controls.Add(this.metroLabel4);
            this.mTabPage_General.HorizontalScrollbarBarColor = true;
            this.mTabPage_General.HorizontalScrollbarHighlightOnWheel = false;
            this.mTabPage_General.HorizontalScrollbarSize = 5;
            this.mTabPage_General.Location = new System.Drawing.Point(4, 41);
            this.mTabPage_General.Name = "mTabPage_General";
            this.mTabPage_General.Size = new System.Drawing.Size(628, 303);
            this.mTabPage_General.TabIndex = 0;
            this.mTabPage_General.Text = "General";
            this.mTabPage_General.VerticalScrollbarBarColor = true;
            this.mTabPage_General.VerticalScrollbarHighlightOnWheel = false;
            this.mTabPage_General.VerticalScrollbarSize = 4;
            // 
            // txttblSales_Persen
            // 
            this.txttblSales_Persen.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblSales_Persen.Location = new System.Drawing.Point(115, 165);
            this.txttblSales_Persen.MaxLength = 10;
            this.txttblSales_Persen.Name = "txttblSales_Persen";
            this.txttblSales_Persen.Size = new System.Drawing.Size(135, 23);
            this.txttblSales_Persen.TabIndex = 6;
            this.txttblSales_Persen.Text = "0.00";
            this.txttblSales_Persen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txttblSales_Group
            // 
            this.txttblSales_Group.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblSales_Group.Location = new System.Drawing.Point(115, 136);
            this.txttblSales_Group.MaxLength = 10;
            this.txttblSales_Group.Name = "txttblSales_Group";
            this.txttblSales_Group.Size = new System.Drawing.Size(135, 23);
            this.txttblSales_Group.TabIndex = 5;
            // 
            // cbxtblSales_Toko_Proyek
            // 
            this.cbxtblSales_Toko_Proyek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxtblSales_Toko_Proyek.FormattingEnabled = true;
            this.cbxtblSales_Toko_Proyek.Items.AddRange(new object[] {
            "",
            "TOKO",
            "PROYEK"});
            this.cbxtblSales_Toko_Proyek.Location = new System.Drawing.Point(115, 106);
            this.cbxtblSales_Toko_Proyek.Name = "cbxtblSales_Toko_Proyek";
            this.cbxtblSales_Toko_Proyek.Size = new System.Drawing.Size(135, 24);
            this.cbxtblSales_Toko_Proyek.TabIndex = 4;
            // 
            // cbxtblSales_Counter_Sls
            // 
            this.cbxtblSales_Counter_Sls.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxtblSales_Counter_Sls.FormattingEnabled = true;
            this.cbxtblSales_Counter_Sls.Items.AddRange(new object[] {
            "",
            "COUNTER",
            "SALES"});
            this.cbxtblSales_Counter_Sls.Location = new System.Drawing.Point(115, 76);
            this.cbxtblSales_Counter_Sls.Name = "cbxtblSales_Counter_Sls";
            this.cbxtblSales_Counter_Sls.Size = new System.Drawing.Size(135, 24);
            this.cbxtblSales_Counter_Sls.TabIndex = 3;
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(1, 167);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(48, 19);
            this.metroLabel6.TabIndex = 91;
            this.metroLabel6.Text = "Persen";
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(1, 138);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(46, 19);
            this.metroLabel5.TabIndex = 90;
            this.metroLabel5.Text = "Group";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(1, 109);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(91, 19);
            this.metroLabel3.TabIndex = 89;
            this.metroLabel3.Text = "Toko / Proyek";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(1, 79);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(98, 19);
            this.metroLabel2.TabIndex = 88;
            this.metroLabel2.Text = "Counter / Sales";
            // 
            // txttblSales_Nama_Sales
            // 
            this.txttblSales_Nama_Sales.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblSales_Nama_Sales.Location = new System.Drawing.Point(115, 46);
            this.txttblSales_Nama_Sales.MaxLength = 50;
            this.txttblSales_Nama_Sales.Name = "txttblSales_Nama_Sales";
            this.txttblSales_Nama_Sales.Size = new System.Drawing.Size(503, 23);
            this.txttblSales_Nama_Sales.TabIndex = 2;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(1, 48);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(78, 19);
            this.metroLabel1.TabIndex = 87;
            this.metroLabel1.Text = "Nama Sales";
            // 
            // txttblSales_Kode_Sls
            // 
            this.txttblSales_Kode_Sls.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblSales_Kode_Sls.Location = new System.Drawing.Point(115, 17);
            this.txttblSales_Kode_Sls.MaxLength = 8;
            this.txttblSales_Kode_Sls.Name = "txttblSales_Kode_Sls";
            this.txttblSales_Kode_Sls.Size = new System.Drawing.Size(75, 23);
            this.txttblSales_Kode_Sls.TabIndex = 1;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(1, 19);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(72, 19);
            this.metroLabel4.TabIndex = 86;
            this.metroLabel4.Text = "Kode Sales";
            // 
            // mTabPage_Contact
            // 
            this.mTabPage_Contact.Controls.Add(this.mPanel_Contact);
            this.mTabPage_Contact.HorizontalScrollbarBarColor = true;
            this.mTabPage_Contact.HorizontalScrollbarHighlightOnWheel = false;
            this.mTabPage_Contact.HorizontalScrollbarSize = 5;
            this.mTabPage_Contact.Location = new System.Drawing.Point(4, 41);
            this.mTabPage_Contact.Name = "mTabPage_Contact";
            this.mTabPage_Contact.Size = new System.Drawing.Size(628, 303);
            this.mTabPage_Contact.TabIndex = 1;
            this.mTabPage_Contact.Text = "| Contact";
            this.mTabPage_Contact.VerticalScrollbarBarColor = true;
            this.mTabPage_Contact.VerticalScrollbarHighlightOnWheel = false;
            this.mTabPage_Contact.VerticalScrollbarSize = 4;
            // 
            // mPanel_Contact
            // 
            this.mPanel_Contact.Controls.Add(this.DtGridView_Contact);
            this.mPanel_Contact.Controls.Add(this.mPanel_Contact_Bottom);
            this.mPanel_Contact.Dock = System.Windows.Forms.DockStyle.Top;
            this.mPanel_Contact.HorizontalScrollbarBarColor = true;
            this.mPanel_Contact.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_Contact.HorizontalScrollbarSize = 5;
            this.mPanel_Contact.Location = new System.Drawing.Point(0, 0);
            this.mPanel_Contact.Name = "mPanel_Contact";
            this.mPanel_Contact.Size = new System.Drawing.Size(628, 236);
            this.mPanel_Contact.TabIndex = 2;
            this.mPanel_Contact.VerticalScrollbarBarColor = true;
            this.mPanel_Contact.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Contact.VerticalScrollbarSize = 4;
            // 
            // DtGridView_Contact
            // 
            this.DtGridView_Contact.AllowUserToAddRows = false;
            this.DtGridView_Contact.AllowUserToDeleteRows = false;
            this.DtGridView_Contact.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DtGridView_Contact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DtGridView_Contact.Location = new System.Drawing.Point(0, 0);
            this.DtGridView_Contact.Name = "DtGridView_Contact";
            this.DtGridView_Contact.ReadOnly = true;
            this.DtGridView_Contact.Size = new System.Drawing.Size(628, 209);
            this.DtGridView_Contact.TabIndex = 3;
            // 
            // mPanel_Contact_Bottom
            // 
            this.mPanel_Contact_Bottom.Controls.Add(this.btnDelete_Ct);
            this.mPanel_Contact_Bottom.Controls.Add(this.btnEdit_Ct);
            this.mPanel_Contact_Bottom.Controls.Add(this.btnNew_Ct);
            this.mPanel_Contact_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mPanel_Contact_Bottom.HorizontalScrollbarBarColor = true;
            this.mPanel_Contact_Bottom.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel_Contact_Bottom.HorizontalScrollbarSize = 5;
            this.mPanel_Contact_Bottom.Location = new System.Drawing.Point(0, 209);
            this.mPanel_Contact_Bottom.Name = "mPanel_Contact_Bottom";
            this.mPanel_Contact_Bottom.Size = new System.Drawing.Size(628, 27);
            this.mPanel_Contact_Bottom.TabIndex = 2;
            this.mPanel_Contact_Bottom.VerticalScrollbarBarColor = true;
            this.mPanel_Contact_Bottom.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Contact_Bottom.VerticalScrollbarSize = 4;
            // 
            // btnDelete_Ct
            // 
            this.btnDelete_Ct.Location = new System.Drawing.Point(165, 2);
            this.btnDelete_Ct.Name = "btnDelete_Ct";
            this.btnDelete_Ct.Size = new System.Drawing.Size(75, 23);
            this.btnDelete_Ct.TabIndex = 14;
            this.btnDelete_Ct.Text = "&Delete";
            this.btnDelete_Ct.UseVisualStyleBackColor = true;
            this.btnDelete_Ct.Click += new System.EventHandler(this.btnDelete_Ct_Click);
            // 
            // btnEdit_Ct
            // 
            this.btnEdit_Ct.Location = new System.Drawing.Point(84, 2);
            this.btnEdit_Ct.Name = "btnEdit_Ct";
            this.btnEdit_Ct.Size = new System.Drawing.Size(75, 23);
            this.btnEdit_Ct.TabIndex = 13;
            this.btnEdit_Ct.Text = "&Edit";
            this.btnEdit_Ct.UseVisualStyleBackColor = true;
            this.btnEdit_Ct.Click += new System.EventHandler(this.btnEdit_Ct_Click);
            // 
            // btnNew_Ct
            // 
            this.btnNew_Ct.Location = new System.Drawing.Point(3, 2);
            this.btnNew_Ct.Name = "btnNew_Ct";
            this.btnNew_Ct.Size = new System.Drawing.Size(75, 23);
            this.btnNew_Ct.TabIndex = 12;
            this.btnNew_Ct.Text = "&New";
            this.btnNew_Ct.UseVisualStyleBackColor = true;
            this.btnNew_Ct.Click += new System.EventHandler(this.btnNew_Ct_Click);
            // 
            // FrmM_Sales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 479);
            this.Controls.Add(this.mTabCtrl_Sales);
            this.Controls.Add(this.mPanel_Bottom);
            this.Font = new System.Drawing.Font("Courier New", 10F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmM_Sales";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Resizable = false;
            this.Text = "Master Sales";
            this.Load += new System.EventHandler(this.FrmM_Sales_Load);
            this.Shown += new System.EventHandler(this.FrmM_Sales_Shown);
            this.mPanel_Bottom.ResumeLayout(false);
            this.mTabCtrl_Sales.ResumeLayout(false);
            this.mTabPage_General.ResumeLayout(false);
            this.mTabPage_General.PerformLayout();
            this.mTabPage_Contact.ResumeLayout(false);
            this.mPanel_Contact.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DtGridView_Contact)).EndInit();
            this.mPanel_Contact_Bottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel mPanel_Bottom;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnEdit;
        private MetroFramework.Controls.MetroTabControl mTabCtrl_Sales;
        private MetroFramework.Controls.MetroTabPage mTabPage_General;
        private MetroFramework.Controls.MetroTabPage mTabPage_Contact;
        private System.Windows.Forms.TextBox txttblSales_Persen;
        private System.Windows.Forms.TextBox txttblSales_Group;
        private System.Windows.Forms.ComboBox cbxtblSales_Toko_Proyek;
        private System.Windows.Forms.ComboBox cbxtblSales_Counter_Sls;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private System.Windows.Forms.TextBox txttblSales_Nama_Sales;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private System.Windows.Forms.TextBox txttblSales_Kode_Sls;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroPanel mPanel_Contact;
        private MetroFramework.Controls.MetroPanel mPanel_Contact_Bottom;
        private System.Windows.Forms.DataGridView DtGridView_Contact;
        private System.Windows.Forms.Button btnDelete_Ct;
        private System.Windows.Forms.Button btnEdit_Ct;
        private System.Windows.Forms.Button btnNew_Ct;
    }
}