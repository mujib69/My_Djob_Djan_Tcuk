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
    public partial class AddInventSite : MetroFramework.Forms.MetroForm
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

        public AddInventSite()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.NotaReturJual.ReturJualHeader F)
        {
            Parent = F;
        }

        private void AddInventSite_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select Distinct [InventSiteID], [InventSiteName] From [InventSite]";

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

        private void SelectSite()
        {
            String SiteID, SiteName = "";

            SiteID = (dgvItem.CurrentRow.Cells["InventSiteID"].Value == null ? "" : dgvItem.CurrentRow.Cells["InventSiteID"].Value.ToString());
            SiteName = (dgvItem.CurrentRow.Cells["InventSiteName"].Value == null ? "" : dgvItem.CurrentRow.Cells["InventSiteName"].Value.ToString());

            Parent.AddSite(SiteID, SiteName);

            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectSite();
        }

        private void dgvItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectSite();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
