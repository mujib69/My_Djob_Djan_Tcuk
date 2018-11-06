using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.Purchase.PurchaseQuotation
{
    public partial class InquiryPQ : MetroFramework.Forms.MetroForm
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
        List<FormPQ> ListFormPQ = new List<FormPQ>();

        //List<FormPQ> ListFormPQ = new List<FormPQ>();

        //timer autorefresh
        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10*1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        public InquiryPQ()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                Query = "Select a.No,a.PurchQuotID,a.OrderDate,a.VendorQuotNo,a.VendorQuotDate,a.VendID,a.TransStatus,UPPER(b.Deskripsi) StatusName,a.ApprovedBy,a.CreatedDate, a.CreatedBy From (Select ROW_NUMBER() OVER (ORDER BY PurchQuotID desc) No, PurchQuotID, OrderDate, VendorQuotNo, VendorQuotDate,VendID,TransStatus,ApprovedBy,CreatedDate, CreatedBy From [dbo].[PurchQuotationH] ) a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select a.No,a.PurchQuotID,a.OrderDate,a.VendorQuotNo,a.VendorQuotDate,a.VendID,a.TransStatus,UPPER(b.Deskripsi) StatusName,a.ApprovedBy,a.CreatedDate, a.CreatedBy From (Select ROW_NUMBER() OVER (ORDER BY PurchQuotID desc) No, PurchQuotID, OrderDate, VendorQuotNo, VendorQuotDate,VendID,TransStatus,ApprovedBy,CreatedDate, CreatedBy From [dbo].[PurchQuotationH] Where ";
                Query += "PurchQuotID like '%" + txtSearch.Text + "%' or VendorQuotNo like '%" + txtSearch.Text + "%' or VendID like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%') a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("OrderDate"))
            {
                Query = "Select a.No,a.PurchQuotID,a.OrderDate,a.VendorQuotNo,a.VendorQuotDate,a.VendID,a.TransStatus,UPPER(b.Deskripsi) StatusName,a.ApprovedBy,a.CreatedDate, a.CreatedBy From (Select ROW_NUMBER() OVER (ORDER BY PurchQuotID desc) No, PurchQuotID, OrderDate, VendorQuotNo, VendorQuotDate,VendID,TransStatus,ApprovedBy,CreatedDate, CreatedBy From [dbo].[PurchQuotationH] Where ";
                Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchQuotID like '%" + txtSearch.Text + "%' or VendorQuotNo like '%" + txtSearch.Text + "%' or VendID like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%')) a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "Select a.No,a.PurchQuotID,a.OrderDate,a.VendorQuotNo,a.VendorQuotDate,a.VendID,a.TransStatus,UPPER(b.Deskripsi) StatusName,a.ApprovedBy,a.CreatedDate, a.CreatedBy From (Select ROW_NUMBER() OVER (ORDER BY PurchQuotID desc) No, PurchQuotID, OrderDate, VendorQuotNo, VendorQuotDate,VendID,TransStatus,ApprovedBy,CreatedDate,CreatedBy From [dbo].[PurchQuotationH] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchQuotID like '%" + txtSearch.Text + "%' or VendorQuotNo like '%" + txtSearch.Text + "%' or VendID like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%')) a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select a.No,a.PurchQuotID,a.OrderDate,a.VendorQuotNo,a.VendorQuotDate,a.VendID,a.TransStatus,UPPER(b.Deskripsi) StatusName,a.ApprovedBy,a.CreatedDate, a.CreatedBy From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchQuotationH] Where ";
                Query += crit + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPQ.AutoGenerateColumns = true;
            dgvPQ.DataSource = Dt;
            dgvPQ.Refresh();
            dgvPQ.AutoResizeColumns();
            dgvPQ.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["VendorQuotDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(PurchQuotID) From [dbo].[PurchQuotationH];";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(PurchQuotID) From [dbo].[PurchQuotationH] Where ";
                Query += "TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%' ";
            }
            else if (crit.Equals("OrderDate"))
            {
                Query = "Select Count(PurchQuotID) From [dbo].[PurchQuotationH] Where ";
                Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchQuotID like '%" + txtSearch.Text + "%' or VendorQuotNo like '%" + txtSearch.Text + "%' or VendID like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%'); ";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "Select Count(PurchQuotID) From [dbo].[PurchQuotationH] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchQuotID like '%" + txtSearch.Text + "%' or VendorQuotNo like '%" + txtSearch.Text + "%' or VendID like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' or ApprovedBy like '%" + txtSearch.Text + "%');";
            }
            else
            {
                Query = "Select Count(PurchQuotID) From [dbo].[PurchQuotationH] Where ";
                Query += crit + " Like '%" + txtSearch.Text + "%'";
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
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchQuotationH'";

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
            FormPQ FormPQ = new FormPQ();
            //header.flag("", "New");
            ListFormPQ.Add(FormPQ);
            FormPQ.SetMode("New", "");
            FormPQ.SetParent(this);
            FormPQ.Show();
            RefreshGrid();
        }

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                FormPQ header = new FormPQ();
                header.SetMode("BeforeEdit", dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString());
                header.Show();
                header.SetParent(this);
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPQ.RowCount > 0)
                {
                    Index = dgvPQ.CurrentRow.Index;
                    string PurchQuotID = dgvPQ.Rows[Index].Cells["PurchQuotID"].Value == null ? "" : dgvPQ.Rows[Index].Cells["PurchQuotID"].Value.ToString();
                    //string TransType = dgvPR.Rows[Index].Cells["TransType"].Value == null ? "" : dgvPR.Rows[Index].Cells["TransType"].Value.ToString();
                    //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                    DialogResult dr = MessageBox.Show("PurchQuotID = " + PurchQuotID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        string Check = "";
                        Conn = ConnectionString.GetConnection();

                        Query = "Select TransStatus from [dbo].[PurchQuotationH] where [PurchQuotID]='" + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString() + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        Check = Cmd.ExecuteScalar().ToString();
                        if (Check == "22")
                        {
                            MessageBox.Show("PurchQuotID = " + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                            Conn.Close();
                            return;
                        }

                        //delete header
                        Conn = ConnectionString.GetConnection();

                        //delete detail
                        Query = "Delete from [dbo].[PurchQuotation_Dtl] where PurchQuotID ='" + PurchQuotID + "';";
                        Query += "Delete from [dbo].[PurchQuotationH] where PurchQuotID='" + PurchQuotID + "';";

                        //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & Login.Username & "');";

                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        MessageBox.Show("PurchQuotID = " + PurchQuotID.ToUpper() + "\n" + "Data berhasil dihapus.");

                        ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & Login.Username & "');";
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //Simpen HeaderId
            if (dgvPQ.RowCount > 0)
            {
                FormPQ header = new FormPQ();
                header.SetMode("BeforeEdit", dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString());
                header.SetParent(this);
                header.Show();
                RefreshGrid();
            }
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

        private void InquiryPR_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
            for (int i = 0; i < ListFormPQ.Count(); i++)
            {
                ListFormPQ[i].Close();
            }
        }

        private void InquiryPR_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            lblForm.Location = new Point(16, 11);
            //setTimer();
        }

        private void InquiryPR_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
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
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                string Check="";
                DialogResult dr = MessageBox.Show("PurchQuotID = " + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString() + "\n" + "Apakah data diatas akan diposted ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    Conn = ConnectionString.GetConnection();

                    Query = "Select TransStatus from [dbo].[PurchQuotationH] where [PurchQuotID]='" + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString() + "';";
                    Cmd = new SqlCommand(Query, Conn);
                    Check = Cmd.ExecuteScalar().ToString();
                    if (Check != "22")
                    {
                        Query = "Select Price from [dbo].[PurchQuotation_Dtl] where [PurchQuotID]='" + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString() + "' and Price <=0;";
                        Cmd = new SqlCommand(Query, Conn);
                        Check = Cmd.ExecuteScalar() == null ? "1" : Cmd.ExecuteScalar().ToString();
                        if (Convert.ToDecimal(Check) <= 0)
                        {
                            MessageBox.Show("PurchQuotID = " + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa diposting karena Item price ada yang 0.");
                            Conn.Close();
                            return;
                        }
                        Query = "Update PurchQuotationH set TransStatus='22' where PurchQuotID='" + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString() + "';";
                        Query += "Update PurchRequisitionH set TransStatus='22' where PurchReqID=(select top 1 ReffTransId from PurchQuotation_Dtl where PurchQuotID='" + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString() + "' );";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        MessageBox.Show("PurchQuotID = " + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString().ToUpper() + "\n" + "Data berhasil diposting.");

                        ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & Login.Username & "');";
                        //Index = 0;
                        Conn.Close();
                        RefreshGrid();
                    }
                    else
                    {
                        MessageBox.Show("PurchQuotID = " + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString().ToUpper() + ".\n" + "Sudah terposting.");
                        return;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
