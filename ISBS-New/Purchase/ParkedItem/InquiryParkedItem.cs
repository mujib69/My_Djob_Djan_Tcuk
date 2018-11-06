using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Purchase.ParkedItem
{
    public partial class InquiryParkedItem : MetroFramework.Forms.MetroForm
    {
        public InquiryParkedItem()
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
        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Limit1D, Limit2D, TotalD, Page1D, Page2D;
        public static int dataShow, dataShowDetail;
        public int DataShow { get { return dataShow; } set { dataShow = value; } }
        public int DataShowDetail { get { return dataShowDetail; } set { dataShowDetail = value; } }
      
        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'GoodsReceivedH2'";

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
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            cmbCriteria.SelectedIndex = 0;

            RefreshGrid();
        }

        private void ModeLoadDetail()
        {
            cmbShowLoadDetail();
            dataShowDetail = Int32.Parse(cmbShowD.Text);
            Limit1D = 1;
            Limit2D = Int32.Parse(cmbShowD.Text);
            Page1D = 1;
            txtPageD.Text = "1";

            RefreshGridDetail();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                Query = "SELECT * FROM( ";
                Query += "SELECT ROW_NUMBER() OVER (ORDER BY GoodsReceivedDate asc, GoodsReceivedNumber asc) No,* "; 
                Query += "FROM( ";
                Query += "SELECT h.GoodsReceivedDate, h.GoodsReceivedId AS GoodsReceivedNumber, h.ReceiptOrderDate, "; 
                Query += "h.ReceiptOrderId AS ReceiptOrderNumber, h.VendId AS VendorID, h.VendorName ";
                Query += "FROM  GoodsReceivedH h "; 
                Query += "INNER JOIN GoodsReceivedD d ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' ";
                Query += "GROUP By h.GoodsReceivedDate, h.GoodsReceivedId, h.ReceiptOrderDate, "; 
                Query += "h.ReceiptOrderId, h.VendId, h.VendorName) AS a) AS b ";
                Query += "WHERE b.No Between " + Limit1 + " and " + Limit2 + "";
            }
            else if (crit.Equals("All"))
            {
                Query ="SELECT * FROM( ";
                Query += "SELECT ROW_NUMBER() OVER (ORDER BY GoodsReceivedDate asc, GoodsReceivedNumber asc) No,* "; 
                Query += "FROM( ";
                Query += "SELECT h.GoodsReceivedDate, h.GoodsReceivedId AS GoodsReceivedNumber, h.ReceiptOrderDate, "; 
                Query += "h.ReceiptOrderId AS ReceiptOrderNumber, h.VendId AS VendorID, h.VendorName ";
                Query += "FROM  GoodsReceivedH h "; 
                Query += "INNER JOIN GoodsReceivedD d ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' ";
                Query += "GROUP By h.GoodsReceivedDate, h.GoodsReceivedId, h.ReceiptOrderDate, "; 
                Query += "h.ReceiptOrderId, h.VendId, h.VendorName) AS a ";
                Query += "WHERE a.GoodsReceivedNumber like '%" + txtSearch.Text + "%' OR a.ReceiptOrderNumber like '%" + txtSearch.Text + "%') AS b ";
                Query += "WHERE b.No Between " + Limit1 + " and " + Limit2 + "";

            }
            else if (crit.Equals("GoodsReceivedNumber"))
            {
                Query = "SELECT * FROM( ";
                Query += "SELECT ROW_NUMBER() OVER (ORDER BY GoodsReceivedDate asc, GoodsReceivedNumber asc) No,* ";
                Query += "FROM( ";
                Query += "SELECT h.GoodsReceivedDate, h.GoodsReceivedId AS GoodsReceivedNumber, h.ReceiptOrderDate, ";
                Query += "h.ReceiptOrderId AS ReceiptOrderNumber, h.VendId AS VendorID, h.VendorName ";
                Query += "FROM  GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' ";
                Query += "GROUP By h.GoodsReceivedDate, h.GoodsReceivedId, h.ReceiptOrderDate, ";
                Query += "h.ReceiptOrderId, h.VendId, h.VendorName) AS a ";
                Query += "WHERE a.GoodsReceivedNumber like '%" + txtSearch.Text + "%') AS b ";
                Query += "WHERE b.No Between " + Limit1 + " and " + Limit2 + "";
            }
            else if (crit.Equals("ReceiptOrderNumber"))
            {
                Query = "SELECT * FROM( ";
                Query += "SELECT ROW_NUMBER() OVER (ORDER BY GoodsReceivedDate asc, GoodsReceivedNumber asc) No,* ";
                Query += "FROM( ";
                Query += "SELECT h.GoodsReceivedDate, h.GoodsReceivedId AS GoodsReceivedNumber, h.ReceiptOrderDate, ";
                Query += "h.ReceiptOrderId AS ReceiptOrderNumber, h.VendId AS VendorID, h.VendorName ";
                Query += "FROM  GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' ";
                Query += "GROUP By h.GoodsReceivedDate, h.GoodsReceivedId, h.ReceiptOrderDate, ";
                Query += "h.ReceiptOrderId, h.VendId, h.VendorName) AS a ";
                Query += "WHERE a.ReceiptOrderNumber like '%" + txtSearch.Text + "%') AS b ";
                Query += "WHERE b.No Between " + Limit1 + " and " + Limit2 + "";
            }
            else if (crit.Equals("GoodsReceivedDate"))
            {
                Query = "SELECT * FROM( ";
                Query += "SELECT ROW_NUMBER() OVER (ORDER BY GoodsReceivedDate asc, GoodsReceivedNumber asc) No,* ";
                Query += "FROM( ";
                Query += "SELECT h.GoodsReceivedDate, h.GoodsReceivedId AS GoodsReceivedNumber, h.ReceiptOrderDate, ";
                Query += "h.ReceiptOrderId AS ReceiptOrderNumber, h.VendId AS VendorID, h.VendorName ";
                Query += "FROM  GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' ";
                Query += "GROUP By h.GoodsReceivedDate, h.GoodsReceivedId, h.ReceiptOrderDate, ";
                Query += "h.ReceiptOrderId, h.VendId, h.VendorName) AS a ";
                Query += "WHERE (CONVERT(VARCHAR(10),a.GoodsReceivedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), a.GoodsReceivedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) AS b ";
                Query += "WHERE b.No Between " + Limit1 + " and " + Limit2 + "";

            }
            else
            {
                Query = "SELECT * FROM( ";
                Query += "SELECT ROW_NUMBER() OVER (ORDER BY GoodsReceivedDate asc, GoodsReceivedNumber asc) No,* ";
                Query += "FROM( ";
                Query += "SELECT h.GoodsReceivedDate, h.GoodsReceivedId AS GoodsReceivedNumber, h.ReceiptOrderDate, ";
                Query += "h.ReceiptOrderId AS ReceiptOrderNumber, h.VendId AS VendorID, h.VendorName ";
                Query += "FROM  GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' ";
                Query += "GROUP By h.GoodsReceivedDate, h.GoodsReceivedId, h.ReceiptOrderDate, ";
                Query += "h.ReceiptOrderId, h.VendId, h.VendorName) AS a ";
                Query += crit + " like '%" + txtSearch.Text + "%') AS b ";
                Query += "WHERE b.No Between " + Limit1 + " and " + Limit2 + "";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvParkedItemH.AutoGenerateColumns = true;
            dgvParkedItemH.DataSource = Dt;
            dgvParkedItemH.Refresh();
            dgvParkedItemH.AutoResizeColumns();
            dgvParkedItemH.ReadOnly = true;
            dgvParkedItemH.Columns["GoodsReceivedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvParkedItemH.Columns["ReceiptOrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();
            

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "SELECT Count(GoodsReceivedId) FROM ( ";
                Query += "Select Count(h.GoodsReceivedId) AS GoodsReceivedId ";
                Query += "From GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d "; 
                Query += "ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' ";
                Query += "Group By h.GoodsReceivedId) a ";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT Count(GoodsReceivedId) FROM ( ";
                Query += "Select Count(h.GoodsReceivedId) AS GoodsReceivedId ";
                Query += "From GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d "; 
                Query += "ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' AND ( h.GoodsReceivedId like '%" + txtSearch.Text + "%' OR h.ReceiptOrderId like '%" + txtSearch.Text + "%') ";
                Query += "Group By h.GoodsReceivedId) a ";
            
            }
            else if (crit.Equals("GoodsReceivedNumber"))
            {
                Query = "SELECT Count(GoodsReceivedId) FROM ( ";
                Query += "Select Count(h.GoodsReceivedId) AS GoodsReceivedId ";
                Query += "From GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d ";
                Query += "ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' AND h.GoodsReceivedId like '%" + txtSearch.Text + "%' ";
                Query += "Group By h.GoodsReceivedId) a ";
            }
            else if (crit.Equals("ReceiptOrderNumber"))
            {

                Query = "SELECT Count(GoodsReceivedId) FROM ( ";
                Query += "Select Count(h.GoodsReceivedId) AS GoodsReceivedId ";
                Query += "From GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d ";
                Query += "ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' AND h.ReceiptOrderId like '%" + txtSearch.Text + "%' ";
                Query += "Group By h.GoodsReceivedId) a ";
            }
            else if (crit.Equals("GoodsReceivedDate"))
            {
                Query = "SELECT Count(GoodsReceivedId) FROM ( ";
                Query += "Select Count(h.GoodsReceivedId) AS GoodsReceivedId ";
                Query += "From GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d ";
                Query += "ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' AND ((CONVERT(VARCHAR(10),h.GoodsReceivedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.GoodsReceivedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) ";
                Query += "Group By h.GoodsReceivedId) a ";
          
            }
            else
            {
                Query = "SELECT Count(GoodsReceivedId) FROM ( ";
                Query += "Select Count(h.GoodsReceivedId) AS GoodsReceivedId ";
                Query += "From GoodsReceivedH h ";
                Query += "INNER JOIN GoodsReceivedD d ";
                Query += "ON h.GoodsReceivedId = d.GoodsReceivedId ";
                Query += "WHERE d.ActionCodeStatus = '02' AND " + crit + " like '%" + txtSearch.Text + "%' ";
                Query += "Group By h.GoodsReceivedId) a ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (dataShow != 0)
                Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        public void RefreshGridDetail()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();

            String GoodsReceivedId = "";
            GoodsReceivedId = dgvParkedItemH.CurrentRow.Cells["GoodsReceivedNumber"].Value.ToString();

            Query = "SELECT No, FullItemId, ItemName, Qty_Actual, Unit ";
            Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY GoodsReceivedId desc) No, FullItemId, ItemName, Qty_Actual, Unit  FROM GoodsReceivedD WHERE GoodsReceivedId = '" + GoodsReceivedId + "' AND ActionCodeStatus = '02') a ";
            Query += "WHERE No Between " + Limit1D + " and " + Limit2D + " ;";


            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvParkedItemD.AutoGenerateColumns = true;
            dgvParkedItemD.DataSource = Dt;
            dgvParkedItemD.Refresh();
            dgvParkedItemD.AutoResizeColumns();
            dgvParkedItemD.ReadOnly = true;
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();


            Query = "Select Count(GoodsReceivedId) From [dbo].[GoodsReceivedD] Where GoodsReceivedId = '" + GoodsReceivedId + "' AND ActionCodeStatus = '02' ;";

            Cmd = new SqlCommand(Query, Conn);
            TotalD = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotalD.Text = "Total Rows : " + TotalD.ToString();
            if (dataShowDetail != 0)
                Page2D = (int)Math.Ceiling((decimal)TotalD / dataShowDetail);
            else
                Page2D = (int)Math.Ceiling((decimal)TotalD / Int32.Parse(cmbShowD.Text));
            lblPageD.Text = "/ " + Page2D;
            
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

        private void cmbShowLoadDetail()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox] ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            cmbShowD.Items.Clear();
            while (Dr.Read())
            {
                cmbShowD.Items.Add(Dr.GetInt32(0));
            }
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            TotalD = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShowD.SelectedIndex = 0;
        }

        private void gvheader()
        {
           
            dgvParkedItemH.Columns["GoodsReceivedDate"].HeaderText = "Goods Receiver Date";
            dgvParkedItemH.Columns["GoodsReceivedNumber"].HeaderText = "Goods Receiver Number";
            dgvParkedItemH.Columns["ReceiptOrderDate"].HeaderText = "Receipt Order Date";
            dgvParkedItemH.Columns["ReceiptOrderNumber"].HeaderText = "Receipt Order Number";
            dgvParkedItemH.Columns["VendorID"].HeaderText = "Vendor ID";
            dgvParkedItemH.Columns["VendorName"].HeaderText = "Vendor Name";

            dgvParkedItemH.Columns["VendorID"].Visible = false;

        }
      
        private void gvheaderDetail()
        {
            dgvParkedItemD.Columns["FullItemId"].HeaderText = "FullItem ID";
            dgvParkedItemD.Columns["ItemName"].HeaderText = "Item Name";
            dgvParkedItemD.Columns["Qty_Actual"].HeaderText = "Qty";
            dgvParkedItemD.Columns["Unit"].HeaderText = "Unit";
        }

        private void InquiryParkedItem_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            //lblForm.Location = new Point(16, 11);
            gvheader();
        }

        private void InquiryParkedItem_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void InquiryParkedItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
            ClearGridDetail();
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
            ClearGridDetail();
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

        private void dgvParkedItemH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            LoadGridDetail();            
        }

        private void LoadGridDetail()
        {
            ModeLoadDetail();
            gvheaderDetail();
        }

        private void ClearGridDetail()
        {
            dgvParkedItemD.DataSource = null;
        }

        private void btnMPrevD_Click(object sender, EventArgs e)
        {
            txtPageD.Text = "1";
            Limit1D = (Int32.Parse(txtPageD.Text) - 1) * Int32.Parse(cmbShowD.Text) + 1;
            Limit2D = Int32.Parse(txtPageD.Text) * Int32.Parse(cmbShowD.Text);
            RefreshGridDetail();
        }

        private void btnPrevD_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShowD.Text) >= 1)
            {
                Limit1D -= Int32.Parse(cmbShowD.Text);
                Limit2D -= Int32.Parse(cmbShowD.Text);
                txtPageD.Text = (Int32.Parse(txtPageD.Text) - 1).ToString();
            }
            RefreshGridDetail();
        }

        private void btnNextD_Click(object sender, EventArgs e)
        {
            if (Limit1D + Int32.Parse(cmbShowD.Text) <= TotalD)
            {
                Limit1D += Int32.Parse(cmbShowD.Text);
                Limit2D += Int32.Parse(cmbShowD.Text);
                txtPageD.Text = (Int32.Parse(txtPageD.Text) + 1).ToString();
            }

            RefreshGridDetail();
        }

        private void btnMNextD_Click(object sender, EventArgs e)
        {
            txtPageD.Text = Math.Ceiling((decimal)TotalD / Int32.Parse(cmbShowD.Text)).ToString();
            Limit1D = (Int32.Parse(txtPageD.Text) - 1) * Int32.Parse(cmbShowD.Text) + 1;
            Limit2D = Int32.Parse(txtPageD.Text) * Int32.Parse(cmbShowD.Text);
            RefreshGridDetail();
        }

        private void cmbShowD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1D = (Int32.Parse(txtPageD.Text) - 1) * Int32.Parse(cmbShowD.Text) + 1;
                Limit2D = Int32.Parse(txtPageD.Text) * Int32.Parse(cmbShowD.Text);
                txtPageD.Text = "1";
                RefreshGridDetail();

            }
        }

        private void cmbShowD_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPageD.Text = "1";
            Limit1D = (Int32.Parse(txtPageD.Text) - 1) * Int32.Parse(cmbShowD.Text) + 1;
            Limit2D = Int32.Parse(txtPageD.Text) * Int32.Parse(cmbShowD.Text);
            RefreshGridDetail();
        }

        private void txtPageD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1D = (Int32.Parse(txtPageD.Text) - 1) * Int32.Parse(cmbShowD.Text) + 1;
                Limit2D = Int32.Parse(txtPageD.Text) * Int32.Parse(cmbShowD.Text);
                RefreshGridDetail();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectParkedItem();
        }

        private void SelectParkedItem()
        {
            if (dgvParkedItemH.RowCount > 0)
            {
                HeaderParkedItem header = new HeaderParkedItem();
                header.SetMode("BeforeEdit", dgvParkedItemH.CurrentRow.Cells["GoodsReceivedNumber"].Value.ToString(), dgvParkedItemH.CurrentRow.Cells["VendorID"].Value.ToString(), dgvParkedItemH.CurrentRow.Cells["VendorName"].Value.ToString());
                header.SetParent(this);
                header.Show();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show("Silahkan pilih data");
            }
        }
    }
}
