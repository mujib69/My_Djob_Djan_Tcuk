using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.Stock
{
    public partial class Stock : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private int Index;

        public string FullItemId;

        private string itemID;

        public Stock()
        {
            InitializeComponent();
        }

        private void Stock_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            if(Sales.SalesQuotation.SQHeader2.itemID != null)
                lblItemId.Text = itemID = Sales.SalesQuotation.SQHeader2.itemID;
            if (Purchase.CanvasSheet.FormCanvasSheet2.itemID != null)
                lblItemId.Text = itemID = Purchase.CanvasSheet.FormCanvasSheet2.itemID;
            if (Purchase.PurchaseRequisition.HeaderPR.itemID != null)
                lblItemId.Text = itemID = Purchase.PurchaseRequisition.HeaderPR.itemID;
            if (Purchase.RFQ.RFQForm.itemID != null)
                lblItemId.Text = itemID = Purchase.RFQ.RFQForm.itemID;
            if (Purchase.PurchaseQuotation.FormPQ.itemID != null)
                lblItemId.Text = itemID = Purchase.PurchaseQuotation.FormPQ.itemID;
            if (Purchase.PurchaseAgreement.PAForm.itemID != null)
                lblItemId.Text = itemID = Purchase.PurchaseAgreement.PAForm.itemID;
            if (Purchase.PurchaseOrderNew.POForm.itemID != null)
                lblItemId.Text = itemID = Purchase.PurchaseOrderNew.POForm.itemID;

            Conn = ConnectionString.GetConnection();
            Query = "select A.FullItemID, A.ItemDeskripsi, A.GroupID, A.GroupDeskripsi, A.SubGroup1ID, A.SubGroup1Deskripsi, A.SubGroup2ID, A.SubGroup2Deskripsi, A.Ukuran1Value, A.Ukuran1MeasurementID, A.Ukuran2Value, A.Ukuran2MeasurementID, A.Ukuran3Value, A.Ukuran3MeasurementID, A.Ukuran4Value, A.Ukuran4MeasurementID, A.Ukuran5Value, A.Ukuran5MeasurementID, B.Deskripsi, C.Deskripsi, D.Deskripsi, E.Deskripsi, F.Deskripsi ";
            Query += "from [dbo].[InventTable] AS a left join [dbo].[InventManufacturer] as b on a.[ManufacturerID] = b.[ManufacturerID] left join [dbo].[InventMerek] as c on a.[MerekID] = c.[MerekID] left join [dbo].[InventGolongan] as d on d.[GolonganID] = a.[GolonganID] left join [dbo].[InventSpec] as e on e.[SpecID] = a.[SpecID] left join [dbo].[InventQuality] as f on f.[QualityID] = a.[QualityID] ";
            Query += "where a.[FullItemID] = '" + itemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lblItemId.Text = ": " + Dr["FullItemID"].ToString();
                lblItemName.Text = ": " + Dr["ItemDeskripsi"].ToString();
                lblGroupId.Text = ": " + Dr["GroupID"].ToString() + " , " + Dr["GroupDeskripsi"].ToString();
                lblSubGroup1Id.Text = ": " + Dr["SubGroup1ID"].ToString() + " , " + Dr["SubGroup1Deskripsi"].ToString();
                lblSubGroup2Id.Text = ": " + Dr["SubGroup2ID"].ToString() + " , " + Dr["SubGroup2Deskripsi"].ToString();
                lblUkuran1Value.Text = ": " + Dr["Ukuran1Value"].ToString() + " " + Dr["Ukuran1MeasurementID"].ToString();
                lblUkuran2Value.Text = ": " + Dr["Ukuran2Value"].ToString() + " " + Dr["Ukuran2MeasurementID"].ToString();
                lblUkuran3Value.Text = ": " + Dr["Ukuran3Value"].ToString() + " " + Dr["Ukuran3MeasurementID"].ToString();
                lblUkuran4Value.Text = ": " + Dr["Ukuran4Value"].ToString() + " " + Dr["Ukuran4MeasurementID"].ToString();
                lblUkuran5Value.Text = ": " + Dr["Ukuran5Value"].ToString() + " " + Dr["Ukuran5MeasurementID"].ToString();
                lblManufacturer.Text = ": " + Dr[18].ToString();
                lblMerek.Text = ": " + Dr[19].ToString();
                lblGolongan.Text = ": " + Dr[20].ToString();
                //lblStock.Text = ;
                lblQuality.Text = ": " + Dr[22].ToString();
                lblSpek.Text = ": " + Dr[21].ToString();
            }
        }

        #region Funtion
        public void GetData(string FullItemID)
        {
            FullItemId=FullItemID;
            Conn = ConnectionString.GetConnection();
            Query = "Select AvailableForSale,Locked,GRsInprogress,ReturMasukInProgress,NextExpectedDate,PendingResize,DOsIssued,POsIssued,DOsInProgress,POsInProgress,PAsInProgress,OtherPRsInProgress From [dbo].[InventLogQty]";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lblAvailableForSale.Text = Dr[0].ToString();
                lblLocked.Text = Dr[1].ToString();
                lblGRsInProgress.Text = Dr[2].ToString();
                lblReturMasukInProgress.Text = Dr[3].ToString();
                lblNextExpectedDate.Text = Dr[4].ToString();
                lblPendingResize.Text = Dr[5].ToString();
                lblDOsIssued.Text = Dr[6].ToString();
                lblPOsIssued.Text = Dr[7].ToString();
                lblDOsInProgress.Text = Dr[8].ToString();
                lblPOsInProgress.Text = Dr[9].ToString();
                lblPAsInProgress.Text = Dr[10].ToString();
                lblOtherPRsInProgress.Text = Dr[11].ToString();
            }
            Dr.Close();

            Query = "Select FullItemID,ItemDeskripsi From [dbo].[InventTable] where FullItemID='" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lblItemId.Text = Dr[0].ToString();
                lblItemName.Text = Dr[1].ToString();
            }
        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GetData(FullItemId);
        }

        private void btnOtherPRsInProgress_Click(object sender, EventArgs e)
        {
            SearchQuery tmpSearch = new SearchQuery();
            tmpSearch.PrimaryKey = "PurchReqID";
            tmpSearch.Table = "[dbo].[PurchRequisitionH]";
            tmpSearch.QuerySearch = "Select PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, UpdatedDate FROM [dbo].[PurchRequisitionH]";
            tmpSearch.FilterText = new string[] { "PurchReqID", "TransType", "TransStatus"};
            tmpSearch.FilterDate = new string[] { "OrderDate", "CreatedDate", "UpdatedDate" };
            tmpSearch.Select = new string[] { "PurchReqID"};
            //tmpSearch.WherePlus = "and GroupDeskripsi like '%" + txtItemGroupName.Text.Trim() + "%' and SubGroup1Deskripsi like '%" + txtSubGroup1Name.Text.Trim() + "%' and SubGroup2Deskripsi like '%" + txtSubGroup2Name.Text.Trim() + "%' ";
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                Purchase.PurchaseRequisition.HeaderPR PRHeader = new Purchase.PurchaseRequisition.HeaderPR();
                PRHeader.SetMode("ModeView", ConnectionString.Kodes[0]);
                PRHeader.Show();
                ConnectionString.Kodes = null;
            }
        }

        private void lblSubGroup2Id_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Stock_FormClosed(object sender, FormClosedEventArgs e)
        {
            Purchase.CanvasSheet.FormCanvasSheet2.itemID = null;
            Purchase.PurchaseRequisition.HeaderPR.itemID = null;
            Purchase.RFQ.RFQForm.itemID = null;
            Purchase.PurchaseQuotation.FormPQ.itemID = null;
            Purchase.PurchaseAgreement.PAForm.itemID = null;
            Purchase.PurchaseOrderNew.POForm.itemID = null;
            Sales.SalesQuotation.SQHeader2.itemID = null;
        }
    }
}
