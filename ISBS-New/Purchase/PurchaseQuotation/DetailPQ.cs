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
        Purchase.RFQ.CreatePQ Parent1;

        public DetailPQ()
        {
            InitializeComponent();
        }

        private void DetailPQ_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            if(Parent != null)
                Query = "Select ROW_NUMBER() OVER (ORDER BY FullItemId) No, [RfqSeqNo], GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemDeskripsi, [Qty], [Unit], [Deskripsi], a.PurchReqId From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqID=b.RfqID Where a.RfqID = '" + PrNumber + "' " + Parent.DetailHeaderItem() + " order by a.RfqSeqNo asc";
            else
                Query = "Select ROW_NUMBER() OVER (ORDER BY FullItemId) No, [RfqSeqNo], GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemDeskripsi, [Qty], [Unit], [Deskripsi], a.PurchReqId From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqID=b.RfqID Where a.RfqID = '" + PrNumber + "' " + Parent1.DetailHeaderItem() + " order by a.RfqSeqNo asc";
            
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
            dgvDetailPQ.Columns["RfqSeqNo"].ReadOnly = true;
            dgvDetailPQ.Columns["GroupId"].ReadOnly = true;
            dgvDetailPQ.Columns["SubGroup1Id"].ReadOnly = true;
            dgvDetailPQ.Columns["SubGroup2Id"].ReadOnly = true;
            dgvDetailPQ.Columns["ItemId"].ReadOnly = true;
            dgvDetailPQ.Columns["FullItemID"].ReadOnly = true;
            dgvDetailPQ.Columns["ItemDeskripsi"].ReadOnly = true;
            dgvDetailPQ.Columns["Qty"].ReadOnly = true;
            dgvDetailPQ.Columns["Unit"].ReadOnly = true;
            dgvDetailPQ.Columns["Deskripsi"].ReadOnly = true;
            dgvDetailPQ.Columns["PurchReqId"].ReadOnly = true;

            dgvDetailPQ.Columns["GroupId"].Visible = false;
            dgvDetailPQ.Columns["SubGroup1ID"].Visible = false;
            dgvDetailPQ.Columns["SubGroup2ID"].Visible = false;
            dgvDetailPQ.Columns["ItemID"].Visible = false;
            dgvDetailPQ.Columns["PurchReqId"].Visible = false;
            dgvDetailPQ.AutoResizeColumns();

            Conn.Close();

        }

        public void SetParent(Purchase.PurchaseQuotation.FormPQ F)
        {
            Parent = F;
        }

        public void SetParent(Purchase.RFQ.CreatePQ F)
        {
            Parent1 = F;
        }

        public void SetPrNumber(string tmpPrNumber)
        {
            PrNumber = tmpPrNumber;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> FullItemID = new List<string>();
            List<string> PRId = new List<string>();
            List<string> GelombangID = new List<string>();
            List<string> PurchReqSeqNo = new List<string>();
            List<string> SeqNoGroup = new List<string>();
            int CountChk = 0;
            for (int i = 0; i <= dgvDetailPQ.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvDetailPQ.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    SeqNoGroup.Add(dgvDetailPQ.Rows[i].Cells["SeqNoGroup"].Value == null ? "" : dgvDetailPQ.Rows[i].Cells["SeqNoGroup"].Value.ToString());
                    PRId.Add(dgvDetailPQ.Rows[i].Cells["PurchReqId"].Value == null ? "" : dgvDetailPQ.Rows[i].Cells["PurchReqId"].Value.ToString());
                }
            }

            Parent.AddDataGridDetail(PRId, SeqNoGroup);
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
