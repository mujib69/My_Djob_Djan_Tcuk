using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.SalesOrder
{
    public partial class SalesOrder : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;

        private string Query;
        private string SalesOrderNo;

        PopUp.MoUCustomerId.MoUId MOUID = null;
        PopUp.CustomerID.Customer Cust = null;
        PopUp.FullItemId.FullItemId FID = null;

        string[] tableCols = new string[] { "No", "SalesOrderNo", "SeqNo", "GroupID", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "Base", "Qty", "RemainingQty", "Unit", "Price", "Qty_Alt", "Unit_Alt", "Price_Alt", "ConvertionRatio", "DeliveryMethod", "ExpectedDateFrom", "ExpectedDateTo", "SubTotal", "SubTotal_PPN", "SubTotal_PPH", "LogisticAmount", "LogisticNotes", "DiscType", "DiscPercent", "DiscAmount", "BonusAmount", "CashBackAmount", "Notes", "SA_SQ_Id", "SA_SQ_SeqNo", "RefTransId", "RefTrans_SeqNo", "PLJNo", "PLJSeqNo", "PLJPrice" };
        List<byte[]> test = new List<byte[]>();

        public SalesOrder()
        {
            InitializeComponent();
        }

        private void SalesOrder_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            dtDocDate.Enabled = false;
            tbxNotes.Enabled = false;
            dtDP.Enabled = false;
            dtValidTo.Enabled = false;
        }

        public void GetData(string SalesOrderNo)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select * from [ISBS-NEW4].[dbo].[SalesOrderH] where SalesOrderNo = '" + SalesOrderNo + "'; ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                //header
                lblSONumber.Text = Dr["SalesOrderNo"].ToString();
                if (Dr["SA_SQ_Id"].ToString().Split('/')[0] == "SA")
                {
                    lblRefernce.Text = "Sales Agreement";
                    label26.Text = "Sales Agreement ID  :";
                }
                else if (Dr["SA_SQ_Id"].ToString().Split('/')[0] == "SQ")
                {
                    lblRefernce.Text = "Sales Quotation";
                    label26.Text = "Sales Quotation ID  :";
                }
                else
                {
                    lblRefernce.Text = "";
                }
                lblRefNo.Text = Dr["SA_SQ_Id"].ToString();
                lblMoUNo.Text = Dr["SalesMouNo"].ToString();
                dtDocDate.Value = Convert.ToDateTime(Dr["OrderDate"]);
                lblRefNo2.Text = Dr["RefTransId"].ToString();
                lblCustId.Text = Dr["CustID"].ToString();
                lblCustName.Text = Dr["CustName"].ToString();
                //tab sales
                lblCurrency.Text = Dr["CurrencyID"].ToString();
                lblExchRate.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["ExchRate"].ToString()));
                lblTermOfPayment.Text = Dr["TermofPayment"].ToString();
                Cmd = new SqlCommand("Select [PaymentModeName] from [ISBS-NEW4].[dbo].[PaymentMode] where [PaymentModeID] = '" + Dr["PaymentModeID"].ToString() + "'", Conn);
                lblPaymentMode.Text = Cmd.ExecuteScalar().ToString();
                lblDPRequired.Text = Dr["DPType"].ToString();
                lblDPAmount.Text = string.Format("{0:#,0.00}", double.Parse(Dr["DPPercent"].ToString()));
                lblDPAmount2.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["DPAmount"].ToString()));
                if (Dr["DPType"].ToString() == "Y")
                    dtDP.Value = Convert.ToDateTime(Dr["DPDueDate"]);
                lblPPN.Text = Dr["PPN"].ToString();
                lblPPH.Text = Dr["PPH"].ToString();
                dtValidTo.Value = Convert.ToDateTime(Dr["ValidTo"]);
                //bawah
                tbxNotes.Text = Dr["Notes"].ToString();
                lblTotal.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["Total"].ToString()));
                lblTotalPPN.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["Total_PPN"].ToString()));
                lblTotalDiscount.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["Total_Disk"].ToString()));
                lblGrandTotal.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["Total_Nett"].ToString()));
                lblTotalPPh.Text= string.Format("{0:#,0.0000}", double.Parse(Dr["Total_PPH"].ToString()));  
                //tab logistic
                //lblLogistic.Text = Dr["Total_LogisticAmount"].ToString();
                if (lblRefernce.Text == "Sales Quotation")
                {
                    lblLogistic.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["Total_LogisticAmount"].ToString()));
                }
                else
                {
                    lblLogistic.Text = "0.0000";
                }
                lblTotalBonus.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["Total_Bonus"].ToString()));
                lblTotalCashBack.Text = string.Format("{0:#,0.0000}", double.Parse(Dr["Total_Cashback"].ToString()));
                    
            }
            Dr.Close();
            //dgv Detail Sales
            dgvSO.DataSource = null;
            if (dgvSO.RowCount - 1 <= 0)
            {
                dgvSO.ColumnCount = tableCols.Length;
                for (int i = 0; i < tableCols.Length; i++)
                {
                    dgvSO.Columns[i].Name = tableCols[i];
                }
            }
          
            Query = "select * from [ISBS-NEW4].[dbo].[SalesOrderD] where SalesOrderNo = '" + SalesOrderNo + "' and Deleted = 'N' order by SeqNo asc; ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int no = 1;
            while (Dr.Read())
            {
                dgvSO.Rows.Add(1);
                dgvSO.Rows[no - 1].Cells[0].Value = no;
                for (int i = 1; i < tableCols.Length; i++)
                {
                    if (tableCols[i] == "DiscType")
                    {
                        //if (Mode == "Edit")
                        //{
                        //    cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                        //    Query = "Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr[tableCols[i]] + "'";
                        //    Cmd = new SqlCommand(Query, Conn);
                        //    if (Dr[tableCols[i]] != null)
                        //        cell.Value = Cmd.ExecuteScalar().ToString();
                        //    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableCols[i]] = cell;
                        //}
                        //else
                        //{
                            Query = "Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr["DiscType"] + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            dgvSO.Rows[(dgvSO.Rows.Count - 1)].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                        //}
                    }
                    else if (tableCols[i] == "ExpectedDateFrom" || tableCols[i] == "ExpectedDateTo")
                    {
                        if (Dr[tableCols[i]] != (object)DBNull.Value)
                            dgvSO.Rows[no - 1].Cells[tableCols[i]].Value = Convert.ToDateTime(Dr[tableCols[i]]);
                    }
                    else
                        dgvSO.Rows[no - 1].Cells[i].Value = Dr[tableCols[i]];
                }
                no++;
            }
            Dr.Close();
            dgvSO.AutoResizeColumns();
            dgvSO.ReadOnly = true;

            //dgv Detail Reference
            dgvDetailRef.DataSource = null;
            Query = "select SA_SQ_Id from SalesOrderH where SalesOrderNo = '" + SalesOrderNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            string SA_SQ_Id = "";
            //
            if (!(Cmd.ExecuteScalar() == null || Cmd.ExecuteScalar().ToString() == String.Empty))
            {
                dgvDetailRef.ColumnCount = Convert.ToInt32(tableCols.Length - 3);
                for (int i = 0; i < Convert.ToInt32(tableCols.Length - 3); i++)
                    dgvDetailRef.Columns[i].Name = tableCols[i];


                SA_SQ_Id = Cmd.ExecuteScalar().ToString();
                if (lblRefernce.Text == "Sales Agreement")
                {
                    Cmd = new SqlCommand("select TransType from SalesAgreementH where salesAgreementNo = '" + lblRefNo.Text + "'", Conn);
                    if (Cmd.ExecuteScalar().ToString() == "AMOUNT")
                        dgvDetailRef.Columns["RemainingQty"].HeaderText = "Remaining Amount";
                    else
                        dgvDetailRef.Columns["RemainingQty"].HeaderText = "Remaining Qty";
                }

                if (SA_SQ_Id.Split('/')[0] == "SQ")
                    Query = "select * from [ISBS-NEW4].[dbo].[SalesQuotationD] where SalesQuotationNo = '" + SA_SQ_Id + "' and Deleted = 'N' order by SeqNo asc";
                else if (SA_SQ_Id.Split('/')[0] == "SA")
                    Query = "select * from [SalesAgreement_Dtl] where SalesAgreementNo = '" + SA_SQ_Id + "' and Deleted = 'N' order by SeqNo asc";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int x = 0;
                while (Dr.Read())
                {
                    #region pass value to dgvDetailRef
                    dgvDetailRef.Rows.Add(1);
                    for (int i = 0; i < Convert.ToInt32(tableCols.Length - 3); i++)
                    {
                        if (!(tableCols[i] == "RefTransId" || tableCols[i] == "RefTrans_SeqNo" || tableCols[i] == "SalesOrderNo"))
                        {
                            if (i == 0)
                                dgvDetailRef.Rows[x].Cells[tableCols[i]].Value = dgvDetailRef.Rows.Count;
                            else if (tableCols[i] == "DiscType")
                            {
                                Cmd = new SqlCommand("select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme] where [DiskonSchemeID] = '" + Dr[tableCols[i]].ToString() + "'", Conn);
                                dgvDetailRef.Rows[x].Cells[tableCols[i]].Value = Cmd.ExecuteScalar().ToString();
                            }
                            else if (tableCols[i] == "SA_SQ_Id")
                            {
                                if (lblRefernce.Text == "Sales Quotataion")
                                    dgvDetailRef.Rows[x].Cells[tableCols[i]].Value = Dr["SalesQuotationNo"];
                                else if (lblRefernce.Text == "Sales Agreement")
                                    dgvDetailRef.Rows[x].Cells[tableCols[i]].Value = Dr["SalesAgreementNo"];
                            }
                            else if (tableCols[i] == "SA_SQ_SeqNo")
                                dgvDetailRef.Rows[x].Cells[tableCols[i]].Value = Dr["SeqNo"];
                            else if (tableCols[i] == "RemainingQty")
                            {
                                if (lblRefernce.Text == "Sales Quotataion")
                                    dgvDetailRef.Rows[x].Cells[tableCols[i]].Value = Dr["Qty"];
                                else if (lblRefernce.Text == "Sales Agreement")
                                    dgvDetailRef.Rows[x].Cells[tableCols[i]].Value = Dr["RemainingQty"];
                            }
                            else
                                dgvDetailRef.Rows[x].Cells[tableCols[i]].Value = Dr[tableCols[i]];
                        }
                    }
                    x++;
                    #endregion
                }
                Dr.Close();
                dgvDetailRef.ReadOnly = true;
                dgvDetailRef.AutoResizeColumns();

                dgvAttachment.Rows.Clear();
                if (dgvAttachment.RowCount - 1 <= 0)
                {
                    dgvAttachment.ColumnCount = 5;
                    dgvAttachment.Columns[0].Name = "FileName";
                    dgvAttachment.Columns[1].Name = "ContentType";
                    dgvAttachment.Columns[2].Name = "File Size (kb)";
                    dgvAttachment.Columns[3].Name = "Attachment";
                    dgvAttachment.Columns[4].Name = "Id";

                    dgvAttachment.Columns["Attachment"].Visible = false;
                    dgvAttachment.Columns["Id"].Visible = false;
                }

                Query = "Select * From [tblAttachments] Where ReffTableName = 'SalesOrderH' And ReffTransId = '" + SalesOrderNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                    test.Add((byte[])Dr["Attachment"]);
                }

                Dr.Close();
                dgvAttachment.ReadOnly = true;
                dgvAttachment.AutoResizeColumns();
            }


        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvSO_DoubleClick(object sender, EventArgs e)
        {

        }
        private bool CheckOpened(string name)
        {
            FormCollection FC = Application.OpenForms;
            foreach (Form frm in FC)
            {
                if (frm.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void lblMoUNo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (MOUID == null || MOUID.Text == "")
                {
                    lblMoUNo.Enabled = true;
                    MOUID = new PopUp.MoUCustomerId.MoUId();
                    MOUID.GetData(lblMoUNo.Text);
                    MOUID.Show();
                }
                else if (CheckOpened(MOUID.Name))
                {
                    MOUID.WindowState = FormWindowState.Normal;
                    MOUID.Show();
                    MOUID.Focus();
                }
            }
        }

        private void lblCustId_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    lblCustId.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(lblCustId.Text);
                    Cust.Show();
                }
                else if (CheckOpened(MOUID.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        private void lblCustName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    lblCustName.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(lblCustId.Text);
                    Cust.Show();
                }
                else if (CheckOpened(MOUID.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        private void dgvSO_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvSO.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvSO.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvSO.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvSO.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }
    }
}
