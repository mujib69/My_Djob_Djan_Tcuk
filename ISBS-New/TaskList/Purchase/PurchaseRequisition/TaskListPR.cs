using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.TaskList.Purchase.PurchaseRequisition
{
    public partial class TaskListPR : MetroFramework.Forms.MetroForm
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
        public bool openparent = true;

        MainMenu Parent = new MainMenu();

        public TaskListPR()
        {
            InitializeComponent();
            if (ControlMgr.GroupName != "Purchase Manager")
                btnGenerateRFQ.Visible = false;
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        
        }

        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10*1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        private void TaskListPR_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
            gvheader();

        }
//hasim
        public void RefreshGrid()
        {
            if (crit == "PR No")
            {
                crit = "PurchReqID";
            }
            else if (crit == "PR Type")
            {
                crit = "TransType";
            }
            else if (crit == "Status")
            {
                crit = "TransStatus";
            }
            else if (crit == "Created By")
            {
                crit = "cr.FullName";
            }
            else if (crit == "Updated By")
            {
                crit = "up.FullName";
            }
            String addquery, addque2 = null;
            int mflag;
            if (ControlMgr.GroupName == "Sales Manager")
            {
                //Menampilkan data untuk Sales Manager
                Conn = ConnectionString.GetConnection();
                //Query = "with temp as (select row_number() over (partition by reffID order by logdate desc) as rownum, * from WorkflowLogTable)";
                //Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, UPPER(t.Deskripsi) StatusName, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
                Query = "Select * From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, UPPER(t.Deskripsi) StatusName, cr.FullName[CreatedBy], CreatedDate, up.FullName[UpdatedBy], UpdatedDate From [dbo].[PurchRequisitionH] h ";
                Query += "LEFT JOIN [dbo].[TransStatusTable] t ON h.TransStatus = t.StatusCode ";
                Query += "LEFT JOIN [dbo].[sysPass] cr ON h.CreatedBy=cr.UserID ";
                Query += "LEFT JOIN [dbo].[sysPass] up ON h.UpdatedBy=up.UserID WHERE TransStatus in ('01') And t.TransCode = 'PR'";
                mflag = 0;
                addquery = "(PurchReqID LIKE @search OR TransType LIKE @search OR TransStatus LIKE @search OR cr.FullName LIKE @search OR up.FullName LIKE @search)";
                if (crit == null){
                    Query += ") a ";
                }else if (crit.Equals("All")){
                    Query += " AND " + addquery + ") a ";
                    mflag = 1;
                }else if (crit.Equals("PR Date")){
                    Query += " AND (OrderDate BETWEEN @from AND @to) AND " + addquery + ") a ";
                    mflag = 2;
                }else if (crit.Equals("Created Date")){
                    Query += " AND (CreatedDate BETWEEN @from AND @to) AND " + addquery + ") a ";
                    mflag = 2;
                }else if (crit.Equals("Updated Date")){
                    Query += " AND (UpdatedDate BETWEEN @from AND @to) AND " + addquery + ") a ";
                    mflag = 2;
                }else{
                    Query += " AND " + crit + " LIKE @search) a ";
                    mflag = 1;
                }
                Query += "WHERE No BETWEEN @limit1 AND @limit2 ;";
                
                Da = new SqlDataAdapter(Query, Conn);
                Da.SelectCommand.Parameters.AddWithValue("@limit1", Limit1);
                Da.SelectCommand.Parameters.AddWithValue("@limit2", Limit2);
                if (mflag>0)
                {
                    Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    if (mflag == 2) {
                        Da.SelectCommand.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd")+ " 00:00:00");
                        Da.SelectCommand.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd") + " 23:59:59");
                    }  
                }

                Dt = new DataTable();
                Da.Fill(Dt);

                dgvPR.AutoGenerateColumns = true;
                dgvPR.DataSource = Dt;
                dgvPR.Refresh();
                dgvPR.AutoResizeColumns();
                dgvPR.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
                dgvPR.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
                dgvPR.Columns["TransStatus"].Visible = false;
                Conn.Close();

                //Mengambil nilai total paging
                Conn = ConnectionString.GetConnection();
                Query = "Select Count(*) FROM [dbo].[PurchRequisitionH] h ";
                Query += "LEFT JOIN [dbo].[TransStatusTable] t ON h.TransStatus = t.StatusCode ";
                Query += "LEFT JOIN [dbo].[sysPass] cr ON h.CreatedBy=cr.UserID ";
                Query += "LEFT JOIN [dbo].[sysPass] up ON h.UpdatedBy=up.UserID WHERE TransStatus in ('01') And t.TransCode = 'PR'";
                if (crit == null)
                {
                    Query += ";";
                }
                else if (crit.Equals("All"))
                {
                    Query += " AND " + addquery + " ;";
                }
                else if (crit.Equals("PR Date"))
                {
                    Query += " AND (OrderDate BETWEEN @from AND @to) AND " + addquery + " ;";
                }
                else if (crit.Equals("Created Date"))
                {
                    Query += " AND (CreatedDate BETWEEN @from AND @to) AND " + addquery + " ;";
                }
                else if (crit.Equals("Updated Date"))
                {
                    Query += " AND (UpdatedDate BETWEEN @from AND @to) AND " + addquery + " ;";
                }
                else
                {
                    Query += " AND " + crit + " LIKE @search ;";
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

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
                lblPage.Text = "/ " + Page2;
            }
            else if (ControlMgr.GroupName == "Purchase Manager")
            {
                //Menampilkan data untuk purchase Manager
                Conn = ConnectionString.GetConnection();
                //Query = "with temp as (select row_number() over (partition by reffID order by logdate desc) as rownum, * from WorkflowLogTable)";
                //Query += "Select * From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, UPPER(t.Deskripsi) StatusName, CreatedBy, CreatedDate, b.UserId[UpdatedBy], b.LogDate[UpdatedDate] From [dbo].[PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode left join temp b on h.PurchReqID=b.ReffID and b.rownum=1 Where ";
                Query = "Select * From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) No, PurchReqID, OrderDate, TransType, TransStatus, UPPER(t.Deskripsi) StatusName, cr.FullName[CreatedBy], CreatedDate, up.FullName[UpdatedBy],UpdatedDate From [dbo].[PurchRequisitionH] h ";
                Query += "LEFT JOIN [dbo].[TransStatusTable] t ON h.TransStatus = t.StatusCode ";
                Query += "LEFT JOIN [dbo].[sysPass] cr ON h.CreatedBy=cr.UserID ";
                Query += "LEFT JOIN [dbo].[sysPass] up ON h.UpdatedBy=up.UserID WHERE h.TransStatus in ('03','04') And t.TransCode = 'PR'";

                addquery = "(PurchReqID LIKE @search OR TransType LIKE @search OR TransStatus LIKE @search OR ApprovedBy LIKE @search OR cr.FullName LIKE @search OR up.FullName LIKE @search)";
                mflag = 0;
                if (crit == null)
                {
                    Query += ") a ";
                }
                else if (crit.Equals("All"))
                {
                    Query += " AND " + addquery + ") a ";
                    mflag = 1;
                }
                else if (crit.Equals("PR Date"))
                {
                    Query += " AND (OrderDate BETWEEN @from AND @to) AND " + addquery + ") a ";
                    mflag = 2;
                }
                else if (crit.Equals("Created Date"))
                {
                    Query += " AND (CreatedDate BETWEEN @from AND @to) AND " + addquery + ") a ";
                    mflag = 2;
                }
                else if (crit.Equals("Updated Date"))
                {
                    Query += " AND (UpdatedDate BETWEEN @from AND @to) AND " + addquery + ") a ";
                    mflag = 2;
                }
                else
                {
                    Query += " AND " + crit + " LIKE @search ) a ";
                    mflag = 1;
                }
                Query += "WHERE No BETWEEN @limit1 AND @limit2 ;";

                Da = new SqlDataAdapter(Query, Conn);
                Da.SelectCommand.Parameters.AddWithValue("@limit1", Limit1);
                Da.SelectCommand.Parameters.AddWithValue("@limit2", Limit2);
                if (mflag > 0)
                {
                    Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    if (mflag == 2)
                    {
                        Da.SelectCommand.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd"));
                        Da.SelectCommand.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd"));
                    }
                }
                Dt = new DataTable();
                Da.Fill(Dt);

                dgvPR.AutoGenerateColumns = true;
                dgvPR.DataSource = Dt;
                dgvPR.Refresh();
                dgvPR.AutoResizeColumns();
                dgvPR.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPR.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                Conn.Close();

                
                Conn = ConnectionString.GetConnection();

                Query = "Select Count(*) FROM [dbo].[PurchRequisitionH] h ";
                Query += "LEFT JOIN [dbo].[TransStatusTable] t ON h.TransStatus = t.StatusCode ";
                Query += "LEFT JOIN [dbo].[sysPass] cr ON h.CreatedBy=cr.UserID ";
                Query += "LEFT JOIN [dbo].[sysPass] up ON h.UpdatedBy=up.UserID WHERE h.TransStatus IN ('03','04') AND t.TransCode = 'PR'";
               
                if (crit == null)
                {
                    Query += " ;";
                }
                else if (crit.Equals("All"))
                {
                    Query += " AND " + addquery + " ;";
                }
                else if (crit.Equals("PR Date"))
                {
                    Query += " AND (OrderDate BETWEEN @from AND @to) AND " + addquery + " ;";
                }
                else if (crit.Equals("Created Date"))
                {
                    Query += " AND (CreatedDate BETWEEN @from AND @to) AND " + addquery + " ;";
                }
                
                else if (crit.Equals("Updated Date"))
                {
                    Query += " AND (UpdatedDate BETWEEN @from AND @to) AND " + addquery + " ;";
                }
                else
                {
                    string QueryTemp = "Select FieldName from [User].[Table] Where DisplayName = '" + cmbCriteria.Text + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    crit = Cmd.ExecuteScalar().ToString();

                    Query += " AND " + crit + " Like @search ;";
                }

                Cmd = new SqlCommand(Query, Conn);
                if (mflag > 0)
                {
                    Cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    if (mflag == 2)
                    {
                        Cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd"));
                    }
                }
                Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
                Conn.Close();

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
                lblPage.Text = "/ " + Page2;
            }
            Parent.RefreshDocumentApproval();
        }
//end hasim

        //hasim 10 oct 2018
        private void backtopageone()
        {
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
        }
        //end hasim 10 oct 2018

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchRequisitionH'";

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
            backtopageone(); //hasim 10 okt 2018

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

        private void gvheader()
        {
            if (dgvPR.RowCount > 0)
            {
                dgvPR.Columns["PurchReqId"].HeaderText = "PR No";
                dgvPR.Columns["OrderDate"].HeaderText = "PR Date";
                dgvPR.Columns["TransType"].HeaderText = "PR Type";
                //dgvPR.Columns["TransStatus"].HeaderText = "PR Status";
                dgvPR.Columns["StatusName"].HeaderText = "Status";
                //dgvPR.Columns["StatusName"].HeaderText = "Status Name";
                dgvPR.Columns["CreatedDate"].HeaderText = "Created Date";
                dgvPR.Columns["CreatedBy"].HeaderText = "Created By";
                dgvPR.Columns["UpdatedDate"].HeaderText = "Updated Date";
                dgvPR.Columns["UpdatedBy"].HeaderText = "Updated By";

                dgvPR.Columns["TransStatus"].Visible = false;
                dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
                dgvPR.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
                if (Convert.ToInt32(txtPage.Text) < ((decimal)Total / Int32.Parse(cmbShow.Text)))
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

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                ISBS_New.Purchase.PurchaseRequisitionApproval.HeaderPRApproval header = new ISBS_New.Purchase.PurchaseRequisitionApproval.HeaderPRApproval();
                header.flag(dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString());
                header.setParent2(this);
                header.Show();
                RefreshGrid();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

            ISBS_New.Purchase.PurchaseRequisitionApproval.HeaderPRApproval header = new ISBS_New.Purchase.PurchaseRequisitionApproval.HeaderPRApproval();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPR.RowCount > 0)
                {
                    header.flag(dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString());
                    header.setParent2(this);
                    header.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
          
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
            backtopageone(); //hasim 10 oct 2018
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

        private void TaskListPR_FormClosed(object sender, FormClosedEventArgs e)
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

        private void btnGenerateRFQ_Click(object sender, EventArgs e)
        {
            ISBS_New.Purchase.RFQ.RFQForm F = new ISBS_New.Purchase.RFQ.RFQForm();
            if (F.PermissionAccess(ControlMgr.New) > 0)
            {
                String PurchReqId = dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString();
                String Type = dgvPR.CurrentRow.Cells["TransType"].Value.ToString();
                string Check = "";
                Conn = ConnectionString.GetConnection();

                Query = "Select TransStatus from [dbo].[PurchRequisitionH] where [PurchReqID]='" + PurchReqId + "'";
                Cmd = new SqlCommand(Query, Conn);
                Check = Cmd.ExecuteScalar().ToString();
                if (Check == "13" || Check == "14" || Check == "21")
                {
                    F.flag2(PurchReqId, Type, "Generate");
                    //F.setParent(this);
                    F.Show();
                    RefreshGrid();
                }
                else
                {
                    MessageBox.Show("Must Approved By Purchase Manager");
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        public void SetParent(MainMenu F)
        {
            Parent = F;
        }

       

    }
}
