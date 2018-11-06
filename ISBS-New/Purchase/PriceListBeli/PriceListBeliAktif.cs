using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PriceListBeli
{
    partial class PriceListBeliAktif : MetroFramework.Forms.MetroForm
    {
        Purchase.PriceListBeli.HeaderPLB Parent;
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private string FullItemIDHeader = "";
        private string StatusClose = "";

        public PriceListBeliAktif()
        {
            InitializeComponent();
            
        }

        public void SetParent(Purchase.PriceListBeli.HeaderPLB F)
        {
            Parent = F;
        }

        private void PriceListBeliAktif_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);            
        }

        private void PriceListBeliAktif_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, (63));
        }

        private void PriceListBeliAktif_FormClosed(object sender, EventArgs e)
        {
            if (StatusClose == "")
            {
                Parent.SetGridDetail("", FullItemIDHeader);
            }            
            StatusClose = "";
        }

        public void SetHeader(string prmFullItemIDHeader)
        {
            FullItemIDHeader = prmFullItemIDHeader;

            if (dgvPriceListBeliAktif.RowCount - 1 <= 0)
            {
                dgvPriceListBeliAktif.ColumnCount = 25;
                dgvPriceListBeliAktif.Columns[0].Name = "No";
                dgvPriceListBeliAktif.Columns[1].Name = "FullItemID";
                dgvPriceListBeliAktif.Columns[2].Name = "ItemName";
                dgvPriceListBeliAktif.Columns[3].Name = "DeliveryMethod";
                dgvPriceListBeliAktif.Columns[4].Name = "Price0D";
                dgvPriceListBeliAktif.Columns[5].Name = "Price2D";
                dgvPriceListBeliAktif.Columns[6].Name = "Price3D";
                dgvPriceListBeliAktif.Columns[7].Name = "Price7D";
                dgvPriceListBeliAktif.Columns[8].Name = "Price14D";
                dgvPriceListBeliAktif.Columns[9].Name = "Price21D";
                dgvPriceListBeliAktif.Columns[10].Name = "Price30D";
                dgvPriceListBeliAktif.Columns[11].Name = "Price40D";
                dgvPriceListBeliAktif.Columns[12].Name = "Price45D";
                dgvPriceListBeliAktif.Columns[13].Name = "Price60D";
                dgvPriceListBeliAktif.Columns[14].Name = "Price75D";
                dgvPriceListBeliAktif.Columns[15].Name = "Price90D";
                dgvPriceListBeliAktif.Columns[16].Name = "Price120D";
                dgvPriceListBeliAktif.Columns[17].Name = "Price150D";
                dgvPriceListBeliAktif.Columns[18].Name = "Price180D";
                dgvPriceListBeliAktif.Columns[19].Name = "Tolerance";
                dgvPriceListBeliAktif.Columns[20].Name = "GroupId";
                dgvPriceListBeliAktif.Columns[21].Name = "SubGroup1Id";
                dgvPriceListBeliAktif.Columns[22].Name = "SubGroup2Id";
                dgvPriceListBeliAktif.Columns[23].Name = "ItemId";
                dgvPriceListBeliAktif.Columns[24].Name = "PriceListNo";

                dgvPriceListBeliAktif.Columns[19].HeaderText = "Tolerance(%)";
            }        

            dgvPriceListBeliAktif.ReadOnly = true;

            dgvPriceListBeliAktif.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;           
            dgvPriceListBeliAktif.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;    
            dgvPriceListBeliAktif.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListBeliAktif.Columns["PriceListNo"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPriceListBeliAktif.Columns["GroupId"].Visible = false;
            dgvPriceListBeliAktif.Columns["SubGroup1Id"].Visible = false;
            dgvPriceListBeliAktif.Columns["SubGroup2Id"].Visible = false;
            dgvPriceListBeliAktif.Columns["ItemId"].Visible = false;
            dgvPriceListBeliAktif.Columns["PriceListNo"].Visible = false;

            dgvPriceListBeliAktif.AutoResizeColumns();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvPriceListBeliAktif.RowCount > 0)
            {
                int Index = dgvPriceListBeliAktif.CurrentRow.Index;
                string PriceListNo = dgvPriceListBeliAktif.Rows[Index].Cells["PriceListNo"].Value.ToString();
                string FullItemID = dgvPriceListBeliAktif.Rows[Index].Cells["FullItemID"].Value.ToString();               
                Parent.SetGridDetail(PriceListNo, FullItemID);
                StatusClose = "Select";
                this.Close();
            }
            else
            {
                MessageBox.Show("Silahkan pilih data");
                return;
            }          
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Parent.SetGridDetail("", FullItemIDHeader);
            StatusClose = "Exit";
            this.Close();
        }

    }
}
