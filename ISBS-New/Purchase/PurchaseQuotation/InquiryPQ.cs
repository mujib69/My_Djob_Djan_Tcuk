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
        public static int dataShow;

        public int DataShow { get { return dataShow; } set { dataShow = value; } }

        //List<FormPQ> ListFormPQ = new List<FormPQ>();

        //begin
        //created by : joshua
        //created date : 21 feb 2018
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

        public InquiryPQ()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            //hasim 11 okt 2018
            int mflag;
            String addquery = null;

            Query = "SELECT * FROM ";
            Query += "(Select ROW_NUMBER() OVER (ORDER BY a.OrderDate desc,PurchQuotID desc)No, a.PurchQuotID, a.OrderDate,a.VendID,a.VendName,a.VendorQuotNo,a.VendorQuotDate,a.RfqID, RFQh.RfqDate,a.PurchReqId,PRh.OrderDate[OrderDate1],PRh.TransType, cr.FullName[CreatedBy],a.CreatedDate,up.FullName[UpdatedBy],a.UpdatedDate ";
            Query += "From [dbo].[PurchQuotationH] a ";
            Query += "LEFT JOIN RequestForQuotationH RFQh ON a.RfqID = RFQh.RfqID ";
            Query += "LEFT JOIN PurchRequisitionH PRh ON a.PurchReqId = PRh.PurchReqId ";
            Query += "LEFT JOIN [dbo].[sysPass] cr ON a.CreatedBy = cr.UserID ";
            Query += "LEFT JOIN [dbo].[sysPass] up ON a.UpdatedBy = up.UserID ";

            addquery = "WHERE (a.PurchQuotID LIKE @search OR a.VendName LIKE @search OR a.VendorQuotNo LIKE @search OR a.RfqID LIKE @search OR a.PurchReqId LIKE @search OR a.TransType LIKE @search OR cr.FullName LIKE @search OR up.FullName LIKE @search) ";
            mflag = 1;
            if (crit == null)
            {
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += addquery;
            }
            else
            {
                crit = cmbCriteria.Text;
                Cmd = new SqlCommand("Select FieldName From [User].[Table] WHERE DisplayName = '" + crit + "' AND TableName = 'PurchQuotationH'", Conn);
                crit = Cmd.ExecuteScalar().ToString();
                if (crit == "VendorQuotDate" || crit == "RfqDate" || crit == "OrderDate" || crit == "CreatedDate" || crit == "UpdatedDate")
                {
                    mflag = 2;
                    if (crit == "RfqDate")
                    {
                        crit = "RFQh." + crit;
                    }
                    else
                    {
                        crit = "a." + crit;
                    }
                    Query += addquery + "AND (" + crit + " BETWEEN @from AND @to) ";
                }
                else
                {
                    if (crit == "CreatedBy")
                    {
                        crit = "cr.FullName";
                    }
                    else if (crit == "UpdatedBy")
                    {
                        crit = "up.FullName";
                    }
                    else
                    {
                        crit = "a." + crit;
                    }
                    Query += "WHERE " + crit + " LIKE @search";
                }
            }
            Query += ") z WHERE No BETWEEN @limit1 AND @limit2 ;";

            
            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@limit1", Limit1);
            Da.SelectCommand.Parameters.AddWithValue("@limit2", Limit2);
            if (mflag > 0)
            {
                Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                if (mflag == 2)
                {
                    Da.SelectCommand.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd") + " 00:00:00");
                    Da.SelectCommand.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd") + " 23:59:59");
                }
            } 
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPQ.AutoGenerateColumns = true;
            dgvPQ.DataSource = Dt;
            dgvPQ.Refresh();
            dgvPQ.AutoResizeColumns();
            
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            Query = "SELECT COUNT(*) FROM [dbo].[PurchQuotationH] a ";
            Query += "LEFT JOIN RequestForQuotationH RFQh ON a.RfqID = RFQh.RfqID ";
            Query += "LEFT JOIN PurchRequisitionH PRh ON a.PurchReqId = PRh.PurchReqId ";
            Query += "LEFT JOIN [dbo].[sysPass] cr ON a.CreatedBy = cr.UserID ";
            Query += "LEFT JOIN [dbo].[sysPass] up ON a.UpdatedBy = up.UserID ";

            if (crit == null)
            {
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += addquery;
            }
            else
            {
                if (mflag == 2)
                {
                    Query += addquery + "AND (" + crit + " BETWEEN @from AND @to) ";
                }
                else
                {
                    Query += "WHERE " + crit + " LIKE @search";
                }
            }
            
            Cmd = new SqlCommand(Query, Conn);
            if (mflag > 0)
            {
                Cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                if (mflag == 2)
                {
                    Cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd") + " 00:00:00");
                    Cmd.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd") + " 23:59:59");
                }
            } 
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            //lblTotal.Text = "Total Rows : " + Total.ToString();
            //if (cmbShow.Text == "")
            //    cmbShow.Text = "10";
            //else
            //    Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            //lblPage.Text = "/ " + Page2;


            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchQuotationH' And FieldName <> 'VendID' order by OrderNo";

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
            //cmbShowLoad();
            //Limit1 = 1;
            //Limit2 = Int32.Parse(cmbShow.Text);
            //Page1 = 1;
            //txtPage.Text = "1";

            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            RefreshGrid();

            btnProcess.Visible = false;
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
            }
            if (Convert.ToInt32(txtPage.Text) < 1)
            {
                txtPage.Text = "1";
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
            }
            if (Convert.ToInt32(txtPage.Text) < 1)
            {
                txtPage.Text = "1";
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

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
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
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                //header.flag("", "New");
                FormPQ FormPQ = new FormPQ();
                ListFormPQ.Add(FormPQ);
                FormPQ.SetMode("New", "");
                FormPQ.SetParent(this);
                FormPQ.Show();
                RefreshGrid();
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
                FormPQ header = new FormPQ();
                header.SetMode("BeforeEdit", dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString());
                header.SetParent(this);
                header.Show();
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvPQ.RowCount > 0)
                    {
                        Conn = ConnectionString.GetConnection();

                        Index = dgvPQ.CurrentRow.Index;
                        string PurchQuotID = dgvPQ.Rows[Index].Cells["PurchQuotID"].Value == null ? "" : dgvPQ.Rows[Index].Cells["PurchQuotID"].Value.ToString();
                        //string TransType = dgvPR.Rows[Index].Cells["TransType"].Value == null ? "" : dgvPR.Rows[Index].Cells["TransType"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        Query += "SELECT CanvasId FROM CanvasSheetH C INNER JOIN PurchQuotationH  P ON P.PurchReqId = C.PurchReqId WHERE P.PurchQuotID = '" + PurchQuotID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string GetDataPQ = Cmd.ExecuteScalar().ToString();

                        if (GetDataPQ != "")
                        {
                            MessageBox.Show("Data tidak dapat dihapus karena sudah dibuat Canvas Sheet");
                            return;
                        }
                        
                        DialogResult dr = MessageBox.Show("PurchQuotID = " + PurchQuotID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string Check = "";
                          

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
                            //Conn = ConnectionString.GetConnection();

                            //Pengecekan untuk Quotation yang terhubung dengan RFQ
                            //Query = "Select TransStatus from [dbo].[PurchQuotationH] where [PurchQuotID]='" + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString() + "';";
                            //Cmd = new SqlCommand(Query, Conn);
                            //Check = Cmd.ExecuteScalar().ToString();
                            //if (Check == "22")
                            //{
                            //    MessageBox.Show("PurchQuotID = " + dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                            //    Conn.Close();
                            //    return;
                            //}

                            //delete detail
                            //Query = "Delete from [dbo].[PurchQuotation_DtlDtl] where PurchQuotID ='" + PurchQuotID + "';";
                            //Query += "Delete from [dbo].[tblAttachments] where ReffTransId='" + PurchQuotID + "';";
                            //Query += "Delete from [dbo].[PurchQuotation_Dtl] where PurchQuotID ='" + PurchQuotID + "';";
                            //Query += "Delete from [dbo].[PurchQuotationH] where PurchQuotID='" + PurchQuotID + "';";
                            Query = "UPDATE PurchQuotationH SET TransStatus = '02' WHERE PurchQuotID = '" + PurchQuotID + "'";

                            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("PurchQuotID = " + PurchQuotID.ToUpper() + "\n" + "Data berhasil dicancel.");

                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                            Index = 0;
                         
                            refreshTaskListPQ();
                            RefreshGrid();

                        }

                        Conn.Close();
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

        private void refreshTaskListPQ()
        {
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            FormPQ header = new FormPQ();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                //Simpen HeaderId
                if (dgvPQ.RowCount > 0)
                {                    
                    header.SetMode("BeforeEdit", dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString());
                    header.SetParent(this);
                    header.Show();
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
            MainMenu f = new MainMenu();
            f.refreshTaskList();
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
            cmbCriteria.SelectedIndex = 0;
            ModeLoad();
        }

       private void timerRefresh_Tick(object sender, EventArgs e)
       {
            if (timerRefresh == null)
            {

            }
            else
            {
               // RefreshGrid();
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
            MainMenu f = new MainMenu();
            f.refreshTaskList();
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
            gvHeader();
        }

        private void gvHeader()
        {
            #region Header
            dgvPQ.Columns["No"].HeaderText = "No";
            dgvPQ.Columns["PurchQuotID"].HeaderText = "PQ No";
            dgvPQ.Columns["OrderDate"].HeaderText = "PQ Date";
            dgvPQ.Columns["VendID"].HeaderText = "VendID";
            dgvPQ.Columns["VendName"].HeaderText = "Vendor";
            dgvPQ.Columns["VendorQuotNo"].HeaderText = "Vendor Quot. No";
            dgvPQ.Columns["VendorQuotDate"].HeaderText = "Vendor Quot. Date";
            dgvPQ.Columns["RfqID"].HeaderText = "RFQ No";
            dgvPQ.Columns["RfqDate"].HeaderText = "RFQ Date";
            dgvPQ.Columns["PurchReqId"].HeaderText = "PR No";
            dgvPQ.Columns["OrderDate1"].HeaderText = "PR Date";
            dgvPQ.Columns["TransType"].HeaderText = "PR Type";
            dgvPQ.Columns["CreatedBy"].HeaderText = "Created By";
            dgvPQ.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvPQ.Columns["UpdatedBy"].HeaderText = "Updated By";
            dgvPQ.Columns["UpdatedDate"].HeaderText = "Updated Date";
            #endregion Header

            #region Order
            dgvPQ.Columns["No"].DisplayIndex = 0;
            dgvPQ.Columns["PurchQuotID"].DisplayIndex = 1;
            dgvPQ.Columns["OrderDate"].DisplayIndex = 2;            
            dgvPQ.Columns["VendName"].DisplayIndex = 3;
            dgvPQ.Columns["VendorQuotNo"].DisplayIndex = 4;
            dgvPQ.Columns["VendorQuotDate"].DisplayIndex = 5;
            dgvPQ.Columns["RfqID"].DisplayIndex = 6;
            dgvPQ.Columns["RfqDate"].DisplayIndex = 7;
            dgvPQ.Columns["PurchReqId"].DisplayIndex = 8;
            dgvPQ.Columns["OrderDate1"].DisplayIndex = 9;
            dgvPQ.Columns["TransType"].DisplayIndex = 10;
            dgvPQ.Columns["CreatedBy"].DisplayIndex = 11;
            dgvPQ.Columns["CreatedDate"].DisplayIndex = 12;
            dgvPQ.Columns["UpdatedBy"].DisplayIndex = 13;
            dgvPQ.Columns["UpdatedDate"].DisplayIndex = 14;
            dgvPQ.Columns["VendID"].DisplayIndex = 15;
            #endregion Order

            #region Visibility
            dgvPQ.Columns["VendID"].Visible = false;
            #endregion Visibility

            #region DefaultFormat         
            dgvPQ.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["VendorQuotDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvPQ.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            //dgvPQ.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            //dgvPQ.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            #endregion DefaultFormat
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

                        ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
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

        private void dgvPQ_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dgvPQ.Columns[e.ColumnIndex].Name.ToString() == "VendID")
                {
                    vendID = dgvPQ.Rows[e.RowIndex].Cells["VendID"].Value.ToString();
                    PopUp.Vendor.Vendor f = new PopUp.Vendor.Vendor();
                    f.Close();
                    f = new PopUp.Vendor.Vendor();
                    f.Show();
                }

            }
        }

        public static string vendID;
        public string VendID
        {
            get { return vendID; }
            set { vendID = value; }
        }


    }
}
