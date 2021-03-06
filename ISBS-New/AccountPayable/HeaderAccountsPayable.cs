﻿using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Transactions;

namespace ISBS_New.AccountPayable
{
    public partial class HeaderAccountsPayable : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        DataGridViewComboBoxCell cell;
        DataGridViewCheckBoxCell TmpCheckBox;

        string Mode, Query = null;
        public string invoiceId = "";
        public string checkDAOPAO = "";
        string status = "";
        string LogStatusDesc = "";
        string LogDescription = "";

        int Index;
        double a, b, c;
        double hasil = 0;
        int Limit1, Limit2, Total, Page1, Page2;

        string POId = "";
        int POSeqno = 0;

        DataTable tmptable;
        DataTable passedtable;
        AccountPayable.InquiryAccountsPayable Parent;
        AccountPayable.InquiryAccountPayableApproval Parent2;
        TaskList.AccountPayable.TasklistPurchaseInvoiceAP Parent3;
       
        ContextMenu vendid = new ContextMenu();

        private string TransStatus = String.Empty;

        public void SetParent(AccountPayable.InquiryAccountsPayable F)
        {
            Parent = F;
        }
        public void SetParent2(AccountPayable.InquiryAccountPayableApproval I)
        {
            Parent2 = I;
        }
        public void SetParent3(TaskList.AccountPayable.TasklistPurchaseInvoiceAP H)
        {
            Parent3 = H;
        }

        List<string> sSelectedFile, FileName, Extension; //untuk attachement
        List<byte[]> test = new List<byte[]>();

        public void AddDataGridInvoice(List<string> PONo, List<string> PODate, List<string> POAmount, List<string> PODueDate, List<string> GRNo, List<string> DPAmount, List<string> DPOutstanding, 
                                       List<string> POInvoiced, List<string> POUnInvoiced,
                                       List<string> GRAmount, List<string> PotDP,  List<string> GRPayable)
        {
            if (dgvDetail.RowCount == 0)
            {
                dgvDetail.Rows.Clear();
                dgvDetail.ColumnCount = 14;

                dgvDetail.Columns[0].Name = "No";
                dgvDetail.Columns[1].Name = "PONo";
                dgvDetail.Columns[2].Name = "PODate";
                dgvDetail.Columns[3].Name = "POAmount";

                dgvDetail.Columns[4].Name = "PODueDate";
                dgvDetail.Columns[5].Name = "GRNo";
                dgvDetail.Columns[6].Name = "DPAmount";
                dgvDetail.Columns[7].Name = "DPOutstanding";

                dgvDetail.Columns[8].Name = "POInvoiced";
                dgvDetail.Columns[9].Name = "POUnInvoice";
                dgvDetail.Columns[10].Name = "GRAmount";
                dgvDetail.Columns[11].Name = "PotDP";

                dgvDetail.Columns[12].Name = "GRPayable";
                dgvDetail.Columns[13].Name = "InvoiceAmount";
            }

            for (int i = 0; i < PONo.Count; i++)
            {
                this.dgvDetail.Rows.Add((dgvDetail.RowCount + 1).ToString(), PONo[i], PODate[i], POAmount[i], PODueDate[i], GRNo[i], DPAmount[i], DPOutstanding[i], POInvoiced[i], POUnInvoiced[i], GRAmount[i], PotDP[i], GRPayable[i], "0.00");
            }
            dgvDetail.AutoResizeColumns();
            dgvDetail.Refresh();
        }
   
        public void AddDataGridUangMuka(List<string> PONo, List<string> PODate, List<string> POAmount, List<string> PODueDate, List<string> DPRequired, List<string> DPPercent,
                                       List<string> DPAmount, List<string> DPDeduct, List<string> DPOutstanding,
                                       List<string> POInvoice, List<string> POUnInvoiced)
        {
            if (dgvDetail.RowCount == 0)
            {
                dgvDetail.Rows.Clear();
                dgvDetail.ColumnCount = 14;

                dgvDetail.Columns[0].Name = "No";
                dgvDetail.Columns[1].Name = "PONo";
                dgvDetail.Columns[2].Name = "PODate";
                dgvDetail.Columns[3].Name = "POAmount";

                dgvDetail.Columns[4].Name = "PODueDate";
                dgvDetail.Columns[5].Name = "DPRequired";
                dgvDetail.Columns[6].Name = "DPPercent";
                dgvDetail.Columns[7].Name = "DPAmount";

                dgvDetail.Columns[8].Name = "DPDeduct";
                dgvDetail.Columns[9].Name = "DPOutstanding";
                dgvDetail.Columns[10].Name = "POInvoiced";
                dgvDetail.Columns[11].Name = "POUnInvoice";

                dgvDetail.Columns[12].Name = "PayableAmount";
                dgvDetail.Columns[13].Name = "InvoiceAmount";
            }

            for (int i = 0; i < PONo.Count; i++)
            {
                this.dgvDetail.Rows.Add((dgvDetail.RowCount + 1).ToString(), PONo[i], PODate[i], POAmount[i], PODueDate[i], DPRequired[i], DPPercent[i], DPAmount[i], DPDeduct[i], DPOutstanding[i], POInvoice[i], POUnInvoiced[i], Convert.ToDecimal(POUnInvoiced[i]) - Convert.ToDecimal(POInvoice[i]), "0.00");
            }
            dgvDetail.AutoResizeColumns();
            dgvDetail.Refresh();
        }

        public void AddReturNotaBeli()
        {
            if (cmbInvoiceType.Text != "Uang Muka" &&  cmbInvoiceType.Text != "")
            {
                dgvRetur.ColumnCount = 9;
                dgvRetur.Columns[0].Name = "Check";
                dgvRetur.Columns[1].Name = "No";
                dgvRetur.Columns[2].Name = "NRBNo";
                dgvRetur.Columns[3].Name = "NRBDate";

                dgvRetur.Columns[4].Name = "GRNo";
                dgvRetur.Columns[5].Name = "GRDate";
                dgvRetur.Columns[6].Name = "PONo";
                dgvRetur.Columns[7].Name = "PODate";
                dgvRetur.Columns[8].Name = "TotalRetur";

                if (dgvRetur.RowCount > 0)
                {
                    dgvRetur.Rows.Clear();
                }

                string ListPO = "";
                for (int i = 0; i < dgvDetail.RowCount; i++)
                {
                    ListPO += "'" + dgvDetail.Rows[i].Cells["PONo"].Value.ToString() + "'";
                    if (i != dgvDetail.RowCount - 1)
                    {
                        ListPO += ",";
                    }
                }

                using (Conn = ConnectionString.GetConnection())
                {

                    Query = "SELECT a.NRBId, a.NRBDate, c.GoodsReceivedId, c.GoodsReceivedDate, e.PurchID, e.OrderDate, case when e.Unit=b.Uom_Unit then sum(e.Price*b.Uom_Qty) when e.Unit=b.Alt_Unit then sum(e.Price*b.Alt_Qty) end TotalRetur from [dbo].[NotaReturBeliH] a ";
                    Query += "left join NotaReturBeli_Dtl b on a.NRBId=b.NRBId ";
                    Query += "left join GoodsReceivedD c on b.GoodsReceivedId=c.GoodsReceivedId and c.GoodsReceivedSeqNo=b.GoodsReceived_SeqNo ";
                    Query += "left join ReceiptOrderD d on d.ReceiptOrderId=c.RefTransID and d.SeqNo=c.RefTransSeqNo ";
                    Query += "left join PurchDtl e on e.PurchID=d.PurchaseOrderId and e.SeqNo=d.PurchaseOrderSeqNo ";
                    if(ListPO != "")
                        Query += "where e.Purchid in (" + ListPO + ") and a.NRBId not in (select NRB_No from VendInvoice_Dtl_NotaRetur) group by a.NRBId, a.NRBDate, c.GoodsReceivedId, c.GoodsReceivedDate,e.Unit, b.Uom_Unit,b.Alt_Unit,e.PurchID, e.OrderDate;";
                    else
                        Query += "where a.NRBId not in (select NRB_No from VendInvoice_Dtl_NotaRetur) group by a.NRBId, a.NRBDate, c.GoodsReceivedId, c.GoodsReceivedDate,e.Unit, b.Uom_Unit,b.Alt_Unit,e.PurchID, e.OrderDate;";
                  
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    int i = 1;
                    while (Dr.Read())
                    {
                        TmpCheckBox = new DataGridViewCheckBoxCell();
                        dgvRetur.Rows.Add("", i, Dr["NRBId"], Dr["NRBDate"], Dr["GoodsReceivedId"], Dr["GoodsReceivedDate"], Dr["PurchID"], Dr["OrderDate"], Dr["TotalRetur"]);
                        dgvRetur.Rows[(dgvRetur.Rows.Count - 1)].Cells["Check"] = TmpCheckBox;
                        dgvRetur.Rows[(dgvRetur.Rows.Count - 1)].Cells["Check"].Value = false;
                        i++;
                    }
                    Dr.Close();
                    dgvRetur.AutoResizeColumns();
                }
            }
        }

        private void buttonNewGeneralAccountPayable_Click(object sender, EventArgs e)
        {
            if (txtVendAccount.Text == "")
            {
                MessageBox.Show("Vendor Tidak Boleh Kosong.");
                return;
            }
            if (cmbInvoiceType.Text == "")
            {
                MessageBox.Show("Invoice Type tidak boleh kosong.");
                return;
            }
            else
            {

                SearchQueryV2 tmpSearch = new SearchQueryV2();
                tmpSearch.FormName = "Search PO No";
                tmpSearch.PrimaryKey = "PONo";
                tmpSearch.Table = "[dbo].PO_UnInvoiced_GR_View";
                if (cmbInvoiceType.Text == "Invoice")
                {
                    tmpSearch.QuerySearch = "select PONo, CONVERT(varchar,PODate,103) as PODate, Format(POAmount,'N2') POAmount, CONVERT(varchar,PODueDate,103) as PODueDate, GRNo, DPRequired, Format(DPPercent,'N2') DPPercent, Format(DPAmount,'N2') DPAmount, Format(DPOutstanding,'N2') DPOutstanding, Format(POInvoiced,'N2') POInvoiced, Format(POUnInvoice,'N2') POUnInvoice, Format(GRAmount,'N2') GRAmount, Format(PotDP,'N2') PotDP, Format(GRPayable,'N2') GRPayable from PO_UnInvoiced_GR_View where VendId='" + txtVendAccount.Text + "' and GRPayable is not null and GRPayable >0";
                    tmpSearch.Select = new string[] { "PONo", "PODate", "POAmount", "PODueDate", "GRNo","DPAmount", "DPOutstanding", "POInvoiced", "POUnInvoice", "GRAmount", "PotDP", "GRPayable" };
                }
                else if (cmbInvoiceType.Text == "Uang Muka")
                {
                    tmpSearch.QuerySearch = "select PONo, CONVERT(varchar,PODate,103) as PODate, Format(POAmount,'N2') POAmount, CONVERT(varchar,PODueDate,103) as PODueDate , DPRequired, Format(DPPercent,'N2') DPPercent, Format(DPAmount,'N2') DPAmount, Format(DPDeduct,'N2') DPDeduct, Format(DPOutstanding,'N2') DPOutstanding, Format(POInvoiced,'N2') POInvoiced, Format(POUnInvoice,'N2') POUnInvoice from PO_UnInvoiced_DP_View where VendId='" + txtVendAccount.Text + "' and POUnInvoice is not null and POUnInvoice >0 ";
                    tmpSearch.Select = new string[] { "PONo", "PODate", "POAmount", "PODueDate", "DPRequired", "DPPercent", "DPAmount", "DPDeduct", "DPOutstanding", "POInvoiced", "POUnInvoice" };
                }
                tmpSearch.FilterText = new string[] { "PONo" };
                //tmpSearch.WherePlus = " and PO_No = '" + txtPONumber.Text + "' ";
                tmpSearch.ShowDialog();

                if (Variable.Kode2 != null)
                {
                    if (cmbInvoiceType.Text == "Invoice")
                    {
                        List<string> PONo = new List<string>();
                        List<string> PODate = new List<string>();
                        List<string> POAmount = new List<string>();
                        List<string> PODueDate = new List<string>();
                        List<string> GRNo = new List<string>();
                        List<string> DPRequired = new List<string>();

                        List<string> DPPercent = new List<string>();
                        List<string> DPAmount = new List<string>();
                        List<string> DPOutstanding = new List<string>();
                        List<string> POInvoiced = new List<string>();
                        List<string> POUnInvoiced = new List<string>();

                        List<string> GRAmount = new List<string>();
                        List<string> PotDP = new List<string>();
                        List<string> GRPayable = new List<string>();

                        for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
                        {
                            PONo.Add(Variable.Kode2[i, 0]);
                            PODate.Add(Variable.Kode2[i, 1]);
                            POAmount.Add(Variable.Kode2[i, 2] == "" ? "0.000" : Variable.Kode2[i, 2]);
                            PODueDate.Add(Variable.Kode2[i, 3]);
                            GRNo.Add(Variable.Kode2[i, 4]);
                            DPAmount.Add(Variable.Kode2[i, 5]== "" ? "0.000" : Variable.Kode2[i, 5]);

                            DPOutstanding.Add(Variable.Kode2[i, 6] == "" ? "0.000" : Variable.Kode2[i, 6]);
                            POInvoiced.Add(Variable.Kode2[i, 7] == "" ? "0.000" : Variable.Kode2[i, 7]);
                            POUnInvoiced.Add(Variable.Kode2[i, 8] == "" ? "0.000" : Variable.Kode2[i, 8]);
                            GRAmount.Add(Variable.Kode2[i, 9] == "" ? "0.000" : Variable.Kode2[i, 9]);

                            PotDP.Add(Variable.Kode2[i, 10] == "" ? "0.000" : Variable.Kode2[i, 10]);
                            GRPayable.Add(Variable.Kode2[i, 11] == "" ? "0.000" : Variable.Kode2[i, 11]);
                        }
                        AddDataGridInvoice(PONo, PODate, POAmount, PODueDate, GRNo, DPAmount, DPOutstanding, POInvoiced, POUnInvoiced, GRAmount, PotDP, GRPayable);
                    }
                    else if (cmbInvoiceType.Text == "Uang Muka")
                    {
                        List<string> PONo = new List<string>();
                        List<string> PODate = new List<string>();
                        List<string> POAmount = new List<string>();
                        List<string> PODueDate = new List<string>();
                        List<string> DPRequired = new List<string>();

                        List<string> DPPercent = new List<string>();
                        List<string> DPAmount = new List<string>();
                        List<string> DPDeduct = new List<string>();
                        List<string> DPOutstanding = new List<string>();
                        List<string> POInvoice = new List<string>();

                        List<string> POUnInvoiced = new List<string>();

                        for (int i = 0; i <  Variable.Kode2.GetLength(0); i++)
                        {                            
                            PONo.Add(Variable.Kode2[i, 0]);
                            PODate.Add(Variable.Kode2[i, 1]);
                            POAmount.Add(Variable.Kode2[i, 2] == "" ? "0.000" : Variable.Kode2[i, 2]);
                            PODueDate.Add(Variable.Kode2[i, 3]);
                            DPRequired.Add(Variable.Kode2[i, 4] == "" ? "0.000" : Variable.Kode2[i, 4]);

                            DPPercent.Add(Variable.Kode2[i, 5] == "" ? "0.000" : Variable.Kode2[i, 5]);
                            DPAmount.Add(Variable.Kode2[i, 6] == "" ? "0.000" : Variable.Kode2[i, 6]);
                            DPDeduct.Add(Variable.Kode2[i, 7] == "" ? "0.000" : Variable.Kode2[i, 7]);
                            DPOutstanding.Add(Variable.Kode2[i, 8] == "" ? "0.000" : Variable.Kode2[i, 8]);

                            POInvoice.Add(Variable.Kode2[i, 9] == "" ? "0.000" : Variable.Kode2[i, 9]);
                            POUnInvoiced.Add(Variable.Kode2[i, 10] == "" ? "0.000" : Variable.Kode2[i, 10]);
                        }
                        AddDataGridUangMuka(PONo, PODate, POAmount, PODueDate, DPRequired, DPPercent, DPAmount, DPDeduct, DPOutstanding, POInvoice, POUnInvoiced);
                    }
                    AddReturNotaBeli();
                }
               
                Variable.Kode2 = null;
            }
            dgvAttachmentReadOnlyFalse();
            RefreshDetailItem();
        }

        private int CheckSeqNoGroup()
        {
            for (int j = 1; j <= 1000000; j++)
            {
                for (int i = 0; i < dgvDetail.RowCount; i++)
                {
                    if (Convert.ToInt32(dgvDetail.Rows[i].Cells["No"].Value) == j)
                    {
                        goto Outer;
                    }
                }
                return j;
            Outer:
                continue;
            }
            return 1000000;
        }


        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        public void SetMode(string tmpMode, string tmpInvoiceId)
        {
            Mode = tmpMode;
            invoiceId = tmpInvoiceId;
        }

        public HeaderAccountsPayable()
        {
            InitializeComponent();
        }

        public HeaderAccountsPayable(DataTable TbltoPass)
        {
            InitializeComponent();
            passedtable = TbltoPass;
        }

        public HeaderAccountsPayable(DataTable TbltoPass, string invoiceType, string vendorAccount, string vendorName, string currency, string ExchRate, string vendInvNumber, string invAmount, string invPPN, string invPPH, string invPPNAmount, string invPPHAmount, string amountNett)
        {
            InitializeComponent();
            cmbInvoiceType.Text = invoiceType;
            passedtable = TbltoPass;
            txtVendAccount.Text = vendorAccount;
            txtVendName.Text = vendorName;

            cmbCurrency.Text = currency;
            txtExchRate.Text = ExchRate;
            txtVendInvoiceNumber.Text = vendInvNumber;
            txtInvoiceAmount.Text = invAmount;
            txtInvoicePPN.Text = invPPN;

            txtInvoicePPH.Text = invPPH;
            txtInvoicePPNAmount.Text = invPPNAmount;
            txtInvoicePPHAmount.Text = invPPHAmount;
            txtAmountNett.Text = amountNett;
        }

        private void AddcmbCurrency()
        {
            cmbCurrency.Items.Add("IDR");
        }

        private void AddcmbInvoiceType()
        {
            cmbInvoiceType.Items.Add("Uang Muka");
            cmbInvoiceType.Items.Add("Invoice");
        }

        private void ModeLoad()
        {
            cmbCurrency.Text = "IDR";
            txtExchRate.Text = "1.0000";
            txtInvoicePPHAmount.Text = "0";
            txtInvoicePPNAmount.Text = "0";
            txtInvoiceAmount.Text = "0";

            dtInvoiceDate.Value = DateTime.Today.Date;
            dtVendInvoiceDate.Value = DateTime.Today.Date;
            dtVendInvoiceDueDate.Value = DateTime.Today.Date;
            dtPaymentDueDate.Value = DateTime.Today.Date;
            txtAmountNett.Enabled = false;
            txtInvoiceId.Enabled = false;

            txtVendAccount.Enabled = true;
            txtVendName.Enabled = true;
            txtVendAccount.ReadOnly = true;
            txtVendName.ReadOnly = true;
            txtVendAccount.ContextMenu = vendid;
            txtVendName.ContextMenu = vendid;

            groupBox2.Visible = false;
            btnReject.Visible = false;
            btnRevisi.Visible = false;
            btnApproved.Visible = false;
            btnCancelApprove.Visible = false;
        }

        public void WarnaGrid()
        {
            //for (int i = 0; i < dgvDetail.Rows.Count; i++)
            //{
            //    dgvDetail.Rows[i].Cells[17].Style.BackColor = Color.LightBlue;
            //    dgvDetail.Rows[i].Cells[18].Style.BackColor = Color.LightBlue;
            //    dgvDetail.Rows[i].Cells[19].Style.BackColor = Color.LightBlue;
            //    dgvDetail.Rows[i].Cells[20].Style.BackColor = Color.LightBlue;
            //    dgvDetail.Rows[i].Cells[21].Style.BackColor = Color.LightBlue;
            //    dgvDetail.Rows[i].Cells[22].Style.BackColor = Color.LightBlue;
            //}
        }

        public void InputNilai()
        {
            for (int j = 0; j < dgvDetail.Rows.Count; j++)
            {
                foreach (DataGridViewColumn dc in dgvDetail.Columns)
                {
                    if (cmbInvoiceType.Text == "Invoice")
                    {
                        if (dc.Index.Equals(12))
                        {
                            dc.ReadOnly = false;
                        }
                        else
                        {
                            dc.ReadOnly = true;
                        }
                    }
                    else if (cmbInvoiceType.Text == "Uang Muka")
                    {
                        if (dc.Index.Equals(13))
                        {
                            dc.ReadOnly = false;
                        }
                        else
                        {
                            dc.ReadOnly = true;
                        }
                    }
                }
            }
        }

        public void Hitung()
        {
            
        }

        private void HitungAmountPay()
        {
            double sum = 0;

            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                if (dgvDetail.Rows[i].Cells["InvoiceAmount"].Value != DBNull.Value)
                {
                    sum += Convert.ToDouble(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value);
                }
            }
            txtAmountPay.Text = sum.ToString("N2");
        }

        private void HitungInvoiceAmountNett()
        {
            if (txtInvoicePPN.Text == "")
                txtInvoicePPN.Text = "0.00";
            if (txtInvoicePPH.Text == "")
                txtInvoicePPH.Text = "0.00";

            txtInvoicePPNAmount.Text = (Convert.ToDecimal(txtInvoiceAmount.Text) * Convert.ToDecimal(txtInvoicePPN.Text)/100).ToString("N2");
            txtInvoicePPHAmount.Text = (Convert.ToDecimal(txtInvoiceAmount.Text) * Convert.ToDecimal(txtInvoicePPH.Text)/100).ToString("N2");

            this.txtAmountNett.Text = (Convert.ToDecimal(txtInvoiceAmount.Text) + (Convert.ToDecimal(txtInvoicePPNAmount.Text) + Convert.ToDecimal(txtInvoicePPHAmount.Text))).ToString("N2");

            //if (dgvDetail.RowCount > 0)
            //{
            //    decimal TmpInvoiceAmount = 0;
            //    for (int i = 0; i < dgvDetail.RowCount; i++)
            //    {
            //        TmpInvoiceAmount += Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value);
            //    }

            //    for (int i = 0; i < dgvDetail.RowCount; i++)
            //    {
            //        dgvDetail.Rows[i].Cells["InvoiceAmount"].Value = TmpInvoiceAmount * Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value) / Convert.ToDecimal(txtInvoiceAmount.Text);
            //    }
            //}
        }

        private void AddVendor()
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "VendId";
            tmpSearch.Order = "VendId Asc";
            tmpSearch.Table = "[dbo].[VendTable]";
            tmpSearch.QuerySearch = "SELECT a.VendId, a.VendName, a.PPN,a.PPH, a.NPWP, a.TaxName,a.TaxAddress FROM [dbo].[VendTable] a";
            tmpSearch.FilterText = new string[] { "VendId", "VendName", "PPN", "PPH", "NPWP", "TaxName", "TaxAddress" };
            tmpSearch.Select = new string[] { "VendId", "VendName", "PPN", "PPH", "NPWP", "TaxName", "TaxAddress" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtVendAccount.Text = ConnectionString.Kodes[0];
                txtVendName.Text = ConnectionString.Kodes[1];
                txtInvoicePPN.Text = ConnectionString.Kodes[2];
                txtInvoicePPH.Text = ConnectionString.Kodes[3];
                txtNPWP.Text = ConnectionString.Kodes[4];
                txtTaxName.Text = ConnectionString.Kodes[5];
                richTextBoxTaxAddress.Text = ConnectionString.Kodes[6];
                ConnectionString.Kodes = null;
            }
        }

        private void btnVendAccount_Click(object sender, EventArgs e)
        {
            if (dgvDetail.RowCount == 0)
            {
                AddVendor();
            }
            else
            {
                DialogResult dr = MessageBox.Show("Apakah Anda Ingin Mengganti Vendor?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    dgvDetail.Rows.Clear();
                    txtAmountPay.Clear();
                    AddVendor();
                }
            }

            if (txtInvoicePPN.Text != "" || txtInvoicePPH.Text != "" || txtInvoiceAmount.Text != "")
            {
                HitungInvoiceAmountNett();
            }
            else
            {
                txtAmountNett.Text = txtInvoiceAmount.Text;
            }
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            //belum ditarik dari database
            if (cmbCurrency.Text == "IDR")
            {
                txtExchRate.Text = "1.00";
                txtExchRate.Enabled = false;
            }
            else
            {
                txtExchRate.Enabled = true;
            }
        }

        private void buttonPaymentMethod_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "PaymentModeID";
            tmpSearch.Order = "PaymentModeID Asc";
            tmpSearch.Table = "[dbo].[PaymentMode]";
            tmpSearch.QuerySearch = "SELECT a.PaymentModeID, a.PaymentModeName FROM [dbo].[PaymentMode] a";
            tmpSearch.FilterText = new string[] { "PaymentModeID", "PaymentModeName" };
            tmpSearch.Select = new string[] { "PaymentModeID", "PaymentModeName" };
            tmpSearch.ShowDialog();

            if (ConnectionString.Kodes != null)
            {
                txtPaymentMethod.Text = ConnectionString.Kodes[0];
                ConnectionString.Kodes = null;
            }
        }

        private void HeaderAccountsPayable_Load(object sender, EventArgs e)
        {
            AddcmbCurrency();
            AddcmbInvoiceType();
            ModeLoad();

            Conn = ConnectionString.GetConnection();

            Cmd = new SqlCommand("SELECT CurrencyID from [ISBS-NEW4].[dbo].[CurrencyTable]", Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCurrency.Items.Add(Dr[0]);
            }
            Dr.Close();

            cmbTermOfPayment.Items.Clear();
            Cmd = new SqlCommand("select [TermOfPayment] from [ISBS-NEW4].[dbo].[TermOfPayment]", Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbTermOfPayment.Items.Add(Dr[0]);
            }
            Dr.Close();

            Conn.Close();
            GetDataHeader();

            this.CenterToScreen();
            RefreshDetailItem();

            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
            else if (Mode == "ModeView")
            {
                ModeView();
            }
            else if (Mode == "Approve")
            {
                ModeApprove();
            }
        }

        public void ModeNew()
        {
            invoiceId = "";

            groupBox2.Visible = false;
            btnReject.Visible = false;
            btnRevisi.Visible = false;
            btnApproved.Visible = false;
            btnCancelApprove.Visible = false;
            txtDiscountAmt.ReadOnly = false;

            buttonSaveGeneralAccountPayable.Visible = true;
            btnExit.Visible = true;
            buttonEditGeneralAccountPayable.Visible = false;
            buttonCancelGeneralAccountPayable.Visible = false;

            dgvAttachmentReadOnlyFalse();

            RefreshDetailItem();
        }

        public void ModeEdit()
        {
            Mode = "Edit";
            WarnaGrid();

            txtDiscountAmt.ReadOnly = false;
            groupBox2.Visible = false;
            btnReject.Visible = false;
            btnRevisi.Visible = false;
            btnApproved.Visible = false;
            btnCancelApprove.Visible = false;

            cmbInvoiceType.Enabled = true;
            dtInvoiceDate.Enabled = false;
            btnVendAccount.Enabled = true;
            txtVendInvoiceNumber.Enabled = true;
            cmbCurrency.Enabled = true;

            dtVendInvoiceDate.Enabled = true;
            dtVendInvoiceDueDate.Enabled = true;
            txtInvoiceAmount.Enabled = true;
            txtInvoicePPN.Enabled = true;
            txtInvoicePPH.Enabled = true;

            dtPaymentDueDate.Enabled = true;
            cmbTermOfPayment.Enabled = true;
            txtPaymentMethod.Enabled = true;
            txtNPWP.Enabled = true;
            dtTaxDate.Enabled = true;

            txtTaxNumber.Enabled = true;
            txtTaxName.Enabled = true;
            richTextBoxTaxAddress.Enabled = true;
            richTextBoxNotes.Enabled = true;
            btnAddAttachment.Enabled = true;

            buttonNewGeneralAccountPayable.Enabled = true;
            buttonDeleteAccountPayable.Enabled = true;
            buttonSaveGeneralAccountPayable.Visible = true;
            btnExit.Visible = false;
            buttonEditGeneralAccountPayable.Visible = false;

            buttonCancelGeneralAccountPayable.Visible = true;
            btnDeleteFile.Enabled = true;
            buttonPaymentMethod.Enabled = true;
            btnDownloadFile.Enabled = true;
            
            dgvDetail.DefaultCellStyle.BackColor = Color.White;

            dgvAttachmentReadOnlyFalse();

            RefreshDetailItem();
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            WarnaGrid();
            groupBox2.Visible = false;
            btnReject.Visible = false;
            btnRevisi.Visible = false;
            btnApproved.Visible = false;
            btnCancelApprove.Visible = false;

            txtDiscountAmt.ReadOnly = true;
            cmbInvoiceType.Enabled = false;
            dtInvoiceDate.Enabled = false;
            btnVendAccount.Enabled = false;
            txtVendInvoiceNumber.Enabled = false;
            cmbCurrency.Enabled = false;

            dtVendInvoiceDate.Enabled = false;
            dtVendInvoiceDueDate.Enabled = false;
            txtInvoiceAmount.Enabled = false;
            txtInvoicePPN.Enabled = false;
            txtInvoicePPH.Enabled = false;

            dtPaymentDueDate.Enabled = false;
            cmbTermOfPayment.Enabled = false;
            txtPaymentMethod.Enabled = false;
            txtNPWP.Enabled = false;
            dtTaxDate.Enabled = false;

            txtTaxNumber.Enabled = false;
            txtTaxName.Enabled = false;
            richTextBoxTaxAddress.Enabled = false;
            richTextBoxNotes.Enabled = false;

            btnAddAttachment.Enabled = false;
            buttonNewGeneralAccountPayable.Enabled = false;
            buttonDeleteAccountPayable.Enabled = false;
            buttonSaveGeneralAccountPayable.Visible = false;
            btnExit.Visible = true;

            buttonEditGeneralAccountPayable.Visible = true;
            buttonCancelGeneralAccountPayable.Visible = false;
            buttonPaymentMethod.Enabled = false;

            btnDeleteFile.Enabled = false;
            btnDownloadFile.Enabled = false;
            dgvDetail.DefaultCellStyle.BackColor = Color.LightGray;
            dgvDetail.ReadOnly = true;

            dgvAttachment.ReadOnly = true;

            RefreshDetailItem();
        }

        public void ModeView()
        {
            Mode = "ModeView";
            WarnaGrid();

            txtDiscountAmt.ReadOnly = true;
            groupBox2.Visible = false;
            btnReject.Visible = false;
            btnRevisi.Visible = false;
            btnApproved.Visible = false;
            btnCancelApprove.Visible = false;

            cmbInvoiceType.Enabled = false;
            dtInvoiceDate.Enabled = false;
            btnVendAccount.Enabled = false;
            txtVendInvoiceNumber.Enabled = false;
            cmbCurrency.Enabled = false;

            dtVendInvoiceDate.Enabled = false;
            dtVendInvoiceDueDate.Enabled = false;
            txtInvoiceAmount.Enabled = false;
            txtInvoicePPN.Enabled = false;
            txtInvoicePPH.Enabled = false;

            dtPaymentDueDate.Enabled = false;
            cmbTermOfPayment.Enabled = false;
            txtPaymentMethod.Enabled = false;
            txtNPWP.Enabled = false;
            dtTaxDate.Enabled = false;

            txtTaxNumber.Enabled = false;
            txtTaxName.Enabled = false;
            richTextBoxTaxAddress.Enabled = false;
            richTextBoxNotes.Enabled = false;

            btnAddAttachment.Enabled = false;
            buttonSaveGeneralAccountPayable.Visible = false;
            btnExit.Visible = true;
            buttonEditGeneralAccountPayable.Visible = true;
            buttonCancelGeneralAccountPayable.Visible = false;

            buttonNewGeneralAccountPayable.Enabled = false;
            btnDeleteFile.Enabled = false;
            btnDownloadFile.Enabled = false;
            buttonPaymentMethod.Enabled = false;
            buttonDeleteAccountPayable.Enabled = false;

            dgvAttachment.ReadOnly = true;

            dgvDetail.ReadOnly = true;

            RefreshDetailItem();
        }

        public void ModeApprove()
        {
            Mode = "Approve";
            WarnaGrid();
            txtDiscountAmt.ReadOnly = true;
            cmbInvoiceType.Enabled = false;
            dtInvoiceDate.Enabled = false;
            btnVendAccount.Enabled = false;
            txtVendInvoiceNumber.Enabled = false;
            cmbCurrency.Enabled = false;

            dtVendInvoiceDate.Enabled = false;
            dtVendInvoiceDueDate.Enabled = false;
            txtInvoiceAmount.Enabled = false;
            txtInvoicePPN.Enabled = false;
            txtInvoicePPH.Enabled = false;

            dtPaymentDueDate.Enabled = false;
            cmbTermOfPayment.Enabled = false;
            txtPaymentMethod.Enabled = false;
            txtNPWP.Enabled = false;
            dtTaxDate.Enabled = false;

            txtTaxNumber.Enabled = false;
            txtTaxName.Enabled = false;
            richTextBoxTaxAddress.Enabled = false;
            richTextBoxNotes.Enabled = false;

            btnAddAttachment.Enabled = false;
            buttonSaveGeneralAccountPayable.Visible = false;
            btnExit.Visible = true;
            buttonEditGeneralAccountPayable.Visible = true;
            buttonCancelGeneralAccountPayable.Visible = false;

            buttonNewGeneralAccountPayable.Enabled = false;
            btnDeleteFile.Enabled = false;
            btnDownloadFile.Enabled = false;
            buttonDeleteAccountPayable.Enabled = false;

            dgvDetail.ReadOnly = true;
            groupBox2.Visible = true;
            btnReject.Visible = true;
            btnRevisi.Visible = true;
            btnApproved.Visible = true;
            btnCancelApprove.Visible = true;

            if (((txtStatusKode.Text == "01" || txtStatusKode.Text == "09") && ControlMgr.GroupName == "Tax Admin") || ((txtStatusKode.Text == "02" || txtStatusKode.Text == "06" || txtStatusKode.Text == "10" || txtStatusKode.Text == "12") && ControlMgr.GroupName == "AP Manager"))
            {
                btnCancelApprove.Enabled = false;
                btnReject.Enabled = true;
                btnRevisi.Enabled = true;
                btnApproved.Enabled = true;
            }
            else if ((txtStatusKode.Text == "11") && ControlMgr.GroupName == "Tax Manager")
            {
                btnCancelApprove.Enabled = false;
                btnReject.Enabled = false;
                btnRevisi.Enabled = false;
                btnApproved.Enabled = true;
            }
            else
            {
                btnCancelApprove.Enabled = true;
                btnReject.Enabled = false;
                btnRevisi.Enabled = false;
                btnApproved.Enabled = false;
            }
           
            buttonSaveGeneralAccountPayable.Visible = true;
            btnExit.Visible = false;
            buttonSaveGeneralAccountPayable.Visible = false;
            btnExit.Visible = true;
            buttonEditGeneralAccountPayable.Visible = false;
            buttonCancelGeneralAccountPayable.Visible = false;

            richTextBoxNotes.Enabled = true;
            dgvDetail.ReadOnly = true;
            buttonPaymentMethod.Enabled = false;
            dgvDetail.DefaultCellStyle.BackColor = Color.LightGray;

            dgvAttachment.ReadOnly = true;

            RefreshDetailItem();
        }

        private void buttonDeleteAccountPayable_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Apakah Anda Ingin Menghapus Item?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (dgvDetail.RowCount > 0)
                {
                    if (dgvDetail.RowCount > 0)
                    {
                        Index = dgvDetail.CurrentRow.Index;
                        DialogResult dialogResult = MessageBox.Show("Apakah data: " + Environment.NewLine + "No = " + dgvDetail.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "PurchId = " + dgvDetail.Rows[Index].Cells["PONo"].Value.ToString() + Environment.NewLine + "Akan dihapus?", "Delete Confirmation!", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            dgvDetail.Rows.RemoveAt(Index);
                            for (int i = 0; i < dgvDetail.RowCount; i++)
                            {
                                dgvDetail.Rows[i].Cells["No"].Value = i + 1;
                            }
                        }
                    }
                }
                RefreshDetailItem();
            }
        }

        public void GetDataHeader()
        {
            try
            {
                if (invoiceId == "")
                {
                    invoiceId = txtInvoiceId.Text.Trim();
                }
                using (Conn = ConnectionString.GetConnection())
                {
                    Query = "Select InvoiceType, InvoiceDate, InvoiceId, VendId, VendName, CurrencyId, ExchRate, VendorInvoiceNo, VendorInvoiceDate, DueDate, InvoiceAmount,TotalAmountToPay,InvoiceTaxBaseAmount,TaxPercent, InvoiceTaxAmount, PPHTaxPercent, PPHTaxAmount, PaymentDueDate, TermOfPayment, PaymentMethod, NPWP, TaxNum, TaxAddress, TaxName, TaxDate, Notes,[Additional_Disc], TransStatus FROM [dbo].[VendInvoiceH]";
                    Query += "WHERE InvoiceId='" + invoiceId + "';";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        cmbInvoiceType.Text = Dr["InvoiceType"].ToString();
                        dtInvoiceDate.Text = Dr["InvoiceDate"].ToString();
                        txtInvoiceId.Text = Dr["InvoiceId"].ToString();
                        txtVendAccount.Text = Dr["VendId"].ToString();
                        txtVendName.Text = Dr["VendName"].ToString();

                        cmbCurrency.Text = Dr["CurrencyId"].ToString();
                        txtExchRate.Text = Dr["ExchRate"].ToString();
                        txtVendInvoiceNumber.Text = Dr["VendorInvoiceNo"].ToString();
                        dtVendInvoiceDate.Text = Dr["VendorInvoiceDate"].ToString();
                        dtVendInvoiceDueDate.Text = Dr["DueDate"].ToString();

                        txtInvoiceAmount.Text = Convert.ToDecimal(Dr["InvoiceAmount"]).ToString("N2");
                        txtAmountPay.Text = Convert.ToDecimal(Dr["TotalAmountToPay"]).ToString("N2");
                        txtAmountNett.Text = Convert.ToDecimal(Dr["InvoiceTaxBaseAmount"]).ToString("N2");
                        txtInvoicePPN.Text = Convert.ToDecimal(Dr["TaxPercent"]).ToString("N2");
                        txtInvoicePPNAmount.Text = Convert.ToDecimal(Dr["InvoiceTaxAmount"]).ToString("N2");
                        txtInvoicePPH.Text = Convert.ToDecimal(Dr["PPHTaxPercent"]).ToString("N2");
                        txtInvoicePPHAmount.Text = Convert.ToDecimal(Dr["PPHTaxAmount"]).ToString("N2");

                        dtPaymentDueDate.Text = Dr["PaymentDueDate"].ToString();
                        cmbTermOfPayment.Text = Dr["TermOfPayment"].ToString();
                        txtPaymentMethod.Text = Dr["PaymentMethod"].ToString();
                        txtNPWP.Text = Dr["NPWP"].ToString();
                        txtTaxNumber.Text = Dr["TaxNum"].ToString();

                        richTextBoxTaxAddress.Text = Dr["TaxAddress"].ToString();
                        txtTaxName.Text = Dr["TaxName"].ToString();
                        dtTaxDate.Text = Dr["TaxDate"].ToString();
                        richTextBoxNotes.Text = Dr["Notes"].ToString();

                        txtStatusKode.Text = Dr["TransStatus"].ToString();
                        txtDiscountAmt.Text = Dr["Additional_Disc"].ToString();
                    }
                    Dr.Close();

                    tabControlAccountPayable.SelectedIndex = 1;
                    tabControlAccountPayable.SelectedIndex = 0;

                    dgvDetail.DataSource = null;

                    if (dgvDetail.RowCount == 0)
                    {
                        if (cmbInvoiceType.Text == "Invoice")
                        {
                            dgvDetail.Rows.Clear();
                            dgvDetail.ColumnCount = 14;

                            dgvDetail.Columns[0].Name = "No";
                            dgvDetail.Columns[1].Name = "PONo";
                            dgvDetail.Columns[2].Name = "PODate";
                            dgvDetail.Columns[3].Name = "POAmount";

                            dgvDetail.Columns[4].Name = "PODueDate";
                            dgvDetail.Columns[5].Name = "GRNo";
                            dgvDetail.Columns[6].Name = "DPAmount";
                            dgvDetail.Columns[7].Name = "DPOutstanding";

                            dgvDetail.Columns[8].Name = "POInvoiced";
                            dgvDetail.Columns[9].Name = "POUnInvoice";
                            dgvDetail.Columns[10].Name = "GRAmount";
                            dgvDetail.Columns[11].Name = "PotDP";

                            dgvDetail.Columns[12].Name = "GRPayable";
                            dgvDetail.Columns[13].Name = "InvoiceAmount";
                        }

                        if (cmbInvoiceType.Text == "Uang Muka")
                        {
                            dgvDetail.Rows.Clear();
                            dgvDetail.ColumnCount = 14;

                            dgvDetail.Columns[0].Name = "No";
                            dgvDetail.Columns[1].Name = "PONo";
                            dgvDetail.Columns[2].Name = "PODate";
                            dgvDetail.Columns[3].Name = "POAmount";

                            dgvDetail.Columns[4].Name = "PODueDate";
                            dgvDetail.Columns[5].Name = "DPRequired";
                            dgvDetail.Columns[6].Name = "DPPercent";
                            dgvDetail.Columns[7].Name = "DPAmount";

                            dgvDetail.Columns[8].Name = "DPDeduct";
                            dgvDetail.Columns[9].Name = "DPOutstanding";
                            dgvDetail.Columns[10].Name = "POInvoiced";
                            dgvDetail.Columns[11].Name = "POUnInvoice";

                            dgvDetail.Columns[12].Name = "PayableAmount";
                            dgvDetail.Columns[13].Name = "InvoiceAmount";
                        }
                    }
                    if (cmbInvoiceType.Text == "Uang Muka")
                    {
                        Query = "select distinct SeqNo, PurchId, PurchDate,PurchAmount,PurchDueDate, DPRequired,DPPercent, DPAmount,DPAmountDeduct,DPAmountOutstanding,PurchPaidAmount,PurchAmountOutstanding, PayableAmount, InvoiceAmount ";
                        Query += "from VendInvoice_Dtl ";
                        Query += "where InvoiceId='" + invoiceId + "' order by SeqNo asc ";
                    }
                    if (cmbInvoiceType.Text == "Invoice")
                    {
                        Query = "select distinct SeqNo, PurchId, PurchDate, PurchAmount, PurchDueDate, GRNo, DPAmount, DPAmountOutstanding,PurchPaidAmount,PurchAmountOutstanding,GRAmount,PotDP, PayableAmount, InvoiceAmount ";
                        Query += "from VendInvoice_Dtl ";
                        Query += "where InvoiceId='" + invoiceId + "' order by SeqNo asc ";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    int i = 0;

                    dgvDetail.Rows.Clear();

                    while (Dr.Read())
                    {
                        if (cmbInvoiceType.Text == "Uang Muka")
                        {
                            this.dgvDetail.Rows.Add(Dr["SeqNo"], Dr["PurchId"], Dr["PurchDate"], Dr["PurchAmount"], Dr["PurchDueDate"], Dr["DPRequired"],
                            Dr["DPPercent"], Dr["DPAmount"], Dr["DPAmountDeduct"],
                            Dr["DPAmountOutstanding"], Dr["PurchPaidAmount"], Dr["PurchAmountOutstanding"], Dr["PayableAmount"], Dr["InvoiceAmount"]);
                        }
                        if (cmbInvoiceType.Text == "Invoice")
                        {
                            this.dgvDetail.Rows.Add(Dr["SeqNo"], Dr["PurchId"], Dr["PurchDate"], Dr["PurchAmount"], Dr["PurchDueDate"], Dr["GRNo"], Dr["DPAmount"],
                            Dr["DPAmountOutstanding"], Dr["PurchAmountOutstanding"], Dr["PurchPaidAmount"],
                            Dr["GRAmount"], Dr["PotDP"], Dr["PayableAmount"], Dr["InvoiceAmount"]);
                        }
                        i++;
                    }
                    Dr.Close();

                    dgvDetail.AutoResizeColumns();
                    test.Clear();

                    dgvAttachment.Rows.Clear();

                    if (dgvAttachment.RowCount - 1 <= 0)
                    {
                        dgvAttachment.ColumnCount = 6;

                        dgvAttachment.Columns[0].Name = "FileName";
                        dgvAttachment.Columns[1].Name = "ContentType";
                        dgvAttachment.Columns[2].Name = "File Size (kb)";
                        dgvAttachment.Columns[3].Name = "Attachment";
                        dgvAttachment.Columns[4].Name = "Id";
                        dgvAttachment.Columns[5].Name = "FileType";

                        dgvAttachment.Columns["Attachment"].Visible = false;
                        dgvAttachment.Columns["Id"].Visible = false;
                    }

                    //dgvAttachmentReadOnlyFalse();

                    Query = "Select * From [tblAttachments] Where ReffTableName = 'VendInvoiceH' And ReffTransId = '" + invoiceId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"], Dr["FileType"]);
                        test.Add((byte[])Dr["Attachment"]);

                        cell = new DataGridViewComboBoxCell();
                        cell.Items.Add("Select");
                        cell.Items.Add("Invoice Vendor");
                        cell.Items.Add("Tax Invoice");
                        cell.Items.Add("Tanda Terima");
                        cell.Value = Dr["FileType"].ToString();
                        dgvAttachment.Rows[(dgvAttachment.Rows.Count - 1)].Cells["FileType"] = cell;
                    }
                }
               
                //NoteRetur
                if (cmbInvoiceType.Text != "Uang Muka")
                {
                    AddReturNotaBeli();
                    using (Conn = ConnectionString.GetConnection())
                    {
                        Query = "select distinct SeqNo,NRB_No,NRB_Date,GR_No,GR_Date,PO_No,PO_Date,Amount ";
                        Query += "from VendInvoice_Dtl_NotaRetur ";
                        Query += "where Invoice_Id='" + invoiceId + "' order by SeqNo asc ";

                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            for (int j = 0; j < dgvRetur.RowCount; j++)
                            {
                                if (dgvRetur.Rows[j].Cells["No"].Value.ToString() == Dr["SeqNo"].ToString() && dgvRetur.Rows[j].Cells["PONo"].Value.ToString() == Dr["PO_No"].ToString())
                                {
                                    dgvRetur.Rows[j].Cells["Checked"].Value = true;
                                }
                            }
                        }
                        Dr.Close();
                        //NotaRetur
                    }
                }
                CountAmountAP();
                UpdateAmount();
                dgvAttachment.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvAttachmentReadOnlyFalse()
        {
            dgvAttachment.ReadOnly = false;
            if (dgvAttachment.RowCount > 0)
            {
                dgvAttachment.Columns["FileName"].ReadOnly = true;
                dgvAttachment.Columns["ContentType"].ReadOnly = true;
                dgvAttachment.Columns["File Size (kb)"].ReadOnly = true;
                dgvAttachment.Columns["Attachment"].ReadOnly = true;
                dgvAttachment.Columns["Id"].ReadOnly = true;
            }
            dgvDetail.ReadOnly = false;
            for (int i = 0; i < dgvDetail.ColumnCount; i++)
            {
                if (dgvDetail.Columns[i].Name != "InvoiceAmount")
                {
                    dgvDetail.Columns[i].ReadOnly = true;
                }
            }
            if (dgvDetailItem.RowCount > 0)
            {
                dgvDetailItem.ReadOnly = false;
                for (int i = 0; i < dgvDetailItem.ColumnCount; i++)
                {
                    if (dgvDetailItem.Columns[i].Name != "Disc Allocate")
                    {
                        dgvDetailItem.Columns[i].ReadOnly = true;
                    }
                }
            }
            if (dgvRetur.RowCount > 0)
            {
                dgvRetur.ReadOnly = false;
                for (int i = 0; i < dgvRetur.ColumnCount; i++)
                {
                    if (dgvRetur.Columns[i].Name != "Check")
                    {
                        dgvRetur.Columns[i].ReadOnly = true;
                    }
                }
            }
        }

        private void AddDetailDgv(string invoiceId)
        {
            if (cmbInvoiceType.Text == "Uang Muka")
            {
                for (int i = 0; i < dgvDetail.RowCount; i++)
                {
                    Query = "Insert Into VendInvoice_Dtl (InvoiceDate,InvoiceId,SeqNo,PurchId,PurchDate,PurchDueDate,PurchAmount,DPRequired,DPPercent,DPAmount,DPAmountDeduct,DPAmountOutstanding,InvoiceAmount,CreatedDate,CreatedBy) Values (";
                    Query += "@InvoiceDate,";
                    Query += "@InvoiceId,";
                    Query += "@No,";
                    Query += "@PONo,";
                    Query += "@PODate,";
                    Query += "@PODueDate,";
                    Query += "@POAmount,";
                    Query += "@DPRequired,";
                    Query += "@DPPercent,";
                    Query += "@DPAmount,";
                    Query += "@DPDeduct,";
                    Query += "@DPOutstanding,";
                    Query += "@InvoiceAmount,";
                    Query += "(select CreatedDate from VendInvoiceH where InvoiceId=@InvoiceId),";
                    Query += "@Login);";
                    if (Mode == "Edit")
                    {
                        Query += "Update VendInvoice_Dtl set UpdatedDate=getdate(), UpdatedBy=@Login where InvoiceId=@InvoiceId and SeqNo=@No";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.Add("@InvoiceDate", dtInvoiceDate.Value.ToString("yyyy-MM-dd"));
                    Cmd.Parameters.Add("@InvoiceId", invoiceId);
                    Cmd.Parameters.Add("@No", (i + 1));
                    Cmd.Parameters.Add("@PONo", dgvDetail.Rows[i].Cells["PONo"].Value.ToString());
                    Cmd.Parameters.Add("@PODate", Convert.ToDateTime(dgvDetail.Rows[i].Cells["PODate"].Value).ToString("yyyy-MM-dd"));
                    Cmd.Parameters.Add("@PODueDate", Convert.ToDateTime(dgvDetail.Rows[i].Cells["PODueDate"].Value).ToString("yyyy-MM-dd"));
                    Cmd.Parameters.Add("@POAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["POAmount"].Value.ToString()));
                    Cmd.Parameters.Add("@DPRequired", dgvDetail.Rows[i].Cells["DPRequired"].Value.ToString());
                    Cmd.Parameters.Add("@DPPercent", dgvDetail.Rows[i].Cells["DPPercent"].Value.ToString());
                    Cmd.Parameters.Add("@DPAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["DPAmount"].Value.ToString()));
                    Cmd.Parameters.Add("@DPDeduct", Convert.ToDecimal(dgvDetail.Rows[i].Cells["DPDeduct"].Value.ToString()));
                    Cmd.Parameters.Add("@DPOutstanding", Convert.ToDecimal(dgvDetail.Rows[i].Cells["DPOutstanding"].Value.ToString()));
                    Cmd.Parameters.Add("@InvoiceAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value.ToString()));
                    Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                    Cmd.ExecuteNonQuery();
                }
            }
            if (cmbInvoiceType.Text == "Invoice")
            {
                for (int i = 0; i < dgvDetail.RowCount; i++)
                {   
                    Query = "Insert Into VendInvoice_Dtl (InvoiceDate,InvoiceId,SeqNo,PurchId,PurchDate,PurchDueDate,GRNo,PurchAmount,DPAmount, DPAmountOutstanding, PurchPaidAmount, PurchAmountOutstanding, GRAmount, PotDP, PayableAmount, InvoiceAmount, CreatedDate, CreatedBy) Values (";
                    Query += "@InvoiceDate,@invoiceId,@No,@PONo,@PODate,";
                    //edited by Thaddaeus, 10 Sept 2018
                    if (dgvDetail.Rows[i].Cells["PODueDate"].Value.ToString() == "")
                    {
                        Query += "'',";
                    }
                    else
                    {
                        Query += "@PODueDate,";
                    }
                    Query += "@GRNo,@POAmount,@DPAmount,@DPOutstanding,@POInvoiced,@POUnInvoice,@GRAmount,@PotDP,@GRPayable,@InvoiceAmount,";
                    Query += "(select CreatedDate from VendInvoiceH where InvoiceId=@invoiceId),";
                    Query += "@Login);";
                    if (Mode == "Edit")
                    {
                        Query += "Update VendInvoice_Dtl set UpdatedDate=getdate(), UpdatedBy=@Login where InvoiceId=@invoiceId and SeqNo=@No;";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.Add(new SqlParameter("@InvoiceDate", dtInvoiceDate.Value.ToString("yyyy-MM-dd")));
                    Cmd.Parameters.Add(new SqlParameter("@invoiceId", invoiceId));
                    Cmd.Parameters.Add(new SqlParameter("@No", (i + 1)));
                    Cmd.Parameters.Add(new SqlParameter("@PONo", dgvDetail.Rows[i].Cells["PONo"].Value.ToString()));
                    DateTime PODate = Convert.ToDateTime(dgvDetail.Rows[i].Cells["PODate"].Value);
                    DateTime PODueDate = Convert.ToDateTime(dgvDetail.Rows[i].Cells["PODueDate"].Value.ToString());
                    Cmd.Parameters.Add(new SqlParameter("@PODate", PODate.Year + "-" + PODate.Month + "-" + PODate.Day));
                    Cmd.Parameters.Add(new SqlParameter("@PODueDate", +PODueDate.Year + "-" + PODueDate.Month + "-" + PODueDate.Day));
                    Cmd.Parameters.Add(new SqlParameter("@GRNo", dgvDetail.Rows[i].Cells["GRNo"].Value.ToString()));
                    Cmd.Parameters.Add(new SqlParameter("@POAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["POAmount"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@DPAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["DPAmount"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@DPOutstanding", Convert.ToDecimal(dgvDetail.Rows[i].Cells["DPOutstanding"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@POInvoiced", Convert.ToDecimal(dgvDetail.Rows[i].Cells["POInvoiced"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@POUnInvoice", Convert.ToDecimal(dgvDetail.Rows[i].Cells["POUnInvoice"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@GRAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["GRAmount"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@PotDP", Convert.ToDecimal(dgvDetail.Rows[i].Cells["PotDP"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@GRPayable", Convert.ToDecimal(dgvDetail.Rows[i].Cells["GRPayable"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@InvoiceAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@Login", ControlMgr.UserId));
                    Cmd.ExecuteNonQuery();
                }
            }

            for (int i = 0; i < dgvRetur.RowCount; i++)
            {
                if (Convert.ToBoolean(dgvRetur.Rows[i].Cells["Check"].Value.ToString()) == true)
                {
                    Query = "Insert Into VendInvoice_Dtl_NotaRetur (Invoice_Id, SeqNo, NRB_No, NRB_Date, GR_NO, GR_Date, PO_No, PO_Date, Amount, Pot_Retur, CreatedDate, CreatedBy) Values (";
                    Query += "@InvoiceId,";
                    Query += "@No,";
                    Query += "@NRBNo,";
                    Query += "@NRBDate,";
                    Query += "@GRNo,";
                    Query += "@GRDate,";
                    Query += "@PONo,";
                    Query += "@PODate,";
                    Query += "@TotalRetur,";
                    Query += "'0',";
                    Query += "(select CreatedDate from VendInvoiceH where InvoiceId=@InvoiceId),";
                    Query += "@Login);";
                    if (Mode == "Edit")
                    {
                        Query += "Update VendInvoice_Dtl_NotaRetur set UpdatedDate=getdate(), UpdatedBy=@Login where Invoice_Id=@InvoiceId and SeqNo=@No";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.Add(new SqlParameter("@InvoiceId", invoiceId));
                    Cmd.Parameters.Add(new SqlParameter("@No", dgvRetur.Rows[i].Cells["No"].Value.ToString()));
                    Cmd.Parameters.Add(new SqlParameter("@NRBNo", dgvRetur.Rows[i].Cells["NRBNo"].Value.ToString()));
                    Cmd.Parameters.Add(new SqlParameter("@NRBDate", Convert.ToDateTime(dgvRetur.Rows[i].Cells["NRBDate"].Value).ToString("yyyy-MM-dd")));
                    Cmd.Parameters.Add(new SqlParameter("@GRNo", dgvRetur.Rows[i].Cells["GRNo"].Value.ToString()));
                    Cmd.Parameters.Add(new SqlParameter("@GRDate", Convert.ToDateTime(dgvRetur.Rows[i].Cells["GRDate"].Value).ToString("yyyy-MM-dd")));
                    Cmd.Parameters.Add(new SqlParameter("@PONo", dgvRetur.Rows[i].Cells["PONo"].Value.ToString()));
                    Cmd.Parameters.Add(new SqlParameter("@PODate", Convert.ToDateTime(dgvRetur.Rows[i].Cells["PODate"].Value).ToString("yyyy-MM-dd")));
                    Cmd.Parameters.Add(new SqlParameter("@TotalRetur", Convert.ToDecimal(dgvRetur.Rows[i].Cells["TotalRetur"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@Login", ControlMgr.UserId));
                    Cmd.ExecuteNonQuery();
                }
            }

        }

        private void buttonSaveGeneralAccountPayable_Click(object sender, EventArgs e)
        {
            #region Validasi
            if (dgvDetail.RowCount == 0)
            {
                MessageBox.Show("Jumlah Item Tidak Boleh Kosong.");
                return;
            }
            else
            {
                
                DateTime DueDate = DateTime.ParseExact(dtVendInvoiceDueDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                DateTime CurrentDate = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                for (int i = 0; i < dgvAttachment.RowCount; i++)
                {
                    if(dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() == "Select")
                    {
                        MessageBox.Show("Attachment no-" + i+1 + " Filetype harus dipilih.");
                        return;
                    }
                    if (dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() == "Tax Invoice")
                    {
                        if(Convert.ToDecimal(txtInvoicePPN.Text) <= 0)
                        {
                            MessageBox.Show("PPN harus lebih besar dari 0.");
                            return;
                        }
                    }
                }

                if (txtVendAccount.Text == "")
                {
                    MessageBox.Show("Vendor Tidak Boleh Kosong.");
                    return;
                }
             
                if (DueDate < CurrentDate && txtInvoiceId.Text == "")
                {
                    MessageBox.Show("Due Date Harus Lebih Besar dari Hari Ini");
                    return;
                }
                else if (dtPaymentDueDate.Text == "")
                {
                    MessageBox.Show("Payment Due Date Tidak Boleh Kosong.");
                    dtPaymentDueDate.Focus();
                    return;
                }
                else if (txtExchRate.Text == "")
                {
                    MessageBox.Show("Currency tidak Boleh Kosong.");
                    cmbCurrency.Focus();
                    return;
                }
                else if (txtInvoiceAmount.Text == "")
                {
                    MessageBox.Show("Invoice Ammount Tidak Boleh Kosong.");
                    txtInvoiceAmount.Focus();
                    return;
                }

                else if (txtInvoicePPNAmount.Text == "")
                {
                    MessageBox.Show("Invoice PPN Amount Tidak Boleh Kosong.");
                    txtInvoicePPNAmount.Focus();
                    return;
                }
                else if (txtInvoicePPH.Text == "")
                {
                    MessageBox.Show("Invoice PPH Amount Tidak Boleh Kosong.");
                    txtInvoicePPH.Focus();
                    return;
                }
                else if (cmbInvoiceType.Text == "")
                {
                    MessageBox.Show("Invoice Type Tidak Boleh Kosong.");
                    cmbInvoiceType.Focus();
                    return;
                }
                else if (txtVendInvoiceNumber.Text == "")
                {
                    MessageBox.Show("Vendor Invoice Number Tidak Boleh Kosong.");
                    txtVendInvoiceNumber.Focus();
                    return;
                }
                else if (cmbTermOfPayment.Text == "")
                {
                    MessageBox.Show("Term Of Payment Tidak Boleh Kosong.");
                    cmbTermOfPayment.Focus();
                    return;
                }
                else if (txtPaymentMethod.Text == "")
                {
                    MessageBox.Show("Payment Method Tidak Boleh Kosong.");
                    txtPaymentMethod.Focus();
                    return;
                }
                else if (Convert.ToDecimal(txtInvoicePPN.Text) != 0 || Convert.ToDecimal(txtInvoicePPH.Text) != 0)
                {
                    if (dgvAttachment.RowCount == 0)
                    {
                        MessageBox.Show("File Attached Tidak Boleh Kosong.");
                        return;
                    }
                    else if (txtNPWP.Text == "")
                    {
                        MessageBox.Show("NPWP Tidak Boleh Kosong.");
                        txtNPWP.Focus();
                        return;
                    }

                    else if (txtTaxNumber.Text == "")
                    {
                        MessageBox.Show("Tax Number Tidak Boleh Kosong.");
                        txtTaxNumber.Focus();
                        return;
                    }
                    else if (txtTaxName.Text == "")
                    {
                        MessageBox.Show("Tax Name Tidak Boleh Kosong.");
                        txtTaxName.Focus();
                        return;
                    }
                    else if (richTextBoxTaxAddress.Text == "")
                    {
                        MessageBox.Show("Tax Address Tidak Boleh Kosong.");
                        richTextBoxTaxAddress.Focus();
                        return;
                    }

                    else if (Mode == "New")
                    {
                        if (dtTaxDate.Text == "")
                        {
                            MessageBox.Show("Tax Date Tidak Boleh Kosong.");
                            dtTaxDate.Focus();
                            return;
                        }
                    }
                }
                
                else if (Mode == "Edit")
                {
                    if (Convert.ToDecimal(txtInvoicePPN.Text) == 0 && Convert.ToDecimal(txtInvoicePPH.Text) == 0 && dgvAttachment.RowCount != 0)
                    {
                        DialogResult dr = MessageBox.Show("Attachments Akan Dihapus, Karena Tidak Terdapat Pajak", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            dgvAttachment.Rows.Clear();
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                //datagrid
                for (int i = 0; i < dgvDetail.RowCount; i++)
                {
                    if (dgvDetail.Rows[i].Cells["InvoiceAmount"].Value == DBNull.Value || Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value) == 0)
                    {
                        MessageBox.Show("Amount To Pay Tidak Boleh Kosong.");
                        return;
                    }
                }
                //hitung
                for (int i = 0; i < dgvDetail.Rows.Count; i++)
                {

                    if (txtAmountNett.Text != "")
                    {
                        if (dgvDetail.Rows[i].Cells["InvoiceAmount"].Value != DBNull.Value)
                        {
                            double a = Convert.ToDouble(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value);
                            double c = Convert.ToDouble(txtAmountNett.Text);

                            if (a <= c)
                            {
                                HitungAmountPay();
                            }
                            else
                            {
                                MessageBox.Show("Nilai Amount To Pay Tidak Boleh Lebih Besar Dari Nilai Invoice Amount Nett.");
                                dgvDetail.Focus();
                                dgvDetail.CurrentCell = dgvDetail.Rows[i].Cells["InvoiceAmount"];
                                return;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invoice Amount Nett Tidak Boleh Kosong.");
                        txtInvoiceAmount.Focus();
                        return;
                    }
                }

            }
            #endregion
            
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();

                    string VendorInvoicedate = dtVendInvoiceDate.Text == "" ? null : dtVendInvoiceDate.Value.ToString("yyyy-MM-dd");
                    string duedate = dtVendInvoiceDueDate.Text == "" ? null : dtVendInvoiceDueDate.Value.ToString("yyyy-MM-dd");
                    string invoiceDate = dtInvoiceDate.Text == "" ? null : dtInvoiceDate.Value.ToString("yyyy-MM-dd");
                    string PaymentDueDate = dtPaymentDueDate.Text == "" ? null : dtPaymentDueDate.Value.ToString("yyyy-MM-dd");
                    string TaxDate = dtTaxDate.Text == "" ? null : dtTaxDate.Value.ToString("yyyy-MM-dd");
           
                    Hitung();   //hitung untuk menampilkan pada datagrid
                    
                    //jika ada pajak
                    //status untuk tabel TransStatusTable pada database
                    if ((Convert.ToDecimal(txtInvoicePPN.Text) != 0 && Convert.ToDecimal(txtInvoicePPH.Text) == 0) || (Convert.ToDecimal(txtInvoicePPH.Text) != 0 && Convert.ToDecimal(txtInvoicePPN.Text) == 0) || (Convert.ToDecimal(txtInvoicePPN.Text) != 0 && Convert.ToDecimal(txtInvoicePPH.Text) != 0))
                    {
                        status = "01";
                    }
                    else //tidak ada pajak
                    {
                        status = "02";
                    }

                    if (Mode == "New" || txtInvoiceId.Text.Trim() == "")
                    {
                        string Jenis ="" ,kode="";
                        if (cmbInvoiceType.Text=="Uang Muka")
                        {
                            Jenis = "APRN";
                            kode = "APRN";
                        }
                        else //if (cmbInvoiceType.Text=="Invoice")
                        {
                            Jenis = "DPRN";
                            kode = "DPRN";
                        }
                        invoiceId = ConnectionString.GenerateSequenceNo(Jenis, kode, "", "", Conn, Cmd);

                        Query = "Insert into VendInvoiceH (InvoiceId, InvoiceDate,InvoiceType, VendId, VendName, CurrencyId, ExchRate, VendorInvoiceNo, VendorInvoiceDate, DueDate, InvoiceAmount,TotalAmountToPay,InvoiceTaxBaseAmount, TaxStatusCode,TaxPercent, InvoiceTaxAmount, PPHTaxPercent, PPHTaxAmount, PaymentDueDate, TermOfPayment, PaymentMethod,NPWP, TaxNum, TaxAddress, TaxName, TaxDate, Notes,TransStatus, CreatedDate, CreatedBy,[Additional_Disc]) values ";
                        Query += "(@InvoiceID, ";
                        Query += "@InvoiceDate, @InvoiceType, @VendAccount, @VendName, @Currency, @ExchRate, @VendInvoiceNumber, @InvoiceDate, ";
                        Query += "@VendInvoiceDueDate, @InvoiceAmount, @AmountPay, @AmountNett, '', @InvoicePPN, @InvoicePPNAmount, @InvoicePPH, @InvoicePPHAmount, ";
                        Query += "@PaymentDueDate, @TermOfPayment, @PaymentMethod, @NPWP, @TaxNumber, @TaxAddress, @TaxName, @TaxDate, @Notes, @Status, getdate(), @Login, @Discount);";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.Add(new SqlParameter("@InvoiceID",invoiceId));
                        Cmd.Parameters.Add(new SqlParameter("@InvoiceDate", dtInvoiceDate.Value.ToString("yyyy-MM-dd")));
                        Cmd.Parameters.Add(new SqlParameter("@InvoiceType", Convert.ToString(cmbInvoiceType.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@VendAccount", txtVendAccount.Text));
                        Cmd.Parameters.Add(new SqlParameter("@VendName", txtVendName.Text));
                        Cmd.Parameters.Add(new SqlParameter("@Currency", Convert.ToString(cmbCurrency.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@ExchRate", Convert.ToDecimal(txtExchRate.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@VendInvoiceNumber", Convert.ToDecimal(txtVendInvoiceNumber.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@VendInvoiceDueDate", dtVendInvoiceDueDate.Value.ToString("yyyy-MM-dd")));
                        Cmd.Parameters.Add(new SqlParameter("@InvoiceAmount", Convert.ToDecimal(txtInvoiceAmount.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@AmountPay", Convert.ToDecimal(txtAmountPay.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@AmountNett", Convert.ToDecimal(txtAmountNett.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@InvoicePPN", Convert.ToDecimal(txtInvoicePPN.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@InvoicePPNAmount", Convert.ToDecimal(txtInvoicePPNAmount.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@InvoicePPH", Convert.ToDecimal(txtInvoicePPH.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@InvoicePPHAmount", Convert.ToDecimal(txtInvoicePPHAmount.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@PaymentDueDate", dtPaymentDueDate.Value.ToString("yyyy-MM-dd")));
                        Cmd.Parameters.Add(new SqlParameter("@TermOfPayment", Convert.ToString(cmbTermOfPayment.Text)));
                        Cmd.Parameters.Add(new SqlParameter("@PaymentMethod", txtPaymentMethod.Text));
                        Cmd.Parameters.Add(new SqlParameter("@NPWP", txtNPWP.Text));
                        Cmd.Parameters.Add(new SqlParameter("@TaxNumber", txtTaxNumber.Text));
                        Cmd.Parameters.Add(new SqlParameter("@TaxAddress", richTextBoxTaxAddress.Text));
                        Cmd.Parameters.Add(new SqlParameter("@TaxName", txtTaxName.Text));
                        Cmd.Parameters.Add(new SqlParameter("@TaxDate", dtTaxDate.Value.ToString("yyyy-MM-dd")));
                        Cmd.Parameters.Add(new SqlParameter("@Notes", richTextBoxNotes.Text));
                        Cmd.Parameters.Add(new SqlParameter("@Status", status));
                        Cmd.Parameters.Add(new SqlParameter("@Login", ControlMgr.UserId));
                        Cmd.Parameters.Add(new SqlParameter("@Discount", Convert.ToDecimal(txtDiscountAmt.Text)));
                        Cmd.ExecuteNonQuery();

                        Query = "";
                        AddDetailDgv(invoiceId);

                        //save new attachment
                        SaveDgvAttachmentData();

                        //createdBy Thaddaeus, 10 sept 2018, begin
                        //save detail item
                        SaveDgvDetailItem(invoiceId);
                        //end======================================

                        //createdBy Thaddaeus, 26 sept 2018, begin
                        ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, status, "", invoiceId, "", "", "");
                        //end======================================

                        //Save VendInvoice_Logs
                        Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + status + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                        SaveVendInvoiceLogs(invoiceId, "-", LogsStatusDesc, "N");

                        scope.Complete();
                        MessageBox.Show("Data Invoice Id: " + invoiceId + " Berhasil Ditambahkan.");
                        txtInvoiceId.Text = invoiceId;
                        //MainMenu f = new MainMenu();
                    }//tutup if
                    else
                    {
                        Query = "Update VendInvoiceH set ";
                        Query += "InvoiceType=@InvoiceType,";
                        Query += "InvoiceDate=@InvoiceDate,";
                        Query += "InvoiceId=@InvoiceId,";
                        Query += "VendId=@VendId,";

                        Query += "VendName=@VendName,";
                        Query += "CurrencyId=@CurrencyId,";
                        Query += "ExchRate=@ExchRate,";
                        Query += "VendorInvoiceNo=@VendorInvoiceNo,";
                        Query += "VendorInvoiceDate=@VendorInvoiceDate,";

                        Query += "DueDate=@DueDate,";
                        Query += "InvoiceAmount=@InvoiceAmount,";
                        Query += "TotalAmountToPay=@TotalAmountToPay,";
                        Query += "TaxPercent=@TaxPercent,";
                        Query += "InvoiceTaxAmount=@InvoiceTaxAmount,";

                        //Query += "TaxStatusCode='',";
                        //Query += "PPHTaxCode='',";
                        Query += "PPHTaxPercent=@PPHTaxPercent,";
                        Query += "PPHTaxAmount=@PPHTaxAmount,";
                        Query += "PaymentDueDate=@PaymentDueDate,";
                        Query += "TermOfPayment=@TermOfPayment,";

                        Query += "PaymentMethod=@PaymentMethod,";
                        Query += "NPWP=@NPWP,";
                        Query += "TaxNum=@TaxNum,";
                        Query += "TaxAddress=@TaxAddress,";
                        Query += "TaxName=@TaxName,";

                        Query += "TaxDate=@TaxDate,";
                        Query += "Notes=@Notes,";
                        Query += "TransStatus=@TransStatus,";
                        Query += "[Additional_Disc] = @Additional_Disc,";
                        Query += "UpdatedDate=getdate(),";
                        Query += "UpdatedBy=@Login where InvoiceId=@InvoiceId";
                        
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.Add("@InvoiceType", Convert.ToString(cmbInvoiceType.Text));
                        Cmd.Parameters.Add("@InvoiceDate", dtInvoiceDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@InvoiceId", txtInvoiceId.Text);
                        Cmd.Parameters.Add("@VendId", txtVendAccount.Text);
                        Cmd.Parameters.Add("@VendName", txtVendName.Text);
                        Cmd.Parameters.Add("@CurrencyId", Convert.ToString(cmbCurrency.Text));
                        Cmd.Parameters.Add("@ExchRate", Convert.ToDecimal(txtExchRate.Text));
                        Cmd.Parameters.Add("@VendorInvoiceNo", txtVendInvoiceNumber.Text);
                        Cmd.Parameters.Add("@VendorInvoiceDate", dtVendInvoiceDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@DueDate", dtVendInvoiceDueDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@InvoiceAmount", Convert.ToDecimal(txtInvoiceAmount.Text));
                        Cmd.Parameters.Add("@TotalAmountToPay", Convert.ToDecimal(txtAmountPay.Text));
                        Cmd.Parameters.Add("@TaxPercent", Convert.ToDecimal(txtInvoicePPN.Text));
                        Cmd.Parameters.Add("@InvoiceTaxAmount", Convert.ToDecimal(txtInvoicePPNAmount.Text));
                        Cmd.Parameters.Add("@PPHTaxPercent", Convert.ToDecimal(txtInvoicePPH.Text));
                        Cmd.Parameters.Add("@PPHTaxAmount", Convert.ToDecimal(txtInvoicePPHAmount.Text));
                        Cmd.Parameters.Add("@PaymentDueDate", dtPaymentDueDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@TermOfPayment", Convert.ToString(cmbTermOfPayment.Text));
                        Cmd.Parameters.Add("@PaymentMethod", txtPaymentMethod.Text);
                        Cmd.Parameters.Add("@NPWP", txtNPWP.Text);
                        Cmd.Parameters.Add("@TaxNum", txtTaxNumber.Text);
                        Cmd.Parameters.Add("@TaxAddress", richTextBoxTaxAddress.Text);
                        Cmd.Parameters.Add("@TaxName", txtTaxName.Text);
                        Cmd.Parameters.Add("@TaxDate", dtTaxDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@Notes", richTextBoxNotes.Text);
                        Cmd.Parameters.Add("@TransStatus", status);
                        Cmd.Parameters.Add("@Additional_Disc", txtDiscountAmt.Text);
                        Cmd.Parameters.Add("@Login", ControlMgr.UserId);

                        Cmd.ExecuteNonQuery();

                        //createdBy Thaddaeus, 26 sept 2018, begin
                        ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, status, "Edit", txtInvoiceId.Text, "", "", "");
                        //end======================================

                        //untuk menyimpan nilai2 yang ada pada VendInvoice_Dtl
                        List<decimal> APSeqNo = new List<decimal>();
                        List<string> PurchId = new List<string>();
                        List<decimal> PurchAmount = new List<decimal>();
                        List<decimal> DPAmount = new List<decimal>();
                        List<decimal> DPAmountDeduct = new List<decimal>();

                        List<decimal> DPAmountOutstanding = new List<decimal>();
                        List<decimal> PurchPaidAmount = new List<decimal>();
                        List<decimal> PurchAmountOutstanding = new List<decimal>();
                        List<decimal> GRAmount = new List<decimal>();
                        List<decimal> RTBAmount = new List<decimal>();

                        List<decimal> RDNAmount = new List<decimal>();
                        List<decimal> GRNettAmount = new List<decimal>();
                        List<decimal> GR_DPAmount = new List<decimal>();
                        List<decimal> PayableDPAmount = new List<decimal>();
                        List<decimal> PayableAmount = new List<decimal>();
                        List<decimal> InvoiceAmount = new List<decimal>();
                        //mengambil nilai2 yg ada pada VendInvoice_Dtl
                        Query = "SELECT SeqNo,PurchId,PurchAmount, DPAmount, DPAmountDeduct, DPAmountOutstanding,PurchPaidAmount, PurchAmountOutstanding,GRAmount,RTBAmount,RDNAmount,GRNettAmount,GR_DPAmount,PayableDPAmount,PayableAmount,InvoiceAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            APSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                            PurchId.Add(Dr["PurchId"].ToString());
                            PurchAmount.Add(decimal.Parse(Dr["PurchPaidAmount"].ToString()));
                            DPAmount.Add(decimal.Parse(Dr["DPAmount"].ToString()));
                            DPAmountDeduct.Add(decimal.Parse(Dr["DPAmountDeduct"].ToString()));
                            DPAmountOutstanding.Add(decimal.Parse(Dr["DPAmountOutstanding"].ToString()));

                            PurchPaidAmount.Add(decimal.Parse(Dr["PurchPaidAmount"].ToString()));
                            PurchAmountOutstanding.Add(decimal.Parse(Dr["PurchAmountOutstanding"].ToString()));
                            GRAmount.Add(decimal.Parse(Dr["GRAmount"].ToString()));
                            RTBAmount.Add(decimal.Parse(Dr["RTBAmount"].ToString()));
                            RDNAmount.Add(decimal.Parse(Dr["RDNAmount"].ToString()));

                            GRNettAmount.Add(decimal.Parse(Dr["GRNettAmount"].ToString()));
                            GR_DPAmount.Add(decimal.Parse(Dr["GR_DPAmount"].ToString()));
                            PayableDPAmount.Add(decimal.Parse(Dr["PayableDPAmount"].ToString()));
                            PayableAmount.Add(decimal.Parse(Dr["PayableAmount"].ToString()));
                            InvoiceAmount.Add(decimal.Parse(Dr["InvoiceAmount"].ToString()));
                        }
                        Dr.Close();
                        
                        //menghapus nilai2 pada tabel VendInvoice_Dtl sesuai dengan InvoiceId
                        Query = "Delete from VendInvoice_Dtl where InvoiceId='" + txtInvoiceId.Text.Trim() + "';";
                        if (Query != "")
                        {
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        Query = "Delete from tblAttachments where ReffTransID ='" + txtInvoiceId.Text.Trim() + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        AddDetailDgv(invoiceId);
                        SaveDgvAttachmentData();

                        //Save VendInvoice_Logs
                        Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + status + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                        SaveVendInvoiceLogs(invoiceId, "-", LogsStatusDesc, "E");

                        //createdBy Thaddaeus, 10 sept 2018, begin
                        Query = "DELETE FROM [VendInvoice_Dtl_PO_Dtl] WHERE [Invoice_Id] = @Invoice_Id ;";
                        using (Cmd = new SqlCommand(Query, Conn))
                        {
                            Cmd.Parameters.AddWithValue("@Invoice_Id",txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();
                        }
                        SaveDgvDetailItem(txtInvoiceId.Text);
                        //end=========================================

                        scope.Complete();
                        MessageBox.Show("Data  Invoice Id : " + txtInvoiceId.Text + " berhasil diupdate.");
                    }//tutup else
                    
                }
                GetDataHeader();

                //createdBy Thaddaeus, 10 sept 2018, begin
                Conn.Close();
                
                ModeBeforeEdit();
                //end ====================================

            }//tutup Try
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                //edited By Thaddaeus 10Sept2018
                //Conn.Close();
                Parent.RefreshGrid();
                //ModeBeforeEdit();
                //end======================
            }
        }

        private void SaveVendInvoiceLogs(string InvoiceID, string Deskripsi, string Status, string Action)
        {
            string Query = "INSERT INTO VendInvoice_Logs VALUES ";
            Query += "(@InvoiceID,@Deskripsi,@Status,@Action,@Login,getdate())";
            using (Cmd = new SqlCommand(Query, Conn)) 
            {
                Cmd.Parameters.Add("@InvoiceID", InvoiceID);
                Cmd.Parameters.Add("@Deskripsi", Deskripsi);
                Cmd.Parameters.Add("@Status", Status);
                Cmd.Parameters.Add("@Action", Action);
                Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        private void txtInvoiceAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtInvoicePPN.Text != "" || txtInvoicePPH.Text != "" || txtInvoiceAmount.Text != "")
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    HitungInvoiceAmountNett();
                }
            }
            else
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    txtAmountNett.Text = txtInvoiceAmount.Text;
                }
            }
        }

        private void txtInvoiceAmount_Leave(object sender, EventArgs e)
        {
            if (txtInvoicePPN.Text != "" || txtInvoicePPH.Text != "" || txtInvoiceAmount.Text != "")
            {
                HitungInvoiceAmountNett();
            }
            else
            {
                txtAmountNett.Text = txtInvoiceAmount.Text;
            }

            Double value;
            if (Double.TryParse(txtInvoiceAmount.Text, out value))
                txtInvoiceAmount.Text = value.ToString("N2");
            else
                txtInvoiceAmount.Text = "0.00";
        }

        private void txtInvoicePPN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (txtInvoiceAmount.Text != "")
                {
                    HitungInvoiceAmountNett();
                }
                else
                {
                    MessageBox.Show("Isi Nilai Invoice Amount Terlebih Dahulu.");
                    txtInvoiceAmount.Focus();
                }
            }
        }
        private void txtInvoicePPN_Leave(object sender, EventArgs e)
        {
            if (txtInvoiceAmount.Text != "")
            {
                HitungInvoiceAmountNett();
            }

            Double value;
            if (Double.TryParse(txtInvoicePPN.Text, out value))
                txtInvoicePPN.Text = value.ToString("N2");
            else
                txtInvoicePPN.Text = "0.00";
        }

        private void txtInvoicePPH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (txtInvoiceAmount.Text != "")
                {
                    HitungInvoiceAmountNett();
                }
                else
                {
                    MessageBox.Show("Isi Nilai Invoice Amount Terlebih Dahulu.");
                    txtInvoiceAmount.Focus();
                }
            }
        }

        private void txtInvoicePPH_Leave(object sender, EventArgs e)
        {
            if (txtInvoiceAmount.Text != "")
            {
                HitungInvoiceAmountNett();
            }

            Double value;
            if (Double.TryParse(txtInvoicePPH.Text, out value))
                txtInvoicePPH.Text = value.ToString("N2");
            else
                txtInvoicePPH.Text = "0.00";
        }

        private void txtInvoiceAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtInvoicePPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            string strOKChars = "0123456789.";
            if ((strOKChars.IndexOf(e.KeyChar.ToString()) == -1 && e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void txtInvoicePPH_KeyPress(object sender, KeyPressEventArgs e)
        {
            string strOKChars = "0123456789.";
            if ((strOKChars.IndexOf(e.KeyChar.ToString()) == -1 && e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            checkDAOPAO = "";
            this.Close();
        }

        private void buttonEditGeneralAccountPayable_Click(object sender, EventArgs e)
        {

            string Check = "";
            Conn = ConnectionString.GetConnection();

            //Cek apakah status sudah di update
            Query = "Select TransStatus from [dbo].[VendInvoiceH] where [InvoiceId]='" + txtInvoiceId.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Check = Cmd.ExecuteScalar().ToString();

            if (ControlMgr.GroupName != "AP Admin")
            {
                MessageBox.Show("Hanya AP Admin yang bisa melakukan edit");
                return;
            }

            if (Check == "01" || Check == "02" || Check == "05" || Check == "07")
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show("Invoice Id=" + txtInvoiceId.Text + ".\n" + "Tidak Dapat Diedit Karena Sudah Diproses.");
                Conn.Close();
                return;
            }

            //Cek apakah sudah di buat payment Voucher
        }

        private void buttonCancelGeneralAccountPayable_Click(object sender, EventArgs e)
        {
            ModeView();
        }

        private void btnAddAttachment_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            choofdlog.FilterIndex = 3;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                FileName = new List<string>();
                Extension = new List<string>();
                sSelectedFile = new List<string>();

                int i = 0;

                foreach (string file in choofdlog.FileNames)
                {
                    FileStream objFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    int filelength = Convert.ToInt32(objFileStream.Length);
                    byte[] data = new byte[filelength];

                    objFileStream.Read(data, 0, filelength);
                    objFileStream.Close();

                    string tempFullName = Path.GetFileName(file);
                    string[] tempSplit = tempFullName.Split('.');

                    FileName.Add(tempSplit[0]);
                    Extension.Add(tempSplit[tempSplit.Count() - 1]);
                    int filesize = filelength / 1024;
                    this.dgvAttachment.Rows.Add(FileName[i], Extension[i], filesize.ToString(), System.Text.Encoding.UTF8.GetString(data));
                    test.Add(data);

                    cell = new DataGridViewComboBoxCell();
                    cell.Items.Add("Select");
                    cell.Items.Add("Invoice Vendor");
                    cell.Items.Add("Tax Invoice");
                    cell.Items.Add("Tanda Terima");
                    cell.Value = "Select";
                    dgvAttachment.Rows[(dgvAttachment.Rows.Count - 1)].Cells["FileType"] = cell;
                    i++;
                }
            }
        }
        public void SaveDgvAttachmentData()
        {
            for (int i = 0; i < dgvAttachment.RowCount; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, filetype,fileSize, attachment) Values";
                Query += "( 'VendInvoiceH', @InvoiceId, @FileName, @ContentType, @Filetype, @FileSize ,@binaryValue);";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.Add("@InvoiceId", invoiceId);
                Cmd.Parameters.Add("@FileName", dgvAttachment.Rows[i].Cells["FileName"].Value.ToString());
                Cmd.Parameters.Add("@ContentType", dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString());
                Cmd.Parameters.Add("@Filetype", dgvAttachment.Rows[i].Cells["Filetype"].Value.ToString());
                Cmd.Parameters.Add("@FileSize", dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString());
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                if (dgvAttachment.CurrentRow.Index > -1)
                {
                    test.RemoveAt(dgvAttachment.CurrentRow.Index);
                    dgvAttachment.Rows.RemoveAt(dgvAttachment.CurrentRow.Index);
                }
            }
            else
            {
                MessageBox.Show("Silahkan Pilih Data Untuk Dihapus");
                return;
            }
        }

        private void btnDownloadFile_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                String fileid = dgvAttachment.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Id"].Value.ToString();
                String fileName = dgvAttachment.CurrentRow.Cells["FileName"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileName"].Value.ToString();
                String ContentType = dgvAttachment.CurrentRow.Cells["ContentType"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["ContentType"].Value.ToString();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ContentType;
                sfd.FileName = fileName + "." + ContentType;
                sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.AddExtension = true;

                if (ContentType == "pdf")
                {
                    sfd.FilterIndex = 1;
                }
                else if (ContentType == "txt")
                {
                    sfd.FilterIndex = 2;
                }
                else
                {
                    sfd.FilterIndex = 3;
                }

                if (String.IsNullOrEmpty(fileid))
                {
                    MessageBox.Show("File Tidak Ada Dalam Database / Belum Dimasukkan Ke Dalam Database.");
                    return;
                }

                Conn = ConnectionString.GetConnection();
                Query = "Select Attachment From tblAttachments Where Id = '" + fileid + "'";
                Cmd = new SqlCommand(Query, Conn);

                byte[] data = (byte[])Cmd.ExecuteScalar();

                if (sfd.ShowDialog() != DialogResult.Cancel)
                {
                    FileStream objFileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                    objFileStream.Write(data, 0, data.Length);
                    objFileStream.Close();
                    MessageBox.Show("Data tersimpan!");
                }
            }
            else
            {
                MessageBox.Show("Silahkan Pilih Data Untuk Didownload");
                return;
            }
        }

        private void btnApproved_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                Query = "Select TransStatus from [dbo].[VendInvoiceH] where [InvoiceId]='" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Check = Cmd.ExecuteScalar().ToString();

                if (Check != "01" && Check != "07" && Check != "02" && Check != "05" && Check != "06")
                {
                    MessageBox.Show("Invoice Id=" + txtInvoiceId.Text + ".\n" + "Tidak Dapat Diapprove Karena Sudah Diproses.");
                    Conn.Close();
                    return;
                }
              
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        DialogResult dr = MessageBox.Show("Invoice Id No = " + invoiceId + "\n" + "Apakah Data Diatas Akan Diapprove?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string status = "";
                            string LogStatusDesc = "";
                            string LogDescription = "";
                            if (ControlMgr.GroupName == "Tax Admin")
                            {
                                status = "06";
                                LogStatusDesc = "Approved by Tax";
                                LogDescription = "Status: 06. Approved By Tax Admin";
                            }
                            else if (ControlMgr.GroupName == "AP Manager")
                            {
                                status = "03";
                                LogStatusDesc = "Approved By AP Manager";
                                LogDescription = "Status: 03. Approved By AP Manager";
                            }
                            else
                            {
                                if (Check == "01" || Check == "07")
                                    MessageBox.Show("Harus diapprove oleh Tax Admin.");
                                else if (Check == "02" || Check == "05" || Check == "06")
                                    MessageBox.Show("Harus diapprove oleh AP Manager.");
                                return;
                            }
                            //update
                            Query = "UPDATE VendInvoiceH SET ";
                            Query += "TransStatus = @status, ";    //Approved By Tax Admin
                            Query += "Notes = @richTextBoxNotes, ";
                            Query += "ApprovedBy = @Login, ";
                            Query += "UpdatedDate = GETDATE(), ";
                            Query += "UpdatedBy = @Login ";
                            Query += "WHERE InvoiceId=@txtInvoiceId";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.Add("@status", status);
                            Cmd.Parameters.Add("@richTextBoxNotes", richTextBoxNotes.Text);
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();

                            //createdBy Thaddaeus, 26 sept 2018, begin
                            ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, status, "", txtInvoiceId.Text, "", "", "");
                            //end======================================

                            List<decimal> APSeqNo = new List<decimal>();
                            Query = "SELECT SeqNo FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                APSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                            }
                            Dr.Close();

                            for (int i = 0; i < APSeqNo.Count; i++)
                            {
                                Query = "INSERT INTO VendInvoice_LogTable (InvoiceDate,InvoiceNo, VendId, VendName, VendorInvoiceNo, TaxNum, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                                Query += "VALUES (@dtInvoiceDate,@txtInvoiceId,@txtVendAccount,@txtVendName,@txtVendInvoiceNumber, @APSeqNo, @status, @LogStatusDesc, @LogDescription, @Login, GETDATE()) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@dtInvoiceDate", dtInvoiceDate.Value);
                                Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                                Cmd.Parameters.Add("@txtVendAccount", txtVendAccount.Text);
                                Cmd.Parameters.Add("@txtVendName", txtVendName.Text);
                                Cmd.Parameters.Add("@txtVendInvoiceNumber", txtVendInvoiceNumber.Text);
                                Cmd.Parameters.Add("@APSeqNo", APSeqNo[i]);
                                Cmd.Parameters.Add("@status", status);
                                Cmd.Parameters.Add("@LogStatusDesc", LogStatusDesc);
                                Cmd.Parameters.Add("@LogDescription", LogDescription);
                                Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                                Cmd.ExecuteNonQuery();
                            }

                            //Save VendInvoice_Logs
                                //Get Status
                            Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + status + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                                //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(invoiceId, "-", LogsStatusDesc, LogsAction);

                            //Posting Journal
                            if (status == "03")
                            {
                                PostingJournal();
                            }

                            scope.Complete();
                            MessageBox.Show("Data InvoiceId : " + txtInvoiceId.Text + " Berhasil Diupdate ");
                          
                        }
                        else
                        {
                            return;
                        }
                    }
                    GetDataHeader();
                    ModeApprove();

                    Parent2.RefreshGrid();

                }
                catch (Exception Ex)
                {
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void PostingJournal()
        {
            //Begin
            //Updated by : Joshua
            //Updated date : 03 Aug 2018
            //Description : Create Journal

            string Jenis = "JN", Kode = "JN";
            string JournalID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);

            string JournalHID = "";
            if (cmbInvoiceType.SelectedItem.ToString().ToUpper() == "INVOICE")
            {
                JournalHID = "AP01";
            }
            else
            {
                JournalHID = "AP11";
            }

            //Begin Insert Header
            Query = "INSERT INTO [GLJournalH]([GLJournalHID], [JournalHID] ,[Referensi], [Posting], [Status],[CreatedDate],[CreatedBy]) ";
            Query += "VALUES ('" + JournalID + "', '" + JournalHID + "', '" + txtInvoiceId.Text + "', 0, 'Gunakan', GETDATE(), '" + ControlMgr.UserId + "') ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            //End Insert Header

            //Begin Insert Detail
            Query = "SELECT SeqNo, FQA_ID, FQA_Desc, Type FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int SeqNo = 1;
            while (Dr.Read())
            {
                decimal Amount = 0;
                if (cmbInvoiceType.SelectedItem.ToString().ToUpper() == "INVOICE")
                {
                    if (Convert.ToInt32(Dr["SeqNo"]) == 1)
                    {
                        Amount = Convert.ToDecimal(txtInvoiceAmount.Text);
                    }
                    if (Convert.ToInt32(Dr["SeqNo"]) == 2)
                    {
                        Amount = Convert.ToDecimal(txtInvoicePPNAmount.Text) + Convert.ToDecimal(txtInvoicePPHAmount.Text);
                    }
                    if (Convert.ToInt32(Dr["SeqNo"]) == 3)
                    {
                        Amount = Convert.ToDecimal(txtAmountAP.Text);
                    }
                }
                else
                {
                    if (Convert.ToInt32(Dr["SeqNo"]) == 1)
                    {
                        Amount = Convert.ToDecimal(txtAmountAP.Text);
                    }
                    if (Convert.ToInt32(Dr["SeqNo"]) == 2)
                    {
                        Amount = Convert.ToDecimal(txtInvoiceAmount.Text);
                    }
                    if (Convert.ToInt32(Dr["SeqNo"]) == 3)
                    {
                        Amount = Convert.ToDecimal(txtInvoicePPNAmount.Text) + Convert.ToDecimal(txtInvoicePPHAmount.Text);
                    }
                }

                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                SeqNo++;
            }
            Dr.Close();
            //End Insert Detail
            //End
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                Query = "Select TransStatus from [dbo].[VendInvoiceH] where [InvoiceId]='" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Check = Cmd.ExecuteScalar().ToString();

                if (Check != "01" && Check != "02" && Check != "05" && Check != "07")
                {
                    MessageBox.Show("Invoice Id=" + txtInvoiceId.Text + ".\n" + "Tidak Dapat Direject Karena Sudah Diproses.");
                    Conn.Close();
                    return;
                }

                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        //280518 edit reject
                        DialogResult dr = MessageBox.Show("Invoice Id No = " + invoiceId + "\n" + "Apakah Data Diatas Akan direject ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            if (ControlMgr.GroupName == "Tax Admin")
                            {
                                status = "08";
                                LogStatusDesc = "Not Approved by Tax";
                                LogDescription = "Status: 08. Not Approved By Tax Admin";
                            }
                            else if (ControlMgr.GroupName == "AP Manager")
                            {
                                status = "04";
                                LogStatusDesc = "Not Approved By AP Manager";
                                LogDescription = "Status: 04. Not Approved By AP Manager";
                            }

                            Query = "UPDATE VendInvoiceH SET ";
                            Query += "TransStatus = @status, ";    //Not Approved By AP Manager
                            Query += "Notes = @richTextBoxNotes, ";
                            Query += "ApprovedBy = @Login, ";
                            Query += "UpdatedDate = GETDATE(), ";
                            Query += "UpdatedBy = @Login ";
                            Query += "WHERE InvoiceId=@txtInvoiceId";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.Add("@status", status);
                            Cmd.Parameters.Add("@richTextBoxNotes", richTextBoxNotes.Text);
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();

                            //createdBy Thaddaeus, 26 sept 2018, begin
                            ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, status, "", txtInvoiceId.Text, "", "", "");
                            //end======================================

                            List<decimal> APSeqNo = new List<decimal>();
                            List<decimal> PurchAmount = new List<decimal>();
                            List<decimal> DPAmount = new List<decimal>();
                            List<decimal> DPAmountDeduct = new List<decimal>();
                            List<decimal> DPAmountOutstanding = new List<decimal>();

                            List<decimal> PurchPaidAmount = new List<decimal>();
                            List<decimal> PurchAmountOutstanding = new List<decimal>();
                            List<decimal> GRAmount = new List<decimal>();
                            List<decimal> RTBAmount = new List<decimal>();
                            List<decimal> RDNAmount = new List<decimal>();

                            List<decimal> GRNettAmount = new List<decimal>();
                            List<decimal> GR_DPAmount = new List<decimal>();
                            List<decimal> PayableDPAmount = new List<decimal>();
                            List<decimal> PayableAmount = new List<decimal>();
                            List<decimal> InvoiceAmount = new List<decimal>();

                            Query = "SELECT SeqNo,PurchAmount, DPAmount, DPAmountDeduct, DPAmountOutstanding,PurchPaidAmount, PurchAmountOutstanding,GRAmount,RTBAmount,RDNAmount,GRNettAmount,GR_DPAmount,PayableDPAmount,PayableAmount,InvoiceAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                APSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                                PurchAmount.Add(decimal.Parse(Dr["PurchAmount"].ToString()));
                                DPAmount.Add(decimal.Parse(Dr["DPAmount"].ToString()));
                                DPAmountDeduct.Add(decimal.Parse(Dr["DPAmountDeduct"].ToString()));
                                DPAmountOutstanding.Add(decimal.Parse(Dr["DPAmountOutstanding"].ToString()));

                                PurchPaidAmount.Add(decimal.Parse(Dr["PurchPaidAmount"].ToString()));
                                PurchAmountOutstanding.Add(decimal.Parse(Dr["PurchAmountOutstanding"].ToString()));
                                GRAmount.Add(decimal.Parse(Dr["GRAmount"].ToString()));
                                RTBAmount.Add(decimal.Parse(Dr["RTBAmount"].ToString()));
                                RDNAmount.Add(decimal.Parse(Dr["RDNAmount"].ToString()));

                                GRNettAmount.Add(decimal.Parse(Dr["GRNettAmount"].ToString()));
                                GR_DPAmount.Add(decimal.Parse(Dr["GR_DPAmount"].ToString()));
                                PayableDPAmount.Add(decimal.Parse(Dr["PayableDPAmount"].ToString()));
                                PayableAmount.Add(decimal.Parse(Dr["PayableAmount"].ToString()));
                                InvoiceAmount.Add(decimal.Parse(Dr["InvoiceAmount"].ToString()));
                            }
                            Dr.Close();

                            for (int i = 0; i < APSeqNo.Count; i++)
                            {
                                string get_PurchAmount = PurchAmount[i].ToString();
                                string get_DPAmount = DPAmount[i].ToString();
                                string get_DPAmountDeduct = DPAmountDeduct[i].ToString();
                                string get_DPAmountOutstanding = DPAmountOutstanding[i].ToString();
                                string get_PurchPaidAmount = PurchPaidAmount[i].ToString();
                                string get_PurchAmountOutstanding = PurchAmountOutstanding[i].ToString();

                                string get_GRAmount = GRAmount[i].ToString();
                                string get_RTBAmount = RTBAmount[i].ToString();
                                string get_RDNAmount = RDNAmount[i].ToString();
                                string get_GRNettAmount = GRNettAmount[i].ToString();
                                string get_GR_DPAmount = GR_DPAmount[i].ToString();

                                string get_PayableDPAmount = PayableDPAmount[i].ToString();
                                string get_PayableAmount = PayableAmount[i].ToString();
                                string get_InvoiceAmount = InvoiceAmount[i].ToString();

                                Query = "";
                                //Select Nilai yang ada di datagrid
                                Query = "SELECT PurchAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT DPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT DPAmountDeduct FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmountDeduct_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT DPAmountOutstanding FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmountOutstanding_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT PurchPaidAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchPaidAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT PurchAmountOutstanding FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchAmountOutstanding_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT GRAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GRAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT RTBAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string RTBAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT RDNAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string RDNAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT GRNettAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GRNettAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT GR_DPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GR_DPAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT PayableDPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PayableDPAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT PayableAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PayableAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT InvoiceAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string InvoiceAmount_Old = Cmd.ExecuteScalar().ToString();

                                decimal PurchAmount_New = decimal.Parse(PurchAmount_Old.ToString());
                                decimal DPAmount_New = decimal.Parse(DPAmount_Old.ToString()) - decimal.Parse(get_DPAmount.ToString());
                                decimal DPAmountDeduct_New = decimal.Parse(DPAmountDeduct_Old.ToString()) - decimal.Parse(get_DPAmountDeduct.ToString());
                                decimal DPAmountOutstanding_New = decimal.Parse(DPAmountOutstanding_Old.ToString()) - decimal.Parse(get_DPAmountOutstanding.ToString());
                                decimal PurchPaidAmount_New = decimal.Parse(PurchPaidAmount_Old.ToString()) - decimal.Parse(get_PurchPaidAmount.ToString());

                                decimal PurchAmountOutstanding_New = decimal.Parse(PurchAmountOutstanding_Old.ToString()) - decimal.Parse(get_PurchAmountOutstanding.ToString());
                                decimal GRAmount_New = decimal.Parse(GRAmount_Old.ToString()) - decimal.Parse(get_GRAmount.ToString());
                                decimal RTBAmount_New = decimal.Parse(RTBAmount_Old.ToString()) - decimal.Parse(get_RTBAmount.ToString());
                                decimal RDNAmount_New = decimal.Parse(RDNAmount_Old.ToString()) - decimal.Parse(get_RDNAmount.ToString());
                                decimal GRNettAmount_New = decimal.Parse(GRNettAmount_Old.ToString()) - decimal.Parse(get_GRNettAmount.ToString());

                                decimal GR_DPAmount_New = decimal.Parse(GR_DPAmount_Old.ToString()) - decimal.Parse(get_GR_DPAmount.ToString());
                                decimal PayableDPAmount_New = decimal.Parse(PayableDPAmount_Old.ToString()) - decimal.Parse(get_PayableDPAmount.ToString());
                                decimal PayableAmount_New = decimal.Parse(PayableAmount_Old.ToString()) - decimal.Parse(get_PayableAmount.ToString());
                                decimal InvoiceAmount_New = decimal.Parse(InvoiceAmount_Old.ToString()) - decimal.Parse(get_InvoiceAmount.ToString());

                                Query = "";
                                //update nilai yang telah di reject ke table VendInvoice_Dtl
                                Query = "UPDATE VendInvoice_Dtl SET ";
                                Query += "PurchAmount=@PurchAmount_New, ";
                                Query += "DPAmount=@DPAmount_New, ";
                                Query += "DPAmountDeduct=@DPAmountDeduct_New, ";
                                Query += "DPAmountOutstanding=@DPAmountOutstanding_New, ";
                                Query += "PurchPaidAmount=@PurchPaidAmount_New, ";

                                Query += "PurchAmountOutstanding=@PurchAmountOutstanding_New, ";
                                Query += "GRAmount=@GRAmount_New, ";
                                Query += "RTBAmount=@RTBAmount_New, ";
                                Query += "RDNAmount=@RDNAmount_New, ";
                                Query += "GRNettAmount=@GRNettAmount_New, ";

                                Query += "GR_DPAmount=@GR_DPAmount_New, ";
                                Query += "PayableDPAmount=@PayableDPAmount_New, ";
                                Query += "PayableAmount=@PayableAmount_New, ";
                                Query += "InvoiceAmount=@InvoiceAmount_New ";
                                Query += "WHERE InvoiceId=@txtInvoiceId and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@PurchAmount_New", PurchAmount_New);
                                Cmd.Parameters.Add("@DPAmount_New", DPAmount_New);
                                Cmd.Parameters.Add("@DPAmountDeduct_New", DPAmountDeduct_New);
                                Cmd.Parameters.Add("@DPAmountOutstanding_New", DPAmountOutstanding_New);
                                Cmd.Parameters.Add("@PurchPaidAmount_New", PurchPaidAmount_New);
                                Cmd.Parameters.Add("@PurchAmountOutstanding_New", PurchAmountOutstanding_New);
                                Cmd.Parameters.Add("@GRAmount_New", GRAmount_New);
                                Cmd.Parameters.Add("@RTBAmount_New", RTBAmount_New);
                                Cmd.Parameters.Add("@RDNAmount_New", RDNAmount_New);
                                Cmd.Parameters.Add("@GRNettAmount_New", GRNettAmount_New);
                                Cmd.Parameters.Add("@GR_DPAmount_New", GR_DPAmount_New);
                                Cmd.Parameters.Add("@PayableDPAmount_New", PayableDPAmount_New);
                                Cmd.Parameters.Add("@PayableAmount_New", PayableAmount_New);
                                Cmd.Parameters.Add("@InvoiceAmount_New", InvoiceAmount_New);
                                Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                                Cmd.ExecuteNonQuery();

                                Query = "";
                                //update nilai yang telah di reject ke table PurchAmountTable
                                Query = "UPDATE PurchAmountTable SET ";
                                Query += "PurchAmount=@PurchAmount_New, ";
                                Query += "DPAmount=@DPAmount_New, ";
                                Query += "DPAmountDeduct=@DPAmountDeduct_New, ";
                                Query += "DPAmountOutstanding=@DPAmountOutstanding_New, ";
                                Query += "PurchPaidAmount=@PurchPaidAmount_New, ";

                                Query += "PurchAmountOutstanding=@PurchAmountOutstanding_New, ";
                                Query += "GRAmount=@GRAmount_New, ";
                                Query += "RTBAmount=@RTBAmount_New, ";
                                Query += "RDNAmount=@RDNAmount_New, ";
                                Query += "GRNettAmount=@GRNettAmount_New, ";

                                Query += "GR_DPAmount=@GR_DPAmount_New, ";
                                Query += "PayableDPAmount=@PayableDPAmount_New, ";
                                Query += "PayableAmount=@PayableAmount_New ";
                                //Query += "InvoiceAmount='" + InvoiceAmount_New + "' ";
                                Query += "WHERE PurchId=@PONo ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@PurchAmount_New", PurchAmount_New);
                                Cmd.Parameters.Add("@DPAmount_New", DPAmount_New);
                                Cmd.Parameters.Add("@DPAmountDeduct_New", DPAmountDeduct_New);
                                Cmd.Parameters.Add("@DPAmountOutstanding_New", DPAmountOutstanding_New);
                                Cmd.Parameters.Add("@PurchPaidAmount_New", PurchPaidAmount_New);
                                Cmd.Parameters.Add("@PurchAmountOutstanding_New", PurchAmountOutstanding_New);
                                Cmd.Parameters.Add("@GRAmount_New", GRAmount_New);
                                Cmd.Parameters.Add("@RTBAmount_New", RTBAmount_New);
                                Cmd.Parameters.Add("@RDNAmount_New", RDNAmount_New);
                                Cmd.Parameters.Add("@GRNettAmount_New", GRNettAmount_New);
                                Cmd.Parameters.Add("@GR_DPAmount_New", GR_DPAmount_New);
                                Cmd.Parameters.Add("@PayableDPAmount_New", PayableDPAmount_New);
                                Cmd.Parameters.Add("@PayableAmount_New", PayableAmount_New);
                                Cmd.Parameters.Add("@PONo", dgvDetail.Rows[i].Cells["PONo"].Value);
                                Cmd.ExecuteNonQuery();

                                //insert ke VendInvoice_LogTable dari hasil reject
                                Query = "INSERT INTO VendInvoice_LogTable (InvoiceDate,InvoiceNo, VendId, VendName, VendorInvoiceNo, TaxNum, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                                Query += "VALUES (@dtInvoiceDate, @txtInvoiceId, @txtVendAccount, @txtVendName, @txtVendInvoiceNumber, @APSeqNo, @status, @LogStatusDesc , @LogDescription, @Login , GETDATE())";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@dtInvoiceDate", dtInvoiceDate.Value);
                                Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                                Cmd.Parameters.Add("@txtVendAccount", txtVendAccount.Text);
                                Cmd.Parameters.Add("@txtVendName", txtVendName.Text);
                                Cmd.Parameters.Add("@txtVendInvoiceNumber", txtVendInvoiceNumber.Text);
                                Cmd.Parameters.Add("@APSeqNo", APSeqNo[i]);
                                Cmd.Parameters.Add("@status", status);
                                Cmd.Parameters.Add("@LogStatusDesc", LogStatusDesc);
                                Cmd.Parameters.Add("@LogDescription", LogDescription);
                                Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                                Cmd.ExecuteNonQuery();

                                //Save VendInvoice_Logs
                                    //Get Status
                                Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + status + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                                    //Get Last Action
                                Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                                Cmd = new SqlCommand(Query, Conn);
                                string LogsAction = Cmd.ExecuteScalar().ToString();
                                SaveVendInvoiceLogs(invoiceId, "-", LogsStatusDesc, LogsAction);
                            }
                            scope.Complete();
                            MessageBox.Show("Data InvoiceId : " + txtInvoiceId.Text + " Berhasil Diupdate ");
                            //GetDataHeader();
                            //ModeApprove();
                            
                            //Parent2.RefreshGrid();
                        }
                        else
                        {
                            return;
                        }
                    }
                    GetDataHeader();
                    ModeApprove();

                    Parent2.RefreshGrid();
                }
                catch (Exception Ex)
                {
                    return;
                }
                finally
                {
                    Conn.Close();
                    Parent2.RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnRevisi_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                Query = "Select TransStatus from [dbo].[VendInvoiceH] where [InvoiceId]='" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Check = Cmd.ExecuteScalar().ToString();

                if (Check != "01" && Check != "02" && Check != "05" && Check != "07")
                {
                    MessageBox.Show("Invoice Id=" + txtInvoiceId.Text + ".\n" + "Tidak Dapat Direvisi Karena Sudah Diproses.");
                    Conn.Close();
                    return;
                }

                try
                {
                    //DialogResult dr = MessageBox.Show("Invoice Id No = " + invoiceId + "\n" + "Apakah data diatas akan direvisi ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    //if (dr == DialogResult.Yes)
                    //{
                    //    string status = "";
                    //    string LogStatusDesc = "";
                    //    string LogDescription = "";
                    //    if (ControlMgr.GroupName == "Tax Admin")
                    //    {
                    //        status = "07";
                    //        LogStatusDesc = "Revision Needed By Tax";
                    //        LogDescription = "Status: 07. Revision Needed By Tax";
                    //    }
                    //    else if (ControlMgr.GroupName == "AP Manager")
                    //    {
                    //        status = "05";
                    //        LogStatusDesc = "Revision Needed By AP Manager";
                    //        LogDescription = "Status: 05. Revision Needed By AP Manager";
                    //    }
                    //}else
                    //{
                    //  return
                    //}

                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();

                        if (ControlMgr.GroupName == "Tax Admin")
                        {
                            // Revision Needed by Tax Admin

                            Query = "UPDATE VendInvoiceH SET ";
                            Query += "TransStatus = '07', ";    //Revision
                            Query += "Notes = @richTextBoxNotes, ";
                            Query += "ApprovedBy = @Login, ";
                            Query += "UpdatedDate = GETDATE(), ";
                            Query += "UpdatedBy = @Login ";
                            Query += "WHERE InvoiceId= @txtInvoiceId";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.Add("@richTextBoxNotes", richTextBoxNotes.Text);
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();

                            //createdBy Thaddaeus, 26 sept 2018, begin
                            ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, "07", "", txtInvoiceId.Text, "", "", "");
                            //end======================================

                            List<decimal> APSeqNo = new List<decimal>();
                            Query = "SELECT SeqNo FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                APSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                            }
                            Dr.Close();

                            for (int i = 0; i < APSeqNo.Count; i++)
                            {
                                Query = "INSERT INTO VendInvoice_LogTable (InvoiceDate,InvoiceNo, VendId, VendName, VendorInvoiceNo, TaxNum, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                                Query += "VALUES ('" + dtInvoiceDate.Value + "','" + txtInvoiceId.Text + "', '" + txtVendAccount.Text + "', '" + txtVendName.Text + "', '" + txtVendInvoiceNumber.Text + "', '" + APSeqNo[i] + "', '07', 'Revision Needed By Tax', 'Status: 07. Revision Needed By Tax','" + ControlMgr.UserId + "',GETDATE()) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                            }

                            //Save VendInvoice_Logs
                                //Get Status
                            Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + status + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                                //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(invoiceId, "-", LogsStatusDesc, LogsAction);

                            scope.Complete();
                            MessageBox.Show("Data InvoiceId : " + txtInvoiceId.Text + " Berhasil Diupdate ");
                            //GetDataHeader();
                            //ModeApprove();
                            
                            //Parent2.RefreshGrid();
                        }
                        else if (ControlMgr.GroupName == "AP Manager")
                        {
                            // Revision Needed by Tax Admin
                            Query = "UPDATE VendInvoiceH SET ";
                            Query += "TransStatus = '05', ";    //Revision
                            Query += "Notes = @richTextBoxNotes, ";
                            Query += "ApprovedBy = @Login, ";
                            Query += "UpdatedDate = GETDATE(), ";
                            Query += "UpdatedBy = @Login ";
                            Query += "WHERE InvoiceId=@txtInvoiceId";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.Add("@richTextBoxNotes", richTextBoxNotes.Text);
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();

                            List<decimal> APSeqNo = new List<decimal>();
                            Query = "SELECT SeqNo FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                APSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                            }
                            Dr.Close();
                            //insert ke tabel log in
                            for (int i = 0; i < APSeqNo.Count; i++)
                            {
                                Query = "INSERT INTO VendInvoice_LogTable (InvoiceDate,InvoiceNo, VendId, VendName, VendorInvoiceNo, TaxNum, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                                Query += "VALUES (@dtInvoiceDate,@txtInvoiceId, @txtVendAccount, @txtVendName, @txtVendInvoiceNumber, @APSeqNo, '05', 'Revision Needed By AP Manager', 'Status: 05. Revision Needed By AP Manager', @Login, GETDATE()) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@dtInvoiceDate", dtInvoiceDate.Value);
                                Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                                Cmd.Parameters.Add("@txtVendAccount", txtVendAccount.Text);
                                Cmd.Parameters.Add("@txtVendName", txtVendName.Text);
                                Cmd.Parameters.Add("@txtVendInvoiceNumber", txtVendInvoiceNumber.Text);
                                Cmd.Parameters.Add("@APSeqNo", APSeqNo[i]);
                                Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                                Cmd.ExecuteNonQuery();
                            }

                            //Save VendInvoice_Logs
                                //Get Status
                            Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + status + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                                //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(invoiceId, "-", LogsStatusDesc, LogsAction);

                            scope.Complete();
                            MessageBox.Show("Data InvoiceId : " + txtInvoiceId.Text + " Berhasil Diupdate ");
                                                
                        }
                        else
                        {
                            MessageBox.Show(ControlMgr.PermissionDenied);
                        }
                       
                    }
                    GetDataHeader();
                    ModeApprove();

                    Parent2.RefreshGrid();
                }
                catch (Exception Ex)
                {
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnCancelApprove_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                Query = "Select TransStatus from [dbo].[VendInvoiceH] where [InvoiceId]='" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Check = Cmd.ExecuteScalar().ToString();

                if (Check != "03" && Check != "06")
                {
                    MessageBox.Show("Invoice Id=" + txtInvoiceId.Text + ".\n" + "Tidak dapat di Cancel karena belum diapprove.");
                    Conn.Close();
                    return;
                }

                if (Check == "04" && Check == "08" && Check == "05" && Check == "07")
                {
                    MessageBox.Show("Invoice Id=" + txtInvoiceId.Text + ".\n" + "Tidak dapat di cancel karena sudah direject / direvision.");
                    Conn.Close();
                    return;
                }

                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        //280518 edit cancel
                        DialogResult dr = MessageBox.Show("Invoice Id No = " + invoiceId + "\n" + "Apakah Data Diatas Akan diCancel ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            if (ControlMgr.GroupName == "Tax Admin")
                            {
                                status = "01";
                                LogStatusDesc = "Cancel Approved by Tax";
                                LogDescription = "Status: 02. Waiting Approval By Tax Admin";
                            }
                            else if (ControlMgr.GroupName == "AP Manager")
                            {
                                status = "02";
                                LogStatusDesc = "Cancel Approved by AP Manager";
                                LogDescription = "Status: 04. Waiting Approval By AP Manager";
                            }

                            Query = "UPDATE VendInvoiceH SET ";
                            Query += "TransStatus= @status, ";    //Not Approved By AP Manager
                            Query += "Notes = @richTextBoxNotes, ";
                            Query += "ApprovedBy = @Login, ";
                            Query += "UpdatedDate = GETDATE(), ";
                            Query += "UpdatedBy = @Login ";
                            Query += "WHERE InvoiceId= @InvoicedId";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.Add("@status", status);
                            Cmd.Parameters.Add("@richTextBoxNotes", richTextBoxNotes.Text);
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.Parameters.Add("@InvoicedId", txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();

                            //createdBy Thaddaeus, 26 sept 2018, begin
                            ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, status, LogStatusDesc, txtInvoiceId.Text, "", "", "");
                            //end======================================

                            List<decimal> APSeqNo = new List<decimal>();
                            List<decimal> PurchAmount = new List<decimal>();
                            List<decimal> DPAmount = new List<decimal>();
                            List<decimal> DPAmountDeduct = new List<decimal>();
                            List<decimal> DPAmountOutstanding = new List<decimal>();

                            List<decimal> PurchPaidAmount = new List<decimal>();
                            List<decimal> PurchAmountOutstanding = new List<decimal>();
                            List<decimal> GRAmount = new List<decimal>();
                            List<decimal> RTBAmount = new List<decimal>();
                            List<decimal> RDNAmount = new List<decimal>();

                            List<decimal> GRNettAmount = new List<decimal>();
                            List<decimal> GR_DPAmount = new List<decimal>();
                            List<decimal> PayableDPAmount = new List<decimal>();
                            List<decimal> PayableAmount = new List<decimal>();
                            List<decimal> InvoiceAmount = new List<decimal>();

                            Query = "SELECT SeqNo,PurchAmount, DPAmount, DPAmountDeduct, DPAmountOutstanding,PurchPaidAmount, PurchAmountOutstanding,GRAmount,RTBAmount,RDNAmount,GRNettAmount,GR_DPAmount,PayableDPAmount,PayableAmount,InvoiceAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                APSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                                PurchAmount.Add(decimal.Parse(Dr["PurchAmount"].ToString()));
                                DPAmount.Add(decimal.Parse(Dr["DPAmount"].ToString()));
                                DPAmountDeduct.Add(decimal.Parse(Dr["DPAmountDeduct"].ToString()));
                                DPAmountOutstanding.Add(decimal.Parse(Dr["DPAmountOutstanding"].ToString()));

                                PurchPaidAmount.Add(decimal.Parse(Dr["PurchPaidAmount"].ToString()));
                                PurchAmountOutstanding.Add(decimal.Parse(Dr["PurchAmountOutstanding"].ToString()));
                                GRAmount.Add(decimal.Parse(Dr["GRAmount"].ToString()));
                                RTBAmount.Add(decimal.Parse(Dr["RTBAmount"].ToString()));
                                RDNAmount.Add(decimal.Parse(Dr["RDNAmount"].ToString()));

                                GRNettAmount.Add(decimal.Parse(Dr["GRNettAmount"].ToString()));
                                GR_DPAmount.Add(decimal.Parse(Dr["GR_DPAmount"].ToString()));
                                PayableDPAmount.Add(decimal.Parse(Dr["PayableDPAmount"].ToString()));
                                PayableAmount.Add(decimal.Parse(Dr["PayableAmount"].ToString()));
                                InvoiceAmount.Add(decimal.Parse(Dr["InvoiceAmount"].ToString()));
                            }
                            Dr.Close();

                            for (int i = 0; i < APSeqNo.Count; i++)
                            {
                                string get_PurchAmount = PurchAmount[i].ToString();
                                string get_DPAmount = DPAmount[i].ToString();
                                string get_DPAmountDeduct = DPAmountDeduct[i].ToString();
                                string get_DPAmountOutstanding = DPAmountOutstanding[i].ToString();
                                string get_PurchPaidAmount = PurchPaidAmount[i].ToString();
                                string get_PurchAmountOutstanding = PurchAmountOutstanding[i].ToString();

                                string get_GRAmount = GRAmount[i].ToString();
                                string get_RTBAmount = RTBAmount[i].ToString();
                                string get_RDNAmount = RDNAmount[i].ToString();
                                string get_GRNettAmount = GRNettAmount[i].ToString();
                                string get_GR_DPAmount = GR_DPAmount[i].ToString();

                                string get_PayableDPAmount = PayableDPAmount[i].ToString();
                                string get_PayableAmount = PayableAmount[i].ToString();
                                string get_InvoiceAmount = InvoiceAmount[i].ToString();

                                Query = "";
                                //Select Nilai yang ada di datagrid
                                Query = "SELECT PurchAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT DPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT DPAmountDeduct FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmountDeduct_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT DPAmountOutstanding FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmountOutstanding_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT PurchPaidAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchPaidAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT PurchAmountOutstanding FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchAmountOutstanding_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT GRAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GRAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT RTBAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string RTBAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT RDNAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string RDNAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT GRNettAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GRNettAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT GR_DPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GR_DPAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT PayableDPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PayableDPAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT PayableAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PayableAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT InvoiceAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetail.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string InvoiceAmount_Old = Cmd.ExecuteScalar().ToString();

                                decimal PurchAmount_New = decimal.Parse(PurchAmount_Old.ToString());
                                decimal DPAmount_New = decimal.Parse(DPAmount_Old.ToString()) - decimal.Parse(get_DPAmount.ToString());
                                decimal DPAmountDeduct_New = decimal.Parse(DPAmountDeduct_Old.ToString()) - decimal.Parse(get_DPAmountDeduct.ToString());
                                decimal DPAmountOutstanding_New = decimal.Parse(DPAmountOutstanding_Old.ToString()) - decimal.Parse(get_DPAmountOutstanding.ToString());
                                decimal PurchPaidAmount_New = decimal.Parse(PurchPaidAmount_Old.ToString()) - decimal.Parse(get_PurchPaidAmount.ToString());

                                decimal PurchAmountOutstanding_New = decimal.Parse(PurchAmountOutstanding_Old.ToString()) - decimal.Parse(get_PurchAmountOutstanding.ToString());
                                decimal GRAmount_New = decimal.Parse(GRAmount_Old.ToString()) - decimal.Parse(get_GRAmount.ToString());
                                decimal RTBAmount_New = decimal.Parse(RTBAmount_Old.ToString()) - decimal.Parse(get_RTBAmount.ToString());
                                decimal RDNAmount_New = decimal.Parse(RDNAmount_Old.ToString()) - decimal.Parse(get_RDNAmount.ToString());
                                decimal GRNettAmount_New = decimal.Parse(GRNettAmount_Old.ToString()) - decimal.Parse(get_GRNettAmount.ToString());

                                decimal GR_DPAmount_New = decimal.Parse(GR_DPAmount_Old.ToString()) - decimal.Parse(get_GR_DPAmount.ToString());
                                decimal PayableDPAmount_New = decimal.Parse(PayableDPAmount_Old.ToString()) - decimal.Parse(get_PayableDPAmount.ToString());
                                decimal PayableAmount_New = decimal.Parse(PayableAmount_Old.ToString()) - decimal.Parse(get_PayableAmount.ToString());
                                decimal InvoiceAmount_New = decimal.Parse(InvoiceAmount_Old.ToString()) - decimal.Parse(get_InvoiceAmount.ToString());

                                Query = "";
                                //update nilai yang telah di reject ke table VendInvoice_Dtl
                                Query = "UPDATE VendInvoice_Dtl SET ";
                                Query += "PurchAmount=@PurchAmount_New, ";
                                Query += "DPAmount=@DPAmount_New, ";
                                Query += "DPAmountDeduct=@DPAmountDeduct_New, ";
                                Query += "DPAmountOutstanding=@DPAmountOutstanding_New, ";
                                Query += "PurchPaidAmount=@PurchPaidAmount_New, ";

                                Query += "PurchAmountOutstanding=@PurchAmountOutstanding_New, ";
                                Query += "GRAmount=@GRAmount_New, ";
                                Query += "RTBAmount=@RTBAmount_New, ";
                                Query += "RDNAmount=@RDNAmount_New, ";
                                Query += "GRNettAmount=@GRNettAmount_New, ";

                                Query += "GR_DPAmount=@GR_DPAmount_New, ";
                                Query += "PayableDPAmount=@PayableDPAmount_New, ";
                                Query += "PayableAmount=@PayableAmount_New, ";
                                Query += "InvoiceAmount=@InvoiceAmount_New ";
                                Query += "WHERE InvoiceId=@txtInvoiceId and PurchId=@PONo ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@PurchAmount_New", PurchAmount_New);
                                Cmd.Parameters.Add("@DPAmount_New", DPAmount_New);
                                Cmd.Parameters.Add("@DPAmountDeduct_New", DPAmountDeduct_New);
                                Cmd.Parameters.Add("@DPAmountOutstanding_New", DPAmountOutstanding_New);
                                Cmd.Parameters.Add("@PurchPaidAmount_New", PurchPaidAmount_New);
                                Cmd.Parameters.Add("@PurchAmountOutstanding_New", PurchAmountOutstanding_New);
                                Cmd.Parameters.Add("@GRAmount_New", GRAmount_New);
                                Cmd.Parameters.Add("@RTBAmount_New", RTBAmount_New);
                                Cmd.Parameters.Add("@RDNAmount_New", RDNAmount_New);
                                Cmd.Parameters.Add("@GRNettAmount_New", GRNettAmount_New);
                                Cmd.Parameters.Add("@GR_DPAmount_New", GR_DPAmount_New);
                                Cmd.Parameters.Add("@PayableDPAmount_New", PayableDPAmount_New);
                                Cmd.Parameters.Add("@PayableAmount_New", PayableAmount_New);
                                Cmd.Parameters.Add("@InvoiceAmount_New", InvoiceAmount_New);
                                Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                                Cmd.Parameters.Add("@PONo", dgvDetail.Rows[i].Cells["PONo"].Value);
                                Cmd.ExecuteNonQuery();

                                Query = "";
                                //update nilai yang telah di reject ke table PurchAmountTable
                                Query = "UPDATE PurchAmountTable SET ";
                                Query += "PurchAmount=@PurchAmount_New, ";
                                Query += "DPAmount=@DPAmount_New, ";
                                Query += "DPAmountDeduct=@DPAmountDeduct_New, ";
                                Query += "DPAmountOutstanding=@DPAmountOutstanding_New, ";
                                Query += "PurchPaidAmount=@PurchPaidAmount_New, ";

                                Query += "PurchAmountOutstanding=@PurchAmountOutstanding_New, ";
                                Query += "GRAmount=@GRAmount_New, ";
                                Query += "RTBAmount=@RTBAmount_New, ";
                                Query += "RDNAmount=@RDNAmount_New, ";
                                Query += "GRNettAmount=@GRNettAmount_New, ";

                                Query += "GR_DPAmount=@GR_DPAmount_New, ";
                                Query += "PayableDPAmount=@PayableDPAmount_New, ";
                                Query += "PayableAmount=@PayableAmount_New ";
                                //Query += "InvoiceAmount='" + InvoiceAmount_New + "' ";
                                Query += "WHERE PurchId=@PONo ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@PurchAmount_New", PurchAmount_New);
                                Cmd.Parameters.Add("@DPAmount_New", DPAmount_New);
                                Cmd.Parameters.Add("@DPAmountDeduct_New", DPAmountDeduct_New);
                                Cmd.Parameters.Add("@DPAmountOutstanding_New", DPAmountOutstanding_New);
                                Cmd.Parameters.Add("@PurchPaidAmount_New", PurchPaidAmount_New);
                                Cmd.Parameters.Add("@PurchAmountOutstanding_New", PurchAmountOutstanding_New);
                                Cmd.Parameters.Add("@GRAmount_New", GRAmount_New);
                                Cmd.Parameters.Add("@RTBAmount_New", RTBAmount_New);
                                Cmd.Parameters.Add("@RDNAmount_New", RDNAmount_New);
                                Cmd.Parameters.Add("@GRNettAmount_New", GRNettAmount_New);
                                Cmd.Parameters.Add("@GR_DPAmount_New", GR_DPAmount_New);
                                Cmd.Parameters.Add("@PayableDPAmount_New", PayableDPAmount_New);
                                Cmd.Parameters.Add("@PayableAmount_New", PayableAmount_New);
                                Cmd.Parameters.Add("@PONo", dgvDetail.Rows[i].Cells["PONo"].Value);
                                Cmd.ExecuteNonQuery();

                                //insert ke VendInvoice_LogTable dari hasil reject
                                Query = "INSERT INTO VendInvoice_LogTable (InvoiceDate,InvoiceNo, VendId, VendName, VendorInvoiceNo, TaxNum, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                                Query += "VALUES (@dtInvoiceDate, @txtInvoiceId, @txtVendAccount, @txtVendName, @txtVendInvoiceNumber, @APSeqNo, @status, @LogStatusDesc , @LogDescription, @Login, GETDATE())";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@dtInvoiceDate", dtInvoiceDate.Value);
                                Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                                Cmd.Parameters.Add("@txtVendAccount", txtVendAccount.Text);
                                Cmd.Parameters.Add("@txtVendName", txtVendName.Text);
                                Cmd.Parameters.Add("@txtVendInvoiceNumber", txtVendInvoiceNumber.Text);
                                Cmd.Parameters.Add("@APSeqNo", APSeqNo[i]);
                                Cmd.Parameters.Add("@status", status);
                                Cmd.Parameters.Add("@LogStatusDesc", LogStatusDesc);
                                Cmd.Parameters.Add("@LogDescription", LogDescription);
                                Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                                Cmd.ExecuteNonQuery();
                            }

                            //Save VendInvoice_Logs
                                //Get Status
                            Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + status + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                                //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(invoiceId, "-", LogsStatusDesc, LogsAction);

                            //Cancel Journal
                            if (status == "02")
                            {
                                UnPostingJournal();
                            }

                            MessageBox.Show("Data InvoiceId : " + txtInvoiceId.Text + " Berhasil Diupdate ");
                           
                            GetDataHeader();
                            ModeApprove();

                            Parent2.RefreshGrid();
                        }
                        else
                        {
                            return;
                        }
                        scope.Complete();
                    }
                }
                catch (Exception Ex)
                {
                    return;
                }
                finally
                {
                    Conn.Close();
                    Parent2.RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void UnPostingJournal()
        {
            //Begin
            //Updated by : Joshua
            //Updated date : 04 Aug 2018
            //Description : Cancel Journal
            //Query = "SELECT PostingDate FROM GLJournalH WHERE Referensi = '" + txtInvoiceId.Text + "' AND UPPER(Status) = 'GUNAKAN' AND Posting = 0";
            //Cmd = new SqlCommand(Query, Conn);
            //string PostingDate = Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString();
            //if (PostingDate == null || PostingDate == "1900-01-01" || PostingDate == "")
            //{
            //    Query = "UPDATE GLJournalH SET Status = 'Batal' WHERE Referensi = '" + txtInvoiceId.Text + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Cmd.ExecuteNonQuery();
            //}
            //else
            //{
            //    MessageBox.Show("Invoice Id=" + txtInvoiceId.Text + ".\n" + "Tidak Dapat di Cancel karena sudah di proses.");
            //    return;
            //}
            //End Insert Detail
            //End

            string Jenis = "JN", Kode = "JN";
            string JournalID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);

            string JournalHID = "";
            if (cmbInvoiceType.SelectedItem.ToString().ToUpper() == "INVOICE")
            {
                JournalHID = "AP01R";
            }
            else
            {
                JournalHID = "AP11R";
            }

            //Begin Insert Header
            Query = "INSERT INTO [GLJournalH]([GLJournalHID], [JournalHID] ,[Referensi], [Posting], [Status],[CreatedDate],[CreatedBy]) ";
            Query += "VALUES ('" + JournalID + "', '" + JournalHID + "', '" + txtInvoiceId.Text + "', 0, 'Gunakan', GETDATE(), '" + ControlMgr.UserId + "') ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            //End Insert Header

            //Begin Insert Detail
            Query = "SELECT SeqNo, FQA_ID, FQA_Desc, Type FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int SeqNo = 1;
            while (Dr.Read())
            {
                decimal Amount = 0;
                if (cmbInvoiceType.SelectedItem.ToString().ToUpper() == "INVOICE")
                {
                    if (Convert.ToInt32(Dr["SeqNo"]) == 1)
                    {
                        Amount = Convert.ToDecimal(txtInvoiceAmount.Text);
                    }
                    if (Convert.ToInt32(Dr["SeqNo"]) == 2)
                    {
                        Amount = Convert.ToDecimal(txtInvoicePPNAmount.Text) + Convert.ToDecimal(txtInvoicePPHAmount.Text);
                    }
                    if (Convert.ToInt32(Dr["SeqNo"]) == 3)
                    {
                        Amount = Convert.ToDecimal(txtAmountAP.Text);
                    }
                }
                else
                {
                    if (Convert.ToInt32(Dr["SeqNo"]) == 1)
                    {
                        Amount = Convert.ToDecimal(txtAmountAP.Text);
                    }
                    if (Convert.ToInt32(Dr["SeqNo"]) == 2)
                    {
                        Amount = Convert.ToDecimal(txtInvoiceAmount.Text);
                    }
                    if (Convert.ToInt32(Dr["SeqNo"]) == 3)
                    {
                        Amount = Convert.ToDecimal(txtInvoicePPNAmount.Text) + Convert.ToDecimal(txtInvoicePPHAmount.Text);
                    }
                }

                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                SeqNo++;
            }
            Dr.Close();
        }

        private void dgvDetail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvDetail.Columns[dgvDetail.CurrentCell.ColumnIndex].Name == "InvoiceAmount")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

            }
        }

        private void dgvDetail_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvDetail_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvDetail_KeyPress);
            }
        }

        private void dtVendInvoiceDueDate_ValueChanged(object sender, EventArgs e)
        {
            if (Mode == "Edit")
            {
                using (Conn = ConnectionString.GetConnection())
                {
                    Cmd = new SqlCommand("SELECT DueDate from [dbo].[VendInvoiceH] where invoiceid='" + txtInvoiceId.Text + "'", Conn);
                    if (Cmd.ExecuteScalar() != null)
                    {
                        DateTime Date1 = Convert.ToDateTime(Cmd.ExecuteScalar());
                        if (Date1 < dtVendInvoiceDueDate.Value)
                        {
                            MessageBox.Show("Due Date tidak boleh lebih besar dari sebelumnya.");
                            dtVendInvoiceDueDate.Value = Date1;
                        }
                    }
                }
            }
        }

        private void dgvDetail_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.ToString() != "")
                {
                    if (dgvDetail.Columns.Contains("PODate"))
                    {
                        if ((
                            e.ColumnIndex == dgvDetail.Columns["PODate"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["PODueDate"].Index
                            ) && e.Value != null)
                        {
                            try
                            {
                                DateTime d = DateTime.Parse(e.Value.ToString());
                                e.Value = d.ToString("dd/MM/yyyy");
                            }
                            catch
                            {
                                string d = e.Value.ToString();
                                e.Value = d;
                            }
                        }
                    }

                    if (cmbInvoiceType.Text == "Uang Muka")
                    {
                        if ((
                            e.ColumnIndex == dgvDetail.Columns["DPAmount"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["POAmount"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["DPDeduct"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["DPOutstanding"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["POInvoiced"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["POUnInvoice"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["PayableAmount"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["InvoiceAmount"].Index 
                            ) && e.Value != null)
                        {
                            double d = double.Parse(e.Value.ToString());
                            e.Value = d.ToString("N2");
                        }
                    }
                    else if (cmbInvoiceType.Text == "Invoice")
                    {
                        if ((e.ColumnIndex == dgvDetail.Columns["POAmount"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["DPAmount"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["DPOutstanding"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["POInvoiced"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["POUnInvoice"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["GRAmount"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["PotDP"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["GRPayable"].Index ||
                            e.ColumnIndex == dgvDetail.Columns["InvoiceAmount"].Index
                            ) && e.Value != null)
                        {
                         
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                        
                        }
                    }
                }
            }
        }

        private void dgvRetur_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRetur.Columns[e.ColumnIndex].Name.ToString() == "Check")
            {
                decimal TmpNilai = 0;
                for (int i = 0; i < dgvRetur.RowCount; i++)
                {
                    if (Convert.ToBoolean(dgvRetur.Rows[i].Cells["Check"].Value) == true)
                        TmpNilai += Convert.ToDecimal(dgvRetur.Rows[i].Cells["TotalRetur"].Value);
                }
                txtAmountRetur.Text = TmpNilai.ToString("N2");
                txtAmountPay.Text = (Convert.ToDecimal(txtAmountPay.Text) - TmpNilai).ToString("N2");
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvRetur.Columns.Count != 0)
            {
                if (tabControl1.SelectedTab.Text == "Detail Retur")
                {
                    dgvRetur.AutoResizeColumns();
                }
            }
        }

        private void cmbInvoiceType_Click(object sender, EventArgs e)
        {
            if(dgvDetail.RowCount > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Detail sudah terisi. Apakah akan dikosongkan ? ", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvDetail.Rows.Clear();
                    dgvRetur.Rows.Clear();
                    dgvDetailItem.Rows.Clear();
                }
                else if (dialogResult == DialogResult.No)
                {
                    dgvDetail.Focus();
                }
            }
        }

        private void ValidasiAmount()
        {
            if (dgvDetail.RowCount > 0)
            {
                if (dgvDetail.Columns[dgvDetail.CurrentCell.ColumnIndex].Name == "InvoiceAmount")
                {
                    decimal TmpTotalInvAmount = 0;
                    for (int i = 0; i < dgvDetail.RowCount; i++)
                    {
                        TmpTotalInvAmount += Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value);
                    }

                    if (TmpTotalInvAmount > Convert.ToDecimal(txtAmountNett.Text)) 
                    {
                        MessageBox.Show("Nilai tidak boleh melebihi Invoice Amount.");
                        dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value = (Convert.ToDecimal(txtAmountNett.Text) - (TmpTotalInvAmount - Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value)));
                        txtInvoiceAmount.Focus();
                    }

                    if (cmbInvoiceType.Text == "Invoice")
                    {
                        if (Mode != "Edit")
                        {
                            if (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value) > Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["POUnInvoice"].Value))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi Purchase Invoice.");
                                dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value = dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["POUnInvoice"].Value;
                            }
                            if (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value) > Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["GRPayable"].Value))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi Purchase Invoice.");
                                dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value = dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["GRPayable"].Value;
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value) > (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["POUnInvoice"].Value) + Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value)))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi Purchase Invoice.");
                                dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value = Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["POUnInvoice"].Value) + Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value);
                            }
                            if (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value) > (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["GRPayable"].Value) + Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value)))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi Purchase Invoice.");
                                dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value = Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["GRPayable"].Value) + Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value);
                            }
                        }
                    }

                    if (cmbInvoiceType.Text == "Uang Muka")
                    {
                        if (Mode != "Edit")
                        {
                            if (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value) > Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["PayableAmount"].Value))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi PayableAmount.");
                                dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value = dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["PayableAmount"].Value;
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value) > (Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["POUnInvoice"].Value) + Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value)))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi POUnInvoice.");
                                dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value = Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["POUnInvoice"].Value) + Convert.ToDecimal(dgvDetail.Rows[dgvDetail.CurrentCell.RowIndex].Cells["InvoiceAmount"].Value);
                            }
                        }
                    }
                }
                decimal TmpTotalInvAmount1 =0;
                for (int i = 0; i < dgvDetail.RowCount; i++)
                {
                    TmpTotalInvAmount1 += Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value);
                }
                txtAmountAP.Text = TmpTotalInvAmount1.ToString("N2");
                txtAmountPay.Text = (Convert.ToDecimal(txtAmountPay.Text) - Convert.ToDecimal(txtAmountRetur.Text)).ToString("N2");
            }
        }

        private void HitungInvoiceAmount()
        {
            decimal TmpTotalInvAmount = 0;
            decimal TmpTotalRetur = 0;
            for (int i = 0; i < dgvDetail.RowCount; i++)
            {
                TmpTotalInvAmount += Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value);
            }
            for (int i = 0; i < dgvRetur.RowCount; i++)
            {
                if(dgvRetur.Rows[i].Cells["TotalRetur"].Value.ToString() != "" && Convert.ToBoolean(dgvRetur.Rows[i].Cells["Check"].Value) == true)
                    TmpTotalRetur += Convert.ToDecimal(dgvRetur.Rows[i].Cells["TotalRetur"].Value);
            }
            txtAmountPay.Text = (TmpTotalInvAmount - TmpTotalRetur).ToString("N2");
        }

        private void dgvDetail_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ValidasiAmount();
            HitungInvoiceAmount();
        }

        private void txtInvoicePPNAmount_TextChanged(object sender, EventArgs e)
        {
            Double value;
            if (Double.TryParse(txtInvoicePPN.Text, out value))
                txtInvoicePPN.Text = value.ToString("N2");
            else
                txtInvoicePPN.Text = "0.00";
        }

        private void txtInvoicePPHAmount_TextChanged(object sender, EventArgs e)
        {
            Double value;
            if (Double.TryParse(txtInvoicePPH.Text, out value))
                txtInvoicePPH.Text = value.ToString("N2");
            else
                txtInvoicePPH.Text = "0.00";
        }

        private void txtAmountNett_TextChanged(object sender, EventArgs e)
        {
            Double value;
            if (Double.TryParse(txtAmountNett.Text, out value))
                txtAmountNett.Text = value.ToString("N2");
            else
                txtAmountNett.Text = "0.00";
        }

        private void txtAmountPay_TextChanged(object sender, EventArgs e)
        {
            Double value;
            if (Double.TryParse(txtAmountNett.Text, out value))
                txtAmountNett.Text = value.ToString("N2");
            else
                txtAmountNett.Text = "0.00";
        }

        private void dgvDetail_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            UpdateAmount();
        }

        public void UpdateAmount()
        {
            using (Conn = ConnectionString.GetConnection())
            {
                for (int i = 0; i < dgvDetail.RowCount; i++)
                {
                    if (cmbInvoiceType.Text == "Invoice")
                    {
                        Query = "SELECT DPAmount, DPOutstanding, POInvoiced, POUnInvoice from [dbo].[PO_UnInvoiced_GR_View] a where PONo='" + dgvDetail.Rows[i].Cells["PONo"].Value.ToString() + "'";
                    }
                    else if (cmbInvoiceType.Text == "Uang Muka")
                    {
                        Query = "SELECT DPAmount, DPDeduct, DPOutstanding, POInvoiced, POUnInvoice from [dbo].[PO_UnInvoiced_DP_View] a where PONo='" + dgvDetail.Rows[i].Cells["PONo"].Value.ToString() + "'";
                    }

                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (cmbInvoiceType.Text == "Invoice")
                        {
                            dgvDetail.Rows[i].Cells["DPAmount"].Value = Dr["DPAmount"].ToString();
                            dgvDetail.Rows[i].Cells["DPOutstanding"].Value = Dr["DPOutstanding"].ToString();
                            dgvDetail.Rows[i].Cells["POInvoiced"].Value = Dr["POInvoiced"].ToString();
                            dgvDetail.Rows[i].Cells["POUnInvoice"].Value = Dr["POUnInvoice"].ToString();
                        }
                        else if (cmbInvoiceType.Text == "Uang Muka")
                        {
                            dgvDetail.Rows[i].Cells["DPAmount"].Value = Dr["DPAmount"].ToString();
                            dgvDetail.Rows[i].Cells["DPDeduct"].Value = Dr["DPDeduct"].ToString();
                            dgvDetail.Rows[i].Cells["DPOutstanding"].Value = Dr["DPOutstanding"].ToString();
                            dgvDetail.Rows[i].Cells["POInvoiced"].Value = Dr["POInvoiced"].ToString();
                            dgvDetail.Rows[i].Cells["POUnInvoice"].Value = Dr["POUnInvoice"].ToString();
                        }
                    }
                    Dr.Close();
                }
                dgvRetur.AutoResizeColumns();
            }
        }

        public void CountAmountAP()
        {
            decimal TmpAmount = 0;
            for (int i = 0; i < dgvDetail.RowCount; i++)
            { 
                TmpAmount += Convert.ToDecimal(dgvDetail.Rows[i].Cells["InvoiceAmount"].Value);
            }
            txtAmountAP.Text = TmpAmount.ToString("N2");
        }

        private void dgvRetur_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRetur.RowCount > 0)
            {
                if (e.Value != null)
                {
                    if (e.Value.ToString() != "")
                    {
                        if ((
                            e.ColumnIndex == dgvRetur.Columns["NRBDate"].Index ||
                            e.ColumnIndex == dgvRetur.Columns["GRDate"].Index ||
                            e.ColumnIndex == dgvRetur.Columns["PODate"].Index
                            ) && e.Value != null)
                        {
                            DateTime d = DateTime.Parse(e.Value.ToString());
                            e.Value = d.ToString("dd/MM/yyyy");
                        }
                      
                        if ((e.ColumnIndex == dgvRetur.Columns["TotalRetur"].Index) && e.Value != null)
                        {
                            double d = double.Parse(e.Value.ToString());
                            e.Value = d.ToString("N2");
                        }
                    }
                }
            }
        }
        //tia edit
        //klik kanan
        Purchase.NotaReturBeli.ReturBeliHeader NRBId = null;
        PopUp.Vendor.Vendor VendId = null;
        AccountPayable.Payment_Voucher.PaymentVoucher ParentForPaymentVoucher;
        AccountPayable.Payment_Voucher.PaymentVoucherGiro ParentForPaymentVoucherGiro;
        Purchase.PurchaseOrderNew.POForm PONumber = null;
        ISBS_New.Purchase.GoodsReceipt.GRHeaderV2 Gr = null;

        PopUp.FullItemId.FullItemId FID = null;
        public static string itemID;

        public string ItemID { get { return itemID; } set { itemID = value; } }

        public void ParentRefreshGrid(AccountPayable.Payment_Voucher.PaymentVoucher PV)
        {
            ParentForPaymentVoucher = PV;
        }

        public void ParentRefreshGrid(AccountPayable.Payment_Voucher.PaymentVoucherGiro PV)
        {
            ParentForPaymentVoucherGiro = PV;
        }

        private bool CheckOpened(string name)
        {
            // FormCollection FC = Application.OpenForms;
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
     
        private void dgvDetail_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //msh belum bener
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (PONumber == null || PONumber.Text == "")
                {
                    if (dgvDetail.Columns[e.ColumnIndex].Name.ToString() == "PONo")
                    {
                        PONumber = new Purchase.PurchaseOrderNew.POForm();
                        PONumber.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["PONo"].Value.ToString(), "");
                        PONumber.ParentRefreshGrid4(this);
                        PONumber.Show();
                    }
                }
                else if (CheckOpened(PONumber.Name))
                {
                    PONumber.WindowState = FormWindowState.Normal;
                    PONumber.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["PONo"].Value.ToString(), "");
                    PONumber.ParentRefreshGrid4(this);
                    PONumber.Show();
                    PONumber.Focus();
                }
                // NRBNo

                if (NRBId == null || NRBId.Text == "")
                {
                    if (dgvDetail.Columns[e.ColumnIndex].Name.ToString() == "NRBNo")
                    {
                        NRBId = new Purchase.NotaReturBeli.ReturBeliHeader();
                        NRBId.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["NRBNo"].Value.ToString());
                        NRBId.ParentRefreshGrid2(this);
                        NRBId.Show();
                    }
                }
                else if (CheckOpened(NRBId.Name))
                {
                    NRBId.WindowState = FormWindowState.Normal;
                    NRBId.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["NRBNo"].Value.ToString());
                    NRBId.ParentRefreshGrid2(this);
                    NRBId.Show();
                    NRBId.Focus();
                }
                //gr
               
                if (Gr == null || Gr.Text == "")
                {
                     if (dgvDetail.Columns[e.ColumnIndex].Name.ToString() == "GRNo")
                     {
                        Gr = new Purchase.GoodsReceipt.GRHeaderV2("Receipt Order");
                        Gr.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["GRNo"].Value.ToString());
                        Gr.ParentRefreshGrid5(this);
                        Gr.Show();

                     }
                }
                else if (CheckOpened(Gr.Name))
                {
                    Gr.WindowState = FormWindowState.Normal;
                    Gr.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["GRNo"].Value.ToString());
                    Gr.ParentRefreshGrid5(this);
                    Gr.Show();
                    Gr.Focus();
                }
            }
            
        }

        private void dgvRetur_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                // NRBNo
                //msh blm bener nrb
                if (NRBId == null || NRBId.Text == "")
                {
                    if (dgvRetur.Columns[e.ColumnIndex].Name.ToString() == "NRBNo")
                    {
                        NRBId = new Purchase.NotaReturBeli.ReturBeliHeader();
                        NRBId.SetMode("PopUp", dgvRetur.Rows[e.RowIndex].Cells["NRBNo"].Value.ToString());
                        NRBId.ParentRefreshGrid2(this);
                        NRBId.Show();
                    }
                }
                else if (CheckOpened(NRBId.Name))
                {
                    NRBId.WindowState = FormWindowState.Normal;
                    NRBId.SetMode("PopUp", dgvRetur.Rows[e.RowIndex].Cells["NRBNo"].Value.ToString());
                    NRBId.ParentRefreshGrid2(this);
                    NRBId.Show();
                    NRBId.Focus();
                }
                //po
                if (PONumber == null || PONumber.Text == "")
                {
                    if (dgvRetur.Columns[e.ColumnIndex].Name.ToString() == "PONo")
                    {
                        PONumber = new Purchase.PurchaseOrderNew.POForm();
                        PONumber.SetMode("PopUp", dgvRetur.Rows[e.RowIndex].Cells["PONo"].Value.ToString(), "");
                        PONumber.ParentRefreshGrid4(this);
                        PONumber.Show();
                    }
                }
                else if (CheckOpened(PONumber.Name))
                {
                    PONumber.WindowState = FormWindowState.Normal;
                    PONumber.SetMode("PopUp", dgvRetur.Rows[e.RowIndex].Cells["PONo"].Value.ToString(), "");
                    PONumber.ParentRefreshGrid4(this);
                    PONumber.Show();
                    PONumber.Focus();
                }
                //gr
                if (Gr == null || Gr.Text == "")
                {
                    if (dgvRetur.Columns[e.ColumnIndex].Name.ToString() == "GRNo")
                    {
                        Gr = new Purchase.GoodsReceipt.GRHeaderV2("Receipt Order");
                        Gr.SetMode("PopUp", dgvRetur.Rows[e.RowIndex].Cells["GRNo"].Value.ToString());
                        Gr.ParentRefreshGrid5(this);
                        Gr.Show();

                    }
                }
                else if (CheckOpened(Gr.Name))
                {
                    Gr.WindowState = FormWindowState.Normal;
                    Gr.SetMode("PopUp", dgvRetur.Rows[e.RowIndex].Cells["GRNo"].Value.ToString());
                    Gr.Show();
                    Gr.Focus();
                }
            }
        }

        private void txtVendAccount_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (VendId == null || VendId.Text == "")
                {
                    txtVendAccount.Enabled = true;
                    VendId = new PopUp.Vendor.Vendor();
                    VendId.GetData(txtVendAccount.Text);
                    VendId.Show();
                }
                else if (CheckOpened(VendId.Name))
                {
                    VendId.WindowState = FormWindowState.Normal;
                    VendId.Show();
                    VendId.Focus();
                }
            }
        }

        private void txtVendName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (VendId == null || VendId.Text == "")
                {
                    txtVendName.Enabled = true;
                    VendId = new PopUp.Vendor.Vendor();
                    VendId.GetData(txtVendAccount.Text);
                    VendId.Show();
                }
                else if (CheckOpened(VendId.Name))
                {
                    VendId.WindowState = FormWindowState.Normal;
                    VendId.Show();
                    VendId.Focus();
                }
            }
        }

        private void dgvDetailItem_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                //po
                if (PONumber == null || PONumber.Text == "")
                {
                    if (dgvDetailItem.Columns[e.ColumnIndex].Name.ToString() == "PO No")
                    {
                        PONumber = new Purchase.PurchaseOrderNew.POForm();
                        PONumber.SetMode("PopUp", dgvDetailItem.Rows[e.RowIndex].Cells["PO No"].Value.ToString(), "");
                        PONumber.ParentRefreshGrid4(this);
                        PONumber.Show();
                    }
                }
                else if (CheckOpened(PONumber.Name))
                {
                    PONumber.WindowState = FormWindowState.Normal;
                    PONumber.SetMode("PopUp", dgvDetailItem.Rows[e.RowIndex].Cells["PO No"].Value.ToString(), "");
                    PONumber.ParentRefreshGrid4(this);
                    PONumber.Show();
                    PONumber.Focus();
                }
                //gr
                if (Gr == null || Gr.Text == "")
                {
                    if (dgvDetailItem.Columns[e.ColumnIndex].Name.ToString() == "GR No")
                    {
                        Gr = new Purchase.GoodsReceipt.GRHeaderV2("Receipt Order");
                        Gr.SetMode("PopUp", dgvDetailItem.Rows[e.RowIndex].Cells["GR No"].Value.ToString());
                        Gr.ParentRefreshGrid5(this);
                        Gr.Show();

                    }
                }
                else if (CheckOpened(Gr.Name))
                {
                    Gr.WindowState = FormWindowState.Normal;
                    Gr.SetMode("PopUp", dgvDetailItem.Rows[e.RowIndex].Cells["GR No"].Value.ToString());
                    Gr.Show();
                    Gr.Focus();
                }
                //fullitemid
                if (FID == null || FID.Text == "")
                {
                    if (dgvDetailItem.Columns[e.ColumnIndex].Name.ToString() == "Item Name" || dgvDetailItem.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvDetailItem.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                        itemID = dgvDetailItem.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvDetailItem.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }

            }

        }

        //tia edit end

        //Created by Thaddaeus, 6 Sept2018,Begin
        private void RefreshDetailItem()
        {
            if (dgvDetail.RowCount <= 0)
            {
                dgvDetailItem.Columns.Clear();
                return;
            }
            if (!(dgvDetail.Columns.Contains("GRNo")))
            {
                return;
            }
            dgvDetailItem.Columns.Clear();
            string GR_IDs = "";
            string[] HeadersName = new string[] { "Invoice_Id", "SeqNo", "PO_No", "GR_No", "FullItemId", "Item_Name", "GR_Qty", "Retur_Qty", "Qty", "Unit", "Price", "Ratio", "Total", "Total_Disc", "PPN_Percent", "Line_Amount", "Line_Tax_Base_Amount", "Line_Tax_Amount", "PO_SeqNo", "GR_SeqNo", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy", "Disc_Allocate" };
            string[] HeaderText = new string[] { "Invoice Id", "SeqNo", "PO No", "GR No", "FullItemId", "Item Name", "GR Qty", "Retur Qty", "Qty", "Unit", "Price", "Ratio", "Total", "Total Disc", "PPN Percent", "Line Amount", "Line Tax Base Amount", "Line Tax Amount", "PO SeqNo", "GR SeqNo", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy", "Disc Allocate" };
            string[] ReadOnly = new string[] { "Invoice Id", "SeqNo", "PO No", "GR No", "FullItemId", "Item Name", "GR Qty", "Retur Qty", "Qty", "Unit", "Price", "Ratio", "Total", "Total Disc", "PPN Percent", "Line Amount", "Line Tax Base Amount", "Line Tax Amount", "PO SeqNo", "GR SeqNo", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            string[] VisibleTrue = new string[] { "SeqNo", "PO No", "GR No", "FullItemId", "Item Name", "GR Qty", "Retur Qty", "Qty", "Unit", "Price", "Total", "PPN Percent", "Disc Allocate" };
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                if (dgvDetail.Rows[i].Cells["GRNo"].Value.ToString() != "")
                {
                    GR_IDs += "'" + dgvDetail.Rows[i].Cells["GRNo"].Value.ToString() + "',";
                }
            }
            if (Mode == "New")
            {
                string ReffType = "";
                string POID = "";
                int POSeqNo = 0;
                for (int i = 0; i < dgvDetail.Rows.Count; i++)
                {
                    Query = "SELECT a.[RefTransType],b.[RefTransID],b.[RefTransSeqNo] FROM [GoodsReceivedH] a LEFT JOIN [GoodsReceivedD] b ON b.[GoodsReceivedId]=a.[GoodsReceivedId] WHERE b.[GoodsReceivedId]=@GoodsReceivedId ;";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.Parameters.AddWithValue("@GoodsReceivedId", dgvDetail.Rows[i].Cells["GRNo"].Value.ToString());
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                if (Dr["RefTransID"] != System.DBNull.Value && Dr["RefTransSeqNo"] != System.DBNull.Value)
                                {
                                    ReffType = Dr["RefTransType"].ToString();
                                    POID = Dr["RefTransID"].ToString();
                                    POSeqNo = Convert.ToInt32(Dr["RefTransSeqNo"]);
                                }
                            }
                        }
                        Dr.Close();
                    }

                    if (ReffType == "Receipt Order")
                    {
                        Query = "SELECT [PurchaseOrderId],[PurchaseOrderSeqNo] FROM [ReceiptOrderD] WHERE [ReceiptOrderId] = @ReceiptOrderId AND [SeqNo] = @SeqNo";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@ReceiptOrderId",POID);
                            Cmd.Parameters.AddWithValue("@SeqNo",POSeqNo);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    if (Dr["PurchaseOrderId"] != System.DBNull.Value && Dr["PurchaseOrderSeqNo"] != System.DBNull.Value)
                                    {
                                        POID = Dr["PurchaseOrderId"].ToString();
                                        POSeqNo = Convert.ToInt32(Dr["PurchaseOrderSeqNo"]);
                                    }
                                }
                            }
                            Dr.Close();
                        }
                    }
                    else if (ReffType == "Nota Retur Beli")
                    {
                        SearchPOID(POID, POSeqNo);
                        POID = POId;
                        POSeqNo = POSeqno;
                    }

                    Query = "SELECT @Invoice_Id AS 'Invoice Id',0 AS 'SeqNo',@PO_NO AS 'PO No',[GoodsReceivedId] AS 'GR No',[FullItemId],[ItemName] AS 'Item Name',[Qty_Actual] AS 'GR Qty',0 AS 'Retur Qty',[Qty],[Unit],[Price],[Ratio],ISNULL(Total,0) AS 'Total' ";
                    Query += ",[Total_Discount] as 'Total Disc',[PPN] as 'PPN Percent',0 AS 'Line Amount', 0 AS 'Line Tax Base Amount', 0 AS 'Line Tax Amount' ";
                    Query += ",@PO_SeqNo as 'PO SeqNo',[GoodsReceivedSeqNo] as 'GR SeqNo','' as 'CreatedDate','' as 'CreatedBy','' as 'UpdatedDate','' as 'UpdatedBy','0.00' as 'Disc Allocate' FROM [GoodsReceivedD] WHERE GoodsReceivedId = @GoodsReceivedId;";
                    using (Da = new SqlDataAdapter(Query, ConnectionString.GetConnection()))
                    {
                        Da.SelectCommand.Parameters.AddWithValue("@Invoice_Id",txtInvoiceId.Text);
                        Da.SelectCommand.Parameters.AddWithValue("@PO_NO",POID);
                        Da.SelectCommand.Parameters.AddWithValue("@PO_SeqNo",POSeqNo);
                        Da.SelectCommand.Parameters.AddWithValue("@GoodsReceivedId", dgvDetail.Rows[i].Cells["GRNo"].Value.ToString());
                        if (dgvDetailItem.Rows.Count > 0)
                        {
                            DataTable newRow = new DataTable();
                            Da.Fill(newRow);
                            Dt.Merge(newRow, true);
                            dgvDetailItem.Refresh();
                        }
                        else
                        {
                            Dt = new DataTable();
                            Da.Fill(Dt);
                            dgvDetailItem.DataSource = Dt;
                        }
                    }
                }
                for (int i = 0; i < dgvDetailItem.Rows.Count; i++)
                {
                    dgvDetailItem.Rows[i].Cells["SeqNo"].Value = i + 1;
                }
            }
            else if(txtInvoiceId.Text != "")
            {
                Query = "SELECT  ";
                for (int i = 0; i < HeadersName.Count(); i++)
                {
                    if (HeadersName[i] == "Total" || HeadersName[i] == "Disc_Allocate")
                    {
                        //Query += "(ISNULL(" + HeadersName[i] + ",'0.00')) as '" + HeaderText[i] + "'";
                        Query += "CONVERT(varchar,(ISNULL(" + HeadersName[i] + ",'0.00'))) as '" + HeaderText[i] + "'";
                    }
                    else
                    {
                        Query += "" + HeadersName[i] + " as '" + HeaderText[i] + "'";
                    }

                    if ((i + 1) == HeadersName.Count())
                    {
                        Query += " ";
                    }
                    else
                    {
                        Query += ",";
                    }
                }
                Query += " FROM [VendInvoice_Dtl_PO_Dtl] WHERE [Invoice_Id] =@Invoice_Id;";
                using (Da = new SqlDataAdapter(Query, ConnectionString.GetConnection()))
                {
                    Da.SelectCommand.Parameters.AddWithValue("@Invoice_Id", txtInvoiceId.Text);
                    Dt = new DataTable();
                    Da.Fill(Dt);
                    dgvDetailItem.DataSource = Dt;
                }
            }

            foreach (string name in HeaderText)
            {
                dgvDetailItem.Columns[name].Visible = false;
            }
            foreach (string name in VisibleTrue)
            {
                if (HeaderText.Contains(name))
                {
                    dgvDetailItem.Columns[name].Visible = true;
                }
            }
            if (Mode == "New" || Mode == "Edit")
            {
                foreach (string name in ReadOnly)
                {
                    if (HeaderText.Contains(name))
                    {
                        dgvDetailItem.Columns[name].ReadOnly = true;
                        dgvDetailItem.Columns[name].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
            }
            else
            {
                foreach (string name in HeaderText)
                {
                    dgvDetailItem.Columns[name].ReadOnly = true;
                    dgvDetailItem.Columns[name].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
            dgvDetailItem.AutoResizeColumns();
         
        }

        private void SaveDgvDetailItem(string InvoiceId)
        {
            if (invoiceId == "")
            {
                return;
            }
            if (dgvDetailItem.Rows.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < dgvDetailItem.Rows.Count; i++)
            {
                Query = "INSERT INTO [VendInvoice_Dtl_PO_Dtl] ([Invoice_Id],[SeqNo],[PO_No],[GR_No],[FullItemId],[Item_Name],[GR_Qty],[Retur_Qty],[Qty],[Unit],[Price],[Ratio],[Total],[Total_Disc],[PPN_Percent],[Line_Amount],[Line_Tax_Base_Amount],[Line_Tax_Amount],[PO_SeqNo],[GR_SeqNo],[CreatedDate],[CreatedBy],[Disc_Allocate]) VALUES ";
                Query += "(@Invoice_Id,@SeqNo,@PO_No,@GR_No,@FullItemId,@Item_Name,@GR_Qty,@Retur_Qty,@Qty,@Unit,@Price,@Ratio,@Total,@Total_Disc,@PPN_Percent,@Line_Amount,@Line_Tax_Base_Amount,@Line_Tax_Amount,@PO_SeqNo,@GR_SeqNo,getdate(),@CreatedBy,@Disc_Allocate)";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@Invoice_Id",InvoiceId);
                    Cmd.Parameters.AddWithValue("@SeqNo",(i+1));
                    Cmd.Parameters.AddWithValue("@PO_No",dgvDetailItem.Rows[i].Cells["PO No"].Value);
                    Cmd.Parameters.AddWithValue("@GR_No",dgvDetailItem.Rows[i].Cells["GR No"].Value);
                    Cmd.Parameters.AddWithValue("@FullItemId",dgvDetailItem.Rows[i].Cells["FullItemId"].Value);
                    Cmd.Parameters.AddWithValue("@Item_Name",dgvDetailItem.Rows[i].Cells["Item Name"].Value);
                    Cmd.Parameters.AddWithValue("@GR_Qty",dgvDetailItem.Rows[i].Cells["GR Qty"].Value);
                    Cmd.Parameters.AddWithValue("@Retur_Qty", dgvDetailItem.Rows[i].Cells["Retur Qty"].Value);
                    Cmd.Parameters.AddWithValue("@Qty", dgvDetailItem.Rows[i].Cells["Qty"].Value);
                    Cmd.Parameters.AddWithValue("@Unit", dgvDetailItem.Rows[i].Cells["Unit"].Value);
                    Cmd.Parameters.AddWithValue("@Price", dgvDetailItem.Rows[i].Cells["Price"].Value);
                    Cmd.Parameters.AddWithValue("@Ratio", dgvDetailItem.Rows[i].Cells["Ratio"].Value);
                    Cmd.Parameters.AddWithValue("@Total", dgvDetailItem.Rows[i].Cells["Total"].Value);
                    Cmd.Parameters.AddWithValue("@Total_Disc", dgvDetailItem.Rows[i].Cells["Total Disc"].Value);
                    Cmd.Parameters.AddWithValue("@PPN_Percent", dgvDetailItem.Rows[i].Cells["PPN Percent"].Value);
                    decimal LineTaxBaseAmount = Convert.ToDecimal(dgvDetailItem.Rows[i].Cells["Total"].Value) - Convert.ToDecimal(dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value);
                    decimal LineTaxAmount = 0;
                    if (dgvDetailItem.Rows[i].Cells["PPN Percent"].Value != System.DBNull.Value && dgvDetailItem.Rows[i].Cells["PPN Percent"].Value != String.Empty)
                    {
                        LineTaxAmount = Convert.ToDecimal(dgvDetailItem.Rows[i].Cells["PPN Percent"].Value) * LineTaxBaseAmount / 100;
                    }
                    Cmd.Parameters.AddWithValue("@Line_Amount", (LineTaxBaseAmount + LineTaxAmount));
                    Cmd.Parameters.AddWithValue("@Line_Tax_Base_Amount",LineTaxBaseAmount);
                    Cmd.Parameters.AddWithValue("@Line_Tax_Amount",LineTaxAmount);
                    Cmd.Parameters.AddWithValue("@PO_SeqNo", dgvDetailItem.Rows[i].Cells["PO SeqNo"].Value);
                    Cmd.Parameters.AddWithValue("@GR_SeqNo", dgvDetailItem.Rows[i].Cells["GR SeqNo"].Value);
                    Cmd.Parameters.AddWithValue("@Disc_Allocate", dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value);
                    Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void dgvDetailItem_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl tb = new DataGridViewTextBoxEditingControl();
            tb.KeyPress += new KeyPressEventHandler(dgvDetailItem_KeyPress);
            e.Control.KeyPress += new KeyPressEventHandler(dgvDetailItem_KeyPress);
        }

        private void dgvDetailItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvDetailItem.Rows.Count > 0)
            {
                if (dgvDetailItem.Columns[dgvDetailItem.CurrentCell.ColumnIndex].Name == "Disc Allocate")
                {
                    if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.'))
                    {
                        e.Handled = true;
                    }
                    if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    {
                        e.Handled = true;
                    }

                }

            }
        }

        private void dgvDetailItem_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            decimal discamount = 0;
            for (int i = 0; i < dgvDetailItem.Rows.Count; i++)
            {
                if (dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value.ToString() == "")
                {
                    dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value = 0;
                }
                if (Convert.ToDecimal(dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value) > Convert.ToDecimal(dgvDetailItem.Rows[i].Cells["Total"].Value))
                {
                    dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value = "0.00";
                    MessageBox.Show("Discount Amount tidak boleh melebihi "+dgvDetailItem.Rows[i].Cells["Total"].Value.ToString()+" (Total Amount item).");
                    return;
                }
                discamount += Convert.ToDecimal(dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value);
                dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value = String.Format("{0:#,##0.###0}", Convert.ToDecimal(dgvDetailItem.Rows[i].Cells["Disc Allocate"].Value));
            }
            if (discamount > Convert.ToDecimal(txtDiscountAmt.Text))
            {
                MessageBox.Show("Discount Amount yang digunakan melebihi Discount Amount batas.");
                dgvDetailItem.Rows[dgvDetailItem.CurrentRow.Index].Cells["Disc Allocate"].Value = String.Format("{0:#,##0.###0}", 0);
            }
        }

        private void SearchPOID(string Reffid1, int ReffSeqNo1)
        {
            string ReffType = "Nota Retur Beli";
            string ReffId = Reffid1;
            int ReffSeqNo = ReffSeqNo1;
            //loop till get a POID or dead-end, GR > NRB > GR > NRB.... till GR>RO>PO, or till blank/null value get;
            while (ReffType == "Nota Retur Beli")
            {
                //Search in NRB to get GR No
                Query = "SELECT [GoodsReceivedId],[GoodsReceived_SeqNo] FROM [ISBS-NEW4].[dbo].[NotaReturBeli_Dtl] WHERE [NRBId]=@NRBId AND [SeqNo]=@SeqNo;";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@NRBId", ReffId);
                    Cmd.Parameters.AddWithValue("@SeqNo", ReffSeqNo);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            if (Dr["GoodsReceivedId"] != System.DBNull.Value && Dr["GoodsReceived_SeqNo"] != System.DBNull.Value)
                            {
                                ReffType = "GR";
                                ReffId = Dr["GoodsReceivedId"].ToString();
                                ReffSeqNo = Convert.ToInt32(Dr["GoodsReceived_SeqNo"]);
                            }
                        }
                    }
                    Dr.Close();
                }
                //if the REF GR no in the previous NRB didnt contain any GR, then return no po_id and 0 seqno
                if ((ReffId == Reffid1) && (ReffSeqNo == ReffSeqNo1))
                {
                    ReffType = "";
                    POId = "";
                    POSeqno = 0;
                }
                //if the ref gr no in the previous NRB contain a GR no, then search to GR again
                else if (ReffType == "GR")
                {
                    Query = "SELECT a.[RefTransType],b.[RefTransID],b.[RefTransSeqNo] FROM [GoodsReceivedH] a LEFT JOIN [GoodsReceivedD] b ON b.[GoodsReceivedId]=a.[GoodsReceivedId] WHERE b.[GoodsReceivedId]=@GoodsReceivedId AND b.[GoodsReceivedSeqNo]=@GoodsReceivedSeqNo;";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.Parameters.AddWithValue("@GoodsReceivedId", ReffId);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", ReffSeqNo);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                if (Dr["GoodsReceivedId"] != System.DBNull.Value && Dr["GoodsReceived_SeqNo"] != System.DBNull.Value)
                                {
                                    ReffType = Dr["RefTransType"].ToString();
                                    ReffId = Dr["GoodsReceivedId"].ToString();
                                    ReffSeqNo = Convert.ToInt32(Dr["GoodsReceived_SeqNo"]);
                                }
                            }
                        }
                        Dr.Close();
                    }
                    if (ReffType == "Receipt Order")
                    {
                        Query = "SELECT [PurchaseOrderId],[PurchaseOrderSeqNo] FROM [ReceiptOrderD] WHERE [ReceiptOrderId] = @ReceiptOrderId AND [SeqNo] = @SeqNo";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@ReceiptOrderId", ReffId);
                            Cmd.Parameters.AddWithValue("@SeqNo", ReffSeqNo);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    if (Dr["PurchaseOrderId"] != System.DBNull.Value && Dr["PurchaseOrderSeqNo"] != System.DBNull.Value)
                                    {
                                        ReffId = Dr["PurchaseOrderId"].ToString();
                                        ReffSeqNo = Convert.ToInt32(Dr["PurchaseOrderSeqNo"]);
                                        POId = ReffId;
                                        POSeqno= ReffSeqNo;
                                    }
                                }
                            }
                            Dr.Close();
                        }
                    }
                }
                else
                {
                    ReffType = "";
                    POId = "";
                    POSeqno = 0;
                }
            }
        }

        private void dgvDetailItem_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.ToString() != "")
                {
                    //if (cmbInvoiceType.Text == "Uang Muka")
                    //{
                    //    if ((e.ColumnIndex == dgvDetail.Columns["POAmount"].Index ||
                    //        e.ColumnIndex == dgvDetail.Columns["DPDeduct"].Index ||
                    //        e.ColumnIndex == dgvDetail.Columns["DPOutstanding"].Index ||
                    //        e.ColumnIndex == dgvDetail.Columns["POInvoiced"].Index ||
                    //        e.ColumnIndex == dgvDetail.Columns["POUnInvoice"].Index ||
                    //        e.ColumnIndex == dgvDetail.Columns["PayableAmount"].Index ||
                    //        e.ColumnIndex == dgvDetail.Columns["InvoiceAmount"].Index
                    //        ) && e.Value != null)
                    //    {
                    //        double d = double.Parse(e.Value.ToString());
                    //        e.Value = d.ToString("N2");
                    //    }
                    //}
                    //if (cmbInvoiceType.Text == "Invoice")
                    //{
                        if ((e.ColumnIndex == dgvDetailItem.Columns["GR Qty"].Index ||
                           e.ColumnIndex == dgvDetailItem.Columns["Retur Qty"].Index ||
                           e.ColumnIndex == dgvDetailItem.Columns["Qty"].Index ||
                           e.ColumnIndex == dgvDetailItem.Columns["Price"].Index ||
                           e.ColumnIndex == dgvDetailItem.Columns["Total"].Index ||
                           e.ColumnIndex == dgvDetailItem.Columns["PPN Percent"].Index ||
                           e.ColumnIndex == dgvDetailItem.Columns["Disc Allocate"].Index
                           ) && e.Value != null)
                        {
                            double d = double.Parse(e.Value.ToString());
                            e.Value = d.ToString("N2");
                        }
                    //}
                }
            }
        }

        private void txtDiscountAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtDiscountAmt_Leave(object sender, EventArgs e)
        {
            Double value;
            if (Double.TryParse(txtDiscountAmt.Text, out value))
                txtDiscountAmt.Text = value.ToString("N4");
            else
                txtDiscountAmt.Text = "0.00";
        }

     
        //END====================================================================================================


    
    }
}
