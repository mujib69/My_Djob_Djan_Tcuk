using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Quality
{
    public partial class InqQuality : MetroFramework.Forms.MetroForm
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
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public InqQuality()
        {
            InitializeComponent();
        }

        private void InqQuality_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY QualityId) No, QualityId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventQuality]) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY QualityId) No, QualityId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventQuality] Where ";
                Query += "QualityId Like @search or Deskripsi Like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY QualityId) No, QualityId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventQuality] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "(QualityId Like @search or Deskripsi Like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("Updated Date"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY QualityId) No, QualityId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventQuality] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "(QualityId Like @search or Deskripsi Like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "' AND TableName = 'InventQuality'", Conn);
                crit = Cmd.ExecuteScalar().ToString();

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY QualityId) No, QualityId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventQuality] Where " + crit + " Like @search) a ";
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

            dgvQuality.AutoGenerateColumns = true;
            dgvQuality.DataSource = Dt;
            dgvQuality.Refresh();
            dgvQuality.AutoResizeColumns();
            dgvSetting();
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(*) From [dbo].[InventQuality]";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY QualityId) No, QualityId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventQuality] Where ";
                Query += "QualityId Like @search or Deskripsi Like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY QualityId) No, QualityId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventQuality] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "(QualityId Like @search or Deskripsi Like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("Updated Date"))
            {
                Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY QualityId) No, QualityId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventQuality] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "(QualityId Like @search or Deskripsi Like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "' AND TableName = 'InventQuality'", Conn);
                crit = Cmd.ExecuteScalar().ToString();

                Query = "Select Count(*) From [dbo].[InventQuality] Where " + crit + " Like @search";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)) == 0 ? 1 : (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where TableName = 'InventQuality' Order By OrderNo";

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
            string[] Header;
            Header = new string[] { "Quality", "Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
            for (int i = 1; i < Header.Count() + 1; i++)
            {
                dgvQuality.Columns[i].HeaderText = Header[i - 1];
            }

            dgvQuality.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            dgvQuality.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            if (cmbShow.Text == "")
            {
                cmbShow.Text = "5";
            }
            else
                cmbShow.Text = cmbShow.Text;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            
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
                    if (dgvQuality.RowCount > 0)
                    {
                        Index = dgvQuality.CurrentRow.Index;
                        String QualityId = dgvQuality.Rows[Index].Cells["QualityId"].Value == null ? "" : dgvQuality.Rows[Index].Cells["QualityId"].Value.ToString();

                        DialogResult dr = MessageBox.Show("Quality ID = " + QualityId.ToUpper() + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[InventQuality] where QualityId=@QualityId ";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.AddWithValue("@QualityId", QualityId);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("Quality ID = " + QualityId.ToUpper() + "\n" + "Data berhasil dihapus.");

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //if (txtSearch.Text == null || txtSearch.Text.Equals(""))
            //{
            //    MessageBox.Show("Masukkan Kata Kunci");
            //}
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                Quality F = new Quality();
                F.flag("", "New");
                F.setParent(this);
                F.Show();
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
            Quality F = new Quality();               
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvQuality.Rows.Count > 0)
                {
                    String QualityId = dgvQuality.CurrentRow.Cells["QualityId"].Value.ToString();

                    F.flag(QualityId, "Edit");
                    F.setParent(this);
                    F.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void dgvQuality_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                String QualityId = dgvQuality.Rows[e.RowIndex].Cells["QualityId"].Value.ToString();

                Quality F = new Quality();
                F.flag(QualityId, "Edit");
                F.setParent(this);
                F.Show();
                RefreshGrid();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (txtSearch.Text == null || txtSearch.Text.Equals(""))
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
