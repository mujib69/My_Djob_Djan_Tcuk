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

namespace ISBS_New.Master.BankGroup
{
    public partial class FrmM_BankGroup : MetroFramework.Forms.MetroForm
    {
        string Mode = "";
        SqlConnection ConnMaster;
        SqlDataReader drtblBank_Group;
        string strSql = "";
        int RowAffected;

        Boolean vView, vNew, vEdit, vDelete;

        public FrmM_BankGroup(string _Mode, string _Kode)
        {
            InitializeComponent();
            Mode = _Mode;
            txttblBank_Group_Group.Text = _Kode;
        }

        private void FrmM_BankGroup_Load(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator")
            {
                ControlMgr.CheckAccessRight(ref vView, ref vNew, ref vEdit, ref vDelete, this.Name);
                if (vView == false)
                {
                    ControlMgr.DisplayMsgAccessRight("View", this.Text);
                    return;
                }
            }

            if (Mode == "View")
            {
                ModeView();
            }
            else if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                FillForm();
                ModeEdit();
            }
        }

        private void FillForm()
        {
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblBank_Group WHERE tblBank_Group_Group=@Group";
                using (SqlCommand cmdtblBank_Group = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblBank_Group.Parameters.AddWithValue("@Group", txttblBank_Group_Group.Text);
                    drtblBank_Group = cmdtblBank_Group.ExecuteReader();
                    if (drtblBank_Group.HasRows)
                    {
                        while (drtblBank_Group.Read())
                        {
                            txttblBank_Group_Group.Text = Convert.IsDBNull(drtblBank_Group["tblBank_Group_Group"]) ? "" : (string)drtblBank_Group["tblBank_Group_Group"];
                            txttblBank_Group_Nama.Text = Convert.IsDBNull(drtblBank_Group["tblBank_Group_Nama"]) ? "" : (string)drtblBank_Group["tblBank_Group_Nama"];
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data Group Bank " + txttblBank_Group_Group.Text + " tidak ditemukan...");
                        return;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                drtblBank_Group.Close();
                ConnMaster.Close();
            }
        }

        private void txt_ReadOnly(bool vBol)
        {
            txttblBank_Group_Group.ReadOnly = vBol;
            txttblBank_Group_Nama.ReadOnly = vBol;
        }

        private void ModeView()
        {
            txt_ReadOnly(true);
            Btn_EditCancelSaveDel(true, false, false, true);
        }

        private void ModeNew()
        {
            this.ActiveControl = txttblBank_Group_Group;
            txt_ReadOnly(false);
            Btn_EditCancelSaveDel(false, true, true, false);
        }

        private void ModeEdit()
        {
            txt_ReadOnly(false);
            txttblBank_Group_Group.ReadOnly=true;
            Btn_EditCancelSaveDel(false, true, true, false);
        }

        private void Btn_EditCancelSaveDel(bool vEdit, bool vCancel, bool vSave, bool vDel)
        {
            BtnEdit.Enabled = vEdit;
            BtnCancel.Enabled = vCancel;
            BtnSave.Enabled = vSave;
            BtnDelete.Enabled = vDel;
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (Mode == "Edit" && vBol && !CekBank_Group())
            {
                MessageBox.Show("Group Bank " + txttblBank_Group_Group + " tidak ditemukan..");
                vBol = false;
            }


            if (vBol && txttblBank_Group_Group.Text.Trim() == "")
            {
                MessageBox.Show("Group Bank harus diisi..");
                this.ActiveControl = txttblBank_Group_Group;
                vBol = false;
            }

            if (vBol && txttblBank_Group_Nama.Text.Trim() == "")
            {
                MessageBox.Show("Nama Bank harus diisi..");
                this.ActiveControl = txttblBank_Group_Nama;
                vBol = false;
            }           

            if (Mode == "New" && vBol && CekNamaBank())
            {
                MessageBox.Show("Nama Bank sudah ada..");
                this.ActiveControl = txttblBank_Group_Nama;
                vBol = false;
            }           

            return vBol;
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                return;
            }

            if (Mode == "New")
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "INSERT INTO tblBank_Group(";
                            strSql += "tblBank_Group_Group,tblBank_Group_Nama,tblBank_Group_UInput) ";
                            strSql += "VALUES(@Group,@Nama,@UInput)";
                            using (SqlCommand cmdtblBank = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblBank.Parameters.AddWithValue("@Group", txttblBank_Group_Group.Text.Trim());
                                cmdtblBank.Parameters.AddWithValue("@Nama", txttblBank_Group_Nama.Text.Trim());
                                cmdtblBank.Parameters.AddWithValue("@UInput", "ITDIVISI");
                                cmdtblBank.ExecuteNonQuery();
                            }
                        }
                        Scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
                finally
                {
                    ConnMaster.Close();
                }

                Mode = "View";
                MessageBox.Show("Insert Success");
            }
            else
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "UPDATE tblBank_Group SET ";
                            strSql += "tblBank_Group_Group=@Group,";
                            strSql += "tblBank_Group_Nama=@Nama,";
                            strSql += "tblBank_Group_UEdit=@UEdit,";
                            strSql += "tblBank_Group_UDate_Edit=GETDATE() ";
                            strSql += "WHERE tblBank_Group_Group=@Group";
                            using (SqlCommand cmdtblBank = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblBank.Parameters.AddWithValue("@Group", txttblBank_Group_Group.Text.Trim());
                                cmdtblBank.Parameters.AddWithValue("@Nama", txttblBank_Group_Nama.Text.Trim());
                                cmdtblBank.Parameters.AddWithValue("@UEdit", "ITDIVISI");
                                RowAffected = cmdtblBank.ExecuteNonQuery();
                            }
                        }
                        Scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
                finally
                { ConnMaster.Close(); }

                if (RowAffected == 0)
                {
                    MessageBox.Show("Record Data baru saja diupdate oleh orang lain, silihkan ulangi kembali");
                    this.Close();
                }
                else
                {
                    Mode = "View";
                    MessageBox.Show("Update Success");
                }
            }

            ModeView(); 
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator")
            {
                ControlMgr.CheckAccessRight(ref vView, ref vNew, ref vEdit, ref vDelete, this.Name);
                if (vEdit == false)
                {
                    ControlMgr.DisplayMsgAccessRight("Edit", this.Text);
                    return;
                }
            }

            ModeEdit();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean CekBank_Group()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblBank_Group WHERE tblBank_Group_Group=@Group";
                using (SqlCommand cmdtblBank_Group = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblBank_Group.Parameters.AddWithValue("@Group", txttblBank_Group_Group.Text);
                    drtblBank_Group = cmdtblBank_Group.ExecuteReader();
                    if (drtblBank_Group.HasRows)
                    {
                        vBol = true;
                    }
                    else
                    {
                        vBol = false;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                drtblBank_Group.Close();
                ConnMaster.Close();
            }

            return vBol;
        }

        private Boolean CekNamaBank()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblBank_Group WHERE tblBank_Group_Group=@Group OR tblBank_Group_Nama=@Nama";
                using (SqlCommand cmdtblBank_Group = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblBank_Group.Parameters.AddWithValue("@Group", txttblBank_Group_Group.Text.Trim());
                    cmdtblBank_Group.Parameters.AddWithValue("@Nama", txttblBank_Group_Nama.Text.Trim());
                    drtblBank_Group = cmdtblBank_Group.ExecuteReader();
                    if (drtblBank_Group.HasRows)
                    {
                        vBol = true;
                    }
                    else
                    {
                        vBol = false;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                drtblBank_Group.Close();
                ConnMaster.Close();
            }

            return vBol;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator")
            {
                ControlMgr.CheckAccessRight(ref vView, ref vNew, ref vEdit, ref vDelete, this.Name);
                if (vDelete == false)
                {
                    ControlMgr.DisplayMsgAccessRight("Delete", this.Text);
                    return;
                }
            }

            if (!CekBank_Group())
            {
                MessageBox.Show("Group Bank " + txttblBank_Group_Group.Text + " tidak ditemukan..");
                return;
            }
        
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Group Bank " + txttblBank_Group_Group.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "DELETE FROM tblBank_Group WHERE tblBank_Group_Group=@Group";
                            using (SqlCommand cmdtblBank_Group = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblBank_Group.Parameters.AddWithValue("@Group", txttblBank_Group_Group.Text);
                                RowAffected = cmdtblBank_Group.ExecuteNonQuery();
                            }
                        }
                        Scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
                finally
                {
                    ConnMaster.Close();
                }

                if (RowAffected == 0)
                {
                    MessageBox.Show("Record Data baru saja diupdate oleh orang lain, silihkan ulangi kembali");
                }
                else
                {
                    MessageBox.Show("Group Bank " + txttblBank_Group_Group.Text + ", berhasil dihapus..");
                }
                this.Close();
            }
        }

        private void FrmM_BankGroup_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            } 
        }
    }
}
