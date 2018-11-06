using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Sales.NotaReturJual
{
    public partial class NRJHeader : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr, Dr_Bantu1;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Status, Query, crit, NRJNumber, GRID = null;
        Sales.NotaReturJual.InqNRJCreate Parent;
        int Index;
        //tia edit
        ContextMenu vendid = new ContextMenu();
        //tia edit end

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public NRJHeader()
        {
            InitializeComponent();
        }

        public void setParent(Sales.NotaReturJual.InqNRJCreate f)
        {
            Parent = f;
        }

        private void NRJHeader_Load(object sender, EventArgs e)
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
               // GetDataHeader();
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

        public void SetMode(string tmpMode, string tmpNRJNumber)
        {
            Mode = tmpMode;
            NRJNumber = tmpNRJNumber;
        }

        public void CreateGrid()
        {
            if (dgvNRJ.RowCount == 0)
            {
                dgvNRJ.Rows.Clear();
                dgvNRJ.ColumnCount = 18;
                dgvNRJ.Columns[0].Name = "No";
                dgvNRJ.Columns[1].Name = "ItemID";
                dgvNRJ.Columns[2].Name = "FullItemID"; dgvNRJ.Columns["FullItemID"].HeaderText = "Item ID";
                dgvNRJ.Columns[3].Name = "ItemName"; dgvNRJ.Columns["ItemName"].HeaderText = "Name";
                dgvNRJ.Columns[4].Name = "GroupId";
                dgvNRJ.Columns[5].Name = "SubGroup1ID";
                dgvNRJ.Columns[6].Name = "SubGroup2ID";
                dgvNRJ.Columns[7].Name = "RemainingQty";
                dgvNRJ.Columns[8].Name = "UoM_Qty"; dgvNRJ.Columns["UoM_Qty"].HeaderText = "Retur Qty";
                dgvNRJ.Columns[9].Name = "UoM_Unit";
                dgvNRJ.Columns[10].Name = "Alt_Qty";
                dgvNRJ.Columns[11].Name = "Alt_Unit";
                dgvNRJ.Columns[12].Name = "Ratio";
                dgvNRJ.Columns[13].Name = "Ratio_Actual";
                dgvNRJ.Columns[14].Name = "GoodsIssued_SeqNo";
                dgvNRJ.Columns[15].Name = "InventSiteId";
                dgvNRJ.Columns[16].Name = "Notes";
                dgvNRJ.Columns[17].Name = "OldQty";

                DataGridViewComboBoxColumn JenisRetur = new DataGridViewComboBoxColumn();
                JenisRetur.Name = "JenisRetur";
                JenisRetur.HeaderText = "      Jenis Retur       ";
                JenisRetur.Items.Add("Retur Tukar Barang");// code: 01
                JenisRetur.Items.Add("Retur Debet Note");// code: 02
                JenisRetur.Items.Add("Retur Kembalikan Barang");// code: 03
                this.dgvNRJ.Columns.Add(JenisRetur);
            }
        }

        public void ModeNew()
        {
            NRJNumber = "";
            dtNRJ.Value = DateTime.Now;
            txtNotes.Enabled = true;
            cmbJenisRetur.Enabled = true;
            dgvNRJ.ReadOnly = false;

            btnSearchGI.Enabled = true;
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
            dgvNRJ.ReadOnly = false;

            btnSearchGI.Enabled = true;
            btnSearchWH.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            dgvNRJ.ReadOnly = false;
            dgvNRJ.Columns["No"].ReadOnly = true;
            dgvNRJ.Columns["FullItemID"].ReadOnly = true;
            dgvNRJ.Columns["ItemName"].ReadOnly = true;
            dgvNRJ.Columns["RemainingQty"].ReadOnly = true;
            dgvNRJ.Columns["UoM_Qty"].ReadOnly = false;
            dgvNRJ.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Ratio"].ReadOnly = true;
            dgvNRJ.Columns["Ratio_Actual"].ReadOnly = true;
            dgvNRJ.Columns["InventSiteID"].ReadOnly = true;
            dgvNRJ.Columns["Notes"].ReadOnly = true;
            dgvNRJ.Columns["JenisRetur"].ReadOnly = false;

            dgvNRJ.DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRJ.Columns["UoM_Qty"].DefaultCellStyle.BackColor = Color.White;
            dgvNRJ.Columns["JenisRetur"].DefaultCellStyle.BackColor = Color.White;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            GetDataHeader();

            txtNotes.Enabled = false;
            cmbJenisRetur.Enabled = false;
            dgvNRJ.ReadOnly = true;

            btnSearchGI.Enabled = false;
            btnSearchWH.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            //tia edit
            txtCustName.Enabled = true;
            txtCustID.Enabled = true;
            txtSONum.Enabled = true;
            txtGINum.Enabled = true;

            txtCustName.ReadOnly = true;
            txtCustID.ReadOnly = true;
            txtSONum.ReadOnly = true;
            txtGINum.ReadOnly = true;
            //tia edit end

            dgvNRJ.ReadOnly = true;
            dgvNRJ.Columns["No"].ReadOnly = true;
            dgvNRJ.Columns["FullItemID"].ReadOnly = true;
            dgvNRJ.Columns["ItemName"].ReadOnly = true;
            dgvNRJ.Columns["RemainingQty"].ReadOnly = true;
            dgvNRJ.Columns["UoM_Qty"].ReadOnly = true;
            dgvNRJ.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Ratio"].ReadOnly = true;
            dgvNRJ.Columns["Ratio_Actual"].ReadOnly = true;
            dgvNRJ.Columns["InventSiteID"].ReadOnly = true;
            dgvNRJ.Columns["Notes"].ReadOnly = true;
            dgvNRJ.Columns["JenisRetur"].ReadOnly = true;

            dgvNRJ.DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRJ.Columns["UoM_Qty"].DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRJ.Columns["JenisRetur"].DefaultCellStyle.BackColor = Color.LightGray;
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
            dgvNRJ.ReadOnly = true;

            btnSearchGI.Enabled = false;
            btnSearchWH.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            //tia edit
            txtCustName.Enabled = true;
            txtCustID.Enabled = true;
            txtSONum.Enabled = true;
            txtGINum.Enabled = true;

            txtCustName.ReadOnly = true;
            txtCustID.ReadOnly = true;
            txtSONum.ReadOnly = true;
            txtGINum.ReadOnly = true;

            txtCustID.ContextMenu = vendid;
            txtCustName.ContextMenu = vendid;
            txtSONum.ContextMenu = vendid;
            txtGINum.ContextMenu = vendid;
            //tia edit end

            dgvNRJ.ReadOnly = true;
            dgvNRJ.Columns["No"].ReadOnly = true;
            dgvNRJ.Columns["FullItemID"].ReadOnly = true;
            dgvNRJ.Columns["ItemName"].ReadOnly = true;
            dgvNRJ.Columns["RemainingQty"].ReadOnly = true;
            dgvNRJ.Columns["UoM_Qty"].ReadOnly = true;
            dgvNRJ.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Ratio"].ReadOnly = true;
            dgvNRJ.Columns["Ratio_Actual"].ReadOnly = true;
            dgvNRJ.Columns["InventSiteID"].ReadOnly = true;
            dgvNRJ.Columns["Notes"].ReadOnly = true;
            dgvNRJ.Columns["JenisRetur"].ReadOnly = true;

            dgvNRJ.DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRJ.Columns["UoM_Qty"].DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRJ.Columns["JenisRetur"].DefaultCellStyle.BackColor = Color.LightGray;
        }
        //tia edit end

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                if (txtStatusName.Text.Contains("Approve") || txtStatusName.Text.Contains("Reject"))
                {
                    MessageBox.Show("Nota Retur Jual sudah diapprove/reject.");
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
            ModeBeforeEdit();
            GetDataHeader();
        }

        public void GetDataHeader()
        {
            if (NRJNumber != "")
            {
                dgvNRJ.Rows.Clear();
                Conn = ConnectionString.GetConnection();
                Query = "SELECT TOP 1 NRJH.NRJId, NRJH.NRJDate, NRJH.GoodsIssuedId, NRJH.GoodsIssuedDate, NRJH.SalesId, NRJH.CustId, NRJH.CustName, NRJH.SiteId, INS.InventSiteName, INS.Lokasi, NRJH.Notes, TST.Deskripsi, NRJH.ApprovedBy, ActionCode ";
                Query += "FROM NotaReturJualH NRJH LEFT JOIN InventSite INS ON NRJH.SiteId = INS.InventSiteID LEFT JOIN TransStatusTable TST ON NRJH.TransStatusId = TST.StatusCode AND TST.TransCode = 'NotaReturJual' ";
                Query += "WHERE NRJId = '" + NRJNumber + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtNRJNum.Text = NRJNumber;
                    dtNRJ.Text = Dr["NRJDate"].ToString();
                    txtGINum.Text = Dr["GoodsIssuedId"].ToString();
                    dtGI.Text = Dr["GoodsIssuedDate"].ToString();
                    txtSONum.Text = Dr["SalesId"].ToString();
                    txtCustID.Text = Dr["CustId"].ToString();
                    txtCustName.Text = Dr["CustName"].ToString();
                    txtSiteID.Text = Dr["SiteId"].ToString();
                    txtSiteName.Text = Dr["InventSiteName"].ToString();
                    txtSiteLocation.Text = Dr["Lokasi"].ToString();
                    txtNotes.Text = Dr["Notes"].ToString();
                    txtStatusName.Text = Dr["Deskripsi"].ToString();
                    txtApproved.Text = Dr["ApprovedBy"].ToString();
                    if (Dr["ActionCode"].ToString() == "01")
                    {
                        cmbJenisRetur.SelectedIndex = 1;
                    }
                    else if (Dr["ActionCode"].ToString() == "02")
                    {
                        cmbJenisRetur.SelectedIndex = 2;
                    }
                    else if (Dr["ActionCode"].ToString() == "03")
                    {
                        cmbJenisRetur.SelectedIndex = 3;
                    }
                }
                Dr.Close();

                CreateGrid();

                Query = "SELECT NRJId, SeqNo, GroupId, SubGroupId, SubGroup2Id, ItemId, FullItemId, ItemName, RemainingQty, ";
                Query += "GoodsIssuedId, GoodsIssued_SeqNo, UoM_Qty, Alt_Qty, UoM_Unit, Alt_Unit, Ratio, NRJD.InventSiteId, ";
                Query += "ISB.InventSiteBlokID, ActionCode, Notes ";
                Query += "FROM NotaReturJual_Dtl NRJD LEFT JOIN InventSiteBlok ISB ON ISB.InventSiteID = NRJD.InventSiteId ";
                Query += "WHERE NRJId = '" + NRJNumber + "' ORDER BY SeqNo ASC ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    Query = "SELECT Remaining_Qty FROM GoodsIssuedD WHERE GoodsIssuedId = '" + txtGINum.Text + "' AND FullItemID = '" + Dr["FullItemId"] + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    string Remaining_Qty = Cmd.ExecuteScalar().ToString();

                    Query = "SELECT Ratio_Actual FROM GoodsIssuedD WHERE GoodsIssuedId = '" + txtGINum.Text + "' AND FullItemID = '" + Dr["FullItemId"] + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    string Ratio_Actual = Cmd.ExecuteScalar().ToString();

                    this.dgvNRJ.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroupId"], Dr["SubGroup2Id"], decimal.Parse(Remaining_Qty) + decimal.Parse(Dr["UoM_Qty"].ToString()), Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], decimal.Parse(Ratio_Actual), Dr["GoodsIssued_SeqNo"], Dr["InventSiteId"], Dr["Notes"], Dr["UoM_Qty"]);
                    if (Dr["ActionCode"].ToString() == "01")
                    {
                        dgvNRJ.Rows[j].Cells["JenisRetur"].Value = "Retur Tukar Barang";
                    }
                    else if (Dr["ActionCode"].ToString() == "02")
                    {
                        dgvNRJ.Rows[j].Cells["JenisRetur"].Value = "Retur Debet Note";
                    }
                    else if (Dr["ActionCode"].ToString() == "03")
                    {
                        dgvNRJ.Rows[j].Cells["JenisRetur"].Value = "Retur Kembalikan Barang";
                    }
                    j++;
                }
                Dr.Close();

                dgvNRJ.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Ratio_Actual"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["InventSiteID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvNRJ.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRJ.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRJ.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRJ.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRJ.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvNRJ.Columns["ItemID"].Visible = false;
                dgvNRJ.Columns["GroupId"].Visible = false;
                dgvNRJ.Columns["SubGroup1ID"].Visible = false;
                dgvNRJ.Columns["SubGroup2ID"].Visible = false;
                dgvNRJ.Columns["Alt_Unit"].Visible = false;
                dgvNRJ.Columns["Ratio_Actual"].Visible = false;
                dgvNRJ.Columns["GoodsIssued_SeqNo"].Visible = false;
                dgvNRJ.Columns["OldQty"].Visible = false;
                dgvNRJ.Columns["JenisRetur"].Visible = false;

                dgvNRJ.AutoResizeColumns();

                Conn.Close();
            }
        }

        AddGI GI = null;
        private void btnSearchGI_Click(object sender, EventArgs e)
        {
            if (GI == null || GI.Text == "")
            {
                GI = new AddGI();
                GI.setParent(this);
                GI.Show();
            }
            else if (CheckOpened(GI.Name))
            {
                GI.WindowState = FormWindowState.Normal;
                GI.Show();
                GI.Focus();
            }
        }

        public void AddGI(string ginumber)
        {
            //asdf
            txtGINum.Text = ginumber;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT A.GoodsIssuedDate, A.SalesOrderNo, A.AccountNum, A.AccountName, A.InventSiteID, A.InventSiteName, A.Lokasi ";
            Query += "FROM (SELECT DISTINCT GIH.GoodsIssuedId, DOD.DeliveryOrderId, SOD.SalesOrderNo, GIH.GoodsIssuedDate, GIH.AccountNum, ";
            Query += "GIH.AccountName, GIH.InventSiteID, IST.InventSiteName, IST.Lokasi FROM GoodsIssuedH GIH LEFT JOIN GoodsIssuedD GID ON GIH.GoodsIssuedId = GID.GoodsIssuedId ";
            Query += "LEFT JOIN DeliveryOrderD DOD ON GIH.RefTransID = DOD.DeliveryOrderId LEFT JOIN SalesOrderD SOD ON DOD.SalesOrderId = SOD.SalesOrderNo ";
            Query += "LEFT JOIN InventSite IST ON IST.InventSiteID = GID.InventSiteId WHERE GIH.StatusCode = '03' AND GID.ActionCode = '01') A WHERE A.GoodsIssuedId = '" + ginumber + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                txtSONum.Text = Dr["SalesOrderNo"].ToString();
                txtCustID.Text = Dr["AccountNum"].ToString();
                txtCustName.Text = Dr["AccountName"].ToString();
                dtGI.Text = Dr["GoodsIssuedDate"].ToString();
                txtSiteID.Text = Dr["InventSiteID"].ToString();
                txtSiteName.Text = Dr["InventSiteName"].ToString();
                txtSiteLocation.Text = Dr["Lokasi"].ToString();
            }
            Dr.Close();
            dgvNRJ.Rows.Clear();
        }

        AddWH WH = null;
        private void btnSearchWH_Click(object sender, EventArgs e)
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

        public void AddWH(string WHCode)
        {
            txtSiteID.Text = WHCode;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT InventSiteName, Lokasi FROM InventSite WHERE InventSiteID = '" + WHCode + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
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
                //Query = "SELECT * FROM GoodsIssuedD a INNER JOIN InventTable b ON a.FullItemID = b.FullItemID WHERE a.GoodsIssuedId = '" + txtGINum.Text + "' AND a.GoodsIssuedSeqNo = '" + SeqNo[i] + "'";
                Query = "SELECT GID.ItemId, GID.FullItemID, GID.ItemName, GID.GroupId, GID.SubGroup1Id, GID.SubGroup2Id, IT.UoM, IT.UoMAlt, GID.Ratio, GID.Ratio_Actual, GID.GoodsIssuedSeqNo, GID.InventSiteID, GID.Notes, GID.Qty_Actual ";
                Query += "FROM GoodsIssuedD GID INNER JOIN InventTable IT ON GID.FullItemID = IT.FullItemID WHERE GID.GoodsIssuedId = '" + txtGINum.Text + "' AND GID.GoodsIssuedSeqNo = '" + SeqNo[i] + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    Query = "SELECT ISNULL(Remaining_Qty,0) AS Remaining_Qty FROM GoodsIssuedD WHERE GoodsIssuedId = '" + txtGINum.Text + "' AND GoodsIssuedSeqNo = '" + Dr["GoodsIssuedSeqNo"] + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);

                    this.dgvNRJ.Rows.Add(dgvNRJ.RowCount + 1, Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Cmd.ExecuteScalar(), "", Dr["UoM"], "", Dr["UoMAlt"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsIssuedSeqNo"], Dr["InventSiteID"], Dr["Notes"], Dr["Qty_Actual"]);//Dr["Qty_Actual"]--UoM_Qty, Dr["TotalBerat_Actual"], Dr["Quality"]
                }
                Dr.Close();
            }

            dgvNRJ.ReadOnly = false;
            //dgvNRB.DefaultCellStyle.BackColor = Color.White;
            dgvNRJ.Columns["No"].ReadOnly = true;
            dgvNRJ.Columns["FullItemID"].ReadOnly = true;
            dgvNRJ.Columns["ItemName"].ReadOnly = true;
            dgvNRJ.Columns["RemainingQty"].ReadOnly = true;
            dgvNRJ.Columns["UoM_Qty"].ReadOnly = false;
            dgvNRJ.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Ratio"].ReadOnly = true;
            dgvNRJ.Columns["Ratio_Actual"].ReadOnly = true;
            dgvNRJ.Columns["InventSiteID"].ReadOnly = true;
            dgvNRJ.Columns["Notes"].ReadOnly = true;

            dgvNRJ.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Ratio_Actual"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["InventSiteID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvNRJ.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRJ.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRJ.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRJ.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRJ.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvNRJ.Columns["ItemID"].Visible = false;
            dgvNRJ.Columns["GroupId"].Visible = false;
            dgvNRJ.Columns["SubGroup1ID"].Visible = false;
            dgvNRJ.Columns["SubGroup2ID"].Visible = false;
            dgvNRJ.Columns["Alt_Unit"].Visible = false;
            dgvNRJ.Columns["Ratio_Actual"].Visible = false;
            dgvNRJ.Columns["GoodsIssued_SeqNo"].Visible = false;
            dgvNRJ.Columns["OldQty"].Visible = false;
            dgvNRJ.Columns["JenisRetur"].Visible = false;

            dgvNRJ.AutoResizeColumns();

            dgvNRJ.DefaultCellStyle.BackColor = Color.LightGray;
            dgvNRJ.Columns["UoM_Qty"].DefaultCellStyle.BackColor = Color.White;
            dgvNRJ.Columns["JenisRetur"].DefaultCellStyle.BackColor = Color.White;
        }

        AddItem AI = null;
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (txtGINum.Text == "" || txtSiteID.Text == "")
            {
                MessageBox.Show("Pilih Good Issued (BBK) dan Warehouse terlebih dahulu.");
            }
            else
            {
                List<string> seqno = new List<string>();
                if (dgvNRJ.RowCount > 0)
                {
                    for (int i = 0; i < dgvNRJ.RowCount; i++)
                    {
                        seqno.Add(dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString());
                    }
                }

                if (AI == null || AI.Text == "")
                {
                    AI = new AddItem();
                    AI.setParent(this);
                    AI.setMode(txtGINum.Text, seqno);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvNRJ.RowCount > 0)
            {
                dgvNRJ.Rows.RemoveAt(dgvNRJ.CurrentRow.Index);

                for (int i = 0; i < dgvNRJ.RowCount; i++)
                {
                    dgvNRJ.Rows[i].Cells["No"].Value = i + 1;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name == "AddGI")
                    Application.OpenForms[i].Close();
                else if (Application.OpenForms[i].Name == "AddItem")
                    Application.OpenForms[i].Close();
                else if (Application.OpenForms[i].Name == "AddWH")
                    Application.OpenForms[i].Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbJenisRetur.SelectedIndex == 0)
            {
                MessageBox.Show("Jenis Retur harus dipilih.");
                return;
            }
            if (dgvNRJ.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }

            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            decimal QtyNew = 0;
            decimal amountPU = 0, amountOH = 0;

            try
            {
                if (Mode == "New" && txtNRJNum.Text.Trim() == "")
                {
                    for (int i = 0; i < dgvNRJ.RowCount; i++)
                    {
                        if (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" || decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()) == 0)
                        {
                            MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", Jumlah quantity belum diisi atau 0.");
                            Conn.Close();
                            Trans.Dispose();
                            return;
                        }
                        if (decimal.Parse(dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) < decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", Jumlah quantity melebihi batas.");
                            Conn.Close();
                            Trans.Dispose();
                            return;
                        }
                        if (dgvNRJ.Rows[i].Cells["Alt_Qty"].Value.ToString() == "")
                        {
                            MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", silahkan dimasukkan kembali Jumlah quantitynya lalu tekan enter.");
                            Conn.Close();
                            Trans.Dispose();
                            return;
                        }

                        string Action = dgvNRJ.Rows[i].Cells["JenisRetur"].Value == null ? "" : dgvNRJ.Rows[i].Cells["JenisRetur"].Value.ToString();
                        if (Action == "")
                        {
                            //MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", Jenis Retur belum Dipilih.");
                            //return;
                        }

                        string getFullItemID = dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString();

                        Query = "";
                        //Get Price With GIid and FullItemId
                        Query = "SELECT ISNULL(SOD.Price, 0) ";
                        Query += "FROM GoodsIssuedD GID ";
                        Query += "LEFT JOIN DeliveryOrderD DOD ON DOD.DeliveryOrderId=GID.RefTransID AND DOD.SeqNo=GID.RefTransSeqNo ";
                        Query += "LEFT JOIN SalesOrderD SOD ON SOD.SalesOrderNo=DOD.SalesOrderId AND DOD.SalesOrderSeqNo=SOD.SeqNo ";
                        Query += "WHERE GID.GoodsIssuedId = '" + txtGINum.Text + "' AND GID.GoodsIssuedSeqNo = '" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        //Cek Price Ada dalam list atau tidak
                        if (Cmd.ExecuteScalar() == null)
                        {
                            MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                            return;
                        }
                    }

                    string Jenis = "NRJ", Kode = "NRJ";
                    NRJNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    #region INSERT QUERY NotaReturJualH
                    Query = "INSERT INTO NotaReturJualH (NRJId, NRJDate, NRJMode, CustId, CustName, GoodsIssuedId, GoodsIssuedDate, SalesId, SiteId, SiteName, TransStatusId, ActionCode, Notes, CreatedDate, CreatedBy) VALUES  ";
                    Query += "('" + NRJNumber + "', ";
                    Query += "'" + dtNRJ.Value.ToString("yyyy-MM-dd") + "', ";
                    Query += "'MANUAL', ";
                    Query += "'" + txtCustID.Text + "', ";
                    Query += "'" + txtCustName.Text + "', ";
                    Query += "'" + txtGINum.Text + "', ";
                    Query += "'" + dtGI.Value.ToString("yyyy-MM-dd") + "', ";
                    Query += "'" + txtSONum.Text + "', ";
                    Query += "'" + txtSiteID.Text + "', ";
                    Query += "'" + txtSiteName.Text + "', ";
                    Query += "'01', ";
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
                    Query += "@Notes, ";
                    Query += "GETDATE(), '" + ControlMgr.UserId + "');";
                    #endregion

                    ListMethod.StatusLogCustomer("NRJHeader", "NotaReturJual", txtCustID.Text, "01", "", NRJNumber, "", "", "");

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@Notes", txtNotes.Text);
                    Cmd.ExecuteNonQuery();

                    Query = "";
                    for (int i = 0; i < dgvNRJ.RowCount; i++)
                    {
                        #region INSERT QUERY NotaReturJual_Dtl
                        Query = "INSERT INTO NotaReturJual_Dtl(";
                        Query += "NRJId, SeqNo, GroupId, SubGroupId, SubGroup2Id, ItemId, FullItemId, ItemName, GoodsIssuedId, GoodsIssued_SeqNo, ";
                        Query += "UoM_Qty, UoM_Unit, Alt_Qty, Alt_Unit, Ratio, RemainingQty, InventSiteId, ActionCode, Notes, ";
                        //if (cmbJenisRetur.SelectedIndex == 1)
                        //{
                        //    Query += "Remaining_Qty_DO, ";
                        //}
                        Query += "CreatedBy) VALUES ('";
                        Query += NRJNumber + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["No"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["No"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["GroupId"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["SubGroup1ID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["SubGroup1ID"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["SubGroup2ID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["SubGroup2ID"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["ItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["ItemID"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["ItemName"].Value.ToString()) + "', '";
                        Query += txtGINum.Text + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["UoM_Unit"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["UoM_Unit"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["Alt_Qty"].Value == null ? "0" : dgvNRJ.Rows[i].Cells["Alt_Qty"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["Alt_Unit"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["Alt_Unit"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["Ratio"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["Ratio"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["RemainingQty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) + "', '";
                        Query += txtSiteID.Text + "', '";

                        string Action = dgvNRJ.Rows[i].Cells["JenisRetur"].Value == null ? "" : dgvNRJ.Rows[i].Cells["JenisRetur"].Value.ToString();
                        if (Action == "Retur Tukar Barang")
                        {
                            Query += "01', '";
                        }
                        else if (Action == "Retur Debet Note")
                        {
                            Query += "02', '";
                        }
                        else if (Action == "Retur Kembalikan Barang")
                        {
                            Query += "03', '";
                        }
                        else
                        {
                            if (cmbJenisRetur.Text == "Retur Tukar Barang")
                            {
                                Query += "01', '";
                            }
                            else if (cmbJenisRetur.Text == "Retur Debet Note")
                            {
                                Query += "02', '";
                            }
                            else if (cmbJenisRetur.Text == "Retur Kembalikan Barang")
                            {
                                Query += "03', '";
                            }
                        }

                        Query += (dgvNRJ.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["Notes"].Value.ToString()) + "', '";
                        //if (cmbJenisRetur.SelectedIndex == 1)
                        //{
                        //    Query += (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "', '";
                        //}
                        Query += ControlMgr.UserId + "')";
                        #endregion
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) - decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString());
                        Query = "UPDATE GoodsIssuedD SET Remaining_Qty = '" + QtyNew + "' ";
                        Query += "WHERE GoodsIssuedId = '" + txtGINum.Text.Trim() + "' AND GoodsIssuedSeqNo='" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        string getFullItemID = dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString();

                        Query = "";
                        //Get Price With GRid and FullItemId
                        Query = "SELECT ISNULL(SOD.Price, 0) ";
                        Query += "FROM GoodsIssuedD GID ";
                        Query += "LEFT JOIN DeliveryOrderD DOD ON DOD.DeliveryOrderId=GID.RefTransID AND DOD.SeqNo=GID.RefTransSeqNo ";
                        Query += "LEFT JOIN SalesOrderD SOD ON SOD.SalesOrderNo=DOD.SalesOrderId AND DOD.SalesOrderSeqNo=SOD.SeqNo ";
                        Query += "WHERE GID.GoodsIssuedId = '" + txtGINum.Text + "' AND GID.GoodsIssuedSeqNo = '" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        string getUoM_Qty = dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString();
                        string getAlt_Qty = dgvNRJ.Rows[i].Cells["Alt_Qty"].Value.ToString();
                        string Price = Cmd.ExecuteScalar().ToString();
                        amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                        amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                        Query = "";
                        //Get Retur_Jual_Created_UoM, Retur_Jual_Created_Alt (Old)
                        Query = "SELECT Retur_Jual_Created_UoM FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RJC_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Jual_Created_Alt FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RJC_Alt_Old = Cmd.ExecuteScalar().ToString();
                        //Query = "SELECT Retur_Jual_Created_Amount FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        //Cmd = new SqlCommand(Query, Conn, Trans);
                        //string RJC_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RJC_UoM_New = decimal.Parse(RJC_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                        decimal RJC_Alt_New = decimal.Parse(RJC_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                        //decimal RJC_Amount_New = decimal.Parse(amountPU.ToString());
                        Query = "";
                        //Update Invent_Sales_Qty
                        Query = "UPDATE Invent_Sales_Qty SET ";
                        Query += "Retur_Jual_Created_UoM = '" + RJC_UoM_New + "', ";
                        Query += "Retur_Jual_Created_Alt = '" + RJC_Alt_New + "' ";
                        Query += "WHERE FullItemId = '" + getFullItemID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Insert to NotaReturJual_LogTable
                        #region INSERT QUERY NotaReturJual_LogTable
                        Query = "INSERT INTO NotaReturJual_LogTable ";
                        Query += "(NRJDate, NRJId, GoodsIssuedDate, GoodsIssuedId, ";
                        Query += "CustId, SiteId, FullItemId, SeqNo, ";
                        Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                        Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                        Query += "('" + dtNRJ.Value.ToString("yyyy-MM-dd") + "', '" + NRJNumber + "', '" + dtGI.Value.ToString("yyyy-MM-dd") + "', '" + txtGINum.Text + "', ";
                        Query += "'" + txtCustID.Text + "', '" + txtSiteID.Text + "', '" + getFullItemID + "', '" + dgvNRJ.Rows[i].Cells["No"].Value.ToString() + "', ";
                        Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amountPU + "', '01', ";
                        Query += "'Waiting for Approval', 'Status: 01. Waiting for Approval', '" + ControlMgr.UserId + "', GETDATE()) ";
                        #endregion
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        if (cmbJenisRetur.SelectedIndex == 1)
                        {
                            //REMARKED BY: HC (S) 29.06.18
                            /*//Update Qty_Return to SO_Dtl
                            Query = "UPDATE SalesOrderD SET Qty_Return = '" + dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString() + "' ";
                            Query += "WHERE SalesOrderNo = '" + txtSONum.Text + "' AND FullItemID = '" + dgvNRJ.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString() + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();*/
                            //REMARKED BY: HC (E) 

                            //BY: HC (S) 29.06.18 UPDATE SO REMAINING QTY WHEN RETUR
                            Query = "select SOD.SalesOrderNo, SOD.SeqNo, SOD.RemainingQty, SOD.Qty_Return from GoodsIssuedD GID left join DeliveryOrderD DOD on GID.RefTransID = DOD.DeliveryOrderId and GID.RefTransSeqNo = DOD.SeqNo left join SalesOrderD SOD on SOD.SalesOrderNo = DOD.SalesOrderId and SOD.SeqNo = DOD.SalesOrderSeqNo where GID.GoodsIssuedId = '" + txtGINum.Text + "' and GID.GoodsIssuedSeqNo = '" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                decimal qty_return = Dr["Qty_Return"] == (object)DBNull.Value ? 0 : Convert.ToDecimal(Dr["Qty_Return"]);
                                decimal a = qty_return + Convert.ToDecimal(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value);
                                decimal b = Convert.ToDecimal(Dr["RemainingQty"]) + Convert.ToDecimal(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value);
                                Query = "update SalesOrderD set Qty_Return = '" + a + "', RemainingQty = '" + b + "' where SalesOrderNo = '" + Dr["SalesOrderNo"] + "' and SeqNo = '" + Dr["SeqNo"] + "'";
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn, Trans);
                                Cmd2.ExecuteNonQuery();
                            }
                            Dr.Close();

                            Query = "select count(*) from SalesOrderD where SalesOrderNo = '" + txtSONum.Text + "' and Deleted = 'N' and RemainingQty > 0";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            if (Convert.ToInt32(Cmd.ExecuteScalar()) != 0)
                            {
                                Query = "update SalesOrderH set TransStatus = '06', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + txtSONum.Text + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }
                            //BY: HC (E) 29.06.18
                        }

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
                    MessageBox.Show("Data NRJNumber : " + NRJNumber + " berhasil ditambahkan.");
                    txtNRJNum.Text = NRJNumber;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
                else
                {
                    DateTime CreatedDate = DateTime.Now;
                    String CreatedBy = "";

                    for (int i = 0; i < dgvNRJ.RowCount; i++)
                    {
                        if (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" || decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()) == 0)
                        {
                            MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", Jumlah quantity belum diisi atau 0.");
                            Conn.Close();
                            Trans.Dispose();
                            return;
                        }
                        if (decimal.Parse(dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) < decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", Jumlah quantity melebihi batas.");
                            Conn.Close();
                            Trans.Dispose();
                            return;
                        }
                        if (dgvNRJ.Rows[i].Cells["Alt_Qty"].Value.ToString() == "")
                        {
                            MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", silahkan dimasukkan kembali Jumlah quantitynya lalu tekan enter.");
                            Conn.Close();
                            Trans.Dispose();
                            return;
                        }

                        string Action = dgvNRJ.Rows[i].Cells["JenisRetur"].Value == null ? "" : dgvNRJ.Rows[i].Cells["JenisRetur"].Value.ToString();
                        if (Action == "")
                        {
                            //MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", Jenis Retur belum Dipilih.");
                            //return;
                        }

                        string getFullItemID = dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString();

                        Query = "";
                        //Get Price With GIid and FullItemId
                        Query = "SELECT ISNULL(SOD.Price, 0) ";
                        Query += "FROM GoodsIssuedD GID ";
                        Query += "LEFT JOIN DeliveryOrderD DOD ON DOD.DeliveryOrderId=GID.RefTransID AND DOD.SeqNo=GID.RefTransSeqNo ";
                        Query += "LEFT JOIN SalesOrderD SOD ON SOD.SalesOrderNo=DOD.SalesOrderId AND DOD.SalesOrderSeqNo=SOD.SeqNo ";
                        Query += "WHERE GID.GoodsIssuedId = '" + txtGINum.Text + "' AND GID.GoodsIssuedSeqNo = '" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        //Cek Price Ada dalam list atau tidak
                        if (Cmd.ExecuteScalar() == null)
                        {
                            MessageBox.Show("Item No. " + dgvNRJ.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                            return;
                        }
                    }

                    //Select SiteId Lama Untuk Invent Onhand
                    Query = "SELECT SiteId FROM NotaReturJualH WHERE NRJId = '" + txtNRJNum.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string SiteId_Old = Cmd.ExecuteScalar().ToString();

                    //Select SOnum Lama Untuk Remaining_Qty_DO
                    Query = "SELECT SalesId FROM NotaReturJualH WHERE NRJId = '" + txtNRJNum.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string SONum_Old = Cmd.ExecuteScalar().ToString();

                    //Select Jenis retur Lama
                    Query = "SELECT ActionCode FROM NotaReturJualH WHERE NRJId = '" + txtNRJNum.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string JenisRetur_Old = Cmd.ExecuteScalar().ToString();

                    Query = "UPDATE NotaReturJualH SET ";
                    Query += "CustId = '" + txtCustID.Text + "', ";
                    Query += "CustName = '" + txtCustName.Text + "', ";
                    Query += "GoodsIssuedId = '" + txtGINum.Text + "', ";
                    Query += "GoodsIssuedDate = '" + dtGI.Value.ToString("yyyy-MM-dd") + "', ";
                    Query += "SalesId = '" + txtSONum.Text + "', ";
                    Query += "SiteId = '" + txtSiteID.Text + "', ";
                    Query += "SiteName = '" + txtSiteName.Text + "', ";
                    Query += "TransStatusId = '01', ";
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
                    Query += "Notes = @Notes, ";
                    Query += "UpdatedDate = GETDATE(), ";
                    Query += "UpdatedBy = '" + ControlMgr.UserId + "', ";
                    Query += "ApprovedBy = '' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where NRJId='" + txtNRJNum.Text.Trim() + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@Notes", txtNotes.Text);
                    Cmd.ExecuteNonQuery();

                    ListMethod.StatusLogCustomer("NRJHeader", "NotaReturJual", txtCustID.Text, "01", "Edit", NRJNumber, "", "", "");

                    Query = "";

                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                        CreatedBy = Dr["CreatedBy"].ToString();
                    }
                    Dr.Close();

                    //Update Qty
                    List<string> GoodsIssued_SeqNo = new List<string>();
                    List<string> GoodsIssuedId = new List<string>();
                    List<string> FullItemID = new List<string>();
                    List<decimal> UoM_Qty = new List<decimal>();
                    List<decimal> Alt_Qty = new List<decimal>();
                    decimal RemainingQty, QtyNew2 = 0;
                    Query = "SELECT GoodsIssued_SeqNo, GoodsIssuedId, FullItemId, UoM_Qty, Alt_Qty FROM NotaReturJual_Dtl WHERE NRJId='" + txtNRJNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        GoodsIssued_SeqNo.Add(Dr["GoodsIssued_SeqNo"].ToString());
                        GoodsIssuedId.Add(Dr["GoodsIssuedId"].ToString());
                        FullItemID.Add(Dr["FullItemId"].ToString());
                        UoM_Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                        Alt_Qty.Add(decimal.Parse(Dr["Alt_Qty"].ToString()));

                    }
                    Dr.Close();

                    for (int i = 0; i < GoodsIssued_SeqNo.Count; i++)
                    {
                        Query = "SELECT Remaining_Qty FROM GoodsIssuedD WHERE GoodsIssuedId = '" + GoodsIssuedId[i] + "' AND GoodsIssuedSeqNo = '" + GoodsIssued_SeqNo[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                        QtyNew2 = RemainingQty + UoM_Qty[i];

                        Query = "UPDATE GoodsIssuedD SET ";
                        Query += "Remaining_Qty = '" + QtyNew2 + "' ";
                        Query += "WHERE GoodsIssuedId = '" + GoodsIssuedId[i] + "' AND GoodsIssuedSeqNo='" + GoodsIssued_SeqNo[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Get Price With GRid and FullItemId
                        Query = "SELECT ISNULL(SOD.Price, 0) ";
                        Query += "FROM GoodsIssuedD GID ";
                        Query += "LEFT JOIN DeliveryOrderD DOD ON DOD.DeliveryOrderId=GID.RefTransID AND DOD.SeqNo=GID.RefTransSeqNo ";
                        Query += "LEFT JOIN SalesOrderD SOD ON SOD.SalesOrderNo=DOD.SalesOrderId AND DOD.SalesOrderSeqNo=SOD.SeqNo ";
                        Query += "WHERE GID.GoodsIssuedId = '" + txtGINum.Text + "' AND GID.GoodsIssuedSeqNo = '" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        string getUoM_Qty = UoM_Qty[i].ToString();
                        string getAlt_Qty = Alt_Qty[i].ToString();
                        string Price = Cmd.ExecuteScalar().ToString();
                        amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                        amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                        Query = "";
                        //Get Retur_Jual_Created_UoM, Retur_Jual_Created_Alt (Old)
                        Query = "SELECT Retur_Jual_Created_UoM FROM Invent_Sales_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RJC_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Jual_Created_Alt FROM Invent_Sales_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RJC_Alt_Old = Cmd.ExecuteScalar().ToString();
                        //Query = "SELECT Retur_Jual_Created_Amount FROM Invent_Sales_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                        //Cmd = new SqlCommand(Query, Conn, Trans);
                        //string RJC_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RJC_UoM_New = decimal.Parse(RJC_UoM_Old.ToString()) - decimal.Parse(getUoM_Qty.ToString());
                        decimal RJC_Alt_New = decimal.Parse(RJC_Alt_Old.ToString()) - decimal.Parse(getAlt_Qty.ToString());
                        //decimal RJC_Amount_New = decimal.Parse(amountPU.ToString());
                        Query = "";
                        //Update Invent_Sales_Qty
                        Query = "UPDATE Invent_Sales_Qty SET ";
                        Query += "Retur_Jual_Created_UoM = '" + RJC_UoM_New + "', ";
                        Query += "Retur_Jual_Created_Alt = '" + RJC_Alt_New + "' ";
                        Query += "WHERE FullItemId = '" + FullItemID[i] + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        if (JenisRetur_Old == "01")
                        {
                            //Query = "SELECT ISNULL(Qty_Return, 0) FROM SalesOrderD WHERE SalesOrderNo = '" + SONum_Old + "' AND FullItemID = '" + FullItemID[i] + "'";
                            //Cmd = new SqlCommand(Query, Conn, Trans);
                            //decimal Qty_Return_Old = decimal.Parse(Cmd.ExecuteScalar().ToString()) - UoM_Qty[i];

                            Query = "UPDATE SalesOrderD SET Qty_Return = Null ";
                            Query += "WHERE SalesOrderNo = '" + SONum_Old + "' AND FullItemID = '" + FullItemID[i] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }

                        Query = "";
                    }
                    //Delete to NotaReturJual_LogTable yg sudah d.buat sebelumnya
                    Query = "DELETE FROM NotaReturJual_LogTable WHERE NRJId = '" + txtNRJNum.Text + "' AND LogStatusCode = '01'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "DELETE FROM NotaReturJual_Dtl WHERE NRJId='" + txtNRJNum.Text.Trim() + "'";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    Query = "";

                    for (int i = 0; i < dgvNRJ.RowCount; i++)
                    {
                        Query = "INSERT INTO NotaReturJual_Dtl(";
                        Query += "NRJId, SeqNo, GroupId, SubGroupId, SubGroup2Id, ItemId, FullItemId, ItemName, GoodsIssuedId, GoodsIssued_SeqNo, ";
                        Query += "UoM_Qty, UoM_Unit, Alt_Qty, Alt_Unit, Ratio, RemainingQty, InventSiteId, ActionCode, Notes, ";
                        //if (cmbJenisRetur.SelectedIndex == 1)
                        //{
                        //    Query += "Remaining_Qty_DO, ";
                        //}
                        Query += "CreatedBy) VALUES ('";
                        Query += NRJNumber + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["No"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["No"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["GroupId"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["SubGroup1ID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["SubGroup1ID"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["SubGroup2ID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["SubGroup2ID"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["ItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["ItemID"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["ItemName"].Value.ToString()) + "', '";
                        Query += txtGINum.Text + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["UoM_Unit"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["UoM_Unit"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["Alt_Qty"].Value == null ? "0" : dgvNRJ.Rows[i].Cells["Alt_Qty"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["Alt_Unit"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["Alt_Unit"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["Ratio"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["Ratio"].Value.ToString()) + "', '";
                        Query += (dgvNRJ.Rows[i].Cells["RemainingQty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) + "', '";
                        Query += txtSiteID.Text + "', '";

                        string Action = dgvNRJ.Rows[i].Cells["JenisRetur"].Value == null ? "" : dgvNRJ.Rows[i].Cells["JenisRetur"].Value.ToString();
                        if (Action == "Retur Tukar Barang")
                        {
                            Query += "01', '";
                        }
                        else if (Action == "Retur Debet Note")
                        {
                            Query += "02', '";
                        }
                        else if (Action == "Retur Kembalikan Barang")
                        {
                            Query += "03', '";
                        }
                        else
                        {
                            if (cmbJenisRetur.Text == "Retur Tukar Barang")
                            {
                                Query += "01', '";
                            }
                            else if (cmbJenisRetur.Text == "Retur Debet Note")
                            {
                                Query += "02', '";
                            }
                            else if (Action == "Retur Kembalikan Barang")
                            {
                                Query += "03', '";
                            }
                        }

                        Query += (dgvNRJ.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["Notes"].Value.ToString()) + "', '";
                        //if (cmbJenisRetur.SelectedIndex == 1)
                        //{
                        //    Query += (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "', '";
                        //}
                        Query += ControlMgr.UserId + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) - decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString());
                        Query = "UPDATE GoodsIssuedD SET Remaining_Qty = '" + QtyNew + "' ";
                        Query += "WHERE GoodsIssuedId = '" + txtGINum.Text.Trim() + "' AND GoodsIssuedSeqNo='" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "' ";

                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        string getFullItemID = dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString();

                        Query = "";
                        //Get Price With GRid and FullItemId
                        Query = "SELECT ISNULL(SOD.Price, 0) ";
                        Query += "FROM GoodsIssuedD GID ";
                        Query += "LEFT JOIN DeliveryOrderD DOD ON DOD.DeliveryOrderId=GID.RefTransID AND DOD.SeqNo=GID.RefTransSeqNo ";
                        Query += "LEFT JOIN SalesOrderD SOD ON SOD.SalesOrderNo=DOD.SalesOrderId AND DOD.SalesOrderSeqNo=SOD.SeqNo ";
                        Query += "WHERE GID.GoodsIssuedId = '" + txtGINum.Text + "' AND GID.GoodsIssuedSeqNo = '" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);

                        string getUoM_Qty = dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString();
                        string getAlt_Qty = dgvNRJ.Rows[i].Cells["Alt_Qty"].Value.ToString();
                        string Price = Cmd.ExecuteScalar().ToString();
                        amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                        amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                        Query = "";
                        //Get Retur_Jual_Created_UoM, Retur_Jual_Created_Alt (Old)
                        Query = "SELECT Retur_Jual_Created_UoM FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RJC_UoM_Old = Cmd.ExecuteScalar().ToString();
                        Query = "SELECT Retur_Jual_Created_Alt FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string RJC_Alt_Old = Cmd.ExecuteScalar().ToString();
                        //Query = "SELECT Retur_Jual_Created_Amount FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                        //Cmd = new SqlCommand(Query, Conn, Trans);
                        //string RJC_Amount_Old = Cmd.ExecuteScalar().ToString();

                        decimal RJC_UoM_New = decimal.Parse(RJC_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                        decimal RJC_Alt_New = decimal.Parse(RJC_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                        //decimal RJC_Amount_New = decimal.Parse(amountPU.ToString());
                        Query = "";
                        //Update Invent_Sales_Qty
                        Query = "UPDATE Invent_Sales_Qty SET ";
                        Query += "Retur_Jual_Created_UoM = '" + RJC_UoM_New + "', ";
                        Query += "Retur_Jual_Created_Alt = '" + RJC_Alt_New + "' ";
                        Query += "WHERE FullItemId = '" + getFullItemID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        //Insert to NotaReturJual_LogTable
                        Query = "INSERT INTO NotaReturJual_LogTable ";
                        Query += "(NRJDate, NRJId, GoodsIssuedDate, GoodsIssuedId, ";
                        Query += "CustId, SiteId, FullItemId, SeqNo, ";
                        Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                        Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                        Query += "('" + dtNRJ.Value.ToString("yyyy-MM-dd") + "', '" + NRJNumber + "', '" + dtGI.Value.ToString("yyyy-MM-dd") + "', '" + txtGINum.Text + "', ";
                        Query += "'" + txtCustID.Text + "', '" + txtSiteID.Text + "', '" + getFullItemID + "', '" + dgvNRJ.Rows[i].Cells["No"].Value.ToString() + "', ";
                        Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amountPU + "', '01', ";
                        Query += "'Waiting for Approval', 'Status: 01. Waiting for Approval', '" + ControlMgr.UserId + "', GETDATE()) ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        if (cmbJenisRetur.SelectedIndex == 1)
                        {
                            //REMARKED BY: HC (S) 29.06.18
                            /*//Update Qty_Return to SO_Dtl
                            Query = "UPDATE SalesOrderD SET Qty_Return = '" + getUoM_Qty + "' ";
                            Query += "WHERE SalesOrderNo = '" + txtSONum.Text + "' AND FullItemID = '";
                            Query += dgvNRJ.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString() + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();*/
                            //REMARKED BY: HC (E) 29.06.18

                            //BY: HC (S) 29.06.18 UPDATE SO REMAINING QTY WHEN RETUR
                            Query = "select SOD.SalesOrderNo, SOD.SeqNo, SOD.RemainingQty, SOD.Qty_Return from GoodsIssuedD GID left join DeliveryOrderD DOD on GID.RefTransID = DOD.DeliveryOrderId and GID.RefTransSeqNo = DOD.SeqNo left join SalesOrderD SOD on SOD.SalesOrderNo = DOD.SalesOrderId and SOD.SeqNo = DOD.SalesOrderSeqNo where GID.GoodsIssuedId = '" + txtGINum.Text + "' and GID.GoodsIssuedSeqNo = '" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                decimal qty_return = Dr["Qty_Return"] == (object)DBNull.Value ? 0 : Convert.ToDecimal(Dr["Qty_Return"]);
                                decimal a = qty_return + Convert.ToDecimal(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value);
                                decimal b = Convert.ToDecimal(Dr["RemainingQty"]) + Convert.ToDecimal(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value);
                                Query = "update SalesOrderD set Qty_Return = '" + a + "', RemainingQty = '" + b + "' where SalesOrderNo = '" + Dr["SalesOrderNo"] + "' and SeqNo = '" + Dr["SeqNo"] + "'";
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn, Trans);
                                Cmd2.ExecuteNonQuery();
                            }
                            Dr.Close();

                            Query = "select count(*) from SalesOrderD where SalesOrderNo = '" + txtSONum.Text + "' and Deleted = 'N' and RemainingQty > 0";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            //Dr = Cmd.ExecuteReader();
                            if (Convert.ToInt32(Cmd.ExecuteScalar()) != 0)
                            {
                                Query = "update SalesOrderH set TransStatus = '06', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + txtSONum.Text + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }
                            //BY: HC (E) 29.06.18
                        }

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
                    MessageBox.Show("Data NRJNumber : " + txtNRJNum.Text + " berhasil diupdate.");
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

        private bool CekNRJKembalikanBarang(string NRJID, int SeqNo)
        {
            bool stat = false;
            Query = "SELECT [ActionCode] FROM [NotaReturJual_Dtl] WHERE [NRJId] = @NRJId AND [SeqNo] = @SeqNo";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@NRJId", NRJID);
                Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                if (Cmd.ExecuteScalar().ToString() == "03")
                {
                    stat = true;
                }
            }
            return stat;
        }

        private bool CekNRBKembalikanBarang(string NRBID, int SeqNo)
        {
            bool stat = false;
            Query = "SELECT [ActionCode] FROM [NotaReturBeli_Dtl] WHERE [NRBId] = @NRBId AND [SeqNo] = @SeqNo";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@NRBId", NRBID);
                Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                if (Cmd.ExecuteScalar().ToString() == "03")
                {
                    stat = true;
                }
            }
            return stat;
        }

        private void dgvNRJ_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvNRJ.RowCount > 0)
            {
                if (dgvNRJ.Columns[e.ColumnIndex].Name == "UoM_Qty")
                {
                    if (dgvNRJ.CurrentRow.Cells["UoM_Qty"].Value == null)
                    {
                        dgvNRJ.CurrentRow.Cells["Alt_Qty"].Value = "";
                    }
                    else
                    {
                        dgvNRJ.CurrentRow.Cells["Alt_Qty"].Value = decimal.Parse(dgvNRJ.CurrentRow.Cells["UoM_Qty"].Value.ToString()) * decimal.Parse(dgvNRJ.CurrentRow.Cells["Ratio"].Value.ToString());
                    }
                }
            }
        }

        private void dgvNRJ_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dgvNRJ.CurrentCell.ColumnIndex == dgvNRJ.Columns["UoM_Qty"].Index)
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
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
        //tia edit
        //klik kanan
        PopUp.CustomerID.Customer Cust = null;
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.InventSite InventsiteId = null;
        Sales.SalesOrder.SOHeader SOID = null;
        Sales.BBK.BBKHeader BBKId = null;

        Sales.NotaCredit.FrmT_NotaCredit ParentToNC;
        AccountsReceivable.ReceiptVoucher.HeaderReceiptVoucher ParentToNRV;

        public void ParentRefreshGrid(Sales.NotaCredit.FrmT_NotaCredit nc)
        {
            ParentToNC = nc;
        }

        public void ParentRefreshGrid2(AccountsReceivable.ReceiptVoucher.HeaderReceiptVoucher NRV)
        {
            ParentToNRV = NRV;
        }


        public static string itemID;

        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void txtCustID_MouseDown(object sender, MouseEventArgs e)
        {
            txtCustID.ContextMenu = vendid;
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    txtCustID.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(txtCustID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        private void txtCustName_MouseDown(object sender, MouseEventArgs e)
        {
            txtCustName.ContextMenu = vendid;
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    txtCustName.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(txtCustID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        private void dgvNRJ_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvNRJ.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvNRJ.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvNRJ.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvNRJ.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvNRJ.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
                if (InventsiteId == null || InventsiteId.Text == "")
                {
                    if (dgvNRJ.Columns[e.ColumnIndex].Name.ToString() == "InventSiteId")
                    {
                        InventsiteId = new PopUp.InventSite();
                        InventsiteId.GetData(dgvNRJ.Rows[e.RowIndex].Cells["InventSiteId"].Value.ToString(), dgvNRJ.Rows[e.RowIndex].Cells["ItemID"].Value.ToString());
                        InventsiteId.Show();
                    }
                }
                else if (CheckOpened(InventsiteId.Name))
                {
                    InventsiteId.WindowState = FormWindowState.Normal;
                    InventsiteId.GetData(dgvNRJ.Rows[e.RowIndex].Cells["InventSiteId"].Value.ToString(), dgvNRJ.Rows[e.RowIndex].Cells["ItemID"].Value.ToString());
                       
                    InventsiteId.Show();
                    InventsiteId.Focus();
                }

            }
        }

        private void txtSONum_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                txtSONum.ContextMenu = vendid;
                if (SOID == null || SOID.Text == "")
                {
                    txtSONum.Enabled = true;
                    SOID = new Sales.SalesOrder.SOHeader();
                    SOID.SetMode("PopUp", txtSONum.Text);
                    SOID.ParentRefreshGrid5(this);
                    SOID.Show();
                }
                else if (CheckOpened(SOID.Name))
                {
                    SOID.WindowState = FormWindowState.Normal;
                    SOID.Show();
                    SOID.Focus();
                }
            }
        }

        private void txtGINum_MouseDown(object sender, MouseEventArgs e)
        {
            txtGINum.ContextMenu = vendid;
            if (e.Button == MouseButtons.Right)
            {
                if (BBKId == null || BBKId.Text == "")
                {
                    txtGINum.Enabled = true;
                    BBKId = new Sales.BBK.BBKHeader();
                    BBKId.SetMode("PopUp", txtGINum.Text);
                    BBKId.ParentRefreshGrid(this);
                    BBKId.Show();
                }
                else if (CheckOpened(BBKId.Name))
                {
                    BBKId.WindowState = FormWindowState.Normal;
                    BBKId.Show();
                    BBKId.Focus();
                }
            }
        }
        //tia edit end
    }
}
