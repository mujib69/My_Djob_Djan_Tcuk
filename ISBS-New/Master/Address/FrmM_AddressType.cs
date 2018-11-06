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

namespace ISBS_New.Master.Address
{
    public partial class FrmM_AddressType : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConMaster = new SqlConnection();
        SqlDataReader Dr;
        string strSql, vOldtxtType, vOldtxtDeskripsi;
        Boolean BolNew;

        public FrmM_AddressType(bool _BolNew, string _kode)
        {
            InitializeComponent();
            BolNew = _BolNew;
            txtType.Text = _kode;
            MulaiDariAwal();
        }

        private void FrmM_AddressType_Load(object sender, EventArgs e)
        {
            if (BolNew)
            {
                EmptyTextBox();
                txt_ReadOnly(false);
                Btn_EditCancelSaveDel(false, true, true, false);
                this.ActiveControl = txtType;
            }
            else
            {
                try
                {
                    ConMaster = ConnectionString.GetConnection();
                    strSql = "SELECT * FROM AddressType ";
                    strSql += "WHERE PurposeType='" + txtType.Text + "'";
                    using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                    {
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                txtType.Text = Convert.IsDBNull(Dr["PurposeType"]) ? "" : (string)Dr["PurposeType"];
                                txtDeskripsi.Text = Convert.IsDBNull(Dr["Deskripsi"]) ? "" : (string)Dr["Deskripsi"];
                            }
                            Btn_EditCancelSaveDel(true, false, false, true);
                        }
                        else
                        {
                            MessageBox.Show("Data Contact Type " + txtType.Text + " tidak ditemukan...");
                            this.BeginInvoke(new MethodInvoker(this.Close));
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Dr.Close();
                    ConMaster.Close();
                }
            }
        }

        private void MulaiDariAwal()
        {
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            EmptyTextBox();
            Btn_EditCancelSaveDel(false, false, false, false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txtType.ReadOnly = vbol;
            txtDeskripsi.ReadOnly = vbol;
        }

        private void EmptyTextBox()
        {
            txtDeskripsi.Text = "";
        }

        private void Btn_EditCancelSaveDel(bool vEdit, bool vCancel, bool vSave, bool vDel)
        {
            btnEdit.Enabled = vEdit;
            btnCancel.Enabled = vCancel;
            btnSave.Enabled = vSave;
            btnDelete.Enabled = vDel;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.ActiveControl = txtDeskripsi;
            txt_ReadOnly(false);
            txtType.ReadOnly = true;
            Btn_EditCancelSaveDel(false, true, true, false);

            vOldtxtType = txtType.Text;
            vOldtxtDeskripsi = txtDeskripsi.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Btn_EditCancelSaveDel(true, false, false, true);

            txtType.Text = vOldtxtType;
            txtDeskripsi.Text = vOldtxtDeskripsi;

            this.ActiveControl = btnEdit;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Address Type " + txtType.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        ConMaster = ConnectionString.GetConnection();
                        using (ConMaster)
                        {
                            string strSql;
                            strSql = "DELETE FROM AddressType WHERE PurposeType='" + txtType.Text + "'";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        scope.Complete();
                        MessageBox.Show("Berhasil Delete data..");


                        var FrmL_AddressType = Application.OpenForms.OfType<Master.AddressType.FrmL_AddressType>().FirstOrDefault();
                        if (FrmL_AddressType != null)
                        {
                            FrmL_AddressType.Activate();
                        }
                        else
                        {
                            new Master.AddressType.FrmL_AddressType().Show();
                        }
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    ConMaster.Close();
                }
            } 
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txtType.Text == "")
            {
                MessageBox.Show("Address Type harus diisi..");
                vBol = false;
            }

            try
            {
                strSql = "SELECT * FROM AddressType WHERE PurposeType=@Type";
                ConMaster = ConnectionString.GetConnection();
                using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                {
                    Cmd.Parameters.AddWithValue("@Type", txtType.Text);
                    Dr = Cmd.ExecuteReader();

                    if (Dr.HasRows)
                    {
                        MessageBox.Show("Address Type Sudah ada");
                        vBol = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Dr.Close();
                ConMaster.Close();
            }
            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (BolNew && !ValidGeneral())
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
                            strSql = "INSERT INTO AddressType(PurposeType,Deskripsi,CreatedBy,CreatedDate ";
                            strSql += ") VALUES(";
                            strSql += "@Type,@Deskripsi,@Uinput,@UdateInput)";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                            {
                                Cmd.Parameters.AddWithValue("@Type", txtType.Text);
                                Cmd.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text);
                                Cmd.Parameters.AddWithValue("@Uinput", ControlMgr.UserId);
                                Cmd.Parameters.AddWithValue("@UdateInput", DateTime.Now);
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            strSql = "UPDATE AddressType SET ";
                            strSql += "PurposeType=@Type,Deskripsi=@Deskripsi,UpdatedBy=@UEdit,UpdatedDate=@UDateEdit ";
                            strSql += "WHERE PurposeType=@Type";
                            using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
                            {
                                cmdKota.Parameters.AddWithValue("@Type", txtType.Text);
                                cmdKota.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text);
                                cmdKota.Parameters.AddWithValue("@UEdit", ControlMgr.UserId);
                                cmdKota.Parameters.AddWithValue("@UDateEdit", DateTime.Now);
                                cmdKota.ExecuteNonQuery();
                            }
                        }
                    }
                    scope.Complete();
                    if (BolNew)
                    {
                        MessageBox.Show("Berhasil Insert data..");
                        BolNew = false;
                    }
                    else
                    {
                        MessageBox.Show("Berhasil Update data..");
                    }

                    txt_ReadOnly(true);
                    Btn_EditCancelSaveDel(true, false, false, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                ConMaster.Close();
            }
        }
    }
}
