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
    public partial class FrmPop_Address : MetroFramework.Forms.MetroForm
    {
        Boolean BolNew;
        string Name_Address;
        string tblName;
        string Kode_Prsh;
        SqlDataReader Dr;
        SqlConnection ConMaster;
        string strSql;

        string vOldtxtNama_Address, vOldtxtAddress, vOldtxtKota, vOldtxtKode_Pos, vOldtxtRT, vOldtxtRW, vOldtxtprovinsi;
        string vOldCmb_Purpose_Purpose;
        Boolean vOldchkPrimaryC;

        public FrmPop_Address(Boolean _BolNew, string _Name_Address, string _Kode_Prsh, string _tblName)
        {
            InitializeComponent();
            BolNew = _BolNew;
            Name_Address = _Name_Address;
            tblName = _tblName;
            Kode_Prsh = _Kode_Prsh;
            txtKode_Prsh.Text = _Kode_Prsh;
            MulaiDariAwal();
            IsiCmbItem();
        }

        private void IsiCmbItem()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT PurposeType FROM AddressType";
            using(SqlCommand Cmd = new SqlCommand(strSql,ConMaster))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    Cmb_Purpose.Items.Add(Dr["PurposeType"].ToString());
                }
            }
        }

        private void MulaiDariAwal()
        {
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            EmptyTextBox();
            ButtonSearch(false);
            Chk_Enable(false);
            Cmb_Enable(false);
            Btn_EditCancelSaveDel(false, false, false, false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txtKode_Prsh.ReadOnly = true;
            txtNama_Prsh.ReadOnly = true;
            txtNama_Address.ReadOnly = vbol;
            txtAddress.ReadOnly = vbol;
            txtKota.ReadOnly = vbol;
            txtKode_Pos.ReadOnly = vbol;
            txtRT.ReadOnly = vbol;
            txtRW.ReadOnly = vbol;
            txtProvinsi.ReadOnly = true;
        }

        private void Chk_Enable(bool vbol)
        {
            chkPrimaryC.Enabled = vbol;
        }

        private void Cmb_Enable(bool vbol)
        {
            Cmb_Purpose.Enabled = vbol;
        }

        private void EmptyTextBox()
        {
            txtNama_Address.Text = "";
            txtAddress.Text = "";
            txtKota.Text = "";
            txtRT.Text = "";
            txtRW.Text = "";
            txtProvinsi.Text = "";
            txtKode_Pos.Text = "";
            chkPrimaryC.Checked = false;          
        }

        private void ButtonSearch(bool vbol)
        {
            btntxttblCustomer_Kota.Enabled = vbol;          
        }

        private void Btn_EditCancelSaveDel(bool vEdit, bool vCancel, bool vSave, bool vDel)
        {
            btnEdit.Enabled = vEdit;
            btnCancel.Enabled = vCancel;
            btnSave.Enabled = vSave;
            btnDelete.Enabled = vDel;
        }

        private void FrmPop_Address_Load(object sender, EventArgs e)
        {
            if (BolNew)
            {
                EmptyTextBox();
                txt_ReadOnly(false);
                Chk_Enable(true);
                Cmb_Enable(true);
                ButtonSearch(true);
                Btn_EditCancelSaveDel(false, true, true, false);
                AmbilNamaPerusahaan();
                this.ActiveControl = txtNama_Address;
            }
            else
            {                
                Boolean BolFound = false;
                try
                {
                    strSql = "SELECT * ";
                    strSql += "FROM Address ";
                    strSql += "WHERE ReffTableName='" + tblName + "' ";
                    strSql += "AND ReffId='" + Kode_Prsh + "' ";
                    strSql += "AND Name=@Name_Address";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                    {
                        Cmd.Parameters.AddWithValue("@Name_Address",Name_Address);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            BolFound = true;
                            while (Dr.Read())
                            {
                                txtNama_Address.Text = Convert.IsDBNull(Dr["Name"]) ? "" : (string)Dr["Name"];
                                txtAddress.Text = Convert.IsDBNull(Dr["Address"]) ? "" : (string)Dr["Address"];
                                txtKota.Text = Convert.IsDBNull(Dr["Kota"]) ? "" : (string)Dr["Kota"];
                                txtKode_Pos.Text = Convert.IsDBNull(Dr["Kode_Pos"]) ? "" : (string)Dr["Kode_Pos"];
                                txtRW.Text = Convert.IsDBNull(Dr["RT"]) ? "" : (string)Dr["RT"];
                                txtRT.Text = Convert.IsDBNull(Dr["RW"]) ? "" : (string)Dr["RW"];
                                txtProvinsi.Text = Convert.IsDBNull(Dr["Provinsi"]) ? "" : (string)Dr["Provinsi"];
                                Cmb_Purpose.SelectedItem = Convert.IsDBNull(Dr["PurposeType"]) ? "" : (string)Dr["PurposeType"];                       
                                chkPrimaryC.Checked = Convert.IsDBNull(Dr["PrimaryC"]) ? false : (Boolean)Dr["PrimaryC"];
                            }
                            Btn_EditCancelSaveDel(true, false, false, true);           
                        }
                        else
                        {
                            MessageBox.Show("Data Address " + txtNama_Address.Text + " tidak ditemukan...");
                            this.BeginInvoke(new MethodInvoker(this.Close));
                            return;       
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;     
                }
                finally
                { 
                    Dr.Close();
                    ConMaster.Close();
                }

                if(BolFound)
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
            catch(Exception ex)
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

        private void btntxttblCustomer_Kota_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "VIEW_tblKota";
            ControlMgr.tmpSort = "ORDER BY tblKota_Nama_Kota_Kabupaten";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Kota";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtKota.Text = ControlMgr.Kode;
                Ambil_Kota();
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void txttblCustomer_Kota_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Kota();
            }
        }

        private void Ambil_Kota()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Kota WHERE Kota=@Kota";
            using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
            {
                cmdKota.Parameters.AddWithValue("@Kota", txtKota.Text.Trim());
                Dr = cmdKota.ExecuteReader();

                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        txtKota.Text = Convert.IsDBNull(Dr["Kota"]) ? "" : (string)Dr["Kota"];
                        txtProvinsi.Text = Convert.IsDBNull(Dr["Provinsi"]) ? "" : (string)Dr["Provinsi"];
                    }
                }
                else
                {
                    MessageBox.Show("Kota " + txtKota.Text + " doesn't exist");
                }
                Dr.Close();
                ConMaster.Close();
            }
        }

        private void txttblCustomer_Kota_Leave(object sender, EventArgs e)
        {
            if (txtKota.Text.Trim() != "")
            {
                Cek_Kota();
            }
        }

        private void Cek_Kota()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Kota WHERE Kota=@Kota";
            using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
            {
                Cmd.Parameters.AddWithValue("@Kota", txtKota.Text.Trim());
                Dr = Cmd.ExecuteReader();

                if (!Dr.HasRows)
                {
                    MessageBox.Show("Kota " + txtKota.Text + " doesn't exist");
                    ControlMgr.TblName = "Kota";
                    ControlMgr.tmpSort = "ORDER BY Kota";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Kota";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txtKota.Text = ControlMgr.Kode;
                        Ambil_Kota();
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
                    Ambil_Kota();
                }
                else
                {
                    txtKota.Focus();
                }
            }
        }       

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(false);
            Chk_Enable(true);
            Cmb_Enable(true);
            ButtonSearch(true);
            Btn_EditCancelSaveDel(false, true, true, false);

            vOldtxtNama_Address = txtNama_Address.Text;
            vOldtxtAddress = txtAddress.Text;
            vOldtxtKota = txtKota.Text;
            vOldtxtKode_Pos = txtKode_Pos.Text;
            vOldtxtRT = txtRT.Text;
            vOldtxtRW = txtRW.Text;
            vOldtxtprovinsi = txtProvinsi.Text;
            vOldCmb_Purpose_Purpose = Convert.IsDBNull(Cmb_Purpose.SelectedItem) ? "" : (string)Cmb_Purpose.SelectedItem;
            vOldchkPrimaryC = chkPrimaryC.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Chk_Enable(false);
            ButtonSearch(false);
            Btn_EditCancelSaveDel(true, false, false, true);

            txtNama_Address.Text=vOldtxtNama_Address;
            txtAddress.Text=vOldtxtAddress;
            txtKota.Text=vOldtxtKota;
            txtKode_Pos.Text=vOldtxtKode_Pos;
            txtRT.Text=vOldtxtRT;
            txtRW.Text=vOldtxtRW;
            txtProvinsi.Text=vOldtxtprovinsi;
            Cmb_Purpose.SelectedItem=vOldCmb_Purpose_Purpose;
            chkPrimaryC.Checked=vOldchkPrimaryC; 
            this.ActiveControl = btnEdit;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Alamat " + txtNama_Address.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                            strSql = "DELETE FROM Address ";
                            strSql += "WHERE ReffTableName='" + tblName + "'";
                            strSql += "AND ReffId='" + txtKode_Prsh.Text + "'";
                            strSql += "AND Name=@Name";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@Name",txtNama_Address.Text.Trim());
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
            if (txtNama_Address.Text.Trim() == "")
            {
                MessageBox.Show("Nama Address harus diisi..");
                vBol = false;
            }

            if (txtAddress.Text.Trim() == "")
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
                                strSql = "UPDATE ADDRESS SET PRIMARYC=0 ";
                                strSql += "WHERE ReffTableName='" + tblName + "' ";
                                strSql += "AND ReffID='" + txtKode_Prsh.Text + "'";
                                strSql += "AND PurposeType='" + Cmb_Purpose.SelectedItem + "'";
                                using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                            }

                            strSql = "INSERT INTO Address(ReffTableName,ReffID,Name,PurposeType,Address,Provinsi,Kota,Kode_Pos,RT,RW,PrimaryC,CreatedBy,CreatedDate ";
                            strSql += ") VALUES(";
                            strSql += "@ReffTableName,@Kode_Prsh,@Name,@Purpose,@Address,@Provinsi,@Kota,@Kode_Pos,@RT,@RW,@Primary,@Uinput,@UdateInput)";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@ReffTableName", tblName);
                                cmdCustomer.Parameters.AddWithValue("@Kode_Prsh", txtKode_Prsh.Text);
                                cmdCustomer.Parameters.AddWithValue("@Name", txtNama_Address.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@Purpose", Convert.IsDBNull(Cmb_Purpose.SelectedItem) ? "" : Cmb_Purpose.GetItemText(Cmb_Purpose.SelectedItem));
                                cmdCustomer.Parameters.AddWithValue("@Address", txtAddress.Text);
                                cmdCustomer.Parameters.AddWithValue("@provinsi", txtProvinsi.Text);
                                cmdCustomer.Parameters.AddWithValue("@Kota", txtKota.Text);
                                cmdCustomer.Parameters.AddWithValue("@Kode_Pos", txtKode_Pos.Text);
                                cmdCustomer.Parameters.AddWithValue("@RT", txtRT.Text);
                                cmdCustomer.Parameters.AddWithValue("@RW", txtRW.Text);
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
                                strSql = "UPDATE ADDRESS SET PRIMARYC=0 ";
                                strSql += "WHERE ReffTableName='" + tblName + "' ";
                                strSql += "AND ReffID='" + txtKode_Prsh.Text + "'";
                                strSql += "AND PurposeType='" + Cmb_Purpose.SelectedItem + "'";
                                using (SqlCommand Cmd = new SqlCommand(strSql, ConMaster))
                                {
                                    Cmd.ExecuteNonQuery();

                                }
                            }

                            strSql = "UPDATE Address SET Name=@Name, ";
                            strSql += "PurposeType=@Purpose,Address=@Address,provinsi=@provinsi,Kota=@Kota,Kode_Pos=@Kode_Pos,RT=@RT,RW=@RW,PrimaryC=@Primary, ";
                            strSql += "UpdatedBy=@UEdit,UpdatedDate=@UDateEdit ";
                            strSql += "WHERE ReffTableName='" + tblName + "' ";
                            strSql += "AND ReffId='" + txtKode_Prsh.Text + "' ";
                            strSql += "AND Name=@vOldName";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@vOldName", vOldtxtNama_Address);
                                cmdCustomer.Parameters.AddWithValue("@Name", txtNama_Address.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@Purpose", Cmb_Purpose.GetItemText(Cmb_Purpose.SelectedItem));                                
                                cmdCustomer.Parameters.AddWithValue("@Address", txtAddress.Text);
                                cmdCustomer.Parameters.AddWithValue("@provinsi", txtProvinsi.Text);
                                cmdCustomer.Parameters.AddWithValue("@Kota", txtKota.Text);
                                cmdCustomer.Parameters.AddWithValue("@Kode_Pos", txtKode_Pos.Text);
                                cmdCustomer.Parameters.AddWithValue("@RT", txtRT.Text);
                                cmdCustomer.Parameters.AddWithValue("@RW", txtRW.Text);
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
    }
}
