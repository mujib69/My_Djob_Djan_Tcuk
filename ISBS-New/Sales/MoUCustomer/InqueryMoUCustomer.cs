using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Sales.MoUCustomer
{
    public partial class InqueryMoUCustomer : MetroFramework.Forms.MetroForm
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
        public static int dataShow;

        public int DataShow { get { return dataShow; } set { dataShow = value; } }

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public InqueryMoUCustomer()
        {
            InitializeComponent();           
        }

        private void InquiryMoUCustomer_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            gvheader();
        }

        private void InquiryMoUCustomer_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'CustMouH'";

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

            cmbCriteria.SelectedIndex = 0;

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

        private void gvheader()
        {
            dgvMoU.Columns["MoUDate"].HeaderText = "MoU Date";
            dgvMoU.Columns["MoUNo"].HeaderText = "MoU No";
            dgvMoU.Columns["CustID"].HeaderText = "Cust ID";
            dgvMoU.Columns["CustName"].HeaderText = "Cust Name";
            dgvMoU.Columns["TotalAmount"].HeaderText = "Total Amount";
            dgvMoU.Columns["ValidTo"].HeaderText = "Valid To";
            dgvMoU.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvMoU.Columns["CreatedBy"].HeaderText = "Created By";
            dgvMoU.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvMoU.Columns["UpdatedBy"].HeaderText = "Updated By";
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                Query = "SELECT No, MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY MoUNo desc) No, MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy FROM CustMouH) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT No, MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY MoUNo desc) [No], MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy  From [dbo].[CustMouH] where ";
                Query += "MoUNo like '%" + txtSearch.Text + "%' OR CustID like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("MoUNo"))
            {
                Query = "SELECT No, MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY MoUNo desc) [No], MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy  From [dbo].[CustMouH] where ";
                Query += "MoUNo like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("CustID"))
            {
                Query = "SELECT No, MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY MoUNo desc) [No], MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy  From [dbo].[CustMouH] where ";
                Query += "CustID like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("MoUDate"))
            {
                Query = "SELECT No, MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY MoUNo desc) [No], MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy  From [dbo].[CustMouH] where ";
                Query += "(CONVERT(VARCHAR(10),MoUDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), MoUDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "SELECT No, MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY MoUNo desc) [No], MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy  From [dbo].[CustMouH] where ";
                Query += crit + " Like '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvMoU.AutoGenerateColumns = true;
            dgvMoU.DataSource = Dt;
            dgvMoU.Refresh();
            dgvMoU.AutoResizeColumns();
            dgvMoU.Columns["MoUDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvMoU.Columns["ValidTo"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvMoU.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvMoU.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(MoUNo) From (Select ROW_NUMBER() OVER (ORDER BY MoUNo) No, MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy  From [dbo].[CustMouH] ) a ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(MoUNo) From (Select ROW_NUMBER() OVER (ORDER BY MoUNo) No, ";
                Query += "MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[CustMouH] ";
                Query += "WHERE MoUNo like '%" + txtSearch.Text + "%' OR CustID like '%" + txtSearch.Text + "%') a ";
            }
            else if (crit.Equals("MoUNo"))
            {
                Query = "Select Count(MoUNo) From (Select ROW_NUMBER() OVER (ORDER BY MoUNo) No, ";
                Query += "MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[CustMouH] ";
                Query += "WHERE MoUNo like '%" + txtSearch.Text + "%') a ";
            }
            else if (crit.Equals("CustID"))
            {
                Query = "Select Count(MoUNo) From (Select ROW_NUMBER() OVER (ORDER BY MoUNo) No, ";
                Query += "MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[CustMouH] ";
                Query += "WHERE CustID like '%" + txtSearch.Text + "%') a ";
            }
            else if (crit.Equals("MoUDate"))
            {
                Query = "Select Count(MoUNo) From (Select ROW_NUMBER() OVER (ORDER BY MoUNo) No, ";
                Query += "MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[CustMouH] ";
                Query += "WHERE (CONVERT(VARCHAR(10),MoUDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),MoUDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
            }
            else
            {
                Query = "Select Count(MoUNo) From (Select ROW_NUMBER() OVER (ORDER BY MoUNo) No, ";
                Query += "MoUDate, MoUNo, CustID, CustName, TotalAmount, ValidTo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[CustMouH] Where ";
                Query += crit + " Like '%" + txtSearch.Text + "%') a ";           
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.refreshTaskList();
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

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void cmbCriteria_SelectedIndexChanged_1(object sender, EventArgs e)
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

            RefreshGrid();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                HeaderMoUCustomer HeaderMoUCustomer = new HeaderMoUCustomer();
                HeaderMoUCustomer.SetMode("New", "");
                HeaderMoUCustomer.SetParent(this);
                HeaderMoUCustomer.Show();
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
            SelectPLJ();
        }

        private void SelectPLJ()
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            HeaderMoUCustomer header = new HeaderMoUCustomer();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvMoU.RowCount > 0)
                {
                   header.SetMode("BeforeEdit", dgvMoU.CurrentRow.Cells["MoUNo"].Value.ToString());
                    header.SetParent(this);
                    header.Show();
                    RefreshGrid();
                }
                else
                {
                    MessageBox.Show("Silahkan pilih data");
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private bool cekValidasi(string MoUNumber)
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (vBol == true)
            {
                try
                {
                    Query = "Select * From CustMouH Where MouNo='" + MoUNumber + "'";
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
                            ErrMsg = "MoU tidak ditemukan..";
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
                    Query = "SELECT SalesMouNo FROM SalesQuotationH WHERE SalesMouNo='" + MoUNumber + "' ";
                    Query += "UNION ALL SELECT SalesMouNo FROM SalesOrderH WHERE SalesMouNo='" + MoUNumber + "' ";
                    Query += "UNION ALL SELECT SalesMouNo FROM SalesAgreementH WHERE SalesMouNo='" + MoUNumber + "' ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "MoU sudah pernah digunakan..";
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
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvMoU.RowCount > 0)
                    {
                        Index = dgvMoU.CurrentRow.Index;
                        string MoUNumber = dgvMoU.Rows[Index].Cells["MoUNo"].Value == null ? "" : dgvMoU.Rows[Index].Cells["MoUNo"].Value.ToString();

                        if (cekValidasi(MoUNumber) == false)
                        {
                            return;
                        }

                        DialogResult dr = MessageBox.Show("MoU Number = " + MoUNumber + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();

                            //delete detail
                            Query = "Delete from [dbo].[CustMouH] where MoUNo ='" + MoUNumber + "';";


                            //delete header
                            Query += "Delete from [dbo].[CustMou_Dtl] where MoUNo='" + MoUNumber + "';";


                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("MoU Number = " + MoUNumber.ToUpper() + "\n" + "Data berhasil dihapus.");

                            Index = 0;
                            Conn.Close();
                            RefreshGrid();

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void dgvMoU_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectPLJ();
        }

    }
}
