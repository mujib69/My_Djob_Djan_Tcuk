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

//BY: HC 19.04.2018
namespace ISBS_New.Inventory.StockView
{
    public partial class StockViewHeader : MetroFramework.Forms.MetroForm
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
        string[] tableCols = new string[] {  };
        /*********datagridview cols name*********/

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        bool check; //label
        private string msg; //Validation
        /*********VALIDATION*********/

        //GV POP UP 
        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        DataGridViewComboBoxCell cell; //COMBOBOX CELLVALUE
        private SqlDataReader Dr2; //COMBOBOX CELLVALUE

        /*********PARENT*********/
        //SQInq Parent = new SQInq();
        //public void SetParent(SQInq F) { Parent = F; }

        DateTimePicker dtp = new DateTimePicker(); //DATE IN GRIDVIEW1 

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public StockViewHeader()
        {
            InitializeComponent();
            tabHeader.SelectedTab = tabOnHand;
            tabDetail.SelectedTab = tabOnHandDetail;
            GetDataHeader();
            gvFormat();
        }

        private void GetDataHeader()
        {
            Conn = ConnectionString.GetConnection();
            PassDataSource("Invent_OnHand_Qty", gvOnHand, "FullItemId asc", "", "*");//GroupId, SubGroupId, SubGroup2Id, FullItemId, ItemId, ItemName, InventSiteId, Available_UoM 'AvailableForSale', Available_For_Sale_Reserved_UoM 'Reserved', Available_For_Sale_Amount 'InFlow', Available_For_Sale_Reserved_Amount 'OutFlow'");
            PassDataSource("Invent_Purchase_Qty", gvPurchase, "FullItemId asc", "", "*");
            PassDataSource("Invent_Sales_Qty", gvSales, "FullItemId asc", "", "*");
            PassDataSource("Invent_Movement_Qty", gvMovement, "FullItemId asc", "", "*");
            PassDataSource("Invent_OnHand_Qty a", gvOnHand2, "FullItemId asc", "", "GroupId, SubGroupId, SubGroup2Id, FullItemId, ItemId, ItemName, InventSiteId, Available_UoM, Available_For_Sale_UoM, Available_For_Sale_Reserved_UoM,(select sum(case when Available_Uom >0 then Available_Uom else 0 end) from InventTrans b where b.FullItemId=a.FullItemId and a.InventSiteId=b.InventSiteId) InFlow, (select sum(case when Available_Uom <0 then Available_Uom else 0 end) from InventTrans b where b.FullItemId=a.FullItemId and a.InventSiteId=b.InventSiteId )OutFlow");

            gvOnHand2.Columns["InventSiteId"].HeaderText = "Warehouse";
            gvOnHand2.Columns["Available_UoM"].HeaderText = "Warehouse Stock";
            gvOnHand2.Columns["Available_For_Sale_UoM"].HeaderText = "Available For Sale";
            gvOnHand2.Columns["Available_For_Sale_Reserved_UoM"].HeaderText = "Reserved";
            
            PassDataSource("InventTrans", gvOnHandDetail, "TransDate desc", "and FullItemId = '" + gvOnHand.Rows[0].Cells["FullItemId"].Value + "' and InventSiteId = '" + gvOnHand.Rows[0].Cells["InventSiteId"].Value + "'", "*");
            Conn.Close();
        }

        private void gvFormat()
        {
            //HEADER
            //GV ON HAND 
            gvColumnsHide(gvOnHand);
            gvOnHandDetail.Columns["Notes"].Visible = false;
            gvOnHandDetail.Columns["RecID"].Visible = false;
            gvColumnsHide(gvPurchase);
            gvColumnsHide(gvSales);
            gvColumnsHide(gvMovement);

            //GV ON HAND DETAIL
            gvColumnsHide(gvOnHandDetail);
            //gvColumnsHide(gvPurchaseDetail);
            //gvColumnsHide(gvSalesDetail);
            //gvColumnsHide(gvMovementDetail);
        }

        private void gvColumnsHide(DataGridView gv)
        {
            gv.Columns["GroupId"].Visible = false;
            gv.Columns["SubGroupId"].Visible = false;
            gv.Columns["SubGroup2Id"].Visible = false;
            gv.Columns["ItemId"].Visible = false;
        }

        private void PassDataSource(string tableName, DataGridView gv, string orderField, string where, string column)
        {
            string tmpFieldName = "";
            if (tableName == "Invent_Purchase_Qty")
            {
                Query = "SELECT COLUMN_NAME FROM [ISBS-NEW4].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'invent_purchase_qty'";
                using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        string Col = (string)Dr[0];


                        if (Col == "GroupId" || Col == "SubGroupId" || Col == "SubGroup2Id" || Col == "ItemId" || Col == "ItemName" || Col == "FullItemID")
                            {
                                if (tmpFieldName == "")
                                {
                                    tmpFieldName = (string)Dr[0];
                                }
                                else
                                {
                                    tmpFieldName += "," + (string)Dr[0];
                                }
                            }
                        
                        else
                        {
                            tmpFieldName += "," + "CASE WHEN " + (string)Dr[0] + " IS NULL THEN 0 ELSE " + (string)Dr[0] + " END AS " + (string)Dr[0];
                        }
                        
                    }
                }
            }
            else { tmpFieldName = column; }


            Query = "Select ROW_NUMBER() OVER (ORDER BY a." + orderField + ") No, * from ( select " + tmpFieldName + " from " + tableName + " where 1=1 " + where + " )a";
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);
            gv.DataSource = Dt;
        }

        private void gvOnHand_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            PassDataSource("InventTrans", gvOnHandDetail, "TransDate desc", "and FullItemId = '" + gvOnHand.Rows[gvOnHand.CurrentRow.Index].Cells["FullItemId"].Value + "' and InventSiteId = '" + gvOnHand.Rows[gvOnHand.CurrentRow.Index].Cells["InventSiteId"].Value + "'", "*");//TransId, TransDate, Ref_TransId, Availablle_Amount, Available_For_Sale Amount, 
            Conn.Close();
        }

        private void gvPurchase_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gvPurchase.Columns[e.ColumnIndex].Name == "PR_Issued_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_Issued_Alt" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_Issued_Amount")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Issued_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Issued_Alt"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Issued_Amount"].Selected = true;

                    PassDataSource("PurchRequisition_LogTable a left join PurchRequisition_Dtl b on a.PurchReqID = b.PurchReqId and a.PurchReqSeqNo = b.SeqNo left join Invent_Purchase_Qty c on c.FullItemID = b.FullItemId", gvPurchaseDetail, "LogDate desc", "and a.LogStatusCode = '01' and c.FullItemId = '" + gvPurchase.Rows[gvPurchase.CurrentRow.Index].Cells["FullItemId"].Value + "'", "a.*");
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "PR_Approved_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_Approved_Alt" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_Issued_Amount")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Approved_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Approved_Alt"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Approved_Amount"].Selected = true;

                    PassDataSource("PurchRequisition_LogTable a left join PurchRequisition_Dtl b on a.PurchReqID = b.PurchReqId and a.PurchReqSeqNo = b.SeqNo left join Invent_Purchase_Qty c on c.FullItemID = b.FullItemId", gvPurchaseDetail, "LogDate desc", "and a.LogStatusCode in ('03', '04') and c.FullItemId = '" + gvPurchase.Rows[gvPurchase.CurrentRow.Index].Cells["FullItemId"].Value + "'", "a.*");
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "PR_Approved2_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_Approved2_Alt" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_Approved2_Amount")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Approved2_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Approved2_Alt"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_Approved2_Amount"].Selected = true;

                    PassDataSource("PurchRequisition_LogTable a left join PurchRequisition_Dtl b on a.PurchReqID = b.PurchReqId and a.PurchReqSeqNo = b.SeqNo left join Invent_Purchase_Qty c on c.FullItemID = b.FullItemId", gvPurchaseDetail, "LogDate desc", "and a.LogStatusCode in ('13', '14') and c.FullItemId = '" + gvPurchase.Rows[gvPurchase.CurrentRow.Index].Cells["FullItemId"].Value + "'", "a.*");
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "PR_CS_Approved_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_CS_Approved_Alt" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_CS_Approved_Amount")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PR_CS_Approved_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_CS_Approved_Alt"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_CS_Approved_Amount"].Selected = true;

                    PassDataSource("PurchRequisition_LogTable a left join PurchRequisition_Dtl b on a.PurchReqID = b.PurchReqId and a.PurchReqSeqNo = b.SeqNo left join Invent_Purchase_Qty c on c.FullItemID = b.FullItemId", gvPurchaseDetail, "LogDate desc", "and a.LogStatusCode in ('13', '14') and c.FullItemId = '" + gvPurchase.Rows[gvPurchase.CurrentRow.Index].Cells["FullItemId"].Value + "'", "a.*");
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "PR_CS_Approved2_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_CS_Approved2_Alt" || gvPurchase.Columns[e.ColumnIndex].Name == "PR_CS_Approved2_Amount")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PR_CS_Approved2_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_CS_Approved2_Alt"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PR_CS_Approved2_Amount"].Selected = true;
                    //MESTI DI CEK LAGI QUERY NYA BENER APA KAGA
                    //DI TABLE LOM ADA DATA, JD BLM BISA DI CEK
                    PassDataSource("CanvassSheets_LogTable a left join CanvasSheetD b on a.CSID = b.CanvasId and a.CanvasSeqNo = b.CanvasSeqNo left join Invent_Purchase_Qty c on c.FullItemID = b.FullItemId", gvPurchaseDetail, "LogDate desc", "and LogStatusCode='02' and c.FullItemId = '" + gvPurchase.Rows[gvPurchase.CurrentRow.Index].Cells["FullItemId"].Value + "'", "a.*");
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "PO_Issued_Outstanding_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PO_Issued_Outstanding_Alt" || gvPurchase.Columns[e.ColumnIndex].Name == "PO_Issued_Outstanding_Amount")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PO_Issued_Outstanding_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PO_Issued_Outstanding_Alt"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PO_Issued_Outstanding_Amount"].Selected = true;
                    //MESTI DI CEK LAGI QUERY NYA BENER APA KAGA
                    //DI TABLE LOM ADA DATA, JD BLM BISA DI CEK
                    PassDataSource("PO_Issued_LogTable a left join PurchDtl b on a.POId = b.PurchID and a.POSeqNo = b.SeqNo left join Invent_Purchase_Qty c on c.FullItemID = b.FullItemId", gvPurchaseDetail, "LogDate desc", "and c.FullItemId = '" + gvPurchase.Rows[gvPurchase.CurrentRow.Index].Cells["FullItemId"].Value + "'", "a.*");
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "PO_From_PA_Issued_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PO_From_PA_Issued_Alt")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PO_From_PA_Issued_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PO_From_PA_Issued_Alt"].Selected = true;
                    //BELUM PASS DATA SOURCE
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "PO_From_PA_Approved_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PO_From_PA_Approved_Alt")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PO_From_PA_Approved_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PO_From_PA_Approved_Alt"].Selected = true;
                    //BELUM PASS DATA SOURCE
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "PO_From_PA_Approved2_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "PO_From_PA_Approved2_Alt")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["PO_From_PA_Approved2_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["PO_From_PA_Approved2_Alt"].Selected = true;
                    //BELUM PASS DATA SOURCE
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "RO_Issued_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "RO_Issued_Alt" || gvPurchase.Columns[e.ColumnIndex].Name == "RO_Issued_Amount")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["RO_Issued_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["RO_Issued_Alt"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["RO_Issued_Amount"].Selected = true;
                    //NANTI DI CEK DATA YG MUNCUL UDA BENER BLM
                    PassDataSource("ReceiptOrder_LogTable a left join Invent_Purchase_Qty c on c.FullItemID = a.FullItemId", gvPurchaseDetail, "LogDate desc", "and c.FullItemId = '" + gvPurchase.Rows[gvPurchase.CurrentRow.Index].Cells["FullItemId"].Value + "'", "a.*");
                }
            }
            else if (gvPurchase.Columns[e.ColumnIndex].Name == "Retur_Beli_In_Progress_UoM" || gvPurchase.Columns[e.ColumnIndex].Name == "Retur_Beli_In_Progress_Alt" || gvPurchase.Columns[e.ColumnIndex].Name == "Retur_Beli_In_Progress_Amount")
            {
                if (e.RowIndex > -1)
                {
                    gvPurchase.Rows[e.RowIndex].Cells["Retur_Beli_In_Progress_UoM"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["Retur_Beli_In_Progress_Alt"].Selected = true;
                    gvPurchase.Rows[e.RowIndex].Cells["Retur_Beli_In_Progress_Amount"].Selected = true;
                    //NANTI DI CEK DATA YG MUNCUL UDA BENER BLM
                    PassDataSource("NotaReturBeli_LogTable a left join Invent_Purchase_Qty c on c.FullItemID = a.FullItemId", gvPurchaseDetail, "LogDate desc", "and c.FullItemId = '" + gvPurchase.Rows[gvPurchase.CurrentRow.Index].Cells["FullItemId"].Value + "'", "a.*");
                }
            }
        }

        private void gvOnHandDetail_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gvOnHandDetail.Columns[e.ColumnIndex].Name.Contains("UoM") || gvOnHandDetail.Columns[e.ColumnIndex].Name.Contains("Alt") || gvOnHandDetail.Columns[e.ColumnIndex].Name.Contains("Amount"))
            {
                if (e.Value == null)
                {
                    e.Value = "0";
                }
                else
                {
                    if (e.Value.ToString() == "")
                        e.Value = "0";
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                gvOnHandDetail.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (gvOnHandDetail.Columns[e.ColumnIndex].Name.Contains("Date"))
                gvOnHandDetail.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void gvOnHand_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gvOnHand.Columns[e.ColumnIndex].Name.Contains("UoM") || gvOnHand.Columns[e.ColumnIndex].Name.Contains("Alt") || gvOnHand.Columns[e.ColumnIndex].Name.Contains("Amount"))
            {
                if (e.Value == null)
                {
                    e.Value = "0";
                }
                else
                {
                    if (e.Value.ToString() == "")
                        e.Value = "0";
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                gvOnHand.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            //hendry
            //if (gvOnHandDetail.Columns[e.ColumnIndex].Name.Contains("Date"))
                //gvOnHandDetail.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void gvPurchase_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gvPurchase.Columns[e.ColumnIndex].Name.Contains("UoM") || gvPurchase.Columns[e.ColumnIndex].Name.Contains("Alt") || gvPurchase.Columns[e.ColumnIndex].Name.Contains("Amount"))
            {
                if (e.Value == null)
                {
                    e.Value = "0";
                }
                else
                {
                    if (e.Value.ToString() == "")
                        e.Value = "0";
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                gvPurchase.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (gvPurchase.Columns[e.ColumnIndex].Name.Contains("Date"))
                gvPurchase.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void gvSales_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gvSales.Columns[e.ColumnIndex].Name.Contains("UoM") || gvPurchase.Columns[e.ColumnIndex].Name.Contains("Alt") || gvPurchase.Columns[e.ColumnIndex].Name.Contains("Amount"))
            {
                if (e.Value == null)
                {
                    e.Value = "0";
                }
                else
                {
                    if (e.Value.ToString() == "")
                        e.Value = "0";
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                gvPurchase.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (gvPurchase.Columns[e.ColumnIndex].Name.Contains("Date"))
                gvPurchase.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void gvMovement_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gvMovement.Columns[e.ColumnIndex].Name.Contains("UoM") || gvMovement.Columns[e.ColumnIndex].Name.Contains("Alt") || gvMovement.Columns[e.ColumnIndex].Name.Contains("Amount"))
            {
                if (e.Value == null)
                {
                    e.Value = "0";
                }
                else
                {
                    if(e.Value.ToString() == "")
                        e.Value = "0";
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                gvMovement.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (gvMovement.Columns[e.ColumnIndex].Name.Contains("Date"))
                gvMovement.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void tabHeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabDetail.SelectedIndex = tabHeader.SelectedIndex;
        }

        private void gvPurchaseDetail_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gvPurchaseDetail.Columns[e.ColumnIndex].Name.Contains("UoM") || gvPurchaseDetail.Columns[e.ColumnIndex].Name.Contains("Alt") || gvPurchaseDetail.Columns[e.ColumnIndex].Name.Contains("Amount"))
            {
                if (e.Value == null)
                {
                    e.Value = "0";
                }
                else
                {
                    if (e.Value.ToString() == "")
                        e.Value = "0";
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                gvPurchaseDetail.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (gvPurchaseDetail.Columns[e.ColumnIndex].Name.Contains("Date"))
                gvPurchaseDetail.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void gvOnHand2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            PassDataSource("InventTrans a left join InventTable b on a.FullItemId=b.FullItemId ", gvOnHand2Detail, "TransDate desc", "and a.FullItemId = '" + gvOnHand2.Rows[gvOnHand2.CurrentRow.Index].Cells["FullItemId"].Value + "' and a.InventSiteId = '" + gvOnHand2.Rows[gvOnHand2.CurrentRow.Index].Cells["InventSiteId"].Value + "'", "TransId, TransDate, Ref_TransId, case when Available_UoM > 0 then Available_UoM else 0 end 'InFlow', case when Available_UoM < 0 then Available_UoM else 0 end 'OutFlow', Available_UoM-Available_UoM 'Balance', b.UOM 'Unit', Available_For_Sale_Reserved_UoM 'Reserved'");//TransId, TransDate, Ref_TransId, Availablle_Amount, Available_For_Sale Amount, 
            Conn.Close();
            CountBalancegvOnHand2();
        }

        private void gvOnHand2Detail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CountBalancegvOnHand2();
        }

        private void CountBalancegvOnHand2()
        {
            gvOnHand2Detail.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvOnHand2Detail.Columns["TransId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvOnHand2Detail.Columns["TransDate"].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvOnHand2Detail.Columns["Ref_TransId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvOnHand2Detail.Columns["InFlow"].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvOnHand2Detail.Columns["OutFlow"].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvOnHand2Detail.Columns["Balance"].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvOnHand2Detail.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvOnHand2Detail.Columns["Reserved"].SortMode = DataGridViewColumnSortMode.NotSortable;

            decimal TmpBalance = 0;
            for (int i = 0; i < gvOnHand2Detail.Rows.Count; i++)
            {
                gvOnHand2Detail.Rows[i].Cells["Balance"].Value = TmpBalance + Convert.ToDecimal(gvOnHand2Detail.Rows[i].Cells["InFlow"].Value) + Convert.ToDecimal(gvOnHand2Detail.Rows[i].Cells["OutFlow"].Value);
                TmpBalance = Convert.ToDecimal(gvOnHand2Detail.Rows[i].Cells["Balance"].Value);
            }
        }

        private void StockViewHeader_Load(object sender, EventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //tabHeader.SelectedTab = tabOnHand;
            //tabDetail.SelectedTab = tabOnHandDetail;
            GetDataHeader();
            gvFormat();
        }



    }
}
