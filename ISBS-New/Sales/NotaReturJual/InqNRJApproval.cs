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
    public partial class InqNRJApproval : MetroFramework.Forms.MetroForm
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

        public InqNRJApproval()
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

        private void InqNRJApproval_Load(object sender, EventArgs e)
        {
            addCmbCriteria();
            ModeLoad();
        }

        private void addCmbCriteria()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName, DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'NotaReturJualH' AND UPPER(FieldName) <> 'StatusName'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[1]);
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
            if (ControlMgr.GroupName == "Sales Manager")
            {
                Approval_List("'01'");
            }
            else if (ControlMgr.GroupName == "Stock Manager")
            {
                Approval_List("'04'");
            }
            //else if (ControlMgr.GroupName == "Administrator")
            //{
            //    Approval_List("'01', '04'");
            //}
        }

        private void Approval_List(string StatusCode)
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY NRJ.CreatedDate DESC) AS [No], NRJ.NRJId AS [NRJ No], NRJ.NRJDate AS [NRJ Date], NRJ.NRJMode AS [Mode], ";
            Query += "NRJ.GoodsIssuedId AS [GI No], NRJ.GoodsIssuedDate AS [GI Date], CT.CustName AS [Customer], NRJ.SiteName AS [Warehouse], TST.Deskripsi AS [Status], ";
            Query += "NRJ.CreatedDate AS [Created Date], NRJ.CreatedBy AS [Created By], NRJ.UpdatedDate AS [Updated Date], NRJ.UpdatedBy AS [Updated By] ";
            Query += "FROM NotaReturJualH NRJ LEFT JOIN TransStatusTable TST ON NRJ.TransStatusId = TST.StatusCode LEFT JOIN CustTable CT ON NRJ.CustId = CT.CustId WHERE TransCode = 'NotaReturJual' AND NRJ.TransStatusId IN (" + StatusCode + ") ";
            if (crit == null)
            {
                Query += ") A ";
            }
            else if (crit == "All")
            {
                Query += "AND (NRJ.NRJId LIKE '%" + txtSearch.Text + "%' OR CT.CustName LIKE '%" + txtSearch.Text + "%' OR NRJ.GoodsIssuedId LIKE '%" + txtSearch.Text + "%' OR NRJ.SiteName LIKE '%" + txtSearch.Text + "%' OR NRJ.CreatedBy LIKE '%" + txtSearch.Text + "%')) A ";
            }
            else if (crit == "NRJ No")
            {
                Query += "AND NRJ.NRJId LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "NRJ Date")
            {
                Query += "AND (CONVERT(VARCHAR(10),NRJ.NRJDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRJ.NRJDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            else if (crit == "Customer")
            {
                Query += "AND CT.CustName LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "GI No")
            {
                Query += "AND NRJ.GoodsIssuedId LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "Warehouse")
            {
                Query += "AND NRJ.SiteName LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "Created Date")
            {
                Query += "AND (CONVERT(VARCHAR(10),NRJ.CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRJ.CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            else if (crit == "Created By")
            {
                Query += "AND NRJ.CreatedBy LIKE '%" + txtSearch.Text + "%') A ";
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

            //DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            //buttonSend.Name = "Send Email";
            //buttonSend.HeaderText = "Send Email";
            //buttonSend.Text = "Send Email";
            //buttonSend.UseColumnTextForButtonValue = true;

            dgvNRJ.AutoGenerateColumns = true;
            dgvNRJ.DataSource = Dt;
            dgvNRJ.Refresh();
            dgvNRJ.AutoResizeColumns();
            dgvNRJ.Columns["NRJ Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRJ.Columns["GI Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRJ.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRJ.Columns["Updated Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            //if (!dgvNRJ.Columns.Contains("Preview"))
            //    dgvNRJ.Columns.Add(buttonpreview);
            //if (!dgvNRJ.Columns.Contains("Send Email"))
            //    dgvNRJ.Columns.Add(buttonSend);

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE TransStatusId IN (" + StatusCode + ") ";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE ";
                Query += "(NRJId LIKE '%" + txtSearch.Text + "%' OR SiteName LIKE '%" + txtSearch.Text + "%' OR CustName LIKE '%" + txtSearch.Text + "%' OR GoodsIssuedId LIKE '%" + txtSearch.Text + "%' OR CreatedBy LIKE '%" + txtSearch.Text + "%') AND TransStatusId IN (" + StatusCode + ") ";
            }
            else if (crit.Equals("NRJ Date"))
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE ";
                Query += "(CONVERT(VARCHAR(10),NRJDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRJDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatusId IN (" + StatusCode + ") ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatusId IN (" + StatusCode + ") ;";
            }
            else
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE TransStatusId IN (" + StatusCode + ") ";
                if (crit.Equals("NRJ No"))
                    Query += "AND NRJId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Warehouse"))
                    Query += "AND SiteName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Customer"))
                    Query += "AND CustName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("GI No"))
                    Query += "AND GoodsIssuedId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Created By"))
                    Query += "AND CreatedBy Like '%" + txtSearch.Text + "%' ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            crit = null;
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
            RefreshGrid();
        }
                
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name == "NRJApproval")
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectNRJ();
        }

        private void SelectNRJ()
        {
            NRJApproval f = new NRJApproval();
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvNRJ.CurrentRow == null)
                {
                    MessageBox.Show("Maaf List Masih Kosong");
                    return;
                }
                else
                {
                    string NRJID = dgvNRJ.CurrentRow.Cells["NRJ No"].Value.ToString();
                    f.SetMode("BeforeEdit", NRJID);
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

        private void dgvNRJ_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectNRJ();
            }
        }

        private void dgvNRJ_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                if (e.ColumnIndex > -1 && dgvNRJ.Columns[e.ColumnIndex].Name == "Preview")
                {
                    string NRBId = dgvNRJ.Rows[e.RowIndex].Cells["NRJ No"].Value == null ? "" : dgvNRJ.Rows[e.RowIndex].Cells["NRJ No"].Value.ToString();

                    //PreviewNRB f = new PreviewNRB(NRBId);
                    //f.Show();
                }
                else if (e.ColumnIndex > -1 && dgvNRJ.Columns[e.ColumnIndex].Name == "Send Email")
                {
                    string NRBId = dgvNRJ.Rows[e.RowIndex].Cells["NRJ No"].Value == null ? "" : dgvNRJ.Rows[e.RowIndex].Cells["NRJ No"].Value.ToString();

                    //SendEmail s = new SendEmail(NRBId);
                    //s.Show();
                }
            }
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
