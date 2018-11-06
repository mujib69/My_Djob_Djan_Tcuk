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

namespace ISBS_New.Master.Customer
{
    public partial class FrmPop_Contact : MetroFramework.Forms.MetroForm
    {
        Boolean BolNew;
        string Deskripsi;
        string tblName;
        string Kode_Prsh;
        SqlDataReader Dr;
        SqlConnection ConMaster;
        string strSql;

        string vOldtxtDeskripsi, vOldtxtContact, vOldtxtExtNo;
        string vOldCmb_Type;
        Boolean vOldchkPrimaryC;


        public FrmPop_Contact(Boolean _BolNew, string _Deskripsi, string _Kode_Prsh,string _tblName)
        {
            InitializeComponent();
            BolNew = _BolNew;
            Deskripsi = _Deskripsi;
            tblName = _tblName;
            Kode_Prsh = _Kode_Prsh;
            txtKode_Prsh.Text = _Kode_Prsh;
            MulaiDariAwal();

            IsiCmbItem();
        }

        private void IsiCmbItem()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT ContactType FROM ContactType";
            using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    Cmb_Type.Items.Add(Dr["ContactType"].ToString());
                }
            }
        }

        private void MulaiDariAwal()
        {
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            EmptyTextBox();
            Chk_Enable(false);
            Cmb_Enable(false);
            Btn_EditCancelSaveDel(false, false, false, false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txtKode_Prsh.ReadOnly = true;
            txtNama_Prsh.ReadOnly = true;
            txtDeskripsi.ReadOnly = vbol;
            txtContactNo.ReadOnly = vbol;
            txtExtNo.ReadOnly = vbol;
        }

        private void Chk_Enable(bool vbol)
        {
            chkPrimaryC.Enabled = vbol;
        }

        private void Cmb_Enable(bool vbol)
        {
            Cmb_Type.Enabled = vbol;
        }

        private void EmptyTextBox()
        {
            txtDeskripsi.Text = "";
            txtContactNo.Text = "";
            txtExtNo.Text = "";
            chkPrimaryC.Checked = false;
        }

        private void Btn_EditCancelSaveDel(bool vEdit, bool vCancel, bool vSave, bool vDel)
        {
            btnEdit.Enabled = vEdit;
            btnCancel.Enabled = vCancel;
            btnSave.Enabled = vSave;
            btnDelete.Enabled = vDel;
        }

        private void FrmPop_Contact_Load(object sender, EventArgs e)
        {
            if (BolNew)
            {
                EmptyTextBox();
                txt_ReadOnly(false);
                Chk_Enable(true);
                Cmb_Enable(true);
                Btn_EditCancelSaveDel(false, true, true, false);
                AmbilNamaPerusahaan();
                this.ActiveControl = txtDeskripsi;
            }
            else
            {
                Boolean BolFound = false;
                try
                {
                    strSql = "SELECT * ";
                    strSql += "FROM Contact ";
                    strSql += "WHERE ReffTableName='" + tblName + "' ";
                    strSql += "AND ReffId='" + Kode_Prsh + "' ";
                    strSql += "AND Deskripsi=@Deskripsi ";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                    {
                        Cmd.Parameters.AddWithValue("@Deskripsi",Deskripsi);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            BolFound = true;
                            while (Dr.Read())
                            {
                                txtKode_Prsh.Text = Convert.IsDBNull(Dr["ReffID"]) ? "" : (string)Dr["ReffID"];
                                txtDeskripsi.Text = Convert.IsDBNull(Dr["Deskripsi"]) ? "" : (string)Dr["Deskripsi"];
                                txtContactNo.Text = Convert.IsDBNull(Dr["ContactNo"]) ? "" : (string)Dr["ContactNo"];
                                txtExtNo.Text = Convert.IsDBNull(Dr["ExtNo"]) ? "" : (string)Dr["ExtNo"];
                                Cmb_Type.SelectedItem = Convert.IsDBNull(Dr["ContactType"]) ? "" : (string)Dr["ContactType"];
                                chkPrimaryC.Checked = Convert.IsDBNull(Dr["PrimaryC"]) ? false : (Boolean)Dr["PrimaryC"];
                            }
                            Btn_EditCancelSaveDel(true, false, false, true);
                        }
                        else
                        {
                            MessageBox.Show("Data Contact " + txtDeskripsi.Text + " tidak ditemukan...");
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(false);
            Chk_Enable(true);
            Cmb_Enable(true);
            Btn_EditCancelSaveDel(false, true, true, false);

            vOldtxtDeskripsi = txtDeskripsi.Text;
            vOldtxtContact = txtContactNo.Text;
            vOldtxtExtNo = txtExtNo.Text;
            vOldCmb_Type = Convert.IsDBNull(Cmb_Type.SelectedItem) ? "" : (string)Cmb_Type.SelectedItem;
            vOldchkPrimaryC = chkPrimaryC.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Chk_Enable(false);
            Btn_EditCancelSaveDel(true, false, false, true);

            txtDeskripsi.Text = vOldtxtDeskripsi;
            txtContactNo.Text = vOldtxtContact;
            txtExtNo.Text = vOldtxtExtNo;
          
            Cmb_Type.SelectedItem = vOldCmb_Type;
            chkPrimaryC.Checked = vOldchkPrimaryC;
            this.ActiveControl = btnEdit;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Alamat " + txtDeskripsi.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                            strSql = "DELETE FROM Contact ";
                            strSql += "WHERE ReffTableName='" + tblName +"'";
                            strSql += "AND Reffid='" + txtKode_Prsh.Text +"'";
                            strSql += "AND Deskripsi=@Deskripsi";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@Deskripsi",txtDeskripsi.Text.Trim());
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

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txtDeskripsi.Text.Trim() == "")
            {
                MessageBox.Show("Nama Address harus diisi..");
                vBol = false;
            }

            if (Cmb_Type.GetItemText(Cmb_Type.SelectedItem) == "")
            {
                MessageBox.Show("Type Contact harus dipilih..");
                vBol = false;
            }

            if (txtContactNo.Text.Trim() == "")
            {
                MessageBox.Show("Address harus diisi..");
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
                            if (chkPrimaryC.Checked)
                            {
                                strSql = "UPDATE Contact SET PRIMARYC=0 ";
                                strSql += "WHERE ReffTableName='" + tblName + "' ";
                                strSql += "AND ReffID='" + txtKode_Prsh.Text + "'";
                                strSql += "AND ContactType='" + Cmb_Type.SelectedItem + "'";
                                using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                                {
                                    Cmd.ExecuteNonQuery();

                                }
                            }

                            strSql = "INSERT INTO Contact(ReffTableName,ReffID,Deskripsi,ContactType,ContactNo,ExtNo,PrimaryC,CreatedBy,CreatedDate ";
                            strSql += ") VALUES(";
                            strSql += "@ReffTableName,@Kode_Prsh,@Deskripsi,@ContactType,@Contact,@ExtNo,@Primary,@Uinput,@UdateInput)";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@ReffTableName", tblName);
                                cmdCustomer.Parameters.AddWithValue("@Kode_Prsh", txtKode_Prsh.Text);
                                cmdCustomer.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text);
                                cmdCustomer.Parameters.AddWithValue("@Contact", txtContactNo.Text);
                                cmdCustomer.Parameters.AddWithValue("@ExtNo", txtExtNo.Text);
                                cmdCustomer.Parameters.AddWithValue("@ContactType", Convert.IsDBNull(Cmb_Type.SelectedItem) ? "" : Cmb_Type.GetItemText(Cmb_Type.SelectedItem));
                                cmdCustomer.Parameters.AddWithValue("@Primary", chkPrimaryC.Checked);
                                cmdCustomer.Parameters.AddWithValue("@Uinput", ControlMgr.UserId);
                                cmdCustomer.Parameters.AddWithValue("@UdateInput", DateTime.Now);
                                cmdCustomer.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            if (chkPrimaryC.Checked)
                            {
                                strSql = "UPDATE Contact SET PRIMARYC=0 ";
                                strSql += "WHERE ReffTableName='" + tblName + "' ";
                                strSql += "AND ReffID='" + txtKode_Prsh.Text + "'";
                                strSql += "AND ContactType='" + Cmb_Type.SelectedItem + "'";
                                using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                            }

                            strSql = "UPDATE Contact SET Deskripsi=@Deskripsi, ";
                            strSql += "ContactType=@ContactType,ContactNo=@ContactNo,ExtNo=@ExtNo,PrimaryC=@Primary, ";
                            strSql += "UpdatedBy=@UEdit,UpdatedDate=@UDateEdit ";
                            strSql += "WHERE ReffTableName='" + tblName + "' ";
                            strSql += "AND ReffId='" + txtKode_Prsh.Text + "' ";
                            strSql += "AND Deskripsi=@vOldDeskripsi";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@vOldDeskripsi", vOldtxtDeskripsi);
                                cmdCustomer.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                                cmdCustomer.Parameters.AddWithValue("@ExtNo", txtExtNo.Text);                                
                                cmdCustomer.Parameters.AddWithValue("@ContactType", Cmb_Type.GetItemText(Cmb_Type.SelectedItem));
                                cmdCustomer.Parameters.AddWithValue("@Primary", chkPrimaryC.Checked);
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
                    Cmb_Enable(false);
                    Chk_Enable(false);
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
