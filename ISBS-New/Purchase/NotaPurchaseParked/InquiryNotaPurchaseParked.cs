using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Purchase.NotaPurchaseParked
{
    public partial class InquiryNotaPurchaseParked : MetroFramework.Forms.MetroForm
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

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public InquiryNotaPurchaseParked()     
        {
            InitializeComponent();
        }

        private void InquiryNotaPurchaseParked_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            ModeLoad();
            //lblForm.Location = new Point(16, 11);

            TransStatus = "'00','01', '04', '05'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;

            gvheader();                     
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='NotaPurchasePark' ORDER BY StatusCode ASC";

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

        private void InquiryNotaPurchaseParked_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void InquiryNotaPurchaseParked_FormClosed(object sender, FormClosedEventArgs e)
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

        //private void setCmbAction()
        //{
        //    cmbAction.DisplayMember = "Text";
        //    cmbAction.ValueMember = "Value";

        //    var itemsAction = new[] { 
        //        new { Text = "All", Value = "All" }, 
        //        new { Text = "Retur Debit", Value = "01" }, 
        //        new { Text = "Retur Tukar Barang", Value = "02" },
        //        new { Text = "New Purchase", Value = "03" }
        //    };

        //    cmbAction.DataSource = itemsAction;
        //    cmbAction.DropDownStyle = ComboBoxStyle.DropDownList;
        //}

        private void setCmbCriteria()
        {
            cmbCriteria.DisplayMember = "Text";
            cmbCriteria.ValueMember = "Value";

            var itemsCriteria = new[] { 
                new { Text = "All", Value = "All" },                             
                new { Text = "NPP Date", Value = "NPPDate" },
                new { Text = "NPP ID", Value = "NPPID" },     
                new { Text = "Goods Received ID", Value = "GoodsReceivedID" },
                new { Text = "Vendor ID", Value = "VendID" },
               // new { Text = "Action", Value = "Action" },
                new { Text = "Vendor Name", Value = "VendorName" },
                new { Text = "Status Name", Value = "StatusName" },
                new { Text = "Created By", Value = "CreatedBy" },
                new { Text = "Created Date", Value = "CreatedDate" }
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

        private void SelectedIndexChanged(object sender, EventArgs e)
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

            if (cmbCriteria.Text.Contains("Status Name"))
            {
                cmbStatusCode.Enabled = true;
            }
            else
            {
                cmbStatusCode.Enabled = false;
                cmbStatusCode.SelectedIndex = 0;
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

            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("Status Name"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else {
                txtSearch.Enabled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
         //   MainMenu f = new MainMenu();
         //   f.refreshTaskList();
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
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

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

        private void cmbShow_Keypress(object sender, KeyPressEventArgs e)
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
            string cmbSelected = Convert.ToString(cmbStatusCode.SelectedIndex);
            if (cmbSelected == "1")
            {
                cmbSelected = "00";
            }
            else if (cmbSelected == "2")
            {
                cmbSelected = "01";
            }
            else if (cmbSelected == "3")
            {
                cmbSelected = "02";
            }
            else if (cmbSelected == "4")
            {
                cmbSelected = "03";
            }
            else if (cmbSelected == "5")
            {
                cmbSelected = "04";
            }
            else
            {
                cmbSelected = "";
            }

            //Menampilkan data
            Conn = ConnectionString.GetConnection();

            int mflag;

            if (TransStatus == String.Empty)
            {
                TransStatus = "'00','01', '02'"; Limit1 = 1; Limit2 = dataShow;
            }

            Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, TransCode, StatusName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
            Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
            Query += "h.TransCode, (SELECT Deskripsi FROM TransStatusTable WHERE StatusCode = h.TransCode AND TransCode = 'NotaPurchasePark') AS StatusName, ";
            Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, h.CreatedBy, h.CreatedDate ";
            Query += "FROM NotaPurchaseParkH h LEFT JOIN VendTable v ON v.VendId = h.VendID INNER JOIN TransStatusTable b ON b.StatusCode = h.TransCode WHERE b.TransCode='NotaPurchasePark' AND h.TransCode IN (" + TransStatus + ") ";
            mflag = 1;

            if (crit == null)
            {
                Query += ") a ";
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += "AND (h.NPPID LIKE @search OR h.GoodsReceivedID LIKE @search OR h.VendID LIKE @search OR v.VendName LIKE @search OR h.CreatedBy LIKE @search)) a ";
            }
            else if (crit.Equals("NPPID"))
            {
                Query += "AND h.NPPID LIKE @search) a ";
            }
            else if (crit.Equals("CreatedBy"))
            {
                Query += "AND h.CreatedBy LIKE @search)) a ";
            }
            else if (crit.Equals("VendorName"))
            {
                Query += "AND v.VendName LIKE @search) a ";
            }
            else if (crit.Equals("GoodsReceivedID"))
            {
                Query += "AND h.GoodsReceivedID LIKE @search) a ";
            }
            else if (crit.Equals("VendID"))
            {
                Query += "AND h.VendID LIKE @search) a ";
            }
            else if (crit.Equals("NPPDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),NPPDate,120) >= @from AND CONVERT(VARCHAR(10), NPPDate,120) <= @to)) a ";
                mflag = 2;
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query += "ND (CONVERT(VARCHAR(10),CreatedDate,120) >= @from AND CONVERT(VARCHAR(10), CreatedDate,120) <= @to)) a ";
                mflag = 2;
            }
            else if (crit.Equals("StatusName"))
            {
                Query += "AND b.StatusCode LIKE @search) a ";
                mflag = 3;
            }
            else
            {
                Query += "AND " + crit + " Like @search) a ";
            }

            Query += "Where No Between @limit1 AND @limit2 ;";

            Da = new SqlDataAdapter(Query, Conn);

            Da.SelectCommand.Parameters.AddWithValue("@limit1", Limit1);
            Da.SelectCommand.Parameters.AddWithValue("@limit2", Limit2);
            switch (mflag)
            {
                case 1:
                    Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    break;
                case 2:
                    Da.SelectCommand.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd"));
                    Da.SelectCommand.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd"));
                    break;
                case 3:
                    Da.SelectCommand.Parameters.AddWithValue("@search", cmbSelected);
                    break;
            }

            Dt = new DataTable();
            Da.Fill(Dt);

            dgvNotaPurchaseParked.AutoGenerateColumns = true;
            dgvNotaPurchaseParked.DataSource = Dt;
            dgvNotaPurchaseParked.Refresh();
           // dgvNotaPurchaseParked.AutoResizeColumns();
            dgvNotaPurchaseParked.Columns["NPPDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNotaPurchaseParked.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            dgvNotaPurchaseParked.AutoResizeColumns();
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH h LEFT JOIN VendTable v ON v.VendId = h.VendID INNER JOIN TransStatusTable b ON b.StatusCode = h.TransCode WHERE b.TransCode='NotaPurchasePark' AND h.TransCode IN (" + TransStatus + ") ";
            mflag = 1;
            if (crit == null)
            {
                Query += "";
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += "AND (h.NPPID LIKE @search OR h.GoodsReceivedID LIKE @search OR h.VendID LIKE @search OR v.VendName LIKE @search OR h.CreatedBy LIKE @search) ";
            }
            else if (crit.Equals("NPPID"))
            {
                Query += "AND h.NPPID LIKE @search";
            }
            else if (crit.Equals("GoodsReceivedID"))
            {
                Query += "AND h.GoodsReceivedID LIKE @search";
            }
            else if (crit.Equals("VendID"))
            {
                Query += "AND h.VendID LIKE @search";
            }
            else if (crit.Equals("VendorName"))
            {
                Query += "AND v.VendName LIKE @search";
            }
            else if (crit.Equals("CreatedBy"))
            {
                Query += "AND h.AND CreatedBy LIKE @search";
            }
            else if (crit.Equals("StatusName"))
            {
                Query += "AND b.StatusCode LIKE @search";
                mflag = 3;
            }
            else if (crit.Equals("NPPDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),h.NPPDate,120) >= @from And CONVERT(VARCHAR(10),h.NPPDate,120) <= @to)";
                mflag = 2;
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),h.CreatedDate,120) >= @from And CONVERT(VARCHAR(10),h.CreatedDate,120) <= @to)";
                mflag = 2;
            }
            else
            {
                Query = "AND " + crit + " Like @search";
            }

            Cmd = new SqlCommand(Query, Conn);

            switch (mflag)
            {
                case 1:
                    Cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    break;
                case 2:
                    Cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd"));
                    Cmd.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd"));
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
                crit = cmbCriteria.SelectedValue.ToString();
            }

            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

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
            HeaderNotaPurchaseParked header = new HeaderNotaPurchaseParked();

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
            TransStatus = "'00','01', '02'";
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

        private void InquiryNotaPurchaseParked_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectParkedItem();
        }
       
    }
}
