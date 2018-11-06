using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.NotaReturBeli
{
    public partial class AddGR : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query, crit = null;
        Purchase.NotaReturBeli.ReturBeliHeader Parent;
        
        public AddGR()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectGR();
        }

        private void AddGR_Load(object sender, EventArgs e)
        {
            addCmbCriteria();
            Conn = ConnectionString.GetConnection();

            Query = "SELECT ROW_NUMBER() OVER( ORDER BY A.[GR Date] DESC) AS [No],* FROM (SELECT DISTINCT GRH.GoodsReceivedId AS [GR No], GRH.GoodsReceivedDate AS [GR Date], GRH.VendorName AS [Vendor], ";
            Query += "GRH.SiteName AS [Warehouse], GRD.FullItemId AS [FullItemId], GRD.ItemName AS [Item Name], GRD.Qty AS [Qty], GRD.Remaining_Qty AS [Remaining Qty], GRD.Unit AS [Unit] ";
            Query += "FROM GoodsReceivedH GRH LEFT JOIN GoodsReceivedD GRD ON GRH.GoodsReceivedId = GRD.GoodsReceivedId LEFT JOIN ReceiptOrderD ROD ON GRH.RefTransID = ROD.ReceiptOrderId ";
            Query += "LEFT JOIN PurchH POH ON POH.PurchID = ROD.PurchaseOrderId WHERE (GRH.GoodsReceivedStatus = '03' or GRH.GoodsReceivedStatus = '06') And GRD.ActionCodeStatus = '05' AND GRD.Remaining_Qty <> 0 ) A ";
            
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGR.AutoGenerateColumns = true;
            dgvGR.DataSource = Dt;
            dgvGR.Refresh();
            dgvGR.ReadOnly = true;
            dgvGR.AutoResizeColumns();
            dgvGR.Columns["GR Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();
        }

        public void setParent(Purchase.NotaReturBeli.ReturBeliHeader F)
        {
            Parent = F;
        }

        private void SelectGR()
        {
            string GRNumber = "";
            GRNumber = (dgvGR.CurrentRow.Cells["GR No"].Value == null ? "" : dgvGR.CurrentRow.Cells["GR No"].Value.ToString());
            Parent.AddGR(GRNumber);
            this.Close();
        }

        private void dgvItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectGR();
            }
        }

        private void addCmbCriteria()
        {
            cmbCriteria.Items.Add("All");
            cmbCriteria.Items.Add("GR No");
            cmbCriteria.Items.Add("Vendor");
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
            Query = "SELECT ROW_NUMBER() OVER( ORDER BY A.[GR Date] DESC) AS [No],* FROM (SELECT DISTINCT GRH.GoodsReceivedId AS [GR No], GRH.GoodsReceivedDate AS [GR Date], GRH.VendorName AS [Vendor], ";
            Query += "GRH.SiteName AS [Warehouse], GRD.FullItemId AS [FullItemId], GRD.ItemName AS [Item Name], GRD.Qty AS [Qty], GRD.Remaining_Qty AS [Remaining Qty], GRD.Unit AS [Unit] ";
            Query += "FROM GoodsReceivedH GRH LEFT JOIN GoodsReceivedD GRD ON GRH.GoodsReceivedId = GRD.GoodsReceivedId LEFT JOIN ReceiptOrderD ROD ON GRH.RefTransID = ROD.ReceiptOrderId ";
            Query += "LEFT JOIN PurchH POH ON POH.PurchID = ROD.PurchaseOrderId WHERE GRH.GoodsReceivedStatus = '03' And GRD.ActionCodeStatus = '05' AND GRD.Remaining_Qty <> 0 ) A ";

            if (crit == null)
            {
                Query += "";
            }
            else if (crit.Equals("All"))
            {
                Query += "WHERE A.[GR No] LIKE '%" + txtCrit.Text + "%' OR A.[Vendor] LIKE '%" + txtCrit.Text + "%' ";
            }
            else if (crit.Equals("GR No"))
            {
                Query += "WHERE A.[GR No] LIKE '%" + txtCrit.Text + "%' ";
            }
            else if (crit.Equals("Vendor"))
            {
                Query += "WHERE A.[Vendor] LIKE '%" + txtCrit.Text + "%' ";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGR.AutoGenerateColumns = true;
            dgvGR.DataSource = Dt;
            dgvGR.Refresh();
            dgvGR.AutoResizeColumns();
            dgvGR.Columns["GR Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();
        }
    }
}
