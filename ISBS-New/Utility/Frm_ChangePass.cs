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
    public partial class Frm_ChangePass : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        SqlDataReader drtblPass;
        string strSql;
        string Mode="";

        public Frm_ChangePass(string _Mode)
        {
            InitializeComponent();
            Mode = _Mode;
        }

        private void Frm_ChangePass_Load(object sender, EventArgs e)
        {

        }

        private Boolean CekOldPass()
        {
            Boolean vBol=true;
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblPass WHERE tblPass_UserId=@UserId";
                using (SqlCommand cmdtblPass = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblPass.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                    drtblPass = cmdtblPass.ExecuteReader();
                    if (drtblPass.HasRows)
                    {
                        while (drtblPass.Read())
                        {
                            string OldPass = Convert.IsDBNull(drtblPass["tblPass_PasswordUser"]) ? "" : (string)drtblPass["tblPass_PasswordUser"];
                            if (OldPass != txtOldPassword.Text.Trim())
                            {
                                vBol = false;
                            }
                            else
                            {
                                vBol = true;
                            }
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
                drtblPass.Close();
                ConnMaster.Close();
            }

            return vBol;
        }

        private void mBtnSave_Click(object sender, EventArgs e)
        {
            if (txtOldPassword.Text.Trim() == "" || Convert.IsDBNull(txtOldPassword.Text))
            {
                MessageBox.Show("Password lama tidak boleh kosong..");
                txtOldPassword.Focus();
                return;
            }

            if(!CekOldPass())
            {
                MessageBox.Show("Password lama salah..");
                txtOldPassword.Focus();
                return;            
            }

            if (txtNewPassword.Text.Trim() == "" || Convert.IsDBNull(txtNewPassword.Text))
            {
                MessageBox.Show("Password baru tidak boleh kosong..");
                txtNewPassword.Focus();
                return;
            }
         
            if (txtNewPassword.Text.Trim().Length < 6)
            {
                MessageBox.Show("Password harus minimal 6 character");
                return;
            }

            if (!txtNewPassword.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Password harus mengandung minimal 1 angka");
                return;
            }

            if (txtNewPassword2.Text.Trim() == "" || Convert.IsDBNull(txtNewPassword2.Text))
            {
                MessageBox.Show("Konfirmasi Password baru tidak boleh kosong..");
                txtNewPassword2.Focus();
                return;
            }

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "UPDATE tblPass SET tblPass_PasswordUser=@NewPassword,tblPass_PwdDateChange=GETDATE() ";
                strSql += "WHERE tblPass_UserId=@UserId";
                using (SqlCommand cmdtblPass = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblPass.Parameters.AddWithValue("@NewPassword", txtNewPassword.Text.Trim());
                    cmdtblPass.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                    cmdtblPass.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            { ConnMaster.Close(); }


            MessageBox.Show("Password Telah Diganti..");
            this.Close();

            if (Mode == "Login")
            {
                Form Frm_Login = new Frm_Login();
                Frm_Login.Show();
            }
        }
    }
}
