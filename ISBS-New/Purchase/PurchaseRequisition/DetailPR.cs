using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseRequisition
{
    public partial class DetailPR : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;

        Purchase.PurchaseRequisition.HeaderPR Parent;

        public DetailPR()
        {
            InitializeComponent();
        }

        private void DetailPR2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);
            if (Parent.cmbPrType.Text != "FIX")
                chkAll.Visible = false;
            else
                chkAll.Visible = true;
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            //string SchemaName = "dbo";
            //string TableName = "InventGroup";

            //Search tmpSearch = new Search();
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            //tmpSearch.ShowDialog();
            //txtItemGroupId.Text = ConnectionString.Kode;
            //txtItemGroupName.Text = ConnectionString.Kode2;

            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "GroupId";
            tmpSearch.Order = "GroupId asc";
            tmpSearch.Table = "[dbo].[InventGroup]";
            tmpSearch.QuerySearch = "SELECT a.[GroupId],a.Deskripsi GroupDeskripsi,a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM [dbo].[InventGroup] a";
            tmpSearch.FilterText = new string[] { "GroupId", "GroupDeskripsi", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "Group Id", "Group Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
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
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "SubGroup1ID";
            tmpSearch.Order = "SubGroup1ID asc";
            tmpSearch.Table = "[dbo].[InventSubGroup1]";
            tmpSearch.QuerySearch = "SELECT a.[GroupId],b.Deskripsi GroupDeskripsi,a.[SubGroup1ID],a.Deskripsi SubGroup1Deskripsi,a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM [dbo].[InventSubGroup1] a left join InventGroup b on a.GroupId=b.GroupId ";
            tmpSearch.FilterText = new string[] { "GroupId", "GroupDeskripsi", "SubGroup1ID", "SubGroup1Deskripsi", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "Group Id", "Group Deskripsi", "SubGroup1 ID", "SubGroup1 Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
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
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "SubGroup2ID";
            tmpSearch.Order = "SubGroup2ID Asc";
            tmpSearch.Table = "[dbo].[InventSubGroup2]";
            tmpSearch.QuerySearch = "SELECT a.[GroupId],c.Deskripsi GroupDeskripsi,a.[SubGroup1ID],b.Deskripsi SubGroup1Deskripsi,a.[SubGroup2ID], a.[Deskripsi] SubGroup2Deskripsi,a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM [dbo].[InventSubGroup2] a left join InventSubGroup1 b on a.SubGroup1ID=b.SubGroup1ID left join InventGroup c on a.GroupId=c.GroupId ";
            tmpSearch.FilterText = new string[] { "GroupId", "GroupDeskripsi", "SubGroup1ID", "SubGroup1Deskripsi", "SubGroup2ID", "SubGroup2Deskripsi", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "Group Id", "Group Deskripsi", "SubGroup1 ID", "SubGroup1 Deskripsi", "SubGroup2 ID", "SubGroup2 Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
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
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "FullItemID";
            tmpSearch.Order = "FullItemID asc";
            tmpSearch.Table = "[dbo].[InventTable]";
            tmpSearch.QuerySearch = "SELECT [FullItemID],[ItemDeskripsi],[GroupID],[GroupDeskripsi],[SubGroup1ID],[SubGroup1Deskripsi],[SubGroup2ID],[SubGroup2Deskripsi],[ItemID],CreatedDate,UpdatedDate FROM [dbo].[InventTable]";
            tmpSearch.FilterText = new string[] { "FullItemID", "ItemDeskripsi", "GroupID", "GroupDeskripsi", "SubGroup1ID", "SubGroup1Deskripsi", "SubGroup2ID", "SubGroup2Deskripsi", "ItemID", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "FullItemID", "Item Deskripsi", "Group ID", "Group Deskripsi", "SubGroup1 ID", "SubGroup1 Deskripsi", "SubGroup2 ID", "SubGroup2 Deskripsi", "Item ID", "Created Date", "Created By", "Updated Date", "Updated By" };
            tmpSearch.FilterDate = new string[] {"CreatedDate","UpdatedDate"};
            tmpSearch.Select = new string[] { "GroupId","GroupDeskripsi","SubGroup1Id","SubGroup1Deskripsi","SubGroup2Id","SubGroup2Deskripsi","FullItemID", "ItemDeskripsi" };
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

        private void RefreshDataGrid()
        {
            Conn = ConnectionString.GetConnection();

            if (Parent.cmbPrType.Text == "FIX")
            {
                //Query = "Select ROW_NUMBER() OVER (ORDER BY GelombangId asc,Base desc,FullItemId asc) No,* From (Select a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.FullItemId, a.ItemDeskripsi ItemName, b.Base, b.GelombangId, b.BracketId, case when b.Price is null then '0.00' end Price, c.VendId From dbo.[InventTable] a left join dbo.InventGelombangD b on a.FullItemId=b.ItemId AND b.Type='Purchase' left join dbo.InventGelombangVendor c on b.GelombangId=c.GelombangId and b.BracketId=c.BracketId ";
                //Query += "Where a.GroupDeskripsi like'%" + txtItemGroupName.Text + "%' and a.SubGroup1Deskripsi like '%" + txtSubGroup1Name.Text + "%' and a.SubGroup2Deskripsi like '%" + txtSubGroup2Name.Text + "%' and a.ItemDeskripsi like '%" + txtItemName.Text + "%' ";
                //Query += Parent.GetInvStockId() + ") a;";

                Query = "Select ROW_NUMBER() OVER (ORDER BY FullItemId asc) No,* From (Select a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.FullItemId, a.ItemDeskripsi ItemName From dbo.[InventTable] a  ";
                Query += "Where a.GroupDeskripsi like @group and a.SubGroup1Deskripsi like @sub1 and a.SubGroup2Deskripsi like @sub2 and a.ItemDeskripsi like @search ";
                Query += Parent.GetInvStockId() + " AND TagSizeID = '000') a;";
            }
            else
            {
                Query = "Select ROW_NUMBER() OVER (ORDER BY GelombangId asc,Base desc,FullItemId asc) No,* From (Select a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.FullItemId, a.ItemDeskripsi ItemName, b.Base, b.GelombangId, b.BracketId, case when b.Price is null then '0.00' end Price, c.VendId, d.BracketDesc ";
                Query += "From dbo.[InventTable] a left join dbo.InventGelombangD b on a.FullItemId=b.ItemId AND b.Type='Purchase' ";
                Query += "LEFT JOIN dbo.InventGelombangH d on d.GelombangId = b.GelombangId AND d.BracketId=b.BracketId ";
                Query += "LEFT JOIN dbo.InventGelombangVendor c on b.GelombangId=c.GelombangId AND b.BracketId=c.BracketId ";
                Query += "Where b.base is not null and a.GroupDeskripsi like @group and a.SubGroup1Deskripsi like @sub1 and a.SubGroup2Deskripsi like @sub2 and a.ItemDeskripsi like @search ";
                Query += Parent.GetInvStockId() + " AND TagSizeID = '000') a;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@group", "%" + txtItemGroupName.Text + "%");
            Da.SelectCommand.Parameters.AddWithValue("@sub1", "%" + txtSubGroup1Name.Text + "%");
            Da.SelectCommand.Parameters.AddWithValue("@sub2", "%" + txtSubGroup2Name.Text + "%");
            Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtItemName.Text + "%");
            Dt = new DataTable();
            if (Parent.cmbPrType.Text == "FIX")
            {
                if (dgvDetailPR.Columns.Contains("chk") == false)
                {
                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                    dgvDetailPR.Columns.Add(chk);
                    chk.HeaderText = "Check";
                    chk.Name = "chk";
                }
            }
            Da.Fill(Dt);

            dgvDetailPR.AutoGenerateColumns = true;
            dgvDetailPR.DataSource = Dt;
            dgvDetailPR.Refresh();

            dgvDetailPR.ReadOnly = false;
            dgvDetailPR.Columns["No"].ReadOnly = true;
            dgvDetailPR.Columns["FullItemId"].ReadOnly = true;
            dgvDetailPR.Columns["ItemName"].ReadOnly = true;
            


            dgvDetailPR.Columns["GroupId"].Visible = false;
            dgvDetailPR.Columns["SubGroup1Id"].Visible = false;
            dgvDetailPR.Columns["SubGroup2Id"].Visible = false;
            dgvDetailPR.Columns["ItemId"].Visible = false;
            //dgvDetailPR.Columns["BracketId"].Visible = false;


            if (Parent.cmbPrType.Text != "FIX")
            {
                dgvDetailPR.Columns["GelombangId"].ReadOnly = true;
                dgvDetailPR.Columns["Base"].ReadOnly = true;
                dgvDetailPR.Columns["BracketId"].ReadOnly = true;
                dgvDetailPR.Columns["Price"].Visible = false;
                dgvDetailPR.Columns["VendId"].Visible = false;
            }
            
            dgvDetailPR.AutoResizeColumns();

            Conn.Close();

        }

        public void ParentRefreshGrid(Purchase.PurchaseRequisition.HeaderPR F)
        {
            Parent = F;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            MethodSelectData();
        }

        private void dgvDetailPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Parent.cmbPrType.Text != "FIX")
            {
                MethodSelectData();
            }
        }

        private void MethodSelectData()
        {
            if (dgvDetailPR.Rows.Count > 0)
            {
                if (Parent.cmbPrType.Text != "FIX")
                {
                    Parent.Gelombang = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["GelombangId"].Value.ToString();
                    Parent.Bracket = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["BracketId"].Value.ToString();

                    string GroupId = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["GroupId"].Value.ToString();
                    string SubGroup1Id = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["SubGroup1Id"].Value.ToString();
                    string SubGroup2Id = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["SubGroup2Id"].Value.ToString();
                    string ItemId = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["ItemId"].Value.ToString();
                    string FullItemId = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["FullItemId"].Value.ToString();
                    string ItemName = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["ItemName"].Value.ToString();
                    string GelombangId = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["GelombangId"].Value.ToString();
                    string BracketId = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["BracketId"].Value.ToString();
                    string Base = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["Base"].Value.ToString();
                    string Price = "";
                    if (dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["Price"].Value == null)
                    {
                        Price = "0.00";
                    }
                    else if (dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["Price"].Value == "")
                    {
                        Price = "0.00";
                    }
                    else
                    {
                        Price = dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["Price"].Value.ToString();
                    }

                    if (Parent.Gelombang == "")
                    {
                        Parent.AddDataGridDetail(GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, GelombangId, BracketId, Base, Price);
                    }

                }
                else
                {
                    //Parent.Gelombang1.Add(dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["GelombangId"].Value.ToString());
                    //Parent.Bracket1.Add(dgvDetailPR.Rows[dgvDetailPR.CurrentCell.RowIndex].Cells["BracketId"].Value.ToString());

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

                    int CountChk = 0;
                    for (int i = 0; i <= dgvDetailPR.RowCount - 1; i++)
                    {
                        Boolean Check = Convert.ToBoolean(dgvDetailPR.Rows[i].Cells["chk"].Value);
                        if (Check == true)
                        {
                            CountChk++;
                            GroupId.Add(dgvDetailPR.Rows[i].Cells["GroupId"].Value.ToString());
                            SubGroup1Id.Add(dgvDetailPR.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                            SubGroup2Id.Add(dgvDetailPR.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                            ItemId.Add(dgvDetailPR.Rows[i].Cells["ItemId"].Value.ToString());
                            FullItemId.Add(dgvDetailPR.Rows[i].Cells["FullItemId"].Value.ToString());
                            ItemName.Add(dgvDetailPR.Rows[i].Cells["ItemName"].Value.ToString());

                            //Updated stv
                            //GelombangId.Add(dgvDetailPR.Rows[i].Cells["GelombangId"].Value.ToString());
                            //BracketId.Add(dgvDetailPR.Rows[i].Cells["BracketId"].Value.ToString());
                            GelombangId.Add("");
                            BracketId.Add("");
                            Base.Add("");
                            //string PriceT = "";
                            //if (dgvDetailPR.Rows[i].Cells["Price"].Value == null)
                            //{
                            //    PriceT = "0.00";
                            //}
                            //else if (dgvDetailPR.Rows[i].Cells["Price"].Value.ToString() == "")
                            //{
                            //    PriceT = "0.00";
                            //}
                            //else
                            //{
                            //    PriceT = dgvDetailPR.Rows[i].Cells["Price"].Value.ToString();
                            //}
                            //Price.Add(PriceT);
                            Price.Add("0.00");
                            //end
                        }
                    }

                    Parent.AddDataGridDetail(GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, GelombangId, BracketId, Base, Price);

                }
            }

            this.Close();
        }

        private void DetailPR2_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void chkCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDataGrid();

            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvDetailPR.RowCount - 1; i++)
            {
                dgvDetailPR.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void dgvDetailPR_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                MethodSelectData();
        }

        private void txtItemName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                RefreshDataGrid();
        }



    }
}
