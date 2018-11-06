namespace ISBS_New.Master.ExchangeRate
{
    partial class ExchangeRate
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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtExchRate = new System.Windows.Forms.TextBox();
            this.txtCurrencyId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRecId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSCust = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(225, 162);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 82;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(144, 162);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 81;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(63, 162);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 80;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtExchRate
            // 
            this.txtExchRate.Enabled = false;
            this.txtExchRate.Location = new System.Drawing.Point(180, 115);
            this.txtExchRate.MaxLength = 30;
            this.txtExchRate.Name = "txtExchRate";
            this.txtExchRate.Size = new System.Drawing.Size(157, 20);
            this.txtExchRate.TabIndex = 79;
            this.txtExchRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtExchRate_KeyPress);
            // 
            // txtCurrencyId
            // 
            this.txtCurrencyId.Enabled = false;
            this.txtCurrencyId.Location = new System.Drawing.Point(180, 89);
            this.txtCurrencyId.MaxLength = 3;
            this.txtCurrencyId.Name = "txtCurrencyId";
            this.txtCurrencyId.Size = new System.Drawing.Size(128, 20);
            this.txtCurrencyId.TabIndex = 78;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 84;
            this.label3.Text = "Exchange Rate";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 83;
            this.label1.Text = "Currency ID";
            // 
            // txtRecId
            // 
            this.txtRecId.Enabled = false;
            this.txtRecId.Location = new System.Drawing.Point(180, 63);
            this.txtRecId.MaxLength = 3;
            this.txtRecId.Name = "txtRecId";
            this.txtRecId.Size = new System.Drawing.Size(157, 20);
            this.txtRecId.TabIndex = 85;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 86;
            this.label2.Text = "Rec ID";
            // 
            // btnSCust
            // 
            this.btnSCust.Location = new System.Drawing.Point(314, 87);
            this.btnSCust.Name = "btnSCust";
            this.btnSCust.Size = new System.Drawing.Size(23, 23);
            this.btnSCust.TabIndex = 113;
            this.btnSCust.Text = "...";
            this.btnSCust.UseVisualStyleBackColor = true;
            this.btnSCust.Click += new System.EventHandler(this.btnSCust_Click);
            // 
            // ExchangeRate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 208);
            this.Controls.Add(this.btnSCust);
            this.Controls.Add(this.txtRecId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtExchRate);
            this.Controls.Add(this.txtCurrencyId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "ExchangeRate";
            this.Text = "ExchangeRate";
            this.Load += new System.EventHandler(this.ExchangeRate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtExchRate;
        private System.Windows.Forms.TextBox txtCurrencyId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRecId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSCust;
    }
}