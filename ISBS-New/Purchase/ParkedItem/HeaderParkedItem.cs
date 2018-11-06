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

namespace ISBS_New.Purchase.ParkedItem
{
    public partial class HeaderParkedItem : MetroFramework.Forms.MetroForm
    {
        public string GoodsReceivedNumber = "";
        public string NotaNumber = "";
        public string EditFrom = "";

        List<DetailParkedItem> ListDetailParkedItem = new List<DetailParkedItem>();


        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        int Index;

        string Mode, Query, crit  = null;

        List<byte[]> ListAttachment = new List<byte[]>();
        List<string> sSelectedFile, FileName, Extension;

        Purchase.ParkedItem.InquiryParkedItem Parent;

        public HeaderParkedItem()
        {
            InitializeComponent();            
        }

        public void SetParent(Purchase.ParkedItem.InquiryParkedItem F)
        {
            Parent = F;
        }

        public void SetMode(string tmpMode, string tmpGoodsReceivedNumber, string tmpVendorID, string tmpVendorName)
        {
            Mode = tmpMode;
            GoodsReceivedNumber = tmpGoodsReceivedNumber;
            txtGoodsReceivedNumber.Text = tmpGoodsReceivedNumber;
            txtVendorID.Text = tmpVendorID;
            txtVendorName.Text = tmpVendorName;
        }

        private void HeaderParkedItem_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
            
            cmbAction.DisplayMember = "Text";
            cmbAction.ValueMember = "Value";

            var items = new[] { 
                new { Text = "-select-", Value = "-select-" }, 
                new { Text = "Retur Debit", Value = "01" }, 
                new { Text = "Retur Tukar Barang", Value = "02" },
                new { Text = "New Purchase", Value = "03" }
            };

            cmbAction.DataSource = items;
            cmbAction.DropDownStyle = ComboBoxStyle.DropDownList;

            ModeBeforeEdit();

            Purchase.ParkedItem.InquiryParkedItem f = new Purchase.ParkedItem.InquiryParkedItem();
            f.RefreshGrid();
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            string NotaNumber = "";           
            NotaNumber = GetNotaNumber(GoodsReceivedNumber);

            if (EditFrom == "")
            {
                NotaNumber = "0";
            }
                      
            if (NotaNumber == "0")
           {
                btnSave.Visible = true;
                btnExit.Visible = true;
                btnEdit.Visible = false;
                btnCancel.Visible = false;

                btnNew.Enabled = true;
                btnDelete.Enabled = true;
                btnDownload.Enabled = true;
                btnUpload.Enabled = true;

                dgvParkedItemDetails.ReadOnly = false;
                dgvParkedItemDetails.DefaultCellStyle.BackColor = Color.White;
                dgvAttachment.DefaultCellStyle.BackColor = Color.White;

                dgvHeaderAttachment();
           }
            else
            {
                btnSave.Visible = false;
                btnExit.Visible = true;
                btnEdit.Visible = true;
                btnCancel.Visible = false;

                btnNew.Enabled = false;
                btnDelete.Enabled = false;
                btnDownload.Enabled = false;
                btnUpload.Enabled = false;
                btnDeleteFile.Enabled = false;
                cmbAction.Enabled = false;

                dgvParkedItemDetails.ReadOnly = true;
                dgvParkedItemDetails.DefaultCellStyle.BackColor = Color.LightGray;
                dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;

                dgvHeaderAttachment();

                SetHeaderNotaPurchaseparked();
                SetValueAttachment();
            }
           
            BeforeEditColor();  
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

        private string GetNotaNumber(string GoodReceivedNumber)
        {
            string CountNPPID = "";

            Conn = ConnectionString.GetConnection();

            Query = "Select COUNT(NPPID) AS NPPID From NotaPurchaseParkH ";
            Query += "Where GoodsReceivedID = '" + GoodReceivedNumber + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                CountNPPID = Dr["NPPID"].ToString();
            }
            Dr.Close();

            return CountNPPID;
        }

        public void RefreshGridDetail()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();

            dgvParkedItemDetails.Rows.Clear();
            if (dgvParkedItemDetails.RowCount - 1 <= 0)
            {
                dgvParkedItemDetails.ColumnCount = 8;
                dgvParkedItemDetails.Columns[0].Name = "No";
                dgvParkedItemDetails.Columns[1].Name = "FullItemID";
                dgvParkedItemDetails.Columns[2].Name = "ItemName";
                dgvParkedItemDetails.Columns[3].Name = "Qty";
                dgvParkedItemDetails.Columns[4].Name = "Unit";
                dgvParkedItemDetails.Columns[5].Name = "Price";
                dgvParkedItemDetails.Columns[6].Name = "Notes";
                dgvParkedItemDetails.Columns[7].Name = "SeqNo";
            }

            dgvParkedItemDetails.Columns["No"].ReadOnly = true;
            dgvParkedItemDetails.Columns["FullItemID"].ReadOnly = true;
            dgvParkedItemDetails.Columns["ItemName"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Qty"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Unit"].ReadOnly = true;
            dgvParkedItemDetails.Columns["SeqNo"].Visible = false;
           
            dgvParkedItemDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["SeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;


            if (txtNotaNumber.Text == "")
            {
            Query = "SELECT No, FullItemId, ItemName, Qty_Actual AS Qty, Unit, '0.0000' AS Price, '' AS Notes, GoodsReceivedSeqNo AS SeqNo ";
            Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY GoodsReceivedId desc) No, FullItemId, ItemName, Qty_Actual, Unit, GoodsReceivedSeqNo  FROM GoodsReceivedD WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND ActionCodeStatus = '02') a ORDER BY a.GoodsReceivedSeqNo ASC ";
            }
            else
            {
                Query = "SELECT No, FullItemID, ItemName, Qty, Unit, Price, Notes, SeqNo ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY NPPID DESC) No, FullItemID, ItemName, Qty, Unit, Price, Notes, SeqNo FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text + "') a ORDER BY a.SeqNo ASC";
            }
          
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                this.dgvParkedItemDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7]);

                //Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemID = '" + Dr[1].ToString() + "' ";
                //Cmd = new SqlCommand(Query, Conn);
                //SqlDataReader DrCmb;
                //DrCmb = Cmd.ExecuteReader();
                //DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                //while (DrCmb.Read())
                //{
                //    if (DrCmb[0] != null)
                //        combo.Items.Add(DrCmb[0].ToString());
                //    if (DrCmb[1] != null)
                //        combo.Items.Add(DrCmb[1].ToString());
                //}
                //if (Dr[4] != null)
                //{
                //    combo.Value = Dr[4].ToString();
                //}
                //dgvParkedItemDetails.Rows[i].Cells[4] = combo;

                //DrCmb.Close();

                i++;
            }
            Dr.Close();

            Conn.Close();

                     
        }

        private void BeforeEditColor()
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                //dgvParkedItemDetails.Rows[i].Cells["CustID"].Style.BackColor = Color.LightGray;
                //dgvParkedItemDetails.Rows[i].Cells["CustName"].Style.BackColor = Color.LightGray;
                //dgvParkedItemDetails.Rows[i].Cells["CreditAmount"].Style.BackColor = Color.LightGray;
            }
        }

        private void HeaderParkedItem_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void HeaderParkedItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            //for (int i = 0; i < ListDetailMoUCustomer.Count(); i++)
            //{
            //    ListDetailMoUCustomer[i].Close();
            //}
            Purchase.ParkedItem.InquiryParkedItem f = new Purchase.ParkedItem.InquiryParkedItem();
            f.RefreshGrid();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvParkedItemDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvParkedItemDetails.Columns["Price"].Index && e.Value != null)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }

            if (e.ColumnIndex == dgvParkedItemDetails.Columns["Qty"].Index && e.Value != null)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
        }

        private void dgvParkedItemDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvParkedItemDetails.Columns[dgvParkedItemDetails.CurrentCell.ColumnIndex].Name == "Price" || dgvParkedItemDetails.Columns[dgvParkedItemDetails.CurrentCell.ColumnIndex].Name == "Qty")
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

        private void dgvParkedItemDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvParkedItemDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvParkedItemDetails_KeyPress);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {

            if (dgvParkedItemDetails.RowCount > 0)
            {
                Index = dgvParkedItemDetails.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "No = " + dgvParkedItemDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FullItemId = " + dgvParkedItemDetails.Rows[Index].Cells["FullItemId"].Value.ToString() + Environment.NewLine + "ItemName = " + dgvParkedItemDetails.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvParkedItemDetails.Rows.RemoveAt(Index);
                    SortNoDataGrid();
                }
            }
            else {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;

            }
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                dgvParkedItemDetails.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            DetailParkedItem DetailParkedItem = new DetailParkedItem();

            List<DetailParkedItem> ListDetailParkedItem = new List<DetailParkedItem>();
            DetailParkedItem.ParamHeader(txtGoodsReceivedNumber.Text, dgvParkedItemDetails, EditFrom, txtNotaNumber.Text);
            DetailParkedItem.ParentRefreshGrid(this);
            DetailParkedItem.ShowDialog();
            EditColor();
        }

        private void EditColor()
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                dgvParkedItemDetails.Rows[i].Cells["Qty"].Style.BackColor = Color.White;
                dgvParkedItemDetails.Rows[i].Cells["Unit"].Style.BackColor = Color.White;
                dgvParkedItemDetails.Rows[i].Cells["Price"].Style.BackColor = Color.White;
                dgvParkedItemDetails.Rows[i].Cells["Notes"].Style.BackColor = Color.White;
            }
        }

        public void AddDataGridDetail(List<string> FullItemId, List<string> ItemName, List<string> Qty, List<string> Unit, List<string> SeqNo)
        {
            for (int i = 0; i < FullItemId.Count; i++)
            {
                this.dgvParkedItemDetails.Rows.Add((dgvParkedItemDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], Qty[i], Unit[i], "0.0000", "", SeqNo[i]);
                int j = dgvParkedItemDetails.RowCount - 1;
                
                Conn = ConnectionString.GetConnection();

                //Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemID = '" + FullItemId[i].ToString() + "' ";
                //Cmd = new SqlCommand(Query, Conn);
                //SqlDataReader DrCmb;
                //DrCmb = Cmd.ExecuteReader();
                //DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                //while (DrCmb.Read())
                //{
                //    if (DrCmb[0] != null)
                //        combo.Items.Add(DrCmb[0].ToString());
                //    if (DrCmb[1] != null)
                //        combo.Items.Add(DrCmb[1].ToString());
                //}
                //if (Unit[i] != null)
                //{
                //    combo.Value = Unit[i].ToString();
                //}
                //dgvParkedItemDetails.Rows[j].Cells[4] = combo;

                //DrCmb.Close();
            
            }           
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            if (dgvParkedItemDetails.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }
            else
            {
                for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
                {
                    if (Convert.ToDecimal((dgvParkedItemDetails.Rows[i].Cells["Qty"].Value == "" ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Qty"].Value.ToString())) <= 0)
                    {

                        MessageBox.Show("Item No = " + dgvParkedItemDetails.Rows[i].Cells["No"].Value + ", Qty tidak boleh lebih kecil atau sama dengan 0");
                        return;
                    }
                    else if ((dgvParkedItemDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["Unit"].Value.ToString()) == "")
                    {

                        MessageBox.Show("Item No = " + dgvParkedItemDetails.Rows[i].Cells["No"].Value + ", Unit tidak boleh kosong.");
                        return;
                    }
                    else if (Convert.ToDecimal((dgvParkedItemDetails.Rows[i].Cells["Price"].Value == "" ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Price"].Value.ToString())) <= 0)
                    {

                        MessageBox.Show("Item No = " + dgvParkedItemDetails.Rows[i].Cells["No"].Value + ", Price tidak boleh lebih kecil atau sama dengan 0");
                        return;
                    }
                    else if ((dgvParkedItemDetails.Rows[i].Cells["Notes"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["Notes"].Value.ToString()) == "")
                    {

                        MessageBox.Show("Item No = " + dgvParkedItemDetails.Rows[i].Cells["No"].Value + ", Notes tidak boleh kosong.");
                        return;
                    }
               }
               
               if (cmbAction.SelectedIndex == 0)
               {
                   MessageBox.Show("Action harus diisi");
                   return;
               }

               if (dgvAttachment.RowCount == 0)
               {
                   MessageBox.Show("File attachment harus diupload");
                   return;
               }
            }

            try
            {
                string ActionCodeStatus = cmbAction.SelectedValue.ToString();

                if (ActionCodeStatus == "01")
                {
                    ActionCodeStatus = "07";
                }
                else if (ActionCodeStatus == "02")
                {
                    ActionCodeStatus = "08";
                }
                else if (ActionCodeStatus == "03")
                {
                    ActionCodeStatus = "06";
                } 

                if (Mode == "New" || txtNotaNumber.Text == "")
                {
             
                    Query = "Insert into NotaPurchaseParkH (NPPID, NPPDate, GoodsReceivedID, VendID, ActionCode, TransCode,CreatedDate, CreatedBy) OUTPUT INSERTED.NPPID values ";
                    Query += "((Select 'NPP/'+FORMAT(getdate(), 'yyMM')+'/'+Right('00000' + CONVERT(NVARCHAR, case when Max(NPPID) is null then '1' else substring(Max(NPPID),11,5)+1 end), 5) ";
                    Query += "from [NotaPurchaseParkH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', '" + txtGoodsReceivedNumber.Text + "', '" + txtVendorID.Text + "','" + cmbAction.SelectedValue.ToString() + "',";
                    Query += "'01', getdate(),'" + ControlMgr.UserId + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string NotaNumber = Cmd.ExecuteScalar().ToString();                    
                   
                                       

                    Query = "";
                    for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
                    {
                       // int SeqNo = i+1;
                        Query += "Insert NotaPurchaseParkD (NPPID, SeqNo, FullItemID, ItemName, Qty, Unit, Price, Notes, CreatedDate, CreatedBy) Values ";
                        Query += "('" + NotaNumber + "',";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value.ToString()) + ",'";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["FullItemId"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["Qty"].Value == null ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Qty"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["Unit"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["Price"].Value == null ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Price"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["Notes"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["Notes"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "');";
                      
                        Query += "UPDATE GoodsReceivedD SET ActionCodeStatus = '" + ActionCodeStatus + "' ";
                        Query += ",UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GoodsReceivedSeqNo = '" + dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "' ;";

                       
                        if (i % 5 == 0 && i > 0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            Query = "";
                        }
                    }

                    if (Query != "")
                    {
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }

                    

                    for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                    {
                        Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                        Query += "( 'NotaPurchaseParkH', '" + NotaNumber + "', '";
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
                    Query += "VALUES('NotaPurchaseParkedH', '" + NotaNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '01', '" + StatusDesc + "', getdate()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data Nota Number : " + NotaNumber + " berhasil ditambahkan.");
                    txtNotaNumber.Text = NotaNumber;                   
                }
                else
                {
                    Query = "Delete from tblAttachments where ReffTransID='" + txtNotaNumber.Text.Trim() + "';";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

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

                    Query = "Update NotaPurchaseParkH set ";
                    Query += "NPPDate='" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "ActionCode='" + cmbAction.SelectedValue + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where NPPID='" + txtNotaNumber.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();


                    Query = "UPDATE GoodsReceivedD SET ActionCodeStatus = '02' ";
                    Query += ",UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GoodsReceivedSeqNo IN (SELECT SeqNo FROM NotaPurchaseParkD WHERE NPPID = '"+txtNotaNumber.Text+"') AND ActionCodeStatus IN ('06', '07', '08') ;";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();   

                    Query = "Delete from NotaPurchaseParkD where NPPID='" + txtNotaNumber.Text.Trim() + "';";
                    for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
                    {
                        //int SeqNo = i + 1;

                        Query += "Insert NotaPurchaseParkD (NPPID, SeqNo, FullItemID, ItemName, Qty, Unit, Price, Notes, CreatedDate, CreatedBy) Values ";
                        Query += "('" + txtNotaNumber.Text + "',";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value.ToString()) + ",'";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["FullItemId"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["Qty"].Value == null ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Qty"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["Unit"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["Price"].Value == null ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Price"].Value.ToString()) + "','";
                        Query += (dgvParkedItemDetails.Rows[i].Cells["Notes"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["Notes"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "');";

                        Query += "UPDATE GoodsReceivedD SET ActionCodeStatus = '" + ActionCodeStatus + "' ";
                        Query += ",UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GoodsReceivedSeqNo = '" + dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "' ;";
                      


                        if (i % 5 == 0 && i > 0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            Query = "";
                        }
                    }
                    if (Query != "")
                    {
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("Data Nota Number : " + txtNotaNumber.Text + " berhasil diupdate.");
                }

                EditFrom = "Header";
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
                ModeBeforeEdit();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Mode = "Edit";
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;           

            ModeEdit();
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnNew.Enabled = true;
            btnDelete.Enabled = true;                     
            btnDeleteFile.Enabled = true;
            cmbAction.Enabled = true;
            btnDownload.Enabled = true;
            btnUpload.Enabled = true;

            dgvParkedItemDetails.ReadOnly = false;
            dgvParkedItemDetails.Columns["No"].ReadOnly = true;
            dgvParkedItemDetails.Columns["FullItemId"].ReadOnly = true;
            dgvParkedItemDetails.Columns["ItemName"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Qty"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Unit"].ReadOnly = true;

            dgvParkedItemDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["FullItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;


            dgvParkedItemDetails.AutoResizeColumns();
            EditColor();
            dgvParkedItemDetails.DefaultCellStyle.BackColor = Color.White;
            dgvAttachment.DefaultCellStyle.BackColor = Color.White;
        }

        private void SetHeaderNotaPurchaseparked()
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select NPPDate, NPPID, VendID, ActionCode FROM NotaPurchaseParkH ";
            Query += "Where NPPID = '" + txtNotaNumber.Text + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtNotaDate.Text = Dr["NPPDate"].ToString();
                txtNotaNumber.Text = Dr["NPPID"].ToString();
                txtVendorID.Text = Dr["VendID"].ToString();
                cmbAction.SelectedValue = Dr["ActionCode"].ToString();
            }
            Dr.Close();

            Query = "Select VendName FROM VendTable ";
            Query += "Where VendID = '" + txtVendorID.Text + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtVendorName.Text = Dr["VendName"].ToString();
            }
            Dr.Close();
        }

        private void SetValueAttachment()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();         

            if (txtNotaNumber.Text != "")
            {
                Query = "SELECT fileName, ContentType, fileSize, attachment, id ";
                Query += "FROM tblAttachments WHERE ReffTransID = '" + txtNotaNumber.Text + "' ORDER BY id DESC";
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr[0], Dr[1], Dr[2], "", Dr[4]);
                ListAttachment.Add((byte[])Dr[3]);

                i++;
            }
            Dr.Close();   
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            ModeBeforeEdit();
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

    }
}
