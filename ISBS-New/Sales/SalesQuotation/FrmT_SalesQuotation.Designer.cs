namespace ISBS_New.Sales.SalesQuotation
{
    partial class FrmT_SalesQuotation
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
            this.TabCtrl_tblSalesQuotation = new MetroFramework.Controls.MetroTabControl();
            this.TabPage_Header = new MetroFramework.Controls.MetroTabPage();
            this.TabPage_Detail = new MetroFramework.Controls.MetroTabPage();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroDateTime1 = new MetroFramework.Controls.MetroDateTime();
            this.metroTextBox1 = new MetroFramework.Controls.MetroTextBox();
            this.TabCtrl_tblSalesQuotation.SuspendLayout();
            this.TabPage_Header.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabCtrl_tblSalesQuotation
            // 
            this.TabCtrl_tblSalesQuotation.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TabCtrl_tblSalesQuotation.Controls.Add(this.TabPage_Header);
            this.TabCtrl_tblSalesQuotation.Controls.Add(this.TabPage_Detail);
            this.TabCtrl_tblSalesQuotation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabCtrl_tblSalesQuotation.Location = new System.Drawing.Point(27, 74);
            this.TabCtrl_tblSalesQuotation.Name = "TabCtrl_tblSalesQuotation";
            this.TabCtrl_tblSalesQuotation.SelectedIndex = 0;
            this.TabCtrl_tblSalesQuotation.Size = new System.Drawing.Size(814, 400);
            this.TabCtrl_tblSalesQuotation.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabCtrl_tblSalesQuotation.TabIndex = 2;
            this.TabCtrl_tblSalesQuotation.UseSelectable = true;
            // 
            // TabPage_Header
            // 
            this.TabPage_Header.Controls.Add(this.metroTextBox1);
            this.TabPage_Header.Controls.Add(this.metroDateTime1);
            this.TabPage_Header.Controls.Add(this.metroLabel2);
            this.TabPage_Header.Controls.Add(this.metroLabel1);
            this.TabPage_Header.HorizontalScrollbarBarColor = true;
            this.TabPage_Header.HorizontalScrollbarHighlightOnWheel = false;
            this.TabPage_Header.HorizontalScrollbarSize = 2;
            this.TabPage_Header.Location = new System.Drawing.Point(4, 41);
            this.TabPage_Header.Name = "TabPage_Header";
            this.TabPage_Header.Size = new System.Drawing.Size(806, 355);
            this.TabPage_Header.TabIndex = 2;
            this.TabPage_Header.Text = "HEADER";
            this.TabPage_Header.VerticalScrollbarBarColor = true;
            this.TabPage_Header.VerticalScrollbarHighlightOnWheel = false;
            this.TabPage_Header.VerticalScrollbarSize = 2;
            // 
            // TabPage_Detail
            // 
            this.TabPage_Detail.HorizontalScrollbarBarColor = true;
            this.TabPage_Detail.HorizontalScrollbarHighlightOnWheel = false;
            this.TabPage_Detail.HorizontalScrollbarSize = 2;
            this.TabPage_Detail.Location = new System.Drawing.Point(4, 41);
            this.TabPage_Detail.Name = "TabPage_Detail";
            this.TabPage_Detail.Size = new System.Drawing.Size(806, 355);
            this.TabPage_Detail.TabIndex = 3;
            this.TabPage_Detail.Text = "| DETAIL";
            this.TabPage_Detail.VerticalScrollbarBarColor = true;
            this.TabPage_Detail.VerticalScrollbarHighlightOnWheel = false;
            this.TabPage_Detail.VerticalScrollbarSize = 2;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(0, 62);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(49, 19);
            this.metroLabel1.TabIndex = 14;
            this.metroLabel1.Text = "No SQ";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(0, 30);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(49, 19);
            this.metroLabel2.TabIndex = 16;
            this.metroLabel2.Text = "Tgl SQ";
            // 
            // metroDateTime1
            // 
            this.metroDateTime1.Location = new System.Drawing.Point(126, 25);
            this.metroDateTime1.MinimumSize = new System.Drawing.Size(0, 29);
            this.metroDateTime1.Name = "metroDateTime1";
            this.metroDateTime1.Size = new System.Drawing.Size(200, 29);
            this.metroDateTime1.TabIndex = 17;
            // 
            // metroTextBox1
            // 
            this.metroTextBox1.Lines = new string[0];
            this.metroTextBox1.Location = new System.Drawing.Point(126, 60);
            this.metroTextBox1.MaxLength = 32767;
            this.metroTextBox1.Name = "metroTextBox1";
            this.metroTextBox1.PasswordChar = '\0';
            this.metroTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.metroTextBox1.SelectedText = "";
            this.metroTextBox1.Size = new System.Drawing.Size(200, 23);
            this.metroTextBox1.TabIndex = 18;
            this.metroTextBox1.UseSelectable = true;
            // 
            // FrmT_SalesQuotation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 499);
            this.Controls.Add(this.TabCtrl_tblSalesQuotation);
            this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmT_SalesQuotation";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Text = "Sales Quotation";
            this.Load += new System.EventHandler(this.FrmT_SalesQuotation_Load);
            this.TabCtrl_tblSalesQuotation.ResumeLayout(false);
            this.TabPage_Header.ResumeLayout(false);
            this.TabPage_Header.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTabControl TabCtrl_tblSalesQuotation;
        private MetroFramework.Controls.MetroTabPage TabPage_Header;
        private MetroFramework.Controls.MetroTabPage TabPage_Detail;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroDateTime metroDateTime1;
        private MetroFramework.Controls.MetroTextBox metroTextBox1;
    }
}