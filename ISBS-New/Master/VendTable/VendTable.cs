using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.VendTable
{
    public partial class VendTable : Form
    {
        private SqlConnection Conn, Conn2;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        public DataRow row;

        public DataTable TableAddress = new DataTable();
        public DataTable TableCP = new DataTable();

        Master.VendTable.InqVendTable Parent;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String VendId = null;
        bool flagVendor = false;
        bool flagTransaksi = false;
        bool flagProduct = false;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public VendTable()
        {
            InitializeComponent();
        }

        public void setParent(Master.VendTable.InqVendTable F)
        {
            Parent = F;
        }

        private void validateNumber(System.Windows.Forms.TextBox F)
        {
            double num;
            if (double.TryParse(F.Text, out num))
            {
                //hasilnya angka
            }
            else
            {
                //selain angka
                MessageBox.Show("Field hanya boleh angka.");
                F.Text = "";
                F.Focus();
            }
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

            column = new DataColumn("Email", typeof(String));
            column.ReadOnly = false;
            column.Unique = false;
            TableCP.Columns.Add(column);

            dgvCP.DataSource = TableCP;
        }

        public void setDataRowAddress(DataRow dr)
        {
            //for (int i = 0; dr.ItemArray.Length(); i++)
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
            if (dgvCP.DataSource == null)
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
            row["Email"] = dr.ItemArray[5].ToString();

            TableCP.Rows.Add(row);
        }

        private void setDefaultLocation()
        {

            lblVendor.Top = 20;
            grpVendor.Top = lblVendor.Location.Y - 6;

            lblTransaksi.Top = lblVendor.Location.Y + 50;
            grpTransaksi.Top = lblTransaksi.Location.Y - 6;

            lblProduct.Top = lblTransaksi.Location.Y + 50;
            grpFullItemId.Top = lblProduct.Location.Y - 6;

            btnSave.Top = lblProduct.Location.Y + 50;
            btnEdit.Top = lblProduct.Location.Y + 50;
            btnCancel.Top = lblProduct.Location.Y + 50;
            btnExit.Top = lblProduct.Location.Y + 50;

            grpVendor.Height = 0;
            grpTransaksi.Height = 0;
            grpFullItemId.Height = 0;

        }

        public void flag(String vendid, String mode)
        {
            VendId = vendid;
            Mode = mode;
        }

        private String GenerateId()
        {
            Conn = ConnectionString.GetConnection();
            String LastId = "";
            Query = "Select Top 1 (VendId) From dbo.VendTable order by VendId DESC";

            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                LastId = Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString();
            }

            if (String.IsNullOrEmpty(LastId))
            {
                return "VN0001";
            }
            else
            {
                int temp;
                temp = Int32.Parse(LastId.Substring(2, 4)) + 1;

                if (temp < 10)
                {
                    return "VN000" + temp;
                }
                else if (temp < 100)
                {
                    return "VN00" + temp;
                }
                else if (temp < 1000)
                {
                    return "VN0" + temp;
                }
                else if (temp < 10000)
                {
                    return "VN" + temp;
                }
                else
                {
                    return temp.ToString();
                }
            }
        }

        private void VendTable_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select * From [dbo].[VendTable] Where VendId = '" + VendId + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtVendId.Text = Dr["VendID"].ToString();
                    txtVendName.Text = Dr["VendName"].ToString();
                    txtTaxName.Text = Dr["TaxName"].ToString();
                    txtNPWP.Text = Dr["NPWP"].ToString();
                    txtPKP.Text = Dr["PKP"].ToString();
                    txtType.Text = Dr["Type"].ToString();
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
                    txtVendGroupId.Text = Dr["VendGroupID"].ToString();
                    txtDepositAmountAffiatedtoGroup.Text = Dr["DepositAmountAffiatedtoGroup"].ToString();
                    txtDepositAmountCurrencyID.Text = Dr["DepositAmountCurrencyID"].ToString();
                    txtDepositAmount.Text = Dr["DepositAmount"].ToString();
                    txtDepositType.Text = Dr["DepositType"].ToString();
                    txtDPAmountCurrencyID.Text = Dr["DPAmountCurrencyID"].ToString();
                    txtDPAmount.Text = Dr["DPAmount"].ToString();
                    txtCreditLimitCurrencyID.Text = Dr["CreditLimitCurrencyID"].ToString();
                    txtCreditLimit.Text = Dr["CreditLimit"].ToString();
                    txtCreditLimitPerPO.Text = Dr["CreditLimitPerPO"].ToString();
                    txtCashbackBalance.Text = Dr["CashbackBalance"].ToString();
                    txtDiscountBalance.Text = Dr["DiscountBalance"].ToString();
                    txtDebitNoteBalance.Text = Dr["DebitNoteBalance"].ToString();
                    txtPurchaseAmount.Text = Dr["PurchaseAmount"].ToString();
                    txtPOAmount.Text = Dr["POAmount"].ToString();
                    txtDOAmount.Text = Dr["DOAmount"].ToString();
                    txtPaymentAmount.Text = Dr["PaymentAmount"].ToString();
                    txtChequeAmount.Text = Dr["ChequeAmount"].ToString();
                }
                Dr.Close();
            }

            if (dgvAddress.DataSource == null)
            {
                MakeDataTableAddress();
            }

            if (dgvCP.DataSource == null)
            {
                MakeDataTableCP();
            }

            Query = "Select * From [dbo].[Address] Where ReffId = '" + VendId + "'";
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

            Query = "Select * From [dbo].[Contact] Where ReffRecId = '" + VendId + "'";
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
                    row["Email"] = Dr["Email"].ToString();

                    TableCP.Rows.Add(row);
                }
                Dr.Close();
            }
            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID1.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    lblReffFullItemID1.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID2.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    lblReffFullItemID2.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID3.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    lblReffFullItemID3.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID4.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    lblReffFullItemID4.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }

            Query = "Select ItemDeskripsi From [dbo].[InventTable] where FullItemId = '" + txtReffFullItemID5.Text + "'";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    lblReffFullItemID5.Text = Dr["ItemDeskripsi"].ToString();
                }
                Dr.Close();
            }



            Conn.Close();
        }

        private void ModeLoad()
        {
            setDefaultLocation();
            //if (Mode == "Edit")
            //{
            //    btnSelectAddress.Visible = true;
            //    btnSelectCP.Visible = true;
            //}
            RefreshGrid();
        }

        private void resetText()
        {
            txtVendId.Text = "";
            txtVendName.Text = "";
            txtTaxName.Text = "";
            txtNPWP.Text = "";
            txtPKP.Text = "";
            txtType.Text = "";
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
            txtVendGroupId.Text = "";
            txtDepositAmountAffiatedtoGroup.Text = "";
            txtDepositAmountCurrencyID.Text = "";
            txtDepositAmount.Text = "";
            txtDepositType.Text = "";
            txtDPAmountCurrencyID.Text = "";
            txtDPAmount.Text = "";
            txtCreditLimitCurrencyID.Text = "";
            txtCreditLimit.Text = "";
            txtCreditLimitPerPO.Text = "";
            txtCashbackBalance.Text = "";
            txtDiscountBalance.Text = "";
            txtDebitNoteBalance.Text = "";
            txtPurchaseAmount.Text = "";
            txtPOAmount.Text = "";
            txtDOAmount.Text = "";
            txtPaymentAmount.Text = "";
            txtChequeAmount.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtVendId.Enabled = false;
            txtVendName.Enabled = true;
            txtTaxName.Enabled = true;
            txtNPWP.Enabled = true;
            txtPKP.Enabled = true;
            txtType.Enabled = true;
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
            txtVendGroupId.Enabled = true;
            txtDepositAmountAffiatedtoGroup.Enabled = false;
            txtDepositAmountCurrencyID.Enabled = false;
            txtDepositAmount.Enabled = false;
            txtDepositType.Enabled = false;
            txtDPAmountCurrencyID.Enabled = false;
            txtDPAmount.Enabled = false;
            txtCreditLimitCurrencyID.Enabled = true;
            txtCreditLimit.Enabled = true;
            txtCreditLimitPerPO.Enabled = true;
            txtCashbackBalance.Enabled = false;
            txtDiscountBalance.Enabled = false;
            txtDebitNoteBalance.Enabled = false;
            txtPurchaseAmount.Enabled = false;
            txtPOAmount.Enabled = false;
            txtDOAmount.Enabled = false;
            txtPaymentAmount.Enabled = false;
            txtChequeAmount.Enabled = false;
            dgvAddress.ReadOnly = false;
            dgvCP.ReadOnly = false;

            btnSearchPaymentMode.Enabled = true;
            btnSearchCurrency.Enabled = true;
            btnSearchVendorGroup.Enabled = true;
            btnSearchReffBank1.Enabled = true;
            btnSearchReffBank2.Enabled = true;
            btnSearchCreditLimitCurrency.Enabled = true;

            btnSearchReffItemId1.Enabled = true;
            btnSearchReffItemId2.Enabled = true;
            btnSearchReffItemId3.Enabled = true;
            btnSearchReffItemId4.Enabled = true;
            btnSearchReffItemId5.Enabled = true;

            btnAddAddress.Enabled = true;
            btnAddCP.Enabled = true;
            //btnSelectAddress.Enabled = true;
            //btnSelectCP.Enabled = true;
            btnDeleteAddress.Enabled = true;
            btnDeleteCP.Enabled = true;

            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtVendId.Enabled = false;
            txtVendName.Enabled = true;
            txtTaxName.Enabled = true;
            txtNPWP.Enabled = true;
            txtPKP.Enabled = true;
            txtType.Enabled = true;
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
            txtVendGroupId.Enabled = true;
            txtDepositAmountAffiatedtoGroup.Enabled = false;
            txtDepositAmountCurrencyID.Enabled = true;
            txtDepositAmount.Enabled = false;
            txtDepositType.Enabled = false;
            txtDPAmountCurrencyID.Enabled = true;
            txtDPAmount.Enabled = false;
            txtCreditLimitCurrencyID.Enabled = true;
            txtCreditLimit.Enabled = false;
            txtCreditLimitPerPO.Enabled = false;
            txtCashbackBalance.Enabled = false;
            txtDiscountBalance.Enabled = false;
            txtDebitNoteBalance.Enabled = false;
            txtPurchaseAmount.Enabled = false;
            txtPOAmount.Enabled = false;
            txtDOAmount.Enabled = false;
            txtPaymentAmount.Enabled = false;
            txtChequeAmount.Enabled = false;
            dgvAddress.ReadOnly = false;
            dgvCP.ReadOnly = false;

            btnSearchPaymentMode.Enabled = true;
            btnSearchCurrency.Enabled = true;
            btnSearchVendorGroup.Enabled = true;
            btnSearchReffBank1.Enabled = true;
            btnSearchReffBank2.Enabled = true;
            btnSearchCreditLimitCurrency.Enabled = true;
            btnSearchDepositCurrency.Enabled = true;
            btnSearchDPCurrency.Enabled = true;

            btnSearchReffItemId1.Enabled = true;
            btnSearchReffItemId2.Enabled = true;
            btnSearchReffItemId3.Enabled = true;
            btnSearchReffItemId4.Enabled = true;
            btnSearchReffItemId5.Enabled = true;

            btnAddAddress.Enabled = true;
            btnAddCP.Enabled = true;
            //btnSelectAddress.Enabled = true;
            //btnSelectCP.Enabled = true;
            btnDeleteAddress.Enabled = true;
            btnDeleteCP.Enabled = true;
            btnSave.Visible = true;
            btnCancel.Visible = true;
            btnEdit.Visible = false;

            btnSave.Top = lblProduct.Location.Y + 50;
            btnCancel.Top = lblProduct.Location.Y + 50;
            btnEdit.Top = lblProduct.Location.Y + 50;
            btnExit.Top = lblProduct.Location.Y + 50;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
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
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(txtVendName.Text))
            {
                MessageBox.Show("Data harus diisi");
                return;
            }

            int TempoBayar = string.IsNullOrEmpty(txtTempoBayar.Text) ? 0 : int.Parse(txtTempoBayar.Text);
            decimal DepositAmountAffiatedtoGroup = string.IsNullOrEmpty(txtDepositAmountAffiatedtoGroup.Text) ? 0 : decimal.Parse(txtDepositAmountAffiatedtoGroup.Text);
            decimal DepositAmount = string.IsNullOrEmpty(txtDepositAmount.Text) ? 0 : decimal.Parse(txtDepositAmount.Text);
            decimal DPAmount = string.IsNullOrEmpty(txtDPAmount.Text) ? 0 : decimal.Parse(txtDPAmount.Text);
            decimal CreditLimit = string.IsNullOrEmpty(txtCreditLimit.Text) ? 0 : decimal.Parse(txtCreditLimit.Text);
            decimal CreditLimitPerPO = string.IsNullOrEmpty(txtCreditLimitPerPO.Text) ? 0 : decimal.Parse(txtCreditLimitPerPO.Text);
            decimal CashbackBalance = string.IsNullOrEmpty(txtCashbackBalance.Text) ? 0 : decimal.Parse(txtCashbackBalance.Text);
            decimal DiscountBalance = string.IsNullOrEmpty(txtDiscountBalance.Text) ? 0 : decimal.Parse(txtDiscountBalance.Text);
            decimal DebitNoteBalance = string.IsNullOrEmpty(txtDebitNoteBalance.Text) ? 0 : decimal.Parse(txtDebitNoteBalance.Text);
            decimal PurchaseAmount = string.IsNullOrEmpty(txtPurchaseAmount.Text) ? 0 : decimal.Parse(txtPurchaseAmount.Text);
            decimal POAmount = string.IsNullOrEmpty(txtPOAmount.Text) ? 0 : decimal.Parse(txtPOAmount.Text);
            decimal DOAmount = string.IsNullOrEmpty(txtDOAmount.Text) ? 0 : decimal.Parse(txtDOAmount.Text);
            decimal PaymentAmount = string.IsNullOrEmpty(txtPaymentAmount.Text) ? 0 : decimal.Parse(txtPaymentAmount.Text);
            decimal ChequeAmount = string.IsNullOrEmpty(txtChequeAmount.Text) ? 0 : decimal.Parse(txtChequeAmount.Text);

            if (Mode == "New")
            {

                String TempId = GenerateId();

                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "insert into [dbo].[VendTable] (VendId, VendName, TaxName, NPWP, PKP, Type, TempoBayar, PaymentModeID, TaxGroup, ReffBank1ID, ";
                    Query += "ReffBank2ID, ReffFullItemID1, ReffFullItemID2, ReffFullItemID3, ReffFullItemID4, ReffFullItemID5, CurrencyID, ";
                    Query += "VendGroupID, DepositAmountAffiatedtoGroup, DepositAmountCurrencyID, DepositAmount, DepositType, DPAmountCurrencyID, DPAmount, CreditLimitCurrencyID, CreditLimit, ";
                    Query += "CreditLimitPerPO, Status, LastStatusChange, CashbackBalance, DiscountBalance, DebitNoteBalance, PurchaseAmount, POAmount, DOAmount, PaymentAmount, ChequeAmount) ";
                    Query += "values ('" + TempId + "', '" + txtVendName.Text + "', '" + txtTaxName.Text + "', '" + txtNPWP.Text + "', '" + txtPKP.Text + "', '" + txtType.Text + "', '" + txtTempoBayar.Text + "', '" + txtPaymentModeId.Text + "', '" + txtTaxGroup.Text + "', '" + txtReffBank1ID.Text + "', ";
                    Query += "'" + txtReffBank2ID.Text + "', '" + txtReffFullItemID1.Text + "', '" + txtReffFullItemID2.Text + "', '" + txtReffFullItemID3.Text + "', '" + txtReffFullItemID4.Text + "', '" + txtReffFullItemID5.Text + "', '" + txtCurrencyId.Text + "', ";
                    Query += "'" + txtVendGroupId.Text + "', '" + DepositAmountAffiatedtoGroup + "', '" + txtDepositAmountCurrencyID.Text + "', '" + DepositAmount + "', '" + txtDepositType.Text + "', '" + txtDPAmountCurrencyID.Text + "', '" + DPAmount + "', '" + txtCreditLimitCurrencyID.Text + "', '" + CreditLimit + "', ";
                    Query += "'" + CreditLimitPerPO + "', 'y', getdate(), '" + CashbackBalance + "', '" + DiscountBalance + "', '" + DebitNoteBalance + "', '" + PurchaseAmount + "', '" + POAmount + "', '" + DOAmount + "', '" + PaymentAmount + "', '" + ChequeAmount + "');";

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
                            Query += "values ('VendTable', '" + TempId + "', '" + PurposeType + "', '" + Name + "', '" + Address + "', '" + ProvinceId + "', '" + CityId + "', '" + AreaCode + "', '" + RT + "', '" + RW + "', ";
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
                    if (dgvCP.Rows.Count > 0)
                    {
                        int flagPrimaryCP = 0;

                        for (int i = 0; i <= dgvCP.RowCount - 1; i++)
                        {
                            String Deskripsi = dgvCP.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvCP.Rows[i].Cells["Deskripsi"].Value.ToString();
                            String ContactType = dgvCP.Rows[i].Cells["ContactType"].Value == null ? "" : dgvCP.Rows[i].Cells["ContactType"].Value.ToString();
                            String ContactNo = dgvCP.Rows[i].Cells["ContactNo"].Value == null ? "" : dgvCP.Rows[i].Cells["ContactNo"].Value.ToString();
                            String ExtNo = dgvCP.Rows[i].Cells["ExtNo"].Value == null ? "" : dgvCP.Rows[i].Cells["ExtNo"].Value.ToString();
                            String Email = dgvCP.Rows[i].Cells["Email"].Value == null ? "" : dgvCP.Rows[i].Cells["Email"].Value.ToString();

                            bool PrimaryCP;
                            if ((bool)dgvCP.Rows[i].Cells["Primary"].Value == false)
                            {
                                PrimaryCP = false;
                            }
                            else
                            {
                                PrimaryCP = true;
                                flagPrimaryCP++;
                            }
                            Query = "insert into [dbo].[Contact] (ReffTableName, ReffRecID, Deskripsi, ContactType, ContactNo, ExtNo, PrimaryC, Email) ";
                            Query += "values ('VendTable', '" + TempId + "', '" + Deskripsi + "', '" + ContactType + "', '" + ContactNo + "', '" + ExtNo + "', ";
                            Query += "'" + PrimaryCP + "', '" + Email + "');";

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
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
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
                Conn2 = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "update [dbo].[VendTable] set VendName='" + txtVendName.Text + "', TaxName='" + txtTaxName.Text + "', NPWP='" + txtNPWP.Text + "', PKP='" + txtPKP.Text + "', ";
                    Query += "Type='" + txtType.Text + "', TempoBayar='" + TempoBayar + "', PaymentModeID='" + txtPaymentModeId.Text + "', TaxGroup='" + txtTaxGroup.Text + "', ReffBank1ID='" + txtReffBank1ID.Text + "', ReffBank2ID='" + txtReffBank2ID.Text + "', ";
                    Query += "ReffFullItemID1='" + txtReffFullItemID1.Text + "', ReffFullItemID2='" + txtReffFullItemID2.Text + "', ReffFullItemID3='" + txtReffFullItemID3.Text + "', ReffFullItemID4='" + txtReffFullItemID4.Text + "', ReffFullItemID5='" + txtReffFullItemID5.Text + "', CurrencyID='" + txtCurrencyId.Text + "', VendGroupID='" + txtVendGroupId.Text + "', ";
                    Query += "DepositAmountAffiatedtoGroup='" + DepositAmountAffiatedtoGroup + "', DepositAmountCurrencyID='" + txtDepositAmountCurrencyID.Text + "', DepositAmount='" + DepositAmount + "', DepositType='" + txtDepositType.Text + "', DPAmountCurrencyID='" + txtDPAmountCurrencyID.Text + "', DPAmount='" + DPAmount + "', ";
                    Query += "CreditLimitCurrencyID='" + txtCreditLimitCurrencyID.Text + "', CreditLimit='" + CreditLimit + "', CreditLimitPerPO='" + CreditLimitPerPO + "', Status='y', LastStatusChange=getdate(), CashbackBalance='" + CashbackBalance + "', ";
                    Query += "DiscountBalance='" + DiscountBalance + "', DebitNoteBalance='" + DebitNoteBalance + "', PurchaseAmount='" + PurchaseAmount + "', POAmount='" + POAmount + "', DOAmount='" + DOAmount + "', PaymentAmount='" + PaymentAmount + "', ChequeAmount='" + ChequeAmount + "' ";
                    Query += "where VendId='" + txtVendId.Text + "';";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteScalar();
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
                            int RecId = String.IsNullOrEmpty(dgvAddress.Rows[i].Cells["ID"].Value.ToString())? 0 : Int32.Parse(dgvAddress.Rows[i].Cells["ID"].Value.ToString());
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

                            Query = "Select * From [dbo].[Address] Where ReffId = '" + VendId + "' And RecId = '" + RecId + "'";
                            using (SqlCommand cmd = new SqlCommand(Query, Conn2))
                            {
                                Dr = cmd.ExecuteReader();

                                if (Dr.Read())
                                {
                                    Query = "update [dbo].[Address] set PurposeType='" + PurposeType + "', Name='" + Name + "', Address='" + Address + "', ProvinceID='" + ProvinceId + "', ";
                                    Query += "CityID='" + CityId + "', AreaCode='" + AreaCode + "', RT='" + RT + "', RW='" + RW + "', PrimaryC='" + Primary + "' ";
                                    Query += "where ReffId='" + txtVendId.Text + "' And RecId='" + RecId + "';";

                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    Cmd.ExecuteScalar();
                                }
                                else
                                {
                                    Query = "insert into [dbo].[Address] (ReffTableName, ReffID, PurposeType, Name, Address, ProvinceID, CityID, AreaCode, RT, RW, PrimaryC) ";
                                    Query += "values ('VendTable', '" + VendId + "', '" + PurposeType + "', '" + Name + "', '" + Address + "', '" + ProvinceId + "', '" + CityId + "', '" + AreaCode + "', '" + RT + "', '" + RW + "', ";
                                    Query += "'" + Primary + "');";

                                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                                    {
                                        Cmd.ExecuteNonQuery();
                                    }
                                }
                                Dr.Close();
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
                    if (dgvCP.Rows.Count > 0)
                    {
                        int flagPrimaryCP = 0;

                        for (int i = 0; i <= dgvCP.RowCount - 1; i++)
                        {
                            int RecId = String.IsNullOrEmpty(dgvCP.Rows[i].Cells["ID"].Value.ToString()) ? 0 : Int32.Parse(dgvCP.Rows[i].Cells["ID"].Value.ToString());
                            String Deskripsi = dgvCP.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvCP.Rows[i].Cells["Deskripsi"].Value.ToString();
                            String ContactType = dgvCP.Rows[i].Cells["ContactType"].Value == null ? "" : dgvCP.Rows[i].Cells["ContactType"].Value.ToString();
                            String ContactNo = dgvCP.Rows[i].Cells["ContactNo"].Value == null ? "" : dgvCP.Rows[i].Cells["ContactNo"].Value.ToString();
                            String ExtNo = dgvCP.Rows[i].Cells["ExtNo"].Value == null ? "" : dgvCP.Rows[i].Cells["ExtNo"].Value.ToString();
                            String Email = dgvCP.Rows[i].Cells["Email"].Value == null ? "" : dgvCP.Rows[i].Cells["Email"].Value.ToString();
                            bool PrimaryCP;
                            if ((bool)dgvCP.Rows[i].Cells["Primary"].Value == false)
                            {
                                PrimaryCP = false;
                            }
                            else
                            {
                                PrimaryCP = true;
                                flagPrimaryCP++;
                            }

                            Query = "Select * From [dbo].[Contact] Where ReffRecId = '" + VendId + "' And RecId = '" + RecId + "'";
                            using (SqlCommand cmd = new SqlCommand(Query, Conn2))
                            {
                                Dr = cmd.ExecuteReader();

                                if (Dr.Read())
                                {
                                    Query = "update [dbo].[Contact] set Deskripsi='" + Deskripsi + "', ContactType='" + ContactType + "', ContactNo='" + ContactNo + "', ExtNo='" + ExtNo + "', ";
                                    Query += "PrimaryC='" + PrimaryCP + "', Email='" + Email + "' ";
                                    Query += "where ReffRecId='" + VendId + "' And RecId='" + RecId + "';";

                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    Cmd.ExecuteScalar();
                                }
                                else
                                {
                                    Query = "insert into [dbo].[Contact] (ReffTableName, ReffRecID, Deskripsi, ContactType, ContactNo, ExtNo, PrimaryC, Email) ";
                                    Query += "values ('VendTable', '" + VendId + "', '" + Deskripsi + "', '" + ContactType + "', '" + ContactNo + "', '" + ExtNo + "', ";
                                    Query += "'" + PrimaryCP + "', '" + Email + "');";

                                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                                    {
                                        Cmd.ExecuteNonQuery();
                                    }
                                }
                                Dr.Close();
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
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + txtVendId.Text + ", berhasil diupdate.");
                Parent.RefreshGrid();
                this.Close();
            }
        }

        private void lblVendor_Click(object sender, EventArgs e)
        {
            if (flagVendor == false)//buka
            {
                grpVendor.Height = 622;
                grpVendor.Top = lblVendor.Location.Y - 6;

                if (flagTransaksi == false)
                {
                    lblTransaksi.Top = grpVendor.Bottom + 10;
                    lblProduct.Top = lblTransaksi.Location.Y + 50;
                    grpFullItemId.Top = lblProduct.Location.Y - 6;
                }
                else
                {
                    lblTransaksi.Top = grpVendor.Bottom + 10;
                    grpTransaksi.Top = lblTransaksi.Location.Y - 6;
                    lblProduct.Top = grpTransaksi.Bottom + 10;
                    if (flagProduct == true)
                    {
                        grpFullItemId.Top = lblProduct.Location.Y - 6;
                    }
                }
                flagVendor = true;
            }
            else
            { // tutup
                grpVendor.Height = 0;
                lblTransaksi.Top = lblVendor.Location.Y + 50;

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
                flagVendor = false;
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
                grpTransaksi.Height = 309;
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

        private void btnSearchVendorGroup_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "VendGroup";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtVendGroupId.Text = ConnectionString.Kode;
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
            lblReffFullItemID1.Text = ConnectionString.Kode2;
        }

        private void btnSearchReffItemId2_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID2.Text = ConnectionString.Kode;
            lblReffFullItemID2.Text = ConnectionString.Kode2;
        }

        private void btnSearchReffItemId3_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID3.Text = ConnectionString.Kode;
            lblReffFullItemID3.Text = ConnectionString.Kode2;
        }

        private void btnSearchReffItemId4_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID4.Text = ConnectionString.Kode;
            lblReffFullItemID4.Text = ConnectionString.Kode2;
        }

        private void btnSearchReffItemId5_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtReffFullItemID5.Text = ConnectionString.Kode;
            lblReffFullItemID5.Text = ConnectionString.Kode2;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            resetText();
            RefreshGrid();

            txtVendId.Enabled = false;
            txtVendName.Enabled = false;
            txtTaxName.Enabled = false;
            txtNPWP.Enabled = false;
            txtPKP.Enabled = false;
            txtType.Enabled = false;
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
            txtVendGroupId.Enabled = false;
            txtDepositAmountAffiatedtoGroup.Enabled = false;
            txtDepositAmountCurrencyID.Enabled = false;
            txtDepositAmount.Enabled = false;
            txtDepositType.Enabled = false;
            txtDPAmountCurrencyID.Enabled = false;
            txtDPAmount.Enabled = false;
            txtCreditLimitCurrencyID.Enabled = false;
            txtCreditLimit.Enabled = false;
            txtCreditLimitPerPO.Enabled = false;
            txtCashbackBalance.Enabled = false;
            txtDiscountBalance.Enabled = false;
            txtDebitNoteBalance.Enabled = false;
            txtPurchaseAmount.Enabled = false;
            txtPOAmount.Enabled = false;
            txtDOAmount.Enabled = false;
            txtPaymentAmount.Enabled = false;
            txtChequeAmount.Enabled = false;
            dgvAddress.ReadOnly = true;
            dgvCP.ReadOnly = true;

            btnSearchPaymentMode.Enabled = false;
            btnSearchCurrency.Enabled = false;
            btnSearchVendorGroup.Enabled = false;
            btnSearchReffBank1.Enabled = false;
            btnSearchReffBank2.Enabled = false;
            btnSearchCreditLimitCurrency.Enabled = false;
            btnSearchDepositCurrency.Enabled = false;
            btnSearchDPCurrency.Enabled = false;

            btnSearchReffItemId1.Enabled = false;
            btnSearchReffItemId2.Enabled = false;
            btnSearchReffItemId3.Enabled = false;
            btnSearchReffItemId4.Enabled = false;
            btnSearchReffItemId5.Enabled = false;

            btnAddAddress.Enabled = false;
            btnAddCP.Enabled = false;
            //btnSelectAddress.Enabled = false;
            //btnSelectCP.Enabled = false;
            btnDeleteAddress.Enabled = false;
            btnDeleteCP.Enabled = false;

            btnSave.Visible = false;
            btnCancel.Visible = false;
            btnEdit.Visible = true;

            btnSave.Top = lblProduct.Location.Y + 50;
            btnCancel.Top = lblProduct.Location.Y + 50;
            btnEdit.Top = lblProduct.Location.Y + 50;
            btnExit.Top = lblProduct.Location.Y + 50;
        }



        private void txtVendName_Validating(object sender, CancelEventArgs e)
        {
            if (txtVendName.Text == "")
            {

            }
            else
            {
                if (Mode == "New")
                {
                    Query = "Select * From [dbo].[VendTable] Where REPLACE(VendName,' ','') = '" + txtVendName.Text.Replace(" ","") + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        if (Dr.Read())//sama dengan di database
                        {
                            MessageBox.Show("Nama sudah ada di database.");
                            txtVendName.Focus();
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

        private void txtVendGroupId_Validating(object sender, CancelEventArgs e)
        {
            if (txtVendGroupId.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[VendGroup] Where VendGroupId = '" + txtVendGroupId.Text + "'";

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
                        btnSearchVendorGroup_Click(sender, e);
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

        private void btnAddAddress_Click(object sender, EventArgs e)
        {
            //Master.Address.AddressForm F = new Master.Address.AddressForm();
            //F.setParentForm(this);
            //F.flag("", "New");
            //F.Show();
        }

        private void btnSelectAddress_Click(object sender, EventArgs e)
        {
            //String RecId = dgvAddress.CurrentRow.Cells["RecId"].Value.ToString();

            //Master.Address.AddressForm F = new Master.Address.AddressForm();
            //F.setParentForm(this);
            //F.flag(RecId, "Edit");
            //F.Show();
        }

        private void btnDeleteAddress_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
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
            else if (Mode == "Edit")
            {
                try
                {
                    if (dgvAddress.RowCount > 0)
                    {
                        Conn2 = ConnectionString.GetConnection();
                        Index = dgvAddress.CurrentRow.Index;
                        String RecId = dgvAddress.Rows[Index].Cells["ID"].Value == null ? "" : dgvAddress.Rows[Index].Cells["ID"].Value.ToString();

                        Query = "Select * From [dbo].[Address] Where ReffId = '" + VendId + "' And RecId = '" + RecId + "'";
                        using (SqlCommand cmd = new SqlCommand(Query, Conn2))
                        {
                            Dr = cmd.ExecuteReader();

                            if (Dr.Read())
                            {
                                DialogResult dr = MessageBox.Show("Data ini sudah tercatat di dalam Database. Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                                if (dr == DialogResult.Yes)
                                {
                                    Conn = ConnectionString.GetConnection();
                                    Query = "Delete from [dbo].[Address] where ReffId = '" + VendId + "' And RecId = '" + RecId + "'";

                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();

                                    MessageBox.Show("Data berhasil dihapus.");

                                    Index = 0;
                                    Conn.Close();
                                    dgvCP.Rows.RemoveAt(Index);
                                }
                            }
                            else
                            {
                                DialogResult dr = MessageBox.Show("Data ini belum tercatat di dalam Database. Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                                if (dr == DialogResult.Yes)
                                {
                                    dgvAddress.Rows.RemoveAt(Index);
                                    Index = 0;
                                }
                            }
                            Dr.Close();
                        }
                    }
                }
                catch (Exception exx)
                {
                    MessageBox.Show(exx.Message);
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

        private void btnAddCP_Click(object sender, EventArgs e)
        {
            //Master.Address.ContactForm F = new Master.Address.ContactForm();
            //F.setParentForm(this);
            //F.flag("", "New");
            //F.Show();
        }

        private void btnSelectCP_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteCP_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                if (dgvCP.RowCount > 0)
                {
                    Index = dgvCP.CurrentRow.Index;

                    DialogResult dr = MessageBox.Show("Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        dgvCP.Rows.RemoveAt(Index);
                        Index = 0;
                    }

                }
            }
            else if (Mode == "Edit")
            {
                try
                {
                    if (dgvCP.RowCount > 0)
                    {
                        Conn2 = ConnectionString.GetConnection();
                        Index = dgvCP.CurrentRow.Index;
                        String RecId = dgvCP.Rows[Index].Cells["ID"].Value == null ? "" : dgvCP.Rows[Index].Cells["ID"].Value.ToString();

                        Query = "Select * From [dbo].[Contact] Where ReffRecId = '" + VendId + "' And RecId = '" + RecId + "'";
                        using (SqlCommand cmd = new SqlCommand(Query, Conn2))
                        {
                            Dr = cmd.ExecuteReader();

                            if (Dr.Read())
                            {
                                DialogResult dr = MessageBox.Show("Data ini sudah tercatat di dalam Database. Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                                if (dr == DialogResult.Yes)
                                {
                                    Conn = ConnectionString.GetConnection();
                                    Query = "Delete from [dbo].[Contact] where ReffRecId = '" + VendId + "' And RecId = '" + RecId + "'";

                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();

                                    MessageBox.Show("Data berhasil dihapus.");

                                    Index = 0;
                                    Conn.Close();
                                    dgvCP.Rows.RemoveAt(Index);
                                }
                            }
                            else
                            {
                                DialogResult dr = MessageBox.Show("Data ini belum tercatat di dalam Database. Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                                if (dr == DialogResult.Yes)
                                {
                                    dgvCP.Rows.RemoveAt(Index);
                                    Index = 0;
                                }
                            }
                            Dr.Close();
                        }
                    }
                }
                catch (Exception exx)
                {
                    MessageBox.Show(exx.Message);
                }
            }
        }

        private void dgvCP_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCP.Columns[dgvCP.CurrentCell.ColumnIndex].Name == "ContactType")
            {
                if ((String)dgvCP.CurrentRow.Cells["ContactType"].Value == String.Empty)
                {

                }
                else
                {
                    Query = "Select * From [dbo].[ContactType] Where ContactType = '" + dgvCP.CurrentRow.Cells["ContactType"].Value.ToString() + "'";

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
                            dgvCP.CurrentRow.Cells["ContactType"].Value = "";

                            string SchemaName = "dbo";
                            string TableName = "ContactType";

                            Search tmpSearch = new Search();
                            tmpSearch.SetSchemaTable(SchemaName, TableName);
                            tmpSearch.ShowDialog();
                            dgvCP.CurrentRow.Cells["ContactType"].Value = ConnectionString.Kode;
                        }
                    }
                }
            }
        }

        private void txtCreditLimit_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCreditLimit.Text))
            {

            }
            else
            {
                validateNumber(txtCreditLimit);
            }
        }

        private void txtCreditLimitPerPO_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCreditLimitPerPO.Text))
            {

            }
            else
            {
                validateNumber(txtCreditLimitPerPO);
            }
        }

        private void txtDepositAmountAffiatedtoGroup_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDepositAmountAffiatedtoGroup.Text))
            {

            }
            else
            {
                validateNumber(txtDepositAmountAffiatedtoGroup);
            }
        }

        private void txtDepositAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDepositAmount.Text))
            {

            }
            else
            {
                validateNumber(txtDepositAmount);
            }
        }

        private void txtDPAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDPAmount.Text))
            {

            }
            else
            {
                validateNumber(txtDPAmount);
            }
        }

        private void txtPurchaseAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPurchaseAmount.Text))
            {

            }
            else
            {
                validateNumber(txtPurchaseAmount);
            }
        }

        private void txtPOAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPOAmount.Text))
            {

            }
            else
            {
                validateNumber(txtPOAmount);
            }
        }

        private void txtDOAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDOAmount.Text))
            {

            }
            else
            {
                validateNumber(txtDOAmount);
            }
        }

        private void txtPaymentAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPaymentAmount.Text))
            {

            }
            else
            {
                validateNumber(txtPaymentAmount);
            }
        }

        private void txtChequeAmount_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtChequeAmount.Text))
            {

            }
            else
            {
                validateNumber(txtChequeAmount);
            }
        }

        private void txtCashbackBalance_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCashbackBalance.Text))
            {

            }
            else
            {
                validateNumber(txtCashbackBalance);
            }
        }

        private void txtDiscountBalance_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDiscountBalance.Text))
            {

            }
            else
            {
                validateNumber(txtDiscountBalance);
            }
        }

        private void txtDebitNoteBalance_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDebitNoteBalance.Text))
            {

            }
            else
            {
                validateNumber(txtDebitNoteBalance);
            }
        }
    }
}
