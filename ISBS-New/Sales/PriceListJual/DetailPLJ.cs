using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;


namespace ISBS_New.Sales.PriceListJual
{
     public partial class DetailPLJ : MetroFramework.Forms.MetroForm
    {

        Sales.PriceListJual.HeaderPLJ Parent;

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        DataGridView dgvDetailFromHeader;

        public DetailPLJ()
        {
            InitializeComponent();
           
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            SearchQuery tmpSearch = new SearchQuery();
            tmpSearch.PrimaryKey = "GroupId";
            tmpSearch.Table = "[dbo].[InventGroup]";
            tmpSearch.QuerySearch = "SELECT a.[GroupId],a.Deskripsi GroupDeskripsi,a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM [dbo].[InventGroup] a";
            tmpSearch.FilterText = new string[] { "GroupId", "GroupDeskripsi" };
            tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
            tmpSearch.Select = new string[] { "GroupId", "GroupDeskripsi" };
            //tmpSearch.WherePlus = "";
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtItemGroupId.Text = ConnectionString.Kodes[0];
                txtItemGroupName.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }
        }

        private void btnSubGroup1_Click(object sender, EventArgs e)
        {
            SearchQuery tmpSearch = new SearchQuery();
            tmpSearch.PrimaryKey = "SubGroup1ID";
            tmpSearch.Table = "[dbo].[InventSubGroup1]";
            tmpSearch.QuerySearch = "SELECT a.[GroupId],b.Deskripsi GroupDeskripsi,a.[SubGroup1ID],a.Deskripsi SubGroup1Deskripsi,a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM [dbo].[InventSubGroup1] a left join InventGroup b on a.GroupId=b.GroupId ";
            tmpSearch.FilterText = new string[] { "GroupId", "GroupDeskripsi", "SubGroup1ID", "SubGroup1Deskripsi" };
            tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
            tmpSearch.Select = new string[] { "GroupId", "GroupDeskripsi", "SubGroup1Id", "SubGroup1Deskripsi" };
            tmpSearch.WherePlus = "and GroupDeskripsi like '%" + txtItemGroupName.Text.Trim() + "%'";
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtItemGroupId.Text = ConnectionString.Kodes[0];
                txtItemGroupName.Text = ConnectionString.Kodes[1];
                txtItemSubGroup1Id.Text = ConnectionString.Kodes[2];
                txtSubGroup1Name.Text = ConnectionString.Kodes[3];
                ConnectionString.Kodes = null;
            }
        }

        private void btnSubGroup2_Click(object sender, EventArgs e)
        {
            SearchQuery tmpSearch = new SearchQuery();
            tmpSearch.PrimaryKey = "SubGroup2ID";
            tmpSearch.Table = "[dbo].[InventSubGroup2]";
            tmpSearch.QuerySearch = "SELECT a.[GroupId],c.Deskripsi GroupDeskripsi,a.[SubGroup1ID],b.Deskripsi SubGroup1Deskripsi,a.[SubGroup2ID], a.[Deskripsi] SubGroup2Deskripsi,a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM [dbo].[InventSubGroup2] a left join InventSubGroup1 b on a.SubGroup1ID=b.SubGroup1ID left join InventGroup c on a.GroupId=c.GroupId ";
            tmpSearch.FilterText = new string[] { "GroupId", "GroupDeskripsi", "SubGroup1ID", "SubGroup1Deskripsi", "SubGroup2ID", "SubGroup2Deskripsi" };
            tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
            tmpSearch.Select = new string[] { "GroupId", "GroupDeskripsi", "SubGroup1Id", "SubGroup1Deskripsi", "SubGroup2Id", "SubGroup2Deskripsi" };
            tmpSearch.WherePlus = "and GroupDeskripsi like '%" + txtItemGroupName.Text.Trim() + "%' and SubGroup1Deskripsi like '%" + txtSubGroup1Name.Text.Trim() + "%'";
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtItemGroupId.Text = ConnectionString.Kodes[0];
                txtItemGroupName.Text = ConnectionString.Kodes[1];
                txtItemSubGroup1Id.Text = ConnectionString.Kodes[2];
                txtSubGroup1Name.Text = ConnectionString.Kodes[3];
                txtItemSubGroup2Id.Text = ConnectionString.Kodes[4];
                txtSubGroup2Name.Text = ConnectionString.Kodes[5];
                ConnectionString.Kodes = null;
            }
        }

        private void btnItem_Click(object sender, EventArgs e)
        {
            SearchQuery tmpSearch = new SearchQuery();
            tmpSearch.PrimaryKey = "FullItemID";
            tmpSearch.Table = "[dbo].[InventTable]";
            tmpSearch.QuerySearch = "SELECT [FullItemID],[ItemDeskripsi],[GroupID],[GroupDeskripsi],[SubGroup1ID],[SubGroup1Deskripsi],[SubGroup2ID],[SubGroup2Deskripsi],[ItemID],CreatedDate,UpdatedDate FROM [dbo].[InventTable]";
            tmpSearch.FilterText = new string[] { "FullItemID", "ItemDeskripsi", "GroupID", "GroupDeskripsi", "SubGroup1ID", "SubGroup1Deskripsi", "SubGroup2ID", "SubGroup2Deskripsi", "ItemID" };
            tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
            tmpSearch.Select = new string[] { "GroupId", "GroupDeskripsi", "SubGroup1Id", "SubGroup1Deskripsi", "SubGroup2Id", "SubGroup2Deskripsi", "FullItemID", "ItemDeskripsi" };
            tmpSearch.WherePlus = "and GroupDeskripsi like '%" + txtItemGroupName.Text.Trim() + "%' and SubGroup1Deskripsi like '%" + txtSubGroup1Name.Text.Trim() + "%' and SubGroup2Deskripsi like '%" + txtSubGroup2Name.Text.Trim() + "%' ";
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtItemGroupId.Text = ConnectionString.Kodes[0];
                txtItemGroupName.Text = ConnectionString.Kodes[1];
                txtItemSubGroup1Id.Text = ConnectionString.Kodes[2];
                txtSubGroup1Name.Text = ConnectionString.Kodes[3];
                txtItemSubGroup2Id.Text = ConnectionString.Kodes[4];
                txtSubGroup2Name.Text = ConnectionString.Kodes[5];
                txtItemId.Text = ConnectionString.Kodes[6];
                txtItemName.Text = ConnectionString.Kodes[7];
                ConnectionString.Kodes = null;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshDataGrid();
        }

        public void ParamHeader(DataGridView prmDgvDetailFromHeader)
        {
            dgvDetailFromHeader = prmDgvDetailFromHeader;
        }


        private void RefreshDataGrid()
        {
            Conn = ConnectionString.GetConnection();

            //Query = "Select ROW_NUMBER() OVER (ORDER BY GelombangId asc,Base desc,FullItemId asc) No,* From (Select a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.FullItemId, a.ItemDeskripsi ItemName, b.GelombangId, case when b.Price is null then '0.00' end Price, c.VendId From dbo.[InventTable] a left join dbo.InventGelombangD b on a.FullItemId=b.ItemId left join dbo.InventGelombangVendor c on b.GelombangId=c.GelombangId and b.BracketId=c.BracketId ";
            //Query += "Where a.GroupDeskripsi like'%" + txtItemGroupName.Text + "%' and a.SubGroup1Deskripsi like '%" + txtSubGroup1Name.Text + "%' and a.SubGroup2Deskripsi like '%" + txtSubGroup2Name.Text + "%' and a.ItemDeskripsi like '%" + txtItemName.Text + "%' ";
            //Query += Parent.GetInvStockId() + ") a;";

            Query = "Select ROW_NUMBER() OVER (ORDER BY FullItemId asc) No,* From (Select a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.FullItemId, a.ItemDeskripsi ItemName From dbo.[InventTable] a ";
            Query += "Where a.GroupDeskripsi like'%" + txtItemGroupName.Text + "%' and a.SubGroup1Deskripsi like '%" + txtSubGroup1Name.Text + "%' and a.SubGroup2Deskripsi like '%" + txtSubGroup2Name.Text + "%' and a.ItemDeskripsi like '%" + txtItemName.Text + "%') a;";


            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            //if (Parent.cmbPrType.Text == "FIX")
            //{
            //    if (dgvPLJDetails.Columns.Contains("chk") == false)
            //    {
            //        DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            //        dgvPLJDetails.Columns.Add(chk);
            //        chk.HeaderText = "Check";
            //        chk.Name = "chk";
            //    }
            //}
            if (dgvPLJDetails.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvPLJDetails.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);

            dgvPLJDetails.AutoGenerateColumns = true;
            dgvPLJDetails.DataSource = Dt;
            dgvPLJDetails.Refresh();

            string FullItemIdH = "";
            string FullItemID = "";
            List<string> RemoveFullItemId = new List<string>();

            for (int i = 0; i < dgvPLJDetails.RowCount; i++)
            {
                FullItemID = dgvPLJDetails.Rows[i].Cells["FullItemId"].Value.ToString();

                for (int j = 0; j < dgvDetailFromHeader.RowCount; j++)
                {
                    FullItemIdH = dgvDetailFromHeader.Rows[j].Cells["FullItemId"].Value.ToString();

                    if (FullItemIdH == FullItemID)
                    {
                        RemoveFullItemId.Add(FullItemID);
                    }
                }
            }

            for (int i = 0; i < RemoveFullItemId.Count; i++)
            {
                for (int j = 0; j < dgvPLJDetails.RowCount; j++)
                {
                    FullItemID = dgvPLJDetails.Rows[j].Cells["FullItemId"].Value.ToString();
                    if (FullItemID == RemoveFullItemId[i])
                    {
                        dgvPLJDetails.Rows.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < dgvPLJDetails.RowCount; i++)
            {
                dgvPLJDetails.Rows[i].Cells["No"].Value = i + 1;
            }

            dgvPLJDetails.ReadOnly = false;
            dgvPLJDetails.Columns["No"].ReadOnly = true;
            dgvPLJDetails.Columns["FullItemId"].ReadOnly = true;
            dgvPLJDetails.Columns["ItemName"].ReadOnly = true;
           // dgvPLJDetails.Columns["GelombangId"].ReadOnly = true;
          //  dgvPLJDetails.Columns["Base"].ReadOnly = true;

            dgvPLJDetails.Columns["GroupId"].Visible = false;
            dgvPLJDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPLJDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPLJDetails.Columns["ItemId"].Visible = false;
            //dgvDetailPR.Columns["BracketId"].Visible = false;
            //dgvPLJDetails.Columns["Price"].Visible = false;
           // dgvPLJDetails.Columns["VendId"].Visible = false;
            dgvPLJDetails.AutoResizeColumns();

            Conn.Close();

        }

        public void ParentRefreshGrid(Sales.PriceListJual.HeaderPLJ F)
        {
            Parent = F;
        }

        private void DetailPLJ2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            lblForm.Location = new Point(16, 11);
           
        }
        
        private void DetailPLJ2_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }


        private void dgvPLJDetails_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
                MethodSelectData();
            
        }

        private void MethodSelectData()
        {
            List<string> GroupId = new List<string>();
            List<string> SubGroup1Id = new List<string>();
            List<string> SubGroup2Id = new List<string>();
            List<string> ItemId = new List<string>();
            List<string> FullItemId = new List<string>();
            List<string> ItemName = new List<string>();
          //  List<string> GelombangId = new List<string>();
         //  List<string> BracketId = new List<string>();
         //   List<string> Base = new List<string>();
           // List<string> Price = new List<string>();

            int CountChk = 0;
            for (int i = 0; i <= dgvPLJDetails.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvPLJDetails.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    GroupId.Add(dgvPLJDetails.Rows[i].Cells["GroupId"].Value.ToString());
                    SubGroup1Id.Add(dgvPLJDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                    SubGroup2Id.Add(dgvPLJDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                    ItemId.Add(dgvPLJDetails.Rows[i].Cells["ItemId"].Value.ToString());
                    FullItemId.Add(dgvPLJDetails.Rows[i].Cells["FullItemId"].Value.ToString());
                    ItemName.Add(dgvPLJDetails.Rows[i].Cells["ItemName"].Value.ToString());
                  //  GelombangId.Add(dgvPLJDetails.Rows[i].Cells["GelombangId"].Value.ToString());
                   // BracketId.Add(dgvPLJDetails.Rows[i].Cells["BracketId"].Value.ToString());
                   // Base.Add(dgvPLJDetails.Rows[i].Cells["Base"].Value.ToString());
                    //Base.Add("");
                    //string PriceT = "";
                    //if (dgvPLJDetails.Rows[i].Cells["Price"].Value == null)
                    //{
                    //    PriceT = "0.00";
                    //}
                    //else if (dgvPLJDetails.Rows[i].Cells["Price"].Value.ToString() == "")
                    //{
                    //    PriceT = "0.00";
                    //}
                    //else
                    //{
                    //    PriceT = dgvPLJDetails.Rows[i].Cells["Price"].Value.ToString();
                    //}
                    //Price.Add(PriceT);
                }
            }

            Parent.AddDataGridDetail(GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName);

        

            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDataGrid();

            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvPLJDetails.RowCount - 1; i++)
            {
                dgvPLJDetails.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            MethodSelectData();
        }

      

    }
}
