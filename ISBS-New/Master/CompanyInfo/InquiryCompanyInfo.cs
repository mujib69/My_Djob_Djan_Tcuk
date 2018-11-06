using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.CompanyInfo
{
    public partial class InquiryCompanyInfo : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
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
            timerRefresh.Interval = (10*1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        public InquiryCompanyInfo()
        {
            InitializeComponent();
        }

        private void DashPR_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
            //setTimer();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (cmbCriteria.Text == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CompanyId) No, CompanyId, CompanyName, CompanyAddress, CompanyPhone, CompanyFax, NPWP, TaxName, TaxAddress, SignatoryName From [dbo].[CompanyInfo] ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text =="All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CompanyId) No, CompanyId, CompanyName, CompanyAddress, CompanyPhone, CompanyFax, NPWP, TaxName, TaxAddress, SignatoryName From [dbo].[CompanyInfo] Where ";
                Query += "CompanyId like '%" + txtSearch.Text + "%' or CompanyName like '%" + txtSearch.Text + "%' or CompanyAddress like '%" + txtSearch.Text + "%' or CompanyPhone like '%" + txtSearch.Text + "%' or CompanyFax like '%" + txtSearch.Text + "%' or NPWP like '%" + txtSearch.Text + "%' or TaxName like '%" + txtSearch.Text + "%' or TaxAddress like '%" + txtSearch.Text + "%' or SignatoryName like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CompanyId) No, CompanyId, CompanyName, CompanyAddress, CompanyPhone, CompanyFax, NPWP, TaxName, TaxAddress, SignatoryName From [dbo].[CompanyInfo] Where ";
                Query += cmbCriteria.Text + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvConfigItem.AutoGenerateColumns = true;
            dgvConfigItem.DataSource = Dt;
            dgvConfigItem.Refresh();
            dgvConfigItem.AutoResizeColumns();
            //dgvCompanyInfo.Columns["GroupId"].Visible = false;
            //dgvCompanyInfo.Columns["SubGroup1Id"].Visible = false;
            //dgvCompanyInfo.Columns["SubGroup2Id"].Visible = false;
            //dgvCompanyInfo.Columns["CompanyInfoId"].Visible = false;
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(CompanyId) From (Select ROW_NUMBER() OVER (ORDER BY CompanyId) No, CompanyId, CompanyName, CompanyAddress, CompanyPhone, CompanyFax, NPWP, TaxName, TaxAddress, SignatoryName From [dbo].[CompanyInfo] ) a ;";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(CompanyId) From (Select ROW_NUMBER() OVER (ORDER BY CompanyId) No, CompanyId, CompanyName, CompanyAddress, CompanyPhone, CompanyFax, NPWP, TaxName, TaxAddress, SignatoryName From [dbo].[CompanyInfo] Where ";
                Query += "CompanyId like '%" + txtSearch.Text + "%' or CompanyName like '%" + txtSearch.Text + "%' or CompanyAddress like '%" + txtSearch.Text + "%' or CompanyPhone like '%" + txtSearch.Text + "%' or CompanyFax like '%" + txtSearch.Text + "%' or NPWP like '%" + txtSearch.Text + "%' or TaxName like '%" + txtSearch.Text + "%' or TaxAddress like '%" + txtSearch.Text + "%' or SignatoryName like '%" + txtSearch.Text + "%') a ";
            }
            else
            {
                Query = "Select Count(CompanyId) From (Select ROW_NUMBER() OVER (ORDER BY CompanyId) No, CompanyId, CompanyName, CompanyAddress, CompanyPhone, CompanyFax, NPWP, TaxName, TaxAddress, SignatoryName From [dbo].[CompanyInfo] Where ";
                Query += cmbCriteria.Text + " Like '%" + txtSearch.Text + "%') a ";
            }
            
            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'CompanyInfo'";

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

        private void btnNew_Click(object sender, EventArgs e)
        { 
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                FormCompanyInfo CompanyInfo = new FormCompanyInfo();
                //CompanyInfo.SetParent(this);
                CompanyInfo.ModeNew();
                CompanyInfo.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                FormCompanyInfo CompanyInfo = new FormCompanyInfo();
                //CompanyInfo.SetParent(this);
                CompanyInfo.GetDataHeader(dgvConfigItem.CurrentRow.Cells["CompanyId"].Value.ToString());
                CompanyInfo.ModeBeforeEdit();
                CompanyInfo.Show();
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvConfigItem.RowCount > 0)
                    {
                        Index = dgvConfigItem.CurrentRow.Index;
                        string CompanyId = dgvConfigItem.Rows[Index].Cells["CompanyId"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["CompanyId"].Value.ToString();
                        string No = dgvConfigItem.Rows[Index].Cells["No"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["No"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        DialogResult dr = MessageBox.Show("CompanyId = " + CompanyId + "\n" + Environment.NewLine + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            //delete header
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[CompanyInfo] where CompanyId='" + CompanyId + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("No = " + No + "\n" + "Data berhasil dihapus.");

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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            FormCompanyInfo CompanyInfo = new FormCompanyInfo();

            if (CompanyInfo.PermissionAccess(ControlMgr.View) > 0)
            {
                //Simpen HeaderId
                if (dgvConfigItem.RowCount > 0)
                {
                    //CompanyInfo.SetParent(this);
                    CompanyInfo.GetDataHeader(dgvConfigItem.CurrentRow.Cells["CompanyId"].Value.ToString());
                    CompanyInfo.ModeBeforeEdit();
                    CompanyInfo.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            
            txtSearch.Text = "";
            ModeLoad();
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

        private void DashPR_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
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

        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                RefreshGrid();
            }
        }

    }
}
