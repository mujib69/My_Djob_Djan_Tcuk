using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.FullItemId
{
    public partial class FullItemId : MetroFramework.Forms.MetroForm
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
        private string FullItemID;

        PopUp.Vendor.Vendor Vendor = null;

        public FullItemId()
        {
            InitializeComponent();
        }

        private void FullItemId_Load(object sender, EventArgs e)
        {
            //if (Purchase.PurchaseRequisition.HeaderPR.FullItemId != null)
            //    LblFullItemId.Text = FullItemID = Purchase.PurchaseRequisition.HeaderPR.FullItemId;
           
            
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            dgvStockItem.AutoResizeColumns();
        }

        public void GetData(string FullItemID)
        {
            Conn = ConnectionString.GetConnection();
            // Query = "Select FullItemID, Ukuran2, Ukuran3, Ukuran4, Ukuran5, Ukuran1Value ,Ukuran2Value ,Ukuran3Value, Ukuran4Value, Ukuran5Value, Ukuran1MeasurementID, Ukuran2MeasurementID, Ukuran3MeasurementID, Ukuran4MeasurementID, Ukuran5MeasurementID, ManufacturerID, MerekID, GolonganID, UoMQty, QualityID, SpecID From [dbo].[InventTable] where FullItemID='" + FullItemID + "'";
            Query = "Select a.FullItemID, a.ItemDeskripsi, a.UoM, a.UomAlt, a.GroupDeskripsi, a.SubGroup1Deskripsi, a.SubGroup2Deskripsi, b.Deskripsi, c.Deskripsi, d.Deskripsi, e.Deskripsi, f.Deskripsi ";
            Query += "from [dbo].[InventTable] AS a left join [dbo].[InventManufacturer] as b on a.[ManufacturerID] = b.[ManufacturerID] left join [dbo].[InventMerek] as c on a.[MerekID] = c.[MerekID] left join [dbo].[InventGolongan] as d on d.[GolonganID] = a.[GolonganID] left join [dbo].[InventSpec] as e on e.[SpecID] = a.[SpecID] left join [dbo].[InventQuality] as f on f.[QualityID] = a.[QualityID] ";
            Query += "where FullItemID='" + FullItemID + "'";
           
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                LblFullItemId.Text =" " + Dr["FullItemID"].ToString();
                lblItemName.Text =" " + Dr["ItemDeskripsi"].ToString();
                LblUoM.Text =" " + Dr["UoM"].ToString();
                LblUoMAlt.Text =" " + Dr["UoMAlt"].ToString();
                LblGroupDesk.Text =" " + Dr["GroupDeskripsi"].ToString();
                LblSubGroup1Desk.Text =" " + Dr["SubGroup1Deskripsi"].ToString();
                LblSubGroup2Desk.Text =" " + Dr["SubGroup2Deskripsi"].ToString();
                lblManufacturer.Text = " " + Dr[7].ToString();
                lblMerek.Text = " " + Dr[8].ToString();
                lblGolongan.Text = " " + Dr[9].ToString();
                lblQuality.Text = " " + Dr[10].ToString();
                lblSpek.Text = " " + Dr[11].ToString();
            }
            Dr.Close();
            //vendor Preference
            dgvVendPreference.DataSource = null;
            if (dgvVendPreference.RowCount==0)
            {
                dgvVendPreference.Rows.Clear();
                dgvVendPreference.ColumnCount = 4;
                dgvVendPreference.Columns[0].Name = "No";
                dgvVendPreference.Columns[1].Name = "VendorPreferenceID1";
                dgvVendPreference.Columns[2].Name = "VendorPreferenceID2";
                dgvVendPreference.Columns[3].Name = "VendorPreferenceID3";
            }
            Query = "SELECT distinct VendorPreferenceID1,VendorPreferenceID2,VendorPreferenceID3 FROM [dbo].[InventTable] ";
            Query += "where FullItemID='" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int i = 1;
            while (Dr.Read())
            {
                this.dgvVendPreference.Rows.Add(i, Dr["VendorPreferenceID1"], Dr["VendorPreferenceID2"], Dr["VendorPreferenceID3"]);
                i++;
            }
          
            Dr.Close();

          
            dgvVendPreference.ReadOnly = true;
            
            //Stock
            dgvStockItem.DataSource = null;
            if (dgvStockItem.RowCount==0)
            {
                dgvStockItem.Rows.Clear();
                dgvStockItem.ColumnCount = 8;
                dgvStockItem.Columns[0].Name = "No";
                dgvStockItem.Columns[1].Name = "FullItemId";
                dgvStockItem.Columns[2].Name = "InventSiteID";
                dgvStockItem.Columns[3].Name = "InventSiteName";
                dgvStockItem.Columns[4].Name = "Available_For_Sale_UoM";
                dgvStockItem.Columns[5].Name = "Available_For_Sale_Reserved_UoM";
                dgvStockItem.Columns[6].Name = "Available_For_Sale_Alt";
                dgvStockItem.Columns[7].Name = "Available_For_Sale_Reserved_Alt";
            }
            Query = "SELECT a.FullItemId, b.InventSiteID, b.InventSiteName, a.Available_For_Sale_UoM, a.Available_For_Sale_Reserved_UoM, a.Available_For_Sale_Alt, a.Available_For_Sale_Reserved_Alt ";
            Query += "FROM [dbo].[Invent_OnHand_Qty] AS a left join [dbo].[InventSite] as b on a.InventSiteId = b.InventSiteID ";
            Query += "where FullItemID='" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int j = 1;
            while (Dr.Read())
            {
                this.dgvStockItem.Rows.Add(j, Dr["FullItemId"],Dr["InventSiteID"], Dr["InventSiteName"], Dr["Available_For_Sale_UoM"], Dr["Available_For_Sale_Reserved_UoM"], Dr["Available_For_Sale_Alt"], Dr["Available_For_Sale_Reserved_Alt"]);
                j++;
            }
            Dr.Close();

         
            dgvStockItem.ReadOnly = true;

            //Price
            dgvPriceItem.DataSource = null;
            if (dgvPriceItem.RowCount==0)
            {
                dgvPriceItem.Rows.Clear();
                dgvPriceItem.ColumnCount = 7;
                dgvPriceItem.Columns[0].Name = "No";
                dgvPriceItem.Columns[1].Name = "FullItemId";
                dgvPriceItem.Columns[2].Name = "PricelistNo";
                dgvPriceItem.Columns[3].Name = "Type";
                dgvPriceItem.Columns[4].Name = "Price";
                dgvPriceItem.Columns[5].Name = "UoM_AvgPrice";
                dgvPriceItem.Columns[6].Name = "Alt_AvgPrice";
                //dgvPriceItem.Columns[7].Name = "TransStatus";
            }
            Query = "SELECT a.FullItemId, b.PricelistNo, b.Type, b.Price, a.UoM_AvgPrice, a.Alt_AvgPrice  ";
            Query += "from [dbo].[InventTable] AS a left join [dbo].[Pricelist_Dtl] as b on a.FullItemId = b.FullItemId left join [dbo].[PricelistH] as c on b.PricelistNo = c.PricelistNo ";
            Query += "where a.FullItemID='" + FullItemID + "' and c.TransStatus=03 ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int k = 1;
            while (Dr.Read())
            {
                this.dgvPriceItem.Rows.Add(k, Dr["FullItemId"], Dr["PricelistNo"], Dr["Type"], Dr["Price"], Dr["UoM_AvgPrice"], Dr["Alt_AvgPrice"]);
                k++;
            }
            Dr.Close();

           
            dgvPriceItem.ReadOnly = true;

            dgvVendPreference.AutoResizeColumns();
            dgvPriceItem.AutoResizeColumns();
            dgvStockItem.AutoResizeColumns();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvVendPreference_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex>-1 && e.ColumnIndex>-1)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    if (dgvVendPreference.Columns[e.ColumnIndex].Name.ToString() == "VendorPreferenceID1")
                    {
                        Vendor = new PopUp.Vendor.Vendor();
                        Vendor.GetData(dgvVendPreference.Rows[e.RowIndex].Cells["VendorPreferenceID1"].Value.ToString());
                        Vendor.Show();
                    }
                    else if (dgvVendPreference.Columns[e.ColumnIndex].Name.ToString() == "VendorPreferenceID2")
                    {
                        Vendor = new PopUp.Vendor.Vendor();
                        Vendor.GetData(dgvVendPreference.Rows[e.RowIndex].Cells["VendorPreferenceID2"].Value.ToString());
                        Vendor.Show();
                    }
                    else if (dgvVendPreference.Columns[e.ColumnIndex].Name.ToString() == "VendorPreferenceID3")
                    {
                        Vendor = new PopUp.Vendor.Vendor();
                        Vendor.GetData(dgvVendPreference.Rows[e.RowIndex].Cells["VendorPreferenceID3"].Value.ToString());
                        Vendor.Show();
                    }

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

        PopUp.InventSite InventSite = null;
        private void dgvStockItem_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
             if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (InventSite == null || InventSite.Text == "")
                {
                    //if (dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "InventSiteID")
                    //{
                    if (dgvStockItem.Columns[e.ColumnIndex].Name.ToString() == "InventSiteID" || dgvStockItem.Columns[e.ColumnIndex].Name.ToString() == "InventSiteName")
                    {
                        InventSite = new PopUp.InventSite();
                        InventSite.GetData(dgvStockItem.Rows[e.RowIndex].Cells["InventSiteID"].Value.ToString(), dgvStockItem.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        InventSite.Show();
                    }
                  
                    //}
                }
                else if (CheckOpened(InventSite.Name))
                {
                    InventSite.WindowState = FormWindowState.Normal;
                     InventSite.GetData(dgvStockItem.Rows[e.RowIndex].Cells["InventSiteID"].Value.ToString(), dgvStockItem.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    InventSite.Show();
                    InventSite.Focus();
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvStockItem.AutoResizeColumns();
            dgvPriceItem.AutoResizeColumns();
            dgvVendPreference.AutoResizeColumns();
        }
    }
}
