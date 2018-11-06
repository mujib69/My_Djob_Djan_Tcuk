using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.RTBApproval
{
    public partial class InqRTBApproval : MetroFramework.Forms.MetroForm
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

        public InqRTBApproval()
        {
            InitializeComponent();
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

        private void InqRTBApproval_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            //lblForm.Location = new Point(16, 11);
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY [RTBId] desc) [No], [RTBId] 'RTB ID',  [RTBDate] 'RTB Date', [VendId], [VendName],  [SiteID] 'Warehouse Code', [SiteName] 'Warehouse', [SiteLocation] 'Location', b.Deskripsi, [ApprovedBy], [CreatedDate], [CreatedBy] From [ReturTukarBarangH] a Left JOIN [TransStatusTable] b ON a.TransStatus = b.StatusCode And b.TransCode = 'ReturTukarBarang' ";

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

            dgvPR.AutoGenerateColumns = true;
            dgvPR.DataSource = Dt;
            dgvPR.Refresh();
            dgvPR.AutoResizeColumns();
            dgvPR.Columns["RTB Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

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

        private void SelectRTB()
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            RTBApproval f = new RTBApproval();              
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

        
    }
}
