using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.ARCollection.CollectionResult
{
    public partial class CLR_Inquery : MetroFramework.Forms.MetroForm
    {
        public CLR_Inquery()
        {
            InitializeComponent();
        }

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query, crit = null;
        int Total, Limit1, Limit2, Page, Index;
        public static int dataShow;
        private string TransStatus = String.Empty;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
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

        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10 * 1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        private void CLR_Inquery_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            addCmbCriteria();
            ModeLoad();

            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void addCmbCriteria()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName, DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'Collection_H'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbCriteria.DisplayMember = "Text";
            cmbCriteria.ValueMember = "Value";

            while (Dr.Read())
            {
                //cmbCriteria.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[1] + "" });
                cmbCriteria.Items.Add(Dr[1]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Clear();
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01', '02'";
                }
            }
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='Collection' AND StatusCode IN (" + TransStatus + ")";

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

        private void ModeLoad()
        {
            cmbShowLoad();
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            cmbCriteria.SelectedIndex = 0;
            cmbStatusCode.SelectedIndex = 0;

            cmbStatusCode.Enabled = false;

            RefreshGrid();
        }

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox]";

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

        public void RefreshGrid()
        {
            string cmbStatusSelected = cmbStatusCode.Text;

            if (cmbStatusSelected == "Collection initiated")
            {
                cmbStatusSelected = "01";
            }
            else if (cmbStatusSelected == "Collection in Collector")
            {
                cmbStatusSelected = "02";
            }
            else if (cmbStatusSelected == "Completed")
            {
                cmbStatusSelected = "03";
            }
            else if (cmbStatusSelected == "Reconciled")
            {
                cmbStatusSelected = "04";
            }
            else
            {
                cmbStatusSelected = "";
            }

            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01', '02'";
                }
            }

            Conn = ConnectionString.GetConnection();
            Query = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY CLH.CreatedDate DESC) AS [No], CLH.CL_No AS [CL No], CLH.CL_Date AS [CL Date], ";
            Query += "CLH.CL_Type AS [CL Type], CLH.Collector AS [Collector], CLD.JmlInv AS [Jumlah Invoice], CLD.TotInvAm AS [Total Invoice Amount], ";
            Query += "TST.Deskripsi AS [Status], CLH.CreatedDate AS [Created Date], CLH.CreatedBy AS [Created By], CLH.UpdatedDate AS [Updated Date], ";
            Query += "CLH.UpdatedBy AS [Updated By] ";
            Query += "FROM Collection_H CLH ";
            Query += "	LEFT JOIN (SELECT CL_No, COUNT(col.Invoice_Id) AS JmlInv, SUM(Invoice_Amount) AS TotInvAm ";
            Query += "				FROM Collection_Dtl col JOIN CustInvoice_H cus ON col.Invoice_Id = cus.Invoice_Id ";
            Query += "				GROUP BY CL_No) CLD ON CLD.CL_No = CLH.CL_No ";
            Query += "	LEFT JOIN TransStatusTable TST ON CLH.Status_Code = TST.StatusCode ";
            Query += "WHERE TransCode = 'Collection' AND CLH.Status_Code IN (" + TransStatus + ") ";
            if (crit == null)
            {
                Query += ") A ";
            }
            else if (crit == "All")
            {
                Query += "AND (CLH.CL_No LIKE '%" + txtSearch.Text + "%' OR CLH.CL_Type LIKE '%" + txtSearch.Text + "%' OR CLH.Collector LIKE '%" + txtSearch.Text + "%')) A ";
            }
            else if (crit == "CL No")
            {
                Query += "AND CLH.CL_No LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "CL Date")
            {
                Query += "AND (CONVERT(VARCHAR(10),CLH.CL_Date,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CLH.CL_Date,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            else if (crit == "CL Type")
            {
                Query += "AND CLH.CL_Type LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "Collector")
            {
                Query += "AND CLH.Collector LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "Status")
            {
                Query += "AND CLH.Status_Code LIKE '%" + cmbStatusSelected + "%') A ";
            }
            Query += "WHERE A.[No] BETWEEN " + Limit1 + " AND " + Limit2;

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvCLResult.AutoGenerateColumns = true;
            dgvCLResult.DataSource = Dt;
            dgvCLResult.Refresh();
            dgvCLResult.AutoResizeColumns();
            dgvCLResult.Columns["CL Date"].DefaultCellStyle.Format = "  dd/MM/yyyy  ";
            dgvCLResult.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvCLResult.Columns["Updated Date"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
            Query = " SELECT COUNT(CL_No) FROM Collection_H WHERE Status_Code IN (" + TransStatus + ") ";
            if (crit == null)
                Query += "";
            else if (crit == "All")
                Query += "AND (CL_No LIKE '%" + txtSearch.Text + "%' OR CL_Type LIKE '%" + txtSearch.Text + "%' OR Collector LIKE '%" + txtSearch.Text + "%')";
            else if (crit == "CL No")
                Query += "AND CL_No LIKE '%" + txtSearch.Text + "%'";
            else if (crit == "CL Date")
                Query += "AND (CONVERT(VARCHAR(10),CL_Date,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CL_Date,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')";
            else if (crit == "CL Type")
                Query += "AND CL_Type LIKE '%" + txtSearch.Text + "%'";
            else if (crit == "Collector")
                Query += "AND Collector LIKE '%" + txtSearch.Text + "%'";
            else if (crit == "Status")
                Query += "AND Status_Code LIKE '%" + cmbStatusSelected + "%'";
            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page;
            Conn.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            crit = null;
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name == "CLR_Form")
                    Application.OpenForms[i].Close();
            //    else if (Application.OpenForms[i].Name == "AddGI")
            //        Application.OpenForms[i].Close();
            //    else if (Application.OpenForms[i].Name == "AddItem")
            //        Application.OpenForms[i].Close();
            //    else if (Application.OpenForms[i].Name == "AddWH")
            //        Application.OpenForms[i].Close();
            }
            MainMenu f = new MainMenu();
            f.refreshTaskList();
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

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void SelectCL()
        {
            ARCollection.CollectionResult.CLR_Form f = new ARCollection.CollectionResult.CLR_Form();
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvCLResult.CurrentRow == null)
                {
                    MessageBox.Show("Maaf List Masih Kosong");
                    return;
                }
                else
                {
                    string CL_NO = dgvCLResult.CurrentRow.Cells["CL No"].Value.ToString();
                    f.SetMode("BeforeEdit", CL_NO);
                    f.setParent(this);
                    f.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectCL();
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

        private void dgvCLResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectCL();
            }
        }

        private bool CheckOpened(string name)
        {
            FormCollection FC = Application.OpenForms;
            foreach (Form frm in FC)
            {
                if (frm.Name == name)
                {
                    return true;
                }
            }
            return false;
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

            if (cmbCriteria.Text.Contains("Status"))
            {
                cmbStatusCode.Enabled = true;
            }
            else
            {
                cmbStatusCode.Enabled = false;
                cmbStatusCode.SelectedIndex = 0;
            }

            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("Status"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch.PerformClick();
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01', '02'";
            addCmbStatusCode();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'03', '04'";
            addCmbStatusCode();
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }
    }
}
