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

namespace ISBS_New.Master.Golongan
{
    public partial class FrmM_GolPrsh : MetroFramework.Forms.MetroForm
    {
        string Mode = "";

        SqlConnection ConnMaster;
        SqlDataReader drtblGol_Prsh;
        string strSql = "";
        string vOldtxttblGol_Prsh_Gol_Prsh = "";
        int RowAffected;

        Boolean vView, vNew, vEdit, vDelete;


        public FrmM_GolPrsh(string _Mode, string _Kode)
        {
            InitializeComponent();
            Mode = _Mode;
            txttblGol_Prsh_Gol_Prsh.Text = _Kode;
        }

        private void FrmM_GolPrsh_Load(object sender, EventArgs e)
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

                vOldtxttblGol_Prsh_Gol_Prsh = txttblGol_Prsh_Gol_Prsh.Text;
            }
        }

        private void FillForm()
        {
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh=@Gol_Prsh";
                using (SqlCommand cmdtblGol_Prsh = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txttblGol_Prsh_Gol_Prsh.Text);
                    drtblGol_Prsh = cmdtblGol_Prsh.ExecuteReader();
                    if (drtblGol_Prsh.HasRows)
                    {
                        while (drtblGol_Prsh.Read())
                        {
                            txttblGol_Prsh_Gol_Prsh.Text = Convert.IsDBNull(drtblGol_Prsh["tblGol_Prsh_Gol_Prsh"]) ? "" : (string)drtblGol_Prsh["tblGol_Prsh_Gol_Prsh"];
                         }
                    }
                    else
                    {
                        MessageBox.Show("Data Golongan Perusahaan " + txttblGol_Prsh_Gol_Prsh.Text + " tidak ditemukan...");
                        this.BeginInvoke(new MethodInvoker(this.Close));
                        return;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                drtblGol_Prsh.Close();
                ConnMaster.Close();
            }
        }

        private void txt_ReadOnly(bool vbol)
        {
            txttblGol_Prsh_Gol_Prsh.ReadOnly = vbol;
        }

        private void ModeView()
        {
            txt_ReadOnly(true);
            Btn_EditCancelSaveDel(true, false, false, true);
        }

        private void ModeNew()
        {
            this.ActiveControl = txttblGol_Prsh_Gol_Prsh;

            txt_ReadOnly(false);
            Btn_EditCancelSaveDel(false, true, true, false);
        }

        private void ModeEdit()
        {
            vOldtxttblGol_Prsh_Gol_Prsh = txttblGol_Prsh_Gol_Prsh.Text;
            txt_ReadOnly(false);
            if (Used())
            {
                MessageBox.Show("Golongan " + txttblGol_Prsh_Gol_Prsh.Text + " sudah pernah digunakan..");
                txttblGol_Prsh_Gol_Prsh.ReadOnly = true; 
            }
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
            if (Mode == "Edit" && vBol && !CekGolongan())
            {
                MessageBox.Show("Golongan " + vOldtxttblGol_Prsh_Gol_Prsh + " tidak ditemukan..");
                vBol = false;
            }
            if (vBol && txttblGol_Prsh_Gol_Prsh.Text.Trim() == "")
            {
                MessageBox.Show("Golongan harus diisi..");
                vBol = false;
            }

            if (Mode=="New" && vBol && CekGolongan())
            {
                MessageBox.Show("Golongan sudah ada..");
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
                            strSql = "INSERT INTO tblGol_Prsh(";
                            strSql += "tblGol_Prsh_Gol_Prsh,";
                            strSql += "tblGol_Prsh_UInput) ";
                            strSql += "VALUES(@Gol_Prsh,";
                            strSql += "@UInput)";
                            using (SqlCommand cmdtblGol_Prsh = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txttblGol_Prsh_Gol_Prsh.Text);
                                cmdtblGol_Prsh.Parameters.AddWithValue("@UInput", "ITDIVISI");
                                cmdtblGol_Prsh.ExecuteNonQuery();
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
                            strSql = "UPDATE tblGol_Prsh SET ";
                            strSql += "tblGol_Prsh_Gol_Prsh=@Gol_Prsh,";
                            strSql += "tblGol_Prsh_UEdit=@UEdit,";
                            strSql += "tblGol_Prsh_UDate_Edit=GETDATE() ";
                            strSql += "WHERE tblGol_Prsh_Gol_Prsh=@vOldGol_Prsh";
                            using (SqlCommand cmdtblGol_Prsh = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txttblGol_Prsh_Gol_Prsh.Text);
                                cmdtblGol_Prsh.Parameters.AddWithValue("@UEdit", "ITDIVISI");
                                cmdtblGol_Prsh.Parameters.AddWithValue("@vOldGol_Prsh", vOldtxttblGol_Prsh_Gol_Prsh);
                                RowAffected = cmdtblGol_Prsh.ExecuteNonQuery();
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

        private Boolean CekGolongan()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh=@Gol_Prsh";
                using (SqlCommand cmdtblGol_Prsh = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", vOldtxttblGol_Prsh_Gol_Prsh);
                    drtblGol_Prsh = cmdtblGol_Prsh.ExecuteReader();
                    if (drtblGol_Prsh.HasRows)
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
                drtblGol_Prsh.Close();
                ConnMaster.Close();
            }

            return vBol;
        }

        private Boolean Used()
        {
            Boolean vBol = true;
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblCustomer WHERE tblCustomer_Gol_Prsh=@Gol_Prsh";
                using (SqlCommand cmdtblGol_Prsh = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblGol_Prsh.Parameters.AddWithValue("@Gol_Prsh",txttblGol_Prsh_Gol_Prsh.Text);
                    drtblGol_Prsh = cmdtblGol_Prsh.ExecuteReader();
                    if (drtblGol_Prsh.HasRows)
                    {
                        vBol = true;
                    }
                    else { vBol = false; }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                drtblGol_Prsh.Close();
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

            if (!CekGolongan())
            {
                MessageBox.Show("Golongan " + txttblGol_Prsh_Gol_Prsh.Text + " tidak ditemukan..");
                return;
            }
            if (Used())
            {
                MessageBox.Show("Golongan " + txttblGol_Prsh_Gol_Prsh.Text + " sudah pernah digunakan..");
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Golongan " + txttblGol_Prsh_Gol_Prsh.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "DELETE FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh=@Gol_Prsh";
                            using (SqlCommand cmdtblGol_Prsh_Gol_Prsh = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblGol_Prsh_Gol_Prsh.Parameters.AddWithValue("@Gol_Prsh",txttblGol_Prsh_Gol_Prsh.Text);                               
                                RowAffected=cmdtblGol_Prsh_Gol_Prsh.ExecuteNonQuery();
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
                    MessageBox.Show("Golongan " + txttblGol_Prsh_Gol_Prsh.Text + ", berhasil dihapus..");                    
                }
                this.Close();
            }
        }

        private void FrmM_GolPrsh_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            } 
        }
    }
}
