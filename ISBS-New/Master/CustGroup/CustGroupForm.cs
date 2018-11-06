using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.CustGroup
{
    public partial class CustGroupForm : MetroFramework.Forms.MetroForm
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
        String CustGroupId = null;
        Master.CustGroup.CustGroupInquiry Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public CustGroupForm()
        {
            InitializeComponent();
        }

        public void flag(String custgroupid, String mode)
        {
            CustGroupId = custgroupid;
            Mode = mode;
        }

        private String GenerateId()
        {
            Conn = ConnectionString.GetConnection();
            String LastId = "";
            Query = "Select Top 1 (CustGroupId) From dbo.CustGroup order by CustGroupId DESC";

            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                LastId = Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString();
            }

            if (String.IsNullOrEmpty(LastId))
            {
                return "CG0001";
            }
            else
            {
                int temp;
                temp = Int32.Parse(LastId.Substring(2,4)) + 1;

                if (temp < 10)
                {
                    return "CG000" + temp;
                }
                else if (temp < 100)
                {
                    return "CG00" + temp;
                }
                else if (temp < 1000)
                {
                    return "CG0" + temp;
                }
                else if (temp < 10000)
                {
                    return "CG" + temp;
                }
                else
                {
                    return temp.ToString();
                }
            }
        }

        public void ParentRefreshGrid(Master.CustGroup.CustGroupInquiry f)
        {
            Parent = f;
        }

        private void CustGroupForm_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[CustGroup] Where CustGroupId = '" + CustGroupId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtCustGroupId.Text = Dr["CustGroupId"].ToString();
                    txtCustGroupName.Text = Dr["CustGroupName"].ToString();
                    txtKursOrigin.Text = Dr["KursOrigin"].ToString();
                    txtDepositCur.Text = Dr["DepositAmountCurrencyID"].ToString();
                    txtDepositAmount.Text = Dr["DepositAmount"].ToString();
                    txtDPCur.Text = Dr["DPAmountCurrencyID"].ToString();
                    txtDPAmount.Text = Dr["DPAmount"].ToString();
                    txtCreditCur.Text = Dr["CreditLimitCurrencyID"].ToString();
                    txtCreditLimit.Text = Dr["CreditLimit"].ToString();
                    txtCreditLimitSO.Text = Dr["CreditLimitPerSO"].ToString();
                }
            }
            Conn.Close();
        }

        private void ModeLoad()
        {
            RefreshGrid();
        }

        private void resetText()
        {
            txtCustGroupId.Text = "";
            txtCustGroupName.Text = "";
            txtKursOrigin.Text = "";
            txtDepositCur.Text = "";
            txtDepositAmount.Text = "";
            txtDPCur.Text = "";
            txtDPAmount.Text = "";
            txtCreditCur.Text = "";
            txtCreditLimit.Text = "";
            txtCreditLimitSO.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtCustGroupId.Enabled = false;
            txtCustGroupName.Enabled = true;
            txtKursOrigin.Enabled = true;
            txtDepositCur.Enabled = true;
            txtDepositAmount.Enabled = true;
            txtDPCur.Enabled = true;
            txtDPAmount.Enabled = true;
            txtCreditCur.Enabled = true;
            txtCreditLimit.Enabled = true;
            txtCreditLimitSO.Enabled = true;

            btnSearchKurs.Enabled = true;
            btnSearchDPCur.Enabled = true;
            btnSearchDepositCur.Enabled = true;
            btnSearchCreditCur.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtCustGroupId.Enabled = false;
            txtCustGroupName.Enabled = true;
            txtKursOrigin.Enabled = true;
            txtDepositCur.Enabled = true;
            txtDepositAmount.Enabled = true;
            txtDPCur.Enabled = true;
            txtDPAmount.Enabled = true;
            txtCreditCur.Enabled = true;
            txtCreditLimit.Enabled = true;
            txtCreditLimitSO.Enabled = true;

            btnSearchKurs.Enabled = true;
            btnSearchDPCur.Enabled = true;
            btnSearchDepositCur.Enabled = true;
            btnSearchCreditCur.Enabled = true;
            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end             
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtCustGroupName.Text) || String.IsNullOrEmpty(txtKursOrigin.Text))
            {
                MessageBox.Show("Data harus diisi");
                return;
            }

            decimal DepositAmount = string.IsNullOrEmpty(txtDepositAmount.Text) ? 0 : decimal.Parse(txtDepositAmount.Text);
            decimal DPAmount = string.IsNullOrEmpty(txtDPAmount.Text) ? 0 : decimal.Parse(txtDPAmount.Text);
            decimal CreditLimit = string.IsNullOrEmpty(txtCreditLimit.Text) ? 0 : decimal.Parse(txtCreditLimit.Text);
            decimal CreditLimitPerSO = string.IsNullOrEmpty(txtCreditLimitSO.Text) ? 0 : decimal.Parse(txtCreditLimitSO.Text);

            if (Mode == "New")
            {
                

                String TempId = GenerateId();

                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "insert into [dbo].[CustGroup] (CustGroupId, CustGroupName, KursOrigin, DepositAmountCurrencyID, DepositAmount, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerSO) ";
                    Query += "values ('" + TempId + "', '" + txtCustGroupName.Text + "', '" + txtKursOrigin.Text + "', '" + txtDepositCur.Text + "', '" + DepositAmount + "', '" + txtDPCur.Text + "', '" + DPAmount + "', '" + txtCreditCur.Text + "', '" + CreditLimit + "', '" + CreditLimitPerSO + "');";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + TempId + ", berhasil ditambahkan.");
                Parent.RefreshGrid();
                this.Close();
            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "update [dbo].[CustGroup] set CustGroupName ='" + txtCustGroupName.Text + "', KursOrigin='" + txtKursOrigin.Text + "', DepositAmountCurrencyID='" + txtDepositCur.Text + "', DepositAmount='" + DepositAmount + "', ";
                    Query += "DPAmountCurrencyID='" + txtDPCur.Text + "', DPAmount='" + DPAmount + "', CreditLimitCurrencyID='" + txtCreditCur.Text + "', CreditLimit='" + CreditLimit + "', CreditLimitPerSO='" + CreditLimitPerSO + "' where CustGroupId='" + txtCustGroupId.Text + "';";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteScalar();
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + txtCustGroupId.Text + ", berhasil diupdate.");
                Parent.RefreshGrid();
                this.Close();
            }
        }

        private void btnSearchKurs_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtKursOrigin.Text = ConnectionString.Kode;
        }

        private void btnSearchCreditCur_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtCreditCur.Text = ConnectionString.Kode;
        }

        private void btnSearchDepositCur_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtDepositCur.Text = ConnectionString.Kode;
        }

        private void btnSearchDPCur_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtDPCur.Text = ConnectionString.Kode;
        }

        private void txtKursOrigin_Validating(object sender, CancelEventArgs e)
        {
            if (txtKursOrigin.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + txtKursOrigin.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kurs tidak ada di database.");
                        btnSearchKurs_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtCreditCur_Validating(object sender, CancelEventArgs e)
        {
            if (txtCreditCur.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + txtCreditCur.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kurs tidak ada di database.");
                        btnSearchCreditCur_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtDepositCur_Validating(object sender, CancelEventArgs e)
        {
            if (txtDepositCur.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + txtDepositCur.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kurs tidak ada di database.");
                        btnSearchDepositCur_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtDPCur_Validating(object sender, CancelEventArgs e)
        {
            if (txtDPCur.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + txtDPCur.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kurs tidak ada di database.");
                        btnSearchDPCur_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtCustGroupId.Enabled = false;
            txtCustGroupName.Enabled = false;
            txtKursOrigin.Enabled = false;
            txtDepositCur.Enabled = false;
            txtDepositAmount.Enabled = false;
            txtDPCur.Enabled = false;
            txtDPAmount.Enabled = false;
            txtCreditCur.Enabled = false;
            txtCreditLimit.Enabled = false;
            txtCreditLimitSO.Enabled = false;

            btnSearchKurs.Enabled = false;
            btnSearchDPCur.Enabled = false;
            btnSearchDepositCur.Enabled = false;
            btnSearchCreditCur.Enabled = false;
            btnSave.Visible = false;
            btnEdit.Visible = true;
        }

        private void txtCustGroupName_Validating(object sender, CancelEventArgs e)
        {
            if (txtCustGroupName.Text == "")
            {

            }
            else
            {
                if (Mode == "New")
                {
                    Query = "Select * From [dbo].[CustGroup] Where REplace(CustGroupName,' ','')= '" + txtCustGroupName.Text.Replace(" ","") + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        if (Dr.Read())//sama dengan di database
                        {
                            MessageBox.Show("Nama sudah ada di database.");
                            txtCustGroupName.Focus();
                            Conn.Close();
                        }
                        else
                        {
                            Conn.Close();
                        }
                    }
                }
            }
        }

        private void txtCreditLimit_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtCreditLimit.Text))
            {

            }
            else
            {
                MessageBox.Show("Credit Limit Tidak boleh di isi selain angka!!");
                txtCreditLimit.Focus();
                txtCreditLimit.Clear();
            }
        }

        private void txtCreditLimitSO_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtCreditLimitSO.Text))
            {

            }
            else
            {
                MessageBox.Show("Credit LimitPerSO Tidak boleh di isi selain angka!!");
                txtCreditLimitSO.Focus();
                txtCreditLimitSO.Clear();
            }
        }

        private void txtDepositAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtDepositAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("Deposit Amount Tidak boleh di isi selain angka!!");
                txtDepositAmount.Focus();
                txtDepositAmount.Clear();
            }
        }

        private void txtDPAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtDPAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("DP Amount Tidak boleh di isi selain angka!!");
                txtDPAmount.Focus();
                txtDPAmount.Clear();
            }
        }




    }
}
