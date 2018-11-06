namespace ISBS_New.Inventory.Master.InvantTable
{
    partial class FrmM_ResizeMapping
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFullItemId = new System.Windows.Forms.TextBox();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.dtGridViewResize = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtJenisBrg = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dtGridViewResize)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(499, 370);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 166;
            this.btnSave.Text = "Sa&ve";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(585, 370);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 23);
            this.btnCancel.TabIndex = 167;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 168;
            this.label1.Text = "FullItemId";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 20);
            this.label2.TabIndex = 169;
            this.label2.Text = "Nama Barang";
            // 
            // txtFullItemId
            // 
            this.txtFullItemId.Enabled = false;
            this.txtFullItemId.Location = new System.Drawing.Point(146, 78);
            this.txtFullItemId.Name = "txtFullItemId";
            this.txtFullItemId.Size = new System.Drawing.Size(204, 26);
            this.txtFullItemId.TabIndex = 170;
            // 
            // txtItemName
            // 
            this.txtItemName.Enabled = false;
            this.txtItemName.Location = new System.Drawing.Point(146, 107);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(523, 26);
            this.txtItemName.TabIndex = 171;
            // 
            // dtGridViewResize
            // 
            this.dtGridViewResize.AllowUserToAddRows = false;
            this.dtGridViewResize.AllowUserToDeleteRows = false;
            this.dtGridViewResize.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGridViewResize.Location = new System.Drawing.Point(33, 194);
            this.dtGridViewResize.Name = "dtGridViewResize";
            this.dtGridViewResize.ReadOnly = true;
            this.dtGridViewResize.Size = new System.Drawing.Size(636, 154);
            this.dtGridViewResize.TabIndex = 172;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(33, 165);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 173;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(114, 165);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(89, 23);
            this.btnDelete.TabIndex = 174;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 20);
            this.label3.TabIndex = 175;
            this.label3.Text = "Resize To:";
            // 
            // txtJenisBrg
            // 
            this.txtJenisBrg.Enabled = false;
            this.txtJenisBrg.Location = new System.Drawing.Point(356, 78);
            this.txtJenisBrg.Name = "txtJenisBrg";
            this.txtJenisBrg.Size = new System.Drawing.Size(134, 26);
            this.txtJenisBrg.TabIndex = 176;
            // 
            // FrmM_ResizeMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 421);
            this.Controls.Add(this.txtJenisBrg);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dtGridViewResize);
            this.Controls.Add(this.txtItemName);
            this.Controls.Add(this.txtFullItemId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Courier New", 10F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmM_ResizeMapping";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Resizable = false;
            this.Text = "Resize Mapping";
            this.Load += new System.EventHandler(this.FrmM_ResizeMapping_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtGridViewResize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFullItemId;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.DataGridView dtGridViewResize;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtJenisBrg;
    }
}