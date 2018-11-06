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
    public partial class AddWH : MetroFramework.Forms.MetroForm
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

        public AddWH()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddWH_Load(object sender, EventArgs e)
        {
            addCmbCriteria();
            Conn = ConnectionString.GetConnection();

            Query = "SELECT InventSiteID, InventSiteName, Lokasi FROM InventSite";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvWH.AutoGenerateColumns = true;
            dgvWH.DataSource = Dt;
            dgvWH.Refresh();
            dgvWH.ReadOnly = true;
            dgvWH.AutoResizeColumns();

            Conn.Close();
        }

        public void setParent(Purchase.NotaReturBeli.ReturBeliHeader F)
        {
            Parent = F;
        }

        private void dgvItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectGR();
            }
        }

        private void SelectGR()
        {
            string WHCode = "";
            WHCode = (dgvWH.CurrentRow.Cells["InventSiteID"].Value == null ? "" : dgvWH.CurrentRow.Cells["InventSiteID"].Value.ToString());
            Parent.AddWH(WHCode);
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectGR();
        }

        private void addCmbCriteria()
        {
            cmbCriteria.Items.Add("All");
            cmbCriteria.Items.Add("InventSiteName");
            cmbCriteria.Items.Add("Lokasi");
            cmbCriteria.SelectedIndex = 0;
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT InventSiteID, InventSiteName, Lokasi FROM InventSite ";

            if (crit == null)
            {
                Query += "";
            }
            else if (crit.Equals("All"))
            {
                Query += "WHERE InventSiteName LIKE '%" + txtCrit.Text + "%' OR Lokasi LIKE '%" + txtCrit.Text + "%' ";
            }
            else if (crit.Equals("InventSiteName"))
            {
                Query += "WHERE InventSiteName LIKE '%" + txtCrit.Text + "%' ";
            }
            else if (crit.Equals("Lokasi"))
            {
                Query += "WHERE Lokasi LIKE '%" + txtCrit.Text + "%' ";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvWH.AutoGenerateColumns = true;
            dgvWH.DataSource = Dt;
            dgvWH.Refresh();
            dgvWH.AutoResizeColumns();
            Conn.Close();
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
    }
}
