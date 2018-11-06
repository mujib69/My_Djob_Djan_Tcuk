using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.DashboardApprovalPR
{
    public partial class DashboardApprovalPR : MetroFramework.Forms.MetroForm
    {
        //private SqlConnection Conn;
        //private SqlCommand Cmd;
        //private SqlTransaction Trans;
        //private SqlDataReader Dr;
        //private SqlDataAdapter Da;
        //private DataTable Dt;
        //private DataSet Ds;

        //String Mode, Query, crit = null;
        //int Limit1, Limit2, Total, Page1, Page2, Index;

        //MainMenu Parent = new MainMenu();

        ////timer autorefresh
        private void setTimer()
        {
        //    Timer timerRefresh = new Timer();
        //    timerRefresh.Interval = (10*1000);//milisecond
        //    timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
        //    timerRefresh.Start();
        }

        public DashboardApprovalPR()
        {
        //    InitializeComponent();
        }

        private void DashPR_Load(object sender, EventArgs e)
        {
        //    addCmbCrit();
        //    ModeLoad();
        //    this.Location = new Point(148, 47);
        //    //setTimer();
        }

        public void RefreshGrid()
        {

        //    if (Login.UserGroup == "SalesManager")
        //    {
        //        //Menampilkan data
        //        Conn = ConnectionString.GetConnection();
        //        Query = "with temp as (select row_number() over (partition by reffID order by logdate desc) as rownum, * from WorkflowLogTable)";

        //        if (crit == null)
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, t.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where TransStatus in ('01')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
        //        }
        //        else if (crit.Equals("All"))
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, t.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += "(PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%') and TransStatus in ('01')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
        //        }
        //        else if (crit.Equals("OrderDate"))
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, t.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') and TransStatus in ('01') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
        //        }
        //        else if (crit.Equals("CreatedDate"))
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, Tt.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') and TransStatus in ('01') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
        //        }
        //        else
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, t.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += crit + " Like '%" + txtSearch.Text + "%' and TransStatus in ('01')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
        //        }

        //        Da = new SqlDataAdapter(Query, Conn);
        //        Dt = new DataTable();
        //        Da.Fill(Dt);

        //        dgvPR.AutoGenerateColumns = true;
        //        dgvPR.DataSource = Dt;
        //        dgvPR.Refresh();
        //        dgvPR.AutoResizeColumns();
        //        dgvPR.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
        //        dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
        //        Conn.Close();

        //        //Mengambil nilai total paging
        //        Conn = ConnectionString.GetConnection();

        //        if (crit == null)
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH]) a where a.TransStatus in ('01');";
        //        }
        //        else if (crit.Equals("All"))
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
        //            Query += "(PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%')) a where a.TransStatus in ('01');";
        //        }
        //        else if (crit.Equals("PRDate"))
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
        //            Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%')) a  where a.TransStatus in ('01');";
        //        }
        //        else if (crit.Equals("CreatedDate"))
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
        //            Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%')) a  where a.TransStatus in ('01');";
        //        }
        //        else
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
        //            Query += crit + " Like '%" + txtSearch.Text + "%') a where a.TransStatus in ('01');";
        //        }

        //        Cmd = new SqlCommand(Query, Conn);
        //        Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
        //        Conn.Close();

        //        lblTotal.Text = "Total Rows : " + Total.ToString();
        //        Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
        //        lblPage.Text = "/ " + Page2;
        //    }
        //    else if (Login.UserGroup == "PurchaseManager")
        //    {
        //        //Menampilkan data
        //        Conn = ConnectionString.GetConnection();
        //        Query = "with temp as (select row_number() over (partition by reffID order by logdate desc) as rownum, * from WorkflowLogTable)";

        //        if (crit == null)
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, t.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += "h.TransStatus = '03' or h.TransStatus = '04' or h.TransStatus = '12' or h.TransStatus = '13' or h.TransStatus = '14' or h.TransStatus = '15' or h.TransStatus = '21') a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
        //        }
        //        else if (crit.Equals("All"))
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, t.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += "(PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%') And (h.TransStatus = '03' or h.TransStatus = '04' or h.TransStatus = '12' or h.TransStatus = '13' or h.TransStatus = '14' or h.TransStatus = '15' or h.TransStatus = '21')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
        //        }
        //        else if (crit.Equals("OrderDate"))
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, t.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%') And (h.TransStatus = '03' or h.TransStatus = '04' or h.TransStatus = '12' or h.TransStatus = '13' or h.TransStatus = '14' or h.TransStatus = '15' or h.TransStatus = '21')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
        //        }
        //        else if (crit.Equals("CreatedDate"))
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, Tt.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%') And (h.TransStatus = '03' or h.TransStatus = '04' or h.TransStatus = '12' or h.TransStatus = '13' or h.TransStatus = '14' or h.TransStatus = '15' or h.TransStatus = '21')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
        //        }
        //        else
        //        {
        //            Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, t.[Deskripsi], b.LogDate[Updated Date], CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
        //            Query += crit + " Like '%" + txtSearch.Text + "%' And (h.TransStatus = '03' or h.TransStatus = '04' or h.TransStatus = '12' or h.TransStatus = '13' or h.TransStatus = '14' or h.TransStatus = '15' or h.TransStatus = '21')) a ";
        //            Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
        //        }

        //        Da = new SqlDataAdapter(Query, Conn);
        //        Dt = new DataTable();
        //        Da.Fill(Dt);

        //        dgvPR.AutoGenerateColumns = true;
        //        dgvPR.DataSource = Dt;
        //        dgvPR.Refresh();
        //        dgvPR.AutoResizeColumns();
        //        dgvPR.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
        //        dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
        //        Conn.Close();

        //        //Mengambil nilai total paging
        //        Conn = ConnectionString.GetConnection();

        //        if (crit == null)
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where TransStatus = '03' or TransStatus = '04' or TransStatus = '12' or TransStatus = '13' or TransStatus = '14' or TransStatus = '15' or TransStatus = '21') a ;";
        //        }
        //        else if (crit.Equals("All"))
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
        //            Query += "(PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%') And (TransStatus = '03' or TransStatus = '04' or TransStatus = '12' or TransStatus = '13' or TransStatus = '14' or TransStatus = '15' or TransStatus = '21')) a;";
        //        }
        //        else if (crit.Equals("PRDate"))
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
        //            Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%') And (TransStatus = '03' or TransStatus = '04' or TransStatus = '12' or TransStatus = '13' or TransStatus = '14' or TransStatus = '15' or TransStatus = '21')) a;";
        //        }
        //        else if (crit.Equals("CreatedDate"))
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
        //            Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%') And (TransStatus = '03' or TransStatus = '04' or TransStatus = '12' or TransStatus = '13' or TransStatus = '14' or TransStatus = '15' or TransStatus = '21')) a;";
        //        }
        //        else
        //        {
        //            Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
        //            Query += crit + " Like '%" + txtSearch.Text + "%' And (TransStatus = '03' or TransStatus = '04' or TransStatus = '12' or TransStatus = '13' or TransStatus = '14' or TransStatus = '15' or TransStatus = '21')) a;";
        //        }

        //        Cmd = new SqlCommand(Query, Conn);
        //        Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
        //        Conn.Close();

        //        lblTotal.Text = "Total Rows : " + Total.ToString();
        //        Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
        //        lblPage.Text = "/ " + Page2;
        //    }
        //    Parent.RefreshDocumentApproval();
        }

        private void addCmbCrit()
        {
        //    cmbCriteria.Items.Add("All");
        //    Conn = ConnectionString.GetConnection();
        //    Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchRequisitionH'";

        //    Cmd = new SqlCommand(Query, Conn);
        //    Dr = Cmd.ExecuteReader();

        //    while (Dr.Read())
        //    {
        //        cmbCriteria.Items.Add(Dr[0]);
        //    }
        //    cmbCriteria.SelectedIndex = 0;
        //    Conn.Close();
        }

        private void ModeLoad()
        {
        //    cmbShowLoad();
        //    Limit1 = 1;
        //    Limit2 = Int32.Parse(cmbShow.Text);
        //    Page1 = 1;
        //    txtPage.Text = "1";

        //    dtFrom.Value = DateTime.Today.Date;
        //    dtTo.Value = DateTime.Today.Date;

        //    RefreshGrid();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
        //    txtPage.Text = "1";
        //    Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
        //    Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
        //    RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
        //    if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
        //    {
        //        Limit1 -= Int32.Parse(cmbShow.Text);
        //        Limit2 -= Int32.Parse(cmbShow.Text);
        //        txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
        //    }
        //    RefreshGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
        //    if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
        //    {
        //        Limit1 += Int32.Parse(cmbShow.Text);
        //        Limit2 += Int32.Parse(cmbShow.Text);
        //        txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
        //    }

        //    RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
        //    txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
        //    Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
        //    Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
        //    RefreshGrid();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
        //    if (e.KeyChar == (char)13)
        //    {
        //        Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
        //        Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
        //        RefreshGrid();
        //    }
        }

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
        //    if (e.KeyChar == (char)13)
        //    {
        //        Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
        //        Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
        //        txtPage.Text = "1";
        //        RefreshGrid();

        //    }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    txtPage.Text = "1";
        //    Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
        //    Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
        //    RefreshGrid();
        }

        private void cmbShowLoad()
        {
        //    Conn = ConnectionString.GetConnection();
        //    Query = "Select CmbValue From [Setting].[CmbBox] ";

        //    Cmd = new SqlCommand(Query, Conn);
        //    Dr = Cmd.ExecuteReader();
        //    cmbShow.Items.Clear();
        //    while (Dr.Read())
        //    {
        //        cmbShow.Items.Add(Dr.GetInt32(0));
        //    }
        //    Conn.Close();

        //    Conn = ConnectionString.GetConnection();
        //    SqlCommand Cmd1 = new SqlCommand(Query, Conn);
        //    Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
        //    Conn.Close();

        //    cmbShow.SelectedIndex = 0;
        }

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        //    if (e.RowIndex > -1)
        //    {
        //        PopUp.DashboardApprovalPR.DashboardHeaderPRApproval header = new PopUp.DashboardApprovalPR.DashboardHeaderPRApproval();
        //        header.flag(dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString());
        //        header.setParent(this);
        //        header.Show();
        //        RefreshGrid();
        //    }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
        //    //Simpen HeaderId
        //    if (dgvPR.RowCount > 0)
        //    {
        //        PopUp.DashboardApprovalPR.DashboardHeaderPRApproval header  = new PopUp.DashboardApprovalPR.DashboardHeaderPRApproval();
        //        header.flag(dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString());
        //        header.setParent(this);
        //        header.Show();
        //        RefreshGrid();
        //    }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
        //    this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
        //    if (cmbCriteria.SelectedIndex == -1)
        //    {
        //        crit = "All";
        //    }
        //    else
        //    {
        //        crit = cmbCriteria.SelectedItem.ToString();
        //    }
        //    RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
        //    crit = null;
        //    txtSearch.Text = "";
        //    ModeLoad();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
        //    if (timerRefresh == null)
        //    {

        //    }
        //    else
        //    {
        //        RefreshGrid();
        //    }
        }

        private void DashPR_FormClosed(object sender, FormClosedEventArgs e)
        {
        //    timerRefresh = null;
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    if (cmbCriteria.Text.Contains("Date"))
        //    {
        //        dtFrom.Enabled = true;
        //        dtTo.Enabled = true;
        //    }
        //    else
        //    {
        //        dtFrom.Enabled = false;
        //        dtTo.Enabled = false;
        //    }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
        //    if (e.KeyChar == (char)13)
        //    {
        //        if (txtSearch.Text == null || txtSearch.Text.Equals(""))
        //        {
        //            MessageBox.Show("Masukkan Kata Kunci");
        //        }
        //        else if (cmbCriteria.SelectedIndex == -1)
        //        {
        //            crit = "All";
        //        }
        //        else
        //        {
        //            crit = cmbCriteria.SelectedItem.ToString();
        //        }
        //        RefreshGrid();
        //    }
        }

        private void btnGenerateRFQ_Click(object sender, EventArgs e)
        {
        //    //String Deskripsi = dgvPR.CurrentRow.Cells["Deskripsi"].Value.ToString();
        //    //String PurchReqId = dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString();
        //    //String Type = dgvPR.CurrentRow.Cells["TransType"].Value.ToString();

        //    //if (Deskripsi.Equals("Request approved by Purchase Manager – Partial") || Deskripsi.Equals("Request approved by Purchase Manager - ALL") || Deskripsi.Equals("Quotation on Progress"))
        //    //{
        //    //    RFQ.RFQForm F = new RFQ.RFQForm();
        //    //    F.flag2(PurchReqId, Type, "Generate");
        //    //    F.setParent(this);
        //    //    F.Show();
        //    //    RefreshGrid();
        //    //}
        //    //else
        //    //{
        //    //    MessageBox.Show("Must Approved By Purchase Manager");
        //    //}

        //    String PurchReqId = dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString();
        //    String Type = dgvPR.CurrentRow.Cells["TransType"].Value.ToString();
        //    string Check = "";
        //    Conn = ConnectionString.GetConnection();

        //    Query = "Select TransStatus from [dbo].[PurchRequisitionH] where [PurchReqID]='" + PurchReqId + "'";
        //    Cmd = new SqlCommand(Query, Conn);
        //    Check = Cmd.ExecuteScalar().ToString();
        //    if (Check == "13" || Check == "14" || Check == "21")
        //    {
        //        Purchase.RFQ.RFQForm F = new Purchase.RFQ.RFQForm();
        //        F.flag2(PurchReqId, Type, "Generate");
        //        //F.setParent(this);
        //        F.Show();
        //        RefreshGrid();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Must Approved By Purchase Manager");
        //    }
        }

        public void SetParent(MainMenu F)
        {
        //    Parent = F;
        }


    }
}
