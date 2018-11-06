using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;
using System.Text.RegularExpressions;

namespace ISBS_New.Sales.SalesAgreement
{
    public partial class SAReserved : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
        private SqlConnection Conn2;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        private TransactionScope scope;
        /**********SQL*********/

        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "No", "RefTransType", "RefTransId", "RefTrans_SeqNo", "RefTrans2Id", "RefTrans2_SeqNo", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemId", "ItemName", "InventSiteId", "Available_UoM", "Available_For_Sale_UoM", "Available_For_Sale_Reserved_UoM", "Lock_Qty", "Unit", "Lock_Qty_Alt", "Unit_Alt", "Ratio", "GelombangId", "BracketId", "Base", "GelombangSeqNo_Base" };
        /*********datagridview cols name*********/

        /**********SET MODE*********/
        //private string Mode;
        //private string SAID;
        /**********SET MODE*********/

        /*********PARENT*********/
        //GlobalInquiry Parent = new GlobalInquiry();
        //public void SetParent(GlobalInquiry F) { Parent = F; }
        /*********PARENT*********/

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        bool check; //label
        private string msg; //Validation
        /*********VALIDATION*********/

        //DataGridViewComboBoxCell cell; //CELLVALUE
        //private SqlDataReader Dr2; //CELLVALUE

        //DateTimePicker dtp;

        //PASS VALUE FROM SO HEADER FORM
        public List<string> SAID = new List<string>();
        public List<int> SA_SeqNo = new List<int>();
        public List<decimal> SA_Qty = new List<decimal>();

        public SAReserved()
        {
            InitializeComponent();
        }

        private void SAReserved_Load(object sender, EventArgs e)
        {
            GetDataHeader();
            gv1Format();
        }

        private void GetDataHeader()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.ColumnCount = tableCols.Length;
            for (int i = 0; i < tableCols.Length; i++)
                dataGridView1.Columns[i].Name = tableCols[i];

            Conn = ConnectionString.GetConnection();
            Query = "select d.SalesAgreementNo, d.SeqNo, d.SA_SQ_Id, d.SA_SQ_SeqNo, c.GroupID, c.SubGroup1ID, c.SubGroup2ID, c.ItemID, a.FullItemId, a.ItemName, a.InventSiteId, a.Available_UoM, a.Available_For_Sale_UoM, a.Available_For_Sale_Reserved_UoM, b.Ratio, c.UoM, c.UoMAlt, d.GelombangId, d.BracketId, d.Base, d.GelombangSeqNo_Base from Invent_OnHand_Qty a left join InventConversion b on a.FullItemId = b.FullItemID left join InventTable c on c.FullItemID = a.FullItemID left join SalesAgreement_Dtl d on a.FullItemId = d.FullItemID where a.FullItemID in (select FullItemID from SalesOrderD where SalesAgreementNo = '" + SAID[0] + "' and Deleted = 'N') and d.SalesAgreementNo = '" + SAID[0] + "' and d.Deleted = 'N'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                #region pass item to datagridview1
                dataGridView1.Rows.Add(1);
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["No"].Value = dataGridView1.RowCount;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["RefTransType"].Value = "SALES AGREEMENT";
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["RefTransId"].Value = Dr["SalesAgreementNo"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["RefTrans_SeqNo"].Value = Dr["SeqNo"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["RefTrans2Id"].Value = Dr["SA_SQ_Id"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["RefTrans2_SeqNo"].Value = Dr["SA_SQ_SeqNo"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["GroupID"].Value = Dr["GroupID"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SubGroup1ID"].Value = Dr["GroupID"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["SubGroup2ID"].Value = Dr["SubGroup2ID"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ItemID"].Value = Dr["ItemID"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["FullItemId"].Value = Dr["FullItemId"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["ItemName"].Value = Dr["ItemName"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["InventSiteId"].Value = Dr["InventSiteId"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Available_UoM"].Value = Dr["Available_UoM"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Available_For_Sale_UoM"].Value = Dr["Available_For_Sale_UoM"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Available_For_Sale_Reserved_UoM"].Value = Dr["Available_For_Sale_Reserved_UoM"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Unit"].Value = Dr["UoM"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Unit_Alt"].Value = Dr["UoMAlt"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Ratio"].Value = Dr["Ratio"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["GelombangId"].Value = Dr["GelombangId"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["BracketId"].Value = Dr["BracketId"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Base"].Value = Dr["Base"];
                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["GelombangSeqNo_Base"].Value = Dr["GelombangSeqNo_Base"];

                //GET LOCK QTY AND LOCK QTY ALT FOR THIS SO
                int RefTrans2_SeqNo = Dr["SA_SQ_SeqNo"] == (object)DBNull.Value ? 0 : Convert.ToInt32(Dr["SA_SQ_SeqNo"]);
                Query = "select Lock_Qty, Lock_Qty_Alt from InventLockTable where RefTransId = '" + Dr["SalesAgreementNo"] + "' and RefTrans_SeqNo = '" + Dr["SeqNo"] + "' and RefTrans2Id = '" + Dr["SA_SQ_Id"] + "' and RefTrans2_SeqNo = '" + RefTrans2_SeqNo + "' and FullItemId = '" + Dr["FullItemId"] + "' and SiteId = '" + Dr["InventSiteId"] + "'";
                Cmd = new SqlCommand(Query, Conn);
                SqlDataReader Dr2 = Cmd.ExecuteReader();
                if (Dr2.HasRows)
                {
                    while (Dr2.Read())
                    {
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Lock_Qty"].Value = Dr2["Lock_Qty"].ToString();
                        dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Lock_Qty_Alt"].Value = Dr2["Lock_Qty_Alt"].ToString();
                    }
                }
                else
                {
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Lock_Qty"].Value = "0";
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Lock_Qty_Alt"].Value = "0";
                }
                Dr2.Close();
                #endregion
            }
            Dr.Close();
            Conn.Close();
        }

        private void gv1Format()
        {
            //dataGridView1.Columns["RefTrans_SeqNo"].HeaderText = "SA Item No";
            dataGridView1.Columns["InventSiteId"].HeaderText = "Site";
            dataGridView1.Columns["Available_For_Sale_UoM"].HeaderText = "For Sale UoM";
            dataGridView1.Columns["Available_For_Sale_Reserved_UoM"].HeaderText = "Reserved UoM";

            int prev_SeqNo = 0;
            Color color = Color.LightGray;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (dataGridView1.Columns[j].Name != "Lock_Qty")
                    {
                        dataGridView1.Rows[i].Cells[j].ReadOnly = true;
                        if (i == 0)
                        {
                            prev_SeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value);
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = color;
                        }
                        else
                        {
                            if (prev_SeqNo == Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value))
                            {
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = color;
                            }
                            else
                            {
                                prev_SeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value);
                                if (dataGridView1.Rows[i - 1].Cells[j].Style.BackColor == Color.LightSteelBlue)
                                {
                                    color = Color.LightGray;
                                    dataGridView1.Rows[i].Cells[j].Style.BackColor = color;
                                }
                                else
                                {
                                    color = Color.LightSteelBlue;
                                    dataGridView1.Rows[i].Cells[j].Style.BackColor = color;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SAReserved_Leave(object sender, EventArgs e)
        {
            SAID.Clear();
            SA_SeqNo.Clear();
            SA_Qty.Clear();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView1.Columns["RefTrans_SeqNo"].Visible = false;
            dataGridView1.Columns["RefTransType"].Visible = false;
            dataGridView1.Columns["RefTransId"].Visible = false;
            dataGridView1.Columns["RefTrans_SeqNo"].Visible = false;
            dataGridView1.Columns["RefTrans2Id"].Visible = false;
            dataGridView1.Columns["RefTrans2_SeqNo"].Visible = false;
            dataGridView1.Columns["GroupID"].Visible = false;
            dataGridView1.Columns["SubGroup1ID"].Visible = false;
            dataGridView1.Columns["SubGroup2ID"].Visible = false;
            dataGridView1.Columns["ItemID"].Visible = false;
            dataGridView1.Columns["Lock_Qty_Alt"].Visible = false;
            dataGridView1.Columns["Unit_Alt"].Visible = false;
            dataGridView1.Columns["Ratio"].Visible = false;
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Available_UoM") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Available_For_Sale_UoM") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Available_For_Sale_Reserved_UoM") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Lock_Qty") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Lock_Qty_Alt") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Ratio"))
            {
                if (e.Value == "" || e.Value == null || e.Value == (object)DBNull.Value)
                    e.Value = "0";
                else if (e.Value.ToString().Contains('-') == true)
                {
                    string numString = "-";
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = numString + Regex.Replace(e.Value.ToString(), "-", "");
                    e.Value = numString + Regex.Replace(e.Value.ToString(), "-", "");
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (tableCols[e.ColumnIndex] == "Lock_Qty")
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value == null || dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value == (object)DBNull.Value)
                    dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value = "0";

                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == String.Empty || dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null || dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == (object)DBNull.Value)
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "0";

                dataGridView1.Rows[e.RowIndex].Cells["Lock_Qty_Alt"].Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) * Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Amount") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Price") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Total") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Disc"))
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allowed numeric and one dot  ex. 10.23
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '-' && (sender as TextBox).Text.IndexOf('-') > -1)
            {
                e.Handled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private char Validation()
        {
            flag = '\0'; msg = null;
            decimal populateQty = 0;
            string id = "";
            int seqNo = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                populateQty = 0;
                if (dataGridView1.Rows[i].Cells["Lock_Qty"].Value == null || dataGridView1.Rows[i].Cells["Lock_Qty"].Value == String.Empty || dataGridView1.Rows[i].Cells["Lock_Qty"].Value == (object)DBNull.Value)
                    dataGridView1.Rows[i].Cells["Lock_Qty"].Value = 0;

                if (!(dataGridView1.Rows[i].Cells["RefTransId"].Value.ToString() == id && Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value) == seqNo))
                {
                    id = dataGridView1.Rows[i].Cells["RefTransId"].Value.ToString();
                    seqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value);

                    Conn = ConnectionString.GetConnection();
                    Query = "select RemainingQty from SalesAgreement_Dtl where GelombangId = (select GelombangId from SalesAgreement_Dtl where SalesAgreementNo = '" + id + "' and SeqNo = '" + seqNo + "') and BracketId = (select BracketId from SalesAgreement_Dtl where SalesAgreementNo = 'SA/1804/00038' and SeqNo = '2') and GelombangSeqNo_Base = 1 and SalesAgreementNo = '" + id + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    decimal qty = Convert.ToDecimal(Cmd.ExecuteScalar());
                    Conn.Close();

                    for (int j = 0; j < dataGridView1.RowCount; j++)
                    {
                        if (dataGridView1.Rows[j].Cells["Lock_Qty"].Value == null || dataGridView1.Rows[j].Cells["Lock_Qty"].Value == String.Empty || dataGridView1.Rows[j].Cells["Lock_Qty"].Value == (object)DBNull.Value)
                            dataGridView1.Rows[j].Cells["Lock_Qty"].Value = 0;
                        if (dataGridView1.Rows[i].Cells["GelombangId"].Value.ToString() == dataGridView1.Rows[j].Cells["GelombangId"].Value.ToString() && dataGridView1.Rows[i].Cells["BracketId"].Value.ToString() == dataGridView1.Rows[j].Cells["BracketId"].Value.ToString())
                        {
                            populateQty += Convert.ToDecimal(dataGridView1.Rows[j].Cells["Lock_Qty"].Value);
                        }
                    }
                    if (populateQty > qty)
                        msg += "Total Quantity of SA Item No " + dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value.ToString() + " (" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + " " + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + ") cannot more than " + qty + "!\r\n";
                }
            }

            if (!(String.IsNullOrEmpty(msg)))
            {
                MetroFramework.MetroMessageBox.Show(this, msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                flag = 'X';
            }
            return flag;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation() != 'X')
            {
                try
                {
                    using (scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            #region variable
                            string RefTransType = dataGridView1.Rows[i].Cells["RefTransType"].Value.ToString();
                            string RefTransId = dataGridView1.Rows[i].Cells["RefTransId"].Value.ToString();
                            int RefTrans_SeqNo = 0;
                            if (dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value != (object)DBNull.Value)
                                RefTrans_SeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans_SeqNo"].Value);
                            string RefTrans2Id = dataGridView1.Rows[i].Cells["RefTrans2Id"].Value.ToString();
                            int RefTrans2_SeqNo = 0;
                            if (dataGridView1.Rows[i].Cells["RefTrans2_SeqNo"].Value != (object)DBNull.Value)
                                RefTrans2_SeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTrans2_SeqNo"].Value);
                            string GroupId = dataGridView1.Rows[i].Cells["GroupId"].Value.ToString();
                            string SubGroup1Id = dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString();
                            string SubGroup2Id = dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString();
                            string ItemId = dataGridView1.Rows[i].Cells["ItemId"].Value.ToString();
                            string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                            string ItemName = dataGridView1.Rows[i].Cells["ItemName"].Value.ToString();
                            string SiteId = dataGridView1.Rows[i].Cells["InventSiteId"].Value.ToString();
                            decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value);
                            decimal Lock_Qty = 0;
                            if (!(dataGridView1.Rows[i].Cells["Lock_Qty"].Value == null || dataGridView1.Rows[i].Cells["Lock_Qty"].Value == String.Empty || dataGridView1.Rows[i].Cells["Lock_Qty"].Value == (object)DBNull.Value))
                                Lock_Qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Lock_Qty"].Value);
                            string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();
                            decimal Lock_Qty_Alt = 0;
                            if (!(dataGridView1.Rows[i].Cells["Lock_Qty_Alt"].Value == null || dataGridView1.Rows[i].Cells["Lock_Qty_Alt"].Value == String.Empty || dataGridView1.Rows[i].Cells["Lock_Qty_Alt"].Value == (object)DBNull.Value))
                                Lock_Qty_Alt = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Lock_Qty_Alt"].Value);
                            string Unit_Alt = dataGridView1.Rows[i].Cells["Unit_Alt"].Value.ToString();
                            #endregion
                            Query = "select d.RefTransId, d.RefTrans_SeqNo, d.Lock_Qty, d.Lock_Qty_Alt, a.FullItemId, a.ItemName, a.InventSiteId, a.Available_UoM, a.Available_For_Sale_UoM, a.Available_For_Sale_Reserved_UoM, b.Ratio, c.UoM, c.UoMAlt from Invent_OnHand_Qty a left join InventConversion b on a.FullItemId = b.FullItemID left join InventTable c on c.FullItemID = a.FullItemID left join InventLockTable d on d.FullItemId = a.FullItemId and d.SiteId = a.InventSiteId where a.FullItemID in (select FullItemID from SalesAgreement_Dtl where SalesAgreementNo = '" + SAID[0] + "') and d.RefTransId = '" + SAID[0] + "' and d.RefTrans_SeqNo = '" + RefTrans_SeqNo + "' and d.SiteId = '" + SiteId + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                Query = "select Price from SalesAgreement_Dtl where SalesAgreementNo = '" + RefTransId + "' and SeqNo = '" + RefTrans_SeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                decimal price = Convert.ToDecimal(Cmd.ExecuteScalar());

                                Query = "select Lock_Qty from InventLockTable where RefTransId = '" + RefTransId + "' and RefTrans_SeqNo = '" + RefTrans_SeqNo + "' and SiteId = '" + SiteId + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                decimal OldLock_Qty = (decimal)Cmd.ExecuteScalar();
                                updateInventOnHandQty(Lock_Qty, OldLock_Qty, FullItemId, SiteId, Ratio, price);
                                updateInventLockTable(RefTransId, RefTrans_SeqNo, RefTrans2Id, RefTrans2_SeqNo, SiteId, Lock_Qty, Lock_Qty_Alt);
                                updateInventTrans(RefTransId, RefTrans_SeqNo, Lock_Qty * -1, Lock_Qty * -1 * Ratio, 0, Lock_Qty, Lock_Qty * Ratio, 0);
                            }
                            else if (Lock_Qty != 0)
                            {
                                Query = "select Price from SalesOrderD where salesOrderNo = '" + RefTransId + "' and SeqNo = '" + RefTrans_SeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                decimal price = Convert.ToDecimal(Cmd.ExecuteScalar());

                                decimal OldLock_Qty = 0;
                                updateInventOnHandQty(Lock_Qty, OldLock_Qty, FullItemId, SiteId, Ratio, price);
                                insertInventLockTable(RefTransType, RefTransId, RefTrans_SeqNo, RefTrans2Id, RefTrans2_SeqNo, FullItemId, SiteId, Ratio, Lock_Qty, Unit, Lock_Qty_Alt, Unit_Alt);

                                insertInventTrans(GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, SiteId, RefTransId, RefTrans_SeqNo, Lock_Qty * -1, Lock_Qty * -1 * Ratio, 0, Lock_Qty, Lock_Qty * Ratio, 0);
                            }
                        }
                        Conn.Close();
                        scope.Complete();
                    }
                    MetroFramework.MetroMessageBox.Show(this, "Save Success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MetroFramework.MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally { }
            }
        }

        private void updateInventOnHandQty(decimal Lock_Qty, decimal OldLock_Qty, string FullItemId, string InventSiteId, decimal Ratio, decimal Price)
        {
            decimal qtyGap = Math.Abs(Lock_Qty - OldLock_Qty);

            decimal Available_For_Sale_UoM = 0;
            decimal Available_For_Sale_Reserved_UoM = 0;
            decimal Available_For_Sale_Alt = 0;
            decimal Available_For_Sale_Reserved_Alt = 0;
            decimal Available_For_Sale_Amount = 0;
            decimal Available_For_Sale_Reserved_Amount = 0;
            Query = "select Available_For_Sale_UoM, Available_For_Sale_Reserved_UoM, Available_For_Sale_Alt, Available_For_Sale_Reserved_Alt,Available_For_Sale_Amount,Available_For_Sale_Reserved_Amount from Invent_OnHand_Qty where FullItemId = '" + FullItemId + "' and InventSiteId = '" + InventSiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            while (Dr2.Read())
            {
                Available_For_Sale_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_UoM"]);
                Available_For_Sale_Reserved_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_UoM"]);
                Available_For_Sale_Amount = Convert.ToDecimal(Dr2["Available_For_Sale_Amount"]);
                Available_For_Sale_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Alt"]);
                Available_For_Sale_Reserved_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Alt"]);
                Available_For_Sale_Reserved_Amount = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Amount"]);
            }
            Dr2.Close();

            if (Lock_Qty > OldLock_Qty)
            {
                Available_For_Sale_UoM -= qtyGap;
                Available_For_Sale_Alt -= qtyGap * Ratio;
                Available_For_Sale_Amount -= qtyGap * Price;
                Available_For_Sale_Reserved_UoM += qtyGap;
                Available_For_Sale_Reserved_Alt += qtyGap * Ratio;
                Available_For_Sale_Reserved_Amount += qtyGap * Price;
            }
            else if (Lock_Qty < OldLock_Qty)
            {
                Available_For_Sale_UoM += qtyGap;
                Available_For_Sale_Alt += qtyGap * Ratio;
                Available_For_Sale_Amount += qtyGap * Price;
                Available_For_Sale_Reserved_UoM -= qtyGap;
                Available_For_Sale_Reserved_Alt -= qtyGap * Ratio;
                Available_For_Sale_Reserved_Amount -= qtyGap * Price;
            }

            Query = "update Invent_OnHand_Qty set Available_For_Sale_UoM = '" + Available_For_Sale_UoM + "'";
            Query += ", Available_For_Sale_Reserved_UoM = '" + Available_For_Sale_Reserved_UoM + "'";
            Query += ", Available_For_Sale_Alt = '" + Available_For_Sale_Alt + "'";
            Query += ", Available_For_Sale_Reserved_Alt = '" + Available_For_Sale_Reserved_Alt + "'";
            Query += ", Available_For_Sale_Amount = '" + Available_For_Sale_Amount + "'";
            Query += ", Available_For_Sale_Reserved_Amount = '" + Available_For_Sale_Reserved_Amount + "'";
            Query += " where FullItemId = '" + FullItemId + "' and InventSiteId = '" + InventSiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateInventLockTable(string RefTransId, int RefTrans_SeqNo, string RefTrans2Id, int RefTrans2_SeqNo, string SiteId, decimal Lock_Qty, decimal Lock_Qty_Alt)
        {
            Query = "UPDATE [dbo].[InventLockTable] ";
            Query += "SET [Lock_Qty] = '" + Lock_Qty + "'";
            Query += ",[Lock_Qty_Alt] = '" + Lock_Qty_Alt + "'";
            Query += ",[UpdatedDate] = getdate()";
            Query += ",[UpdatedBy] = '" + ControlMgr.UserId + "' ";
            Query += "WHERE [RefTransId] = '" + RefTransId + "' and [RefTrans_SeqNo] = '" + RefTrans_SeqNo + "' and RefTrans2Id = '" + RefTrans2Id + "' and RefTrans2_SeqNo = '" + RefTrans2_SeqNo + "' and SiteId = '" + SiteId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void updateInventTrans(string TransId, int SeqNo, decimal Available_For_Sale_UoM, decimal Available_For_Sale_Alt, decimal Available_For_Sale_Amount, decimal Available_For_Sale_Reserved_UoM, decimal Available_For_Sale_Reserved_Alt, decimal Available_For_Sale_Reserved_Amount)
        {
            Query = "update InventTrans set Available_For_Sale_UoM = '" + Available_For_Sale_UoM + "'";
            Query += ", Available_For_Sale_Alt = '" + Available_For_Sale_Alt + "'";
            Query += ", Available_For_Sale_Amount = '" + Available_For_Sale_Amount + "'";
            Query += ", Available_For_Sale_Reserved_UoM = '" + Available_For_Sale_Reserved_UoM + "'";
            Query += ", Available_For_Sale_Reserved_Alt = '" + Available_For_Sale_Reserved_Alt + "'";
            Query += ", Available_For_Sale_Reserved_Amount = '" + Available_For_Sale_Reserved_Amount + "'";
            Query += " where TransId = '" + TransId + "' and SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void insertInventLockTable(string RefTransType, string RefTransId, int RefTrans_SeqNo, string RefTrans2Id, int RefTrans2_SeqNo, string FullItemId, string SiteId, decimal Ratio, decimal Lock_Qty, string Unit, decimal Lock_Qty_Alt, string Unit_Alt)
        {
            Query = "INSERT INTO [dbo].[InventLockTable] ([RefTransType],[RefTransId],[RefTrans_SeqNo], RefTrans2Id, RefTrans2_SeqNo,[FullItemId],[SiteId],[Ratio],[Lock_Qty],[Unit],[Lock_Qty_Alt],[Unit_Alt],[CreatedBy])VALUES (@RefTransType,@RefTransId,@RefTrans_SeqNo, @RefTrans2Id, @RefTrans2_SeqNo,@FullItemId,@SiteId,@Ratio,@Lock_Qty,@Unit,@Lock_Qty_Alt,@Unit_Alt,'" + ControlMgr.UserId + "')";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@RefTransType", RefTransType);
            Cmd.Parameters.AddWithValue("@RefTransId", RefTransId);
            Cmd.Parameters.AddWithValue("@RefTrans_SeqNo", RefTrans_SeqNo);
            Cmd.Parameters.AddWithValue("@RefTrans2Id", RefTrans2Id);
            Cmd.Parameters.AddWithValue("@RefTrans2_SeqNo", RefTrans2_SeqNo);
            Cmd.Parameters.AddWithValue("@FullItemId", FullItemId);
            Cmd.Parameters.AddWithValue("@SiteId", SiteId);
            Cmd.Parameters.AddWithValue("@Ratio", Ratio);
            Cmd.Parameters.AddWithValue("@Lock_Qty", Lock_Qty);
            Cmd.Parameters.AddWithValue("@Unit", Unit);
            Cmd.Parameters.AddWithValue("@Lock_Qty_Alt", Lock_Qty_Alt);
            Cmd.Parameters.AddWithValue("@Unit_Alt", Unit_Alt);
            Cmd.ExecuteNonQuery();
        }

        private void insertInventTrans(string GroupId, string SubGroupId, string SubGroup2Id, string ItemId, string FullItemId, string ItemName, string InventSiteId, string TransId, int SeqNo, decimal Available_For_Sale_UoM, decimal Available_For_Sale_Alt, decimal Available_For_Sale_Amount, decimal Available_For_Sale_Reserved_UoM, decimal Available_For_Sale_Reserved_Alt, decimal Available_For_Sale_Reserved_Amount)
        {
            DateTime TransDate = new DateTime(1753, 1, 1);
            string Ref_TransId = "";
            DateTime Ref_TransDate = new DateTime(1753, 1, 1);
            int Ref_Trans_SeqNo = 0;
            string AccountId = "";
            string AccountName = "";
            Query = "select a.OrderDate, b.SA_SQ_Id, b.SA_SQ_SeqNo, c.OrderDate 'SQDate', a.CustID, a.CustName from SalesAgreementH a left join SalesAgreement_Dtl b on a.SalesAgreementNo = b.SalesAgreementNo left join SalesQuotationH c on c.SalesQuotationNo = a.SalesQuotationNo where b.SA_SQ_Id = '" + TransId + "' and b.SA_SQ_SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            while (Dr2.Read())
            {
                TransDate = Convert.ToDateTime(Dr2["OrderDate"]);
                if (Dr2["SA_SQ_Id"] != (object)DBNull.Value)
                {
                    Ref_TransId = Dr2["SA_SQ_Id"].ToString();
                    Ref_Trans_SeqNo = Convert.ToInt32(Dr2["SA_SQ_SeqNo"]);
                    Ref_TransDate = Convert.ToDateTime(Dr2["SQDate"]);
                }
                AccountId = Dr2["CustID"].ToString();
                AccountName = Dr2["CustName"].ToString();
            }
            Dr2.Close();

            Query = "INSERT INTO [dbo].[InventTrans] ([GroupId], [SubGroupId], [SubGroup2Id], [ItemId], [FullItemId], [ItemName], [InventSiteId], [TransId], [SeqNo], [TransDate], [Ref_TransId], [Ref_TransDate], [Ref_Trans_SeqNo], [AccountId], [AccountName], [Available_UoM], [Available_Alt], [Available_Amount], [Available_For_Sale_UoM], [Available_For_Sale_Alt], [Available_For_Sale_Amount], [Available_For_Sale_Reserved_UoM], [Available_For_Sale_Reserved_Alt], [Available_For_Sale_Reserved_Amount]) VALUES (@GroupId, @SubGroupId, @SubGroup2Id, @ItemId, @FullItemId, @ItemName, @InventSiteId, @TransId, @SeqNo, @TransDate, @Ref_TransId, @Ref_TransDate, @Ref_Trans_SeqNo, @AccountId, @AccountName, @Available_UoM, @Available_Alt, @Available_Amount, @Available_For_Sale_UoM, @Available_For_Sale_Alt, @Available_For_Sale_Amount, @Available_For_Sale_Reserved_UoM, @Available_For_Sale_Reserved_Alt, @Available_For_Sale_Reserved_Amount)";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@GroupId", GroupId);
            Cmd.Parameters.AddWithValue("@SubGroupId", SubGroupId);
            Cmd.Parameters.AddWithValue("@SubGroup2Id", SubGroup2Id);
            Cmd.Parameters.AddWithValue("@ItemId", ItemId);
            Cmd.Parameters.AddWithValue("@FullItemId", FullItemId);
            Cmd.Parameters.AddWithValue("@ItemName", ItemName);
            Cmd.Parameters.AddWithValue("@InventSiteId", InventSiteId);
            Cmd.Parameters.AddWithValue("@TransId", TransId);
            Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
            Cmd.Parameters.AddWithValue("@TransDate", TransDate);
            Cmd.Parameters.AddWithValue("@Ref_TransId", Ref_TransId == String.Empty ? (object)DBNull.Value : Ref_TransId); //OK
            Cmd.Parameters.AddWithValue("@Ref_TransDate", Ref_TransId == String.Empty ? (object)DBNull.Value : Ref_TransDate); //OK
            Cmd.Parameters.AddWithValue("@Ref_Trans_SeqNo", Ref_TransId == String.Empty ? (object)DBNull.Value : Ref_Trans_SeqNo); //OK
            Cmd.Parameters.AddWithValue("@AccountId", AccountId);
            Cmd.Parameters.AddWithValue("@AccountName", AccountName);
            Cmd.Parameters.AddWithValue("@Available_UoM", 0);
            Cmd.Parameters.AddWithValue("@Available_Alt", 0);
            Cmd.Parameters.AddWithValue("@Available_Amount", 0);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", Available_For_Sale_UoM);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", Available_For_Sale_Alt);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", Available_For_Sale_Amount);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_UoM", Available_For_Sale_Reserved_UoM);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Alt", Available_For_Sale_Reserved_Alt);
            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Amount", Available_For_Sale_Reserved_Amount);
            Cmd.ExecuteNonQuery();
        }
    }
}
