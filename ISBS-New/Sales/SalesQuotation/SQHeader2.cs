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
using System.Text.RegularExpressions;

//BY: HC
namespace ISBS_New.Sales.SalesQuotation
{
    public partial class SQHeader2 : MetroFramework.Forms.MetroForm
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
        private TransactionScope scope;
        /**********SQL*********/

        /**********STANDARD*********/
        private string Mode;
        private string SQID;
        /**********STANDARD*********/

        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "No", "SeqNo", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "RefTransId", "RefTrans_SeqNo", "Base", "Qty", "Unit", "Price", "Qty_Alt", "Unit_Alt", "Price_Alt", "ConvertionRatio", "SubTotal", "DiscType", "DiscPercent", "DiscAmount", "SubTotal_PPN", "SubTotal_PPH", "Notes", "GelombangId", "BracketId", "Gelombang_Price", "GelombangSeqNo_Base", "PLJNo", "PLJSeqNo", "PLJPrice" };

        string[] tableCols2 = new string[] { "No", "FullItemID", "ItemName", "DeliveryMethod", "ExpectedDateFrom", "ExpectedDateTo", "LogisticAmount", "LogisticNotes", "BonusAmount", "CashBackAmount" };
        /*********datagridview cols name*********/

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

        List<string> pricelistDtl = new List<string>();
        ContextMenu vendid = new ContextMenu();

        string oldSAType; //CEK KALAU ADA PERUBAHAN VALUE CHECKBOX SALES TYPE

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public SQHeader2()
        {
            InitializeComponent();
        }
        private void SQHeader2_Load(object sender, EventArgs e)
        {
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy";
            dtp.Visible = false;
            dtp.Width = 100;

            dataGridView2.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;
            dataGridView2.CellBeginEdit += this.dataGridView2_CellBeginEdit;
            dataGridView2.CellEndEdit += this.dataGridView2_CellEndEdit;

            Conn = ConnectionString.GetConnection();
            cbCurrency.Items.Clear();
            Cmd = new SqlCommand("select CurrencyID from [ISBS-NEW4].[dbo].[CurrencyTable] order by CurrencyID asc", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbCurrency.Items.Add(Dr[0]);
            }
            Dr.Close();

            cbPaymentMode.Items.Clear();
            cbPaymentMode.Items.Add("Select");
            Cmd = new SqlCommand("select [PaymentModeName] from [ISBS-NEW4].[dbo].[PaymentMode] order by PaymentModeName", Conn);
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

                ModeNew();

                cbCurrency.Text = "IDR";
                cbPaymentMode.SelectedIndex = 0;
                cbDPType.SelectedIndex = 1;
                cbPPh.SelectedIndex = 0;
                cbPPN.SelectedIndex = 1;
                cbToP.SelectedIndex = 0;

                oldSAType = cbType.Text;
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
            //tia edit
            else if (Mode == "PopUp")
            {
                ModePopUp();
                GetDataHeader();

            }
            //tia edit end
            metroTabControl1.SelectedTab = tabSales;
        }

        //tia edit
        private void ModePopUp()
        {
            Mode = "PopUp";
            btnMoUID.Enabled = false;
            cbType.Enabled = false;
            cbDPType.Enabled = false; cbCurrency.Enabled = false;
            tbxDPPercent.Enabled = false;
            tbxDPAmount.Enabled = false;
            dtDP.Enabled = false;
            cbToP.Enabled = false;
            dtSQ.Enabled = false;
            tbxXRate.Enabled = false;
            tbxCustID.Visible = true; tbxCustName.Visible = true;
            lblCustID.Visible = true; lblCustName.Visible = false;
            dtValidTo.Enabled = false; dtValidFrom.Enabled = false;

            btnAddItem.Enabled = false; btnDeleteItem.Enabled = false;
            btnAddCust.Enabled = false; btnDelCust.Enabled = false;
            btnAddCust.Visible = false; btnDelCust.Visible = false;
            listBox1.Visible = false; label6.Visible = false;

            cbPPh.Enabled = false; cbPPN.Enabled = false;
            tbxNotes.Enabled = false;
            cbPaymentMode.Enabled = false;

            btnEdit.Enabled = true; btnCancel.Enabled = false; btnSave.Enabled = false;
            //tia edit
            tbxCustID.ReadOnly = false;
            tbxCustName.Enabled = true;
            tbxCustID.ReadOnly = true;
            tbxCustName.ReadOnly = true;
            tbxMoUID.Enabled = true;
            tbxMoUID.ReadOnly = true;
            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            tbxMoUID.ContextMenu = vendid;
            //tia end

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn);
            string transStatus = Cmd.ExecuteScalar().ToString();
            Conn.Close();

            if (transStatus == "03")
                btnApprove.Text = "UnApprove";
            else
                btnApprove.Text = "Approve";
            if (transStatus == "01")
            {
                btnEdit.Enabled = true;
                btnApprove.Visible = false; btnApprove.Enabled = false;
                btnReject.Visible = false; btnReject.Enabled = false;
                btnAmend.Visible = true; btnAmend.Enabled = true;
            }
            else if (transStatus == "02" || transStatus == "22")
            {
                btnEdit.Enabled = true;
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Administrator")
                {
                    btnApprove.Visible = true; btnApprove.Enabled = true;
                    btnReject.Visible = true; btnReject.Enabled = true;
                    btnAmend.Visible = false; btnAmend.Enabled = false;
                }
                else
                {
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnReject.Visible = false; btnReject.Enabled = false;
                    btnAmend.Visible = false; btnAmend.Enabled = false;
                }
            }
            else if (transStatus == "03" || transStatus == "23")
            {
                btnEdit.Enabled = false;
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Administrator")
                {
                    btnAmend.Visible = true; btnAmend.Enabled = true;
                    btnApprove.Visible = true; btnApprove.Enabled = true;
                    btnReject.Visible = false; btnReject.Enabled = false;
                }
                else if (ControlMgr.GroupName == "Sales Admin")
                {
                    btnAmend.Visible = true; btnAmend.Enabled = true;
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnReject.Visible = false; btnReject.Enabled = false;
                }
            }
            else if (transStatus == "04")
            {
                btnEdit.Enabled = false;
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Administrator")
                {
                    btnApprove.Visible = true; btnApprove.Enabled = true;
                    btnAmend.Visible = true; btnAmend.Enabled = true;
                    btnReject.Visible = false; btnReject.Visible = false;
                }
                else
                {
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnAmend.Visible = true; btnAmend.Enabled = true;
                    btnReject.Visible = false; btnReject.Visible = false;
                }
            }
            else if (transStatus == "07")
            {
                btnEdit.Enabled = false;
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Sales Admin" || ControlMgr.GroupName == "Administrator")
                {
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnAmend.Visible = true; btnAmend.Enabled = true;
                    btnReject.Visible = false; btnReject.Visible = false;
                }
                else
                {
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnAmend.Visible = false; btnAmend.Enabled = false;
                    btnReject.Visible = false; btnReject.Visible = false;
                }
            }
            else if (transStatus == "21" || transStatus == "11")
            {
                btnEdit.Enabled = false;
                btnAmend.Visible = false; btnAmend.Enabled = false;
                btnApprove.Visible = false; btnApprove.Enabled = false;
                btnReject.Visible = false; btnReject.Enabled = false;
            }
            else
            {
                btnApprove.Visible = false; /*btnRevision.Visible = false;*/ btnReject.Visible = false; btnAmend.Visible = false;
            }
        }
        //tia edit end

        public void GetDataHeader()
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                if (Mode != "New")
                {
                    dataGridView1.Columns.Clear();
                    dataGridView1.Rows.Clear();
                    dataGridView2.Columns.Clear();
                    dataGridView2.Rows.Clear();

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
                        tbxXRate.Text = string.Format("{0:#,0.00}", double.Parse(Dr["ExchRate"].ToString()));
                        tbxSTotal.Text = Dr["Total"].ToString();
                        tbxGDisc.Text = Dr["Total_Disk"].ToString();
                        cbPPN.Text = Dr["PPN"].ToString();
                        tbxGPPN.Text = Dr["Total_PPN"].ToString();
                        cbPPh.Text = Dr["PPH"].ToString();
                        tbxGPPh.Text = Dr["Total_PPH"].ToString();
                        tbxGTotal.Text = Dr["Total_Nett"].ToString();
                        tbxGBonus.Text = Dr["Total_Bonus"].ToString();
                        tbxGCashback.Text = Dr["Total_Cashback"].ToString();
                        cbToP.Text = Dr["TermofPayment"].ToString();
                        Cmd = new SqlCommand("Select PaymentModeName from [ISBS-NEW4].[dbo].[PaymentMode] where PaymentModeID = @PaymentModeID", Conn);
                        Cmd.Parameters.AddWithValue("@PaymentModeID", Dr["PaymentModeID"]);
                        cbPaymentMode.Text = Cmd.ExecuteScalar().ToString();
                        cbDPType.Text = Dr["DPType"].ToString();
                        if (Dr["DPType"].ToString() == "Y")
                            dtDP.Value = Convert.ToDateTime(Dr["DPDueDate"]);
                        tbxNotes.Text = Dr["Notes"].ToString();
                        cbType.Text = Dr["TransType"].ToString();
                        tbxCustID.Text = Dr["CustID"].ToString();
                        tbxCustName.Text = Dr["CustName"].ToString();
                        if (Dr["DPDueDate"] != (object)DBNull.Value)
                            dtDP.Value = Convert.ToDateTime(Dr["DPDueDate"]);
                        dtValidFrom.Value = Convert.ToDateTime(Dr["ValidFrom"]);
                        dtValidTo.Value = Convert.ToDateTime(Dr["ValidTo"]);
                    }
                    Dr.Close();

                    dataGridView1.AllowUserToAddRows = false;
                    if (dataGridView1.RowCount - 1 <= 0)
                    {
                        dataGridView1.ColumnCount = tableCols.Length;
                        for (int i = 0; i < tableCols.Length; i++)
                        {
                            dataGridView1.Columns[i].Name = tableCols[i];
                        }
                    }

                    if (dataGridView2.RowCount - 1 <= 0)
                    {
                        dataGridView2.ColumnCount = tableCols2.Length;
                        for (int i = 0; i < tableCols2.Length; i++)
                        {
                            dataGridView2.Columns[i].Name = tableCols2[i];
                        }
                    }

                    Query = "select * from [ISBS-NEW4].[dbo].[SalesQuotationD] where SalesQuotationNo = '" + tbxSQID.Text + "' and Deleted = 'N' order by SeqNo asc; ";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    int no = 1;
                    while (Dr.Read())
                    {
                        dataGridView1.Rows.Add(1);
                        dataGridView1.Rows[no - 1].Cells[0].Value = dataGridView1.Rows.Count;
                        for (int i = 1; i < tableCols.Length; i++)
                        {
                            if (tableCols[i] != "DiscType")
                            {
                                if (tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo")
                                {
                                    if (Dr[tableCols[i]] != (object)DBNull.Value)
                                        dataGridView1.Rows[no - 1].Cells[i].Value = Convert.ToDateTime(Dr[tableCols[i]]);
                                }
                                else
                                    dataGridView1.Rows[no - 1].Cells[i].Value = Dr[tableCols[i]].ToString();
                            }
                        }
                        if (Mode == "Edit")
                        {
                            if (Dr["Base"].ToString() != "N")
                            {
                                cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                                Query = "Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr["DiscType"] + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                if (!(Dr["DiscType"] == null || Dr["DiscType"] == (object)DBNull.Value))
                                    cell.Value = Cmd.ExecuteScalar().ToString();
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                            }
                        }
                        else
                        {
                            if (Dr["Base"].ToString() != "N")
                            {
                                Query = "Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr["DiscType"] + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                if (Cmd.ExecuteScalar() != null)
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"].Value = Cmd.ExecuteScalar().ToString();
                            }
                        }

                        dataGridView2.Rows.Add(1);
                        dataGridView2.Rows[no - 1].Cells[0].Value = dataGridView2.Rows.Count;
                        for (int i = 1; i < tableCols2.Length; i++)
                        {
                            if (tableCols2[i] == "ExpectedDateFrom" || tableCols2[i] == "ExpectedDateTo")
                            {
                                if (Dr[tableCols2[i]] != (object)DBNull.Value)
                                    dataGridView2.Rows[no - 1].Cells[i].Value = Convert.ToDateTime(Dr[tableCols2[i]]);
                            }
                            else
                                dataGridView2.Rows[no - 1].Cells[i].Value = Dr[tableCols2[i]].ToString();
                        }
                        no++;
                    }
                    Dr.Close();

                    Query = "Select DPAmount, DPPercent from [ISBS-NEW4].[dbo].[SalesQuotationH] where SalesQuotationNo = '" + tbxSQID.Text + "'; ";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        tbxDPAmount.Text = string.Format("{0:#,0.00}", double.Parse(Dr["DPAmount"].ToString()));
                        tbxDPPercent.Text = string.Format("{0:#,0.00}", double.Parse(Dr["DPPercent"].ToString()));
                    }
                    Dr.Close();
                }
                gvFormat();
            }
            //catch (Exception ex)
            //{
            //    MetroFramework.MetroMessageBox.Show(this, ex.Message, "System failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            finally { }
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

        public void SetMode(string tmpMode, string tmpNumber)
        {
            Mode = tmpMode;
            tbxSQID.Text = tmpNumber;
            SQID = tmpNumber;
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

        private char Validation()
        {
            try
            {
                flag = '\0';
                msg = "";
                string msg2 = null;
                string msg3 = null;
                if (validate == false)
                {
                    label = new Label[20];
                }

                createLabel(cbDPType, lblDPType, tabSales, "string");
                createLabel(cbCurrency, lblCurrency, tabSales, "string");
                createLabel(cbToP, lblToP, tabSales, "string");
                createLabel(tbxXRate, lblXRate, tabSales, "decimal");
                createLabel(cbPaymentMode, lblPaymentMode, tabSales, "string");
                if (cbDPType.Text == "Y")
                {
                    createLabel(tbxDPPercent, label7, tabSales, "decimal");
                    createLabel(tbxDPAmount, label7, tabSales, "decimal");
                }

                if (flag == 'X')
                    msg2 += "* field is required!\r\n";

                int result = DateTime.Compare(Convert.ToDateTime(dtValidFrom.Value.ToShortDateString()), Convert.ToDateTime(dtValidTo.Value.ToShortDateString()));
                if (result < 0) { }//is earlier than
                else if (result == 0) { } //is the same time as
                else //is later than
                    msg2 += "Valid To (" + Convert.ToDateTime(dtValidTo.Value).ToString("dd/MM/yyyy") + ") must be later than Valid From (" + Convert.ToDateTime(dtValidFrom.Value).ToString("dd/MM/yyyy") + ")\n";

                if (dataGridView1.Rows.Count == 0)
                    msg += "Please add Item!\n";

                if (Mode == "New" && tbxRefID.Text == String.Empty)
                {
                    //if (listBox1.Items.Count == 0)
                    if (tbxCustID.Text == "")
                        msg2 += "Please add Customer!\n";
                }

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() != "N")
                    {
                        if (cbType.Text != "AMOUNT")
                            if (dataGridView1.Rows[i].Cells["Qty"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) == 0)
                                msg += "Row " + Convert.ToInt32(i + 1) + " quantity cannot 0!\r\n";

                        if (dataGridView1.Rows[i].Cells["Price"].Value == String.Empty || dataGridView1.Rows[i].Cells["Price"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) == 0)
                            msg += "Row " + Convert.ToInt32(i + 1) + " price cannot 0!\r\n";

                        if (dataGridView2.Rows[i].Cells["DeliveryMethod"].Value.ToString() == "Select")
                        {
                            msg += "Row " + Convert.ToInt32(i + 1) + " Select Delivery Method!\n";
                        }

                        if (dataGridView2.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView2.Rows[i].Cells["ExpectedDateFrom"].Value == String.Empty)
                            msg += "Row " + Convert.ToInt32(i + 1) + " Fill Delivery Expected Date From!\n";
                        if (dataGridView2.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView2.Rows[i].Cells["ExpectedDateTo"].Value == String.Empty)
                            msg += "Row " + Convert.ToInt32(i + 1) + " Fill Delivery Expected Date To!\n";

                        if (!(dataGridView2.Rows[i].Cells["ExpectedDateFrom"].Value == null || dataGridView2.Rows[i].Cells["ExpectedDateFrom"].Value == String.Empty || dataGridView2.Rows[i].Cells["ExpectedDateTo"].Value == null || dataGridView2.Rows[i].Cells["ExpectedDateTo"].Value == String.Empty))
                        {
                            result = DateTime.Compare(Convert.ToDateTime(Convert.ToDateTime(dataGridView2.Rows[i].Cells["ExpectedDateFrom"].Value).ToShortDateString()), Convert.ToDateTime(Convert.ToDateTime(dataGridView2.Rows[i].Cells["ExpectedDateTo"].Value).ToShortDateString()));

                            if (result < 0) { }//is earlier than
                            else if (result == 0) { } //is the same time as
                            else //is later than
                                msg += "Row " + Convert.ToInt32(i + 1) + " Delivery Expected Date To (" + Convert.ToDateTime(dataGridView2.Rows[i].Cells["ExpectedDateTo"].Value).ToString("dd/MM/yyyy") + ") must be later than Date From (" + Convert.ToDateTime(dataGridView2.Rows[i].Cells["ExpectedDateFrom"].Value).ToString("dd/MM/yyyy") + ")\n";
                        }
                        if (dataGridView2.Rows[i].Cells["DeliveryMethod"].Value.ToString().ToUpper() == "FRANCO")
                        {
                            if (dataGridView2.Rows[i].Cells["LogisticAmount"].Value == String.Empty || dataGridView2.Rows[i].Cells["LogisticAmount"].Value == null || Convert.ToDecimal(dataGridView2.Rows[i].Cells["LogisticAmount"].Value) == 0)
                            msg += "Row " + Convert.ToInt32(i + 1) + " Logistik Amount tidak boleh 0 karena Franco!\n";
                        }
                    }
                }

                if (!(msg2 == null || msg2 == String.Empty))
                    metroTabControl1.SelectedTab = tabSales;
                else if (!(msg == null || msg == String.Empty))
                    metroTabControl1.SelectedTab = tabItem;
                //else if (!(msg3 == null || msg3 == String.Empty))
                //    metroTabControl1.SelectedTab = tabDocuments;

                if (!(String.IsNullOrEmpty(msg)) || !(String.IsNullOrEmpty(msg2))/* || !(String.IsNullOrEmpty(msg3))*/)
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
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return flag;
            }
        }

        private void ModeEdit()
        {
            Mode = "Edit";

            DateTime date1 = new DateTime(dtSQ.Value.Year, dtSQ.Value.Month, dtSQ.Value.Day, 00, 00, 00);
            //DateTime date1 = new DateTime(dtSQ.Value.Year, dtSQ.Value.Day, dtSQ.Value.Month, 00, 00, 00);            

            dtDP.MinDate = date1;
            dtValidFrom.MinDate = date1;
            dtValidTo.MinDate = date1;
            btnSCust.Enabled = true;

            btnMoUID.Enabled = false; cbDPType.Enabled = true;
            cbCurrency.Enabled = true;
            if (cbCurrency.Text == "IDR")
                tbxXRate.Enabled = false;
            else
                tbxXRate.Enabled = true;
            cbToP.Enabled = true;

            dtValidFrom.Enabled = true; dtValidTo.Enabled = true;

            btnAddItem.Enabled = true; btnDeleteItem.Enabled = true;

            cbPPh.Enabled = true; cbPPN.Enabled = true;
            cbPaymentMode.Enabled = true;

            tbxNotes.Enabled = true;

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select DP_Required from CustTable where CustId = '" + tbxCustID.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() == "Y")
            {
                cbDPType.Enabled = false;
            }
            else
                cbDPType.Enabled = true;
            if (cbDPType.Text == "Y")
            {
                //cbDPType.Enabled = true;
                tbxDPPercent.Enabled = true;
                tbxDPAmount.Enabled = true;
                dtDP.Enabled = true;
                //hendry tbxDPPercent.Visible = true; tbxDPAmount.Visible = true; label4.Visible = true;dtDP.Visible = true; 
            }
            else
            {
                //cbDPType.Enabled = false;
                tbxDPPercent.Enabled = false;
                tbxDPAmount.Enabled = false;
                dtDP.Enabled = false;
                //hendry tbxDPPercent.Visible = false; tbxDPAmount.Visible = false; label4.Visible = false; dtDP.Visible = false;
            }

            btnEdit.Enabled = false; btnCancel.Enabled = true; btnSave.Enabled = true;

            if (/*btnRevision.Visible == true || */btnApprove.Visible == true || btnReject.Visible == true || btnAmend.Visible == true)
            {
                /*btnRevision.Visible = false; */
                btnApprove.Visible = false; btnReject.Visible = false; btnAmend.Visible = false;
            }
        }

        private void ModeNew()
        {
            Mode = "New";
            dtSQ.Value = DateTime.Now;
            DateTime date1 = new DateTime(dtSQ.Value.Year, dtSQ.Value.Month, dtSQ.Value.Day, 00, 00, 00);
            dtDP.MinDate = date1;
            dtValidFrom.MinDate = date1;
            dtValidTo.MinDate = date1;
            dtDP.Value = DateTime.Now;

            btnMoUID.Enabled = true; cbDPType.Enabled = true;
            cbCurrency.Enabled = true; cbToP.Enabled = true;
            btnAddItem.Enabled = true; btnDeleteItem.Enabled = true;
            cbPPh.Enabled = true; cbPPN.Enabled = true; cbPaymentMode.Enabled = true;
            btnSCust.Enabled = true;

            tbxCustID.ReadOnly = false;
            tbxCustName.Enabled = true;
            tbxCustID.ReadOnly = true;
            tbxCustName.ReadOnly = true;
            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            //if (cbDPType.Text == "Y") //hendry
            //{
            //    tbxDPPercent.Visible = true;
            //    tbxDPAmount.Visible = true;
            //    label4.Visible = true;
            //    dtDP.Visible = true;
            //}
            //else
            //{
            //    tbxDPPercent.Visible = false;
            //    tbxDPAmount.Visible = false;
            //    label4.Visible = false;
            //    dtDP.Visible = false;
            //}

            if (listBox1.Items.Count != 0)
            {
                Conn = ConnectionString.GetConnection();
                Query = "select DP_Required from CustTable where CustId = '" + listBox1.Items[0].ToString().Split(' ')[0] + "'";
                Cmd = new SqlCommand(Query, Conn);
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
            }

            tbxNotes.Enabled = true;
            btnApprove.Visible = false; btnReject.Visible = false;
            //btnRevision.Visible = false; 
            btnAmend.Visible = false;
            btnEdit.Enabled = false; btnCancel.Enabled = false; btnSave.Enabled = true;
        }

        private void gvFormat()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            if (Mode != "BeforeEdit")
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    if (!(tableCols[i] == "No" || tableCols[i] == "SeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "FullItemID" || tableCols[i] == "ItemName" || tableCols[i] == "Unit" || tableCols[i] == "Qty_Alt" || tableCols[i] == "SubTotal" || tableCols[i] == "SubTotal_PPN" || tableCols[i] == "SubTotal_PPH" || tableCols[i] == "DiscPercent" || tableCols[i] == "DiscAmount" || tableCols[i] == "Base" || tableCols[i] == "Unit_Alt" || tableCols[i] == "ConvertionRatio" || tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo" || tableCols[i] == "Price_Alt" || tableCols[i] == "GelombangId" || tableCols[i] == "BracketId" || tableCols[i] == "Gelombang_Price" || tableCols[i] == "GelombangSeqNo_Base" || tableCols[i] == "PLJNo" || tableCols[i] == "PLJSeqNo" || tableCols[i] == "PLJPrice"))
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

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        if (dataGridView1.Rows[i].Cells["Base"].Value == null || dataGridView1.Rows[i].Cells["Base"].Value == String.Empty)
                            dataGridView1.Rows[i].Cells["Base"].Value = "";
                        if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "Y" && cbType.Text == "QUANTITY")
                        {
                            dataGridView1.Rows[i].Cells["Qty_Alt"].ReadOnly = false;
                            dataGridView1.Rows[i].Cells["Qty_Alt"].Style.BackColor = Color.White;
                            dataGridView1.Rows[i].Cells["Qty"].ReadOnly = true;
                            dataGridView1.Rows[i].Cells["Qty"].Style.BackColor = Color.LightGray;
                            dataGridView1.Rows[i].Cells["Price"].ReadOnly = false;
                            dataGridView1.Rows[i].Cells["Price"].Style.BackColor = Color.White;
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
                    if (!(tableCols2[i] == "No" || tableCols2[i] == "FullItemID" || tableCols2[i] == "ItemName" || tableCols2[i] == "DeliveryMethod"))
                    {
                        dataGridView2.Columns[i].ReadOnly = false;
                        dataGridView2.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dataGridView2.Columns[i].ReadOnly = true;
                        dataGridView2.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView2.ColumnCount; j++)
                    {
                        if (dataGridView1.Rows[i].Cells["Base"].Value == null || dataGridView1.Rows[i].Cells["Base"].Value == String.Empty)
                            dataGridView1.Rows[i].Cells["Base"].Value = "";

                        if (dataGridView1.Rows[i].Cells["Base"].Value.ToString() == "N")
                        {
                            dataGridView2.Rows[i].Cells[j].ReadOnly = true;
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                        }
                        else
                            break;
                    }
                }
            }
            else
            {
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
            }
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            btnMoUID.Enabled = false;
            cbType.Enabled = false;
            cbDPType.Enabled = false; cbCurrency.Enabled = false;
            tbxDPPercent.Enabled = false;
            tbxDPAmount.Enabled = false;
            dtDP.Enabled = false;
            cbToP.Enabled = false;
            dtSQ.Enabled = false;
            tbxXRate.Enabled = false;
            tbxCustID.Visible = true; tbxCustName.Visible = true; btnSCust.Enabled = false;
            lblCustID.Visible = true; lblCustName.Visible = false;
            dtValidTo.Enabled = false; dtValidFrom.Enabled = false;

            btnAddItem.Enabled = false; btnDeleteItem.Enabled = false;
            btnAddCust.Enabled = false; btnDelCust.Enabled = false;
            btnAddCust.Visible = false; btnDelCust.Visible = false;
            listBox1.Visible = false; label6.Visible = false;

            cbPPh.Enabled = false; cbPPN.Enabled = false;
            tbxNotes.Enabled = false;
            cbPaymentMode.Enabled = false;

            btnEdit.Enabled = true; btnCancel.Enabled = false; btnSave.Enabled = false;
            //tia edit
            tbxCustID.ReadOnly = false;
            tbxCustName.Enabled = true;
            tbxCustName.ReadOnly = true;
            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            //tia end

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn);
            string transStatus = Cmd.ExecuteScalar().ToString();
            Conn.Close();

            if (transStatus == "03")
                btnApprove.Text = "UnApprove";
            else
                btnApprove.Text = "Approve";
            if (transStatus == "01")
            {
                btnEdit.Enabled = true;
                btnApprove.Visible = false; btnApprove.Enabled = false;
                btnReject.Visible = false; btnReject.Enabled = false;
                btnAmend.Visible = true; btnAmend.Enabled = true;
            }
            else if (transStatus == "02" || transStatus == "22")
            {
                btnEdit.Enabled = true;
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Administrator")
                {
                    btnApprove.Visible = true; btnApprove.Enabled = true;
                    btnReject.Visible = true; btnReject.Enabled = true;
                    btnAmend.Visible = false; btnAmend.Enabled = false;
                }
                else
                {
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnReject.Visible = false; btnReject.Enabled = false;
                    btnAmend.Visible = false; btnAmend.Enabled = false;
                }
            }
            else if (transStatus == "03" || transStatus == "23")
            {
                btnEdit.Enabled = false;
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Administrator")
                {
                    btnAmend.Visible = true; btnAmend.Enabled = true;
                    btnApprove.Visible = true; btnApprove.Enabled = true;
                    btnReject.Visible = false; btnReject.Enabled = false;
                }
                else if (ControlMgr.GroupName == "Sales Admin")
                {
                    btnAmend.Visible = true; btnAmend.Enabled = true;
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnReject.Visible = false; btnReject.Enabled = false;
                }
            }
            else if (transStatus == "04")
            {
                btnEdit.Enabled = false;
                //if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Administrator")
                //{
                //    btnApprove.Visible = true; btnApprove.Enabled = true;
                //    btnAmend.Visible = true; btnAmend.Enabled = true;
                //    btnReject.Visible = false; btnReject.Visible = false;
                //}
                //else
                //{
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnAmend.Visible = false; btnAmend.Enabled = false;
                    btnReject.Visible = false; btnReject.Visible = false;
                //}
            }
            else if (transStatus == "07")
            {
                btnEdit.Enabled = false;
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Sales Admin" || ControlMgr.GroupName == "Administrator")
                {
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnAmend.Visible = true; btnAmend.Enabled = true;
                    btnReject.Visible = false; btnReject.Visible = false;
                }
                else
                {
                    btnApprove.Visible = false; btnApprove.Enabled = false;
                    btnAmend.Visible = false; btnAmend.Enabled = false;
                    btnReject.Visible = false; btnReject.Visible = false;
                }
            }
            else if (transStatus == "21" || transStatus == "11")
            {
                btnEdit.Enabled = false;
                btnAmend.Visible = false; btnAmend.Enabled = false;
                btnApprove.Visible = false; btnApprove.Enabled = false;
                btnReject.Visible = false; btnReject.Enabled = false;
            }
            else
            {
                btnApprove.Visible = false; /*btnRevision.Visible = false;*/ btnReject.Visible = false; btnAmend.Visible = false;
            }
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

        private void UpdateStatusLogCustomer()
        {
            //Conn = ConnectionString.GetConnection();

            //Cmd = new SqlCommand("SELECT [Deskripsi] FROM [ISBS-NEW4].[dbo].[TransStatusTable] Where TransCode = 'SalesQuotation' And [StatusCode] = '" + TransStatus + "'", Conn);
            //string statusdesk = Cmd.ExecuteScalar().ToString();

            //Query = "INSERT INTO [dbo].[StatusLog_Customer] ";
            //Query += "([StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]) VALUES ";
            //Query += "('Sales Quotation', '" + tbxSQID.Text + "', '" + CustID + "', '" + TransStatus + "', '" + statusdesk + "','" + ControlMgr.UserId + "' , GetDate())";
            //Cmd = new SqlCommand(Query, Conn, Trans);
            //Cmd.ExecuteNonQuery();
            //Conn.Close();

            ListMethod.StatusLogCustomer("SQHeader2", "SalesQuotation", tbxCustID.Text, TransStatus, "", tbxSQID.Text, "", "", "");
        }

        private void insertSQHeader(string SalesQuotationNo, DateTime OrderDate, string SalesMouNo, string RefTransId, string CustID, string CustName, string CurrencyID, decimal ExchRate, decimal Total, decimal Total_Disk, decimal PPN, decimal Total_PPN, decimal PPH, decimal Total_PPH, decimal Total_Nett, decimal Total_Bonus, decimal Total_Cashback, string TermofPayment, string PaymentModeID, string DPType, decimal DPPercent, decimal DPAmount, DateTime DPDueDate, string Notes, string TransStatus, decimal Total_LogisticAmount, string TransType, DateTime ValidTo, DateTime ValidFrom)
        {
            Query = "insert into [dbo].[SalesQuotationH] ([SalesQuotationNo],[OrderDate],[SalesMouNo],[RefTransId],[CustID],[CustName],[CurrencyID],[ExchRate],[Total],[Total_Disk],[PPN],[Total_PPN],[PPH],[Total_PPH],[Total_Nett],[Total_Bonus],[Total_Cashback],[TermofPayment],[PaymentModeID],[DPType],[DPPercent],[DPAmount],[DPDueDate],[Notes],[TransStatus],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Total_LogisticAmount],[TransType],ValidTo,ValidFrom) ";
            Query += "values (@SalesQuotationNo, @OrderDate, @SalesMouNo, @RefTransId, @CustID, @CustName, @CurrencyID, CAST(@ExchRate as numeric(20, 4)), @Total, @Total_Disk, @PPN, @Total_PPN, @PPH, @Total_PPH, @Total_Nett, @Total_Bonus, @Total_Cashback, @TermofPayment, @PaymentModeID, @DPType, CAST(@DPPercent as numeric(6,2)), cast(@DPAmount as numeric(20,4)), @DPDueDate, @Notes, @TransStatus, getdate(), @CreatedBy, '1753-01-01', NULL,@Total_LogisticAmount, @TransType, @ValidTo, @ValidFrom); ";
            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
            Cmd2.Parameters.AddWithValue("@SalesQuotationNo", SalesQuotationNo);
            Cmd2.Parameters.AddWithValue("@OrderDate", OrderDate);
            Cmd2.Parameters.AddWithValue("@SalesMouNo", SalesMouNo == String.Empty ? (object)DBNull.Value : SalesMouNo);
            Cmd2.Parameters.AddWithValue("RefTransId", RefTransId == String.Empty ? (object)DBNull.Value : RefTransId);
            Cmd2.Parameters.AddWithValue("@CustID", CustID);
            Cmd2.Parameters.AddWithValue("@CustName", CustName);
            Cmd2.Parameters.AddWithValue("@CurrencyID", CurrencyID == "Select" ? (object)DBNull.Value : CurrencyID);
            Cmd2.Parameters.AddWithValue("@ExchRate", ExchRate);
            Cmd2.Parameters.AddWithValue("@Total", Total);
            Cmd2.Parameters.AddWithValue("@Total_Disk", Total_Disk);
            Cmd2.Parameters.AddWithValue("@PPN", PPN);
            Cmd2.Parameters.AddWithValue("@Total_PPN", Total_PPN);
            Cmd2.Parameters.AddWithValue("@PPH", PPH);
            Cmd2.Parameters.AddWithValue("@Total_PPH", Total_PPH);
            Cmd2.Parameters.AddWithValue("@Total_Nett", Total_Nett);
            Cmd2.Parameters.AddWithValue("@Total_Bonus", Total_Bonus);
            Cmd2.Parameters.AddWithValue("@Total_Cashback", Total_Cashback);
            Cmd2.Parameters.AddWithValue("@TermofPayment", TermofPayment == "Select" ? (object)DBNull.Value : TermofPayment);
            Cmd2.Parameters.AddWithValue("@PaymentModeID", PaymentModeID);
            Cmd2.Parameters.AddWithValue("@DPType", DPType);
            Cmd2.Parameters.AddWithValue("@DPPercent", DPPercent);
            Cmd2.Parameters.AddWithValue("@DPAmount", DPAmount);
            Cmd2.Parameters.AddWithValue("@DPDueDate", DPType == "N" ? (object)DBNull.Value : DPDueDate);
            Cmd2.Parameters.AddWithValue("@Notes", Notes);
            Cmd2.Parameters.AddWithValue("@TransStatus", TransStatus);
            Cmd2.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
            Cmd2.Parameters.AddWithValue("@Total_LogisticAmount", Total_LogisticAmount);
            Cmd2.Parameters.AddWithValue("@TransType", TransType);
            Cmd2.Parameters.AddWithValue("@ValidTo", ValidTo);
            Cmd2.Parameters.AddWithValue("@ValidFrom", ValidFrom);
            Cmd2.ExecuteNonQuery();
        }

        private void insertSQDetail(string SalesQuotationNo, int SeqNo, string GroupID, string SubGroup1ID, string SubGroup2ID, string ItemID, string FullItemID, string ItemName, string RefTransId, int RefTrans_SeqNo, string DeliveryMethod, decimal LogisticAmount, string LogisticNotes, DateTime ExpectedDateFrom, DateTime ExpectedDateTo, decimal Qty, string Unit, decimal Qty_Alt, string Unit_Alt, decimal ConvertionRatio, decimal Price, decimal Price_Alt, string DiscType, decimal DiscPercent, decimal DiscAmount, decimal BonusAmount, decimal CashBackAmount, decimal SubTotal, decimal SubTotal_PPN, decimal SubTotal_PPH, string Notes, string GelombangId, string BracketId, string Base, decimal Gelombang_Price, int GelombangSeqNo_Base, string PLJNo, int PLJSeqNo, decimal PLJPrice)
        {
            Query = "insert into [dbo].[SalesQuotationD] ([SalesQuotationNo],[SeqNo],[GroupID],[SubGroup1ID],[SubGroup2ID],[ItemID],[FullItemID],[ItemName],[RefTransId],[RefTrans_SeqNo],[DeliveryMethod],[LogisticAmount],[LogisticNotes],[ExpectedDateFrom],[ExpectedDateTo],[Qty],[Unit],[Qty_Alt],[Unit_Alt],[ConvertionRatio],[Price],[Price_Alt],[DiscType],[DiscPercent],[DiscAmount],[BonusAmount],[CashBackAmount],[SubTotal],[SubTotal_PPN],[SubTotal_PPH],[Notes],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GelombangId],[BracketId],[Base],[Gelombang_Price],[GelombangSeqNo_Base], PLJNo, PLJSeqNo, PLJPrice) ";
            Query += "values (@SalesQuotationNo ,@SeqNo ,@GroupID ,@SubGroup1ID ,@SubGroup2ID ,@ItemID ,@FullItemID ,@ItemName,@RefTransId ,@RefTrans_SeqNo ,@DeliveryMethod ,@LogisticAmount ,@LogisticNotes ,@ExpectedDateFrom ,@ExpectedDateTo ,@Qty ,@Unit ,@Qty_Alt ,@Unit_Alt ,@ConvertionRatio ,@Price ,@Price_Alt ,@DiscType ,@DiscPercent ,@DiscAmount ,@BonusAmount ,@CashBackAmount ,@SubTotal ,@SubTotal_PPN ,@SubTotal_PPH ,@Notes ,getdate() ,@CreatedBy ,'1753-01-01' , NULL, @GelombangId, @BracketId, @Base, @Gelombang_Price, @GelombangSeqNo_Base, @PLJNo, @PLJSeqNo, @PLJPrice); ";
            SqlCommand Cmd3 = new SqlCommand(Query, Conn);
            Cmd3.Parameters.AddWithValue("@SalesQuotationNo", SalesQuotationNo);
            Cmd3.Parameters.AddWithValue("@SeqNo", SeqNo);
            Cmd3.Parameters.AddWithValue("@GroupID", GroupID);
            Cmd3.Parameters.AddWithValue("@SubGroup1ID", SubGroup1ID);
            Cmd3.Parameters.AddWithValue("@SubGroup2ID", SubGroup2ID);
            Cmd3.Parameters.AddWithValue("@ItemID", ItemID);
            Cmd3.Parameters.AddWithValue("@FullItemID", FullItemID);
            Cmd3.Parameters.AddWithValue("@ItemName", ItemName);
            Cmd3.Parameters.AddWithValue("@RefTransId", RefTransId == String.Empty ? (object)DBNull.Value : RefTransId);
            Cmd3.Parameters.AddWithValue("@RefTrans_SeqNo", RefTrans_SeqNo == 0 ? (object)DBNull.Value : RefTrans_SeqNo);
            Cmd3.Parameters.AddWithValue("@DeliveryMethod", DeliveryMethod == String.Empty ? (object)DBNull.Value : DeliveryMethod);
            Cmd3.Parameters.AddWithValue("@LogisticAmount", LogisticAmount);
            Cmd3.Parameters.AddWithValue("@LogisticNotes", LogisticNotes);
            Cmd3.Parameters.AddWithValue("@ExpectedDateFrom", ExpectedDateFrom == new DateTime(1753, 1, 1) ? (object)DBNull.Value : ExpectedDateFrom);
            Cmd3.Parameters.AddWithValue("@ExpectedDateTo", ExpectedDateTo == new DateTime(1753, 1, 1) ? (object)DBNull.Value : ExpectedDateTo);
            Cmd3.Parameters.AddWithValue("@Qty", Qty);
            Cmd3.Parameters.AddWithValue("@Unit", Unit);
            Cmd3.Parameters.AddWithValue("@Qty_Alt", Qty_Alt);
            Cmd3.Parameters.AddWithValue("@Unit_Alt", Unit_Alt);
            Cmd3.Parameters.AddWithValue("@ConvertionRatio", ConvertionRatio);
            Cmd3.Parameters.AddWithValue("@Price", Price);
            Cmd3.Parameters.AddWithValue("@Price_Alt", Price_Alt);
            Cmd3.Parameters.AddWithValue("@DiscType", Base == "N" ? (object)DBNull.Value : DiscType);
            Cmd3.Parameters.AddWithValue("@DiscPercent", DiscPercent);
            Cmd3.Parameters.AddWithValue("@DiscAmount", DiscAmount);
            Cmd3.Parameters.AddWithValue("@BonusAmount", BonusAmount);
            Cmd3.Parameters.AddWithValue("@CashBackAmount", CashBackAmount);
            Cmd3.Parameters.AddWithValue("@SubTotal", SubTotal);
            Cmd3.Parameters.AddWithValue("@SubTotal_PPN", SubTotal_PPN);
            Cmd3.Parameters.AddWithValue("@SubTotal_PPH", SubTotal_PPH);
            Cmd3.Parameters.AddWithValue("@Notes", Notes);
            Cmd3.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
            Cmd3.Parameters.AddWithValue("@GelombangId", GelombangId == String.Empty ? (object)DBNull.Value : GelombangId);
            Cmd3.Parameters.AddWithValue("@BracketId", BracketId == String.Empty ? (object)DBNull.Value : BracketId);
            Cmd3.Parameters.AddWithValue("@Base", Base == String.Empty ? (object)DBNull.Value : Base);
            Cmd3.Parameters.AddWithValue("@Gelombang_Price", GelombangId == String.Empty ? (object)DBNull.Value : Gelombang_Price);
            Cmd3.Parameters.AddWithValue("@GelombangSeqNo_Base", GelombangSeqNo_Base == 0 ? (object)DBNull.Value : GelombangSeqNo_Base);
            Cmd3.Parameters.AddWithValue("@PLJNo", PLJNo == String.Empty ? (object)DBNull.Value : PLJNo);
            Cmd3.Parameters.AddWithValue("@PLJSeqNo", PLJSeqNo == 0 ? (object)DBNull.Value : PLJSeqNo);
            Cmd3.Parameters.AddWithValue("@PLJPrice", PLJNo == String.Empty ? (object)DBNull.Value : PLJPrice);
            Cmd3.ExecuteNonQuery();
        }


        string CustID, CustName, SalesQuotationNo, SalesMouNo, RefTransId, CurrencyID, TermofPayment, PaymentModeID, DPType,Notes,TransStatus,TransType;
        decimal ExchRate, Total, Total_Disk, PPN, Total_PPN, PPH, Total_PPH, Total_Nett, Total_Bonus, Total_Cashback, DPPercent, DPAmount,Total_LogisticAmount;

        DateTime DPDueDate, ValidTo, ValidFrom, OrderDate;

        string SalesQuotationNoDtl, GroupID, SubGroup1ID, SubGroup2ID, FullItemID, ItemName, RefTransIdDtl, DeliveryMethod, LogisticNotes, Unit, Unit_Alt, DiscType, NotesDtl, GelombangId, BracketId, Base, PLJNo;
        int SeqNo;
        int RefTrans_SeqNo = 0;
        int PLJSeqNo;
        int GelombangSeqNo_Base = 0;        

        DateTime ExpectedDateFrom = new DateTime(1753, 1, 1);
        DateTime ExpectedDateTo = new DateTime(1753, 1, 1);
        string[] TmpSQNumber;
        string TmpMsgSqNo;

        decimal LogisticAmount, Qty, Qty_Alt, ConvertionRatio, Price, Price_Alt, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, Gelombang_Price, PLJPrice, price, qty, STotal, STotal_PPN, STotal_PPH, GTotal;

        string tempID;

        private void SetupSaveVariable()
        {
            CustID = tbxCustID.Text;
            OrderDate = dtSQ.Value;
            SalesMouNo = tbxMoUID.Text;
            RefTransId = tbxRefID.Text;
            CurrencyID = cbCurrency.Text;
            ExchRate = Convert.ToDecimal(tbxXRate.Text);
            Total = tbxSTotal.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxSTotal.Text);
            Total_Disk = tbxGDisc.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGDisc.Text);
            PPN = Convert.ToDecimal(cbPPN.Text);
            Total_PPN = tbxGPPN.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGPPN.Text);
            PPH = Convert.ToDecimal(cbPPh.Text);
            Total_PPH = tbxGPPh.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGPPh.Text);
            Total_Nett = tbxGTotal.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGTotal.Text);
            Total_Bonus = tbxGBonus.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGBonus.Text);
            Total_Cashback = tbxGCashback.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxGCashback.Text);
            TermofPayment = cbToP.Text;
            Cmd = new SqlCommand("Select [PaymentModeID] from [ISBS-NEW4].[dbo].[PaymentMode] where [PaymentModeName] = '" + cbPaymentMode.Text + "'", Conn2);
            PaymentModeID = Cmd.ExecuteScalar().ToString();
            DPType = cbDPType.Text;
            DPPercent = Convert.ToDecimal(tbxDPPercent.Text);
            DPAmount = Convert.ToDecimal(tbxDPAmount.Text);
            DPDueDate = dtDP.Value;
            Notes = tbxNotes.Text;
            TransStatus = "";
            flag = checkPriceTolerance();
            if (flag == 'X')
                TransStatus = "02"; //Waiting for Approval
            else
                TransStatus = "01"; //Created
            Total_LogisticAmount = tbxLogAmount.Text == String.Empty ? Convert.ToDecimal("0") : Convert.ToDecimal(tbxLogAmount.Text);
            TransType = cbType.Text;
            ValidTo = dtValidTo.Value;
            ValidFrom = dtValidFrom.Value;
        }

        private void SaveNewSQDetailNoRef()
        {
            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                SalesQuotationNoDtl = tempID;
                SeqNo = Convert.ToInt32(j + 1);
                GroupID = dataGridView1.Rows[j].Cells["GroupID"].Value.ToString();
                SubGroup1ID = dataGridView1.Rows[j].Cells["SubGroup1ID"].Value.ToString();
                SubGroup2ID = dataGridView1.Rows[j].Cells["SubGroup2ID"].Value.ToString();
                ItemID = dataGridView1.Rows[j].Cells["ItemID"].Value.ToString();
                FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();
                ItemName = dataGridView1.Rows[j].Cells["ItemName"].Value.ToString();
                RefTransIdDtl = "";
                if (!(dataGridView1.Rows[j].Cells["RefTransId"].Value == null || dataGridView1.Rows[j].Cells["RefTransId"].Value == String.Empty))
                    RefTransIdDtl = dataGridView1.Rows[j].Cells["RefTransId"].Value.ToString();
                RefTrans_SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == String.Empty))
                    RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value);
                DeliveryMethod = "";
                if (!(dataGridView2.Rows[j].Cells["DeliveryMethod"].Value == null))
                    DeliveryMethod = dataGridView2.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                LogisticAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["LogisticAmount"].Value == null || dataGridView2.Rows[j].Cells["LogisticAmount"].Value == String.Empty))
                    LogisticAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["LogisticAmount"].Value.ToString());

                LogisticNotes = "";
                if (!(dataGridView2.Rows[j].Cells["LogisticNotes"].Value == null || dataGridView2.Rows[j].Cells["LogisticNotes"].Value == String.Empty))
                    LogisticNotes = dataGridView2.Rows[j].Cells["LogisticNotes"].Value.ToString();
                ExpectedDateFrom = new DateTime(1753, 1, 1);
                if (!(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value == null || dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value == String.Empty))
                    ExpectedDateFrom = Convert.ToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value);
                ExpectedDateTo = new DateTime(1753, 1, 1);
                if (!(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value == null || dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value == String.Empty))
                    ExpectedDateTo = Convert.ToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value);

                Qty = 0;
                if (!(dataGridView1.Rows[j].Cells["Qty"].Value == null || dataGridView1.Rows[j].Cells["Qty"].Value == String.Empty))
                    Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                Unit = dataGridView1.Rows[j].Cells["Unit"].Value.ToString();
                Qty_Alt = 0;
                if (!(dataGridView1.Rows[j].Cells["Qty_Alt"].Value == null || dataGridView1.Rows[j].Cells["Qty_Alt"].Value == String.Empty))
                    Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                Unit_Alt = dataGridView1.Rows[j].Cells["Unit_Alt"].Value.ToString();
                ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value);
                Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value);
                Price_Alt = 0;
                if (!(dataGridView1.Rows[j].Cells["Price_Alt"].Value == null || dataGridView1.Rows[j].Cells["Price_Alt"].Value == String.Empty))
                    Price_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value);
                DiscType = "1";
                if (!(dataGridView1.Rows[j].Cells["DiscType"].Value == null || dataGridView1.Rows[j].Cells["DiscType"].Value == String.Empty || dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() == "Select"))
                {
                    Cmd = new SqlCommand("select [DiskonSchemeID] from [dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() + "'", Conn2);
                    DiscType = Cmd.ExecuteScalar().ToString();
                }
                DiscPercent = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscPercent"].Value == null || dataGridView1.Rows[j].Cells["DiscPercent"].Value == String.Empty))
                    DiscPercent = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscPercent"].Value);
                DiscAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscAmount"].Value == null || dataGridView1.Rows[j].Cells["DiscAmount"].Value == String.Empty))
                    DiscAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscAmount"].Value);
                BonusAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["BonusAmount"].Value == null || dataGridView2.Rows[j].Cells["BonusAmount"].Value == String.Empty))
                    BonusAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["BonusAmount"].Value);
                CashBackAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["CashBackAmount"].Value == null || dataGridView2.Rows[j].Cells["CashBackAmount"].Value == String.Empty))
                    CashBackAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["CashBackAmount"].Value);
                SubTotal = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal"].Value);
                SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPN"].Value);
                SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPH"].Value);
                NotesDtl = "";
                if (!(dataGridView1.Rows[j].Cells["Notes"].Value == null || dataGridView1.Rows[j].Cells["Notes"].Value == String.Empty))
                    NotesDtl = dataGridView1.Rows[j].Cells["Notes"].Value.ToString();
                GelombangId = "";
                if (!(dataGridView1.Rows[j].Cells["GelombangId"].Value == null || dataGridView1.Rows[j].Cells["GelombangId"].Value == String.Empty))
                    GelombangId = dataGridView1.Rows[j].Cells["Gelombang_Price"].Value.ToString();
                BracketId = "";
                if (!(dataGridView1.Rows[j].Cells["BracketId"].Value == null || dataGridView1.Rows[j].Cells["BracketId"].Value == String.Empty))
                    BracketId = dataGridView1.Rows[j].Cells["BracketId"].Value.ToString();
                Base = "";
                if (!(dataGridView1.Rows[j].Cells["Base"].Value.ToString() == String.Empty || dataGridView1.Rows[j].Cells["Base"].Value.ToString() == null))
                    Base = dataGridView1.Rows[j].Cells["Base"].Value.ToString();
                Gelombang_Price = 0;
                if (!(dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == null || dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == String.Empty))
                    Gelombang_Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Gelombang_Price"].Value);
                GelombangSeqNo_Base = 0;
                if (!(dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == null || dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == String.Empty))
                    GelombangSeqNo_Base = Convert.ToInt32(dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value);
                PLJNo = "";
                if (!(dataGridView1.Rows[j].Cells["PLJNo"].Value == null || dataGridView1.Rows[j].Cells["PLJNo"].Value == String.Empty))
                    PLJNo = dataGridView1.Rows[j].Cells["PLJNo"].Value.ToString();
                PLJSeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value == null || dataGridView1.Rows[j].Cells["PLJSeqNo"].Value == String.Empty))
                    PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value);
                PLJPrice = 0;
                if (!(dataGridView1.Rows[j].Cells["PLJPrice"].Value == null || dataGridView1.Rows[j].Cells["PLJPrice"].Value == String.Empty))
                    PLJPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value);

                insertSQDetail(SalesQuotationNoDtl, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, RefTransIdDtl, RefTrans_SeqNo, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesDtl, GelombangId, BracketId, Base, Gelombang_Price, GelombangSeqNo_Base, PLJNo, PLJSeqNo, PLJPrice);
                tbxSQID.Text = tempID;
            }
        }

        private void SaveNewSQDetailWithRef()
        {
            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                int SeqNo = Convert.ToInt32(j + 1);
                string GroupID = dataGridView1.Rows[j].Cells["GroupID"].Value.ToString();
                string SubGroup1ID = dataGridView1.Rows[j].Cells["SubGroup1ID"].Value.ToString();
                string SubGroup2ID = dataGridView1.Rows[j].Cells["SubGroup2ID"].Value.ToString();
                string ItemID = dataGridView1.Rows[j].Cells["ItemID"].Value.ToString();
                string FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();
                string ItemName = dataGridView1.Rows[j].Cells["ItemName"].Value.ToString();
                string RefTransIdDtl = "";
                if (!(dataGridView1.Rows[j].Cells["RefTransId"].Value == null || dataGridView1.Rows[j].Cells["RefTransId"].Value == String.Empty))
                    RefTransIdDtl = dataGridView1.Rows[j].Cells["RefTransId"].Value.ToString();
                int RefTrans_SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == String.Empty))
                    RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value);
                string DeliveryMethod = "";
                if (!(dataGridView2.Rows[j].Cells["DeliveryMethod"].Value == null))
                    DeliveryMethod = dataGridView2.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                decimal LogisticAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["LogisticAmount"].Value == null || dataGridView2.Rows[j].Cells["LogisticAmount"].Value == String.Empty))
                    LogisticAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["LogisticAmount"].Value.ToString());

                string LogisticNotes = "";
                if (!(dataGridView2.Rows[j].Cells["LogisticNotes"].Value == null || dataGridView2.Rows[j].Cells["LogisticNotes"].Value == String.Empty))
                    LogisticNotes = dataGridView2.Rows[j].Cells["LogisticNotes"].Value.ToString();
                DateTime ExpectedDateFrom = new DateTime(1753, 1, 1);
                if (!(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value == null || dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value == String.Empty))
                    ExpectedDateFrom = Convert.ToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value);
                //ExpectedDateFrom = ConvertObjectToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value.ToString());
                DateTime ExpectedDateTo = new DateTime(1753, 1, 1);
                if (!(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value == null || dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value == String.Empty))
                    ExpectedDateTo = Convert.ToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value);
                //ExpectedDateTo = ConvertObjectToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value.ToString());

                decimal Qty = 0;
                if (!(dataGridView1.Rows[j].Cells["Qty"].Value == null || dataGridView1.Rows[j].Cells["Qty"].Value == String.Empty))
                    Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                string Unit = dataGridView1.Rows[j].Cells["Unit"].Value.ToString();
                decimal Qty_Alt = 0;
                if (!(dataGridView1.Rows[j].Cells["Qty_Alt"].Value == null || dataGridView1.Rows[j].Cells["Qty_Alt"].Value == String.Empty))
                    Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                string Unit_Alt = dataGridView1.Rows[j].Cells["Unit_Alt"].Value.ToString();
                decimal ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value);
                decimal Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value);
                decimal Price_Alt = 0;
                if (!(dataGridView1.Rows[j].Cells["Price_Alt"].Value == null || dataGridView1.Rows[j].Cells["Price_Alt"].Value == String.Empty))
                    Price_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value);
                string DiscType = "1";
                if (!(dataGridView1.Rows[j].Cells["DiscType"].Value == null || dataGridView1.Rows[j].Cells["DiscType"].Value == String.Empty || dataGridView1.Rows[j].Cells["DiscType"].Value == "Select"))
                {
                    Cmd = new SqlCommand("select [DiskonSchemeID] from [dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() + "'", Conn2);
                    DiscType = Cmd.ExecuteScalar().ToString();
                }
                decimal DiscPercent = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscPercent"].Value == null || dataGridView1.Rows[j].Cells["DiscPercent"].Value == String.Empty))
                    DiscPercent = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscPercent"].Value);
                decimal DiscAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscAmount"].Value == null || dataGridView1.Rows[j].Cells["DiscAmount"].Value == String.Empty))
                    DiscAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscAmount"].Value);
                decimal BonusAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["BonusAmount"].Value == null || dataGridView2.Rows[j].Cells["BonusAmount"].Value == String.Empty))
                    BonusAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["BonusAmount"].Value);
                decimal CashBackAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["CashBackAmount"].Value == null || dataGridView2.Rows[j].Cells["CashBackAmount"].Value == String.Empty))
                    CashBackAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["CashBackAmount"].Value);
                decimal SubTotal = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal"].Value);
                decimal SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPN"].Value);
                decimal SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPH"].Value);
                string NotesDtl = "";
                if (!(dataGridView1.Rows[j].Cells["Notes"].Value == null || dataGridView1.Rows[j].Cells["Notes"].Value == String.Empty))
                    NotesDtl = dataGridView1.Rows[j].Cells["Notes"].Value.ToString();
                string GelombangId = "";
                if (!(dataGridView1.Rows[j].Cells["GelombangId"].Value == null || dataGridView1.Rows[j].Cells["GelombangId"].Value == String.Empty))
                    GelombangId = dataGridView1.Rows[j].Cells["Gelombang_Price"].Value.ToString();
                string BracketId = "";
                if (!(dataGridView1.Rows[j].Cells["BracketId"].Value == null || dataGridView1.Rows[j].Cells["BracketId"].Value == String.Empty))
                    BracketId = dataGridView1.Rows[j].Cells["BracketId"].Value.ToString();
                string Base = "";
                if (!(dataGridView1.Rows[j].Cells["Base"].Value.ToString() == String.Empty || dataGridView1.Rows[j].Cells["Base"].Value.ToString() == null))
                    Base = dataGridView1.Rows[j].Cells["Base"].Value.ToString();
                decimal Gelombang_Price = 0;
                if (!(dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == null || dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == String.Empty))
                    Gelombang_Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Gelombang_Price"].Value);
                int GelombangSeqNo_Base = 0;
                if (!(dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == null || dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == String.Empty))
                    GelombangSeqNo_Base = Convert.ToInt32(dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value);
                string PLJNo = "";
                if (!(dataGridView1.Rows[j].Cells["PLJNo"].Value == null || dataGridView1.Rows[j].Cells["PLJNo"].Value == String.Empty))
                    PLJNo = dataGridView1.Rows[j].Cells["PLJNo"].Value.ToString();
                int PLJSeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value == null || dataGridView1.Rows[j].Cells["PLJSeqNo"].Value == String.Empty))
                    PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value);
                decimal PLJPrice = 0;
                if (!(dataGridView1.Rows[j].Cells["PLJPrice"].Value == null || dataGridView1.Rows[j].Cells["PLJPrice"].Value == String.Empty))
                    PLJPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value);
                

                insertSQDetail(SalesQuotationNo, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, RefTransIdDtl, RefTrans_SeqNo, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesDtl, GelombangId, BracketId, Base, Gelombang_Price, GelombangSeqNo_Base, PLJNo, PLJSeqNo, PLJPrice);
            }
        }

        private void SaveEditSQDetail()
        {
            Query = "DELETE FROM [SalesQuotationD] WHERE [SalesQuotationNo] = '" + SalesQuotationNo + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                Cmd.ExecuteNonQuery();

            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                #region variable
                string SalesQuotationNoDtl = tbxSQID.Text;
                int SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["seqNo"].Value == null))
                    SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["SeqNo"].Value);
                string GroupID = dataGridView1.Rows[j].Cells["GroupID"].Value.ToString();
                string SubGroup1ID = dataGridView1.Rows[j].Cells["SubGroup1ID"].Value.ToString();
                string SubGroup2ID = dataGridView1.Rows[j].Cells["SubGroup2ID"].Value.ToString();
                string ItemID = dataGridView1.Rows[j].Cells["ItemID"].Value.ToString();
                string FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();
                string ItemName = dataGridView1.Rows[j].Cells["ItemName"].Value.ToString();
                string RefTransIdDtl = "";
                if (!(dataGridView1.Rows[j].Cells["RefTransId"].Value == null || dataGridView1.Rows[j].Cells["RefTransId"].Value == String.Empty))
                    RefTransIdDtl = dataGridView1.Rows[j].Cells["RefTransId"].Value.ToString();
                int RefTrans_SeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == String.Empty))
                    RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value);
                string DeliveryMethod = "";
                if (!(dataGridView2.Rows[j].Cells["DeliveryMethod"].Value == null))
                    DeliveryMethod = dataGridView2.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                decimal LogisticAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["LogisticAmount"].Value == null || dataGridView2.Rows[j].Cells["LogisticAmount"].Value == String.Empty))
                    LogisticAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["LogisticAmount"].Value.ToString());

                string LogisticNotes = "";
                if (!(dataGridView2.Rows[j].Cells["LogisticNotes"].Value == null || dataGridView2.Rows[j].Cells["LogisticNotes"].Value == String.Empty))
                    LogisticNotes = dataGridView2.Rows[j].Cells["LogisticNotes"].Value.ToString();
                DateTime ExpectedDateFrom = new DateTime(1753, 1, 1);
                if (!(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value == null || dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value == String.Empty))
                    ExpectedDateFrom = Convert.ToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value);
                //ExpectedDateFrom = ConvertObjectToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value.ToString());
                DateTime ExpectedDateTo = new DateTime(1753, 1, 1);
                if (!(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value == null || dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value == String.Empty))
                    ExpectedDateTo = Convert.ToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value);
                //ExpectedDateTo = ConvertObjectToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value.ToString());

                decimal Qty = 0;
                if (!(dataGridView1.Rows[j].Cells["Qty"].Value == null || dataGridView1.Rows[j].Cells["Qty"].Value == String.Empty))
                    Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                string Unit = dataGridView1.Rows[j].Cells["Unit"].Value.ToString();
                decimal Qty_Alt = 0;
                if (!(dataGridView1.Rows[j].Cells["Qty_Alt"].Value == null || dataGridView1.Rows[j].Cells["Qty_Alt"].Value == String.Empty))
                    Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                string Unit_Alt = dataGridView1.Rows[j].Cells["Unit_Alt"].Value.ToString();
                decimal ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value);
                decimal Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value);
                decimal Price_Alt = 0;
                if (!(dataGridView1.Rows[j].Cells["Price_Alt"].Value == null || dataGridView1.Rows[j].Cells["Price_Alt"].Value == String.Empty))
                    Price_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value);
                string DiscType = "1";
                if (!(dataGridView1.Rows[j].Cells["DiscType"].Value == null || dataGridView1.Rows[j].Cells["DiscType"].Value == String.Empty || dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() == "Select"))
                {
                    Cmd = new SqlCommand("select [DiskonSchemeID] from [dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() + "'", Conn2);
                    DiscType = Cmd.ExecuteScalar().ToString();
                }
                decimal DiscPercent = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscPercent"].Value == null || dataGridView1.Rows[j].Cells["DiscPercent"].Value == String.Empty))
                    DiscPercent = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscPercent"].Value);
                decimal DiscAmount = 0;
                if (!(dataGridView1.Rows[j].Cells["DiscAmount"].Value == null || dataGridView1.Rows[j].Cells["DiscAmount"].Value == String.Empty))
                    DiscAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscAmount"].Value);
                decimal BonusAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["BonusAmount"].Value == null || dataGridView2.Rows[j].Cells["BonusAmount"].Value == String.Empty))
                    BonusAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["BonusAmount"].Value);
                decimal CashBackAmount = 0;
                if (!(dataGridView2.Rows[j].Cells["CashBackAmount"].Value == null || dataGridView2.Rows[j].Cells["CashBackAmount"].Value == String.Empty))
                    CashBackAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["CashBackAmount"].Value);
                decimal SubTotal = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal"].Value);
                decimal SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPN"].Value);
                decimal SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPH"].Value);
                string NotesDtl = "";
                if (!(dataGridView1.Rows[j].Cells["Notes"].Value == null || dataGridView1.Rows[j].Cells["Notes"].Value == String.Empty))
                    NotesDtl = dataGridView1.Rows[j].Cells["Notes"].Value.ToString();
                string GelombangId = "";
                if (!(dataGridView1.Rows[j].Cells["GelombangId"].Value == null || dataGridView1.Rows[j].Cells["GelombangId"].Value == String.Empty))
                    GelombangId = dataGridView1.Rows[j].Cells["Gelombang_Price"].Value.ToString();
                string BracketId = "";
                if (!(dataGridView1.Rows[j].Cells["BracketId"].Value == null || dataGridView1.Rows[j].Cells["BracketId"].Value == String.Empty))
                    BracketId = dataGridView1.Rows[j].Cells["BracketId"].Value.ToString();
                string Base = "";
                if (!(dataGridView1.Rows[j].Cells["Base"].Value.ToString() == String.Empty || dataGridView1.Rows[j].Cells["Base"].Value.ToString() == null))
                    Base = dataGridView1.Rows[j].Cells["Base"].Value.ToString();
                decimal Gelombang_Price = 0;
                if (!(dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == null || dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == String.Empty))
                    Gelombang_Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Gelombang_Price"].Value);
                int GelombangSeqNo_Base = 0;
                if (!(dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == null || dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == String.Empty))
                    GelombangSeqNo_Base = Convert.ToInt32(dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value);
                string PLJNo = "";
                if (!(dataGridView1.Rows[j].Cells["PLJNo"].Value == null || dataGridView1.Rows[j].Cells["PLJNo"].Value == String.Empty))
                    PLJNo = dataGridView1.Rows[j].Cells["PLJNo"].Value.ToString();
                int PLJSeqNo = 0;
                if (!(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value == null || dataGridView1.Rows[j].Cells["PLJSeqNo"].Value == String.Empty))
                    PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value);
                decimal PLJPrice = 0;
                if (!(dataGridView1.Rows[j].Cells["PLJPrice"].Value == null || dataGridView1.Rows[j].Cells["PLJPrice"].Value == String.Empty))
                    PLJPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value);
                #endregion                

                //GET SEQNO
                Query = "select top 1 SeqNo from SalesQuotationD where SalesQuotationNo = '" + SalesQuotationNo + "' order by SeqNo desc";
                Cmd = new SqlCommand(Query, Conn);
                SeqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;

                insertSQDetail(SalesQuotationNo, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, RefTransIdDtl, RefTrans_SeqNo, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesDtl, GelombangId, BracketId, Base, Gelombang_Price, GelombangSeqNo_Base, PLJNo, PLJSeqNo, PLJPrice);

                #region old
                //for (int j = 0; j < dataGridView1.RowCount; j++)
                //{
                //    #region variable
                //    string SalesQuotationNoDtl = tbxSQID.Text;
                //    int SeqNo = 0;
                //    if (!(dataGridView1.Rows[j].Cells["SeqNo"].Value == String.Empty || dataGridView1.Rows[j].Cells["seqNo"].Value == null))
                //        SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["SeqNo"].Value);
                //    string GroupID = dataGridView1.Rows[j].Cells["GroupID"].Value.ToString();
                //    string SubGroup1ID = dataGridView1.Rows[j].Cells["SubGroup1ID"].Value.ToString();
                //    string SubGroup2ID = dataGridView1.Rows[j].Cells["SubGroup2ID"].Value.ToString();
                //    string ItemID = dataGridView1.Rows[j].Cells["ItemID"].Value.ToString();
                //    string FullItemID = dataGridView1.Rows[j].Cells["FullItemID"].Value.ToString();
                //    string ItemName = dataGridView1.Rows[j].Cells["ItemName"].Value.ToString();
                //    string RefTransIdDtl = "";
                //    if (!(dataGridView1.Rows[j].Cells["RefTransId"].Value == null || dataGridView1.Rows[j].Cells["RefTransId"].Value == String.Empty))
                //        RefTransIdDtl = dataGridView1.Rows[j].Cells["RefTransId"].Value.ToString();
                //    int RefTrans_SeqNo = 0;
                //    if (!(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == null || dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value == String.Empty))
                //        RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["RefTrans_SeqNo"].Value);
                //    string DeliveryMethod = "";
                //    if (!(dataGridView2.Rows[j].Cells["DeliveryMethod"].Value == null))
                //        DeliveryMethod = dataGridView2.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                //    decimal LogisticAmount = 0;
                //    if (!(dataGridView2.Rows[j].Cells["LogisticAmount"].Value == null || dataGridView2.Rows[j].Cells["LogisticAmount"].Value == String.Empty))
                //        LogisticAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["LogisticAmount"].Value.ToString());

                //    string LogisticNotes = "";
                //    if (!(dataGridView2.Rows[j].Cells["LogisticNotes"].Value == null || dataGridView2.Rows[j].Cells["LogisticNotes"].Value == String.Empty))
                //        LogisticNotes = dataGridView2.Rows[j].Cells["LogisticNotes"].Value.ToString();
                //    DateTime ExpectedDateFrom = new DateTime(1753, 1, 1);
                //    if (!(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value == null || dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value == String.Empty))
                //        ExpectedDateFrom = Convert.ToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value);
                //    //ExpectedDateFrom = ConvertObjectToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateFrom"].Value.ToString());
                //    DateTime ExpectedDateTo = new DateTime(1753, 1, 1);
                //    if (!(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value == null || dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value == String.Empty))
                //        ExpectedDateTo = Convert.ToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value);
                //    //ExpectedDateTo = ConvertObjectToDateTime(dataGridView2.Rows[j].Cells["ExpectedDateTo"].Value.ToString());

                //    decimal Qty = 0;
                //    if (!(dataGridView1.Rows[j].Cells["Qty"].Value == null || dataGridView1.Rows[j].Cells["Qty"].Value == String.Empty))
                //        Qty = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty"].Value);
                //    string Unit = dataGridView1.Rows[j].Cells["Unit"].Value.ToString();
                //    decimal Qty_Alt = 0;
                //    if (!(dataGridView1.Rows[j].Cells["Qty_Alt"].Value == null || dataGridView1.Rows[j].Cells["Qty_Alt"].Value == String.Empty))
                //        Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Qty_Alt"].Value);
                //    string Unit_Alt = dataGridView1.Rows[j].Cells["Unit_Alt"].Value.ToString();
                //    decimal ConvertionRatio = Convert.ToDecimal(dataGridView1.Rows[j].Cells["ConvertionRatio"].Value);
                //    decimal Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price"].Value);
                //    decimal Price_Alt = 0;
                //    if (!(dataGridView1.Rows[j].Cells["Price_Alt"].Value == null || dataGridView1.Rows[j].Cells["Price_Alt"].Value == String.Empty))
                //        Price_Alt = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Price_Alt"].Value);
                //    string DiscType = "1";
                //    if (!(dataGridView1.Rows[j].Cells["DiscType"].Value == null || dataGridView1.Rows[j].Cells["DiscType"].Value == String.Empty || dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() == "Select"))
                //    {
                //        Cmd = new SqlCommand("select [DiskonSchemeID] from [dbo].[DiskonScheme] where [Deskripsi] = '" + dataGridView1.Rows[j].Cells["DiscType"].Value.ToString() + "'", Conn2);
                //        DiscType = Cmd.ExecuteScalar().ToString();
                //    }
                //    decimal DiscPercent = 0;
                //    if (!(dataGridView1.Rows[j].Cells["DiscPercent"].Value == null || dataGridView1.Rows[j].Cells["DiscPercent"].Value == String.Empty))
                //        DiscPercent = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscPercent"].Value);
                //    decimal DiscAmount = 0;
                //    if (!(dataGridView1.Rows[j].Cells["DiscAmount"].Value == null || dataGridView1.Rows[j].Cells["DiscAmount"].Value == String.Empty))
                //        DiscAmount = Convert.ToDecimal(dataGridView1.Rows[j].Cells["DiscAmount"].Value);
                //    decimal BonusAmount = 0;
                //    if (!(dataGridView2.Rows[j].Cells["BonusAmount"].Value == null || dataGridView2.Rows[j].Cells["BonusAmount"].Value == String.Empty))
                //        BonusAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["BonusAmount"].Value);
                //    decimal CashBackAmount = 0;
                //    if (!(dataGridView2.Rows[j].Cells["CashBackAmount"].Value == null || dataGridView2.Rows[j].Cells["CashBackAmount"].Value == String.Empty))
                //        CashBackAmount = Convert.ToDecimal(dataGridView2.Rows[j].Cells["CashBackAmount"].Value);
                //    decimal SubTotal = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal"].Value);
                //    decimal SubTotal_PPN = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPN"].Value);
                //    decimal SubTotal_PPH = Convert.ToDecimal(dataGridView1.Rows[j].Cells["SubTotal_PPH"].Value);
                //    string NotesDtl = "";
                //    if (!(dataGridView1.Rows[j].Cells["Notes"].Value == null || dataGridView1.Rows[j].Cells["Notes"].Value == String.Empty))
                //        NotesDtl = dataGridView1.Rows[j].Cells["Notes"].Value.ToString();
                //    string GelombangId = "";
                //    if (!(dataGridView1.Rows[j].Cells["GelombangId"].Value == null || dataGridView1.Rows[j].Cells["GelombangId"].Value == String.Empty))
                //        GelombangId = dataGridView1.Rows[j].Cells["Gelombang_Price"].Value.ToString();
                //    string BracketId = "";
                //    if (!(dataGridView1.Rows[j].Cells["BracketId"].Value == null || dataGridView1.Rows[j].Cells["BracketId"].Value == String.Empty))
                //        BracketId = dataGridView1.Rows[j].Cells["BracketId"].Value.ToString();
                //    string Base = "";
                //    if (!(dataGridView1.Rows[j].Cells["Base"].Value.ToString() == String.Empty || dataGridView1.Rows[j].Cells["Base"].Value.ToString() == null))
                //        Base = dataGridView1.Rows[j].Cells["Base"].Value.ToString();
                //    decimal Gelombang_Price = 0;
                //    if (!(dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == null || dataGridView1.Rows[j].Cells["Gelombang_Price"].Value == String.Empty))
                //        Gelombang_Price = Convert.ToDecimal(dataGridView1.Rows[j].Cells["Gelombang_Price"].Value);
                //    int GelombangSeqNo_Base = 0;
                //    if (!(dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == null || dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value == String.Empty))
                //        GelombangSeqNo_Base = Convert.ToInt32(dataGridView1.Rows[j].Cells["GelombangSeqNo_Base"].Value);
                //    string PLJNo = "";
                //    if (!(dataGridView1.Rows[j].Cells["PLJNo"].Value == null || dataGridView1.Rows[j].Cells["PLJNo"].Value == String.Empty))
                //        PLJNo = dataGridView1.Rows[j].Cells["PLJNo"].Value.ToString();
                //    int PLJSeqNo = 0;
                //    if (!(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value == null || dataGridView1.Rows[j].Cells["PLJSeqNo"].Value == String.Empty))
                //        PLJSeqNo = Convert.ToInt32(dataGridView1.Rows[j].Cells["PLJSeqNo"].Value);
                //    decimal PLJPrice = 0;
                //    if (!(dataGridView1.Rows[j].Cells["PLJPrice"].Value == null || dataGridView1.Rows[j].Cells["PLJPrice"].Value == String.Empty))
                //        PLJPrice = Convert.ToDecimal(dataGridView1.Rows[j].Cells["PLJPrice"].Value);
                //    #endregion
                //    Query = "select top 1 SalesQuotationNo from SalesQuotationD where SalesQuotationNo = '" + SalesQuotationNoDtl + "' and SeqNo = '" + SeqNo + "'";
                //    Cmd = new SqlCommand(Query, Conn);
                //    Dr = Cmd.ExecuteReader();
                //    if (Dr.HasRows)
                //    {
                //        updateSQDetail(GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesDtl, SalesQuotationNo, SeqNo, PLJNo, PLJSeqNo, PLJPrice);
                //    }
                //    else
                //    {
                //        //GET SEQNO
                //        Query = "select top 1 SeqNo from SalesQuotationD where SalesQuotationNo = '" + SalesQuotationNo + "' order by SeqNo desc";
                //        Cmd = new SqlCommand(Query, Conn);
                //        SeqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;

                //        insertSQDetail(SalesQuotationNo, SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, RefTransIdDtl, RefTrans_SeqNo, DeliveryMethod, LogisticAmount, LogisticNotes, ExpectedDateFrom, ExpectedDateTo, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, Price, Price_Alt, DiscType, DiscPercent, DiscAmount, BonusAmount, CashBackAmount, SubTotal, SubTotal_PPN, SubTotal_PPH, NotesDtl, GelombangId, BracketId, Base, Gelombang_Price, GelombangSeqNo_Base, PLJNo, PLJSeqNo, PLJPrice);
                //    }
                //}
                #endregion
            }
        }

        private void SaveNew()
        {
            if (tbxRefID.Text == String.Empty)
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    string Jenis = "SQ", Kode = "SQ";
                    tempID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    SalesQuotationNo = tempID;
                    CustID = listBox1.Items[i].ToString().Split(' ')[0];
                    CustName = listBox1.Items[i].ToString().Substring(7, listBox1.Items[i].ToString().Length - 7);

                    insertSQHeader(SalesQuotationNo, OrderDate, SalesMouNo, RefTransId, CustID, CustName, CurrencyID, ExchRate, Total, Total_Disk, PPN, Total_PPN, PPH, Total_PPH, Total_Nett, Total_Bonus, Total_Cashback, TermofPayment, PaymentModeID, DPType, DPPercent, DPAmount, DPDueDate, Notes, TransStatus, Total_LogisticAmount, TransType, ValidTo, ValidFrom);
                    SaveNewSQDetailNoRef();

                    TmpSQNumber[i] = tempID;
                }
                for (int i = 0; i < TmpSQNumber.Count(); i++)
                {
                    TmpMsgSqNo += TmpSQNumber[i] + "\n";
                }
            }
            else if (tbxRefID.Text != String.Empty)
            {
                string Jenis = "SQ", Kode = "SQ";
                tbxSQID.Text = ConnectionString.GenerateSequenceNo(Jenis, Kode, "SalesQuotationH", tbxRefID.Text, Conn, Trans, Cmd);
                SalesQuotationNo = tbxSQID.Text;
                CustID = tbxCustID.Text;
                CustName = tbxCustName.Text;

                insertSQHeader(SalesQuotationNo, OrderDate, SalesMouNo, RefTransId, CustID, CustName, CurrencyID, ExchRate, Total, Total_Disk, PPN, Total_PPN, PPH, Total_PPH, Total_Nett, Total_Bonus, Total_Cashback, TermofPayment, PaymentModeID, DPType, DPPercent, DPAmount, DPDueDate, Notes, TransStatus, Total_LogisticAmount, TransType, ValidTo, ValidFrom);
                SaveNewSQDetailWithRef();

                //UPDATE OLD SQ STATUS
                Query = "Update [dbo].[SalesQuotationH] SET [TransStatus] = '21',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' where [SalesQuotationNo] = '" + tbxRefID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                ListMethod.insertLogTable("E", "SalesQuotation_LogTable", tbxRefID.Text, "", "SalesQuotation", "21");           
            }

            ListMethod.insertLogTable("N", "SalesQuotation_LogTable", tbxSQID.Text, "", "SalesQuotation", TransStatus);           
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation() == 'X')
                return;

            try
            {
                TmpMsgSqNo = "";
                if (String.IsNullOrEmpty(tbxDPAmount.Text.Trim()))
                    tbxDPAmount.Text = "0";
                using (scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    Conn2 = ConnectionString.GetConnection();
                    TmpSQNumber = new string[listBox1.Items.Count];

                    SetupSaveVariable();
                    if (Mode == "New")
                    {
                        SaveNew();
                    }
                    else if (Mode != "New")
                    {
                        SalesQuotationNo = tbxSQID.Text;
                        Cmd = new SqlCommand("Select [PaymentModeID] from [dbo].[PaymentMode] where [PaymentModeName] = '" + cbPaymentMode.Text + "'", Conn2);
                        PaymentModeID = Cmd.ExecuteScalar().ToString();

                        Cmd = new SqlCommand("Select [TransStatus] from [dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn2);
                        if (Cmd.ExecuteScalar().ToString() == "05")
                            TransStatus = "22";

                        updateSQHeader(CurrencyID, ExchRate, Total, Total_Disk, PPN, Total_PPN, PPH, Total_PPH, Total_Nett, Total_Bonus, Total_Cashback, TermofPayment, PaymentModeID, DPType, DPPercent, DPAmount, DPDueDate, Notes, TransStatus, Total_LogisticAmount, TransType, SalesQuotationNo, ValidTo, ValidFrom);

                        //checkIfItemStillExist(SalesQuotationNo);

                        SaveEditSQDetail();

                        ListMethod.insertLogTable("E", "SalesQuotation_LogTable", tbxSQID.Text, "", "SalesQuotation", TransStatus);           
                    }

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

                    Conn2.Close();
                    Conn.Close();
                    scope.Complete();
                }
                if (Mode == "New")
                {
                    if (tbxRefID.Text == String.Empty)
                        MetroFramework.MetroMessageBox.Show(this, TmpMsgSqNo + "\nSave Successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else if (tbxRefID.Text != String.Empty)
                        MetroFramework.MetroMessageBox.Show(this, tbxSQID.Text + "\nSave Successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MetroFramework.MetroMessageBox.Show(this, tbxSQID.Text + " Update Successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Mode = "BeforeEdit";
                GetDataHeader();
                ModeBeforeEdit();
                UpdateStatusLogCustomer();
                Parent.RefreshGrid();                
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally { };
        }

        private void checkIfItemStillExist(string SQID)
        {
            string where = "";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (i >= 1 && dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() != String.Empty)
                    where += ",";
                if (dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() != String.Empty)
                    where += "'" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
            }

            //UPDATE DETAIL STATUS TO CLOSED IF GV ITEM != DB ITEM
            Query = "update SalesQuotationD set Deleted = 'Y', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesQuotationNo = '" + SQID + "' and SeqNo not in (" + where + ")";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateSQDetail(string GroupID, string SubGroup1ID, string SubGroup2ID, string ItemID, string FullItemID, string ItemName, string DeliveryMethod, decimal LogisticAmount, string LogisticNotes, DateTime ExpectedDateFrom, DateTime ExpectedDateTo, decimal Qty, string Unit, decimal Qty_Alt, string Unit_Alt, decimal ConvertionRatio, decimal Price, decimal Price_Alt, string DiscType, decimal DiscPercent, decimal DiscAmount, decimal BonusAmount, decimal CashBackAmount, decimal SubTotal, decimal SubTotal_PPN, decimal SubTotal_PPH, string Notes, string SalesQuotationNo, int SeqNo, string PLJNo, int PLJSeqNo, decimal PLJPrice)
        {
            Query = "UPDATE [dbo].[SalesQuotationD] SET [GroupID] = '" + GroupID + "'";
            Query += ",[SubGroup1ID] = '" + SubGroup1ID + "'";
            Query += ",[SubGroup2ID] = '" + SubGroup2ID + "'";
            Query += ",[ItemID] = '" + ItemID + "'";
            Query += ",[FullItemID] = '" + FullItemID + "'";
            Query += ",[ItemName] = '" + ItemName + "'";
            Query += ",[DeliveryMethod] = '" + DeliveryMethod + "'";
            Query += ",[LogisticAmount] = '" + LogisticAmount + "'";
            Query += ",[LogisticNotes] = @LogisticNotes";
            Query += ", [ExpectedDateFrom] = @ExpectedDateFrom";
            Query += ",[ExpectedDateTo] = @ExpectedDateTo";
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
            Query += ", PLJNo = @PLJNo";
            Query += ", PLJSeqNo = @PLJSeqNo";
            Query += ", PLJPrice = @PLJPrice";
            Query += ",[UpdatedDate] = getdate()";
            Query += ",[UpdatedBy] = '" + ControlMgr.UserId + "' ";
            Query += "WHERE [SalesQuotationNo] = '" + SalesQuotationNo + "' ";
            Query += "and [SeqNo] = '" + SeqNo + "'; ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@ExpectedDateFrom", ExpectedDateFrom == new DateTime(1753, 1, 1) ? (object)DBNull.Value : ExpectedDateFrom);
            Cmd.Parameters.AddWithValue("@ExpectedDateTo", ExpectedDateTo == new DateTime(1753, 1, 1) ? (object)DBNull.Value : ExpectedDateTo);
            Cmd.Parameters.AddWithValue("@DiscType", DiscType == String.Empty ? (object)DBNull.Value : DiscType);
            Cmd.Parameters.AddWithValue("@LogisticNotes", LogisticNotes);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.Parameters.AddWithValue("@PLJNo", PLJNo == String.Empty ? (object)DBNull.Value : PLJNo);
            Cmd.Parameters.AddWithValue("@PLJSeqNo", PLJSeqNo == 0 ? (object)DBNull.Value : PLJSeqNo);
            Cmd.Parameters.AddWithValue("@PLJPrice", PLJNo == String.Empty ? (object)DBNull.Value : PLJPrice);
            Cmd.ExecuteNonQuery();
        }

        private void updateSQHeader(string CurrencyID, decimal ExchRate, decimal Total, decimal Total_Disk, decimal PPN, decimal Total_PPN, decimal PPH, decimal Total_PPH, decimal Total_Nett, decimal Total_Bonus, decimal Total_Cashback, string TermofPayment, string PaymentModeID, string DPType, decimal DPPercent, decimal DPAmount, DateTime DPDueDate, string Notes, string TransStatus, decimal Total_LogisticAmount, string TransType, string SalesQuotationNo, DateTime ValidTo, DateTime ValidFrom)
        {
            Query = "update [dbo].[SalesQuotationH] SET [CurrencyID] = '" + CurrencyID + "'";
            Query += ",[ExchRate] = '" + ExchRate + "'";
            Query += ",[Total] = '" + Total + "'";
            Query += ",[Total_Disk] = '" + Total_Disk + "'";
            Query += ",[PPN] = '" + PPN + "'";
            Query += ",[Total_PPN] = '" + Total_PPN + "'";
            Query += ",[PPH] = '" + PPH + "'";
            Query += ",[Total_PPH] = '" + Total_PPH + "'";
            Query += ",[Total_Nett] = '" + Total_Nett + "'";
            Query += ",[Total_Bonus] = '" + Total_Bonus + "'";
            Query += ",[Total_Cashback] = '" + Total_Cashback + "'";
            Query += ",[TermofPayment] = '" + TermofPayment + "'";
            Query += ",[PaymentModeID] = @PaymentModeID";
            Query += ",[DPType] = '" + DPType + "'";
            Query += ", [DPPercent] = '" + DPPercent + "'";
            Query += ", [DPAmount] = '" + DPAmount + "'";
            Query += ",[DPDueDate] = @DpDueDate";
            Query += ",[Notes] = @Notes";
            Query += ",[TransStatus] = '" + TransStatus + "'";
            Query += ",[UpdatedDate] = getdate()";
            Query += ",[UpdatedBy] = '" + ControlMgr.UserId + "'";
            Query += ",[Total_LogisticAmount] = '" + Total_LogisticAmount + "'";
            Query += ",[TransType] = '" + TransType + "'";
            Query += ",ValidTo = @ValidTo ";
            Query += ",ValidFrom = @ValidFrom ";
            Query += "WHERE [SalesQuotationNo] = '" + SalesQuotationNo + "'; ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.Parameters.AddWithValue("@DpDueDate", DPDueDate);
            Cmd.Parameters.AddWithValue("@ValidTo", ValidTo);
            Cmd.Parameters.AddWithValue("@ValidFrom", ValidFrom);
            Cmd.Parameters.AddWithValue("@PaymentModeID", PaymentModeID);
            Cmd.ExecuteNonQuery();
        }

        private char checkPriceTolerance()
        {
            flag = '\0';
            for (int j = 0; j < dataGridView1.Rows.Count; j++)
            {
                if (dataGridView1.Rows[j].Cells["Base"].Value.ToString() != "N")
                {
                    Query = "select a.Tolerance from Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo where a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and b.Active = '1' and b.TransStatus = '03' and cast(a.PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and a.FullItemId = '" + dataGridView1.Rows[j].Cells["FullItemId"].Value + "' and a.PriceListNo = '" + dataGridView1.Rows[j].Cells["PLJNo"].Value + "' and a.SeqNo = '" + dataGridView1.Rows[j].Cells["PLJSeqNo"].Value + "'";
                    Cmd = new SqlCommand(Query, Conn2);
                    Dr = Cmd.ExecuteReader();
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Mode = "Edit";               
                ModeEdit();
                GetDataHeader();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void btnSSAID_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change MoU!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                tbxMoUID.Text = "";
                SearchV2 f = new SearchV2();
                f.SetMode("No");
                f.SetSchemaTable("dbo", "CustMouH", "and ValidTo >= DATEADD(day,-1,GETDATE())", "a.*", "CustMouH a");
                f.ShowDialog();

                if (SearchV2.data.Count != 0)
                {
                    tbxMoUID.Text = SearchV2.data[0];
                    listBox1.Items.Clear();
                    tbxCustID.Text = "";
                    tbxCustName.Text = "";

                    SearchV2 f2 = new SearchV2();
                    f2.SetMode("No");
                    f2.SetSchemaTable("dbo", "CustTable", "and CustId in (select b.CustID from CustMouH a left join CustMou_Dtl b on a.MouNo = b.MouNo where a.MouNo = '" + tbxMoUID.Text + "')", "a.*", "CustTable a");
                    f2.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        listBox1.Items.Add(SearchV2.data[0] + " " + SearchV2.data[1]);
                        tbxCustID.Text = SearchV2.data[0];
                        tbxCustName.Text = SearchV2.data[1];
                        Conn = ConnectionString.GetConnection();
                        Query = "select DP_Required from CustTable where CustId = '" + listBox1.Items[0].ToString().Split(' ')[0] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        cbDPType.Text = Cmd.ExecuteScalar().ToString();
                        Conn.Close();
                        if (cbDPType.Text == "Y")
                            cbDPType.Enabled = false;
                        else
                            cbDPType.Enabled = true;
                        CustID = listBox1.Items[0].ToString().Split(' ')[0];
                        setDefaultValue();
                    }
                }
            }

            /*ControlMgr.TblName = "CustMouH";
            Methods.ControlMgr.tmpWhere = "WHERE ValidTo >= DATEADD(day,-1,GETDATE())";
            ControlMgr.tmpSort = "ORDER BY MouDate DESC";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();
            FrmSearch.Text = "Search Customer MoU";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                listBox1.Items.Clear();
                tbxMoUID.Text = ControlMgr.Kode;
                Conn = ConnectionString.GetConnection();
                Query = "SELECT CM.CustID,CT.CustName FROM CustMou_Dtl CM LEFT JOIN CustTable CT ON CT.CustId=CM.CustID WHERE MouNo='" + ControlMgr.Kode + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    listBox1.Items.Add(Dr["CustID"] + " " + Dr["CustName"]);
                }
                Conn.Close();
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            Methods.ControlMgr.tmpWhere = "";
            ControlMgr.Kode = "";*/
        }

        private void btnAddItem_Click(object sender, EventArgs e)
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
                f.SetSchemaTable("dbo", "SalesQuotationD", "and a.SalesQuotationNo = '" + tbxRefID.Text + "' and a.Deleted = 'N'" + excludeItem, "a.*", "SalesQuotationD a");
                f.ShowDialog();
                if (SearchV2.data.Count != 0)
                {
                    string where = "";
                    for (int i = 0; i < SearchV2.data.Count; i++)
                    {
                        if (i >= 1)
                            where += " or ";
                        where += "(SalesQuotationNo = '" + SearchV2.data[i] + "' and SeqNo = '" + SearchV2.data2[i] + "')";
                    }
                    Conn = ConnectionString.GetConnection();
                    Query = "Select * from SalesQuotationD where " + where;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        #region pass value to GV 1 & 2
                        dataGridView1.Rows.Add(1);
                        for (int i = 0; i < tableCols.Length; i++)
                        {
                            if (!(tableCols[i] == "SalesQuotationNo" || tableCols[i] == "SeqNo"))
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
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["SalesQuotationNo"];
                                else if (tableCols[i] == "RefTrans_SeqNo")
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr["SeqNo"];
                                else
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                            }
                        }

                        dataGridView2.Rows.Add(1);
                        for (int i = 0; i < tableCols2.Length; i++)
                        {
                            if (i == 0)
                                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[0].Value = dataGridView2.RowCount;
                            else if (tableCols2[i] == "DiscType")
                            {
                                cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                                cell.Value = "Select";
                                dataGridView2.Rows[(dataGridView2.Rows.Count - 1)].Cells["DiscType"] = cell;
                            }
                            else if (tableCols2[i] == "ExpectedDateFrom" || tableCols2[i] == "ExpectedDateTo")
                                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[tableCols2[i]].Value = Convert.ToDateTime(Dr[tableCols2[i]]);
                            else
                                dataGridView2.Rows[dataGridView1.RowCount - 1].Cells[tableCols2[i]].Value = Dr[tableCols2[i]];
                        }
                        #endregion
                    }
                    Dr.Close();
                    Conn.Close();
                }
            }
            else
            {
                string criteria = " and b.Criteria in ('1', ";
                string wherePlus = "";
                Conn = ConnectionString.GetConnection();
                string customerID = "";
                if (tbxCustID.Text == String.Empty)
                    customerID = listBox1.Items[0].ToString().Split(' ')[0];
                else
                    customerID = tbxCustID.Text;
                Query = "select a.FullItemId, a.DeliveryMethod from Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo left join PriceList_AccountList c on c.PriceListNo = b.PricelistNo where 1=1 and a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and CAST(a.PriceType as int) = '2' and b.Active = '1' and b.TransStatus = '03' and c.AccountId = '" + customerID + "'";
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
                if (cbType.Text == "FIX")
                    f.SetMode("Check");
                else
                    f.SetMode("No");

                string where = "and type = 'sales' ";
                where += "and ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) ";
                where += "and  ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) ";
                where += "and CAST(PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' ";
                where += "and Active = '1' and TransStatus = '03')a";

                //string columnName = "case when a.PricelistNo is null then b.PricelistNo else a.PricelistNo end 'PricelistNo', case when a.SeqNo is null then b.SeqNo else a.SeqNo end 'SeqNo',case when a.AccountId is null then b.AccountId else a.AccountId end 'AccountId',case when a.FullItemId is null then b.FullItemId else a.FullItemId end 'FullItemId',case when a.ItemName is null then b.ItemName else a.ItemName end 'ItemName',case when a.Price is null then b.Price else a.Price end 'Price',case when a.DeliveryMethod is null then b.DeliveryMethod else a.DeliveryMethod end 'DeliveryMethod',case when a.Type is null then b.Type else a.Type end 'Type',case when a.ValidFrom is null then b.ValidFrom else a.ValidFrom end 'ValidFrom',case when a.ValidTo is null then b.ValidTo else a.ValidTo end 'ValidTo',case when a.PriceType is null then b.PriceType else a.PriceType end 'PriceType',case when a.Active is null then b.Active else a.Active end 'Active',case when a.TransStatus is null then b.TransStatus else a.TransStatus end 'TransStatus'";
                //string table = "select a.PricelistNo, a.SeqNo, c.AccountId, a.FullItemId, a.ItemName, a.Price, a.DeliveryMethod, a.Type, b.ValidFrom, b.ValidTo, a.PriceType, b.Active, b.TransStatus from Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo left join PriceList_AccountList c on c.PriceListNo = b.PricelistNo where 1=1 and a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and CAST(a.PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and b.Active = '1' and b.TransStatus = '03'";

                string columnName = "case when a.PricelistNo is null then b.PricelistNo else a.PricelistNo end 'PricelistNo',";
                columnName += "case when a.SeqNo is null then b.SeqNo else a.SeqNo end 'SeqNo',";
                columnName += "case when a.AccountId is null then b.AccountId else a.AccountId end 'AccountId',";
                columnName += "case when a.FullItemId is null then b.FullItemId else a.FullItemId end 'FullItemId',";
                columnName += "case when a.ItemName is null then b.ItemName else a.ItemName end 'ItemName',";
                columnName += "case when a.Price is null then b.Price else a.Price end 'Price',";
                columnName += "case when a.DeliveryMethod is null then b.DeliveryMethod else a.DeliveryMethod end 'DeliveryMethod',";
                columnName += "case when a.Type is null then b.Type else a.Type end 'Type',";
                columnName += "case when a.ValidFrom is null then b.ValidFrom else a.ValidFrom end 'ValidFrom',";
                columnName += "case when a.ValidTo is null then b.ValidTo else a.ValidTo end 'ValidTo',";
                columnName += "case when a.PriceType is null then b.PriceType else a.PriceType end 'PriceType',";
                columnName += "case when a.Active is null then b.Active else a.Active end 'Active',";
                columnName += "case when a.TransStatus is null then b.TransStatus else a.TransStatus end 'TransStatus'";
                
                
                string table = "select a.PricelistNo, a.SeqNo, c.AccountId, a.FullItemId, a.ItemName, a.Price, a.DeliveryMethod, a.Type, b.ValidFrom, b.ValidTo, a.PriceType, b.Active, b.TransStatus ";
                table += "from Pricelist_Dtl a ";
                table += "left join PricelistH b on a.PricelistNo = b.PricelistNo ";
                table += "left join PriceList_AccountList c on c.PriceListNo = b.PricelistNo ";
                table += "where 1=1 and a.type = 'sales' ";
                table += "and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) ";
                table += "and CAST(a.PriceType as int) >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' ";
                table += "and b.Active = '1' and b.TransStatus = '03'";
                
                string tableName2 = "(select * from (Select " + columnName + " from (" + table + wherePlus + criteria + " )a full join (" + table + " and c.AccountId = '" + customerID + "') b on a.PricelistNo = b.PricelistNo )a";

                f.SetSchemaTable("dbo", "Pricelist_Dtl", where, "case when a.PricelistNo is null then b.PricelistNo else a.PricelistNo end, case when a.SeqNo is null then b.SeqNo else a.SeqNo end ,case when a.AccountId is null then b.AccountId else a.AccountId end ,case when a.FullItemId is null then b.FullItemId else a.FullItemId end ,case when a.ItemName is null then b.ItemName else a.ItemName end ,case when a.Price is null then b.Price else a.Price end ,case when a.DeliveryMethod is null then b.DeliveryMethod else a.DeliveryMethod end", tableName2);
                f.ShowDialog();
                if (cbType.Text != "FIX")
                {
                    if (SearchV2.data.Count != 0)
                    {
                        SearchV2 f2 = new SearchV2();
                        f2.SetMode("No");
                        pricelistDtl.Clear();
                        where = "and a.ItemId = '" + SearchV2.data[2] + "'";
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
                        }
                    }
                }

                //PASS DATA TO GV
                if (SearchV2.data.Count != 0)
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
                    if (dataGridView2.RowCount - 1 <= 0)
                    {
                        dataGridView2.ColumnCount = tableCols2.Length;
                        for (int i = 0; i < tableCols2.Length; i++)
                        {
                            dataGridView2.Columns[i].Name = tableCols2[i];
                        }
                    }
                    Conn = ConnectionString.GetConnection();
                    if (cbType.Text == "FIX")
                    {
                        Query = "select a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.FullItemId, a.ItemName, b.UoM, b.UoMAlt, c.Ratio, a.PricelistNo, a.SeqNo, a.Price, a.DeliveryMethod from Pricelist_Dtl a left join InventTable b on a.FullItemId = b.FullItemID left join InventConversion c on a.FullItemId = c.FullItemID where ";
                        for (int i = 0; i < SearchV2.data.Count; i++)
                        {
                            if (i >= 1)
                                Query += " or ";
                            Query += "a.PricelistNo = '" + SearchV2.data[i] + "' and ";
                            Query += "a.SeqNo = '" + SearchV2.data2[i] + "'";
                        }
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            //GRIDVIEW 1 TAB SALES
                            #region dataGridView1.Rows.Add
                            this.dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "");
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["GroupID"].Value = Dr[0];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SubGroup1ID"].Value = Dr[1];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SubGroup2ID"].Value = Dr[2];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ItemID"].Value = Dr[3];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["FullItemID"].Value = Dr[4];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ItemName"].Value = Dr[5];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Unit"].Value = Dr["UoM"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price"].Value = Convert.ToDecimal(Dr["Price"]) * Convert.ToDecimal(Dr["Ratio"]);
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price_Alt"].Value = Convert.ToDecimal(Dr["Price"]);
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Unit_Alt"].Value = Dr["UoMAlt"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ConvertionRatio"].Value = Dr["Ratio"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["PLJNo"].Value = Dr["PricelistNo"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["PLJSeqNo"].Value = Dr["SeqNo"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["PLJPrice"].Value = Dr["Price"];

                            cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                            cell.Value = "Select";
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                            #endregion

                            //GRIDVIEW 2 TAB DELIVERY
                            this.dataGridView2.Rows.Add(dataGridView2.RowCount + 1, Dr["FullItemId"], Dr["ItemName"]);
                            dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["DeliveryMethod"].Value = Dr["DeliveryMethod"];
                        }
                        Dr.Close();
                    }
                    else
                    {
                        Query = "select a.GelombangId, a.BracketId, a.Type, a.ItemId 'FullItemId', a.ItemName, a.Base, a.Price, a.SeqNo, b.Ratio, c.GroupID, c.SubGroup1ID, c.SubGroup2ID, c.ItemID , c.UoM, c.UoM_AvgPrice, c.UoMAlt, d.DeliveryMethod, d.Price 'PLJPrice', d.SeqNo 'PLJSeqNo', d.PricelistNo from InventGelombangD a left join InventConversion b on a.ItemId = b.FullItemId left join InventTable c on a.ItemId = c.FullItemID left join Pricelist_Dtl d on d.FullItemId = a.ItemId left join PricelistH e on e.PricelistNo = d.PricelistNo where (";
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
                            //GRIDVIEW2 TAB DELIVERY
                            #region dataGridView2.Rows.Add
                            this.dataGridView2.Rows.Add(dataGridView2.RowCount + 1, Dr["FullItemID"], Dr["ItemName"]);
                            if (Dr["Base"].ToString() == "Y")
                            {
                                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["DeliveryMethod"].Value = Dr["DeliveryMethod"];
                            }
                            #endregion

                            //GRIDVIEW1 TAB SALES
                            #region dataGridView1.Rows.Add
                            dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "");
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["GroupID"].Value = Dr["GroupID"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SubGroup1ID"].Value = Dr["SubGroup1ID"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SubGroup2ID"].Value = Dr["SubGroup2ID"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ItemID"].Value = Dr["ItemID"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["FullItemID"].Value = Dr["FullItemId"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ItemName"].Value = Dr["ItemName"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Base"].Value = Dr["Base"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Unit"].Value = Dr["UoM"];
                            if (Dr["Base"].ToString() == "Y")
                            {
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price"].Value = Convert.ToDecimal(Dr["PLJPrice"]) * Convert.ToDecimal(Dr["Ratio"]);
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price_Alt"].Value = Convert.ToDecimal(Dr["PLJPrice"]);
                            }
                            else
                            {
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price"].Value = Convert.ToDecimal(Dr["Price"]) * Convert.ToDecimal(Dr["Ratio"]);
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Price_Alt"].Value = Convert.ToDecimal(Dr["Price"]);
                            }

                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Unit_Alt"].Value = Dr["UoMAlt"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ConvertionRatio"].Value = Dr["Ratio"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["GelombangId"].Value = Dr["GelombangId"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["BracketId"].Value = Dr["BracketId"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Gelombang_Price"].Value = Dr["Price"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["GelombangSeqNo_Base"].Value = Dr["SeqNo"];
                            if (Dr["Base"].ToString() == "Y")
                            {
                                cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                                cell.Value = "Select";
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["DiscType"] = cell;
                            }
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["PLJNo"].Value = Dr["PricelistNo"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["PLJSeqNo"].Value = Dr["PLJSeqNo"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["PLJPrice"].Value = Dr["PLJPrice"];
                            #endregion
                        }
                        Dr.Close();
                    }
                    Conn.Close();
                    //ModeNew();
                    gvFormat();
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView1.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["ItemName"].Frozen = true;

            dataGridView1.Columns["SeqNo"].Visible = false;
            dataGridView1.Columns["GroupID"].Visible = false;
            dataGridView1.Columns["SubGroup1ID"].Visible = false;
            dataGridView1.Columns["SubGroup2ID"].Visible = false;
            dataGridView1.Columns["ItemID"].Visible = false;
            if (cbType.Text == "AMOUNT" || cbType.Text == "FIX")
            {
                dataGridView1.Columns["Qty_Alt"].Visible = false;
                dataGridView1.Columns["Unit_Alt"].Visible = false;
                dataGridView1.Columns["ConvertionRatio"].Visible = true;
            }
            else
            {
                dataGridView1.Columns["Qty_Alt"].Visible = true;
                dataGridView1.Columns["Unit_Alt"].Visible = true;
                dataGridView1.Columns["ConvertionRatio"].Visible = true;
            }
            //dataGridView1.Columns["Price_Alt"].Visible = false;
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
            else if (cbType.Text == "QUANTITY")
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

            //if (Mode == "New" || Mode == "BeforeEdit")
            //{
            dataGridView1.Columns["RefTransId"].Visible = false;
            dataGridView1.Columns["RefTrans_SeqNo"].Visible = false;
            //}
            dataGridView1.Columns["PLJNo"].Visible = false;
            dataGridView1.Columns["PLJSeqNo"].Visible = false;
            dataGridView1.Columns["PLJPrice"].Visible = false;

            dataGridView1.Columns["SubTotal_PPH"].Visible = false;

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
                if (e.Value == "" || e.Value == null || e.Value == (object)DBNull.Value)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("ExpectedDateTo"))
                dataGridView1.Columns["ExpectedDateTo"].DefaultCellStyle.Format = "dd/MM/yyyy";

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("ExpectedDateFrom"))
            {
                dataGridView1.Columns["ExpectedDateFrom"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }

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

                if (cbType.Text == "QUANTITY")
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

                //QUANTITY
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

                //STotal_PPN = (STotal - DiscAmount) * Convert.ToDecimal(cbPPN.Text) / 100;
                //dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = STotal_PPN;

                //STotal_PPH = (STotal - DiscAmount) * Convert.ToDecimal(cbPPh.Text) / 100;
                //dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = STotal_PPH;

                if (cbPPN.Text != "Select")
                    STotal_PPN = (STotal - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPN.Text) / 100;
                else
                    STotal_PPN = 0;
                dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = STotal_PPN;

                if (cbPPh.Text != "Select")
                    STotal_PPH = (STotal - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPh.Text) / 100;
                else
                    STotal_PPH = 0;
                dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = STotal_PPH;                
            }

            tbxSTotal.Text = "0"; tbxGPPN.Text = "0"; tbxGPPh.Text = "0"; tbxGDisc.Text = "0";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                tbxSTotal.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal"].Value) + Convert.ToDecimal(tbxSTotal.Text)).ToString()));
                tbxGPPN.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value) + Convert.ToDecimal(tbxGPPN.Text)).ToString()));
                tbxGPPh.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value) + Convert.ToDecimal(tbxGPPh.Text)).ToString()));
                tbxGDisc.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value) + Convert.ToDecimal(tbxGDisc.Text)).ToString()));
            }

            tbxGTotal.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(tbxSTotal.Text) + Convert.ToDecimal(tbxGPPN.Text) + Convert.ToDecimal(tbxGPPh.Text) - Convert.ToDecimal(tbxGDisc.Text)).ToString()));
        }

        private void cbPPN_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value = Convert.ToString(((Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value)) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPN.Text) / 100);
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (cbPPN.Text != "0.00")
                    tbxGPPN.Text = string.Format("{0:#,0.00}", double.Parse(((Convert.ToDecimal(tbxSTotal.Text) - Convert.ToDecimal(tbxGDisc.Text)) * Convert.ToDecimal(cbPPN.Text) / 100/*Convert.ToDecimal(tbxGPPN.Text) + Convert.ToDecimal(dataGridView1.Rows[i].Cells["SubTotal_PPN"].Value)*/).ToString()));
                else
                {
                    tbxGPPN.Text = "0.00";
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
                dataGridView1.Rows[i].Cells["SubTotal_PPH"].Value = ((Convert.ToDecimal(dataGridView1.Rows[i].Cells["Price"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value)) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["DiscAmount"].Value)) * Convert.ToDecimal(cbPPh.Text) / 100;
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (cbPPh.Text != "0.00")
                    tbxGPPh.Text = string.Format("{0:#,0.00}", double.Parse(((Convert.ToDecimal(tbxSTotal.Text) - Convert.ToDecimal(tbxGDisc.Text)) * Convert.ToDecimal(cbPPh.Text) / 100).ToString()));
                else
                {
                    tbxGPPh.Text = "0.00";
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
                tbxDPPercent.Enabled = true;
                tbxDPAmount.Enabled = true;
                dtDP.Enabled = true;
                cbDPType.BackColor = Color.Empty;
                //hendry dtDP.Visible = true; tbxDPAmount.Visible = true; tbxDPPercent.Visible = true; label4.Visible = true; lblDueDate.Visible = true; label7.Visible = true;
            }
            else
            {
                if (cbDPType.Text == "N")
                {
                    tbxDPPercent.Enabled = false;
                    tbxDPAmount.Enabled = false;
                    dtDP.Enabled = false;
                    cbDPType.BackColor = Color.Empty;
                    //hendry tbxDPAmount.Visible = false; tbxDPPercent.Visible = false; label4.Visible = false; lblDueDate.Visible = false; label7.Visible = false; dtDP.Visible = false;
                    tbxDPAmount.Text = "0.00";
                    tbxDPPercent.Text = "0.00";
                    dtDP.Value = DateTime.Now;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";
            GetDataHeader();
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
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
            using (scope = new TransactionScope())
            {
                Conn = ConnectionString.GetConnection();
                if (btnApprove.Text == "Approve")
                {
                    Cmd = new SqlCommand("Select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn);
                    if (Cmd.ExecuteScalar().ToString() == "22")
                    {
                        Query = "Update [ISBS-NEW4].[dbo].[SalesQuotationH] SET [TransStatus] = '23',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
                        TransStatus = "23";
                    }
                    else if (Cmd.ExecuteScalar().ToString() == "02")
                    {
                        Query = "Update [ISBS-NEW4].[dbo].[SalesQuotationH] SET [TransStatus] = '03',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
                        TransStatus = "03";
                    }
                    else if (Cmd.ExecuteScalar().ToString() == "21")
                    {
                        Query = "Update [ISBS-NEW4].[dbo].[SalesQuotationH] SET [TransStatus] = '23',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
                        TransStatus = "23";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                }
                else if (btnApprove.Text == "UnApprove")
                {
                    Query = "update SalesQuotationH set TransStatus = '02',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    TransStatus = "02";
                }

                ListMethod.insertLogTableFollowParentAct("SalesQuotation_LogTable", tbxSQID.Text, "", "SalesQuotation", TransStatus);           

                CustID = tbxCustID.Text;
                UpdateStatusLogCustomer();
                Conn.Close();
                scope.Complete();
            }
            if (btnApprove.Text == "Approve")
                MetroFramework.MetroMessageBox.Show(this, tbxSQID.Text + " approved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MetroFramework.MetroMessageBox.Show(this, tbxSQID.Text + " un-approved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnRevision_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Update [dbo].[SalesQuotationH] SET [TransStatus] = '05',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            
            //insertStatusLogRevision();
            UpdateStatusLogCustomer();
            
            MessageBox.Show(tbxSQID.Text + " will be revised!", "Information");
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            using (scope = new TransactionScope())
            {
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("Select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + tbxSQID.Text + "'", Conn);
                if (Cmd.ExecuteScalar().ToString() == "21")
                {
                    Query = "Update [dbo].[SalesQuotationH] SET [TransStatus] = '22',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
                    TransStatus = "22";
                }
                else
                {
                    Query = "Update [dbo].[SalesQuotationH] SET [TransStatus] = '04',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' where [SalesQuotationNo] = '" + tbxSQID.Text + "'";
                    TransStatus = "04";
                }
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                ListMethod.insertLogTableFollowParentAct("SalesQuotation_LogTable", tbxSQID.Text, "", "SalesQuotation", TransStatus);           

                CustID = tbxCustID.Text;
                UpdateStatusLogCustomer();

                Conn.Close();
                scope.Complete();
            }
            MetroFramework.MetroMessageBox.Show(this, tbxSQID.Text + " rejected!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnAmend_Click(object sender, EventArgs e)
        {
            ModeEdit();
            GetDataHeader();
            tbxRefID.Text = tbxSQID.Text;
            tbxSQID.Text = "";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells["RefTransId"].Value = tbxRefID.Text;
                dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value = dataGridView1.Rows[i].Cells["SeqNo"].Value;
                dataGridView1.Rows[i].Cells["SeqNo"].Value = "";
            }
            Mode = "New";
            dataGridView1.Columns["Price"].ReadOnly = true;
            dataGridView1.Columns["Price"].DefaultCellStyle.BackColor = Color.LightGray;
        }

        //private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right && e.RowIndex > -1)
        //    {
        //        if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
        //        {
        //            PopUp.Stock.Stock f = new PopUp.Stock.Stock();
        //            itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
        //            f.Show();
        //        }
        //        if (Mode != "BeforeEdit")
        //        {
        //            if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Qty")
        //            {
        //                SearchV2 f = new SearchV2();
        //                f.SetMode("No");
        //                f.SetSchemaTable("dbo", "Invent_OnHand_Qty", "and a.FullItemID = '" + dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'", "a.*", "Invent_OnHand_Qty a");
        //                f.ShowDialog();
        //            }
        //            if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Price")
        //            {
        //                SearchV2 f = new SearchV2();
        //                f.SetMode("No");
        //                string where = "and a.type = 'sales' and b.ValidFrom <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 1) and  b.ValidTo >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) and a.PriceType >= '" + Regex.Replace(cbToP.Text, "[^0-9.]", "") + "' and b.Active = '1' and b.TransStatus = '03' and a.FullItemID = '" + dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'";
        //                f.SetSchemaTable("dbo", "Pricelist_Dtl", where, "a.*", "Pricelist_Dtl a left join PricelistH b on a.PricelistNo = b.PricelistNo");
        //                f.ShowDialog();
        //                if (SearchV2.data.Count != 0)
        //                {
        //                    Conn = ConnectionString.GetConnection();
        //                    Query = "select Price from Pricelist_Dtl where PricelistNo = '" + SearchV2.data[1] + "' and SeqNo = '" + SearchV2.data[2] + "'";
        //                    Cmd = new SqlCommand(Query, Conn);
        //                    decimal price = Convert.ToDecimal(Cmd.ExecuteScalar());
        //                    Conn.Close();
        //                    dataGridView1.Rows[e.RowIndex].Cells["Price"].Value = price;
        //                    dataGridView1.Rows[e.RowIndex].Cells["PLJNo"].Value = SearchV2.data[1];
        //                    dataGridView1.Rows[e.RowIndex].Cells["PLJSeqNo"].Value = SearchV2.data[2];
        //                    dataGridView1.Rows[e.RowIndex].Cells["PLJPrice"].Value = price;
        //                }
        //            }
        //        }
        //    }
        //}

        private void btnDeleteItem_Click(object sender, EventArgs e)
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
                        dataGridView2.Rows.RemoveAt(indexData[i]);
                    }
                }
                else
                {
                    int rowIndex = dataGridView1.CurrentRow.Index;
                    dataGridView1.Rows.RemoveAt(rowIndex);
                    dataGridView2.Rows.RemoveAt(rowIndex);
                }
                populateGVData();
            }
        }

        private void btnDeleteCust_Click(object sender, EventArgs e)
        {
            //if (dataGridView2.RowCount != 0)
            //    dataGridView2.Rows.RemoveAt(dataGridView2.CurrentRow.Index);
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
            if (tbxDPAmount.Text == String.Empty)
                tbxDPAmount.Text = "0.00";
            else
                tbxDPAmount.Text = string.Format("{0:#,0.00}", double.Parse(Convert.ToString(Convert.ToDecimal(tbxDPAmount.Text))));

            if (Convert.ToDecimal(tbxGTotal.Text) != 0)
                tbxDPPercent.Text = string.Format("{0:#,0.00}", double.Parse(Convert.ToString(Convert.ToDecimal(tbxDPAmount.Text) / Convert.ToDecimal(tbxGTotal.Text) * 100)));
        }

        private void tbxXRate_Leave(object sender, EventArgs e)
        {
            if (tbxXRate.Text == String.Empty)
                tbxXRate.Text = "0";
            tbxXRate.Text = string.Format("{0:#,0.00}", double.Parse(tbxXRate.Text));
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
            dataGridView2.CurrentCell.Value = dtp.Text;
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

        private void cbSAType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                if (cbType.Text != oldSAType)
                {
                    DialogResult result = MetroFramework.MetroMessageBox.Show(this, "Item detail will be cleared.\r\nAre you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        oldSAType = cbType.Text;
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                        dataGridView2.Rows.Clear();
                        dataGridView2.Columns.Clear();
                    }
                    else
                        cbType.Text = oldSAType;
                }
            }
            else
                oldSAType = cbType.Text;
        }

        private void cbSAType_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnAddCust_Click_1(object sender, EventArgs e)
        {
            /*//Jika bukan Mou maka search all customer yang tidak ada di list 
            if (tbxMoUID.Text.Trim() == "")
            {
                ControlMgr.TblName = "CustTable";
                string CustID = String.Empty;
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (i >= 1)
                        CustID += ",";
                    CustID += "'" + listBox1.Items[i].ToString().Split(' ')[0] + "'";
                }
                if (CustID == String.Empty)
                {
                    CustID = "''";
                }
                else
                {
                    Methods.ControlMgr.tmpWhere = "WHERE CustID not in (" + CustID + ")";
                }
                ControlMgr.tmpSort = "ORDER BY CustId DESC";

                Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();
                FrmSearch.Text = "Search Customer";
                FrmSearch.ShowDialog();

                if (ControlMgr.Kode != "")
                {
                    listBox1.Items.Add(ControlMgr.Kode + " " + ControlMgr.Kode2);
                }

                ControlMgr.TblName = "";
                ControlMgr.tmpSort = "";
                Methods.ControlMgr.tmpWhere = "";
                ControlMgr.Kode = "";
                ControlMgr.Kode2 = "";
            }
            //End Bukan Mou

            //Jika Mou maka search customer yang ada di mou dan tidak ada di list
            else
            {
                ControlMgr.TblName = "CustTable";
                Methods.ControlMgr.tmpWhere = "WHERE CustID in (SELECT CustID FROM CustMou_Dtl WHERE MouNo='" + tbxMoUID.Text.Trim() + "')";

                string CustID = String.Empty;
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (i >= 1)
                        CustID += ",";
                    CustID += "'" + listBox1.Items[i].ToString().Split(' ')[0] + "'";
                }
                if (CustID == String.Empty)
                {
                    CustID = "''";
                }
                else
                {
                    Methods.ControlMgr.tmpWhere += " AND CustID not in (" + CustID + ")";
                }
                ControlMgr.tmpSort = "ORDER BY CustId DESC";

                Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();
                FrmSearch.Text = "Search Customer";
                FrmSearch.ShowDialog();

                if (ControlMgr.Kode != "")
                {
                    listBox1.Items.Add(ControlMgr.Kode + " " + ControlMgr.Kode2);
                }

                ControlMgr.TblName = "";
                ControlMgr.tmpSort = "";
                Methods.ControlMgr.tmpWhere = "";
                ControlMgr.Kode = "";
                ControlMgr.Kode2 = "";
            }
            //end Mou

            return;*/

            if (dataGridView1.RowCount != 0)
            {
                MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change Customer!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                listBox1.Items.Clear();
                SearchV2 f = new SearchV2();
                f.SetMode("No");
                if (tbxMoUID.Text == String.Empty)
                    f.SetSchemaTable("dbo", "CustTable", "", "a.*", "CustTable a");
                else
                    f.SetSchemaTable("dbo", "CustTable", "and CustId in (select b.CustID from CustMouH a left join CustMou_Dtl b on a.MouNo = b.MouNo where a.MouNo = '" + tbxMoUID.Text + "')", "a.*", "CustTable a");

                f.ShowDialog();
                if (SearchV2.data.Count != 0)
                {
                    //if (tbxMoUID.Text == String.Empty)
                    listBox1.Items.Add(SearchV2.data[0] + " " + SearchV2.data[1]);

                    tbxCustID.Text = SearchV2.data[0];
                    tbxCustName.Text = SearchV2.data[1]; 
                    //else
                    //    listBox1.Items.Add(SearchV2.data[2] + " " + SearchV2.data[3]);
                    Conn = ConnectionString.GetConnection();
                    Query = "select DP_Required from CustTable where CustId = '" + listBox1.Items[0].ToString().Split(' ')[0] + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    cbDPType.Text = Cmd.ExecuteScalar().ToString();
                    Conn.Close();
                    if (cbDPType.Text == "Y")
                        cbDPType.Enabled = false;
                    else
                        cbDPType.Enabled = true;

                    CustID = listBox1.Items[0].ToString().Split(' ')[0];
                    setDefaultValue();
                }
            }
        }

        private void btnDelCust_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change Customer!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (listBox1.SelectedIndex != -1)
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

        private void tbxDPPercent_Leave(object sender, EventArgs e)
        {
            if (tbxDPPercent.Text == String.Empty)
                tbxDPPercent.Text = "0.00";
            tbxDPAmount.Text = string.Format("{0:#,0.00}", double.Parse(Convert.ToString(Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDPPercent.Text) / 100)));
        }

        private void tbxGTotal_TextChanged(object sender, EventArgs e)
        {
            tbxDPAmount.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(tbxGTotal.Text) * Convert.ToDecimal(tbxDPPercent.Text) / 100).ToString()));
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            tbxGBonus.Text = "0"; tbxGCashback.Text = "0"; tbxLogAmount.Text = "0";
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                tbxLogAmount.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView2.Rows[i].Cells["LogisticAmount"].Value) + Convert.ToDecimal(tbxLogAmount.Text)).ToString()));
                tbxGBonus.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView2.Rows[i].Cells["BonusAmount"].Value) + Convert.ToDecimal(tbxGBonus.Text)).ToString()));
                tbxGCashback.Text = string.Format("{0:#,0.00}", double.Parse((Convert.ToDecimal(dataGridView2.Rows[i].Cells["CashBackAmount"].Value) + Convert.ToDecimal(tbxGCashback.Text)).ToString()));
            }
        }

        private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
            {
                dtp.Location = dataGridView2.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                dtp.Visible = true;

                if (dataGridView2.CurrentCell.Value != "" && dataGridView2.CurrentCell.Value != null)
                {
                    //DateTime dDate;
                    //if (!DateTime.TryParse(dataGridView2.CurrentCell.Value.ToString(), out dDate))
                    //{
                    //    dtp.Value = Convert.ToDateTime(FormateDateddmmyyyy(dataGridView2.CurrentCell.Value.ToString()));
                    //}
                    //else
                    //{
                    if (dataGridView2.CurrentCell.Value.GetType() == typeof(DateTime))
                        dtp.Value = Convert.ToDateTime(dataGridView2.CurrentCell.Value);
                    else
                        dtp.Value = DateTime.ParseExact(dataGridView2.CurrentCell.Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //}
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
            {
                if (dataGridView2.CurrentCell.Value != "" && dataGridView2.CurrentCell.Value != null)
                {
                    dataGridView2.CurrentCell.Value = dtp.Value;
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
            dtp.Visible = false;
            dataGridView2.AutoResizeColumns();
        }

        private void dataGridView2_CellFormatting_1(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns["ItemName"].Frozen = true;

            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("Amount") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("Price") || dataGridView2.Columns[e.ColumnIndex].Name.Contains("Total"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("ExpectedDateTo"))
                dataGridView2.Columns["ExpectedDateTo"].DefaultCellStyle.Format = "dd/MM/yyyy";

            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("ExpectedDateFrom"))
            {
                dataGridView2.Columns["ExpectedDateFrom"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }

        //private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right && e.RowIndex > -1)
        //    {
        //        if (dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
        //        {
        //            PopUp.Stock.Stock f = new PopUp.Stock.Stock();
        //            itemID = dataGridView2.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
        //            f.Show();
        //        }
        //        if (Mode != "BeforeEdit")
        //        {
        //            if (dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "Qty")
        //            {
        //                SearchV2 f = new SearchV2();
        //                f.SetMode("No");
        //                f.SetSchemaTable("dbo", "Invent_OnHand_Qty", "and a.FullItemID = '" + dataGridView2.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'", "a.*", "Invent_OnHand_Qty a");
        //                f.ShowDialog();
        //            }
        //            if (dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "Price")
        //            {
        //                SearchV2 f = new SearchV2();
        //                f.SetMode("No");
        //                f.SetSchemaTable("dbo", "SalesPriceListDtl", "and a.FullItemID = '" + dataGridView2.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString() + "'", "a.*", "SalesPriceListDtl a");
        //                f.ShowDialog();
        //            }
        //        }
        //    }
        //}

        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView2.Columns[dataGridView2.CurrentCell.ColumnIndex].Name.Contains("Amount") || dataGridView2.Columns[dataGridView2.CurrentCell.ColumnIndex].Name.Contains("Price") || dataGridView2.Columns[dataGridView2.CurrentCell.ColumnIndex].Name.Contains("Total") || dataGridView2.Columns[dataGridView2.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView2.Columns[dataGridView2.CurrentCell.ColumnIndex].Name.Contains("Disc"))
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void cbToP_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true; //hendry
        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroTabControl1.SelectedTab == tabItem || metroTabControl1.SelectedTab == tabBonus || metroTabControl1.SelectedTab == tabLogistic)
            {
                if (cbToP.Text == "Select" || cbPaymentMode.Text == "Select" || (listBox1.Items.Count == 0 && tbxCustID.Text == String.Empty))
                {
                    string oldTab = Regex.Replace(metroTabControl1.SelectedTab.Text, "[^a-zA-Z0-9]", "");
                    metroTabControl1.SelectedTab = tabSales;

                    if (validate == false)
                        label = new Label[20];
                    createLabel(cbToP, lblToP, tabSales, "string");
                    createLabel(cbPaymentMode, lblPaymentMode, tabSales, "string");
                    count = 0;
                    validate = true;

                    List<string> msg2 = new List<string>();

                    msg = "Please Select ";
                    if (cbToP.Text == "Select")
                        msg2.Add("Term of Payment");
                    if (cbPaymentMode.Text == "Select")
                        msg2.Add("Payment Mode");
                    if (listBox1.Items.Count == 0)
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

        public void ParentRefreshGrid(Sales.SalesOrder.SOHeader so)
        {
            ParentToSO = so;
        }

        private void tbxCustID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    tbxCustID.ReadOnly = false;
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

        private void tbxMoUID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MOUID == null || MOUID.Text == "")
                {
                    tbxMoUID.Enabled = true;
                    MOUID = new Sales.MoUCustomer.HeaderMoUCustomer();
                    MOUID.SetMode("PopUp", tbxMoUID.Text);
                    MOUID.ParentRefreshGrid3(this);
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

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
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

        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dataGridView2.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dataGridView2.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dataGridView2.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }

        }
        //end

        private void setDefaultValue()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT * FROM [CustTable] WHERE [CustId] = '" + CustID + "'";
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

        private void btnSCust_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                MetroFramework.MetroMessageBox.Show(this, "Must delete all items to change Customer!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                listBox1.Items.Clear();
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
                    //if (tbxMoUID.Text == String.Empty)
                    listBox1.Items.Add(SearchV2.data[0] + " " + SearchV2.data[1]);
                    tbxCustID.Text = SearchV2.data[0];
                    tbxCustName.Text = SearchV2.data[1];
                    //else
                    //    listBox1.Items.Add(SearchV2.data[2] + " " + SearchV2.data[3]);
                    Conn = ConnectionString.GetConnection();
                    Query = "select DP_Required from CustTable where CustId = '" + listBox1.Items[0].ToString().Split(' ')[0] + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    cbDPType.Text = Cmd.ExecuteScalar().ToString();
                    Conn.Close();
                    if (cbDPType.Text == "Y")
                        cbDPType.Enabled = false;
                    else
                        cbDPType.Enabled = true;

                    CustID = listBox1.Items[0].ToString().Split(' ')[0];
                    setDefaultValue();
                }
            }
        }

        //Created by Thaddaeus, 12 Sept2018
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

        private void tbxMoUID_TextChanged(object sender, EventArgs e)
        {
           
        }

       
        //end======================================================================================

    }
}
