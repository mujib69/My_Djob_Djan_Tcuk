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

namespace ISBS_New.Master.Rekening
{
    public partial class FrmPop_Rekening : MetroFramework.Forms.MetroForm
    {
        Boolean BolNew;
        string No_Rekening;
        string Kode_Prsh;
        string tblName;
        SqlDataReader Dr;
        SqlConnection ConMaster;
        string strSql;


        string vOldtxtNo_Rekening, vOldtxtCabang, vOldtxtPemilik, vOldBankGroupId, vOldtxtBankGroupName, vOldtxtKeterangan;
        Boolean vOldchkAktif;

        public FrmPop_Rekening(Boolean _BolNew, string _No_Rekening, string _Kode_Prsh, string _tblName)
        {
            InitializeComponent();
            BolNew = _BolNew;
            No_Rekening = _No_Rekening;
            tblName = _tblName;
            Kode_Prsh = _Kode_Prsh;
            txtKode_Prsh.Text = _Kode_Prsh;
            MulaiDariAwal();
        }

        private void MulaiDariAwal()
        {
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            EmptyTextBox();
            ButtonSearch(false);
            Chk_Enable(false);
            Btn_EditCancelSaveDel(false, false, false, false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txtKode_Prsh.ReadOnly = true;
            txtNama_Prsh.ReadOnly = true;
            txtNo_Rekening.ReadOnly = vbol;
            txtCabang.ReadOnly = vbol;
            txtBankGroupId.ReadOnly = vbol;
            txtBankGroupName.ReadOnly = true;
            txtPemilik.ReadOnly = vbol;
            txtKeterangan.ReadOnly = vbol;
        }

        private void Chk_Enable(bool vbol)
        {
            chkAktif.Enabled = vbol;
        }

        private void EmptyTextBox()
        {
            txtNo_Rekening.Text = "";
            txtCabang.Text = "";
            txtBankGroupId.Text = "";
            txtBankGroupName.Text = "";
            txtPemilik.Text = "";
            txtKeterangan.Text = "";
            chkAktif.Checked = false;
        }

        private void ButtonSearch(bool vbol)
        {
            BtnBankGroupId.Enabled = vbol;
        }

        private void Btn_EditCancelSaveDel(bool vEdit, bool vCancel, bool vSave, bool vDel)
        {
            btnEdit.Enabled = vEdit;
            btnCancel.Enabled = vCancel;
            btnSave.Enabled = vSave;
            btnDelete.Enabled = vDel;
        }

        private void FrmPop_Rekening_Load(object sender, EventArgs e)
        {
            if (BolNew)
            {
                EmptyTextBox();
                txt_ReadOnly(false);
                Chk_Enable(true);
                ButtonSearch(true);
                Btn_EditCancelSaveDel(false, true, true, false);
                AmbilNamaPerusahaan();
                this.ActiveControl = txtNo_Rekening;
            }
            else
            {
                Boolean BolFound = false;
                try
                {
                    strSql = "SELECT * ";
                    strSql += "FROM Rekening ";
                    strSql += "WHERE ReffTableName='" + tblName + "' ";
                    strSql += "AND ReffId='" + Kode_Prsh + "' ";
                    strSql += "AND No_Rekening=@No_Rekening";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                    {
                        Cmd.Parameters.AddWithValue("@No_Rekening",No_Rekening);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            BolFound = true;
                            while (Dr.Read())
                            {
                                txtNo_Rekening.Text = Convert.IsDBNull(Dr["No_Rekening"]) ? "" : (string)Dr["No_Rekening"];
                                txtCabang.Text = Convert.IsDBNull(Dr["Cabang"]) ? "" : (string)Dr["Cabang"];
                                txtBankGroupId.Text = Convert.IsDBNull(Dr["BankGroupId"]) ? "" : (string)Dr["BankGroupId"];
                                txtBankGroupName.Text = Convert.IsDBNull(Dr["BankGroupName"]) ? "" : (string)Dr["BankGroupName"];
                                txtPemilik.Text = Convert.IsDBNull(Dr["Pemilik"]) ? "" : (string)Dr["Pemilik"];
                                txtKeterangan.Text = Convert.IsDBNull(Dr["Keterangan"]) ? "" : (string)Dr["Keterangan"];
                                chkAktif.Checked = Convert.IsDBNull(Dr["Aktif"]) ? false : (Boolean)Dr["Aktif"];
                            }
                            Btn_EditCancelSaveDel(true, false, false, true);
                        }
                        else
                        {
                            MessageBox.Show("Data Rekening " + txtNo_Rekening.Text + " tidak ditemukan...");
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

                if (BolFound)
                {
                    AmbilNamaPerusahaan();
                }
            }
        }

        private void AmbilNamaPerusahaan()
        {
            try
            {
                if (tblName == "CustTable")
                {
                    strSql = "SELECT CustName as Nama_Prsh ";
                    strSql += "FROM CustTable ";
                    strSql += "WHERE CustId='" + txtKode_Prsh.Text + "'";
                }
                else
                {
                    strSql = "SELECT VendName as Nama_Prsh ";
                    strSql += "FROM VendTable ";
                    strSql += "WHERE VendId='" + txtKode_Prsh.Text + "'";
                }
                ConMaster = ConnectionString.GetConnection();
                using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                {
                    Dr = Cmd.ExecuteReader();

                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            txtNama_Prsh.Text = Convert.IsDBNull(Dr["Nama_Prsh"]) ? "" : (string)Dr["Nama_Prsh"];
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nama Perusahaan " + txtNama_Prsh.Text + " tidak ditemukan...");
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

        private void BtnBankGroupId_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "BankGroup";
            ControlMgr.tmpSort = "ORDER BY BankGroupId";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Bank";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtBankGroupId.Text = ControlMgr.Kode;
                Ambil_BankGroup();
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void txtBankGroupId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_BankGroup();
            }
        }

        private void Ambil_BankGroup()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM BankGroup WHERE BankGroupId=@BankGroupId";
            using (SqlCommand cmdBank = new SqlCommand(strSql, ConMaster))
            {
                cmdBank.Parameters.AddWithValue("@BankGroupId", txtBankGroupId.Text.Trim());
                Dr = cmdBank.ExecuteReader();

                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        txtBankGroupId.Text = Convert.IsDBNull(Dr["BankGroupId"]) ? "" : (string)Dr["BankGroupId"];
                        txtBankGroupName.Text = Convert.IsDBNull(Dr["BankGroupName"]) ? "" : (string)Dr["BankGroupName"];
                    }
                }
                else
                {
                    MessageBox.Show("Bank " + txtBankGroupId.Text + " doesn't exist");
                }
                Dr.Close();
                ConMaster.Close();
            }
        }

        private void txtBankGroupId_Leave(object sender, EventArgs e)
        {
            if (txtBankGroupId.Text.Trim() != "")
            {
                Cek_BankGroup();
            }
        }

        private void Cek_BankGroup()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM BankGroup WHERE BankGroupId=@BankGroupId";
            using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
            {
                Cmd.Parameters.AddWithValue("@BankGroupId", txtBankGroupId.Text.Trim());
                Dr = Cmd.ExecuteReader();

                if (!Dr.HasRows)
                {
                    MessageBox.Show("Bank " + txtBankGroupId.Text + " doesn't exist");
                    ControlMgr.TblName = "BankGroup";
                    ControlMgr.tmpSort = "ORDER BY BankGroupId";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Bank";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txtBankGroupId.Text = ControlMgr.Kode;
                        Ambil_BankGroup();
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                Dr.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_BankGroup();
                }
                else
                {
                    txtBankGroupId.Focus();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(false);
            txtNo_Rekening.ReadOnly = true;
            Chk_Enable(true);
            ButtonSearch(true);
            Btn_EditCancelSaveDel(false, true, true, false);

            vOldtxtNo_Rekening = txtNo_Rekening.Text;
            vOldtxtCabang = txtCabang.Text;
            vOldBankGroupId = txtBankGroupId.Text;
            vOldtxtBankGroupName = txtBankGroupName.Text;
            vOldtxtPemilik = txtPemilik.Text;
            vOldtxtKeterangan = txtKeterangan.Text;
            
            vOldchkAktif = chkAktif.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Chk_Enable(false);
            ButtonSearch(false);
            Btn_EditCancelSaveDel(true, false, false, true);

            txtNo_Rekening.Text = vOldtxtNo_Rekening;
            txtCabang.Text = vOldtxtCabang;
            txtBankGroupId.Text = vOldBankGroupId;
            txtBankGroupName.Text = vOldtxtBankGroupName;
            txtKeterangan.Text = vOldtxtKeterangan;
            txtPemilik.Text = vOldtxtPemilik;
            chkAktif.Checked = vOldchkAktif;
            this.ActiveControl = btnEdit;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Rekening " + txtNo_Rekening.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                            strSql = "DELETE FROM Rekening WHERE No_Rekening=@No_Rekening";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@No_Rekening",txtNo_Rekening.Text.Trim());
                                cmdCustomer.ExecuteNonQuery();
                            }
                        }
                        scope.Complete();
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

                MessageBox.Show("Berhasil Delete data..");
                this.Close();
            }
        }

        private Boolean CekNo_Rekening()
        {
            strSql="SELECT * FROM Rekening ";
            strSql += "WHERE No_Rekening=@No_Rekening ";
            ConMaster = ConnectionString.GetConnection();            
            using(SqlCommand Cmd = new SqlCommand(strSql,ConMaster))
            {
                Cmd.Parameters.AddWithValue("@No_Rekening", txtNo_Rekening.Text.Trim());
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            Dr.Close();
            ConMaster.Close();
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txtNo_Rekening.Text.Trim() == "")
            {
                MessageBox.Show("No Rekening harus diisi..");
                vBol = false;
            }

            if (BolNew && CekNo_Rekening())
            {
                MessageBox.Show("No Rekening Sudah ada..");
                vBol = false;
            }

            if (txtPemilik.Text.Trim() == "")
            {
                MessageBox.Show("Nama Pemilik harus diisi..");
                vBol = false;
            }

            if (txtBankGroupId.Text.Trim() == "")
            {
                MessageBox.Show("Bank harus dipilih..");
                vBol = false;
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
                            strSql = "INSERT INTO Rekening(ReffTableName,ReffID,No_Rekening,BankGroupId,BankGroupName,Pemilik,Cabang,Keterangan,Aktif,CreatedBy,CreatedDate ";
                            strSql += ") VALUES(";
                            strSql += "@ReffTableName,@Kode_Prsh,@No_Rekening,@BankGroupId,@BankGroupName,@Pemilik,@Cabang,@Keterangan,@Aktif,@Uinput,@UdateInput)";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@ReffTableName", tblName);
                                cmdCustomer.Parameters.AddWithValue("@Kode_Prsh", txtKode_Prsh.Text);
                                cmdCustomer.Parameters.AddWithValue("@No_Rekening", txtNo_Rekening.Text);
                                cmdCustomer.Parameters.AddWithValue("@Cabang", txtCabang.Text);
                                cmdCustomer.Parameters.AddWithValue("@BankGroupId", txtBankGroupId.Text);
                                cmdCustomer.Parameters.AddWithValue("@BankGroupName", txtBankGroupName.Text);
                                cmdCustomer.Parameters.AddWithValue("@Pemilik", txtPemilik.Text);
                                cmdCustomer.Parameters.AddWithValue("@Keterangan", txtKeterangan.Text);
                                cmdCustomer.Parameters.AddWithValue("@Aktif", chkAktif.Checked);
                                cmdCustomer.Parameters.AddWithValue("@Uinput", ControlMgr.UserId);
                                cmdCustomer.Parameters.AddWithValue("@UdateInput", DateTime.Now);
                                cmdCustomer.ExecuteNonQuery();
                            }
                        }
                        else
                        {                           
                            strSql = "UPDATE Rekening SET ";
                            strSql += "Cabang=@Cabang,BankGroupId=@BankGroupId,BankGroupName=@BankGroupName,Pemilik=@Pemilik,Keterangan=@Keterangan,Aktif=@Aktif, ";
                            strSql += "UpdatedBy=@UEdit,UpdatedDate=@UDateEdit ";
                            strSql += "WHERE No_Rekening=@No_Rekening";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@No_Rekening", txtNo_Rekening.Text);
                                cmdCustomer.Parameters.AddWithValue("@Cabang", txtCabang.Text);
                                cmdCustomer.Parameters.AddWithValue("@BankGroupId", txtBankGroupId.Text);
                                cmdCustomer.Parameters.AddWithValue("@BankGroupName", txtBankGroupName.Text);
                                cmdCustomer.Parameters.AddWithValue("@Pemilik", txtPemilik.Text);
                                cmdCustomer.Parameters.AddWithValue("@Keterangan", txtKeterangan.Text);
                                cmdCustomer.Parameters.AddWithValue("@Aktif", chkAktif.Checked);
                                cmdCustomer.Parameters.AddWithValue("@UEdit", ControlMgr.UserId);
                                cmdCustomer.Parameters.AddWithValue("@UDateEdit", DateTime.Now);
                                cmdCustomer.ExecuteNonQuery();
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
                    Chk_Enable(false);
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
            }
        }

        private void txtNo_Rekening_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

        }
    }
}
