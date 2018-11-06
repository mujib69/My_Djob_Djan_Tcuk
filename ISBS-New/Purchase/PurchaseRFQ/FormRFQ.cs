using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseQuotation
{
    public partial class FormPQ : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query, crit = null;

        int Index;

        public string PQNumber = "", tmpPrType = "";

        DateTimePicker dtp;

        List<string> ListSeqNum;
        List<Purchase.PurchaseRequisition.Info> ListInfo = new List<Purchase.PurchaseRequisition.Info>();

        Purchase.PurchaseQuotation.InquiryPQ Parent;

        public FormPQ()
        {
            InitializeComponent();
        }

        private void FormPQ_Load(object sender, EventArgs e)
        {
            lblForm.Location = new Point(16, 11);
            GetDataHeader();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "yyyy-MM-dd";
            dtp.Visible = false;
            dtp.Width = 100;

            dgvPqDetails.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;
            dgvPqDetails.CellBeginEdit += this.dgvPrDetails_CellBeginEdit;
            dgvPqDetails.CellEndEdit += this.dgvPrDetails_CellEndEdit;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (txtPrNumber.Text.Trim() == "")
            {
                MessageBox.Show("Silahkan pilih PR Number terlebih dahulu.");
            }
            else
            {
                DetailPQ DetailPQ = new DetailPQ();

                List<DetailPQ> ListDetailPQ = new List<DetailPQ>();
                DetailPQ.SetParent(this);
                DetailPQ.SetPrNumber(txtPrNumber.Text);
                DetailPQ.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPqDetails.RowCount > 0)
            {
                Index = dgvPqDetails.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvPqDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvPqDetails.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvPqDetails.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvPqDetails.Rows.RemoveAt(Index);
                    SortNoDataGrid();
                }
            }
        }

        public void SetMode(string tmpMode, string tmpPQNumber)
        {
            Mode = tmpMode;
            PQNumber = tmpPQNumber;
        }

        private void btnEditH_Click(object sender, EventArgs e)
        {
            string Check = "";
            Conn = ConnectionString.GetConnection();

            Query = "Select TransStatus from [dbo].[PurchQuotationH] where [PurchQuotID]='" + txtPqNumber.Text + "';";
            Cmd = new SqlCommand(Query, Conn);
            Check = Cmd.ExecuteScalar().ToString();
            if (Check == "22")
            {
                MessageBox.Show("PurchQuotID = " + txtPqNumber.Text + ".\n" + "Tidak bisa diedit karena sudah diproses.");
                Conn.Close();
                return;
            }

            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            dtPqDate.Enabled = false;
            txtVendorPqNumber.Enabled = true;

            ModeEdit();
        }

        private void dgvPrDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPqDetails.Columns[dgvPqDetails.CurrentCell.ColumnIndex].Name == "Price")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
            }
            if (dgvPqDetails.Columns[dgvPqDetails.CurrentCell.ColumnIndex].Name == "DiscScheme")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
            }
            if (dgvPqDetails.Columns[dgvPqDetails.CurrentCell.ColumnIndex].Name == "CashBackScheme")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
            }
            if (dgvPqDetails.Columns[dgvPqDetails.CurrentCell.ColumnIndex].Name == "SubTotal")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
            }
            if (dgvPqDetails.Columns[dgvPqDetails.CurrentCell.ColumnIndex].Name == "SubTotal_PPN")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
            }
            if (dgvPqDetails.Columns[dgvPqDetails.CurrentCell.ColumnIndex].Name == "SubTotal_PPH")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
            }
        }

        public void AddDataGridDetail(List<string> FullItemID)
        {
            if (dgvPqDetails.RowCount - 1 <= 0)
            {
                dgvPqDetails.ColumnCount = 29;
                dgvPqDetails.Columns[0].Name = "No";
                dgvPqDetails.Columns[1].Name = "GroupId";
                dgvPqDetails.Columns[2].Name = "SubGroup1ID";
                dgvPqDetails.Columns[3].Name = "SubGroup2ID";
                dgvPqDetails.Columns[4].Name = "ItemID";
                dgvPqDetails.Columns[5].Name = "FullItemID";
                dgvPqDetails.Columns[6].Name = "ItemName";
                dgvPqDetails.Columns[7].Name = "Qty";
                dgvPqDetails.Columns[8].Name = "Unit";
                dgvPqDetails.Columns[9].Name = "Price";
                dgvPqDetails.Columns[10].Name = "Qty2";
                dgvPqDetails.Columns[11].Name = "Unit2";
                dgvPqDetails.Columns[12].Name = "AvailableDate";
                dgvPqDetails.Columns[13].Name = "DeliveryMethod";
                dgvPqDetails.Columns[14].Name = "ReffTransID";
                dgvPqDetails.Columns[15].Name = "ReffSeqNo";
                dgvPqDetails.Columns[16].Name = "ReffTransType";
                dgvPqDetails.Columns[17].Name = "TransStatus";
                dgvPqDetails.Columns[18].Name = "Deskripsi";
                dgvPqDetails.Columns[19].Name = "DiscScheme";
                dgvPqDetails.Columns[20].Name = "BonusScheme";
                dgvPqDetails.Columns[21].Name = "CashBackScheme";
                dgvPqDetails.Columns[22].Name = "SubTotal";
                dgvPqDetails.Columns[23].Name = "SubTotal_PPN";
                dgvPqDetails.Columns[24].Name = "SubTotal_PPH";
                dgvPqDetails.Columns[25].Name = "D1";
                dgvPqDetails.Columns[26].Name = "D2";
                dgvPqDetails.Columns[27].Name = "D3";
                dgvPqDetails.Columns[28].Name = "D4";
            }

            for (int i = 0; i < FullItemID.Count; i++)
            {
                Conn = ConnectionString.GetConnection();

                Query = "Select GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, [Qty], [Unit], a.PurchReqId, SeqNo, TransType, b.TransStatus, Deskripsi From [PurchRequisition_Dtl] a inner join PurchRequisitionH b on a.PurchReqId=b.PurchReqId Where a.FullItemID = '" + FullItemID[i] + "' and a.PurchReqId='" + txtPrNumber.Text.Trim() + "' order by SeqNo asc";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    this.dgvPqDetails.Rows.Add(dgvPqDetails.Rows.Count+1, Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], "", "", "", "", "", Dr[8], Dr[9], Dr[10], Dr[11], Dr[12], "", "", "", "", "", "");

                    //Conn = ConnectionString.GetConnection();
                    Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + Dr[4].ToString() + "' ";
                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader Dr1;
                    Dr1 = Cmd.ExecuteReader();
                    DataGridViewComboBoxCell combo1 = new DataGridViewComboBoxCell();
                    while (Dr1.Read())
                    {
                        combo1.Items.Add(Dr1[0].ToString());
                        combo1.Items.Add(Dr1[1].ToString());
                    }
                    dgvPqDetails.Rows[(dgvPqDetails.Rows.Count - 1)].Cells[11] = combo1;

                    Conn = ConnectionString.GetConnection();
                    Query = "Select DeliveryMethod FROM [dbo].[DeliveryMethod] ";
                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader Dr2;
                    Dr2 = Cmd.ExecuteReader();
                    DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
                    while (Dr2.Read())
                    {
                        combo2.Items.Add(Dr2[0].ToString());
                    }
                    dgvPqDetails.Rows[(dgvPqDetails.Rows.Count - 1)].Cells[13] = combo2;

                    dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[25].Value = "...";
                    dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[26].Value = "...";
                    dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[27].Value = "...";
                    dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[28].Value = "...";

                    i++;
                }
                Dr.Close();
            }
            dgvPqDetails.ReadOnly = true;

            dgvPqDetails.ReadOnly = false;
            dgvPqDetails.Columns["No"].ReadOnly = true;
            dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPqDetails.Columns["ItemName"].ReadOnly = true;
            dgvPqDetails.Columns["Qty"].ReadOnly = true;
            dgvPqDetails.Columns["Unit"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal_PPN"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal_PPH"].ReadOnly = true;
            dgvPqDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubGroup1ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubGroup2ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Qty2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Unit2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["AvailableDate"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffTransID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffSeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffTransType"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["TransStatus"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["DiscScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["BonusScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["CashBackScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal_PPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal_PPH"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPqDetails.Columns["GroupId"].Visible = false;
            dgvPqDetails.Columns["SubGroup1ID"].Visible = false;
            dgvPqDetails.Columns["SubGroup2ID"].Visible = false;
            dgvPqDetails.Columns["ItemID"].Visible = false;
            dgvPqDetails.Columns["ReffTransID"].Visible = false;
            dgvPqDetails.Columns["ReffSeqNo"].Visible = false;
            dgvPqDetails.Columns["ReffTransType"].Visible = false;
            dgvPqDetails.Columns["TransStatus"].Visible = false;

            dgvPqDetails.AutoResizeColumns();

            ListSeqNum = FullItemID;
        }


        private void CallFormGelombang(List<string> InvStockId)
        {
            if (InvStockId != null)
            {
                if (InvStockId.Count != 0)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "Select count([GelombangId]) From [InventGelombang] Where GelombangId in (Select GelombangId from InventGelombang where ItemId in (";
                    for (var i = 0; i < InvStockId.Count; i++)
                    {
                        if (i == 0)
                        {
                            Query += "'" + InvStockId[i] + "'";
                        }
                        else
                        {
                            Query += ",'" + InvStockId[i] + "'";
                        }
                    }
                    Query += "))";

                    Cmd = new SqlCommand(Query, Conn);
                    int CountChk = Convert.ToInt32(Cmd.ExecuteScalar());

                    if (CountChk > 0)
                    {
                       
                    }
                }
            }
        }

        public string DetailHeaderItem()
        {
            string InvStockId = "";

            if (dgvPqDetails.RowCount > 0)
            {
                for (int i = 0; i <= dgvPqDetails.RowCount - 1; i++)
                {
                    if (i == 0)
                    {
                        InvStockId += "and FullItemId not in ('";
                        InvStockId += dgvPqDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPqDetails.Rows[i].Cells["FullItemId"].Value.ToString();
                        InvStockId += "'";
                    }
                    else
                    {
                        InvStockId += ",'";
                        InvStockId += dgvPqDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPqDetails.Rows[i].Cells["FullItemId"].Value.ToString();
                        InvStockId += "'";
                    }
                }
                InvStockId += ")";
                return InvStockId;
            }
            else
            {
                InvStockId = "";
                return InvStockId;
            }
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvPqDetails.RowCount ; i++)
            {
                dgvPqDetails.Rows[i].Cells["No"].Value = i+1;
            }
        }

        public void ModeNew()
        {
            PQNumber = "";
            dtVendorPqDate.Value = Convert.ToDateTime("01-01-1990");

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            btnSearchPR.Enabled = true;
            btnSearchVendor.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            txtVendorPqNumber.Enabled = true;
            dtVendorPqDate.Enabled = true;
            txtVendorId.Enabled = true;
            txtVendorName.Enabled = true;

            cmbPaymentMode.Enabled = true;
            txtPPN.Enabled = true;
            txtPPH.Enabled = true;
            txtTermOfPayment.Enabled = true;
            txtDiscScheme.Enabled = true;
            txtCashBackScheme.Enabled = true;
            txtTotal.Enabled = true;
            txtTotalPPN.Enabled = true;
            txtTotalPPH.Enabled = true;
            txtDeskripsi.Enabled = true;

        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnSearchPR.Enabled = true;
            btnSearchVendor.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            txtVendorPqNumber.Enabled = true;
            dtVendorPqDate.Enabled = true;
            txtVendorId.Enabled = true;
            txtVendorName.Enabled = true;

            cmbPaymentMode.Enabled = true;
            txtPPN.Enabled = true;
            txtPPH.Enabled = true;
            txtTermOfPayment.Enabled = true;
            txtDiscScheme.Enabled = true;
            txtCashBackScheme.Enabled = true;
            txtTotal.Enabled = true;
            txtTotalPPN.Enabled = true;
            txtTotalPPH.Enabled = true;
            txtDeskripsi.Enabled = true;

            dgvPqDetails.ReadOnly = false;
            dgvPqDetails.Columns["No"].ReadOnly = true;
            dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPqDetails.Columns["ItemName"].ReadOnly = true;
            dgvPqDetails.Columns["Qty"].ReadOnly = true;
            dgvPqDetails.Columns["Unit"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal_PPN"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal_PPH"].ReadOnly = true;
            dgvPqDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubGroup1ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubGroup2ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffTransID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffSeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffTransType"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["TransStatus"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["BonusScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["CashBackScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal_PPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal_PPH"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPqDetails.Columns["GroupId"].Visible = false;
            dgvPqDetails.Columns["SubGroup1ID"].Visible = false;
            dgvPqDetails.Columns["SubGroup2ID"].Visible = false;
            dgvPqDetails.Columns["ItemID"].Visible = false;
            dgvPqDetails.Columns["ReffTransID"].Visible = false;
            dgvPqDetails.Columns["ReffSeqNo"].Visible = false;
            dgvPqDetails.Columns["ReffTransType"].Visible = false;
            dgvPqDetails.Columns["TransStatus"].Visible = false;

            dgvPqDetails.AutoResizeColumns();
            dgvPqDetails.DefaultCellStyle.BackColor = Color.White;
           
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            btnSearchPR.Enabled = false;
            btnSearchVendor.Enabled = false;

            txtVendorPqNumber.Enabled = false;
            dtVendorPqDate.Enabled = false;
            txtVendorId.Enabled = false;
            txtVendorName.Enabled = false;

            cmbPaymentMode.Enabled = false;
            txtPPN.Enabled = false;
            txtPPH.Enabled = false;
            txtTermOfPayment.Enabled = false;
            txtDiscScheme.Enabled = false;
            txtCashBackScheme.Enabled = false;
            txtTotal.Enabled = false;
            txtTotalPPN.Enabled = false;
            txtTotalPPH.Enabled = false;
            txtDeskripsi.Enabled = false;

            dgvPqDetails.ReadOnly = true;
            dgvPqDetails.DefaultCellStyle.BackColor = Color.LightGray;
        }

        public void GetDataHeader()
        {
            if(PQNumber != "")
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select Top 1 a.[PurchQuotID],a.[OrderDate],a.[VendorQuotNo],a.[VendorQuotDate],a.[VendID],a.VendName,a.TransStatus,a.ApprovedBy,b.[ReffTransID],a.TermOfPayment,a.PaymentModeId,a.PPN,a.PPH,a.Deskripsi,a.Total,a.Total_PPN,a.Total_PPH From [PurchQuotationH] a left join PurchQuotation_Dtl b on a.PurchQuotID=b.PurchQuotID Where a.PurchQuotID = '" + PQNumber + "'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtPqDate.Text = Dr["OrderDate"].ToString();
                    txtPqNumber.Text = Dr["PurchQuotID"].ToString();
                    txtPrNumber.Text = Dr["ReffTransID"].ToString();
                    txtVendorPqNumber.Text = Dr["VendorQuotNo"].ToString();
                    dtVendorPqDate.Text = Dr["VendorQuotDate"].ToString();
                    txtVendorId.Text = Dr["VendID"].ToString();
                    txtVendorName.Text = Dr["VendName"].ToString();
                    txtTermOfPayment.Text = Dr["TermOfPayment"].ToString();
                    cmbPaymentMode.SelectedItem = Dr["PaymentModeId"].ToString();
                    txtPPN.Text = Dr["PPN"].ToString();
                    txtPPH.Text = Dr["PPH"].ToString();
                    txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                    txtTotal.Text = Dr["Total"].ToString();
                    txtTotalPPN.Text = Dr["Total_PPN"].ToString();
                    txtTotalPPH.Text = Dr["Total_PPH"].ToString();
                }
                Dr.Close();

                dgvPqDetails.Rows.Clear();
                if (dgvPqDetails.RowCount - 1 <= 0)
                {
                    dgvPqDetails.ColumnCount = 29;
                    dgvPqDetails.Columns[0].Name = "No";
                    dgvPqDetails.Columns[1].Name = "GroupId";
                    dgvPqDetails.Columns[2].Name = "SubGroup1ID";
                    dgvPqDetails.Columns[3].Name = "SubGroup2ID";
                    dgvPqDetails.Columns[4].Name = "ItemID";
                    dgvPqDetails.Columns[5].Name = "FullItemID";
                    dgvPqDetails.Columns[6].Name = "ItemName";
                    dgvPqDetails.Columns[7].Name = "Qty";
                    dgvPqDetails.Columns[8].Name = "Unit";
                    dgvPqDetails.Columns[9].Name = "Price";
                    dgvPqDetails.Columns[10].Name = "Qty2";
                    dgvPqDetails.Columns[11].Name = "Unit2";
                    dgvPqDetails.Columns[12].Name = "AvailableDate";
                    dgvPqDetails.Columns[13].Name = "DeliveryMethod";
                    dgvPqDetails.Columns[14].Name = "ReffTransID";
                    dgvPqDetails.Columns[15].Name = "ReffSeqNo";
                    dgvPqDetails.Columns[16].Name = "ReffTransType";
                    dgvPqDetails.Columns[17].Name = "TransStatus";
                    dgvPqDetails.Columns[18].Name = "Deskripsi";
                    dgvPqDetails.Columns[19].Name = "DiscScheme";
                    dgvPqDetails.Columns[20].Name = "BonusScheme";
                    dgvPqDetails.Columns[21].Name = "CashBackScheme";
                    dgvPqDetails.Columns[22].Name = "SubTotal";
                    dgvPqDetails.Columns[23].Name = "SubTotal_PPN";
                    dgvPqDetails.Columns[24].Name = "SubTotal_PPH";
                    dgvPqDetails.Columns[25].Name = "D1";
                    dgvPqDetails.Columns[26].Name = "D2";
                    dgvPqDetails.Columns[27].Name = "D3";
                    dgvPqDetails.Columns[28].Name = "D4";
                }

                Query = "Select SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, [Qty], [Unit], [Price], [Qty2], [Unit2], [AvailableDate], [DeliveryMethod], ReffTransId, ReffSeqNo, ReffTransType, TransStatus, Deskripsi, DiscScheme, BonusScheme, CashBackScheme, SubTotal, SubTotal_PPN, SubTotal_PPH From [PurchQuotation_Dtl] Where PurchQuotID = '" + PQNumber + "' order by SeqNo asc";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    this.dgvPqDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Convert.ToDateTime(Dr[12]).ToString("dd-MM-yyyy"), Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23], Dr[24]);

                    Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + Dr[5].ToString() + "' ";
                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader Dr1;
                    Dr1 = Cmd.ExecuteReader();
                    DataGridViewComboBoxCell combo1 = new DataGridViewComboBoxCell();
                    while (Dr1.Read())
                    {
                        combo1.Items.Add(Dr1[0].ToString());
                        combo1.Items.Add(Dr1[1].ToString());
                    }
                    if (Dr[11] != null)
                    {
                        combo1.Value = Dr[11].ToString();
                    }
                    dgvPqDetails.Rows[(dgvPqDetails.Rows.Count - 1)].Cells[11] = combo1;
                    

                    Query = "Select DeliveryMethod FROM [dbo].[DeliveryMethod] ";
                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader Dr2;
                    Dr2 = Cmd.ExecuteReader();
                    DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
                    while (Dr2.Read())
                    {
                        combo2.Items.Add(Dr2[0].ToString());
                    }
                    if (Dr[13] != null)
                    {
                        combo2.Value = Dr[13].ToString();
                    }
                    dgvPqDetails.Rows[(dgvPqDetails.Rows.Count - 1)].Cells[13] = combo2;

                    dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[25].Value = "...";
                    dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[26].Value = "...";
                    dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[27].Value = "...";
                    dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[28].Value = "...";

                }
                Dr.Close();

                dgvPqDetails.ReadOnly = false;
                dgvPqDetails.Columns["No"].ReadOnly = true;
                dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
                dgvPqDetails.Columns["ItemName"].ReadOnly = true;
                dgvPqDetails.Columns["Qty"].ReadOnly = true;
                dgvPqDetails.Columns["Unit"].ReadOnly = true;
                dgvPqDetails.Columns["SubTotal"].ReadOnly = true;
                dgvPqDetails.Columns["SubTotal_PPN"].ReadOnly = true;
                dgvPqDetails.Columns["SubTotal_PPH"].ReadOnly = true;
                dgvPqDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["SubGroup1ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["SubGroup2ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["ReffTransID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["ReffSeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["ReffTransType"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["TransStatus"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["BonusScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["CashBackScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["SubTotal"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["SubTotal_PPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["SubTotal_PPH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvPqDetails.Columns["GroupId"].Visible = false;
                dgvPqDetails.Columns["SubGroup1ID"].Visible = false;
                dgvPqDetails.Columns["SubGroup2ID"].Visible = false;
                dgvPqDetails.Columns["ItemID"].Visible = false;
                dgvPqDetails.Columns["ReffTransID"].Visible = false;
                dgvPqDetails.Columns["ReffSeqNo"].Visible = false;
                dgvPqDetails.Columns["ReffTransType"].Visible = false;
                dgvPqDetails.Columns["TransStatus"].Visible = false;

                dgvPqDetails.AutoResizeColumns();

                Conn.Close();
            }
        }

        public void GetDataHeader(string PQNumber)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select Top 1 a.[PurchQuotID],a.[OrderDate],a.[VendorQuotNo],a.[VendorQuotDate],a.[VendID],a.VendName,a.TransStatus,a.ApprovedBy,b.[ReffTransID],a.TermOfPayment,a.PaymentModeId,a.PPN,a.PPH,a.Deskripsi,a.Total,a.Total_PPN,a.Total_PPH From [PurchQuotationH] a left join PurchQuotation_Dtl b on a.PurchQuotID=b.PurchQuotID Where a.PurchQuotID = '" + PQNumber + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtPqDate.Text = Dr["OrderDate"].ToString();
                txtPqNumber.Text = Dr["PurchQuotID"].ToString();
                txtPrNumber.Text = Dr["ReffTransID"].ToString();
                txtVendorPqNumber.Text = Dr["VendorQuotNo"].ToString();
                dtVendorPqDate.Text = Dr["VendorQuotDate"].ToString();
                txtVendorId.Text = Dr["VendID"].ToString();
                txtVendorName.Text = Dr["VendName"].ToString();
                txtTermOfPayment.Text = Dr["TermOfPayment"].ToString();
                cmbPaymentMode.SelectedItem = Dr["PaymentModeId"].ToString();
                txtPPN.Text = Dr["PPN"].ToString();
                txtPPH.Text = Dr["PPH"].ToString();
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                txtTotal.Text = Dr["Total"].ToString();
                txtTotalPPN.Text = Dr["Total_PPN"].ToString();
                txtTotalPPH.Text = Dr["Total_PPH"].ToString();
            }
            Dr.Close();

            dgvPqDetails.Rows.Clear();
            if (dgvPqDetails.RowCount - 1 <= 0)
            {
                dgvPqDetails.ColumnCount = 29;
                dgvPqDetails.Columns[0].Name = "No";
                dgvPqDetails.Columns[1].Name = "GroupId";
                dgvPqDetails.Columns[2].Name = "SubGroup1ID";
                dgvPqDetails.Columns[3].Name = "SubGroup2ID";
                dgvPqDetails.Columns[4].Name = "ItemID";
                dgvPqDetails.Columns[5].Name = "FullItemID";
                dgvPqDetails.Columns[6].Name = "ItemName";
                dgvPqDetails.Columns[7].Name = "Qty";
                dgvPqDetails.Columns[8].Name = "Unit";
                dgvPqDetails.Columns[9].Name = "Price";
                dgvPqDetails.Columns[10].Name = "Qty2";
                dgvPqDetails.Columns[11].Name = "Unit2";
                dgvPqDetails.Columns[12].Name = "AvailableDate";
                dgvPqDetails.Columns[13].Name = "DeliveryMethod";
                dgvPqDetails.Columns[14].Name = "ReffTransID";
                dgvPqDetails.Columns[15].Name = "ReffSeqNo";
                dgvPqDetails.Columns[16].Name = "ReffTransType";
                dgvPqDetails.Columns[17].Name = "TransStatus";
                dgvPqDetails.Columns[18].Name = "Deskripsi";
                dgvPqDetails.Columns[19].Name = "DiscScheme";
                dgvPqDetails.Columns[20].Name = "BonusScheme";
                dgvPqDetails.Columns[21].Name = "CashBackScheme";
                dgvPqDetails.Columns[22].Name = "SubTotal";
                dgvPqDetails.Columns[23].Name = "SubTotal_PPN";
                dgvPqDetails.Columns[24].Name = "SubTotal_PPH";
                dgvPqDetails.Columns[25].Name = "D1";
                dgvPqDetails.Columns[26].Name = "D2";
                dgvPqDetails.Columns[27].Name = "D3";
                dgvPqDetails.Columns[28].Name = "D4";
            }

            Query = "Select SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, [Qty], [Unit], [Price], [Qty2], [Unit2], [AvailableDate], [DeliveryMethod], ReffTransId, ReffSeqNo, ReffTransType, TransStatus, Deskripsi, DiscScheme, BonusScheme, CashBackScheme, SubTotal, SubTotal_PPN, SubTotal_PPH From [PurchQuotation_Dtl] Where PurchQuotID = '" + PQNumber + "' order by SeqNo asc";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                this.dgvPqDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Convert.ToDateTime(Dr[12]).ToString("dd-MM-yyyy"), Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23], Dr[24]);

                Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + Dr[5].ToString() + "' ";
                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader Dr1;
                Dr1 = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo1 = new DataGridViewComboBoxCell();
                while (Dr1.Read())
                {
                    combo1.Items.Add(Dr1[0].ToString());
                    combo1.Items.Add(Dr1[1].ToString());
                }
                if (Dr[11] != null)
                {
                    combo1.Value = Dr[11].ToString();
                }
                dgvPqDetails.Rows[(dgvPqDetails.Rows.Count - 1)].Cells[11] = combo1;


                Query = "Select DeliveryMethod FROM [dbo].[DeliveryMethod] ";
                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader Dr2;
                Dr2 = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
                while (Dr2.Read())
                {
                    combo2.Items.Add(Dr2[0].ToString());
                }
                if (Dr[13] != null)
                {
                    combo2.Value = Dr[13].ToString();
                }
                dgvPqDetails.Rows[(dgvPqDetails.Rows.Count - 1)].Cells[13] = combo2;

                dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[25].Value = "...";
                dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[26].Value = "...";
                dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[27].Value = "...";
                dgvPqDetails.Rows[dgvPqDetails.Rows.Count - 1].Cells[28].Value = "...";

            }
            Dr.Close();


            dgvPqDetails.ReadOnly = false;
            dgvPqDetails.Columns["No"].ReadOnly = true;
            dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPqDetails.Columns["ItemName"].ReadOnly = true;
            dgvPqDetails.Columns["Qty"].ReadOnly = true;
            dgvPqDetails.Columns["Unit"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal_PPN"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal_PPH"].ReadOnly = true;
            dgvPqDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubGroup1ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubGroup2ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Qty2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Unit2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["AvailableDate"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffTransID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffSeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffTransType"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["TransStatus"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["BonusScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["CashBackScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal_PPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal_PPH"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPqDetails.Columns["GroupId"].Visible = false;
            dgvPqDetails.Columns["SubGroup1ID"].Visible = false;
            dgvPqDetails.Columns["SubGroup2ID"].Visible = false;
            dgvPqDetails.Columns["ItemID"].Visible = false;
            dgvPqDetails.Columns["ReffTransID"].Visible = false;
            dgvPqDetails.Columns["ReffSeqNo"].Visible = false;
            dgvPqDetails.Columns["ReffTransType"].Visible = false;
            dgvPqDetails.Columns["TransStatus"].Visible = false;

            dgvPqDetails.AutoResizeColumns();

            Conn.Close();
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;

            //Validasi Items
            if (dgvPqDetails.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }
            else if (txtVendorId.Text.Trim() == "" || txtVendorName.Text.Trim() == "")
            {
                MessageBox.Show("Vendor Id tidak boleh kosong.");
                return;
            }
            //else if (txtVendorPqNumber.Text.Trim() == "")
            //{
            //    MessageBox.Show("Vendor Quote Number tidak boleh kosong.");
            //    return;
            //}
            //else
            //{
            //    for (int i = 0; i <= dgvPqDetails.RowCount - 1; i++)
            //    {
            //        if (Convert.ToDecimal((dgvPqDetails.Rows[i].Cells["Price"].Value == "" ? "0.000" : dgvPqDetails.Rows[i].Cells["Price"].Value.ToString())) <= 0)
            //        {
            //            MessageBox.Show("Item No = " + dgvPqDetails.Rows[i].Cells["No"].Value + ", Price tidak boleh lebih kecil atau sama dengan 0");
            //            return;
            //        }
            //    }
            //}
        
            try
            {
                if (Mode == "New" && txtPqNumber.Text.Trim() == "")
                {
                    Query = "Insert into PurchQuotationH (PurchQuotID,OrderDate,VendorQuotNo,VendorQuotDate,VendID,VendName,TermOfPayment,PaymentModeID,PPN,PPH,Deskripsi,Total,Total_PPN,Total_PPH,CreatedDate,CreatedBy) OUTPUT INSERTED.PurchQuotID values ";
                    Query += "((Select 'PQ-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max(PurchQuotID) is null then '1' else substring(Max(PurchQuotID),11,4)+1 end), 5) ";
                    Query += "from [PurchQuotationH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                    Query += "'" + dtPqDate.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "'" + txtVendorPqNumber.Text + "',";
                    Query += "'" + dtVendorPqDate.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "'" + txtVendorId.Text + "',";
                    Query += "'" + txtVendorName.Text + "',";
                    Query += "'" + txtTermOfPayment.Text + "',";
                    Query += "'" + cmbPaymentMode.Text + "',";
                    Query += "'" + (txtPPN.Text.ToString() == "" ? "0.00" : txtPPN.Text) + "',";
                    Query += "'" + (txtPPH.Text.ToString() == "" ? "0.00" : txtPPH.Text) + "',";
                    Query += "'" + txtDeskripsi.Text + "',";
                    Query += "'" + (txtTotal.Text.ToString() == "" ? "0.00" : txtTotal.Text) + "',";
                    Query += "'" + (txtTotalPPN.Text.ToString() == "" ? "0.00" : txtTotalPPN.Text) + "',";
                    Query += "'" + (txtTotalPPH.Text.ToString() == "" ? "0.00" : txtTotalPPH.Text) + "',";
                    Query += "getdate(),'');";
                    Cmd = new SqlCommand(Query, Conn, Trans);

                    string PQNumber = Cmd.ExecuteScalar().ToString();

                    Query = "";
                    for (int i = 0; i <= dgvPqDetails.RowCount - 1; i++)
                    {
                        if (dgvPqDetails.Rows[i].Cells["AvailableDate"].Value == null || dgvPqDetails.Rows[i].Cells["AvailableDate"].Value == "")
                            dgvPqDetails.Rows[i].Cells["AvailableDate"].Value = "01-01-1900";

                        Query += "Insert PurchQuotation_Dtl (PurchQuotID,OrderDate,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,Qty,Unit,Price,Qty2,Unit2,AvailableDate,DeliveryMethod,ReffTransId,ReffSeqNo,ReffTransType,TransStatus,Deskripsi,DiscScheme,BonusScheme,CashBackScheme,SubTotal,SubTotal_PPN,SubTotal_PPH,CreatedDate,CreatedBy) Values ";
                        Query += "('" + PQNumber + "','";
                        Query += (dtPqDate.Value == null ? "" : dtPqDate.Value.ToString("yyyy-MM-dd")) + "','";

                        Query += (dgvPqDetails.Rows[i].Cells["No"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["No"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubGroup1Id"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubGroup2Id"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ItemId"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ItemId"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Qty"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["Qty"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvPqDetails.Rows[i].Cells["Unit"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Price"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["Price"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Qty2"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["Qty2"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Unit2"].Value == null ? "" : dgvPqDetails.Rows[i].Cells["Unit2"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["AvailableDate"].Value == null ? "" : FormateDateddmmyyyy(dgvPqDetails.Rows[i].Cells["AvailableDate"].Value.ToString())) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPqDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ReffTransId"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ReffTransId"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ReffSeqNo"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ReffSeqNo"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ReffTransType"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ReffTransType"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["TransStatus"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["TransStatus"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Deskripsi"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["Deskripsi"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["DiscScheme"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["DiscScheme"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["BonusScheme"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["BonusScheme"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["CashBackScheme"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["CashBackScheme"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubTotal"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["SubTotal"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'');";

                        if (i % 5 == 0 && i>0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            Query = "";
                        }
                    }
                    //Insert AppDocLogs
                    //Query += "Update PurchRequisitionH set TransStatus='13' where PurchReqID='" + PRNumber + "';";

                    if (Query != "")
                    {
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    
                    Trans.Commit();
                    MessageBox.Show("Data PQNumber : " + PQNumber + " berhasil ditambahkan.");
                    txtPqNumber.Text = PQNumber;
                    txtVendorPqNumber.Text = "";
                    ModeBeforeEdit();
                    GetDataHeader(PQNumber);
                }
                else
                {
                    Query = "Update PurchQuotationH set ";
                    Query += "OrderDate='" + dtPqDate.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "VendorQuotNo='" + txtVendorPqNumber.Text + "',";
                    Query += "VendorQuotDate='" + dtVendorPqDate.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "VendID='" + txtVendorId.Text + "',";
                    Query += "VendName='" + txtVendorName.Text + "',";
                    Query += "TermOfPayment='" + (txtTermOfPayment.Text == "" ? "0" : txtTermOfPayment.Text) + "',";
                    Query += "PaymentModeID='" + cmbPaymentMode.Text + "',";
                    Query += "PPN='" + (txtPPN.Text == "" ? "0.00" : txtPPN.Text) + "',";
                    Query += "PPH='" + (txtPPH.Text == "" ? "0.00" : txtPPH.Text) + "',";
                    Query += "Deskripsi='" + txtDeskripsi.Text + "',";
                    Query += "Total='" + (txtTotal.Text == "" ? "0.00" : txtTotal.Text) + "',";
                    Query += "Total_PPN='" + (txtTotalPPN.Text == "" ? "0.00" : txtTotalPPN.Text) + "',";
                    Query += "Total_PPH='" + (txtTotalPPH.Text == "" ? "0.00" : txtTotalPPH.Text) + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where PurchQuotID='" + txtPqNumber.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                        CreatedBy = Dr["CreatedBy"].ToString();
                    }
                    Dr.Close();

                    Query = "Delete from PurchQuotation_Dtl where PurchQuotID='" + txtPqNumber.Text.Trim() + "';";
                    for (int i = 0; i <= dgvPqDetails.RowCount - 1; i++)
                    {
                        if (dgvPqDetails.Rows[i].Cells["AvailableDate"].Value == null || dgvPqDetails.Rows[i].Cells["AvailableDate"].Value == "")
                            dgvPqDetails.Rows[i].Cells["AvailableDate"].Value = "01-01-1900";

                        Query += "Insert PurchQuotation_Dtl (PurchQuotID,OrderDate,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,Qty,Unit,Price,Qty2,Unit2,AvailableDate,DeliveryMethod,ReffTransId,ReffSeqNo,ReffTransType,TransStatus,Deskripsi,DiscScheme,BonusScheme,CashBackScheme,SubTotal,SubTotal_PPN,SubTotal_PPH,CreatedDate,CreatedBy) Values ";
                        Query += "('" + txtPqNumber.Text.Trim() + "','";
                        Query += (dtPqDate.Value == null ? "" : dtPqDate.Value.ToString("yyyy-MM-dd")) + "','";

                        Query += (dgvPqDetails.Rows[i].Cells["No"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["No"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubGroup1Id"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubGroup2Id"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ItemId"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ItemId"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Qty"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["Qty"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvPqDetails.Rows[i].Cells["Unit"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Price"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["Price"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Qty2"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["Qty2"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Unit2"].Value == null ? "" : dgvPqDetails.Rows[i].Cells["Unit2"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["AvailableDate"].Value == null ? "" : FormateDateddmmyyyy(dgvPqDetails.Rows[i].Cells["AvailableDate"].Value.ToString())) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPqDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ReffTransId"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ReffTransId"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ReffSeqNo"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ReffSeqNo"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["ReffTransType"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["ReffTransType"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["TransStatus"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["TransStatus"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["Deskripsi"].Value == "" ? "" : dgvPqDetails.Rows[i].Cells["Deskripsi"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["DiscScheme"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["DiscScheme"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["BonusScheme"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["BonusScheme"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["CashBackScheme"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["CashBackScheme"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubTotal"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["SubTotal"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value.ToString()) + "','";
                        Query += (dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value == "" ? "0.00" : dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'');";

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
                    MessageBox.Show("Data PurchReqID : " + txtPqNumber.Text + " berhasil diupdate.");
                    ModeBeforeEdit();
                    GetDataHeader(PQNumber);

                }
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

        private string FormateDateddmmyyyy(string tmpDate)
        { 
            //string reformat="";
            string [] data = tmpDate.Split('-');
            return data[2] + "-" + data[1] + "-" + data[0];
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            dtPqDate.Enabled = false;
            txtVendorPqNumber.Enabled = false;

            ModeBeforeEdit();
        }

        private void dgvPrDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (Mode != "BeforeEdit")
                {
                    if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "AvailableDate")
                    {
                        dtp.Location = dgvPqDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        dtp.Visible = true;

                        if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                        {
                            DateTime dDate;
                            if (!DateTime.TryParse(dgvPqDetails.CurrentCell.Value.ToString(), out dDate))
                            {
                                dtp.Value = Convert.ToDateTime(FormateDateddmmyyyy(dgvPqDetails.CurrentCell.Value.ToString()));
                            }
                            else
                            {
                                dtp.Value = Convert.ToDateTime(dgvPqDetails.CurrentCell.Value);
                            }
                        }
                        else
                        {
                            dtp.Value = DateTime.Now;
                        }
                    }
                    else
                    {
                        dtp.Visible = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

        }

        void cbo_Validating(object sender, CancelEventArgs e)
        {

            DataGridViewComboBoxEditingControl cbo = sender as DataGridViewComboBoxEditingControl;

            DataGridView grid = cbo.EditingControlDataGridView;

            object value = cbo.Text;

            // Add value to list if not there

            if (cbo.Items.IndexOf(value) == -1)
            {

                DataGridViewComboBoxCell cboCol = (DataGridViewComboBoxCell)grid.CurrentCell;

                // Must add to both the current combobox as well as the template, to avoid duplicate entries...

                cbo.Items.Add(value);

                cboCol.Items.Add(value);

                grid.CurrentCell.Value = value;

            }

        }

        private void dgvPrDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Index = dgvPqDetails.CurrentRow.Index;
            if (Mode != "BeforeEdit")
            {

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "AvailableDate")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        dgvPqDetails.CurrentCell.Value = dtp.Value.Date.ToString("dd-MM-yyyy");
                    }
                    else
                    {
                        dtp.Value = DateTime.Now;
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "Unit2")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        Conn = ConnectionString.GetConnection();

                        if (dgvPqDetails.Rows[Index].Cells["Unit"].Value.ToString() != dgvPqDetails.Rows[Index].Cells["Unit2"].Value.ToString())
                        {
                            Query = "Select Ratio From dbo.[InventConversion] where FullItemID='" + dgvPqDetails.Rows[Index].Cells["FullItemID"].Value.ToString() + "' and FromUnit='" + dgvPqDetails.Rows[Index].Cells["Unit"].Value.ToString() + "' and ToUnit='" + dgvPqDetails.Rows[Index].Cells["Unit2"].Value.ToString() + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Decimal Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
                            dgvPqDetails.Rows[Index].Cells["Qty2"].Value = decimal.Round(Ratio * Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["Qty"].Value), 2, MidpointRounding.AwayFromZero);
                            Conn.Close();
                        }
                        else
                        {
                            dgvPqDetails.Rows[Index].Cells["Qty2"].Value = dgvPqDetails.Rows[Index].Cells["Qty"].Value;

                        }
                    }
                    else
                    {
                        dtp.Value = DateTime.Now;
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "Price")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        dgvPqDetails.Rows[Index].Cells["SubTotal"].Value = Convert.ToDecimal(dgvPqDetails.CurrentCell.Value.ToString()) * Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["Qty2"].Value.ToString() == "" ? "0" : dgvPqDetails.Rows[Index].Cells["Qty2"].Value.ToString());
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "Qty2")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        dgvPqDetails.Rows[Index].Cells["SubTotal"].Value = Convert.ToDecimal(dgvPqDetails.CurrentCell.Value.ToString()) * Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["Price"].Value.ToString() == "" ? "0" : dgvPqDetails.Rows[Index].Cells["Price"].Value.ToString());
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "DiscScheme")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        SumFooter();
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "BonusScheme")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        SumFooter();
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "CashBackScheme")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        SumFooter();
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "SubTotal")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        SumFooter();
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "SubTotal_PPN")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        SumFooter();
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "SubTotal_PPH")
                {
                    if (dgvPqDetails.CurrentCell.Value != "" && dgvPqDetails.CurrentCell.Value != null)
                    {
                        SumFooter();
                    }
                }
                dtp.Visible = false;
                dgvPqDetails.AutoResizeColumns();
            }
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dgvPqDetails.CurrentCell.Value = dtp.Text;
        }

        private void dgvPrDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 )
            {
                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "ReffTransID" && Mode != "BeforeEdit")
                {
                    string SchemaName = "dbo";
                    string TableName = "SalesOrder";

                    Search tmpSearch = new Search();
                    tmpSearch.SetSchemaTable(SchemaName, TableName);
                    tmpSearch.ShowDialog();
                    dgvPqDetails.CurrentCell.Value = ConnectionString.Kode;
                    //dgvPrDetails.Columns[e.ColumnIndex].Name = ConnectionString.Kode2;
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "VendId" && Mode != "BeforeEdit")
                {
                    string SchemaName = "dbo";
                    string TableName = "VendTable";

                    Search tmpSearch = new Search();
                    tmpSearch.SetSchemaTable(SchemaName, TableName);
                    tmpSearch.ShowDialog();
                    dgvPqDetails.CurrentCell.Value = ConnectionString.Kode;
                    //dgvPrDetails.Columns[e.ColumnIndex].Name = ConnectionString.Kode2;
                }
                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "D1")
                {
                    Purchase.PurchaseRequisition.Info tmpInfo = new Purchase.PurchaseRequisition.Info();

                    ListInfo.Add(tmpInfo);
                    //tmpInfo.SetParent(this);
                    tmpInfo.Show();
                }
                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "D2")
                {
                    Purchase.PurchaseRequisition.Info tmpInfo = new Purchase.PurchaseRequisition.Info();

                    ListInfo.Add(tmpInfo);
                    //tmpInfo.SetParent(this);
                    tmpInfo.Show();
                }
                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "D3")
                {
                    Purchase.PurchaseRequisition.Info tmpInfo = new Purchase.PurchaseRequisition.Info();

                    ListInfo.Add(tmpInfo);
                    //tmpInfo.SetParent(this);
                    tmpInfo.Show();
                }
                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "D4")
                {
                    Purchase.PurchaseRequisition.Info tmpInfo = new Purchase.PurchaseRequisition.Info();

                    ListInfo.Add(tmpInfo);
                    //tmpInfo.SetParent(this);
                    tmpInfo.Show();
                }
                
            }
        }

        private void dgvPrDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);
            }

            //if (((DataGridView)sender).CurrentCell.ColumnIndex == 11) //Assuming 0 is the index of the ComboBox Column you want to show
            //{
            //    ComboBox cb = e.Control as ComboBox;
            //    if (cb != null)
            //    {
            //        cb.SelectionChangeCommitted -= new EventHandler(cmbCombo_SelectedIndexChanged);
            //        // now attach the event handler
            //        cb.SelectionChangeCommitted += new EventHandler(cmbCombo_SelectedIndexChanged);
            //    }
            //}

            //if (e.Control.AccessibilityObject.Role.ToString() == "ComboBox")
            //{
            //    DataGridViewComboBoxEditingControl tb = (DataGridViewComboBoxEditingControl)e.Control;
            //    tb.SelectedIndex += new Dara(cmbCombo_SelectedIndexChanged);

            //    e.Control.KeyPress += new DataGridViewDataErrorEventHandler(cmbCombo_SelectedIndexChanged);
            //}
        }

        private void cmbCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Index = dgvPqDetails.CurrentRow.Index;

            //Conn = ConnectionString.GetConnection();

            //Query = "Select Ratio From dbo.[InventConversion] where FullItemID='" + dgvPqDetails.Rows[Index].Cells["FullItemID"].Value.ToString() + "' and FromUnit='" + dgvPqDetails.Rows[Index].Cells["FromUnit"].Value.ToString() + "' and ToUnit='" + dgvPqDetails.Rows[Index].Cells["ToUnit"].Value.ToString() + "'";
            //Cmd = new SqlCommand(Query, Conn);
            //Decimal Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
            //dgvPqDetails.Rows[Index].Cells["FullItemID"].Value = Ratio * Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["Qty"].Value);

            //dtp.Visible = false;
            //dgvPqDetails.AutoResizeColumns();
        }

        public void SetParent(Purchase.PurchaseQuotation.InquiryPQ F)
        {
            Parent = F;
        }

        private void FormPQ_FormClosed(object sender, FormClosedEventArgs e)
        {
            //for (int i = 0; i < ListDetailPR.Count(); i++)
            //{
            //    ListDetailPR[i].Close();
            //}
            //for (int i = 0; i < ListGelombang.Count(); i++)
            //{
            //    ListGelombang[i].Close();
            //}
            for (int i = 0; i < ListInfo.Count(); i++)
            {
                ListInfo[i].Close();
            }
            Parent.RefreshGrid();
        }

       
        private void FormPQ_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void btnSearchPR_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "PurchRequisitionH";
            string Where = " AND TransStatus='13'";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, Where);
            tmpSearch.ShowDialog();
            txtPrNumber.Text = ConnectionString.Kode;

            dgvPqDetails.Rows.Clear();
            if (dgvPqDetails.RowCount - 1 <= 0)
            {
                dgvPqDetails.ColumnCount = 29;
                dgvPqDetails.Columns[0].Name = "No";
                dgvPqDetails.Columns[1].Name = "GroupId";
                dgvPqDetails.Columns[2].Name = "SubGroup1ID";
                dgvPqDetails.Columns[3].Name = "SubGroup2ID";
                dgvPqDetails.Columns[4].Name = "ItemID";
                dgvPqDetails.Columns[5].Name = "FullItemID";
                dgvPqDetails.Columns[6].Name = "ItemName";
                dgvPqDetails.Columns[7].Name = "Qty";
                dgvPqDetails.Columns[8].Name = "Unit";
                dgvPqDetails.Columns[9].Name = "Price";
                dgvPqDetails.Columns[10].Name = "Qty2";
                dgvPqDetails.Columns[11].Name = "Unit2";
                dgvPqDetails.Columns[12].Name = "AvailableDate";
                dgvPqDetails.Columns[13].Name = "DeliveryMethod";
                dgvPqDetails.Columns[14].Name = "ReffTransID";
                dgvPqDetails.Columns[15].Name = "ReffSeqNo";
                dgvPqDetails.Columns[16].Name = "ReffTransType";
                dgvPqDetails.Columns[17].Name = "TransStatus";
                dgvPqDetails.Columns[18].Name = "Deskripsi";
                dgvPqDetails.Columns[19].Name = "DiscScheme";
                dgvPqDetails.Columns[20].Name = "BonusScheme";
                dgvPqDetails.Columns[21].Name = "CashBackScheme";
                dgvPqDetails.Columns[22].Name = "SubTotal";
                dgvPqDetails.Columns[23].Name = "SubTotal_PPN";
                dgvPqDetails.Columns[24].Name = "SubTotal_PPH";
                dgvPqDetails.Columns[25].Name = "D1";
                dgvPqDetails.Columns[26].Name = "D2";
                dgvPqDetails.Columns[27].Name = "D3";
                dgvPqDetails.Columns[28].Name = "D4";
            }

            Conn = ConnectionString.GetConnection();

            Query = "Select GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, [Qty], [Unit], a.PurchReqId, SeqNo, TransType, b.TransStatus, Deskripsi From [PurchRequisition_Dtl] a inner join PurchRequisitionH b on a.PurchReqId=b.PurchReqId Where a.PurchReqID = '" + txtPrNumber.Text + "' order by SeqNo asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                this.dgvPqDetails.Rows.Add(dgvPqDetails.Rows.Count+1, Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], "", "", "", "", "", Dr[8], Dr[9], Dr[10], Dr[11], "", "", "", "", "", "", "");

                Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + Dr[4].ToString() + "' ";
                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader Dr1;
                Dr1 = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo1 = new DataGridViewComboBoxCell();
                while (Dr1.Read())
                {
                    combo1.Items.Add(Dr1[0].ToString());
                    combo1.Items.Add(Dr1[1].ToString());
                }
                dgvPqDetails.Rows[(dgvPqDetails.Rows.Count - 1)].Cells[11] = combo1;

                Query = "Select DeliveryMethod FROM [dbo].[DeliveryMethod] ";
                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader Dr2;
                Dr2 = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
                while (Dr2.Read())
                {
                    combo2.Items.Add(Dr2[0].ToString());
                }
                dgvPqDetails.Rows[(dgvPqDetails.Rows.Count - 1)].Cells[13] = combo2;

                dgvPqDetails.Rows[i].Cells[25].Value = "...";
                dgvPqDetails.Rows[i].Cells[26].Value = "...";
                dgvPqDetails.Rows[i].Cells[27].Value = "...";
                dgvPqDetails.Rows[i].Cells[28].Value = "...";

                i++;

                Dr1.Close();
                Dr2.Close();
            }
            Dr.Close();

            dgvPqDetails.ReadOnly = false;
            dgvPqDetails.Columns["No"].ReadOnly = true; 
            dgvPqDetails.Columns["FullItemID"].ReadOnly = true; 
            dgvPqDetails.Columns["ItemName"].ReadOnly = true; 
            dgvPqDetails.Columns["Qty"].ReadOnly = true; 
            dgvPqDetails.Columns["Unit"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal_PPN"].ReadOnly = true;
            dgvPqDetails.Columns["SubTotal_PPH"].ReadOnly = true;
            dgvPqDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubGroup1ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubGroup2ID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffTransID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffSeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ReffTransType"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["TransStatus"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["BonusScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["CashBackScheme"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal_PPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SubTotal_PPH"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPqDetails.Columns["GroupId"].Visible = false;
            dgvPqDetails.Columns["SubGroup1ID"].Visible = false;
            dgvPqDetails.Columns["SubGroup2ID"].Visible = false;
            dgvPqDetails.Columns["ItemID"].Visible = false;
            dgvPqDetails.Columns["ReffTransID"].Visible = false;
            dgvPqDetails.Columns["ReffSeqNo"].Visible = false;
            dgvPqDetails.Columns["ReffTransType"].Visible = false;
            dgvPqDetails.Columns["TransStatus"].Visible = false;

            dgvPqDetails.AutoResizeColumns();
        }

        private void SumFooter()
        {
            txtDiscScheme.Text = "";
            txtCashBackScheme.Text = "";
            txtTotal.Text = "";
            txtTotalPPN.Text = "";
            txtTotalPPH.Text = "";
            for (int i = 0; i < dgvPqDetails.RowCount; i++)
            {
                if (dgvPqDetails.Rows[i].Cells["DiscScheme"].Value == null || dgvPqDetails.Rows[i].Cells["DiscScheme"].Value == "")
                    dgvPqDetails.Rows[i].Cells["DiscScheme"].Value = "0";
                if (dgvPqDetails.Rows[i].Cells["BonusScheme"].Value == null || dgvPqDetails.Rows[i].Cells["BonusScheme"].Value == "")
                    dgvPqDetails.Rows[i].Cells["BonusScheme"].Value = "0";
                if (dgvPqDetails.Rows[i].Cells["CashBackScheme"].Value == null || dgvPqDetails.Rows[i].Cells["CashBackScheme"].Value == "")
                    dgvPqDetails.Rows[i].Cells["CashBackScheme"].Value = "0";
                if (dgvPqDetails.Rows[i].Cells["SubTotal"].Value == null || dgvPqDetails.Rows[i].Cells["SubTotal"].Value == "")
                    dgvPqDetails.Rows[i].Cells["SubTotal"].Value = "0";
                if (dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value == null || dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value == "")
                    dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value = "0";
                if (dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value == null || dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value == "")
                    dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value = "0";
                if (txtDiscScheme.Text == "")
                    txtDiscScheme.Text = "0";
                if (txtBonusScheme.Text == "")
                    txtBonusScheme.Text = "0";
                if (txtCashBackScheme.Text == "")
                    txtCashBackScheme.Text = "0";
                if (txtTotal.Text == "")
                    txtTotal.Text = "0";
                if (txtTotalPPN.Text == "")
                    txtTotalPPN.Text = "0";
                if (txtTotalPPH.Text == "")
                    txtTotalPPH.Text = "0";
                txtDiscScheme.Text = (Convert.ToDecimal(txtDiscScheme.Text) + Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["DiscScheme"].Value.ToString())).ToString();
                txtBonusScheme.Text = (Convert.ToDecimal(txtBonusScheme.Text) + Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["BonusScheme"].Value.ToString())).ToString();
                txtCashBackScheme.Text = (Convert.ToDecimal(txtCashBackScheme.Text) + Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["CashBackScheme"].Value.ToString())).ToString();
                txtTotal.Text = (Convert.ToDecimal(txtTotal.Text) + Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["SubTotal"].Value.ToString())).ToString();
                txtTotalPPN.Text = (Convert.ToDecimal(txtTotalPPN.Text) + Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value.ToString())).ToString();
                txtTotalPPH.Text = (Convert.ToDecimal(txtTotalPPH.Text) + Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value.ToString())).ToString();
            }
        }

        private void btnSearchVendor_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "VendTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtVendorId.Text = ConnectionString.Kode;
            txtVendorName.Text = ConnectionString.Kode2;
        }

        private void dgvPqDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "Price")
            {
                dgvPqDetails.Rows[Index].Cells["SubTotal"].Value = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["Price"].Value.ToString() == "" ? "0" : dgvPqDetails.Rows[Index].Cells["Price"].Value.ToString()) * Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["Qty2"].Value.ToString() == "" ? "0" : dgvPqDetails.Rows[Index].Cells["Qty2"].Value.ToString());
                SumFooter();
            }

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "Qty2")
            {
                dgvPqDetails.Rows[Index].Cells["SubTotal"].Value = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["Qty2"].Value.ToString() == "" ? "0" : dgvPqDetails.Rows[Index].Cells["Qty2"].Value.ToString()) * Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["Price"].Value.ToString() == "" ? "0" : dgvPqDetails.Rows[Index].Cells["Price"].Value.ToString());
                SumFooter();
            }

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "DiscScheme")
            {
                SumFooter();
            }

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "BonusScheme")
            {
                SumFooter();
            }

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "CashBackScheme")
            {
                SumFooter();
            }

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "SubTotal")
            {
                SumFooter();
            }

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "SubTotal_PPN")
            {
                SumFooter();
            }

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "SubTotal_PPH")
            {
                SumFooter();
            }
        }

        private void txtPPH_TextChanged(object sender, EventArgs e)
        {
            txtTotalPPH.Text = "0";
            if (txtPPH.Text != "")
            {
                for (int i = 0; i < dgvPqDetails.RowCount; i++)
                {
                    dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value = (Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["SubTotal"].Value.ToString()) * Convert.ToDecimal(txtPPH.Text.ToString()) / 100).ToString();
                    txtTotalPPH.Text = (Convert.ToDecimal(txtTotalPPH.Text.ToString()) + Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["SubTotal_PPH"].Value.ToString())).ToString();
                }
            }
        }

        private void txtPPN_TextChanged(object sender, EventArgs e)
        {
            txtTotalPPN.Text = "0";
            if (txtPPN.Text != "")
            {
                for (int i = 0; i < dgvPqDetails.RowCount; i++)
                {
                    dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value = (Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["SubTotal"].Value.ToString()) * Convert.ToDecimal(txtPPN.Text.ToString()) / 100).ToString();
                    txtTotalPPN.Text = (Convert.ToDecimal(txtTotalPPN.Text.ToString()) + Convert.ToDecimal(dgvPqDetails.Rows[i].Cells["SubTotal_PPN"].Value.ToString())).ToString();
                }
            }
        }

        private void txtPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtPPH_KeyPress(object sender, KeyPressEventArgs e)
        {
           if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
           {
               e.Handled = true;
           }
        }

        private void txtTermOfPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDiscScheme_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtTotalPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtTotalPPH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

    }
}
