using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Sales.NotaReturJual
{
    public partial class AddGI : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query, crit = null;
        Sales.NotaReturJual.NRJHeader Parent;

        public AddGI()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectGI();
        }

        private void AddGI_Load(object sender, EventArgs e)
        {
            addCmbCriteria();
            Conn = ConnectionString.GetConnection();

            //Query = "SELECT DISTINCT GIH.GoodsIssuedId, SOD.SalesOrderNo, GID.ItemName, GID.Remaining_Qty, GIH.AccountNum, GIH.AccountName, DOD.DeliveryOrderId, GIH.GoodsIssuedDate ";
            //Query += "FROM GoodsIssuedH GIH LEFT JOIN GoodsIssuedD GID ON GIH.GoodsIssuedId = GID.GoodsIssuedId LEFT JOIN DeliveryOrderD DOD ON GIH.RefTransID = DOD.DeliveryOrderId ";
            //Query += "LEFT JOIN SalesOrderD SOD ON DOD.SalesOrderId = SOD.SalesOrderNo WHERE GIH.StatusCode = '03' AND GID.ActionCode = '01' AND GID.Remaining_Qty <> 0 ";

            Query = "SELECT ROW_NUMBER() OVER(ORDER BY A.[GI Date] DESC) AS [No], * FROM (SELECT DISTINCT GIH.GoodsIssuedId AS [GI No], GIH.GoodsIssuedDate AS [GI Date], ";
            Query += "GIH.AccountName AS [Customer], IST.InventSiteName AS [Warehaouse], GID.FullItemId AS [FullItemId], GID.ItemName AS [Item Name], GID.Qty AS [Qty], ";
            Query += "GID.Remaining_Qty AS [Remaining Qty], GID.Unit AS [Unit] FROM GoodsIssuedH GIH LEFT JOIN GoodsIssuedD GID ON GIH.GoodsIssuedId = GID.GoodsIssuedId ";
            Query += "LEFT JOIN InventSite IST ON IST.InventSiteID = GID.InventSiteId WHERE GIH.StatusCode = '03' AND GID.ActionCode = '01' AND GID.Remaining_Qty <> 0 ) A ";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGI.AutoGenerateColumns = true;
            dgvGI.DataSource = Dt;
            dgvGI.Refresh();
            dgvGI.ReadOnly = true;
            dgvGI.AutoResizeColumns();
            dgvGI.Columns["GI Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();
        }

        public void setParent(Sales.NotaReturJual.NRJHeader F)
        {
            Parent = F;
        }

        private void SelectGI()
        {
            string GINumber = "";
            GINumber = (dgvGI.CurrentRow.Cells["GI No"].Value == null ? "" : dgvGI.CurrentRow.Cells["GI No"].Value.ToString());
            Parent.AddGI(GINumber);
            this.Close();
        }

        private void dgvGI_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectGI();
            }
        }

        private void addCmbCriteria()
        {
            cmbCriteria.Items.Add("All");
            cmbCriteria.Items.Add("GI No");
            cmbCriteria.Items.Add("Customer");
            cmbCriteria.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT ROW_NUMBER() OVER(ORDER BY A.[GI Date] DESC) AS [No], * FROM (SELECT DISTINCT GIH.GoodsIssuedId AS [GI No], GIH.GoodsIssuedDate AS [GI Date], ";
            Query += "GIH.AccountName AS [Customer], IST.InventSiteName AS [Warehaouse], GID.FullItemId AS [FullItemId], GID.ItemName AS [Item Name], GID.Qty AS [Qty], ";
            Query += "GID.Remaining_Qty AS [Remaining Qty], GID.Unit AS [Unit] FROM GoodsIssuedH GIH LEFT JOIN GoodsIssuedD GID ON GIH.GoodsIssuedId = GID.GoodsIssuedId ";
            Query += "LEFT JOIN InventSite IST ON IST.InventSiteID = GID.InventSiteId WHERE GIH.StatusCode = '03' AND GID.ActionCode = '01' AND GID.Remaining_Qty <> 0 ) A ";
            if (crit == null)
            {
                Query += "";
            }
            else if (crit.Equals("All"))
            {
                Query += "WHERE A.[GI No] LIKE '%" + txtCrit.Text + "%' OR A.[Customer] LIKE '%" + txtCrit.Text + "%' ";
            }
            else if (crit.Equals("GI No"))
            {
                Query += "WHERE A.[GI No] LIKE '%" + txtCrit.Text + "%' ";
            }
            else if (crit.Equals("Customer"))
            {
                Query += "WHERE A.[Customer] LIKE '%" + txtCrit.Text + "%' ";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGI.AutoGenerateColumns = true;
            dgvGI.DataSource = Dt;
            dgvGI.Refresh();
            dgvGI.AutoResizeColumns();
            dgvGI.Columns["GI Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();
        }
    }
}
