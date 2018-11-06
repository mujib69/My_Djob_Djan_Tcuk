using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseAgreement
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

        String Mode, Query, crit = null;
        int flagRefresh = 0;
        String AgreementId = "";
        DataGridView dgvHeader;

        Purchase.PurchaseAgreement.PAForm Parent;

        public void setParent(Purchase.PurchaseAgreement.PAForm F)
        {
            Parent = F;
        }

        public void flag(String agreementid, DataGridView prmDgvHeader)
        {
            AgreementId = agreementid;
            dgvHeader = prmDgvHeader;
        }

        public AddItem()
        {
            InitializeComponent();
        }

        private void AddItem_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            Query = "select TransType from PurchAgreementH where AgreementID = '" + AgreementId + "'";
            Cmd = new SqlCommand(Query, Conn);
            string TransType = Cmd.ExecuteScalar().ToString();

            if (dgvItem.RowCount - 1 <= 0)
            {
                dgvItem.ColumnCount = 10;
                dgvItem.Columns[0].Name = "Check";
                dgvItem.Columns[1].Name = "FullItemId";
                dgvItem.Columns[2].Name = "Item Name";
                dgvItem.Columns[3].Name = "Unit";
                dgvItem.Columns[4].Name = "Purchase Agreement ID";
                dgvItem.Columns[5].Name = "PA Sequence Number";
                dgvItem.Columns[6].Name = "Deskripsi";
                if (TransType == "QTY")
                {
                    dgvItem.Columns[7].Name = "Qty";
                    dgvItem.Columns[8].Name = "Remaining Qty";
                }
                else if (TransType == "AMOUNT")
                {
                    dgvItem.Columns[7].Name = "Amount";
                    dgvItem.Columns[8].Name = "Remaining Amount";
                }
                dgvItem.Columns[9].Name = "SeqNoGroup";

            }


            string DataExisting = "";
            if (dgvHeader.RowCount > 0)
            {
                for (int j = 0; j < dgvHeader.RowCount; j++)
                {
                    DataExisting += "'" + dgvHeader.Rows[j].Cells["FullItemID"].Value + "',";
                }
                DataExisting = DataExisting.Remove(DataExisting.Length - 1);

            }
            if (DataExisting == "")
            {
                DataExisting = "''";
            }

            if (TransType == "QTY")
            {
                Query = "Select a.[FullItemID], a.ItemName, [Unit], a.AgreementID,a.SeqNo,a.Deskripsi,a.Qty,a.RemainingQty,a.Base, a.SeqNoGroup ";
                Query += "From ([PurchAgreementDtl] a join PurchAgreementH b on a.AgreementId=b.AgreementId) ";
                Query += "Where a.FullItemID NOT IN (" + DataExisting + ") And a.AgreementId = '" + AgreementId + "' And a.RemainingQty > 0  Order By SeqNo Asc";
            }
            else if (TransType == "AMOUNT")
            {
                Query = "Select a.[FullItemID], a.ItemName, [Unit], a.AgreementID,a.SeqNo,a.Deskripsi,a.Amount,a.RemainingAmount,a.Base, a.SeqNoGroup ";
                Query += "From ([PurchAgreementDtl] a join PurchAgreementH b on a.AgreementId=b.AgreementId) ";
                Query += "Where a.FullItemID NOT IN (" + DataExisting + ") And a.AgreementId = '" + AgreementId + "' And a.RemainingAmount > 0  Order By SeqNo Asc";
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;

            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();

                if (TransType == "QTY")
                    this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemName"], Dr["Unit"], Dr["AgreementID"], Dr["SeqNo"], Dr["Deskripsi"], Convert.ToDecimal(Dr["Qty"]).ToString("N4"), Convert.ToDecimal(Dr["RemainingQty"]).ToString("N4"), Dr["SeqNoGroup"]);
                else if (TransType == "AMOUNT")
                    this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemName"], Dr["Unit"], Dr["AgreementID"], Dr["SeqNo"], Dr["Deskripsi"], Convert.ToDecimal(Dr["Amount"]).ToString("N2"), Convert.ToDecimal(Dr["RemainingAmount"]).ToString("N2"), Dr["SeqNoGroup"]);
                // if (Dr["Base"].ToString() == "Y")
                // {
                dgvItem.Rows[i].Cells["Check"] = chk;
                dgvItem.Rows[i].Cells["Check"].Value = false;
                // }

                i++;
            }

            dgvItem.ReadOnly = false;
            dgvItem.Columns["Check"].ReadOnly = false;
            dgvItem.Columns["FullItemID"].ReadOnly = true;
            dgvItem.Columns["Item Name"].ReadOnly = true;
            dgvItem.Columns["Unit"].ReadOnly = true;
            dgvItem.Columns["Purchase Agreement ID"].ReadOnly = true;
            dgvItem.Columns["PA Sequence Number"].ReadOnly = true;
            dgvItem.Columns["Deskripsi"].ReadOnly = true;
            if (TransType == "QTY")
            {
                dgvItem.Columns["Qty"].ReadOnly = true;
                dgvItem.Columns["Remaining Qty"].ReadOnly = true;
            }
            else if(TransType == "AMOUNT")
            {
                dgvItem.Columns["Amount"].ReadOnly = true;
                dgvItem.Columns["Remaining Amount"].ReadOnly = true;
            }
            dgvItem.Columns["SeqNoGroup"].Visible = false;
            dgvItem.AutoResizeColumns();

            Conn.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvItem.RowCount - 1; i++)
            {

                if (dgvItem.Rows[i].Cells["Base"].Value.ToString() == "Y")
                {
                    dgvItem.Rows[i].Cells["Check"].Value = Check;
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> SeqNoGroup = new List<string>();
            int CountChk = 0;
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
                    SeqNoGroup.Add(dgvItem.Rows[i].Cells["SeqNoGroup"].Value == null ? "" : dgvItem.Rows[i].Cells["SeqNoGroup"].Value.ToString());
                }
            }
            Parent.AddDataGridDetail(AgreementId, SeqNoGroup);

            this.Close();
        }

        private void dgvItem_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvItem.Columns[e.RowIndex].Name == "Qty" || dgvItem.Columns[e.RowIndex].Name == "Remaining Qty")
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                else
                {
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N2");
                }
            }
            if (dgvItem.Columns[e.RowIndex].Name == "Amount" || dgvItem.Columns[e.RowIndex].Name == "Remaining Amount")
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.0000";
                    return;
                }
                else
                {
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N4");
                }
            }
        }
    }
}
