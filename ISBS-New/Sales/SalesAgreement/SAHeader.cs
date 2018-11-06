using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Transactions;
using System.IO;
using System.Text.RegularExpressions;

namespace ISBS_New.Sales.SalesAgreement
{
    public partial class SAHeader : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
        private SqlConnection Conn2;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        //private SqlDataAdapter Da;
        //private DataTable Dt;
        //private DataSet Ds;
        private string Query;
        private TransactionScope scope;
        /**********SQL*********/

        /**********Steven Edit start************/
        List<string> sSelectedFile, FileName, Extension;
        int Index;
        List<byte[]> test = new List<byte[]>();
        string vOldReferensi, vNewReferensi;
        /**********Steven Edit end ************/

        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "No", "SalesAgreementNo", "SeqNo", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "Base", "RemainingQty", "Qty", "Unit", "Price", "Qty_Alt", "Unit_Alt", "Price_Alt", "ConvertionRatio", "DeliveryMethod", "ExpectedDateFrom", "ExpectedDateTo", "SubTotal", "SubTotal_PPN", "SubTotal_PPH", "LogisticAmount", "LogisticNotes", "DiscType", "DiscPercent", "DiscAmount", "BonusAmount", "CashBackAmount", "Notes", "LockStatus", "LockID", "LockQty", "SA_SQ_Id", "SA_SQ_SeqNo", "RefTransId", "RefTrans_SeqNo", "GelombangId", "BracketId", "Gelombang_Price", "GelombangSeqNo_Base", "BracketDesc", "PLJNo", "PLJSeqNo", "PLJPrice" };

        string[] tableCols2 = new string[] { "No", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "RefTransId", "RefTrans_SeqNo", "Qty", "Unit", "Price", "Qty_Alt", "Unit_Alt", "Price_Alt", "ConvertionRatio", "DeliveryMethod", "ExpectedDateFrom", "ExpectedDateTo", "SubTotal", "SubTotal_PPN", "SubTotal_PPH", "LogisticAmount", "LogisticNotes", "DiscType", "DiscPercent", "DiscAmount", "BonusAmount", "CashBackAmount", "Notes" };
        /*********datagridview cols name*********/

        /**********SET MODE*********/
        private string Mode;
        private string SAID;
        /**********SET MODE*********/

        /*********PARENT*********/
        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F) { Parent = F; }
        /*********PARENT*********/

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        bool check; //label
        private string msg; //Validation
        /*********VALIDATION*********/

        DataGridViewComboBoxCell cell; //CELLVALUE
        private SqlDataReader Dr2; //CELLVALUE
        ContextMenu vendid = new ContextMenu();

        DateTimePicker dtp;

        List<string> pricelistDtl = new List<string>();

        string oldSAType; //CEK KALAU ADA PERUBAHAN VALUE CHECKBOX SALES TYPE

        //GV POP UP 
        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public void SetMode(string tmpMode, string tmpNumber)
        {
            Mode = tmpMode;
            tbxSAID.Text = tmpNumber;
            SAID = tmpNumber;
        }

        public SAHeader()
        {
            InitializeComponent();
        }

        private void ModeNew()
        {
            Mode = "New";
            //STEVEN EDIT START
            txtReferensi.Enabled = true;
            cbByPhone.Enabled = true;
            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnDelAttachment.Enabled = true;
            dgvAttachment.Rows.Clear();
            GetDgvAttachmentData();
            //STEVEN EDIT START
            dtDP.MinDate = dtSA.Value;
            dtSADue.MinDate = dtSA.Value;
            btnReserved.Visible = false;

            cbSAType.SelectedIndex = 0;
            cbDPType.SelectedIndex = 1;
            if (cbCurrency.Text == String.Empty)
                cbCurrency.Text = "IDR";
            if (cbPaymentMode.Text == String.Empty)
                cbPaymentMode.SelectedIndex = 0;
            if (cbDPType.Text == String.Empty)
                cbDPType.SelectedIndex = 0;
            if (cbPPh.Text == String.Empty)
                cbPPh.SelectedIndex = 0;
            if (cbPPN.Text == String.Empty)
                cbPPN.SelectedIndex = 1;

            dtSA.Value = DateTime.Now;
            dtSADue.Value = DateTime.Now;

            dtSA.Enabled = false;
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            tbxXRate.Enabled = false;
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            //STEVEN EDIT START
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;
            txtReferensi.Enabled = false;
            cbByPhone.Enabled = false;
            //dgvAttachment.Rows.Clear();
            GetDgvAttachmentData();
            //STEVEN EDIT END
            cbSAType.Enabled = false;
            cbCurrency.Enabled = false;
            tbxXRate.Enabled = false;
            cbToP.Enabled = false;
            cbPaymentMode.Enabled = false;
            cbDPType.Enabled = false;
            tbxDPAmount.Enabled = false;
            tbxDPPercent.Enabled = false;
            dtDP.Enabled = false;
            //dtDP.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dtSADue.Enabled = false;
            cbPPh.Enabled = false;
            cbPPN.Enabled = false;
            tbxNotes.Enabled = false;
            //tia edit
            tbxCustName.Enabled = true;
            tbxCustID.Enabled = true;
            tbxCustID.ReadOnly = true;
            tbxCustName.ReadOnly = true;
            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            tbxMOUID.Enabled = true;
            tbxMOUID.ReadOnly = true;
            tbxMOUID.ContextMenu = vendid;
            //tia end

            btnSMoU.Enabled = false;
            btnCust.Enabled = false;
            btnSSQ.Enabled = false;
            btnRef.Enabled = false;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;

            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;

            Conn = ConnectionString.GetConnection();
            Query = "select TransStatus from SalesAgreementH where SalesAgreementNo = '" + tbxSAID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string SAStatus = Cmd.ExecuteScalar().ToString();
            Conn.Close();
            if (SAStatus == "01" || SAStatus == "05")
            {
                btnConfirm.Visible = true;
                btnAmend.Visible = false; btnAmend.Enabled = false;
                btnApprove.Visible = false; btnApprove.Enabled = false;
                btnReject.Visible = false; btnReject.Enabled = false;
                btnReserved.Enabled = false;
                btnEdit.Enabled = true;
            }
            else if (SAStatus == "12" || SAStatus == "06" || SAStatus == "08")
            {
                btnConfirm.Visible = false; btnConfirm.Enabled = false;
                btnAmend.Visible = true; btnAmend.Enabled = true;
                btnApprove.Visible = false; btnApprove.Enabled = false;
                btnReject.Visible = false; btnReject.Enabled = false;
                btnReserved.Enabled = true;
                btnEdit.Enabled = false;
            }
            else if (SAStatus == "09" || SAStatus == "11")
            {
                btnConfirm.Visible = false; btnConfirm.Enabled = false;
                btnAmend.Visible = false; btnAmend.Enabled = false;
                btnApprove.Visible = true; btnApprove.Enabled = true;
                btnReject.Visible = true; btnReject.Enabled = true;
                btnReserved.Enabled = false;
                btnEdit.Enabled = true;
            }
            else if (SAStatus == "03")
            {
                btnConfirm.Visible = true; btnConfirm.Enabled = true;
                btnAmend.Visible = true; btnAmend.Enabled = true;
                btnApprove.Visible = false; btnApprove.Enabled = false;
                btnReject.Visible = false; btnReject.Enabled = false;
                btnReserved.Enabled = false;
                btnEdit.Enabled = false;
            }
            else
            {
                btnConfirm.Visible = false; btnConfirm.Enabled = false;
                btnAmend.Visible = false; btnAmend.Enabled = false;
                btnApprove.Visible = false; btnApprove.Enabled = false;
                btnReject.Visible = false; btnReject.Enabled = false;
                btnReserved.Enabled = false;
                btnEdit.Enabled = false;
            }
            //btnReserved.Visible = false;
        }

        //tia edit
        private void ModePopUp()
        {
            Mode = "PopUp";

            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }

            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;
            txtReferensi.Enabled = false;
            cbByPhone.Enabled = false;
            //dgvAttachment.Rows.Clear();
            GetDgvAttachmentData();
            cbSAType.Enabled = false;
            cbCurrency.Enabled = false;
            tbxXRate.Enabled = false;
            cbToP.Enabled = false;
            cbPaymentMode.Enabled = false;
            cbDPType.Enabled = false;
            tbxDPAmount.Enabled = false;
            tbxDPPercent.Enabled = false;
            dtDP.Enabled = false;
            //dtDP.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dtSADue.Enabled = false;
            cbPPh.Enabled = false;
            cbPPN.Enabled = false;
            tbxNotes.Enabled = false;
            //tia edit
            tbxCustName.Enabled = true;
            tbxCustID.Enabled = true;
            tbxCustID.ReadOnly = true;
            tbxCustName.ReadOnly = true;
            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            tbxMOUID.Enabled = true;
            tbxMOUID.ReadOnly = true;
            tbxMOUID.ContextMenu = vendid;
            //tia end

            btnSMoU.Enabled = false;
            btnCust.Enabled = false;
            btnSSQ.Enabled = false;
            btnRef.Enabled = false;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;

            btnSave.Visible = false;
            btnCancel.Visible = false;
            btnExit.Enabled = true;
            btnConfirm.Visible = false; btnConfirm.Enabled = false;
            btnAmend.Visible = false; btnAmend.Enabled = false;
            btnApprove.Visible = false; btnApprove.Enabled = false;
            btnReject.Visible = false; btnReject.Enabled = false;
            btnReserved.Enabled = false;
            btnEdit.Visible = false;
            btnReserved.Visible = false;
        }

        //tia edit end

        private void ModeEdit()
        {
            Mode = "Edit";
            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnDelAttachment.Enabled = true;

            if (txtReferensi.Text == "By Phone")
            {
                txtReferensi.Enabled = false;
                cbByPhone.Enabled = true;
                btnRef.Enabled = false;
            }
            if (txtReferensi.Text != "By Phone")
            {
                txtReferensi.Enabled = true;
                cbByPhone.Enabled = true;
                btnRef.Enabled = true;
            }
            cbCurrency.Enabled = true;
            if (tbxXRate.Text == "IDR")
                tbxXRate.Enabled = true;
            else
                tbxXRate.Enabled = false;
            cbToP.Enabled = true;
            cbPaymentMode.Enabled = true;

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select DP_Required from CustTable where CustId = '" + tbxCustID.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() == "Y")
            {
                cbDPType.Enabled = false;
                tbxDPAmount.Enabled = true;
                tbxDPPercent.Enabled = true;
                dtDP.Enabled = true;
            }
            else
            {
                cbDPType.Enabled = true;
                tbxDPAmount.Enabled = false;
                tbxDPPercent.Enabled = false;
                dtDP.Enabled = false;
            }
            Conn.Close();
            dtSADue.Enabled = true;            
            btnConfirm.Enabled = false;
            btnConfirm.Visible = false;
            btnReserved.Enabled = false;

            cbPPN.Enabled = true;
            cbPPh.Enabled = true;
            tbxNotes.Enabled = true;

            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;

            btnApprove.Visible = false;
            btnReject.Visible = false;
            btnAmend.Visible = false;

            if (tbxSQID.Text.Trim() == "")
            {
                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = false;
                btnDelete.Enabled = false;

                //gv1Format();
                btnSMoU.Enabled = false;
                btnCust.Enabled = false;

                cbCurrency.Enabled = false;
                cbToP.Enabled = false;
                cbPaymentMode.Enabled = false;
                cbDPType.Enabled = false;
                tbxDPPercent.Enabled = false;
                tbxDPAmount.Enabled = false;
                cbPPN.Enabled = false;
                cbPPh.Enabled = false;
                btnAdd.Enabled = false;
                btnDelete.Enabled = false;
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
                oldSQ = tbxSQID.Text;
            }
        }

        private void SAHeader_Load(object sender, EventArgs e)
        {
            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy";
            dtp.Visible = false;
            dtp.Width = 100;
            dataGridView1.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;
            dataGridView1.CellBeginEdit += this.dataGridView1_CellBeginEdit;
            dataGridView1.CellEndEdit += this.dataGridView1_CellEndEdit;

            Conn = ConnectionString.GetConnection();
            cbCurrency.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select CurrencyID from [ISBS-NEW4].[dbo].[CurrencyTable] order by CurrencyID asc", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
                cbCurrency.Items.Add(Dr[0]);
            Dr.Close();

            cbPaymentMode.Items.Clear();
            cbPaymentMode.Items.Add("Select");
            Cmd = new SqlCommand("select [PaymentModeName] from [ISBS-NEW4].[dbo].[PaymentMode] order by PaymentModeName asc", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
                cbPaymentMode.Items.Add(Dr[0]);
            Dr.Close();

            cbToP.Items.Clear();
            cbToP.Items.Add("Select");
            Cmd = new SqlCommand("select [TermOfPayment] from TermOfPayment order by DueDate asc", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
                cbToP.Items.Add(Dr[0]);
            Dr.Close();
            Conn.Close();

            if (Mode == "New")
            {
                ModeNew();
                cbToP.SelectedIndex = 0;
                oldSAType = cbSAType.Text;
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
                GetDataHeader();
                gv1Format();
            }
            else if (Mode=="PopUp")//tia edit
            {
                ModePopUp();
                GetDataHeader();
            }//tia edit end
            metroTabControl1.SelectedTab = tabSales;
        }

        private void GenerateGVRows(string Query)
        {
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                this.dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", "", Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemID"], Dr["ItemName"], Dr["Base"], "", Dr["Qty"], Dr["Unit"], Dr["Price"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["Price_Alt"], Dr["ConvertionRatio"], Dr["DeliveryMethod"], Dr["ExpectedDateFrom"], Dr["ExpectedDateTo"], Dr["SubTotal"], Dr["SubTotal_PPN"], Dr["SubTotal_PPH"], Dr["LogisticAmount"], Dr["LogisticNotes"], "", Dr["DiscPercent"], Dr["DiscAmount"], Dr["BonusAmount"], Dr["CashBackAmount"], Dr["Notes"], "", "", "", "", "", "", "", Dr["GelombangId"], Dr["BracketId"], Dr["Gelombang_Price"], Dr["GelombangSeqNo_Base"], "", Dr["PLJNo"], Dr["PLJSeqNo"], Dr["PLJPrice"]);
                if (Dr["Base"].ToString() != "N")
                {
                    if (tbxSQID.Text == String.Empty)
                    {
                        cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                        if (Dr["DiscType"] != (object)DBNull.Value)
                        {
                            Query = "select Deskripsi from DiskonScheme where diskonschemeID = '" + Dr["DiscType"] + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            cell.Value = Cmd.ExecuteScalar().ToString();
                        }
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                    }
                    else
                    {
                        if (Dr["DiscType"] != (object)DBNull.Value)
                        {
                            Query = "select Deskripsi from DiskonScheme where diskonschemeID = '" + Dr["DiscType"] + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"].Value = Cmd.ExecuteScalar().ToString();
                        }
                    }
                }
            }
            Dr.Close();
        }

        private void GetDataHeader()
        {
            try
            {
                test.Clear();

                Conn = ConnectionString.GetConnection();
                if (Mode == "New")
                {
                    GetDgvAttachmentData();

                    Query = "select * from SalesQuotationH where SalesQuotationNo = '" + tbxSQID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        cbSAType.Text = Dr["TransType"].ToString();
                        tbxSQID.Text = Dr["SalesQuotationNo"].ToString();
                        tbxMOUID.Text = Dr["SalesMouNo"].ToString();
                        tbxCustID.Text = Dr["CustID"].ToString();
                        tbxCustName.Text = Dr["CustName"].ToString();
                        cbCurrency.Text = Dr["CurrencyID"].ToString();
                        tbxXRate.Text = Dr["ExchRate"].ToString();
                        cbToP.Text = Dr["TermOfPayment"].ToString();
                        tbxNotes.Text = Dr["Notes"].ToString();

                        cbPPN.Text = Dr["PPN"].ToString();
                        cbPPh.Text = Dr["PPh"].ToString();

                        tbxSTotal.Text = Dr["Total"].ToString();
                        tbxGDisc.Text = Dr["Total_Disk"].ToString();
                        tbxGPPh.Text = Dr["total_pph"].ToString();
                        tbxGPPN.Text = Dr["Total_PPN"].ToString();
                        tbxGTotal.Text = Dr["Total_Nett"].ToString();
                        tbxGBonus.Text = Dr["Total_Bonus"].ToString();
                        tbxGCashBack.Text = Dr["Total_CashBack"].ToString();
                        Cmd = new SqlCommand("select [PaymentModeName] from [PaymentMode] where PaymentModeID = '" + Dr["PaymentModeID"] + "'", Conn);
                        if (Cmd.ExecuteScalar() != null)
                            cbPaymentMode.Text = Cmd.ExecuteScalar().ToString();
                    }
                    Dr.Close();

                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();
                    dataGridView1.ColumnCount = tableCols.Length;
                    for (int i = 0; i < tableCols.Length; i++)
                    {
                        dataGridView1.Columns[i].Name = tableCols[i];
                    }
                    dataGridView1.AllowUserToAddRows = false;
                    Query = "select * from SalesQuotationD where SalesQuotationNo = '" + tbxSQID.Text + "' and Deleted = 'N'";
                    GenerateGVRows(Query);
                }
                else
                {
                    Query = "select * from SalesAgreementH where SalesAgreementNo = '" + tbxSAID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Dr["DPDueDate"] == (object)DBNull.Value)
                            dtDP.MinDate = Convert.ToDateTime("01/01/1753");
                        else
                            dtDP.MinDate = Convert.ToDateTime(Dr["OrderDate"]);
                        dtSADue.MinDate = Convert.ToDateTime(Dr["OrderDate"]);

                        tbxSAID.Text = Dr["SalesAgreementNo"].ToString();
                        tbxRefID.Text = Dr["RefTransId"].ToString();
                        cbSAType.Text = Dr["TransType"].ToString();
                        tbxMOUID.Text = Dr["SalesMouNo"].ToString();
                        tbxSQID.Text = Dr["SalesQuotationNo"].ToString();
                        tbxCustID.Text = Dr["CustID"].ToString();
                        tbxCustName.Text = Dr["CustName"].ToString();
                        cbCurrency.Text = Dr["CurrencyID"].ToString();
                        tbxXRate.Text = string.Format("{0:#,0.00}", double.Parse(Dr["ExchRate"].ToString()));
                        dtSA.Value = Convert.ToDateTime(Dr["OrderDate"]);
                        cbToP.Text = Dr["TermofPayment"].ToString();
                        Cmd = new SqlCommand("select [PaymentModeName] from [PaymentMode] where PaymentModeID = @PaymentModeID", Conn);
                        Cmd.Parameters.AddWithValue("@PaymentModeID", Dr["PaymentModeID"]);
                        cbPaymentMode.Text = Cmd.ExecuteScalar().ToString();
                        cbDPType.Text = Dr["DPType"].ToString();
                        tbxDPAmount.Text = Dr["DPAmount"].ToString();
                        tbxDPPercent.Text = Dr["DPPercent"].ToString();
                        if (Dr["DPDueDate"] != (object)DBNull.Value)
                            dtDP.Value = Convert.ToDateTime(Dr["DPDueDate"]);
                        dtSADue.Value = Convert.ToDateTime(Dr["ValidTo"]);
                        cbPPN.Text = Dr["PPN"].ToString();
                        cbPPh.Text = Dr["PPh"].ToString();
                        tbxNotes.Text = Dr["Notes"].ToString();
                        tbxSTotal.Text = Dr["Total"].ToString();
                        tbxGPPh.Text = Dr["Total_PPh"].ToString();
                        tbxGPPN.Text = Dr["Total_PPN"].ToString();
                        tbxGTotal.Text = Dr["Total_Nett"].ToString();
                        tbxGBonus.Text = Dr["Total_Bonus"].ToString();
                        tbxGCashBack.Text = Dr["Total_Cashback"].ToString();
                        txtReferensi.Text = Dr["Referensi"].ToString();
                    }
                    Dr.Close();

                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();
                    dataGridView1.ColumnCount = tableCols.Length;
                    for (int i = 0; i < tableCols.Length; i++)
                    {
                        dataGridView1.Columns[i].Name = tableCols[i];
                    }
                    dataGridView1.AllowUserToAddRows = false;

                    Query = "select * from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSAID.Text + "' and Deleted = 'N'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        dataGridView1.Rows.Add(1);
                        for (int i = 0; i < tableCols.Length; i++)
                        {
                            if (i == 0)
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                            else if (tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo")
                            {
                                if (Dr["Base"].ToString() == "Y")
                                {
                                    if (Dr[tableCols[i]] != (object)DBNull.Value)
                                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Convert.ToDateTime(Dr[tableCols[i]]);
                                }
                                else
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = (object)DBNull.Value;
                            }
                            else if (tableCols[i] == "DiscType")
                            {
                                if (Mode != "Edit")
                                {
                                    if (Dr["Base"].ToString() == "Y")
                                    {
                                        Query = "select Deskripsi from DiskonScheme where diskonSchemeID = '" + Dr[tableCols[i]] + "'";
                                        Cmd = new SqlCommand(Query, Conn);
                                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                                    }
                                }
                                else
                                {
                                    if (Dr["Base"].ToString() == "Y")
                                    {
                                        cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                                        cell.Value = "Select";
                                        if (Dr[tableCols[i]] != null)
                                        {
                                            Query = "select Deskripsi from DiskonScheme where diskonSchemeID = '" + Dr[tableCols[i]] + "'";
                                            Cmd = new SqlCommand(Query, Conn);
                                            cell.Value = Cmd.ExecuteScalar().ToString();
                                        }
                                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                                    }
                                }
                            }
                            else if (tableCols[i] == "LockStatus")
                            {
                                cell = null;
                                cell = new DataGridViewComboBoxCell();
                                cell.Items.Add("Select");
                                cell.Items.Add("Warehouse");
                                cell.Value = "Select";
                                if (Dr["LockStatus"] != null)
                                    cell.Value = Dr["LockStatus"];
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]] = cell;
                            }
                            else if (tableCols[i] == "DeliveryMethod")
                            {
                                if (Dr["Base"].ToString() == "Y")
                                {
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr[tableCols[i]].ToString();
                                }
                            }
                            else
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr[tableCols[i]].ToString();
                        }
                    }
                    Dr.Close();


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

                    Query = "Select * From [tblAttachments] Where ReffTableName = 'SalesAgreementH' And ReffTransId = '" + tbxSAID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                        test.Add((byte[])Dr["Attachment"]);
                    }

                    dgvAttachment.AutoResizeColumns();
                }

                /*****************************************************************************/
                dataGridView2.Rows.Clear();
                dataGridView2.ColumnCount = tableCols2.Length;
                for (int i = 0; i < tableCols2.Length; i++)
                {
                    dataGridView2.Columns[i].Name = tableCols2[i];
                }
                dataGridView2.AllowUserToAddRows = false;
                Query = "select * from SalesQuotationD where SalesQuotationNo = '" + tbxSQID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int addRow = 0;
                while (Dr.Read())
                {
                    dataGridView2.Rows.Add(1);
                    for (int i = 0; i < tableCols2.Length; i++)
                    {
                        if (i == 0)
                            dataGridView2.Rows[addRow].Cells[tableCols2[i]].Value = dataGridView2.RowCount;
                        else if (tableCols2[i] == "DiscType")
                        {
                            if (Dr["Base"].ToString() != "N")
                            {
                                if (Dr["DiscType"] != (object)DBNull.Value)
                                {
                                    Query = "select Deskripsi from DiskonScheme where diskonschemeID = '" + Dr["DiscType"] + "'";
                                    Cmd = new SqlCommand(Query, Conn);
                                    dataGridView2.Rows[addRow].Cells[tableCols2[i]].Value = Cmd.ExecuteScalar().ToString();
                                }
                            }
                        }
                        else
                            dataGridView2.Rows[addRow].Cells[tableCols2[i]].Value = Dr[tableCols2[i]];
                    }
                    addRow++;
                }
                Dr.Close();
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                Conn.Close();
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally { }
        }

        private void gv1Format()
        {

            if (Mode == "BeforeEdit")
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[tableCols[i]].ReadOnly = true;
                    dataGridView1.Columns[tableCols[i]].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
            else
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    if (tableCols[i] == "No" || tableCols[i] == "SalesAgreementNo" || tableCols[i] == "SeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "FullItemID" || tableCols[i] == "ItemName" || tableCols[i] == "Unit" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "ConvertionRatio" || tableCols[i] == "RemainingQty" || tableCols[i] == "SA_SQ_Id" || tableCols[i] == "SA_SQ_SeqNo" || tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo" || tableCols[i] == "GelombangId" || tableCols[i] == "BracketId" || tableCols[i] == "Base" || tableCols[i] == "Gelombang_Price" || tableCols[i] == "GelombangSeqNo_Base" || tableCols[i] == "SubTotal" || tableCols[i] == "SubTotal_PPN" || tableCols[i] == "SubTotal_PPH" || tableCols[i] == "DiscAmount" || tableCols[i] == "DiscPercent" || tableCols[i] == "BracketDesc" || tableCols[i] == "PLJNo" || tableCols[i] == "PLJSeqNo" || tableCols[i] == "PLJPrice" || tableCols[i] == "DeliveryMethod" || tableCols[i] == "Price")
                    {
                        dataGridView1.Columns[tableCols[i]].ReadOnly = true;
                        dataGridView1.Columns[tableCols[i]].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dataGridView1.Columns[tableCols[i]].ReadOnly = false;
                        dataGridView1.Columns[tableCols[i]].DefaultCellStyle.BackColor = Color.White;
                    }
                }

                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        if (dataGridView1.Rows[j].Cells["Base"].Value == String.Empty || dataGridView1.Rows[j].Cells["Base"].Value == null)
                            dataGridView1.Rows[j].Cells["Base"].Value = "";
                        if (dataGridView1.Rows[j].Cells["Base"].Value.ToString() == "Y" && cbSAType.Text == "QUANTITY")
                        {
                            dataGridView1.Rows[j].Cells["Qty_Alt"].ReadOnly = false;
                            dataGridView1.Rows[j].Cells["Qty_Alt"].Style.BackColor = Color.White;
                            dataGridView1.Rows[j].Cells["Qty"].ReadOnly = true;
                            dataGridView1.Rows[j].Cells["Qty"].Style.BackColor = Color.LightGray;
                            //if (tbxRefID.Text != String.Empty)
                            //{
                            //    dataGridView1.Rows[j].Cells["Price"].ReadOnly = true;
                            //    dataGridView1.Rows[j].Cells["Price"].Style.BackColor = Color.LightGray;
                            //}
                            if (tbxRefID.Text == String.Empty)
                            {
                                dataGridView1.Rows[j].Cells["Price"].ReadOnly = false;
                                dataGridView1.Rows[j].Cells["Price"].Style.BackColor = Color.White;
                            }
                        }
                        else if (dataGridView1.Rows[j].Cells["Base"].Value.ToString() == "Y" && cbSAType.Text == "AMOUNT")
                        {
                            dataGridView1.Rows[j].Cells["SubTotal"].ReadOnly = false;
                            dataGridView1.Rows[j].Cells["SubTotal"].Style.BackColor = Color.White;
                            //if (tbxRefID.Text != String.Empty)
                            //{
                            //    dataGridView1.Rows[j].Cells["Price"].ReadOnly = true;
                            //    dataGridView1.Rows[j].Cells["Price"].Style.BackColor = Color.LightGray;
                            //}

                            if (tbxRefID.Text == String.Empty)
                            {
                                dataGridView1.Rows[j].Cells["Price"].ReadOnly = false;
                                dataGridView1.Rows[j].Cells["Price"].Style.BackColor = Color.White;
                            }
                        }
                        if (dataGridView1.Rows[j].Cells["Base"].Value.ToString() == "N")
                        {
                            dataGridView1.Rows[j].Cells[i].ReadOnly = true;
                            dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.LightGray;
                        }
                        else
                            break;
                    }
                }
            }

        }

        private DataGridViewComboBoxCell cellValue(string query)
        {
            cell = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(query, Conn);
            Dr2 = Cmd.ExecuteReader();
            cell = new DataGridViewComboBoxCell();
            cell.Items.Add("Select");
            while (Dr2.Read())
                cell.Items.Add(Dr2[0].ToString());
            return cell;
        }

        string oldSQ;
        private void btnSSQ_Click(object sender, EventArgs e)
        {
            tbxSQID.Text = "";
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            Query = "and a.TransStatus in ('01', '03', '23', '07') and a.TransType != 'FIX' ";
            //Query += "and DATEADD(day,0,GETDATE()) between a.ValidFrom and a.ValidTo ";
            //Query += "and a.ValidFrom > DATEADD(day,-1,GETDATE())";
            Query += "and a.ValidFrom < DATEADD(day,0,GETDATE())";
            Query += "and a.ValidTo > DATEADD(day,-1,GETDATE())";
            f.SetSchemaTable("dbo", "SalesQuotationH", Query, "a.*", "SalesQuotationH a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                tbxSQID.Text = SearchV2.data[0];
                GetDataHeader();
                //gv1Format();
                btnSMoU.Enabled = false;
                btnCust.Enabled = false;

                cbCurrency.Enabled = false;
                cbToP.Enabled = false;
                cbPaymentMode.Enabled = false;
                cbDPType.Enabled = false;
                tbxDPPercent.Enabled = false;
                tbxDPAmount.Enabled = false;
                cbPPN.Enabled = false;
                cbPPh.Enabled = false;
                btnAdd.Enabled = false;
                btnDelete.Enabled = false;
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
                oldSQ = tbxSQID.Text;
            }
            else
            {
                if (oldSQ != tbxSQID.Text)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();
                    ResetForm();
                    oldSQ = tbxSQID.Text;
                    btnSMoU.Enabled = true;
                    btnCust.Enabled = true;

                    cbCurrency.Enabled = true;
                    cbToP.Enabled = true;
                    cbPaymentMode.Enabled = true;
                    cbDPType.Enabled = true;
                    tbxDPPercent.Enabled = false;
                    tbxDPAmount.Enabled = false;
                    cbPPN.Enabled = true;
                    cbPPh.Enabled = true;
                    btnAdd.Enabled = true;
                    btnDelete.Enabled = true;
                }
            }
        }

        private void ResetForm()
        {
            tbxMOUID.Text = "";
            tbxCustID.Text = "";
            tbxCustName.Text = "";

            cbCurrency.SelectedItem = "IDR";
            tbxXRate.Text = "1.00";
            cbToP.SelectedIndex = 0;
            cbPaymentMode.SelectedIndex = 0;

            cbDPType.SelectedItem = "N";
            tbxDPAmount.Text = "0.00";
            tbxDPPercent.Text = "0.00";
            dtDP.MinDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dtDP.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            cbPPN.SelectedIndex = 1;
            cbPPh.SelectedIndex = 0;

            tbxNotes.Text = "";

            tbxSTotal.Text = "0.00";
            tbxGDisc.Text = "0.00";
            tbxGPPh.Text = "0.00";
            tbxGPPN.Text = "0.00";
            tbxGTotal.Text = "0.00";
            tbxGLog.Text = "0.00";
            tbxGBonus.Text = "0.00";
            tbxGCashBack.Text = "0.00";
        }

        private void btnCust_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
                MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change Customer!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                tbxCustID.Text = "";
                tbxCustName.Text = "";
                SearchV2 f = new SearchV2();
                f.SetMode("No");
                if (tbxMOUID.Text == String.Empty)
                    f.SetSchemaTable("dbo", "CustTable", "", "a.*", "CustTable a");
                else
                    f.SetSchemaTable("dbo", "CustTable", "and CustId in (select b.CustID from CustMouH a left join CustMou_Dtl b on a.MouNo = b.MouNo where a.MouNo = '" + tbxMOUID.Text + "')", "a.*", "CustTable a");
                f.ShowDialog();
                if (SearchV2.data.Count != 0)
                {
                    tbxCustID.Text = SearchV2.data[0];
                    tbxCustName.Text = SearchV2.data[1];
                    Conn = ConnectionString.GetConnection();
                    Query = "select DP_Required from CustTable where CustId = '" + tbxCustID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    cbDPType.Text = Cmd.ExecuteScalar().ToString();
                    Conn.Close();
                    if (cbDPType.Text == "Y")
                        cbDPType.Enabled = false;
                    else
                        cbDPType.Enabled = true;
                    setDefaultValue();
                }
            }
        }

        private void btnRef_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "tblRef", "and a.Referensi not in (select Referensi from SalesAgreementH where TransStatus not in ('02', '04', '10')) or a.Referensi not in (select Referensi from SalesOrderH where TransStatus not in ('02', '04', '11'))", "a.*", "tblRef a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                txtReferensi.Text = SearchV2.data[0];
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView2.Columns["ItemName"].Frozen = true;

            dataGridView2.Columns["GroupID"].Visible = false;
            dataGridView2.Columns["SubGroup1ID"].Visible = false;
            dataGridView2.Columns["SubGroup2ID"].Visible = false;
            dataGridView2.Columns["ItemID"].Visible = false;
            dataGridView2.Columns["RefTransId"].Visible = false;
            dataGridView2.Columns["RefTrans_SeqNo"].Visible = false;
            dataGridView2.Columns["Qty_Alt"].Visible = false;
            dataGridView2.Columns["Unit_Alt"].Visible = false;
            dataGridView2.Columns["Price_Alt"].Visible = false;
            dataGridView2.Columns["ConvertionRatio"].Visible = false;

            dataGridView2.Columns["SubTotal_PPH"].Visible = false;

            dataGridView2.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView2.ReadOnly = true;
            dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.BackColor = Color.LightGray;

            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("Qty") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("Qty_Alt") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("ConvertionRatio") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("DiscPercent") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("LockQty"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("Amount") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("Price") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("Total"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void cbDPType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Mode == "Edit" || Mode == "New")
            {
                if (cbDPType.Text == "N")
                {
                    tbxDPPercent.Enabled = false;
                    tbxDPAmount.Enabled = false;
                    dtDP.Enabled = false;
                }
                else if (cbDPType.Text == "Y")
                {
                    tbxDPAmount.Enabled = true;
                    tbxDPPercent.Enabled = true;
                    dtDP.Enabled = true;
                }
            }
            else
            {
                tbxDPPercent.Enabled = false;
                tbxDPAmount.Enabled = false;
                dtDP.Enabled = false;
            }

        }

        private void tbxSTotal_TextChanged(object sender, EventArgs e)
        {
            tbxSTotal.Text = string.Format("{0:#,0.00}", double.Parse(tbxSTotal.Text));
        }

        private void tbxGDisc_TextChanged(object sender, EventArgs e)
        {
            tbxGDisc.Text = string.Format("{0:#,0.00}", double.Parse(tbxGDisc.Text));
        }

        private void tbxGPPN_TextChanged(object sender, EventArgs e)
        {
            tbxGPPN.Text = string.Format("{0:#,0.00}", double.Parse(tbxGPPN.Text));
        }

        private void tbxGPPh_TextChanged(object sender, EventArgs e)
        {
            tbxGPPh.Text = string.Format("{0:#,0.00}", double.Parse(tbxGPPh.Text));
        }

        private void tbxGTotal_TextChanged(object sender, EventArgs e)
        {
            tbxGTotal.Text = string.Format("{0:#,0.00}", double.Parse(tbxGTotal.Text));
            tbxDPAmount.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDPPercent.Text) / 100).ToString()));
        }

        private void tbxGBonus_TextChanged(object sender, EventArgs e)
        {
            tbxGBonus.Text = string.Format("{0:#,0.00}", double.Parse(tbxGBonus.Text));
        }

        private void tbxGCashBack_TextChanged(object sender, EventArgs e)
        {
            tbxGCashBack.Text = string.Format("{0:#,0.00}", double.Parse(tbxGCashBack.Text));
        }

        private void tbxXRate_Leave(object sender, EventArgs e)
        {
            if (tbxXRate.Text.Trim() == String.Empty)
                tbxXRate.Text = "0";
            tbxXRate.Text = string.Format("{0:#,0.00}", double.Parse(tbxXRate.Text));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbxRefID.Text != String.Empty)
            {
                SearchV2 f = new SearchV2();
                f.SetMode("Check");
                string excludeItem = "";
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (i == 0)
                        excludeItem += " and a.SeqNo not in (";
                    else if (i >= 1)
                        excludeItem += ", ";
                    excludeItem += "'" + dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value.ToString() + "'";
                    if (i == dataGridView1.RowCount - 1)
                        excludeItem += ")";
                }
                f.SetSchemaTable("dbo", "SalesAgreement_Dtl", "and a.SalesAgreementNo = '" + tbxRefID.Text + "' and a.Deleted = 'N'" + excludeItem, "a.*", "SalesAgreement_Dtl a");
                f.ShowDialog();
                if (SearchV2.data.Count != 0)
                {
                    Conn = ConnectionString.GetConnection();
                    string where = "";
                    string GelombangId = "";
                    string BracketId = "";
                    int GelombangSeqNo_Base = 0;
                    for (int i = 0; i < SearchV2.data.Count; i++)
                    {
                        Query = "select GelombangId, BracketId, GelombangSeqNo_Base from salesAgreement_Dtl where salesAgreementNo = '" + SearchV2.data[i] + "' and seqNo = '" + SearchV2.data2[i] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            GelombangId = Dr["GelombangId"].ToString();
                            BracketId = Dr["BracketId"].ToString();
                            GelombangSeqNo_Base = Convert.ToInt32(Dr["GelombangSeqNo_Base"]);
                        }
                        Dr.Close();

                        if (i == 0)
                            where += " and (";
                        if (i >= 1)
                            where += " or ";
                        where += "(GelombangId = '" + GelombangId + "' and BracketId = '" + BracketId + "' and (GelombangSeqNo_Base = '" + GelombangSeqNo_Base + "' or Base = 'Y'))";
                        if (i == SearchV2.data2.Count - 1)
                            where += ")";
                    }
                    Query = "select * from SalesAgreement_Dtl where salesAgreementNo = '" + tbxRefID.Text + "' and Deleted = 'N'" + where;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        #region pass value to gv
                        this.dataGridView1.Rows.Add(1);
                        for (int i = 0; i < tableCols.Length; i++)
                        {
                            if (i == 0)
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                            else if (tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo")
                            {
                                if (Dr["Base"].ToString() == "Y")
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Convert.ToDateTime(Dr[tableCols[i]]);
                            }
                            else if (tableCols[i] == "DiscType")
                            {
                                if (Dr["Base"].ToString() == "Y")
                                {
                                    cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                                }
                            }
                            else
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                        }
                        #endregion
                    }
                    Dr.Close();
                    Conn.Close();
                    gv1Format();
                }
            }
            else
            {
                string criteria = " and b.Criteria in ('1', ";
                string wherePlus = "";
                Conn = ConnectionString.GetConnection();
                Query = "select a.FullItemId, a.DeliveryMethod from Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo left join PriceList_AccountList c on c.PriceListNo = b.PricelistNo where 1=1 and a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and CAST(a.PriceType as int) = '2' and b.Active = '1' and b.TransStatus = '03' and c.AccountId = '" + tbxCustID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    wherePlus += " and not (";
                    int idx = 0;
                    while (Dr.Read())
                    {
                        if (idx >= 1)
                            wherePlus += " or ";
                        wherePlus += "a.FullItemId = '" + Dr["FullItemId"].ToString() + "' and a.DeliveryMethod = '" + Dr["DeliveryMethod"].ToString() + "'";
                        idx++;
                    }
                    wherePlus += ")";
                    criteria += "'3')";
                }
                else
                    criteria += "'2')";
                Dr.Close();
                Conn.Close();

                SearchV2 f = new SearchV2();
                f.SetMode("No");
                string where = "and type = 'sales' and ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and CAST(PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and Active = '1' and TransStatus = '03')a";
                string columnName = "case when a.PricelistNo is null then b.PricelistNo else a.PricelistNo end 'PricelistNo', case when a.SeqNo is null then b.SeqNo else a.SeqNo end 'SeqNo',case when a.AccountId is null then b.AccountId else a.AccountId end 'AccountId',case when a.FullItemId is null then b.FullItemId else a.FullItemId end 'FullItemId',case when a.ItemName is null then b.ItemName else a.ItemName end 'ItemName',case when a.Price is null then b.Price else a.Price end 'Price',case when a.DeliveryMethod is null then b.DeliveryMethod else a.DeliveryMethod end 'DeliveryMethod',case when a.Type is null then b.Type else a.Type end 'Type',case when a.ValidFrom is null then b.ValidFrom else a.ValidFrom end 'ValidFrom',case when a.ValidTo is null then b.ValidTo else a.ValidTo end 'ValidTo',case when a.PriceType is null then b.PriceType else a.PriceType end 'PriceType',case when a.Active is null then b.Active else a.Active end 'Active',case when a.TransStatus is null then b.TransStatus else a.TransStatus end 'TransStatus'";
                string table = "select a.PricelistNo, a.SeqNo, c.AccountId, a.FullItemId, a.ItemName, a.Price, a.DeliveryMethod, a.Type, b.ValidFrom, b.ValidTo, a.PriceType, b.Active, b.TransStatus from Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo left join PriceList_AccountList c on c.PriceListNo = b.PricelistNo where 1=1 and a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and CAST(a.PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and b.Active = '1' and b.TransStatus = '03'";
                string tableName2 = "(select * from (Select " + columnName + " from (" + table + wherePlus + criteria + " )a full join (" + table + " and c.AccountId = '" + tbxCustID.Text + "') b on a.PricelistNo = b.PricelistNo )a";
                f.SetSchemaTable("dbo", "Pricelist_Dtl", where, "case when a.PricelistNo is null then b.PricelistNo else a.PricelistNo end, case when a.SeqNo is null then b.SeqNo else a.SeqNo end ,case when a.AccountId is null then b.AccountId else a.AccountId end ,case when a.FullItemId is null then b.FullItemId else a.FullItemId end ,case when a.ItemName is null then b.ItemName else a.ItemName end ,case when a.Price is null then b.Price else a.Price end ,case when a.DeliveryMethod is null then b.DeliveryMethod else a.DeliveryMethod end", tableName2);
                f.ShowDialog();

                if (SearchV2.data.Count != 0)
                {
                    SearchV2 f2 = new SearchV2();
                    f2.SetMode("No");
                    where = "and a.ItemId = '" + SearchV2.data[2] + "'";
                    pricelistDtl.Clear();
                    pricelistDtl.Add(SearchV2.data[0]); //PLJ ID
                    pricelistDtl.Add(SearchV2.data[1]); //PLJ SEQNO
                    pricelistDtl.Add(SearchV2.data[5]); //PLJ CREDIT DAYS
                    f2.SetSchemaTable("dbo", "InventGelombangD", "and a.Type = 'Sales' " + where, "a.*", "InventGelombangD a");
                    f2.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        SearchV2 f3 = new SearchV2();
                        f3.SetMode("Check");
                        where = "and a.GelombangId = '" + SearchV2.data[3] + "' and a.BracketId = '" + SearchV2.data[4] + "'";
                        f3.SetSchemaTable("dbo", "InventGelombangD", "and a.Type = 'Sales' " + where, "a.*", "InventGelombangD a");
                        f3.ShowDialog();
                        if (SearchV2.data.Count != 0)
                        {
                            Conn = ConnectionString.GetConnection();
                            dataGridView1.ColumnCount = tableCols.Length;
                            for (int i = 0; i < tableCols.Length; i++)
                            {
                                dataGridView1.Columns[i].Name = tableCols[i];
                            }
                            dataGridView1.AllowUserToAddRows = false;
                            Query = "select a.GelombangId, a.BracketId, a.Type, a.ItemId 'FullItemId', a.ItemName, a.Base, a.Price, a.SeqNo, b.Ratio, c.GroupID, c.SubGroup1ID, c.SubGroup2ID, c.ItemID , c.UoM, c.UoM_AvgPrice, c.UoMAlt, d.DeliveryMethod, d.Price 'PLJPrice', d.SeqNo 'PLJSeqNo', d.PricelistNo, IGH.BracketDesc from InventGelombangD a left join InventGelombangH IGH on a.GelombangId = IGH.GelombangId and a.Type = IGH.Type left join InventConversion b on a.ItemId = b.FullItemId left join InventTable c on a.ItemId = c.FullItemID left join Pricelist_Dtl d on d.FullItemId = a.ItemId left join PricelistH e on e.PricelistNo = d.PricelistNo where (";
                            for (int i = 0; i < SearchV2.data.Count; i++)
                            {
                                if (i >= 1)
                                    Query += " or ";
                                Query += "a.GelombangID = '" + SearchV2.data[i] + "' and ";
                                Query += "a.BracketId = '" + SearchV2.data2[i] + "' and ";
                                Query += "(a.SeqNo = '" + SearchV2.data3[i] + "' or a.Base = 'Y')";
                            }
                            Query += ") and a.Type = 'Sales' and d.PricelistNo = '" + pricelistDtl[0] + "' and d.PriceType = '" + pricelistDtl[2] + "' order by a.GelombangId, a.SeqNo";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                #region add item to gv
                                this.dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", "", Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Base"], 0, 0, Dr["UoM"], 0, 0, Dr["UoMAlt"], Dr["PLJPrice"], Dr["Ratio"]);
                                if (Dr["Base"].ToString() == "Y")
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DeliveryMethod"].Value = Dr["DeliveryMethod"];

                                    cell = new DataGridViewComboBoxCell();
                                    cell.Items.Add("Select");
                                    cell.Items.Add("Warehouse");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["LockStatus"] = cell;

                                    cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;

                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price"].Value = Convert.ToDecimal(Dr["PLJPrice"]) * Convert.ToDecimal(Dr["Ratio"]);
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price_Alt"].Value = Convert.ToDecimal(Dr["PLJPrice"]);
                                }
                                else
                                {
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price"].Value = Convert.ToDecimal(Dr["Price"]) * Convert.ToDecimal(Dr["Ratio"]);
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price_Alt"].Value = Convert.ToDecimal(Dr["Price"]);
                                }

                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["GelombangId"].Value = Dr["GelombangId"];
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["BracketId"].Value = Dr["BracketId"];
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Gelombang_Price"].Value = Dr["Price"];
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["GelombangSeqNo_Base"].Value = Dr["SeqNo"];
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["BracketDesc"].Value = Dr["BracketDesc"];
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["PLJNo"].Value = Dr["PricelistNo"];
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["PLJSeqNo"].Value = Dr["PLJSeqNo"];
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["PLJPrice"].Value = Dr["PLJPrice"];
                                #endregion
                            }
                            Dr.Close();
                            Conn.Close();
                            gv1Format();
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView1.Columns["ItemName"].Frozen = true;


            if (Mode == "New")
                dataGridView1.Columns["RemainingQty"].Visible = false;
            else
                dataGridView1.Columns["RemainingQty"].Visible = true;
            dataGridView1.Columns["SalesAgreementNo"].Visible = false;
            dataGridView1.Columns["SeqNo"].Visible = false;
            dataGridView1.Columns["GroupID"].Visible = false;
            dataGridView1.Columns["SubGroup1ID"].Visible = false;
            dataGridView1.Columns["SubGroup2ID"].Visible = false;
            dataGridView1.Columns["ItemID"].Visible = false;
            if (cbSAType.Text != "QUANTITY")
            {
                dataGridView1.Columns["Qty_Alt"].Visible = false;
                dataGridView1.Columns["Unit_Alt"].Visible = false;
            }
            else
            {
                dataGridView1.Columns["Qty_Alt"].Visible = true;
                dataGridView1.Columns["Unit_Alt"].Visible = true;
            }
            dataGridView1.Columns["ConvertionRatio"].Visible = false;
            dataGridView1.Columns["Price_Alt"].Visible = false;
            dataGridView1.Columns["LockStatus"].Visible = false;
            dataGridView1.Columns["LockID"].Visible = false;
            dataGridView1.Columns["LockQty"].Visible = false;
            dataGridView1.Columns["SA_SQ_Id"].Visible = false;
            dataGridView1.Columns["SA_SQ_SeqNo"].Visible = false;
            dataGridView1.Columns["RefTransId"].Visible = false;
            dataGridView1.Columns["RefTrans_SeqNo"].Visible = false;
            dataGridView1.Columns["GelombangId"].Visible = false;
            dataGridView1.Columns["BracketId"].Visible = false;
            dataGridView1.Columns["Gelombang_Price"].Visible = false;
            dataGridView1.Columns["GelombangSeqNo_Base"].Visible = false;
            dataGridView1.Columns["BracketDesc"].Visible = false;
            if (cbSAType.Text == "FIX")
            {
                dataGridView1.Columns["Base"].Visible = false;
                dataGridView1.Columns["Price"].Visible = true;
                dataGridView1.Columns["Qty"].Visible = true;
                dataGridView1.Columns["Unit"].Visible = true;
            }
            else if (cbSAType.Text == "QUANTITY")
            {
                dataGridView1.Columns["Base"].Visible = true;
                dataGridView1.Columns["Price"].Visible = true;
                dataGridView1.Columns["Qty"].Visible = true;
                dataGridView1.Columns["Unit"].Visible = true;
            }
            else if (cbSAType.Text == "AMOUNT")
            {
                dataGridView1.Columns["Base"].Visible = true;
                dataGridView1.Columns["Qty"].Visible = false;
                dataGridView1.Columns["Unit"].Visible = false;
                dataGridView1.Columns["Price"].Visible = true;
            }
            //dataGridView1.Columns["BracketDesc"].Visible = false;
            dataGridView1.Columns["PLJNo"].Visible = false;
            dataGridView1.Columns["PLJSeqNo"].Visible = false;
            dataGridView1.Columns["PLJPrice"].Visible = false;
            dataGridView1.Columns["SubTotal_PPH"].Visible = false;

            dataGridView1.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Qty_Alt") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ConvertionRatio") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DiscPercent") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("LockQty"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Amount") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Price") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Total"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Amount") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Price") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Total") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Disc"))
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allowed numeric and one dot  ex. 10.23
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void cbSAType_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbCurrency_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbPaymentMode_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbDPType_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbPPh_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
            {
                dtp.Location = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                dtp.Visible = true;

                if (dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
                {
                    //DateTime dDate;
                    //if (!DateTime.TryParse(dataGridView1.CurrentCell.Value.ToString(), out dDate))
                    //{
                    //    dtp.Value = Convert.ToDateTime(FormateDateddmmyyyy(dataGridView1.CurrentCell.Value.ToString()));
                    //}
                    //else
                    //{
                    //dtp.Value = DateTime.Now;
                    if (dataGridView1.CurrentCell.Value.GetType() == typeof(DateTime))
                        dtp.Value = Convert.ToDateTime(dataGridView1.CurrentCell.Value);
                    else
                        dtp.Value = DateTime.ParseExact(dataGridView1.CurrentCell.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //}
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell.Value = dtp.Text;
        }

        //private string FormateDateddmmyyyy(string tmpDate)
        //{
        //    //string reformat="";
        //    if (tmpDate == "")
        //    {
        //        tmpDate = "01/01/1900";
        //    }
        //    //string reformat="";
        //    string[] data = tmpDate.Split('/');
        //    return data[2] + "/" + data[1] + "/" + data[0];
        //}


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
            {
                if (dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
                {
                    //dataGridView1.CurrentCell.Value = dtp.Value.Date.ToString("dd/MM/yyyy");
                    //dataGridView1.CurrentCell.Value = Convert.ToDateTime(dtp.Value);
                    dataGridView1.CurrentCell.Value = dtp.Value;
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }

            dtp.Visible = false;
            dataGridView1.AutoResizeColumns();
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            dtp.Visible = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Base"].Value.ToString() == "Y")
                {
                    int rowCount = dataGridView1.RowCount;
                    List<int> indexData = new List<int>();
                    indexData.Add(dataGridView1.CurrentRow.Index);
                    for (int i = dataGridView1.CurrentRow.Index + 1; i < rowCount; i++)
                    {
                        if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "N")
                            indexData.Add(i);
                        else
                            break;
                    }
                    for (int i = indexData.Count - 1; i >= 0; i--)
                    {
                        dataGridView1.Rows.RemoveAt(indexData[i]);
                    }
                }
                else
                    dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                populateGVData();
            }
        }

        private void createLabel(Control textbox, Control lblName, Control location, string type)
        {
            //if (validate == false)
            //{
            //    label[count] = new Label();
            //}
            if (label[count] == null)
            {
                label[count] = new Label();
            }
            if (type == "string")
            {
                if (textbox.Text == String.Empty || textbox.Text == "Select")
                {
                    textbox.BackColor = Color.LightGoldenrodYellow;

                    label[count].Text = "*";
                    label[count].ForeColor = Color.Red;
                    label[count].Width = 10;
                    label[count].Location = new System.Drawing.Point(lblName.Location.X - 9, lblName.Location.Y);
                    label[count].BringToFront();

                    location.Controls.Add(label[count]);
                    label[count].Visible = true;
                    flag = 'X';
                }
                else
                {
                    label[count].Visible = false;
                    textbox.BackColor = Color.Empty;
                }
            }
            else if (type == "decimal" || type == "int")
            {
                if (Convert.ToDecimal(textbox.Text) == 0)
                {
                    textbox.BackColor = Color.LightGoldenrodYellow;

                    label[count].Text = "*";
                    label[count].ForeColor = Color.Red;
                    label[count].Width = 10;
                    label[count].Location = new System.Drawing.Point(lblName.Location.X - 9, lblName.Location.Y);
                    label[count].BringToFront();

                    location.Controls.Add(label[count]);
                    label[count].Visible = true;
                    flag = 'X';
                }
                else
                {
                    label[count].Visible = false;
                    textbox.BackColor = Color.Empty;
                }
            }
            count++;
        }

        private char Validation()
        {
            try
            {
                flag = '\0'; msg = null;
                string msg2 = null, msg3 = null;
                if (validate == false) { label = new Label[20]; }

                createLabel(tbxCustID, lblCust, gbMain, "string");
                createLabel(tbxCustName, lblCust, gbMain, "string");
                createLabel(cbCurrency, lblCurrency, tabSales, "string");
                createLabel(cbPaymentMode, lblPaymentMode, tabSales, "string");
                createLabel(cbToP, lblToP, tabSales, "string");
                if (cbDPType.Text == "Y")
                {
                    //createLabel(tbxDPAmount, lblDPAmount, tabSales, "decimal");
                    createLabel(tbxDPPercent, lblDPAmount, tabSales, "decimal");
                }
                else
                {
                    //createLabel(tbxDPAmount, lblDPAmount, tabSales, "string");
                    createLabel(tbxDPPercent, lblDPAmount, tabSales, "string");
                }

                if (flag == 'X')
                    msg2 += "* field is required!\r\n";

                //validation detail
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() != "N")
                    {
                        if (cbSAType.Text != "AMOUNT")
                            if (dataGridView1.Rows[i].Cells["Qty"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) == 0)
                                msg += "Row " + Convert.ToInt32(i + 1) + " quantity cannot 0!\r\n";

                        if (dataGridView1.Rows[i].Cells["Price"].Value == String.Empty || dataGridView1.Rows[i].Cells["Price"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) == 0)
                            msg += "Row " + Convert.ToInt32(i + 1) + " price cannot 0!\r\n";

                        if (dataGridView1.Rows[i].Cells["DeliveryMethod"].Value.ToString() == "Select")
                            msg += "Row " + Convert.ToInt32(i + 1) + " Select Delivery Method!\n";

                        if (dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == String.Empty)
                            msg += "Row " + Convert.ToInt32(i + 1) + " Fill Expected Date From!\n";

                        if (dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == String.Empty)
                            msg += "Row " + Convert.ToInt32(i + 1) + " Fill Expected Date To!\n";

                        if (!(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == String.Empty || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == String.Empty))
                        {
                            int result = DateTime.Compare(Convert.ToDateTime(Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value).ToShortDateString()), Convert.ToDateTime(Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value).ToShortDateString()));
                            //string relationship;
                            if (result < 0) { }//relationship = "is earlier than";
                            else if (result == 0) { } //relationship = "is the same time as";
                            else //relationship = "is later than";
                                msg += "Row " + Convert.ToInt32(i + 1) + " Expected Date To (" + Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value).ToString("dd/MM/yyyy") + ") must be later than Date From (" + Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value).ToString("dd/MM/yyyy") + ")\n";
                        }

                        if (dataGridView1.Rows[i].Cells["DeliveryMethod"].Value.ToString().ToUpper() == "FRANCO")
                        {
                            if (dataGridView1.Rows[i].Cells["LogisticAmount"].Value == String.Empty || dataGridView1.Rows[i].Cells["LogisticAmount"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value) == 0)
                                msg += "Row " + Convert.ToInt32(i + 1) + " Logistik Amount tidak boleh 0 karena Franco!\n";
                        }

                        if (Mode != "New")
                        {
                            if (!(dataGridView1.Rows[i].Cells["SalesAgreementNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["SalesAgreementNo"].Value == null))
                            {
                                //CHECK WHEN EDIT
                                //QTY CANNOT SMALLER THAN REMAINING QTY
                                decimal RemainingQty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["RemainingQty"].Value);
                                decimal Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                                Conn = ConnectionString.GetConnection();
                                Query = "select Qty from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSAID.Text + "' and SeqNo = '" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                decimal oldSAQty = (Decimal)Cmd.ExecuteScalar();
                                Conn.Close();

                                RemainingQty = RemainingQty - (oldSAQty - Qty);
                                if (RemainingQty < 0)
                                    msg += "Row " + Convert.ToInt32(i + 1) + " Qty cannot smaller than " + Convert.ToDecimal(Qty - RemainingQty) + "!\r\n";
                            }
                        }
                    }
                }

                //STEVEN EDIT START
                if (cbByPhone.Checked == false && txtReferensi.Text.Trim() == "")//Jika tidak pakai "By Phone"
                {
                    if (dgvAttachment.RowCount > 0)//Ada Attachment tapi Reference kosong
                    {
                        //REMARK BY: HC 13.04.2018
                        //MessageBox.Show("Reference harus diisi karena tidak by Phone.");
                        //return;
                        msg3 += "Reference harus diisi karena tidak by Phone!\r\n"; //BY: HC 13.04.2018
                    }
                    else if (dgvAttachment.RowCount < 1)//Tidak Ada Attachment
                    {
                        //REMARK BY: HC 13.04.2018
                        //MessageBox.Show("Attachment harus ada.");
                        //return;
                        msg3 += "Attachment harus ada!\r\n"; //BY: HC 13.04.2018
                    }

                    //REMARKED BY: HC (S) 25.05.2018
                    //Conn = ConnectionString.GetConnection();
                    //SqlCommand check_Referensi = new SqlCommand("SELECT COUNT(*) FROM [tblRef] WHERE ([Referensi] = @Referensi)", Conn);
                    //check_Referensi.Parameters.AddWithValue("@Referensi", txtReferensi.Text);
                    //int ReferensiExist = (int)check_Referensi.ExecuteScalar();

                    //if (ReferensiExist > 0)
                    //{
                    //    //REMARK BY: HC 13.04.2018
                    //    //MessageBox.Show("Referensi sudah digunakan.");
                    //    //return;
                    //    msg += "Referensi sudah digunakan!\r\n"; //BY: HC 13.04.2018
                    //}
                    //REMARKED BY: HC (E)
                    //else
                    //{
                    //UserId doesn't exist.
                    //}
                }
                //STEVEN EDIT END

                if (dataGridView1.Rows.Count == 0)
                    msg += "Please add Item!\n";

                if (!(msg2 == null || msg2 == String.Empty))
                    metroTabControl1.SelectedTab = tabSales;
                else if (!(msg == null || msg == String.Empty))
                    metroTabControl1.SelectedTab = tabItemDetail;
                else if (!(msg3 == null || msg3 == String.Empty))
                    metroTabControl1.SelectedTab = tabDocuments;

                if (!(String.IsNullOrEmpty(msg)) || !(String.IsNullOrEmpty(msg2)) || !(String.IsNullOrEmpty(msg3)))
                {
                    MetroFramework.MetroMessageBox.Show(this, msg2 + msg + msg3, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    flag = 'X';
                }
                count = 0;
                validate = true;
                return flag;
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return flag;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation() == 'X')
                return;
            try
            {
                using (scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    string Jenis = "SA", Kode = "SA";

                    #region variable
                    string SAID = ""; ;
                    if (tbxRefID.Text == String.Empty)
                    {
                        SAID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    }
                    else
                    {
                        SAID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "SalesAgreementH", tbxRefID.Text, Conn, Trans, Cmd);
                    }
                    string refId = tbxRefID.Text;
                    string salesType = cbSAType.Text;
                    string mouID = tbxMOUID.Text;
                    string SQID = tbxSQID.Text;
                    string custID = tbxCustID.Text;
                    string custName = tbxCustName.Text;
                    string reff = txtReferensi.Text;
                    string currency = cbCurrency.Text;
                    decimal xRate = Convert.ToDecimal(tbxXRate.Text);
                    DateTime SADate = dtSA.Value;
                    string ToP = cbToP.Text;
                    Cmd = new SqlCommand("select [PaymentModeID] from [PaymentMode] where PaymentModeName = '" + cbPaymentMode.Text + "'", Conn);
                    string paymentMode = Cmd.ExecuteScalar().ToString();
                    string dpType = cbDPType.Text;
                    decimal dpAmount = 0;
                    decimal dpPercent = 0;
                    if (dpType != "Not Required")
                    {
                        dpAmount = Convert.ToDecimal(tbxDPAmount.Text);
                        dpPercent = Convert.ToDecimal(tbxDPPercent.Text);
                    }
                    DateTime dpDate = dtDP.Value;
                    DateTime saDueDate = dtSADue.Value;
                    decimal sTotal = Convert.ToDecimal(tbxSTotal.Text);
                    decimal gDisc = Convert.ToDecimal(tbxGDisc.Text);
                    decimal gPPN = Convert.ToDecimal(tbxGPPN.Text);
                    decimal gPPh = Convert.ToDecimal(tbxGPPh.Text);
                    decimal gTotal = Convert.ToDecimal(tbxGTotal.Text);
                    decimal gBonus = Convert.ToDecimal(tbxGBonus.Text);
                    decimal gCashBack = Convert.ToDecimal(tbxGCashBack.Text);
                    decimal ppn = Convert.ToDecimal(cbPPN.Text);
                    decimal pph = Convert.ToDecimal(cbPPh.Text);
                    string notes = tbxNotes.Text;
                    string Referensi = txtReferensi.Text;
                    string TransStatus = "";
                    #endregion

                    if (tbxMOUID.Text.Trim() != "")
                    {
                        if (!(ListMethod.checkCustomerMoU(tbxMOUID.Text, custID, gTotal)))
                            return;
                    }
                    else
                    {
                        if (!(ListMethod.checkCreditLimit("Ask", custID, gTotal)))
                            return;
                    }

                    if (Mode == "New")
                    {
                        tbxSAID.Text = SAID;

                        if (tbxRefID.Text == String.Empty)
                        {
                            flag = checkPriceTolerance();
                            if (flag == 'X')
                                TransStatus = "11";
                            else
                                TransStatus = "01";
                        }
                        else
                            TransStatus = "09";

                        insertSAHeader(SAID, refId, salesType, mouID, SQID, custID, custName, reff, currency, xRate, SADate, ToP, paymentMode, dpType, dpAmount, dpPercent, dpDate, saDueDate, sTotal, gDisc, gPPN, gPPh, gTotal, gBonus, gCashBack, ppn, pph, notes, Referensi, TransStatus);

                        //SET AMENDED SA STATUS TO CLOSED
                        if (tbxRefID.Text != String.Empty)
                        {
                            Query = "update SalesAgreementH set TransStatus = '04', UpdatedBy = '" + ControlMgr.UserId + "', UpdatedDate = getdate() where SalesAgreementNo = '" + tbxRefID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            ListMethod.insertLogTable("E", "SalesAgreement_LogTable", tbxRefID.Text, "", "SA", "04");
                        }
                        ListMethod.insertLogTable("N", "SalesAgreement_LogTable", SAID, "", "SA", TransStatus);

                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            #region variable
                            int seqNo = Convert.ToInt32(i + 1);
                            string GroupID = dataGridView1.Rows[i].Cells["GroupID"].Value.ToString();
                            string SubGroup1ID = dataGridView1.Rows[i].Cells["SubGroup1ID"].Value.ToString();
                            string SubGroup2ID = dataGridView1.Rows[i].Cells["SubGroup2ID"].Value.ToString();
                            string ItemID = dataGridView1.Rows[i].Cells["ItemID"].Value.ToString();
                            string FullItemID = dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString();
                            string ItemName = dataGridView1.Rows[i].Cells["ItemName"].Value.ToString();
                            decimal Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                            string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();
                            decimal Price = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value);
                            decimal Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Alt"].Value);
                            string Unit_Alt = dataGridView1.Rows[i].Cells["Unit_Alt"].Value.ToString();
                            decimal Price_Alt = 0;
                            if (!(dataGridView1.Rows[i].Cells["Price_Alt"].Value == null || dataGridView1.Rows[i].Cells["Price_Alt"].Value == String.Empty))
                                Price_Alt = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price_Alt"].Value);
                            decimal ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["ConvertionRatio"].Value);
                            string DeliveryMethod = "";
                            if (!(dataGridView1.Rows[i].Cells["DeliveryMethod"].Value == null || dataGridView1.Rows[i].Cells["DeliveryMethod"].Value == String.Empty))
                                DeliveryMethod = dataGridView1.Rows[i].Cells["DeliveryMethod"].Value.ToString();
                            DateTime ExpectedDateFrom = new DateTime(1753, 01, 01);
                            if (!(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value.ToString() == String.Empty))
                                ExpectedDateFrom = Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value);
                            DateTime ExpectedDateTo = new DateTime(1753, 01, 01);
                            if (!(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value.ToString() == String.Empty))
                                ExpectedDateTo = Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value);
                            decimal SubTotal = 0;
                            if (!(dataGridView1.Rows[i].Cells["SubTotal"].Value == String.Empty || dataGridView1.Rows[i].Cells["SubTotal"].Value == null))
                                SubTotal = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value);
                            decimal SubTotal_PPN = 0;
                            if (!(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value == String.Empty || dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value == null))
                                SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value);
                            decimal SubTotal_PPH = 0;
                            if (!(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value == String.Empty || dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value == null))
                                SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value);
                            decimal LogisticAmount = 0;
                            if (!(dataGridView1.Rows[i].Cells["LogisticAmount"].Value == String.Empty || dataGridView1.Rows[i].Cells["LogisticAmount"].Value == null))
                                LogisticAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value);
                            string LogisticNotes = "";
                            if (!(dataGridView1.Rows[i].Cells["LogisticNotes"].Value == null || dataGridView1.Rows[i].Cells["LogisticNotes"].Value == String.Empty))
                                LogisticNotes = dataGridView1.Rows[i].Cells["LogisticNotes"].Value.ToString();
                            string DiscType = "";
                            if (dataGridView1.Rows[i].Cells["DiscType"].Value == String.Empty || dataGridView1.Rows[i].Cells["DiscType"].Value == null || dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Select")
                                DiscType = "1";
                            else
                            {
                                Cmd = new SqlCommand("Select DiskonSchemeID from DiskonScheme where Deskripsi = '" + dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() + "'", Conn);
                                DiscType = Cmd.ExecuteScalar().ToString();
                            }
                            decimal DiscPercent = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscPercent"].Value);
                            decimal DiscAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value);
                            decimal BonusAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["BonusAmount"].Value);
                            decimal CashBackAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["CashBackAmount"].Value);
                            string Notes = "";
                            if (!(dataGridView1.Rows[i].Cells["Notes"].Value == String.Empty || dataGridView1.Rows[i].Cells["Notes"].Value == null))
                                Notes = dataGridView1.Rows[i].Cells["Notes"].Value.ToString();
                            string LockStatus = "";
                            if (!(dataGridView1.Rows[i].Cells["LockStatus"].Value == null || dataGridView1.Rows[i].Cells["LockStatus"].Value == String.Empty))
                                LockStatus = dataGridView1.Rows[i].Cells["LockStatus"].Value.ToString();
                            string LockID = "";
                            if (!(dataGridView1.Rows[i].Cells["LockID"].Value == String.Empty || dataGridView1.Rows[i].Cells["LockID"].Value == null))
                                LockID = dataGridView1.Rows[i].Cells["LockID"].Value.ToString();
                            decimal LockQty = 0;
                            if (!(dataGridView1.Rows[i].Cells["LockQty"].Value == String.Empty || dataGridView1.Rows[i].Cells["LockQty"].Value == null))
                                LockQty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["LockQty"].Value);
                            string SA_SQ_Id = "";
                            if (!(dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == null || dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == (object)DBNull.Value))
                                SA_SQ_Id = dataGridView1.Rows[i].Cells["SA_SQ_Id"].Value.ToString();
                            int SA_SQ_SeqNo = 0;
                            if (!(dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == null || dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == (object)DBNull.Value))
                                SA_SQ_SeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value);
                            string RefTransId = "";
                            if (!(dataGridView1.Rows[i].Cells["RefTransId"].Value == null || dataGridView1.Rows[i].Cells["RefTransId"].Value == String.Empty))
                                RefTransId = dataGridView1.Rows[i].Cells["RefTransId"].Value.ToString();
                            int RefTrans_SeqNo = 0;
                            if (!(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value == String.Empty))
                                RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value);
                            string GelombangId = "";
                            if (!(dataGridView1.Rows[i].Cells["GelombangId"].Value == null || dataGridView1.Rows[i].Cells["GelombangId"].Value == String.Empty))
                                GelombangId = dataGridView1.Rows[i].Cells["GelombangId"].Value.ToString();
                            string BracketId = "";
                            if (!(dataGridView1.Rows[i].Cells["BracketId"].Value == String.Empty || dataGridView1.Rows[i].Cells["BracketId"].Value == null))
                                BracketId = dataGridView1.Rows[i].Cells["BracketId"].Value.ToString();
                            string Base = "";
                            if (!(dataGridView1.Rows[i].Cells["Base"].Value == null || dataGridView1.Rows[i].Cells["Base"].Value == String.Empty))
                                Base = dataGridView1.Rows[i].Cells["Base"].Value.ToString();
                            decimal Gelombang_Price = 0;
                            if (!(dataGridView1.Rows[i].Cells["Gelombang_Price"].Value == null || dataGridView1.Rows[i].Cells["Gelombang_Price"].Value == String.Empty))
                                Gelombang_Price = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Gelombang_Price"].Value);
                            int GelombangSeqNo_Base = 0;
                            if (!(dataGridView1.Rows[i].Cells["GelombangSeqNo_Base"].Value == null || dataGridView1.Rows[i].Cells["GelombangSeqNo_Base"].Value == String.Empty))
                                GelombangSeqNo_Base = Convert.ToInt32(dataGridView1.Rows[i].Cells["GelombangSeqNo_Base"].Value);
                            string BracketDesc = "";
                            if (!(dataGridView1.Rows[i].Cells["BracketDesc"].Value == null || dataGridView1.Rows[i].Cells["BracketDesc"].Value == String.Empty))
                                BracketDesc = dataGridView1.Rows[i].Cells["BracketDesc"].Value.ToString();
                            string PLJNo = "";
                            if (!(dataGridView1.Rows[i].Cells["PLJNo"].Value == null || dataGridView1.Rows[i].Cells["PLJNo"].Value == String.Empty))
                                PLJNo = dataGridView1.Rows[i].Cells["PLJNo"].Value.ToString();
                            int PLJSeqNo = 0;
                            if (!(dataGridView1.Rows[i].Cells["PLJSeqNo"].Value == null || dataGridView1.Rows[i].Cells["PLJSeqNo"].Value == String.Empty))
                                PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["PLJSeqNo"].Value);
                            decimal PLJPrice = 0;
                            if (!(dataGridView1.Rows[i].Cells["PLJPrice"].Value == null || dataGridView1.Rows[i].Cells["PLJPrice"].Value == String.Empty))
                                PLJPrice = Convert.ToDecimal(dataGridView1.Rows[i].Cells["PLJPrice"].Value);

                            decimal RemainingQty = 0;
                            if (cbSAType.Text == "QUANTITY")
                                RemainingQty = Qty;
                            else if (cbSAType.Text == "AMOUNT")
                            {
                                if (!(dataGridView1.Rows[i].Cells["SubTotal"].Value == String.Empty || dataGridView1.Rows[i].Cells["SubTotal"].Value == null))
                                    RemainingQty = SubTotal + SubTotal_PPH + SubTotal_PPN - DiscAmount;
                            }
                            #endregion
                            insertSADetail(SAID, seqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, Qty, RemainingQty, Unit, Price, Qty_Alt, Unit_Alt, Price_Alt, ConvertionRatio, DeliveryMethod, ExpectedDateFrom, ExpectedDateTo, SubTotal, SubTotal_PPN, SubTotal_PPH, LogisticAmount, LogisticNotes, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, Notes, LockStatus, LockID, LockQty, SA_SQ_Id, SA_SQ_SeqNo, RefTransId, RefTrans_SeqNo, GelombangId, BracketId, Base, Gelombang_Price, GelombangSeqNo_Base, BracketDesc, PLJNo, PLJSeqNo, PLJPrice);
                        }
                        if (tbxSQID.Text != "")
                            updateSQHeaderStats(SQID);
                    }
                    else if (Mode == "Edit")
                    {
                        if (tbxRefID.Text == String.Empty)
                        {
                            flag = checkPriceTolerance();
                            if (flag == 'X')
                                TransStatus = "11";
                            else
                                TransStatus = "01";
                        }
                        else
                            TransStatus = "09";

                        updateSAHeader(tbxSAID.Text, refId, salesType, mouID, SQID, custID, custName, reff, currency, xRate, SADate, ToP, paymentMode, dpType, dpAmount, dpPercent, dpDate, saDueDate, sTotal, gDisc, gPPN, gPPh, gTotal, gBonus, gCashBack, ppn, pph, notes, TransStatus);
                        ListMethod.insertLogTable("E", "SalesAgreement_LogTable", tbxSAID.Text, "", "SA", TransStatus);

                        checkIfItemStillExist(tbxSAID.Text);

                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            #region variable for SA Detail
                            int seqNo = 0;
                            if (!(dataGridView1.Rows[i].Cells["seqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["seqNo"].Value == null))
                                seqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["seqNo"].Value);
                            string GroupID = dataGridView1.Rows[i].Cells["GroupID"].Value.ToString();
                            string SubGroup1ID = dataGridView1.Rows[i].Cells["SubGroup1ID"].Value.ToString();
                            string SubGroup2ID = dataGridView1.Rows[i].Cells["SubGroup2ID"].Value.ToString();
                            string ItemID = dataGridView1.Rows[i].Cells["ItemID"].Value.ToString();
                            string FullItemID = dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString();
                            string ItemName = dataGridView1.Rows[i].Cells["ItemName"].Value.ToString();
                            decimal Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                            string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();
                            decimal Price = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value);
                            decimal Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Alt"].Value);
                            string Unit_Alt = dataGridView1.Rows[i].Cells["Unit_Alt"].Value.ToString();
                            decimal Price_Alt = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price_Alt"].Value);
                            decimal ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["ConvertionRatio"].Value);
                            string DeliveryMethod = "";
                            if (!(dataGridView1.Rows[i].Cells["DeliveryMethod"].Value == null || dataGridView1.Rows[i].Cells["DeliveryMethod"].Value == String.Empty))
                                DeliveryMethod = dataGridView1.Rows[i].Cells["DeliveryMethod"].Value.ToString();
                            DateTime ExpectedDateFrom = new DateTime(1753, 01, 01);
                            if (!(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == String.Empty || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == (object)DBNull.Value))
                                ExpectedDateFrom = Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value);
                            DateTime ExpectedDateTo = new DateTime(1753, 01, 01);
                            if (!(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == String.Empty || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == (object)DBNull.Value))
                                ExpectedDateTo = Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value);
                            decimal SubTotal = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value);
                            decimal SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value);
                            decimal SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value);
                            decimal LogisticAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value);
                            string LogisticNotes = "";
                            if (!(dataGridView1.Rows[i].Cells["LogisticNotes"].Value == String.Empty || dataGridView1.Rows[i].Cells["LogisticNotes"].Value == null))
                                LogisticNotes = dataGridView1.Rows[i].Cells["LogisticNotes"].Value.ToString();
                            string DiscType = "";
                            if (dataGridView1.Rows[i].Cells["DiscType"].Value == String.Empty || dataGridView1.Rows[i].Cells["DiscType"].Value == null || dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Select")
                                DiscType = "1";
                            else
                            {
                                Cmd = new SqlCommand("Select DiskonSchemeID from DiskonScheme where Deskripsi = '" + dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() + "'", Conn);
                                DiscType = Cmd.ExecuteScalar().ToString();
                            }
                            decimal DiscPercent = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscPercent"].Value);
                            decimal DiscAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value);
                            decimal BonusAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["BonusAmount"].Value);
                            decimal CashBackAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["CashBackAmount"].Value);
                            string Notes = "";
                            if (!(dataGridView1.Rows[i].Cells["Notes"].Value == String.Empty || dataGridView1.Rows[i].Cells["Notes"].Value == null))
                                Notes = dataGridView1.Rows[i].Cells["Notes"].Value.ToString();
                            string LockStatus = "";
                            if (!(dataGridView1.Rows[i].Cells["LockStatus"].Value == null || dataGridView1.Rows[i].Cells["LockStatus"].Value == String.Empty))
                                LockStatus = dataGridView1.Rows[i].Cells["LockStatus"].Value.ToString();
                            string LockID = "";
                            if (!(dataGridView1.Rows[i].Cells["LockID"].Value == String.Empty || dataGridView1.Rows[i].Cells["LockID"].Value == null))
                                LockID = dataGridView1.Rows[i].Cells["LockID"].Value.ToString();
                            decimal LockQty = 0;
                            if (!(dataGridView1.Rows[i].Cells["LockQty"].Value == String.Empty || dataGridView1.Rows[i].Cells["LockQty"].Value == null))
                                LockQty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["LockQty"].Value);
                            string SA_SQ_Id = "";
                            if (!(dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == null || dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == String.Empty))
                                SA_SQ_Id = dataGridView1.Rows[i].Cells["SA_SQ_Id"].Value.ToString();
                            int SA_SQ_SeqNo = 0;
                            if (!(dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == null || dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value == String.Empty))
                                SA_SQ_SeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value);
                            string RefTransId = "";
                            if (!(dataGridView1.Rows[i].Cells["RefTransId"].Value == null || dataGridView1.Rows[i].Cells["RefTransId"].Value == String.Empty))
                                RefTransId = dataGridView1.Rows[i].Cells["RefTransId"].Value.ToString();
                            int RefTrans_SeqNo = 0;
                            if (!(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value == String.Empty))
                                RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value);
                            string GelombangId = "";
                            if (!(dataGridView1.Rows[i].Cells["GelombangId"].Value == null || dataGridView1.Rows[i].Cells["GelombangId"].Value == String.Empty))
                                GelombangId = dataGridView1.Rows[i].Cells["GelombangId"].Value.ToString();
                            string BracketId = "";
                            if (!(dataGridView1.Rows[i].Cells["BracketId"].Value == String.Empty || dataGridView1.Rows[i].Cells["BracketId"].Value == null))
                                BracketId = dataGridView1.Rows[i].Cells["BracketId"].Value.ToString();
                            string Base = "";
                            if (!(dataGridView1.Rows[i].Cells["Base"].Value == null || dataGridView1.Rows[i].Cells["Base"].Value == String.Empty))
                                Base = dataGridView1.Rows[i].Cells["Base"].Value.ToString();
                            decimal Gelombang_Price = 0;
                            if (!(dataGridView1.Rows[i].Cells["Gelombang_Price"].Value == null || dataGridView1.Rows[i].Cells["Gelombang_Price"].Value == String.Empty))
                                Gelombang_Price = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Gelombang_Price"].Value);
                            int GelombangSeqNo_Base = 0;
                            if (!(dataGridView1.Rows[i].Cells["GelombangSeqNo_Base"].Value == null || dataGridView1.Rows[i].Cells["GelombangSeqNo_Base"].Value == String.Empty))
                                GelombangSeqNo_Base = Convert.ToInt32(dataGridView1.Rows[i].Cells["GelombangSeqNo_Base"].Value);
                            string BracketDesc = "";
                            if (!(dataGridView1.Rows[i].Cells["BracketDesc"].Value == null || dataGridView1.Rows[i].Cells["BracketDesc"].Value == String.Empty))
                                BracketDesc = dataGridView1.Rows[i].Cells["BracketDesc"].Value.ToString();
                            string PLJNo = "";
                            if (!(dataGridView1.Rows[i].Cells["PLJNo"].Value == null || dataGridView1.Rows[i].Cells["PLJNo"].Value == String.Empty))
                                PLJNo = dataGridView1.Rows[i].Cells["PLJNo"].Value.ToString();
                            int PLJSeqNo = 0;
                            if (!(dataGridView1.Rows[i].Cells["PLJSeqNo"].Value == null || dataGridView1.Rows[i].Cells["PLJSeqNo"].Value == String.Empty))
                                PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["PLJSeqNo"].Value);
                            decimal PLJPrice = 0;
                            if (!(dataGridView1.Rows[i].Cells["PLJPrice"].Value == null || dataGridView1.Rows[i].Cells["PLJPrice"].Value == String.Empty))
                                PLJPrice = Convert.ToDecimal(dataGridView1.Rows[i].Cells["PLJPrice"].Value);
                            decimal RemainingQty = 0;
                            if (cbSAType.Text == "QUANTITY")
                                RemainingQty = Qty;
                            else if (cbSAType.Text == "AMOUNT")
                            {
                                if (!(dataGridView1.Rows[i].Cells["SubTotal"].Value == String.Empty || dataGridView1.Rows[i].Cells["SubTotal"].Value == null))
                                    RemainingQty = SubTotal + SubTotal_PPH + SubTotal_PPN - DiscAmount;
                            }
                            #endregion
                            Query = "select top 1 SalesAgreementNo from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSAID.Text + "' and SeqNo = '" + seqNo + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                updateSADetail(tbxSAID.Text, seqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, Qty, RemainingQty, Unit, Price, Qty_Alt, Unit_Alt, Price_Alt, ConvertionRatio, DeliveryMethod, ExpectedDateFrom, ExpectedDateTo, SubTotal, SubTotal_PPN, SubTotal_PPH, LogisticAmount, LogisticNotes, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, Notes, LockStatus, LockID, LockQty, SA_SQ_Id, SA_SQ_SeqNo, RefTransId, RefTrans_SeqNo, GelombangId, BracketId, Base, Gelombang_Price, GelombangSeqNo_Base, BracketDesc, PLJNo, PLJSeqNo, PLJPrice);
                            }
                            else
                            {
                                RemainingQty = Qty;
                                SAID = tbxSAID.Text;
                                //GET LAST SEQNO 
                                Query = "select top 1 SeqNo from SalesAgreement_Dtl where SalesAgreementNo = '" + SAID + "' order by SeqNo desc";
                                Cmd = new SqlCommand(Query, Conn);
                                seqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;
                                insertSADetail(SAID, seqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, Qty, RemainingQty, Unit, Price, Qty_Alt, Unit_Alt, Price_Alt, ConvertionRatio, DeliveryMethod, ExpectedDateFrom, ExpectedDateTo, SubTotal, SubTotal_PPN, SubTotal_PPH, LogisticAmount, LogisticNotes, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, Notes, LockStatus, LockID, LockQty, SA_SQ_Id, SA_SQ_SeqNo, RefTransId, RefTrans_SeqNo, GelombangId, BracketId, Base, Gelombang_Price, GelombangSeqNo_Base, BracketDesc, PLJNo, PLJSeqNo, PLJPrice);
                            }
                            Dr.Close();
                        }
                    }


                    //STEVEN EDIT BEGIN DELETE OLD + ADD NEW ATTACHMENT
                    Query = "Delete from tblAttachments where ReffTableName='SalesAgreementH' And ReffTransId='" + tbxSAID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    SaveDgvAttachmentData();
                    //STEVEN EDIT END

                    //STEVEN EDIT START INSERT TO tblRef
                    #region referensi
                    if (cbByPhone.Checked == false && txtReferensi.Text.Trim() != "") //Referensi + Attachment tidak kosong
                    {
                        //BY: HC (S) 25.05.2018
                        if (vNewReferensi == vOldReferensi)
                        {
                            Query = "update tblRef set UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where Referensi = '" + vOldReferensi + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            //delete Old reference
                            Query = "DELETE FROM [dbo].[tblRef] WHERE [Referensi] = '" + vOldReferensi + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            Query = "select * from tblRef where Referensi = '" + txtReferensi.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            if (!(Dr.HasRows))
                            {
                                Query = "insert into tblRef values ('" + txtReferensi.Text + "', '" + ControlMgr.UserId + "', getdate(), NULL, NULL)";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    #endregion
                    //STEVEN EDIT END
                    Conn.Close();

                    ListMethod.StatusLogCustomer("SAHeader", "SA", tbxCustID.Text, TransStatus, "", tbxSAID.Text, "", "", "");

                    scope.Complete();
                }
                ModeBeforeEdit();
                GetDataHeader();
                gv1Format();
                Parent.RefreshGrid();
                MetroFramework.MetroMessageBox.Show(this, tbxSAID.Text + " save success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //catch (Exception ex)
            //{
            //    MetroFramework.MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            finally { }
        }
        

        private void checkIfItemStillExist(string SAID)
        {
            string where = " and SeqNo not in (";
            flag = '\0';
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (i >= 1 && dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() != String.Empty)
                    where += ",";
                if (dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() != String.Empty)
                {
                    flag = 'X';
                    where += "'" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                }
            }
            where += ")";

            if (flag == '\0')
                where = "";
            //UPDATE DETAIL STATUS TO CLOSED IF GV ITEM != DB ITEM
            Query = "update SalesAgreement_Dtl set Deleted = 'Y', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + SAID + "'" + where + "";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateSQHeaderStats(string SQID)
        {
            Query = "update SalesQuotationH set TransStatus = '10' where SalesQuotationNo = '" + SQID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            ListMethod.insertLogTableFollowParentAct("SalesQuotation_LogTable", SQID, "", "SalesQuotation", "10");           
        }

        private char checkPriceTolerance()
        {
            flag = '\0';
            for (int j = 0; j < dataGridView1.Rows.Count; j++)
            {
                if (dataGridView1.Rows[j].Cells["Base"].Value.ToString() != "N")
                {
                    Cmd = new SqlCommand("select a.Tolerance from Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo where a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and b.Active = '1' and b.TransStatus = '03' and cast(a.PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and a.FullItemId = '" + dataGridView1.Rows[j].Cells["FullItemId"].Value + "' and a.PriceListNo = '" + dataGridView1.Rows[j].Cells["PLJNo"].Value + "' and a.SeqNo = '" + dataGridView1.Rows[j].Cells["PLJSeqNo"].Value + "'", Conn);
                    Dr = Cmd.ExecuteReader();
                    List<decimal> salesPrice = new List<decimal>();
                    decimal tolerance = 0;
                    while (Dr.Read())
                    {
                        tolerance = Convert.ToDecimal(Dr["Tolerance"]);
                    }
                    Dr.Close();

                    decimal maxPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value) + (Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value) * tolerance / 100);
                    if (Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value) > maxPrice || Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value) < Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value))
                    {
                        flag = 'X';
                        return flag;
                    }
                }
            }
            return flag;
        }

        private void updateSADetail(string SAID, int seqNo, string GroupID, string SubGroup1ID, string SubGroup2ID, string ItemID, string FullItemID, string ItemName, decimal Qty, decimal RemainingQty, string Unit, decimal Price, decimal Qty_Alt, string Unit_Alt, decimal Price_Alt, decimal ConvertionRatio, string DeliveryMethod, DateTime ExpectedDateFrom, DateTime ExpectedDateTo, decimal SubTotal, decimal SubTotal_PPN, decimal SubTotal_PPH, decimal LogisticAmount, string LogisticNotes, string DiscType, decimal DiscPercent, decimal DiscAmount, decimal BonusAmount, decimal CashBackAmount, string Notes, string LockStatus, string LockID, decimal LockQty, string SA_SQ_Id, int SA_SQ_SeqNo, string RefTransId, int RefTrans_SeqNo, string GelombangId, string BracketId, string Base, decimal Gelombang_Price, int GelombangSeqNo_Base, string BracketDesc, string PLJNo, int PLJSeqNo, decimal PLJPrice)
        {
            Query = "select Qty from SalesAgreement_Dtl where SalesAgreementNo = '" + SAID + "' and SeqNo = '" + seqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            decimal oldSAQty = (Decimal)Cmd.ExecuteScalar();

            RemainingQty = RemainingQty - (oldSAQty - Qty);

            Query = "UPDATE [dbo].[SalesAgreement_Dtl] SET [GroupID] = '" + GroupID + "', [SubGroup1ID] = '" + SubGroup1ID + "', [SubGroup2ID] = '" + SubGroup2ID + "', [ItemID] = '" + ItemID + "',[FullItemID] = '" + FullItemID + "',[ItemName] = '" + ItemName + "',[DeliveryMethod] = @DeliveryMethod,[LogisticAmount] = '" + LogisticAmount + "',[LogisticNotes] = @LogisticNotes, [ExpectedDateFrom] = @ExpectedDateFrom,[ExpectedDateTo] = @ExpectedDateTo, [Qty] = '" + Qty + "', [Unit] = '" + Unit + "',[Qty_Alt] = '" + Qty_Alt + "', [Unit_Alt] = '" + Unit_Alt + "', [ConvertionRatio] = '" + ConvertionRatio + "', [Price] = '" + Price + "',[Price_Alt] = '" + Price_Alt + "', [DiscType] = '" + DiscType + "', [DiscPercent] = '" + DiscPercent + "',[DiscAmount] = '" + DiscAmount + "', [BonusAmount] = '" + BonusAmount + "', [CashBackAmount] = '" + CashBackAmount + "',[SubTotal] = '" + SubTotal + "',[SubTotal_PPN] = '" + SubTotal_PPN + "', [SubTotal_PPH] = '" + SubTotal_PPH + "',[Notes] = @Notes, [LockStatus] = @LockStatus, [LockID] = @LockID,[LockQty] = '" + LockQty + "', [RemainingQty] = '" + RemainingQty + "',[SA_SQ_Id] = @SA_SQ_Id,[SA_SQ_SeqNo] = @SA_SQ_SeqNo,[RefTransId] = @RefTransId, [RefTrans_SeqNo] = @RefTrans_SeqNo, [GelombangId] = @GelombangId, [BracketId] = @BracketId, [Base] = @Base, [Gelombang_Price] = '" + Gelombang_Price + "', [GelombangSeqNo_Base] = @GelombangSeqNo_Base, [BracketDesc] = @BracketDesc, [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.UserId + "', PLJNo = @PLJNo, PLJSeqNo = @PLJSeqNo, PLJPrice = @PLJPrice WHERE SalesAgreementNo = '" + SAID + "' and SeqNo = '" + seqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DeliveryMethod", DeliveryMethod == String.Empty ? (object)DBNull.Value : DeliveryMethod);
            Cmd.Parameters.AddWithValue("@ExpectedDateFrom", Base == "N" ? (object)DBNull.Value : ExpectedDateFrom);
            Cmd.Parameters.AddWithValue("@ExpectedDateTo", Base == "N" ? (object)DBNull.Value : ExpectedDateTo);
            Cmd.Parameters.AddWithValue("@LogisticNotes", LogisticNotes);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.Parameters.AddWithValue("@LockStatus", LockStatus == "Select" ? (object)DBNull.Value : LockStatus);
            Cmd.Parameters.AddWithValue("@LockID", LockID == String.Empty ? (object)DBNull.Value : LockStatus);
            Cmd.Parameters.AddWithValue("@SA_SQ_Id", SA_SQ_Id == String.Empty ? (object)DBNull.Value : SA_SQ_Id);
            Cmd.Parameters.AddWithValue("@SA_SQ_SeqNo", SA_SQ_SeqNo == 0 ? (object)DBNull.Value : SA_SQ_SeqNo);
            Cmd.Parameters.AddWithValue("@RefTransId", RefTransId == String.Empty ? (object)DBNull.Value : RefTransId);
            Cmd.Parameters.AddWithValue("@RefTrans_SeqNo", RefTrans_SeqNo == 0 ? (object)DBNull.Value : RefTrans_SeqNo);
            Cmd.Parameters.AddWithValue("@GelombangId", GelombangId == String.Empty ? (object)DBNull.Value : GelombangId);
            Cmd.Parameters.AddWithValue("@BracketId", BracketId == String.Empty ? (object)DBNull.Value : BracketId);
            Cmd.Parameters.AddWithValue("@Base", Base == String.Empty ? (object)DBNull.Value : Base);
            Cmd.Parameters.AddWithValue("@GelombangSeqNo_Base", GelombangSeqNo_Base == 0 ? (object)DBNull.Value : GelombangSeqNo_Base);
            Cmd.Parameters.AddWithValue("@BracketDesc", BracketDesc);
            Cmd.Parameters.AddWithValue("@PLJNo", PLJNo == String.Empty ? (object)DBNull.Value : PLJNo);
            Cmd.Parameters.AddWithValue("@PLJSeqNo", PLJSeqNo == 0 ? (object)DBNull.Value : PLJSeqNo);
            Cmd.Parameters.AddWithValue("@PLJPrice", PLJNo == String.Empty ? (object)DBNull.Value : PLJPrice);
            Cmd.ExecuteNonQuery();
        }

        private void updateSAHeader(string SAID, string refId, string salesType, string mouID, string SQID, string custID, string custName, string reff, string currency, decimal xRate, DateTime SADate, string ToP, string paymentMode, string dpType, decimal dpAmount, decimal dpPercent, DateTime dpDate, DateTime saDueDate, decimal sTotal, decimal gDisc, decimal gPPN, decimal gPPh, decimal gTotal, decimal gBonus, decimal gCashBack, decimal ppn, decimal pph, string notes,string TransStatus)
        {
            Query = "UPDATE [dbo].[SalesAgreementH] SET [TransType] = '" + salesType + "', [CustID] = '" + custID + "',[CustName] = '" + custName + "', [CurrencyID] = '" + currency + "', [ExchRate] = '" + xRate + "', [Total] = '" + sTotal + "',[Total_Disk] = '" + gDisc + "', [PPN] = '" + ppn + "', [Total_PPN] = '" + gPPN + "', [PPH] = '" + pph + "',[Total_PPH] = '" + gPPh + "',[Total_Nett] = '" + gTotal + "',[Total_Bonus] = '" + gBonus + "',[Total_Cashback] = '" + gCashBack + "', [TermofPayment] = '" + ToP + "', [PaymentModeID] = @PaymentModeID, [DPType] = '" + dpType + "', [DPPercent] = '" + dpPercent + "',[DPAmount] = '" + dpAmount + "',[DPDueDate] = @dpDate, [Notes] = @Notes,[ValidTo] = @validTo, [Referensi] = '" + reff + "', [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.UserId + "' ";
            //if (tbxRefID.Text == String.Empty)
            //{
            //    flag = checkPriceTolerance();
            //    if (flag == 'X')
            //        Query += ", TransStatus = '11' ";
            //    else
            //        Query += ", TransStatus = '01' ";
            //}
            //else
            //    Query += ", TransStatus = '09'";
            Query += ",TransStatus = '" + TransStatus + "'";
            Query += "WHERE [SalesAgreementNo] = '" + SAID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@Notes", notes);
            Cmd.Parameters.AddWithValue("@validTo", saDueDate);
            Cmd.Parameters.AddWithValue("PaymentModeID", paymentMode);
            Cmd.Parameters.AddWithValue("@dpDate", dpType == "N" ? (object)DBNull.Value : dpDate);
            Cmd.ExecuteNonQuery();
        }

        private void insertSADetail(string SAID, int seqNo, string GroupID, string SubGroup1ID, string SubGroup2ID, string ItemID, string FullItemID, string ItemName, decimal Qty, decimal RemainingQty, string Unit, decimal Price, decimal Qty_Alt, string Unit_Alt, decimal Price_Alt, decimal ConvertionRatio, string DeliveryMethod, DateTime ExpectedDateFrom, DateTime ExpectedDateTo, decimal SubTotal, decimal SubTotal_PPN, decimal SubTotal_PPH, decimal LogisticAmount, string LogisticNotes, string DiscType, decimal DiscPercent, decimal DiscAmount, decimal BonusAmount, decimal CashBackAmount, string Notes, string LockStatus, string LockID, decimal LockQty, string SA_SQ_Id, int SA_SQ_SeqNo, string RefTransId, int RefTrans_SeqNo, string GelombangId, string BracketId, string Base, decimal Gelombang_Price, int GelombangSeqNo_Base, string BracketDesc, string PLJNo, int PLJSeqNo, decimal PLJPrice)
        {
            Query = "INSERT INTO [dbo].[SalesAgreement_Dtl] ([SalesAgreementNo],[SeqNo],[GroupID],[SubGroup1ID],[SubGroup2ID],[ItemID],[FullItemID],[ItemName],[DeliveryMethod],[LogisticAmount],[LogisticNotes],[ExpectedDateFrom],[ExpectedDateTo],[Qty],[Unit],[Qty_Alt],[Unit_Alt],[ConvertionRatio],[Price],[Price_Alt],[DiscType],[DiscPercent],[DiscAmount],[BonusAmount],[CashBackAmount],[SubTotal],[SubTotal_PPN],[SubTotal_PPH],[Notes],[LockStatus],[LockID],[LockQty],[RemainingQty],[SA_SQ_Id],[SA_SQ_SeqNo],[RefTransId],[RefTrans_SeqNo],[GelombangId],[BracketId],[Base],[Gelombang_Price],[GelombangSeqNo_Base],[BracketDesc],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy], PLJNo, PLJSeqNo, PLJPrice) VALUES ('" + SAID + "', '" + seqNo + "', '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', @DeliveryMethod, '" + LogisticAmount + "', @LogisticNotes, @ExpectedDateFrom, @ExpectedDateTo, '" + Qty + "', '" + Unit + "', '" + Qty_Alt + "', '" + Unit_Alt + "', '" + ConvertionRatio + "', '" + Price + "', '" + Price_Alt + "', '" + DiscType + "', '" + DiscPercent + "', '" + DiscAmount + "', '" + BonusAmount + "', '" + CashBackAmount + "', '" + SubTotal + "', '" + SubTotal_PPN + "', '" + SubTotal_PPH + "', @Notes, @LockStatus, @LockID, '" + LockQty + "', '" + RemainingQty + "', @SA_SQ_Id, @SA_SQ_SeqNo, @RefTransId, @RefTrans_SeqNo, @GelombangId, @BracketId, @Base, '" + Gelombang_Price + "', @GelombangSeqNo_Base, @BracketDesc, getdate(), '" + ControlMgr.UserId + "', '1753-01-01' , NULL, @PLJNo, @PLJSeqNo, @PLJPrice)";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DeliveryMethod", DeliveryMethod == String.Empty ? (object)DBNull.Value : DeliveryMethod);
            Cmd.Parameters.AddWithValue("@LogisticNotes", LogisticNotes);
            Cmd.Parameters.AddWithValue("@ExpectedDateFrom", Base == "N" ? (object)DBNull.Value : ExpectedDateFrom);
            Cmd.Parameters.AddWithValue("@ExpectedDateTo", Base == "N" ? (object)DBNull.Value : ExpectedDateTo);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.Parameters.AddWithValue("@LockStatus", LockStatus == "Select" ? (object)DBNull.Value : LockStatus);
            Cmd.Parameters.AddWithValue("@LockID", LockID == String.Empty ? (object)DBNull.Value : LockStatus);
            Cmd.Parameters.AddWithValue("@SA_SQ_Id", SA_SQ_Id == String.Empty ? (object)DBNull.Value : SA_SQ_Id);
            Cmd.Parameters.AddWithValue("@SA_SQ_SeqNo", SA_SQ_SeqNo == 0 ? (object)DBNull.Value : SA_SQ_SeqNo);
            Cmd.Parameters.AddWithValue("@RefTransId", RefTransId == String.Empty ? (object)DBNull.Value : RefTransId);
            Cmd.Parameters.AddWithValue("@RefTrans_SeqNo", RefTrans_SeqNo == 0 ? (object)DBNull.Value : RefTrans_SeqNo);
            Cmd.Parameters.AddWithValue("@GelombangId", GelombangId == String.Empty ? (object)DBNull.Value : GelombangId);
            Cmd.Parameters.AddWithValue("@BracketId", BracketId == String.Empty ? (object)DBNull.Value : BracketId);
            Cmd.Parameters.AddWithValue("@Base", Base == String.Empty ? (object)DBNull.Value : Base);
            Cmd.Parameters.AddWithValue("@GelombangSeqNo_Base", GelombangSeqNo_Base == 0 ? (object)DBNull.Value : GelombangSeqNo_Base);
            Cmd.Parameters.AddWithValue("@BracketDesc", BracketDesc);
            Cmd.Parameters.AddWithValue("@PLJNo", PLJNo == String.Empty ? (object)DBNull.Value : PLJNo);
            Cmd.Parameters.AddWithValue("@PLJSeqNo", PLJSeqNo == 0 ? (object)DBNull.Value : PLJSeqNo);
            Cmd.Parameters.AddWithValue("@PLJPrice", PLJNo == String.Empty ? (object)DBNull.Value : PLJPrice);
            Cmd.ExecuteNonQuery();
        }

        private void insertSAHeader(string SAID, string refId, string salesType, string mouID, string SQID, string custID, string custName, string reff, string currency, decimal xRate, DateTime SADate, string ToP, string paymentMode, string dpType, decimal dpAmount, decimal dpPercent, DateTime dpDate, DateTime saDueDate, decimal sTotal, decimal gDisc, decimal gPPN, decimal gPPh, decimal gTotal, decimal gBonus, decimal gCashBack, decimal ppn, decimal pph, string notes, string Referensi, string TransStatus)
        {
            Query = "INSERT INTO [dbo].[SalesAgreementH] ([SalesAgreementNo],[OrderDate],[TransType],[SalesQuotationNo],[CustID],[CustName],[CurrencyID],[ExchRate],[Total],[Total_Disk],[PPN],[Total_PPN],[PPH],[Total_PPH],[Total_Nett],[Total_Bonus],[Total_Cashback],[TermofPayment],[PaymentModeID],[DPType],[DPPercent],[DPAmount],[DPDueDate],[Notes],[TransStatus],[ValidTo],[SalesMouNo],[SA_SQ_Id],[RefTransId],[Referensi],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES (@SalesAgreementNo, @OrderDate, @TransType, @SQID, @CustID, @CustName, @CurrencyID, @ExchRate, @Total, @Total_Disk, @PPN, @Total_PPN, @PPH, @Total_PPH, @Total_Nett, @Total_Bonus, @Total_Cashback, @TermofPayment, @PaymentModeID, @DPType, @DPPercent, @DPAmount, @dpDate, @Notes";

            //if (tbxRefID.Text == String.Empty)
            //{
            //    flag = checkPriceTolerance();
            //    if (flag == 'X')
            //        Query += ", '11'";
            //    else
            //        Query += ", '01'";
            //}
            //else
            //    Query += ", '09'";
            Query += ", '" + TransStatus + "'";
            Query += ", @ValidTo, @mouID, NULL, @RefTransId, '" + reff + "', getdate(), '" + ControlMgr.UserId + "', '1753-01-01', NULL)";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@SalesAgreementNo", SAID);
            Cmd.Parameters.AddWithValue("@OrderDate", SADate);
            Cmd.Parameters.AddWithValue("@TransType", salesType);
            Cmd.Parameters.AddWithValue("@SQID", SQID == String.Empty ? (object)DBNull.Value : SQID);
            Cmd.Parameters.AddWithValue("@CustID", custID);
            Cmd.Parameters.AddWithValue("@CustName", custName);
            Cmd.Parameters.AddWithValue("@CurrencyID", currency);
            Cmd.Parameters.AddWithValue("@ExchRate", xRate);
            Cmd.Parameters.AddWithValue("@Total", sTotal);
            Cmd.Parameters.AddWithValue("@Total_Disk", gDisc);
            Cmd.Parameters.AddWithValue("@PPN", ppn);
            Cmd.Parameters.AddWithValue("@Total_PPN", gPPN);
            Cmd.Parameters.AddWithValue("@PPH", pph);
            Cmd.Parameters.AddWithValue("@Total_PPH", gPPh);
            Cmd.Parameters.AddWithValue("@Total_Nett", gTotal);
            Cmd.Parameters.AddWithValue("@Total_Bonus", gBonus);
            Cmd.Parameters.AddWithValue("@Total_Cashback", gCashBack);
            Cmd.Parameters.AddWithValue("@TermofPayment", ToP);
            Cmd.Parameters.AddWithValue("@PaymentModeID", paymentMode);
            Cmd.Parameters.AddWithValue("@DPType", dpType);
            Cmd.Parameters.AddWithValue("@DPPercent", dpPercent);
            Cmd.Parameters.AddWithValue("@DPAmount", dpAmount);
            Cmd.Parameters.AddWithValue("@dpDate", dpType == "N" ? (object)DBNull.Value : dpDate);
            Cmd.Parameters.AddWithValue("@Notes", notes);
            Cmd.Parameters.AddWithValue("@ValidTo", saDueDate);
            Cmd.Parameters.AddWithValue("@mouID", mouID == String.Empty ? (object)DBNull.Value : mouID);
            Cmd.Parameters.AddWithValue("@RefTransId", refId == String.Empty ? (object)DBNull.Value : refId);
            Cmd.Parameters.AddWithValue("@Referensi", Referensi);
            Cmd.ExecuteNonQuery();
        }


        decimal price, qty, STotal, LogisticAmount, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, STotal_PPN, STotal_PPH, GTotal;
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (tableCols[e.ColumnIndex] == "Qty")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == null)
                    dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value = "0";

                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value != null && Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value) != 0)
                    dataGridView1.Rows[e.RowIndex].Cells["Qty_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) / Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value);
            }
            if (tableCols[e.ColumnIndex] == "Qty_Alt")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == null)
                    dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value = "0";

                if (cbSAType.Text == "QUANTITY")
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Math.Round(Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value));
                else
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value != null && Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value) != 0)
                    dataGridView1.Rows[e.RowIndex].Cells["Qty"].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) * Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value);
            }
            if (tableCols[e.ColumnIndex] == "Price")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["Price"].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells["Price"].Value == null)
                    dataGridView1.Rows[e.RowIndex].Cells["Price"].Value = "0";
                if (dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == null)
                    dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value = "0";

                if (dataGridView1.Rows[e.RowIndex].Cells["Price"].Value != null && dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value != null && Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value) != 0)
                    dataGridView1.Rows[e.RowIndex].Cells["Price_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Price"].Value) / Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value);
            }
            populateGVData();
        }

        private void populateGVData()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                STotal = 0; STotal_PPN = 0; STotal_PPH = 0; DiscAmount = 0; DiscPercent = 0;
                //PRICE
                if (dataGridView1.Rows[i].Cells["Price"].Value == "" || dataGridView1.Rows[i].Cells["Price"].Value == null)
                    price = 0;
                else
                    price = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value);

                //QTY
                if (dataGridView1.Rows[i].Cells["Qty"].Value == "" || dataGridView1.Rows[i].Cells["Qty"].Value == null)
                    qty = 0;
                else
                    qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);

                //WHEN SA TYPE AMOUNT 
                if (dataGridView1.Rows[i].Cells["Base"].Value == String.Empty || dataGridView1.Rows[i].Cells["Base"].Value == null)
                    dataGridView1.Rows[i].Cells["Base"].Value = "";
                if (!(dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y" && cbSAType.Text == "AMOUNT"))
                {
                    STotal = price * qty;
                    dataGridView1.Rows[i].Cells["SubTotal"].Value = STotal;
                }
                else
                    STotal = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value);

                if (dataGridView1.Rows[i].Cells["DiscAmount"].Value != null && dataGridView1.Rows[i].Cells["DiscType"].Value != null)
                {
                    if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Amount" || dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "1")
                    {
                        if (dataGridView1.Rows[i].Cells["DiscAmount"].Value == "" || dataGridView1.Rows[i].Cells["DiscAmount"].Value == null)
                            DiscAmount = 0;
                        else
                            DiscAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value);
                    }
                    else if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Percentage" || dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "2")
                    {
                        if (dataGridView1.Rows[i].Cells["DiscPercent"].Value == "" || dataGridView1.Rows[i].Cells["DiscPercent"].Value == null)
                            DiscPercent = 0;
                        else
                            DiscPercent = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscPercent"].Value);
                    }
                }

                if (Mode != "BeforeEdit" && tbxSQID.Text == String.Empty)
                {
                    if (dataGridView1.Rows[i].Cells["DiscType"].Value != null)
                    {
                        if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Amount" || dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "1")
                        {
                            dataGridView1.Rows[i].Cells["DiscAmount"].ReadOnly = false;
                            dataGridView1.Rows[i].Cells["DiscAmount"].Style.BackColor = Color.White;
                            dataGridView1.Rows[i].Cells["DiscPercent"].ReadOnly = true;
                            dataGridView1.Rows[i].Cells["DiscPercent"].Style.BackColor = Color.LightGray;
                            if (STotal != 0 && DiscAmount != 0)
                                DiscPercent = DiscAmount / STotal * 100;
                            dataGridView1.Rows[i].Cells["DiscPercent"].Value = DiscPercent;
                        }
                        else if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Percentage" || dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "2")
                        {
                            dataGridView1.Rows[i].Cells["DiscAmount"].ReadOnly = true;
                            dataGridView1.Rows[i].Cells["DiscAmount"].Style.BackColor = Color.LightGray;
                            dataGridView1.Rows[i].Cells["DiscPercent"].ReadOnly = false;
                            dataGridView1.Rows[i].Cells["DiscPercent"].Style.BackColor = Color.White;
                            if (DiscPercent != 0)
                                DiscAmount = STotal * DiscPercent / 100;
                            dataGridView1.Rows[i].Cells["DiscAmount"].Value = DiscAmount;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells["DiscPercent"].Value = 0;
                            dataGridView1.Rows[i].Cells["DiscAmount"].Value = 0;
                            dataGridView1.Rows[i].Cells["DiscAmount"].ReadOnly = true;
                            dataGridView1.Rows[i].Cells["DiscPercent"].ReadOnly = true;
                            dataGridView1.Rows[i].Cells["DiscPercent"].Style.BackColor = Color.LightGray;
                            dataGridView1.Rows[i].Cells["DiscAmount"].Style.BackColor = Color.LightGray;
                        }
                    }
                }

                STotal_PPN = (STotal - DiscAmount) * Convert.ToDecimal(cbPPN.Text) / 100;
                dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = STotal_PPN;

                STotal_PPH = (STotal - DiscAmount) * Convert.ToDecimal(cbPPh.Text) / 100;
                dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = STotal_PPH;
            }

            tbxSTotal.Text = "0"; tbxGPPN.Text = "0"; tbxGPPh.Text = "0"; tbxGBonus.Text = "0"; tbxGCashBack.Text = "0"; tbxGDisc.Text = "0"; tbxGLog.Text = "0";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                tbxSTotal.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value) + Convert.ToDecimal(tbxSTotal.Text)).ToString()));
                tbxGPPN.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value) + Convert.ToDecimal(tbxGPPN.Text)).ToString()));
                tbxGPPh.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value) + Convert.ToDecimal(tbxGPPh.Text)).ToString()));
                tbxGDisc.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value) + Convert.ToDecimal(tbxGDisc.Text)).ToString()));
                tbxGLog.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value) + Convert.ToDecimal(tbxGLog.Text)).ToString()));
                tbxGBonus.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["BonusAmount"].Value) + Convert.ToDecimal(tbxGBonus.Text)).ToString()));
                tbxGCashBack.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["CashBackAmount"].Value) + Convert.ToDecimal(tbxGCashBack.Text)).ToString()));
            }

            tbxGTotal.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(tbxSTotal.Text) + Convert.ToDecimal(tbxGPPN.Text) + Convert.ToDecimal(tbxGPPh.Text) - Convert.ToDecimal(tbxGDisc.Text)).ToString()));
        }

        private void cbPPN_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = Convert.ToString(((Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value)) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPN.Text) / 100);
            }
            //for (int i = 0; i < dataGridView1.RowCount; i++)
            //{
            if (cbPPN.Text != "0.00")
                tbxGPPN.Text = string.Format("{0:#,0.00}", double.Parse(((Convert.ToDecimal(tbxSTotal.Text) - Convert.ToDecimal(tbxGDisc.Text)) * Convert.ToDecimal(cbPPN.Text) / 100/*Convert.ToDecimal(tbxGPPN.Text) + Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value)*/).ToString()));
            else
            {
                tbxGPPN.Text = "0.00";
                //break;
            }
            //}

        }

        private void cbPPh_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = Convert.ToString(((Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value)) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPh.Text) / 100);
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (cbPPh.Text != "0.00")
                    tbxGPPh.Text = string.Format("{0:#,0.00}", double.Parse(((Convert.ToDecimal(tbxSTotal.Text) - Convert.ToDecimal(tbxGDisc.Text)) * Convert.ToDecimal(cbPPh.Text) / 100/*Convert.ToDecimal(tbxGPPh.Text) + Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPh"].Value)*/).ToString()));
                else
                {
                    tbxGPPh.Text = "0.00";
                    break;
                }
            }
        }

        //private void tbxDPPercent_TextChanged(object sender, EventArgs e)
        //{
        //    tbxDPAmount.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDPPercent.Text) / 100).ToString()));
        //}

        //private void tbxDPAmount_TextChanged(object sender, EventArgs e)
        //{
        //    tbxDPPercent.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(tbxDPAmount.Text) / Convert.ToDecimal(tbxGTotal.Text)).ToString()));
        //}

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
            GetDataHeader();
            dtDP.MinDate = new DateTime(dtSA.Value.Year, dtSA.Value.Month, dtSA.Value.Day, 0, 0, 0);
            gv1Format();
            //steven edit s
            vOldReferensi = txtReferensi.Text;
            //steven edit e
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (tbxRefID.Text != String.Empty && tbxSAID.Text == String.Empty)
            {
                tbxSAID.Text = tbxRefID.Text;
                tbxRefID.Text = "";
            }
            ModeBeforeEdit();
            GetDataHeader();
            gv1Format();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2)
            {
                btnAdd.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                if (Mode == "New" || Mode == "Edit")
                {
                    if (tbxSQID.Text == String.Empty)
                    {
                        btnAdd.Enabled = true;
                        btnDelete.Enabled = true;
                    }
                }
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (ConnectionString.CheckPermissionAccess("SAHeader", "Approve") == 0)
            {
                MetroFramework.MetroMessageBox.Show(this, "You don't have Permission..", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "update SalesAgreementH set TransStatus = '03', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + tbxSAID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    Conn.Close();

                    ListMethod.insertLogTableFollowParentAct("SalesAgreement_LogTable", tbxSAID.Text, "", "SA", "03");

                    scope.Complete();
                }
                ModeBeforeEdit();
                Parent.RefreshGrid();
                MetroFramework.MetroMessageBox.Show(this, tbxSAID.Text + " approved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAmend_Click(object sender, EventArgs e)
        {
            try
            {
                flag = '\0';
                Conn = ConnectionString.GetConnection();
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    Query = "select Lock_Qty from InventLockTable where RefTransId = '" + dataGridView1.Rows[i].Cells["SalesAgreementNo"].Value + "' and RefTrans_SeqNo = '" + dataGridView1.Rows[i].Cells["SeqNo"].Value + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            if (Convert.ToDecimal(Dr["Lock_Qty"]) != 0)
                                flag = 'X';
                        }
                    }
                    Dr.Close();
                    if (flag == 'X')
                        break;
                }
                Conn.Close();
                if (flag == 'X')
                    MetroFramework.MetroMessageBox.Show(this, "Reserved item will be cleared when amend!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                ModeEdit();
                GetDataHeader();
                tbxRefID.Text = tbxSAID.Text;
                tbxSAID.Text = "";
                gv1Format();
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Cells["RefTransId"].Value = dataGridView1.Rows[i].Cells["SalesAgreementNo"].Value;
                    dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value = dataGridView1.Rows[i].Cells["SeqNo"].Value;
                    dataGridView1.Rows[i].Cells["SalesAgreementNo"].Value = "";
                    dataGridView1.Rows[i].Cells["SeqNo"].Value = "";
                }
                Mode = "New";
                btnAmend.Visible = false;
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbSAType_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Mode != "BeforeEdit")
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();
                tbxSQID.Text = "";
                tbxMOUID.Text = "";
                tbxCustID.Text = "";
                tbxCustName.Text = "";
                btnSMoU.Enabled = true;
                btnCust.Enabled = true;
            }
        }

        private void tbxDPAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void tbxDPPercent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void tbxXRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void tbxToP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.')
            {
                e.Handled = true;
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (ConnectionString.CheckPermissionAccess("SAHeader", "Approve") == 0)
            {
                MetroFramework.MetroMessageBox.Show(this, "You don't have Permission..", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "update SalesAgreementH set TransStatus = '10', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + tbxSAID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    Conn.Close();

                    ListMethod.insertLogTableFollowParentAct("SalesAgreement_LogTable", tbxSAID.Text, "", "SA", "10");
                    ListMethod.StatusLogCustomer("SAHeader", "SA", tbxCustID.Text, "10", "", tbxSAID.Text, "", "", "");

                    scope.Complete();
                }
                ModeBeforeEdit();
                Parent.RefreshGrid();
                MetroFramework.MetroMessageBox.Show(this, tbxSAID.Text + " rejected!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSMoU_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
                MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change MoU!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                tbxMOUID.Text = "";
                SearchV2 f = new SearchV2();
                f.SetMode("No");
                f.SetSchemaTable("dbo", "CustMouH", "and ValidTo >= DATEADD(day,-1,GETDATE())", "a.*", "CustMouH a");
                f.ShowDialog();

                if (SearchV2.data.Count != 0)
                {
                    tbxMOUID.Text = SearchV2.data[0];
                    tbxCustID.Text = "";
                    tbxCustName.Text = "";

                    SearchV2 f2 = new SearchV2();
                    f2.SetMode("No");
                    f2.SetSchemaTable("dbo", "CustTable", "and CustId in (select b.CustID from CustMouH a left join CustMou_Dtl b on a.MouNo = b.MouNo where a.MouNo = '" + tbxMOUID.Text + "')", "a.*", "CustTable a");
                    f2.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        tbxCustID.Text = SearchV2.data[0];
                        tbxCustName.Text = SearchV2.data[1];
                        Conn = ConnectionString.GetConnection();
                        Query = "select DP_Required from CustTable where CustId = '" + tbxCustID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        cbDPType.Text = Cmd.ExecuteScalar().ToString();
                        Conn.Close();
                        if (cbDPType.Text == "Y")
                            cbDPType.Enabled = false;
                        else
                            cbDPType.Enabled = true;
                        setDefaultValue();
                    }
                }
            }
        }

        private void cbCurrency_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Mode != "BeforeEdit")
            {
                if (cbCurrency.Text == "IDR")
                {
                    tbxXRate.Text = "1.00";
                    tbxXRate.Enabled = false;
                }
                else
                {
                    if (tbxSQID.Text == String.Empty)
                    {
                        SqlConnection Conn2 = ConnectionString.GetConnection();
                        Query = "select top 1 ExchRate from ExchRate where CurrencyId = '" + cbCurrency.Text + "' and CreatedDate between DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) + '00:00:00' and DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) + '23:59:59' order by CreatedDate desc";
                        Cmd = new SqlCommand(Query, Conn2);
                        SqlDataReader Dr2 = Cmd.ExecuteReader();
                        if (Dr2.HasRows)
                        {
                            while (Dr2.Read())
                            {
                                tbxXRate.Text = string.Format("{0:#,0.00}", double.Parse(Dr2[0].ToString()));
                            }
                        }
                        else
                        {
                            if (Mode != "BeforeEdit")
                            {
                                MetroFramework.MetroMessageBox.Show(this, "Today rate is not available! Please insert manually!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tbxXRate.Text = "1.00";
                            }
                        }
                        Dr2.Close();
                        Conn2.Close();
                        tbxXRate.Enabled = true;
                    }
                }
            }
        }
       

        private void tbxDPAmount_Leave(object sender, EventArgs e)
        {
            if (tbxDPAmount.Text == String.Empty)
                tbxDPAmount.Text = "0.00";
            if (Convert.ToDecimal(tbxGTotal.Text) != 0)
                tbxDPPercent.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(Convert.ToDecimal(tbxDPAmount.Text) / Convert.ToDecimal(tbxGTotal.Text) * 100)).ToString()));
            tbxDPAmount.Text = string.Format("{0:#,0.00}", double.Parse(tbxDPAmount.Text));
        }

        private void tbxDPPercent_Leave(object sender, EventArgs e)
        {
            if (tbxDPPercent.Text == String.Empty)
                tbxDPPercent.Text = "0.00";
            decimal dpAmount = Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDPPercent.Text) / 100;
            tbxDPAmount.Text = string.Format("{0:#,0.00}", double.Parse(dpAmount.ToString()));
            tbxDPPercent.Text = string.Format("{0:#,0.00}", double.Parse(tbxDPPercent.Text));
        }

        private void btnConfirm_Click_1(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Sales Admin")
            {
                MetroFramework.MetroMessageBox.Show(this, "You don't have Permission..", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (scope = new TransactionScope())
            {
                Conn = ConnectionString.GetConnection();

                if (tbxMOUID.Text.Trim() != "")
                {
                    if (!(ListMethod.checkCustomerMoU(tbxMOUID.Text, tbxCustID.Text, Convert.ToDecimal(tbxGTotal.Text))))
                        return;
                }
                else
                {
                    if (!(ListMethod.checkCreditLimit("Stop", tbxCustID.Text, Convert.ToDecimal(tbxGTotal.Text))))
                        return;
                }

                Query = "update SalesAgreementH set TransStatus = '12', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + tbxSAID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                Conn.Close();

                ListMethod.insertLogTableFollowParentAct("SalesAgreement_LogTable", tbxSAID.Text, "", "SA", "12");
                ListMethod.StatusLogCustomer("SAHeader", "SA", tbxCustID.Text, "12", "", tbxSAID.Text, "", "", "");

                scope.Complete();
            }
            ModeBeforeEdit();
            Parent.RefreshGrid();
            MetroFramework.MetroMessageBox.Show(this, tbxSAID.Text + " confirmed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cbToP_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true; //hendry
        }

        private void cbSAType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                if (cbSAType.Text != oldSAType)
                {
                    DialogResult result = MetroFramework.MetroMessageBox.Show(this, "Item detail will be cleared.\r\nAre you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        oldSAType = cbSAType.Text;
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                        dataGridView2.Rows.Clear();
                        dataGridView2.Columns.Clear();
                    }
                    else
                        cbSAType.Text = oldSAType;
                }
            }
            else
                oldSAType = cbSAType.Text;
        }

        private void btnUpload_Click_1(object sender, EventArgs e)
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

        private void btnDownload_Click_1(object sender, EventArgs e)
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

        private void btnDelAttachment_Click_1(object sender, EventArgs e)
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

        public void GetDgvAttachmentData()
        {
            Conn = ConnectionString.GetConnection();
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

            Query = "Select * From [tblAttachments] Where ReffTableName = 'SalesAgreementH' And ReffTransId = '" + tbxSAID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                test.Add((byte[])Dr["Attachment"]);
            }

            dgvAttachment.AutoResizeColumns();
        }

        private void txtReferensi_TextChanged(object sender, EventArgs e)
        {
            if (txtReferensi.Text == "By Phone")
            {
                cbByPhone.Checked = true;
            }
            else
            {
                cbByPhone.Checked = false;
            }
            vNewReferensi = txtReferensi.Text;
        }

        private void cbByPhone_CheckedChanged(object sender, EventArgs e)
        {
            if (cbByPhone.Checked == true)
            {
                txtReferensi.Text = "By Phone";
                txtReferensi.Enabled = false;
                btnRef.Enabled = false;
            }
            else
            {
                txtReferensi.Text = "";
                txtReferensi.Enabled = true;
                btnRef.Enabled = true;
            }
        }

        public void SaveDgvAttachmentData()
        {
            //START STEVEN EDIT
            for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                Query += "( 'SalesAgreementH', '" + tbxSAID.Text + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString();
                Query += "',@binaryValue";
                Query += ");";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }
            //END STEVEN EDIT
        }

        private void btnReserved_Click(object sender, EventArgs e)
        {
            SAReserved f = new SAReserved();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                f.SAID.Add(dataGridView1.Rows[i].Cells["SalesAgreementNo"].Value.ToString());
                f.SA_SeqNo.Add(Convert.ToInt32(dataGridView1.Rows[i].Cells["SeqNo"].Value));
                f.SA_Qty.Add(Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value));
            }
            f.ShowDialog();
        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroTabControl1.SelectedTab == tabItemDetail || metroTabControl1.SelectedTab == tabBonus || metroTabControl1.SelectedTab == tabLogistic)
            {
                if (cbToP.Text == "Select" || cbPaymentMode.Text == "Select" || tbxCustID.Text == String.Empty)
                {
                    string oldTab = Regex.Replace(metroTabControl1.SelectedTab.Text, "[^a-zA-Z0-9]", "");
                    metroTabControl1.SelectedTab = tabSales;

                    if (validate == false)
                        label = new Label[20];
                    createLabel(cbToP, lblToP, tabSales, "string");
                    createLabel(cbPaymentMode, lblPaymentMode, tabSales, "string");
                    createLabel(tbxCustID, lblCust, gbMain, "string");
                    count = 0;
                    validate = true;

                    List<string> msg2 = new List<string>();

                    msg = "Please Select ";
                    if (cbToP.Text == "Select")
                        msg2.Add("Term of Payment");
                    if (cbPaymentMode.Text == "Select")
                        msg2.Add("Payment Mode");
                    if (tbxCustID.Text == String.Empty)
                        msg2.Add("Customer");
                    for (int i = 0; i < msg2.Count; i++)
                    {
                        if (i == msg2.Count - 1 && i != 0)
                            msg += " and ";
                        else if (i >= 1)
                            msg += ", ";
                        msg += msg2[i];
                    }
                    msg += " before proceed to " + oldTab + "!";
                    MetroFramework.MetroMessageBox.Show(this, msg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        string oldToP;
        private void cbToP_SelectedValueChanged(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (cbToP.Text != oldToP)
                    MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change Term of Payment!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbToP.Text = oldToP;
            }
            else
                oldToP = cbToP.Text;
        }

        string oldPaymentMode;
        private void cbPaymentMode_SelectedValueChanged(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (cbPaymentMode.Text != oldPaymentMode)
                    MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change Payment Mode!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbPaymentMode.Text = oldPaymentMode;
            }
            else
                oldPaymentMode = cbPaymentMode.Text;
        }
        //tia edit
        //klik kanan
        PopUp.CustomerID.Customer Cust = null;
        PopUp.FullItemId.FullItemId FID = null;
        Sales.MoUCustomer.HeaderMoUCustomer MOUID = null;

        Sales.SalesOrder.SOHeader ParentToSO;

        TaskList.GlobalTasklist Parent2 = new TaskList.GlobalTasklist();

        public void SetParent2(TaskList.GlobalTasklist Tl)
        {
            Parent2 = Tl;
        }

        public void ParentRefreshGrid(Sales.SalesOrder.SOHeader so)
        {
            ParentToSO = so;
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }
        private void tbxCustID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    tbxCustID.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(tbxCustID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }
        private bool CheckOpened(string name)
        {
            // FormCollection FC = Application.OpenForms;
            FormCollection FC = Application.OpenForms;
            foreach (Form frm in FC)
            {
                if (frm.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void tbxCustName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    tbxCustName.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(tbxCustID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        private void tbxMOUID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MOUID == null || MOUID.Text == "")
                {
                    tbxMOUID.Enabled = true;
                    MOUID = new Sales.MoUCustomer.HeaderMoUCustomer();
                    MOUID.SetMode("PopUp", tbxMOUID.Text);
                    MOUID.ParentRefreshGrid(this);
                    MOUID.Show();
                }
                else if (CheckOpened(MOUID.Name))
                {
                    MOUID.WindowState = FormWindowState.Normal;
                    MOUID.Show();
                    MOUID.Focus();
                }
            }
        }

        //end

        ////Created by Thaddaeus, 13 Sept2018
        //private bool CreditLimit(SqlConnection Conn, bool update)
        //{
        //    decimal TotalNett = Convert.ToDecimal(tbxGTotal.Text);
        //    bool status = false;

        //    Query = "SELECT [Limit_Total], Sisa_Limit_Total,[Limit_Per_PO],Limit_Temp FROM [dbo].[CustTable] WHERE [CustId] = '" + tbxCustID.Text + "'";
        //    using (Cmd = new SqlCommand(Query, Conn))
        //    {
        //        Dr = Cmd.ExecuteReader();
        //        while (Dr.Read())
        //        {
        //            decimal Limit = Convert.ToDecimal(Dr["Limit_Temp"]) + Convert.ToDecimal(Dr["Limit_Total"]);
        //            if (Limit < (Convert.ToDecimal(Dr["Sisa_Limit_Total"]) + TotalNett))
        //            {
        //                status = true;
        //            }
        //            if (Convert.ToDecimal(Dr["Limit_Per_PO"]) < TotalNett)
        //            {
        //                status = true;
        //            }
        //        }
        //        Dr.Close();
        //    }
        //    if (status == false && update == true)
        //    {
        //        Query = "UPDATE [dbo].[CustTable] SET [Sisa_Limit_Total] += " + TotalNett + " WHERE [CustId] = '" + tbxCustID.Text + "'";
        //        using (Cmd = new SqlCommand(Query, Conn))
        //        {
        //            Cmd.ExecuteNonQuery();
        //        }
        //    }
        //    return status;
        //}
        ////end======================================================================================

        private void setDefaultValue()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT * FROM [CustTable] WHERE [CustId] = '" + tbxCustID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbDPType.SelectedItem = Dr["DP_Required"].ToString();
                cbPPN.SelectedItem = Dr["PPN"].ToString();
                cbPPh.SelectedItem = Dr["PPH"].ToString();
                cbToP.SelectedItem = Dr["TermOfPayment"].ToString();
                cbPaymentMode.SelectedItem = Dr["PaymentModeId"].ToString();
                cbCurrency.SelectedItem = Dr["CurrencyId"].ToString();
            }
        }


    }
}