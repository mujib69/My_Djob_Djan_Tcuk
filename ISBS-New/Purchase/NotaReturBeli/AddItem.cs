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
    public partial class AddItem : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query, GRNumber;
        Purchase.NotaReturBeli.ReturBeliHeader Parent;
        List<string> SeqNo;
        
        public AddItem()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.NotaReturBeli.ReturBeliHeader F)
        {
            Parent = F;
        }

        public void setMode(string grnumber, List<string> seqno)
        {
            GRNumber = grnumber;
            SeqNo = seqno;
        }

        private void AddItem_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if (dgvItem.RowCount - 1 <= 0)
            {
                dgvItem.ColumnCount = 16;
                dgvItem.Columns[0].Name = "Check";
                dgvItem.Columns[1].Name = "FullItemId";
                dgvItem.Columns[2].Name = "Item Name";
                dgvItem.Columns[3].Name = "Qty Actual"; dgvItem.Columns["Qty Actual"].HeaderText = "Qty";
                dgvItem.Columns[4].Name = "Remaining_Qty"; dgvItem.Columns["Remaining_Qty"].HeaderText = "Remaining Qty";
                dgvItem.Columns[5].Name = "Unit";
                dgvItem.Columns[6].Name = "Price";
                dgvItem.Columns[7].Name = "InventSiteID";
                dgvItem.Columns[8].Name = "InventSiteBlokID";
                dgvItem.Columns[9].Name = "Quality";
                dgvItem.Columns[10].Name = "Notes";
                dgvItem.Columns[11].Name = "Action";
                dgvItem.Columns[12].Name = "SeqNo";
                dgvItem.Columns[13].Name = "GroupId";
                dgvItem.Columns[14].Name = "SubGroup1Id";
                dgvItem.Columns[15].Name = "SubGroup2Id";
            }

            if (SeqNo.Count > 0)
            {
                string addIn = "";
                for (int x = 0; x < SeqNo.Count; x++)
                {
                    addIn += "'" + SeqNo[x] + "'";
                    if (x == SeqNo.Count-1)
                    {
                        break;
                    }
                    else
                    {
                        addIn += ",";
                    }
                }
                //Query = "SELECT DISTINCT FullItemID, ItemName, Qty_Actual, ISNULL(Remaining_Qty, 0) AS Remaining_Qty, Unit, InventSiteID, InventSiteBlokID, Quality, Notes, b.Deskripsi, GoodsReceivedSeqNo, GroupId, SubGroup1Id, SubGroup2Id FROM GoodsReceivedD a INNER JOIN TransStatusTable b on a.ActionCodeStatus=b.StatusCode And b.TransCode = 'GRD' WHERE a.GoodsReceivedId = '" + GRNumber + "' And a.ActionCodeStatus = '05' And GoodsReceivedSeqNo Not In (" + addIn + ")";
                Query = "SELECT DISTINCT GRD.FullItemID, GRD.ItemName, Qty_Actual, ISNULL(Remaining_Qty, 0) AS Remaining_Qty, GRD.Unit, ISNULL(POD.Price, 0) AS Price, GRD.InventSiteID, InventSiteBlokID, Quality, Notes, b.Deskripsi, GoodsReceivedSeqNo, GRD.GroupId, GRD.SubGroup1Id, GRD.SubGroup2Id ";
                Query += "FROM GoodsReceivedD GRD INNER JOIN TransStatusTable b on GRD.ActionCodeStatus=b.StatusCode And b.TransCode = 'GRD' ";
                Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                Query += "WHERE GRD.GoodsReceivedId = '" + GRNumber + "' And GRD.ActionCodeStatus = '05' And GoodsReceivedSeqNo Not In (" + addIn + ") ";
                if (Parent.Mode == "New")
                {
                    Query += "AND GRD.Remaining_Qty <> 0 ";
                }
            }
            else
            {
                //Query = "SELECT DISTINCT FullItemID, ItemName, Qty_Actual, ISNULL(Remaining_Qty, 0) AS Remaining_Qty, Unit, InventSiteID, InventSiteBlokID, Quality, Notes, b.Deskripsi, GoodsReceivedSeqNo, GroupId, SubGroup1Id, SubGroup2Id FROM GoodsReceivedD a INNER JOIN TransStatusTable b on a.ActionCodeStatus=b.StatusCode And b.TransCode = 'GRD' WHERE a.GoodsReceivedId = '" + GRNumber + "' And a.ActionCodeStatus = '05'";
                Query = "SELECT DISTINCT GRD.FullItemID, GRD.ItemName, Qty_Actual, ISNULL(Remaining_Qty, 0) AS Remaining_Qty, GRD.Unit, ISNULL(POD.Price, 0) AS Price, GRD.InventSiteID, InventSiteBlokID, Quality, Notes, b.Deskripsi, GoodsReceivedSeqNo, GRD.GroupId, GRD.SubGroup1Id, GRD.SubGroup2Id ";
                Query += "FROM GoodsReceivedD GRD INNER JOIN TransStatusTable b on GRD.ActionCodeStatus=b.StatusCode And b.TransCode = 'GRD' ";
                Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                Query += "WHERE GRD.GoodsReceivedId = '" + GRNumber + "' And GRD.ActionCodeStatus = '05' ";
                if (Parent.Mode == "New")
                {
                    Query += "AND GRD.Remaining_Qty <> 0 ";
                }
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;

            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                if (Parent.Mode == "New")
                {
                    this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemName"], Dr["Qty_Actual"], Dr["Remaining_Qty"], Dr["Unit"], Dr["Price"], Dr["InventSiteID"], Dr["InventSiteBlokID"], Dr["Quality"], Dr["Notes"], Dr["Deskripsi"], Dr["GoodsReceivedSeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"]);
                }
                else if (Parent.Mode == "Edit")
                {
                    if (Parent.DelStatusItem == "ItemDeleted")
                    {
                        for (int j = 0; j < Parent.DelSeq_No.Count; j++)
                        {
                            if (Dr["FullItemId"].ToString() == Parent.DelFullItemID[j] && int.Parse(Dr["GoodsReceivedSeqNo"].ToString()) == Parent.DelSeq_No[j])
                            {
                                this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemName"], Dr["Qty_Actual"], decimal.Parse(Dr["Remaining_Qty"].ToString()) + Parent.DelUoM_Qty[j], Dr["Unit"], Dr["Price"], Dr["InventSiteID"], Dr["InventSiteBlokID"], Dr["Quality"], Dr["Notes"], Dr["Deskripsi"], Dr["GoodsReceivedSeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"]);
                            }
                        }
                    }
                    else
                    {
                        this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemName"], Dr["Qty_Actual"], Dr["Remaining_Qty"], Dr["Unit"], Dr["Price"], Dr["InventSiteID"], Dr["InventSiteBlokID"], Dr["Quality"], Dr["Notes"], Dr["Deskripsi"], Dr["GoodsReceivedSeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"]);
                    }
                }
                if (dgvItem.Rows.Count > 0)
                {
                    dgvItem.Rows[i].Cells["Check"] = chk;
                    dgvItem.Rows[i].Cells["Check"].Value = false;
                }
                i++;
            }

            dgvItem.ReadOnly = false;
            dgvItem.Columns["Check"].ReadOnly = false;
            dgvItem.Columns["FullItemId"].ReadOnly = true;
            dgvItem.Columns["Item Name"].ReadOnly = true;
            dgvItem.Columns["Qty Actual"].ReadOnly = true;
            dgvItem.Columns["Remaining_Qty"].ReadOnly = true;
            dgvItem.Columns["Unit"].ReadOnly = true;
            dgvItem.Columns["Price"].Visible = false;
            dgvItem.Columns["InventSiteID"].Visible = false;
            dgvItem.Columns["InventSiteBlokID"].Visible = false;
            dgvItem.Columns["Quality"].Visible = false;
            dgvItem.Columns["Notes"].Visible = false;
            dgvItem.Columns["Action"].Visible = false;
            dgvItem.Columns["SeqNo"].Visible = false;
            dgvItem.Columns["GroupId"].Visible = false;
            dgvItem.Columns["SubGroup1Id"].Visible = false;
            dgvItem.Columns["SubGroup2Id"].Visible = false;
            dgvItem.AutoResizeColumns();
            Conn.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean CekList = chkAll.Checked;
            for (int i = 0; i < dgvItem.RowCount; i++)
            {
                dgvItem.Rows[i].Cells["Check"].Value = CekList;
            }
        }

        private void SelectItem()
        {
            int CountChk = 0;
            List<string> SelectSeqNo = new List<string>();
            for (int i = 0; i < dgvItem.RowCount; i++)
            {
                Boolean CekList = false;
                if (dgvItem.Rows[i].Cells["Check"].Value == null || dgvItem.Rows[i].Cells["Check"].Value.ToString() == "")
                {
                    
                }
                else
                {
                    CekList = Convert.ToBoolean(dgvItem.Rows[i].Cells["Check"].Value);
                }
                if (CekList == true)
                {
                    CountChk++;
                    SelectSeqNo.Add(dgvItem.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvItem.Rows[i].Cells["Seqno"].Value.ToString());
                }
            }
            Parent.AddDataGridDetail(SelectSeqNo);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectItem();
        }
    }
}
