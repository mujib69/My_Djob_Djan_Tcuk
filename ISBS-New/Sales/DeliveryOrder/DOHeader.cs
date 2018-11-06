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
using ISBS_New.Inventory.NotaTransfer;

//BY: HC
namespace ISBS_New.Sales.DeliveryOrder
{
    public partial class DOHeader : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
        private SqlConnection Conn2;
        private SqlCommand Cmd;
        //private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        private TransactionScope scope;
        bool Journal = false;
        /**********SQL*********/

        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "No", "SeqNo", "SO_SeqNo", "RefTransId", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemId", "ItemName", "Qty", "RemainingQty", "Unit", "Qty Alt", "Unit Alt", "Ratio" };

        string[] tableCols2 = new String[] { "No", "SeqNo", "SO_SeqNo", "SOID", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemId", "ItemName", "SO Qty", "SO Remaining Qty", "Qty Availble For Sale", "Qty Available", "SO Reserved Qty", "Qty Reserved", "DO Qty", "Unit", "Qty Alt", "Unit Alt", "Ratio" };

        string[] tableCols3 = new String[] { "No", "NT No", "Seq_No", "FullItemId", "Item Name", "NT Warehouse From", "Transfer Qty", "Transfer Qty Remaining", "Unit" };

        string[] tableCols4 = new String[] { "No", "NRJId", "NRJ_SeqNo", "SeqNo", "GroupID", "SubGroupID", "SubGroup2ID", "ItemID", "FullItemId", "ItemName", "RemainingQty", "UoM_Qty", "UoM_Unit", "Alt_Qty", "Alt_Unit", "Ratio" };
        /*********datagridview cols name*********/

        /**********SET MODE*********/
        private string Mode;
        private string DOID;
        /**********SET MODE*********/

        /*********PARENT*********/
        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F) { Parent = F; }
        List<NTForm> ListNTForm = new List<NTForm>();


        /*********PARENT*********/

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        bool check; //label
        /*********VALIDATION*********/
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

        public DOHeader()
        {
            InitializeComponent();
        }

        private void DOHeader_Load(object sender, EventArgs e)
        {
            //tabPage1.Text = "Item Overview";
            //tabPage2.Text = "Item Detail";
            //tabPage3.Text = "Transfer Note";
            if (Mode == "New")
            {
                label4.Visible = false;
                cmbDelivMethod.Visible = false;
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                GetDataHeader();
                ModeBeforeEdit();
            }
            else if (Mode == "PopUp")
            {
                GetDataHeader();
                ModePopUp();
            }
            tabControl1.SelectedTab = tabPage1;
        }

        public void SetMode(string tmpMode, string tmpNumber)
        {
            Mode = tmpMode;
            tbxDOID.Text = tmpNumber;
            DOID = tmpNumber;
        }

        private void ModeNew()
        {
            Mode = "New";
            dtDO.Value = DateTime.Now;
            dtDeliv.Value = DateTime.Now;
            dtDeliv.MinDate = dtDO.Value;

            label3.Visible = false; tbxDOStatus.Visible = false;

            btnAddNotaTransfer.Enabled = false;

            btnPrint.Enabled = false; btnEdit.Enabled = false; btnCancel.Enabled = false;

            if (dataGridView1.ColumnCount != 0)
            {
                btnAdd.Enabled = true; btnDelete.Enabled = true;
            }
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            dtDeliv.Enabled = true;
            btnSVendor.Enabled = true;
            tbxDriverName.Enabled = true;
            tbxVType.Enabled = true;
            tbxVNumber.Enabled = true;
            tbxNotes.Enabled = true;
            btnAddNotaTransfer.Enabled = false;
            cmbDelivMethod.Enabled = true;

            btnAdd.Enabled = true; btnDelete.Enabled = true;

            btnPrint.Enabled = false; btnEdit.Enabled = false; btnSave.Enabled = true; btnCancel.Enabled = true;

            if (dataGridView1.ColumnCount != 0)
            {
                btnAdd.Enabled = true; btnDelete.Enabled = true;
            }
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            //dtDeliv.MinDate = dtDO.Value;
            label3.Visible = true; tbxDOStatus.Visible = true;

            btnSSOID.Enabled = false;
            dtDeliv.Enabled = false;
            btnSVendor.Enabled = false; btnWarehouse.Enabled = false;
            tbxDriverName.Enabled = false; tbxVType.Enabled = false; tbxVNumber.Enabled = false;
            tbxNotes.Enabled = false;
            btnAddNotaTransfer.Enabled = true;
            cmbDelivMethod.Enabled = false;

            tbxCustName.Enabled = true;
            tbxCustID.Enabled = true;

            btnAdd.Enabled = false; btnDelete.Enabled = false;

            btnPrint.Enabled = true; btnEdit.Enabled = true; btnSave.Enabled = false; btnCancel.Enabled = false;
            //tia edit
            tbxCustID.Enabled = true;
            tbxCustName.Enabled = true;
            tbxVendID.Enabled = true;
            tbxVendName.Enabled = true;
            tbxSOID.Enabled = true;

            tbxCustName.ReadOnly = true;
            tbxCustID.ReadOnly = true;
            tbxVendID.ReadOnly = true;
            tbxVendName.ReadOnly = true;
            tbxSOID.ReadOnly = true;

            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            tbxVendID.ContextMenu = vendid;
            tbxVendName.ContextMenu = vendid;
            tbxSOID.ContextMenu = vendid;
            //tia end
            Conn = ConnectionString.GetConnection();
            Query = "select DeliveryOrderStatus from DeliveryOrderH where DeliveryOrderId = '" + tbxDOID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string DOStats = Cmd.ExecuteScalar().ToString();
            string[] stats = new string[] { "05", "06", "07", "08", "09" };
            flag = '\0';
            for (int i = 0; i < stats.Length; i++)
            {
                if (DOStats == stats[i])
                {
                    flag = 'X';
                    break;
                }
            }
            if (flag == 'X')
            {
                btnAddNotaTransfer.Enabled = false;
                btnPrint.Enabled = false;
                btnEdit.Enabled = false;
            }
            Conn.Close();
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
            //dtDeliv.MinDate = dtDO.Value;
            label3.Visible = true; tbxDOStatus.Visible = true;

            btnSSOID.Enabled = false;
            dtDeliv.Enabled = false;
            btnSVendor.Enabled = false; btnWarehouse.Enabled = false;
            tbxDriverName.Enabled = false; tbxVType.Enabled = false; tbxVNumber.Enabled = false;
            tbxNotes.Enabled = false;
            btnAddNotaTransfer.Enabled = true;

            tbxCustName.Enabled = true;
            tbxCustID.Enabled = true;

            btnAdd.Enabled = false; btnDelete.Enabled = false;

            btnPrint.Enabled = true; btnEdit.Enabled = true; btnSave.Enabled = false; btnCancel.Enabled = false;
            //tia edit
            tbxCustID.Enabled = true;
            tbxCustName.Enabled = true;
            tbxVendID.Enabled = true;
            tbxVendName.Enabled = true;
            tbxSOID.Enabled = true;

            tbxCustName.ReadOnly = true;
            tbxCustID.ReadOnly = true;
            tbxVendID.ReadOnly = true;
            tbxVendName.ReadOnly = true;
            tbxSOID.ReadOnly = true;

            tbxCustID.ContextMenu = vendid;
            tbxCustName.ContextMenu = vendid;
            tbxVendID.ContextMenu = vendid;
            tbxVendName.ContextMenu = vendid;
            tbxSOID.ContextMenu = vendid;

            btnCancel.Visible = false;
            btnSave.Visible = false;
            //tia end
            Conn = ConnectionString.GetConnection();
            Query = "select DeliveryOrderStatus from DeliveryOrderH where DeliveryOrderId = '" + tbxDOID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string DOStats = Cmd.ExecuteScalar().ToString();
            string[] stats = new string[] { "05", "06", "07", "08", "09" };
            flag = '\0';
            for (int i = 0; i < stats.Length; i++)
            {
                if (DOStats == stats[i])
                {
                    flag = 'X';
                    break;
                }
            }
            if (flag == 'X')
            {
                btnAddNotaTransfer.Enabled = false;
                btnPrint.Visible = false;
                btnEdit.Visible = false;
            }
            Conn.Close();
        }
        //tia edit end

        private void btnSSOID_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            Query = "(select * from SalesOrderH where TransStatus in ('03', '05', '06', '08') and DPType = 'N' union all select SOH.* from SalesOrderH SOH left join CustDown_Payment CDP on SOH.SalesOrderNo = CDP.SO_Id where SOH.TransStatus in ('03', '05', '06', '08') and SOH.DPType = 'Y' and CDP.DP_Amount = CDP.DP_Deduct) a";
            f.SetSchemaTable("dbo", "SalesOrderH", "", "a.*", Query);
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                checkDP(SearchV2.data[0]);
            }
        }

        private void checkDP(string soid)
        {
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("SELECT [DPType] FROM [SalesOrderH] WHERE [SalesOrderNo] = '" + soid + "'", Conn);
            string DPRequired = Cmd.ExecuteScalar().ToString();

            if (DPRequired == "Y")
            {
                decimal InvoiceAmount = 0;
                decimal SettleAmount = 0;
                bool booleanDP = false;

                Query = "SELECT [Invoice_Amount], [Settle_Amount] FROM [CustInvoice_H] a ";
                Query += "LEFT JOIN [CustInvoice_Dtl_DP] b ON a.[Invoice_Id] = b.[Invoice_Id] ";
                Query += "WHERE [SO_No] = '" + soid + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    InvoiceAmount = Convert.ToDecimal(Dr["Invoice_Amount"].ToString());
                    SettleAmount = Convert.ToDecimal(Dr["Settle_Amount"].ToString());

                    if ((InvoiceAmount - SettleAmount) == 0)
                    {
                        booleanDP = true;
                    }
                    else
                    {
                        Cmd = new SqlCommand("SELECT [DP_Requried_UnCheck] FROM [SalesOrderH] WHERE [SalesOrderNo]= '" + soid + "'", Conn);
                        string flagDP = Cmd.ExecuteScalar().ToString();

                        if (flagDP == "1")
                        {
                            booleanDP = true;
                        }
                        else
                        {
                            booleanDP = false;
                            MessageBox.Show("Sales Order : " + soid + " DP belum dibayar!" + Environment.NewLine + "Untuk menggunakan, minta agreement dahulu");
                            return;
                        }
                    }
                }

                if (booleanDP == true)
                {
                    tbxSOID.Text = soid;
                    GetDataHeader();
                }
            }
            else if (DPRequired == "N")
            {
                tbxSOID.Text = soid;
                GetDataHeader();
            }
            Conn.Close();
        }

        private void GetDataHeader()
        {
            //TAB 1
            dataGridView1.Rows.Clear(); dataGridView1.Columns.Clear();
            dataGridView1.ColumnCount = Convert.ToInt32(tableCols.Length);
            for (int i = 0; i < tableCols.Length; i++)
                dataGridView1.Columns[i].Name = tableCols[i];
            //TAB 2
            dataGridView2.Rows.Clear(); dataGridView2.Columns.Clear();
            dataGridView2.ColumnCount = tableCols2.Length;
            for (int i = 0; i < tableCols2.Length; i++)
                dataGridView2.Columns[i].Name = tableCols2[i];
            //TAB 4
            dataGridView4.Rows.Clear(); dataGridView4.Columns.Clear();
            dataGridView4.ColumnCount = tableCols4.Length;
            for (int i = 0; i < tableCols4.Length; i++)
                dataGridView4.Columns[i].Name = tableCols4[i];

            Conn = ConnectionString.GetConnection();
            if (Mode == "New")
            {
                Query = "Select OrderDate, CustID, CustName from SalesOrderH where SalesOrderNo = '" + tbxSOID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtSO.Value = (DateTime)Dr["OrderDate"];
                    tbxCustID.Text = Dr["CustID"].ToString();
                    tbxCustName.Text = Dr["CustName"].ToString();
                }
                Dr.Close();

                //BY: HC (S) | MASUKIN VALUE DELIVERY METHOD KE COMBOBOX
                if (tbxSOID.Text != "")
                {
                    label4.Visible = true;
                    cmbDelivMethod.Visible = true;
                    cmbDelivMethod.Items.Clear();
                    Query = "select distinct(DeliveryMethod) from SalesOrderD where SalesOrderNo = '" + tbxSOID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        cmbDelivMethod.Items.Add(Dr["DeliveryMethod"]);
                    }
                    Dr.Close();
                    cmbDelivMethod.SelectedIndex = 0;
                }
                //BY: HC (E)

                Query = "select a.SeqNo, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.FullItemID, a.ItemName, a.Qty, a.RemainingQty, b.Available_For_Sale_UoM, CASE WHEN c.Lock_Qty IS NULL then 0 else c.Lock_Qty END AS Lock_Qty, b.Available_For_Sale_Reserved_UoM, b.Available_UoM, a.Unit, a.Qty_Alt, a.Unit_Alt, a.ConvertionRatio, a.SalesOrderNo from SalesOrderD a left join Invent_OnHand_Qty b on a.FullItemID=b.FullItemId left join InventLockTable c on a.SalesOrderNo = c.RefTransId and a.SeqNo = c.RefTrans_SeqNo and c.SiteId = b.InventSiteId where a.SalesOrderNo = '" + tbxSOID.Text + "' and c.SiteId = '" + tbxInventSiteID.Text + "' and a.RemainingQty != 0 and c.RefTrans2Id = '' and RefTrans2_SeqNo = 0 and a.DeliveryMethod = '" + cmbDelivMethod.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    //TAB ITEM OVERVIEW
                    dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", Dr["SeqNo"], Dr["SalesOrderNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["ConvertionRatio"]);

                    //TAB ITEM DETAIL 
                    dataGridView2.Rows.Add(dataGridView2.RowCount + 1, "", Dr["SeqNo"], Dr["SalesOrderNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Available_For_Sale_UoM"], Dr["Available_UoM"], Dr["Lock_Qty"], Dr["Available_For_Sale_Reserved_UoM"], 0, Dr["Unit"], 0, Dr["Unit_Alt"], Dr["ConvertionRatio"]);
                }
                Dr.Close();
                ModeNew();
            }
            else if (Mode != "New")
            {
                //BY: HC (S) | MASUKIN VALUE DELIVERY METHOD KE COMBOBOX
                if (tbxSOID.Text != "")
                {
                    label4.Visible = true;
                    cmbDelivMethod.Visible = true;
                    cmbDelivMethod.Items.Clear();
                    Query = "select distinct(DeliveryMethod) from SalesOrderD where SalesOrderNo = '" + tbxSOID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        cmbDelivMethod.Items.Add(Dr["DeliveryMethod"]);
                    }
                    Dr.Close();
                    cmbDelivMethod.SelectedIndex = 0;
                }
                //BY: HC (E)

                Query = "select a.*, b.Deskripsi from DeliveryOrderH a left join TransStatusTable b on a.DeliveryOrderStatus = b.StatusCode where b.TransCode = 'do' and a.DeliveryOrderId = '" + tbxDOID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtDO.Value = (DateTime)Dr["DeliveryOrderDate"];
                    dtSO.Value = (DateTime)Dr["SalesOrderDate"];
                    tbxSOID.Text = (String)Dr["SalesOrderId"];
                    //dtDeliv.MinDate = new DateTime(Convert.ToInt32(Dr["DeliveryDate"].ToString().Split('/')[2].Substring(0, 4)), Convert.ToInt32(Dr["DeliveryDate"].ToString().Split('/')[0]), Convert.ToInt32(Dr["DeliveryDate"].ToString().Split('/')[1]), 0, 0, 0);
                    dtDeliv.MinDate = (DateTime)Dr["DeliveryDate"];
                    dtDeliv.Value = (DateTime)Dr["DeliveryDate"];
                    tbxCustID.Text = (String)Dr["CustID"];
                    tbxCustName.Text = (String)Dr["CustName"];
                    tbxVendID.Text = (String)Dr["VendorEkspedisi"];
                    tbxVendName.Text = (String)Dr["VendorEkspedisiName"];
                    tbxDriverName.Text = (String)Dr["DriverName"];
                    tbxVType.Text = (String)Dr["VehicleType"];
                    tbxVNumber.Text = (String)Dr["VehicleNumber"];
                    tbxInventSiteID.Text = (String)Dr["InventSiteID"];
                    tbxWarehouse.Text = (String)Dr["InventSiteName"];
                    tbxLocation.Text = (String)Dr["InventSiteLocation"];
                    tbxNotes.Text = Dr["Notes"].ToString();
                    tbxDOStatus.Text = Dr["Deskripsi"].ToString();
                    cmbDelivMethod.Text = Dr["DeliveryMethod"].ToString();
                }
                Dr.Close();

                Query = "select a.SeqNo, a.SalesOrderSeqNo, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.FullItemID, a.ItemName, b.Qty 'SO_Qty', b.RemainingQty 'SO_RemainingQty', c.Available_For_Sale_UoM, 0, d.Lock_Qty, 0, a.Qty, a.RemainingQty, a.Unit, a.Qty_Alt, a.Unit_Alt, a.ConvertionRatio, b.SalesOrderNo, a.NRJ_Id, b.Qty_return from DeliveryOrderH DOH left join DeliveryOrderD a on DOH.DeliveryOrderId = a.DeliveryOrderId left join SalesOrderD b on a.SalesOrderId = b.SalesOrderNo and a.SalesOrderSeqNo = b.SeqNo left join Invent_OnHand_Qty c on a.FullItemID = c.FullItemId and DOH.InventSiteID = c.InventSiteId left join InventLockTable d on d.RefTransId = b.SalesOrderNo and d.RefTrans_SeqNo = b.SeqNo and d.SiteId = DOH.InventSiteID where a.DeliveryOrderId = '" + tbxDOID.Text + "' and a.Closed = 'N'"; //and a.Qty != '0'
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    //TAB ITEM OVERVIEW
                    dataGridView1.Rows.Add(dataGridView1.RowCount + 1, Dr["SeqNo"], Dr["SalesOrderSeqNo"], Dr["SalesOrderNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["ConvertionRatio"]);

                    if (Dr["NRJ_Id"] == (object)DBNull.Value)
                        //TAB ITEM DETAIL 
                        dataGridView2.Rows.Add(dataGridView2.RowCount + 1, Dr["SeqNo"], Dr["SalesOrderSeqNo"], Dr["SalesOrderNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["SO_Qty"], Dr["SO_RemainingQty"], Dr["Available_For_Sale_UoM"], "", Dr["Lock_Qty"], "", Dr["Qty"], Dr["Unit"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["ConvertionRatio"]);
                    else
                    {
                        dataGridView4.Rows.Add(dataGridView4.RowCount + 1, Dr["NRJ_Id"], Dr["SalesOrderSeqNo"], Dr["SeqNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty_Return"], Dr["Qty"], Dr["Unit"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["ConvertionRatio"]);
                    }
                }
                Dr.Close();
            }

            Query = "select b.TransferNo, b.SeqNo, a.InventSiteFrom, a.InventSiteTo, b.FullItemId, b.ItemName, b.Qty, b.RemainingQty, b.Uom from NotaTransferH a left join NotaTransferD b on a.TransferNo = b.TransferNo where a.ReferenceId = '" + DOID + "' and a.ReferenceType = 'DELIVERY ORDER' and a.TransStatus != 'XX'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            if (Dr.HasRows)
            {
                //TAB NOTA TRANSFER
                dataGridView3.Rows.Clear(); dataGridView3.Columns.Clear();
                dataGridView3.ColumnCount = tableCols3.Length;
                for (int i = 0; i < tableCols3.Length; i++)
                    dataGridView3.Columns[i].Name = tableCols3[i];
                while (Dr.Read())
                {
                    this.dataGridView3.Rows.Add(dataGridView3.RowCount + 1, Dr["TransferNo"], Dr["SeqNo"], Dr["FullItemId"], Dr["ItemName"], Dr["InventSiteFrom"], Dr["Qty"], Dr["RemainingQty"], Dr["Uom"]);
                }
            }
            Dr.Close();
            Conn.Close();
            gvFormat();
        }

        private void btnWarehouse_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "InventSite", "", "a.*", "InventSite a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                tbxInventSiteID.Text = SearchV2.data[0];
                Conn = ConnectionString.GetConnection();
                Query = "select InventSiteID, InventSiteName, Lokasi, SiteType from InventSite where InventSiteID = '" + tbxInventSiteID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    tbxInventSiteID.Text = Dr["InventSiteID"].ToString();
                    tbxWarehouse.Text = Dr["InventSiteName"].ToString();
                    tbxLocation.Text = Dr["Lokasi"].ToString();
                    tbxInventSiteType.Text = Dr["SiteType"].ToString();
                }
                Dr.Close();
                Conn.Close();
                GetDataHeader();
            }
        }

        private void btnSVendor_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "VendTable", "and Gol_Prsh = 'EXPEDISI'", "a.*", "VendTable a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                tbxVendID.Text = SearchV2.data[0];
                Conn = ConnectionString.GetConnection();
                Query = "select VendName from VendTable where VendID = '" + tbxVendID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                tbxVendName.Text = Cmd.ExecuteScalar().ToString();
                Conn.Close();
            }
        }

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

        private char Validation()
        {
            flag = '\0';
            string msg = "";
            if (validate == false)
            {
                label = new Label[20];
            }

            //MAIN
            createLabel(tbxSOID, lSOID, gbMain, "string");
            createLabel(tbxVendID, lVendor, gbMain, "string");
            createLabel(tbxVendName, lVendor, gbMain, "string");
            createLabel(tbxVType, lVType, gbMain, "string");
            createLabel(tbxVNumber, lVNumber, gbMain, "string");
            createLabel(tbxDriverName, lDriverName, gbMain, "string");
            createLabel(tbxInventSiteID, lWarehouse, gbMain, "string");
            createLabel(tbxWarehouse, lWarehouse, gbMain, "string");
            createLabel(tbxLocation, lWarehouse, gbMain, "string");

            if (flag == 'X')
                msg += "* Field is required!\r\n";

            int tempSeqNo; string tempFullItemID; decimal tempQty;
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                tempSeqNo = Convert.ToInt32(dataGridView2.Rows[i].Cells["SO_SeqNo"].Value);
                tempFullItemID = dataGridView2.Rows[i].Cells["FullItemId"].Value.ToString();
                tempQty = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value);
                //if (dataGridView2.Rows[i].Cells["DO Qty"].Value.ToString() != "0.00")
                //    tempQty = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value);
                //else
                //    tempQty = 0;
                for (int j = 0; j < dataGridView2.RowCount; j++)
                {
                    if (i != j) //DIFFERENT ROW
                    {
                        if (tempSeqNo == Convert.ToInt32(dataGridView2.Rows[j].Cells["SO_SeqNo"].Value) && tempFullItemID == dataGridView2.Rows[j].Cells["FullItemId"].Value.ToString()) //DIFFERENT SEQNO && FULLITEMID
                        {
                            tempQty += Convert.ToDecimal(dataGridView2.Rows[j].Cells["DO Qty"].Value);
                        }
                    }
                }
                if (Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value) == 0)
                {
                    msg += tempFullItemID + " quantity cannot be 0!\r\n";
                }
                if (tbxDOID.Text == String.Empty)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "Select RemainingQty from SalesOrderD where SalesOrderNo = '" + tbxSOID.Text + "' and SeqNo = '" + tempSeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (tempQty > (Decimal)Cmd.ExecuteScalar())
                    {
                        msg += tempFullItemID + " quantity cannot more than " + Cmd.ExecuteScalar().ToString() + "!\r\n";
                    }
                    Conn.Close();
                }
                else
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "Select Qty from SalesOrderD where SalesOrderNo = '" + tbxSOID.Text + "' and SeqNo = '" + tempSeqNo + "' and FullItemID = '" + tempFullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (tempQty > (Decimal)Cmd.ExecuteScalar())
                    {
                        msg += tempFullItemID + " quantity cannot more than " + Cmd.ExecuteScalar().ToString() + "!\r\n";
                    }
                    Conn.Close();
                }
            }

            if (tbxDOID.Text != String.Empty)
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select RemainingQty, Qty from DeliveryOrderD where DeliveryOrderId = '" + tbxDOID.Text + "' and SeqNo = '" + dataGridView2.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        decimal rQty = 0, qty = 0;
                        while (Dr.Read())
                        {
                            rQty = Convert.ToDecimal(Dr["RemainingQty"]);
                            qty = Convert.ToDecimal(Dr["Qty"]);
                        }
                        Dr.Close();
                        if (Convert.ToDecimal(qty - rQty) > Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value))
                            msg += "Row " + Convert.ToInt32(i + 1) + " qty cannot smaller than " + Convert.ToDecimal(qty - rQty) + "!\r\n";
                    }
                    Conn.Close();
                }
            }

            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                if (Convert.ToDecimal(dataGridView4.Rows[i].Cells["UoM_Qty"].Value) > Convert.ToDecimal(dataGridView4.Rows[i].Cells["RemainingQty"].Value))
                {
                    msg += "Row " + Convert.ToInt32(i + 1) + " Retur qty cannot more than " + dataGridView4.Rows[i].Cells["RemainingQty"].Value.ToString() + "!\r\n";
                }
            }

            if (msg != String.Empty)
            {
                MetroFramework.MetroMessageBox.Show(this, msg, "Fill the form!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                flag = 'X';
            }
            count = 0;
            validate = true;
            return flag;
        }

        //private void insertStatusLogSave(string statnum)
        //{
        //    Query = "INSERT INTO [dbo].[StatusLog_Customer] ([StatusLog_FormName],[Customer_Id],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
        //    Query += " VALUES ('DO Form','" + tbxCustID.Text + "', '" + tbxDOID.Text + "','" + tbxCustID.Text + "', '', '','" + statnum + "','','" + ControlMgr.UserId + "' , GetDate())";
        //    SqlCommand Cmd2 = new SqlCommand(Query, Conn);
        //    Cmd2.ExecuteNonQuery();
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation() != 'X')
            {
                //HEADER
                #region variable
                string DOID;
                DateTime dtDO = this.dtDO.Value;
                DateTime dtSO = this.dtSO.Value;
                DateTime dtDeliv = this.dtDeliv.Value;
                string SOID = tbxSOID.Text;
                string CustID = tbxCustID.Text;
                string CustName = tbxCustName.Text;
                string VendID = tbxVendID.Text;
                string VendName = tbxVendName.Text;
                string DriverName = tbxDriverName.Text.Trim();
                string VType = tbxVType.Text.Trim();
                string VNumber = tbxVNumber.Text.Trim();
                string WarehouseID = tbxInventSiteID.Text;
                string WarehouseName = tbxWarehouse.Text;
                string WarehouseLoc = tbxLocation.Text;
                string WarehouseType = tbxInventSiteType.Text;
                string Notes = tbxNotes.Text.Trim();
                #endregion
                try
                {
                    using (scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        Conn2 = ConnectionString.GetConnection();
                        if (Mode == "New")
                        {
                            //begin==================================================
                            //updated by : joshua 14 Feb 2018
                            //description : change generate sequence number, get from global function and update counter 
                            string Jenis = "DO", Kode = "DO";
                            DOID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);
                            //end update counter
                            //end====================================================

                            InsertDeliveryOrderH(dtDO, dtSO, dtDeliv, DOID, SOID, CustID, CustName, VendID, VendName, DriverName, VType, VNumber, WarehouseID, WarehouseName, WarehouseLoc, WarehouseType, Notes);

                            for (int i = 0; i < dataGridView2.RowCount; i++)
                            {
                                #region variable
                                int DOSeqNo = Convert.ToInt32(i + 1);
                                int SOSeqNo = Convert.ToInt32(dataGridView2.Rows[i].Cells["SO_SeqNo"].Value);
                                string GroupID = dataGridView2.Rows[i].Cells["GroupID"].Value.ToString();
                                string SubGroup1ID = dataGridView2.Rows[i].Cells["SubGroup1ID"].Value.ToString();
                                string SubGroup2ID = dataGridView2.Rows[i].Cells["SubGroup2ID"].Value.ToString();
                                string ItemID = dataGridView2.Rows[i].Cells["ItemID"].Value.ToString();
                                string FullItemID = dataGridView2.Rows[i].Cells["FullItemId"].Value.ToString();
                                string ItemName = dataGridView2.Rows[i].Cells["ItemName"].Value.ToString();
                                decimal Qty = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value);
                                string Unit = dataGridView2.Rows[i].Cells["Unit"].Value.ToString();
                                decimal Qty_Alt = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Qty Alt"].Value);
                                string Unit_Alt = dataGridView2.Rows[i].Cells["Unit Alt"].Value.ToString();
                                decimal ConvertionRatio = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Ratio"].Value);
                                #endregion

                                //SO CONFIRMED OUTS && DO ISSUED OUTS
                                updateInventSalesQty(SOID, SOSeqNo, DOID, DOSeqNo, FullItemID, Qty, ConvertionRatio);

                                updateInventOnHand(DOID, DOSeqNo, SOID, SOSeqNo, FullItemID, Qty, ConvertionRatio, WarehouseID);
                                insertUpdate_InventLockTable(SOID, SOSeqNo, DOID, DOSeqNo, FullItemID, WarehouseID, ConvertionRatio, Qty, Unit, Qty_Alt, Unit_Alt);

                                insertUpdate_InventTrans(SOID, SOSeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, WarehouseID, DOID, DOSeqNo, dtDO, SOID, dtSO, SOSeqNo, CustID, CustName, Qty, ConvertionRatio, Notes);

                                InsertDeliveryOrderD_SORemainingQty(DOID, DOSeqNo, SOID, SOSeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, WarehouseID, "");

                                insertDOIssuedLogTable(dtDO, DOID, CustID, DOSeqNo, WarehouseID, Qty, ConvertionRatio, SOID, dtSO);
                            }

                            for (int i = 0; i < dataGridView4.RowCount; i++)
                            {
                                #region variable
                                string NRJId = dataGridView4.Rows[i].Cells["NRJId"].Value.ToString();
                                int NRJ_SeqNo = Convert.ToInt32(dataGridView4.Rows[i].Cells["NRJ_SeqNo"].Value);
                                int seqNo = Convert.ToInt32(dataGridView4.Rows[i].Cells["SeqNo"].Value);
                                decimal UoM_Qty = Convert.ToDecimal(dataGridView4.Rows[i].Cells["UoM_Qty"].Value);
                                string UoM_Unit = dataGridView4.Rows[i].Cells["UoM_Unit"].Value.ToString();
                                string Alt_Unit = dataGridView4.Rows[i].Cells["Alt_Unit"].Value.ToString();
                                string GroupID = dataGridView4.Rows[i].Cells["GroupID"].Value.ToString();
                                string SubGroupID = dataGridView4.Rows[i].Cells["SubGroupID"].Value.ToString();
                                string SubGroup2ID = dataGridView4.Rows[i].Cells["SubGroup2ID"].Value.ToString();
                                string ItemID = dataGridView4.Rows[i].Cells["ItemID"].Value.ToString();
                                string FullItemID = dataGridView4.Rows[i].Cells["FullItemId"].Value.ToString();
                                string ItemName = dataGridView4.Rows[i].Cells["ItemName"].Value.ToString();
                                decimal Ratio = Convert.ToDecimal(dataGridView4.Rows[i].Cells["Ratio"].Value);
                                int DOSeqNo = Convert.ToInt32(i + 1 + dataGridView2.RowCount);
                                #endregion
                                //GET SO ID FROM SELECTED NRJ
                                string soid = "";
                                int so_seqno = 0;
                                Query = "select DOD.SalesOrderId, DOD.SalesOrderSeqNo from NotaReturJual_Dtl NRJD left join GoodsIssuedD GID on NRJD.GoodsIssuedId = GID.GoodsIssuedId and NRJD.GoodsIssued_SeqNo = GID.GoodsIssuedSeqNo left join DeliveryOrderD DOD on GID.RefTransID = DOD.DeliveryOrderId and GID.RefTransSeqNo = DOD.SeqNo where NRJD.NRJId = '" + NRJId + "' and NRJD.SeqNo = '" + NRJ_SeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    soid = Dr["SalesOrderId"].ToString();
                                    so_seqno = Convert.ToInt32(Dr["SalesOrderSeqNo"]);
                                }
                                Dr.Close();

                                updateInventSalesQty(soid, so_seqno, DOID, DOSeqNo, FullItemID, UoM_Qty, Ratio);
                                updateInventOnHand(DOID, DOSeqNo, soid, so_seqno, FullItemID, UoM_Qty, Ratio, WarehouseID);
                                insertUpdate_InventLockTable(soid, so_seqno, DOID, DOSeqNo, FullItemID, WarehouseID, Ratio, UoM_Qty, UoM_Unit, UoM_Qty * Ratio, Alt_Unit);
                                insertUpdate_InventTrans(soid, so_seqno, GroupID, SubGroupID, SubGroup2ID, ItemID, FullItemID, ItemName, WarehouseID, DOID, DOSeqNo, dtDO, soid, dtSO, so_seqno, CustID, CustName, UoM_Qty, Ratio, Notes);
                                InsertDeliveryOrderD_SORemainingQty(DOID, DOSeqNo, soid, so_seqno, GroupID, SubGroupID, SubGroup2ID, ItemID, FullItemID, ItemName, UoM_Qty, UoM_Unit, Ratio * UoM_Qty, Alt_Unit, Ratio, WarehouseID, NRJId);
                                insertDOIssuedLogTable(dtDO, DOID, CustID, DOSeqNo, WarehouseID, UoM_Qty, Ratio, soid, dtSO);

                                updateNRJRemainingQty(soid, so_seqno, UoM_Qty, NRJId, NRJ_SeqNo, seqNo);
                            }

                            UpdateSalesOrderH();

                            tbxDOID.Text = DOID;
                            //insertStatusLogSave("01");
                            ListMethod.StatusLogCustomer("DOHeader", "DO", tbxCustID.Text, "01", "", tbxDOID.Text, "", "", "");

                            //Begin
                            //Created By : Joshua
                            //Created Date ; 24 Aug 2018
                            //Desc : Create Journal
                            CreateJournal();
                            //End
                        }
                        else if (Mode != "New")
                        {
                            DOID = tbxDOID.Text;
                            UpdateDeliveryOrderH(dtDeliv, DOID, VendID, VendName, DriverName, VType, VNumber, WarehouseID, WarehouseName, WarehouseLoc, Notes);

                            checkItemStillExist(SOID, DOID, WarehouseID); //CHECK IF ITEM IS DELETED

                            //DELETE INVENT TRANS RELATED TO THIS TRANSID (DOID)
                            Query = "delete InventTrans where TransId = '" + DOID + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            for (int i = 0; i < dataGridView2.RowCount; i++)
                            {
                                #region variable
                                int DOSeqNo = 0;
                                if (dataGridView2.Rows[i].Cells["SeqNo"].Value.ToString() == String.Empty)
                                {
                                    Query = "Select top 1 SeqNo from DeliveryOrderD where DeliveryOrderId = '" + DOID + "' order by SeqNo desc";
                                    Cmd = new SqlCommand(Query, Conn);
                                    DOSeqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;
                                }
                                else
                                    DOSeqNo = Convert.ToInt32(dataGridView2.Rows[i].Cells["SeqNo"].Value);
                                int SOSeqNo = Convert.ToInt32(dataGridView2.Rows[i].Cells["SO_SeqNo"].Value);
                                string GroupID = dataGridView2.Rows[i].Cells["GroupID"].Value.ToString();
                                string SubGroup1ID = dataGridView2.Rows[i].Cells["SubGroup1ID"].Value.ToString();
                                string SubGroup2ID = dataGridView2.Rows[i].Cells["SubGroup2ID"].Value.ToString();
                                string ItemID = dataGridView2.Rows[i].Cells["ItemID"].Value.ToString();
                                string FullItemID = dataGridView2.Rows[i].Cells["FullItemId"].Value.ToString();
                                string ItemName = dataGridView2.Rows[i].Cells["ItemName"].Value.ToString();
                                decimal Qty = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value);
                                string Unit = dataGridView2.Rows[i].Cells["Unit"].Value.ToString();
                                decimal Qty_Alt = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Qty Alt"].Value);
                                string Unit_Alt = dataGridView2.Rows[i].Cells["Unit Alt"].Value.ToString();
                                decimal ConvertionRatio = Convert.ToDecimal(dataGridView2.Rows[i].Cells["Ratio"].Value);
                                #endregion

                                //SO CONFIRMED OUTS && DO ISSUED OUTS
                                updateInventSalesQty(SOID, SOSeqNo, DOID, DOSeqNo, FullItemID, Qty, ConvertionRatio);
                                updateInventOnHand(DOID, DOSeqNo, SOID, SOSeqNo, FullItemID, Qty, ConvertionRatio, WarehouseID);
                                insertUpdate_InventLockTable(SOID, SOSeqNo, DOID, DOSeqNo, FullItemID, WarehouseID, ConvertionRatio, Qty, Unit, Qty_Alt, Unit_Alt);

                                insertUpdate_InventTrans(SOID, SOSeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, WarehouseID, DOID, DOSeqNo, dtDO, SOID, dtSO, SOSeqNo, CustID, CustName, Qty, ConvertionRatio, Notes);

                                Query = "select 0,SeqNo,GroupID,SubGroup1ID,SubGroup2ID,ItemID,FullItemId,ItemName,Qty,Unit,Qty_Alt,Unit_Alt,ConvertionRatio,RemainingQty from [dbo].[DeliveryOrderD] where DeliveryOrderId = '" + DOID + "' and SeqNo = '" + DOSeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn2);
                                Dr = Cmd.ExecuteReader();
                                if (Dr.HasRows)
                                {
                                    UpdateDeliveryOrderD_SORemainingQty(DOID, SOID, SOSeqNo, FullItemID, Qty, Qty_Alt, WarehouseID, DOSeqNo);
                                }
                                else
                                {
                                    InsertDeliveryOrderD_SORemainingQty(DOID, DOSeqNo, SOID, SOSeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, Qty, Unit, Qty_Alt, Unit_Alt, ConvertionRatio, WarehouseID, "");
                                }
                                insertDOIssuedLogTable(dtDO, DOID, CustID, DOSeqNo, WarehouseID, Qty, ConvertionRatio, SOID, dtSO);
                            }

                            for (int i = 0; i < dataGridView4.RowCount; i++)
                            {
                                #region variable
                                string NRJId = dataGridView4.Rows[i].Cells["NRJId"].Value.ToString();
                                int NRJ_SeqNo = Convert.ToInt32(dataGridView4.Rows[i].Cells["NRJ_SeqNo"].Value);
                                int seqNo = 0;
                                if (dataGridView4.Rows[i].Cells["SeqNo"].Value.ToString() == String.Empty)
                                {
                                    Query = "Select top 1 SeqNo from DeliveryOrderD where DeliveryOrderId = '" + DOID + "' order by SeqNo desc";
                                    Cmd = new SqlCommand(Query, Conn);
                                    seqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;
                                }
                                else
                                    seqNo = Convert.ToInt32(dataGridView2.Rows[i].Cells["SeqNo"].Value);

                                decimal UoM_Qty = Convert.ToDecimal(dataGridView4.Rows[i].Cells["UoM_Qty"].Value);
                                string UoM_Unit = dataGridView4.Rows[i].Cells["UoM_Unit"].Value.ToString();
                                string Alt_Unit = dataGridView4.Rows[i].Cells["Alt_Unit"].Value.ToString();
                                string GroupID = dataGridView4.Rows[i].Cells["GroupID"].Value.ToString();
                                string SubGroupID = dataGridView4.Rows[i].Cells["SubGroupID"].Value.ToString();
                                string SubGroup2ID = dataGridView4.Rows[i].Cells["SubGroup2ID"].Value.ToString();
                                string ItemID = dataGridView4.Rows[i].Cells["ItemID"].Value.ToString();
                                string FullItemID = dataGridView4.Rows[i].Cells["FullItemId"].Value.ToString();
                                string ItemName = dataGridView4.Rows[i].Cells["ItemName"].Value.ToString();
                                decimal Ratio = Convert.ToDecimal(dataGridView4.Rows[i].Cells["Ratio"].Value);
                                int DOSeqNo = Convert.ToInt32(i + 1 + dataGridView2.RowCount);
                                #endregion
                                //GET SO ID FROM SELECTED NRJ
                                string soid = "";
                                int so_seqno = 0;
                                Query = "select DOD.SalesOrderId, DOD.SalesOrderSeqNo from NotaReturJual_Dtl NRJD left join GoodsIssuedD GID on NRJD.GoodsIssuedId = GID.GoodsIssuedId and NRJD.GoodsIssued_SeqNo = GID.GoodsIssuedSeqNo left join DeliveryOrderD DOD on GID.RefTransID = DOD.DeliveryOrderId and GID.RefTransSeqNo = DOD.SeqNo where NRJD.NRJId = '" + NRJId + "' and NRJD.SeqNo = '" + NRJ_SeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    soid = Dr["SalesOrderId"].ToString();
                                    so_seqno = Convert.ToInt32(Dr["SalesOrderSeqNo"]);
                                }
                                Dr.Close();

                                updateInventSalesQty(soid, so_seqno, DOID, DOSeqNo, FullItemID, UoM_Qty, Ratio);
                                updateInventOnHand(DOID, DOSeqNo, soid, so_seqno, FullItemID, UoM_Qty, Ratio, WarehouseID);
                                insertUpdate_InventLockTable(soid, so_seqno, DOID, DOSeqNo, FullItemID, WarehouseID, Ratio, UoM_Qty, UoM_Unit, UoM_Qty * Ratio, Alt_Unit);
                                insertUpdate_InventTrans(soid, so_seqno, GroupID, SubGroupID, SubGroup2ID, ItemID, FullItemID, ItemName, WarehouseID, DOID, DOSeqNo, dtDO, soid, dtSO, so_seqno, CustID, CustName, UoM_Qty, Ratio, Notes);

                                updateNRJRemainingQty(soid, so_seqno, UoM_Qty, NRJId, NRJ_SeqNo, DOSeqNo);

                                Query = "select 0,SeqNo,GroupID,SubGroup1ID,SubGroup2ID,ItemID,FullItemId,ItemName,Qty,Unit,Qty_Alt,Unit_Alt,ConvertionRatio,RemainingQty from [dbo].[DeliveryOrderD] where DeliveryOrderId = '" + DOID + "' and SeqNo = '" + seqNo + "'";
                                Cmd = new SqlCommand(Query, Conn2);
                                Dr = Cmd.ExecuteReader();
                                if (Dr.HasRows)
                                {
                                    UpdateDeliveryOrderD_SORemainingQty(DOID, soid, so_seqno, FullItemID, UoM_Qty, Ratio * UoM_Qty, WarehouseID, DOSeqNo);
                                }
                                else
                                {
                                    InsertDeliveryOrderD_SORemainingQty(DOID, seqNo, soid, so_seqno, GroupID, SubGroupID, SubGroup2ID, ItemID, FullItemID, ItemName, UoM_Qty, UoM_Unit, Ratio * UoM_Qty, Alt_Unit, Ratio, WarehouseID, NRJId);
                                }
                                insertDOIssuedLogTable(dtDO, DOID, CustID, DOSeqNo, WarehouseID, UoM_Qty, Ratio, soid, dtSO);
                            }

                            UpdateJournal();
                            UpdateSalesOrderH();
                            //insertStatusLogSave(); status ny?
                        }
                        Conn2.Close();
                        Conn.Close();
                        scope.Complete();


                    }
                    Parent.RefreshGrid();
                    if (Mode == "New")
                        MetroFramework.MetroMessageBox.Show(this, tbxDOID.Text + " save success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MetroFramework.MetroMessageBox.Show(this, tbxDOID.Text + " update success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //Outer: ;
                    ModeBeforeEdit();
                    GetDataHeader();
                }
                //catch (Exception ex)
                //{
                //    MetroFramework.MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //}
                finally
                {
                    Parent.RefreshGrid();
                }
            }
        }

        private void CreateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date ; 24 Aug 2018
            //Desc : Create Journal

            decimal Available = 0;
            decimal Price = 0;

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                string FullItemID = dataGridView2.Rows[i].Cells["FullItemId"].Value.ToString();
                decimal Qty = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value);
                string Unit = dataGridView2.Rows[i].Cells["Unit"].Value.ToString();

                if (Unit.ToUpper() == "KG")
                {
                    Price = GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                    Available = Available + (Qty * Price);
                }
                else
                {
                    Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                    Available = Available + (Qty * Price);
                }
            }

            if (Available != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "IN51";
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Cmd);
                string Notes = tbxSOID.Text;

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxDOID.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                    Type = Convert.ToString(Dr["Type"]);
                    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                    AmountValue = 0;

                    if (JournalHID == "IN51")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            AmountValue = Available;
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            AmountValue = Available;
                        }
                    }

                    if (AmountValue == 0)
                    {
                        continue;
                    }

                    //Insert Detail GLJournal
                    Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                    Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                    Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                Dr.Close();

            }
            //End
        }

        private void UpdateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date ; 24 Aug 2018
            //Desc : Update Journal

            decimal OldAvailable, Available = 0;
            decimal Price = 0;

            //Get GLJournalHID
            Query = "SELECT TOP 1 GLJournalHID FROM GLJournalH WHERE Referensi = '" + tbxDOID.Text + "' ORDER BY CreatedDate DESC";
            Cmd = new SqlCommand(Query, Conn);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            Query = "SELECT TOP 1 Amount FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "' AND SeqNo = 1";
            Cmd = new SqlCommand(Query, Conn);
            OldAvailable = Convert.ToDecimal(Cmd.ExecuteScalar());

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                string FullItemID = dataGridView2.Rows[i].Cells["FullItemId"].Value.ToString();
                decimal Qty = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value);
                string Unit = dataGridView2.Rows[i].Cells["Unit"].Value.ToString();

                if (Unit.ToUpper() == "KG")
                {
                    Price = GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                    Available = Available + (Qty * Price);
                }
                else
                {
                    Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                    Available = Available + (Qty * Price);
                }
            }

            if (Available != 0 && OldAvailable != Available)
            {
                //Journal Reverse
                //Insert Header GLJournal
                Query = "SELECT TOP 1 Notes FROM GLJournalH WHERE GLJournalHID = '" + GLJournalHID + "'";
                Cmd = new SqlCommand(Query, Conn);
                string Notes = Convert.ToString(Cmd.ExecuteScalar());

                string JournalHID = "IN51R";
                string Jenis = "JN", Kode = "JN";
                GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Cmd);

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxDOID.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                    Type = Convert.ToString(Dr["Type"]);
                    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                    AmountValue = 0;

                    if (JournalHID == "IN51R")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            AmountValue = OldAvailable;
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            AmountValue = OldAvailable;
                        }
                    }

                    if (AmountValue == 0)
                    {
                        continue;
                    }

                    //Insert Detail GLJournal
                    Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                    Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                    Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                Dr.Close();

                //New Journal
                //Insert Header GLJournal
                JournalHID = "IN51";
                GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Cmd);
                Notes = tbxSOID.Text;

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxDOID.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                //Select Config Journal
                SeqNo = 1;
                JournalIDSeqNo = 0;
                Type = "";
                FQA_ID = "";
                FQA_Desc = "";
                AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                    Type = Convert.ToString(Dr["Type"]);
                    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                    AmountValue = 0;

                    if (JournalHID == "IN51")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            AmountValue = Available;
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            AmountValue = Available;
                        }
                    }

                    if (AmountValue == 0)
                    {
                        continue;
                    }

                    //Insert Detail GLJournal
                    Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                    Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                    Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                Dr.Close();

            }

            //decimal Available = 0;
            //decimal Price = 0;

            //for (int i = 0; i < dataGridView2.RowCount; i++)
            //{
            //    string FullItemID = dataGridView2.Rows[i].Cells["FullItemId"].Value.ToString();
            //    decimal Qty = Convert.ToDecimal(dataGridView2.Rows[i].Cells["DO Qty"].Value);
            //    string Unit = dataGridView2.Rows[i].Cells["Unit"].Value.ToString();

            //    if (Unit.ToUpper() == "KG")
            //    {
            //        Price = GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
            //        Available = Available + (Qty * Price);
            //    }
            //    else
            //    {
            //        Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
            //        Available = Available + (Qty * Price);
            //    }
            //}

            ////Get GLJournalHID
            //Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + tbxDOID.Text + "' ";
            //Cmd = new SqlCommand(Query, Conn);
            //string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            //Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
            //Cmd = new SqlCommand(Query, Conn);
            //int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

            //if (CountData == 1)
            //{
            //    //Delete Journal Detail
            //    Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Cmd.ExecuteNonQuery();
            //}
            //else
            //{
            //    MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    Journal = true;
            //    return;
            //}

            //if (Available != 0)
            //{
            //    //Insert Header GLJournal
            //    string JournalHID = "IN51";

            //    //Select Config Journal
            //    int SeqNo = 1;
            //    int JournalIDSeqNo = 0;
            //    string Type = "";
            //    string FQA_ID = "";
            //    string FQA_Desc = "";
            //    decimal AmountValue = 0;

            //    Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();
            //    while (Dr.Read())
            //    {
            //        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
            //        Type = Convert.ToString(Dr["Type"]);
            //        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
            //        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
            //        AmountValue = 0;

            //        if (JournalHID == "IN51")
            //        {
            //            if (JournalIDSeqNo == 1)
            //            {
            //                AmountValue = Available;
            //            }
            //            else if (JournalIDSeqNo == 2)
            //            {
            //                AmountValue = Available;
            //            }
            //        }

            //        if (AmountValue == 0)
            //        {
            //            continue;
            //        }

            //        //Insert Detail GLJournal
            //        Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
            //        Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
            //        Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
            //        Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
            //        Cmd = new SqlCommand(Query, Conn);
            //        Cmd.ExecuteNonQuery();
            //        SeqNo++;
            //    }
            //    Dr.Close();

            //}
            //End
        }


        private decimal GetPriceFromInventTable(string FieldName, string FullItemID)
        {
            Query = "SELECT " + FieldName + " FROM InventTable WHERE FullItemID = '" + FullItemID + "'";

            Cmd = new SqlCommand(Query, Conn);
            string result = Convert.ToString(Cmd.ExecuteScalar());
            decimal Price;
            if (result == "")
            {
                Price = 1;
            }
            else if (Convert.ToDecimal(result) == 0)
            {
                Price = 1;
            }
            else
            {
                Price = Convert.ToDecimal(result);
            }
            return Price;
        }

        private void updateNRJRemainingQty(string soid, int so_seqno, decimal UoM_Qty, string NRJId, int NRJ_SeqNo, int seqNo)
        {
            decimal qty_Return = 0;
            Query = "select Qty_Return from SalesOrderD where SalesOrderNo = '" + soid + "' and SeqNo = '" + so_seqno + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                qty_Return = Convert.ToDecimal(Dr["Qty_Return"]);
            }
            Dr.Close();

            decimal Qty = 0;
            decimal remainingQty = 0;
            if (Mode == "Edit")
            {
                Query = "select Qty, remainingQty from DeliveryOrderD where DeliveryOrderId = '" + tbxDOID.Text + "' and SeqNo = '" + seqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    Qty = Convert.ToDecimal(Dr["Qty"]);
                    remainingQty = Convert.ToDecimal(Dr["remainingQty"]);
                }
                Dr.Close();
            }

            if (UoM_Qty > Qty)
                qty_Return = qty_Return + (UoM_Qty - Qty);
            else if (UoM_Qty < Qty)
                qty_Return = qty_Return - (UoM_Qty - Qty);
            Query = "update SalesOrderD set ";
            //Query += "RemainingQty = '" + Convert.ToDecimal(so_remainingQty - UoM_Qty) + "'";
            //Query += "Qty_Return = '" + Convert.ToDecimal(qty_Return - UoM_Qty) + "'";
            Query += "Qty_Return = '" + qty_Return + "'";
            Query += ", UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "'";
            Query += " where SalesOrderNo = '" + soid + "' and SeqNo = '" + so_seqno + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            //Query = "update NotaReturJual_Dtl set RemainingQty = '" + Convert.ToDecimal(qty_Return - UoM_Qty) + "'";
            Query = "update NotaReturJual_Dtl set RemainingQty = '" + qty_Return + "'";
            Query += ", UpdatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "'";
            Query += " where NRJId = '" + NRJId + "' and seqNo = '" + NRJ_SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void checkItemStillExist(string SOID, string DOID, string WarehouseID)
        {
            string where = "";
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (i >= 1 && dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() != String.Empty)
                    where += ",";
                if (dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() != String.Empty)
                    where += "'" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
            }
            //CHECK IF GV ITEM == DB ITEM
            Query = "select * from DeliveryOrderD where DeliveryOrderId = '" + DOID + "' and SeqNo not in (" + where + ") and Closed = 'N'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                int DOSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                int SOSeqNo = Convert.ToInt32(Dr["SalesOrderSeqNo"]);
                string FullItemID = Dr["FullItemId"].ToString();
                decimal oldDOQty = Convert.ToDecimal(Dr["Qty"]);
                decimal Qty = 0;
                decimal Qty_Alt = 0;
                decimal ConvertionRatio = Convert.ToDecimal(Dr["ConvertionRatio"]);
                string Unit = Dr["Unit"].ToString();
                string Unit_Alt = Dr["Unit_Alt"].ToString();

                updateInventSalesQty(SOID, SOSeqNo, DOID, DOSeqNo, FullItemID, Qty, ConvertionRatio);
                updateInventOnHand(DOID, DOSeqNo, SOID, SOSeqNo, FullItemID, Qty, ConvertionRatio, WarehouseID);
                insertUpdate_InventLockTable(SOID, SOSeqNo, DOID, DOSeqNo, FullItemID, WarehouseID, ConvertionRatio, Qty, Unit, Qty_Alt, Unit_Alt);

                Query = "select * from SalesOrderD where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader Dr2 = Cmd.ExecuteReader();
                while (Dr2.Read())
                {
                    decimal SOQty = Convert.ToDecimal(Dr["RemainingQty"]);
                    SOQty += oldDOQty;

                    Query = "update SalesOrderD set RemainingQty = '" + SOQty + "', UpdatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                }
                Dr2.Close();
            }
            Dr.Close();

            //UPDATE DETAIL STATUS TO CLOSED IF GV ITEM != DB ITEM
            Query = "update DeliveryOrderD set Closed = 'Y', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where DeliveryOrderId = '" + DOID + "' and SeqNo not in (" + where + ")";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void insertDOIssuedLogTable(DateTime DeliveryOrderDate, string DOID, string CustID, int SeqNo, string InventSiteID, decimal Qty, decimal Ratio, string SOID, DateTime SalesOrderDate)
        {
            string DOStats = "";
            string DOStatsDesc = "";
            decimal price = 0;
            Query = "select a.DeliveryOrderStatus, c.Deskripsi, d.Price from DeliveryOrderH a left join DeliveryOrderD b on a.DeliveryOrderId = b.DeliveryOrderId left join TransStatusTable c on a.DeliveryOrderStatus = c.StatusCode left join SalesOrderD d on d.SalesOrderNo = b.SalesOrderId and d.SeqNo = b.SalesOrderSeqNo where c.TransCode = 'DO' and b.DeliveryOrderId = '" + DOID + "' and b.SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                DOStats = Dr["DeliveryOrderStatus"].ToString();
                DOStatsDesc = Dr["Deskripsi"].ToString();
                price = Convert.ToDecimal(Dr["Price"]);
            }
            Dr.Close();

            Query = "INSERT INTO [dbo].[DO_Issued_LogTable] ([DeliveryOrderDate],[DeliveryOrderId],[CustID],[SeqNo],[InventSiteID],[Qty_UoM],[Qty_Alt],[Amount],[SalesOrderId],[SalesOrderDate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES (@DeliveryOrderDate,@DeliveryOrderId,@CustID,@SeqNo,@InventSiteID,@Qty_UoM,@Qty_Alt,@Amount,@SalesOrderId,@SalesOrderDate,@LogStatusCode,@LogStatusDesc,@LogDescription,'" + ControlMgr.UserId + "',getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DeliveryOrderDate", DeliveryOrderDate);
            Cmd.Parameters.AddWithValue("@DeliveryOrderId", DOID);
            Cmd.Parameters.AddWithValue("@CustID", CustID);
            Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
            Cmd.Parameters.AddWithValue("@InventSiteID", InventSiteID);
            Cmd.Parameters.AddWithValue("@Qty_UoM", Qty);
            Cmd.Parameters.AddWithValue("@Qty_Alt", Qty * Ratio);
            Cmd.Parameters.AddWithValue("@Amount", price);
            Cmd.Parameters.AddWithValue("@SalesOrderId", SOID);
            Cmd.Parameters.AddWithValue("@SalesOrderDate", SalesOrderDate);
            Cmd.Parameters.AddWithValue("@LogStatusCode", DOStats);
            Cmd.Parameters.AddWithValue("@LogStatusDesc", DOStatsDesc);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + DOStats + ". " + DOStatsDesc);
            Cmd.ExecuteNonQuery();
        }

        private void insertUpdate_InventTrans(string SOID, int SOSeqNo, string GroupId, string SubGroupId, string SubGroup2Id, string ItemId, string FullItemId, string ItemName, string InventSiteId, string TransId, int SeqNo, DateTime TransDate, string Ref_TransId, DateTime Ref_TransDate, int Ref_Trans_SeqNo, string AccountId, string AccountName, decimal Qty, decimal Ratio, string Notes)
        {
            Query = "select Price from SalesOrderD where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            decimal price = Convert.ToDecimal(Cmd.ExecuteScalar());

            Query = "select Lock_Qty from InventLockTable where RefTransId = '" + SOID + "' and RefTrans_SeqNo = '" + SOSeqNo + "' and SiteId = '" + InventSiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            decimal Lock_Qty = Convert.ToDecimal(Cmd.ExecuteScalar());

            Query = "INSERT INTO [dbo].[InventTrans] ([GroupId],[SubGroupId],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteId],[TransId],[SeqNo],[TransDate],[Ref_TransId],[Ref_TransDate],[Ref_Trans_SeqNo],[AccountId],[AccountName],[Available_UoM],[Available_Alt],[Available_Amount],[Available_For_Sale_UoM],[Available_For_Sale_Alt],[Available_For_Sale_Amount],[Available_For_Sale_Reserved_UoM],[Available_For_Sale_Reserved_Alt],[Available_For_Sale_Reserved_Amount],[Notes]) VALUES (@GroupId ,@SubGroupId ,@SubGroup2Id ,@ItemId ,@FullItemId ,@ItemName ,@InventSiteId ,@TransId ,@SeqNo ,@TransDate ,@Ref_TransId ,@Ref_TransDate ,@Ref_Trans_SeqNo ,@AccountId ,@AccountName ,@Available_UoM ,@Available_Alt ,@Available_Amount ,@Available_For_Sale_UoM ,@Available_For_Sale_Alt ,@Available_For_Sale_Amount ,@Available_For_Sale_Reserved_UoM ,@Available_For_Sale_Reserved_Alt ,@Available_For_Sale_Reserved_Amount ,@Notes)";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@GroupId", GroupId);
            Cmd.Parameters.AddWithValue("@SubGroupId", SubGroupId);
            Cmd.Parameters.AddWithValue("@SubGroup2Id", SubGroup2Id);
            Cmd.Parameters.AddWithValue("@ItemId", ItemId);
            Cmd.Parameters.AddWithValue("@FullItemId", FullItemId);
            Cmd.Parameters.AddWithValue("@ItemName", ItemName);
            Cmd.Parameters.AddWithValue("@InventSiteId", InventSiteId);
            Cmd.Parameters.AddWithValue("@TransId", TransId);
            Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
            Cmd.Parameters.AddWithValue("@TransDate", TransDate);
            Cmd.Parameters.AddWithValue("@Ref_TransId", Ref_TransId);
            Cmd.Parameters.AddWithValue("@Ref_TransDate", Ref_TransDate);
            Cmd.Parameters.AddWithValue("@Ref_Trans_SeqNo", Ref_Trans_SeqNo);
            Cmd.Parameters.AddWithValue("@AccountId", AccountId);
            Cmd.Parameters.AddWithValue("@AccountName", AccountName);
            Cmd.Parameters.AddWithValue("@Available_UoM", Qty);
            Cmd.Parameters.AddWithValue("@Available_Alt", Qty * Ratio);
            Cmd.Parameters.AddWithValue("@Available_Amount", Qty * price);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", Qty <= Lock_Qty ? 0 : Qty - Lock_Qty);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", Qty <= Lock_Qty ? 0 * Ratio : (Qty - Lock_Qty) * Ratio);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", Qty <= Lock_Qty ? 0 * price : (Qty - Lock_Qty) * price);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_UoM", Qty <= Lock_Qty ? Qty : Lock_Qty);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Alt", Qty <= Lock_Qty ? Qty * Ratio : Lock_Qty * Ratio);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Amount", Qty <= Lock_Qty ? Qty * price : Lock_Qty * price);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.ExecuteNonQuery();
        }

        private void insertUpdate_InventLockTable(string SOID, int SOSeqNo, string DOID, int DOSeqNo, string FullItemId, string SiteId, decimal Ratio, decimal Lock_Qty, string Unit, decimal Lock_Qty_Alt, string Unit_Alt)
        {
            Query = "select * from InventLockTable where RefTransId = '" + SOID + "' and RefTrans_SeqNo = '" + SOSeqNo + "' and SiteId = '" + SiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            if (Dr2.HasRows)
            {
                decimal qty = 0;
                while (Dr2.Read())
                {
                    qty = Convert.ToDecimal(Dr2["Lock_Qty"]);
                }

                if (Lock_Qty > qty)
                    Lock_Qty = qty;
                if (Mode == "New")
                {
                    Query = "INSERT INTO [dbo].[InventLockTable] ([RefTransType],[RefTransId],[RefTrans_SeqNo],[RefTrans2Id],[RefTrans2_SeqNo],[FullItemId],[SiteId],[Ratio],[Lock_Qty],[Unit],[Lock_Qty_Alt],[Unit_Alt],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('DELIVERY ORDER', '" + DOID + "', '" + DOSeqNo + "', '" + SOID + "', '" + SOSeqNo + "', '" + FullItemId + "', '" + SiteId + "', '" + Ratio + "', '" + Lock_Qty * -1 + "', '" + Unit + "', '" + Lock_Qty_Alt * -1 + "', '" + Unit_Alt + "', getdate(), '" + ControlMgr.UserId + "', '1753-01-01', NULL)";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Query = "update InventLockTable set Lock_Qty = '" + Lock_Qty * -1 + "', Lock_Qty_Alt = '" + Lock_Qty_Alt * -1 + "' where RefTransId = '" + DOID + "' and RefTrans_SeqNo = '" + DOSeqNo + "' and RefTrans2Id = '" + SOID + "' and RefTrans2_SeqNo = '" + SOSeqNo + "' and SiteId = '" + SiteId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                }
            }
            Dr2.Close();
        }

        private void updateInventOnHand(string DOID, int DOSeqNo, string SOID, int SOSeqNo, string FullItemId, decimal Qty, decimal Ratio, string InventSiteId)
        {
            decimal old_Qty = 0;
            Query = "select qty from DeliveryOrderD where DeliveryOrderId = '" + DOID + "' and SeqNo = '" + DOSeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            if (Dr2.HasRows)
            {
                while (Dr2.Read())
                {
                    old_Qty = Convert.ToDecimal(Dr2["Qty"]);
                }
            }
            Dr2.Close();

            decimal Available_For_Sale_UoM = 0;
            decimal Available_For_Sale_Alt = 0;
            decimal Available_For_Sale_Amount = 0;
            decimal Available_For_Sale_Reserved_UoM = 0;
            decimal Available_For_Sale_Reserved_Alt = 0;
            decimal Available_For_Sale_Reserved_Amount = 0;
            decimal avgPrice = 0;
            Query = "select a.Available_For_Sale_UoM, a.Available_For_Sale_Alt, a.Available_For_Sale_Amount, a.Available_For_Sale_Reserved_UoM, a.Available_For_Sale_Reserved_Alt, a.Available_For_Sale_Reserved_Amount, b.UoM_AvgPrice from Invent_OnHand_Qty a left join InventTable b on a.FullItemId = b.FullItemID where a.FullItemId = '" + FullItemId + "' and a.InventSiteId = '" + InventSiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr2 = Cmd.ExecuteReader();
            while (Dr2.Read())
            {
                Available_For_Sale_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_UoM"]);
                Available_For_Sale_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Alt"]);
                Available_For_Sale_Amount = Convert.ToDecimal(Dr2["Available_For_Sale_Amount"]);
                Available_For_Sale_Reserved_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_UoM"]);
                Available_For_Sale_Reserved_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Alt"]);
                Available_For_Sale_Reserved_Amount = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Amount"]);
                avgPrice = Convert.ToDecimal(Dr2["UoM_AvgPrice"]);
            }
            Dr2.Close();

            if (avgPrice == 0)
            {
                avgPrice = 1;
            }

            Query = "select Lock_Qty from InventLockTable where RefTransId = '" + SOID + "' and RefTrans_SeqNo = '" + SOSeqNo + "' and SiteId = '" + InventSiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            decimal LockQty = Convert.ToDecimal(Cmd.ExecuteScalar());

            if (Mode == "Edit")
            {
                if (old_Qty > LockQty)
                {
                    Available_For_Sale_Reserved_UoM += LockQty;
                    Available_For_Sale_Reserved_Alt += (LockQty * Ratio);
                    Available_For_Sale_Reserved_Amount += (LockQty * avgPrice);

                    Available_For_Sale_UoM += old_Qty - LockQty;
                    Available_For_Sale_Alt += ((old_Qty - LockQty) * Ratio);
                    Available_For_Sale_Amount += ((old_Qty - LockQty) * avgPrice);
                }
                else
                {
                    Available_For_Sale_Reserved_UoM += old_Qty;
                    Available_For_Sale_Reserved_Alt += (old_Qty * Ratio);
                    Available_For_Sale_Reserved_Amount += (old_Qty * avgPrice);
                }
            }

            if (Qty > LockQty)
            {
                Available_For_Sale_Reserved_UoM -= LockQty;
                Available_For_Sale_Reserved_Alt -= (LockQty * Ratio);
                Available_For_Sale_Reserved_Amount -= (LockQty * avgPrice);

                Available_For_Sale_UoM -= Qty - LockQty;
                Available_For_Sale_Alt -= ((Qty - LockQty) * Ratio);
                Available_For_Sale_Amount -= ((Qty - LockQty) * avgPrice);
            }
            else
            {
                Available_For_Sale_Reserved_UoM -= Qty;
                Available_For_Sale_Reserved_Alt -= (Qty * Ratio);
                Available_For_Sale_Reserved_Amount -= (Qty * avgPrice);
            }
            Query = "update Invent_OnHand_Qty set Available_For_Sale_Reserved_UoM = '" + Available_For_Sale_Reserved_UoM + "', Available_For_Sale_Reserved_Alt = '" + Available_For_Sale_Reserved_Alt + "', Available_For_Sale_Reserved_Amount = '" + Available_For_Sale_Reserved_Amount + "', Available_For_Sale_UoM = '" + Available_For_Sale_UoM + "', Available_For_Sale_Alt = '" + Available_For_Sale_Alt + "', Available_For_Sale_Amount = '" + Available_For_Sale_Amount + "' where FullItemId = '" + FullItemId + "' and InventSiteId = '" + InventSiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateInventSalesQty(string SOID, int SOSeqNo, string DOID, int DOSeqNo, string FullItemId, decimal Qty, decimal Ratio)
        {
            decimal old_Qty = 0;
            Query = "select qty from DeliveryOrderD where DeliveryOrderId = '" + DOID + "' and SeqNo = '" + DOSeqNo + "' ";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            if (Dr2.HasRows)
            {
                while (Dr2.Read())
                {
                    old_Qty = Convert.ToDecimal(Dr2["Qty"]);
                }
            }
            Dr2.Close();

            Query = "select Price from SalesOrderD where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            decimal price = Convert.ToDecimal(Cmd.ExecuteScalar());

            decimal DO_Issued_Outstanding_UoM = 0;
            decimal DO_Issued_Outstanding_Alt = 0;
            decimal DO_Issued_Outstanding_Amount = 0;
            decimal SO_Confirmed_Outstanding_UoM = 0;
            decimal SO_Confirmed_Outstanding_Alt = 0;
            decimal SO_Confirmed_Outstanding_Amount = 0;
            decimal avgPrice = 0;

            Query = "select a.SO_Confirmed_Outstanding_UoM, a.SO_Confirmed_Outstanding_Alt, a.SO_Confirmed_Outstanding_Amount, a.DO_Issued_Outstanding_UoM, a.DO_Issued_Outstanding_Alt, a.DO_Issued_Outstanding_Amount, b.UoM_AvgPrice from Invent_Sales_Qty a left join InventTable b on a.FullItemId = b.FullItemID where a.FullItemId = '" + FullItemId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr2 = Cmd.ExecuteReader();
            while (Dr2.Read())
            {
                SO_Confirmed_Outstanding_UoM = Convert.ToDecimal(Dr2["SO_Confirmed_Outstanding_UoM"]);
                SO_Confirmed_Outstanding_Alt = Convert.ToDecimal(Dr2["SO_Confirmed_Outstanding_Alt"]);
                SO_Confirmed_Outstanding_Amount = Convert.ToDecimal(Dr2["SO_Confirmed_Outstanding_Amount"]);
                DO_Issued_Outstanding_UoM = Convert.ToDecimal(Dr2["DO_Issued_Outstanding_UoM"]);
                DO_Issued_Outstanding_Alt = Convert.ToDecimal(Dr2["DO_Issued_Outstanding_Alt"]);
                DO_Issued_Outstanding_Amount = Convert.ToDecimal(Dr2["DO_Issued_Outstanding_Amount"]);
                avgPrice = Convert.ToDecimal(Dr2["UoM_AvgPrice"]);
            }
            Dr2.Close();

            if (avgPrice == 0)
            { avgPrice = 1; }

            if (Mode == "Edit")
            {
                SO_Confirmed_Outstanding_UoM += old_Qty;
                SO_Confirmed_Outstanding_Alt += (old_Qty * Ratio);
                SO_Confirmed_Outstanding_Amount += (old_Qty * price);

                DO_Issued_Outstanding_UoM -= old_Qty;
                DO_Issued_Outstanding_Alt -= (old_Qty * Ratio);
                DO_Issued_Outstanding_Amount -= (old_Qty * avgPrice);
            }

            SO_Confirmed_Outstanding_UoM -= Qty;
            SO_Confirmed_Outstanding_Alt = SO_Confirmed_Outstanding_Alt - (Qty * Ratio);
            SO_Confirmed_Outstanding_Amount = SO_Confirmed_Outstanding_Amount - (Qty * price);

            DO_Issued_Outstanding_UoM += Qty;
            DO_Issued_Outstanding_Alt += (Qty * Ratio);
            DO_Issued_Outstanding_Amount += (Qty * avgPrice);

            Query = "update Invent_Sales_Qty set SO_Confirmed_Outstanding_UoM = '" + SO_Confirmed_Outstanding_UoM + "', SO_Confirmed_Outstanding_Alt = '" + SO_Confirmed_Outstanding_Alt + "', SO_Confirmed_Outstanding_Amount = '" + SO_Confirmed_Outstanding_Amount + "', DO_Issued_Outstanding_UoM = '" + DO_Issued_Outstanding_UoM + "', DO_Issued_Outstanding_Alt = '" + DO_Issued_Outstanding_Alt + "', DO_Issued_Outstanding_Amount = '" + DO_Issued_Outstanding_Amount + "' where FullItemId = '" + FullItemId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void UpdateDeliveryOrderD_SORemainingQty(string DOID, string SOID, int SOSeqNo, string FullItemID, decimal Qty, decimal Qty_Alt, string SiteId, int DOSeqNo)
        {
            decimal oldQty = 0, newQty = 0, tempQty = 0, oldRemainingQty = 0, newRemainingQty = 0;
            newQty = Qty;

            Cmd = new SqlCommand("select Qty from [dbo].[DeliveryOrderD] where DeliveryOrderId = '" + DOID + "' and SeqNo = '" + DOSeqNo + "'", Conn2);
            oldQty = (decimal)Cmd.ExecuteScalar();

            Cmd = new SqlCommand("select RemainingQty from [dbo].[DeliveryOrderD] where DeliveryOrderId = '" + DOID + "' and SeqNo = '" + DOSeqNo + "'", Conn2);
            oldRemainingQty = (decimal)Cmd.ExecuteScalar();

            Cmd = new SqlCommand("select RemainingQty from [dbo].[SalesOrderD] where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "'", Conn2);
            tempQty = (decimal)Cmd.ExecuteScalar();

            if (newQty > oldQty)
            {
                tempQty = tempQty - (newQty - oldQty);
                newRemainingQty = oldRemainingQty + (newQty - oldQty);
            }
            else if (newQty < oldQty)
            {
                tempQty = tempQty + (oldQty - newQty);
                newRemainingQty = oldRemainingQty - (oldQty - newQty);
            }
            else
            {
                newRemainingQty = oldRemainingQty;
            }

            Query = "select FullItemId, SiteId, sum(Lock_Qty) from InventLockTable where ((RefTransId = '" + SOID + "' and RefTrans_SeqNo = '" + SOSeqNo + "') or (RefTrans2Id = '" + SOID + "' and RefTrans_SeqNo = '" + SOSeqNo + "')) and SiteId = '" + SiteId + "' group by FullItemId, SiteId";
            Cmd = new SqlCommand(Query, Conn);
            decimal Lock_Qty = Convert.ToDecimal(Cmd.ExecuteNonQuery());

            Query = "Update [dbo].[SalesOrderD] set RemainingQty = '" + tempQty + "' where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "' ";

            Query += "update [dbo].[DeliveryOrderD] set ";
            if (Qty > Lock_Qty)
                Query += "Qty_Available = '" + Convert.ToDecimal(Qty - Lock_Qty) + "', Qty_Reserved = '" + Lock_Qty + "', ";
            else
                Query += "Qty_Available = '0', Qty_Reserved = '" + Lock_Qty + "', ";
            Query += "Qty = '" + Qty + "', Qty_Alt = '" + Qty_Alt + "', Notes = @NotesD, UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "', RemainingQty = '" + newRemainingQty + "' where DeliveryOrderId = '" + DOID + "' and SalesOrderId = '" + SOID + "' and SalesOrderSeqNo = '" + SOSeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NotesD", "");
            Cmd.ExecuteNonQuery();
        }

        private void UpdateDeliveryOrderH(DateTime dtDeliv, string DOID, string VendID, string VendName, string DriverName, string VType, string VNumber, string WarehouseID, string WarehouseName, string WarehouseLoc, string Notes)
        {
            Query = "UPDATE [dbo].[DeliveryOrderH] SET [DeliveryDate] = '" + dtDeliv + "',[VendorEkspedisi] = '" + VendID + "',[VendorEkspedisiName] = '" + VendName + "',[DriverName] = @DriverName,[VehicleType] = @VType,[VehicleNumber] = @VNumber,[InventSiteID] = '" + WarehouseID + "',[InventSiteName] = '" + WarehouseName + "',[InventSiteLocation] = '" + WarehouseLoc + "',[Notes] = @Notes,[UpdatedDate] = getdate() ,[UpdatedBy] = '" + ControlMgr.UserId + "', DeliveryMethod = '" + cmbDelivMethod.Text + "' WHERE DeliveryOrderId = '" + DOID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DriverName", DriverName);
            Cmd.Parameters.AddWithValue("@VType", VType);
            Cmd.Parameters.AddWithValue("@VNumber", VNumber);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.ExecuteNonQuery();
        }

        private void InsertDeliveryOrderH(DateTime dtDO, DateTime dtSO, DateTime dtDeliv, string DOID, string SOID, string CustID, string CustName, string VendID, string VendName, string DriverName, string VType, string VNumber, string WarehouseID, string WarehouseName, string WarehouseLoc, string WarehouseType, string Notes)
        {
            Query = "INSERT INTO [dbo].[DeliveryOrderH] ([DeliveryOrderId] ,[DeliveryOrderDate] ,[DeliveryOrderStatus] ,[SalesOrderId] ,[SalesOrderDate] ,[DeliveryDate] ,[CustID] ,[CustName] ,[VendorEkspedisi] ,[VendorEkspedisiName] ,[DriverName] ,[VehicleType] ,[VehicleNumber] ,[InventSiteID] ,[InventSiteName] ,[InventSiteLocation], SiteType ,[Notes] ,[CreatedDate] ,[CreatedBy] ,[UpdatedDate] ,[UpdatedBy], DeliveryMethod) VALUES ";
            Query += "('" + DOID + "', @DeliveryOrderDate, ";
            if (cmbDelivMethod.Text == "FRANCO")
                Query += "'03', ";
            else
                Query += "'01', ";
            Query += "'" + SOID + "', @SalesOrderDate, @DeliveryDate, '" + CustID + "', '" + CustName + "', '" + VendID + "', '" + VendName + "', @DriverName, @VType, @VNumber, '" + WarehouseID + "', '" + WarehouseName + "', '" + WarehouseLoc + "', '" + WarehouseType + "', COALESCE(NULLIF(RTRIM(@Notes), ''), NULL), getdate(), '" + ControlMgr.UserId + "', '1753-01-01', NULL, '" + cmbDelivMethod.Text + "')";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DeliveryOrderDate", dtDO);
            Cmd.Parameters.AddWithValue("@SalesOrderDate", dtSO);
            Cmd.Parameters.AddWithValue("@DeliveryDate", dtDeliv);
            Cmd.Parameters.AddWithValue("@DriverName", DriverName);
            Cmd.Parameters.AddWithValue("@VType", VType);
            Cmd.Parameters.AddWithValue("@VNumber", VNumber);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.ExecuteNonQuery();
        }

        private void InsertDeliveryOrderD_SORemainingQty(string DOID, int DOSeqNo, string SOID, int SOSeqNo, string GroupID, string SubGroup1ID, string SubGroup2ID, string ItemID, string FullItemID, string ItemName, decimal Qty, string Unit, decimal Qty_Alt, string Unit_Alt, decimal ConvertionRatio, string SiteId, string NRJID)
        {
            decimal tempQty;
            tempQty = Qty;
            Query = "Select RemainingQty from SalesOrderD where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            tempQty = (Decimal)Cmd.ExecuteScalar() - tempQty;

            //Query = "select Lock_Qty from InventLockTable where RefTransId = '" + SOID + "' and RefTrans_SeqNo = '" + SOSeqNo + "' and SiteId = '" + SiteId + "'";
            Query = "select sum(Lock_Qty) from InventLockTable where (RefTransId = '" + SOID + "' and RefTrans_SeqNo = '" + SOSeqNo + "') or (RefTrans2Id = '" + SOSeqNo + "' and RefTrans2_SeqNo = '" + SOSeqNo + "') and SiteId = '" + SiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            decimal Lock_Qty = Convert.ToDecimal(Cmd.ExecuteNonQuery());

            Query = "update SalesOrderD set RemainingQty = '" + tempQty + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + SOID + "' and SeqNo = '" + SOSeqNo + "'";

            Query += "INSERT INTO [dbo].[DeliveryOrderD] ([DeliveryOrderId] ,[SeqNo] ,[SalesOrderId] ,[SalesOrderSeqNo] ,[GroupID] ,[SubGroup1ID] ,[SubGroup2ID] ,[ItemID] ,[FullItemID] ,[ItemName] ,Qty_Available, Qty_Reserved,[Qty] ,[Unit] ,[Qty_Alt] ,[Unit_Alt] ,[ConvertionRatio] ,[CreatedDate] ,[CreatedBy] ,[UpdatedDate] ,[UpdatedBy], [RemainingQty], NRJ_Id) VALUES ('" + DOID + "', '" + DOSeqNo + "', '" + SOID + "', '" + SOSeqNo + "', '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', ";
            if (Qty > Lock_Qty)
                Query += "'" + Convert.ToDecimal(Qty - Lock_Qty) + "', '" + Lock_Qty + "', ";
            else
                Query += "'0', '" + Qty + "', ";
            Query += "'" + Qty + "', '" + Unit + "', '" + Qty_Alt + "', '" + Unit_Alt + "', '" + ConvertionRatio + "', getdate(), '" + ControlMgr.UserId + "', '1753-01-01', NULL, '" + Qty + "', @NRJ_Id) ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NRJ_Id", NRJID == String.Empty ? (object)DBNull.Value : NRJID);
            Cmd.ExecuteNonQuery();
        }

        private void UpdateSalesOrderH()
        {
            string status = "";
            int tempSeqNo; string tempFullItemID; decimal tempQty;
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                tempSeqNo = Convert.ToInt32(dataGridView2.Rows[i].Cells["SO_SeqNo"].Value);
                tempFullItemID = dataGridView2.Rows[i].Cells["FullItemId"].Value.ToString();

                Query = "select RemainingQty from SalesOrderD where SalesOrderNo = '" + tbxSOID.Text + "' and SeqNo = '" + tempSeqNo + "' and FullItemID = '" + tempFullItemID + "'";
                Cmd = new SqlCommand(Query, Conn);
                if ((Decimal)Cmd.ExecuteScalar() <= 0)
                    status = "07"; //DO Completed
                else
                {
                    status = "06"; //DO Partial
                    break;
                }
            }
            Query = "update [dbo].[SalesOrderH] set TransStatus = '" + status + "',UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + tbxSOID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "RemainingQty")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["RemainingQty"].Value != null && dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Ratio"].Value != null)
                    dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Qty Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["RemainingQty"].Value) * Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Ratio"].Value);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                if (tabControl1.SelectedTab == tabPage1)
                {
                    int index = dataGridView1.CurrentRow.Index;
                    dataGridView1.Rows.RemoveAt(index);
                    dataGridView2.Rows.RemoveAt(index);
                }
                else if (tabControl1.SelectedTab == tabPage2)
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (dataGridView1.Rows[i].Cells["RefTransId"].Value.ToString() == dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["SOID"].Value.ToString() && dataGridView1.Rows[i].Cells["SO_SeqNo"].Value.ToString() == dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["SO_SeqNo"].Value.ToString())
                            dataGridView1.Rows.RemoveAt(i);
                    }
                    dataGridView2.Rows.RemoveAt(dataGridView2.CurrentRow.Index);
                }
                else if (tabControl1.SelectedTab == tabRetur)
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (dataGridView1.Rows[i].Cells["RefTransId"].Value.ToString() == dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["NRJId"].Value.ToString() && dataGridView1.Rows[i].Cells["SO_SeqNo"].Value.ToString() == dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["NRJ_SeqNo"].Value.ToString())
                            dataGridView1.Rows.RemoveAt(i);
                    }
                    dataGridView4.Rows.RemoveAt(dataGridView4.CurrentRow.Index);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabRetur)
            {
                if (tbxInventSiteID.Text == String.Empty || tbxSOID.Text == String.Empty)
                {
                    MetroFramework.MetroMessageBox.Show(this, "Please select SO ID and Warehouse to add item!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SearchV2 f = new SearchV2();
                string where = "";
                for (int i = 0; i < dataGridView4.RowCount; i++)
                {
                    if (i >= 1)
                        where += " or ";
                    where += "a.NRJId = '" + dataGridView4.Rows[i].Cells["NRJId"].Value.ToString() + "' and a.SeqNo = '" + dataGridView4.Rows[i].Cells["NRJ_SeqNo"].Value.ToString() + "'";
                }
                if (dataGridView4.RowCount > 0)
                    f.SetSchemaTable("dbo", "NotaReturJual_Dtl", "and SOH.SalesOrderNo = '" + tbxSOID.Text + "' and a.RemainingQty > 0 and not (" + where + ")", "a.*", "NotaReturJual_Dtl a left join NotaReturJualH b on a.NRJId = b.NRJId left join SalesOrderH SOH on b.SalesId = SOH.SalesOrderNo");
                else
                    f.SetSchemaTable("dbo", "NotaReturJual_Dtl", "and SOH.SalesOrderNo = '" + tbxSOID.Text + "' and a.RemainingQty > 0", "a.*", "NotaReturJual_Dtl a left join NotaReturJualH b on a.NRJId = b.NRJId left join SalesOrderH SOH on b.SalesId = SOH.SalesOrderNo");
                f.SetMode("Check");
                f.ShowDialog();

                if (SearchV2.data.Count != 0)
                {
                    dataGridView4.ColumnCount = tableCols4.Length;
                    for (int i = 0; i < tableCols4.Length; i++)
                        dataGridView4.Columns[i].Name = tableCols4[i];

                    Conn = ConnectionString.GetConnection();
                    for (int i = 0; i < SearchV2.data.Count; i++)
                    {
                        Query = "select * from NotaReturJual_Dtl where NRJId = '" + SearchV2.data[i] + "' and SeqNo = '" + SearchV2.data2[i] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            #region pass value to dataGridView1
                            dataGridView1.Rows.Add(1);
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["No"].Value = dataGridView1.RowCount;
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SO_SeqNo"].Value = Dr["SeqNo"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["RefTransId"].Value = Dr["NRJId"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["GroupID"].Value = Dr["GroupId"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SubGroup1ID"].Value = Dr["SubGroupID"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SubGroup2ID"].Value = Dr["SubGroup2ID"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ItemID"].Value = Dr["ItemID"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["FullItemId"].Value = Dr["FullItemId"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ItemName"].Value = Dr["ItemName"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Qty"].Value = Dr["UoM_Qty"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["RemainingQty"].Value = Dr["RemainingQty"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Unit"].Value = Dr["UoM_Unit"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Qty Alt"].Value = Convert.ToDecimal(Dr["RemainingQty"]) / Convert.ToDecimal(Dr["Ratio"]);
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Unit Alt"].Value = Dr["Alt_Unit"];
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Ratio"].Value = Dr["Ratio"];
                            #endregion

                            #region pass value to dataGridView4
                            dataGridView4.Rows.Add(1);
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["No"].Value = dataGridView4.RowCount;
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["NRJId"].Value = Dr["NRJId"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["NRJ_SeqNo"].Value = Dr["SeqNo"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["GroupID"].Value = Dr["GroupId"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["SubGroupID"].Value = Dr["SubGroupID"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["SubGroup2ID"].Value = Dr["SubGroup2ID"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["ItemID"].Value = Dr["ItemID"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["FullItemId"].Value = Dr["FullItemId"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["ItemName"].Value = Dr["ItemName"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["RemainingQty"].Value = Dr["RemainingQty"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["UoM_Qty"].Value = Dr["UoM_Qty"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["UoM_Unit"].Value = Dr["UoM_Unit"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["Alt_Qty"].Value = Dr["Alt_Qty"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["Alt_Unit"].Value = Dr["Alt_Unit"];
                            dataGridView4.Rows[dataGridView4.RowCount - 1].Cells["Ratio"].Value = Dr["Ratio"];
                            #endregion
                        }
                        Dr.Close();
                    }
                    Conn.Close();
                    gvFormat();
                }
            }
            else
            {
                if (tbxInventSiteID.Text == String.Empty || tbxSOID.Text == String.Empty)
                {
                    MetroFramework.MetroMessageBox.Show(this, "Please select SO ID and Warehouse to add item!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //SearchQueryV2 tmpSearch = new SearchQueryV2();

                AddItemDO tmpSearch = new AddItemDO();

                tmpSearch.Text = "Search Item Resize";
                tmpSearch.PrimaryKey = "FullItemId";
                if (dataGridView1.RowCount > 0)
                {
                    string where = "";
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (i >= 1)
                            where += " or ";
                        where += "a.FullItemID = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' and a.SeqNo = '" + dataGridView1.Rows[i].Cells["SO_SeqNo"].Value.ToString() + "' and b.SiteId = '" + tbxInventSiteID.Text + "'";
                    }
                    tmpSearch.QuerySearch = "Select a.SalesOrderNo,a.SeqNo,a.GroupId,a.SubGroup1Id,a.SubGroup2Id,a.ItemId,a.FullItemId,a.ItemName, b.SiteId,a.Qty,b.LockQtyUom 'LockQty',a.RemainingQty,a.Unit from SalesOrderD a left join (Select sum(Lock_Qty) 'LockQtyUom', RefTransId, RefTrans_SeqNo, SiteId from [InventLockTable] group by RefTransId, RefTrans_SeqNo, SiteId) b on a.SalesOrderNo=b.RefTransId and a.SeqNo=b.RefTrans_SeqNo where a.SalesOrderNo='" + tbxSOID.Text + "' and a.RemainingQty != 0 and a.Qty_Return = 0  and a.DeliveryMethod = '" + cmbDelivMethod.Text + "' and not ( " + where + " )";
                }
                else
                {
                    tmpSearch.QuerySearch = "Select a.SalesOrderNo,a.SeqNo,a.GroupId,a.SubGroup1Id,a.SubGroup2Id,a.ItemId,a.FullItemId,a.ItemName, b.SiteId,a.Qty,b.LockQtyUom 'LockQty',a.RemainingQty,a.Unit from SalesOrderD a left join (Select sum(Lock_Qty) 'LockQtyUom', RefTransId, RefTrans_SeqNo, SiteId from [InventLockTable] group by RefTransId, RefTrans_SeqNo, SiteId) b on a.SalesOrderNo=b.RefTransId and a.SeqNo=b.RefTrans_SeqNo where a.SalesOrderNo='" + tbxSOID.Text + "' and a.RemainingQty != 0 and a.Qty_Return = 0 and a.DeliveryMethod = '" + cmbDelivMethod.Text + "'";
                }
                tmpSearch.FilterText = new string[] { "FullItemId", "ItemName", "Unit" };
                tmpSearch.Select = new string[] { "SalesOrderNo", "SeqNo", "SiteId" };
                tmpSearch.Select2 = new string[] { "FullItemId" };
                tmpSearch.HideField = new string[] { "SeqNo", "GroupId", "SubGroup1Id", "SubGroup2Id", "ItemId" };
                //tmpSearch.WherePlus = " and ( a.FullItemID != '" + dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["FullItemId"].Value.ToString() + "' or a.SeqNo != '" + dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["SeqNo"].Value.ToString() + "' ) ";
                tmpSearch.Parent = this;
                tmpSearch.Notes = "";

                tmpSearch.ShowDialog();

                if (Variable.Kode2 != null && Variable.Kode2.Length != 0)
                {
                    string where = "";
                    for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
                    {
                        if (i >= 1)
                            where += " or ";
                        where += "RefTransId = '" + Variable.Kode2[i, 0] + "' and ";
                        where += "RefTrans_SeqNo = '" + Variable.Kode2[i, 1] + "' and ";
                        where += "SiteId = '" + Variable.Kode2[i, 2] + "'";

                    }
                    //CHECK IF ITEMS LOCKID ARE FROM DIFFERENT WAREHOUSE
                    Conn = ConnectionString.GetConnection();
                    Query = "Select count(distinct(SiteId)) from InventLockTable where " + where;
                    Cmd = new SqlCommand(Query, Conn);
                    int countWarehouse = (Int32)Cmd.ExecuteScalar();

                    if (countWarehouse == 0)
                    {
                        PassDataToGV();
                    }
                    else if (countWarehouse == 1)
                    {
                        //CHECK IF SELECTED ITEM LOCKID EQUAL TO TBXWAREHOUSE
                        Query = "Select distinct(SiteId) from InventLockTable where " + where;
                        Cmd = new SqlCommand(Query, Conn);
                        string warehouse = Cmd.ExecuteScalar().ToString();

                        if (tbxInventSiteID.Text == String.Empty || warehouse == tbxInventSiteID.Text)
                        {
                            //PASS WAREHOUSE NAME BASED ON SELECTED ITEM LOCK ID
                            //Query = "select InventSiteID, InventSiteName, Lokasi, SiteType from InventSite where InventSiteID = '" + warehouse + "'";
                            //Cmd = new SqlCommand(Query, Conn);
                            //Dr = Cmd.ExecuteReader();
                            //while (Dr.Read())
                            //{
                            //    tbxInventSiteID.Text = Dr["InventSiteID"].ToString();
                            //    tbxWarehouse.Text = Dr["InventSiteName"].ToString();
                            //    tbxLocation.Text = Dr["Lokasi"].ToString();
                            //    tbxInventSiteType.Text = Dr["SiteType"].ToString();
                            //}
                            //Dr.Close();

                            PassDataToGV();
                        }
                        else
                            MetroFramework.MetroMessageBox.Show(this, "Please change warehouse to " + warehouse + " to add this item!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        MetroFramework.MetroMessageBox.Show(this, "Cannot add item with different warehouse!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Variable.Kode2 = null;
                }

                //STV EDIT start 25/07/2018
                //DO, Create DO bisa tarik item lain selain di SO dengan ketentuan : 4 suku item nya sama.
                if (Variable.Kode2a != null && Variable.Kode2a.Length != 0)
                {
                    PassItemToGV();
                    Variable.Kode2a = null;
                }
                //STV EDIT end
            }
        }

        //public void AdditionalItemOverview(string fullitemid, string itemname, string qty, string remainingqty, string unit)
        //{
        //    dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", /*Dr["SeqNo"]*/"", /*Dr["SalesOrderNo"]*/"", /*Dr["GroupID"]*/"", Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["ConvertionRatio"]);

        //}

        private void PassDataToGV()
        {
            //where = "";
            //for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
            //{
            //    if (i >= 1)
            //        where += " or ";
            //    where += "a.SalesOrderNo = '" + Variable.Kode2[i, 0] + "' and ";
            //    where += "a.SeqNo = '" + Variable.Kode2[i, 1] + "' and ";
            //    where += "c.SiteId = '" + Variable.Kode2[i, 2] + "'";
            //}
            //Query = "select a.SeqNo, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.FullItemID, a.ItemName, a.Qty, a.RemainingQty, b.Available_For_Sale_UoM, CASE WHEN c.Lock_Qty IS NULL then 0 else c.Lock_Qty END AS Lock_Qty, b.Available_For_Sale_Reserved_UoM, a.Unit, a.Qty_Alt, a.Unit_Alt, a.ConvertionRatio from SalesOrderD a left join Invent_OnHand_Qty b on a.FullItemID=b.FullItemId left join InventLockTable c on a.SalesOrderNo = c.RefTransId and a.SeqNo = c.RefTrans_SeqNo and c.SiteId = b.InventSiteId where " + where;
            string where = "";
            for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
            {
                if (i >= 1)
                    where += " or ";
                where += "a.SalesOrderNo = '" + Variable.Kode2[i, 0] + "' and ";
                where += "a.SeqNo = '" + Variable.Kode2[i, 1] + "' and ";
                if (Variable.Kode2[i, 2] == String.Empty)
                    where += "c.SiteId is null and ";
                else
                    where += "c.SiteId = '" + Variable.Kode2[i, 2] + "' and ";
                where += "b.InventSiteId = '" + tbxInventSiteID.Text + "'";
            }
            Query = "select a.SalesOrderNo, a.SeqNo, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.FullItemID, a.ItemName, a.Qty, a.RemainingQty, b.Available_For_Sale_UoM, CASE WHEN c.LockQtyUom IS NULL then 0 else c.LockQtyUom END AS Lock_Qty, b.Available_For_Sale_Reserved_UoM, a.Unit, a.Qty_Alt, a.Unit_Alt, a.ConvertionRatio, c.SiteId, b.InventSiteId, a.SalesOrderNo from SalesOrderD a left join Invent_OnHand_Qty b on a.FullItemID=b.FullItemId left join(Select sum(Lock_Qty) 'LockQtyUom', RefTransId, RefTrans_SeqNo, SiteId from [InventLockTable] group by RefTransId, RefTrans_SeqNo, SiteId) c on a.SalesOrderNo = c.RefTransId and a.SeqNo = c.RefTrans_SeqNo and c.SiteId = b.InventSiteId where " + where;
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            //if(!(Dr.HasRows))
            //    MetroFramework.MetroMessageBox.Show(this, "
            while (Dr.Read())
            {
                //TAB ITEM OVERVIEW
                dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", Dr["SeqNo"], Dr["SalesOrderNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["ConvertionRatio"]);

                //TAB ITEM DETAIL 
                dataGridView2.Rows.Add(dataGridView2.RowCount + 1, "", Dr["SeqNo"], Dr["SalesOrderNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Available_For_Sale_UoM"], 0, Dr["Lock_Qty"], Dr["Available_For_Sale_Reserved_UoM"], 0, Dr["Unit"], 0, Dr["Unit_Alt"], Dr["ConvertionRatio"]);
            }
            Dr.Close();
        }

        private void PassItemToGV()
        {
            string where = "";
            for (int i = 0; i < Variable.Kode2a.GetLength(0); i++)
            {
                if (i >= 1)
                    where += " or ";
                where += " FullItemId = '" + Variable.Kode2a[i, 0] + "'";
            }

            Query = "select GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemId, ItemDeskripsi, UoM,UoMAlt ,Ratio From InventTable where " + where;
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                //TAB ITEM OVERVIEW
                //dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", Dr["SeqNo"], Dr["SalesOrderNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["ConvertionRatio"]);
                dataGridView1.Rows.Add(dataGridView1.RowCount + 1, "", 0, "", Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], "", Dr["FullItemId"], Dr["ItemDeskripsi"], 0, 0, Dr["UoM"], 0, Dr["UomAlt"], Dr["Ratio"]);

                //TAB ITEM DETAIL 
                //dataGridView2.Rows.Add(dataGridView2.RowCount + 1, "", Dr["SeqNo"], Dr["SalesOrderNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Available_For_Sale_UoM"], 0, Dr["Lock_Qty"], Dr["Available_For_Sale_Reserved_UoM"], 0, Dr["Unit"], 0, Dr["Unit_Alt"], Dr["ConvertionRatio"]);
                dataGridView2.Rows.Add(dataGridView2.RowCount + 1, "", 0, "", Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemId"], Dr["ItemDeskripsi"], 0, 0, 0, 0, 0, 0, 0, Dr["UoM"], 0, Dr["UomAlt"], Dr["Ratio"]);
            }
            Dr.Close();
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
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end  
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";
            GetDataHeader();
            ModeBeforeEdit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Conn = ConnectionString.GetConnection();
                Query = "update DeliveryOrderH set DeliveryOrderStatus = '06', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where DeliveryOrderId = '" + tbxDOID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                Conn.Close();
                scope.Complete();
            }
            Parent.RefreshGrid();
            MetroFramework.MetroMessageBox.Show(this, tbxDOID.Text + " printing!\r\nOnly update status, still not printing!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnPrint.Enabled = false; btnEdit.Enabled = false;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                e.Control.KeyPress -= new KeyPressEventHandler(dataGridView1_KeyPress);
                if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "RatioActual")
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                        tb.KeyPress += new KeyPressEventHandler(dataGridView1_KeyPress);
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["ItemName"].Frozen = true;

            dataGridView1.Columns["SeqNo"].Visible = false;
            dataGridView1.Columns["SO_SeqNo"].Visible = false;
            dataGridView1.Columns["RefTransId"].Visible = false;
            dataGridView1.Columns["GroupID"].Visible = false;
            dataGridView1.Columns["SubGroup1ID"].Visible = false;
            dataGridView1.Columns["SubGroup2ID"].Visible = false;
            dataGridView1.Columns["ItemID"].Visible = false;
            dataGridView1.Columns["Qty Alt"].Visible = false;
            dataGridView1.Columns["Unit Alt"].Visible = false;
            dataGridView1.Columns["Ratio"].Visible = false;

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Qty"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void gvFormat()
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
                dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
            }

            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                if (tableCols2[i] == "DO Qty" && Mode != "BeforeEdit")
                {
                    dataGridView2.Columns[tableCols2[i]].ReadOnly = false;
                    dataGridView2.Columns[tableCols2[i]].DefaultCellStyle.BackColor = Color.White;
                }
                else
                {
                    dataGridView2.Columns[tableCols2[i]].ReadOnly = true;
                    dataGridView2.Columns[tableCols2[i]].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            for (int i = 0; i < dataGridView3.ColumnCount; i++)
            {
                dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
            }

            for (int i = 0; i < dataGridView4.ColumnCount; i++)
            {
                if (tableCols4[i] == "UoM_Qty" && Mode != "BeforeEdit")
                {
                    dataGridView4.Columns[tableCols4[i]].ReadOnly = false;
                    dataGridView4.Columns[tableCols4[i]].DefaultCellStyle.BackColor = Color.White;
                }
                else
                {
                    dataGridView4.Columns[tableCols4[i]].ReadOnly = true;
                    dataGridView4.Columns[tableCols4[i]].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.Columns["ItemName"].Frozen = true;

            dataGridView2.Columns["SeqNo"].Visible = false;
            dataGridView2.Columns["SO_SeqNo"].Visible = false;
            dataGridView2.Columns["SOID"].Visible = false;
            dataGridView2.Columns["GroupID"].Visible = false;
            dataGridView2.Columns["SubGroup1ID"].Visible = false;
            dataGridView2.Columns["SubGroup2ID"].Visible = false;
            dataGridView2.Columns["ItemID"].Visible = false;
            dataGridView2.Columns["Qty Available"].Visible = false;
            dataGridView2.Columns["Qty Reserved"].Visible = false;
            dataGridView2.Columns["Qty Alt"].Visible = false;
            dataGridView2.Columns["Unit Alt"].Visible = false;
            dataGridView2.Columns["Ratio"].Visible = false;

            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("Qty"))
            {

                if (e.Value == "" || e.Value == null || e.Value.ToString() == "")
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void btnAddNotaTransfer_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                NTForm NTForm = new NTForm();
                ListNTForm.Add(NTForm);
                //NTForm.SetMode("New", "", "");
                NTForm.SetParent(this);
                NTForm.ModeNew();
                NTForm.txtBoxRefType.Text = "DELIVERY ORDER";
                NTForm.txtInventSiteIDTo.Text = tbxInventSiteID.Text;
                NTForm.txtWarehouseTo.Text = tbxWarehouse.Text;
                NTForm.tbxRefID.Text = tbxDOID.Text;
                NTForm.btnSearchTo.Enabled = false;
                NTForm.ShowDialog();
                using (TransactionScope scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select * from NotaTransferH where ReferenceType = 'DELIVERY ORDER' and ReferenceId = '" + tbxDOID.Text + "' and (TransStatus = '01' or TransStatus = '02')";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        Query = "update DeliveryOrderH set DeliveryOrderStatus = '02', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where DeliveryOrderId = '" + tbxDOID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    Conn.Close();
                    scope.Complete();
                }
                Parent.RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end  
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView2.Columns[e.ColumnIndex].Name == "DO Qty")
            {
                if (dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["DO Qty"].Value != null && dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["Ratio"].Value != null)
                    dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["Qty Alt"].Value = Convert.ToDecimal(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["DO Qty"].Value) * Convert.ToDecimal(dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["Ratio"].Value);
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView3.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView3.Columns["Seq_No"].Visible = false;

            if (dataGridView3.Columns[e.ColumnIndex].Name.Contains("Qty"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView3.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void dataGridView4_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView4.Columns["NRJId"].Visible = false;
            dataGridView4.Columns["NRJ_SeqNo"].Visible = false;
            dataGridView4.Columns["SeqNo"].Visible = false;
            dataGridView4.Columns["GroupID"].Visible = false;
            dataGridView4.Columns["SubGroupID"].Visible = false;
            dataGridView4.Columns["SubGroup2ID"].Visible = false;
            dataGridView4.Columns["ItemID"].Visible = false;
            dataGridView4.Columns["Alt_Qty"].Visible = false;
            dataGridView4.Columns["Alt_Unit"].Visible = false;
            dataGridView4.Columns["Ratio"].Visible = false;
            if (dataGridView4.Columns[e.ColumnIndex].Name.Contains("Qty"))
            {

                if (e.Value == "" || e.Value == null || e.Value.ToString() == "")
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView4.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dataGridView4.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView4.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void dataGridView4_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView4.Columns[e.ColumnIndex].Name == "UoM_Qty")
            {
                if (dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["UoM_Qty"].Value != null && dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["Ratio"].Value != null)
                    dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["Alt_Qty"].Value = Convert.ToDecimal(dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["UoM_Qty"].Value) * Convert.ToDecimal(dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["Ratio"].Value);
            }
        }
        //tia edit
        //klik kanan
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.CustomerID.Customer Cust = null;
        PopUp.Vendor.Vendor Vend = null;
        Sales.SalesOrder.SOHeader SOID = null;

        Sales.BBK.BBKHeader ParentToGI;
        AccountAssignment.GLJournal.FormGLJournalHeader ParentToGL;
        TaskList.GlobalTasklist Parent2 = new TaskList.GlobalTasklist();

        public void SetParent2(TaskList.GlobalTasklist Tl)
        {
            Parent2 = Tl;
        }
        public void ParentRefreshGrid(Sales.BBK.BBKHeader gi)
        {
            ParentToGI = gi;
        }
        public void ParentRefreshGrid2(AccountAssignment.GLJournal.FormGLJournalHeader GL)
        {
            ParentToGL = GL;
        }


        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {

                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                        itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                    FID.Show();
                    FID.Focus();
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

        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView2.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {

                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dataGridView2.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                        itemID = dataGridView2.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dataGridView2.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }

        private void dataGridView4_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dataGridView4.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView4.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dataGridView4.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                        itemID = dataGridView4.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dataGridView4.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
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

        private void tbxVendID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    tbxVendID.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(tbxVendID.Text);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.Show();
                    Vend.Focus();
                }
            }
        }

        private void tbxVendName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    tbxVendName.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(tbxVendID.Text);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.Show();
                    Vend.Focus();
                }
            }
        }

        private void tbxSOID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SOID == null || SOID.Text == "")
                {
                    tbxSOID.Enabled = true;
                    SOID = new Sales.SalesOrder.SOHeader();
                    SOID.SetMode("PopUp", tbxSOID.Text);
                    SOID.ParentRefreshGrid4(this);
                    SOID.Show();
                }
                else if (CheckOpened(SOID.Name))
                {
                    SOID.WindowState = FormWindowState.Normal;
                    SOID.Show();
                    SOID.Focus();
                }
            }
        }

        private void cmbDelivMethod_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        //end
    }
}
