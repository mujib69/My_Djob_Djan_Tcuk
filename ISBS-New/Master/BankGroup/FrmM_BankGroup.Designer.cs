namespace ISBS_New.Master.BankGroup
{
    partial class FrmM_BankGroup
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
            this.txttblBank_Group_Group = new System.Windows.Forms.TextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.txttblBank_Group_Nama = new System.Windows.Forms.TextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.mPanel_Bottom = new MetroFramework.Controls.MetroPanel();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnEdit = new System.Windows.Forms.Button();
            this.mPanel_Bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // txttblBank_Group_Group
            // 
            this.txttblBank_Group_Group.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Group_Group.Location = new System.Drawing.Point(140, 77);
            this.txttblBank_Group_Group.MaxLength = 5;
            this.txttblBank_Group_Group.Name = "txttblBank_Group_Group";
            this.txttblBank_Group_Group.Size = new System.Drawing.Size(75, 23);
            this.txttblBank_Group_Group.TabIndex = 1;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(30, 79);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(86, 19);
            this.metroLabel4.TabIndex = 55;
            this.metroLabel4.Text = "Group Name";
            // 
            // txttblBank_Group_Nama
            // 
            this.txttblBank_Group_Nama.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblBank_Group_Nama.Location = new System.Drawing.Point(140, 106);
            this.txttblBank_Group_Nama.MaxLength = 50;
            this.txttblBank_Group_Nama.Name = "txttblBank_Group_Nama";
            this.txttblBank_Group_Nama.Size = new System.Drawing.Size(503, 23);
            this.txttblBank_Group_Nama.TabIndex = 2;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(30, 108);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(77, 19);
            this.metroLabel1.TabIndex = 57;
            this.metroLabel1.Text = "Nama Bank";
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
            this.mPanel_Bottom.Location = new System.Drawing.Point(27, 165);
            this.mPanel_Bottom.Name = "mPanel_Bottom";
            this.mPanel_Bottom.Size = new System.Drawing.Size(634, 32);
            this.mPanel_Bottom.TabIndex = 58;
            this.mPanel_Bottom.VerticalScrollbarBarColor = true;
            this.mPanel_Bottom.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Bottom.VerticalScrollbarSize = 10;
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(553, 5);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(75, 23);
            this.BtnDelete.TabIndex = 18;
            this.BtnDelete.Text = "&Delete";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(391, 5);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 16;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(472, 5);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 17;
            this.BtnSave.Text = "Sa&ve";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnEdit
            // 
            this.BtnEdit.Location = new System.Drawing.Point(313, 5);
            this.BtnEdit.Name = "BtnEdit";
            this.BtnEdit.Size = new System.Drawing.Size(75, 23);
            this.BtnEdit.TabIndex = 15;
            this.BtnEdit.Text = "&Edit";
            this.BtnEdit.UseVisualStyleBackColor = true;
            this.BtnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // FrmM_BankGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 222);
            this.Controls.Add(this.mPanel_Bottom);
            this.Controls.Add(this.txttblBank_Group_Nama);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.txttblBank_Group_Group);
            this.Controls.Add(this.metroLabel4);
            this.Font = new System.Drawing.Font("Courier New", 10F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmM_BankGroup";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Resizable = false;
            this.Text = "Master Group Bank";
            this.Load += new System.EventHandler(this.FrmM_BankGroup_Load);
            this.Shown += new System.EventHandler(this.FrmM_BankGroup_Shown);
            this.mPanel_Bottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txttblBank_Group_Group;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private System.Windows.Forms.TextBox txttblBank_Group_Nama;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroPanel mPanel_Bottom;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnEdit;
    }
}