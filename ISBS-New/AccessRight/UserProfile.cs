using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;


namespace ISBS_New.AccessRight
{
    public partial class UserProfile : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private string Query = null;
        String UserID;
        String userPassOld, userPassNew, userPassConfirm;
        String emailPassOld, emailPassNew, emailPassConfirm;
        string encryptedEmailPass, decryptedEmailPassOld;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public UserProfile()
        {
            InitializeComponent();
        }

        private void UserProfile_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void UserProfile_Load(object sender, EventArgs e)
        {
            GetDataHeader();
            GetOldPwd();
            ModeBeforeEdit();
        }

        private void ModeBeforeEdit()
        {
            //Header
            txtUserID.Enabled = false;
            txtFullName.Enabled = false;
            txtAtasanKerja.Enabled = false;
            txtUserEmail.Enabled = false;

            //Detail
            txtUserPassOld.Enabled = false;
            txtUserPassNew.Enabled = false;      
            txtUserPassConfirm.Enabled = false;

            txtUserPassOld.Text = userPassOld;
            userPassNew = "";
            txtUserPassNew.Text = userPassNew;
            userPassConfirm = "";
            txtUserPassConfirm.Text = userPassConfirm;

            txtEmailPassOld.Enabled = false;
            txtEmailPassNew.Enabled = false;
            txtEmailPassConfirm.Enabled = false;

            txtEmailPassOld.Text = decryptedEmailPassOld;
            emailPassNew = "";
            txtEmailPassNew.Text = emailPassNew;
            emailPassConfirm = "";
            txtEmailPassConfirm.Text = emailPassConfirm;

            //Button
            btnEditUserID.Visible = true;
            btnCancelUserID.Visible = false;
            btnSaveUserID.Visible = false;
            btnExit.Visible = true;
        }

        private void ModeEdit()
        {
            //Header
            txtUserID.Enabled = false;
            txtFullName.Enabled = false;
            txtAtasanKerja.Enabled = false;
            txtUserEmail.Enabled = false;

            //Detail
            txtUserPassOld.Enabled = false;
            txtUserPassNew.Enabled = true;
            txtUserPassConfirm.Enabled = true;
            txtEmailPassOld.Enabled = false;
            txtEmailPassNew.Enabled = true;
            txtEmailPassConfirm.Enabled = true;

            //Button
            btnEditUserID.Visible = false;
            btnCancelUserID.Visible = true;
            btnSaveUserID.Visible = true;
            btnExit.Visible = true;
        }
        
        public void GetDataHeader()
        {
            Conn = ConnectionString.GetConnection();
            UserID = ControlMgr.UserId;
            txtUserID.Text = UserID;

            Query = "SELECT * FROM sysPass WHERE UserID = '" + UserID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                txtFullName.Text = Dr["FullName"].ToString();
                txtUserEmail.Text = Dr["Email"].ToString();
                txtAtasanKerja.Text = Dr["Head_Department"].ToString();
            }
            Dr.Close();
            Conn.Close();
        }

        public void GetOldPwd()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT * FROM sysPass WHERE UserID = '" + UserID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                userPassOld = Dr["PasswordUser"].ToString();
                emailPassOld = Dr["EmailPass"].ToString();
            }
            Dr.Close();
            //NOTE : TRY CATCH IF THE EMAIL PASS HAVE NOT BEEN ENCRYPTED BEFORE, THEN USE THE NON-ENCRYPTED ONE
            //DELETE IF ALL EMAIL START ENCRYPTED
            try
            {
                decryptedEmailPassOld = StringCipher.Decrypt(emailPassOld);
            }
            catch (Exception ex)
            {
                decryptedEmailPassOld = emailPassOld;
            }
            Conn.Close();
        }

        private void btnEditUserID_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void btnCancelUserID_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit(); 
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSaveUserID_Click(object sender, EventArgs e)
        {
            pwdNullChecker();
            Conn = ConnectionString.GetConnection();

            #region SAVE NEW USER PASS
            if (userPassNew != "" && userPassConfirm != "")
            {
                if (userPassNew == userPassConfirm)
                {
                    Query = "Update dbo.sysPass set PasswordUser='"+ userPassNew +"' , PwdDateChange= getdate() where UserID='"+ UserID +"'";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("User Password berhasil diupdate.");
                }
                if (userPassNew != userPassConfirm)
                {
                    MessageBox.Show("User Password New dan Confirm tidak sama!" + Environment.NewLine + "Silahkan coba lagi.");
                    return;
                }
            }
            #endregion

            #region SAVE NEW EMAIL PASS
            encryptedEmailPass = StringCipher.encrypt(emailPassNew);

            if (emailPassNew != "" && emailPassConfirm != "")
            {
                if (emailPassNew == emailPassConfirm)
                {
                    Query = "Update dbo.sysPass set EmailPass='" + encryptedEmailPass + "' where UserID='" + UserID + "'";

                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("Password Email berhasil diupdate.");
                    
                }
                if (emailPassNew != emailPassConfirm)
                {
                    MessageBox.Show("Email Password New dan Confirm tidak sama!" + Environment.NewLine + "Silahkan coba lagi.");
                    return;
                }
            }
            Conn.Close();
            GetOldPwd();
            GetDataHeader();
            ModeBeforeEdit();      
            #endregion
        }

        private void pwdNullChecker()
        {
            userPassNew = txtUserPassNew.Text;
            userPassConfirm = txtUserPassConfirm.Text;
            emailPassNew = txtEmailPassNew.Text;
            emailPassConfirm = txtEmailPassConfirm.Text;

            if (userPassNew != "" && userPassConfirm == "")
            {
                MessageBox.Show("Isi Confirm User Pass.");
                return;
            }

            if (userPassNew == "" && userPassConfirm != "")
            {
                MessageBox.Show("Isi New User Pass.");
                return;
            }

            if (emailPassNew != "" && emailPassConfirm == "")
            {
                MessageBox.Show("Isi Confirm Email Pass.");
                return;
            }

            if (emailPassNew == "" && emailPassConfirm != "")
            {
                MessageBox.Show("Isi New Email Pass.");
                return;
            }

            if (userPassNew == "" && userPassConfirm == "" && emailPassNew == "" && emailPassConfirm == "")
            {
                MessageBox.Show("Isi data yang akan diganti atau Cancel.");
                return;
            }
        }
    }
}
