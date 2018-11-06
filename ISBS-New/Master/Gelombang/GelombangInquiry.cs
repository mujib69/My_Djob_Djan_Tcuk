using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ISBS_New.Master.Gelombang
{
    public partial class GelombangInquiry : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query,vJenis;
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

        public GelombangInquiry(string _vJenis)
        {
            InitializeComponent();
            vJenis = _vJenis;
        }

        private void GelombangInquiry_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            dgvGelombang.AutoGenerateColumns = false;
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null)
            {
                //Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY g.CreatedDate desc) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, ";
                Query += " VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type='" +vJenis+ "') a ";
                Query += " WHERE No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "All")
            {
                //Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY g.CreatedDate desc) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type= '" + vJenis + "' AND ";
                Query += "(GelombangId like @search or BracketId like @search or BracketDesc like @search))a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text.Equals("Created Date"))
            {
                //Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY g.CreatedDate desc) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type= '" + vJenis + "' AND ";
                Query += " (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (GelombangId like @search or BracketId like @search or VendId like @search)) a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text.Equals("Updated Date"))
            {
                //Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY g.CreatedDate desc) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type= '" + vJenis + "' AND ";
                Query += " (CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (GelombangId like @search or BracketId like @search or VendId like @search)) a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                string crit;
                if (cmbCriteria.Text.Equals("Updated By"))
                {
                    crit = "g.UpdatedBy";
                }
                else if (cmbCriteria.Text.Equals("Created By"))
                {
                    crit = "g.CreatedBy";
                }
                else
                {
                    Query = "Select FieldName from [User].[Table] Where DisplayName = '" + cmbCriteria.Text.ToString() + "' AND TableName = 'GelombangInquiry'";
                    Cmd = new SqlCommand(Query, Conn);
                    crit = Cmd.ExecuteScalar().ToString();
                }

                //Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query = " SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY g.CreatedDate desc) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type= '" + vJenis + "' AND ";
                Query += crit + " Like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGelombang.AutoGenerateColumns = true;
            dgvGelombang.DataSource = Dt;
            dgvGelombang.Refresh();
            dgvGelombang.AutoResizeColumns();
            dgvSetting();
            Conn.Close();
            dgvPaging();            
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where TableName = 'GelombangInquiry' order by OrderNo";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void dgvSetting()
        {
            dgvGelombang.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvGelombang.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvGelombang.Columns["GelombangId"].HeaderText = "Gelombang Id";
            dgvGelombang.Columns["BracketId"].HeaderText = "Bracket Id";
            dgvGelombang.Columns["BracketDesc"].HeaderText = "Bracket Desc";
            dgvGelombang.Columns["Deskripsi"].HeaderText = "Description";
            dgvGelombang.Columns["VendName"].HeaderText = "Vendor";
            dgvGelombang.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvGelombang.Columns["CreatedBy"].HeaderText = "Created By";
            dgvGelombang.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvGelombang.Columns["UpdatedBy"].HeaderText = "Updated By";

            dgvGelombang.Columns["VendId"].Visible = false;
        }

        private void dgvPaging()
        {
            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();            

            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(GelombangId ) From [dbo].[InventGelombangH]";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(GelombangId ) From (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type= '" + vJenis + "' AND ";
                Query += " GelombangId like @search or BracketId like @search)a ";
            }
            else if (cmbCriteria.Text.Equals("Created Date"))
            {
                Query = "Select Count(GelombangId ) From (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type= '" + vJenis + "' AND ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (GelombangId like @search or BracketId like @search or VendId like @search )) a ;";
            }
            else if (cmbCriteria.Text.Equals("Updated Date"))
            {
                Query = "Select Count(GelombangId ) From (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type= '" + vJenis + "' AND ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (GelombangId like @search or BracketId like @search or VendId like @search )) a ;";
            }
            else
            {
                string crit;
                if (cmbCriteria.Text.Equals("Updated By"))
                {
                    crit = "g.UpdatedBy";
                }
                else if (cmbCriteria.Text.Equals("Created By"))
                {
                    crit = "g.CreatedBy";
                }
                else
                {
                    Query = "Select FieldName from [User].[Table] Where DisplayName = '" + cmbCriteria.Text.ToString() + "' AND TableName = 'GelombangInquiry'";
                    Cmd = new SqlCommand(Query, Conn);
                    crit = Cmd.ExecuteScalar().ToString();
                }

                Query = "Select Count(GelombangId ) From (SELECT ROW_NUMBER() OVER (ORDER BY GelombangId,BracketId) ";
                Query += " No, GelombangId, BracketId, BracketDesc, Deskripsi, g.VendId, VendName, g.CreatedDate, g.CreatedBy, g.UpdatedDate, g.UpdatedBy ";
                Query += " FROM [dbo].[InventGelombangH] g ";
                Query += " LEFT JOIN [VendTable] v On g.VendId = v.VendId ";
                Query += " WHERE Type= '" + vJenis + "' AND ";
                Query += crit + " Like @search )a";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text == "" ? "0" : cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
            cmbCriteria.SelectedItem = "All";
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
            if (Page2 != null)
            {
                if (Convert.ToInt32(txtPage.Text) > Page2)
                {
                    txtPage.Text = Page2.ToString();
                }
            }
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
            else if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
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
                GelombangForm f = new GelombangForm(vJenis);
                f.flag("", "", "NewGel");
                f.Show();
                f.ParentRefreshGrid(this);
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end           
        }

        private void dgvGelombang_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                String GelombangId = dgvGelombang.Rows[e.RowIndex].Cells[1].Value.ToString();
                String BracketId = dgvGelombang.Rows[e.RowIndex].Cells[2].Value.ToString();
                
                GelombangForm f = new GelombangForm(vJenis);
                f.flag(GelombangId, BracketId, "Edit");
                f.Show();
                f.ParentRefreshGrid(this);
                RefreshGrid();
            }    
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            
            txtSearch.Text = "";
            RefreshGrid();
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
                    if (dgvGelombang.RowCount > 0)
                    {
                        Index = dgvGelombang.CurrentRow.Index;
                        String GelombangId = dgvGelombang.Rows[Index].Cells["GelombangId"].Value == null ? "" : dgvGelombang.Rows[Index].Cells["GelombangId"].Value.ToString();
                        String BracketId = dgvGelombang.Rows[Index].Cells["BracketId"].Value == null ? "" : dgvGelombang.Rows[Index].Cells["BracketId"].Value.ToString();
                        
                        //stv edit s
                        Query = "Select GelombangId from PurchRequisition_Dtl where GelombangId = '" + GelombangId + "'";
                        using (Conn = ConnectionString.GetConnection())
                        using (Cmd = new SqlCommand(Query, Conn))
                        {
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                MessageBox.Show("Tidak bisa hapus, sudah terpakai");
                                return;
                            }
                            else
                            {
                                // Query = "Select BracketId from PurchRequisition_Dtl where BracketId = '" + BracketId + "'";
                                Query = "Select BracketId from PurchRequisition_Dtl where BracketId = '" + BracketId + "' and GelombangId = '" + GelombangId + "'";
                                using (Conn = ConnectionString.GetConnection())
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Dr = Cmd.ExecuteReader();
                                    if (Dr.HasRows)
                                    {
                                        MessageBox.Show("Tidak bisa hapus, sudah terpakai");
                                        return;
                                    }
                                    else
                                    {
                                        //old code untouched
                                        DialogResult dr = MessageBox.Show("GelombangId = " + GelombangId.ToUpper() + "\n" + "BracketId = " + BracketId.ToUpper() + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                                        if (dr == DialogResult.Yes)
                                        {
                                            //Delete Header
                                            Conn = ConnectionString.GetConnection();
                                            Query = "Delete from [dbo].[InventGelombangH] where GelombangId ='" + GelombangId + "' and BracketId = '" + BracketId + "' ";

                                            Cmd = new SqlCommand(Query, Conn);
                                            Cmd.ExecuteNonQuery();

                                            //delete item
                                            Query = "Delete from [dbo].[InventGelombangD] where GelombangId ='" + GelombangId + "' and BracketId = '" + BracketId + "' ";
                                            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                                            Cmd = new SqlCommand(Query, Conn, Trans);
                                            Cmd.ExecuteNonQuery();

                                            MessageBox.Show("GelombangId = " + GelombangId.ToUpper() + "\n" + "BracketId = " + BracketId.ToUpper() + "\n" + "Data berhasil dihapus.");

                                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                                            Index = 0;
                                            Conn.Close();
                                            RefreshGrid();
                                        }
                                        //untouched
                                    }
                                }
                            }
                        }
                        //stv edit ed                                            
                    }
                }
                catch (Exception exx)
                {
                    MessageBox.Show(exx.Message);
                    throw;
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            GelombangForm f = new GelombangForm(vJenis);               
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                String GelombangId = dgvGelombang.CurrentRow.Cells["GelombangId"].Value.ToString();
                String BracketId = dgvGelombang.CurrentRow.Cells["BracketId"].Value.ToString();

                f.flag(GelombangId, BracketId, "Edit");
                f.Show();
                f.ParentRefreshGrid(this);
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end           
        }

        private void btnNewBrk_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                if (dgvGelombang.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada Gelombang");
                }
                else
                {
                    String GelombangId = dgvGelombang.CurrentRow.Cells["GelombangId"].Value.ToString();
                    String BracketId = dgvGelombang.CurrentRow.Cells["BracketId"].Value.ToString();

                    GelombangForm f = new GelombangForm(vJenis);
                    f.flag(GelombangId, BracketId, "NewBrk");
                    f.Show();
                    f.ParentRefreshGrid(this);
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
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }
        }

        //bool IsTheSameCellValue(int column, int row)
        //{
        //    DataGridViewCell cell1 = dgvGelombang[column, row];
        //    DataGridViewCell cell2 = dgvGelombang[column, row - 1];
        //    if (cell1.Value == null || cell2.Value == null)
        //    {
        //        return false;
        //    }
        //    return cell1.Value.ToString() == cell2.Value.ToString();
        //}

        //private void dgvGelombang_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        //{
        //    e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
        //    if (e.RowIndex < 1 || e.ColumnIndex < 0)
        //        return;
        //    if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
        //    {
        //        e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        //    }
        //    else
        //    {
        //        e.AdvancedBorderStyle.Top = dgvGelombang.AdvancedCellBorderStyle.Top;
        //    }
        //}

        //private void dgvGelombang_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        //{
        //    if (e.RowIndex == 0)
        //        return;
        //    if (IsTheSameCellValue(e.ColumnIndex, e.RowIndex))
        //    {
        //        e.Value = "";
        //        e.FormattingApplied = true;
        //    }
        //}



    }
}
