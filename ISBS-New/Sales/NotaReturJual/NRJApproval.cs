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
    public partial class NRJApproval : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Status, Query, crit, NRJNumber, GRID = null;
        Sales.NotaReturJual.InqNRJApproval Parent;
        //tia edit
        TaskList.Sales.NotaReturJual.TaskListNRJ Parent2;
        ContextMenu vendid = new ContextMenu();
        public void setParent2(TaskList.Sales.NotaReturJual.TaskListNRJ ParentToTLNRJ)
        {
            Parent2 = ParentToTLNRJ;
        }
        //tia edit end
        
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public NRJApproval()
        {
            InitializeComponent();
        }

        private void NRJApproval_Load(object sender, EventArgs e)
        {
            AddJenisRetur();
            ModeApprove();
            GetDataHeader();
        }

        public void setParent(Sales.NotaReturJual.InqNRJApproval f)
        {
            Parent = f;
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
                dgvNRJ.ColumnCount = 17;
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

                DataGridViewComboBoxColumn JenisRetur = new DataGridViewComboBoxColumn();
                JenisRetur.Name = "JenisRetur";
                JenisRetur.HeaderText = "      Jenis Retur       ";
                JenisRetur.Items.Add("Retur Tukar Barang");// code: 01
                JenisRetur.Items.Add("Retur Credit Nota");// code: 02
                this.dgvNRJ.Columns.Add(JenisRetur);
            }
        }

        private void AddJenisRetur()
        {
            cmbJenisRetur.Items.Add("---- Jenis Retur ----");
            cmbJenisRetur.Items.Add("Retur Tukar Barang");
            cmbJenisRetur.Items.Add("Retur Debet Note");
            cmbJenisRetur.SelectedIndex = 0;
        }

        public void ModeApprove()
        {
            Mode = "Approve";
            txtNotes.Enabled = false;
            cmbJenisRetur.Enabled = false;
            dgvNRJ.ReadOnly = true;
            dgvNRJ.DefaultCellStyle.BackColor = Color.LightGray;

            //tia edit
            txtCustID.Enabled = true;
            txtCustName.Enabled = true;
            txtSONum.Enabled = true;
            txtGINum.Enabled = true;

            txtCustID.ReadOnly = true;
            txtCustName.ReadOnly = true;
            txtSONum.ReadOnly = true;
            txtGINum.ReadOnly = true;

            txtCustID.ContextMenu = vendid;
            txtCustName.ContextMenu = vendid;
            txtSONum.ContextMenu = vendid;
            txtGINum.ContextMenu = vendid;
            //tia edit end

            if (ControlMgr.GroupName == "Sales Manager")
            {
                btnApproveRSM.Visible = true;
                btnApprove.Text = "Approved by Sales Manager";
            }
            else if (ControlMgr.GroupName == "Stock Manager")
            {
                btnApproveRSM.Visible = false;
                btnApprove.Text = "Approved by Stock Manager";
            }
        }

        public void ModeAfterApproveOrRejectOrRequestSM()
        {
            btnApprove.Visible = false;
            btnApproveRSM.Visible = false;
            btnReject.Visible = false;
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

                    this.dgvNRJ.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroupId"], Dr["SubGroup2Id"], decimal.Parse(Remaining_Qty) + decimal.Parse(Dr["UoM_Qty"].ToString()), Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], decimal.Parse(Ratio_Actual), Dr["GoodsIssued_SeqNo"], Dr["InventSiteId"], Dr["Notes"]);
                    if (Dr["ActionCode"].ToString() == "01")
                    {
                        dgvNRJ.Rows[j].Cells["JenisRetur"].Value = "Retur Tukar Barang";
                    }
                    else if (Dr["ActionCode"].ToString() == "02")
                    {
                        dgvNRJ.Rows[j].Cells["JenisRetur"].Value = "Retur Credit Nota";
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
                dgvNRJ.Columns["JenisRetur"].Visible = false;

                dgvNRJ.AutoResizeColumns();

                Conn.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                DialogResult dr = MessageBox.Show("NRJ No = " + NRJNumber + "\n" + "Apakah data diatas akan direject ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    if (ControlMgr.GroupName == "Sales Manager")
                    {
                        Query = "";

                        Query = "UPDATE NotaReturJualH SET ";
                        Query += "TransStatusId = '02', ";
                        Query += "UpdatedDate = GETDATE(), ";
                        Query += "UpdatedBy = '" + ControlMgr.UserId + "', ";
                        Query += "ApprovedBy = '' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where NRJId='" + txtNRJNum.Text.Trim() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        ListMethod.StatusLogCustomer("NRJApproval", "NotaReturJual", txtCustID.Text, "02", "", txtNRJNum.Text.Trim(), "", "", "");

                        //Update Qty
                        List<string> GoodsIssued_SeqNo = new List<string>();
                        List<string> GoodsIssuedId = new List<string>();
                        List<decimal> UoM_Qty = new List<decimal>();
                        decimal RemainingQty, QtyNew = 0;

                        Query = "SELECT GoodsIssued_SeqNo, GoodsIssuedId, UoM_Qty FROM NotaReturJual_Dtl WHERE NRJId='" + txtNRJNum.Text.Trim() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            GoodsIssued_SeqNo.Add(Dr["GoodsIssued_SeqNo"].ToString());
                            GoodsIssuedId.Add(Dr["GoodsIssuedId"].ToString());
                            UoM_Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                        }
                        Dr.Close();

                        for (int i = 0; i < dgvNRJ.RowCount; i++)
                        {
                            Query = "SELECT Remaining_Qty FROM GoodsIssuedD WHERE GoodsIssuedId = '" + GoodsIssuedId[i] + "' AND GoodsIssuedSeqNo = '" + GoodsIssued_SeqNo[i] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                            QtyNew = RemainingQty + UoM_Qty[i];

                            Query = "UPDATE GoodsIssuedD SET ";
                            Query += "Remaining_Qty = '" + QtyNew + "' ";
                            Query += "WHERE GoodsIssuedId = '" + GoodsIssuedId[i] + "' AND GoodsIssuedSeqNo='" + GoodsIssued_SeqNo[i] + "'";
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
                            decimal amount = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                            Query = "";
                            //Insert to NotaReturJual_LogTable
                            Query = "INSERT INTO NotaReturJual_LogTable ";
                            Query += "(NRJDate, NRJId, GoodsIssuedDate, GoodsIssuedId, ";
                            Query += "CustId, SiteId, FullItemId, SeqNo, ";
                            Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                            Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                            Query += "('" + dtNRJ.Value.ToString("yyyy-MM-dd") + "', '" + NRJNumber + "', '" + dtGI.Value.ToString("yyyy-MM-dd") + "', '" + txtGINum.Text + "', ";
                            Query += "'" + txtCustID.Text + "', '" + txtSiteID.Text + "', '" + getFullItemID + "', '" + dgvNRJ.Rows[i].Cells["No"].Value.ToString() + "', ";
                            Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amount + "', '02', ";
                            Query += "'Rejected by Sales Manager', 'Status: 02. Rejected by Sales Manager', '" + ControlMgr.UserId + "', GETDATE()) ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else if (ControlMgr.GroupName == "Stock Manager")
                    {
                        Query = "";

                        Query = "UPDATE NotaReturJualH SET ";
                        Query += "TransStatusId = '06', ";
                        Query += "UpdatedDate = GETDATE(), ";
                        Query += "UpdatedBy = '" + ControlMgr.UserId + "', ";
                        Query += "ApprovedBy = '' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where NRJId='" + txtNRJNum.Text.Trim() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        //Update Qty
                        List<string> GoodsIssued_SeqNo = new List<string>();
                        List<string> GoodsIssuedId = new List<string>();
                        List<decimal> UoM_Qty = new List<decimal>();
                        decimal RemainingQty, QtyNew = 0;

                        Query = "SELECT GoodsIssued_SeqNo, GoodsIssuedId, UoM_Qty FROM NotaReturJual_Dtl WHERE NRJId='" + txtNRJNum.Text.Trim() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            GoodsIssued_SeqNo.Add(Dr["GoodsIssued_SeqNo"].ToString());
                            GoodsIssuedId.Add(Dr["GoodsIssuedId"].ToString());
                            UoM_Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                        }
                        Dr.Close();

                        for (int i = 0; i < dgvNRJ.RowCount; i++)
                        {
                            Query = "SELECT Remaining_Qty FROM GoodsIssuedD WHERE GoodsIssuedId = '" + GoodsIssuedId[i] + "' AND GoodsIssuedSeqNo = '" + GoodsIssued_SeqNo[i] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                            QtyNew = RemainingQty + UoM_Qty[i];

                            Query = "UPDATE GoodsIssuedD SET ";
                            Query += "Remaining_Qty = '" + QtyNew + "' ";
                            Query += "WHERE GoodsIssuedId = '" + GoodsIssuedId[i] + "' AND GoodsIssuedSeqNo='" + GoodsIssued_SeqNo[i] + "'";
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
                            decimal amount = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                            Query = "";
                            //Insert to NotaReturJual_LogTable
                            Query = "INSERT INTO NotaReturJual_LogTable ";
                            Query += "(NRJDate, NRJId, GoodsIssuedDate, GoodsIssuedId, ";
                            Query += "CustId, SiteId, FullItemId, SeqNo, ";
                            Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                            Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                            Query += "('" + dtNRJ.Value.ToString("yyyy-MM-dd") + "', '" + NRJNumber + "', '" + dtGI.Value.ToString("yyyy-MM-dd") + "', '" + txtGINum.Text + "', ";
                            Query += "'" + txtCustID.Text + "', '" + txtSiteID.Text + "', '" + getFullItemID + "', '" + dgvNRJ.Rows[i].Cells["No"].Value.ToString() + "', ";
                            Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amount + "', '06', ";
                            Query += "'Rejected by Stock Manager', 'Status: 06. Rejected by Stock Manager', '" + ControlMgr.UserId + "', GETDATE()) ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                }

                Trans.Commit();
                MessageBox.Show("Data NRJNumber : " + txtNRJNum.Text + " berhasil direject.");
                Parent.RefreshGrid();
                ModeAfterApproveOrRejectOrRequestSM();
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

        private void btnApprove_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                DialogResult dr = MessageBox.Show("NRJ No = " + NRJNumber + "\n" + "Apakah data diatas akan diapprove ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    if (ControlMgr.GroupName == "Sales Manager")
                    {
                        Query = "";

                        Query = "UPDATE NotaReturJualH SET ";
                        Query += "TransStatusId = '03', ";
                        Query += "ApprovedBy = '" + ControlMgr.UserId + "', ";
                        Query += "UpdatedDate = GETDATE(), ";
                        Query += "UpdatedBy = '" + ControlMgr.UserId + "' ";
                        Query += "WHERE NRJId = '" + txtNRJNum.Text.Trim() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        ListMethod.StatusLogCustomer("NRJApproval", "NotaReturJual", txtCustID.Text, "03", "", txtNRJNum.Text.Trim(), "", "", "");


                        for (int i = 0; i < dgvNRJ.RowCount; i++)
                        {
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
                            decimal amount = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                            Query = "";
                            //Get Retur_Jual_Approved_Oustanding_UoM, Retur_Jual_Approved_Oustanding_Alt, Retur_Jual_Approved_Oustanding_Amount (Old)
                            Query = "SELECT Retur_Jual_Approved_Oustanding_UoM FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RJAO_UoM_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Retur_Jual_Approved_Oustanding_Alt FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RJAO_Alt_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Retur_Jual_Approved_Oustanding_Amount FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RJAO_Amount_Old = Cmd.ExecuteScalar().ToString();

                            decimal RJAO_UoM_New = decimal.Parse(RJAO_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                            decimal RJAO_Alt_New = decimal.Parse(RJAO_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                            decimal RJAO_Amount_New = decimal.Parse(RJAO_Amount_Old.ToString()) + decimal.Parse(amount.ToString());

                            Query = "";
                            //Update Invent_Sales_Qty
                            Query = "UPDATE Invent_Sales_Qty SET ";
                            Query += "Retur_Jual_Approved_Oustanding_UoM = '" + RJAO_UoM_New + "', ";
                            Query += "Retur_Jual_Approved_Oustanding_Alt = '" + RJAO_Alt_New + "', ";
                            Query += "Retur_Jual_Approved_Oustanding_Amount = '" + RJAO_Amount_New + "' ";
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
                            Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amount + "', '03', ";
                            Query += "'Approved by Sales Manager', 'Status: 03. Approved by Sales Manager', '" + ControlMgr.UserId + "', GETDATE()) ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else if (ControlMgr.GroupName == "Stock Manager")
                    {
                        Query = "";

                        Query = "UPDATE NotaReturJualH SET ";
                        Query += "TransStatusId = '05', ";
                        Query += "ApprovedBy = '" + ControlMgr.UserId + "', ";
                        Query += "UpdatedDate = GETDATE(), ";
                        Query += "UpdatedBy = '" + ControlMgr.UserId + "' ";
                        Query += "WHERE NRJId = '" + txtNRJNum.Text.Trim() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        for (int i = 0; i < dgvNRJ.RowCount; i++)
                        {
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
                            decimal amount = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                            Query = "";
                            //Get Retur_Jual_Approved_Oustanding_UoM, Retur_Jual_Approved_Oustanding_Alt, Retur_Jual_Approved_Oustanding_Amount (Old)
                            Query = "SELECT Retur_Jual_Approved_Oustanding_UoM FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RJAO_UoM_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Retur_Jual_Approved_Oustanding_Alt FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RJAO_Alt_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Retur_Jual_Approved_Oustanding_Amount FROM Invent_Sales_Qty WHERE FullItemID = '" + getFullItemID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RJAO_Amount_Old = Cmd.ExecuteScalar().ToString();

                            decimal RJAO_UoM_New = decimal.Parse(RJAO_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                            decimal RJAO_Alt_New = decimal.Parse(RJAO_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                            decimal RJAO_Amount_New = decimal.Parse(RJAO_Amount_Old.ToString()) + decimal.Parse(amount.ToString());

                            Query = "";
                            //Update Invent_Sales_Qty
                            Query = "UPDATE Invent_Sales_Qty SET ";
                            Query += "Retur_Jual_Approved_Oustanding_UoM = '" + RJAO_UoM_New + "', ";
                            Query += "Retur_Jual_Approved_Oustanding_Alt = '" + RJAO_Alt_New + "', ";
                            Query += "Retur_Jual_Approved_Oustanding_Amount = '" + RJAO_Amount_New + "' ";
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
                            Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amount + "', '05', ";
                            Query += "'Approved by Stock Manager', 'Status: 05. Approved by Stock Manager', '" + ControlMgr.UserId + "', GETDATE()) ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                }

                Trans.Commit();
                MessageBox.Show("Data NRJNumber : " + txtNRJNum.Text + " berhasil diapprove.");
                Parent.RefreshGrid();
                ModeAfterApproveOrRejectOrRequestSM();
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

        private void btnApproveRSM_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                DialogResult dr = MessageBox.Show("NRJ No = " + NRJNumber + "\n" + "Apakah data diatas akan dipindah Approval ke Stock Manager ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    if (ControlMgr.GroupName == "Sales Manager")
                    {
                        Query = "";

                        Query = "UPDATE NotaReturJualH SET ";
                        Query += "TransStatusId = '04', ";
                        Query += "UpdatedDate = GETDATE(), ";
                        Query += "UpdatedBy = '" + ControlMgr.UserId + "', ";
                        Query += "ApprovedBy = '' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where NRJId='" + txtNRJNum.Text.Trim() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        ListMethod.StatusLogCustomer("NRJApproval", "NotaReturJual", txtCustID.Text, "04", "", txtNRJNum.Text.Trim(), "", "", "");
                        
                        for (int i = 0; i < dgvNRJ.RowCount; i++)
                        {
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
                            decimal amount = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                            Query = "";
                            //Insert to NotaReturJual_LogTable
                            Query = "INSERT INTO NotaReturJual_LogTable ";
                            Query += "(NRJDate, NRJId, GoodsIssuedDate, GoodsIssuedId, ";
                            Query += "CustId, SiteId, FullItemId, SeqNo, ";
                            Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                            Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                            Query += "('" + dtNRJ.Value.ToString("yyyy-MM-dd") + "', '" + NRJNumber + "', '" + dtGI.Value.ToString("yyyy-MM-dd") + "', '" + txtGINum.Text + "', ";
                            Query += "'" + txtCustID.Text + "', '" + txtSiteID.Text + "', '" + getFullItemID + "', '" + dgvNRJ.Rows[i].Cells["No"].Value.ToString() + "', ";
                            Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amount + "', '04', ";
                            Query += "'Waiting for Approval by Stock Manager', 'Status: 04. Waiting for Approval by Stock Manager', '" + ControlMgr.UserId + "', GETDATE()) ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    //else if (ControlMgr.GroupName == "Stock Manager")
                    //{

                    //}
                }

                Trans.Commit();
                MessageBox.Show("Data NRJNumber : " + txtNRJNum.Text + " berpindah Approval ke Stock Manager.");
                Parent.RefreshGrid();
                ModeAfterApproveOrRejectOrRequestSM();
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
        //tia edit
        //klik kanan
        PopUp.CustomerID.Customer Cust = null;
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.InventSite InventsiteId = null;
        Sales.SalesOrder.SOHeader SOID = null;
        Sales.BBK.BBKHeader BBKId = null;

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void txtCustID_MouseDown(object sender, MouseEventArgs e)
        {
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

        private void txtCustName_MouseDown(object sender, MouseEventArgs e)
        {
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
                if (SOID == null || SOID.Text == "")
                {
                    txtSONum.Enabled = true;
                    SOID = new Sales.SalesOrder.SOHeader();
                    SOID.SetMode("PopUp", txtSONum.Text);
                    SOID.ParentRefreshGrid6(this);
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
            if (e.Button == MouseButtons.Right)
            {
                if (BBKId == null || BBKId.Text == "")
                {
                    txtGINum.Enabled = true;
                    BBKId = new Sales.BBK.BBKHeader();
                    BBKId.SetMode("PopUp", txtGINum.Text);
                    BBKId.ParentRefreshGrid2(this);
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
