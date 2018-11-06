namespace ISBS_New.Purchase.PurchaseRequisition
{
    partial class DetailPR
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
            this.label4 = new System.Windows.Forms.Label();
            this.txtItemGroupName = new System.Windows.Forms.TextBox();
            this.txtSubGroup1Name = new System.Windows.Forms.TextBox();
            this.txtSubGroup2Name = new System.Windows.Forms.TextBox();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.txtItemGroupId = new System.Windows.Forms.TextBox();
            this.txtItemSubGroup1Id = new System.Windows.Forms.TextBox();
            this.txtItemSubGroup2Id = new System.Windows.Forms.TextBox();
            this.txtItemId = new System.Windows.Forms.TextBox();
            this.btnGroup = new System.Windows.Forms.Button();
            this.btnSubGroup1 = new System.Windows.Forms.Button();
            this.btnSubGroup2 = new System.Windows.Forms.Button();
            this.btnItem = new System.Windows.Forms.Button();
            this.dgvDetailPR = new System.Windows.Forms.DataGridView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailPR)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item Group";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Item SubGroup 1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Item Name";
            // 
            // txtItemGroupName
            // 
            this.txtItemGroupName.Location = new System.Drawing.Point(129, 13);
            this.txtItemGroupName.Name = "txtItemGroupName";
            this.txtItemGroupName.Size = new System.Drawing.Size(221, 20);
            this.txtItemGroupName.TabIndex = 4;
            // 
            // txtSubGroup1Name
            // 
            this.txtSubGroup1Name.Location = new System.Drawing.Point(129, 38);
            this.txtSubGroup1Name.Name = "txtSubGroup1Name";
            this.txtSubGroup1Name.Size = new System.Drawing.Size(221, 20);
            this.txtSubGroup1Name.TabIndex = 5;
            // 
            // txtSubGroup2Name
            // 
            this.txtSubGroup2Name.Location = new System.Drawing.Point(129, 63);
            this.txtSubGroup2Name.Name = "txtSubGroup2Name";
            this.txtSubGroup2Name.Size = new System.Drawing.Size(221, 20);
            this.txtSubGroup2Name.TabIndex = 6;
            // 
            // txtItemName
            // 
            this.txtItemName.Location = new System.Drawing.Point(129, 88);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(221, 20);
            this.txtItemName.TabIndex = 7;
            this.txtItemName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtItemName_KeyPress);
            // 
            // txtItemGroupId
            // 
            this.txtItemGroupId.Location = new System.Drawing.Point(339, 15);
            this.txtItemGroupId.Name = "txtItemGroupId";
            this.txtItemGroupId.ReadOnly = true;
            this.txtItemGroupId.Size = new System.Drawing.Size(47, 20);
            this.txtItemGroupId.TabIndex = 8;
            this.txtItemGroupId.Visible = false;
            // 
            // txtItemSubGroup1Id
            // 
            this.txtItemSubGroup1Id.Location = new System.Drawing.Point(339, 40);
            this.txtItemSubGroup1Id.Name = "txtItemSubGroup1Id";
            this.txtItemSubGroup1Id.ReadOnly = true;
            this.txtItemSubGroup1Id.Size = new System.Drawing.Size(47, 20);
            this.txtItemSubGroup1Id.TabIndex = 9;
            this.txtItemSubGroup1Id.Visible = false;
            // 
            // txtItemSubGroup2Id
            // 
            this.txtItemSubGroup2Id.Location = new System.Drawing.Point(339, 65);
            this.txtItemSubGroup2Id.Name = "txtItemSubGroup2Id";
            this.txtItemSubGroup2Id.ReadOnly = true;
            this.txtItemSubGroup2Id.Size = new System.Drawing.Size(47, 20);
            this.txtItemSubGroup2Id.TabIndex = 10;
            this.txtItemSubGroup2Id.Visible = false;
            // 
            // txtItemId
            // 
            this.txtItemId.Location = new System.Drawing.Point(339, 90);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.ReadOnly = true;
            this.txtItemId.Size = new System.Drawing.Size(47, 20);
            this.txtItemId.TabIndex = 11;
            this.txtItemId.Visible = false;
            // 
            // btnGroup
            // 
            this.btnGroup.Location = new System.Drawing.Point(352, 12);
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Size = new System.Drawing.Size(32, 23);
            this.btnGroup.TabIndex = 12;
            this.btnGroup.Text = "...";
            this.btnGroup.UseVisualStyleBackColor = true;
            this.btnGroup.Click += new System.EventHandler(this.btnGroup_Click);
            // 
            // btnSubGroup1
            // 
            this.btnSubGroup1.Location = new System.Drawing.Point(352, 37);
            this.btnSubGroup1.Name = "btnSubGroup1";
            this.btnSubGroup1.Size = new System.Drawing.Size(32, 23);
            this.btnSubGroup1.TabIndex = 13;
            this.btnSubGroup1.Text = "...";
            this.btnSubGroup1.UseVisualStyleBackColor = true;
            this.btnSubGroup1.Click += new System.EventHandler(this.btnSubGroup1_Click);
            // 
            // btnSubGroup2
            // 
            this.btnSubGroup2.Location = new System.Drawing.Point(352, 62);
            this.btnSubGroup2.Name = "btnSubGroup2";
            this.btnSubGroup2.Size = new System.Drawing.Size(32, 23);
            this.btnSubGroup2.TabIndex = 14;
            this.btnSubGroup2.Text = "...";
            this.btnSubGroup2.UseVisualStyleBackColor = true;
            this.btnSubGroup2.Click += new System.EventHandler(this.btnSubGroup2_Click);
            // 
            // btnItem
            // 
            this.btnItem.Location = new System.Drawing.Point(352, 87);
            this.btnItem.Name = "btnItem";
            this.btnItem.Size = new System.Drawing.Size(32, 23);
            this.btnItem.TabIndex = 15;
            this.btnItem.Text = "...";
            this.btnItem.UseVisualStyleBackColor = true;
            this.btnItem.Visible = false;
            this.btnItem.Click += new System.EventHandler(this.btnItem_Click);
            // 
            // dgvDetailPR
            // 
            this.dgvDetailPR.AllowUserToAddRows = false;
            this.dgvDetailPR.AllowUserToDeleteRows = false;
            this.dgvDetailPR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetailPR.Location = new System.Drawing.Point(9, 143);
            this.dgvDetailPR.Name = "dgvDetailPR";
            this.dgvDetailPR.ReadOnly = true;
            this.dgvDetailPR.Size = new System.Drawing.Size(558, 219);
            this.dgvDetailPR.TabIndex = 16;
            this.dgvDetailPR.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetailPR_CellDoubleClick);
            this.dgvDetailPR.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvDetailPR_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(434, 87);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(86, 23);
            this.btnSearch.TabIndex = 17;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Item SubGroup 2";
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(481, 368);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(86, 23);
            this.btnSelect.TabIndex = 20;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkAll);
            this.groupBox1.Controls.Add(this.btnSubGroup1);
            this.groupBox1.Controls.Add(this.btnGroup);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Controls.Add(this.txtItemGroupName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtSubGroup1Name);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtSubGroup2Name);
            this.groupBox1.Controls.Add(this.dgvDetailPR);
            this.groupBox1.Controls.Add(this.txtItemName);
            this.groupBox1.Controls.Add(this.btnItem);
            this.groupBox1.Controls.Add(this.txtItemGroupId);
            this.groupBox1.Controls.Add(this.btnSubGroup2);
            this.groupBox1.Controls.Add(this.txtItemSubGroup1Id);
            this.groupBox1.Controls.Add(this.txtItemSubGroup2Id);
            this.groupBox1.Controls.Add(this.txtItemId);
            this.groupBox1.Location = new System.Drawing.Point(14, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(577, 397);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(9, 120);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(71, 17);
            this.chkAll.TabIndex = 21;
            this.chkAll.Text = "Check All";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkCheckAll_CheckedChanged);
            // 
            // DetailPR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(604, 474);
            this.Controls.Add(this.groupBox1);
            this.Name = "DetailPR";
            this.Resizable = false;
            this.Text = "Detail Purchase Requition";
            this.Load += new System.EventHandler(this.DetailPR2_Load);
            this.Shown += new System.EventHandler(this.DetailPR2_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailPR)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtItemGroupName;
        private System.Windows.Forms.TextBox txtSubGroup1Name;
        private System.Windows.Forms.TextBox txtSubGroup2Name;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.TextBox txtItemGroupId;
        private System.Windows.Forms.TextBox txtItemSubGroup1Id;
        private System.Windows.Forms.TextBox txtItemSubGroup2Id;
        private System.Windows.Forms.TextBox txtItemId;
        private System.Windows.Forms.Button btnGroup;
        private System.Windows.Forms.Button btnSubGroup1;
        private System.Windows.Forms.Button btnSubGroup2;
        private System.Windows.Forms.Button btnItem;
        private System.Windows.Forms.DataGridView dgvDetailPR;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkAll;
    }
}