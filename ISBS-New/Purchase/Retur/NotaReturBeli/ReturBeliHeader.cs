using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.NotaReturBeli
{
    public partial class ReturBeliHeader : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd, cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Status, Query, crit, NRBNumber, GRID = null;
        Purchase.Retur.NotaReturBeli.InqReturBeli Parent;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public ReturBeliHeader()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.NotaReturBeli.InqReturBeli f)
        {
            Parent = f;
        }

        private void ReturBeliHeader_Load(object sender, EventArgs e)
        {
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
            GetDataHeader();
        }

        public void SetMode(string tmpMode, string tmpNRBNumber)
        {
            Mode = tmpMode;
            NRBNumber = tmpNRBNumber;
        }

        public void CreateGrid()
        {
            if (dgvNRB.RowCount == 0)
            {
                dgvNRB.Rows.Clear();
                dgvNRB.ColumnCount = 20;
                dgvNRB.Columns[0].Name = "No";
                dgvNRB.Columns[1].Name = "ItemID";
                dgvNRB.Columns[2].Name = "FullItemID"; dgvNRB.Columns["FullItemID"].HeaderText = "Item ID";
                dgvNRB.Columns[3].Name = "ItemName"; dgvNRB.Columns["ItemName"].HeaderText = "Name";
                dgvNRB.Columns[4].Name = "GroupId";
                dgvNRB.Columns[5].Name = "SubGroup1ID";
                dgvNRB.Columns[6].Name = "SubGroup2ID";
                dgvNRB.Columns[7].Name = "RemainingQty";
                dgvNRB.Columns[8].Name = "UoM_Qty";
                dgvNRB.Columns[9].Name = "UoM_Unit";
                dgvNRB.Columns[10].Name = "Alt_Qty";
                dgvNRB.Columns[11].Name = "Alt_Unit";
                dgvNRB.Columns[12].Name = "Ratio";
                dgvNRB.Columns[13].Name = "Ratio_Actual";
                dgvNRB.Columns[14].Name = "GoodsReceivedSeqNo";
                dgvNRB.Columns[15].Name = "InventSiteId";
                dgvNRB.Columns[16].Name = "Quality";
                dgvNRB.Columns[17].Name = "Notes";
                dgvNRB.Columns[18].Name = "OldQty";
                //dgvNRB.Columns[19].Name = "JenisRetur";

                DataGridViewComboBoxColumn JenisRetur = new DataGridViewComboBoxColumn();
                JenisRetur.Name = "JenisRetur";
                JenisRetur.HeaderText = "      Jenis Retur       ";
                JenisRetur.Items.Add("Retur Tukar Barang");// code: 01
                JenisRetur.Items.Add("Retur Debet Note");// code: 02
                this.dgvNRB.Columns.Add(JenisRetur);
            }
        }

        public void ModeNew()
        {
            NRBNumber = "";
            dtNRB.Value = DateTime.Now;
            txtNotes.Enabled = true;
            dgvNRB.ReadOnly = false;

            btnSearchGR.Enabled = true;
            btnSearchWH.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtNotes.Enabled = true;
            dgvNRB.ReadOnly = false;

            btnSearchGR.Enabled = true;
            btnSearchWH.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            dgvNRB.ReadOnly = false;
            dgvNRB.Columns["No"].ReadOnly = true;
            dgvNRB.Columns["FullItemID"].ReadOnly = true;
            dgvNRB.Columns["ItemName"].ReadOnly = true;
            dgvNRB.Columns["RemainingQty"].ReadOnly = true;
            dgvNRB.Columns["UoM_Qty"].ReadOnly = false;
            dgvNRB.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRB.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRB.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRB.Columns["Ratio"].ReadOnly = true;
            dgvNRB.Columns["Ratio_Actual"].ReadOnly = true;
            dgvNRB.Columns["InventSiteID"].ReadOnly = true;
            dgvNRB.Columns["Quality"].ReadOnly = true;
            dgvNRB.Columns["Notes"].ReadOnly = true;
            dgvNRB.Columns["JenisRetur"].ReadOnly = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            GetDataHeader();

            txtNotes.Enabled = false;
            dgvNRB.ReadOnly = true;

            btnSearchGR.Enabled = false;
            btnSearchWH.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            dgvNRB.ReadOnly = true;
            dgvNRB.Columns["No"].ReadOnly = true;
            dgvNRB.Columns["FullItemID"].ReadOnly = true;
            dgvNRB.Columns["ItemName"].ReadOnly = true;
            dgvNRB.Columns["RemainingQty"].ReadOnly = true;
            dgvNRB.Columns["UoM_Qty"].ReadOnly = true;
            dgvNRB.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRB.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRB.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRB.Columns["Ratio"].ReadOnly = true;
            dgvNRB.Columns["Ratio_Actual"].ReadOnly = true;
            dgvNRB.Columns["InventSiteID"].ReadOnly = true;
            dgvNRB.Columns["Quality"].ReadOnly = true;
            dgvNRB.Columns["Notes"].ReadOnly = true;
            dgvNRB.Columns["JenisRetur"].ReadOnly = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(Login.Edit) > 0)
            {
                if (txtStatusName.Text.Contains("Approve") || txtStatusName.Text.Contains("Reject"))
                {
                    MessageBox.Show("Nota Retur Beli sudah diapprove/reject.");
                    return;
                }
                ModeEdit();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            GetDataHeader();
        }

        public void GetDataHeader()
        {
            if (NRBNumber != "")
            {
                dgvNRB.Rows.Clear();
                Conn = ConnectionString.GetConnection();
                //Query = "SELECT * FROM VW_NRBH WHERE NRBId = '" + NRBNumber + "'";
                Query = "SELECT TOP 1 NRBH.NRBId, NRBH.NRBDate, NRBH.GoodsReceivedID, NRBH.VendID, NRBH.VendName, NRBH.SiteId, INS.InventSiteName, INS.Lokasi, NRBH.Notes, TST.Deskripsi, NRBH.ApprovedBy ";
                Query += "FROM NotaReturBeliH NRBH LEFT JOIN InventSite INS ON INS.InventSiteID = NRBH.SiteId LEFT JOIN TransStatusTable TST ON NRBH.TransStatusId = TST.StatusCode And TST.TransCode = 'NotaReturBeli' ";
                Query += "WHERE NRBId = '" + NRBNumber + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    //txtGRNum.Text = Dr["NRBId"].ToString();
                    dtNRB.Text = Dr["NRBDate"].ToString();
                    txtNRBNum.Text = NRBNumber;
                    txtGRNum.Text = Dr["GoodsReceivedID"].ToString();
                    txtVendID.Text = Dr["VendID"].ToString();
                    txtVendName.Text = Dr["VendName"].ToString();
                    txtSiteID.Text = Dr["SiteId"].ToString();
                    txtSiteName.Text = Dr["InventSiteName"].ToString();
                    txtSiteLocation.Text = Dr["Lokasi"].ToString();
                    txtNotes.Text = Dr["Notes"].ToString();
                    txtStatusName.Text = Dr["Deskripsi"].ToString();
                    txtApproved.Text = Dr["ApprovedBy"].ToString();
                }
                Dr.Close();

                CreateGrid();

                //Query = "SELECT * FROM VW_NRBD WHERE NRBId = '" + NRBNumber + "' ORDER BY SeqNo ASC";
                Query = "SELECT NRBId, SeqNo, GroupId, SubGroupId, SubGroup2Id, ItemId, FullItemId, ItemName, RemainingQty, GoodsReceivedId, GoodsReceived_SeqNo, UoM_Qty, Alt_Qty, UoM_Unit, Alt_Unit, Ratio, NRBD.InventSiteId, ISB.InventSiteBlokID, ActionCode, Notes, Ratio_Actual, Quality ";
                Query += "FROM NotaReturBeli_Dtl NRBD LEFT JOIN InventSiteBlok ISB ON ISB.InventSiteID = NRBD.InventSiteId WHERE NRBId = '" + NRBNumber + "' ORDER BY SeqNo ASC";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    Query = "SELECT Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" + txtGRNum.Text + "' AND FullItemID = '" + Dr["FullItemID"] + "'";
                    cmd = new SqlCommand(Query, Conn);
                    this.dgvNRB.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroupId"], Dr["SubGroup2Id"], cmd.ExecuteScalar(), Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceived_SeqNo"], Dr["InventSiteId"], Dr["Quality"], Dr["Notes"], Dr["UoM_Qty"], Dr["ActionCode"]);//                    
                    if (Dr["ActionCode"].ToString() == "01")
                    {
                        dgvNRB.Rows[j].Cells["JenisRetur"].Value = "Retur Tukar Barang";
                    }
                    else if (Dr["ActionCode"].ToString() == "02")
                    {
                        dgvNRB.Rows[j].Cells["JenisRetur"].Value = "Retur Debet Note";
                    }
                    j++;
                }
                Dr.Close();

                dgvNRB.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Ratio_Actual"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["InventSiteID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Quality"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvNRB.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvNRB.Columns["ItemID"].Visible = false;
                dgvNRB.Columns["GroupId"].Visible = false;
                dgvNRB.Columns["SubGroup1ID"].Visible = false;
                dgvNRB.Columns["SubGroup2ID"].Visible = false;
                dgvNRB.Columns["Ratio"].Visible = false;
                dgvNRB.Columns["GoodsReceivedSeqNo"].Visible = false;
                dgvNRB.Columns["OldQty"].Visible = false;

                dgvNRB.AutoResizeColumns();

                Conn.Close();
            }
        }

        private void btnSearchGR_Click(object sender, EventArgs e)
        {
            AddGR GR = new AddGR();
            GR.setParent(this);
            GR.Show();
        }

        public void AddGR(string grnumber)
        {
            txtGRNum.Text = grnumber;
            Conn = ConnectionString.GetConnection();
            //Query = "SELECT PurchID, VendId, VendorName FROM VW_ADDGR_NRB WHERE GoodsReceivedId = '" + grnumber + "'";
            Query = "SELECT PurchID, VendId, VendorName FROM (SELECT DISTINCT GRH.GoodsReceivedId, GRD.Remaining_Qty, ROD.ReceiptOrderId, POH.PurchID, GRH.GoodsReceivedDate, GRH.VendId, GRH.VendorName, GRH.Notes, GRH.SiteName, GRH.SiteLocation ";
            Query += "FROM GoodsReceivedH GRH LEFT JOIN GoodsReceivedD GRD ON GRH.GoodsReceivedId = GRD.GoodsReceivedId LEFT JOIN ReceiptOrderD ROD ON GRH.RefTransID = ROD.ReceiptOrderId LEFT JOIN PurchH POH ON POH.PurchID = ROD.PurchaseOrderId ";
            Query += "WHERE GRH.GoodsReceivedStatus = '03' And GRD.ActionCodeStatus = '05') A WHERE GoodsReceivedId = '" + grnumber + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                txtPONum.Text = Dr["PurchID"].ToString();
                txtVendID.Text = Dr["VendId"].ToString();
                txtVendName.Text = Dr["VendorName"].ToString();
            }
            Dr.Close();
            dgvNRB.Rows.Clear();
        }

        private void btnSearchWH_Click(object sender, EventArgs e)
        {
            AddWH WH = new AddWH();
            WH.setParent(this);
            WH.Show();
        }

        public void AddWH(string WHCode)
        {
            txtSiteID.Text = WHCode;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT InventSiteName, Lokasi FROM InventSite WHERE InventSiteID = '" + WHCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                txtSiteName.Text = Dr["InventSiteName"].ToString();
                txtSiteLocation.Text = Dr["Lokasi"].ToString();
            }
        }

        public void AddDataGridDetail(List<string> SeqNo)
        {
            Conn = ConnectionString.GetConnection();
            CreateGrid();

            for (int i = 0; i < SeqNo.Count; i++)
            {
                Query = "SELECT * FROM GoodsReceivedD a INNER JOIN InventTable b ON a.FullItemID = b.FullItemID WHERE a.GoodsReceivedId = '" + txtGRNum.Text + "' AND a.GoodsReceivedSeqNo = '" + SeqNo[i] + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    Query = "SELECT ISNULL(Remaining_Qty,0) AS Remaining_Qty FROM [GoodsReceivedD] WHERE GoodsReceivedId = '" + txtGRNum.Text + "' AND GoodsReceivedSeqNo = '" + Dr["GoodsReceivedSeqNo"] + "'";
                    cmd = new SqlCommand(Query, Conn);

                    this.dgvNRB.Rows.Add(dgvNRB.RowCount + 1, Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], cmd.ExecuteScalar(), Dr["Qty_Actual"], Dr["UoM"], Dr["TotalBerat_Actual"], Dr["UoMAlt"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceivedSeqNo"], Dr["InventSiteID"], Dr["Quality"], Dr["Notes"], Dr["Qty_Actual"]);
                }
                Dr.Close();
            }

            dgvNRB.ReadOnly = false;
            //dgvNRB.DefaultCellStyle.BackColor = Color.White;
            dgvNRB.Columns["No"].ReadOnly = true;
            dgvNRB.Columns["FullItemID"].ReadOnly = true;
            dgvNRB.Columns["ItemName"].ReadOnly = true;
            dgvNRB.Columns["RemainingQty"].ReadOnly = true;
            dgvNRB.Columns["UoM_Qty"].ReadOnly = false;
            dgvNRB.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRB.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRB.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRB.Columns["Ratio"].ReadOnly = true;
            dgvNRB.Columns["Ratio_Actual"].ReadOnly = true;
            dgvNRB.Columns["InventSiteID"].ReadOnly = true;
            dgvNRB.Columns["Quality"].ReadOnly = true;
            dgvNRB.Columns["Notes"].ReadOnly = true;

            dgvNRB.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["Ratio_Actual"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["InventSiteID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["Quality"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRB.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvNRB.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRB.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRB.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRB.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRB.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvNRB.Columns["ItemID"].Visible = false;
            dgvNRB.Columns["GroupId"].Visible = false;
            dgvNRB.Columns["SubGroup1ID"].Visible = false;
            dgvNRB.Columns["SubGroup2ID"].Visible = false;
            dgvNRB.Columns["Ratio"].Visible = false;
            dgvNRB.Columns["GoodsReceivedSeqNo"].Visible = false;
            dgvNRB.Columns["OldQty"].Visible = false;

            dgvNRB.AutoResizeColumns();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (txtGRNum.Text == "" || txtSiteID.Text == "")
            {
                MessageBox.Show("Pilih Good Received dan Warehouse terlebih dahulu.");
            }
            else
            {
                List<string> seqno = new List<string>();
                if (dgvNRB.RowCount > 0)
                {
                    for (int i = 0; i < dgvNRB.RowCount; i++)
                    {
                        seqno.Add(dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString());
                    }
                }

                AddItem add = new AddItem();
                add.setParent(this);
                add.setMode(txtGRNum.Text, seqno);
                add.Show();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvNRB.RowCount > 0)
            {
                dgvNRB.Rows.RemoveAt(dgvNRB.CurrentRow.Index);
                for (int i = 0; i < dgvNRB.RowCount; i++)
                {
                    dgvNRB.Rows[i].Cells["No"].Value = i + 1;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            decimal QtyNew = 0;

            if (dgvNRB.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }
            try
            {
                if (Mode == "New" && txtNRBNum.Text.Trim() == "")
                {
                    string Jenis = "NRB", Kode = "NRB";
                    NRBNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "INSERT INTO NotaReturBeliH (NRBId, NRBDate, NRBMode, VendId, VendName, GoodsReceivedId, PurchId, SiteId, SiteName, TransStatusId, Notes, CreatedDate, CreatedBy) VALUES ";
                    Query += "('" + NRBNumber + "', ";
                    Query += "'" + dtNRB.Value.ToString("yyyy-MM-dd") + "', ";
                    Query += "'AUTO', ";
                    Query += "'" + txtVendID.Text + "', ";
                    Query += "'" + txtVendName.Text + "', ";
                    Query += "'" + txtGRNum.Text + "', ";
                    Query += "'" + txtPONum.Text + "', ";
                    Query += "'" + txtSiteID.Text + "', ";
                    Query += "'" + txtSiteName.Text + "', ";
                    Query += "'01', ";
                    Query += "'" + txtNotes.Text + "', ";
                    Query += "GETDATE(), '" + Login.Username + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "";
                    for (int i = 0; i < dgvNRB.RowCount; i++)
                    {
                        if (dgvNRB.Rows[i].Cells["UoM_Qty"].Value == null || decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()) == 0)
                        {
                            MessageBox.Show("Terdapat quantity yang belum diisi.");
                            return;
                        }
                        if (decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) < decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Terdapat quantity yang melebihi batas.");
                            return;
                        }
                        
                        Query = "INSERT INTO NotaReturBeli_Dtl( ";
                        Query += "NRBId,SeqNo,GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemId,ItemName,GoodsReceivedId,GoodsReceived_SeqNo,";
                        Query += "UoM_Qty,UoM_Unit,Alt_Qty,Alt_Unit,Ratio,Ratio_Actual,RemainingQty,Quality,InventSiteId,ActionCode,Notes,CreatedBy) ";
                        Query += "VALUES('";
                        Query += NRBNumber + "', '";
                        Query += (dgvNRB.Rows[i].Cells["No"].Value == "" ? "" : dgvNRB.Rows[i].Cells["No"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvNRB.Rows[i].Cells["GroupId"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["SubGroup1ID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["SubGroup1ID"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["SubGroup2ID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["SubGroup2ID"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["ItemID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["ItemID"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvNRB.Rows[i].Cells["ItemName"].Value.ToString()) + "', '";
                        Query += txtGRNum.Text + "', '";
                        Query += (dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value == "" ? "" : dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["UoM_Unit"].Value == "" ? "" : dgvNRB.Rows[i].Cells["UoM_Unit"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["Alt_Qty"].Value == null ? "0" : dgvNRB.Rows[i].Cells["Alt_Qty"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["Alt_Unit"].Value == "" ? "" : dgvNRB.Rows[i].Cells["Alt_Unit"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["Ratio"].Value == "" ? "0" : dgvNRB.Rows[i].Cells["Ratio"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["Ratio_Actual"].Value == "" ? "0" : dgvNRB.Rows[i].Cells["Ratio_Actual"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["RemainingQty"].Value == "" ? "0" : dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["Quality"].Value == "" ? "" : dgvNRB.Rows[i].Cells["Quality"].Value.ToString()) + "', '";
                        //Query += (dgvNRB.Rows[i].Cells["InventSiteID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["InventSiteID"].Value.ToString()) + "', '";
                        Query += txtSiteID.Text + "', '";

                        string Action = dgvNRB.Rows[i].Cells["JenisRetur"].Value == null ? "" : dgvNRB.Rows[i].Cells["JenisRetur"].Value.ToString();
                        if (Action == "")
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jenis Retur belum Dipilih.");
                            return;
                        }
                        else if (Action == "Retur Tukar Barang")
                        {
                            Query += "01', '";
                        }
                        else if (Action == "Retur Debet Note")
                        {
                            Query += "02', '";
                        }
                        
                        Query += (dgvNRB.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNRB.Rows[i].Cells["Notes"].Value.ToString()) + "', '";
                        Query += Login.Username + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) - decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString());
                        Query = "UPDATE GoodsReceivedD SET Remaining_Qty = '" + QtyNew + "'";
                        Query += "WHERE GoodsReceivedId = '" + txtGRNum.Text.Trim() + "' AND FullItemId = '" + dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
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

                    Trans.Commit();
                    MessageBox.Show("Data NRBNumber : " + NRBNumber + " berhasil ditambahkan.");
                    txtNRBNum.Text = NRBNumber;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
                else
                {
                    DateTime CreatedDate = DateTime.Now;
                    String CreatedBy = "";

                    Query = "UPDATE NotaReturBeliH SET ";
                    Query += "VendId = '" + txtVendID.Text + "',";
                    Query += "VendName = '" + txtVendName.Text + "',";
                    Query += "GoodsReceivedId = '" + txtGRNum.Text + "',";
                    Query += "SiteId = '" + txtSiteID.Text + "',";
                    Query += "SiteName = '" + txtSiteName.Text + "',";
                    Query += "TransStatusId = '01',";
                    Query += "Notes = '" + txtNotes.Text + "',";
                    Query += "UpdatedDate = GETDATE(),";
                    Query += "UpdatedBy = '" + Login.Username + "',";
                    Query += "ApprovedBy = '' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where NRBId='" + txtNRBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "";

                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                        CreatedBy = Dr["CreatedBy"].ToString();
                    }
                    Dr.Close();

                    //Update Qty
                    List<string> GoodsReceivedSeqNo = new List<string>();
                    List<string> GoodsReceivedID = new List<string>();
                    List<decimal> Qty = new List<decimal>();
                    decimal RemainingQty, QtyNew2 = 0;
                    Query = "SELECT GoodsReceivedSeqNo,UoM_Qty,GoodsReceivedID FROM NotaReturBeli_Dtl WHERE NRBId='" + txtNRBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        GoodsReceivedSeqNo.Add(Dr["GoodsReceivedSeqNo"].ToString());
                        GoodsReceivedID.Add(Dr["GoodsReceivedID"].ToString());
                        Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                    }
                    Dr.Close();

                    for (int i = 0; i < GoodsReceivedSeqNo.Count; i++)
                    {
                        Query = "SELECT Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                        QtyNew2 = RemainingQty + Qty[i];

                        Query = "UPDATE GoodsReceivedD SET ";
                        Query += "Remaining_Qty = '" + QtyNew2 + "' ";
                        Query += "WHERE GoodsReceivedId = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo='" + GoodsReceivedSeqNo[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }

                    Query = "DELETE FROM NotaReturBeli_Dtl WHERE NRBId='" + txtNRBNum.Text.Trim() + "'";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    Query = "";

                    for (int i = 0; i < dgvNRB.RowCount; i++)
                    {
                        if (dgvNRB.Rows[i].Cells["UoM_Qty"].Value == null || decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()) == 0)
                        {
                            MessageBox.Show("Terdapat quantity yang belum diisi.");
                            return;
                        }
                        if ((decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) + decimal.Parse(dgvNRB.Rows[i].Cells["OldQty"].Value.ToString())) < decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Terdapat quantity yang melebihi batas.");
                            return;
                        }

                        Query = "INSERT INTO NotaReturBeli_Dtl( ";
                        Query += "NRBId,SeqNo,GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemId,ItemName,GoodsReceivedId,GoodsReceived_SeqNo,";
                        Query += "UoM_Qty,UoM_Unit,Alt_Qty,Alt_Unit,Ratio,RemainingQty,InventSiteId,ActionCode,Notes,CreatedBy) ";
                        Query += "VALUES('";
                        Query += NRBNumber + "', '";
                        Query += (dgvNRB.Rows[i].Cells["No"].Value == "" ? "" : dgvNRB.Rows[i].Cells["No"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvNRB.Rows[i].Cells["GroupId"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["SubGroup1ID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["SubGroup1ID"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["SubGroup2ID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["SubGroup2ID"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["ItemID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["ItemID"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvNRB.Rows[i].Cells["ItemName"].Value.ToString()) + "', '";
                        Query += txtGRNum.Text + "', '";
                        Query += (dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value == "" ? "" : dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["UoM_Unit"].Value == "" ? "" : dgvNRB.Rows[i].Cells["UoM_Unit"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["Alt_Qty"].Value == null ? "0" : dgvNRB.Rows[i].Cells["Alt_Qty"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["Alt_Unit"].Value == "" ? "" : dgvNRB.Rows[i].Cells["Alt_Unit"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["Ratio"].Value == "" ? "0" : dgvNRB.Rows[i].Cells["Ratio"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["RemainingQty"].Value == "" ? "0" : dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) + "', '";
                        Query += (dgvNRB.Rows[i].Cells["InventSiteID"].Value == "" ? "" : dgvNRB.Rows[i].Cells["InventSiteID"].Value.ToString()) + "', '";

                        string Action = dgvNRB.Rows[i].Cells["JenisRetur"].Value == null ? "" : dgvNRB.Rows[i].Cells["JenisRetur"].Value.ToString();
                        if (Action == "")
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jenis Retur belum Dipilih.");
                            return;
                        }
                        else if (Action == "Retur Tukar Barang")
                        {
                            Query += "01', '";
                        }
                        else if (Action == "Retur Debet Note")
                        {
                            Query += "02', '";
                        }

                        Query += (dgvNRB.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNRB.Rows[i].Cells["Notes"].Value.ToString()) + "', '";
                        Query += Login.Username + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) + decimal.Parse(dgvNRB.Rows[i].Cells["OldQty"].Value.ToString()) - decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString());
                        Query = "UPDATE GoodsReceivedD SET Remaining_Qty = '" + QtyNew + "'";
                        Query += "WHERE GoodsReceivedId = '" + txtGRNum.Text.Trim() + "' AND GoodsReceivedSeqNo='" + dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
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

                    Trans.Commit();
                    MessageBox.Show("Data NRBNumber : " + txtNRBNum.Text + " berhasil diupdate.");
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(Ex.Message);
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void dgvNRB_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvNRB.RowCount > 0)
            {
                if (dgvNRB.Columns[e.ColumnIndex].Name == "UoM_Qty")
                {
                    if (dgvNRB.CurrentRow.Cells["UoM_Qty"].Value == null)
                    {
                        dgvNRB.CurrentRow.Cells["Alt_Qty"].Value = "";
                    }
                    else
                    {
                        dgvNRB.CurrentRow.Cells["Alt_Qty"].Value = decimal.Parse(dgvNRB.CurrentRow.Cells["UoM_Qty"].Value.ToString()) * decimal.Parse(dgvNRB.CurrentRow.Cells["Ratio_Actual"].Value.ToString());
                    }
                }
            }
        }

        private void dgvNRB_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dgvNRB.CurrentCell.ColumnIndex == dgvNRB.Columns["UoM_Qty"].Index)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }
    }
}
