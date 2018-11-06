using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.ReturTukarBarang
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

        String Query, GRNumber;
        Purchase.Retur.ReturTukarBarang.RTBHeader Parent;
        List<string> SeqNo;

        public AddItem()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.ReturTukarBarang.RTBHeader F)
        {
            Parent = F;
        }

        public void setMode(String grnumber, List<string> seqno)
        {
            GRNumber = grnumber;
            SeqNo = seqno;
        }

        private void AddItem_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            if (dgvItem.RowCount - 1 <= 0)
            {
                dgvItem.ColumnCount = 11;
                dgvItem.Columns[0].Name = "Check";
                dgvItem.Columns[1].Name = "FullItemId";
                dgvItem.Columns[2].Name = "Item Name";
                dgvItem.Columns[3].Name = "Qty Actual";
                dgvItem.Columns[4].Name = "Unit";
                dgvItem.Columns[5].Name = "InventSiteID";
                dgvItem.Columns[6].Name = "InventSiteBlokID";
                dgvItem.Columns[7].Name = "Quality";
                dgvItem.Columns[8].Name = "Notes";
                dgvItem.Columns[9].Name = "Action";
                dgvItem.Columns[10].Name = "SeqNo";
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

                Query = "Select Distinct [FullItemID], ItemName, [Qty_Actual], [Unit], InventSiteID, InventSiteBlokID, Quality, Notes, b.Deskripsi, GoodsReceivedSeqNo From ([GoodsReceivedD] a inner join TransStatusTable b on a.ActionCodeStatus=b.StatusCode And b.TransCode = 'GRD') Where a.GoodsReceivedId = '" + GRNumber + "' And a.ActionCodeStatus = '05' And GoodsReceivedSeqNo Not In (" + addIn + ")";
            }
            else
            {
                Query = "Select Distinct [FullItemID], ItemName, [Qty_Actual], [Unit], InventSiteID, InventSiteBlokID, Quality, Notes, b.Deskripsi, GoodsReceivedSeqNo From ([GoodsReceivedD] a inner join TransStatusTable b on a.ActionCodeStatus=b.StatusCode And b.TransCode = 'GRD') Where a.GoodsReceivedId = '" + GRNumber + "' And a.ActionCodeStatus = '05'";
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;

            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();

                this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemName"], Dr["Qty_Actual"], Dr["Unit"], Dr["InventSiteID"], Dr["InventSiteBlokID"], Dr["Quality"], Dr["Notes"], Dr["Deskripsi"], Dr["GoodsReceivedSeqNo"]);
                dgvItem.Rows[i].Cells["Check"] = chk;
                dgvItem.Rows[i].Cells["Check"].Value = false;

                i++;
            }

            dgvItem.ReadOnly = false;
            dgvItem.Columns["Check"].ReadOnly = false;
            dgvItem.Columns["FullItemID"].ReadOnly = true;
            dgvItem.Columns["Item Name"].ReadOnly = true;
            dgvItem.Columns["Qty Actual"].ReadOnly = true;
            dgvItem.Columns["Unit"].ReadOnly = true;
            dgvItem.Columns["InventSiteID"].ReadOnly = true;
            dgvItem.Columns["InventSiteBlokID"].ReadOnly = true;
            dgvItem.Columns["Quality"].ReadOnly = true;
            dgvItem.Columns["Notes"].ReadOnly = true;
            dgvItem.Columns["Action"].ReadOnly = true;

            dgvItem.Columns["SeqNo"].Visible = true;
            dgvItem.AutoResizeColumns();

            Conn.Close();

        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvItem.RowCount - 1; i++)
            {
                dgvItem.Rows[i].Cells["Check"].Value = Check;
            }
        }

        private void SelectItem()
        {
            int CountChk = 0;
            List<string> SelectSeqNo = new List<string>();
            for (int i = 0; i <= dgvItem.RowCount - 1; i++)
            {
                Boolean Check = false;
                if (dgvItem.Rows[i].Cells["Check"].Value == null || dgvItem.Rows[i].Cells["Check"].Value.ToString() == "")
                {

                }
                else
                {
                    Check = Convert.ToBoolean(dgvItem.Rows[i].Cells["Check"].Value);
                }

                if (Check == true)
                {
                    CountChk++;
                    SelectSeqNo.Add(dgvItem.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvItem.Rows[i].Cells["SeqNo"].Value.ToString());
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
