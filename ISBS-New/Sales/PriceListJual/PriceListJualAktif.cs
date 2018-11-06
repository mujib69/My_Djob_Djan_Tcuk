using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Sales.PriceListJual
{
    partial class PriceListJualAktif : MetroFramework.Forms.MetroForm
    {
        Sales.PriceListJual.HeaderPLJ Parent;
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private string FullItemIDHeader = "";
        private string StatusClose = "";

        public PriceListJualAktif()
        {
            InitializeComponent();
            
        }

        public void SetParent(Sales.PriceListJual.HeaderPLJ F)
        {
            Parent = F;
        }

        private void PriceListJualAktif_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            lblForm.Location = new Point(16, 11);            
        }

        private void PriceListJualAktif_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, (63));
        }

        private void PriceListJualAktif_FormClosed(object sender, EventArgs e)
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

            if (dgvPriceListJualAktif.RowCount - 1 <= 0)
            {
                dgvPriceListJualAktif.ColumnCount = 25;
                dgvPriceListJualAktif.Columns[0].Name = "No";
                dgvPriceListJualAktif.Columns[1].Name = "FullItemID";
                dgvPriceListJualAktif.Columns[2].Name = "ItemName";
                dgvPriceListJualAktif.Columns[3].Name = "DeliveryMethod";
                dgvPriceListJualAktif.Columns[4].Name = "Price0D";
                dgvPriceListJualAktif.Columns[5].Name = "Price2D";
                dgvPriceListJualAktif.Columns[6].Name = "Price3D";
                dgvPriceListJualAktif.Columns[7].Name = "Price7D";
                dgvPriceListJualAktif.Columns[8].Name = "Price14D";
                dgvPriceListJualAktif.Columns[9].Name = "Price21D";
                dgvPriceListJualAktif.Columns[10].Name = "Price30D";
                dgvPriceListJualAktif.Columns[11].Name = "Price40D";
                dgvPriceListJualAktif.Columns[12].Name = "Price45D";
                dgvPriceListJualAktif.Columns[13].Name = "Price60D";
                dgvPriceListJualAktif.Columns[14].Name = "Price75D";
                dgvPriceListJualAktif.Columns[15].Name = "Price90D";
                dgvPriceListJualAktif.Columns[16].Name = "Price120D";
                dgvPriceListJualAktif.Columns[17].Name = "Price150D";
                dgvPriceListJualAktif.Columns[18].Name = "Price180D";
                dgvPriceListJualAktif.Columns[19].Name = "Tolerance";
                dgvPriceListJualAktif.Columns[20].Name = "GroupId";
                dgvPriceListJualAktif.Columns[21].Name = "SubGroup1Id";
                dgvPriceListJualAktif.Columns[22].Name = "SubGroup2Id";
                dgvPriceListJualAktif.Columns[23].Name = "ItemId";
                dgvPriceListJualAktif.Columns[24].Name = "PriceListNo";

                dgvPriceListJualAktif.Columns[19].HeaderText = "Tolerance(%)";
            }        

            dgvPriceListJualAktif.ReadOnly = true;

            dgvPriceListJualAktif.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;           
            dgvPriceListJualAktif.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;    
            dgvPriceListJualAktif.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPriceListJualAktif.Columns["PriceListNo"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPriceListJualAktif.Columns["GroupId"].Visible = false;
            dgvPriceListJualAktif.Columns["SubGroup1Id"].Visible = false;
            dgvPriceListJualAktif.Columns["SubGroup2Id"].Visible = false;
            dgvPriceListJualAktif.Columns["ItemId"].Visible = false;
            dgvPriceListJualAktif.Columns["PriceListNo"].Visible = false;

            dgvPriceListJualAktif.AutoResizeColumns();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvPriceListJualAktif.RowCount > 0)
            {
                int Index = dgvPriceListJualAktif.CurrentRow.Index;
                string PriceListNo = dgvPriceListJualAktif.Rows[Index].Cells["PriceListNo"].Value.ToString();
                string FullItemID = dgvPriceListJualAktif.Rows[Index].Cells["FullItemID"].Value.ToString();               
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
