using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.NotaTransfer
{
    public partial class PopUpDetail : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        //Inventory.NotaTransfer.NTForm Parent;
        Inventory.NotaTransfer.NTForm Parent = new Inventory.NotaTransfer.NTForm();

        private string Query = "";

        public PopUpDetail()
        {
            InitializeComponent();
        }

        private void PopUpDetail_Load(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
            RefreshGrid();
        }

        public void SetParentForm(Inventory.NotaTransfer.NTForm F)
        {
            Parent = F;
        }   

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            //Query = "Select [GelombangId], [BracketId], ItemId, ItemName, [Base], [Price] From dbo.[InventGelombangD] where GelombangId+BracketId in (Select GelombangId+BracketId From dbo.[InventGelombangD] ";
            Query = "Select a.[FullItemID],a.[GroupId],a.SubGroup1Id,a.SubGroup2Id,a.ItemId, a.[ItemDeskripsi], a.UoM, a.UoMAlt, b.Ratio ";
            Query += "From dbo.[InventTable] a Left Join dbo.[InventConversion] b on a.FullItemID=b.FullItemID and a.[UoM]=b.FromUnit and a.UoMAlt=b.ToUnit ";
            Query += "Where a.FullItemID like '%" + txtSearch.Text.Trim() + "%' order by FullItemID asc ";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvSearch.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvSearch.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }
            Da.Fill(Dt);

            dgvSearch.AutoGenerateColumns = true;
            dgvSearch.DataSource = Dt;
            dgvSearch.Refresh();

            dgvSearch.ReadOnly = false;

            dgvSearch.Columns["GroupId"].Visible = false;
            dgvSearch.Columns["SubGroup1Id"].Visible = false;
            dgvSearch.Columns["SubGroup2Id"].Visible = false;
            dgvSearch.Columns["GroupId"].Visible = false;
            dgvSearch.Columns["ItemId"].Visible = false;

            dgvSearch.Columns["FullItemID"].ReadOnly = true;
            dgvSearch.Columns["ItemDeskripsi"].ReadOnly = true;
            dgvSearch.Columns["UoM"].ReadOnly = true;
            dgvSearch.Columns["UoMAlt"].ReadOnly = true;
            dgvSearch.Columns["Ratio"].ReadOnly = true;

            dgvSearch.AutoResizeColumns();

            Conn.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;
            for (int i = 0; i <= dgvSearch.RowCount - 1; i++)
            {
                dgvSearch.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> FullItemID = new List<string>();
            List<string> GroupId = new List<string>();
            List<string> SubGroup1Id = new List<string>();
            List<string> SubGroup2Id = new List<string>();
            List<string> ItemId = new List<string>();
            List<string> ItemDeskripsi = new List<string>();
            List<string> UoM = new List<string>();
            List<string> UoMAlt = new List<string>();
            List<string> Ratio = new List<string>();

            int CountChk = 0;
            for (int i = 0; i <= dgvSearch.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvSearch.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    FullItemID.Add(dgvSearch.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvSearch.Rows[i].Cells["FullItemID"].Value.ToString());
                    GroupId.Add(dgvSearch.Rows[i].Cells["GroupId"].Value == null ? "" : dgvSearch.Rows[i].Cells["GroupId"].Value.ToString());
                    SubGroup1Id.Add(dgvSearch.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvSearch.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                    SubGroup2Id.Add(dgvSearch.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvSearch.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                    ItemId.Add(dgvSearch.Rows[i].Cells["ItemId"].Value == null ? "" : dgvSearch.Rows[i].Cells["ItemId"].Value.ToString());
                    ItemDeskripsi.Add(dgvSearch.Rows[i].Cells["ItemDeskripsi"].Value == null ? "" : dgvSearch.Rows[i].Cells["ItemDeskripsi"].Value.ToString());
                    UoM.Add(dgvSearch.Rows[i].Cells["UoM"].Value == null ? "" : dgvSearch.Rows[i].Cells["UoM"].Value.ToString());
                    UoMAlt.Add(dgvSearch.Rows[i].Cells["UoMAlt"].Value == null ? "" : dgvSearch.Rows[i].Cells["UoMAlt"].Value.ToString());
                    Ratio.Add(dgvSearch.Rows[i].Cells["Ratio"].Value == null ? "" : dgvSearch.Rows[i].Cells["Ratio"].Value.ToString());
                   
                }
            }

            Parent.AddDataGridGelombang(FullItemID,GroupId,SubGroup1Id,SubGroup2Id,ItemId, ItemDeskripsi, UoM, UoMAlt, Ratio);
            this.Close();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
