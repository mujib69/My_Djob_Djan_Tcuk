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
using System.IO;

namespace ISBS_New.Master.Vendor
{
    public partial class FrmM_Vendor : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConMaster;
        SqlDataReader drVendor, drCounter, drGol_Prsh, drSales, drToP, drMata_Uang, drPayment_Method;
        string strSql = "";
        string vOldtxtNama_Prsh, vOldtxtGol_Prsh, vOldtxtStatus;
        string vOldtxtNPWP, vOldtxtTaxName, vOldtxtTaxAddress, vOldtxtPKP, vOldtxtSIUP, vOldtxtTermOfPayment, vOldtxtMata_Uang, vOldtxtPayment_Method, vOldPool;
        decimal vOldtxtLimit_Total, vOldtxtLimit_Per_PO, vOldtxtSisa_Limit_Total, vOldtxtDeposito,  vOldtxtPPN, vOldtxtPPH;
        decimal vOldtxtDownpayment, vOldtxtCashback_Balance, vOldtxtDiscount_Balance, vOldtxtDebit_Note_Balance, vOldtxtBonus_Balance;
        DataTable namesTable_Address = new DataTable("tmpAddress");
        DataTable namesTable_Contact = new DataTable("tmpContact");
        DataTable namesTable_Rekening = new DataTable("tmpRekening");

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        string Mode, Status, Query, crit = null;
        List<string> sSelectedFile, FileName, Extension;
        List<byte[]> test = new List<byte[]>();
        FrmL_Vendor _owner;

        public FrmM_Vendor(FrmL_Vendor owner, string _kode)
        {
            InitializeComponent();
            txtKode_Prsh.Text = _kode;
            MulaiDariAwal();
            _owner = owner;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmL_Vendor_FormClosing);
        }

        private void FrmL_Vendor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _owner.loadmaster();
        }

        //Address Datatable
        private void Create_tmpAddress()
        {
            DataColumn idColumn00 = new DataColumn();
            DataColumn idColumn01 = new DataColumn();
            DataColumn idColumn02 = new DataColumn();
            DataColumn idColumn03 = new DataColumn();
            DataColumn idColumn04 = new DataColumn();
            DataColumn idColumn05 = new DataColumn();
            DataColumn idColumn06 = new DataColumn();
            DataColumn idColumn07 = new DataColumn();
            DataColumn idColumn08 = new DataColumn();
            DataColumn idColumn09 = new DataColumn();

            idColumn00.DataType = System.Type.GetType("System.String");
            idColumn00.ColumnName = "Name";
            idColumn01.DataType = System.Type.GetType("System.String");
            idColumn01.ColumnName = "Purpose";
            idColumn02.DataType = System.Type.GetType("System.Boolean");
            idColumn02.ColumnName = "Primary";
            idColumn03.DataType = System.Type.GetType("System.String");
            idColumn03.ColumnName = "Address";
            idColumn04.DataType = System.Type.GetType("System.String");
            idColumn04.ColumnName = "Provinsi";
            idColumn05.DataType = System.Type.GetType("System.String");
            idColumn05.ColumnName = "Kota";
            idColumn06.DataType = System.Type.GetType("System.String");
            idColumn06.ColumnName = "Kode Pos";
            idColumn07.DataType = System.Type.GetType("System.String");
            idColumn07.ColumnName = "RT";
            idColumn08.DataType = System.Type.GetType("System.String");
            idColumn08.ColumnName = "RW";
            idColumn09.DataType = System.Type.GetType("System.Decimal");
            idColumn09.ColumnName = "RecId";

            namesTable_Address.Columns.Add(idColumn00);
            namesTable_Address.Columns.Add(idColumn01);
            namesTable_Address.Columns.Add(idColumn02);
            namesTable_Address.Columns.Add(idColumn03);
            namesTable_Address.Columns.Add(idColumn04);
            namesTable_Address.Columns.Add(idColumn05);
            namesTable_Address.Columns.Add(idColumn06);
            namesTable_Address.Columns.Add(idColumn07);
            namesTable_Address.Columns.Add(idColumn08);
            namesTable_Address.Columns.Add(idColumn09);
        }

        private void BuatDtGridView_Address()
        {
            namesTable_Address.Clear();
            DtGridView_Address.DataSource = namesTable_Address;

            namesTable_Address.Columns[0].ColumnName = "Name";
            namesTable_Address.Columns[1].ColumnName = "Purpose";
            namesTable_Address.Columns[2].ColumnName = "Primary";
            namesTable_Address.Columns[3].ColumnName = "Address";
            namesTable_Address.Columns[4].ColumnName = "Provinsi";
            namesTable_Address.Columns[5].ColumnName = "Kota";
            namesTable_Address.Columns[6].ColumnName = "Kode Pos";
            namesTable_Address.Columns[7].ColumnName = "RT";
            namesTable_Address.Columns[8].ColumnName = "RW";
            namesTable_Address.Columns[9].ColumnName = "RecId";

            DtGridView_Address.Columns[0].Width = 100;
            DtGridView_Address.Columns[1].Width = 100;
            DtGridView_Address.Columns[2].Width = 100;
            DtGridView_Address.Columns[3].Width = 100;
            DtGridView_Address.Columns[4].Width = 100;
            DtGridView_Address.Columns[5].Width = 100;
            DtGridView_Address.Columns[6].Width = 100;
            DtGridView_Address.Columns[7].Width = 100;
            DtGridView_Address.Columns[8].Width = 100;
            DtGridView_Address.Columns[9].Width = 50;

            DtGridView_Address.Columns[9].Visible = false;
            DtGridView_Address.Refresh();
        }

        private void IsiDtGridView_Address()
        {
            strSql = "SELECT Name,Address,PurposeType,PrimaryC,RecID,Provinsi,Kota,Kode_Pos,RT,RW,RecId ";
            strSql += "FROM Address ";
            strSql += "WHERE ReffTableName='VendTable' ";
            strSql += "AND ReffID='" + txtKode_Prsh.Text + "'";
            namesTable_Address.Clear();
            ConMaster = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
            {
                drVendor = cmd.ExecuteReader();
                while (drVendor.Read())
                {
                    DataRow row;
                    row = namesTable_Address.NewRow();
                    row["Name"] = Convert.IsDBNull(drVendor["Name"]) ? "" : (string)drVendor["Name"];
                    row["Address"] = Convert.IsDBNull(drVendor["Address"]) ? "" : (string)drVendor["Address"];
                    row["Purpose"] = Convert.IsDBNull(drVendor["PurposeType"]) ? "" : (string)drVendor["PurposeType"];
                    row["Primary"] = Convert.IsDBNull(drVendor["PrimaryC"]) ? false : (Boolean)drVendor["PrimaryC"];
                    row["Provinsi"] = Convert.IsDBNull(drVendor["Provinsi"]) ? "" : (string)drVendor["Provinsi"];
                    row["Kota"] = Convert.IsDBNull(drVendor["Kota"]) ? "" : (string)drVendor["Kota"];
                    row["Kode Pos"] = Convert.IsDBNull(drVendor["Kode_Pos"]) ? "" : (string)drVendor["Kode_Pos"];
                    row["RT"] = Convert.IsDBNull(drVendor["RT"]) ? "" : (string)drVendor["RT"];
                    row["RW"] = Convert.IsDBNull(drVendor["RW"]) ? "" : (string)drVendor["RW"];
                    row["RecId"] = Convert.IsDBNull(drVendor["RecId"]) ? 0 : Convert.ToInt32(drVendor["RecId"]);
                    namesTable_Address.Rows.Add(row);
                }
            }
            drVendor.Close();
            ConMaster.Close();
            DtGridView_Address.Refresh();
        }
        //Address Datatable End

        //Contact Datatable
        private void Create_tmpContact()
        {
            DataColumn idColumn00 = new DataColumn();
            DataColumn idColumn01 = new DataColumn();
            DataColumn idColumn02 = new DataColumn();
            DataColumn idColumn03 = new DataColumn();
            DataColumn idColumn04 = new DataColumn();
            DataColumn idColumn05 = new DataColumn();

            idColumn00.DataType = System.Type.GetType("System.String");
            idColumn00.ColumnName = "Deskripsi";
            idColumn01.DataType = System.Type.GetType("System.Boolean");
            idColumn01.ColumnName = "Primary";
            idColumn02.DataType = System.Type.GetType("System.String");
            idColumn02.ColumnName = "Type";
            idColumn03.DataType = System.Type.GetType("System.String");
            idColumn03.ColumnName = "Contact";
            idColumn04.DataType = System.Type.GetType("System.String");
            idColumn04.ColumnName = "Ext. No";
            idColumn05.DataType = System.Type.GetType("System.Decimal");
            idColumn05.ColumnName = "RecId";


            namesTable_Contact.Columns.Add(idColumn00);
            namesTable_Contact.Columns.Add(idColumn01);
            namesTable_Contact.Columns.Add(idColumn02);
            namesTable_Contact.Columns.Add(idColumn03);
            namesTable_Contact.Columns.Add(idColumn04);
            namesTable_Contact.Columns.Add(idColumn05);
        }

        private void BuatDtGridView_Contact()
        {
            namesTable_Contact.Clear();
            DtGridView_Contact.DataSource = namesTable_Contact;

            namesTable_Contact.Columns[0].ColumnName = "Deskripsi";
            namesTable_Contact.Columns[1].ColumnName = "Primary";
            namesTable_Contact.Columns[2].ColumnName = "Type";
            namesTable_Contact.Columns[3].ColumnName = "Contact";
            namesTable_Contact.Columns[4].ColumnName = "Ext. No";
            namesTable_Contact.Columns[5].ColumnName = "RecId";

            DtGridView_Contact.Columns[0].Width = 100;
            DtGridView_Contact.Columns[1].Width = 100;
            DtGridView_Contact.Columns[2].Width = 100;
            DtGridView_Contact.Columns[3].Width = 100;
            DtGridView_Contact.Columns[4].Width = 100;
            DtGridView_Contact.Columns[5].Width = 100;

            DtGridView_Contact.Columns[5].Visible = false;
            DtGridView_Contact.Refresh();
        }

        private void IsiDtGridView_Contact()
        {
            strSql = "SELECT * ";
            strSql += "FROM Contact ";
            strSql += "WHERE ReffTableName='VendTable' ";
            strSql += "AND ReffID='" + txtKode_Prsh.Text + "'";
            namesTable_Contact.Clear();
            ConMaster = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
            {
                drVendor = cmd.ExecuteReader();
                while (drVendor.Read())
                {
                    DataRow row;
                    row = namesTable_Contact.NewRow();
                    row["Deskripsi"] = Convert.IsDBNull(drVendor["Deskripsi"]) ? "" : (string)drVendor["Deskripsi"];
                    row["Type"] = Convert.IsDBNull(drVendor["ContactType"]) ? "" : (string)drVendor["ContactType"];
                    row["Contact"] = Convert.IsDBNull(drVendor["ContactNo"]) ? "" : (string)drVendor["ContactNo"];
                    row["Ext. No"] = Convert.IsDBNull(drVendor["ExtNo"]) ? "" : (string)drVendor["ExtNo"];
                    row["Primary"] = Convert.IsDBNull(drVendor["PrimaryC"]) ? false : (Boolean)drVendor["PrimaryC"];
                    row["RecId"] = Convert.IsDBNull(drVendor["RecId"]) ? 0 : Convert.ToInt32(drVendor["RecId"]);
                    namesTable_Contact.Rows.Add(row);
                }
            }
            drVendor.Close();
            ConMaster.Close();
            DtGridView_Contact.Refresh();
        }
        //Contact Datatable End

        //Rekening Datatable
        private void Create_tmpRekening()
        {
            DataColumn idColumn00 = new DataColumn();
            DataColumn idColumn01 = new DataColumn();
            DataColumn idColumn02 = new DataColumn();
            DataColumn idColumn03 = new DataColumn();
            DataColumn idColumn04 = new DataColumn();
            DataColumn idColumn05 = new DataColumn();
            DataColumn idColumn06 = new DataColumn();

            idColumn00.DataType = System.Type.GetType("System.String");
            idColumn00.ColumnName = "No Rekening";
            idColumn01.DataType = System.Type.GetType("System.String");
            idColumn01.ColumnName = "Bank";
            idColumn02.DataType = System.Type.GetType("System.String");
            idColumn02.ColumnName = "Nama Bank";
            idColumn03.DataType = System.Type.GetType("System.String");
            idColumn03.ColumnName = "Cabang";
            idColumn04.DataType = System.Type.GetType("System.String");
            idColumn04.ColumnName = "Pemilik";
            idColumn05.DataType = System.Type.GetType("System.String");
            idColumn05.ColumnName = "Keterangan";
            idColumn06.DataType = System.Type.GetType("System.Boolean");
            idColumn06.ColumnName = "Aktif";

            namesTable_Rekening.Columns.Add(idColumn00);
            namesTable_Rekening.Columns.Add(idColumn01);
            namesTable_Rekening.Columns.Add(idColumn02);
            namesTable_Rekening.Columns.Add(idColumn03);
            namesTable_Rekening.Columns.Add(idColumn04);
            namesTable_Rekening.Columns.Add(idColumn05);
            namesTable_Rekening.Columns.Add(idColumn06);
        }

        private void BuatDtGridView_Rekening()
        {
            namesTable_Rekening.Clear();
            DtGridView_Rekening.DataSource = namesTable_Rekening;

            namesTable_Rekening.Columns[0].ColumnName = "No Rekening";
            namesTable_Rekening.Columns[1].ColumnName = "Bank";
            namesTable_Rekening.Columns[2].ColumnName = "Nama Bank";
            namesTable_Rekening.Columns[3].ColumnName = "Cabang";
            namesTable_Rekening.Columns[4].ColumnName = "Pemilik";
            namesTable_Rekening.Columns[5].ColumnName = "Keterangan";
            namesTable_Rekening.Columns[6].ColumnName = "Aktif";

            DtGridView_Rekening.Columns[0].Width = 100;
            DtGridView_Rekening.Columns[1].Width = 100;
            DtGridView_Rekening.Columns[2].Width = 100;
            DtGridView_Rekening.Columns[3].Width = 100;
            DtGridView_Rekening.Columns[4].Width = 100;
            DtGridView_Rekening.Columns[5].Width = 100;
            DtGridView_Rekening.Columns[6].Width = 100;

            DtGridView_Rekening.Refresh();
        }

        private void IsiDtGridView_Rekening()
        {
            strSql = "SELECT * ";
            strSql += "FROM Rekening ";
            strSql += "WHERE ReffTableName='VendTable' ";
            strSql += "AND ReffID='" + txtKode_Prsh.Text + "'";
            namesTable_Rekening.Clear();
            ConMaster = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
            {
                drVendor = cmd.ExecuteReader();
                while (drVendor.Read())
                {
                    DataRow row;
                    row = namesTable_Rekening.NewRow();
                    row["No Rekening"] = Convert.IsDBNull(drVendor["No_Rekening"]) ? "" : (string)drVendor["No_Rekening"];
                    row["Bank"] = Convert.IsDBNull(drVendor["BankGroupId"]) ? "" : (string)drVendor["BankGroupId"];
                    row["Nama Bank"] = Convert.IsDBNull(drVendor["BankGroupName"]) ? "" : (string)drVendor["BankGroupName"];
                    row["Cabang"] = Convert.IsDBNull(drVendor["Cabang"]) ? "" : (string)drVendor["Cabang"];
                    row["Pemilik"] = Convert.IsDBNull(drVendor["Pemilik"]) ? "" : (string)drVendor["Pemilik"];
                    row["Keterangan"] = Convert.IsDBNull(drVendor["Keterangan"]) ? "" : (string)drVendor["Keterangan"];
                    row["Aktif"] = Convert.IsDBNull(drVendor["Aktif"]) ? false : (Boolean)drVendor["Aktif"];
                    namesTable_Rekening.Rows.Add(row);
                }
            }
            drVendor.Close();
            ConMaster.Close();
            DtGridView_Rekening.Refresh();
        }
        //Rekening Datatable End

        private void FrmM_Vendor_Load(object sender, EventArgs e)
        {
            Create_tmpAddress();
            DtGridView_Address.DataSource = "";
            Create_tmpContact();
            DtGridView_Contact.DataSource = "";
            Create_tmpRekening();
            DtGridView_Rekening.DataSource = "";
            txtFieldPool.ReadOnly = true;

            Boolean BolFound = false;

            try
            {
                ConMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM VendTable ";
                strSql += "WHERE VendId='" + txtKode_Prsh.Text + "'";
                using (SqlCommand cmdVendor = new SqlCommand(strSql, ConMaster))
                {
                    drVendor = cmdVendor.ExecuteReader();
                    if (drVendor.HasRows)
                    {
                        BolFound = true;
                        while (drVendor.Read())
                        {
                            txtNama_Prsh.Text = Convert.IsDBNull(drVendor["VendName"]) ? "" : (string)drVendor["VendName"];
                            txtGol_Prsh.Text = Convert.IsDBNull(drVendor["Gol_Prsh"]) ? "" : (string)drVendor["Gol_Prsh"];
                            txtStatus.Text = Convert.IsDBNull(drVendor["Status"]) ? "" : (string)drVendor["Status"];
                            txtNPWP.Text = Convert.IsDBNull(drVendor["NPWP"]) ? "" : (string)drVendor["NPWP"];
                            txtTaxName.Text = Convert.IsDBNull(drVendor["TaxName"]) ? "" : (string)drVendor["TaxName"];
                            txtTaxAddress.Text = Convert.IsDBNull(drVendor["TaxAddress"]) ? "" : (string)drVendor["TaxAddress"];
                            txtPKP.Text = Convert.IsDBNull(drVendor["PKP"]) ? "" : (string)drVendor["PKP"];
                            txtSIUP.Text = Convert.IsDBNull(drVendor["SIUP"]) ? "" : (string)drVendor["SIUP"];
                            txtTermOfPayment.Text = Convert.IsDBNull(drVendor["TermOfPayment"]) ? "" : (string)drVendor["TermOfPayment"];
                            txtFieldPool.Text = Convert.IsDBNull(drVendor["POOL"]) ? "" : (string)drVendor["POOL"];

                           
                            txtPPN.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["PPN"]) ? 0 : (decimal)drVendor["PPN"]);
                            txtPPH.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["PPH"]) ? 0 : (decimal)drVendor["PPH"]);
                            txtMata_Uang.Text = Convert.IsDBNull(drVendor["CurrencyId"]) ? "" : (string)drVendor["CurrencyId"];
                            txtPayment_Method.Text = Convert.IsDBNull(drVendor["PaymentModeId"]) ? "" : (string)drVendor["PaymentModeId"];

                            txtLimit_Total.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Limit_Total"]) ? 0 : (decimal)drVendor["Limit_Total"]);
                            txtLimit_Per_PO.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Limit_Per_PO"]) ? 0 : (decimal)drVendor["Limit_Per_PO"]);
                            txtSisa_Limit_Total.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Sisa_Limit_Total"]) ? 0 : (decimal)drVendor["Sisa_Limit_Total"]);
                            txtDeposito.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Deposito"]) ? 0 : (decimal)drVendor["Deposito"]);
                            txtDownpayment.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Downpayment"]) ? 0 : (decimal)drVendor["Downpayment"]);
                            txtCashback_Balance.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Cashback_Balance"]) ? 0 : (decimal)drVendor["Cashback_Balance"]);
                            txtDiscount_Balance.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Discount_Balance"]) ? 0 : (decimal)drVendor["Discount_Balance"]);
                            txtDebit_Note_Balance.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Debit_Note_Balance"]) ? 0 : (decimal)drVendor["Debit_Note_Balance"]);
                            txtBonus_Balance.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drVendor["Bonus_Balance"]) ? 0 : (decimal)drVendor["Bonus_Balance"]);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data Vendor " + txtKode_Prsh.Text + " tidak ditemukan...");
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
                drVendor.Close();
                ConMaster.Close();
            }

            if (BolFound)
            {
                Btn_EditCancelSaveDel(true, false, false, true);
                BuatDtGridView_Address();
                IsiDtGridView_Address();
                BuatDtGridView_Contact();
                IsiDtGridView_Contact();
                BuatDtGridView_Rekening();
                IsiDtGridView_Rekening();
            }

            //steven  dgvAttachment
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;

            dgvAttachment.Rows.Clear();
            if (dgvAttachment.RowCount - 1 <= 0)
            {
                dgvAttachment.ColumnCount = 5;
                dgvAttachment.Columns[0].Name = "FileName";
                dgvAttachment.Columns[1].Name = "ContentType";
                dgvAttachment.Columns[2].Name = "File Size (kb)";
                dgvAttachment.Columns[3].Name = "Attachment";
                dgvAttachment.Columns[4].Name = "Id";

                dgvAttachment.Columns["Attachment"].Visible = false;
                dgvAttachment.Columns["Id"].Visible = false;
            }
            Conn = ConnectionString.GetConnection();
            Query = "Select * From [tblAttachments] Where ReffTableName = 'CustTable' And ReffTransId = '" + txtKode_Prsh.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                test.Add((byte[])Dr["Attachment"]);
            }

            dgvAttachment.AutoResizeColumns();
            //steven  dgvAttachment
        }

        private void MulaiDariAwal()
        {
            this.TabCtrl_Vendor.SelectedTab = TabPage_General;
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            EmptyTextBox();
            ButtonSearch(false);
            Chk_Enable(false);
            Btn_Addr_Cont(false);
            Btn_EditCancelSaveDel(false, false, false, false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txtKode_Prsh.ReadOnly = true;
            txtNama_Prsh.ReadOnly = vbol;
            txtGol_Prsh.ReadOnly = vbol;
            txtStatus.ReadOnly = true;
            txtNPWP.ReadOnly = vbol;
            txtTaxName.ReadOnly = vbol;
            txtTaxAddress.ReadOnly = vbol;
            txtPKP.ReadOnly = vbol;
            txtSIUP.ReadOnly = vbol;
            txtTermOfPayment.ReadOnly = vbol;
            txtPPN.ReadOnly = vbol;
            txtPPH.ReadOnly = vbol;
            txtMata_Uang.ReadOnly = vbol;
            txtPayment_Method.ReadOnly = vbol;
            txtLimit_Total.ReadOnly = vbol;
            txtLimit_Per_PO.ReadOnly = vbol;
            txtFieldPool.ReadOnly = vbol;
            txtSisa_Limit_Total.ReadOnly = true;
            txtDeposito.ReadOnly = true;
            txtDownpayment.ReadOnly = true;
            txtCashback_Balance.ReadOnly = true;
            txtDiscount_Balance.ReadOnly = true;
            txtDebit_Note_Balance.ReadOnly = true;
            txtBonus_Balance.ReadOnly = true;
         }

        private void Chk_Enable(bool vbol)
        {
            
        }

        private void EmptyTextBox()
        {
            //txtKode_Prsh.Text="";      
            txtNama_Prsh.Text = "";
            txtGol_Prsh.Text = "";
            txtStatus.Text = "";
            txtNPWP.Text = "";
            txtTaxName.Text = "";
            txtTaxAddress.Text = "";
            txtPKP.Text = "";
            txtSIUP.Text = "";
            txtTermOfPayment.Text = "";
            txtFieldPool.Text = "";
            txtPPN.Text = string.Format("{0:#,##0.00}", 0);
            txtPPH.Text = string.Format("{0:#,##0.00}", 0);
            txtMata_Uang.Text = "";
            txtPayment_Method.Text = "";
            txtLimit_Total.Text = string.Format("{0:#,##0.00}", 0);
            txtLimit_Per_PO.Text = string.Format("{0:#,##0.00}", 0);
            txtSisa_Limit_Total.Text = string.Format("{0:#,##0.00}", 0);
            txtDeposito.Text = string.Format("{0:#,##0.00}", 0);
            txtDownpayment.Text = string.Format("{0:#,##0.00}", 0);
            txtCashback_Balance.Text = string.Format("{0:#,##0.00}", 0);
            txtDiscount_Balance.Text = string.Format("{0:#,##0.00}", 0);
            txtDebit_Note_Balance.Text = string.Format("{0:#,##0.00}", 0);
            txtBonus_Balance.Text = string.Format("{0:#,##0.00}", 0);
        }

        private void ButtonSearch(bool vbol)
        {
            btntxtGol_Prsh.Enabled = vbol;
            btnBrowse.Enabled = vbol;
            btntxtTermOfPayment.Enabled = vbol;
            btntxtMata_Uang.Enabled = vbol;
            btntxtPayment_Method.Enabled = vbol;
        }

        private void Btn_Addr_Cont(bool vBol)
        {
            BtnNew_Addr.Enabled = vBol;
            BtnNew_Contact.Enabled = vBol;
            BtnEdit_Addr.Enabled = vBol;
            BtnEdit_Contact.Enabled = vBol;
            BtnDelete_Addr.Enabled = vBol;
            BtnDelete_Contact.Enabled = vBol;
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
            //this.TabCtrl_Vendor.SelectedTab = TabPage_General;
            //this.ActiveControl = txtNama_Prsh;
            txt_ReadOnly(false);
            Chk_Enable(true);
            ButtonSearch(true);
            Btn_Addr_Cont(true);
            Btn_EditCancelSaveDel(false, true, true, false);

            vOldtxtNama_Prsh = txtNama_Prsh.Text;
            vOldtxtGol_Prsh = txtGol_Prsh.Text;
            vOldtxtStatus = txtStatus.Text;
            vOldtxtNPWP = txtNPWP.Text;
            vOldtxtTaxName = txtTaxName.Text;
            vOldtxtTaxAddress = txtTaxAddress.Text;
            vOldtxtPKP = txtPKP.Text;
            vOldtxtSIUP = txtSIUP.Text;
            vOldtxtTermOfPayment = txtTermOfPayment.Text;
            vOldtxtMata_Uang = txtMata_Uang.Text;
            vOldtxtPayment_Method = txtPayment_Method.Text;
            vOldPool = txtFieldPool.Text;

            vOldtxtLimit_Total = Convert.ToDecimal(txtLimit_Total.Text);
            vOldtxtLimit_Per_PO = Convert.ToDecimal(txtLimit_Per_PO.Text);
            vOldtxtSisa_Limit_Total = Convert.ToDecimal(txtSisa_Limit_Total.Text);
            vOldtxtDeposito = Convert.ToDecimal(txtDeposito.Text);
            vOldtxtPPN = Convert.ToDecimal(txtPPN.Text);
            vOldtxtPPH = Convert.ToDecimal(txtPPH.Text);
            vOldtxtDownpayment = Convert.ToDecimal(txtDownpayment.Text);
            vOldtxtCashback_Balance = Convert.ToDecimal(txtCashback_Balance.Text);
            vOldtxtDiscount_Balance = Convert.ToDecimal(txtDiscount_Balance.Text);
            vOldtxtDebit_Note_Balance = Convert.ToDecimal(txtDebit_Note_Balance.Text);
            vOldtxtBonus_Balance = Convert.ToDecimal(txtBonus_Balance.Text);

            //STEVEN dgvAttachment
            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnDelAttachment.Enabled = true;

            dgvAttachment.Rows.Clear();
            if (dgvAttachment.RowCount - 1 <= 0)
            {
                dgvAttachment.ColumnCount = 5;
                dgvAttachment.Columns[0].Name = "FileName";
                dgvAttachment.Columns[1].Name = "ContentType";
                dgvAttachment.Columns[2].Name = "File Size (kb)";
                dgvAttachment.Columns[3].Name = "Attachment";
                dgvAttachment.Columns[4].Name = "Id";

                dgvAttachment.Columns["Attachment"].Visible = false;
                dgvAttachment.Columns["Id"].Visible = false;
            }

            Query = "Select * From [tblAttachments] Where ReffTableName = 'CustTable' And ReffTransId = '" + txtKode_Prsh.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                test.Add((byte[])Dr["Attachment"]);
            }
            //STEVEN dgvAttachment
            fieldPoolSetting();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Chk_Enable(false);
            ButtonSearch(false);
            Btn_Addr_Cont(false);
            Btn_EditCancelSaveDel(true, false, false, true);


            txtNama_Prsh.Text = vOldtxtNama_Prsh;
            txtGol_Prsh.Text = vOldtxtGol_Prsh;
            txtStatus.Text = vOldtxtStatus;
            txtNPWP.Text = vOldtxtNPWP;
            txtTaxName.Text = vOldtxtTaxName;
            txtTaxAddress.Text = vOldtxtTaxAddress;
            txtPKP.Text = vOldtxtPKP;
            txtSIUP.Text = vOldtxtSIUP;
            txtTermOfPayment.Text = vOldtxtTermOfPayment;
            txtMata_Uang.Text = vOldtxtMata_Uang;
            txtPayment_Method.Text = vOldtxtPayment_Method;
            txtFieldPool.Text = vOldPool;

            txtLimit_Total.Text = string.Format("{0:#,##0.00}", vOldtxtLimit_Total);
            txtLimit_Per_PO.Text = string.Format("{0:#,##0.00}", vOldtxtLimit_Per_PO);
            txtSisa_Limit_Total.Text = string.Format("{0:#,##0.00}", vOldtxtSisa_Limit_Total);
            txtDeposito.Text = string.Format("{0:#,##0.00}", vOldtxtDeposito);
            txtPPN.Text = string.Format("{0:#,##0.00}", vOldtxtPPN);
            txtPPH.Text = string.Format("{0:#,##0.00}", vOldtxtPPH);
            txtDownpayment.Text = string.Format("{0:#,##0.00}", vOldtxtDownpayment);
            txtCashback_Balance.Text = string.Format("{0:#,##0.00}", vOldtxtCashback_Balance);
            txtDiscount_Balance.Text = string.Format("{0:#,##0.00}", vOldtxtDiscount_Balance);
            txtDebit_Note_Balance.Text = string.Format("{0:#,##0.00}", vOldtxtDebit_Note_Balance);
            txtBonus_Balance.Text = string.Format("{0:#,##0.00}", vOldtxtBonus_Balance);

            //steven dgvAttachment
            dgvAttachment.ReadOnly = true;
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;

            this.ActiveControl = btnEdit;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kode Vendor " + txtKode_Prsh.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                            strSql = "DELETE FROM VendTable WHERE VendId='" + txtKode_Prsh.Text + "'";
                            using (SqlCommand cmdVendor = new SqlCommand(strSql, ConMaster))
                            {
                                cmdVendor.ExecuteNonQuery();
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

                var FrmL_Vendor = Application.OpenForms.OfType<Master.Vendor.FrmL_Vendor>().FirstOrDefault();
                if (FrmL_Vendor != null)
                {
                    FrmL_Vendor.Activate();
                }
                else
                {
                    new Master.Vendor.FrmL_Vendor().Show();
                }
                this.Close();
            } 
        }

        private void AmbilKodePerusahaan()
        {
            string strSql;
            strSql = "SELECT TOP 1 'V'+RIGHT('00000' + CAST((RIGHT(VendId,5)+1) AS NVARCHAR(5)),5) AS VendId ";
            strSql += "FROM VendTable ORDER BY VendId DESC";
            using (SqlCommand cmdCounter = new SqlCommand(strSql, ConMaster))
            {
                drCounter = cmdCounter.ExecuteReader();
                if (drCounter.HasRows)
                {
                    while (drCounter.Read())
                    {
                        txtKode_Prsh.Text = drCounter["VendId"].ToString();
                    }
                }
                else
                {
                    txtKode_Prsh.Text = "V00001";
                }
                drCounter.Close();
            }
        }

        private Boolean CekNamaVendor()
        {
            try
            {
                ConMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM VendTable WHERE VendName=@Nama and VendId<>@Kode_Prsh";
                using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
                {
                    cmd.Parameters.AddWithValue("@Nama", txtNama_Prsh.Text.Trim());
                    cmd.Parameters.AddWithValue("@Kode_Prsh", txtKode_Prsh.Text.Trim());
                    drVendor = cmd.ExecuteReader();
                    if (drVendor.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return true;
            }
            finally
            {
                drVendor.Close();
                ConMaster.Close();
            }
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txtNama_Prsh.Text == "")
            {
                MessageBox.Show("Nama Vendor harus diisi..");
                vBol = false;
            }

            if (CekNamaVendor())
            {
                MessageBox.Show("Nama Vendor sudah ada..");
                vBol = false;
            }

            if (txtGol_Prsh.Text.Trim() == "")
            {
                MessageBox.Show("Type Vendor harus diisi..");
                vBol = false;
            }

            if (txtGol_Prsh.Text.ToUpper() == "EXPEDISI")
            {
                if (txtFieldPool.Text.Trim() == "")
                {
                    MessageBox.Show("Untuk type expedisi, pool harus diisi");
                    vBol = false;
                }
            }

            return vBol;
        }

        private Boolean ValidTax()
        {
            Boolean vBol = true;

            if (Convert.ToDecimal(txtPPN.Text) > 0)
            {
                if (txtNPWP.Text.Trim() == "")
                {
                    MessageBox.Show("NPWP harus diisi..");
                    txtNPWP.Focus();
                    vBol = false;
                }
                else if (txtTaxName.Text.Trim() == "")
                {
                    MessageBox.Show("Tax Name harus diisi..");
                    txtTaxName.Focus();
                    vBol = false;
                }
                else if (dgvAttachment.RowCount < 1)
                {
                    MessageBox.Show("Mohon lampirkan dokumen di Attachment..");
                    vBol = false;
                }


            }
            else if (txtNPWP.Text.Trim() != "")
            {
                if (dgvAttachment.RowCount < 1)
                {
                    MessageBox.Show("Mohon lampirkan dokumen di Attachment..");
                    vBol = false;
                }
            }
            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                this.TabCtrl_Vendor.SelectedTab = TabPage_General;
                return;
            }
            else if (!ValidTax())
            {
                this.TabCtrl_Vendor.SelectedTab = TabPage_Documents;
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
                        strSql = "UPDATE VendTable SET VendName=@Nama, ";
                        strSql += "Gol_Prsh=@Gol_Prsh,Status=@Status,";
                        strSql += "NPWP=@NPWP,TaxName=@TaxName,TaxAddress=@TaxAddress,PKP=@PKP,";
                        strSql += "SIUP=@SIUP,TermOfPayment=@TermOfPayment,";
                        strSql += "PPN=@PPN,PPH=@PPH,CurrencyId=@Mata_Uang,";
                        strSql += "PaymentModeId=@Payment_Method,Limit_Total=@Limit_Total,Limit_Per_PO=@Limit_Per_PO,UpdatedBy=@UEdit,UpdatedDate=@UDateEdit,POOL = @pool ";
                        strSql += "WHERE VendId='" + txtKode_Prsh.Text + "'";
                        using (SqlCommand cmdVendor = new SqlCommand(strSql, ConMaster))
                        {
                            cmdVendor.Parameters.AddWithValue("@Nama", txtNama_Prsh.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@Status", txtStatus.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@NPWP", txtNPWP.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@TaxName", txtTaxName.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@TaxAddress", txtTaxAddress.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@PKP", txtPKP.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@SIUP", txtSIUP.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@TermOfPayment", txtTermOfPayment.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@PPN", txtPPN.Text.Replace(",", ""));
                            cmdVendor.Parameters.AddWithValue("@PPH", txtPPH.Text.Replace(",", ""));
                            cmdVendor.Parameters.AddWithValue("@Mata_Uang", txtMata_Uang.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@Payment_Method", txtPayment_Method.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@Limit_Total", txtLimit_Total.Text.Replace(",", ""));
                            cmdVendor.Parameters.AddWithValue("@Limit_Per_PO", txtLimit_Per_PO.Text.Replace(",", ""));
                            cmdVendor.Parameters.AddWithValue("@UEdit", ControlMgr.UserId);
                            cmdVendor.Parameters.AddWithValue("@pool", txtFieldPool.Text.Trim());
                            cmdVendor.Parameters.AddWithValue("@UDateEdit", DateTime.Now);
                            cmdVendor.ExecuteNonQuery();
                        }
                    }
                    scope.Complete();
                }
                //Steven dgvAttachment
                Query = "Delete from tblAttachments where ReffTableName='CustTable' And ReffTransId='" + txtKode_Prsh.Text.Trim() + "';";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                {
                    Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                    Query += "( 'CustTable', '" + txtKode_Prsh.Text.Trim() + "', '";
                    Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                    Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";
                    Query += dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString();// +"', CONVERT(varbinary(MAX),'";
                    Query += "',@binaryValue";
                    Query += ");";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                    Cmd.ExecuteNonQuery();
                }
                //steven end
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

            MessageBox.Show("Berhasil Update data..");
            txt_ReadOnly(true);
            Chk_Enable(false);
            ButtonSearch(false);
            Btn_Addr_Cont(false);
            Btn_EditCancelSaveDel(true, false, false, true);

            //steven dgvAttachment
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;
        }

        private void txtPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as MetroFramework.Controls.MetroTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtPPN.Text = String.Format("{0:#,##0.00}", txtPPN.Text == "" ? 0 : Convert.ToDecimal(txtPPN.Text));
            }
        }

        private void txtPPN_Leave(object sender, EventArgs e)
        {
            txtPPN.Text = String.Format("{0:#,##0.00}", txtPPN.Text == "" ? 0 : Convert.ToDecimal(txtPPN.Text));       
        }

        private void txtPPH_Leave(object sender, EventArgs e)
        {
            txtPPH.Text = String.Format("{0:#,##0.00}", txtPPH.Text == "" ? 0 : Convert.ToDecimal(txtPPH.Text));      
        }

        private void txtPPH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
     (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as MetroFramework.Controls.MetroTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtPPH.Text = String.Format("{0:#,##0.00}", txtPPH.Text == "" ? 0 : Convert.ToDecimal(txtPPH.Text));
            }
        }

        private void txtLimit_Total_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
     (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as MetroFramework.Controls.MetroTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtLimit_Total.Text = String.Format("{0:#,##0.00}", txtLimit_Total.Text == "" ? 0 : Convert.ToDecimal(txtLimit_Total.Text));
            }
        }

        private void txtLimit_Total_Leave(object sender, EventArgs e)
        {
            txtLimit_Total.Text = String.Format("{0:#,##0.00}", txtLimit_Total.Text == "" ? 0 : Convert.ToDecimal(txtLimit_Total.Text));     
        }

        private void txtLimit_Per_PO_Leave(object sender, EventArgs e)
        {
            txtLimit_Per_PO.Text = String.Format("{0:#,##0.00}", txtLimit_Per_PO.Text == "" ? 0 : Convert.ToDecimal(txtLimit_Per_PO.Text));
        }

        private void txtLimit_Per_PO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
    (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as MetroFramework.Controls.MetroTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtLimit_Per_PO.Text = String.Format("{0:#,##0.00}", txtLimit_Per_PO.Text == "" ? 0 : Convert.ToDecimal(txtLimit_Per_PO.Text));
            }
        }

        private void Ambil_Gol_Prsh()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Gol_Prsh WHERE Gol_Prsh=@Gol_Prsh";
            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
            {
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (drGol_Prsh.HasRows)
                {
                    while (drGol_Prsh.Read())
                    {
                        txtGol_Prsh.Text = Convert.IsDBNull(drGol_Prsh["Gol_Prsh"]) ? "" : (string)drGol_Prsh["Gol_Prsh"];
                    }
                }
                else
                {
                    MessageBox.Show("Type Perusahaan " + txtGol_Prsh.Text + " doesn't exist");
                }
                drGol_Prsh.Close();
                ConMaster.Close();
            }
        }

        private void Cek_Gol_Prsh()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Gol_Prsh WHERE Gol_Prsh=@Gol_Prsh";
            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
            {
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (!drGol_Prsh.HasRows)
                {
                    MessageBox.Show("Type Perusahaan " + txtGol_Prsh.Text + " doesn't exist");
                    ControlMgr.TblName = "Gol_Prsh";
                    ControlMgr.tmpSort = "ORDER BY Gol_Prsh";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Type Vendor";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txtGol_Prsh.Text = ControlMgr.Kode;
                        Ambil_Gol_Prsh();
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drGol_Prsh.Close();
                ConMaster.Close();
            }

            if (vBolFound)
            {
                Ambil_Gol_Prsh();
            }
            else
            {
                txtGol_Prsh.Focus();
            }
        }        

        private void Ambil_ToP()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM TermOfPayment WHERE TermOfPayment=@ToP";

            using (SqlCommand cmdToP = new SqlCommand(strSql, ConMaster))
            {
                cmdToP.Parameters.AddWithValue("@Top", txtTermOfPayment.Text);
                drToP = cmdToP.ExecuteReader();

                if (drToP.HasRows)
                {
                    while (drToP.Read())
                    {
                        txtTermOfPayment.Text = Convert.IsDBNull(drToP["TermOfPayment"]) ? "" : (string)drToP["TermOfPayment"];
                    }
                }
                else
                {
                    MessageBox.Show("Kode ToP " + txtTermOfPayment.Text + " doesn't exist");
                }
                drToP.Close();
                ConMaster.Close();
            }
        }

        private void Cek_ToP()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM TermOfPayment WHERE TermOfPayment=@ToP";

            using (SqlCommand cmdToP = new SqlCommand(strSql, ConMaster))
            {
                cmdToP.Parameters.AddWithValue("@ToP", txtTermOfPayment.Text);
                drToP = cmdToP.ExecuteReader();

                if (!drToP.HasRows)
                {
                    MessageBox.Show("Term of Payment " + txtTermOfPayment.Text + " doesn't exist");
                    ControlMgr.TblName = "TermOfPayment";
                    ControlMgr.tmpSort = "ORDER BY TermOfPayment";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Term of Payment";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txtTermOfPayment.Text = ControlMgr.Kode;
                        Ambil_ToP();
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drToP.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_ToP();
                }
                else
                {
                    txtTermOfPayment.Focus();
                }
            }
        }

        private void Ambil_Mata_Uang()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM CurrencyTable WHERE CurrencyId=@Mata_Uang";

            using (SqlCommand cmdMata_Uang = new SqlCommand(strSql, ConMaster))
            {
                cmdMata_Uang.Parameters.AddWithValue("@Mata_Uang", txtMata_Uang.Text);
                drMata_Uang = cmdMata_Uang.ExecuteReader();
                if (drMata_Uang.HasRows)
                {
                    while (drMata_Uang.Read())
                    {
                        txtMata_Uang.Text = Convert.IsDBNull(drMata_Uang["CurrencyId"]) ? "" : (string)drMata_Uang["CurrencyId"];
                    }
                }
                else
                {
                    MessageBox.Show("Mata Uang " + txtMata_Uang.Text + " doesn't exist");
                }
                drMata_Uang.Close();
                ConMaster.Close();
            }
        }

        private void Cek_Mata_Uang()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM CurrencyTable WHERE CurrencyId=@Mata_Uang";

            using (SqlCommand cmdMata_Uang = new SqlCommand(strSql, ConMaster))
            {
                cmdMata_Uang.Parameters.AddWithValue("@Mata_Uang", txtMata_Uang.Text);
                drMata_Uang = cmdMata_Uang.ExecuteReader();

                if (!drMata_Uang.HasRows)
                {
                    MessageBox.Show("Mata Uang " + txtMata_Uang.Text + " doesn't exist");
                    ControlMgr.TblName = "CurrencyTable";
                    ControlMgr.tmpSort = "ORDER BY CurrencyId";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Mata Uang";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txtMata_Uang.Text = ControlMgr.Kode;
                        Ambil_Mata_Uang();
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drMata_Uang.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_Mata_Uang();
                }
                else
                {
                    txtMata_Uang.Focus();
                }
            }
        }

        private void Ambil_Payment_Method()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM PaymentMode WHERE PaymentModeId=@Payment_Method";

            using (SqlCommand cmdPayment_Method = new SqlCommand(strSql, ConMaster))
            {
                cmdPayment_Method.Parameters.AddWithValue("@Payment_Method", txtPayment_Method.Text);
                drPayment_Method = cmdPayment_Method.ExecuteReader();

                if (drPayment_Method.HasRows)
                {
                    while (drPayment_Method.Read())
                    {
                        txtPayment_Method.Text = Convert.IsDBNull(drPayment_Method["PaymentModeId"]) ? "" : (string)drPayment_Method["PaymentModeId"];
                    }
                }
                else
                {
                    MessageBox.Show("Payment Method " + txtPayment_Method.Text + " doesn't exist");
                }
                drPayment_Method.Close();
                ConMaster.Close();
            }
        }

        private void Cek_Payment_Method()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM PaymentMode WHERE PaymentModeId=@Payment_Method";

            using (SqlCommand cmdPayment_Method = new SqlCommand(strSql, ConMaster))
            {
                cmdPayment_Method.Parameters.AddWithValue("@Payment_Method", txtPayment_Method.Text);
                drPayment_Method = cmdPayment_Method.ExecuteReader();
                if (!drPayment_Method.HasRows)
                {
                    MessageBox.Show("Payment Mode " + txtPayment_Method.Text + " doesn't exist");

                    ControlMgr.TblName = "PaymentMode";
                    ControlMgr.tmpSort = "ORDER BY PaymentModeId";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Payment Method";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txtPayment_Method.Text = ControlMgr.Kode;
                        Ambil_Payment_Method();
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drPayment_Method.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_Payment_Method();
                }
                else
                {
                    txtPayment_Method.Focus();
                }
            }
        }                  

        private void BtnNew_Addr_Click(object sender, EventArgs e)
        {
            Form FrmPop_Address = new Master.Customer.FrmPop_Address(true, "", txtKode_Prsh.Text, "VendTable");
            FrmPop_Address.Text = "Vendor Address";
            FrmPop_Address.ShowDialog();
            IsiDtGridView_Address();
        }

        private void BtnEdit_Addr_Click(object sender, EventArgs e)
        {
            if (DtGridView_Address.Rows.Count > 0)
            {
                var _NameAddress = DtGridView_Address.CurrentRow.Cells["Name"].Value.ToString();
                Form FrmPop_Address = new Master.Customer.FrmPop_Address(false, _NameAddress, txtKode_Prsh.Text, "VendTable");
                FrmPop_Address.Text = "Vendor Address";
                FrmPop_Address.ShowDialog();
                IsiDtGridView_Address();
            }
        }

        private void BtnDelete_Addr_Click(object sender, EventArgs e)
        {
            if (DtGridView_Address.Rows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Alamat " + DtGridView_Address.CurrentRow.Cells["Name"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                                strSql = "DELETE FROM Address WHERE RecId='" + Convert.ToInt32(DtGridView_Address.CurrentRow.Cells["RecId"].Value.ToString()) + "'";
                                using (SqlCommand cmdVendor = new SqlCommand(strSql, ConMaster))
                                {
                                    cmdVendor.ExecuteNonQuery();
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
                    IsiDtGridView_Address();
                }
            }
        }

        private void BtnNew_Contact_Click(object sender, EventArgs e)
        {
            //Form FrmPop_Contact = new Master.Customer.FrmPop_Contact(true, "", txtKode_Prsh.Text, "VendTable");
            //FrmPop_Contact.Text = "Vendor Contact";
            //FrmPop_Contact.ShowDialog();
            //IsiDtGridView_Contact();
        }

        private void BtnEdit_Contact_Click(object sender, EventArgs e)
        {
            /*
            if (DtGridView_Contact.Rows.Count > 0)
            {
                var _Deskripsi = DtGridView_Contact.CurrentRow.Cells["Deskripsi"].Value.ToString();
                Form FrmPop_Contact = new Master.Customer.FrmPop_Contact(false, _Deskripsi, txtKode_Prsh.Text, "VendTable");
                FrmPop_Contact.Text = "Vendor Address";
                FrmPop_Contact.ShowDialog();
                IsiDtGridView_Contact();
            }*/
        }

        private void BtnDelete_Contact_Click(object sender, EventArgs e)
        {
            if (DtGridView_Contact.Rows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Contact " + DtGridView_Contact.CurrentRow.Cells["Deskripsi"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                                strSql = "DELETE FROM Contact WHERE RecId='" + Convert.ToInt32(DtGridView_Contact.CurrentRow.Cells["RecId"].Value.ToString()) + "'";
                                using (SqlCommand cmdVendor = new SqlCommand(strSql, ConMaster))
                                {
                                    cmdVendor.ExecuteNonQuery();
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
                    IsiDtGridView_Contact();
                }
            }
        }

        private void btntxtGol_Prsh_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "Gol_Prsh";
            ControlMgr.tmpSort = "ORDER BY Gol_Prsh";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Type Vendor";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtGol_Prsh.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
    
            fieldPoolSetting();
        }

        private void fieldPoolSetting()
        {
            txtFieldPool.ReadOnly = true;

            if (txtGol_Prsh.Text.ToUpper() == "EXPEDISI")
            {
                txtFieldPool.ReadOnly = false;
            }
            else
            {
                txtFieldPool.Text = "";
                txtFieldPool.ReadOnly = true;
            }
        }

        private void txtGol_Prsh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Gol_Prsh();
            }
        }

        private void txtGol_Prsh_Leave(object sender, EventArgs e)
        {
            if (txtGol_Prsh.Text.Trim() != "")
            {
                Cek_Gol_Prsh();
            }
        }

        private void txtTermOfPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_ToP();
            }
        }

        private void txtTermOfPayment_Leave(object sender, EventArgs e)
        {
            if (txtTermOfPayment.Text.Trim() != "")
            {
                Cek_ToP();
            }
        }

        private void txtMata_Uang_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Mata_Uang();
            }
        }

        private void txtMata_Uang_Leave(object sender, EventArgs e)
        {
            if (txtMata_Uang.Text.Trim() != "")
            {
                Cek_Mata_Uang();
            }
        }

        private void txtPayment_Method_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Payment_Method();
            }
        }

        private void txtPayment_Method_Leave(object sender, EventArgs e)
        {
            if (txtPayment_Method.Text.Trim() != "")
            {
                Cek_Payment_Method();
            }
        }

        private void btntxtTermOfPayment_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "TermOfPayment";
            ControlMgr.tmpSort = "ORDER BY TermOfPayment";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Term of Payment";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtTermOfPayment.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void btntxtMata_Uang_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "CurrencyTable";
            ControlMgr.tmpSort = "ORDER BY CurrencyId";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Mata Uang";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtMata_Uang.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void btntxtPayment_Method_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "PaymentMode";
            ControlMgr.tmpSort = "ORDER BY PaymentModeId";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Payment Method";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtPayment_Method.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void BtnNew_Rek_Click(object sender, EventArgs e)
        {
            Form FrmPop_Rekening = new Master.Rekening.FrmPop_Rekening(true, "", txtKode_Prsh.Text, "VendTable");
            FrmPop_Rekening.Text = "Rekening Vendor";
            FrmPop_Rekening.ShowDialog();
            IsiDtGridView_Rekening();
        }

        private void BtnEdit_Rek_Click(object sender, EventArgs e)
        {
            if (DtGridView_Rekening.Rows.Count > 0)
            {
                var _No_Rekening = DtGridView_Rekening.CurrentRow.Cells["No Rekening"].Value.ToString();
                Form FrmPop_Rekening = new Master.Rekening.FrmPop_Rekening(false, _No_Rekening, txtKode_Prsh.Text, "VendTable");
                FrmPop_Rekening.Text = "Rekening Vendor";
                FrmPop_Rekening.ShowDialog();
                IsiDtGridView_Rekening();
            }
        }

        private void BtnDelete_Rek_Click(object sender, EventArgs e)
        {
            if (DtGridView_Rekening.Rows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Rekening " + DtGridView_Rekening.CurrentRow.Cells["No Rekening"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                                strSql = "DELETE FROM Rekening WHERE No_Rekening='" + DtGridView_Rekening.CurrentRow.Cells["No Rekening"].Value.ToString() + "'";
                                using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                                {
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
                    IsiDtGridView_Rekening();
                }
            }
        }

        //Created by : Thaddaeus Matthias, 14 March 2018
        // for status log customer
        //==================================begin=====================================
        private void TabCtrl_Vendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabCtrl_Vendor.SelectedTab == TabPage_StatusLog)
            {
                refreshgridStatusLog();
            }
        }

        private void refreshgridStatusLog()
        {
            DtGridView_StatusLog.Columns.Clear();
            string query1;
            string query2;
            string query3;
            SqlConnection con = ConnectionString.GetConnection();
            query1 = "SELECT TOP 1200 [StatusLog_Date], [StatusLog_PK1],[StatusLog_Status], [StatusLog_Description] ";
            query1 += ", cast(ROW_NUMBER() OVER (ORDER BY [StatusLog_Date] desc) as int) as 'RowNumber' INTO #Temp FROM [dbo].[StatusLog_Vendor] ";
            query1 += " WHERE Vendor_Id='" + txtKode_Prsh.Text + "' AND ([StatusLog_PK1] LIKE '%" + txtBoxStatusLog.Text + "%')";
            if (chkBoxStatusLogDate.Checked == true)
            {
                query1 += "AND CONVERT (varchar(10),[StatusLog_Date],103) BETWEEN CONVERT (varchar(10),'" + DateStatusLog1.Value.Date.ToString("dd/MM/yyyy") + "',103) AND CONVERT (varchar(10),'" + DateStatusLog2.Value.Date.ToString("dd/MM/yyyy") + "',103)";
            }
            query1 += " ORDER BY [StatusLog_Date] DESC";
            SqlCommand cmd2 = new SqlCommand(query1, con);
            cmd2.ExecuteNonQuery();

            query3 = "SELECT COUNT(RowNumber) FROM #Temp";
            SqlCommand cmd3 = new SqlCommand(query3, con);
            int limit = Convert.ToInt32(cmd3.ExecuteScalar().ToString());
            if (limit % 12 != 0) lblStatusLogPage.Text = ((limit / 12) + 1).ToString();
            else lblStatusLogPage.Text = (limit / 12).ToString();
            lblStatusLogTotalRows.Text = "Total Rows : " + limit + "";

            int num1 = (Int32.Parse(txtBoxStatusLogPage.Text) * 12) - 11;
            int num2 = Int32.Parse(txtBoxStatusLogPage.Text) * 12;
            query2 = "SELECT [StatusLog_Date] AS 'Date', [StatusLog_PK1] AS 'ID',[StatusLog_Status] AS 'Status', [StatusLog_Description] AS 'Description' from #Temp WHERE RowNumber BETWEEN " + num1 + " AND " + num2 + " DROP table #Temp";
            DataTable namatable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(query2, con);
            adapter.Fill(namatable);
            DtGridView_StatusLog.DataSource = namatable;
            DtGridView_StatusLog.AutoResizeColumns();
            DtGridView_StatusLog.ReadOnly = true;
            DtGridView_StatusLog.AllowUserToAddRows = false;
            con.Close();

        }

        private void txtBoxStatusLog_TextChanged(object sender, EventArgs e)
        {
            txtBoxStatusLogPage.Text = "1";
            refreshgridStatusLog();
        }

        private void txtBoxStatusLog_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsLetter(e.KeyChar) && !(e.KeyChar == 46))
            {
                e.Handled = true;
            }
        }

        private void DateStatusLog2_ValueChanged(object sender, EventArgs e)
        {
            txtBoxStatusLogPage.Text = "1";
            refreshgridStatusLog();
        }

        private void DateStatusLog1_ValueChanged(object sender, EventArgs e)
        {
            txtBoxStatusLogPage.Text = "1";
            refreshgridStatusLog();
        }

        private void chkBoxStatusLogDate_CheckedChanged(object sender, EventArgs e)
        {
            txtBoxStatusLogPage.Text = "1";
            refreshgridStatusLog();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int pagel1 = Int32.Parse(txtBoxStatusLogPage.Text == string.Empty ? "1" : txtBoxStatusLogPage.Text);
            int pagel2 = Int32.Parse(lblStatusLogPage.Text);
            if (pagel1 > 1)
            {
                pagel1 -= 1;
            }
            txtBoxStatusLogPage.Text = pagel1.ToString();
            refreshgridStatusLog();
        }

        private void btnNex_Click(object sender, EventArgs e)
        {
            int pagel1 = Int32.Parse(txtBoxStatusLogPage.Text == string.Empty ? "1" : txtBoxStatusLogPage.Text);
            int pagel2 = Int32.Parse(lblStatusLogPage.Text);
            if (pagel1 < pagel2)
            {
                pagel1 += 1;
            }
            txtBoxStatusLogPage.Text = pagel1.ToString();
            refreshgridStatusLog();
        }

        private void btnMprev_Click(object sender, EventArgs e)
        {
            int pagel1 = Int32.Parse(txtBoxStatusLogPage.Text == string.Empty ? "1" : txtBoxStatusLogPage.Text);
            int pagel2 = Int32.Parse(lblStatusLogPage.Text);
            if (pagel1 > 1)
            {
                pagel1 = 1;
            }
            txtBoxStatusLogPage.Text = pagel1.ToString();
            refreshgridStatusLog();
        }

        private void btnMnex_Click(object sender, EventArgs e)
        {
            int pagel1 = Int32.Parse(txtBoxStatusLogPage.Text == string.Empty ? "1" : txtBoxStatusLogPage.Text);
            int pagel2 = Int32.Parse(lblStatusLogPage.Text);
            if (pagel1 < pagel2)
            {
                pagel1 = pagel2;
            }
            txtBoxStatusLogPage.Text = pagel1.ToString();
            refreshgridStatusLog();
        }

        private void txtBoxStatusLogPage_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxStatusLogPage.Text.Trim() != string.Empty)
            {
                if (Convert.ToInt32(txtBoxStatusLogPage.Text) > Convert.ToInt32(lblStatusLogPage.Text))
                {
                    txtBoxStatusLogPage.Text = lblStatusLogPage.Text;
                }
                refreshgridStatusLog();
            }
        }

        private void txtBoxStatusLogPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (int)e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtPPH_Leave_1(object sender, EventArgs e)
        {
            txtPPH.Text = String.Format("{0:#,##0.00}", txtPPH.Text == "" ? 0 : Convert.ToDecimal(txtPPH.Text)); 
        }

        private void txtLimit_Total_Leave_1(object sender, EventArgs e)
        {
            txtLimit_Total.Text = String.Format("{0:#,##0.00}", txtLimit_Total.Text == "" ? 0 : Convert.ToDecimal(txtLimit_Total.Text)); 
        }

        private void txtLimit_Per_PO_Leave_1(object sender, EventArgs e)
        {
            txtLimit_Per_PO.Text = String.Format("{0:#,##0.00}", txtLimit_Per_PO.Text == "" ? 0 : Convert.ToDecimal(txtLimit_Per_PO.Text)); 
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            choofdlog.FilterIndex = 3;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                FileName = new List<string>();
                Extension = new List<string>();
                sSelectedFile = new List<string>();


                int i = 0;

                foreach (string file in choofdlog.FileNames)
                {
                    FileStream objFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    int filelength = Convert.ToInt32(objFileStream.Length);
                    byte[] data = new byte[filelength];

                    objFileStream.Read(data, 0, filelength);
                    objFileStream.Close();

                    string tempFullName = Path.GetFileName(file);
                    string[] tempSplit = tempFullName.Split('.');

                    FileName.Add(tempSplit[0]);
                    Extension.Add(tempSplit[tempSplit.Count() - 1]);
                    int filesize = filelength / 1024;
                    this.dgvAttachment.Rows.Add(FileName[i], Extension[i], filesize.ToString(), System.Text.Encoding.UTF8.GetString(data));
                    test.Add(data);
                    i++;
                }
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                String fileid = dgvAttachment.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Id"].Value.ToString();
                String fileName = dgvAttachment.CurrentRow.Cells["FileName"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileName"].Value.ToString();
                String ContentType = dgvAttachment.CurrentRow.Cells["ContentType"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["ContentType"].Value.ToString();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ContentType;
                sfd.FileName = fileName + "." + ContentType;
                sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.AddExtension = true;

                if (ContentType == "pdf")
                {
                    sfd.FilterIndex = 1;
                }
                else if (ContentType == "txt")
                {
                    sfd.FilterIndex = 2;
                }
                else
                {
                    sfd.FilterIndex = 3;
                }

                if (String.IsNullOrEmpty(fileid))
                {
                    MessageBox.Show("File tidak ada dalam database / belum di masukkan.");
                    return;
                }

                Conn = ConnectionString.GetConnection();
                Query = "Select Attachment From tblAttachments Where Id = '" + fileid + "'";
                Cmd = new SqlCommand(Query, Conn);

                byte[] data = (byte[])Cmd.ExecuteScalar();

                if (sfd.ShowDialog() != DialogResult.Cancel)
                {
                    FileStream objFileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                    objFileStream.Write(data, 0, data.Length);
                    objFileStream.Close();
                    MessageBox.Show("Data tersimpan!");
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk didownload");
                return;
            }
        }

        private void btnDelAttachment_Click(object sender, EventArgs e)
        {

            if (dgvAttachment.RowCount > 0)
            {
                if (dgvAttachment.CurrentRow.Index > -1)
                {
                    test.RemoveAt(dgvAttachment.CurrentRow.Index);
                    dgvAttachment.Rows.RemoveAt(dgvAttachment.CurrentRow.Index);
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        //===================================end=======================================
    }
}
