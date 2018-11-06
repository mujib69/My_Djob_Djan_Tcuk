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
    public partial class AddVehicle : MetroFramework.Forms.MetroForm
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

        public AddVehicle()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.NotaReturJual.ReturJualHeader F)
        {
            Parent = F;
        }


        private void AddVehicle_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select Distinct VendID, VendName From [VendTable] Where [CompanyGroupID] = 'E'";

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

        private void SelectVehicle()
        {
            String VendorID,VendorName = "";

            VendorID = (dgvItem.CurrentRow.Cells["VendID"].Value == null ? "" : dgvItem.CurrentRow.Cells["VendID"].Value.ToString());
            VendorName = (dgvItem.CurrentRow.Cells["VendName"].Value == null ? "" : dgvItem.CurrentRow.Cells["VendName"].Value.ToString());

            Parent.AddVehicle(VendorID,VendorName);

            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectVehicle();
        }

        private void dgvItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectVehicle();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
