using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.GoodReceiptNT
{
    public partial class PopUpGRNT : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;

        public PopUpGRNT()
        {
            InitializeComponent();
        }

        private void PopUpGRNT_Load(object sender, EventArgs e)
        {
            ModeLoad();
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
            RefreshGrid();
        }

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox] ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            cmbShow.Items.Clear();
            while (Dr.Read())
            {
                cmbShow.Items.Add(Dr.GetInt32(0));
            }
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShow.SelectedIndex = 0;
        }

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            Query = "select * from ( Select ROW_NUMBER() OVER (ORDER BY a.GoodsReceivedId DESC) [No], a.GoodsReceivedId  from ( Select distinct(a.GoodsReceivedId) from [dbo].[GoodsReceivedH] as a left join [dbo].[GoodsReceivedD] as b on a.[GoodsReceivedId] = b.[GoodsReceivedId] left join InventResizeD as c on a.GoodsReceivedId = c.RefTransId left join InventResizeH as d on c.TransId = d.TransId where a.[GoodsReceivedStatus] = '03' and b.FullItemId = ANY (Select a.FullItemId from ResizeTableD as a left join ResizeTableH as b on a.TransId = b.TransId where b.Posted != '2' ) and a.GoodsReceivedId not in (select a.RefTransId from InventResizeD as a left join InventResizeH as b on a.TransId = b.TransId where b.Posted != '02') and c.TransId is null and a.GoodsReceivedId LIKE '%" + txtSearch.Text + "%' ) as a ) as a ";
            Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);
            dgvSearch.DataSource = Dt;
            dgvSearch.AutoResizeColumns();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
            Query = "Select count(distinct(a.GoodsReceivedId)) from [dbo].[GoodsReceivedH] as a left join [dbo].[GoodsReceivedD] as b on a.[GoodsReceivedId] = b.[GoodsReceivedId] left join InventResizeD as c on a.GoodsReceivedId = c.RefTransId left join InventResizeH as d on c.TransId = d.TransId where a.[GoodsReceivedStatus] = '03' and b.FullItemId = ANY (Select a.FullItemId from ResizeTableD as a left join ResizeTableH as b on a.TransId = b.TransId where b.Posted != '2' ) and a.GoodsReceivedId not in (select a.RefTransId from InventResizeD as a left join InventResizeH as b on a.TransId = b.TransId where b.Posted != '02') and c.TransId is null and a.GoodsReceivedId LIKE '%" + txtSearch.Text + "%';";

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (cmbShow.Text == String.Empty)
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse("10"));
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            RefreshGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }

            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        public static string grID;
        private void dgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectGR();
        }

        #region SelectGR
        private void SelectGR()
        {
            if (dgvSearch.RowCount > 0)
            {
                grID = dgvSearch.CurrentRow.Cells["GoodsReceivedId"].Value.ToString();
                this.Close();
            }
        }
        #endregion

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectGR();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
