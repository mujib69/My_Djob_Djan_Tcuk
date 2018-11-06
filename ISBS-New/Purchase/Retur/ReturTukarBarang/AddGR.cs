using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.ReturTukarBarang
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

        String Query;
        Purchase.Retur.ReturTukarBarang.RTBHeader Parent;

        public AddGR()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.ReturTukarBarang.RTBHeader F)
        {
            Parent = F;
        }

        private void AddGR_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select Distinct h.GoodsReceivedId, h.GoodsReceivedDate, [VendId], [VendorName], h.Notes,SiteName,SiteLocation From [GoodsReceivedH] h LEFT JOIN [GoodsReceivedD] d ON h.GoodsReceivedId = d.GoodsReceivedId Where h.[GoodsReceivedStatus] = '03' And d.ActionCodeStatus = '05'";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvItem.AutoGenerateColumns = true;
            dgvItem.DataSource = Dt;
            dgvItem.Refresh();
            dgvItem.ReadOnly = true;
            dgvItem.AutoResizeColumns();

            Conn.Close();
        }

        private void SelectGR()
        {
            String GRNumber = "";

            GRNumber = (dgvItem.CurrentRow.Cells["GoodsReceivedId"].Value == null ? "" : dgvItem.CurrentRow.Cells["GoodsReceivedId"].Value.ToString());

            Parent.AddGR(GRNumber);

            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectGR();
        }

        private void dgvItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >-1)
            {
                SelectGR();
            }
        }
    }
}
