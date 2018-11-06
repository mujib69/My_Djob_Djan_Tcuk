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
    public partial class InqReturBeli : MetroFramework.Forms.MetroForm
    {
        public InqReturBeli()
        {
            InitializeComponent();
        }

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        bool Journal = false;

        string Query, crit = null;
        int Total, Limit1, Limit2, Page1, Page2, Index;
        public static int dataShow;
        private string TransStatus = String.Empty;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10 * 1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (timerRefresh == null)
            {

            }
            else
            {
                RefreshGrid();
            }
        }

        private void InqReturBeli_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            addCmbCriteria();
            ModeLoad();

            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void addCmbCriteria()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName, DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'NotaReturBeliH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[1]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Clear();
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'03', '04', '13'";
                }
            }
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='NotaReturBeli' AND StatusCode IN (" + TransStatus + ")";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbStatusCode.DisplayMember = "Text";
            cmbStatusCode.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbStatusCode.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[1] + "" });
            }
            cmbStatusCode.SelectedIndex = 0;
            Conn.Close();
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            RefreshGrid();
        }

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox]";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            cmbShow.Items.Clear();
            while (Dr.Read())
            {
                cmbShow.Items.Add(Dr.GetInt32(0));
            }
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShow.SelectedIndex = 0;
        }

        public void RefreshGrid()
        {
            string cmbStatusSelected = cmbStatusCode.Text;

            if (cmbStatusSelected == "Auto – Approved by Purchasing Manager")
            {
                cmbStatusSelected = "03";
            }
            else if (cmbStatusSelected == "Approved by Purchasing Manager")
            {
                cmbStatusSelected = "04";
            }
            else if (cmbStatusSelected == "Rejected")
            {
                cmbStatusSelected = "05";
            }
            else if (cmbStatusSelected == "GI Issued")
            {
                cmbStatusSelected = "08";
            }
            else if (cmbStatusSelected == "GI Issued - Partial")
            {
                cmbStatusSelected = "09";
            }
            else if (cmbStatusSelected == "GR In Progress")
            {
                cmbStatusSelected = "10";
            }
            else if (cmbStatusSelected == "GR Issued")
            {
                cmbStatusSelected = "11";
            }
            else if (cmbStatusSelected == "GR Partial")
            {
                cmbStatusSelected = "12";
            }
            else if (cmbStatusSelected == "Waiting for Approval")
            {
                cmbStatusSelected = "13";
            }
            else if (cmbStatusSelected == "Deleted")
            {
                cmbStatusSelected = "14";
            }
            else
            {
                cmbStatusSelected = "";
            }

            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'03', '04', '13'";
                }
            }

            Conn = ConnectionString.GetConnection();

            Query = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY NRB.NRBId DESC) AS [No], ";
            Query += "NRB.NRBId AS [NRB No], NRB.NRBDate AS [NRB Date], NRB.NRBMode AS [Mode], NRB.GoodsReceivedId AS [GR No], NRB.GoodsReceivedDate AS [GR Date], ";
            Query += "VT.VendName AS [Vendor], NRB.SiteName AS [Warehouse], TST.Deskripsi AS [Status], NRB.CreatedDate AS [Created Date], NRB.CreatedBy AS [Created By], ";
            Query += "NRB.UpdatedDate AS [Updated Date], NRB.UpdatedBy AS [Updated By] ";
            Query += "FROM NotaReturBeliH NRB LEFT JOIN TransStatusTable TST ON NRB.TransStatusId = TST.StatusCode ";
            Query += "LEFT JOIN VendTable VT ON VT.VendId = NRB.VendId WHERE TST.TransCode = 'NotaReturBeli' AND NRB.TransStatusId IN (" + TransStatus + ") ";

            if (crit == null)
            {
                Query += ") A ";
            }
            else if (crit.Equals("All"))
            {
                Query += "AND (NRB.NRBId LIKE '%" + txtSearch.Text + "%' OR NRB.SiteName LIKE '%" + txtSearch.Text + "%' OR NRB.VendName LIKE '%" + txtSearch.Text + "%' OR NRB.GoodsReceivedId LIKE '%" + txtSearch.Text + "%' OR NRB.CreatedBy LIKE '%" + txtSearch.Text + "%')) A ";
            }  
            else if (crit.Equals("NRB No"))
            {
                Query += "AND NRB.NRBId LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("Warehouse"))
            {
                Query += "AND NRB.SiteName LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("Vendor"))
            {
                Query += "AND NRB.VendName LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("GR No"))
            {
                Query += "AND NRB.GoodsReceivedId LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("Created By"))
            {
                Query += "AND NRB.CreatedBy LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("NRB Date"))
            {
                Query += "AND (CONVERT(VARCHAR(10),NRB.NRBDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRB.NRBDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            else if (crit.Equals("Created Date"))
            {
                Query += "AND (CONVERT(VARCHAR(10),NRB.CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRB.CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            else if (crit == "Status Name")
            {
                Query += "AND NRB.TransStatusId LIKE '%" + cmbStatusSelected + "%') A ";
            }
            Query += "WHERE A.[No] BETWEEN " + Limit1 + " AND " + Limit2;

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            DataGridViewButtonColumn buttonpreview = new DataGridViewButtonColumn();
            buttonpreview.Name = "Preview";
            buttonpreview.HeaderText = "Preview";
            buttonpreview.Text = "Preview";
            buttonpreview.UseColumnTextForButtonValue = true;
            //Dt.Columns.Add(new DataColumn("colStatus", typeof(System.Windows.Forms.Button)));

            DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            buttonSend.Name = "Send Email";
            buttonSend.HeaderText = "Send Email";
            buttonSend.Text = "Send Email";
            buttonSend.UseColumnTextForButtonValue = true;

            dgvNRB.AutoGenerateColumns = true;
            dgvNRB.DataSource = Dt;
            dgvNRB.Refresh();
            dgvNRB.AutoResizeColumns();
            dgvNRB.Columns["NRB Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRB.Columns["GR Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRB.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvNRB.Columns["Updated Date"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            Conn.Close();

            if (!dgvNRB.Columns.Contains("Preview"))
                dgvNRB.Columns.Add(buttonpreview);
            if (!dgvNRB.Columns.Contains("Send Email"))
                dgvNRB.Columns.Add(buttonSend);

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
            
            if (crit == null)
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE TransStatusId IN (" + TransStatus + ") ";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "(NRBId LIKE '%" + txtSearch.Text + "%' OR SiteName LIKE '%" + txtSearch.Text + "%' OR VendName LIKE '%" + txtSearch.Text + "%' OR GoodsReceivedId LIKE '%" + txtSearch.Text + "%' OR CreatedBy LIKE '%" + txtSearch.Text + "%') AND TransStatusId IN (" + TransStatus + ") ";
            }
            else if (crit.Equals("NRB Date"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "(CONVERT(VARCHAR(10),NRBDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRBDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatusId IN (" + TransStatus + ") ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatusId IN (" + TransStatus + ") ;";
            }
            else
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE TransStatusId IN (" + TransStatus + ") ";
                if (crit.Equals("NRB No"))
                    Query += "AND NRBId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Warehouse"))
                    Query += "AND SiteName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Vendor"))
                    Query += "AND VendName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("GR No"))
                    Query += "AND GoodsReceivedId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Created By"))
                    Query += "AND CreatedBy Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Status Name"))
                    Query += "AND TransStatusId LIKE '%" + cmbStatusSelected + "%' ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            //if (dataShow != 0)
            //    Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            //else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
            RefreshGrid();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name == "ReturBeliHeader")
                    Application.OpenForms[i].Close();
                else if (Application.OpenForms[i].Name == "AddGR")
                    Application.OpenForms[i].Close();
                else if (Application.OpenForms[i].Name == "AddItem")
                    Application.OpenForms[i].Close();
                else if (Application.OpenForms[i].Name == "AddWH")
                    Application.OpenForms[i].Close();
            }
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }
            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            RefreshGrid();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvNRB.RowCount > 0)
                    {
                        Conn = ConnectionString.GetConnection();
                        Trans = Conn.BeginTransaction();

                        Index = dgvNRB.CurrentRow.Index;
                        string NRBId = dgvNRB.Rows[Index].Cells["NRB No"].Value == null ? "" : dgvNRB.Rows[Index].Cells["NRB No"].Value.ToString();

                        DialogResult dr = MessageBox.Show("NRB No = " + NRBId + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string Check = "";

                            Query = "SELECT TransStatusId FROM NotaReturBeliH WHERE NRBId ='" + NRBId + "';";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Check = Cmd.ExecuteScalar().ToString();
                            if (Check != "13")
                            {
                                MessageBox.Show("NRB ID = " + NRBId + ".\n" + "Tidak bisa diclosed karena sudah diposting.");
                                Conn.Close();
                                return;
                            }

                            string getSiteId = "";
                            string getNRBDate = "";
                            string getGoodsReceivedDate = "";
                            string getVendId = "";
                            Query = "SELECT SiteId, NRBDate, GoodsReceivedDate, VendId FROM NotaReturBeliH WHERE NRBId = '" + NRBId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.Read())
                            {
                                getSiteId = Dr["SiteId"].ToString();
                                getNRBDate = (Convert.ToDateTime(Dr["NRBDate"])).ToString("yyyy-MM-dd");
                                getGoodsReceivedDate = (Convert.ToDateTime(Dr["GoodsReceivedDate"])).ToString("yyyy-MM-dd");
                                getVendId = Dr["VendId"].ToString();
                            }
                            Dr.Close();

                            //Update Qty
                            List<decimal> NRBSeqNo = new List<decimal>();
                            List<string> GoodsReceivedSeqNo = new List<string>();
                            List<string> GoodsReceivedID = new List<string>();
                            List<string> FullItemID = new List<string>();
                            List<decimal> UoM_Qty = new List<decimal>();
                            List<decimal> Alt_Qty = new List<decimal>();
                            decimal RemainingQty, QtyNew = 0;
                            Query = "SELECT SeqNo, GoodsReceived_SeqNo, GoodsReceivedId, FullItemId, UoM_Qty, Alt_Qty FROM NotaReturBeli_Dtl WHERE NRBId='" + NRBId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                NRBSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                                GoodsReceivedSeqNo.Add(Dr["GoodsReceived_SeqNo"].ToString());
                                GoodsReceivedID.Add(Dr["GoodsReceivedId"].ToString());
                                FullItemID.Add(Dr["FullItemId"].ToString());
                                UoM_Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                                Alt_Qty.Add(decimal.Parse(Dr["Alt_Qty"].ToString()));
                            }
                            Dr.Close();

                            for (int i = 0; i < GoodsReceivedSeqNo.Count; i++)
                            {
                                Query = "SELECT ISNULL(Remaining_Qty, 0)AS Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo[i] + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                                QtyNew = RemainingQty + UoM_Qty[i];

                                Query = "Update GoodsReceivedD set ";
                                Query += "Remaining_Qty='" + QtyNew + "' ";
                                Query += "where GoodsReceivedId='" + GoodsReceivedID[i] + "' And GoodsReceivedSeqNo='" + GoodsReceivedSeqNo[i] + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                                Query = "";

                                Query = "";
                                //Get Price With GRid and FullItemId
                                Query = "SELECT POD.Price ";
                                Query += "FROM GoodsReceivedD GRD ";
                                Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                                Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                                Query += "WHERE GRD.GoodsReceivedId = '" + GoodsReceivedID[i] + "' AND POD.FullItemId = '" + FullItemID[i] + "' ";
                                Cmd = new SqlCommand(Query, Conn, Trans);

                                ////Cek Price Ada dalam list atau tidak
                                //if (Cmd.ExecuteScalar() == null)
                                //{
                                //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                                //    return;
                                //}

                                string getUoM_Qty = UoM_Qty[i].ToString();
                                string getAlt_Qty = Alt_Qty[i].ToString();

                                //string Price = Cmd.ExecuteScalar().ToString();

                                string POPrice = Convert.ToString(Cmd.ExecuteScalar());
                                string Price = "";
                                if (POPrice == "")
                                {
                                    Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + FullItemID[i] + "'";
                                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                                    {
                                        Price = Cmd.ExecuteScalar().ToString();
                                    }
                                }
                                else
                                {
                                    Price = POPrice;
                                }

                                decimal amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                                decimal amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

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
                                Query = "SELECT Available_For_Sale_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + getSiteId + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                string RBIO_UoM_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT Available_For_Sale_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + getSiteId + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                string RBIO_Alt_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT Available_For_Sale_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + getSiteId + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                string RBIO_Amount_Old = Cmd.ExecuteScalar().ToString();

                                //Get Available_UoM, Available_Alt, Available_Amount (Old)
                                Query = "SELECT Available_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + getSiteId + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                string RBIO_Av_UoM_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT Available_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + getSiteId + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                string RBIO_Av_Alt_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT Available_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + getSiteId + "'";
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
                                Query += "WHERE FullItemId = '" + FullItemID[i] + "' AND InventSiteId = '" + getSiteId + "' ";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                Query = "INSERT INTO NotaReturBeli_LogTable ";
                                Query += "(NRBDate, NRBId, GoodsReceivedDate, GoodsReceivedId, ";
                                Query += "VendId, SiteId, FullItemId, SeqNo, ";
                                Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                                Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                                Query += "('" + getNRBDate + "', '" + NRBId + "', '" + getGoodsReceivedDate + "', '" + GoodsReceivedID[i] + "', ";
                                Query += "'" + getVendId + "', '" + getSiteId + "', '" + FullItemID[i] + "', '" + NRBSeqNo[i] + "', ";
                                Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amountPU + "', '14', ";
                                Query += "'Deleted', 'Status: 14. Deleted', '" + ControlMgr.UserId + "', GETDATE()) ";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                //update delete item
                                Query = "UPDATE NotaReturBeli_Dtl SET Closed = 'TRUE' WHERE NRBId = '" + NRBId + "' AND SeqNo = '" + NRBSeqNo[i] + "'; ";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }

                            //update delete header
                            Query = "UPDATE NotaReturBeliH SET ";
                            Query += "TransStatusId = '14',";
                            Query += "UpdatedDate = GETDATE(),";
                            Query += "UpdatedBy = '" + ControlMgr.UserId + "',";
                            Query += "Closed = 'TRUE',";
                            Query += "ApprovedBy = '' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where NRBId='" + NRBId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            //Begin
                            //Created By : Joshua
                            //Created Date : 06 Sept 2018
                            BatalJournal(NRBId);
                            if (Journal == true)
                            {
                                Journal = false;
                                goto Outer;
                            }
                            //Desc : Batal Journal
                            //End

                            MessageBox.Show("NRB ID = " + NRBId.ToUpper() + "\n" + "Data berhasil closed.");
                        }                       

                        Trans.Commit();
                        Index = 0;
                        RefreshGrid();

                        Outer: ;
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void BatalJournal(string NRBId)
        {
            //Get GLJournalHID
            Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + NRBId + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            if (GLJournalHID != "")
            {
                Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                Cmd = new SqlCommand(Query, Conn, Trans);
                int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                if (CountData == 1)
                {
                    //Batal Journal
                    Query = "UPDATE GLJournalH SET Status = 'Batal' WHERE GLJournalHID = '" + GLJournalHID + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    MetroFramework.MetroMessageBox.Show(this, "Tidak dapat closed karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Journal = true;
                    return;
                }
            }           
        }

        ReturBeliHeader header = null;
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                if (header == null || header.Text == "")
                {
                    header = new ReturBeliHeader();
                    header.SetMode("New", "");
                    header.setParent(this);
                    header.Show();
                }
                else if (CheckOpened(header.Name))
                {
                    header.WindowState = FormWindowState.Normal;
                    header.Show();
                    header.Focus();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectNRB();
        }

        private void SelectNRB()
        {
            ReturBeliHeader f = new ReturBeliHeader();

            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvNRB.CurrentRow == null)
                {
                    MessageBox.Show("Maaf List Masih Kosong");
                    return;
                }
                else
                {
                    string NRBID = dgvNRB.CurrentRow.Cells["NRB No"].Value.ToString();

                    f.SetMode("BeforeEdit", NRBID);
                    f.setParent(this);
                    f.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                txtPage.Text = "1";
                RefreshGrid();
            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
        }

        private void dgvNRB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectNRB();
            }
        }

        private void dgvNRB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.ColumnIndex > -1)
            {
                Conn = ConnectionString.GetConnection();
                string NRBId = dgvNRB.Rows[e.RowIndex].Cells["NRB No"].Value == null ? "" : dgvNRB.Rows[e.RowIndex].Cells["NRB No"].Value.ToString();

                Cmd = new SqlCommand("SELECT [VendId] FROM [NotaReturBeliH] WHERE [NRBId] = '" + NRBId + "'", Conn);
                string VendId = Cmd.ExecuteScalar().ToString();

                if (dgvNRB.Columns[e.ColumnIndex].Name == "Preview")
                {             
                    Query = "Select PreviewC From [NotaReturBeliH] Where [NRBId] = '" + NRBId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    int PreviewC = Convert.ToInt32(Cmd.ExecuteScalar());
 
                    if (PreviewC == 0)
                    {
                        //PreviewNRB f = new PreviewNRB(NRBId);
                        //f.Show();

                        GlobalPreview f = new GlobalPreview("Nota Retur Beli", NRBId);
                        f.Show();

                        //Set PreviewC to 1
                        Query = "Update [dbo].[NotaReturBeliH] Set [PreviewC] = '1' , PreviewCtr = PreviewCtr + 1 Where NRBId = '" + NRBId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        if (ControlMgr.GroupName == "Administrator")
                        {
                            DialogResult resPreview = MessageBox.Show(NRBId + Environment.NewLine + "Document already previewed!" + Environment.NewLine + "Allow document to be previewed again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (resPreview == DialogResult.Yes)
                            {
                                Query = "Update [dbo].[NotaReturBeliH] Set [PreviewC] = '0' Where NRBId = '" + NRBId + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                DialogResult resPreviewPrompt = MessageBox.Show("Do you want to preview document now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (resPreviewPrompt == DialogResult.Yes)
                                {
                                    //PreviewNRB f = new PreviewNRB(NRBId);
                                    //f.Show();

                                    GlobalPreview f = new GlobalPreview("Nota Retur Beli", NRBId);
                                    f.Show();

                                    //Set PreviewC to 1
                                    Query = "Update [dbo].[NotaReturBeliH] Set [PreviewC] = '1', PreviewCtr = PreviewCtr + 1 Where NRBId = '" + NRBId + "'";
                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();
                                }
                                if (resPreviewPrompt == DialogResult.No)
                                {
                                    //Some task…  
                                }
                            }
                            if (resPreview == DialogResult.No) //Action not allow email to be sent
                            {
                                //Some task…  
                            }
                        }
                        else
                        {
                            MessageBox.Show("Document already previed!" + Environment.NewLine + "Contact Administrator");
                        }
                    }
                }

                else if (dgvNRB.Columns[e.ColumnIndex].Name == "Send Email")
                {                  
                    Query = "Select SendEmailC From [NotaReturBeliH] Where [NRBId] = '" + NRBId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    int SendEmailC = Convert.ToInt32(Cmd.ExecuteScalar());

                    if (SendEmailC == 0)
                    {
                        //SendEmail s = new SendEmail(this);
                        //s.flag(NRBId);
                        //s.Show();

                        GlobalSendEmail f = new GlobalSendEmail("Nota Retur Beli", NRBId, VendId);
                        f.Show();
                    }
                    else
                    {
                        if (ControlMgr.GroupName == "Administrator")
                        {
                            DialogResult resSendEmail = MessageBox.Show(NRBId + Environment.NewLine + "Email already been Sent!" + Environment.NewLine + "Allow Email to be sent again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (resSendEmail == DialogResult.Yes)
                            {
                                Query = "Update [dbo].[NotaReturBeliH] Set [SendEmailC] = '0' Where NRBId = '" + NRBId + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                #region Ask Resend Email Now?
                                DialogResult resSendEmailPrompt = MessageBox.Show("Do you want to resend email now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (resSendEmailPrompt == DialogResult.Yes)
                                {
                                    //SendEmail s = new SendEmail(this);
                                    //s.flag(NRBId);
                                    //s.Show();

                                    GlobalSendEmail f = new GlobalSendEmail("Nota Retur Beli", NRBId, VendId);
                                    f.Show();
                                }
                                if (resSendEmailPrompt == DialogResult.No)//Action not sending email immidiately
                                {
                                    //Some task…  
                                }
                                #endregion
                            }
                            if (resSendEmail == DialogResult.No) //Action not allow email to be sent
                            {
                                //Some task…  
                            }
                        }
                        else
                        {
                            MessageBox.Show("Email already been Sent!" + Environment.NewLine + "Contact Administrator");
                        }
                    }
                }
                Conn.Close();
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

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }

            if (cmbCriteria.Text.Contains("Status"))
            {
                cmbStatusCode.Enabled = true;
            }
            else
            {
                cmbStatusCode.Enabled = false;
                cmbStatusCode.SelectedIndex = 0;
            }

            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("Status"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'03', '04', '13'";
            addCmbStatusCode();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'05', '08', '09', '10', '11', '12', '14'";
            addCmbStatusCode();
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch.PerformClick();
            }
        }
    }
}
