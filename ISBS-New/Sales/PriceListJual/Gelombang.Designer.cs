﻿namespace ISBS_New.Sales.PriceListJual
{
    partial class Gelombang
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
            this.dgvGelombang = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblForm = new System.Windows.Forms.Label();
            this.chkAll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGelombang)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvGelombang
            // 
            this.dgvGelombang.AllowUserToAddRows = false;
            this.dgvGelombang.AllowUserToDeleteRows = false;
            this.dgvGelombang.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGelombang.Location = new System.Drawing.Point(8, 35);
            this.dgvGelombang.Name = "dgvGelombang";
            this.dgvGelombang.ReadOnly = true;
            this.dgvGelombang.Size = new System.Drawing.Size(600, 217);
            this.dgvGelombang.TabIndex = 0;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(533, 258);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkAll);
            this.groupBox1.Controls.Add(this.dgvGelombang);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Location = new System.Drawing.Point(8, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(622, 292);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // lblForm
            // 
            this.lblForm.AutoSize = true;
            this.lblForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForm.Location = new System.Drawing.Point(13, 12);
            this.lblForm.Name = "lblForm";
            this.lblForm.Size = new System.Drawing.Size(40, 13);
            this.lblForm.TabIndex = 3;
            this.lblForm.Text = "Wave";
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(8, 12);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(71, 17);
            this.chkAll.TabIndex = 3;
            this.chkAll.Text = "Check All";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged_1);
            // 
            // Gelombang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 331);
            this.Controls.Add(this.lblForm);
            this.Controls.Add(this.groupBox1);
            this.Name = "Gelombang";
            this.Resizable = false;
            this.Text = "Gelombang";
            this.Load += new System.EventHandler(this.Gelombang2_Load);
            this.Shown += new System.EventHandler(this.Gelombang2_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGelombang)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvGelombang;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblForm;
        private System.Windows.Forms.CheckBox chkAll;
    }
}