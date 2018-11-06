using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.NotaReturJual
{
    public partial class AddItem : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd,cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Query, BBKNumber;
        Purchase.Retur.NotaReturJual.ReturJualHeader Parent;
        List<string> GISeqNo;

        public AddItem()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.NotaReturJual.ReturJualHeader F)
        {
            Parent = F;
        }

        public void setMode(String bbknumber, List<string> seqno)
        {
            BBKNumber = bbknumber;
            GISeqNo = seqno;
        }

        private void AddItem_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            if (dgvItem.RowCount - 1 <= 0)
            {
                dgvItem.ColumnCount = 8;
                dgvItem.Columns[0].Name = "Check";
                dgvItem.Columns[1].Name = "FullItemId";
                dgvItem.Columns[2].Name = "Item Name";
                dgvItem.Columns[3].Name = "Qty";
                dgvItem.Columns[4].Name = "Unit";
                dgvItem.Columns[5].Name = "Remaining Qty";
                dgvItem.Columns[6].Name = "InventSiteBlokID";
                dgvItem.Columns[7].Name = "GoodsIssuedSeqNo";
            }

            if (GISeqNo.Count > 0)
            {
                string addIn = "";

                for (int x = 0; x < GISeqNo.Count; x++)
                {
                    addIn += "'" + GISeqNo[x] + "'";
                    if (x == GISeqNo.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        addIn += ",";
                    }
                }

                Query = "Select [FullItemID], ItemName, [Qty], [Unit],[InventSiteBlokID], [GoodsIssuedSeqNo] From [GoodsIssuedD] Where GoodsIssuedId = '" + BBKNumber + "' And [GoodsIssuedSeqNo] Not In (" + addIn + ")";
            }
            else
            {
                Query = "Select [FullItemID], ItemName, [Qty], [Unit], [InventSiteBlokID], [GoodsIssuedSeqNo] From [GoodsIssuedD] Where GoodsIssuedId = '" + BBKNumber + "'";
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;

            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();

                Query = "Select Remaining_Qty From [GoodsIssuedD] Where GoodsIssuedId = '" + BBKNumber + "' And GoodsIssuedSeqNo = '" + Dr["GoodsIssuedSeqNo"] + "'";
                cmd = new SqlCommand(Query, Conn);

                this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["Unit"], cmd.ExecuteScalar(), Dr["InventSiteBlokID"], Dr["GoodsIssuedSeqNo"]);
                dgvItem.Rows[i].Cells["Check"] = chk;
                dgvItem.Rows[i].Cells["Check"].Value = false;

                i++;
            }

            dgvItem.ReadOnly = false;
            dgvItem.Columns["Check"].ReadOnly = false;
            dgvItem.Columns["FullItemID"].ReadOnly = true;
            dgvItem.Columns["Item Name"].ReadOnly = true;
            dgvItem.Columns["Qty"].ReadOnly = true;
            dgvItem.Columns["Unit"].ReadOnly = true;
            dgvItem.Columns["Remaining Qty"].ReadOnly = true;
            dgvItem.Columns["InventSiteBlokID"].ReadOnly = true;
            dgvItem.Columns["GoodsIssuedSeqNo"].ReadOnly = true;
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
            List<string> SeqNo = new List<string>();
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
                    SeqNo.Add(dgvItem.Rows[i].Cells["GoodsIssuedSeqNo"].Value == null ? "" : dgvItem.Rows[i].Cells["GoodsIssuedSeqNo"].Value.ToString());
                }
            }
            Parent.AddDataGridDetail(SeqNo);
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
