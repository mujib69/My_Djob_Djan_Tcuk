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

namespace ISBS_New.Inventory.Master.Bentuk
{
    public partial class FrmM_Bentuk : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConMaster = new SqlConnection();
        SqlDataReader drBentuk;
        string strSql, vOldtxtBentuk;
        Boolean BolNew;

        public FrmM_Bentuk(bool _BolNew, string _kode)
        {
            InitializeComponent();
            BolNew = _BolNew;
            txtBentuk.Text = _kode;
            MulaiDariAwal();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        FrmL_Bentuk Parent;

        public void setParent(FrmL_Bentuk F)
        {
            Parent = F;
        }

        private void FrmM_Bentuk_Load(object sender, EventArgs e)
        {
            if (BolNew)
            {
                EmptyTextBox();
                txt_ReadOnly(false);
                Btn_EditCancelSaveDel(false, true, true, false);
                this.ActiveControl = txtBentuk;
            }
            else
            {
                try
                {
                    ConMaster = ConnectionString.GetConnection();
                    strSql = "SELECT * FROM InventBentuk ";
                    strSql += "WHERE Bentuk='" + txtBentuk.Text + "'";
                    using (SqlCommand cmdBentuk = new SqlCommand(strSql, ConMaster))
                    {
                        drBentuk = cmdBentuk.ExecuteReader();
                        if (drBentuk.HasRows)
                        {
                            while (drBentuk.Read())
                            {
                                //txtdatalain.Text = Convert.IsDBNull(drBentuk["datalain"]) ? "" : (string)drBentuk["datalain"];                               
                            }
                            Btn_EditCancelSaveDel(true, false, false, true);
                        }
                        else
                        {
                            MessageBox.Show("Data Bentuk " + txtBentuk.Text + " tidak ditemukan...");
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
                    drBentuk.Close();
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
            txtBentuk.ReadOnly = vbol;
            //txtdatalain.ReadOnly = vbol;
        }

        private void EmptyTextBox()
        {
            //txtdatalain.Text = "";            
        }

        private void Btn_EditCancelSaveDel(bool vEdit, bool vCancel, bool vSave, bool vDel)
        {
            btnEdit.Enabled = vEdit;
            btnCancel.Enabled = vCancel;
            btnSave.Enabled = vSave;
            btnDelete.Enabled = vDel;
        }

        private bool Used()
        {
            Boolean vBol = false;
            try
            {
                strSql = "Select * From InventTable Where Bentuk='" + txtBentuk.Text.Trim() + "'";
                ConMaster = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
                {
                    drBentuk = cmd.ExecuteReader();
                    if (drBentuk.Read())
                    {
                        vBol = true;
                    }
                    else
                    {
                        vBol = false;
                    }
                    drBentuk.Close();
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

            return vBol;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                if (Used() == true)
                {
                    MessageBox.Show("Tidak boleh Edit, Bentuk sudah pernah digunakan..");
                }
                else
                {
                    //this.ActiveControl = txtdatalain;
                    txt_ReadOnly(false);
                    txtBentuk.ReadOnly = true;
                    Btn_EditCancelSaveDel(false, true, true, false);

                    vOldtxtBentuk = txtBentuk.Text;
                    //vOldtxtDatalain = txtdatalain.Text;
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Btn_EditCancelSaveDel(true, false, false, true);

            txtBentuk.Text = vOldtxtBentuk;
            //txtdatalain.Text = vOldtxtdatalain;

            this.ActiveControl = btnEdit;
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";
            string strSql;

            if (vBol == true)
            {
                try
                {
                    strSql = "Select * From InventBentuk Where Bentuk='" + txtBentuk.Text + "'";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
                    {
                        drBentuk = cmd.ExecuteReader();
                        if (drBentuk.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Bentuk tidak ditemukan..";
                            vBol = false;
                        }
                        drBentuk.Close();
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

            if (vBol == true)
            {
                try
                {
                    strSql = "Select * From InventTable Where Bentuk='" + txtBentuk.Text.Trim() + "'";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
                    {
                        drBentuk = cmd.ExecuteReader();
                        if (drBentuk.Read())
                        {
                            ErrMsg = "Bentuk sudah pernah digunakan..";
                            vBol = false;
                        }
                        else
                        {
                            vBol = true;
                        }
                        drBentuk.Close();
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

            if (vBol == false) { MessageBox.Show(ErrMsg); }
            return vBol;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                if (cekValidasi() == false)
                {
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Bentuk " + txtBentuk.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                                strSql = "DELETE FROM InventBentuk WHERE Bentuk='" + txtBentuk.Text + "'";
                                using (SqlCommand cmdBentuk = new SqlCommand(strSql, ConMaster))
                                {
                                    cmdBentuk.ExecuteNonQuery();
                                }
                            }
                            scope.Complete();
                            MessageBox.Show("Berhasil Delete data..");


                            var FrmL_Bentuk = Application.OpenForms.OfType<Bentuk.FrmL_Bentuk>().FirstOrDefault();
                            if (FrmL_Bentuk != null)
                            {
                                FrmL_Bentuk.Activate();
                            }
                            else
                            {
                                new Bentuk.FrmL_Bentuk().Show();
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
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txtBentuk.Text.Trim() == "")
            {
                MessageBox.Show("Bentuk harus diisi..");
                vBol = false;
            }
            else
            {
                txtBentuk.Text = txtBentuk.Text.Trim().ToUpper();
            }

            /*
            if (txtdatalain.Text.Trim() == "")
            {
                MessageBox.Show("datalain harus diisi..");
                vBol = false;
            }
            else
            {
                txtdatalain.Text = txtdatalain.Text.Trim().ToUpper();
            }*/

            try
            {
                if (BolNew) //mode new
                {
                    strSql = "SELECT * FROM InventBentuk WHERE Bentuk=@Bentuk";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand cmdBentuk = new SqlCommand(strSql, ConMaster))
                    {
                        cmdBentuk.Parameters.AddWithValue("@Bentuk", txtBentuk.Text);
                        drBentuk = cmdBentuk.ExecuteReader();

                        if (drBentuk.HasRows)
                        {
                            MessageBox.Show("Bentuk Sudah ada");
                            vBol = false;
                        }
                    }
                }
                else if (!BolNew)
                {
                    strSql = "SELECT * FROM InventBentuk WHERE Bentuk=@Bentuk";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand cmdBentuk = new SqlCommand(strSql, ConMaster))
                    {
                        cmdBentuk.Parameters.AddWithValue("@Bentuk", txtBentuk.Text);
                        drBentuk = cmdBentuk.ExecuteReader();

                        if (drBentuk.HasRows)
                        {
                            vBol = true;
                        }
                        else
                        {
                            MessageBox.Show("Bentuk tidak ditemukan..");
                            vBol = false;
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
                drBentuk.Close();
                ConMaster.Close();
            }

            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
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
                            strSql = "INSERT INTO InventBentuk(Bentuk,CreatedBy,CreatedDate ";
                            strSql += ") VALUES(";
                            strSql += "@Bentuk,@Uinput,@UdateInput)";
                            using (SqlCommand cmdBentuk = new SqlCommand(strSql, ConMaster))
                            {
                                cmdBentuk.Parameters.AddWithValue("@Bentuk", txtBentuk.Text);
                                //cmdBentuk.Parameters.AddWithValue("@datalain", txtdatalain.Text);
                                cmdBentuk.Parameters.AddWithValue("@Uinput", ControlMgr.UserId);
                                cmdBentuk.Parameters.AddWithValue("@UdateInput", DateTime.Now);
                                cmdBentuk.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            strSql = "UPDATE InventBentuk SET ";
                            strSql += "Bentuk=@Bentuk,UpdatedBy=@UEdit,UpdatedDate=@UDateEdit ";
                            strSql += "WHERE Bentuk=@Bentuk";
                            using (SqlCommand cmdBentuk = new SqlCommand(strSql, ConMaster))
                            {
                                cmdBentuk.Parameters.AddWithValue("@Bentuk", txtBentuk.Text);
                                //cmdBentuk.Parameters.AddWithValue("@datalain", txtdatalain.Text);
                                cmdBentuk.Parameters.AddWithValue("@UEdit", ControlMgr.UserId);
                                cmdBentuk.Parameters.AddWithValue("@UDateEdit", DateTime.Now);
                                cmdBentuk.ExecuteNonQuery();
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

                Form parentform = Application.OpenForms["FrmL_Bentuk"];
                if (parentform != null)
                    Parent.ResetGrid();
            }
        }
    }
}
