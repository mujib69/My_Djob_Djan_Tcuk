using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Utility
{
    public partial class Frm_Login : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        SqlDataReader drtblPass;
        SqlDataReader drtblPassUserGroup;
        string strSql = "";


        public Frm_Login()
        {
            InitializeComponent();
            txtUserID.Text = "ITDIVISI";
            txtPassword.Text = "rahasia123";
        }

        private void Frm_Login_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;

            string Remember = ISBS_New.Properties.Settings.Default.Type;

            if (Remember == "Remember")
            {
                ChkRemember.Checked = true;
                txtUserID.Text = ISBS_New.Properties.Settings.Default.UserID;
                txtPassword.Text = ISBS_New.Properties.Settings.Default.Password;
                mBtnLogin.Focus();
            }
            else
            {
                txtUserID.Focus();
            }
        }

        private Boolean CekUserGroup()
        {
            Boolean vBol = true;
            try
            {
                strSql = "SELECT * FROM tblPassUserGroup WHERE tblPassUserGroup_UserId=@UserId";
                using (SqlCommand cmdtblPassUserGroup = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblPassUserGroup.Parameters.AddWithValue("@UserId", txtUserID.Text.Trim());
                    drtblPassUserGroup = cmdtblPassUserGroup.ExecuteReader();
                    if (!drtblPassUserGroup.HasRows)
                    {
                        vBol = false;
                    }
                    else
                    {
                        while (drtblPassUserGroup.Read())
                        {
                            ControlMgr.GroupName = (string)drtblPassUserGroup["tblPassUserGroup_GroupName"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                drtblPassUserGroup.Close();
            }
            return vBol;
        }

        private Boolean ValidDate(string _Today)
        {
            Boolean vBol = true;
            string TglServer="";
            ConnMaster=ConnectionString.GetConnection();
            strSql = "SELECT CAST(DATEPART(YEAR, GETDATE()) AS VARCHAR)+ '-' +CAST(DATEPART(MONTH, GETDATE()) AS VARCHAR) + '-' + CAST(DATEPART(DAY, GETDATE()) AS VARCHAR) AS Today";
            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
            {
                drtblPass = Cmd.ExecuteReader();
                if (drtblPass.HasRows)
                {
                    while (drtblPass.Read())
                    { 
                        TglServer=(string)drtblPass["Today"];
                    }
                }
                drtblPass.Close();
            }

            if (_Today != TglServer)
            {
                MessageBox.Show("Tanggal Komputer : " + _Today + " tidak sesuai dengan server : " + TglServer);
                vBol = false;
            }
            else
            {
                vBol = true;
            }
            ConnMaster.Close();
            return vBol;
        }

        private Boolean CekUserAktif()
        {
            Boolean vBol = true;
            SqlDataReader Dr;
            SqlConnection Conn = ConnectionString.GetConnection();
            strSql = "SELECT tblPass_Aktif FROM tblPass WHERE tblPass_UserId=@UserId AND tblPass_Aktif=1";
            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
            {
                Cmd.Parameters.AddWithValue("@UserId",ControlMgr.UserId);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    vBol = true;
                }
                else
                {
                    vBol = false;
                }
                Dr.Close();
            }
            Conn.Close();
            return vBol;
        }

        private void mBtnLogin_Click(object sender, EventArgs e)
        {
            if (txtUserID.Text.Trim() == "" || Convert.IsDBNull(txtUserID.Text))
            {
                MessageBox.Show("User Id tidak boleh kosong..");
                txtUserID.Focus();
                return;
            }

            if (txtPassword.Text.Trim() == "" || Convert.IsDBNull(txtPassword.Text))
            {
                MessageBox.Show("Password tidak boleh kosong..");
                txtUserID.Focus();
                return;
            }

            if (System.Globalization.CultureInfo.CurrentCulture.ToString() != "en-US")
            {
                MessageBox.Show("Ubah Regional Setting ke United State..");
                return;
            }

            string Today=DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            if (!ValidDate(Today))
            {
                return;
            }           

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblPass WHERE tblPass_UserId=@UserId AND tblPass_PasswordUser=@Password";
                using (SqlCommand cmtblPass = new SqlCommand(strSql, ConnMaster))
                {
                    cmtblPass.Parameters.AddWithValue("@UserId",txtUserID.Text.Trim());
                    cmtblPass.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    drtblPass = cmtblPass.ExecuteReader();
                    if (drtblPass.HasRows)
                    {
                        while (drtblPass.Read())
                        {
                            string Password = Convert.IsDBNull(drtblPass["tblPass_PasswordUser"]) ? "" : (string)drtblPass["tblPass_PasswordUser"];
                            if(Password!=txtPassword.Text.Trim())
                            {                                         
                                MessageBox.Show("User Id atau Password salah..");
                                return;
                            }

                            ControlMgr.UserId = txtUserID.Text.Trim();

                            if (Password == "123")
                            {
                                MessageBox.Show("Password Default Harus Diganti, Setelah itu Login Ulang");
                                Form Frm_ChangePass = new Frm_ChangePass("Login");
                                Frm_ChangePass.Show();                                
                                this.Hide();
                                return;                            
                            }

                            //Password Expired, jika sudah 60 hari maka harus ganti password                                                    
                            //MessageBox.Show(ControlMgr.GetDateTime().ToString());
                            //Password Expired End
                        
                            if(!CekUserAktif())
                            {
                                MessageBox.Show("User Id belum Aktif..");
                                return;
                            }
                        }
                    }
                    else 
                    {
                        MessageBox.Show("User Id atau Password salah..");
                        return;
                    }
                }

                if (!CekUserGroup())
                {
                    MessageBox.Show("User Id belum ditentukan Groupnya..");
                    return;                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                drtblPass.Close();
                ConnMaster.Close();
            }

            if (ChkRemember.Checked)
            {
                ISBS_New.Properties.Settings.Default.UserID = txtUserID.Text.Trim();
                ISBS_New.Properties.Settings.Default.Password = txtPassword.Text.Trim();
                ISBS_New.Properties.Settings.Default.Type = "Remember";
            }
            else
            {
                ISBS_New.Properties.Settings.Default.UserID = "";
                ISBS_New.Properties.Settings.Default.Password = "";
                ISBS_New.Properties.Settings.Default.Type = "";
            }
            ISBS_New.Properties.Settings.Default.Save();

            Form MainMenu = new MainMenu();
            MainMenu.Show();
            this.Hide();
        }

        private void txtUserID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                mBtnLogin.PerformClick(); 
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                mBtnLogin.PerformClick();
            }
        }
    }
}
