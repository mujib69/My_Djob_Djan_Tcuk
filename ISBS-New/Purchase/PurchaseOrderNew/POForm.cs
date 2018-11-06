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

namespace ISBS_New.Purchase.PurchaseOrderNew
{
    public partial class POForm : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        //private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataReader DrD;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query, Query1, crit = null;

        List<string> ListSeqNoDelete = new List<string>();

        List<string> sSelectedFile, FileName, Extension;
        List<byte[]> test = new List<byte[]>();
        List<decimal> HitungAmount = new List<decimal>();
        List<string> TampungIdItem = new List<string>();


        int Index, SelectedCell;

        public string PONumber = "", tmpPOType = "", CanvasId = "";

        DateTimePicker dtp;
        ComboBox InventSite;
        ComboBox DeliveryMethod;

        List<string> ListSeqNum;
        List<Purchase.PurchaseRequisition.Info> ListInfo = new List<Purchase.PurchaseRequisition.Info>();
        PopUp.Stock.Stock PopUpItemName = new PopUp.Stock.Stock();
        PopUp.FullItemId.FullItemId FullItemId = new PopUp.FullItemId.FullItemId();
        PopUp.Vendor.Vendor VendorId = new PopUp.Vendor.Vendor();

        Purchase.PurchaseOrderNew.POInquiry Parent;

        //Insert LogsTable
        DateTime PaDate;
        Decimal QtyUoM = 0;
        Decimal QtyAlt = 0;
        Decimal QtyAmount = 0;
        //Insert LogsTable

        DataGridViewComboBoxCell cell;
        DataGridViewComboBoxCell cmbDeliveryMethod;

        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public POForm()
        {
            InitializeComponent();
            //if (txtCurrencyID.Right=)
            //{

            //}
        }

        private void POForm_Load(object sender, EventArgs e)
        {
            //BY: HC (S)
            cmbDPType.SelectedIndex = 0;
            tbxDPAmount.Visible = false;
            //BY: HC (E)
            //lblForm.Location = new Point(16, 11);
            cmbPPn_Load();
            cmbPPh_Load();
            AddCmbCurrency();
            AddCmbTermOfPayment();
            AddCmbPaymentMode();
            txtVendName.Visible = true;
            lblReffID2.Visible = false;
            txtReffID2.Visible = false;
            lblDueDate.Visible = false;
            dtDueDate.Visible = false;
            lblTransType.Visible = false;
            txtTransType.Visible = false;
            txtCurrencyID.Visible = false;
            btnCurrency.Visible = false;

            if (Mode == "New")
            {
                ModeNew();
                cmbReffTableName.Items.Remove("Purchase Order");
                btnClose.Visible = false;
                txtExchRate_SetText();
            }
            else if (Mode == "Generate")
            {
                ModeNew();
                cmbReffTableName.Text = "Canvass Sheet";
                txtReffID.Text = CanvasId;
                btnClose.Visible = false;
                cmbReffTableName.Enabled = false;
                CallFormCSId(CanvasId);
                cmbCurrency.Enabled = false;
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
                GetDataHeader();
            }
            else if (Mode == "BeforeEdit")
            {
                txtPOId.Text = PONumber;
                ModeBeforeEdit();
                GetDataHeader();
                BeforeEditdgvAttachment();
            }
            else if (Mode == "Amend")
            {
                ModeAmend();
                cmbReffTableName.Items.Add("Purchase Order");
                cmbReffTableName.Text = "Purchase Order";
                txtReffID.Text = PONumber;
                txtExchRate_SetText();
                cmbCurrency.SelectedItem = "IDR";
                GetDataHeader();
                amendSetting();
                BeforeEditdgvAttachment();
            }
            //tia edit
            else if (Mode == "PopUp")
            {
                ModePopUp();
                GetDataHeader();

            }
            //tia edit end

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy";
            dtp.Visible = false;
            dtp.Width = 100;

            //STEVEN EDIT BEGIN   
            txtExchRate.TextAlign = HorizontalAlignment.Right;
            this.txtExchRate.Enter += new EventHandler(txtExchRate_Enter);
            this.txtExchRate.Leave += new EventHandler(txtExchRate_Leave);
            //STEVEN EDIT END

            dgvPODetails1.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;
            dgvPODetails1.CellBeginEdit += this.dgvPODetails1_CellBeginEdit;
            dgvPODetails1.CellEndEdit += this.dgvPODetails1_CellEndEdit;

            InventSite = new ComboBox();
            InventSite.DropDownStyle = ComboBoxStyle.DropDownList;
            InventSite.Visible = false;
            dgvPODetails1.Controls.Add(InventSite);
            InventSite.SelectedIndexChanged += this.InventSite_SelectedIndexChanged;
            InventSite.DropDownClosed += this.InventSite_DropDownClosed;

            DeliveryMethod = new ComboBox();
            DeliveryMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            DeliveryMethod.Visible = false;
            dgvPODetails1.Controls.Add(DeliveryMethod);
            DeliveryMethod.DropDownClosed += this.DeliveryMethod_DropDownClosed;
            DeliveryMethod.SelectionChangeCommitted += this.DeliveryMethod_SelectionChangeCommitted;

        }

        private void AddCmbCurrency()
        {
            cmbCurrency.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select CurrencyID from [dbo].[CurrencyTable]", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbCurrency.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();

            if (cmbCurrency.Items.Contains("IDR"))
            {
                cmbCurrency.SelectedItem = "IDR";
            }
        }

        private void AddCmbTermOfPayment()
        {
            cmbTermOfPayment.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [TermOfPayment] from TermOfPayment", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbTermOfPayment.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
        }

        private void AddCmbPaymentMode()
        {
            cmbPaymentMode.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [PaymentModeName] from [dbo].[PaymentMode]", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbPaymentMode.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
        }

        //START STEVEN EDIT (SET DEFAULT TEXT txtExchRate)
        protected void txtExchRate_SetText()
        {
            this.txtExchRate.Text = "1.00";
            txtExchRate.ForeColor = Color.Gray;
        }

        private void txtExchRate_Enter(object sender, EventArgs e)
        {
            if (txtExchRate.ForeColor != Color.Black)
                txtExchRate.Text = "";
            txtExchRate.ForeColor = Color.Black;
        }
        private void txtExchRate_Leave(object sender, EventArgs e)
        {
            if (txtExchRate.Text.Trim() == "")
                txtExchRate_SetText();
        }
        //END STEVEN EDIT

        protected void txtExchRate_Focus(Object sender, EventArgs e)
        {
            txtExchRate.Text = "";
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dgvPODetails1.CurrentCell.Value = dtp.Text;
        }

        private void tabDgvControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvPODetails1.Columns.Count != 0)
            {
                if (tabDgvControl.SelectedTab.Text == "Detail PO")
                {
                   
                    //TAB DETAIL PO
                    dgvPODetails1.Columns["No"].Visible = true;
                    dgvPODetails1.Columns["FullItemId"].Visible = true;
                    dgvPODetails1.Columns["ItemName"].Visible = true;
                    dgvPODetails1.Columns["Base"].Visible = false;
                    dgvPODetails1.Columns["RemainingAmount"].Visible = false;
                    dgvPODetails1.Columns["Qty"].Visible = true;
                    dgvPODetails1.Columns["RemainingQty"].Visible = false;
                    dgvPODetails1.Columns["Unit"].Visible = true;
                    dgvPODetails1.Columns["Ratio"].Visible = true;
                    dgvPODetails1.Columns["Price"].Visible = true;
                    dgvPODetails1.Columns["DeliveryMethod"].Visible = true;
                    dgvPODetails1.Columns["AvailableDate"].Visible = true;
                    dgvPODetails1.Columns["Total"].Visible = true;
                    dgvPODetails1.Columns["DiscScheme"].Visible = true;
                    dgvPODetails1.Columns["Diskon(%)"].Visible = true;
                    dgvPODetails1.Columns["TotalDisk"].Visible = true;
                    dgvPODetails1.Columns["TotalPPN"].Visible = true;
                    dgvPODetails1.Columns["TotalPPh"].Visible = true;
                    dgvPODetails1.Columns["BonusScheme"].Visible = true;
                    dgvPODetails1.Columns["CashBackScheme"].Visible = true;
                    dgvPODetails1.Columns["Deskripsi"].Visible = true;

                    //other
                    dgvPODetails1.Columns["OrderDate"].Visible = false;
                    dgvPODetails1.Columns["GroupId"].Visible = false;
                    dgvPODetails1.Columns["SubGroup1Id"].Visible = false;
                    dgvPODetails1.Columns["SubGroup2Id"].Visible = false;
                    dgvPODetails1.Columns["ItemId"].Visible = false;
                    dgvPODetails1.Columns["InventSiteId"].Visible = false;
                    dgvPODetails1.Columns["ReffId"].Visible = false; ;
                    //dgvPODetails1.Columns["TotalNett"].Visible = false;
                    dgvPODetails1.Columns["RemainingQtyNew"].Visible = false;
                    dgvPODetails1.Columns["ReffId"].Visible = false;
                    dgvPODetails1.Columns["ReffSeqNo"].Visible = false;
                    dgvPODetails1.Columns["CanvasSeqNo"].Visible = false;
                    dgvPODetails1.Columns["SeqNoGroup"].Visible = false;
                    dgvPODetails1.Columns["Base"].Visible = false;
                    dgvPODetails1.Columns["AgreementID"].Visible = false;
                    dgvPODetails1.Columns["SeqNo"].Visible = false;

                }
                else
                {
                    
                    //TAB DETAIL PO
                    dgvPODetails1.Columns["No"].Visible = true;
                    dgvPODetails1.Columns["ReffId"].Visible = true;
                    dgvPODetails1.Columns["FullItemId"].Visible = true;
                    dgvPODetails1.Columns["ItemName"].Visible = true;


                    if (cmbReffTableName.Text == "Canvass Sheet")
                    {
                        dgvPODetails1.Columns["Base"].Visible = false;
                        dgvPODetails1.Columns["RemainingAmount"].Visible = false;
                        dgvPODetails1.Columns["Qty"].Visible = true;
                        dgvPODetails1.Columns["Qty"].ReadOnly = true; //BY: HC 
                        dgvPODetails1.Columns["Unit"].Visible = true;
                        dgvPODetails1.Columns["Ratio"].Visible = true;

                        dgvPODetails1.Columns["ReffId"].HeaderText = "CS No";

                    }
                    else if (cmbReffTableName.Text == "Purchase Agreement")
                    {
                        dgvPODetails1.Columns["Base"].Visible = true;

                        Conn = ConnectionString.GetConnection();
                        if (txtReffID.Text.Contains("PO"))
                        {
                            Query = "select ReffId from PurchH where PurchID = '" + txtReffID.Text.Substring(0, 13) + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Query = "select TransType from PurchAgreementH where AgreementID = '" + Cmd.ExecuteScalar().ToString() + "'";
                        }
                        else
                            Query = "select TransType from PurchAgreementH where AgreementID = '" + txtReffID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        if (Cmd.ExecuteScalar().ToString() == "AMOUNT")
                        {
                            dgvPODetails1.Columns["RemainingAmount"].Visible = true;
                            dgvPODetails1.Columns["Qty"].Visible = false;
                            dgvPODetails1.Columns["Unit"].Visible = false;
                        }
                        else if (Cmd.ExecuteScalar().ToString() != "AMOUNT")
                        {
                            dgvPODetails1.Columns["RemainingAmount"].Visible = false;
                            dgvPODetails1.Columns["Qty"].Visible = true;
                            dgvPODetails1.Columns["Unit"].Visible = true;
                        }
                        Conn.Close();
                        dgvPODetails1.Columns["Ratio"].Visible = false;

                        dgvPODetails1.Columns["ReffId"].HeaderText = "PA No";
                    }
                    dgvPODetails1.Columns["AvailableDate"].Visible = false;


                    dgvPODetails1.Columns["RemainingQty"].Visible = false;
                    dgvPODetails1.Columns["Price"].Visible = false;
                    dgvPODetails1.Columns["DeliveryMethod"].Visible = false;
                    dgvPODetails1.Columns["Total"].Visible = false;
                    dgvPODetails1.Columns["DiscScheme"].Visible = false;
                    dgvPODetails1.Columns["Diskon(%)"].Visible = false;
                    dgvPODetails1.Columns["TotalDisk"].Visible = false;
                    dgvPODetails1.Columns["TotalPPN"].Visible = false;
                    dgvPODetails1.Columns["TotalPPh"].Visible = false;
                    dgvPODetails1.Columns["BonusScheme"].Visible = false;
                    dgvPODetails1.Columns["CashBackScheme"].Visible = false;
                    dgvPODetails1.Columns["Deskripsi"].Visible = false;
                    //other
                    dgvPODetails1.Columns["OrderDate"].Visible = false;
                    dgvPODetails1.Columns["GroupId"].Visible = false;
                    dgvPODetails1.Columns["SubGroup1Id"].Visible = false;
                    dgvPODetails1.Columns["SubGroup2Id"].Visible = false;
                    dgvPODetails1.Columns["ItemId"].Visible = false;
                    dgvPODetails1.Columns["InventSiteId"].Visible = false;
                    dgvPODetails1.Columns["ReffId"].Visible = true;
                    //dgvPODetails1.Columns["TotalNett"].Visible = false;
                    dgvPODetails1.Columns["RemainingQtyNew"].Visible = false;
                    dgvPODetails1.Columns["ReffSeqNo"].Visible = false;
                    dgvPODetails1.Columns["CanvasSeqNo"].Visible = false;
                    dgvPODetails1.Columns["SeqNoGroup"].Visible = false;
                    dgvPODetails1.Columns["AgreementID"].Visible = false;
                    dgvPODetails1.Columns["SeqNo"].Visible = false;

                    dgvPODetails1.Columns["ReffId"].Visible = false; //BY: HC
                }
            }
        }

        private void GenerateDatagridViewHeader()
        {
            string[] DataGridViewHeader = new string[] { "No", "ReffId","FullItemId","Item Name","Base","Qty", "Remaining Qty", "Unit",
            "Amount","Ratio","Price","Delivery Method","Available Date","Total","Disc. Type","Diskon(%)","TotalDisk",
            "Total PPN","Total PPH", "Bonus Scheme","CashBack Scheme","Notes","OrderDate",
            "GroupId","SubGroup1Id","SubGroup2Id","ItemId","InventSiteId","TotalNett",
            "RemainingQtyNew","ReffSeqNo","CanvasSeqNo","SeqNoGroup","AgreementID","SeqNo","BaseItemId"};

            string[] DataGridViewName = new string[] { "No", "ReffId","FullItemId","ItemName","Base","Qty", "RemainingQty", "Unit",
            "RemainingAmount","Ratio","Price","DeliveryMethod","AvailableDate","Total","DiscScheme","Diskon(%)","TotalDisk",
            "TotalPPN","TotalPPH", "BonusScheme","CashBackScheme","Deskripsi","OrderDate",
            "GroupId","SubGroup1Id","SubGroup2Id","ItemId","InventSiteId","TotalNett",
            "RemainingQtyNew","ReffSeqNo","CanvasSeqNo","SeqNoGroup","AgreementID","SeqNo","BaseItemID"};

            dgvPODetails1.ColumnCount = (DataGridViewHeader.Length);
            for (int z = 0; z < DataGridViewHeader.Length; z++)
            {
                dgvPODetails1.Columns[z].HeaderText = DataGridViewHeader[z].ToString();
                dgvPODetails1.Columns[z].Name = DataGridViewName[z].ToString();
            }
        }


        public void GetDataHeader()
        {
            string FullItemId = "";

            if (PONumber == "")
            {
                PONumber = txtPOId.Text.Trim();
            }

            Conn = ConnectionString.GetConnection();

            //Query = "Select PurchId, OrderDate, DueDate, TransType, ReffTableName, ReffId, ReffId2, CurrencyId, ExchRate, VendId, DP, total, Total_Disk, PPN, Total_PPN, PPH, Total_PPH, Deskripsi, Total_Nett, TermofPayment, PaymentMode FROM [dbo].[PurchH] where PurchId = '" + PONumber + "'";
            Query = "Select PurchId, OrderDate, DueDate, TransType, ReffTableName, ReffId, ReffId2, PH.CurrencyId, ExchRate, PH.VendId, VT.VendName, DP, total, Total_Disk, PH.PPN, Total_PPN, PH.PPH, Total_PPH, Deskripsi, Total_Nett, PH.TermofPayment, PaymentMode, PH.DPAmount, PH.DPType ";
            Query += "FROM PurchH PH ";
            Query += "LEFT JOIN VendTable VT ON PH.VendID = VT.VendId ";
            Query += "WHERE PurchId = '" + PONumber + "' ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                if (Mode == "Amend")
                    txtPOId.Text = "";
                else
                    txtPOId.Text = Dr["PurchID"].ToString();
                dtPODate.Text = Dr["OrderDate"].ToString();
                cmbReffTableName.SelectedItem = Dr["ReffTableName"].ToString();
                if (Mode != "Amend")
                    txtReffID.Text = Dr["ReffId"].ToString();
                else
                    txtReffID.Text = PONumber;
                txtReffID2.Text = Dr["ReffId2"].ToString();
                txtVendId.Text = Dr["VendId"].ToString();
                txtVendName.Text = Dr["VendName"].ToString();
                txtTransType.Text = Dr["TransType"].ToString().ToUpper();
                cmbCurrency.SelectedItem = Dr["CurrencyId"].ToString();
                txtExchRate.Text = Convert.ToDecimal(Dr["ExchRate"]).ToString("N2");
                cmbPPn.SelectedItem = Dr["PPN"].ToString();
                cmbPPh.SelectedItem = Dr["PPH"].ToString();
                txtTotalPPH.Text = Convert.ToDecimal(Dr["Total_PPH"]).ToString("N2");
                txtTotalPPN.Text = Convert.ToDecimal(Dr["Total_PPN"]).ToString("N2");
                txtTotalDisk.Text = Convert.ToDecimal(Dr["Total_Disk"]).ToString("N2");
                txtTotal.Text = Convert.ToDecimal(Dr["Total"]).ToString("N2");
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                txtTotalNett.Text = Convert.ToDecimal(Dr["Total_Nett"]).ToString("N2");
                cmbTermOfPayment.SelectedItem = Dr["TermofPayment"].ToString();
                cmbPaymentMode.SelectedItem = Dr["PaymentMode"].ToString();

                //BY: HC (S)
                if (Dr["DPType"].ToString() == "")
                    cmbDPRequired.SelectedItem = "NO";
                else
                    cmbDPRequired.SelectedItem = "YES";
                cmbDPType.Text = Dr["DPType"].ToString();
                tbxDPPercent.Text = Convert.ToDecimal(Dr["DP"]).ToString("N2");
                tbxDPAmount.Text = (Convert.ToDecimal(txtTotalNett.Text) * Convert.ToDecimal(tbxDPPercent.Text) / 100).ToString("N2");
                //BY: HC (E)
            }
            Dr.Close();

            dgvPODetails1.Rows.Clear();
            dgvPODetails1.Columns.Clear();
            if (dgvPODetails1.RowCount - 1 <= 0)
            {
                GenerateDatagridViewHeader();
            }

            Query = "SELECT DISTINCT(Transtype) from PurchDtl po LEFT JOIN PurchAgreementH h ON po.ReffId=h.AgreementID WHERE po.PurchID='" + txtPOId.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            var firstColumn = Cmd.ExecuteScalar();
            string transtype = "FIX";

            if (firstColumn != null)
            {
                transtype = firstColumn.ToString();
            }

            Query = "Select a.*,case when SUBSTRING(b.ReffId, 1, 2)='PA' then b.ReffId else '' end AgreementID, ";
            Query += "(CASE WHEN a.Base = 'Y' OR a.Base = '' THEN a.FullItemID ELSE ( SELECT s.FullItemID FROM [PurchDtl] s WHERE s.Base='Y' AND s.ReffBaseSeqNo=a.ReffBaseSeqNo AND s.PurchID=a.PurchID AND s.DeliveryMethod=a.DeliveryMethod) END ) AS baseid, ";
                
            switch (transtype)
            {
                case "QTY":
                    Query += "dbo.QtyPA(a.ReffId,a.ReffBaseSeqNo,a.DeliveryMethod) - dbo.QtyPO(a.ReffId,a.ReffBaseSeqNo,a.DeliveryMethod,a.PurchID) AS maxamount ";
                    break;
                case "AMOUNT":
                    Query += "dbo.AmountPA(a.ReffId,a.ReffBaseSeqNo,a.DeliveryMethod) - dbo.AmountPO(a.ReffId,a.ReffBaseSeqNo,a.DeliveryMethod,a.PurchID) AS maxamount ";
                    break;
                default:
                    Query += "0 AS maxamount ";
                    break;
            }
            Query += ",'" +transtype + "' AS TransType FROM [dbo].[PurchDtl] a ";
            Query += "LEFT JOIN [dbo].[PurchH] b ON a.PurchID=b.PurchID ";
            if (Mode == "Amend")
                Query += "WHERE a.PurchId = '" + PONumber + "' ";
            else
                Query += "WHERE a.PurchId = '" + txtPOId.Text + "' ";
            Conn = ConnectionString.GetConnection();
            string lol = "";

            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    if (transtype != "FIX")
                    {
                        lol = Dr["ReffBaseSeqNo"].ToString() + Dr["DeliveryMethod"].ToString() + Dr["baseid"].ToString();
                        if (TampungIdItem.Contains(lol) == false)
                        {
                            TampungIdItem.Add(lol);
                            HitungAmount.Add(0);
                        }
                    }

                    String DiscScheme = Dr["DiscScheme"].ToString() == null ? "" : Dr["DiscScheme"].ToString();
                    String AvailableDate = Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy");

                    //this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["InventSiteId"], Dr["Qty"], Dr["RemainingQty"], Dr["Qty_KG"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], Dr["Total"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPH"], Dr["Total_Nett"], Dr["ReffId"], Dr["ReffSeqNo"], Dr["Deskripsi"], AvailableDate, DiscScheme, Dr["BonusScheme"], Dr["CashBackScheme"], Dr["DeliveryMethod"], Dr["ReffSeqNo"], Dr["ReffBaseSeqNo"], Dr["Base"], Dr["AgreementID"], Dr["SeqNo"], Dr["Total_Nett"]);
                    this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["ReffId"], Dr["FullItemId"], Dr["ItemName"],
                        Dr["Base"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["maxamount"], Dr["Konv_Ratio"], Dr["Price"], Dr["DeliveryMethod"],
                        AvailableDate, Dr["Total"], Dr["DiscScheme"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPH"],
                        Dr["BonusScheme"], Dr["CashBackScheme"], Dr["Deskripsi"], Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"],
                        Dr["SubGroup2Id"], Dr["ItemId"], Dr["InventSiteId"], Dr["Total_Nett"], "",
                        Dr["maxamount"], Dr["ReffSeqNo"], Dr["ReffBaseSeqNo"], Dr["AgreementID"], Dr["SeqNo"], Dr["baseid"]);

                    if (Mode == "Amend")
                    {
                        if (Convert.ToDecimal(Dr["RemainingQty"]) > 0) //BY: HC | TAMBAH IF AGAR YG OUTS MASI BISA EDIT
                        {
                            DeliveryMethodValue("SELECT [DeliveryMethod] FROM [dbo].[DeliveryMethod]");
                            cmbDeliveryMethod.Value = Dr["DeliveryMethod"].ToString();
                            dgvPODetails1.Rows[(dgvPODetails1.Rows.Count - 1)].Cells["DeliveryMethod"] = cmbDeliveryMethod;
                        }
                    }


                }
                Dr.Close();
            }
            TotalNett2(); //BY: HC
            dgvPODetails1.Columns["BaseItemId"].Visible = false;
            Conn.Close();
            //tia edit
            HidePrice();
            //tia edit end
            dgvPODetails1Setting();
            dgvPODetails1.AutoResizeColumns();
        }

        private void BeforeEditdgvAttachment()
        {
            if (dgvAttachment.Columns.Count <= 0)
            {
                dgvAttachment.ColumnCount = 6;
                dgvAttachment.Columns[0].Name = "No";
                dgvAttachment.Columns[2].Name = "FileName"; dgvAttachment.Columns[2].HeaderText = "File Name";
                dgvAttachment.Columns[3].Name = "ContentType";
                dgvAttachment.Columns[4].Name = "FileSize"; dgvAttachment.Columns[4].HeaderText = "File Size (Kb)";
                dgvAttachment.Columns[5].Name = "Attachment";
                dgvAttachment.Columns[1].Name = "FileId";
            }
            dgvAttachment.Columns[1].Visible = false;
            Query = "SELECT * FROM [dbo].[tblAttachments] WHERE [ReffTableName] = 'POForm' AND [ReffTransID] = @ReffTransID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                if (Mode == "Amend")
                    Cmd.Parameters.AddWithValue("@ReffTransID", PONumber);
                else
                    Cmd.Parameters.AddWithValue("@ReffTransID", txtPOId.Text);
                int i = 0;
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dgvAttachment.Rows.Add(1);
                    dgvAttachment.Rows[i].Cells["No"].Value = dgvAttachment.Rows.Count;
                    dgvAttachment.Rows[i].Cells["FileId"].Value = Dr["id"];
                    dgvAttachment.Rows[i].Cells["FileName"].Value = Dr["fileName"];
                    dgvAttachment.Rows[i].Cells["ContentType"].Value = Dr["ContentType"];
                    dgvAttachment.Rows[i].Cells["FileSize"].Value = Dr["fileSize"];
                    test.Add((byte[])Dr["attachment"]);
                    i++;
                }
                Dr.Close();
            }
        }

        private void dgvPODetails1Setting()
        {
            dgvPODetails1.Columns["GroupId"].Visible = false;
            dgvPODetails1.Columns["SubGroup1Id"].Visible = false;
            dgvPODetails1.Columns["SubGroup2Id"].Visible = false;
            dgvPODetails1.Columns["ItemId"].Visible = false;
            dgvPODetails1.Columns["InventSiteId"].Visible = false;
            dgvPODetails1.Columns["CanvasSeqNo"].Visible = false;
            dgvPODetails1.Columns["SeqNoGroup"].Visible = false;
            dgvPODetails1.Columns["ReffId"].Visible = false;
            dgvPODetails1.Columns["ReffSeqNo"].Visible = false;
            dgvPODetails1.Columns["SeqNo"].Visible = false;
            dgvPODetails1.Columns["RemainingAmount"].Visible = false;
            dgvPODetails1.Columns["Base"].Visible = false;
            dgvPODetails1.Columns["SeqNoGroup"].Visible = false;
            dgvPODetails1.Columns["AgreementID"].Visible = false;
            dgvPODetails1.Columns["OrderDate"].Visible = false;
            dgvPODetails1.Columns["TotalNett"].Visible = false;
            dgvPODetails1.Columns["RemainingQtyNew"].Visible = false;

            dgvPODetails1.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["AvailableDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["RemainingQty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Price"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Total"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalDisk"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalPPN"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalPPH"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalNett"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["BonusScheme"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["CashBackScheme"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Diskon(%)"].DefaultCellStyle.Format = "N2";

            dgvPODetails1.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalDisk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalPPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalPPh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["BonusScheme"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["CashBackScheme"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }

        public void AddDataGridDetail(List<string> PQID1)
        {
            AddCmbPaymentMode();
            AddCmbTermOfPayment();
            AddCmbCurrency();

            dgvPODetails1.Rows.Clear();
            if (dgvPODetails1.RowCount - 1 <= 0)
            {
                string[] DataGridViewHeader = new string[] { };
                GenerateDatagridViewHeader();
            }

            for (int i = 0; i < PQID1.Count; i++)
            {
                Query = "SELECT DISTINCT DiscType,DiscPercent,DiscAmount,CurrencyId, DP, DPAmount,ExchRate, a.PurchQuotId, ";
                Query += "a.base, b.OrderDate, b.GroupId, a.VendId, b.SubGroup1Id, b.SubGroup2Id, b.ItemId, a.FullItemId, ";
                Query += "a.ItemName, a.Qty, a.Qty_Kg, a.Ratio, a.Unit, a.Price, c.TransType, a.PPN, a.PPH, a.DeliveryMethod, ";
                Query += "a.CanvasSeqNo, a.SeqNoGroup, h.PaymentModeID AS PaymentMode, h.TermofPayment ,b.BonusScheme,b.CashBackScheme, h.DPType ";

                Query += "FROM [dbo].[CanvasSheetD] a ";
                Query += "LEFT JOIN [dbo].[PurchQuotation_Dtl] b ON a.PurchQuotId = b.PurchQuotId ";
                Query += "INNER JOIN PurchQuotationH h ON h.PurchQuotID = b.PurchQuotID and a.FullItemId = b.FullItemId and a.ItemName = b.ItemName ";
                Query += "LEFT JOIN [dbo].[CanvasSheetH] c ON a.[CanvasId] = c.[CanvasId] ";
                Query += "WHERE a.CanvasId = '" + txtReffID.Text + "' and a.StatusApproval = 'YES' and a.Qty > '0' and a.PurchQuotId = '" + PQID1[i] + "' ";
                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        //this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["InventSiteId"], Dr["Qty"], Dr["RemainingQty"], Dr["Qty_KG"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], Dr["Total"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPH"], Dr["Total_Nett"], Dr["ReffId"], Dr["ReffSeqNo"], Dr["Deskripsi"], AvailableDate, DiscScheme, Dr["BonusScheme"], Dr["CashBackScheme"], Dr["DeliveryMethod"], Dr["ReffSeqNo"], Dr["ReffBaseSeqNo"], Dr["Base"], Dr["SeqNo"], Dr["Total_Nett"]);
                        //this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], "", Dr["Qty"], Dr["Qty"], Dr["Qty_KG"], Dr["Unit"], Dr["Ratio"], Dr["Price"], "", "", "", "", "", "", "", "", "", "", "", "", "", Dr["DeliveryMethod"], Dr["CanvasSeqNo"], Dr["SeqNoGroup"], Dr["Base"], "", "");
                        this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), txtReffID.Text, Dr["FullItemId"],
                            Dr["ItemName"], Dr["Base"], Dr["Qty"], /*Dr["RemainingQty"]*/"", Dr["Unit"], "", Dr["Ratio"],
                            Dr["Price"], Dr["DeliveryMethod"], /*AvailableDate*/ "", /*Dr["Total"]*/"", Dr["DiscType"],
                            Convert.ToDecimal(Dr["DiscPercent"]), Convert.ToDecimal(Dr["DiscAmount"]), /*Dr["Total_PPN"]*/"",
                            /*Dr["Total_PPH"]*/"", Dr["BonusScheme"], Dr["CashBackScheme"], /*Dr["Deskripsi"]*/"",
                            Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], /*Dr["InventSiteId"]*/"",
                            /*Dr["Qty_KG"]*/"", /*Dr["Total_Nett"]*/"", /*Dr["ReffSeqNo"]*/"", /*DiscScheme*/"", /*Dr["ReffSeqNo"]*/"",
                            Dr["CanvasSeqNo"], Dr["SeqNoGroup"], /*Dr["SeqNo"]*/"", /*Dr["Total_Nett"]*/"");

                        

                        txtVendId.Text = Dr["VendId"].ToString();

                        Cmd = new SqlCommand("Select VendName From VendTable Where VendId = '" + txtVendId.Text + "'", Conn);
                        txtVendName.Text = Cmd.ExecuteScalar().ToString();

                        txtTransType.Text = Dr["TransType"].ToString();

                        //STV edit
                        //cmbPPh.SelectedItem = Dr["PPH"].ToString() != "" ? Convert.ToDecimal(Dr["PPH"]).ToString("N2") : "0";
                        string PPH = Dr["PPH"].ToString() != "" ? Convert.ToDecimal(Dr["PPH"]).ToString("N2") : "0";
                        cmbPPh.Items.Add(PPH);
                        cmbPPh.SelectedItem = PPH;

                        //cmbPPn.SelectedItem = Dr["PPN"].ToString() != "" ? Convert.ToDecimal(Dr["PPH"]).ToString("N2") : "0";
                        string PPN = Dr["PPN"].ToString() != "" ? Convert.ToDecimal(Dr["PPN"]).ToString("N2") : "0";
                        cmbPPn.Items.Add(PPN);
                        cmbPPn.SelectedItem = PPN;

                        txtExchRate.Text = Convert.ToDecimal(Dr["ExchRate"]).ToString("N2");

                        //BY: HC (S)
                        if (Dr["DPType"].ToString() == "")
                            cmbDPRequired.SelectedText = "NO";
                        else
                            cmbDPRequired.SelectedText = "YES";
                        cmbDPType.Text = Dr["DPType"].ToString();
                        tbxDPAmount.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");
                        tbxDPPercent.Text = Convert.ToDecimal(Dr["DP"]).ToString("N2");
                        //BY: HC (E)

                        cmbCurrency.SelectedItem = Dr["CurrencyId"].ToString();
                        cmbPaymentMode.SelectedItem = Dr["PaymentMode"].ToString();
                        cmbTermOfPayment.SelectedItem = Dr["TermofPayment"].ToString();
                        dgvPODetails1Setting();

                    }
                    Dr.Close();
                }
                Conn.Close();

                txtReffID2.Text = PQID1[i];

            }

            for (int i = 0; i < dgvPODetails1.ColumnCount; i++)
            {
                if (/*i == 15 || */i == 21 || i == 22 || i == 26)
                {
                    dgvPODetails1.Columns[i].ReadOnly = false;
                }
                else
                {
                    dgvPODetails1.Columns[i].ReadOnly = true;
                }
            }
            dgvPODetails1.Columns["GroupId"].Visible = false;
            dgvPODetails1.Columns["SubGroup1Id"].Visible = false;
            dgvPODetails1.Columns["SubGroup2Id"].Visible = false;
            dgvPODetails1.Columns["ItemId"].Visible = false;
            dgvPODetails1.Columns["InventSiteId"].Visible = false;
            dgvPODetails1.Columns["RemainingQty"].Visible = false;
            dgvPODetails1.Columns["CanvasSeqNo"].Visible = false;
            dgvPODetails1.Columns["SeqNoGroup"].Visible = false;
            dgvPODetails1.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["AvailableDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["RemainingQty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Price"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Total"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalDisk"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalPPN"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalPPH"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["BonusScheme"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["CashBackScheme"].DefaultCellStyle.Format = "N2";
            TotalNett();
            TotalNett2();
            dgvPODetails1.AutoResizeColumns();

            tabDgvControl.SelectedIndex = 0;
            calculateTotalBonus();

        }

        private DataGridViewComboBoxCell cellValue(string query)
        {
            cell = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            cell = new DataGridViewComboBoxCell();
            cell.Items.Add("Select");
            while (Dr2.Read())
                cell.Items.Add(Dr2[0].ToString());
            return cell;
        }

        private DataGridViewComboBoxCell DeliveryMethodValue(string query)
        {
            cmbDeliveryMethod = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            cmbDeliveryMethod = new DataGridViewComboBoxCell();
            cmbDeliveryMethod.Items.Add("Select");
            while (Dr2.Read())
                cmbDeliveryMethod.Items.Add(Dr2[0].ToString());
            return cmbDeliveryMethod;
        }

        private void InventSite_DropDownClosed(object sender, EventArgs e)
        {
            InventSite.Visible = false;
        }

        private void InventSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvPODetails1.CurrentCell.Value = InventSite.Text.ToString();
        }

        private void cmbPPh_Load()
        {
            Query = "Select [TaxStatusCode], [TaxStatusName], [TaxPercent] From [dbo].[TaxGroup] where TaxStatusCode like '%PPH%' or TaxStatusCode like ''";
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            //cmbPPh.Items.Add("- Select -");
            while (Dr.Read())
            {
                cmbPPh.Items.Add(Dr["TaxPercent"].ToString());
            }
            Dr.Close();
        }

        private void cmbPPn_Load()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select [TaxStatusCode], [TaxStatusName], [TaxPercent] From dbo.[TaxGroup] where TaxStatusCode like '%PPN%' or TaxStatusCode like ''";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            //cmbPPn.Items.Add("- Select -");
            while (Dr.Read())
            {
                cmbPPn.Items.Add(Dr["TaxPercent"].ToString());
            }
            Dr.Close();
        }

        public void SetMode(string tmpMode, string tmpPONumber, string tmpCanvasID)
        {
            Mode = tmpMode;
            PONumber = tmpPONumber;
            CanvasId = tmpCanvasID;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select [ReceiptOrderId] From dbo.[ReceiptOrderH] where [PurchaseOrderId]='" + txtPOId.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                string TmpCekRO = Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString();

                if (TmpCekRO != "")
                {
                    MessageBox.Show("PO tidak dapat diedit karena sudah direlease " + TmpCekRO + " .");
                    return;
                }

                Mode = "Edit";

                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        public void SetParent(Purchase.PurchaseOrderNew.POInquiry F)
        {
            Parent = F;
        }

        public void ModeNew()
        {
            PONumber = "";
            
            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            btnSearchCS.Enabled = true;
            btnCurrency.Enabled = false;
            //txtDPPercent.Enabled = false;

            dtDueDate.Enabled = true;
            cmbReffTableName.Enabled = true;

            txtExchRate.Enabled = false;
            cmbTermOfPayment.Enabled = false;
            cmbPaymentMode.Enabled = false;
            cmbDPRequired.Enabled = false;
            txtTotal.Enabled = false;
            txtTotalPPN.Enabled = false;
            txtTotalPPH.Enabled = false;
            txtTotalDisk.Enabled = false;
            txtDeskripsi.Enabled = true;

            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnDelAttachment.Enabled = true;

            //BY: HC (S)
            cmbCurrency.Enabled = false;
            cmbDPType.Enabled = false;
            tbxDPAmount.Enabled = false;
            tbxDPPercent.Enabled = false;
            cbxHitung.Enabled = false;
            label10.Visible = false;
            label2.Visible = false;
            tbxDPPercent.Visible = false;
            tbxDPAmount.Visible = false;
            cmbDPType.Visible = false;
            cbxHitung.Visible = false;
            //BY: HC (E)
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnClose.Visible = false;

            // btnNew.Enabled = true;
            // btnDelete.Enabled = true;

            btnSearchCS.Enabled = false;
            btnCurrency.Enabled = true;
            //txtDPPercent.Enabled = true;
            txtExchRate.Enabled = false;
            txtDeskripsi.Enabled = true;
            cmbTermOfPayment.Enabled = false;
            cmbPaymentMode.Enabled = false;
            cmbDPRequired.Enabled = false;
            dgvPODetails1.ReadOnly = false;

            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnDelAttachment.Enabled = true;

            for (int i = 0; i < dgvPODetails1.ColumnCount; i++)
            {
                if (cmbReffTableName.Text == "Purchase Order")
                {
                    btnNew.Enabled = true;
                    btnDelete.Enabled = true;

                    if (i == 9 || i == 11) //Ratio & DeliveryMethod
                    {
                        dgvPODetails1.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvPODetails1.Columns[i].ReadOnly = true;
                    }
                }
                else
                {
                    if (i == 21 || i == 22) //Deskripsi & OrderDate
                    {
                        dgvPODetails1.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvPODetails1.Columns[i].ReadOnly = true;
                    }
                }
                if (cmbReffTableName.Text == "Purchase Agreement")
                {
                    dgvPODetails1.Columns["Qty"].ReadOnly = false;
                }
            }
            //make grid not sortable
            foreach (DataGridViewColumn column in dgvPODetails1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        //tia edit
        ContextMenu Cm = new ContextMenu();

        public void ModePopUp()
        {
            Mode = "PopUp";

            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
            btnEdit.Visible = false;
            btnNew.Visible = false;
            btnDelete.Visible = false;
            btnClose.Visible = false;

            dtPODate.Enabled = false;
            dtDueDate.Enabled = false;
            cmbReffTableName.Enabled = false;
            txtReffID.Enabled = false;
            btnSearchCS.Enabled = false;
            btnCurrency.Enabled = false;
            //txtDPPercent.Enabled = false;

            txtExchRate.Enabled = false;
            txtVendId.Enabled = true;
            txtVendId.ReadOnly = true;
            txtVendName.Enabled = true;
            txtVendName.ReadOnly = true;
            txtVendId.ContextMenu = Cm;
            txtVendName.ContextMenu = Cm;
            txtReffID.ContextMenu = Cm;

            txtTransType.Enabled = false;
            cmbCurrency.Enabled = false;
            txtExchRate.Enabled = false;
            txtTotal.Enabled = false;
            txtDeskripsi.Enabled = false;

            cmbPPh.Enabled = false;
            cmbPPn.Enabled = false;
            txtTotalDisk.Enabled = false;
            cmbPaymentMode.Enabled = false;
            cmbTermOfPayment.Enabled = false;

            txtReffID.Enabled = true;
            txtReffID.ReadOnly = true;
            cmbDPRequired.Enabled = false;
            dgvPODetails1.ReadOnly = true;

            btnUpload.Visible = false;
            btnDownload.Visible = false;
            btnDelAttachment.Visible = false;
        }
        //tia edit end
        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            Conn = ConnectionString.GetConnection();

            Query = "Select TransStatus from dbo.PurchH where PurchID = '" + txtPOId.Text + "' ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                String Status = Dr["TransStatus"].ToString();

                if (Status == "02" || Status == "03" || Status == "07")
                {
                    btnClose.Enabled = false;
                    btnEdit.Enabled = false;
                }
                else if (Status == "05")
                {
                    //btnEdit.Enabled = false; //REMARKED BY: HC 
                    btnEdit.Visible = true; //BY: HC | SEPERTINYA STATUS 05 MASI BISA DI EDIT
                }
                else
                {
                    btnEdit.Visible = true;
                }

            }
            Dr.Close();

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
            btnDelete.Enabled = false;
            btnNew.Enabled = false;

            btnClose.Visible = true;

            dtPODate.Enabled = false;
            dtDueDate.Enabled = false;
            cmbReffTableName.Enabled = false;
            //txtReffID.Enabled = false;
            btnSearchCS.Enabled = false;
            btnCurrency.Enabled = false;
            //txtDPPercent.Enabled = false;
            txtExchRate.Enabled = false;
            //tia edit
            txtVendId.Enabled = true; //popup
            txtVendId.ReadOnly = true;
            txtVendName.Enabled = true;
            txtVendName.ReadOnly = true;
            txtReffID.Enabled = true;
            txtReffID.ReadOnly = true;
            txtVendId.ContextMenu = Cm;
            txtVendName.ContextMenu = Cm;
            txtReffID.ContextMenu = Cm;
            //tia end
            txtTransType.Enabled = false;
            cmbCurrency.Enabled = false;
            txtExchRate.Enabled = false;
            txtTotal.Enabled = false;
            txtDeskripsi.Enabled = false;
            cmbPPh.Enabled = false;
            cmbPPn.Enabled = false;
            txtTotalDisk.Enabled = false;
            cmbPaymentMode.Enabled = false;
            cmbTermOfPayment.Enabled = false;
            cmbDPRequired.Enabled = false;
            dgvPODetails1.ReadOnly = true;

            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;

            //BY: HC (S)
            cmbDPType.Enabled = false;
            tbxDPAmount.Enabled = false;
            tbxDPPercent.Enabled = false;
            cbxHitung.Enabled = false;
            //BY: HC (E)
        }

        public void ModeAmend()
        {
            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnClose.Visible = false;

            btnCurrency.Enabled = true;
            //STEVEN EDIT BEGIN
            //txtDPPercent.Enabled = false;
            //STEVEN EDIT END

            dtDueDate.Enabled = true;
            btnSearchCS.Visible = false;

            txtExchRate.Enabled = false;
            cmbCurrency.Enabled = false;
            cmbTermOfPayment.Enabled = false;
            cmbPaymentMode.Enabled = false;
            cmbDPRequired.Enabled = false;
            txtTotal.Enabled = false;
            txtTotalPPN.Enabled = false;
            txtTotalPPH.Enabled = false;
            txtTotalDisk.Enabled = false;
            txtDeskripsi.Enabled = true;

            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnDelAttachment.Enabled = true;

            //txtDPAmount.Enabled = false; //BY: HC

            //BY: HC (S)
            cbxHitung.Enabled = false;
            cmbDPType.Enabled = false;
            tbxDPAmount.Enabled = false;
            tbxDPPercent.Enabled = false;
            //BY: HC (E)
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string getPAID()
        {
            string PAID = "";

            if (dgvPODetails1.RowCount > 0)
            {
                for (int i = 0; i <= dgvPODetails1.RowCount - 1; i++)
                {
                    
                    string SeqNoDelete = "";
                    if (i == 0)
                    {
                        PAID += "(a.AgreementID = '";
                        PAID += txtReffID.Text;
                        //REMARKED BY: HC (S)
                        //PAID += "' AND SeqNoGroup <> '";
                        //PAID += dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value == null ? "" : Convert.ToString(dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value) + "' ";
                        //REMARKED BY: HC (E)
                        //BY: HC (S)
                        PAID += "' AND SeqNo <> '";
                        string paSeqNo = dgvPODetails1.Rows[i].Cells["SeqNo"].Value == null ? "" : Convert.ToString(dgvPODetails1.Rows[i].Cells["SeqNo"].Value);
                        PAID += paSeqNo + "' ";
                        //BY: HC (E)
                        if (Mode == "Edit")
                        {
                            for (int j = 0; j < ListSeqNoDelete.Count(); j++)
                            {
                                SeqNoDelete = SeqNoDelete + "'" + ListSeqNoDelete[j] + "',";
                            }
                            if (SeqNoDelete != "")
                            {
                                SeqNoDelete = SeqNoDelete.Remove(SeqNoDelete.Length - 1);
                                PAID += "OR SeqNo IN (" + SeqNoDelete + ") ";
                            }

                        }
                        else
                        {
                            PAID += " AND (RemainingQty <> CASE a.Base WHEN 'Y' THEN 0 ELSE -1 END OR RemainingAmount <> CASE a.Base WHEN 'Y' THEN 0 ELSE -1 END)";
                        }
                        PAID += ")";
                    }
                    else
                    {
                        PAID += " and (a.AgreementID = '";
                        PAID += txtReffID.Text;
                        //REMARKED BY: HC (S)
                        //PAID += "' AND SeqNoGroup <> '";
                        //PAID += dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value == null ? "" : Convert.ToString(dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value) + "' ";
                        //REMARKED BY: HC (E)
                        //BY: HC (S)
                        PAID += "' AND SeqNo <> '";
                        string paSeqNo = dgvPODetails1.Rows[i].Cells["SeqNo"].Value == null ? "" : Convert.ToString(dgvPODetails1.Rows[i].Cells["SeqNo"].Value);
                        PAID += paSeqNo + "' ";
                        //BY: HC (E)
                        if (Mode == "Edit")
                        {
                            for (int k = 0; k < ListSeqNoDelete.Count(); k++)
                            {
                                SeqNoDelete = SeqNoDelete + "'" + ListSeqNoDelete[k] + "',";
                            }
                            if (SeqNoDelete != "")
                            {
                                SeqNoDelete = SeqNoDelete.Remove(SeqNoDelete.Length - 1);
                                PAID += "OR SeqNo IN (" + SeqNoDelete + ") ";
                            }
                        }
                        else
                        {
                            PAID += " AND (RemainingQty <> CASE a.Base WHEN 'Y' THEN 0 ELSE -1 END OR RemainingAmount <> CASE a.Base WHEN 'Y' THEN 0 ELSE -1 END)";
                        }
                        PAID += ")";
                    }
                }

                return PAID;
            }
            else
            {
                PAID = "(a.AgreementID <> '' or SeqNo <> '0') AND (RemainingQty <> CASE a.Base WHEN 'Y' THEN 0 ELSE -1 END OR RemainingAmount <> CASE a.Base WHEN 'Y' THEN 0 ELSE -1 END)";
                return PAID;
            }
        }

        public string getPOID()
        {
            string POID = "";

            if (dgvPODetails1.RowCount > 0)
            {
                for (int i = 0; i <= dgvPODetails1.RowCount - 1; i++)
                {
                    if (i == 0)
                    {
                        POID += "(PurchID <> '";
                        POID += dgvPODetails1.Rows[i].Cells["PurchID"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["PurchID"].Value.ToString();
                        POID += "' or SeqNo <> '";
                        POID += dgvPODetails1.Rows[i].Cells["ReffSeqNo"].Value == null ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["ReffSeqNo"].Value.ToString());
                        POID += "')";
                    }
                    else
                    {
                        POID += " and (PurchID <> '";
                        POID += dgvPODetails1.Rows[i].Cells["PurchID"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["PurchID"].Value.ToString();
                        POID += "' or SeqNo <> '";
                        POID += dgvPODetails1.Rows[i].Cells["ReffSeqNo"].Value == null ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["ReffSeqNo"].Value.ToString());
                        POID += "')";
                    }
                }

                return POID;
            }
            else
            {
                POID = "(PurchID <> '' or SeqNo <> '0')";
                return POID;
            }
        }

        private void searchCS()
        {
            string Query = "";
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "CanvasId";
            tmpSearch.Order = "CanvasDate Desc";

            Query = "SELECT Distinct D.CanvasId, H.CanvasDate, D.PurchReqId, D.PurchQuotId, P.OrderDate AS PurchReqDate, Q.OrderDate AS PurchQuotDate, Q.VendName, H.CreatedDate, H.CreatedBy, H.UpdatedDate, H.UpdatedBy ";
            Query += "FROM CanvasSheetH H INNER JOIN CanvasSheetD D ON D.CanvasId = H.CanvasId INNER JOIN PurchRequisitionH P ON P.PurchReqId = H.PurchReqId INNER JOIN PurchQuotationH Q ON Q.PurchQuotID = D.PurchQuotId ";
            Query += "Where D.PurchQuotId NOT IN (select ReffId2 from PurchH where ReffTableName='CanvasSheet') AND UPPER(D.StatusApproval) = 'YES' AND UPPER(H.TransStatus) = '02' AND UPPER(H.TransType) = 'FIX'";

            tmpSearch.QuerySearch = Query;
            tmpSearch.FilterText = new string[] { "CanvasId", "CanvasDate", "PurchReqId", "PurchQuotId", "PurchReqDate", "PurchQuotDate", "VendName", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "CS No", "CS Date", "PR No", "PQ No", "PR Date", "PQ Date", "Vendor", "Created Date", "Created By", "Updated Date", "Updated By" };
            tmpSearch.Select = new string[] { "CanvasId", "VendName" };
            tmpSearch.Order = "CreatedDate Desc";
            tmpSearch.ShowDialog();

            if (ConnectionString.Kodes != null)
            {
                Query = "select CASE WHEN a.ValidTo > cast(convert(char(8), getdate(), 112) + ' 23:59:59.99' as datetime) then 'no' else 'yes' end as expired from CanvasSheetD b left join PurchQuotationH a on b.PurchQuotId = a.PurchQuotID where b.CanvasId = '" + ConnectionString.Kodes[0] + "'";
                Cmd = new SqlCommand(Query, Conn);
                if (Cmd.ExecuteScalar().ToString().ToUpper() == "YES")
                {
                    MessageBox.Show("Valid PQ untuk Canvas Sheet ini sudah expired!\r\n");
                    return;
                }
                txtReffID.Text = ConnectionString.Kodes[0];
                txtVendName.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }

            Query = "SELECT VendId FROM VendTable where VendName = '" + txtVendName.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            //txtVendId.Text = Cmd.ExecuteNonQuery().ToString();
            if (Cmd.ExecuteScalar() != System.DBNull.Value && Cmd.ExecuteScalar() != null)
            {
                txtVendId.Text = Cmd.ExecuteScalar().ToString();
            }

            if (tmpSearch.cekClose == true)
            {
                tmpSearch.cekClose = false;
            }
            else
            {
                CallFormCSId(txtReffID.Text);
            }
        }

        private void searchPA()
        {
            string Query = "";
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.CekMasuk = "NotPopUpError";
            tmpSearch.PrimaryKey = "AgreementID";
            tmpSearch.Order = "AgreementID Desc";

            Query = "SELECT DISTINCT h.AgreementID,h.OrderDate,h.TransType, PurchQuotID,v.VendName, h.DueDate ,h.CreatedDate,h.CreatedBy,h.UpdatedDate,h.UpdatedBy ";
            Query += " FROM PurchAgreementH h ";
            Query += " LEFT JOIN VendTable v ON h.VendId = v.VendId ";
            Query += "WHERE h.StClose = 0 AND h.TransStatus = '03' AND h.AgreementID ";
            Query += "IN(SELECT d.AgreementID FROM PurchAgreementDtl d WHERE h.TransStatus = '03' ";
            Query += "GROUP BY d.AgreementID HAVING (SUM(d.RemainingQty) > 0 OR SUM(d.RemainingAmount) > 0))";
            tmpSearch.QuerySearch = Query;
            tmpSearch.FilterText = new string[] { "AgreementID", "OrderDate", "TransType", "VendName", "DueDate", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "Agreement No", "Agreement Date", "Type", "Vendor", "Expired Date", "Created Date", "Created By", "Updated Date", "Updated By" };
            tmpSearch.Order = "CreatedDate Desc";
            tmpSearch.Select = new string[] { "AgreementID", "VendName", "PurchQuotID" };
            tmpSearch.ShowDialog();

            if (ConnectionString.Kodes != null)
            {
                txtReffID.Text = ConnectionString.Kodes[0];
                txtVendName.Text = ConnectionString.Kodes[1];
                if (ConnectionString.Kodes[2] != null)
                {
                    //BY: HC (S) | BIAR KEAMBIL PQ ID UNTUK ID PA REV
                    if (ConnectionString.Kodes[0].Contains("Rev"))
                    {
                        Query = "select PurchQuotID from PurchAgreementH where AgreementID = '" + ConnectionString.Kodes[0].Split(' ')[0] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        txtReffID2.Text = Cmd.ExecuteScalar().ToString();
                    } //BY: HC (E)
                    else
                        txtReffID2.Text = ConnectionString.Kodes[2];
                }
                ConnectionString.Kodes = null;
            }

            Query = "SELECT VendId FROM VendTable where VendName = '" + txtVendName.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            if (Cmd.ExecuteScalar() != System.DBNull.Value && Cmd.ExecuteScalar() != null)
            {
                txtVendId.Text = Cmd.ExecuteScalar().ToString();
            }
            dgvPODetails1.Rows.Clear();
        }

        private void btnSearchCS_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if (cmbReffTableName.Text == "")
            {
                MessageBox.Show("Pilih Refference terlebih dahulu");
                return;
            }
            else if (cmbReffTableName.Text == "Canvass Sheet")
            {
                searchCS();
            }
            else if (cmbReffTableName.Text == "Purchase Agreement")
            {
                searchPA();
            }

            Conn = ConnectionString.GetConnection();
            string QueryPQ = "SELECT CurrencyId, ExchRate, DP, DPAmount, PPH, PPN, TermofPayment, PaymentModeID, TransType, VendID, VendName, DPType FROM PurchQuotationH WHERE PurchQuotID = '" + txtReffID2.Text + "'";
            Cmd = new SqlCommand(QueryPQ, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbCurrency.SelectedItem = Dr["CurrencyId"].ToString();
                txtExchRate.Text = Convert.ToDecimal(Dr["ExchRate"]).ToString("N2");

                cmbPPh.Text = Dr["PPH"].ToString();
                cmbPPn.Text = Dr["PPN"].ToString();
                cmbPaymentMode.SelectedItem = Dr["PaymentModeID"].ToString();
                cmbTermOfPayment.SelectedItem = Dr["TermofPayment"].ToString();
                //txtTransType.Text = Dr["TransType"].ToString();
                //txtVendId.Text = Dr["VendID"].ToString();
                //txtVendName.Text = Dr["VendName"].ToString();

                //txtDPPercent.Text = Convert.ToDecimal(Dr["DP"]).ToString("N2"); //REMARKED BY: HC

                //BY: HC (S)
                if (Dr["DPType"].ToString() == "")
                    cmbDPRequired.SelectedItem = "NO";
                else
                    cmbDPRequired.SelectedItem = "YES";
                cmbDPType.Text = Dr["DPType"].ToString();
                tbxDPPercent.Text = Convert.ToDecimal(Dr["DP"]).ToString("N2");
                tbxDPAmount.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");
                //BY: HC (E)
            }
            Dr.Close();
            Conn.Close();
        }

        private void calculateTotalBonus()
        {
            decimal totalBonus = 0;
            decimal totalCashBack = 0;

            if (dgvPODetails1.Rows.Count > 0)
            {
                for (int i = 0; i <= dgvPODetails1.Rows.Count - 1; i++)
                {
                    if (dgvPODetails1.Rows[i].Cells["BonusScheme"].Value == null || dgvPODetails1.Rows[i].Cells["BonusScheme"].Value == DBNull.Value || String.IsNullOrEmpty(dgvPODetails1.Rows[i].Cells["BonusScheme"].Value.ToString()))
                        dgvPODetails1.Rows[i].Cells["BonusScheme"].Value = 0;
                    if (dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value == null || dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value == DBNull.Value || String.IsNullOrEmpty(dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value.ToString()))
                        dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value = 0;

                    totalBonus = totalBonus + Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["BonusScheme"].Value);
                    totalCashBack = totalCashBack + Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value);
                }
            }

            txtBonusScheme.Text = totalBonus.ToString("N2");
            txtCashBackScheme.Text = totalCashBack.ToString("N2");
        }

        public void AddDataGridFromDetail(List<string> PurchAID, List<string> SeqNo, List<string> SeqNoGroup)
        {
            decimal PriceY = 0;
            string unit = "";
            string FullItemID = "";

            if (dgvPODetails1.RowCount - 1 <= 0)
            {
                dgvPODetails1.ColumnCount = 37;
                dgvPODetails1.Columns[0].Name = "No";
                dgvPODetails1.Columns[1].Name = "OrderDate";
                dgvPODetails1.Columns[2].Name = "GroupId";
                dgvPODetails1.Columns[3].Name = "SubGroup1Id";
                dgvPODetails1.Columns[4].Name = "SubGroup2Id";
                dgvPODetails1.Columns[5].Name = "ItemId";
                dgvPODetails1.Columns[6].Name = "FullItemId";
                dgvPODetails1.Columns[7].Name = "ItemName";
                dgvPODetails1.Columns[8].Name = "InventSiteId";
                dgvPODetails1.Columns[9].Name = "Qty";
                dgvPODetails1.Columns[10].Name = "RemainingQtyNew"; //dgvPODetails1.Columns["RemainingQtyNew"].HeaderText = "RemainingQty";
                dgvPODetails1.Columns[11].Name = "RemainingQty"; //dgvPODetails1.Columns["RemainingQty"].HeaderText = "Total Berat"; //REMARKED BY: HC
                dgvPODetails1.Columns[12].Name = "Unit";
                dgvPODetails1.Columns[13].Name = "Ratio";
                dgvPODetails1.Columns[14].Name = "Price";
                dgvPODetails1.Columns[15].Name = "Total";
                dgvPODetails1.Columns[16].Name = "Diskon(%)";
                dgvPODetails1.Columns[17].Name = "TotalDisk";
                dgvPODetails1.Columns[18].Name = "TotalPPN";
                dgvPODetails1.Columns[19].Name = "TotalPPh";
                dgvPODetails1.Columns[20].Name = "TotalNett";
                dgvPODetails1.Columns[21].Name = "ReffId";
                dgvPODetails1.Columns[22].Name = "ReffSeqNo";
                dgvPODetails1.Columns[23].Name = "Deskripsi"; //dgvPODetails1.Columns[23].HeaderText = "Notes";
                dgvPODetails1.Columns[24].Name = "AvailableDate";
                dgvPODetails1.Columns[25].Name = "DiscScheme";
                dgvPODetails1.Columns[26].Name = "BonusScheme";
                dgvPODetails1.Columns[27].Name = "CashBackScheme";
                dgvPODetails1.Columns[28].Name = "DeliveryMethod";
                dgvPODetails1.Columns[29].Name = "CanvasSeqNo";
                dgvPODetails1.Columns[30].Name = "SeqNoGroup";
                dgvPODetails1.Columns[31].Name = "Base";
                dgvPODetails1.Columns[32].Name = "AgreementID";
                dgvPODetails1.Columns[33].Name = "SeqNo";
                dgvPODetails1.Columns[34].Name = "RemainingAmount";
                dgvPODetails1.Columns[35].Name = "BaseItemId";
                dgvPODetails1.Columns[36].Name = "TransType";
                //dgvPODetails1.Columns[37].Name = "PPN";
                //dgvPODetails1.Columns[38].Name = "PPH";
            }

            for (int i = 0; i < PurchAID.Count; i++)
            {
                //BY: HC (S) | AMBIL HARGA YG BASE Y
                Conn = ConnectionString.GetConnection();
                Query = "select top 1 Price from PurchAgreementDtl where AgreementID = '" + txtReffID.Text + "' and Base = 'Y' and SeqNo < " + SeqNo[i] + " order by SeqNo desc";
                Cmd = new SqlCommand(Query, Conn);
                PriceY = Convert.ToDecimal(Cmd.ExecuteScalar());
                
                Query = "SELECT a.AgreementID, b.SeqNo, b.OrderDate, b.GroupId, a.VendId, b.SubGroup1Id, b.SubGroup2Id, b.ItemId, b.FullItemId, b.ItemName,b.Base, b.RemainingQty 'Qty', b.Konv_Ratio, b.Unit, b.Price, b.DiscPercentage, a.TransType, a.PPN, a.PPH, b.SeqNoGroup,b.Unit,DeliveryMethod,DiscAmount, b.RemainingAmount ";
                Query += ",(CASE WHEN b.Base = 'Y' THEN b.FullItemID ELSE ( SELECT s.FullItemID FROM [PurchAgreementDtl] s WHERE s.Base='Y' AND s.SeqNoGroup=b.SeqNoGroup AND s.AgreementID=b.AgreementID AND s.DeliveryMethod=b.DeliveryMethod) END ) AS baseid ";
                Query += ",(CASE WHEN b.Base = 'Y' THEN b.RemainingAmount ELSE ( SELECT s.RemainingAmount FROM [PurchAgreementDtl] s WHERE s.Base='Y' AND s.SeqNoGroup=b.SeqNoGroup AND s.AgreementID=b.AgreementID AND s.DeliveryMethod=b.DeliveryMethod) END ) AS remAmount ";
                Query += ",(CASE WHEN b.Base = 'Y' THEN b.RemainingQty ELSE ( SELECT s.RemainingQty FROM [PurchAgreementDtl] s WHERE s.Base='Y' AND s.SeqNoGroup=b.SeqNoGroup AND s.AgreementID=b.AgreementID AND s.DeliveryMethod=b.DeliveryMethod) END ) AS remQty ";
                Query += "FROM [dbo].[PurchAgreementH] a ";
                Query += "LEFT JOIN [dbo].[PurchAgreementDtl] b ON a.AgreementID = b.AgreementID ";
                Query += "WHERE a.AgreementID = '" + PurchAID[i] + "' and b.SeqNo = '" + SeqNo[i] + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        unit = Dr["SeqNoGroup"].ToString() + Dr["DeliveryMethod"].ToString() + Dr["baseid"].ToString();
                        if (TampungIdItem.Contains(unit) == false)
                        {
                            TampungIdItem.Add(unit);
                            HitungAmount.Add(0);
                        }

                        if (Dr["TransType"].ToString() == "AMOUNT")
                        {
                            if (Dr["Base"].ToString() == "Y")
                            {
                                this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], "", Dr["Qty"], Dr["Qty"], Dr["Qty"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], "", Dr["DiscPercentage"], Dr["DiscAmount"], "", "", "", "", "", "", "", "", "", "", Dr["DeliveryMethod"], Dr["SeqNo"], Dr["SeqNoGroup"], Dr["Base"], Dr["AgreementID"], SeqNo[i], Dr["remAmount"], Dr["baseid"], Dr["TransType"]);
                            }
                            else
                            {
                                decimal PriceN = Convert.ToDecimal(string.IsNullOrEmpty(Dr["Price"].ToString()) ? "0" : Dr["Price"]);
                                decimal Qty = Convert.ToDecimal(string.IsNullOrEmpty(Dr["Qty"].ToString()) ? "0" : Dr["Qty"]);

                                this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], "", Qty, Qty, Qty, Dr["Unit"], Dr["Konv_Ratio"], PriceY + PriceN, "", Dr["DiscPercentage"], Dr["DiscAmount"], "", "", "", "", "", "", "", "", "", "", Dr["DeliveryMethod"], Dr["SeqNo"], Dr["SeqNoGroup"], Dr["Base"], Dr["AgreementID"], SeqNo[i], Dr["remAmount"], Dr["baseid"], Dr["TransType"]);
                            }
                        }
                        else
                        {
                            if (Dr["Base"].ToString() == "Y")
                            {
                                this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], "", Dr["Qty"], Dr["Qty"], Dr["Qty"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], "", Dr["DiscPercentage"], Dr["DiscAmount"], "", "", "", "", "", "", "", "", "", "", Dr["DeliveryMethod"], Dr["SeqNo"], Dr["SeqNoGroup"], Dr["Base"], Dr["AgreementID"], SeqNo[i], Dr["remQty"], Dr["baseid"], Dr["TransType"]);
                            }
                            else
                            {
                                decimal PriceN = Convert.ToDecimal(string.IsNullOrEmpty(Dr["Price"].ToString()) ? "0" : Dr["Price"]);
                                decimal Qty = Convert.ToDecimal(string.IsNullOrEmpty(Dr["Qty"].ToString()) ? "0" : Dr["Qty"]);

                                this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], "", Qty, Qty, Qty, Dr["Unit"], Dr["Konv_Ratio"], PriceY + PriceN, "", Dr["DiscPercentage"], Dr["DiscAmount"], "", "", "", "", "", "", "", "", "", "", Dr["DeliveryMethod"], Dr["SeqNo"], Dr["SeqNoGroup"], Dr["Base"], Dr["AgreementID"], SeqNo[i], Dr["remQty"], Dr["baseid"], Dr["TransType"]);
                            }
                        }


                        txtVendId.Text = Dr["VendId"].ToString();
                        txtTransType.Text = Dr["TransType"].ToString();
                        cmbPPh.SelectedItem = Convert.ToDecimal(Dr["PPH"]).ToString("N2");
                        cmbPPn.SelectedItem = Convert.ToDecimal(Dr["PPN"]).ToString("N2");
                        FullItemID = Dr["FullItemId"].ToString();
                    }
                    Dr.Close();
                }
                Conn.Close();
            }

            dgvPODetails1.ReadOnly = false;

            for (int i = 0; i < dgvPODetails1.ColumnCount; i++)
            {
                if (i == 9 || i == 12 || i == 26)
                {
                    dgvPODetails1.Columns[i].ReadOnly = false;
                }
                else
                {
                    dgvPODetails1.Columns[i].ReadOnly = true;
                }
            }

            if (dgvPODetails1.Columns["Base"].ToString() == "Y")
            {
                dgvPODetails1.Columns["Base"].ReadOnly = false;
            }
            dgvPODetails1.Columns["OrderDate"].Visible = false;
            dgvPODetails1.Columns["GroupId"].Visible = false;
            dgvPODetails1.Columns["SubGroup1Id"].Visible = false;
            dgvPODetails1.Columns["SubGroup2Id"].Visible = false;
            dgvPODetails1.Columns["ItemId"].Visible = false;
            dgvPODetails1.Columns["RemainingQtyNew"].Visible = false;
            dgvPODetails1.Columns["RemainingQty"].Visible = false;
            dgvPODetails1.Columns["InventSiteId"].Visible = false;
            dgvPODetails1.Columns["CanvasSeqNo"].Visible = false;
            dgvPODetails1.Columns["SeqNoGroup"].Visible = false;
            dgvPODetails1.Columns["AgreementID"].Visible = false;
            dgvPODetails1.Columns["Base"].Visible = false;
            dgvPODetails1.Columns["SeqNo"].Visible = false;
            dgvPODetails1.Columns["RemainingAmount"].Visible = false;
            dgvPODetails1.Columns["BaseItemId"].Visible = false;
            dgvPODetails1.Columns["TransType"].Visible = false;
            //dgvPODetails1.Columns["Base"].Visible = true;
            // dgvPODetails1.Columns["DeliveryMethod"].Visible = false;
            dgvPODetails1.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["AvailableDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["RemainingQty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["RemainingAmount"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Price"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Total"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalDisk"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalPPN"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalPPH"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalNett"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Unit"].ReadOnly = true;

            //BY: HC (S)
            dgvPODetails1.Columns["BonusScheme"].ReadOnly = true;
            dgvPODetails1.Columns["Deskripsi"].ReadOnly = false;
            dgvPODetails1.Columns["TotalDisk"].ReadOnly = false;
            dgvPODetails1.Columns["ReffId"].Visible = false;
            dgvPODetails1.Columns["ReffSeqNo"].Visible = false;

            dgvPODetails1.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalDisk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalPPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalPPh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["BonusScheme"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["CashBackScheme"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["RemainingAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //BY: HC (E)

            TotalNett();
            TotalNett2();
            dgvPODetails1.AutoResizeColumns();

            tabDgvControl.SelectedIndex = 0;
            calculateTotalBonus();
        }

        public void AddDataGridFromDetailAmend(List<string> PurchID, List<string> SeqNo, List<string> ReffSeqNo)
        {
            decimal PriceY = 0;
            string unit = "";
            string FullItemID = "";

            if (dgvPODetails1.RowCount - 1 <= 0)
            {
                dgvPODetails1.ColumnCount = 34;
                dgvPODetails1.Columns[0].Name = "No";
                dgvPODetails1.Columns[1].Name = "OrderDate";
                dgvPODetails1.Columns[2].Name = "GroupId";
                dgvPODetails1.Columns[3].Name = "SubGroup1Id";
                dgvPODetails1.Columns[4].Name = "SubGroup2Id";
                dgvPODetails1.Columns[5].Name = "ItemId";
                dgvPODetails1.Columns[6].Name = "FullItemId";
                dgvPODetails1.Columns[7].Name = "ItemName";
                dgvPODetails1.Columns[8].Name = "InventSiteId";
                dgvPODetails1.Columns[9].Name = "Qty";
                dgvPODetails1.Columns[10].Name = "RemainingQty";
                dgvPODetails1.Columns[11].Name = "Unit";
                dgvPODetails1.Columns[12].Name = "Ratio";
                dgvPODetails1.Columns[13].Name = "Price";
                dgvPODetails1.Columns[14].Name = "Total";
                dgvPODetails1.Columns[15].Name = "Diskon(%)";
                dgvPODetails1.Columns[16].Name = "TotalDisk";
                dgvPODetails1.Columns[17].Name = "TotalPPN";
                dgvPODetails1.Columns[18].Name = "TotalPPh";
                dgvPODetails1.Columns[19].Name = "TotalNett";
                dgvPODetails1.Columns[20].Name = "ReffId";
                dgvPODetails1.Columns[21].Name = "ReffSeqNo";
                dgvPODetails1.Columns[22].Name = "Deskripsi";
                dgvPODetails1.Columns[23].Name = "AvailableDate";
                dgvPODetails1.Columns[24].Name = "DiscScheme";
                dgvPODetails1.Columns[25].Name = "BonusScheme";
                dgvPODetails1.Columns[26].Name = "CashBackScheme";
                dgvPODetails1.Columns[27].Name = "DeliveryMethod";
                dgvPODetails1.Columns[28].Name = "SeqNoGroup";
                dgvPODetails1.Columns[29].Name = "PurchID";
                dgvPODetails1.Columns[30].Name = "Base";
                dgvPODetails1.Columns[31].Name = "RemainingAmount";
                dgvPODetails1.Columns[32].Name = "BaseItemId";
                dgvPODetails1.Columns[33].Name = "TransType";

            }
            for (int i = 0; i < PurchID.Count; i++)
            {
                Query = "Select a.PurchID, b.SeqNo, b.OrderDate, b.GroupId, b.Diskon, b.Deskripsi, b.AvailableDate,b.DiscScheme, b.Base, b.BonusScheme, b.CashBackScheme, b.DeliveryMethod, b.Total_Nett, b.Total_Disk, b.Total_PPN, b.Total_PPh, b.ReffId, b.InventSiteId, b.Total, b.ReffSeqNo, a.VendId, b.SubGroup1Id, b.SubGroup2Id, b.ItemId, b.FullItemId, b.ItemName, b.Qty, b.RemainingQty,b.ReffbaseSeqNo, b.Konv_Ratio, b.Unit, b.Price, a.TransType, a.PPN, a.PPH, b.ReffBaseSeqNo from [dbo].[PurchH] a left JOIN [dbo].[PurchDtl] b ON a.PurchId = b.PurchId where a.PurchID = '" + PurchID[i] + "' and b.SeqNo = '" + SeqNo[i] + "'";
                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        string AvailableDate = Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy");
                        decimal SeqNoGroup = Convert.ToDecimal(string.IsNullOrEmpty(Dr["ReffBaseSeqNo"].ToString()) ? "0" : Dr["ReffBaseSeqNo"].ToString());

                        this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Dr["OrderDate"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["InventSiteID"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], Dr["Total"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPh"], Dr["Total_Nett"], Dr["ReffID"], Dr["ReffSeqNo"], Dr["Deskripsi"], AvailableDate, Dr["DiscScheme"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["DeliveryMethod"], SeqNoGroup, Dr["PurchID"], Dr["Base"],"","","");

                        txtVendId.Text = Dr["VendId"].ToString();
                        txtTransType.Text = Dr["TransType"].ToString();
                        cmbPPh.SelectedItem = Convert.ToDecimal(Dr["PPH"]).ToString("N2");
                        cmbPPn.SelectedItem = Convert.ToDecimal(Dr["PPN"]).ToString("N2");
                        FullItemID = Dr["FullItemId"].ToString();
                    }
                    Dr.Close();
                }

                Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + FullItemID + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();

                while (Dr.Read())
                {
                    combo.Items.Add(Dr[0].ToString());
                    combo.Items.Add(Dr[1].ToString());
                }
                Dr.Close();
                dgvPODetails1.Rows[(dgvPODetails1.Rows.Count - 1)].Cells[11] = combo;


                Conn.Close();
            }

            dgvPODetails1.ReadOnly = false;

            for (int i = 0; i < dgvPODetails1.ColumnCount; i++)
            {
                if (i == 9 || i == 11 || i == 26)
                {
                    dgvPODetails1.Columns[i].ReadOnly = false;
                }
                else
                {
                    dgvPODetails1.Columns[i].ReadOnly = true;
                }
            }

            Conn = ConnectionString.GetConnection();
            Query = "SELECT COUNT(*) COUNTDATA FROM ReceiptOrderH WHERE PurchaseOrderId = '" + txtReffID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            int CountDataPO = Convert.ToInt32(Cmd.ExecuteScalar());
            if (CountDataPO == 0)
            {
                dgvPODetails1.Columns["Price"].ReadOnly = false;
            }
            else
            {
                dgvPODetails1.Columns["Price"].ReadOnly = true;
            }
            Conn.Close();


            dgvPODetails1.Columns["GroupId"].Visible = false;
            dgvPODetails1.Columns["SubGroup1Id"].Visible = false;
            dgvPODetails1.Columns["SubGroup2Id"].Visible = false;
            dgvPODetails1.Columns["ItemId"].Visible = false;
            dgvPODetails1.Columns["RemainingQty"].Visible = false;
            dgvPODetails1.Columns["InventSiteId"].Visible = false;

            //dgvPODetails1.Columns["SeqNoGroup"].Visible = false;
            dgvPODetails1.Columns["PurchID"].Visible = false;
            dgvPODetails1.Columns["ReffID"].Visible = false;
            dgvPODetails1.Columns["Base"].Visible = false;
            dgvPODetails1.Columns["SeqNoGroup"].Visible = false;

            dgvPODetails1.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["AvailableDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["RemainingQty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Price"].DefaultCellStyle.Format = "N4";
            dgvPODetails1.Columns["Total"].DefaultCellStyle.Format = "N4";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalDisk"].DefaultCellStyle.Format = "N4";
            dgvPODetails1.Columns["TotalPPN"].DefaultCellStyle.Format = "N4";
            dgvPODetails1.Columns["TotalPPH"].DefaultCellStyle.Format = "N4";
            TotalNett();
            TotalNett2();
            dgvPODetails1.AutoResizeColumns();

            tabDgvControl.SelectedIndex = 0;
            calculateTotalBonus();
        }

        private void CallFormCSId(string CSId)
        {
            //Conn = ConnectionString.GetConnection();
            //Query = "Select count([GelombangId]) From [InventGelombangD] Where GelombangId in (Select GelombangId from InventGelombangD where GelombangId = '" + GelombangId + "' and BracketId='" + BracketId + "')";

            //Cmd = new SqlCommand(Query, Conn);
            //int CountChk = Convert.ToInt32(Cmd.ExecuteScalar());

            //if (CountChk > 0)
            //{
            PQ PQ = new PQ();
            //ListGelombang.Add(Gelombang);
            PQ.SetParentForm(this);
            PQ.GetCSId(CSId);
            PQ.ShowDialog();
            //}
        }

        bool Close = false;
        public void CalltoClose()
        {
            Close = true;
        }

        private void InsertAttachment(SqlConnection Con, string ID)
        {
            for (int i = 0; i < dgvAttachment.Rows.Count; i++)
            {

                Query = "INSERT INTO [dbo].[tblAttachments] ([ReffTableName],[ReffTransID],[fileName],[ContentType],[fileSize],[attachment]) VALUES (@ReffTableName,@ReffTransID,@fileName,@ContentType,@fileSize,@attachment)";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.Parameters.AddWithValue("@ReffTableName", "POForm");
                    Cmd.Parameters.AddWithValue("@ReffTransID", ID);
                    Cmd.Parameters.AddWithValue("@fileName", dgvAttachment.Rows[i].Cells["FileName"].Value);
                    Cmd.Parameters.AddWithValue("@ContentType", dgvAttachment.Rows[i].Cells["ContentType"].Value);
                    Cmd.Parameters.AddWithValue("@fileSize", dgvAttachment.Rows[i].Cells["FileSize"].Value);
                    Cmd.Parameters.Add("@attachment", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        string Jenis = "";
        string Kode = "";

        private void saveNewPurchHeader()
        {
            //begin============================================================================================
            //updated by : joshua
            //updated date : 14 Feb 2018
            //description : change generate sequence number, get from global function and update counter 
            Jenis = "PO";
            Kode = "PO";
            String TransStatus = "05";
            decimal dp_amount=0, dp_percent=0;
            PurchOrderId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);

            Query = "Insert into [dbo].[PurchH] (PurchId, OrderDate, DueDate, TransType,";
            Query += "ReffTableName, ReffId, ReffId2, CurrencyId, ExchRate, VendId,";
            Query += "Total, Total_Disk, PPN, Total_PPN, PPh, Total_PPh, Total_Nett,";
            Query += "Deskripsi, TransStatus, TermofPayment, PaymentMode,";
            Query += "CreatedDate, CreatedBy, DP, DPAmount, DPType)";
            Query += " VALUES ";
            Query += "(@PurchId, @OrderDate, @DueDate, @TransType, ";
            Query += "@ReffTableName, @ReffId, @ReffId2, @CurrencyId, @ExchRate, @VendId, ";
            Query += "@Total, @Total_Disk, @PPN, @Total_PPN, @PPh, @Total_PPh, @Total_Nett, ";
            Query += "@Deskripsi, @TransStatus, @TermofPayment, @PaymentMode, ";
            Query += "getdate(),";
            Query += "'" + ControlMgr.UserId + "',";
            Query += "@DP, @DPAmount, @DPType)";
            
            //BY: HC (S)
            if (cmbDPRequired.Text == "YES")
            {
                if (cmbDPType.Text == "Percentage")
                {
                    dp_percent= Convert.ToDecimal(tbxDPPercent.Text);
                    dp_amount = 0;
                }
                else if (cmbDPType.Text == "Amount")
                {
                    dp_percent= 0;
                    dp_amount = Convert.ToDecimal(tbxDPAmount.Text);
                }
            }
            //BY: HC (E)
            Cmd = new SqlCommand(Query, Conn);

            Cmd.Parameters.AddWithValue("@PurchId", PurchOrderId);
            Cmd.Parameters.AddWithValue("@OrderDate", dtPODate.Value.ToString("yyyy-MM-dd") );
            Cmd.Parameters.AddWithValue("@DueDate", dtDueDate.Value.ToString("yyyy-MM-dd"));
            Cmd.Parameters.AddWithValue("@TransType", txtTransType.Text);
            Cmd.Parameters.AddWithValue("@ReffTableName", cmbReffTableName.Text);
            Cmd.Parameters.AddWithValue("@ReffId", txtReffID.Text);
            Cmd.Parameters.AddWithValue("@ReffId2", txtReffID2.Text);
            Cmd.Parameters.AddWithValue("@CurrencyId", cmbCurrency.Text);
            Cmd.Parameters.AddWithValue("@ExchRate", (txtExchRate.Text == "" ? "0" : Convert.ToDecimal(txtExchRate.Text).ToString()));
            Cmd.Parameters.AddWithValue("@VendId", txtVendId.Text);
            Cmd.Parameters.AddWithValue("@Total", (txtTotal.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotal.Text).ToString()));
            Cmd.Parameters.AddWithValue("@Total_Disk", (txtTotalDisk.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalDisk.Text).ToString()));
            Cmd.Parameters.AddWithValue("@PPN", (cmbPPn.Text == "" ? "0" : cmbPPn.Text));
            Cmd.Parameters.AddWithValue("@Total_PPN", (txtTotalPPN.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPN.Text).ToString()));
            Cmd.Parameters.AddWithValue("@PPh", (cmbPPh.Text == "" ? "0" : cmbPPh.Text));
            Cmd.Parameters.AddWithValue("@Total_PPh", (txtTotalPPH.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPH.Text).ToString()));
            Cmd.Parameters.AddWithValue("@Total_Nett", (txtTotalNett.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalNett.Text).ToString()));
            Cmd.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text.Trim());
            Cmd.Parameters.AddWithValue("@TransStatus", TransStatus);
            Cmd.Parameters.AddWithValue("@TermofPayment", cmbTermOfPayment.Text);
            Cmd.Parameters.AddWithValue("@PaymentMode", cmbPaymentMode.Text);
            Cmd.Parameters.AddWithValue("@DP", dp_percent);
            Cmd.Parameters.AddWithValue("@DPAmount", dp_amount);
            Cmd.Parameters.AddWithValue("@DPType", cmbDPRequired.Text.ToUpper() == "NO" ? "" : cmbDPType.Text);
            Cmd.ExecuteNonQuery();
        }

        private void saveNewPurchDetail()
        {
            if (dgvPODetails1.Rows.Count > 0)
            {
                for (int i = 0; i <= dgvPODetails1.Rows.Count - 1; i++)
                {
                    if (dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == null || dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == "")
                        dgvPODetails1.Rows[i].Cells["AvailableDate"].Value = "01-01-1900";

                    String SeqNo = dgvPODetails1.Rows[i].Cells["No"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["No"].Value.ToString();
                    //String OrderDate = dgvPODetails1.Rows[i].Cells["OrderDate"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["OrderDate"].Value.ToString();
                    //String OrderDate = dgvPODetails1.Rows[i].Cells["OrderDate"].Value == null ? "" : FormateDateddmmyyyy(dgvPODetails1.Rows[i].Cells["OrderDate"].Value.ToString());
                    DateTime OrderDate = Convert.ToDateTime(dgvPODetails1.Rows[i].Cells["OrderDate"].Value);

                    String GroupId = dgvPODetails1.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["GroupId"].Value.ToString();
                    String SubGroup1Id = dgvPODetails1.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["SubGroup1Id"].Value.ToString();
                    String SubGroup2Id = dgvPODetails1.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["SubGroup2Id"].Value.ToString();
                    String ItemId = dgvPODetails1.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["ItemId"].Value.ToString();
                    String FullItemId = dgvPODetails1.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString();
                    String ItemName = dgvPODetails1.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["ItemName"].Value.ToString();
                    String InventSiteId = dgvPODetails1.Rows[i].Cells["InventSiteId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["InventSiteId"].Value.ToString();
                    decimal Qty = dgvPODetails1.Rows[i].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                    //decimal RemainingQty = dgvPODetails1.Rows[i].Cells["RemainingQty"].Value == null ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["RemainingQty"].Value.ToString());
                    String Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value == "" ? "" : dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                    decimal Ratio = dgvPODetails1.Rows[i].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Ratio"].Value.ToString());
                    decimal Price = dgvPODetails1.Rows[i].Cells["Price"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Price"].Value.ToString());
                    decimal Total = dgvPODetails1.Rows[i].Cells["Total"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Total"].Value.ToString());
                    decimal Diskon = dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value.ToString());
                    decimal TotalDisk = dgvPODetails1.Rows[i].Cells["TotalDisk"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalDisk"].Value.ToString());
                    decimal TotalPPN = dgvPODetails1.Rows[i].Cells["TotalPPN"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalPPN"].Value.ToString());
                    decimal TotalPPh = dgvPODetails1.Rows[i].Cells["TotalPPh"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalPPh"].Value.ToString());
                    decimal TotalNett = dgvPODetails1.Rows[i].Cells["TotalNett"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalNett"].Value.ToString());
                    String Deskripsi = dgvPODetails1.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["Deskripsi"].Value.ToString().Trim();
                    //String AvailableDate = dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == null ? "" : FormateDateddmmyyyy(dgvPODetails1.Rows[i].Cells["AvailableDate"].Value.ToString());
                    DateTime AvailableDate = dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dgvPODetails1.Rows[i].Cells["AvailableDate"].Value);
                    String DiscScheme = dgvPODetails1.Rows[i].Cells["DiscScheme"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["DiscScheme"].Value.ToString();
                    String BonusScheme = dgvPODetails1.Rows[i].Cells["BonusScheme"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["BonusScheme"].Value.ToString();
                    String CashBackScheme = dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value.ToString();
                    String DeliveryMethod = dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString();
                    String CanvasSeqNo = dgvPODetails1.Rows[i].Cells["CanvasSeqNo"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["CanvasSeqNo"].Value.ToString();
                    decimal SeqNoGroup = dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString());
                    decimal Qty_KG = 0;
                    decimal Price_KG = 0;
                    //hendry comment bentar jika fix error
                    string Base = dgvPODetails1.Rows[i].Cells["Base"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["Base"].Value.ToString();

                    if (Unit == "BTG")
                    {
                        if (Ratio > 0)
                        {
                            Qty_KG = Qty / Ratio;
                            Price_KG = Price / Ratio;
                        }
                    }
                    else
                    {
                        Qty_KG = Qty;
                        Price_KG = Price;
                    }

                    Query = "Insert into [dbo].[PurchDtl] (PurchId, OrderDate, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, InventSiteId, Qty, Qty_KG, RemainingQty, Unit, Konv_Ratio, Price, Price_KG, Total, Diskon, Total_Disk, Total_PPN, Total_PPh, Total_Nett, Deskripsi, AvailableDate, DiscScheme, BonusScheme, CashBackScheme, DeliveryMethod, ReffTableName, ReffId, Reffid2, ReffBaseSeqNo, CreatedDate, CreatedBy, Base) ";

                    if (Unit != "KG")
                        Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty * Ratio + "', '" + Qty + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "', @Deskripsi, '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' , '" + SeqNoGroup + "',getdate(), '" + ControlMgr.UserId + "', '" + Base + "');";

                    else
                        Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty + "', '" + Qty_KG + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "', @Deskripsi, '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' , '" + SeqNoGroup + "',getdate(), '" + ControlMgr.UserId + "', '" + Base + "');";

                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.AddWithValue("@Deskripsi", Deskripsi);
                    Cmd.ExecuteNonQuery();

                    if (cmbReffTableName.Text == "Canvass Sheet")
                    {
                        decimal QtyInput = decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                        Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                        decimal ConvRatio = 0;

                        string QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        string UoM = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        ConvRatio = (decimal)Cmd.ExecuteScalar();

                        if (Price_KG == null || Price_KG <= 0)
                        {
                            QueryTemp = "SELECT [Alt_AvgPrice] FROM [InventTable] WHERE FullItemID = '" + FullItemId + "' ";
                            Cmd = new SqlCommand(QueryTemp, Conn);
                            Price_KG = (decimal)Cmd.ExecuteScalar();
                        }

                        if (Unit == UoM)
                        {

                            QtyUoM = QtyInput;
                            QtyAlt = QtyInput * ConvRatio;
                            QtyAmount = QtyAlt * Price_KG;
                        }
                        else
                        {

                            QtyAlt = QtyInput;
                            QtyUoM = QtyInput / ConvRatio;
                            QtyAmount = QtyAlt * Price_KG;
                        }

                        //Update QtyPRIssued
                        if (txtTransType.Text != "AMOUNT")
                        {
                            Query = "Update Invent_Purchase_Qty Set ";
                            Query += "PR_CS_Approved_UoM = PR_CS_Approved_UoM - " + QtyUoM + ",";
                            Query += "PR_CS_Approved_Alt = PR_CS_Approved_Alt - " + QtyAlt + ",";
                            Query += "PO_Issued_Outstanding_UoM = PO_Issued_Outstanding_UoM + " + QtyUoM + ", ";
                            Query += "PO_Issued_Outstanding_Alt = PO_Issued_Outstanding_Alt + " + QtyAlt + ", ";
                            Query += "PO_Issued_Outstanding_Amount = PO_Issued_Outstanding_Amount + " + QtyAmount + " ";
                            Query += "Where FullItemID = '" + FullItemId + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                        }
                        //else if (txtTransType.Text == "AMOUNT")
                        //{
                        //    Query = "Update Invent_Purchase_Qty Set ";
                        //    Query += "PR_CS_Approved_Amount = PR_CS_Approved_Amount - " + QtyAmount + ",";
                        //    Query += "PO_Issued_Outstanding_Amount = PO_Issued_Outstanding_Amount + " + QtyAmount + " ";
                        //    Query += "Where FullItemID = '" + FullItemId + "'";
                        //    Cmd = new SqlCommand(Query, Conn);
                        //    Cmd.ExecuteNonQuery();
                        //}

                        //Steven Edit save to PurchRequisition_LogTable
                        QueryTemp = "Select [CanvasDate] from [CanvasSheetH] where [CanvasId]='" + txtReffID.Text.Trim() + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        DateTime TmpDate = Convert.ToDateTime(Cmd.ExecuteScalar() == null ? "1900-01-01" : Cmd.ExecuteScalar());

                        //QueryTemp = "Insert into [PO_Issued_LogTable] ([PODate],[POId],[VendId],[Qty_UoM],[Qty_Alt],[PAId],[PADate],[LogStatusCode],LogStatusDesc,LogDescription,UserID,LogDate,POSeqNo) ";
                        //QueryTemp += "VALUES('" + dtPODate.Value.ToString("yyyy-MM-dd") + "', '" + PurchOrderId + "', '" + txtVendId.Text.Trim() + "', '" + QtyUoM + "', '" + QtyAlt + "' , '" + txtReffID.Text.Trim() + "', '" + TmpDate.ToString("yyyy-MM-dd") + "','01','Open Order.' ,'Open Order.','" + ControlMgr.UserId + "', getdate(),'" + SeqNo + "')";
                        //Cmd = new SqlCommand(QueryTemp, Conn);
                        //Cmd.ExecuteNonQuery();
                        insertPO_LogTable(dtPODate.Value, PurchOrderId, txtVendId.Text.Trim(), QtyUoM, QtyAlt, txtReffID.Text.Trim(), TmpDate, "01", "Open Order", "Open Order", SeqNo == "" ? 0 : Convert.ToInt32(SeqNo));
                        //Steven Edit save to PurchRequisition_LogTable
                    }
                    else if (cmbReffTableName.Text == "Purchase Agreement")
                    {
                        decimal QtyInput = decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                        Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                        decimal ConvRatio = 0;

                        string QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        string UoM = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        ConvRatio = (decimal)Cmd.ExecuteScalar();

                        if (Unit == UoM)
                        {
                            QtyUoM = QtyInput;
                            QtyAlt = QtyInput * ConvRatio;
                        }
                        else
                        {
                            QtyAlt = QtyInput;
                            QtyUoM = QtyInput / ConvRatio;
                        }

                        Dr.Close();

                        //Update QtyPRIssued

                        if (txtTransType.Text == "AMOUNT")
                        {
                            #region PO dibuat Dari PA untuk category AMOUNT

                            Query = "Update Invent_Purchase_Qty Set PA_Issued_Amount= (PA_Issued_Amount - " + TotalNett + ") ";
                            Query += "WHERE FullItemID = '" + dgvPODetails1.Rows[i].Cells["BaseItemId"].Value.ToString() + "'; ";

                            Query += "UPDATE Invent_Purchase_Qty SET ";
                            Query += "PO_From_PA_Issued_UoM = PO_From_PA_Issued_UoM + " + QtyUoM + ",";
                            Query += "PO_From_PA_Issued_Alt = PO_From_PA_Issued_Alt + " + QtyAlt + ",";
                            Query += "PO_From_PA_Issued_Amount = (PO_From_PA_Issued_Amount + " + TotalNett + ")";
                            Query += "WHERE FullItemID = '" + FullItemId + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            #endregion
                        }
                        else
                        {
                            #region PO dibuat Dari PA untuk category QTY

                            Query = "UPDATE Invent_Purchase_Qty SET ";
                            Query += "PA_Issued_UoM = PA_Issued_UoM - " + QtyUoM + ",";
                            Query += "PA_Issued_Alt = PA_Issued_Alt - " + QtyAlt + " ";
                            Query += "WHERE FullItemID = '" + dgvPODetails1.Rows[i].Cells["BaseItemId"].Value.ToString() + "'; ";

                            Query += "UPDATE Invent_Purchase_Qty SET ";
                            Query += "PO_From_PA_Issued_UoM = PO_From_PA_Issued_UoM + " + QtyUoM + ",";
                            Query += "PO_From_PA_Issued_Alt = PO_From_PA_Issued_Alt + " + QtyAlt + "  WHERE FullItemID = '" + FullItemId + "'; ";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            #endregion
                        }


                        Query = "SELECT [OrderDate] FROM [dbo].[PurchAgreementH] where [AgreementID]='" + txtReffID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        PaDate = Convert.ToDateTime(Cmd.ExecuteScalar() == null ? "1900-01-01" : Cmd.ExecuteScalar());

                        //save to PurchRequisition_LogTable
                        insertPO_LogTable(dtPODate.Value, PurchOrderId, txtVendId.Text.Trim(), QtyUoM, QtyAlt, txtReffID.Text.Trim(), PaDate, "01", "Open Order", "Open Order", SeqNo == "" ? 0 : Convert.ToInt32(SeqNo));

                        String tmpUnit = "";
                        decimal tmpSeqNoGroup = 0;
                        decimal tmpSeqNo = 0;
                        //decimal tmpQty = 0;

                        Query = "Select Unit,SeqNoGroup,SeqNo from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "' and Base='Y'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            tmpUnit = Dr["Unit"].ToString();
                            tmpSeqNoGroup = decimal.Parse(Dr["SeqNoGroup"].ToString());
                            tmpSeqNo = decimal.Parse(Dr["SeqNo"].ToString());
                        }
                        Dr.Close();

                        #region Update Purchase Agreement
                        if (txtTransType.Text.ToUpper() == "FIX")
                        {
                            if (tmpUnit == Unit && tmpSeqNoGroup == SeqNoGroup)
                            {
                                Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + Qty + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                            }
                            else if (tmpUnit == "BTG" && (Unit == "KG" || Unit == "LBR") && tmpSeqNoGroup == SeqNoGroup)
                            {
                                decimal QtyNew = 0;
                                QtyNew = Qty / Ratio;

                                Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                            }
                            else if ((tmpUnit == "KG" || tmpUnit == "LBR") && Unit == "BTG" && tmpSeqNoGroup == SeqNoGroup)
                            {
                                decimal QtyNew = 0;
                                QtyNew = Qty * Ratio;

                                Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                            }
                        }
                        else
                        {
                            if (dgvPODetails1.Rows[i].Cells["Base"].Value.ToString() == "Y" || dgvPODetails1.Rows[i].Cells["Base"].Value.ToString() == "N")
                            {
                                if (txtTransType.Text.ToUpper() == "QTY")
                                {
                                    decimal KG;
                                    if (Unit == "KG")
                                        KG = Qty;
                                    else
                                        KG = Qty * Ratio;
                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + KG + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "' and Base='Y'";
                                }
                                else
                                {
                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingAmount = (RemainingAmount - " + TotalNett + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "' and Base='Y'";
                                }
                            }
                            else
                            {
                                if (tmpUnit == Unit && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    decimal KG = 0;
                                    if (Unit == "KG")
                                        KG = Qty;
                                    else
                                        KG = Qty * Ratio;
                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + Qty + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                                else if (tmpUnit == "BTG" && (Unit == "KG" || Unit == "LBR") && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    decimal QtyNew = 0;
                                    QtyNew = Qty / Ratio;

                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                                else if ((tmpUnit == "KG" || tmpUnit == "LBR") && Unit == "BTG" && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    decimal QtyNew = 0;
                                    QtyNew = Qty * Ratio;

                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                            }
                        }
                        #endregion

                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        SavePurchAmountTable(PurchOrderId);
                    }
                }
            }
        }

        private void SaveNew()
        {
            try
            {
                saveNewPurchHeader();
                insertstatuslog(PurchOrderId);
                InsertAttachment(Conn, PurchOrderId);

                ListMethod ListMethod1 = new ListMethod();

                saveNewPurchDetail();
                //duplicate();

                Conn.Close();
                MessageBox.Show("Data :" + PurchOrderId + " Berhasil ditambahkan.");
                if (Mode == "New")
                {
                    Parent.RefreshGrid();
                }

                txtPOId.Text = PurchOrderId;
                ModeBeforeEdit();

                MainMenu f = new MainMenu();
                f.refreshTaskList();
            }
            //catch (Exception x)
            //{
            //    MessageBox.Show(x.Message);
            //    return;
            //}
            finally
            {

            }
        }

        #region duplicate
        private void duplicate()
        {
            //begin============================================================================================
            //updated by : joshua
            //updated date : 14 Feb 2018
            //description : change generate sequence number, get from global function and update counter 
            Jenis = "PO"; Kode = "PO";
            PurchOrderId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);

            Query = "Insert into [dbo].[PurchH] (PurchId,OrderDate,DueDate,TransType,ReffTableName,ReffId,ReffId2,CurrencyId,ExchRate,VendId,Total,Total_Disk,PPN,Total_PPN,PPh,Total_PPh,Total_Nett,Deskripsi,TransStatus,TermofPayment,PaymentMode,DP,DPAmount, DPType,CreatedDate,CreatedBy) values (";
            Query += "'" + PurchOrderId + "',";
            Query += "'" + dtPODate.Value.Date + "',";
            Query += "'" + dtDueDate.Value.Date + "',";
            Query += "'" + txtTransType.Text + "',";
            Query += "'" + cmbReffTableName.Text + "',";
            Query += "'" + txtReffID.Text + "',";
            Query += "'" + txtReffID2.Text + "',";
            Query += "'" + cmbCurrency.Text + "',";
            Query += "'" + (txtExchRate.Text == "" ? "0" : Convert.ToDecimal(txtExchRate.Text).ToString()) + "',";
            Query += "'" + txtVendId.Text + "',";
            Query += "'" + (txtTotal.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotal.Text).ToString()) + "',";
            Query += "'" + (txtTotalDisk.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalDisk.Text).ToString()) + "',";
            Query += "'" + (cmbPPn.Text == "" ? "0" : cmbPPn.Text) + "',";
            Query += "'" + (txtTotalPPN.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPN.Text).ToString()) + "',";
            Query += "'" + (cmbPPh.Text == "" ? "0" : cmbPPh.Text) + "',";
            Query += "'" + (txtTotalPPH.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPH.Text).ToString()) + "',";
            Query += "'" + (txtTotalNett.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalNett.Text).ToString()) + "',";
            Query += "'" + txtDeskripsi.Text + "',";
            if (cmbReffTableName.Text == "Purchase Agreement")
            {
                Query += "'08',";
            }
            else
            {
                Query += "'05',";
            }
            //Query += "'" + txtToP.Text + "',";
            Query += "'" + cmbTermOfPayment.Text + "',";
            Query += "'" + cmbPaymentMode.Text + "',";
            //BY: HC (S)
            if (cmbDPRequired.Text == "YES")
            {
                if (cmbDPType.Text == "Percentage")
                {
                    Query += "'" + tbxDPPercent.Text + "', ";
                    Query += "'" + Convert.ToDecimal(tbxDPPercent.Text) * Convert.ToDecimal(txtTotalNett.Text) / 100 + "', ";
                }
                else if (cmbDPType.Text == "Amount")
                {
                    Query += "'" + Convert.ToDecimal(tbxDPAmount.Text) * 100 / Convert.ToDecimal(txtTotalNett.Text) + "', ";
                    Query += "'" + tbxDPAmount.Text + "', ";
                }
            }
            else if (cmbDPRequired.Text == "NO")
            {
                Query += "'0', '0', ";
            }
            Query += "@DPType,";
            //BY: HC (E)
            Query += "getdate(),";
            Query += "'" + ControlMgr.UserId + "');";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DPType", cmbDPRequired.Text == "NO" ? "" : cmbDPType.Text);
            Cmd.ExecuteNonQuery();

            //created by : Thaddaeus Matthias, 14 March 2018
            //Insert into status log table customer
            //========================================begin=============================================
            insertstatuslog(PurchOrderId);
            //=========================================end==============================================


            //update counter
            //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
            //end update counter
            //end=============================================================================================

            //insert to table attachment for the document
            InsertAttachment(Conn, PurchOrderId);
            //end======================================

            //ListMethod1 = new ListMethod();

            if (dgvPODetails1.Rows.Count > 0)
            {
                for (int i = 0; i <= dgvPODetails1.Rows.Count - 1; i++)
                {
                    if (dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == null || dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == "")
                        dgvPODetails1.Rows[i].Cells["AvailableDate"].Value = "01-01-1900";

                    String SeqNo = dgvPODetails1.Rows[i].Cells["No"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["No"].Value.ToString();
                    String OrderDate = dgvPODetails1.Rows[i].Cells["OrderDate"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["OrderDate"].Value.ToString();
                    String GroupId = dgvPODetails1.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["GroupId"].Value.ToString();
                    String SubGroup1Id = dgvPODetails1.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["SubGroup1Id"].Value.ToString();
                    String SubGroup2Id = dgvPODetails1.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["SubGroup2Id"].Value.ToString();
                    String ItemId = dgvPODetails1.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["ItemId"].Value.ToString();
                    String FullItemId = dgvPODetails1.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString();
                    String ItemName = dgvPODetails1.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["ItemName"].Value.ToString();
                    String InventSiteId = dgvPODetails1.Rows[i].Cells["InventSiteId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["InventSiteId"].Value.ToString();
                    decimal Qty = dgvPODetails1.Rows[i].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                    //decimal RemainingQty = dgvPODetails1.Rows[i].Cells["RemainingQty"].Value == null ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["RemainingQty"].Value.ToString());
                    String Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value == "" ? "" : dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                    decimal Ratio = dgvPODetails1.Rows[i].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Ratio"].Value.ToString());
                    decimal Price = dgvPODetails1.Rows[i].Cells["Price"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Price"].Value.ToString());
                    decimal Total = dgvPODetails1.Rows[i].Cells["Total"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Total"].Value.ToString());
                    decimal Diskon = dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value.ToString());
                    decimal TotalDisk = dgvPODetails1.Rows[i].Cells["TotalDisk"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalDisk"].Value.ToString());
                    decimal TotalPPN = dgvPODetails1.Rows[i].Cells["TotalPPN"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalPPN"].Value.ToString());
                    decimal TotalPPh = dgvPODetails1.Rows[i].Cells["TotalPPh"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalPPh"].Value.ToString());
                    decimal TotalNett = dgvPODetails1.Rows[i].Cells["TotalNett"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalNett"].Value.ToString());
                    String Deskripsi = dgvPODetails1.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["Deskripsi"].Value.ToString().Trim();
                    DateTime AvailableDate = dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dgvPODetails1.Rows[i].Cells["AvailableDate"].Value);
                    String DiscScheme = dgvPODetails1.Rows[i].Cells["DiscScheme"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["DiscScheme"].Value.ToString();
                    String BonusScheme = dgvPODetails1.Rows[i].Cells["BonusScheme"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["BonusScheme"].Value.ToString();
                    String CashBackScheme = dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value.ToString();
                    String DeliveryMethod = dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString();
                    String CanvasSeqNo = dgvPODetails1.Rows[i].Cells["CanvasSeqNo"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["CanvasSeqNo"].Value.ToString();
                    decimal SeqNoGroup = dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString());
                    decimal Qty_KG = 0;
                    decimal Price_KG = 0;
                    //hendry comment bentar jika fix error
                    string Base = dgvPODetails1.Rows[i].Cells["Base"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["Base"].Value.ToString();

                    if (Unit == "BTG")
                    {
                        if (Ratio > 0)
                        {
                            Qty_KG = Qty / Ratio;
                            Price_KG = Price / Ratio;
                        }
                    }
                    else
                    {
                        Qty_KG = Qty;
                        Price_KG = Price;
                    }

                    //Query = "Insert into [dbo].[PurchDtl] (PurchId, OrderDate, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, InventSiteId, Qty, Qty_KG, RemainingQty, Unit, Konv_Ratio, Price, Price_KG, Total, Diskon, Total_Disk, Total_PPN, Total_PPh, Total_Nett, Deskripsi, AvailableDate, DiscScheme, BonusScheme, CashBackScheme, DeliveryMethod, ReffTableName, ReffId, Reffid2, ReffSeqNo, ReffBaseSeqNo, CreatedDate, CreatedBy, Base) ";
                    Query = "Insert into [dbo].[PurchDtl] (PurchId, OrderDate, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, InventSiteId, Qty, Qty_KG, RemainingQty, Unit, Konv_Ratio, Price, Price_KG, Total, Diskon, Total_Disk, Total_PPN, Total_PPh, Total_Nett, Deskripsi, AvailableDate, DiscScheme, BonusScheme, CashBackScheme, DeliveryMethod, ReffTableName, ReffId, Reffid2, ReffBaseSeqNo, CreatedDate, CreatedBy, Base) ";

                    if (Unit != "KG")
                        //Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty * Ratio + "', '" + Qty_KG + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "', '" + Deskripsi + "', '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' ,'" + CanvasSeqNo + "', '" + SeqNoGroup + "',getdate(), '" + ControlMgr.UserId + "', '" + Base + "');";
                        //Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty * Ratio + "', '" + Qty + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "', '" + Deskripsi + "', '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' ,'" + CanvasSeqNo + "', '" + SeqNoGroup + "',getdate(), '" + ControlMgr.UserId + "', '" + Base + "');";
                        Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty * Ratio + "', '" + Qty + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "', '" + Deskripsi + "', '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' , '" + SeqNoGroup + "',getdate(), '" + ControlMgr.UserId + "', '" + Base + "');";

                    else
                        //Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty + "', '" + Qty_KG + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "', '" + Deskripsi + "', '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' ,'" + CanvasSeqNo + "', '" + SeqNoGroup + "',getdate(), '" + ControlMgr.UserId + "', '" + Base + "');";
                        Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty + "', '" + Qty_KG + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "', '" + Deskripsi + "', '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' , '" + SeqNoGroup + "',getdate(), '" + ControlMgr.UserId + "', '" + Base + "');";
                    //ListMethod1.POIssued(Conn, FullItemId, Qty, Unit);
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    if (cmbReffTableName.Text == "Canvass Sheet")
                    {
                        //Update QtyPRIssued
                        if (txtTransType.Text != "AMOUNT")
                        {
                            FullItemId = dgvPODetails1.Rows[i].Cells["FullItemID"].Value.ToString();
                            decimal QtyInput = decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                            Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                            decimal ConvRatio = 0;

                            string QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(QueryTemp, Conn);
                            string UoM = Cmd.ExecuteScalar().ToString();

                            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(QueryTemp, Conn);
                            ConvRatio = (decimal)Cmd.ExecuteScalar();


                            if (Unit == UoM)
                            {
                                QtyUoM = QtyInput;
                                QtyAlt = QtyInput * ConvRatio;
                            }
                            else
                            {

                                QtyAlt = QtyInput;
                                QtyUoM = QtyInput / ConvRatio;
                                QtyAmount = QtyAlt * Price_KG;
                            }

                            decimal QtyPRIssued_UoM = 0;
                            decimal QtyPRIssued_Alt = 0;
                            decimal QtyPRIssued_Amount = 0;

                            //Steven Edit save to PurchRequisition_LogTable
                            QueryTemp = "Select [CanvasDate] from [CanvasSheetH] where [CanvasId]='" + txtReffID.Text.Trim() + "'";
                            Cmd = new SqlCommand(QueryTemp, Conn);
                            DateTime TmpDate = Convert.ToDateTime(Cmd.ExecuteScalar() == null ? "1900-01-01" : Cmd.ExecuteScalar());

                            //Steven Edit save to PurchRequisition_LogTable
                            insertPO_LogTable(dtPODate.Value, PurchOrderId, txtVendId.Text.Trim(), QtyUoM, QtyAlt, txtReffID.Text.Trim(), TmpDate, "01", "Open Order", "Open Order", SeqNo == "" ? 0 : Convert.ToInt32(SeqNo));

                            QueryTemp = "Select PO_Issued_Outstanding_UoM, PO_Issued_Outstanding_Alt,[PO_Issued_Outstanding_Amount] From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(QueryTemp, Conn);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    QtyPRIssued_UoM = decimal.Parse(Dr["PO_Issued_Outstanding_UoM"].ToString());
                                    QtyPRIssued_Alt = decimal.Parse(Dr["PO_Issued_Outstanding_Alt"].ToString());
                                    QtyPRIssued_Amount = decimal.Parse(Dr["PO_Issued_Outstanding_Amount"].ToString());
                                }
                            }

                            ////Update ke PR_CS_Approved_UoM
                            Query = "Update Invent_Purchase_Qty Set PR_CS_Approved_UoM = PR_CS_Approved_UoM-" + QtyUoM + ", PR_CS_Approved_Alt = PR_CS_Approved_Alt-" + QtyAlt + " , PO_Issued_Outstanding_UoM = PO_Issued_Outstanding_UoM + " + QtyUoM + ", PO_Issued_Outstanding_Alt = PO_Issued_Outstanding_Alt + " + QtyAlt + "  Where FullItemID = '" + FullItemId + "'";
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else if (cmbReffTableName.Text == "Purchase Agreement")
                    {
                        //Update QtyPRIssued
                        if (txtTransType.Text != "AMOUNT")
                        {
                            FullItemId = dgvPODetails1.Rows[i].Cells["FullItemID"].Value.ToString();
                            decimal QtyInput = decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                            Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                            decimal ConvRatio = 0;

                            string QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(QueryTemp, Conn);
                            string UoM = Cmd.ExecuteScalar().ToString();

                            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(QueryTemp, Conn);
                            ConvRatio = (decimal)Cmd.ExecuteScalar();

                            if (Unit == UoM)
                            {

                                QtyUoM = QtyInput;
                                QtyAlt = QtyInput * ConvRatio;
                            }
                            else
                            {
                                QtyAlt = QtyInput;
                                QtyUoM = QtyInput / ConvRatio;
                            }

                            //Update ke PR_CS_Approved_UoM
                            Query = "Update Invent_Purchase_Qty Set PO_Issued_Outstanding_UoM = PO_Issued_Outstanding_UoM-" + QtyUoM + ", PO_Issued_Outstanding_Alt = PO_Issued_Outstanding_Alt-" + QtyAlt + ", PO_From_PA_Issued_UoM = PO_From_PA_Issued_UoM + " + QtyUoM + ", PO_From_PA_Issued_Alt = PO_From_PA_Issued_Alt + " + QtyAlt + "  Where FullItemID = '" + FullItemId + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            Query = "";
                        }
                        else
                        {
                            Double Amount = ((dgvPODetails1.Rows[i].Cells["TotalNett"].Value == "" ? 0.0000 : Double.Parse(dgvPODetails1.Rows[i].Cells["Total"].Value.ToString())));

                            //TmpQty = Amount
                            Query = "Update Invent_Purchase_Qty Set PO_From_PA_Issued_Amount = (PO_From_PA_Issued_Amount + " + Amount + "), PO_Issued_Outstanding_Amount= (PO_Issued_Outstanding_Amount - " + Amount + ")   Where FullItemID = '" + (dgvPODetails1.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["FullItemID"].Value.ToString()) + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }

                        Query = "SELECT [OrderDate] FROM [dbo].[PurchAgreementH] where [AgreementID]='" + txtReffID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        PaDate = Convert.ToDateTime(Cmd.ExecuteScalar() == null ? "1900-01-01" : Cmd.ExecuteScalar());

                        //save to PurchRequisition_LogTable
                        insertPO_LogTable(dtPODate.Value, PurchOrderId, txtVendId.Text.Trim(), QtyUoM, QtyAlt, txtReffID.Text.Trim(), PaDate, "01", "Open Order", "Open Order", SeqNo == "" ? 0 : Convert.ToInt32(SeqNo));

                        String tmpUnit = "";
                        decimal tmpSeqNoGroup = 0;
                        decimal tmpSeqNo = 0;
                        decimal tmpQty = 0;

                        Query = "Select Unit,SeqNoGroup,SeqNo from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "' and Base='Y'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            tmpUnit = Dr["Unit"].ToString();
                            tmpSeqNoGroup = decimal.Parse(Dr["SeqNoGroup"].ToString());
                            tmpSeqNo = decimal.Parse(Dr["SeqNo"].ToString());
                        }
                        Dr.Close();

                        #region Update Purchase Agreement
                        if (txtTransType.Text.ToUpper() == "FIX")
                        {
                            if (tmpUnit == Unit && tmpSeqNoGroup == SeqNoGroup)
                            {
                                Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + Qty + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                            }
                            else if (tmpUnit == "BTG" && (Unit == "KG" || Unit == "LBR") && tmpSeqNoGroup == SeqNoGroup)
                            {
                                decimal QtyNew = 0;
                                QtyNew = Qty / Ratio;

                                Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                            }
                            else if ((tmpUnit == "KG" || tmpUnit == "LBR") && Unit == "BTG" && tmpSeqNoGroup == SeqNoGroup)
                            {
                                decimal QtyNew = 0;
                                QtyNew = Qty * Ratio;

                                Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                            }
                        }
                        else
                        {
                            if (dgvPODetails1.Rows[i].Cells["Base"].Value.ToString() == "Y" || dgvPODetails1.Rows[i].Cells["Base"].Value.ToString() == "N")
                            {
                                if (txtTransType.Text.ToUpper() == "QTY")
                                {
                                    decimal KG;
                                    if (Unit == "KG")
                                        KG = Qty;
                                    else
                                        KG = Qty * Ratio;
                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + KG + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "' and Base='Y'";
                                }
                                else
                                {
                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingAmount = (RemainingAmount - " + TotalNett + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "' and Base='Y'";
                                }
                            }
                            else
                            {
                                if (tmpUnit == Unit && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    decimal KG = 0;
                                    if (Unit == "KG")
                                        KG = Qty;
                                    else
                                        KG = Qty * Ratio;
                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + Qty + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                                else if (tmpUnit == "BTG" && (Unit == "KG" || Unit == "LBR") && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    decimal QtyNew = 0;
                                    QtyNew = Qty / Ratio;

                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                                else if ((tmpUnit == "KG" || tmpUnit == "LBR") && Unit == "BTG" && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    decimal QtyNew = 0;
                                    QtyNew = Qty * Ratio;

                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (RemainingQty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                            }
                        }
                        #endregion

                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        SavePurchAmountTable(PurchOrderId);
                    }
                }
            }
        }
        #endregion


        private void SaveEdit()
        {
            //Sebelum di delete, hrs bandingin qty lama dan baru
            string FullItemId = "";
            string Unit = "";
            string UoM = "";
            decimal ConvRatio = 0;
            string QueryTemp = "";
            decimal QtyOld = 0;
            decimal QtyUoMOld = 0;
            decimal QtyAltOld = 0;

            decimal QtyNew = 0;
            decimal QtyUoMNew = 0;
            decimal QtyAltNew = 0;
            decimal TmpQtyUoMNew = 0;
            decimal TmpQtyAltNew = 0;
            decimal QtyPRIssued_UoM = 0;
            decimal QtyPRIssued_Alt = 0;

            try
            {
                //delete attachment
                Query = "DELETE FROM [dbo].[tblAttachments] WHERE [ReffTableName] = 'POForm' AND [ReffTransID] = @ReffTransID";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@ReffTransID", txtPOId.Text);
                    Cmd.ExecuteNonQuery();
                }
                //insert new edited attachment
                InsertAttachment(Conn, txtPOId.Text);

                //Update QtyPRIssued
                if (txtTransType.Text != "AMOUNT")
                {
                    for (int i = 0; i <= dgvPODetails1.RowCount - 1; i++)
                    {
                        FullItemId = dgvPODetails1.Rows[i].Cells["FullItemID"].Value.ToString();
                        Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                        ConvRatio = 0;

                        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        UoM = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        ConvRatio = (decimal)Cmd.ExecuteScalar();

                        if (Unit == UoM)
                        {
                            QtyUoMNew = decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                            QtyAltNew = decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString()) * ConvRatio;
                        }
                        else
                        {
                            QtyAltNew = decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                            QtyUoMNew = decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString()) / ConvRatio;
                        }

                        Query = "Select Qty From PurchDtl Where PurchID='" + txtPOId.Text + "' and FullItemID = '" + dgvPODetails1.Rows[i].Cells["FullItemID"].Value.ToString() + "' and SeqNo = '" + dgvPODetails1.Rows[i].Cells["SeqNo"].Value.ToString() + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal OldQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                        if (Unit == UoM)
                        {
                            TmpQtyUoMNew = (-1 * OldQty) + QtyUoMNew;
                            TmpQtyAltNew = ((-1 * OldQty) + QtyUoMNew) * ConvRatio;
                        }
                        else
                        {
                            TmpQtyAltNew = (-1 * OldQty) + QtyAltNew;
                            TmpQtyUoMNew = ((-1 * OldQty) + QtyAltNew) / ConvRatio;
                        }

                        if (cmbReffTableName.Text != "PA")
                            QueryTemp = "Select PO_Issued_Outstanding_UoM,PO_Issued_Outstanding_Alt From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "';";
                        else
                            QueryTemp = "Select PO_FROM_PA_Issued_UoM,PO_FROM_PA_Issued_ALT From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "';";

                        Cmd = new SqlCommand(QueryTemp, Conn);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                QtyPRIssued_UoM = decimal.Parse(Dr[0].ToString());
                                QtyPRIssued_Alt = decimal.Parse(Dr[1].ToString());
                            }
                        }

                        Dr.Close();

                        TmpQtyUoMNew = TmpQtyUoMNew + QtyPRIssued_UoM;
                        TmpQtyAltNew = TmpQtyAltNew + QtyPRIssued_Alt;

                        if (cmbReffTableName.Text != "PA")
                            Query = "Update Invent_Purchase_Qty Set PO_Issued_Outstanding_UoM = " + TmpQtyUoMNew + ", PO_Issued_Outstanding_Alt = " + TmpQtyAltNew + "  Where FullItemID = '" + FullItemId + "'";
                        else
                            Query = "Update Invent_Purchase_Qty Set PO_FROM_PA_Issued_UoM = " + TmpQtyUoMNew + ", PO_FROM_PA_Issued_Alt = " + TmpQtyAltNew + "  Where FullItemID = '" + FullItemId + "'";

                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        Query = "Select Qty From PurchDtl Where PurchID='" + txtPOId.Text + "' and FullItemID = '" + dgvPODetails1.Rows[i].Cells["FullItemID"].Value.ToString() + "' and ReffBaseSeqNo = '" + dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        OldQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                        //Update PurchaseAgreement saat edit menambahkan remaining QTY
                        if (cmbReffTableName.Text == "PA")
                        {
                            Query = "Select Unit From PurchDtl Where PurchID='" + txtPOId.Text + "' and FullItemID = '" + dgvPODetails1.Rows[i].Cells["FullItemID"].Value.ToString() + "' and ReffBaseSeqNo = '" + dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            string TmpUnit = Cmd.ExecuteScalar().ToString();

                            if (TmpUnit == "KG")
                            {
                                if (dgvPODetails1.Rows[i].Cells["Base"].Value.ToString() != "")
                                {
                                    Query = "Update PurchAgreementDtl Set RemainingQty = (RemainingQty + " + OldQty + ") Where AgreementID ='" + txtReffID.Text + "' and SeqNoGroup='" + dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "' and Base='Y';";
                                }
                                else
                                {
                                    Query = "Update PurchAgreementDtl Set RemainingQty = (RemainingQty + " + OldQty + ") Where AgreementID ='" + txtReffID.Text + "' and SeqNoGroup='" + dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "';";
                                }
                            }
                            else
                            {
                                if (dgvPODetails1.Rows[i].Cells["Base"].Value.ToString() != "")
                                {
                                    Query = "Update PurchAgreementDtl Set RemainingQty = (RemainingQty + " + (OldQty * decimal.Parse(dgvPODetails1.Rows[i].Cells["Ratio"].Value.ToString())) + ") Where AgreementID ='" + txtReffID.Text + "' and SeqNoGroup='" + dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "' and Base='Y';";
                                }
                                else
                                {
                                    Query = "Update PurchAgreementDtl Set RemainingQty = (RemainingQty + " + (OldQty * decimal.Parse(dgvPODetails1.Rows[i].Cells["Ratio"].Value.ToString())) + ") Where AgreementID ='" + txtReffID.Text + "' and SeqNoGroup='" + dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "';";
                                }
                            }
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    Query = "SELECT h.ReffId, d.ReffBaseSeqNo, d.Total_Nett FROM PurchDtl d INNER JOIN PurchH h ";
                    Query += "ON d.PurchID = h.PurchID WHERE h.ReffTableName = 'PA' AND d.PurchID = '" + txtPOId.Text + "'";
                    using (SqlCommand cmd2 = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd2.ExecuteReader();
                        while (Dr.Read())
                        {
                            decimal AmountPO = Convert.ToDecimal(Dr["Total_Nett"]);
                            decimal ReffBaseSeqNo = Convert.ToDecimal(Dr["ReffBaseSeqNo"]);
                            string PAID = Convert.ToString(Dr["ReffId"]);

                            Query = "Update PurchAgreementDtl set RemainingAmount = (RemainingAmount + " + AmountPO + ") where AgreementID = '" + PAID + "' and SeqNoGroup = '" + ReffBaseSeqNo + "' and Base='Y'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    Dr.Close();
                }

                //Query = "Update [dbo].[PurchH] set OrderDate = '" + dtPODate.Value.Date + "' , DueDate = '" + dtDueDate.Value.Date + "', TransType = '" + txtTransType.Text + "', ReffTableName = '" + cmbReffTableName.Text + "', ReffId ='" + txtReffID.Text + "', ReffId2 ='" + txtReffID2.Text + "', CurrencyId = '" + txtCurrencyID.Text + "', ExchRate = '" + (txtExchRate.Text == "" ? "0" : txtExchRate.Text) + "', VendId = '" + txtVendId.Text + "', DP = '" + (txtDP.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtDP.Text).ToString()) + "', Total = '" + (txtTotal.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotal.Text).ToString()) + "', Total_Disk = '" + (txtTotalDisk.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalDisk.Text).ToString()) + "', PPN = '" + (cmbPPn.Text == "" ? "0" : cmbPPn.Text) + "', PPH = '" + (cmbPPh.Text == "" ? "0" : cmbPPh.Text) + "',  Total_PPN = '" + (txtTotalPPN.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPN.Text).ToString()) + "', Total_PPH = '" + (txtTotalPPH.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPH.Text).ToString()) + "', Total_Nett = '" + (txtTotalNett.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalNett.Text).ToString()) + "', Deskripsi = '" + txtDeskripsi.Text + "', TermofPayment = '" + txtToP.Text + "', PaymentMode = '" + txtPaymentMode.Text + "', UpdatedDate = getDate(), UpdatedBy = '" + ControlMgr.UserId + "' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy where PurchId = '" + txtPOId.Text + "' ";


                //BEGIN STEVEN EDIT
                Query = "Update [dbo].[PurchH] set ";
                Query += "OrderDate = '" + dtPODate.Value.Date + "',";
                Query += "DueDate = '" + dtDueDate.Value.Date + "',";
                Query += "TransType = '" + txtTransType.Text + "',";
                Query += "ReffTableName = '" + cmbReffTableName.Text + "',";
                Query += "ReffId = '" + txtReffID.Text + "',";
                Query += "ReffId2 = '" + txtReffID2.Text + "',";
                Query += "CurrencyId = '" + cmbCurrency.Text + "',";
                Query += "ExchRate = '" + (txtExchRate.Text == "" ? "0" : txtExchRate.Text) + "',";
                Query += "VendId = '" + txtVendId.Text + "',";
                //Query += "DP = '" + (txtDPPercent.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtDPPercent.Text).ToString()) + "',"; //REMARKED BY: HC
                //BY: HC (S)
                if (cmbDPRequired.Text == "YES")
                {
                    if (cmbDPType.Text == "Percentage")
                    {
                        Query += "DP = '" + tbxDPPercent.Text + "', ";
                        Query += "DPAmount = '" + Convert.ToDecimal(tbxDPPercent.Text) * Convert.ToDecimal(txtTotalNett.Text) / 100 + "', ";
                    }
                    else if (cmbDPType.Text == "Amount")
                    {
                        Query += "DP = '" + Convert.ToDecimal(tbxDPAmount.Text) * 100 / Convert.ToDecimal(txtTotalNett.Text) + "', ";
                        Query += "DPAmount = '" + tbxDPAmount.Text + "', ";
                    }
                }
                else if (cmbDPRequired.Text == "NO")
                {
                    Query += "DP = '0', ";
                    Query += "DPAmount = '0', ";
                }
                Query += "DPType = @DPType, ";
                //BY: HC (E)
                Query += "Total = '" + (txtTotal.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotal.Text).ToString()) + "',";
                Query += "Total_Disk = '" + (txtTotalDisk.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalDisk.Text).ToString()) + "',";
                Query += "PPN = '" + (cmbPPn.Text == "" ? "0" : cmbPPn.Text) + "',";
                Query += "PPH = '" + (cmbPPh.Text == "" ? "0" : cmbPPh.Text) + "',";
                Query += "Total_PPN = '" + (txtTotalPPN.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPN.Text).ToString()) + "',";
                Query += "Total_PPH = '" + (txtTotalPPH.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPH.Text).ToString()) + "',";
                Query += "Total_Nett = '" + (txtTotalNett.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalNett.Text).ToString()) + "',";
                Query += "Deskripsi = @Deskripsi,";
                //Query += "TermofPayment = '" + txtToP.Text + "',";
                Query += "TermofPayment = '" + cmbTermOfPayment.Text + "',";
                Query += "PaymentMode = '" + cmbPaymentMode.Text + "',";
                Query += "UpdatedDate = getDate(),";
                Query += "UpdatedBy = '" + ControlMgr.UserId + "' ";
                //Query += "OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy";
                Query += "where PurchId = '" + txtPOId.Text + "' ";
                //END STEVEN EDIT

                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text.Trim());
                Cmd.Parameters.AddWithValue("@DPType", cmbDPRequired.Text.ToUpper() == "NO" ? "" : cmbDPType.Text);
                Cmd.ExecuteNonQuery();
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                //    CreatedBy = Dr["CreatedBy"].ToString();
                //}
                //Dr.Close();

                if (dgvPODetails1.Rows.Count > 0)
                {
                    Query1 = "Delete from [dbo].[PurchDtl] where PurchId = '" + txtPOId.Text.Trim() + "' ";
                    Cmd = new SqlCommand(Query1, Conn);
                    Cmd.ExecuteNonQuery();
                    for (int j = 0; j <= dgvPODetails1.Rows.Count - 1; j++)
                    {
                        if (dgvPODetails1.Rows[j].Cells["AvailableDate"].Value == null || dgvPODetails1.Rows[j].Cells["AvailableDate"].Value == "")
                            dgvPODetails1.Rows[j].Cells["AvailableDate"].Value = "01-01-1900";

                        String SeqNo = dgvPODetails1.Rows[j].Cells["No"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["No"].Value.ToString();
                        String OrderDate = dgvPODetails1.Rows[j].Cells["OrderDate"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["OrderDate"].Value.ToString();
                        String GroupId = dgvPODetails1.Rows[j].Cells["GroupId"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["GroupId"].Value.ToString();
                        String SubGroup1Id = dgvPODetails1.Rows[j].Cells["SubGroup1Id"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["SubGroup1Id"].Value.ToString();
                        String SubGroup2Id = dgvPODetails1.Rows[j].Cells["SubGroup2Id"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["SubGroup2Id"].Value.ToString();
                        String ItemId = dgvPODetails1.Rows[j].Cells["ItemId"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["ItemId"].Value.ToString();
                        FullItemId = dgvPODetails1.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["FullItemId"].Value.ToString();
                        String ItemName = dgvPODetails1.Rows[j].Cells["ItemName"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["ItemName"].Value.ToString();
                        String InventSiteId = dgvPODetails1.Rows[j].Cells["InventSiteId"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["InventSiteId"].Value.ToString();
                        decimal Qty = dgvPODetails1.Rows[j].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["Qty"].Value.ToString());
                        decimal RemainingQty = dgvPODetails1.Rows[j].Cells["RemainingQty"].Value == null ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["RemainingQty"].Value.ToString());
                        Unit = dgvPODetails1.Rows[j].Cells["Unit"].Value == "" ? "" : dgvPODetails1.Rows[j].Cells["Unit"].Value.ToString();
                        decimal Ratio = dgvPODetails1.Rows[j].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["Ratio"].Value.ToString());
                        decimal Price = dgvPODetails1.Rows[j].Cells["Price"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["Price"].Value.ToString());
                        decimal Total = dgvPODetails1.Rows[j].Cells["Total"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["Total"].Value.ToString());
                        decimal Diskon = dgvPODetails1.Rows[j].Cells["Diskon(%)"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["Diskon(%)"].Value.ToString());
                        decimal TotalDisk = dgvPODetails1.Rows[j].Cells["TotalDisk"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["TotalDisk"].Value.ToString());
                        decimal TotalPPN = dgvPODetails1.Rows[j].Cells["TotalPPN"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["TotalPPN"].Value.ToString());
                        decimal TotalPPh = dgvPODetails1.Rows[j].Cells["TotalPPh"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["TotalPPh"].Value.ToString());
                        decimal TotalNett = dgvPODetails1.Rows[j].Cells["TotalNett"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["TotalNett"].Value.ToString());
                        String Deskripsi = dgvPODetails1.Rows[j].Cells["Deskripsi"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["Deskripsi"].Value.ToString().Trim();
                        DateTime AvailableDate = dgvPODetails1.Rows[j].Cells["AvailableDate"].Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dgvPODetails1.Rows[j].Cells["AvailableDate"].Value);
                        String DiscScheme = dgvPODetails1.Rows[j].Cells["DiscScheme"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["DiscScheme"].Value.ToString();
                        String BonusScheme = dgvPODetails1.Rows[j].Cells["BonusScheme"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["BonusScheme"].Value.ToString();
                        String CashBackScheme = dgvPODetails1.Rows[j].Cells["CashBackScheme"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["CashBackScheme"].Value.ToString();
                        String DeliveryMethod = dgvPODetails1.Rows[j].Cells["DeliveryMethod"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                        String CanvasSeqNo = dgvPODetails1.Rows[j].Cells["CanvasSeqNo"].Value == null ? "" : dgvPODetails1.Rows[j].Cells["CanvasSeqNo"].Value.ToString();
                        decimal SeqNoGroup = dgvPODetails1.Rows[j].Cells["SeqNoGroup"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[j].Cells["SeqNoGroup"].Value.ToString());
                        decimal Qty_KG = 0;
                        decimal Price_KG = 0;
                        string Base = dgvPODetails1.Rows[j].Cells["Base"].Value == "" ? "" : dgvPODetails1.Rows[j].Cells["Base"].Value.ToString();

                        if (Unit == "BTG")
                        {
                            if (Ratio > 0)
                            {
                                Qty_KG = Qty / Ratio;
                                Price_KG = Price / Ratio;
                            }
                        }
                        else
                        {
                            Qty_KG = Qty;
                            Price_KG = Price;
                        }

                        Query = "Insert into [dbo].[PurchDtl] (PurchId, OrderDate, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, InventSiteId, Qty, Qty_KG, RemainingQty, Unit, Konv_Ratio, Price, Price_KG, Total, Diskon, Total_Disk, Total_PPN, Total_PPh, Total_Nett, Deskripsi, AvailableDate, DiscScheme, BonusScheme, CashBackScheme, DeliveryMethod, ReffTableName, ReffId, Reffid2, ReffSeqNo, ReffBaseSeqNo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy,Base) ";
                        if (Unit != "KG")
                            Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty * Ratio + "', '" + Qty_KG + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "', @Deskripsi, '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' ,'" + CanvasSeqNo + "','" + SeqNoGroup + "','" + CreatedDate + "', '" + CreatedBy + "',getdate(), '" + ControlMgr.UserId + "','" + Base + "');";
                        else
                            Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty + "', '" + Qty_KG + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', '" + TotalNett + "',@Deskripsi, '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' ,'" + CanvasSeqNo + "','" + SeqNoGroup + "','" + CreatedDate + "', '" + CreatedBy + "',getdate(), '" + ControlMgr.UserId + "','" + Base + "');";

                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.AddWithValue("@Deskripsi", Deskripsi);
                        Cmd.ExecuteNonQuery();

                        if (cmbReffTableName.Text == "CanvasSheet")
                        {
                            Query = "SELECT [OrderDate] FROM [dbo].[PurchAgreementH] where [AgreementID]='" + txtReffID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            PaDate = Convert.ToDateTime(Cmd.ExecuteScalar() == null ? "1900-01-01" : Cmd.ExecuteScalar());

                            //save to PurchRequisition_LogTable
                            Query = "Update [PO_Issued_LogTable] set [PODate]='" + dtPODate.Value.ToString("yyyy-MM-dd") + "',";
                            Query += "[POId]='" + txtPOId.Text + "',";
                            Query += "[VendId]='" + txtVendId.Text + "',";
                            Query += "[Qty_UoM]='" + QtyUoM + "',";
                            Query += "[Qty_Alt]='" + QtyAlt + "',";
                            Query += "[PAId]='" + txtReffID.Text + "',";
                            Query += "[PADate]='" + PaDate + "',";
                            Query += "[LogStatusCode]='01',";
                            Query += "[LogStatusDesc]='Open Order.',";
                            Query += "[LogDescription]='Open Order.',";
                            Query += "[UserID]='" + ControlMgr.UserId + "',";
                            Query += "[LogDate]=getdate(),";
                            Query += "[POSeqNo]='" + SeqNo + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        else if (cmbReffTableName.Text == "PA")
                        {
                            String tmpUnit = "";
                            decimal tmpSeqNoGroup = 0;
                            decimal tmpQty = 0;
                            decimal tmpRemainingQty = 0;

                            Query = "Select Unit,SeqNoGroup,RemainingQty from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                tmpUnit = Dr["Unit"].ToString();
                                tmpSeqNoGroup = decimal.Parse(Dr["SeqNoGroup"].ToString());
                            }
                            Dr.Close();

                            if (txtTransType.Text.ToUpper() != "AMOUNT")
                            {
                                if (tmpUnit == Unit && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (Qty - " + Qty + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                                else if (tmpUnit == "BTG" && (Unit == "KG" || Unit == "LBR") && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    QtyNew = 0;
                                    QtyNew = Qty / Ratio;

                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (Qty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                                else if ((tmpUnit == "KG" || tmpUnit == "LBR") && Unit == "BTG" && tmpSeqNoGroup == SeqNoGroup)
                                {
                                    QtyNew = 0;
                                    QtyNew = Qty * Ratio;

                                    Query = "Update [dbo].[PurchAgreementDtl] set RemainingQty = (Qty - " + QtyNew + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                }
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                            }

                            //Update PurchaseAgreement saat edit mengurangkan remaining QTY
                            if (cmbReffTableName.Text == "PA")
                            {
                                if (txtTransType.Text.ToUpper() != "AMOUNT")
                                {
                                    Query = "Select Unit From PurchDtl Where PurchID='" + txtPOId.Text + "' and FullItemID = '" + dgvPODetails1.Rows[j].Cells["FullItemID"].Value.ToString() + "' and ReffBaseSeqNo = '" + dgvPODetails1.Rows[j].Cells["SeqNoGroup"].Value.ToString() + "';";
                                    Cmd = new SqlCommand(Query, Conn);
                                    string TmpUnit = Cmd.ExecuteScalar().ToString();

                                    if (TmpUnit == "KG")
                                    {
                                        if (dgvPODetails1.Rows[j].Cells["Base"].Value.ToString() != "")
                                        {
                                            Query = "Update PurchAgreementDtl Set RemainingQty = (RemainingQty - " + dgvPODetails1.Rows[j].Cells["Qty"].Value.ToString() + ") Where AgreementID ='" + txtReffID.Text + "' and SeqNoGroup='" + dgvPODetails1.Rows[j].Cells["SeqNoGroup"].Value.ToString() + "' and Base='Y';";
                                        }
                                        else
                                        {
                                            Query = "Update PurchAgreementDtl Set RemainingQty = (RemainingQty - " + dgvPODetails1.Rows[j].Cells["Qty"].Value.ToString() + ") Where AgreementID ='" + txtReffID.Text + "' and SeqNoGroup='" + dgvPODetails1.Rows[j].Cells["SeqNoGroup"].Value.ToString() + "';";
                                        }
                                    }
                                    else
                                    {
                                        if (dgvPODetails1.Rows[j].Cells["Base"].Value.ToString() != "")
                                        {
                                            Query = "Update PurchAgreementDtl Set RemainingQty = (RemainingQty - " + (Convert.ToDecimal(dgvPODetails1.Rows[j].Cells["Qty"].Value) * Convert.ToDecimal(dgvPODetails1.Rows[j].Cells["Ratio"].Value)) + ") Where AgreementID ='" + txtReffID.Text + "' and SeqNoGroup='" + dgvPODetails1.Rows[j].Cells["SeqNoGroup"].Value.ToString() + "' and Base='Y';";
                                        }
                                        else
                                        {
                                            Query = "Update PurchAgreementDtl Set RemainingQty = (RemainingQty - " + (Convert.ToDecimal(dgvPODetails1.Rows[j].Cells["Qty"].Value) * Convert.ToDecimal(dgvPODetails1.Rows[j].Cells["Ratio"].Value)) + ") Where AgreementID ='" + txtReffID.Text + "' and SeqNoGroup='" + dgvPODetails1.Rows[j].Cells["SeqNoGroup"].Value.ToString() + "';";
                                        }
                                    }
                                }
                                else
                                {
                                    if (dgvPODetails1.Rows[j].Cells["Base"].Value.ToString() != "")
                                    {
                                        Query = "Update [dbo].[PurchAgreementDtl] set RemainingAmount = (RemainingAmount - " + TotalNett + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "' and Base='Y'";
                                    }
                                    else
                                    {
                                        Query = "Update [dbo].[PurchAgreementDtl] set RemainingAmount = (RemainingAmount - " + TotalNett + ") where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "'";
                                    }
                                }
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                            }

                            SavePurchAmountTable(txtPOId.Text);
                            //Query = "SELECT COUNT(AgreementID) CountData FROM PurchAgreementDtl WHERE RemainingQty <> 0 AND AgreementID = '" + txtReffID.Text + "'";
                            //Cmd = new SqlCommand(Query, Conn, Trans);
                            //int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                            //if (CountData == 0)
                            //{
                            //    Query = "UPDATE PurchAgreementH SET StClose = '1', TransStatus = '06' WHERE AgreementID = '" + txtReffID.Text + "' ";
                            //    Cmd = new SqlCommand(Query, Conn, Trans);
                            //    Cmd.ExecuteNonQuery();
                            //}                                
                        }
                    }
                }
                MessageBox.Show("Data :" + PurchOrderId + " Berhasil diupdate.");
                ModeBeforeEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void SaveAmend()
        {
            try
            {
                deleteOldInventPurchaseOty();

                string Jenis = "PO", Kode = "PO";
                PurchOrderId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "PurchH", txtReffID.Text, Conn, Cmd);
                PONumber = PurchOrderId;
                Query = "Insert into [dbo].[PurchH] (PurchId,OrderDate,DueDate,TransType,ReffTableName,ReffId,ReffId2,CurrencyId,ExchRate,VendId,Total,Total_Disk,PPN,Total_PPN,PPh,Total_PPh,Total_Nett,Deskripsi,TransStatus,TermofPayment,PaymentMode,CreatedDate,CreatedBy, DP, DPAmount, DPType) values (";
                Query += "'" + PurchOrderId + "',";
                Query += "'" + dtPODate.Value.ToString("yyyy-MM-dd") + "',";
                Query += "'" + dtDueDate.Value.ToString("yyyy-MM-dd") + "',";
                Query += "'" + txtTransType.Text + "',";
                Query += "'" + cmbReffTableName.Text + "',";
                Query += "'" + txtReffID.Text + "',";
                Query += "'" + txtReffID2.Text + "',";
                Query += "'" + cmbCurrency.Text + "',";
                Query += "'" + (txtExchRate.Text == "" ? "0" : txtExchRate.Text) + "',";
                Query += "'" + txtVendId.Text + "',";
                //Query += txtDPPercent.Text == "" ? "'0'," : "'" + txtDPPercent.Text + "',"; //REMARKED BY: HC
                Query += "'" + (txtTotal.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotal.Text).ToString()) + "',";
                Query += "'" + (txtTotalDisk.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalDisk.Text).ToString()) + "',";
                Query += "'" + (cmbPPn.Text == "" ? "0" : cmbPPn.Text) + "',";
                Query += "'" + (txtTotalPPN.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPN.Text).ToString()) + "',";
                Query += "'" + (cmbPPh.Text == "" ? "0" : cmbPPh.Text) + "',";
                Query += "'" + (txtTotalPPH.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalPPH.Text).ToString()) + "',";
                Query += "'" + (txtTotalNett.Text.ToString() == "" ? "0" : Convert.ToDecimal(txtTotalNett.Text).ToString()) + "',";
                Query += "@Deskripsi,";
                Query += "'04',";
                //Query += "'" + txtToP.Text + "',";
                Query += "'" + cmbTermOfPayment.Text + "',";
                Query += "'" + cmbPaymentMode.Text + "',";
                Query += "getdate(),";
                Query += "'" + ControlMgr.UserId + "', ";
                //BY: HC (S)
                if (cmbDPRequired.Text == "YES")
                {
                    if (cmbDPType.Text == "Percentage")
                    {
                        Query += "'" + tbxDPPercent.Text + "', ";
                        Query += "'" + Convert.ToDecimal(tbxDPPercent.Text) * Convert.ToDecimal(txtTotalNett.Text) / 100 + "', '";
                    }
                    else if (cmbDPType.Text == "Amount")
                    {
                        Query += "'" + Convert.ToDecimal(tbxDPAmount.Text) * 100 / Convert.ToDecimal(txtTotalNett.Text) + "', ";
                        Query += "'" + tbxDPAmount.Text + "', '";
                    }
                }
                else if (cmbDPRequired.Text == "NO")
                {
                    Query += "'0', '0', '";
                }
                Query += cmbDPRequired.Text.ToUpper() == "NO" ? "" : cmbDPType.Text;
                Query += "'); ";
                //BY: HC (E)
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text.Trim());
                Cmd.ExecuteNonQuery();

                Query = "Update PurchH Set StClose = 1, TransStatus = '07' Where PurchID = '" + txtReffID.Text + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                //created by: Thaddaeus Matthias, 14 March 2018
                // inserting status log
                insertstatuslogclose();
                insertstatuslog(PurchOrderId);

                //BY: HC (S) | INSERT KE TABLE LOG PO
                Query = "Insert into [PO_Issued_LogTable] ([PODate],[POId],[VendId],[Qty_UoM],[Qty_Alt],[PAId],[PADate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[POSeqNo]) ";
                Query += "Select top 1 PODate, POId, VendId, Qty_UoM, Qty_Alt, PAId, PADate, '07', 'Closed', 'Closed', '" + ControlMgr.UserId + "', getdate(), POSeqNo from PO_Issued_LogTable where POId = '" + txtReffID.Text + "' order by LogDate desc";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                DateTime dtReff = new DateTime(1900, 1, 1);
                Query = "select OrderDate from PurchH where PurchID = '" + txtReffID.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                dtReff = Convert.ToDateTime(Cmd.ExecuteScalar());
                insertPO_LogTable(dtPODate.Value, txtPOId.Text, txtVendId.Text.Trim(), 0, 0, txtReffID.Text.Trim(), dtReff, "04", "Amend - Waiting for Approval", "Amend - Waiting for Approval", 0);
                //BY: HC (E)

                ListMethod ListMethod1 = new ListMethod();

                if (dgvPODetails1.Rows.Count > 0)
                {
                    for (int i = 0; i <= dgvPODetails1.Rows.Count - 1; i++)
                    {
                        if (dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == null || dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == "")
                            dgvPODetails1.Rows[i].Cells["AvailableDate"].Value = "01-01-1900";

                        String SeqNo = dgvPODetails1.Rows[i].Cells["No"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["No"].Value.ToString();
                        //String OrderDate = dgvPODetails1.Rows[i].Cells["OrderDate"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["OrderDate"].Value.ToString();
                        //String OrderDate = dgvPODetails1.Rows[i].Cells["OrderDate"].Value == null ? "" : FormateDateddmmyyyy(dgvPODetails1.Rows[i].Cells["OrderDate"].Value.ToString());
                        DateTime OrderDate = dgvPODetails1.Rows[i].Cells["OrderDate"].Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dgvPODetails1.Rows[i].Cells["OrderDate"].Value);
                        String GroupId = dgvPODetails1.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["GroupId"].Value.ToString();
                        String SubGroup1Id = dgvPODetails1.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["SubGroup1Id"].Value.ToString();
                        String SubGroup2Id = dgvPODetails1.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["SubGroup2Id"].Value.ToString();
                        String ItemId = dgvPODetails1.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["ItemId"].Value.ToString();
                        String FullItemId = dgvPODetails1.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString();
                        String ItemName = dgvPODetails1.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["ItemName"].Value.ToString();
                        String InventSiteId = dgvPODetails1.Rows[i].Cells["InventSiteId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["InventSiteId"].Value.ToString();
                        decimal Qty = dgvPODetails1.Rows[i].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                        decimal RemainingQty = dgvPODetails1.Rows[i].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString()); //BY: HC | AMEND REMAINING QTY IKUT QTY BARU
                        //decimal RemainingQty = dgvPODetails1.Rows[i].Cells["RemainingQty"].Value == null ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["RemainingQty"].Value.ToString()); //REMARKED BY: HC 
                        String Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value == "" ? "" : dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                        decimal Ratio = dgvPODetails1.Rows[i].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Ratio"].Value.ToString());
                        decimal Price = dgvPODetails1.Rows[i].Cells["Price"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Price"].Value.ToString());
                        decimal Total = dgvPODetails1.Rows[i].Cells["Total"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Total"].Value.ToString());
                        decimal Diskon = dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value.ToString());
                        decimal TotalDisk = dgvPODetails1.Rows[i].Cells["TotalDisk"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalDisk"].Value.ToString());
                        decimal TotalPPN = dgvPODetails1.Rows[i].Cells["TotalPPN"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalPPN"].Value.ToString());
                        decimal TotalPPh = dgvPODetails1.Rows[i].Cells["TotalPPh"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["TotalPPh"].Value.ToString());
                        String Deskripsi = dgvPODetails1.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["Deskripsi"].Value.ToString().Trim();
                        //String AvailableDate = dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == null ? "" : FormateDateddmmyyyy(dgvPODetails1.Rows[i].Cells["AvailableDate"].Value.ToString());
                        DateTime AvailableDate = dgvPODetails1.Rows[i].Cells["AvailableDate"].Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dgvPODetails1.Rows[i].Cells["AvailableDate"].Value);
                        String DiscScheme = dgvPODetails1.Rows[i].Cells["DiscScheme"].Value.ToString() == "Select" ? "Amount" : dgvPODetails1.Rows[i].Cells["DiscScheme"].Value.ToString();
                        String BonusScheme = dgvPODetails1.Rows[i].Cells["BonusScheme"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["BonusScheme"].Value.ToString();
                        String CashBackScheme = dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["CashBackScheme"].Value.ToString();
                        String DeliveryMethod = dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString();
                        String ReffSeqNo = dgvPODetails1.Rows[i].Cells["ReffSeqNo"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["ReffSeqNo"].Value.ToString();
                        decimal SeqNoGroup = dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value == null ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString());
                        decimal Qty_KG = 0;
                        decimal Price_KG = 0;

                        if (Unit == "BTG")
                        {
                            if (Ratio > 0)
                            {
                                Qty_KG = Qty / Ratio;
                                Price_KG = Price / Ratio;
                            }
                        }
                        else
                        {
                            Qty_KG = Qty;
                            Price_KG = Price;
                        }

                        Query = "Insert into [dbo].[PurchDtl] (PurchId, OrderDate, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, InventSiteId, Qty, Qty_KG, RemainingQty, Unit, Konv_Ratio, Price, Price_KG, Total, Diskon, Total_Disk, Total_PPN, Total_PPh, Deskripsi, AvailableDate, DiscScheme, BonusScheme, CashBackScheme, DeliveryMethod, ReffTableName, ReffId, Reffid2, ReffSeqNo, ReffBaseSeqNo, CreatedDate, CreatedBy) ";
                        Query += "values ('" + PurchOrderId + "',  '" + OrderDate + "', '" + SeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "','" + ItemName + "','" + InventSiteId + "','" + Qty + "', '" + Qty_KG + "', '" + Qty + "', '" + Unit + "','" + Ratio + "', '" + Price + "', '" + Price_KG + "', '" + Total + "', '" + Diskon + "', '" + TotalDisk + "', '" + TotalPPN + "', '" + TotalPPh + "', @Deskripsi, '" + AvailableDate + "','" + DiscScheme + "', '" + BonusScheme + "','" + CashBackScheme + "', '" + DeliveryMethod + "' , '" + cmbReffTableName.Text + "' , '" + txtReffID.Text + "' ,'" + txtReffID2.Text + "' ,'" + ReffSeqNo + "', '" + SeqNoGroup + "',getdate(), '" + ControlMgr.UserId + "');";

                        //ListMethod1.POIssued(Conn, FullItemId, Qty, Unit);
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.AddWithValue("@Deskripsi", Deskripsi);
                        Cmd.ExecuteNonQuery();

                        //Update InventPurchase
                        Query = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        string UoM = Cmd.ExecuteScalar().ToString();

                        if (Unit == UoM)
                        {
                            Query = "Update Invent_Purchase_Qty Set ";
                            Query += "[PO_Issued_Outstanding_UoM] = [PO_Issued_Outstanding_UoM] + " + Qty + ",";
                            Query += "[PO_Issued_Outstanding_Alt] = [PO_Issued_Outstanding_Alt] + " + Qty * Ratio + " ";
                            Query += "Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            Query = "Update Invent_Purchase_Qty Set ";
                            Query += "[PO_Issued_Outstanding_UoM] = [PO_Issued_Outstanding_UoM] + " + Qty / Ratio + ",";
                            Query += "[PO_Issued_Outstanding_Alt] = [PO_Issued_Outstanding_Alt] + " + Qty + " ";
                            Query += "Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }

                        SavePurchAmountTable(PurchOrderId);
                    }
                }
                InsertAttachment(Conn, PurchOrderId);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
                return;
            }
            finally { }
            Conn.Close();
            MessageBox.Show("Data :" + PurchOrderId + " Berhasil ditambahkan.");
            if (Mode == "Amend")
            {
                Parent.RefreshGrid();
            }
            ModeBeforeEdit();
            txtPOId.Text = PurchOrderId;

            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void deleteOldInventPurchaseOty()
        {
            string FullItemId = "";
            string UnitOld = "";
            decimal QtyOld = 0;
            decimal ConvRatio = 0;
            string UoM = "";

            Query = "SELECT COUNT (PurchID) FROM [ISBS-NEW4].[dbo].[PurchDtl] WHERE PurchID = '" + PONumber + "'";
            Cmd = new SqlCommand(Query, Conn);
            int JumlahPODetailLama = (int)Cmd.ExecuteScalar();

            for (int i = 1; i <= JumlahPODetailLama; i++)
            {
                Query = "Select [FullItemId],[Unit],[Qty] From [PurchDtl] Where [SeqNo] = '" + i + "' and PurchID = '" + PONumber + "';";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    FullItemId = Dr["FullItemId"].ToString();
                    UnitOld = Dr["Unit"].ToString();
                    QtyOld = Convert.ToDecimal(Dr["Qty"].ToString());
                }
                Dr.Close();

                Query = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "';";
                Cmd = new SqlCommand(Query, Conn);
                ConvRatio = (decimal)Cmd.ExecuteScalar();

                Query = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "';";
                Cmd = new SqlCommand(Query, Conn);
                UoM = Cmd.ExecuteScalar().ToString();

                decimal QtyUoMOld = 0;
                decimal QtyAltOld = 0;
                if (UnitOld == UoM)
                {
                    QtyUoMOld = QtyOld;
                    QtyAltOld = QtyOld * ConvRatio;
                }
                else
                {
                    QtyAltOld = QtyOld;
                    QtyUoMOld = QtyOld / ConvRatio;
                }

                //Update InventPurchase
                Query = "Update Invent_Purchase_Qty Set ";
                Query += "[PO_Issued_Outstanding_UoM] = [PO_Issued_Outstanding_UoM] - " + QtyUoMOld + ",";
                Query += "[PO_Issued_Outstanding_Alt] = [PO_Issued_Outstanding_Alt] - " + QtyAltOld + " ";
                Query += "Where FullItemID = '" + FullItemId + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }

        }

        private string compareRemaining(int seqNoGroup, decimal totalQty, decimal totalAmount, string row)
        {
            string msg = "";
            decimal remainingQty = 0;
            decimal remainingAmount = 0;
            //AMBIL REMAINING VALUE (QTY/AMOUNT) PA DI DATABASE
            Query = "Select RemainingQty,ISNULL(RemainingAmount,0) AS RemainingAmount from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and Base = 'Y' and SeqNoGroup = '" + seqNoGroup + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                remainingQty = Convert.ToDecimal(Dr["RemainingQty"]);
                remainingAmount = Convert.ToDecimal(Dr["RemainingAmount"]);
            }
            Dr.Close();

            //CEK KALAU TOTAL MELEBIHI REMAINING
            if (Mode == "Edit")
            {
                decimal qtyPO = 0;
                decimal amountPO = 0;
                if (Mode == "Edit")
                {
                    //SUM VALUE QTY & AMOUNT DI PO
                    Query = "select sum(Qty) as Qty, sum(Price) as Price from PurchDtl where PurchID = '" + txtPOId.Text + "' and ReffBaseSeqNo = '" + seqNoGroup + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        qtyPO = Convert.ToDecimal(Dr["Qty"]);
                        amountPO = Convert.ToDecimal(Dr["Price"]) * qtyPO;
                    }
                    Dr.Close();
                }
                if (txtTransType.Text.ToUpper() == "QTY" && totalQty > remainingQty + qtyPO)
                {
                    msg = "Total qty untuk baris " + row + " tidak boleh melebihi " + (remainingQty + qtyPO).ToString("N2") + "!\r\n";
                }
                else if (txtTransType.Text.ToUpper() == "AMOUNT" && totalAmount > remainingAmount + amountPO)
                {
                    msg = "Total subtotal untuk baris " + row + " tidak boleh melebihi " + (remainingAmount + amountPO).ToString("N2") + "!\r\n";
                }
            }
            else
            {
                if (txtTransType.Text.ToUpper() == "QTY" && totalQty > remainingQty)
                {
                    msg = "Total qty untuk baris " + row + " tidak boleh melebihi " + remainingQty.ToString("N2") + "!\r\n";
                }
                else if (txtTransType.Text.ToUpper() == "AMOUNT" && totalAmount > remainingAmount)
                {
                    msg = "Total subtotal untuk baris " + row + " tidak boleh melebihi " + remainingAmount.ToString("N2") + "!\r\n";
                }
            }
            return msg;
        }

        String PurchOrderId;
        string CreatedBy;
        DateTime CreatedDate;

        private Boolean validasiSave()
        {
            bool vBol = true;

            if (txtReffID.Text == "")
            {
                MessageBox.Show("Canvas ID/PA ID/PO ID harus diisi.");
                vBol = false;
                return vBol;
            }

            if (dgvPODetails1.Rows.Count < 1)
            {
                MessageBox.Show("Item harus diisi.");
                vBol = false;
                return vBol;
            }

            PurchOrderId = txtPOId.Text;
            CreatedBy = "";
            CreatedDate = DateTime.Now;
            //Decimal QtyOld = 0;

            for (int i = 0; i <= dgvPODetails1.RowCount - 1; i++)
            {
                if (dgvPODetails1.Rows[i].Cells["Base"].Value.ToString() == "Y")
                {
                    Decimal Qty = 0;
                    string Unit = Convert.ToString(dgvPODetails1.Rows[i].Cells["Unit"].Value);

                    if (dgvPODetails1.Rows[i].Cells["Qty"].Value != null && dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString() != "")
                        Qty = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                    if (Qty <= 0)
                    {
                        MessageBox.Show("Item No = " + dgvPODetails1.Rows[i].Cells["No"].Value + ", Qty tidak boleh 0");
                        vBol = false;
                        return vBol;
                    }
                    else if (Unit == "")
                    {
                        MessageBox.Show("Item No = " + dgvPODetails1.Rows[i].Cells["Unit"].Value + ", Unit tidak boleh kosong");
                        vBol = false;
                        return vBol;
                    }

                }
            }

            //BY: HC (S)
            //CEK QTY / AMOUNT TIDAK BOLEH MELEBIHI REMANING QTY / AMOUNT PA
            if (cmbReffTableName.Text == "Purchase Agreement")
            {
                string msg = "";
                decimal totalQty = 0;
                decimal totalAmount = 0;
                string row = "";
                string vc = "";

                int tempSeqNoGroup = 0;
                if (Mode == "New" || Mode == "Generate" )
                {

                    #region New PO from PA
                    for (int k = 0; k < TampungIdItem.Count(); k++)
                    {
                        HitungAmount[k] = 0;
                    }

                    for (int i = 0; i < dgvPODetails1.RowCount; i++)
                    {
                        for (int k = 0; k < TampungIdItem.Count(); k++)
                        {
                            vc = dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() + dgvPODetails1.Rows[i].Cells["BaseItemId"].Value.ToString();
                            if (TampungIdItem[k] == vc)
                            {
                                if (txtTransType.Text.ToUpper() == "AMOUNT")
                                {
                                    HitungAmount[k] += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                                }
                                else
                                {
                                    HitungAmount[k] += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                }

                            }
                        }
                    }
                    for (int i = 0; i < dgvPODetails1.RowCount; i++)
                    {
                        for (int k = 0; k < TampungIdItem.Count(); k++)
                        {
                            vc = dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() + dgvPODetails1.Rows[i].Cells["BaseItemId"].Value.ToString();

                            if (TampungIdItem[k] == vc && Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["RemainingAmount"].Value) < HitungAmount[k])
                            {

                                if (txtTransType.Text.ToUpper() == "AMOUNT")
                                {
                                    msg = "Total Amount Order tidak boleh lebih besar dari Amount PA.";
                                }
                                else
                                {
                                    msg = "Total Quantity Order tidak boleh lebih besar dari Quantity PA.";
                                }
                            }
                        }


                    }
                    #endregion
                }
                else if (Mode == "Edit")
                {

                    #region Edit PO from PA
                    for (int k = 0; k < TampungIdItem.Count(); k++)
                    {
                        HitungAmount[k] = 0;
                    }

                    for (int i = 0; i < dgvPODetails1.RowCount; i++)
                    {
                        for (int k = 0; k < TampungIdItem.Count(); k++)
                        {
                            vc = dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() + dgvPODetails1.Rows[i].Cells["BaseItemId"].Value.ToString();
                            if (TampungIdItem[k] == vc)
                            {
                                if (txtTransType.Text.ToUpper() == "AMOUNT")
                                {
                                    HitungAmount[k] += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                                }
                                else
                                {
                                    HitungAmount[k] += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                }

                            }
                        }
                    }
                    for (int i = 0; i < dgvPODetails1.RowCount; i++)
                    {
                        for (int k = 0; k < TampungIdItem.Count(); k++)
                        {
                            vc = dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() + dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() + dgvPODetails1.Rows[i].Cells["BaseItemId"].Value.ToString();

                            if (TampungIdItem[k] == vc && Convert.ToDecimal(dgvPODetails1.Rows[i].Cells[30].Value) < HitungAmount[k])
                            {

                                if (txtTransType.Text.ToUpper() == "AMOUNT")
                                {
                                    msg = "Total Amount Order tidak boleh lebih besar dari Amount PA.";
                                }
                                else
                                {
                                    msg = "Total Quantity Order tidak boleh lebih besar dari Quantity PA.";
                                }
                            }
                        }


                    }
                    #endregion
                }


                if (msg != "")
                {
                    MessageBox.Show(msg);
                    vBol = false;
                    return vBol;
                }
            }
            //BY: HC (E)
            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (TransactionScope Scope = new TransactionScope())
            {
                Conn = ConnectionString.GetConnection();

                if (!validasiSave())
                    return;

                //UpdateCredit limit at vend table
                if (CreditLimit(Conn) == true)
                {
                    DialogResult dialogResult = MessageBox.Show("Credit untuk vendor ini sudah melebihi batas Credit Limit, apakah tetap ingin melanjutkan transaksi? ", "Update Status Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        Conn.Close();
                        return;
                    }
                }
                //================================
                if (Mode == "New" || Mode == "Generate")
                {
                    SaveNew();
                }
                else if (Mode == "Edit")
                {
                    SaveEdit();
                }
                else if (Mode == "Amend")
                {
                    SaveAmend();
                }
                Scope.Complete();
            }
            GetDataHeader();
        }

        //created by Thaddaeus
        private bool CreditLimit(SqlConnection Conn)
        {
            decimal TotalNett = Convert.ToDecimal(txtTotalNett.Text);
            decimal OldTotal = 0;
            string OldVendorId = "";
            bool status = false;
            Query = "SELECT [Total_Nett],[VendID] FROM [dbo].[PurchH] WHERE [PurchID] = '" + txtPOId.Text + "'";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        OldTotal = Convert.ToDecimal(Dr["Total_Nett"]);
                        OldVendorId = Dr["VendID"].ToString();
                    }
                }
                Dr.Close();
            }
            if (OldTotal != 0)
            {
                Query = "UPDATE [dbo].[VendTable] SET [Sisa_Limit_Total] += " + OldTotal + " WHERE [VendId] = '" + OldVendorId + "'";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.ExecuteNonQuery();
                }
            }
            Query = "UPDATE [dbo].[VendTable] SET [Sisa_Limit_Total] -= " + TotalNett + " WHERE [VendId] = '" + txtVendId.Text + "'";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.ExecuteNonQuery();
            }

            Query = "SELECT [Limit_Total], Sisa_Limit_Total,[Limit_Per_PO] FROM [dbo].[VendTable] WHERE [VendId] = '" + txtVendId.Text + "'";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    if (Convert.ToDecimal(Dr["Sisa_Limit_Total"]) < 0)
                    {
                        status = true;
                    }
                    if (Convert.ToDecimal(Dr["Limit_Per_PO"]) < Convert.ToDecimal(txtTotalNett.Text))
                    {
                        status = true;
                    }
                }
                Dr.Close();
            }
            return status;
        }
        //end========================================================

        private void SavePurchAmountTable(string TmpPurchId)
        {
            Query = "SELECT [PurchId] FROM [dbo].[PurchAmountTable] where PurchID = '" + TmpPurchId + "'";
            Cmd = new SqlCommand(Query, Conn);
            if (Cmd.ExecuteScalar() == null)
            {
                Query = "INSERT INTO [dbo].[PurchAmountTable] ([PurchId],[PurchDate],[TermOfPayment],[CurrencyCode],[ExchRate],[PurchAmount]";
                Query += ",[CreatedDate],[CreatedBy])";
                Query += "Values (";
                Query += "'" + TmpPurchId + "',";
                Query += "'" + dtPODate.Value.ToString("yyyy-MM-dd") + "',";
                //Query += "'" + txtToP.Text + "',";
                Query += "'" + cmbTermOfPayment.Text + "',";
                Query += "'" + cmbCurrency.Text + "','";
                Query += txtExchRate.Text == "" ? "0.00','" : Decimal.Parse(txtExchRate.Text.ToString()) + "','";
                Query += txtTotalNett.Text == "" ? "0.00'," : Decimal.Parse(txtTotalNett.Text.ToString()) + "',";
                Query += "getdate(),";
                Query += "'" + ControlMgr.UserId + "')";
            }
            else
            {
                Query = "Update [dbo].[PurchAmountTable] Set ";
                Query += "[PurchDate]='" + dtPODate.Value.ToString("yyyy-MM-dd") + "',";
                //Query += "[TermOfPayment]='" + txtToP.Text + "',";
                Query += "[TermOfPayment]='" + cmbTermOfPayment.Text + "',";
                Query += "[CurrencyCode]='" + cmbCurrency.Text + "',";
                Query += "[ExchRate]='";// +txtExchRate.Text == "" ? "0.00','" : Decimal.Parse(txtExchRate.Text.ToString()) + "',";
                Query += txtExchRate.Text == "" ? "0.00'," : Decimal.Parse(txtExchRate.Text.ToString()) + "',";
                Query += "[PurchAmount]='";// +txtTotalNett.Text == "" ? "0.00'," : Decimal.Parse(txtTotalNett.Text.ToString()) + "',";
                Query += txtTotalNett.Text == "" ? "0.00'," : Decimal.Parse(txtTotalNett.Text.ToString()) + "',";
                Query += "[CreatedDate]=getdate(),";
                Query += "[CreatedBy]='" + ControlMgr.UserId + "' where [PurchId]='" + TmpPurchId + "';";
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";
            ModeBeforeEdit();
            GetDataHeader();
        }

        private void dgvPODetails1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "InventSiteId")
            {
                InventSite.Location = dgvPODetails1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                InventSite.Visible = true;

                Conn = ConnectionString.GetConnection();

                Query = "SELECT [InventSiteId] FROM [dbo].[InventSite] ";

                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader DrCmb;
                DrCmb = Cmd.ExecuteReader();

                InventSite.Items.Clear();
                InventSite.Items.Add("");
                while (DrCmb.Read())
                {
                    InventSite.Items.Add(DrCmb[0].ToString());
                }
                //InventSite.SelectedIndex = 0;
                DrCmb.Close();

                Conn.Close();
            }


            if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "AvailableDate")
            {
                dtp.Location = dgvPODetails1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                dtp.Visible = true;

                if (dgvPODetails1.CurrentCell.Value != "" && dgvPODetails1.CurrentCell.Value != null)
                {
                    DateTime dDate;
                    if (!DateTime.TryParse(dgvPODetails1.CurrentCell.Value.ToString(), out dDate))
                    {
                        dtp.Value = Convert.ToDateTime(FormateDateddmmyyyy(dgvPODetails1.CurrentCell.Value.ToString()));
                    }
                    else
                    {
                        dtp.Value = Convert.ToDateTime(dgvPODetails1.CurrentCell.Value);
                    }
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
            else
            {
                dtp.Visible = false;
            }

            if (Mode != "BeforeEdit")
            {

                if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                {
                    DeliveryMethod.Location = dgvPODetails1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                    DeliveryMethod.Visible = true;
                    string tmpFullItemId = dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                    string tmpDeliveryMethod = "";
                    Conn = ConnectionString.GetConnection();
                    //for (int i = 0; i < dgvPODetails1.RowCount; i++)
                    //{
                    //    if (dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                    //    {
                    //        if (dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value != null)
                    //        {
                    //            if (tmpDeliveryMethod == "")
                    //            {
                    //                tmpDeliveryMethod = "'" + dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                    //            }
                    //            else
                    //            {
                    //                tmpDeliveryMethod += ",'" + dgvPODetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                    //            }
                    //        }
                    //    }
                    //}

                    Query = "SELECT [DeliveryMethod] FROM [dbo].[DeliveryMethod] ";

                    //if (tmpDeliveryMethod != "")
                    //    Query += "Where DeliveryMethod not in (" + tmpDeliveryMethod + ");";

                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader DrCmb;
                    DrCmb = Cmd.ExecuteReader();

                    DeliveryMethod.Items.Clear();
                    DeliveryMethod.Items.Add("");
                    while (DrCmb.Read())
                    {
                        DeliveryMethod.Items.Add(DrCmb[0].ToString());
                    }
                    DeliveryMethod.SelectedIndex = 0;
                    DrCmb.Close();

                    Conn.Close();
                }
            }
        }

        private void DeliveryMethod_DropDownClosed(object sender, EventArgs e)
        {
            DeliveryMethod.Visible = false;
        }

        private void DeliveryMethod_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvPODetails1.CurrentCell.Value = DeliveryMethod.Text.ToString();
            for (int j = 0; j < dgvPODetails1.RowCount; j++)
            {
                if (dgvPODetails1.Rows[j].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[dgvPODetails1.CurrentCell.RowIndex].Cells["No"].Value.ToString())
                {
                    dgvPODetails1.Rows[j].Cells["DeliveryMethod"].Value = dgvPODetails1.Rows[dgvPODetails1.CurrentCell.RowIndex].Cells["DeliveryMethod"].Value.ToString();
                }
            }
        }

        private string FormateDateddmmyyyy(string tmpDate)
        {
            //string reformat="";
            if (tmpDate.Contains('-'))
            {
                string[] data = tmpDate.Split('-');
                return data[2] + "-" + data[1] + "-" + data[0];
            }
            else if (tmpDate.Contains('/'))
            {
                string[] data = tmpDate.Split('/');
                return data[2] + "-" + data[1] + "-" + data[0];
            }
            return tmpDate;
        }

        private void TotalNett()
        {
            txtTotal.Text = "";
            txtTotalPPN.Text = "";
            txtTotalPPH.Text = "";

            for (int i = 0; i < dgvPODetails1.RowCount; i++)
            {
                if (txtTotal.Text == "")
                    txtTotal.Text = "0";
                if (txtTotalDisk.Text == "")
                    txtTotalDisk.Text = "0";
                if (txtTotalNett.Text == "")
                    txtTotalNett.Text = "0";
                if (txtTotalPPN.Text == "")
                    txtTotalPPN.Text = "0";
                if (txtTotalPPH.Text == "")
                    txtTotalPPH.Text = "0";

                decimal PPh = 0;
                decimal PPn = 0;
                decimal Qty = 0;
                decimal TotalGrid = 0;
                decimal Diskon = 0;
                decimal TotalDisk = 0;
                decimal Price = 0;

                if (cmbPPn.Text == "" || cmbPPn.SelectedIndex == 0)
                {
                    PPn = 0;
                }
                else
                {
                    PPn = Convert.ToDecimal(cmbPPn.Text);
                }
                if (cmbPPh.Text == "" || cmbPPh.SelectedIndex == 0)
                {
                    PPh = 0;
                }
                else
                {
                    PPh = Convert.ToDecimal(cmbPPh.Text);
                }

                Qty = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value == "" || dgvPODetails1.Rows[i].Cells["Qty"].Value == null ? "0" : dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                Price = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Price"].Value == "" || dgvPODetails1.Rows[i].Cells["Price"].Value == null ? "0" : dgvPODetails1.Rows[i].Cells["Price"].Value.ToString());
                TotalGrid = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Total"].Value == "" || dgvPODetails1.Rows[i].Cells["Total"].Value == null ? "0" : dgvPODetails1.Rows[i].Cells["Total"].Value.ToString());
                Diskon = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value == "" || dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value == null ? "0" : dgvPODetails1.Rows[i].Cells["Diskon(%)"].Value.ToString());
                TotalDisk = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalDisk"].Value == "" || dgvPODetails1.Rows[i].Cells["TotalDisk"].Value == null ? "0" : dgvPODetails1.Rows[i].Cells["TotalDisk"].Value.ToString());
                //edite by Thaddaeus, 30 August 2018
                if (dgvPODetails1.Rows[i].Cells["Unit"].Value == null)
                {
                    dgvPODetails1.Rows[i].Cells["Unit"].Value = "";
                }

                dgvPODetails1.Rows[i].Cells["Total"].Value = Qty * Price;
                if (dgvPODetails1.Rows[i].Cells["DiscScheme"].Value.ToString() == "Percentage")
                    dgvPODetails1.Rows[i].Cells["TotalDisk"].Value = (Diskon / 100) * TotalGrid;
                else
                    dgvPODetails1.Rows[i].Cells["TotalDisk"].Value = TotalDisk;
                dgvPODetails1.Rows[i].Cells["TotalPPN"].Value = ((Qty * Price) - TotalDisk) * PPn / 100;
                dgvPODetails1.Rows[i].Cells["TotalPPH"].Value = ((Qty * Price) - TotalDisk) * PPh / 100;

                decimal TotalToNett = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Total"].Value);
                decimal TotalDiskToNett = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalDisk"].Value);
                decimal TotalPPNToNett = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalPPN"].Value);
                decimal TotalPPHToNett = Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalPPH"].Value);
                dgvPODetails1.Rows[i].Cells["TotalNett"].Value = TotalToNett - TotalDiskToNett + TotalPPNToNett + TotalPPHToNett;
            }
        }

        private void TotalNett2()
        {
            decimal total = 0;
            decimal totalppn = 0;
            decimal totalpph = 0;
            decimal totaldisk = 0;

            for (int i = 0; i < dgvPODetails1.Rows.Count; ++i)
            {
                total += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Total"].Value);
                totaldisk += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalDisk"].Value);
                totalpph += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalPPH"].Value);
                totalppn += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalPPN"].Value);
            }
            txtTotal.Text = total.ToString("N2");
            txtTotalDisk.Text = totaldisk.ToString("N2");
            txtTotalPPH.Text = totalpph.ToString("N2");
            txtTotalPPN.Text = totalppn.ToString("N2");
            txtTotalNett.Text = ((decimal.Parse(txtTotal.Text) - decimal.Parse(txtTotalDisk.Text)) + decimal.Parse(txtTotalPPH.Text) + decimal.Parse(txtTotalPPN.Text)).ToString("N2");
        }

        private void dgvPODetails1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (Mode != "BeforeEdit")
            {
                TotalNett();
                TotalNett2();
            }
        }

        private string getOldUnit(string PAID, string SeqNo)
        {
            string result = "";
            try
            {
                Conn = ConnectionString.GetConnection();
                string Query2 = "SELECT Unit FROM [PurchAgreementDtl] WHERE [AgreementID] = '" + PAID + "' AND SeqNo = '" + SeqNo + "' ";
                SqlCommand Cmd2 = new SqlCommand(Query2, Conn);
                SqlDataReader Dr2 = Cmd2.ExecuteReader();
                while (Dr2.Read())
                {
                    result = Convert.ToString(Dr2["Unit"]);
                }
                Dr2.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                Conn.Close();
            }
            return result;
        }

        private void cmbPPn_TextChanged(object sender, EventArgs e)
        {
            TotalNett();
            TotalNett2();
        }

        private void cmbPPh_TextChanged(object sender, EventArgs e)
        {
            TotalNett();
            TotalNett2();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        //tia edit
        //klik kanan

        #region ParentRefreshGrid
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.Vendor.Vendor Vendor = null;

        Purchase.ReceiptOrder.HeaderReceiptOrder ParentToRO;
        Purchase.NotaReturBeli.ReturBeliHeader ParentToRBH;
        Purchase.NotaReturBeli.NRBApproval ParentToRBHA;
        AccountPayable.HeaderAccountsPayable ParentToAP;
        Master.Vendor.FrmM_Vendor ParentToMasterVendor;
        Purchase.PurchaseOrderNew.POForm ParentToPO;
        Purchase.PurchaseAgreement.PAForm ParentToPA;

        Purchase.CanvasSheet.FormCanvasSheet2 CS = null;
        Purchase.PurchaseAgreement.PAForm PA = null;
        Purchase.PurchaseOrderNew.POForm PO = null;
        Purchase.PurchaseOrderApproval.POHeaderApproval ParentToPAApproval;

        public void ParentRefreshGrid(Purchase.ReceiptOrder.HeaderReceiptOrder ro)
        {
            ParentToRO = ro;
        }

        public void ParentRefreshGrid2(Purchase.NotaReturBeli.ReturBeliHeader rbh)
        {
            ParentToRBH = rbh;
        }

        public void ParentRefreshGrid3(Purchase.NotaReturBeli.NRBApproval rbha)
        {
            ParentToRBHA = rbha;
        }
        public void ParentRefreshGrid4(AccountPayable.HeaderAccountsPayable ap)
        {
            ParentToAP = ap;
        }
        public void ParentRefreshGrid5(Master.Vendor.FrmM_Vendor mv)
        {
            ParentToMasterVendor = mv;
        }

        public void ParentRefreshGrid6(Purchase.PurchaseOrderNew.POForm Po)
        {
            ParentToPO = Po;
        }
        public void ParentRefreshGrid7(Purchase.PurchaseAgreement.PAForm pa)
        {
            ParentToPA = pa;
        }
        public void ParentRefreshGrid8(Purchase.PurchaseOrderApproval.POHeaderApproval Poa)
        {
            ParentToPAApproval = Poa;
        }
        #endregion

        private void txtReffID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (cmbReffTableName.Text == "Canvass Sheet")
                {
                    if (CS == null || CS.Text == "")
                    {
                        CS = new Purchase.CanvasSheet.FormCanvasSheet2();
                        CS.ModePopUp(txtReffID.Text);
                        CS.ParentRefreshGrid2(this);
                        CS.Show();

                    }
                    else if (CheckOpened(CS.Name))
                    {
                        CS.WindowState = FormWindowState.Normal;
                        CS.Show();
                        CS.Focus();
                    }
                }
                else if (cmbReffTableName.Text == "Purchase Agreement")
                {

                    if (PA == null || PA.Text == "")
                    {
                        PA = new Purchase.PurchaseAgreement.PAForm();
                        PA.SetMode("View", "", txtReffID.Text);
                        // Pa.GetDataHeader();
                        PA.Show();

                        //}
                    }
                    else if (CheckOpened(PA.Name))
                    {
                        PA.WindowState = FormWindowState.Normal;
                        PA.Show();
                        PA.Focus();
                    }
                }


                else if (cmbReffTableName.Text == "Purchase Order")
                {

                    if (PO == null || PO.Text == "")
                    {
                        PO = new Purchase.PurchaseOrderNew.POForm();
                        PO.SetMode("PopUp", txtReffID.Text, "");
                        PO.ParentRefreshGrid6(this);
                        PO.Show();
                    }
                    else if (CheckOpened(PO.Name))
                    {
                        PO.WindowState = FormWindowState.Normal;
                        PO.Show();
                        PO.Focus();
                    }
                }


            }
        }

        private void dgvPODetails1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {
                        PopUpItemName.Close();
                        // PopUpItemName = new PopUp.Stock.Stock();
                        //PopUpItemName = new PopUp.FullItemId.FullItemId();
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPODetails1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvPODetails1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvPODetails1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
                //cs, pa, po
                if (cmbReffTableName.Text == "Canvass Sheet")
                {
                    if (CS == null || CS.Text == "")
                    {
                        if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "ReffId")
                        {
                            CS = new Purchase.CanvasSheet.FormCanvasSheet2();
                            CS.ModePopUp(txtReffID.Text);
                            CS.ParentRefreshGrid2(this);
                            CS.Show();
                        }
                    }
                    else if (CheckOpened(CS.Name))
                    {
                        CS.WindowState = FormWindowState.Normal;
                        CS.Show();
                        CS.Focus();
                    }
                }
                else if (cmbReffTableName.Text == "Purchase Agreement")
                {

                    if (PA == null || PA.Text == "")
                    {
                        if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "ReffId")
                        {
                            PA = new Purchase.PurchaseAgreement.PAForm();
                            PA.SetMode("View", "", txtReffID.Text);
                            // Pa.GetDataHeader();
                            PA.Show();

                        }
                    }
                    else if (CheckOpened(PA.Name))
                    {
                        PA.WindowState = FormWindowState.Normal;
                        PA.Show();
                        PA.Focus();
                    }
                }
                else if (cmbReffTableName.Text == "Purchase Order")
                {

                    if (PO == null || PO.Text == "")
                    {
                        if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "ReffId")
                        {
                            PO = new Purchase.PurchaseOrderNew.POForm();
                            PO.SetMode("PopUp", txtReffID.Text, "");
                            PO.ParentRefreshGrid6(this);
                            PO.Show();
                        }
                    }
                    else if (CheckOpened(PO.Name))
                    {
                        PO.WindowState = FormWindowState.Normal;
                        PO.Show();
                        PO.Focus();
                    }
                }

            }
        }

        private void txtVendId_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {

                    //if (txtVendId.Bounds.Contains(e.Location))
                    //{
                    txtVendId.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendId.Text);

                    Vendor.Show();
                    //}
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void txtVendName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    txtVendName.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendId.Text);
                    Vendor.Show();

                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
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

        public void HidePrice()
        {
            if (ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI" || ControlMgr.GroupName == "SITE MANAGER")
            {
                txtTotal.Visible = false;
                txtTotalDisk.Visible = false;
                txtTotalPPN.Visible = false;
                txtTotalPPH.Visible = false;
                txtTotalNett.Visible = false;
                cmbCurrency.Visible = false;
                txtExchRate.Visible = false;
                //txtDPPercent.Visible = false;
                //txtDPAmount.Visible = false;
                cmbPPn.Visible = false;
                cmbPPh.Visible = false;
                txtBonusScheme.Visible = false;
                txtCashBackScheme.Visible = false;
                dgvPODetails1.Columns["Price"].Visible = false;
                dgvPODetails1.Columns["Total"].Visible = false;
                dgvPODetails1.Columns["Diskon(%)"].Visible = false;
                dgvPODetails1.Columns["TotalDisk"].Visible = false;
                dgvPODetails1.Columns["TotalPPN"].Visible = false;
                dgvPODetails1.Columns["TotalPPh"].Visible = false;
                dgvPODetails1.Columns["TotalNett"].Visible = false;
                dgvPODetails1.Columns["DiscScheme"].Visible = false;
                dgvPODetails1.Columns["BonusScheme"].Visible = false;
                dgvPODetails1.Columns["CashBackScheme"].Visible = false;

            }
        }

        //tia edit end      

        public static string itemID;

        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvPODetails1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvPODetails1.Columns["Total"].Index ||
                e.ColumnIndex == dgvPODetails1.Columns["TotalDisk"].Index ||
                e.ColumnIndex == dgvPODetails1.Columns["TotalPPN"].Index ||
                e.ColumnIndex == dgvPODetails1.Columns["TotalPPh"].Index ||
                e.ColumnIndex == dgvPODetails1.Columns["Price"].Index ||
                e.ColumnIndex == dgvPODetails1.Columns["BonusScheme"].Index ||
                e.ColumnIndex == dgvPODetails1.Columns["CashBackScheme"].Index ||
                dgvPODetails1.Columns[e.ColumnIndex].Name == "RemainingAmount")
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                dgvPODetails1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N2");
            }

            if (dgvPODetails1.Columns[e.ColumnIndex].Name == "CreatedDate" || dgvPODetails1.Columns[e.ColumnIndex].Name == "UpdatedDate" || dgvPODetails1.Columns[e.ColumnIndex].Name.Contains("ValidTo"))
                dgvPODetails1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            else if (dgvPODetails1.Columns[e.ColumnIndex].Name.Contains("Date"))
                dgvPODetails1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (cmbReffTableName.Text == "Purchase Agreement")
            {
                if (Mode == "Edit")
                {
                    ListSeqNoDelete.Clear();
                }

                Purchase.PurchaseOrderNew.PA F = new Purchase.PurchaseOrderNew.PA();
                if (txtReffID.Text.Contains("PO"))
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select ReffID from PurchH where PurchID = '" + txtReffID.Text.Substring(0, 13) + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    F.flag(Cmd.ExecuteScalar().ToString(), "");
                    Conn.Close();
                }
                else
                    F.flag(txtReffID.Text, "");
                F.setParent(this);
                F.ShowDialog();
            }
            else
            {
                Purchase.PurchaseOrderNew.PA F = new Purchase.PurchaseOrderNew.PA();
                F.flag("", txtReffID.Text);
                F.setParent(this);
                F.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!amendDeleteCheck())
                return;

            if (dgvPODetails1.RowCount > 0)
            {
                Index = dgvPODetails1.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvPODetails1.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvPODetails1.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvPODetails1.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string SeqNo = dgvPODetails1.Rows[dgvPODetails1.CurrentCell.RowIndex].Cells["No"].Value.ToString();

                    if (Mode == "Edit")
                    {
                        ListSeqNoDelete.Add(SeqNo);
                    }

                    dgvPODetails1.Rows.RemoveAt(Index);

                    SortNoDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvPODetails1.RowCount; i++)
            {
                dgvPODetails1.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void ValidationRemainingQty()
        {
            string msg = null;
            Index = dgvPODetails1.CurrentRow.Index;

            if (dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString() == "" && dgvPODetails1.Columns[dgvPODetails1.CurrentCell.ColumnIndex].Name.ToString() != "Unit")
            {
                MessageBox.Show("Unit tidak boleh kosong.");
                dgvPODetails1.Rows[Index].Cells["Qty"].Value = 0;
                return;
            }

            if (cmbReffTableName.Text == "Purchase Agreement")
            {

                String SeqNo = dgvPODetails1.Rows[Index].Cells["SeqNo"].Value == null ? "" : dgvPODetails1.Rows[Index].Cells["SeqNo"].Value.ToString();
                String SeqNoGroup = dgvPODetails1.Rows[Index].Cells["SeqNoGroup"].Value == null ? "" : dgvPODetails1.Rows[Index].Cells["SeqNoGroup"].Value.ToString();
                String Unit = dgvPODetails1.Rows[Index].Cells["Unit"].Value == null ? "" : dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString();
                decimal Ratio = dgvPODetails1.Rows[Index].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[Index].Cells["Ratio"].Value.ToString());
                decimal Qty = 0;
                decimal Amount = 0;
                String tmpUnit = "";

                if (Mode == "New" || Mode == "Generate")
                {
                    if (txtTransType.Text.ToUpper() == "FIX")
                    {
                        Query = "Select Qty,RemainingQty,Unit,SeqNoGroup,ISNULL(RemainingAmount,0) AS RemainingAmount from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and SeqNo = '" + SeqNo + "'";
                    }
                    else
                    {
                        //if (Mode.ToUpper() != "EDIT")
                        //{
                        if (dgvPODetails1.Rows[Index].Cells["Base"].Value.ToString() == "N")
                        {
                            Query = "Select Qty,RemainingQty,Unit,SeqNoGroup,ISNULL(RemainingAmount,0) AS RemainingAmount from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and Base = 'Y' and SeqNoGroup = '" + SeqNoGroup + "'";
                        }
                        else
                        {
                            Query = "Select Qty,RemainingQty,Unit,SeqNoGroup,ISNULL(RemainingAmount,0) AS RemainingAmount from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and SeqNo = '" + SeqNo + "'";
                        }
                        //}
                        //else
                        //{
                        //    //  Query = "Select Qty,RemainingQty,Unit,SeqNoGroup from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "'";
                        //    Query = "Select Qty,RemainingQty,Unit,SeqNoGroup from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and SeqNo = '" + SeqNo + "'";
                        //}
                    }
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            Qty = Convert.ToDecimal(string.IsNullOrEmpty(Dr["RemainingQty"].ToString()) ? "0" : Dr["RemainingQty"]);
                            Amount = Convert.ToDecimal(string.IsNullOrEmpty(Dr["RemainingAmount"].ToString()) ? "0" : Dr["RemainingAmount"]);
                            if ((Dr["Unit"].ToString().ToUpper() == "KG" || Dr["Unit"].ToString().ToUpper() == "LBR") && dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString().ToUpper() == "BTG")
                            {
                                Qty = Qty / Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value.ToString());
                            }
                            else if (Dr["Unit"].ToString().ToUpper() == "BTG" && (dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString().ToUpper() == "KG" || dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString().ToUpper() == "LBR"))
                            {
                                Qty = Qty * Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value.ToString());
                            }
                            if (Mode == "Edit")
                            {
                                Qty = Qty + GetOldQty(Dr["Unit"].ToString());
                            }

                            tmpUnit = Dr["Unit"].ToString();
                        }
                    }
                    Dr.Close();

                    decimal TmpQty = 0;
                    if (txtTransType.Text.ToUpper() == "FIX" && dgvPODetails1.Rows[Index].Cells["Base"].Value.ToString() == "Y")
                    {
                        for (int i = 0; i < dgvPODetails1.RowCount; i++)
                        {
                            if (SeqNo == dgvPODetails1.Rows[i].Cells["SeqNo"].Value.ToString())
                            {
                                String Unit1 = dgvPODetails1.Rows[i].Cells["Unit"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();

                                if (Unit1 == "KG" || Unit1 == "LBR")
                                {
                                    TmpQty += (Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) / Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value));
                                }
                                else
                                {
                                    TmpQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                }
                            }
                        }
                        if (tmpUnit == Unit)
                        {
                            if (Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Qty"].Value) > Qty)
                            {
                                msg = "Qty tidak boleh lebih besar dari Qty Agreement.\n";
                                dgvPODetails1.Rows[Index].Cells["Qty"].Value = 0;
                            }
                            if (tmpUnit == "BTG")
                            {
                                if (TmpQty > Qty)
                                {
                                    msg += "Total Qty tidak boleh lebih besar dari Qty Agreement.\n";
                                    dgvPODetails1.Rows[Index].Cells["Qty"].Value = 0;
                                }
                            }
                            else
                            {
                                if (Math.Floor(TmpQty * Ratio) > (Qty))
                                {
                                    msg += "Total Qty tidak boleh lebih besar dari Qty Agreement.\n";
                                    dgvPODetails1.Rows[Index].Cells["Qty"].Value = 0;
                                }
                            }
                        }
                        else if (tmpUnit == "BTG" && (Unit == "KG" || Unit == "LBR"))
                        {
                            if ((Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Qty"].Value) / Ratio) > Qty)
                            {
                                msg = "Qty tidak boleh lebih besar dari Qty Agreement.\n";
                                dgvPODetails1.Rows[Index].Cells["Qty"].Value = 0;
                            }

                            if (TmpQty > Qty)
                            {
                                msg += "Total Qty tidak boleh lebih besar dari Qty Agreement.\n";
                                dgvPODetails1.Rows[Index].Cells["Qty"].Value = 0;
                            }
                        }
                        else if ((tmpUnit == "KG" || tmpUnit == "LBR") && Unit == "BTG")
                        {
                            if ((Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Qty"].Value) * Ratio) > Qty)
                            {
                                msg = "Qty tidak boleh lebih besar dari Qty Agreement.\n";
                                dgvPODetails1.Rows[Index].Cells["Qty"].Value = 0;
                            }

                            if (TmpQty * Ratio > Qty)
                            {
                                msg += "Total Qty tidak boleh lebih besar dari Qty Agreement.\n";
                                dgvPODetails1.Rows[Index].Cells["Qty"].Value = 0;
                            }
                        }
                    }
                    else
                    {
                        TmpQty = 0;
                        decimal TmpAmount = 0;
                        //String tmpUnit = "";
                        decimal tmpSeqNoGroup = 0;
                        decimal tmpSeqNo = 0;
                        decimal tmpQty = 0;
                        decimal tmpRemainingQty = 0;
                        decimal tmpRemainingAmount = 0;

                        Conn = ConnectionString.GetConnection();

                        Query = "Select Unit,SeqNoGroup,SeqNo,RemainingQty,ISNULL(RemainingAmount,0) AS RemainingAmount from PurchAgreementDtl where  AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + SeqNoGroup + "' and Base='Y'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            tmpUnit = Dr["Unit"].ToString();
                            tmpSeqNoGroup = decimal.Parse(Dr["SeqNoGroup"].ToString());
                            tmpSeqNo = decimal.Parse(Dr["SeqNo"].ToString());
                            tmpRemainingQty = decimal.Parse(Dr["RemainingQty"].ToString());
                            tmpRemainingAmount = decimal.Parse(Dr["RemainingAmount"].ToString());
                        }
                        Dr.Close();
                        if (txtPOId.Text != "")
                        {
                            Query = "Select Unit,ReffBaseSeqNo,Qty,Konv_Ratio from PurchDtl where  [PurchID] = '" + txtPOId.Text + "' and ReffSeqNo = '" + dgvPODetails1.Rows[Index].Cells["SeqNoGroup"].Value.ToString() + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();

                            while (Dr.Read())
                            {
                                if (tmpUnit == Dr["Unit"].ToString())
                                    tmpRemainingQty += decimal.Parse(Dr["Qty"].ToString());
                                else
                                    if (Dr["Unit"].ToString() == "KG")
                                        tmpRemainingQty += decimal.Parse(Dr["Qty"].ToString()) * decimal.Parse(Dr["Konv_Ratio"].ToString());
                                    else
                                        tmpRemainingQty += decimal.Parse(Dr["Qty"].ToString()) / decimal.Parse(Dr["Konv_Ratio"].ToString());
                            }
                            Dr.Close();
                        }


                        for (int i = 0; i < dgvPODetails1.RowCount; i++)
                        {
                            if (SeqNoGroup == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() && dgvPODetails1.Rows[i].Cells["Unit"].Value != null)
                            {
                                String Unit1 = dgvPODetails1.Rows[i].Cells["Unit"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();

                                if (tmpUnit == Unit1)
                                {
                                    TmpQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                    TmpAmount += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                                }
                                else
                                {
                                    if (Unit1 == "KG")
                                    {
                                        TmpQty += (Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) / Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value));
                                        //TmpQty = TmpQty * Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value);
                                        TmpAmount += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                                    }
                                    else
                                    {
                                        TmpQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) * Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value);
                                        //TmpQty = TmpQty / Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value);
                                        TmpAmount += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                                    }
                                }
                                //// if (tmpUnit == dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString())
                                //// {
                                //TmpQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                //// }
                                ////else
                                ////{
                                ////    if (tmpUnit == dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString())
                                ////        TmpQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                ////    else if (tmpUnit != "KG" && dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString() != tmpUnit)
                                ////        TmpQty += (Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) / Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value));
                                ////    else if (tmpUnit == "KG" && dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString() != tmpUnit)
                                ////        TmpQty += (Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) * Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value));
                                ////}
                            }
                        }

                        if (txtTransType.Text.ToUpper() == "QTY")
                        {
                            if (TmpQty > Qty)
                            {
                                //REMARKED BY: HC (S)
                                //decimal TmpQty1 = 0;
                                //for (int i = 0; i < dgvPODetails1.RowCount; i++)
                                //{
                                //    if (dgvPODetails1.Rows[Index].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() && dgvPODetails1.Rows[Index].Cells["FullItemId"].Value.ToString() != dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString())
                                //    {
                                //        if (tmpUnit == dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString())
                                //            TmpQty1 += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                //        else
                                //        {
                                //            if (tmpUnit == "KG")
                                //                TmpQty1 += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) * Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value);
                                //            else
                                //                TmpQty1 += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) / Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value);
                                //        }
                                //    }
                                //}
                                //if (tmpUnit == dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString())
                                //    dgvPODetails1.Rows[Index].Cells["Qty"].Value = (tmpRemainingQty - TmpQty1);
                                //else
                                //{
                                //    if (dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString() == "KG")
                                //        dgvPODetails1.Rows[Index].Cells["Qty"].Value = ((tmpRemainingQty - TmpQty1) * Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value));
                                //    else
                                //        dgvPODetails1.Rows[Index].Cells["Qty"].Value = ((tmpRemainingQty - TmpQty1) / Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value));
                                //}


                                ////dgvPODetails1.Rows[Index].Cells["Qty"].Value = Qty;

                                ////if (tmpUnit == Unit )
                                ////    dgvPODetails1.Rows[Index].Cells["Qty"].Value = Qty - (TmpQty - Decimal.Parse(dgvPODetails1.Rows[Index].Cells["Qty"].Value.ToString()));
                                ////else if (tmpUnit != Unit && tmpUnit!="KG")
                                ////    dgvPODetails1.Rows[Index].Cells["Qty"].Value = (Qty - (TmpQty - (Decimal.Parse(dgvPODetails1.Rows[Index].Cells["Qty"].Value.ToString()) / Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value)))) * Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value);
                                ////else if (tmpUnit != Unit && tmpUnit == "KG")
                                ////    dgvPODetails1.Rows[Index].Cells["Qty"].Value = (Qty / Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value)) - ((TmpQty / Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value))-Decimal.Parse(dgvPODetails1.Rows[Index].Cells["Qty"].Value.ToString()));

                                //msg = "Maaf Qty tidak boleh lebih besar dari Qty Agreement,\nberikut nilai Qty maksimal yang bisa diambil yaitu: " + (decimal.Parse(dgvPODetails1.Rows[Index].Cells["Qty"].Value.ToString())).ToString("N2") + ".\nUntuk Item No. " + dgvPODetails1.Rows[Index].Cells["No"].Value + "";
                                //REMARKED BY: HC (E)
                                msg = "Maaf Qty tidak boleh lebih besar dari Qty Agreement!";
                            }
                        }
                        else
                        {
                            if (TmpAmount > Amount)
                            {
                                //REMARKED BY: HC (S)
                                //decimal TmpAmount1 = 0;
                                //for (int i = 0; i < dgvPODetails1.RowCount; i++)
                                //{
                                //    if (dgvPODetails1.Rows[Index].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() && dgvPODetails1.Rows[Index].Cells["FullItemId"].Value.ToString() != dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString())
                                //    {
                                //        if (tmpUnit == dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString())
                                //            TmpAmount1 += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                                //        else
                                //        {
                                //            TmpAmount1 += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                                //        }
                                //    }
                                //}

                                //decimal PPhA = 0;
                                //decimal PPnA = 0;
                                //decimal DiskonA = 0;
                                //decimal PriceA = 0;
                                //decimal RatioA = 0;

                                //if (cmbPPn.Text == "" || cmbPPn.SelectedIndex == 0)
                                //    PPnA = 0;
                                //else
                                //    PPnA = Convert.ToDecimal(cmbPPn.Text);
                                //if (cmbPPh.Text == "" || cmbPPh.SelectedIndex == 0)
                                //    PPhA = 0;
                                //else
                                //    PPhA = Convert.ToDecimal(cmbPPh.Text);

                                //PriceA = Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Price"].Value == null ? "0" : dgvPODetails1.Rows[Index].Cells["Price"].Value.ToString());
                                //DiskonA = Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Diskon(%)"].Value == null ? "0" : dgvPODetails1.Rows[Index].Cells["Diskon(%)"].Value.ToString());
                                //RatioA = Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value == null ? "0" : dgvPODetails1.Rows[Index].Cells["Ratio"].Value.ToString());

                                //if (tmpUnit == dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString())
                                //    dgvPODetails1.Rows[Index].Cells["Qty"].Value = (tmpRemainingAmount - TmpAmount1) / (PriceA);
                                ////dgvPODetails1.Rows[Index].Cells["Qty"].Value = (tmpRemainingAmount - TmpAmount1) / ((1 - (DiskonA / 100)) * (1 + (PPnA / 100) + (PPhA / 100)) * (PriceA));
                                //else
                                //{
                                //    if (dgvPODetails1.Rows[Index].Cells["Unit"].Value.ToString() == "KG")
                                //        dgvPODetails1.Rows[Index].Cells["Qty"].Value = (tmpRemainingAmount - TmpAmount1) / (PriceA);
                                //    //dgvPODetails1.Rows[Index].Cells["Qty"].Value = (tmpRemainingAmount - TmpAmount1) / ((1 - (DiskonA / 100)) * (1 + (PPnA / 100) + (PPhA / 100)) * (PriceA));
                                //    else
                                //        dgvPODetails1.Rows[Index].Cells["Qty"].Value = (tmpRemainingAmount - TmpAmount1) / (PriceA * RatioA);
                                //    //dgvPODetails1.Rows[Index].Cells["Qty"].Value = (tmpRemainingAmount - TmpAmount1) / ((1 - (DiskonA / 100)) * (1 + (PPnA / 100) + (PPhA / 100)) * (PriceA * RatioA));
                                //}
                                //msg = "Maaf Qty menghasilkan Total Nett lebih besar dari Amount Agreement,\nberikut nilai Qty maksimal yang bisa diambil yaitu: " + (decimal.Parse(dgvPODetails1.Rows[Index].Cells["Qty"].Value.ToString())).ToString("N2") + ".\nUntuk Item No. " + dgvPODetails1.Rows[Index].Cells["No"].Value + "";
                                //REMARKED BY: HC (E)

                                msg = "Maaf Qty menghasilkan Total Nett lebih besar dari Amount Agreement!";
                            }
                        }

                        Conn.Close();
                    }
                }
                else if (Mode == "Edit")
                {
                    decimal RemainingQty = 0;
                    decimal JumlahQty = 0;
                    decimal JumlahQtyLain = 0;
                    decimal RemainingAmount = 0;
                    decimal JumlahAmount = 0;
                    decimal JumlahAmountLain = 0;

                    Conn = ConnectionString.GetConnection();
                    if (txtTransType.Text.ToUpper() != "AMOUNT")
                    {
                        if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Base"].Value.ToString() != "")
                        {
                            Query = "Select RemainingQty from PurchAgreementDtl where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() + "' and Base='Y'";
                        }
                        else
                        {
                            Query = "Select RemainingQty from PurchAgreementDtl where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() + "'";
                        }
                        Cmd = new SqlCommand(Query, Conn);
                        RemainingQty += Convert.ToDecimal(Cmd.ExecuteScalar());

                        //Jumlah Remaining QTY
                        for (int i = 0; i < dgvPODetails1.RowCount; i++)
                        {
                            if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                            {
                                RemainingQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["RemainingQty"].Value);
                                if (dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString() != "KG")
                                {
                                    JumlahQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) * Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value);
                                }
                                else
                                    JumlahQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                            }
                            if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() && dgvPODetails1.CurrentRow.Index != i)
                            {
                                if (dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString() != "KG")
                                    JumlahQtyLain += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value) * Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Ratio"].Value);
                                else
                                    JumlahQtyLain += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                            }
                        }

                        //Conn = ConnectionString.GetConnection();
                        //if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Base"].Value.ToString() != "")
                        //{
                        //    Query = "Select RemainingQty from PurchAgreementDtl where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() + "' and Base='Y'";
                        //}
                        //else
                        //{
                        //    Query = "Select RemainingQty from PurchAgreementDtl where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() + "'"; 
                        //}
                        //Cmd = new SqlCommand(Query, Conn);
                        //RemainingQty += Convert.ToDecimal(Cmd.ExecuteScalar());

                        //RemainingQty -= JumlahQty;

                        //if (Convert.ToDecimal(dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Qty"].Value) > RemainingQty)


                        if (JumlahQty > RemainingQty)
                        {
                            //msg = "Qty tidak boleh lebih besar dari remaining qty.";
                            if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Unit"].Value.ToString() == "KG")
                            {
                                dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Qty"].Value = RemainingQty - JumlahQtyLain;
                            }
                            else
                            {
                                dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Qty"].Value = (RemainingQty - JumlahQtyLain) / Convert.ToDecimal(dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Ratio"].Value);
                            }
                            msg = "Maaf Qty tidak boleh lebih besar dari Qty Agreement,\nberikut nilai Qty maksimal yang bisa diambil yaitu: " + (decimal.Parse(dgvPODetails1.Rows[Index].Cells["Qty"].Value.ToString())).ToString("N2") + ".\nUntuk Item No. " + dgvPODetails1.Rows[Index].Cells["No"].Value + "";
                        }
                    }
                    else
                    {
                        if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Base"].Value.ToString() != "")
                        {
                            Query = "Select RemainingAmount from PurchAgreementDtl where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() + "' and Base='Y'";
                        }
                        else
                        {
                            Query = "Select RemainingAmount from PurchAgreementDtl where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() + "'";
                        }
                        Cmd = new SqlCommand(Query, Conn);
                        RemainingAmount += Convert.ToDecimal(Cmd.ExecuteScalar());

                        for (int i = 0; i < dgvPODetails1.RowCount; i++)
                        {
                            if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                            {
                                if (dgvPODetails1.Rows[i].Cells["RemainingAmount"].Value == null || dgvPODetails1.Rows[i].Cells["RemainingAmount"].Value == String.Empty || dgvPODetails1.Rows[i].Cells["RemainingAmount"].Value == DBNull.Value)
                                    dgvPODetails1.Rows[i].Cells["RemainingAmount"].Value = 0;
                                RemainingAmount += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["RemainingAmount"].Value);

                                JumlahAmount += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                            }
                            if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() && dgvPODetails1.CurrentRow.Index != i)
                            {
                                JumlahAmountLain += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["TotalNett"].Value);
                            }
                        }

                        if (JumlahAmount > RemainingAmount)
                        {
                            decimal PPhA = 0;
                            decimal PPnA = 0;
                            decimal DiskonA = 0;
                            decimal PriceA = 0;
                            decimal RatioA = 0;

                            if (cmbPPn.Text == "" || cmbPPn.SelectedIndex == 0)
                                PPnA = 0;
                            else
                                PPnA = Convert.ToDecimal(cmbPPn.Text);
                            if (cmbPPh.Text == "" || cmbPPh.SelectedIndex == 0)
                                PPhA = 0;
                            else
                                PPhA = Convert.ToDecimal(cmbPPh.Text);

                            PriceA = Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Price"].Value == null ? "0" : dgvPODetails1.Rows[Index].Cells["Price"].Value.ToString());
                            DiskonA = Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Diskon(%)"].Value == null ? "0" : dgvPODetails1.Rows[Index].Cells["Diskon(%)"].Value.ToString());
                            RatioA = Convert.ToDecimal(dgvPODetails1.Rows[Index].Cells["Ratio"].Value == null ? "0" : dgvPODetails1.Rows[Index].Cells["Ratio"].Value.ToString());

                            if (dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Unit"].Value.ToString() == "KG")
                            {
                                dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Qty"].Value = (RemainingAmount - JumlahAmountLain) / ((1 - (DiskonA / 100)) * (1 + (PPnA / 100) + (PPhA / 100)) * (PriceA));
                            }
                            else
                            {
                                dgvPODetails1.Rows[dgvPODetails1.CurrentRow.Index].Cells["Qty"].Value = (RemainingAmount - JumlahAmountLain) / ((1 - (DiskonA / 100)) * (1 + (PPnA / 100) + (PPhA / 100)) * (PriceA * RatioA));
                            }
                            msg = "Maaf Qty menghasilkan Total Nett lebih besar dari Amount Agreement,\nberikut nilai Qty maksimal yang bisa diambil yaitu: " + (decimal.Parse(dgvPODetails1.Rows[Index].Cells["Qty"].Value.ToString())).ToString("N2") + ".\nUntuk Item No. " + dgvPODetails1.Rows[Index].Cells["No"].Value + "";
                        }
                    }
                }
                if (msg != null)
                {
                    MessageBox.Show(msg);
                }
            }
        }

        private void dgvPODetails1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //ValidationRemainingQty();

            if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "Unit")
            {
                if (dgvPODetails1.CurrentCell.Value == "" || dgvPODetails1.CurrentCell.Value == null)
                {
                    dgvPODetails1.Rows[e.RowIndex].Cells["RemainingQty"].Value = "0";
                }

                Conn = ConnectionString.GetConnection();
                Query = "Select Unit,RemainingQty from PurchAgreementDtl where AgreementID = '" + txtReffID.Text + "' and SeqNoGroup = '" + dgvPODetails1.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString() + "' and Base='Y'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                string TmpUnit = "";
                decimal TmpRemainingQty = 0;
                while (Dr.Read())
                {
                    TmpUnit = Dr["Unit"].ToString();
                    TmpRemainingQty = decimal.Parse(Dr["RemainingQty"].ToString());
                }
                if (Mode != "Edit")
                {
                    if (TmpUnit == dgvPODetails1.Rows[e.RowIndex].Cells["Unit"].Value.ToString())
                    {
                        if (dgvPODetails1.Rows[e.RowIndex].Cells["Base"].Value.ToString() == "Y")
                        {
                            dgvPODetails1.Rows[e.RowIndex].Cells["RemainingQty"].Value = TmpRemainingQty;
                            //dgvPODetails1.Rows[e.RowIndex].Cells["Qty"].Value = TmpRemainingQty;
                        }
                        else
                        {
                            decimal TmpQty = 0;
                            for (int i = 0; i < dgvPODetails1.RowCount; i++)
                            {
                                if (dgvPODetails1.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() && dgvPODetails1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString() != dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString())
                                {
                                    TmpQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                }
                            }
                            //dgvPODetails1.Rows[e.RowIndex].Cells["Qty"].Value = TmpRemainingQty - TmpQty;
                        }
                    }
                    else
                    {
                        if (TmpUnit == "KG")
                        {
                            if (dgvPODetails1.Rows[e.RowIndex].Cells["Base"].Value.ToString() == "Y")
                            {
                                dgvPODetails1.Rows[e.RowIndex].Cells["RemainingQty"].Value = TmpRemainingQty / Convert.ToDecimal(dgvPODetails1.Rows[e.RowIndex].Cells["Ratio"].Value);
                                //dgvPODetails1.Rows[e.RowIndex].Cells["Qty"].Value = TmpRemainingQty / Convert.ToDecimal(dgvPODetails1.Rows[e.RowIndex].Cells["Ratio"].Value);
                            }
                            else
                            {
                                decimal TmpQty = 0;
                                for (int i = 0; i < dgvPODetails1.RowCount; i++)
                                {
                                    if (dgvPODetails1.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() && dgvPODetails1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString() != dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString())
                                    {
                                        TmpQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                    }
                                }
                                //dgvPODetails1.Rows[e.RowIndex].Cells["Qty"].Value = (TmpRemainingQty - TmpQty) / Convert.ToDecimal(dgvPODetails1.Rows[e.RowIndex].Cells["Ratio"].Value);
                            }
                        }
                        else
                        {
                            if (dgvPODetails1.Rows[e.RowIndex].Cells["Base"].Value.ToString() == "Y")
                            {
                                dgvPODetails1.Rows[e.RowIndex].Cells["RemainingQty"].Value = TmpRemainingQty * Convert.ToDecimal(dgvPODetails1.Rows[e.RowIndex].Cells["Ratio"].Value);
                                //dgvPODetails1.Rows[e.RowIndex].Cells["Qty"].Value = TmpRemainingQty * Convert.ToDecimal(dgvPODetails1.Rows[e.RowIndex].Cells["Ratio"].Value);
                            }
                            else
                            {
                                decimal TmpQty = 0;
                                for (int i = 0; i < dgvPODetails1.RowCount; i++)
                                {
                                    if (dgvPODetails1.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString() == dgvPODetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() && dgvPODetails1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString() != dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString())
                                    {
                                        TmpQty += Convert.ToDecimal(dgvPODetails1.Rows[i].Cells["Qty"].Value);
                                    }
                                }
                                //dgvPODetails1.Rows[e.RowIndex].Cells["Qty"].Value = (TmpRemainingQty - TmpQty) * Convert.ToDecimal(dgvPODetails1.Rows[e.RowIndex].Cells["Ratio"].Value);
                            }
                        }
                    }
                }
            }

            //ValidationRemainingQty();

            if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "AvailableDate")
            {
                if (dgvPODetails1.CurrentCell.Value != "" && dgvPODetails1.CurrentCell.Value != null)
                {
                    //dgvPODetails1.CurrentCell.Value = dtp.Value.Date.ToString("dd-MM-yyyy"); //REMARKED BY: HC 
                    dgvPODetails1.CurrentCell.Value = Convert.ToDateTime(dtp.Value); //BY: HC
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }

            if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "Diskon(%)")
            {
                if (dgvPODetails1.CurrentCell.Value == "" || dgvPODetails1.CurrentCell.Value == null)
                {
                    dgvPODetails1.CurrentCell.Value = "0";
                }
            }

            dtp.Visible = false;

            if (Mode != "BeforeEdit")
            {

                if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                {
                    DeliveryMethod.Visible = false;
                }
            }

            //BY: HC (S)
            if (dgvPODetails1.Columns[e.ColumnIndex].Name.Contains("Price"))
            {
                if (dgvPODetails1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "")
                    dgvPODetails1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0.00";
                dgvPODetails1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dgvPODetails1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("N2");
            }
            if (dgvPODetails1.Columns[e.ColumnIndex].Name.Contains("Qty"))
            {
                if (dgvPODetails1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == "")
                    dgvPODetails1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0.00";
                dgvPODetails1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dgvPODetails1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value).ToString("N2");
            }
            //BY: HC (E)

            dgvPODetails1.AutoResizeColumns();
        }

        private decimal GetOldQty(string TmpUnit)
        {
            Conn = ConnectionString.GetConnection();
            decimal result = 0;
            decimal Ratio = 0;
            string TmpUnit1 = "";
            string Query1 = "SELECT Qty,Unit,Konv_Ratio FROM PurchDtl WHERE PurchID = '" + txtPOId.Text + "' AND ReffSeqNo = '" + dgvPODetails1.Rows[Index].Cells["ReffSeqNo"].Value + "' ";
            Cmd = new SqlCommand(Query1, Conn);
            SqlDataReader Dr1 = Cmd.ExecuteReader();
            while (Dr1.Read())
            {
                result = Convert.ToDecimal(Dr1["Qty"]);
                TmpUnit1 = Dr1["Unit"].ToString();
                Ratio = Convert.ToDecimal(Dr1["Konv_Ratio"]);
            }
            if (TmpUnit != TmpUnit1)
            {
                if (TmpUnit1 == "KG")
                {
                    result = result / Ratio;
                }
                else
                {
                    result = result * Ratio;
                }
            }
            Dr1.Close();
            Conn.Close();

            return result;
        }

        private void cmbReffTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbReffTableName.Text == "Canvass Sheet")
            {
                btnNew.Enabled = false;
                btnDelete.Enabled = false;
                cmbCurrency.Enabled = false;
            }
            else
            {
                if (Mode == "BeforeEdit")
                {
                    btnNew.Enabled = false;
                    btnDelete.Enabled = false;
                    cmbCurrency.Enabled = false;
                }
                else
                {
                    btnNew.Enabled = true;
                    btnDelete.Enabled = true;
                    //cmbCurrency.Enabled = true; //REMARKED BY: HC
                }
            }
            dgvPODetails1.Rows.Clear();
            dgvPODetails1.Columns.Clear();
            txtReffID.Text = "";
            txtReffID2.Text = "";
            txtVendId.Text = "";
            txtTransType.Text = "";

            cmbCurrency.Text = "";
            //txtDPPercent.Text = "";
            txtExchRate.Text = "";
            cmbPPh.Text = "0.00";
            cmbPPn.Text = "0.00";

            txtVendName.Text = "";
            cmbTermOfPayment.SelectedIndex = -1;
            cmbPaymentMode.SelectedIndex = -1;
            cmbDPRequired.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();

                    DialogResult dialogResult = MessageBox.Show("Apakah PO : " + txtPOId.Text + " akan di close ", "Update Status Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        //UpdateCredit limit at vend table
                        decimal TotalNett = Convert.ToDecimal(txtTotalNett.Text);
                        decimal OldTotal = 0;
                        string OldVendorId = "";
                        bool status = false;
                        Query = "SELECT [Total_Nett],[VendID] FROM [dbo].[PurchH] WHERE [PurchID] = '" + txtPOId.Text + "'";
                        using (Cmd = new SqlCommand(Query, Conn))
                        {
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    OldTotal = Convert.ToDecimal(Dr["Total_Nett"]);
                                    OldVendorId = Dr["VendID"].ToString();
                                }
                            }
                            Dr.Close();
                        }
                        if (OldTotal != 0)
                        {
                            Query = "UPDATE [dbo].[VendTable] SET [Sisa_Limit_Total] += " + OldTotal + " WHERE [VendId] = '" + OldVendorId + "'";
                            using (Cmd = new SqlCommand(Query, Conn))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        //================================

                        Query = "Update [dbo].[PurchH] set StClose = 1, TransStatus = '07' where PurchId = '" + txtPOId.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        Query = "Update [dbo].[PurchDtl] set RemainingQty = '0.00' where PurchId = '" + txtPOId.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        //created by: Thaddaeus Matthias, 14 March 2018
                        //for inserting the status log
                        //=======================================begin======================================
                        insertstatuslogclose();
                        //========================================end=======================================

                        //BY: HC (S) | INSERT KE TABLE LOG PO
                        Query = "Insert into [PO_Issued_LogTable] ([PODate],[POId],[VendId],[Qty_UoM],[Qty_Alt],[PAId],[PADate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[POSeqNo]) ";
                        Query += "Select top 1 PODate, POId, VendId, Qty_UoM, Qty_Alt, PAId, PADate, '07', 'Closed', 'Closed', '" + ControlMgr.UserId + "', getdate(), POSeqNo from PO_Issued_LogTable where POId = '" + txtPOId.Text + "' order by LogDate desc";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        //BY: HC (E)

                        MessageBox.Show("PO : " + txtPOId.Text + " berhasil di Close");
                        Parent.RefreshGrid();
                    }
                    Scope.Complete();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
                return;
            }
            finally
            {
                Conn.Close();
            }

            this.Close();
        }

        private void textExchangeRate_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void textExchangeRate_Leave(object sender, EventArgs e)
        {
            if (txtExchRate.Text != "")
            {
                double d = double.Parse(txtExchRate.Text);
                txtExchRate.Text = d.ToString("N2");
            }
        }

        //created by : Thaddaeus Matthias, 14 March 2018
        //Insert into status log table supplier/vendor
        //========================================begin=============================================
        private void insertstatuslog(string id)
        {
            string PurchOrderId = txtPOId.Text;
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] (StatusLog_FormName, Vendor_Id, StatusLog_PK1, StatusLog_PK2, StatusLog_PK3, StatusLog_PK4, StatusLog_Status, StatusLog_Description, StatusLog_UserID, StatusLog_Date) VALUES ('POForm', '" + txtVendId.Text + "',";
            if (Mode == "New" || Mode == "Generate")
            {
                PurchOrderId = id;
                Query += "'" + PurchOrderId + "' , ";
            }
            else if (Mode == "Amend")
            {
                PurchOrderId = id;
                Query += "'" + PurchOrderId + "' , ";
            }
            Query += " '" + txtReffID.Text + "', '" + txtReffID2.Text + "', '', ";
            if (cmbReffTableName.Text == "Purchase Agreement")
            {
                Query += "'04', 'Amend - Waiting for Approval',";
            }
            else
            {
                Query += "'01', 'Open Order',";
            }
            Query += " '" + ControlMgr.UserId + "', getdate())";
            SqlCommand cmd2 = new SqlCommand(Query, Conn);
            cmd2.ExecuteNonQuery();
        }

        private void insertstatuslogclose()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] (StatusLog_FormName, Vendor_Id, StatusLog_PK1, StatusLog_PK2, StatusLog_PK3, StatusLog_PK4, ";
            Query += " StatusLog_Status, StatusLog_Description, StatusLog_UserID, StatusLog_Date) VALUES ('POForm','" + txtVendId.Text + "',";
            if (Mode == "Amend")
            {
                Query += "'" + txtReffID.Text + "' , ";
            }
            else
            {
                Query += "'" + txtPOId.Text + "' , ";
            }
            Query += " '" + txtReffID.Text + "', '" + txtReffID2.Text + "', '', ";
            Query += "'07','Closed',";
            Query += " '" + ControlMgr.UserId + "', getdate())";
            SqlCommand cmd2 = new SqlCommand(Query, Conn);
            cmd2.ExecuteNonQuery();
        }

        //=========================================end==============================================

        private void dgvPODetails1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPODetails1.Columns[dgvPODetails1.CurrentCell.ColumnIndex].Name == "Diskon(%)" || dgvPODetails1.Columns[dgvPODetails1.CurrentCell.ColumnIndex].Name == "Qty" || dgvPODetails1.Columns[dgvPODetails1.CurrentCell.ColumnIndex].Name == "BonusScheme" || dgvPODetails1.Columns[dgvPODetails1.CurrentCell.ColumnIndex].Name == "Price")//edited by Thaddaeus, 5JUNE2018
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
                if ((sender as TextBox) != null) //edited by Thaddaeus, 5JUNE2018
                {
                    if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    {
                        e.Handled = true;
                    }
                }
            }

            if (dgvPODetails1.Columns[dgvPODetails1.CurrentCell.ColumnIndex].Name == "DeliveryMethod")
            {
                if (!char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

        }

        private void dgvPODetails1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString().ToUpper() == "TEXT")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPODetails1_KeyPress);
                e.Control.KeyPress += new KeyPressEventHandler(dgvPODetails1_KeyPress);
            }

            if (dgvPODetails1.CurrentCell.ColumnIndex == 11)
            {
                SelectedCell = dgvPODetails1.CurrentCell.RowIndex;
                // Check box column
                ComboBox comboBox = e.Control as ComboBox;
                comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
            }
        }

        void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = ((ComboBox)sender).SelectedItem.ToString();
            //int selectedIndex = ((ComboBox)sender).SelectedIndex;

            if (cmbReffTableName.Text == "Purchase Agreement")
            {
                string SeqNo = dgvPODetails1.Rows[Index].Cells["SeqNo"].Value.ToString();
                if (selectedItem != getOldUnit(txtReffID.Text, SeqNo))
                {

                    ValidationRemainingQty();
                }
            }
        }

        //created by Thaddaeus, 5JUNE2018, BEGIN
        private void txtDP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
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
                    if (dgvAttachment.Columns.Count <= 0)
                    {
                        dgvAttachment.ColumnCount = 6;
                        dgvAttachment.Columns[0].Name = "No";
                        dgvAttachment.Columns[2].Name = "FileName"; dgvAttachment.Columns[2].HeaderText = "File Name";
                        dgvAttachment.Columns[3].Name = "ContentType";
                        dgvAttachment.Columns[4].Name = "FileSize"; dgvAttachment.Columns[4].HeaderText = "File Size (Kb)";
                        dgvAttachment.Columns[5].Name = "Attachment";
                        dgvAttachment.Columns[1].Name = "FileId";
                    }
                    this.dgvAttachment.Rows.Add(dgvAttachment.Rows.Count + 1, "", FileName[i], Extension[i], filesize.ToString(), System.Text.Encoding.UTF8.GetString(data));
                    test.Add(data);
                    i++;
                }
                dgvAttachment.Columns[1].Visible = false;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                String fileid = dgvAttachment.CurrentRow.Cells["FileId"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileId"].Value.ToString();
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

        private void cmbDPRequired_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (Mode == "Amend")
            //{
            //    if (cmbDPRequired.Text == "NO")
            //    {
            //        txtDPPercent.ReadOnly = true;
            //        txtDPAmount.ReadOnly = true;
            //        txtDPPercent.Text = "0.00";
            //        txtDPAmount.Text = "0.00";
            //    }
            //    else
            //    {
            //        txtDPPercent.ReadOnly = false;
            //        txtDPAmount.ReadOnly = false;
            //    }
            //}
            //else
            //{
            //    txtDPPercent.ReadOnly = true;
            //    txtDPAmount.ReadOnly = true;
            //}

            //BY: HC (S)
            if (cmbDPRequired.Text == "NO")
            {
                label10.Visible = false;
                cmbDPType.Visible = false;
                cbxHitung.Visible = false;
                label2.Visible = false;
                tbxDPAmount.Visible = false;
                tbxDPPercent.Visible = false;
            }
            else if (cmbDPRequired.Text == "YES")
            {
                label10.Visible = true;
                cmbDPType.Visible = true;
                cbxHitung.Visible = true;
                label2.Visible = true;
                tbxDPPercent.Visible = true;
            }
            //BY: HC (E)
        }

        private void txtDPPercent_TextChanged(object sender, EventArgs e)
        {
            //if (txtDPPercent.Text == "")
            //    txtDPPercent.Text = "0";
            //if (txtTotalNett.Text == "")
            //    txtTotalNett.Text = "0";
            //txtDPAmount.Text = (Convert.ToDecimal(txtTotalNett.Text) * Convert.ToDecimal(txtDPPercent.Text) / 100).ToString("N2");

            //if (txtDPPercent.Text != "0")
            //    cmbDPRequired.SelectedItem = "YES";
        }

        private void txtDPAmount_TextChanged(object sender, EventArgs e)
        {
            //if (txtDPAmount.Text == "")
            //txtDPAmount.Text = "0.00";
        }

        private void POForm_Shown(object sender, EventArgs e)
        {
            if (Close == true)
            {
                this.Close();
            }
        }

        private void txtTotalNett_TextChanged(object sender, EventArgs e)
        {
            //if (txtDPPercent.Text == "")
            //    txtDPPercent.Text = "0";

            //if (txtTotalNett.Text == "")
            //    txtTotalNett.Text = "0";

            //txtDPAmount.Text = (Convert.ToDecimal(txtTotalNett.Text) * Convert.ToDecimal(txtDPPercent.Text) / 100).ToString("N2");
        }

        private void amendSetting()
        {
            Conn = ConnectionString.GetConnection();
            if (dgvPODetails1.Rows.Count > 0)
            {
                //BY: HC (S)
                for (int i = 0; i < dgvPODetails1.ColumnCount; i++)
                {
                    dgvPODetails1.Columns[i].ReadOnly = true;
                }
                dgvPODetails1.Columns["Deskripsi"].ReadOnly = false;
                //BY: HC (E)
                for (int i = 0; i <= dgvPODetails1.Rows.Count - 1; i++)
                {
                    String SeqNo = dgvPODetails1.Rows[i].Cells["No"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["No"].Value.ToString();
                    String FullItemId = dgvPODetails1.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString();

                    Query = "SELECT [RemainingQty] FROM [ISBS-NEW4].[dbo].[PurchDtl] WHERE PurchID = '" + PONumber + "' AND [SeqNo] = '" + SeqNo + "' AND [FullItemId] = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Decimal RemainingQty = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());

                    if (RemainingQty == 0)
                    {
                        dgvPODetails1.Rows[i].Cells["Qty"].ReadOnly = true;
                        dgvPODetails1.Rows[i].Cells["Price"].ReadOnly = true;

                    }
                    else
                    {
                        //if (cmbReffTableName.Text == "Canvass Sheet") //REMARKED BY: HC
                        //    dgvPODetails1.Rows[i].Cells["Qty"].ReadOnly = true;
                        //else
                        dgvPODetails1.Rows[i].Cells["Qty"].ReadOnly = false;
                        dgvPODetails1.Rows[i].Cells["Price"].ReadOnly = false;
                        dgvPODetails1.Rows[i].Cells["DeliveryMethod"].ReadOnly = false;
                        dgvPODetails1.Rows[i].Cells["AvailableDate"].ReadOnly = false;
                    }
                }
            }
            Conn.Close();
        }

        private bool amendDeleteCheck()
        {
            Conn = ConnectionString.GetConnection();
            Boolean vBol = true;

            if (Mode == "Amend")
            {
                if (dgvPODetails1.RowCount > 0)
                {
                    Index = dgvPODetails1.CurrentRow.Index;
                    String SeqNo = dgvPODetails1.Rows[Index].Cells["No"].Value == null ? "" : dgvPODetails1.Rows[Index].Cells["No"].Value.ToString();
                    String FullItemId = dgvPODetails1.Rows[Index].Cells["FullItemId"].Value == null ? "" : dgvPODetails1.Rows[Index].Cells["FullItemId"].Value.ToString();
                    //Decimal Qty = dgvPODetails1.Rows[Index].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[Index].Cells["Qty"].Value.ToString());

                    Decimal RemainingQty = 0;
                    Decimal Qty = 0;

                    Query = "SELECT [RemainingQty],[Qty] FROM [ISBS-NEW4].[dbo].[PurchDtl] WHERE PurchID = '" + PONumber + "' AND [SeqNo] = '" + SeqNo + "' AND [FullItemId] = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        RemainingQty = Convert.ToDecimal(Dr["RemainingQty"].ToString());
                        Qty = Convert.ToDecimal(Dr["Qty"].ToString());
                    }

                    if (RemainingQty != Qty) //GA BISA DELETE JIKA SUDAH ADA DELIVERY
                    {
                        MessageBox.Show("Tidak bisa delte" + Environment.NewLine + "Item sudah ada delivery");
                        vBol = false;
                    }
                }
            }
            return vBol;
            Conn.Close();
        }

        private void cmbDPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BY: HC (S)
            switch (cmbDPType.SelectedIndex)
            {
                case 0: //PERCENTAGE
                    cbxHitung.Checked = false;
                    tbxDPAmount.Visible = false;
                    tbxDPPercent.Visible = true;
                    label2.Visible = true;
                    cbxHitung.Visible = true;
                    break;
                case 1: //AMOUNT
                    cbxHitung.Checked = false;
                    tbxDPAmount.Visible = true;
                    tbxDPPercent.Visible = false;
                    label2.Visible = false;
                    cbxHitung.Visible = false;
                    break;
            }
            //BY: HC (E)
        }

        private void cbxHitung_CheckedChanged(object sender, EventArgs e)
        {
            //BY: HC (S)
            if (cbxHitung.Checked == true)
            {
                tbxDPAmount.Location = new Point(224, 160);
                tbxDPAmount.Visible = true;
                tbxDPAmount.Enabled = false;
            }
            else if (cbxHitung.Checked == false)
            {
                tbxDPAmount.Location = new Point(224, 137);
                tbxDPAmount.Visible = false;
                tbxDPAmount.Enabled = true;
            }
            //BY: HC (E)
        }

        private void insertPO_LogTable(DateTime PODate, string POID, string VendId, decimal Qty_UoM, decimal Qty_Alt, string PAID, DateTime PADate, string LogStatusCode, string LogStatusDesc, string LogDescription, int POSeqNo)
        {
            Query = "Insert into [PO_Issued_LogTable] ([PODate],[POId],[VendId],[Qty_UoM],[Qty_Alt],[PAId],[PADate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[POSeqNo]) ";
            Query += "VALUES (@PODate, '" + POID + "', '" + VendId + "', '" + Qty_UoM + "', '" + Qty_Alt + "','" + PAID + "', @PADate,'" + LogStatusCode + "', '" + LogStatusDesc + "' ,'" + LogDescription + "', '" + ControlMgr.UserId + "', getdate(),'" + POSeqNo + "');";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@PODate", PODate);
            Cmd.Parameters.AddWithValue("@PADate", PADate);
            Cmd.ExecuteNonQuery();
        }

    }
}
