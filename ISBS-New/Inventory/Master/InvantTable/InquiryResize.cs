using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.Master.InvantTable
{
    public partial class InquiryResize : MetroFramework.Forms.MetroForm
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

        public InquiryResize()
        {
            InitializeComponent();
        }

        private void InquiryResize_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (cmbCriteria.Text == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY [TransId] desc) No, [TransId], [TransDate], [Deskripsi], [Posted], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[ResizeTableH] where Posted != '02' order by UpdatedDate, CreatedDate ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY [TransId] desc) No, [TransId], [TransDate], [Deskripsi], [Posted], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[ResizeTableH] ";
                Query += "Where Posted != '02' and ( [TransId] like '%" + txtSearch.Text + "%' or CreatedBy like '%" + txtSearch.Text + "%' or UpdatedBy like '%" + txtSearch.Text + "%' ) ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == String.Empty)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY [TransId] desc) No, [TransId], [TransDate], [Deskripsi], [Posted], CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[ResizeTableH] where Posted != '02' ) a Where No Between 1 and 10 ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = Dt;
            dataGridView1.Refresh();
            dataGridView1.AutoResizeColumns();
            //dataGridView1.Columns["RecID"].Visible = false;
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null || cmbCriteria.Text == String.Empty)
            {
                Query = "Select Count([TransId]) From [dbo].[ResizeTableH] where Posted != '2';";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count([TransId]) From [dbo].[ResizeTableH] where Posted != '2';";
            }
            //else
            //{
                //Query = "Select Count(FullItemID) From [dbo].[InventTable] Where ";
                //Query += cmbCriteria.Text + " Like '%" + txtSearch.Text + "%'";
            //}

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if(cmbShow.Text == String.Empty)
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse("10"));
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

            dataGridView1.Columns["TransDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dataGridView1.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dataGridView1.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
        }


        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            //Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'InventTable'";

            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();
            //cmbCriteria.Items.Add("FullItemId");
            //cmbCriteria.Items.Add("UoMQty");
            //cmbCriteria.Items.Add("GroupID");
            //while (Dr.Read())
            //{
            //    cmbCriteria.Items.Add(Dr[0]);
            //}
            cmbCriteria.SelectedIndex = 0;
            //Conn.Close();
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

        private void txtPage_TextChanged(object sender, EventArgs e)
        {
            //if (e.KeyChar == (char)13)
            //{
            //    Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            //    Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            //    RefreshGrid();
            //}
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ModeLoad();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                HeaderResize f = new HeaderResize();
                f.SetMode("New", "", "");
                f.SetParent(this);
                f.Show();
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
            SelectPR();
        }

        #region SelectPR
        private void SelectPR()
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            HeaderResize f = new HeaderResize();                   
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dataGridView1.RowCount > 0)
                {
                    f.SetMode("BeforeEdit", dataGridView1.CurrentRow.Cells["TransID"].Value.ToString(), "");
                    f.SetParent(this);
                    f.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }
        #endregion

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
                SelectPR();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                string msg = "";
                int count = 0;
                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                {
                    if (count >= 1)
                        msg += ", ";
                    msg += dataGridView1.Rows[r.Index].Cells["TransId"].Value.ToString();
                    count++;
                }

                if (msg == String.Empty)
                {
                    MetroFramework.MetroMessageBox.Show(this, "Select Row(s)!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult dialogResult = MetroFramework.MetroMessageBox.Show(this, "Are you sure to delete " + msg/*dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["TransId"].Value.ToString()*/ + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Conn = ConnectionString.GetConnection();
                        foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                        {
                            Query = "update ResizeTableH set Posted = '02' where Transid = '" + dataGridView1.Rows[r.Index].Cells["TransId"].Value.ToString() + "'; ";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        RefreshGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Selected = true;
        }

    }
}
