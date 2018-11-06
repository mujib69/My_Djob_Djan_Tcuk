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
using System.Globalization;
using System.IO;
using System.Transactions;

//BY: HC
namespace ISBS_New.Sales.SalesOrder
{
    public partial class SOHeader : MetroFramework.Forms.MetroForm
    {
        #region Initialization
        /**********SQL*********/
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        private TransactionScope scope;
        /**********SQL*********/
        /**********Steven Edit start************/
        List<string> sSelectedFile, FileName, Extension;
        int Index;
        List<byte[]> test = new List<byte[]>();
        string vOldReferensi, vNewReferensi;
        /**********Steven Edit end ************/
        /**********STANDARD*********/
        private string Mode;
        private string SOID;
        /**********STANDARD*********/
        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "No", "SalesOrderNo", "SeqNo", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "Base", "Qty", "RemainingQty", "Unit", "Price", "Qty_Alt", "Unit_Alt", "Price_Alt", "ConvertionRatio", "DeliveryMethod", "ExpectedDateFrom", "ExpectedDateTo", "SubTotal", "SubTotal_PPN", "SubTotal_PPH", "LogisticAmount", "LogisticNotes", "DiscType", "DiscPercent", "DiscAmount", "BonusAmount", "CashBackAmount", "Notes", "SA_SQ_Id", "SA_SQ_SeqNo", "RefTransId", "RefTrans_SeqNo", "PLJNo", "PLJSeqNo", "PLJPrice" };
        /*********datagridview cols name*********/

        //NUMBER BASED ON SALESPRICELIST DAYS
        int[] priceListDays = new int[] { 0, 2, 3, 7, 14, 21, 30, 40, 45, 60, 75, 90, 120, 150, 180 };

        //GV POP UP 
        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        DataGridViewComboBoxCell cell; //CELLVALUE
        private SqlDataReader Dr2; //CELLVALUE

        //SOInq Parent = new SOInq();
        //public void SetParent(SOInq F) { Parent = F; }
        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F) { Parent = F; }

        //private char flag; private string msg; //Validation

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        bool check; //label
        /*private char flag;*/
        private string msg; //Validation
        /*********VALIDATION*********/

        //DateTimePicker _dateTimePicker = new DateTimePicker(); //DATE IN GRIDVIEW1 #OPTION 1
        DateTimePicker dtp; //DATE IN GRIDVIEW1 #OPTION 2
        ContextMenu vendid = new ContextMenu();

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public SOHeader()
        {
            InitializeComponent();
        }
        #endregion

        private void SOHeader_Load(object sender, EventArgs e)
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

            tabPage1.AutoScroll = true;
            tabPage2.AutoScroll = true;
            tabPage1.Text = "Detail SO";
            tabPage2.Text = "Detail Reference";

            cbCurrency.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select CurrencyID from [ISBS-NEW4].[dbo].[CurrencyTable] order by CurrencyID asc", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbCurrency.Items.Add(Dr[0]);
            }
            Dr.Close();

            cbPaymentMode.Items.Clear();
            cbPaymentMode.Items.Add("Select");
            Cmd = new SqlCommand("select [PaymentModeName] from [ISBS-NEW4].[dbo].[PaymentMode] order by PaymentModeID asc", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbPaymentMode.Items.Add(Dr[0]);
            }
            Dr.Close();

            cbToP.Items.Clear();
            cbToP.Items.Add("Select");
            Cmd = new SqlCommand("select [TermOfPayment] from TermOfPayment order by DueDate asc", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbToP.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();

            if (Mode == "New")
            {
                cbRef.SelectedIndex = 0;
                cbToP.SelectedIndex = 0;
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                GetDataHeader();
                ModeBeforeEdit();
            }
            else if (Mode == "Edit")
            {
                GetDataHeader();
                ModeEdit();
            }
            else if (Mode == "PopUp")//tia edit
            {
                GetDataHeader();
                ModePopUp();
            }

            //tia edit end
            metroTabControl1.SelectedTab = tabSales;
            /**********DATAGRIDVIEW DATETIME V2**********/
            //dtp = new DateTimePicker();
            //dtp.Format = DateTimePickerFormat.Custom;
            //dtp.CustomFormat = "dd-MM-yyyy";
            //dtp.Visible = false;
            //dtp.Width = 100;

            //dataGridView1.Controls.Add(dtp);
            //dtp.ValueChanged += this.dtp_ValueChanged;
            //dataGridView1.CellBeginEdit += this.dataGridView1_CellBeginEdit;
            //dataGridView1.CellEndEdit += this.dataGridView1_CellEndEdit;
            /**********DATAGRIDVIEW DATETIME**********/
        }

        public void SetMode(string tmpMode, string tmpNumber)
        {
            Mode = tmpMode;
            tbxSOID.Text = tmpNumber;
            SOID = tmpNumber;
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

        private void GetDataHeader()
        {
            //Steven edit
            test.Clear();
            //steven edit end
            Conn = ConnectionString.GetConnection();
            if (dataGridView1.RowCount - 1 <= 0)
            {
                dataGridView1.ColumnCount = tableCols.Length;
                for (int i = 0; i < tableCols.Length; i++)
                {
                    dataGridView1.Columns[i].Name = tableCols[i];
                }
            }
            if (Mode == "New")
            {
                GetDataHeaderNew();
            }
            else
            {
                GetDataHeaderEdit();
            }
            //tia edit
            HidePrice();
            //tia edit end
        }

        private void GetDataHeaderNew()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            if (SearchV2.data.Count >= 1)
                tbxSQID.Text = SearchV2.data[0];

            if (cbRef.Text == "Sales Agreement")
            {
                GetAttachmentSA();
            }
            else
                GetDgvAttachmentData();

            Conn = ConnectionString.GetConnection();

            if (cbRef.Text == "Sales Agreement")
            {
                Query = "select * from [SalesAgreementH] where SalesAgreementNo = '" + tbxSQID.Text + "'";
            }
            else if (cbRef.Text == "Sales Quotation")
            {
                Query = "select * from [SalesQuotationH] where SalesQuotationNo = '" + tbxSQID.Text + "'";
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                //TOP
                tbxMoUID.Text = Dr["SalesMouNo"].ToString();
                cbDPType.Text = Dr["DPType"].ToString();
                DateTime date1;
                if (Dr["DPDueDate"].ToString() == String.Empty)
                    date1 = Convert.ToDateTime("01/01/1753");
                else
                    date1 = new DateTime(Convert.ToDateTime(Dr["DPDueDate"]).Year, Convert.ToDateTime(Dr["DPDueDate"]).Month, Convert.ToDateTime(Dr["DPDueDate"]).Day, 00, 00, 00);
                dtDP.MinDate = date1;

                if (cbDPType.Text == "Y")
                {
                    dtDP.Text = Dr["DPDueDate"].ToString();
                    tbxDPPercent.Text = string.Format("{0:#,0.00}", double.Parse(Dr["DPPercent"].ToString()));
                    tbxDP.Text = string.Format("{0:#,0.00}", double.Parse(Dr["DPAmount"].ToString()));
                    llDPDueDate.Visible = true;
                    dtDP.Visible = true;
                }
                else if (cbDPType.Text == "N")
                {
                    llDPDueDate.Visible = false;
                    dtDP.Visible = false;
                }
                tbxCustID.Text = Dr["CustID"].ToString();
                tbxCustName.Text = Dr["CustName"].ToString();
                cbCurrency.Text = Dr["CurrencyID"].ToString();
                cbToP.Text = Dr["TermofPayment"].ToString();
                tbxXRate.Text = string.Format("{0:#,0.00}", double.Parse(Dr["ExchRate"].ToString()));

                //BOTTOM
                cbPPN.Text = Dr["PPN"].ToString();
                cbPPh.Text = Dr["PPH"].ToString();
                Cmd = new SqlCommand("Select [PaymentModeName] from [ISBS-NEW4].[dbo].[PaymentMode] where [PaymentModeID] = @PaymentModeID", Conn);
                Cmd.Parameters.AddWithValue("@PaymentModeID", Dr["PaymentModeID"]);
                cbPaymentMode.Text = Cmd.ExecuteScalar().ToString();
                tbxNotes.Text = Dr["Notes"].ToString();
                tbxSTotal.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total"].ToString()));
                tbxGPPN.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_PPN"].ToString()));
                tbxGPPh.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_PPH"].ToString()));
                tbxGTotal.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_Nett"].ToString()));
                tbxGBonus.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_Bonus"].ToString()));
                tbxGCashback.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_Cashback"].ToString()));
                //tbxDP.Text = Dr["DPAmount"].ToString();
                if (cbRef.Text == "Sales Quotation")
                    tbxGLog.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_LogisticAmount"].ToString()));
                //else if(cbRef.Text == "Sales Agreement")
                //    tbxGLog.Text = Dr["LogisticAmount"].ToString();
                tbxGDisc.Text = Dr["Total_Disk"].ToString();
                if (cbRef.Text == "Sales Agreement")
                    txtReferensi.Text = Dr["Referensi"].ToString();
            }
            Dr.Close();

            dataGridView1.ColumnCount = Convert.ToInt32(tableCols.Length);
            for (int i = 0; i < Convert.ToInt32(tableCols.Length); i++)
                dataGridView1.Columns[i].Name = tableCols[i];
            if (cbRef.Text == "Sales Quotation")
            {
                Query = "select * from [SalesQuotationD] where SalesQuotationNo = '" + tbxSQID.Text + "' and Deleted = 'N' order by SeqNo asc";
                Cmd = new SqlCommand(Query, Conn);
            }
            else if (cbRef.Text == "Sales Agreement")
            {
                //Cmd = new SqlCommand("select * from [SalesAgreement_Dtl] where SalesAgreementNo = '" + tbxSQID.Text + "' and Base != 'Y' and Deleted = 'N' order by SeqNo asc", Conn);
                Cmd = new SqlCommand("select * from [SalesAgreement_Dtl] where SalesAgreementNo = '" + tbxSQID.Text + "' and Deleted = 'N' order by SeqNo asc", Conn);
            }
            Dr = Cmd.ExecuteReader();
            int x = 0;
            while (Dr.Read())
            {
                #region pass value to dataGridView1fo
                //dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", "", Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemID"], Dr["ItemName"], Dr["Base"], "", "", Dr["Unit"], 0, Dr["Qty_Alt"], Dr["Unit_Alt"], 0, Dr["ConvertionRatio"], Dr["DeliveryMethod"], "", "", Dr["SubTotal"], Dr["SubTotal_PPN"], Dr["SubTotal_PPH"], Dr["LogisticAmount"], Dr["LogisticNotes"], Dr["DiscType"], Dr["DiscPercent"],Dr["DiscAmount"],Dr["BonusAmount"], Dr["CashBackAmount"], Dr["Notes"], Dr["BonusAmount"], Dr["CashBackAmount"], Dr["Notes"], "", Dr["SeqNo"], "", "", Dr["PLJNo"], Dr["PLJSeqNo"], Dr["PLJPrice"]);
                dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", "", Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemID"], Dr["ItemName"], Dr["Base"], "", "", Dr["Unit"], 0, Dr["Qty_Alt"], Dr["Unit_Alt"], 0, Dr["ConvertionRatio"], Dr["DeliveryMethod"], "", "", Dr["SubTotal"], Dr["SubTotal_PPN"], Dr["SubTotal_PPH"], Dr["LogisticAmount"], Dr["LogisticNotes"], Dr["DiscType"], Dr["DiscPercent"], Dr["DiscAmount"], Dr["BonusAmount"], Dr["CashBackAmount"], Dr["Notes"], Dr["BonusAmount"], Dr["CashBackAmount"], Dr["Notes"], "", Dr["PLJNo"], Dr["PLJSeqNo"], Dr["PLJPrice"], "", "", "");

                Cmd = new SqlCommand("select [Deskripsi] from [DiskonScheme] where [DiskonSchemeID] = '" + Dr["DiscType"] + "'", Conn);
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["DiscType"].Value = Cmd.ExecuteScalar().ToString();

                if (cbRef.Text == "Sales Quotation")
                {
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Qty"].Value = Dr["Qty"];
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SA_SQ_Id"].Value = Dr["SalesQuotationNo"];
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SA_SQ_SeqNo"].Value = Dr["SeqNo"];
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price"].Value = Dr["Price"];
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price_Alt"].Value = Dr["Price_Alt"];
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ExpectedDateTo"].Value = Convert.ToDateTime(Dr["ExpectedDateTo"]);
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ExpectedDateFrom"].Value = Convert.ToDateTime(Dr["ExpectedDateFrom"]);
                }
                else if (cbRef.Text == "Sales Agreement")
                {
                    if (Dr["Base"].ToString() == "Y")
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Qty"].Value = "0";
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SA_SQ_Id"].Value = Dr["SalesAgreementNo"];
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SA_SQ_SeqNo"].Value = Dr["SeqNo"];
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["DeliveryMethod"].Value = Dr["DeliveryMethod"];
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ExpectedDateFrom"].Value = Convert.ToDateTime(Dr["ExpectedDateFrom"]);
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ExpectedDateTo"].Value = Convert.ToDateTime(Dr["ExpectedDateTo"]);
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["LogisticNotes"].Value = Dr["LogisticNotes"];
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Notes"].Value = Dr["Notes"];
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price"].Value = Convert.ToDecimal(Dr["Price"]);
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price_Alt"].Value = Convert.ToDecimal(Dr["Price"]) / Convert.ToDecimal(Dr["ConvertionRatio"]);
                    }
                    else if (Dr["Base"].ToString() == "N")
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Qty"].Value = Dr["RemainingQty"];
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SA_SQ_Id"].Value = Dr["SalesAgreementNo"];

                        //GET GELOMBANG PRICE
                        Query = "select Price, DeliveryMethod, ExpectedDateFrom, ExpectedDateTo, LogisticNotes, Notes from SalesAgreement_Dtl where SalesAgreementNo = '" + Dr["SalesAgreementNo"] + "' and GelombangId = '" + Dr["GelombangId"] + "' and BracketId = '" + Dr["BracketId"] + "' and Base = 'Y'";
                        Cmd = new SqlCommand(Query, Conn);
                        SqlDataReader Dr2 = Cmd.ExecuteReader();
                        decimal basePrice = 0;
                        while (Dr2.Read())
                        {
                            basePrice = Convert.ToDecimal(Dr2["Price"]);
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["DeliveryMethod"].Value = Dr2["DeliveryMethod"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ExpectedDateFrom"].Value = Convert.ToDateTime(Dr2["ExpectedDateFrom"]);
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ExpectedDateTo"].Value = Convert.ToDateTime(Dr2["ExpectedDateTo"]);
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["LogisticNotes"].Value = Dr2["LogisticNotes"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Notes"].Value = Dr2["Notes"];
                        }
                        Dr2.Close();

                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price"].Value = basePrice + Convert.ToDecimal(Dr["Price"]);
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price_Alt"].Value = (basePrice + Convert.ToDecimal(Dr["Price"])) / Convert.ToDecimal(Dr["ConvertionRatio"]);
                    }
                }
                #endregion
            }
            Dr.Close();
            Conn.Close();

            if (cbRef.Text == "Sales Agreement")
                Query = "select DPAmount, DPType from [SalesAgreementH] where SalesAgreementNo = '" + tbxSQID.Text + "'";
            else if (cbRef.Text == "Sales Quotation")
                Query = "select DPAmount, DPType from [SalesQuotationH] where SalesQuotationNo = '" + tbxSQID.Text + "'";
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["DPType"].ToString() == "Y")
                    tbxDP.Text = string.Format("{0:#,0.00}", double.Parse(Dr["DPAmount"].ToString()));
            }
            Dr.Close();
            Conn.Close();

            //GV2
            dataGridView2.ColumnCount = Convert.ToInt32(tableCols.Length - 3);
            for (int i = 0; i < Convert.ToInt32(tableCols.Length - 3); i++)
                dataGridView2.Columns[i].Name = tableCols[i];

            Conn = ConnectionString.GetConnection();
            if (cbRef.Text == "Sales Agreement")
            {
                Query = "select TransType from SalesAgreementH where salesAgreementNo = '" + tbxSQID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                if (Cmd.ExecuteScalar().ToString() == "AMOUNT")
                    dataGridView2.Columns["Qty"].HeaderText = "Remaining Amount";
                else
                    dataGridView2.Columns["Qty"].HeaderText = "Remaining Qty";
            }

            if (cbRef.Text == "Sales Quotation")
                Query = "select * from [ISBS-NEW4].[dbo].[SalesQuotationD] where SalesQuotationNo = '" + tbxSQID.Text + "' and Deleted = 'N' order by SeqNo asc";
            else if (cbRef.Text == "Sales Agreement")
                Query = "select * from [SalesAgreement_Dtl] where SalesAgreementNo = '" + tbxSQID.Text + "' and Deleted = 'N' order by SeqNo asc";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            x = 0;
            while (Dr.Read())
            {
                //string[] tableCols = new string[] { "No", "SalesOrderNo", "SeqNo", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "Base", "Qty", "RemainingQty", "Unit", "Price", "Qty_Alt", "Unit_Alt", "Price_Alt", "ConvertionRatio", "DeliveryMethod", "ExpectedDateFrom", "ExpectedDateTo", "SubTotal", "SubTotal_PPN", "SubTotal_PPH", "LogisticAmount", "LogisticNotes", "DiscType", "DiscPercent", "DiscAmount", "BonusAmount", "CashBackAmount", "Notes", "SA_SQ_Id", "SA_SQ_SeqNo", "RefTransId", "RefTrans_SeqNo", "PLJNo", "PLJSeqNo", "PLJPrice" };
                #region pass value to dataGridView2
                dataGridView2.Rows.Add(1);
                for (int i = 0; i < Convert.ToInt32(tableCols.Length - 3); i++)
                {
                    if (!(tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo" || tableCols[i] == "SalesOrderNo"))
                    {
                        if (i == 0)
                            dataGridView2.Rows[x].Cells[tableCols[i]].Value = dataGridView2.Rows.Count;
                        else if (tableCols[i] == "DiscType")
                        {
                            Cmd = new SqlCommand("select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr[tableCols[i]].ToString() + "'", Conn);
                            dataGridView2.Rows[x].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                        }
                        else if (tableCols[i] == "SA_SQ_Id")
                        {
                            if (cbRef.Text == "Sales Quotataion")
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["SalesQuotationNo"];
                            else if (cbRef.Text == "Sales Agreement")
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["SalesAgreementNo"];
                        }
                        else if (tableCols[i] == "SA_SQ_SeqNo")
                            dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["SeqNo"];
                        else if (tableCols[i] == "RemainingQty")
                        {
                            if (cbRef.Text == "Sales Quotataion")
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["Qty"];
                            else if (cbRef.Text == "Sales Agreement")
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["RemainingQty"];
                        }
                        else if (tableCols[i] == "Qty")
                        {
                            if (cbRef.Text == "Sales Quotataion")
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["Qty"];
                            else if (cbRef.Text == "Sales Agreement")
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["RemainingQty"];
                        }
                        else
                            dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                    }
                }
                x++;
                #endregion
            }
            Dr.Close();
            Conn.Close();

            ModeNew();
        }

        private void GetDataHeaderEdit()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            //STEVEN EDIT START
            //test.Clear();

            //if (SOID != String.Empty)
            //    tbxSOID.Text = SOID;
            Query = "Select * from [ISBS-NEW4].[dbo].[SalesOrderH] where SalesOrderNo = '" + tbxSOID.Text + "'; ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                #region pass header value to form
                if (Dr["SA_SQ_Id"].ToString().Split('/')[0] == "SA")
                    cbRef.Text = "Sales Agreement";
                else if (Dr["SA_SQ_Id"].ToString().Split('/')[0] == "SQ")
                    cbRef.Text = "Sales Quotation";
                dateTimePicker1.Value = Convert.ToDateTime(Dr["OrderDate"]);
                dtDP.MinDate = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 0, 0, 0);
                dateTimePicker3.Value = Convert.ToDateTime(Dr["ValidTo"]);
                tbxCustID.Text = Dr["CustID"].ToString();
                tbxCustName.Text = Dr["CustName"].ToString();
                tbxMoUID.Text = Dr["SalesMouNo"].ToString();
                tbxSQID.Text = Dr["SA_SQ_Id"].ToString();
                cbCurrency.Text = Dr["CurrencyID"].ToString();
                tbxXRate.Text = string.Format("{0:#,0.00}", double.Parse(Dr["ExchRate"].ToString()));
                tbxSTotal.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total"].ToString()));
                tbxGDisc.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_Disk"].ToString()));
                cbPPN.Text = Dr["PPN"].ToString();
                tbxGPPN.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_PPN"].ToString()));
                cbPPh.Text = Dr["PPH"].ToString();
                tbxGPPh.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_PPH"].ToString()));
                tbxGTotal.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_Nett"].ToString()));
                tbxGBonus.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_Bonus"].ToString()));
                tbxGCashback.Text = string.Format("{0:#,0.00}", double.Parse(Dr["Total_Cashback"].ToString()));
                cbToP.Text = Dr["TermofPayment"].ToString();
                Cmd = new SqlCommand("Select [PaymentModeName] from [ISBS-NEW4].[dbo].[PaymentMode] where [PaymentModeID] = @PaymentModeID", Conn);
                Cmd.Parameters.AddWithValue("@PaymentModeID", Dr["PaymentModeID"]);
                cbPaymentMode.Text = Cmd.ExecuteScalar().ToString();
                cbDPType.Text = Dr["DPType"].ToString();
                tbxDPPercent.Text = string.Format("{0:#,0.00}", double.Parse(Dr["DPPercent"].ToString()));
                tbxDP.Text = string.Format("{0:#,0.00}", double.Parse(Dr["DPAmount"].ToString()));
                if (Dr["DPType"].ToString() == "Y")
                    dtDP.Value = Convert.ToDateTime(Dr["DPDueDate"]);
                tbxNotes.Text = Dr["Notes"].ToString();
                tbxRefID.Text = Dr["RefTransId"].ToString();
                //STEVEN EDIT S
                txtReferensi.Text = Dr["Referensi"].ToString();
                //STEVEN EDIT E
                #endregion
            }

            if (dataGridView1.RowCount - 1 <= 0)
            {
                dataGridView1.ColumnCount = tableCols.Length;
                for (int i = 0; i < tableCols.Length; i++)
                {
                    dataGridView1.Columns[i].Name = tableCols[i];
                }
            }

            Query = "select * from [ISBS-NEW4].[dbo].[SalesOrderD] where SalesOrderNo = '" + tbxSOID.Text + "' and Deleted = 'N' order by SeqNo asc; ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int no = 1;
            while (Dr.Read())
            {
                dataGridView1.Rows.Add(1);
                dataGridView1.Rows[no - 1].Cells[0].Value = no;
                for (int i = 1; i < tableCols.Length; i++)
                {
                    if (tableCols[i] == "DiscType")
                    {
                        if (Mode == "Edit")
                        {
                            cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                            Query = "Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr[tableCols[i]] + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            if (Dr[tableCols[i]] != null)
                                cell.Value = Cmd.ExecuteScalar().ToString();
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]] = cell;
                        }
                        else
                        {
                            Query = "Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr["DiscType"] + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                        }
                    }
                    else if (tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo")
                    {
                        if (Dr[tableCols[i]] != (object)DBNull.Value)
                            dataGridView1.Rows[no - 1].Cells[tableCols[i]].Value = Convert.ToDateTime(Dr[tableCols[i]]);
                    }
                    else
                        dataGridView1.Rows[no - 1].Cells[i].Value = Dr[tableCols[i]];
                }
                no++;
            }

            //GV2
            Query = "select SA_SQ_Id from SalesOrderH where SalesOrderNo = '" + tbxSOID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string SA_SQ_Id = "";
            if (!(Cmd.ExecuteScalar() == null || Cmd.ExecuteScalar().ToString() == String.Empty))
            {
                dataGridView2.ColumnCount = Convert.ToInt32(tableCols.Length - 3);
                for (int i = 0; i < Convert.ToInt32(tableCols.Length - 3); i++)
                    dataGridView2.Columns[i].Name = tableCols[i];


                SA_SQ_Id = Cmd.ExecuteScalar().ToString();
                if (cbRef.Text == "Sales Agreement")
                {
                    Cmd = new SqlCommand("select TransType from SalesAgreementH where salesAgreementNo = '" + tbxSQID.Text + "'", Conn);
                    if (Cmd.ExecuteScalar().ToString() == "AMOUNT")
                        dataGridView2.Columns["RemainingQty"].HeaderText = "Remaining Amount";
                    else
                        dataGridView2.Columns["RemainingQty"].HeaderText = "Remaining Qty";
                }

                if (SA_SQ_Id.Split('/')[0] == "SQ")
                    Query = "select * from [ISBS-NEW4].[dbo].[SalesQuotationD] where SalesQuotationNo = '" + SA_SQ_Id + "' and Deleted = 'N' order by SeqNo asc";
                else if (SA_SQ_Id.Split('/')[0] == "SA")
                    Query = "select * from [SalesAgreement_Dtl] where SalesAgreementNo = '" + SA_SQ_Id + "' and Deleted = 'N' order by SeqNo asc";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int x = 0;
                while (Dr.Read())
                {
                    #region pass value to dataGridView2
                    dataGridView2.Rows.Add(1);
                    for (int i = 0; i < Convert.ToInt32(tableCols.Length - 3); i++)
                    {
                        if (!(tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo" || tableCols[i] == "SalesOrderNo"))
                        {
                            if (i == 0)
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = dataGridView2.Rows.Count;
                            else if (tableCols[i] == "DiscType")
                            {
                                Cmd = new SqlCommand("select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr[tableCols[i]].ToString() + "'", Conn);
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                            }
                            else if (tableCols[i] == "SA_SQ_Id")
                            {
                                if (cbRef.Text == "Sales Quotataion")
                                    dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["SalesQuotationNo"];
                                else if (cbRef.Text == "Sales Agreement")
                                    dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["SalesAgreementNo"];
                            }
                            else if (tableCols[i] == "SA_SQ_SeqNo")
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["SeqNo"];
                            else if (tableCols[i] == "RemainingQty")
                            {
                                if (cbRef.Text == "Sales Quotataion")
                                    dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["Qty"];
                                else if (cbRef.Text == "Sales Agreement")
                                    dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr["RemainingQty"];
                            }
                            else
                                dataGridView2.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                        }
                    }
                    x++;
                    #endregion
                }
                Dr.Close();
            }

            //steven edit s
            //Convert to test
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

            Query = "Select * From [tblAttachments] Where ReffTableName = 'SalesOrderH' And ReffTransId = '" + tbxSOID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                test.Add((byte[])Dr["Attachment"]);
            }

            dgvAttachment.AutoResizeColumns();
            //steven edit e
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

            Query = "Select * From [tblAttachments] Where ReffTableName = 'SalesOrderH' And ReffTransId = '" + SOID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                test.Add((byte[])Dr["Attachment"]);
            }

            dgvAttachment.AutoResizeColumns();
        }

        public void GetAttachmentSA()
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

            Query = "Select * From [tblAttachments] Where ReffTableName = 'SalesAgreementH' And ReffTransId = '" + tbxSQID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                test.Add((byte[])Dr["Attachment"]);
            }

            dgvAttachment.AutoResizeColumns();
        }

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
            }
            if (txtReferensi.Text != "By Phone")
            {
                txtReferensi.Enabled = true;
                cbByPhone.Enabled = true;
            }
            DateTime date1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 00, 00, 00);
            dtDP.MinDate = date1;

            if (tbxSQID.Text == String.Empty)
            {
                btnSCust.Enabled = true;

                if (tbxCustID.Text != String.Empty)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select DP_Required from CustTable where CustId = '" + tbxCustID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar().ToString() == "Y")
                        cbDPType.Enabled = false;
                    else
                        cbDPType.Enabled = true;
                    Conn.Close();
                }
                else
                    cbDPType.Enabled = true;
                //cbDPType.Enabled = true;
                if (cbDPType.Text == "Y")
                { tbxDP.Enabled = true; tbxDPPercent.Enabled = true; dtDP.Enabled = true; }
                else
                { tbxDP.Enabled = false; tbxDPPercent.Enabled = false; dtDP.Enabled = false; }
                //else { tbxDP.Enabled = false; tbxDPPercent.Enabled = false; }
                dtDP.Enabled = true;
                cbCurrency.Enabled = true; cbToP.Enabled = true;
                cbPPN.Enabled = true; cbPPh.Enabled = true; cbPaymentMode.Enabled = true;
                if (cbCurrency.Text == "IDR")
                    tbxXRate.Enabled = false;
                else
                    tbxXRate.Enabled = true;
            }
            btnSCust.Enabled = false;
            dateTimePicker3.Enabled = true;
            tbxNotes.Enabled = true;

            btnAddItem.Enabled = true; btnDeleteItem.Enabled = true; btnReserved.Enabled = false;

            btnEdit.Enabled = false; btnCancel.Enabled = true; btnSave.Enabled = true; btnConfirm.Visible = false;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (tbxSQID.Text == String.Empty)
                {
                    if (tableCols[i] == "Qty" || tableCols[i] == "Price" || tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo" || tableCols[i] == "LogisticAmount" || tableCols[i] == "LogisticNotes" || tableCols[i] == "DiscType" || tableCols[i] == "DiscPercent" || tableCols[i] == "DiscAmount" || tableCols[i] == "BonusAmount" || tableCols[i] == "CashBackAmount" || tableCols[i] == "Notes")
                    {
                        dataGridView1.Columns[i].ReadOnly = false;
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                else
                {
                    if (tableCols[i] == "Notes")
                    {
                        dataGridView1.Columns[i].ReadOnly = false;
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dataGridView1.Columns[i].ReadOnly = true;
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
            }

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (dataGridView1.Rows[i].Cells["Base"].Value == null || dataGridView1.Rows[i].Cells["Base"].Value == String.Empty)
                        dataGridView1.Rows[i].Cells["Base"].Value = "";
                    if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y") //hanya yang Base Y bisa ganti Qty
                    {
                        dataGridView1.Rows[i].Cells["Qty"].Style.BackColor = Color.White;
                        dataGridView1.Rows[i].Cells["Qty"].ReadOnly = false;
                    }
                }
            }


            //DATE ON GV
            //_dateTimePicker.Visible = false;
            //_dateTimePicker.CustomFormat = "dd-MM-yyyy";
            //_dateTimePicker.ValueChanged += cellDateTimePickerValueChanged;
            //dataGridView1.Dock = DockStyle.Fill;
            //dataGridView1.Controls.Add(_dateTimePicker);
            //dataGridView1.CellClick += dataGridView1_CellClick;

            //dataGridView1.Columns["ExpectedDateTo"].DefaultCellStyle.Format = "dd-MM-yyyy";
            //dataGridView1.Columns["ExpectedDateFrom"].DefaultCellStyle.Format = "dd-MM-yyyy";
        }

        private void ModeNew()
        {
            Mode = "New";
            //STEVEN EDIT START
            if (txtReferensi.Text == "By Phone")
                txtReferensi.Enabled = false;
            else
                txtReferensi.Enabled = true;
            cbByPhone.Enabled = true;
            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnDelAttachment.Enabled = true;
            dgvAttachment.Rows.Clear();
            if (cbRef.Text == "Sales Agreement")
            {
                GetAttachmentSA();
            }
            else GetDgvAttachmentData();
            //STEVEN EDIT START
            cbRef.Enabled = true;
            DateTime date1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 00, 00, 00);
            dtDP.MinDate = date1;
            date1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 00, 00, 00);
            dateTimePicker3.MinDate = date1;

            if (cbCurrency.Text == String.Empty)
                cbCurrency.SelectedText = "IDR";
            if (cbPaymentMode.Text == String.Empty)
                cbPaymentMode.SelectedIndex = 0;
            if (cbDPType.Text == String.Empty)
                cbDPType.SelectedIndex = 1;
            if (cbPPh.Text == String.Empty)
                cbPPh.SelectedIndex = 0;
            if (cbPPN.Text == String.Empty)
                cbPPN.SelectedIndex = 1;

            dateTimePicker1.Value = DateTime.Now;
            dtDP.Value = DateTime.Now;
            dateTimePicker3.Value = DateTime.Now;

            dtDP.Enabled = false;
            dateTimePicker3.Enabled = true;

            if (tbxSQID.Text != String.Empty)
            {
                cbDPType.Enabled = false;
                tbxDP.Enabled = false;
                tbxDPPercent.Enabled = false;
                cbCurrency.Enabled = false; tbxXRate.Enabled = false; cbToP.Enabled = false;
                cbPPh.Enabled = false; cbPPN.Enabled = false; cbPaymentMode.Enabled = false;
            }
            else
            {
                if (tbxCustID.Text != String.Empty)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select DP_Required from CustTable where CustId = '" + tbxCustID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar().ToString() == "Y")
                        cbDPType.Enabled = false;
                    else
                        cbDPType.Enabled = true;
                    Conn.Close();
                }
                else
                    cbDPType.Enabled = true;
                cbCurrency.Enabled = true; tbxXRate.Enabled = false; cbToP.Enabled = true;
                cbPPh.Enabled = true; cbPPN.Enabled = true; cbPaymentMode.Enabled = true;
                if (cbDPType.Text == "Y")
                {
                    tbxDP.Enabled = true; tbxDPPercent.Enabled = true;
                    dtDP.Enabled = true;
                }
                else
                {
                    tbxDP.Enabled = false; tbxDPPercent.Enabled = false;
                    dtDP.Enabled = false;
                }
            }
            btnAddItem.Enabled = true; btnDeleteItem.Enabled = true;
            tbxNotes.Enabled = true;
            btnEdit.Enabled = false; btnSave.Enabled = true;
            btnAmend.Visible = false; btnAmend.Enabled = false;
            if (tbxRefID.Text != String.Empty)
                btnCancel.Enabled = true;
            else
                btnCancel.Enabled = false;

            if (tbxSQID.Text != String.Empty)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    if (!(tableCols[i] == "Notes"))
                    {
                        dataGridView1.Columns[i].ReadOnly = true;
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dataGridView1.Columns[i].ReadOnly = false;
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                }
                if (cbRef.Text == "Sales Agreement")
                {
                    dataGridView1.Columns["Qty"].ReadOnly = false;
                    dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.White;
                }
            }
            else
            {
                if (dataGridView1.RowCount != 0)
                {
                    for (int i = 0; i < tableCols.Length; i++)
                    {
                        if (tableCols[i] == "No" || tableCols[i] == "SalesOrderNo" || tableCols[i] == "SeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "ItemID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "FullItemID" || tableCols[i] == "ItemName" || tableCols[i] == "Unit" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "Price_Alt" || tableCols[i] == "ConvertionRatio" || tableCols[i] == "SubTotal" || tableCols[i] == "SubTotal_PPN" || tableCols[i] == "SubTotal_PPH" || tableCols[i] == "Base" || tableCols[i] == "DeliveryMethod" || tableCols[i] == "SA_SQ_Id" || tableCols[i] == "SA_SQ_SeqNo" || tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo" || tableCols[i] == "PLJNo" || tableCols[i] == "PLJSeqNo" || tableCols[i] == "PLJPrice" || tableCols[i] == "RemainingQty")
                        {
                            dataGridView1.Columns[i].ReadOnly = true;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            dataGridView1.Columns[i].ReadOnly = false;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
            }
            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                if (tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "Price_Alt" || tableCols[i] == "ConvertionRatio" || tableCols[i] == "SA_SQ_Id" || tableCols[i] == "SA_SQ_SeqNo" || tableCols[i] == "RemainingQty" || tableCols[i] == "SalesOrderNo" || tableCols[i] == "SeqNo" || tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo")
                    dataGridView2.Columns[i].Visible = false;

                dataGridView2.Columns[i].ReadOnly = true;
                dataGridView2.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
            }

            //DATE ON GV
            //_dateTimePicker.Visible = false;
            //_dateTimePicker.CustomFormat = "dd-MM-yyyy";
            //_dateTimePicker.ValueChanged += cellDateTimePickerValueChanged;
            //dataGridView1.Dock = DockStyle.Fill;
            //dataGridView1.Controls.Add(_dateTimePicker);
            //dataGridView1.CellClick += dataGridView1_CellClick;

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

            cbRef.Enabled = false;
            btnSSQ.Enabled = false; btnSCust.Enabled = false; btnSMoU.Enabled = false;
            cbDPType.Enabled = false; cbCurrency.Enabled = false; dtDP.Enabled = false; dateTimePicker3.Enabled = false;
            cbToP.Enabled = false; tbxXRate.Enabled = false; dateTimePicker1.Enabled = false;

            btnAddItem.Enabled = false; btnDeleteItem.Enabled = false; btnReserved.Visible = true; btnReserved.Enabled = true;

            cbPPh.Enabled = false; cbPPN.Enabled = false;
            tbxDP.Enabled = false; tbxDPPercent.Enabled = false; tbxNotes.Enabled = false;
            cbPaymentMode.Enabled = false;

            btnEdit.Enabled = true; btnCancel.Enabled = false; btnSave.Enabled = false; btnConfirm.Visible = true;

            //STEVEN EDIT START
            txtReferensi.Enabled = false;
            cbByPhone.Enabled = false;
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;

            dgvAttachment.ReadOnly = true;
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;
            //STEVEN EDIT END
            //tia edit
            tbxCustName.Enabled = true;
            tbxCustID.Enabled = true;
            tbxCustID.ReadOnly = true;
            tbxCustName.ReadOnly = true;
            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            tbxMoUID.Enabled = true;
            tbxMoUID.ReadOnly = true;
            tbxMoUID.ContextMenu = vendid;
            //tia edit end
            btnEdit.Visible = false;
            btnReserved.Visible = false;
             btnConfirm.Visible = false;
            btnAmend.Visible = false;
            btnApprove.Visible = false;
            btnReject.Visible = false;
           btnSave.Visible = false;
           btnCancel.Visible = false;

            if (tbxSQID.Text != String.Empty)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
            else
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            if (dataGridView2.ColumnCount != 0)
            {
                for (int i = 0; i < dataGridView2.ColumnCount; i++)
                {
                    if (tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "Price_Alt" || tableCols[i] == "ConvertionRatio" || tableCols[i] == "SA_SQ_Id" || tableCols[i] == "SA_SQ_SeqNo" || tableCols[i] == "SalesOrderNo" || tableCols[i] == "SeqNo" || tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo")
                        dataGridView2.Columns[i].Visible = false;
                    dataGridView2.Columns[i].ReadOnly = true;
                    dataGridView2.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }
        //tia edit end

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            cbRef.Enabled = false;
            btnSSQ.Enabled = false; btnSCust.Enabled = false; btnSMoU.Enabled = false;
            cbDPType.Enabled = false; cbCurrency.Enabled = false; dtDP.Enabled = false; dateTimePicker3.Enabled = false;
            cbToP.Enabled = false; tbxXRate.Enabled = false; dateTimePicker1.Enabled = false;

            btnAddItem.Enabled = false; btnDeleteItem.Enabled = false; btnReserved.Visible = true; btnReserved.Enabled = true;

            cbPPh.Enabled = false; cbPPN.Enabled = false;
            tbxDP.Enabled = false; tbxDPPercent.Enabled = false; tbxNotes.Enabled = false;
            cbPaymentMode.Enabled = false;
           
            btnEdit.Enabled = true; btnCancel.Enabled = false; btnSave.Enabled = false; btnConfirm.Visible = true;

            //STEVEN EDIT START
            txtReferensi.Enabled = false;
            cbByPhone.Enabled = false;
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;

            dgvAttachment.ReadOnly = true;
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;
            //STEVEN EDIT END
            //tia edit
            tbxCustName.Enabled = true;
            tbxCustID.Enabled = true;
            tbxMoUID.Enabled = true;
            tbxSQID.Enabled = true;

            tbxCustID.ReadOnly = true;
            tbxCustName.ReadOnly = true;
            tbxMoUID.ReadOnly = true;
            tbxSQID.ReadOnly = true;

            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            tbxMoUID.ContextMenu = vendid;
            tbxSQID.ContextMenu = vendid;
            //tia edit end
            if (cbDPType.Text == "N")
            {
                dtDP.Visible = false; llDPDueDate.Visible = false;
            }
            else
            {
                dtDP.Visible = true; llDPDueDate.Visible = true;
            }

            SqlConnection Conn = ConnectionString.GetConnection();
            Query = "Select TransStatus from [ISBS-NEW4].[dbo].[SalesOrderH] where [SalesOrderNo] = '" + tbxSOID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string transStatus = Cmd.ExecuteScalar().ToString();

            if (transStatus == "09" || transStatus == "12")
            {
                btnEdit.Enabled = true;
                btnReserved.Enabled = false;
                btnConfirm.Enabled = false; btnConfirm.Visible = false;
                btnAmend.Enabled = false; btnAmend.Visible = false;
                btnApprove.Enabled = true; btnApprove.Visible = true;
                btnReject.Enabled = true; btnReject.Visible = true;
            }
            else if (transStatus == "01" || transStatus == "05")
            {
                btnEdit.Enabled = true;
                btnReserved.Enabled = false;
                btnConfirm.Enabled = true; btnConfirm.Visible = true;
                btnAmend.Enabled = false; btnAmend.Visible = false;
                btnApprove.Enabled = false; btnApprove.Visible = false;
                btnReject.Enabled = false; btnReject.Visible = false;
            }
            else if (transStatus == "10")
            {
                btnEdit.Enabled = false;
                btnReserved.Enabled = false;
                btnConfirm.Enabled = true; btnConfirm.Visible = true;
                btnAmend.Enabled = true; btnAmend.Visible = true;
                btnApprove.Enabled = false; btnApprove.Visible = false;
                btnReject.Enabled = false; btnReject.Visible = false;
            }
            else if (transStatus == "03" || transStatus == "08" || transStatus == "06")
            {
                btnEdit.Enabled = false;
                btnReserved.Enabled = true;
                btnConfirm.Enabled = false; btnConfirm.Visible = false;
                btnAmend.Enabled = true; btnAmend.Visible = true;
                btnApprove.Enabled = false; btnApprove.Visible = false;
                btnReject.Enabled = false; btnReject.Visible = false;
            }
            else
            {
                btnEdit.Enabled = false;
                btnReserved.Enabled = false;
                btnConfirm.Enabled = false; btnConfirm.Visible = false;
                btnAmend.Enabled = false; btnAmend.Visible = false;
                btnApprove.Enabled = false; btnApprove.Visible = false;
                btnReject.Enabled = false; btnReject.Visible = false;
            }

            if (ControlMgr.GroupName == "Sales Manager")
            {
                btnApprove.Visible = true;
                btnReject.Visible = true;
            }
            else
            {
                btnApprove.Visible = false;
                btnReject.Visible = false;
            }

            if (tbxSQID.Text != String.Empty)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
            else
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            if (dataGridView2.ColumnCount != 0)
            {
                for (int i = 0; i < dataGridView2.ColumnCount; i++)
                {
                    if (tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "Price_Alt" || tableCols[i] == "ConvertionRatio" || tableCols[i] == "SA_SQ_Id" || tableCols[i] == "SA_SQ_SeqNo" || tableCols[i] == "SalesOrderNo" || tableCols[i] == "SeqNo" || tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo")
                        dataGridView2.Columns[i].Visible = false;
                    dataGridView2.Columns[i].ReadOnly = true;
                    dataGridView2.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        //type value : #1 string #2 decimal #3 int
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

        private void returnTabItemDetail()
        {
            if (cbToP.SelectedIndex <= 0)
                metroTabControl1.SelectedIndex = 0;
            else if (cbPaymentMode.SelectedIndex <= 0)
                metroTabControl1.SelectedIndex = 0;
            else
                metroTabControl1.SelectedIndex = 1;
        }

        private void returnTabDocuments()
        {
            if (cbToP.SelectedIndex <= 0)
                metroTabControl1.SelectedIndex = 0;
            else if (cbPaymentMode.SelectedIndex <= 0)
                metroTabControl1.SelectedIndex = 0;
            else
                metroTabControl1.SelectedIndex = 4;
        }

        private char Validation()
        {
            flag = '\0'; msg = null;
            if (validate == false) { label = new Label[20]; }

            createLabel(cbDPType, lblDPType, tabSales, "string");
            createLabel(tbxCustID, lblCustomer, gbMain, "string");
            createLabel(cbCurrency, lblCurrency, tabSales, "string");
            createLabel(cbToP, lblToP, tabSales, "string");
            createLabel(tbxXRate, lblXRate, tabSales, "decimal");
            createLabel(cbPaymentMode, lblPaymentMode, tabSales, "string");
            createLabel(cbPPN, lblPPN, tabSales, "string");
            createLabel(cbPPh, lblPPh, tabSales, "string");
            createLabel(txtReferensi, label5, tabAttachment, "string");

            if (tbxCustID.Text == String.Empty)
                tbxCustName.BackColor = Color.LightGoldenrodYellow;
            else
                tbxCustName.BackColor = Color.Empty;

            if (flag == 'X')
                msg += "* field is required!\r\n";

            Conn = ConnectionString.GetConnection();

            if (Mode == "New" && (cbRef.Text == "Sales Quotation" || cbRef.Text == "Sales Agreement") && tbxSQID.Text.Trim() == "")
                msg += lblSASQId.Text + " harus diisi!\r\n";

            if (cbRef.Text == "Sales Agreement" && tbxSQID.Text.Trim() != "")
            {
                decimal totalNett = 0;
                List<int> row = new List<int>();

                Query = "select TransType from SalesAgreementH where SalesAgreementNo = '" + tbxSQID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                if (Cmd.ExecuteScalar().ToString() == "AMOUNT")
                {
                    #region POPULATE TOTAL NETT BASE N THEN COMPARE WITH BASE Y
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        row.Add(i + 1);

                        Query = "select top 1 SeqNo from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSQID.Text + "' and Base = 'Y' and SeqNo <= '" + dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value + "' order by seqNo desc";
                        Cmd = new SqlCommand(Query, Conn);
                        int BaseY_SeqNo = Convert.ToInt32(Cmd.ExecuteScalar());

                        Query = "select RemainingQty from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSQID.Text + "' and SeqNo = '" + BaseY_SeqNo + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal SARemainingQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                        decimal subTotal = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value);
                        decimal ppn = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value);
                        decimal pph = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value);
                        totalNett = totalNett + subTotal + ppn + pph;

                        if (i + 1 < dataGridView1.RowCount)
                        {
                            Query = "select top 1 SeqNo from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSQID.Text + "' and Base = 'Y' and SeqNo <= '" + dataGridView1.Rows[i + 1].Cells["SA_SQ_SeqNo"].Value + "' order by seqNo desc";
                            Cmd = new SqlCommand(Query, Conn);
                            if (Convert.ToInt32(Cmd.ExecuteScalar()) != BaseY_SeqNo)
                            {
                                if (totalNett > SARemainingQty)
                                {
                                    string rowString = "";
                                    for (int j = 0; j < row.Count; j++)
                                    {
                                        if (j == row.Count - 1 && j != 0)
                                            rowString += " and ";
                                        else if (j >= 1)
                                            rowString += ", ";
                                        rowString += row[j];
                                    }
                                    msg += "Row " + rowString + " cannot exceed agreement amount!\r\n";
                                    returnTabItemDetail();
                                }
                                row.Clear();
                                totalNett = 0;
                            }
                        }
                        else
                        {
                            if (totalNett > SARemainingQty)
                            {
                                string rowString = "";
                                for (int j = 0; j < row.Count; j++)
                                {
                                    if (j == row.Count - 1 && j != 0)
                                        rowString += " and ";
                                    else if (j >= 1)
                                        rowString += ", ";
                                    rowString += row[j];
                                }
                                msg += "Row " + rowString + " cannot exceed agreement amount!\r\n";
                                returnTabItemDetail();
                            }
                            row.Clear();
                            totalNett = 0;
                        }
                    }
                    #endregion
                }
                else if (Cmd.ExecuteScalar().ToString() == "QUANTITY")
                {
                    #region POPULATE QUANTITY BASE N THEN COMPARE WITH BASE Y
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        row.Add(i + 1);

                        Query = "select top 1 SeqNo from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSQID.Text + "' and Base = 'Y' and SeqNo <= '" + dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value + "' order by seqNo desc";
                        Cmd = new SqlCommand(Query, Conn);
                        int BaseY_SeqNo = Convert.ToInt32(Cmd.ExecuteScalar());

                        Query = "select RemainingQty from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSQID.Text + "' and SeqNo = '" + BaseY_SeqNo + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal SARemainingQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                        decimal qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                        totalNett = totalNett + qty;

                        if (i + 1 < dataGridView1.RowCount)
                        {
                            Query = "select top 1 SeqNo from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSQID.Text + "' and Base = 'Y' and SeqNo <= '" + dataGridView1.Rows[i + 1].Cells["SA_SQ_SeqNo"].Value + "' order by seqNo desc";
                            Cmd = new SqlCommand(Query, Conn);
                            if (Convert.ToInt32(Cmd.ExecuteScalar()) != BaseY_SeqNo)
                            {
                                if (totalNett > SARemainingQty)
                                {
                                    string rowString = "";
                                    for (int j = 0; j < row.Count; j++)
                                    {
                                        if (j == row.Count - 1 && j != 0)
                                            rowString += " and ";
                                        else if (j >= 1)
                                            rowString += ", ";
                                        rowString += row[j];
                                    }
                                    msg += "Row " + rowString + " cannot exceed agreement amount!\r\n";
                                    returnTabItemDetail();
                                }
                                row.Clear();
                                totalNett = 0;
                            }
                        }
                        else
                        {
                            if (totalNett > SARemainingQty)
                            {
                                string rowString = "";
                                for (int j = 0; j < row.Count; j++)
                                {
                                    if (j == row.Count - 1 && j != 0)
                                        rowString += " and ";
                                    else if (j >= 1)
                                        rowString += ", ";
                                    rowString += row[j];
                                }
                                msg += "Row " + rowString + " cannot exceed agreement amount!\r\n";
                                returnTabItemDetail();
                            }
                            row.Clear();
                            totalNett = 0;
                        }
                    }
                    #endregion
                }
            }

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells["Qty"].Value != String.Empty)
                {
                    decimal tempQty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                    for (int j = 0; j < dataGridView1.RowCount; j++)
                    {
                        if (j != i)
                        {
                            if (dataGridView1.Rows[j].Cells["Qty"].Value == String.Empty || dataGridView1.Rows[j].Cells["Qty"].Value == null)
                                dataGridView1.Rows[j].Cells["Qty"].Value = 0;
                            if (dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString() == dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString())
                            {
                                tempQty = tempQty + Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                            }
                        }
                    }

                    Query = "Select Available_For_Sale_UoM from [ISBS-NEW4].[dbo].[Invent_OnHand_Qty] where FullItemID = '" + dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    decimal totalQty_Available_For_Sale_UoM = 0;
                    while (Dr.Read())
                    {
                        totalQty_Available_For_Sale_UoM += Convert.ToDecimal(Dr["Available_For_Sale_UoM"]);
                    }
                    Dr.Close();
                    if (tempQty > totalQty_Available_For_Sale_UoM)
                    {
                        msg += "Total quantity for item " + dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString() + " cannot more than " + totalQty_Available_For_Sale_UoM.ToString() + "!\r\n";                                                
                        returnTabItemDetail();
                    }
                }
            }


            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells["Qty"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) == 0)
                {
                    msg += "Row " + Convert.ToInt32(i + 1) + " quantity cannot 0!\r\n";               
                    returnTabItemDetail();
                }

                if (dataGridView1.Rows[i].Cells["Price"].Value == String.Empty || dataGridView1.Rows[i].Cells["Price"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) == 0)
                {
                    msg += "Row " + Convert.ToInt32(i + 1) + " price cannot 0!\r\n";
                    returnTabItemDetail();
                }

                if (dataGridView1.Rows[i].Cells["DeliveryMethod"].Value.ToString() == "Select")
                {
                    msg += "Row " + Convert.ToInt32(i + 1) + " Select Delivery Method!\n";
                    returnTabItemDetail();
                }

                if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() != "N")
                {
                    if (dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == String.Empty)
                        msg += "Row " + Convert.ToInt32(i + 1) + " Fill Expected Date From!\n"; returnTabItemDetail(); //metroTabControl1.SelectedIndex = 1;

                    if (dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == String.Empty)
                        msg += "Row " + Convert.ToInt32(i + 1) + " Fill Expected Date To!\n"; returnTabItemDetail();//metroTabControl1.SelectedIndex = 1;

                    if (!(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == String.Empty || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == String.Empty))
                    {
                        ////CHECK RESULT
                        int result = DateTime.Compare(Convert.ToDateTime(Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value).ToShortDateString()), Convert.ToDateTime(Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value).ToShortDateString()));
                        //int result = DateTime.Compare(Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value), Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value));
                        //string relationship;
                        if (result < 0) { }//relationship = "is earlier than";
                        else if (result == 0) { } //relationship = "is the same time as";
                        else //relationship = "is later than";
                        {
                            msg += "Row " + Convert.ToInt32(i + 1) + " Expected Date To (" + Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value).ToString("dd/MM/yyyy") + ") must be later than Date From (" + Convert.ToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value).ToString("dd/MM/yyyy") + ")\n";
                            returnTabItemDetail();
                        }
                    }
                }

                if (dataGridView1.Rows[i].Cells["DeliveryMethod"].Value.ToString().ToUpper() == "FRANCO")
                {
                    if (tbxSQID.Text == "")
                    {
                        if (dataGridView1.Rows[i].Cells["LogisticAmount"].Value == String.Empty || dataGridView1.Rows[i].Cells["LogisticAmount"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value) == 0)
                            msg += "Row " + Convert.ToInt32(i + 1) + " Logistik Amount tidak boleh 0 karena Franco!\n";
                    }
                    else
                    {
                        if (dataGridView2.Rows[i].Cells["LogisticAmount"].Value == String.Empty || dataGridView2.Rows[i].Cells["LogisticAmount"].Value == null || Convert.ToDecimal(dataGridView2.Rows[i].Cells["LogisticAmount"].Value) == 0)
                            msg += "Row " + Convert.ToInt32(i + 1) + " Logistik Amount tidak boleh 0 karena Franco!\n";
                    }
                }

                if (dataGridView1.Rows[i].Cells["Qty"].Value != (object)DBNull.Value && dataGridView1.Rows[i].Cells["Qty"].Value != String.Empty)
                {
                    //CHECK QTY CANNOT MORE THAN REMAINGING QTY WHEN AMEND
                    if (tbxRefID.Text != String.Empty)
                    {
                        if (!(dataGridView1.Rows[i].Cells["SalesOrderNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["SalesOrderNo"].Value == null))
                        {
                            if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) > Convert.ToDecimal(dataGridView1.Rows[i].Cells["RemainingQty"].Value))
                            {
                                msg += "Row " + Convert.ToInt32(i + 1) + " Qty cannot more than " + dataGridView1.Rows[i].Cells["RemainingQty"].Value.ToString() + "!\r\n";                               
                                returnTabItemDetail();
                            }
                        }
                    }
                }
            }

            //STEVEN EDIT START
            //if (cbByPhone.Checked == false && txtReferensi.Text.Trim() == "")//Jika tidak pakai "By Phone"
            //{
                //REMARKED BY: HC (S) 05.06.2018
                //if (dgvAttachment.RowCount > 0)//Ada Attachment tapi Reference kosong 
                //{
                //    MessageBox.Show("Reference harus diisi karena tidak by Phone.");
                //    return;
                //}
                //REMARKED BY: HC (E)
                if (dgvAttachment.RowCount < 1)//Tidak Ada Attachment
                {
                    //REMARK BY: HC 13.04.2018
                    //MessageBox.Show("Attachment harus ada.");
                    //return;
                    msg += "Must insert Attachment!\r\n"; //BY: HC 13.04.2018
                    //metroTabControl1.SelectedIndex = 4;
                    returnTabDocuments();
                }

                SqlCommand check_Referensi = new SqlCommand("SELECT COUNT(*) FROM [tblRef] WHERE ([Referensi] = @Referensi)", Conn);
                check_Referensi.Parameters.AddWithValue("@Referensi", txtReferensi.Text);
                int ReferensiExist = (int)check_Referensi.ExecuteScalar();

                if (ReferensiExist > 0)
                {
                    //REMARK BY: HC 13.04.2018
                    //MessageBox.Show("Referensi sudah digunakan.");
                    //return;
                    msg += "Customer Document reference has been used!\r\n"; //BY: HC 13.04.2018                   
                    returnTabDocuments();
                }
            //}

            Conn.Close();

            if (dataGridView1.Rows.Count == 0)
            {
                msg += "Please add Item!\n"; 
                returnTabItemDetail();
               
            }

            if (!(String.IsNullOrEmpty(msg)))
            {
                //MessageBox.Show(msg, "Warning");
                MetroFramework.MetroMessageBox.Show(this, msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                flag = 'X';
            }
            count = 0;
            validate = true;
            return flag;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void addItemWithRefID()
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
            f.SetSchemaTable("dbo", "SalesOrderD", "and a.SalesOrderNo = '" + tbxRefID.Text + "' and a.Deleted = 'N'" + excludeItem, "a.*", "SalesOrderD a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                string where = "";
                for (int i = 0; i < SearchV2.data.Count; i++)
                {
                    if (i >= 1)
                        where += " or ";
                    where += "(SalesOrderNo = '" + SearchV2.data[i] + "' and SeqNo = '" + SearchV2.data2[i] + "')";
                }
                Conn = ConnectionString.GetConnection();
                Query = "Select * from SalesOrderD where " + where;
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    #region pass value to GV
                    dataGridView1.Rows.Add(1);
                    for (int i = 0; i < tableCols.Length; i++)
                    {
                        if (!(tableCols[i] == "SalesOrderNo" || tableCols[i] == "SeqNo"))
                        {
                            if (i == 0)
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[0].Value = dataGridView1.RowCount;
                            else if (tableCols[i] == "DiscType")
                            {
                                cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                                cell.Value = "Select";
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                            }
                            else if (tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo")
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Convert.ToDateTime(Dr[tableCols[i]]);
                            else if (tableCols[i] == "RefTransId")
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["SalesOrderNo"];
                            else if (tableCols[i] == "RefTrans_SeqNo")
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["SeqNo"];
                            else
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                        }
                    }
                    #endregion
                }
                Dr.Close();
                Conn.Close();
            }
        }

        private void addItemWithSQID()
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
                excludeItem += "'" + dataGridView1.Rows[i].Cells["SA_SQ_SeqNo"].Value.ToString() + "'";
                if (i == dataGridView1.RowCount - 1)
                    excludeItem += ")";
            }
            switch (cbRef.Text)
            {
                case "Sales Quotation":
                    f.SetSchemaTable("dbo", "SalesQuotationD", "and a.SalesQuotationNo = '" + tbxSQID.Text + "' and a.Deleted = 'N'" + excludeItem, "a.*", "SalesQuotationD a");
                    break;
                case "Sales Agreement":
                    //f.SetSchemaTable("dbo", "SalesAgreement_Dtl", "and a.SalesAgreementNo = '" + tbxSQID.Text + "' and a.Deleted = 'N' and a.Base != 'Y'" + excludeItem, "a.*", "SalesAgreement_Dtl a");
                    f.SetSchemaTable("dbo", "SalesAgreement_Dtl", "and a.SalesAgreementNo = '" + tbxSQID.Text + "' and a.Deleted = 'N'" + excludeItem, "a.*", "SalesAgreement_Dtl a");
                    break;
            }
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                Conn = ConnectionString.GetConnection();
                if (cbRef.Text == "Sales Quotation")
                {
                    Query = "select * from [SalesQuotationD] where (";
                    for (int i = 0; i < SearchV2.data.Count; i++)
                    {
                        if (i >= 1)
                            Query += " or ";
                        Query += "SalesQuotationNo = '" + SearchV2.data[i] + "' and ";
                        Query += "SeqNo = '" + SearchV2.data2[i] + "'";
                    }
                    Query += ") order by SeqNo asc";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    int x = dataGridView1.RowCount;
                    while (Dr.Read())
                    {
                        #region pass value to gridview
                        dataGridView1.Rows.Add(1);
                        for (int i = 0; i < Convert.ToInt32(tableCols.Length - 2); i++)
                        {
                            if (!(tableCols[i] == "SalesOrderNo" || tableCols[i] == "SeqNo"))
                            {
                                if (i == 0)
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = dataGridView1.Rows.Count;
                                else if (tableCols[i] == "DiscType")
                                {
                                    Cmd = new SqlCommand("select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr[tableCols[i]].ToString() + "'", Conn);
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                                }
                                else if (tableCols[i] == "SA_SQ_Id")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["SalesQuotationNo"];
                                else if (tableCols[i] == "SA_SQ_SeqNo")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["SeqNo"];
                                else if (tableCols[i] == "RemainingQty")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["Qty"];
                                else
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                            }
                        }
                        x++;
                        #endregion
                    }
                    Dr.Close();
                }
                else if (cbRef.Text == "Sales Agreement")
                {
                    Query = "select * from [SalesAgreement_Dtl] where (";
                    for (int i = 0; i < SearchV2.data.Count; i++)
                    {
                        if (i >= 1)
                            Query += " or ";
                        Query += "SalesAgreementNo = '" + SearchV2.data[i] + "' and ";
                        Query += "SeqNo = '" + SearchV2.data2[i] + "'";
                    }
                    Query += ") order by SeqNo asc";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    int x = dataGridView1.RowCount;
                    while (Dr.Read())
                    {
                        #region add SA item to detail
                        dataGridView1.Rows.Add(1);
                        if (Dr["Base"].ToString() == "Y")
                        {
                            dataGridView1.Rows[x].Cells["Qty"].Value = "0";
                            dataGridView1.Rows[x].Cells["Qty"].Style.BackColor = Color.White;                            
                            dataGridView1.Rows[x].Cells["Qty_Alt"].Value = "0";

                            dataGridView1.Rows[x].Cells["Price"].Value = Convert.ToDecimal(Dr["Price"]);
                            dataGridView1.Rows[x].Cells["Price_Alt"].Value = Convert.ToDecimal(Dr["Price"]) / Convert.ToDecimal(Dr["ConvertionRatio"]);
                            dataGridView1.Rows[x].Cells["DeliveryMethod"].Value = Dr["DeliveryMethod"];
                            dataGridView1.Rows[x].Cells["ExpectedDateFrom"].Value = Dr["ExpectedDateFrom"];
                            dataGridView1.Rows[x].Cells["ExpectedDateTo"].Value = Dr["ExpectedDateTo"];
                            dataGridView1.Rows[x].Cells["LogisticNotes"].Value = Dr["LogisticNotes"];
                            dataGridView1.Rows[x].Cells["Notes"].Value = Dr["Notes"];

                        }
                        else if (Dr["Base"].ToString() == "N")
                        {
                            dataGridView1.Rows[x].Cells["Qty"].Value = Dr["RemainingQty"];
                            dataGridView1.Rows[x].Cells["Qty_Alt"].Value = Convert.ToDecimal(Dr["RemainingQty"]) / Convert.ToDecimal(Dr["ConvertionRatio"]);

                            Query = "select Price, Price_Alt, DeliveryMethod, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Notes from SalesAgreement_Dtl where salesagreementNo = '" + tbxSQID.Text + "' and GelombangId = '" + Dr["GelombangId"] + "' and BracketId = '" + Dr["BracketId"] + "' and Base = 'Y'";
                            Cmd = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd.ExecuteReader();
                            while (Dr2.Read())
                            {
                                dataGridView1.Rows[x].Cells["Price"].Value = Convert.ToDecimal(Dr["Price"]) + Convert.ToDecimal(Dr2["Price"]);
                                dataGridView1.Rows[x].Cells["Price_Alt"].Value = (Convert.ToDecimal(Dr["Price"]) + Convert.ToDecimal(Dr2["Price"])) / Convert.ToDecimal(Dr["ConvertionRatio"]);
                                dataGridView1.Rows[x].Cells["DeliveryMethod"].Value = Dr2["DeliveryMethod"];
                                dataGridView1.Rows[x].Cells["ExpectedDateFrom"].Value = Dr2["ExpectedDateFrom"];
                                dataGridView1.Rows[x].Cells["ExpectedDateTo"].Value = Dr2["ExpectedDateTo"];
                                dataGridView1.Rows[x].Cells["LogisticNotes"].Value = Dr2["LogisticNotes"];
                                dataGridView1.Rows[x].Cells["Notes"].Value = Dr2["Notes"];
                            }
                            Dr2.Close();
                        }

                        dataGridView1.Rows[x].Cells["No"].Value = dataGridView1.RowCount;
                        dataGridView1.Rows[x].Cells["GroupID"].Value = Dr["GroupID"];
                        dataGridView1.Rows[x].Cells["SubGroup1ID"].Value = Dr["SubGroup1ID"];
                        dataGridView1.Rows[x].Cells["SubGroup2ID"].Value = Dr["SubGroup2ID"];
                        dataGridView1.Rows[x].Cells["ItemID"].Value = Dr["ItemID"];
                        dataGridView1.Rows[x].Cells["FullItemID"].Value = Dr["FullItemID"];
                        dataGridView1.Rows[x].Cells["ItemName"].Value = Dr["ItemName"];
                        dataGridView1.Rows[x].Cells["Base"].Value = Dr["Base"];
                        dataGridView1.Rows[x].Cells["Unit"].Value = Dr["Unit"];
                        dataGridView1.Rows[x].Cells["Unit_Alt"].Value = Dr["Unit_Alt"];
                        dataGridView1.Rows[x].Cells["ConvertionRatio"].Value = Dr["ConvertionRatio"];
                        dataGridView1.Rows[x].Cells["SubTotal"].Value = Dr["SubTotal"];
                        dataGridView1.Rows[x].Cells["SubTotal_PPN"].Value = Dr["SubTotal_PPN"];
                        dataGridView1.Rows[x].Cells["SubTotal_PPH"].Value = Dr["SubTotal_PPH"];
                        //dataGridView1.Rows[x].Cells["LogisticAmount"].Value = Dr["LogisticAmount"];
                        dataGridView1.Rows[x].Cells["LogisticAmount"].Value = 0;

                        Cmd = new SqlCommand("select [Deskripsi] from [DiskonScheme] where [DiskonSchemeID] = '" + Dr["DiscType"] + "'", Conn);
                        dataGridView1.Rows[x].Cells["DiscType"].Value = Cmd.ExecuteScalar().ToString();
                        //dataGridView1.Rows[x].Cells["DiscPercent"].Value = Dr["DiscPercent"];
                        //dataGridView1.Rows[x].Cells["DiscAmount"].Value = Dr["DiscAmount"];
                        dataGridView1.Rows[x].Cells["DiscPercent"].Value = 0;
                        dataGridView1.Rows[x].Cells["DiscAmount"].Value = 0;
                        //dataGridView1.Rows[x].Cells["BonusAmount"].Value = Dr["BonusAmount"];
                        //dataGridView1.Rows[x].Cells["CashBackAmount"].Value = Dr["CashBackAmount"];
                        dataGridView1.Rows[x].Cells["BonusAmount"].Value = 0;
                        dataGridView1.Rows[x].Cells["CashBackAmount"].Value = 0;
                        dataGridView1.Rows[x].Cells["SA_SQ_Id"].Value = Dr["SalesAgreementNo"];
                        dataGridView1.Rows[x].Cells["SA_SQ_SeqNo"].Value = Dr["SeqNo"];
                        dataGridView1.Rows[x].Cells["PLJNo"].Value = Dr["PLJNo"];
                        dataGridView1.Rows[x].Cells["PLJSeqNo"].Value = Dr["PLJSeqNo"];
                        dataGridView1.Rows[x].Cells["PLJPrice"].Value = Dr["PLJPrice"];
                        x++;
                        #endregion
                    }
                }
                Conn.Close();
            }
        }

        private void addItemElse()
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
            f.SetMode("Check");
            string where = "and type = 'sales' and ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and CAST(PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and Active = '1' and TransStatus = '03')a";
            string columnName = "case when a.PricelistNo is null then b.PricelistNo else a.PricelistNo end 'PricelistNo', case when a.SeqNo is null then b.SeqNo else a.SeqNo end 'SeqNo',case when a.AccountId is null then b.AccountId else a.AccountId end 'AccountId',case when a.FullItemId is null then b.FullItemId else a.FullItemId end 'FullItemId',case when a.ItemName is null then b.ItemName else a.ItemName end 'ItemName',case when a.Price is null then b.Price else a.Price end 'Price',case when a.DeliveryMethod is null then b.DeliveryMethod else a.DeliveryMethod end 'DeliveryMethod',case when a.Type is null then b.Type else a.Type end 'Type',case when a.ValidFrom is null then b.ValidFrom else a.ValidFrom end 'ValidFrom',case when a.ValidTo is null then b.ValidTo else a.ValidTo end 'ValidTo',case when a.PriceType is null then b.PriceType else a.PriceType end 'PriceType',case when a.Active is null then b.Active else a.Active end 'Active',case when a.TransStatus is null then b.TransStatus else a.TransStatus end 'TransStatus'";
            string table = "select a.PricelistNo, a.SeqNo, c.AccountId, a.FullItemId, a.ItemName, a.Price, a.DeliveryMethod, a.Type, b.ValidFrom, b.ValidTo, a.PriceType, b.Active, b.TransStatus from Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo left join PriceList_AccountList c on c.PriceListNo = b.PricelistNo where 1=1 and a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and CAST(a.PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and b.Active = '1' and b.TransStatus = '03'";
            string tableName2 = "(select * from (Select " + columnName + " from (" + table + wherePlus + criteria + " )a full join (" + table + " and c.AccountId = '" + tbxCustID.Text + "') b on a.PricelistNo = b.PricelistNo )a";
            f.SetSchemaTable("dbo", "Pricelist_Dtl", where, "case when a.PricelistNo is null then b.PricelistNo else a.PricelistNo end, case when a.SeqNo is null then b.SeqNo else a.SeqNo end ,case when a.AccountId is null then b.AccountId else a.AccountId end ,case when a.FullItemId is null then b.FullItemId else a.FullItemId end ,case when a.ItemName is null then b.ItemName else a.ItemName end ,case when a.Price is null then b.Price else a.Price end ,case when a.DeliveryMethod is null then b.DeliveryMethod else a.DeliveryMethod end", tableName2);
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                if (dataGridView1.RowCount - 1 <= 0)
                {
                    dataGridView1.ColumnCount = tableCols.Length;
                    for (int i = 0; i < (tableCols.Length); i++)
                    {
                        dataGridView1.Columns[i].Name = tableCols[i];
                    }
                }
                Query = "select a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.FullItemId, a.ItemName, b.UoM, b.UoMAlt, c.Ratio, a.PricelistNo, a.SeqNo, a.Price, a.DeliveryMethod from Pricelist_Dtl a left join InventTable b on a.FullItemId = b.FullItemID left join InventConversion c on a.FullItemId = c.FullItemID where (";
                for (int i = 0; i < SearchV2.data.Count; i++)
                {
                    if (i >= 1)
                        Query += " or ";
                    Query += "a.PricelistNo = '" + SearchV2.data[i] + "' and ";
                    Query += "a.SeqNo = '" + SearchV2.data2[i] + "'";
                }
                Query += ")";
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    this.dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", "", Dr["GroupId"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemID"], Dr["ItemName"], "", 0, 0, Dr["UoM"], 0, 0, Dr["UoMAlt"], Dr["Price"], Dr["Ratio"], Dr["DeliveryMethod"]);

                    cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                    cell.Value = "Select";
                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;

                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Price"].Value = Convert.ToDecimal(Dr["Price"]) * Convert.ToDecimal(Dr["Ratio"]);
                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["PLJNo"].Value = Dr["PricelistNo"];
                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["PLJSeqNo"].Value = Dr["SeqNo"];
                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["PLJPrice"].Value = Dr["Price"];
                }
                Dr.Close();
                Conn.Close();
                if (Mode == "New")
                    ModeNew();
                else if (Mode == "Edit")
                    ModeEdit();
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxRefID.Text != String.Empty)
                {
                    addItemWithRefID();
                }
                else if (tbxSQID.Text != String.Empty)
                {
                    addItemWithSQID();
                }
                else
                {
                    addItemElse();
                }
            }
            catch (Exception Ex)
            {
                MetroFramework.MetroMessageBox.Show(this, Ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //ALL CB KEYPRESS USE THIS FUNCTION
        private void cbDPType_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                if (tbxRefID.Text == String.Empty)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select Lock_Qty from InventLockTable where RefTransId = '" + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["SalesOrderNo"].Value + "' and RefTrans_SeqNo = '" + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["SeqNo"].Value + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            if (Convert.ToDecimal(Dr["Lock_Qty"]) == 0)
                                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                            else
                                MetroFramework.MetroMessageBox.Show(this, "This item has been reserved. Cannot delete this item!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                        dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                    Dr.Close();
                    Conn.Close();
                }
                else
                    dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                populateGVData();
            }
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

        private void btnSSQ_Click(object sender, EventArgs e)
        {
            tbxSQID.Text = "";
            ResetForm();
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            if (cbRef.Text == "Sales Quotation")
            {
                Query = "and a.TransStatus in ('01', '03', '09', '23') and a.TransType = 'FIX' ";
                Query += "and a.ValidFrom < DATEADD(day,0,GETDATE())";
                Query += "and a.ValidTo > DATEADD(day,-1,GETDATE())";
                f.SetSchemaTable("dbo", "SalesQuotationH", Query , "a.*", "SalesQuotationH a");
            }
            else if (cbRef.Text == "Sales Agreement")
            {
                Query = "and a.TransStatus in ('03', '06', '08', '12') ";
                Query += "and a.ValidTo > DATEADD(day,-1,GETDATE())";
                f.SetSchemaTable("dbo", "SalesAgreementH", Query, "a.*", "SalesAgreementH a");
            }
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                GetDataHeader();
                btnSCust.Enabled = false;
                btnSMoU.Enabled = false;
            }
        }

        private void ResetForm()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView2.Rows.Clear();
            dataGridView1.Rows.Clear();
            tbxMoUID.Text = "";
            tbxCustID.Text = "";
            tbxCustName.Text = "";

            cbCurrency.SelectedItem = "IDR";
            tbxXRate.Text = "1.00";
            cbToP.SelectedIndex = 0;
            cbPaymentMode.SelectedIndex = 0;

            cbDPType.SelectedItem = "N";
            tbxDP.Text = "0.00";
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
            tbxGCashback.Text = "0.00";
        }

        private void tbxToP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (tbxSOID.Text == String.Empty && tbxRefID.Text != String.Empty)
                tbxSOID.Text = tbxRefID.Text;
            Mode = "BeforeEdit";
            GetDataHeader();
            ModeBeforeEdit();
        }

        private char checkPriceTolerance()
        {
            flag = '\0';
            if (tbxSQID.Text == String.Empty)
            {
                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    Query = "select a.Tolerance from Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo where a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and b.Active = '1' and b.TransStatus = '03' and cast(a.PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and a.FullItemId = '" + dataGridView1.Rows[j].Cells["FullItemId"].Value + "' and a.PriceListNo = '" + dataGridView1.Rows[j].Cells["PLJNo"].Value + "' and a.SeqNo = '" + dataGridView1.Rows[j].Cells["PLJSeqNo"].Value + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    decimal tolerance = 0;
                    while (Dr.Read())
                    {
                        tolerance = Convert.ToDecimal(Dr["Tolerance"]);
                    }
                    Dr.Close();

                    decimal maxPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value) + (Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value) * tolerance / 100);
                    decimal priceNormal = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value) / Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value);
                    if (priceNormal > maxPrice || priceNormal < Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value))
                    {
                        flag = 'X';
                        return flag;
                    }
                }
            }
            return flag;
        }

        private void SaveNew()
        {
            #region variable
            string SalesOrderNo;
            DateTime OrderDate = dateTimePicker1.Value;
            string SalesMouNo = tbxMoUID.Text;
            string SA_SQ_Id = tbxSQID.Text;
            string CustID = tbxCustID.Text;
            string CustName = tbxCustName.Text;
            string CurrencyID = cbCurrency.Text;
            decimal ExchRate = Convert.ToDecimal(tbxXRate.Text);
            decimal Total = Convert.ToDecimal(tbxSTotal.Text);
            decimal Total_Disk = Convert.ToDecimal(tbxGDisc.Text);
            decimal PPN = Convert.ToDecimal(cbPPN.Text);
            decimal Total_PPN = Convert.ToDecimal(tbxGPPN.Text);
            decimal PPH = Convert.ToDecimal(cbPPh.Text);
            decimal Total_PPH = Convert.ToDecimal(tbxGPPh.Text);
            decimal Total_Nett = Convert.ToDecimal(tbxGTotal.Text);
            decimal Total_Bonus = Convert.ToDecimal(tbxGBonus.Text);
            decimal Total_Cashback = Convert.ToDecimal(tbxGCashback.Text);
            string TermofPayment = cbToP.Text;
            Cmd = new SqlCommand("Select [PaymentModeID] from [PaymentMode] where [PaymentModeName] = '" + cbPaymentMode.Text + "'", Conn);
            string PaymentModeID = Cmd.ExecuteScalar().ToString();
            string DPType = cbDPType.Text;
            decimal DPPercent = Convert.ToDecimal(tbxDPPercent.Text);
            decimal DPAmount = Convert.ToDecimal(tbxDP.Text);
            DateTime DPDueDate = dtDP.Value;
            string Notes = tbxNotes.Text;
            string TransStatus = "";
            if (tbxRefID.Text == String.Empty)
            {
                flag = checkPriceTolerance();
                if (flag == 'X')
                    TransStatus = "12"; //WAITING FOR APPROVAL
                else
                    TransStatus = "01"; //TransStatus Preorder
            }
            else
                TransStatus = "09"; //TransStatus Amended – Waiting for Approval
            DateTime ValidTo = dateTimePicker3.Value;
            string RefTransId = tbxRefID.Text;
            string Referensi = txtReferensi.Text;
            #endregion

            //begin==============================================================================
            //updated by : joshua
            if (tbxRefID.Text == String.Empty)
            {
                string Jenis = "SO", Kode = "SO";
                tbxSOID.Text = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
            }
            else
            {
                string Jenis = "SO", Kode = "SO";
                tbxSOID.Text = ConnectionString.GenerateSequenceNo(Jenis, Kode, "SalesOrderH", tbxRefID.Text, Conn, Trans, Cmd);
            }
            //end================================================================================
            SalesOrderNo = tbxSOID.Text;
            insertSOHeader(SalesOrderNo, OrderDate, SalesMouNo, SA_SQ_Id, CustID, CustName, CurrencyID, ExchRate, Total, Total_Disk, PPN, Total_PPN, PPH, Total_PPH, Total_Nett, Total_Bonus, Total_Cashback, TermofPayment, PaymentModeID, DPType, DPPercent, DPAmount, DPDueDate, Notes, TransStatus, ValidTo, RefTransId, Referensi);
            ListMethod.insertLogTable("N", "SalesOrder_LogTable", SalesOrderNo, "", "SalesOrder", TransStatus);

            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                #region variable
                int SeqNo = Convert.ToInt32(j + 1);
                string GroupID = dataGridView1.Rows[j].Cells["GroupID"].Value.ToString();
                string SubGroup1ID = dataGridView1.Rows[j].Cells["SubGroup1ID"].Value.ToString();
                string SubGroup2ID = dataGridView1.Rows[j].Cells["SubGroup2ID"].Value.ToString();
                string ItemID = dataGridView1.Rows[j].Cells["ItemID"].Value.ToString();
                string FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();
                string ItemName = dataGridView1.Rows[j].Cells["ItemName"].Value.ToString();
                string DeliveryMethod = dataGridView1.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                decimal LogisticAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["LogisticAmount"].Value == String.Empty || dataGridView1.Rows[j].Cells["LogisticAmount"].Value == null))
                    LogisticAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["LogisticAmount"].Value);
                string LogisticNotes = "";
                if (!(dataGridView1.Rows[j].Cells["LogisticNotes"].Value == null || dataGridView1.Rows[j].Cells["LogisticNotes"].Value == String.Empty))
                    LogisticNotes = dataGridView1.Rows[j].Cells["LogisticNotes"].Value.ToString();

                DateTime ExpectedDateFrom = new DateTime(1753, 01, 01);
                if (!(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value.ToString() == String.Empty))
                    ExpectedDateFrom = Convert.ToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value);
                DateTime ExpectedDateTo = new DateTime(1753, 01, 01);
                if (!(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value == String.Empty))
                    ExpectedDateTo = Convert.ToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value);
                decimal Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                string Unit = dataGridView1.Rows[j].Cells["Unit"].Value.ToString();
                decimal Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                string Unit_Alt = dataGridView1.Rows[j].Cells["Unit_Alt"].Value.ToString();
                decimal ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value);
                decimal Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value);
                decimal Price_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value);
                string DiscType = "1";
                if (dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() != "Select")
                {
                    Cmd = new SqlCommand("select [DiskonSchemeID] from [ISBS-NEW4].[dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[j].Cells["DiscType"].Value + "'", Conn);
                    DiscType = Cmd.ExecuteScalar().ToString();
                }
                decimal DiscPercent = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscPercent"].Value == null || dataGridView1.Rows[j].Cells["DiscPercent"].Value == String.Empty))
                    DiscPercent = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscPercent"].Value);
                decimal DiscAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscAmount"].Value == null || dataGridView1.Rows[j].Cells["DiscAmount"].Value == String.Empty))
                    DiscAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscAmount"].Value);
                decimal BonusAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["BonusAmount"].Value);
                decimal CashBackAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["CashBackAmount"].Value);
                decimal SubTotal = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal"].Value);
                decimal SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPN"].Value);
                decimal SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPH"].Value);
                string NotesD = "";
                if (!(dataGridView1.Rows[j].Cells["Notes"].Value == null || dataGridView1.Rows[j].Cells["Notes"].Value == String.Empty))
                    NotesD = dataGridView1.Rows[j].Cells["Notes"].Value.ToString();
                string SA_SQ_IdD = "";
                if (!(dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == null || dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == String.Empty || dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == (object)DBNull.Value))
                    SA_SQ_IdD = dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value.ToString();
                int SA_SQ_SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == (object)DBNull.Value))
                    SA_SQ_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value);
                int RefTrans_SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == (object)DBNull.Value))
                    RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value);
                string Base = "";
                if (!(dataGridView1.Rows[j].Cells["Base"].Value == null || dataGridView1.Rows[j].Cells["Base"].Value == String.Empty || dataGridView1.Rows[j].Cells["Base"].Value == (object)DBNull.Value))
                    Base = dataGridView1.Rows[j].Cells["Base"].Value.ToString();
                decimal RemainingQty = Qty;
                string PLJNo = dataGridView1.Rows[j].Cells["PLJNo"].Value.ToString();
                int PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value);
                decimal PLJPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value);
                #endregion

                if (cbRef.Text == "Sales Agreement")
                {
                    //if (Base == "N")
                    updateSARemainingQty(SOID, SeqNo, SA_SQ_IdD, SA_SQ_SeqNo, Qty, SubTotal);
                }

                insertSODetail(SalesOrderNo, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesD, SA_SQ_IdD, SA_SQ_SeqNo, RefTransId, RefTrans_SeqNo, Base, RemainingQty, PLJNo, PLJSeqNo, PLJPrice);

                if (TransStatus == "01")
                {
                    if (cbRef.Text == "Sales Agreement")
                        ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_From_SA_Issued_UoM]", "[SO_From_SA_Issued_Alt]", Qty, Qty_Alt);
                    else
                        ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_Preordered_UoM]", "[SO_Preordered_Alt]", Qty, Qty_Alt);
                }
                if (TransStatus == "09")
                    ListMethod.updateInventUomAlt("Decrease", "[Invent_Sales_Qty]", FullItemID, "[SO_Preordered_UoM]", "[SO_Preordered_Alt]", Qty, Qty_Alt);

            }

            insertSOAmountTable(SalesOrderNo, OrderDate, PPN, DPPercent, Total_Nett, Total_Nett, tbxCustID.Text);

            updateAmendedForm(); //UPDATE SQ, SA & AMENDED SO STATUS
            SaveDgvAttachmentData();

            if (cbByPhone.Checked == false && txtReferensi.Text.Trim() != "")
            {
                SqlCommand check_Referensi1 = new SqlCommand("SELECT COUNT(*) FROM [tblRef] WHERE ([Referensi] = @Referensi)", Conn);
                check_Referensi1.Parameters.AddWithValue("@Referensi", txtReferensi.Text);
                int ReferensiExist1 = (int)check_Referensi1.ExecuteScalar();

                if (ReferensiExist1 > 0)
                {
                    //MessageBox.Show("Referensi sudah digunakan.");
                    MetroFramework.MetroMessageBox.Show(this, "Referensi sudah digunakan.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    Query = "INSERT INTO [dbo].[tblRef] ([Referensi], [CreatedBy], [CreatedDate]) VALUES (";
                    Query += "'" + txtReferensi.Text + "', ";
                    Query += "'" + ControlMgr.UserId + "', ";
                    Query += "getdate());";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                }
            }
            //insertStatusLog("01");
            ListMethod.StatusLogCustomer("SOHeader", "SalesOrder", tbxCustID.Text, "01", "", tbxSOID.Text, "", "", "");
        }

        private void SaveEdit()
        {
            #region variable
            string SalesOrderNo;
            DateTime OrderDate = dateTimePicker1.Value;
            string SalesMouNo = tbxMoUID.Text;
            string SA_SQ_Id = tbxSQID.Text;
            string CustID = tbxCustID.Text;
            string CustName = tbxCustName.Text;
            string CurrencyID = cbCurrency.Text;
            decimal ExchRate = Convert.ToDecimal(tbxXRate.Text);
            decimal Total = Convert.ToDecimal(tbxSTotal.Text);
            decimal Total_Disk = Convert.ToDecimal(tbxGDisc.Text);
            decimal PPN = Convert.ToDecimal(cbPPN.Text);
            decimal Total_PPN = Convert.ToDecimal(tbxGPPN.Text);
            decimal PPH = Convert.ToDecimal(cbPPh.Text);
            decimal Total_PPH = Convert.ToDecimal(tbxGPPh.Text);
            decimal Total_Nett = Convert.ToDecimal(tbxGTotal.Text);
            decimal Total_Bonus = Convert.ToDecimal(tbxGBonus.Text);
            decimal Total_Cashback = Convert.ToDecimal(tbxGCashback.Text);
            string TermofPayment = cbToP.Text;
            Cmd = new SqlCommand("Select [PaymentModeID] from [PaymentMode] where [PaymentModeName] = '" + cbPaymentMode.Text + "'", Conn);
            string PaymentModeID = Cmd.ExecuteScalar().ToString();
            string DPType = cbDPType.Text;
            decimal DPPercent = Convert.ToDecimal(tbxDPPercent.Text);
            decimal DPAmount = Convert.ToDecimal(tbxDP.Text);
            DateTime DPDueDate = dtDP.Value;
            string Notes = tbxNotes.Text;
            string TransStatus = "";
            if (tbxRefID.Text == String.Empty)
            {
                flag = checkPriceTolerance();
                if (flag == 'X')
                    TransStatus = "12"; //WAITING FOR APPROVAL
                else
                    TransStatus = "01"; //TransStatus Preorder
            }
            else
                TransStatus = "09"; //TransStatus Amended – Waiting for Approval
            DateTime ValidTo = dateTimePicker3.Value;
            string RefTransId = tbxRefID.Text;
            string Referensi = txtReferensi.Text;
            #endregion

            SalesOrderNo = tbxSOID.Text;

            string OldTS = getTransStatus(SalesOrderNo);
            if (OldTS == "01")
            {
                if (cbRef.Text == "Sales Agreement")
                    ListMethod.revertInventUomAlt("Decrease", "Invent_Sales_Qty", "SO_From_SA_Issued_UoM", "SO_From_SA_Issued_Alt", "SalesOrderD", "SalesOrderNo", SalesOrderNo, "Qty", "Qty_Alt");
                else
                    ListMethod.revertInventUomAlt("Decrease", "Invent_Sales_Qty", "SO_Preordered_UoM", "SO_Preordered_Alt", "SalesOrderD", "SalesOrderNo", SalesOrderNo, "Qty", "Qty_Alt");
            }

            if (tbxRefID.Text == String.Empty)
            {
                flag = checkPriceTolerance();
                if (flag == 'X')
                    TransStatus = "12"; //WAITING FOR APPROVAL
                else
                    TransStatus = "01"; //TransStatus Preorder
            }
            else
                TransStatus = "09"; //TransStatus Amended – Waiting for Approval

            updateSOHeader(CurrencyID, ExchRate, Total, Total_Disk, PPN, Total_PPN, PPH, Total_PPH, Total_Nett, Total_Bonus, Total_Cashback, TermofPayment, PaymentModeID, DPType, DPPercent, DPAmount, DPDueDate, Notes, ValidTo, Referensi, TransStatus, SalesOrderNo);
            ListMethod.insertLogTable("E", "SalesOrder_LogTable", SalesOrderNo, "", "SalesOrder", TransStatus);

            //checkIfItemStillExist(tbxSOID.Text);

            //updateSODetailOld(SalesOrderNo,RefTransId,TransStatus);
            updateSODetailNew(SalesOrderNo, RefTransId, TransStatus);

            updateSOAmountTable(SalesOrderNo, PPN, DPPercent, Total_Nett, Total_Nett, tbxCustID.Text);

            updateAmendedForm(); //UPDATE SQ, SA & AMENDED SO STATUS

            Query = "Delete from tblAttachments where ReffTableName='SalesOrderH' And ReffTransId='" + SOID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            SaveDgvAttachmentData();

            if (cbByPhone.Checked == false && txtReferensi.Text.Trim() != "") //Referensi + Attachment tidak kosong
            {
                if (dgvAttachment.RowCount > 0)
                {
                    vNewReferensi = txtReferensi.Text;
                    if (vNewReferensi != vOldReferensi)
                    {
                        SqlCommand check_Referensi = new SqlCommand("SELECT COUNT(*) FROM [tblRef] WHERE ([Referensi] = @NewReferensi)", Conn);
                        check_Referensi.Parameters.AddWithValue("@NewReferensi", vNewReferensi);
                        int ReferensiExist = (int)check_Referensi.ExecuteScalar();

                        if (ReferensiExist > 0)
                        {
                            //MessageBox.Show("Referensi sudah digunakan.");
                            MetroFramework.MetroMessageBox.Show(this, "Referensi sudah digunakan.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        else if (ReferensiExist == 0)
                        {
                            //If reference data not exist in database add new reference
                            Query = "INSERT INTO [dbo].[tblRef] ([Referensi], [CreatedBy], [CreatedDate]) VALUES (";
                            Query += "'" + vNewReferensi + "', ";
                            Query += "'" + ControlMgr.UserId + "', ";
                            Query += "getdate());";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            //delete Old reference
                            Query = "DELETE FROM [dbo].[tblRef] WHERE [Referensi] = '" + vOldReferensi + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else if (vNewReferensi == vOldReferensi)
                    {
                        Query = "update tblRef set Referensi = '" + txtReferensi.Text + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where Referensi = '" + vOldReferensi + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                }
                else if (dgvAttachment.RowCount < 1)
                {
                    //MessageBox.Show("Attachment harus ada.");
                    MetroFramework.MetroMessageBox.Show(this, "Attachment harus ada.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            //insertStatusLog(TransStatus);
            ListMethod.StatusLogCustomer("SOHeader", "SalesOrder", tbxCustID.Text, TransStatus, "", tbxSOID.Text, "", "", "");
                    
        }

        private void btnSave_Click(object sender, EventArgs e)
        {         
            try
            {
                using (scope = new TransactionScope())
                {
                    if (cbRef.Text == "Sales Agreement" && Mode == "Edit")
                        revertSARemainingQty(SOID);

                    if (Validation() == 'X')
                        return;

                    Conn = ConnectionString.GetConnection();
                    #region variable
                    string SalesOrderNo;
                    DateTime OrderDate = dateTimePicker1.Value;
                    string SalesMouNo = tbxMoUID.Text;
                    string SA_SQ_Id = tbxSQID.Text;
                    string CustID = tbxCustID.Text;
                    string CustName = tbxCustName.Text;
                    string CurrencyID = cbCurrency.Text;
                    decimal ExchRate = Convert.ToDecimal(tbxXRate.Text);
                    decimal Total = Convert.ToDecimal(tbxSTotal.Text);
                    decimal Total_Disk = Convert.ToDecimal(tbxGDisc.Text);
                    decimal PPN = Convert.ToDecimal(cbPPN.Text);
                    decimal Total_PPN = Convert.ToDecimal(tbxGPPN.Text);
                    decimal PPH = Convert.ToDecimal(cbPPh.Text);
                    decimal Total_PPH = Convert.ToDecimal(tbxGPPh.Text);
                    decimal Total_Nett = Convert.ToDecimal(tbxGTotal.Text);
                    decimal Total_Bonus = Convert.ToDecimal(tbxGBonus.Text);
                    decimal Total_Cashback = Convert.ToDecimal(tbxGCashback.Text);
                    string TermofPayment = cbToP.Text;
                    Cmd = new SqlCommand("Select [PaymentModeID] from [PaymentMode] where [PaymentModeName] = '" + cbPaymentMode.Text + "'", Conn);
                    string PaymentModeID = Cmd.ExecuteScalar().ToString();
                    string DPType = cbDPType.Text;
                    decimal DPPercent = Convert.ToDecimal(tbxDPPercent.Text);
                    decimal DPAmount = Convert.ToDecimal(tbxDP.Text);
                    DateTime DPDueDate = dtDP.Value;
                    string Notes = tbxNotes.Text;
                    string TransStatus = "";
                    if (tbxRefID.Text == String.Empty)
                    {
                        flag = checkPriceTolerance();
                        if (flag == 'X')
                            TransStatus = "12"; //WAITING FOR APPROVAL
                        else
                            TransStatus = "01"; //TransStatus Preorder
                    }
                    else
                        TransStatus = "09"; //TransStatus Amended – Waiting for Approval
                    DateTime ValidTo = dateTimePicker3.Value;
                    string RefTransId = tbxRefID.Text;
                    string Referensi = txtReferensi.Text;
                    #endregion

                    if (tbxMoUID.Text.Trim() != "")
                    {
                        if (!(ListMethod.checkCustomerMoU(tbxMoUID.Text, CustID, Total_Nett)))
                            return;
                    }
                    else
                    {
                        if (!(ListMethod.checkCreditLimit("Ask", CustID, Total_Nett)))
                            return;
                    }

                    if (Mode == "New")
                    {
                        SaveNew();
                    }
                    else if (Mode != "New")
                    {
                        SaveEdit();
                    }
                    Conn.Close();
                    scope.Complete();
                }
                ModeBeforeEdit();
                GetDataHeader();
                Parent.RefreshGrid();
                MetroFramework.MetroMessageBox.Show(this, tbxSOID.Text + " save success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //catch (Exception ex)
            //{
            //    MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            finally { }
        }

        private string getTransStatus(string SOno)
        {
            string TStatus = "";

            Query = "SELECT TransStatus FROM SalesOrderH where SalesOrderNo = '" + SOno + "'";
             using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                 TStatus = Cmd.ExecuteScalar().ToString();

             return TStatus;
        }

        private void updateSODetailOld(string SalesOrderNo,string RefTransId,string TransStatus)
        {
            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                #region variable
                int SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["SeqNo"].Value == null))
                    SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["SeqNo"].Value);
                string GroupID = dataGridView1.Rows[j].Cells["GroupID"].Value.ToString();
                string SubGroup1ID = dataGridView1.Rows[j].Cells["SubGroup1ID"].Value.ToString();
                string SubGroup2ID = dataGridView1.Rows[j].Cells["SubGroup2ID"].Value.ToString();
                string ItemID = dataGridView1.Rows[j].Cells["ItemID"].Value.ToString();
                string FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();
                string ItemName = dataGridView1.Rows[j].Cells["ItemName"].Value.ToString();
                string DeliveryMethod = dataGridView1.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                decimal LogisticAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["LogisticAmount"].Value == String.Empty || dataGridView1.Rows[j].Cells["LogisticAmount"].Value == null))
                    LogisticAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["LogisticAmount"].Value);
                string LogisticNotes = "";
                if (!(dataGridView1.Rows[j].Cells["LogisticNotes"].Value == null || dataGridView1.Rows[j].Cells["LogisticNotes"].Value == String.Empty))
                    LogisticNotes = dataGridView1.Rows[j].Cells["LogisticNotes"].Value.ToString();

                //DateTime ExpectedDateFrom = ConvertObjectToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value.ToString());
                //DateTime ExpectedDateTo = ConvertObjectToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value.ToString());
                DateTime ExpectedDateFrom = new DateTime(1753, 01, 01);
                if (!(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value.ToString() == String.Empty))
                    ExpectedDateFrom = Convert.ToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value);
                DateTime ExpectedDateTo = new DateTime(1753, 01, 01);
                if (!(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value == String.Empty))
                    ExpectedDateTo = Convert.ToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value);
                decimal Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                string Unit = dataGridView1.Rows[j].Cells["Unit"].Value.ToString();
                decimal Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                string Unit_Alt = dataGridView1.Rows[j].Cells["Unit_Alt"].Value.ToString();
                decimal ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value);
                decimal Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value);
                decimal Price_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value);
                string DiscType = "1";
                if (dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() != "Select")
                {
                    Cmd = new SqlCommand("select [DiskonSchemeID] from [ISBS-NEW4].[dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[j].Cells["DiscType"].Value + "'", Conn);
                    DiscType = Cmd.ExecuteScalar().ToString();
                }
                decimal DiscPercent = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscPercent"].Value == null || dataGridView1.Rows[j].Cells["DiscPercent"].Value == String.Empty))
                    DiscPercent = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscPercent"].Value);
                decimal DiscAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscAmount"].Value == null || dataGridView1.Rows[j].Cells["DiscAmount"].Value == String.Empty))
                    DiscAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscAmount"].Value);
                decimal BonusAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["BonusAmount"].Value);
                decimal CashBackAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["CashBackAmount"].Value);
                decimal SubTotal = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal"].Value);
                decimal SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPN"].Value);
                decimal SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPH"].Value);
                string NotesD = "";
                if (!(dataGridView1.Rows[j].Cells["Notes"].Value == null || dataGridView1.Rows[j].Cells["Notes"].Value == String.Empty))
                    NotesD = dataGridView1.Rows[j].Cells["Notes"].Value.ToString();
                string SA_SQ_IdD = "";
                if (!(dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == null || dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == String.Empty || dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == (object)DBNull.Value))
                    SA_SQ_IdD = dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value.ToString();
                int SA_SQ_SeqNo = 0;
                //HENDRY tambah ""                                
                if (!(dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == (object)DBNull.Value))
                    SA_SQ_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value);
                //HENDRY END
                int RefTrans_SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == (object)DBNull.Value))
                    RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value);
                string Base = "";
                if (!(dataGridView1.Rows[j].Cells["Base"].Value == null || dataGridView1.Rows[j].Cells["Base"].Value == String.Empty || dataGridView1.Rows[j].Cells["Base"].Value == (object)DBNull.Value))
                    Base = dataGridView1.Rows[j].Cells["Base"].Value.ToString();
                decimal RemainingQty = Qty;
                string PLJNo = dataGridView1.Rows[j].Cells["PLJNo"].Value.ToString();
                int PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value);
                decimal PLJPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value);
                #endregion

                Cmd = new SqlCommand("select * from [dbo].[SalesOrderD] where [SalesOrderNo] = '" + SalesOrderNo + "' and [SeqNo] = '" + SeqNo + "'", Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    if (cbRef.Text == "Sales Agreement")
                    {
                        //if (Base == "N")
                        updateSARemainingQty(SOID, SeqNo, SA_SQ_IdD, SA_SQ_SeqNo, Qty, SubTotal);
                    }

                    updateSODetail(GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, RemainingQty, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesD, SA_SQ_IdD, SA_SQ_SeqNo, Base, SalesOrderNo, SeqNo, RefTransId, RefTrans_SeqNo);
                }
                else
                {
                    Query = "select top 1seqno from SalesOrderD where salesorderno = '" + SOID + "' order by seqno desc";
                    Cmd = new SqlCommand(Query, Conn);
                    SeqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;

                    if (cbRef.Text == "Sales Agreement")
                    {
                        //if (Base == "N")
                        updateSARemainingQty(SOID, SeqNo, SA_SQ_IdD, SA_SQ_SeqNo, Qty, SubTotal);
                    }

                    insertSODetail(SalesOrderNo, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesD, SA_SQ_IdD, SA_SQ_SeqNo, RefTransId, RefTrans_SeqNo, Base, RemainingQty, PLJNo, PLJSeqNo, PLJPrice);
                }

                if (TransStatus == "01")
                {
                    if (cbRef.Text == "Sales Agreement")
                        ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_From_SA_Issued_UoM]", "[SO_From_SA_Issued_Alt]", Qty, Qty_Alt);
                    else
                        ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_Preordered_UoM]", "[SO_Preordered_Alt]", Qty, Qty_Alt);
                }
            }
        }

        private void updateSODetailNew(string SalesOrderNo, string RefTransId, string TransStatus)
        {          
            Query = "DELETE FROM [SalesOrderD] WHERE [SalesOrderNo] = '" + SalesOrderNo + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                Cmd.ExecuteNonQuery();

            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                #region variable
                int SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["SeqNo"].Value == null))
                    SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["SeqNo"].Value);
                string GroupID = dataGridView1.Rows[j].Cells["GroupID"].Value.ToString();
                string SubGroup1ID = dataGridView1.Rows[j].Cells["SubGroup1ID"].Value.ToString();
                string SubGroup2ID = dataGridView1.Rows[j].Cells["SubGroup2ID"].Value.ToString();
                string ItemID = dataGridView1.Rows[j].Cells["ItemID"].Value.ToString();
                string FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();
                string ItemName = dataGridView1.Rows[j].Cells["ItemName"].Value.ToString();
                string DeliveryMethod = dataGridView1.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                decimal LogisticAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["LogisticAmount"].Value == String.Empty || dataGridView1.Rows[j].Cells["LogisticAmount"].Value == null))
                    LogisticAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["LogisticAmount"].Value);
                string LogisticNotes = "";
                if (!(dataGridView1.Rows[j].Cells["LogisticNotes"].Value == null || dataGridView1.Rows[j].Cells["LogisticNotes"].Value == String.Empty))
                    LogisticNotes = dataGridView1.Rows[j].Cells["LogisticNotes"].Value.ToString();

                //DateTime ExpectedDateFrom = ConvertObjectToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value.ToString());
                //DateTime ExpectedDateTo = ConvertObjectToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value.ToString());
                DateTime ExpectedDateFrom = new DateTime(1753, 01, 01);
                if (!(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value.ToString() == String.Empty))
                    ExpectedDateFrom = Convert.ToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value);
                DateTime ExpectedDateTo = new DateTime(1753, 01, 01);
                if (!(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value == String.Empty))
                    ExpectedDateTo = Convert.ToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value);
                decimal Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                string Unit = dataGridView1.Rows[j].Cells["Unit"].Value.ToString();
                decimal Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                string Unit_Alt = dataGridView1.Rows[j].Cells["Unit_Alt"].Value.ToString();
                decimal ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value);
                decimal Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value);
                decimal Price_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value);
                string DiscType = "1";
                if (dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() != "Select")
                {
                    Cmd = new SqlCommand("select [DiskonSchemeID] from [ISBS-NEW4].[dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[j].Cells["DiscType"].Value + "'", Conn);
                    DiscType = Cmd.ExecuteScalar().ToString();
                }
                decimal DiscPercent = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscPercent"].Value == null || dataGridView1.Rows[j].Cells["DiscPercent"].Value == String.Empty))
                    DiscPercent = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscPercent"].Value);
                decimal DiscAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscAmount"].Value == null || dataGridView1.Rows[j].Cells["DiscAmount"].Value == String.Empty))
                    DiscAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscAmount"].Value);
                decimal BonusAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["BonusAmount"].Value);
                decimal CashBackAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["CashBackAmount"].Value);
                decimal SubTotal = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal"].Value);
                decimal SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPN"].Value);
                decimal SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPH"].Value);
                string NotesD = "";
                if (!(dataGridView1.Rows[j].Cells["Notes"].Value == null || dataGridView1.Rows[j].Cells["Notes"].Value == String.Empty))
                    NotesD = dataGridView1.Rows[j].Cells["Notes"].Value.ToString();
                string SA_SQ_IdD = "";
                if (!(dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == null || dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == String.Empty || dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value == (object)DBNull.Value))
                    SA_SQ_IdD = dataGridView1.Rows[j].Cells["SA_SQ_Id"].Value.ToString();
                int SA_SQ_SeqNo = 0;
                //HENDRY tambah ""                                
                if (!(dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value == (object)DBNull.Value))
                    SA_SQ_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["SA_SQ_SeqNo"].Value);
                //HENDRY END
                int RefTrans_SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == (object)DBNull.Value))
                    RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value);
                string Base = "";
                if (!(dataGridView1.Rows[j].Cells["Base"].Value == null || dataGridView1.Rows[j].Cells["Base"].Value == String.Empty || dataGridView1.Rows[j].Cells["Base"].Value == (object)DBNull.Value))
                    Base = dataGridView1.Rows[j].Cells["Base"].Value.ToString();
                decimal RemainingQty = Qty;
                string PLJNo = dataGridView1.Rows[j].Cells["PLJNo"].Value.ToString();
                int PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value);
                decimal PLJPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value);
                #endregion

                Query = "select top 1seqno from SalesOrderD where salesorderno = '" + SOID + "' order by seqno desc";
                Cmd = new SqlCommand(Query, Conn);
                SeqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;

                if (cbRef.Text == "Sales Agreement")
                    updateSARemainingQty(SOID, SeqNo, SA_SQ_IdD, SA_SQ_SeqNo, Qty, SubTotal);                

                insertSODetail(SalesOrderNo, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesD, SA_SQ_IdD, SA_SQ_SeqNo, RefTransId, RefTrans_SeqNo, Base, RemainingQty, PLJNo, PLJSeqNo, PLJPrice);

                if (TransStatus == "01")
                {
                    if (cbRef.Text == "Sales Agreement")
                        ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_From_SA_Issued_UoM]", "[SO_From_SA_Issued_Alt]", Qty, Qty_Alt);
                    else
                        ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_Preordered_UoM]", "[SO_Preordered_Alt]", Qty, Qty_Alt);
                }
            }
        }

        private void revertSARemainingQty(string SOID)
        {
            int totalDetail = 0;
            Query = "SELECT COUNT(*) FROM [SalesOrderD] WHERE [SalesOrderNo] = '" + SOID + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                totalDetail = Convert.ToInt32(Cmd.ExecuteScalar());            

            decimal oldSORemainingQty = 0;
            decimal oldSOSubTotal = 0;
            decimal oldSOSubPPN = 0;
            decimal oldSOSubPPH = 0;

            string SASQId = "";
            string SASQSeqNo = "";
            string TransType = "";

            for (int i = 1; i < totalDetail + 1; i++)
            {
                Query = "SELECT * FROM [SalesOrderD] WHERE [SalesOrderNo] = '" + SOID + "' AND SeqNo = '" + i + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    oldSORemainingQty = Convert.ToDecimal(Dr["RemainingQty"]);
                    oldSOSubTotal = Convert.ToDecimal(Dr["SubTotal"]);
                    oldSOSubPPN = Convert.ToDecimal(Dr["SubTotal_PPN"]);
                    oldSOSubPPH = Convert.ToDecimal(Dr["SubTotal_PPH"]);

                    SASQId = Dr["SA_SQ_Id"].ToString();
                    SASQSeqNo = Dr["SA_SQ_SeqNo"].ToString();
                }

                Query = "SELECT TransType from SalesAgreementH WHERE SalesAgreementNo = '" + SASQId + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    TransType = Cmd.ExecuteScalar().ToString();

                if (TransType == "QUANTITY")
                {
                    Query = "UPDATE SalesAgreement_Dtl set [RemainingQty] = RemainingQty + '" + oldSORemainingQty + "' ";
                    Query += "WHERE SalesAgreementNo = '" + SASQId + "' and SeqNo = '" + SASQSeqNo + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();
                }
                else if (TransType == "AMOUNT")
                {
                    decimal OldSOAmount = oldSORemainingQty + oldSOSubPPN + oldSOSubPPH;
                    Query = "UPDATE SalesAgreement_Dtl set [RemainingQty] = RemainingQty + '" + OldSOAmount + "' ";
                    Query += "WHERE SalesAgreementNo = '" + SASQId + "' and SeqNo = '" + SASQSeqNo + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();
                }
            }
        }

        private void updateSARemainingQty(string SOID, int SOSeqNo, string SA_SQ_IdD, int SA_SQ_SeqNo, decimal Qty, decimal SubTotal)
        {
            //GET BASE Y SEQNO
            Query = "select top 1 SeqNo from SalesAgreement_Dtl where SalesAgreementNo = '" + SA_SQ_IdD + "' and Base = 'Y' and SeqNo <= '" + SA_SQ_SeqNo + "' and Deleted = 'N' order by SeqNo desc";
            Cmd = new SqlCommand(Query, Conn);
            int seqNo_BaseY = Convert.ToInt32(Cmd.ExecuteScalar());

            //GET SA REMAINING QTY & SALES TYPE
            string TransType = "";
            decimal remainingQty = 0;
            Query = "select b.RemainingQty, a.TransType from SalesAgreementH a left join SalesAgreement_Dtl b on a.SalesAgreementNo = b.SalesAgreementNo where b.SalesAgreementNo = '" + SA_SQ_IdD + "' and b.SeqNo = '" + seqNo_BaseY + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                remainingQty = Convert.ToDecimal(Dr["RemainingQty"]);
                TransType = Dr["TransType"].ToString();
            }
            Dr.Close();

            //GET OLD SO QTY & REMAINING QTY
            decimal oldSOQty = 0;
            decimal oldRemainingQty = 0;
            decimal oldSOSubTotal = 0;
            Query = "Select Qty, RemainingQty, SubTotal from SalesOrderD where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            if (Dr.HasRows)
            {
                while (Dr.Read())
                {
                    oldSOQty = Convert.ToDecimal(Dr["Qty"]);
                    oldRemainingQty = Convert.ToDecimal(Dr["RemainingQty"]);
                    oldSOSubTotal = Convert.ToDecimal(Dr["SubTotal"]);
                }
                Dr.Close();
            }

            if (TransType == "QUANTITY")
            {
                remainingQty = remainingQty + oldSOQty - Qty;
            }
            else if (TransType == "AMOUNT")
            {
                oldSOSubTotal = oldSOSubTotal + (Convert.ToDecimal(cbPPN.Text) * oldSOSubTotal / 100) + (Convert.ToDecimal(cbPPh.Text) * oldSOSubTotal / 100);
                decimal newSubTotal = SubTotal + (Convert.ToDecimal(cbPPN.Text) * SubTotal / 100) + (Convert.ToDecimal(cbPPh.Text) * SubTotal / 100);
                remainingQty = remainingQty + oldSOSubTotal - newSubTotal;
            }

            //if (TransType == "QUANTITY")
            //    remainingQty = remainingQty - Qty;
            //else if (TransType == "AMOUNT")
            //    remainingQty = remainingQty - (SubTotal + (Convert.ToDecimal(cbPPN.Text) * SubTotal / 100) + (Convert.ToDecimal(cbPPh.Text) * SubTotal / 100));

            Query = "update SalesAgreement_Dtl set remainingQty = '" + remainingQty + "' where SalesAgreementNo = '" + SA_SQ_IdD + "' and SeqNo = '" + seqNo_BaseY + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateSOAmountTable(string SO_Id, decimal SO_PPN_Percent, decimal SO_DP_Percent, decimal SO_Amount, decimal SO_Outstanding_Amount, string Cust_Id)
        {
            Query = "UPDATE [dbo].[SOAmountTable] SET [SO_PPN_Percent] = @SO_PPN_Percent, [SO_DP_Percent] = @SO_DP_Percent, [SO_Amount] = @SO_Amount, [SO_Outstanding_Amount] = @SO_Outstanding_Amount, [Cust_Id] = @Cust_Id WHERE [SO_Id] = @SO_Id";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@SO_Id", SO_Id);
            Cmd.Parameters.AddWithValue("@SO_PPN_Percent", SO_PPN_Percent);
            Cmd.Parameters.AddWithValue("@SO_DP_Percent", SO_DP_Percent);
            Cmd.Parameters.AddWithValue("@SO_Amount", SO_Amount);
            Cmd.Parameters.AddWithValue("@SO_Outstanding_Amount", SO_Outstanding_Amount);
            Cmd.Parameters.AddWithValue("@Cust_Id", Cust_Id);
            Cmd.ExecuteNonQuery();
        }

        private void insertSOAmountTable(string SO_Id, DateTime SO_Date, decimal SO_PPN_Percent, decimal SO_DP_Percent, decimal SO_Amount, decimal SO_Outstanding_Amount, string Cust_Id)
        {
            Query = "INSERT INTO [dbo].[SOAmountTable](SO_Id,SO_Date,SO_PPN_Percent,SO_DP_Percent,SO_Amount,SO_Outstanding_Amount,Cust_Id) VALUES (@SO_Id,@SO_Date,@SO_PPN_Percent,@SO_DP_Percent,@SO_Amount,@SO_Outstanding_Amount,@Cust_Id)";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@SO_Id", SO_Id);
            Cmd.Parameters.AddWithValue("@SO_Date", SO_Date);
            Cmd.Parameters.AddWithValue("@SO_PPN_Percent", SO_PPN_Percent);
            Cmd.Parameters.AddWithValue("@SO_DP_Percent", SO_DP_Percent);
            Cmd.Parameters.AddWithValue("@SO_Amount", SO_Amount);
            Cmd.Parameters.AddWithValue("@SO_Outstanding_Amount", SO_Outstanding_Amount);
            Cmd.Parameters.AddWithValue("@Cust_Id", Cust_Id);
            Cmd.ExecuteNonQuery();
        }

        private void insertSOHeader(string SalesOrderNo, DateTime OrderDate, string SalesMouNo, string SA_SQ_Id, string CustID, string CustName, string CurrencyID, decimal ExchRate, decimal Total, decimal Total_Disk, decimal PPN, decimal Total_PPN, decimal PPH, decimal Total_PPH, decimal Total_Nett, decimal Total_Bonus, decimal Total_Cashback, string TermofPayment, string PaymentModeID, string DPType, decimal DPPercent, decimal DPAmount, DateTime DPDueDate, string Notes, string TransStatus, DateTime ValidTo, string RefTransId, string Referensi)
        {
            Query = "INSERT INTO [dbo].[SalesOrderH] ([SalesOrderNo], [OrderDate], [SalesMouNo], [SA_SQ_Id], [CustID], [CustName], [CurrencyID], [ExchRate], [Total], [Total_Disk], [PPN], [Total_PPN], [PPH], [Total_PPH], [Total_Nett], [Total_Bonus], [Total_Cashback], [TermofPayment], [PaymentModeID], [DPType], [DPPercent], [DPAmount], [DPDueDate], [Notes], [TransStatus], [ValidTo], [RefTransId], [Referensi], [CreatedDate], [CreatedBy], UpdatedDate, UpdatedBy) VALUES (@SalesOrderNo, @OrderDate, @SalesMouNo, @SA_SQ_Id, @CustID, @CustName, @CurrencyID, @ExchRate, @Total, @Total_Disk, @PPN, @Total_PPN, @PPH, @Total_PPH, @Total_Nett, @Total_Bonus, @Total_Cashback, @TermofPayment, @PaymentModeID, @DPType, @DPPercent, @DPAmount, @DPDueDate, @Notes, @TransStatus, @ValidTo, @RefTransId, @Referensi, getdate(), '" + ControlMgr.UserId + "', '1753-01-01', NULL)";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@SalesOrderNo", SalesOrderNo);
            Cmd.Parameters.AddWithValue("@OrderDate", OrderDate);
            Cmd.Parameters.AddWithValue("@SalesMouNo", SalesMouNo);
            Cmd.Parameters.AddWithValue("@SA_SQ_Id", SA_SQ_Id == String.Empty ? (object)DBNull.Value : SA_SQ_Id);
            Cmd.Parameters.AddWithValue("@CustID", CustID);
            Cmd.Parameters.AddWithValue("@CustName", CustName);
            Cmd.Parameters.AddWithValue("@CurrencyID", CurrencyID);
            Cmd.Parameters.AddWithValue("@ExchRate", ExchRate);
            Cmd.Parameters.AddWithValue("@Total", Total);
            Cmd.Parameters.AddWithValue("@Total_Disk", Total_Disk);
            Cmd.Parameters.AddWithValue("@PPN", PPN);
            Cmd.Parameters.AddWithValue("@Total_PPN", Total_PPN);
            Cmd.Parameters.AddWithValue("@PPH", PPH);
            Cmd.Parameters.AddWithValue("@Total_PPH", Total_PPH);
            Cmd.Parameters.AddWithValue("@Total_Nett", Total_Nett);
            Cmd.Parameters.AddWithValue("@Total_Bonus", Total_Bonus);
            Cmd.Parameters.AddWithValue("@Total_Cashback", Total_Cashback);
            Cmd.Parameters.AddWithValue("@TermofPayment", TermofPayment);
            Cmd.Parameters.AddWithValue("@PaymentModeID", PaymentModeID);
            Cmd.Parameters.AddWithValue("@DPType", DPType);
            Cmd.Parameters.AddWithValue("@DPPercent", DPPercent);
            Cmd.Parameters.AddWithValue("@DPAmount", DPAmount);
            Cmd.Parameters.AddWithValue("@DPDueDate", DPType == "N" ? (object)DBNull.Value : DPDueDate);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.Parameters.AddWithValue("@TransStatus", TransStatus);
            Cmd.Parameters.AddWithValue("@ValidTo", ValidTo);
            Cmd.Parameters.AddWithValue("@RefTransId", RefTransId);
            Cmd.Parameters.AddWithValue("@Referensi", Referensi);
            Cmd.ExecuteNonQuery();
        }

        private void insertSODetail(string SalesOrderNo, int SeqNo, string GroupID, string SubGroup1ID, string SubGroup2ID, string ItemID, string FullItemID, string ItemName, string DeliveryMethod, decimal LogisticAmount, string LogisticNotes, DateTime ExpectedDateFrom, DateTime ExpectedDateTo, decimal Qty, string Unit, decimal Qty_Alt, string Unit_Alt, decimal ConvertionRatio, decimal Price, decimal Price_Alt, string DiscType, decimal DiscPercent, decimal DiscAmount, decimal BonusAmount, decimal CashBackAmount, decimal SubTotal, decimal SubTotal_PPN, decimal SubTotal_PPH, string Notes, string SA_SQ_Id, int SA_SQ_SeqNo, string RefTransId, int RefTrans_SeqNo, string Base, decimal RemainingQty, string PLJNo, int PLJSeqNo, decimal PLJPrice)
        {
            Query = "INSERT INTO [dbo].[SalesOrderD] ([SalesOrderNo], [SeqNo], [GroupID], [SubGroup1ID], [SubGroup2ID], [ItemID], [FullItemID], [ItemName], [DeliveryMethod], [LogisticAmount], [LogisticNotes], [ExpectedDateFrom], [ExpectedDateTo], [Qty], [Unit], [Qty_Alt], [Unit_Alt], [ConvertionRatio], [Price], [Price_Alt], [DiscType], [DiscPercent], [DiscAmount], [BonusAmount], [CashBackAmount], [SubTotal], [SubTotal_PPN], [SubTotal_PPH], [Notes], CreatedDate, CreatedBy,[RemainingQty], [SA_SQ_Id], [SA_SQ_SeqNo], RefTransId, RefTrans_SeqNo, Base,[LockStatus],[LockID],[LockQty],PLJNo, PLJSeqNo, PLJPrice) VALUES (@SalesOrderNo, @SeqNo, @GroupID, @SubGroup1ID, @SubGroup2ID, @ItemID, @FullItemID, @ItemName, @DeliveryMethod, @LogisticAmount, @LogisticNotes, @ExpectedDateFrom, @ExpectedDateTo, @Qty, @Unit, @Qty_Alt, @Unit_Alt, @ConvertionRatio, @Price, @Price_Alt, @DiscType, @DiscPercent, @DiscAmount, @BonusAmount, @CashBackAmount, @SubTotal, @SubTotal_PPN, @SubTotal_PPH, @Notes, getdate(), '" + ControlMgr.UserId + "',@RemainingQty, @SA_SQ_Id, @SA_SQ_SeqNo, @RefTransId, @RefTrans_SeqNo, @Base, NULL, NULL, NULL,@PLJNo, @PLJSeqNo, @PLJPrice)";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@SalesOrderNo", SalesOrderNo);
            Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
            Cmd.Parameters.AddWithValue("@GroupID", GroupID);
            Cmd.Parameters.AddWithValue("@SubGroup1ID", SubGroup1ID);
            Cmd.Parameters.AddWithValue("@SubGroup2ID", SubGroup2ID);
            Cmd.Parameters.AddWithValue("@ItemID", ItemID);
            Cmd.Parameters.AddWithValue("@FullItemID", FullItemID);
            Cmd.Parameters.AddWithValue("@ItemName", ItemName);
            Cmd.Parameters.AddWithValue("@DeliveryMethod", DeliveryMethod);
            Cmd.Parameters.AddWithValue("@LogisticAmount", LogisticAmount);
            Cmd.Parameters.AddWithValue("@LogisticNotes", LogisticNotes);
            //Cmd.Parameters.AddWithValue("@ExpectedDateFrom", Base == "N" ? (object)DBNull.Value : ExpectedDateFrom);
            //Cmd.Parameters.AddWithValue("@ExpectedDateTo", Base == "N" ? (object)DBNull.Value : ExpectedDateTo);
            Cmd.Parameters.AddWithValue("@ExpectedDateFrom", ExpectedDateFrom);
            Cmd.Parameters.AddWithValue("@ExpectedDateTo", ExpectedDateTo);
            Cmd.Parameters.AddWithValue("@Qty", Qty);
            Cmd.Parameters.AddWithValue("@Unit", Unit);
            Cmd.Parameters.AddWithValue("@Qty_Alt", Qty_Alt);
            Cmd.Parameters.AddWithValue("@Unit_Alt", Unit_Alt);
            Cmd.Parameters.AddWithValue("@ConvertionRatio", ConvertionRatio);
            Cmd.Parameters.AddWithValue("@Price", Price);
            Cmd.Parameters.AddWithValue("@Price_Alt", Price_Alt);
            Cmd.Parameters.AddWithValue("@DiscType", DiscType);
            Cmd.Parameters.AddWithValue("@DiscPercent", DiscPercent);
            Cmd.Parameters.AddWithValue("@DiscAmount", DiscAmount);
            Cmd.Parameters.AddWithValue("@BonusAmount", BonusAmount);
            Cmd.Parameters.AddWithValue("@CashBackAmount", CashBackAmount);
            Cmd.Parameters.AddWithValue("@SubTotal", SubTotal);
            Cmd.Parameters.AddWithValue("@SubTotal_PPN", SubTotal_PPN);
            Cmd.Parameters.AddWithValue("@SubTotal_PPH", SubTotal_PPH);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.Parameters.AddWithValue("@RemainingQty", RemainingQty);
            Cmd.Parameters.AddWithValue("@SA_SQ_Id", SA_SQ_Id == String.Empty ? (object)DBNull.Value : SA_SQ_Id);
            Cmd.Parameters.AddWithValue("@SA_SQ_SeqNo", SA_SQ_Id == String.Empty ? (object)DBNull.Value : SA_SQ_SeqNo);
            Cmd.Parameters.AddWithValue("@RefTransId", RefTransId == String.Empty ? (object)DBNull.Value : RefTransId);
            Cmd.Parameters.AddWithValue("@RefTrans_SeqNo", RefTransId == String.Empty ? (object)DBNull.Value : RefTrans_SeqNo);
            Cmd.Parameters.AddWithValue("@Base", Base == String.Empty ? (object)DBNull.Value : Base);
            Cmd.Parameters.AddWithValue("@PLJNo", PLJNo);
            Cmd.Parameters.AddWithValue("@PLJSeqNo", PLJSeqNo);
            Cmd.Parameters.AddWithValue("@PLJPrice", PLJPrice);
            Cmd.ExecuteNonQuery();
        }

        private void updateAmendedForm()
        {
            //IF AMEND UPDATE OLD SO STATS
            if (tbxRefID.Text != String.Empty)
            {
                Query = "update SalesOrderH set TransStatus = '04', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + tbxRefID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                ListMethod.insertLogTable("E", "SalesOrder_LogTable", tbxRefID.Text, "", "SalesOrder", "04");
                ListMethod.StatusLogCustomer("SOHeader", "SalesOrder", tbxCustID.Text, "04", "", tbxRefID.Text, "", "", "");

                //ADJUST QTY
                Query = "select * from InventLockTable where RefTransId = '" + tbxRefID.Text + "' or RefTrans2Id = (select SA_SQ_Id from SalesOrderH where SalesOrderNo = '" + tbxRefID.Text + "')";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    //DEDUCT INVENT LOCK TABLE
                    Query = "INSERT INTO [dbo].[InventLockTable] ([RefTransType] ,[RefTransId] ,[RefTrans_SeqNo] ,[RefTrans2Id] ,[RefTrans2_SeqNo] ,[FullItemId] ,[SiteId] ,[Ratio] ,[Lock_Qty] ,[Unit] ,[Lock_Qty_Alt] ,[Unit_Alt] ,[CreatedDate] ,[CreatedBy] ,[UpdatedDate] ,[UpdatedBy]) VALUES ";
                    Query += "('SALES ORDER', '" + Dr["RefTransId"] + "', '" + Dr["RefTrans_SeqNo"] + "', '" + Dr["RefTrans2Id"] + "', '" + Dr["RefTrans2_SeqNo"] + "', '" + Dr["FullItemId"] + "', '" + Dr["SiteId"] + "', '" + Dr["Ratio"] + "', '" + Convert.ToInt32(Dr["Lock_Qty"]) * -1 + "', '" + Dr["Unit"] + "', '" + Convert.ToInt32(Dr["Lock_Qty_Alt"]) * -1 + "', '" + Dr["Unit_Alt"] + "', getdate(), '" + ControlMgr.UserId + "', '1753-01-01', NULL)";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    //INVENT ON HAND QTY
                    decimal Available_For_Sale_UoM = 0;
                    decimal Available_For_Sale_Reserved_UoM = 0;
                    decimal Available_For_Sale_Alt = 0;
                    decimal Available_For_Sale_Reserved_Alt = 0;
                    Query = "select Available_For_Sale_UoM, Available_For_Sale_Reserved_UoM, Available_For_Sale_Alt, Available_For_Sale_Reserved_Alt from Invent_OnHand_Qty where FullItemId = '" + Dr["FullItemId"] + "' and InventSiteId = '" + Dr["SiteId"] + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader Dr2 = Cmd.ExecuteReader();
                    while (Dr2.Read())
                    {
                        Available_For_Sale_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_UoM"]);
                        Available_For_Sale_Reserved_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_UoM"]);
                        Available_For_Sale_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Alt"]);
                        Available_For_Sale_Reserved_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Alt"]);
                    }
                    Dr2.Close();

                    Available_For_Sale_UoM += Convert.ToDecimal(Dr["Lock_Qty"]);
                    Available_For_Sale_Alt += Convert.ToDecimal(Dr["Lock_Qty_Alt"]);
                    Available_For_Sale_Reserved_UoM -= Convert.ToDecimal(Dr["Lock_Qty"]);
                    Available_For_Sale_Reserved_Alt -= Convert.ToDecimal(Dr["Lock_Qty_Alt"]);

                    Query = "update Invent_OnHand_Qty set Available_For_Sale_UoM = '" + Available_For_Sale_UoM + "'";
                    Query += ", Available_For_Sale_Reserved_UoM = '" + Available_For_Sale_Reserved_UoM + "'";
                    Query += ", Available_For_Sale_Alt = '" + Available_For_Sale_Alt + "'";
                    Query += ", Available_For_Sale_Reserved_Alt = '" + Available_For_Sale_Reserved_Alt + "'";
                    Query += " where FullItemId = '" + Dr["FullItemId"] + "' and InventSiteId = '" + Dr["SiteId"] + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    //INVENT TRANS
                    Cmd = new SqlCommand("select * from InventTrans where TransId = '" + Dr["RefTransId"] + "' and SeqNo = '" + Dr["RefTrans_SeqNo"] + "'", Conn);
                    Dr2 = Cmd.ExecuteReader();
                    while (Dr2.Read())
                    {
                        Query = "INSERT INTO [dbo].[InventTrans] ([GroupId],[SubGroupId],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteId],[TransId],[SeqNo],[TransDate],[Ref_TransId],[Ref_TransDate],[Ref_Trans_SeqNo],[AccountId],[AccountName],[Available_UoM],[Available_Alt],[Available_Amount],[Available_For_Sale_UoM],[Available_For_Sale_Alt],[Available_For_Sale_Amount],[Available_For_Sale_Reserved_UoM],[Available_For_Sale_Reserved_Alt],[Available_For_Sale_Reserved_Amount],[Notes]) VALUES (@GroupId ,@SubGroupId ,@SubGroup2Id ,@ItemId ,@FullItemId ,@ItemName ,@InventSiteId ,@TransId ,@SeqNo ,@TransDate ,@Ref_TransId ,@Ref_TransDate ,@Ref_Trans_SeqNo ,@AccountId ,@AccountName ,@Available_UoM ,@Available_Alt ,@Available_Amount ,@Available_For_Sale_UoM ,@Available_For_Sale_Alt ,@Available_For_Sale_Amount ,@Available_For_Sale_Reserved_UoM ,@Available_For_Sale_Reserved_Alt ,@Available_For_Sale_Reserved_Amount ,@Notes)";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.AddWithValue("@GroupId", Dr2["GroupId"]);
                        Cmd.Parameters.AddWithValue("@SubGroupId", Dr2["SubGroupId"]);
                        Cmd.Parameters.AddWithValue("@SubGroup2Id", Dr2["SubGroup2Id"]);
                        Cmd.Parameters.AddWithValue("@ItemId", Dr2["ItemId"]);
                        Cmd.Parameters.AddWithValue("@FullItemId", Dr2["FullItemId"]);
                        Cmd.Parameters.AddWithValue("@ItemName", Dr2["ItemName"]);
                        Cmd.Parameters.AddWithValue("@InventSiteId", Dr2["InventSiteId"]);
                        Cmd.Parameters.AddWithValue("@TransId", Dr2["TransId"]);
                        Cmd.Parameters.AddWithValue("@SeqNo", Dr2["SeqNo"]);
                        Cmd.Parameters.AddWithValue("@TransDate", Dr2["TransDate"]);
                        Cmd.Parameters.AddWithValue("@Ref_TransId", Dr2["Ref_TransId"]);
                        Cmd.Parameters.AddWithValue("@Ref_TransDate", Dr2["Ref_TransDate"]);
                        Cmd.Parameters.AddWithValue("@Ref_Trans_SeqNo", Dr2["Ref_Trans_SeqNo"]);
                        Cmd.Parameters.AddWithValue("@AccountId", Dr2["AccountId"]);
                        Cmd.Parameters.AddWithValue("@AccountName", Dr2["AccountName"]);
                        Cmd.Parameters.AddWithValue("@Available_UoM", Convert.ToDecimal(Dr2["Available_UoM"]) * -1);
                        Cmd.Parameters.AddWithValue("@Available_Alt", Convert.ToDecimal(Dr2["Available_Alt"]) * -1);
                        Cmd.Parameters.AddWithValue("@Available_Amount", Convert.ToDecimal(Dr2["Available_Amount"]) * -1);
                        Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", Convert.ToDecimal(Dr2["Available_For_Sale_UoM"]) * -1);
                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", Convert.ToDecimal(Dr2["Available_For_Sale_Alt"]) * -1);
                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", Convert.ToDecimal(Dr2["Available_For_Sale_Amount"]) * -1);
                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_UoM", Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_UoM"]) * -1);
                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Alt", Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Alt"]) * -1);
                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Amount", Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Amount"]) * -1);
                        Cmd.Parameters.AddWithValue("@Notes", Dr2["Notes"]);
                    }
                    Dr2.Close();
                    Cmd.ExecuteNonQuery();
                }
                Dr.Close();

            }

            if (cbRef.Text == "Sales Quotation")
            {
                //UPDATE SQ STATUS TO SO IN PROGRESS
                Query = "update SalesQuotationH set TransStatus = '09', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesQuotationNo = '" + tbxSQID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                ListMethod.insertLogTableFollowParentAct("SalesQuotation_LogTable", tbxSQID.Text, "", "SalesQuotation", "09");
                ListMethod.StatusLogCustomer("SQHeader2", "SalesQuotation", tbxCustID.Text, "09", "", tbxSQID.Text, "", "", "");

            }
            else if (cbRef.Text == "Sales Agreement")
            {
                flag = '\0';
                Query = "select count(*) from SalesAgreement_Dtl where SalesAgreementNo = '" + tbxSQID.Text + "' and RemainingQty > 0 ";
                Cmd = new SqlCommand(Query, Conn);
                //UPDATE SA STATUS 
                if (Convert.ToInt32(Cmd.ExecuteScalar()) == 0)
                {
                    Query = "update SalesAgreementH set TransStatus = '07', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + tbxSQID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    ListMethod.insertLogTableFollowParentAct("SalesAgreement_LogTable", tbxSQID.Text, "", "SA", "07");
                    ListMethod.StatusLogCustomer("SAHeader", "SA", tbxCustID.Text, "07", "", tbxSQID.Text, "", "", "");

                }
                else
                {
                    Query = "update SalesAgreementH set TransStatus = '06', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + tbxSQID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    ListMethod.insertLogTableFollowParentAct("SalesAgreement_LogTable", tbxSQID.Text, "", "SA", "06");
                    ListMethod.StatusLogCustomer("SAHeader", "SA", tbxCustID.Text, "06", "", tbxSQID.Text, "", "", "");

                }
            }
        }


        private void updateSOHeader(string CurrencyID, decimal ExchRate, decimal Total, decimal Total_Disk, decimal PPN, decimal Total_PPN, decimal PPH, decimal Total_PPH, decimal Total_Nett, decimal Total_Bonus, decimal Total_Cashback, string TermofPayment, string PaymentModeID, string DPType, decimal DPPercent, decimal DPAmount, DateTime DPDueDate, string Notes, DateTime ValidTo, string Referensi, string TransStatus, string SalesOrderNo)
        {
            Query = "update [dbo].[SalesOrderH] SET [CurrencyID] = '" + CurrencyID + "',[ExchRate] = '" + ExchRate + "',[Total] = '" + Total + "',";
            Query += "[Total_Disk] = '" + Total_Disk + "',[PPN] = '" + PPN + "',[Total_PPN] = '" + Total_PPN + "',[PPH] = '" + PPH + "',[Total_PPH] = '" + Total_PPH + "',";
            Query += "[Total_Nett] = '" + Total_Nett + "',[Total_Bonus] = '" + Total_Bonus + "',[Total_Cashback] = '" + Total_Cashback + "',";
            Query += "[TermofPayment] = '" + TermofPayment + "',[PaymentModeID] = @PaymentModeID,[DPType] = '" + DPType + "', DPPercent = '" + DPPercent + "',";
            Query += "[DPDueDate] = @DPDueDate,[Notes] = @Notes,ValidTo = @ValidTo,[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "', TransStatus = '" + TransStatus + "',[Referensi] = '" + Referensi + "' ";
            Query += "WHERE [SalesOrderNo] = '" + SalesOrderNo + "'; ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.Parameters.AddWithValue("@DPDueDate", DPType == "N" ? (object)DBNull.Value : DPDueDate);
            Cmd.Parameters.AddWithValue("@ValidTo", ValidTo);
            Cmd.Parameters.AddWithValue("@PaymentModeID", PaymentModeID);
            Cmd.ExecuteNonQuery();
        }

        private void updateSODetail(string GroupID, string SubGroup1ID, string SubGroup2ID, string ItemID, string FullItemID, string ItemName, string DeliveryMethod, decimal LogisticAmount, string LogisticNotes, DateTime ExpectedDateFrom, DateTime ExpectedDateTo, decimal RemainingQty, decimal Qty, string Unit, decimal Qty_Alt, string Unit_Alt, decimal ConvertionRatio, decimal Price, decimal Price_Alt, string DiscType, decimal DiscPercent, decimal DiscAmount, decimal BonusAmount, decimal CashBackAmount, decimal SubTotal, decimal SubTotal_PPN, decimal SubTotal_PPH, string Notes, string SA_SQ_Id, int SA_SQ_SeqNo, string Base, string SalesOrderNo, int SeqNo, string RefTransId, int RefTrans_SeqNo)
        {
            decimal oldSORemainingQty = 0;
            decimal oldSOQty = 0;
            Query = "Select Qty, RemainingQty from salesorderD where salesorderno = '" + SOID + "' and SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                oldSORemainingQty = Convert.ToDecimal(Dr["RemainingQty"]);
                oldSOQty = Convert.ToDecimal(Dr["Qty"]);
            }
            Dr.Close();

            if (oldSOQty > Qty)
                RemainingQty = oldSORemainingQty - (oldSOQty - Qty);
            else if (oldSOQty < Qty)
                RemainingQty = oldSORemainingQty + (Qty - oldSOQty);
            else
                RemainingQty = oldSORemainingQty;

            Query = "UPDATE [dbo].[SalesOrderD] SET [GroupID] = '" + GroupID + "'";
            Query += ",[SubGroup1ID] = '" + SubGroup1ID + "'";
            Query += ",[SubGroup2ID] = '" + SubGroup2ID + "'";
            Query += ",[ItemID] = '" + ItemID + "'";
            Query += ",[FullItemID] = '" + FullItemID + "'";
            Query += ",[ItemName] = '" + ItemName + "'";
            Query += ",[DeliveryMethod] = '" + DeliveryMethod + "'";
            Query += ",[LogisticAmount] = '" + LogisticAmount + "'";
            Query += ",[LogisticNotes] = @LogisticNotes";
            Query += ",[ExpectedDateFrom] = @ExpectedDateFrom";
            Query += ",[ExpectedDateTo] = @ExpectedDateTo";
            Query += ",[RemainingQty] = '" + RemainingQty + "'";
            Query += ",[Qty] = '" + Qty + "'";
            Query += ",[Unit] = '" + Unit + "'";
            Query += ",[Qty_Alt] = '" + Qty_Alt + "'";
            Query += ",[Unit_Alt] = '" + Unit_Alt + "'";
            Query += ",[ConvertionRatio] = '" + ConvertionRatio + "'";
            Query += ",[Price] = '" + Price + "'";
            Query += ",[Price_Alt] = '" + Price_Alt + "'";
            Query += ",[DiscType] = @DiscType";
            Query += ",[DiscPercent] = '" + DiscPercent + "'";
            Query += ",[DiscAmount] = '" + DiscAmount + "'";
            Query += ",[BonusAmount] = '" + BonusAmount + "'";
            Query += ",[CashBackAmount] = '" + CashBackAmount + "'";
            Query += ",[SubTotal] = '" + SubTotal + "'";
            Query += ",[SubTotal_PPN] = '" + SubTotal_PPN + "'";
            Query += ",[SubTotal_PPH] = '" + SubTotal_PPH + "'";
            Query += ",[Notes] = @Notes";
            Query += ",[UpdatedDate] = getdate()";
            Query += ",[UpdatedBy] = '" + ControlMgr.UserId + "'";
            Query += ", SA_SQ_Id = @SA_SQ_Id";
            Query += ", SA_SQ_SeqNo = @SA_SQ_SeqNo";
            Query += ", Base = @Base";
            Query += ", RefTransId = @RefTransId";
            Query += ", RefTrans_SeqNo = @RefTrans_SeqNo ";
            Query += "WHERE [SalesOrderNo] = '" + SalesOrderNo + "' ";
            Query += "and [SeqNo] = '" + SeqNo + "'; ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@LogisticNotes", LogisticNotes);
            Cmd.Parameters.AddWithValue("@ExpectedDateFrom", ExpectedDateFrom);
            Cmd.Parameters.AddWithValue("@ExpectedDateTo", ExpectedDateTo);
            Cmd.Parameters.AddWithValue("@DiscType", DiscType);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.Parameters.AddWithValue("@SA_SQ_Id", SA_SQ_Id == String.Empty ? (object)DBNull.Value : SA_SQ_Id);
            Cmd.Parameters.AddWithValue("@SA_SQ_SeqNo", SA_SQ_Id == String.Empty ? (object)DBNull.Value : SA_SQ_SeqNo);
            Cmd.Parameters.AddWithValue("@RefTransId", RefTransId == String.Empty ? (object)DBNull.Value : RefTransId);
            Cmd.Parameters.AddWithValue("@RefTrans_SeqNo", RefTransId == String.Empty ? (object)DBNull.Value : RefTrans_SeqNo);
            Cmd.Parameters.AddWithValue("@Base", Base == String.Empty ? (object)DBNull.Value : Base);
            Cmd.ExecuteNonQuery();
        }

        //START STEVEN EDIT
        public void SaveDgvAttachmentData()
        {
            for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                Query += "( 'SalesOrderH', '" + tbxSOID.Text + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString();
                Query += "',@binaryValue";
                Query += ");";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }
        }
        //END STEVEN EDIT

        decimal price, qty, STotal, LogisticAmount, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, STotal_PPN, STotal_PPH, GTotal;
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (tbxSQID.Text == String.Empty)
            //{
            if (tableCols[e.ColumnIndex] == "Qty")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == null)
                    dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value = "0";

                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value != null && Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value) != 0)
                    dataGridView1.Rows[e.RowIndex].Cells["Qty_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) / Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value);
            }
            if (tableCols[e.ColumnIndex] == "Price")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["Price"].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells["Price"].Value == null)
                    dataGridView1.Rows[e.RowIndex].Cells["Price"].Value = "0";
                if (dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value == null)
                    dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value = "0";

                if (dataGridView1.Rows[e.RowIndex].Cells["Price"].Value != null && dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value != null && Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value) != 0)
                    dataGridView1.Rows[e.RowIndex].Cells["Price_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["ConvertionRatio"].Value);
            }
            populateGVData();
            //}
        }

        private void populateGVData()
        {
            //if (tbxSQID.Text == String.Empty)
            //{
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

                if (dataGridView1.Rows[i].Cells["DiscAmount"].Value != null)
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

                STotal = price * qty;
                dataGridView1.Rows[i].Cells["SubTotal"].Value = STotal;


                if (Mode != "BeforeEdit")
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

                if (cbPPN.Text != "Select")
                    STotal_PPN = (STotal - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value))* Convert.ToDecimal(cbPPN.Text) / 100;
                else
                    STotal_PPN = 0;
                dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = STotal_PPN;

                if (cbPPh.Text != "Select")
                    STotal_PPH = (STotal - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPh.Text) / 100;
                else
                    STotal_PPH = 0;
                dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = STotal_PPH;
            }

            tbxSTotal.Text = "0"; tbxGPPN.Text = "0"; tbxGPPh.Text = "0"; tbxGBonus.Text = "0"; tbxGCashback.Text = "0"; tbxGDisc.Text = "0"; tbxGLog.Text = "0";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                tbxSTotal.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value) + Convert.ToDecimal(tbxSTotal.Text)).ToString()));
                tbxGPPN.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value) + Convert.ToDecimal(tbxGPPN.Text)).ToString()));
                tbxGPPh.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value) + Convert.ToDecimal(tbxGPPh.Text)).ToString()));

                //if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value) > 0)
                //    MessageBox.Show(dataGridView1.Rows[i].Cells["DiscAmount"].Value.ToString());

                tbxGDisc.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value) + Convert.ToDecimal(tbxGDisc.Text)).ToString()));
                tbxGLog.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value) + Convert.ToDecimal(tbxGLog.Text)).ToString()));
                tbxGBonus.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["BonusAmount"].Value) + Convert.ToDecimal(tbxGBonus.Text)).ToString()));
                tbxGCashback.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["CashBackAmount"].Value) + Convert.ToDecimal(tbxGCashback.Text)).ToString()));
            }

            tbxGTotal.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(tbxSTotal.Text) + Convert.ToDecimal(tbxGPPN.Text) + Convert.ToDecimal(tbxGPPh.Text) - Convert.ToDecimal(tbxGDisc.Text)).ToString()));
            //}
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["ItemName"].Frozen = true;

            dataGridView1.Columns["GroupID"].Visible = false;
            dataGridView1.Columns["SubGroup1ID"].Visible = false;
            dataGridView1.Columns["SubGroup2ID"].Visible = false;
            dataGridView1.Columns["ItemID"].Visible = false;
            dataGridView1.Columns["Qty_Alt"].Visible = false;
            dataGridView1.Columns["Unit_Alt"].Visible = false;
            dataGridView1.Columns["Price_Alt"].Visible = false;
            dataGridView1.Columns["ConvertionRatio"].Visible = false;
            dataGridView1.Columns["PLJNo"].Visible = false;
            dataGridView1.Columns["PLJSeqNo"].Visible = false;
            dataGridView1.Columns["PLJPrice"].Visible = false;
            //if (cbRef.Text == "Select" || cbRef.Text == "Sales Quotation")
            //    dataGridView1.Columns["Base"].Visible = false;
            //else
            //    dataGridView1.Columns["Base"].Visible = true;
            dataGridView1.Columns["Base"].Visible = false;
            if (tbxSQID.Text != String.Empty)
            {
                dataGridView1.Columns["DeliveryMethod"].Visible = false;
                dataGridView1.Columns["ExpectedDateFrom"].Visible = false;
                dataGridView1.Columns["ExpectedDateTo"].Visible = false;
                dataGridView1.Columns["SubTotal"].Visible = false;
                dataGridView1.Columns["SubTotal_PPN"].Visible = false;
                //dataGridView1.Columns["SubTotal_PPH"].Visible = false;
                dataGridView1.Columns["LogisticAmount"].Visible = false;
                dataGridView1.Columns["LogisticNotes"].Visible = false;
                dataGridView1.Columns["DiscType"].Visible = false;
                dataGridView1.Columns["DiscPercent"].Visible = false;
                dataGridView1.Columns["DiscAmount"].Visible = false;
                dataGridView1.Columns["BonusAmount"].Visible = false;
                dataGridView1.Columns["CashBackAmount"].Visible = false;
                dataGridView1.Columns["SA_SQ_Id"].Visible = false;
                dataGridView1.Columns["SA_SQ_SeqNo"].Visible = false;
                dataGridView1.Columns["RefTransId"].Visible = false;
                dataGridView1.Columns["RefTrans_SeqNo"].Visible = false;
                dataGridView1.Columns["SalesOrderNo"].Visible = false;
                dataGridView1.Columns["SeqNo"].Visible = false;

                if (Mode == "New")
                {
                    dataGridView1.Columns["RemainingQty"].Visible = false;
                }
                else if (Mode == "BeforeEdit")
                {
                    dataGridView1.Columns["RemainingQty"].Visible = true;
                }
            }
            else if (tbxSQID.Text == String.Empty)
            {
                if (Mode == "New")
                {
                    dataGridView1.Columns["DeliveryMethod"].Visible = true;
                    dataGridView1.Columns["ExpectedDateFrom"].Visible = true;
                    dataGridView1.Columns["ExpectedDateTo"].Visible = true;
                    dataGridView1.Columns["SubTotal"].Visible = true;
                    dataGridView1.Columns["SubTotal_PPN"].Visible = true;
                    //dataGridView1.Columns["SubTotal_PPH"].Visible = true;
                    dataGridView1.Columns["LogisticAmount"].Visible = true;
                    dataGridView1.Columns["LogisticNotes"].Visible = true;
                    dataGridView1.Columns["DiscType"].Visible = true;
                    dataGridView1.Columns["DiscPercent"].Visible = true;
                    dataGridView1.Columns["DiscAmount"].Visible = true;
                    dataGridView1.Columns["BonusAmount"].Visible = true;
                    dataGridView1.Columns["CashBackAmount"].Visible = true;

                    dataGridView1.Columns["RemainingQty"].Visible = false;
                }
                else if (Mode == "BeforeEdit")
                {

                }
                dataGridView1.Columns["SA_SQ_Id"].Visible = false;
                dataGridView1.Columns["SA_SQ_SeqNo"].Visible = false;
                dataGridView1.Columns["RefTransId"].Visible = false;
                dataGridView1.Columns["RefTrans_SeqNo"].Visible = false;
                dataGridView1.Columns["SalesOrderNo"].Visible = false;
                dataGridView1.Columns["SeqNo"].Visible = false;
            }
            else if (Mode == "Edit")
            {
                dataGridView1.Columns["SA_SQ_Id"].Visible = true;
                dataGridView1.Columns["SA_SQ_SeqNo"].Visible = true;
                dataGridView1.Columns["RemainingQty"].Visible = true;
                dataGridView1.Columns["RefTransId"].Visible = true;
                dataGridView1.Columns["RefTrans_SeqNo"].Visible = true;
                dataGridView1.Columns["SalesOrderNo"].Visible = true;
                dataGridView1.Columns["SeqNo"].Visible = true;
            }

            dataGridView1.Columns["SubTotal_PPH"].Visible = false;

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Qty_Alt") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ConvertionRatio") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DiscPercent"))
            {
                if (e.Value == "" || e.Value == null || e.Value == (object)DBNull.Value)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Amount") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Price") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Total"))
            {
                if (e.Value == "" || e.Value == null || e.Value == (object)DBNull.Value)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

       
        private void cbPPN_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (cbPPN.Text != "Select")
                    dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = Convert.ToString((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPN.Text) / 100);
                else
                    dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = "0";
            }
        }

        private void cbPPh_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (cbPPh.Text != "Select")
                    dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = Convert.ToString((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPh.Text) / 100);
                else
                    dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = "0";
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Mode = "Edit";
                GetDataHeader();
                ModeEdit();
            }
            else
            {
                //MessageBox.Show(ControlMgr.PermissionDenied);
                MetroFramework.MetroMessageBox.Show(this, ControlMgr.PermissionDenied, "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //end
            //steven edit s
            vOldReferensi = txtReferensi.Text;
            //steven edit e
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Sales Admin" && ControlMgr.GroupName != "Administrator")
            {
                MetroFramework.MetroMessageBox.Show(this, "You don't have Permission..", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (tbxMoUID.Text.Trim() != "")
                {
                    if (!(ListMethod.checkCustomerMoU(tbxMoUID.Text, tbxCustID.Text, Convert.ToDecimal(tbxGTotal.Text))))
                        return;

                    Query = "UPDATE [CustMou_Dtl] SET [Remaining_Amount] = [Remaining_Amount] - '" + Convert.ToDecimal(tbxGTotal.Text) + "'";
                    Query += "WHERE [MouNo] = '" + tbxMoUID.Text + "' AND [CustID] = '" + tbxCustID.Text + "'";

                    Query += "UPDATE [dbo].[CustTable] SET [Sisa_Limit_MoU] = [Sisa_Limit_MoU] - '" + Convert.ToDecimal(tbxGTotal.Text) + "' WHERE [CustId] = '" + tbxCustID.Text + "'";

                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();
                }
                else
                {
                    if (!(ListMethod.checkCreditLimit("Stop", tbxCustID.Text, Convert.ToDecimal(tbxGTotal.Text))))
                        return;

                    Query = "UPDATE [dbo].[CustTable] SET [Sisa_Limit_Total] = [Sisa_Limit_Total] - '" + Convert.ToDecimal(tbxGTotal.Text) + "' WHERE [CustId] = '" + tbxCustID.Text + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();
                }

                Query = "update [dbo].[SalesOrderH] set [TransStatus] = '03', [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + tbxSOID.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                ListMethod.insertLogTableFollowParentAct("SalesOrder_LogTable", tbxSOID.Text, "", "SalesOrder", "03");
                ListMethod.StatusLogCustomer("SOHeader", "SalesOrder", tbxCustID.Text, "03", "", tbxSOID.Text, "", "", "");

                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    decimal Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                    decimal Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                    string FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();

                    ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_Confirmed_Outstanding_UoM]", "[SO_Confirmed_Outstanding_Alt]", Qty, Qty_Alt);

                    if (cbRef.Text == "Sales Agreement")
                        ListMethod.updateInventUomAlt("Decrease", "[Invent_Sales_Qty]", FullItemID, "[SO_From_SA_Issued_UoM]", "[SO_From_SA_Issued_Alt]", Qty, Qty_Alt);
                    else
                        ListMethod.updateInventUomAlt("Decrease", "[Invent_Sales_Qty]", FullItemID, "[SO_Preordered_UoM]", "[SO_Preordered_Alt]", Qty, Qty_Alt);
                }
                
                Trans.Commit();
                Conn.Close();
                MetroFramework.MetroMessageBox.Show(this, tbxSOID.Text + " confirm!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnConfirm.Enabled = false; btnEdit.Enabled = false;
                ModeBeforeEdit();
                GetDataHeader();
                Parent.RefreshGrid();                
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                MetroFramework.MetroMessageBox.Show(this, "Confirm Error!\r\n Error message : " + ex, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            DialogResult dialogResult = MessageBox.Show("Apakah anda ingin lock stock?", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
                loadSOReserved();
        }

        private void btnSCust_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                //if (tbxCustID.Text != oldCustID)
                MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change Customer!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //tbxCustID.Text = oldCustID;
            }
            else
            {
                tbxCustID.Text = "";
                tbxCustName.Text = "";
                SearchV2 f = new SearchV2();
                f.SetMode("No");
                if (tbxMoUID.Text == String.Empty)
                    f.SetSchemaTable("dbo", "CustTable", "", "a.*", "CustTable a");
                else
                    f.SetSchemaTable("dbo", "CustTable", "and CustId in (select b.CustID from CustMouH a left join CustMou_Dtl b on a.MouNo = b.MouNo where a.MouNo = '" + tbxMoUID.Text + "')", "a.*", "CustTable a");
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

                /*SearchV2 f = new SearchV2();
                f.SetMode("No");
                if (tbxMoUID.Text != String.Empty)
                {
                    string CustID = String.Empty;
                    for (int i = 0; i < dataGridView2.RowCount; i++)
                    {
                        if (i >= 1)
                            CustID += ",";
                        CustID += "'" + dataGridView2.Rows[i].Cells["CustID"].Value.ToString() + "'";
                    }
                    if (CustID == String.Empty)
                        CustID = "''";
                    f.SetSchemaTable("dbo", "CustMou_Dtl", "and a.[MouNo] = '" + tbxMoUID.Text + "' and a.CustID not in (" + CustID + ")", "a.*", "CustMou_Dtl a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        string dataString = "";
                        string dataString2 = "";
                        for (int i = 0; i < SearchV2.data.Count; i++)
                        {
                            if (i >= 1)
                                dataString += ", ";
                            dataString += "'" + SearchV2.data[i] + "'";
                        }
                        for (int i = 0; i < SearchV2.data2.Count; i++)
                        {
                            if (i >= 1)
                                dataString2 += ", ";
                            dataString2 += "'" + SearchV2.data2[i] + "'";
                        }
                        Conn = ConnectionString.GetConnection();
                        Query = "select a.CustID, a.CustName, a.DP_Required from [ISBS-NEW4].[dbo].[CustTable] as a left join CustMou_Dtl as b on a.CustID = b.CustID where b.MouNo in (" + dataString + ") ";
                        if (SearchV2.data2.Count != 0)
                            Query += "and b.SeqNo in (" + dataString2 + ") ";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            tbxCustID.Text = Dr["CustID"].ToString();
                            tbxCustName.Text = Dr["CustName"].ToString();
                            cbDPType.Text = Dr["DP_Required"].ToString();
                            if (Dr["DP_Required"].ToString() == "Y")
                                cbDPType.Enabled = false;
                            else
                                cbDPType.Enabled = true;
                        }
                        Dr.Close();
                        Conn.Close();
                    }
                }
                else
                {
                    f.SetSchemaTable("dbo", "CustTable", "", "a.*", "CustTable a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        Conn = ConnectionString.GetConnection();
                        Query = "select CustID, CustName, DP_Required from CustTable where CustID = '" + SearchV2.data[0] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            tbxCustID.Text = Dr["CustID"].ToString();
                            tbxCustName.Text = Dr["CustName"].ToString();
                            cbDPType.Text = Dr["DP_Required"].ToString();
                            if (Dr["DP_Required"].ToString() == "Y")
                                cbDPType.Enabled = false;
                            else
                                cbDPType.Enabled = true;
                        }
                        Dr.Close();
                        Conn.Close();
                    }
                }*/
            }
        }

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

        private void cbDPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDPType.Text == "Y")
            {
                dtDP.Enabled = true;
                cbDPType.BackColor = Color.Empty;
                tbxDP.Enabled = true; tbxDPPercent.Enabled = true;
            }
            else
            {
                dtDP.Enabled = false;
                if (cbDPType.Text == "N")
                {
                    cbDPType.BackColor = Color.Empty;
                    tbxDP.Enabled = false; tbxDPPercent.Enabled = false;
                    tbxDP.Text = "0"; tbxDPPercent.Text = "0";
                }
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView2.Columns["ItemName"].Frozen = true;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

            if (cbRef.Text == "Sales Agreement")
                dataGridView2.Columns["Base"].Visible = true;
            else
                dataGridView2.Columns["Base"].Visible = false;

            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("Qty") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("Qty_Alt") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("ConvertionRatio") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("DiscPercent"))
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

        private void tbxDP_Leave(object sender, EventArgs e)
        {
            if (tbxDP.Text == String.Empty)
                tbxDP.Text = "0.00";
            if (Convert.ToDecimal(tbxGTotal.Text) != 0)
                tbxDPPercent.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(Convert.ToDecimal(tbxDP.Text) / Convert.ToDecimal(tbxGTotal.Text) * 100)).ToString()));
        }

        private void btnAmend_Click(object sender, EventArgs e)
        {
            //CHECK IF ITEM IS RESERVED FOR AMENDED SO
            Conn = ConnectionString.GetConnection();
            flag = '\0';
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                Query = "select Lock_Qty from InventLockTable where RefTransId = '" + dataGridView1.Rows[i].Cells["SalesOrderNo"].Value + "' and RefTrans_SeqNo = '" + dataGridView1.Rows[i].Cells["SeqNo"].Value + "'";
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
            tbxRefID.Text = tbxSOID.Text;
            tbxSOID.Text = "";
            //SOID = "";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells["RefTransId"].Value = dataGridView1.Rows[i].Cells["SalesOrderNo"].Value;
                dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value = dataGridView1.Rows[i].Cells["SeqNo"].Value;
                dataGridView1.Rows[i].Cells["SalesOrderNo"].Value = "";
                dataGridView1.Rows[i].Cells["SeqNo"].Value = "";
            }
            Mode = "New";
            btnAmend.Visible = false;
            dataGridView1.Columns["Price"].ReadOnly = true;
            dataGridView1.Columns["Price"].DefaultCellStyle.BackColor = Color.LightGray;
        }

        private void tbxXRate_Leave(object sender, EventArgs e)
        {
            if (tbxXRate.Text == String.Empty)
                tbxXRate.Text = "0";
            tbxXRate.Text = string.Format("{0:#,0.00}", double.Parse(tbxXRate.Text));
        }

        private void tbxSTotal_TextChanged(object sender, EventArgs e)
        {
            tbxSTotal.Text = string.Format("{0:#,0.00}", double.Parse(tbxSTotal.Text));
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
            tbxDP.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDPPercent.Text) / 100).ToString()));
        }

        private void tbxGDisc_TextChanged(object sender, EventArgs e)
        {
            tbxGDisc.Text = string.Format("{0:#,0.00}", double.Parse(tbxGDisc.Text));
        }

        private void tbxGLog_TextChanged(object sender, EventArgs e)
        {
            tbxGLog.Text = string.Format("{0:#,0.00}", double.Parse(tbxGLog.Text));
        }

        private void tbxGBonus_TextChanged(object sender, EventArgs e)
        {
            tbxGBonus.Text = string.Format("{0:#,0.00}", double.Parse(tbxGBonus.Text));
        }

        private void tbxGCashback_TextChanged(object sender, EventArgs e)
        {
            tbxGCashback.Text = string.Format("{0:#,0.00}", double.Parse(tbxGCashback.Text));
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
                    //dtp.Value = Convert.ToDateTime(dataGridView1.CurrentCell.Value);
                    if (dataGridView1.CurrentCell.Value.GetType() == typeof(DateTime))
                        dtp.Value = Convert.ToDateTime(dataGridView1.CurrentCell.Value);
                    else
                        dtp.Value = DateTime.ParseExact(dataGridView1.CurrentCell.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //dtp.Value = DateTime.ParseExact(dataGridView1.CurrentCell.Value.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    //}
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
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

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell.Value = dtp.Text;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
            {
                if (dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
                {
                    //dataGridView1.CurrentCell.Value = dtp.Value.Date.ToString("dd-MM-yyyy");
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
            //_dateTimePicker.Visible = false;
            dtp.Visible = false;
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
                                MetroFramework.MetroMessageBox.Show(this, "Today rate is not available! Please insert manually!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                tbxXRate.Text = "1.00";
                            }
                        }
                        Dr2.Close();
                        Conn2.Close();
                        tbxXRate.Enabled = true;
                    }
                }
            }
            //if (cbCurrency.Text == "IDR")
            //{
            //    tbxXRate.Text = "1.00";
            //    tbxXRate.Enabled = false;
            //}
            //else
            //{
            //    tbxXRate.Enabled = true;
            //}
        }

        //STEVEN EDIT (ADD UPLOAD + REFERENSI)
        private void cbByPhone_CheckedChanged(object sender, EventArgs e)
        {
            if (cbByPhone.Checked == true)
            {
                txtReferensi.Text = "By Phone";
                txtReferensi.Enabled = false;
            }
            else
            {
                txtReferensi.Text = "";
                txtReferensi.Enabled = true;
            }
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
                    //MessageBox.Show("File tidak ada dalam database / belum di masukkan.");
                    MetroFramework.MetroMessageBox.Show(this, "File tidak ada dalam database / belum di masukkan.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    //MessageBox.Show("Data tersimpan!");
                    MetroFramework.MetroMessageBox.Show(this, "Data tersimpan!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                //MessageBox.Show("Silahkan pilih data untuk didownload");
                MetroFramework.MetroMessageBox.Show(this, "Silahkan pilih data untuk didownload", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                //MessageBox.Show("Silahkan pilih data untuk dihapus");
                MetroFramework.MetroMessageBox.Show(this, "Silahkan pilih data untuk dihapus", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void txtReferensi_TextChanged_1(object sender, EventArgs e)
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
        //STEVEN EDIT END

        private void cbRef_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbRef.Text == "Sales Quotation")
            {
                lblSASQId.Text = "Sales Quotation ID";
                tabPage2.Text = "Detail SQ";
                btnSSQ.Enabled = true;
                btnAddItem.Enabled = false;
                btnDeleteItem.Enabled = false;
            }
            else if (cbRef.Text == "Sales Agreement")
            {
                lblSASQId.Text = "Sales Agreement ID";
                tabPage2.Text = "Detail SA";
                btnSSQ.Enabled = true;
                btnAddItem.Enabled = false;
                btnDeleteItem.Enabled = false;
            }
            else
            {
                lblSASQId.Text = "";
                tbxSQID.Text = "";
                btnSSQ.Enabled = false;
                btnAddItem.Enabled = true;
                btnDeleteItem.Enabled = true;
            }
            ResetForm();

            cbCurrency.Enabled = true;
            cbToP.Enabled = true;
            cbPaymentMode.Enabled = true;
            btnSCust.Enabled = true;

            cbPPh.Enabled = true;
            cbPPN.Enabled = true;
        }

        private void cbRef_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnSMoU_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                //if (tbxCustID.Text != oldCustID)
                MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change MoU!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //tbxCustID.Text = oldCustID;
            }
            else
            {
                tbxMoUID.Text = "";
                tbxCustID.Text = "";
                tbxCustName.Text = "";
                SearchV2 f = new SearchV2();
                f.SetMode("No");
                f.SetSchemaTable("dbo", "CustMouH", "and ValidTo >= DATEADD(day,-1,GETDATE())", "a.*", "CustMouH a");
                f.ShowDialog();

                if (SearchV2.data.Count != 0)
                {
                    tbxMoUID.Text = SearchV2.data[0];
                    SearchV2 f2 = new SearchV2();
                    f2.SetMode("No");
                    f2.SetSchemaTable("dbo", "CustTable", "and CustId in (select b.CustID from CustMouH a left join CustMou_Dtl b on a.MouNo = b.MouNo where a.MouNo = '" + tbxMoUID.Text + "')", "a.*", "CustTable a");
                    f2.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        tbxCustID.Text = SearchV2.data[0];
                        tbxCustName.Text = SearchV2.data[1];
                        setDefaultValue();

                        Conn = ConnectionString.GetConnection();
                        Query = "select DP_Required from CustTable where CustId = '" + tbxCustID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        cbDPType.Text = Cmd.ExecuteScalar().ToString();
                        Conn.Close();
                        if (cbDPType.Text == "Y")
                            cbDPType.Enabled = false;
                        else
                            cbDPType.Enabled = true;
                    }
                }
            }

            /*SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "CustMouH", "and a.ValidTo >= DATEADD(day,-1,GETDATE())", "a.*", "CustMouH a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                tbxMoUID.Text = SearchV2.data[0];
                tbxCustID.Text = "";
                tbxCustName.Text = "";
            }*/
        }

        private void tbxDPPercent_Leave(object sender, EventArgs e)
        {
            if (tbxDPPercent.Text == String.Empty)
                tbxDPPercent.Text = "0.00";
            decimal dpAmount = Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDPPercent.Text) / 100;
            tbxDP.Text = string.Format("{0:#,0.00}", double.Parse(dpAmount.ToString()));
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            using (scope = new TransactionScope())
            {
                Conn = ConnectionString.GetConnection();
                Query = "update SalesOrderH set TransStatus = '10', updatedDate = getdate(), updatedby = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + tbxSOID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                ListMethod.insertLogTableFollowParentAct("SalesOrder_LogTable", tbxSOID.Text, "", "SalesOrder", "10");
                ListMethod.StatusLogCustomer("SOHeader", "SalesOrder", tbxCustID.Text, "10", "", tbxSOID.Text, "", "", "");

                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    decimal Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                    decimal Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                    string FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();

                    if (cbRef.Text == "Sales Agreement")
                        ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_From_SA_Issued_UoM]", "[SO_From_SA_Issued_Alt]", Qty, Qty_Alt);
                    else
                        ListMethod.updateInventUomAlt("Increase", "[Invent_Sales_Qty]", FullItemID, "[SO_Preordered_UoM]", "[SO_Preordered_Alt]", Qty, Qty_Alt);
                }

                Conn.Close();
                scope.Complete();
            }
            ModeBeforeEdit();
            GetDataHeader();
            Parent.RefreshGrid();
            //MessageBox.Show("Approval Success..");
            MetroFramework.MetroMessageBox.Show(this, "Approval Success..", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            using (scope = new TransactionScope())
            {
                Conn = ConnectionString.GetConnection();
                Query = "update SalesOrderH set TransStatus = '11', updatedDate = getdate(), updatedby = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + tbxSOID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                Conn.Close();
                ListMethod.insertLogTableFollowParentAct("SalesOrder_LogTable", tbxSOID.Text, "", "SalesOrder", "11");
                ListMethod.StatusLogCustomer("SOHeader", "SalesOrder", tbxCustID.Text, "11", "", tbxSOID.Text, "", "", "");

                scope.Complete();
            }
            ModeBeforeEdit();
            GetDataHeader();
            Parent.RefreshGrid();
            //MessageBox.Show("Reject Success..");
            MetroFramework.MetroMessageBox.Show(this, "Reject Success..", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cbToP_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true; //hendry
        }

        private void cbByPhone_CheckedChanged_1(object sender, EventArgs e)
        {
            if (cbByPhone.Checked == true)
            {
                txtReferensi.Text = "By Phone";
                txtReferensi.Enabled = false;
            }
            else
            {
                txtReferensi.Text = "";
                txtReferensi.Enabled = true;
            }
        }

        private void btnReserved_Click(object sender, EventArgs e)
        {
            loadSOReserved();
        }

        private void loadSOReserved()
        {
            SOReserved f = new SOReserved();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                f.SOID.Add(tbxSOID.Text);
                //f.SOID.Add(tbxRefID.Text);
                f.SO_SeqNo.Add(Convert.ToInt32(dataGridView1.Rows[i].Cells["SeqNo"].Value));
                f.SO_Qty.Add(Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value));
                //f.rowNum(Convert.ToInt32(i + 1));
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
                    createLabel(tbxCustID, lblCustomer, gbMain, "string");
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2)
            {
                btnAddItem.Enabled = false;
                btnDeleteItem.Enabled = false;
            }
            else
            {
                if (Mode != "BeforeEdit")
                {
                    //if (Mode == "New" || Mode == "Edit")
                    //{
                    //    if (tbxSQID.Text == String.Empty)
                    //    {
                    btnAddItem.Enabled = true;
                    btnDeleteItem.Enabled = true;
                    //    }
                }
            }
        }

        string oldCustID;
        private void tbxCustID_TextChanged(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (tbxCustID.Text != oldCustID)
                    MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change Customer!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbxCustID.Text = oldCustID;
            }
            else
                oldCustID = tbxCustID.Text;
        }
        //tia edit
        //klik kanan
        PopUp.CustomerID.Customer Cust = null;
        PopUp.FullItemId.FullItemId FID = null;
        Sales.MoUCustomer.HeaderMoUCustomer MOUID = null;
        Sales.SalesAgreement.SAHeader SAG = null;
        Sales.SalesQuotation.SQHeader2 SQU = null;

        Purchase.PurchaseRequisition.HeaderPR ParentToPR;
        Purchase.PurchaseRequisitionApproval.HeaderPRApproval ParentToPRA;
        Purchase.ReceiptOrder.HeaderReceiptOrder ParentToRO;
        Sales.DeliveryOrder.DOHeader ParentToDO;
        Sales.NotaReturJual.NRJHeader ParentToNRJ;

        Sales.NotaReturJual.NRJApproval ParentToNRJA;
        AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice ParentToCI;
        PopUp.CustomerID.Customer ParentToPopUpCust;
        TaskList.GlobalTasklist Parent2 = new TaskList.GlobalTasklist();

        public void SetParent2(TaskList.GlobalTasklist Tl)
        {
            Parent2 = Tl;
        }
        public void ParentRefreshGrid(Purchase.PurchaseRequisition.HeaderPR pr)
        {
            ParentToPR = pr;
        }
        public void ParentRefreshGrid2(Purchase.PurchaseRequisitionApproval.HeaderPRApproval pra)
        {
            ParentToPRA = pra;
        }
        public void ParentRefreshGrid3(Purchase.ReceiptOrder.HeaderReceiptOrder ro)
        {
            ParentToRO = ro;
        }

        public void ParentRefreshGrid4(Sales.DeliveryOrder.DOHeader deo)
        {
            ParentToDO = deo;
        }
        public void ParentRefreshGrid5(Sales.NotaReturJual.NRJHeader nrj)
        {
            ParentToNRJ = nrj;
        }
        public void ParentRefreshGrid6(Sales.NotaReturJual.NRJApproval nrja)
        {
            ParentToNRJA = nrja;
        }
        public void ParentRefreshGrid7(AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice ci)
        {
            ParentToCI = ci;
        }

        public void ParentRefreshGrid8(PopUp.CustomerID.Customer pc)
        {
            ParentToPopUpCust = pc;
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            {
                //if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                //{
                //    PopUp.Stock.Stock f = new PopUp.Stock.Stock();
                //    itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                //    f.Show();
                //}
                //if (Mode != "BeforeEdit")
                //{
                //    //if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "LockID")
                //    //{
                //    //    if (dataGridView1.Rows[e.RowIndex].Cells["LockStatus"].Value.ToString() != "Select")
                //    //    {
                //    //        SearchV2 f = new SearchV2();
                //    //        f.SetMode("No");
                //    //        string lockStatus = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["LockStatus"].Value.ToString();
                //    //        if (lockStatus == "Warehouse") { f.SetSchemaTable("dbo", "InventSite", "", "a.*", "InventSite a"); }
                //    //        else if (lockStatus == "PO") { f.SetSchemaTable("dbo", "PurchH", "and a.TransStatus = '01'", "a.*", "PurchH a"); }
                //    //        else if (lockStatus == "PA") { f.SetSchemaTable("dbo", "PurchAgreementH", "", "a.*", "PurchAgreementH a"); }
                //    //        else if (lockStatus == "PR") { f.SetSchemaTable("dbo", "PurchRequisitionH", "", "a.*", "PurchRequisitionH a"); }
                //    //        f.ShowDialog();
                //    //        if (SearchV2.data.Count != 0)
                //    //            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["LockID"].Value = SearchV2.data[0];
                //    //    }
                //    //}
                //    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Qty")
                //    {
                //        SearchV2 f = new SearchV2();
                //        f.SetMode("No");
                //        f.SetSchemaTable("dbo", "Invent_OnHand_Qty", "and a.FullItemID = '" + dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'", "a.*", "Invent_OnHand_Qty a");
                //        f.ShowDialog();
                //    }
                //    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Price")
                //    {
                //        //SearchV2 f = new SearchV2();
                //        //f.SetMode("No");
                //        //f.SetSchemaTable("dbo", "SalesPriceListDtl", "and FullItemID = '" + dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'");
                //        //f.ShowDialog();
                //        SearchV2 f = new SearchV2();
                //        f.SetMode("No");
                //        string where = "and a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and a.PriceType >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and b.Active = '1' and b.TransStatus = '03' and a.FullItemID = '" + dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'";
                //        f.SetSchemaTable("dbo", "Pricelist_Dtl", where, "a.*", "Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo");
                //        f.ShowDialog();
                //        if (SearchV2.data.Count != 0)
                //        {
                //            Conn = ConnectionString.GetConnection();
                //            Query = "select Price from Pricelist_Dtl where PricelistNo = '" + SearchV2.data[1] + "' and SeqNo = '" + SearchV2.data[2] + "'";
                //            Cmd = new SqlCommand(Query, Conn);
                //            decimal price = Convert.ToDecimal(Cmd.ExecuteScalar());
                //            Conn.Close();
                //            dataGridView1.Rows[e.RowIndex].Cells["Price"].Value = price;
                //            dataGridView1.Rows[e.RowIndex].Cells["PLJNo"].Value = SearchV2.data[1];
                //            dataGridView1.Rows[e.RowIndex].Cells["PLJSeqNo"].Value = SearchV2.data[2];
                //            dataGridView1.Rows[e.RowIndex].Cells["PLJPrice"].Value = price;
                //        }
                //    }
                //}
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

        private void tbxMoUID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MOUID == null || MOUID.Text == "")
                {
                    tbxMoUID.Enabled = true;
                    MOUID = new Sales.MoUCustomer.HeaderMoUCustomer();
                    MOUID.SetMode("PopUp", tbxMoUID.Text);
                    MOUID.ParentRefreshGrid2(this);
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

        private void tbxSQID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (cbRef.Text == "Sales Agreement")
                {
                    if (SAG == null || SAG.Text == "")
                    {
                        tbxSQID.Enabled = true;
                        SAG = new Sales.SalesAgreement.SAHeader();
                        SAG.SetMode("PopUp", tbxSQID.Text);
                        SAG.ParentRefreshGrid(this);
                        SAG.Show();
                    }
                    else if (CheckOpened(SAG.Name))
                    {
                        SAG.WindowState = FormWindowState.Normal;
                        SAG.Show();
                        SAG.Focus();
                    }
                }
                else if (cbRef.Text=="Sales Quotation")
                {
                    if (SQU == null || SQU.Text == "")
                    {
                        tbxSQID.Enabled = true;
                        SQU = new Sales.SalesQuotation.SQHeader2();
                        SQU.SetMode("PopUp", tbxSQID.Text);
                        SQU.ParentRefreshGrid(this);
                        SQU.Show();
                    }
                    else if (CheckOpened(SAG.Name))
                    {
                        SQU.WindowState = FormWindowState.Normal;
                        SQU.Show();
                        SQU.Focus();
                    }   
                }
            }
        }
        //fungsi untuk klik kanan ketika WB, Kerani dan Site Manager
        public void HidePrice() 
        {
            if (ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI" || ControlMgr.GroupName == "SITE MANAGER")
            {
                tbxSTotal.Visible = false;
                tbxGPPN.Visible = false;
                tbxGDisc.Visible = false;
                tbxGTotal.Visible = false;
                tbxGPPh.Visible = false;
                tbxGBonus.Visible = false;
                tbxGCashback.Visible = false;
                tbxGLog.Visible = false;
                cbPPh.Visible = false;
                cbPPN.Visible = false;
                cbCurrency.Visible = false;
                tbxXRate.Visible = false;
                tbxDPPercent.Visible = false;
                tbxDP.Visible = false;
               //dgv so detail
                dataGridView1.Columns["Price"].Visible = false;
                dataGridView1.Columns["Price_Alt"].Visible = false;
                dataGridView1.Columns["SubTotal"].Visible = false;
                dataGridView1.Columns["SubTotal_PPN"].Visible = false;
                dataGridView1.Columns["SubTotal_PPH"].Visible = false;
                dataGridView1.Columns["LogisticAmount"].Visible = false;
                dataGridView1.Columns["DiscPercent"].Visible = false;
                dataGridView1.Columns["DiscAmount"].Visible = false;
                dataGridView1.Columns["BonusAmount"].Visible = false;
                dataGridView1.Columns["CashBackAmount"].Visible = false;
                dataGridView1.Columns["PLJPrice"].Visible = false;
               //detail reference
                //dataGridView2.Columns["Price"].Visible = false;
                //dataGridView2.Columns["Price_Alt"].Visible = false;
                //dataGridView2.Columns["SubTotal"].Visible = false;
                //dataGridView2.Columns["SubTotal_PPN"].Visible = false;
               // dataGridView2.Columns["SubTotal_PPH"].Visible = false;
                //dataGridView2.Columns["LogisticAmount"].Visible = false;
                //dataGridView2.Columns["DiscPercent"].Visible = false;
               // dataGridView2.Columns["DiscAmount"].Visible = false;
               // dataGridView2.Columns["BonusAmount"].Visible = false;
                //dataGridView2.Columns["CashBackAmount"].Visible = false;
                //dataGridView2.Columns["PLJPrice"].Visible = false;
                //dataGridView2.Columns[""].Visible=false;
                
            }
        }

        //end

        //Created by Thaddaeus, 13 Sept2018
        private bool CreditLimit(SqlConnection Conn)
        {
            decimal TotalNett = Convert.ToDecimal(tbxGTotal.Text);
            bool status = false;

            Query = "SELECT [Limit_Total], Sisa_Limit_Total,[Limit_Per_PO],Limit_Temp FROM [dbo].[CustTable] WHERE [CustId] = '" + tbxCustID.Text + "'";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    decimal Limit = Convert.ToDecimal(Dr["Limit_Temp"]) + Convert.ToDecimal(Dr["Limit_Total"]);
                    if (Limit < (Convert.ToDecimal(Dr["Sisa_Limit_Total"]) + TotalNett))
                    {
                        status = true;
                    }
                    if (Convert.ToDecimal(Dr["Limit_Per_PO"]) < TotalNett)
                    {
                        status = true;
                    }
                }
                Dr.Close();
            }
            return status;
        }

        private bool CreditLimit(SqlConnection Conn, SqlTransaction Trans)
        {
            decimal TotalNett = Convert.ToDecimal(tbxGTotal.Text);
            bool status = false;

            Query = "SELECT [Limit_Total], Sisa_Limit_Total,[Limit_Per_PO],Limit_Temp FROM [dbo].[CustTable] WHERE [CustId] = '" + tbxCustID.Text + "'";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    decimal Limit = Convert.ToDecimal(Dr["Limit_Temp"]) + Convert.ToDecimal(Dr["Limit_Total"]);
                    if (Limit < (Convert.ToDecimal(Dr["Sisa_Limit_Total"]) + TotalNett))
                    {
                        status = true;
                    }
                    if (Convert.ToDecimal(Dr["Limit_Per_PO"]) < TotalNett)
                    {
                        status = true;
                    }
                }
                Dr.Close();
            }
            if (status == false)
            {
                Query = "UPDATE [dbo].[CustTable] SET [Sisa_Limit_Total] += " + TotalNett + " WHERE [CustId] = '" + tbxCustID.Text + "'";
                using (Cmd = new SqlCommand(Query, Conn,Trans))
                {
                    Cmd.ExecuteNonQuery();
                }
            }
            return status;
        }
        //end======================================================================================

        //private void CreditLimit(SqlConnection Con)
        //{
        //    decimal PaymentAmount = Convert.ToDecimal(tbxGTotal.Text);
        //    decimal OldPaymentAmount = 0;
        //    string OldCust = "";
        //    Query = "SELECT Total_Nett,CustID  FROM  [dbo].[SalesOrderH] WHERE [SalesOrderNo] = '" + tbxSOID.Text + "' ";
        //    using (Cmd = new SqlCommand(Query, Con))
        //    {
        //        Dr = Cmd.ExecuteReader();
        //        if (Dr.HasRows)
        //        {
        //            while (Dr.Read())
        //            {
        //                OldPaymentAmount = Dr[0] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr[0]);
        //                OldCust = Dr[1] == System.DBNull.Value ? "" : Dr[1].ToString();
        //            }
        //        }
        //        Dr.Close();
        //    }
        //    if (OldPaymentAmount != 0)
        //    {
        //        Query = "UPDATE [dbo].[CustTable] SET [Sisa_Limit_Total] = Sisa_Limit_Total -" + OldPaymentAmount + " WHERE [CustId] = '" + oldCustID + "' ";
        //        using (Cmd = new SqlCommand(Query, Con))
        //        {
        //            Cmd.ExecuteNonQuery();
        //        }
        //    }

        //    Query = "UPDATE [dbo].[CustTable] SET [Sisa_Limit_Total] = Sisa_Limit_Total +" + PaymentAmount + " WHERE [CustId] = '" + tbxCustID.Text + "' ";
        //    using (Cmd = new SqlCommand(Query, Con))
        //    {
        //        Cmd.ExecuteNonQuery();
        //    }
        //}

        //private void checkIfItemStillExist(string SOID)
        //{
        //    string where = "";
        //    for (int i = 0; i < dataGridView1.RowCount; i++)
        //    {
        //        if (dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() != String.Empty)
        //        {
        //            if (i == 0)
        //                where = " and SeqNo not in (";
        //            else if (i >= 1)
        //                where += ",";

        //            flag = 'X';
        //            where += "'" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
        //        }

        //        if (i == dataGridView1.RowCount - 1)
        //            where += ")";
        //    }

        //    //string where = " and SeqNo not in (";
        //    //for (int i = 0; i < dataGridView1.RowCount; i++)
        //    //{
        //    //    if (dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() != String.Empty)
        //    //    {
        //    //        if (i >= 1)
        //    //            where += ",";

        //    //        flag = 'X';
        //    //        where += "'" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
        //    //    }
        //    //}
        //    //where += ")";

        //    //UPDATE DETAIL STATUS TO CLOSED IF GV ITEM != DB ITEM
        //    Query = "update SalesOrderD set Deleted = 'Y', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + SOID + "'" + where + "";
        //    Cmd = new SqlCommand(Query, Conn);
        //    Cmd.ExecuteNonQuery();
        //}

    }
}
