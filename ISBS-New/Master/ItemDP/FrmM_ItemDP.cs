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

namespace ISBS_New.Master.ItemDP
{
    public partial class FrmM_ItemDP : MetroFramework.Forms.MetroForm
    {
        string Mode = "";
        int ItemID = 0;
        SqlConnection ConnMaster;
        SqlDataReader Dr;
        string strSql = "";

        int RowAffected;

        Boolean vView, vNew, vEdit, vDelete;

        public FrmM_ItemDP(string _Mode, int _Kode)
        {
            InitializeComponent();
            Mode = _Mode;
            ItemID = _Kode;
        }

        private void FrmM_ItemDP_Load(object sender, EventArgs e)
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
                strSql = "SELECT * FROM ItemDP WHERE ID=@ItemID";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            txttblItemDP_Ket.Text = Convert.IsDBNull(Dr["Description"]) ? "" : (string)Dr["Description"];                          
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data ID " + ItemID + " tidak ditemukan...");
                        return;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }
        }

        private void txt_ReadOnly(bool vBol)
        {
            txttblItemDP_Ket.ReadOnly = vBol;
        }

        private void ModeView()
        {
            txt_ReadOnly(true);
            Btn_EditCancelSaveDel(true, false, false, true);
        }
     
        private void ModeNew()
        {
            this.ActiveControl = txttblItemDP_Ket;
            txt_ReadOnly(false);
            Btn_EditCancelSaveDel(false, true, true, false);
        }

        private void ModeEdit()
        {
            txt_ReadOnly(false);         
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
            if (Mode == "Edit" && vBol && !CekItemID())
            {
                MessageBox.Show("ID " + ItemID + " tidak ditemukan..");
                vBol = false;
            }

            if (vBol && txttblItemDP_Ket.Text.Trim() == "")
            {
                MessageBox.Show("Description harus diisi..");
                this.ActiveControl = txttblItemDP_Ket;
                vBol = false;
            }

            if (vBol && CekItemDesc())
            {
                MessageBox.Show("Description sudah ada..");
                this.ActiveControl = txttblItemDP_Ket;
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
                            strSql = "INSERT INTO ItemDP(Description,CreatedBy) ";
                            strSql += "VALUES(@Description,@UInput)";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@Description", txttblItemDP_Ket.Text.Trim());
                                Cmd.Parameters.AddWithValue("@UInput", ControlMgr.UserId);
                                Cmd.ExecuteNonQuery();
                            }

                            strSql = "SELECT TOP 1 ID FROM ItemDP ORDER BY ID DESC";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Dr = Cmd.ExecuteReader();
                                if (Dr.HasRows)
                                {
                                    while (Dr.Read())
                                    {
                                        ItemID = Convert.ToInt32(Dr["ID"]);
                                    }
                                }
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
                            strSql = "UPDATE ItemDP SET ";
                            strSql += "Description=@Description,";
                            strSql += "UpdatedBy=@UEdit,";
                            strSql += "UpdatedDate=GETDATE() ";
                            strSql += "WHERE ID=@ItemID";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@Description", txttblItemDP_Ket.Text.Trim());
                                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                                Cmd.Parameters.AddWithValue("@UEdit", ControlMgr.UserId);
                                RowAffected = Cmd.ExecuteNonQuery();
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

        private Boolean CekItemID()
        {
            Boolean vBol = true;
            
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM ItemDP WHERE ID=@ItemID";                
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
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
                Dr.Close();
                ConnMaster.Close();
            }

            return vBol;
        }

        private Boolean CekItemDesc()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM ItemDP WHERE Description = @Description AND ID<>@ItemID";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Description", txttblItemDP_Ket.Text.Trim());
                    Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
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
                Dr.Close();
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

            if (!CekItemID())
            {
                MessageBox.Show("ID " + ItemID + " tidak ditemukan..");
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus ID " + ItemID + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "DELETE FROM ItemDP WHERE ID=@ItemID";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@ItemID", ItemID);
                                RowAffected = Cmd.ExecuteNonQuery();
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
                    MessageBox.Show("ID " + ItemID + ", berhasil dihapus..");
                }
                this.Close();
            }
        }

        private void FrmM_ItemDP_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            } 
        }
    }
}
