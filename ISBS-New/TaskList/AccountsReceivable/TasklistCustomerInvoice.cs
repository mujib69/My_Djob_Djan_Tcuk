using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.TaskList.AccountsReceivable
{
    public partial class TasklistCustomerInvoice : MetroFramework.Forms.MetroForm
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

        public TasklistCustomerInvoice()
        {
            InitializeComponent();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void TasklistCustomerInvoice_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            addCmbCrit();
            ModeLoad();
        }

        private void TasklistCustomerInvoice_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
            RefreshGrid2();
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
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'CustInvoiceH' and DisplayName!='CreatedBy' and DisplayName!='UpdatedBy' and DisplayName!='CreatedDate' and DisplayName!='UpdatedDate' ORDER BY OrderNo ASC";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='CustInvoice' ORDER BY StatusCode ASC";

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
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {

                if (ControlMgr.GroupName.ToUpper() == "AR ADMIN")
                {
                    ISBS_New.AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice H = new ISBS_New.AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice();
                    H.SetMode("New", "");
                    H.SetParent2(this);
                    H.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }

            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        public void RefreshGrid2()
        {
            if (ControlMgr.GroupName == "Tax Admin")
            {
                RefreshGrid("'01'");
            }
            else if (ControlMgr.GroupName == "AR Manager")
            {
                RefreshGrid("'02'");
            }
            else if (ControlMgr.GroupName == "Tax Manager")
            {
                RefreshGrid("'11'");
            }
            else if (ControlMgr.GroupName == "AR Admin")
            {
                RefreshGrid("'01', '02', '07', '08', '11', '17'");
            }
            else
            {
                RefreshGrid("'XX'");
            }
            //dgvInquiry.ReadOnly = true;
        }

        public void RefreshGrid(string TransCode)
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
                cmbSelected = "05";
            }
            else if (cmbSelected == "5")
            {
                cmbSelected = "07";
            }
            else if (cmbSelected == "6")
            {
                cmbSelected = "08";
            }
            else if (cmbSelected == "7")
            {
                cmbSelected = "09";
            }
            else if (cmbSelected == "8")
            {
                cmbSelected = "11";
            }
            else if (cmbSelected == "9")
            {
                cmbSelected = "13";
            }
            else if (cmbSelected == "10")
            {
                cmbSelected = "17";
            }
            else if (cmbSelected == "11")
            {
                cmbSelected = "20";
            }
            else if (cmbSelected == "12")
            {
                cmbSelected = "21";
            }
            else if (cmbSelected == "13")
            {
                cmbSelected = "22";
            }
            else
            {
                cmbSelected = "";
            }
            Conn = ConnectionString.GetConnection();

            if (TransStatus == String.Empty)
            {
                TransStatus=TransCode;
               // TransStatus = "'01', '02', '07', '08', '11', '17'";
                Limit1 = 1; Limit2 = dataShow;
            }

            int mflag;

            Query = "Select * From (Select ROW_NUMBER()OVER(ORDER BY CreatedDate desc) as [No],Invoice_Date AS InvoiceDate,";
            Query += "Invoice_Id AS InvoiceNo, Invoice_Type AS InvoiceType, Cust_Id AS CustomerId, Cust_Name AS CustName, Invoice_DueDate AS PaymentDueDate, b.Deskripsi AS StatusName, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApprovedBy ";
            Query += "From CustInvoice_H a ";
            Query += "Inner join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='CustInvoice'";
            Query += "Where StatusCode IN (" + TransStatus + ") ";
            mflag = 1;


            if (crit == null)
            {
                Query += ") A ";
                mflag = 0;
            }
            else if (crit.Equals("InvoiceNo"))
            {
                Query += "AND Invoice_Id like @search) A ";
            }
            else if (crit.Equals("InvoiceType"))
            {
                Query += "AND Invoice_Type like @search) A ";
            }
            else if (crit.Equals("CustId"))
            {
                Query += "AND Cust_Id like @search) A ";
            }
            else if (crit.Equals("CustName"))
            {
                Query += "AND Cust_Name like @search) A ";
            }
            else if (crit.Equals("CreatedBy"))
            {
                Query += "AND CreatedBy like @search) A ";
            }
            else if (crit.Equals("UpdatedBy"))
            {
                Query += "AND UpdatedBy like @search) A ";
            }
            else if (crit.Equals("ApprovedBy"))
            {
                Query += "AND ApprovedBy like @search) A ";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),a.CreatedDate,120) >= @from AND CONVERT(VARCHAR(10),a.CreatedDate,120) <= @to)) A ";
                mflag = 2;
            }
            else if (crit.Equals("UpdatedDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),a.UpdatedDate,120) >= @from AND CONVERT(VARCHAR(10),a.UpdatedDate,120) <= @to)) A ";
                mflag = 2;
            }
            else if (crit.Equals("InvoiceDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),a.Invoice_Date,120) >= @from AND CONVERT(VARCHAR(10),a.Invoice_Date,120) <= @to)) A ";
                mflag = 2;
            }
            else if (crit.Equals("PaymentDueDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),a.Invoice_DueDate,120) >= @from AND CONVERT(VARCHAR(10),a.Invoice_DueDate,120) <= @to)) A ";
                mflag = 2;
            }
            else if (crit.Equals("StatusName"))
            {
                Query += "AND b.StatusCode LIKE @search) A ";
                mflag = 3;
            }
            else if (crit.Equals("All"))
            {
                Query += "AND (Invoice_Id LIKE @search OR Cust_Id LIKE @search OR Cust_Name LIKE @search OR Invoice_Type LIKE @search OR CreatedBy LIKE @search OR UpdatedBy LIKE @search OR ApprovedBy LIKE @search)) A ";
            }

            Query += "Where A.No Between @limit1 AND @limit2 ;";

            Da = new SqlDataAdapter(Query, Conn);

            Da.SelectCommand.Parameters.AddWithValue("@limit1", Limit1);
            Da.SelectCommand.Parameters.AddWithValue("@limit2", Limit2);
            switch (mflag)
            {
                case 1:
                    Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    break;
                case 2:
                    Da.SelectCommand.Parameters.AddWithValue("@from", dtFromDate.Value.Date.ToString("yyyy-MM-dd"));
                    Da.SelectCommand.Parameters.AddWithValue("@to", dtToDate.Value.Date.ToString("yyyy-MM-dd"));
                    break;
                case 3:
                    Da.SelectCommand.Parameters.AddWithValue("@search", cmbSelected);
                    break;
            }
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvCustomerInvoiceInquiry.AutoGenerateColumns = true;
            dgvCustomerInvoiceInquiry.DataSource = Dt;

            for (int i = 0; i < dgvCustomerInvoiceInquiry.RowCount; i++)
            {
                if (Convert.ToDateTime(dgvCustomerInvoiceInquiry.Rows[i].Cells["UpdatedDate"].Value) == new DateTime(1753, 1, 1))
                    dgvCustomerInvoiceInquiry.Rows[i].Cells["UpdatedDate"].Value = (object)DBNull.Value;
            }

            dgvCustomerInvoiceInquiry.Refresh();
            dgvCustomerInvoiceInquiry.AutoResizeColumns();
            dgvCustomerInvoiceInquiry.ReadOnly = true;
            Conn.Close();

            dgvCustomerInvoiceInquiry.Columns["InvoiceDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvCustomerInvoiceInquiry.Columns["PaymentDueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvCustomerInvoiceInquiry.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            dgvCustomerInvoiceInquiry.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";

            dgvCustomerInvoiceInquiry.Columns["CreatedDate"].Visible = false;
            dgvCustomerInvoiceInquiry.Columns["CreatedBy"].Visible = false;
            dgvCustomerInvoiceInquiry.Columns["UpdatedDate"].Visible = false;
            dgvCustomerInvoiceInquiry.Columns["UpdatedBy"].Visible = false;

            //paging
            Conn = ConnectionString.GetConnection();

            Query = "SELECT COUNT(Invoice_Id) FROM CustInvoice_H WHERE TransStatus IN (" + TransStatus + ") ";

            mflag = 1;
            if (crit == null)
            {
                Query += "";
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += "AND (Invoice_Id LIKE @search OR Cust_Id LIKE @search OR Cust_Name LIKE @search OR Invoice_Type LIKE @search OR CreatedBy LIKE @search OR UpdatedBy LIKE @search OR ApprovedBy LIKE @search) ";
            }
            else if (crit.Equals("InvoiceNo"))
            {
                Query += "AND Invoice_Id LIKE @search ";
            }
            else if (crit.Equals("InvoiceType"))
            {
                Query += "AND Invoice_Type LIKE @search ";
            }
            else if (crit.Equals("CustId"))
            {
                Query += "AND Cust_Id LIKE @search ";
            }
            else if (crit.Equals("CustName"))
            {
                Query += "AND Cust_Name LIKE @search ";
            }
            else if (crit.Equals("CreatedBy"))
            {
                Query += "AND CreatedBy LIKE @search ";
            }
            else if (crit.Equals("UpdatedBy"))
            {
                Query += "AND UpdatedBy LIKE @search ";
            }
            else if (crit.Equals("ApprovedBy"))
            {
                Query += "AND ApprovedBy LIKE @search ";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),CreatedDate,120) >= @from AND CONVERT(VARCHAR(10),CreatedDate,120) <= @to);";
                mflag = 2;
            }
            else if (crit.Equals("UpdatedDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),UpdatedDate,120) >= @from AND CONVERT(VARCHAR(10),UpdatedDate,120) <= @to);";
                mflag = 2;
            }
            else if (crit.Equals("InvoiceDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),Invoice_Date,120) >= @from AND CONVERT(VARCHAR(10),Invoice_Date,120) <= @to);";
                mflag = 2;
            }
            else if (crit.Equals("PaymentDueDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),Invoice_DueDate,120) >= @from AND CONVERT(VARCHAR(10),Invoice_DueDate,120) <= @to);";
                mflag = 2;
            }
            else if (crit.Equals("StatusName"))
            {
                Query += "AND b.StatusCode LIKE @search";
                mflag = 3;
            }


            Cmd = new SqlCommand(Query, Conn);
            switch (mflag)
            {
                case 1:
                    Cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    break;
                case 2:
                    Cmd.Parameters.AddWithValue("@from", dtFromDate.Value.Date.ToString("yyyy-MM-dd"));
                    Cmd.Parameters.AddWithValue("@to", dtToDate.Value.Date.ToString("yyyy-MM-dd"));
                    break;
                case 3:
                    Cmd.Parameters.AddWithValue("@search", cmbSelected);
                    break;
            }
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

            cmbShowLoad();
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            RefreshGrid2();
        }

        private void SelectData()
        {
            ISBS_New.AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice header = new ISBS_New.AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvCustomerInvoiceInquiry.RowCount > 0)
                {
                    header.SetParent2(this);
                    header.SetMode("BeforeEdit", dgvCustomerInvoiceInquiry.CurrentRow.Cells["InvoiceNo"].Value.ToString());
                    header.Show();
                    RefreshGrid2();
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
            SelectData();
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid2();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid2();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            RefreshGrid2();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }
            RefreshGrid2();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid2();
        }

        private void dgvCustomerInvoiceInquiry_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectData();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid2();
            }
        }

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                txtPage.Text = "1";
                RefreshGrid2();

            }
        }



    }
}
