using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;


namespace ISBS_New.CashAndBank.BankStatement
{
    public partial class InquiryBankStatement : MetroFramework.Forms.MetroForm
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
        private string StatusCode = String.Empty;

        public InquiryBankStatement()
        {
            InitializeComponent();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void InquiryBankStatement_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void InquiryBankStatement_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void addCmbCrit()
        {
            try
            {
                cmbCriteria.Items.Add("All");
                Conn = ConnectionString.GetConnection();
                Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'ImportBankStatementH'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    cmbCriteria.Items.Add(Dr[0]);
                }
                Conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
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

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            StatusCode = "'01', '02'";
            RefreshGrid();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            StatusCode = "'03'";
            RefreshGrid();
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
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
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }

            RefreshGrid();
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

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
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


            if (cmbCriteria.Text.Contains("Date"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                if (StatusCode == String.Empty)
                {
                    StatusCode = "'01', '02'"; Limit1 = 1; Limit2 = dataShow;
                }

                Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY B.CreatedDate DESC) No, B.BS_Id, B.Account_No, B.Account_Name, B.Bank_Code AS Bank_Id, B.CreatedDate, B.CreatedBy ";
                Query += "FROM ImportBankStatementH B INNER JOIN TransStatusTable T on B.Status_Code = T.StatusCode ";
                Query += "AND T.TransCode='BankStatement' AND B.Status_Code IN (" + StatusCode + ")) a ";
                Query += "WHERE No Between " + Limit1 + " AND " + Limit2 + " ";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY B.CreatedDate DESC) No, B.BS_Id, B.Account_No, B.Account_Name, B.Bank_Code AS Bank_Id, B.CreatedDate, B.CreatedBy ";
                Query += "FROM ImportBankStatementH B INNER JOIN TransStatusTable T on B.Status_Code = T.StatusCode ";
                Query += "AND T.TransCode='BankStatement' AND B.Status_Code IN (" + StatusCode + ") ";
                Query += "AND (BS_Id LIKE @search OR Account_No LIKE @search OR Account_Name LIKE @search OR Bank_Code LIKE @search))a ";
                Query += "WHERE No Between " + Limit1 + " AND " + Limit2 + " ";
            }
            else if (crit.Equals("BS_Id"))
            {
                Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY B.CreatedDate DESC) No, B.BS_Id, B.Account_No, B.Account_Name, B.Bank_Code AS Bank_Id, B.CreatedDate, B.CreatedBy ";
                Query += "FROM ImportBankStatementH B INNER JOIN TransStatusTable T on B.Status_Code = T.StatusCode ";
                Query += "AND T.TransCode='BankStatement' AND B.Status_Code IN (" + StatusCode + ") ";
                Query += "AND BS_Id LIKE @search)a ";
                Query += "WHERE No Between " + Limit1 + " AND " + Limit2 + " ";
            }
            else if (crit.Equals("Account_No"))
            {
                Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY B.CreatedDate DESC) No, B.BS_Id, B.Account_No, B.Account_Name, B.Bank_Code AS Bank_Id, B.CreatedDate, B.CreatedBy ";
                Query += "FROM ImportBankStatementH B INNER JOIN TransStatusTable T on B.Status_Code = T.StatusCode ";
                Query += "AND T.TransCode='BankStatement' AND B.Status_Code IN (" + StatusCode + ") ";
                Query += "AND Account_No LIKE @search)a ";
                Query += "WHERE No Between " + Limit1 + " AND " + Limit2 + " ";
            }
            else if (crit.Equals("Account_Name"))
            {
                Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY B.CreatedDate DESC) No, B.BS_Id, B.Account_No, B.Account_Name, B.Bank_Code AS Bank_Id, B.CreatedDate, B.CreatedBy ";
                Query += "FROM ImportBankStatementH B INNER JOIN TransStatusTable T on B.Status_Code = T.StatusCode ";
                Query += "AND T.TransCode='BankStatement' AND B.Status_Code IN (" + StatusCode + ") ";
                Query += "AND Account_Name LIKE @search)a ";
                Query += "WHERE No Between " + Limit1 + " AND " + Limit2 + " ";
            }
            else if (crit.Equals("Bank_Id"))
            {
                Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY B.CreatedDate DESC) No, B.BS_Id, B.Account_No, B.Account_Name, B.Bank_Code AS Bank_Id, B.CreatedDate, B.CreatedBy ";
                Query += "FROM ImportBankStatementH B INNER JOIN TransStatusTable T on B.Status_Code = T.StatusCode ";
                Query += "AND T.TransCode='BankStatement' AND B.Status_Code IN (" + StatusCode + ") ";
                Query += "AND Bank_Code LIKE @search)a ";
                Query += "WHERE No Between " + Limit1 + " AND " + Limit2 + " ";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY B.CreatedDate DESC) No, B.BS_Id, B.Account_No, B.Account_Name, B.Bank_Code AS Bank_Id, B.CreatedDate, B.CreatedBy ";
                Query += "FROM ImportBankStatementH B INNER JOIN TransStatusTable T on B.Status_Code = T.StatusCode ";
                Query += "AND T.TransCode='BankStatement' AND B.Status_Code IN (" + StatusCode + ") ";
                Query += "AND (CONVERT(VARCHAR(10),B.CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' AND CONVERT(VARCHAR(10), B.CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
                Query += "WHERE No Between " + Limit1 + " AND " + Limit2 + " ";
            }
            else if (crit.Equals("CreatedBy"))
            {
                Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY B.CreatedDate DESC) No, B.BS_Id, B.Account_No, B.Account_Name, B.Bank_Code AS Bank_Id, B.CreatedDate, B.CreatedBy ";
                Query += "FROM ImportBankStatementH B INNER JOIN TransStatusTable T on B.Status_Code = T.StatusCode ";
                Query += "AND T.TransCode='BankStatement' AND B.Status_Code IN (" + StatusCode + ") ";
                Query += "AND CreatedBy LIKE @search)a ";
                Query += "WHERE No Between " + Limit1 + " AND " + Limit2 + " ";
            }

            Query += " ORDER BY a.CreatedDate DESC";

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvBankStatement.AutoGenerateColumns = true;
            dgvBankStatement.DataSource = Dt;
            dgvBankStatement.Refresh();
            dgvBankStatement.AutoResizeColumns();
            dgvBankStatement.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";

            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementH WHERE Status_Code IN (" + StatusCode + ")";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementH WHERE Status_Code IN (" + StatusCode + ") ";
                Query += "AND (BS_Id LIKE @search OR Account_No LIKE @search OR Account_Name LIKE @search OR Bank_Code LIKE @search)";
            }
            else if (crit.Equals("BS_Id"))
            {
                Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementH WHERE Status_Code IN (" + StatusCode + ") ";
                Query += "AND BS_Id LIKE @search";
            }
            else if (crit.Equals("Account_No"))
            {
                Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementH WHERE Status_Code IN (" + StatusCode + ") ";
                Query += "AND Account_No LIKE @search";         
            }
            else if (crit.Equals("Account_Name"))
            {
                Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementH WHERE Status_Code IN (" + StatusCode + ") ";
                Query += "AND Account_Name LIKE @search";
            }
            else if (crit.Equals("Bank_Id"))
            {
                Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementH WHERE Status_Code IN (" + StatusCode + ") ";
                Query += "AND Bank_Code LIKE @search";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementH WHERE Status_Code IN (" + StatusCode + ") ";
                Query += "AND (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' AND CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
            }
            else if (crit.Equals("CreatedBy"))
            {
                Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementH WHERE Status_Code IN (" + StatusCode + ") ";
                Query += "AND CreatedBy LIKE @search";
            } 

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
            ModeLoad();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                HeaderBankStatement H = new HeaderBankStatement();
                H.SetMode("New", "", "");
                H.SetParent(this);
                H.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectData();
        }

        private void SelectData()
        {
            HeaderBankStatement header = new HeaderBankStatement();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvBankStatement.RowCount > 0)
                {
                    header.SetParent(this);
                    header.SetMode("BeforeEdit", dgvBankStatement.CurrentRow.Cells["BS_Id"].Value.ToString(), dgvBankStatement.CurrentRow.Cells["Bank_Id"].Value.ToString());
                    header.Show();
                }
                else
                {
                    MessageBox.Show("Silahkan Pilih Data.");
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void dgvBankStatement_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectData();
        }
      
    }
}
