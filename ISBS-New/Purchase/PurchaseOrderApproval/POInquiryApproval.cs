using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseOrderApproval
{
    public partial class POInquiryApproval : MetroFramework.Forms.MetroForm
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

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public POInquiryApproval()
        {
            InitializeComponent();
        }

        private void POInquiryApproval_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
        }

        public void RefreshGrid()
        {
            //if (ControlMgr.GroupName == "Purchase Manager" || ControlMgr.GroupName == "Management")
            //{
                //Menampilkan data
            //hasim 10 okt 2018
            int mflag;
            String addquery = null;

                Conn = ConnectionString.GetConnection();
                
            Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CASE WHEN a.CreatedDate >= a.UpdatedDate THEN a.CreatedDate ELSE a.UpdatedDate END DESC) No, PurchID, OrderDate, DueDate, TransType, ReffTableName, ReffID, CurrencyID, ExchRate, VendID, TransStatus, b.Deskripsi From [dbo].[PurchH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='PO' Where UPPER(a.ReffTableName) IN ('Canvass Sheet', 'Purchase Agreement','Purchase Order') and TransStatus IN ('04', '08') ";
            
            addquery = "AND (PurchID like @search or OrderDate like @search or DueDate like @search or TransType like @search or ReffTableName like @search or ReffID like @search or CurrencyID like @search or ExchRate like @search or VendID like @search)";
            mflag = 1;    
            if (crit == null)
                {
                    mflag = 0;
                }
                else if (crit.Equals("All"))
                {
                    Query += addquery;
                }
                else if (crit.Equals("OrderDate"))
                {
                    Query += addquery + " AND OrderDate BETWEEN @from AND @to ";
                    mflag = 2;
                }
                else if (crit.Equals("DueDate"))
                {
                    Query += addquery + " AND DueDate BETWEEN @from AND @to ";
                    mflag = 2;
                }
                else
                {
                    Query =  crit + " Like @search ";
                }
                Query += ") a WHERE No BETWEEN @limit1 and @limit2 ;";
                
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

                dgvPO.AutoGenerateColumns = true;
                dgvPO.DataSource = Dt;
                dgvPO.Refresh();
                dgvPO.AutoResizeColumns();
                dgvPO.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPO.Columns["DueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                Conn.Close();

                //Mengambil nilai total paging
                Conn = ConnectionString.GetConnection();

                Query = "Select Count(PurchID) From [dbo].[PurchH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='PO' Where UPPER(a.ReffTableName) IN ('Canvass Sheet', 'Purchase Agreement','Purchase Order') and TransStatus IN ('04', '08') ";
               
                if (crit == null)
                {
                    Query += "";
                }
                else if (crit.Equals("All"))
                {
                    Query += addquery;
                }
                else if (crit.Equals("OrderDate"))
                {
                    Query += addquery + "AND (OrderDate BETWEEN @from AND @to)";
                }
                else if (crit.Equals("DueDate"))
                {
                    Query += addquery + "AND (DueDate BETWEEN @from AND @to)";
                }
                else
                {
                   Query += crit + " Like @search";
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
           // }
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchH'";

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
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
                if (Int32.Parse(txtPage.Text) <= Page2)
                {
                    if (e.KeyChar == (char)13)
                    {
                        Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                        Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                        RefreshGrid();
                    }
                }
                else
                {
                    txtPage.Text = Page2.ToString();
                    if (e.KeyChar == (char)13)
                    {
                        Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                        Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                        RefreshGrid();
                    }
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            POHeaderApproval header = new POHeaderApproval();                   
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPO.RowCount > 0)
                {
                    header.flag(dgvPO.CurrentRow.Cells["PurchID"].Value.ToString());
                    header.setParent(this);
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
            this.Close();
        }

        private void backtopageone()
        {
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
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

        private void dgvPO_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            POHeaderApproval header = new POHeaderApproval();                    
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPO.RowCount > 0)
                {
                    header.flag(dgvPO.CurrentRow.Cells["PurchID"].Value.ToString());
                    header.setParent(this);
                    header.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            
        }

        private void dgvPO_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dgvPO.Columns["TransStatus"].Visible = false; //BY: HC 
        }


    }
}
