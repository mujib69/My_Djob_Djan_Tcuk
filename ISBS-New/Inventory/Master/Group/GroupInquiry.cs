using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ISBS_New.Master.Group
{
    public partial class GroupInquiry : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public GroupInquiry()
        {
            InitializeComponent();
        }

        private void GroupInquiry_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (cmbCriteria.Text == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No, GroupId, Deskripsi ,CreatedDate, CreatedBy, UpdatedDate,UpdatedBy From [dbo].[InventGroup]) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No, GroupId, Deskripsi ,CreatedDate, CreatedBy, UpdatedDate,UpdatedBy From [dbo].[InventGroup] Where ";
                Query += "GroupId Like @search or Deskripsi Like @search or CreatedBy like @search  or UpdatedBy like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "Created Date")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No, GroupId, Deskripsi ,CreatedDate, CreatedBy, UpdatedDate,UpdatedBy From [dbo].[InventGroup] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "(GroupID like @search or Deskripsi like @search or CreatedBy like @search  or UpdatedBy like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "Updated Date")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No, GroupId, Deskripsi ,CreatedDate, CreatedBy, UpdatedDate,UpdatedBy From [dbo].[InventGroup] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "(GroupID like @search or Deskripsi like @search or CreatedBy like @search  or UpdatedBy like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "'", Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No, GroupId, Deskripsi ,CreatedDate, CreatedBy, UpdatedDate,UpdatedBy ";
                Query += "From [dbo].[InventGroup] Where ";
                Query += crit + " Like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@search",
                Value = "%" + txtSearch.Text + "%",
                SqlDbType = SqlDbType.NVarChar,
                Size = 2000  // Assuming a 2000 char size of the field annotation (-1 for MAX)
            });
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGroup.AutoGenerateColumns = true;
            dgvGroup.DataSource = Dt;
            dgvGroup.Refresh();
            dgvGroup.AutoResizeColumns();
            dgvSetting();
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(GroupId) From [dbo].[InventGroup]";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(GroupId) From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No, GroupId, Deskripsi ,CreatedDate, CreatedBy, UpdatedDate,UpdatedBy From [dbo].[InventGroup] Where ";
                //Query += "GroupId like @search or Deskripsi like @search )a ";
                Query += "GroupID like @search or Deskripsi like @search or CreatedBy like @search  or UpdatedBy like @search) a ";
            }
            else if (cmbCriteria.Text == "Created Date")
            {
                Query = "Select Count(GroupId) From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No, GroupId, Deskripsi ,CreatedDate, CreatedBy, UpdatedDate,UpdatedBy From [dbo].[InventGroup] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "(GroupID like @search or Deskripsi like @search or CreatedBy like @search  or UpdatedBy like @search)) a ";
            }
            else if (cmbCriteria.Text == "Updated Date")
            {
                Query = "Select Count(GroupId) From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No, GroupId, Deskripsi ,CreatedDate, CreatedBy, UpdatedDate,UpdatedBy From [dbo].[InventGroup] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "(GroupID like @search or Deskripsi like @search or CreatedBy like @search  or UpdatedBy like @search)) a ";
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "'", Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select Count(GroupId) From [dbo].[InventGroup] Where ";
                Query += crit + " Like @search2";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
            Cmd.Parameters.AddWithValue("@search2",txtSearch.Text);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)) == 0 ? 1 : (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        private void dgvSetting()
        {
            dgvGroup.Columns["GroupId"].HeaderText = "Group Id";
            dgvGroup.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvGroup.Columns["CreatedBy"].HeaderText = "Created By";
            dgvGroup.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvGroup.Columns["UpdatedBy"].HeaderText = "Updated By";

            dgvGroup.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvGroup.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where TableName = 'InventGroup'";

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
            Limit2 = Int32.Parse(cmbShow.Text == "" ? "0" : cmbShow.Text);
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
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                RefreshGrid();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                GroupForm F = new GroupForm();
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
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            GroupForm F = new GroupForm();                  
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvGroup.Rows.Count > 0)
                {
                    String GroupId = dgvGroup.CurrentRow.Cells["GroupId"].Value.ToString();
                    F.flag(GroupId, "Edit");
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

        private bool cekValidasi(string GroupId)
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (vBol == true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventGroup] Where GroupId='" + GroupId + "'";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Group Name tidak ditemukan..";
                            vBol = false;
                            RefreshGrid();
                        }
                        Dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Conn.Close();
                }
            }

            if (vBol == true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventSubGroup1] Where GroupId='" + GroupId + "'";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Group Name sudah pernah digunakan..";
                            vBol = false;
                        }
                        else
                        {
                            vBol = true;
                        }
                        Dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Conn.Close();
                }
            }

            if (vBol == false) { MessageBox.Show(ErrMsg); }
            return vBol;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {            
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvGroup.RowCount > 0)
                    {
                        Index = dgvGroup.CurrentRow.Index;
                        String GroupId = dgvGroup.Rows[Index].Cells["GroupId"].Value == null ? "" : dgvGroup.Rows[Index].Cells["GroupId"].Value.ToString();

                        if (cekValidasi(GroupId) == false)
                        {
                            return;
                        }

                        DialogResult dr = MessageBox.Show("Group ID = " + GroupId.ToUpper() + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[InventGroup] where GroupId ='" + GroupId + "' ";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("Group ID = " + GroupId.ToUpper() + "\n" + "Data berhasil dihapus.");

                            Index = 0;
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception exx)
                {
                    MessageBox.Show(exx.Message);
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void dgvGroup_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                String GroupId = dgvGroup.Rows[e.RowIndex].Cells["GroupId"].Value.ToString();
                GroupForm F = new GroupForm();
                F.flag(GroupId, "Edit");
                F.Show();
                F.ParentRefreshGrid(this);
                RefreshGrid();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {            
            txtSearch.Text = "";
            RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {            
            RefreshGrid();
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

    }
}
