namespace ISBS_New.Inventory.Master.InvantTable
{
    partial class PopUpConversion
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFullItemId = new System.Windows.Forms.TextBox();
            this.txtItemDesc = new System.Windows.Forms.TextBox();
            this.dgvConversion = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConversion)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Item Description";
            // 
            // txtFullItemId
            // 
            this.txtFullItemId.Enabled = false;
            this.txtFullItemId.Location = new System.Drawing.Point(107, 60);
            this.txtFullItemId.Name = "txtFullItemId";
            this.txtFullItemId.ReadOnly = true;
            this.txtFullItemId.Size = new System.Drawing.Size(273, 20);
            this.txtFullItemId.TabIndex = 2;
            // 
            // txtItemDesc
            // 
            this.txtItemDesc.Enabled = false;
            this.txtItemDesc.Location = new System.Drawing.Point(107, 84);
            this.txtItemDesc.Name = "txtItemDesc";
            this.txtItemDesc.ReadOnly = true;
            this.txtItemDesc.Size = new System.Drawing.Size(273, 20);
            this.txtItemDesc.TabIndex = 3;
            // 
            // dgvConversion
            // 
            this.dgvConversion.AllowUserToAddRows = false;
            this.dgvConversion.AllowUserToDeleteRows = false;
            this.dgvConversion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConversion.Location = new System.Drawing.Point(18, 110);
            this.dgvConversion.Name = "dgvConversion";
            this.dgvConversion.ReadOnly = true;
            this.dgvConversion.Size = new System.Drawing.Size(362, 130);
            this.dgvConversion.TabIndex = 4;
            this.dgvConversion.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvConversion_CellFormatting);
            this.dgvConversion.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvConversion_EditingControlShowing);
            this.dgvConversion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvConversion_KeyPress);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(224, 246);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(305, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PopUpConversion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 288);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvConversion);
            this.Controls.Add(this.txtItemDesc);
            this.Controls.Add(this.txtFullItemId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PopUpConversion";
            this.Text = "Form Conversion";
            this.Load += new System.EventHandler(this.PopUpConversion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConversion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFullItemId;
        private System.Windows.Forms.TextBox txtItemDesc;
        private System.Windows.Forms.DataGridView dgvConversion;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}