using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.ReturTukarBarang
{
    public partial class InqRTB : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        public static int dataShow;

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //timer autorefresh
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

        public InqRTB()
        {
            InitializeComponent();
        }

        private void InqRTB_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            //lblForm.Location = new Point(16, 11);
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY [RTBId] desc) [No], [RTBId] 'RTB ID',  [RTBDate] 'RTB Date', [VendId], [VendName],  [SiteID] 'Warehouse Code', [SiteName] 'Warehouse', [SiteLocation] 'Location',  b.Deskripsi, [ApprovedBy], [CreatedDate], [CreatedBy] From [ReturTukarBarangH] a Left JOIN [TransStatusTable] b ON a.TransStatus = b.StatusCode And b.TransCode = 'ReturTukarBarang' ";

            if (crit == null)
                Query += ") a ";
            else if (crit.Equals("All"))
            {
                Query += "and [RTBId] like '%" + txtSearch.Text + "%' or [SiteID] like '%" + txtSearch.Text + "%' or [SiteName] like '%" + txtSearch.Text + "%' or [SiteLocation] like '%" + txtSearch.Text + "%' or [VendId] like '%" + txtSearch.Text + "%' or [VendName] like '%" + txtSearch.Text + "%') a ";
            }
            else if (crit.Equals("ReceiptOrderId") || crit.Equals("CreatedDate") || crit.Equals("ReceiptOrderStatus"))
            {
                //Query += "where (CONVERT(VARCHAR(10),ReceiptOrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),ReceiptOrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (" + crit + " like '%" + txtSearch.Text + "%')) a ";
            }
            Query += "Where a.No Between " + Limit1 + " and " + Limit2 + " ;";

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

            dgvPR.AutoGenerateColumns = true;
            dgvPR.DataSource = Dt;
            dgvPR.Refresh();
            dgvPR.AutoResizeColumns();
            dgvPR.Columns["RTB Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            if (!dgvPR.Columns.Contains("Preview"))
                dgvPR.Columns.Add(buttonpreview);
            if (!dgvPR.Columns.Contains("Send Email"))
                dgvPR.Columns.Add(buttonSend);

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(RTBId) From [dbo].[ReturTukarBarangH] ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(RTBId) From [dbo].[ReturTukarBarangH] Where ";
                Query += "RTBId like '%" + txtSearch.Text + "%' or VendId like '%" + txtSearch.Text + "%'  or VendName like '%" + txtSearch.Text + "%' or SiteID like '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("RTBDate"))
            {
                Query = "Select Count(RTBId) From [dbo].[ReturTukarBarangH] Where ";
                Query += "(CONVERT(VARCHAR(10),RTBDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),RTBDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ;";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "Select Count (RTBId) From [dbo].[ReturTukarBarangH] Where ";
                Query += "(CONVERT(VARCHAR(10),RTBDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),RTBDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ;";
            }
            else
            {
                Query = "Select Count(RTBId) From [dbo].[ReturTukarBarangH] Where ";
                Query += crit + " Like '%" + txtSearch.Text + "%' ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (dataShow != 0)
                Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        public int DataShow { get { return dataShow; } set { dataShow = value; } }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'ReturTukarBarangH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
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

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
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

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
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

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox] ";

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
                crit = "All";
            else
                crit = cmbCriteria.SelectedItem.ToString();

            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                RTBHeader header = new RTBHeader();
                header.SetMode("New", "");
                header.setParent(this);
                header.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void SelectRTB()
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            RTBHeader f = new RTBHeader();
               
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                String RTBID = dgvPR.CurrentRow.Cells["RTB ID"].Value.ToString();

                f.SetMode("BeforeEdit", RTBID);
                f.setParent(this);
                f.Show();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectRTB();
        }

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectRTB();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvPR.RowCount > 0)
                    {
                        Index = dgvPR.CurrentRow.Index;
                        string RTBId = dgvPR.Rows[Index].Cells["RTB ID"].Value == null ? "" : dgvPR.Rows[Index].Cells["RTB ID"].Value.ToString();

                        DialogResult dr = MessageBox.Show("RTB ID = " + RTBId + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            string Check = "";

                            Query = "Select TransStatus from [ReturTukarBarangH] where RTBId='" + RTBId + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            Check = Cmd.ExecuteScalar().ToString();
                            if (Check != "01")
                            {
                                MessageBox.Show("RTB ID = " + RTBId + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                                Conn.Close();
                                return;
                            }

                            //Update Qty
                            List<string> GoodsReceivedSeqNo = new List<string>();
                            List<string> GoodsReceivedID = new List<string>();
                            List<decimal> Qty = new List<decimal>();
                            decimal RemainingQty, QtyNew = 0;
                            Query = "Select GoodsReceivedSeqNo,UoM_Qty,[GoodsReceivedID] From [ReturTukarBarangD] Where RTBId='" + RTBId + "'";
                            Cmd = new SqlCommand(Query, Conn);
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
                                Query = "Select Remaining_Qty From [GoodsReceivedD] Where [GoodsReceivedId] = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo[i] + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                                QtyNew = RemainingQty + Qty[i];

                                Query = "Update [GoodsReceivedD] set ";
                                Query += "Remaining_Qty='" + QtyNew + "' ";
                                Query += "where GoodsReceivedId='" + GoodsReceivedID[i] + "' And GoodsReceivedSeqNo='" + GoodsReceivedSeqNo[i] + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                                Query = "";
                            }

                            //delete header
                            Query = "Delete from [ReturTukarBarangH] where RTBId='" + RTBId + "';";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            //delete item
                            Query = "Delete from ReturTukarBarangD where RTBId ='" + RTBId + "'; ";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("RTB ID = " + RTBId.ToUpper() + "\n" + "Data berhasil dihapus.");

                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                            Index = 0;
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        
    }
}
