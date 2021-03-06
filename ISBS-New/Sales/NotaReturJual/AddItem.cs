﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Sales.NotaReturJual
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

        string Query, GINumber;
        Sales.NotaReturJual.NRJHeader Parent;
        List<string> SeqNo;

        public AddItem()
        {
            InitializeComponent();
        }

        public void setParent(Sales.NotaReturJual.NRJHeader F)
        {
            Parent = F;
        }

        public void setMode(string ginumber, List<string> seqno)
        {
            GINumber = ginumber;
            SeqNo = seqno;
        }

        private void AddItem_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if (dgvItem.RowCount - 1 <= 0)
            {
                dgvItem.ColumnCount = 15;
                dgvItem.Columns[0].Name = "Check";
                dgvItem.Columns[1].Name = "FullItemId";
                dgvItem.Columns[2].Name = "Item Name";
                dgvItem.Columns[3].Name = "Qty Actual"; dgvItem.Columns["Qty Actual"].HeaderText = "Qty";
                dgvItem.Columns[4].Name = "Remaining_Qty"; dgvItem.Columns["Remaining_Qty"].HeaderText = "Remaining Qty";
                dgvItem.Columns[5].Name = "Unit";
                dgvItem.Columns[6].Name = "Price";
                dgvItem.Columns[7].Name = "InventSiteID";
                dgvItem.Columns[8].Name = "InventSiteBlokID";
                dgvItem.Columns[9].Name = "Notes";
                dgvItem.Columns[10].Name = "Action";
                dgvItem.Columns[11].Name = "SeqNo";
                dgvItem.Columns[12].Name = "GroupId";
                dgvItem.Columns[13].Name = "SubGroup1Id";
                dgvItem.Columns[14].Name = "SubGroup2Id";
            }

            if (SeqNo.Count > 0)
            {
                string addIn = "";
                for (int x = 0; x < SeqNo.Count; x++)
                {
                    addIn += "'" + SeqNo[x] + "'";
                    if (x == SeqNo.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        addIn += ",";
                    }
                }
                Query = "SELECT GID.FullItemId, GID.ItemName, GID.Qty_Actual, ISNULL(GID.Remaining_Qty, 0) AS Remaining_Qty, GID.Unit, ISNULL(SOD.Price, 0) AS Price, GID.InventSiteId, ";
                Query += "GID.InventSiteBlokID, GID.Notes, TST.Deskripsi, GID.GoodsIssuedSeqNo, GID.GroupId, GID.SubGroup1Id, GID.SubGroup2Id ";
                Query += "FROM GoodsIssuedD GID INNER JOIN TransStatusTable TST ON GID.ActionCode = TST.StatusCode AND TST.TransCode = 'GID' ";
                Query += "LEFT JOIN DeliveryOrderD DOD ON GID.RefTransID = DOD.DeliveryOrderId AND DOD.SeqNo = GID.RefTransSeqNo ";
                Query += "LEFT JOIN SalesOrderD SOD ON DOD.SalesOrderId = SOD.SalesOrderNo AND SOD.SeqNo = DOD.SalesOrderSeqNo ";
                Query += "WHERE GID.GoodsIssuedId = '" + GINumber + "' AND GID.ActionCode = '01' AND GID.Remaining_Qty <> 0 AND GoodsIssuedSeqNo NOT IN (" + addIn + ") ";
            }
            else
            {
                Query = "SELECT GID.FullItemId, GID.ItemName, GID.Qty_Actual, ISNULL(GID.Remaining_Qty, 0) AS Remaining_Qty, GID.Unit, ISNULL(SOD.Price, 0) AS Price, GID.InventSiteId, ";
                Query += "GID.InventSiteBlokID, GID.Notes, TST.Deskripsi, GID.GoodsIssuedSeqNo, GID.GroupId, GID.SubGroup1Id, GID.SubGroup2Id ";
                Query += "FROM GoodsIssuedD GID INNER JOIN TransStatusTable TST ON GID.ActionCode = TST.StatusCode AND TST.TransCode = 'GID' ";
                Query += "LEFT JOIN DeliveryOrderD DOD ON GID.RefTransID = DOD.DeliveryOrderId AND DOD.SeqNo = GID.RefTransSeqNo ";
                Query += "LEFT JOIN SalesOrderD SOD ON DOD.SalesOrderId = SOD.SalesOrderNo AND SOD.SeqNo = DOD.SalesOrderSeqNo ";
                Query += "WHERE GID.GoodsIssuedId = '" + GINumber + "' AND GID.ActionCode = '01' AND GID.Remaining_Qty <> 0 ";                
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;

            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemName"], Dr["Qty_Actual"], Dr["Remaining_Qty"], Dr["Unit"], Dr["Price"], Dr["InventSiteID"], Dr["InventSiteBlokID"], Dr["Notes"], Dr["Deskripsi"], Dr["GoodsIssuedSeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"]);
                dgvItem.Rows[i].Cells["Check"] = chk;
                dgvItem.Rows[i].Cells["Check"].Value = false;
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
