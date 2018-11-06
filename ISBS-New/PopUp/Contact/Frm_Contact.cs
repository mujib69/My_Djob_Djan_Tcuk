using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;

namespace ISBS_New.PopUp.Contact
{
    public partial class Frm_Contact : MetroFramework.Forms.MetroForm
    {
        Boolean BolNew;
        decimal RecId;
        string tblName;
        string Kode;
        SqlDataReader Dr;
        SqlConnection ConMaster;
        string strSql;

        public Frm_Contact(Boolean _BolNew, decimal _RecId, string _Kode, string _tblName)
        {
            InitializeComponent();
            BolNew = _BolNew;
            RecId = _RecId;
            tblName = _tblName;
            Kode = _Kode;
        }

        private void Frm_Contact_Load(object sender, EventArgs e)
        {
            if (!BolNew)
            {
                if (txttblContact_Email.Text.Trim() != "")
                {
                    btntxttblContact_Email.Enabled = true;
                }

                try
                {
                    strSql = "SELECT * ";
                    strSql += "FROM tblContact ";
                    strSql += "WHERE tblContact_tblName ='" + tblName + "' ";
                    strSql += "AND tblContact_Kode='" + Kode + "' ";
                    strSql += "AND tblContact_RecId='" + RecId + "' ";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                    {
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                txttblContact_Nama.Text = Convert.IsDBNull(Dr["tblContact_Nama"]) ? "" : (string)Dr["tblContact_Nama"];
                                txttblContact_Jabatan.Text = Convert.IsDBNull(Dr["tblContact_Jabatan"]) ? "" : (string)Dr["tblContact_Jabatan"];
                                txttblContact_Phone1.Text = Convert.IsDBNull(Dr["tblContact_Phone1"]) ? "" : (string)Dr["tblContact_Phone1"];
                                txttblContact_Phone2.Text = Convert.IsDBNull(Dr["tblContact_Phone2"]) ? "" : (string)Dr["tblContact_Phone2"];
                                txttblContact_Ext1.Text = Convert.IsDBNull(Dr["tblContact_Ext1"]) ? "" : (string)Dr["tblContact_Ext1"];
                                txttblContact_Ext2.Text = Convert.IsDBNull(Dr["tblContact_Ext2"]) ? "" : (string)Dr["tblContact_Ext1"];
                                txttblContact_HP.Text = Convert.IsDBNull(Dr["tblContact_HP"]) ? "" : (string)Dr["tblContact_HP"];
                                txttblContact_Email.Text = Convert.IsDBNull(Dr["tblContact_Email"]) ? "" : (string)Dr["tblContact_Email"];
                            }
                        }
                        else
                        {
                            MessageBox.Show("Data Contact tidak ditemukan...");
                            this.BeginInvoke(new MethodInvoker(this.Close));
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
                finally
                {
                    Dr.Close();
                    ConMaster.Close();
                }
            }
            else
            {
                btntxttblContact_Email.Enabled = false;
            }
            this.ActiveControl = txttblContact_Nama;
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;

            if (txttblContact_Nama.Text.Trim() == "")
            {
                MessageBox.Show("Nama Contact harus diisi..");
                vBol = false;
                this.ActiveControl = txttblContact_Nama;
            }

            return vBol;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                return;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ConMaster = ConnectionString.GetConnection();
                    using (ConMaster)
                    {
                        string strSql;
                        if (BolNew)
                        {
                            strSql = "INSERT INTO tblContact(tblContact_tblName,tblContact_Kode,tblContact_Nama,tblContact_Jabatan,tblContact_Phone1,tblContact_Phone2,tblContact_Ext1,tblContact_Ext2,tblContact_HP,tblContact_Email,tblContact_UInput ";
                            strSql += ") VALUES(";
                            strSql += "'" + tblName + "','" + Kode + "',@Nama,@Jabatan,@Phone1,@Phone2,@Ext1,@Ext2,@HP,@Email,@Uinput)";
                            using (SqlCommand cmdtblContact = new SqlCommand(strSql, ConMaster))
                            {
                                cmdtblContact.Parameters.AddWithValue("@Nama", txttblContact_Nama.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Jabatan", txttblContact_Jabatan.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Phone1", txttblContact_Phone1.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Phone2", txttblContact_Phone2.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Ext1", txttblContact_Ext1.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Ext2", txttblContact_Ext2.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@HP", txttblContact_HP.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Email", txttblContact_Email.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Uinput", "ITDIVISI");
                                cmdtblContact.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            strSql = "UPDATE tblContact SET tblContact_Nama=@Nama, ";
                            strSql += "tblContact_Jabatan=@Jabatan,tblContact_Phone1=@Phone1,tblContact_Phone2=@Phone2,tblContact_Ext1=@Ext1,tblContact_Ext2=@Ext2, ";
                            strSql += "tblContact_HP=@HP,tblContact_Email=@Email,";
                            strSql += "tblContact_UEdit=@UEdit,tblContact_UDate_Edit=GETDATE() ";
                            strSql += "WHERE tblContact_tblName='" + tblName + "' ";
                            strSql += "AND tblContact_Kode='" + Kode + "' ";
                            strSql += "AND tblContact_RecId='" + RecId + "'";
                            using (SqlCommand cmdtblContact = new SqlCommand(strSql, ConMaster))
                            {
                                cmdtblContact.Parameters.AddWithValue("@Nama", txttblContact_Nama.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Jabatan", txttblContact_Jabatan.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Phone1", txttblContact_Phone1.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Phone2", txttblContact_Phone2.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Ext1", txttblContact_Ext1.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Ext2", txttblContact_Ext2.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@HP", txttblContact_HP.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@Email", txttblContact_Email.Text.Trim());
                                cmdtblContact.Parameters.AddWithValue("@UEdit", "ITDIVISI");
                                cmdtblContact.ExecuteNonQuery();
                            }
                        }
                    }
                    scope.Complete();
                    if (BolNew)
                    {
                        MessageBox.Show("Berhasil Insert data..");
                    }
                    else
                    {
                        MessageBox.Show("Berhasil Update data..");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                ConMaster.Close();
            }
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
