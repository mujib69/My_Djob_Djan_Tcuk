using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.AccountPayable
{
    public partial class InquiryAccountsPayable : MetroFramework.Forms.MetroForm
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

        List<HeaderAccountsPayable> ListHeaderAP = new List<HeaderAccountsPayable>();


        public InquiryAccountsPayable()
        {
            InitializeComponent();
        }
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
               
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
           
            this.Close();
        }

        private void InquiryAccountsPayable_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            addCmbCrit();
            ModeLoad();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void InquiryAccountsPayable_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void InquiryAccountsPayable_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);

            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFromDate.Value = DateTime.Today.Date;
            dtToDate.Value = DateTime.Today.Date;

            cmbCriteria.SelectedIndex = 0;
            cmbStatusCode.SelectedIndex = 0;
            cmbStatusCode.Enabled = false;
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

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'AccountsPayableH'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            if (!(cmbCriteria.Items.Contains("Paid Status")))
            {
                cmbCriteria.Items.Add("Paid Status");
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='VendInvoice'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbStatusCode.DisplayMember = "Text";
            cmbStatusCode.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbStatusCode.Items.Add(new {Value = "" + Dr[0] + "", Text = "" + Dr[1] + "" });
               
            }
            cmbStatusCode.SelectedIndex = 0;
            Conn.Close();
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
               dtFromDate.Enabled = true;
                dtToDate.Enabled = true;
            }
            else
            {
                dtFromDate.Enabled = false;
                dtToDate.Enabled = false;
            }

            if (cmbCriteria.Text.Contains("VendName"))
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
            if (this.PermissionAccess(ControlMgr.New)>0)
            {
                if (ControlMgr.GroupName != "AP Admin")
                {
                    MessageBox.Show("Create New hanya bisa dilakukan oleh user AP Admin.");
                    return;
                }
                HeaderAccountsPayable HeaderAP = new HeaderAccountsPayable();
                HeaderAP.SetMode("New", "");
                HeaderAP.SetParent(this);
                HeaderAP.Show();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        public void RefreshGrid()
        {
            if (ControlMgr.GroupName == "Tax Admin")
            {
                Tampil("'01'");
            }
            else if (ControlMgr.GroupName == "AP Manager")
            {
                Tampil("'02'");
            }
            else
            {
                Tampil("'01','02'");
            }
            dgvInquiry.ReadOnly = true;
        }

        private void Tampil(string TransCode)
        {
            string cmbSelected = Convert.ToString(cmbStatusCode.SelectedIndex);
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
            else if (cmbSelected == "6")
            {
                cmbSelected = "06";
            }
            else if (cmbSelected == "7")
            {
                cmbSelected = "07";
            }
            else
            {
                cmbSelected = "";
            }
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = TransCode;
                    Limit1 = 1;
                    Limit2 = dataShow;
                }
                Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "InvoiceId,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "[InvoiceType],VendId, VendName,DueDate, b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += "Where StatusCode IN (" + TransStatus + ")) A ";
            }

            else if (crit.Equals("InvoiceId"))
            {
                Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "InvoiceId,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "[InvoiceType],VendId, VendName,DueDate, b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += "where InvoiceId like @search And TransStatus IN (" + TransStatus + ")) A ";
            }
            else if (crit.Equals("InvoiceType"))
            {
                Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "InvoiceId,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "[InvoiceType],VendId, VendName,DueDate, b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += "where InvoiceType like @search And TransStatus IN (" + TransStatus + ")) A ";
            }
            else if (crit.Equals("VendAccount"))
            {
                Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "InvoiceId,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "[InvoiceType],VendId, VendName,DueDate, b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += "where VendId like @search And TransStatus IN (" + TransStatus + ")) A ";
            }

            else if (crit.Equals("VendName"))
            {
                Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "InvoiceId,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "[InvoiceType],VendId, VendName,DueDate, b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += "where VendName like @search And TransStatus IN (" + TransStatus + ")) A ";
            }

            else if (crit.Equals("CreateDate"))
            {
                Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "InvoiceId,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "[InvoiceType],VendId, VendName,DueDate, b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += "where TransStatus IN (" + TransStatus + ") AND (CONVERT(VARCHAR(10),a.CreatedDate,120) >= '" + dtFromDate.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),a.CreatedDate,120) <= '" + dtToDate.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }

            else if (crit.Equals("All"))
            {
                Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "InvoiceId,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "[InvoiceType],VendId, VendName,DueDate, b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += "where  TransStatus IN (" + TransStatus + ") And (InvoiceID LIKE @search OR VendId LIKE @search OR VendName LIKE @search)) A ";
            }

            else if (crit.Equals("Paid Status"))
            {
                Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "InvoiceId,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "[InvoiceType],VendId, VendName,DueDate, b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += " where TransStatus IN (" + TransStatus + ")) A ";
                
            }

            Query += "Where A.No Between " + Limit1 + " and " + Limit2 + " ";
            if (crit != null && crit.Equals("Paid Status"))
            {
                Query += " AND Paid_Status like @search";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@search","%" + txtSearch.Text + "%"); 
            Dt = new DataTable();
            Da.Fill(Dt);

            //DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            //buttonSend.Name = "Send Email";
            //buttonSend.HeaderText = "Send Email";
            //buttonSend.Text = "Send Email";

            //buttonSend.UseColumnTextForButtonValue = true;
            dgvInquiry.AutoGenerateColumns = true;
            dgvInquiry.DataSource = Dt;

            for (int i = 0; i < dgvInquiry.RowCount; i++)
            {
                if (Convert.ToDateTime(dgvInquiry.Rows[i].Cells["UpdatedDate"].Value) == new DateTime(1753, 1, 1))
                    dgvInquiry.Rows[i].Cells["UpdatedDate"].Value = (object)DBNull.Value;
            }

            //if (!dgvInquiry.Columns.Contains("Send Email"))
                //dgvInquiry.Columns.Add(buttonSend);

            dgvInquiry.Refresh();
            dgvInquiry.AutoResizeColumns();
            dgvInquiry.Columns["InvoiceDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvInquiry.Columns["DueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvInquiry.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm tt";
            dgvInquiry.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm tt";
            Conn.Close();

            //paging
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                Query = "SELECT COUNT(InvoiceId) FROM VendInvoiceH Where TransStatus IN (" + TransStatus + ")";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(InvoiceID) FROM VendInvoiceH WHERE ";
                Query += "(InvoiceID LIKE @search OR VendId LIKE @search OR VendName LIKE @search) And TransStatus IN (" + TransStatus + ")";
            }
            else if (crit.Equals("CreateDate"))
            {
                Query = "SELECT COUNT(InvoiceId) FROM VendInvoiceH WHERE ";
                Query += "(CONVERT(VARCHAR(10), CreatedDate,120) >= '" + dtFromDate.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtToDate.Value.Date.ToString("yyyy-MM-dd") + "') And TransStatus IN (" + TransStatus + ");";
            }
            else if (crit.Equals("Paid Status"))
            {
                Query = "Select Count(*) From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],InvoiceDate,";
                Query += "CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' ";
                Query += "WHEN (Settle_Amount + [Additional_Disc] = [InvoiceAmount]) THEN 'Paid' ";
                Query += "ELSE 'Paid-Outstanding' END Paid_Status, ";
                Query += "InvoiceId, VendId, VendName,DueDate,  b.Deskripsi,TransStatus, CreatedDate,  CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                Query += "From VendInvoiceH a ";
                Query += "Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                Query += " where TransStatus IN (" + TransStatus + ")) A ";
                Query += " WHERE Paid_Status like @search";
            }
            else
            {
                Query = "SELECT COUNT(InvoiceId) FROM VendInvoiceH WHERE ";
                if (crit.Equals("VendAccount"))
                {
                    Query += "VendId";
                }
                else
                {
                    Query += cmbCriteria.Text;
                }
                Query += " LIKE @search And TransStatus IN (" + TransStatus + ")";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.Add("@search", "%" + txtSearch.Text + "%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);

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

        private void SelectAP()
        {
            HeaderAccountsPayable header = new HeaderAccountsPayable();
            if (header.PermissionAccess(ControlMgr.View)>0)
            {
                if (dgvInquiry.RowCount>0)
                {
                    header.SetParent(this);
                    header.SetMode("BeforeEdit", dgvInquiry.CurrentRow.Cells["InvoiceId"].Value.ToString());
                    header.Show();
                    RefreshGrid();
                    header.Hitung();
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectAP();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName == "Tax Admin")
            {
                TransStatus = "'06','07','08','11'";
            }
            else if (ControlMgr.GroupName == "AP Manager")
            {
                TransStatus = "'03','04','05','11'";
            }
            else
            {
                TransStatus = "'03','04','05','06','07', '08','11'";
            }
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;

            RefreshGrid();
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName == "Tax Admin")
            {
                TransStatus = "'01','07'";
            }
            else if (ControlMgr.GroupName == "AP Manager")
            {
                TransStatus = "'02','05'";
            }
            else
            {
                TransStatus = "'01','02','05','07'";
            }
            //TransStatus = "'01', '02', '05', '06', '07'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;

            RefreshGrid();
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
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
            if (Convert.ToInt32(txtPage.Text) > Convert.ToInt32((decimal)Total / Int32.Parse(cmbShow.Text)))
            {
                txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text)) * Int32.Parse(cmbShow.Text);
            }
            if (Convert.ToInt32(txtPage.Text) < 1)
            {
                txtPage.Text = "1";
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text)) * Int32.Parse(cmbShow.Text);
            }
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 2) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text);
                if (Convert.ToInt32(txtPage.Text) > 1)
                {
                    txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
                }
            }
            RefreshGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) > Convert.ToInt32((decimal)Total / Int32.Parse(cmbShow.Text)))
            {
                txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text)) * Int32.Parse(cmbShow.Text);
            }
            if (Convert.ToInt32(txtPage.Text) < 1)
            {
                txtPage.Text = "1";
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text)) * Int32.Parse(cmbShow.Text);
            }
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 = (Int32.Parse(txtPage.Text)) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text) + 1) * Int32.Parse(cmbShow.Text);
                if (Convert.ToInt32(txtPage.Text) < Convert.ToInt32((decimal)Total / Int32.Parse(cmbShow.Text)))
                {
                    txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
                }
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

        private void dgvInquiry_DoubleClick(object sender, EventArgs e)
        {
            SelectAP();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.'))
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

        private void cmbShow_TextChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (dgvInquiry.CurrentCell.ColumnIndex > -1 && dgvInquiry.CurrentCell.RowIndex > -1)
            {
                string APId = dgvInquiry.Rows[dgvInquiry.CurrentCell.RowIndex].Cells["InvoiceId"].Value == null ? "" : dgvInquiry.Rows[dgvInquiry.CurrentCell.RowIndex].Cells["InvoiceId"].Value.ToString();

                GlobalPreview f = new GlobalPreview("Account Payable", APId);
                f.Show();
            }
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            if (dgvInquiry.CurrentCell.ColumnIndex > -1 && dgvInquiry.CurrentCell.RowIndex > -1)
            {
                
                try
                {
                    //SendEmailSQ s = new SendEmailSQ(this);
                    //s.flag(SQId); //,TransType);
                    //s.Show();
                    string SQId = dgvInquiry.Rows[dgvInquiry.CurrentCell.RowIndex].Cells["InvoiceId"].Value == null ? "" : dgvInquiry.Rows[dgvInquiry.CurrentCell.RowIndex].Cells["InvoiceId"].Value.ToString();

                    string CustID = dgvInquiry.Rows[dgvInquiry.CurrentCell.RowIndex].Cells["VendId"].Value == null ? "" : dgvInquiry.Rows[dgvInquiry.CurrentCell.RowIndex].Cells["VendId"].Value.ToString();

                    GlobalSendEmail f = new GlobalSendEmail("InvoicePayment", SQId, CustID);
                    f.Show();
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MetroFramework.MetroMessageBox.Show(this, "Error message : " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //else if (dataGridView1.Rows[e.RowIndex].Cells["TransStatus Deskripsi"].Value.ToString() == "Sent")
                //    MetroFramework.MetroMessageBox.Show(this, "Cannot send email more than once to " + dataGridView1.Rows[e.RowIndex].Cells["SalesQuotationNo"].Value.ToString() + "\nPlease contact admin!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
        }
    }
}





