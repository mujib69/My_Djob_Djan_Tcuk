using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.CustTable
{
    public partial class CustTableForm : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn,Conn2;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        public DataRow row;

        String Mode, Query, Query1;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String CustId = null;
        bool flagCustomer = false;
        bool flagTransaksi = false;
        bool flagProduct = false;
        Master.CustTable.CustTableInquiry Parent;

        public DataTable TableAddress = new DataTable();
        public DataTable TableCP = new DataTable();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public CustTableForm()
        {
            InitializeComponent();
        }

        public void flag(String custid, String mode)
        {
            CustId = custid;
            Mode = mode;
        }

        private void MakeDataTableAddress()
        {
            DataColumn column;
            DataRow row;

            column = new DataColumn("ID", typeof(String));
            column.ReadOnly = true;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("PurposeType", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("Name", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("Address", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("ProvinceId", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("CityId", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("AreaCode", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("RT", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("RW", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            column = new DataColumn("Primary", typeof(bool));
            column.ReadOnly = false;
            column.Unique = false;
            TableAddress.Columns.Add(column);

            dgvAddress.DataSource = TableAddress;
        }

        
        private void MakeDataTableCP()
        {
            DataColumn column;

            column = new DataColumn("ID", typeof(String));
            column.ReadOnly = true;
            column.Unique = false;
            TableCP.Columns.Add(column);

            column = new DataColumn("Deskripsi", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableCP.Columns.Add(column);

            column = new DataColumn("ContactType", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableCP.Columns.Add(column);

            column = new DataColumn("ContactNo", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableCP.Columns.Add(column);

            column = new DataColumn("ExtNo", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableCP.Columns.Add(column);

            column = new DataColumn("Primary", typeof(bool));
            column.ReadOnly = false;
            column.Unique = false;
            TableCP.Columns.Add(column);

            dgvContact.DataSource = TableCP;
        }

        public void setDataRowAddress(DataRow dr)
        {
            if (dgvAddress.DataSource == null)
            {
                MakeDataTableAddress();
            }

            DataRow row = TableAddress.NewRow();
            row["ID"] = "";
            row["PurposeType"] = dr.ItemArray[0].ToString();
            row["Name"] = dr.ItemArray[1].ToString();
            row["Address"] = dr.ItemArray[2].ToString();
            row["ProvinceId"] = dr.ItemArray[3].ToString();
            row["CityId"] = dr.ItemArray[4].ToString();
            row["AreaCode"] = dr.ItemArray[5].ToString();
            row["RT"] = dr.ItemArray[6].ToString();
            row["RW"] = dr.ItemArray[7].ToString();
            row["Primary"] = dr.ItemArray[8].ToString();
            
            TableAddress.Rows.Add(row);
        }

        public void setDataRowCP(DataRow dr)
        {
            //for (int i = 0; dr.ItemArray.Length(); i++)
            if (dgvContact.DataSource == null)
            {
                MakeDataTableCP();
            }

            DataRow row = TableCP.NewRow();
            row["ID"] = "";
            row["Deskripsi"] = dr.ItemArray[0].ToString();
            row["ContactType"] = dr.ItemArray[1].ToString();
            row["ContactNo"] = dr.ItemArray[2].ToString();
            row["ExtNo"] = dr.ItemArray[3].ToString();
            row["Primary"] = dr.ItemArray[4].ToString();

            TableCP.Rows.Add(row);
        }

        private String GenerateId()
        {
            Conn = ConnectionString.GetConnection();
            String LastId = "";
            Query = "Select Top 1 (CustId) From [dbo].[CustTable] order by CustId DESC";

            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                LastId = Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString();
            }

            if (String.IsNullOrEmpty(LastId))
            {
                return "C0001";
            }
            else
            {
                int temp;
                temp = Int32.Parse(LastId.Substring(1, 4)) + 1;

                if (temp < 10)
                {
                    return "C000" + temp;
                }
                else if (temp < 100)
                {
                    return "C00" + temp;
                }
                else if (temp < 1000)
                {
                    return "C0" + temp;
                }
                else if (temp < 10000)
                {
                    return "C" + temp;
                }
                else
                {
                    return temp.ToString();
                }
            }
        }

        private void CustTableForm_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        public void ParentRefreshGrid(Master.CustTable.CustTableInquiry f)
        {
            Parent = f;
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[CustTable] Where CustID = '" + CustId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtCustId.Text = Dr["CustID"].ToString();
                    txtCustName.Text = Dr["CustName"].ToString();
                    txtTaxName.Text = Dr["TaxName"].ToString();
                    txtNPWP.Text = Dr["NPWP"].ToString();
                    txtPKP.Text = Dr["PKP"].ToString();
                    txtCompanyGroupId.Text = Dr["CompanyGroupID"].ToString();
                    txtTempoBayar.Text = Dr["TempoBayar"].ToString();
                    txtPaymentModeId.Text = Dr["PaymentModeID"].ToString();
                    txtTaxGroup.Text = Dr["TaxGroup"].ToString();
                    txtReffBank1ID.Text = Dr["ReffBank1ID"].ToString();
                    txtReffBank2ID.Text = Dr["ReffBank2ID"].ToString();
                    txtReffFullItemID1.Text = Dr["ReffFullItemID1"].ToString();
                    txtReffFullItemID2.Text = Dr["ReffFullItemID2"].ToString();
                    txtReffFullItemID3.Text = Dr["ReffFullItemID3"].ToString();
                    txtReffFullItemID4.Text = Dr["ReffFullItemID4"].ToString();
                    txtReffFullItemID5.Text = Dr["ReffFullItemID5"].ToString();
                    txtCurrencyId.Text = Dr["CurrencyID"].ToString();
                    
                    txtCustGroupId.Text = Dr["CustGroupID"].ToString();
                    txtDepositAmountAffiatedtoGroup.Text = Dr["DepositAmountAffiatedtoGroup"].ToString();
                    txtDepositAmountCurrencyID.Text = Dr["DepositAmountCurrencyID"].ToString();
                    txtDepositAmount.Text = Dr["DepositAmount"].ToString();
                    txtDepositType.Text = Dr["DepositType"].ToString();
                    txtDPAmountCurrencyID.Text = Dr["DPAmountCurrencyID"].ToString();
                    txtDPAmount.Text = Dr["DPAmount"].ToString();
                    txtCreditLimitCurrencyID.Text = Dr["CreditLimitCurrencyID"].ToString();
                    txtCreditLimit.Text = Dr["CreditLimit"].ToString();
                    txtCreditLimitPerSO.Text = Dr["CreditLimitPerSO"].ToString();
                    txtCashbackBalance.Text = Dr["CashbackBalance"].ToString();
                    txtDiscountBalance.Text = Dr["DiscountBalance"].ToString();
                    txtDebitNoteBalance.Text = Dr["DebitNoteBalance"].ToString();
                    txtPurchaseAmount.Text = Dr["PurchaseAmount"].ToString();
                    txtSOAmount.Text = Dr["SOAmount"].ToString();
                    txtDOAmount.Text = Dr["DOAmount"].ToString();
                    txtPaymentAmount.Text = Dr["PaymentAmount"].ToString();
                    txtChequeAmount.Text = Dr["ChequeAmount"].ToString();
                    txtEstablished.Text = Dr["EstablishedFor"].ToString();
                    txtSurvey.Text = Dr["Survey"].ToString();
                    
                }
                Dr.Close();
            }

            if (dgvAddress.DataSource == null)
            {
                MakeDataTableAddress();
            }

            if (dgvContact.DataSource == null)
            {
                MakeDataTableCP();
            }

            Query = "Select * From [dbo].[Address] where ReffId = '" + CustId +"'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    DataRow row = TableAddress.NewRow();
                    row["ID"] = Dr["RecId"].ToString();
                    row["PurposeType"] = Dr["PurposeType"].ToString();
                    row["Name"] = Dr["Name"].ToString();
                    row["Address"] = Dr["Address"].ToString();
                    row["ProvinceId"] = Dr["ProvinceId"].ToString();
                    row["CityId"] = Dr["CityId"].ToString();
                    row["AreaCode"] = Dr["AreaCode"].ToString();
                    row["RT"] = Dr["RT"].ToString();
                    row["RW"] = Dr["RW"].ToString();
                    row["Primary"] = Dr["PrimaryC"].ToString();

                    TableAddress.Rows.Add(row);
                }
                Dr.Close();
            }

            Query = "Select * From [dbo].[Contact] where ReffRecId = '"+ CustId +"'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    DataRow row = TableCP.NewRow();
                    row["ID"] = Dr["RecId"].ToString();
                    row["Deskripsi"] = Dr["Deskripsi"].ToString();
                    row["ContactType"] = Dr["ContactType"].ToString();
                    row["ContactNo"] = Dr["ContactNo"].ToString();
                    row["ExtNo"] = Dr["ExtNo"].ToString();
                    row["Primary"] = Dr["PrimaryC"].ToString();

                    TableCP.Rows.Add(row);
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID1.Text+ "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtItemDeskripsi1.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID2.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtItemDeskripsi2.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID3.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtItemDeskripsi3.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID4.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtItemDeskripsi4.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID5.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtItemDeskripsi5.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Conn.Close();
        }

        private void setDefaultLocation()
        {
            lblCustomer.Top = 40;
            grpCustomer.Top = lblCustomer.Location.Y - 6;

            lblTransaksi.Top = lblCustomer.Location.Y + 50;
            grpTransaksi.Top = lblTransaksi.Location.Y - 6;

            lblProduct.Top = lblTransaksi.Location.Y + 50;
            grpFullItemId.Top = lblProduct.Location.Y - 6;

            btnSave.Top = lblProduct.Location.Y + 50;
            btnEdit.Top = lblProduct.Location.Y + 50;
            btnCancel.Top = lblProduct.Location.Y + 50;
            btnExit.Top = lblProduct.Location.Y + 50;

            grpCustomer.Height = 0;
            grpTransaksi.Height = 0;
            grpFullItemId.Height = 0;

        }

        private void ModeLoad()
        {
            setDefaultLocation();
            RefreshGrid();
        }

        private void resetText()
        {
            txtCustId.Text = "";
            txtCustName.Text = "";
            txtTaxName.Text = "";
            txtNPWP.Text = "";
            txtPKP.Text = "";
            txtCompanyGroupId.Text = "";
            txtTempoBayar.Text = "";
            txtPaymentModeId.Text = "";
            txtTaxGroup.Text = "";
            txtReffBank1ID.Text = "";
            txtReffBank2ID.Text = "";
            txtReffFullItemID1.Text = "";
            txtReffFullItemID2.Text = "";
            txtReffFullItemID3.Text = "";
            txtReffFullItemID4.Text = "";
            txtReffFullItemID5.Text = "";
            txtCurrencyId.Text = "";
            
            txtCustGroupId.Text = "";
            txtDepositAmountAffiatedtoGroup.Text = "";
            txtDepositAmountCurrencyID.Text = "";
            txtDepositAmount.Text = "";
            txtDepositType.Text = "";
            txtDPAmountCurrencyID.Text = "";
            txtDPAmount.Text = "";
            txtCreditLimitCurrencyID.Text = "";
            txtCreditLimit.Text = "";
            txtCreditLimitPerSO.Text = "";
            txtCashbackBalance.Text = "";
            txtDiscountBalance.Text = "";
            txtDebitNoteBalance.Text = "";
            txtPurchaseAmount.Text = "";
            txtSOAmount.Text = "";
            txtDOAmount.Text = "";
            txtPaymentAmount.Text = "";
            txtChequeAmount.Text = "";
            txtEstablished.Text = "";
            txtSurvey.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtCustId.Enabled = false;
            txtCustName.Enabled = true;
            txtTaxName.Enabled = true;
            txtNPWP.Enabled = true;
            txtPKP.Enabled = true;
            txtCompanyGroupId.Enabled = true;
            txtTempoBayar.Enabled = true;
            txtPaymentModeId.Enabled = true;
            txtTaxGroup.Enabled = true;
            txtReffBank1ID.Enabled = true;
            txtReffBank2ID.Enabled = true;
            txtReffFullItemID1.Enabled = true;
            txtReffFullItemID2.Enabled = true;
            txtReffFullItemID3.Enabled = true;
            txtReffFullItemID4.Enabled = true;
            txtReffFullItemID5.Enabled = true;
            txtCurrencyId.Enabled = true;
            
            txtCustGroupId.Enabled = true;
            txtCreditLimitCurrencyID.Enabled = true;
            txtCreditLimit.Enabled = true;
            txtCreditLimitPerSO.Enabled = true;            
            txtEstablished.Enabled = true;
            txtSurvey.Enabled = true;

            btnAddAddr.Enabled = true;
            btnDeleteAddr.Enabled = true;
            
            dgvAddress.Enabled = true;

            btnAddC.Enabled = true;
            
            btnDeleteC.Enabled = true;
            dgvContact.Enabled = true;

            btnSearchPaymentMode.Enabled = true;
            btnSearchCurrency.Enabled = true;
            
            btnSearchVendorGroup.Enabled = true;
            btnSearchReffBank1.Enabled = true;
            btnSearchReffBank2.Enabled = true;
            btnSearchCreditLimitCurrency.Enabled = true;
            btnSearchDepositCurrency.Enabled = false;
            btnSearchDPCurrency.Enabled = false;
            btnSearchCompanyGroup.Enabled = true;

            btnSearchReffItemId1.Enabled = true;
            btnSearchReffItemId2.Enabled = true;
            btnSearchReffItemId3.Enabled = true;
            btnSearchReffItemId4.Enabled = true;
            btnSearchReffItemId5.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtCustId.Enabled = false;
            txtCustName.Enabled = true;
            txtTaxName.Enabled = true;
            txtNPWP.Enabled = true;
            txtPKP.Enabled = true;
            txtCompanyGroupId.Enabled = true;
            txtTempoBayar.Enabled = true;
            txtPaymentModeId.Enabled = true;
            txtTaxGroup.Enabled = true;
            txtReffBank1ID.Enabled = true;
            txtReffBank2ID.Enabled = true;
            txtReffFullItemID1.Enabled = true;
            txtReffFullItemID2.Enabled = true;
            txtReffFullItemID3.Enabled = true;
            txtReffFullItemID4.Enabled = true;
            txtReffFullItemID5.Enabled = true;
            txtCurrencyId.Enabled = true;
            
            txtCustGroupId.Enabled = true;
            txtCreditLimitCurrencyID.Enabled = true;
            txtCreditLimit.Enabled = true;
            txtCreditLimitPerSO.Enabled = true;
            txtSurvey.Enabled = true;
            txtEstablished.Enabled = true;

            btnAddAddr.Enabled = true;
            btnDeleteAddr.Enabled = true;
            
            dgvAddress.Enabled = true;

            btnAddC.Enabled = true;
            
            btnDeleteC.Enabled = true;
            dgvContact.Enabled = true;

            btnSearchPaymentMode.Enabled = true;
            btnSearchCurrency.Enabled = true;
            
            btnSearchVendorGroup.Enabled = true;
            btnSearchReffBank1.Enabled = true;
            btnSearchReffBank2.Enabled = true;
            btnSearchCreditLimitCurrency.Enabled = true;
            btnSearchDepositCurrency.Enabled = true;
            btnSearchDPCurrency.Enabled = true;
            btnSearchCompanyGroup.Enabled = true;

            btnSearchReffItemId1.Enabled = true;
            btnSearchReffItemId2.Enabled = true;
            btnSearchReffItemId3.Enabled = true;
            btnSearchReffItemId4.Enabled = true;
            btnSearchReffItemId5.Enabled = true;
            
            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = true;

            btnSave.Top = lblProduct.Location.Y + 50;
            btnCancel.Top = lblProduct.Location.Y + 50;
            btnEdit.Top = lblProduct.Location.Y + 50;
            btnExit.Top = lblProduct.Location.Y + 50;
        }

        private void ModeCancel()
        {
            txtCustId.Enabled = false;
            txtCustName.Enabled = false;
            txtTaxName.Enabled = false;
            txtNPWP.Enabled = false;
            txtPKP.Enabled = false;
            txtCompanyGroupId.Enabled = false;
            txtTempoBayar.Enabled = false;
            txtPaymentModeId.Enabled = false;
            txtTaxGroup.Enabled = false;
            txtReffBank1ID.Enabled = false;
            txtReffBank2ID.Enabled = false;
            txtReffFullItemID1.Enabled = false;
            txtReffFullItemID2.Enabled = false;
            txtReffFullItemID3.Enabled = false;
            txtReffFullItemID4.Enabled = false;
            txtReffFullItemID5.Enabled = false;
            txtCurrencyId.Enabled = false;
            
            txtCustGroupId.Enabled = false;
            txtDepositAmountAffiatedtoGroup.Enabled = false;
            txtDepositAmountCurrencyID.Enabled = false;
            txtDepositAmount.Enabled = false;
            txtDepositType.Enabled = false;
            txtDPAmountCurrencyID.Enabled = false;
            txtDPAmount.Enabled = false;
            txtCreditLimitCurrencyID.Enabled = false;
            txtCreditLimit.Enabled = false;
            txtCreditLimitPerSO.Enabled = false;
            txtCashbackBalance.Enabled = false;
            txtDiscountBalance.Enabled = false;
            txtDebitNoteBalance.Enabled = false;
            txtPurchaseAmount.Enabled = false;
            txtSOAmount.Enabled = false;
            txtDOAmount.Enabled = false;
            txtPaymentAmount.Enabled = false;
            txtChequeAmount.Enabled = false;
            txtSurvey.Enabled = false;
            txtEstablished.Enabled = false;

            btnAddAddr.Enabled = false;
            btnDeleteAddr.Enabled = false;

            dgvAddress.Enabled = false;

            btnAddC.Enabled = false;

            btnDeleteC.Enabled = false;
            dgvContact.Enabled = false;

            btnSearchPaymentMode.Enabled = false;
            btnSearchCurrency.Enabled = false;
            
            btnSearchVendorGroup.Enabled = false;
            btnSearchReffBank1.Enabled = false;
            btnSearchReffBank2.Enabled = false;
            btnSearchCreditLimitCurrency.Enabled = false;
            btnSearchDepositCurrency.Enabled = false;
            btnSearchDPCurrency.Enabled = false;
            btnSearchCompanyGroup.Enabled = false;

            btnSearchReffItemId1.Enabled = false;
            btnSearchReffItemId2.Enabled = false;
            btnSearchReffItemId3.Enabled = false;
            btnSearchReffItemId4.Enabled = false;
            btnSearchReffItemId5.Enabled = false;
            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;       
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeCancel();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtCustName.Text))
            {
                MessageBox.Show("Data harus diisi");
                return;
            }
            
            int TempoBayar = string.IsNullOrEmpty(txtTempoBayar.Text) ? 0 : int.Parse(txtTempoBayar.Text);
            decimal DepositAmountAffiatedtoGroup = string.IsNullOrEmpty(txtDepositAmountAffiatedtoGroup.Text) ? 0 : decimal.Parse(txtDepositAmountAffiatedtoGroup.Text);
            decimal DepositAmount = string.IsNullOrEmpty(txtDepositAmount.Text) ? 0 : decimal.Parse(txtDepositAmount.Text);
            decimal DPAmount = string.IsNullOrEmpty(txtDPAmount.Text) ? 0 : decimal.Parse(txtDPAmount.Text);
            decimal CreditLimit = string.IsNullOrEmpty(txtCreditLimit.Text) ? 0 : decimal.Parse(txtCreditLimit.Text);
            decimal CreditLimitPerSO = string.IsNullOrEmpty(txtCreditLimitPerSO.Text) ? 0 : decimal.Parse(txtCreditLimitPerSO.Text);
            decimal CashbackBalance = string.IsNullOrEmpty(txtCashbackBalance.Text) ? 0 : decimal.Parse(txtCashbackBalance.Text);
            decimal DiscountBalance = string.IsNullOrEmpty(txtDiscountBalance.Text) ? 0 : decimal.Parse(txtDiscountBalance.Text);
            decimal DebitNoteBalance = string.IsNullOrEmpty(txtDebitNoteBalance.Text) ? 0 : decimal.Parse(txtDebitNoteBalance.Text);
            decimal PurchaseAmount = string.IsNullOrEmpty(txtPurchaseAmount.Text) ? 0 : decimal.Parse(txtPurchaseAmount.Text);
            decimal SOAmount = string.IsNullOrEmpty(txtSOAmount.Text) ? 0 : decimal.Parse(txtSOAmount.Text);
            decimal DOAmount = string.IsNullOrEmpty(txtDOAmount.Text) ? 0 : decimal.Parse(txtDOAmount.Text);
            decimal PaymentAmount = string.IsNullOrEmpty(txtPaymentAmount.Text) ? 0 : decimal.Parse(txtPaymentAmount.Text);
            decimal ChequeAmount = string.IsNullOrEmpty(txtChequeAmount.Text) ? 0 : decimal.Parse(txtChequeAmount.Text);

            if (dgvAddress.Rows.Count == 0 || dgvContact.Rows.Count == 0)
            {
                MessageBox.Show("Isi Address&Contact dahulu");
            }
            else
            if (Mode == "New")
            {
                String TempId = GenerateId();

                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "insert into [dbo].[CustTable] (CustId, CustName, TaxName, NPWP, PKP, CompanyGroupId, TempoBayar, PaymentModeID, TaxGroup, ReffBank1ID, ";
                    Query += "ReffBank2ID, ReffFullItemID1, ReffFullItemID2, ReffFullItemID3, ReffFullItemID4, ReffFullItemID5, CurrencyID, ";
                    Query += "CustGroupID, DepositAmountAffiatedtoGroup, DepositAmountCurrencyID, DepositAmount, DepositType, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, ";
                    Query += "CreditLimitPerSO, CompanyStatusId, LastStatusChange, CashbackBalance, DiscountBalance, DebitNoteBalance, PurchaseAmount, SOAmount, DOAmount, PaymentAmount, ChequeAmount, EstablishedFor, Survey, CreatedDate, CreatedBy) ";
                    Query += "values ('" + TempId + "', '" + txtCustName.Text + "', '" + txtTaxName.Text + "', '" + txtNPWP.Text + "', '" + txtPKP.Text + "', '" + txtCompanyGroupId.Text + "', '" + txtTempoBayar.Text + "', '" + txtPaymentModeId.Text + "', '" + txtTaxGroup.Text + "', '" + txtReffBank1ID.Text + "', ";
                    Query += "'" + txtReffBank2ID.Text + "', '" + txtReffFullItemID1.Text + "', '" + txtReffFullItemID2.Text + "', '" + txtReffFullItemID3.Text + "', '" + txtReffFullItemID4.Text + "', '" + txtReffFullItemID5.Text + "', '" + txtCurrencyId.Text + "', ";
                    Query += "'" + txtCustGroupId.Text + "', '" + DepositAmountAffiatedtoGroup + "', '" + txtDepositAmountCurrencyID.Text + "', '" + DepositAmount + "', '" + txtDepositType.Text + "', '" + txtDPAmountCurrencyID.Text + "', '" + DPAmount + "', '" + txtCreditLimitCurrencyID.Text + "', '" + CreditLimit + "', ";
                    Query += "'" + CreditLimitPerSO + "', 'y', getdate(), '" + CashbackBalance + "', '" + DiscountBalance + "', '" + DebitNoteBalance + "', '" + PurchaseAmount + "', '" + SOAmount + "', '" + DOAmount + "', '" + PaymentAmount + "', '" + ChequeAmount + "', '"+ txtEstablished.Text +"', '"+ txtSurvey.Text +"', getDate(), '"+ Login.Username +"');";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }

                try
                {
                    if (dgvAddress.Rows.Count > 0)
                    {
                        int flagPrimaryAddress = 0;

                        for (int i = 0; i <= dgvAddress.RowCount - 1; i++)
                        {
                            String PurposeType = dgvAddress.Rows[i].Cells["PurposeType"].Value == null ? "" : dgvAddress.Rows[i].Cells["PurposeType"].Value.ToString();
                            String Name = dgvAddress.Rows[i].Cells["Name"].Value == null ? "" : dgvAddress.Rows[i].Cells["Name"].Value.ToString();
                            String Address = dgvAddress.Rows[i].Cells["Address"].Value == null ? "" : dgvAddress.Rows[i].Cells["Address"].Value.ToString();
                            String ProvinceId = dgvAddress.Rows[i].Cells["ProvinceId"].Value == null ? "" : dgvAddress.Rows[i].Cells["ProvinceId"].Value.ToString();
                            String CityId = dgvAddress.Rows[i].Cells["CityId"].Value == null ? "" : dgvAddress.Rows[i].Cells["CityId"].Value.ToString();
                            String AreaCode = dgvAddress.Rows[i].Cells["AreaCode"].Value == null ? "" : dgvAddress.Rows[i].Cells["AreaCode"].Value.ToString();
                            String RT = dgvAddress.Rows[i].Cells["RT"].Value == null ? "" : dgvAddress.Rows[i].Cells["RT"].Value.ToString();
                            String RW = dgvAddress.Rows[i].Cells["RW"].Value == null ? "" : dgvAddress.Rows[i].Cells["RW"].Value.ToString();
                            bool Primary;
                            if ((bool)dgvAddress.Rows[i].Cells["Primary"].Value == false)
                            {
                                Primary = false;
                            }
                            else
                            {
                                Primary = true;
                                flagPrimaryAddress++;
                            }

                            Query = "insert into [dbo].[Address] (ReffTableName, ReffID, PurposeType, Name, Address, ProvinceID, CityID, AreaCode, RT, RW, PrimaryC) ";
                            Query += "values ('CustTable', '" + TempId + "', '" + PurposeType + "', '" + Name + "', '" + Address + "', '" + ProvinceId + "', '" + CityId + "', '" + AreaCode + "', '" + RT + "', '" + RW + "', ";
                            Query += "'" + Primary + "');";

                            using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }

                        if (flagPrimaryAddress == 0)
                        {
                            Trans.Rollback();
                            MessageBox.Show("Primary Address harus dicentang");
                            return;
                        }
                        else if (flagPrimaryAddress > 1)
                        {
                            Trans.Rollback();
                            MessageBox.Show("Primary Address tidak boleh lebih dari 1");
                            return;
                        }
                    }
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }

                try
                {
                    if (dgvContact.Rows.Count > 0)
                    {
                        int flagPrimaryCP = 0;

                        for (int i = 0; i <= dgvContact.RowCount - 1; i++)
                        {
                            String Deskripsi = dgvContact.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvContact.Rows[i].Cells["Deskripsi"].Value.ToString();
                            String ContactType = dgvContact.Rows[i].Cells["ContactType"].Value == null ? "" : dgvContact.Rows[i].Cells["ContactType"].Value.ToString();
                            String ContactNo = dgvContact.Rows[i].Cells["ContactNo"].Value == null ? "" : dgvContact.Rows[i].Cells["ContactNo"].Value.ToString();
                            String ExtNo = dgvContact.Rows[i].Cells["ExtNo"].Value == null ? "" : dgvContact.Rows[i].Cells["ExtNo"].Value.ToString();

                            bool PrimaryCP;
                            if ((bool)dgvContact.Rows[i].Cells["Primary"].Value == false)
                            {
                                PrimaryCP = false;
                            }
                            else
                            {
                                PrimaryCP = true;
                                flagPrimaryCP++;
                            }
                            Query = "insert into [dbo].[Contact] (ReffTableName, ReffRecID, Deskripsi, ContactType, ContactNo, ExtNo, PrimaryC) ";
                            Query += "values ('CustTable', '" + TempId + "', '" + Deskripsi + "', '" + ContactType + "', '" + ContactNo + "', '" + ExtNo + "', ";
                            Query += "'" + PrimaryCP + "');";

                            using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Cmd.ExecuteNonQuery();
                            }

                        }

                        if (flagPrimaryCP == 0)
                        {
                            Trans.Rollback();
                            MessageBox.Show("Primary Contact Person harus dicentang");
                            return;
                        }
                        else if (flagPrimaryCP > 1)
                        {
                            Trans.Rollback();
                            MessageBox.Show("Primary Contact Person tidak boleh lebih dari 1");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + TempId + ", berhasil ditambahkan.");
                Parent.RefreshGrid();
                this.Close();
            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                //Conn2 = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "update [dbo].[CustTable] set CustName='" + txtCustName.Text + "', TaxName='" + txtTaxName.Text + "', NPWP='" + txtNPWP.Text + "', PKP='" + txtPKP.Text + "', ";
                    Query += "CompanyGroupId='" + txtCompanyGroupId.Text + "', TempoBayar='" + TempoBayar + "', PaymentModeID='" + txtPaymentModeId.Text + "', TaxGroup='" + txtTaxGroup.Text + "', ReffBank1ID='" + txtReffBank1ID.Text + "', ReffBank2ID='" + txtReffBank2ID.Text + "' ,";
                    Query += "ReffFullItemID1='" + txtReffFullItemID1.Text + "', ReffFullItemID2='" + txtReffFullItemID2.Text + "', ReffFullItemID3='" + txtReffFullItemID3.Text + "', ReffFullItemID4='" + txtReffFullItemID4.Text + "', ReffFullItemID5='" + txtReffFullItemID5.Text + "', CurrencyID='" + txtCurrencyId.Text + "', CustGroupID='" + txtCustGroupId.Text + "', ";
                    Query += "DepositAmountAffiatedtoGroup='" + DepositAmountAffiatedtoGroup + "', DepositAmountCurrencyID='" + txtDepositAmountCurrencyID.Text + "', DepositAmount='" + DepositAmount + "', DepositType='" + txtDepositType.Text + "', DPAmountCurrencyID='" + txtDPAmountCurrencyID.Text + "', DPAmount='" + DPAmount + "', ";
                    Query += "CreditLimitCurrencyID='" + txtCreditLimitCurrencyID.Text + "', CreditLimit='" + CreditLimit + "', CreditLimitPerSO='" + CreditLimitPerSO + "', CompanyStatusID ='y', LastStatusChange = getdate(), CashbackBalance='" + CashbackBalance + "', ";
                    Query += "DiscountBalance='" + DiscountBalance + "', DebitNoteBalance='" + DebitNoteBalance + "', PurchaseAmount='" + PurchaseAmount + "', SOAmount='" + SOAmount + "', DOAmount='" + DOAmount + "', PaymentAmount='" + PaymentAmount + "', ChequeAmount='" + ChequeAmount + "', EstablishedFor = '" + txtEstablished.Text + "', Survey = '" + txtSurvey.Text + "',UpdatedDate = getdate(), UpdatedBy = '" + Login.Username + "' ";
                    Query += "where CustId='" + txtCustId.Text + "';";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteScalar();
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }

                //try
                //{
                //    if (dgvAddress.Rows.Count > 0)
                //    {
                //        int flagPrimaryAddress = 0;

                //        for (int i = 0; i <= dgvAddress.RowCount - 1; i++)
                //        {
                //            int RecId = String.IsNullOrEmpty(dgvAddress.Rows[i].Cells["ID"].Value.ToString()) ? 0 : Int32.Parse(dgvAddress.Rows[i].Cells["ID"].Value.ToString());
                //            String PurposeType = dgvAddress.Rows[i].Cells["PurposeType"].Value == null ? "" : dgvAddress.Rows[i].Cells["PurposeType"].Value.ToString();
                //            String Name = dgvAddress.Rows[i].Cells["Name"].Value == null ? "" : dgvAddress.Rows[i].Cells["Name"].Value.ToString();
                //            String Address = dgvAddress.Rows[i].Cells["Address"].Value == null ? "" : dgvAddress.Rows[i].Cells["Address"].Value.ToString();
                //            String ProvinceId = dgvAddress.Rows[i].Cells["ProvinceId"].Value == null ? "" : dgvAddress.Rows[i].Cells["ProvinceId"].Value.ToString();
                //            String CityId = dgvAddress.Rows[i].Cells["CityId"].Value == null ? "" : dgvAddress.Rows[i].Cells["CityId"].Value.ToString();
                //            String AreaCode = dgvAddress.Rows[i].Cells["AreaCode"].Value == null ? "" : dgvAddress.Rows[i].Cells["AreaCode"].Value.ToString();
                //            String RT = dgvAddress.Rows[i].Cells["RT"].Value == null ? "" : dgvAddress.Rows[i].Cells["RT"].Value.ToString();
                //            String RW = dgvAddress.Rows[i].Cells["RW"].Value == null ? "" : dgvAddress.Rows[i].Cells["RW"].Value.ToString();
                //            bool Primary;
                //            if ((bool)dgvAddress.Rows[i].Cells["Primary"].Value == false)
                //            {
                //                Primary = false;
                //            }
                //            else
                //            {
                //                Primary = true;
                //                flagPrimaryAddress++;
                //            }

                //            Query = "Select * From [dbo].[Address] Where ReffId = '" + CustId + "' And RecId = '" + RecId + "'";
                //            using (SqlCommand cmd = new SqlCommand(Query, Conn2))
                //            {
                //                Dr = cmd.ExecuteReader();

                //                if (Dr.Read())
                //                {
                //                    Query = "update [dbo].[Address] set PurposeType='" + PurposeType + "', Name='" + Name + "', Address='" + Address + "', ProvinceID='" + ProvinceId + "', ";
                //                    Query += "CityID='" + CityId + "', AreaCode='" + AreaCode + "', RT='" + RT + "', RW='" + RW + "', PrimaryC='" + Primary + "' ";
                //                    Query += "where ReffId='" + txtCustId.Text + "' And RecId='" + RecId + "';";

                //                    Cmd = new SqlCommand(Query, Conn, Trans);
                //                    Cmd.ExecuteScalar();
                //                }
                //                else
                //                {
                //                    Query = "insert into [dbo].[Address] (ReffTableName, ReffID, PurposeType, Name, Address, ProvinceID, CityID, AreaCode, RT, RW, PrimaryC) ";
                //                    Query += "values ('CustTable', '" + CustId + "', '" + PurposeType + "', '" + Name + "', '" + Address + "', '" + ProvinceId + "', '" + CityId + "', '" + AreaCode + "', '" + RT + "', '" + RW + "', ";
                //                    Query += "'" + Primary + "');";

                //                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                //                    {
                //                        Cmd.ExecuteNonQuery();
                //                    }
                //                }
                //                Dr.Close();
                //            }
                //        }

                //        if (flagPrimaryAddress == 0)
                //        {
                //            MessageBox.Show("Primary Address harus dicentang");
                //            Trans.Rollback();
                //            return;
                //        }
                //        else if (flagPrimaryAddress > 1)
                //        {
                //            MessageBox.Show("Primary Address tidak boleh lebih dari 1");
                //            Trans.Rollback();
                //            return;
                //        }
                //    }
                //}
                //catch (Exception x)
                //{
                //    MessageBox.Show(x.Message);
                //    Trans.Rollback();
                //    return;
                //}

                //try
                //{
                //    if (dgvContact.Rows.Count > 0)
                //    {
                //        int flagPrimaryCP = 0;

                //        for (int i = 0; i <= dgvContact.RowCount - 1; i++)
                //        {
                //            int RecId = String.IsNullOrEmpty(dgvContact.Rows[i].Cells["ID"].Value.ToString()) ? 0 : Int32.Parse(dgvContact.Rows[i].Cells["ID"].Value.ToString());
                //            String Deskripsi = dgvContact.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvContact.Rows[i].Cells["Deskripsi"].Value.ToString();
                //            String ContactType = dgvContact.Rows[i].Cells["ContactType"].Value == null ? "" : dgvContact.Rows[i].Cells["ContactType"].Value.ToString();
                //            String ContactNo = dgvContact.Rows[i].Cells["ContactNo"].Value == null ? "" : dgvContact.Rows[i].Cells["ContactNo"].Value.ToString();
                //            String ExtNo = dgvContact.Rows[i].Cells["ExtNo"].Value == null ? "" : dgvContact.Rows[i].Cells["ExtNo"].Value.ToString();
                //            bool PrimaryCP;
                //            if ((bool)dgvContact.Rows[i].Cells["Primary"].Value == false)
                //            {
                //                PrimaryCP = false;
                //            }
                //            else
                //            {
                //                PrimaryCP = true;
                //                flagPrimaryCP++;
                //            }

                //            Query = "Select * From [dbo].[Contact] Where ReffRecId = '" + CustId + "' And RecId = '" + RecId + "'";
                //            using (SqlCommand cmd = new SqlCommand(Query, Conn2))
                //            {
                //                Dr = cmd.ExecuteReader();

                //                if (Dr.Read())
                //                {
                //                    Query = "update [dbo].[Contact] set Deskripsi='" + Deskripsi + "', ContactType='" + ContactType + "', ContactNo='" + ContactNo + "', ExtNo='" + ExtNo + "', ";
                //                    Query += "PrimaryC='" + PrimaryCP + "' ";
                //                    Query += "where ReffRecId='" + CustId + "' And RecId='" + RecId + "';";

                //                    Cmd = new SqlCommand(Query, Conn, Trans);
                //                    Cmd.ExecuteScalar();
                //                }
                //                else
                //                {
                //                    Query = "insert into [dbo].[Contact] (ReffTableName, ReffRecID, Deskripsi, ContactType, ContactNo, ExtNo, PrimaryC) ";
                //                    Query += "values ('CustTable', '" + CustId + "', '" + Deskripsi + "', '" + ContactType + "', '" + ContactNo + "', '" + ExtNo + "', ";
                //                    Query += "'" + PrimaryCP + "');";

                //                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                //                    {
                //                        Cmd.ExecuteNonQuery();
                //                    }
                //                }
                //                Dr.Close();
                //            }
                //        }

                //        if (flagPrimaryCP == 0)
                //        {
                //            MessageBox.Show("Primary Contact Person harus dicentang");
                //            Trans.Rollback();
                //            return;
                //        }
                //        else if (flagPrimaryCP > 1)
                //        {
                //            MessageBox.Show("Primary Contact Person tidak boleh lebih dari 1");
                //            Trans.Rollback();
                //            return;
                //        }
                //    }
                //}
                //catch (Exception x)
                //{
                //    MessageBox.Show(x.Message);
                //    Trans.Rollback();
                //    return;
                //}

                try
                {
                    if (dgvAddress.Rows.Count > 0)
                    {
                        int flagPrimaryAddress = 0;

                        Query1 = "Delete from [dbo].[Address] where ReffTableName= 'CustTable' and ReffID = '" + txtCustId.Text.Trim() + "';";
                        Cmd = new SqlCommand(Query1, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        for (int i = 0; i <= dgvAddress.RowCount - 1; i++)
                        {
                            int RecId = String.IsNullOrEmpty(dgvAddress.Rows[i].Cells["ID"].Value.ToString()) ? 0 : Int32.Parse(dgvAddress.Rows[i].Cells["ID"].Value.ToString());
                            String PurposeType = dgvAddress.Rows[i].Cells["PurposeType"].Value == null ? "" : dgvAddress.Rows[i].Cells["PurposeType"].Value.ToString();
                            String Name = dgvAddress.Rows[i].Cells["Name"].Value == null ? "" : dgvAddress.Rows[i].Cells["Name"].Value.ToString();
                            String Address = dgvAddress.Rows[i].Cells["Address"].Value == null ? "" : dgvAddress.Rows[i].Cells["Address"].Value.ToString();
                            String ProvinceId = dgvAddress.Rows[i].Cells["ProvinceId"].Value == null ? "" : dgvAddress.Rows[i].Cells["ProvinceId"].Value.ToString();
                            String CityId = dgvAddress.Rows[i].Cells["CityId"].Value == null ? "" : dgvAddress.Rows[i].Cells["CityId"].Value.ToString();
                            String AreaCode = dgvAddress.Rows[i].Cells["AreaCode"].Value == null ? "" : dgvAddress.Rows[i].Cells["AreaCode"].Value.ToString();
                            String RT = dgvAddress.Rows[i].Cells["RT"].Value == null ? "" : dgvAddress.Rows[i].Cells["RT"].Value.ToString();
                            String RW = dgvAddress.Rows[i].Cells["RW"].Value == null ? "" : dgvAddress.Rows[i].Cells["RW"].Value.ToString();
                            bool Primary;
                            if ((bool)dgvAddress.Rows[i].Cells["Primary"].Value == false)
                            {
                                Primary = false;
                            }
                            else
                            {
                                Primary = true;
                                flagPrimaryAddress++;
                            }

                            Query = "insert into [dbo].[Address] (ReffTableName, ReffID, PurposeType, Name, Address, ProvinceID, CityID, AreaCode, RT, RW, PrimaryC) ";
                            Query += "values ('CustTable', '" + CustId + "', '" + PurposeType + "', '" + Name + "', '" + Address + "', '" + ProvinceId + "', '" + CityId + "', '" + AreaCode + "', '" + RT + "', '" + RW + "', ";
                            Query += "'" + Primary + "');";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            if (flagPrimaryAddress == 0)
                            {
                                Trans.Rollback();
                                MessageBox.Show("Primary Address harus dicentang");
                                return;
                            }
                            else if (flagPrimaryAddress > 1)
                            {
                                Trans.Rollback();
                                MessageBox.Show("Primary Address tidak boleh lebih dari 1");
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }

                try
                {
                    if (dgvContact.Rows.Count > 0)
                    {
                        int flagPrimaryCP = 0;

                        Query1 = "Delete from [dbo].[Contact] where ReffTableName= 'CustTable' and ReffRecID = '" + txtCustId.Text.Trim() + "';";
                        Cmd = new SqlCommand(Query1, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        for (int i = 0; i <= dgvContact.RowCount - 1; i++)
                        {
                            int RecId = String.IsNullOrEmpty(dgvContact.Rows[i].Cells["ID"].Value.ToString()) ? 0 : Int32.Parse(dgvContact.Rows[i].Cells["ID"].Value.ToString());
                            String Deskripsi = dgvContact.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvContact.Rows[i].Cells["Deskripsi"].Value.ToString();
                            String ContactType = dgvContact.Rows[i].Cells["ContactType"].Value == null ? "" : dgvContact.Rows[i].Cells["ContactType"].Value.ToString();
                            String ContactNo = dgvContact.Rows[i].Cells["ContactNo"].Value == null ? "" : dgvContact.Rows[i].Cells["ContactNo"].Value.ToString();
                            String ExtNo = dgvContact.Rows[i].Cells["ExtNo"].Value == null ? "" : dgvContact.Rows[i].Cells["ExtNo"].Value.ToString();
                            bool PrimaryCP;
                            if ((bool)dgvContact.Rows[i].Cells["Primary"].Value == false)
                            {
                                PrimaryCP = false;
                            }
                            else
                            {
                                PrimaryCP = true;
                                flagPrimaryCP++;
                            }

                            Query = "insert into [dbo].[Contact] (ReffTableName, ReffRecID, Deskripsi, ContactType, ContactNo, ExtNo, PrimaryC) ";
                            Query += "values ('CustTable', '" + CustId + "', '" + Deskripsi + "', '" + ContactType + "', '" + ContactNo + "', '" + ExtNo + "', ";
                            Query += "'" + PrimaryCP + "');";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            if (flagPrimaryCP == 0)
                            {
                                Trans.Rollback();
                                MessageBox.Show("Primary Contact Person harus dicentang");
                                return;
                            }
                            else if (flagPrimaryCP > 1)
                            {
                                Trans.Rollback();
                                MessageBox.Show("Primary Contact Person tidak boleh lebih dari 1");
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + txtCustId.Text + ", berhasil diupdate.");
                Parent.RefreshGrid();
                this.Close();
            }
        }

        private void lblCustomer_Click(object sender, EventArgs e)
        {
            if (flagCustomer == false)//buka
            {
                grpCustomer.Height = 454;
                grpCustomer.Top = lblCustomer.Location.Y - 6;

                if (flagTransaksi == false)
                {
                    lblTransaksi.Top = grpCustomer.Bottom + 10;
                    lblProduct.Top = lblTransaksi.Location.Y + 50;
                    grpFullItemId.Top = lblProduct.Location.Y - 6;
                }
                else
                {
                    lblTransaksi.Top = grpCustomer.Bottom + 10;
                    grpTransaksi.Top = lblTransaksi.Location.Y - 6;
                    lblProduct.Top = grpTransaksi.Bottom + 10;
                    if (flagProduct == true)
                    {
                        grpFullItemId.Top = lblProduct.Location.Y - 6;
                    }
                }
                flagCustomer = true;
            }
            else
            { // tutup
                grpCustomer.Height = 0;
                lblTransaksi.Top = lblCustomer.Location.Y + 50;

                if (flagTransaksi == false)
                {
                    lblProduct.Top = lblTransaksi.Location.Y + 50;
                    grpFullItemId.Top = lblProduct.Location.Y - 6;
                }
                else
                {
                    grpTransaksi.Top = lblTransaksi.Location.Y - 6;
                    lblProduct.Top = grpTransaksi.Bottom + 10;
                    if (flagProduct == true)
                    {
                        grpFullItemId.Top = lblProduct.Location.Y - 6;
                    }
                }
                flagCustomer = false;
            }
            btnSave.Top = lblProduct.Location.Y + 50;
            btnCancel.Top = lblProduct.Location.Y + 50;
            btnEdit.Top = lblProduct.Location.Y + 50;
            btnExit.Top = lblProduct.Location.Y + 50;
        }

        private void lblTransaksi_Click(object sender, EventArgs e)
        {
            if (flagTransaksi == false)
            {
                grpTransaksi.Height = 200;
                grpTransaksi.Top = lblTransaksi.Location.Y - 6;
                lblProduct.Top = grpTransaksi.Bottom + 10;

                flagTransaksi = true;
            }
            else
            {
                grpTransaksi.Height = 0;
                lblProduct.Top = lblTransaksi.Location.Y + 50;

                if (flagProduct == true)
                {
                    grpFullItemId.Top = lblProduct.Location.Y - 6;
                }
                flagTransaksi = false;
            }

            if (flagProduct == true)
            {
                grpFullItemId.Top = lblProduct.Location.Y - 6;
            }
            btnSave.Top = lblProduct.Location.Y + 50;
            btnCancel.Top = lblProduct.Location.Y + 50;
            btnEdit.Top = lblProduct.Location.Y + 50;
            btnExit.Top = lblProduct.Location.Y + 50;
        }

        private void lblProduct_Click(object sender, EventArgs e)
        {
            if (flagProduct == false)
            {
                grpFullItemId.Height = 163;
                grpFullItemId.Top = lblProduct.Location.Y - 6;
                flagProduct = true;
            }
            else
            {
                
                grpFullItemId.Height = 0;
                flagProduct = false;
            }
            btnSave.Top = lblProduct.Location.Y + 50;
            btnCancel.Top = lblProduct.Location.Y + 50;
            btnEdit.Top = lblProduct.Location.Y + 50;
            btnExit.Top = lblProduct.Location.Y + 50;
        }

        private void btnSearchPaymentMode_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "PaymentMode";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtPaymentModeId.Text = ConnectionString.Kode;
        }

        private void btnSearchCurrency_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtCurrencyId.Text = ConnectionString.Kode;
        }

        //private void btnReffCustId_Click(object sender, EventArgs e)
        //{
        //    string SchemaName = "dbo";
        //    string TableName = "CustTable";

        //    Search tmpSearch = new Search();
        //    tmpSearch.SetSchemaTable(SchemaName, TableName);
        //    tmpSearch.ShowDialog();
        //    txtReffCustId.Text = ConnectionString.Kode;
        //}

        private void btnSearchCustGroup_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CustGroup";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtCustGroupId.Text = ConnectionString.Kode;
        }

        private void btnSearchReffBank1_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "BankTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffBank1ID.Text = ConnectionString.Kode;
        }

        private void btnSearchReffBank2_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "BankTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffBank2ID.Text = ConnectionString.Kode;
        }

        private void btnSearchCreditLimitCurrency_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtCreditLimitCurrencyID.Text = ConnectionString.Kode;
        }

        private void btnSearchDepositCurrency_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtDepositAmountCurrencyID.Text = ConnectionString.Kode;
        }

        private void btnSearchDPCurrency_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtDPAmountCurrencyID.Text = ConnectionString.Kode;
        }

        private void btnSearchReffItemId1_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID1.Text = ConnectionString.Kode;
            txtItemDeskripsi1.Text = ConnectionString.Kode2;
        }

        private void btnSearchReffItemId2_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID2.Text = ConnectionString.Kode;
            txtItemDeskripsi2.Text = ConnectionString.Kode2;
        }

        private void btnSearchReffItemId3_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID3.Text = ConnectionString.Kode;
            txtItemDeskripsi3.Text = ConnectionString.Kode2;
        }

        private void btnSearchReffItemId4_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID4.Text = ConnectionString.Kode;
            txtItemDeskripsi4.Text = ConnectionString.Kode2;
        }

        private void btnSearchReffItemId5_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID5.Text = ConnectionString.Kode;
            txtItemDeskripsi5.Text = ConnectionString.Kode2;
        }

        private void txtVendName_Validating(object sender, CancelEventArgs e)
        {
            if (txtCustName.Text == "")
            {

            }
            else
            {
                if (Mode == "New")
                {
                    Query = "Select * From [dbo].[CustTable] Where REPLACE(CustName,' ','') = '" + txtCustName.Text.Replace(" ", "") + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        if (Dr.Read())//sama dengan di database
                        {
                            MessageBox.Show("Nama sudah ada di database.");
                            txtCustName.Focus();
                            Conn.Close();
                        }
                        else
                        {
                            Conn.Close();
                        }
                    }
                }
            }
        }

        private void txtPaymentModeId_Validating(object sender, CancelEventArgs e)
        {
            if (txtPaymentModeId.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[PaymentMode] Where PaymentModeId = '" + txtPaymentModeId.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Payment mode tidak ada di database.");
                        btnSearchPaymentMode_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtCurrencyId_Validating(object sender, CancelEventArgs e)
        {
            if (txtCurrencyId.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + txtCurrencyId.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kurs tidak ada di database.");
                        btnSearchCurrency_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        //private void txtReffCustId_Validating(object sender, CancelEventArgs e)
        //{
        //    if (txtReffCustId.Text == "")
        //    {

        //    }
        //    else
        //    {
        //        Query = "Select * From [dbo].[VendTable] Where CustId = '" + txtReffCustId.Text + "'";

        //        Conn = ConnectionString.GetConnection();
        //        using (SqlCommand cmd = new SqlCommand(Query, Conn))
        //        {
        //            Dr = cmd.ExecuteReader();

        //            if (Dr.Read())//sama dengan di database
        //            {
        //                Conn.Close();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Vendor ID tidak ada di database.");
        //                btnReffCustId_Click(sender, e);
        //                Conn.Close();
        //            }
        //        }
        //    }
        //}

        private void txtVendGroupId_Validating(object sender, CancelEventArgs e)
        {
            if (txtCustGroupId.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CustGroup] Where CustGroupID = '" + txtCustGroupId.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Vendor Group ID tidak ada di database.");
                        btnSearchCustGroup_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtReffBank1ID_Validating(object sender, CancelEventArgs e)
        {
            if (txtReffBank1ID.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[BankTable] Where BankId = '" + txtReffBank1ID.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Bank ID tidak ada di database.");
                        btnSearchReffBank1_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtReffBank2ID_Validating(object sender, CancelEventArgs e)
        {
            if (txtReffBank2ID.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[BankTable] Where BankId = '" + txtReffBank2ID.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Bank ID tidak ada di database.");
                        btnSearchReffBank2_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtCreditLimitCurrencyID_Validating(object sender, CancelEventArgs e)
        {
            if (txtCreditLimitCurrencyID.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + txtCreditLimitCurrencyID.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kurs tidak ada di database.");
                        btnSearchCreditLimitCurrency_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtDepositAmountCurrencyID_Validating(object sender, CancelEventArgs e)
        {
            if (txtDepositAmountCurrencyID.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + txtDepositAmountCurrencyID.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kurs tidak ada di database.");
                        btnSearchDepositCurrency_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtDPAmountCurrencyID_Validating(object sender, CancelEventArgs e)
        {
            if (txtDPAmountCurrencyID.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + txtDPAmountCurrencyID.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Kurs tidak ada di database.");
                        btnSearchDPCurrency_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtReffFullItemID1_Validating(object sender, CancelEventArgs e)
        {
            if (txtReffFullItemID1.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[InventTable] Where FullItemId = '" + txtReffFullItemID1.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Item ID tidak ada di database.");
                        btnSearchReffItemId1_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtReffFullItemID2_Validating(object sender, CancelEventArgs e)
        {
            if (txtReffFullItemID2.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[InventTable] Where FullItemId = '" + txtReffFullItemID2.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Item ID tidak ada di database.");
                        btnSearchReffItemId2_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtReffFullItemID3_Validating(object sender, CancelEventArgs e)
        {
            if (txtReffFullItemID3.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[InventTable] Where FullItemId = '" + txtReffFullItemID3.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Item ID tidak ada di database.");
                        btnSearchReffItemId3_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtReffFullItemID4_Validating(object sender, CancelEventArgs e)
        {
            if (txtReffFullItemID4.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[InventTable] Where FullItemId = '" + txtReffFullItemID4.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Item ID tidak ada di database.");
                        btnSearchReffItemId4_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtReffFullItemID5_Validating(object sender, CancelEventArgs e)
        {
            if (txtReffFullItemID5.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[InventTable] Where FullItemId = '" + txtReffFullItemID5.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Item ID tidak ada di database.");
                        btnSearchReffItemId5_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void btnAddAddr_Click(object sender, EventArgs e)
        {
            Address.AddressForm F = new Address.AddressForm();
            F.flag("", "New");
            F.Show();
            F.setParentForm2(this);
            
        }

        private void btnAddC_Click(object sender, EventArgs e)
        {
            Address.ContactForm F = new Address.ContactForm();
            F.flag("", "New");
            F.Show();
            F.setParentForm2(this);
        }

        private void btnDeleteAddr_Click(object sender, EventArgs e)
        {
            //if (Mode == "New")
            //{
            //    if (dgvAddress.RowCount > 0)
            //    {
            //        Index = dgvAddress.CurrentRow.Index;

            //        DialogResult dr = MessageBox.Show("Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            //        if (dr == DialogResult.Yes)
            //        {
            //            dgvAddress.Rows.RemoveAt(Index);
            //            Index = 0;
            //        }

            //    }
            //}
            //else if (Mode == "Edit")
            //{
            //    try
            //    {
            //        if (dgvAddress.RowCount > 0)
            //        {
            //            Conn2 = ConnectionString.GetConnection();
            //            Index = dgvAddress.CurrentRow.Index;
            //            String RecId = dgvAddress.Rows[Index].Cells["ID"].Value == null ? "" : dgvAddress.Rows[Index].Cells["ID"].Value.ToString();

            //            Query = "Select * From [dbo].[Address] Where ReffId = '" + CustId + "' And RecId = '" + RecId + "'";
            //            using (SqlCommand cmd = new SqlCommand(Query, Conn2))
            //            {
            //                Dr = cmd.ExecuteReader();

            //                if (Dr.Read())
            //                {
            //                    DialogResult dr = MessageBox.Show("Data ini sudah tercatat di dalam Database. Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            //                    if (dr == DialogResult.Yes)
            //                    {
            //                        Conn = ConnectionString.GetConnection();
            //                        Query = "Delete from [dbo].[Address] where ReffId = '" + CustId + "' And RecId = '" + RecId + "'";

            //                        Cmd = new SqlCommand(Query, Conn);
            //                        Cmd.ExecuteNonQuery();

            //                        MessageBox.Show("Data berhasil dihapus.");

            //                        Index = 0;
            //                        Conn.Close();
            //                        dgvContact.Rows.RemoveAt(Index);
            //                    }
            //                }
            //                else
            //                {
            //                    DialogResult dr = MessageBox.Show("Data ini belum tercatat di dalam Database. Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            //                    if (dr == DialogResult.Yes)
            //                    {
            //                        dgvAddress.Rows.RemoveAt(Index);
            //                        Index = 0;
            //                    }
            //                }
            //                Dr.Close();
            //            }
            //        }
            //    }
            //    catch (Exception exx)
            //    {
            //        MessageBox.Show(exx.Message);
            //    }
            //}

            if (dgvAddress.RowCount > 0)
            {
                Index = dgvAddress.CurrentRow.Index;

                DialogResult dr = MessageBox.Show("Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    dgvAddress.Rows.RemoveAt(Index);
                    Index = 0;
                }

            }

        }

        private void btnDeleteC_Click(object sender, EventArgs e)
        {
            //if (Mode == "New")
            //{
            //    if (dgvContact.RowCount > 0)
            //    {
            //        Index = dgvContact.CurrentRow.Index;

            //        DialogResult dr = MessageBox.Show("Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            //        if (dr == DialogResult.Yes)
            //        {
            //            dgvContact.Rows.RemoveAt(Index);
            //            Index = 0;
            //        }

            //    }
            //}
            //else if (Mode == "Edit")
            //{
            //    try
            //    {
            //        if (dgvContact.RowCount > 0)
            //        {
            //            Conn2 = ConnectionString.GetConnection();
            //            Index = dgvContact.CurrentRow.Index;
            //            String RecId = dgvContact.Rows[Index].Cells["ID"].Value == null ? "" : dgvContact.Rows[Index].Cells["ID"].Value.ToString();

            //            Query = "Select * From [dbo].[Contact] Where ReffRecId = '" + CustId + "' And RecId = '" + RecId + "'";
            //            using (SqlCommand cmd = new SqlCommand(Query, Conn2))
            //            {
            //                Dr = cmd.ExecuteReader();

            //                if (Dr.Read())
            //                {
            //                    DialogResult dr = MessageBox.Show("Data ini sudah tercatat di dalam Database. Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            //                    if (dr == DialogResult.Yes)
            //                    {
            //                        Conn = ConnectionString.GetConnection();
            //                        Query = "Delete from [dbo].[Contact] where ReffRecId = '" + CustId + "' And RecId = '" + RecId + "'";

            //                        Cmd = new SqlCommand(Query, Conn);
            //                        Cmd.ExecuteNonQuery();

            //                        MessageBox.Show("Data berhasil dihapus.");

            //                        Index = 0;
            //                        Conn.Close();
            //                        dgvContact.Rows.RemoveAt(Index);
            //                    }
            //                }
            //                else
            //                {
            //                    DialogResult dr = MessageBox.Show("Data ini belum tercatat di dalam Database. Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            //                    if (dr == DialogResult.Yes)
            //                    {
            //                        dgvContact.Rows.RemoveAt(Index);
            //                        Index = 0;
            //                    }
            //                }
            //                Dr.Close();
            //            }
            //        }
            //    }
            //    catch (Exception exx)
            //    {
            //        MessageBox.Show(exx.Message);
            //    }
            //}

            if (dgvContact.RowCount > 0)
            {
                Index = dgvContact.CurrentRow.Index;

                DialogResult dr = MessageBox.Show("Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    dgvContact.Rows.RemoveAt(Index);
                    Index = 0;
                }
            }
        }

        private void dgvAddress_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvAddress.Columns[dgvAddress.CurrentCell.ColumnIndex].Name == "PurposeType")
            {
                if ((String)dgvAddress.CurrentRow.Cells["PurposeType"].Value == String.Empty)
                {

                }
                else
                {
                    Query = "Select * From [dbo].[AddressType] Where PurposeType = '" + dgvAddress.CurrentRow.Cells["PurposeType"].Value.ToString() + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        if (Dr.Read())//sama dengan di database
                        {
                            Conn.Close();
                        }
                        else
                        {
                            MessageBox.Show("Purpose Type tidak ada di database.");
                            Conn.Close();
                            dgvAddress.CurrentRow.Cells["PurposeType"].Value = "";

                            string SchemaName = "dbo";
                            string TableName = "AddressType";

                            Search tmpSearch = new Search();
                            tmpSearch.SetSchemaTable(SchemaName, TableName);
                            tmpSearch.ShowDialog();
                            dgvAddress.CurrentRow.Cells["PurposeType"].Value = ConnectionString.Kode;
                        }
                    }
                }
            }

            if (dgvAddress.Columns[dgvAddress.CurrentCell.ColumnIndex].Name == "ProvinceId")
            {
                if ((String)dgvAddress.CurrentRow.Cells["ProvinceId"].Value == String.Empty)
                {

                }
                else
                {
                    Query = "Select * From [dbo].[Province] Where ProvinceId = '" + dgvAddress.CurrentRow.Cells["ProvinceId"].Value.ToString() + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        if (Dr.Read())//sama dengan di database
                        {
                            Conn.Close();
                        }
                        else
                        {
                            MessageBox.Show("Province ID tidak ada di database.");
                            Conn.Close();
                            dgvAddress.CurrentRow.Cells["ProvinceId"].Value = "";

                            string SchemaName = "dbo";
                            string TableName = "Province";

                            Search tmpSearch = new Search();
                            tmpSearch.SetSchemaTable(SchemaName, TableName);
                            tmpSearch.ShowDialog();
                            dgvAddress.CurrentRow.Cells["ProvinceId"].Value = ConnectionString.Kode;
                        }
                    }
                }
            }

            if (dgvAddress.Columns[dgvAddress.CurrentCell.ColumnIndex].Name == "CityId")
            {
                if ((String)dgvAddress.CurrentRow.Cells["CityId"].Value == String.Empty)
                {

                }
                else
                {
                    Query = "Select * From [dbo].[City] Where CityId = '" + dgvAddress.CurrentRow.Cells["CityId"].Value.ToString() + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        if (Dr.Read())//sama dengan di database
                        {
                            Conn.Close();
                        }
                        else
                        {
                            MessageBox.Show("City ID tidak ada di database.");
                            Conn.Close();
                            dgvAddress.CurrentRow.Cells["CityId"].Value = "";

                            string SchemaName = "dbo";
                            string TableName = "City";

                            Search tmpSearch = new Search();
                            tmpSearch.SetSchemaTable(SchemaName, TableName);
                            tmpSearch.ShowDialog();
                            dgvAddress.CurrentRow.Cells["CityId"].Value = ConnectionString.Kode;
                        }
                    }
                }
            }
        }

        private void dgvContact_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvContact.Columns[dgvContact.CurrentCell.ColumnIndex].Name == "ContactType")
            {
                if ((String)dgvContact.CurrentRow.Cells["ContactType"].Value == String.Empty)
                {

                }
                else
                {
                    Query = "Select * From [dbo].[ContactType] Where ContactType = '" + dgvContact.CurrentRow.Cells["ContactType"].Value.ToString() + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        if (Dr.Read())//sama dengan di database
                        {
                            Conn.Close();
                        }
                        else
                        {
                            MessageBox.Show("Contact Type tidak ada di database.");
                            Conn.Close();
                            dgvContact.CurrentRow.Cells["ContactType"].Value = "";

                            string SchemaName = "dbo";
                            string TableName = "ContactType";

                            Search tmpSearch = new Search();
                            tmpSearch.SetSchemaTable(SchemaName, TableName);
                            tmpSearch.ShowDialog();
                            dgvContact.CurrentRow.Cells["ContactType"].Value = ConnectionString.Kode;
                        }
                    }
                }
            }
        }

        private void txtTempoBayar_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtTempoBayar.Text))
            {

            }
            else
            {
                MessageBox.Show("Tempo Bayar Tidak boleh di isi selain angka!!");
                txtTempoBayar.Focus();
                txtTempoBayar.Clear();
            }
        }

        private void txtDepositAmountAffiatedtoGroup_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtDepositAmountAffiatedtoGroup.Text))
            {

            }
            else
            {
                MessageBox.Show("Deposit Amount Affiated Group Tidak boleh di isi selain angka!!");
                txtDepositAmountAffiatedtoGroup.Focus();
                txtDepositAmountAffiatedtoGroup.Clear();
            }
        }

        private void txtDepositAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtDepositAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("Deposit Amount Tidak boleh di isi selain angka!!");
                txtDepositAmount.Focus();
                txtDepositAmount.Clear();
            }
        }

        private void txtCreditLimitPerSO_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtCreditLimitPerSO.Text))
            {

            }
            else
            {
                MessageBox.Show("Credit LimitPerSO Tidak boleh di isi selain angka!!");
                txtCreditLimitPerSO.Focus();
                txtCreditLimitPerSO.Clear();
            }
        }

        private void txtCreditLimit_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtCreditLimit.Text))
            {

            }
            else
            {
                MessageBox.Show("Credit Limit Tidak boleh di isi selain angka!!");
                txtCreditLimit.Focus();
                txtCreditLimit.Clear();
            }
        }

        private void txtCashbackBalance_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtCashbackBalance.Text))
            {

            }
            else
            {
                MessageBox.Show("CashBack Balance Tidak boleh di isi selain angka!!");
                txtCashbackBalance.Focus();
                txtCashbackBalance.Clear();
            }
        }

        private void txtDiscountBalance_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtDiscountBalance.Text))
            {

            }
            else
            {
                MessageBox.Show("Discount Balance Tidak boleh di isi selain angka!!");
                txtDiscountBalance.Focus();
                txtDiscountBalance.Clear();
            }
        }

        private void txtDebitNoteBalance_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtDebitNoteBalance.Text))
            {

            }
            else
            {
                MessageBox.Show("Debit Note Balance Tidak boleh di isi selain angka!!");
                txtDebitNoteBalance.Focus();
                txtDebitNoteBalance.Clear();
            }
        }

        private void txtPurchaseAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtPurchaseAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("Purchase Amount Tidak boleh di isi selain angka!!");
                txtPurchaseAmount.Focus();
                txtPurchaseAmount.Clear();
            }
        }

        private void txtSOAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtSOAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("SO Amount Tidak boleh di isi selain angka!!");
                txtSOAmount.Focus();
                txtSOAmount.Clear();
            }
        }

        private void txtDOAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtDOAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("DO Amount Tidak boleh di isi selain angka!!");
                txtDOAmount.Focus();
                txtDOAmount.Clear();
            }
        }

        private void txtPaymentAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtPaymentAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("Payment Amount Tidak boleh di isi selain angka!!");
                txtPaymentAmount.Focus();
                txtPaymentAmount.Clear();
            }
        }

        private void txtChequeAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtChequeAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("Cheque Amount Tidak boleh di isi selain angka!!");
                txtChequeAmount.Focus();
                txtChequeAmount.Clear();
            }
        }

        private void txtDPAmount_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtDPAmount.Text))
            {

            }
            else
            {
                MessageBox.Show("DP Amount Tidak boleh di isi selain angka!!");
                txtDPAmount.Focus();
                txtDPAmount.Clear();
            }
        }

        private void txtEstablished_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.]*$");
            if (strPattern.IsMatch(txtEstablished.Text))
            {

            }
            else
            {
                MessageBox.Show("Established Years Tidak boleh di isi selain angka!!");
                txtEstablished.Focus();
                txtEstablished.Clear();
            }
        }

        private void dgvAddress_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewTextBoxCell cellAreaCode = dgvAddress[6, e.RowIndex] as DataGridViewTextBoxCell;
            DataGridViewTextBoxCell cellRT = dgvAddress[7, e.RowIndex] as DataGridViewTextBoxCell;
            DataGridViewTextBoxCell cellRW = dgvAddress[8, e.RowIndex] as DataGridViewTextBoxCell;

            if (cellAreaCode != null)
            {
                if (e.ColumnIndex == 6)
                {
                    Regex strPattern = new Regex("^[0-9.]*$");
                    if (strPattern.IsMatch(e.FormattedValue.ToString()))
                    {

                    }
                    else
                    {
                        MessageBox.Show("Masukan Angka saja di kolom AreaCode");
                        e.Cancel = true;
                    }
                }
            }

            if (cellRT != null)
            {
                if (e.ColumnIndex == 7)
                {
                    Regex strPattern = new Regex("^[0-9.]*$");
                    if (strPattern.IsMatch(e.FormattedValue.ToString()))
                    {

                    }
                    else
                    {
                        MessageBox.Show("Masukan Angka saja di kolom RT");
                        e.Cancel = true;
                    }
                }
            }

            if (cellRW != null)
            {
                if (e.ColumnIndex == 8)
                {
                    Regex strPattern = new Regex("^[0-9.]*$");
                    if (strPattern.IsMatch(e.FormattedValue.ToString()))
                    {

                    }
                    else
                    {
                        MessageBox.Show ("Masukan Angka saja di kolom RW");
                        e.Cancel = true;
                    }
                }
            }
        }

        private void dgvContact_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewTextBoxCell cellContactNo = dgvContact[3, e.RowIndex] as DataGridViewTextBoxCell;
            DataGridViewTextBoxCell cellExtNo = dgvContact[4, e.RowIndex] as DataGridViewTextBoxCell;

            if (cellContactNo != null)
            {
                if (e.ColumnIndex == 3)
                {
                    Regex strPattern = new Regex("^[0-9.]*$");
                    if (strPattern.IsMatch(e.FormattedValue.ToString()))
                    {

                    }
                    else
                    {
                        MessageBox.Show("Masukan Angka saja di kolom ContactNo");
                        e.Cancel = true;
                    }
                }
            }

            if (cellExtNo != null)
            {
                if (e.ColumnIndex == 4)
                {
                    Regex strPattern = new Regex("^[0-9.]*$");
                    if (strPattern.IsMatch(e.FormattedValue.ToString()))
                    {

                    }
                    else
                    {
                        MessageBox.Show("Masukan Angka saja di kolom ExtNo");
                        e.Cancel = true;
                    }
                }
            }

        }

        private void txtNPWP_Validating(object sender, CancelEventArgs e)
        {
            Regex strPattern = new Regex("^[0-9.-]*$");
            if (strPattern.IsMatch(txtNPWP.Text))
            {

            }
            else
            {
                MessageBox.Show("NPWP Tidak boleh di isi selain angka!!");
                txtNPWP.Focus();
                txtNPWP.Clear();
            }
        }

        private void btnSearchCompanyGroup_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CompanyGroup";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtCompanyGroupId.Text = ConnectionString.Kode;
        }

        private void txtCompanyGroupId_Validating(object sender, CancelEventArgs e)
        {
            if (txtCompanyGroupId.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[CompanyGroup] Where CompanyGroupId = '" + txtCompanyGroupId.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Company Type tidak ada di database.");
                        btnSearchCompanyGroup_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

    }
}
