using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.TaskList.AccountPayable
{
    public partial class TasklistPurchaseInvoiceAP : MetroFramework.Forms.MetroForm
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

        List<ISBS_New.AccountPayable.HeaderAccountsPayable> ListHeaderAP = new List<ISBS_New.AccountPayable.HeaderAccountsPayable>();
      
        public TasklistPurchaseInvoiceAP()
        {
            InitializeComponent();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            backtopageone(); 
            //dataShow = Int32.Parse(cmbShow.Text);
            //Limit1 = 1;
            //Limit2 = Int32.Parse(cmbShow.Text);
            //Page1 = 1;
            //txtPage.Text = "1";

            dtFromDate.Value = DateTime.Today.Date;
            dtToDate.Value = DateTime.Today.Date;

            cmbCriteria.SelectedIndex = 0;
            cmbStatusCode.SelectedIndex = 0;
            cmbStatusCode.Enabled = false;
            RefreshGrid();
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'AccountsPayableH' and DisplayName!='CreateDate'";
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
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='VendInvoice'";

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

        private void TasklistPurchaseInvoiceAP_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            addCmbCrit();
            ModeLoad();
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
            else if (ControlMgr.GroupName=="AP Admin")
            {
                Tampil("'01','02'");
            }
            else
            {
                Tampil("'XX'");
            }
            dgvInquiry.ReadOnly = true;
        }
        //tia edit 11102018
        private void backtopageone()
        {
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
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
            Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
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
            
            dgvInquiry.Columns["CreatedDate"].Visible = false;
            dgvInquiry.Columns["UpdatedDate"].Visible = false;
            dgvInquiry.Columns["UpdatedBy"].Visible = false;
            dgvInquiry.Columns["CreatedBy"].Visible = false;
            dgvInquiry.Columns["ApprovedBy"].Visible = false;
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
      

        private void ApproveAP()
        {
            ISBS_New.AccountPayable.HeaderAccountsPayable header = new ISBS_New.AccountPayable.HeaderAccountsPayable();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvInquiry.RowCount > 0)
                {
                    header.SetParent3(this);
                    header.SetMode("Approve", dgvInquiry.CurrentRow.Cells["InvoiceId"].Value.ToString());
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
            ApproveAP();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
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
            backtopageone(); 
            RefreshGrid();
        }

        private void TasklistPurchaseInvoiceAP_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void TasklistPurchaseInvoiceAP_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
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
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
        }

        private void dgvInquiry_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ApproveAP();
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

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (txtSearch.Text == null || txtSearch.Text.Equals(""))
                {
                    MessageBox.Show("Masukkan Kata Kunci");
                }
                else if (cmbCriteria.SelectedIndex == -1)
                {
                    crit = "All";
                }
                else
                {
                    crit = cmbCriteria.SelectedItem.ToString();
                }
                backtopageone();  //hasim 10 oct 2018
                RefreshGrid();
            }
        }





    }
}
