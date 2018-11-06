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

namespace ISBS_New.Purchase.ReceiptOrder
{
    public partial class HeaderReceiptOrder : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query, Query1, crit = null;
        String UoM = "";
        int Index, CountLoop = 0;
        List<decimal> OldPORemaining = new List<decimal>();

        //============Record Deleted Item, when save can be used to reduce the invent purchase qty======//
        List<string> deletedItem = new List<string>();
        List<decimal> deletedQty = new List<decimal>();
        List<string> deletedUnit = new List<string>();
        List<decimal> deletedPrice = new List<decimal>();
        List<decimal> deletedRatio = new List<decimal>();
        //==============================================================================================//

        //=========Retain Highest SeqNo====
        int ROSeqNo = 0;
        //=================================

        public string RONumber = "", tmpROType = "";

        DateTimePicker dtp;
        ComboBox DeliveryMethod;

        public string Gelombang;
        public string Bracket;

        public List<string> Gelombang1 = new List<string>();
        public List<string> Bracket1 = new List<string>();
        //List<DetailPR> ListDetailPR = new List<DetailPR>();
        //List<Gelombang> ListGelombang = new List<Gelombang>();
        //List<Info> ListInfo = new List<Info>();

        List<PopUp.Vendor.Vendor> ListVendor = new List<PopUp.Vendor.Vendor>();
        PopUp.Stock.Stock PopUpItemName = new PopUp.Stock.Stock();
        PopUp.FullItemId.FullItemId FullItemId = new PopUp.FullItemId.FullItemId();
        //tia edit
        TaskList.Purchase.ReceiptOrder.TaskListRO ParentToTLRO;
        public void setParent2(TaskList.Purchase.ReceiptOrder.TaskListRO F2)
        {
            ParentToTLRO = F2;
        }
        ContextMenu vendid = new ContextMenu();
        //tia edit end
        Purchase.ReceiptOrder.InquiryReceiptOrder Parent;
        String ROID = null;
        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public HeaderReceiptOrder()
        {
            InitializeComponent();
            if (Purchase.PurchaseQuotation.FormPQ.reffID != null)
                txtRoNumber.Text = Purchase.PurchaseQuotation.FormPQ.reffID;
            if (Purchase.PurchaseQuotation.FormPQ.pRID != null)
                txtRoNumber.Text = Purchase.PurchaseQuotation.FormPQ.pRID;
        }

        private void HeaderReceiptOrder_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
            GetDataHeader();

            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();

            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
            else if (Mode == "ModeView")
            {
                ModeView();
            }

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy";
            dtp.Visible = false;
            dtp.Width = 100;
            // asdf
            dgvRODetails.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;
            dgvRODetails.CellBeginEdit += this.dgvPrDetails_CellBeginEdit;
            dgvRODetails.CellEndEdit += this.dgvPrDetails_CellEndEdit;

            DeliveryMethod = new ComboBox();
            DeliveryMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            DeliveryMethod.Visible = false;
            dgvRODetails.Controls.Add(DeliveryMethod);
            DeliveryMethod.DropDownClosed += this.DeliveryMethod_DropDownClosed;
            DeliveryMethod.SelectionChangeCommitted += this.DeliveryMethod_SelectionChangeCommitted;
            Purchase.ReceiptOrder.InquiryReceiptOrder f = new Purchase.ReceiptOrder.InquiryReceiptOrder();
            f.RefreshGrid();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //SearchQuery tmpSearch = new SearchQuery();
            //tmpSearch.PrimaryKey = "PurchID";
            //tmpSearch.Table = "[dbo].[PurchH]";

            //tmpSearch.QuerySearch += "Select distinct a.[PurchID],a.SeqNo,a.ItemId,a.FullItemID,a.ItemName,case when a.Qty is null then 0 else a.Qty end Qty , case when a.Qty_Kg is null then 0 else a.Qty_Kg end Qty_Kg,a.Unit,d.ExpectedDateFrom,d.ExpectedDateTo,a.Deskripsi ";
            //tmpSearch.QuerySearch += "FROM [dbo].[PurchDtl] a ";
            //tmpSearch.QuerySearch += "inner join [dbo].[PurchH] b on a.[PurchID]=b.[PurchID] ";
            //tmpSearch.QuerySearch += "inner join [dbo].[PurchQuotation_Dtl] c on b.ReffId2=c.PurchQuotID ";
            //tmpSearch.QuerySearch += "inner join [dbo].[PurchRequisition_Dtl] d on c.ReffTransID=d.PurchReqID and c.ReffSeqNo=d.SeqNo ";
            //tmpSearch.QuerySearch += "where b.VendId ='" + txtPONumber.Text.Trim() + "' ";

            //if (dgvRODetails.Rows.Count > 0)
            //{
            //    tmpSearch.QuerySearch += "and a.PurchID+convert(varchar(10),a.SeqNo) not in (";
            //    for (int i = 0; i < dgvRODetails.Rows.Count; i++)
            //    {
            //        tmpSearch.QuerySearch += "'" + dgvRODetails.Rows[i].Cells["PurchaseOrderId"].Value.ToString() + dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value.ToString() + "'";
            //        if (i < dgvRODetails.Rows.Count - 1)
            //            tmpSearch.QuerySearch += ",";
            //    }
            //    tmpSearch.QuerySearch += ")";
            //}

            //tmpSearch.FilterText = new string[] { "PurchID", "ItemId", "FullItemID", "ItemName", "Qty", "Qty_Kg", "Unit", "ExpectedDateFrom", "ExpectedDateTo", "Deskripsi" };
            ////tmpSearch.FilterDate = new string[] { "" };
            //tmpSearch.Select = new string[] { "PurchID", "SeqNo", "ItemId", "FullItemID", "ItemName", "Qty", "Qty_Kg", "Unit", "ExpectedDateFrom", "ExpectedDateTo", "Deskripsi" };
            ////tmpSearch.WherePlus = "";
            ////tmpSearch.SetSchemaTable(SchemaName, TableName);
            //tmpSearch.ShowDialog();
            //if (ConnectionString.Kodes != null)
            //{
            //    string ExpectedDateFrom = Convert.ToDateTime(ConnectionString.Kodes[8]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(ConnectionString.Kodes[8]).ToString("dd-MM-yyyy");
            //    string ExpectedDateTo = Convert.ToDateTime(ConnectionString.Kodes[9]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(ConnectionString.Kodes[9]).ToString("dd-MM-yyyy");
            //    dgvRODetails.Rows.Add(dgvRODetails.RowCount + 1, ConnectionString.Kodes[0], ConnectionString.Kodes[1], "", ConnectionString.Kodes[2], ConnectionString.Kodes[3], ConnectionString.Kodes[4], ConnectionString.Kodes[5], ConnectionString.Kodes[6], ConnectionString.Kodes[7], ExpectedDateFrom, ExpectedDateTo, ConnectionString.Kodes[10]);
            //    ConnectionString.Kodes = null;
            //}

            if (txtPONumber.Text == "")
            {
                MessageBox.Show("Silahkan pilih no PO terlebih dahulu.");
                return;
            }

            SearchQueryV2 tmpSearch = new SearchQueryV2();
            tmpSearch.FormName = "Search RO Items";
            tmpSearch.PrimaryKey = "FullItemID";
            //tmpSearch.Order = "FullItemID asc";
            tmpSearch.Table = "[dbo].[PurchDtl]";
            //tmpSearch.QuerySearch = "Select a.PurchID 'PO_No', a.OrderDate 'PO Date', a.SeqNo, a.FullItemID, a.ItemName, a.Qty, a.RemainingQty, a.Unit From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID ";
            tmpSearch.QuerySearch = "Select a.PurchID, a.OrderDate, a.SeqNo, a.FullItemID, a.ItemName, a.Qty, a.RemainingQty, a.Unit, a.DeliveryMethod From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID";

            //tmpSearch.FilterText = new string[] { "FullItemID", "ItemName", "Unit" };
            tmpSearch.FilterText = new string[] { "PurchID", "OrderDate", "SeqNo", "FullItemID", "ItemName", "Qty", "RemainingQty", "Unit" };
            tmpSearch.Mask = new string[] { "PO No", "PO Date", "Seq No", "FullItemID", "Item Name", "PO Qty", "Remaining Qty", "Unit" };

            //tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
            //tmpSearch.Select = new string[] { "PO_No", "SeqNo" };
            tmpSearch.Select = new string[] { "PurchID", "SeqNo" };

            //tmpSearch.Hide = new string[] { "" };
            //tmpSearch.WherePlus = " and PO_No = '" + txtPONumber.Text + "' ";

            //BY: HC (S) | GET SEQNO YG ADA DI GRID
            string seqNo = "";
            for (int i = 0; i < dgvRODetails.RowCount; i++)
            {
                if (i == 0)
                    seqNo += " and a.seqNo not in ('" + dgvRODetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                else
                    seqNo += ",'" + dgvRODetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                if (i == dgvRODetails.RowCount - 1)
                    seqNo += ")";
            }
            //BY: HC (E)
            tmpSearch.WherePlus = " and a.PurchID = '" + txtPONumber.Text + "' and a.DeliveryMethod = '" + cmbDelivMethod.Text + "'" + seqNo;

            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();

            List<string> POID = new List<string>();
            List<string> SeqNo = new List<string>();

            if (Variable.Kode2 != null)
            {
                for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
                {
                    POID.Add(Variable.Kode2[i, 0]);
                    SeqNo.Add(Variable.Kode2[i, 1]);
                }
            }
            AddDataGridFromDetail(POID, SeqNo);
            Variable.Kode2 = null;

            //Purchase.ReceiptOrder.PO F = new Purchase.ReceiptOrder.PO();
            //F.flag(txtPONumber.Text);
            //F.setParent(this);
            //F.ShowDialog();
        }

        public void AddDataGridFromDetail(List<string> POID, List<string> SeqNo)
        {

            if (txtRoNumber.Text == "")
            {
                if (dgvRODetails.RowCount - 1 <= 0)
                {
                    dgvRODetails.ColumnCount = 31;
                    dgvRODetails.Columns[0].Name = "No";
                    dgvRODetails.Columns[1].Name = "FullItemID";
                    dgvRODetails.Columns[2].Name = "GroupId";
                    dgvRODetails.Columns[3].Name = "SubGroup1Id";
                    dgvRODetails.Columns[4].Name = "SubGroup2Id";
                    dgvRODetails.Columns[5].Name = "ItemId";
                    dgvRODetails.Columns[6].Name = "ItemName";
                    dgvRODetails.Columns[7].Name = "Qty"; dgvRODetails.Columns[7].HeaderText = "RO Qty";
                    dgvRODetails.Columns[8].Name = "RORemaining"; dgvRODetails.Columns[8].HeaderText = "RO Remaining";
                    dgvRODetails.Columns[9].Name = "Unit";
                    dgvRODetails.Columns[10].Name = "Qty_Kg"; dgvRODetails.Columns[10].HeaderText = "Total Berat";
                    dgvRODetails.Columns[11].Name = "POQty"; dgvRODetails.Columns[11].HeaderText = "PO Qty";
                    dgvRODetails.Columns[12].Name = "PORemaining"; dgvRODetails.Columns[12].HeaderText = "PO Remaining";
                    dgvRODetails.Columns[13].Name = "POUnit"; dgvRODetails.Columns[13].HeaderText = "PO Unit";
                    dgvRODetails.Columns[14].Name = "Ratio";
                    dgvRODetails.Columns[15].Name = "Notes";
                    dgvRODetails.Columns[16].Name = "Price";
                    dgvRODetails.Columns[17].Name = "Price_KG";
                    dgvRODetails.Columns[18].Name = "Total";
                    dgvRODetails.Columns[19].Name = "Diskon";
                    dgvRODetails.Columns[20].Name = "Total_Disk"; dgvRODetails.Columns[20].HeaderText = "Total Diskon";
                    dgvRODetails.Columns[21].Name = "Total_PPN"; dgvRODetails.Columns[21].HeaderText = "Total PPN";
                    dgvRODetails.Columns[22].Name = "Total_PPH"; dgvRODetails.Columns[22].HeaderText = "Total PPH";
                    dgvRODetails.Columns[23].Name = "POEstDeliveryDate"; dgvRODetails.Columns[23].HeaderText = "PO est. Delivery Date";
                    dgvRODetails.Columns[24].Name = "SONumber"; dgvRODetails.Columns[24].HeaderText = "SO Number";
                    dgvRODetails.Columns[25].Name = "SOExpectedDateFrom";
                    dgvRODetails.Columns[26].Name = "SOExpectedDateTo";
                    dgvRODetails.Columns[27].Name = "PurchaseOrderSeqNo";
                    dgvRODetails.Columns[28].Name = "DeliveryMethod";
                    dgvRODetails.Columns[29].Name = "PurchaseOrderID";
                    dgvRODetails.Columns[30].Name = "SeqNo";
                }

                for (int i = 0; i < POID.Count; i++)
                {
                    Query = "Select a.FullItemID, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.ItemName, case when a.Qty is null then 0 else a.Qty end Qty, a.RemainingQty, a.Unit, b.UoM, a.KOnv_Ratio, a.Price, case when a.Price_KG is null then 0 else a.Price_KG end Price_KG, a.Total, a.Diskon, a.Total_Disk, a.Total_PPN, a.Total_PPH, a.PurchID, a.SeqNo, a.DeliveryMethod, d.ReffTransID ";
                    Query += "From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID ";
                    Query += "LEFT JOIN dbo.[CanvasSheetD] c ON a.ReffId = c.CanvasID and a.ReffId2 = c.PurchQuotId and a.ReffSeqNo = c.CanvasSeqNo ";
                    Query += "LEFT JOIN dbo.[PurchRequisition_Dtl] d ON c.PurchReqId = d.PurchReqId and c.PurchReqSeqNo = d.SeqNo Where a.PurchId = '" + POID[i] + "' and a.SeqNo = '" + SeqNo[i] + "' ;";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            if (Dr["Unit"].ToString() == "KG" || Dr["Unit"].ToString() == "LBR")
                            {
                                UoM = Dr["UoM"].ToString();
                            }
                            else
                            {
                                UoM = Dr["Unit"].ToString();
                            }

                            this.dgvRODetails.Rows.Add((dgvRODetails.RowCount + 1).ToString(), Dr["FullItemID"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["ItemName"], "0.00", "0.00", UoM, "0.00", Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Konv_Ratio"], "", Dr["Price"], Dr["Price_KG"], Dr["Total"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPH"], "", Dr["ReffTransId"], "", "", Dr["SeqNo"], Dr["DeliveryMethod"], Dr["PurchId"], (dgvRODetails.Rows.Count + 1).ToString());
                            OldPORemaining.Add(Convert.ToDecimal(Dr["RemainingQty"]));
                        }
                        Dr.Close();
                    }
                    Conn.Close();
                }

                if (dgvRODetails.Columns.Count != 0)
                {
                    if (tabDgvControl.SelectedTab.Text == "Detail RO")
                    {
                        for (int i = 0; i < dgvRODetails.Columns.Count; i++)
                        {
                            if (dgvRODetails.Columns[i].HeaderText == "No" || dgvRODetails.Columns[i].HeaderText == "FullItemID" || dgvRODetails.Columns[i].HeaderText == "ItemName" || dgvRODetails.Columns[i].HeaderText == "RO Qty" || dgvRODetails.Columns[i].HeaderText == "Unit" || dgvRODetails.Columns[i].HeaderText == "Total Berat")
                            {
                                dgvRODetails.Columns[i].Visible = true;
                            }
                            else if (dgvRODetails.Columns[i].HeaderText == "PO Qty" || dgvRODetails.Columns[i].HeaderText == "PO Remaining" || dgvRODetails.Columns[i].HeaderText == "PO Unit" || dgvRODetails.Columns[i].HeaderText == "Ratio" || dgvRODetails.Columns[i].HeaderText == "Notes")
                            {
                                dgvRODetails.Columns[i].Visible = true;
                            }
                            else
                            {
                                dgvRODetails.Columns[i].Visible = false;
                            }

                            if (dgvRODetails.Columns[i].HeaderText == "RO Qty" || dgvRODetails.Columns[i].HeaderText == "Notes")
                            {
                                dgvRODetails.Columns[i].ReadOnly = false;
                            }
                            else
                            {
                                dgvRODetails.Columns[i].ReadOnly = true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < dgvRODetails.Columns.Count; i++)
                        {
                            if (dgvRODetails.Columns[i].HeaderText == "No" || dgvRODetails.Columns[i].HeaderText == "FullItemID" || dgvRODetails.Columns[i].HeaderText == "ItemName")
                            {
                                dgvRODetails.Columns[i].Visible = true;
                            }
                            else if (dgvRODetails.Columns[i].HeaderText == "PO est. Delivery Date" || dgvRODetails.Columns[i].HeaderText == "SO Number" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateFrom" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateTo")
                            {
                                dgvRODetails.Columns[i].Visible = true;
                            }
                            else
                            {
                                dgvRODetails.Columns[i].Visible = false;
                            }

                            if (dgvRODetails.Columns[i].HeaderText == "PO est. Delivery Date" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateFrom" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateTo")
                            {
                                dgvRODetails.Columns[i].ReadOnly = false;
                            }
                            else
                            {
                                dgvRODetails.Columns[i].ReadOnly = true;
                            }

                        }
                    }
                }

                dgvRODetails.Columns["GroupId"].Visible = false;
                dgvRODetails.Columns["SubGroup1Id"].Visible = false;
                dgvRODetails.Columns["SubGroup2Id"].Visible = false;
                dgvRODetails.Columns["ItemId"].Visible = false;
                dgvRODetails.Columns["PurchaseOrderSeqNo"].Visible = false;
                dgvRODetails.Columns["PurchaseOrderID"].Visible = false;



                dgvRODetails.Columns["Price"].Visible = false;
                dgvRODetails.Columns["Price_KG"].Visible = false;
                dgvRODetails.Columns["Total"].Visible = false;
                dgvRODetails.Columns["Diskon"].Visible = false;
                dgvRODetails.Columns["Total_Disk"].Visible = false;
                dgvRODetails.Columns["Total_PPN"].Visible = false;
                dgvRODetails.Columns["Total_PPH"].Visible = false;
                dgvRODetails.Columns["POEstDeliveryDate"].Visible = false;
                dgvRODetails.Columns["SONumber"].Visible = false;
                dgvRODetails.Columns["SOExpectedDateFrom"].Visible = false;
                dgvRODetails.Columns["SOExpectedDateTo"].Visible = false;
                dgvRODetails.Columns["PurchaseOrderSeqNo"].Visible = false;
                dgvRODetails.Columns["DeliveryMethod"].Visible = false;

                dgvRODetails.AutoResizeColumns();
            }
            else
            {
                if (dgvRODetails.RowCount - 1 <= 0)
                {
                    dgvRODetails.ColumnCount = 31;
                    dgvRODetails.Columns[0].Name = "No";
                    dgvRODetails.Columns[1].Name = "FullItemID";
                    dgvRODetails.Columns[2].Name = "GroupId";
                    dgvRODetails.Columns[3].Name = "SubGroup1Id";
                    dgvRODetails.Columns[4].Name = "SubGroup2Id";
                    dgvRODetails.Columns[5].Name = "ItemId";
                    dgvRODetails.Columns[6].Name = "ItemName";
                    dgvRODetails.Columns[7].Name = "Qty"; dgvRODetails.Columns[7].HeaderText = "RO Qty";
                    dgvRODetails.Columns[8].Name = "RORemaining"; dgvRODetails.Columns[8].HeaderText = "RO Remaining";
                    dgvRODetails.Columns[9].Name = "Unit";
                    dgvRODetails.Columns[10].Name = "Qty_Kg"; dgvRODetails.Columns[10].HeaderText = "Total Berat";
                    dgvRODetails.Columns[11].Name = "POQty"; dgvRODetails.Columns[11].HeaderText = "PO Qty";
                    dgvRODetails.Columns[12].Name = "PORemaining"; dgvRODetails.Columns[12].HeaderText = "PO Remaining";
                    dgvRODetails.Columns[13].Name = "POUnit"; dgvRODetails.Columns[13].HeaderText = "PO Unit";
                    dgvRODetails.Columns[14].Name = "Ratio";
                    dgvRODetails.Columns[15].Name = "Notes";
                    dgvRODetails.Columns[16].Name = "Price";
                    dgvRODetails.Columns[17].Name = "Price_KG";
                    dgvRODetails.Columns[18].Name = "Total";
                    dgvRODetails.Columns[19].Name = "Diskon";
                    dgvRODetails.Columns[20].Name = "Total_Disk"; dgvRODetails.Columns[20].HeaderText = "Total Diskon";
                    dgvRODetails.Columns[21].Name = "Total_PPN"; dgvRODetails.Columns[21].HeaderText = "Total PPN";
                    dgvRODetails.Columns[22].Name = "Total_PPH"; dgvRODetails.Columns[22].HeaderText = "Total PPH";
                    dgvRODetails.Columns[23].Name = "POEstDeliveryDate"; dgvRODetails.Columns[23].HeaderText = "PO est. Delivery Date";
                    dgvRODetails.Columns[24].Name = "SONumber"; dgvRODetails.Columns[24].HeaderText = "SO Number";
                    dgvRODetails.Columns[25].Name = "SOExpectedDateFrom";
                    dgvRODetails.Columns[26].Name = "SOExpectedDateTo";
                    dgvRODetails.Columns[27].Name = "PurchaseOrderSeqNo";
                    dgvRODetails.Columns[28].Name = "DeliveryMethod";
                    dgvRODetails.Columns[29].Name = "PurchaseOrderID";
                    dgvRODetails.Columns[30].Name = "SeqNo";
                }

                for (int i = 0; i < POID.Count; i++)
                {
                    Query = "Select a.FullItemID, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.ItemName, a.Qty, a.RemainingQty, a.Unit, b.UoM, a.KOnv_Ratio, a.Price, a.Price_KG, a.Total, a.Diskon, a.Total_Disk, a.Total_PPN, a.Total_PPH, a.PurchID, a.SeqNo, a.DeliveryMethod, d.ReffTransID ";
                    Query += "From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID ";
                    Query += "LEFT JOIN dbo.[CanvasSheetD] c ON a.ReffId = c.CanvasID and a.ReffId2 = c.PurchQuotId and a.ReffSeqNo = c.CanvasSeqNo ";
                    Query += "LEFT JOIN dbo.[PurchRequisition_Dtl] d ON c.PurchReqId = d.PurchReqId and c.PurchReqSeqNo = d.SeqNo Where a.PurchId = '" + POID[i] + "' and a.SeqNo = '" + SeqNo[i] + "' ;";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            if (Dr["Unit"].ToString() == "KG" || Dr["Unit"].ToString() == "LBR")
                            {
                                UoM = Dr["UoM"].ToString();
                            }
                            else
                            {
                                UoM = Dr["Unit"].ToString();
                            }

                            this.dgvRODetails.Rows.Add((dgvRODetails.RowCount + 1).ToString(), Dr["FullItemID"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["ItemName"], "", "", UoM, "", Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Konv_Ratio"], "", Dr["Price"], Dr["Price_KG"], Dr["Total"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPH"], "", Dr["ReffTransId"], "", "", Dr["SeqNo"], Dr["DeliveryMethod"], Dr["PurchId"], ROSeqNo == 0 ? dgvRODetails.RowCount + 1 : ROSeqNo);
                            OldPORemaining.Add(Convert.ToDecimal(Dr["RemainingQty"]));
                            if (ROSeqNo != 0)
                            {
                                ROSeqNo++;
                            }
                        }
                        Dr.Close();
                    }
                    Conn.Close();
                }

                for (int i = 0; i < 27; i++)
                {
                    if (i == 7 || i == 15)
                    {
                        dgvRODetails.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvRODetails.Columns[i].ReadOnly = true;
                    }
                }

                dgvRODetails.Columns["GroupId"].Visible = false;
                dgvRODetails.Columns["SubGroup1Id"].Visible = false;
                dgvRODetails.Columns["SubGroup2Id"].Visible = false;
                dgvRODetails.Columns["ItemId"].Visible = false;
                dgvRODetails.Columns["PurchaseOrderSeqNo"].Visible = false;
                dgvRODetails.Columns["PurchaseOrderID"].Visible = false;
                dgvRODetails.Columns["Price"].Visible = false;
                dgvRODetails.Columns["Price_KG"].Visible = false;
                dgvRODetails.Columns["Total"].Visible = false;
                dgvRODetails.Columns["Diskon"].Visible = false;
                dgvRODetails.Columns["Total_Disk"].Visible = false;
                dgvRODetails.Columns["Total_PPN"].Visible = false;
                dgvRODetails.Columns["Total_PPH"].Visible = false;
                dgvRODetails.Columns["POEstDeliveryDate"].Visible = false;
                dgvRODetails.Columns["SONumber"].Visible = false;
                dgvRODetails.Columns["SOExpectedDateFrom"].Visible = false;
                dgvRODetails.Columns["SOExpectedDateTo"].Visible = false;
                dgvRODetails.Columns["PurchaseOrderSeqNo"].Visible = false;
                dgvRODetails.Columns["DeliveryMethod"].Visible = false;

                dgvRODetails.AutoResizeColumns();
            }
        }

        public string getPOID()
        {
            string POID = "";

            if (dgvRODetails.RowCount > 0)
            {
                for (int i = 0; i <= dgvRODetails.RowCount - 1; i++)
                {
                    if (i == 0)
                    {
                        POID += "(PurchID <> '";
                        POID += dgvRODetails.Rows[i].Cells["PurchaseOrderID"].Value == null ? "" : dgvRODetails.Rows[i].Cells["PurchaseOrderID"].Value.ToString();
                        POID += "' or SeqNo <> '";
                        POID += dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value == null ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value.ToString());
                        POID += "')";
                    }
                    else
                    {
                        POID += " and (PurchID <> '";
                        POID += dgvRODetails.Rows[i].Cells["PurchaseOrderID"].Value == null ? "" : dgvRODetails.Rows[i].Cells["PurchaseOrderID"].Value.ToString();
                        POID += "' or SeqNo <> '";
                        POID += dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value == null ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value.ToString());
                        POID += "')";
                    }
                }

                return POID;
            }
            else
            {
                POID = "(PurchId <> '' or SeqNo <> '0')";
                return POID;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRODetails.RowCount > 0)
            {
                Index = dgvRODetails.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvRODetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvRODetails.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvRODetails.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string NumberGroupSeq = dgvRODetails.Rows[dgvRODetails.CurrentCell.RowIndex].Cells["PurchaseOrderSeqNo"].Value.ToString();
                    if (txtRoNumber.Text != "" && txtRoNumber.Text != null)
                    {
                        Query = "SELECT [Qty],[Unit],[Ratio],[Price] FROM [dbo].[ReceiptOrderD] WHERE [ReceiptOrderId] = '" + txtRoNumber.Text + "' AND [PurchaseOrderId]='" + txtPONumber.Text + "' AND [PurchaseOrderSeqNo] = " + dgvRODetails.Rows[dgvRODetails.CurrentCell.RowIndex].Cells["PurchaseOrderSeqNo"].Value + " AND [FullItemId] = '" + dgvRODetails.Rows[dgvRODetails.CurrentCell.RowIndex].Cells["FullItemID"].Value.ToString() + "' AND SeqNo = '" + dgvRODetails.Rows[dgvRODetails.CurrentCell.RowIndex].Cells["SeqNo"].Value + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    deletedItem.Add(dgvRODetails.Rows[dgvRODetails.CurrentCell.RowIndex].Cells["FullItemID"].Value.ToString());
                                    deletedQty.Add(Convert.ToDecimal(Dr["Qty"]));
                                    deletedPrice.Add(Convert.ToDecimal(Dr["Price_KG"]));
                                    deletedRatio.Add(Convert.ToDecimal(Dr["Ratio"]));
                                    deletedUnit.Add(Dr["Unit"].ToString());
                                }
                            }
                            Dr.Close();
                        }
                    }
                    dgvRODetails.Rows.RemoveAt(Index);
                    OldPORemaining.RemoveAt(Index);

                    SortNoDataGrid();
                }
                //GetGelombang();
            }
        }

        public void SetMode(string tmpMode, string tmpRONumber)
        {
            Mode = tmpMode;
            RONumber = tmpRONumber;
            txtRoNumber.Text = tmpRONumber;
        }

        private void btnEditH_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access

            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                //btnSave.Visible = true;
                //btnExit.Visible = false;
                //btnEdit.Visible = false;
                //btnCancel.Visible = true;
                //dtRoDate.Enabled = false;
                //txtPrStatus.Enabled = true;

                ModeEdit();
                //MethodReadOnlyRowBaseN();                
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void dgvPrDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvRODetails.Columns[dgvRODetails.CurrentCell.ColumnIndex].Name == "Qty")
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
                //hendry end

                //BY: HC (S)
                if (e.KeyChar != '\b')
                {
                    if ((sender as TextBox).Text.Length >= 15)
                        e.Handled = true;
                }
                //BY: HC (E)

            }
            //BY: HC (S)
            if (dgvRODetails.Columns[dgvRODetails.CurrentCell.ColumnIndex].Name == "Notes")
            {
                if (e.KeyChar != '\b')
                {
                    if ((sender as TextBox).Text.Length >= 255)
                        e.Handled = true;
                }
            }
            //BY: HC (E)
            if (dgvRODetails.Columns[dgvRODetails.CurrentCell.ColumnIndex].Name == "DeliveryMethod")
            {
                if (!char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

        }

        //public void AddDataGridDetail(string GroupId, string SubGroup1Id, string SubGroup2Id, string ItemId, string FullItemId, string ItemName, string GelombangId, string BracketId, string Base, string Price)
        //{
        //    if (dgvRODetails.RowCount - 1 <= 0)
        //    {
        //        dgvRODetails.ColumnCount = 21;
        //        dgvRODetails.Columns[0].Name = "No";
        //        dgvRODetails.Columns[1].Name = "FullItemID";
        //        dgvRODetails.Columns[2].Name = "ItemName";
        //        dgvRODetails.Columns[3].Name = "Qty";
        //        dgvRODetails.Columns[4].Name = "Unit";
        //        dgvRODetails.Columns[5].Name = "Base";
        //        dgvRODetails.Columns[6].Name = "Vendor"; //VendId
        //        dgvRODetails.Columns[7].Name = "DeliveryMethod";
        //        dgvRODetails.Columns[8].Name = "SalesSO"; //ReffTransID
        //        dgvRODetails.Columns[9].Name = "ExpectedDateFrom";
        //        dgvRODetails.Columns[10].Name = "ExpectedDateTo";;
        //        dgvRODetails.Columns[11].Name = "Deskripsi";
        //        dgvRODetails.Columns[12].Name = "GroupId";
        //        dgvRODetails.Columns[13].Name = "SubGroup1Id";
        //        dgvRODetails.Columns[14].Name = "SubGroup2Id";
        //        dgvRODetails.Columns[15].Name = "ItemId";
        //        dgvRODetails.Columns[16].Name = "GelombangId";
        //        dgvRODetails.Columns[17].Name = "BracketId";
        //        dgvRODetails.Columns[18].Name = "Price";
        //        dgvRODetails.Columns[19].Name = "SeqNoGroup";
        //        dgvRODetails.Columns[20].Name = "BracketDesc";
        //    }
        //    int SeqNoGroup = CheckSeqNoGroup();
        //    this.dgvRODetails.Rows.Add((dgvRODetails.RowCount + 1).ToString(), FullItemId, ItemName, "0", "", Base, "", "", "", "", "", "", GroupId, SubGroup1Id, SubGroup2Id, ItemId, GelombangId, BracketId, Price, SeqNoGroup);

        //        Conn = ConnectionString.GetConnection();
        //        Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + FullItemId + "' ";
        //        Cmd = new SqlCommand(Query, Conn);
        //        Dr = Cmd.ExecuteReader();
        //        DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
        //        while (Dr.Read())
        //        {
        //            combo.Items.Add(Dr[0].ToString());
        //            combo.Items.Add(Dr[1].ToString());
        //        }
        //        dgvRODetails.Rows[(dgvRODetails.Rows.Count - 1)].Cells[4] = combo;

        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Qty"].Style.BackColor = Color.LightPink;
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Vendor"].Style.BackColor = Color.LightYellow;
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["DeliveryMethod"].Style.BackColor = Color.LightYellow;
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["SalesSO"].Style.BackColor = Color.LightYellow;
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["ExpectedDateFrom"].Style.BackColor = Color.LightYellow;
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["ExpectedDateTo"].Style.BackColor = Color.LightYellow;
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Deskripsi"].Style.BackColor = Color.LightYellow;

        //        //Query = "Select DeliveryMethod From dbo.[DeliveryMethod]";
        //        //Cmd = new SqlCommand(Query, Conn);
        //        //SqlDataReader DrDeliveryMethod;
        //        //DrDeliveryMethod = Cmd.ExecuteReader();
        //        ////DataGridViewComboBoxCell DeliveryMethod = new DataGridViewComboBoxCell();
        //        //DeliveryMethod.Items.Clear();

        //        //while (DrDeliveryMethod.Read())
        //        //{
        //        //    DeliveryMethod.Items.Add(DrDeliveryMethod[0].ToString());
        //        //}
        //        //DeliveryMethod.ValueChanged += this.dtp_ValueChanged;
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[3] = DeliveryMethod;


        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[10].Value = "...";
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[11].Value = "...";
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[12].Value = "...";
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[13].Value = "...";

        //    dgvRODetails.ReadOnly = false;
        //    dgvRODetails.Columns["No"].ReadOnly = true;
        //    dgvRODetails.Columns["FullItemID"].ReadOnly = true;
        //    dgvRODetails.Columns["ItemName"].ReadOnly = true;
        //    dgvRODetails.Columns["SalesSO"].ReadOnly = true;
        //    dgvRODetails.Columns["Base"].ReadOnly = true;
        //    dgvRODetails.Columns["BracketDesc"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D1"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D2"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D3"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D4"].ReadOnly = true;

        //    dgvRODetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["SalesSO"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["ExpectedDateFrom"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["ExpectedDateTo"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["Vendor"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["GelombangId"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvRODetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    dgvRODetails.Columns["GroupId"].Visible = false;
        //    dgvRODetails.Columns["SubGroup1Id"].Visible = false;
        //    dgvRODetails.Columns["SubGroup2Id"].Visible = false;
        //    dgvRODetails.Columns["ItemId"].Visible = false;
        //    dgvRODetails.Columns["GelombangId"].Visible = false;
        //    dgvRODetails.Columns["BracketId"].Visible = false;
        //    dgvRODetails.Columns["Price"].Visible = false;
        //    dgvRODetails.Columns["SeqNoGroup"].Visible = false;


        //    //dgvPrDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    dgvRODetails.AutoResizeColumns();
        //    //InvStockDetail.Clear();
        //    //InvStockDetail = GelombangId;
        //}

        //Check SeqNoGroup yang belum digunakan.
        private int CheckSeqNoGroup()
        {
            for (int j = 1; j <= 1000000; j++)
            {
                for (int i = 0; i < dgvRODetails.RowCount; i++)
                {
                    if (Convert.ToInt32(dgvRODetails.Rows[i].Cells["SeqNoGroup"].Value) == j)
                    {
                        goto Outer;
                    }
                }
                return j;
            Outer:
                continue;
            }
            return 1000000;
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvRODetails.RowCount; i++)
            {
                dgvRODetails.Rows[i].Cells["No"].Value = i + 1;
                if (txtRoNumber.Text == "" || txtRoNumber.Text == null)
                {
                    dgvRODetails.Rows[i].Cells["SeqNo"].Value = dgvRODetails.Rows[i].Cells["No"].Value;
                }
            }
        }

        public void ModeNew()
        {
            RONumber = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            btnClose.Visible = false;
            dtRoDate.Enabled = false;
            txtPrStatus.Enabled = false;
            btnWarehouse.Enabled = true;
            btnPrint.Visible = false;

            //BY: HC (S)
            label12.Visible = false;
            cmbDelivMethod.Visible = false;
            //BY: HC (E)
        }

        public void ModeEdit()
        {
            Mode = "Edit";
            passDelivMethodValue();
            cmbDelivMethod.Enabled = true;//BY: HC

            btnSOwner.Enabled = true;
            cbVOwner.Enabled = true;

            btnClose.Visible = false;
            btnPrint.Visible = false;
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;


            btnNew.Enabled = true;
            btnDelete.Enabled = true;
            dtRoDate.Enabled = false;
            btnWarehouse.Enabled = true;
            dgvRODetails.AutoResizeColumns();

            dgvRODetails.ReadOnly = false;

            if (dgvRODetails.Columns.Count != 0)
            {
                if (tabDgvControl.SelectedTab.Text == "Detail RO")
                {
                    for (int i = 0; i < dgvRODetails.Columns.Count; i++)
                    {
                        if (dgvRODetails.Columns[i].HeaderText == "No" || dgvRODetails.Columns[i].HeaderText == "FullItemID" || dgvRODetails.Columns[i].HeaderText == "ItemName" || dgvRODetails.Columns[i].HeaderText == "RO Qty" || dgvRODetails.Columns[i].HeaderText == "Unit" || dgvRODetails.Columns[i].HeaderText == "Total Berat")
                        {
                            dgvRODetails.Columns[i].Visible = true;
                        }
                        else if (dgvRODetails.Columns[i].HeaderText == "PO Qty" || dgvRODetails.Columns[i].HeaderText == "PO Remaining" || dgvRODetails.Columns[i].HeaderText == "PO Unit" || dgvRODetails.Columns[i].HeaderText == "Ratio" || dgvRODetails.Columns[i].HeaderText == "Notes")
                        {
                            dgvRODetails.Columns[i].Visible = true;
                        }
                        else
                        {
                            dgvRODetails.Columns[i].Visible = false;
                        }

                        if (dgvRODetails.Columns[i].HeaderText == "RO Qty" || dgvRODetails.Columns[i].HeaderText == "Notes")
                        {
                            dgvRODetails.Columns[i].ReadOnly = false;
                        }
                        else
                        {
                            dgvRODetails.Columns[i].ReadOnly = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < dgvRODetails.Columns.Count; i++)
                    {
                        if (dgvRODetails.Columns[i].HeaderText == "No" || dgvRODetails.Columns[i].HeaderText == "FullItemID" || dgvRODetails.Columns[i].HeaderText == "ItemName")
                        {
                            dgvRODetails.Columns[i].Visible = true;
                        }
                        else if (dgvRODetails.Columns[i].HeaderText == "PO est. Delivery Date" || dgvRODetails.Columns[i].HeaderText == "SO Number" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateFrom" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateTo")
                        {
                            dgvRODetails.Columns[i].Visible = true;
                        }
                        else
                        {
                            dgvRODetails.Columns[i].Visible = false;
                        }

                        if (dgvRODetails.Columns[i].HeaderText == "PO est. Delivery Date" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateFrom" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateTo")
                        {
                            dgvRODetails.Columns[i].ReadOnly = false;
                        }
                        else
                        {
                            dgvRODetails.Columns[i].ReadOnly = true;
                        }

                    }
                }
            }

            txtVendName.Enabled = true;
            txtNotes.Enabled = true;
            txtDriverName.Enabled = true;
            txtVehicleNumber.Enabled = true;
            txtVehicleType.Enabled = true;

            dgvRODetails.DefaultCellStyle.BackColor = Color.White;

            //asdf
            ROSeqNo = Convert.ToInt32(dgvRODetails.Rows[dgvRODetails.Rows.Count - 1].Cells["No"].Value) + 1;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            Conn = ConnectionString.GetConnection();

            Query = "Select stClose, ReceiptOrderStatus from dbo.ReceiptOrderH where ReceiptOrderId = '" + txtRoNumber.Text + "' ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                string Status = Dr["StClose"].ToString();
                string ReceiptOrderStatus = Dr["ReceiptOrderStatus"].ToString();

                if (Status == "True")
                {
                    cbClose.Checked = true;
                    btnClose.Enabled = false;
                    BtnGunakan.Enabled = true;
                    btnEdit.Enabled = false;
                    btnPrint.Enabled = false;
                }
                else
                {
                    cbClose.Checked = false;
                    btnCancel.Enabled = true;
                    BtnGunakan.Enabled = false;
                }
                if (ReceiptOrderStatus == "01")
                {
                    btnEdit.Enabled = true;
                }
                else
                {
                    btnEdit.Enabled = false;
                }

            }
            Dr.Close();

            cmbDelivMethod.Enabled = false;//BY: HC

            btnSOwner.Enabled = false;
            cbVOwner.Enabled = false;

            btnClose.Visible = true;
            btnPrint.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnSearchPO.Enabled = false;
            btnWarehouse.Enabled = false;

            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            dtRoDate.Enabled = false;
            dtPODate.Enabled = false;
            dtDeliveryDate.Enabled = false;
            txtDriverName.Enabled = false;
            txtVendName.Enabled = false;
            txtVehicleType.Enabled = false;
            txtVehicleNumber.Enabled = false;
            txtPrStatus.Enabled = false;
            txtNotes.Enabled = false;
            //tia edit
            txtVendName.ContextMenu = vendid;
            txtVendID.ContextMenu = vendid;
            tbxVOwnerID.ContextMenu = vendid;
            tbxVOwner.ContextMenu = vendid;
            txtPONumber.ContextMenu = vendid;

            txtVendName.Enabled = true;
            txtVendID.Enabled = true;
            txtVendName.ReadOnly = true;
            txtVendID.ReadOnly = true;
            txtPONumber.ReadOnly = true;

            tbxVOwnerID.Enabled = true;
            tbxVOwnerID.ReadOnly = true;
            tbxVOwner.ReadOnly = true;
            tbxVOwner.Enabled = true;
            txtPONumber.Enabled = true;
            //tia end
            dgvRODetails.ReadOnly = true;

            dgvRODetails.DefaultCellStyle.BackColor = Color.LightGray;

        }

        public void ModeView()
        {
            Mode = "ModeView";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnNew.Visible = false;
            btnDelete.Visible = false;

            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            dtRoDate.Enabled = false;
            //cmbPrType.Enabled = false;
            txtPrStatus.Enabled = false;

            dgvRODetails.ReadOnly = true;

            //dgvRODetails.DefaultCellStyle.BackColor = Color.LightGray;
        }

        public void GetDataHeader()
        {
            OldPORemaining.Clear();

            if (RONumber == "")
            {
                RONumber = txtRoNumber.Text.Trim();
            }
            Conn = ConnectionString.GetConnection();

            Query = "Select [ReceiptOrderId],[ReceiptOrderDate],[SOExpectedDate],[ReceiptOrderStatus],PurchaseOrderID,PurchaseOrderDate, a.VendId,VendorName,InventSiteID,InventSiteName,InventSiteLocation,[DriverName],[DriverOwnerSameAsAccountNum],VendorEkspedisi,[VehicleType],[VehicleNumber],[DeliveryDate],[Notes],b.VendName, a.DeliveryMethod From [ReceiptOrderH] a left join VendTable b on a.VendorEkspedisi=b.VendId ";
            Query += "Where ReceiptOrderId = '" + RONumber + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtRoNumber.Text = Dr["ReceiptOrderId"].ToString();
                dtRoDate.Text = Dr["ReceiptOrderDate"].ToString();

                dtPODate.Text = Dr["PurchaseOrderDate"].ToString();
                txtVendID.Text = Dr["VendID"].ToString();
                txtVendName.Text = Dr["VendorName"].ToString();
                txtPONumber.Text = Dr["PurchaseOrderID"].ToString();
                dtDeliveryDate.Text = Dr["DeliveryDate"].ToString();
                txtDriverName.Text = Dr["DriverName"].ToString();
                Boolean TmpDriverOwnerSameAsAccountNum = false;
                if (Dr["DriverOwnerSameAsAccountNum"] != null)
                    cbVOwner.Checked = Convert.ToBoolean(Dr["DriverOwnerSameAsAccountNum"]);
                tbxVOwnerID.Text = Dr["VendorEkspedisi"].ToString();
                tbxVOwner.Text = Dr["VendName"].ToString();
                txtVehicleType.Text = Dr["VehicleType"].ToString();
                txtVehicleNumber.Text = Dr["VehicleNumber"].ToString();
                txtInventSiteID.Text = Dr["InventSiteID"].ToString();
                txtWarehouse.Text = Dr["InventSiteName"].ToString();
                txtLocation.Text = Dr["InventSiteLocation"].ToString();
                txtNotes.Text = Dr["Notes"].ToString();
                cmbDelivMethod.Text = Dr["DeliveryMethod"].ToString();
            }
            Dr.Close();

            dgvRODetails.Rows.Clear();
            dgvRODetails.Columns.Clear();
            if (dgvRODetails.RowCount - 1 <= 0)
            {
                dgvRODetails.ColumnCount = 31;
                dgvRODetails.Columns[0].Name = "No";
                dgvRODetails.Columns[1].Name = "FullItemID";
                dgvRODetails.Columns[2].Name = "GroupId";
                dgvRODetails.Columns[3].Name = "SubGroup1Id";
                dgvRODetails.Columns[4].Name = "SubGroup2Id";
                dgvRODetails.Columns[5].Name = "ItemId";
                dgvRODetails.Columns[6].Name = "ItemName";
                dgvRODetails.Columns[7].Name = "Qty"; dgvRODetails.Columns[7].HeaderText = "RO Qty";
                dgvRODetails.Columns[8].Name = "RORemaining"; dgvRODetails.Columns[8].HeaderText = "RO Remaining";
                dgvRODetails.Columns[9].Name = "Unit";
                dgvRODetails.Columns[10].Name = "Qty_Kg"; dgvRODetails.Columns[10].HeaderText = "Total Berat";
                dgvRODetails.Columns[11].Name = "POQty"; dgvRODetails.Columns[11].HeaderText = "PO Qty";
                dgvRODetails.Columns[12].Name = "PORemaining"; dgvRODetails.Columns[12].HeaderText = "PO Remaining";
                dgvRODetails.Columns[13].Name = "POUnit"; dgvRODetails.Columns[13].HeaderText = "PO Unit";
                dgvRODetails.Columns[14].Name = "Ratio";
                dgvRODetails.Columns[15].Name = "Notes";
                dgvRODetails.Columns[16].Name = "Price";
                dgvRODetails.Columns[17].Name = "Price_KG";
                dgvRODetails.Columns[18].Name = "Total";
                dgvRODetails.Columns[19].Name = "Diskon";
                dgvRODetails.Columns[20].Name = "Total_Disk"; dgvRODetails.Columns[20].HeaderText = "Total Diskon";
                dgvRODetails.Columns[21].Name = "Total_PPN"; dgvRODetails.Columns[21].HeaderText = "Total PPN";
                dgvRODetails.Columns[22].Name = "Total_PPH"; dgvRODetails.Columns[22].HeaderText = "Total PPH";
                dgvRODetails.Columns[23].Name = "POEstDeliveryDate"; dgvRODetails.Columns[23].HeaderText = "PO est. Delivery Date";
                dgvRODetails.Columns[24].Name = "SONumber"; dgvRODetails.Columns[24].HeaderText = "SO Number";
                dgvRODetails.Columns[25].Name = "SOExpectedDateFrom";
                dgvRODetails.Columns[26].Name = "SOExpectedDateTo";
                dgvRODetails.Columns[27].Name = "PurchaseOrderSeqNo";
                dgvRODetails.Columns[28].Name = "DeliveryMethod";
                dgvRODetails.Columns[29].Name = "PurchaseOrderID";
                dgvRODetails.Columns[30].Name = "SeqNo";
            }

            Query = "Select a.FullItemId, a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.ItemName, b.Qty as poqty, b.RemainingQty, b.Unit as pounit, a.Qty as roqty, a.RemainingQty as RORemaining, a.Unit as rounit, a.Qty_KG, a.Ratio, a.Deskripsi, a.Price, a.Price_KG, a.Total, a.Diskon, a.Total_Disk, a.Total_PPN, a.Total_PPH, a.PurchaseOrder_EstDeliveryDate, a.SalesOrderNo, a.SalesExpectedDateFrom, a.SalesExpectedDateTo, a.PurchaseOrderSeqNo, a.DeliveryMethod, a.SalesOrderNo, a.PurchaseOrderId,a.SeqNo from ReceiptOrderH ROH left join [dbo].[ReceiptOrderD] a on ROH.ReceiptOrderId = a.ReceiptOrderId JOIN [dbo].[PurchDtl] b ON (a.PurchaseOrderId = b.PurchID and a.PurchaseOrderSeqNo = b.SeqNo) Where a.ReceiptOrderID = '" + txtRoNumber.Text + "' and ROH.DeliveryMethod = '" + cmbDelivMethod.Text + "' ";
            Conn = ConnectionString.GetConnection();

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                String POQty = Dr["poqty"].ToString();
                String ROQty = Dr["roqty"].ToString();
                String POUnit = Dr["pounit"].ToString();
                String ROUnit = Dr["rounit"].ToString();
                String RORemaining = Dr["RORemaining"].ToString();

                this.dgvRODetails.Rows.Add((dgvRODetails.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["GroupID"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["ItemName"], ROQty, RORemaining, ROUnit, Dr["Qty_KG"], POQty, Dr["RemainingQty"], POUnit, Dr["Ratio"], Dr["Deskripsi"], Dr["Price"], Dr["Price_KG"], Dr["Total"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPH"], Dr["PurchaseOrder_EstDeliveryDate"], Dr["SalesOrderNo"], Dr["SalesExpectedDateFrom"], Dr["SalesExpectedDateTo"], Dr["PurchaseOrderSeqNo"], Dr["DeliveryMethod"], Dr["PurchaseOrderId"], Dr["SeqNo"]);
                OldPORemaining.Add(Convert.ToDecimal(Dr["RemainingQty"]));
            }
            Dr.Close();

            for (int i = 0; i < 31; i++)
            {
                dgvRODetails.Columns[i].ReadOnly = true;
            }

            dgvRODetails.Columns["RORemaining"].Visible = false;
            dgvRODetails.Columns["GroupId"].Visible = false;
            dgvRODetails.Columns["SubGroup1Id"].Visible = false;
            dgvRODetails.Columns["SubGroup2Id"].Visible = false;
            dgvRODetails.Columns["ItemId"].Visible = false;
            dgvRODetails.Columns["PurchaseOrderSeqNo"].Visible = false;
            dgvRODetails.Columns["PurchaseOrderID"].Visible = false;

            dgvRODetails.Columns["Price"].Visible = false;
            dgvRODetails.Columns["Price_KG"].Visible = false;
            dgvRODetails.Columns["Total"].Visible = false;
            dgvRODetails.Columns["Diskon"].Visible = false;
            dgvRODetails.Columns["Total_Disk"].Visible = false;
            dgvRODetails.Columns["Total_PPN"].Visible = false;
            dgvRODetails.Columns["Total_PPH"].Visible = false;
            dgvRODetails.Columns["POEstDeliveryDate"].Visible = false;
            dgvRODetails.Columns["SONumber"].Visible = false;
            dgvRODetails.Columns["SOExpectedDateFrom"].Visible = false;
            dgvRODetails.Columns["SOExpectedDateTo"].Visible = false;
            dgvRODetails.Columns["PurchaseOrderSeqNo"].Visible = false;
            dgvRODetails.Columns["DeliveryMethod"].Visible = false;


            dgvRODetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRODetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRODetails.Columns["FullItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRODetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRODetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRODetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRODetails.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvRODetails.Columns["POEstDeliverydate"].DefaultCellStyle.Format = "dd-MM-yyyy";
            dgvRODetails.Columns["SOExpectedDateFrom"].DefaultCellStyle.Format = "dd-MM-yyyy";
            dgvRODetails.Columns["SOExpectedDateTo"].DefaultCellStyle.Format = "dd-MM-yyyy";

            dgvRODetails.AutoResizeColumns();
        }

        private bool validation()
        {
            string msg = "";
            decimal combinedQty = 0;
            decimal RemainingPOQty = 0;
            List<int> checkedRow = new List<int>();



            if (dgvRODetails.RowCount == 0)
            {
                msg += "Jumlah item tidak boleh kosong.\r\n";
            }
            if (txtWarehouse.Text.Trim() == "")
            {
                msg += "Warehouse harus dipilih.\r\n";
            }
            if (tbxVOwner.Text == "")
            {
                msg += "Vendor Ekspedisi harus dipilih.\r\n";
            }
            //Validasi Qty tidak boleh 0
            for (int i = 0; i < dgvRODetails.RowCount; i++)
            {
                if (Convert.ToDouble(dgvRODetails.Rows[i].Cells["Qty"].Value) <= 0)
                {
                    msg += "Item baris ke-" + (i + 1) + " tidak boleh lebih kecil atau sama dengan 0.\r\n";
                }
            }
            for (int i = 0; i < dgvRODetails.Rows.Count; i++)
            {
                Decimal NewPORemaining = Convert.ToDecimal(dgvRODetails.Rows[i].Cells["PORemaining"].Value == "" ? "0" : dgvRODetails.Rows[i].Cells["PORemaining"].Value.ToString());

                if (NewPORemaining < 0)
                {
                    msg += "Item No = " + dgvRODetails.Rows[i].Cells["No"].Value + ", PO Remaining tidak boleh lebih kecil dari 0\r\n";
                }

                if (checkedRow.Contains(i))
                {
                    continue;
                }

                combinedQty = Convert.ToDecimal(dgvRODetails.Rows[i].Cells["Qty"].Value);
                RemainingPOQty = 0;

                Query = "SELECT [RemainingQty] FROM [dbo].[PurchDtl] WHERE [PurchID] = '" + txtPONumber.Text + "' AND [FullItemID] = '" + dgvRODetails.Rows[i].Cells["FullItemID"].Value.ToString() + "' AND [SeqNo] = " + dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value + " ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    if (dgvRODetails.Rows[i].Cells["POUnit"].Value.ToString() != dgvRODetails.Rows[i].Cells["Unit"].Value.ToString())
                    {
                        if (dgvRODetails.Rows[i].Cells["Unit"].Value.ToString() == "KG")
                        {
                            RemainingPOQty = (Convert.ToDecimal(Cmd.ExecuteScalar()) * Convert.ToDecimal(dgvRODetails.Rows[i].Cells["Ratio"].Value));
                        }
                        else
                        {
                            RemainingPOQty = (Convert.ToDecimal(Cmd.ExecuteScalar()) / Convert.ToDecimal(dgvRODetails.Rows[i].Cells["Ratio"].Value));
                        }
                    }
                    else
                    {
                        RemainingPOQty = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }
                }
                Query = "SELECT [Qty] FROM [dbo].[ReceiptOrderD] WHERE [ReceiptOrderId] = '" + txtRoNumber.Text + "' AND [PurchaseOrderId] = '" + txtPONumber.Text + "' AND [PurchaseOrderSeqNo] = " + dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value + "";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            RemainingPOQty += Convert.ToDecimal(Dr["Qty"]);
                        }
                    }
                    Dr.Close();
                }

                for (int x = 0; x < dgvRODetails.Rows.Count; x++)
                {
                    if (checkedRow.Contains(x) || x <= i)
                    {
                        continue;
                    }
                    if ((dgvRODetails.Rows[i].Cells["FullItemID"].Value.ToString() == dgvRODetails.Rows[x].Cells["FullItemID"].Value.ToString()) && (Convert.ToInt32(dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value) == Convert.ToInt32(dgvRODetails.Rows[x].Cells["PurchaseOrderSeqNo"].Value)))
                    {
                        combinedQty += Convert.ToDecimal(dgvRODetails.Rows[x].Cells["Qty"].Value);
                        if (combinedQty > RemainingPOQty)
                        {
                            checkedRow.Add(x);
                            msg += "- Combined Qty Row " + (i + 1).ToString() + " " + (x + 1).ToString() + " dengan FullItemID " + dgvRODetails.Rows[i].Cells["FullItemID"].Value.ToString() + " melebihi Remaining PO.\n";
                            break;
                        }
                    }
                }
            }
            if (msg == "")
            {
                return true;
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(this, msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            String RONumber = txtRoNumber.Text;
            DateTime CreatedDate = DateTime.Now; ;
            string CreatedBy = "";
            decimal QtyOld = 0;
            if (validation() == false)
            {
                return;
            }
            if (Mode == "New" || txtRoNumber.Text == "")
            {

                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    //begin============================================================================================
                    //updated by : joshua
                    //updated date : 14 Feb 2018
                    //description : change generate sequence number, get from global function and update counter 
                    string Jenis = "RO", Kode = "RO";
                    RONumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                    Query = "Insert into [dbo].[ReceiptOrderH] (ReceiptOrderID,ReceiptOrderDate,ReceiptOrderStatus,PurchaseOrderDate,PurchaseOrderID,VendID,VendorName,DeliveryDate,InventSiteID,InventSiteName,InventSiteLocation,DriverOwnerSameAsAccountNum,VendorEkspedisi,VehicleType,VehicleNumber,DriverName,Notes,DeliveryMethod,CreatedDate,";
                    Query += "CreatedBy) Output (Inserted.ReceiptOrderID) values (";
                    Query += "'" + RONumber + "',";
                    Query += "'" + dtRoDate.Value.Date + "',";
                    Query += "'01',";
                    Query += "'" + dtPODate.Value.Date + "',";
                    Query += "'" + txtPONumber.Text + "',";
                    Query += "'" + txtVendID.Text + "',";
                    Query += "'" + txtVendName.Text + "',";
                    Query += "'" + dtDeliveryDate.Value.Date + "',";
                    Query += "'" + txtInventSiteID.Text + "',";
                    Query += "'" + txtWarehouse.Text + "',";
                    Query += "'" + txtLocation.Text + "',";
                    Query += "'" + Convert.ToBoolean(cbVOwner.Checked) + "',";
                    Query += "'" + tbxVOwnerID.Text + "',";
                    Query += "@VehicleType,";
                    Query += "@VehicleNumber,";
                    Query += "@DriverName,";
                    Query += "@Notes,";
                    Query += "'" + cmbDelivMethod.Text + "',";
                    Query += "getdate(),";
                    Query += "'" + ControlMgr.UserId + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                    Cmd.Parameters.AddWithValue("@DriverName", txtDriverName.Text.Trim());
                    Cmd.Parameters.AddWithValue("@VehicleNumber", txtVehicleNumber.Text.Trim());
                    Cmd.Parameters.AddWithValue("@VehicleType", txtVehicleType.Text.Trim());
                    Cmd.ExecuteNonQuery();

                    //update counter
                    //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                    //end update counter
                    //end=============================================================================================

                    //created by Thaddaeus Matthias, 27 Sept 2018
                    //inserting to status log
                    //=====================================begin========================================
                    ListMethod.StatusLogVendor("HeaderReceiptOrder", "RO", txtVendID.Text, "01", "", RONumber, txtPONumber.Text, txtInventSiteID.Text, "");
                    //======================================end=========================================
                    decimal TmpPORemainingTotal = 0;
                    if (dgvRODetails.Rows.Count > 0)
                    {
                        OldPORemaining.Clear();

                        for (int i = 0; i <= dgvRODetails.Rows.Count - 1; i++)
                        {
                            if (dgvRODetails.Rows[i].Cells["POEstDeliveryDate"].Value == null || dgvRODetails.Rows[i].Cells["POEstDeliveryDate"].Value == "")
                                dgvRODetails.Rows[i].Cells["POEstDeliveryDate"].Value = "01-01-1900";
                            if (dgvRODetails.Rows[i].Cells["SOExpectedDateFrom"].Value == null || dgvRODetails.Rows[i].Cells["SOExpectedDateFrom"].Value == "")
                                dgvRODetails.Rows[i].Cells["SOExpectedDateFrom"].Value = "01-01-1900";
                            if (dgvRODetails.Rows[i].Cells["SOExpectedDateTo"].Value == null || dgvRODetails.Rows[i].Cells["SOExpectedDateTo"].Value == "")
                                dgvRODetails.Rows[i].Cells["SOExpectedDateTo"].Value = "01-01-1900";

                            #region variable
                            String PurchaseOrderSeqNo = dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value == null ? "" : dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value.ToString();
                            String SeqNo = dgvRODetails.Rows[i].Cells["No"].Value == null ? "" : dgvRODetails.Rows[i].Cells["No"].Value.ToString();
                            String GroupId = dgvRODetails.Rows[i].Cells["GroupId"].Value == null ? "" : dgvRODetails.Rows[i].Cells["GroupId"].Value.ToString();
                            String SubGroup1Id = dgvRODetails.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvRODetails.Rows[i].Cells["SubGroup1Id"].Value.ToString();
                            String SubGroup2Id = dgvRODetails.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvRODetails.Rows[i].Cells["SubGroup2Id"].Value.ToString();
                            String ItemId = dgvRODetails.Rows[i].Cells["ItemId"].Value == null ? "" : dgvRODetails.Rows[i].Cells["ItemId"].Value.ToString();
                            String FullItemId = dgvRODetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvRODetails.Rows[i].Cells["FullItemId"].Value.ToString();
                            String ItemName = dgvRODetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvRODetails.Rows[i].Cells["ItemName"].Value.ToString();
                            decimal Qty = dgvRODetails.Rows[i].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString());
                            decimal QtyKG = dgvRODetails.Rows[i].Cells["Qty_KG"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Qty_KG"].Value.ToString());
                            String Unit = dgvRODetails.Rows[i].Cells["Unit"].Value == "" ? "" : dgvRODetails.Rows[i].Cells["Unit"].Value.ToString();
                            String Notes = dgvRODetails.Rows[i].Cells["Notes"].Value == null ? "" : dgvRODetails.Rows[i].Cells["Notes"].Value.ToString().Trim();
                            decimal Ratio = dgvRODetails.Rows[i].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Ratio"].Value.ToString());
                            decimal Price = dgvRODetails.Rows[i].Cells["Price"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Price"].Value.ToString());
                            decimal PriceKG = dgvRODetails.Rows[i].Cells["Price_KG"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Price_KG"].Value.ToString());
                            decimal Total = dgvRODetails.Rows[i].Cells["Total"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Total"].Value.ToString());
                            decimal Diskon = dgvRODetails.Rows[i].Cells["Diskon"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Diskon"].Value.ToString());
                            decimal Total_Disk = dgvRODetails.Rows[i].Cells["Total_Disk"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Total_Disk"].Value.ToString());
                            decimal Total_PPN = dgvRODetails.Rows[i].Cells["Total_PPN"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Total_PPN"].Value.ToString());
                            decimal Total_PPH = dgvRODetails.Rows[i].Cells["Total_PPH"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Total_PPH"].Value.ToString());
                            //REMARKED BY: HC (S)
                            //String POEstDeliveryDate = dgvRODetails.Rows[i].Cells["POEstDeliveryDate"].Value == null ? "" : FormateDateddmmyyyy(dgvRODetails.Rows[i].Cells["POEstDeliveryDate"].Value.ToString());
                            //String SOExpectedDateFrom = dgvRODetails.Rows[i].Cells["SOExpectedDateFrom"].Value == null ? "" : FormateDateddmmyyyy(dgvRODetails.Rows[i].Cells["SOExpectedDateFrom"].Value.ToString());
                            //String SOExpectedDateTo = dgvRODetails.Rows[i].Cells["SOExpectedDateTo"].Value == null ? "" : FormateDateddmmyyyy(dgvRODetails.Rows[i].Cells["SOExpectedDateTo"].Value.ToString());
                            //REMARKED BY: HC (E)
                            //BY: HC (S)
                            DateTime POEstDeliveryDate = dgvRODetails.Rows[i].Cells["POEstDeliveryDate"].Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dgvRODetails.Rows[i].Cells["POEstDeliveryDate"].Value);
                            DateTime SOExpectedDateFrom = dgvRODetails.Rows[i].Cells["SOExpectedDateFrom"].Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dgvRODetails.Rows[i].Cells["SOExpectedDateFrom"].Value);
                            DateTime SOExpectedDateTo = dgvRODetails.Rows[i].Cells["SOExpectedDateTo"].Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dgvRODetails.Rows[i].Cells["SOExpectedDateTo"].Value);
                            //BY: HC (E)
                            String SONumber = dgvRODetails.Rows[i].Cells["SONumber"].Value == null ? "" : dgvRODetails.Rows[i].Cells["SONumber"].Value.ToString();
                            String DeliveryMethod = dgvRODetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvRODetails.Rows[i].Cells["DeliveryMethod"].Value.ToString();
                            String POUnit = dgvRODetails.Rows[i].Cells["POUnit"].Value == "" ? "" : dgvRODetails.Rows[i].Cells["POUnit"].Value.ToString();
                            String PORemaining = dgvRODetails.Rows[i].Cells["PORemaining"].Value == "" ? "" : dgvRODetails.Rows[i].Cells["PORemaining"].Value.ToString();
                            #endregion
                            OldPORemaining.Add(Convert.ToDecimal(PORemaining));

                            Query = "Insert into [dbo].[ReceiptOrderD] (ReceiptOrderID, PurchaseOrder_EstDeliveryDate, PurchaseOrderID, PurchaseOrderSeqNo, SalesExpectedDateFrom, SalesExpectedDateTo, SalesOrderNo, SeqNo,";
                            Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_KG, RemainingQty, Unit, Ratio, Price, Price_KG, Total, Diskon, Total_Disk, Total_PPN, Total_PPH, InventSiteId, Deskripsi, DeliveryMethod, CreatedDate, CreatedBy) ";
                            Query += "values ('" + RONumber + "',  '" + POEstDeliveryDate + "', '" + txtPONumber.Text + "', '" + PurchaseOrderSeqNo + "', '" + SOExpectedDateFrom + "', '" + SOExpectedDateTo + "', '" + SONumber + "', '" + SeqNo + "', ";
                            Query += "'" + GroupId + "','" + SubGroup1Id + "','" + SubGroup2Id + "','" + ItemId + "','" + FullItemId + "', '" + ItemName + "', '" + Qty + "', '" + QtyKG + "', '" + Qty + "', '" + Unit + "', '" + Ratio + "', '" + Price + "', '" + PriceKG + "', '" + Total + "', '" + Diskon + "', ";
                            Query += "'" + Total_Disk + "', '" + Total_PPN + "', '" + Total_PPH + "', '" + txtInventSiteID.Text + "',@Notes, '" + DeliveryMethod + "',getdate(), '" + ControlMgr.UserId + "');";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.Parameters.AddWithValue("@Notes", Notes);
                            Cmd.ExecuteNonQuery();

                            TmpPORemainingTotal += Convert.ToDecimal(PORemaining);
                            //updatePO
                            //Query = "Update [dbo].[PurchDtl] set RemainingQty = (RemainingQty - " + Qty + ") where PurchID = '" + txtPONumber.Text + "' and SeqNo = '" + PurchaseOrderSeqNo + "'";
                            //Cmd = new SqlCommand(Query, Conn, Trans);
                            //Cmd.ExecuteNonQuery();
                            //decimal NewRemainingQty = 0;
                            //if (POUnit == Unit)
                            //{
                            //    NewRemainingQty = Convert.ToDecimal(PORemaining) - Convert.ToDecimal(Qty);
                            //}
                            //else {
                            //    if (POUnit.ToUpper() == "KG")
                            //    {
                            //        NewRemainingQty = Convert.ToDecimal(PORemaining) - (Convert.ToDecimal(Qty) * Ratio);
                            //    }
                            //    else {
                            //        NewRemainingQty = Convert.ToDecimal(PORemaining) - (Convert.ToDecimal(Qty) / Ratio);
                            //    }
                            //}

                            Query = "Update [dbo].[PurchDtl] set RemainingQty = " + PORemaining + " where PurchID = '" + txtPONumber.Text + "' and SeqNo = '" + PurchaseOrderSeqNo + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            //Update Invent_Purchase_Qty RO
                            decimal QtyInput;
                            decimal ConvRatio;
                            decimal QtyUoM;
                            decimal QtyAlt;
                            decimal QtyAmt;
                            decimal QtyPRIssued_UoM = 0;
                            decimal QtyPRIssued_Alt = 0;
                            decimal QtyPRIssued_Amt = 0;

                            FullItemId = dgvRODetails.Rows[i].Cells["FullItemID"].Value.ToString();
                            QtyInput = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString());
                            Unit = dgvRODetails.Rows[i].Cells["Unit"].Value.ToString();
                            ConvRatio = 0;

                            Query = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            UoM = Cmd.ExecuteScalar().ToString();

                            Query = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            ConvRatio = (decimal)Cmd.ExecuteScalar();

                            if (Unit == UoM && Unit != "KG")
                            {
                                //QtyUoM = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString());
                                //QtyAlt = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString()) * ConvRatio;

                                QtyUoM = QtyInput;
                                QtyAlt = QtyInput * ConvRatio;
                            }
                            else if (Unit == "KG")
                            {
                                //QtyAlt = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString());
                                //QtyUoM = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString()) / ConvRatio;

                                QtyAlt = QtyInput;
                                QtyUoM = QtyInput / ConvRatio;
                            }
                            else
                            {
                                MessageBox.Show("Unit Measurement " + Unit + " untuk FullItemId = " + FullItemId + " tidak terdaftar pada InventTable.");
                                return;
                            }
                            QtyAmt = QtyUoM * decimal.Parse(dgvRODetails.Rows[i].Cells["Price"].Value.ToString());
                            if (dgvRODetails.Rows[i].Cells["POUnit"].Value.ToString() == "KG")
                            {
                                QtyAmt = QtyAlt * decimal.Parse(dgvRODetails.Rows[i].Cells["Price_KG"].Value.ToString());
                            }

                            //Edit save to PurchRequisition_LogTable
                            Query = "Insert into [ReceiptOrder_LogTable] ([ReceiptOrderDate],[ReceiptOrderNo],[PurchaseOrderNo],[PurchaseOrderDate],[VendorID],[InventSiteID],[FullItemId],[SeqNo],[Qty_UoM],[Qty_Alt],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) ";
                            Query += "VALUES ('" + dtRoDate.Value.ToString("yyyy-MM-dd") + "', '" + RONumber + "', '" + txtPONumber.Text.Trim() + "', '" + dtPODate.Value.ToString("yyyy-MM-dd") + "', '" + txtVendID.Text.Trim() + "','" + txtInventSiteID.Text.Trim() + "', '" + dgvRODetails.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["No"].Value.ToString() + "','" + QtyUoM + "', '" + QtyAlt + "', '01' ,'Created' ,'Created " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate())";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            //Edit save to PurchRequisition_LogTable

                            Query = "Select RO_Issued_UoM, RO_Issued_Alt,[RO_Issued_Amount]  From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    QtyPRIssued_UoM = decimal.Parse(Dr["RO_Issued_UoM"].ToString());
                                    QtyPRIssued_Alt = decimal.Parse(Dr["RO_Issued_Alt"].ToString());
                                    QtyPRIssued_Amt = decimal.Parse(Dr["RO_Issued_Amount"].ToString());
                                }
                            }
                            else
                            {
                                Cmd = new SqlCommand("INSERT INTO Invent_Purchase_Qty ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemId] ,[ItemName]) VALUES ( '" + dgvRODetails.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["ItemName"].Value.ToString() + "')", Conn);
                                Cmd.ExecuteNonQuery();
                            }

                            Dr.Close();

                            Query = "Update Invent_Purchase_Qty Set RO_Issued_UoM += " + QtyUoM + ", RO_Issued_Alt += " + QtyAlt + ",RO_Issued_Amount +=" + QtyAmt + ",PO_Issued_Outstanding_UoM-=" + QtyUoM + ", [PO_Issued_Outstanding_Alt] -=" + QtyAlt + ", [PO_Issued_Outstanding_Amount] -= " + QtyAmt + " Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            //Update Invent_Purchase_Qty RO



                            //if (Unit == "KG" || Unit == "LBR")
                            //{
                            //    Decimal QtyAlt = Qty / Ratio;
                            //    Query = "Update Invent_Purchase_Qty set RO_Issued_UoM = (RO_Issued_UoM + " + Qty + "),RO_Issued_Alt = (RO_Issued_Alt + " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ;";
                            //}
                            //else
                            //{
                            //    Decimal QtyAlt = Qty * Ratio;
                            //    Query = "Update Invent_Purchase_Qty set RO_Issued_UoM = (RO_Issued_UoM + " + Qty + "),RO_Issued_Alt = (RO_Issued_Alt + " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ;";
                            //}

                            ////Update Invent_Purchase_Qty PO
                            //if (Unit == POUnit)
                            //{
                            //    Decimal QtyAlt = Qty;
                            //    Query += "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM - " + Qty + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt - " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ";
                            //}
                            //else
                            //{
                            //    if (POUnit == "KG" || POUnit == "LBR")
                            //    {
                            //        Decimal QtyUoM = Qty * Ratio;
                            //        Query += "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM - " + QtyUoM + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt - " + Qty + ") where FullItemID = '" + FullItemId + "' ";
                            //    }
                            //    else
                            //    {
                            //        Decimal QtyUoM = Qty / Ratio;
                            //        Query += "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM - " + QtyUoM + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt - " + Qty + ") where FullItemID = '" + FullItemId + "' ";
                            //    }
                            //}
                            //Cmd = new SqlCommand(Query, Conn, Trans);
                            //Cmd.ExecuteNonQuery();
                        }
                        //if (TmpPORemainingTotal <= 1)
                        //{
                        //    Query = "Update [dbo].[PurchH] set TransStatus = '07', StClose='1' where PurchID = '" + txtPONumber.Text + "'";
                        //    Cmd = new SqlCommand(Query, Conn, Trans);
                        //    Cmd.ExecuteNonQuery();
                        //}
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
                MessageBox.Show("Data :" + RONumber + " Berhasil ditambahkan.");
                txtRoNumber.Text = RONumber;
                MainMenu f = new MainMenu();
                f.refreshTaskList();
                Parent.RefreshGrid();
                GetDataHeader();
                ModeBeforeEdit();
            }
            else
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    //return deleted item Qty to inventpurchase
                    if (deletedItem.Count > 0 && deletedQty.Count > 0)
                    {
                        decimal delQty = 0;
                        decimal delQtyAlt = 0;
                        decimal delQtyAmt = 0;
                        for (int i = 0; i < deletedItem.Count; i++)
                        {
                            if (deletedUnit[i].ToString() == "KG")
                            {
                                delQty = deletedQty[i] / deletedRatio[i];
                                delQtyAlt = deletedQty[i];
                                delQtyAmt = delQtyAlt * deletedPrice[i];
                            }
                            else
                            {
                                delQty = deletedQty[i];
                                delQtyAlt = deletedQty[i] * deletedRatio[i];
                                delQtyAmt = delQtyAlt * deletedPrice[i];
                            }
                            Query = "UPDATE [dbo].[Invent_Purchase_Qty] SET [PO_Issued_Outstanding_UoM]+=" + delQty + ",[PO_Issued_Outstanding_Alt]+=" + delQtyAlt + ",[PO_Issued_Outstanding_Amount]+=" + delQtyAmt + ",[RO_Issued_UoM]-=" + delQty + ",[RO_Issued_Alt]-=" + delQtyAlt + ",[RO_Issued_Amount]-=" + delQtyAmt + " WHERE [FullItemID] = '" + deletedItem[i] + "'";
                            using (Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        deletedQty.Clear();
                        deletedPrice.Clear();
                        deletedItem.Clear();
                        deletedRatio.Clear();
                        deletedUnit.Clear();
                    }

                    Query = "Delete From ReceiptOrder_LogTable where ReceiptOrderNo='" + txtRoNumber.Text + "';";
                    Query += "Update [dbo].[ReceiptOrderH] set ReceiptOrderDate = '" + dtRoDate.Value.Date + "' ,VendID = '" + txtVendID.Text + "', VendorName = '" + txtVendName.Text + "', InventSiteID ='" + txtInventSiteID.Text + "', InventSiteName ='" + txtWarehouse.Text + "', InventSiteLocation = '" + txtLocation.Text + "', DriverOwnerSameAsAccountNum = '" + Convert.ToBoolean(cbVOwner.Checked) + "', VendorEkspedisi = '" + tbxVOwnerID.Text + "', VehicleType = @VehicleType, VehicleNumber = @VehicleNumber, DriverName = @DriverName, DeliveryDate = '" + dtDeliveryDate.Value.Date + "', PurchaseOrderID = '" + txtPONumber.Text + "', PurchaseOrderDate = '" + dtPODate.Value.Date + "',  Notes = @Notes, UpdatedDate = getDate(), UpdatedBy = '" + ControlMgr.UserId + "', DeliveryMethod = '" + cmbDelivMethod.Text + "' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy where ReceiptOrderID = '" + txtRoNumber.Text + "' ";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                    Cmd.Parameters.AddWithValue("@VehicleType", txtVehicleType.Text.Trim());
                    Cmd.Parameters.AddWithValue("@VehicleNumber", txtVehicleNumber.Text.Trim());
                    Cmd.Parameters.AddWithValue("@DriverName", txtDriverName.Text.Trim());
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                        CreatedBy = Dr["CreatedBy"].ToString();
                    }

                    Dr.Close();

                    //created by Thaddaeus Matthias, 27 Sept 2018
                    //inserting to status log
                    //=====================================begin========================================
                    ListMethod.StatusLogVendor("HeaderReceiptOrder", "RO", txtVendID.Text, "01", "Edit", RONumber, txtPONumber.Text, txtInventSiteID.Text, "");
                    //======================================end=========================================

                    if (dgvRODetails.Rows.Count > 0)
                    {
                        OldPORemaining.Clear();

                        for (int i = 0; i <= dgvRODetails.RowCount - 1; i++)
                        {
                            //Update Invent_Purchase_Qty RO
                            decimal QtyInput;
                            decimal ConvRatio;
                            decimal Price;
                            decimal QtyUoM;
                            decimal QtyAlt;
                            decimal QtyAmt;
                            decimal QtyPRIssued_UoM = 0;
                            decimal QtyPRIssued_Alt = 0;
                            decimal QtyPRIssued_Amt = 0;
                            decimal QtyPOIssued_UoM = 0;
                            decimal QtyPOIssued_Alt = 0;
                            decimal QtyPOIssued_Amt = 0;
                            decimal QtyUoMNew;
                            decimal QtyAltNew;
                            decimal QtyAmtNew;
                            decimal TmpQtyUoMNew;
                            decimal TmpQtyAltNew;
                            decimal TmpQtyAmtNew;

                            string FullItemId = dgvRODetails.Rows[i].Cells["FullItemID"].Value.ToString();
                            string Unit = dgvRODetails.Rows[i].Cells["Unit"].Value.ToString();
                            ConvRatio = 0;

                            string QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "';";
                            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                            UoM = Cmd.ExecuteScalar().ToString();

                            QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "';";
                            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                            ConvRatio = (decimal)Cmd.ExecuteScalar();

                            if (Unit == UoM && Unit != "KG")
                            {
                                QtyUoMNew = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString());
                                QtyAltNew = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString()) * ConvRatio;
                            }
                            else if (Unit == "KG")
                            {
                                QtyAltNew = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString());
                                QtyUoMNew = decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString()) / ConvRatio;
                            }
                            else
                            {
                                MessageBox.Show("Unit Measurement " + Unit + " untuk FullItemId = " + FullItemId + " tidak terdaftar pada InventTable");
                                return;
                            }
                            QtyAmtNew = (QtyUoMNew * decimal.Parse(dgvRODetails.Rows[i].Cells["Price"].Value.ToString()));

                            //Edit save to PurchRequisition_LogTable
                            Query = "Insert into [ReceiptOrder_LogTable] ([ReceiptOrderDate],[ReceiptOrderNo],[PurchaseOrderNo],[PurchaseOrderDate],[VendorID],[InventSiteID],[FullItemId],[SeqNo],[Qty_UoM],[Qty_Alt],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) ";
                            Query += "VALUES ('" + dtRoDate.Value.ToString("yyyy-MM-dd") + "', '" + RONumber + "', '" + txtPONumber.Text.Trim() + "', '" + dtPODate.Value.ToString("yyyy-MM-dd") + "', '" + txtVendID.Text.Trim() + "','" + txtInventSiteID.Text.Trim() + "', '" + dgvRODetails.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["SeqNo"].Value.ToString() + "','" + QtyUoMNew + "', '" + QtyAltNew + "', '01' ,'Created' ,'Created " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate())";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            //Edit save to PurchRequisition_LogTable

                            Query = "Select Qty From ReceiptOrderD Where ReceiptOrderId='" + txtRoNumber.Text + "' and FullItemID = '" + dgvRODetails.Rows[i].Cells["FullItemID"].Value.ToString() + "' and SeqNo = '" + dgvRODetails.Rows[i].Cells["No"].Value.ToString() + "';";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            decimal OldQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                            if (Unit == UoM)
                            {
                                TmpQtyUoMNew = (-1 * OldQty) + QtyUoMNew;
                                TmpQtyAltNew = ((-1 * OldQty) + QtyUoMNew) * ConvRatio;
                            }
                            else
                            {
                                TmpQtyAltNew = (-1 * OldQty) + QtyUoMNew;
                                TmpQtyUoMNew = ((-1 * OldQty) + QtyUoMNew) / ConvRatio;
                            }
                            TmpQtyAmtNew = (TmpQtyUoMNew * decimal.Parse(dgvRODetails.Rows[i].Cells["Price"].Value.ToString()));
                            if (dgvRODetails.Rows[i].Cells["POUnit"].Value.ToString() == "KG")
                            {
                                TmpQtyAmtNew = (TmpQtyAltNew * decimal.Parse(dgvRODetails.Rows[i].Cells["Price_KG"].Value.ToString()));
                            }

                            QueryTemp = "Select RO_Issued_UoM,RO_Issued_Alt,[RO_Issued_Amount],PO_Issued_Outstanding_UoM,PO_Issued_Outstanding_Alt,PO_Issued_Outstanding_Amount From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "';";
                            Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    QtyPRIssued_UoM = decimal.Parse(Dr["RO_Issued_UoM"].ToString());
                                    QtyPRIssued_Alt = decimal.Parse(Dr["RO_Issued_Alt"].ToString());
                                    QtyPRIssued_Amt = decimal.Parse(Dr["RO_Issued_Amount"].ToString());
                                    QtyPOIssued_UoM = decimal.Parse(Dr["PO_Issued_Outstanding_UoM"].ToString());
                                    QtyPOIssued_Alt = decimal.Parse(Dr["PO_Issued_Outstanding_Alt"].ToString());
                                    QtyPOIssued_Amt = decimal.Parse(Dr["PO_Issued_Outstanding_Amount"].ToString());
                                }
                            }
                            //else
                            //{
                            //    Cmd = new SqlCommand("INSERT INTO Invent_Purchase_Qty {[GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemId] ,[ItemName]) VALUES ( '" + dgvRODetails.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dgvRODetails.Rows[i].Cells["ItemName"].Value.ToString() + "');", Conn);
                            //    Cmd.ExecuteNonQuery();
                            //}

                            Dr.Close();

                            Query = "Update Invent_Purchase_Qty Set RO_Issued_UoM += " + TmpQtyUoMNew + ", RO_Issued_Alt += " + TmpQtyAltNew + ",RO_Issued_Amount += " + TmpQtyAmtNew + ",PO_Issued_Outstanding_UoM -= " + TmpQtyUoMNew + ", PO_Issued_Outstanding_Alt-= " + TmpQtyAltNew + ",PO_Issued_Outstanding_Amount -= " + TmpQtyAmtNew + "   Where FullItemID = '" + FullItemId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            //Update Invent_Purchase_Qty RO

                            //String SeqNo1 = dgvRODetails.Rows[i].Cells["No"].Value == null ? "" : dgvRODetails.Rows[i].Cells["No"].Value.ToString();
                            //String PurchOrderSeqNo1 = dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value == null ? "" : dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value.ToString();
                            //decimal Qty1 = dgvRODetails.Rows[i].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Qty"].Value.ToString());
                            //decimal Ratio1 = dgvRODetails.Rows[i].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[i].Cells["Ratio"].Value.ToString());
                            //String Unit1 = dgvRODetails.Rows[i].Cells["Unit"].Value == "" ? "" : dgvRODetails.Rows[i].Cells["Unit"].Value.ToString();

                            //Query = "Select Qty from [dbo].[ReceiptOrderD] where ReceiptOrderId = '" + txtRoNumber.Text + "' and PurchaseOrderID = '" + txtPONumber.Text + "' and PurchOrderSeqNo = '" + PurchOrderSeqNo1 + "'";
                            //Cmd = new SqlCommand(Query, Conn, Trans);
                            //Dr = Cmd.ExecuteReader();

                            //while (Dr.Read())
                            //{
                            //    QtyOld = decimal.Parse(Dr["Qty"].ToString());
                            //}
                            //Dr.Close();

                            //decimal VQty = (QtyOld - Qty1);

                            //Query = "Update [dbo].[PurchDtl] set RemainingQty = (RemainingQty + " + VQty + ") where PurchID = '" + txtPONumber.Text + "' and SeqNo = '" + PurchOrderSeqNo1 + "'";
                            //Cmd = new SqlCommand(Query, Conn, Trans);
                            //Cmd.ExecuteNonQuery();   

                            //if (Unit1 == "KG")
                            //{
                            //    Decimal QtyUoM = (QtyOld / Ratio1) - (Qty1 / Ratio1);
                            //    Decimal QtyEdit = QtyOld - Qty1;
                            //    Query = "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM + " + QtyUoM + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt + " + QtyEdit + ") where FullItemID = '" + FullItemId + "' ";
                            //}
                            //else
                            //{
                            //    Decimal QtyAlt = (QtyOld / Ratio1) - (Qty1 * Ratio1);
                            //    Decimal QtyEdit = QtyOld - Qty1;
                            //    Query = "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM + " + QtyEdit + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt + " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ";
                            //}
                            //Cmd = new SqlCommand(Query, Conn, Trans);
                            //Cmd.ExecuteNonQuery();

                        }
                        Query1 = "Delete from [dbo].[ReceiptOrderD] where ReceiptOrderID = '" + txtRoNumber.Text.Trim() + "' ";
                        Cmd = new SqlCommand(Query1, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        decimal TmpPORemainingTotal = 0;
                        for (int j = 0; j <= dgvRODetails.RowCount - 1; j++)
                        {
                            if (dgvRODetails.Rows[j].Cells["POEstDeliveryDate"].Value == null || dgvRODetails.Rows[j].Cells["POEstDeliveryDate"].Value == "")
                                dgvRODetails.Rows[j].Cells["POEstDeliveryDate"].Value = "01-01-1900";
                            if (dgvRODetails.Rows[j].Cells["SOExpectedDateFrom"].Value == null || dgvRODetails.Rows[j].Cells["SOExpectedDateFrom"].Value == "")
                                dgvRODetails.Rows[j].Cells["SOExpectedDateFrom"].Value = "01-01-1900";
                            if (dgvRODetails.Rows[j].Cells["SOExpectedDateTo"].Value == null || dgvRODetails.Rows[j].Cells["SOExpectedDateTo"].Value == "")
                                dgvRODetails.Rows[j].Cells["SOExpectedDateTo"].Value = "01-01-1900";

                            String PurchaseOrderSeqNo2 = dgvRODetails.Rows[j].Cells["PurchaseOrderSeqNo"].Value == null ? "" : dgvRODetails.Rows[j].Cells["PurchaseOrderSeqNo"].Value.ToString();
                            String SeqNo2 = dgvRODetails.Rows[j].Cells["SeqNo"].Value == null ? "" : dgvRODetails.Rows[j].Cells["SeqNo"].Value.ToString();
                            String GroupId = dgvRODetails.Rows[j].Cells["GroupId"].Value == null ? "" : dgvRODetails.Rows[j].Cells["GroupId"].Value.ToString();
                            String SubGroup1Id = dgvRODetails.Rows[j].Cells["SubGroup1Id"].Value == null ? "" : dgvRODetails.Rows[j].Cells["SubGroup1Id"].Value.ToString();
                            String SubGroup2Id = dgvRODetails.Rows[j].Cells["SubGroup2Id"].Value == null ? "" : dgvRODetails.Rows[j].Cells["SubGroup2Id"].Value.ToString();
                            String ItemId = dgvRODetails.Rows[j].Cells["ItemId"].Value == null ? "" : dgvRODetails.Rows[j].Cells["ItemId"].Value.ToString();
                            String FullItemId = dgvRODetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvRODetails.Rows[j].Cells["FullItemId"].Value.ToString();
                            String ItemName = dgvRODetails.Rows[j].Cells["ItemName"].Value == null ? "" : dgvRODetails.Rows[j].Cells["ItemName"].Value.ToString();
                            decimal Qty2 = dgvRODetails.Rows[j].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Qty"].Value.ToString());
                            decimal QtyKG = dgvRODetails.Rows[j].Cells["Qty_KG"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Qty_KG"].Value.ToString());
                            String Unit = dgvRODetails.Rows[j].Cells["Unit"].Value == "" ? "" : dgvRODetails.Rows[j].Cells["Unit"].Value.ToString();
                            String Notes = dgvRODetails.Rows[j].Cells["Notes"].Value == null ? "" : dgvRODetails.Rows[j].Cells["Notes"].Value.ToString().Trim();
                            decimal Ratio = dgvRODetails.Rows[j].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Ratio"].Value.ToString());
                            decimal Price = dgvRODetails.Rows[j].Cells["Price"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Price"].Value.ToString());
                            decimal PriceKG = dgvRODetails.Rows[j].Cells["Price_KG"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Price_KG"].Value.ToString());
                            decimal Total = dgvRODetails.Rows[j].Cells["Total"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Total"].Value.ToString());
                            decimal Diskon = dgvRODetails.Rows[j].Cells["Diskon"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Diskon"].Value.ToString());
                            decimal Total_Disk = dgvRODetails.Rows[j].Cells["Total_Disk"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Total_Disk"].Value.ToString());
                            decimal Total_PPN = dgvRODetails.Rows[j].Cells["Total_PPN"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Total_PPN"].Value.ToString());
                            decimal Total_PPH = dgvRODetails.Rows[j].Cells["Total_PPH"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[j].Cells["Total_PPH"].Value.ToString());
                            String POEstDeliveryDate = dgvRODetails.Rows[j].Cells["POEstDeliveryDate"].Value == null ? "" : FormateDateddmmyyyy(dgvRODetails.Rows[j].Cells["POEstDeliveryDate"].Value.ToString());
                            String SOExpectedDateFrom = dgvRODetails.Rows[j].Cells["SOExpectedDateFrom"].Value == null ? "" : FormateDateddmmyyyy(dgvRODetails.Rows[j].Cells["SOExpectedDateFrom"].Value.ToString());
                            String SOExpectedDateTo = dgvRODetails.Rows[j].Cells["SOExpectedDateTo"].Value == null ? "" : FormateDateddmmyyyy(dgvRODetails.Rows[j].Cells["SOExpectedDateTo"].Value.ToString());
                            String SONumber = dgvRODetails.Rows[j].Cells["SONumber"].Value == null ? "" : dgvRODetails.Rows[j].Cells["SONumber"].Value.ToString();
                            String DeliveryMethod = dgvRODetails.Rows[j].Cells["DeliveryMethod"].Value == null ? "" : dgvRODetails.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                            String POUnit = dgvRODetails.Rows[j].Cells["POUnit"].Value == "" ? "" : dgvRODetails.Rows[j].Cells["POUnit"].Value.ToString();
                            String PORemaining = dgvRODetails.Rows[j].Cells["PORemaining"].Value == "" ? "" : dgvRODetails.Rows[j].Cells["PORemaining"].Value.ToString();

                            OldPORemaining.Add(Convert.ToDecimal(PORemaining));
                            TmpPORemainingTotal += Convert.ToDecimal(PORemaining);

                            Query = "Insert into [dbo].[ReceiptOrderD] (ReceiptOrderID, PurchaseOrder_EstDeliveryDate, PurchaseOrderID, PurchaseOrderSeqNo, SalesExpectedDateFrom, SalesExpectedDateTo, SalesOrderNo, SeqNo,";
                            Query += "GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, Qty, Qty_KG, RemainingQty, Unit, Ratio, Price, Price_KG, Total, Diskon, Total_Disk, Total_PPN, Total_PPH, InventSiteId, Deskripsi, DeliveryMethod, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) ";
                            Query += "values ('" + RONumber + "',  '" + POEstDeliveryDate + "', '" + txtPONumber.Text + "', '" + PurchaseOrderSeqNo2 + "', '" + SOExpectedDateFrom + "', '" + SOExpectedDateTo + "', '" + SONumber + "', '" + SeqNo2 + "', ";
                            Query += "'" + GroupId + "','" + SubGroup1Id + "','" + SubGroup2Id + "','" + ItemId + "','" + FullItemId + "', '" + ItemName + "', '" + Qty2 + "', '" + QtyKG + "', '" + Qty2 + "', '" + Unit + "', '" + Ratio + "', '" + Price + "', '" + PriceKG + "', '" + Total + "', '" + Diskon + "', ";
                            Query += "'" + Total_Disk + "', '" + Total_PPN + "', '" + Total_PPH + "', '" + txtInventSiteID.Text + "',@Notes, '" + DeliveryMethod + "','" + CreatedDate + "','" + CreatedBy + "',getdate(), '" + ControlMgr.UserId + "');";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.Parameters.AddWithValue("@Notes", Notes);
                            Cmd.ExecuteNonQuery();

                            //decimal NewRemainingQty = 0;
                            //if (POUnit == Unit)
                            //{
                            //    NewRemainingQty = Convert.ToDecimal(PORemaining) + GetOldQty() - Convert.ToDecimal(Qty2);
                            //}
                            //else
                            //{
                            //    if (POUnit.ToUpper() == "KG")
                            //    {
                            //        NewRemainingQty = Convert.ToDecimal(PORemaining) + GetOldQty() - (Convert.ToDecimal(Qty2) * Ratio);
                            //    }
                            //    else
                            //    {
                            //        NewRemainingQty = Convert.ToDecimal(PORemaining) + GetOldQty() - (Convert.ToDecimal(Qty2) / Ratio);
                            //    }
                            //}

                            Query = "Update [dbo].[PurchDtl] set RemainingQty = " + PORemaining + " where PurchID = '" + txtPONumber.Text + "' and SeqNo = '" + PurchaseOrderSeqNo2 + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            //if (Unit == "KG")
                            //{
                            //    Qty2 = Qty2 - GetOldQty();
                            //    Decimal QtyUoM = Qty2 / Ratio;
                            //    Query = "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM - " + QtyUoM + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt - " + Qty2 + ") where FullItemID = '" + FullItemId + "' ";
                            //}
                            //else
                            //{
                            //    Qty2 = Qty2 - GetOldQty();
                            //    Decimal QtyAlt = Qty2 * Ratio;
                            //    Query = "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM - " + Qty2 + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt - " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ";
                            //}



                            ////Update Invent_Purchase_Qty RO
                            //if (Unit == "KG" || Unit == "LBR")
                            //{
                            //    Decimal TmpQty = GetOldQty(0) - Qty2;
                            //    Decimal QtyAlt = TmpQty / Ratio;
                            //    Query = "Update Invent_Purchase_Qty set RO_Issued_UoM = (RO_Issued_UoM - " + TmpQty + "),RO_Issued_Alt = (RO_Issued_Alt - " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ;";
                            //}
                            //else
                            //{
                            //    Decimal TmpQty = GetOldQty(0) - Qty2;
                            //    Decimal QtyAlt = TmpQty / Ratio;
                            //    Query = "Update Invent_Purchase_Qty set RO_Issued_UoM = (RO_Issued_UoM - " + TmpQty + "),RO_Issued_Alt = (RO_Issued_Alt - " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ;";
                            //}

                            ////Update Invent_Purchase_Qty PO
                            //if (Unit == POUnit)
                            //{
                            //    Decimal TmpQty = GetOldQty(0) - Qty2;
                            //    Decimal QtyAlt = TmpQty;
                            //    Query += "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM + " + TmpQty + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt + " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ";
                            //}
                            //else
                            //{
                            //    if (POUnit == "KG" || POUnit == "LBR")
                            //    {
                            //        Decimal TmpQty = GetOldQty(0) - Qty2;
                            //        Decimal QtyAlt = TmpQty;
                            //        Query += "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM + " + TmpQty + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt + " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ";
                            //    }
                            //    else
                            //    {
                            //        Decimal TmpQty = GetOldQty(0) - Qty2;
                            //        Decimal QtyAlt = TmpQty;
                            //        Query += "Update Invent_Purchase_Qty set PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM + " + TmpQty + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt + " + QtyAlt + ") where FullItemID = '" + FullItemId + "' ";
                            //    }
                            //}
                            //Cmd = new SqlCommand(Query, Conn, Trans);
                            //Cmd.ExecuteNonQuery();

                        }
                        if (TmpPORemainingTotal <= 1)
                        {
                            Query = "Update [dbo].[PurchH] set TransStatus = '07', StClose='1' where PurchID = '" + txtPONumber.Text + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
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
                MessageBox.Show("Data :" + RONumber + " Berhasil diupdate.");
                GetDataHeader();
                ModeBeforeEdit();
                Parent.RefreshGrid();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            dtRoDate.Enabled = false;
            //cmbPrType.Enabled = false;
            txtPrStatus.Enabled = false;

            ModeBeforeEdit();
            GetDataHeader();

            //asdf
            deletedItem.Clear();
            deletedQty.Clear();
            deletedUnit.Clear();
            deletedRatio.Clear();
            deletedPrice.Clear();
            ROSeqNo = Convert.ToInt32(dgvRODetails.Rows[dgvRODetails.Rows.Count - 1].Cells["No"].Value);
        }

        private string FormateDateddmmyyyy(string tmpDate)
        {
            //string reformat="";
            string[] data = tmpDate.Split('/');
            if (data.Count() != 1)
            {
                return data[2] + "/" + data[1] + "/" + data[0];
            }
            else
            {
                data = tmpDate.Split('/');
                return data[2].Substring(0, 4) + "/" + (data[0].Length != 2 ? "0" + data[0] : data[0]) + "/" + (data[1].Length != 2 ? "0" + data[1] : data[1]);
            }

        }

        private void dgvPrDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "POEstDeliveryDate" || dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "SOExpectedDateFrom" || dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "SOExpectedDateTo")
            {
                dtp.Location = dgvRODetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                dtp.Visible = true;

                if (dgvRODetails.CurrentCell.Value != "" && dgvRODetails.CurrentCell.Value != null)
                {
                    DateTime dDate;
                    if (!DateTime.TryParse(dgvRODetails.CurrentCell.Value.ToString(), out dDate))
                    {
                        dtp.Value = Convert.ToDateTime(FormateDateddmmyyyy(dgvRODetails.CurrentCell.Value.ToString()));
                    }
                    else
                    {
                        dtp.Value = Convert.ToDateTime(dgvRODetails.CurrentCell.Value);
                    }
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
        }

        void cbo_Validating(object sender, CancelEventArgs e)
        {
            DataGridViewComboBoxEditingControl cbo = sender as DataGridViewComboBoxEditingControl;

            DataGridView grid = cbo.EditingControlDataGridView;

            object value = cbo.Text;

            // Add value to list if not there

            if (cbo.Items.IndexOf(value) == -1)
            {
                DataGridViewComboBoxCell cboCol = (DataGridViewComboBoxCell)grid.CurrentCell;

                // Must add to both the current combobox as well as the template, to avoid duplicate entries...

                cbo.Items.Add(value);

                cboCol.Items.Add(value);

                grid.CurrentCell.Value = value;
            }
        }

        private void dgvPrDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (CountLoop == 0)
            {
                if (dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "POEstDeliveryDate" || dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "SOExpectedDateFrom" || dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "SOExpectedDateTo")
                {
                    if (dgvRODetails.CurrentCell.Value != "" && dgvRODetails.CurrentCell.Value != null)
                    {
                        dgvRODetails.CurrentCell.Value = dtp.Value.Date.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dtp.Value = DateTime.Now;
                    }
                }

                for (int i = 0; i < dgvRODetails.RowCount; i++)
                {
                    //Index = dgvRODetails.CurrentRow.Index;
                    Index = i;
                    Decimal TmpQty = 0;

                    if (dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "Qty")
                    {
                        GetUpdatePORemainingQty();

                        if (Mode.ToUpper() == "EDIT")
                        {
                            if (dgvRODetails.Rows[Index].Cells["Ratio"].Value == "")
                            {
                                dgvRODetails.Rows[Index].Cells["Ratio"].Value = 1;
                            }
                            if (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value) == 0)
                            {
                                dgvRODetails.Rows[Index].Cells["Ratio"].Value = 1;
                            }
                            if (dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() == dgvRODetails.Rows[Index].Cells["Unit"].Value.ToString().ToUpper())
                            {
                                TmpQty = OldPORemaining[Index] + GetOldQty(1) - Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value);
                                if (TmpQty < 0)
                                {
                                    MessageBox.Show("Qty tidak boleh lebih besar dari PO Remaining.");
                                    dgvRODetails.Rows[Index].Cells["Qty"].Value = (Math.Floor(OldPORemaining[Index]) + GetOldQty(1)).ToString("F");
                                    dgvRODetails.Rows[i].Cells["Qty_Kg"].Value = Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value);
                                }
                                dgvRODetails.Rows[Index].Cells["PORemaining"].Value = OldPORemaining[Index] + GetOldQty(1) - Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value);
                            }
                            else if (dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() != dgvRODetails.Rows[Index].Cells["Unit"].Value.ToString().ToUpper())
                            {
                                if (dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() == "KG" || dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() == "LBR")
                                {
                                    TmpQty = (OldPORemaining[Index] / Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "1" : dgvRODetails.Rows[Index].Cells["Ratio"].Value)) + GetOldQty(1) - Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value);
                                    if (TmpQty < 0)
                                    {
                                        MessageBox.Show("Qty tidak boleh lebih besar dari PO Remaining.");
                                        dgvRODetails.Rows[Index].Cells["Qty"].Value = (Math.Floor(OldPORemaining[Index] / Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value)) + GetOldQty(1)).ToString("F");
                                        dgvRODetails.Rows[i].Cells["Qty_Kg"].Value = Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value);
                                    }
                                    dgvRODetails.Rows[Index].Cells["PORemaining"].Value = (OldPORemaining[Index] + (GetOldQty(1) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value)) - (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value)));

                                }
                                else if (dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() != "KG" && dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() != "LBR")
                                {
                                    TmpQty = OldPORemaining[Index] + GetOldQty(1) - (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value) / Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value));
                                    if (TmpQty < 0)
                                    {
                                        MessageBox.Show("Qty tidak boleh lebih besar dari PO Remaining.");
                                        dgvRODetails.Rows[Index].Cells["Qty"].Value = (Math.Floor(OldPORemaining[Index] + GetOldQty(1))).ToString("F");
                                        dgvRODetails.Rows[i].Cells["Qty_Kg"].Value = Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value);
                                    }
                                    dgvRODetails.Rows[Index].Cells["PORemaining"].Value = (OldPORemaining[Index] + GetOldQty(1) - (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value) / Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value)));
                                }
                            }
                        }
                        else
                        {
                            if (dgvRODetails.Rows[Index].Cells["Ratio"].Value == "")
                            {
                                dgvRODetails.Rows[Index].Cells["Ratio"].Value = 1;
                            }
                            if (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value) == 0)
                            {
                                dgvRODetails.Rows[Index].Cells["Ratio"].Value = 1;
                            }
                            if (dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() == dgvRODetails.Rows[Index].Cells["Unit"].Value.ToString().ToUpper())
                            {
                                TmpQty = OldPORemaining[Index] - Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value);
                                if (TmpQty < 0)
                                {
                                    MessageBox.Show("Qty tidak boleh lebih besar dari PO Remaining.");
                                    dgvRODetails.Rows[Index].Cells["Qty"].Value = (Math.Floor(OldPORemaining[Index])).ToString("F");
                                    dgvRODetails.Rows[i].Cells["Qty_Kg"].Value = Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value);
                                }
                                dgvRODetails.Rows[Index].Cells["PORemaining"].Value = OldPORemaining[Index] - Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value);
                            }
                            else if (dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() != dgvRODetails.Rows[Index].Cells["Unit"].Value.ToString().ToUpper())
                            {
                                if (dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() == "KG" || dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() == "LBR")
                                {
                                    TmpQty = OldPORemaining[Index] - (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value));
                                    if (TmpQty < 0)
                                    {
                                        MessageBox.Show("Qty tidak boleh lebih besar dari PO Remaining.");
                                        dgvRODetails.Rows[Index].Cells["Qty"].Value = (Math.Floor(OldPORemaining[Index] / Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value))).ToString("F");
                                        dgvRODetails.Rows[i].Cells["Qty_Kg"].Value = Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value);
                                    }
                                    dgvRODetails.Rows[Index].Cells["PORemaining"].Value = (OldPORemaining[Index] - (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value)));
                                }
                                else if (dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() != "KG" && dgvRODetails.Rows[Index].Cells["POUnit"].Value.ToString().ToUpper() != "LBR")
                                {
                                    TmpQty = OldPORemaining[Index] - (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value) / Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value));
                                    if (TmpQty < 0)
                                    {
                                        MessageBox.Show("Qty tidak boleh lebih besar dari PO Remaining.");
                                        dgvRODetails.Rows[Index].Cells["Qty"].Value = (Math.Floor(OldPORemaining[Index] * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value))).ToString("F");
                                        dgvRODetails.Rows[i].Cells["Qty_Kg"].Value = Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value);
                                    }
                                    dgvRODetails.Rows[Index].Cells["PORemaining"].Value = (OldPORemaining[Index] - (Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Qty"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Qty"].Value) / Convert.ToDecimal(dgvRODetails.Rows[Index].Cells["Ratio"].Value == "" ? "0" : dgvRODetails.Rows[Index].Cells["Ratio"].Value)));
                                }
                            }
                        }
                    }
                }

                dtp.Visible = false;
                dgvRODetails.AutoResizeColumns();

                CountLoop = 1;
            }
            else
            {
                CountLoop = 0;
            }
        }

        private decimal GetOldQty(int statusCellEdit)
        {
            decimal result = 0;
            try
            {
                if (statusCellEdit == 1)
                {
                    Conn = ConnectionString.GetConnection();
                }
                string Query2 = "SELECT Qty FROM ReceiptOrderD WHERE ReceiptOrderId = '" + txtRoNumber.Text + "' AND PurchaseOrderSeqNo = '" + dgvRODetails.Rows[Index].Cells["PurchaseOrderSeqNo"].Value + "' ";
                SqlCommand Cmd2 = new SqlCommand(Query2, Conn, Trans);
                SqlDataReader Dr2 = Cmd2.ExecuteReader();
                while (Dr2.Read())
                {
                    result = Convert.ToDecimal(Dr2["Qty"]);
                }
                Dr2.Close();

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                if (statusCellEdit == 1)
                {
                    Conn.Close();
                }
            }
            return result;
        }

        private decimal GetOldPORemainingQty()
        {
            decimal result = 0;
            try
            {
                Conn = ConnectionString.GetConnection();
                string Query2 = "SELECT RemainingQty FROM PurchDtl WHERE [PurchID] = '" + txtPONumber.Text + "' AND SeqNo = '" + dgvRODetails.Rows[Index].Cells["PurchaseOrderSeqNo"].Value + "' ";
                SqlCommand Cmd2 = new SqlCommand(Query2, Conn, Trans);
                SqlDataReader Dr2 = Cmd2.ExecuteReader();
                while (Dr2.Read())
                {
                    result = Convert.ToDecimal(Dr2["RemainingQty"]);
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

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dgvRODetails.CurrentCell.Value = dtp.Text;
        }

        private void cmbPrType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GetGelombang();
            //tmpPrType = cmbPrType.Text;
        }

        private void dgvPrDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex > -1 && e.RowIndex > -1)
            //{
            //    if (dgvPrDetails.Rows[e.RowIndex].Cells["Base"].Value.ToString() != "N")
            //    {
            //        if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "SalesSO" && Mode != "BeforeEdit")
            //        {
            //            string SchemaName = "dbo";
            //            string TableName = "SalesOrder";

            //            Search tmpSearch = new Search();
            //            tmpSearch.SetSchemaTable(SchemaName, TableName);
            //            tmpSearch.ShowDialog();
            //            dgvPrDetails.CurrentCell.Value = ConnectionString.Kode;
            //            //dgvPrDetails.Columns[e.ColumnIndex].Name = ConnectionString.Kode2;
            //        }

            //        //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Vendor" && Mode != "BeforeEdit")
            //        //{
            //        //    string SchemaName = "dbo";
            //        //    string TableName = "VendTable";

            //        //    Search tmpSearch = new Search();
            //        //    tmpSearch.SetSchemaTable(SchemaName, TableName);
            //        //    tmpSearch.ShowDialog();
            //        //    dgvPrDetails.CurrentCell.Value = ConnectionString.Kode;
            //        //    //dgvPrDetails.Columns[e.ColumnIndex].Name = ConnectionString.Kode2;
            //        //}

            //        if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Vendor" && Mode != "BeforeEdit")
            //        {
            //            PopUpSelect.Vendor tmpSearch = new PopUpSelect.Vendor();
            //            tmpSearch.VendId = dgvPrDetails.CurrentCell.Value.ToString();
            //            //tmpSearch.RefreshGrid(dgvPrDetails.CurrentCell.Value.ToString());
            //            tmpSearch.ShowDialog();
            //            dgvPrDetails.CurrentCell.Value = tmpSearch.VendId;
            //            tmpSearch.VendId = "";
            //            //dgvPrDetails.CurrentCell.Value = ConnectionString.Kode;
            //            //dgvPrDetails.Columns[e.ColumnIndex].Name = ConnectionString.Kode2;
            //        }

            //    }


            //    //if (Mode != "BeforeEdit")
            //    //{
            //    //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "D1")
            //    //{
            //    //    Info tmpInfo = new Info();

            //    //    ListInfo.Add(tmpInfo);
            //    //    //tmpInfo.SetParent(this);
            //    //    tmpInfo.Show();
            //    //}
            //    //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "D2")
            //    //{
            //    //    Info tmpInfo = new Info();

            //    //    ListInfo.Add(tmpInfo);
            //    //    //tmpInfo.SetParent(this);
            //    //    tmpInfo.Show();
            //    //}
            //    //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "D3")
            //    //{
            //    //    Info tmpInfo = new Info();

            //    //    ListInfo.Add(tmpInfo);
            //    //    //tmpInfo.SetParent(this);
            //    //    tmpInfo.Show();
            //    //}
            //    //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "D4")
            //    //{
            //    //    Info tmpInfo = new Info();

            //    //    ListInfo.Add(tmpInfo);
            //    //    //tmpInfo.SetParent(this);
            //    //    tmpInfo.Show();
            //    //}
            //    //}
            //}
            ////dgvPrDetails.CurrentRow.Cells["ItemFullID"].Value.ToString();

        }

        private void dgvPrDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);
            }
            //hendry 
            //if (e.Control.AccessibilityObject.Role.ToString() == "Qty")
            //{
            //    int count = 0;
            //    foreach (char c in dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value.ToString())
            //    {
            //        if (c == '.') count++;
            //    }
            //}
            //hendry end
        }

        public void SetParent(Purchase.ReceiptOrder.InquiryReceiptOrder F)
        {
            Parent = F;
        }

        private void HeaderReceiptOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
            //for (int i = 0; i < ListDetailPR.Count(); i++)
            //{
            //    ListDetailPR[i].Close();
            //}
            //for (int i = 0; i < ListGelombang.Count(); i++)
            //{
            //    ListGelombang[i].Close();
            //}
            //for (int i = 0; i < ListInfo.Count(); i++)
            //{
            //    ListInfo[i].Close();
            //}
            //for (int i = 0; i < ListVendor.Count(); i++)
            //{
            //    ListVendor[i].Close();
            //}
            //Purchase.PurchaseQuotation.FormPQ.reffID = null;
            //Purchase.PurchaseRequisition.InquiryPR f = new Purchase.PurchaseRequisition.InquiryPR();
            //f.RefreshGrid();
            //if (Mode != "ModeView")
            //{
            //    Parent.RefreshGrid();
            //}
        }

        private void HeaderReceiptOrder_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void MethodReadOnlyRowBaseN()
        {
            for (int i = 0; i < dgvRODetails.RowCount; i++)
            {
                if (dgvRODetails.Rows[i].Cells["Base"].Value.ToString() == "N") //&& cmbPrType.Text.Trim() != "FIX")
                {
                    dgvRODetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvRODetails.Rows[i].ReadOnly = true;
                }
                else
                {
                    dgvRODetails.Rows[i].ReadOnly = false;
                }
            }
        }

        private void DeliveryMethod_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvRODetails.CurrentCell.Value = DeliveryMethod.Text.ToString();
            for (int j = 0; j < dgvRODetails.RowCount; j++)
            {
                if (dgvRODetails.Rows[j].Cells["SeqNoGroup"].Value == dgvRODetails.Rows[dgvRODetails.CurrentCell.RowIndex].Cells["No"].Value.ToString())
                {
                    dgvRODetails.Rows[j].Cells["DeliveryMethod"].Value = dgvRODetails.Rows[dgvRODetails.CurrentCell.RowIndex].Cells["DeliveryMethod"].Value.ToString();
                }
            }
        }

        private void DeliveryMethod_DropDownClosed(object sender, EventArgs e)
        {
            DeliveryMethod.Visible = false;
        }
        //tia edit
        //kanan
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.Vendor.Vendor Vend = null;
        Sales.SalesOrder.SOHeader Salesorder = null;
        Purchase.PurchaseOrderNew.POForm PONumber = null;

        Purchase.GoodsReceipt.GRHeaderV2 ParentToGRNumber;

        public void ParentRefreshGrid(Purchase.GoodsReceipt.GRHeaderV2 GRNumb)
        {
            ParentToGRNumber = GRNumb;
        }

        private void dgvRODetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        PopUpItemName.Close();
                        // PopUpItemName = new PopUp.Stock.Stock();
                        //PopUpItemName = new PopUp.FullItemId.FullItemId();
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvRODetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvRODetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvRODetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
                if (Salesorder == null || Salesorder.Text == "")
                {
                    if (dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "SONumber")
                    {
                        Salesorder = new Sales.SalesOrder.SOHeader();
                        Salesorder.SetMode("PopUp", dgvRODetails.Rows[e.RowIndex].Cells["SONumber"].Value.ToString());
                        Salesorder.ParentRefreshGrid3(this);
                        Salesorder.Show();
                    }
                }
                else if (CheckOpened(Salesorder.Name))
                {
                    Salesorder.WindowState = FormWindowState.Normal;
                    Salesorder.SetMode("PopUp", dgvRODetails.Rows[e.RowIndex].Cells["SONumber"].Value.ToString());
                    Salesorder.ParentRefreshGrid3(this);
                    Salesorder.Show();
                    Salesorder.Focus();
                }

            }
        }

        //private void dgvPrDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //   
        //}

        private void txtVendID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    txtVendID.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtVendID.Text);
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
        private void txtVendName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    txtVendName.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtVendID.Text);
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
        private void tbxVOwnerID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    tbxVOwnerID.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtVendID.Text);
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
                    Vend.GetData(txtVendID.Text);
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
        private void txtPONumber_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (PONumber == null || PONumber.Text == "")
                {
                    txtPONumber.Enabled = true;
                    PONumber = new Purchase.PurchaseOrderNew.POForm();
                    PONumber.SetMode("PopUp", txtPONumber.Text, "");
                    PONumber.ParentRefreshGrid(this);
                    PONumber.Show();
                }
                else if (CheckOpened(PONumber.Name))
                {
                    PONumber.WindowState = FormWindowState.Normal;
                    PONumber.Show();
                    PONumber.Focus();
                }
            }
        }
        //tia edit end

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void btnSearchVendor_Click(object sender, EventArgs e)
        {
            //string SchemaName = "dbo";
            //string TableName = "PurchH";
            //string Where = "And (StClose = 'False')";

            //Search tmpSearch = new Search();
            //tmpSearch.SetSchemaTable(SchemaName, TableName, Where);
            //tmpSearch.ShowDialog();

            //txtPONumber.Text = ConnectionString.Kode;
            //txtVendID.Text = ConnectionString.Kode2;
            //dtPODate.Text = ConnectionString.Kode3;

            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.FormName = "Search Po";
            tmpSearch.PrimaryKey = "[PO No]";
            tmpSearch.Order = "[PO No] asc";
            tmpSearch.Table = "[dbo].[PurchH]";
            Query = "SELECT distinct a.[PurchID] 'PO No',a.[OrderDate] 'PO Date', b.VendName 'Vendor',a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy],a.VendId ";
            Query += "FROM [dbo].[PurchH] a ";
            Query += "Left Join VendTable b on a.VendId=b.VendId ";
            Query += "Left Join PurchDtl c on c.PurchID = a.PurchID ";
            Query += "WHERE (TransStatus = '01' OR TransStatus = '05') AND RemainingQty != 0 AND StClose = 0 ";
            tmpSearch.QuerySearch = Query;
            //tmpSearch.FilterText = new string[] { "[PO No]", "[PO Date]", "Name" };
            tmpSearch.FilterText = new string[] { "[PO No]", "[PO Date]", "Vendor", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "PO No", "PO Date", "Vendor", "Created Date", "Created By", "Updated Date", "Updated By" };
            //tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
            tmpSearch.Select = new string[] { "PO No", "VendId", "PO Date" };
            tmpSearch.Hide = new string[] { "VendId" };
            tmpSearch.Order = "CreatedDate Desc";
            //tmpSearch.WherePlus = "";
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtPONumber.Text = ConnectionString.Kodes[0];
                txtVendID.Text = ConnectionString.Kodes[1];
                dtPODate.Text = ConnectionString.Kodes[2];
                ConnectionString.Kodes = null;
            }
            passDelivMethodValue();
            GetGridDataModeNew();
        }

        private void passDelivMethodValue()
        {
            //BY: HC (S) | MASUKIN VALUE DELIVERY METHOD KE COMBOBOX
            if (txtPONumber.Text != "")
            {
                label12.Visible = true;
                cmbDelivMethod.Visible = true;
                cmbDelivMethod.Items.Clear();
                //cmbDelivMethod.Items.Add("Select");
                //cmbDelivMethod.SelectedIndex = 0;
                Conn = ConnectionString.GetConnection();
                Query = "select distinct(DeliveryMethod) from PurchDtl where PurchID = '" + txtPONumber.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    cmbDelivMethod.Items.Add(Dr["DeliveryMethod"]);
                }
                Dr.Close();
                Conn.Close();
                //if (cmbDelivMethod.Items.Count == 2)
                //{
                //    cmbDelivMethod.Items.RemoveAt(0);
                cmbDelivMethod.SelectedIndex = 0;
                //}
            }
            //BY: HC (E)
        }

        private void GetGridDataModeNew()
        {
            dgvRODetails.Rows.Clear();
            dgvRODetails.Columns.Clear();
            if (dgvRODetails.RowCount - 1 <= 0)
            {
                dgvRODetails.ColumnCount = 31;
                dgvRODetails.Columns[0].Name = "No";
                dgvRODetails.Columns[1].Name = "FullItemID";
                dgvRODetails.Columns[2].Name = "GroupId";
                dgvRODetails.Columns[3].Name = "SubGroup1Id";
                dgvRODetails.Columns[4].Name = "SubGroup2Id";
                dgvRODetails.Columns[5].Name = "ItemId";
                dgvRODetails.Columns[6].Name = "ItemName";
                dgvRODetails.Columns[7].Name = "Qty"; dgvRODetails.Columns[7].HeaderText = "RO Qty";
                dgvRODetails.Columns[8].Name = "RORemaining"; dgvRODetails.Columns[8].HeaderText = "RO Remaining";
                dgvRODetails.Columns[9].Name = "Unit";
                dgvRODetails.Columns[10].Name = "Qty_Kg"; dgvRODetails.Columns[10].HeaderText = "Total Berat";
                dgvRODetails.Columns[11].Name = "POQty"; dgvRODetails.Columns[11].HeaderText = "PO Qty";
                dgvRODetails.Columns[12].Name = "PORemaining"; dgvRODetails.Columns[12].HeaderText = "PO Remaining";
                dgvRODetails.Columns[13].Name = "POUnit"; dgvRODetails.Columns[13].HeaderText = "PO Unit";
                dgvRODetails.Columns[14].Name = "Ratio";
                dgvRODetails.Columns[15].Name = "Notes";
                dgvRODetails.Columns[16].Name = "Price";
                dgvRODetails.Columns[17].Name = "Price_KG";
                dgvRODetails.Columns[18].Name = "Total";
                dgvRODetails.Columns[19].Name = "Diskon";
                dgvRODetails.Columns[20].Name = "Total_Disk"; dgvRODetails.Columns[20].HeaderText = "Total Diskon";
                dgvRODetails.Columns[21].Name = "Total_PPN"; dgvRODetails.Columns[21].HeaderText = "Total PPN";
                dgvRODetails.Columns[22].Name = "Total_PPH"; dgvRODetails.Columns[22].HeaderText = "Total PPH";
                dgvRODetails.Columns[23].Name = "POEstDeliveryDate"; dgvRODetails.Columns[23].HeaderText = "PO est. Delivery Date";
                dgvRODetails.Columns[24].Name = "SONumber"; dgvRODetails.Columns[24].HeaderText = "SO Number";
                dgvRODetails.Columns[25].Name = "SOExpectedDateFrom";
                dgvRODetails.Columns[26].Name = "SOExpectedDateTo";
                dgvRODetails.Columns[27].Name = "PurchaseOrderSeqNo";
                dgvRODetails.Columns[28].Name = "DeliveryMethod";
                dgvRODetails.Columns[29].Name = "PurchaseOrderID";
                dgvRODetails.Columns[30].Name = "SeqNo";
            }

            Conn = ConnectionString.GetConnection();
            Query = "Select a.FullItemID, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.ItemName, case when a.Qty is null then 0 else a.Qty end Qty, a.RemainingQty, a.Unit, b.UoM, a.KOnv_Ratio, a.Price, case when a.Price_KG is null then 0 else a.Price_KG end Price_KG, a.Total, a.Diskon, a.Total_Disk, a.Total_PPN, a.Total_PPH, a.PurchID, a.SeqNo, a.DeliveryMethod, d.ReffTransID ";
            Query += "From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID ";
            Query += "LEFT JOIN dbo.[CanvasSheetD] c ON a.ReffId = c.CanvasID and a.ReffId2 = c.PurchQuotId and a.ReffSeqNo = c.CanvasSeqNo ";
            Query += "LEFT JOIN dbo.[PurchRequisition_Dtl] d ON c.PurchReqId = d.PurchReqId and c.PurchReqSeqNo = d.SeqNo Where a.PurchId = '" + txtPONumber.Text + "' and a.DeliveryMethod = '" + cmbDelivMethod.Text + "';";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    if (Dr["Unit"].ToString() == "KG" || Dr["Unit"].ToString() == "LBR")
                    {
                        UoM = Dr["UoM"].ToString();
                    }
                    else
                    {
                        UoM = Dr["Unit"].ToString();
                    }

                    this.dgvRODetails.Rows.Add((dgvRODetails.RowCount + 1).ToString(), Dr["FullItemID"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["ItemName"], "0.00", "", UoM, "0.00", Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Konv_Ratio"], "", Dr["Price"], Dr["Price_KG"], Dr["Total"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPH"], "", Dr["ReffTransID"], "", "", Dr["SeqNo"], Dr["DeliveryMethod"], Dr["PurchId"], Dr["SeqNo"]/*(dgvRODetails.RowCount + 1).ToString()*/);
                    OldPORemaining.Add(Convert.ToDecimal(Dr["RemainingQty"]));
                }
                Dr.Close();
            }
            Conn.Close();


            for (int i = 0; i < dgvRODetails.Columns.Count; i++)
            {
                if (dgvRODetails.Columns[i].HeaderText == "No" || dgvRODetails.Columns[i].HeaderText == "FullItemID" || dgvRODetails.Columns[i].HeaderText == "ItemName" || dgvRODetails.Columns[i].HeaderText == "RO Qty" || dgvRODetails.Columns[i].HeaderText == "Unit" || dgvRODetails.Columns[i].HeaderText == "Total Berat")
                {
                    dgvRODetails.Columns[i].Visible = true;
                }
                else if (dgvRODetails.Columns[i].HeaderText == "PO Qty" || dgvRODetails.Columns[i].HeaderText == "PO Remaining" || dgvRODetails.Columns[i].HeaderText == "PO Unit" || dgvRODetails.Columns[i].HeaderText == "Ratio" || dgvRODetails.Columns[i].HeaderText == "Notes")
                {
                    dgvRODetails.Columns[i].Visible = true;
                }
                else
                {
                    dgvRODetails.Columns[i].Visible = false;
                }

                if (dgvRODetails.Columns[i].HeaderText == "RO Qty" || dgvRODetails.Columns[i].HeaderText == "Notes")
                {
                    dgvRODetails.Columns[i].ReadOnly = false;
                }
                else
                {
                    dgvRODetails.Columns[i].ReadOnly = true;
                    dgvRODetails.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            dgvRODetails.Columns["GroupId"].Visible = false;
            dgvRODetails.Columns["SubGroup1Id"].Visible = false;
            dgvRODetails.Columns["SubGroup2Id"].Visible = false;
            dgvRODetails.Columns["ItemId"].Visible = false;
            dgvRODetails.Columns["PurchaseOrderSeqNo"].Visible = false;

            dgvRODetails.AutoResizeColumns();
        }

        private void btnWarehouse_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSite";
            //string Where = "And (TransStatus = '21' or TransStatus = '13' or TransStatus = '14')";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);//, Where);
            tmpSearch.ShowDialog();
            txtInventSiteID.Text = ConnectionString.Kode;
            txtWarehouse.Text = ConnectionString.Kode2;
            txtLocation.Text = ConnectionString.Kode3;
        }

        private void txtVendID_TextChanged(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                Conn = ConnectionString.GetConnection();

                Query = "Select VendName from dbo.VendTable where VendID = '" + txtVendID.Text + "' ";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtVendName.Text = Dr["VendName"].ToString();
                }
                Dr.Close();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string ROId = txtRoNumber.Text;

            //PreviewRO f = new PreviewRO(ROId);
            //f.Show();

            ISBS_New.GlobalPreview f = new ISBS_New.GlobalPreview("Receipt Order", ROId);
            f.Show();
        }

        private void dgvRODetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //decimal a = decimal.MaxValue;
            //if (dgvRODetails.RowCount > 0)
            //{
            //    if (Regex.Replace(dgvRODetails.Rows[e.RowIndex].Cells["Qty"].Value.ToString(), "[^0-9]", "").Length > 29)
            //    {
            //        MessageBox.Show("Qty terlalu besar.");
            //        dgvRODetails.Rows[e.RowIndex].Cells["Qty"].Value = dgvRODetails.Rows[e.RowIndex].Cells["Qty"].Value.ToString().Substring(0, 26);
            //    }

            //    if ((Convert.ToDecimal(dgvRODetails.Rows[e.RowIndex].Cells["Qty"].Value) * Convert.ToDecimal(dgvRODetails.Rows[e.RowIndex].Cells["Ratio"].Value)).ToString().Length > 27)
            //    {
            //        MessageBox.Show("Hasil perkalian Qty & Ratio tidak boleh lebih besar dari 79,228,162,514,264,337,593,543,950,335."); 
            //        dgvRODetails.Rows[e.RowIndex].Cells["Qty"].Value = dgvRODetails.Rows[e.RowIndex].Cells["Qty"].Value.ToString().Substring(0,29 - (Convert.ToDecimal(dgvRODetails.Rows[e.RowIndex].Cells["Ratio"].Value).ToString().Length));
            //    }
            //}
            for (int i = 0; i < dgvRODetails.RowCount; i++)
            {
                decimal Qty = 0;
                decimal Ratio = 0;

                Qty = Convert.ToDecimal(dgvRODetails.Rows[i].Cells["Qty"].Value == "" || dgvRODetails.Rows[i].Cells["Qty"].Value == null ? "0" : dgvRODetails.Rows[i].Cells["Qty"].Value.ToString());
                Ratio = Convert.ToDecimal(dgvRODetails.Rows[i].Cells["Ratio"].Value == "" || dgvRODetails.Rows[i].Cells["Ratio"].Value == null ? "0" : dgvRODetails.Rows[i].Cells["Ratio"].Value.ToString());

                dgvRODetails.Rows[i].Cells["Qty_Kg"].Value = Qty * Ratio;
            }
        }

        private void dgvRODetails_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //Conn = ConnectionString.GetConnection();
            //if (dgvRODetails.Columns[e.ColumnIndex].Name.ToString() == "Qty")
            //{
            //    String PurchaseOrderSeqNo = dgvRODetails.Rows[e.RowIndex].Cells["PurchaseOrderSeqNo"].Value == null ? "" : dgvRODetails.Rows[e.RowIndex].Cells["PurchaseOrderSeqNo"].Value.ToString();
            //    String PurchaseOrderID = dgvRODetails.Rows[e.RowIndex].Cells["PurchaseOrderId"].Value == null ? "" : dgvRODetails.Rows[e.RowIndex].Cells["PurchaseOrderId"].Value.ToString();
            //    decimal Qty = dgvRODetails.Rows[e.RowIndex].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvRODetails.Rows[e.RowIndex].Cells["Qty"].Value.ToString());

            //    Query = "Select RemainingQty from dbo.PurchDtl where PurchID = '" + PurchaseOrderID + "' and SeqNo = '" + PurchaseOrderSeqNo + "'";

            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();

            //    while (Dr.Read())
            //    {
            //        decimal PORemainingQty = decimal.Parse(Dr["RemainingQty"].ToString());

            //        if (Qty <= PORemainingQty)
            //        {
            //            MessageBox.Show("Quantity tidak mencukupi sisa : '" + PORemainingQty + "'");
            //        }
            //    }
            //    Dr.Close();
            //}
        }

        private void tabDgvControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvRODetails.Columns.Count != 0)
            {
                if (tabDgvControl.SelectedTab.Text == "Detail RO")
                {
                    for (int i = 0; i < dgvRODetails.Columns.Count; i++)
                    {
                        if (dgvRODetails.Columns[i].HeaderText == "No" || dgvRODetails.Columns[i].HeaderText == "FullItemID" || dgvRODetails.Columns[i].HeaderText == "ItemName" || dgvRODetails.Columns[i].HeaderText == "RO Qty" || dgvRODetails.Columns[i].HeaderText == "Unit" || dgvRODetails.Columns[i].HeaderText == "Total Berat")
                        {
                            dgvRODetails.Columns[i].Visible = true;
                        }
                        else if (dgvRODetails.Columns[i].HeaderText == "PO Qty" || dgvRODetails.Columns[i].HeaderText == "PO Remaining" || dgvRODetails.Columns[i].HeaderText == "PO Unit" || dgvRODetails.Columns[i].HeaderText == "Ratio" || dgvRODetails.Columns[i].HeaderText == "Notes")
                        {
                            dgvRODetails.Columns[i].Visible = true;
                        }
                        else
                        {
                            dgvRODetails.Columns[i].Visible = false;
                        }

                        if (dgvRODetails.Columns[i].HeaderText == "RO Qty" || dgvRODetails.Columns[i].HeaderText == "Notes")
                        {
                            dgvRODetails.Columns[i].ReadOnly = false;
                        }
                        else
                        {
                            dgvRODetails.Columns[i].ReadOnly = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < dgvRODetails.Columns.Count; i++)
                    {
                        if (dgvRODetails.Columns[i].HeaderText == "No" || dgvRODetails.Columns[i].HeaderText == "FullItemID" || dgvRODetails.Columns[i].HeaderText == "ItemName")
                        {
                            dgvRODetails.Columns[i].Visible = true;
                        }
                        else if (dgvRODetails.Columns[i].HeaderText == "PO est. Delivery Date" || dgvRODetails.Columns[i].HeaderText == "SO Number" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateFrom" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateTo")
                        {
                            dgvRODetails.Columns[i].Visible = true;
                        }
                        else
                        {
                            dgvRODetails.Columns[i].Visible = false;
                        }

                        if (dgvRODetails.Columns[i].HeaderText == "PO est. Delivery Date" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateFrom" || dgvRODetails.Columns[i].HeaderText == "SOExpectedDateTo")
                        {
                            dgvRODetails.Columns[i].ReadOnly = false;
                        }
                        else
                        {
                            dgvRODetails.Columns[i].ReadOnly = true;
                        }

                    }
                }

            }
        }

        //Hendry Cek Sisa RO

        private decimal CekSisaRO()
        {
            decimal vSisa = 0;
            String strSql = "SELECT ROD.ReceiptOrderId,ROD.SeqNo,SUM(ROD.Qty)-SUM(ISNULL(GR.Qty,0)) AS SisaRO  ";
            strSql += "FROM ReceiptOrderD ROD LEFT JOIN ReceiptOrderH ROH ON ROD.ReceiptOrderId=ROH.ReceiptOrderId LEFT JOIN GoodsReceivedD GR ";
            strSql += "ON ROD.ReceiptOrderId=GR.RefTransID AND ROD.SeqNo=GR.RefTransSeqNo ";
            strSql += "WHERE ROd.ReceiptOrderId='" + txtRoNumber.Text + "' and isnull(stClose,0) <> 1 ";
            strSql += "GROUP BY ROd.ReceiptOrderId,ROd.SeqNo";
            Cmd = new SqlCommand(strSql, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                vSisa = decimal.Parse(Dr["SisaRO"].ToString());
            }
            Dr.Close();

            return vSisa;
        }
        //Hendry End

        private void ROClose_Click(object sender, EventArgs e)
        {
            //Hendry tambah Cek Sisa, Jika sudah tidak ada sisa maka tidak perlu Close
            decimal vSisa = CekSisaRO();
            if (vSisa == 0) { MessageBox.Show("RO sudah Closed atau Sisa RO sudah habis, tidak perlu Close.."); return; }
            //Hendry End

            Boolean vBolClose = false;
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                DialogResult dialogResult = MessageBox.Show("Apakah RO : " + txtRoNumber.Text + " akan di close ", "Update Status Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Query = "Update [dbo].[ReceiptOrderH] set stClose = 1,ReceiptOrderStatus='05', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where ReceiptOrderId = '" + txtRoNumber.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    vBolClose = true;

                    //created by Thaddaeus Matthias, 27 Sept 2018
                    //inserting status log
                    //========================================begin=======================================
                    ListMethod.StatusLogVendor("HeaderReceiptOrder", "RO", txtVendID.Text, "05", "Closed", txtRoNumber.Text, txtPONumber.Text, txtInventSiteID.Text, "");
                    //=========================================end========================================

                    //BY: HC (S)
                    //INSERT KE RO LOG TABLE
                    Query = "insert into ReceiptOrder_LogTable select top 1 [ReceiptOrderDate],[ReceiptOrderNo] ,[PurchaseOrderNo] ,[PurchaseOrderDate] ,[VendorID] ,[InventSiteID] ,[FullItemId] ,[SeqNo] ,[Qty_UoM] ,[Qty_Alt] ,[Amount] ,[GoodsReceivedId] ,'05' ,'Closed' ,'Closed' ,'" + ControlMgr.UserId + "' ,getdate() from ReceiptOrder_LogTable where ReceiptOrderNo = '" + txtRoNumber.Text + "' order by LogDate desc";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //BALIKIN QTY PO
                    dgvRODetails.Columns[30].Name = "SeqNo";
                    for (int i = 0; i < dgvRODetails.RowCount; i++)
                    {
                        Query = "select Price, Price_KG, Ratio, Unit, FullItemId, Qty, PurchaseOrderId, PurchaseOrderSeqNo from ReceiptOrderD where ReceiptOrderId = '" + txtRoNumber.Text + "' and seqNo = '" + dgvRODetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            //REVERT VALUE INVENT PURCHASE QTY
                            decimal QtyAlt = 0;
                            decimal QtyUoM = 0;
                            decimal QtyAmt = 0;
                            if (Dr["Unit"].ToString() == "KG")
                            {
                                QtyAlt = Convert.ToDecimal(Dr["Qty"]);
                                QtyUoM = Convert.ToDecimal(Dr["Qty"]) / Convert.ToDecimal(Dr["Ratio"]);
                                QtyAmt = QtyUoM * Convert.ToDecimal(Dr["Price_KG"]);
                            }
                            else
                            {
                                QtyUoM = Convert.ToDecimal(Dr["Qty"]);
                                QtyAlt = Convert.ToDecimal(Dr["Qty"]) * Convert.ToDecimal(Dr["Ratio"]);
                                QtyAmt = QtyUoM * Convert.ToDecimal(Dr["Price"]);
                            }
                            Query = "Update Invent_Purchase_Qty Set RO_Issued_UoM -= " + QtyUoM + ", RO_Issued_Alt -= " + QtyAlt + ",RO_Issued_Amount -=" + QtyAmt + ",PO_Issued_Outstanding_UoM +=" + QtyUoM + ", [PO_Issued_Outstanding_Alt] +=" + QtyAlt + ", [PO_Issued_Outstanding_Amount] += " + QtyAmt + " Where FullItemID = '" + Dr["FullItemId"] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            //UPDATE REMAINING QTY PO DETAIL
                            Query = "update PurchDtl set RemainingQty = RemainingQty + " + Dr["Qty"] + " where PurchID = '" + Dr["PurchaseOrderId"] + "' and SeqNo = '" + Dr["PurchaseOrderSeqNo"] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                        Dr.Close();
                    }
                    //BY: HC (E)
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
            Parent.RefreshGrid();
            if (vBolClose == true)
            {
                MessageBox.Show("RO : " + txtRoNumber.Text + " berhasil di Close");
                this.Close();
            }
        }

        private void BtnGunakan_Click(object sender, EventArgs e)
        {
            Boolean vBolClose = false;
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                DialogResult dialogResult = MessageBox.Show("Apakah RO : " + txtRoNumber.Text + " akan digunakan? ", "Update Status Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Query = "Update [dbo].[ReceiptOrderH] set stClose = 0,ReceiptOrderStatus='09', UpdatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where ReceiptOrderId = '" + txtRoNumber.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    vBolClose = true;

                    //created by Thaddaeus Matthias, 27 Sept 2018
                    //inserting status log
                    //========================================begin=======================================
                    ListMethod.StatusLogVendor("HeaderReceiptOrder", "RO", txtVendID.Text, "09", "Sent", txtRoNumber.Text, txtPONumber.Text, txtInventSiteID.Text, "");
                    //=========================================end========================================

                    //BY: HC (S) | INSERT KE RO LOG TABLE
                    Query = "insert into ReceiptOrder_LogTable select top 1 [ReceiptOrderDate],[ReceiptOrderNo] ,[PurchaseOrderNo] ,[PurchaseOrderDate] ,[VendorID] ,[InventSiteID] ,[FullItemId] ,[SeqNo] ,[Qty_UoM] ,[Qty_Alt] ,[Amount] ,[GoodsReceivedId] ,'09' ,'Sent' ,'Sent' ,'" + ControlMgr.UserId + "' ,getdate() from ReceiptOrder_LogTable where ReceiptOrderNo = '" + txtRoNumber.Text + "' order by LogDate desc";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //BY: HC (E)
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
            if (vBolClose == true)
            {
                MessageBox.Show("RO : " + txtRoNumber.Text + " berhasil digunakan");
                this.Close();
            }
        }

        //public void UpdateRemainingQty(string tmpROID)
        //{
        //    ROID = tmpROID;
        //    decimal QtyBefore = 0;
        //    decimal SeqNo = 0 ;

        //    Conn = ConnectionString.GetConnection();

        //    Query = "Select Count (ReceiptOrderId) from [dbo].[ReceiptOrderD] where ReceiptOrderId = '" + ROID + "'";
        //    Cmd = new SqlCommand(Query, Conn);
        //    int Total = Int32.Parse(Cmd.ExecuteScalar().ToString());


        //    for (int i = 0; i <= Total-1 ; i++)
        //    {
        //        String SeqNo1 = dgvRODetails.Rows[i].Cells["No"].Value == null ? "" : dgvRODetails.Rows[i].Cells["No"].Value.ToString();
        //        String PurchOrderSeqNo1 = dgvRODetails.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvRODetails.Rows[i].Cells["SeqNo"].Value.ToString();

        //        Query = "Select Qty from [dbo].[ReceiptOrderD] where ReceiptOrderId = '" + ROID + "' and PurchaseOrderID = '" + txtPONumber.Text + "' and SeqNo = '" + SeqNo1 + "'";
        //        Cmd = new SqlCommand(Query, Conn, Trans);
        //        Dr = Cmd.ExecuteReader();

        //        while (Dr.Read())
        //        {
        //            QtyBefore = decimal.Parse(Dr["Qty"].ToString());
        //        }
        //        Dr.Close();

        //        Query = "Update [dbo].[PurchDtl] set RemainingQty = (RemainingQty + " + QtyBefore + ") where PurchID = '" + txtPONumber.Text + "' and SeqNo = '" + PurchOrderSeqNo1 + "'";
        //        Cmd = new SqlCommand(Query, Conn, Trans);
        //        Cmd.ExecuteNonQuery();
        //    }

        //    Conn.Close();
        //}

        private void GetUpdatePORemainingQty()
        {
            OldPORemaining.Clear();

            if (txtRoNumber.Text != "")
            {
                for (int i = 0; i < dgvRODetails.RowCount; i++)
                {
                    Query = "Select b.RemainingQty from [dbo].[ReceiptOrderD] a JOIN [dbo].[PurchDtl] b ON (a.PurchaseOrderId = b.PurchID and a.PurchaseOrderSeqNo = b.SeqNo) Where ReceiptOrderID = '" + txtRoNumber.Text + "' AND b.SeqNo = '" + dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value.ToString() + "' ";
                    Conn = ConnectionString.GetConnection();

                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        OldPORemaining.Add(Convert.ToDecimal(Dr["RemainingQty"]));
                        //dgvRODetails.Rows[i].Cells["PORemaining"].Value = Convert.ToDecimal(Dr["RemainingQty"]);
                    }
                    Dr.Close();
                    Conn.Close();


                }
            }
            else
            {
                for (int i = 0; i < dgvRODetails.RowCount; i++)
                {
                    Query = "Select a.RemainingQty ";
                    Query += "From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID ";
                    Query += "LEFT JOIN dbo.[CanvasSheetD] c ON a.ReffId = c.CanvasID and a.ReffId2 = c.PurchQuotId and a.ReffSeqNo = c.CanvasSeqNo ";
                    Query += "LEFT JOIN dbo.[PurchRequisition_Dtl] d ON c.PurchReqId = d.PurchReqId and c.PurchReqSeqNo = d.SeqNo Where a.PurchId = '" + txtPONumber.Text + "' and a.SeqNo = '" + dgvRODetails.Rows[i].Cells["PurchaseOrderSeqNo"].Value.ToString() + "' ;";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            OldPORemaining.Add(Convert.ToDecimal(Dr["RemainingQty"]));
                            //  dgvRODetails.Rows[i].Cells["PORemaining"].Value = Convert.ToDecimal(Dr["RemainingQty"]);
                        }
                        Dr.Close();
                    }
                    Conn.Close();
                }
            }
        }

        private void btnSOwner_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.FormName = "Search Vendor";
            tmpSearch.PrimaryKey = "VendId";
            tmpSearch.Order = "VendId asc";
            tmpSearch.Table = "[dbo].[VendTable]";
            tmpSearch.QuerySearch = "SELECT VendId,VendName, Gol_Prsh from VendTable ";
            tmpSearch.FilterText = new string[] { "VendId", "VendName" };
            //tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
            tmpSearch.Select = new string[] { "VendId", "VendName" };
            //tmpSearch.Hide = new string[] { "VendId" };
            tmpSearch.WherePlus = " and Gol_Prsh = 'EXPEDISI'";
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                tbxVOwnerID.Text = ConnectionString.Kodes[0];
                tbxVOwner.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }
        }

        private void cbVOwner_CheckedChanged(object sender, EventArgs e)
        {
            if (cbVOwner.Checked == true)
            {
                tbxVOwnerID.Text = txtVendID.Text;
                tbxVOwner.Text = txtVendName.Text;
                btnSOwner.Enabled = false;
            }
            else
            {
                tbxVOwnerID.Text = "";
                tbxVOwner.Text = "";
                btnSOwner.Enabled = true;
            }
        }

        private void dgvRODetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            for (int i = 0; i < dgvRODetails.Columns.Count; i++)
            {
                if (dgvRODetails.Columns[i].ReadOnly == true)
                {
                    dgvRODetails.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
                else
                {
                    dgvRODetails.Columns[i].DefaultCellStyle.BackColor = Color.White;
                }
            }
            if (txtRoNumber.Text != "" && txtRoNumber.Text != null)
            {
                dgvRODetails.Columns["RORemaining"].Visible = true;
            }
            //BY: HC (S)
            if (dgvRODetails.Columns[e.ColumnIndex].Name.Contains("Qty") || dgvRODetails.Columns[e.ColumnIndex].Name.Contains("Qty_Alt") || dgvRODetails.Columns[e.ColumnIndex].Name.Contains("DiscPercent") || dgvRODetails.Columns[e.ColumnIndex].Name.Contains("Remaining"))
            {
                if (e.Value == "" || e.Value == null || e.Value == (object)DBNull.Value)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            if (dgvRODetails.Columns[e.ColumnIndex].Name.Contains("Amount") || dgvRODetails.Columns[e.ColumnIndex].Name.Contains("Price") || dgvRODetails.Columns[e.ColumnIndex].Name.Contains("Total"))
            {
                if (e.Value == "" || e.Value == null || e.Value == (object)DBNull.Value)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dgvRODetails.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            } if (dgvRODetails.Columns[e.ColumnIndex].Name.Contains("Ratio") || dgvRODetails.Columns[e.ColumnIndex].Name.Contains("ConvertionRatio"))
            {
                if (e.Value == "" || e.Value == null || e.Value == (object)DBNull.Value)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }
            if (dgvRODetails.Columns[e.ColumnIndex].Name.Contains("Date"))
                dgvRODetails.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
            //BY: HC (E)

        }

        private void dgvRODetails_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void cmbDelivMethod_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cmbDelivMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BY: HC (S)
            if (Mode == "New")
                GetGridDataModeNew();
            else
                GetDataHeader();
            //BY: HC (E)
        }
    }
}
