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

namespace ISBS_New.PopUp.DeliveryOrder
{
    public partial class Deliveryorder : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        //private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataReader DrD;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        private int Index;
        private string DONo;

        public Deliveryorder()
        {
            InitializeComponent();
        }

        private void Deliveryorder_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
        }
        public void GetData(string DONo)
        {
            Conn=ConnectionString.GetConnection();
            Query="Select DeliveryOrderId, DeliveryOrderDate, DeliveryOrderStatus, SalesOrderId, SalesOrderDate, DeliveryDate, CustID, CustName,InventSiteID, InventSiteName, VehicleOwner, VehicleType, VehicleNumber, DriverName, InventSiteID , Notes ";
            Query+="From [dbo].[DeliveryOrderH] Where DeliveryOrderId='"+DONo+"' ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lblDONo.Text = " " + Dr["DeliveryOrderId"].ToString();
                dtDeliveryOrderDate.Text = " " + Dr["DeliveryOrderDate"].ToString();
                lblSONo.Text = " " + Dr["SalesOrderId"].ToString();
                dtSODate.Text = " " + Dr["SalesOrderDate"].ToString();
                lblCustomer.Text = " " + Dr["CustID"].ToString();
                lblName.Text = " " + Dr["CustName"].ToString();
                dtDate.Text = " " + Dr["DeliveryDate"].ToString();
                lblDOStatus.Text = " " + Dr["DeliveryOrderStatus"].ToString();
                lblWarehouse.Text = " " + Dr["InventSiteID"].ToString();
                LblWarehouseName.Text = " " + Dr["InventSiteName"].ToString();
                lblVehicleOwner.Text = " " + Dr["VehicleOwner"].ToString();
                lblVehicleType.Text = " " + Dr["VehicleType"].ToString();
                lblVehicleNumber.Text = " " + Dr["VehicleNumber"].ToString();
                lblDriverName.Text = " " + Dr["DriverName"].ToString();
                rtbNotes.Text = " " + Dr["Notes"].ToString();
                
            }
            Dr.Close();
            dtDate.Enabled = false;
            dtDeliveryOrderDate.Enabled = false;
            dtSODate.Enabled = false;
            rtbNotes.Enabled = false;

            dgvItemOverview.DataSource = null;
            if (dgvItemOverview.RowCount==0)
            {
                dgvItemOverview.Rows.Clear();
                dgvItemOverview.ColumnCount = 6;
                dgvItemOverview.Columns[0].Name = "No";
                dgvItemOverview.Columns[1].Name = "FullItemId";
                dgvItemOverview.Columns[2].Name = "ItemName";
                dgvItemOverview.Columns[3].Name = "Qty";
                dgvItemOverview.Columns[4].Name = "RemainingQty";
                dgvItemOverview.Columns[5].Name = "Unit";
            }

            dgvItemDetail.DataSource = null;
            if (dgvItemDetail.RowCount==0)
            {
                dgvItemDetail.Rows.Clear();
                dgvItemDetail.ColumnCount = 9;
                dgvItemDetail.Columns[0].Name = "No";
                dgvItemDetail.Columns[1].Name = "FullItemId";
                dgvItemDetail.Columns[2].Name = "ItemName";
                dgvItemDetail.Columns[3].Name = "SO Qty";
                dgvItemDetail.Columns[4].Name = "SO Remaining Qty";
                dgvItemDetail.Columns[5].Name = "Qty Available For Sale";
                dgvItemDetail.Columns[6].Name = "SO Reserved Qty";
                dgvItemDetail.Columns[7].Name = "DO Qty";
                dgvItemDetail.Columns[8].Name = "Unit";

            }

            Query = "select a.SeqNo, a.SalesOrderSeqNo, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.FullItemID, a.ItemName, b.Qty 'SO_Qty', b.RemainingQty 'SO_RemainingQty', c.Available_For_Sale_UoM, 0, d.Lock_Qty, 0, a.Qty, a.RemainingQty, a.Unit, a.Qty_Alt, a.Unit_Alt, a.ConvertionRatio, b.SalesOrderNo, a.NRJ_Id, b.Qty_return from DeliveryOrderH DOH left join DeliveryOrderD a on DOH.DeliveryOrderId = a.DeliveryOrderId left join SalesOrderD b on a.SalesOrderId = b.SalesOrderNo and a.SalesOrderSeqNo = b.SeqNo left join Invent_OnHand_Qty c on a.FullItemID = c.FullItemId and DOH.InventSiteID = c.InventSiteId left join InventLockTable d on d.RefTransId = b.SalesOrderNo and d.RefTrans_SeqNo = b.SeqNo and d.SiteId = DOH.InventSiteID where a.DeliveryOrderId='" + DONo + "' and a.Closed = 'N'"; //and a.Qty != '0'
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int j = 1;
            int k = 1;
            while (Dr.Read())
            {
                //TAB ITEM OVERVIEW
                dgvItemOverview.Rows.Add(j , Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"]);
                j++;
               // dgvItemDetail.Rows.Add(k, Dr["SeqNo"], Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Available_For_Sale_UoM"], Dr["Available_UoM"], Dr["Lock_Qty"], Dr["Available_For_Sale_Reserved_UoM"], 0, Dr["Unit"], 0, Dr["Unit_Alt"], Dr["ConvertionRatio"]);
               //k++;
            }
            Dr.Close();
            dgvItemOverview.ReadOnly = true;
            dgvItemOverview.AutoResizeColumns();
        }
    }
}
