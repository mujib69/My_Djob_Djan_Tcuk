using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Sales.PriceListJual
{
    public partial class Gelombang : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query = null;

        string Gelombang1 = "";
        string Bracket = "";
        Sales.PriceListJual.HeaderPLJ Parent;
        

        public Gelombang()
        {
            InitializeComponent();
        }

        private void Gelombang2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            lblForm.Location = new Point(16, 11);
            RefreshGrid();
        }

        public void GetInventStockId(string tmpGelombang, string tmpBracket)
        {
            Gelombang1 = tmpGelombang;
            Bracket = tmpBracket;
        }

        public void SetParentForm(Sales.PriceListJual.HeaderPLJ F)
        {
            Parent = F;
        }      

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            //Query = "Select [GelombangId], [BracketId], ItemId, ItemName, [Base], [Price] From dbo.[InventGelombangD] where GelombangId+BracketId in (Select GelombangId+BracketId From dbo.[InventGelombangD] ";
            Query = "Select b.GroupId, b.SubGroup1Id, b.SubGroup2Id, a.[GelombangId], a.[BracketId], b.ItemId, b.FullItemId, a.ItemName, a.[Base], case when a.Price is null then '0.00' else a.Price end Price, c.VendId ,c.BracketDesc ";
            Query += "From dbo.[InventGelombangD] a Left Join dbo.[InventTable] b on a.[ItemId]=b.FullItemId left join dbo.InventGelombangH c on a.GelombangId=c.GelombangId and a.BracketId=c.BracketId ";
            Query += "Where a.GelombangId='" + Gelombang1 + "' and a.BracketId='" + Bracket + "' order by GelombangId asc, BracketId asc, Base desc ";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvGelombang.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvGelombang.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }
            Da.Fill(Dt);

            dgvGelombang.AutoGenerateColumns = true;
            dgvGelombang.DataSource = Dt;
            dgvGelombang.Refresh();

            dgvGelombang.ReadOnly = false;

            for (int i = 0; i < dgvGelombang.RowCount; i++)
            {
                if (dgvGelombang.Rows[i].Cells["Base"].Value.ToString() == "Y")
                {
                    dgvGelombang.Rows[i].Cells["chk"].Value = true;
                    dgvGelombang.Rows[i].Cells["chk"].ReadOnly = true;
                    dgvGelombang.Rows[i].Cells["chk"].Style.BackColor = Color.LightGray;
                }
            }

            dgvGelombang.Columns["GelombangId"].ReadOnly = true;
            dgvGelombang.Columns["BracketId"].ReadOnly = true;
            dgvGelombang.Columns["ItemId"].ReadOnly = true;
            dgvGelombang.Columns["ItemName"].ReadOnly = true;
            dgvGelombang.Columns["Base"].ReadOnly = true;
            dgvGelombang.Columns["Price"].ReadOnly = true;

            dgvGelombang.Columns["GroupId"].Visible = false;
            dgvGelombang.Columns["SubGroup1Id"].Visible = false;
            dgvGelombang.Columns["SubGroup2Id"].Visible = false;
            dgvGelombang.Columns["ItemId"].Visible = false;
            dgvGelombang.Columns["VendId"].Visible = false;
            dgvGelombang.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvGelombang.AutoResizeColumns();

            Conn.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> GroupId = new List<string>();
            List<string> SubGroup1Id = new List<string>();
            List<string> SubGroup2Id = new List<string>();
            List<string> ItemId = new List<string>();
            List<string> FullItemId = new List<string>();
            List<string> ItemName = new List<string>();
            List<string> GelombangId = new List<string>();
            List<string> BracketId = new List<string>();
            List<string> Base = new List<string>();
            List<string> Price = new List<string>();
            List<string> VendId = new List<string>();
            List<string> BracketDesc = new List<string>();

            int CountChk = 0;
            for (int i = 0; i <= dgvGelombang.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvGelombang.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    GroupId.Add(dgvGelombang.Rows[i].Cells["GroupId"].Value == null ? "" : dgvGelombang.Rows[i].Cells["GroupId"].Value.ToString());
                    SubGroup1Id.Add(dgvGelombang.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvGelombang.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                    SubGroup2Id.Add(dgvGelombang.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvGelombang.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                    ItemId.Add(dgvGelombang.Rows[i].Cells["ItemId"].Value == null ? "" : dgvGelombang.Rows[i].Cells["ItemId"].Value.ToString());
                    FullItemId.Add(dgvGelombang.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvGelombang.Rows[i].Cells["FullItemId"].Value.ToString());
                    ItemName.Add(dgvGelombang.Rows[i].Cells["ItemName"].Value == null ? "" : dgvGelombang.Rows[i].Cells["ItemName"].Value.ToString());
                    GelombangId.Add(dgvGelombang.Rows[i].Cells["GelombangId"].Value == null ? "" : dgvGelombang.Rows[i].Cells["GelombangId"].Value.ToString());
                    BracketId.Add(dgvGelombang.Rows[i].Cells["BracketId"].Value == null ? "" : dgvGelombang.Rows[i].Cells["BracketId"].Value.ToString());
                    Base.Add(dgvGelombang.Rows[i].Cells["Base"].Value == null ? "" : dgvGelombang.Rows[i].Cells["Base"].Value.ToString());
                    BracketDesc.Add(dgvGelombang.Rows[i].Cells["BracketDesc"].Value == null ? "" : dgvGelombang.Rows[i].Cells["BracketDesc"].Value.ToString());

                    if (dgvGelombang.Rows[i].Cells["Base"].Value.ToString() == "Y")
                    {
                        VendId.Add(dgvGelombang.Rows[i].Cells["VendId"].Value == null ? "" : dgvGelombang.Rows[i].Cells["VendId"].Value.ToString());
                    }
                    else
                    {
                        VendId.Add(dgvGelombang.Rows[i].Cells["VendId"].Value == null ? "" : "");
                    }

                    string PriceT = "";
                    if (dgvGelombang.Rows[i].Cells["Price"].Value == null)
                    {
                        PriceT = "0.00";
                    }
                    else if (dgvGelombang.Rows[i].Cells["Price"].Value.ToString() == "")
                    {
                        PriceT = "0.00";
                    }
                    else
                    {
                        PriceT = dgvGelombang.Rows[i].Cells["Price"].Value.ToString();
                    }
                    Price.Add(PriceT);
                }
            }
        }

        private void Gelombang2_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkAll_CheckedChanged_1(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvGelombang.RowCount - 1; i++)
            {
                if (dgvGelombang.Rows[i].Cells["Base"].Value.ToString() == "N")
                {
                    dgvGelombang.Rows[i].Cells["chk"].Value = Check;
                }
            }
        }

        //private void chkAll_CheckedChanged(object sender, EventArgs e)
        //{
        //    Boolean Check = chkAll.Checked;

        //    for (int i = 0; i <= dgvGelombang.RowCount - 1; i++)
        //    {
        //        if (dgvGelombang.Rows[i].Cells["Base"].Value.ToString() == "N")
        //        {
        //            dgvGelombang.Rows[i].Cells["chk"].Value = Check;
        //        }
        //    }
        //}

        

    }
}
