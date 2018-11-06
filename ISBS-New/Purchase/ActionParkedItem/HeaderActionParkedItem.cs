using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace ISBS_New.Purchase.ActionParkedItem
{
    public partial class HeaderActionParkedItem : MetroFramework.Forms.MetroForm
    {
        public string GoodsReceivedNumber, NotaNumber = "";

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        int Index, SelectedCell;
        private DataGridViewComboBoxCell combo = null;

        string Query, crit = null;

        List<byte[]> ListAttachment = new List<byte[]>();
        List<string> sSelectedFile, FileName, Extension;

        Purchase.ActionParkedItem.InquiryActionParkedItem Parent;

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void SetParent(Purchase.ActionParkedItem.InquiryActionParkedItem F)
        {
            Parent = F;
        }

        public void SetMode(string tmpGoodsReceivedNumber, string tmpVendorID, string tmpVendorName, string tmpNPPID, string tmpNPPDate)
        {
            GoodsReceivedNumber = tmpGoodsReceivedNumber;
            txtGoodsReceivedNumber.Text = tmpGoodsReceivedNumber;
            txtVendorID.Text = tmpVendorID;
            txtVendorName.Text = tmpVendorName;
            txtNotaNumber.Text = tmpNPPID;
            NotaNumber = tmpNPPID;
            dtNotaDate.Text = tmpNPPDate;
        }

        public HeaderActionParkedItem()
        {
            InitializeComponent();
        }

        private void HeaderActionParkedItem_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void HeaderActionParkedItem_FormClosed(object sender, FormClosedEventArgs e)
        {
           // Purchase.ActionParkedItem.InquiryActionParkedItem f = new Purchase.ActionParkedItem.InquiryActionParkedItem();
           // f.RefreshGrid();
        }

        private void HeaderActionParkedItem_Load(object sender, EventArgs e)
        {
            lblForm.Location = new Point(16, 11);            

            ModeBeforeEdit();

           // Purchase.ActionParkedItem.InquiryActionParkedItem f = new Purchase.ActionParkedItem.InquiryActionParkedItem();
            //f.RefreshGrid();
        }

        public void ModeBeforeEdit()
        {
            dgvActionParkedItemDetails.ReadOnly = false;
            dgvActionParkedItemDetails.DefaultCellStyle.BackColor = Color.White;
            dgvAttachment.DefaultCellStyle.BackColor = Color.White;

            dgvHeaderAttachment();
            RefreshGridDetail();
        }

        private void dgvHeaderAttachment()
        {
            dgvAttachment.Rows.Clear();
            if (dgvAttachment.RowCount - 1 <= 0)
            {
                dgvAttachment.ColumnCount = 5;
                dgvAttachment.Columns[0].Name = "FileName";
                dgvAttachment.Columns[1].Name = "Extension";
                dgvAttachment.Columns[2].Name = "FileSize";
                dgvAttachment.Columns[3].Name = "Attachment";
                dgvAttachment.Columns[4].Name = "Id";
            }
            dgvAttachment.Columns["FileName"].HeaderText = "File Name";
            dgvAttachment.Columns["Extension"].HeaderText = "Extension";
            dgvAttachment.Columns["FileSize"].HeaderText = "File Size (kb)";
            dgvAttachment.Columns["Id"].HeaderText = "Id";

            dgvAttachment.Columns["Attachment"].Visible = false;
            dgvAttachment.Columns["Id"].Visible = false;
        }

        public void RefreshGridDetail()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();

            dgvActionParkedItemDetails.Rows.Clear();
            if (dgvActionParkedItemDetails.RowCount - 1 <= 0)
            {
                dgvActionParkedItemDetails.ColumnCount = 9;
                dgvActionParkedItemDetails.Columns[0].Name = "No";
                dgvActionParkedItemDetails.Columns[1].Name = "FullItemID";
                dgvActionParkedItemDetails.Columns[2].Name = "ItemName";
                dgvActionParkedItemDetails.Columns[3].Name = "Qty";
                dgvActionParkedItemDetails.Columns[4].Name = "Unit";
                dgvActionParkedItemDetails.Columns[5].Name = "ActionCode";
                dgvActionParkedItemDetails.Columns[6].Name = "Price";
                dgvActionParkedItemDetails.Columns[7].Name = "Notes";
                dgvActionParkedItemDetails.Columns[8].Name = "SeqNo";
            }

            dgvActionParkedItemDetails.Columns["No"].ReadOnly = true;
            dgvActionParkedItemDetails.Columns["FullItemID"].ReadOnly = true;
            dgvActionParkedItemDetails.Columns["ItemName"].ReadOnly = true;
            dgvActionParkedItemDetails.Columns["Qty"].ReadOnly = true;
            dgvActionParkedItemDetails.Columns["Unit"].ReadOnly = true;
            dgvActionParkedItemDetails.Columns["Price"].ReadOnly = true;
            dgvActionParkedItemDetails.Columns["SeqNo"].Visible = false;

            dgvActionParkedItemDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvActionParkedItemDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvActionParkedItemDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvActionParkedItemDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvActionParkedItemDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvActionParkedItemDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvActionParkedItemDetails.Columns["ActionCode"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvActionParkedItemDetails.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvActionParkedItemDetails.Columns["SeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvActionParkedItemDetails.AutoResizeColumns();


            Query = "SELECT No, FullItemID, ItemName, Qty, Unit, ActionCode, Price, Notes, SeqNo ";
            Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY NPPID DESC) No, FullItemID, ItemName, Qty, Unit, Price, ActionCode, Notes, SeqNo FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text + "' AND GoodsReceivedID = '"+txtGoodsReceivedNumber.Text+"') a ORDER BY a.SeqNo ASC";         
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                this.dgvActionParkedItemDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8]);

                Query = "SELECT StatusCode AS ActionCode, Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaPurchaseParkD'";
                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader DrCmb;
                DrCmb = Cmd.ExecuteReader();
                combo = new DataGridViewComboBoxCell();
                while (DrCmb.Read())
                {
                   combo.Items.Add(DrCmb[1].ToString());
                }
                if (Dr[5] != null)
                {
                    combo.Value = Dr[5].ToString();
                }
                dgvActionParkedItemDetails.Rows[i].Cells[5] = combo;

                DrCmb.Close();

                i++;
            }
            Dr.Close();

            Conn.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //DetailActionParkedItem DetailActionParkedItem = new DetailActionParkedItem();

            //List<DetailActionParkedItem> ListDetailActionParkedItem = new List<DetailActionParkedItem>();
            //DetailActionParkedItem.ParamHeader(txtGoodsReceivedNumber.Text, dgvActionParkedItemDetails, txtNotaNumber.Text);
            //DetailActionParkedItem.ParentRefreshGrid(this);
            //DetailActionParkedItem.ShowDialog();
            //EditColor();
        }

        private void EditColor()
        {
            for (int i = 0; i < dgvActionParkedItemDetails.RowCount; i++)
            {
                dgvActionParkedItemDetails.Rows[i].Cells["Qty"].Style.BackColor = Color.White;
                dgvActionParkedItemDetails.Rows[i].Cells["Unit"].Style.BackColor = Color.White;
                dgvActionParkedItemDetails.Rows[i].Cells["Price"].Style.BackColor = Color.White;
                dgvActionParkedItemDetails.Rows[i].Cells["Notes"].Style.BackColor = Color.White;
            }
        }

        //public void AddDataGridDetail(List<string> FullItemId, List<string> ItemName, List<string> Qty, List<string> Unit, List<string> SeqNo, List<string> ActionCode, List<string> Price, List<string> Notes)
        //{
        //    for (int i = 0; i < FullItemId.Count; i++)
        //    {
        //        this.dgvActionParkedItemDetails.Rows.Add((dgvActionParkedItemDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], Qty[i], Unit[i], ActionCode[i], Price[i], Notes[i], SeqNo[i]);

        //        Query = "SELECT StatusCode AS ActionCode, Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaPurchaseParkD'";
        //        Cmd = new SqlCommand(Query, Conn);
        //        SqlDataReader DrCmb;
        //        DrCmb = Cmd.ExecuteReader();
        //        combo = new DataGridViewComboBoxCell();
        //        while (DrCmb.Read())
        //        {
        //            combo.Items.Add(DrCmb[1].ToString());
        //        }
        //        if (ActionCode[i] != null)
        //        {
        //            combo.Value = ActionCode[i].ToString();
        //        }
        //        dgvActionParkedItemDetails.Rows[i].Cells[5] = combo;

        //        DrCmb.Close();

        //        i++;
        //    }
        //}

        //private void btnDelete_Click(object sender, EventArgs e)
        //{
        //    if (dgvActionParkedItemDetails.RowCount > 0)
        //    {
        //        Index = dgvActionParkedItemDetails.CurrentRow.Index;
        //        DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "No = " + dgvActionParkedItemDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FullItemId = " + dgvActionParkedItemDetails.Rows[Index].Cells["FullItemId"].Value.ToString() + Environment.NewLine + "ItemName = " + dgvActionParkedItemDetails.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
        //        if (dialogResult == DialogResult.Yes)
        //        {
        //            dgvActionParkedItemDetails.Rows.RemoveAt(Index);
        //            SortNoDataGrid();
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Silahkan pilih data untuk dihapus");
        //        return;
        //    }
        //}

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvActionParkedItemDetails.RowCount; i++)
            {
                dgvActionParkedItemDetails.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            choofdlog.FilterIndex = 3;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                FileName = new List<string>();
                Extension = new List<string>();
                sSelectedFile = new List<string>();

                int i = 0;

                foreach (string file in choofdlog.FileNames)
                {
                    FileStream objFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    int filelength = Convert.ToInt32(objFileStream.Length);
                    byte[] data = new byte[filelength];

                    objFileStream.Read(data, 0, filelength);
                    objFileStream.Close();

                    string tempFullName = Path.GetFileName(file);
                    string[] tempSplit = tempFullName.Split('.');

                    FileName.Add(tempSplit[0]);
                    Extension.Add(tempSplit[tempSplit.Count() - 1]);
                    int filesize = filelength / 1024;
                    this.dgvAttachment.Rows.Add(FileName[i], Extension[i], filesize.ToString(), System.Text.Encoding.UTF8.GetString(data));
                    ListAttachment.Add(data);
                    i++;
                }
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                String fileid = dgvAttachment.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Id"].Value.ToString();
                String fileName = dgvAttachment.CurrentRow.Cells["FileName"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileName"].Value.ToString();
                String ContentType = dgvAttachment.CurrentRow.Cells["Extension"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Extension"].Value.ToString();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ContentType;
                sfd.FileName = fileName + "." + ContentType;
                sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.AddExtension = true;

                if (ContentType == "pdf")
                {
                    sfd.FilterIndex = 1;
                }
                else if (ContentType == "txt")
                {
                    sfd.FilterIndex = 2;
                }
                else
                {
                    sfd.FilterIndex = 3;
                }

                if (String.IsNullOrEmpty(fileid))
                {
                    MessageBox.Show("File tidak ada dalam database / belum di masukkan.");
                    return;
                }

                Conn = ConnectionString.GetConnection();
                Query = "Select Attachment From tblAttachments Where Id = '" + fileid + "'";
                Cmd = new SqlCommand(Query, Conn);

                byte[] data = (byte[])Cmd.ExecuteScalar();

                if (sfd.ShowDialog() != DialogResult.Cancel)
                {
                    FileStream objFileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                    objFileStream.Write(data, 0, data.Length);
                    objFileStream.Close();
                    MessageBox.Show("Data tersimpan!");
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk didownload");
                return;
            }
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                if (dgvAttachment.RowCount > 0)
                {
                    Index = dgvAttachment.CurrentRow.Index;
                    DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "FileName = " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (dgvAttachment.RowCount > 0)
                        {
                            if (dgvAttachment.CurrentRow.Index > -1)
                            {
                                ListAttachment.RemoveAt(dgvAttachment.CurrentRow.Index);
                                dgvAttachment.Rows.RemoveAt(dgvAttachment.CurrentRow.Index);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;

            }
        }

        private void dgvActionParkeditemDetails_EditControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvActionParkedItemDetails.CurrentCell.ColumnIndex == 5)
            {
                SelectedCell = dgvActionParkedItemDetails.CurrentCell.RowIndex;
                // Check box column
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            }

            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvActionParkedItemDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvActionParkedItemDetails_KeyPress);
            }
        }

        void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = ((ComboBox)sender).SelectedIndex;

            if (selectedIndex == 2)
            {
                dgvActionParkedItemDetails.Rows[SelectedCell].Cells["Price"].ReadOnly = false;
            }
            else
            {
                dgvActionParkedItemDetails.Rows[SelectedCell].Cells["Price"].ReadOnly = true;
                int Seq = Convert.ToInt32(dgvActionParkedItemDetails.Rows[SelectedCell].Cells["SeqNo"].Value);
                decimal Price = GetPrice(Seq);
                dgvActionParkedItemDetails.Rows[SelectedCell].Cells["Price"].Value = Price;

            }
        }

        private decimal GetPrice(int Seq)
        {
            Conn = ConnectionString.GetConnection();
            decimal Result = 0;
            Query = "SELECT Price FROM NotaPurchaseParkD WHERE SeqNo = " + Seq + " AND NPPID = '"+txtNotaNumber.Text.Trim()+"'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr;
            Dr = Cmd.ExecuteReader();
             while (Dr.Read())
            {
                Result = Convert.ToDecimal(Dr[0]);
            }          
            Dr.Close();

            return Result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (dgvActionParkedItemDetails.RowCount == 0)
                {
                    MessageBox.Show("Jumlah item tidak boleh kosong.");
                    return;
                }
                else
                {
                   
                    for (int i = 0; i <= dgvActionParkedItemDetails.RowCount - 1; i++)
                    {
                        string Price = Convert.ToString(dgvActionParkedItemDetails.Rows[i].Cells["Price"].Value);


                        if (Convert.ToDecimal((dgvActionParkedItemDetails.Rows[i].Cells["Qty"].Value == "" ? "0.0000" : dgvActionParkedItemDetails.Rows[i].Cells["Qty"].Value.ToString())) <= 0)
                        {

                            MessageBox.Show("Item No = " + dgvActionParkedItemDetails.Rows[i].Cells["No"].Value + ", Qty tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        }
                        else if ((dgvActionParkedItemDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvActionParkedItemDetails.Rows[i].Cells["Unit"].Value.ToString()) == "")
                        {

                            MessageBox.Show("Item No = " + dgvActionParkedItemDetails.Rows[i].Cells["No"].Value + ", Unit tidak boleh kosong.");
                            return;
                        }
                        else if ((dgvActionParkedItemDetails.Rows[i].Cells["Notes"].Value == null ? "" : dgvActionParkedItemDetails.Rows[i].Cells["Notes"].Value.ToString()) == "")
                        {

                            MessageBox.Show("Item No = " + dgvActionParkedItemDetails.Rows[i].Cells["No"].Value + ", Notes tidak boleh kosong.");
                            return;
                        }

                        else if ((dgvActionParkedItemDetails.Rows[i].Cells["ActionCode"].Value == null ? "" : dgvActionParkedItemDetails.Rows[i].Cells["ActionCode"].Value.ToString()) == "")
                        {

                            MessageBox.Show("Item No = " + dgvActionParkedItemDetails.Rows[i].Cells["No"].Value + ", Action Code tidak boleh kosong.");
                            return;
                        }
                        else if (Convert.ToDecimal((Price == "" ? "0.0000" : Price)) <= 0)
                        {
                            if (Convert.ToString(dgvActionParkedItemDetails.Rows[i].Cells["ActionCode"].Value).ToUpper() == "NEW PURCHASE")
                            {
                                 MessageBox.Show("Item No = " + dgvActionParkedItemDetails.Rows[i].Cells["No"].Value + ", Price tidak boleh lebih kecil atau sama dengan 0");
                                return;
                            }                           
                        }
                    }

                    if (dgvAttachment.RowCount == 0)
                    {
                        MessageBox.Show("File attachment harus diupload");
                        return;
                    }
                }

                try
                {

                    Query = "UPDATE NotaPurchaseParkH SET TransCode = '01', ";
                    Query += "UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    for (int i = 0; i <= dgvActionParkedItemDetails.RowCount - 1; i++)
                    {
                        string ActionValue = Convert.ToString(dgvActionParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
                        string ActionCode = "";
                        string ActionCodeStatus = "";
                        if (ActionValue == "Retur Debit")
                        {
                            ActionCode = "01";
                            ActionCodeStatus = "07";
                        }
                        else if (ActionValue == "Retur Tukar Barang")
                        {
                            ActionCode = "02";
                            ActionCodeStatus = "08";
                        }
                        else if (ActionValue == "New Purchase")
                        {
                            ActionCode = "03";
                            ActionCodeStatus = "06";
                        }

                        Query = "UPDATE NotaPurchaseParkD SET ActionCode = '" + ActionCode + "', ";
                        Query += "Price = '" + dgvActionParkedItemDetails.Rows[i].Cells["Price"].Value + "', ";
                        Query += "Notes = '" + dgvActionParkedItemDetails.Rows[i].Cells["Notes"].Value + "', ";
                        Query += "UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' ";
                        Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND SeqNo = '" + dgvActionParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "'";

                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        int GoodReceivedSeqNo = 0;
                        Query = "SELECT GoodsReceived_SeqNo FROM NotaPurchaseParkD ";
                        Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND SeqNo = '" + dgvActionParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            GoodReceivedSeqNo = Convert.ToInt32(Dr["GoodsReceived_SeqNo"]);
                        }
                        Dr.Close();

                        Query = "UPDATE GoodsReceivedD SET ActionCodeStatus = '" + ActionCodeStatus + "' ";
                        Query += ",UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GoodsReceivedSeqNo = '" + GoodReceivedSeqNo + "' ;";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                    }

                    for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                    {
                        Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                        Query += "( 'NotaPurchaseParkH', '" + txtNotaNumber.Text.Trim() + "', '";
                        Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                        Query += dgvAttachment.Rows[i].Cells["Extension"].Value.ToString() + "', '";
                        Query += dgvAttachment.Rows[i].Cells["FileSize"].Value.ToString();
                        Query += "',@binaryValue";
                        Query += ");";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, ListAttachment[i].Length).Value = ListAttachment[i];
                        Cmd.ExecuteNonQuery();
                    }

                    String StatusDesc = null;
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='01' AND TransCode = 'NotaPurchasePark' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                    }
                    Dr.Close();

                    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                    Query += "VALUES('NotaPurchaseParkedH', '" + txtNotaNumber.Text.Trim() + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '01', '" + StatusDesc + "', getdate()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data Nota Number : " + txtNotaNumber.Text.Trim() + " berhasil ditambahkan.");

                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return;
                }
                finally
                {
                    Conn.Close();
                    Parent.RefreshGrid();
                    ModeAfterSave();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void ModeAfterSave()
        {
            dgvActionParkedItemDetails.ReadOnly = true;
            dgvAttachment.ReadOnly = true;
            dgvActionParkedItemDetails.DefaultCellStyle.BackColor = Color.LightGray;
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;

            btnSave.Enabled = false;
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDeleteFile.Enabled = false;
        }

        private void dgvActionParkedItemDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvActionParkedItemDetails.Columns["Price"].Index && e.Value != null)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }

            if (e.ColumnIndex == dgvActionParkedItemDetails.Columns["Qty"].Index && e.Value != null)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
        }

        private void dgvActionParkedItemDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvActionParkedItemDetails.Columns[dgvActionParkedItemDetails.CurrentCell.ColumnIndex].Name == "Price" || dgvActionParkedItemDetails.Columns[dgvActionParkedItemDetails.CurrentCell.ColumnIndex].Name == "Qty")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

            }
        }
    }
}
