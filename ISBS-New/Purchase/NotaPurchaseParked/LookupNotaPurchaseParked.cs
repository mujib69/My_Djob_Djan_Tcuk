using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.NotaPurchaseParked
{
    public partial class LookupNotaPurchaseParked : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        public string NotaNumber, GoodsReceived;

        string Query = "";

        NotaPurchaseParked.HeaderNotaPurchaseParked Parent;

        public LookupNotaPurchaseParked()
        {
            InitializeComponent();
        }

        private void LookupNotaPurchaseParked_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT No, FullItemID, ItemName, '0.00' AS Qty, Unit, Price, SeqNo, ";
            Query += "(SELECT Ratio FROM GoodsReceivedD ";
            Query += "WHERE GoodsReceivedId = (SELECT GoodsReceivedId FROM NotaPurchaseParkH WHERE NPPID = '" + NotaNumber + "' AND GoodsReceivedId = a.GoodsReceivedID) AND GoodsReceivedSeqNo = a.GoodsReceived_SeqNo) AS Ratio, GoodsReceived_SeqNo ";
            Query += "FROM (Select ROW_NUMBER() OVER (ORDER BY NPPID DESC) No, FullItemID, ItemName, Unit, Price, SeqNo, GoodsReceived_SeqNo, GoodsReceivedID FROM NotaPurchaseParkD WHERE NPPID = '" + NotaNumber + "' AND GoodsReceivedID = '" + GoodsReceived + "') a ORDER BY a.SeqNo ASC";
          
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvNotaPurchaseParked.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvNotaPurchaseParked.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);
            dgvNotaPurchaseParked.AutoGenerateColumns = true;
            dgvNotaPurchaseParked.ReadOnly = false;
            dgvNotaPurchaseParked.DataSource = Dt;
            dgvNotaPurchaseParked.Refresh();

            dgvNotaPurchaseParked.ReadOnly = false;
            dgvNotaPurchaseParked.Columns["No"].ReadOnly = true;
            dgvNotaPurchaseParked.Columns["FullItemID"].ReadOnly = true;
            dgvNotaPurchaseParked.Columns["ItemName"].ReadOnly = true;
            dgvNotaPurchaseParked.Columns["Price"].ReadOnly = true;


            dgvNotaPurchaseParked.Columns["Qty"].Visible = false;
            dgvNotaPurchaseParked.Columns["Unit"].Visible = false;
            //dgvNotaPurchaseParked.Columns["Price"].Visible = false;
            dgvNotaPurchaseParked.Columns["SeqNo"].Visible = false;
            dgvNotaPurchaseParked.Columns["Ratio"].Visible = false;
            dgvNotaPurchaseParked.Columns["GoodsReceived_SeqNo"].Visible = false;

            Conn.Close();
           
        }

        private void LookupNotaPurchaseParked_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, (63));
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> FullItemID = new List<string>();
            List<string> ItemName = new List<string>();
            List<decimal> Qty = new List<decimal>();
            List<string> Unit = new List<string>();
            List<decimal> Price = new List<decimal>();
            List<string> SeqNo = new List<string>();
            List<decimal> Ratio = new List<decimal>();
            List<string> GoodsReceived_SeqNo = new List<string>();

            int CountChk = 0;

            for (int i = 0; i <= dgvNotaPurchaseParked.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvNotaPurchaseParked.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    FullItemID.Add(dgvNotaPurchaseParked.Rows[i].Cells["FullItemID"].Value.ToString());
                    ItemName.Add(dgvNotaPurchaseParked.Rows[i].Cells["ItemName"].Value.ToString());
                    Qty.Add(Convert.ToDecimal(dgvNotaPurchaseParked.Rows[i].Cells["Qty"].Value));
                    Unit.Add(dgvNotaPurchaseParked.Rows[i].Cells["Unit"].Value.ToString());
                    Price.Add(Convert.ToDecimal(dgvNotaPurchaseParked.Rows[i].Cells["Price"].Value));
                    SeqNo.Add(dgvNotaPurchaseParked.Rows[i].Cells["SeqNo"].Value.ToString());
                    Ratio.Add(Convert.ToDecimal(dgvNotaPurchaseParked.Rows[i].Cells["Ratio"].Value));
                    GoodsReceived_SeqNo.Add(Convert.ToString(dgvNotaPurchaseParked.Rows[i].Cells["GoodsReceived_SeqNo"].Value));
                }
            }

            if (CountChk == 0)
            {
                MessageBox.Show("Silahkan checklist data");
                return;
            }

            Parent.AddDataGridNotaPurchaseParked(FullItemID, ItemName, Qty, Unit, Price, SeqNo, Ratio, GoodsReceived_SeqNo);
            this.Close();
        }

        public void ParamHeader(string prmNotaNumber, string prmGoodsReceived)
        {
            NotaNumber = prmNotaNumber;
            GoodsReceived = prmGoodsReceived;
        }

        public void ParentRefreshGrid(NotaPurchaseParked.HeaderNotaPurchaseParked F)
        {
            Parent = F;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
