using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.NotaReturBeli
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
        Purchase.Retur.NotaReturBeli.ReturBeliHeader Parent;
        
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

            //Query = "SELECT * FROM VW_ADDGR_NRB";
            Query = "SELECT DISTINCT GRH.GoodsReceivedId, GRD.Remaining_Qty, ROD.ReceiptOrderId, POH.PurchID, GRH.GoodsReceivedDate, GRH.VendId, GRH.VendorName, GRH.Notes, GRH.SiteName, GRH.SiteLocation ";
            Query += "FROM GoodsReceivedH GRH LEFT JOIN GoodsReceivedD GRD ON GRH.GoodsReceivedId = GRD.GoodsReceivedId LEFT JOIN ReceiptOrderD ROD ON GRH.RefTransID = ROD.ReceiptOrderId LEFT JOIN PurchH POH ON POH.PurchID = ROD.PurchaseOrderId ";
            Query += "WHERE GRH.GoodsReceivedStatus = '03' And GRD.ActionCodeStatus = '05'";
            
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGR.AutoGenerateColumns = true;
            dgvGR.DataSource = Dt;
            dgvGR.Refresh();
            dgvGR.ReadOnly = true;
            dgvGR.AutoResizeColumns();

            Conn.Close();
        }

        public void setParent(Purchase.Retur.NotaReturBeli.ReturBeliHeader F)
        {
            Parent = F;
        }

        private void SelectGR()
        {
            string GRNumber = "";
            GRNumber = (dgvGR.CurrentRow.Cells["GoodsReceivedId"].Value == null ? "" : dgvGR.CurrentRow.Cells["GoodsReceivedId"].Value.ToString());
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
            cmbCriteria.Items.Add("GoodsReceivedId");
            cmbCriteria.Items.Add("ReceiptOrderId");
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
            Query = "SELECT * FROM VW_ADDGR_NRB ";

            if (crit == null)
            {
                Query += "";
            }
            else if (crit.Equals("All"))
            {
                Query += "WHERE GoodsReceivedId LIKE '%" + txtCrit.Text + "%' OR ReceiptOrderId LIKE '%" + txtCrit.Text + "%' ";
            }
            else if (crit.Equals("GoodsReceivedId"))
            {
                Query += "WHERE GoodsReceivedId LIKE '%" + txtCrit.Text + "%' ";
            }
            else if (crit.Equals("ReceiptOrderId"))
            {
                Query += "WHERE ReceiptOrderId LIKE '%" + txtCrit.Text + "%' ";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvGR.AutoGenerateColumns = true;
            dgvGR.DataSource = Dt;
            dgvGR.Refresh();
            dgvGR.AutoResizeColumns();
            Conn.Close();
        }
    }
}
