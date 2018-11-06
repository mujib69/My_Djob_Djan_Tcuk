using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseQuotation
{
    public partial class DetailPQ : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;

        string PrNumber = "";
        Purchase.PurchaseQuotation.FormPQ Parent;

        public DetailPQ()
        {
            InitializeComponent();
        }

        private void DetailPQ_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            lblForm.Location = new Point(16, 11);
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select ROW_NUMBER() OVER (ORDER BY FullItemId) No, [SeqNo], GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, [Qty], [Unit], [Deskripsi], a.PurchReqId, TransType From [PurchRequisition_Dtl] a inner join PurchRequisitionH b on a.PurchReqId=b.PurchReqId Where a.PurchReqID = '" + PrNumber + "' " + Parent.DetailHeaderItem() + " order by a.SeqNo asc";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvDetailPQ.AutoGenerateColumns = true;
            dgvDetailPQ.DataSource = Dt;
            dgvDetailPQ.Refresh();
            if (dgvDetailPQ.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvDetailPQ.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            dgvDetailPQ.ReadOnly = false;
            dgvDetailPQ.Columns["SeqNo"].ReadOnly = true;
            dgvDetailPQ.Columns["GroupId"].ReadOnly = true;
            dgvDetailPQ.Columns["SubGroup1Id"].ReadOnly = true;
            dgvDetailPQ.Columns["SubGroup2Id"].ReadOnly = true;
            dgvDetailPQ.Columns["ItemId"].ReadOnly = true;
            dgvDetailPQ.Columns["FullItemID"].ReadOnly = true;
            dgvDetailPQ.Columns["ItemName"].ReadOnly = true;
            dgvDetailPQ.Columns["Qty"].ReadOnly = true;
            dgvDetailPQ.Columns["Unit"].ReadOnly = true;
            dgvDetailPQ.Columns["Deskripsi"].ReadOnly = true;
            dgvDetailPQ.Columns["PurchReqId"].ReadOnly = true;
            dgvDetailPQ.Columns["TransType"].ReadOnly = true;

            dgvDetailPQ.Columns["GroupId"].Visible = false;
            dgvDetailPQ.Columns["SubGroup1ID"].Visible = false;
            dgvDetailPQ.Columns["SubGroup2ID"].Visible = false;
            dgvDetailPQ.Columns["ItemID"].Visible = false;
            dgvDetailPQ.Columns["PurchReqId"].Visible = false;
            dgvDetailPQ.Columns["TransType"].Visible = false;
            dgvDetailPQ.AutoResizeColumns();

            Conn.Close();

        }

        public void SetParent(Purchase.PurchaseQuotation.FormPQ F)
        {
            Parent = F;
        }

        public void SetPrNumber(string tmpPrNumber)
        {
            PrNumber = tmpPrNumber;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> SeqNum = new List<string>();
            int CountChk = 0;
            for (int i = 0; i <= dgvDetailPQ.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvDetailPQ.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    SeqNum.Add(dgvDetailPQ.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvDetailPQ.Rows[i].Cells["SeqNo"].Value.ToString());
                    //ItemDesc.Add(dgvDetailPQ.Rows[i].Cells["ItemName"].Value == null ? "" : dgvDetailPQ.Rows[i].Cells["ItemName"].Value.ToString());
                }
            }

            Parent.AddDataGridDetail(SeqNum);
            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvDetailPQ.RowCount - 1; i++)
            {
                dgvDetailPQ.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void DetailPQ_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }



    }
}
