using System;
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

namespace ISBS_New.AccountPayable.InvoicePayment
{
    public partial class InvoicePayment : MetroFramework.Forms.MetroForm
    {
        #region Variable
        //SQL Connection
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;

        string InvoiceId="";
        string Mode, Query = null;
        int Index = 0;

        DataGridViewComboBoxCell cell;
        DataGridViewCheckBoxCell TmpCheckBox;

        //Looping
        int i = 0;

        //Attachment
        List<string> sSelectedFile, FileName, Extension; 
        List<byte[]> test = new List<byte[]>();

        //Parent
        InquiryV2 Parent;
        #endregion

        public InvoicePayment()
        {
            InitializeComponent();
        }

        private void InvoicePayment_Load(object sender, EventArgs e)
        {
            LoadInvoiceType();
            LoadCmbCurrency();
            LoadPaymentMethod();
            cmbCurrency.SelectedItem = "IDR";

            if (Variable.Kode != null)
            {
                txtInvoiceId.Text = Variable.Kode[0];
                GetDataHeader();
            }

            if (txtInvoiceId.Text == "")
            {
                ModeNew();
            }
            else if (txtInvoiceId.Text != "")
            {
                ModeView();
            }
        }

        #region Function

        public void SetParent(InquiryV2 F)
        {
            Parent = new InquiryV2();
        }
        
        public void ModeNew()
        {
            cmbInvoiceType.Enabled = true;
            dtInvoiceDate.Enabled = true;
            dtInvoiceDate.CustomFormat = " ";
            btnVendAccount.Enabled = true;
            txtVendInvoiceNumber.ReadOnly = false;
            dtInvoiceDate.Enabled = true;
            dtVendInvoiceDate.CustomFormat = " ";
            dtVendInvoiceDate.Enabled = true;
            dtVendInvoiceDueDate.CustomFormat = " ";
            dtVendInvoiceDueDate.Enabled = false;

            cmbCurrency.Enabled = true;
            txtExchRate.ReadOnly = false;
            txtDiscountAmt.ReadOnly = false;
            txtInvoiceAmount.ReadOnly = false;
            txtInvoicePPN.ReadOnly = false;
            txtInvoicePPH.ReadOnly = false;
            txtNotes.ReadOnly = false;

            txtNPWP.ReadOnly = false;
            dtTaxDate.Enabled = true;
            dtTaxDate.CustomFormat = " ";
            txtTaxNumber.ReadOnly = false;
            txtTaxName.ReadOnly = false;
            txtTaxAddress.ReadOnly = false;

            cmbPaymentMethod.Enabled = true;

            btnNewDetailAP.Enabled = true;
            btnDeleteDetailAP.Enabled = true;
            dgvDetailAP.ReadOnly = false;
            dgvRetur.ReadOnly = false;
            dgvDetailItem.ReadOnly = false;
            btnDeleteAttachment.Enabled = true;
            btnDownloadAttachment.Enabled = true;
            btnNewAttachment.Enabled = true;
            dgvAttachment.ReadOnly = false;

            btnCancelApprove.Enabled = false;
            btnApproved.Enabled = false;
            btnReject.Enabled = false;
            btnRevisi.Enabled = false;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
        }

        public void ModeEdit()
        {
            cmbInvoiceType.Enabled = true;
            dtInvoiceDate.Enabled = true;
            dtInvoiceDate.CustomFormat = " ";
            btnVendAccount.Enabled = false;
            txtVendInvoiceNumber.ReadOnly = false;
            dtInvoiceDate.Enabled = true;
            dtVendInvoiceDate.CustomFormat = " ";
            dtVendInvoiceDate.Enabled = true;
            dtVendInvoiceDueDate.CustomFormat = " ";
            dtVendInvoiceDueDate.Enabled = false;

            cmbCurrency.Enabled = true;
            txtExchRate.ReadOnly = false;
            txtDiscountAmt.ReadOnly = false;
            txtInvoiceAmount.ReadOnly = false;
            txtInvoicePPN.ReadOnly = false;
            txtInvoicePPH.ReadOnly = false;
            txtNotes.ReadOnly = false;

            txtNPWP.ReadOnly = false;
            dtTaxDate.Enabled = true;
            dtTaxDate.CustomFormat = " ";
            txtTaxNumber.ReadOnly = false;
            txtTaxName.ReadOnly = false;
            txtTaxAddress.ReadOnly = false;

            cmbPaymentMethod.Enabled = true;

            btnNewDetailAP.Enabled = true;
            btnDeleteDetailAP.Enabled = true;
            dgvDetailAP.ReadOnly = false;
            dgvRetur.ReadOnly = false;
            dgvDetailItem.ReadOnly = false;
            btnDeleteAttachment.Enabled = true;
            btnDownloadAttachment.Enabled = true;
            btnNewAttachment.Enabled = true;
            dgvAttachment.ReadOnly = false;

            btnCancelApprove.Enabled = false;
            btnApproved.Enabled = false;
            btnReject.Enabled = false;
            btnRevisi.Enabled = false;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
        }

        public void ModeView()
        {
            cmbInvoiceType.Enabled = false;
            dtInvoiceDate.Enabled = false;
            dtInvoiceDate.CustomFormat = "dd/MM/yyyy";
            btnVendAccount.Enabled = false;
            txtVendInvoiceNumber.ReadOnly = true;
            dtInvoiceDate.Enabled = false;
            dtVendInvoiceDate.CustomFormat = "dd/MM/yyyy";
            dtVendInvoiceDate.Enabled = false;
            dtVendInvoiceDueDate.CustomFormat = "dd/MM/yyyy";
            dtVendInvoiceDueDate.Enabled = false;

            cmbCurrency.Enabled = false;
            txtExchRate.ReadOnly = true;
            txtDiscountAmt.ReadOnly = true;
            txtInvoiceAmount.ReadOnly = true;
            txtInvoicePPN.ReadOnly = true;
            txtInvoicePPH.ReadOnly = true;
            txtNotes.ReadOnly = true;

            txtNPWP.ReadOnly = true;
            dtTaxDate.Enabled = false;
            dtTaxDate.CustomFormat = "dd/MM/yyyy";
            txtTaxNumber.ReadOnly = true;
            txtTaxName.ReadOnly = true;
            txtTaxAddress.ReadOnly = true;

            cmbPaymentMethod.Enabled = false;

            btnNewDetailAP.Enabled = false;
            btnDeleteDetailAP.Enabled = false;
            dgvDetailAP.ReadOnly = true;
            dgvRetur.ReadOnly = true;
            dgvDetailItem.ReadOnly = true;
            btnDeleteAttachment.Enabled = false;
            btnDownloadAttachment.Enabled = false;
            btnNewAttachment.Enabled = false;
            dgvAttachment.ReadOnly = true;

            btnCancelApprove.Enabled = false;
            btnApproved.Enabled = false;
            btnReject.Enabled = false;
            btnRevisi.Enabled = false;
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
        }

        public void ModeApprove()
        {
            ModeView();
            btnApproved.Enabled = true;
            btnReject.Enabled = true;
            btnRevisi.Enabled = true;
        }

        public void ModeCancelApprove()
        {
            ModeView();
            btnApproved.Enabled = false;
            btnReject.Enabled = false;
            btnRevisi.Enabled = false;
        }

        public void LoadInvoiceType()
        {
            try
            {
                cmbInvoiceType.Items.Clear();
                cmbInvoiceType.Items.Add("Uang Muka");
                cmbInvoiceType.Items.Add("Invoice");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadCmbCurrency()
        {
            try
            {
                using (Conn = ConnectionString.GetConnection())
                {
                    cmbCurrency.Items.Clear();
                    Cmd = new SqlCommand("SELECT CurrencyID from [dbo].[CurrencyTable]", Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        cmbCurrency.Items.Add(Dr[0]);
                    }
                    Dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadPaymentMethod()
        {
            try
            {
                cmbPaymentMethod.Items.Clear();
                cmbPaymentMethod.Items.Add("CASH");
                cmbPaymentMethod.Items.Add("CHEQUE");
                cmbPaymentMethod.Items.Add("TRANSFER");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        public bool Validasi()
        {
            if (dtInvoiceDate.CustomFormat == " ")
            {
                MessageBox.Show("Invoice Date harus diisi terlebih dahulu.");
                return false;
            }
            if (dtVendInvoiceDate.CustomFormat == "")
            {
                MessageBox.Show("Vendor Invoice Date harus diisi terlebih dahulu.");
                return false;
            }
            if (dtVendInvoiceDueDate.CustomFormat == "")
            {
                MessageBox.Show("Vendor Invoice Due Date harus diisi terlebih dahulu.");
                return false;
            }
            if (txtVendInvoiceNumber.Text == "")
            {
                MessageBox.Show("Vendor Invoice Number harus diisi terlebih dahulu.");
                return false;
            }
            if (Convert.ToDecimal(txtInvoicePPN.Text) > 0)
            {
                if (txtNPWP.Text == "")
                {
                    MessageBox.Show("NPWP harus diisi terlebih dahulu.");
                    return false;
                }
                if (dtTaxDate.CustomFormat == "")
                {
                    MessageBox.Show("Tax Date harus diisi terlebih dahulu.");
                    return false;
                }
                if (txtTaxNumber.Text == "")
                {
                    MessageBox.Show("Tax Number harus diisi terlebih dahulu.");
                    return false;
                }
                if (txtTaxName.Text == "")
                {
                    MessageBox.Show("Tax Name harus diisi terlebih dahulu.");
                    return false;
                }
                if (txtTaxAddress.Text == "")
                {
                    MessageBox.Show("Tax Address harus diisi terlebih dahulu.");
                    return false;
                }
            }
            if (cmbPaymentMethod.Text == "")
            {
                MessageBox.Show("Payment Method harus diisi terlebih dahulu.");
                return false;
            }
            if(Convert.ToDecimal(dgvDetailAP.RowCount) <=0)
            {
                MessageBox.Show("Datagrid Detail AP harus diisi terlebih dahulu.");
                return false;
            }
            for (i = 0; i < dgvDetailAP.RowCount; i++)
            {
                if (dgvDetailAP.Rows[i].Cells["InvoiceAmount"].Value != DBNull.Value)
                {
                    double a = Convert.ToDouble(dgvDetailAP.Rows[i].Cells["InvoiceAmount"].Value);
                    double c = Convert.ToDouble(txtAmountNett.Text);

                    if (a <= c)
                    {
                        HitungInvoiceAmount();
                    }
                    else
                    {
                        MessageBox.Show("Nilai Amount To Pay Tidak Boleh Lebih Besar Dari Nilai Invoice Amount Nett.");
                        dgvDetailAP.Focus();
                        dgvDetailAP.CurrentCell = dgvDetailAP.Rows[i].Cells["InvoiceAmount"];
                        return false; ;
                    }
                }
            }

            return true;
        }

        private void PilihVendor()
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
                txtTaxAddress.Text = ConnectionString.Kodes[6];
                ConnectionString.Kodes = null;
            }
            HitungInvoiceAmount();
        }

        private void HitungInvoiceAmount()
        {
            if (txtInvoicePPN.Text == "")
                txtInvoicePPN.Text = "0.00";
            if (txtInvoicePPH.Text == "")
                txtInvoicePPH.Text = "0.00";
            txtInvoicePPNAmount.Text = (Convert.ToDecimal(txtInvoiceAmount.Text) * Convert.ToDecimal(txtInvoicePPN.Text) / 100).ToString("N2");
            txtInvoicePPHAmount.Text = (Convert.ToDecimal(txtInvoiceAmount.Text) * Convert.ToDecimal(txtInvoicePPH.Text) / 100).ToString("N2");
            this.txtAmountNett.Text = (Convert.ToDecimal(txtInvoiceAmount.Text) + (Convert.ToDecimal(txtInvoicePPNAmount.Text) + Convert.ToDecimal(txtInvoicePPHAmount.Text))).ToString("N2");
        }

        public void AddDataGridUangMuka(List<string> PONo, List<string> PODate, List<string> POAmount, List<string> PODueDate, List<string> DPRequired, List<string> DPPercent,
                                       List<string> DPAmount, List<string> DPDeduct, List<string> DPOutstanding,
                                       List<string> POInvoice, List<string> POUnInvoiced)
        {
            if (dgvDetailAP.RowCount == 0)
            {
                dgvDetailAP.Rows.Clear();
                dgvDetailAP.ColumnCount = 14;

                dgvDetailAP.Columns[0].Name = "No";
                dgvDetailAP.Columns[1].Name = "PONo";
                dgvDetailAP.Columns[2].Name = "PODate";
                dgvDetailAP.Columns[3].Name = "POAmount";

                dgvDetailAP.Columns[4].Name = "PODueDate";
                dgvDetailAP.Columns[5].Name = "DPRequired";
                dgvDetailAP.Columns[6].Name = "DPPercent";
                dgvDetailAP.Columns[7].Name = "DPAmount";

                dgvDetailAP.Columns[8].Name = "DPDeduct";
                dgvDetailAP.Columns[9].Name = "DPOutstanding";
                dgvDetailAP.Columns[10].Name = "POInvoiced";
                dgvDetailAP.Columns[11].Name = "POUnInvoice";

                dgvDetailAP.Columns[12].Name = "PayableAmount";
                dgvDetailAP.Columns[13].Name = "InvoiceAmount";
            }

            for (int i = 0; i < PONo.Count; i++)
            {
                this.dgvDetailAP.Rows.Add((dgvDetailAP.RowCount + 1).ToString(), PONo[i], PODate[i], POAmount[i], PODueDate[i], DPRequired[i], DPPercent[i], DPAmount[i], DPDeduct[i], DPOutstanding[i], POInvoice[i], POUnInvoiced[i], Convert.ToDecimal(POUnInvoiced[i]) - Convert.ToDecimal(POInvoice[i]), "0.00");
            }
            dgvDetailAP.AutoResizeColumns();
            dgvDetailAP.Refresh();
        }

        public void AddDataGridInvoice(List<string> PONo, List<string> PODate, List<string> POAmount, List<string> PODueDate, List<string> GRNo, List<string> DPAmount, List<string> DPOutstanding,
                                       List<string> POInvoiced, List<string> POUnInvoiced,
                                       List<string> GRAmount, List<string> PotDP, List<string> GRPayable)
        {
            if (dgvDetailAP.RowCount == 0)
            {
                dgvDetailAP.Rows.Clear();
                dgvDetailAP.ColumnCount = 14;

                dgvDetailAP.Columns[0].Name = "No";
                dgvDetailAP.Columns[1].Name = "PONo";
                dgvDetailAP.Columns[2].Name = "PODate";
                dgvDetailAP.Columns[3].Name = "POAmount";

                dgvDetailAP.Columns[4].Name = "PODueDate";
                dgvDetailAP.Columns[5].Name = "GRNo";
                dgvDetailAP.Columns[6].Name = "DPAmount";
                dgvDetailAP.Columns[7].Name = "DPOutstanding";

                dgvDetailAP.Columns[8].Name = "POInvoiced";
                dgvDetailAP.Columns[9].Name = "POUnInvoice";
                dgvDetailAP.Columns[10].Name = "GRAmount";
                dgvDetailAP.Columns[11].Name = "PotDP";

                dgvDetailAP.Columns[12].Name = "GRPayable";
                dgvDetailAP.Columns[13].Name = "InvoiceAmount";
            }

            for (int i = 0; i < PONo.Count; i++)
            {
                this.dgvDetailAP.Rows.Add((dgvDetailAP.RowCount + 1).ToString(), PONo[i], PODate[i], POAmount[i], PODueDate[i], GRNo[i], DPAmount[i], DPOutstanding[i], POInvoiced[i], POUnInvoiced[i], GRAmount[i], PotDP[i], GRPayable[i], "0.00");
            }
            dgvDetailAP.AutoResizeColumns();
            dgvDetailAP.Refresh();
        }

        public void AddReturNotaBeli()
        {
            if (cmbInvoiceType.Text != "Uang Muka" && cmbInvoiceType.Text != "")
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
                for (int i = 0; i < dgvDetailAP.RowCount; i++)
                {
                    ListPO += "'" + dgvDetailAP.Rows[i].Cells["PONo"].Value.ToString() + "'";
                    if (i != dgvDetailAP.RowCount - 1)
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
                    if (ListPO != "")
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
            dgvDetailAP.ReadOnly = false;
            for (int i = 0; i < dgvDetailAP.ColumnCount; i++)
            {
                if (dgvDetailAP.Columns[i].Name != "InvoiceAmount")
                {
                    dgvDetailAP.Columns[i].ReadOnly = true;
                }
            }
        }

        private void RefreshNumber()
        {
            for (i = 0; i < dgvDetailAP.RowCount; i++)
            {
                dgvDetailAP.Rows[0].Cells["No"].Value = i;
            }
        }

        private void AddDetailDgv(string invoiceId)
        {
            if (cmbInvoiceType.Text == "Uang Muka")
            {
                for (int i = 0; i < dgvDetailAP.RowCount; i++)
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
                    Cmd.Parameters.Add("@PONo", dgvDetailAP.Rows[i].Cells["PONo"].Value.ToString());
                    Cmd.Parameters.Add("@PODate", Convert.ToDateTime(dgvDetailAP.Rows[i].Cells["PODate"].Value).ToString("yyyy-MM-dd"));
                    Cmd.Parameters.Add("@PODueDate", Convert.ToDateTime(dgvDetailAP.Rows[i].Cells["PODueDate"].Value).ToString("yyyy-MM-dd"));
                    Cmd.Parameters.Add("@POAmount", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["POAmount"].Value.ToString()));
                    Cmd.Parameters.Add("@DPRequired", dgvDetailAP.Rows[i].Cells["DPRequired"].Value.ToString());
                    Cmd.Parameters.Add("@DPPercent", dgvDetailAP.Rows[i].Cells["DPPercent"].Value.ToString());
                    Cmd.Parameters.Add("@DPAmount", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["DPAmount"].Value.ToString()));
                    Cmd.Parameters.Add("@DPDeduct", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["DPDeduct"].Value.ToString()));
                    Cmd.Parameters.Add("@DPOutstanding", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["DPOutstanding"].Value.ToString()));
                    Cmd.Parameters.Add("@InvoiceAmount", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["InvoiceAmount"].Value.ToString()));
                    Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                    Cmd.ExecuteNonQuery();
                }
            }
            if (cmbInvoiceType.Text == "Invoice")
            {
                for (int i = 0; i < dgvDetailAP.RowCount; i++)
                {
                    Query = "Insert Into VendInvoice_Dtl (InvoiceDate,InvoiceId,SeqNo,PurchId,PurchDate,PurchDueDate,GRNo,PurchAmount,DPAmount, DPAmountOutstanding, PurchPaidAmount, PurchAmountOutstanding, GRAmount, PotDP, PayableAmount, InvoiceAmount, CreatedDate, CreatedBy) Values (";
                    Query += "@InvoiceDate,@invoiceId,@No,@PONo,@PODate,";
                    //edited by Thaddaeus, 10 Sept 2018
                    if (dgvDetailAP.Rows[i].Cells["PODueDate"].Value.ToString() == "")
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
                    Cmd.Parameters.Add(new SqlParameter("@PONo", dgvDetailAP.Rows[i].Cells["PONo"].Value.ToString()));
                    DateTime PODate = Convert.ToDateTime(dgvDetailAP.Rows[i].Cells["PODate"].Value);
                    DateTime PODueDate = Convert.ToDateTime(dgvDetailAP.Rows[i].Cells["PODueDate"].Value.ToString());
                    Cmd.Parameters.Add(new SqlParameter("@PODate", PODate.Year + "-" + PODate.Month + "-" + PODate.Day));
                    Cmd.Parameters.Add(new SqlParameter("@PODueDate", +PODueDate.Year + "-" + PODueDate.Month + "-" + PODueDate.Day));
                    Cmd.Parameters.Add(new SqlParameter("@GRNo", dgvDetailAP.Rows[i].Cells["GRNo"].Value.ToString()));
                    Cmd.Parameters.Add(new SqlParameter("@POAmount", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["POAmount"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@DPAmount", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["DPAmount"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@DPOutstanding", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["DPOutstanding"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@POInvoiced", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["POInvoiced"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@POUnInvoice", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["POUnInvoice"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@GRAmount", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["GRAmount"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@PotDP", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["PotDP"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@GRPayable", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["GRPayable"].Value.ToString())));
                    Cmd.Parameters.Add(new SqlParameter("@InvoiceAmount", Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["InvoiceAmount"].Value.ToString())));
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

        public void SaveDgvAttachmentData()
        {
            for (int i = 0; i < dgvAttachment.RowCount; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, filetype,fileSize, attachment) Values";
                Query += "( 'VendInvoiceH', @InvoiceId, @FileName, @ContentType, @Filetype, @FileSize ,@binaryValue);";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.Add("@InvoiceId", InvoiceId);
                Cmd.Parameters.Add("@FileName", dgvAttachment.Rows[i].Cells["FileName"].Value.ToString());
                Cmd.Parameters.Add("@ContentType", dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString());
                Cmd.Parameters.Add("@Filetype", dgvAttachment.Rows[i].Cells["Filetype"].Value.ToString());
                Cmd.Parameters.Add("@FileSize", dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString());
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }
        }

        private void SaveDgvDetailItem(string InvoiceId)
        {
            if (InvoiceId == "")
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

        public void CountAmountAP()
        {
            decimal TmpAmount = 0;
            for (int i = 0; i < dgvDetailAP.RowCount; i++)
            {
                TmpAmount += Convert.ToDecimal(dgvDetailAP.Rows[i].Cells["InvoiceAmount"].Value);
            }
            txtAmountAP.Text = TmpAmount.ToString("N2");
        }

        public void UpdateAmount()
        {
            using (Conn = ConnectionString.GetConnection())
            {
                for (int i = 0; i < dgvDetailAP.RowCount; i++)
                {
                    if (cmbInvoiceType.Text == "Invoice")
                    {
                        Query = "SELECT DPAmount, DPOutstanding, POInvoiced, POUnInvoice from [dbo].[PO_UnInvoiced_GR_View] a where PONo='" + dgvDetailAP.Rows[i].Cells["PONo"].Value.ToString() + "'";
                    }
                    else if (cmbInvoiceType.Text == "Uang Muka")
                    {
                        Query = "SELECT DPAmount, DPDeduct, DPOutstanding, POInvoiced, POUnInvoice from [dbo].[PO_UnInvoiced_DP_View] a where PONo='" + dgvDetailAP.Rows[i].Cells["PONo"].Value.ToString() + "'";
                    }

                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (cmbInvoiceType.Text == "Invoice")
                        {
                            dgvDetailAP.Rows[i].Cells["DPAmount"].Value = Dr["DPAmount"].ToString();
                            dgvDetailAP.Rows[i].Cells["DPOutstanding"].Value = Dr["DPOutstanding"].ToString();
                            dgvDetailAP.Rows[i].Cells["POInvoiced"].Value = Dr["POInvoiced"].ToString();
                            dgvDetailAP.Rows[i].Cells["POUnInvoice"].Value = Dr["POUnInvoice"].ToString();
                        }
                        else if (cmbInvoiceType.Text == "Uang Muka")
                        {
                            dgvDetailAP.Rows[i].Cells["DPAmount"].Value = Dr["DPAmount"].ToString();
                            dgvDetailAP.Rows[i].Cells["DPDeduct"].Value = Dr["DPDeduct"].ToString();
                            dgvDetailAP.Rows[i].Cells["DPOutstanding"].Value = Dr["DPOutstanding"].ToString();
                            dgvDetailAP.Rows[i].Cells["POInvoiced"].Value = Dr["POInvoiced"].ToString();
                            dgvDetailAP.Rows[i].Cells["POUnInvoice"].Value = Dr["POUnInvoice"].ToString();
                        }
                    }
                    Dr.Close();
                }
                dgvRetur.AutoResizeColumns();
            }
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public void GetDataHeader()
        {
            try
            {
                if (InvoiceId == "")
                {
                    InvoiceId = txtInvoiceId.Text.Trim();
                }
                using (Conn = ConnectionString.GetConnection())
                {
                    Query = "Select InvoiceType, InvoiceDate, InvoiceId, VendId, VendName, CurrencyId, ExchRate, VendorInvoiceNo, VendorInvoiceDate, DueDate, InvoiceAmount,TotalAmountToPay,InvoiceTaxBaseAmount,TaxPercent, InvoiceTaxAmount, PPHTaxPercent, PPHTaxAmount, PaymentDueDate, TermOfPayment, PaymentMethod, NPWP, TaxNum, TaxAddress, TaxName, TaxDate, Notes,[Additional_Disc], TransStatus, b.Deskripsi FROM [dbo].[VendInvoiceH] a left join TransStatusTable b on a.TransStatus=b.StatusCode ";
                    Query += "WHERE InvoiceId='" + InvoiceId + "';";
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

                        //dtPaymentDueDate.Text = Dr["PaymentDueDate"].ToString();
                        //cmbTermOfPayment.Text = Dr["TermOfPayment"].ToString();
                        cmbPaymentMethod.Text = Dr["PaymentMethod"].ToString();
                        txtNPWP.Text = Dr["NPWP"].ToString();
                        txtTaxNumber.Text = Dr["TaxNum"].ToString();

                        txtTaxAddress.Text = Dr["TaxAddress"].ToString();
                        txtTaxName.Text = Dr["TaxName"].ToString();
                        dtTaxDate.Text = Dr["TaxDate"].ToString();
                        txtNotes.Text = Dr["Notes"].ToString();

                        txtStatusDesc.Text = Dr["Deskripsi"].ToString();
                        txtStatusKode.Text = Dr["TransStatus"].ToString();
                        txtDiscountAmt.Text = Dr["Additional_Disc"].ToString();
                    }
                    Dr.Close();

                    dgvDetailAP.DataSource = null;

                    if (dgvDetailAP.RowCount == 0)
                    {
                        if (cmbInvoiceType.Text == "Invoice")
                        {
                            dgvDetailAP.Rows.Clear();
                            dgvDetailAP.ColumnCount = 14;

                            dgvDetailAP.Columns[0].Name = "No";
                            dgvDetailAP.Columns[1].Name = "PONo";
                            dgvDetailAP.Columns[2].Name = "PODate";
                            dgvDetailAP.Columns[3].Name = "POAmount";

                            dgvDetailAP.Columns[4].Name = "PODueDate";
                            dgvDetailAP.Columns[5].Name = "GRNo";
                            dgvDetailAP.Columns[6].Name = "DPAmount";
                            dgvDetailAP.Columns[7].Name = "DPOutstanding";

                            dgvDetailAP.Columns[8].Name = "POInvoiced";
                            dgvDetailAP.Columns[9].Name = "POUnInvoice";
                            dgvDetailAP.Columns[10].Name = "GRAmount";
                            dgvDetailAP.Columns[11].Name = "PotDP";

                            dgvDetailAP.Columns[12].Name = "GRPayable";
                            dgvDetailAP.Columns[13].Name = "InvoiceAmount";
                        }

                        if (cmbInvoiceType.Text == "Uang Muka")
                        {
                            dgvDetailAP.Rows.Clear();
                            dgvDetailAP.ColumnCount = 14;

                            dgvDetailAP.Columns[0].Name = "No";
                            dgvDetailAP.Columns[1].Name = "PONo";
                            dgvDetailAP.Columns[2].Name = "PODate";
                            dgvDetailAP.Columns[3].Name = "POAmount";

                            dgvDetailAP.Columns[4].Name = "PODueDate";
                            dgvDetailAP.Columns[5].Name = "DPRequired";
                            dgvDetailAP.Columns[6].Name = "DPPercent";
                            dgvDetailAP.Columns[7].Name = "DPAmount";

                            dgvDetailAP.Columns[8].Name = "DPDeduct";
                            dgvDetailAP.Columns[9].Name = "DPOutstanding";
                            dgvDetailAP.Columns[10].Name = "POInvoiced";
                            dgvDetailAP.Columns[11].Name = "POUnInvoice";

                            dgvDetailAP.Columns[12].Name = "PayableAmount";
                            dgvDetailAP.Columns[13].Name = "InvoiceAmount";
                        }
                    }
                    if (cmbInvoiceType.Text == "Uang Muka")
                    {
                        Query = "select distinct SeqNo, PurchId, PurchDate,PurchAmount,PurchDueDate, DPRequired,DPPercent, DPAmount,DPAmountDeduct,DPAmountOutstanding,PurchPaidAmount,PurchAmountOutstanding, PayableAmount, InvoiceAmount ";
                        Query += "from VendInvoice_Dtl ";
                        Query += "where InvoiceId='" + InvoiceId + "' order by SeqNo asc ";
                    }
                    if (cmbInvoiceType.Text == "Invoice")
                    {
                        Query = "select distinct SeqNo, PurchId, PurchDate, PurchAmount, PurchDueDate, GRNo, DPAmount, DPAmountOutstanding,PurchPaidAmount,PurchAmountOutstanding,GRAmount,PotDP, PayableAmount, InvoiceAmount ";
                        Query += "from VendInvoice_Dtl ";
                        Query += "where InvoiceId='" + InvoiceId + "' order by SeqNo asc ";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    int i = 0;

                    dgvDetailAP.Rows.Clear();

                    while (Dr.Read())
                    {
                        if (cmbInvoiceType.Text == "Uang Muka")
                        {
                            this.dgvDetailAP.Rows.Add(Dr["SeqNo"], Dr["PurchId"], Dr["PurchDate"], Dr["PurchAmount"], Dr["PurchDueDate"], Dr["DPRequired"],
                            Dr["DPPercent"], Dr["DPAmount"], Dr["DPAmountDeduct"],
                            Dr["DPAmountOutstanding"], Dr["PurchPaidAmount"], Dr["PurchAmountOutstanding"], Dr["PayableAmount"], Dr["InvoiceAmount"]);
                        }
                        if (cmbInvoiceType.Text == "Invoice")
                        {
                            this.dgvDetailAP.Rows.Add(Dr["SeqNo"], Dr["PurchId"], Dr["PurchDate"], Dr["PurchAmount"], Dr["PurchDueDate"], Dr["GRNo"], Dr["DPAmount"],
                            Dr["DPAmountOutstanding"], Dr["PurchAmountOutstanding"], Dr["PurchPaidAmount"],
                            Dr["GRAmount"], Dr["PotDP"], Dr["PayableAmount"], Dr["InvoiceAmount"]);
                        }
                        i++;
                    }
                    Dr.Close();

                    dgvDetailAP.AutoResizeColumns();
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

                    Query = "Select * From [tblAttachments] Where ReffTableName = 'VendInvoiceH' And ReffTransId = '" + InvoiceId + "'";
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
                        Query += "where Invoice_Id='" + InvoiceId + "' order by SeqNo asc ";

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

        private void PostingJournal()
        {
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
        }

        private void UnPostingJournal()
        {
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

        #endregion

        #region Event Action

        private void btnVendAccount_Click(object sender, EventArgs e)
        {
            if (dgvDetailAP.RowCount == 0)
            {
                PilihVendor();
            }
            else
            {
                DialogResult dr = MessageBox.Show("Apakah Anda Ingin Mengganti Vendor?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    dgvDetailAP.Rows.Clear();
                    txtAmountPay.Clear();
                    PilihVendor();
                }
            }

            if (txtInvoicePPN.Text != "" || txtInvoicePPH.Text != "" || txtInvoiceAmount.Text != "")
            {
                HitungInvoiceAmount();
            }
            else
            {
                txtAmountNett.Text = txtInvoiceAmount.Text;
            }
        }

        private void btnNewDetailAP_Click(object sender, EventArgs e)
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
                    tmpSearch.Select = new string[] { "PONo", "PODate", "POAmount", "PODueDate", "GRNo", "DPAmount", "DPOutstanding", "POInvoiced", "POUnInvoice", "GRAmount", "PotDP", "GRPayable" };
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
                            DPAmount.Add(Variable.Kode2[i, 5] == "" ? "0.000" : Variable.Kode2[i, 5]);

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

                        for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
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
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validasi() == false)
            {
                return;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();

                    string VendorInvoicedate = dtVendInvoiceDate.Text == "" ? null : dtVendInvoiceDate.Value.ToString("yyyy-MM-dd");
                    string duedate = dtVendInvoiceDueDate.Text == "" ? null : dtVendInvoiceDueDate.Value.ToString("yyyy-MM-dd");
                    string invoiceDate = dtInvoiceDate.Text == "" ? null : dtInvoiceDate.Value.ToString("yyyy-MM-dd");
                    string TaxDate = dtTaxDate.Text == "" ? null : dtTaxDate.Value.ToString("yyyy-MM-dd");

                    //jika ada pajak
                    //status untuk tabel TransStatusTable pada database
                    if ((Convert.ToDecimal(txtInvoicePPN.Text) != 0 && Convert.ToDecimal(txtInvoicePPH.Text) == 0) || (Convert.ToDecimal(txtInvoicePPH.Text) != 0 && Convert.ToDecimal(txtInvoicePPN.Text) == 0) || (Convert.ToDecimal(txtInvoicePPN.Text) != 0 && Convert.ToDecimal(txtInvoicePPH.Text) != 0))
                    {
                        txtStatusKode.Text = "01";
                    }
                    else //tidak ada pajak
                    {
                        txtStatusKode.Text = "02";
                    }

                    if (txtInvoiceId.Text.Trim() == "")
                    {
                        string Jenis = "", kode = "";
                        if (cmbInvoiceType.Text == "Uang Muka")
                        {
                            Jenis = "APRN";
                            kode = "APRN";
                        }
                        else //if (cmbInvoiceType.Text=="Invoice")
                        {
                            Jenis = "DPRN";
                            kode = "DPRN";
                        }
                        string invoiceId = ConnectionString.GenerateSequenceNo(Jenis, kode, "", "", Conn, Cmd);

                        Query = "Insert into VendInvoiceH (InvoiceId, InvoiceDate,InvoiceType, VendId, VendName, CurrencyId, ExchRate, VendorInvoiceNo, VendorInvoiceDate, DueDate, InvoiceAmount,TotalAmountToPay,InvoiceTaxBaseAmount, TaxStatusCode,TaxPercent, InvoiceTaxAmount, PPHTaxPercent, PPHTaxAmount, PaymentMethod,NPWP, TaxNum, TaxAddress, TaxName, TaxDate, Notes,TransStatus, CreatedDate, CreatedBy,[Additional_Disc]) values ";
                        Query += "(@InvoiceID, ";
                        Query += "@InvoiceDate, @InvoiceType, @VendAccount, @VendName, @Currency, @ExchRate, @VendInvoiceNumber, @InvoiceDate, ";
                        Query += "@VendInvoiceDueDate, @InvoiceAmount, @AmountPay, @AmountNett, '', @InvoicePPN, @InvoicePPNAmount, @InvoicePPH, @InvoicePPHAmount, ";
                        Query += "@PaymentMethod, @NPWP, @TaxNumber, @TaxAddress, @TaxName, @TaxDate, @Notes, @Status, getdate(), @Login, @Discount);";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.Add(new SqlParameter("@InvoiceID", invoiceId));
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
                        Cmd.Parameters.Add(new SqlParameter("@PaymentMethod", cmbPaymentMethod.Text));
                        Cmd.Parameters.Add(new SqlParameter("@NPWP", txtNPWP.Text));
                        Cmd.Parameters.Add(new SqlParameter("@TaxNumber", txtTaxNumber.Text));
                        Cmd.Parameters.Add(new SqlParameter("@TaxAddress", txtTaxAddress.Text));
                        Cmd.Parameters.Add(new SqlParameter("@TaxName", txtTaxName.Text));
                        Cmd.Parameters.Add(new SqlParameter("@TaxDate", dtTaxDate.Value.ToString("yyyy-MM-dd")));
                        Cmd.Parameters.Add(new SqlParameter("@Notes", txtNotes.Text));
                        Cmd.Parameters.Add(new SqlParameter("@Status", txtStatusKode.Text));
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
                        ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, txtStatusKode.Text, "", invoiceId, "", "", "");
                        //end======================================

                        //Save VendInvoice_Logs
                        Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + txtStatusKode.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                        SaveVendInvoiceLogs(InvoiceId, "-", LogsStatusDesc, "N");

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
                        //Query += "PaymentDueDate=@PaymentDueDate,";
                        //Query += "TermOfPayment=@TermOfPayment,";

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
                        //Cmd.Parameters.Add("@PaymentDueDate", dtPaymentDueDate.Value.ToString("yyyy-MM-dd"));
                        //Cmd.Parameters.Add("@TermOfPayment", Convert.ToString(cmbTermOfPayment.Text));
                        Cmd.Parameters.Add("@PaymentMethod", cmbPaymentMethod.Text);
                        Cmd.Parameters.Add("@NPWP", txtNPWP.Text);
                        Cmd.Parameters.Add("@TaxNum", txtTaxNumber.Text);
                        Cmd.Parameters.Add("@TaxAddress", txtTaxAddress.Text);
                        Cmd.Parameters.Add("@TaxName", txtTaxName.Text);
                        Cmd.Parameters.Add("@TaxDate", dtTaxDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@Notes", txtNotes.Text);
                        Cmd.Parameters.Add("@TransStatus", txtStatusKode.Text);
                        Cmd.Parameters.Add("@Additional_Disc", txtDiscountAmt.Text);
                        Cmd.Parameters.Add("@Login", ControlMgr.UserId);

                        Cmd.ExecuteNonQuery();

                        //createdBy Thaddaeus, 26 sept 2018, begin
                        ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, txtStatusKode.Text, "Edit", txtInvoiceId.Text, "", "", "");
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

                        AddDetailDgv(InvoiceId);
                        SaveDgvAttachmentData();

                        //Save VendInvoice_Logs
                        Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + txtStatusKode.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                        SaveVendInvoiceLogs(InvoiceId, "-", LogsStatusDesc, "E");

                        Query = "DELETE FROM [VendInvoice_Dtl_PO_Dtl] WHERE [Invoice_Id] = @Invoice_Id ;";
                        using (Cmd = new SqlCommand(Query, Conn))
                        {
                            Cmd.Parameters.AddWithValue("@Invoice_Id", txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();
                        }
                        SavedgvDetailItem(txtInvoiceId.Text);
                        
                        scope.Complete();
                        MessageBox.Show("Data  Invoice Id : " + txtInvoiceId.Text + " berhasil diupdate.");
                    }

                }
                GetDataHeader();

                Conn.Close();

                ModeView();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                Parent.RefreshDataGrid();
            }
        }

        private void SavedgvDetailItem(string p)
        {
            throw new NotImplementedException();
        }


        private void cmbInvoiceType_Click(object sender, EventArgs e)
        {
            if (dgvDetailAP.RowCount > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Detail sudah terisi. Apakah akan dikosongkan ? ", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvDetailAP.Rows.Clear();
                    dgvRetur.Rows.Clear();
                    dgvDetailItem.Rows.Clear();
                }
                else if (dialogResult == DialogResult.No)
                {
                    dgvDetailAP.Focus();
                }
            }
        }

        private void dtInvoiceDate_ValueChanged(object sender, EventArgs e)
        {
            dtInvoiceDate.CustomFormat = "dd/MM/yyyy";
        }

        private void dtVendInvoiceDate_ValueChanged(object sender, EventArgs e)
        {
            dtVendInvoiceDate.CustomFormat = "dd/MM/yyyy";
        }

        private void dtVendInvoiceDueDate_ValueChanged(object sender, EventArgs e)
        {
            dtVendInvoiceDate.CustomFormat = "dd/MM/yyyy";
        }

        private void dtTaxDate_ValueChanged(object sender, EventArgs e)
        {
            dtTaxDate.CustomFormat = "dd/MM/yyyy";
        }

        #endregion

        private void btnDeleteDetailAP_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Apakah Anda Ingin Menghapus Item?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (dgvDetailAP.RowCount > 0)
                {
                    if (dgvDetailAP.RowCount > 0)
                    {
                        Index = dgvDetailAP.CurrentRow.Index;
                        DialogResult dialogResult = MessageBox.Show("Apakah data: " + Environment.NewLine + "No = " + dgvDetailAP.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "PurchId = " + dgvDetailAP.Rows[Index].Cells["PONo"].Value.ToString() + Environment.NewLine + "Akan dihapus?", "Delete Confirmation!", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            dgvDetailAP.Rows.RemoveAt(Index);
                            for (int i = 0; i < dgvDetailAP.RowCount; i++)
                            {
                                dgvDetailAP.Rows[i].Cells["No"].Value = i + 1;
                            }
                        }
                    }
                }
                RefreshNumber();
            }
        }

        private void btnNewAttachment_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            OpenFile.FilterIndex = 3;
            OpenFile.Multiselect = true;

            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                FileName = new List<string>();
                Extension = new List<string>();
                sSelectedFile = new List<string>();

                i = 0;

                foreach (string file in OpenFile.FileNames)
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

        private void btnDownloadAttachment_Click(object sender, EventArgs e)
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

        private void btnDeleteAttachment_Click(object sender, EventArgs e)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeView();
        }

        private void btnEdit_Click(object sender, EventArgs e)
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
                            Cmd.Parameters.Add("@richTextBoxNotes", txtNotes.Text);
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
                            Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + txtStatusKode.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                            //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(InvoiceId, "-", LogsStatusDesc, LogsAction);

                            scope.Complete();
                            MessageBox.Show("Data InvoiceId : " + txtInvoiceId.Text + " Berhasil Diupdate ");
                            
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
                            Cmd.Parameters.Add("@richTextBoxNotes", txtNotes.Text);
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
                            Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + txtStatusKode.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                            //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(InvoiceId, "-", LogsStatusDesc, LogsAction);

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

                    Parent.RefreshDataGrid();
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

                        string LogStatusDesc = "";
                        string LogDescription = "";

                        DialogResult dr = MessageBox.Show("Invoice Id No = " + InvoiceId + "\n" + "Apakah Data Diatas Akan direject ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            if (ControlMgr.GroupName == "Tax Admin")
                            {
                                txtStatusKode.Text = "08";
                                LogStatusDesc = "Not Approved by Tax";
                                LogDescription = "Status: 08. Not Approved By Tax Admin";
                            }
                            else if (ControlMgr.GroupName == "AP Manager")
                            {
                                txtStatusKode.Text = "04";
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
                            Cmd.Parameters.Add("@status", txtStatusKode.Text);
                            Cmd.Parameters.Add("@richTextBoxNotes", txtNotes.Text);
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.Parameters.Add("@txtInvoiceId", txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();

                            //createdBy Thaddaeus, 26 sept 2018, begin
                            ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, txtStatusKode.Text, "", txtInvoiceId.Text, "", "", "");
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
                                Query = "SELECT PurchAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT DPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT DPAmountDeduct FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmountDeduct_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT DPAmountOutstanding FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmountOutstanding_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT PurchPaidAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchPaidAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT PurchAmountOutstanding FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchAmountOutstanding_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT GRAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GRAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT RTBAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string RTBAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT RDNAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string RDNAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT GRNettAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GRNettAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT GR_DPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GR_DPAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT PayableDPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PayableDPAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT PayableAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PayableAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT InvoiceAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
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
                                Query += "WHERE InvoiceId=@txtInvoiceId and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
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
                                Cmd.Parameters.Add("@PONo", dgvDetailAP.Rows[i].Cells["PONo"].Value);
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
                                Cmd.Parameters.Add("@status", txtStatusKode.Text);
                                Cmd.Parameters.Add("@LogStatusDesc", LogStatusDesc);
                                Cmd.Parameters.Add("@LogDescription", LogDescription);
                                Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                                Cmd.ExecuteNonQuery();

                                //Save VendInvoice_Logs
                                //Get Status
                                Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + txtStatusKode.Text + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                                //Get Last Action
                                Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                                Cmd = new SqlCommand(Query, Conn);
                                string LogsAction = Cmd.ExecuteScalar().ToString();
                                SaveVendInvoiceLogs(InvoiceId, "-", LogsStatusDesc, LogsAction);
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

                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
                finally
                {
                    Conn.Close();
                    Parent.RefreshDataGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
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
                        DialogResult dr = MessageBox.Show("Invoice Id No = " + InvoiceId + "\n" + "Apakah Data Diatas Akan Diapprove?", "Konfirmasi", MessageBoxButtons.YesNo);
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
                            Cmd.Parameters.Add("@richTextBoxNotes", txtNotes.Text);
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
                            SaveVendInvoiceLogs(InvoiceId, "-", LogsStatusDesc, LogsAction);

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

                    Parent.RefreshDataGrid();

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

                        string LogStatusDesc = "";
                        string LogDescription = "";

                        DialogResult dr = MessageBox.Show("Invoice Id No = " + InvoiceId + "\n" + "Apakah Data Diatas Akan diCancel ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            if (ControlMgr.GroupName == "Tax Admin")
                            {
                                txtStatusKode.Text = "01";
                                LogStatusDesc = "Cancel Approved by Tax";
                                LogDescription = "Status: 02. Waiting Approval By Tax Admin";
                            }
                            else if (ControlMgr.GroupName == "AP Manager")
                            {
                                txtStatusKode.Text = "02";
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
                            Cmd.Parameters.Add("@status", txtStatusKode.Text);
                            Cmd.Parameters.Add("@richTextBoxNotes", txtNotes.Text);
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.Parameters.Add("@InvoicedId", txtInvoiceId.Text);
                            Cmd.ExecuteNonQuery();

                            //createdBy Thaddaeus, 26 sept 2018, begin
                            ListMethod.StatusLogVendor("HeaderAccountsPayable", "VendInvoice", txtVendAccount.Text, txtStatusKode.Text, LogStatusDesc, txtInvoiceId.Text, "", "", "");
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
                                Query = "SELECT PurchAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT DPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT DPAmountDeduct FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmountDeduct_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT DPAmountOutstanding FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string DPAmountOutstanding_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT PurchPaidAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchPaidAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT PurchAmountOutstanding FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PurchAmountOutstanding_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT GRAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GRAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT RTBAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string RTBAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT RDNAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string RDNAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT GRNettAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GRNettAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT GR_DPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string GR_DPAmount_Old = Cmd.ExecuteScalar().ToString();

                                Query = "SELECT PayableDPAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PayableDPAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT PayableAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
                                Cmd = new SqlCommand(Query, Conn);
                                string PayableAmount_Old = Cmd.ExecuteScalar().ToString();
                                Query = "SELECT InvoiceAmount FROM VendInvoice_Dtl WHERE InvoiceId='" + txtInvoiceId.Text + "' and PurchId='" + dgvDetailAP.Rows[i].Cells["PONo"].Value + "' ";
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
                                Cmd.Parameters.Add("@PONo", dgvDetailAP.Rows[i].Cells["PONo"].Value);
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
                                Cmd.Parameters.Add("@PONo", dgvDetailAP.Rows[i].Cells["PONo"].Value);
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
                                Cmd.Parameters.Add("@status", txtStatusKode.Text);
                                Cmd.Parameters.Add("@LogStatusDesc", LogStatusDesc);
                                Cmd.Parameters.Add("@LogDescription", LogDescription);
                                Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                                Cmd.ExecuteNonQuery();
                            }

                            //Save VendInvoice_Logs
                            //Get Status
                            Query = "Select Deskripsi from TransStatusTable where TransCode='VendInvoice' and StatusCode='" + txtStatusKode.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                            //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(InvoiceId, "-", LogsStatusDesc, LogsAction);

                            //Cancel Journal
                            if (txtStatusKode.Text == "02")
                            {
                                UnPostingJournal();
                            }

                            MessageBox.Show("Data InvoiceId : " + txtInvoiceId.Text + " Berhasil Diupdate ");

                            GetDataHeader();
                            ModeApprove();

                            Parent.RefreshDataGrid();
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
                    Parent.RefreshDataGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void txtExchRate_KeyPress(object sender, KeyPressEventArgs e)
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtInvoicePPH_KeyPress(object sender, KeyPressEventArgs e)
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


    }
}
