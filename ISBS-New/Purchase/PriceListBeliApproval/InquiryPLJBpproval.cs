using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Purchase.PriceListBeliApproval
{
    public partial class InquiryPLBApproval : MetroFramework.Forms.MetroForm
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

        MainMenu Parent = new MainMenu();

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end


        public InquiryPLBApproval()
        {
            InitializeComponent();
           
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
            if (timerRefresh != null)
            {
            //    RefreshGrid();
            }
        }

        private void InquiryPLBApproval_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            addCmbCrit();
            ModeLoad();
            //lblForm.Location = new Point(16, 11);
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='PurchPriceList'";

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

        private void InquiryPLBApproval_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void InquiryPLBApproval_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
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

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PriceListBeliH' And UPPER(FieldName) <> 'STATUSNAME' ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                 cmbCriteria.Items.Add(Dr[0]);                            
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectPLBApproval();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            cmbCriteria.SelectedIndex = 0;
            ModeLoad();
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

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
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

        private void SelectPLBApproval()
        {
            if (dgvPLB.RowCount > 0)
            {
                HeaderPLBApproval header = new HeaderPLBApproval();
                header.flag(dgvPLB.CurrentRow.Cells["PriceListNo"].Value.ToString(), dgvPLB.CurrentRow.Cells["StatusCode"].Value.ToString());
                header.SetParent(this);
                header.Show();
                RefreshGrid();

            }
            else
            {
                MessageBox.Show("Silahkan pilih data");
            }
        }

        public void RefreshGrid()
        {
           // if (ControlMgr.GroupName == "SalesManager")
           // if (ControlMgr.GroupName == "")
           // {
                string cmbSelected = Convert.ToString(cmbStatusCode.SelectedIndex);

                //string cmbs = cmbStatusCode.Va;

                if (cmbSelected == "1")
                {
                    cmbSelected = "01";
                }
                else if (cmbSelected == "2")
                {
                    cmbSelected = "02";
                }
                else if (cmbSelected == "3")
                {
                    cmbSelected = "03";
                }
                else if (cmbSelected == "4")
                {
                    cmbSelected = "04";
                }
                else if (cmbSelected == "5")
                {
                    cmbSelected = "05";
                }
                else
                {
                    cmbSelected = "";
                }

                //Menampilkan data
                Conn = ConnectionString.GetConnection();
                if (crit == null)
                {
                    Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Vendor' WHEN a.Criteria = '2' THEN 'All Vendor Except' WHEN a.Criteria = '3' THEN 'Specific Vendor' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                    Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[PurchPriceListH] WHERE StatusCode IN('01', '05')) a ";
                    Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='PurchPriceList' ";
                    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
                }
                else if (crit.Equals("All"))
                {
                    Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Vendor' WHEN a.Criteria = '2' THEN 'All Vendor Except' WHEN a.Criteria = '3' THEN 'Specific Vendor' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                    Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[PurchPriceListH] ";
                    Query += "Where StatusCode IN('01', '05') AND PriceListNo like '%" + txtSearch.Text + "%' ) a ";
                    Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='PurchPriceList' ";
                    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
                }
                else if (crit.Equals("PriceListNo"))
                {
                    Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Vendor' WHEN a.Criteria = '2' THEN 'All Vendor Except' WHEN a.Criteria = '3' THEN 'Specific Vendor' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                    Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[PurchPriceListH] ";
                    Query += "Where StatusCode IN('01', '05') AND PriceListNo like '%" + txtSearch.Text + "%' ) a ";
                    Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='PurchPriceList' ";
                    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
                }
                //else if (crit.Equals("StatusName"))
                //{
                //    Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Vendor' WHEN a.Criteria = '2' THEN 'All Vendor Except' WHEN a.Criteria = '3' THEN 'Specific Vendor' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                //    Query += "From (Select ROW_NUMBER() OVER (ORDER BY PriceListNo desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[PurchPriceListH] WHERE StatusCode IN('01', '05')) a ";
                //    Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='PurchPriceList' ";
                //    Query += "Where No Between " + Limit1 + " and " + Limit2 + " AND b.StatusCode like '%" + cmbSelected + "%' ;";
                //}
                else if (crit.Equals("CreatedDate"))
                {
                    Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Vendor' WHEN a.Criteria = '2' THEN 'All Vendor Except' WHEN a.Criteria = '3' THEN 'Specific Vendor' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                    Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[PurchPriceListH] ";
                    Query += "Where StatusCode IN('01', '05') AND (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
                    Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='PurchPriceList' ";
                    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
                }
                else
                {
                    Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Vendor' WHEN a.Criteria = '2' THEN 'All Vendor Except' WHEN a.Criteria = '3' THEN 'Specific Vendor' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                    Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[PurchPriceListH] ";
                    Query += "Where StatusCode IN('01', '05') AND " + crit + " Like '%" + txtSearch.Text + "%' ) a ";
                    Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='PurchPriceList' ";
                    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
                }

                Da = new SqlDataAdapter(Query, Conn);
                Dt = new DataTable();
                Da.Fill(Dt);

                dgvPLB.AutoGenerateColumns = true;
                dgvPLB.DataSource = Dt;
                dgvPLB.Refresh();
                dgvPLB.AutoResizeColumns();

                dgvPLB.Columns["ValidFrom"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                dgvPLB.Columns["ValidTo"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                dgvPLB.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                dgvPLB.Columns["Criteria"].Visible = false;
                dgvPLB.Columns["StatusCode"].Visible = false;
                Conn.Close();

               // Query += " AND a.StatusCode IN ('01', '05')";

                Da = new SqlDataAdapter(Query, Conn);
                Dt = new DataTable();
                Da.Fill(Dt);

                dgvPLB.AutoGenerateColumns = true;
                dgvPLB.DataSource = Dt;
                dgvPLB.Refresh();
                dgvPLB.AutoResizeColumns();
                dgvPLB.Columns["ValidFrom"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                dgvPLB.Columns["ValidTo"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                Conn.Close();

                //Mengambil nilai total paging
                Conn = ConnectionString.GetConnection();

                if (crit == null)
                {
                    Query = "Select Count(PriceListNo) From PurchPriceListH WHERE StatusCode IN('01', '05')";
                }
                else if (crit.Equals("All"))
                {
                    Query = "Select Count(PriceListNo) From PurchPriceListH ";
                    Query += "WHERE StatusCode IN('01', '05') AND PriceListNo like '%" + txtSearch.Text + "%'";
                }
                else if (crit.Equals("PriceListNo"))
                {
                    Query = "Select Count(PriceListNo) From PurchPriceListH ";
                    Query += "WHERE StatusCode IN('01', '05') AND PriceListNo like '%" + txtSearch.Text + "%'";
                }
                //else if (crit.Equals("StatusName"))
                //{
                //    Query = "Select Count(PriceListNo) From PurchPriceListH a ";
                //    Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='PurchPriceList' ";
                //    Query += "WHERE b.StatusCode like '%" + cmbSelected + "%';";
                //}
                else if (crit.Equals("CreatedDate"))
                {
                    Query = "Select Count(PriceListNo) From PurchPriceListH ";
                    Query += "Where StatusCode IN('01', '05') AND (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')";
                }
                else
                {
                    Query = "Select Count(PriceListNo) From PurchPriceListH WHERE StatusCode IN('01', '05') AND ";
                    Query += crit + " Like '%" + txtSearch.Text + "%'";
                }

                Cmd = new SqlCommand(Query, Conn);
                Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
                Conn.Close();

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
                lblPage.Text = "/ " + Page2;
            //}                
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

           // cmbCriteria.SelectedIndex = 0;

            RefreshGrid();
        }

        public void SetParent(MainMenu F)
        {
            Parent = F;
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

            if (cmbCriteria.Text.Contains("StatusName"))
            {
                cmbStatusCode.Enabled = true;
            }
            else
            {
                cmbStatusCode.Enabled = false;
                cmbStatusCode.SelectedIndex = 0;
            }

            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("StatusName"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        private void dgvPLB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectPLBApproval();
        }

    }
}
