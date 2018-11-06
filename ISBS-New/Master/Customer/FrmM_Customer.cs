using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace ISBS_New.Master.Customer
{   
    public partial class FrmM_Customer : MetroFramework.Forms.MetroForm
    {       
        SqlConnection ConMaster;
        SqlDataReader drCustomer,drCounter,drGol_Prsh,drSales,drToP,drMata_Uang,drPayment_Method;
        string strSql="";
        string vOldtxttblCustomer_Nama_Prsh,vOldtxttblCustomer_Gol_Prsh,vOldtxttblCustomer_Status;
        string vOldtxttblCustomer_Kode_Sls;
        string vOldtxttblCustomer_NPWP,vOldtxttblCustomer_TaxName,vOldtxttblCustomer_TaxAddress,vOldtxttblCustomer_PKP,vOldtxttblCustomer_SIUP,vOldtxttblCustomer_TermOfPayment,vOldtxttblCustomer_Mata_Uang,vOldtxttblCustomer_Payment_Method;
        Boolean vOldchktblCustomer_AffiliatedToGroup, vOldchktblCustomer_DP_Required, vOldchktblCustomer_Bank_Garansi_Required;
        decimal vOldtxttblCustomer_Limit_Total,vOldtxttblCustomer_Limit_Per_PO,vOldtxttblCustomer_Sisa_Limit_Total,vOldtxttblCustomer_Deposito,vOldtxttblCustomer_Bank_Garansi_Total,vOldtxttblCustomer_PPN,vOldtxttblCustomer_PPH;
        decimal vOldtxttblCustomer_Downpayment,vOldtxttblCustomer_Cashback_Balance,vOldtxttblCustomer_Discount_Balance,vOldtxttblCustomer_Debit_Note_Balance,vOldtxttblCustomer_Bonus_Balance;
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
        FrmL_Customer _owner;
            
        public FrmM_Customer(FrmL_Customer owner,string _kode)
        {
            InitializeComponent();
            txttblCustomer_Kode_Prsh.Text = _kode;
            MulaiDariAwal();
            _owner = owner;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmM_Customer_FormClosing);
        }

        private void FrmM_Customer_FormClosing(object sender, FormClosingEventArgs e)
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
            strSql += "WHERE ReffTableName='CustTable' ";
            strSql += "AND ReffID='" + txttblCustomer_Kode_Prsh.Text + "'";
            namesTable_Address.Clear();
            ConMaster = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
            {
                drCustomer = cmd.ExecuteReader();
                while (drCustomer.Read())
                {
                    DataRow row;
                    row = namesTable_Address.NewRow();
                    row["Name"] = Convert.IsDBNull(drCustomer["Name"]) ? "" : (string)drCustomer["Name"];
                    row["Address"] = Convert.IsDBNull(drCustomer["Address"]) ? "" : (string)drCustomer["Address"];
                    row["Purpose"] = Convert.IsDBNull(drCustomer["PurposeType"]) ? "" : (string)drCustomer["PurposeType"];
                    row["Primary"] = Convert.IsDBNull(drCustomer["PrimaryC"]) ? false : (Boolean)drCustomer["PrimaryC"];
                    row["Provinsi"] = Convert.IsDBNull(drCustomer["Provinsi"]) ? "" : (string)drCustomer["Provinsi"];
                    row["Kota"] = Convert.IsDBNull(drCustomer["Kota"]) ? "" : (string)drCustomer["Kota"];
                    row["Kode Pos"] = Convert.IsDBNull(drCustomer["Kode_Pos"]) ? "" : (string)drCustomer["Kode_Pos"];
                    row["RT"] = Convert.IsDBNull(drCustomer["RT"]) ? "" : (string)drCustomer["RT"];
                    row["RW"] = Convert.IsDBNull(drCustomer["RW"]) ? "" : (string)drCustomer["RW"];
                    row["RecId"] = Convert.IsDBNull(drCustomer["RecId"]) ? 0 : Convert.ToInt32(drCustomer["RecId"]);
                    namesTable_Address.Rows.Add(row);
                }
            }
            drCustomer.Close();
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
            strSql += "WHERE ReffTableName='CustTable' ";
            strSql += "AND ReffID='" + txttblCustomer_Kode_Prsh.Text + "'";
            namesTable_Contact.Clear();
            ConMaster = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
            {
                drCustomer = cmd.ExecuteReader();
                while (drCustomer.Read())
                {
                    DataRow row;
                    row = namesTable_Contact.NewRow();
                    row["Deskripsi"] = Convert.IsDBNull(drCustomer["Deskripsi"]) ? "" : (string)drCustomer["Deskripsi"];
                    row["Type"] = Convert.IsDBNull(drCustomer["ContactType"]) ? "" : (string)drCustomer["ContactType"];
                    row["Contact"] = Convert.IsDBNull(drCustomer["ContactNo"]) ? "" : (string)drCustomer["ContactNo"];
                    row["Ext. No"] = Convert.IsDBNull(drCustomer["ExtNo"]) ? "" : (string)drCustomer["ExtNo"];
                    row["Primary"] = Convert.IsDBNull(drCustomer["PrimaryC"]) ? false : (Boolean)drCustomer["PrimaryC"];
                    row["RecId"] = Convert.IsDBNull(drCustomer["RecId"]) ? 0 : Convert.ToInt32(drCustomer["RecId"]);
                    namesTable_Contact.Rows.Add(row);
                }
            }
            drCustomer.Close();
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
            strSql += "WHERE ReffTableName='CustTable' ";
            strSql += "AND ReffID='" + txttblCustomer_Kode_Prsh.Text + "'";
            namesTable_Rekening.Clear();
            ConMaster = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
            {
                drCustomer = cmd.ExecuteReader();
                while (drCustomer.Read())
                {
                    DataRow row;
                    row = namesTable_Rekening.NewRow();
                    row["No Rekening"] = Convert.IsDBNull(drCustomer["No_Rekening"]) ? "" : (string)drCustomer["No_Rekening"];
                    row["Bank"] = Convert.IsDBNull(drCustomer["BankGroupId"]) ? "" : (string)drCustomer["BankGroupId"];
                    row["Nama Bank"] = Convert.IsDBNull(drCustomer["BankGroupName"]) ? "" : (string)drCustomer["BankGroupName"];
                    row["Cabang"] = Convert.IsDBNull(drCustomer["Cabang"]) ? "" : (string)drCustomer["Cabang"];
                    row["Pemilik"] = Convert.IsDBNull(drCustomer["Pemilik"]) ? "" : (string)drCustomer["Pemilik"];
                    row["Keterangan"] = Convert.IsDBNull(drCustomer["Keterangan"]) ? "" : (string)drCustomer["Keterangan"];
                    row["Aktif"] = Convert.IsDBNull(drCustomer["Aktif"]) ? false : (Boolean)drCustomer["Aktif"];
                    namesTable_Rekening.Rows.Add(row);
                }
            }
            drCustomer.Close();
            ConMaster.Close();
            DtGridView_Rekening.Refresh();
        }
        //Rekening Datatable End

        private void FrmM_Customer_Load(object sender, EventArgs e)
        {
            Create_tmpAddress();
            DtGridView_Address.DataSource = "";
            Create_tmpContact();
            DtGridView_Contact.DataSource = "";
            Create_tmpRekening();
            DtGridView_Rekening.DataSource = "";

            Boolean BolFound = false;

            try                       
            {                          
                ConMaster = ConnectionString.GetConnection();                              
                strSql = "SELECT * FROM CustTable ";                
                strSql += "WHERE CustId='" + txttblCustomer_Kode_Prsh.Text + "'";                
                using(SqlCommand cmdCustomer = new SqlCommand(strSql,ConMaster))                                
                {                
                    drCustomer = cmdCustomer.ExecuteReader();                   
                    if (drCustomer.HasRows)                   
                    {
                        BolFound = true;
                        while (drCustomer.Read())                        
                        {
                            txttblCustomer_Nama_Prsh.Text = Convert.IsDBNull(drCustomer["CustName"]) ? "" : (string)drCustomer["CustName"];
                            txttblCustomer_Gol_Prsh.Text = Convert.IsDBNull(drCustomer["Gol_Prsh"]) ? "" : (string)drCustomer["Gol_Prsh"];
                            txttblCustomer_Status.Text = Convert.IsDBNull(drCustomer["Status"]) ? "" : (string)drCustomer["Status"];

                            var AffiliatedToGroup = Convert.IsDBNull(drCustomer["AffiliatedToGroup"]) ? "" : (string)drCustomer["AffiliatedToGroup"];
                            if (AffiliatedToGroup == "N") { chktblCustomer_AffiliatedToGroup.Checked = false; } else { chktblCustomer_AffiliatedToGroup.Checked = true; }

                            txttblCustomer_Kode_Sls.Text = Convert.IsDBNull(drCustomer["Kode_Sls"]) ? "" : (string)drCustomer["Kode_Sls"];
                            txttblCustomer_NPWP.Text = Convert.IsDBNull(drCustomer["NPWP"]) ? "" : (string)drCustomer["NPWP"];
                            txttblCustomer_TaxName.Text = Convert.IsDBNull(drCustomer["TaxName"]) ? "" : (string)drCustomer["TaxName"];
                            txttblCustomer_TaxAddress.Text = Convert.IsDBNull(drCustomer["TaxAddress"]) ? "" : (string)drCustomer["TaxAddress"];
                            txttblCustomer_PKP.Text = Convert.IsDBNull(drCustomer["PKP"]) ? "" : (string)drCustomer["PKP"];
                            txttblCustomer_SIUP.Text = Convert.IsDBNull(drCustomer["SIUP"]) ? "" : (string)drCustomer["SIUP"];
                            txtIdentityNo.Text = Convert.IsDBNull(drCustomer["ID_No"]) ? "" : (string)drCustomer["ID_No"];
                            txttblCustomer_TermOfPayment.Text = Convert.IsDBNull(drCustomer["TermOfPayment"]) ? "" : (string)drCustomer["TermOfPayment"];

                            var DP_Required = Convert.IsDBNull(drCustomer["DP_Required"]) ? "" : (string)drCustomer["DP_Required"];
                            if (DP_Required == "N") { chktblCustomer_DP_Required.Checked = false; } else { chktblCustomer_DP_Required.Checked = true; }

                            var Bank_Garansi_Required = Convert.IsDBNull(drCustomer["Bank_Garansi_Required"]) ? "" : (string)drCustomer["Bank_Garansi_Required"];
                            if (Bank_Garansi_Required == "N") { chktblCustomer_Bank_Garansi_Required.Checked = false; } else { chktblCustomer_Bank_Garansi_Required.Checked = true; }

                            txttblCustomer_PPN.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["PPN"]) ? 0 : (decimal)drCustomer["PPN"]);
                            txttblCustomer_PPH.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["PPH"]) ? 0 : (decimal)drCustomer["PPH"]);
                            txttblCustomer_Mata_Uang.Text = Convert.IsDBNull(drCustomer["CurrencyId"]) ? "" : (string)drCustomer["CurrencyId"];
                            txttblCustomer_Payment_Method.Text = Convert.IsDBNull(drCustomer["PaymentModeId"]) ? "" : (string)drCustomer["PaymentModeId"];

                            txttblCustomer_Limit_Total.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Limit_Total"]) ? 0 : (decimal)drCustomer["Limit_Total"]);
                            txttblCustomer_Limit_Per_PO.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Limit_Per_PO"]) ? 0 : (decimal)drCustomer["Limit_Per_PO"]);
                            txtTempLimit.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Limit_Temp"]) ? 0 : (decimal)drCustomer["Limit_Temp"]);
                            txttblCustomer_Sisa_Limit_Total.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Sisa_Limit_Total"]) ? 0 : (decimal)drCustomer["Sisa_Limit_Total"]);
                            txttblCustomer_Deposito.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Deposito"]) ? 0 : (decimal)drCustomer["Deposito"]);
                            txttblCustomer_Bank_Garansi_Total.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Bank_Garansi_Total"]) ? 0 : (decimal)drCustomer["Bank_Garansi_Total"]);
                            txttblCustomer_Downpayment.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Downpayment"]) ? 0 : (decimal)drCustomer["Downpayment"]);
                            txttblCustomer_Cashback_Balance.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Cashback_Balance"]) ? 0 : (decimal)drCustomer["Cashback_Balance"]);
                            txttblCustomer_Discount_Balance.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Discount_Balance"]) ? 0 : (decimal)drCustomer["Discount_Balance"]);
                            txttblCustomer_Debit_Note_Balance.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Debit_Note_Balance"]) ? 0 : (decimal)drCustomer["Debit_Note_Balance"]);
                            txttblCustomer_Bonus_Balance.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drCustomer["Bonus_Balance"]) ? 0 : (decimal)drCustomer["Bonus_Balance"]);
                        }                                        
                    }
                    else 
                    {                        
                        MessageBox.Show("Data Customer " + txttblCustomer_Kode_Prsh.Text + " tidak ditemukan...");                        
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
                drCustomer.Close();            
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
            Query = "Select * From [tblAttachments] Where ReffTableName = 'CustTable' And ReffTransId = '" + txttblCustomer_Kode_Prsh.Text + "'";
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
            this.TabCtrl_tblCustomer.SelectedTab=TabPage_General;
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            EmptyTextBox();
            ButtonSearch(false);
            Chk_Enable(false);
            Btn_Addr_Cont_Rek(false);
            Btn_EditCancelSaveDel(false,false,false,false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txttblCustomer_Kode_Prsh.ReadOnly = true;
            txttblCustomer_Nama_Prsh.ReadOnly = vbol;
            txttblCustomer_Gol_Prsh.ReadOnly = vbol;
            txttblCustomer_Status.ReadOnly = true;
            txttblCustomer_Kode_Sls.ReadOnly = vbol;
            txttblCustomer_NPWP.ReadOnly = vbol;
            txttblCustomer_TaxName.ReadOnly = vbol;
            txttblCustomer_TaxAddress.ReadOnly = vbol;
            txttblCustomer_PKP.ReadOnly = vbol;
            txttblCustomer_SIUP.ReadOnly = vbol;
            txtIdentityNo.ReadOnly = vbol;
            txttblCustomer_TermOfPayment.ReadOnly = vbol;
            txttblCustomer_PPN.ReadOnly = vbol;
            txttblCustomer_PPH.ReadOnly = vbol;
            txttblCustomer_Mata_Uang.ReadOnly = vbol;
            txttblCustomer_Payment_Method.ReadOnly = vbol;
            txttblCustomer_Limit_Total.ReadOnly = vbol;
            txttblCustomer_Limit_Per_PO.ReadOnly = vbol;
            txttblCustomer_Sisa_Limit_Total.ReadOnly = true;
            txttblCustomer_Deposito.ReadOnly = true;
            txttblCustomer_Bank_Garansi_Total.ReadOnly = true;
            txttblCustomer_Downpayment.ReadOnly = true;
            txttblCustomer_Cashback_Balance.ReadOnly = true;
            txttblCustomer_Discount_Balance.ReadOnly = true;
            txttblCustomer_Debit_Note_Balance.ReadOnly = true;
            txttblCustomer_Bonus_Balance.ReadOnly = true;
        }

        private void Chk_Enable(bool vbol)
        {
            chktblCustomer_AffiliatedToGroup.Enabled = false;
            chktblCustomer_Bank_Garansi_Required.Enabled = vbol;
            chktblCustomer_DP_Required.Enabled = vbol;
        }
      
        private void EmptyTextBox()
        {
            //txttblCustomer_Kode_Prsh.Text="";      
            txttblCustomer_Nama_Prsh.Text="";    
            txttblCustomer_Gol_Prsh.Text="";    
            txttblCustomer_Status.Text="";   
            chktblCustomer_AffiliatedToGroup.Checked=false;   
            txttblCustomer_Kode_Sls.Text = "";
            txttblCustomer_NPWP.Text="";  
            txttblCustomer_TaxName.Text="";  
            txttblCustomer_TaxAddress.Text="";  
            txttblCustomer_PKP.Text="";  
            txttblCustomer_SIUP.Text=""; 
            txttblCustomer_TermOfPayment.Text="";  
            chktblCustomer_DP_Required.Checked=false;        
            chktblCustomer_Bank_Garansi_Required.Checked=false;
            txttblCustomer_PPN.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_PPH.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Mata_Uang.Text="";  
            txttblCustomer_Payment_Method.Text="";
            txttblCustomer_Limit_Total.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Limit_Per_PO.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Sisa_Limit_Total.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Deposito.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Bank_Garansi_Total.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Downpayment.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Cashback_Balance.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Discount_Balance.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Debit_Note_Balance.Text = string.Format("{0:#,##0.00}", 0);
            txttblCustomer_Bonus_Balance.Text = string.Format("{0:#,##0.00}", 0);
        }

        private void ButtonSearch(bool vbol)
        {
            btntxttblCustomer_Gol_Prsh.Enabled = vbol;
            btntblCustomer_Kode_Sls.Enabled = vbol;
            //btnBrowse.Enabled = vbol;

            btntxttblCustomer_TermOfPayment.Enabled = vbol;
            btntxttblCustomer_Mata_Uang.Enabled = vbol;
            btntxttblCustomer_Payment_Method.Enabled = vbol;
        }

        private void Btn_Addr_Cont_Rek(bool vBol)
        {
            BtnNew_Addr.Enabled = vBol;
            BtnEdit_Addr.Enabled = vBol;
            BtnDelete_Addr.Enabled = vBol;

            BtnNew_Contact.Enabled = vBol;
            BtnEdit_Contact.Enabled = vBol;
            BtnDelete_Contact.Enabled = vBol;

            BtnNew_Rek.Enabled = vBol;
            BtnEdit_Rek.Enabled = vBol;
            BtnDelete_Rek.Enabled = vBol;
        }

        private void Btn_EditCancelSaveDel(bool vEdit,bool vCancel,bool vSave,bool vDel)
        {
            btnEdit.Enabled = vEdit;
            btnCancel.Enabled = vCancel;
            btnSave.Enabled = vSave;
            btnDelete.Enabled = vDel;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //this.TabCtrl_tblCustomer.SelectedTab = TabPage_General;
            //this.ActiveControl = txttblCustomer_Nama_Prsh;
            txt_ReadOnly(false);
            Chk_Enable(true);
            ButtonSearch(true);
            Btn_Addr_Cont_Rek(true);
            Btn_EditCancelSaveDel(false,true,true, false);

            vOldtxttblCustomer_Nama_Prsh=txttblCustomer_Nama_Prsh.Text;
            vOldtxttblCustomer_Gol_Prsh=txttblCustomer_Gol_Prsh.Text;
            vOldtxttblCustomer_Status=txttblCustomer_Status.Text;
            vOldtxttblCustomer_Kode_Sls = txttblCustomer_Kode_Sls.Text;
            vOldtxttblCustomer_NPWP=txttblCustomer_NPWP.Text; 
            vOldtxttblCustomer_TaxName=txttblCustomer_TaxName.Text; 
            vOldtxttblCustomer_TaxAddress=txttblCustomer_TaxAddress.Text;
            vOldtxttblCustomer_PKP=txttblCustomer_PKP.Text; 
            vOldtxttblCustomer_SIUP=txttblCustomer_SIUP.Text; 
            vOldtxttblCustomer_TermOfPayment=txttblCustomer_TermOfPayment.Text;
            vOldtxttblCustomer_Mata_Uang = txttblCustomer_Mata_Uang.Text;
            vOldtxttblCustomer_Payment_Method = txttblCustomer_Payment_Method.Text;

            vOldchktblCustomer_AffiliatedToGroup = chktblCustomer_AffiliatedToGroup.Checked;
            vOldchktblCustomer_DP_Required=chktblCustomer_DP_Required.Checked;
            vOldchktblCustomer_Bank_Garansi_Required = chktblCustomer_Bank_Garansi_Required.Checked;
            vOldtxttblCustomer_Limit_Total=Convert.ToDecimal(txttblCustomer_Limit_Total.Text);
            vOldtxttblCustomer_Limit_Per_PO = Convert.ToDecimal(txttblCustomer_Limit_Per_PO.Text);
            vOldtxttblCustomer_Sisa_Limit_Total = Convert.ToDecimal(txttblCustomer_Sisa_Limit_Total.Text);
            vOldtxttblCustomer_Deposito = Convert.ToDecimal(txttblCustomer_Deposito.Text);
            vOldtxttblCustomer_Bank_Garansi_Total = Convert.ToDecimal(txttblCustomer_Bank_Garansi_Total.Text);
            vOldtxttblCustomer_PPN = Convert.ToDecimal(txttblCustomer_PPN.Text);
            vOldtxttblCustomer_PPH = Convert.ToDecimal(txttblCustomer_PPH.Text);
            vOldtxttblCustomer_Downpayment = Convert.ToDecimal(txttblCustomer_Downpayment.Text);
            vOldtxttblCustomer_Cashback_Balance = Convert.ToDecimal(txttblCustomer_Cashback_Balance.Text);
            vOldtxttblCustomer_Discount_Balance = Convert.ToDecimal(txttblCustomer_Discount_Balance.Text);
            vOldtxttblCustomer_Debit_Note_Balance = Convert.ToDecimal(txttblCustomer_Debit_Note_Balance.Text);
            vOldtxttblCustomer_Bonus_Balance = Convert.ToDecimal(txttblCustomer_Bonus_Balance.Text);

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

            Query = "Select * From [tblAttachments] Where ReffTableName = 'CustTable' And ReffTransId = '" + txttblCustomer_Kode_Prsh.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                test.Add((byte[])Dr["Attachment"]);
            }
            //STEVEN dgvAttachment
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Chk_Enable(false);
            ButtonSearch(false);
            Btn_Addr_Cont_Rek(false);
            Btn_EditCancelSaveDel(true, false, false, true);


            txttblCustomer_Nama_Prsh.Text=vOldtxttblCustomer_Nama_Prsh;
            txttblCustomer_Gol_Prsh.Text=vOldtxttblCustomer_Gol_Prsh;
            txttblCustomer_Status.Text=vOldtxttblCustomer_Status;
            txttblCustomer_Kode_Sls.Text=vOldtxttblCustomer_Kode_Sls;
            txttblCustomer_NPWP.Text=vOldtxttblCustomer_NPWP;
            txttblCustomer_TaxName.Text=vOldtxttblCustomer_TaxName;
            txttblCustomer_TaxAddress.Text=vOldtxttblCustomer_TaxAddress;
            txttblCustomer_PKP.Text=vOldtxttblCustomer_PKP;
            txttblCustomer_SIUP.Text=vOldtxttblCustomer_SIUP;
            txttblCustomer_TermOfPayment.Text=vOldtxttblCustomer_TermOfPayment;
            txttblCustomer_Mata_Uang.Text=vOldtxttblCustomer_Mata_Uang;             
            txttblCustomer_Payment_Method.Text=vOldtxttblCustomer_Payment_Method;

            chktblCustomer_AffiliatedToGroup.Checked=vOldchktblCustomer_AffiliatedToGroup;
            chktblCustomer_DP_Required.Checked=vOldchktblCustomer_DP_Required;
            chktblCustomer_Bank_Garansi_Required.Checked=vOldchktblCustomer_Bank_Garansi_Required;
            txttblCustomer_Limit_Total.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Limit_Total);
            txttblCustomer_Limit_Per_PO.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Limit_Per_PO);
            txttblCustomer_Sisa_Limit_Total.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Sisa_Limit_Total);
            txttblCustomer_Deposito.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Deposito);
            txttblCustomer_Bank_Garansi_Total.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Bank_Garansi_Total);
            txttblCustomer_PPN.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_PPN);
            txttblCustomer_PPH.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_PPH);
            txttblCustomer_Downpayment.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Downpayment);
            txttblCustomer_Cashback_Balance.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Cashback_Balance);
            txttblCustomer_Discount_Balance.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Discount_Balance);
            txttblCustomer_Debit_Note_Balance.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Debit_Note_Balance);
            txttblCustomer_Bonus_Balance.Text= string.Format("{0:#,##0.00}",vOldtxttblCustomer_Bonus_Balance); 

            this.ActiveControl = btnEdit;

            //steven dgvAttachment
            dgvAttachment.ReadOnly = true;
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;

            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kode Customer " + txttblCustomer_Kode_Prsh.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                            strSql = "DELETE FROM CustTable WHERE CustId='" + txttblCustomer_Kode_Prsh.Text + "'";
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

                var FrmL_Customer = Application.OpenForms.OfType<Master.Customer.FrmL_Customer>().FirstOrDefault();
                if (FrmL_Customer != null)
                {
                    FrmL_Customer.Activate();
                }
                else
                {
                    new Master.Customer.FrmL_Customer().Show();
                }
                this.Close();
            }          
        }

        private void AmbilKodeCustomer()
        {
            string strSql;
            strSql = "SELECT TOP 1 'C'+RIGHT('00000' + CAST((RIGHT(CustId,5)+1) AS NVARCHAR(5)),5) AS CustId ";
            strSql += "FROM CustTable ORDER BY CustId DESC";
            using (SqlCommand cmdCounter = new SqlCommand(strSql, ConMaster))
            {
                drCounter = cmdCounter.ExecuteReader();
                if (drCounter.HasRows)
                {
                    while (drCounter.Read())
                    {
                        txttblCustomer_Kode_Prsh.Text = drCounter["CustId"].ToString();
                    }
                }
                else
                {
                    txttblCustomer_Kode_Prsh.Text = "C00001";
                }
                drCounter.Close();
            }
        }

        private Boolean CekNamaCustomer()
        {
            try
            {
                ConMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM CustTable WHERE CustName=@Nama and CustId<>@Kode_Prsh";
                using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
                {
                    cmd.Parameters.AddWithValue("@Nama", txttblCustomer_Nama_Prsh.Text.Trim());
                    cmd.Parameters.AddWithValue("@Kode_Prsh", txttblCustomer_Kode_Prsh.Text.Trim());
                    drCustomer = cmd.ExecuteReader();
                    if (drCustomer.HasRows)
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
                drCustomer.Close();
                ConMaster.Close();
            }
        }


        private Boolean ValidTax()
        {
            Boolean vBol = true;
            
            if (Convert.ToDecimal(txttblCustomer_PPN.Text) > 0)
            {
                if (txttblCustomer_NPWP.Text.Trim() == "")
                {
                    MessageBox.Show("NPWP harus diisi..");
                    txttblCustomer_NPWP.Focus();
                    vBol = false;
                }
                else if (txttblCustomer_TaxName.Text.Trim() == "")
                {
                    MessageBox.Show("Tax Name harus diisi..");
                    txttblCustomer_TaxName.Focus();
                    vBol = false;
                }
                else if (dgvAttachment.RowCount < 1)
                {
                    MessageBox.Show("Mohon lampirkan dokumen di Attachment..");
                    vBol = false;
                }


            }
            else if (txttblCustomer_NPWP.Text.Trim() != "")
            {
                if (dgvAttachment.RowCount < 1)
                {
                    MessageBox.Show("Mohon lampirkan dokumen di Attachment..");
                    vBol = false;
                }
            }
            return vBol;
        }

        
        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txttblCustomer_Nama_Prsh.Text.Trim() == "")
            {
                MessageBox.Show("Nama Customer harus diisi..");
                vBol = false;
            }
            
            if (CekNamaCustomer())
            {
                MessageBox.Show("Nama Customer sudah ada..");
                vBol = false;
            }

            if (txttblCustomer_Gol_Prsh.Text.Trim() == "")
            {
                MessageBox.Show("Type Customer harus diisi..");
                vBol = false;
            }

            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                this.TabCtrl_tblCustomer.SelectedTab = TabPage_General;
                return;
            }
            else if (!ValidTax())
            {
                this.TabCtrl_tblCustomer.SelectedTab = TabPage_Documents;
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
                            strSql = "UPDATE CustTable SET CustName=@Nama, ";
                            strSql += "Gol_Prsh=@Gol_Prsh,Status=@Status,";
                            strSql += "NPWP=@NPWP,TaxName=@TaxName,TaxAddress=@TaxAddress,PKP=@PKP,";
                            strSql += "SIUP=@SIUP,TermOfPayment=@TermOfPayment,DP_Required=@DP_Required,Bank_Garansi_Required=@Bank_Garansi_Required,";
                            strSql += "PPN=@PPN,PPH=@PPH,CurrencyId=@Mata_Uang,Kode_Sls=@Kode_Sls,";
                            strSql += "PaymentModeId=@Payment_Method,Limit_Total=@Limit_Total,Limit_Per_PO=@Limit_Per_PO,UpdatedBy=@UEdit,UpdatedDate=@UDateEdit,ID_No=@idno,[Limit_Temp]=@TempLimit ";
                            strSql += ",Sisa_Limit_Total = @Limit_Total "; /*stv edit*/
                            strSql += "WHERE CustId='" + txttblCustomer_Kode_Prsh.Text + "'";
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.Parameters.AddWithValue("@Nama", txttblCustomer_Nama_Prsh.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@Gol_Prsh", txttblCustomer_Gol_Prsh.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@Status", txttblCustomer_Status.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@NPWP", txttblCustomer_NPWP.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@TaxName", txttblCustomer_TaxName.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@TaxAddress", txttblCustomer_TaxAddress.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@PKP", txttblCustomer_PKP.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@SIUP", txttblCustomer_SIUP.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@TermOfPayment", txttblCustomer_TermOfPayment.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@DP_Required", chktblCustomer_DP_Required.Checked == true ? "Y" : "N");
                                cmdCustomer.Parameters.AddWithValue("@Bank_Garansi_Required", chktblCustomer_Bank_Garansi_Required.Checked == true ? "Y" : "N");
                                cmdCustomer.Parameters.AddWithValue("@PPN", txttblCustomer_PPN.Text.Replace(",", ""));
                                cmdCustomer.Parameters.AddWithValue("@PPH", txttblCustomer_PPH.Text.Replace(",", ""));
                                cmdCustomer.Parameters.AddWithValue("@Mata_Uang", txttblCustomer_Mata_Uang.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@Kode_Sls", txttblCustomer_Kode_Sls.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@Payment_Method", txttblCustomer_Payment_Method.Text.Trim());
                                cmdCustomer.Parameters.AddWithValue("@Limit_Total", txttblCustomer_Limit_Total.Text.Replace(",", ""));
                                cmdCustomer.Parameters.AddWithValue("@Limit_Per_PO", txttblCustomer_Limit_Per_PO.Text.Replace(",", ""));
                                cmdCustomer.Parameters.AddWithValue("@TempLimit", txtTempLimit.Text.Replace(",", ""));
                                cmdCustomer.Parameters.AddWithValue("@UEdit", "ITDIVISI");
                                cmdCustomer.Parameters.AddWithValue("@UDateEdit", DateTime.Now);
                                cmdCustomer.Parameters.AddWithValue("@idno", txtIdentityNo.Text.Trim());
                                cmdCustomer.ExecuteNonQuery();
                            }                        
                    }
                    scope.Complete();                   
                }
                //Steven dgvAttachment
                Query = "Delete from tblAttachments where ReffTableName='CustTable' And ReffTransId='" + txttblCustomer_Kode_Prsh.Text.Trim() + "';";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                {
                    Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                    Query += "( 'CustTable', '" + txttblCustomer_Kode_Prsh.Text.Trim() + "', '";
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
            Btn_Addr_Cont_Rek(false);
            Btn_EditCancelSaveDel(true, false, false, true);

            //steven dgvAttachment
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;
        }

        private void txttblCustomer_PPN_KeyPress(object sender, KeyPressEventArgs e)
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
                txttblCustomer_PPN.Text = String.Format("{0:#,##0.00}", txttblCustomer_PPN.Text == "" ? 0 : Convert.ToDecimal(txttblCustomer_PPN.Text));
            }
        }

        private void txttblCustomer_PPN_Leave(object sender, EventArgs e)
        {
            txttblCustomer_PPN.Text = String.Format("{0:#,##0.00}", txttblCustomer_PPN.Text == "" ? 0 : Convert.ToDecimal(txttblCustomer_PPN.Text));
        }

        private void txttblCustomer_PPH_Leave(object sender, EventArgs e)
        {
            txttblCustomer_PPH.Text = String.Format("{0:#,##0.00}", txttblCustomer_PPH.Text == "" ? 0 : Convert.ToDecimal(txttblCustomer_PPH.Text));
        }

        private void txttblCustomer_PPH_KeyPress(object sender, KeyPressEventArgs e)
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
                txttblCustomer_PPH.Text = String.Format("{0:#,##0.00}", txttblCustomer_PPH.Text == "" ? 0 : Convert.ToDecimal(txttblCustomer_PPH.Text));
            }
        }

        private void txttblCustomer_Limit_Total_KeyPress(object sender, KeyPressEventArgs e)
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
                txttblCustomer_Limit_Total.Text = String.Format("{0:#,##0.00}", txttblCustomer_Limit_Total.Text == "" ? 0 : Convert.ToDecimal(txttblCustomer_Limit_Total.Text));
            }
        }

        private void txttblCustomer_Limit_Total_Leave(object sender, EventArgs e)
        {
            txttblCustomer_Limit_Total.Text = String.Format("{0:#,##0.00}", txttblCustomer_Limit_Total.Text == "" ? 0 : Convert.ToDecimal(txttblCustomer_Limit_Total.Text));
        }

        private void txttblCustomer_Limit_Per_PO_Leave(object sender, EventArgs e)
        {
            txttblCustomer_Limit_Per_PO.Text = String.Format("{0:#,##0.00}", txttblCustomer_Limit_Per_PO.Text == "" ? 0 : Convert.ToDecimal(txttblCustomer_Limit_Per_PO.Text));
        }

        private void txttblCustomer_Limit_Per_PO_KeyPress(object sender, KeyPressEventArgs e)
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
                txttblCustomer_Limit_Per_PO.Text = String.Format("{0:#,##0.00}", txttblCustomer_Limit_Per_PO.Text == "" ? 0 : Convert.ToDecimal(txttblCustomer_Limit_Per_PO.Text));
            }
        }

        private void Ambil_Gol_Prsh()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Gol_Prsh WHERE Gol_Prsh=@Gol_Prsh";
            using(SqlCommand cmdGol_Prsh = new SqlCommand(strSql,ConMaster))
            {
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh",txttblCustomer_Gol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (drGol_Prsh.HasRows)
                {
                    while (drGol_Prsh.Read())
                    {
                        txttblCustomer_Gol_Prsh.Text = Convert.IsDBNull(drGol_Prsh["Gol_Prsh"]) ? "" : (string)drGol_Prsh["Gol_Prsh"];
                    }
                }
                else
                {
                    MessageBox.Show("Type Perusahaan " + txttblCustomer_Gol_Prsh.Text + " doesn't exist");
                }
                drGol_Prsh.Close();
                ConMaster.Close();
            }        
        }

        private void Cek_Gol_Prsh()
        {
            Boolean vBolFound=false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Gol_Prsh WHERE Gol_Prsh=@Gol_Prsh";           
            using(SqlCommand cmdGol_Prsh = new SqlCommand(strSql,ConMaster))
            {
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txttblCustomer_Gol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (!drGol_Prsh.HasRows)
                {
                    MessageBox.Show("Type Perusahaan " + txttblCustomer_Gol_Prsh.Text + " doesn't exist");
                    ControlMgr.TblName = "Gol_Prsh";
                    ControlMgr.tmpSort = "ORDER BY Gol_Prsh";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Type Customer";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txttblCustomer_Gol_Prsh.Text = ControlMgr.Kode;
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
                txttblCustomer_Gol_Prsh.Focus();
            }
        }        
       
        private void Ambil_Kode_Sls()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Sales WHERE Kode_Sls=@Kode_Sls";
            using(SqlCommand cmdSales = new SqlCommand(strSql,ConMaster))
            {
                cmdSales.Parameters.AddWithValue("@Kode_Sls",txttblCustomer_Kode_Sls.Text);
                drSales = cmdSales.ExecuteReader();

                if (drSales.HasRows)
                {
                    while (drSales.Read())
                    {
                        txttblCustomer_Kode_Sls.Text = Convert.IsDBNull(drSales["Kode_Sls"]) ? "" : (string)drSales["Kode_Sls"];
                    }
                }
                else
                {
                    MessageBox.Show("Kode Sales " + txttblCustomer_Kode_Sls.Text + " doesn't exist");
                }
                drSales.Close();
                ConMaster.Close();
            }
        }

        private void Cek_Kode_Sls()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Sales WHERE Kode_Sls=@Kode_Sls";
            
            using(SqlCommand cmdSales = new SqlCommand(strSql,ConMaster))
            {
                cmdSales.Parameters.AddWithValue("@Kode_Sls",txttblCustomer_Kode_Sls.Text);
                drSales = cmdSales.ExecuteReader();

                if (!drSales.HasRows)
                {
                    MessageBox.Show("Kode Sales " + txttblCustomer_Kode_Sls.Text + " doesn't exist");
                    ControlMgr.TblName = "Sales";
                    ControlMgr.tmpSort = "ORDER BY Kode_Sls";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Sales";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txttblCustomer_Kode_Sls.Text = ControlMgr.Kode;
                        Ambil_Kode_Sls();
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drSales.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_Kode_Sls();
                }
                else
                {
                    txttblCustomer_Kode_Sls.Focus();
                }
            }
        }

        private void Ambil_ToP()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM TermOfPayment WHERE TermOfPayment=@ToP";
            
            using(SqlCommand cmdToP = new SqlCommand(strSql,ConMaster))
            {
                cmdToP.Parameters.AddWithValue("@Top",txttblCustomer_TermOfPayment.Text);
                drToP = cmdToP.ExecuteReader();

                if (drToP.HasRows)
                {
                    while (drToP.Read())
                    {
                        txttblCustomer_TermOfPayment.Text = Convert.IsDBNull(drToP["TermOfPayment"]) ? "" : (string)drToP["TermOfPayment"];
                    }
                }
                else
                {
                    MessageBox.Show("Kode ToP " + txttblCustomer_TermOfPayment.Text + " doesn't exist");
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

            using(SqlCommand cmdToP = new SqlCommand(strSql,ConMaster))
            {
                cmdToP.Parameters.AddWithValue("@ToP",txttblCustomer_TermOfPayment.Text);
                drToP = cmdToP.ExecuteReader();

                if (!drToP.HasRows)
                {
                    MessageBox.Show("Term of Payment " + txttblCustomer_TermOfPayment.Text + " doesn't exist");
                    ControlMgr.TblName = "TermOfPayment";
                    ControlMgr.tmpSort = "ORDER BY TermOfPayment";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Term of Payment";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txttblCustomer_TermOfPayment.Text = ControlMgr.Kode;
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
                    txttblCustomer_TermOfPayment.Focus();
                }
            }            
        }

        private void Ambil_Mata_Uang()
        {
            ConMaster = ConnectionString.GetConnection();         
            strSql= "SELECT * FROM CurrencyTable WHERE CurrencyId=@Mata_Uang";

            using(SqlCommand cmdMata_Uang = new SqlCommand(strSql,ConMaster))
            {
                cmdMata_Uang.Parameters.AddWithValue("@Mata_Uang",txttblCustomer_Mata_Uang.Text);
                drMata_Uang = cmdMata_Uang.ExecuteReader();
                if (drMata_Uang.HasRows)
                {
                    while (drMata_Uang.Read())
                    {
                        txttblCustomer_Mata_Uang.Text = Convert.IsDBNull(drMata_Uang["CurrencyId"]) ? "" : (string)drMata_Uang["CurrencyId"];
                    }
                }
                else
                {
                    MessageBox.Show("Mata Uang " + txttblCustomer_Mata_Uang.Text + " doesn't exist");
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

            using(SqlCommand cmdMata_Uang= new SqlCommand(strSql,ConMaster))
            {
                cmdMata_Uang.Parameters.AddWithValue("@Mata_Uang",txttblCustomer_Mata_Uang.Text);
                drMata_Uang = cmdMata_Uang.ExecuteReader();

                if (!drMata_Uang.HasRows)
                {
                    MessageBox.Show("Mata Uang " + txttblCustomer_Mata_Uang.Text + " doesn't exist");
                    ControlMgr.TblName = "CurrencyTable";
                    ControlMgr.tmpSort = "ORDER BY CurrencyId";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Mata Uang";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txttblCustomer_Mata_Uang.Text = ControlMgr.Kode;
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
                    txttblCustomer_Mata_Uang.Focus();
                }
            }          
        }

        private void Ambil_Payment_Method()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM PaymentMode WHERE PaymentModeId=@Payment_Method";
            
            using(SqlCommand cmdPayment_Method = new SqlCommand(strSql,ConMaster))
            {
                cmdPayment_Method.Parameters.AddWithValue("@Payment_Method",txttblCustomer_Payment_Method.Text);
                drPayment_Method = cmdPayment_Method.ExecuteReader();

                if (drPayment_Method.HasRows)
                {
                    while (drPayment_Method.Read())
                    {
                        txttblCustomer_Payment_Method.Text = Convert.IsDBNull(drPayment_Method["PaymentModeId"]) ? "" : (string)drPayment_Method["PaymentModeId"];
                    }
                }
                else
                {
                    MessageBox.Show("Payment Method " + txttblCustomer_Payment_Method.Text + " doesn't exist");
                }
                drPayment_Method.Close();
                ConMaster.Close();
            }                      
        }

        private void Cek_Payment_Method()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();         
            strSql= "SELECT * FROM PaymentMode WHERE PaymentModeId=@Payment_Method";
           
            using(SqlCommand cmdPayment_Method = new SqlCommand(strSql,ConMaster) )
            {
                cmdPayment_Method.Parameters.AddWithValue("@Payment_Method",txttblCustomer_Payment_Method.Text);
                drPayment_Method = cmdPayment_Method.ExecuteReader();
                if (!drPayment_Method.HasRows)
                {
                    MessageBox.Show("Payment Mode " + txttblCustomer_Payment_Method.Text + " doesn't exist");

                    ControlMgr.TblName = "PaymentMode";
                    ControlMgr.tmpSort = "ORDER BY PaymentModeId";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Payment Method";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txttblCustomer_Payment_Method.Text = ControlMgr.Kode;
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
                    txttblCustomer_Payment_Method.Focus();
                }
            }           
        }

        private void txttblCustomer_Gol_Prsh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Gol_Prsh();
            }
        }

        private void txttblCustomer_Gol_Prsh_Leave(object sender, EventArgs e)
        {           
            if(txttblCustomer_Gol_Prsh.Text.Trim()!="")
            {
                Cek_Gol_Prsh();
            }
        }
            
        private void txttblCustomer_Kode_Sls_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Kode_Sls();
            }
        }

        private void txttblCustomer_Kode_Sls_Leave(object sender, EventArgs e)
        {
            if(txttblCustomer_Kode_Sls.Text.Trim()!="")
            {
                Cek_Kode_Sls();
            }
        }

        private void txttblCustomer_TermOfPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_ToP();
            }
        }

        private void txttblCustomer_TermOfPayment_Leave(object sender, EventArgs e)
        {
            if(txttblCustomer_TermOfPayment.Text.Trim()!="")
            {
                Cek_ToP();
            }
        }

        private void txttblCustomer_Mata_Uang_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Mata_Uang();
            }
        }

        private void txttblCustomer_Mata_Uang_Leave(object sender, EventArgs e)
        {
            if(txttblCustomer_Mata_Uang.Text.Trim()!="")
            {
                Cek_Mata_Uang();
            }
        }

        private void txttblCustomer_Payment_Method_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Payment_Method();
            }
        }

        private void txttblCustomer_Payment_Method_Leave(object sender, EventArgs e)
        {
            if (txttblCustomer_Payment_Method.Text.Trim() != "")
            {
                Cek_Payment_Method();
            }
        }

        private void btntxttblCustomer_Gol_Prsh_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "Gol_Prsh";
            ControlMgr.tmpSort = "ORDER BY Gol_Prsh";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Type Customer";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txttblCustomer_Gol_Prsh.Text=ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }
        
        private void btntblCustomer_Kode_Sls_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "Sales";
            ControlMgr.tmpSort = "ORDER BY Kode_Sls";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Sales";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txttblCustomer_Kode_Sls.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void btntxttblCustomer_TermOfPayment_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "TermOfPayment";
            ControlMgr.tmpSort = "ORDER BY TermOfPayment";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Term of Payment";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txttblCustomer_TermOfPayment.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void btntxttblCustomer_Mata_Uang_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "CurrencyTable";
            ControlMgr.tmpSort = "ORDER BY CurrencyId";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Mata Uang";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txttblCustomer_Mata_Uang.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void btntxttblCustomer_Payment_Method_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "PaymentMode";
            ControlMgr.tmpSort = "ORDER BY PaymentModeId";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Payment Method";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txttblCustomer_Payment_Method.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void BtnNew_Addr_Click(object sender, EventArgs e)
        {
            Form FrmPop_Address = new Master.Customer.FrmPop_Address(true, "", txttblCustomer_Kode_Prsh.Text, "CustTable");
            FrmPop_Address.Text = "Customer Address";
            FrmPop_Address.ShowDialog();           
            IsiDtGridView_Address();
        }

        private void BtnEdit_Addr_Click(object sender, EventArgs e)
        {
            if (DtGridView_Address.Rows.Count > 0)
            {                                
                var _NameAddress =DtGridView_Address.CurrentRow.Cells["Name"].Value.ToString();
                Form FrmPop_Address = new Master.Customer.FrmPop_Address(false,_NameAddress,txttblCustomer_Kode_Prsh.Text,"CustTable");
                FrmPop_Address.Text = "Customer Address";
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
                    IsiDtGridView_Address();
                }
            }
        }

        private void BtnNew_Contact_Click(object sender, EventArgs e)
        {
            /*
            Form FrmPop_Contact = new Master.Customer.FrmPop_Contact(true, "", txttblCustomer_Kode_Prsh.Text, "CustTable");
            FrmPop_Contact.Text = "Customer Contact";
            FrmPop_Contact.ShowDialog();
            IsiDtGridView_Contact();*/

        }

        private void BtnEdit_Contact_Click(object sender, EventArgs e)
        {
            /*
            if (DtGridView_Contact.Rows.Count > 0)
            {
                var _Deskripsi = DtGridView_Contact.CurrentRow.Cells["Deskripsi"].Value.ToString();
                Form FrmPop_Contact = new Master.Customer.FrmPop_Contact(false, _Deskripsi, txttblCustomer_Kode_Prsh.Text, "CustTable");
                FrmPop_Contact.Text = "Customer Address";
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
                    IsiDtGridView_Contact();
                }
            }
        }

        private void BtnNew_Rek_Click(object sender, EventArgs e)
        {
            Form FrmPop_Rekening = new Master.Rekening.FrmPop_Rekening(true, "", txttblCustomer_Kode_Prsh.Text, "CustTable");
            FrmPop_Rekening.Text = "Rekening Customer";
            FrmPop_Rekening.ShowDialog();
            IsiDtGridView_Rekening();
        }

        private void BtnEdit_Rek_Click(object sender, EventArgs e)
        {
            if (DtGridView_Rekening.Rows.Count > 0)
            {
                var _No_Rekening = DtGridView_Rekening.CurrentRow.Cells["No Rekening"].Value.ToString();
                Form FrmPop_Rekening = new Master.Rekening.FrmPop_Rekening(false, _No_Rekening, txttblCustomer_Kode_Prsh.Text, "CustTable");
                FrmPop_Rekening.Text = "Rekening Customer";
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

        private void TabPage_StatusLog_Click(object sender, EventArgs e)
        {

        }
        //BEGIN STEVEN EDIT
        private void refreshgridStatusLog()
        {
            DtGridView_StatusLog.Columns.Clear();
            string query1;
            string query2;
            string query3;
            SqlConnection con = ConnectionString.GetConnection();
            query1 = "SELECT TOP 1200 [StatusLog_Date], [StatusLog_PK1],[StatusLog_Status], [StatusLog_Description] ";
            query1 += ", cast(ROW_NUMBER() OVER (ORDER BY [StatusLog_Date] desc) as int) as 'RowNumber' INTO #Temp FROM [dbo].[StatusLog_Customer] ";
            query1 += " WHERE Customer_Id='" + txttblCustomer_Kode_Prsh.Text + "' AND ([StatusLog_PK1] LIKE '%" + txtBoxStatusLog.Text + "%')";
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

            query2 = "SELECT [StatusLog_Date] AS 'Date', [StatusLog_PK1] AS 'ID',[StatusLog_Status] AS 'Status', [StatusLog_Description] AS 'Description' from #Temp WHERE RowNumber BETWEEN " + Page1() + " AND " + Page2() + " DROP table #Temp";
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

        private void DateStatusLog1_ValueChanged(object sender, EventArgs e)
        {
            txtBoxStatusLogPage.Text = "1";
            refreshgridStatusLog();
        }

        private void DateStatusLog2_ValueChanged(object sender, EventArgs e)
        {
            txtBoxStatusLogPage.Text = "1";
            refreshgridStatusLog();
        }

        private void chkBoxStatusLogDate_CheckedChanged(object sender, EventArgs e)
        {
            txtBoxStatusLogPage.Text = "1";
            refreshgridStatusLog();
        }

        private void btnNex_Click(object sender, EventArgs e)
        {
            int pagel1 = Int32.Parse(txtBoxStatusLogPage.Text);
            int pagel2 = Int32.Parse(lblStatusLogPage.Text);
            if (pagel1 < pagel2)
            {
                pagel1 += 1;
            }
            txtBoxStatusLogPage.Text = pagel1.ToString();
            refreshgridStatusLog();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            int pagel1 = Int32.Parse(txtBoxStatusLogPage.Text);
            int pagel2 = Int32.Parse(lblStatusLogPage.Text);
            if (pagel1 > 1)
            {
                pagel1 -= 1;
            }
            txtBoxStatusLogPage.Text = pagel1.ToString();
            refreshgridStatusLog();
        }

        private void btnMnex_Click(object sender, EventArgs e)
        {
            int pagel1 = Int32.Parse(txtBoxStatusLogPage.Text);
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
            int pagel1 = Int32.Parse(txtBoxStatusLogPage.Text);
            int pagel2 = Int32.Parse(lblStatusLogPage.Text);
            if (pagel1 > 1)
            {
                pagel1 = 1;
            }
            txtBoxStatusLogPage.Text = pagel1.ToString();
            refreshgridStatusLog();
        }

        private int Page1()
        {
            int num = (Int32.Parse(txtBoxStatusLogPage.Text) * 12) - 11;
            return num;
        }

        private int Page2()
        {
            int num = Int32.Parse(txtBoxStatusLogPage.Text) * 12;
            return num;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            //OpenFileDialog choofdlog = new OpenFileDialog();
            //choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            //choofdlog.FilterIndex = 3;
            //choofdlog.Multiselect = false;

            //if (choofdlog.ShowDialog() == DialogResult.OK)
            //{
            //    metroLabel20.Text = System.IO.Path.GetFileName(choofdlog.FileName);
            //}       
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

        private void txtIdentityNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
    }
}
