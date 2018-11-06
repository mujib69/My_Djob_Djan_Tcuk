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

namespace ISBS_New.Master.Bank
{
    public partial class FrmM_Bank : MetroFramework.Forms.MetroForm
    {
        string Mode = "";

        SqlConnection ConnMaster;
        SqlDataReader drtblBank;
        SqlDataReader drtblBank_Group;
        SqlDataReader drtblKota;
        SqlDataReader drtblCounter;
        SqlDataReader drtblContact;
        string strSql = "";

        DataTable namesTable_Contact = new DataTable("tmpContact");
       
        int RowAffected;

        Boolean vView, vNew, vEdit, vDelete;

        public FrmM_Bank(string _Mode, string _Kode)
        {
            InitializeComponent();
            Mode = _Mode;
            txttblBank_Kode_Bank.Text = _Kode;
        }

        private void Create_tmpContact()
        {
            DataColumn idColumn00 = new DataColumn();
            DataColumn idColumn01 = new DataColumn();
            DataColumn idColumn02 = new DataColumn();
            DataColumn idColumn03 = new DataColumn();
            DataColumn idColumn04 = new DataColumn();
            DataColumn idColumn05 = new DataColumn();
            DataColumn idColumn06 = new DataColumn();

            idColumn00.DataType = System.Type.GetType("System.String");
            idColumn00.ColumnName = "tblContact_Nama";
            idColumn01.DataType = System.Type.GetType("System.String");
            idColumn01.ColumnName = "tblContact_Jabatan";
            idColumn02.DataType = System.Type.GetType("System.String");
            idColumn02.ColumnName = "tblContact_Phone1";
            idColumn03.DataType = System.Type.GetType("System.String");
            idColumn03.ColumnName = "tblContact_Ext1";
            idColumn04.DataType = System.Type.GetType("System.String");
            idColumn04.ColumnName = "tblContact_HP";
            idColumn05.DataType = System.Type.GetType("System.String");
            idColumn05.ColumnName = "tblContact_Email";
            idColumn06.DataType = System.Type.GetType("System.Decimal");
            idColumn06.ColumnName = "tblContact_RecId";


            namesTable_Contact.Columns.Add(idColumn00);
            namesTable_Contact.Columns.Add(idColumn01);
            namesTable_Contact.Columns.Add(idColumn02);
            namesTable_Contact.Columns.Add(idColumn03);
            namesTable_Contact.Columns.Add(idColumn04);
            namesTable_Contact.Columns.Add(idColumn05);
            namesTable_Contact.Columns.Add(idColumn06);
        }


        private void BuatDtGridView_Contact()
        {
            namesTable_Contact.Clear();
            DtGridView_Contact.DataSource = namesTable_Contact;

            namesTable_Contact.Columns[0].ColumnName = "Nama";
            namesTable_Contact.Columns[1].ColumnName = "Jabatan";
            namesTable_Contact.Columns[2].ColumnName = "Phone";
            namesTable_Contact.Columns[3].ColumnName = "Ext";
            namesTable_Contact.Columns[4].ColumnName = "Handphone";
            namesTable_Contact.Columns[5].ColumnName = "Email";
            namesTable_Contact.Columns[6].ColumnName = "RecId";

            DtGridView_Contact.Columns[0].Width = 100;
            DtGridView_Contact.Columns[1].Width = 100;
            DtGridView_Contact.Columns[2].Width = 100;
            DtGridView_Contact.Columns[3].Width = 100;
            DtGridView_Contact.Columns[4].Width = 100;
            DtGridView_Contact.Columns[5].Width = 100;
            DtGridView_Contact.Columns[6].Width = 100;

            DtGridView_Contact.Columns[6].Visible = false;
            DtGridView_Contact.Refresh();
        }

        private void IsiDtGridView_Contact()
        {
            strSql = "SELECT * ";
            strSql += "FROM tblContact ";
            strSql += "WHERE tblContact_tblName='tblBank' ";
            strSql += "AND tblContact_Kode='" + txttblBank_Kode_Bank.Text + "'";
            namesTable_Contact.Clear();
            ConnMaster = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(strSql, ConnMaster))
            {
                drtblContact = cmd.ExecuteReader();
                while (drtblContact.Read())
                {
                    DataRow row;
                    row = namesTable_Contact.NewRow();
                    row["Nama"] = Convert.IsDBNull(drtblContact["tblContact_Nama"]) ? "" : (string)drtblContact["tblContact_Nama"];
                    row["Jabatan"] = Convert.IsDBNull(drtblContact["tblContact_Jabatan"]) ? "" : (string)drtblContact["tblContact_Jabatan"];
                    row["Phone"] = Convert.IsDBNull(drtblContact["tblContact_Phone1"]) ? "" : (string)drtblContact["tblContact_Phone1"];
                    row["Ext"] = Convert.IsDBNull(drtblContact["tblContact_Ext1"]) ? "" : (string)drtblContact["tblContact_Ext1"];
                    row["Handphone"] = Convert.IsDBNull(drtblContact["tblContact_HP"]) ? "" : (string)drtblContact["tblContact_HP"];
                    row["Email"] = Convert.IsDBNull(drtblContact["tblContact_Email"]) ? "" : (string)drtblContact["tblContact_Email"];
                    row["RecId"] = Convert.IsDBNull(drtblContact["tblContact_RecId"]) ? 0 : Convert.ToInt32(drtblContact["tblContact_RecId"]);                    
                    namesTable_Contact.Rows.Add(row);
                }
            }
            drtblContact.Close();
            ConnMaster.Close();
            DtGridView_Contact.Refresh();
        }

        private void FrmM_Bank_Load(object sender, EventArgs e)
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

            mTabCtrl_Bank.SelectedTab = mTabPage_General;
            FillcbxtblBank_Group();
            FillcbxtblBank_Provinsi();

            cbxtblBank_Provinsi.SelectedItem = "";

            cbxtblBank_Kota_Kabupaten.Items.Add("");
            cbxtblBank_Kota_Kabupaten.SelectedItem = "";

            cbxtblBank_Kecamatan.Items.Add("");
            cbxtblBank_Kecamatan.SelectedItem = "";

            cbxtblBank_Desa_Kelurahan.Items.Add("");
            cbxtblBank_Desa_Kelurahan.SelectedItem = "";

            cbxtblBank_Kode_Pos.Items.Add("");
            cbxtblBank_Kode_Pos.SelectedItem = "";

            /*FillcbxtblBank_Kota_Kabupaten();
            FillcbxtblBank_Kecamatan();
            FillcbxtblBank_Desa_Kelurahan();
            FillcbxtblBank_Kode_Pos();*/

            Create_tmpContact();
            DtGridView_Contact.DataSource = "";
            BuatDtGridView_Contact(); 

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
                IsiDtGridView_Contact();
                ModeEdit();
            }
        }

        
        private void FillcbxtblBank_Group()
        {
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblBank_Group";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    drtblBank_Group = Cmd.ExecuteReader();
                    if (drtblBank_Group.HasRows)
                    {
                        while (drtblBank_Group.Read())
                        {
                            cbxtblBank_Group.Items.Add(Convert.IsDBNull(drtblBank_Group["tblBank_Group_Group"]) ? "" : (string)drtblBank_Group["tblBank_Group_Group"]);
                        }
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

        private void FillcbxtblBank_Provinsi() 
        {
            cbxtblBank_Provinsi.Items.Clear();
            cbxtblBank_Provinsi.Items.Add("");   
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
                            cbxtblBank_Provinsi.Items.Add(Convert.IsDBNull(drtblKota["Provinsi"]) ? "" : (string)drtblKota["Provinsi"]);
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
        
        private void FillcbxtblBank_Kota_Kabupaten()
        {
            cbxtblBank_Kota_Kabupaten.Items.Clear();
            cbxtblBank_Kota_Kabupaten.Items.Add("");
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT DISTINCT Nama_Kota_Kabupaten FROM VIEW_tblKota WHERE Provinsi=@Provinsi";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Provinsi",cbxtblBank_Provinsi.SelectedItem.ToString());
                    drtblKota = Cmd.ExecuteReader();
                    if (drtblKota.HasRows)
                    {
                        while (drtblKota.Read())
                        {
                            cbxtblBank_Kota_Kabupaten.Items.Add(Convert.IsDBNull(drtblKota["Nama_Kota_Kabupaten"]) ? "" : (string)drtblKota["Nama_Kota_Kabupaten"]);
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

        private void FillcbxtblBank_Kecamatan()        
        {
            cbxtblBank_Kecamatan.Items.Clear();
            cbxtblBank_Kecamatan.Items.Add("");
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT DISTINCT Kecamatan FROM VIEW_tblKota WHERE Nama_Kota_Kabupaten=@Kota_Kabupaten";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Kota_Kabupaten",cbxtblBank_Kota_Kabupaten.SelectedItem.ToString());
                    drtblKota = Cmd.ExecuteReader();
                    if (drtblKota.HasRows)
                    {
                        while (drtblKota.Read())
                        {
                            cbxtblBank_Kecamatan.Items.Add(Convert.IsDBNull(drtblKota["Kecamatan"]) ? "" : (string)drtblKota["Kecamatan"]);
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

        private void FillcbxtblBank_Desa_Kelurahan()
        {
            cbxtblBank_Desa_Kelurahan.Items.Clear();
            cbxtblBank_Desa_Kelurahan.Items.Add("");
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT DISTINCT Desa_Kelurahan FROM VIEW_tblKota WHERE Kecamatan=@Kecamatan";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Kecamatan",cbxtblBank_Kecamatan.SelectedItem.ToString());
                    drtblKota = Cmd.ExecuteReader();
                    if (drtblKota.HasRows)
                    {
                        while (drtblKota.Read())
                        {
                            cbxtblBank_Desa_Kelurahan.Items.Add(Convert.IsDBNull(drtblKota["Desa_Kelurahan"]) ? "" : (string)drtblKota["Desa_Kelurahan"]);
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

        private void FillcbxtblBank_Kode_Pos()
        {
            cbxtblBank_Kode_Pos.Items.Clear();
            cbxtblBank_Kode_Pos.Items.Add("");
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT DISTINCT Kode_Pos FROM VIEW_tblKota WHERE Desa_Kelurahan=@Desa_Kelurahan";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Desa_Kelurahan",cbxtblBank_Desa_Kelurahan.SelectedItem.ToString());
                    drtblKota = Cmd.ExecuteReader();
                    if (drtblKota.HasRows)
                    {
                        while (drtblKota.Read())
                        {
                            cbxtblBank_Kode_Pos.Items.Add(Convert.IsDBNull(drtblKota["Kode_Pos"]) ? "" : (string)drtblKota["Kode_Pos"]);
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
                strSql = "SELECT * FROM tblBank WHERE tblBank_Kode_Bank=@Kode_Bank";
                using (SqlCommand cmdtblBank = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblBank.Parameters.AddWithValue("@Kode_Bank", txttblBank_Kode_Bank.Text);
                    drtblBank = cmdtblBank.ExecuteReader();
                    if (drtblBank.HasRows)
                    {
                        while (drtblBank.Read())
                        {
                            txttblBank_Kode_Bank.Text = Convert.IsDBNull(drtblBank["tblBank_Kode_Bank"]) ? "" : (string)drtblBank["tblBank_Kode_Bank"];
                            txttblBank_No_Rek.Text = Convert.IsDBNull(drtblBank["tblBank_No_Rek"]) ? "" : (string)drtblBank["tblBank_No_Rek"];
                            txttblBank_Atas_Nama.Text = Convert.IsDBNull(drtblBank["tblBank_Atas_Nama"]) ? "" : (string)drtblBank["tblBank_Atas_Nama"];
                            txttblBank_Cabang.Text = Convert.IsDBNull(drtblBank["tblBank_Cabang"]) ? "" : (string)drtblBank["tblBank_Cabang"];
                            txttblBank_Alamat1.Text = Convert.IsDBNull(drtblBank["tblBank_Alamat1"]) ? "" : (string)drtblBank["tblBank_Alamat1"];
                            txttblBank_Alamat2.Text = Convert.IsDBNull(drtblBank["tblBank_Alamat2"]) ? "" : (string)drtblBank["tblBank_Alamat2"];
                            txttblBank_Ket.Text = Convert.IsDBNull(drtblBank["tblBank_Ket"]) ? "" : (string)drtblBank["tblBank_Ket"];

                            cbxtblBank_Group.SelectedItem = Convert.IsDBNull(drtblBank["tblBank_Group"]) ? "" : (string)drtblBank["tblBank_Group"];

                            cbxtblBank_Provinsi.SelectedItem = Convert.IsDBNull(drtblBank["tblBank_Provinsi"]) ? "" : (string)drtblBank["tblBank_Provinsi"];
                            cbxtblBank_Kota_Kabupaten.SelectedItem = Convert.IsDBNull(drtblBank["tblBank_Kota_Kabupaten"]) ? "" : (string)drtblBank["tblBank_Kota_Kabupaten"];
                            cbxtblBank_Kecamatan.SelectedItem = Convert.IsDBNull(drtblBank["tblBank_Kecamatan"]) ? "" : (string)drtblBank["tblBank_Kecamatan"];
                            cbxtblBank_Desa_Kelurahan.SelectedItem = Convert.IsDBNull(drtblBank["tblBank_Desa_Kelurahan"]) ? "" : (string)drtblBank["tblBank_Desa_Kelurahan"];
                            cbxtblBank_Kode_Pos.SelectedItem = Convert.IsDBNull(drtblBank["tblBank_Kode_Pos"]) ? "" : (string)drtblBank["tblBank_Kode_Pos"];
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data Kode Bank " + txttblBank_Kode_Bank.Text + " tidak ditemukan...");                        
                        return;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                drtblBank.Close();
                ConnMaster.Close();
            }
        }
       
        private void txt_ReadOnly(bool vBol)
        {
            //txttblBank_Kode_Bank.ReadOnly = vBol;
            txttblBank_No_Rek.ReadOnly = vBol;
            txttblBank_Atas_Nama.ReadOnly = vBol;
            txttblBank_Cabang.ReadOnly = vBol;
            txttblBank_Alamat1.ReadOnly = vBol;
            txttblBank_Alamat2.ReadOnly = vBol;
            txttblBank_Ket.ReadOnly = vBol;
        }

        private void cbx_Enable(bool vBol)
        {
            cbxtblBank_Group.Enabled = vBol;
            cbxtblBank_Provinsi.Enabled = vBol;
            cbxtblBank_Kota_Kabupaten.Enabled = vBol;
            cbxtblBank_Kecamatan.Enabled = vBol;
            cbxtblBank_Desa_Kelurahan.Enabled = vBol;
            cbxtblBank_Kode_Pos.Enabled = vBol;
        }

        private void ModeView()
        {
            txt_ReadOnly(true);
            cbx_Enable(false);
            btn_Enable(false); //contact
            Btn_EditCancelSaveDel(true, false, false, true);
        }

        private void btn_Enable(bool vBol)
        {
            btnNew_Ct.Enabled = vBol;
            btnEdit_Ct.Enabled = vBol;
            btnDelete_Ct.Enabled = vBol;
        }

        private void ModeNew()
        {
            this.ActiveControl = cbxtblBank_Group;

            txt_ReadOnly(false);
            cbx_Enable(true);
            btn_Enable(true); //contact

            cbxtblBank_Kota_Kabupaten.Enabled = false;
            cbxtblBank_Kecamatan.Enabled = false;
            cbxtblBank_Desa_Kelurahan.Enabled = false;
            cbxtblBank_Kode_Pos.Enabled = false;
            Btn_EditCancelSaveDel(false, true, true, false);
        }

        private void ModeEdit()
        {
            txt_ReadOnly(false);
            cbx_Enable(true);
            btn_Enable(true); //contact
            
            /*if (Used())
            {
                MessageBox.Show("Kode Bank " + txttblBank_Kode_Bank.Text + " sudah pernah digunakan..");
                txttblBank_Kode_Bank.ReadOnly = true;
            }*/
            /*
            cbxtblBank_Kota_Kabupaten.Enabled = false;
            cbxtblBank_Kecamatan.Enabled = false;
            cbxtblBank_Desa_Kelurahan.Enabled = false;
            cbxtblBank_Kode_Pos.Enabled = false;
             */
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
            if (Mode == "Edit" && vBol && !CekBank())
            {
                MessageBox.Show("Kode Bank " + txttblBank_Kode_Bank + " tidak ditemukan..");
                vBol = false;
            }

            if (vBol && txttblBank_Group_Nama.Text.Trim() == "")
            {
                MessageBox.Show("Bank harus dipilih..");
                this.ActiveControl = cbxtblBank_Group;
                vBol = false;
            }

            if (vBol && txttblBank_No_Rek.Text.Trim() == "")
            {
                MessageBox.Show("No. Rekening harus diisi..");
                this.ActiveControl = txttblBank_No_Rek;
                vBol = false;
            }

            if (Mode == "New" && vBol && CekNoRek())
            {
                MessageBox.Show("No. Rekening sudah ada..");
                this.ActiveControl = txttblBank_No_Rek;
                vBol = false;
            }

            if (vBol && txttblBank_Atas_Nama.Text.Trim() == "")
            {
                MessageBox.Show("Atas Nama harus diisi..");
                this.ActiveControl = txttblBank_Atas_Nama;
                vBol = false;
            }

            if (vBol && txttblBank_Cabang.Text.Trim() == "")
            {
                MessageBox.Show("Cabang harus diisi..");
                this.ActiveControl = txttblBank_Cabang;
                vBol = false;
            }

            return vBol;
        }

        private void AmbilKode()
        {
            string strSql;
            strSql = "SELECT TOP 1 RIGHT('00000' + CAST((RIGHT(tblBank_Kode_Bank,5)+1) AS NVARCHAR(5)),5) AS Kode_Bank ";
            strSql += "FROM tblBank ORDER BY tblBank_Kode_Bank DESC";
            using (SqlCommand cmdCounter = new SqlCommand(strSql, ConnMaster))
            {
                drtblCounter = cmdCounter.ExecuteReader();
                if (drtblCounter.HasRows)
                {
                    while (drtblCounter.Read())
                    {
                        txttblBank_Kode_Bank.Text = drtblCounter["Kode_Bank"].ToString();
                    }
                }
                else
                {
                    txttblBank_Kode_Bank.Text = "00001";
                }
                drtblCounter.Close();
            }
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
                            AmbilKode();
                            strSql = "INSERT INTO tblBank(";
                            strSql += "tblBank_Kode_Bank,tblBank_Group,tblBank_No_Rek,tblBank_Atas_Nama,tblBank_Cabang,";
                            strSql += "tblBank_Alamat1,tblBank_Alamat2,tblBank_Provinsi,tblBank_Kota_Kabupaten,tblBank_Kecamatan,";
                            strSql += "tblBank_Desa_Kelurahan,tblBank_Kode_Pos,tblBank_Ket,tblBank_UInput) ";
                            strSql += "VALUES(@Kode_Bank,";
                            strSql += "@Group,@No_Rek,@Atas_Nama,@Cabang,";
                            strSql += "@Alamat1,@Alamat2,@Provinsi,@Kota_Kabupaten,@Kecamatan,";
                            strSql += "@Desa_Kelurahan,@Kode_Pos,@Ket,@UInput)";
                            using (SqlCommand cmdtblBank = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblBank.Parameters.AddWithValue("@Kode_Bank", txttblBank_Kode_Bank.Text);
                                cmdtblBank.Parameters.AddWithValue("@Group", cbxtblBank_Group.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@No_Rek", txttblBank_No_Rek.Text);
                                cmdtblBank.Parameters.AddWithValue("@Atas_Nama", txttblBank_Atas_Nama.Text);
                                cmdtblBank.Parameters.AddWithValue("@Cabang", txttblBank_Cabang.Text);
                                cmdtblBank.Parameters.AddWithValue("@Alamat1", txttblBank_Alamat1.Text);
                                cmdtblBank.Parameters.AddWithValue("@Alamat2", txttblBank_Alamat2.Text);
                                cmdtblBank.Parameters.AddWithValue("@Ket", txttblBank_Ket.Text);
                                cmdtblBank.Parameters.AddWithValue("@Provinsi", cbxtblBank_Provinsi.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@Kota_Kabupaten", cbxtblBank_Kota_Kabupaten.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@Kecamatan", cbxtblBank_Kecamatan.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@Desa_Kelurahan", cbxtblBank_Desa_Kelurahan.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@Kode_Pos", cbxtblBank_Kode_Pos.SelectedItem.ToString());                                
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
                    txttblBank_Kode_Bank.Text = "";
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
                            strSql = "UPDATE tblBank SET ";
                            strSql += "tblBank_Group=@Group,";
                            strSql += "tblBank_No_Rek=@No_Rek,";
                            strSql += "tblBank_Atas_Nama=@Atas_Nama,";
                            strSql += "tblBank_Cabang=@Cabang,";
                            strSql += "tblBank_Alamat1=@Alamat1,";
                            strSql += "tblBank_Alamat2=@Alamat2,";
                            strSql += "tblBank_Ket=@Ket,";
                            strSql += "tblBank_Provinsi=@Provinsi,";
                            strSql += "tblBank_Kota_Kabupaten=@Kota_Kabupaten,";
                            strSql += "tblBank_Kecamatan=@Kecamatan,";
                            strSql += "tblBank_Desa_Kelurahan=@Desa_Kelurahan,";
                            strSql += "tblBank_Kode_Pos=@Kode_Pos,";
                            strSql += "tblBank_UEdit=@UEdit,";
                            strSql += "tblBank_UDate_Edit=GETDATE() ";
                            strSql += "WHERE tblBank_Kode_Bank=@Kode_Bank";
                            using (SqlCommand cmdtblBank = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblBank.Parameters.AddWithValue("@Kode_Bank", txttblBank_Kode_Bank.Text);
                                cmdtblBank.Parameters.AddWithValue("@Group", cbxtblBank_Group.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@No_Rek", txttblBank_No_Rek.Text);
                                cmdtblBank.Parameters.AddWithValue("@Atas_Nama", txttblBank_Atas_Nama.Text);
                                cmdtblBank.Parameters.AddWithValue("@Cabang", txttblBank_Cabang.Text);
                                cmdtblBank.Parameters.AddWithValue("@Alamat1", txttblBank_Alamat1.Text);
                                cmdtblBank.Parameters.AddWithValue("@Alamat2", txttblBank_Alamat2.Text);
                                cmdtblBank.Parameters.AddWithValue("@Ket", txttblBank_Ket.Text);
                                cmdtblBank.Parameters.AddWithValue("@Provinsi", cbxtblBank_Provinsi.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@Kota_Kabupaten", cbxtblBank_Kota_Kabupaten.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@Kecamatan", cbxtblBank_Kecamatan.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@Desa_Kelurahan", cbxtblBank_Desa_Kelurahan.SelectedItem.ToString());
                                cmdtblBank.Parameters.AddWithValue("@Kode_Pos", cbxtblBank_Kode_Pos.SelectedItem.ToString());
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

        private Boolean CekBank()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblBank WHERE tblBank_Kode_Bank=@Kode_Bank";
                using (SqlCommand cmdtblBank = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblBank.Parameters.AddWithValue("@Kode_Bank", txttblBank_Kode_Bank.Text);
                    drtblBank = cmdtblBank.ExecuteReader();
                    if (drtblBank.HasRows)
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
                drtblBank.Close();
                ConnMaster.Close();
            }

            return vBol;
        }

        private Boolean CekNoRek()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblBank WHERE tblBank_No_Rek=@No_Rek";
                using (SqlCommand cmdtblBank = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblBank.Parameters.AddWithValue("@No_Rek", txttblBank_No_Rek.Text);
                    drtblBank = cmdtblBank.ExecuteReader();
                    if (drtblBank.HasRows)
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
                drtblBank.Close();
                ConnMaster.Close();
            }

            return vBol;
        }
        /*
        private Boolean Used()
        {
            Boolean vBol = true;
            try
            {
                ConnMaster = ConnectionManager.GetConnection();
                strSql = "SELECT * FROM tblCustomer WHERE tblCustomer_Kode_Bank=@Kode_Bank";
                using (SqlCommand cmdtblBank = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblBank.Parameters.AddWithValue("@Kode_Bank", txttblBank_Kode_Bank.Text);
                    drtblBank = cmdtblBank.ExecuteReader();
                    if (drtblBank.HasRows)
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
                drtblBank.Close();
                ConnMaster.Close();
            }
            return vBol;
        }*/

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

            if (!CekBank())
            {
                MessageBox.Show("Kode Bank " + txttblBank_Kode_Bank.Text + " tidak ditemukan..");
                return;
            }
            /*
            if (Used())
            {
                MessageBox.Show("Kode Bank " + txttblBank_Kode_Bank.Text + " sudah pernah digunakan..");
                return;
            }*/


            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kode Bank " + txttblBank_Kode_Bank.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "DELETE FROM tblBank WHERE tblBank_Kode_Bank=@Kode_Bank";
                            using (SqlCommand cmdtblBank_Kode_Bank = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdtblBank_Kode_Bank.Parameters.AddWithValue("@Kode_Bank", txttblBank_Kode_Bank.Text);
                                RowAffected = cmdtblBank_Kode_Bank.ExecuteNonQuery();
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
                    MessageBox.Show("Kode Bank " + txttblBank_Kode_Bank.Text + ", berhasil dihapus..");
                }
                this.Close();
            }
        }

        private void cbxtblBank_Provinsi_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxtblBank_Kecamatan.Enabled = false;
            cbxtblBank_Kecamatan.SelectedItem = "";
            cbxtblBank_Desa_Kelurahan.Enabled = false;
            cbxtblBank_Desa_Kelurahan.SelectedItem = "";
            cbxtblBank_Kode_Pos.Enabled = false;
            cbxtblBank_Kode_Pos.SelectedItem = "";

            if (cbxtblBank_Provinsi.SelectedItem.ToString() != "")
            {
                FillcbxtblBank_Kota_Kabupaten();
                cbxtblBank_Kota_Kabupaten.Enabled = true;
                cbxtblBank_Kota_Kabupaten.SelectedItem = "";            
            }
        }

        private void cbxtblBank_Kota_Kabupaten_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxtblBank_Desa_Kelurahan.Enabled = false;
            cbxtblBank_Desa_Kelurahan.SelectedItem = "";
            cbxtblBank_Kode_Pos.Enabled = false;
            cbxtblBank_Kode_Pos.SelectedItem = "";

            if (cbxtblBank_Kota_Kabupaten.SelectedItem.ToString() != "")
            {
                FillcbxtblBank_Kecamatan();
                cbxtblBank_Kecamatan.Enabled = true;
                cbxtblBank_Kecamatan.SelectedItem = "";
            }
        }

        private void cbxtblBank_Kecamatan_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxtblBank_Kode_Pos.Enabled = false;
            cbxtblBank_Kode_Pos.SelectedItem = "";

            if (cbxtblBank_Kecamatan.SelectedItem.ToString() != "")
            {
                FillcbxtblBank_Desa_Kelurahan();
                cbxtblBank_Desa_Kelurahan.Enabled = true;
                cbxtblBank_Desa_Kelurahan.SelectedItem = "";
            }
        }

        private void cbxtblBank_Desa_Kelurahan_SelectedIndexChanged(object sender, EventArgs e)
        {    
            if (cbxtblBank_Desa_Kelurahan.SelectedItem.ToString() != "")
            {
                FillcbxtblBank_Kode_Pos();
                cbxtblBank_Kode_Pos.Enabled = true;
                cbxtblBank_Kode_Pos.SelectedItem = "";
            }
        }

        private void cbxtblBank_Kode_Pos_SelectedIndexChanged(object sender, EventArgs e)
        {
            //FillcbxtblBank_Kode_Pos();
        }

        private void cbxtblBank_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxtblBank_Group.SelectedItem.ToString() != "")
            {
                AmbilNamaBank();
            }
        }

        private void AmbilNamaBank()
        {
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblBank_Group WHERE tblBank_Group_Group=@Group";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Group",cbxtblBank_Group.SelectedItem.ToString());
                    drtblBank_Group = Cmd.ExecuteReader();
                    if (drtblBank_Group.HasRows)
                    {
                        while (drtblBank_Group.Read())
                        {
                            txttblBank_Group_Nama.Text = Convert.IsDBNull(drtblBank_Group["tblBank_Group_Nama"]) ? "" : (string)drtblBank_Group["tblBank_Group_Nama"];
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
                drtblBank_Group.Close();
                ConnMaster.Close();
            }
        }

        private void btnNew_Ct_Click(object sender, EventArgs e)
        {
            if (txttblBank_Kode_Bank.Text.Trim() == "")
            {
                MessageBox.Show("Save Kode Bank terlebih dahulu..");
                return;
            }

            Form Frm_Contact = new PopUp.Contact.Frm_Contact(true, 0, txttblBank_Kode_Bank.Text.Trim(), "tblBank");
            Frm_Contact.Text = "Contact " + txttblBank_Kode_Bank.Text + " - " + txttblBank_Group_Nama.Text;
            Frm_Contact.ShowDialog();
            IsiDtGridView_Contact();
        }

        private void btnEdit_Ct_Click(object sender, EventArgs e)
        {
            if (DtGridView_Contact.Rows.Count > 0)
            {
                var _RecId = (decimal)DtGridView_Contact.CurrentRow.Cells["RecId"].Value;
                Form Frm_Contact = new PopUp.Contact.Frm_Contact(false, _RecId, txttblBank_Kode_Bank.Text, "tblBank");
                Frm_Contact.ShowDialog();
                Frm_Contact.Text = "Contact " + txttblBank_Kode_Bank.Text + " - " + txttblBank_Group_Nama.Text;
                IsiDtGridView_Contact();
            }
            else
            {
                MessageBox.Show("Tidak ada data untuk diedit");
            }
        }

        private void btnDelete_Ct_Click(object sender, EventArgs e)
        {
            if (DtGridView_Contact.Rows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Contact " + DtGridView_Contact.CurrentRow.Cells["Nama"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            ConnMaster = ConnectionString.GetConnection();
                            using (ConnMaster)
                            {
                                string strSql;
                                strSql = "DELETE FROM tblContact WHERE tblContact_RecId='" + DtGridView_Contact.CurrentRow.Cells["RecId"].Value.ToString() + "'";
                                using (SqlCommand cmdContact = new SqlCommand(strSql, ConnMaster))
                                {
                                    cmdContact.ExecuteNonQuery();
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
                        ConnMaster.Close();
                    }

                    MessageBox.Show("Berhasil Delete data..");
                    IsiDtGridView_Contact();
                }
            }
            else
            {
                MessageBox.Show("Tidak ada data untuk dihapus..");
            }
        }

        private void FrmM_Bank_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            } 
        }
    }
}
