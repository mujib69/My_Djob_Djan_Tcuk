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

namespace ISBS_New.Master.Kota
{
    public partial class FrmM_Kota : MetroFramework.Forms.MetroForm
    {
        string Mode = "";
        int RecId = 0;

        SqlConnection ConnMaster;
        SqlDataReader drtblKota;
        string strSql = "";

        int RowAffected;
        Boolean vView, vNew, vEdit, vDelete;

        public FrmM_Kota(string _Mode, int _RecId)
        {
            InitializeComponent();
            Mode = _Mode;
            RecId = _RecId;
        }

        private void FrmM_Kota_Load(object sender, EventArgs e)
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

            FillcbxtblKota_Provinsi();
            cbxtblKota_Provinsi.SelectedItem = "";
            cbxtblKota_Kota_Kabupaten.SelectedItem = "";

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

        private void FillcbxtblKota_Provinsi()
        {
            cbxtblKota_Provinsi.Items.Clear();
            cbxtblKota_Provinsi.Items.Add("");
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT DISTINCT Provinsi FROM VIEW_tblKota";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    drtblKota = Cmd.ExecuteReader();
                    if (drtblKota.HasRows)
                    {
                        while (drtblKota.Read())
                        {
                            cbxtblKota_Provinsi.Items.Add(Convert.IsDBNull(drtblKota["Provinsi"]) ? "" : (string)drtblKota["Provinsi"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                drtblKota.Close();
                ConnMaster.Close();
            }
        }

        private void FillForm()
        {
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblKota WHERE tblKota_RecId=@RecId";
                using (SqlCommand cmdtblKota = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblKota.Parameters.AddWithValue("@RecId", RecId);
                    drtblKota = cmdtblKota.ExecuteReader();
                    if (drtblKota.HasRows)
                    {
                        while (drtblKota.Read())
                        {
                            cbxtblKota_Provinsi.SelectedItem = Convert.IsDBNull(drtblKota["tblKota_Provinsi"]) ? "" : (string)drtblKota["tblKota_Provinsi"];
                            cbxtblKota_Kota_Kabupaten.SelectedItem = Convert.IsDBNull(drtblKota["tblKota_Kota_Kabupaten"]) ? "" : (string)drtblKota["tblKota_Kota_Kabupaten"];
                            txttblKota_Nama_Kota_Kabupaten.Text = Convert.IsDBNull(drtblKota["tblKota_Nama_Kota_Kabupaten"]) ? "" : (string)drtblKota["tblKota_Nama_Kota_Kabupaten"];
                            txttblKota_Kecamatan.Text = Convert.IsDBNull(drtblKota["tblKota_Kecamatan"]) ? "" : (string)drtblKota["tblKota_Kecamatan"];
                            txttblKota_Desa_Kelurahan.Text = Convert.IsDBNull(drtblKota["tblKota_Desa_Kelurahan"]) ? "" : (string)drtblKota["tblKota_Desa_Kelurahan"];
                            txttblKota_Kode_Pos.Text = Convert.IsDBNull(drtblKota["tblKota_Kode_Pos"]) ? "" : (string)drtblKota["tblKota_Kode_Pos"];
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data Nama Kota tidak ditemukan...");
                        return;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                drtblKota.Close();
                ConnMaster.Close();
            }
        }

        private void txt_ReadOnly(bool vBol)
        {
            txttblKota_Nama_Kota_Kabupaten.ReadOnly = vBol;
            txttblKota_Kecamatan.ReadOnly = vBol;
            txttblKota_Desa_Kelurahan.ReadOnly = vBol;
            txttblKota_Kode_Pos.ReadOnly = vBol;
        }

        private void cbx_Enable(bool vBol)
        {
            cbxtblKota_Provinsi.Enabled = vBol;
            cbxtblKota_Kota_Kabupaten.Enabled = vBol;
        }

        private void ModeView()
        {
            txt_ReadOnly(true);
            cbx_Enable(false);
            Btn_EditCancelSaveDel(true, false, false, true);
        }

        private void ModeNew()
        {
            this.ActiveControl = cbxtblKota_Provinsi;

            txt_ReadOnly(false);
            cbx_Enable(true);
            Btn_EditCancelSaveDel(false, true, true, false);
        }

        private void ModeEdit()
        {
            txt_ReadOnly(false);
            cbx_Enable(true);

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
            if (Mode == "Edit" && vBol && !CekKota())
            {
                MessageBox.Show("Nama Kota " + txttblKota_Nama_Kota_Kabupaten + " tidak ditemukan..");
                vBol = false;
            }

            if (vBol && cbxtblKota_Provinsi.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Provinsi harus dipilih..");
                this.ActiveControl = cbxtblKota_Provinsi;
                vBol = false;
            }

            if (vBol && cbxtblKota_Kota_Kabupaten.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Kota / Kabupaten harus dipilih..");
                this.ActiveControl = cbxtblKota_Kota_Kabupaten;
                vBol = false;
            }

            if (vBol && txttblKota_Nama_Kota_Kabupaten.Text.Trim() == "")
            {
                MessageBox.Show("Nama Kota harus diisi..");
                this.ActiveControl = txttblKota_Nama_Kota_Kabupaten;
                vBol = false;
            }

            if (vBol && txttblKota_Kecamatan.Text.Trim() == "")
            {
                MessageBox.Show("Nama Kecamatan harus diisi..");
                this.ActiveControl = txttblKota_Kecamatan;
                vBol = false;
            }

            if (vBol && txttblKota_Desa_Kelurahan.Text.Trim() == "")
            {
                MessageBox.Show("Nama Desa / Kelurahan harus diisi..");
                this.ActiveControl = txttblKota_Desa_Kelurahan;
                vBol = false;
            }

            if (vBol && txttblKota_Kode_Pos.Text.Trim() == "")
            {
                MessageBox.Show("Kode Pos harus diisi..");
                this.ActiveControl = txttblKota_Kode_Pos;
                vBol = false;
            }

            if (vBol && CekNamaKota())
            {
                MessageBox.Show("Nama Kota sudah ada..");
                this.ActiveControl = txttblKota_Nama_Kota_Kabupaten;
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

            try
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ConnMaster = ConnectionString.GetConnection();
                    using (ConnMaster)
                    {
                        if (Mode == "New")
                        {
                            strSql = "INSERT INTO tblKota(";
                            strSql += "tblKota_Provinsi,tblKota_Kota_Kabupaten,tblKota_Nama_Kota_Kabupaten,tblKota_Kecamatan,tblKota_Desa_Kelurahan,tblKota_Kode_Pos,";
                            strSql += "tblKota_UInput) ";
                            strSql += "VALUES(";
                            strSql += "@Provinsi,@Kota_Kabupaten,@Nama_Kota_Kabupaten,@Kecamatan,@Desa_Kelurahan,@Kode_Pos,";
                            strSql += "@UInput)";
                            using (SqlCommand cmdtblKota = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblKota.Parameters.AddWithValue("@Provinsi", cbxtblKota_Provinsi.SelectedItem.ToString());
                                cmdtblKota.Parameters.AddWithValue("@Kota_Kabupaten", cbxtblKota_Kota_Kabupaten.SelectedItem.ToString());
                                cmdtblKota.Parameters.AddWithValue("@Nama_Kota_Kabupaten", txttblKota_Nama_Kota_Kabupaten.Text);
                                cmdtblKota.Parameters.AddWithValue("@Kecamatan", txttblKota_Kecamatan.Text);
                                cmdtblKota.Parameters.AddWithValue("@Desa_Kelurahan", txttblKota_Desa_Kelurahan.Text);
                                cmdtblKota.Parameters.AddWithValue("@Kode_Pos", txttblKota_Kode_Pos.Text);
                                cmdtblKota.Parameters.AddWithValue("@UInput", "ITDIVISI");
                                cmdtblKota.ExecuteNonQuery();
                            }

                            strSql = "SELECT TOP 1 tblKota_RecId FROM tblKota WHERE ";
                            strSql += "tblKota_Provinsi=@Provinsi ";
                            strSql += "AND tblKota_Kota_Kabupaten=@Kota_Kabupaten ";
                            strSql += "AND tblKota_Nama_Kota_Kabupaten=@Nama_Kota_Kabupaten ";
                            strSql += "AND tblKota_Kecamatan=@Kecamatan ";
                            strSql += "AND tblKota_Desa_Kelurahan=@Desa_Kelurahan ";
                            strSql += "AND tblKota_Kode_Pos=@Kode_Pos";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@Provinsi", cbxtblKota_Provinsi.SelectedItem.ToString());
                                Cmd.Parameters.AddWithValue("@Kota_Kabupaten", cbxtblKota_Kota_Kabupaten.SelectedItem.ToString());
                                Cmd.Parameters.AddWithValue("@Nama_Kota_Kabupaten", txttblKota_Nama_Kota_Kabupaten.Text);
                                Cmd.Parameters.AddWithValue("@Kecamatan", txttblKota_Kecamatan.Text);
                                Cmd.Parameters.AddWithValue("@Desa_Kelurahan", txttblKota_Desa_Kelurahan.Text);
                                Cmd.Parameters.AddWithValue("@Kode_Pos", txttblKota_Kode_Pos.Text);
                                Cmd.Parameters.AddWithValue("@UInput", "ITDIVISI");

                                drtblKota = Cmd.ExecuteReader();
                                while (drtblKota.Read())
                                {
                                    RecId = Convert.ToInt32(drtblKota["tblKota_RecId"]);
                                }
                            }
                        }
                        else
                        {
                            strSql = "UPDATE tblKota SET ";
                            strSql += "tblKota_Provinsi=@Provinsi,";
                            strSql += "tblKota_Kota_Kabupaten=@Kota_Kabupaten,";
                            strSql += "tblKota_Nama_Kota_Kabupaten=@Nama_Kota_Kabupaten,";
                            strSql += "tblKota_Kecamatan=@Kecamatan,";
                            strSql += "tblKota_Desa_Kelurahan=@Desa_Kelurahan,";
                            strSql += "tblKota_Kode_Pos=@Kode_Pos,";
                            strSql += "tblKota_UEdit=@UEdit,";
                            strSql += "tblKota_UDate_Edit=GETDATE() ";
                            strSql += "WHERE tblKota_RecId=@RecId";
                            using (SqlCommand cmdtblKota = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblKota.Parameters.AddWithValue("@RecId", RecId);
                                cmdtblKota.Parameters.AddWithValue("@Provinsi", cbxtblKota_Provinsi.SelectedItem.ToString());
                                cmdtblKota.Parameters.AddWithValue("@Kota_Kabupaten", cbxtblKota_Kota_Kabupaten.SelectedItem.ToString());
                                cmdtblKota.Parameters.AddWithValue("@Nama_Kota_Kabupaten", txttblKota_Nama_Kota_Kabupaten.Text);
                                cmdtblKota.Parameters.AddWithValue("@Kecamatan", txttblKota_Kecamatan.Text);
                                cmdtblKota.Parameters.AddWithValue("@Desa_Kelurahan", txttblKota_Desa_Kelurahan.Text);
                                cmdtblKota.Parameters.AddWithValue("@Kode_Pos", txttblKota_Kode_Pos.Text);
                                cmdtblKota.Parameters.AddWithValue("@UEdit", "ITDIVISI");
                                RowAffected = cmdtblKota.ExecuteNonQuery();
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


            if (Mode == "New")
            {
                Mode = "View";
                MessageBox.Show("Insert Success");
                drtblKota.Close();
            }
            else
            {
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

        private Boolean CekKota()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblKota WHERE tblKota_Kota_Kabupaten=@Kota_Kabupaten";
                strSql += " AND tblKota_Nama_Kota_Kabupaten=@Nama_Kota_Kabupaten";
                using (SqlCommand cmdtblKota = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblKota.Parameters.AddWithValue("@Kota_Kabupaten", cbxtblKota_Kota_Kabupaten.SelectedItem.ToString());
                    cmdtblKota.Parameters.AddWithValue("@Nama_Kota_Kabupaten", txttblKota_Nama_Kota_Kabupaten.Text.Trim());
                    drtblKota = cmdtblKota.ExecuteReader();
                    if (drtblKota.HasRows)
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
                drtblKota.Close();
                ConnMaster.Close();
            }

            return vBol;
        }

        private Boolean CekNamaKota()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblKota ";
                strSql += " WHERE tblKota_Provinsi=@Provinsi";
                strSql += " AND tblKota_Kota_Kabupaten=@Kota_Kabupaten";
                strSql += " AND tblKota_Nama_Kota_Kabupaten=@Nama_Kota_Kabupaten";
                strSql += " AND tblKota_Kecamatan=@Kecamatan";
                strSql += " AND tblKota_Desa_Kelurahan=@Desa_Kelurahan";
                strSql += " AND tblKota_Kode_Pos=@Kode_Pos";
                strSql += " AND tblKota_RecId<>@RecId";
                using (SqlCommand cmdtblKota = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblKota.Parameters.AddWithValue("@Provinsi", cbxtblKota_Provinsi.SelectedItem.ToString());
                    cmdtblKota.Parameters.AddWithValue("@Kota_Kabupaten", cbxtblKota_Kota_Kabupaten.SelectedItem.ToString());
                    cmdtblKota.Parameters.AddWithValue("@Nama_Kota_Kabupaten", txttblKota_Nama_Kota_Kabupaten.Text.Trim());
                    cmdtblKota.Parameters.AddWithValue("@Kecamatan", txttblKota_Kecamatan.Text.Trim());
                    cmdtblKota.Parameters.AddWithValue("@Desa_Kelurahan", txttblKota_Desa_Kelurahan.Text.Trim());
                    cmdtblKota.Parameters.AddWithValue("@Kode_Pos", txttblKota_Kode_Pos.Text.Trim());
                    cmdtblKota.Parameters.AddWithValue("@RecId", RecId);
                    drtblKota = cmdtblKota.ExecuteReader();
                    if (drtblKota.HasRows)
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
                drtblKota.Close();
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

            if (!CekKota())
            {
                MessageBox.Show("Nama Kota " + txttblKota_Nama_Kota_Kabupaten.Text + " tidak ditemukan..");
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Nama Kota " + txttblKota_Nama_Kota_Kabupaten.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "DELETE FROM tblKota WHERE tblKota_Kota_Kabupaten=@Kota_Kabupaten";
                            strSql += " AND tblKota_Nama_Kota_Kabupaten=@Nama_Kota_Kabupaten";
                            using (SqlCommand cmdtblKota = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblKota.Parameters.AddWithValue("@Kota_Kabupaten", cbxtblKota_Kota_Kabupaten.SelectedItem.ToString());
                                cmdtblKota.Parameters.AddWithValue("@Nama_Kota_Kabupaten", txttblKota_Nama_Kota_Kabupaten.Text.Trim());
                                RowAffected = cmdtblKota.ExecuteNonQuery();
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
                    MessageBox.Show("Nama Kota " + txttblKota_Nama_Kota_Kabupaten.Text + ", berhasil dihapus..");
                }
                this.Close();
            }
        }

        private void FrmM_Kota_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            } 
        }
    }
}
