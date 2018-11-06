using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Golongan
{
    public partial class GolonganInquiry : MetroFramework.Forms.MetroForm
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

        public GolonganInquiry()
        {
            InitializeComponent();
        }

        private void GolonganInquiry_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            string Select = " ,GolonganId, Deskripsi ,CreatedDate, CreatedBy ,UpdatedDate, UpdatedBy ";
            string SelectLike = " (GolonganId Like @search or Deskripsi Like @search or CreatedBy Like @search or UpdatedBy Like @search) ";

            if (cmbCriteria.Text == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan]) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan] Where ";
                Query += SelectLike + ") a ";                
            }
            else if (cmbCriteria.Text == "Created Date")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += SelectLike + ") a ";
            }
            else if (cmbCriteria.Text == "Updated Date")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += SelectLike + " ) a ";
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "' AND TableName = 'Golongan'", Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan] Where ";
                Query += crit + " Like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@search",
                Value = "%" + txtSearch.Text + "%",
                SqlDbType = SqlDbType.NVarChar,
                Size = 2000 
            });
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGolongan.AutoGenerateColumns = true;
            dgvGolongan.DataSource = Dt;
            dgvGolongan.Refresh();
            dgvSetting();
            dgvGolongan.AutoResizeColumns();
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
                        
            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(GolonganId) OVER (ORDER BY GolonganId) No "+Select+" From [dbo].[InventGolongan]";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(GolonganId) From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan] Where ";
                Query += SelectLike + ")a ";
            }
            else if (cmbCriteria.Text == "Created Date")
            {
                Query = "Select Count(GolonganId) From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += SelectLike + ")a ";
            }
            else if (cmbCriteria.Text == "Updated Date")
            {
                Query = "Select Count(GolonganId) From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += SelectLike + ")a ";
            }            
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "' AND TableName = 'Golongan'", Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select Count(GolonganId) From (Select ROW_NUMBER() OVER (ORDER BY GolonganId) No " + Select + " From [dbo].[InventGolongan] Where ";
                Query += crit + " Like @search) a ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)) == 0 ? 1 : (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        public string[] Header;
        private void dgvSetting()
        {
            Header = new string[] { "Golongan", "Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
            for (int i = 1; i < Header.Count() + 1; i++)
            {
                dgvGolongan.Columns[i].HeaderText = Header[i - 1];
            }

            dgvGolongan.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            dgvGolongan.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT DisplayName FROM [User].[Table] WHERE TableName = 'Golongan' ORDER BY OrderNo";

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
                GolonganForm F = new GolonganForm();
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
            GolonganForm F = new GolonganForm();               
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvGolongan.Rows.Count > 0)
                {
                    String GolonganId = dgvGolongan.CurrentRow.Cells["GolonganId"].Value.ToString();
                    F.flag(GolonganId, "Edit");
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

        private bool cekValidasi(string GolonganId)
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (vBol == true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventGolongan] Where GolonganID='" + GolonganId + "'";
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
                            ErrMsg = "Golongan tidak ditemukan..";
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
                    Query = "Select * From [dbo].[InventTable] Where GolonganID='" + GolonganId + "'";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Golongan sudah pernah digunakan..";
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
                    if (dgvGolongan.RowCount > 0)
                    {
                        Index = dgvGolongan.CurrentRow.Index;
                        String GolonganId = dgvGolongan.Rows[Index].Cells["GolonganId"].Value == null ? "" : dgvGolongan.Rows[Index].Cells["GolonganId"].Value.ToString();

                        if (cekValidasi(GolonganId) == false)
                        {
                            return;
                        }

                        DialogResult dr = MessageBox.Show("Golongan ID = " + GolonganId.ToUpper() + "\nApakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[InventGolongan] where GolonganId =@GolonganId ";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.AddWithValue("@GolonganId", GolonganId);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("Golongan ID = " + GolonganId.ToUpper() + "\n" + "Data berhasil dihapus.");

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

        private void dgvGolongan_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                String GolonganId = dgvGolongan.Rows[e.RowIndex].Cells["GolonganId"].Value.ToString();
               
                GolonganForm F = new GolonganForm();
                F.flag(GolonganId,"Edit");
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
