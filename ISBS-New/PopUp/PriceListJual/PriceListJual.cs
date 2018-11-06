using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.PriceListJual
{
    partial class PriceListJual :  MetroFramework.Forms.MetroForm
    {
        public int Y = 29;
        public string FullItemID;


        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query = "";

        Sales.PriceListJual.HeaderPLJ Parent;

        public PriceListJual()
        {
            InitializeComponent();
        }

        private void PriceList_Load(object sender, EventArgs e)
        {

            this.Location = new Point(148, 47);
            RefreshGrid();
        }
        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT D.[PriceListNo], D.[FullItemID], D.[ItemName], D.[DeliveryMethod], D.[Price0D], D.[Price2D], D.[Price3D], D.[Price7D], D.[Price14D], D.[Price21] AS Price21D, D.[Price30D], D.[Price40D], D.[Price45D], D.[Price60D], D.[Price75D], D.[Price90D], D.[Price120D], D.[Price150D], D.[Price180D] FROM [SalesPriceListDtl] AS D INNER JOIN [SalesPriceListH] AS H ON D.PriceListNo = H.PriceListNo WHERE D.FullItemID = '" + FullItemID + "' AND  H.ValidFrom < GETDATE() AND H.ValidTo > GETDATE() AND StatusCode = '02'";

     
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPriceListJual.AutoGenerateColumns = true;
            dgvPriceListJual.DataSource = Dt;
            dgvPriceListJual.Refresh();

            Conn.Close();

            //GetVendorList();
            dgvPriceListJual.ReadOnly = false;
            dgvPriceListJual.AutoResizeColumns();

        }



        private void PriceList_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, (63));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
    }
}
