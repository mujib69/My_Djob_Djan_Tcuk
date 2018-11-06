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

//BY: HC
namespace ISBS_New.Sales.SalesQuotation
{
    public partial class SQHeader : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
        private SqlConnection Conn2;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        /**********SQL*********/

        /**********STANDARD*********/
        private string Mode;
        private string SQID;
        /**********STANDARD*********/

        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "No", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "RefTransId", "RefTrans_SeqNo", "Base", "Qty", "Unit", "Price", "Qty_Alt", "Unit_Alt", "Price_Alt", "ConvertionRatio", "DeliveryMethod", "ExpectedDateFrom", "ExpectedDateTo", "SubTotal", "SubTotal_PPN", "SubTotal_PPH", "LogisticAmount", "LogisticNotes", "DiscType", "DiscPercent", "DiscAmount", "BonusAmount", "CashBackAmount", "Notes", "GelombangId", "BracketId", "Gelombang_Price", "GelombangSeqNo_Base" };

        string[] tableCols2 = new string[] { "No", "CustID", "CustName" };
        /*********datagridview cols name*********/

        //NUMBER BASED ON SALESPRICELIST DAYS
        int[] priceListDays = new int[] { 0, 2, 3, 7, 14, 21, 30, 40, 45, 60, 75, 90, 120, 150, 180 };

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        bool check; //label
        /*private char flag;*/
        private string msg; //Validation
        /*********VALIDATION*********/

        //GV POP UP 
        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        DataGridViewComboBoxCell cell; //COMBOBOX CELLVALUE
        private SqlDataReader Dr2; //COMBOBOX CELLVALUE

        /*********PARENT*********/
        SQInq Parent = new SQInq();
        public void SetParent(SQInq F) { Parent = F; }

        DateTimePicker dtp = new DateTimePicker(); //DATE IN GRIDVIEW1 

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public SQHeader()
        {
            InitializeComponent();
        }
        private void SQHeader_Load(object sender, EventArgs e)
        {
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
            Cmd = new SqlCommand("select CurrencyID from [ISBS-NEW4].[dbo].[CurrencyTable]", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbCurrency.Items.Add(Dr[0]);
            }
            Dr.Close();

            cbPaymentMode.Items.Clear();
            cbPaymentMode.Items.Add("Select");
            Cmd = new SqlCommand("select [PaymentModeName] from [ISBS-NEW4].[dbo].[PaymentMode]", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbPaymentMode.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();

            if (Mode == "New")
            {

                ModeNew();

                cbCurrency.SelectedText = "IDR";
                cbPaymentMode.SelectedIndex = 0;
                cbDPType.SelectedIndex = 1;
                cbPPh.SelectedIndex = 0;
                cbPPN.SelectedIndex = 1;
                cbType.SelectedIndex = 0;
            }
            else if (Mode == "BeforeEdit")
            {
                GetDataHeader();
                GetDataHeader2();
                ModeBeforeEdit();
            }
            else if (Mode == "Edit")
            {
                GetDataHeader();
                GetDataHeader2();
                ModeEdit();
            }
        }

        public void GetDataHeader()
        {
            Conn = ConnectionString.GetConnection();
            if (Mode == "New")
            {
                dataGridView1.AllowUserToAddRows = false;
                if (dataGridView1.RowCount - 1 <= 0)
                {
                    dataGridView1.ColumnCount = tableCols.Length;
                    for (int i = 0; i < tableCols.Length; i++)
                    {
                        dataGridView1.Columns[i].Name = tableCols[i];
                    }
                }
                if (SearchV2.data.Count != 0)
                {
                    if (cbType.Text == "FIX")
                    {
                        string dataString = ""; ;
                        for (int i = 0; i < SearchV2.data.Count; i++)
                        {
                            if (i >= 1)
                                dataString += ", ";
                            dataString += "'" + SearchV2.data[i] + "'";
                        }

                        Query = "Select [GroupID], [SubGroup1ID], [SubGroup2ID], [ItemID], [FullItemID], [ItemDeskripsi], [UoM], [UoMAlt] from [ISBS-NEW4].[dbo].[InventTable] where [FullItemID] in (" + dataString + "); ";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            Query = "select [Ratio] from [ISBS-NEW4].[dbo].[InventConversion] where FullItemID = '" + Dr[4].ToString() + "'";
                            Cmd = new SqlCommand(Query, Conn);

                            this.dataGridView1.Rows.Add(dataGridView1.RowCount + 1, Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], "", "", "", "", Dr[6], "", "", Dr[7], "", Cmd.ExecuteScalar().ToString());

                            cellValue("Select [DeliveryMethod] from [ISBS-NEW4].[dbo].[DeliveryMethod]");
                            cell.Value = "Select";
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DeliveryMethod"] = cell;

                            cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                            cell.Value = "Select";
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                        }
                        ModeNew();
                    }
                    else
                    {
                        string dataString = "";
                        string dataString2 = "";
                        string dataString3 = "";
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
                        for (int i = 0; i < SearchV2.data3.Count; i++)
                        {
                            if (i >= 1)
                                dataString3 += ", ";
                            else if (i == 0)
                                dataString3 += "'01', ";
                            dataString3 += "'" + SearchV2.data3[i] + "'";
                        }
                        Query = "select a.GelombangId, a.BracketId, a.Type, a.ItemId 'FullItemId', a.ItemName, a.Base, a.Price, a.SeqNo, b.Ratio, c.GroupID, c.SubGroup1ID, c.SubGroup2ID, c.ItemID , c.UoM, c.UoM_AvgPrice, c.UoMAlt from InventGelombangD a left join InventConversion b on a.ItemId = b.FullItemId left join InventTable c on a.ItemId = c.FullItemID where a.GelombangID in (" + dataString + ") and a.BracketId in (" + dataString2 + ") and a.SeqNo in (" + dataString3 + ") and Type = 'Sales' order by a.GelombangId, a.SeqNo";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            dataGridView1.Rows.Add(1);
                            for (int i = 0; i < tableCols.Length; i++)
                            {
                                if (!(tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo" || tableCols[i] == "Qty" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Price_Alt" || tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo" || tableCols[i] == "SubTotal" || tableCols[i] == "SubTotal_PPN" || tableCols[i] == "SubTotal_PPH" || tableCols[i] == "LogisticAmount" || tableCols[i] == "LogisticNotes" || tableCols[i] == "DiscPercent" || tableCols[i] == "DiscAmount" || tableCols[i] == "BonusAmount" || tableCols[i] == "CashBackAmount" || tableCols[i] == "Notes"))
                                {
                                    if (tableCols[i] == "No")
                                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                                    else if (tableCols[i] == "Unit")
                                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["UoM"];
                                    else if (tableCols[i] == "Unit_Alt")
                                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["UoMAlt"];
                                    else if (tableCols[i] == "Price" || tableCols[i] == "Gelombang_Price")
                                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["Price"];
                                    else if (tableCols[i] == "ConvertionRatio")
                                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["Ratio"];
                                    else if (tableCols[i] == "GelombangSeqNo_Base")
                                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["SeqNo"];
                                    else if (tableCols[i] == "DeliveryMethod")
                                    {
                                        if (Dr["Base"].ToString() == "Y")
                                        {
                                            cellValue("Select [DeliveryMethod] from [ISBS-NEW4].[dbo].[DeliveryMethod]");
                                            cell.Value = "Select";
                                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["DeliveryMethod"] = cell;
                                        }
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
                            }
                        }
                        Dr.Close();
                        ModeNew();
                    }
                }
            }
            else if (Mode != "New")
            {
                dataGridView1.Rows.Clear();
                if (SQID != String.Empty)
                    tbxSQID.Text = SQID;
                Query = "Select * from [ISBS-NEW4].[dbo].[SalesQuotationH] where SalesQuotationNo = '" + tbxSQID.Text + "'; ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtSQ.Value = Convert.ToDateTime(Dr["OrderDate"]);
                    tbxMoUID.Text = Dr["SalesMouNo"].ToString();
                    tbxRefID.Text = Dr["RefTransId"].ToString();
                    cbCurrency.Text = Dr["CurrencyID"].ToString();
                    tbxXRate.Text = Dr["ExchRate"].ToString();
                    tbxSTotal.Text = Dr["Total"].ToString();
                    tbxGDisc.Text = Dr["Total_Disk"].ToString();
                    cbPPN.Text = Dr["PPN"].ToString();
                    tbxGPPN.Text = Dr["Total_PPN"].ToString();
                    cbPPh.Text = Dr["PPH"].ToString();
                    tbxGPPh.Text = Dr["Total_PPH"].ToString();
                    tbxGTotal.Text = Dr["Total_Nett"].ToString();
                    tbxGBonus.Text = Dr["Total_Bonus"].ToString();
                    tbxGCashback.Text = Dr["Total_Cashback"].ToString();
                    tbxToP.Text = Dr["TermofPayment"].ToString();
                    Cmd = new SqlCommand("Select PaymentModeName from [ISBS-NEW4].[dbo].[PaymentMode] where PaymentModeID = '" + Dr["PaymentModeID"].ToString() + "'", Conn);
                    cbPaymentMode.Text = Cmd.ExecuteScalar().ToString();
                    cbDPType.Text = Dr["DPType"].ToString();
                    tbxDP.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["DPAmount"].ToString()));

                    //DateTime dateTo = DateTime.ParseExact(this.Dr["DPDueDate"].ToString(), "dd/MM/yyyy", null);
                    if (Dr["DPType"].ToString() == "Y")
                        dtDP.Value = Convert.ToDateTime(Dr["DPDueDate"]);
                    tbxNotes.Text = Dr["Notes"].ToString();
                    cbType.Text = Dr["TransType"].ToString();
                }

                dataGridView1.AllowUserToAddRows = false;
                if (dataGridView1.RowCount - 1 <= 0)
                {
                    dataGridView1.ColumnCount = tableCols.Length;
                    for (int i = 0; i < tableCols.Length; i++)
                    {
                        dataGridView1.Columns[i].Name = tableCols[i];
                    }
                }

                Query = "select * from [ISBS-NEW4].[dbo].[SalesQuotationD] where SalesQuotationNo = '" + tbxSQID.Text + "' order by SeqNo asc; ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int no = 1;
                while (Dr.Read())
                {
                    dataGridView1.Rows.Add(1);
                    dataGridView1.Rows[no - 1].Cells[0].Value = dataGridView1.Rows.Count;
                    for (int i = 1; i < tableCols.Length; i++)
                    {
                        if (tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo")
                        {
                            if (Dr[tableCols[i]] != (object)DBNull.Value)
                                dataGridView1.Rows[no - 1].Cells[i].Value = Convert.ToDateTime(Dr[tableCols[i]]);
                        }
                        else
                            dataGridView1.Rows[no - 1].Cells[i].Value = Dr[tableCols[i]].ToString();
                    }
                    if (Mode == "Edit")
                    {
                        cellValue("Select [DeliveryMethod] from [ISBS-NEW4].[dbo].[DeliveryMethod]");
                        if (Dr["DeliveryMethod"] != null)
                            cell.Value = Dr["DeliveryMethod"].ToString();
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DeliveryMethod"] = cell;

                        cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                        Query = "Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr["DiscType"] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        if (Dr["DiscType"] != null)
                            cell.Value = Cmd.ExecuteScalar().ToString();
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                    }
                    else
                    {
                        Query = "Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr["DiscType"] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"].Value = Cmd.ExecuteScalar().ToString();
                    }
                    no++;
                }
            }
        }

        #region cellValue
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
        #endregion

        public void GetDataHeader2()
        {
            Conn = ConnectionString.GetConnection();
            dataGridView2.AllowUserToAddRows = false;
            if (dataGridView2.RowCount - 1 <= 0)
            {
                dataGridView2.ColumnCount = tableCols2.Length;
                for (int i = 0; i < tableCols2.Length; i++)
                {
                    dataGridView2.Columns[i].Name = tableCols2[i];
                }
            }

            if (Mode == "New")
            {
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
                    if (tbxMoUID.Text != String.Empty)
                    {
                        Query = "select a.CustID, a.CustName from [ISBS-NEW4].[dbo].[CustTable] as a left join CustMou_Dtl as b on a.CustID = b.CustID where b.MouNo in (" + dataString + ") ";
                        if (SearchV2.data2.Count != 0)
                            Query += "and b.SeqNo in (" + dataString2 + ") ";
                    }
                    else
                    {
                        Query = "Select CustID, CustName from [ISBS-NEW4].[dbo].[CustTable] where CustID in (" + dataString + "); ";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        this.dataGridView2.Rows.Add(dataGridView2.RowCount + 1, Dr[0], Dr[1]);
                    }
                    ModeNew();
                }
            }
            else if (Mode != "New")
            {
                dataGridView2.Rows.Clear();
                if (SQID != String.Empty)
                    tbxSQID.Text = SQID;

                //Cmd = new SqlCommand("Select count(*) from [dbo].[CustTable] where CustID in (" + tbxSQID.Text + "); ", Conn);
                //int rowCount = (Int32)Cmd.ExecuteScalar();

                Query = "Select CustID, CustName from [ISBS-NEW4].[dbo].[SalesQuotationH] where SalesQuotationNo = '" + tbxSQID.Text + "'; ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int no = 1;
                while (Dr.Read())
                {
                    dataGridView2.Rows.Add(1);
                    dataGridView2.Rows[no - 1].Cells[0].Value = no;
                    for (int i = 1; i < tableCols2.Length; i++)
                    {
                        dataGridView2.Rows[no - 1].Cells[i].Value = Dr[tableCols2[i]];
                    }
                    no++;
                }
            }
        }

        #region SetMode
        public void SetMode(string tmpMode, string tmpNumber)
        {
            Mode = tmpMode;
            tbxSQID.Text = tmpNumber;
            SQID = tmpNumber;
        }
        #endregion

        //type value : #1 string #2 decimal #3 int
        private void createLabel(Control textbox, Control lblName, Control location, string type)
        {
            if (validate == false)
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


        private DateTime ConvertObjectToDateTime(string dateTimeString)
        {
            int year = Convert.ToInt32(dateTimeString.Split('/')[2].Substring(0, 4));
            int day = Convert.ToInt32(dateTimeString.Split('/')[1]); //Convert.ToInt32(dateTimeString.Split('/')[1]);
            int month = Convert.ToInt32(dateTimeString.Split('/')[0]); //Convert.ToInt32(dateTimeString.Split('/')[0]);

            DateTime date;
            try
            {
                date = new DateTime(year, month, day, 0, 0, 0);
            }
            catch
            {
                month = Convert.ToInt32(dateTimeString.Split('/')[1]); //Convert.ToInt32(dateTimeString.Split('/')[1]);
                day = Convert.ToInt32(dateTimeString.Split('/')[0]); //Convert.ToInt32(dateTimeString.Split('/')[0]);
                date = new DateTime(year, month, day, 0, 0, 0);
            }
            return date;
        }

        private char Validation()
        {
            flag = '\0';
            msg = "";
            if (validate == false)
            {
                label = new Label[20];
            }

            createLabel(cbDPType, lblDPType, gbMain, "string");
            createLabel(cbCurrency, lblCurrency, gbMain, "string");
            createLabel(tbxToP, lblToP, gbMain, "string");
            createLabel(tbxXRate, lblXRate, gbMain, "decimal");
            createLabel(cbPaymentMode, lblPaymentMode, gbMain, "string");

            if (flag == 'X')
                msg += "* field is required!\r\n";

            if (dataGridView1.Rows.Count == 0)
                msg += "Please add Item!\n";

            if (dataGridView2.Rows.Count == 0)
                msg += "Please add Customer!\n";

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() != "N")
                {
                    if (cbType.Text != "AMOUNT")
                        if (dataGridView1.Rows[i].Cells["Qty"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) == 0)
                            msg += "Row " + Convert.ToInt32(i + 1) + " quantity cannot 0!\r\n";

                    if (dataGridView1.Rows[i].Cells["Price"].Value == String.Empty || dataGridView1.Rows[i].Cells["Price"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) == 0)
                        msg += "Row " + Convert.ToInt32(i + 1) + " price cannot 0!\r\n";

                    if (dataGridView1.Rows[i].Cells["DeliveryMethod"].Value.ToString() == "Select")
                    {
                        msg += "Row " + Convert.ToInt32(i + 1) + " Select Delivery Method!\n";
                    }

                    if (dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == String.Empty)
                        msg += "Row " + Convert.ToInt32(i + 1) + " Fill Expected Date From!\n";
                    if (dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == String.Empty)
                        msg += "Row " + Convert.ToInt32(i + 1) + " Fill Expected Date To!\n";

                    if (!(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null))
                    {
                        DateTime date1;
                        date1 = ConvertObjectToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value.ToString());
                        DateTime date2;
                        date2 = ConvertObjectToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value.ToString());
                        int result = DateTime.Compare(date1, date2); //hendry switch
                        //string relationship;

                        if (result < 0) { }//relationship = "is earlier than";
                        else if (result == 0) { } //relationship = "is the same time as";
                        else //relationship = "is later than";
                            msg += "Row " + Convert.ToInt32(i + 1) + " Expected Date To (" + dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value.ToString() + ") must be later than Date From (" + dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value.ToString() + ")\n";
                    }
                }
            }

            if (!(String.IsNullOrEmpty(msg)))
            {
                MessageBox.Show(msg, "Warning");
                flag = 'X';
            }
            count = 0;
            validate = true;
            return flag;
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            DateTime date1 = new DateTime(dtSQ.Value.Year, dtSQ.Value.Month, dtSQ.Value.Day, 00, 00, 00);
            dtDP.MinDate = date1;

            btnMoUID.Enabled = false; cbDPType.Enabled = true;
            cbCurrency.Enabled = true; tbxToP.Enabled = true;
            if (cbCurrency.Text == "IDR")
                tbxXRate.Enabled = false;
            else
                tbxXRate.Enabled = true;

            btnAddItem.Enabled = true; btnDeleteItem.Enabled = true;

            cbPPh.Enabled = true; cbPPN.Enabled = true;
            cbPaymentMode.Enabled = true; 

            tbxNotes.Enabled = true;
            if (cbDPType.Text == "Y")
            {
                tbxDP.Visible = true; dtDP.Visible = true; dtDP.Enabled = true;
            }
            else
            {
                tbxDP.Visible = false; dtDP.Visible = false;
            }

            btnEdit.Enabled = false; btnCancel.Enabled = true; btnSave.Enabled = true;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (!(tableCols[i] == "No" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "FullItemID" || tableCols[i] == "ItemName" || tableCols[i] == "Unit" || tableCols[i] == "Qty_Alt" || tableCols[i] == "SubTotal" || tableCols[i] == "SubTotal_PPN" || tableCols[i] == "SubTotal_PPH" || tableCols[i] == "DiscPercent" || tableCols[i] == "DiscAmount" || tableCols[i] == "Base" || tableCols[i] == "Unit_Alt" || tableCols[i] == "ConvertionRatio"))
                {
                    dataGridView1.Columns[i].ReadOnly = false;
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                }
            }

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (dataGridView1.Rows[i].Cells["Base"].Value == null || dataGridView1.Rows[i].Cells["Base"].Value == String.Empty)
                        dataGridView1.Rows[i].Cells["Base"].Value = "";
                    if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y" && cbType.Text == "QTY")
                    {
                        dataGridView1.Rows[i].Cells["Qty_Alt"].ReadOnly = false;
                        dataGridView1.Rows[i].Cells["Qty_Alt"].Style.BackColor = Color.White;
                        dataGridView1.Rows[i].Cells["Qty"].ReadOnly = true;
                        dataGridView1.Rows[i].Cells["Qty"].Style.BackColor = Color.LightGray;
                        dataGridView1.Rows[i].Cells["Price"].ReadOnly = true;
                        dataGridView1.Rows[i].Cells["Price"].Style.BackColor = Color.LightGray;
                    }
                    else if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y" && cbType.Text == "AMOUNT")
                    {
                        dataGridView1.Rows[i].Cells["SubTotal"].ReadOnly = false;
                        dataGridView1.Rows[i].Cells["SubTotal"].Style.BackColor = Color.White;
                    }
                    if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "N")
                    {
                        dataGridView1.Rows[i].Cells[j].ReadOnly = true;
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                    }
                    else
                        break;
                }
            }

            if (btnRevision.Visible == true || btnApprove.Visible == true || btnReject.Visible == true || btnAmend.Visible == true)
            {
                btnRevision.Visible = false; btnApprove.Visible = false; btnReject.Visible = false; btnAmend.Visible = false;
            }
        }

        private void ModeNew()
        {
            DateTime date1 = new DateTime(dtSQ.Value.Year, dtSQ.Value.Month, dtSQ.Value.Day, 00, 00, 00);
            dtDP.MinDate = date1;
            dtSQ.Value = DateTime.Now;
            dtDP.Value = DateTime.Now;

            btnMoUID.Enabled = true; cbDPType.Enabled = true;
            cbCurrency.Enabled = true; tbxXRate.Enabled = false; tbxToP.Enabled = true;
            btnAddCust.Enabled = true; btnDeleteCust.Enabled = true;
            btnAddItem.Enabled = true; btnDeleteItem.Enabled = true;
            cbPPh.Enabled = true; cbPPN.Enabled = true; cbPaymentMode.Enabled = true;
            if (cbDPType.Text == "Y")
            {
                tbxDP.Visible = true;
                dtDP.Visible = true;
            }
            else
            {
                tbxDP.Visible = false;
                dtDP.Visible = false;
            }
            tbxNotes.Enabled = true;
            btnApprove.Visible = false; btnReject.Visible = false;
            btnRevision.Visible = false; btnAmend.Visible = false;
            btnEdit.Enabled = false; btnCancel.Enabled = false; btnSave.Enabled = true;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (tableCols[i] == "No" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "FullItemID" || tableCols[i] == "ItemName" || tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo" || tableCols[i] == "Unit" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "Price_Alt" || tableCols[i] == "ConvertionRatio" || tableCols[i] == "SubTotal" || tableCols[i] == "SubTotal_PPN" || tableCols[i] == "SubTotal_PPH" || tableCols[i] == "DiscPercent" || tableCols[i] == "DiscAmount" || tableCols[i] == "GelombangId" || tableCols[i] == "BracketId" || tableCols[i] == "Base" || tableCols[i] == "Gelombang_Price" || tableCols[i] == "GelombangSeqNo_Base")
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (dataGridView1.Rows[i].Cells["Base"].Value == null || dataGridView1.Rows[i].Cells["Base"].Value == String.Empty)
                        dataGridView1.Rows[i].Cells["Base"].Value = "";
                    if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y" && cbType.Text == "QTY")
                    {
                        dataGridView1.Rows[i].Cells["Qty_Alt"].ReadOnly = false;
                        dataGridView1.Rows[i].Cells["Qty_Alt"].Style.BackColor = Color.White;
                        dataGridView1.Rows[i].Cells["Qty"].ReadOnly = true;
                        dataGridView1.Rows[i].Cells["Qty"].Style.BackColor = Color.LightGray;
                        dataGridView1.Rows[i].Cells["Price"].ReadOnly = true;
                        dataGridView1.Rows[i].Cells["Price"].Style.BackColor = Color.LightGray;
                    }
                    else if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y" && cbType.Text == "AMOUNT")
                    {
                        dataGridView1.Rows[i].Cells["SubTotal"].ReadOnly = false;
                        dataGridView1.Rows[i].Cells["SubTotal"].Style.BackColor = Color.White;
                    }
                    if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "N")
                    {
                        dataGridView1.Rows[i].Cells[j].ReadOnly = true;
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                    }
                    else
                        break;
                }
            }

            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                dataGridView2.Columns[i].ReadOnly = true;
                dataGridView2.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            btnMoUID.Enabled = false;
            cbType.Enabled = false;
            cbDPType.Enabled = false; cbCurrency.Enabled = false; dtDP.Enabled = false;
            tbxToP.Enabled = false; tbxXRate.Enabled = false; dtSQ.Enabled = false;

            btnAddItem.Enabled = false; btnDeleteItem.Enabled = false;
            btnAddCust.Enabled = false; btnDeleteCust.Enabled = false;

            cbPPh.Enabled = false; cbPPN.Enabled = false;
            tbxDP.Enabled = false; tbxNotes.Enabled = false;
            cbPaymentMode.Enabled = false;

            btnEdit.Enabled = true; btnCancel.Enabled = false; btnSave.Enabled = false;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
                dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
            }
            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                dataGridView2.Columns[i].ReadOnly = true;
                dataGridView2.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
            }

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn);
            string transStatus = Cmd.ExecuteScalar().ToString();
            if (transStatus == "02" || transStatus == "22")
            {
                btnApprove.Visible = true; btnRevision.Visible = true; btnReject.Visible = true;
            }
            else if (transStatus == "03" || transStatus == "01" || transStatus == "07" || transStatus == "23")
            {
                btnAmend.Visible = true;
            }
            else if (transStatus == "04" || transStatus == "21" || transStatus == "09")
            {
                btnEdit.Enabled = false;
            }
            else
            {
                btnApprove.Visible = false; btnRevision.Visible = false; btnReject.Visible = false; btnAmend.Visible = false;
            }
            Conn.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbCurrency_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbDPType_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        //Start Steven Edit 15/03/2018--------------------
        private void insertStatusLogSave()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Customer] ([StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
            Query += " VALUES ('SQ Header', '" + tbxSQID.Text + "', 'PK2Test', 'PK3Test', 'PK4Test',";
            if (flag == 'X')
            {
                Query += "'02' ,'Waiting for Approval'";
            }
            else
            {
                Query += "'01', 'Created'";
            }
            Query += ",'" + Login.Username + "' , GetDate())";
            SqlCommand Cmd2 = new SqlCommand(Query, Conn, Trans);
            Cmd2.ExecuteNonQuery();
        }

        private void insertStatusLogApprove()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Customer] ([StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
            Query += " VALUES ('SQ Header', '" + tbxSQID.Text + "', 'PK2Test', 'PK3Test', 'PK4Test',";
            Cmd = new SqlCommand("Select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() == "23")
            {
                Query += "'23','Revised – Approved'";
            }
            else if (Cmd.ExecuteScalar().ToString() == "03")
            {
                Query += "'03','Approved'";
            }
            Query += ",'" + Login.Username + "' , GetDate())";
            SqlCommand CmdinsertSLApprove = new SqlCommand(Query, Conn, Trans);
            CmdinsertSLApprove.ExecuteNonQuery();
        }

        private void insertStatusLogReject()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Customer] ([StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
            Query += " VALUES ('SQ Header', '" + tbxSQID.Text + "', 'PK2Test', 'PK3Test', 'PK4Test',";
            Query += "'04','Not-approved - Closed'";
            Query += ",'" + Login.Username + "' , GetDate())";
            SqlCommand CmdinsertSLApprove = new SqlCommand(Query, Conn, Trans);
            CmdinsertSLApprove.ExecuteNonQuery();
        }

        private void insertStatusLogRevision()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Customer] ([StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
            Query += " VALUES ('SQ Header', '" + tbxSQID.Text + "', 'PK2Test', 'PK3Test', 'PK4Test',";
            Query += "'05','Revision Needed'";
            Query += ",'" + Login.Username + "' , GetDate())";
            SqlCommand CmdinsertSLApprove = new SqlCommand(Query, Conn, Trans);
            CmdinsertSLApprove.ExecuteNonQuery();
        }
        //End Steven Edit 15/03/2018--------------------

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation() != 'X')
            {
                try
                {
                    if (String.IsNullOrEmpty(tbxDP.Text.Trim()))
                        tbxDP.Text = "0";
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        Conn2 = ConnectionString.GetConnection();
                        string[] TmpSQNumber = new string[dataGridView2.RowCount];
                        if (Mode == "New")
                        {
                            string tempID;
                            for (int i = 0; i < dataGridView2.RowCount; i++)
                            {
                                //Old Code======================================================================
                                //tempID = ConnectionString.GenerateID("SalesQuotationNo", "SalesQuotationH", "SQ");
                                //End Old Code==================================================================

                                //begin=========================================================================
                                //updated by : joshua
                                //updated date : 13 Feb 2018
                                //description : change generate sequence number, get from global function and update counter 
                                string Jenis = "SQ", Kode = "SQ";
                                tempID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                                //update counter
                                //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                                //end update counter
                                //end=============================================================================
                                Query = "insert into [dbo].[SalesQuotationH] ([SalesQuotationNo],[OrderDate],[SalesMouNo],[RefTransId],[CustGroupID],[CustID],[CustName],[CurrencyID],[ExchRate],[Total],[Total_Disk],[PPN],[Total_PPN],[PPH],[Total_PPH],[Total_Nett],[Total_Bonus],[Total_Cashback],[TermofPayment],[PaymentModeID],[DPType],[DPPercent],[DPAmount],[DPDueDate],[Notes],[TransStatus],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Total_LogisticAmount],[TransType]) values (@SalesQuotationNo, @OrderDate, @SalesMouNo, @RefTransId, @CustGroupID, @CustID, @CustName, @CurrencyID, CAST(@ExchRate as numeric(20, 4)), @Total, @Total_Disk, @PPN, @Total_PPN, @PPH, @Total_PPH, @Total_Nett, @Total_Bonus, @Total_Cashback, @TermofPayment, @PaymentModeID, @DPType, CAST(@DPPercent as numeric(6,2)), cast(@DPAmount as numeric(20,4)), @DPDueDate, @Notes, @TransStatus, @CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy,@Total_LogisticAmount, '" + cbType.Text + "'); ";
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                #region insert value for SalesQuotationH
                                Cmd2.Parameters.AddWithValue("@SalesQuotationNo", tempID);
                                Cmd2.Parameters.AddWithValue("@OrderDate", dtSQ.Value);
                                Cmd2.Parameters.AddWithValue("@SalesMouNo", tbxMoUID.Text == String.Empty ? (object)DBNull.Value : tbxMoUID.Text);
                                Cmd2.Parameters.AddWithValue("RefTransId", tbxRefID.Text == String.Empty ? (object)DBNull.Value : tbxRefID.Text);
                                Cmd2.Parameters.AddWithValue("@CustGroupID", (object)DBNull.Value);
                                Cmd2.Parameters.AddWithValue("@CustID", dataGridView2.Rows[i].Cells["CustID"].Value.ToString());
                                Cmd2.Parameters.AddWithValue("@CustName", dataGridView2.Rows[i].Cells["CustName"].Value.ToString());
                                Cmd2.Parameters.AddWithValue("@CurrencyID", cbCurrency.Text == "Select" ? (object)DBNull.Value : cbCurrency.Text);
                                Cmd2.Parameters.AddWithValue("@ExchRate", tbxXRate.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxXRate.Text));
                                Cmd2.Parameters.AddWithValue("@Total", tbxSTotal.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxSTotal.Text));
                                Cmd2.Parameters.AddWithValue("@Total_Disk", tbxGDisc.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGDisc.Text));
                                Cmd2.Parameters.AddWithValue("@PPN", cbPPN.Text == "Select" ? (object)DBNull.Value : Convert.ToDecimal(cbPPN.Text));
                                Cmd2.Parameters.AddWithValue("@Total_PPN", tbxGPPN.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGPPN.Text));
                                Cmd2.Parameters.AddWithValue("@PPH", cbPPh.Text == "Select" ? (object)DBNull.Value : Convert.ToDecimal(cbPPh.Text));
                                Cmd2.Parameters.AddWithValue("@Total_PPH", tbxGPPN.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGPPh.Text));
                                Cmd2.Parameters.AddWithValue("@Total_Nett", tbxGTotal.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGTotal.Text));
                                Cmd2.Parameters.AddWithValue("@Total_Bonus", tbxGBonus.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGBonus.Text));
                                Cmd2.Parameters.AddWithValue("@Total_Cashback", tbxGCashback.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGCashback.Text));
                                Cmd2.Parameters.AddWithValue("@TermofPayment", String.IsNullOrEmpty(tbxToP.Text) ? (object)DBNull.Value : tbxToP.Text);
                                Cmd = new SqlCommand("Select [PaymentModeID] from [ISBS-NEW4].[dbo].[PaymentMode] where [PaymentModeName] = '" + cbPaymentMode.Text + "'", Conn2);
                                Cmd2.Parameters.AddWithValue("@PaymentModeID", Cmd.ExecuteScalar().ToString());
                                Cmd2.Parameters.AddWithValue("@DPType", cbDPType.Text == "Select" ? (object)DBNull.Value : cbDPType.Text);
                                if (tbxDP.Text.Contains('%'))
                                {
                                    decimal dpPercent = Convert.ToDecimal(tbxDP.Text.Trim(new char[] { '%' }));
                                    Cmd2.Parameters.AddWithValue("@DPPercent", dpPercent);
                                    Cmd2.Parameters.AddWithValue("@DPAmount", Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDP.Text.Trim(new char[] { '%' })) / 100);
                                }
                                else
                                {
                                    if (Convert.ToDecimal(tbxDP.Text) == 0 || Convert.ToDecimal(tbxGTotal.Text) == 0)
                                        Cmd2.Parameters.AddWithValue("@DPPercent", Convert.ToDecimal("0"));
                                    else
                                        Cmd2.Parameters.AddWithValue("@DPPercent", Decimal.Round(Convert.ToDecimal(tbxDP.Text) / Convert.ToDecimal(tbxGTotal.Text), 2));
                                    Cmd2.Parameters.AddWithValue("@DPAmount", Convert.ToDecimal(tbxDP.Text));
                                }
                                Cmd2.Parameters.AddWithValue("@DPDueDate", cbDPType.Text == "N" ? (object)DBNull.Value : dtDP.Value);
                                Cmd2.Parameters.AddWithValue("@Notes", tbxNotes.Text);
                                flag = checkPriceTolerance();
                                if (flag == 'X')
                                    Cmd2.Parameters.AddWithValue("@TransStatus", "02");
                                else
                                    Cmd2.Parameters.AddWithValue("@TransStatus", "01");
                                Cmd2.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                Cmd2.Parameters.AddWithValue("@CreatedBy", Login.Username);
                                Cmd2.Parameters.AddWithValue("@UpdatedDate", DBNull.Value);
                                Cmd2.Parameters.AddWithValue("@UpdatedBy", DBNull.Value);
                                Cmd2.Parameters.AddWithValue("@Total_LogisticAmount", tbxLogAmount.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxLogAmount.Text));
                                #endregion
                                Cmd2.ExecuteNonQuery();
                                for (int j = 0; j < dataGridView1.RowCount; j++)
                                {
                                    Query = "insert into [dbo].[SalesQuotationD] ([SalesQuotationNo],[SeqNo],[GroupID],[SubGroup1ID],[SubGroup2ID],[ItemID],[FullItemID],[ItemName],[RefTransId],[RefTrans_SeqNo],[DeliveryMethod],[LogisticAmount],[LogisticNotes],[ExpectedDateFrom],[ExpectedDateTo],[Qty],[Unit],[Qty_Alt],[Unit_Alt],[ConvertionRatio],[Price],[Price_Alt],[DiscType],[DiscPercent],[DiscAmount],[BonusAmount],[CashBackAmount],[SubTotal],[SubTotal_PPN],[SubTotal_PPH],[Notes],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GelombangId],[BracketId],[Base],[Gelombang_Price],[GelombangSeqNo_Base]) values (@SalesQuotationNo ,@SeqNo ,@GroupID ,@SubGroup1ID ,@SubGroup2ID ,@ItemID ,@FullItemID ,@ItemName,@RefTransId ,@RefTrans_SeqNo ,@DeliveryMethod ,@LogisticAmount ,@LogisticNotes ,@ExpectedDateFrom ,@ExpectedDateTo ,@Qty ,@Unit ,@Qty_Alt ,@Unit_Alt ,@ConvertionRatio ,@Price ,@Price_Alt ,@DiscType ,@DiscPercent ,@DiscAmount ,@BonusAmount ,@CashBackAmount ,@SubTotal ,@SubTotal_PPN ,@SubTotal_PPH ,@Notes ,@CreatedDate ,@CreatedBy ,@UpdatedDate ,@UpdatedBy, @GelombangId, @BracketId, @Base, @Gelombang_Price, @GelombangSeqNo_Base); ";
                                    SqlCommand Cmd3 = new SqlCommand(Query, Conn);
                                    #region insert value SalesQuotationD
                                    Cmd3.Parameters.AddWithValue("@SalesQuotationNo", tempID);
                                    Cmd3.Parameters.AddWithValue("@SeqNo", j + 1);
                                    Cmd3.Parameters.AddWithValue("@GroupID", dataGridView1.Rows[j].Cells["GroupID"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@SubGroup1ID", dataGridView1.Rows[j].Cells["SubGroup1ID"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@SubGroup2ID", dataGridView1.Rows[j].Cells["SubGroup2ID"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@ItemID", dataGridView1.Rows[j].Cells["ItemID"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@ItemName", dataGridView1.Rows[j].Cells["ItemName"].Value.ToString());
                                    if (dataGridView1.Rows[j].Cells["RefTransId"].Value == null || dataGridView1.Rows[j].Cells["RefTransId"].Value == String.Empty)
                                        Cmd3.Parameters.AddWithValue("@RefTransId", (object)DBNull.Value);
                                    else
                                        Cmd3.Parameters.AddWithValue("@RefTransId", dataGridView1.Rows[j].Cells["RefTransId"].Value.ToString());
                                    if (dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == String.Empty)
                                        Cmd3.Parameters.AddWithValue("@RefTrans_SeqNo", (object)DBNull.Value);
                                    else
                                        Cmd3.Parameters.AddWithValue("@RefTrans_SeqNo", dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@DeliveryMethod", dataGridView1.Rows[j].Cells["DeliveryMethod"].Value == null ? (object)DBNull.Value : dataGridView1.Rows[j].Cells["DeliveryMethod"].Value.ToString());
                                    if (dataGridView1.Rows[j].Cells["LogisticAmount"].Value == null)
                                        Cmd3.Parameters.AddWithValue("@LogisticAmount", Convert.ToDecimal("0"));
                                    else if (dataGridView1.Rows[j].Cells["LogisticAmount"].Value.ToString() == String.Empty)
                                        Cmd3.Parameters.AddWithValue("@LogisticAmount", Convert.ToDecimal("0"));
                                    else
                                        Cmd3.Parameters.AddWithValue("@LogisticAmount", Convert.ToDecimal(dataGridView1.Rows[j].Cells["LogisticAmount"].Value.ToString()));

                                    if (dataGridView1.Rows[j].Cells["LogisticNotes"].Value == null || dataGridView1.Rows[j].Cells["LogisticNotes"].Value == String.Empty)
                                        Cmd3.Parameters.AddWithValue("@LogisticNotes", String.Empty);
                                    else
                                        Cmd3.Parameters.AddWithValue("@LogisticNotes", dataGridView1.Rows[j].Cells["LogisticNotes"].Value.ToString());
                                    if (dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value == null || dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value == String.Empty)
                                        Cmd3.Parameters.AddWithValue("@ExpectedDateFrom", (object)DBNull.Value);
                                    else
                                    {
                                        DateTime date1 = ConvertObjectToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateFrom"].Value.ToString());
                                        Cmd3.Parameters.AddWithValue("@ExpectedDateFrom", date1);
                                    }
                                    if (dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value == null || dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value == String.Empty)
                                        Cmd3.Parameters.AddWithValue("@ExpectedDateTo", (object)DBNull.Value);
                                    else
                                    {
                                        DateTime date2 = ConvertObjectToDateTime(dataGridView1.Rows[j].Cells["ExpectedDateTo"].Value.ToString());
                                        Cmd3.Parameters.AddWithValue("@ExpectedDateTo", date2);
                                    }

                                    Cmd3.Parameters.AddWithValue("@Qty", dataGridView1.Rows[j].Cells["Qty"].Value == null ? 0 : Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value.ToString()));
                                    Cmd3.Parameters.AddWithValue("@Unit", dataGridView1.Rows[j].Cells["Unit"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@Qty_Alt", dataGridView1.Rows[j].Cells["Qty_Alt"].Value == null ? 0 : Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value.ToString()));
                                    Cmd3.Parameters.AddWithValue("@Unit_Alt", dataGridView1.Rows[j].Cells["Unit_Alt"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@ConvertionRatio", Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value.ToString()));
                                    Cmd3.Parameters.AddWithValue("@Price", Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value.ToString()));
                                    Cmd3.Parameters.AddWithValue("@Price_Alt", dataGridView1.Rows[j].Cells["Price_Alt"].Value == null ? 0 : Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value.ToString()));
                                    if (dataGridView1.Rows[j].Cells["DiscType"].Value == null || dataGridView1.Rows[j].Cells["DiscType"].Value == String.Empty)
                                        dataGridView1.Rows[j].Cells["DiscType"].Value = "Amount";
                                    else if (dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() == "Select")
                                        dataGridView1.Rows[j].Cells["DiscType"].Value = "Amount";
                                    Cmd = new SqlCommand("select [DiskonSchemeID] from [ISBS-NEW4].[dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() + "'", Conn2);
                                    Cmd3.Parameters.AddWithValue("@DiscType", Cmd.ExecuteScalar().ToString());
                                    Cmd3.Parameters.AddWithValue("@DiscPercent", dataGridView1.Rows[j].Cells["DiscPercent"].Value == null ? 0 : Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscPercent"].Value.ToString()));
                                    Cmd3.Parameters.AddWithValue("@DiscAmount", dataGridView1.Rows[j].Cells["DiscAmount"].Value == null ? 0 : Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscAmount"].Value.ToString()));
                                    if (dataGridView1.Rows[j].Cells["BonusAmount"].Value == null)
                                        Cmd3.Parameters.AddWithValue("@BonusAmount", Convert.ToDecimal("0"));
                                    else
                                        Cmd3.Parameters.AddWithValue("@BonusAmount", Convert.ToDecimal(dataGridView1.Rows[j].Cells["BonusAmount"].Value.ToString()));
                                    if (dataGridView1.Rows[j].Cells["CashBackAmount"].Value == null)
                                        Cmd3.Parameters.AddWithValue("@CashBackAmount", Convert.ToDecimal("0"));
                                    else
                                        Cmd3.Parameters.AddWithValue("@CashBackAmount", Convert.ToDecimal(dataGridView1.Rows[j].Cells["CashBackAmount"].Value.ToString()));
                                    Cmd3.Parameters.AddWithValue("@SubTotal", Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal"].Value.ToString()));
                                    Cmd3.Parameters.AddWithValue("@SubTotal_PPN", Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPN"].Value.ToString()));
                                    Cmd3.Parameters.AddWithValue("@SubTotal_PPH", Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPH"].Value.ToString()));
                                    if (dataGridView1.Rows[j].Cells["Notes"].Value == null)
                                        Cmd3.Parameters.AddWithValue("@Notes", String.Empty);
                                    else
                                        Cmd3.Parameters.AddWithValue("@Notes", dataGridView1.Rows[j].Cells["Notes"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                                    Cmd3.Parameters.AddWithValue("@CreatedBy", Login.Username);
                                    Cmd3.Parameters.AddWithValue("@UpdatedDate", DBNull.Value);
                                    Cmd3.Parameters.AddWithValue("@UpdatedBy", DBNull.Value);
                                    Cmd3.Parameters.AddWithValue("@GelombangId", dataGridView1.Rows[j].Cells["GelombangId"].Value == null ? (object)DBNull.Value : dataGridView1.Rows[j].Cells["GelombangId"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@BracketId", dataGridView1.Rows[j].Cells["BracketId"].Value == null ? (object)DBNull.Value : dataGridView1.Rows[j].Cells["BracketId"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@Gelombang_Price", dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == null ? (object)DBNull.Value : dataGridView1.Rows[j].Cells["Gelombang_Price"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@GelombangSeqNo_Base", dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == null ? (object)DBNull.Value : dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value.ToString());
                                    Cmd3.Parameters.AddWithValue("@Base", dataGridView1.Rows[j].Cells["Base"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView1.Rows[j].Cells["Base"].Value.ToString());
                                    #endregion
                                    Cmd3.ExecuteNonQuery();
                                    tbxSQID.Text = tempID;
                                }
                                TmpSQNumber[i] = tempID;
                            }
                            //UPDATE OLD SQ STATUS
                            Query = "Update [dbo].[SalesQuotationH] SET [TransStatus] = '21',[UpdatedDate] = getdate(),[UpdatedBy] = '" + Login.Username + "' where [SalesQuotationNo] = '" + tbxRefID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                            //STEVEN EDIT
                            insertStatusLogSave();
                            //STEVEN EDIT
                            string TmpMsgSqNo = "";
                            for (int i = 0; i < TmpSQNumber.Count(); i++)
                            {
                                TmpMsgSqNo += TmpSQNumber[i] + "\n";
                            }
                            MessageBox.Show(TmpMsgSqNo + "\nSave Successful!", "Information", MessageBoxButtons.OK);
                        }
                        else if (Mode != "New")
                        {
                            #region update query for SalesQuotationH
                            Query = "update [dbo].[SalesQuotationH] SET [CurrencyID] = '" + cbCurrency.Text + "'";
                            Query += ",[ExchRate] = '" + Convert.ToDecimal(tbxXRate.Text) + "'";
                            Query += ",[Total] = '" + Convert.ToDecimal(tbxSTotal.Text) + "'";
                            Query += ",[Total_Disk] = '" + Convert.ToDecimal(tbxGDisc.Text) + "'";
                            Query += ",[PPN] = '" + Convert.ToDecimal(cbPPN.Text) + "'";
                            Query += ",[Total_PPN] = '" + Convert.ToDecimal(tbxGPPN.Text) + "'";
                            Query += ",[PPH] = '" + Convert.ToDecimal(cbPPh.Text) + "'";
                            Query += ",[Total_PPH] = '" + Convert.ToDecimal(tbxGPPh.Text) + "'";
                            Query += ",[Total_Nett] = '" + Convert.ToDecimal(tbxGTotal.Text) + "'";
                            Query += ",[Total_Bonus] = '" + Convert.ToDecimal(tbxGBonus.Text) + "'";
                            Query += ",[Total_Cashback] = '" + Convert.ToDecimal(tbxGCashback.Text) + "'";
                            Query += ",[TermofPayment] = '" + tbxToP.Text + "'";
                            Cmd = new SqlCommand("Select [PaymentModeID] from [ISBS-NEW4].[dbo].[PaymentMode] where [PaymentModeName] = '" + cbPaymentMode.Text + "'", Conn2);
                            Query += ",[PaymentModeID] = '" + Cmd.ExecuteScalar().ToString() + "'";
                            Query += ",[DPType] = '" + cbDPType.Text + "'";
                            if (tbxDP.Text.Contains('%'))
                            {
                                decimal dpPercent = Convert.ToDecimal(tbxDP.Text.Trim(new char[] { '%' }));
                                Query += ",[DPPercent] = '" + dpPercent + "'";
                                Query += ",[DPAmount] = '" + Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDP.Text.Trim(new char[] { '%' })) / 100 + "'";
                            }
                            else
                            {
                                if (Convert.ToDecimal(tbxDP.Text) == 0 || Convert.ToDecimal(tbxGTotal.Text) == 0)
                                    Query += ",[DPPercent] = 0";
                                else
                                {
                                    Query += ",[DPPercent] = '" + Decimal.Round(Convert.ToDecimal(tbxDP.Text) / Convert.ToDecimal(tbxGTotal.Text), 2) + "'";
                                    Query += ",[DPAmount] = '" + Convert.ToDecimal(tbxDP.Text) + "'";
                                }
                            }
                            Query += ",[DPDueDate] = '" + dtDP.Value + "'";
                            Query += ",[Notes] = @Notes";
                            Cmd = new SqlCommand("Select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn2);
                            if (Cmd.ExecuteScalar().ToString() == "05")
                                Query += ",[TransStatus] = '22'";
                            else
                            {
                                flag = checkPriceTolerance();
                                if (flag == 'X')
                                    Query += ",[TransStatus] = '02'";
                                else
                                    Query += ",[TransStatus] = '01'";
                            }
                            Query += ",[UpdatedDate] = '" + DateTime.Now + "'";
                            Query += ",[UpdatedBy] = '" + Login.Username + "'";
                            Query += ",[Total_LogisticAmount] = '" + Convert.ToDecimal(tbxLogAmount.Text) + "'";
                            Query += ",[TransType] = '" + cbType.Text + "'";
                            Query += "WHERE [SalesQuotationNo] = '" + tbxSQID.Text + "'; ";
                            #endregion
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.AddWithValue("@Notes", tbxNotes.Text);
                            Cmd.ExecuteNonQuery();
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                #region update query for SalesQuotationD
                                Query = "UPDATE [dbo].[SalesQuotationD] SET [GroupID] = '" + dataGridView1.Rows[i].Cells["GroupID"].Value + "'";
                                Query += ",[SubGroup1ID] = '" + dataGridView1.Rows[i].Cells["SubGroup1ID"].Value + "'";
                                Query += ",[SubGroup2ID] = '" + dataGridView1.Rows[i].Cells["SubGroup2ID"].Value + "'";
                                Query += ",[ItemID] = '" + dataGridView1.Rows[i].Cells["ItemID"].Value + "'";
                                Query += ",[FullItemID] = '" + dataGridView1.Rows[i].Cells["FullItemID"].Value + "'";
                                Query += ",[ItemName] = '" + dataGridView1.Rows[i].Cells["ItemName"].Value + "'";
                                Query += ",[DeliveryMethod] = '" + dataGridView1.Rows[i].Cells["DeliveryMethod"].Value + "'";
                                Query += ",[LogisticAmount] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value) + "'";
                                Query += ",[LogisticNotes] = @LogisticNotes";
                                if (dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value == null)
                                    Query += ", [ExpectedDateFrom] = NULL";
                                else
                                {
                                    DateTime date1 = ConvertObjectToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateFrom"].Value.ToString());
                                    Query += ",[ExpectedDateFrom] = '" + date1 + "'";
                                }
                                if (dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value == null)
                                    Query += ", [ExpectedDateTo] = NULL";
                                else
                                {
                                    DateTime date2 = ConvertObjectToDateTime(dataGridView1.Rows[i].Cells["ExpectedDateTo"].Value.ToString());
                                    Query += ",[ExpectedDateTo] = '" + date2 + "'";
                                }
                                Query += ",[Qty] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) + "'";
                                Query += ",[Unit] = '" + dataGridView1.Rows[i].Cells["Unit"].Value + "'";
                                Query += ",[Qty_Alt] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Alt"].Value) + "'";
                                Query += ",[Unit_Alt] = '" + dataGridView1.Rows[i].Cells["Unit_Alt"].Value + "'";
                                Query += ",[ConvertionRatio] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["ConvertionRatio"].Value) + "'";
                                Query += ",[Price] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) + "'";
                                Query += ",[Price_Alt] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price_Alt"].Value) + "'";
                                if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Select")
                                    dataGridView1.Rows[i].Cells["DiscType"].Value = "Amount";
                                Cmd = new SqlCommand("Select [DiskonSchemeID] from [ISBS-NEW4].[dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() + "'", Conn2);
                                Query += ",[DiscType] = '" + Cmd.ExecuteScalar().ToString() + "'";
                                Query += ",[DiscPercent] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscPercent"].Value) + "'";
                                Query += ",[DiscAmount] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value) + "'";
                                Query += ",[BonusAmount] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["BonusAmount"].Value) + "'";
                                Query += ",[CashBackAmount] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["CashBackAmount"].Value) + "'";
                                Query += ",[SubTotal] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value) + "'";
                                Query += ",[SubTotal_PPN] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value) + "'";
                                Query += ",[SubTotal_PPH] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value) + "'";
                                Query += ",[Notes] = @Notes";
                                Query += ",[UpdatedDate] = '" + DateTime.Now + "'";
                                Query += ",[UpdatedBy] = '" + Login.Username + "'";
                                Query += "WHERE [SalesQuotationNo] = '" + tbxSQID.Text + "' ";
                                Query += "and [SeqNo] = '" + Convert.ToInt32(i + 1) + "'; ";
                                #endregion
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.AddWithValue("@LogisticNotes", dataGridView1.Rows[i].Cells["LogisticNotes"].Value == null ? String.Empty : dataGridView1.Rows[i].Cells["LogisticNotes"].Value.ToString());
                                Cmd.Parameters.AddWithValue("@Notes", dataGridView1.Rows[i].Cells["Notes"].Value == null ? String.Empty : dataGridView1.Rows[i].Cells["Notes"].Value.ToString());
                                Cmd.ExecuteNonQuery();
                            }
                            //STEVEN EDIT
                            insertStatusLogSave();
                            //STEVEN EDIT

                            MessageBox.Show(tbxSQID.Text + " Update Successful!", "Information", MessageBoxButtons.OK);
                        }
                        //Trans.Commit();
                        Conn2.Close();
                        Conn.Close();
                        scope.Complete();
                    }
                    ModeBeforeEdit();
                    GetDataHeader();
                    Parent.RefreshGrid();
                }
                catch (Exception ex)
                {
                    //Trans.Rollback();
                    MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private char checkPriceTolerance()
        {
            flag = '\0';
            for (int j = 0; j < dataGridView1.Rows.Count; j++)
            {
                Cmd = new SqlCommand("select * from [ISBS-NEW4].[dbo].[SalesPriceListH] as a left join SalesPriceListDtl as b on a.PriceListNo = b.PriceListNo where a.ValidFrom <= '" + DateTime.Now + "' and a.ValidTo >= '" + DateTime.Now + "' and a.[StatusCode] = '02' and b.FullItemID = '" + dataGridView1.Rows[j].Cells["FullItemId"].Value.ToString() + "'", Conn2);
                Dr = Cmd.ExecuteReader();
                List<decimal> salesPrice = new List<decimal>();
                decimal tolerance = 0;
                while (Dr.Read())
                {
                    tolerance = Convert.ToDecimal(Dr["Tolerance"]);
                    for (int k = 0; k < priceListDays.Length; k++)
                    {
                        if (k == priceListDays.Length - 1)
                        {
                            if (Convert.ToInt32(tbxToP.Text) >= priceListDays[k])
                            {
                                salesPrice.Add(Convert.ToDecimal(Dr["Price" + priceListDays[k] + "D"]));
                                break;
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(tbxToP.Text) >= priceListDays[k] && Convert.ToInt32(tbxToP.Text) < priceListDays[k + 1])
                            {
                                salesPrice.Add(Convert.ToDecimal(Dr["Price" + priceListDays[k] + "D"]));
                                break;
                            }
                        }
                    }
                }
                Dr.Close();

                decimal temp;
                for (int a = 0; a < salesPrice.Count; a++)
                {
                    for (int b = a + 1; b < salesPrice.Count; b++)
                    {
                        if (salesPrice[a] > salesPrice[b])
                        {
                            temp = salesPrice[a];
                            salesPrice[a] = salesPrice[b];
                            salesPrice[b] = temp;
                        }
                    }
                }

                //PRICE EQUAL SALES PRICE
                if (salesPrice.Count() > 0)
                {
                    if (Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value) >= salesPrice[0] && Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value) <= salesPrice[salesPrice.Count - 1])
                    {
                    }
                    //PRICE CANNOT LOWER THAN SALES PRICE
                    else if (Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value) < salesPrice[0])
                    {
                        flag = 'X';
                        break;
                    }
                    else
                    {
                        //PRICE CANNOT HIGHER THAN SALES PRICE + TOLERANCE
                        if ((Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value) - salesPrice[salesPrice.Count - 1]) / salesPrice[salesPrice.Count - 1] * 100 > tolerance)
                        {
                            flag = 'X';
                            break;
                        }
                    }
                }
            }
            return flag;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.Edit) > 0)
            {
                Mode = "Edit";
                GetDataHeader();
                GetDataHeader2();
                ModeEdit();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end
        }

        private void btnSSAID_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "CustMouH", "");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                tbxMoUID.Text = SearchV2.data[0];
                dataGridView2.Rows.Clear();
                GetDataHeader2();
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (tbxRefID.Text == String.Empty)
            {
                SearchV2 f = new SearchV2();
                if (cbType.Text == "FIX")
                {
                    f.SetMode("Check");
                    f.SetSchemaTable("dbo", "InventTable", "");
                }
                else
                {
                    f.SetMode("No");
                    f.SetSchemaTable("dbo", "InventGelombangD", "and type = 'Sales'");
                }
                f.ShowDialog();
                GetDataHeader();
            }
            else
            {
                string itemRef = "";
                using (SqlConnection Conn = ConnectionString.GetConnection())
                {
                    Query = "Select FullItemID from [ISBS-NEW4].[dbo].[SalesQuotationD] where SalesQuotationNo = '" + tbxRefID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    int i = 0;
                    while (Dr.Read())
                    {
                        if (i >= 1)
                            itemRef += ", ";
                        itemRef += "'" + Dr["FullItemID"].ToString() + "'";
                        i++;
                    }
                }
                SearchV2 f = new SearchV2();
                f.SetMode("Check");
                f.SetSchemaTable("dbo", "InventTable", "and FullItemID in ( " + itemRef + " )");
                f.ShowDialog();
                GetDataHeader();
            }
        }

        private void btnAddCust_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("Check");
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
                f.SetSchemaTable("dbo", "CustMou_Dtl", "and [MouNo] = '" + tbxMoUID.Text + "' and CustID not in (" + CustID + ")");
            }
            else
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
                f.SetSchemaTable("dbo", "CustTable", "and CustID not in (" + CustID + ")");
            }
            f.ShowDialog();
            GetDataHeader2();
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
            if (cbType.Text == "AMOUNT" || cbType.Text == "FIX")
            {
                dataGridView1.Columns["Qty_Alt"].Visible = false;
                dataGridView1.Columns["Unit_Alt"].Visible = false;
                dataGridView1.Columns["ConvertionRatio"].Visible = false;
            }
            else
            {
                dataGridView1.Columns["Qty_Alt"].Visible = true;
                dataGridView1.Columns["Unit_Alt"].Visible = true;
                dataGridView1.Columns["ConvertionRatio"].Visible = true;
            }
            dataGridView1.Columns["Price_Alt"].Visible = false;
            dataGridView1.Columns["GelombangId"].Visible = false;
            dataGridView1.Columns["BracketId"].Visible = false;
            dataGridView1.Columns["Gelombang_Price"].Visible = false;
            dataGridView1.Columns["GelombangSeqNo_Base"].Visible = false;

            if (cbType.Text == "FIX")
            {
                dataGridView1.Columns["Base"].Visible = false;
                dataGridView1.Columns["Price"].Visible = true;
                dataGridView1.Columns["Qty"].Visible = true;
                dataGridView1.Columns["Unit"].Visible = true;
            }
            else if (cbType.Text == "QTY")
            {
                dataGridView1.Columns["Base"].Visible = true;
                dataGridView1.Columns["Price"].Visible = true;
                dataGridView1.Columns["Qty"].Visible = true;
                dataGridView1.Columns["Unit"].Visible = true;
            }
            else if (cbType.Text == "AMOUNT")
            {
                dataGridView1.Columns["Base"].Visible = true;
                dataGridView1.Columns["Qty"].Visible = false;
                dataGridView1.Columns["Unit"].Visible = false;
                dataGridView1.Columns["Price"].Visible = true;
            }

            if (Mode == "New" || Mode == "BeforeEdit")
            {
                dataGridView1.Columns["RefTransId"].Visible = false;
                dataGridView1.Columns["RefTrans_SeqNo"].Visible = false;
            }

            if (e.ColumnIndex == dataGridView1.Columns["Qty"].Index || e.ColumnIndex == dataGridView1.Columns["Qty_Alt"].Index || e.ColumnIndex == dataGridView1.Columns["ConvertionRatio"].Index || e.ColumnIndex == dataGridView1.Columns["DiscPercent"].Index || e.ColumnIndex == dataGridView1.Columns["ConvertionRatio"].Index)
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
                e.Value = d.ToString("N4");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("ExpectedDateTo"))
                dataGridView1.Columns["ExpectedDateTo"].DefaultCellStyle.Format = "dd/MM/yyyy";

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("ExpectedDateFrom"))
            {
                dataGridView1.Columns["ExpectedDateFrom"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            //if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date"))
            //    dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";

        }

        decimal price, qty, STotal, LogisticAmount, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, STotal_PPN, STotal_PPH, GTotal;
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (tableCols[e.ColumnIndex] == "Qty")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == null)
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value = "0";

                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value != null && dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value != null)
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Qty_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value) * Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value);
            }
            if (tableCols[e.ColumnIndex] == "Qty_Alt")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == null)
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value = "0";

                if (cbType.Text == "QTY")
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value = Math.Round(Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value));
                else
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value);

                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value != null && dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value != null && Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value) != 0)
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Qty"].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value) / Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value);
            }
            if (tableCols[e.ColumnIndex] == "Price")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value == String.Empty || dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value == null)
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value = "0";
                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == null)
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value = "0";

                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value != null && dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value != null)
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value);
            }
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
                if (!(dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y" && cbType.Text == "AMOUNT"))
                {
                    STotal = price * qty;
                    dataGridView1.Rows[i].Cells["SubTotal"].Value = STotal;
                }
                else
                    STotal = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value);

                STotal_PPN = STotal * Convert.ToDecimal(cbPPN.Text) / 100;
                dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = STotal_PPN;

                STotal_PPH = STotal * Convert.ToDecimal(cbPPh.Text) / 100;
                dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = STotal_PPH;

                if (dataGridView1.Rows[i].Cells["DiscAmount"].Value != null)
                {
                    if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Amount")
                    {
                        if (dataGridView1.Rows[i].Cells["DiscAmount"].Value == "" || dataGridView1.Rows[i].Cells["DiscAmount"].Value == null)
                            DiscAmount = 0;
                        else
                            DiscAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value);
                    }
                    else if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Percentage")
                    {
                        if (dataGridView1.Rows[i].Cells["DiscPercent"].Value == "" || dataGridView1.Rows[i].Cells["DiscPercent"].Value == null)
                            DiscPercent = 0;
                        else
                            DiscPercent = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscPercent"].Value);
                    }
                }

                if (Mode != "BeforeEdit")
                {
                    if (dataGridView1.Rows[i].Cells["DiscType"].Value != null)
                    {
                        if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Amount")
                        {
                            dataGridView1.Rows[i].Cells["DiscAmount"].ReadOnly = false;
                            dataGridView1.Rows[i].Cells["DiscAmount"].Style.BackColor = Color.White;
                            dataGridView1.Rows[i].Cells["DiscPercent"].ReadOnly = true;
                            dataGridView1.Rows[i].Cells["DiscPercent"].Style.BackColor = Color.LightGray;
                            if (STotal != 0 && DiscAmount != 0)
                                DiscPercent = DiscAmount / STotal * 100;
                            dataGridView1.Rows[i].Cells["DiscPercent"].Value = DiscPercent;
                        }
                        else if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Percentage")
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
            }

            tbxSTotal.Text = "0"; tbxGPPN.Text = "0"; tbxGPPh.Text = "0"; tbxGBonus.Text = "0"; tbxGCashback.Text = "0"; tbxGDisc.Text = "0"; tbxLogAmount.Text = "0";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                tbxSTotal.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value) + Convert.ToDecimal(tbxSTotal.Text)).ToString()));
                tbxGPPN.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value) + Convert.ToDecimal(tbxGPPN.Text)).ToString()));
                tbxGPPh.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value) + Convert.ToDecimal(tbxGPPh.Text)).ToString()));
                tbxGDisc.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value) + Convert.ToDecimal(tbxGDisc.Text)).ToString()));
                tbxLogAmount.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value) + Convert.ToDecimal(tbxLogAmount.Text)).ToString()));
                tbxGBonus.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["BonusAmount"].Value) + Convert.ToDecimal(tbxGBonus.Text)).ToString()));
                tbxGCashback.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["CashBackAmount"].Value) + Convert.ToDecimal(tbxGCashback.Text)).ToString()));
            }

            tbxGTotal.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(tbxSTotal.Text) + Convert.ToDecimal(tbxGPPN.Text) + Convert.ToDecimal(tbxGPPh.Text)).ToString()));
            //if (tableCols[e.ColumnIndex] == "Qty")
            //{
            //    if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == String.Empty)
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value = "0";

            //    if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value != null && dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value != null)
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Qty_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value) * Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value);
            //}

            //if (tableCols[e.ColumnIndex] == "Qty_Alt")
            //{
            //    if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == String.Empty || dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == null)
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value = "0";

            //    if (cbSAType.Text == "QTY")
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value = Math.Round(Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value));
            //    else
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value);

            //    if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value != null && dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value != null && Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value) != 0)
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Qty"].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[e.ColumnIndex].Value) / Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value);
            //}

            //if (tableCols[e.ColumnIndex] == "Price")
            //{
            //    if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value == String.Empty)
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value = "0";
            //    if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value == String.Empty)
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value = "0";

            //    if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value != null && dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value != null)
            //        dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["ConvertionRatio"].Value);
            //}
            //for (int i = 0; i < dataGridView1.RowCount; i++)
            //{
            //    STotal = 0; STotal_PPN = 0; STotal_PPH = 0; DiscAmount = 0; DiscPercent = 0;
            //    //PRICE
            //    if (dataGridView1.Rows[i].Cells["Price"].Value == "" || dataGridView1.Rows[i].Cells["Price"].Value == null)
            //        price = 0;
            //    else
            //        price = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value);
            //    //QTY
            //    if (dataGridView1.Rows[i].Cells["Qty"].Value == "" || dataGridView1.Rows[i].Cells["Qty"].Value == null)
            //        qty = 0;
            //    else
            //        qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);

            //    //WHEN SA TYPE AMOUNT 
            //    if (dataGridView1.Rows[i].Cells["Base"].Value == String.Empty || dataGridView1.Rows[i].Cells["Base"].Value == null)
            //        dataGridView1.Rows[i].Cells["Base"].Value = "";
            //    if (!(dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y" && cbSAType.Text == "AMOUNT"))
            //    {
            //        STotal = price * qty;
            //        dataGridView1.Rows[i].Cells["SubTotal"].Value = STotal;
            //    }
            //    else
            //        STotal = Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value);

            //    if (dataGridView1.Rows[i].Cells["DiscAmount"].Value != null)
            //    {
            //        if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Amount" || dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "1")
            //        {
            //            if (dataGridView1.Rows[i].Cells["DiscAmount"].Value == "" || dataGridView1.Rows[i].Cells["DiscAmount"].Value == null)
            //                DiscAmount = 0;
            //            else
            //                DiscAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value);
            //        }
            //        else if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Percentage" || dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "2")
            //        {
            //            if (dataGridView1.Rows[i].Cells["DiscPercent"].Value == "" || dataGridView1.Rows[i].Cells["DiscPercent"].Value == null)
            //                DiscPercent = 0;
            //            else
            //                DiscPercent = Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscPercent"].Value);
            //        }
            //    }

            //    STotal = price * qty;
            //    dataGridView1.Rows[i].Cells["SubTotal"].Value = STotal;

            //    STotal_PPN = STotal * Convert.ToDecimal(cbPPN.Text) / 100;
            //    dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = STotal_PPN;

            //    STotal_PPH = STotal * Convert.ToDecimal(cbPPh.Text) / 100;
            //    dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = STotal_PPH;

            //    if (Mode != "BeforeEdit")
            //    {
            //        if (dataGridView1.Rows[i].Cells["DiscType"].Value != null)
            //        {
            //            if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Amount")
            //            {
            //                dataGridView1.Rows[i].Cells["DiscAmount"].ReadOnly = false;
            //                dataGridView1.Rows[i].Cells["DiscAmount"].Style.BackColor = Color.White;
            //                dataGridView1.Rows[i].Cells["DiscPercent"].ReadOnly = true;
            //                dataGridView1.Rows[i].Cells["DiscPercent"].Style.BackColor = Color.LightGray;
            //                if (STotal != 0 && DiscAmount != 0)
            //                    DiscPercent = DiscAmount / STotal * 100;
            //                dataGridView1.Rows[i].Cells["DiscPercent"].Value = DiscPercent;
            //            }
            //            else if (dataGridView1.Rows[i].Cells["DiscType"].Value.ToString() == "Percentage")
            //            {
            //                dataGridView1.Rows[i].Cells["DiscAmount"].ReadOnly = true;
            //                dataGridView1.Rows[i].Cells["DiscAmount"].Style.BackColor = Color.LightGray;
            //                dataGridView1.Rows[i].Cells["DiscPercent"].ReadOnly = false;
            //                dataGridView1.Rows[i].Cells["DiscPercent"].Style.BackColor = Color.White;
            //                if (DiscPercent != 0)
            //                    DiscAmount = STotal * DiscPercent / 100;
            //                dataGridView1.Rows[i].Cells["DiscAmount"].Value = DiscAmount;
            //            }
            //            else
            //            {
            //                dataGridView1.Rows[i].Cells["DiscPercent"].Value = 0;
            //                dataGridView1.Rows[i].Cells["DiscAmount"].Value = 0;
            //                dataGridView1.Rows[i].Cells["DiscAmount"].ReadOnly = true;
            //                dataGridView1.Rows[i].Cells["DiscPercent"].ReadOnly = true;
            //                dataGridView1.Rows[i].Cells["DiscPercent"].Style.BackColor = Color.LightGray;
            //                dataGridView1.Rows[i].Cells["DiscAmount"].Style.BackColor = Color.LightGray;
            //            }
            //        }
            //    }
            //}

            //tbxSTotal.Text = "0"; tbxGPPN.Text = "0"; tbxGPPh.Text = "0"; tbxGBonus.Text = "0"; tbxGCashback.Text = "0"; tbxGDisc.Text = "0"; tbxLogAmount.Text = "0";
            //for (int i = 0; i < dataGridView1.RowCount; i++)
            //{
            //    tbxSTotal.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value) + Convert.ToDecimal(tbxSTotal.Text)).ToString()));
            //    tbxGPPN.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value) + Convert.ToDecimal(tbxGPPN.Text)).ToString()));
            //    tbxGPPh.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value) + Convert.ToDecimal(tbxGPPh.Text)).ToString()));
            //    tbxGDisc.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value) + Convert.ToDecimal(tbxGDisc.Text)).ToString()));
            //    tbxLogAmount.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["LogisticAmount"].Value) + Convert.ToDecimal(tbxLogAmount.Text)).ToString()));
            //    tbxGBonus.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["BonusAmount"].Value) + Convert.ToDecimal(tbxGBonus.Text)).ToString()));
            //    tbxGCashback.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["CashBackAmount"].Value) + Convert.ToDecimal(tbxGCashback.Text)).ToString()));
            //}

            //tbxGTotal.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(tbxSTotal.Text) + Convert.ToDecimal(tbxGPPN.Text) + Convert.ToDecimal(tbxGPPh.Text)).ToString()));
        }

        private void cbPPN_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = Convert.ToString(Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) * Convert.ToDecimal(cbPPN.Text) / 100);
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (cbPPN.Text != "0.00")
                    tbxGPPN.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(tbxGPPN.Text) + Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value)).ToString()));
                else
                {
                    tbxGPPN.Text = "0.0000";
                    break;
                }
            }
        }

        private void cbPPh_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells["Price"].Value == null || dataGridView1.Rows[i].Cells["Price"].Value == String.Empty)
                    dataGridView1.Rows[i].Cells["Price"].Value = 0;
                if (dataGridView1.Rows[i].Cells["Qty"].Value == null || dataGridView1.Rows[i].Cells["Qty"].Value == String.Empty)
                    dataGridView1.Rows[i].Cells["Qty"].Value = 0;
                dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = Convert.ToString(Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) * Convert.ToDecimal(cbPPh.Text) / 100);
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (cbPPh.Text != "0.00")
                    tbxGPPh.Text = string.Format("{0:#,0.0000}", double.Parse((Convert.ToDecimal(tbxGPPh.Text) + Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPh"].Value)).ToString()));
                else
                {
                    tbxGPPh.Text = "0.0000";
                    break;
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex == dataGridView1.Columns["ExpectedDateFrom"].Index || e.ColumnIndex == dataGridView1.Columns["ExpectedDateTo"].Index)
            //{
            //    Rectangle cellRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            //    _dateTimePicker.Location = cellRectangle.Location;
            //    _dateTimePicker.Width = cellRectangle.Width;
            //    DateTime date1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 01, 00, 00);
            //    _dateTimePicker.MinDate = date1;
            //    try
            //    {
            //        _dateTimePicker.Value = DateTime.Parse(dataGridView1.CurrentCell.Value.ToString());

            //    }
            //    catch
            //    {
            //    }
            //    _dateTimePicker.Visible = true;
            //}
            //else
            //{
            //    _dateTimePicker.Visible = false;
            //}

            //dtp = new DateTimePicker();

        }

        //void cellDateTimePickerValueChanged(object sender, EventArgs e)
        //{
        //    dataGridView1.CurrentCell.Value = _dateTimePicker.Value.ToString("dd/MM/yyyy");
        //    _dateTimePicker.Visible = false;
        //}

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            //_dateTimePicker.Visible = false;
            dtp.Visible = false;
        }

        private void cbDPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDPType.Text == "Y")
            {
                dtDP.Visible = true;
                dtDP.Enabled = true;
                cbDPType.BackColor = Color.Empty;
                tbxDP.Visible = true;
                lblDueDate.Visible = true;
                label7.Visible = true;
            }
            else
            {
                dtDP.Visible = false;
                if (cbDPType.Text == "N")
                {
                    cbDPType.BackColor = Color.Empty;
                    tbxDP.Visible = false;
                    tbxDP.Text = "0";
                    lblDueDate.Visible = false;
                    label7.Visible = false;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";
            GetDataHeader();
            GetDataHeader2();
            ModeBeforeEdit();
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void tbxDP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '%'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '%') && ((sender as TextBox).Text.IndexOf('%') > -1))
            {
                e.Handled = true;
            }
        }

        private void tbxToP_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbxXRate_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("Select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() == "22")
            {
                Query = "Update [ISBS-NEW4].[dbo].[SalesQuotationH] SET [TransStatus] = '23',[UpdatedDate] = getdate(),[UpdatedBy] = '" + Login.Username + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
            }
            else if (Cmd.ExecuteScalar().ToString() == "02")
            {
                Query = "Update [ISBS-NEW4].[dbo].[SalesQuotationH] SET [TransStatus] = '03',[UpdatedDate] = getdate(),[UpdatedBy] = '" + Login.Username + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            //BEGIN STEVEN EDIT-------------
            insertStatusLogApprove();
            //END STEVEN EDIT-----------------
            MessageBox.Show(tbxSQID.Text + " approved!", "Information");
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnRevision_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Update [dbo].[SalesQuotationH] SET [TransStatus] = '05',[UpdatedDate] = getdate(),[UpdatedBy] = '" + Login.Username + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            //BEGIN STEVEN EDIT-----------
            insertStatusLogRevision();
            //END STEVEN EDIT-------------
            MessageBox.Show(tbxSQID.Text + " will be revised!", "Information");
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Update [dbo].[SalesQuotationH] SET [TransStatus] = '04',[UpdatedDate] = getdate(),[UpdatedBy] = '" + Login.Username + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            //BEGIN STEVEN EDIT---------
            insertStatusLogReject();
            //END STEVEN EDIT-----------
            MessageBox.Show(tbxSQID.Text + " rejected!", "Information");
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnAmend_Click(object sender, EventArgs e)
        {
            ModeEdit();
            GetDataHeader();
            tbxRefID.Text = tbxSQID.Text;
            tbxSQID.Text = "";
            Mode = "New";
            dataGridView1.Rows.Clear();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ColumnCount = tableCols.Length;
            for (int i = 0; i < tableCols.Length; i++)
            {
                dataGridView1.Columns[i].Name = tableCols[i];
            }
            Query = "select * from [ISBS-NEW4].[dbo].[SalesQuotationD] where SalesQuotationNo = '" + tbxRefID.Text + "' order by SeqNo asc; ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int no = 0;
            while (Dr.Read())
            {
                dataGridView1.Rows.Add(1);
                for (int i = 0; i < tableCols.Length; i++)
                {
                    if (tableCols[i] == "No")
                        dataGridView1.Rows[no].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                    else if (tableCols[i] == "DeliveryMethod")
                    {
                        cellValue("Select [DeliveryMethod] from [dbo].[DeliveryMethod]");
                        if (Dr[tableCols[i]] != null)
                            cell.Value = Dr[tableCols[i]].ToString();
                        dataGridView1.Rows[no].Cells[tableCols[i]] = cell;
                    }
                    else if (tableCols[i] == "DiscType")
                    {
                        cellValue("Select [Deskripsi] from [dbo].[DiskonScheme]");
                        Query = "Select [Deskripsi] from [dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr[tableCols[i]] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        if (Dr[tableCols[i]] != null)
                            cell.Value = Cmd.ExecuteScalar().ToString();
                        dataGridView1.Rows[no].Cells[tableCols[i]] = cell;
                    }
                    else if (tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo")
                    {
                        if (Dr["Base"].ToString() != "N")
                            dataGridView1.Rows[no].Cells[tableCols[i]].Value = Convert.ToDateTime(Dr[tableCols[i]]);
                    }
                    else if (tableCols[i] == "RefTransId")
                        dataGridView1.Rows[no].Cells[tableCols[i]].Value = Dr["SalesQuotationNo"].ToString();
                    else if (tableCols[i] == "RefTrans_SeqNo")
                        dataGridView1.Rows[no].Cells[tableCols[i]].Value = Dr["SeqNo"].ToString();
                    else
                        dataGridView1.Rows[no].Cells[tableCols[i]].Value = Dr[tableCols[i]].ToString();
                }
                no++;
            }

            //Mode = "New";
            //SQHeader_Load(new object(), new EventArgs());
            //SQID = "";
            //tbxRefID.Text = tbxSQID.Text;

            //tbxSQID.Text = "";
            //dataGridView1.Rows.Clear(); dataGridView1.Columns.Clear();
            //dataGridView2.Rows.Clear(); dataGridView2.Columns.Clear();
            //tbxMoUID.Text = ""; tbxToP.Text = ""; tbxXRate.Text = ""; tbxFileDialog.Text = "";
            //tbxSTotal.Text = "0"; tbxGPPN.Text = "0"; tbxGPPh.Text = "0"; tbxGDisc.Text = "0";
            //tbxGTotal.Text = "0"; tbxGBonus.Text = "0"; tbxGCashback.Text = "0"; tbxDP.Text = "0";
            //tbxLogAmount.Text = "0";
            //tbxNotes.Text = "";
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            {
                if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                {
                    PopUp.Stock.Stock f = new PopUp.Stock.Stock();
                    itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                    f.Show();
                }
                if (Mode != "BeforeEdit")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Qty")
                    {
                        SearchV2 f = new SearchV2();
                        f.SetMode("No");
                        f.SetSchemaTable("dbo", "Invent_OnHand_Qty", "and FullItemID = '" + dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'");
                        f.ShowDialog();
                    }
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Price")
                    {
                        SearchV2 f = new SearchV2();
                        f.SetMode("No");
                        f.SetSchemaTable("dbo", "SalesPriceListDtl", "and FullItemID = '" + dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'");
                        f.ShowDialog();
                    }
                }
            }
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
        }

        private void btnDeleteCust_Click(object sender, EventArgs e)
        {
            if (dataGridView2.RowCount != 0)
                dataGridView2.Rows.RemoveAt(dataGridView2.CurrentRow.Index);
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

        private void cbPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbPPh_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbPaymentMode_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void tbxDP_Leave(object sender, EventArgs e)
        {
            if (!(tbxDP.Text.Contains('%')))
                tbxDP.Text = string.Format("{0:#,0.0000}", double.Parse(tbxDP.Text));
        }

        private void tbxXRate_Leave(object sender, EventArgs e)
        {
            if (tbxXRate.Text == String.Empty)
                tbxXRate.Text = "0";
            tbxXRate.Text = string.Format("{0:#,0.0000}", double.Parse(tbxXRate.Text));
        }

        private void cbCurrency_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbCurrency.Text == "IDR")
            {
                tbxXRate.Text = "1.0000";
                tbxXRate.Enabled = false;
            }
            else
            {
                tbxXRate.Enabled = true;
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
            {
                dtp.Location = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                dtp.Visible = true;

                if (dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
                {
                    DateTime dDate;
                    if (!DateTime.TryParse(dataGridView1.CurrentCell.Value.ToString(), out dDate))
                    {
                        dtp.Value = Convert.ToDateTime(FormateDateddmmyyyy(dataGridView1.CurrentCell.Value.ToString()));
                    }
                    else
                    {
                        dtp.Value = Convert.ToDateTime(dataGridView1.CurrentCell.Value);
                        //dtp.Value = DateTime.ParseExact(dataGridView1.CurrentCell.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
        }

        private string FormateDateddmmyyyy(string tmpDate)
        {
            //string reformat="";
            string[] data = tmpDate.Split('/');
            return data[2] + "/" + data[1] + "/" + data[0];
        }

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
                    //dataGridView1.CurrentCell.Value = dtp.Value.Date.ToString("dd/MM/yyyy");
                    dataGridView1.CurrentCell.Value = Convert.ToDateTime(dtp.Value);
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
            dtp.Visible = false;
            dataGridView1.AutoResizeColumns();
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void cbSAType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void cbSAType_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
