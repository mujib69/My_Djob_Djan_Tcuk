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
namespace ISBS_New.Sales.BBK
{
    public partial class BBKHeader : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
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
        private string BBKID;
        /**********STANDARD*********/

        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "No", "GoodsIssuedSeqNo", "SeqNo", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "Qty", "Qty_Actual", "Unit", "Ratio", "TotalBerat", "TotalBerat_Actual", "InventSiteBlokID", "ActionCode", "Notes" };

        string[] tableCols2 = new string[] { "No", "FullItemID", "ItemName", "Ref Qty", "Ref Remaining Qty", "Unit", "Item Type" };
        /*********datagridview cols name*********/

        /*********PARENT*********/
        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F) { Parent = F; }
        /*********PARENT*********/

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        //bool check; //label
        /*********VALIDATION*********/

        /*********GV COMBO BOX*********/
        DataGridViewComboBoxCell cell;
        private SqlDataReader Dr2;
        /*********GV COMBO BOX*********/

        /*******Res Remaining**********/
        //for nota transfer
        decimal[] RemainingReserved;
        /******************************/

        /*******REFERENCE**************/
        string RefType = "";
        /******************************/
        //tia edit
        PopUp.FullItemId.FullItemId FullItemId = new PopUp.FullItemId.FullItemId();
        ContextMenu vendid = new ContextMenu();

        bool Journal = false;
        //tia edit end
        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public BBKHeader()
        {
            InitializeComponent();
        }

        public BBKHeader(string Referencetype)
        {
            InitializeComponent();
            RefType = Referencetype;
        }

        private void BBKHeader_Load(object sender, EventArgs e)
        {
            tabPage1.Text = "Detail BBK";
            if (Mode == "New")
            {
                cbRef.SelectedIndex = 0;
                ModeNew();
                //created by Thaddaeus, 28MAY2018
                if (RefType == "NOTA TRANSFER")
                {
                    cbRef.SelectedIndex = 2;
                    cbRef.Enabled = false;
                }
                else
                {
                    cbRef.Items.Remove("Nota Transfer");
                }
                //END===========================
            }
            else if (Mode == "BeforeEdit")
            {
                GetDataHeader();
                GetDataHeader2();
                ModeBeforeEdit();
            }
            else if (Mode == "PopUp")
            {
                GetDataHeader();
                GetDataHeader2();
                ModePopUp();

            }
            modeVirtualSite();
        }

        #region SetMode
        public void SetMode(string tmpMode, string tmpNumber)
        {
            Mode = tmpMode;
            tbxBBKID.Text = tmpNumber;
            BBKID = tmpNumber;
        }
        #endregion

        private void btnSSOID_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    if (ControlMgr.GroupName == "Sales Admin")
                    {
                        f.SetSchemaTable("dbo", "DeliveryOrderH", "and a.DeliveryOrderStatus not in ('02', '07', '08') AND b.[SiteType]='Virtual Site' ", "a.*", "DeliveryOrderH a LEFT JOIN [dbo].[InventSite] b ON a.[InventSiteID]=b.[InventSiteID] ");
                    }
                    else
                    {
                        f.SetSchemaTable("dbo", "DeliveryOrderH", "and a.DeliveryOrderStatus not in ('02', '07', '08') AND b.[SiteType]='Physical Site' ", "a.*", "DeliveryOrderH a LEFT JOIN [dbo].[InventSite] b ON a.[InventSiteID]=b.[InventSiteID] ");
                    }
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        dataGridView1.Rows.Clear();
                        tbxRefID.Text = SearchV2.data[0];
                        GetDataHeader();
                        GetDataHeader2();
                    }
                    break;
                case "Nota Transfer":
                    f.SetSchemaTable("dbo", "NotaTransferH", "and a.TransStatus in ('02','05','09')", "a.*", "NotaTransferH a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        dataGridView1.Rows.Clear();
                        tbxRefID.Text = SearchV2.data[0];
                        GetDataHeader();
                        GetDataHeader2();
                    }
                    break;
                case "Nota Retur Beli":
                    f.SetSchemaTable("dbo", "NotaReturBeliH", "and a.TransStatusId in ('03', '04')", "a.*", "NotaReturBeliH a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        dataGridView1.Rows.Clear();
                        tbxRefID.Text = SearchV2.data[0];
                        GetDataHeader();
                        GetDataHeader2();
                    }
                    break;
                case "Nota Retur Jual":
                    f.SetSchemaTable("dbo", "NotaReturJualH", "and a.TransStatusId in ('03')", "a.*", "NotaReturJualH a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        dataGridView1.Rows.Clear();
                        tbxRefID.Text = SearchV2.data[0];
                        GetDataHeader();
                        GetDataHeader2();
                    }
                    break;
            }
        }

        private void GetDataHeader2()
        {
            dataGridView2.Rows.Clear();
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.ColumnCount = tableCols2.Length + 1;
            for (int i = 0; i < tableCols2.Length; i++)
            {
                dataGridView2.Columns[i].Name = tableCols2[i];
            }
            Conn = ConnectionString.GetConnection();
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select FullItemID, ItemName, qty 'Ref Qty', remainingQty 'Ref Remaining Qty', unit 'Unit', NRJ_Id from DeliveryOrderD where DeliveryOrderId = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Transfer":
                    Query = "select FullItemID, ItemName, qty 'Ref Qty', remainingQty 'Ref Remaining Qty', uom 'Unit','' as 'Item Type' from NotaTransferD where TransferNo = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Retur Beli":
                    Query = "select FullItemID, ItemName, uom_qty 'Ref Qty', remainingQty 'Ref Remaining Qty', UoM_Unit 'Unit','' as 'Item Type' from NotaReturBeli_Dtl where NRBId = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Retur Jual":
                    Query = "select [FullItemId] as 'FullItemID', ItemName, [UoM_Qty] 'Ref Qty', [Remaining_Qty_DO] 'Ref Remaining Qty', UoM_Unit 'Unit','' as 'Item Type' from NotaReturJual_Dtl where NRJId = '" + tbxRefID.Text + "'";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int row = 0;
            while (Dr.Read())
            {
                dataGridView2.Rows.Add(1);
                for (int i = 0; i < tableCols2.Length; i++)
                {
                    if (i == 0)
                        dataGridView2.Rows[row].Cells[tableCols2[i]].Value = dataGridView2.RowCount;
                    else if (tableCols2[i] == "Item Type")
                    {
                        if (cbRef.Text == "Delivery Order")
                        {
                            if (Dr["NRJ_Id"] != (object)DBNull.Value)
                                dataGridView2.Rows[row].Cells[tableCols2[i]].Value = "Item Retur";
                        }
                    }
                    else
                        dataGridView2.Rows[row].Cells[tableCols2[i]].Value = Dr[tableCols2[i]];
                }
                row++;
            }
            Dr.Close();
            Conn.Close();
        }

        private void GenerateGVRows(string Query)
        {
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int x = dataGridView1.RowCount;
            decimal qty = 0, ratio = 0;
            while (Dr.Read())
            {
                dataGridView1.Rows.Add(1);
                for (int i = 0; i < Convert.ToInt32(tableCols.Length); i++)
                {
                    if (!(tableCols[i] == "Qty_Actual" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "ActionCode" || tableCols[i] == "GoodsIssuedSeqNo"))
                    {
                        switch (cbRef.Text)
                        {
                            case "Delivery Order":
                                if (tableCols[i] == "No")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                                else if (tableCols[i] == "Qty")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["RemainingQty"];
                                else if (tableCols[i] == "Ratio")
                                {
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["ConvertionRatio"];
                                }
                                else if (tableCols[i] == "InventSiteBlokID")
                                {
                                    cellValue("Select [InventSiteBlokID] from [dbo].[InventSiteBlok] where InventSiteID = '" + tbxInventSiteID.Text + "'");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]] = cell;
                                }
                                else if (tableCols[i] == "TotalBerat")
                                {
                                    qty = (decimal)Dr["RemainingQty"];
                                    ratio = (decimal)Dr["ConvertionRatio"];
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Convert.ToString(qty * ratio);
                                }
                                else
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                                break;
                            case "Nota Transfer":
                                if (tableCols[i] == "No")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                                else if (tableCols[i] == "Qty")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["RemainingQty"];
                                else if (tableCols[i] == "Unit")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["UoM"];
                                else if (tableCols[i] == "InventSiteBlokID")
                                {
                                    cellValue("Select [InventSiteBlokID] from [dbo].[InventSiteBlok] where InventSiteID = '" + tbxInventSiteID.Text + "'");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]] = cell;
                                }
                                else if (tableCols[i] == "TotalBerat")
                                {
                                    qty = (decimal)Dr["RemainingQty"];
                                    ratio = (decimal)Dr["Ratio"];
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Convert.ToString(qty * ratio);
                                }
                                else
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                                break;
                            case "Nota Retur Beli":
                                if (tableCols[i] == "No")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                                else if (tableCols[i] == "Qty")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["RemainingQty"];
                                else if (tableCols[i] == "Unit")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["UoM_Unit"];
                                else if (tableCols[i] == "InventSiteBlokID")
                                {
                                    cellValue("Select [InventSiteBlokID] from [dbo].[InventSiteBlok] where InventSiteID = '" + tbxInventSiteID.Text + "'");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]] = cell;
                                }
                                else if (tableCols[i] == "TotalBerat")
                                {
                                    qty = (decimal)Dr["RemainingQty"];
                                    ratio = (Dr["Ratio_Actual"].ToString() == "" ? 1 : (decimal)Dr["Ratio_Actual"]);
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Convert.ToString(qty * ratio);
                                }
                                else
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i].ToString()];
                                break;
                            case "Nota Retur Jual":
                                if (tableCols[i] == "No")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                                else if (tableCols[i] == "Qty")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["Remaining_Qty_DO"];
                                else if (tableCols[i] == "Unit")
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["UoM_Unit"];
                                else if (tableCols[i] == "InventSiteBlokID")
                                {
                                    cellValue("Select [InventSiteBlokID] from [dbo].[InventSiteBlok] where InventSiteID = '" + tbxInventSiteID.Text + "'");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]] = cell;
                                }
                                else if (tableCols[i] == "TotalBerat")
                                {
                                    qty = (decimal)Dr["Remaining_Qty_DO"];
                                    ratio = (Dr["Ratio"].ToString() == "" ? 1 : (decimal)Dr["Ratio"]);
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Convert.ToString(qty * ratio);
                                }
                                else if (tableCols[i] == "SubGroup1ID")
                                {
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["SubGroupId"].ToString();
                                }
                                else
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i].ToString()];
                                break;
                        }
                    }
                }
                x++;
            }
            Dr.Close();
            Conn.Close();
        }

        private void modeVirtualSite()
        {
            if (ControlMgr.GroupName != "Sales Admin")
            {
                return;
            }
            if (Mode == "New")
            {
                cbRef.SelectedIndex = 1;
                cbRef.Enabled = false;
                cbWeight1.Enabled = false;
                cbWeight2.Enabled = false;
                btnWeight1.Enabled = false;
                btnWeight2.Enabled = false;
                tbxDriverName.Enabled = false;
                tbxName.Enabled = false;
                tbxVNumber.Enabled = false;
                tbxVType.Enabled = false;
            }
        }

        private void GetDataHeader()
        {
            if (Mode == "New")
            {
                Conn = ConnectionString.GetConnection();
                switch (cbRef.Text)
                {
                    case "Delivery Order":
                        Query = "Select DeliveryOrderDate, CustID, CustName, VendorEkspedisiName, VendorEkspedisi, VehicleType, VehicleNumber, DriverName, InventSiteID, InventSiteName, InventSiteLocation, Notes, SiteType from DeliveryOrderH where DeliveryOrderId = '" + tbxRefID.Text + "'";
                        break;
                    case "Nota Transfer":
                        Query = "select TransferDate, InventSiteTo, InventSiteToName, VendName,VehicleOwner, VehicleType, VehicleNo, DriverName, InventSiteFrom, InventSiteFromName, InventSiteFromName, Notes from NotaTransferH LEFT OUTER JOIN VendTable ON VehicleOwner=VendId where TransferNo = '" + tbxRefID.Text + "'";
                        break;
                    case "Nota Retur Beli":
                        Query = "select a.NRBDate, a.VendId, a.VendName, a.VendName, a.VendId, b.VehicleType, b.VehicleNumber, b.DriverName, a.SiteID, a.SiteName, a.SiteName, a.Notes  from NotaReturBeliH a left join GoodsReceivedH b on a.GoodsReceivedId = b.GoodsReceivedId where NRBId = '" + tbxRefID.Text + "'";
                        break;
                    case "Nota Retur Jual":
                        Query = "select a.NRJDate, a.[CustId], a.[CustName], '', '', '', '', '', a.[SiteId], a.[SiteName], a.[SiteName], a.Notes  from NotaReturJualH a left join [GoodsIssuedH] b on a.[GoodsIssuedId] = b.[GoodsIssuedId] where NRJId = '" + tbxRefID.Text + "'";
                        break;
                }
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtRef.Value = Convert.ToDateTime(Dr[0]);
                    tbxNameID.Text = Dr[1].ToString();
                    tbxName.Text = Dr[2].ToString();
                    tbxVOwner.Text = Dr[3].ToString();
                    tbxVOwnerID.Text = Dr[4].ToString();
                    tbxVType.Text = Dr[5].ToString();
                    tbxVNumber.Text = Dr[6].ToString();
                    tbxDriverName.Text = Dr[7].ToString();
                    tbxInventSiteID.Text = Dr[8].ToString();
                    tbxWarehouse.Text = Dr[9].ToString();
                    tbxLocation.Text = Dr[10].ToString();
                    tbxNotes.Text = Dr[11].ToString();
                    if (cbRef.Text == "Delivery Order")
                    {
                        txtSiteType.Text = Dr["SiteType"] == System.DBNull.Value ? "" : Dr["SiteType"].ToString();
                    }
                }
                Dr.Close();
                Conn.Close();

                //COLS
                dataGridView1.ColumnCount = Convert.ToInt32(tableCols.Length);
                dataGridView1.AllowUserToAddRows = false;
                for (int i = 0; i < Convert.ToInt32(tableCols.Length); i++)
                    dataGridView1.Columns[i].Name = tableCols[i];

                //ROWS
                switch (cbRef.Text)
                {
                    case "Delivery Order":
                        Query = "Select * from DeliveryOrderD where DeliveryOrderId = '" + tbxRefID.Text + "'";
                        break;
                    case "Nota Transfer":
                        Query = "select * from NotaTransferD where TransferNo = '" + tbxRefID.Text + "'";
                        break;
                    case "Nota Retur Beli":
                        Query = "select * from NotaReturBeli_Dtl where NRBId = '" + tbxRefID.Text + "'";
                        break;
                    case "Nota Retur Jual":
                        Query = "select * from NotaReturJual_Dtl where NRJId = '" + tbxRefID.Text + "'";
                        break;
                }
                GenerateGVRows(Query);
                ModeNew();
            }
            else if (Mode != "New")
            {
                Conn = ConnectionString.GetConnection();
                Query = "select * from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtBBK.Value = (DateTime)Dr["GoodsIssuedDate"];
                    tbxSJ.Text = Dr["No_SJ"].ToString();
                    cbRef.Text = Dr["RefTransType"].ToString();
                    tbxRefID.Text = Dr["RefTransID"].ToString();
                    dtRef.Value = (DateTime)Dr["RefTransDate"];
                    tbxNameID.Text = Dr["AccountNum"].ToString();
                    tbxName.Text = Dr["AccountName"].ToString();
                    tbxVOwner.Text = Dr["VehicleOwnerName"].ToString();
                    tbxVOwnerID.Text = Dr["VehicleOwnerID"].ToString();
                    tbxVType.Text = Dr["VehicleType"].ToString();
                    tbxVNumber.Text = Dr["VehicleNumber"].ToString();
                    tbxDriverName.Text = Dr["DriverName"].ToString();
                    tbxInventSiteID.Text = Dr["InventSiteID"].ToString();
                    tbxNotes.Text = Dr["Notes"].ToString();
                    dtWeight1.Value = (DateTime)Dr["Timbang1Date"];
                    if (Dr["StatusWeight1"].ToString() == "Manual")
                        cbWeight1.Checked = true;
                    else
                        cbWeight1.Checked = false;
                    tbxWeight1.Text = Dr["Timbang1Weight"].ToString();
                    if (Dr["Timbang2Date"].ToString() == String.Empty)
                        dtWeight2.Value = DateTime.Now;
                    else
                        dtWeight2.Value = (DateTime)Dr["Timbang2Date"];
                    if (Dr["StatusWeight2"].ToString() == "Manual")
                        cbWeight2.Checked = true;
                    else
                        cbWeight2.Checked = false;
                    if (Dr["Timbang2Weight"] == null || Dr["Timbang2Weight"].ToString() == String.Empty)
                        tbxWeight2.Text = "0.0000";
                    else
                        tbxWeight2.Text = Dr["Timbang2Weight"].ToString();
                }
                Dr.Close();
                Conn.Close();

                Conn = ConnectionString.GetConnection();
                Query = "select InventSiteName, Lokasi from InventSite where InventSiteID = '" + tbxInventSiteID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    tbxWarehouse.Text = Dr["InventSiteName"].ToString();
                    tbxLocation.Text = Dr["Lokasi"].ToString();
                }
                Dr.Close();
                Conn.Close();

                //COLS
                dataGridView1.ColumnCount = tableCols.Length;
                dataGridView1.AllowUserToAddRows = false;
                for (int i = 0; i < tableCols.Length; i++)
                    dataGridView1.Columns[i].Name = tableCols[i];

                dataGridView1.Rows.Clear();

                Conn = ConnectionString.GetConnection();
                Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                string status = Cmd.ExecuteScalar().ToString();

                Query = "select * from GoodsIssuedD where GoodsIssuedId = '" + tbxBBKID.Text + "' order by GoodsIssuedSeqNo asc";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int x = 0;
                while (Dr.Read())
                {
                    dataGridView1.Rows.Add(1);
                    for (int i = 0; i < tableCols.Length; i++)
                    {
                        if (!(tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt"))
                        {
                            if (i == 0)
                                dataGridView1.Rows[x].Cells[tableCols[i]].Value = dataGridView1.RowCount;
                            else if (tableCols[i] == "SeqNo")
                                dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr["RefTransSeqNo"];
                            else if (tableCols[i] == "InventSiteBlokID" && Mode == "Edit" && ControlMgr.GroupName == "WB OPERATOR")
                            {
                                if (ControlMgr.GroupName == "WB OPERATOR" && (status == "02" || status == "03"))
                                {
                                    Query = "Select [InventSiteBlokID] from [dbo].[InventSiteBlok] where InventSiteID = '" + tbxInventSiteID.Text + "'";
                                    Cmd = new SqlCommand(Query, Conn);
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                                }
                                else
                                {
                                    cellValue("Select [InventSiteBlokID] from [dbo].[InventSiteBlok] where InventSiteID = '" + tbxInventSiteID.Text + "'");
                                    if (Dr["InventSiteBlokID"] != null)
                                        cell.Value = Dr["InventSiteBlokID"];
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]] = cell;
                                }
                            }
                            else if (tableCols[i] == "ActionCode")
                            {
                                if ((Mode != "Edit" && ((ControlMgr.GroupName == "SITE MANAGER" || ControlMgr.GroupName == "KERANI" || ControlMgr.GroupName == "WB OPERATOR") && (status == "02" || status == "03" || status == "05" || status == "06"))) || (Mode == "Edit" && ControlMgr.GroupName == "WB OPERATOR" && status == "02"))
                                {
                                    if (Dr["ActionCode"].ToString() != String.Empty)
                                    {
                                        Query = "select [Deskripsi] from [dbo].[TransStatusTable] where TransCode = 'GID' and StatusCode = '" + Dr["ActionCode"].ToString() + "'";
                                        Cmd = new SqlCommand(Query, Conn);
                                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                                    }
                                    else
                                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]].Value = String.Empty;
                                }
                                else if (Mode != "Edit" && Dr["ActionCode"].ToString() == String.Empty)
                                    dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                                else if (Mode == "Edit")
                                {
                                    cellValue("Select [Deskripsi] from [dbo].[TransStatusTable] where TransCode = 'GID'");
                                    if (Dr["ActionCode"] != null)
                                    {
                                        Cmd = new SqlCommand("select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'", Conn);
                                        if (Dr["ActionCode"].ToString() != String.Empty)
                                        {
                                            Query = "select [Deskripsi] from [dbo].[TransStatusTable] where TransCode = 'GID' and StatusCode = '" + Dr["ActionCode"].ToString() + "'";
                                            Cmd = new SqlCommand(Query, Conn);
                                            cell.Value = Cmd.ExecuteScalar().ToString();
                                        }
                                        else
                                            cell.Value = "Select";
                                    }
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]] = cell;
                                }
                            }
                            else
                                dataGridView1.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                        }
                    }
                    x++;
                }
                Dr.Close();
                Conn.Close();
                ModeBeforeEdit();
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

        private void ModeNew()
        {
            dtBBK.Value = DateTime.Now;
            dtWeight1.Value = DateTime.Now;

            if (cbRef.Text == "Select")
                btnSRefID.Enabled = false;
            else
                btnSRefID.Enabled = true;

            btnPrint.Visible = false; rbTicket.Visible = false; rbSuratJalan.Visible = false;
            btnEdit.Enabled = false; btnCancel.Enabled = false;

            if (ControlMgr.GroupName == "Sales Admin")
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    //HIDE COLS
                    if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "ActionCode")
                        dataGridView1.Columns[i].Visible = false;

                    //READONLY AND COLOR
                    if (tableCols[i] == "Qty" || tableCols[i] == "Qty_Actual" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "InventSiteBlokID" || tableCols[i] == "ActionCode" || tableCols[i] == "Notes")
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
            else
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    //HIDE COLS
                    if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Actual" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "ActionCode")
                        dataGridView1.Columns[i].Visible = false;

                    //READONLY AND COLOR
                    if (tableCols[i] == "Qty" || tableCols[i] == "Qty_Actual" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "InventSiteBlokID" || tableCols[i] == "ActionCode" || tableCols[i] == "Notes")
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
            if (dataGridView1.ColumnCount != 0)
            {
                btnAdd.Enabled = true; btnDelete.Enabled = true; btnNT.Enabled = true;
            }
        }

        private void ModeEdit()
        {
            btnSOwner.Enabled = true;
            tbxVType.Enabled = true; tbxVNumber.Enabled = true; tbxDriverName.Enabled = true;
            tbxNotes.Enabled = true;

            if (cbWeight1.Checked == true)
            {
                cbWeight1.Enabled = true;
                btnWeight1.Enabled = false;
                tbxWeight1.Enabled = true;
            }
            else
            {
                cbWeight1.Enabled = false;
                btnWeight1.Enabled = true;
                tbxWeight1.Enabled = false;
            }
            if (cbWeight2.Checked == true)
            {
                cbWeight2.Enabled = true;
                btnWeight2.Enabled = false;
                tbxWeight2.Enabled = true;
            }
            else
            {
                cbWeight2.Enabled = true;
                btnWeight2.Enabled = true;
                tbxWeight2.Enabled = false;
            }

            btnPrint.Enabled = false; rbTicket.Enabled = false; rbSuratJalan.Enabled = false;
            btnSave.Enabled = true; btnCancel.Enabled = true; btnExit.Enabled = false; btnEdit.Enabled = false;

            Conn = ConnectionString.GetConnection();
            Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            if (Mode == "New" || (Cmd.ExecuteScalar().ToString() == "01" && ControlMgr.GroupName == "WB OPERATOR"))
            {
                btnAdd.Enabled = true; btnDelete.Enabled = true;
            }
            if (Cmd.ExecuteScalar().ToString() == "01")
            {
                if (ControlMgr.GroupName == "KERANI")
                {
                    btnSOwner.Enabled = false;
                    tbxVType.Enabled = false; tbxVNumber.Enabled = false; tbxDriverName.Enabled = false;
                    tbxNotes.Enabled = false;
                    tbxWeight1.Enabled = false; cbWeight1.Enabled = false; btnWeight1.Enabled = false;
                }
            }
            else if (Cmd.ExecuteScalar().ToString() == "02")
            {
                btnSOwner.Enabled = false;
                tbxVType.Enabled = false; tbxVNumber.Enabled = false; tbxDriverName.Enabled = false;
                tbxNotes.Enabled = false;
                tbxWeight1.Enabled = false; cbWeight1.Enabled = false;
            }
            else if (Cmd.ExecuteScalar().ToString() == "03")
            {
                btnSOwner.Enabled = false;
                tbxVType.Enabled = false; tbxVNumber.Enabled = false; tbxDriverName.Enabled = false;
                tbxNotes.Enabled = false;
                tbxWeight1.Enabled = false; cbWeight1.Enabled = false; btnWeight1.Enabled = false;
            }

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (Cmd.ExecuteScalar().ToString() == "01")
                {
                    if (ControlMgr.GroupName == "WB OPERATOR")
                    {
                        //READONLY AND COLOR
                        if (tableCols[i] == "Qty" || tableCols[i] == "Qty_Actual" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "InventSiteBlokID" || tableCols[i] == "ActionCode" || tableCols[i] == "Notes")
                        {
                            dataGridView1.Columns[i].ReadOnly = false;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        }
                        else
                        {
                            dataGridView1.Columns[i].ReadOnly = true;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        //HIDE COLS
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Actual" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "ActionCode")
                            dataGridView1.Columns[i].Visible = false;
                    }
                    else if (ControlMgr.GroupName == "KERANI")
                    {
                        //READONLY AND COLOR
                        if (tableCols[i] == "Qty_Actual" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "ActionCode" || tableCols[i] == "Notes")
                        {
                            dataGridView1.Columns[i].ReadOnly = false;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
                else if (Cmd.ExecuteScalar().ToString() == "02")
                {
                    if (ControlMgr.GroupName == "WB OPERATOR")
                    {
                        dataGridView1.Columns[i].ReadOnly = true;
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else if (ControlMgr.GroupName == "KERANI")
                    {
                        //READONLY AND COLOR
                        if (tableCols[i] == "Qty_Actual" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "ActionCode" || tableCols[i] == "Notes")
                        {
                            dataGridView1.Columns[i].ReadOnly = false;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
            }
            Conn.Close();
        }

        private void ModeBeforeEdit()
        {
            if (cbRef.Text == "Select")
                tabPage2.Text = "Detail Ref";
            else if (cbRef.Text == "Delivery Order")
                tabPage2.Text = "Detail Delivery Order";
            else if (cbRef.Text == "Nota Transfer")
                tabPage2.Text = "Detail Nota Transfer";
            else if (cbRef.Text == "Nota Retur Beli")
                tabPage2.Text = "Detail Nota Retur Beli";
            //tia edit
            tbxNameID.Enabled = true;
            tbxNameID.ReadOnly = true;
            tbxName.Enabled = true;
            tbxName.ReadOnly = true;
            tbxRefID.Enabled = true;

            tbxRefID.ReadOnly = true;
            tbxVOwner.Enabled = true;
            tbxVOwner.ReadOnly = true;
            tbxVOwnerID.Enabled = true;
            tbxVOwnerID.ReadOnly = true;

            tbxNameID.ContextMenu = vendid;
            tbxName.ContextMenu = vendid;
            tbxRefID.ContextMenu = vendid;
            tbxVOwner.ContextMenu = vendid;
            tbxVOwnerID.ContextMenu = vendid;
            //end

            cbRef.Enabled = false; btnSRefID.Enabled = false;
            btnSOwner.Enabled = false;
            tbxVType.Enabled = false; tbxVNumber.Enabled = false; tbxDriverName.Enabled = false;
            tbxNotes.Enabled = false;

            btnAdd.Enabled = false; btnDelete.Enabled = false; btnNT.Enabled = false;

            tbxWeight1.Enabled = false; cbWeight1.Enabled = false; btnWeight1.Enabled = false;
            tbxWeight2.Enabled = false; cbWeight2.Enabled = false; btnWeight2.Enabled = false;

            btnPrint.Enabled = true; rbTicket.Enabled = true; rbSuratJalan.Enabled = true;

            btnSave.Enabled = false; btnCancel.Enabled = false; btnExit.Enabled = true; btnEdit.Enabled = true;

            Conn = ConnectionString.GetConnection();
            Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string status = Cmd.ExecuteScalar().ToString();
            if (status == "01")
            {
                if (ControlMgr.GroupName == "WB OPERATOR")
                {
                    rbTicket.Checked = true;
                    btnPrint.Visible = true; rbTicket.Visible = true; rbSuratJalan.Visible = false;
                }
                else if (ControlMgr.GroupName == "KERANI")
                {
                    btnPrint.Visible = false; rbTicket.Visible = false; rbSuratJalan.Visible = false;
                }
            }
            else if (status == "02")
            {
                if (ControlMgr.GroupName == "WB OPERATOR")
                {
                    //rbTT.Checked = true;
                    btnPrint.Visible = false; rbTicket.Visible = false; rbSuratJalan.Visible = false;
                    gbWeight2.Visible = true;
                }
                else if (ControlMgr.GroupName == "KERANI")
                {
                    btnPrint.Visible = false; rbTicket.Visible = false; rbSuratJalan.Visible = false;
                    gbWeight2.Visible = false;
                }

                //GENERATE SJ NUM
                Conn = ConnectionString.GetConnection();
                Query = "Select max(No_SJ) from GoodsIssuedH where No_SJ LIKE '%";
                if (cbRef.Text == "Delivery Order")
                    Query += "SJD-%'";
                else if (cbRef.Text == "Nota Transfer")
                    Query += "SJT-%'";
                else if (cbRef.Text == "Nota Retur Beli" || cbRef.Text == "Nota Retur Jual")
                    Query += "SJR-%'";
                Cmd = new SqlCommand(Query, Conn);

                string SJnum = "";
                if (cbRef.Text == "Delivery Order")
                    SJnum = "SJD-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-";
                else if (cbRef.Text == "Nota Transfer")
                {
                    SJnum = "SJT-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-";
                }
                else if (cbRef.Text == "Nota Retur Beli" || cbRef.Text == "Nota Retur Jual")
                {
                    SJnum = "SJR-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-";
                }

                int count;
                if (Cmd.ExecuteScalar().ToString() == String.Empty)
                    count = 1;
                else
                    count = Convert.ToInt32(Cmd.ExecuteScalar().ToString().Split('-')[2]) + 1;

                if (count.ToString().Length == 1)
                    SJnum += "0000" + count;
                else if (count.ToString().Length == 2)
                    SJnum += "000" + count;
                else if (count.ToString().Length == 3)
                    SJnum += "00" + count;
                else if (count.ToString().Length == 4)
                    SJnum += "0" + count;
                else if (count.ToString().Length == 5)
                    SJnum += "" + count;

                tbxSJ.Text = SJnum;
            }
            else if (status == "03" || status == "06")
            {
                lblSJID.Visible = true; tbxSJ.Visible = true;
                rbSuratJalan.Checked = true;
                gbWeight2.Visible = true;
                if (ControlMgr.GroupName == "KERANI")
                {
                    btnPrint.Enabled = false; rbTicket.Enabled = false; rbSuratJalan.Enabled = false;
                    btnEdit.Enabled = false;
                }
                else if (ControlMgr.GroupName == "WB OPERATOR")
                {
                    rbSuratJalan.Checked = true; rbTicket.Enabled = false;
                    btnPrint.Visible = true; rbSuratJalan.Visible = true; rbTicket.Visible = true;
                    //tbxWeight1.Enabled = false; cbWeight1.Enabled = false; btnWeight1.Enabled = false;
                    btnEdit.Enabled = false;
                    btnApprove.Enabled = false;
                }
            }
            else if (status == "05")
            {
                gbWeight2.Visible = true;
                btnApprove.Visible = true;
                btnEdit.Enabled = false;
                btnPrint.Enabled = false; rbSuratJalan.Visible = false; rbTicket.Visible = false;
            }

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                //HIDE COLS
                if (status == "01")
                {
                    if (ControlMgr.GroupName == "WB OPERATOR")
                    {
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Actual" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "ActionCode")
                            dataGridView1.Columns[i].Visible = false;
                    }
                    else if (ControlMgr.GroupName == "KERANI")
                    {
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                            dataGridView1.Columns[i].Visible = false;
                    }
                }
                else if (status == "02")
                {
                    if (ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI")
                    {
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                            dataGridView1.Columns[i].Visible = false;
                    }
                    //else if (ControlMgr.GroupName == "PurchaseManager")
                    //{
                    //    if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                    //        dataGridView1.Columns[i].Visible = false;
                    //}
                }
                else if (status == "03" || status == "05" || status == "06")
                {
                    if (ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI" || ControlMgr.GroupName == "SITE MANAGER")
                    {
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                            dataGridView1.Columns[i].Visible = false;
                    }
                }

                if (ControlMgr.GroupName == "Administrator")
                {
                    if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                        dataGridView1.Columns[i].Visible = false;
                }
                dataGridView1.Columns[i].ReadOnly = true;
                dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;

                if (ControlMgr.GroupName == "Sales Admin")
                {
                    if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                        dataGridView1.Columns[i].Visible = false;

                    btnEdit.Enabled = false;
                }
            }
            Conn.Close();
        }
        //tia edit
        private void ModePopUp()
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }

            if (cbRef.Text == "Select")
                tabPage2.Text = "Detail Ref";
            else if (cbRef.Text == "Delivery Order")
                tabPage2.Text = "Detail Delivery Order";
            else if (cbRef.Text == "Nota Transfer")
                tabPage2.Text = "Detail Nota Transfer";
            else if (cbRef.Text == "Nota Retur Beli")
                tabPage2.Text = "Detail Nota Retur Beli";
            //tia edit
            tbxNameID.Enabled = true;
            tbxNameID.ReadOnly = true;
            tbxName.Enabled = true;
            tbxName.ReadOnly = true;
            tbxRefID.Enabled = true;

            tbxRefID.ReadOnly = true;
            tbxVOwner.Enabled = true;
            tbxVOwner.ReadOnly = true;
            tbxVOwnerID.Enabled = true;
            tbxVOwnerID.ReadOnly = true;

            tbxNameID.ContextMenu = vendid;
            tbxName.ContextMenu = vendid;
            tbxRefID.ContextMenu = vendid;
            tbxVOwner.ContextMenu = vendid;
            tbxVOwnerID.ContextMenu = vendid;

            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnSave.Visible = false;
            btnApprove.Visible = false;
            btnPrint.Visible = false;
            rbTicket.Visible = false;
            rbSuratJalan.Visible = false;

            cbRef.Enabled = false; btnSRefID.Enabled = false;
            btnSOwner.Enabled = false;
            tbxVType.Enabled = false; tbxVNumber.Enabled = false; tbxDriverName.Enabled = false;
            tbxNotes.Enabled = false;

            btnAdd.Enabled = false; btnDelete.Enabled = false; btnNT.Enabled = false;

            tbxWeight1.Enabled = false; cbWeight1.Enabled = false; btnWeight1.Enabled = false;
            tbxWeight2.Enabled = false; cbWeight2.Enabled = false; btnWeight2.Enabled = false;

            btnPrint.Enabled = true; rbTicket.Enabled = true; rbSuratJalan.Enabled = true;

            btnSave.Enabled = false; btnCancel.Enabled = false; btnExit.Enabled = true; btnEdit.Enabled = true;

            Conn = ConnectionString.GetConnection();
            Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string status = Cmd.ExecuteScalar().ToString();
            if (status == "01")
            {
                if (ControlMgr.GroupName == "WB OPERATOR")
                {
                    rbTicket.Checked = false;
                    btnPrint.Visible = false; rbTicket.Visible = true; rbSuratJalan.Visible = false;
                }
                else if (ControlMgr.GroupName == "KERANI")
                {
                    btnPrint.Visible = false; rbTicket.Visible = false; rbSuratJalan.Visible = false;
                }
            }
            else if (status == "02")
            {
                if (ControlMgr.GroupName == "WB OPERATOR")
                {
                    //rbTT.Checked = true;
                    btnPrint.Visible = false; rbTicket.Visible = false; rbSuratJalan.Visible = false;
                    gbWeight2.Visible = false;
                }
                else if (ControlMgr.GroupName == "KERANI")
                {
                    btnPrint.Visible = false; rbTicket.Visible = false; rbSuratJalan.Visible = false;
                    gbWeight2.Visible = false;
                }

                //GENERATE SJ NUM
                Conn = ConnectionString.GetConnection();
                Query = "Select max(No_SJ) from GoodsIssuedH where No_SJ LIKE '%";
                if (cbRef.Text == "Delivery Order")
                    Query += "SJD-%'";
                else if (cbRef.Text == "Nota Transfer")
                    Query += "SJT-%'";
                else if (cbRef.Text == "Nota Retur Beli")
                    Query += "SJR-%'";
                Cmd = new SqlCommand(Query, Conn);

                string SJnum = "";
                if (cbRef.Text == "Delivery Order")
                    SJnum = "SJD-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-";
                else if (cbRef.Text == "Nota Transfer")
                {
                    SJnum = "SJT-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-";
                }
                else if (cbRef.Text == "Nota Retur Beli")
                {
                    SJnum = "SJR-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-";
                }

                int count;
                if (Cmd.ExecuteScalar().ToString() == String.Empty)
                    count = 1;
                else
                    count = Convert.ToInt32(Cmd.ExecuteScalar().ToString().Split('-')[2]) + 1;

                if (count.ToString().Length == 1)
                    SJnum += "0000" + count;
                else if (count.ToString().Length == 2)
                    SJnum += "000" + count;
                else if (count.ToString().Length == 3)
                    SJnum += "00" + count;
                else if (count.ToString().Length == 4)
                    SJnum += "0" + count;
                else if (count.ToString().Length == 5)
                    SJnum += "" + count;

                tbxSJ.Text = SJnum;
            }
            else if (status == "03" || status == "06")
            {
                lblSJID.Visible = false; tbxSJ.Visible = false;
                rbSuratJalan.Checked = false;
                gbWeight2.Visible = false;
                if (ControlMgr.GroupName == "KERANI")
                {
                    btnPrint.Enabled = false; rbTicket.Enabled = false; rbSuratJalan.Enabled = false;
                    btnEdit.Enabled = false;
                }
                else if (ControlMgr.GroupName == "WB OPERATOR")
                {
                    rbSuratJalan.Checked = true; rbTicket.Enabled = false;
                    btnPrint.Visible = false; rbSuratJalan.Visible = false; rbTicket.Visible = false;
                    //tbxWeight1.Enabled = false; cbWeight1.Enabled = false; btnWeight1.Enabled = false;
                }
            }
            else if (status == "05")
            {
                gbWeight2.Visible = false;
                btnApprove.Visible = false;
                btnEdit.Enabled = false;
                btnPrint.Enabled = false; rbSuratJalan.Visible = false; rbTicket.Visible = false;
            }

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                //HIDE COLS
                if (status == "01")
                {
                    if (ControlMgr.GroupName == "WB OPERATOR")
                    {
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Actual" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt" || tableCols[i] == "TotalBerat_Actual" || tableCols[i] == "ActionCode")
                            dataGridView1.Columns[i].Visible = false;
                    }
                    else if (ControlMgr.GroupName == "KERANI")
                    {
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                            dataGridView1.Columns[i].Visible = false;
                    }
                }
                else if (status == "02")
                {
                    if (ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI")
                    {
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                            dataGridView1.Columns[i].Visible = false;
                    }
                    //else if (ControlMgr.GroupName == "PurchaseManager")
                    //{
                    //    if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                    //        dataGridView1.Columns[i].Visible = false;
                    //}
                }
                else if (status == "03" || status == "05" || status == "06")
                {
                    if (ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI" || ControlMgr.GroupName == "SITE MANAGER")
                    {
                        if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                            dataGridView1.Columns[i].Visible = false;
                    }
                }

                if (ControlMgr.GroupName == "Administrator")
                {
                    if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                        dataGridView1.Columns[i].Visible = false;
                }
                dataGridView1.Columns[i].ReadOnly = true;
                dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;

                if (ControlMgr.GroupName == "Sales Admin")
                {
                    if (tableCols[i] == "SeqNo" || tableCols[i] == "GoodsIssuedSeqNo" || tableCols[i] == "GroupID" || tableCols[i] == "SubGroup1ID" || tableCols[i] == "SubGroup2ID" || tableCols[i] == "ItemID" || tableCols[i] == "Qty_Alt" || tableCols[i] == "Unit_Alt")
                        dataGridView1.Columns[i].Visible = false;

                    btnEdit.Enabled = false;
                }
            }
            Conn.Close();
        }
        //tia edit end

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
            createLabel(tbxRefID, lSOID, gbMain, "string");
            createLabel(tbxVOwnerID, lVehicle, gbMain, "string");
            if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
            {
                createLabel(tbxVType, lVType, gbMain, "string");
                createLabel(tbxVNumber, lVNumber, gbMain, "string");
                createLabel(tbxDriverName, lDriverName, gbMain, "string");
            }
            createLabel(cbRef, lRefType, gbMain, "string");
            createLabel(tbxWeight1, lWeight1, gbWeight1, "decimal");
            if (tbxVOwnerID.Text == String.Empty)
                tbxVOwner.BackColor = Color.LightGoldenrodYellow;
            else
                tbxVOwner.BackColor = Color.Empty;

            string GIStats = "";
            if (Mode != "New")
            {
                Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                GIStats = Cmd.ExecuteScalar().ToString();
                if ((GIStats == "02" || GIStats == "03") && ControlMgr.GroupName == "WB OPERATOR")
                {
                    createLabel(tbxWeight2, lWeight2, gbWeight2, "decimal");
                }
            }

            if (flag == 'X')
                msg += "Field * mesti diisi!\r\n";

            if (dataGridView1.RowCount == 0 && tbxRefID.Text != String.Empty)
                msg += "Tolong Add Item(s)!\r\n";

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //QUANTITY
                if (Mode == "New" || (ControlMgr.GroupName == "WB OPERATOR" && GIStats == "01"))
                {
                    switch (cbRef.Text)
                    {
                        case "Delivery Order":
                            Query = "select RemainingQty from DeliveryOrderD where DeliveryOrderId = '" + tbxRefID.Text + "' and SeqNo = '" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                            break;
                        case "Nota Transfer":
                            Query = "select RemainingQty from NotaTransferD where TransferNo = '" + tbxRefID.Text + "' and SeqNo = '" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                            break;
                        case "Nota Retur Beli":
                            Query = "select RemainingQty from NotaReturBeli_Dtl where NRBId = '" + tbxRefID.Text + "' and SeqNo = '" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                            break;
                        case "Nota Retur Jual":
                            Query = "select [Remaining_Qty_DO] as 'RemainingQty' from NotaReturJual_Dtl where NRJId = '" + tbxRefID.Text + "' and SeqNo = '" + dataGridView1.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                            break;
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    decimal RemainingQty = Convert.ToDecimal(Cmd.ExecuteScalar());
                    //REMARKED BY: HC (S) LUPA UNTUK APA
                    //if (Cmd.ExecuteScalar() == null)
                    //{
                    //    Query = "Select RefTransSeqNo from GoodsIssuedD where RefTransID = '" + tbxRefID.Text + "' and GoodsIssuedSeqNo = '" + dataGridView1.Rows[i].Cells["GoodsIssuedSeqNo"].Value.ToString() + "'";
                    //    Cmd = new SqlCommand(Query, Conn);
                    //}
                    //REMARKED BY: HC (E) 
                    if (ControlMgr.GroupName == "WB OPERATOR" && GIStats == "01")
                    {
                        Query = "select Qty from GoodsIssuedD where GoodsIssuedId = '" + tbxBBKID.Text + "' and GoodsIssuedSeqNo = '" + dataGridView1.Rows[i].Cells["GoodsIssuedSeqNo"].Value + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal qty = Convert.ToDecimal(Cmd.ExecuteScalar());

                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) > RemainingQty + qty)
                        {
                            msg += "Baris " + Convert.ToInt32(i + 1) + " " + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + " tidak boleh lebih dari " + Convert.ToString(RemainingQty + qty) + "!\r\n";
                        }
                    }
                    else if (Mode == "New")
                    {
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) > Convert.ToDecimal(Cmd.ExecuteScalar()))
                        {
                            msg += "Baris " + Convert.ToInt32(i + 1) + " " + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + " tidak boleh lebih dari " + Cmd.ExecuteScalar().ToString() + "!\r\n";
                        }
                    }
                }

                //COMPARE QTY WITH QTY_ACTUAL
                if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value != null)
                {
                    if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() != String.Empty)
                    {
                        decimal qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                        decimal qty_actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                        if (qty < qty_actual)
                            msg += "Baris " + Convert.ToInt32(i + 1) + " qty actual tidak boleh lebih dari " + dataGridView1.Rows[i].Cells["Qty"].Value.ToString() + "!\r\n";
                    }
                }
                //InventSiteBlokID
                if (dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value.ToString() == "Select")
                    msg += "Baris " + Convert.ToInt32(i + 1) + " Tolong pilih InventSiteBlokID!\r\n";

                //ActionCode
                if (Mode != "New")
                {
                    if ((GIStats == "01" || GIStats == "02") && ControlMgr.GroupName == "KERANI")
                    {
                        if (dataGridView1.Rows[i].Cells["ActionCode"].Value.ToString() == "Select")
                            msg += "Baris " + Convert.ToInt32(i + 1) + " Tolong pilih ActionCode!\r\n";
                    }
                }
            }

            //CHECK COMBINE QUANTITY
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select DeliveryOrderId, SeqNo, FullItemID, ItemName, RemainingQty from DeliveryOrderD where DeliveryOrderId = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Transfer":
                    Query = "select TransferNo, SeqNo, FullItemID, ItemName, RemainingQty from NotaTransferD where TransferNo = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Retur Beli":
                    Query = "select NRBId, SeqNo, FullItemID, ItemName, RemainingQty from NotaReturBeli_Dtl where NRBId = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Retur Jual":
                    Query = "select NRJId, SeqNo, FullItemId, ItemName, [Remaining_Qty_DO] as 'RemainingQty' from NotaReturJual_Dtl where NRJId = '" + tbxRefID.Text + "'";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                decimal qty = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (Convert.ToInt32(Dr["SeqNo"]) == Convert.ToInt32(dataGridView1.Rows[i].Cells["SeqNo"].Value))
                    {
                        qty += Convert.ToDecimal(dataGridView1.Rows[i].Cells["qty"].Value);
                    }
                }
                decimal totalQty = 0;
                if (ControlMgr.GroupName == "WB OPERATOR" && GIStats == "01")
                {
                    Query = "select sum(qty) from GoodsIssuedD where GoodsIssuedId = '" + tbxBBKID.Text + "' and RefTransID = '" + Dr[0] + "' and RefTransSeqNo = '" + Dr[1] + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar() != (object)DBNull.Value)
                        totalQty = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                else if (Mode == "New")
                {
                    if (qty > (Decimal)Dr["RemainingQty"] + totalQty)
                    {
                        msg += "Quantity gabungan dari " + (String)Dr["FullItemID"] + " ( " + (String)Dr["ItemName"] + " ) tidak boleh lebih dari " + Convert.ToString(Convert.ToDecimal(Dr["RemainingQty"]) + totalQty) + "!\r\n";
                    }
                }
            }
            Dr.Close();

            if (msg != String.Empty)
            {
                MetroFramework.MetroMessageBox.Show(this, msg, "Fill the form!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                flag = 'X';
            }
            count = 0;
            validate = true;
            return flag;
        }

        private void btnSOwner_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "VendTable", "and Gol_Prsh = 'EXPEDISI'", "a.*", "VendTable a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                tbxVOwnerID.Text = SearchV2.data[0];
                Conn = ConnectionString.GetConnection();
                Query = "select VendName from VendTable where VendID = '" + tbxVOwnerID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                tbxVOwner.Text = Cmd.ExecuteScalar().ToString();
                Conn.Close();
            }
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
                Query = "select InventSiteName, Lokasi from InventSite where InventSiteID = '" + tbxInventSiteID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    tbxWarehouse.Text = Dr["InventSiteName"].ToString();
                    tbxLocation.Text = Dr["Lokasi"].ToString();
                }
                Dr.Close();
                Conn.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount != 0)
            {
                if (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["GoodsIssuedSeqNo"].Value == null)
                    dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                else
                    MetroFramework.MetroMessageBox.Show(this, "Tidak boleh delete item ini!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cbWeight1_CheckedChanged(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "select Timbang1Weight from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            if (Cmd.ExecuteScalar() == null)
                tbxWeight1.Text = "0.0000";
            else
                tbxWeight1.Text = Cmd.ExecuteScalar().ToString();
            if (cbWeight1.Checked == true)
            {
                tbxWeight1.Enabled = true;
                btnWeight1.Enabled = false;
            }
            else
            {
                tbxWeight1.Enabled = false;
                btnWeight1.Enabled = true;
            }
            Conn.Close();
        }

        ////BEGIN STEVEN EDIT
        //private void insertStatusLogSave(string GIId)
        //{
        //    //edited by Thaddaeus, 25 Sept 2018, begin=================
        //    if (cbRef.Text == "Nota Retur Beli")
        //    {
        //        string Status = "";
        //        Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + GIId + "'";
        //        using(Cmd = new SqlCommand(Query, Conn))
        //        {
        //            if (Cmd.ExecuteScalar() != System.DBNull.Value)
        //            {
        //                Status = Cmd.ExecuteScalar().ToString();
        //            }
        //        }

        //        string StatusDescription = "";
        //        Query = "SELECT [Deskripsi] FROM [TransStatusTable] WHERE [TransCode] = @TransCode AND [StatusCode]=@StatusCode";
        //        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
        //        {
        //            Cmd.Parameters.AddWithValue("@TransCode", "GI");
        //            Cmd.Parameters.AddWithValue("@StatusCode", Status);
        //            if (Cmd.ExecuteScalar() != System.DBNull.Value)
        //            {
        //                StatusDescription = Cmd.ExecuteScalar().ToString();
        //            }
        //        }
  
        //        Query = "INSERT INTO [StatusLog_Vendor] ([StatusLog_FormName],[Vendor_Id],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]) VALUES (@StatusLog_FormName,@Vendor_Id,@StatusLog_PK1,@StatusLog_PK2,@StatusLog_PK3,@StatusLog_PK4,@StatusLog_Status,@StatusLog_Description,@StatusLog_UserID,getdate())";
        //        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
        //        {
        //            Cmd.Parameters.AddWithValue("@StatusLog_FormName", "BBKHeader");
        //            Cmd.Parameters.AddWithValue("@Vendor_Id", tbxNameID.Text);
        //            Cmd.Parameters.AddWithValue("@StatusLog_PK1", GIId);
        //            Cmd.Parameters.AddWithValue("@StatusLog_PK2", "");
        //            Cmd.Parameters.AddWithValue("@StatusLog_PK3", "");
        //            Cmd.Parameters.AddWithValue("@StatusLog_PK4", "");
        //            Cmd.Parameters.AddWithValue("@StatusLog_Status", Status);
        //            Cmd.Parameters.AddWithValue("@StatusLog_Description", StatusDescription);
        //            Cmd.Parameters.AddWithValue("@StatusLog_UserID", ControlMgr.UserId);
        //            Cmd.ExecuteNonQuery();
        //        }
        //    }
        //    //end=====================================================

        //    ////Conn = ConnectionString.GetConnection();
        //    //Query = "INSERT INTO [dbo].[StatusLog_Customer] (Customer_Id,[StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
        //    //Query += " VALUES ('BBK Header','" + tbxBBKID.Text + "', '" + tbxBBKID.Text + "', 'PK2Test', 'PK3Test', 'PK4Test',";

        //    //ListMethod.StatusLogCustomer("BBKHeader", "GI", tbxNameID.Text, "", "", tbxBBKID.Text, "", "", "");

        //    //BY: HC (S) 30.04.18
        //    Cmd = new SqlCommand("select a.StatusCode, b.Deskripsi from GoodsIssuedH a left join TransStatusTable b on a.StatusCode = b.StatusCode where b.TransCode = 'GI' and GoodsIssuedId = '" + tbxBBKID.Text + "'", Conn);
        //    Dr = Cmd.ExecuteReader();
        //    while (Dr.Read())
        //    {
        //        Query += "'" + Dr[0] + "', ";
        //        Query += "'" + Dr[1] + "'";
        //    }
        //    Dr.Close();
        //    //BY: HC (E)
        //    //REMARKED BY: HC (S) 30.04.18
        //    //Cmd = new SqlCommand("Select [StatusCode] from [ISBS-NEW4].[dbo].[GoodsIssuedH] where [GoodsIssuedId] = '" + tbxBBKID.Text + "'", Conn);
        //    //if (Cmd.ExecuteScalar().ToString() == "02")
        //    //{
        //    //    Query += "'02' ,'Loading Item'";
        //    //}
        //    //else if (Cmd.ExecuteScalar().ToString() == "01")
        //    //{
        //    //    Query += "'01', 'Load Complete'";
        //    //}
        //    //else if (Cmd.ExecuteScalar().ToString() == "03")
        //    //{
        //    //    Query += "'03', 'Completed'";
        //    //}
        //    //else
        //    //{
        //    //    Query += "'??', '????'";
        //    //}
        //    //Query += "'" + StatusCode + "' ,'Test'";
        //    //REMARKED BY: HC (E)
        //    Query += ",'" + ControlMgr.UserId + "' , GetDate())";
        //    SqlCommand Cmd2 = new SqlCommand(Query, Conn);
        //    Cmd2.ExecuteNonQuery();
        //}
        ////END STEVEN EDIT

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    if (Validation() != 'X')
                    {
                        #region variable
                        DateTime GoodsIssuedDate = dtBBK.Value;
                        string No_SJ = tbxSJ.Text;
                        string AccountNum = tbxNameID.Text;
                        string AccountName = tbxName.Text;
                        string RefTransType = cbRef.Text;
                        string RefTransID = tbxRefID.Text;
                        DateTime RefTransDate = dtRef.Value;
                        string InventSiteID = tbxInventSiteID.Text;
                        string VehicleOwnerID = tbxVOwnerID.Text;
                        string VehicleOwnerName = tbxVOwner.Text;
                        string VehicleType = tbxVType.Text.Trim();
                        string VehicleNumber = tbxVNumber.Text.Trim();
                        string DriverName = tbxDriverName.Text.Trim();
                        string StatusWeight1 = "";
                        if (cbWeight1.Checked == true)
                            StatusWeight1 = "Manual";
                        else
                            StatusWeight1 = "Auto";
                        DateTime Timbang1Date = dtWeight1.Value;
                        decimal Timbang1Weight = Convert.ToDecimal(tbxWeight1.Text);
                        string Notes = tbxNotes.Text.Trim();
                        string StatusWeight2 = "";
                        if (cbWeight2.Checked == true)
                            StatusWeight2 = "Manual";
                        else
                            StatusWeight2 = "Auto";
                        DateTime Timbang2Date = dtWeight2.Value;
                        decimal Timbang2Weight = 0;
                        if (tbxWeight2.Text == String.Empty)
                            Timbang2Weight = 0;
                        else
                            Timbang2Weight = Convert.ToDecimal(tbxWeight2.Text);

                        //SET STATUSCODE
                        string StatusCode = "";
                        Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        if (ControlMgr.GroupName == "KERANI" && (Cmd.ExecuteScalar().ToString() == "01" || Cmd.ExecuteScalar().ToString() == "02"))
                            StatusCode = "02";
                        else if ((ControlMgr.GroupName == "Administrator" || ControlMgr.GroupName == "WB OPERATOR") && (Cmd.ExecuteScalar() == null || Cmd.ExecuteScalar().ToString() == String.Empty || Cmd.ExecuteScalar().ToString() == "01"))
                            StatusCode = "01";
                        else if (ControlMgr.GroupName == "WB OPERATOR" && (Cmd.ExecuteScalar().ToString() == "02" || Cmd.ExecuteScalar().ToString() == "03"))
                        {
                            decimal GRatioActual = 0;
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                GRatioActual += Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value);
                            }
                            if (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual == 0)
                                StatusCode = "03";
                            else
                            {
                                decimal weightTolerance = (Convert.ToDecimal(tbxWeight2.Text) - (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual)) / (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual) * 100;
                                //hendry
                                //decimal weightTolerance = (((Convert.ToDecimal(tbxWeight2.Text) - GRatioActual) - Convert.ToDecimal(tbxWeight1.Text)) / Convert.ToDecimal(tbxWeight1.Text)) * 100; //REMARK BY: HC 6.8.18
                                if (Math.Abs(weightTolerance) > 1)
                                    StatusCode = "05";
                                else
                                    StatusCode = "03";
                            }
                        }
                        else if (ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                        {
                            StatusCode = "03";
                        }
                        #endregion
                        if (Mode == "New")
                        {
                            //Old Code============================================================
                            //GoodsIssuedId = ConnectionString.GenerateID("GoodsIssuedId", "GoodsIssuedH", "BBK");
                            //End Old Code========================================================

                            //begin===============================================================
                            //updated by : joshua
                            //updated date : 14 Feb 2018
                            //description : change generate sequence number, get from global function and update counter 
                            //Conn = ConnectionString.GetConnection();
                            string Jenis = "", Kode = "";

                            if (cbRef.Text == "Delivery Order")
                            {
                                Jenis = "GI";
                                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                                {
                                    Kode = "BBKV";
                                }
                                else
                                {
                                    Kode = "BBK";
                                }
                            }
                            else if (cbRef.Text == "Nota Transfer")
                            {
                                Jenis = "GI";
                                Kode = "BBKT";
                            }
                            else if (cbRef.Text == "Nota Retur Beli" )
                            {
                                Jenis = "GI";
                                Kode = "BBKR";
                            }
                            else if (cbRef.Text == "Nota Retur Jual")
                            {
                                Jenis = "GI";
                                Kode = "BBKR";
                            }
                            string GoodsIssuedId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                            tbxBBKID.Text = GoodsIssuedId;

                            //update counter
                            //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                            //end update counter
                            //end===================================================================

                            insertGoodsIssuedH(GoodsIssuedDate, GoodsIssuedId, StatusCode, AccountNum, AccountName, RefTransType, RefTransID, RefTransDate, InventSiteID, VehicleOwnerID, VehicleOwnerName, VehicleType, VehicleNumber, DriverName, StatusWeight1, Timbang1Date, Timbang1Weight, Notes);
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                #region variable
                                int GoodsIssuedSeqNo = Convert.ToInt32(i + 1);
                                int RefTransSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["SeqNo"].Value);
                                string GroupId = dataGridView1.Rows[i].Cells["GroupId"].Value.ToString();
                                string SubGroup1Id = dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString();
                                string SubGroup2Id = dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString();
                                string ItemId = dataGridView1.Rows[i].Cells["ItemId"].Value.ToString();
                                string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                                string ItemName = dataGridView1.Rows[i].Cells["ItemName"].Value.ToString();
                                decimal Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                                string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();
                                decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value.ToString() == "" ? 1 : dataGridView1.Rows[i].Cells["Ratio"].Value);
                                decimal TotalBerat = Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat"].Value);
                                string InventSiteBlokID = dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value.ToString();
                                if (dataGridView1.Rows[i].Cells["Notes"].Value == null)
                                    dataGridView1.Rows[i].Cells["Notes"].Value = "";
                                string NotesD = dataGridView1.Rows[i].Cells["Notes"].Value.ToString().Trim();
                                #endregion
                                updateRefRemainingQty(RefTransID, RefTransSeqNo, Qty, GoodsIssuedId, GoodsIssuedSeqNo, FullItemId, i);

                                insertGoodsIssuedD(GoodsIssuedDate, GoodsIssuedId, GoodsIssuedSeqNo, RefTransID, RefTransSeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Unit, Ratio, TotalBerat, InventSiteID, InventSiteBlokID, NotesD);

                                if (cbRef.Text == "Delivery Order")
                                {
                                    insertDOIssuedLogTable(RefTransID, RefTransSeqNo, Qty, Qty * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                }
                                else if (cbRef.Text == "Nota Transfer")
                                {
                                    insertNotaTransferLogTable(RefTransID, RefTransSeqNo, Qty, Qty * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                }
                                else if (cbRef.Text == "Nota Retur Beli")
                                {
                                    insertNotaReturBeliLogTable(RefTransID, RefTransSeqNo, Qty, Qty * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                }
                                else if (cbRef.Text == "Nota Retur Jual")
                                {
                                    insertNotaReturJualLogTable(RefTransID, RefTransSeqNo, Qty, Qty * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                }
                                insertGoodsIssuedLogTable(GoodsIssuedDate, GoodsIssuedId, AccountNum, GoodsIssuedSeqNo, InventSiteID, Qty, Qty * Ratio, RefTransID, RefTransDate, RefTransSeqNo, FullItemId, StatusCode);
                            }
                            updateRefStatus(RefTransID);
                            //BEGIN STEVEN EDIT
                            //insertStatusLogSave(GoodsIssuedId);
                            ListMethod.StatusLogCustomer("BBKHeader", "GI", tbxNameID.Text, StatusCode, "", tbxBBKID.Text, "", "", "");

                            //END STEVEN EDIT
                            if (ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                            {
                                Mode = "Edit";
                            }
                            else
                            {
                                Mode = "New";
                                MetroFramework.MetroMessageBox.Show(this, tbxBBKID.Text + " Save berhasil!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        if (Mode != "New")
                        {
                            //CreateJournal();

                            string GoodsIssuedId = tbxBBKID.Text;

                            RemainingReserved = new decimal[dataGridView1.Rows.Count]; // created by Thaddaeus, 23MAY2018
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                #region variable
                                int GoodsIssuedSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["GoodsIssuedSeqNo"].Value);
                                int RefTransSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["SeqNo"].Value);
                                string fullitemid = dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString();
                                decimal Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                                decimal Qty_Actual = 0;
                                if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
                                    Qty_Actual = 0;
                                else
                                    Qty_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                                #endregion
                                if (ControlMgr.GroupName == "WB OPERATOR")
                                    updateRefRemainingQty(RefTransID, RefTransSeqNo, Qty, GoodsIssuedId, GoodsIssuedSeqNo, fullitemid, i);
                                else
                                    updateRefRemainingQty(RefTransID, RefTransSeqNo, Qty_Actual, GoodsIssuedId, GoodsIssuedSeqNo, fullitemid, i);
                            }

                            updateGoodsIssuesH(StatusCode, InventSiteID, VehicleOwnerID, VehicleOwnerName, VehicleType, VehicleNumber, DriverName, StatusWeight1, Timbang1Date, Timbang1Weight, Notes, GoodsIssuedId, StatusWeight2, Timbang2Date, Timbang2Weight, No_SJ);

                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                #region variable
                                int GoodsIssuedSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["GoodsIssuedSeqNo"].Value);
                                int RefTransSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["SeqNo"].Value);
                                string GroupId = dataGridView1.Rows[i].Cells["GroupId"].Value.ToString();
                                string SubGroup1Id = dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString();
                                string SubGroup2Id = dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString();
                                string ItemId = dataGridView1.Rows[i].Cells["ItemId"].Value.ToString();
                                string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                                string ItemName = dataGridView1.Rows[i].Cells["ItemName"].Value.ToString();
                                decimal Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                                string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();

                                decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value == null ? 1 : dataGridView1.Rows[i].Cells["Ratio"].Value);
                                decimal TotalBerat = Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat"].Value);
                                string InventSiteBlokID = dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value.ToString();
                                if (dataGridView1.Rows[i].Cells["Notes"].Value == null)
                                    dataGridView1.Rows[i].Cells["Notes"].Value = "";
                                string NotesD = dataGridView1.Rows[i].Cells["Notes"].Value.ToString().Trim();
                                decimal Qty_Actual = 0;
                                if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
                                    Qty_Actual = 0;
                                else
                                    Qty_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                                decimal TotalBerat_Actual = 0;
                                if (dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() == String.Empty)
                                    TotalBerat_Actual = 0;
                                else
                                    TotalBerat_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value);

                                string ActionCode = "";
                                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && ControlMgr.GroupName == "Sales Admin")
                                {
                                    ActionCode = "Load Complete";
                                }
                                else
                                {
                                    ActionCode = dataGridView1.Rows[i].Cells["ActionCode"].Value.ToString();
                                }
                                #endregion

                                updateGoodsIssuesD(GoodsIssuedDate, No_SJ, RefTransID, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Unit, Ratio, TotalBerat, InventSiteID, InventSiteBlokID, NotesD, GoodsIssuedId, GoodsIssuedSeqNo, Qty_Actual, TotalBerat_Actual, ActionCode, i);

                                Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + GoodsIssuedId + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                string GIStats = Cmd.ExecuteScalar().ToString();
                                if ((ControlMgr.GroupName == "WB OPERATOR" && GIStats == "03") || (ControlMgr.GroupName.ToUpper() == "SITE MANAGER" && GIStats == "06") || (ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE"))
                                {
                                    if (cbRef.Text != "Nota Retur Beli")
                                    {
                                        //INSERT TO INVENT TRANS
                                        insertInventTrans(GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, InventSiteID, GoodsIssuedId, GoodsIssuedSeqNo, GoodsIssuedDate, RefTransID, RefTransDate, RefTransSeqNo, VehicleOwnerID, VehicleOwnerName, Qty_Actual, NotesD);

                                        //REDUCE INVENT STOCK
                                        insertInventOnHandQty(FullItemId, InventSiteID, Qty_Actual, Ratio, RefTransID, RefTransSeqNo, GoodsIssuedSeqNo);
                                    }

                                    if (cbRef.Text == "Delivery Order")
                                    {
                                        //UPDATE DO: DO Issued Outstanding
                                        updateDOIssuedOuts(FullItemId, Qty_Actual, Qty_Actual * Ratio);
                                    }
                                    else if (cbRef.Text == "Nota Transfer")
                                    {
                                        //UPDATE Nota Transfer: Transfer in Progress, Transfer Out in Progress
                                        updateTransferInProgress_TransferOutProgress(RefTransID, RefTransSeqNo, FullItemId, Qty_Actual, Qty_Actual * Ratio);
                                    }
                                    else if (cbRef.Text == "Nota Retur Beli" )
                                    {
                                        //UPDATE NRB: PO Issued Outstanding, Retur Beli In Progress
                                        updatePOIssuedOuts_NRBInProgress(RefTransID, RefTransSeqNo, FullItemId, Qty_Actual, Qty_Actual * Ratio);
                                        //updateROremaining(RefTransID, RefTransSeqNo, FullItemId, Qty_Actual, Qty_Actual * Ratio);
                                    }
                                    else if (cbRef.Text == "Nota Retur Jual")
                                    {
                                        updateNRJInProgress(RefTransID, RefTransSeqNo, FullItemId, Qty_Actual, Qty_Actual * Ratio);
                                    }
                                    
                                    //Begin
                                    //Created By : Joshua
                                    //Created Date ; 24 Aug 2018
                                    //Desc : Create Journal
                                    CreateJournal();
                                    if (Journal == true)
                                    {
                                        Journal = false;
                                        goto Outer;
                                    }
                                    //End
                                }

                                //INSERT DO ISSUED LOG TABLE
                                if (cbRef.Text == "Delivery Order")
                                {
                                    if ((ControlMgr.GroupName == "WB OPERATOR" && GIStats == "01"))
                                    {
                                        insertDOIssuedLogTable(RefTransID, RefTransSeqNo, Qty, Qty * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                    }
                                    else if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                                    {
                                        insertDOIssuedLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                    }
                                }
                                //INSERT NOTA TRANSFER LOG TABLE 
                                else if (cbRef.Text == "Nota Transfer")
                                {
                                    if (ControlMgr.GroupName == "WB OPERATOR" && GIStats == "01")
                                    {
                                        insertNotaTransferLogTable(RefTransID, RefTransSeqNo, Qty, Qty * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                    }
                                    else if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                                    {
                                        insertNotaTransferLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                    }
                                }
                                //INSERT NOTA RETUR BELI LOG TABLE 
                                else if (cbRef.Text == "Nota Retur Beli")
                                {
                                    if (ControlMgr.GroupName == "WB OPERATOR" && GIStats == "01")
                                    {
                                        insertNotaReturBeliLogTable(RefTransID, RefTransSeqNo, Qty, Qty * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                    }
                                    else if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                                    {
                                        insertNotaReturBeliLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                    }
                                }
                                else if (cbRef.Text == "Nota Retur Jual")
                                {
                                    if (ControlMgr.GroupName == "WB OPERATOR" && GIStats == "01")
                                    {
                                        insertNotaReturJualLogTable(RefTransID, RefTransSeqNo, Qty, Qty * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                    }
                                    else if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                                    {
                                        insertNotaReturJualLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, RefTransDate, AccountNum, InventSiteID, FullItemId, StatusCode);
                                    }
                                }
                                //INSERT GI LOG TABLE 
                                if ((ControlMgr.GroupName == "WB OPERATOR" && GIStats == "01"))
                                {
                                    insertGoodsIssuedLogTable(GoodsIssuedDate, GoodsIssuedId, AccountNum, GoodsIssuedSeqNo, InventSiteID, Qty, Qty * Ratio, RefTransID, RefTransDate, RefTransSeqNo, FullItemId, StatusCode);
                                }
                                else if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                                {
                                    insertGoodsIssuedLogTable(GoodsIssuedDate, GoodsIssuedId, AccountNum, GoodsIssuedSeqNo, InventSiteID, Qty_Actual, Qty_Actual * Ratio, RefTransID, RefTransDate, RefTransSeqNo, FullItemId, StatusCode);
                                }
                            }
                            updateRefStatus(RefTransID);
                            //insertStatusLogSave(GoodsIssuedId);
                            ListMethod.StatusLogCustomer("BBKHeader", "GI", tbxNameID.Text, StatusCode, "", tbxBBKID.Text, "", "", "");

                            Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + GoodsIssuedId + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            if (Cmd.ExecuteScalar().ToString() == "05")
                                MetroFramework.MetroMessageBox.Show(this, tbxBBKID.Text + " weight2 melebihi toleransi. Tolong request Site Manager untuk approval!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                            {
                                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                                {
                                    MetroFramework.MetroMessageBox.Show(this, tbxBBKID.Text + " Save berhasil!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MetroFramework.MetroMessageBox.Show(this, tbxBBKID.Text + " Update berhasil!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                        Mode = "BeforeEdit";
                        GetDataHeader();
                        GetDataHeader2();
                        ModeBeforeEdit();
                    }
                    Conn.Close();
                    scope.Complete();

                Outer: ;
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
            Parent.RefreshGrid();
        }

        private void CreateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date ; 24 Aug 2018
            //Desc : Create Journal
           
            //decimal NYIDN = 0, NYI = 0, ReturInProgress = 0, DVarian = 0, KVarian = 0, Tax = 0, InventPrice = 0;           
            //decimal Varian = 0;
            decimal DOValue = 0, TukarBarang = 0, Price = 0;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemID"].Value);
                decimal Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);
                string SeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["SeqNo"].Value);
                

                if (cbRef.SelectedItem.ToString() == "Delivery Order")
                {
                    if (Unit.ToUpper() == "KG")
                    {
                        Price = GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                        DOValue = DOValue + (Qty * Price);
                    }
                    else
                    {
                        Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                        DOValue = DOValue + (Qty * Price);
                    }
                }
                else if (cbRef.SelectedItem.ToString() == "Nota Retur Jual")
                {
                    string ReturJualType = GetReturJualType();

                    //Tukar Barang
                    if (ReturJualType == "01")
                    {
                        //GET PricePO
                        string SPrice = GetBBKPrice(SeqNo);

                        decimal BKKPrice;
                        if (SPrice == "")
                        {
                            BKKPrice = 1;
                        }
                        else
                        {
                            BKKPrice = Convert.ToDecimal(SPrice);
                        }
                        
                        TukarBarang = TukarBarang + (BKKPrice * Qty);
                    }
                    //else
                    //{
                    //    int RefTransSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["SeqNo"].Value);

                    // //   if (tbxRefID.Text.Contains("NRBA"))
                    //  //  {
                    //        //Without Varian
                    // //   }
                    //  //  else
                    //   // {
                    //        Query = "SELECT H.ActionCode, D.Price, D.PPN, D.PPH FROM NotaReturBeli_Dtl D ";
                    //        Query += "INNER JOIN NotaReturBeliH H ON H.NRBId = D.NRBId ";
                    //        Query += "AND H.NRBId = '" + tbxRefID.Text + "' ";
                    //        Query += "AND D.SeqNo = " + RefTransSeqNo + "";
                    //        Cmd = new SqlCommand(Query, Conn);
                    //        Dr = Cmd.ExecuteReader();
                    //        while (Dr.Read())
                    //        {
                    //            string ActionCode = Convert.ToString(Dr["ActionCode"]);
                    //            string StringPrice = Convert.ToString(Dr["Price"]);
                    //            string StringPPN = Convert.ToString(Dr["PPN"]);
                    //            string StringPPH = Convert.ToString(Dr["PPH"]);
                    //            decimal PPN = 0;
                    //            decimal PPH = 0;

                    //            if (StringPrice == "")
                    //            {
                    //                Price = 0;
                    //            }
                    //            else
                    //            {
                    //                Price = Convert.ToDecimal(StringPrice);
                    //            }

                    //            if (StringPPN == "")
                    //            {
                    //                PPN = 0;
                    //            }
                    //            else
                    //            {
                    //                PPN = Convert.ToDecimal(StringPPN);
                    //            }

                    //            if (StringPPH == "")
                    //            {
                    //                PPH = 0;
                    //            }
                    //            else
                    //            {
                    //                PPH = Convert.ToDecimal(StringPPH);
                    //            }

                    //            if (ActionCode != "02")
                    //            {
                    //                NYI = NYI + (Qty * Price);
                    //            }
                    //            else
                    //            {
                    //                NYIDN = NYIDN + (Qty * Price);
                    //            }

                    //            if (PPN != 0)
                    //            {
                    //                Tax = Tax + (Qty * Price * (PPN / 100));
                    //            }

                    //            if (PPH != 0)
                    //            {
                    //                Tax = Tax + (Qty * Price * (PPH / 100));
                    //            }

                    //            if (Unit.ToUpper() == "KG")
                    //            {
                    //                InventPrice = GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                    //                ReturInProgress = ReturInProgress + (Qty * InventPrice);
                    //            }
                    //            else
                    //            {
                    //                InventPrice = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                    //                ReturInProgress = ReturInProgress + (Qty * InventPrice);
                    //            }

                    //            Varian = Varian + (InventPrice - Price);

                    //        }
                    //        Dr.Close();
                    //   // }
                    
                }
            }

            //if (Varian > 0)
            //{
            //    KVarian = Convert.ToDecimal(Convert.ToString(Varian).Substring(1));
            //}
            //else
            //{
            //    DVarian = Convert.ToDecimal(Convert.ToString(Varian).Substring(1));
            //}



            //if (DOValue != 0 || NYIDN != 0 || NYI != 0 || ReturInProgress != 0 || KVarian != 0 || DVarian != 0)
            //{
            if (DOValue != 0 || TukarBarang != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "";
                if (DOValue != 0)
                {
                    JournalHID = "IN52";
                }
                else if (TukarBarang != 0)
                {
                    JournalHID = "IN61R";
                }

                //Get GLJournalHID
                Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + tbxBBKID.Text + "' ";
                Cmd = new SqlCommand(Query, Conn);
                string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

                if (GLJournalHID != "")
                {
                    Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                    Cmd = new SqlCommand(Query, Conn);
                    int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                    if (CountData == 1)
                    {
                        //Delete Journal Detail
                        Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    string Jenis = "JN", Kode = "JN";
                    GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Cmd);

                    string Notes = tbxRefID.Text + " - " + tbxNameID.Text + " - " + tbxName.Text;

                    Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                    Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxBBKID.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                }

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

                    if (JournalHID == "IN52")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            AmountValue = DOValue;
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            AmountValue = DOValue;
                        }
                    }
                    else if (JournalHID == "IN61R")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            AmountValue = TukarBarang;
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            AmountValue = TukarBarang;
                        }

                        //if (JournalIDSeqNo == 1)
                        //{
                        //    if (NYIDN != 0)
                        //    {
                        //        AmountValue = NYIDN + Tax;
                        //    }
                        //}
                        //else if (JournalIDSeqNo == 2)
                        //{
                        //    if (NYI != 0)
                        //    {
                        //        AmountValue = NYI + Tax;
                        //    }
                        //}
                        //else if (JournalIDSeqNo == 3)
                        //{
                        //    AmountValue = ReturInProgress;
                        //}
                        //else if (JournalIDSeqNo == 4)
                        //{
                        //    AmountValue = Tax;
                        //}
                        //else if (JournalIDSeqNo == 5)
                        //{
                        //    AmountValue = DVarian;
                        //}
                        //else if (JournalIDSeqNo == 6)
                        //{
                        //    AmountValue = KVarian;
                        //}
                    }

                    if (AmountValue == 0)
                    {
                        continue;
                    }

                    //else if (JournalIDSeqNo == 5 || JournalIDSeqNo == 6)
                    //{
                    //    if (tbxRefID.Text.Contains("NRBA"))
                    //    {
                    //        continue;
                    //    }
                    //}

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

        private string GetBBKPrice(string RefSeqNo)
        {
            //GET BBK Price
            Query = "SELECT GI.Price FROM GoodsIssuedD GI INNER JOIN NotaReturJual_Dtl NR ";
            Query += "ON NR.GoodsIssuedId = GI.GoodsIssuedId ";
            Query += "AND NR.GoodsIssued_SeqNo = GI.GoodsIssuedSeqNo ";
            Query += "AND NR.NRJId = '" + tbxRefID.Text + "' ";
            Query += "AND NR.SeqNo = " + RefSeqNo + " ";

            Cmd = new SqlCommand(Query, Conn);
            string Price = Convert.ToString(Cmd.ExecuteScalar());

            return Price;
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

        private string GetReturJualType()
        {
            Query = "SELECT ActionCode FROM NotaReturJualH WHERE NRJId = '" + tbxRefID.Text + "'";

            Cmd = new SqlCommand(Query, Conn);
            string ActionCode = Convert.ToString(Cmd.ExecuteScalar());

            return ActionCode;
        }


        //UPDATE Nota Transfer: Transfer in Progress, Transfer Out in Progress
        private void updateTransferInProgress_TransferOutProgress(string NTID, int SeqNo, string FullItemID, decimal Qty_UoM, decimal Qty_Alt)
        {
            decimal UoM = 0;
            decimal Alt = 0;
            decimal Amount = 0;
            decimal price = 0;
            decimal UoM2 = 0;
            decimal Alt2 = 0;
            decimal Amount2 = 0;
            decimal price2 = 0;

            Query = "select a.Transfer_In_Progress_UoM, a.Transfer_In_Progress_Alt, a.Transfer_In_Progress_Amount, a.Transfer_Keluar_In_Progress_UoM, a.Transfer_Keluar_In_Progress_Alt, a.Transfer_Keluar_In_Progress_Amount, b.UoM_AvgPrice from Invent_Movement_Qty a left join InventTable b on a.FullItemID = b.FullItemID where a.FullItemID = '" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                UoM = Convert.ToDecimal(Dr[0]);
                Alt = Convert.ToDecimal(Dr[1]);
                Amount = Convert.ToDecimal(Dr[2]);
                UoM2 = Convert.ToDecimal(Dr[3]);
                Alt2 = Convert.ToDecimal(Dr[4]);
                Amount2 = Convert.ToDecimal(Dr[5]);
                price2 = Convert.ToDecimal(Dr["UoM_AvgPrice"]);
            }
            Dr.Close();

            //Edited by: Thaddaeus, 15 May 2018, Begin
            Query = "select d.Price from NotaTransferH a left join  NotaTransferD b on a.TransferNo = b.TransferNo left join SalesOrderH c on b.ReferenceId = c.SalesOrderNo left join SalesOrderD d on c.SalesOrderNo = d.SalesOrderNo where b.TransferNo = '" + NTID + "' and b.SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            if (Cmd.ExecuteScalar() == (object)DBNull.Value)
            {
                Query = "select UoM_AvgPrice from InventTable where FullItemID = '" + FullItemID + "'";
                Cmd = new SqlCommand(Query, Conn);
                price = Convert.ToDecimal(Cmd.ExecuteScalar());
            }
            else
            {
                price = Convert.ToDecimal(Cmd.ExecuteScalar());
            }
            //End======================================

            UoM -= Qty_UoM;
            Alt -= Qty_Alt;
            Amount = Amount - (Qty_UoM * price);
            UoM2 += Qty_UoM;
            Alt2 += Qty_Alt;
            Amount2 = Amount2 + (Qty_UoM * price2);

            Query = "update Invent_Movement_Qty set Transfer_In_Progress_UoM = '" + UoM + "', Transfer_In_Progress_Alt = '" + Alt + "', Transfer_In_Progress_Amount = '" + Amount + "', Transfer_Keluar_In_Progress_UoM = '" + UoM2 + "', Transfer_Keluar_In_Progress_Alt = '" + Alt2 + "', Transfer_Keluar_In_Progress_Amount = '" + Amount2 + "' where FullItemID = '" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

        }

        //UPDATE DO: DO Issued Outstanding
        private void updateDOIssuedOuts(string FullItemId, decimal Qty_UoM, decimal Qty_Alt)
        {
            decimal UoM = 0;
            decimal Alt = 0;
            decimal Amount = 0;
            decimal price = 0;
            Query = "select a.DO_Issued_Outstanding_UoM, a.DO_Issued_Outstanding_Alt, a.DO_Issued_Outstanding_Amount, b.UoM_AvgPrice from Invent_Sales_Qty a left join InventTable b on a.FullItemId = b.FullItemID where a.FullItemID = '" + FullItemId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                UoM = Convert.ToDecimal(Dr[0]);
                Alt = Convert.ToDecimal(Dr[1]);
                Amount = Convert.ToDecimal(Dr[2]);
                price = Convert.ToDecimal(Dr[3]);
            }
            Dr.Close();

            UoM -= Qty_UoM;
            Alt -= Qty_Alt;
            Amount = Amount - (Qty_UoM * price);

            Query = "update Invent_Sales_Qty set DO_Issued_Outstanding_UoM = '" + UoM + "', DO_Issued_Outstanding_Alt = '" + Alt + "', DO_Issued_Outstanding_Amount = '" + Amount + "' where FullItemID = '" + FullItemId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateROremaining(string NRBId, int SeqNo, string FullItemID, decimal Qty_UoM, decimal Qty_Alt)
        {
            string ROId = "";
            int ROSeqNo = 0;
            Query = "SELECT a.[RefTransID],a.[RefTransSeqNo] FROM [dbo].[GoodsReceivedD] a LEFT JOIN [dbo].[NotaReturBeli_Dtl] b ON (a.[GoodsReceivedId]=b.[GoodsReceivedId] and a.GoodsReceivedSeqNo = b.GoodsReceived_SeqNo ) WHERE b.[NRBId]='" + NRBId + "' AND b.[SeqNo]='" + SeqNo + "' ";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    ROId = Dr["RefTransID"].ToString();
                    ROSeqNo = Convert.ToInt32(Dr["RefTransSeqNo"]);
                }
                Dr.Close();
            }
            Query = " UPDATE [ReceiptOrderD] SET [RemainingQty] += '" + Qty_UoM + "' WHERE FullItemId = '" + FullItemID + "' AND [ReceiptOrderId] = '" + ROId + "' AND [SeqNo] = " + ROSeqNo + "";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        //UPDATE NRB: PO Issued Outstanding, Retur Beli In Progress
        private void updatePOIssuedOuts_NRBInProgress(string NRBId, int SeqNo, string FullItemID, decimal Qty_UoM, decimal Qty_Alt)
        {
            decimal UoM = 0;
            decimal Alt = 0;
            decimal Amount = 0;
            decimal UoM2 = 0;
            decimal Alt2 = 0;
            decimal Amount2 = 0;
            //edited by Thaddaeus, 25JUNE2018, BEGIN
            //Query = "select a.PO_Issued_Outstanding_UoM, a.PO_Issued_Outstanding_Alt, a.PO_Issued_Outstanding_Amount, a.Retur_Beli_In_Progress_UoM, a.Retur_Beli_In_Progress_Alt, a.Retur_Beli_In_Progress_Amount, b.UoM_AvgPrice from Invent_Purchase_Qty a left join InventTable b on a.FullItemId = b.FullItemID where a.FullItemID = '" + FullItemID + "'";
            Query = "select a.RO_Issued_UoM, a.RO_Issued_Alt, a.RO_Issued_Amount, a.Retur_Beli_In_Progress_UoM, a.Retur_Beli_In_Progress_Alt, a.Retur_Beli_In_Progress_Amount, b.UoM_AvgPrice from Invent_Purchase_Qty a left join InventTable b on a.FullItemId = b.FullItemID where a.FullItemID = '" + FullItemID + "'";
            //END===================================
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                UoM = Convert.ToDecimal(Dr[0]);
                Alt = Convert.ToDecimal(Dr[1]);
                Amount = Convert.ToDecimal(Dr[2]);
                UoM2 = Convert.ToDecimal(Dr[3]);
                Alt2 = Convert.ToDecimal(Dr[4]);
                Amount2 = Convert.ToDecimal(Dr[5]);
            }
            Dr.Close();
            decimal price = 0;
            Query = "select ROD.Price from NotaReturBeliH a left join NotaReturBeli_Dtl b on a.NRBId = b.NRBId left join GoodsReceivedD c on c.GoodsReceivedId = b.GoodsReceivedId and b.GoodsReceived_SeqNo = c.GoodsReceivedSeqNo left join ReceiptOrderD ROD on c.RefTransID = ROD.ReceiptOrderId where b.NRBId = '" + NRBId + "' and b.SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            if (Cmd.ExecuteScalar() == System.DBNull.Value)
            {
                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@FullItemID", FullItemID);
                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
            }
            else
            {
                price = Convert.ToDecimal(Cmd.ExecuteScalar());
            }

            UoM += Qty_UoM;
            Alt += Qty_Alt;
            Amount = Amount + (Qty_UoM * price);
            UoM2 -= Qty_UoM;
            Alt2 -= Qty_Alt;
            Amount2 = Amount2 - (Qty_UoM * price);
            //edited by Thaddaeus, 25JUNE2018, BEGIN
            //Query = "update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = '" + UoM + "', PO_Issued_Outstanding_Alt = '" + Alt + "', PO_Issued_Outstanding_Amount = '" + Amount + "', Retur_Beli_In_Progress_UoM = '" + UoM2 + "', Retur_Beli_In_Progress_Alt = '" + Alt2 + "', Retur_Beli_In_Progress_Amount = '" + Amount2 + "' where FullItemId = '" + FullItemID + "'";
            //edited by Thaddaeus, 1SEPT2018, BEGIN
            //Query = " UPDATE Invent_Purchase_Qty SET [RO_Issued_UoM] = '" + UoM + "', [RO_Issued_Alt] = '" + Alt + "', [RO_Issued_Amount] = '" + Amount + "', Retur_Beli_In_Progress_UoM = '" + UoM2 + "', Retur_Beli_In_Progress_Alt = '" + Alt2 + "', Retur_Beli_In_Progress_Amount = '" + Amount2 + "' where FullItemId = '" + FullItemID + "'";
            Query = " UPDATE Invent_Purchase_Qty SET Retur_Beli_In_Progress_UoM = '" + UoM2 + "', Retur_Beli_In_Progress_Alt = '" + Alt2 + "', Retur_Beli_In_Progress_Amount = '" + Amount2 + "' where FullItemId = '" + FullItemID + "'";
            //END===================================
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateNRJInProgress(string NRJId, int SeqNo, string FullItemID, decimal Qty_UoM, decimal Qty_Alt)
        {
            decimal UoM = 0;
            decimal Alt = 0;
            decimal Amount = 0;
            decimal price = 0;
            Query = "select d.Price from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + NRJId + "' and a.SeqNo = " + SeqNo;
            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
            SqlDataReader Dr2 = Cmd2.ExecuteReader();
            while (Dr2.Read())
            {
                price = Convert.ToDecimal(Dr2["Price"]);
            }
            Dr2.Close();

            UoM += Qty_UoM;
            Alt += Qty_Alt;
            Amount += (Qty_UoM * price);
            Query = " UPDATE [Invent_Sales_Qty] SET [Retur_Jual_Approved_Oustanding_UoM]-= "+UoM+",[Retur_Jual_Approved_Oustanding_Alt] -= "+ Alt+",[Retur_Jual_Approved_Oustanding_Amount] -= "+Amount+",[Retur_Jual_GR_In_Progress_UoM] += "+UoM+",[Retur_Jual_GR_In_Progress_Alt] += "+Alt+",[Retur_Jual_GR_In_Progress_Amount] +="+Amount +" where FullItemId = '" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void insertGoodsIssuedLogTable(DateTime GoodsIssuedDate, string GoodsIssuedNo, string CustID, int SeqNo, string InventSiteID, decimal Qty_UoM, decimal Qty_Alt, string RefTransID, DateTime RefTransDate, int RefTransSeqNo, string FullItemId, string LogStatusCode)
        {
            decimal Amount = 0;
            if (cbRef.Text == "Delivery Order")
            {
                Query = "select SOD.Price from DeliveryOrderD DOD left join SalesOrderD SOD on DOD.SalesOrderId = SOD.SalesOrderNo and DOD.SalesOrderSeqNo = SOD.SeqNo where DOD.DeliveryOrderId = '" + RefTransID + "' and DOD.SeqNo = '" + RefTransSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                if (Cmd.ExecuteScalar() != null)
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
            }
            else if (cbRef.Text == "Nota Transfer")
            {
                Query = "select SOD.Price from NotaTransferD NTD left join SalesOrderD SOD on NTD.ReferenceId = SOD.SalesOrderNo and NTD.Reference_SeqNo = SOD.SeqNo where NTD.TransferNo = '" + RefTransID + "' and NTD.SeqNo = '" + RefTransSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                if (Cmd.ExecuteScalar() != (object)DBNull.Value)
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
            }
            else if (cbRef.Text == "Nota Retur Beli")
            {
                Query = "select ROD.Price from NotaReturBeli_Dtl NRBD left join GoodsReceivedD GRD on NRBD.GoodsReceivedId = GRD.GoodsReceivedId and GoodsReceived_SeqNo = GRD.GoodsReceivedSeqNo left join ReceiptOrderD ROD on GRD.RefTransID = ROD.ReceiptOrderId and GRD.RefTransSeqNo = ROD.SeqNo where NRBD.NRBId = '" + RefTransID + "' and NRBD.SeqNo = '" + RefTransSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                if (Cmd.ExecuteScalar() != System.DBNull.Value)
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
            }
            else if (cbRef.Text == "Nota Retur Jual")
            {
                Query = "select d.Price from NotaReturJual_Dtl as a left join [GoodsIssuedD] as b on a.[GoodsIssuedId] = b.[GoodsIssuedId] left join [DeliveryOrderD] as c on b.[RefTransID] = c.[DeliveryOrderId] Left join [SalesOrderD] d ON d.[SalesOrderNo] = c.[SalesOrderId] AND d.[SeqNo] = c.[SalesOrderSeqNo] where a.NRJId = '" + RefTransID + "' and a.SeqNo = '" + RefTransSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                if (Cmd.ExecuteScalar() == (object)DBNull.Value)
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemID = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                else
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
            }

            Query = "select deskripsi from TransStatusTable where TransCode = 'GI' and StatusCode = '" + LogStatusCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            string LogStatusDesc = Cmd.ExecuteScalar().ToString();

            Query = "INSERT INTO [dbo].[GoodsIssued_LogTable] ([GoodsIssuedDate],[GoodsIssuedNo],[CustID],[SeqNo],[InventSiteID],[Qty_UoM],[Qty_Alt],[Amount],[DeliveryOrderNo],[DeliveryOrderDate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES ('" + GoodsIssuedDate + "', '" + GoodsIssuedNo + "', '" + CustID + "', '" + SeqNo + "', '" + InventSiteID + "', '" + Qty_UoM + "', '" + Qty_Alt + "', '" + Amount + "', '" + RefTransID + "', '" + RefTransDate + "', '" + LogStatusCode + "', '" + LogStatusDesc + "', @LogDescription, '" + ControlMgr.UserId + "', getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + LogStatusCode + ". " + LogStatusDesc);
            Cmd.ExecuteNonQuery();
        }

        private void insertDOIssuedLogTable(string DeliveryOrderId, int SeqNo, decimal Qty_UoM, decimal Qty_Alt, DateTime DeliveryOrderDate, string CustID, string InventSiteID, string FullItemID, string LogStatusCode)
        {
            //string LogStatusCode = checkRemaingRefQty(DeliveryOrderId);

            decimal Amount = 0;
            string SalesOrderId = "";
            DateTime SalesOrderDate = new DateTime(1753, 1, 1);
            Query = "select DOH.DeliveryOrderDate, DOH.CustID, DOH.InventSiteID, DOH.SalesOrderId, DOH.SalesOrderDate, SOD.Price, DOD.FullItemID from DeliveryOrderH DOH left join DeliveryOrderD DOD on DOH.DeliveryOrderId = DOD.DeliveryOrderId left join SalesOrderD SOD on DOD.SalesOrderId = SOD.SalesOrderNo and DOD.SalesOrderSeqNo = SOD.SeqNo where DOD.DeliveryOrderId = '" + DeliveryOrderId + "' and DOD.SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["Price"] != (object)DBNull.Value)
                    Amount = Convert.ToDecimal(Dr["Price"]);
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemID + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                SalesOrderId = Dr["SalesOrderId"].ToString();
                SalesOrderDate = Convert.ToDateTime(Dr["SalesOrderDate"]);
            }
            Dr.Close();

            Query = "select deskripsi from TransStatusTable where TransCode = 'GI' and StatusCode = '" + LogStatusCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            string LogStatusDesc = Cmd.ExecuteScalar().ToString();

            Query = "INSERT INTO [dbo].[DO_Issued_LogTable] ([DeliveryOrderDate],[DeliveryOrderId],[CustID],[SeqNo],[InventSiteID],[Qty_UoM],[Qty_Alt],[Amount],[SalesOrderId],[SalesOrderDate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES ('" + DeliveryOrderDate + "', '" + DeliveryOrderId + "', '" + CustID + "', '" + SeqNo + "', '" + InventSiteID + "', '" + Qty_UoM + "', '" + Qty_Alt + "', '" + Amount + "', '" + SalesOrderId + "', '" + SalesOrderDate + "', '" + LogStatusCode + "', '" + LogStatusDesc + "', @LogDescription, '" + ControlMgr.UserId + "', getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + LogStatusCode + ". " + LogStatusDesc);
            Cmd.ExecuteNonQuery();
        }

        private void insertNotaReturBeliLogTable(string NRBId, int SeqNo, decimal Qty_UoM, decimal Qty_Alt, DateTime NRBDate, string VendId, string SiteId, string FullItemId, string LogStatusCode)
        {
            decimal Amount = 0;
            DateTime GoodsIssuedDate = new DateTime(1753, 1, 1);
            string GoodsIssuedId = "";
            Query = "select a.NRBDate, a.GoodsReceivedId, a.GoodsReceivedDate, a.VendId, a.SiteId, b.FullItemId, ROD.Price from NotaReturBeliH a left join NotaReturBeli_Dtl b on a.NRBId = b.NRBId left join GoodsReceivedD c on c.GoodsReceivedId = b.GoodsReceivedId and b.GoodsReceived_SeqNo = c.GoodsReceivedSeqNo left join ReceiptOrderD ROD on c.RefTransID = ROD.ReceiptOrderId where b.NRBId = '" + NRBId + "' and b.SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["Price"] != (object)DBNull.Value)
                    Amount = Convert.ToDecimal(Dr["Price"]);
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                NRBDate = Convert.ToDateTime(Dr["NRBDate"]);
                GoodsIssuedId = tbxBBKID.Text;
                GoodsIssuedDate = dtBBK.Value;
                VendId = Dr["VendId"].ToString();
                SiteId = Dr["SiteId"].ToString();
                FullItemId = Dr["FullItemId"].ToString();
            }
            Dr.Close();

            Query = "select deskripsi from TransStatusTable where TransCode = 'GI' and StatusCode = '" + LogStatusCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            string LogStatusDesc = Cmd.ExecuteScalar().ToString();

            Query = "INSERT INTO [dbo].[NotaReturBeli_LogTable] ([NRBDate],[NRBId],[GoodsReceivedDate],[GoodsReceivedId],[VendId],[SiteId],[FullItemId],[SeqNo],[Qty_UoM],[Qty_Alt],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES ('" + NRBDate + "', '" + NRBId + "', '" + GoodsIssuedDate + "', '" + GoodsIssuedId + "', '" + VendId + "', '" + SiteId + "', '" + FullItemId + "', '" + SeqNo + "', '" + Qty_UoM + "', '" + Qty_Alt + "', '" + Amount + "', '" + LogStatusCode + "', '" + LogStatusDesc + "', @LogDescription, '" + ControlMgr.UserId + "', getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + LogStatusCode + ". " + LogStatusDesc);
            Cmd.ExecuteNonQuery();
        }

        private void insertNotaReturJualLogTable(string NRJId, int SeqNo, decimal Qty_UoM, decimal Qty_Alt, DateTime NRJDate, string CustId, string SiteId, string FullItemId, string LogStatusCode)
        {

            decimal Amount = 0;
            DateTime GoodsIssuedDate = new DateTime(1753, 1, 1);
            string GoodsIssuedId = "";
            Query = "SELECT a.[NRJDate], a.[GoodsIssuedId],a.[GoodsIssuedDate],a.[CustId],a.[SiteId],b.[FullItemId],e.[Price] FROM [NotaReturJualH] a LEFT JOIN [NotaReturJual_Dtl] b ON a.[NRJId]=b.[NRJId] LEFT JOIN [GoodsIssuedD] c ON b.[GoodsIssuedId] = c.[GoodsIssuedId] AND b.[GoodsIssued_SeqNo] = c.[GoodsIssuedSeqNo] LEFT JOIN [DeliveryOrderD] d ON d.[DeliveryOrderId] = c.[RefTransID] AND d.[SeqNo] = c.[RefTransSeqNo] LEFT JOIN [SalesOrderD] e ON e.[SalesOrderNo] =d.[SalesOrderId] AND e.[SeqNo] =d.[SalesOrderSeqNo]  WHERE a.NRJId = @NRJId AND b.SeqNo = @SeqNo ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NRJId", NRJId);
            Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                
                if (Dr["Price"] != (object)DBNull.Value)
                    Amount = Convert.ToDecimal(Dr["Price"]);
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                NRJDate = Convert.ToDateTime(Dr["NRJDate"]);
                GoodsIssuedDate = Convert.ToDateTime(Dr["GoodsIssuedDate"]);
                GoodsIssuedId = Dr["GoodsIssuedId"].ToString();
                CustId = Dr["CustId"].ToString();
                SiteId = Dr["SiteId"].ToString();
                FullItemId = Dr["FullItemId"].ToString();
            }
            Dr.Close();

            Query = "select deskripsi from TransStatusTable where TransCode = 'GI' and StatusCode = '" + LogStatusCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            string LogStatusDesc = Cmd.ExecuteScalar().ToString();

            Query = "INSERT INTO [dbo].[NotaReturJual_LogTable] ([NRJDate],[NRJId],[GoodsIssuedDate],[GoodsIssuedId],[CustId],[SiteId],[FullItemId],[SeqNo],[Qty_UoM],[Qty_Alt],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES ('" + NRJDate + "', '" + NRJId + "', '" + GoodsIssuedDate + "', '" + GoodsIssuedId + "', '" + CustId + "', '" + SiteId + "', '" + FullItemId + "', '" + SeqNo + "', '" + Qty_UoM + "', '" + Qty_Alt + "', '" + Amount + "', '" + LogStatusCode + "', '" + LogStatusDesc + "', @LogDescription, '" + ControlMgr.UserId + "', getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + LogStatusCode + ". " + LogStatusDesc);
            Cmd.ExecuteNonQuery();
        }

        private void insertNotaTransferLogTable(string NTId, int SeqNo, decimal Qty_UoM, decimal Qty_Alt, DateTime NTDate, string FromSiteId, string ToSiteId, string FullItemId, string LogStatusCode)
        {
            decimal Amount = 0;
            //string LogStatusCode = checkRemaingRefQty(NTId);

            string RefTransId = "";
            DateTime RefTransDate = new DateTime(1753, 1, 1);
            Query = "select a.TransferDate, b.ReferenceId, c.OrderDate, a.InventSiteFrom, a.InventSiteTo, b.FullItemId, b.SeqNo, d.Price from NotaTransferH a left join  NotaTransferD b on a.TransferNo = b.TransferNo left join SalesOrderH c on b.ReferenceId = c.SalesOrderNo left join SalesOrderD d on c.SalesOrderNo = d.SalesOrderNo where b.TransferNo = '" + NTId + "' and b.SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["Price"] != (object)DBNull.Value)
                    Amount = Convert.ToDecimal(Dr["Price"]);
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                if (!(Dr["ReferenceId"] == (object)DBNull.Value || Dr["ReferenceId"].ToString() == String.Empty))
                {
                    RefTransId = Dr["ReferenceId"].ToString();
                    RefTransDate = Convert.ToDateTime(Dr["TransferDate"]);
                }
            }

            Query = "select deskripsi from TransStatusTable where TransCode = 'GI' and StatusCode = '" + LogStatusCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            string LogStatusDesc = Cmd.ExecuteScalar().ToString();

            Query = "INSERT INTO [dbo].[NotaTransfer_LogTable] ([NTDate],[NTId],[RefTransId],[RefTransDate],[FromSiteId],[ToSiteId],[FullItemId],[SeqNo],[Flag],[LockDocument],[Qty_UoM],[Qty_Alt],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES (@NTDate, '" + NTId + "', @RefTransId, @RefTransDate, '" + FromSiteId + "', '" + ToSiteId + "', '" + FullItemId + "', '" + SeqNo + "', '0', 'BBK', '" + Qty_UoM + "', '" + Qty_Alt + "', '" + Amount + "', '" + LogStatusCode + "', '" + LogStatusDesc + "', @LogDescription, '" + ControlMgr.UserId + "', getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NTDate", NTDate);
            Cmd.Parameters.AddWithValue("@RefTransId", RefTransId == "" ? (object)DBNull.Value : RefTransId);
            Cmd.Parameters.AddWithValue("@RefTransDate", RefTransId == "" ? (object)DBNull.Value : RefTransDate);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + LogStatusCode + ". " + LogStatusDesc);
            Cmd.ExecuteNonQuery();
        }

        private void insertInventOnHandQty(string FullItemId, string InventSiteID, decimal Qty_Actual, decimal Ratio, string RefTransID, int RefTransSeqNo, int goodseqno)
        {
            decimal Available_UoM = 0;
            decimal Available_For_Sale_UoM = 0;
            decimal Available_For_Sale_Reserved_UoM = 0;
            decimal Available_Amount = 0;
            decimal Available_For_Sale_Amount = 0;
            decimal Available_For_Sale_Reserved_Amount = 0;
            Query = "select Available_UoM, Available_For_Sale_UoM, Available_Amount, Available_For_Sale_Amount,[Available_For_Sale_Reserved_Amount],[Available_For_Sale_Reserved_UoM] from Invent_OnHand_Qty where FullItemId = '" + FullItemId + "' and InventSiteId = '" + InventSiteID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                Available_UoM = Convert.ToDecimal(Dr["Available_UoM"]);
                Available_For_Sale_UoM = Convert.ToDecimal(Dr["Available_For_Sale_UoM"]);
                Available_Amount = Convert.ToDecimal(Dr["Available_Amount"]);
                Available_For_Sale_Amount = Convert.ToDecimal(Dr["Available_For_Sale_Amount"]);
                Available_For_Sale_Reserved_UoM = Convert.ToDecimal(Dr["Available_For_Sale_Reserved_UoM"]);
                Available_For_Sale_Reserved_Amount = Convert.ToDecimal(Dr["Available_For_Sale_Reserved_UoM"]);
            }
            Dr.Close();

            decimal price = 0;
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select b.Price from DeliveryOrderD a left join SalesOrderD b on a.SalesOrderId = b.SalesOrderNo and a.SalesOrderSeqNo = b.SeqNo where a.DeliveryOrderId = '" + RefTransID + "' and a.SeqNo = '" + RefTransSeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar() == (object)DBNull.Value)
                    {
                        Query = "select UoM_AvgPrice from InventTable where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }
                    else
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    break;
                case "Nota Transfer":
                    Query = "select b.Price from NotaTransferD a left join SalesOrderD b on a.ReferenceId = b.SalesOrderNo and a.Reference_SeqNo = b.SeqNo where a.TransferNo = '" + RefTransID + "' and a.SeqNo = '" + RefTransSeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar() == (object)DBNull.Value)
                    {
                        Query = "select UoM_AvgPrice from InventTable where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }
                    else
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    break;
                case "Nota Retur Beli":
                    Query = "select c.Price from NotaReturBeli_Dtl a left join GoodsReceivedD b on a.GoodsReceivedId = B.GoodsReceivedId AND a.GoodsReceived_SeqNo = b.GoodsReceivedSeqNo left join ReceiptOrderD c on b.RefTransID = c.ReceiptOrderId and b.RefTransSeqNo = c.SeqNo where a.NRBId = '" + RefTransID + "' and a.SeqNo = '" + RefTransSeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar() == (object)DBNull.Value)
                    {
                        Query = "select UoM_AvgPrice from InventTable where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }
                    else
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    break;
                case "Nota Retur Jual":
                    Query = "select d.Price from NotaReturJual_Dtl as a left join [GoodsIssuedD] as b on a.[GoodsIssuedId] = b.[GoodsIssuedId] left join [DeliveryOrderD] as c on b.[RefTransID] = c.[DeliveryOrderId] Left join [SalesOrderD] d ON d.[SalesOrderNo] = c.[SalesOrderId] AND d.[SeqNo] = c.[SalesOrderSeqNo] where a.NRJId = '" + RefTransID + "' and a.SeqNo = '" + RefTransSeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar() == (object)DBNull.Value)
                    {
                        Query = "select UoM_AvgPrice from InventTable where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }
                    else
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    break;
            }

            //created by Thaddaeus 21 May2018,begin
            if (cbRef.Text == "Nota Transfer")
            {
                //decimal availableQty = 0;
                //decimal reservedQty = 0;
                //Query = "SELECT [Available_Qty],[Reserved_Qty] FROM [dbo].[GoodsIssuedD] WHERE [GoodsIssuedId] = '" + tbxBBKID.Text + "' AND [GoodsIssuedSeqNo]=" + goodseqno + " ";
                //using (Cmd = new SqlCommand(Query, Conn))
                //{
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        availableQty = Convert.ToDecimal(Dr["Available_Qty"]);
                //        reservedQty = Convert.ToDecimal(Dr["Reserved_Qty"]);
                //    }
                //    Dr.Close();
                //}
                Available_UoM -= Qty_Actual;
                Available_Amount = Available_Amount - (Qty_Actual * price);
                //Available_For_Sale_UoM -= availableQty;
                //Available_For_Sale_Amount = Available_For_Sale_Amount - (availableQty * price);
                //Available_For_Sale_Reserved_UoM -= reservedQty;
                //Available_For_Sale_Reserved_Amount -= Available_For_Sale_Reserved_Amount - (reservedQty * price);

                decimal Available_Alt = Available_UoM * Ratio;
                //decimal Available_For_Sale_Alt = Available_For_Sale_UoM * Ratio;
                //decimal Available_For_Sale_Reserved_Alt = Available_For_Sale_Reserved_UoM * Ratio;

                //Query = "update Invent_OnHand_Qty set Available_UoM = '" + Available_UoM + "', Available_For_Sale_UoM = '" + Available_For_Sale_UoM + "',Available_For_Sale_Reserved_UoM='" + Available_For_Sale_Reserved_UoM + "', Available_Alt = '" + Available_Alt + "', Available_For_Sale_Alt = '" + Available_For_Sale_Alt + "',Available_For_Sale_Reserved_Alt='" + Available_For_Sale_Reserved_Alt + "', Available_Amount = '" + Available_Amount + "', Available_For_Sale_Amount = '" + Available_For_Sale_Amount + "',Available_For_Sale_Reserved_Amount='"+Available_For_Sale_Reserved_Amount+"' where FullItemId = '" + FullItemId + "' and InventSiteId = '" + InventSiteID + "'";
                Query = "update Invent_OnHand_Qty set Available_UoM = '" + Available_UoM + "',  Available_Alt = '" + Available_Alt + "', Available_Amount = '" + Available_Amount + "' where FullItemId = '" + FullItemId + "' and InventSiteId = '" + InventSiteID + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }
            //end==============================
            else
            {
                Available_UoM -= Qty_Actual;
                Available_For_Sale_UoM -= Qty_Actual;
                Available_Amount = Available_Amount - (Qty_Actual * price);
                Available_For_Sale_Amount = Available_For_Sale_Amount - (Qty_Actual * price);

                decimal Available_Alt = Available_UoM * Ratio;
                decimal Available_For_Sale_Alt = Available_For_Sale_UoM * Ratio;

                Query = "update Invent_OnHand_Qty set Available_UoM = '" + Available_UoM + "', Available_For_Sale_UoM = '" + Available_For_Sale_UoM + "', Available_Alt = '" + Available_Alt + "', Available_For_Sale_Alt = '" + Available_For_Sale_Alt + "', Available_Amount = '" + Available_Amount + "', Available_For_Sale_Amount = '" + Available_For_Sale_Amount + "' where FullItemId = '" + FullItemId + "' and InventSiteId = '" + InventSiteID + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }
        }

        private void insertInventTrans(string GroupId, string SubGroup1Id, string SubGroup2Id, string ItemId, string FullItemId, string ItemName, string InventSiteID, string GoodsIssuedId, int GoodsIssuedSeqNo, DateTime GoodsIssuedDate, string RefTransID, DateTime RefTransDate, int RefTransSeqNo, string VehicleOwnerID, string VehicleOwnerName, decimal Qty_Actual, string NotesD)
        {
            Query = "select ratio from inventConversion where FullItemID = '" + FullItemId + "'";
            Cmd = new SqlCommand(Query, Conn);
            decimal conversionRatio = (Decimal)Cmd.ExecuteScalar();
            decimal qtyAvailable = 0;
            decimal qtyReserved = 0;
            //GET PRICE
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select b.Price from DeliveryOrderD as a left join SalesOrderD as b on a.SalesOrderId = b.SalesOrderNo where a.DeliveryOrderId = '" + RefTransID + "' and a.SeqNo = '" + RefTransSeqNo + "'";
                    break;
                case "Nota Transfer":
                    Query = "select COGS from NotaTransferD where TransferNo = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'";
                    break;
                case "Nota Retur Beli":
                    Query = "select c.Price from NotaReturBeli_Dtl as a left join GoodsReceivedD as b on a.GoodsReceivedID = b.GoodsReceivedId left join ReceiptOrderD as c on b.RefTransID = c.ReceiptOrderId where a.NRBId = '" + RefTransID + "' and a.SeqNo = '" + RefTransSeqNo + "'";
                    break;
                case "Nota Retur Jual":
                    Query = "select d.Price from NotaReturJual_Dtl as a left join [GoodsIssuedD] as b on a.[GoodsIssuedId] = b.[GoodsIssuedId] left join [DeliveryOrderD] as c on b.[RefTransID] = c.[DeliveryOrderId] Left join [SalesOrderD] d ON d.[SalesOrderNo] = c.[SalesOrderId] AND d.[SeqNo] = c.[SalesOrderSeqNo] where a.NRJId = '" + RefTransID + "' and a.SeqNo = '" + RefTransSeqNo + "'";
                    break;
            }
            if (cbRef.Text == "Nota Transfer")
            {
                decimal price = 0;
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                Query = "SELECT [Available_Qty],[Reserved_Qty] FROM [dbo].[GoodsIssuedD] WHERE [GoodsIssuedId] = '" + tbxBBKID.Text + "' AND [GoodsIssuedSeqNo] = " + GoodsIssuedSeqNo + "";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        qtyAvailable = Convert.ToDecimal(Dr["Available_Qty"]);
                        qtyReserved = Convert.ToDecimal(Dr["Reserved_Qty"]);
                    }
                    Dr.Close();
                }
                Query = "INSERT INTO [dbo].[InventTrans] ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId] ,[FullItemId] ,[ItemName] ,[InventSiteId] ,[TransId] ,[SeqNo] ,[TransDate] ,[Ref_TransId] ,[Ref_TransDate] ,[Ref_Trans_SeqNo] ,[AccountId] ,[AccountName] ,[Available_UoM] ,[Available_Alt] ,[Available_Amount] ,[Available_For_Sale_UoM] ,[Available_For_Sale_Alt] ,[Available_For_Sale_Amount] ,[Available_For_Sale_Reserved_UoM] ,[Available_For_Sale_Reserved_Alt] ,[Available_For_Sale_Reserved_Amount] ,[Notes]) VALUES ('" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "', '" + ItemName + "', '" + InventSiteID + "', '" + GoodsIssuedId + "', '" + GoodsIssuedSeqNo + "', '" + GoodsIssuedDate + "', '" + RefTransID + "', '" + RefTransDate + "', '" + RefTransSeqNo + "', '" + VehicleOwnerID + "', '" + VehicleOwnerName + "', '" + Qty_Actual + "', '" + Qty_Actual * conversionRatio + "', '" + Qty_Actual * price + "', '" + qtyAvailable + "', '" + qtyAvailable * conversionRatio + "', '" + qtyAvailable * price + "', '" + qtyReserved + "', '" + qtyReserved * conversionRatio + "', '" + qtyReserved * price + "', @NotesD)";
            }
            else
            {
                Cmd = new SqlCommand(Query, Conn);
                decimal price = (Decimal)Cmd.ExecuteScalar();
                Query = "INSERT INTO [dbo].[InventTrans] ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId] ,[FullItemId] ,[ItemName] ,[InventSiteId] ,[TransId] ,[SeqNo] ,[TransDate] ,[Ref_TransId] ,[Ref_TransDate] ,[Ref_Trans_SeqNo] ,[AccountId] ,[AccountName] ,[Available_UoM] ,[Available_Alt] ,[Available_Amount] ,[Available_For_Sale_UoM] ,[Available_For_Sale_Alt] ,[Available_For_Sale_Amount] ,[Available_For_Sale_Reserved_UoM] ,[Available_For_Sale_Reserved_Alt] ,[Available_For_Sale_Reserved_Amount] ,[Notes]) VALUES ('" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "', '" + ItemName + "', '" + InventSiteID + "', '" + GoodsIssuedId + "', '" + GoodsIssuedSeqNo + "', '" + GoodsIssuedDate + "', '" + RefTransID + "', '" + RefTransDate + "', '" + RefTransSeqNo + "', '" + VehicleOwnerID + "', '" + VehicleOwnerName + "', '" + Qty_Actual + "', '" + Qty_Actual * conversionRatio + "', '" + Qty_Actual * price + "', '" + Qty_Actual + "', '" + Qty_Actual * conversionRatio + "', '" + Qty_Actual * price + "', 0, 0, 0, @NotesD)";
            }

            //INSERT TO INVENT TRANS

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NotesD", NotesD);
            Cmd.ExecuteNonQuery();
        }

        private string checkRemaingRefQty(string RefTransID)
        {
            string status = "";
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select count(*) from DeliveryOrderD where DeliveryOrderId = '" + RefTransID + "'";
                    break;
                case "Nota Transfer":
                    Query = "Select count(*) From NotaTransferD where TransferNo = '" + RefTransID + "'";
                    break;
                case "Nota Retur Beli":
                    Query = "select count(*) from NotaReturBeli_Dtl where NRBId = '" + RefTransID + "'";
                    break;
                case "Nota Retur Jual":
                    Query = "select count(*) from NotaReturJual_Dtl where NRJId = '" + RefTransID + "'";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            int countItem = (Int32)Cmd.ExecuteScalar();

            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select Qty, RemainingQty, 0 from DeliveryOrderD where DeliveryOrderId = '" + RefTransID + "'";
                    break;
                case "Nota Transfer":
                    Query = "Select Qty, RemainingQty, 0 From NotaTransferD where TransferNo = '" + RefTransID + "'";
                    break;
                case "Nota Retur Beli":
                    //EDITED BY Thaddaeus, 30 August 2018, START
                    Query = "select SUM(UoM_Qty), SUM(RemainingQty),SUM(Remaining_Qty_RO) from NotaReturBeli_Dtl where NRBId = '" + RefTransID + "'";
                    break;
                case "Nota Retur Jual":
                    Query = "select SUM(UoM_Qty),SUM(RemainingQty), SUM([Remaining_Qty_DO]) from NotaReturJual_Dtl where NRJId = '" + RefTransID + "'";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int checkZero = 0;
            while (Dr.Read())
            {
                decimal UoM_Qty = Dr[0] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr[0]);
                decimal RemainingQty = Dr[1] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr[1]);
                decimal Remaining_Qty_Ref = Dr[2] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr[2]);
                if (cbRef.Text == "Nota Retur Beli" || cbRef.Text == "Nota Retur Jual")
                {
                    if (RemainingQty == 0)
                    {
                        if (Remaining_Qty_Ref == 0)
                        {
                            if (cbRef.Text == "Nota Retur Beli")
                            {
                                status = "11";
                            }
                            else if (cbRef.Text == "Nota Retur Jual")
                            {
                                status = "08";
                            }
                        }
                        else if(Remaining_Qty_Ref != 0)
                        {
                            if (UoM_Qty == Remaining_Qty_Ref)
                            {
                                if (cbRef.Text == "Nota Retur Beli")
                                {
                                    status = "08";
                                }
                                else if (cbRef.Text == "Nota Retur Jual")
                                {
                                    status = "11";
                                }
                            }
                            else if (UoM_Qty != Remaining_Qty_Ref)
                            {
                                if (cbRef.Text == "Nota Retur Beli")
                                {
                                    status = "09";
                                }
                                else if (cbRef.Text == "Nota Retur Jual")
                                {
                                    status = "12";
                                }
                            }
                        }
                    }
                    else if (RemainingQty != 0)
                    {
                        if (Remaining_Qty_Ref == 0)
                        {
                            if (UoM_Qty == RemainingQty)
                            {
                                status = "";
                            }
                            else if (UoM_Qty != RemainingQty)
                            {
                                if (cbRef.Text == "Nota Retur Beli")
                                {
                                    status = "09";
                                }
                                else if (cbRef.Text == "Nota Retur Jual")
                                {
                                    status = "10";
                                }
                            }
                        }
                        else if (Remaining_Qty_Ref != 0)
                        {
                            if (cbRef.Text == "Nota Retur Beli")
                            {
                                status = "09";
                            }
                            else if (cbRef.Text == "Nota Retur Jual")
                            {
                                status = "10";
                            }
                        }
                    }
                }
                    //END============================================================================================
                else if (RemainingQty == 0)
                {
                    checkZero++;
                }
            }
            Dr.Close();

            if (!(cbRef.Text == "Nota Retur Beli" || cbRef.Text == "Nota Retur Jual"))
            {
                //UPDATE REF STATUS
                if (checkZero == countItem)
                    status = "08";
                else
                    status = "09";
            }
            return status;
        }

        private void updateRefStatus(string RefTransID)
        {
            string status = checkRemaingRefQty(RefTransID);
            if (status == "")
            {
                return;
            }
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "update DeliveryOrderH set DeliveryOrderStatus = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where DeliveryOrderId = '" + RefTransID + "'; ";
                    break;
                case "Nota Transfer":
                    Query = "update NotaTransferH set TransStatus = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where TransferNo = '" + RefTransID + "'; ";
                    break;
                case "Nota Retur Beli":
                    if (ControlMgr.GroupName != "KERANI")
                    {
                        if (!(ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && Mode == "New"))
                        {
                            return;
                        }
                    }
                    Query = "update NotaReturBeliH set TransStatusId = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where NRBId= '" + RefTransID + "'; ";
                    break;
                case "Nota Retur Jual":
                    if (ControlMgr.GroupName != "KERANI")
                    {
                        if (!(ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && Mode == "New"))
                        {
                            return;
                        }
                    }
                    Query = "update NotaReturJualH set TransStatusId = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where NRJId= '" + RefTransID + "'; ";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateRefRemainingQty(string RefTransID, int RefTransSeqNo, decimal Qty_Actual, string GoodsIssuedId, int GoodsIssuedSeqNo, string FullItemId, int i)
        {
            decimal remainingQty = 0;
            decimal remainingQtyAvailable = 0;
            decimal remainingQtyReserved = 0;
            //GET OLD REF REMAINING QTY 
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select RemainingQty from DeliveryOrderD where DeliveryOrderId = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'";
                    break;
                case "Nota Transfer":
                    Query = "select RemainingQty,[RemainingAvailable],[RemainingReserved],[NT_Available_Qty],[NT_Reserved_Qty] from NotaTransferD where TransferNo = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'";
                    break;
                case "Nota Retur Beli":
                    if( ControlMgr.GroupName != "KERANI")
                    {
                        if (!(ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && Mode == "New"))
                        {
                            return;
                        }
                    }
                    Query = "select RemainingQty from NotaReturBeli_Dtl where NRBId = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'";
                    break;
                case "Nota Retur Jual":
                    if (ControlMgr.GroupName != "KERANI")
                    {
                        if (!(ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && Mode == "New"))
                        {
                            return;
                        }
                    }
                    Query = "select Remaining_Qty_DO 'RemainingQty' from [NotaReturJual_Dtl] where [NRJId] = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                remainingQty = Convert.ToDecimal(Dr["RemainingQty"]);
                if (cbRef.Text == "Nota Transfer")
                {
                    remainingQtyAvailable = Dr["RemainingAvailable"] == System.DBNull.Value ? Convert.ToDecimal(Dr["NT_Available_Qty"]) : Convert.ToDecimal(Dr["RemainingAvailable"]);
                    remainingQtyReserved = Dr["RemainingReserved"] == System.DBNull.Value ? Convert.ToDecimal(Dr["NT_Reserved_Qty"]) : Convert.ToDecimal(Dr["RemainingReserved"]);
                }
            }
            Dr.Close();

            //CALCULATE NEW REF REMAINING QTY
            Cmd = new SqlCommand("select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'", Conn);
            string GIstatus = Cmd.ExecuteScalar().ToString();
            if (ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && Mode == "New")
            {
                remainingQty = remainingQty - Qty_Actual;
            }
            else if ((ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "Administrator") && Cmd.ExecuteScalar().ToString() == "01" && Mode == "New")
            {
                remainingQty = remainingQty - Qty_Actual;
            }
            else if (ControlMgr.GroupName == "WB OPERATOR" && Cmd.ExecuteScalar().ToString() == "01" && Mode == "Edit")
            {
                Query = "Select Qty from GoodsIssuedD where GoodsIssuedId = '" + GoodsIssuedId + "' and GoodsIssuedSeqNo = '" + GoodsIssuedSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                decimal oldGIQty = (Decimal)Cmd.ExecuteScalar();
                if (oldGIQty > Qty_Actual)
                    remainingQty = remainingQty + (oldGIQty - Qty_Actual);
                else if (oldGIQty < Qty_Actual)
                    remainingQty = remainingQty - (Qty_Actual - oldGIQty);
            }
            else if (ControlMgr.GroupName == "KERANI" && Cmd.ExecuteScalar().ToString() == "01")
            {
                if (cbRef.Text == "Nota Retur Jual" || cbRef.Text == "Nota Retur Beli")
                {
                    remainingQty -= Qty_Actual;
                }
                else
                {
                    Query = "Select Qty from GoodsIssuedD where GoodsIssuedId = '" + GoodsIssuedId + "' and GoodsIssuedSeqNo = '" + GoodsIssuedSeqNo + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    decimal oldGIQty = (Decimal)Cmd.ExecuteScalar();
                    remainingQty = remainingQty + oldGIQty - Qty_Actual;
                }
            }
            else if (ControlMgr.GroupName == "KERANI" && Cmd.ExecuteScalar().ToString() == "02")
            {
                Query = "Select Qty_Actual from GoodsIssuedD where GoodsIssuedId = '" + GoodsIssuedId + "' and GoodsIssuedSeqNo = '" + GoodsIssuedSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                decimal oldGIQty_Actual = (Decimal)Cmd.ExecuteScalar();
                if (oldGIQty_Actual > Qty_Actual)
                    remainingQty = remainingQty + (oldGIQty_Actual - Qty_Actual);
                else if (oldGIQty_Actual < Qty_Actual)
                    remainingQty = remainingQty - (Qty_Actual - oldGIQty_Actual);
            }
            else if (ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && Mode == "Edit")
            {
                decimal oldGIQty_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                if (oldGIQty_Actual > Qty_Actual)
                    remainingQty = remainingQty + (oldGIQty_Actual - Qty_Actual);
                else if (oldGIQty_Actual < Qty_Actual)
                    remainingQty = remainingQty - (Qty_Actual - oldGIQty_Actual);
            }
            //Created by Thaddaeus, 21May2018, begin
            else if (ControlMgr.GroupName == "WB OPERATOR" && Cmd.ExecuteScalar().ToString() == "02" && cbRef.Text == "Nota Transfer")
            {
                decimal oldGIQty_Actual = 0;
                decimal qtyReserved = 0;
                Query = "Select Qty_Actual from GoodsIssuedD where GoodsIssuedId = '" + GoodsIssuedId + "' and GoodsIssuedSeqNo = '" + GoodsIssuedSeqNo + "'";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    oldGIQty_Actual = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                Query = "SELECT SUM(NT_Reserved_Qty) AS NT_Reserved_Qty, SUM(RemainingReserved) AS RemainingReserved FROM [dbo].[NotaTransferD] WHERE [TransferNo] = '" + tbxRefID.Text + "' AND [FullItemId] = '" + FullItemId + "'";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Dr["RemainingReserved"] == System.DBNull.Value)
                        {
                            qtyReserved = Convert.ToDecimal(Dr["NT_Reserved_Qty"]);
                        }
                        else
                            qtyReserved = Convert.ToDecimal(Dr["RemainingReserved"]);
                    }
                    Dr.Close();
                }
                if (qtyReserved > oldGIQty_Actual)
                {
                    remainingQtyReserved = remainingQtyReserved - oldGIQty_Actual;
                    RemainingReserved[i] = oldGIQty_Actual;
                }
                else if (qtyReserved == oldGIQty_Actual)
                {
                    remainingQtyReserved = 0;
                    RemainingReserved[i] = oldGIQty_Actual;
                }
                else
                {
                    remainingQtyAvailable = remainingQtyAvailable - (oldGIQty_Actual - qtyReserved);
                    remainingQtyReserved = 0;
                    RemainingReserved[i] = qtyReserved;
                }
            }
            //end====================================

            //UPDATE REF REMAINING QTY
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "update DeliveryOrderD set RemainingQty = '" + remainingQty + "',UpdatedDate = getdate(),UpdatedBy = '" + ControlMgr.UserId + "' where DeliveryOrderId = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'; ";
                    break;
                case "Nota Transfer":
                    Query = "update NotaTransferD set RemainingQty = '" + remainingQty + "',[RemainingAvailable]=" + remainingQtyAvailable + ",[RemainingReserved]=" + remainingQtyReserved + ",UpdatedDate = getdate(),UpdatedBy = '" + ControlMgr.UserId + "' where TransferNo = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'; ";
                    break;
                case "Nota Retur Beli":
                    decimal oldGIQty_Actual = 0;
                    if (CekNRBKembalikanBarang(RefTransID, RefTransSeqNo,Conn) == true)
                    {
                        Qty_Actual = 0;
                    }
                    else if (GIstatus == "02")
                    {
                        Query = "Select Qty_Actual from GoodsIssuedD where GoodsIssuedId = '" + GoodsIssuedId + "' and GoodsIssuedSeqNo = '" + GoodsIssuedSeqNo + "'";
                        using (Cmd = new SqlCommand(Query, Conn))
                        {
                            oldGIQty_Actual = Convert.ToDecimal(Cmd.ExecuteScalar());
                        }
                    }
                    Query = "update NotaReturBeli_Dtl set [Remaining_Qty_RO] = 0 ,UpdatedDate = getdate(),UpdatedBy = '" + ControlMgr.UserId + "' where NRBId = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "' AND Remaining_Qty_RO IS NULL; ";
                    Query += "update NotaReturBeli_Dtl set RemainingQty = '" + remainingQty + "',[Remaining_Qty_RO] += '" + (Qty_Actual - oldGIQty_Actual) + "',UpdatedDate = getdate(),UpdatedBy = '" + ControlMgr.UserId + "' where NRBId = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'; ";
                    break;
                case "Nota Retur Jual":
                    Query = "update [NotaReturJual_Dtl] set [Remaining_Qty_DO] = '" + remainingQty + "',UpdatedDate = getdate(),UpdatedBy = '" + ControlMgr.UserId + "' where [NRJId] = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'; ";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateGoodsIssuesD(DateTime GoodsIssuedDate, string No_SJ, string RefTransID, string GroupId, string SubGroup1Id, string SubGroup2Id, string ItemId, string FullItemId, string ItemName, decimal Qty, string Unit, decimal Ratio, decimal TotalBerat, string InventSiteID, string InventSiteBlokID, string NotesD, string GoodsIssuedId, int GoodsIssuedSeqNo, decimal Qty_Actual, decimal TotalBerat_Actual, string ActionCode, int i)
        {
            decimal price = 0;
            decimal total = 0;
            decimal totalDisc = 0;
            decimal ppn = 0;
            decimal totalPPN = 0;
            decimal pph = 0;
            decimal totalPPH = 0;
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select b.Price, b.DiscPercent, c.PPN, c.PPH from DeliveryOrderD a left join SalesOrderD b on a.SalesOrderId = b.SalesOrderNo and a.SalesOrderSeqNo = b.SeqNo left join SalesOrderH c on b.SalesOrderNo = c.SalesOrderNo where a.DeliveryOrderId = '" + RefTransID + "' and a.SeqNo = " + i;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        price = Convert.ToDecimal(Dr["Price"]);
                        totalDisc = price * Convert.ToDecimal(Dr["DiscPercent"]) / 100;
                        ppn = Convert.ToDecimal(Dr["PPN"]);
                        totalPPN = (price - totalDisc) * ppn / 100;
                        pph = Convert.ToDecimal(Dr["PPH"]);
                        totalPPH = (price - totalDisc) * pph / 100;
                    }
                    Dr.Close();
                    break;
                case "Nota Transfer":
                    Query = "select COGS from NotaTransferD where TransferNo = '" + RefTransID + "' and SeqNo = " + i;
                    Cmd = new SqlCommand(Query, Conn);
                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    break;
                case "Nota Retur Beli":
                    Query = "select b.RefTransID from NotaReturBeli_Dtl a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId and a.GoodsReceived_SeqNo = b.GoodsReceivedSeqNo where a.NRBId = '" + RefTransID + "' and a.SeqNo = " + i;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Dr["RefTransID"].ToString().Split('/')[0] == "RO" || Dr["RefTransID"].ToString().Split('/')[0] == "ROA")
                        {
                            Query = "select b.Price, b.Diskon, c.PPN, c.PPH from ReceiptOrderD a left join PurchDtl b on a.PurchaseOrderId = b.PurchID and a.PurchaseOrderSeqNo = b.SeqNo left join PurchH c on b.PurchID = c.PurchID where a.ReceiptOrderId = '" + RefTransID + "' and a.SeqNo = " + i;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr2["Diskon"]) / 100;
                                ppn = Convert.ToDecimal(Dr2["PPN"]);
                                totalPPN = (price - totalDisc) * Convert.ToDecimal(Dr2["PPN"]) / 100;
                                pph = Convert.ToDecimal(Dr2["PPH"]);
                                totalPPH = (price - totalDisc) * Convert.ToDecimal(Dr2["PPH"]) / 100;
                            }
                            Dr2.Close();
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NT")
                        {
                            Query = "select COGS from NotaTransferD where TransferNo = '" + RefTransID + "' and SeqNo = " + i;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NRJ")
                        {
                            Query = "select d.Price, d.DiscPercent, e.PPN, e.PPH from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr2["DiscPercent"]) / 100;
                                ppn = Convert.ToDecimal(Dr2["PPN"]);
                                totalPPN = (price - totalDisc) * ppn / 100;
                                pph = Convert.ToDecimal(Dr2["PPH"]);
                                totalPPH = (price - totalDisc) * pph / 100;
                            }
                            Dr2.Close();
                        }
                        else
                        {
                            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            {
                                Cmd2.Parameters.AddWithValue("@FullItemID", FullItemId);
                                price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                            }
                        }
                    }
                    break;
                case "Nota Retur Jual":
                    Query = "select b.RefTransID from NotaReturJual_Dtl a left join [GoodsIssuedD] b on a.[GoodsIssuedId] = b.[GoodsIssuedId] and a.[GoodsIssued_SeqNo] = b.[GoodsIssuedSeqNo] where a.NRJId = '" + RefTransID + "' and a.SeqNo = " + i;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Dr["RefTransID"].ToString().Split('/')[0] == "DO" || Dr["RefTransID"].ToString().Split('/')[0] == "DOA")
                        {
                            Query = "select b.Price, b.[DiscPercent], c.PPN, c.PPH from [DeliveryOrderD] a left join [SalesOrderD] b on a.[SalesOrderId] = b.[SalesOrderNo] and a.[SalesOrderSeqNo] = b.SeqNo left join [SalesOrderH] c on b.[SalesOrderNo] = c.[SalesOrderNo] where a.[DeliveryOrderId] = '" + RefTransID + "' and a.SeqNo = " + i;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr2["DiscPercent"]) / 100;
                                ppn = Convert.ToDecimal(Dr2["PPN"]);
                                totalPPN = (price - totalDisc) * Convert.ToDecimal(Dr2["PPN"]) / 100;
                                pph = Convert.ToDecimal(Dr2["PPH"]);
                                totalPPH = (price - totalDisc) * Convert.ToDecimal(Dr2["PPH"]) / 100;
                            }
                            Dr2.Close();
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NT")
                        {
                            Query = "select COGS from NotaTransferD where TransferNo = '" + RefTransID + "' and SeqNo = " + i;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NRB")
                        {
                            Query = "select d.Price, d.Diskon, e.PPN, e.PPH from NotaReturBeli_Dtl a left join [GoodsReceivedD] b on a.[GoodsReceivedId] = b.[GoodsReceivedId] and a.[GoodsReceived_SeqNo] = b.[GoodsReceivedSeqNo] left join [ReceiptOrderD] c on b.[RefTransID] = c.[ReceiptOrderId] and b.[RefTransSeqNo] = c.SeqNo left join [PurchDtl] d on c.[PurchaseOrderId] = d.[PurchID] and c.[PurchaseOrderSeqNo] = d.SeqNo left join [PurchH] e on d.[PurchID] = e.[PurchID] where a.NRBId = '" + RefTransID + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr2["Diskon"]) / 100;
                                ppn = Convert.ToDecimal(Dr2["PPN"]);
                                totalPPN = (price - totalDisc) * ppn / 100;
                                pph = Convert.ToDecimal(Dr2["PPH"]);
                                totalPPH = (price - totalDisc) * pph / 100;
                            }
                            Dr2.Close();
                        }
                        else
                        {
                            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            {
                                Cmd2.Parameters.AddWithValue("@FullItemID", FullItemId);
                                price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                            }
                        }
                    }
                    break;
            }
            total = price * Qty;

            Query = "UPDATE [dbo].[GoodsIssuedD] SET [GoodsIssuedDate] = '" + GoodsIssuedDate + "',[No_SJ] = '" + No_SJ + "' ,[RefTransID] = '" + RefTransID + "',[GroupId] = '" + GroupId + "',[SubGroup1Id] = '" + SubGroup1Id + "',[SubGroup2Id] = '" + SubGroup2Id + "',[ItemId] = '" + ItemId + "',[FullItemId] = '" + FullItemId + "',[ItemName] = '" + ItemName + "',[Qty] = '" + Qty + "',[Unit] = '" + Unit + "',[Ratio] = '" + Ratio + "',[TotalBerat] = '" + TotalBerat + "',[InventSiteId] = '" + InventSiteID + "',[InventSiteBlokID] = '" + InventSiteBlokID + "',[Notes] = @NotesD,[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "', price = '" + price + "', Total = '" + total + "', Total_Discount = '" + totalDisc + "', PPN = '" + ppn + "', Total_PPN = '" + totalPPN + "', pph = '" + pph + "', total_PPH = '" + totalPPH + "' WHERE [GoodsIssuedId] = '" + GoodsIssuedId + "' and[GoodsIssuedSeqNo] = '" + GoodsIssuedSeqNo + "'";
            Cmd = new SqlCommand("Select StatusCode from [dbo].[GoodsIssuedH] where GoodsIssuedId = '" + tbxBBKID.Text + "'", Conn);
            if (ControlMgr.GroupName == "Sales Admin" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
            {
                Query += "Update GoodsIssuedD set Qty_Actual = '" + Qty_Actual + "', TotalBerat_Actual = '" + TotalBerat_Actual + "',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' WHERE [GoodsIssuedId] = '" + GoodsIssuedId + "' and[GoodsIssuedSeqNo] = '" + GoodsIssuedSeqNo + "'";
            }
            else if (ControlMgr.GroupName == "KERANI" && (Cmd.ExecuteScalar().ToString() == "01" || Cmd.ExecuteScalar().ToString() == "02"))
            {
                using (Cmd = new SqlCommand("select StatusCode from TransStatusTable where TransCode = 'GID' and Deskripsi = '" + ActionCode + "'", Conn))
                {
                    Query += "Update GoodsIssuedD set Qty_Actual = '" + Qty_Actual + "', TotalBerat_Actual = '" + TotalBerat_Actual + "', ActionCode = '" + Cmd.ExecuteScalar().ToString() + "',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' WHERE [GoodsIssuedId] = '" + GoodsIssuedId + "' and[GoodsIssuedSeqNo] = '" + GoodsIssuedSeqNo + "'";
                }
            }
            //Insert available and reserved qty at WB2
            //Begin , Created BY Thaddaeus, 21 May2018
            else if (cbRef.Text == "Nota Transfer" && ((ControlMgr.GroupName == "WB OPERATOR" && Cmd.ExecuteScalar().ToString() == "03") || (ControlMgr.GroupName.ToUpper() == "SITE MANAGER" && Cmd.ExecuteScalar().ToString() == "06")))
            {
                decimal ReservedQty = 0;
                string Query2 = "SELECT SUM(RemainingReserved) FROM [NotaTransferD] WHERE [TransferNo] = '" + tbxRefID.Text + "' AND [FullItemId] = '" + FullItemId + "'";
                using (Cmd = new SqlCommand(Query2, Conn))
                {
                    if (Cmd.ExecuteScalar() == System.DBNull.Value)
                    {
                        Query2 = "SELECT SUM(NT_Reserved_Qty) FROM [NotaTransferD] WHERE [TransferNo] = '" + tbxRefID.Text + "' AND [FullItemId] = '" + FullItemId + "'";
                        using (SqlCommand Cmd2 = new SqlCommand(Query2, Conn))
                        {
                            ReservedQty = Convert.ToDecimal(Cmd2.ExecuteScalar());
                        }
                    }
                    else if (Convert.ToDecimal(Cmd.ExecuteScalar()) != 0)
                    {
                        ReservedQty = Convert.ToDecimal(Cmd.ExecuteScalar()) + Qty_Actual;
                    }
                    else
                    {
                        ReservedQty = RemainingReserved[i];
                    }
                }
                if (ReservedQty >= Qty_Actual)
                {
                    Query += " UPDATE GoodsIssuedD SET [Available_Qty]= 0 , [Reserved_Qty]=" + Qty_Actual + " WHERE [GoodsIssuedId] = '" + GoodsIssuedId + "' and[GoodsIssuedSeqNo] = '" + GoodsIssuedSeqNo + "' ";
                }
                else
                {
                    Query += " UPDATE GoodsIssuedD SET [Available_Qty]=" + (Qty_Actual - ReservedQty) + ", [Reserved_Qty]=" + ReservedQty + " WHERE [GoodsIssuedId] = '" + GoodsIssuedId + "' and[GoodsIssuedSeqNo] = '" + GoodsIssuedSeqNo + "' ";
                }
            }
            //end=========================================
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NotesD", NotesD);
            Cmd.ExecuteNonQuery();
        }

        private void updateGoodsIssuesH(string StatusCode, string InventSiteID, string VehicleOwnerID, string VehicleOwnerName, string VehicleType, string VehicleNumber, string DriverName, string StatusWeight1, DateTime Timbang1Date, decimal Timbang1Weight, string Notes, string GoodsIssuedId, string StatusWeight2, DateTime Timbang2Date, decimal Timbang2Weight, string No_SJ)
        {
            Query = "UPDATE [dbo].[GoodsIssuedH] SET [StatusCode] = '" + StatusCode + "',[InventSiteID] = '" + InventSiteID + "',[VehicleOwnerID] = '" + VehicleOwnerID + "',[VehicleOwnerName] = '" + VehicleOwnerName + "',[VehicleType] = @VehicleType,[VehicleNumber] = @VehicleNumber,[DriverName] = @DriverName,[StatusWeight1] = '" + StatusWeight1 + "',[Timbang1Date] = '" + Timbang1Date + "',[Timbang1Weight] = '" + Timbang1Weight + "',[Notes] = Notes,[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' WHERE GoodsIssuedId = '" + GoodsIssuedId + "'";
            Cmd = new SqlCommand("Select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'", Conn);
            if (ControlMgr.GroupName == "WB OPERATOR" && (Cmd.ExecuteScalar().ToString() == "02" || Cmd.ExecuteScalar().ToString() == "03"))
            {
                Query += "update GoodsIssuedH set No_SJ = '" + No_SJ + "', StatusWeight2 = '" + StatusWeight2 + "', Timbang2Date = '" + Timbang2Date + "', Timbang2Weight = '" + Timbang2Weight + "',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' WHERE GoodsIssuedId = '" + GoodsIssuedId + "'";
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@VehicleType", VehicleType);
            Cmd.Parameters.AddWithValue("@VehicleNumber", VehicleNumber);
            Cmd.Parameters.AddWithValue("@DriverName", DriverName);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.ExecuteNonQuery();
        }

        private void insertGoodsIssuedH(DateTime GoodsIssuedDate, string GoodsIssuedId, string StatusCode, string AccountNum, string AccountName, string RefTransType, string RefTransID, DateTime RefTransDate, string InventSiteID, string VehicleOwnerID, string VehicleOwnerName, string VehicleType, string VehicleNumber, string DriverName, string StatusWeight1, DateTime Timbang1Date, decimal Timbang1Weight, string Notes)
        {
            Query = "INSERT INTO [dbo].[GoodsIssuedH] ([GoodsIssuedDate] ,[GoodsIssuedId] ,[StatusCode] ,[AccountNum] ,[AccountName] ,[RefTransType] ,[RefTransID] ,[RefTransDate] ,[InventSiteID] ,[VehicleOwnerID] ,[VehicleOwnerName] ,[VehicleType] ,[VehicleNumber] ,[DriverName] ,[StatusWeight1] ,[Timbang1Date] ,[Timbang1Weight] ,[Notes] ,[CreatedDate] ,[CreatedBy], UpdatedDate) VALUES ('" + GoodsIssuedDate + "', '" + GoodsIssuedId + "', '" + StatusCode + "', '" + AccountNum + "', '" + AccountName + "', '" + RefTransType + "', '" + RefTransID + "', '" + RefTransDate + "', '" + InventSiteID + "', '" + VehicleOwnerID + "', '" + VehicleOwnerName + "', @VehicleType, @VehicleNumber, @DriverName, '" + StatusWeight1 + "', '" + Timbang1Date + "', '" + Timbang1Weight + "', @Notes, GetDate(), '" + ControlMgr.UserId + "', '1753-01-01')";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@VehicleType", VehicleType);
            Cmd.Parameters.AddWithValue("@VehicleNumber", VehicleNumber);
            Cmd.Parameters.AddWithValue("@DriverName", DriverName);
            Cmd.Parameters.AddWithValue("@Notes", Notes);
            Cmd.ExecuteNonQuery();
        }

        private void insertGoodsIssuedD(DateTime GoodsIssuedDate, string GoodsIssuedId, int GoodsIssuedSeqNo, string RefTransID, int RefTransSeqNo, string GroupId, string SubGroup1Id, string SubGroup2Id, string ItemId, string FullItemId, string ItemName, decimal Qty, string Unit, decimal Ratio, decimal TotalBerat, string InventSiteID, string InventSiteBlokID, string NotesD)
        {
            decimal price = 0;
            decimal total = 0;
            decimal totalDisc = 0;
            decimal ppn = 0;
            decimal totalPPN = 0;
            decimal pph = 0;
            decimal totalPPH = 0;
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    Query = "select b.Price, b.DiscPercent, c.PPN, c.PPH from DeliveryOrderD a left join SalesOrderD b on a.SalesOrderId = b.SalesOrderNo and a.SalesOrderSeqNo = b.SeqNo left join SalesOrderH c on b.SalesOrderNo = c.SalesOrderNo where a.DeliveryOrderId = '" + RefTransID + "' and a.SeqNo = " + RefTransSeqNo;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        price = Convert.ToDecimal(Dr["Price"]);
                        totalDisc = price * Convert.ToDecimal(Dr["DiscPercent"]) / 100;
                        ppn = Convert.ToDecimal(Dr["PPN"]);
                        totalPPN = (price - totalDisc) * ppn / 100;
                        pph = Convert.ToDecimal(Dr["PPH"]);
                        totalPPH = (price - totalDisc) * pph / 100;
                    }
                    Dr.Close();
                    break;
                case "Nota Transfer":
                    Query = "select COGS from NotaTransferD where TransferNo = '" + RefTransID + "' and SeqNo = " + RefTransSeqNo;
                    Cmd = new SqlCommand(Query, Conn);
                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    break;
                case "Nota Retur Beli":
                    Query = "select b.RefTransID from NotaReturBeli_Dtl a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId and a.GoodsReceived_SeqNo = b.GoodsReceivedSeqNo where a.NRBId = '" + RefTransID + "' and a.SeqNo = " + RefTransSeqNo;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Dr["RefTransID"].ToString().Split('/')[0] == "RO" || Dr["RefTransID"].ToString().Split('/')[0] == "ROA")
                        {
                            Query = "select b.Price, b.Diskon, c.PPN, c.PPH from ReceiptOrderD a left join PurchDtl b on a.PurchaseOrderId = b.PurchID and a.PurchaseOrderSeqNo = b.SeqNo left join PurchH c on b.PurchID = c.PurchID where a.ReceiptOrderId = '" + RefTransID + "' and a.SeqNo = " + RefTransSeqNo;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr2["Diskon"]) / 100;
                                ppn = Convert.ToDecimal(Dr2["PPN"]);
                                totalPPN = (price - totalDisc) * Convert.ToDecimal(Dr2["PPN"]) / 100;
                                pph = Convert.ToDecimal(Dr2["PPH"]);
                                totalPPH = (price - totalDisc) * Convert.ToDecimal(Dr2["PPH"]) / 100;
                            }
                            Dr2.Close();
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NT")
                        {
                            Query = "select COGS from NotaTransferD where TransferNo = '" + RefTransID + "' and SeqNo = " + RefTransSeqNo;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NRJ")
                        {
                            Query = "select d.Price, d.DiscPercent, e.PPN, e.PPH from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + RefTransID + "' and a.SeqNo = " + RefTransSeqNo;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr2["DiscPercent"]) / 100;
                                ppn = Convert.ToDecimal(Dr2["PPN"]);
                                totalPPN = (price - totalDisc) * ppn / 100;
                                pph = Convert.ToDecimal(Dr2["PPH"]);
                                totalPPH = (price - totalDisc) * pph / 100;
                            }
                            Dr2.Close();
                        }
                        else
                        {
                            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            {
                                Cmd2.Parameters.AddWithValue("@FullItemID", FullItemId);
                                price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                            }
                        }
                    }
                    break;
                case "Nota Retur Jual":
                    Query = "select b.RefTransID from NotaReturJual_Dtl a left join [GoodsIssuedD] b on a.[GoodsIssuedId] = b.[GoodsIssuedId] and a.[GoodsIssued_SeqNo] = b.[GoodsIssuedSeqNo] where a.NRJId = '" + RefTransID + "' and a.SeqNo = " + RefTransSeqNo;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Dr["RefTransID"].ToString().Split('/')[0] == "DO" || Dr["RefTransID"].ToString().Split('/')[0] == "DOA")
                        {
                            Query = "select b.Price, b.[DiscPercent], c.PPN, c.PPH from [DeliveryOrderD] a left join [SalesOrderD] b on a.[SalesOrderId] = b.[SalesOrderNo] and a.[SalesOrderSeqNo] = b.SeqNo left join [SalesOrderH] c on b.[SalesOrderNo] = c.[SalesOrderNo] where a.[DeliveryOrderId] = '" + RefTransID + "' and a.SeqNo = " + RefTransSeqNo;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr2["DiscPercent"]) / 100;
                                ppn = Convert.ToDecimal(Dr2["PPN"]);
                                totalPPN = (price - totalDisc) * Convert.ToDecimal(Dr2["PPN"]) / 100;
                                pph = Convert.ToDecimal(Dr2["PPH"]);
                                totalPPH = (price - totalDisc) * Convert.ToDecimal(Dr2["PPH"]) / 100;
                            }
                            Dr2.Close();
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NT")
                        {
                            Query = "select COGS from NotaTransferD where TransferNo = '" + RefTransID + "' and SeqNo = " + RefTransSeqNo;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NRB")
                        {
                            Query = "select d.Price, d.Diskon, e.PPN, e.PPH from NotaReturBeli_Dtl a left join [GoodsReceivedD] b on a.[GoodsReceivedId] = b.[GoodsReceivedId] and a.[GoodsReceived_SeqNo] = b.[GoodsReceivedSeqNo] left join [ReceiptOrderD] c on b.[RefTransID] = c.[ReceiptOrderId] and b.[RefTransSeqNo] = c.SeqNo left join [PurchDtl] d on c.[PurchaseOrderId] = d.[PurchID] and c.[PurchaseOrderSeqNo] = d.SeqNo left join [PurchH] e on d.[PurchID] = e.[PurchID] where a.NRBId = '" + RefTransID + "' and a.SeqNo = " + RefTransSeqNo;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr2["Diskon"]) / 100;
                                ppn = Convert.ToDecimal(Dr2["PPN"]);
                                totalPPN = (price - totalDisc) * ppn / 100;
                                pph = Convert.ToDecimal(Dr2["PPH"]);
                                totalPPH = (price - totalDisc) * pph / 100;
                            }
                            Dr2.Close();
                        }
                        else
                        {
                            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            {
                                Cmd2.Parameters.AddWithValue("@FullItemID", FullItemId);
                                price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                            }
                        }
                    }
                    break;
            }
            total = price * Qty;

            Query = "INSERT INTO [dbo].[GoodsIssuedD] ([GoodsIssuedDate] ,[GoodsIssuedId] ,[GoodsIssuedSeqNo] ,[RefTransID] ,[RefTransSeqNo] ,[GroupId] ,[SubGroup1Id] ,[SubGroup2Id] ,[ItemId] ,[FullItemId] ,[ItemName] ,[Qty] ,[Unit] ,[Ratio] ,[TotalBerat] ,[Remaining_Qty] ,[InventSiteId] ,[InventSiteBlokID] ,[Notes] ,[CreatedDate] ,[CreatedBy], UPDATEDdate, price, total, total_Discount, PPN, Total_PPN, PPH, Total_PPH) VALUES ( '" + GoodsIssuedDate + "', '" + GoodsIssuedId + "', '" + GoodsIssuedSeqNo + "', '" + RefTransID + "', '" + RefTransSeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "', '" + ItemName + "', '" + Qty + "', '" + Unit + "', '" + Ratio + "', '" + TotalBerat + "', '" + Qty + "', '" + InventSiteID + "', '" + InventSiteBlokID + "', @NotesD, getdate(), '" + ControlMgr.UserId + "', '1753-01-01', '" + price + "', '" + total + "', '" + totalDisc + "', '" + ppn + "', '" + totalPPN + "', '" + pph + "', '" + totalPPH + "')";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NotesD", NotesD);
            Cmd.ExecuteNonQuery();
        }

        private void cbRef_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                if (cbRef.Text != "Select")
                {
                    btnSRefID.Enabled = true;
                }

                if (cbRef.Text == "Select")
                {
                    tabPage2.Text = "Detail Ref";
                    ResetForm();
                }
                else if (cbRef.Text == "Delivery Order")
                {
                    tabPage2.Text = "Detail Delivery Order";
                    ResetForm();
                }
                else if (cbRef.Text == "Nota Transfer")
                {
                    tabPage2.Text = "Detail Nota Transfer";
                    ResetForm();
                }
                else if (cbRef.Text == "Nota Retur Beli")
                {
                    tabPage2.Text = "Detail Nota Retur Beli";
                    ResetForm();
                }
                else if (cbRef.Text == "Nota Retur Jual")
                {
                    tabPage2.Text = "Detail Nota Retur Jual";
                    ResetForm();
                }
            }
        }

        private void ResetForm()
        {
            tbxRefID.Text = "";
            tbxNameID.Text = "";
            tbxName.Text = "";
            tbxVOwnerID.Text = "";
            tbxVOwner.Text = "";
            tbxVType.Text = "";
            tbxVNumber.Text = "";
            tbxDriverName.Text = "";
            tbxInventSiteID.Text = "";
            tbxWarehouse.Text = "";
            tbxLocation.Text = "";
            tbxNotes.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnNT.Enabled = false;
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
                GetDataHeader2();
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
            GetDataHeader2();
            ModeBeforeEdit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            decimal ratio = 0, qty = 0, totalBerat = 0;
            if (tableCols[e.ColumnIndex] == "Qty")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value != null)
                {
                    qty = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                    ratio = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value);
                    totalBerat = qty * ratio;
                    dataGridView1.Rows[e.RowIndex].Cells["TotalBerat"].Value = totalBerat.ToString();
                }
            }
            else if (tableCols[e.ColumnIndex] == "Qty_Actual")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value != null)
                {
                    qty = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                    ratio = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value);
                    totalBerat = qty * ratio;
                    dataGridView1.Rows[e.RowIndex].Cells["TotalBerat_Actual"].Value = totalBerat.ToString();
                }
            }
            if (tableCols[e.ColumnIndex] == "Qty" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
            {
                decimal beratTotal = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    beratTotal += Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat"].Value);
                }
                tbxWeight2.Text = beratTotal.ToString();
                tbxWeight1.Text = beratTotal.ToString();
            }
        }

        private void tbxWeight1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            if (tableCols[e.ColumnIndex] == "Qty" || tableCols[e.ColumnIndex] == "Qty_Actual")
            {
                if (e.Value == null)
                    e.Value = "0";
                if (e.Value.ToString() == "")
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            if (tableCols[e.ColumnIndex] == "Ratio" || tableCols[e.ColumnIndex] == "TotalBerat" || tableCols[e.ColumnIndex] == "TotalBerat_Actual")
            {
                if (e.Value == null)
                    e.Value = "0";
                if (e.Value.ToString() == "")
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }
            if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
            {
                decimal berattotal = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells["GoodsIssuedSeqNo"].Value = i + 1;
                    berattotal += Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value);
                }
                tbxWeight1.Text = berattotal.ToString();
                tbxWeight2.Text = berattotal.ToString();
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Qty_Actual") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("TotalBerat_Actual"))
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("Check");
            switch (cbRef.Text)
            {
                case "Delivery Order":
                    f.SetSchemaTable("dbo", "DeliveryOrderD", "and a.DeliveryOrderId = '" + tbxRefID.Text + "'", "a.*", "DeliveryOrderD a");
                    break;
                case "Nota Transfer":
                    f.SetSchemaTable("dbo", "NotaTransferD", "and a.TransferNo = '" + tbxRefID.Text + "'", "a.*", "NotaTransferD a");
                    break;
                case "Nota Retur Beli":
                    f.SetSchemaTable("dbo", "NotaReturBeli_Dtl", "and a.NRBId = '" + tbxRefID.Text + "'", "a.*", "NotaReturBeli_Dtl a");
                    break;
                case "Nota Retur Jual":
                    f.SetSchemaTable("dbo", "NotaReturJual_Dtl", "and a.NRJId = '" + tbxRefID.Text + "'", "a.*", "NotaReturJual_Dtl a");
                    break;
            }
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                string combineID = "";
                for (int i = 0; i < SearchV2.data.Count; i++)
                {
                    if (i >= 1)
                        combineID += ", ";
                    combineID += "'" + SearchV2.data[i] + "'";
                }
                string combineNo = "";
                for (int i = 0; i < SearchV2.data2.Count; i++)
                {
                    if (i >= 1)
                        combineNo += ", ";
                    combineNo += "'" + SearchV2.data2[i] + "'";
                }
                //ROWS
                switch (cbRef.Text)
                {
                    case "Delivery Order":
                        GenerateGVRows("Select * from DeliveryOrderD where DeliveryOrderId in (" + combineID + ") and SeqNo in (" + combineNo + ")");
                        break;
                    case "Nota Transfer":
                        GenerateGVRows("Select * from NotaTransferD where TransferNo in (" + combineID + ") and SeqNo in (" + combineNo + ")");
                        break;
                    case "Nota Retur Beli":
                        GenerateGVRows("Select * from NotaReturBeli_Dtl where NRBId in (" + combineID + ") and SeqNo in (" + combineNo + ")");
                        break;
                    case "Nota Retur Jual":
                        GenerateGVRows("Select * from NotaReturJual_Dtl where NRJId in (" + combineID + ") and SeqNo in (" + combineNo + ")");
                        break;
                }
            }
        }

        private void cbWeight2_CheckedChanged(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "select Timbang2Weight from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            if (Cmd.ExecuteScalar() == null)
                tbxWeight2.Text = "0.0000";
            else if (Cmd.ExecuteScalar().ToString() == String.Empty)
                tbxWeight2.Text = "0.0000";
            else
                tbxWeight2.Text = Cmd.ExecuteScalar().ToString();
            if (cbWeight2.Checked == true)
            {
                tbxWeight2.Enabled = true;
                btnWeight2.Enabled = false;
            }
            else
            {
                tbxWeight2.Enabled = false;
                btnWeight2.Enabled = true;
            }
            Conn.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //Conn = ConnectionString.GetConnection();
            //Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + tbxBBKID.Text + "'";
            //Cmd = new SqlCommand(Query, Conn);
            //if (Cmd.ExecuteScalar().ToString() == "01")
            //    MetroFramework.MetroMessageBox.Show(this, "Printing " + tbxBBKID.Text + " Ticket!\r\n Only info. still not processed", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //else if (Cmd.ExecuteScalar().ToString() == "03")
            //    MetroFramework.MetroMessageBox.Show(this, "Printing " + tbxSJ.Text + "!\r\n Only info. still not processed", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //Conn.Close();

            string BBKid = tbxBBKID.Text;
            string recipientId = tbxName.Text;

            if (rbTicket.Checked == true)
            {
                ISBS_New.GlobalPreview f = new ISBS_New.GlobalPreview("Goods Issued", BBKid);
                f.SetMode("Ticket");
                f.Show();
            }
            else if (rbSuratJalan.Checked == true)
            {
                ISBS_New.GlobalPreview f = new ISBS_New.GlobalPreview("Goods Issued", BBKid);
                f.SetMode("Surat Jalan");
                f.Show();
            }
            else MessageBox.Show("Pilih Tiket / Surat Jalan");
        }

        private void btnNT_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nota Transfer feature coming soon!", "Information");
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView2.ReadOnly = true;
            dataGridView2.DefaultCellStyle.BackColor = Color.LightGray;
            for (int i = 0; i < dataGridView2.ColumnCount; i++)
                dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Mode == "New" || Mode == "Edit")
            {
                if (tabControl1.SelectedIndex == 1)
                {
                    btnAdd.Enabled = false;
                    btnDelete.Enabled = false;
                }
                else
                {
                    btnAdd.Enabled = true;
                    btnDelete.Enabled = true;
                }
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.PermissionAccess(ControlMgr.Approve) > 0)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        //UPDATE GI STATUS TO APPROVED
                        Query = "Update GoodsIssuedH set StatusCode = '06', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where GoodsIssuedId = '" + tbxBBKID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        #region variable
                        string GoodsIssuedId = tbxBBKID.Text;
                        DateTime GoodsIssuedDate = dtBBK.Value;
                        string RefTransType = cbRef.Text;
                        string RefTransID = tbxRefID.Text;
                        DateTime RefTransDate = dtRef.Value;
                        string InventSiteID = tbxInventSiteID.Text;
                        string VehicleOwnerID = tbxVOwnerID.Text;
                        string VehicleOwnerName = tbxVOwner.Text;
                        #endregion
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            #region variable
                            int GoodsIssuedSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["GoodsIssuedSeqNo"].Value);
                            int RefTransSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["SeqNo"].Value);
                            string GroupId = dataGridView1.Rows[i].Cells["GroupId"].Value.ToString();
                            string SubGroup1Id = dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString();
                            string SubGroup2Id = dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString();
                            string ItemId = dataGridView1.Rows[i].Cells["ItemId"].Value.ToString();
                            string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                            string ItemName = dataGridView1.Rows[i].Cells["ItemName"].Value.ToString();
                            decimal Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value);
                            string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();

                            decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value == null ? 1 : dataGridView1.Rows[i].Cells["Ratio"].Value);
                            decimal TotalBerat = Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat"].Value);
                            string InventSiteBlokID = dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value.ToString();
                            if (dataGridView1.Rows[i].Cells["Notes"].Value == null)
                                dataGridView1.Rows[i].Cells["Notes"].Value = "";
                            string NotesD = dataGridView1.Rows[i].Cells["Notes"].Value.ToString();

                            decimal Qty_Actual = 0;
                            if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
                                Qty_Actual = 0;
                            else
                                Qty_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                            decimal TotalBerat_Actual = 0;
                            if (dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() == String.Empty)
                                TotalBerat_Actual = 0;
                            else
                                TotalBerat_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value);
                            string ActionCode = dataGridView1.Rows[i].Cells["ActionCode"].Value.ToString();
                            #endregion

                            if (cbRef.Text != "Nota Retur Beli")
                            {
                                //INSERT TO INVENT TRANS
                                insertInventTrans(GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, InventSiteID, GoodsIssuedId, GoodsIssuedSeqNo, GoodsIssuedDate, RefTransID, RefTransDate, RefTransSeqNo, VehicleOwnerID, VehicleOwnerName, Qty_Actual, NotesD);

                                //REDUCE INVENT STOCK
                                insertInventOnHandQty(FullItemId, InventSiteID, Qty_Actual, Ratio, RefTransID, RefTransSeqNo, GoodsIssuedSeqNo);
                            }
                            

                            if (cbRef.Text == "Delivery Order")
                            {
                                //UPDATE DO: DO Issued Outstanding
                                updateDOIssuedOuts(FullItemId, Qty, Qty * Ratio);
                            }
                            else if (cbRef.Text == "Nota Transfer")
                            {
                                //UPDATE Nota Transfer: Transfer in Progress, Transfer Out in Progress
                                updateTransferInProgress_TransferOutProgress(RefTransID, RefTransSeqNo, FullItemId, Qty, Qty * Ratio);
                            }
                            else if (cbRef.Text == "Nota Retur Beli")
                            {
                                //UPDATE NRB: PO Issued Outstanding, Retur Beli In Progress
                                updatePOIssuedOuts_NRBInProgress(RefTransID, RefTransSeqNo, FullItemId, Qty, Qty * Ratio);
                            }
                            else if (cbRef.Text == "Nota Retur Jual")
                            {
                                updateNRJInProgress(RefTransID, RefTransSeqNo, FullItemId, Qty, Qty * Ratio);
                            }
                        }

                        //Begin
                        //Created By : Joshua
                        //Created Date ; 24 Aug 2018
                        //Desc : Create Journal
                        CreateJournal();
                        if (Journal == true)
                        {
                            Journal = false;
                            goto Outer;
                        }
                        //End

                        Conn.Close();
                        scope.Complete();
                    }
                    Parent.RefreshGrid();
                    btnApprove.Enabled = false;
                    MetroFramework.MetroMessageBox.Show(this, "Approve berhasil! Tolong beri tau WB untuk aksi selanjutnya!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Outer: ;
                }
                else
                    MessageBox.Show(ControlMgr.PermissionDenied);
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbxWeight2_KeyPress(object sender, KeyPressEventArgs e)
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

        private void tbxWeight2_Leave(object sender, EventArgs e)
        {
            if (tbxWeight2.Text == String.Empty)
                tbxWeight2.Text = "0";
            tbxWeight2.Text = string.Format("{0:#,0.0000}", double.Parse(tbxWeight2.Text));
        }

        private void tbxWeight1_Leave(object sender, EventArgs e)
        {
            if (tbxWeight1.Text == String.Empty)
                tbxWeight1.Text = "0";
            tbxWeight1.Text = string.Format("{0:#,0.0000}", double.Parse(tbxWeight1.Text));
        }
        //tia edit
        //klik kanan
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.CustomerID.Customer Cust = null;
        Sales.DeliveryOrder.DOHeader DeliveryOrder = null;
        PopUp.Vendor.Vendor Vend = null;
        Purchase.NotaReturBeli.ReturBeliHeader NrbId = null;

        TaskList.GlobalTasklist ParentToTLDO;
        AccountAssignment.GLJournal.FormGLJournalHeader ParentToGL;
        Sales.NotaReturJual.NRJHeader ParentToNRJ;
        Sales.NotaReturJual.NRJApproval ParentToNRJA;

        public void SetParent2(TaskList.GlobalTasklist TlDo)
        {
            ParentToTLDO = TlDo;
        }

        public void ParentRefreshGrid(Sales.NotaReturJual.NRJHeader nrj)
        {
            ParentToNRJ = nrj;
        }

        public void ParentRefreshGrid2(Sales.NotaReturJual.NRJApproval nrja)
        {
            ParentToNRJA = nrja;
        }

        //public void ParentRefreshGrid3(TaskList.Sales.TaskListBBK TLBBK)
        //{
        //    PArentToTLBBK = TLBBK;
        //}

        public void ParentRefreshGrid4(AccountAssignment.GLJournal.FormGLJournalHeader BBKID)
        {
            ParentToGL = BBKID;
        }

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

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

        private bool CekNRJKembalikanBarang(string NRJID, int SeqNo, SqlConnection Con)
        {
            bool stat = false;
            Query = "SELECT [ActionCode] FROM [NotaReturJual_Dtl] WHERE [NRJId] = @NRJId AND [SeqNo] = @SeqNo";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@NRJId", NRJID);
                Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                if (Cmd.ExecuteScalar().ToString() == "03")
                {
                    stat = true;
                }
            }
            return stat;
        }

        private bool CekNRBKembalikanBarang(string NRBID, int SeqNo, SqlConnection Con)
        {
            bool stat = false;
            Query = "SELECT [ActionCode] FROM [NotaReturBeli_Dtl] WHERE [NRBId] = @NRBId AND [SeqNo] = @SeqNo";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Cmd.Parameters.AddWithValue("@NRBId", NRBID);
                Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                if (Cmd.ExecuteScalar().ToString() == "03")
                {
                    stat = true;
                }
            }
            return stat;
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

        private void tbxNameID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    tbxNameID.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(tbxNameID.Text);
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

        private void tbxName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    tbxNameID.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(tbxNameID.Text);
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

        private void tbxRefID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (cbRef.Text == "Delivery Order")
                {
                    if (DeliveryOrder == null || DeliveryOrder.Text == "")
                    {
                        tbxRefID.Enabled = true;
                        DeliveryOrder = new Sales.DeliveryOrder.DOHeader();
                        DeliveryOrder.SetMode("PopUp", tbxRefID.Text);
                        DeliveryOrder.ParentRefreshGrid(this);
                        DeliveryOrder.Show();
                    }
                    else if (CheckOpened(DeliveryOrder.Name))
                    {
                        DeliveryOrder.WindowState = FormWindowState.Normal;
                        DeliveryOrder.Show();
                        DeliveryOrder.Focus();
                    }
                }
                else if (cbRef.Text == "Nota Retur Beli")
                {
                    if (NrbId == null || NrbId.Text == "")
                    {
                        tbxRefID.Enabled = true;
                        NrbId = new Purchase.NotaReturBeli.ReturBeliHeader();
                        NrbId.SetMode("PopUp", tbxRefID.Text);
                        NrbId.ParentRefreshGrid3(this);
                        NrbId.Show();
                    }
                    else if (CheckOpened(NrbId.Name))
                    {
                        NrbId.WindowState = FormWindowState.Normal;
                        NrbId.Show();
                        NrbId.Focus();
                    }
                }

            }
        }

        private void tbxVOwnerID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    tbxVOwnerID.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(tbxVOwnerID.Text);
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

        private void tbxVOwner_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    tbxVOwner.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(tbxVOwnerID.Text);
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
        //end

        
    }
}
