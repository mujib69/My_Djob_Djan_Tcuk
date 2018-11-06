using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ISBS_New.Master.PricelistConfig
{
    public partial class PricelistConfigInquiry : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        //String Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        public static int dataShow;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public PricelistConfigInquiry()
        {
            InitializeComponent();
        }

        private void PricelistConfigInquiry_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
        }

        private void gvHeader()
        {
            dgvPricelistConfig.Columns["SubGroup2"].HeaderText = "Sub Group 2";
            dgvPricelistConfig.Columns["PriceType"].HeaderText = "Price Type (Days)";
            dgvPricelistConfig.Columns["Factor"].HeaderText = "Factor (%)";
            dgvPricelistConfig.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvPricelistConfig.Columns["CreatedBy"].HeaderText = "Created By";
            dgvPricelistConfig.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvPricelistConfig.Columns["UpdatedBy"].HeaderText = "Updated By";
            dgvPricelistConfig.Columns["SubGroup2Id"].HeaderText = "Sub Group 2 ID";

            dgvPricelistConfig.Columns["SubGroup2Id"].Visible = false;
            dgvPricelistConfig.Columns["RecId"].Visible = false;

            dgvPricelistConfig.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvPricelistConfig.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
              

        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            //if (cmbCriteria.Text == null)
            if (crit == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY SubGroup2Id, PriceType ASC) No, SubGroup2ID, SubGroup2Name AS SubGroup2, PriceType, Factor, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, RecId From PricelistConfig) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            //else if (cmbCriteria.Text == "All")
            else if (crit.Equals("All"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY SubGroup2Id, PriceType ASC) No, SubGroup2ID, SubGroup2Name AS SubGroup2, PriceType, Factor, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, RecId From PricelistConfig Where SubGroup2Name Like '%" + txtSearch.Text + "%' ";
                Query += "or PriceType Like '%" + txtSearch.Text + "%' or Factor Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "SubGroup2")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY SubGroup2Id, PriceType ASC) No, SubGroup2ID, SubGroup2Name AS SubGroup2, PriceType, Factor, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, RecId  From PricelistConfig Where ";
                Query += "SubGroup2Name Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "CreatedDate")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY SubGroup2Id, PriceType ASC) No, SubGroup2ID, SubGroup2Name AS SubGroup2, PriceType, Factor, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, RecId  From PricelistConfig Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY SubGroup2Id, PriceType ASC) No, SubGroup2ID, SubGroup2Name AS SubGroup2, PriceType, Factor, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, RecId From PricelistConfig Where " + crit + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPricelistConfig.AutoGenerateColumns = true;
            dgvPricelistConfig.DataSource = Dt;
            gvHeader();
            dgvPricelistConfig.Refresh();
            dgvPricelistConfig.AutoResizeColumns();
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            //if (cmbCriteria.Text == null)
            if (crit == null)
            {
                Query = "Select Count(*) From PricelistConfig";
            }
            //else if (cmbCriteria.Text == "All")
            else if (crit.Equals("All"))
            {
                Query = "Select Count(*) From PricelistConfig WHERE SubGroup2Name Like '%" + txtSearch.Text + "%' ";
                Query += "or PriceType Like '%" + txtSearch.Text + "%' or Factor Like '%" + txtSearch.Text + "%' "; 
            }
            else if (cmbCriteria.Text == "CreatedDate")
            {
                Query = "Select Count(*) From PricelistConfig Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
            }
            else if (cmbCriteria.Text == "SubGroup2")
            {
                Query = "Select Count(*) From PricelistConfig Where ";
                Query += "SubGroup2Name Like '%" + txtSearch.Text + "%' ";
            }
            else
            {
                Query = "Select Count(*) From PricelistConfig Where " + crit + " Like '%" + txtSearch.Text + "%'";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            //Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)) == 0 ? 1 : (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));  //REMARKED BY: HC
            //BY: HC (S)
            if (dataShow != 0)
                Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            //BY: HC (E)
            lblPage.Text = "/ " + Page2;
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where TableName = 'PricelistConfig'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
        }

        private void ModeLoad()
        {
            //cmbShowLoad();
            //Limit1 = 1;
            //Limit2 = Int32.Parse(cmbShow.Text == "" ? "0" : cmbShow.Text);
            //Page1 = 1;
            //txtPage.Text = "1";

            //RefreshGrid();


            cmbShowLoad();
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            cmbCriteria.SelectedIndex = 0;

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
            Dr.Close();
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShow.SelectedIndex = 0;
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if ((txtSearch.Text == null || txtSearch.Text.Equals("")) && cmbCriteria.Text != "CreatedDate")
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

                RefreshGrid();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 25 apr 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                PricelistConfigForm F = new PricelistConfigForm();
                F.flag("", "New");
                F.Show();
                F.ParentRefreshGrid(this);
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end          
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            PricelistConfigForm F = new PricelistConfigForm();
            //begin
            //updated by : joshua
            //updated date : 25 apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPricelistConfig.RowCount > 0)
                {
                    String RecId = dgvPricelistConfig.CurrentRow.Cells["RecId"].Value.ToString();
                    F.flag(RecId, "BeforeEdit");
                    F.Show();
                    F.ParentRefreshGrid(this);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 25 apr 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvPricelistConfig.RowCount > 0)
                    {
                        Index = dgvPricelistConfig.CurrentRow.Index;
                        String RecId = dgvPricelistConfig.Rows[Index].Cells["RecId"].Value == null ? "" : dgvPricelistConfig.Rows[Index].Cells["RecId"].Value.ToString();
                        String No = dgvPricelistConfig.Rows[Index].Cells["No"].Value == null ? "" : dgvPricelistConfig.Rows[Index].Cells["No"].Value.ToString();

                        Conn = ConnectionString.GetConnection();
                        Query = "Select count(Ref_Config_RecId) from [dbo].[Pricelist_Dtl] where Ref_Config_RecId ='" + RecId + "' ";

                        Cmd = new SqlCommand(Query, Conn);
                        int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountData != 0)
                        {
                            MessageBox.Show("No = " + No.ToUpper() + "\n" + "Data tidak dapat dihapus karena telah digunakan.");
                            return;
                        }
                        else
                        {
                            DialogResult dr = MessageBox.Show("No = " + No.ToUpper() + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                Query = "INSERT INTO PricelistConfig_LogTable (SubGroup2ID, SubGroup2Name, PriceType, Factor, LogDescription, UserId, LogDate) ";
                                Query += "VALUES('" + dgvPricelistConfig.Rows[Index].Cells["SubGroup2Id"].Value + "', '" + dgvPricelistConfig.Rows[Index].Cells["SubGroup2"].Value + "', '" + dgvPricelistConfig.Rows[Index].Cells["PriceType"].Value + "', '" + dgvPricelistConfig.Rows[Index].Cells["Factor"].Value + "', 'Deleted', '" + ControlMgr.UserId + "', GETDATE())";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                Query = "Delete from [dbo].[PricelistConfig] where RecId ='" + RecId + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                MessageBox.Show("No = " + No.ToUpper() + "\n" + "Data berhasil dihapus.");

                                Index = 0;
                                RefreshGrid();
                            }
                        }
                    }
                }
                catch (Exception exx)
                {
                    MessageBox.Show(exx.Message);
                }
                finally
                {
                    Conn.Close();
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
            if ((txtSearch.Text == null || txtSearch.Text.Equals("")) && cmbCriteria.Text != "CreatedDate")
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

            RefreshGrid();
        }

        private void dgvPricelistConfig_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            PricelistConfigForm F = new PricelistConfigForm();
            //begin
            //updated by : joshua
            //updated date : 25 apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (e.RowIndex > -1)
                {
                    String RecId = dgvPricelistConfig.Rows[e.RowIndex].Cells["RecId"].Value.ToString();                   
                    F.flag(RecId, "BeforeEdit");
                    F.Show();
                    F.ParentRefreshGrid(this);
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
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
            //RefreshGrid();
        }
    }
}
