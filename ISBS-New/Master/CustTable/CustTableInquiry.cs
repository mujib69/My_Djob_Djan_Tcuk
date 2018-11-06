using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.CustTable
{
    public partial class CustTableInquiry : MetroFramework.Forms.MetroForm
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

        public CustTableInquiry()
        {
            InitializeComponent();
        }

        private void CustTableInquiry_Load(object sender, EventArgs e)
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
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CustId) No, CustID, CustName, TaxName, NPWP, PKP, CompanyGroupID, TempoBayar, PaymentModeID, TaxGroup, ReffBank1Id, ReffBank2Id, ReffFullItemId1, ReffFullItemId2, ReffFullItemId3, ReffFullItemId4, ReffFullItemId5, CurrencyID, CustGroupID, DepositAmountAffiatedtoGroup, DepositAmountCurrencyID, DepositAmount, DepositType, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerSO, CompanyStatusId, LastStatusChange, CashBackBalance, DiscountBalance, DebitNoteBalance, PurchaseAmount, SOAmount, DOAmount, PaymentAmount, ChequeAmount, EstablishedFor, Survey, DateOfArchive From [dbo].[CustTable]) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CustId) No, CustID, CustName, TaxName, NPWP, PKP, CompanyGroupID, TempoBayar, PaymentModeID, TaxGroup, ReffBank1Id, ReffBank2Id, ReffFullItemId1, ReffFullItemId2, ReffFullItemId3, ReffFullItemId4, ReffFullItemId5, CurrencyID, CustGroupID, DepositAmountAffiatedtoGroup, DepositAmountCurrencyID, DepositAmount, DepositType, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerSO, CompanyStatusId, LastStatusChange, CashBackBalance, DiscountBalance, DebitNoteBalance, PurchaseAmount, SOAmount, DOAmount, PaymentAmount, ChequeAmount, EstablishedFor, Survey, DateOfArchive  From [dbo].[CustTable] Where CustId Like '%" + txtSearch.Text + "%' or CustName Like '%" + txtSearch.Text + "%' or TaxName Like '%" + txtSearch.Text + "%' or NPWP Like '%" + txtSearch.Text + "%' or PKP Like '%" + txtSearch.Text + "%' or CompanyGroupID Like '%" + txtSearch.Text + "%' ";
                Query += "or TempoBayar Like '%" + txtSearch.Text + "%' or PaymentModeID Like '%" + txtSearch.Text + "%' or TaxGroup Like '%" + txtSearch.Text + "%' or ReffBank1ID Like '%" + txtSearch.Text + "%' or ReffBank2ID Like '%" + txtSearch.Text + "%'or ReffFullItemID1 Like '%" + txtSearch.Text + "%' or ReffFullItemID2 Like '%" + txtSearch.Text + "%' or ReffFullItemID3 Like '%" + txtSearch.Text + "%' or ReffFullItemID4 Like '%" + txtSearch.Text + "%' or ReffFullItemID5 Like '%" + txtSearch.Text + "%' or CurrencyID Like '%" + txtSearch.Text + "%' or CustGroupID Like '%" + txtSearch.Text + "%' or CustGroupID Like '%" + txtSearch.Text + "%' or DepositAmountAffiatedtoGroup Like '%" + txtSearch.Text + "%' or DepositAmountCurrencyID Like '%" + txtSearch.Text + "%' or CustGroupID Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY CustId) No, CustID, CustName, TaxName, NPWP, PKP, CompanyGroupID, TempoBayar, PaymentModeID, TaxGroup, ReffBank1Id, ReffBank2Id, ReffFullItemId1, ReffFullItemId2, ReffFullItemId3, ReffFullItemId4, ReffFullItemId5, CurrencyID, CustGroupID, DepositAmountAffiatedtoGroup, DepositAmountCurrencyID, DepositAmount, DepositType, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerSO, CompanyStatusId, LastStatusChange, CashBackBalance, DiscountBalance, DebitNoteBalance, PurchaseAmount, SOAmount, DOAmount, PaymentAmount, ChequeAmount, EstablishedFor, Survey, DateOfArchive  From [dbo].[CustTable] Where " + cmbCriteria.Text + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvCustTable.AutoGenerateColumns = true;
            dgvCustTable.DataSource = Dt;
            dgvCustTable.Refresh();
            dgvCustTable.Columns["DateOfArchive"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvCustTable.AutoResizeColumns();
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(*) From [dbo].[CustTable]";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(*) From (Select ROW_NUMBER() OVER (ORDER BY CustId) No, * From [dbo].[CustTable] Where CustID Like '%" + txtSearch.Text + "%' or CustName Like '%" + txtSearch.Text + "%' or TaxName Like '%" + txtSearch.Text + "%' or NPWP Like '%" + txtSearch.Text + "%' or PKP Like '%" + txtSearch.Text + "%' or CompanyGroupID Like '%" + txtSearch.Text + "%' ";
                Query += "or TempoBayar Like '%" + txtSearch.Text + "%' or PaymentModeID Like '%" + txtSearch.Text + "%' or TaxGroup Like '%" + txtSearch.Text + "%' or ReffBank1ID Like '%" + txtSearch.Text + "%' or CurrencyID Like '%" + txtSearch.Text + "%' or CustGroupID Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select Count(*) From [dbo].[CustTable] Where " + cmbCriteria.Text + " Like '%" + txtSearch.Text + "%'";
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
            Query = "Select FieldName From [User].[Table] Where TableName = 'CustTable'";

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
                    if (dgvCustTable.RowCount > 0)
                    {
                        Index = dgvCustTable.CurrentRow.Index;
                        String CustId = dgvCustTable.Rows[Index].Cells["CustId"].Value == null ? "" : dgvCustTable.Rows[Index].Cells["CustId"].Value.ToString();

                        DialogResult dr = MessageBox.Show("Cust ID = " + CustId.ToUpper() + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            //delete header
                            Query = "Delete from [dbo].[CustTable] where CustId='" + CustId + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            //delete Address
                            Query = "Delete from [dbo].[Address] where ReffTableName = 'CustTable' and ReffId = '" + CustId + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            //delete Contact
                            Query = "Delete from [dbo].[Contact] where ReffTableName = 'CustTable' and ReffRecId = '" + CustId + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("Cust ID = " + CustId.ToUpper() + "\n" + "Data berhasil dihapus.");

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
                CustTableForm F = new CustTableForm();
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
            CustTableForm F = new CustTableForm();              
            if (F.PermissionAccess(Login.View) > 0)
            {
                String CustId = dgvCustTable.CurrentRow.Cells["CustId"].Value.ToString();

                F.flag(CustId, "Edit");
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

        private void dgvCustTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                String CustId = dgvCustTable.Rows[e.RowIndex].Cells["CustId"].Value.ToString();

                CustTableForm F = new CustTableForm();
                F.flag(CustId, "Edit");
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
