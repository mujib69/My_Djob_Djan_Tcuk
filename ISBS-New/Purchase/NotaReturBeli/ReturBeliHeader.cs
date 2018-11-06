using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.NotaReturBeli
{
    public partial class ReturBeliHeader : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd, cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr, helpdr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        public string Mode;
        string Status, Query, crit, NRBNumber, GRID = null;
        Purchase.NotaReturBeli.InqReturBeli Parent;
        Purchase.NotaReturBeli.AddItem ParentAI;
        PopUp.FullItemId.FullItemId FullItemId = new PopUp.FullItemId.FullItemId();
        ContextMenu vendid = new ContextMenu();

        int Index;
        bool Journal = false;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public ReturBeliHeader()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.NotaReturBeli.InqReturBeli f)
        {
            Parent = f;
        }

        private void ReturBeliHeader_Load(object sender, EventArgs e)
        {
            AddJenisRetur();
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
            else if (Mode=="PopUp")//tia edit
            {
                ModePopUp();
            }//tia edit end
            GetDataHeader();
        }

        private void AddJenisRetur()
        {
            cmbJenisRetur.Items.Add("---- Jenis Retur ----");
            cmbJenisRetur.Items.Add("Retur Tukar Barang");
            cmbJenisRetur.Items.Add("Retur Debet Note");
            cmbJenisRetur.Items.Add("Retur Kembalikan Barang");
            cmbJenisRetur.SelectedIndex = 0;
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
                dgvNRB.ColumnCount = 26;
                dgvNRB.Columns[0].Name = "No";
                dgvNRB.Columns[1].Name = "ItemID";
                dgvNRB.Columns[2].Name = "FullItemID"; dgvNRB.Columns["FullItemID"].HeaderText = "Item ID";
                dgvNRB.Columns[3].Name = "ItemName"; dgvNRB.Columns["ItemName"].HeaderText = "Name";
                dgvNRB.Columns[4].Name = "GroupId";
                dgvNRB.Columns[5].Name = "SubGroup1ID";
                dgvNRB.Columns[6].Name = "SubGroup2ID";
                dgvNRB.Columns[7].Name = "RemainingQty";
                dgvNRB.Columns[8].Name = "UoM_Qty"; dgvNRB.Columns["UoM_Qty"].HeaderText = "Retur Qty";
                dgvNRB.Columns[9].Name = "UoM_Unit"; dgvNRB.Columns["UoM_Qty"].HeaderText = "Unit";
                dgvNRB.Columns[10].Name = "Alt_Qty";
                dgvNRB.Columns[11].Name = "Alt_Unit";
                dgvNRB.Columns[12].Name = "Ratio";
                dgvNRB.Columns[13].Name = "Ratio_Actual";
                dgvNRB.Columns[14].Name = "GoodsReceivedSeqNo";
                dgvNRB.Columns[15].Name = "InventSiteId";
                dgvNRB.Columns[16].Name = "Quality";
                dgvNRB.Columns[17].Name = "Notes";
                dgvNRB.Columns[18].Name = "OldQty";
                dgvNRB.Columns[19].Name ="Price";
                dgvNRB.Columns[20].Name ="Total";
                dgvNRB.Columns[21].Name ="Total_Discount";
                dgvNRB.Columns[22].Name ="PPN";
                dgvNRB.Columns[23].Name ="Total_PPN";
                dgvNRB.Columns[24].Name ="PPH";
                dgvNRB.Columns[25].Name ="Total_PPH";

                DataGridViewComboBoxColumn JenisRetur = new DataGridViewComboBoxColumn();
                JenisRetur.Name = "JenisRetur";
                JenisRetur.HeaderText = "      Jenis Retur       ";
                JenisRetur.Items.Add("Retur Tukar Barang");// code: 01
                JenisRetur.Items.Add("Retur Debet Note");// code: 02
                JenisRetur.Items.Add("Retur Kembalikan Barang");// code: 03
                this.dgvNRB.Columns.Add(JenisRetur);
            }
        }

        public void ModeNew()
        {
            NRBNumber = "";
            dtNRB.Value = DateTime.Now;
            txtNotes.Enabled = true;
            cmbJenisRetur.Enabled = true;
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
            cmbJenisRetur.Enabled = true;
            dgvNRB.ReadOnly = false;

            btnSearchGR.Enabled = false;
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

            dgvNRB.DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRB.Columns["UoM_Qty"].DefaultCellStyle.BackColor = Color.White;
            dgvNRB.Columns["JenisRetur"].DefaultCellStyle.BackColor = Color.White;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            GetDataHeader();

            txtNotes.Enabled = false;
            cmbJenisRetur.Enabled = false;
            dgvNRB.ReadOnly = true;

            //tia edit
            txtVendName.Enabled = true;
            txtVendID.Enabled = true;
            txtPONum.Enabled = true;
            txtGRNum.Enabled = true;

            txtVendID.ReadOnly = true;
            txtVendName.ReadOnly = true;
            txtPONum.ReadOnly = true;
            txtGRNum.ReadOnly = true;

            txtVendID.ContextMenu = vendid;
            txtVendName.ContextMenu = vendid;
            txtPONum.ContextMenu = vendid;
            txtGRNum.ContextMenu = vendid;

            //tia end

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

            dgvNRB.DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRB.Columns["UoM_Qty"].DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRB.Columns["JenisRetur"].DefaultCellStyle.BackColor = Color.LightGray;
        }
        //tia edit
        public void ModePopUp()
        {
            Mode = "PopUp";

            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }

            GetDataHeader();

            txtNotes.Enabled = false;
            cmbJenisRetur.Enabled = false;
            dgvNRB.ReadOnly = true;

            txtVendName.Enabled = true;
            txtVendID.Enabled = true;
            txtPONum.Enabled = true;
            txtGRNum.Enabled = true;

            txtVendID.ReadOnly = true;
            txtVendName.ReadOnly = true;
            txtPONum.ReadOnly = true;
            txtGRNum.ReadOnly = true;

            txtVendID.ContextMenu = vendid;
            txtVendName.ContextMenu = vendid;
            txtPONum.ContextMenu = vendid;
            txtGRNum.ContextMenu = vendid;

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

            dgvNRB.DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRB.Columns["UoM_Qty"].DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRB.Columns["JenisRetur"].DefaultCellStyle.BackColor = Color.LightGray;
        }
        //tia edit end

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
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
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (Mode == "Edit")
            {
                DialogResult dr = MessageBox.Show("Apakah anda ingin membatalkan perubahan ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
                    {
                        if (Application.OpenForms[i].Name == "AddGR")
                            Application.OpenForms[i].Close();
                        else if (Application.OpenForms[i].Name == "AddItem")
                            Application.OpenForms[i].Close();
                        else if (Application.OpenForms[i].Name == "AddWH")
                            Application.OpenForms[i].Close();
                    }
                    ModeBeforeEdit();
                    GetDataHeader();

                    DelStatusItem = "";
                    DelSeq_No.Clear();
                    DelFullItemID.Clear();
                    DelUoM_Qty.Clear();
                }
            }
        }

        public void GetDataHeader()
        {
            if (NRBNumber != "")
            {
                dgvNRB.Rows.Clear();
                Conn = ConnectionString.GetConnection();
                Query = "SELECT TOP 1 NRBH.NRBId, NRBH.NRBDate, NRBH.GoodsReceivedID, NRBH.GoodsReceivedDate, NRBH.PurchId, NRBH.VendID, NRBH.VendName, NRBH.SiteId, INS.InventSiteName, INS.Lokasi, NRBH.Notes, TST.Deskripsi, NRBH.ApprovedBy, NRBH.ActionCode ";
                Query += "FROM NotaReturBeliH NRBH LEFT JOIN InventSite INS ON INS.InventSiteID = NRBH.SiteId LEFT JOIN TransStatusTable TST ON NRBH.TransStatusId = TST.StatusCode And TST.TransCode = 'NotaReturBeli' ";
                Query += "WHERE NRBId = '" + NRBNumber + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtNRB.Text = Dr["NRBDate"].ToString();
                    txtNRBNum.Text = NRBNumber;
                    txtGRNum.Text = Dr["GoodsReceivedID"].ToString();
                    dtGR.Text = Dr["GoodsReceivedDate"].ToString();
                    txtVendID.Text = Dr["VendID"].ToString();
                    txtVendName.Text = Dr["VendName"].ToString();
                    txtSiteID.Text = Dr["SiteId"].ToString();
                    txtSiteName.Text = Dr["InventSiteName"].ToString();
                    txtSiteLocation.Text = Dr["Lokasi"].ToString();
                    txtNotes.Text = Dr["Notes"].ToString();
                    txtStatusName.Text = Dr["Deskripsi"].ToString();
                    txtApproved.Text = Dr["ApprovedBy"].ToString();
                    txtPONum.Text = Dr["PurchId"].ToString();
                    if (Dr["ActionCode"].ToString() == "01")
                    {
                        cmbJenisRetur.SelectedIndex = 1;
                    }
                    else if (Dr["ActionCode"].ToString() == "02")
                    {
                        cmbJenisRetur.SelectedIndex = 2;
                    }
                }
                Dr.Close();

                CreateGrid();

                Query = "SELECT NRBId, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, RemainingQty, GoodsReceivedId, GoodsReceived_SeqNo, UoM_Qty, Alt_Qty, UoM_Unit, Alt_Unit, Ratio, NRBD.InventSiteId, ISB.InventSiteBlokID, ActionCode, Notes, Ratio_Actual, Quality,[Price],[Total],[Total_Discount],[PPN],[Total_PPN],[PPH],[Total_PPH] ";
                Query += "FROM NotaReturBeli_Dtl NRBD LEFT JOIN InventSiteBlok ISB ON ISB.InventSiteID = NRBD.InventSiteId WHERE NRBId = '" + NRBNumber + "' ORDER BY SeqNo ASC";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    Query = "SELECT Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" + txtGRNum.Text + "' AND FullItemID = '" + Dr["FullItemID"] + "'";
                    cmd = new SqlCommand(Query, Conn);
                    this.dgvNRB.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], decimal.Parse(cmd.ExecuteScalar().ToString()) + decimal.Parse(Dr["UoM_Qty"].ToString()), Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], Dr["Ratio_Actual"] == System.DBNull.Value ? "":Dr["Ratio_Actual"], Dr["GoodsReceived_SeqNo"], Dr["InventSiteId"], Dr["Quality"] == System.DBNull.Value ? "":Dr["Quality"], Dr["Notes"], Dr["UoM_Qty"],Dr["Price"],Dr["Total"],Dr["Total_Discount"],Dr["PPN"],Dr["Total_PPN"],Dr["PPH"],Dr["Total_PPH"]);
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
                dgvNRB.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Total"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Total_Discount"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["PPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Total_PPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["PPH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Total_PPH"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvNRB.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvNRB.Columns["ItemID"].Visible = false;
                dgvNRB.Columns["GroupId"].Visible = false;
                dgvNRB.Columns["SubGroup1ID"].Visible = false;
                dgvNRB.Columns["SubGroup2ID"].Visible = false;
                dgvNRB.Columns["Alt_Unit"].Visible = false;
                dgvNRB.Columns["Ratio_Actual"].Visible = false;
                dgvNRB.Columns["GoodsReceivedSeqNo"].Visible = false;
                dgvNRB.Columns["OldQty"].Visible = false;
                dgvNRB.Columns["JenisRetur"].Visible = false;
                dgvNRB.Columns["Alt_Qty"].Visible = false;
                dgvNRB.Columns["Ratio"].Visible = false;

                dgvNRB.AutoResizeColumns();

                Conn.Close();
            }
        }

        AddGR GR = null;
        private void btnSearchGR_Click(object sender, EventArgs e)
        {
            if (GR == null || GR.Text == "")
            {
                GR = new AddGR();
                GR.setParent(this);
                GR.Show();
            }
            else if (CheckOpened(GR.Name))
            {
                GR.WindowState = FormWindowState.Normal;
                GR.Show();
                GR.Focus();
            }
        }

        public void AddGR(string grnumber)
        {
            txtGRNum.Text = grnumber;
            Conn = ConnectionString.GetConnection();
            //Query = "SELECT PurchID, VendId, VendorName FROM VW_ADDGR_NRB WHERE GoodsReceivedId = '" + grnumber + "'";
            Query = "SELECT PurchID, VendId, VendorName, GoodsReceivedDate, SiteID, SiteName, SiteLocation FROM (SELECT DISTINCT GRH.GoodsReceivedId, GRD.Remaining_Qty, ROD.ReceiptOrderId, POH.PurchID, GRH.GoodsReceivedDate, GRH.VendId, GRH.VendorName, GRH.Notes, GRH.SiteID, GRH.SiteName, GRH.SiteLocation ";
            Query += "FROM GoodsReceivedH GRH LEFT JOIN GoodsReceivedD GRD ON GRH.GoodsReceivedId = GRD.GoodsReceivedId LEFT JOIN ReceiptOrderD ROD ON GRH.RefTransID = ROD.ReceiptOrderId LEFT JOIN PurchH POH ON POH.PurchID = ROD.PurchaseOrderId ";
            Query += "WHERE GRH.GoodsReceivedStatus in ('03','06') And GRD.ActionCodeStatus = '05') A WHERE GoodsReceivedId = '" + grnumber + "'";
            Cmd = new SqlCommand(Query, Conn);
            cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            helpdr = cmd.ExecuteReader();
            if (helpdr.Read())
            {
                while (Dr.Read())
                {
                    txtPONum.Text = Dr["PurchID"].ToString();
                    txtVendID.Text = Dr["VendId"].ToString();
                    txtVendName.Text = Dr["VendorName"].ToString();
                    dtGR.Text = Dr["GoodsReceivedDate"].ToString();
                    txtSiteID.Text = Dr["SiteID"].ToString();
                    txtSiteName.Text = Dr["SiteName"].ToString();
                    txtSiteLocation.Text = Dr["SiteLocation"].ToString();
                }
            }
            else
            {
                txtPONum.Text = "";
                txtVendID.Text = "";
                txtVendName.Text = "";
                dtGR.Text = "";
                txtSiteID.Text = "";
                txtSiteName.Text = "";
                txtSiteLocation.Text = "";
            }
            Dr.Close();
            helpdr.Close();
            dgvNRB.Rows.Clear();
        }

        AddWH WH = null;
        private void btnSearchWH_Click(object sender, EventArgs e)
        {
            if (Mode == "Edit")
            {
                if (dgvNRB.RowCount > 0)
                {
                    MessageBox.Show("Jika Anda mengganti Warehouse, hapus item yang sudah ada terlebih dahulu", "Warning!!!", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    if (WH == null || WH.Text == "")
                    {
                        WH = new AddWH();
                        WH.setParent(this);
                        WH.Show();
                    }
                    else if (CheckOpened(WH.Name))
                    {
                        WH.WindowState = FormWindowState.Normal;
                        WH.Show();
                        WH.Focus();
                    }
                }
            }
            else if (Mode == "New")
            {
                DialogResult dr = MessageBox.Show("Apakah anda ingin mengubah Warehouse ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    if (WH == null || WH.Text == "")
                    {
                        WH = new AddWH();
                        WH.setParent(this);
                        WH.Show();
                    }
                    else if (CheckOpened(WH.Name))
                    {
                        WH.WindowState = FormWindowState.Normal;
                        WH.Show();
                        WH.Focus();
                    }
                }
            }
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

                    if (Mode == "New")
                    {
                        this.dgvNRB.Rows.Add(dgvNRB.RowCount + 1, Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], cmd.ExecuteScalar(), "", Dr["UoM"], "", Dr["UoMAlt"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceivedSeqNo"], Dr["InventSiteID"], Dr["Quality"], Dr["Notes"], Dr["Qty_Actual"]);//Dr["Qty_Actual"]--UoM_Qty, Dr["TotalBerat_Actual"]
                    }
                    else if (Mode == "Edit")
                    {
                        if (DelStatusItem == "ItemDeleted")
                        {
                            for (int j = 0; j < DelSeq_No.Count; j++)
                            {
                                if (Dr["FullItemId"].ToString() == DelFullItemID[j] && int.Parse(Dr["GoodsReceivedSeqNo"].ToString()) == DelSeq_No[j])
                                {
                                    this.dgvNRB.Rows.Add(dgvNRB.RowCount + 1, Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], decimal.Parse(cmd.ExecuteScalar().ToString()) + DelUoM_Qty[j], "", Dr["UoM"], "", Dr["UoMAlt"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceivedSeqNo"], Dr["InventSiteID"], Dr["Quality"], Dr["Notes"], Dr["Qty_Actual"]);//Dr["Qty_Actual"]--UoM_Qty, Dr["TotalBerat_Actual"]
                                }
                            }
                        }
                        else
                        {
                            this.dgvNRB.Rows.Add(dgvNRB.RowCount + 1, Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], cmd.ExecuteScalar(), "", Dr["UoM"], "", Dr["UoMAlt"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceivedSeqNo"], Dr["InventSiteID"], Dr["Quality"], Dr["Notes"], Dr["Qty_Actual"]);//Dr["Qty_Actual"]--UoM_Qty, Dr["TotalBerat_Actual"]
                        }
                    }
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
            dgvNRB.Columns["Alt_Unit"].Visible = false;
            dgvNRB.Columns["Ratio_Actual"].Visible = false;
            dgvNRB.Columns["GoodsReceivedSeqNo"].Visible = false;
            dgvNRB.Columns["OldQty"].Visible = false;
            dgvNRB.Columns["JenisRetur"].Visible = false;

            dgvNRB.AutoResizeColumns();

            dgvNRB.DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRB.Columns["UoM_Qty"].DefaultCellStyle.BackColor = Color.White;
            dgvNRB.Columns["JenisRetur"].DefaultCellStyle.BackColor = Color.White;
        }

        AddItem AI = null;
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

                if (AI == null || AI.Text == "")
                {
                    AI = new AddItem();
                    AI.setParent(this);
                    AI.setMode(txtGRNum.Text, seqno);
                    AI.Show();
                }
                else if (CheckOpened(AI.Name))
                {
                    AI.WindowState = FormWindowState.Normal;
                    AI.Show();
                    AI.Focus();
                }
            }
        }

        public List<string> DelFullItemID = new List<string>();
        public List<decimal> DelUoM_Qty = new List<decimal>();
        public List<int> DelSeq_No = new List<int>();
        public string DelStatusItem = "";

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvNRB.RowCount == 0)
            {
                MessageBox.Show("Maaf data item kosong.", "Warning!!!", MessageBoxButtons.OK);
                return;
            }
            else
            {
                if (Mode == "New")
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
                else if (Mode == "Edit")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "SELECT InventSiteId, ActionCode, GoodsReceivedId, FullItemId, GoodsReceived_SeqNo, UoM_Qty FROM NotaReturBeli_Dtl WHERE NRBId = '" + NRBNumber + "' AND GoodsReceived_SeqNo = '" + dgvNRB.Rows[Index].Cells["GoodsReceivedSeqNo"].Value + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.Read())
                    {
                        if (Dr["InventSiteId"].ToString() == txtSiteID.Text && Dr["ActionCode"].ToString() == "0" + cmbJenisRetur.SelectedIndex)
                        {
                            if (DelStatusItem == "ItemDeleted")
                            {
                                for (int i = 0; i < DelSeq_No.Count; i++)
                                {
                                    if (dgvNRB.CurrentRow.Cells["FullItemID"].Value.ToString() == DelFullItemID[i] && int.Parse(dgvNRB.CurrentRow.Cells["GoodsReceivedSeqNo"].Value.ToString()) == DelSeq_No[i])
                                    {
                                        MessageBox.Show("Maaf item sudah pernah dihapus sebelumnya dengan gudang dan transaksi yang sama dengan sebelumnya.", "Warning!!!", MessageBoxButtons.OK);
                                        return;
                                    }
                                }
                            }

                            if (dgvNRB.RowCount > 0)
                            {
                                DelFullItemID.Add(Dr["FullItemId"].ToString());
                                DelSeq_No.Add(int.Parse(Dr["GoodsReceived_SeqNo"].ToString()));
                                DelUoM_Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                                DelStatusItem = "ItemDeleted";

                                dgvNRB.Rows.RemoveAt(dgvNRB.CurrentRow.Index);

                                for (int i = 0; i < dgvNRB.RowCount; i++)
                                {
                                    dgvNRB.Rows[i].Cells["No"].Value = i + 1;
                                }
                            }
                        }
                        else
                        {
                            if (dgvNRB.RowCount > 0)
                            {
                                //DelStatusItem = "";

                                dgvNRB.Rows.RemoveAt(dgvNRB.CurrentRow.Index);

                                for (int i = 0; i < dgvNRB.RowCount; i++)
                                {
                                    dgvNRB.Rows[i].Cells["No"].Value = i + 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dgvNRB.RowCount > 0)
                        {
                            //DelStatusItem = "";

                            dgvNRB.Rows.RemoveAt(dgvNRB.CurrentRow.Index);

                            for (int i = 0; i < dgvNRB.RowCount; i++)
                            {
                                dgvNRB.Rows[i].Cells["No"].Value = i + 1;
                            }
                        }
                    }
                    Dr.Close();
                    Conn.Close();
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                DialogResult dr = MessageBox.Show("Apakah anda ingin membatalkan transaksi ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    this.Close();
                    for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
                    {
                        if (Application.OpenForms[i].Name == "AddGR")
                            Application.OpenForms[i].Close();
                        else if (Application.OpenForms[i].Name == "AddItem")
                            Application.OpenForms[i].Close();
                        else if (Application.OpenForms[i].Name == "AddWH")
                            Application.OpenForms[i].Close();
                    }
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Trans.Commit();//from btn_Delete
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            decimal QtyNew = 0;
            decimal amountPU = 0, amountOH = 0;

            if (cmbJenisRetur.SelectedIndex == 0)
            {
                MessageBox.Show("Jenis Retur harus dipilih.");
                return;
            }
            if (dgvNRB.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }
            try
            {
                if (Mode == "New" && txtNRBNum.Text.Trim() == "")
                {
                    for (int i = 0; i < dgvNRB.RowCount; i++)
                    {
                        if (dgvNRB.Rows[i].Cells["UoM_Qty"].Value == "" || decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()) == 0)
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jumlah quantity belum diisi.");
                            return;
                        }
                        if (decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) < decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jumlah quantity melebihi batas.");
                            return;
                        }
                        if (dgvNRB.Rows[i].Cells["Alt_Qty"].Value.ToString() == "")
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", silahkan dimasukkan kembali Jumlah quantitynya lalu tekan enter.");
                            return;
                        }

                        string Action = dgvNRB.Rows[i].Cells["JenisRetur"].Value == null ? "" : dgvNRB.Rows[i].Cells["JenisRetur"].Value.ToString();
                        if (Action == "")
                        {
                            //MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jenis Retur belum Dipilih.");
                            //return;
                        }

                        string getFullItemID = dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString();

                        Query = "";
                        //Get Price With GRid and FullItemId
                        Query = "SELECT ISNULL(POD.Price, 0) ";
                        Query += "FROM GoodsReceivedD GRD ";
                        Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                        Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                        Query += "WHERE GRD.GoodsReceivedId = '" + txtGRNum.Text + "' AND GRD.GoodsReceivedSeqNo = '" + dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        //Cek Price Ada dalam list atau tidak
                        if (Cmd.ExecuteScalar() == null)
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                            return;
                        }

                        //cek ketersedian di gudang tertentu
                        Query = "SELECT Available_For_Sale_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                string RBFS = Dr[0].ToString();
                                if (decimal.Parse(RBFS) < decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                                {
                                    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", pada " + txtSiteName + " tidak cukup untuk melakukan retur.");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", pada " + txtSiteName + " tidak terdaftar pada database onhand.");
                            return;
                        }
                        Dr.Close();
                    }
                    
                    string Jenis = "NRB", Kode = "NRB";
                    NRBNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "INSERT INTO NotaReturBeliH (NRBId, NRBDate, NRBMode, VendId, VendName, GoodsReceivedId, GoodsReceivedDate, PurchId, SiteId, SiteName, TransStatusId, ActionCode, Notes, CreatedDate, CreatedBy) VALUES ";
                    Query += "('" + NRBNumber + "', ";
                    Query += "'" + dtNRB.Value.ToString("yyyy-MM-dd") + "', ";
                    Query += "'MANUAL', ";
                    Query += "'" + txtVendID.Text + "', ";
                    Query += "'" + txtVendName.Text + "', ";
                    Query += "'" + txtGRNum.Text + "', ";
                    Query += "'" + dtGR.Value.ToString("yyyy-MM-dd") + "', ";
                    Query += "'" + txtPONum.Text + "', ";
                    Query += "'" + txtSiteID.Text + "', ";
                    Query += "'" + txtSiteName.Text + "', ";
                    Query += "'13', ";
                    if (cmbJenisRetur.SelectedIndex == 1)
                    {
                        Query += "'01', ";
                    }
                    else if (cmbJenisRetur.SelectedIndex == 2)
                    {
                        Query += "'02', ";
                    }
                    else if (cmbJenisRetur.SelectedIndex == 3)
                    {
                        Query += "'03', ";
                    }
                    else
                    {
                        Query += "'', ";
                    }
                    Query += "'" + txtNotes.Text + "', ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "";
                    for (int i = 0; i < dgvNRB.RowCount; i++)
                    {
                        //if (dgvNRB.Rows[i].Cells["UoM_Qty"].Value == null || decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()) == 0)
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jumlah quantity belum diisi.");
                        //    return;
                        //}
                        //if (decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) < decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jumlah quantity melebihi batas.");
                        //    return;
                        //}
                        //if (dgvNRB.Rows[i].Cells["Alt_Qty"].Value.ToString() == "")
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", silahkan dimasukkan kembali Jumlah quantitynya lalu tekan enter.");
                        //    return;
                        //}
                        
                        Query = "INSERT INTO NotaReturBeli_Dtl( ";
                        Query += "NRBId,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemId,ItemName,GoodsReceivedId,GoodsReceived_SeqNo,";
                        Query += "UoM_Qty,UoM_Unit,Alt_Qty,Alt_Unit,Ratio,Ratio_Actual,RemainingQty,Quality,InventSiteId,ActionCode,Notes,CreatedBy,[Price],[Total],[Total_Discount],[PPN],[Total_PPN],[PPH],[Total_PPH]) ";
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
                        //if (Action == "Retur Tukar Barang")
                        //{
                        //    Query += "01', '";
                        //}
                        //else if (Action == "Retur Debet Note")
                        //{
                        //    Query += "02', '";
                        //}
                        //else
                        //{
                            if (cmbJenisRetur.SelectedIndex == 1)
                            {
                                Query += "01', '";
                            }
                            else if (cmbJenisRetur.SelectedIndex == 2)
                            {
                                Query += "02', '";
                            }
                            else if (cmbJenisRetur.SelectedIndex == 3)
                            {
                                Query += "03', '";
                            }
                            else
                            {
                                Query += "', '";
                            }
                        //}
                        
                        Query += (dgvNRB.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNRB.Rows[i].Cells["Notes"].Value.ToString()) + "', '";
                        Query += ControlMgr.UserId + "', ";

                        //get price, total, total discount, ppn,total ppn, pph, total pph
                        //Nominal[0] = price, [1] = total, [2] = total discount, [3] = ppn, [4] = total ppn, [5] = pph, [6] = total pph
                        decimal[] Nominal = GetPriceTotalPPNPPHDiscount(txtGRNum.Text, Convert.ToInt32(dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value), dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString());

                        Query += " "+Nominal[0]+","+Nominal[1]+","+Nominal[2]+","+Nominal[3]+","+Nominal[4]+","+Nominal[5]+","+Nominal[6]+" ) ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) - decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString());
                        Query = "UPDATE GoodsReceivedD SET Remaining_Qty = '" + QtyNew + "'";
                        Query += "WHERE GoodsReceivedId = '" + txtGRNum.Text.Trim() + "' AND FullItemId = '" + dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        string getFullItemID = dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString();

                        Query = "";
                        //Get Price With GRid and FullItemId
                        Query = "SELECT ISNULL(POD.Price, 0) ";
                        Query += "FROM GoodsReceivedD GRD ";
                        Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                        Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                        Query += "WHERE GRD.GoodsReceivedId = '" + txtGRNum.Text + "' AND GRD.GoodsReceivedSeqNo = '" + dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        ////Cek Price Ada dalam list atau tidak
                        //if (Cmd.ExecuteScalar() == null)
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                        //    return;
                        //}

                        string getUoM_Qty = dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString();
                        string getAlt_Qty = dgvNRB.Rows[i].Cells["Alt_Qty"].Value.ToString();
                        string Price = Cmd.ExecuteScalar().ToString();
                        amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                        amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                        Query = "";
                        //Get Retur_Beli_In_Progress_UoM, Retur_Beli_In_Progress_Alt, Retur_Beli_In_Progress_Amount (Old)
                        Query = "SELECT Retur_Beli_In_Progress_UoM FROM Invent_Purchase_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Beli_In_Progress_Alt FROM Invent_Purchase_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Beli_In_Progress_Amount FROM Invent_Purchase_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RBIP_UoM_New = decimal.Parse(RBIP_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIP_Alt_New = decimal.Parse(RBIP_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIP_Amount_New = decimal.Parse(RBIP_Amount_Old.ToString()) + decimal.Parse(amountPU.ToString());
                        Query = "";
                        //Update Invent_Purchase_Qty
                        Query = "UPDATE Invent_Purchase_Qty SET ";
                        Query += "Retur_Beli_In_Progress_UoM = '" + RBIP_UoM_New + "', ";
                        Query += "Retur_Beli_In_Progress_Alt = '" + RBIP_Alt_New + "', ";
                        Query += "Retur_Beli_In_Progress_Amount = '" + RBIP_Amount_New + "' ";
                        Query += "WHERE FullItemId = '"+getFullItemID+"' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Get Available_For_Sale_UoM,Available_For_Sale_Alt,Available_For_Sale_Amount (Old)
                        Query = "SELECT Available_For_Sale_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_For_Sale_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_For_Sale_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Amount_Old = Cmd.ExecuteScalar().ToString();

                        //Get Available_UoM, Available_Alt, Available_Amount (Old)
                        Query = "SELECT Available_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RBIO_UoM_New = decimal.Parse(RBIO_UoM_Old.ToString()) - decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIO_Alt_New = decimal.Parse(RBIO_Alt_Old.ToString()) - decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIO_Amount_New = decimal.Parse(RBIO_Amount_Old.ToString()) - decimal.Parse(amountOH.ToString());

                        decimal RBIO_Av_UoM_New = decimal.Parse(RBIO_Av_UoM_Old.ToString()) - decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIO_Av_Alt_New = decimal.Parse(RBIO_Av_Alt_Old.ToString()) - decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIO_Av_Amount_New = decimal.Parse(RBIO_Av_Amount_Old.ToString()) - decimal.Parse(amountOH.ToString());
                        
                        Query = "";
                        //Update Invent_OnHand_Qty
                        Query = "UPDATE Invent_OnHand_Qty SET ";
                        Query += "Available_For_Sale_UoM = '" + RBIO_UoM_New + "', ";
                        Query += "Available_For_Sale_Alt = '" + RBIO_Alt_New + "', ";
                        Query += "Available_For_Sale_Amount = '" + RBIO_Amount_New + "', ";
                        Query += "Available_UoM = '" + RBIO_Av_UoM_New + "', ";
                        Query += "Available_Alt = '" + RBIO_Av_Alt_New + "', ";
                        Query += "Available_Amount = '" + RBIO_Av_Amount_New + "' ";
                        Query += "WHERE FullItemId = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        
                        Query = "";
                        //Insert to NotaReturBeli_LogTable
                        Query = "INSERT INTO NotaReturBeli_LogTable ";
                        Query += "(NRBDate, NRBId, GoodsReceivedDate, GoodsReceivedId, ";
                        Query += "VendId, SiteId, FullItemId, SeqNo, ";
                        Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                        Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                        Query += "('" + dtNRB.Value.ToString("yyyy-MM-dd") + "', '" + NRBNumber + "', '" + dtGR.Value.ToString("yyyy-MM-dd") + "', '" + txtGRNum.Text + "', ";
                        Query += "'" + txtVendID.Text + "', '" + txtSiteID.Text + "', '" + getFullItemID + "', '" + dgvNRB.Rows[i].Cells["No"].Value.ToString() + "', ";
                        Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amountPU + "', '13', ";
                        Query += "'Waiting for Approval', 'Status: 13. Waiting for Approval', '" + ControlMgr.UserId + "', GETDATE()) ";
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

                    //Begin
                    //Created By : Joshua
                    //Created Date : 06 Sep 2018
                    //Desc : Create Journal
                    CreateJournal(NRBNumber);
                    //End

                    Trans.Commit();
                    MessageBox.Show("Data NRBNumber : " + NRBNumber + " berhasil ditambahkan.");
                    txtNRBNum.Text = NRBNumber;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
                else
                {
                    for (int i = 0; i < dgvNRB.RowCount; i++)
                    {
                        if (dgvNRB.Rows[i].Cells["UoM_Qty"].Value == "" || decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()) == 0)
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jumlah quantity belum diisi.");
                            return;
                        }
                        if (decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) < decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jumlah quantity melebihi batas.");
                            return;
                        }
                        if (dgvNRB.Rows[i].Cells["Alt_Qty"].Value.ToString() == "")
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", silahkan dimasukkan kembali Jumlah quantitynya lalu tekan enter.");
                            return;
                        }

                        string Action = dgvNRB.Rows[i].Cells["JenisRetur"].Value == null ? "" : dgvNRB.Rows[i].Cells["JenisRetur"].Value.ToString();
                        if (Action == "")
                        {
                            //MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jenis Retur belum Dipilih.");
                            //return;
                        }

                        string getFullItemID = dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString();

                        Query = "";
                        //Get Price With GRid and FullItemId
                        Query = "SELECT ISNULL(POD.Price, 0) ";
                        Query += "FROM GoodsReceivedD GRD ";
                        Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                        Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                        Query += "WHERE GRD.GoodsReceivedId = '" + txtGRNum.Text + "' AND GRD.GoodsReceivedSeqNo = '" + dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        //Cek Price Ada dalam list atau tidak
                        if (Cmd.ExecuteScalar() == null)
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                            return;
                        }

                        //cek ketersedian di gudang tertentu
                        Query = "SELECT Available_For_Sale_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBFS = Cmd.ExecuteScalar().ToString();
                        if (decimal.Parse(RBFS) < decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", pada " + txtSiteName.Text + " tidak cukup untuk melakukan retur.");
                            return;
                        }
                    }

                    DateTime CreatedDate = DateTime.Now;
                    String CreatedBy = "";
                    
                    //Select SiteId Lama Untuk Invent Onhand
                    Query = "SELECT SiteId FROM NotaReturBeliH WHERE NRBId = '" + txtNRBNum.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string SiteId_Old = Cmd.ExecuteScalar().ToString();

                    Query = "UPDATE NotaReturBeliH SET ";
                    Query += "VendId = '" + txtVendID.Text + "',";
                    Query += "VendName = '" + txtVendName.Text + "',";
                    Query += "GoodsReceivedId = '" + txtGRNum.Text + "',";
                    Query += "SiteId = '" + txtSiteID.Text + "',";
                    Query += "SiteName = '" + txtSiteName.Text + "',";
                    Query += "TransStatusId = '13',";
                    if (cmbJenisRetur.SelectedIndex == 1)
                    {
                        Query += "ActionCode = '01', ";
                    }
                    else if (cmbJenisRetur.SelectedIndex == 2)
                    {
                        Query += "ActionCode = '02', ";
                    }
                    else if (cmbJenisRetur.SelectedIndex == 3)
                    {
                        Query += "ActionCode = '03', ";
                    }
                    Query += "Notes = '" + txtNotes.Text + "',";
                    Query += "UpdatedDate = GETDATE(),";
                    Query += "UpdatedBy = '" + ControlMgr.UserId + "',";
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
                    List<string> FullItemID = new List<string>();
                    List<decimal> UoM_Qty = new List<decimal>();
                    List<decimal> Alt_Qty = new List<decimal>();
                    decimal RemainingQty, QtyNew2 = 0;
                    Query = "SELECT GoodsReceived_SeqNo, GoodsReceivedID, FullItemId, UoM_Qty, Alt_Qty FROM NotaReturBeli_Dtl WHERE NRBId='" + txtNRBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        GoodsReceivedSeqNo.Add(Dr["GoodsReceived_SeqNo"].ToString());
                        GoodsReceivedID.Add(Dr["GoodsReceivedID"].ToString());
                        FullItemID.Add(Dr["FullItemId"].ToString());
                        UoM_Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                        Alt_Qty.Add(decimal.Parse(Dr["Alt_Qty"].ToString()));

                    }
                    Dr.Close();

                    for (int i = 0; i < GoodsReceivedSeqNo.Count; i++)
                    {
                        Query = "SELECT Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                        QtyNew2 = RemainingQty + UoM_Qty[i];

                        Query = "UPDATE GoodsReceivedD SET ";
                        Query += "Remaining_Qty = '" + QtyNew2 + "' ";
                        Query += "WHERE GoodsReceivedId = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo='" + GoodsReceivedSeqNo[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Get Price With GRid and FullItemId
                        Query = "SELECT POD.Price ";
                        Query += "FROM GoodsReceivedD GRD ";
                        Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                        Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                        Query += "WHERE GRD.GoodsReceivedId = '" + txtGRNum.Text + "' AND POD.FullItemId = '" + FullItemID[i] + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        ////Cek Price Ada dalam list atau tidak
                        //if (Cmd.ExecuteScalar() == null)
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                        //    return;
                        //}
                        string Price = "";
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                if (Dr[0] == System.DBNull.Value)
                                {
                                    Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + FullItemID[i] + "'";
                                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                                    {
                                        Price = Cmd.ExecuteScalar().ToString();
                                    }
                                }
                                else
                                {
                                    Price = Dr[0].ToString();
                                }
                            }
                        }
                        else
                        {
                            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + FullItemID[i] + "'";
                            using (Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Price = Cmd.ExecuteScalar().ToString();
                            }
                        }
                        Dr.Close();

                        string getUoM_Qty = UoM_Qty[i].ToString();
                        string getAlt_Qty = Alt_Qty[i].ToString();
                        amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                        amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                        Query = "";
                        //Get Retur_Beli_In_Progress_UoM, Retur_Beli_In_Progress_Alt, Retur_Beli_In_Progress_Amount (Old)
                        Query = "SELECT Retur_Beli_In_Progress_UoM FROM Invent_Purchase_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Beli_In_Progress_Alt FROM Invent_Purchase_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Beli_In_Progress_Amount FROM Invent_Purchase_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RBIP_UoM_New = decimal.Parse(RBIP_UoM_Old.ToString()) - decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIP_Alt_New = decimal.Parse(RBIP_Alt_Old.ToString()) - decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIP_Amount_New = decimal.Parse(RBIP_Amount_Old.ToString()) - decimal.Parse(amountPU.ToString());
                        Query = "";
                        //Update Invent_Purchase_Qty
                        Query = "UPDATE Invent_Purchase_Qty SET ";
                        Query += "Retur_Beli_In_Progress_UoM = '" + RBIP_UoM_New + "', ";
                        Query += "Retur_Beli_In_Progress_Alt = '" + RBIP_Alt_New + "', ";
                        Query += "Retur_Beli_In_Progress_Amount = '" + RBIP_Amount_New + "' ";
                        Query += "WHERE FullItemId = '" + FullItemID[i] + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Get Available_For_Sale_UoM,Available_For_Sale_Alt,Available_For_Sale_Amount (Old)
                        Query = "SELECT Available_For_Sale_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + SiteId_Old + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_For_Sale_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + SiteId_Old + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_For_Sale_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + SiteId_Old + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Amount_Old = Cmd.ExecuteScalar().ToString();

                        //Get Available_UoM, Available_Alt, Available_Amount (Old)
                        Query = "SELECT Available_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + SiteId_Old + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + SiteId_Old + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + SiteId_Old + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RBIO_UoM_New = decimal.Parse(RBIO_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIO_Alt_New = decimal.Parse(RBIO_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIO_Amount_New = decimal.Parse(RBIO_Amount_Old.ToString()) + decimal.Parse(amountOH.ToString());

                        decimal RBIO_Av_UoM_New = decimal.Parse(RBIO_Av_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIO_Av_Alt_New = decimal.Parse(RBIO_Av_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIO_Av_Amount_New = decimal.Parse(RBIO_Av_Amount_Old.ToString()) + decimal.Parse(amountOH.ToString());

                        Query = "";
                        //Update Invent_OnHand_Qty
                        Query = "UPDATE Invent_OnHand_Qty SET ";
                        Query += "Available_For_Sale_UoM = '" + RBIO_UoM_New + "', ";
                        Query += "Available_For_Sale_Alt = '" + RBIO_Alt_New + "', ";
                        Query += "Available_For_Sale_Amount = '" + RBIO_Amount_New + "', ";
                        Query += "Available_UoM = '" + RBIO_Av_UoM_New + "', ";
                        Query += "Available_Alt = '" + RBIO_Av_Alt_New + "', ";
                        Query += "Available_Amount = '" + RBIO_Av_Amount_New + "' ";
                        Query += "WHERE FullItemId = '" + FullItemID[i] + "' AND InventSiteId = '" + SiteId_Old + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        
                        Query = "";
                    }
                    //Delete to NotaReturBeli_LogTable yg sudah d.buat sebelumnya
                    Query = "DELETE FROM NotaReturBeli_LogTable WHERE NRBId = '" + txtNRBNum.Text + "' AND LogStatusCode = '13'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "DELETE FROM NotaReturBeli_Dtl WHERE NRBId='" + txtNRBNum.Text.Trim() + "'";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    Query = "";

                    for (int i = 0; i < dgvNRB.RowCount; i++)
                    {
                        //if (dgvNRB.Rows[i].Cells["UoM_Qty"].Value == null || decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()) == 0)
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jumlah quantity belum diisi.");
                        //    return;
                        //}
                        //if ((decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) + decimal.Parse(dgvNRB.Rows[i].Cells["OldQty"].Value.ToString())) < decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", Jumlah quantity melebihi batas.");
                        //    return;
                        //}
                        //if (dgvNRB.Rows[i].Cells["Alt_Qty"].Value.ToString() == "")
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", silahkan dimasukkan kembali Jumlah quantitynya lalu tekan enter.");
                        //    return;
                        //}

                        Query = "INSERT INTO NotaReturBeli_Dtl( ";
                        Query += "NRBId,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemId,ItemName,GoodsReceivedId,GoodsReceived_SeqNo,";
                        Query += "UoM_Qty,UoM_Unit,Alt_Qty,Alt_Unit,Ratio,Ratio_Actual,RemainingQty,Quality,InventSiteId,ActionCode,Notes,CreatedBy,[Price],[Total],[Total_Discount],[PPN],[Total_PPN],[PPH],[Total_PPH]) ";
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
                        //if (Action == "Retur Tukar Barang")
                        //{
                        //    Query += "01', '";
                        //}
                        //else if (Action == "Retur Debet Note")
                        //{
                        //    Query += "02', '";
                        //}
                        //else
                        //{
                            if (cmbJenisRetur.SelectedIndex == 1)
                            {
                                Query += "01', '";
                            }
                            else if (cmbJenisRetur.SelectedIndex == 2)
                            {
                                Query += "02', '";
                            }
                            else if (cmbJenisRetur.SelectedIndex == 3)
                            {
                                Query += "03', '";
                            }
                            else
                            {
                                Query += "', '";
                            }
                        //}

                        Query += (dgvNRB.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNRB.Rows[i].Cells["Notes"].Value.ToString()) + "', '";
                        Query += ControlMgr.UserId + "', ";

                        //get price, total, total discount, ppn,total ppn, pph, total pph
                        //Nominal[0] = price, [1] = total, [2] = total discount, [3] = ppn, [4] = total ppn, [5] = pph, [6] = total pph
                        decimal[] Nominal = GetPriceTotalPPNPPHDiscount(txtGRNum.Text, Convert.ToInt32(dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value), dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString());

                        Query += " "+Nominal[0]+","+Nominal[1]+","+Nominal[2]+","+Nominal[3]+","+Nominal[4]+","+Nominal[5]+","+Nominal[6]+" )";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvNRB.Rows[i].Cells["RemainingQty"].Value.ToString()) - decimal.Parse(dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString());
                        Query = "UPDATE GoodsReceivedD SET Remaining_Qty = '" + QtyNew + "'";
                        Query += "WHERE GoodsReceivedId = '" + txtGRNum.Text.Trim() + "' AND GoodsReceivedSeqNo='" + dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        string getFullItemID = dgvNRB.Rows[i].Cells["FullItemID"].Value.ToString();

                        Query = "";
                        //Get Price With GRid and FullItemId
                        Query = "SELECT POD.Price ";
                        Query += "FROM GoodsReceivedD GRD ";
                        Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                        Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                        Query += "WHERE GRD.GoodsReceivedId = '" + txtGRNum.Text + "' AND POD.FullItemId = '" + getFullItemID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        ////Cek Price Ada dalam list atau tidak
                        //if (Cmd.ExecuteScalar() == null)
                        //{
                        //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                        //    return;
                        //}
                        string Price = "";
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                if (Dr[0] == System.DBNull.Value)
                                {
                                    Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + FullItemID[i] + "'";
                                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                                    {
                                        Price = Cmd.ExecuteScalar().ToString();
                                    }
                                }
                                else
                                {
                                    Price = Dr[0].ToString();
                                }
                            }
                        }
                        else
                        {
                            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + getFullItemID + "'";
                            using (Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Price = Cmd.ExecuteScalar().ToString();
                            }
                        }
                        Dr.Close();

                        string getUoM_Qty = dgvNRB.Rows[i].Cells["UoM_Qty"].Value.ToString();
                        string getAlt_Qty = dgvNRB.Rows[i].Cells["Alt_Qty"].Value.ToString();
                        amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                        amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                        Query = "";
                        //Get Retur_Beli_In_Progress_UoM, Retur_Beli_In_Progress_Alt, Retur_Beli_In_Progress_Amount (Old)
                        Query = "SELECT Retur_Beli_In_Progress_UoM FROM Invent_Purchase_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Beli_In_Progress_Alt FROM Invent_Purchase_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Beli_In_Progress_Amount FROM Invent_Purchase_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIP_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RBIP_UoM_New = decimal.Parse(RBIP_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIP_Alt_New = decimal.Parse(RBIP_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIP_Amount_New = decimal.Parse(RBIP_Amount_Old.ToString()) + decimal.Parse(amountPU.ToString());
                        Query = "";
                        //Update Invent_Purchase_Qty
                        Query = "UPDATE Invent_Purchase_Qty SET ";
                        Query += "Retur_Beli_In_Progress_UoM = '" + RBIP_UoM_New + "', ";
                        Query += "Retur_Beli_In_Progress_Alt = '" + RBIP_Alt_New + "', ";
                        Query += "Retur_Beli_In_Progress_Amount = '" + RBIP_Amount_New + "' ";
                        Query += "WHERE FullItemId = '" + getFullItemID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Get Available_For_Sale_UoM,Available_For_Sale_Alt,Available_For_Sale_Amount (Old)
                        Query = "SELECT Available_For_Sale_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_For_Sale_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_For_Sale_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Amount_Old = Cmd.ExecuteScalar().ToString();

                        //Get Available_UoM, Available_Alt, Available_Amount (Old)
                        Query = "SELECT Available_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_Alt_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Available_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RBIO_Av_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RBIO_UoM_New = decimal.Parse(RBIO_UoM_Old.ToString()) - decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIO_Alt_New = decimal.Parse(RBIO_Alt_Old.ToString()) - decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIO_Amount_New = decimal.Parse(RBIO_Amount_Old.ToString()) - decimal.Parse(amountOH.ToString());

                        decimal RBIO_Av_UoM_New = decimal.Parse(RBIO_Av_UoM_Old.ToString()) - decimal.Parse(getUoM_Qty.ToString());
                        decimal RBIO_Av_Alt_New = decimal.Parse(RBIO_Av_Alt_Old.ToString()) - decimal.Parse(getAlt_Qty.ToString());
                        decimal RBIO_Av_Amount_New = decimal.Parse(RBIO_Av_Amount_Old.ToString()) - decimal.Parse(amountOH.ToString());

                        Query = "";
                        //Update Invent_OnHand_Qty
                        Query = "UPDATE Invent_OnHand_Qty SET ";
                        Query += "Available_For_Sale_UoM = '" + RBIO_UoM_New + "', ";
                        Query += "Available_For_Sale_Alt = '" + RBIO_Alt_New + "', ";
                        Query += "Available_For_Sale_Amount = '" + RBIO_Amount_New + "', ";
                        Query += "Available_UoM = '" + RBIO_Av_UoM_New + "', ";
                        Query += "Available_Alt = '" + RBIO_Av_Alt_New + "', ";
                        Query += "Available_Amount = '" + RBIO_Av_Amount_New + "' ";
                        Query += "WHERE FullItemId = '" + getFullItemID + "' AND InventSiteId = '" + txtSiteID.Text + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Insert to NotaReturBeli_LogTable
                        Query = "INSERT INTO NotaReturBeli_LogTable ";
                        Query += "(NRBDate, NRBId, GoodsReceivedDate, GoodsReceivedId, ";
                        Query += "VendId, SiteId, FullItemId, SeqNo, ";
                        Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                        Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                        Query += "('" + dtNRB.Value.ToString("yyyy-MM-dd") + "', '" + NRBNumber + "', '" + dtGR.Value.ToString("yyyy-MM-dd") + "', '" + txtGRNum.Text + "', ";
                        Query += "'" + txtVendID.Text + "', '" + txtSiteID.Text + "', '" + getFullItemID + "', '" + dgvNRB.Rows[i].Cells["No"].Value.ToString() + "', ";
                        Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amountPU + "', '13', ";
                        Query += "'Waiting for Approval', 'Status: 13. Waiting for Approval', '" + ControlMgr.UserId + "', GETDATE()) ";
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

                    //Begin
                    //Created By : Joshua
                    //Created Date : 06 Sep 2018
                    //Desc : Update Journal
                    UpdateJournal();
                    //if (Journal == true)
                    //{
                    //    Journal = false;
                    //    goto Outer;
                    //}
                    //End

                    Trans.Commit();
                    MessageBox.Show("Data NRBNumber : " + txtNRBNum.Text + " berhasil diupdate.");

                    Parent.RefreshGrid();
                    ModeBeforeEdit();

                    //Outer: ;
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
                DelStatusItem = "";
                DelSeq_No.Clear();
                DelFullItemID.Clear();
                DelUoM_Qty.Clear();
            }
        }

        private decimal[] GetPriceTotalPPNPPHDiscount(string GRNUM, int GRSeqNo, string FullItemId)
        {
            decimal price = 0;
            decimal Total = 0;
            decimal Total_Discount = 0;
            decimal PPN = 0;
            decimal PPH = 0;
            decimal Total_PPH = 0;
            decimal Total_PPN = 0;
            string QueryNew = "SELECT [Price],[Total],[Total_Discount],[PPN],[Total_PPN],[PPH],[Total_PPH] FROM [ISBS-NEW4].[dbo].[GoodsReceivedD] WHERE [GoodsReceivedId] = @GoodsReceivedId AND [GoodsReceivedSeqNo] = @GoodsReceivedSeqNo";
            using (Cmd = new SqlCommand(QueryNew, Conn, Trans))
            {
                Cmd.Parameters.AddWithValue("@GoodsReceivedId", GRNUM);
                Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", GRSeqNo);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        if (Dr["Price"] != System.DBNull.Value)
                        {
                            price = Convert.ToDecimal(Dr["Price"]);
                            Total = Convert.ToDecimal(Dr["Total"]); ;
                            Total_Discount = Convert.ToDecimal(Dr["Total_Discount"]); ;
                            PPN = Convert.ToDecimal(Dr["PPN"]); ;
                            PPH = Convert.ToDecimal(Dr["PPH"]); ;
                            Total_PPH = Convert.ToDecimal(Dr["Total_PPH"]); ;
                            Total_PPN = Convert.ToDecimal(Dr["Total_PPN"]); ;
                        }
                    }
                }
                Dr.Close();
            }
            if (price == 0)
            {
                price = GetPriceFromInventTable("UoM_AvgPrice", FullItemId);
            }
            decimal[] Nominal = new decimal[] { price, Total, Total_Discount, PPN, Total_PPN,PPH,Total_PPH };
            return Nominal;
        }

        private void CreateJournal(string NRBNumber)
        {
            //Begin
            //Created By : Joshua
            //Created Date : 05 Sept 2018
            //Desc : Create Journal

            decimal Retur = 0;

            for (int i = 0; i < dgvNRB.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dgvNRB.Rows[i].Cells["FullItemID"].Value);
                decimal Qty = Convert.ToDecimal(dgvNRB.Rows[i].Cells["UoM_Qty"].Value);
                decimal Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);

                Retur = Retur + (Qty * Price);
            }

            if (Retur != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "IN41";
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);
                string Notes = txtGRNum.Text + " - " + txtVendName.Text;

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + NRBNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

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

                    if (JournalHID == "IN41")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            AmountValue = Retur;
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            AmountValue = Retur;
                        }
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

            //End
        }

        private void UpdateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date ; 06 Sept 2018
            //Desc : Update Journal

            decimal OldRetur = 0, Retur = 0;

            //Get GLJournalHID
            Query = "SELECT TOP 1 GLJournalHID FROM GLJournalH WHERE Referensi = '" + txtNRBNum.Text + "' ORDER BY CreatedDate DESC";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            Query = "SELECT TOP 1 Amount FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "' AND SeqNo = 1";
            Cmd = new SqlCommand(Query, Conn, Trans);
            OldRetur = Convert.ToDecimal(Cmd.ExecuteScalar());

            for (int i = 0; i < dgvNRB.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dgvNRB.Rows[i].Cells["FullItemID"].Value);
                decimal Qty = Convert.ToDecimal(dgvNRB.Rows[i].Cells["UoM_Qty"].Value);
                decimal Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);

                Retur = Retur + (Qty * Price);
            }

            if (Retur != 0 && OldRetur != Retur)
            {
                //Reverse Journal
                //Insert Header GLJournal
                Query = "SELECT TOP 1 Notes FROM GLJournalH WHERE GLJournalHID = '" + GLJournalHID + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);       
                string Notes = Convert.ToString(Cmd.ExecuteScalar());

                string JournalHID = "IN49";
                string Jenis = "JN", Kode = "JN";
                GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);                       

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + NRBNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

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

                    if (JournalHID == "IN49")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            AmountValue = OldRetur;
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            AmountValue = OldRetur;
                        }
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

                //New Journal
                //Insert Header GLJournal
                JournalHID = "IN41";
                GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);
                Notes = txtGRNum.Text + " - " + txtVendName.Text;

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + NRBNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                //Select Config Journal
                SeqNo = 1;
                JournalIDSeqNo = 0;
                Type = "";
                FQA_ID = "";
                FQA_Desc = "";
                AmountValue = 0;

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

                    if (JournalHID == "IN41")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            AmountValue = Retur;
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            AmountValue = Retur;
                        }
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

            //decimal Retur = 0;

            //for (int i = 0; i < dgvNRB.RowCount; i++)
            //{
            //    string FullItemID = Convert.ToString(dgvNRB.Rows[i].Cells["FullItemID"].Value);
            //    decimal Qty = Convert.ToDecimal(dgvNRB.Rows[i].Cells["UoM_Qty"].Value);
            //    decimal Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);

            //    Retur = Retur + (Qty * Price);
            //}

            ////Get GLJournalHID
            //Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + txtNRBNum.Text + "' ";
            //Cmd = new SqlCommand(Query, Conn, Trans);
            //string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            //Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
            //Cmd = new SqlCommand(Query, Conn, Trans);
            //int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

            //if (CountData == 1)
            //{
            //    //Delete Journal Detail
            //    Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Cmd.ExecuteNonQuery();
            //}
            //else
            //{
            //    MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    Journal = true;
            //    return;
            //}

            //if (Retur != 0)
            //{
            //    //Insert Header GLJournal
            //    string JournalHID = "IN41";               

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
            //        AmountValue = 0;

            //        if (JournalHID == "IN41")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = Retur;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = Retur;
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
            //End
        }

        private decimal GetPriceFromInventTable(string FieldName, string FullItemID)
        {
            string QueryNew = "SELECT " + FieldName + " FROM InventTable WHERE FullItemID = '" + FullItemID + "'";

            Cmd = new SqlCommand(QueryNew, Conn, Trans);
            string result = Convert.ToString(Cmd.ExecuteScalar());
            decimal Price;
            if (result == "")
            {
                Price = 1;
            }
            else if (Convert.ToDecimal(result) == 0)
            {
                Price = 1;
            }
            else
            {
                Price = Convert.ToDecimal(result);
            }
            return Price;
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
                        dgvNRB.CurrentRow.Cells["Alt_Qty"].Value = decimal.Parse(dgvNRB.CurrentRow.Cells["UoM_Qty"].Value.ToString()) * decimal.Parse(dgvNRB.CurrentRow.Cells["Ratio"].Value.ToString());
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

        private bool CheckOpened(string name)
        {
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

        private void cmbJenisRetur_MouseClick(object sender, MouseEventArgs e)
        {
            if (Mode == "Edit")
            {
                if (dgvNRB.RowCount > 0)
                {
                    MessageBox.Show("Jika Anda mengganti Type Transaksi, hapus item yang sudah ada terlebih dahulu", "Warning!!!", MessageBoxButtons.OK);
                    return;
                }
            }
        }
        //Tia 26062018
        //klik kanan
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.Vendor.Vendor Vend = null;
        PopUp.InventSite Inventsite = null;
        Purchase.PurchaseOrderNew.POForm PONumber = null;
        ISBS_New.Purchase.GoodsReceipt.GRHeaderV2 Gr = null;

        Purchase.NotaDebet.FrmT_NotaDebet ParentToND;
        AccountPayable.HeaderAccountsPayable ParentToAP;
        Sales.BBK.BBKHeader ParentToGI; 

       
        public void ParentRefreshGrid(Purchase.NotaDebet.FrmT_NotaDebet nd)
        {
            ParentToND = nd;
        }

        public void ParentRefreshGrid2(AccountPayable.HeaderAccountsPayable ap)
        {
            ParentToAP = ap;
        }

        public void ParentRefreshGrid3(Sales.BBK.BBKHeader gi)
        {
            ParentToGI = gi;
        }

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvNRB_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID==null || FID.Text=="")
                {
                    if (dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        //PopUpItemName.Close();
                        // PopUpItemName = new PopUp.Stock.Stock();
                        //PopUpItemName = new PopUp.FullItemId.FullItemId();
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
                if (Inventsite == null || Inventsite.Text == "")
                {
                    if (dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "InventSiteId")
                    {
                        Inventsite = new PopUp.InventSite();
                        Inventsite.GetData(dgvNRB.Rows[e.RowIndex].Cells["InventSiteId"].Value.ToString(), dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        Inventsite.Show();
                    }
                }
                else if (CheckOpened(Inventsite.Name))
                {
                    Inventsite.WindowState = FormWindowState.Normal;
                    Inventsite.GetData(dgvNRB.Rows[e.RowIndex].Cells["InventSiteId"].Value.ToString(), dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    Inventsite.Show();
                    Inventsite.Focus();
                }
            }
        }

        private void txtVendID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    txtVendID.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtVendID.Text);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.Show();
                    Vend.Focus();
                }
            }

        }

        private void txtVendName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    txtVendName.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtVendID.Text);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.Show();
                    Vend.Focus();
                }
            }
        }

        private void txtPONum_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (PONumber == null || PONumber.Text == "")
                {
                    txtPONum.Enabled = true;
                    PONumber = new Purchase.PurchaseOrderNew.POForm();
                    PONumber.SetMode("PopUp", txtPONum.Text, "");
                    PONumber.ParentRefreshGrid2(this);
                    PONumber.Show();
                }
                else if (CheckOpened(PONumber.Name))
                {
                    PONumber.WindowState = FormWindowState.Normal;
                    PONumber.Show();
                    PONumber.Focus();
                }
            }
        }

        private void txtGRNum_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Gr == null || Gr.Text == "")
                {
                    Gr = new Purchase.GoodsReceipt.GRHeaderV2("Receipt Order");
                    Gr.SetMode("PopUp", txtGRNum.Text);
                    Gr.ParentRefreshGrid2(this);
                    Gr.Show();
                    //}
                }
                else if (CheckOpened(Gr.Name))
                {
                    Gr.WindowState = FormWindowState.Normal;
                    Gr.ParentRefreshGrid2(this);                   
                    Gr.Show();
                    Gr.Focus();
                }
            }
        }
        //End
    }
}
