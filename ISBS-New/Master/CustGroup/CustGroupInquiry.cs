using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.CustGroup
{
    public partial class CustGroupInquiry : MetroFramework.Forms.MetroForm
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

        public CustGroupInquiry()
        {
            InitializeComponent();
        }

        private void CustGroupInquiry_Load(object sender, EventArgs e)
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
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CustGroupId) No, CustGroupId, CustGroupName, KursOrigin, DepositAmountCurrencyId, DepositAmount, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerSO From [dbo].[CustGroup]) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CustGroupId) No, CustGroupId, CustGroupName, KursOrigin, DepositAmountCurrencyId, DepositAmount, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerSO From [dbo].[CustGroup] Where CustGroupId Like '%" + txtSearch.Text + "%' or CustGroupName Like '%" + txtSearch.Text + "%' or KursOrigin Like '%" + txtSearch.Text + "%' ";
                Query += "or DepositAmountCurrencyID Like '%" + txtSearch.Text + "%' or DepositAmount Like '%" + txtSearch.Text + "%' or DPAmountCurrencyID Like '%" + txtSearch.Text + "%' or DPAmount Like '%" + txtSearch.Text + "%' or CreditLimitCurrencyID Like '%" + txtSearch.Text + "%' or CreditLimit Like '%" + txtSearch.Text + "%' or CreditLimitPerSO Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CustGroupId) No, CustGroupId, CustGroupName, KursOrigin, DepositAmountCurrencyId, DepositAmount, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerSO From [dbo].[CustGroup] Where " + cmbCriteria.Text + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvCustGroup.AutoGenerateColumns = true;
            dgvCustGroup.DataSource = Dt;
            dgvCustGroup.Refresh();
            dgvCustGroup.AutoResizeColumns();
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(*) From [dbo].[CustGroup]";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY CustGroupId) No, CustGroupId, CustGroupName, KursOrigin, DepositAmountCurrencyId, DepositAmount, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerSO From [dbo].[CustGroup] Where CustGroupId Like '%" + txtSearch.Text + "%' or CustGroupName Like '%" + txtSearch.Text + "%' or KursOrigin Like '%" + txtSearch.Text + "%' ";
                Query += "or DepositAmountCurrencyID Like '%" + txtSearch.Text + "%' or DepositAmount Like '%" + txtSearch.Text + "%' or DPAmountCurrencyID Like '%" + txtSearch.Text + "%' or DPAmount Like '%" + txtSearch.Text + "%' or CreditLimitCurrencyID Like '%" + txtSearch.Text + "%' or CreditLimit Like '%" + txtSearch.Text + "%' or CreditLimitPerSO Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select Count(*) From [dbo].[CustGroup] Where " + cmbCriteria.Text + " Like '%" + txtSearch.Text + "%'";
            }

            Cmd = new SqlCommand(Query, Conn);
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
            Query = "Select FieldName From [User].[Table] Where TableName = 'CustGroup'";

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

            cmbShowLoad();
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
            if (this.PermissionAccess(Login.Delete) > 0)
            {
                try
                {
                    if (dgvCustGroup.RowCount > 0)
                    {
                        Index = dgvCustGroup.CurrentRow.Index;
                        String CustGroupId = dgvCustGroup.Rows[Index].Cells["CustGroupId"].Value == null ? "" : dgvCustGroup.Rows[Index].Cells["CustGroupId"].Value.ToString();

                        DialogResult dr = MessageBox.Show("Customer Group ID = " + CustGroupId.ToUpper() + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[CustGroup] where CustGroupId ='" + CustGroupId + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("Customer Group ID = " + CustGroupId.ToUpper() + "\n" + "Data berhasil dihapus.");

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
                MessageBox.Show(Login.PermissionDenied);
            }
            //end              
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
           
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            
            txtSearch.Text = "";
            ModeLoad();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.New) > 0)
            {
                CustGroupForm F = new CustGroupForm();
                F.flag("", "New");
                F.Show();
                F.ParentRefreshGrid(this);
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end              
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            CustGroupForm F = new CustGroupForm();             
            if (F.PermissionAccess(Login.View) > 0)
            {
                String CustGroupId = dgvCustGroup.CurrentRow.Cells["CustGroupId"].Value.ToString();

                F.flag(CustGroupId, "Edit");
                F.Show();
                F.ParentRefreshGrid(this);
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end            
        }

        private void dgvCustGroup_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                String CustGroupId = dgvCustGroup.Rows[e.RowIndex].Cells["CustGroupId"].Value.ToString();

                CustGroupForm F = new CustGroupForm();
                F.flag(CustGroupId, "Edit");
                F.Show();
                F.ParentRefreshGrid(this);
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
                RefreshGrid();
            }
        }
       
    }
}
