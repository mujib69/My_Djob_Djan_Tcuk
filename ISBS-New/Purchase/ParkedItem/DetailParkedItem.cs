using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Purchase.ParkedItem
{
    public partial class DetailParkedItem : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit, GoodsReceivedId = null;
        DataGridView dgvDdetailFromHeader;
        public String EditFrom = "";
        String NotaNumber = "0";

        Purchase.ParkedItem.HeaderParkedItem Parent;

        public DetailParkedItem()
        {
            InitializeComponent();           
        }

        public void ParentRefreshGrid(Purchase.ParkedItem.HeaderParkedItem F)
        {
            Parent = F;
        }

        private void DetailParkedItem_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);

            cmbCriteria.SelectedIndex = 0;

        }

        private void DetailParkedItem_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshDataGrid();
        }

        public void ParamHeader(string prmGoodsReceivedId, DataGridView prmGridFromHeader, string prmEditFrom, string prmNotaNumber)
        {
            GoodsReceivedId = prmGoodsReceivedId;
            dgvDdetailFromHeader = prmGridFromHeader;
            EditFrom = prmEditFrom;
            NotaNumber = prmNotaNumber;
        }

        private void RefreshDataGrid()
        {
            
            Query = "SELECT '' AS No, GoodsReceivedSeqNo AS SeqNo, a.FullItemId, a.ItemName, a.Qty_Actual AS Qty, a.Unit FROM GoodsReceivedD a WHERE a.GoodsReceivedId = '" + GoodsReceivedId + "' AND a.ActionCodeStatus = '02' ";
           
            
            if (cmbCriteria.SelectedIndex == 0)
            {
                Query += "AND (FullItemId like'%" + txtSearch.Text + "%' or ItemName like '%" + txtSearch.Text + "%') ";
            }
            else if (cmbCriteria.SelectedIndex == 1)
            {
                Query += "AND (FullItemId like'%" + txtSearch.Text + "%') ";
            }
            else
            {
                Query += "AND (ItemName like '%" + txtSearch.Text + "%') ";
            }

            if(EditFrom != "")
            {
                Query += "UNION ";
                Query += "SELECT '' AS No, b.SeqNo, b.FullItemId, b.ItemName, b.Qty, b.Unit ";
                Query += "FROM NotaPurchaseParkD b ";
                Query += "WHERE b.NPPID = '"+NotaNumber+"'";
            }

            Conn = ConnectionString.GetConnection();


            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvParkedItemDetails.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvParkedItemDetails.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);

            dgvParkedItemDetails.AutoGenerateColumns = true;
            dgvParkedItemDetails.DataSource = Dt;
            dgvParkedItemDetails.Refresh();


           
            string SeqNoH = "";
            string SeqNoD = "";
            List<string> RemoveSeqNo = new List<string>();

            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {             
                SeqNoD = dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value.ToString();

                for (int j = 0; j < dgvDdetailFromHeader.RowCount; j++)
                {
                    SeqNoH = dgvDdetailFromHeader.Rows[j].Cells["SeqNo"].Value.ToString();

                    if (SeqNoD == SeqNoH)
                    {
                        RemoveSeqNo.Add(SeqNoD);                       
                    }
                }               
            }

            for (int i = 0; i < RemoveSeqNo.Count; i++)
            {
                for (int j = 0; j < dgvParkedItemDetails.RowCount; j++)
                {
                    SeqNoD = dgvParkedItemDetails.Rows[j].Cells["SeqNo"].Value.ToString();
                    if (SeqNoD == RemoveSeqNo[i])
                    {
                        dgvParkedItemDetails.Rows.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < dgvParkedItemDetails.RowCount; i++)
            {
                dgvParkedItemDetails.Rows[i].Cells["No"].Value = i + 1;
            }



           //  string Noj = "";
           // int k = 1;
            //int l = dgvParkedItemDetails.RowCount - 1;
              
            //for (int i = 0; i < dgvDdetailFromHeader.RowCount; i++)
            //{

            //    FullItemIdi = dgvDdetailFromHeader.Rows[i].Cells["FullItemId"].Value.ToString();
            //    ItemNamei = dgvDdetailFromHeader.Rows[i].Cells["ItemName"].Value.ToString();
                
            //    for (int j = 0; j < l; j++)
            //    {

            //        Noj = dgvParkedItemDetails.Rows[j].Cells["No"].Value.ToString();
            //        FullItemIdj = dgvParkedItemDetails.Rows[j].Cells["FullItemId"].Value.ToString();
            //        ItemNamej = dgvParkedItemDetails.Rows[j].Cells["ItemName"].Value.ToString();

            //        if (FullItemIdj == FullItemIdi && ItemNamej == ItemNamei)
            //        {
            //            dgvParkedItemDetails.Rows.RemoveAt(j);
            //        }
            //        else {
            //            dgvParkedItemDetails.Rows[j].Cells["No"].Value = k;
            //            k++;
            //        }
                   
            //    }
            //    l = dgvParkedItemDetails.RowCount - 1;
            //}

            //EndLoop:
            dgvParkedItemDetails.ReadOnly = false;
            dgvParkedItemDetails.Columns["No"].ReadOnly = true;
            dgvParkedItemDetails.Columns["FullItemId"].ReadOnly = true;
            dgvParkedItemDetails.Columns["ItemName"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Qty"].ReadOnly = true;
            dgvParkedItemDetails.Columns["Unit"].ReadOnly = true;
            dgvParkedItemDetails.Columns["SeqNo"].ReadOnly = true;
            dgvParkedItemDetails.Columns["SeqNo"].Visible = false;
            dgvParkedItemDetails.AutoResizeColumns();

            Conn.Close();

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            MethodSelectData();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDataGrid();

            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
            {
                dgvParkedItemDetails.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void MethodSelectData()
        {
            List<string> FullItemId = new List<string>();
            List<string> ItemName = new List<string>();
            List<string> Qty = new List<string>();
            List<string> Unit = new List<string>();
            List<string> SeqNo = new List<string>();

            int CountChk = 0;
            for (int i = 0; i <= dgvParkedItemDetails.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvParkedItemDetails.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    FullItemId.Add(dgvParkedItemDetails.Rows[i].Cells["FullItemId"].Value.ToString());
                    ItemName.Add(dgvParkedItemDetails.Rows[i].Cells["ItemName"].Value.ToString());
                    Qty.Add(dgvParkedItemDetails.Rows[i].Cells["Qty"].Value.ToString());
                    Unit.Add(dgvParkedItemDetails.Rows[i].Cells["Unit"].Value.ToString());
                    SeqNo.Add(dgvParkedItemDetails.Rows[i].Cells["SeqNo"].Value.ToString());
                }
            }

            Parent.AddDataGridDetail(FullItemId, ItemName, Qty, Unit, SeqNo);

            this.Close();
        }
    }
}
