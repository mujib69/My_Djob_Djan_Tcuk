using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp
{
    public partial class InventSite : MetroFramework.Forms.MetroForm
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
        private string InventSiteID;
        private string FullItemId;


        public InventSite()
        {
            InitializeComponent();
        }

        private void InventSite_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            dgvStockInvent.AutoResizeColumns();
        }

        public void GetData(string InventSiteID, string FullItemId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select InventSiteID, InventSiteName, Deskripsi, SiteType, Lokasi, Alamat1, Alamat2, Province, Kota, Area_Code, RT, RW, Luas, JumlahBlok";
            Query += " From [dbo].[InventSite] Where InventSiteID='"+InventSiteID+"' ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                LblInventSiteID.Text = " " + Dr["InventSiteID"].ToString();
                LblInventSiteName.Text = " " + Dr["InventSiteName"].ToString();
                LblDesc.Text = " " + Dr["Deskripsi"].ToString();
                LblSiteType.Text = " " + Dr["SiteType"].ToString();
                LblLocation.Text = " " + Dr["Lokasi"].ToString();
                LblAddress1.Text = " " + Dr["Alamat1"].ToString();
                LblAddress2.Text = " " + Dr["Alamat2"].ToString();
                LblProv.Text = " " + Dr["Province"].ToString();
                LblCity.Text = " " + Dr["Kota"].ToString();
                LblAreaCode.Text = " " + Dr["Area_Code"].ToString();
            }
            Dr.Close();

            dgvStockInvent.DataSource = null;
            if (dgvStockInvent.RowCount==0)
            {
                dgvStockInvent.Rows.Clear();
                dgvStockInvent.ColumnCount = 9;
                dgvStockInvent.Columns[0].Name = "No";
                dgvStockInvent.Columns[1].Name = "InventSiteID";
                dgvStockInvent.Columns[2].Name = "InventSiteName";
                dgvStockInvent.Columns[3].Name = "FullItemId";
                dgvStockInvent.Columns[4].Name = "ItemName";
                dgvStockInvent.Columns[5].Name = "Available_For_Sale_UoM";
                dgvStockInvent.Columns[6].Name = "Available_For_Sale_Reserved_UoM";
                dgvStockInvent.Columns[7].Name = "Available_For_Sale_Alt";
                dgvStockInvent.Columns[8].Name = "Available_For_Sale_Reserved_Alt";
            }
            Query = "SELECT a.InventSiteID,b.InventSiteName,a.FullItemId, a.ItemName, a.Available_For_Sale_UoM, a.Available_For_Sale_Reserved_UoM, a.Available_For_Sale_Alt, a.Available_For_Sale_Reserved_Alt ";
            Query += "FROM [dbo].[Invent_OnHand_Qty] AS a left join [dbo].[InventSite] as b on a.InventSiteId = b.InventSiteID ";
            Query += "  Where b.InventSiteID='" + InventSiteID + "' and a.FullItemId='" + FullItemId + "' ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int j = 1;
            while (Dr.Read())
            {
                this.dgvStockInvent.Rows.Add(j, Dr["InventSiteID"], Dr["InventSiteName"], Dr["FullItemId"], Dr["ItemName"], Dr["Available_For_Sale_UoM"], Dr["Available_For_Sale_Reserved_UoM"], Dr["Available_For_Sale_Alt"], Dr["Available_For_Sale_Reserved_Alt"]);
                j++;
            }
            Dr.Close();
            dgvStockInvent.AutoResizeColumns();
            dgvStockInvent.ReadOnly = true;

        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
