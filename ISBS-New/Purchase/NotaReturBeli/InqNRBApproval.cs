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
    public partial class InqNRBApproval : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query, crit = null;
        int Total, Limit1, Limit2, Page1, Page2, Index;
        public static int dataShow;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = 10 * 1000;//miliscond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        public InqNRBApproval()
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

        private void InqNRBApproval_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            Query = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY NRB.NRBId DESC) AS [No], ";
            Query += "NRB.NRBId AS [NRB No], NRB.NRBDate AS [NRB Date], NRB.NRBMode AS [Mode], NRB.GoodsReceivedId AS [GR No], NRB.GoodsReceivedDate AS [GR Date], ";
            Query += "VT.VendName AS [Vendor], NRB.SiteName AS [Warehouse], TST.Deskripsi AS [Status], NRB.CreatedDate AS [Created Date], NRB.CreatedBy AS [Created By], ";
            Query += "NRB.UpdatedDate AS [Updated Date], NRB.UpdatedBy AS [Updated By] ";
            Query += "FROM NotaReturBeliH NRB LEFT JOIN TransStatusTable TST ON NRB.TransStatusId = TST.StatusCode ";
            Query += "LEFT JOIN VendTable VT ON VT.VendId = NRB.VendId WHERE TST.TransCode = 'NotaReturBeli' AND TST.StatusCode = '13' ";

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
            Query += "WHERE A.[No] BETWEEN " + Limit1 + " AND " + Limit2;

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            //DataGridViewButtonColumn buttonpreview = new DataGridViewButtonColumn();
            //buttonpreview.Name = "Preview";
            //buttonpreview.HeaderText = "Preview";
            //buttonpreview.Text = "Preview";
            //buttonpreview.UseColumnTextForButtonValue = true;
            
            //Dt.Columns.Add(new DataColumn("colStatus", typeof(System.Windows.Forms.Button)));

            //DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            //buttonSend.Name = "Send Email";
            //buttonSend.HeaderText = "Send Email";
            //buttonSend.Text = "Send Email";
            //buttonSend.UseColumnTextForButtonValue = true;

            dgvNRB.AutoGenerateColumns = true;
            dgvNRB.DataSource = Dt;
            dgvNRB.Refresh();
            dgvNRB.AutoResizeColumns();
            dgvNRB.Columns["NRB Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRB.Columns["GR Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRB.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvNRB.Columns["Updated Date"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            Conn.Close();

            //if (!dgvNRB.Columns.Contains("Preview"))
            //    dgvNRB.Columns.Add(buttonpreview);
            //if (!dgvNRB.Columns.Contains("Send Email"))
            //    dgvNRB.Columns.Add(buttonSend);

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE TransStatusId = '13'";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "(NRBId LIKE '%" + txtSearch.Text + "%' OR SiteName LIKE '%" + txtSearch.Text + "%' OR VendName LIKE '%" + txtSearch.Text + "%' OR GoodsReceivedId LIKE '%" + txtSearch.Text + "%' OR CreatedBy LIKE '%" + txtSearch.Text + "%') AND TransStatusId = '13' ";
            }
            else if (crit.Equals("NRB Date"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "(CONVERT(VARCHAR(10),NRBDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRBDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatusId = '13' ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatusId = '13' ;";
            }
            else
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE TransStatusId = '13' ";
                if (crit.Equals("NRB No"))
                    Query += "AND NRBId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Warehouse"))
                    Query += " AND SiteName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Vendor"))
                    Query += "AND VendName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("GR No"))
                    Query += "AND GoodsReceivedId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Created By"))
                    Query += "AND CreatedBy Like '%" + txtSearch.Text + "%' ";
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

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT FieldName, DisplayName FROM [User].[Table] WHERE SchemaName = 'dbo' AND TableName = 'NotaReturBeliH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbCriteria.DisplayMember = "Text";
            cmbCriteria.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr["DisplayName"]);
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
            Query = "SELECT CmbValue FROM [Setting].[CmbBox]";
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

        private void SelectNRB()
        {
            NRBApproval f = new NRBApproval();
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
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectNRB();
        }

        private void dgvNRB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectNRB();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name == "NRBApproval")
                    Application.OpenForms[i].Close();
            }
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
                txtSearch.Enabled = true;
            }
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
