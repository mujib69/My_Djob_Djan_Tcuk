using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.BankGroup
{
    public partial class BankGroupForm : MetroFramework.Forms.MetroForm
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
        String BankGroupId = null;
        //Master.BankGroup.BankGroupInquiry Parent;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //public void flag(String bankgroupid, String mode)
        //{
        //    BankGroupId = bankgroupid;
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
            BankGroupId = id;
        }

        public BankGroupForm()
        {
            InitializeComponent();
        }

        private void BankGroupForm_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[BankGroup] Where BankGroupId = @bankgroupid";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@bankgroupid", BankGroupId);
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtBankGroupId.Text = BankGroupId.ToString();
                    txtBankGroupName.Text = Dr["BankGroupName"].ToString();
                }
            }
            Conn.Close();
        }

        private void ModeNew()
        {
            Mode = "New";
            txtBankGroupId.Enabled = true;
            txtBankGroupName.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnExit.Visible = true;
            btnCancel.Visible = false;
        }

        private void ModeEdit()
        {
            txtBankGroupId.Enabled = false;
            txtBankGroupName.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnCancel.Visible = true;
        }

        public void ModeBeforeEdit()
        {
            Mode = "Edit";
            txtBankGroupId.Enabled = false;
            txtBankGroupName.Enabled = false;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
        }

        //public void ParentRefreshGrid(Master.BankGroup.BankGroupInquiry f)
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
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtBankGroupId.Enabled = false;
            txtBankGroupName.Enabled = false;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;

            GetData();
        }

        private void GetData()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT [BankGroupName] FROM [dbo].[BankGroup] WHERE [BankGroupID] = @bankgroupid";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@bankgroupid", txtBankGroupId.Text);
            txtBankGroupName.Text = Cmd.ExecuteScalar().ToString();
            Conn.Close();
        }

        private bool cekValidasi(String BankGroupId)
        {
            Query = "Select * From [dbo].[BankGroup] Where BankGroupId = @bankgroupid ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@bankgroupid", BankGroupId);
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validasi() == false)
                return;

            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (Mode == "New")
                {
                    Query = "insert into [dbo].[BankGroup] (BankGroupId, BankGroupName, CreatedDate, CreatedBy) ";
                    Query += "values (@bankgroupid, @bankgroupname, getDate(), @UserId);";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@bankgroupid", txtBankGroupId.Text);
                        Cmd.Parameters.AddWithValue("@bankgroupname", txtBankGroupName.Text);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("Data :" + txtBankGroupId.Text + ", berhasil ditambahkan.");
                }
                else if (Mode == "Edit")
                {
                    Query = "update [dbo].[BankGroup] set BankGroupName = @bankgroupname, UpdatedDate = getDate(), UpdatedBy = @UserId where BankGroupId = @bankgroupid";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@bankgroupid", txtBankGroupId.Text);
                        Cmd.Parameters.AddWithValue("@bankgroupname", txtBankGroupName.Text);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);

                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("Data: " + txtBankGroupId.Text + " , berhasil diupdate.");

                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteScalar();
                }
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
            finally
            {               
                Conn.Close();
                ModeBeforeEdit();

                Form f = Application.OpenForms["GlobalInquiry"];
                if (f != null)
                    if (f.Text == "Bank Group")
                        Parent.RefreshGrid();
            }
        }

        private bool Validasi()
        {
            if (txtBankGroupId.Text.Trim() == "")
            {
                MessageBox.Show("Data harus diisi");
                return false;
            }          
            else if (txtBankGroupName.Text.Trim() == "")
            {
                MessageBox.Show("Data harus diisi");
                return false;
            }
            else if (Mode == "New")
            {
                if (cekValidasi(txtBankGroupId.Text) == false)
                {
                    MessageBox.Show("Data sudah ada di database.");
                    return false;
                }
                else
                {
                    txtBankGroupId.Text = txtBankGroupId.Text.Trim().ToUpper();
                    return true;
                }
            }
            else
            {                 
                txtBankGroupId.Text = txtBankGroupId.Text.Trim().ToUpper();
                return true;
            }

        }



    }
}
