using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;

namespace ISBS_New.Master.User.UserId
{
    public partial class UserIdForm : Form
    {
        public static string IdPrimary = "";
        //SQL Function
        private SqlConnection Conn;
        private SqlTransaction Trans;

        //private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        private int Index;

        //Param RefreshGrid
        public Form tmpForm;
        public string SchemaName = "User";
        public string TableName = "User";
        public Boolean EditStatus = false;
        public UserIdForm()
        {
            InitializeComponent();
        }

        //Mode
        string Variable = "";
        //Check Primary Id
        string CheckPKId = "";
        //Check Parent Class
        Master.User.UserId.UserIdDashboard Parent;

        private void Edit_Load(object sender, EventArgs e)
        {
            try
            {
                this.Location = new Point(148, 47);
                this.Text = "Form " + SchemaName + " " + TableName;
                CheckPKId = ConnectionString.IdSearchPK;
                if (ConnectionString.IdSearchPK != "") //Detect apakah new atau view
                {
                    ModeView();
                }
                else
                {
                    ModeNew();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Validasi jika kosong
                if (txtUserId.Text == "")
                {
                    MessageBox.Show("Data User Id tidak boleh kosong.");
                    return;
                }
                else if (txtUserId.Text.Trim() == "")
                {
                    MessageBox.Show("Data User Name tidak boleh kosong.");
                    return;
                }
                else if (txtUserPassword.Text.Trim() == "")
                {
                    MessageBox.Show("Data Password tidak boleh kosong.");
                    return;
                }
                else if (txtRePassword.Text.Trim() == "")
                {
                    MessageBox.Show("Data Re-Password tidak boleh kosong.");
                    return;
                }
                else if (txtUserPassword.Text.Trim() != txtRePassword.Text.Trim())
                {
                    MessageBox.Show("Data Re Password tidak boleh kosong.");
                    return;
                }

                //Cek Data apakah sudah ada
                string CekData;
                Conn = ConnectionString.GetConnection();

                Query = "Select User Id from [" + SchemaName + "].[" + TableName + "] where UserId='" + txtUserId.Text.Trim() + "' and UserId<>'" + CheckPKId + "'";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    CekData = Command.ExecuteScalar() == null ? "" : Command.ExecuteScalar().ToString();
                }


                //Jika UserId sudah ada
                if (CekData != "")
                {
                    MessageBox.Show("User Id sudah digunakan, silahkan diganti dengan yang lain.");
                    Conn.Close();
                    return;
                }

                //Jika save baru
                string EncryptPass = ConnectionString.Encrypt(txtUserPassword.Text.Trim());

                if (Variable == "New")
                {
                    Query = "Insert into [" + SchemaName + "].[" + TableName + "] (UserId,UserId,Password,CreatedDate,CreatedBy) values ('" + txtUserId.Text.Trim() + "','" + txtUserId.Text.Trim() + "','" + EncryptPass + "',getdate(),'" + ControlMgr.UserId + "')";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("User Id : " + txtUserId.Text.Trim() + " berhasil ditambahkan.");
                }

                //Jika edit data
                else if (Variable == "Edit")
                {

                    Query = "Update [" + SchemaName + "].[" + TableName + "] set UserId='" + txtUserId.Text.Trim() + "', UserId='" + txtUserId.Text.Trim() + "', Password='" + EncryptPass + "', UpdatedDate =getdate(), UpdatedBy ='" + ControlMgr.UserId + "' where UserId='" + CheckPKId + "'";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("User Id : " + txtUserId.Text.Trim() + " berhasil diedit.");
                }
                this.Close();
                Parent.RefreshGrid();
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            { 
                Conn.Close();
            }
        }

        public void ParentRefreshGrid(Master.User.UserId.UserIdDashboard F)
        {
            Parent = F;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void ModeView()
        {
            try
            {
                Variable = "View";
                txtUserId.Enabled = false;
                txtUserId.Enabled = false;
                txtUserPassword.Enabled = false;
                txtRePassword.Enabled = false;

                btnSave.Visible = false;
                btnEdit.Visible = true;
                btnExit.Visible = true;

                string tmpFieldPK = "";
                string tmpDisplay = "";
                int i = 0;

                //Get PK
                Conn = ConnectionString.GetConnection();
                Query = "Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = '" + TableName + "'" + " and PK=1 order by OrderNo";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 0;

                    while (Dr.Read())
                    {
                        if (i == 0)
                        {
                            //IdName = Dr[0].ToString();
                            tmpFieldPK = Dr[0].ToString();
                        }
                        else
                        {
                            tmpFieldPK += " or " + Dr[0].ToString();
                        }
                        i += 1;
                    }
                    Dr.Close();
                }

                //Get Field Display
                Conn = ConnectionString.GetConnection();
                Query = "Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = '" + TableName + "'" + " order by OrderNo";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 0;

                    while (Dr.Read())
                    {
                        if (i == 0)
                        {
                            tmpDisplay = Dr[0].ToString(); // + "='" + dgvSearch.Rows[Index].Cells[Dr[0].ToString()].Value.ToString() + "'";
                        }
                        else
                        {
                            tmpDisplay += "," + Dr[0].ToString(); //+ "='" + dgvSearch.Rows[Index].Cells[Dr[0].ToString()].Value.ToString() + "'";
                        }
                        i += 1;
                    }
                    Dr.Close();
                }

                //Get Data
                Query = "Select " + tmpDisplay + " from [" + SchemaName + "].[" + TableName + "] where " + tmpFieldPK + "='" + ConnectionString.IdSearchPK + "';";
                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();

                    while (Dr.Read())
                    {
                        txtUserId.Text = Dr["UserId"].ToString();
                        txtUserId.Text = Dr["UserId"].ToString();
                        txtUserPassword.Text = Dr["Password"].ToString();
                        txtRePassword.Text = Dr["Password"].ToString();
                    }
                    Dr.Close();
                }
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }

        private void ModeNew()
        {
            Variable = "New";
            txtUserId.Enabled = true;
            txtUserId.Enabled = true;
            txtUserPassword.Enabled = true;
            txtRePassword.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnExit.Visible = true;
        }

        private void ModeEdit()
        {
            Variable = "Edit";
            txtUserId.Enabled = true;
            txtUserId.Enabled = true;
            txtUserPassword.Enabled = true;
            txtRePassword.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnExit.Visible = true;
        }

    }
}
