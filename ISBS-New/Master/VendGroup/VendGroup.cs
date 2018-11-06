using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.VendGroup
{
    public partial class VendGroup : Form
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
        String VendGroupId = null;

        Master.VendGroup.InqVendGroup Parent;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public VendGroup()
        {
            InitializeComponent();
        }

        public void flag(String vendgroupid, String mode)
        {
            VendGroupId = vendgroupid;
            Mode = mode;
        }

        public void setParent(Master.VendGroup.InqVendGroup F)
        {
            Parent = F;
        }

        private void validateNumber(System.Windows.Forms.TextBox F)
        {
            double num;
            if (double.TryParse(F.Text, out num))
            {
                //hasilnya angka
            }
            else
            {
                //selain angka
                MessageBox.Show("Field hanya boleh angka.");
                F.Text = "";
                F.Focus();
            }
        }

        private String GenerateId()
        {
            Conn = ConnectionString.GetConnection();
            String LastId = "";
            Query = "Select Top 1 (VendGroupId) From dbo.VendGroup order by VendGroupId DESC";

            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                LastId = Cmd.ExecuteScalar() == null? "" : Cmd.ExecuteScalar().ToString();
            }

            if (String.IsNullOrEmpty(LastId))
            {
                return "V0001";
            }
            else
            {
                int temp;
                temp = Int32.Parse(LastId.Substring(1,4)) + 1;

                if (temp < 10)
                {
                    return "V000" + temp;
                }
                else if (temp < 100)
                {
                    return "V00" + temp;
                }
                else if (temp < 1000)
                {
                    return "V0" + temp;
                }
                else if (temp < 10000)
                {
                    return "V" + temp;
                }
                else
                {
                    return temp.ToString();
                }
            }
        }

        private void VendGroup_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private bool cekValidasi(String VendGroupId)
        {
            Query = "Select * From [dbo].[VendGroup] Where VendGroupId = '" + VendGroupId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                if (Dr.Read())//sama dengan di database
                {
                    Conn.Close();
                    return false;
                }
                else
                {
                    Conn.Close();
                    return true;
                }
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[VendGroup] Where VendGroupId = '" + VendGroupId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtVendGroupId.Text = Dr["VendGroupId"].ToString();
                    txtVendGroupName.Text = Dr["VendGroupName"].ToString();
                    txtKursOrigin.Text = Dr["KursOrigin"].ToString();
                    txtDepositCur.Text = Dr["DepositAmountCurrencyID"].ToString();
                    txtDepositAmount.Text = Dr["DepositAmount"].ToString();
                    txtDPCur.Text = Dr["DPAmountCurrencyID"].ToString();
                    txtDPAmount.Text = Dr["DPAmount"].ToString();
                    txtCreditCur.Text = Dr["CreditLimitCurrencyID"].ToString();
                    txtCreditLimit.Text = Dr["CreditLimit"].ToString();
                    txtCreditLimitPO.Text = Dr["CreditLimitPerPO"].ToString();
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
            txtVendGroupId.Text = "";
            txtVendGroupName.Text = "";
            txtKursOrigin.Text = "";
            txtDepositCur.Text = "";
            txtDepositAmount.Text = "";
            txtDPCur.Text = "";
            txtDPAmount.Text = "";
            txtCreditCur.Text = "";
            txtCreditLimit.Text = "";
            txtCreditLimitPO.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtVendGroupId.Enabled = false;
            txtVendGroupName.Enabled = true;
            txtKursOrigin.Enabled = true;
            txtDepositCur.Enabled = true;
            txtDepositAmount.Enabled = true;
            txtDPCur.Enabled = true;
            txtDPAmount.Enabled = true;
            txtCreditCur.Enabled = true;
            txtCreditLimit.Enabled = true;
            txtCreditLimitPO.Enabled = true;

            btnSearchKurs.Enabled = true;
            btnSearchDPCur.Enabled = true;
            btnSearchDepositCur.Enabled = true;
            btnSearchCreditCur.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtVendGroupId.Enabled = false;
            txtVendGroupName.Enabled = true;
            txtKursOrigin.Enabled = true;
            txtDepositCur.Enabled = true;
            txtDepositAmount.Enabled = true;
            txtDPCur.Enabled = true;
            txtDPAmount.Enabled = true;
            txtCreditCur.Enabled = true;
            txtCreditLimit.Enabled = true;
            txtCreditLimitPO.Enabled = true;

            btnSearchKurs.Enabled = true;
            btnSearchDPCur.Enabled = true;
            btnSearchDepositCur.Enabled = true;
            btnSearchCreditCur.Enabled = true;
            btnSave.Visible = true;
            btnEdit.Visible = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
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
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            decimal DepositAmount = string.IsNullOrEmpty(txtDepositAmount.Text) ? 0 : decimal.Parse(txtDepositAmount.Text);
            decimal DPAmount = string.IsNullOrEmpty(txtDPAmount.Text) ? 0 : decimal.Parse(txtDPAmount.Text);
            decimal CreditLimit = string.IsNullOrEmpty(txtCreditLimit.Text) ? 0 : decimal.Parse(txtCreditLimit.Text);
            decimal CreditLimitPerPO = string.IsNullOrEmpty(txtCreditLimitPO.Text) ? 0 : decimal.Parse(txtCreditLimitPO.Text);

            if (Mode == "New")
            {
                if (String.IsNullOrEmpty(txtVendGroupName.Text) || String.IsNullOrEmpty(txtKursOrigin.Text))
                {
                    MessageBox.Show("Data harus diisi");
                    return;
                }

            String TempId = GenerateId();

                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    try
                    {
                        Query = "insert into [dbo].[VendGroup] (VendGroupId, VendGroupName, KursOrigin, DepositAmountCurrencyID, DepositAmount, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, CreditLimitPerPO) ";
                        Query += "values ('" + TempId + "', '" + txtVendGroupName.Text + "', '" + txtKursOrigin.Text + "', '" + txtDepositCur.Text + "', '" + DepositAmount + "', '" + txtDPCur.Text + "', '" + DPAmount + "', '" + txtCreditCur.Text + "', '" + CreditLimit + "', '" + CreditLimitPerPO + "');";

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
                    Query = "update [dbo].[VendGroup] set VendGroupName='" + txtVendGroupName.Text + "', KursOrigin='" + txtKursOrigin.Text + "', DepositAmountCurrencyID='" + txtDepositCur.Text + "', DepositAmount='" + DepositAmount + "', ";
                    Query += "DPAmountCurrencyID='" + txtDPCur.Text + "', DPAmount='" + DPAmount + "', CreditLimitCurrencyID='" + txtCreditCur.Text + "', CreditLimit='" + CreditLimit + "', CreditLimitPerPO='" + CreditLimitPerPO + "' where VendGroupId='" + txtVendGroupId.Text + "';";

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
                MessageBox.Show("Data " + txtVendGroupId.Text + ", berhasil diupdate.");
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

        private void txtVendGroupName_Validating(object sender, CancelEventArgs e)
        {
            if (txtVendGroupName.Text == "")
            {

            }
            else
            {
                if (Mode == "New")
                {
                    Query = "Select * From [dbo].[VendGroup] Where REPLACE(VendGroupName,' ','') = '" + txtVendGroupName.Text.Replace(" ", "") + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        if (Dr.Read())//sama dengan di database
                        {
                            MessageBox.Show("Nama sudah ada di database.");
                            txtVendGroupName.Focus();
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
            if (string.IsNullOrEmpty(txtCreditLimit.Text))
            {
                
            }
            else
            {
                validateNumber(txtCreditLimit);
            }
        }

        private void txtCreditLimitPO_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCreditLimitPO.Text))
            {

            }
            else
            {
                validateNumber(txtCreditLimitPO);
            }
        }

        private void txtDepositAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDepositAmount.Text))
            {

            }
            else
            {
                validateNumber(txtDepositAmount);
            }
        }

        private void txtDPAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDPAmount.Text))
            {

            }
            else
            {
                validateNumber(txtDPAmount);
            }
        }
    }
}
