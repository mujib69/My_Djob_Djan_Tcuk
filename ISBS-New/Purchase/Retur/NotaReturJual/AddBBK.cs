using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.NotaReturJual
{
    public partial class AddBBK : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Query;
        Purchase.Retur.NotaReturJual.ReturJualHeader Parent;

        public AddBBK()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.NotaReturJual.ReturJualHeader F)
        {
            Parent = F;
        }

        private void AddBBK_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select Distinct a.GoodsIssuedId, a.GoodsIssuedDate, a.[No_SJ], [AccountNum], AccountName,FullItemID,ItemName From [GoodsIssuedH] a JOIN [GoodsIssuedD] b On a.GoodsIssuedId = b.GoodsIssuedId Where a.[RefTransType] = 'Delivery Order' And a.StatusCode='03'";

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

        private void SelectBBK()
        {
            String BBKNum = "";

            BBKNum = (dgvItem.CurrentRow.Cells["GoodsIssuedId"].Value == null ? "" : dgvItem.CurrentRow.Cells["GoodsIssuedId"].Value.ToString());

            Parent.AddBBK(BBKNum);

            this.Close();

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectBBK();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectBBK();
            }
        }
    }
}
