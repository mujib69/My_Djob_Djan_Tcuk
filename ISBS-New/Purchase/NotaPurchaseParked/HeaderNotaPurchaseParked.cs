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

namespace ISBS_New.Purchase.NotaPurchaseParked
{
    public partial class HeaderNotaPurchaseParked : MetroFramework.Forms.MetroForm
    {
        public string GoodsReceivedNumber = "";
        public string NPPID = "";
        public string NPPDate = "";
        public string NotaNumber = "";
       // public string Action = "";
        public string TransCode = "";
        bool Journal = false;

        List<DetailParkedItem> ListDetailParkedItem = new List<DetailParkedItem>();


        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataReader DrDetail;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        int Index, SelectedCell;

        string Mode, Query, crit  = null;

        List<byte[]> ListAttachment = new List<byte[]>();
        List<string> sSelectedFile, FileName, Extension;
        private DataGridViewComboBoxCell combo = null;
        ContextMenu vendid = new ContextMenu();
        Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked Parent;
        string ReturDNRefNumber = "";
        string ReturTBRefNumber = "";
        string ReturKBRefNumber = "";
        string GRARefNumber = "";
        string BarangBonusRefNumber = "";

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end


        public HeaderNotaPurchaseParked()
        {
            InitializeComponent();            
        }

        public void SetParent(Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked F)
        {
            Parent = F;
        }

        public void SetMode(string tmpMode, string tmpGoodsReceivedNumber, string tmpVendorID, string tmpVendorName, string tmpNPPID, string tmpNPPDate, string tmpTransCode, string tmpApprovalNotes)
        {
            Mode = tmpMode;
            GoodsReceivedNumber = tmpGoodsReceivedNumber;
            txtGoodsReceivedNumber.Text = tmpGoodsReceivedNumber;
            txtVendorID.Text = tmpVendorID;
            txtVendorName.Text = tmpVendorName;
            txtNotaNumber.Text = tmpNPPID;
            dtNotaDate.Text = tmpNPPDate;
           // Action = tmpAction;
            TransCode = tmpTransCode;
            txtNotes.Text = tmpApprovalNotes;
        }

        private void HeaderNotaPurchaseParked_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
            
            //cmbAction.DisplayMember = "Text";
            //cmbAction.ValueMember = "Value";

            //var items = new[] { 
            //    new { Text = "-select-", Value = "-select-" }, 
            //    new { Text = "Retur Debit", Value = "01" }, 
            //    new { Text = "Retur Tukar Barang", Value = "02" },
            //    new { Text = "New Purchase", Value = "03" }
            //};

            //cmbAction.DataSource = items;
            //cmbAction.DropDownStyle = ComboBoxStyle.DropDownList;

            //if (Action == "")
            //{
            //    cmbAction.SelectedIndex = 0;
            //}
            //else {
            //    cmbAction.SelectedValue = Action;
            //}

            ModeBeforeEdit();

           // Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked f = new Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked();
            //f.RefreshGrid();
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            string NotaNumber = "";           
            NotaNumber = GetNotaNumber(GoodsReceivedNumber);
           //tia edit
            txtVendorID.Enabled = true;
            txtGoodsReceivedNumber.Enabled = true;
            txtGoodsReceivedNumber.ReadOnly = true;
            txtVendorID.ReadOnly = true;
            txtVendorName.Enabled = true;
            txtVendorName.ReadOnly = true;
            txtVendorID.ContextMenu = vendid;
            txtVendorName.ContextMenu = vendid;
            txtGoodsReceivedNumber.ContextMenu = vendid;

            //tia end          
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
               // ControlMgr.GroupName = "PurchaseManager";
                 //ControlMgr.GroupName = "PurchaseAdmin";
                if (ControlMgr.GroupName == "Purchase Manager")
                {
                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = true;
                    btnCancel.Visible = false;

                    btnNew.Enabled = false;
                    btnDelete.Enabled = false;
                    btnDownload.Enabled = true;
                    btnUpload.Enabled = false;
                    btnDeleteFile.Enabled = false;
                   // cmbAction.Enabled = false;

                    dgvParkedItemDetails.ReadOnly = true;
                    dgvParkedItemDetails.DefaultCellStyle.BackColor = Color.LightGray;
                    dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;

                    dgvHeaderAttachment();
                    SetValueAttachment();

                    if (TransCode == "01")
                    {
                        txtNotes.Enabled = true;
                        btnApprove.Visible = true;
                        ////btnreject.Visible = true;
                        btnRevision.Visible = true;

                        btnApprove.Enabled = true;
                        //btnreject.Enabled = false;
                        btnRevision.Enabled = true;
                    }
                    //else if (TransCode == "04")
                    //{
                    //    txtNotes.Enabled = false;
                    //    btnApprove.Enabled = false;
                    //    //btnreject.Enabled = false;
                    //    btnRevision.Enabled = false;
                    //    btnDownload.Enabled = false;
                    //}
                    else if(TransCode == "03")
                    {
                        txtNotes.Enabled = false;
                        btnApprove.Enabled = false;
                        //btnreject.Enabled = false;
                        btnRevision.Enabled = false;
                        btnEdit.Enabled = false;
                    }
                    else
                    {
                        txtNotes.Enabled = false;
                        btnApprove.Enabled = false;
                        //btnreject.Enabled = false;
                        btnRevision.Enabled = false;
                    }
                    
                }
                else if (ControlMgr.GroupName == "Purchase Admin")
                {
                    if (TransCode != "03")
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
                        //  cmbAction.Enabled = false;

                        dgvParkedItemDetails.ReadOnly = true;
                        dgvParkedItemDetails.DefaultCellStyle.BackColor = Color.LightGray;
                        dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;

                        dgvHeaderAttachment();
                        SetValueAttachment();

                        txtNotes.Enabled = false;
                        btnApprove.Visible = false;
                        //btnreject.Visible = false;
                        btnRevision.Visible = false;

                        //if (TransCode == "04")
                        //{
                        //    btnEdit.Enabled = false;
                        //}
                    }
                    else {
                        btnSave.Visible = false;
                        btnExit.Visible = true;
                        btnEdit.Visible = false;
                        btnCancel.Visible = false;

                        btnNew.Enabled = false;
                        btnDelete.Enabled = false;
                        btnDownload.Enabled = false;
                        btnUpload.Enabled = false;
                        btnDeleteFile.Enabled = false;
                        //  cmbAction.Enabled = false;

                        dgvParkedItemDetails.ReadOnly = true;
                        dgvParkedItemDetails.DefaultCellStyle.BackColor = Color.LightGray;
                        dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;

                        dgvHeaderAttachment();
                        SetValueAttachment();

                        txtNotes.Enabled = false;
                        btnApprove.Visible = false;
                        //btnreject.Visible = false;
                        btnRevision.Visible = false;

                        //if (TransCode == "04")
                        //{
                        //    btnEdit.Enabled = false;
                        //}
                    }
                   
                }
                else { 
                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;

                    btnNew.Enabled = false;
                    btnDelete.Enabled = false;
                    btnDownload.Enabled = true;
                    btnUpload.Enabled = false;
                    btnDeleteFile.Enabled = false;
                    //cmbAction.Enabled = false;

                    dgvParkedItemDetails.ReadOnly = true;
                    dgvParkedItemDetails.DefaultCellStyle.BackColor = Color.LightGray;
                    dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;

                    dgvHeaderAttachment();
                    SetValueAttachment();

                    txtNotes.Enabled = false;
                    btnApprove.Enabled = false;
                    //btnreject.Enabled = false;
                    btnRevision.Enabled = false;
                    btnDownload.Enabled = false;                   
                }
                
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
                dgvParkedItemDetails.ColumnCount = 14;
                dgvParkedItemDetails.Columns[0].Name = "No";
                dgvParkedItemDetails.Columns[1].Name = "FullItemID";
                dgvParkedItemDetails.Columns[2].Name = "ItemName";
                dgvParkedItemDetails.Columns[3].Name = "Qty";
                dgvParkedItemDetails.Columns[4].Name = "Unit";
                dgvParkedItemDetails.Columns[5].Name = "ActionCode";
                dgvParkedItemDetails.Columns[6].Name = "Price";
                dgvParkedItemDetails.Columns[7].Name = "Notes";
                dgvParkedItemDetails.Columns[8].Name = "SeqNo";
                dgvParkedItemDetails.Columns[9].Name = "Ratio";
                dgvParkedItemDetails.Columns[10].Name = "ReceiptOrderID";
                dgvParkedItemDetails.Columns[11].Name = "PurchID";
                dgvParkedItemDetails.Columns[12].Name = "ReceiptOrder_SeqNo";
                dgvParkedItemDetails.Columns[13].Name = "GoodsReceived_SeqNo";
            }

            dgvParkedItemDetails.Columns["No"].ReadOnly = true;
            dgvParkedItemDetails.Columns["FullItemID"].ReadOnly = true;
            dgvParkedItemDetails.Columns["ItemName"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Qty"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Unit"].ReadOnly = true;
            dgvParkedItemDetails.Columns["ReceiptOrderID"].ReadOnly = true;
            dgvParkedItemDetails.Columns["PurchID"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Price"].ReadOnly = true;

            dgvParkedItemDetails.Columns["SeqNo"].Visible = false;
            dgvParkedItemDetails.Columns["Ratio"].Visible = false;
            dgvParkedItemDetails.Columns["ReceiptOrder_SeqNo"].Visible = false;
            dgvParkedItemDetails.Columns["GoodsReceived_SeqNo"].Visible = false;
           
            dgvParkedItemDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["ActionCode"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["SeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["ReceiptOrderID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["PurchID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["ReceiptOrder_SeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["GoodsReceived_SeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvParkedItemDetails.AutoResizeColumns();

            //if (txtNotaNumber.Text == "")
            //{
            //Query = "SELECT No, FullItemId, ItemName, Qty_Actual AS Qty, Unit, '0.0000' AS Price, '' AS Notes, GoodsReceivedSeqNo AS SeqNo, Ratio ";
            //Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY GoodsReceivedId desc) No, FullItemId, ItemName, Qty_Actual, Unit, GoodsReceivedSeqNo, Ratio  FROM GoodsReceivedD WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND ActionCodeStatus = '02') a ORDER BY a.GoodsReceivedSeqNo ASC ";
            //}
            //else
            //{
                Query = "SELECT No, FullItemID, ItemName, Qty, Unit, ActionCode, Price, Notes, SeqNo, ";
                Query += "(SELECT Ratio FROM GoodsReceivedD ";
                Query += "WHERE GoodsReceivedId = (SELECT GoodsReceivedId FROM NotaPurchaseParkH WHERE NPPID = '" + txtNotaNumber.Text + "' AND GoodsReceivedId = a.GoodsReceivedID) AND GoodsReceivedSeqNo = a.GoodsReceived_SeqNo) AS Ratio, ReceiptOrderId, PurchaseOrderID, ReceiptOrder_SeqNo, GoodsReceived_SeqNo ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY NPPID DESC) No, FullItemID, ItemName, Qty, Unit, ActionCode, Price, Notes, SeqNo, GoodsReceived_SeqNo, GoodsReceivedID, ReceiptOrderId, PurchaseOrderID, ReceiptOrder_SeqNo FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text + "' AND GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "') a ORDER BY a.SeqNo ASC";
           // }
          
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                this.dgvParkedItemDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Dr[12], Dr[13]);

                string ActionName = "";
                if (Dr[5].ToString() == "02")
                {
                    ActionName = "Retur Debit";
                }
                else if (Dr[5].ToString() == "01")
                {
                    ActionName = "Retur Tukar Barang";
                }
                else if (Dr[5].ToString() == "04")
                {
                    ActionName = "New Purchase";
                }
                else if (Dr[5].ToString() == "03")
                {
                    ActionName = "Retur Kembali Barang";
                }
                else if (Dr[5].ToString() == "05")
                {
                    ActionName = "Barang Bonus";
                }
                else if (Dr[5].ToString() == "06")
                {
                    ActionName = "Received With PO";
                }


                Query = "SELECT COUNT(RO.ReceiptOrderId) FROM NotaPurchaseParkD NP ";
                Query += "INNER JOIN GoodsReceivedD GR ON NP.GoodsReceivedID = GR.GoodsReceivedId ";
                Query += "INNER JOIN ReceiptOrderD RO ON GR.RefTransID = RO.ReceiptOrderId ";
                Query += "AND NP.NPPID = '" + txtNotaNumber.Text + "' ";
                Query += "AND NP.SeqNo = " + Convert.ToString(Dr["SeqNo"])  + " ";
                Query += "AND NP.GoodsReceived_SeqNo = GR.GoodsReceivedSeqNo ";
                Query += "AND GR.RefTransSeqNo = RO.SeqNo ";
                Cmd = new SqlCommand(Query, Conn);
                int CountData = Convert.ToInt32(Cmd.ExecuteScalar());
                if (CountData == 0)
                {
                    Query = "SELECT StatusCode AS ActionCode, Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaPurchaseParkD' AND StatusCode IN ('03', '04', '05', '06')";
  }
                else
                {
                    Query = "SELECT StatusCode AS ActionCode, Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaPurchaseParkD' AND StatusCode IN ('01', '02', '04', '05', '06')";
                }   

                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader DrCmb;
                DrCmb = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                while (DrCmb.Read())
                {
                    combo.Items.Add(DrCmb[1].ToString());
                }
                if (Dr[5] != null)
                {
                    combo.Value = ActionName;
                }
                dgvParkedItemDetails.Rows[i].Cells[5] = combo;

                DrCmb.Close();
                i++;
            }
            Dr.Close();          

                     
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

        private void HeaderNotaPurchaseParked_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 35);
        }

        private void HeaderNotaPurchaseParked_FormClosed(object sender, FormClosedEventArgs e)
        {
            //for (int i = 0; i < ListDetailMoUCustomer.Count(); i++)
            //{
            //    ListDetailMoUCustomer[i].Close();
            //}
           // Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked f = new Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked();
          //  f.RefreshGrid();
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
                e.Value = d.ToString("N2");
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
            if (dgvParkedItemDetails.CurrentCell.ColumnIndex == 5)
            {
                SelectedCell = dgvParkedItemDetails.CurrentCell.RowIndex;
                // Check box column
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            }

            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvParkedItemDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvParkedItemDetails_KeyPress);
            }

            if (dgvParkedItemDetails.CurrentCell.ColumnIndex == 10)
            {
                SelectedCell = dgvParkedItemDetails.CurrentCell.RowIndex;

                string ItemName = dgvParkedItemDetails.Rows[SelectedCell].Cells["ItemName"].Value.ToString();

                ConnectionString.Kodes = null;
                SearchQueryV1 tmpSearch = new SearchQueryV1();
                tmpSearch.PrimaryKey = "ReceiptOrderId";
                tmpSearch.Order = "ReceiptOrderId ASC";
                //tmpSearch.Table = "[dbo].[CustTable]";
                tmpSearch.QuerySearch = "SELECT RO.ReceiptOrderId, RO.PurchaseOrderId, RO.ItemName, RO.RemainingQty, RO.SeqNo FROM ReceiptOrderD RO ";
                tmpSearch.QuerySearch += "INNER JOIN PurchH PO ON PO.PurchID = RO.PurchaseOrderId ";
                tmpSearch.QuerySearch += "INNER JOIN ReceiptOrderH ROH ON ROH.ReceiptOrderId = RO.ReceiptOrderId ";
                tmpSearch.QuerySearch += "AND PO.StClose = 0 AND RO.RemainingQty > 0 AND UPPER(RO.ItemName) = UPPER('" + ItemName + "') ";
                tmpSearch.QuerySearch += "AND UPPER(ROH.VendId) = UPPER('" + txtVendorID.Text + "')";
                tmpSearch.FilterText = new string[] { "ReceiptOrderId", "PurchaseOrderId", "ItemName" };
                tmpSearch.Select = new string[] { "ReceiptOrderId", "PurchaseOrderId", "ItemName", "RemainingQty", "SeqNo"};
                tmpSearch.Hide = new string[] { "SeqNo" };
                tmpSearch.ShowDialog();
                if (ConnectionString.Kodes != null)
                {
                    if (Convert.ToDecimal(ConnectionString.Kodes[3]) >= Convert.ToDecimal(dgvParkedItemDetails.Rows[SelectedCell].Cells["Qty"].Value))
                    {
                        dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrderID"].Value = ConnectionString.Kodes[0];
                        dgvParkedItemDetails.Rows[SelectedCell].Cells["PurchID"].Value = ConnectionString.Kodes[1];
                        dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrder_SeqNo"].Value = ConnectionString.Kodes[4];

                        txtVendorName.Focus();
                    }
                    else
                    {
                        MessageBox.Show("RemainingQty (" + Convert.ToDecimal(ConnectionString.Kodes[3]) + ") harus lebih besar atau sama dengan Qty (" + Convert.ToDecimal(dgvParkedItemDetails.Rows[SelectedCell].Cells["Qty"].Value).ToString("F2") + ")");
                        dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrderID"].Value = "";
                        dgvParkedItemDetails.Rows[SelectedCell].Cells["PurchID"].Value = "";
                        dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrder_SeqNo"].Value = "";

                        txtVendorName.Focus();
                        return;
                    }
                }
                else
                {
                    txtVendorName.Focus();                
                }
            }
            if (dgvParkedItemDetails.CurrentCell.ColumnIndex == 3)
            {
                SelectedCell = dgvParkedItemDetails.CurrentCell.RowIndex;
                dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrderID"].Value = "";
                dgvParkedItemDetails.Rows[SelectedCell].Cells["PurchID"].Value = "";
                dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrder_SeqNo"].Value = "";
            }
        }

        void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = ((ComboBox)sender).SelectedIndex;
            SelectedCell = dgvParkedItemDetails.CurrentCell.RowIndex;
         
            //if ((selectedIndex == 2 && ((ComboBox)sender).SelectedItem.ToString() == "New Purchase") || (selectedIndex == 0 && ((ComboBox)sender).SelectedItem.ToString() == "New Purchase"))
            if ((selectedIndex == 0 && ((ComboBox)sender).SelectedItem.ToString() == "New Purchase"))
            {
                dgvParkedItemDetails.Rows[SelectedCell].Cells["Price"].ReadOnly = false;
            }
            else
            {
                dgvParkedItemDetails.Rows[SelectedCell].Cells["Price"].ReadOnly = true;
              //  int Seq = Convert.ToInt32(dgvParkedItemDetails.Rows[SelectedCell].Cells["SeqNo"].Value);
              //  decimal Price = GetPrice(Seq);
                //string ActionCode = GetActionCode(Seq);
               // if (ActionCode == "04")
               // {
                    //dgvParkedItemDetails.Rows[SelectedCell].Cells["Price"].Value = "0.0000";
                //}
               // else
               // {
                 //   dgvParkedItemDetails.Rows[SelectedCell].Cells["Price"].Value = Price;
              //  }                
            }

            if ((selectedIndex == 3 && ((ComboBox)sender).SelectedItem.ToString() == "Received With PO") || (selectedIndex == 4 && ((ComboBox)sender).SelectedItem.ToString() == "Received With PO"))
            {
                dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrderID"].ReadOnly = false;
            }
            else
            {
                dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrderID"].ReadOnly = true;
                dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrderID"].Value = "";
                dgvParkedItemDetails.Rows[SelectedCell].Cells["PurchID"].Value = "";
                dgvParkedItemDetails.Rows[SelectedCell].Cells["ReceiptOrder_SeqNo"].Value = "";
            }
        }

        private string GetActionCode(int Seq)
        {
            Conn = ConnectionString.GetConnection();
            string Result = "";
            Query = "SELECT ActionCode FROM NotaPurchaseParkD WHERE SeqNo = " + Seq + " AND NPPID = '" + txtNotaNumber.Text.Trim() + "'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr;
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                Result = Convert.ToString(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
            return Result;
        }

        private decimal GetPrice(int Seq)
        {
            Conn = ConnectionString.GetConnection();
            decimal Result = 0;
            Query = "SELECT Price FROM NotaPurchaseParkD WHERE SeqNo = " + Seq + " AND NPPID = '" + txtNotaNumber.Text.Trim() + "'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr;
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                Result = Convert.ToDecimal(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
            return Result;
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
            else
            {
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
            //DetailParkedItem DetailParkedItem = new DetailParkedItem();

            //List<DetailParkedItem> ListDetailParkedItem = new List<DetailParkedItem>();
            //DetailParkedItem.ParamHeader(txtGoodsReceivedNumber.Text, dgvParkedItemDetails, EditFrom, txtNotaNumber.Text);
            //DetailParkedItem.ParentRefreshGrid(this);
            //DetailParkedItem.ShowDialog();
            //EditColor();

            LookupNotaPurchaseParked LN = new LookupNotaPurchaseParked();
            LN.ParentRefreshGrid(this);
            LN.ParamHeader(txtNotaNumber.Text, txtGoodsReceivedNumber.Text);
            LN.ShowDialog();
        }

        public void AddDataGridNotaPurchaseParked(List<string> FullItemID, List<string> ItemName, List<decimal> Qty, List<string> Unit, List<decimal> Price, List<string> SeqNo, List<decimal> Ratio, List<string> GoodsReceived_SeqNo)
        {
            Conn.Close();
            Conn = ConnectionString.GetConnection();

            dgvParkedItemDetails.Columns["Price"].ReadOnly = true;

            for (int i = 0; i < FullItemID.Count; i++)
            {
                int No = dgvParkedItemDetails.RowCount + 1;
                this.dgvParkedItemDetails.Rows.Add(No, FullItemID[i], ItemName[i], Qty[i], Unit[i], null, Price[i], null, SeqNo[i], Ratio[i], null, null, null, GoodsReceived_SeqNo[i]);
               
                Query = "SELECT COUNT(RO.ReceiptOrderId) FROM NotaPurchaseParkD NP ";
                Query += "INNER JOIN GoodsReceivedD GR ON NP.GoodsReceivedID = GR.GoodsReceivedId ";
                Query += "INNER JOIN ReceiptOrderD RO ON GR.RefTransID = RO.ReceiptOrderId ";
                Query += "AND NP.NPPID = '" + txtNotaNumber.Text + "' ";
                Query += "AND NP.SeqNo = " + Convert.ToString(SeqNo[i]) + " ";
                Query += "AND NP.GoodsReceived_SeqNo = GR.GoodsReceivedSeqNo ";
                Query += "AND GR.RefTransSeqNo = RO.SeqNo ";
                Cmd = new SqlCommand(Query, Conn);
                int CountData = Convert.ToInt32(Cmd.ExecuteScalar());
                if (CountData == 0)
                {
                    Query = "SELECT StatusCode AS ActionCode, Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaPurchaseParkD' AND StatusCode IN ('03', '04', '05', '06')";
                }
                else
                {
                    Query = "SELECT StatusCode AS ActionCode, Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaPurchaseParkD' AND StatusCode IN ('01', '02', '04', '05', '06')";
                }

                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader DrCmb;
                DrCmb = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                while (DrCmb.Read())
                {
                    combo.Items.Add(DrCmb[1].ToString());
                }

                dgvParkedItemDetails.Rows[No - 1].Cells[5] = combo;

                DrCmb.Close();
            }
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

        public void AddDataGridDetail(List<string> FullItemId, List<string> ItemName, List<string> Qty, List<string> Unit, List<string> SeqNo, List<string> Ratio)
        {
            for (int i = 0; i < FullItemId.Count; i++)
            {
                this.dgvParkedItemDetails.Rows.Add((dgvParkedItemDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], Qty[i], Unit[i], "0.0000", "", SeqNo[i], Ratio[i]);
               // int j = dgvParkedItemDetails.RowCount - 1;
                
               // Conn = ConnectionString.GetConnection();

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
            try
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
                Dictionary<string, decimal> DataGrid = new Dictionary<string, decimal>();
                DataGrid.Clear();

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
                    //else if (Convert.ToDecimal((dgvParkedItemDetails.Rows[i].Cells["Price"].Value == "" ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Price"].Value.ToString())) <= 0)
                    //{
                    //    if (dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value.ToString().ToUpper() == "NEW PURCHASE" && Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value.ToString()) <= 1)
                    //    {
                    //        MessageBox.Show("Item No = " + dgvParkedItemDetails.Rows[i].Cells["No"].Value + ", Price tidak boleh lebih kecil atau sama dengan 1");
                    //        return;
                    //    }                        
                    //}
                     else if (dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value.ToString().ToUpper() == "NEW PURCHASE" && Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value.ToString()) <= 1)
                     {
                         MessageBox.Show("Item No = " + dgvParkedItemDetails.Rows[i].Cells["No"].Value + ", Price tidak boleh lebih kecil atau sama dengan 1");
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
                    else if ((dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value.ToString()) == "")
                    {

                        MessageBox.Show("Item No = " + dgvParkedItemDetails.Rows[i].Cells["No"].Value + ", Action Code tidak boleh kosong.");
                        return;
                    }

                    dgvParkedItemDetails.Rows[i].Cells["Notes"].Value = dgvParkedItemDetails.Rows[i].Cells["Notes"].Value.ToString().Replace("'", "").Trim();

                    if (!DataGrid.ContainsKey(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value)))
                    {
                        DataGrid.Add(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value), Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value));
                    }
                    else
                    {
                        DataGrid[Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value)] += Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    }                    
               }

                txtNotes.Text = txtNotes.Text.Replace("'", "").Trim();
   
                //Rollback RemainingQty RO
                Query = "SELECT GR.RefTransID AS ReceiptOrderId, GR.RefTransSeqNo AS SeqNo, NP.ReceiptOrderId AS NPPReceiptOrderId, NP.ActionCode, NP.Qty FROM NotaPurchaseParkD NP ";
                Query += "INNER JOIN GoodsReceivedD GR ON GR.GoodsReceivedId = NP.GoodsReceivedID AND GR.GoodsReceivedSeqNo = NP.GoodsReceived_SeqNo ";
                Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "' ";
                Query += "AND ActionCode IN ('01', '02', '06')";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);
                    string SeqNo = Convert.ToString(Dr["SeqNo"]);
                    string NPPReceiptOrderId = Convert.ToString(Dr["NPPReceiptOrderId"]);
                    string ActionCode = Convert.ToString(Dr["ActionCode"]);
                    decimal Qty = Convert.ToDecimal(Dr["Qty"]);

                    if (ActionCode == "01" || ActionCode == "02")
                    {
                        Query = "UPDATE ReceiptOrderD SET RemainingQty = RemainingQty + " + Qty + " WHERE ReceiptOrderId = '" + ReceiptOrderId + "' AND SeqNo = " + SeqNo + "";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Query = "UPDATE ReceiptOrderD SET RemainingQty = RemainingQty + " + Qty + " WHERE ReceiptOrderId = '" + NPPReceiptOrderId + "' AND SeqNo = " + SeqNo + "";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                }

                //Check Qty NPP vs DataGrid
                Query = "SELECT SUM(Qty), FullItemID FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text + "' AND GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'  GROUP BY FullItemID";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                int j = 1;
                int k = 1;
                while (Dr.Read())
                {
                    decimal Qty = Convert.ToDecimal(Dr[0]);
                    //string SeqNo = Convert.ToString(Dr[1]);
                    string FullItemID = Convert.ToString(Dr[1]);

                    foreach (var getDataGrid in DataGrid)
                    {
                        if (FullItemID == getDataGrid.Key)
                        {
                            if (Qty != getDataGrid.Value)
                            {
                                MessageBox.Show("Total Qty dengan FullItemID " + FullItemID + " harus sama dengan " + Convert.ToDecimal(Qty));
                                return;
                            }

                            k++;
                        }
                    }

                    if (k == j)
                    {
                        MessageBox.Show("Item dengan FullItemID " + FullItemID + " harus ditambahkan");
                        return;
                    }
                    else
                    {
                        j++;
                    }
                }
                Dr.Close(); 

                //Check Qty NPP vs RO
                Dictionary<string, decimal> DataGridReceived = new Dictionary<string, decimal>();
                DataGridReceived.Clear();

                //Dictionary<string, decimal> DataGridDebit = new Dictionary<string, decimal>();
                //DataGridDebit.Clear();

                //Dictionary<string, decimal> DataGridTukarBarang = new Dictionary<string, decimal>();
                //DataGridTukarBarang.Clear();

                for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
                {     
                    if (dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value.ToString() == "Received With PO")
                    {
                        if (Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ReceiptOrderID"].Value) == "")
                        {
                            MessageBox.Show("Data dengan No " + Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["No"].Value) + ", ReceiptOrderID harus diisi");
                            return;
                        }
                        else
                        {
                            if (!DataGridReceived.ContainsKey(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ReceiptOrderID"].Value + "|" + dgvParkedItemDetails.Rows[i].Cells["ReceiptOrder_SeqNo"].Value)))
                            {
                                DataGridReceived.Add(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ReceiptOrderID"].Value + "|" + dgvParkedItemDetails.Rows[i].Cells["ReceiptOrder_SeqNo"].Value), Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value));
                            }
                            else
                            {
                                DataGridReceived[Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ReceiptOrderID"].Value + "|" + dgvParkedItemDetails.Rows[i].Cells["ReceiptOrder_SeqNo"].Value)] += Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                            }
                        }

                         
                    }
                    //if (dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value.ToString() == "Retur Debit")
                    //{
                    //    if (!DataGridDebit.ContainsKey(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value) + "|" + Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["No"].Value)))
                    //    {
                    //        DataGridDebit.Add(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "|" + Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["No"].Value)), Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value));
                    //    }
                    //    else
                    //    {
                    //        DataGridDebit[Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value) + "|" + Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["No"].Value)] += Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    //    }                  
                    //}

                    //if (dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value.ToString() == "Retur Tukar Barang")
                    //{
                    //    if (!DataGridTukarBarang.ContainsKey(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value) + "|" + Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["No"].Value)))
                    //    {
                    //        DataGridTukarBarang.Add(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value) + "|" + Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["No"].Value), Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value));
                    //    }
                    //    else
                    //    {
                    //        DataGridTukarBarang[Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value) + "|" + Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["No"].Value)] += Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    //    }
                    //}
                }

                //Received
                foreach (var getDataGridReceived in DataGridReceived)
                {
                    string [] Key = getDataGridReceived.Key.Split('|');
                    string ReceiptOrderID = Key[0];
                    string ReceiptOrder_SeqNo = Key[1];
                    decimal Qty = getDataGridReceived.Value;

                    Query = "SELECT RemainingQty FROM ReceiptOrderD WHERE ReceiptOrderId = '" + ReceiptOrderID + "' AND SeqNo = " + ReceiptOrder_SeqNo + "";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    decimal RemainingQtyRO = Convert.ToDecimal(Cmd.ExecuteScalar());

                    if (RemainingQtyRO < Qty)
                    {
                        MessageBox.Show("Total Qty (" + Qty + ") dengan ReceiptOrderID " + ReceiptOrderID + " melebihi Remaining Qty (" + RemainingQtyRO + ") pada ReceiptOrder");
                        return;
                    }
                    else
                    {
                        Query = "UPDATE ReceiptOrderD SET RemainingQty = RemainingQty - " + Qty + " WHERE ReceiptOrderId = '" + ReceiptOrderID + "' AND SeqNo = " + ReceiptOrder_SeqNo + "";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                }


                for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
                {
                    string No = dgvParkedItemDetails.Rows[i].Cells["No"].Value.ToString();
                    string FullItemID = dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value.ToString();
                    string SeqNo = dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value.ToString();
                    string ActionCode = dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value.ToString();
                    decimal Qty =  Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value.ToString());

                    if (ActionCode == "Retur Debit" || ActionCode == "Retur Tukar Barang")
                    {
                        Query = "SELECT RO.RemainingQty FROM NotaPurchaseParkD NP INNER JOIN GoodsReceivedD GR ON GR.GoodsReceivedId = NP.GoodsReceivedID AND GR.GoodsReceivedSeqNo = NP.GoodsReceived_SeqNo ";
                        Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                        Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.SeqNo = " + SeqNo + " AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        decimal RemainingQtyRO = Convert.ToDecimal(Cmd.ExecuteScalar());

                        if (RemainingQtyRO < Qty)
                        {
                            MessageBox.Show("Qty (" + Qty + ") pada No " + No + " dan FullItemID " + FullItemID + " melebihi Remaining Qty (" + RemainingQtyRO + ") pada ReceiptOrder");
                            return;
                        }
                        else
                        {
                            Query = "SELECT RO.SeqNo, RO.ReceiptOrderId FROM NotaPurchaseParkD NP INNER JOIN GoodsReceivedD GR ON GR.GoodsReceivedId = NP.GoodsReceivedID AND GR.GoodsReceivedSeqNo = NP.GoodsReceived_SeqNo ";
                            Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                            Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.SeqNo = " + SeqNo + " AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                string ROSeqNo = Convert.ToString(Dr["SeqNo"]);
                                string ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);

                                Query = "UPDATE ReceiptOrderD SET RemainingQty = RemainingQty - " + Qty + " WHERE ReceiptOrderId = '" + ReceiptOrderId + "' AND SeqNo = " + ROSeqNo + "";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }
                            Dr.Close();
                        }
                    }
                }

                ////Debit
                //foreach (var getDataGridDebit in DataGridDebit)
                //{
                //    string[] Key = getDataGridDebit.Key.Split('|');
                //    string SeqNo = Key[0];
                //    string No = Key[1];
                //    decimal Qty = getDataGridDebit.Value;
                //    Query = "SELECT RO.RemainingQty FROM NotaPurchaseParkD NP INNER JOIN GoodsReceivedD GR ON GR.GoodsReceivedId = NP.GoodsReceivedID AND GR.GoodsReceivedSeqNo = NP.GoodsReceived_SeqNo ";
                //    Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //    Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.SeqNo = " + SeqNo + " AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    decimal RemainingQtyRO = Convert.ToDecimal(Cmd.ExecuteScalar());

                //    //Query = "SELECT NP.FullItemID FROM NotaPurchaseParkD NP ";
                //    //Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.SeqNo = " + SeqNo + " AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //    //Cmd = new SqlCommand(Query, Conn, Trans);
                //    //string FullItemID = Convert.ToString(Cmd.ExecuteScalar());

                //    if (RemainingQtyRO < Qty)
                //    {
                //        MessageBox.Show("Qty (" + Qty + ") pada No " + No + " melebihi Remaining Qty (" + RemainingQtyRO + ") pada ReceiptOrder");
                //        return;
                //    }
                //    else
                //    {
                //        Query = "SELECT RO.SeqNo, RO.ReceiptOrderId FROM NotaPurchaseParkD NP INNER JOIN GoodsReceivedD GR ON GR.GoodsReceivedId = NP.GoodsReceivedID AND GR.GoodsReceivedSeqNo = NP.GoodsReceived_SeqNo ";
                //        Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //        Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.SeqNo = " + SeqNo + " AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            string ROSeqNo = Convert.ToString(Dr["SeqNo"]);
                //            string ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);

                //            Query = "UPDATE ReceiptOrderD SET RemainingQty = RemainingQty - " + Qty + " WHERE ReceiptOrderId = '" + ReceiptOrderId + "' AND SeqNo = " + ROSeqNo + "";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();
                //        }
                //        Dr.Close();                       
                //    }
                //}

                ////Tukar Barang
                //foreach (var getDataGridTukarBarang in DataGridTukarBarang)
                //{
                //    string[] Key = getDataGridTukarBarang.Key.Split('|');
                //    string SeqNo = Key[0];
                //    string No = Key[1];
                //    decimal Qty = getDataGridTukarBarang.Value;
                //    Query = "SELECT RO.RemainingQty FROM NotaPurchaseParkD NP INNER JOIN GoodsReceivedD GR ON GR.GoodsReceivedId = NP.GoodsReceivedID AND GR.GoodsReceivedSeqNo = NP.GoodsReceived_SeqNo ";
                //    Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //    Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.SeqNo = " + SeqNo + " AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    decimal RemainingQtyRO = Convert.ToDecimal(Cmd.ExecuteScalar());

                //    //Query = "SELECT NP.FullItemID FROM NotaPurchaseParkD NP ";
                //    //Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.SeqNo = " + SeqNo + " AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //    //Cmd = new SqlCommand(Query, Conn, Trans);
                //    //string FullItemID = Convert.ToString(Cmd.ExecuteScalar());

                //    if (RemainingQtyRO < Qty)
                //    {
                //        MessageBox.Show("Qty (" + Qty + ") pada No " + No + " melebihi Remaining Qty (" + RemainingQtyRO + ") pada ReceiptOrder");
                //        return;
                //    }
                //    else
                //    {
                //        Query = "SELECT RO.SeqNo, RO.ReceiptOrderId FROM NotaPurchaseParkD NP INNER JOIN GoodsReceivedD GR ON GR.GoodsReceivedId = NP.GoodsReceivedID AND GR.GoodsReceivedSeqNo = NP.GoodsReceived_SeqNo ";
                //        Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //        Query += "WHERE NP.NPPID = '" + txtNotaNumber.Text + "' AND NP.SeqNo = " + SeqNo + " AND NP.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            string ROSeqNo = Convert.ToString(Dr["SeqNo"]);
                //            string ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);

                //            Query = "UPDATE ReceiptOrderD SET RemainingQty = RemainingQty - " + Qty + " WHERE ReceiptOrderId = '" + ReceiptOrderId + "' AND SeqNo = " + ROSeqNo + "";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();
                //        }
                //        Dr.Close(); 
                //    }
                //}

               
               
               //if (cmbAction.SelectedIndex == 0)
               //{
               //    MessageBox.Show("Action harus diisi");
               //    return;
               //}

               if (dgvAttachment.RowCount == 0)
               {
                   MessageBox.Show("File attachment harus diupload");
                   return;
               }
            }

           
                //string ActionCodeStatus = cmbAction.SelectedValue.ToString();

                //if (ActionCodeStatus == "01")
                //{
                //    ActionCodeStatus = "07";
                //}
                //else if (ActionCodeStatus == "02")
                //{
                //    ActionCodeStatus = "08";
                //}
                //else if (ActionCodeStatus == "03")
                //{
                //    ActionCodeStatus = "06";
                //} 

                //if (Mode == "New" || txtNotaNumber.Text == "")
                //{
             
                //    Query = "Insert into NotaPurchaseParkH (NPPID, NPPDate, GoodsReceivedID, VendID, ActionCode, TransCode,CreatedDate, CreatedBy) OUTPUT INSERTED.NPPID values ";
                //    Query += "((Select 'NPP/'+FORMAT(getdate(), 'yyMM')+'/'+Right('00000' + CONVERT(NVARCHAR, case when Max(NPPID) is null then '1' else substring(Max(NPPID),11,5)+1 end), 5) ";
                //    Query += "from [NotaPurchaseParkH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', '" + txtGoodsReceivedNumber.Text + "', '" + txtVendorID.Text + "','" + cmbAction.SelectedValue.ToString() + "',";
                //    Query += "'01', getdate(),'" + ControlMgr.UserId + "');";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    string NotaNumber = Cmd.ExecuteScalar().ToString();                    
                   
                                       

                //    Query = "";
                //    for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
                //    {
                //       // int SeqNo = i+1;
                //        Query += "Insert NotaPurchaseParkD (NPPID, SeqNo, FullItemID, ItemName, Qty, Unit, Price, Notes, CreatedDate, CreatedBy) Values ";
                //        Query += "('" + NotaNumber + "',";
                //        Query += (dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value.ToString()) + ",'";
                //        Query += (dgvParkedItemDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["FullItemId"].Value.ToString()) + "','";
                //        Query += (dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                //        Query += (dgvParkedItemDetails.Rows[i].Cells["Qty"].Value == null ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Qty"].Value.ToString()) + "','";
                //        Query += (dgvParkedItemDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["Unit"].Value.ToString()) + "','";
                //        Query += (dgvParkedItemDetails.Rows[i].Cells["Price"].Value == null ? "0.0000" : dgvParkedItemDetails.Rows[i].Cells["Price"].Value.ToString()) + "','";
                //        Query += (dgvParkedItemDetails.Rows[i].Cells["Notes"].Value == null ? "" : dgvParkedItemDetails.Rows[i].Cells["Notes"].Value.ToString()) + "',";
                //        Query += "getdate(),";
                //        Query += "'" + ControlMgr.UserId + "');";
                      
                //        Query += "UPDATE GoodsReceivedD SET ActionCodeStatus = '" + ActionCodeStatus + "' ";
                //        Query += ",UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GoodsReceivedSeqNo = '" + dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "' ;";

                       
                //        if (i % 5 == 0 && i > 0)
                //        {
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();
                //            Query = "";
                //        }
                //    }

                //    if (Query != "")
                //    {
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        Query = "";
                //    }

                    

                //    for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                //    {
                //        Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                //        Query += "( 'NotaPurchaseParkH', '" + NotaNumber + "', '";
                //        Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                //        Query += dgvAttachment.Rows[i].Cells["Extension"].Value.ToString() + "', '";
                //        Query += dgvAttachment.Rows[i].Cells["FileSize"].Value.ToString();
                //        Query += "',@binaryValue";
                //        Query += ");";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, ListAttachment[i].Length).Value = ListAttachment[i];
                //        Cmd.ExecuteNonQuery();
                //    }

                //    Trans.Commit();
                //    MessageBox.Show("Data Nota Number : " + NotaNumber + " berhasil ditambahkan.");
                //    txtNotaNumber.Text = NotaNumber;
                //    MainMenu f = new MainMenu();
                //}
                //else
                //{

                Query = "UPDATE NotaPurchaseParkH SET TransCode = '01', ";              
                Query += "UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "', ApprovalNotes = '" + txtNotes.Text + "' ";
                Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

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

               Query = "DELETE FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text + "'";
               Cmd = new SqlCommand(Query, Conn, Trans);
               Cmd.ExecuteNonQuery();

               int NPPSeqNo = 1;
               for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
               {
                    //string POPrice = "";
                    //decimal Price = 0; 

                    string ActionValue = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
                    string ActionCode = "";
                    //string ActionCodeStatus = "";
                    if (ActionValue == "Retur Debit")
                    {
                        ActionCode = "02";
                        //ActionCodeStatus = "07";

                        //POPrice = GetPOPrice(txtNotaNumber.Text, Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value));

                        //if (POPrice == "")
                        //{
                        //    Price = 1;
                        //}
                        //else
                        //{
                        //    Price = Convert.ToDecimal(POPrice);
                        //}

                        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(Price);
                    }
                    else if (ActionValue == "Retur Tukar Barang")
                    {
                        ActionCode = "01";
                        //ActionCodeStatus = "08";

                        //POPrice = GetPOPrice(txtNotaNumber.Text, Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value));

                        //if (POPrice == "")
                        //{
                        //    Price = 1;
                        //}
                        //else
                        //{
                        //    Price = Convert.ToDecimal(POPrice);
                        //}

                        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(Price);

                    }
                    else if (ActionValue == "New Purchase")
                    {
                        ActionCode = "04";
                        //ActionCodeStatus = "06";
                    }
                    else if (ActionValue == "Retur Kembali Barang")
                    {
                        ActionCode = "03";
                        //ActionCodeStatus = "10";

                        //POPrice = GetPOPrice(txtNotaNumber.Text, Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value));

                        //if (POPrice == "")
                        //{
                        //    Price = 1;
                        //}
                        //else
                        //{
                        //    Price = Convert.ToDecimal(POPrice);
                        //}

                        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(Price);

                    }
                    else if (ActionValue == "Barang Bonus")
                    {
                        ActionCode = "05";
                        //ActionCodeStatus = "11";
                       
                        //Price = 0;

                        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(Price);
                    }
                    else if (ActionValue == "Received With PO")
                    {
                        ActionCode = "06";
                        //ActionCodeStatus = "05";

                        //POPrice = GetPOPrice(txtNotaNumber.Text, Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value));

                        //if (POPrice == "")
                        //{
                        //    Price = 1;
                        //}
                        //else
                        //{
                        //    Price = Convert.ToDecimal(POPrice);
                        //}

                        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(Price);

                    }

                    //Query = "UPDATE NotaPurchaseParkD SET ActionCode = '" + ActionCode + "', ";
                    //Query += "Price = '" + dgvParkedItemDetails.Rows[i].Cells["Price"].Value + "', ";
                    //Query += "Notes = '" + dgvParkedItemDetails.Rows[i].Cells["Notes"].Value + "', ";
                    //Query += "UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' ";
                    //Query += "WHERE NPPID = '" + txtNotaNumber.Text..0Trim() + "' AND SeqNo = '" + dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "'";

   
                    Query = "INSERT INTO NotaPurchaseParkD (NPPID, SeqNo, FullItemID, ItemName, Qty, Unit, ";
                    Query += "Price, Amount, GoodsReceivedID, GoodsReceived_SeqNo, Notes, ";
                    Query += "ActionCode, CreatedDate, CreatedBy, PurchaseOrderId, ReceiptOrderId, ReceiptOrder_SeqNo, UpdatedDate, UpdatedBy) ";
                    Query += "VALUES('" + txtNotaNumber.Text + "', " + NPPSeqNo + ", ";
                    Query += "'" + dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value + "', ";
                    Query += "'" + dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value + "', ";
                    Query += "" + dgvParkedItemDetails.Rows[i].Cells["Qty"].Value + ", ";
                    Query += "'" + dgvParkedItemDetails.Rows[i].Cells["Unit"].Value + "', ";
                    Query += "" + Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value) + ", ";
                    Query += "" + Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value) * Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value) + " , ";
                    Query += "'" + txtGoodsReceivedNumber.Text + "', ";
                    Query += "'" + dgvParkedItemDetails.Rows[i].Cells["GoodsReceived_SeqNo"].Value + "', ";
                    Query += "'" + dgvParkedItemDetails.Rows[i].Cells["Notes"].Value + "', ";
                    Query += "'" + ActionCode + "', ";
                    Query += "GETDATE(), ";
                    Query += "'" + ControlMgr.UserId + "', ";
                    Query += "'" + dgvParkedItemDetails.Rows[i].Cells["PurchID"].Value + "', ";
                    Query += "'" + dgvParkedItemDetails.Rows[i].Cells["ReceiptOrderID"].Value + "', ";
                    Query += "" + dgvParkedItemDetails.Rows[i].Cells["ReceiptOrder_SeqNo"].Value == "" ? "0" : dgvParkedItemDetails.Rows[i].Cells["ReceiptOrder_SeqNo"].Value == null ? "0" : dgvParkedItemDetails.Rows[i].Cells["ReceiptOrder_SeqNo"].Value + "";
                    Query += ", GETDATE(), ";
                    Query += "'" + ControlMgr.UserId + "'"; 
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    NPPSeqNo++;

                    //int GoodReceivedSeqNo = 0;
                    //Query = "SELECT GoodsReceived_SeqNo FROM NotaPurchaseParkD ";
                    //Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND SeqNo = '" + dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "' ";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Dr = Cmd.ExecuteReader();

                    //while (Dr.Read())
                    //{
                    //    GoodReceivedSeqNo = Convert.ToInt32(Dr["GoodsReceived_SeqNo"]);
                    //}
                    //Dr.Close();

                    //Query = "UPDATE GoodsReceivedD SET ActionCodeStatus = '" + ActionCodeStatus + "' ";
                    //Query += ",UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GoodsReceivedSeqNo = '" + GoodReceivedSeqNo + "' ;";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteNonQuery();
                    
                }

               //string GetLastStatus = "";
               //Query = "SELECT TOP 1 LogStatus FROM WorkflowLogTable ";
               //Query += "WHERE ReffID = '"+txtNotaNumber.Text.Trim()+"' ORDER BY LogDate DESC ";
               //Cmd = new SqlCommand(Query, Conn, Trans);
               //Dr = Cmd.ExecuteReader();

               //while (Dr.Read())
               //{
               //    GetLastStatus = Convert.ToString(Dr["LogStatus"]);
               //}
               //Dr.Close();

               //if (GetLastStatus != "01")
               //{
               //    String StatusDesc = null;
               //    Query = "SELECT Deskripsi FROM TransStatusTable ";
               //    Query += "WHERE StatusCode='01' AND TransCode = 'NotaPurchasePark' ";
               //    Cmd = new SqlCommand(Query, Conn, Trans);
               //    Dr = Cmd.ExecuteReader();

               //    while (Dr.Read())
               //    {
               //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
               //    }
               //    Dr.Close();

               //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
               //    Query += "VALUES('NotaPurchaseParkedH', '" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '01', '" + StatusDesc + "', getdate()) ";
               //    Cmd = new SqlCommand(Query, Conn, Trans);
               //    Cmd.ExecuteNonQuery();
               //}

               InsertLog(txtNotaNumber.Text, "NotaPurchasePark", "01", "E", Conn, Trans, Cmd);
              

                TransCode = "01";
                Trans.Commit();
                MessageBox.Show("Data Nota Number : " + txtNotaNumber.Text + " berhasil diupdate.");

                //}

                Parent.RefreshGrid();
                ModeBeforeEdit();
            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();               
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Mode = "Edit";
                btnSave.Visible = true;
                btnExit.Visible = false;
                btnEdit.Visible = false;
                btnCancel.Visible = true;

                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnApprove.Visible = false;
            btnRevision.Visible = false;

            btnNew.Enabled = true;
            btnDelete.Enabled = true;                     
            btnDeleteFile.Enabled = true;
            //cmbAction.Enabled = true;
            btnDownload.Enabled = true;
            btnUpload.Enabled = true;

            dgvParkedItemDetails.ReadOnly = false;
            dgvParkedItemDetails.Columns["No"].ReadOnly = true;
            dgvParkedItemDetails.Columns["FullItemId"].ReadOnly = true;
            dgvParkedItemDetails.Columns["ItemName"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Qty"].ReadOnly = false;
            dgvParkedItemDetails.Columns["Unit"].ReadOnly = true;
            dgvParkedItemDetails.Columns["ReceiptOrderID"].ReadOnly = true;
            dgvParkedItemDetails.Columns["PurchID"].ReadOnly = true;

            dgvParkedItemDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["FullItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["ReceiptOrderID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvParkedItemDetails.Columns["PurchID"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvParkedItemDetails.AutoResizeColumns();
            EditColor();
            dgvParkedItemDetails.DefaultCellStyle.BackColor = Color.White;
            dgvAttachment.DefaultCellStyle.BackColor = Color.White;

            if (dgvParkedItemDetails.RowCount > 0)
            {
                for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
                {
                    string ActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
                    if (ActionCode != "New Purchase")
                    {
                        dgvParkedItemDetails.Rows[i].Cells["Price"].ReadOnly = true;
                    }

                    if (ActionCode == "Received With PO")
                    {
                        dgvParkedItemDetails.Rows[i].Cells["ReceiptOrderID"].ReadOnly = false;                        
                    }
                }               
            }
        }

        //private void SetHeaderNotaPurchaseparked()
        //{
        //    Conn = ConnectionString.GetConnection();

        //    Query = "Select NPPDate, NPPID, VendID, ActionCode FROM NotaPurchaseParkH ";
        //    Query += "Where GoodsReceivedID = '" + GoodsReceivedNumber + "'";

        //    Cmd = new SqlCommand(Query, Conn);
        //    Dr = Cmd.ExecuteReader();

        //    while (Dr.Read())
        //    {
        //        dtNotaDate.Text = Dr["NPPDate"].ToString();
        //        txtNotaNumber.Text = Dr["NPPID"].ToString();
        //        txtVendorID.Text = Dr["VendID"].ToString();
        //        cmbAction.SelectedValue = Dr["ActionCode"].ToString();
        //    }
        //    Dr.Close();

        //    Query = "Select VendName FROM VendTable ";
        //    Query += "Where VendID = '" + txtVendorID.Text + "'";

        //    Cmd = new SqlCommand(Query, Conn);
        //    Dr = Cmd.ExecuteReader();

        //    while (Dr.Read())
        //    {
        //        txtVendorName.Text = Dr["VendName"].ToString();
        //    }
        //    Dr.Close();
        //}

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

        private void btnRevision_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();          

            try
            {
                Query = "UPDATE NotaPurchaseParkH SET ApprovalNotes = '"+txtNotes.Text+"', TransCode = '02', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' ";
                Query += "WHERE NPPID = '" + txtNotaNumber.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                String StatusDesc = null;
                Query = "SELECT Deskripsi FROM TransStatusTable ";
                Query += "WHERE StatusCode='02' AND TransCode = 'NotaPurchasePark' ";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                }
                Dr.Close();

                Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                Query += "VALUES('NotaPurchaseParkedH', '" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '02', '" + StatusDesc + "', getdate()) ";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                TransCode = "02";

                InsertLog(txtNotaNumber.Text, "NotaPurchasePark", TransCode, "", Conn, Trans, Cmd);

                Trans.Commit();
                MessageBox.Show("Data Nota Number : " + txtNotaNumber.Text + " berhasil disimpan");                             
               
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

        private void btnReject_Click(object sender, EventArgs e)
        {
            //Conn = ConnectionString.GetConnection();
            //Trans = Conn.BeginTransaction();

            //try
            //{
            //    Query = "UPDATE GoodsReceivedD SET ActionCodeStatus = '02', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' ";
            //    Query += "WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GoodsReceivedSeqNo IN (SELECT GoodsReceived_SeqNo FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text + "')";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Cmd.ExecuteNonQuery();

            //    Query = "UPDATE NotaPurchaseParkH SET ApprovalNotes = '" + txtNotes.Text + "', TransCode = '04', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' ";
            //    Query += "WHERE NPPID = '" + txtNotaNumber.Text + "'";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Cmd.ExecuteNonQuery();

            //    String StatusDesc = null;
            //    Query = "SELECT Deskripsi FROM TransStatusTable ";
            //    Query += "WHERE StatusCode='04' AND TransCode = 'NotaPurchasePark' ";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Dr = Cmd.ExecuteReader();

            //    while (Dr.Read())
            //    {
            //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
            //    }
            //    Dr.Close();

            //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
            //    Query += "VALUES('NotaPurchaseParkedH', '" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '04', '" + StatusDesc + "', getdate()) ";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Cmd.ExecuteNonQuery();

            //    TransCode = "04";
            //    Trans.Commit();
            //    MessageBox.Show("Data Nota Number : " + txtNotaNumber.Text + " berhasil di reject");

            //}
            //catch (Exception)
            //{
            //    Trans.Rollback();
            //    return;
            //}
            //finally
            //{
            //    Conn.Close();
            //    Parent.RefreshGrid();
            //    ModeBeforeEdit();
            //}
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            try
            {
                //Update Price
                //for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
                //{
                //    string POPrice = "";
                //    decimal DPrice = 0;

                //    string ActionValue = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
                //    string ActionCode = "";
                //    if (ActionValue == "Retur Debit")
                //    {
                //        ActionCode = "02";

                //        //POPrice = GetPOPrice(txtNotaNumber.Text, Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value));

                //        //if (POPrice == "")
                //        //{
                //        //    DPrice = 1;
                //        //}
                //        //else
                //        //{
                //        //    DPrice = Convert.ToDecimal(POPrice);
                //        //}

                //        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(DPrice);
                //    }
                //    else if (ActionValue == "Retur Tukar Barang")
                //    {
                //        ActionCode = "01";

                //        //POPrice = GetPOPrice(txtNotaNumber.Text, Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value));

                //        //if (POPrice == "")
                //        //{
                //        //    DPrice = 1;
                //        //}
                //        //else
                //        //{
                //        //    DPrice = Convert.ToDecimal(POPrice);
                //        //}

                //        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(DPrice);

                //    }
                //    else if (ActionValue == "New Purchase")
                //    {
                //        ActionCode = "04";
                //    }
                //    else if (ActionValue == "Retur Kembali Barang")
                //    {
                //        ActionCode = "03";

                //        //POPrice = GetPOPrice(txtNotaNumber.Text, Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value));

                //        //if (POPrice == "")
                //        //{
                //        //    DPrice = 1;
                //        //}
                //        //else
                //        //{
                //        //    DPrice = Convert.ToDecimal(POPrice);
                //        //}

                //        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(DPrice);

                //    }
                //    else if (ActionValue == "Barang Bonus")
                //    {
                //        ActionCode = "05";

                //        //DPrice = 0;

                //        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(DPrice);
                //    }
                //    else if (ActionValue == "Received With PO")
                //    {
                //        ActionCode = "06";

                //        //POPrice = GetPOPriceReceived(Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ReceiptOrderID"].Value), Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ReceiptOrder_SeqNo"].Value));

                //        //if (POPrice == "")
                //        //{
                //        //    DPrice = 1;
                //        //}
                //        //else
                //        //{
                //        //    DPrice = Convert.ToDecimal(POPrice);
                //        //}

                //        //dgvParkedItemDetails.Rows[i].Cells["Price"].Value = Convert.ToString(DPrice);

                //    }

                //    Query = "UPDATE NotaPurchaseParkD SET ActionCode = '" + ActionCode + "', ";
                //    Query += "Price = '" + dgvParkedItemDetails.Rows[i].Cells["Price"].Value + "', ";
                //    Query += "Notes = '" + dgvParkedItemDetails.Rows[i].Cells["Notes"].Value + "', ";
                //    Query += "UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' ";
                //    Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND SeqNo = '" + dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value + "'";

                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();

                //}
                //End Update Price

                string StatusDesc = "";
                
                //Retur Tukar Barang
                ReturTukarBarang(Conn, Trans, Cmd);
                //End Retur Tukar Barang

                //Retur Debit Nota
 
                //End Retur Debit Nota
                ReturDebitNota(Conn, Trans, Cmd);
                //New Purchase

                //End New Purchase
                NewPurchase(Conn, Trans, Cmd);
                //Retur Kembali Barang

                //End Retur Kembali Barang
                ReturKembaliBarang(Conn, Trans, Cmd);
                //Barang Bonus
                BarangBonus(Conn, Trans, Cmd);
                //End Barang Bonus

                //Received With PO
                ReceivedWithPO(Conn, Trans, Cmd);
                //End Received With PO

                InsertLog(txtNotaNumber.Text, "NotaPurchasePark", "03", "", Conn, Trans, Cmd);

                //int SeqNoGroup = 0, GoodReceivedSeqNo = 0, CountData = 0, SeqNo = 0, WorkflowLogDetail = 0, SeqNoResize = 0, ResizeLoop = 1, CountResize = 0;
                //string ToFullItemId = "", ToGroupID = "", ToSubGroup1ID = "", ToSubGroup2ID = "", ToItemID = "", ToItemName = "";
                //string GroupID = "", SubGroup1ID = "", SubGroup2ID = "", ItemID = "", DeliveryMethod = "";
                //string UoM = "", QueryTemp = "", StatusDesc = "", TermofPayment = "", PaymentMode = "";
                //string InventSiteID = "", InventSiteName = "", InventSiteLocation = "", VehicleType = "";
                //string VehicleNumber = "", DriverName = "", GoodsReceivedDate = "", SJNumber = "", SJDate = "";
                //string Resize = "", ResizeType = "", InventSiteId = "", InventSiteBlokID = "", Quality = "", ActionStatusCodeDetail = "05";
                //string FullItemID = "", ItemName = "", Unit = "", InventSiteIDDetail, InventSiteBlokIDDetail, TransId = "";
                //decimal Ratio = 0, Qty_KG = 0, ConvRatio = 0, QtyUoM = 0, QtyAlt = 0, PPN = 0, PPH = 0;
                //decimal Total = 0, Total_PPN = 0, Total_PPH = 0, Price_KG = 0, SubTotal = 0, SubTotalPPN = 0;
                //decimal SubTotalPPH = 0, Total_Nett = 0, ExchRate = 0, TotalQty = 0, TotalRatio = 0, Qty = 0, Price = 0;
                //decimal TimbanganWeight = 0;
                //string[] SplitFullItemID = null;

                ////NEW PURCHASE======================================================================================
                ////Get Count Data New Purchase
                //Query ="SELECT COUNT(FullItemID) CountData FROM NotaPurchaseParkD ";
                //Query += "WHERE NPPID = '"+txtNotaNumber.Text.Trim()+"' AND ActionCode = '04' AND Qty != 0 "; //Edited By Thaddaeus 21JUNE2018
                //Cmd = new SqlCommand(Query, Conn, Trans);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    CountData = Convert.ToInt32(Dr["CountData"]);
                //}
                //Dr.Close();

                //if (CountData > 0)
                //{
                //    //GET DATA PPN, PPH, TERM OF PAYMENT, PAYMENT METHOD, EXCHRATE, INVENT SITE ID==================================
                //    Query = "SELECT c.ExchRate,c.TermofPayment, ";
                //    Query += "c.PaymentMode, c.PPN, c.PPH, a.SiteID, a.SiteName, ";
                //    Query += "a.SiteLocation, a.VehicleType, a.VehicleNumber, a.DriverName, ";
                //    Query += "a.GoodsReceivedDate, a.SJNumber, a.SJDate FROM GoodsReceivedH a ";
                //    Query += "INNER JOIN ReceiptOrderH b ";
                //    Query += "ON b.ReceiptOrderId = a.RefTransID ";
                //    Query += "INNER JOIN PurchH c ";
                //    Query += "ON c.PurchID = b.PurchaseOrderId ";
                //    Query += "WHERE a.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "'";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        TermofPayment = Convert.ToString(Dr["TermofPayment"]);
                //        PaymentMode = Convert.ToString(Dr["PaymentMode"]);
                //        PPN = Convert.ToDecimal(Dr["PPN"])/100;
                //        PPH = Convert.ToDecimal(Dr["PPH"])/100;
                //        ExchRate = Convert.ToDecimal(Dr["ExchRate"]);
                //        InventSiteID = Convert.ToString(Dr["SiteID"]);
                //        InventSiteName = Convert.ToString(Dr["SiteName"]);
                //        InventSiteLocation = Convert.ToString(Dr["SiteLocation"]);
                //        VehicleType = Convert.ToString(Dr["VehicleType"]);
                //        VehicleNumber = Convert.ToString(Dr["VehicleNumber"]);
                //        DriverName = Convert.ToString(Dr["DriverName"]);
                //        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                //        SJNumber = Convert.ToString(Dr["SJNumber"]);
                //        SJDate = Convert.ToString(Dr["SJDate"]);
                //    }
                //    Dr.Close();
                //    //END GET DATA PPN, PPH, TERM OF PAYMENT, PAYMENT METHOD, EXCHRATE, INVENT SITE ID==================


                //    //GET DATA TOTAL, TOTAL PPN, TOTAL PPH, TOTAL NET===================================================
                //    Query = "SELECT SUM(SubTotal) AS Total FROM ( ";
                //    Query += "SELECT (Price * Qty) AS SubTotal FROM NotaPurchaseParkD WHERE NPPID = '"+txtNotaNumber.Text.Trim()+"' AND ActionCode = '04') a ";

                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        Total = Convert.ToDecimal(Dr["Total"]);
                //    }                  
                //    Dr.Close();
                //    Total_PPN = Total * PPN;
                //    Total_PPH = Total * PPH;
                //    Total_Nett = Total + Total_PPH + Total_PPN;
                //    //END GET DATA TOTAL, TOTAL PPN, TOTAL PPH, TOTAL NET==============================================


                //    //GET TOTAL QTY====================================================================================
                //    Query = "SELECT SUM(SubTotalQty) AS TotalQty FROM ( ";
                //    Query += "SELECT (Qty) AS SubTotalQty FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '04') a ";

                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        TotalQty = Convert.ToDecimal(Dr["TotalQty"]);
                //    }
                //    Dr.Close();                    
                //    //END GET TOTAL QTY============================================================================


                //    //GET TOTAL RATIO, TIMBANGAN WEIGHT==============================================================================
                //    Query = "SELECT SUM(Ratio) AS TotalRatio FROM ( ";
                //    Query +="SELECT CASE WHEN Ratio IS NULL THEN 0 ELSE Ratio End AS Ratio FROM GoodsReceivedD ";
                //    Query += "WHERE GoodsReceivedId = '"+txtGoodsReceivedNumber.Text.Trim()+"' AND ActionCodeStatus = '06') a ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        TotalRatio = Dr["TotalRatio"] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr["TotalRatio"]);
                //    }
                //    Dr.Close();
                //    TimbanganWeight = TotalQty * TotalRatio;
                //    //END GET TOTAL RATIO, TIMBANGAN WEIGHT==========================================================================


                //    //CREATE HEADER=================================================================================                    
                //    //PR============================================================================================
                //    string Jenis = "PR", Kode = "PRA";
                //    string PRNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    
                //    Query = "Insert into PurchRequisitionH (PurchReqID,OrderDate,TransType,TransStatus,ApprovedBy,CreatedDate,CreatedBy) values ";
                //    Query += "('" + PRNumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "','FIX',";
                //    Query += "'22','" + ControlMgr.GroupName + "',getdate(),'" + ControlMgr.UserId + "');";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();


                //    //INSERT WORKFLOW LOG=========================================================================
                //    Query = "SELECT Deskripsi FROM TransStatusTable ";
                //    Query += "WHERE StatusCode='22' AND TransCode = 'PR' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //    }
                //    Dr.Close();

                //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //    Query += "VALUES('PurchRequisitionH', '" + PRNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '22', '" + StatusDesc + "', getdate()) ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //END INSERT WORKFLOW LOG=======================================================================
                //    //END PR========================================================================================


                //    //QUOTATION=====================================================================================
                //    Jenis = "PQ";
                //    Kode = "PQA";
                //    string PQNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    
                //    Query = "Insert into PurchQuotationH (PurchQuotID,TransType, ";
                //    Query += "OrderDate,VendorQuotDate,VendID,VendName,TermOfPayment,TransStatus,PaymentModeID,ApprovedBy, ";
                //    Query += "PPN,PPH,Total,Total_PPN,Total_PPH,CreatedDate,CreatedBy, TotalDiscount) ";//Edited by Thaddaeus, 21JUNE2018
                //    Query += "values ";
                //    Query += "('" + PQNumber + "',";
                //    Query += "'FIX',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + txtVendorID.Text + "',";
                //    Query += "'" + txtVendorName.Text + "',";
                //    Query += "'" + TermofPayment + "',";
                //    Query += "'03',";
                //    Query += "'" + PaymentMode + "',";
                //    Query += "'" + ControlMgr.UserId + "',";
                //    Query += "" + PPN * 100 + ",";
                //    Query += "" + PPH * 100 + ",";
                //    Query += "" + Total + ",";
                //    Query += "" + Total_PPN + ",";
                //    Query += "" + Total_PPH + ",";
                //    Query += "getdate(),'" + ControlMgr.UserId + "', 0);"; //Edited by Thaddaeus, 21JUNE2018
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();

                //    //INSERT ATTACHMENTS=======================================================================
                //    for (int j = 0; j <= dgvAttachment.RowCount - 1; j++)
                //    {
                //        Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                //        Query += "( 'PurchQuotationH', '" + PQNumber + "', '";
                //        Query += dgvAttachment.Rows[j].Cells["FileName"].Value.ToString() + "', '";
                //        Query += dgvAttachment.Rows[j].Cells["Extension"].Value.ToString() + "', '";
                //        Query += dgvAttachment.Rows[j].Cells["FileSize"].Value.ToString();
                //        Query += "',@binaryValue";
                //        Query += ");";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, ListAttachment[j].Length).Value = ListAttachment[j];
                //        Cmd.ExecuteNonQuery();
                //    }
                //    //END INSERT ATTACHMENTS======================================================================


                //    //INSERT WORKFLOW LOG=========================================================================
                //    Query = "SELECT Deskripsi FROM TransStatusTable ";
                //    Query += "WHERE StatusCode='03' AND TransCode = 'SalesQuotation' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //    }
                //    Dr.Close();

                //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //    Query += "VALUES('PurchQuotationH', '" + PQNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '03', '" + StatusDesc + "', getdate()) ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //END INSERT WORKFLOW LOG=========================================================================
                //    //END QUOTATION===================================================================================


                //    //CANVAS SHEET====================================================================================
                //    Jenis = "CS";
                //    Kode = "CSA";
                //    string CSNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    
                //    Query = "Insert into [CanvasSheetH] (CanvasId,CanvasDate,PurchReqId,TransType,TransStatus, ";
                //    Query += "CreatedDate,CreatedBy) values ";
                //    Query += "('" + CSNumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + PRNumber + "',";
                //    Query += "'FIX',";
                //    Query += "'02',"; 
                //    Query += "getdate(),'" + ControlMgr.UserId + "');";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();


                //    //INSERT WORKFLOW LOG==============================================================================
                //    Query = "SELECT Deskripsi FROM TransStatusTable ";
                //    Query += "WHERE StatusCode='xx' AND TransCode = 'CS02' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //    }
                //    Dr.Close();
                //    //ENDINSERT WORKFLOW LOG===========================================================================


                //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //    Query += "VALUES('CanvasSheetH', '" + CSNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', 'CS02', '" + StatusDesc + "', getdate()) ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //END CANVAS SHEET==================================================================================


                //    //PO=================================================================================================
                //    Jenis = "PO";
                //    Kode = "POA";
                //    string PONumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    
                //    Query = "Insert into [PurchH] (PurchID, OrderDate, DueDate, TransType, ReffTableName, ReffId, ";
                //    Query += "ReffId2, ExchRate, VendID, Total, Total_Nett, Total_Disk, PPN, Total_PPN, PPH, Total_PPH, ";
                //    Query += "StClose, TransStatus, CreatedDate, CreatedBy, PurchH_Print, PurchH_SendEmail, EmailID, TermofPayment, ";
                //    Query += "PaymentMode) values ";
                //    Query += "('" + PONumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'FIX',";
                //    Query += "'CanvasSheet',";
                //    Query += "'" + CSNumber + "',";
                //    Query += "'" + PQNumber + "',";
                //    Query += "" + ExchRate + ",";
                //    Query += "'" + txtVendorID.Text + "',";
                //    Query += "" + Total + ",";
                //    Query += "" + Total_Nett + ",";
                //    Query += "0,";
                //    Query += "" + PPN * 100 + ",";
                //    Query += "" + Total_PPN + ",";
                //    Query += "" + PPH * 100 + ",";
                //    Query += "" + Total_PPH + ",";
                //    Query += "0,";
                //    Query += "'02',";
                //    Query += "getdate(),'" + ControlMgr.UserId + "', ";
                //    Query += "0,";
                //    Query += "0,";
                //    Query += "'',";
                //    Query += "'" + TermofPayment + "',";
                //    Query += "'" + PaymentMode + "'";
                //    Query += ");";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();


                //    //INSERT WORKFLOW LOG============================================================================
                //    Query = "SELECT Deskripsi FROM TransStatusTable ";
                //    Query += "WHERE StatusCode='02' AND TransCode = 'PO' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //    }
                //    Dr.Close();

                //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //    Query += "VALUES('PurchH', '" + PONumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '02', '" + StatusDesc + "', getdate()) ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //ENDINSERT WORKFLOW LOG============================================================================
                //    //END PO============================================================================================


                //    //RO===============================================================================================
                //    Jenis = "RO";
                //    Kode = "ROA";
                //    string RONumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    
                //    Query = "Insert into [ReceiptOrderH] ( ";
                //    Query += "ReceiptOrderId, ReceiptOrderDate, ReceiptOrderStatus, VendId, VendorName, InventSiteID, ";
                //    Query += "InventSiteName, InventSiteLocation, VehicleType, VehicleNumber, DriverName, DeliveryDate, ";
                //    Query += "PurchaseOrderId, PurchaseOrderDate, CreatedDate, CreatedBy, PaymentMode) VALUES( ";
                //    Query += "'" + RONumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'03',";
                //    Query += "'" + txtVendorID.Text + "',";
                //    Query += "'" + txtVendorName.Text + "',";
                //    Query += "'" + InventSiteID + "',";
                //    Query += "'" + InventSiteName + "',";
                //    Query += "'" + InventSiteLocation + "',";
                //    Query += "'" + VehicleType + "',";
                //    Query += "'" + VehicleNumber + "',";
                //    Query += "'" + DriverName + "',";
                //    Query += "'" + GoodsReceivedDate + "',";
                //    Query += "'" + PONumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "getdate(),";
                //    Query += "'" + ControlMgr.UserId + "',";
                //    Query += "'" + PaymentMode + "'";
                //    Query += ") ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //END RO===========================================================================================


                //    //INSERT WORKFLOW LOG===============================================================================
                //    Query = "SELECT Deskripsi FROM TransStatusTable ";
                //    Query += "WHERE StatusCode='03' AND TransCode = 'RO' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //    }
                //    Dr.Close();

                //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //    Query += "VALUES('ReceiptOrderH', '" + RONumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '03', '" + StatusDesc + "', getdate()) ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //ENDINSERT WORKFLOW LOG============================================================================ 
                   

                //    //GR================================================================================================
                //    Jenis = "GR";
                //    Kode = "BBMA";
                //    string NewGRNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                //    GRARefNumber = NewGRNumber;
                //    Query = "Insert into [GoodsReceivedH] ( ";
                //    Query += "GoodsReceivedId, GoodsReceivedDate, GoodsReceivedStatus, RefTransID, RefTransDate, ";
                //    Query += "SJDate, SJNumber, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, ";
                //    Query += "Timbang1Weight, Timbang2Date, Timbang2Weight, SiteID, SiteName, SiteLocation, CreatedDate, ";
                //    Query += "CreatedBy, StatusWeight1, StatusWeight2, RefTransType, NPPId) VALUES( ";
                //    Query += "'" + NewGRNumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'03',";
                //    Query += "'" + RONumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + SJDate + "',";
                //    Query += "'" + SJNumber + "',";
                //    Query += "'" + txtVendorID.Text + "',";
                //    Query += "'" + txtVendorName.Text + "',";
                //    Query += "'" + VehicleType + "',";
                //    Query += "'" + VehicleNumber + "',";
                //    Query += "'" + DriverName + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + TimbanganWeight + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + TimbanganWeight + "',";
                //    Query += "'" + InventSiteID + "',";
                //    Query += "'" + InventSiteName + "',";
                //    Query += "'" + InventSiteLocation + "',";
                //    Query += "getdate(),";
                //    Query += "'" + ControlMgr.UserId + "',";
                //    Query += "'Manual',";
                //    Query += "'Manual',";
                //    Query += "'Receipt Order', ";
                //    Query += "'" + txtNotaNumber.Text +"') ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();

                //    //INSERT WORKFLOW LOG================================================================================                   
                //    Query = "SELECT Deskripsi FROM TransStatusTable ";
                //    Query += "WHERE StatusCode='03' AND TransCode = 'GR' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();

                //    while (Dr.Read())
                //    {
                //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //    }
                //    Dr.Close();

                //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //    Query += "VALUES('GoodsReceivedH', '" + NewGRNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '03', '" + StatusDesc + "', getdate()) ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //ENDINSERT WORKFLOW LOG============================================================================                    
                //    //END GR============================================================================================                   
                //    //END CREATED HEADER================================================================================

                //    //GET DATA DETAIL===================================================================================
                //    Query = "SELECT FullItemID, ItemName, Qty, Unit, Price, SeqNo, GoodsReceived_SeqNo ";
                //    Query += "FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '04' ORDER BY SeqNo ASC ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    DrDetail = Cmd.ExecuteReader();

                //    SeqNoGroup = 1;
                //    SeqNoResize = 1;
                //    while (DrDetail.Read())
                //    {                        
                //        FullItemID = Convert.ToString(DrDetail["FullItemID"]);
                //        ItemName = Convert.ToString(DrDetail["ItemName"]);
                //        Qty = Convert.ToString(DrDetail["Qty"]) == null ? 0 : Convert.ToDecimal(DrDetail["Qty"]);
                //        Unit = Convert.ToString(DrDetail["Unit"]);
                //        Price = Convert.ToString(DrDetail["Price"]) == null ? 0 : Convert.ToDecimal(DrDetail["Price"]);
                //        SeqNo = Convert.ToInt32(DrDetail["SeqNo"]);
                //        GoodReceivedSeqNo = Convert.ToInt32(DrDetail["GoodsReceived_SeqNo"]);

                //        SplitFullItemID = FullItemID.Split('.');
                //        GroupID = Convert.ToString(SplitFullItemID[0]);
                //        SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                //        SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                //        ItemID = Convert.ToString(SplitFullItemID[3]);

                //        //Created By Thaddaeus, 21JUNE2018
                //        if (Qty == 0)
                //        {           
                //            continue;
                //        }
                //        //END==============================

                //        //GET RATIO, DELIVERY METHOD==================================================================
                //        Query = "SELECT CASE WHEN Ratio IS NULL THEN 0 ELSE Ratio End AS Ratio, DeliveryMethod FROM GoodsReceivedD ";
                //        Query += "WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                //        Query += "AND GoodsReceivedSeqNo = '" + GoodReceivedSeqNo + "'; ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            Ratio = Convert.ToInt32(Dr["Ratio"]);
                //            DeliveryMethod = Convert.ToString(Dr["DeliveryMethod"]);
                //        }
                //        Dr.Close();

                //        if (Unit.ToUpper() == "KG")
                //        {
                //            Qty_KG = Qty;
                //            Price_KG = Price;
                //        }
                //        else
                //        {
                //            Qty_KG = Qty / Ratio;
                //            Price_KG = Price / Ratio;
                //        }
                //        //END GET RATIO, DELIVERY METHOD=============================================================


                //        //GET UOM, ALT===============================================================================
                //        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //        UoM = Cmd.ExecuteScalar().ToString();
                //        if (Unit == UoM)
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyUoM = Qty;
                //            QtyAlt = Qty * ConvRatio;
                //        }
                //        else
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyAlt = Qty;
                //            QtyUoM = Qty / ConvRatio;
                //        }
                //        //END GET UOM, ALT==============================================================================

                        
                //        //GET SUBTOTAL, SUBTOTAL PPN, SUBTOTAL PPH
                //        SubTotal = Price * Qty;
                //        SubTotalPPN = SubTotal * PPN;
                //        SubTotalPPH = SubTotal * PPH;
                //        //END GET SUBTOTAL                       

                //        //CREATE DETAIL=================================================================================
                //        //PR============================================================================================
                //        Query = "Insert PurchRequisition_Dtl (PurchReqID,SeqNo,FullItemID, ";
                //        Query += "ItemName,DeliveryMethod,OrderDate, ";
                //        Query += "VendID,Qty, Qty_KG,Unit,Ratio, GroupId, SubGroup1Id, SubGroup2Id, ItemId, ";
                //        Query += "Price, SeqNoGroup,TransStatus,TransStatusPurch,ApprovePersonPurch,CreatedDate,CreatedBy) Values ";
                //        Query += "('" + PRNumber + "','";
                //        Query += "" + SeqNoGroup + "','";
                //        Query += FullItemID + "','";
                //        Query += ItemName + "','";
                //        Query += "" + DeliveryMethod + "','";
                //        Query += "" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "','";
                //        Query += "" + txtVendorID.Text + "',";
                //        Query += Qty + ",";
                //        Query += "" + Qty_KG + ",'";
                //        Query += Unit + "',";
                //        Query += "" + Ratio + ",'";
                //        Query += "" + GroupID + "','";
                //        Query += "" + SubGroup1ID + "','";
                //        Query += "" + SubGroup2ID + "','";
                //        Query += "" + ItemID + "',";
                //        Query += "0,";
                //        Query += "" + SeqNo + ",'";
                //        Query += "Yes','";
                //        Query += "Yes','";
                //        Query += "" + ControlMgr.UserId + "',";
                //        Query += "getdate(),";
                //        Query += "'" + ControlMgr.UserId + "');";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();                                             
                //        //END PR========================================================================================


                //        //QUOTATION=====================================================================================
                //        Query = "Insert PurchQuotation_Dtl (PurchQuotID,OrderDate,SeqNo,GroupID,SubGroup1ID,SubGroup2ID, ";
                //        Query += "ItemID,FullItemID,ItemName,Qty,Qty_KG,Unit,Ratio,Price,Price_KG,ReffTransID, ";
                //        Query += "ReffSeqNo,ReffTransType,ApprovalNotes,CreatedDate,CreatedBy,Qty2,DeliveryMethod,SubTotal, ";
                //        Query += "SubTotal_PPN,SubTotal_PPH,SeqNoGroup) VALUES ";
                //        Query += "('" + PQNumber + "', ";
                //        Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "','";
                //        Query += "" + SeqNoGroup + "',";
                //        Query += "'" + GroupID + "', ";
                //        Query += "'" + SubGroup1ID + "', ";
                //        Query += "'" + SubGroup2ID + "', ";
                //        Query += "'" + ItemID + "', ";
                //        Query += "'" + FullItemID + "',' ";
                //        Query += ItemName + "',";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty_KG + ",' ";
                //        Query += Unit + "',";
                //        Query += "" + Ratio + ", ";
                //        Query += "" + Price + ", ";
                //        Query += "" + Price_KG + ", ";
                //        Query += "'" + PRNumber + "', ";
                //        Query += "" + SeqNo + ", ";
                //        Query += "'FIX', ";
                //        Query += "'" + txtNotes.Text + "', ";
                //        Query += "getdate(), ";
                //        Query += "'" + ControlMgr.UserId + "', ";
                //        Query += "" + Qty + ", ";
                //        Query += "'" + DeliveryMethod + "', ";
                //        Query += "" + SubTotal + ", ";
                //        Query += "" + SubTotalPPN + ", ";
                //        Query += "" + SubTotalPPH + ", ";
                //        Query += "" + SeqNo + " ";
                //        Query += ")";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END QUOTATION===================================================================================


                //        //CANVAS SHEET====================================================================================
                //        Query = "Insert CanvasSheetD ( ";
                //        Query += "CanvasId, CanvasSeqNo, VendID, PurchQuotId, PurchReqId, PurchReqSeqNo, ";
                //        Query += "DeliveryMethod, FullItemId, ItemName, Price, Price_KG, SeqNo, SeqNoGroup, PRQty, ";
                //        Query += "PQQty, Qty, Qty_KG, Unit, Ratio, StatusApproval, PPN, PPH, CreatedDate, CreatedBy";
                //        Query += ") ";
                //        Query += "VALUES ( ";
                //        Query += "'" + CSNumber + "',";
                //        Query += "'" + SeqNoGroup + "',";
                //        Query += "'" + txtVendorID.Text + "',";
                //        Query += "'" + PQNumber + "',";
                //        Query += "'" + PRNumber + "',";
                //        Query += "'" + SeqNoGroup + "',";
                //        Query += "'" + DeliveryMethod + "',";
                //        Query += "'" + FullItemID + "',";
                //        Query += "'" + ItemName + "',";
                //        Query += "" + Price + ",";
                //        Query += "" + Price_KG + ",";
                //        Query += "'" + SeqNoGroup + "',";
                //        Query += "" + SeqNo + ",";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty_KG + ",";
                //        Query += "'" + Unit + "',";
                //        Query += "" + Ratio + ",";
                //        Query += "'Yes',";
                //        Query += "" + PPN * 100 + ",";
                //        Query += "" + PPH * 100 + ",";
                //        Query += "getdate(),";
                //        Query += "'" + ControlMgr.UserId + "'";
                //        Query += ")";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();                      
                    
                //        //PO=========================================================================================
                //        Query = "Insert PurchDtl ( ";
                //        Query += "PurchID, OrderDate, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ";
                //        Query += "ItemName, InventSiteID, Qty, Qty_KG, RemainingQty, Unit, Konv_Ratio, Price, Price_KG,";
                //        Query += "Total, Diskon, Total_Disk, Total_PPN, Total_PPH, ReffTableName, ReffId, ReffId2, ";
                //        Query += "ReffSeqNo, CreatedDate, CreatedBy, AvailableDate, DeliveryMethod";
                //        Query += ") VALUES (";
                //        Query += "'" + PONumber + "',";
                //        Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //        Query += "'" + SeqNoGroup + "',";
                //        Query += "'" + GroupID + "',";
                //        Query += "'" + SubGroup1ID + "',";
                //        Query += "'" + SubGroup2ID + "',";
                //        Query += "'" + ItemID + "',";
                //        Query += "'" + FullItemID + "',";
                //        Query += "'" + ItemName + "',";
                //        Query += "'" + InventSiteID + "',";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty_KG + ",";
                //        Query += "0,";
                //        Query += "'" + Unit + "',";
                //        Query += "" + Ratio + ",";
                //        Query += "" + Price + ",";
                //        Query += "" + Price_KG + ",";
                //        Query += "" + SubTotal + ",";
                //        Query += "0,";
                //        Query += "0,";
                //        Query += "" + SubTotalPPN + ",";
                //        Query += "" + SubTotalPPH + ",";
                //        Query += "'CanvasSheet',";
                //        Query += "'" + CSNumber + "',";
                //        Query += "'" + PQNumber + "',";
                //        Query += "'" + SeqNo + "',";
                //        Query += "getdate(),";
                //        Query += "'" + ControlMgr.UserId + "',";
                //        Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //        Query += "'" + DeliveryMethod + "'";
                //        Query += ")";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();


                //        ////UPDATE QTY PO ISSUED==========================================================================
                //        //Query = "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM=(PO_Issued_Outstanding_UoM+" + QtyUoM + "), PO_Issued_Outstanding_Alt=(PO_Issued_Outstanding_Alt+" + QtyAlt + "), ";
                //        //Query += "PO_Issued_Outstanding_Amount=(PO_Issued_Outstanding_Amount+" + (SubTotal+SubTotalPPN+SubTotalPPH) + ") ";
                //        //Query += "where FullItemID='" + FullItemID + "';";
                //        //Cmd = new SqlCommand(Query, Conn, Trans);
                //        //Cmd.ExecuteNonQuery();
                //        //END UPDATE QTY PO ISSUED======================================================================
                //        //END PO========================================================================================


                //        //RO============================================================================================
                //        Query = "Insert ReceiptOrderD(ReceiptOrderId, PurchaseOrderId, PurchaseOrderSeqNo, SeqNo, ";
                //        Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_KG, RemainingQty, ";
                //        Query += "Unit, Ratio, Price, Price_KG, Total, Total_PPN, Total_PPH, InventSiteID, ";
                //        Query += "CreatedDate, CreatedBy, DeliveryMethod) VALUES( ";
                //        Query += "'" + RONumber + "',";
                //        Query += "'" + PONumber + "',";
                //        Query += "" + SeqNoGroup + ",";
                //        Query += "" + SeqNoGroup + ",";
                //        Query += "'" + GroupID + "',";
                //        Query += "'" + SubGroup1ID + "',";
                //        Query += "'" + SubGroup2ID + "',";
                //        Query += "'" + ItemID + "',";
                //        Query += "'" + FullItemID + "',";
                //        Query += "'" + ItemName + "',";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty_KG + ",";
                //        Query += "0,";
                //        Query += "'" + Unit + "',";
                //        Query += "" + Ratio + ",";
                //        Query += "" + Price + ",";
                //        Query += "" + Price_KG + ",";
                //        Query += "" + SubTotal + ",";
                //        Query += "" + SubTotalPPN + ",";
                //        Query += "" + SubTotalPPH + ",";
                //        Query += "'" + InventSiteID + "',";
                //        Query += "getdate(),";
                //        Query += "'" + ControlMgr.UserId + "',";
                //        Query += "'" + DeliveryMethod + "'";
                //        Query += ")";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();


                //        //UPDATE QTY RO ISSUED========================================================================
                //        //Query = "Update Invent_Purchase_Qty set RO_Issued_UoM=(RO_Issued_UoM+" + QtyUoM + "), RO_Issued_Alt=(RO_Issued_Alt+" + QtyAlt + ") ";
                //        //Query += "where FullItemID='" + FullItemID + "';";
                //        //Cmd = new SqlCommand(Query, Conn, Trans);
                //        //Cmd.ExecuteNonQuery();
                //        //END UPDATE QTY RO ISSUED======================================================================
                //        //END RO========================================================================================


                //        //GR============================================================================================
                //        //INSERT INVENT RESIZE==========================================================================
                //        Query = "SELECT TOP 1 Resize, ResizeType ";
                //        Query += "FROM InventTable ";
                //        Query += "WHERE FullItemID = '" + FullItemID + "' ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            Resize = Convert.ToString(Dr["Resize"]);
                //            ResizeType = Convert.ToString(Dr["ResizeType"]);
                //        }
                //        Dr.Close();


                //        Query = "SELECT TOP 1 InventSiteId, InventSiteBlokID, Quality ";
                //        Query += "FROM GoodsReceivedD ";
                //        Query += "WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                //        Query += "AND GoodsReceivedSeqNo = '" + GoodReceivedSeqNo + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            InventSiteIDDetail = Convert.ToString(Dr["InventSiteId"]);
                //            InventSiteBlokIDDetail = Convert.ToString(Dr["InventSiteBlokID"]);
                //            Quality = Convert.ToString(Dr["Quality"]);
                //        }
                //        Dr.Close();

                //        decimal Amount = 0;
                //        if (Unit.ToUpper() == "BTG")
                //        {
                //            Amount = Qty * Price;
                //        }
                //        else
                //        {
                //            Amount = (Qty * ConvRatio) * Price;
                //        }

                //        if (Resize.ToUpper() == "TRUE")
                //        {
                //            ActionStatusCodeDetail = "04";

                //            if (ResizeLoop == 1)
                //            {
                //                Jenis = "NRZ";
                //                Kode = "NRZA";
                //                TransId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                //                Query = "INSERT INTO NotaResizeH ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                //                Query += "[SiteID], [VendID], [Posted], [CreatedDate], [CreatedBy]) VALUES ( ";
                //                Query += "'" + TransId + "',";
                //                Query += "getdate(),";
                //                Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //                Query += "'" + NewGRNumber + "',";
                //                Query += "'" + InventSiteID + "',";
                //                Query += "'" + txtVendorID.Text + "',";
                //                Query += "1,";
                //                Query += "getdate(),";
                //                Query += "'" + ControlMgr.UserId + "'";
                //                Query += ")";
                //                Cmd = new SqlCommand(Query, Conn, Trans);
                //                Cmd.ExecuteNonQuery();

                //                ResizeLoop++;
                //            }

                //            //Query = "INSERT INTO ResizeH (TransDate, TransId, CreatedDate, CreatedBy, ";
                //            //Query += "Posted, ResizeType) VALUES ( ";
                //            //Query += "getdate(),";
                //            //Query += "('" + TransId + "',";
                //            //Query += "getdate(),";
                //            //Query += "'" + ControlMgr.UserId + "',";
                //            //Query += "1,";
                //            //Query += "'" + ResizeType + "'";
                //            //Query += ")";
                //            //Cmd = new SqlCommand(Query, Conn, Trans);
                //            //Cmd.EndExecuteNonQuery();                            

                //            Query = "SELECT COUNT(*) FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            CountResize = Convert.ToInt32(Cmd.ExecuteScalar().ToString());

                //            if (CountResize > 1)
                //            {
                //                Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], ";
                //                Query += "[Qty], [Price], [Unit], [LineAmount], [GoodsReceivedId], ";
                //                Query += "[GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                //                Query += "'" + TransId + "',";
                //                Query += "(SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') + 1  END AS SeqNo),";
                //                Query += "'" + FullItemID + "',";
                //                Query += "'" + ItemName + "',";
                //                Query += "'" + Qty + "',";
                //                Query += "" + Price + ",";
                //                Query += "'" + Unit + "',";
                //                Query += "'" + SubTotal + "',";
                //                Query += "'" + NewGRNumber + "',";
                //                Query += "'" + SeqNoGroup + "',";
                //                Query += "getdate(),";
                //                Query += "'" + ControlMgr.UserId + "'";
                //                Query += ")";
                //            }
                //            else
                //            {
                //                Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], [ToFullItemId], [ToItemName], ";
                //                Query += "[Qty], [Price], [Unit], [LineAmount], [GoodsReceivedId], ";
                //                Query += "[GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                //                Query += "'" + TransId + "',";
                //                Query += "(SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') + 1  END AS SeqNo),";
                //                Query += "'" + FullItemID + "',";
                //                Query += "'" + ItemName + "',";
                //                Query += "(SELECT To_FullItemId FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'),";
                //                Query += "(SELECT To_ItemName FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'),";
                //                Query += "'" + Qty + "',";
                //                Query += "" + Price + ",";
                //                Query += "'" + Unit + "',";
                //                Query += "'" + SubTotal + "',";
                //                Query += "'" + NewGRNumber + "',";
                //                Query += "'" + SeqNoGroup + "',";
                //                Query += "getdate(),";
                //                Query += "'" + ControlMgr.UserId + "'";
                //                Query += ")";
                //            }
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();

                //            //Query = "INSERT INTO Resize_Dtl(TransId, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ";
                //            //Query += "ItemId, FullItemId, ItemName, InventSiteIdIssue, InventSiteIdReceive, Qty, ";
                //            //Query += "Unit, RefTransId, RefSeqNo, CreatedDate, CreatedBy, ResizeType) VALUES ( ";
                //            //Query += "'" + TransId + "',";
                //            //Query += "" + SeqNoResize + ",";
                //            //Query += "'" + GroupID + "',";
                //            //Query += "'" + SubGroup1ID + "',";
                //            //Query += "'" + SubGroup2ID + "',";
                //            //Query += "'" + ItemID + "',";
                //            //Query += "'" + FullItemID + "',";
                //            //Query += "'" + ItemName + "',";
                //            //Query += "'" + InventSiteId + "',";
                //            //Query += "'" + InventSiteId + "',";
                //            //Query += "" + -Qty + ",";
                //            //Query += "'" + Unit + "',";
                //            //Query += "'" + NewGRNumber + "',";
                //            //Query += "'" + SeqNoGroup + "',";
                //            //Query += "getdate(),";
                //            //Query += "'" + ControlMgr.UserId + "',";
                //            //Query += "'" + ResizeType + "'";
                //            //Query += ")";
                //            //Cmd = new SqlCommand(Query, Conn, Trans);
                //            //Cmd.ExecuteNonQuery();

                //            //if (ResizeType != "Manual")
                //            //{
                //            //    Query = "SELECT TOP 1 ToFullItemId, ToItemName FROM [ResizeTableD] ";
                //            //    Query += "WHERE FullItemId = '" + FullItemID + "'";
                //            //    Cmd = new SqlCommand(Query, Conn, Trans);
                //            //    Dr = Cmd.ExecuteReader();
                //            //    while (Dr.Read())
                //            //    {
                //            //        ToFullItemId = Convert.ToString(Dr["ToFullItemId"]);
                //            //        ToItemName = Convert.ToString(Dr["ToItemName"]);
                //            //    }
                //            //    Dr.Close();

                //            //    string[] SplitToFullItemID = ToFullItemId.Split('.');
                //            //    ToGroupID = Convert.ToString(SplitToFullItemID[0]);
                //            //    ToSubGroup1ID = Convert.ToString(SplitToFullItemID[1]);
                //            //    ToSubGroup2ID = Convert.ToString(SplitToFullItemID[2]);
                //            //    ToItemID = Convert.ToString(SplitToFullItemID[3]);

                //            //    Query = "INSERT INTO Resize_Dtl(TransId, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ";
                //            //    Query += "ItemId, FullItemId, ItemName, InventSiteIdIssue, InventSiteIdReceive, Qty, ";
                //            //    Query += "Unit, RefTransId, RefSeqNo, OriginalGroupId, OriginalSubGroup1Id, OriginalSubGroup2Id, OriginalItemId, ";
                //            //    Query += "OriginalFullItemId, ParentSeqNo, CreatedDate, CreatedBy, OriginalItemName, ResizeType) VALUES ( ";
                //            //    Query += "'" + TransId + "',";
                //            //    Query += "" + (SeqNoResize + 1) + ",";
                //            //    Query += "'" + ToGroupID + "',";
                //            //    Query += "'" + ToSubGroup1ID + "',";
                //            //    Query += "'" + ToSubGroup2ID + "',";
                //            //    Query += "'" + ToItemID + "',";
                //            //    Query += "'" + ToFullItemId + "',";
                //            //    Query += "'" + ToItemName + "',";
                //            //    Query += "'" + InventSiteId + "',";
                //            //    Query += "'" + InventSiteId + "',";
                //            //    Query += "" + Qty + ",";
                //            //    Query += "'" + Unit + "',";
                //            //    Query += "'" + NewGRNumber + "',";
                //            //    Query += "'" + SeqNoGroup + "',";
                //            //    Query += "'" + GroupID + "',";
                //            //    Query += "'" + SubGroup1ID + "',";
                //            //    Query += "'" + SubGroup2ID + "',";
                //            //    Query += "'" + ItemID + "',";
                //            //    Query += "'" + FullItemID + "',";
                //            //    Query += "'" + SeqNoResize + "',";
                //            //    Query += "getdate(),";
                //            //    Query += "'" + ControlMgr.UserId + "',";
                //            //    Query += "'" + ItemName + "',";
                //            //    Query += "'" + ResizeType + "'";
                //            //    Query += ")";
                //            //    Cmd = new SqlCommand(Query, Conn, Trans);
                //            //    Cmd.ExecuteNonQuery();
                //            //}


                //            //INSERT WORKFLOW LOG==========================================================================
                //            if (WorkflowLogDetail == 0)
                //            {
                //                Query = "SELECT Deskripsi FROM TransStatusTable ";
                //                Query += "WHERE StatusCode='" + ActionStatusCodeDetail + "' AND TransCode = 'GRD' ";
                //                Cmd = new SqlCommand(Query, Conn, Trans);
                //                Dr = Cmd.ExecuteReader();
                //                while (Dr.Read())
                //                {
                //                    StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //                }
                //                Dr.Close();

                //                Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //                Query += "VALUES('GoodsReceivedD', '" + NewGRNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '" + ActionStatusCodeDetail + "', '" + StatusDesc + "', getdate()) ";
                //                Cmd = new SqlCommand(Query, Conn, Trans);
                //                Cmd.ExecuteNonQuery();
                //                WorkflowLogDetail++;
                //            }
                //            //END INSERT WORKFLOW LOG======================================================================

                //        }
                //        else
                //        {
                //            Query = "Update Invent_OnHand_Qty SET Available_For_Sale_UoM = (Available_For_Sale_UoM+" + QtyUoM + "), ";
                //            Query += "Available_For_Sale_Alt = (Available_For_Sale_Alt+" + QtyAlt + "), Available_For_Sale_Amount = (Available_For_Sale_Amount+ " + Amount + ") ";
                //            Query += "where FullItemID='" + FullItemID + "' ";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();

                //            Query = "INSERT INTO InventTrans ([GroupId], [SubGroupId], [SubGroup2Id], [ItemId], [FullItemId], ";
                //            Query += "[ItemName], [InventSiteId], [TransId], [SeqNo], [TransDate], [Ref_TransId], [Ref_TransDate], ";
                //            Query += "[Ref_Trans_SeqNo], [AccountId], [AccountName], [Available_UoM], [Available_Alt], ";
                //            Query += "[Available_Amount], [Available_For_Sale_UoM], [Available_For_Sale_Alt], [Available_For_Sale_Amount]) VALUES ( ";
                //            Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', ";
                //            Query += "'" + ItemName + "', '" + InventSiteID + "', '" + txtNotaNumber.Text + "', " + SeqNoGroup + ", getdate(), ";
                //            Query += "'" + NewGRNumber + "', getdate(), '" + SeqNoGroup + "', '" + txtVendorID.Text + "', '" + txtVendorName.Text + "', ";
                //            Query += "" + QtyUoM + ", " + QtyAlt + ", " + Amount + "," + QtyUoM + ", " + QtyAlt + ", " + Amount + " ";
                //            Query += ")";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();
                        
                //        }
                //        //END INSERT INVENT RESIZE=========================================================================

                //        Query = "INSERT INTO GoodsReceivedD ( ";
                //        Query += "GoodsReceivedDate, GoodsReceivedId, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, ";
                //        Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_SJ, Qty_Actual, ";
                //        Query += "Unit, Ratio, Ratio_Actual, InventSiteId, InventSiteBlokID, Quality, ActionCodeStatus, ";
                //        Query += "CreatedDate, CreatedBy, DeliveryMethod, NPPId, NPP_SeqNo, Price, Total, PPN, Total_PPN, PPH, Total_PPH) ";
                //        Query += "VALUES (";
                //        Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //        Query += "'" + NewGRNumber + "',";
                //        Query += "'" + SeqNoGroup + "',";
                //        Query += "'" + RONumber + "',";
                //        Query += "'" + SeqNoGroup + "',";
                //        Query += "'" + GroupID + "',";
                //        Query += "'" + SubGroup1ID + "',";
                //        Query += "'" + SubGroup2ID + "',";
                //        Query += "'" + ItemID + "',";
                //        Query += "'" + FullItemID + "',";
                //        Query += "'" + ItemName + "',";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty + ",";
                //        Query += "'" + Unit + "',";
                //        Query += "'" + Ratio + "',";
                //        Query += "'" + Ratio + "',";
                //        Query += "'" + InventSiteId + "',";
                //        Query += "'" + InventSiteBlokID + "',";
                //        Query += "'" + Quality + "',";
                //        Query += "'" + ActionStatusCodeDetail + "',";
                //        Query += "getdate(),";
                //        Query += "'" + ControlMgr.UserId + "',";
                //        Query += "'" + DeliveryMethod + "',";
                //        Query += "'" + txtNotaNumber.Text + "',";
                //        Query += "'" + SeqNo + "',";
                //        Query += "" + Price + ",";
                //        Query += "" + SubTotal + ",";
                //        Query += "" + PPN * 100 + ",";
                //        Query += "" + SubTotalPPN + ",";
                //        Query += "" + PPH * 100 + ",";
                //        Query += "" + SubTotalPPH + "";
                //        Query += ")";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //       //END GR=========================================================================================
                //        //END CREATED DETAIL============================================================================                      

                        
                       

                //        Query = "Update Invent_Movement_Qty SET Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM- " + QtyUoM + "), ";
                //        Query += "Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt- " + QtyAlt + "), ";
                //        Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount-" + Amount + ") ";
                //        Query += "where FullItemID='" + FullItemID + "' ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();                        

                //        if (Resize.ToUpper() == "TRUE")
                //        {
                //            Query = "Update Invent_Movement_Qty SET Resize_In_Progress_UoM = (Resize_In_Progress_UoM + " + QtyUoM + "), ";
                //            Query += "Resize_In_Progress_Alt = (Resize_In_Progress_Alt + " + QtyAlt + "), ";
                //            Query += "Resize_In_Progress_Amount = (Resize_In_Progress_Amount+ " + Amount + ") ";
                //            Query += "where FullItemID='" + FullItemID + "' ";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();
                //        }
                //        //END SYSTEM UPDATE============================================================================


                //        //SYSTEM INSERT=================================================================================
                      
                //        Query = "INSERT INTO NotaPurchasePark_LogTable ([NPPId], [NPPDate], [RefTrans2Id], [RefTrans2Date], ";
                //        Query += "[AccountId], [AccountName], [InventSiteID], [Qty_UoM], ";
                //        Query += "[Qty_Alt], [Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES ( ";
                //        Query += "'" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', ";
                //        Query += "'" + NewGRNumber + "', getdate(), '" + txtVendorID.Text + "', '" + txtVendorName.Text + "', ";
                //        Query += "'" + InventSiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '04', ";
                //        Query += "'Approved by " + ControlMgr.GroupName + "', 'Status: 04. Approved by " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate()";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        if (Resize.ToUpper() == "TRUE")
                //        {
                //            if (CountResize > 1)
                //            {
                //                Query = "INSERT INTO NotaResize_LogTable ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                //                Query += "[VendID], [InventSiteID], [FullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], ";
                //                Query += "[Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                //                Query += "'" + TransId + "', getdate(), getdate(), '" + NewGRNumber + "', ";
                //                Query += "'" + txtVendorID.Text + "', '" + InventSiteID + "', '" + FullItemID + "',";
                //                Query += "" + SeqNoGroup + ", " + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '01', ";
                //                Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                //                Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                //                Query += "'"+ControlMgr.UserId+"', getdate()";
                //                Query += ")";
                               
                //            }
                //            else
                //            {
                //                Query = "INSERT INTO NotaResize_LogTable ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                //                Query += "[VendID], [InventSiteID], [FullItemId], [ToFullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], ";
                //                Query += "[Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                //                Query += "'" + TransId + "', getdate(), getdate(), '" + NewGRNumber + "', ";
                //                Query += "'" + txtVendorID.Text + "', '" + InventSiteID + "', '" + FullItemID + "', ";
                //                Query += "(SELECT To_FullItemId FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'), ";                               
                //                Query += "" + SeqNoGroup + ", " + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '01', ";
                //                Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                //                Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                //                Query += "'" + ControlMgr.UserId + "', getdate()";
                //                Query += ")";
                //            }
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();
                //        }
                       

                //        //END SYSTEM INSERT=============================================================================

                //        SeqNoGroup++;
                //    }
                //    DrDetail.Close();
                //    //END GET DATA DETAIL===============================================================================

                //    //UPDATE Amount NotaResizeH
                //    Query = "UPDATE NotaResizeH SET Amount = (SELECT SUM(Qty * Price) FROM NotaResize_Dtl ";
                //    Query += "WHERE NRZId = '" + TransId + "') WHERE NRZId = '" + TransId + "'";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //END UPDATE Amount NotaResizeH

                //}
                ////End Get Count Data New Purchase=======================================================================
                ////END NEW PURCHASE======================================================================================


                ////RETUR TUKAR BARANG====================================================================================
                ////Get Count Data Retur Tukar Barang=====================================================================
                //Query ="SELECT COUNT(FullItemID) CountData FROM NotaPurchaseParkD ";
                //Query += "WHERE NPPID = '"+txtNotaNumber.Text.Trim()+"' AND ActionCode = '01'";
                //Cmd = new SqlCommand(Query, Conn, Trans);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    CountData = Convert.ToInt32(Dr["CountData"]);
                //}
                //Dr.Close();

                //if (CountData > 0)
                //{
                //    string Jenis = "NRB", Kode = "NRBA";
                //    string ReturTukarBarang = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                //    ReturTBRefNumber = ReturTukarBarang;
                //    string PurchID = "", SiteID = "", SiteName = "", UnitAlt = "";
                //    int SeqNoRTB = 1;
                //    decimal RatioRTB = 0, Amount = 0, PricePO = 0;
                //    string GoodsReceivedDateRTB = "";

                //    Query = "SELECT R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, G.GoodsReceivedDate ";
                //    Query += "FROM ReceiptOrderH R INNER JOIN GoodsReceivedH G ";
                //    Query += "ON G.RefTransID = R.ReceiptOrderId WHERE G.GoodsReceivedId = '"+txtGoodsReceivedNumber.Text+"' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                //        SiteID = Convert.ToString(Dr["InventSiteID"]);
                //        SiteName = Convert.ToString(Dr["InventSiteName"]);
                //        GoodsReceivedDateRTB = Convert.ToString(Dr["GoodsReceivedDate"]);
                //    }
                //    Dr.Close();                   

                //    Query = "INSERT INTO [NotaReturBeliH] ([NRBId], [NRBDate], [NRBMode], [VendId], [VendName], [GoodsReceivedId], [GoodsReceivedDate], ";
                //    Query += "[PurchId], [SiteId], [SiteName], [TransStatusId], [ActionCode], [CreatedDate], [CreatedBy]) VALUES";
                //    Query += "('" + ReturTukarBarang + "', getdate(), 'AUTO', '"+txtVendorID.Text+"', ";
                //    Query += "'"+txtVendorName.Text+"', '"+txtGoodsReceivedNumber.Text+"', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', ";
                //    Query += "'" + PurchID + "', '" + SiteID + "', '" + SiteName + "', '03', '01', getdate(), '"+ControlMgr.UserId+"') ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();

                //     //GET DATA DETAIL===================================================================================
                //    Query = "SELECT FullItemID, ItemName, Qty, Unit, Price, SeqNo, GoodsReceived_SeqNo ";
                //    Query += "FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '01' ORDER BY SeqNo ASC ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    DrDetail = Cmd.ExecuteReader();

                //    while (DrDetail.Read())
                //    {
                //        FullItemID = Convert.ToString(DrDetail["FullItemID"]);
                //        ItemName = Convert.ToString(DrDetail["ItemName"]);
                //        Qty = Convert.ToString(DrDetail["Qty"]) == null ? 0 : Convert.ToDecimal(DrDetail["Qty"]);
                //        Unit = Convert.ToString(DrDetail["Unit"]);
                //        Price = Convert.ToString(DrDetail["Price"]) == null ? 0 : Convert.ToDecimal(DrDetail["Price"]);
                //        SeqNo = Convert.ToInt32(DrDetail["SeqNo"]);
                //        GoodReceivedSeqNo = Convert.ToInt32(DrDetail["GoodsReceived_SeqNo"]);

                //        SplitFullItemID = FullItemID.Split('.');
                //        GroupID = Convert.ToString(SplitFullItemID[0]);
                //        SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                //        SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                //        ItemID = Convert.ToString(SplitFullItemID[3]);

                //        Query = "SELECT Ratio FROM InventConversion WHERE FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        RatioRTB = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());

                //        Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());                       

                //        //SYSTEM UPDATE=================================================================================
                //        Query = "SELECT P.Price, N.* FROM PurchDtl P INNER JOIN ReceiptOrderD R ON R.PurchaseOrderId = P.PurchID ";
                //        Query += "INNER JOIN GoodsReceivedD G ON G.RefTransID = R.ReceiptOrderId ";
                //        Query += "INNER JOIN NotaPurchaseParkD N ON N.GoodsReceivedID = G.GoodsReceivedId ";
                //        Query += "AND R.PurchaseOrderSeqNo = P.SeqNo AND G.RefTransSeqNo = R.SeqNo ";
                //        Query += "AND N.GoodsReceivedID = G.GoodsReceivedId AND N.GoodsReceived_SeqNo = G.GoodsReceivedSeqNo ";
                //        Query += "AND N.NPPID = '"+txtNotaNumber.Text+"' AND N.GoodsReceived_SeqNo = '"+GoodReceivedSeqNo+"' AND N.GoodsReceivedID = '"+txtGoodsReceivedNumber.Text+"'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        PricePO = Convert.ToDecimal(Cmd.ExecuteScalar());

                //        if (PricePO == 0)
                //        {
                //            PricePO = 1;
                //        }

                //        if (Unit.ToUpper() == "BTG")
                //        {
                //            Amount = Qty * PricePO;
                //        }
                //        else
                //        {
                //            Amount = (Qty * ConvRatio) * PricePO;
                //        }

                //        //GET UOM, ALT===============================================================================
                //        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //        UoM = Cmd.ExecuteScalar().ToString();
                //        if (Unit == UoM)
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyUoM = Qty;
                //            QtyAlt = Qty * ConvRatio;
                //        }
                //        else
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyAlt = Qty;
                //            QtyUoM = Qty / ConvRatio;
                //        }
                //        //END GET UOM, ALT==============================================================================


                //        Query = "INSERT INTO [NotaReturBeli_Dtl] ([NRBId], [SeqNo], [GroupId], [SubGroup1Id], [SubGroup2Id], ";
                //        Query += "[ItemId], [FullItemId], [ItemName], [GoodsReceivedId], [GoodsReceived_SeqNo], [UoM_Qty], ";
                //        Query += "[Alt_Qty], [UoM_Unit], [Alt_Unit], [Ratio], [RemainingQty], [InventSiteId], ";
                //        Query += "[CreatedDate], [CreatedBy], [Price]) VALUES ( ";
                //        Query += "'" + ReturTukarBarang + "', '" + SeqNoRTB + "', '" + GroupID + "', '" + SubGroup1ID + "', ";
                //        Query += "'" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                //        Query += "'" + txtGoodsReceivedNumber.Text + "', '" + GoodReceivedSeqNo + "', " + Qty + ", " + (Qty * RatioRTB) + ", ";
                //        Query += "'" + Unit + "', '" + UnitAlt + "', " + RatioRTB + ", " + Qty + " ,'" + SiteID + "', ";
                //        Query += "getdate(), '" + ControlMgr.UserId + "', " + PricePO + "";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                        
                //        Query = "UPDATE Invent_Purchase_Qty SET Retur_Beli_In_Progress_Alt = (Retur_Beli_In_Progress_Alt + " + QtyAlt + "), ";
                //        Query += "Retur_Beli_In_Progress_UoM = (Retur_Beli_In_Progress_UoM + " + QtyUoM + "), ";
                //        Query += "Retur_Beli_In_Progress_Amount = (Retur_Beli_In_Progress_Amount +  " + Amount + ")";
                //        Query += "WHERE FullItemId = '"+FullItemID+"'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        Query = "Update Invent_Movement_Qty SET Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM- " + QtyUoM + "), ";
                //        Query += "Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt- " + QtyAlt + "), ";
                //        Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount-" + Amount + ") ";
                //        Query += "where FullItemID='" + FullItemID + "' ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();       
                //        //END SYSTEM UPDATE=============================================================================

                //        //SYSTEM INSERT=================================================================================
                //        Query = "INSERT INTO NotaPurchasePark_LogTable ([NPPId], [NPPDate], [RefTransId], [RefTransDate], [RefTrans2Id], [RefTrans2Date], ";
                //        Query += "[AccountId], [AccountName], [InventSiteID], [Qty_UoM], ";
                //        Query += "[Qty_Alt], [Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES ( ";
                //        Query += "'" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', ";
                //        Query += "'" + ReturTukarBarang + "', getdate(), '" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceivedDateRTB + "', '" + txtVendorID.Text + "', '" + txtVendorName.Text + "', ";
                //        Query += "'" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '01', ";
                //        Query += "'Approved by " + ControlMgr.GroupName + "', 'Status: 01. Approved by " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate()";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        Query = "INSERT INTO [NotaReturBeli_LogTable] ([NRBDate], [NRBId], [GoodsReceivedDate], [GoodsReceivedId], ";
                //        Query += "[VendId], [SiteId], [FullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], [Amount], ";
                //        Query += "[LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                //        Query += "getdate(), '" + ReturTukarBarang + "', '" + GoodsReceivedDateRTB + "', '" + txtGoodsReceivedNumber.Text + "', ";
                //        Query += "'" + txtVendorID.Text + "', '" + SiteID + "', '" + FullItemID + "', '" + SeqNoRTB + "', ";
                //        Query += ""+QtyUoM+", "+QtyAlt+", "+Amount+", '03', 'Auto - Approved by " + ControlMgr.GroupName +"', ";
                //        Query += "'Status: 03. Auto – Approved by "+ControlMgr.GroupName +"', '"+ControlMgr.UserId+"', getdate()";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END SYSTEM INSERT=================================================================================
                       
                //        SeqNoRTB++;
                //    }
                //    DrDetail.Close();

                //    //GET DATA DETAIL===================================================================================
                  
                //}
                ////End Get Count Data Retur Tukar Barang==================================================================
                ////END RETUR TUKAR BARANG=================================================================================


                ////RETUR DEBIT====================================================================================
                ////Get Count Data Retur Debit=====================================================================
                //Query = "SELECT COUNT(FullItemID) CountData FROM NotaPurchaseParkD ";
                //Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '02'";
                //Cmd = new SqlCommand(Query, Conn, Trans);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    CountData = Convert.ToInt32(Dr["CountData"]);
                //}
                //Dr.Close();

                //if (CountData > 0)
                //{  
                //    string Jenis = "NRB", Kode = "NRBA";
                //    string ReturDebitNote = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                //    ReturDNRefNumber = ReturDebitNote;
                //    string PurchID = "", SiteID = "", SiteName = "", UnitAlt = "";
                //    int SeqNoRDN = 1;
                //    decimal RatioRDN = 0, Amount = 0, PricePO = 0;
                //    string GoodsReceivedDateRDN = "";
                //    string RONumber = "";

                //    Query = "SELECT R.ReceiptOrderId, R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, G.GoodsReceivedDate ";
                //    Query += "FROM ReceiptOrderH R INNER JOIN GoodsReceivedH G ";
                //    Query += "ON G.RefTransID = R.ReceiptOrderId WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                //        SiteID = Convert.ToString(Dr["InventSiteID"]);
                //        SiteName = Convert.ToString(Dr["InventSiteName"]);
                //        GoodsReceivedDateRDN = Convert.ToString(Dr["GoodsReceivedDate"]);
                //        RONumber = Convert.ToString(Dr["ReceiptOrderId"]);
                //    }
                //    Dr.Close();

                //    Query = "INSERT INTO [NotaReturBeliH] ([NRBId], [NRBDate], [NRBMode], [VendId], [VendName], [GoodsReceivedId], [GoodsReceivedDate], ";
                //    Query += "[PurchId], [SiteId], [SiteName], [TransStatusId], [ActionCode], [CreatedDate], [CreatedBy]) VALUES";
                //    Query += "('" + ReturDebitNote + "', getdate(), 'AUTO', '" + txtVendorID.Text + "', ";
                //    Query += "'" + txtVendorName.Text + "', '" + txtGoodsReceivedNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', ";
                //    Query += "'" + PurchID + "', '" + SiteID + "', '" + SiteName + "', '03', '02', getdate(), '" + ControlMgr.UserId + "') ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();

                //    Query = "SELECT c.ExchRate,c.TermofPayment, ";
                //    Query += "c.PaymentMode, c.PPN, c.PPH, a.SiteID, a.SiteName, ";
                //    Query += "a.SiteLocation, a.VehicleType, a.VehicleNumber, a.DriverName, ";
                //    Query += "a.GoodsReceivedDate, a.SJNumber, a.SJDate FROM GoodsReceivedH a ";
                //    Query += "INNER JOIN ReceiptOrderH b ";
                //    Query += "ON b.ReceiptOrderId = a.RefTransID ";
                //    Query += "INNER JOIN PurchH c ";
                //    Query += "ON c.PurchID = b.PurchaseOrderId ";
                //    Query += "WHERE a.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "'";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        TermofPayment = Convert.ToString(Dr["TermofPayment"]);
                //        PaymentMode = Convert.ToString(Dr["PaymentMode"]);
                //        PPN = Convert.ToDecimal(Dr["PPN"]) / 100;
                //        PPH = Convert.ToDecimal(Dr["PPH"]) / 100;
                //        ExchRate = Convert.ToDecimal(Dr["ExchRate"]);
                //        InventSiteID = Convert.ToString(Dr["SiteID"]);
                //        InventSiteName = Convert.ToString(Dr["SiteName"]);
                //        InventSiteLocation = Convert.ToString(Dr["SiteLocation"]);
                //        VehicleType = Convert.ToString(Dr["VehicleType"]);
                //        VehicleNumber = Convert.ToString(Dr["VehicleNumber"]);
                //        DriverName = Convert.ToString(Dr["DriverName"]);
                //        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                //        SJNumber = Convert.ToString(Dr["SJNumber"]);
                //        SJDate = Convert.ToString(Dr["SJDate"]);
                //    }
                //    Dr.Close();                    

                //    //GR================================================================================================
                   
                //     Jenis = "GR";
                //     Kode = "BBMA";
                //    string NewGRNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                //    GRARefNumber = NewGRNumber;
                //    Query = "Insert into [GoodsReceivedH] ( ";
                //    Query += "GoodsReceivedId, GoodsReceivedDate, GoodsReceivedStatus, RefTransID, RefTransDate, ";
                //    Query += "SJDate, SJNumber, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, ";
                //    Query += "Timbang1Weight, Timbang2Date, Timbang2Weight, SiteID, SiteName, SiteLocation, CreatedDate, ";
                //    Query += "CreatedBy, StatusWeight1, StatusWeight2, RefTransType, NPPId) VALUES( ";
                //    Query += "'" + NewGRNumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'03',";
                //    Query += "'" + RONumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + SJDate + "',";
                //    Query += "'" + SJNumber + "',";
                //    Query += "'" + txtVendorID.Text + "',";
                //    Query += "'" + txtVendorName.Text + "',";
                //    Query += "'" + VehicleType + "',";
                //    Query += "'" + VehicleNumber + "',";
                //    Query += "'" + DriverName + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + TimbanganWeight + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + TimbanganWeight + "',";
                //    Query += "'" + InventSiteID + "',";
                //    Query += "'" + InventSiteName + "',";
                //    Query += "'" + InventSiteLocation + "',";
                //    Query += "getdate(),";
                //    Query += "'" + ControlMgr.UserId + "',";
                //    Query += "'Manual',";
                //    Query += "'Manual',";
                //    Query += "'Receipt Order', ";
                //    Query += "'" + txtNotaNumber.Text + "') ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();

                //    //INSERT WORKFLOW LOG================================================================================                   
                //    Query = "SELECT Deskripsi FROM TransStatusTable ";
                //    Query += "WHERE StatusCode='03' AND TransCode = 'GR' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();

                //    while (Dr.Read())
                //    {
                //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //    }
                //    Dr.Close();

                //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //    Query += "VALUES('GoodsReceivedH', '" + NewGRNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '03', '" + StatusDesc + "', getdate()) ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //ENDINSERT WORKFLOW LOG============================================================================                    
                //    //END GR============================================================================================                   

                    

                //    //GET DATA DETAIL===================================================================================
                //    Query = "SELECT FullItemID, ItemName, Qty, Unit, Price, SeqNo, GoodsReceived_SeqNo ";
                //    Query += "FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '02' ORDER BY SeqNo ASC ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    DrDetail = Cmd.ExecuteReader();

                //    while (DrDetail.Read())
                //    {
                //        FullItemID = Convert.ToString(DrDetail["FullItemID"]);
                //        ItemName = Convert.ToString(DrDetail["ItemName"]);
                //        Qty = Convert.ToString(DrDetail["Qty"]) == null ? 0 : Convert.ToDecimal(DrDetail["Qty"]);
                //        Price = Convert.ToString(DrDetail["Price"]) == null ? 0 : Convert.ToDecimal(DrDetail["Price"]);
                //        Unit = Convert.ToString(DrDetail["Unit"]);
                //        SeqNo = Convert.ToInt32(DrDetail["SeqNo"]);
                //        GoodReceivedSeqNo = Convert.ToInt32(DrDetail["GoodsReceived_SeqNo"]);

                //        SplitFullItemID = FullItemID.Split('.');
                //        GroupID = Convert.ToString(SplitFullItemID[0]);
                //        SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                //        SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                //        ItemID = Convert.ToString(SplitFullItemID[3]);

                //        Query = "SELECT Ratio FROM InventConversion WHERE FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        RatioRDN = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());

                //        Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());

                //        Query = "SELECT P.Price, N.* FROM PurchDtl P INNER JOIN ReceiptOrderD R ON R.PurchaseOrderId = P.PurchID ";
                //        Query += "INNER JOIN GoodsReceivedD G ON G.RefTransID = R.ReceiptOrderId ";
                //        Query += "INNER JOIN NotaPurchaseParkD N ON N.GoodsReceivedID = G.GoodsReceivedId ";
                //        Query += "AND R.PurchaseOrderSeqNo = P.SeqNo AND G.RefTransSeqNo = R.SeqNo ";
                //        Query += "AND N.GoodsReceivedID = G.GoodsReceivedId AND N.GoodsReceived_SeqNo = G.GoodsReceivedSeqNo ";
                //        Query += "AND N.NPPID = '" + txtNotaNumber.Text + "' AND N.GoodsReceived_SeqNo = '" + GoodReceivedSeqNo + "' AND N.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        PricePO = Convert.ToDecimal(Cmd.ExecuteScalar());

                //        if (PricePO == 0)
                //        {
                //            PricePO = 1;
                //        }

                //        if (Unit.ToUpper() == "BTG")
                //        {
                //            Amount = Qty * PricePO;
                //        }
                //        else
                //        {
                //            Amount = (Qty * ConvRatio) * PricePO;
                //        }

                //        //GET UOM, ALT===============================================================================
                //        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //        UoM = Cmd.ExecuteScalar().ToString();
                //        if (Unit == UoM)
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyUoM = Qty;
                //            QtyAlt = Qty * ConvRatio;
                //        }
                //        else
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyAlt = Qty;
                //            QtyUoM = Qty / ConvRatio;
                //        }
                //        //END GET UOM, ALT==============================================================================


                //        SubTotal = PricePO * Qty;
                //        SubTotalPPN = SubTotal * PPN;
                //        SubTotalPPH = SubTotal * PPH;

                //        Query = "INSERT INTO [NotaReturBeli_Dtl] ([NRBId], [SeqNo], [GroupId], [SubGroup1Id], [SubGroup2Id], ";
                //        Query += "[ItemId], [FullItemId], [ItemName], [GoodsReceivedId], [GoodsReceived_SeqNo], [UoM_Qty], ";
                //        Query += "[Alt_Qty], [UoM_Unit], [Alt_Unit], [Ratio], [RemainingQty], [InventSiteId], ";
                //        Query += "[CreatedDate], [CreatedBy], [Price]) VALUES ( ";
                //        Query += "'" + ReturDebitNote + "', '" + SeqNoRDN + "', '" + GroupID + "', '" + SubGroup1ID + "', ";
                //        Query += "'" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                //        Query += "'" + txtGoodsReceivedNumber.Text + "', '" + GoodReceivedSeqNo + "', " + Qty + ", " + (Qty * RatioRDN) + ", ";
                //        Query += "'" + Unit + "', '" + UnitAlt + "', " + RatioRDN + ", " + Qty + " ,'" + SiteID + "', ";
                //        Query += "getdate(), '" + ControlMgr.UserId + "', " + PricePO + "";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        Query = "SELECT TOP 1 Quality ";
                //        Query += "FROM GoodsReceivedD ";
                //        Query += "WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                //        Query += "AND GoodsReceivedSeqNo = '" + GoodReceivedSeqNo + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            Quality = Convert.ToString(Dr["Quality"]);
                //        }
                //        Dr.Close();

                //        Query = "SELECT RefTransSeqNo FROM GoodsReceivedD WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GoodsReceivedSeqNo = " + GoodReceivedSeqNo + " ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        string RefTransSeqNo = Convert.ToString(Cmd.ExecuteScalar());

                //        Query = "INSERT INTO GoodsReceivedD ( ";
                //        Query += "GoodsReceivedDate, GoodsReceivedId, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, ";
                //        Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_SJ, Qty_Actual, ";
                //        Query += "Unit, Ratio, Ratio_Actual, InventSiteId, InventSiteBlokID, Quality, ActionCodeStatus, ";
                //        Query += "CreatedDate, CreatedBy, DeliveryMethod, NPPId, NPP_SeqNo, Price, Total, PPN, Total_PPN, PPH, Total_PPH) ";
                //        Query += "VALUES (";
                //        Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //        Query += "'" + NewGRNumber + "',";
                //        Query += "'" + SeqNoRDN + "',";
                //        Query += "'" + RONumber + "',";
                //        Query += "'" + RefTransSeqNo + "',";
                //        Query += "'" + GroupID + "',";
                //        Query += "'" + SubGroup1ID + "',";
                //        Query += "'" + SubGroup2ID + "',";
                //        Query += "'" + ItemID + "',";
                //        Query += "'" + FullItemID + "',";
                //        Query += "'" + ItemName + "',";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty + ",";
                //        Query += "'" + Unit + "',";
                //        Query += "'" + Ratio + "',";
                //        Query += "'" + Ratio + "',";
                //        Query += "'" + InventSiteId + "',";
                //        Query += "'" + InventSiteBlokID + "',";
                //        Query += "'" + Quality + "',";
                //        Query += "'05',";
                //        Query += "getdate(),";
                //        Query += "'" + ControlMgr.UserId + "',";
                //        Query += "'" + DeliveryMethod + "',";
                //        Query += "'" + txtNotaNumber.Text + "',";
                //        Query += "'" + SeqNo + "',";
                //        Query += "" + PricePO + ",";
                //        Query += "" + SubTotal + ",";
                //        Query += "" + PPN * 100 + ",";
                //        Query += "" + SubTotalPPN + ",";
                //        Query += "" + PPH * 100 + ",";
                //        Query += "" + SubTotalPPH + "";
                //        Query += ")";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END GR==================

                //       // Query = "SELECT RO.SeqNo FROM GoodsReceivedD GR ";
                //       // Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //       // Query += "WHERE GR.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GR.GoodsReceivedSeqNo = " + GoodReceivedSeqNo + " ";
                //       // Cmd = new SqlCommand(Query, Conn, Trans);
                //       // string ROSeqNo = Convert.ToString(Cmd.ExecuteScalar());

                //       // //if (ROSeqNo != "")
                //       //// {
                //       //     Query = "SELECT RO.ReceiptOrderId FROM GoodsReceivedD GR ";
                //       //     Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //       //     Query += "WHERE GR.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GR.GoodsReceivedSeqNo = " + GoodReceivedSeqNo + " ";
                //       //     Cmd = new SqlCommand(Query, Conn, Trans);
                //       //     string ReceiptOrderId = Convert.ToString(Cmd.ExecuteScalar());

                //       //     Query = "SELECT RO.RemainingQty FROM GoodsReceivedD GR ";
                //       //     Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //       //     Query += "WHERE GR.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GR.GoodsReceivedSeqNo = " + GoodReceivedSeqNo + " ";
                //       //     Cmd = new SqlCommand(Query, Conn, Trans);
                //       //     decimal RemainingQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                //       //     if (RemainingQty - Qty >= 0)
                //       //     {
                //       //         Query = "UPDATE ReceiptOrderD SET RemainingQty = RemainingQty - " + Qty + " WHERE ReceiptOrderId = '" + ReceiptOrderId + "' AND SeqNo = " + ROSeqNo + " ";
                //       //         Cmd = new SqlCommand(Query, Conn, Trans);
                //       //         Cmd.ExecuteNonQuery();
                //       //     }
                //       //     else
                //       //     {
                //       //         MessageBox.Show("Total Qty (" + Qty + ") pada FullItemID " + FullItemID + " melebihi Remaining Qty (" + RemainingQty + ") pada ReceiptOrder");
                //       //         return;
                //       //     }

                //       // }

                //        //SYSTEM UPDATE=================================================================================
                       

                //        Query = "UPDATE Invent_Purchase_Qty SET Retur_Beli_In_Progress_Alt = (Retur_Beli_In_Progress_Alt + " + QtyAlt + "), ";
                //        Query += "Retur_Beli_In_Progress_UoM = (Retur_Beli_In_Progress_UoM + " + QtyUoM + "), ";
                //        Query += "Retur_Beli_In_Progress_Amount = (Retur_Beli_In_Progress_Amount +  " + Amount + ")";
                //        Query += "WHERE FullItemId = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        Query = "Update Invent_Movement_Qty SET Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM- " + QtyUoM + "), ";
                //        Query += "Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt- " + QtyAlt + "), ";
                //        Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount-" + Amount + ") ";
                //        Query += "where FullItemID='" + FullItemID + "' ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END SYSTEM UPDATE=============================================================================


                //        //SYSTEM INSERT=================================================================================
                //        Query = "INSERT INTO NotaPurchasePark_LogTable ([NPPId], [NPPDate], [RefTransId], [RefTransDate],[RefTrans2Id], [RefTrans2Date], ";
                //        Query += "[AccountId], [AccountName], [InventSiteID], [Qty_UoM], ";
                //        Query += "[Qty_Alt], [Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES ( ";
                //        Query += "'" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', ";
                //        Query += "'" + ReturDebitNote + "', getdate(), '" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceivedDateRDN + "', '" + txtVendorID.Text + "', '" + txtVendorName.Text + "', ";
                //        Query += "'" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '02', ";
                //        Query += "'Approved by " + ControlMgr.GroupName + "', 'Status: 02. Approved by " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate()";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        Query = "INSERT INTO [NotaReturBeli_LogTable] ([NRBDate], [NRBId], [GoodsReceivedDate], [GoodsReceivedId], ";
                //        Query += "[VendId], [SiteId], [FullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], [Amount], ";
                //        Query += "[LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                //        Query += "getdate(), '" + ReturDebitNote + "', '" + GoodsReceivedDateRDN + "', '" + txtGoodsReceivedNumber.Text + "', ";
                //        Query += "'" + txtVendorID.Text + "', '" + SiteID + "', '" + FullItemID + "', '" + SeqNoRDN + "', ";
                //        Query += "" + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '03', 'Auto - Approved by " + ControlMgr.GroupName + "', ";
                //        Query += "'Status: 03. Auto – Approved by " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate()";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END SYSTEM INSERT=================================================================================
                      
                //        SeqNoRDN++;
                //    }
                //    DrDetail.Close();

                //    //GET DATA DETAIL===================================================================================
                //}
                ////End Get Count Data Retur Debit==================================================================
                ////END RETUR DEBIT=================================================================================


                ////RETUR KEMBALI BARANG====================================================================================
                ////Get Count Data Retur Kembali Barang=====================================================================
                //Query = "SELECT COUNT(FullItemID) CountData FROM NotaPurchaseParkD ";
                //Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '03'";
                //Cmd = new SqlCommand(Query, Conn, Trans);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    CountData = Convert.ToInt32(Dr["CountData"]);
                //}
                //Dr.Close();

                //if (CountData > 0)
                //{
                //    string Jenis = "NRB", Kode = "NRBA";
                //    string ReturKembaliBarang = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                //    ReturKBRefNumber = ReturKembaliBarang;
                //    string PurchID = "", SiteID = "", SiteName = "", UnitAlt = "";
                //    int SeqNoRKB = 1;
                //    decimal RatioRKB = 0, Amount = 0, PricePO = 0;
                //    string GoodsReceivedDateRKB = "";

                //    Query = "SELECT R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, G.GoodsReceivedDate ";
                //    Query += "FROM ReceiptOrderH R INNER JOIN GoodsReceivedH G ";
                //    Query += "ON G.RefTransID = R.ReceiptOrderId WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                //        SiteID = Convert.ToString(Dr["InventSiteID"]);
                //        SiteName = Convert.ToString(Dr["InventSiteName"]);
                //        GoodsReceivedDateRKB = Convert.ToString(Dr["GoodsReceivedDate"]);
                //    }
                //    Dr.Close();

                //    Query = "INSERT INTO [NotaReturBeliH] ([NRBId], [NRBDate], [NRBMode], [VendId], [VendName], [GoodsReceivedId], [GoodsReceivedDate], ";
                //    Query += "[PurchId], [SiteId], [SiteName], [TransStatusId], [ActionCode], [CreatedDate], [CreatedBy]) VALUES";
                //    Query += "('" + ReturKembaliBarang + "', getdate(), 'AUTO', '" + txtVendorID.Text + "', ";
                //    Query += "'" + txtVendorName.Text + "', '" + txtGoodsReceivedNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', ";
                //    Query += "'" + PurchID + "', '" + SiteID + "', '" + SiteName + "', '03', '03', getdate(), '" + ControlMgr.UserId + "') ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();

                //    //GET DATA DETAIL===================================================================================
                //    Query = "SELECT FullItemID, ItemName, Qty, Unit, Price, SeqNo, GoodsReceived_SeqNo ";
                //    Query += "FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '03' ORDER BY SeqNo ASC ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    DrDetail = Cmd.ExecuteReader();

                //    while (DrDetail.Read())
                //    {
                //        FullItemID = Convert.ToString(DrDetail["FullItemID"]);
                //        ItemName = Convert.ToString(DrDetail["ItemName"]);
                //        Qty = Convert.ToString(DrDetail["Qty"]) == null ? 0 : Convert.ToDecimal(DrDetail["Qty"]);
                //        Unit = Convert.ToString(DrDetail["Unit"]);
                //        Price = Convert.ToString(DrDetail["Price"]) == null ? 0 : Convert.ToDecimal(DrDetail["Price"]);
                //        SeqNo = Convert.ToInt32(DrDetail["SeqNo"]);
                //        GoodReceivedSeqNo = Convert.ToInt32(DrDetail["GoodsReceived_SeqNo"]);

                //        SplitFullItemID = FullItemID.Split('.');
                //        GroupID = Convert.ToString(SplitFullItemID[0]);
                //        SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                //        SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                //        ItemID = Convert.ToString(SplitFullItemID[3]);

                //        Query = "SELECT Ratio FROM InventConversion WHERE FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        RatioRKB = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());

                //        Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());

                //        Query = "SELECT P.Price, N.* FROM PurchDtl P INNER JOIN ReceiptOrderD R ON R.PurchaseOrderId = P.PurchID ";
                //        Query += "INNER JOIN GoodsReceivedD G ON G.RefTransID = R.ReceiptOrderId ";
                //        Query += "INNER JOIN NotaPurchaseParkD N ON N.GoodsReceivedID = G.GoodsReceivedId ";
                //        Query += "AND R.PurchaseOrderSeqNo = P.SeqNo AND G.RefTransSeqNo = R.SeqNo ";
                //        Query += "AND N.GoodsReceivedID = G.GoodsReceivedId AND N.GoodsReceived_SeqNo = G.GoodsReceivedSeqNo ";
                //        Query += "AND N.NPPID = '" + txtNotaNumber.Text + "' AND N.GoodsReceived_SeqNo = '" + GoodReceivedSeqNo + "' AND N.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        PricePO = Convert.ToDecimal(Cmd.ExecuteScalar());

                //        if (PricePO == 0)
                //        {
                //            PricePO = 1;
                //        }

                //        if (Unit.ToUpper() == "BTG")
                //        {
                //            Amount = Qty * PricePO;
                //        }
                //        else
                //        {
                //            Amount = (Qty * ConvRatio) * PricePO;
                //        }
                //        //GET UOM, ALT===============================================================================
                //        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //        UoM = Cmd.ExecuteScalar().ToString();
                //        if (Unit == UoM)
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyUoM = Qty;
                //            QtyAlt = Qty * ConvRatio;
                //        }
                //        else
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyAlt = Qty;
                //            QtyUoM = Qty / ConvRatio;
                //        }
                //        //END GET UOM, ALT==============================================================================



                //        Query = "INSERT INTO [NotaReturBeli_Dtl] ([NRBId], [SeqNo], [GroupId], [SubGroup1Id], [SubGroup2Id], ";
                //        Query += "[ItemId], [FullItemId], [ItemName], [GoodsReceivedId], [GoodsReceived_SeqNo], [UoM_Qty], ";
                //        Query += "[Alt_Qty], [UoM_Unit], [Alt_Unit], [Ratio], [RemainingQty], [InventSiteId], ";
                //        Query += "[CreatedDate], [CreatedBy], [Price]) VALUES ( ";
                //        Query += "'" + ReturKembaliBarang + "', '" + SeqNoRKB + "', '" + GroupID + "', '" + SubGroup1ID + "', ";
                //        Query += "'" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                //        Query += "'" + txtGoodsReceivedNumber.Text + "', '" + GoodReceivedSeqNo + "', " + Qty + ", " + (Qty * RatioRKB) + ", ";
                //        Query += "'" + Unit + "', '" + UnitAlt + "', " + RatioRKB + ", " + Qty + " ,'" + SiteID + "', ";
                //        Query += "getdate(), '" + ControlMgr.UserId + "', " + PricePO + "";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        //Query = "SELECT PO.SeqNo FROM GoodsReceivedD GR ";
                //        //Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //        //Query += "INNER JOIN PurchDtl PO ON PO.PurchID = RO.PurchaseOrderId AND PO.SeqNo = RO.PurchaseOrderSeqNo ";
                //        //Query += "WHERE GR.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GR.GoodsReceivedSeqNo = " + GoodReceivedSeqNo +" ";
                //        //Cmd = new SqlCommand(Query, Conn, Trans);
                //        //string POSeqNo = Convert.ToString(Cmd.ExecuteScalar());

                //        //if (POSeqNo != "")
                //        //{
                //        //    Query = "SELECT PO.PurchID FROM GoodsReceivedD GR ";
                //        //    Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID AND RO.SeqNo = GR.RefTransSeqNo ";
                //        //    Query += "INNER JOIN PurchDtl PO ON PO.PurchID = RO.PurchaseOrderId AND PO.SeqNo = RO.PurchaseOrderSeqNo ";
                //        //    Query += "WHERE GR.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GR.GoodsReceivedSeqNo = " + GoodReceivedSeqNo + " ";
                //        //    Cmd = new SqlCommand(Query, Conn, Trans);
                //        //    string PurchID = Convert.ToString(Cmd.ExecuteScalar());

                //        //    Query = "UPDATE PurchDtl SET ";
                //        //    Cmd = new SqlCommand(Query, Conn, Trans);
                //        //    Cmd.ExecuteNonQuery();

                //        //}

                //        //SYSTEM UPDATE=================================================================================
                        

                //        Query = "UPDATE Invent_Purchase_Qty SET Retur_Beli_In_Progress_Alt = (Retur_Beli_In_Progress_Alt + " + QtyAlt + "), ";
                //        Query += "Retur_Beli_In_Progress_UoM = (Retur_Beli_In_Progress_UoM + " + QtyUoM + "), ";
                //        Query += "Retur_Beli_In_Progress_Amount = (Retur_Beli_In_Progress_Amount +  " + Amount + ")";
                //        Query += "WHERE FullItemId = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        Query = "Update Invent_Movement_Qty SET Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM- " + QtyUoM + "), ";
                //        Query += "Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt- " + QtyAlt + "), ";
                //        Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount-" + Amount + ") ";
                //        Query += "where FullItemID='" + FullItemID + "' ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END SYSTEM UPDATE=============================================================================

                //        //SYSTEM INSERT=================================================================================
                //        Query = "INSERT INTO NotaPurchasePark_LogTable ([NPPId], [NPPDate], [RefTransId], [RefTransDate], [RefTrans2Id], [RefTrans2Date], ";
                //        Query += "[AccountId], [AccountName], [InventSiteID], [Qty_UoM], ";
                //        Query += "[Qty_Alt], [Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES ( ";
                //        Query += "'" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', ";
                //        Query += "'" + ReturKembaliBarang + "', getdate(), '" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceivedDateRKB + "', '" + txtVendorID.Text + "', '" + txtVendorName.Text + "', ";
                //        Query += "'" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '03', ";
                //        Query += "'Approved by " + ControlMgr.GroupName + "', 'Status: 03. Approved by " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate()";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();

                //        Query = "INSERT INTO [NotaReturBeli_LogTable] ([NRBDate], [NRBId], [GoodsReceivedDate], [GoodsReceivedId], ";
                //        Query += "[VendId], [SiteId], [FullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], [Amount], ";
                //        Query += "[LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                //        Query += "getdate(), '" + ReturKembaliBarang + "', '" + GoodsReceivedDateRKB + "', '" + txtGoodsReceivedNumber.Text + "', ";
                //        Query += "'" + txtVendorID.Text + "', '" + SiteID + "', '" + FullItemID + "', '" + SeqNoRKB + "', ";
                //        Query += "" + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '03', 'Auto - Approved by " + ControlMgr.GroupName + "', ";
                //        Query += "'Status: 03. Auto – Approved by " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate()";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END SYSTEM INSERT=================================================================================

                //        SeqNoRKB++;
                //    }
                //    DrDetail.Close();

                //    //GET DATA DETAIL===================================================================================

                //}
                ////End Get Count Data Retur Kembali Barang==================================================================
                ////END RETUR KEMBALI BARANG=================================================================================


                ////BARANG BONUS====================================================================================
                ////Get Count Data Barang Bonus=====================================================================
                //Query = "SELECT COUNT(FullItemID) CountData FROM NotaPurchaseParkD ";
                //Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '05'";
                //Cmd = new SqlCommand(Query, Conn, Trans);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    CountData = Convert.ToInt32(Dr["CountData"]);
                //}
                //Dr.Close();

                //if (CountData > 0)
                //{
                //    string Jenis = "NRZ", Kode = "NRZA";
                //    string BarangBonus = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                //    BarangBonusRefNumber = BarangBonus;
                //    //string PurchID = "", SiteID = "", SiteName = "", UnitAlt = "";
                //    int SeqNoBB = 1;
                //    //decimal RatioRKB = 0, Amount = 0, PricePO = 0;
                //    //string GoodsReceivedDateRKB = "";                   

                //    //GET DATA DETAIL===================================================================================
                //    Query = "SELECT FullItemID, ItemName, Qty, Unit, Price, SeqNo, GoodsReceived_SeqNo ";
                //    Query += "FROM NotaPurchaseParkD WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '05' ORDER BY SeqNo ASC ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    DrDetail = Cmd.ExecuteReader();

                //    SeqNoGroup = 1;
                //    while (DrDetail.Read())
                //    {
                //        FullItemID = Convert.ToString(DrDetail["FullItemID"]);
                //        ItemName = Convert.ToString(DrDetail["ItemName"]);
                //        Qty = Convert.ToString(DrDetail["Qty"]) == null ? 0 : Convert.ToDecimal(DrDetail["Qty"]);
                //        Unit = Convert.ToString(DrDetail["Unit"]);
                //        //Price = Convert.ToString(DrDetail["Price"]) == null ? 0 : Convert.ToDecimal(DrDetail["Price"]);
                //        //SeqNo = Convert.ToInt32(DrDetail["SeqNo"]);
                //        //GoodReceivedSeqNo = Convert.ToInt32(DrDetail["GoodsReceived_SeqNo"]);
                //        string GoodsReceivedDateBB = "";
                //        SubTotal = 0;
                //        decimal Amount = 0;
                //        string SiteID = "";

                //        SplitFullItemID = FullItemID.Split('.');
                //        GroupID = Convert.ToString(SplitFullItemID[0]);
                //        SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                //        SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                //        ItemID = Convert.ToString(SplitFullItemID[3]);

                //        //Query = "SELECT TOP 1 Resize, ResizeType ";
                //        //Query += "FROM InventTable ";
                //        //Query += "WHERE FullItemID = '" + FullItemID + "' ";
                //        //Cmd = new SqlCommand(Query, Conn, Trans);
                //        //Dr = Cmd.ExecuteReader();
                //        //while (Dr.Read())
                //        //{
                //        //    Resize = Convert.ToString(Dr["Resize"]);
                //        //    ResizeType = Convert.ToString(Dr["ResizeType"]);
                //        //}
                //        //Dr.Close();

                //        //if (Resize.ToUpper() == "TRUE")
                //        //{
                //        //    //ActionStatusCodeDetail = "04";
                //        //    ResizeLoop = 0;

                //        //    Query = "SELECT a.SiteID, a.SiteName FROM GoodsReceivedH a ";
                //        //    Query += "INNER JOIN ReceiptOrderH b ";
                //        //    Query += "ON b.ReceiptOrderId = a.RefTransID ";
                //        //    Query += "INNER JOIN PurchH c ";
                //        //    Query += "ON c.PurchID = b.PurchaseOrderId ";
                //        //    Query += "WHERE a.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "'";
                //        //    Cmd = new SqlCommand(Query, Conn, Trans);
                //        //    Dr = Cmd.ExecuteReader();
                //        //    while (Dr.Read())
                //        //    {
                //        //        InventSiteID = Convert.ToString(Dr["SiteID"]);
                //        //        InventSiteName = Convert.ToString(Dr["SiteName"]);
                //        //    }
                //        //    Dr.Close();


                //        //    if (ResizeLoop == 1)
                //        //    {
                //        //        Query = "INSERT INTO NotaResizeH ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                //        //        Query += "[SiteID], [VendID], [Posted], [CreatedDate], [CreatedBy]) VALUES ( ";
                //        //        Query += "'" + BarangBonus + "',";
                //        //        Query += "getdate(),";
                //        //        Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //        //        Query += "'" + txtNotaNumber.Text + "',";
                //        //        Query += "'" + InventSiteID + "',";
                //        //        Query += "'" + txtVendorID.Text + "',";
                //        //        Query += "1,";
                //        //        Query += "getdate(),";
                //        //        Query += "'" + ControlMgr.UserId + "'";
                //        //        Query += ")";
                //        //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        //        Cmd.ExecuteNonQuery();

                //        //        ResizeLoop++;
                //        //    }

                //        //    Query = "SELECT COUNT(*) FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'";
                //        //    Cmd = new SqlCommand(Query, Conn, Trans);
                //        //    CountResize = Convert.ToInt32(Cmd.ExecuteScalar().ToString());

                //        //    if (CountResize > 1)
                //        //    {
                //        //        Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], ";
                //        //        Query += "[Qty], [Price], [Unit], [LineAmount], [GoodsReceivedId], ";
                //        //        Query += "[GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                //        //        Query += "'" + BarangBonus + "',";
                //        //        Query += "(SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + BarangBonus + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + BarangBonus + "') + 1  END AS SeqNo),";
                //        //        Query += "'" + FullItemID + "',";
                //        //        Query += "'" + ItemName + "',";
                //        //        Query += "'" + Qty + "',";
                //        //        Query += "" + 0 + ",";
                //        //        Query += "'" + Unit + "',";
                //        //        Query += "'" + 0 + "',";
                //        //        Query += "'" + txtNotaNumber.Text + "',";
                //        //        Query += "'" + SeqNoGroup + "',";
                //        //        Query += "getdate(),";
                //        //        Query += "'" + ControlMgr.UserId + "'";
                //        //        Query += ")";
                //        //    }
                //        //    else
                //        //    {
                //        //        Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], [ToFullItemId], [ToItemName], ";
                //        //        Query += "[Qty], [Price], [Unit], [LineAmount], [GoodsReceivedId], ";
                //        //        Query += "[GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                //        //        Query += "'" + BarangBonus + "',";
                //        //        Query += "(SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + BarangBonus + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + BarangBonus + "') + 1  END AS SeqNo),";
                //        //        Query += "'" + FullItemID + "',";
                //        //        Query += "'" + ItemName + "',";
                //        //        Query += "(SELECT To_FullItemId FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'),";
                //        //        Query += "(SELECT To_ItemName FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'),";
                //        //        Query += "'" + Qty + "',";
                //        //        Query += "" + 0 + ",";
                //        //        Query += "'" + Unit + "',";
                //        //        Query += "'" + 0 + "',";
                //        //        Query += "'" + txtNotaNumber.Text + "',";
                //        //        Query += "'" + SeqNoGroup + "',";
                //        //        Query += "getdate(),";
                //        //        Query += "'" + ControlMgr.UserId + "'";
                //        //        Query += ")";
                //        //    }
                //        //    Cmd = new SqlCommand(Query, Conn, Trans);
                //        //    Cmd.ExecuteNonQuery();

                //        //    SeqNoGroup++;
                //        //}
                //        //else
                //        //{
                //        //    //GET UOM, ALT===============================================================================
                //        //    QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                //        //    Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //        //    UoM = Cmd.ExecuteScalar().ToString();
                //        //    if (Unit == UoM)
                //        //    {
                //        //        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //        //        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //        //        ConvRatio = (decimal)Cmd.ExecuteScalar();

                //        //        QtyUoM = Qty;
                //        //        QtyAlt = Qty * ConvRatio;
                //        //    }
                //        //    else
                //        //    {
                //        //        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //        //        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //        //        ConvRatio = (decimal)Cmd.ExecuteScalar();

                //        //        QtyAlt = Qty;
                //        //        QtyUoM = Qty / ConvRatio;
                //        //    }
                //        //    //END GET UOM, ALT==============================================================================


                //        //    Query = "Update Invent_OnHand_Qty SET Available_For_Sale_UoM = (Available_For_Sale_UoM+" + QtyUoM + "), ";
                //        //    Query += "Available_For_Sale_Alt = (Available_For_Sale_Alt+" + QtyAlt + "), Available_For_Sale_Amount = (Available_For_Sale_Amount+ " + Amount + ") ";
                //        //    Query += "where FullItemID='" + FullItemID + "' ";
                //        //    Cmd = new SqlCommand(Query, Conn, Trans);
                //        //    Cmd.ExecuteNonQuery();

                //        //    Query = "INSERT INTO InventTrans ([GroupId], [SubGroupId], [SubGroup2Id], [ItemId], [FullItemId], ";
                //        //    Query += "[ItemName], [InventSiteId], [TransId], [SeqNo], [TransDate], [Ref_TransId], [Ref_TransDate], ";
                //        //    Query += "[Ref_Trans_SeqNo], [AccountId], [AccountName], [Available_UoM], [Available_Alt], ";
                //        //    Query += "[Available_Amount], [Available_For_Sale_UoM], [Available_For_Sale_Alt], [Available_For_Sale_Amount]) VALUES ( ";
                //        //    Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', ";
                //        //    Query += "'" + ItemName + "', '" + InventSiteID + "', '" + txtNotaNumber.Text + "', " + SeqNoGroup + ", getdate(), ";
                //        //    Query += "'" + txtGoodsReceivedNumber.Text + "', getdate(), '" + SeqNoGroup + "', '" + txtVendorID.Text + "', '" + txtVendorName.Text + "', ";
                //        //    Query += "" + QtyUoM + ", " + QtyAlt + ", " + Amount + "," + QtyUoM + ", " + QtyAlt + ", " + Amount + " ";
                //        //    Query += ")";
                //        //    Cmd = new SqlCommand(Query, Conn, Trans);
                //        //    Cmd.ExecuteNonQuery();
                //        //}

                //        Query = "SELECT R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, G.GoodsReceivedDate ";
                //        Query += "FROM ReceiptOrderH R INNER JOIN GoodsReceivedH G ";
                //        Query += "ON G.RefTransID = R.ReceiptOrderId WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            //PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                //            SiteID = Convert.ToString(Dr["InventSiteID"]);
                //            //SiteName = Convert.ToString(Dr["InventSiteName"]);
                //            GoodsReceivedDateBB = Convert.ToString(Dr["GoodsReceivedDate"]);
                //        }
                //        Dr.Close();
                       
                //        //SYSTEM INSERT=================================================================================
                //        Query = "INSERT INTO NotaPurchasePark_LogTable ([NPPId], [NPPDate], [RefTransId], [RefTransDate], [RefTrans2Id], [RefTrans2Date], ";
                //        Query += "[AccountId], [AccountName], [InventSiteID], [Qty_UoM], ";
                //        Query += "[Qty_Alt], [Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES ( ";
                //        Query += "'" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', ";
                //        Query += "'" + BarangBonus + "', getdate(), '" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceivedDateBB + "', '" + txtVendorID.Text + "', '" + txtVendorName.Text + "', ";
                //        Query += "'" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + Amount + ", '05', ";
                //        Query += "'Approved by " + ControlMgr.GroupName + "', 'Status: 05. Approved by " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate()";
                //        Query += ") ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END SYSTEM INSERT=================================================================================

                //        SeqNoBB++;
                //    }
                //    DrDetail.Close();

                //    //GET DATA DETAIL===================================================================================

                //}
                ////End Get Count Data Barang Bonus==================================================================
                ////END BARANG BONUS=================================================================================


                ////RECEIVED WITH PO====================================================================================
                ////Get Count Data RECEIVED WITH PO=====================================================================
                //Query = "SELECT COUNT(FullItemID) CountData FROM NotaPurchaseParkD ";
                //Query += "WHERE NPPID = '" + txtNotaNumber.Text.Trim() + "' AND ActionCode = '06'";
                //Cmd = new SqlCommand(Query, Conn, Trans);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    CountData = Convert.ToInt32(Dr["CountData"]);
                //}
                //Dr.Close();

                //if (CountData > 0)
                //{
                //    int SeqNoReceived = 1;

                //    Query = "SELECT c.ExchRate,c.TermofPayment, ";
                //    Query += "c.PaymentMode, c.PPN, c.PPH, a.SiteID, a.SiteName, ";
                //    Query += "a.SiteLocation, a.VehicleType, a.VehicleNumber, a.DriverName, ";
                //    Query += "a.GoodsReceivedDate, a.SJNumber, a.SJDate FROM GoodsReceivedH a ";
                //    Query += "INNER JOIN ReceiptOrderH b ";
                //    Query += "ON b.ReceiptOrderId = a.RefTransID ";
                //    Query += "INNER JOIN PurchH c ";
                //    Query += "ON c.PurchID = b.PurchaseOrderId ";
                //    Query += "WHERE a.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "'";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        TermofPayment = Convert.ToString(Dr["TermofPayment"]);
                //        PaymentMode = Convert.ToString(Dr["PaymentMode"]);
                //        PPN = Convert.ToDecimal(Dr["PPN"]) / 100;
                //        PPH = Convert.ToDecimal(Dr["PPH"]) / 100;
                //        ExchRate = Convert.ToDecimal(Dr["ExchRate"]);
                //        InventSiteID = Convert.ToString(Dr["SiteID"]);
                //        InventSiteName = Convert.ToString(Dr["SiteName"]);
                //        InventSiteLocation = Convert.ToString(Dr["SiteLocation"]);
                //        VehicleType = Convert.ToString(Dr["VehicleType"]);
                //        VehicleNumber = Convert.ToString(Dr["VehicleNumber"]);
                //        DriverName = Convert.ToString(Dr["DriverName"]);
                //        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                //        SJNumber = Convert.ToString(Dr["SJNumber"]);
                //        SJDate = Convert.ToString(Dr["SJDate"]);
                //    }
                //    Dr.Close();

                //    //GR================================================================================================



                //    string RONumber = "";
                //    Query = "SELECT R.ReceiptOrderId ";
                //    Query += "FROM ReceiptOrderH R INNER JOIN GoodsReceivedH G ";
                //    Query += "ON G.RefTransID = R.ReceiptOrderId WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        RONumber = Convert.ToString(Dr["ReceiptOrderId"]);
                //    }
                //    Dr.Close();

                //    string Jenis = "GR";
                //    string Kode = "BBMA";
                //    string NewGRNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                //    GRARefNumber = NewGRNumber;
                //    Query = "Insert into [GoodsReceivedH] ( ";
                //    Query += "GoodsReceivedId, GoodsReceivedDate, GoodsReceivedStatus, RefTransID, RefTransDate, ";
                //    Query += "SJDate, SJNumber, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, ";
                //    Query += "Timbang1Weight, Timbang2Date, Timbang2Weight, SiteID, SiteName, SiteLocation, CreatedDate, ";
                //    Query += "CreatedBy, StatusWeight1, StatusWeight2, RefTransType, NPPId) VALUES( ";
                //    Query += "'" + NewGRNumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'03',";
                //    Query += "'" + RONumber + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + SJDate + "',";
                //    Query += "'" + SJNumber + "',";
                //    Query += "'" + txtVendorID.Text + "',";
                //    Query += "'" + txtVendorName.Text + "',";
                //    Query += "'" + VehicleType + "',";
                //    Query += "'" + VehicleNumber + "',";
                //    Query += "'" + DriverName + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + TimbanganWeight + "',";
                //    Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //    Query += "'" + TimbanganWeight + "',";
                //    Query += "'" + InventSiteID + "',";
                //    Query += "'" + InventSiteName + "',";
                //    Query += "'" + InventSiteLocation + "',";
                //    Query += "getdate(),";
                //    Query += "'" + ControlMgr.UserId + "',";
                //    Query += "'Manual',";
                //    Query += "'Manual',";
                //    Query += "'Receipt Order', ";
                //    Query += "'" + txtNotaNumber.Text + "') ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();

                //    //INSERT WORKFLOW LOG================================================================================                   
                //    Query = "SELECT Deskripsi FROM TransStatusTable ";
                //    Query += "WHERE StatusCode='03' AND TransCode = 'GR' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Dr = Cmd.ExecuteReader();

                //    while (Dr.Read())
                //    {
                //        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //    }
                //    Dr.Close();

                //    Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //    Query += "VALUES('GoodsReceivedH', '" + NewGRNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '03', '" + StatusDesc + "', getdate()) ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //    //ENDINSERT WORKFLOW LOG============================================================================                    
                //    //END GR============================================================================================                   



                //    //GET DATA DETAIL===================================================================================
                //    Query = "SELECT FullItemID, ItemName, Qty, Unit, Price, GoodsReceivedSeqNo AS GoodsReceived_SeqNo, RefTransID AS ReceiptOrderId ";
                //    Query += "FROM GoodsReceivedD WHERE GoodsReceivedId = '" + NewGRNumber + "' ";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    DrDetail = Cmd.ExecuteReader();

                //    while (DrDetail.Read())
                //    {
                //        FullItemID = Convert.ToString(DrDetail["FullItemID"]);
                //        ItemName = Convert.ToString(DrDetail["ItemName"]);
                //        Qty = Convert.ToString(DrDetail["Qty"]) == null ? 0 : Convert.ToDecimal(DrDetail["Qty"]);
                //        Price = Convert.ToString(DrDetail["Price"]) == null ? 0 : Convert.ToDecimal(DrDetail["Price"]);
                //        Unit = Convert.ToString(DrDetail["Unit"]);
                //        //SeqNo = Convert.ToInt32(DrDetail["SeqNo"]);
                //        GoodReceivedSeqNo = Convert.ToInt32(DrDetail["GoodsReceived_SeqNo"]);

                //        SplitFullItemID = FullItemID.Split('.');
                //        GroupID = Convert.ToString(SplitFullItemID[0]);
                //        SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                //        SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                //        ItemID = Convert.ToString(SplitFullItemID[3]);

                //        Query = "SELECT Ratio FROM InventConversion WHERE FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        decimal RatioRDN = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());

                //        Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        string UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());

                //        Query = "SELECT P.Price, N.* FROM PurchDtl P INNER JOIN ReceiptOrderD R ON R.PurchaseOrderId = P.PurchID ";
                //        Query += "INNER JOIN GoodsReceivedD G ON G.RefTransID = R.ReceiptOrderId ";
                //        Query += "INNER JOIN NotaPurchaseParkD N ON N.GoodsReceivedID = G.GoodsReceivedId ";
                //        Query += "AND R.PurchaseOrderSeqNo = P.SeqNo AND G.RefTransSeqNo = R.SeqNo ";
                //        Query += "AND N.GoodsReceivedID = G.GoodsReceivedId AND N.GoodsReceived_SeqNo = G.GoodsReceivedSeqNo ";
                //        Query += "AND N.NPPID = '" + txtNotaNumber.Text + "' AND N.GoodsReceived_SeqNo = '" + GoodReceivedSeqNo + "' AND N.GoodsReceivedID = '" + txtGoodsReceivedNumber.Text + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        decimal PricePO = Convert.ToDecimal(Cmd.ExecuteScalar());
                //        decimal Amount = 0;

                //        if (PricePO == 0)
                //        {
                //            PricePO = 1;
                //        }

                //        if (Unit.ToUpper() == "BTG")
                //        {
                //            Amount = Qty * PricePO;
                //        }
                //        else
                //        {
                //            Amount = (Qty * ConvRatio) * PricePO;
                //        }

                //        //GET UOM, ALT===============================================================================
                //        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                //        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //        UoM = Cmd.ExecuteScalar().ToString();
                //        if (Unit == UoM)
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyUoM = Qty;
                //            QtyAlt = Qty * ConvRatio;
                //        }
                //        else
                //        {
                //            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                //            ConvRatio = (decimal)Cmd.ExecuteScalar();

                //            QtyAlt = Qty;
                //            QtyUoM = Qty / ConvRatio;
                //        }
                //        //END GET UOM, ALT==============================================================================


                //        SubTotal = PricePO * Qty;
                //        SubTotalPPN = SubTotal * PPN;
                //        SubTotalPPH = SubTotal * PPH;
                        

                //        Query = "SELECT TOP 1 Quality ";
                //        Query += "FROM GoodsReceivedD ";
                //        Query += "WHERE GoodsReceivedId = '" + NewGRNumber + "' ";
                //        Query += "AND GoodsReceivedSeqNo = '" + GoodReceivedSeqNo + "'";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            Quality = Convert.ToString(Dr["Quality"]);
                //        }
                //        Dr.Close();


                //        //INSERT INVENT RESIZE==========================================================================
                //        Query = "SELECT TOP 1 Resize, ResizeType ";
                //        Query += "FROM InventTable ";
                //        Query += "WHERE FullItemID = '" + FullItemID + "' ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Dr = Cmd.ExecuteReader();
                //        while (Dr.Read())
                //        {
                //            Resize = Convert.ToString(Dr["Resize"]);
                //            ResizeType = Convert.ToString(Dr["ResizeType"]);
                //        }
                //        Dr.Close();

                //        if (Resize.ToUpper() == "TRUE")
                //        {
                //            ActionStatusCodeDetail = "04";

                //            if (ResizeLoop == 1)
                //            {
                //                Jenis = "NRZ";
                //                Kode = "NRZA";
                //                TransId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                //                Query = "INSERT INTO NotaResizeH ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                //                Query += "[SiteID], [VendID], [Posted], [CreatedDate], [CreatedBy]) VALUES ( ";
                //                Query += "'" + TransId + "',";
                //                Query += "getdate(),";
                //                Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //                Query += "'" + NewGRNumber + "',";
                //                Query += "'" + InventSiteID + "',";
                //                Query += "'" + txtVendorID.Text + "',";
                //                Query += "1,";
                //                Query += "getdate(),";
                //                Query += "'" + ControlMgr.UserId + "'";
                //                Query += ")";
                //                Cmd = new SqlCommand(Query, Conn, Trans);
                //                Cmd.ExecuteNonQuery();

                //                ResizeLoop++;
                //            }                           

                //            Query = "SELECT COUNT(*) FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            CountResize = Convert.ToInt32(Cmd.ExecuteScalar().ToString());

                //            if (CountResize > 1)
                //            {
                //                Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], ";
                //                Query += "[Qty], [Price], [Unit], [LineAmount], [GoodsReceivedId], ";
                //                Query += "[GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                //                Query += "'" + TransId + "',";
                //                Query += "(SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') + 1  END AS SeqNo),";
                //                Query += "'" + FullItemID + "',";
                //                Query += "'" + ItemName + "',";
                //                Query += "'" + Qty + "',";
                //                Query += "" + Price + ",";
                //                Query += "'" + Unit + "',";
                //                Query += "'" + SubTotal + "',";
                //                Query += "'" + NewGRNumber + "',";
                //                Query += "'" + GoodReceivedSeqNo + "',";
                //                Query += "getdate(),";
                //                Query += "'" + ControlMgr.UserId + "'";
                //                Query += ")";
                //            }
                //            else
                //            {
                //                Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], [ToFullItemId], [ToItemName], ";
                //                Query += "[Qty], [Price], [Unit], [LineAmount], [GoodsReceivedId], ";
                //                Query += "[GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                //                Query += "'" + TransId + "',";
                //                Query += "(SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') + 1  END AS SeqNo),";
                //                Query += "'" + FullItemID + "',";
                //                Query += "'" + ItemName + "',";
                //                Query += "(SELECT To_FullItemId FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'),";
                //                Query += "(SELECT To_ItemName FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'),";
                //                Query += "'" + Qty + "',";
                //                Query += "" + Price + ",";
                //                Query += "'" + Unit + "',";
                //                Query += "'" + SubTotal + "',";
                //                Query += "'" + NewGRNumber + "',";
                //                Query += "'" + GoodReceivedSeqNo + "',";
                //                Query += "getdate(),";
                //                Query += "'" + ControlMgr.UserId + "'";
                //                Query += ")";
                //            }
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();


                //            //INSERT WORKFLOW LOG==========================================================================
                //            if (WorkflowLogDetail == 0)
                //            {
                //                Query = "SELECT Deskripsi FROM TransStatusTable ";
                //                Query += "WHERE StatusCode='" + ActionStatusCodeDetail + "' AND TransCode = 'GRD' ";
                //                Cmd = new SqlCommand(Query, Conn, Trans);
                //                Dr = Cmd.ExecuteReader();
                //                while (Dr.Read())
                //                {
                //                    StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //                }
                //                Dr.Close();

                //                Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //                Query += "VALUES('GoodsReceivedD', '" + NewGRNumber + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '" + ActionStatusCodeDetail + "', '" + StatusDesc + "', getdate()) ";
                //                Cmd = new SqlCommand(Query, Conn, Trans);
                //                Cmd.ExecuteNonQuery();
                //                WorkflowLogDetail++;
                //            }
                //            //END INSERT WORKFLOW LOG======================================================================

                //        }
                //        else
                //        {
                //            Query = "Update Invent_OnHand_Qty SET Available_For_Sale_UoM = (Available_For_Sale_UoM+" + QtyUoM + "), ";
                //            Query += "Available_For_Sale_Alt = (Available_For_Sale_Alt+" + QtyAlt + "), Available_For_Sale_Amount = (Available_For_Sale_Amount+ " + Amount + ") ";
                //            Query += "where FullItemID='" + FullItemID + "' ";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();

                //            Query = "INSERT INTO InventTrans ([GroupId], [SubGroupId], [SubGroup2Id], [ItemId], [FullItemId], ";
                //            Query += "[ItemName], [InventSiteId], [TransId], [SeqNo], [TransDate], [Ref_TransId], [Ref_TransDate], ";
                //            Query += "[Ref_Trans_SeqNo], [AccountId], [AccountName], [Available_UoM], [Available_Alt], ";
                //            Query += "[Available_Amount], [Available_For_Sale_UoM], [Available_For_Sale_Alt], [Available_For_Sale_Amount]) VALUES ( ";
                //            Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', ";
                //            Query += "'" + ItemName + "', '" + InventSiteID + "', '" + txtNotaNumber.Text + "', " + SeqNoReceived + ", getdate(), ";
                //            Query += "'" + NewGRNumber + "', getdate(), '" + GoodReceivedSeqNo + "', '" + txtVendorID.Text + "', '" + txtVendorName.Text + "', ";
                //            Query += "" + QtyUoM + ", " + QtyAlt + ", " + Amount + "," + QtyUoM + ", " + QtyAlt + ", " + Amount + " ";
                //            Query += ")";
                //            Cmd = new SqlCommand(Query, Conn, Trans);
                //            Cmd.ExecuteNonQuery();

                //        }


                //        Query = "SELECT RefTransSeqNo FROM GoodsReceivedD WHERE GoodsReceivedId = '" + NewGRNumber + "' AND GoodsReceivedSeqNo = " + GoodReceivedSeqNo + " ";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        string RefTransSeqNo = Convert.ToString(Cmd.ExecuteScalar());

                //        Query = "INSERT INTO GoodsReceivedD ( ";
                //        Query += "GoodsReceivedDate, GoodsReceivedId, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, ";
                //        Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_SJ, Qty_Actual, ";
                //        Query += "Unit, Ratio, Ratio_Actual, InventSiteId, InventSiteBlokID, Quality, ActionCodeStatus, ";
                //        Query += "CreatedDate, CreatedBy, DeliveryMethod, NPPId, NPP_SeqNo, Price, Total, PPN, Total_PPN, PPH, Total_PPH) ";
                //        Query += "VALUES (";
                //        Query += "'" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "',";
                //        Query += "'" + NewGRNumber + "',";
                //        Query += "'" + SeqNoReceived + "',";
                //        Query += "'" + RONumber + "',";
                //        Query += "'" + RefTransSeqNo + "',";
                //        Query += "'" + GroupID + "',";
                //        Query += "'" + SubGroup1ID + "',";
                //        Query += "'" + SubGroup2ID + "',";
                //        Query += "'" + ItemID + "',";
                //        Query += "'" + FullItemID + "',";
                //        Query += "'" + ItemName + "',";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty + ",";
                //        Query += "" + Qty + ",";
                //        Query += "'" + Unit + "',";
                //        Query += "'" + Ratio + "',";
                //        Query += "'" + Ratio + "',";
                //        Query += "'" + InventSiteId + "',";
                //        Query += "'" + InventSiteBlokID + "',";
                //        Query += "'" + Quality + "',";
                //        Query += "'05',";
                //        Query += "getdate(),";
                //        Query += "'" + ControlMgr.UserId + "',";
                //        Query += "'" + DeliveryMethod + "',";
                //        Query += "'" + txtNotaNumber.Text + "',";
                //        Query += "'" + SeqNo + "',";
                //        Query += "" + PricePO + ",";
                //        Query += "" + SubTotal + ",";
                //        Query += "" + PPN * 100 + ",";
                //        Query += "" + SubTotalPPN + ",";
                //        Query += "" + PPH * 100 + ",";
                //        Query += "" + SubTotalPPH + "";
                //        Query += ")";
                //        Cmd = new SqlCommand(Query, Conn, Trans);
                //        Cmd.ExecuteNonQuery();
                //        //END GR==================

                //        //SYSTEM UPDATE=================================================================================


                //        //Query = "UPDATE Invent_Purchase_Qty SET Retur_Beli_In_Progress_Alt = (Retur_Beli_In_Progress_Alt + " + QtyAlt + "), ";
                //        //Query += "Retur_Beli_In_Progress_UoM = (Retur_Beli_In_Progress_UoM + " + QtyUoM + "), ";
                //        //Query += "Retur_Beli_In_Progress_Amount = (Retur_Beli_In_Progress_Amount +  " + Amount + ")";
                //        //Query += "WHERE FullItemId = '" + FullItemID + "'";
                //        //Cmd = new SqlCommand(Query, Conn, Trans);
                //        //Cmd.ExecuteNonQuery();

                //        //Query = "Update Invent_Movement_Qty SET Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM- " + QtyUoM + "), ";
                //        //Query += "Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt- " + QtyAlt + "), ";
                //        //Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount-" + Amount + ") ";
                //        //Query += "where FullItemID='" + FullItemID + "' ";
                //        //Cmd = new SqlCommand(Query, Conn, Trans);
                //        //Cmd.ExecuteNonQuery();
                //        //END SYSTEM UPDATE=============================================================================

                //        SeqNoReceived++;
                //    }
                //    DrDetail.Close();

                //    //GET DATA DETAIL===================================================================================
                //}
                ////End Get Count Data RECEIVED WITH PO==================================================================
                ////END RECEIVED WITH PO=================================================================================


                //UPDATE NOT PURCHASE PARKED============================================================================
                Query = "UPDATE NotaPurchaseParkH SET ApprovalNotes = '" + txtNotes.Text + "', TransCode = '03', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' ";
                Query += "WHERE NPPID = '" + txtNotaNumber.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
                //END UPDATE NOT PURCHASE PARKED========================================================================

                ////INSERT WORKFLOW LOG===================================================================================
                //Query = "SELECT Deskripsi FROM TransStatusTable ";
                //Query += "WHERE StatusCode='03' AND TransCode = 'NotaPurchasePark' ";
                //Cmd = new SqlCommand(Query, Conn, Trans);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                //}
                //Dr.Close();

                //Query = "Insert into WorkflowLogTable (ReffTableName, ReffID, ReffDate, ReffSeqNo, UserID, WorkFlow, LogStatus, StatusDesc, LogDate) ";
                //Query += "VALUES('NotaPurchaseParkedH', '" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', 0, '" + ControlMgr.UserId + "', '" + ControlMgr.GroupName + "', '03', '" + StatusDesc + "', getdate()) ";
                //Cmd = new SqlCommand(Query, Conn, Trans);
                //Cmd.ExecuteNonQuery();
                ////END INSERT WORKFLOW LOG=================================================================================

                //Begin
                //Created By : Joshua
                //Created Date ; 07 Aug 2018
                //Desc : Create Journal
                CreateJournal();
                if (Journal == true)
                {
                    Journal = false;
                    goto Outer;
                }
                //End
             
                TransCode = "03";
                Trans.Commit();
                MessageBox.Show("Data Nota Number : " + txtNotaNumber.Text + " berhasil di approve");

            Outer: ;
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

        private void ReturTukarBarang(SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                string ActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);


                if (ActionCode == "Retur Tukar Barang")
                {
                    //Get Data 
                    string GoodsReceived_SeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["GoodsReceived_SeqNo"].Value);
                    string FullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
                    string ItemName = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value);
                    string Unit = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["Unit"].Value);
                    decimal Price = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
                    decimal Qty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    decimal Ratio = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Ratio"].Value);

                    string[] SplitFullItemID = FullItemID.Split('.');
                    string GroupID = Convert.ToString(SplitFullItemID[0]);
                    string SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                    string SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                    string ItemID = Convert.ToString(SplitFullItemID[3]);

                    Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());

                    string ReceiptOrderId = "", ReceiptOrderDate = "", PurchID = "", SiteID = "", SiteName = "", GoodsReceivedDate = "", PODate = "", VendorID = "", VendorName = "";
                    Query = "SELECT R.ReceiptOrderId, R.ReceiptOrderDate, R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, R.VendId AS VendorID, R.VendorName, ";
                    Query += "G.GoodsReceivedDate, P.OrderDate AS PODate ";
                    Query += "FROM ReceiptOrderH R INNER JOIN PurchH P ON P.PurchID = R.PurchaseOrderId ";
                    Query += "INNER JOIN GoodsReceivedH G ON G.RefTransID = R.ReceiptOrderId ";
                    Query += "WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);
                        ReceiptOrderDate = Convert.ToString(Dr["ReceiptOrderDate"]);
                        PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                        SiteID = Convert.ToString(Dr["InventSiteID"]);
                        SiteName = Convert.ToString(Dr["InventSiteName"]);
                        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                        PODate = Convert.ToString(Dr["PODate"]);
                        VendorID = Convert.ToString(Dr["VendorID"]);
                        VendorName = Convert.ToString(Dr["VendorName"]);
                    }
                    Dr.Close();

                    //Get UOM, ALT===============================================================================
                    Query = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UoM = Cmd.ExecuteScalar().ToString();
                    decimal QtyUoM = 0, QtyAlt = 0;

                    if (Unit == UoM)
                    {     
                        QtyUoM = Qty;
                        QtyAlt = Qty * Ratio;
                    }
                    else
                    {
                        QtyAlt = Qty;
                        QtyUoM = Qty / Ratio;
                    }
                    //End Get UOM, ALT==============================================================================

                    decimal SubTotal = Convert.ToDecimal((Qty * Price).ToString("F4"));
                    //End Get Data

                    //Insert Header Nota Retur Beli  
                    string Jenis = "NRB", Kode = "NRBA";
                    string NRBNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "INSERT INTO [NotaReturBeliH] ([NRBId], [NRBDate], [NRBMode], [VendId], [VendName], [GoodsReceivedId], [GoodsReceivedDate], ";
                    Query += "[PurchId], [SiteId], [SiteName], [TransStatusId], [ActionCode], [CreatedDate], [CreatedBy]) VALUES";
                    Query += "('" + NRBNo + "', getdate(), 'AUTO', '" + VendorID + "', ";
                    Query += "'" + VendorName + "', '" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceivedDate + "', ";
                    Query += "'" + PurchID + "', '" + SiteID + "', '" + SiteName + "', '03', '01', GETDATE(), '" + ControlMgr.UserId + "') ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header Nota Retur Beli

                    //Insert Detail Nota Retur Beli
                    Query = "INSERT INTO [NotaReturBeli_Dtl] ([NRBId], [SeqNo], [GroupId], [SubGroup1Id], [SubGroup2Id], ";
                    Query += "[ItemId], [FullItemId], [ItemName], [GoodsReceivedId], [GoodsReceived_SeqNo], [UoM_Qty], ";
                    Query += "[Alt_Qty], [UoM_Unit], [Alt_Unit], [Ratio], [RemainingQty], [InventSiteId], ";
                    Query += "[CreatedDate], [CreatedBy], [Price]) VALUES ( ";
                    Query += "'" + NRBNo + "', 1, '" + GroupID + "', '" + SubGroup1ID + "', ";
                    Query += "'" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                    Query += "'" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceived_SeqNo + "', " + Qty + ", " + (Qty * Ratio) + ", ";
                    Query += "'" + Unit + "', '" + UnitAlt + "', " + Ratio + ", " + Qty + " ,'" + SiteID + "', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', " + Price + "";
                    Query += ") ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail Nota Retur Beli                    

                    //Insert Log
                    string StatusDesc = "";
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='03' AND TransCode = 'RO' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO ReceiptOrder_LogTable (ReceiptOrderDate, ReceiptOrderNo, PurchaseOrderDate, VendorID, ";
                    Query += "InventSiteID, FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, GoodsReceivedId, ";
                    Query += "LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES ('" + ReceiptOrderDate + "', '" + ReceiptOrderId + "', '" + PODate + "', '" + VendorID + "', ";
                    Query += "'" + SiteID + "', '" + FullItemID + "', 1, " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '" + txtGoodsReceivedNumber.Text + "', ";
                    Query += "'03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();


                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='04' AND TransCode = 'NotaReturBeli' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO NotaReturBeli_LogTable (NRBDate, NRBId, GoodsReceivedDate, GoodsReceivedId, VendId, ";
                    Query += "SiteId, FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES (GETDATE(), '" + NRBNo + "', '" + GoodsReceivedDate + "', '" + txtGoodsReceivedNumber.Text + "', '" + VendorID + "', ";
                    Query += "'" + SiteID + "', '" + FullItemID + "', 1, " + QtyUoM + ", "+ QtyAlt +", " + SubTotal + ", '04', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();


                    //Query = "SELECT Deskripsi FROM TransStatusTable ";
                    //Query += "WHERE StatusCode='03' AND TransCode = 'NotaPurchasePark' ";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    //Query = "INSERT INTO NotaPurchasePark_LogTable (NPPId, NPPDate, RefTransId, RefTransDate, AccountId, AccountName, ";
                    //Query += "RefTrans2Id, RefTrans2Date, InventSiteID, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    //Query += "VALUES ('" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', '" + NRBNo + "', GETDATE(), '" + VendorID + "', '" + VendorName + "', ";
                    //Query += "'" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceivedDate + "', '" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteNonQuery();
                    //End Insert Log

                    //System Update
                    Query = "UPDATE Invent_Purchase_Qty SET ";
                    Query += "RO_Issued_UoM = (RO_Issued_UoM - " + QtyUoM + "), RO_Issued_Alt = (RO_Issued_Alt - " + QtyAlt + "), ";
                    Query += "RO_Issued_Amount = (RO_Issued_Amount - (" + SubTotal + ")), ";
                    Query += "Retur_Beli_In_Progress_UoM = (Retur_Beli_In_Progress_UoM + " + QtyUoM + "), Retur_Beli_In_Progress_Alt = (Retur_Beli_In_Progress_Alt + " + QtyAlt + "), ";
                    Query += "Retur_Beli_In_Progress_Amount = (Retur_Beli_In_Progress_Amount + (" + SubTotal + ")) ";
                    Query += "WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE Invent_Movement_Qty SET ";
                    Query += "Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM - " + QtyUoM + "), Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt - " + QtyAlt + "), ";
                    Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount - (" + SubTotal + ")) ";
                    Query += "WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End System Update
                }
            }
        }

        private void ReturDebitNota(SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                string ActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);

                if (ActionCode == "Retur Debit")
                {
                    //Get Data 
                    string GoodsReceived_SeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["GoodsReceived_SeqNo"].Value);
                    string FullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
                    string ItemName = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value);
                    string Unit = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["Unit"].Value);
                    decimal Price = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
                    decimal Qty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    decimal Ratio = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Ratio"].Value);
                    string NPPSeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value);


                    string[] SplitFullItemID = FullItemID.Split('.');
                    string GroupID = Convert.ToString(SplitFullItemID[0]);
                    string SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                    string SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                    string ItemID = Convert.ToString(SplitFullItemID[3]);

                    Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());                    

                    string ReceiptOrderId = "", ReceiptOrderSeqNo = "", ReceiptOrderDate = "", PurchID = "", SiteID = "", SiteName = "", 
                    GoodsReceivedDate = "", PODate = "", VendorID = "", VendorName = "", SJDate = "", SJNumber = "", 
                    VehicleType = "", VehicleNumber = "", DriverName = "", SiteLocation = "", DeliveryMethod = "", SiteBlokID = "", Quality = "";

                    decimal PPN = 0, PPH = 0;

                    Query = "SELECT R.ReceiptOrderId, R.ReceiptOrderDate, R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, R.VendId AS VendorID, R.VendorName, ";
                    Query += "G.GoodsReceivedDate, G.SJDate, G.SJNumber, G.VehicleType, G.VehicleNumber, G.DriverName, G.SiteLocation, GD.RefTransSeqNo AS ReceiptOrderSeqNo, ";
                    Query += "GD.DeliveryMethod, GD.InventSiteBlokID, GD.Quality, P.OrderDate AS PODate, P.PPN, P.PPH ";
                    Query += "FROM ReceiptOrderH R INNER JOIN PurchH P ON P.PurchID = R.PurchaseOrderId ";
                    Query += "INNER JOIN GoodsReceivedH G ON G.RefTransID = R.ReceiptOrderId ";
                    Query += "INNER JOIN GoodsReceivedD GD ON GD.GoodsReceivedId = G.GoodsReceivedId ";
                    Query += "WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GD.GoodsReceivedSeqNo = '" + GoodsReceived_SeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);
                        ReceiptOrderDate = Convert.ToString(Dr["ReceiptOrderDate"]);
                        PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                        SiteID = Convert.ToString(Dr["InventSiteID"]);
                        SiteName = Convert.ToString(Dr["InventSiteName"]);
                        SiteLocation = Convert.ToString(Dr["SiteLocation"]);
                        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                        PODate = Convert.ToString(Dr["PODate"]);
                        VendorID = Convert.ToString(Dr["VendorID"]);
                        VendorName = Convert.ToString(Dr["VendorName"]);
                        SJDate = Convert.ToString(Dr["SJDate"]);
                        SJNumber = Convert.ToString(Dr["SJNumber"]);
                        VehicleType = Convert.ToString(Dr["VehicleType"]);
                        VehicleNumber = Convert.ToString(Dr["VehicleNumber"]);
                        DriverName = Convert.ToString(Dr["DriverName"]);
                        SiteLocation = Convert.ToString(Dr["SiteLocation"]);
                        ReceiptOrderSeqNo = Convert.ToString(Dr["ReceiptOrderSeqNo"]);
                        DeliveryMethod = Convert.ToString(Dr["DeliveryMethod"]);
                        PPN = Convert.ToDecimal(Dr["PPN"]);
                        PPH = Convert.ToDecimal(Dr["PPH"]);
                        SiteBlokID = Convert.ToString(Dr["InventSiteBlokID"]);
                        Quality = Convert.ToString(Dr["Quality"]);
                    }
                    Dr.Close();

                    //Get UOM, ALT===============================================================================
                    Query = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UoM = Cmd.ExecuteScalar().ToString();
                    decimal QtyUoM = 0, QtyAlt = 0;

                    if (Unit == UoM)
                    {
                        QtyUoM = Qty;
                        QtyAlt = Qty * Ratio;
                    }
                    else
                    {
                        QtyAlt = Qty;
                        QtyUoM = Qty / Ratio;
                    }
                    //End Get UOM, ALT==============================================================================
                    
                    decimal TimbangWeight = 0;
                    TimbangWeight = Qty * Ratio;

                    decimal SubTotal = Convert.ToDecimal((Qty * Price).ToString("F4"));
                    decimal SubTotalPPN = Convert.ToDecimal(((Qty * Price) * (PPN / 100)).ToString("F4"));
                    decimal SubTotalPPH = Convert.ToDecimal(((Qty * Price) * (PPH / 100)).ToString("F4"));                   
                    //End Get Data

                    //Insert Header GoodsReceived
                    string Jenis = "GR", Kode = "BBMA";
                    string GRNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "INSERT INTO [GoodsReceivedH] ( ";
                    Query += "GoodsReceivedId, GoodsReceivedDate, GoodsReceivedStatus, RefTransID, RefTransDate, ";
                    Query += "SJDate, SJNumber, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, ";
                    Query += "Timbang1Weight, Timbang2Date, Timbang2Weight, SiteID, SiteName, SiteLocation, CreatedDate, ";
                    Query += "CreatedBy, StatusWeight1, StatusWeight2, RefTransType, NPPId) VALUES( ";
                    Query += "'" + GRNo + "', GETDATE(), '03', '" + ReceiptOrderId + "', '" + ReceiptOrderDate + "', ";
                    Query += "'" + SJDate + "', '" + SJNumber + "', '" + VendorID + "', '" + VendorName + "', '" + VehicleType + "', '" + VehicleNumber + "', '" + DriverName + "', ";
                    Query += "GETDATE(), '" + TimbangWeight + "', GETDATE(), '" + TimbangWeight + "', '" + SiteID + "', '" + SiteName + "', '" + SiteLocation + "', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', 'Manual', 'Manual', 'Receipt Order', '" + txtNotaNumber.Text + "') ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();                    
                    //End Insert Header GoodsReceived                   

                    //Insert Detail GoodsReceived
                    Query = "INSERT INTO GoodsReceivedD ( ";
                    Query += "GoodsReceivedDate, GoodsReceivedId, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, ";
                    Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_SJ, Qty_Actual, ";
                    Query += "Unit, Ratio, Ratio_Actual, InventSiteId, InventSiteBlokID, Quality, ActionCodeStatus, ";
                    Query += "CreatedDate, CreatedBy, DeliveryMethod, NPPId, NPP_SeqNo, Price, Total, PPN, Total_PPN, PPH, Total_PPH) ";
                    Query += "VALUES (GETDATE(), '" + GRNo + "', 1, '" + ReceiptOrderId + "', '" + ReceiptOrderSeqNo + "', ";
                    Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                    Query += "" + Qty + ", " + Qty + ", " + Qty + ", '" + Unit + "', '" + Ratio + "', '" + Ratio + "', ";
                    Query += "'" + SiteID + "', '" + SiteBlokID + "', '" + Quality + "', '05', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', '" + DeliveryMethod + "', '" + txtNotaNumber.Text + "',";
                    Query += "'" + NPPSeqNo + "', " + Price + ", " + (SubTotal) + ", " + PPN + ", " + SubTotalPPN + ", " + PPH + ", " + SubTotalPPH + " ";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail GoodsReceived

                    //Insert Header Nota Retur Beli  
                    Jenis = "NRB"; Kode = "NRBA";
                    string NRBNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "INSERT INTO [NotaReturBeliH] ([NRBId], [NRBDate], [NRBMode], [VendId], [VendName], [GoodsReceivedId], [GoodsReceivedDate], ";
                    Query += "[PurchId], [SiteId], [SiteName], [TransStatusId], [ActionCode], [CreatedDate], [CreatedBy]) VALUES";
                    Query += "('" + NRBNo + "', getdate(), 'AUTO', '" + VendorID + "', ";
                    Query += "'" + VendorName + "', '" + GRNo + "', GETDATE(), ";
                    Query += "'" + PurchID + "', '" + SiteID + "', '" + SiteName + "', '03', '01', GETDATE(), '" + ControlMgr.UserId + "') ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header Nota Retur Beli

                    //Insert Detail Nota Retur Beli
                    Query = "INSERT INTO [NotaReturBeli_Dtl] ([NRBId], [SeqNo], [GroupId], [SubGroup1Id], [SubGroup2Id], ";
                    Query += "[ItemId], [FullItemId], [ItemName], [GoodsReceivedId], [GoodsReceived_SeqNo], [UoM_Qty], ";
                    Query += "[Alt_Qty], [UoM_Unit], [Alt_Unit], [Ratio], [RemainingQty], [InventSiteId], ";
                    Query += "[CreatedDate], [CreatedBy], [Price]) VALUES ( ";
                    Query += "'" + NRBNo + "', 1, '" + GroupID + "', '" + SubGroup1ID + "', ";
                    Query += "'" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                    Query += "'" + GRNo + "', 1, " + Qty + ", " + (Qty * Ratio) + ", ";
                    Query += "'" + Unit + "', '" + UnitAlt + "', " + Ratio + ", " + Qty + " ,'" + SiteID + "', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', " + Price + "";
                    Query += ") ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail Nota Retur Beli                    

                    //Insert Log
                    string StatusDesc = "";
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='03' AND TransCode = 'RO' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO ReceiptOrder_LogTable (ReceiptOrderDate, ReceiptOrderNo, PurchaseOrderNo, PurchaseOrderDate, VendorID, ";
                    Query += "InventSiteID, FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, GoodsReceivedId, ";
                    Query += "LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES ('" + ReceiptOrderDate + "', '" + ReceiptOrderId + "', '" + PurchID + "', '" + PODate + "', '" + VendorID + "', ";
                    Query += "'" + SiteID + "', '" + FullItemID + "', '1', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '" + GRNo + "', ";
                    Query += "'03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='04' AND TransCode = 'NotaReturBeli' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO NotaReturBeli_LogTable (NRBDate, NRBId, GoodsReceivedDate, GoodsReceivedId, VendId, ";
                    Query += "SiteId, FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES (GETDATE(), '" + NRBNo + "', GETDATE(), '" + GRNo + "', '" + VendorID + "', ";
                    Query += "'" + SiteID + "', '" + FullItemID + "', 1, " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '04', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='03' AND TransCode = 'GR' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO GoodsReceived_LogTable (GoodsReceivedDate, GoodsReceivedId, ReceiptOrderNo, ReceiptOrderDate, VendorID, InventSiteID, ";
                    Query += "FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES (GETDATE(), '" + GRNo + "', '" + ReceiptOrderId + "', '" + ReceiptOrderDate + "', '" + VendorID + "', '" + SiteID + "', ";
                    Query += "'" + FullItemID + "', 1, " + QtyUoM + ", " + QtyAlt + ", " + (SubTotal) + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    //Query = "SELECT Deskripsi FROM TransStatusTable ";
                    //Query += "WHERE StatusCode='03' AND TransCode = 'NotaPurchasePark' ";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    //Query = "INSERT INTO NotaPurchasePark_LogTable (NPPId, NPPDate, RefTransId, RefTransDate, AccountId, AccountName, ";
                    //Query += "RefTrans2Id, RefTrans2Date, InventSiteID, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    //Query += "VALUES ('" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', '" + NRBNo + "', GETDATE(), '" + VendorID + "', '" + VendorName + "', ";
                    //Query += "'" + GRNo + "', GETDATE(), '" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteNonQuery();                    
                    //End Insert Log

                    //System Update
                    Query = "UPDATE Invent_Purchase_Qty SET ";
                    Query += "RO_Issued_UoM = (RO_Issued_UoM - " + QtyUoM + "), RO_Issued_Alt = (RO_Issued_Alt - " + QtyAlt + "), ";
                    Query += "RO_Issued_Amount = (RO_Issued_Amount - (" + SubTotal + ")), ";
                    Query += "Retur_Beli_In_Progress_UoM = (Retur_Beli_In_Progress_UoM + " + QtyUoM + "), Retur_Beli_In_Progress_Alt = (Retur_Beli_In_Progress_Alt + " + QtyAlt + "), ";
                    Query += "Retur_Beli_In_Progress_Amount = (Retur_Beli_In_Progress_Amount + (" + SubTotal + ")) ";
                    Query += "WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE Invent_Movement_Qty SET ";
                    Query += "Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM - " + QtyUoM + "), Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt - " + QtyAlt + "), ";
                    Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount - (" + SubTotal + ")) ";
                    Query += "WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End System Update
                }
            }
        }

        private void NewPurchase(SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                string ActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);

                if (ActionCode == "New Purchase")
                {
                    //Get Data 
                    string GoodsReceived_SeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["GoodsReceived_SeqNo"].Value);
                    string FullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
                    string ItemName = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value);
                    string Unit = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["Unit"].Value);
                    decimal Price = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
                    decimal Qty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    decimal Ratio = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Ratio"].Value);
                    string NPPSeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value);
                    string NotesDetail = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["Notes"].Value);


                    string[] SplitFullItemID = FullItemID.Split('.');
                    string GroupID = Convert.ToString(SplitFullItemID[0]);
                    string SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                    string SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                    string ItemID = Convert.ToString(SplitFullItemID[3]);

                    decimal Qty_KG = 0, Price_KG = 0; 
                    if (Unit.ToUpper() == "KG")
                    {
                        Qty_KG = Qty;
                        Price_KG = Price;
                    }
                    else
                    {
                        Qty_KG = Qty / Ratio;
                        Price_KG = Price / Ratio;
                    }

                    Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());

                    string ReceiptOrderId = "", ReceiptOrderSeqNo = "", ReceiptOrderDate = "", PurchID = "", SiteID = "", SiteName = "",
                    GoodsReceivedDate = "", PODate = "", VendorID = "", VendorName = "", SJDate = "", SJNumber = "",
                    VehicleType = "", VehicleNumber = "", DriverName = "", SiteLocation = "", DeliveryMethod = "", SiteBlokID = "", Quality = "",
                    PaymentMode = "", TermOfPayment = "";

                    decimal PPN = 0, PPH = 0, ExchRate = 0;

                    Query = "SELECT R.ReceiptOrderId, R.ReceiptOrderDate, R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, R.VendId AS VendorID, R.VendorName, ";
                    Query += "G.GoodsReceivedDate, G.SJDate, G.SJNumber, G.VehicleType, G.VehicleNumber, G.DriverName, G.SiteLocation, GD.RefTransSeqNo AS ReceiptOrderSeqNo, ";
                    Query += "GD.DeliveryMethod, GD.InventSiteBlokID, GD.Quality, P.OrderDate AS PODate, P.PPN, P.PPH, P.PaymentMode, P.TermofPayment AS TermOfPayment, P.ExchRate ";
                    Query += "FROM ReceiptOrderH R INNER JOIN PurchH P ON P.PurchID = R.PurchaseOrderId ";
                    Query += "INNER JOIN GoodsReceivedH G ON G.RefTransID = R.ReceiptOrderId ";
                    Query += "INNER JOIN GoodsReceivedD GD ON GD.GoodsReceivedId = G.GoodsReceivedId ";
                    Query += "WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GD.GoodsReceivedSeqNo = '" + GoodsReceived_SeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);
                        ReceiptOrderDate = Convert.ToString(Dr["ReceiptOrderDate"]);
                        PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                        SiteID = Convert.ToString(Dr["InventSiteID"]);
                        SiteName = Convert.ToString(Dr["InventSiteName"]);
                        SiteLocation = Convert.ToString(Dr["SiteLocation"]);
                        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                        PODate = Convert.ToString(Dr["PODate"]);
                        VendorID = Convert.ToString(Dr["VendorID"]);
                        VendorName = Convert.ToString(Dr["VendorName"]);
                        SJDate = Convert.ToString(Dr["SJDate"]);
                        SJNumber = Convert.ToString(Dr["SJNumber"]);
                        VehicleType = Convert.ToString(Dr["VehicleType"]);
                        VehicleNumber = Convert.ToString(Dr["VehicleNumber"]);
                        DriverName = Convert.ToString(Dr["DriverName"]);
                        SiteLocation = Convert.ToString(Dr["SiteLocation"]);
                        ReceiptOrderSeqNo = Convert.ToString(Dr["ReceiptOrderSeqNo"]);
                        DeliveryMethod = Convert.ToString(Dr["DeliveryMethod"]);
                        PPN = Convert.ToDecimal(Dr["PPN"]);
                        PPH = Convert.ToDecimal(Dr["PPH"]);
                        SiteBlokID = Convert.ToString(Dr["InventSiteBlokID"]);
                        Quality = Convert.ToString(Dr["Quality"]);
                        PaymentMode = Convert.ToString(Dr["PaymentMode"]);
                        TermOfPayment = Convert.ToString(Dr["TermOfPayment"]);
                        ExchRate = Convert.ToDecimal(Dr["ExchRate"]);
                    }
                    Dr.Close();

                    //Get UOM, ALT===============================================================================
                    Query = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UoM = Cmd.ExecuteScalar().ToString();
                    decimal QtyUoM = 0, QtyAlt = 0;

                    if (Unit == UoM)
                    {
                        QtyUoM = Qty;
                        QtyAlt = Qty * Ratio;
                    }
                    else
                    {
                        QtyAlt = Qty;
                        QtyUoM = Qty / Ratio;
                    }
                    //End Get UOM, ALT==============================================================================

                    decimal TimbangWeight = 0;
                    TimbangWeight = Qty * Ratio;

                    decimal SubTotal = Convert.ToDecimal((Qty * Price).ToString("F4"));
                    decimal SubTotalPPN = Convert.ToDecimal(((Qty * Price) * (PPN / 100)).ToString("F4"));
                    decimal SubTotalPPH = Convert.ToDecimal(((Qty * Price) * (PPH / 100)).ToString("F4"));
                    decimal SubTotalNet = SubTotal + SubTotalPPN + SubTotalPPH;

                    //End Get Data

                    //Insert Header PR
                    string Jenis = "PR", Kode = "PRA";
                    string PRNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);                    
                    Query = "INSERT INTO PurchRequisitionH (PurchReqID, OrderDate, TransType, TransStatus, ApprovedBy ,CreatedDate, CreatedBy) VALUES ";
                    Query += "('" + PRNo + "', GETDATE(), 'FIX', '22', '" + ControlMgr.GroupName + "', GETDATE(), '" + ControlMgr.UserId + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header PR

                    //Insert Detail PR
                    Query = "INSERT INTO PurchRequisition_Dtl (PurchReqID, SeqNo, FullItemID, ItemName, DeliveryMethod, OrderDate, ";
                    Query += "VendID, Qty, Qty_KG, Unit, Ratio, GroupId, SubGroup1Id, SubGroup2Id, ItemId, ";
                    Query += "Price, SeqNoGroup, TransStatus, TransStatusPurch, ApprovePersonPurch, CreatedDate, CreatedBy) VALUES ";
                    Query += "('" + PRNo + "', 1, '" + FullItemID + "', '" + ItemName + "', '" + DeliveryMethod + "', GETDATE(), ";
                    Query += "'" + VendorID + "', " + Qty + ", " + Qty_KG + ", '" + Unit + "', " + Ratio + ", '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', ";
                    Query += "" + Price + ", 1, 'Yes', 'Yes', '" + ControlMgr.UserId + "', GETDATE(), '" + ControlMgr.UserId + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail PR

                    //Insert Header Purch Quotation
                    Jenis = "PQ"; Kode = "PQA";
                    string PQNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                    Query = "INSERT INTO PurchQuotationH (PurchQuotID, TransType, OrderDate, VendorQuotDate, VendID ,VendName, ";
                    Query += "TermOfPayment, TransStatus, PaymentModeID, ApprovedBy, ";
                    Query += "PPN, PPH, Total, Total_PPN, Total_PPH, CreatedDate, CreatedBy, TotalDiscount) ";
                    Query += "VALUES ";
                    Query += "('" + PQNo + "', 'FIX', GETDATE(), GETDATE(), '" + VendorID + "', '" + VendorName + "', ";
                    Query += "'" + TermOfPayment + "', '03', '" + PaymentMode + "', '" + ControlMgr.UserId + "',";
                    Query += "" + PPN + ", " + PPH + ", " + SubTotal + ", " + SubTotalPPN + ", " + SubTotalPPH + ", GETDATE(), '" + ControlMgr.UserId + "', 0);";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header Purch Quotation

                    //Insert Detail Purch Quotation
                    Query = "INSERT INTO PurchQuotation_Dtl (PurchQuotID, OrderDate, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ";
                    Query += "ItemID, FullItemID, ItemName, Qty, Qty_KG, Unit, Ratio, Price, Price_KG, ReffTransID, ";
                    Query += "ReffSeqNo, ReffTransType, ApprovalNotes, CreatedDate, CreatedBy, Qty2, DeliveryMethod, SubTotal, ";
                    Query += "SubTotal_PPN, SubTotal_PPH, SeqNoGroup) VALUES ";
                    Query += "('" + PQNo + "', GETDATE(), 1, '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', ";
                    Query += "'" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', " + Qty + ", " + Qty_KG + ", '" + Unit + "', " + Ratio + ", " + Price + ", " + Price_KG + ", '" + PRNo + "', ";
                    Query += "1, 'FIX', '" + NotesDetail + "', GETDATE(), '" + ControlMgr.UserId + "', " + Qty + ", '" + DeliveryMethod + "', " + SubTotal + ", " + SubTotalPPN + ", " + SubTotalPPH + ", 1 ";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail Purch Quotation

                    //Insert Attachment Purch Quotation
                    for (int j = 0; j <= dgvAttachment.RowCount - 1; j++)
                    {
                        Query = "INSERT tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) VALUES";
                        Query += "( 'PurchQuotationH', '" + PQNo + "', '";
                        Query += dgvAttachment.Rows[j].Cells["FileName"].Value.ToString() + "', '";
                        Query += dgvAttachment.Rows[j].Cells["Extension"].Value.ToString() + "', '";
                        Query += dgvAttachment.Rows[j].Cells["FileSize"].Value.ToString();
                        Query += "',@binaryValue";
                        Query += ");";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, ListAttachment[j].Length).Value = ListAttachment[j];
                        Cmd.ExecuteNonQuery();
                    }
                    //Insert Attachment Purch Quotation
                   
                    //Insert Header Canvas Sheet
                    Jenis = "CS"; Kode = "CSA";
                    string CSNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                    Query = "INSERT INTO [CanvasSheetH] (CanvasId, CanvasDate, PurchReqId, TransType, TransStatus, CreatedDate, CreatedBy) VALUES ";
                    Query += "('" + CSNo + "', GETDATE(), '" + PRNo + "', 'FIX', '02', GETDATE(), '" + ControlMgr.UserId + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header Canvas Sheet

                    //Insert Detail Canvas Sheet
                    Query = "INSERT INTO CanvasSheetD (CanvasId, CanvasSeqNo, VendID, PurchQuotId, PurchReqId, PurchReqSeqNo, ";
                    Query += "DeliveryMethod, FullItemId, ItemName, Price, Price_KG, SeqNo, SeqNoGroup, PRQty, ";
                    Query += "PQQty, Qty, Qty_KG, Unit, Ratio, StatusApproval, PPN, PPH, CreatedDate, CreatedBy) ";
                    Query += "VALUES ('" + CSNo + "', 1, '" + VendorID + "', '" + PQNo + "', '" + PRNo + "', 1, ";
                    Query += "'" + DeliveryMethod + "', '" + FullItemID + "', '" + ItemName + "', " + Price + ", " + Price_KG + ", 1, 1, " + Qty + ", ";
                    Query += "" + Qty + ", " + Qty + ", " + Qty_KG + ", '" + Unit + "', " + Ratio + ", 'Yes', " + PPN + ", " + PPH + ", GETDATE(), '" + ControlMgr.UserId + "'";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();     
                    //End Detail Header Canvas Sheet

                    //Insert Header Purch Order
                    Jenis = "PO"; Kode = "POA";
                    string PONo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                    Query = "INSERT INTO [PurchH] (PurchID, OrderDate, DueDate, TransType, ReffTableName, ReffId, ";
                    Query += "ReffId2, ExchRate, VendID, Total, Total_Nett, Total_Disk, ";
                    Query += "PPN, Total_PPN, PPH, Total_PPH, StClose, TransStatus, ";
                    Query += "CreatedDate, CreatedBy, PurchH_Print, PurchH_SendEmail, EmailID, TermofPayment, PaymentMode) VALUES ";
                    Query += "('" + PONo + "', GETDATE(), GETDATE(), 'FIX', 'CanvasSheet', '" + CSNo + "', ";
                    Query += "'" + PQNo + "', " + ExchRate + ", '" + VendorID + "', " + SubTotal + ", " + SubTotalNet + ", 0, ";
                    Query += "" + PPN + ", " + SubTotalPPN + ", " + PPH + ", " + SubTotalPPH + ", 0, '02', ";
                    Query += "GETDATE(),'" + ControlMgr.UserId + "', 0, 0, '', '" + TermOfPayment + "', '" + PaymentMode + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header Purch Order

                    //Insert Detail Purch Order
                    Query = "INSERT INTO PurchDtl ( ";
                    Query += "PurchID, OrderDate, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ";
                    Query += "ItemID, FullItemID, ItemName, InventSiteID, Qty, Qty_KG, ";
                    Query += "RemainingQty, Unit, Konv_Ratio, Price, Price_KG, Total, ";
                    Query += "Diskon, Total_Disk, Total_PPN, Total_PPH, ReffTableName, ReffId, ";
                    Query += "ReffId2, ReffSeqNo, CreatedDate, CreatedBy, AvailableDate, DeliveryMethod) VALUES (";
                    Query += "'" + PONo + "', GETDATE(), 1, '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', ";
                    Query += "'" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', '" + SiteID + "', " + Qty + ", " + Qty_KG + ", ";
                    Query += "0, '" + Unit + "', " + Ratio + ", " + Price + ", " + Price_KG + ", " + SubTotal + ", ";
                    Query += "0, 0, " + SubTotalPPN + "," + SubTotalPPH + ", 'CanvasSheet', '" + CSNo + "', ";
                    Query += "'" + PQNo + "', 1, GETDATE(), '" + ControlMgr.UserId + "', GETDATE(), '" + DeliveryMethod + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail Purch Order

                    //Insert Header Receipt Order
                    Jenis = "RO"; Kode = "ROA";
                    string RONo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                    Query = "INSERT INTO [ReceiptOrderH] ( ";
                    Query += "ReceiptOrderId, ReceiptOrderDate, ReceiptOrderStatus, VendId, VendorName, InventSiteID, ";
                    Query += "InventSiteName, InventSiteLocation, VehicleType, VehicleNumber, DriverName, DeliveryDate, ";
                    Query += "PurchaseOrderId, PurchaseOrderDate, CreatedDate, CreatedBy, PaymentMode) VALUES( ";
                    Query += "'" + RONo + "',";
                    Query += "GETDATE(), '03', '" + VendorID + "', '" + VendorName + "','" + SiteID + "',";
                    Query += "'" + SiteName + "', '" + SiteLocation + "', '" + VehicleType + "', '" + VehicleNumber + "', '" + DriverName + "', GETDATE(), ";
                    Query += "'" + PONo + "', GETDATE(), GETDATE(), '" + ControlMgr.UserId + "', '" + PaymentMode + "') ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header Receipt Order

                    //Insert Detail Receipt Order
                    Query = "Insert ReceiptOrderD(ReceiptOrderId, PurchaseOrderId, PurchaseOrderSeqNo, SeqNo, ";
                    Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, ";
                    Query += "Qty, Qty_KG, RemainingQty, Unit, Ratio, Price, ";
                    Query += "Price_KG, Total, Total_PPN, Total_PPH, InventSiteID, ";
                    Query += "CreatedDate, CreatedBy, DeliveryMethod) VALUES( ";
                    Query += "'" + RONo + "', '" + PONo + "', 1,  1, ";
                    Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "',  '" + FullItemID + "', '" + ItemName + "', ";
                    Query += "" + Qty + ", " + Qty_KG + ", 0, '" + Unit + "', " + Ratio + ", " + Price + ", ";
                    Query += "" + Price_KG + ", " + SubTotal + ", " + SubTotalPPN + ", " + SubTotalPPH + ", '" + SiteID + "', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "','" + DeliveryMethod + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail Receipt Order

                    //Insert Header Goods Received
                    Jenis = "GR"; Kode = "BBMA";
                    string GRNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "INSERT INTO [GoodsReceivedH] ( ";
                    Query += "GoodsReceivedId, GoodsReceivedDate, GoodsReceivedStatus, RefTransID, RefTransDate, ";
                    Query += "SJDate, SJNumber, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, ";
                    Query += "Timbang1Weight, Timbang2Date, Timbang2Weight, SiteID, SiteName, SiteLocation, CreatedDate, ";
                    Query += "CreatedBy, StatusWeight1, StatusWeight2, RefTransType, NPPId) VALUES( ";
                    Query += "'" + GRNo + "', GETDATE(), '03', '" + ReceiptOrderId + "', '" + ReceiptOrderDate + "', ";
                    Query += "'" + SJDate + "', '" + SJNumber + "', '" + VendorID + "', '" + VendorName + "', '" + VehicleType + "', '" + VehicleNumber + "', '" + DriverName + "', ";
                    Query += "GETDATE(), '" + TimbangWeight + "', GETDATE(), '" + TimbangWeight + "', '" + SiteID + "', '" + SiteName + "', '" + SiteLocation + "', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', 'Manual', 'Manual', 'Receipt Order', '" + txtNotaNumber.Text + "') ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery(); 
                    //End Insert Header Goods Received

                    //Insert Detail Goods Received
                    Query = "INSERT INTO GoodsReceivedD ( ";
                    Query += "GoodsReceivedDate, GoodsReceivedId, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, ";
                    Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_SJ, Qty_Actual, ";
                    Query += "Unit, Ratio, Ratio_Actual, InventSiteId, InventSiteBlokID, Quality, ActionCodeStatus, ";
                    Query += "CreatedDate, CreatedBy, DeliveryMethod, NPPId, NPP_SeqNo, Price, Total, PPN, Total_PPN, PPH, Total_PPH) ";
                    Query += "VALUES (GETDATE(), '" + GRNo + "', 1, '" + RONo + "', 1, ";
                    Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                    Query += "" + Qty + ", " + Qty + ", " + Qty + ", '" + Unit + "', '" + Ratio + "', '" + Ratio + "', ";
                    Query += "'" + SiteID + "', '" + SiteBlokID + "', '" + Quality + "', '05', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', '" + DeliveryMethod + "', '" + txtNotaNumber.Text + "',";
                    Query += "'" + NPPSeqNo + "', " + Price + ", " + (Qty * Price) + ", " + PPN + ", " + SubTotalPPN + ", " + PPH + ", " + SubTotalPPH + " ";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail Goods Received

                    //Check Resize & Insert Nota Resize
                    string Resize = "";
                    decimal CountResize = 0;
                    Query = "SELECT TOP 1 Resize, ResizeType FROM InventTable WHERE FullItemID = '" + FullItemID + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        Resize = Convert.ToString(Dr["Resize"]);
                    }
                    Dr.Close();

                    string TransId = "";
                    if (Resize.ToUpper() == "TRUE")
                    {
                        Jenis = "NRZ"; Kode = "NRZA";
                        TransId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                        //Insert Header Nota Resize
                        Query = "INSERT INTO NotaResizeH ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                        Query += "[SiteID], [VendID], [Posted], [CreatedDate], [CreatedBy]) VALUES ( ";
                        Query += "'" + TransId + "', GETDATE(), GETDATE(), '" + GRNo + "', ";
                        Query += "'" + SiteID + "', '" + VendorID + "', 1, GETDATE(), '" + ControlMgr.UserId + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        //End Insert Header Nota Resize

                        Query = "SELECT COUNT(*) FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        CountResize = Convert.ToInt32(Cmd.ExecuteScalar().ToString());

                        //Insert Detail Nota Resize
                        if (CountResize > 1)
                        {
                            Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], ";
                            Query += "[Qty], [Price], [Unit], [LineAmount], [GoodsReceivedId], ";
                            Query += "[GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                            Query += "'" + TransId + "', (SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') + 1  END AS SeqNo), '" + FullItemID + "', '" + ItemName + "', ";
                            Query += "" + Qty + ", " + Price + ", '" + Unit + "', '" + SubTotal + "', '" + GRNo + "', ";
                            Query += "1, GETDATE(), '" + ControlMgr.UserId + "')";
                        }
                        else
                        {
                            Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], ";
                            Query += "[ToFullItemId], [ToItemName], [Qty], [Price], [Unit], [LineAmount], ";
                            Query += "[GoodsReceivedId], [GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                            Query += "'" + TransId + "', (SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') + 1  END AS SeqNo), '" + FullItemID + "', '" + ItemName + "', ";
                            Query += "(SELECT To_FullItemId FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'), ";
                            Query += "(SELECT To_ItemName FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'), ";
                            Query += "" + Qty + ", " + Price + ", '" + Unit + "', '" + SubTotal + "', '" + GRNo + "', ";
                            Query += "1, GETDATE(), '" + ControlMgr.UserId + "'";
                            Query += ")";
                        }
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        //End Insert Detail Nota Resize
                    }
                    else
                    {
                        Query = "INSERT INTO InventTrans ([GroupId], [SubGroupId], [SubGroup2Id], [ItemId], [FullItemId], ";
                        Query += "[ItemName], [InventSiteId], [TransId], [SeqNo], [TransDate], [Ref_TransId], [Ref_TransDate], ";
                        Query += "[Ref_Trans_SeqNo], [AccountId], [AccountName], [Available_UoM], [Available_Alt], ";
                        Query += "[Available_Amount], [Available_For_Sale_UoM], [Available_For_Sale_Alt], [Available_For_Sale_Amount]) VALUES ( ";
                        Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', ";
                        Query += "'" + ItemName + "', '" + SiteID + "', '" + GRNo + "', 1, GETDATE(), ";
                        Query += "'" + RONo + "', GETDATE(), 1, '" + VendorID + "', '" + VendorName + "', ";
                        Query += "" + QtyUoM + ", " + QtyAlt + ", " + SubTotal + "," + QtyUoM + ", " + QtyAlt + ", " + SubTotal + " ";
                        Query += ")";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }


                    //End Check Resize & Insert Nota Resize                    

                    //Insert Log
                    string StatusDesc = "";
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='22' AND TransCode = 'PR' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO PurchRequisition_LogTable (PurchReqDate, PurchReqID, PurchReqType, PurchReqSeqNo, ";
                    Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES(GETDATE(), '" + PRNo + "', 'FIX', 1, ";
                    Query += "" + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '22', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='01' AND TransCode = 'PQ' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO Quotation_LogTable (QuotationDate, QuotationID, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES(GETDATE(), '" + PQNo + "', '01', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='02' AND TransCode = 'CanvasSheet' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO CanvassSheets_LogTable(CSDate, CSID, PurchReqSeqNo, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES(GETDATE(), '" + CSNo + "', 1, " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '02', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();


                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='02' AND TransCode = 'PO' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO PO_Issued_LogTable (PODate, POId, VendId, Qty_UoM, Qty_Alt, Amount, PAId, PADate, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES(GETDATE(), '" + PONo + "', '" + VendorID + "', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '" + CSNo + "', GETDATE(), '02', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='03' AND TransCode = 'RO' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO ReceiptOrder_LogTable (ReceiptOrderDate, ReceiptOrderNo, PurchaseOrderNo, PurchaseOrderDate, VendorID, ";
                    Query += "InventSiteID, FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, GoodsReceivedId, ";
                    Query += "LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES (GETDATE(), '" + RONo + "', '" + PONo + "', GETDATE(), '" + VendorID + "', ";
                    Query += "'" + SiteID + "', '" + FullItemID + "', '1', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '" + GRNo + "', ";
                    Query += "'03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='03' AND TransCode = 'GR' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO GoodsReceived_LogTable (GoodsReceivedDate, GoodsReceivedId, ReceiptOrderNo, ReceiptOrderDate, VendorID, InventSiteID, ";
                    Query += "FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES (GETDATE(), '" + GRNo + "', '" + RONo + "', GETDATE(), '" + VendorID + "', '" + SiteID + "', ";
                    Query += "'" + FullItemID + "', 1, " + QtyUoM + ", " + QtyAlt + ", " + (SubTotal) + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    //Query = "SELECT Deskripsi FROM TransStatusTable ";
                    //Query += "WHERE StatusCode='03' AND TransCode = 'NotaPurchasePark' ";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    //Query = "INSERT INTO NotaPurchasePark_LogTable (NPPId, NPPDate, RefTransId, RefTransDate, AccountId, AccountName, ";
                    //Query += "RefTrans2Id, RefTrans2Date, InventSiteID, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    //Query += "VALUES ('" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', '" + GRNo + "', GETDATE(), '" + VendorID + "', '" + VendorName + "', ";
                    //Query += "'" + RONo + "', GETDATE(), '" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteNonQuery(); 


                    if (Resize.ToUpper() == "TRUE")
                    {
                        if (CountResize > 1)
                        {
                            Query = "INSERT INTO NotaResize_LogTable ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                            Query += "[VendID], [InventSiteID], [FullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], ";
                            Query += "[Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                            Query += "'" + TransId + "', GETDATE(), GETDATE(), '" + GRNo + "', ";
                            Query += "'" + VendorID + "', '" + SiteID + "', '" + FullItemID + "',";
                            Query += "1, " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '01', ";
                            Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                            Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                            Query += "'" + ControlMgr.UserId + "', GETDATE())";

                        }
                        else
                        {
                            Query = "INSERT INTO NotaResize_LogTable ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                            Query += "[VendID], [InventSiteID], [FullItemId], [ToFullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], ";
                            Query += "[Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                            Query += "'" + TransId + "', GETDATE(), GETDATE(), '" + GRNo + "', ";
                            Query += "'" + VendorID + "', '" + SiteID + "', '" + FullItemID + "', ";
                            Query += "(SELECT To_FullItemId FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'), ";
                            Query += "1, " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '01', ";
                            Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                            Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                            Query += "'" + ControlMgr.UserId + "', GETDATE()";
                            Query += ")";
                        }
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }

                    //End Insert Log

                    //System Update
                    if (Resize.ToUpper() != "TRUE")
                    {
                        Query = "Update Invent_OnHand_Qty SET Available_For_Sale_UoM = (Available_For_Sale_UoM+" + QtyUoM + "), ";
                        Query += "Available_For_Sale_Alt = (Available_For_Sale_Alt+" + QtyAlt + "), Available_For_Sale_Amount = (Available_For_Sale_Amount+ " + SubTotal + ") ";
                        Query += "where FullItemID='" + FullItemID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Query = "Update Invent_Movement_Qty SET Resize_In_Progress_UoM = (Resize_In_Progress_UoM + " + QtyUoM + "), ";
                        Query += "Resize_In_Progress_Alt = (Resize_In_Progress_Alt + " + QtyAlt + "), ";
                        Query += "Resize_In_Progress_Amount = (Resize_In_Progress_Amount+ " + SubTotal + ") ";
                        Query += "where FullItemID='" + FullItemID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }

                    Query = "Update Invent_Movement_Qty SET Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM- " + QtyUoM + "), ";
                    Query += "Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt- " + QtyAlt + "), ";
                    Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount-" + SubTotal + ") ";
                    Query += "where FullItemID='" + FullItemID + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();    
                    //End System Update
                }
            }
        }

        private void ReturKembaliBarang(SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                string ActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);

                if (ActionCode == "Retur Kembali Barang")
                {
                    //Get Data 
                    string GoodsReceived_SeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["GoodsReceived_SeqNo"].Value);
                    string FullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
                    string ItemName = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value);
                    string Unit = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["Unit"].Value);
                    decimal Price = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
                    decimal Qty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    decimal Ratio = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Ratio"].Value);

                    string[] SplitFullItemID = FullItemID.Split('.');
                    string GroupID = Convert.ToString(SplitFullItemID[0]);
                    string SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                    string SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                    string ItemID = Convert.ToString(SplitFullItemID[3]);

                    Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());

                    string ReceiptOrderId = "", ReceiptOrderDate = "", PurchID = "", SiteID = "", SiteName = "", GoodsReceivedDate = "", PODate = "", VendorID = "", VendorName = "";
                    Query = "SELECT R.ReceiptOrderId, R.ReceiptOrderDate, R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, R.VendId AS VendorID, R.VendorName, ";
                    Query += "G.GoodsReceivedDate, P.OrderDate AS PODate ";
                    Query += "FROM ReceiptOrderH R INNER JOIN PurchH P ON P.PurchID = R.PurchaseOrderId ";
                    Query += "INNER JOIN GoodsReceivedH G ON G.RefTransID = R.ReceiptOrderId ";
                    Query += "WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);
                        ReceiptOrderDate = Convert.ToString(Dr["ReceiptOrderDate"]);
                        PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                        SiteID = Convert.ToString(Dr["InventSiteID"]);
                        SiteName = Convert.ToString(Dr["InventSiteName"]);
                        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                        PODate = Convert.ToString(Dr["PODate"]);
                        VendorID = Convert.ToString(Dr["VendorID"]);
                        VendorName = Convert.ToString(Dr["VendorName"]);
                    }
                    Dr.Close();

                    //Get UOM, ALT===============================================================================
                    Query = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UoM = Cmd.ExecuteScalar().ToString();
                    decimal QtyUoM = 0, QtyAlt = 0;

                    if (Unit == UoM)
                    {
                        QtyUoM = Qty;
                        QtyAlt = Qty * Ratio;
                    }
                    else
                    {
                        QtyAlt = Qty;
                        QtyUoM = Qty / Ratio;
                    }
                    //End Get UOM, ALT==============================================================================

                    decimal SubTotal = Convert.ToDecimal((Qty * Price).ToString("F4"));
                    //End Get Data

                    string Jenis = "NRB", Kode = "NRBA";
                    string NRBNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                    string StatusDesc = "";

                    //Insert Header Nota Retur Beli  
                    Query = "INSERT INTO [NotaReturBeliH] ([NRBId], [NRBDate], [NRBMode], [VendId], [VendName], [GoodsReceivedId], [GoodsReceivedDate], ";
                    Query += "[PurchId], [SiteId], [SiteName], [TransStatusId], [ActionCode], [CreatedDate], [CreatedBy]) VALUES";
                    Query += "('" + NRBNo + "', getdate(), 'AUTO', '" + VendorID + "', ";
                    Query += "'" + VendorName + "', '" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceivedDate + "', ";
                    Query += "'" + PurchID + "', '" + SiteID + "', '" + SiteName + "', '03', '01', GETDATE(), '" + ControlMgr.UserId + "') ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header Nota Retur Beli

                    //Insert Detail Nota Retur Beli
                    Query = "INSERT INTO [NotaReturBeli_Dtl] ([NRBId], [SeqNo], [GroupId], [SubGroup1Id], [SubGroup2Id], ";
                    Query += "[ItemId], [FullItemId], [ItemName], [GoodsReceivedId], [GoodsReceived_SeqNo], [UoM_Qty], ";
                    Query += "[Alt_Qty], [UoM_Unit], [Alt_Unit], [Ratio], [RemainingQty], [InventSiteId], ";
                    Query += "[CreatedDate], [CreatedBy], [Price]) VALUES ( ";
                    Query += "'" + NRBNo + "', 1, '" + GroupID + "', '" + SubGroup1ID + "', ";
                    Query += "'" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                    Query += "'" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceived_SeqNo + "', " + Qty + ", " + (Qty * Ratio) + ", ";
                    Query += "'" + Unit + "', '" + UnitAlt + "', " + Ratio + ", " + Qty + " ,'" + SiteID + "', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', " + Price + "";
                    Query += ") ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail Nota Retur Beli                    

                    //Insert Log
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='04' AND TransCode = 'NotaReturBeli' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO NotaReturBeli_LogTable (NRBDate, NRBId, GoodsReceivedDate, GoodsReceivedId, VendId, ";
                    Query += "SiteId, FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES (GETDATE(), '" + NRBNo + "', '" + GoodsReceivedDate + "', '" + txtGoodsReceivedNumber.Text + "', '" + VendorID + "', ";
                    Query += "'" + SiteID + "', '" + FullItemID + "', 1, " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '04', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();


                    //Query = "SELECT Deskripsi FROM TransStatusTable ";
                    //Query += "WHERE StatusCode='03' AND TransCode = 'NotaPurchasePark' ";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    //Query = "INSERT INTO NotaPurchasePark_LogTable (NPPId, NPPDate, RefTransId, RefTransDate, AccountId, AccountName, ";
                    //Query += "RefTrans2Id, RefTrans2Date, InventSiteID, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    //Query += "VALUES ('" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', '" + NRBNo + "', GETDATE(), '" + VendorID + "', '" + VendorName + "', ";
                    //Query += "'" + txtGoodsReceivedNumber.Text + "', '" + GoodsReceivedDate + "', '" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteNonQuery();
                    //End Insert Log

                    //System Update
                    Query = "UPDATE Invent_Purchase_Qty SET ";
                    Query += "RO_Issued_UoM = (RO_Issued_UoM - " + QtyUoM + "), RO_Issued_Alt = (RO_Issued_Alt - " + QtyAlt + "), ";
                    Query += "RO_Issued_Amount = (RO_Issued_Amount - (" + SubTotal + ")), ";
                    Query += "Retur_Beli_In_Progress_UoM = (Retur_Beli_In_Progress_UoM + " + QtyUoM + "), Retur_Beli_In_Progress_Alt = (Retur_Beli_In_Progress_Alt + " + QtyAlt + "), ";
                    Query += "Retur_Beli_In_Progress_Amount = (Retur_Beli_In_Progress_Amount + (" + SubTotal + ")) ";
                    Query += "WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE Invent_Movement_Qty SET ";
                    Query += "Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM - " + QtyUoM + "), Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt - " + QtyAlt + "), ";
                    Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount - (" + SubTotal + ")) ";
                    Query += "WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End System Update
                }
            }
        }
       
        private void BarangBonus(SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                string ActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);

                if (ActionCode == "Barang Bonus")
                {
                    //Get Data 
                    string GoodsReceived_SeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["GoodsReceived_SeqNo"].Value);
                    string FullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
                    string ItemName = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value);
                    string Unit = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["Unit"].Value);
                    decimal Price = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
                    decimal Qty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    decimal Ratio = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Ratio"].Value);
                    string NPPSeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value);

                    string[] SplitFullItemID = FullItemID.Split('.');
                    string GroupID = Convert.ToString(SplitFullItemID[0]);
                    string SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                    string SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                    string ItemID = Convert.ToString(SplitFullItemID[3]);

                    Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());

                    string ReceiptOrderId = "", ReceiptOrderDate = "", PurchID = "", SiteID = "", SiteName = "", GoodsReceivedDate = "", PODate = "", VendorID = "", VendorName = "", Quality = "";
                    Query = "SELECT R.ReceiptOrderId, R.ReceiptOrderDate, R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, R.VendId AS VendorID, R.VendorName, ";
                    Query += "G.GoodsReceivedDate, P.OrderDate AS PODate, GD.Quality ";
                    Query += "FROM ReceiptOrderH R INNER JOIN PurchH P ON P.PurchID = R.PurchaseOrderId ";
                    Query += "INNER JOIN GoodsReceivedH G ON G.RefTransID = R.ReceiptOrderId ";
                    Query += "INNER JOIN GoodsReceivedD GD ON GD.GoodsReceivedId = G.GoodsReceivedId ";
                    Query += "WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GD.GoodsReceivedSeqNo = '" + GoodsReceived_SeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        ReceiptOrderId = Convert.ToString(Dr["ReceiptOrderId"]);
                        ReceiptOrderDate = Convert.ToString(Dr["ReceiptOrderDate"]);
                        PurchID = Convert.ToString(Dr["PurchaseOrderId"]);
                        SiteID = Convert.ToString(Dr["InventSiteID"]);
                        SiteName = Convert.ToString(Dr["InventSiteName"]);
                        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                        PODate = Convert.ToString(Dr["PODate"]);
                        VendorID = Convert.ToString(Dr["VendorID"]);
                        VendorName = Convert.ToString(Dr["VendorName"]);
                        Quality = Convert.ToString(Dr["Quality"]);
                    }
                    Dr.Close();

                    //Get UOM, ALT===============================================================================
                    Query = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UoM = Cmd.ExecuteScalar().ToString();
                    decimal QtyUoM = 0, QtyAlt = 0;

                    if (Unit == UoM)
                    {
                        QtyUoM = Qty;
                        QtyAlt = Qty * Ratio;
                    }
                    else
                    {
                        QtyAlt = Qty;
                        QtyUoM = Qty / Ratio;
                    }
                    //End Get UOM, ALT==============================================================================

                    decimal SubTotal = Convert.ToDecimal((Qty * Price).ToString("F4"));

                    string Spec = "", Alt_Unit = "";

                    Query = "SELECT SpecID, UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        Spec = Convert.ToString(Dr["SpecID"]);
                        Alt_Unit = Convert.ToString(Dr["UoMAlt"]);

                    }
                    Dr.Close();

                    //End Get Data

                    //Insert Header Nota Adjustment
                    string Jenis = "NA", Kode = "NA2A";
                    string NANo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "INSERT INTO NotaAdjustmentH (AdjustDate, AdjustID, ActionCode, InventID, TransStatus, ApprovedBy, ApprovedNotes) ";
                    Query += "VALUES(GETDATE(), '" + NANo + "', '02', '" + SiteID + "', '03', '" + ControlMgr.UserId + "', '" + txtNotes.Text + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header Nota Adjustment

                    //Insert Detail Nota Adjustment
                    Query = "INSERT INTO NotaAdjustment_Dtl (AdjustID, SeqNo, FullItemID, ItemName, Spec, Quality, "; 
                    Query += "Qty_UoM, Qty_Unit, Qty_Alt, Alt_Unit, ApprovalNotes, CreatedDate, CreatedBY) ";
                    Query += "VALUES('" + NANo + "', 1, '" + FullItemID + "', '" + ItemName + "', ' " + Spec + " ', '" + Quality + "', ";
                    Query += "" + QtyUoM + ", '" + Unit + "', " + QtyAlt + ", '" + Alt_Unit + "', '" + txtNotes.Text + "', GETDATE(), '" + ControlMgr.UserId + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail Nota Adjustment


                    //Insert Invent Trans
                    Query = "INSERT INTO InventTrans ([GroupId], [SubGroupId], [SubGroup2Id], [ItemId], [FullItemId], ";
                    Query += "[ItemName], [InventSiteId], [TransId], [SeqNo], [TransDate], [Ref_TransId], [Ref_TransDate], ";
                    Query += "[Ref_Trans_SeqNo], [AccountId], [AccountName], [Available_UoM], [Available_Alt], ";
                    Query += "[Available_Amount], [Available_For_Sale_UoM], [Available_For_Sale_Alt], [Available_For_Sale_Amount]) VALUES ( ";
                    Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', ";
                    Query += "'" + ItemName + "', '" + SiteID + "', '" + NANo + "', 1, GETDATE(), ";
                    Query += "'" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', " + NPPSeqNo + ", '" + VendorID + "', '" + VendorName + "', ";
                    Query += "" + QtyUoM + ", " + QtyAlt + ", " + SubTotal + "," + QtyUoM + ", " + QtyAlt + ", " + SubTotal + " ";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Invent Trans

                    //Insert Log
                    string StatusDesc = "";
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='03' AND TransCode = 'NotaAdjustment' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO NotaAdjustment_LogTable(NADate, NAId, [Type], SiteId, FullItemId, SeqNo, ";
                    Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                    Query += "LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES(GETDATE(), '" + NANo + "', 'Qty', '" + SiteID + "', '" + FullItemID + "', 1, ";
                    Query += "" + QtyUoM + ", " + QtyAlt + ", " + Qty + ", '03', ";
                    Query += "'" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    //Query = "SELECT Deskripsi FROM TransStatusTable ";
                    //Query += "WHERE StatusCode='03' AND TransCode = 'NotaPurchasePark' ";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    //Query = "INSERT INTO NotaPurchasePark_LogTable (NPPId, NPPDate, RefTransId, RefTransDate, AccountId, AccountName, ";
                    //Query += "InventSiteID, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    //Query += "VALUES ('" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', '" + NANo + "', GETDATE(), '" + VendorID + "', '" + VendorName + "', ";
                    //Query += "'" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteNonQuery();
                    //End Insert Log

                    //System Update
                    Query = "UPDATE Invent_Purchase_Qty SET ";
                    Query += "RO_Issued_UoM = (RO_Issued_UoM - " + QtyUoM + "), RO_Issued_Alt = (RO_Issued_Alt - " + QtyAlt + "), ";
                    Query += "RO_Issued_Amount = (RO_Issued_Amount - (" + SubTotal + ")), ";
                    Query += "Retur_Beli_In_Progress_UoM = (Retur_Beli_In_Progress_UoM + " + QtyUoM + "), Retur_Beli_In_Progress_Alt = (Retur_Beli_In_Progress_Alt + " + QtyAlt + "), ";
                    Query += "Retur_Beli_In_Progress_Amount = (Retur_Beli_In_Progress_Amount + (" + SubTotal + ")) ";
                    Query += "WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE Invent_Movement_Qty SET ";
                    Query += "Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM - " + QtyUoM + "), Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt - " + QtyAlt + "), ";
                    Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount - (" + SubTotal + ")) ";
                    Query += "WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End System Update
                }
            }
        }

        private void ReceivedWithPO(SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                string ActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);

                if (ActionCode == "Received With PO")
                {
                    //Get Data 
                    string GoodsReceived_SeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["GoodsReceived_SeqNo"].Value);
                    string FullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
                    string ItemName = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value);
                    string Unit = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["Unit"].Value);
                    decimal Price = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
                    decimal Qty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                    decimal Ratio = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Ratio"].Value);
                    string NPPSeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value);
                    string ReceiptOrderId = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ReceiptOrderID"].Value);
                    string ReceiptOrderSeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ReceiptOrder_SeqNo"].Value);
                    string PurchID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["PurchID"].Value);

                    string[] SplitFullItemID = FullItemID.Split('.');
                    string GroupID = Convert.ToString(SplitFullItemID[0]);
                    string SubGroup1ID = Convert.ToString(SplitFullItemID[1]);
                    string SubGroup2ID = Convert.ToString(SplitFullItemID[2]);
                    string ItemID = Convert.ToString(SplitFullItemID[3]);

                    Query = "SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UnitAlt = Convert.ToString(Cmd.ExecuteScalar().ToString());

                    string ReceiptOrderDate = "", SiteID = "", SiteName = "",
                    GoodsReceivedDate = "", PODate = "", VendorID = "", VendorName = "", SJDate = "", SJNumber = "",
                    VehicleType = "", VehicleNumber = "", DriverName = "", SiteLocation = "", DeliveryMethod = "", SiteBlokID = "", Quality = "";

                    decimal PPN = 0, PPH = 0;

                    Query = "SELECT R.ReceiptOrderId, R.ReceiptOrderDate, R.PurchaseOrderId, R.InventSiteID, R.InventSiteName, R.VendId AS VendorID, R.VendorName, ";
                    Query += "G.GoodsReceivedDate, G.SJDate, G.SJNumber, G.VehicleType, G.VehicleNumber, G.DriverName, G.SiteLocation, GD.RefTransSeqNo AS ReceiptOrderSeqNo, ";
                    Query += "GD.DeliveryMethod, GD.InventSiteBlokID, GD.Quality, P.OrderDate AS PODate, P.PPN, P.PPH ";
                    Query += "FROM ReceiptOrderH R INNER JOIN PurchH P ON P.PurchID = R.PurchaseOrderId ";
                    Query += "INNER JOIN GoodsReceivedH G ON G.RefTransID = R.ReceiptOrderId ";
                    Query += "INNER JOIN GoodsReceivedD GD ON GD.GoodsReceivedId = G.GoodsReceivedId ";
                    Query += "WHERE G.GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' AND GD.GoodsReceivedSeqNo = '" + GoodsReceived_SeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        ReceiptOrderDate = Convert.ToString(Dr["ReceiptOrderDate"]);
                        SiteID = Convert.ToString(Dr["InventSiteID"]);
                        SiteName = Convert.ToString(Dr["InventSiteName"]);
                        SiteLocation = Convert.ToString(Dr["SiteLocation"]);
                        GoodsReceivedDate = Convert.ToString(Dr["GoodsReceivedDate"]);
                        PODate = Convert.ToString(Dr["PODate"]);
                        VendorID = Convert.ToString(Dr["VendorID"]);
                        VendorName = Convert.ToString(Dr["VendorName"]);
                        SJDate = Convert.ToString(Dr["SJDate"]);
                        SJNumber = Convert.ToString(Dr["SJNumber"]);
                        VehicleType = Convert.ToString(Dr["VehicleType"]);
                        VehicleNumber = Convert.ToString(Dr["VehicleNumber"]);
                        DriverName = Convert.ToString(Dr["DriverName"]);
                        SiteLocation = Convert.ToString(Dr["SiteLocation"]);
                        DeliveryMethod = Convert.ToString(Dr["DeliveryMethod"]);
                        PPN = Convert.ToDecimal(Dr["PPN"]);
                        PPH = Convert.ToDecimal(Dr["PPH"]);
                        SiteBlokID = Convert.ToString(Dr["InventSiteBlokID"]);
                        Quality = Convert.ToString(Dr["Quality"]);
                    }
                    Dr.Close();

                    //Get UOM, ALT===============================================================================
                    Query = "Select UoM From InventTable Where FullItemID = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string UoM = Cmd.ExecuteScalar().ToString();
                    decimal QtyUoM = 0, QtyAlt = 0;

                    if (Unit == UoM)
                    {
                        QtyUoM = Qty;
                        QtyAlt = Qty * Ratio;
                    }
                    else
                    {
                        QtyAlt = Qty;
                        QtyUoM = Qty / Ratio;
                    }
                    //End Get UOM, ALT==============================================================================

                    decimal TimbangWeight = 0;
                    TimbangWeight = Qty * Ratio;

                    decimal SubTotal = Convert.ToDecimal((Qty * Price).ToString("F4"));
                    decimal SubTotalPPN = Convert.ToDecimal(((Qty * Price) * (PPN / 100)).ToString("F4"));
                    decimal SubTotalPPH = Convert.ToDecimal(((Qty * Price) * (PPH / 100)).ToString("F4"));

                    //End Get Data

                    //Insert Header GoodsReceived
                    string Jenis = "GR", Kode = "BBMA";
                    string GRNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "INSERT INTO [GoodsReceivedH] ( ";
                    Query += "GoodsReceivedId, GoodsReceivedDate, GoodsReceivedStatus, RefTransID, RefTransDate, ";
                    Query += "SJDate, SJNumber, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, ";
                    Query += "Timbang1Weight, Timbang2Date, Timbang2Weight, SiteID, SiteName, SiteLocation, CreatedDate, ";
                    Query += "CreatedBy, StatusWeight1, StatusWeight2, RefTransType, NPPId) VALUES( ";
                    Query += "'" + GRNo + "', GETDATE(), '03', '" + ReceiptOrderId + "', '" + ReceiptOrderDate + "', ";
                    Query += "'" + SJDate + "', '" + SJNumber + "', '" + VendorID + "', '" + VendorName + "', '" + VehicleType + "', '" + VehicleNumber + "', '" + DriverName + "', ";
                    Query += "GETDATE(), '" + TimbangWeight + "', GETDATE(), '" + TimbangWeight + "', '" + SiteID + "', '" + SiteName + "', '" + SiteLocation + "', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', 'Manual', 'Manual', 'Receipt Order', '" + txtNotaNumber.Text + "') ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header GoodsReceived                   

                    //Insert Detail GoodsReceived
                    Query = "INSERT INTO GoodsReceivedD ( ";
                    Query += "GoodsReceivedDate, GoodsReceivedId, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, ";
                    Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_SJ, Qty_Actual, ";
                    Query += "Unit, Ratio, Ratio_Actual, InventSiteId, InventSiteBlokID, Quality, ActionCodeStatus, ";
                    Query += "CreatedDate, CreatedBy, DeliveryMethod, NPPId, NPP_SeqNo, Price, Total, PPN, Total_PPN, PPH, Total_PPH) ";
                    Query += "VALUES (GETDATE(), '" + GRNo + "', 1, '" + ReceiptOrderId + "', '" + ReceiptOrderSeqNo + "', ";
                    Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
                    Query += "" + Qty + ", " + Qty + ", " + Qty + ", '" + Unit + "', '" + Ratio + "', '" + Ratio + "', ";
                    Query += "'" + SiteID + "', '" + SiteBlokID + "', '" + Quality + "', '05', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "', '" + DeliveryMethod + "', '" + txtNotaNumber.Text + "',";
                    Query += "'" + NPPSeqNo + "', " + Price + ", " + (SubTotal) + ", " + PPN + ", " + SubTotalPPN + ", " + PPH + ", " + SubTotalPPH + " ";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail GoodsReceived

                    //Check Resize & Insert Nota Resize
                    string Resize = "";
                    decimal CountResize = 0;
                    Query = "SELECT TOP 1 Resize, ResizeType FROM InventTable WHERE FullItemID = '" + FullItemID + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        Resize = Convert.ToString(Dr["Resize"]);
                    }
                    Dr.Close();

                    string TransId = "";
                    if (Resize.ToUpper() == "TRUE")
                    {
                        Jenis = "NRZ"; Kode = "NRZA";
                        TransId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                        //Insert Header Nota Resize
                        Query = "INSERT INTO NotaResizeH ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                        Query += "[SiteID], [VendID], [Posted], [CreatedDate], [CreatedBy]) VALUES ( ";
                        Query += "'" + TransId + "', GETDATE(), GETDATE(), '" + GRNo + "', ";
                        Query += "'" + SiteID + "', '" + VendorID + "', 1, GETDATE(), '" + ControlMgr.UserId + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        //End Insert Header Nota Resize

                        Query = "SELECT COUNT(*) FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        CountResize = Convert.ToInt32(Cmd.ExecuteScalar().ToString());

                        //Insert Detail Nota Resize
                        if (CountResize > 1)
                        {
                            Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], ";
                            Query += "[Qty], [Price], [Unit], [LineAmount], [GoodsReceivedId], ";
                            Query += "[GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                            Query += "'" + TransId + "', (SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') + 1  END AS SeqNo), '" + FullItemID + "', '" + ItemName + "', ";
                            Query += "" + Qty + ", " + Price + ", '" + Unit + "', '" + SubTotal + "', '" + GRNo + "', ";
                            Query += "1, GETDATE(), '" + ControlMgr.UserId + "')";
                        }
                        else
                        {
                            Query = "INSERT INTO NotaResize_Dtl ([NRZId], [SeqNo], [FromFullItemId], [FromItemName], ";
                            Query += "[ToFullItemId], [ToItemName], [Qty], [Price], [Unit], [LineAmount], ";
                            Query += "[GoodsReceivedId], [GoodsReceiveSeqNo], [CreatedDate], [CreatedBy]) VALUES ( ";
                            Query += "'" + TransId + "', (SELECT CASE WHEN (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') IS NULL  THEN 1 ELSE (SELECT MAX(SeqNo) FROM NotaResize_Dtl WHERE NRZId = '" + TransId + "') + 1  END AS SeqNo), '" + FullItemID + "', '" + ItemName + "', ";
                            Query += "(SELECT To_FullItemId FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'), ";
                            Query += "(SELECT To_ItemName FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'), ";
                            Query += "" + Qty + ", " + Price + ", '" + Unit + "', '" + SubTotal + "', '" + GRNo + "', ";
                            Query += "1, GETDATE(), '" + ControlMgr.UserId + "'";
                            Query += ")";
                        }
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        //End Insert Detail Nota Resize
                    }
                    else
                    {
                        Query = "INSERT INTO InventTrans ([GroupId], [SubGroupId], [SubGroup2Id], [ItemId], [FullItemId], ";
                        Query += "[ItemName], [InventSiteId], [TransId], [SeqNo], [TransDate], [Ref_TransId], [Ref_TransDate], ";
                        Query += "[Ref_Trans_SeqNo], [AccountId], [AccountName], [Available_UoM], [Available_Alt], ";
                        Query += "[Available_Amount], [Available_For_Sale_UoM], [Available_For_Sale_Alt], [Available_For_Sale_Amount]) VALUES ( ";
                        Query += "'" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', ";
                        Query += "'" + ItemName + "', '" + SiteID + "', '" + GRNo + "', 1, GETDATE(), ";
                        Query += "'" + ReceiptOrderId + "', GETDATE(), 1, '" + VendorID + "', '" + VendorName + "', ";
                        Query += "" + QtyUoM + ", " + QtyAlt + ", " + SubTotal + "," + QtyUoM + ", " + QtyAlt + ", " + SubTotal + " ";
                        Query += ")";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    //End Check Resize & Insert Nota Resize 

                    //Insert Log
                    string StatusDesc = "";
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='03' AND TransCode = 'RO' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO ReceiptOrder_LogTable (ReceiptOrderDate, ReceiptOrderNo, PurchaseOrderNo, PurchaseOrderDate, VendorID, ";
                    Query += "InventSiteID, FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, GoodsReceivedId, ";
                    Query += "LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES (GETDATE(), '" + ReceiptOrderId + "', '" + PurchID + "', GETDATE(), '" + VendorID + "', ";
                    Query += "'" + SiteID + "', '" + FullItemID + "', '1', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '" + GRNo + "', ";
                    Query += "'03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='03' AND TransCode = 'GR' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "INSERT INTO GoodsReceived_LogTable (GoodsReceivedDate, GoodsReceivedId, ReceiptOrderNo, ReceiptOrderDate, VendorID, InventSiteID, ";
                    Query += "FullItemId, SeqNo, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES (GETDATE(), '" + GRNo + "', '" + ReceiptOrderId + "', GETDATE(), '" + VendorID + "', '" + SiteID + "', ";
                    Query += "'" + FullItemID + "', 1, " + QtyUoM + ", " + QtyAlt + ", " + (SubTotal) + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();



                    //Query = "SELECT Deskripsi FROM TransStatusTable ";
                    //Query += "WHERE StatusCode='03' AND TransCode = 'NotaPurchasePark' ";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //StatusDesc = Convert.ToString(Cmd.ExecuteScalar());

                    //Query = "INSERT INTO NotaPurchasePark_LogTable (NPPId, NPPDate, RefTransId, RefTransDate, AccountId, AccountName, ";
                    //Query += "RefTrans2Id, RefTrans2Date, InventSiteID, Qty_UoM, Qty_Alt, Amount, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    //Query += "VALUES ('" + txtNotaNumber.Text + "', '" + dtNotaDate.Value.ToString("yyyy-MM-dd") + "', '" + GRNo + "', GETDATE(), '" + VendorID + "', '" + VendorName + "', ";
                    //Query += "'" + ReceiptOrderId + "', GETDATE(), '" + SiteID + "', " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '03', '" + StatusDesc + "', '" + StatusDesc + "', '" + ControlMgr.UserId + "', GETDATE())";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteNonQuery();


                    if (Resize.ToUpper() == "TRUE")
                    {
                        if (CountResize > 1)
                        {
                            Query = "INSERT INTO NotaResize_LogTable ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                            Query += "[VendID], [InventSiteID], [FullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], ";
                            Query += "[Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                            Query += "'" + TransId + "', GETDATE(), GETDATE(), '" + GRNo + "', ";
                            Query += "'" + VendorID + "', '" + SiteID + "', '" + FullItemID + "',";
                            Query += "1, " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '01', ";
                            Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                            Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                            Query += "'" + ControlMgr.UserId + "', GETDATE())";

                        }
                        else
                        {
                            Query = "INSERT INTO NotaResize_LogTable ([NRZId], [NRZDate], [GoodsReceivedDate], [GoodsReceivedId], ";
                            Query += "[VendID], [InventSiteID], [FullItemId], [ToFullItemId], [SeqNo], [Qty_UoM], [Qty_Alt], ";
                            Query += "[Amount], [LogStatusCode], [LogStatusDesc], [LogDescription], [UserID], [LogDate]) VALUES( ";
                            Query += "'" + TransId + "', GETDATE(), GETDATE(), '" + GRNo + "', ";
                            Query += "'" + VendorID + "', '" + SiteID + "', '" + FullItemID + "', ";
                            Query += "(SELECT To_FullItemId FROM InventResize WHERE From_FullItemId = '" + FullItemID + "'), ";
                            Query += "1, " + QtyUoM + ", " + QtyAlt + ", " + SubTotal + ", '01', ";
                            Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                            Query += "(SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'NotaResize' AND StatusCode = '01'), ";
                            Query += "'" + ControlMgr.UserId + "', GETDATE()";
                            Query += ")";
                        }
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    //End Insert Log

                    //System Update
                    if (Resize.ToUpper() != "TRUE")
                    {
                        Query = "Update Invent_OnHand_Qty SET Available_For_Sale_UoM = (Available_For_Sale_UoM+" + QtyUoM + "), ";
                        Query += "Available_For_Sale_Alt = (Available_For_Sale_Alt+" + QtyAlt + "), Available_For_Sale_Amount = (Available_For_Sale_Amount+ " + SubTotal + ") ";
                        Query += "where FullItemID='" + FullItemID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Query = "Update Invent_Movement_Qty SET Resize_In_Progress_UoM = (Resize_In_Progress_UoM + " + QtyUoM + "), ";
                        Query += "Resize_In_Progress_Alt = (Resize_In_Progress_Alt + " + QtyAlt + "), ";
                        Query += "Resize_In_Progress_Amount = (Resize_In_Progress_Amount+ " + SubTotal + ") ";
                        Query += "where FullItemID='" + FullItemID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }

                    Query = "Update Invent_Movement_Qty SET Parked_For_Action_Outstanding_UoM = (Parked_For_Action_Outstanding_UoM- " + QtyUoM + "), ";
                    Query += "Parked_For_Action_Outstanding_Alt = (Parked_For_Action_Outstanding_Alt- " + QtyAlt + "), ";
                    Query += "Parked_For_Action_Outstanding_Amount = (Parked_For_Action_Outstanding_Amount-" + SubTotal + ") ";
                    Query += "where FullItemID='" + FullItemID + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End System Update
                }
            }
        }

        private void InsertLog(string TransaksiID, string TransCode, string StatusCode, string Action, SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            Query = "SELECT Deskripsi FROM TransStatusTable WHERE TransCode = '" + TransCode + "' AND StatusCode = '" + StatusCode + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string StatusTransaksi = Convert.ToString(Cmd.ExecuteScalar());

            if (Action == "")
            {
                Query = "SELECT TOP 1 Action FROM NotaPurchasePark_LogTable WHERE TransaksiID = '" + TransaksiID + "' ORDER BY LogDatetime DESC";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Action = Convert.ToString(Cmd.ExecuteScalar());
            }

            Query = "INSERT INTO NotaPurchasePark_LogTable (TransaksiID, StatusTransaksi, Action, UserID, LogDatetime) ";
            Query += "VALUES ('" + TransaksiID + "', '" + StatusTransaksi + "', '" + Action + "', '" + ControlMgr.UserId + "', GETDATE())";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
        }

        private void CreateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date : 09 Aug 2018
            //Desc : Create Journal

            int CountRetur = 0, CountResize = 0, CountAvailable = 0;
            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
                string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);

                Boolean ResizeType = GetResizeType(JournalFullItemID);

                if (JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
                {
                    if (ResizeType)
                    {
                        CountResize = CountResize + 1;
                    }
                    else
                    {
                        CountAvailable = CountAvailable + 1;
                    }
                }
                else
                {
                    CountRetur = CountRetur + 1;
                }                
            }


            string NPPNumber = txtNotaNumber.Text;
            string GRNo = txtGoodsReceivedNumber.Text;
            decimal Available = 0, Resize = 0, ReturDN = 0, ReturTB = 0, ReturKB = 0, Parked = 0;
            //decimal Tax = 0;
            //decimal POPPN = 0, POPPH = 0;
            string JournalHID = "";

            //POPPN = GetPOTax(GRNo, "PPN");
            //POPPH = GetPOTax(GRNo, "PPH");

            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
                string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
                decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
                decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);
                string JournalSeqNo = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value);

                if (JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
                {
                    Boolean ResizeType = GetResizeType(JournalFullItemID);
                    if (ResizeType)
                    {
                        Resize = Resize + (JournalPrice * JournalQty);

                        //if (POPPN != 0)
                        //{
                        //    Tax = Tax + ((Resize * POPPN) / 100);
                        //}

                        //if (POPPH != 0)
                        //{
                        //    Tax = Tax + ((Resize * POPPH) / 100);
                        //}

                        string POPrice = GetPOPrice(NPPNumber, JournalSeqNo);
                        if (POPrice == "")
                        {
                            POPrice = "1";
                        }

                        Parked = Parked + (Convert.ToDecimal(POPrice) * JournalQty);

                        //if (POPPN != 0)
                        //{
                        //    Tax = Tax + ((Parked * POPPN) / 100);
                        //}

                        //if (POPPH != 0)
                        //{
                        //    Tax = Tax + ((Parked * POPPH) / 100);
                        //}
                    }
                    else
                    {
                        Available = Available + (JournalPrice * JournalQty);

                        //if (POPPN != 0)
                        //{
                        //    Tax = Tax + ((Available * POPPN) / 100);
                        //}

                        //if (POPPH != 0)
                        //{
                        //    Tax = Tax + ((Available * POPPH) / 100);
                        //}

                        string POPrice = GetPOPrice(NPPNumber, JournalSeqNo);
                        if (POPrice == "")
                        {
                            POPrice = "1";
                        }

                        Parked = Parked + (Convert.ToDecimal(POPrice) * JournalQty);

                        //if (POPPN != 0)
                        //{
                        //    Tax = Tax + ((Parked * POPPN) / 100);
                        //}

                        //if (POPPH != 0)
                        //{
                        //    Tax = Tax + ((Parked * POPPH) / 100);
                        //}
                    }
                }
                else  if (JournalActionCode.ToUpper().Contains("RETUR TUKAR BARANG"))
                {
                    string POPrice = GetPOPrice(NPPNumber, JournalSeqNo);
                    if (POPrice == "")
                    {
                        POPrice = "1";
                    }

                    ReturTB = ReturTB + (Convert.ToDecimal(POPrice) * JournalQty);
                }
                else if (JournalActionCode.ToUpper().Contains("RETUR DEBIT"))
                {
                    string POPrice = GetPOPrice(NPPNumber, JournalSeqNo);
                    if (POPrice == "")
                    {
                        POPrice = "1";
                    }

                    ReturDN = ReturDN + (Convert.ToDecimal(POPrice) * JournalQty);
                }
                else if (JournalActionCode.ToUpper().Contains("RETUR KEMBALI BARANG"))
                {
                    string POPrice = GetPOPrice(NPPNumber, JournalSeqNo);
                    if (POPrice == "")
                    {
                        POPrice = "1";
                    }

                    ReturKB = ReturKB + (Convert.ToDecimal(POPrice) * JournalQty);
                }
            }

            if (ReturTB != 0 || ReturDN != 0 || ReturKB != 0 || Resize != 0 || Available != 0)
            {
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = "";
                string Notes = "";

                Notes = GetNotes();

                if (ReturTB != 0 || ReturDN != 0 || ReturKB != 0)
                {
                    JournalHID = "IN11";

                    //Retur Tukar Barang
                     //Get GLJournalHID
                    Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + ReturTBRefNumber + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

                    if (GLJournalHID != "")
                    {
                        Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountData == 1)
                        {
                            //Delete Journal Detail
                            Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else
                    {
                        //Retur Tukar Barang
                        GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);

                        Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                        Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + ReturTBRefNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        //Select Config Journal
                        int SeqNo = 1;
                        int JournalIDSeqNo = 0;
                        string Type = "";
                        string FQA_ID = "";
                        string FQA_Desc = "";
                        decimal AmountValue;

                        Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                            Type = Convert.ToString(Dr["Type"]);
                            FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                            FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                            AmountValue = 0;

                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = ReturTB;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = ReturTB;
                            }

                            if (AmountValue == 0)
                            {
                                continue;
                            }

                            //Insert Detail GLJournal
                            Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                            Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                            Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                            Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            SeqNo++;
                        }
                        Dr.Close();
                    }

                    //Retur Debet Note
                   //Get GLJournalHID
                    Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + ReturDNRefNumber + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

                    if (GLJournalHID != "")
                    {
                        Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountData == 1)
                        {
                            //Delete Journal Detail
                            Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else
                    {
                        //Retur Debet Note
                        GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);

                        Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                        Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + ReturDNRefNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        //Select Config Journal
                        int SeqNo = 1;
                        int JournalIDSeqNo = 0;
                        string Type = "";
                        string FQA_ID = "";
                        string FQA_Desc = "";
                        decimal AmountValue;

                        Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                            Type = Convert.ToString(Dr["Type"]);
                            FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                            FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                            AmountValue = 0;

                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = ReturDN;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = ReturDN;
                            }

                            if (AmountValue == 0)
                            {
                                continue;
                            }

                            //Insert Detail GLJournal
                            Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                            Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                            Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                            Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            SeqNo++;
                        }
                        Dr.Close();
                    }


                    //Retur Kembali Barang
                    //Get GLJournalHID
                    Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + ReturKBRefNumber + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

                    if (GLJournalHID != "")
                    {
                        Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountData == 1)
                        {
                            //Delete Journal Detail
                            Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else
                    {
                        //Retur Kembali Barang
                        GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);

                        Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                        Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + ReturKBRefNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        //Select Config Journal
                        int SeqNo = 1;
                        int JournalIDSeqNo = 0;
                        string Type = "";
                        string FQA_ID = "";
                        string FQA_Desc = "";
                        decimal AmountValue;

                        Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                            Type = Convert.ToString(Dr["Type"]);
                            FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                            FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                            AmountValue = 0;

                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = ReturKB;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = ReturKB;
                            }

                            if (AmountValue == 0)
                            {
                                continue;
                            }

                            //Insert Detail GLJournal
                            Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                            Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                            Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                            Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            SeqNo++;
                        }
                        Dr.Close();
                    }                    
                }

                if (Resize != 0 || Available != 0)
                {
                    JournalHID = "IN12";

                    //Get GLJournalHID
                    Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + GRARefNumber + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

                    if (GLJournalHID != "")
                    {
                        Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountData == 1)
                        {
                            //Delete Journal Detail
                            Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else
                    {
                        GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);

                        Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                        Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + GRARefNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        //Select Config Journal
                        int SeqNo = 1;
                        int JournalIDSeqNo = 0;
                        string Type = "";
                        string FQA_ID = "";
                        string FQA_Desc = "";
                        decimal AmountValue;

                        Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                            Type = Convert.ToString(Dr["Type"]);
                            FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                            FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                            AmountValue = 0;

                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = Parked;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = Resize;
                            }
                            else if (JournalIDSeqNo == 3)
                            {
                                AmountValue = Available;
                            }
                            else if (JournalIDSeqNo == 4)
                            {
                                AmountValue = Parked;
                            }
                            else if (JournalIDSeqNo == 5)
                            {
                                AmountValue = Resize + Available;
                            }

                            if (AmountValue == 0)
                            {
                                continue;
                            }

                            //Insert Detail GLJournal
                            Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                            Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                            Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                            Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            SeqNo++;
                        }
                        Dr.Close();
                    }                    
                }  
            }


            //if (DRetur != 0)
            //{
            //    JournalHID = "IN11";
            //}
            //else
            //{
            //    JournalHID = "IN12";
            //}

            //if (DRetur != 0 || DResize != 0 || DAvailable != 0)
            //{
            //    string Jenis = "JN", Kode = "JN";
            //    string GLJournalHID = ConnectionString.GenerateSeqID(9, Jenis, Kode, Conn, Trans, Cmd);

            //    Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy]) ";
            //    Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + txtNotaNumber.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "')";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Cmd.ExecuteNonQuery();

            //    //Select Config Journal
            //    int SeqNo = 1;
            //    int JournalIDSeqNo = 0;
            //    string Type = "";
            //    string FQA_ID = "";
            //    string FQA_Desc = "";
            //    decimal AmountValue = 0;

            //    Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Dr = Cmd.ExecuteReader();
            //    while (Dr.Read())
            //    {
            //        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
            //        Type = Convert.ToString(Dr["Type"]);
            //        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
            //        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);

            //        if (JournalHID == "IN11")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DResize;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = DResize + DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 3)
            //            {
            //                AmountValue = DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 4)
            //            {
            //                AmountValue = DRetur;
            //            }
            //            else if (JournalIDSeqNo == 5)
            //            {
            //                AmountValue = KResize + KAvailable;
            //            }
            //            else if (JournalIDSeqNo == 6)
            //            {
            //                AmountValue = KResize + KAvailable + KRetur;
            //            }
            //        }
            //        else if (JournalHID == "IN12")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DResize + DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = DResize;
            //            }
            //            else if (JournalIDSeqNo == 3)
            //            {
            //                AmountValue = DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 4)
            //            {
            //                AmountValue = KResize + KAvailable;
            //            }
            //            else if (JournalIDSeqNo == 5)
            //            {
            //                AmountValue = KResize + KAvailable;
            //            }
            //        }

            //        if (AmountValue == 0)
            //        {
            //            continue;
            //        }

            //        //Insert Detail GLJournal
            //        Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
            //        Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
            //        Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
            //        Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
            //        Cmd = new SqlCommand(Query, Conn, Trans);
            //        Cmd.ExecuteNonQuery();
            //        SeqNo++;
            //    }
            //    Dr.Close();
            //}
           

            //if (CountRetur != 0 && CountResize == 0 && CountAvailable == 0)
            //{
            //    JournalHID = "IN11";

            //    for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            //    {
            //        string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
            //        string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
            //        decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
            //        decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);

            //        if (!JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
            //        {
            //            DRetur = DRetur + (JournalPrice * JournalQty);
            //            KRetur = DRetur;                       
            //        }
            //    }
            //}
            //else if (CountRetur == 0 && CountResize != 0 && CountAvailable == 0)
            //{
            //    JournalHID = "IN12";

            //    for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            //    {
            //        string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
            //        string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
            //        decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
            //        decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);

            //        if (JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
            //        {
            //            Boolean Resize = GetResizeType(JournalFullItemID);
            //            if (Resize)
            //            {
            //                DResize = DResize + (JournalPrice * JournalQty);
            //                KResize = DResize;
            //            }                       
            //        }
            //    }
            //}
            //else if (CountRetur == 0 && CountResize == 0 && CountAvailable != 0)
            //{
            //    JournalHID = "IN13";

            //    for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            //    {
            //        string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
            //        string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
            //        decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
            //        decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);

            //        if (JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
            //        {
            //            Boolean Resize = GetResizeType(JournalFullItemID);
            //            if (!Resize)
            //            {
            //                DAvailable = DAvailable + (JournalPrice * JournalQty);
            //                KAvailable = DAvailable;
            //            }
            //        }
            //    }
            //}
            //else if (CountRetur != 0 && CountResize != 0 && CountAvailable == 0)
            //{
            //    JournalHID = "IN14";

            //    for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            //    {
            //        string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
            //        string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
            //        decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
            //        decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);

            //        if (JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
            //        {
            //            Boolean Resize = GetResizeType(JournalFullItemID);
            //            if (Resize)
            //            {
            //                DResize = DResize + (JournalPrice * JournalQty);
            //                KResize = DResize;
            //            }
            //        }
            //        else
            //        {
            //            DRetur = DRetur + (JournalPrice * JournalQty);
            //            KRetur = DRetur; 
            //        }
            //    }
            //}
            //else if (CountRetur != 0 && CountResize == 0 && CountAvailable != 0)
            //{
            //    JournalHID = "IN15";

            //    for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            //    {
            //        string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
            //        string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
            //        decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
            //        decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);

            //        if (JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
            //        {
            //            Boolean Resize = GetResizeType(JournalFullItemID);
            //            if (!Resize)
            //            {
            //                DAvailable = DAvailable + (JournalPrice * JournalQty);
            //                KAvailable = DAvailable;
            //            }
            //        }
            //        else
            //        {
            //            DRetur = DRetur + (JournalPrice * JournalQty);
            //            KRetur = DRetur;
            //        }
            //    }
            //}
            //else if (CountRetur == 0 && CountResize != 0 && CountAvailable != 0)
            //{
            //    JournalHID = "IN16";

            //    for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            //    {
            //        string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
            //        string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
            //        decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
            //        decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);

            //        if (JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
            //        {
            //            Boolean Resize = GetResizeType(JournalFullItemID);
            //            if (Resize)
            //            {
            //                DResize = DResize + (JournalPrice * JournalQty);
            //                KResize = DResize;
            //            }
            //            else
            //            {
            //                DAvailable = DAvailable + (JournalPrice * JournalQty);
            //                KAvailable = DAvailable;
            //            }
            //        }
            //    }
            //}
            //else if (CountRetur != 0 && CountResize != 0 && CountAvailable != 0)
            //{
            //    JournalHID = "IN17";

            //    for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            //    {
            //        string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
            //        string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
            //        decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
            //        decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);

            //        if (JournalActionCode.ToUpper().Contains("NEW PURCHASE"))
            //        {
            //            Boolean Resize = GetResizeType(JournalFullItemID);
            //            if (Resize)
            //            {
            //                DResize = DResize + (JournalPrice * JournalQty);
            //                KResize = DResize;
            //            }
            //            else
            //            {
            //                DAvailable = DAvailable + (JournalPrice * JournalQty);
            //                KAvailable = DAvailable;
            //            }
            //        }
            //        else
            //        {
            //            DRetur = DRetur + (JournalPrice * JournalQty);
            //            KRetur = DRetur;
            //        }
            //    }
            //}

            ////Insert Header GLJournal
            //string Jenis = "JN", Kode = "JN";
            //string GLJournalHID = ConnectionString.GenerateSeqID(9, Jenis, Kode, Conn, Trans, Cmd);

            //Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy]) ";
            //Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + txtNotaNumber.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "')";
            //Cmd = new SqlCommand(Query, Conn, Trans);
            //Cmd.ExecuteNonQuery();

            ////Select Config Journal
            //    int SeqNo = 1;
            //    int JournalIDSeqNo = 0;
            //    string Type = "";
            //    string FQA_ID = "";
            //    string FQA_Desc = "";
            //    decimal AmountValue = 0;

            //    Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Dr = Cmd.ExecuteReader();
            //    while (Dr.Read())
            //    {
            //        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
            //        Type = Convert.ToString(Dr["Type"]);
            //        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
            //        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);

            //        if (JournalHID == "IN11")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DRetur;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = KRetur;
            //            }
            //        }
            //        else if (JournalHID == "IN12")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DResize;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = DResize;
            //            }
            //            else if (JournalIDSeqNo == 3)
            //            {
            //                AmountValue = KResize;
            //            }
            //            else if (JournalIDSeqNo == 4)
            //            {
            //                AmountValue = KResize;
            //            }
            //        }
            //        else if (JournalHID == "IN13")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 3)
            //            {
            //                AmountValue = KAvailable;
            //            }
            //            else if (JournalIDSeqNo == 4)
            //            {
            //                AmountValue = KAvailable;
            //            }
            //        }
            //        else if (JournalHID == "IN14")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DRetur;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = DResize;
            //            }
            //            else if (JournalIDSeqNo == 3)
            //            {
            //                AmountValue = DResize;
            //            }
            //            else if (JournalIDSeqNo == 4)
            //            {
            //                AmountValue = KRetur + KResize;
            //            }
            //            else if (JournalIDSeqNo == 5)
            //            {
            //                AmountValue = KResize;
            //            }
            //        }
            //        else if (JournalHID == "IN15")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DRetur;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 3)
            //            {
            //                AmountValue = DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 4)
            //            {
            //                AmountValue = KAvailable;
            //            }
            //            else if (JournalIDSeqNo == 5)
            //            {
            //                AmountValue = KRetur;
            //            }
            //        }
            //        else if (JournalHID == "IN16")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DResize + DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = DResize;
            //            }
            //            else if (JournalIDSeqNo == 3)
            //            {
            //                AmountValue = DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 4)
            //            {
            //                AmountValue = KAvailable;
            //            }
            //            else if (JournalIDSeqNo == 5)
            //            {
            //                AmountValue = KResize + KAvailable;
            //            }
            //        }
            //        else if (JournalHID == "IN17")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = DResize + DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = DResize;
            //            }
            //            else if (JournalIDSeqNo == 3)
            //            {
            //                AmountValue = DRetur;
            //            }
            //            else if (JournalIDSeqNo == 4)
            //            {
            //                AmountValue = DAvailable;
            //            }
            //            else if (JournalIDSeqNo == 5)
            //            {
            //                AmountValue = KRetur + KAvailable + KResize;
            //            }
            //            else if (JournalIDSeqNo == 6)
            //            {
            //                AmountValue = KResize + KAvailable;
            //            }
            //        }

            //        //Insert Detail GLJournal
            //        Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
            //        Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
            //        Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
            //        Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
            //        Cmd = new SqlCommand(Query, Conn, Trans);
            //        Cmd.ExecuteNonQuery();
            //        SeqNo++;
            //    }
            //    Dr.Close();

            //string JournalHID = "IN11";
            //string Jenis = "GN", Kode = "GN";
            //string GLJournalHID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

            ////Insert Header GLJournal
            //Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy]) ";
            //Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + RBNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "')";
            //Cmd = new SqlCommand(Query, Conn, Trans);
            //Cmd.ExecuteNonQuery();

            ////Select Config Journal
            //int SeqNo = 1;
            //int JournalIDSeqNo = 0;
            //string Type = "";
            //string FQA_ID = "";
            //string FQA_Desc = "";

            //Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();
            //while (Dr.Read())
            //{
            //    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
            //    Type = Convert.ToString(Dr["Type"]);
            //    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
            //    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);

            //    //Insert Detail GLJournal
            //    Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
            //    Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
            //    Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
            //    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + SumAmount + ", GETDATE(), '" + ControlMgr.UserId + "')";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Cmd.ExecuteNonQuery();
            //    SeqNo++;
            //}
            //Dr.Close();
            //End

            //Begin
            //Created By : Joshua
            //Created Date ; 07 Aug 2018
            //Desc : Create Journal
            //string GLJournalID = ConnectionString.GenerateSequenceNo(10, "GLJournal", "GLJournalID", Conn, Trans, Cmd);
            //string JournalHID = "";
            //decimal Amount = 0;
            //for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            //{
            //    string JournalFullItemID = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["FullItemID"].Value);
            //    string JournalActionCode = Convert.ToString(dgvParkedItemDetails.Rows[i].Cells["ActionCode"].Value);
            //    decimal JournalPrice = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Price"].Value);
            //    decimal JournalQty = Convert.ToDecimal(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value);

            //    if (JournalActionCode.ToUpper() == "NEW PURCHASE")
            //    {
            //        JournalHID = "IN12";
            //        Amount = JournalQty * JournalPrice;

            //        InsertJournal(GLJournalID, JournalFullItemID, JournalHID, Amount);

            //        Query = "SELECT Resize FROM InventTable WHERE FullItemID = '" + JournalFullItemID + "' ";
            //        Cmd = new SqlCommand(Query, Conn, Trans);
            //        Boolean Resize = Convert.ToBoolean(Cmd.ExecuteScalar());

            //        if (Resize)
            //        {
            //            JournalHID = "IN13";
            //        }
            //        else
            //        {
            //            JournalHID = "IN14";
            //        }

            //        InsertJournal(GLJournalID, JournalFullItemID, JournalHID, Amount);

            //    }
            //}
            //End
        }

        private decimal GetPOTax(string GRNo, string FieldName)
        {
            //GET Tax
            Query = "SELECT PO." + FieldName + " FROM GoodsReceivedH GR INNER JOIN ReceiptOrderH RO ON GR.RefTransID = RO.ReceiptOrderId ";
            Query += "INNER JOIN PurchH PO ON PO.PurchID = RO.PurchaseOrderId WHERE GR.GoodsReceivedId = '" + GRNo + "' ";

            Cmd = new SqlCommand(Query, Conn, Trans);
            string result = Convert.ToString(Cmd.ExecuteScalar());
            decimal Tax = 0;
            if (result == "")
            {
                Tax = 0;
            }
            else
            {
                Tax = Convert.ToDecimal(result);
            }

            return Tax;
        }

        private bool GetResizeType(string FullItemID)
        {
            Query = "SELECT Resize FROM InventTable WHERE FullItemID = '" + FullItemID + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Boolean Resize = Convert.ToBoolean(Cmd.ExecuteScalar());

            return Resize;
        }

        private string GetPOPrice(string NPPNumber, string NPPSeqNo)
        {
            //GET PricePO
            Query = "SELECT PO.Price FROM NotaPurchaseParkD NP ";
            Query += "INNER JOIN GoodsReceivedD GR ON GR.GoodsReceivedId = NP.GoodsReceivedID ";
            Query += "INNER JOIN ReceiptOrderD RO ON RO.ReceiptOrderId = GR.RefTransID ";
            Query += "INNER JOIN PurchDtl PO  ON PO.PurchID = RO.PurchaseOrderId ";
            Query += "WHERE NP.NPPID = '" + NPPNumber + "' ";
            Query += "AND NP.SeqNo = " + NPPSeqNo + " ";
            Query += "AND NP.GoodsReceived_SeqNo = GR.GoodsReceivedSeqNo ";
            Query += "AND GR.RefTransSeqNo = RO.SeqNo ";
            Query += "AND RO.PurchaseOrderSeqNo = PO.SeqNo ";

            Cmd = new SqlCommand(Query, Conn, Trans);
            string Price = Convert.ToString(Cmd.ExecuteScalar());

            return Price;
        }

        private string GetPOPriceReceived(string ReceiptOrderId, string ReceiptOrder_SeqNo)
        {
            //GET PricePO
            Query = "SELECT Price FROM ReceiptOrderD WHERE ReceiptOrderId = '" + ReceiptOrderId + "' AND SeqNo = '" + ReceiptOrder_SeqNo + "'";

            Cmd = new SqlCommand(Query, Conn, Trans);
            string Price = Convert.ToString(Cmd.ExecuteScalar());

            return Price;
        }

        private string GetNotes()
        {
            //Get Vendor
            string result = "";
            Query = "SELECT TOP 1 VendId, VendorName FROM GoodsReceivedH WHERE GoodsReceivedId = '" + txtGoodsReceivedNumber.Text + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                string VendId = Convert.ToString(Dr["VendId"]);
                string VendorName = Convert.ToString(Dr["VendorName"]);

                result = VendId + " - " + VendorName;
            }
            Dr.Close();

            return result;
        }

        //private void InsertJournal(string GLJournalID, string FullItemID, string JournalHID, decimal Amount)
        //{ //SELECT Config Journal
        //    Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
        //    Cmd = new SqlCommand(Query, Conn, Trans);
        //    Dr = Cmd.ExecuteReader();
        //    while (Dr.Read())
        //    {
        //        if (Convert.ToString(Dr["Type"]).ToUpper() == "K")
        //        {
        //            Amount = Amount * -1;
        //        }

        //        //INSERT INTO GLJournal
        //        Query = "INSERT INTO [GLJournal]([GLJournalID],[SeqNo],[Referensi], [FullItemID],[JournalHID],[JournalHSeqNo],[Type],[FQA_ID] ";
        //        Query += ", [FQA_Desc],[Amount],[Posting] ,[Auto],[Status],[CreatedDate],[CreatedBy]) ";
        //        Query += "VALUES('" + GLJournalID + "', (SELECT CASE WHEN MAX(SeqNo) + 1 IS NULL THEN 1 ELSE MAX(SeqNo) + 1 END FROM GLJournal WHERE GLJournalID = '" + GLJournalID + "'), '" + txtNotaNumber.Text + "', '" + FullItemID + "', '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "'  ";
        //        Query += ", '" + Convert.ToString(Dr["Type"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', '" + Convert.ToString(Dr["FQA_Desc"]) + "' ";
        //        Query += "," + Amount + ", 0, 1, 'Gunakan', GETDATE(), '" + ControlMgr.UserId + "'  ) ";
        //        Cmd = new SqlCommand(Query, Conn, Trans);
        //        Cmd.ExecuteNonQuery();
        //    }
        //    Dr.Close();
        
        //}

        //Tia 26062018
        //klik kanan
        PopUp.Vendor.Vendor Vendor = null;
        PopUp.FullItemId.FullItemId FID = null;
        ISBS_New.Purchase.GoodsReceipt.GRHeaderV2 Gr = null;
        
        TaskList.Purchase.NotaPurchaseParked.TaskListNotaPurchaseParked ParentToTLNPP;
        public void SetParent(TaskList.Purchase.NotaPurchaseParked.TaskListNotaPurchaseParked pa)
        {
            ParentToTLNPP = pa;
        }

        public static string itemID;
       
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvParkedItemDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {

                if (FID == null || FID.Text == "")
                {
                    if (dgvParkedItemDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvParkedItemDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        //PopUpItemName.Close();
                        // PopUpItemName = new PopUp.Stock.Stock();
                        //PopUpItemName = new PopUp.FullItemId.FullItemId();
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvParkedItemDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvParkedItemDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvParkedItemDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }


            }
        }
        
        private bool CheckOpened(string name)
        {
            // FormCollection FC = Application.OpenForms;
            FormCollection FC = Application.OpenForms;
            foreach (Form frm in FC)
            {
                if (frm.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void txtVendorID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {

                    txtVendorID.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendorID.Text);

                    Vendor.Show();
                    //}
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void txtVendorName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendorID.Text);

                    Vendor.Show();
                    //}
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void txtGoodsReceivedNumber_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Gr == null || Gr.Text == "")
                {
                    Gr = new Purchase.GoodsReceipt.GRHeaderV2("Receipt Order");
                    Gr.SetMode("PopUp", txtGoodsReceivedNumber.Text);
                    Gr.ParentRefreshGrid(this);
                    Gr.Show();
                    //}
                }
                else if (CheckOpened(Gr.Name))
                {
                    Gr.WindowState = FormWindowState.Normal;
                    Gr.Show();
                    Gr.Focus();
                }
            }
        }

        //private void txtGoodsReceivedNumber_MouseDown(object sender, MouseEventArgs e)
        //{
           
        //}
        //End

    }
}
