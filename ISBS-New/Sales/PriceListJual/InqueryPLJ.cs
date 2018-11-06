using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;



namespace ISBS_New.Sales.PriceListJual
{
    public partial class InqueryPLJ : MetroFramework.Forms.MetroForm
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
        private string TransStatus = String.Empty;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public InqueryPLJ()
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
            if (timerRefresh == null)
            {

            }
            else
            {
           //    RefreshGrid();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void InquiryPLJ_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            addCmbCrit();
            ModeLoad();
            lblForm.Location = new Point(16, 11);
            //setTimer();
            gvheader();

            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void InquiryPLJ_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void InquiryPLJ_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
            //for (int i = 0; i < ListHeaderPR.Count(); i++)
            //{
            //    ListHeaderPR[i].Close();
            //}
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
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

            cmbCriteria.SelectedIndex = 0;
            cmbStatusCode.SelectedIndex = 0;


            cmbStatusCode.Enabled = false;

            RefreshGrid();
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='SalesPriceList'";

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
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PriceListJualH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void gvheader()
        {
           // dgvPLJ.Columns["PriceListNo"].HeaderText = "Price List No";
            
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

        }

        private void cmbCriteria_SelectedIndexChanged_1(object sender, EventArgs e)
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

        private void btnNew_Click(object sender, EventArgs e)
        {
           //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {                
                HeaderPLJ HeaderPLJ = new HeaderPLJ();
                HeaderPLJ.SetMode("New", "");
                HeaderPLJ.SetParent(this);
                HeaderPLJ.Show();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end

         
        }
       
        private void btnExit_Click_1(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        public void RefreshGrid()
        {

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
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01', '04', '05'"; Limit1 = 1; Limit2 = dataShow;
                }

                Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Customer' WHEN a.Criteria = '2' THEN 'All Customer Except' WHEN a.Criteria = '3' THEN 'Specific Customer' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[SalesPriceListH] Where StatusCode IN (" + TransStatus + ")) a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='SalesPriceList' ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Customer' WHEN a.Criteria = '2' THEN 'All Customer Except' WHEN a.Criteria = '3' THEN 'Specific Customer' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[SalesPriceListH] where ";
                Query += "PriceListNo like '%" + txtSearch.Text + "%' And StatusCode IN (" + TransStatus + ")) a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='SalesPriceList' ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("PriceListNo"))
            {
                Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Customer' WHEN a.Criteria = '2' THEN 'All Customer Except' WHEN a.Criteria = '3' THEN 'Specific Customer' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[SalesPriceListH] where ";
                Query += "PriceListNo like '%" + txtSearch.Text + "%' And StatusCode IN (" + TransStatus + ")) a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='SalesPriceList' ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("StatusName"))
            {
                Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Customer' WHEN a.Criteria = '2' THEN 'All Customer Except' WHEN a.Criteria = '3' THEN 'Specific Customer' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[SalesPriceListH] Where StatusCode IN (" + TransStatus + ")) a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='SalesPriceList' ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " AND b.StatusCode like '%" + cmbSelected + "%' ;";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Customer' WHEN a.Criteria = '2' THEN 'All Customer Except' WHEN a.Criteria = '3' THEN 'Specific Customer' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[SalesPriceListH] where ";
                Query += "StatusCode IN (" + TransStatus + ") And (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='SalesPriceList' ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select a.[No], a.PriceListNo, a.ValidFrom, a.ValidTo, CASE WHEN a.Criteria = '1' THEN 'All Customer' WHEN a.Criteria = '2' THEN 'All Customer Except' WHEN a.Criteria = '3' THEN 'Specific Customer' ELSE '' END AS CriteriaName, a.Criteria, a.StatusCode, UPPER(b.Deskripsi) StatusName, a.CreatedDate, a.CreatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY CreatedDate desc) [No], PriceListNo, ValidFrom, ValidTo, StatusCode, Criteria, CreatedDate, CreatedBy From [dbo].[SalesPriceListH] where ";
                Query += crit + " Like '%" + txtSearch.Text + "%' And StatusCode IN (" + TransStatus + ")) a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='SalesPriceList' ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPLJ.AutoGenerateColumns = true;
            dgvPLJ.DataSource = Dt;
            dgvPLJ.Refresh();
            dgvPLJ.AutoResizeColumns();
            dgvPLJ.Columns["ValidFrom"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvPLJ.Columns["ValidTo"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvPLJ.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvPLJ.Columns["Criteria"].Visible = false;
            dgvPLJ.Columns["StatusCode"].Visible = false;
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(PriceListNo) From SalesPriceListH Where StatusCode IN (" + TransStatus + ")";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(PriceListNo) From SalesPriceListH ";
                Query += "WHERE PriceListNo like '%" + txtSearch.Text + "%' And StatusCode IN (" + TransStatus + ")";
            }
            else if (crit.Equals("PriceListNo"))
            {
                Query = "Select Count(PriceListNo) From SalesPriceListH ";
                Query += "WHERE PriceListNo like '%" + txtSearch.Text + "%' And StatusCode IN (" + TransStatus + ")";
            }
            else if (crit.Equals("StatusName"))
            {
                Query = "Select Count(PriceListNo) From SalesPriceListH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.StatusCode and TransCode='PurchPriceList' ";
                Query += "WHERE b.StatusCode like '%" + cmbSelected + "%' And a.StatusCode IN (" + TransStatus + ");";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "Select Count(PriceListNo) From SalesPriceListH ";
                Query += "Where StatusCode IN (" + TransStatus + ") And (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')";
            }
            else
            {
                Query = "Select Count(PriceListNo) From SalesPriceListH ";
                Query += crit + " Like '%" + txtSearch.Text + "%' And StatusCode IN (" + TransStatus + ")";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

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
                    if (dgvPLJ.RowCount > 0)
                    {
                        Index = dgvPLJ.CurrentRow.Index;
                        string PriceListNo = dgvPLJ.Rows[Index].Cells["PriceListNo"].Value == null ? "" : dgvPLJ.Rows[Index].Cells["PriceListNo"].Value.ToString();
                        //string TransStatus = dgvPLJ.Rows[Index].Cells["TransStatus"].Value == null ? "" : dgvPLJ.Rows[Index].Cells["TransStatus"].Value.ToString();

                        if (TransStatus == "'02', '03'")
                        {
                            MessageBox.Show("Data tidak dapat dihapus karena sudah diapprove.");
                            return;
                        }
                        else
                        {
                            DialogResult dr = MessageBox.Show("PriceListNo = " + PriceListNo + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                string Check = "";
                                Conn = ConnectionString.GetConnection();

                                Query = "Select StatusCode from [dbo].[SalesPriceListH] where [PriceListNo]='" + dgvPLJ.CurrentRow.Cells["PriceListNo"].Value.ToString() + "';";
                                Cmd = new SqlCommand(Query, Conn);
                                Check = Cmd.ExecuteScalar().ToString();
                                if (Check != "01")
                                {
                                    MessageBox.Show("PriceListNo = " + dgvPLJ.CurrentRow.Cells["PriceListNo"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                                    Conn.Close();
                                    return;
                                }


                                Conn = ConnectionString.GetConnection();

                                //delete detail
                                Query = "Delete from [dbo].[SalesPriceListDtl] where PriceListNo ='" + PriceListNo + "';";


                                //delete header
                                Query += "Delete from [dbo].[SalesPriceListH] where PriceListNo='" + PriceListNo + "';";


                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                MessageBox.Show("PriceListNo = " + PriceListNo.ToUpper() + "\n" + "Data berhasil dihapus.");

                                Index = 0;
                                Conn.Close();
                                RefreshGrid();

                            }
                        }                        
                    }
                    else
                    {
                        MessageBox.Show("Silahkan pilih data untuk dihapus");
                        return;
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

        private void btnSelect_Click_1(object sender, EventArgs e)
        {
            SelectPLJ();
        }
      
        private void SelectPLJ()
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            HeaderPLJ header = new HeaderPLJ();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPLJ.RowCount > 0)
                {
                    header.SetMode("BeforeEdit", dgvPLJ.CurrentRow.Cells["PriceListNo"].Value.ToString());
                    header.SetParent(this);
                    header.Show();
                    RefreshGrid();
                }
                else
                {
                    MessageBox.Show("Silahkan pilih data");
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void dgvPLJ_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectPLJ();
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01', '04', '05'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'02', '03'";
            RefreshGrid();
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }
    }
}
