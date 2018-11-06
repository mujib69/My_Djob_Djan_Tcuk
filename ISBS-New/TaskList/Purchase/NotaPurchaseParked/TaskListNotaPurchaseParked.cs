using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.TaskList.Purchase.NotaPurchaseParked
{
    public partial class TaskListNotaPurchaseParked : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2;
        public static int dataShow, dataShowDetail;
        private string TransStatus = String.Empty;
        public int DataShow { get { return dataShow; } set { dataShow = value; } }

        public TaskListNotaPurchaseParked()
        {
            InitializeComponent();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void TaskListNotaPurchaseParked_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (ControlMgr.GroupName == "Purchase Manager")
            {
                TransStatus = "'01'";
            }
            else
            {
                TransStatus = "'00','01', '04', '05'";
            }

           
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;

            gvheader();   
        }

        private void TaskListNotaPurchaseParked_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void TaskListNotaPurchaseParked_FormClosed(object sender, FormClosedEventArgs e)
        {
            // timerRefresh = null;
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

            dtFrom.Enabled = false;
            dtTo.Enabled = false;
            // setCmbAction();
            setCmbCriteria();
            // cmbAction.Enabled = false;
            // cmbAction.SelectedIndex = 0;
            // cmbCriteria.SelectedIndex = 0;
            txtSearch.Text = "";
            txtSearch.Enabled = true;

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

        private void setCmbCriteria()
        {
            cmbCriteria.DisplayMember = "Text";
            cmbCriteria.ValueMember = "Value";

            var itemsCriteria = new[] { 
                new { Text = "All", Value = "All" }, 
                new { Text = "NPP ID", Value = "NPPID" }, 
                new { Text = "Goods Received ID", Value = "GoodsReceivedID" },
                new { Text = "Vendor ID", Value = "VendID" },
               // new { Text = "Action", Value = "Action" },
                new { Text = "NPP Date", Value = "NPPDate" }
            };

            cmbCriteria.DataSource = itemsCriteria;
            cmbCriteria.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void gvheader()
        {

            dgvNotaPurchaseParked.Columns["NPPDate"].HeaderText = "NPP Date";
            dgvNotaPurchaseParked.Columns["NPPID"].HeaderText = "NPP ID";
            dgvNotaPurchaseParked.Columns["GoodsReceivedID"].HeaderText = "Goods Received ID";
            dgvNotaPurchaseParked.Columns["VendID"].HeaderText = "Vendor ID";
            dgvNotaPurchaseParked.Columns["VendName"].HeaderText = "Vendor Name";
            // dgvNotaPurchaseParked.Columns["Action"].HeaderText = "Action";
            // dgvNotaPurchaseParked.Columns["ActionCode"].HeaderText = "Action Code";
            dgvNotaPurchaseParked.Columns["TransCode"].HeaderText = "Trans Code";
            dgvNotaPurchaseParked.Columns["StatusName"].HeaderText = "Status Name";
            dgvNotaPurchaseParked.Columns["ApprovalNotes"].HeaderText = "ApprovalNotes";
            dgvNotaPurchaseParked.Columns["AttachedFileName"].HeaderText = "Attached File Name";
            dgvNotaPurchaseParked.Columns["AttachedFile"].HeaderText = "Attached File";
            dgvNotaPurchaseParked.Columns["CreatedBy"].HeaderText = "Created By";
            dgvNotaPurchaseParked.Columns["CreatedDate"].HeaderText = "Created Date";

            dgvNotaPurchaseParked.Columns["AttachedFileName"].Visible = false;
            dgvNotaPurchaseParked.Columns["AttachedFile"].Visible = false;
            // dgvNotaPurchaseParked.Columns["ActionCode"].Visible = false;
            dgvNotaPurchaseParked.Columns["TransCode"].Visible = false;

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
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
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

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    //TransStatus = "'00','01', '02'";

                    if (ControlMgr.GroupName == "Purchase Manager")
                    {
                        TransStatus = "'01'";
                    }
                    else
                    {
                        TransStatus = "'00','01', '02'";
                    }

                    Limit1 = 1; Limit2 = dataShow;
                }

                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, TransCode, StatusName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.TransCode, (SELECT Deskripsi FROM TransStatusTable WHERE StatusCode = h.TransCode AND TransCode = 'NotaPurchasePark') AS StatusName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode IN (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, TransCode, StatusName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.TransCode, (SELECT Deskripsi FROM TransStatusTable WHERE StatusCode = h.TransCode AND TransCode = 'NotaPurchasePark') AS StatusName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode IN (" + TransStatus + ") AND (h.NPPID LIKE '%" + txtSearch.Text + "%' OR h.GoodsReceivedID LIKE '%" + txtSearch.Text + "%' OR h.VendID LIKE '%" + txtSearch.Text + "%')) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("NPPID"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, TransCode, StatusName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.TransCode, (SELECT Deskripsi FROM TransStatusTable WHERE StatusCode = h.TransCode AND TransCode = 'NotaPurchasePark') AS StatusName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.NPPID LIKE '%" + txtSearch.Text + "%' AND h.TransCode IN (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("GoodsReceivedID"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, TransCode, StatusName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.TransCode, (SELECT Deskripsi FROM TransStatusTable WHERE StatusCode = h.TransCode AND TransCode = 'NotaPurchasePark') AS StatusName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.GoodsReceivedID LIKE '%" + txtSearch.Text + "%' AND h.TransCode IN (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("VendID"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, TransCode, StatusName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.TransCode, (SELECT Deskripsi FROM TransStatusTable WHERE StatusCode = h.TransCode AND TransCode = 'NotaPurchasePark') AS StatusName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.VendID LIKE '%" + txtSearch.Text + "%' AND h.TransCode IN (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("NPPDate"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, TransCode, StatusName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.TransCode, (SELECT Deskripsi FROM TransStatusTable WHERE StatusCode = h.TransCode AND TransCode = 'NotaPurchasePark') AS StatusName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode IN (" + TransStatus + ") AND (CONVERT(VARCHAR(10),NPPDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), NPPDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, TransCode, StatusName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.TransCode, (SELECT Deskripsi FROM TransStatusTable WHERE StatusCode = h.TransCode AND TransCode = 'NotaPurchasePark') AS StatusName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode IN (" + TransStatus + ") AND " + crit + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvNotaPurchaseParked.AutoGenerateColumns = true;
            dgvNotaPurchaseParked.DataSource = Dt;
            dgvNotaPurchaseParked.Refresh();
            // dgvNotaPurchaseParked.AutoResizeColumns();
            dgvNotaPurchaseParked.Columns["NPPDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH WHERE TransCode IN (" + TransStatus + ");";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode IN (" + TransStatus + ") AND (NPPID LIKE '%" + txtSearch.Text + "%' OR GoodsReceivedID LIKE '%" + txtSearch.Text + "%' OR VendID LIKE '%" + txtSearch.Text + "%') ";
            }
            else if (crit.Equals("NPPID"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode IN (" + TransStatus + ") AND NPPID LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("GoodsReceivedID"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode IN (" + TransStatus + ") AND GoodsReceivedID LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("VendID"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode IN (" + TransStatus + ") AND VendID LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("NPPDate"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode IN (" + TransStatus + ") AND (CONVERT(VARCHAR(10),NPPDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NPPDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')";
            }
            else
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode IN (" + TransStatus + ") AND " + crit + " Like '%" + txtSearch.Text + "%'";
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
                crit = cmbCriteria.SelectedValue.ToString();
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectParkedItem();
        }

        private void SelectParkedItem()
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            ISBS_New.Purchase.NotaPurchaseParked.HeaderNotaPurchaseParked header = new ISBS_New.Purchase.NotaPurchaseParked.HeaderNotaPurchaseParked();

            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvNotaPurchaseParked.RowCount > 0)
                {
                    header.SetMode("BeforeEdit", dgvNotaPurchaseParked.CurrentRow.Cells["GoodsReceivedID"].Value.ToString(), dgvNotaPurchaseParked.CurrentRow.Cells["VendID"].Value.ToString(), dgvNotaPurchaseParked.CurrentRow.Cells["VendName"].Value.ToString(), dgvNotaPurchaseParked.CurrentRow.Cells["NPPID"].Value.ToString(), dgvNotaPurchaseParked.CurrentRow.Cells["NPPDate"].Value.ToString(), dgvNotaPurchaseParked.CurrentRow.Cells["TransCode"].Value.ToString(), dgvNotaPurchaseParked.CurrentRow.Cells["ApprovalNotes"].Value.ToString());
                    header.SetParent(this);
                    header.Show();
                    //RefreshGrid();
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

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName == "Purchase Manager")
            {
                TransStatus = "'01'";
            }
            else
            {
                TransStatus = "'00','01', '02'";
            }

            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'03', '04'";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }

        private void dgvNotaPurchaseParked_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectParkedItem();
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

            //if (cmbCriteria.Text.Contains("Action"))
            //{
            //    cmbAction.Enabled = true;              
            //}
            //else
            //{
            //    cmbAction.Enabled = false;
            //    cmbAction.SelectedIndex = 0;               
            //}

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
        










    }
}
