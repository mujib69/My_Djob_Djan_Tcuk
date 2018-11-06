namespace ISBS_New.Master.ItemDP
{
    partial class FrmM_ItemDP
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
            this.txttblItemDP_Ket = new System.Windows.Forms.TextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.mPanel_Bottom.SuspendLayout();
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
            this.mPanel_Bottom.Location = new System.Drawing.Point(27, 265);
            this.mPanel_Bottom.Name = "mPanel_Bottom";
            this.mPanel_Bottom.Size = new System.Drawing.Size(647, 32);
            this.mPanel_Bottom.TabIndex = 1;
            this.mPanel_Bottom.VerticalScrollbarBarColor = true;
            this.mPanel_Bottom.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel_Bottom.VerticalScrollbarSize = 10;
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(561, 5);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(75, 23);
            this.BtnDelete.TabIndex = 18;
            this.BtnDelete.Text = "&Delete";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(399, 5);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 16;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(480, 5);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 17;
            this.BtnSave.Text = "Sa&ve";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnEdit
            // 
            this.BtnEdit.Location = new System.Drawing.Point(321, 5);
            this.BtnEdit.Name = "BtnEdit";
            this.BtnEdit.Size = new System.Drawing.Size(75, 23);
            this.BtnEdit.TabIndex = 15;
            this.BtnEdit.Text = "&Edit";
            this.BtnEdit.UseVisualStyleBackColor = true;
            this.BtnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // txttblItemDP_Ket
            // 
            this.txttblItemDP_Ket.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txttblItemDP_Ket.Location = new System.Drawing.Point(147, 77);
            this.txttblItemDP_Ket.MaxLength = 255;
            this.txttblItemDP_Ket.Multiline = true;
            this.txttblItemDP_Ket.Name = "txttblItemDP_Ket";
            this.txttblItemDP_Ket.Size = new System.Drawing.Size(524, 156);
            this.txttblItemDP_Ket.TabIndex = 52;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(28, 77);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(74, 19);
            this.metroLabel2.TabIndex = 53;
            this.metroLabel2.Text = "Description";
            // 
            // FrmM_ItemDP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 322);
            this.Controls.Add(this.txttblItemDP_Ket);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.mPanel_Bottom);
            this.Font = new System.Drawing.Font("Courier New", 10F);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmM_ItemDP";
            this.Padding = new System.Windows.Forms.Padding(27, 74, 27, 25);
            this.Resizable = false;
            this.Text = "Master Item DP";
            this.Load += new System.EventHandler(this.FrmM_ItemDP_Load);
            this.Shown += new System.EventHandler(this.FrmM_ItemDP_Shown);
            this.mPanel_Bottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel mPanel_Bottom;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnEdit;
        private System.Windows.Forms.TextBox txttblItemDP_Ket;
        private MetroFramework.Controls.MetroLabel metroLabel2;
    }
}