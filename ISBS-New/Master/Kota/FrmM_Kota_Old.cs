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
    public partial class FrmM_Kota_Old : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConMaster=new SqlConnection();
        SqlDataReader drKota,drProvinsi;
        string strSql, vOldtxtKota, vOldtxtProvinsi,vOldtxtDaerah;
        Boolean BolNew;

        public FrmM_Kota_Old(bool _BolNew, string _kode,string _Kode2)
        {
            InitializeComponent();
            BolNew = _BolNew;
            txtkota.Text = _kode;
            MulaiDariAwal();
        }

        FrmL_Kota Parent;

        public void setParent(FrmL_Kota F)
        {
            Parent = F;
        }

        private void FrmM_Kota_Load(object sender, EventArgs e)
        {
            if (BolNew)
            {
                EmptyTextBox();
                txt_ReadOnly(false);
                ButtonSearch(true);
                Btn_EditCancelSaveDel(false, true, true, false);
                this.ActiveControl = txtkota;
            }
            else
            {
                try
                {
                    ConMaster = ConnectionString.GetConnection();
                    strSql = "SELECT * FROM kota ";
                    strSql += "WHERE Kota='" + txtkota.Text + "'";
                    using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
                    {
                        drKota = cmdKota.ExecuteReader();
                        if (drKota.HasRows)
                        {
                            while (drKota.Read())
                            {
                                txtprovinsi.Text = Convert.IsDBNull(drKota["provinsi"]) ? "" : (string)drKota["provinsi"];
                                txtdaerah.Text = Convert.IsDBNull(drKota["daerah"]) ? "" : (string)drKota["daerah"];
                            }
                            Btn_EditCancelSaveDel(true, false, false, true);
                        }
                        else
                        {
                            MessageBox.Show("Data Kota " + txtkota.Text + " tidak ditemukan...");
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
                    drKota.Close();
                    ConMaster.Close();
                }
            }
        }

        private void MulaiDariAwal()
        {
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            EmptyTextBox();
            ButtonSearch(false);
            Btn_EditCancelSaveDel(false, false, false, false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txtkota.ReadOnly = vbol;
            txtprovinsi.ReadOnly = vbol;
            txtdaerah.ReadOnly = vbol;
        }

        private void EmptyTextBox()
        {
            txtprovinsi.Text = "";
            txtdaerah.Text = "";
        }

        private void ButtonSearch(bool vbol)
        {
            Btntxtprovinsi.Enabled = vbol;
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
            this.ActiveControl = txtprovinsi;
            txt_ReadOnly(false);
            txtkota.ReadOnly = true;
            ButtonSearch(true);
            Btn_EditCancelSaveDel(false, true, true, false);

            vOldtxtKota = txtkota.Text;
            vOldtxtProvinsi = txtprovinsi.Text;
            vOldtxtDaerah = txtdaerah.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            ButtonSearch(false);
            Btn_EditCancelSaveDel(true, false, false, true);

            txtkota.Text = vOldtxtKota;
            txtprovinsi.Text = vOldtxtProvinsi;
            txtdaerah.Text = vOldtxtDaerah;
         
            this.ActiveControl = btnEdit;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kota " + txtkota.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                            strSql = "DELETE FROM kota WHERE Kota='" + txtkota.Text + "'";
                            using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
                            {
                                cmdKota.ExecuteNonQuery();
                            }
                        }
                        scope.Complete();
                        MessageBox.Show("Berhasil Delete data..");


                        var FrmL_Kota = Application.OpenForms.OfType<Master.Kota.FrmL_Kota>().FirstOrDefault();
                        if (FrmL_Kota != null)
                        {
                            FrmL_Kota.Activate();
                        }
                        else
                        {
                            new Master.Kota.FrmL_Kota().Show();
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
            if (txtkota.Text.Trim() == "")
            {
                MessageBox.Show("Nama Kota harus diisi..");
                vBol = false;
            }
            else
            {
                txtkota.Text = txtkota.Text.Trim().ToUpper();
            }

            if (txtprovinsi.Text.Trim() == "")
            {
                MessageBox.Show("Nama Provinsi harus diisi..");
                vBol = false;
            }
            else
            {
                txtprovinsi.Text = txtprovinsi.Text.Trim().ToUpper();
            }

            if (txtdaerah.Text.Trim() == "")
            {
                MessageBox.Show("Isikan Daerah dengan DALAM KOTA / LUAR KOTA");
                txtdaerah.Focus();
                vBol = false;
            }
            else if (txtdaerah.Text.Trim() != "" && txtdaerah.Text.ToUpper() == "DALAM")
            {
                txtdaerah.Text = "DALAM KOTA";
            }
            else if (txtdaerah.Text.Trim() != "" && txtdaerah.Text.ToUpper() == "LUAR")
            {
                txtdaerah.Text = "LUAR KOTA";
            }
            else if (txtdaerah.Text.Trim() != "" && txtdaerah.Text.ToUpper() != "DALAM KOTA" && txtdaerah.Text.ToUpper() != "LUAR KOTA")
            {
                MessageBox.Show("Isikan Daerah dengan DALAM KOTA / LUAR KOTA");
                txtdaerah.Focus();
                vBol = false;
            }
            else
            {
                txtdaerah.Text = txtdaerah.Text.ToUpper();
            }

            try
            {
                if (BolNew) //mode new
                {
                    strSql = "SELECT * FROM kota WHERE Kota=@Kota";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
                    {
                        cmdKota.Parameters.AddWithValue("@Kota", txtkota.Text);
                        drKota = cmdKota.ExecuteReader();

                        if (drKota.HasRows)
                        {
                            MessageBox.Show("Nama Kota Sudah ada");
                            vBol = false;
                        }
                    }
                }
            }        
            catch(Exception ex)
            {          
                MessageBox.Show(ex.Message.ToString());           
            }
            finally
            {
                drKota.Close();   
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
                            strSql = "INSERT INTO kota(Kota,Provinsi,Daerah,CreatedBy,CreatedDate ";
                            strSql += ") VALUES(";
                            strSql += "@Kota,@Provinsi,@Daerah,@Uinput,@UdateInput)";
                            using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
                            {
                                cmdKota.Parameters.AddWithValue("@Kota", txtkota.Text);
                                cmdKota.Parameters.AddWithValue("@Provinsi", txtprovinsi.Text);
                                cmdKota.Parameters.AddWithValue("@Daerah", txtdaerah.Text);
                                cmdKota.Parameters.AddWithValue("@Uinput", ControlMgr.Username);
                                cmdKota.Parameters.AddWithValue("@UdateInput", DateTime.Now);
                                cmdKota.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            strSql = "UPDATE kota SET ";
                            strSql += "Kota=@Kota,Provinsi=@Provinsi,Daerah=@Daerah,UpdatedBy=@UEdit,UpdatedDate=@UDateEdit ";
                            strSql += "WHERE Kota=@Kota";
                            using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
                            {
                                cmdKota.Parameters.AddWithValue("@Kota", txtkota.Text);
                                cmdKota.Parameters.AddWithValue("@Provinsi", txtprovinsi.Text);
                                cmdKota.Parameters.AddWithValue("@Daerah", txtdaerah.Text);
                                cmdKota.Parameters.AddWithValue("@UEdit", ControlMgr.Username);
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
                    ButtonSearch(false);
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

                Form parentform = Application.OpenForms["FrmL_Kota"];
                if (parentform != null)
                    Parent.ResetGrid();
            }
        }

        private void Cek_Provinsi()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Provinsi WHERE Provinsi=@Provinsi";
            using (SqlCommand cmdProvinsi = new SqlCommand(strSql, ConMaster))
            {
                cmdProvinsi.Parameters.AddWithValue("@Provinsi", txtprovinsi.Text);
                drProvinsi = cmdProvinsi.ExecuteReader();
                if (!drProvinsi.HasRows)
                {
                    MessageBox.Show("Provinsi " + txtprovinsi.Text + " doesn't exist");

                    ControlMgr.TblName = "Provinsi";
                    ControlMgr.tmpSort = "ORDER BY Provinsi";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Provinsi";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txtprovinsi.Text = ControlMgr.Kode;
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drProvinsi.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_Provinsi();
                }
                else
                {
                    txtprovinsi.Focus();
                }
            }
        }

        private void Ambil_Provinsi()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Provinsi WHERE Provinsi=@Provinsi";

            using (SqlCommand cmdProvinsi = new SqlCommand(strSql, ConMaster))
            {
                cmdProvinsi.Parameters.AddWithValue("@Provinsi", txtprovinsi.Text);
                drProvinsi = cmdProvinsi.ExecuteReader();

                if (drProvinsi.HasRows)
                {
                    while (drProvinsi.Read())
                    {
                        txtprovinsi.Text = Convert.IsDBNull(drProvinsi["Provinsi"]) ? "" : (string)drProvinsi["Provinsi"];
                    }
                }
                else
                {
                    MessageBox.Show("Provinsi " + txtprovinsi.Text + " doesn't exist");
                }
                drProvinsi.Close();
                ConMaster.Close();
            }
        }
        
        private void Btnprovinsi_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "Provinsi";
            ControlMgr.tmpSort = "ORDER BY Provinsi";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Provinsi";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtprovinsi.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        //private void txtdaerah_Leave(object sender, EventArgs e)
        //{
        //    if (txtdaerah.Text != "" && txtdaerah.Text.ToUpper() != "DALAM KOTA" && txtdaerah.Text.ToUpper() != "LUAR KOTA")
        //    {
        //        MessageBox.Show("Isikan Daerah dengan DALAM / LUAR KOTA..");
        //        txtdaerah.Focus();
        //    }
        //    else
        //    {
        //        txtdaerah.Text.ToUpper();
        //    }
        //}

        private void txtprovinsi_Leave(object sender, EventArgs e)
        {
            if (txtprovinsi.Text.Trim() != "")
            {
                Cek_Provinsi();
            }
        }

        private void txtprovinsi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Provinsi();
            }
        }    
    }
}
