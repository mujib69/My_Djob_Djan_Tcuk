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
    public partial class HeaderResizeGRNT : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query, crit = null;

        int count, seqNo;
        string id;

        public string ResizeID = "";
        public string GRID;

        int num;
        DataGridViewComboBoxCell cell;
        SqlDataReader Dr2;
        char flag;

        InquiryResizeGRNT Parent = new InquiryResizeGRNT();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void SetParent(InquiryResizeGRNT F)
        {
            Parent = F;
        }

        #region SetMode
        public void SetMode(string tmpMode, string tmpNumber)
        {
            Mode = tmpMode;
            ResizeID = tmpNumber;
            tbxResizeID.Text = tmpNumber;
        }
        #endregion


        public HeaderResizeGRNT()
        {
            InitializeComponent();
        }

        private void ResizeGRNT_Load(object sender, EventArgs e)
        {
            if (Mode == "BeforeEdit" || Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("select b.[RefTransId] from [dbo].[InventResizeH] as a left join [dbo].[InventResizeD] as b on a.[TransId] = b.[TransId] where a.[TransId] = '" + tbxResizeID.Text + "'; ", Conn);
                tbxGRID.Text = Cmd.ExecuteScalar().ToString();
                Conn.Close();
            }

            RefreshGrid();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
        }

        private void ModeNew()
        {
            Mode = "New";
            //tbxResizeID.Text = GenerateID();
            dateTimePicker1.Value = DateTime.Now;

            btnSGR.Enabled = true;
            button1.Enabled = true;
            btnSave.Enabled = true; btnExit.Enabled = true; btnEdit.Enabled = false; btnCancel.Enabled = false;
            if (!(string.IsNullOrEmpty(GRID)))
            {
                btnSGR.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            btnSGR.Enabled = false;
            button1.Enabled = false;
            btnSave.Enabled = false; btnExit.Enabled = true; btnEdit.Enabled = true; btnCancel.Enabled = false;
            dataGridView1.Columns["Check"].ReadOnly = true;
            dataGridView1.Columns["ToFullItemId"].ReadOnly = true;
            dataGridView1.Columns["ToItemName"].ReadOnly = true;
            dataGridView1.Columns["Check"].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns["ToFullItemId"].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns["ToItemName"].DefaultCellStyle.BackColor = Color.LightGray;
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            btnSGR.Enabled = false;
            button1.Enabled = false;
            btnSave.Enabled = true; btnExit.Enabled = true; btnEdit.Enabled = false; btnCancel.Enabled = true;
            dataGridView1.Columns["Check"].ReadOnly = false;
            dataGridView1.Columns["ToFullItemId"].ReadOnly = false;
            dataGridView1.Columns["ToItemName"].ReadOnly = false;
            dataGridView1.Columns["Check"].DefaultCellStyle.BackColor = Color.White;
            dataGridView1.Columns["ToFullItemId"].DefaultCellStyle.BackColor = Color.White;
            dataGridView1.Columns["ToItemName"].DefaultCellStyle.BackColor = Color.White;
        }

        //public string GenerateID()
        //{
        //    Conn = ConnectionString.GetConnection();
        //    count = 0;
        //    Cmd = new SqlCommand("Select count(*) from [dbo].[InventResizeH]", Conn);
        //    count = (Int32)Cmd.ExecuteScalar();
        //    if (count == 0)
        //        count++;
        //    else
        //    {
        //        Cmd = new SqlCommand("SELECT TOP 1 [TransId] FROM [dbo].[InventResizeH] order by [CreatedDate] desc", Conn);
        //        string[] lastID = Cmd.ExecuteScalar().ToString().Split('-');
        //        if (lastID[1] != DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM"))
        //            count = 1;
        //        else
        //            count = Convert.ToInt32(lastID[2]) + 1;
        //    }
        //    if (count.ToString().Length == 1)
        //        id += "0000" + count;
        //    else if (count.ToString().Length == 2)
        //        id += "000" + count;
        //    else if (count.ToString().Length == 3)
        //        id += "00" + count;
        //    else if (count.ToString().Length == 4)
        //        id += "0" + count;
        //    else if (count.ToString().Length == 5)
        //        id += "" + count;
        //    Conn.Close();
        //    return "RSZ-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-" + id;
        //}

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 12;
            dataGridView1.Columns[0].Name = "Check";
            dataGridView1.Columns[1].Name = "GroupId";
            dataGridView1.Columns[2].Name = "SubGroup1Id";
            dataGridView1.Columns[3].Name = "SubGroup2Id";
            dataGridView1.Columns[4].Name = "ItemId";
            dataGridView1.Columns[5].Name = "Qty_Actual";
            dataGridView1.Columns[6].Name = "Unit";
            dataGridView1.Columns[7].Name = "GoodsReceivedSeqNo";
            dataGridView1.Columns[8].Name = "FullItemId";
            dataGridView1.Columns[9].Name = "ItemName";
            dataGridView1.Columns[10].Name = "ToFullItemId";
            dataGridView1.Columns[11].Name = "ToItemName";
            if (!(string.IsNullOrEmpty(GRID)))
            {
                tbxGRID.Text = GRID;
            }

            num = 1;
            if (Mode == "New")
            {
                Cmd = new SqlCommand("select top 1 RefTransId from InventResizeD where RefTransId = '" + tbxGRID.Text + "'", Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    Query = " Select b.[GroupId] ,b.[SubGroup1Id] ,b.[SubGroup2Id] ,b.[ItemId] , b.[Qty_Actual], b.[Unit], b.[GoodsReceivedSeqNo], b.[FullItemId], b.[ItemName] from [dbo].[GoodsReceivedH] as a left join [dbo].[GoodsReceivedD] as b  on a.[GoodsReceivedId] = b.[GoodsReceivedId] left join InventResizeD as c  on a.GoodsReceivedId = c.RefTransId left join InventResizeH as d on c.TransId = d.TransId where a.[GoodsReceivedId] = '" + tbxGRID.Text + "' and d.Posted != '02' and b.FullItemId != c.OriginalFullItemId";
                }
                else
                {
                    Query = "Select b.[GroupId] ,b.[SubGroup1Id] ,b.[SubGroup2Id] ,b.[ItemId] , b.[Qty_Actual], b.[Unit], b.[GoodsReceivedSeqNo], b.[FullItemId], b.[ItemName] from [dbo].[GoodsReceivedH] as a left join [dbo].[GoodsReceivedD] as b on a.[GoodsReceivedId] = b.[GoodsReceivedId] where a.[GoodsReceivedId] = '" + tbxGRID.Text + "'";
                }
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                    this.dataGridView1.Rows.Add("", Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["Qty_Actual"], Dr["Unit"], Dr["GoodsReceivedSeqNo"], Dr["FullItemId"], Dr["ItemName"]);
                    dataGridView1.Rows[num - 1].Cells["Check"] = chk;
                    dataGridView1.Rows[num - 1].Cells["Check"].Value = false;
                    cellValue("Select distinct([ToFullItemId]) From [dbo].[ResizeTableD] where status != '02' and [FullItemId] = '" + Dr["FullItemId"] + "'");
                    //if (Dr["FullItemId"] != null)
                    //    cell.Value = Dr["FullItemId"];
                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["ToFullItemId"] = cell;

                    cellValue("Select distinct([ToItemName]) From [dbo].[ResizeTableD] where status != '02' and [FullItemId] = '" + Dr["FullItemId"] + "'");
                    //if (Dr["ItemName"] != null)
                    //    cell.Value = Dr["ItemName"];
                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["ToItemName"] = cell;
                    num++;
                }
            }
            else if (Mode == "BeforeEdit" || Mode == "Edit")
            {
                Query = "Select b.[GroupId] ,b.[SubGroup1Id] ,b.[SubGroup2Id] ,b.[ItemId] , b.[Qty], b.[Unit], b.[ParentSeqNo], b.[FullItemId], b.[ItemName], b.OriginalFullItemId, b.OriginalItemName from [dbo].[InventResizeH] as a left join [dbo].[InventResizeD] as b on a.[TransId] = b.[TransId] where a.[TransId] = '" + tbxResizeID.Text + "' and [OriginalItemId] is not null; ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                    this.dataGridView1.Rows.Add("", Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["Qty"], Dr["Unit"], Dr["ParentSeqNo"], Dr["OriginalFullItemId"], Dr["OriginalItemName"], Dr["FullItemId"], Dr["ItemName"]);
                    dataGridView1.Rows[num - 1].Cells["Check"] = chk;
                    dataGridView1.Rows[num - 1].Cells["Check"].Value = false;
                    if (Mode == "Edit")
                    {
                        cellValue("Select distinct([ToFullItemId]) From [dbo].[ResizeTableD] where status != '02' and [FullItemId] = '" + Dr["OriginalFullItemId"] + "'");
                        if (Dr["FullItemId"] != null)
                            cell.Value = Dr["FullItemId"];
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["ToFullItemId"] = cell;

                        cellValue("Select distinct([ToItemName]) From [dbo].[ResizeTableD] where status != '02' and [FullItemId] = '" + Dr["OriginalFullItemId"] + "'");
                        if (Dr["ItemName"] != null)
                            cell.Value = Dr["ItemName"];
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["ToItemName"] = cell;
                    }
                    num++;
                }
            }
            Dr.Close();
            Conn.Close();
            //MODE
            for (int i = 1; i <= 9; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
                dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
            }
            for (int i = 1; i <= 7; i++)
                dataGridView1.Columns[i].Visible = false;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        private void btnSGR_Click(object sender, EventArgs e)
        {
            string Where = " AND [GoodsReceivedStatus]='03'";

            Search F = new Search();
            F.SetSchemaTable("dbo", "GoodsReceivedH", Where);
            F.ShowDialog();
            tbxGRID.Text = ConnectionString.Kode;
            RefreshGrid();
        }

        #region cellValue
        private DataGridViewComboBoxCell cellValue(string query)
        {
            cell = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(query, Conn);
            Dr2 = Cmd.ExecuteReader();
            cell = new DataGridViewComboBoxCell();
            while (Dr2.Read())
                cell.Items.Add(Dr2[0].ToString());
            Dr2.Close();
            Conn.Close();
            return cell;
        }
        #endregion

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ToFullItemId")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["ToFullItemId"].Value != null)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select [ItemDeskripsi] from [dbo].[InventTable] where [FullItemID] = '" + dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["ToFullItemId"].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Query = "";
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["ToItemName"].Value = Cmd.ExecuteScalar().ToString();
                    Conn.Close();
                }
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ToItemName")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["ToItemName"].Value != null)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select [FullItemID] from [dbo].[InventTable] where [ItemDeskripsi] = '" + dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["ToItemName"].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Query = "";
                    dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["ToFullItemId"].Value = Cmd.ExecuteScalar().ToString();
                    Conn.Close();
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private char Validation()
        {
            flag = '\0';
            if (tbxGRID.Text == String.Empty)
            {
                MetroFramework.MetroMessageBox.Show(this, "Fill in GR Number!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                flag = 'X';
            }
            return flag;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation() != 'X')
            {
                Conn = ConnectionString.GetConnection();
                if (Mode != "Edit")
                {
                    //INSERT RESIZE HEADER
                    Query = "insert into [dbo].[InventResizeH] ([TransDate] ,[TransId] ,[CreatedDate] ,[CreatedBy],[Posted])values ('" + dateTimePicker1.Value + "', '" + tbxResizeID.Text + "', getdate(), '" + Login.UserGroup + "', 1); ";

                    //INSERT RESIZE DETAILS
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells["ToFullItemId"].Value != null && dataGridView1.Rows[i].Cells["ToItemName"].Value != null)
                        {
                            Cmd = new SqlCommand("select * from [dbo].[GoodsReceivedD] where [GoodsReceivedId] = '" + tbxGRID.Text + "' and GoodsReceivedSeqNo = '" + dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "'; ", Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                seqNo += 1;
                                Query += "insert into [dbo].[InventResizeD] ( [TransId],[SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteIdIssue],[InventSiteIdReceive],[Qty],[Unit],[RefTransId],[RefSeqNo],[OriginalGroupId] ,[OriginalSubGroup1Id] ,[OriginalSubGroup2Id] ,[OriginalItemId] ,[OriginalFullItemId], [OriginalItemName], [ParentSeqNo] ,[CreatedDate] ,[CreatedBy] ) ";
                                Query += "values ('" + tbxResizeID.Text + "', '" + seqNo + "', '" + Dr["GroupId"] + "', '" + Dr["SubGroup1Id"] + "', '" + Dr["SubGroup2Id"].ToString() + "', '" + Dr["ItemId"].ToString() + "', '" + Dr["FullItemId"].ToString() + "', '" + Dr["ItemName"].ToString() + "', '" + Dr["InventSiteId"] + "', NULL, '" + Convert.ToInt32(Convert.ToInt32(Dr["Qty_Actual"]) * -1) + "', '" + Dr["Unit"] + "', '" + tbxGRID.Text + "', '" + Dr["GoodsReceivedSeqNo"] + "', NULL, NULL, NULL, NULL, NULL, NULL, NULL, getdate(), '" + Login.UserGroup + "'); ";
                            }
                        }
                    }

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells["ToFullItemId"].Value != null && dataGridView1.Rows[i].Cells["ToItemName"].Value != null)
                        {
                            seqNo += 1;
                            Query += "insert into [dbo].[InventResizeD] ( [TransId],[SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteIdIssue],[InventSiteIdReceive],[Qty],[Unit],[RefTransId],[RefSeqNo],[OriginalGroupId] ,[OriginalSubGroup1Id] ,[OriginalSubGroup2Id] ,[OriginalItemId] ,[OriginalFullItemId], [OriginalItemName], [ParentSeqNo] ,[CreatedDate] ,[CreatedBy] ) ";
                            Query += "values ('" + tbxResizeID.Text + "', '" + seqNo + "', '" + dataGridView1.Rows[i].Cells["ToFullItemId"].Value.ToString().Split('.')[0] + "', '" + dataGridView1.Rows[i].Cells["ToFullItemId"].Value.ToString().Split('.')[1] + "', '" + dataGridView1.Rows[i].Cells["ToFullItemId"].Value.ToString().Split('.')[2] + "', '" + dataGridView1.Rows[i].Cells["ToFullItemId"].Value.ToString().Split('.')[3] + "', '" + dataGridView1.Rows[i].Cells["ToFullItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ToItemName"].Value.ToString() + "', NULL, NULL, '" + dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["Unit"].Value.ToString() + "', '" + tbxGRID.Text + "', NULL, '" + dataGridView1.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "', getdate(), '" + Login.UserGroup + "'); ";
                        }
                    }
                }
                else if (Mode == "Edit")
                {
                    Query = "update [dbo].[InventResizeH] set [UpdatedDate] = getdate(), [UpdatedBy] = '" + Login.UserGroup + "' where [TransId] = '" + tbxResizeID.Text + "'; ";
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        Query += "update [dbo].[InventResizeD] set [FullItemId] = '" + dataGridView1.Rows[i].Cells["ToFullItemId"].Value.ToString() + "', [ItemName] = '" + dataGridView1.Rows[i].Cells["ToItemName"].Value.ToString() + "', [UpdatedDate] = getdate(), [UpdatedBy] = '" + Login.UserGroup + "' where [ParentSeqNo] = '" + dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "'; ";
                    }
                }
                Cmd = new SqlCommand(Query, Conn);
                int result = Cmd.ExecuteNonQuery();
                if (result == 0)
                    MetroFramework.MetroMessageBox.Show(this, "Unable to save data!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    MetroFramework.MetroMessageBox.Show(this, "Save Success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //btnSave.Enabled = false;
                    ModeBeforeEdit();
                    RefreshGrid();
                }
                Conn.Close();
                Parent.RefreshGrid();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.Edit) > 0)
            {
                Mode = "Edit";
                ModeEdit();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end             
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";
            ModeBeforeEdit();
            RefreshGrid();
        }

        private void HeaderResizeGRNT_FormClosed(object sender, FormClosedEventArgs e)
        {
            GRID = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PopUpGRNT f = new PopUpGRNT();
            f.ShowDialog();
            tbxGRID.Text = PopUpGRNT.grID;
            RefreshGrid();
        }
    }
}
