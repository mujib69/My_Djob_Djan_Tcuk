using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ISBS_New.Master.PIC
{
    public partial class DashPIC : Form
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private Boolean RefreshGridStatus = false;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;

        public DashPIC()
        {
            InitializeComponent();
            addCmbCrit();
        }

        private void DashPIC_Load(object sender, EventArgs e)
        {
            ModeLoad();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY PICId) No, PICId, PICName, UserId, UserId From [Master].[PIC]) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY PICId) No, PICId, PICName, UserId, UserId From [Master].[PIC] Where ";
                Query += "PICName like '%" + txtSearch.Text + "%' or Userid like '%" + txtSearch.Text + "%' or UserId like '%" + txtSearch.Text + "%' ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                if (crit.Equals("PICName"))
                {
                    crit = "PICName";
                }
                else if (crit.Equals("PICId"))
                {
                    crit = "PICId";
                }

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY PICId) No, PICId, PICName, UserId, UserId From [Master].[PIC] Where  " + crit + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPIC.AutoGenerateColumns = true;
            dgvPIC.DataSource = Dt;
            dgvPIC.Refresh();

            dgvPIC.AutoResizeColumns();
            Conn.Close();

            //Mengambil nilai total paging
            //Conn = ConnectionString.GetConnection();
            //Query = "Select Count(PICId) From [Master].[PIC] Where ";
            //Query += "PICId like '%" + txtSearch.Text + "%' or PICCode like '%" + txtSearch.Text + "%' or PICName like '%" + txtSearch.Text + "%' or UserId like '%" + txtSearch.Text + "%' or UserId like '%" + txtSearch.Text + "%'; ";

            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(PICId) From [Master].[PIC]";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY PICId) No, PICId, PICName, UserId, UserId From [Master].[PIC] Where ";
                Query += "PICName like '%" + txtSearch.Text + "%' or Userid like '%" + txtSearch.Text + "%' or UserId like '%" + txtSearch.Text + "%' ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select Count(PICId) From [Master].[PIC] where " + crit + " Like '" + txtSearch.Text + "'";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where TableName = 'PIC'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            Conn.Close();
        }

        private void ModeLoad()
        {
            Mode = "Edit";
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text == "" ? "0" : cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            grpGrid.Visible = true;
            btnNew.Visible = true;

            btnDelete.Visible = true;

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
            PIC pic = new PIC();
            pic.flag("", "New");
            pic.Show();
            pic.ParentRefreshGrid(this);
            RefreshGrid();
        }

        private void dgvPIC_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {

                String PICId = dgvPIC.Rows[e.RowIndex].Cells[1].Value.ToString();

                PIC pic = new PIC();
                pic.flag(PICId, "Edit");
                pic.Show();
                pic.ParentRefreshGrid(this);
                RefreshGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPIC.RowCount > 0)
                {
                    Index = dgvPIC.CurrentRow.Index;
                    String PICId = dgvPIC.Rows[Index].Cells["PICId"].Value == null ? "" : dgvPIC.Rows[Index].Cells["PICId"].Value.ToString();
                    String PICName = dgvPIC.Rows[Index].Cells["PICName"].Value == null ? "" : dgvPIC.Rows[Index].Cells["PICName"].Value.ToString();

                    DialogResult dr = MessageBox.Show("PICCode = " + PICId.ToUpper() + "\n" + "PICName = " + PICName.ToUpper() + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Conn = ConnectionString.GetConnection();
                        Query = "Delete from [Master].[PIC] where PICId='" + PICId + "'";

                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        MessageBox.Show("PICId = " + PICId.ToUpper() + "\n" + "Data berhasil dihapus.");

                        ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                        Index = 0;
                        Conn.Close();
                        RefreshGrid();
                    }

                }
            }
            catch (Exception exx)
            {
                MessageBox.Show(exx.Message);
                throw;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            String PICId = dgvPIC.CurrentRow.Cells["PICId"].Value.ToString();

            PIC pic = new PIC();
            pic.flag(PICId, "Edit");
            pic.Show();
            pic.ParentRefreshGrid(this);
            RefreshGrid();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Category");
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
            RefreshGrid();
        }


    }
}
