using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Purchase.ActionParkedItem
{
    public partial class InquiryActionParkedItem : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        String Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2;
        public static int dataShow, dataShowDetail;
        public int DataShow { get { return dataShow; } set { dataShow = value; } }

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end


        public InquiryActionParkedItem()
        {
            InitializeComponent();          
        }

        private void InquiryActionParkedItem_Load(object sender, EventArgs e)
        {
            ModeLoad();
            lblForm.Location = new Point(16, 11);
            gvheader();
        }

        private void InquiryActionParkedItem_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void InquiryActionParkedItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
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
            setCmbCriteria();
           // cmbCriteria.SelectedIndex = 0;
            txtSearch.Text = "";
            txtSearch.Enabled = true;

            RefreshGrid();
        }

        private void InquiryActionparkedItem_SelectedIndexChange(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
                txtSearch.Enabled = true;

            }
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
                new { Text = "NPP Date", Value = "NPPDate" }
            };

            cmbCriteria.DataSource = itemsCriteria;
            cmbCriteria.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void gvheader()
        {
            dgvInquiryActionParkedItem.Columns["NPPDate"].HeaderText = "NPP Date";
            dgvInquiryActionParkedItem.Columns["NPPID"].HeaderText = "NPP ID";
            dgvInquiryActionParkedItem.Columns["GoodsReceivedID"].HeaderText = "Goods Received ID";
            dgvInquiryActionParkedItem.Columns["VendID"].HeaderText = "Vendor ID";
            dgvInquiryActionParkedItem.Columns["VendName"].HeaderText = "Vendor Name";
            dgvInquiryActionParkedItem.Columns["ApprovalNotes"].HeaderText = "ApprovalNotes";
            dgvInquiryActionParkedItem.Columns["AttachedFileName"].HeaderText = "Attached File Name";
            dgvInquiryActionParkedItem.Columns["AttachedFile"].HeaderText = "Attached File";
            dgvInquiryActionParkedItem.Columns["CreatedBy"].HeaderText = "Created By";
            dgvInquiryActionParkedItem.Columns["CreatedDate"].HeaderText = "Created Date";

            dgvInquiryActionParkedItem.Columns["AttachedFileName"].Visible = false;
            dgvInquiryActionParkedItem.Columns["AttachedFile"].Visible = false;
            dgvInquiryActionParkedItem.Columns["ApprovalNotes"].Visible = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.refreshTaskList();
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

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            cmbCriteria.SelectedIndex = 0;
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
                crit = cmbCriteria.SelectedValue.ToString();
            }

            RefreshGrid();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, h.CreatedBy, h.CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode = '00') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, h.CreatedBy, h.CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode = '00' AND (h.NPPID LIKE '%" + txtSearch.Text + "%' OR h.GoodsReceivedID LIKE '%" + txtSearch.Text + "%' OR h.VendID LIKE '%" + txtSearch.Text + "%')) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("NPPID"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, h.CreatedBy, h.CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode = '00' AND h.NPPID LIKE '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("GoodsReceivedID"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, h.CreatedBy, h.CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode = '00' AND h.GoodsReceivedID LIKE '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("VendID"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, h.CreatedBy, h.CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode = '00' AND h.VendID LIKE '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("NPPDate"))
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, h.CreatedBy, h.CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode = '00' AND (CONVERT(VARCHAR(10),NPPDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), NPPDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "SELECT No, NPPDate, NPPID, GoodsReceivedID, VendID, VendName, ApprovalNotes, AttachedFileName, AttachedFile, CreatedBy, CreatedDate ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY h.NPPID desc) No, h.NPPDate, h.NPPID, h.GoodsReceivedID, h.VendID, (SELECT TOP 1 VendName FROM VendTable WHERE VendID = h.VendID) AS VendName, ";
                Query += "h.ApprovalNotes, h.AttachedFileName, h.AttachedFile, h.CreatedBy, h.CreatedDate ";
                Query += "FROM NotaPurchaseParkH h WHERE h.TransCode = '00' AND " + crit + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvInquiryActionParkedItem.AutoGenerateColumns = true;
            dgvInquiryActionParkedItem.DataSource = Dt;
            dgvInquiryActionParkedItem.Refresh();
            dgvInquiryActionParkedItem.Columns["NPPDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH WHERE TransCode = '00' ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode = '00' AND (NPPID LIKE '%" + txtSearch.Text + "%' OR GoodsReceivedID LIKE '%" + txtSearch.Text + "%' OR VendID LIKE '%" + txtSearch.Text + "%')";
            }
            else if (crit.Equals("NPPID"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode = '00' AND NPPID LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("GoodsReceivedID"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode = '00' AND GoodsReceivedID LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("VendID"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode = '00' AND VendID LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("NPPDate"))
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode = '00' AND (CONVERT(VARCHAR(10),NPPDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NPPDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')";
            }
            else
            {
                Query = "SELECT COUNT(NPPID) FROM NotaPurchaseParkH ";
                Query += "WHERE TransCode = '00' AND " + crit + " Like '%" + txtSearch.Text + "%'";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectActionParkedItem();
        }

        private void SelectActionParkedItem()
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            HeaderActionParkedItem header = new HeaderActionParkedItem();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvInquiryActionParkedItem.RowCount > 0)
                {
                    header.SetMode(dgvInquiryActionParkedItem.CurrentRow.Cells["GoodsReceivedID"].Value.ToString(), dgvInquiryActionParkedItem.CurrentRow.Cells["VendID"].Value.ToString(), dgvInquiryActionParkedItem.CurrentRow.Cells["VendName"].Value.ToString(), dgvInquiryActionParkedItem.CurrentRow.Cells["NPPID"].Value.ToString(), dgvInquiryActionParkedItem.CurrentRow.Cells["NPPDate"].Value.ToString());
                    header.SetParent(this);
                    header.Show();
                    RefreshGrid();
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

        private void InquiryActionParkedItem_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectActionParkedItem();
        }

    }
}
