using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.Bank
{
    public partial class BankForm : MetroFramework.Forms.MetroForm
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
        String AccountId = null;
        string vOldAccountNo, vOldName, vOldTxt;

        //Master.Bank.BankInquiry Parent;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //public void flag(String accountid, String mode)
        //{
        //    AccountId = accountid;
        //    Mode = mode;
        //}

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            AccountId = id;
        }

        public BankForm()
        {
            InitializeComponent();
        }

        private void BankForm_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
            if (Mode == "Edit")
            {
                ModeEdit();
            }
            if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[BankAccountTable] Where AccountId = '" + AccountId + "' ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtAccountId.Text = AccountId.ToString();
                    txtBankGroupId.Text = Dr["BankGroupId"].ToString();
                    txtAccountNo.Text = Dr["AccountNo"].ToString();
                    txtName.Text = Dr["Name"].ToString();
                    txtTxt.Text = Dr["Txt"].ToString();
                }
            }
            Conn.Close();
        }

        private void ModeLoad()
        {
            RefreshGrid();
        }

        private void ModeNew()
        {
            Mode = "New";
            txtAccountId.Enabled = false;
            txtBankGroupId.Enabled = false;
            txtBankGroupName.Enabled = false;
            txtAccountNo.Enabled = true;
            txtName.Enabled = true;
            txtTxt.Enabled = true;

            btnSearchBank.Enabled = true;     
            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = false;
        }

        private void ModeEdit()
        {
            txtAccountId.Enabled = false;
            txtBankGroupId.Enabled = false;
            txtBankGroupName.Enabled = false;
            txtAccountNo.Enabled = true;
            txtName.Enabled = true;
            txtTxt.Enabled = true;

            btnSearchBank.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = true;

            GetNamaBank();
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            txtAccountId.Enabled = false;
            txtBankGroupId.Enabled = false;
            txtBankGroupName.Enabled = false;
            txtAccountNo.Enabled = false;
            txtName.Enabled = false;
            txtTxt.Enabled = false;

            btnSearchBank.Enabled = false;
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnCancel.Visible = false;

            GetNamaBank();
        }

        public void GetNamaBank()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT BankGroupName FROM [dbo].[BankGroup] Where BankGroupId = '" + txtBankGroupId.Text + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtBankGroupName.Text = Dr["BankGroupName"].ToString();
            }
            Dr.Close();
        }

        public int GetIdNum()
        {
            int id = 0;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT TOP 1(RIGHT(AccountId,5)) FROM [dbo].[BankAccountTable] ORDER BY AccountId DESC";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                id = Convert.ToInt32(Dr.GetString(0));
                return id;
            }
            return id;
        }

        //public void ParentRefreshGrid(Master.Bank.BankInquiry f)
        //{
        //    Parent = f;
        //}

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {  
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                ModeEdit();
                Mode = "Edit";
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end

            vOldAccountNo = txtAccountNo.Text;
            vOldName = txtName.Text;
            vOldTxt = txtTxt.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtBankGroupId.Enabled = false;
            txtBankGroupId.Enabled = false;
            txtAccountId.Enabled = false;
            txtAccountNo.Enabled = false;
            txtName.Enabled = false;
            txtTxt.Enabled = false;

            txtAccountNo.Text = vOldAccountNo;
            txtName.Text = vOldName;
            txtTxt.Text = vOldTxt;

            btnSearchBank.Enabled = false;
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
        }

        private bool cekValidasi(String BankGroupId, String AccountNo)
        {
            Query = "Select * From [dbo].[BankAccountTable] Where BankGroupId = '" + BankGroupId + "' and AccountNo = '"+ AccountNo +"'";

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

        private Boolean Validasi()
        {
            if (txtBankGroupId.Text.Trim() == "")
            {
                MessageBox.Show("Bank Group Id harus di isi");
                return false;
            }

            else if (txtAccountNo.Text.Trim() == "")
            {
                MessageBox.Show("Account No harus di isi");
                return false;
            }

            else if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Nama harus di isi");
                return false;
            }

            else if (txtTxt.Text.Trim() == "")
            {
                MessageBox.Show("Txt harus di isi");
                return false;
            }

            else if (txtBankGroupId.Text.Trim() == "")
            {
                MessageBox.Show("Bank Group Id harus di isi");
                return false;
            }
            else if (!CheckAvailability())
            {
                return false;
            }
            else
                return true;
        }

        private Boolean CheckAvailability()
        {
            Boolean vBol = false;
            string CekData;

            Conn = ConnectionString.GetConnection();
            //Validasi jika AccountNo sudah digunakan
            Query = "Select AccountNo from BankAccountTable where AccountNo = @accountno";
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@accountno", txtAccountNo.Text.Trim().ToUpper());
                CekData = Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString();
            }

            if (CekData != "" && Mode != "Edit")
            {
                MessageBox.Show("Account No " + txtAccountNo.Text.Trim().ToUpper() + " sudah digunakan, silahkan diganti dengan yang lain.");
                Conn.Close();
                vBol = false;
            }
            else
                vBol = true;

            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!Validasi())
                return;

            int newid = GetIdNum() + 1;

            try
            {                
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
         
                //Jika New
                if (Mode == "New")
                {                  
                    Query = "Insert into dbo.BankAccountTable ";
                    Query += "(BankGroupId, AccountId, AccountNo, Name, Txt, CreatedDate, CreatedBy) values ";
                    Query += "(@bankgroupid, @accountid, @accountno, @name, @txt, getdate(), @UserId);";
                    
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@bankgroupid", txtBankGroupId.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@accountid", newid.ToString().PadLeft(5, '0'));
                        Cmd.Parameters.AddWithValue("@accountno", txtAccountNo.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@name", txtName.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@txt", txtTxt.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);

                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("AccountNo = " + txtAccountNo.Text.Trim().ToUpper() + Environment.NewLine + "Name = " + txtName.Text.Trim().ToUpper() + Environment.NewLine + "BankGroupId = " + txtBankGroupId.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                    txtAccountId.Text = newid.ToString().PadLeft(5, '0');
                }

                //Jika Edit
                else if (Mode == "Edit")
                {
                    Query = "Update dbo.BankAccountTable set AccountID = @accountid, AccountNo= @accountno, Name = @name, Txt = @txt, UpdatedDate = getdate(), UpdatedBy = @UserId where AccountId='" + txtAccountId.Text + "'";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        //Cmd.Parameters.AddWithValue("@accountid", newid.ToString().PadLeft(5, '0'));
                        Cmd.Parameters.AddWithValue("@accountid", txtAccountId.Text);
                        Cmd.Parameters.AddWithValue("@accountno", txtAccountNo.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@name", txtName.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@txt", txtTxt.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);

                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("AccountId = " + txtAccountId.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diupdate.");
                }
            }
            //catch (Exception Ex)
            //{
            //    Trans.Rollback();
            //    MessageBox.Show(ConnectionString.GlobalException(Ex));
            //}
            finally
            {
                Conn.Close();                             
            }
            ModeBeforeEdit();   

            Form f = Application.OpenForms["GlobalInquiry"];
            if (f != null)
                if (f.Text == "Bank")
                    Parent.RefreshGrid();
        }
    
        

        private void btnSearchBank_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "BankGroup";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.Text = "Search Bank";
            tmpSearch.ShowDialog();
            if (ConnectionString.Kode != "")
            {
                txtBankGroupId.Text = ConnectionString.Kode;
                txtBankGroupName.Text = ConnectionString.Kode2;
            }
            ConnectionString.Kode = "";
        }

        //private void btnSearch_Click(object sender, EventArgs e)
        //{
        //    if (rbVendor.Checked == false && rbCustomer.Checked == false)
        //    {
        //        MessageBox.Show("Pilih salah satu dari [Vendor | Customer ]");
        //    }
        //    else if (rbVendor.Checked == true)
        //    {
        //        string SchemaName = "dbo";
        //        string TableName = "VendTable";
               
        //        Search tmpSearch = new Search();
        //        tmpSearch.SetSchemaTable(SchemaName, TableName);
        //        tmpSearch.ShowDialog();
        //        txtAccountId.Text = ConnectionString.Kode;
        //        txtName.Text = ConnectionString.Kode2;
        //    }
        //    else if (rbCustomer.Checked == true)
        //    {
        //        string SchemaName = "dbo";
        //        string TableName = "CustTable";

        //        Search tmpSearch = new Search();
        //        tmpSearch.SetSchemaTable(SchemaName, TableName);
        //        tmpSearch.ShowDialog();
        //        txtAccountId.Text = ConnectionString.Kode;
        //        txtName.Text = ConnectionString.Kode2;
        //    }

        //}

        private void txtAccountNo_TextChanged(object sender, EventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.-]*$");
            if (strPattern.IsMatch(txtAccountNo.Text))
            {

            }
            else
            {
                MessageBox.Show("Tidak boleh di isi selain angka!!");
                txtAccountNo.Text = txtAccountNo.Text.Substring(0, txtAccountNo.Text.Length - 1);
            }
        }

        private void BankForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshGrid();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           

        }
    }
}
