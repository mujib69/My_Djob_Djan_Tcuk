using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.TaskList.Purchase
{
    public partial class TaskListCanvasSheet : MetroFramework.Forms.MetroForm
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
        List<ISBS_New.Purchase.CanvasSheet.FormCanvasSheet2> ListFormCanvasSheet = new List<ISBS_New.Purchase.CanvasSheet.FormCanvasSheet2>();
        public List<string> VendorId = new List<string>();
        public List<string> PQId = new List<string>();

      

        string TransStatus = "";

        private static int countCS;

        public static int showData;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public TaskListCanvasSheet()
        {
            InitializeComponent();
        }

        private void CanvasSheet_Load(object sender, EventArgs e)
        {
            TransStatus = "'01'";
            addCmbCrit();
            ModeLoad();
            //lblForm.Location = new Point(16, 11);
            //setTimer();

            //btnOnProgress.BackColor = Color.DeepSkyBlue;
            //btnOnProgress.ForeColor = Color.White;
            //btnCompleted.BackColor = Color.LightGray;
            //btnCompleted.ForeColor = Color.Black;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01'"; Limit1 = 1; Limit2 = ShowData;
                }
                Query = "Select a.No,a.CanvasId 'CS No',a.CanvasDate 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, c.OrderDate, b.Deskripsi ";
                Query += "From [dbo].[CanvasSheetH] a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
                Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where a.TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select a.No,a.CanvasId 'CS No',a.CanvasDate 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, c.OrderDate, b.Deskripsi ";
                Query += "From [dbo].[CanvasSheetH] a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
                Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where (CanvasId like '%" + txtSearch.Text + "%' or a.PurchReqId like '%" + txtSearch.Text + "%' or b.Deskripsi like '%" + txtSearch.Text + "%') and a.TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
            }
            else if (crit.Equals("Status"))
            {
                Query = "Select a.No,a.CanvasId 'CS No',a.CanvasDate 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, c.OrderDate, b.Deskripsi ";
                Query += "From [dbo].[CanvasSheetH] a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
                Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where (b.Deskripsi like '%" + txtSearch.Text + "%') and a.TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
            }
            else if (crit.Equals("CS Date"))
            {
                Query = "Select a.No,a.CanvasId 'CS No',a.CanvasDate 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, c.OrderDate, b.Deskripsi ";
                Query += "From [dbo].[CanvasSheetH] a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
                Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where ((CONVERT(VARCHAR(10),CanvasDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CanvasDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (a.CanvasId like '%" + txtSearch.Text + "%' or a.PurchReqId like '%" + txtSearch.Text + "%')) and a.TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "Select a.No,a.CanvasId 'CS No',a.CanvasDate 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, c.OrderDate, b.Deskripsi ";
                Query += "From [dbo].[CanvasSheetH] a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
                Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where ((CONVERT(VARCHAR(10),a.CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), a.CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (a.CanvasId like '%" + txtSearch.Text + "%' or a.PurchReqId like '%" + txtSearch.Text + "%')) and a.TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
            }
            else if (crit.Equals("Updated Date"))
            {
                Query = "Select a.No,a.CanvasId 'CS No',a.CanvasDate 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, c.OrderDate, b.Deskripsi ";
                Query += "From [dbo].[CanvasSheetH] a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
                Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where ((CONVERT(VARCHAR(10),a.UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), a.UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (a.CanvasId like '%" + txtSearch.Text + "%' or a.PurchReqId like '%" + txtSearch.Text + "%')) and a.TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
            }
            else if (crit.Equals("PR Date"))
            {
                Query = "Select a.No,a.CanvasId 'CS No',a.CanvasDate 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, c.OrderDate, b.Deskripsi ";
                Query += "From [dbo].[CanvasSheetH] a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
                Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where ((CONVERT(VARCHAR(10),c.OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),c.OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (a.CanvasId like '%" + txtSearch.Text + "%' or a.PurchReqId like '%" + txtSearch.Text + "%')) and a.TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
            }
            else
            {
                if (crit == "CS No")
                {
                    crit = "CanvasId";
                }
                else if (crit == "PR No")
                {
                    crit = "PurchReqId";
                }
                else if (crit == "PR Type")
                {
                    crit = "TransType";
                }
                else if (crit == "Created By")
                {
                    crit = "CreatedBy";
                }
                else if (crit == "Updated By")
                {
                    crit = "UpdatedBy";
                }

                Query = "Select a.No,a.CanvasId 'CS No',a.CanvasDate 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
                Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, c.OrderDate, b.Deskripsi ";
                Query += "From [dbo].[CanvasSheetH] a ";
                Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
                Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where a." + crit + " Like '%" + txtSearch.Text + "%' and a.TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + ";";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPQ.AutoGenerateColumns = true;
            dgvPQ.DataSource = Dt;
            dgvPQ.Refresh();
            gvHeader();
            dgvPQ.Columns["CS Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["Updated Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] where TransStatus in (" + TransStatus + ");";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] a Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode Where ";
                Query += "(CanvasId like '%" + txtSearch.Text + "%' or PurchReqId like '%" + txtSearch.Text + "%' or Deskripsi like '%" + txtSearch.Text + "%') and TransStatus in (" + TransStatus + ");";
            }
            else if (crit.Equals("Status"))
            {
                Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] a Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode Where ";
                Query += "b.Deskripsi like '%" + txtSearch.Text + "%' and TransStatus in (" + TransStatus + ");";
            }
            else if (crit.Equals("CS Date"))
            {
                Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] a Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode Where ";
                Query += "((CONVERT(VARCHAR(10),CanvasDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CanvasDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (CanvasId like '%" + txtSearch.Text + "%' or PurchReqId like '%" + txtSearch.Text + "%' or Deskripsi like '%" + txtSearch.Text + "%')) and TransStatus in (" + TransStatus + ") ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] a Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode Where ";
                Query += "((CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (CanvasId like '%" + txtSearch.Text + "%' or PurchReqId like '%" + txtSearch.Text + "%' or Deskripsi like '%" + txtSearch.Text + "%')) and TransStatus in (" + TransStatus + ") ;";
            }
            else if (crit.Equals("Updated Date"))
            {
                Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] a Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode Where ";
                Query += "((CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (CanvasId like '%" + txtSearch.Text + "%' or PurchReqId like '%" + txtSearch.Text + "%' or Deskripsi like '%" + txtSearch.Text + "%')) and TransStatus in (" + TransStatus + ") ;";
            }
            else if (crit.Equals("PR Date"))
            {
                Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] a Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where ";
                Query += "((CONVERT(VARCHAR(10),c.OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), c.OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (CanvasId like '%" + txtSearch.Text + "%' or a.PurchReqId like '%" + txtSearch.Text + "%' or b.Deskripsi like '%" + txtSearch.Text + "%')) and a.TransStatus in (" + TransStatus + ") ;";
            }
            else
            {
                if (crit == "CS No")
                {
                    crit = "CanvasId";
                }
                else if (crit == "PR No")
                {
                    crit = "PurchReqId";
                }
                else if (crit == "PR Type")
                {
                    crit = "TransType";
                }
                else if (crit == "Created By")
                {
                    crit = "CreatedBy";
                }
                else if (crit == "Updated By")
                {
                    crit = "UpdatedBy";
                }
                Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] Where ";
                Query += crit + " Like '%" + txtSearch.Text + "%' and TransStatus in (" + TransStatus + ") ;";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (cmbShow.Text == String.Empty)
                cmbShow.SelectedItem = ShowData.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

            dgvPQ.AutoResizeColumns();

        }

        public static int ShowData { get { return showData; } set { showData = value; } }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select [DisplayName] From [User].[Table] Where SchemaName = 'dbo' And TableName = 'CanvasSheet' order by OrderNo";

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
            ShowData = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            cmbCriteria.SelectedIndex = 0;

            RefreshGrid();
        }

        private void gvHeader()
        {
            //dgvPQ.Columns["CanvasId"].HeaderText = "CS No";
            //dgvPQ.Columns["CanvasDate"].HeaderText = "Date";
            //dgvPQ.Columns["PurchReqId"].HeaderText = "PR ID";
            //dgvPQ.Columns["TransType"].HeaderText = "Transaction Type";
            //dgvPQ.Columns["Status"].HeaderText = "Status";
            //dgvPQ.Columns["CreatedDate"].HeaderText = "Created Date";
            //dgvPQ.Columns["CreatedBy"].HeaderText = "Created By";
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

        private void SelectData()
        {
            ISBS_New.Purchase.CanvasSheet.FormCanvasSheet2 FormCanvasSheet = new ISBS_New.Purchase.CanvasSheet.FormCanvasSheet2();
            if (FormCanvasSheet.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPQ.RowCount > 0)
                {
                    if (TransStatus == "'01'")
                    {
                        FormCanvasSheet.SetParent2(this);
                        //FormCanvasSheet.Show();                   
                        FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
                        FormCanvasSheet.GetDataHeader();
                        FormCanvasSheet.ShowDialog();

                        //RefreshGrid();
                    }
                    else
                    {
                        FormCanvasSheet.SetParent2(this);
                        FormCanvasSheet.ApproveStatus = true;
                        FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
                        FormCanvasSheet.GetDataHeader();
                        FormCanvasSheet.ShowDialog();
                    }
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void TaskListCanvasSheet_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        public static int CountCS { get { return countCS; } set { countCS = value; } }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'02','03'";
            //btnOnProgress.BackColor = Color.LightGray;
            //btnOnProgress.ForeColor = Color.Black;
            //btnCompleted.BackColor = Color.DeepSkyBlue;
            //btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01'";
            //btnOnProgress.BackColor = Color.DeepSkyBlue;
            //btnOnProgress.ForeColor = Color.White;
            //btnCompleted.BackColor = Color.LightGray;
            //btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                ISBS_New.Purchase.CanvasSheet.FormCanvasSheet2 FormCanvasSheet = new ISBS_New.Purchase.CanvasSheet.FormCanvasSheet2();
                FormCanvasSheet.SetParent2(this);
                //FormCanvasSheet.Show();
                FormCanvasSheet.ShowDialog();
                ListFormCanvasSheet.Add(FormCanvasSheet);
                FormCanvasSheet.ModeNew();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            ISBS_New.Purchase.CanvasSheet.FormCanvasSheet2 FormCanvasSheet = new ISBS_New.Purchase.CanvasSheet.FormCanvasSheet2();
            if (FormCanvasSheet.PermissionAccess(ControlMgr.View) > 0)
            {
                try
                {
                    if (dgvPQ.RowCount > 0)
                    {
                        if (dgvPQ.CurrentRow.Cells["Status"].Value.ToString().ToUpper() == "APPROVED")
                        {
                            MessageBox.Show("Dokumen Canvas Sheet sudah di approved.");
                            return;
                        }

                        if (ControlMgr.GroupName == "Purchase Manager")
                        {
                            FormCanvasSheet.SetParent2(this);
                            //FormCanvasSheet.Show();                          
                            FormCanvasSheet.ApproveStatus = true;
                            FormCanvasSheet.txtCSNumber.Text = dgvPQ.CurrentRow.Cells["CS No"].Value.ToString();
                            FormCanvasSheet.GetDataHeader();
                            FormCanvasSheet.ModeEdit();
                            FormCanvasSheet.ShowDialog();

                            if (ConnectionString.GetConnection().State == ConnectionState.Closed)
                            {
                                Conn.Open();
                            }
                            else
                                Conn.Close();
                            Query = "select count(*) from [dbo].[CanvasSheetH] where [TransStatus] = '01'";
                            Cmd = new SqlCommand(Query, Conn);
                            Conn.Open();
                            CountCS = Int32.Parse(Cmd.ExecuteScalar().ToString());
                            lblTotal.Text = "Total Rows : " + CountCS.ToString();
                            RefreshGrid();
                        }
                        else
                        {
                            MessageBox.Show("User yang bisa melakukan approve hanya (Purchase Manager).");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data belum dipilih.");
                    }
                    //if (dgvPQ.RowCount > 0)
                    //{
                    //    FormCanvasSheet2 FormCanvasSheet = new FormCanvasSheet2();
                    //    //header.SetParent(this);
                    //    FormCanvasSheet.Show();
                    //    FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
                    //    FormCanvasSheet.GetDataHeader();

                    //    //RefreshGrid();
                    //}

                    //if (dgvPQ.RowCount > 0)
                    //{
                    //    Query = "Select StatusApproval from [dbo].[CanvasSheetH] where CanvasId = '" + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + "'";
                    //    Conn = ConnectionString.GetConnection();
                    //    SqlCommand cmd = new SqlCommand(Query, Conn);
                    //    if(cmd.ExecuteScalar().ToString() == "YES")
                    //    {
                    //        MessageBox.Show("CanvasSheet " + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + " sudah di approve");
                    //    }
                    //    else
                    //    {
                    //        ApproveCanvasSheet FormCanvasSheet = new ApproveCanvasSheet();
                    //        FormCanvasSheet.Show();
                    //        FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
                    //        FormCanvasSheet.GetDataHeader();
                    //    }
                    //}
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
            SelectData();
        }

        private void btnGeneratePOPA_Click(object sender, EventArgs e)
        {
            //stv edit start
            string CanvasId = dgvPQ.CurrentRow.Cells["CS No"].Value.ToString();

            Query = "SELECT ReffId from [PurchH] Where ReffId = '" + CanvasId + "'";
            using (Conn = ConnectionString.GetConnection())
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    MessageBox.Show("Canvas Sheets sudah direlease PO/PA");
                    return;
                }
            }

            Query = "SELECT CanvasId from [PurchAgreementH] Where CanvasId = '" + CanvasId + "'";
            using (Conn = ConnectionString.GetConnection())
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    MessageBox.Show("Canvas Sheets sudah direlease PO/PA");
                    return;
                }
            }
            //stv edit end

            Conn = ConnectionString.GetConnection();

            if (dgvPQ.RowCount > 0)
            {
                if (dgvPQ.CurrentRow.Cells["Status"].Value.ToString() != "APPROVED")
                {
                    //MessageBox.Show("Canves Sheet : " + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + " belum di approve..");
                    MessageBox.Show("Canvass Sheet harus diapprove terlebih dahulu");
                }
                else
                {

                    CanvasId = dgvPQ.CurrentRow.Cells["CS No"].Value.ToString();

                    if (dgvPQ.CurrentRow.Cells["PR Type"].Value.ToString() == "FIX")
                    {
                        ISBS_New.Purchase.PurchaseOrderNew.POForm POForm = new ISBS_New.Purchase.PurchaseOrderNew.POForm();

                        //begin
                        //updated by : joshua
                        //updated date : 21 feb 2018
                        //description : check permission access
                        if (POForm.PermissionAccess(ControlMgr.New) > 0)
                        {
                            string QuotID = "";

                            Query = "Select ReffId2 From PurchH Where ReffID = '" + CanvasId + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();

                            while (Dr.Read())
                            {
                                if (QuotID == "")
                                {
                                    QuotID = "'" + Dr[0].ToString() + "'";
                                }
                                else
                                {
                                    QuotID += ",";
                                    QuotID += "'" + Dr[0].ToString() + "'";
                                }

                            }
                            Dr.Close();
                            if (QuotID == "")
                            {
                                QuotID = "''";
                            }

                            Query = "Select DISTINCT PurchQuotId from [dbo].[CanvasSheetD] Where CanvasId = '" + CanvasId + "' and StatusApproval = 'YES' and Qty > '0' and PurchQuotId NOT IN(" + QuotID + ")";

                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();

                            if (Dr.Read())
                            {
                                POForm.SetMode("Generate", "", CanvasId);
                                POForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Tidak ada Purchase Quotation yang sudah di Approve");
                            }
                            Dr.Close();
                            //if (Purchase.PurchaseOrderNew.PQ.PQID1.Count == 0)
                            //POForm.Close();
                        }
                        else
                        {
                            MessageBox.Show(ControlMgr.PermissionDenied);
                        }
                        //end                        
                    }
                    else
                    {


                        ISBS_New.Purchase.PurchaseAgreement.PAForm PAForm = new ISBS_New.Purchase.PurchaseAgreement.PAForm();

                        //begin
                        //updated by : joshua
                        //updated date : 21 feb 2018
                        //description : check permission access
                        if (PAForm.PermissionAccess(ControlMgr.New) > 0)
                        {
                            PAForm.SetMode("New", CanvasId, "");
                            if (ISBS_New.Purchase.PurchaseAgreement.SelectPQ.PQId != "" && ISBS_New.Purchase.PurchaseAgreement.SelectPQ.VendId != "")
                            {
                                ISBS_New.Purchase.PurchaseAgreement.PAInq PAInq = new ISBS_New.Purchase.PurchaseAgreement.PAInq();
                                PAForm.SetParent(PAInq);
                                PAForm.Show();
                            }
                            else
                            {
                                ISBS_New.Purchase.PurchaseAgreement.SelectPQ f = new ISBS_New.Purchase.PurchaseAgreement.SelectPQ();
                                f.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show(ControlMgr.PermissionDenied);
                        }
                        //end
                    }
                }
            }
            Conn.Close();
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
                        Index = dgvPQ.CurrentRow.Index;
                        string CanvasId = dgvPQ.Rows[Index].Cells["CS No"].Value == null ? "" : dgvPQ.Rows[Index].Cells["CS No"].Value.ToString();
                        //string TransType = dgvPR.Rows[Index].Cells["TransType"].Value == null ? "" : dgvPR.Rows[Index].Cells["TransType"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        DialogResult dr = MessageBox.Show("CanvasId = " + CanvasId + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string Check = "";
                            Conn = ConnectionString.GetConnection();

                            //Query = "Select TransStatus from [dbo].[CanvasSheetH] where [CanvasId]='" + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + "';";
                            //Cmd = new SqlCommand(Query, Conn);
                            //Check = Cmd.ExecuteScalar().ToString();
                            //if (Check != "01")
                            //{
                            //    MessageBox.Show("CanvasId = " + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                            //    Conn.Close();
                            //    return;
                            //}

                            #region Delete Invent_Purch_Qty
                            string QueryTemp = "";
                            string FullItemId = "";
                            string Unit = "";
                            string UoM = "";
                            decimal ConvRatio = 0;
                            decimal QtyDeleted = 0;
                            decimal QtyUoMDeleted = 0;
                            decimal QtyAltDeleted = 0;
                            string PRType = "";
                            string CSNo = "";
                            int JumlahCS = 0;
                            VendorId.Clear();

                            PRType = dgvPQ.CurrentRow.Cells["PR Type"].Value == null ? "" : dgvPQ.CurrentRow.Cells["PR Type"].Value.ToString();
                            CSNo = dgvPQ.CurrentRow.Cells["CS No"].Value == null ? "" : dgvPQ.CurrentRow.Cells["CS No"].Value.ToString();

                            Query = "Select distinct PurchQuotId from [ISBS-NEW4].[dbo].[CanvasSheetD] where CanvasId = '" + CSNo + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                //VendorId.Add(Dr["VendId"].ToString());
                                PQId.Add(Dr["PurchQuotId"].ToString());
                            }

                            for (int i = 0; i < PQId.Count; i++)
                            {
                                Query = "SELECT COUNT (CanvasId) FROM [ISBS-NEW4].[dbo].[CanvasSheetD] WHERE [CanvasId]='" + CSNo + "' and PurchQuotId = '" + PQId[i] + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                JumlahCS = (int)Cmd.ExecuteScalar();

                                if (PRType != "AMOUNT")
                                {
                                    for (int j = 1; j <= JumlahCS; j++)
                                    {
                                        Query = "Select FullItemID, Qty, Unit From CanvasSheetD Where CanvasId = '" + CSNo + "' and PurchQuotId = '" + PQId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                                        Cmd = new SqlCommand(Query, Conn, Trans);
                                        Dr = Cmd.ExecuteReader();
                                        while (Dr.Read())
                                        {
                                            FullItemId = Dr["FullItemID"].ToString();
                                            Unit = Dr["Unit"].ToString();
                                            QtyDeleted = decimal.Parse(Dr["Qty"].ToString());
                                            ConvRatio = 0;

                                            QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                                            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                                            UoM = Cmd.ExecuteScalar().ToString();

                                            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                                            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                                            ConvRatio = (decimal)Cmd.ExecuteScalar();

                                            if (Unit == UoM)
                                            {
                                                QtyUoMDeleted = QtyDeleted;
                                                QtyAltDeleted = QtyDeleted * ConvRatio;
                                            }
                                            else
                                            {
                                                QtyAltDeleted = QtyDeleted;
                                                QtyUoMDeleted = QtyDeleted / ConvRatio;
                                            }

                                            Query = "Update Invent_Purchase_Qty Set [PR_Approved2_UoM] = PR_Approved2_UoM + " + QtyUoMDeleted + ", PR_Approved2_Alt = PR_Approved2_Alt + " + QtyAltDeleted + ", [PR_CS_Issued_UoM] = [PR_CS_Issued_UoM] - " + QtyUoMDeleted + ", [PR_CS_Issued_Alt] = [PR_CS_Issued_Alt] + " + QtyAltDeleted + " Where FullItemID = '" + FullItemId + "'";
                                            Cmd = new SqlCommand(Query, Conn, Trans);
                                            Cmd.ExecuteNonQuery();
                                            Query = "";
                                        }
                                        Dr.Close();
                                    }
                                }
                                if (PRType == "AMOUNT")
                                {
                                    for (int j = 1; j <= JumlahCS; j++)
                                    {
                                        Query = "Select FullItemID, CSAmount From CanvasSheetD Where CanvasId = '" + CSNo + "' and PurchQuotId = '" + PQId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                                        Cmd = new SqlCommand(Query, Conn, Trans);
                                        Dr = Cmd.ExecuteReader();
                                        while (Dr.Read())
                                        {
                                            FullItemId = Dr["FullItemID"].ToString();
                                            QtyDeleted = decimal.Parse(Dr["CSAmount"].ToString());

                                            Query = "Update Invent_Purchase_Qty Set [PR_Approved2_Amount] = PR_Approved2_Amount + " + QtyDeleted + ", [PR_CS_Issued_Amount] = [PR_CS_Issued_Amount] - " + QtyDeleted + " Where FullItemID = '" + FullItemId + "'";
                                            Cmd = new SqlCommand(Query, Conn, Trans);
                                            Cmd.ExecuteNonQuery();
                                            Query = "";
                                        }
                                        Dr.Close();
                                    }
                                }
                            }
                            #endregion

                            //delete header
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[CanvasSheetH] where [CanvasId]='" + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + "';";

                            //delete detail
                            Query += "Delete from [dbo].[CanvasSheetD] where CanvasId ='" + CanvasId + "';";

                            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("CanvasId = " + CanvasId.ToUpper() + "\n" + "Data berhasil dihapus.");

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
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

        private void dgvPQ_DoubleClick(object sender, EventArgs e)
        {
            SelectData();
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

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (timerRefresh == null)
            {

            }
            else
            {
                //     RefreshGrid();
            }
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
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
        }
       
    }
}
