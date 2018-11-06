namespace ISBS_New.Purchase.PurchaseOrderNew
{
    partial class PQ
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
            this.dgvPQ = new System.Windows.Forms.DataGridView();
            this.btnSelect = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPQ)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvPQ);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Location = new System.Drawing.Point(12, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(622, 284);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // dgvPQ
            // 
            this.dgvPQ.AllowUserToAddRows = false;
            this.dgvPQ.AllowUserToDeleteRows = false;
            this.dgvPQ.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPQ.Location = new System.Drawing.Point(8, 19);
            this.dgvPQ.Name = "dgvPQ";
            this.dgvPQ.ReadOnly = true;
            this.dgvPQ.Size = new System.Drawing.Size(600, 217);
            this.dgvPQ.TabIndex = 0;
            this.dgvPQ.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPQ_CellValueChanged);
            this.dgvPQ.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPQ_CellFormatting);
            this.dgvPQ.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPQ_CellClick);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(533, 246);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // PQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 352);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(651, 352);
            this.Name = "PQ";
            this.Text = "PQ";
            this.Load += new System.EventHandler(this.PQ_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PQ_FormClosing);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPQ)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvPQ;
        private System.Windows.Forms.Button btnSelect;
    }
}