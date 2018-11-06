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

namespace ISBS_New.AccountsReceivable.CustomerInvoice
{
    public partial class HeaderCustomerInvoice :  MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlCommand Cmd2;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query = null;
        string InvoiceId = "", TransStatus = "", IDItemDP = "";
        int Index;
        List<string> sSelectedFile, FileName, Extension;
        List<byte[]> attachByte = new List<byte[]>();
        bool Journal = false;

        //tia edit
        ContextMenu vendid = new ContextMenu();
        //tia edit end
      
        AccountsReceivable.CustomerInvoice.InquiryCustomerInvoice Parent;

        public HeaderCustomerInvoice()
        {
            InitializeComponent();
        }

        public void SetParent(AccountsReceivable.CustomerInvoice.InquiryCustomerInvoice F)
        {
            Parent = F;
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public void SetMode(string tmpMode, string tmpInvoiceId)
        {
            Mode = tmpMode;
            InvoiceId = tmpInvoiceId;
        }

        private void HeaderCustomerInvoice_Load(object sender, EventArgs e)
        {
            SetCmbCurrency();
            SetCmbPaymentMethod();
            SetCmbInvoiceType();
            SetPPN();
            setHeaderDgvAttachment();
            setHeaderDgvCNReference();            
            GetTransStatus();
           
            if (Mode == "New")
            {
                setHeaderDgvSOReferenceDetailsSO();         
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
                GetDataHeader();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }//tia edit
            else if (Mode=="PopUp")
            {
                ModePopUp();
               GetDataHeader();
            }
            //edit end
            
            this.CenterToScreen();
        }

        private void HeaderCustomerInvoice_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 10);
        }

        private void ModeNew()
        {
            InvoiceId = "";
            
            btnReject.Visible = false;
            btnRevisi.Visible = false;
            btnApprove.Visible = false;
            btnCancelApprove.Visible = false;

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment")
            {
                btnLookupDPDescription.Enabled = true;
            }
            else
            {
                btnLookupDPDescription.Enabled = false;            
            }

            dtInvoiceDate.Value = DateTime.Today.Date;
            dtTaxDate.Value = DateTime.Today.Date;
            dtPaymentDueDate.Value = DateTime.Today.Date;
            dgvAttachment.Rows.Clear();

            txtSOAmount.Text = "0.00";
            txtGIAmount.Text = "0.00";
            txtReturAmount.Text = "0.00";
            txtDPPAmount.Text = "0.00";
            txtDPAmount.Text = "0.00";
            txtTaxAmount.Text = "0.00";
            txtInvoiceAmount.Text = "0.00";
            txtCNAmount.Text = "0.00";
            txtARAmount.Text = "0.00";
            txtPembulatan.Text = "0.00";
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";            

            cmbInvoiceType.Enabled = false;
            btnLookupCustomer.Enabled = false;

            btnLookupProforma.Enabled = false;
            btnLookupSONo.Enabled = false;

            btnLookupDPDescription.Enabled = false;


            cmbCurrency.Enabled = false;
            txtExchRate.Enabled = false;
            cmbPPN.Enabled = false;

            cmbPaymentMethod.Enabled = false;
            dtPaymentDueDate.Enabled = false;

            dtTaxDate.Enabled = false;
            txtTaxNumber.Enabled = false;
            txtNPWP.Enabled = false;
            txtTaxName.Enabled = false;
            txtTaxAddress.Enabled = false;
            txtKetTambahan.Enabled = false;
            txtKodeKetTambahan.Enabled = false;
            btnLookupKetTambahan.Enabled = false;

            btnAddNewFile.Enabled = false;
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;
            dgvAttachment.ReadOnly = true;
            setHeaderDgvAttachment();
            btnAddNewFile.Enabled = false;
            btnDeleteFile.Enabled = false;
            btnDownloadFile.Enabled = false;

            txtNotes.Enabled = false;

           
            btnNewSOReference.Enabled = false;
            btnDeleteSOReference.Enabled = false;
            dgvSoReferenceDetailsSO.DefaultCellStyle.BackColor = Color.LightGray;
            dgvSoReferenceDetailsSO.ReadOnly = true;
            setHeaderDgvSOReferenceDetailsSO();

            dgvCNReference.DefaultCellStyle.BackColor = Color.LightGray;
            dgvCNReference.ReadOnly = true;
            //setHeaderDgvCNReference();

            txtPembulatan.Enabled = false; 
            //tia edit
            txtCustName.Enabled = true;
            txtCustID.Enabled = true;
            txtSONo.Enabled = true;
            txtProformaID.Enabled = true;

            txtCustName.ReadOnly = true;
            txtCustID.ReadOnly = true;
            txtSONo.ReadOnly = true;
            txtProformaID.ReadOnly = true;

            txtCustName.ContextMenu = vendid;
            txtCustID.ContextMenu = vendid;
            txtSONo.ContextMenu = vendid;
            txtProformaID.ContextMenu = vendid;

            //tia edit end

            GetDataHeader();

            if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN")
            {
                if (TransStatus == "01" || TransStatus == "17")
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = true;
                    btnApprove.Visible = true;
                    btnCancelApprove.Visible = false;

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = true;
                    btnCancel.Visible = false;
                }
                else
                {                    
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;

                    if (cmbPPN.SelectedItem.ToString() != "0.00" && TransStatus == "02")
                    {
                        btnCancelApprove.Visible = true;
                    }
                    else
                    {
                        btnCancelApprove.Visible = false;
                    }                    

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
            {
                if (TransStatus == "11")
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = true;
                    btnApprove.Visible = true;
                    btnCancelApprove.Visible = false;

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = true;
                    btnCancel.Visible = false;
                }
                else
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;

                    if (TransStatus == "13")
                    {
                        btnCancelApprove.Visible = true;
                    }
                    else
                    {
                        btnCancelApprove.Visible = false;
                    } 

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
            }
            else if (ControlMgr.GroupName.ToUpper() == "AR MANAGER")
            {
                if (TransStatus == "02")
                {
                    btnReject.Visible = true;
                    btnRevisi.Visible = true;
                    btnApprove.Visible = true;
                    btnCancelApprove.Visible = false;

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = true;
                    btnCancel.Visible = false;
                }
                else
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;

                    if ((cmbPPN.SelectedItem.ToString() == "0.00" && TransStatus == "03") || (cmbPPN.SelectedItem.ToString() != "0.00" && TransStatus == "11"))
                    {
                        btnCancelApprove.Visible = true;
                    }
                    else
                    {
                        btnCancelApprove.Visible = false;
                    }

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
            }
            else
            {
                btnReject.Visible = false;
                btnRevisi.Visible = false;
                btnApprove.Visible = false;
                btnCancelApprove.Visible = false;

                btnSave.Visible = false;
                btnExit.Visible = true;

                if (TransStatus == "01" || TransStatus == "02" || TransStatus == "07" || TransStatus == "08")
                {
                    btnEdit.Visible = true;
                }
                else
                {
                    btnEdit.Visible = false;
                }
                btnCancel.Visible = false;
            }
        }

        //tia edit
        private void ModePopUp()
        {
            Mode = "PopUp";

            this.StartPosition = FormStartPosition.Manual;
                foreach (var scrn in Screen.AllScreens)
                {
                    if (scrn.Bounds.Contains(this.Location))
                        this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
                }

            cmbInvoiceType.Enabled = false;
            btnLookupCustomer.Enabled = false;

            btnLookupProforma.Enabled = false;


            cmbCurrency.Enabled = false;
            txtExchRate.Enabled = false;
            cmbPPN.Enabled = false;

            cmbPaymentMethod.Enabled = false;
            dtPaymentDueDate.Enabled = false;

            dtTaxDate.Enabled = false;
            txtTaxNumber.Enabled = false;
            txtNPWP.Enabled = false;
            txtTaxName.Enabled = false;
            txtTaxAddress.Enabled = false;
            txtKetTambahan.Enabled = false;
            txtKodeKetTambahan.Enabled = false;
            btnLookupKetTambahan.Enabled = false;

            btnAddNewFile.Enabled = false;
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;
            dgvAttachment.ReadOnly = true;
            setHeaderDgvAttachment();
            btnAddNewFile.Enabled = false;
            btnDeleteFile.Enabled = false;
            btnDownloadFile.Enabled = false;

            txtNotes.Enabled = false;


            btnNewSOReference.Enabled = false;
            btnDeleteSOReference.Enabled = false;
            dgvSoReferenceDetailsSO.DefaultCellStyle.BackColor = Color.LightGray;
            dgvSoReferenceDetailsSO.ReadOnly = true;
            setHeaderDgvSOReferenceDetailsSO();

            dgvCNReference.DefaultCellStyle.BackColor = Color.LightGray;
            dgvCNReference.ReadOnly = true;
           
            txtPembulatan.Enabled = false;
            txtCustName.Enabled = true;
            txtCustID.Enabled = true;

            txtCustName.ReadOnly = true;
            txtCustID.ReadOnly = true;

            txtCustName.ContextMenu = vendid;
            txtCustID.ContextMenu = vendid;
            GetDataHeader();

            if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN")
            {
                if (TransStatus == "01" || TransStatus == "17")
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;
                    btnCancelApprove.Visible = false;

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
                else
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;

                    if (cmbPPN.SelectedItem.ToString() != "0.00" && TransStatus == "02")
                    {
                        btnCancelApprove.Visible = false;
                    }
                    else
                    {
                        btnCancelApprove.Visible = false;
                    }

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
            {
                if (TransStatus == "11")
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;
                    btnCancelApprove.Visible = false;

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
                else
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;

                    if (TransStatus == "13")
                    {
                        btnCancelApprove.Visible = false;
                    }
                    else
                    {
                        btnCancelApprove.Visible = false;
                    }

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
            }
            else if (ControlMgr.GroupName.ToUpper() == "AR MANAGER")
            {
                if (TransStatus == "02")
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;
                    btnCancelApprove.Visible = false;

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
                else
                {
                    btnReject.Visible = false;
                    btnRevisi.Visible = false;
                    btnApprove.Visible = false;

                    if ((cmbPPN.SelectedItem.ToString() == "0.00" && TransStatus == "03") || (cmbPPN.SelectedItem.ToString() != "0.00" && TransStatus == "11"))
                    {
                        btnCancelApprove.Visible = false;
                    }
                    else
                    {
                        btnCancelApprove.Visible = false;
                    }

                    btnSave.Visible = false;
                    btnExit.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                }
            }
            else
            {
                btnReject.Visible = false;
                btnRevisi.Visible = false;
                btnApprove.Visible = false;
                btnCancelApprove.Visible = false;

                btnSave.Visible = false;
                btnExit.Visible = true;

                if (TransStatus == "01" || TransStatus == "02" || TransStatus == "07" || TransStatus == "08")
                {
                    btnEdit.Visible = false;
                }
                else
                {
                    btnEdit.Visible = false;
                }
                btnCancel.Visible = false;
            }
        }
        //tia edit end

        private void ModeEdit()
        {
            Mode = "Edit";
            GetTransStatus();

            btnReject.Visible = false;
            btnRevisi.Visible = false;
            btnApprove.Visible = false;
            btnCancelApprove.Visible = false;

            if (ControlMgr.GroupName.ToUpper() == "AR ADMIN")
            {
                //cmbInvoiceType.Enabled = true;
                btnLookupCustomer.Enabled = true;
                btnLookupSONo.Enabled = true;

                if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment")
                {
                    btnLookupProforma.Enabled = true;
                    btnLookupDPDescription.Enabled = true;
                }

                cmbCurrency.Enabled = true;
                if (cmbCurrency.SelectedItem.ToString() == "IDR")
                {
                    txtExchRate.Enabled = false;
                }
                else
                {
                    txtExchRate.Enabled = true;
                }

                cmbPPN.Enabled = true;

                cmbPaymentMethod.Enabled = true;
                dtPaymentDueDate.Enabled = true;
                //getPaymentDueDate
                //var GetPaymentDueDate = from CustInovice in dgvSoReferenceDetailsSO.Rows.Cast<DataGridViewRow>()
                //                        orderby Convert.ToInt32(CustInovice.Cells["SODueDate"].FormattedValue.ToString().Substring(6, 4)) + "" +
                //                        Convert.ToInt32(CustInovice.Cells["SODueDate"].FormattedValue.ToString().Substring(3, 2)) + "" +
                //                        Convert.ToInt32(CustInovice.Cells["SODueDate"].FormattedValue.ToString().Substring(0, 2)) descending
                //                        select CustInovice.Cells["SODueDate"].FormattedValue.ToString();

                //string PaymentDueDate = GetPaymentDueDate.First();
                //dtPaymentDueDate.MaxDate = Convert.ToDateTime(PaymentDueDate.Substring(3, 2) + "/" + PaymentDueDate.Substring(0, 2) + "/" + PaymentDueDate.Substring(6, 4));
                dtPaymentDueDate.MaxDate = dtPaymentDueDate.Value;
                //end getPaymentDueDate

                dtTaxDate.Enabled = false;
                txtTaxNumber.Enabled = false;
                txtNPWP.Enabled = false;
                txtTaxName.Enabled = false;
                txtTaxAddress.Enabled = false;
                txtKetTambahan.Enabled = false;
                txtKodeKetTambahan.Enabled = false;
                btnLookupKetTambahan.Enabled = false;

                btnAddNewFile.Enabled = true;
                dgvAttachment.DefaultCellStyle.BackColor = Color.White;
                dgvAttachment.ReadOnly = false;
                setHeaderDgvAttachment();
                btnAddNewFile.Enabled = true;
                btnDeleteFile.Enabled = true;
                btnDownloadFile.Enabled = true;

                txtNotes.Enabled = true;

                if (txtProformaID.Text == "")
                {
                    btnNewSOReference.Visible = true;
                    btnDeleteSOReference.Visible = true;
                    btnNewSOReference.Enabled = true;
                    btnDeleteSOReference.Enabled = true;
                    dgvSoReferenceDetailsSO.DefaultCellStyle.BackColor = Color.White;
                    dgvSoReferenceDetailsSO.ReadOnly = false;
                    setHeaderDgvSOReferenceDetailsSO();

                    dgvCNReference.DefaultCellStyle.BackColor = Color.White;
                    dgvCNReference.ReadOnly = false;
                    setHeaderDgvCNReference();
                }
                else
                {
                    btnNewSOReference.Visible = false;
                    btnDeleteSOReference.Visible = false;
                    btnNewSOReference.Enabled = true;
                    btnDeleteSOReference.Enabled = true;
                }

                txtPembulatan.Enabled = true;
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN")
            {
                
                dtTaxDate.Enabled = true;
                txtTaxNumber.Enabled = true;
                txtNPWP.Enabled = true;
                txtTaxName.Enabled = true;
                txtTaxAddress.Enabled = true;
                //txtKetTambahan.Enabled = true;
                //txtKodeKetTambahan.Enabled = true;

                if (txtTaxNumber.Text.Trim() != "")
                {
                    if (txtTaxNumber.Text.Trim().Substring(0, 2) == "07")
                    {
                        btnLookupKetTambahan.Enabled = true;
                    }
                    else
                    {
                        btnLookupKetTambahan.Enabled = false;
                    }
                }                               

                btnAddNewFile.Enabled = true;
                dgvAttachment.DefaultCellStyle.BackColor = Color.White;
                dgvAttachment.ReadOnly = false;
                setHeaderDgvAttachment();
                btnAddNewFile.Enabled = true;
                btnDeleteFile.Enabled = true;
                btnDownloadFile.Enabled = true;

                txtNotes.Enabled = true;
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
            {
                btnAddNewFile.Enabled = true;
                dgvAttachment.DefaultCellStyle.BackColor = Color.White;
                dgvAttachment.ReadOnly = false;
                setHeaderDgvAttachment();
                btnAddNewFile.Enabled = true;
                btnDeleteFile.Enabled = true;
                btnDownloadFile.Enabled = true;

                txtNotes.Enabled = true;
            }
            else if (ControlMgr.GroupName.ToUpper() == "AR MANAGER")
            {
                txtNotes.Enabled = true;
            }

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
        }

        private void GetDataHeader()
        {
            try
            {
                if (InvoiceId == "")
                {
                    InvoiceId = txtInvoiceId.Text.Trim();
                }
                Conn = ConnectionString.GetConnection();
                Query = "SELECT [Invoice_Date],[Invoice_Id],[Invoice_Type],[Cust_Id],[Cust_Name],[PPN_Percent] ";
                Query +=",[DP_Amount],[Invoice_Round_Amount],[Invoice_Amount] ,[Invoice_Tax_Base_Amount] ";
                Query +=",[Invoice_Tax_Amount] ,[Payment_Method] ,[Invoice_DueDate],[NPWP],[TaxDate] ,[TaxNum] ";
                Query +=",[TaxName] ,[TaxAddress] ,[Notes] ,[CN_Amount] ,[AR_Amount] ";
                Query += ",[TransStatus],[ApprovedBy],[CreatedDate] ,[CreatedBy] ,[UpdatedDate],[UpdatedBy],[CurrencyID],[ExchRate], [Proforma_Id], [SalesOrderNo], [KodeKetTambahan], DPDescription ";
                Query += "FROM CustInvoice_H WHERE Invoice_Id='" + InvoiceId + "';";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtInvoiceDate.Text = Convert.ToString(Dr["Invoice_Date"]);
                    txtInvoiceId.Text = Convert.ToString(Dr["Invoice_Id"]);
                    cmbInvoiceType.SelectedItem = Convert.ToString(Dr["Invoice_Type"]);
                    txtCustID.Text = Convert.ToString(Dr["Cust_Id"]);
                    txtCustName.Text = Convert.ToString(Dr["Cust_Name"]);
                    cmbPPN.SelectedItem = Convert.ToString(Dr["PPN_Percent"]);
                    txtDPAmount.Text = Convert.ToString(Dr["DP_Amount"]);
                    txtPembulatan.Text = Convert.ToString(Dr["Invoice_Round_Amount"]);
                    txtInvoiceAmount.Text = Convert.ToString(Dr["Invoice_Amount"]);
                    txtDPPAmount.Text = Convert.ToString(Dr["Invoice_Tax_Base_Amount"]);
                    txtTaxAmount.Text = Convert.ToString(Dr["Invoice_Tax_Amount"]);
                    cmbPaymentMethod.SelectedItem = Convert.ToString(Dr["Payment_Method"]);
                    dtPaymentDueDate.Text = Convert.ToString(Dr["Invoice_DueDate"]);
                    txtNPWP.Text = Convert.ToString(Dr["NPWP"]);
                    dtTaxDate.Text = Convert.ToString(Dr["TaxDate"]);
                    txtTaxNumber.Text = Convert.ToString(Dr["TaxNum"]);
                    txtTaxName.Text = Convert.ToString(Dr["TaxName"]);
                    txtTaxAddress.Text = Convert.ToString(Dr["TaxAddress"]);
                    txtNotes.Text = Convert.ToString(Dr["Notes"]);
                    txtCNAmount.Text = Convert.ToString(Dr["CN_Amount"]);
                    txtARAmount.Text = Convert.ToString(Dr["AR_Amount"]);
                    TransStatus = Convert.ToString(Dr["TransStatus"]);
                    cmbCurrency.SelectedItem = Convert.ToString(Dr["CurrencyID"]);
                    txtExchRate.Text = Convert.ToString(Dr["ExchRate"]);
                    txtProformaID.Text = Convert.ToString(Dr["Proforma_Id"]);
                    txtSONo.Text = Convert.ToString(Dr["SalesOrderNo"]);
                    txtDPDescription.Text = Convert.ToString(Dr["DPDescription"]);
                    if (Dr["KodeKetTambahan"].ToString() != "0")
                    {
                        txtKodeKetTambahan.Text = Convert.ToString(Dr["KodeKetTambahan"]);

                        Query = "SELECT Keterangan FROM KawasanBerikat WHERE Kode = '" + txtKodeKetTambahan.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        txtKetTambahan.Text = Convert.ToString(Cmd.ExecuteScalar());
                    }                  
                }
                Dr.Close();

                setHeaderDgvSOReferenceDetailsSO();
                dgvSoReferenceDetailsSO.Rows.Clear();

                if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
                {
                    //SO Detail
                    Query = "SELECT S.[SO_No] AS SONo, S.[SO_Date] AS SODate, S.[SO_DueDate] AS SODueDate, V.SOReference, V.DPRequired, V.DPPercent, ";
                    Query += "H.PPN_Percent AS PPN, S.[SO_Amount] AS SOAmount, P.DP_Amount AS DPAmount, V.DPDeduct, (P.DP_Amount - P.DP_Deduct) AS DPOutstanding, "; 
                    Query += "V.SOInvoiced, V.SOUnInvoiced, S.[GI_No] AS GINo,S.[GI_Date] AS GIDate, ";
                    Query += "S.[GI_Amount] AS GIAmount, S.[Retur_Amount] AS ReturAmount, S.[PotDP] AS POTDP , S.GI_Payable AS GIPayable, S.PotDP2 AS POTDP2, "; 
                    Query += "S.[Invoice_Amount] AS InvoiceAmount, S.[Invoice_Tax_Base_Amount] AS InvoiceTaxBaseAmount, S.[Invoice_Tax_Amount] AS InvoiceTaxAmount ";
                    Query += " FROM CustInvoice_Dtl_SO S LEFT JOIN vSalesUnInvoiceTable_GIView V ON V.SONo = S.SO_No AND V.GINo = S.GI_No LEFT JOIN CustDown_Payment P ON P.SO_Id = S.SO_No ";
                    Query += "LEFT JOIN CustInvoice_H H ON H.Invoice_Id = S.Invoice_Id ";
                    Query += "WHERE S.Invoice_Id='" + InvoiceId + "' ORDER BY SeqNo ASC ";                    
                    //End SO Detail
                }
                else
                {
                    //DP Detail
                    Query = "SELECT D.SO_No AS SONO, V.SODate, V.SODueDate, V.SOReference, V.DPRequired, V.DPPercent, D.Tax_Percent AS PPN, ";
                    Query += "V.SOAmount,  V.DPAmount, V.DPDeduct, V.DPOutstanding, V.SOInvoiced, V.SOUnInvoiced, '' AS GINo, GETDATE() AS GIDate, ";
                    Query += "V.GIAmount, V.ReturAmount, V.POTDP, V.GIPayable, '0.00' AS POTDP2, D.Line_Amount AS InvoiceAmount, D.Line_Tax_Base_Amount AS InvoiceTaxBaseAmount, D.Line_Tax_Amount AS InvoiceTaxAmount ";
                    Query += "FROM CustInvoice_Dtl_DP D LEFT JOIN vSalesUnInvoiceTable_DPView V ON V.SONo = D.SO_No ";
                    Query += "WHERE D.Invoice_Id='" + InvoiceId + "' ORDER BY D.SeqNo ASC ";
                    //End DP Detail
                }

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                int i = 1;
                while (Dr.Read())
                {
                    this.dgvSoReferenceDetailsSO.Rows.Add(i, Dr["SONo"], Dr["SODate"], Dr["SODueDate"], Dr["SOReference"], Dr["DPRequired"],
                    Dr["DPPercent"], Dr["PPN"], Dr["SOAmount"], Dr["DPAmount"], Dr["DPDeduct"], Dr["DPOutstanding"], Dr["SOInvoiced"],
                    Dr["SOUnInvoiced"], Dr["GINo"], Dr["GIDate"], Dr["GIAmount"], Dr["ReturAmount"], Dr["POTDP"], Dr["GIPayable"], Dr["POTDP2"], Dr["InvoiceAmount"], Dr["InvoiceTaxBaseAmount"], Dr["InvoiceTaxAmount"]);
                    i++;
                }
                Dr.Close();

                dgvSoReferenceDetailsSO.AutoResizeColumns();

                //Credit Note
                Query = "SELECT ROW_NUMBER() OVER (ORDER BY N.CN_No) No, N.CN_No AS CNNo, CONVERT(VARCHAR, N.CN_Date, 103) AS CNDate, ";
                Query += "N.TotalAmount AS Amount, N.Deduct AS AmountDeduct, N.TotalAmount - Deduct AS AmountOutstanding, ";
                Query += "CASE WHEN (SELECT CASE WHEN C.POT_CN IS NOT NULL THEN C.POT_CN ELSE 0 END FROM CustInvoice_Dtl_CreditNote C WHERE C.CN_No = N.CN_No AND C.Invoice_Id = '"+InvoiceId+"') ";
                Query += "IS NULL THEN 0.00 ELSE (SELECT CASE WHEN C.POT_CN IS NOT NULL THEN C.POT_CN ELSE 0 END FROM CustInvoice_Dtl_CreditNote C WHERE C.CN_No = N.CN_No AND C.Invoice_Id = '"+InvoiceId+"') END AS PotonganCN "; 
                Query += "FROM NotaCreditH N WHERE (N.TotalAmount - N.Deduct) > 0  AND N.AccountNum = '"+txtCustID.Text+"' ORDER BY N.CN_No ASC ";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                dgvCNReference.DataSource = null;
                dgvCNReference.Rows.Clear();
                dgvCNReference.Refresh();
                setHeaderDgvCNReference();
               

                int j = 1;
                while (Dr.Read())
                {   
                    Boolean CheckValue = Dr["PotonganCN"].ToString() == "0.00" ? false : true;
                    this.dgvCNReference.Rows.Add(CheckValue, j, Dr["CNNo"], Dr["CNDate"], Convert.ToString(Convert.ToDecimal(Dr["Amount"]).ToString("N2")), Convert.ToString(Convert.ToDecimal(Dr["AmountDeduct"]).ToString("N2")), Convert.ToString(Convert.ToDecimal(Dr["AmountOutstanding"]).ToString("N2")), Convert.ToString(Convert.ToDecimal(Dr["PotonganCN"]).ToString("N2")));
                    j++;
                }
                Dr.Close();               

                //End CreditNote

                attachByte.Clear();
                dgvAttachment.Rows.Clear();

                Query = "SELECT Id, FileType, FileName, ContentType, fileSize, attachment FROM [tblAttachments] WHERE ReffTableName = 'CustInvoiceH' And ReffTransId = '" + InvoiceId + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                int k = 1;
                while (Dr.Read())
                {
                    DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                    combo.Items.Clear();
                    combo.Items.Add("Faktur Pajak");
                    combo.Items.Add("Surat Jalan");
                    combo.Items.Add("Others");
                    combo.Value = Convert.ToString(Dr["FileType"]);             

                    this.dgvAttachment.Rows.Add(k, "", Dr["FileName"], Dr["ContentType"], Dr["fileSize"], Dr["Id"], "");
                    attachByte.Add((byte[])Dr["attachment"]);
                    dgvAttachment.Rows[(dgvAttachment.Rows.Count - 1)].Cells[1] = combo;
                    k++;
                }

                dgvAttachment.AutoResizeColumns();

                Conn.Close();

                FormulaFooter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetCmbCurrency()
        {
            Conn = ConnectionString.GetConnection();

            cmbCurrency.Items.Clear();
            Cmd = new SqlCommand("SELECT CurrencyID from CurrencyTable", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbCurrency.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();

            if(Mode == "New")
            {
                cmbCurrency.SelectedItem = "IDR";
                txtExchRate.Text = "1.00";
                txtExchRate.Enabled = false;
            }
        }

        private void SetCmbInvoiceType()
        {
            cmbInvoiceType.Items.Clear();
            cmbInvoiceType.Items.Add("Down Payment");
            cmbInvoiceType.Items.Add("Invoice");
            cmbInvoiceType.Items.Add("Proforma");
            cmbInvoiceType.SelectedIndex = 0;
        }

        private void GetTransStatus()
        {
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("SELECT TransStatus FROM CustInvoice_H WHERE Invoice_Id = '"+InvoiceId+"'", Conn);
            TransStatus = Convert.ToString(Cmd.ExecuteScalar());
            Conn.Close();
        }

        private void SetCmbPaymentMethod()
        {
            Conn = ConnectionString.GetConnection();

            cmbPaymentMethod.Items.Clear();
            Cmd = new SqlCommand("SELECT PaymentModeID FROM PaymentMode WHERE PaymentModeID <> '' ORDER BY PaymentModeID ASC", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbPaymentMethod.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();

            cmbPaymentMethod.SelectedIndex = 0;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvSoReferenceDetailsSO.Rows.Clear();
            if (cmbCurrency.SelectedItem.ToString() == "IDR")
            {
                txtExchRate.Text = "1.00";
                txtExchRate.Enabled = false;
            }
            else
            {
                Query = "Select ExchRate FROM [dbo].[ExchRate] where CurrencyID = '" + cmbCurrency.SelectedItem.ToString() + "' and ";
                Query += "CreatedDate >=DATEADD(day, DATEDIFF(day,0,GETDATE()),0) AND CreatedDate < DATEADD(day, DATEDIFF(day,0,GETDATE())+1,0) ";
                
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.Read())
                {
                    txtExchRate.Enabled = false;
                    txtExchRate.Text = Dr["ExchRate"].ToString();
                }
                else
                {
                    txtExchRate.Text = "1.00";
                    txtExchRate.Enabled = true;
                    MessageBox.Show("Belum Ada Exchange Rate Hari ini");
                    return;
                }
            }
        }

        private void ExchRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void ExchRate_Leave(object sender, EventArgs e)
        {
            if (txtExchRate.Text != "")
            {
                
                double d = double.Parse(txtExchRate.Text);

                if (d == 0)
                {
                    d = double.Parse("1.00");
                }

                txtExchRate.Text = d.ToString("N2");                             
            }
            else
            {
                double d = double.Parse("1.00");
                txtExchRate.Text = d.ToString("N2");
            }
        }

        private void SetPPN()
        {
            cmbPPN.Items.Clear();
            cmbPPN.Items.Add("0.00");
            Conn = ConnectionString.GetConnection();
            Query = "Select [TaxStatusCode], [TaxStatusName], [TaxPercent] From dbo.[TaxGroup] where TaxStatusCode like '%PPN%'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbPPN.Items.Add(Dr["TaxPercent"].ToString());
            }
            Dr.Close();
            Conn.Close();

            cmbPPN.SelectedIndex = 0;
        }

        private void btnLookupCustomer_Click(object sender, EventArgs e)
        {        
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "CustId";
            tmpSearch.Order = "CustId Asc";
            tmpSearch.Table = "[dbo].[CustTable]";
            tmpSearch.QuerySearch = "SELECT a.CustId, a.CustName, Gol_Prsh AS Type, a.PPN, a.NPWP, a.TaxName, a.TaxAddress, CurrencyId, PaymentModeId, TermOfPayment FROM [dbo].[CustTable] a";
            tmpSearch.FilterText = new string[] { "CustId", "CustName", "Type" };
            tmpSearch.Select = new string[] { "CustId", "CustName", "Type", "PPN", "NPWP", "TaxName", "TaxAddress", "CurrencyId", "PaymentModeId", "TermOfPayment" };
            tmpSearch.Hide = new string[] { "PPN", "NPWP", "TaxName", "TaxAddress", "CurrencyId", "PaymentModeId", "TermOfPayment" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtCustID.Text = ConnectionString.Kodes[0];
                txtCustName.Text = ConnectionString.Kodes[1];
                if (ConnectionString.Kodes[3] == "")
                {
                    cmbPPN.SelectedItem = "0.00";
                }
                else
                {
                    cmbPPN.SelectedItem = ConnectionString.Kodes[3];
                }

                if (ConnectionString.Kodes[4] == "")
                {
                    txtNPWP.Text = "00.000.000.0-000.000";
                }
                else
                {
                    txtNPWP.Text = ConnectionString.Kodes[4];
                }
                

                txtTaxName.Text = ConnectionString.Kodes[5];
                txtTaxAddress.Text = ConnectionString.Kodes[6];

                cmbCurrency.SelectedItem = ConnectionString.Kodes[7];
                if (ConnectionString.Kodes[7] == "" || ConnectionString.Kodes[7] == "IDR")
                {
                    cmbCurrency.SelectedItem = "IDR";
                    txtExchRate.Text = "1.00";
                    txtExchRate.Enabled = false;
                }
                else
                {
                    Query = "Select ExchRate FROM [dbo].[ExchRate] where CurrencyID = '" + ConnectionString.Kodes[7] + "' and ";
                    Query += "CreatedDate >=DATEADD(day, DATEDIFF(day,0,GETDATE()),0) AND CreatedDate < DATEADD(day, DATEDIFF(day,0,GETDATE())+1,0) ";

                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.Read())
                    {
                        txtExchRate.Enabled = false;
                        txtExchRate.Text = Dr["ExchRate"].ToString();
                    }
                    else
                    {
                        txtExchRate.Text = "1.00";
                        txtExchRate.Enabled = true;
                    }
                }

                if (ConnectionString.Kodes[8] == "")
                {
                    cmbPaymentMethod.SelectedIndex = 0;
                }
                else
                {
                    cmbPaymentMethod.SelectedItem = ConnectionString.Kodes[8];                
                }
               
                ConnectionString.Kodes = null;

                ClearAllDgv();
                dgvCNReference.DataSource = null;
                GetCreditNote();
                txtSONo.Text = "";
            }        
        }

        private void btnAddAttachment_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
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

                    DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                    combo.Items.Clear();
                    combo.Items.Add("Faktur Pajak");
                    combo.Items.Add("Surat Jalan");
                    combo.Items.Add("Others");
                    combo.Value = "Faktur Pajak";

                    this.dgvAttachment.Rows.Add(dgvAttachment.RowCount+1, "", FileName[i], Extension[i], filesize.ToString(), "", System.Text.Encoding.UTF8.GetString(data));
                    attachByte.Add(data);
                    dgvAttachment.Rows[(dgvAttachment.Rows.Count - 1)].Cells[1] = combo;
                    i++;
                }
            }
        }

        private void setHeaderDgvAttachment()
        {
            //if (dgvAttachment.RowCount - 1 <= 0)
            //{
                dgvAttachment.ColumnCount = 7;
                dgvAttachment.Columns[0].Name = "No";
                dgvAttachment.Columns[1].Name = "FileType";
                dgvAttachment.Columns[2].Name = "FileName";
                dgvAttachment.Columns[3].Name = "ContentType";
                dgvAttachment.Columns[4].Name = "FileSize";
                dgvAttachment.Columns[5].Name = "Id";
                dgvAttachment.Columns[6].Name = "Attachment";

                dgvAttachment.Columns[0].HeaderText = "No";
                dgvAttachment.Columns[1].HeaderText = "File Type";
                dgvAttachment.Columns[2].HeaderText = "File Name";
                dgvAttachment.Columns[3].HeaderText = "Content Type";
                dgvAttachment.Columns[4].HeaderText = "File Size (Kb)";
                dgvAttachment.Columns[5].HeaderText = "Id";
                dgvAttachment.Columns[6].HeaderText = "Attachment";

                dgvAttachment.Columns[0].ReadOnly = true;
                dgvAttachment.Columns[1].ReadOnly = false;
                dgvAttachment.Columns[2].ReadOnly = true;
                dgvAttachment.Columns[3].ReadOnly = true;
                dgvAttachment.Columns[4].ReadOnly = true;
                dgvAttachment.Columns[5].ReadOnly = true;
                dgvAttachment.Columns[6].ReadOnly = true;

                dgvAttachment.Columns[5].Visible = false;
                dgvAttachment.Columns[6].Visible = false;

                dgvAttachment.AutoResizeColumns();
            //}
        }

        private void setHeaderDgvCNReference()
        {
            //if (dgvCNReference.RowCount - 1 <= 0)
            //{
                if (dgvCNReference.Columns.Contains("chk") == false)
                {
                    DataGridViewCheckBoxColumn chk = chk = new DataGridViewCheckBoxColumn();
                    dgvCNReference.Columns.Add(chk);
                    chk.HeaderText = "Check";
                    chk.Name = "chk";                    
                }

                dgvCNReference.ColumnCount = 8;
                dgvCNReference.Columns[1].Name = "No";
                dgvCNReference.Columns[2].Name = "CNNo";
                dgvCNReference.Columns[3].Name = "CNDate";
                dgvCNReference.Columns[4].Name = "Amount";
                dgvCNReference.Columns[5].Name = "AmountDeduct";
                dgvCNReference.Columns[6].Name = "AmountOutstanding";
                dgvCNReference.Columns[7].Name = "PotonganCN";

                dgvCNReference.Columns[2].HeaderText = "CN No";
                dgvCNReference.Columns[3].HeaderText = "CN Date";
                dgvCNReference.Columns[4].HeaderText = "Amount Deduct";
                dgvCNReference.Columns[6].HeaderText = "Amount Outstanding";
                dgvCNReference.Columns[7].HeaderText = "Pot. CN";

                dgvCNReference.Columns["CNDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                for (int i = 0; i <= 7; i++)
                {
                    if (i == 0 || i == 7)
                    {
                        dgvCNReference.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvCNReference.Columns[i].ReadOnly = true;
                    }
                }

                
            //}            
        }

        public void setHeaderDgvSOReferenceDetailsSO()
        {
            //if (dgvSoReferenceDetailsSO.RowCount == 0)
            //{
            //    dgvSoReferenceDetailsSO.Rows.Clear();
                dgvSoReferenceDetailsSO.ColumnCount = 24;
                dgvSoReferenceDetailsSO.Columns[0].Name = "No";
                dgvSoReferenceDetailsSO.Columns[1].Name = "SONo";
                dgvSoReferenceDetailsSO.Columns[2].Name = "SODate";
                dgvSoReferenceDetailsSO.Columns[3].Name = "SODueDate";
                dgvSoReferenceDetailsSO.Columns[4].Name = "SOReference";
                dgvSoReferenceDetailsSO.Columns[5].Name = "DPRequired";
                dgvSoReferenceDetailsSO.Columns[6].Name = "DPPercent";
                dgvSoReferenceDetailsSO.Columns[7].Name = "PPN";
                dgvSoReferenceDetailsSO.Columns[8].Name = "SOAmount";
                dgvSoReferenceDetailsSO.Columns[9].Name = "DPAmount";
                dgvSoReferenceDetailsSO.Columns[10].Name = "DPDeduct";
                dgvSoReferenceDetailsSO.Columns[11].Name = "DPOutstanding";
                dgvSoReferenceDetailsSO.Columns[12].Name = "SOInvoiced";
                dgvSoReferenceDetailsSO.Columns[13].Name = "SOUnInvoiced";
                dgvSoReferenceDetailsSO.Columns[14].Name = "GINo";
                dgvSoReferenceDetailsSO.Columns[15].Name = "GIDate";
                dgvSoReferenceDetailsSO.Columns[16].Name = "GIAmount";
                dgvSoReferenceDetailsSO.Columns[17].Name = "ReturAmount";
                dgvSoReferenceDetailsSO.Columns[18].Name = "POTDP";
                dgvSoReferenceDetailsSO.Columns[19].Name = "GIPayable";
                dgvSoReferenceDetailsSO.Columns[20].Name = "POTDP2";
                dgvSoReferenceDetailsSO.Columns[21].Name = "InvoiceAmount";
                dgvSoReferenceDetailsSO.Columns[22].Name = "InvoiceTaxBaseAmount";
                dgvSoReferenceDetailsSO.Columns[23].Name = "InvoiceTaxAmount";

                dgvSoReferenceDetailsSO.Columns[1].HeaderText = "SO No";
                dgvSoReferenceDetailsSO.Columns[2].HeaderText = "SO Date";
                dgvSoReferenceDetailsSO.Columns[3].HeaderText = "SO DueDate";
                dgvSoReferenceDetailsSO.Columns[4].HeaderText = "SO Reference";
                dgvSoReferenceDetailsSO.Columns[5].HeaderText = "DP Required";
                dgvSoReferenceDetailsSO.Columns[6].HeaderText = "DP Percent";
                dgvSoReferenceDetailsSO.Columns[8].HeaderText = "SO Amount";
                dgvSoReferenceDetailsSO.Columns[9].HeaderText = "DP Amount";
                dgvSoReferenceDetailsSO.Columns[10].HeaderText = "DP Deduct";
                dgvSoReferenceDetailsSO.Columns[11].HeaderText = "DP Outstanding";
                dgvSoReferenceDetailsSO.Columns[12].HeaderText = "SO Invoiced";
                dgvSoReferenceDetailsSO.Columns[13].HeaderText = "SO UnInvoiced";
                dgvSoReferenceDetailsSO.Columns[14].HeaderText = "GI No";
                dgvSoReferenceDetailsSO.Columns[15].HeaderText = "GI Date";
                dgvSoReferenceDetailsSO.Columns[16].HeaderText = "GI Amount";
                dgvSoReferenceDetailsSO.Columns[17].HeaderText = "Retur Amount";
                dgvSoReferenceDetailsSO.Columns[18].HeaderText = "Pot. DP";
                dgvSoReferenceDetailsSO.Columns[19].HeaderText = "GI Payable";
                dgvSoReferenceDetailsSO.Columns[20].HeaderText = "Pot. DP2";
                dgvSoReferenceDetailsSO.Columns[21].HeaderText = "Invoice Amount";

               
                dgvSoReferenceDetailsSO.Columns["InvoiceTaxBaseAmount"].Visible = false;
                dgvSoReferenceDetailsSO.Columns["InvoiceTaxAmount"].Visible = false;
                dgvSoReferenceDetailsSO.Columns["SOReference"].Visible = false;
                dgvSoReferenceDetailsSO.Columns["DPRequired"].Visible = false;
                dgvSoReferenceDetailsSO.Columns["DPPercent"].Visible = false;
                dgvSoReferenceDetailsSO.Columns["PPN"].Visible = false;
                dgvSoReferenceDetailsSO.Columns["DPDeduct"].Visible = false;
                dgvSoReferenceDetailsSO.Columns["GIDate"].Visible = false;

                dgvSoReferenceDetailsSO.Columns["SOAmount"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["DPAmount"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["DPDeduct"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["DPOutstanding"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["SOInvoiced"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["SOUnInvoiced"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["GIAmount"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["ReturAmount"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["POTDP"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["GIPayable"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["POTDP2"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["InvoiceAmount"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["InvoiceTaxBaseAmount"].DefaultCellStyle.Format = "N2";
                dgvSoReferenceDetailsSO.Columns["InvoiceTaxAmount"].DefaultCellStyle.Format = "N2";

                if (cmbInvoiceType.SelectedItem.ToString() != "Invoice")
                {
                    dgvSoReferenceDetailsSO.Columns["GINo"].Visible = false;
                    dgvSoReferenceDetailsSO.Columns["POTDP2"].Visible = false;                   
                }
                else
                {
                    dgvSoReferenceDetailsSO.Columns["GINo"].Visible = true;
                    dgvSoReferenceDetailsSO.Columns["POTDP2"].Visible = true;                   
                }
                
                for (int i = 0; i < 24; i++)
                {
                    if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
                    {
                        dgvSoReferenceDetailsSO.Columns[i].ReadOnly = true;
                    }
                    else if (cmbInvoiceType.SelectedItem.ToString() != "Invoice" && i == 21)
                    {
                        dgvSoReferenceDetailsSO.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvSoReferenceDetailsSO.Columns[i].ReadOnly = true;
                    }
                }              

                dgvSoReferenceDetailsSO.Columns["SODate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvSoReferenceDetailsSO.Columns["SODueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvSoReferenceDetailsSO.Columns["GIDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            //}
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                if (dgvAttachment.CurrentRow.Index > -1)
                {
                    Index = dgvAttachment.CurrentRow.Index;

                    string Id = dgvAttachment.Rows[Index].Cells["Id"].Value.ToString();

                    Conn = ConnectionString.GetConnection();
                    
                    Query = "SELECT CreatedBy FROM tblAttachments WHERE id = '" + Id + "' ";
                    Cmd = new SqlCommand(Query, Conn);
                    string CreatedBy = Convert.ToString(Cmd.ExecuteScalar());

                    Query = "SELECT GroupName FROM sysUserGroup WHERE UserID = '" + CreatedBy + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    string CreatedGroupName = Convert.ToString(Cmd.ExecuteScalar());

                    string DeletedGroupName = ControlMgr.GroupName;

                    if (Id != "")
                    {
                        //Edit New AR
                        if (CreatedGroupName == DeletedGroupName)
                        {
                            DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "No = " + dgvAttachment.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FileName = " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                attachByte.RemoveAt(Index);
                                dgvAttachment.Rows.RemoveAt(Index);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Anda tidak memiliki akses untuk menghapus dokumen : " + Environment.NewLine + "No : " + dgvAttachment.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FileName : " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString());
                            return;
                        }
                    }
                    else
                    {  //Create New AR
                        DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "No = " + dgvAttachment.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FileName = " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            attachByte.RemoveAt(Index);
                            dgvAttachment.Rows.RemoveAt(Index);
                        }
                    }
                    
                    //else
                    //{
                    //    if (CreatedGroupName.ToUpper() == "TAX ADMIN" || CreatedGroupName.ToUpper() == "TAX MANAGER")
                    //    {
                    //        if (DeletedGroupName.ToUpper() == "TAX ADMIN" || DeletedGroupName.ToUpper() == "TAX MANAGER")
                    //        {
                    //            DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "No = " + dgvAttachment.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FileName = " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    //            if (dialogResult == DialogResult.Yes)
                    //            {
                    //                attachByte.RemoveAt(Index);
                    //                dgvAttachment.Rows.RemoveAt(Index);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            MessageBox.Show("Anda tidak memiliki akses untuk menghapus dokumen : " + Environment.NewLine + "No : " + dgvAttachment.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FileName : " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString());
                    //            return;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("Anda tidak memiliki akses untuk menghapus dokumen : " + Environment.NewLine + "No : " + dgvAttachment.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FileName : " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString());
                    //        return;
                    //    }                        
                    //}                                      
                }
                else
                {
                    MessageBox.Show("Silahkan Pilih Data Untuk Dihapus");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Silahkan Pilih Data Untuk Dihapus");
                return;
            }

            SortNoDataGridAttachment();
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
                sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                sfd.AddExtension = true;

                if (ContentType == "pdf")
                {
                    sfd.FilterIndex = 1;
                }
                else
                {
                    sfd.FilterIndex = 2;
                }

                if (String.IsNullOrEmpty(fileid))
                {
                    MessageBox.Show("File tidak ada dalam database / belum di masukkan.");
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
                MessageBox.Show("Silahkan pilih data untuk didownload");
                return;
            }
        }

        private void ValidasiOld()
        {
            if (txtCustID.Text == "")
            {
                MessageBox.Show("Customer ID harus diisi");
                return;
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN")
            {
                if (txtNPWP.Text == "")
                {
                    MessageBox.Show("NPWP harus diisi");
                    return;
                }
                else if (txtTaxNumber.Text == "")
                {
                    MessageBox.Show("Tax Number harus diisi");
                    return;
                }
                else if (txtTaxName.Text == "")
                {
                    MessageBox.Show("Tax Name harus diisi");
                    return;
                }
                else if (txtTaxAddress.Text == "")
                {
                    MessageBox.Show("Tax Address harus diisi");
                    return;
                }
            }

            if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
            {
                if (dgvAttachment.RowCount > 0)
                {
                    int j = 0;
                    for (int i = 0; i < dgvAttachment.RowCount; i++)
                    {
                        if (dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() == "Surat Jalan")
                        {
                            j++;
                        }
                    }

                    if (j == 0)
                    {
                        MessageBox.Show("Documents Surat Jalan harus ada");
                        return;
                    }
                    else
                    {
                        if (j < dgvSoReferenceDetailsSO.RowCount)
                        {
                            MessageBox.Show("Documents Surat Jalan harus sebanyak " + dgvSoReferenceDetailsSO.RowCount + " dokumen");
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Documents Surat Jalan harus ada");
                    return;
                }
            }

            if (dgvSoReferenceDetailsSO.RowCount == 0)
            {
                MessageBox.Show("Customer Invoice details harus diisi");
                return;
            }

            if (Convert.ToDecimal(txtCNAmount.Text) > Convert.ToDecimal(txtInvoiceAmount.Text))
            {
                MessageBox.Show("AR Amount harus lebih dari 0.00");
                return;
            }

            decimal CheckPembulatan;
            bool isNumeric = decimal.TryParse(txtPembulatan.Text, out CheckPembulatan);
            if (isNumeric == false)
            {
                MessageBox.Show("Pembulatan harus bilangan");
                return;
            }

            if (ControlMgr.GroupName.ToUpper() == "AR ADMIN")
            {
                if (cmbPPN.SelectedItem.ToString() == "0.00")
                {
                    TransStatus = "02";
                }
                else
                {
                    TransStatus = "01";
                }
            }  
        }

        private Boolean Validasi()
        {
            decimal CheckPembulatan;
            bool isNumeric = decimal.TryParse(txtPembulatan.Text, out CheckPembulatan);

            int j = 0;
            for (int i = 0; i < dgvAttachment.RowCount; i++)
            {
                if (dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() == "Surat Jalan")
                {
                    j++;
                }
            }

            if (txtCustID.Text == "")
            {
                MessageBox.Show("Customer ID harus diisi");
                return false;
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && txtNPWP.Text.Trim() == "")
            {
                MessageBox.Show("NPWP harus diisi");
                return false;
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && txtNPWP.Text.Trim() != "")
            {
                string NPWP = txtNPWP.Text.Trim();
                int CheckNPWP;
                bool result = true;

                if (!int.TryParse(NPWP.Substring(0, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(1, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (NPWP.Substring(2, 1) != ".")
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(3, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(4, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(5, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (NPWP.Substring(6, 1) != ".")
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(7, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(8, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(9, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (NPWP.Substring(10, 1) != ".")
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(11, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (NPWP.Substring(12, 1) != "-")
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(13, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(14, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(15, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (NPWP.Substring(16, 1) != ".")
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(17, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(18, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (!int.TryParse(NPWP.Substring(19, 1), out CheckNPWP))
                {
                    result = false;
                }
                else if (NPWP.Length != 20)
                {
                    result = false;
                }

                if (result == false)
                {
                    MessageBox.Show("Format NPWP tidak sesuai");
                    txtNPWP.Text = "00.000.000.0-000.000";
                    return false;
                }                
            }



            if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && txtTaxNumber.Text.Trim() == "")
            {
                MessageBox.Show("Tax Number harus diisi");
                return false;
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && txtTaxNumber.Text.Trim() != "")
            {
                string TaxNumber = txtTaxNumber.Text.Trim();
                int CheckTaxNumber;
                bool result = true;

                if (!int.TryParse(TaxNumber.Substring(0, 1), out CheckTaxNumber))
                {
                    result = false;                
                }
                else if (!int.TryParse(TaxNumber.Substring(1, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(2, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (TaxNumber.Substring(3, 1) != ".")
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(4, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(5, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(6, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (TaxNumber.Substring(7, 1) != "-")
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(8, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(9, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (TaxNumber.Substring(10, 1) != ".")
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(11, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(12, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(13, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(14, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(15, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(16, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(17, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (!int.TryParse(TaxNumber.Substring(18, 1), out CheckTaxNumber))
                {
                    result = false;
                }
                else if (TaxNumber.Length != 19)
                {
                    result = false;
                }

                if (result == false)
                {
                    MessageBox.Show("Format Tax Number tidak sesuai");
                    txtTaxNumber.Text = "000.000-00.00000000";
                    txtKetTambahan.Text = "";
                    txtKodeKetTambahan.Text = "";
                    btnLookupKetTambahan.Enabled = false;
                    return false;
                }                
            }



            if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && txtTaxNumber.Text.Trim() != "")
            {
                if (txtTaxNumber.Text.Trim().Substring(0, 2) == "07" && txtKodeKetTambahan.Text == "")
                {
                    MessageBox.Show("Kode Ket Tambahan harus diisi");
                    return false; 
                }               
            }



            if (ControlMgr.GroupName.ToUpper() == "AR ADMIN" && cmbInvoiceType.SelectedItem.ToString() == "Down Payment")
            {
                if (txtDPDescription.Text.Trim() == "")
                {
                    MessageBox.Show("DP Description harus diisi");
                    return false;
                }
            }     
      


            if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && txtTaxName.Text.Trim() == "")
            {
                MessageBox.Show("Tax Name harus diisi");
                return false;
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && txtTaxAddress.Text.Trim() == "")
            {
                MessageBox.Show("Tax Address harus diisi");
                return false;
            }
            else if (dgvSoReferenceDetailsSO.RowCount == 0)
            {
                MessageBox.Show("Customer Invoice details harus diisi");
                return false;
            }
            else if (Convert.ToDecimal(txtCNAmount.Text) > Convert.ToDecimal(txtInvoiceAmount.Text))
            {
                MessageBox.Show("AR Amount harus lebih dari 0.00");
                return false;
            }
            else if (isNumeric == false)
            {
                MessageBox.Show("Pembulatan harus bilangan");
                return false;
            }
            else if (cmbInvoiceType.SelectedItem.ToString() == "Invoice" && dgvAttachment.RowCount > 0 && j == 0)
            {
                MessageBox.Show("Documents Surat Jalan harus ada");
                return false;
            }
            else if (cmbInvoiceType.SelectedItem.ToString() == "Invoice" && dgvAttachment.RowCount > 0 && j < dgvSoReferenceDetailsSO.RowCount)
            {
                MessageBox.Show("Documents Surat Jalan harus sebanyak " + dgvSoReferenceDetailsSO.RowCount + " dokumen");
                return false;
            }
            else if (cmbInvoiceType.SelectedItem.ToString() == "Invoice" && dgvAttachment.RowCount < 1)
            {
                MessageBox.Show("Documents Surat Jalan harus ada");
                return false;
            }
            else
                return true;
        }        

        int SeqNo;
        int SeqNo2;
        int CountData;
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!Validasi())
                return;

            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            //ValidasiOld();  

            txtNPWP.Text = txtNPWP.Text.Replace("'", "").Trim();
            txtTaxNumber.Text = txtTaxNumber.Text.Replace("'", "").Trim();
            txtTaxName.Text = txtTaxName.Text.Replace("'", "").Trim();
            txtTaxAddress.Text = txtTaxAddress.Text.Replace("'", "").Trim();
            txtNotes.Text = txtNotes.Text.Replace("'", "").Trim();

            if (ControlMgr.GroupName.ToUpper() == "AR ADMIN")
            {
                if (cmbPPN.SelectedItem.ToString() == "0.00")
                {
                    TransStatus = "02";
                }
                else
                {
                    TransStatus = "01";
                }
            }

            try
            {
                if (Mode == "New")
                {                    
                    InsertCustomerInvoiceH();                    
                    InsertCustomerInvoiceD();
                    InsertCreditNote();
                    InsertAttachment();
                    InsertLog(InvoiceId, "CustInvoice", TransStatus, "N", Conn, Trans, Cmd);
                    Trans.Commit();
                    txtInvoiceId.Text = InvoiceId;

                    MessageBox.Show("Data Invoice No : " + InvoiceId + " berhasil disimpan.");
                }
                else //UPDATE Invoice Customer
                { 
                    SeqNo = 1;
                    SeqNo2 = 1;

                    //UPDATE
                    Query = "SELECT COUNT(Invoice_Id) FROM CustInvoice_H WHERE Invoice_Id = '" + txtInvoiceId.Text+ "' AND Invoice_Type = '"+ cmbInvoiceType.SelectedItem.ToString() +"'";
                    Cmd = new SqlCommand(Query, Conn, Trans); 
                    CountData = Convert.ToInt32(Cmd.ExecuteScalar());
                   
                    // Invoice Type Changed
                    if (CountData == 0)
                    {
                        UpdateInvoiceTypeChanged();
                    }
                    else
                    {
                        UpdateInvoiceTypeNotChanged();
                    }

                    UpdateHeader();
                    UpdateCreditNote();
                    UpdateAttachment();
                    InsertLog(InvoiceId, "CustInvoice", TransStatus, "E", Conn, Trans, Cmd);
                    Trans.Commit();
                    txtInvoiceId.Text = InvoiceId;

                    MessageBox.Show("Data Invoice No : " + InvoiceId + " berhasil diubah.");
                    //END UPDATE
                }

                Parent.RefreshGrid();
                ModeBeforeEdit();
            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();              
            }
        }

        private void InsertLog(string TransaksiID, string TransCode, string StatusCode, string Action, SqlConnection Conn, SqlTransaction Trans,SqlCommand Cmd)
        {
            Query = "SELECT Deskripsi FROM TransStatusTable WHERE TransCode = '" + TransCode + "' AND StatusCode = '"+ StatusCode +  "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string StatusTransaksi = Convert.ToString(Cmd.ExecuteScalar());

            if (Action == "")
            {
                Query = "SELECT TOP 1 Action FROM CustInvoice_LogTable WHERE TransaksiID = '" + TransaksiID + "' ORDER BY LogDatetime DESC";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Action = Convert.ToString(Cmd.ExecuteScalar());
            }

            Query = "INSERT INTO CustInvoice_LogTable (TransaksiID, StatusTransaksi, Action, UserID, LogDatetime) ";
            Query += "VALUES ('" + TransaksiID + "', '" + StatusTransaksi + "', '" + Action + "', '" + ControlMgr.UserId + "', GETDATE())";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
        }

        private void InsertCustomerInvoiceH()
        {
            //Insert Customer Invoice Header
            string Jenis = "", Kode = "";
            if (cmbInvoiceType.SelectedItem.ToString() != "Proforma")
            {
                Jenis = "ARIN"; Kode = "ARIN";
            }
            else
            {
                Jenis = "ARPN"; Kode = "ARPN";
                TransStatus = "02";
            }

            InvoiceId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

            if (txtTaxNumber.Text == "")
            {
                txtTaxNumber.Text = "000.000-00.00000000";
            }

            //if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment" && txtProformaID.Text != "")
            //{
            //    Query = "INSERT INTO CustInvoice_H(Invoice_Date, Invoice_Id, Invoice_Type, Cust_Id, Cust_Name, PPN_Percent, ";
            //    Query += "DP_Amount, Invoice_Round_Amount, Invoice_Amount, Invoice_Tax_Base_Amount, Invoice_Tax_Amount, ";
            //    Query += "Payment_Method, Invoice_DueDate, NPWP, TaxDate, TaxNum, TaxName, TaxAddress, Notes, CN_Amount, AR_Amount, Settle_Amount, CurrencyID, ExchRate, TransStatus, CreatedDate, CreatedBy, Proforma_Id, Settle_Invoice_DP) ";
            //    Query += "VALUES('" + dtInvoiceDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + InvoiceId + "', '" + cmbInvoiceType.SelectedItem.ToString() + "', '" + txtCustID.Text + "', ";
            //    Query += "'" + txtCustName.Text + "', '" + cmbPPN.SelectedItem.ToString() + "', " + Convert.ToDecimal(txtDPAmount.Text) + ", " + Convert.ToDecimal(txtPembulatan.Text) + ", ";
            //    Query += "" + Convert.ToDecimal(txtInvoiceAmount.Text) + ", " + Convert.ToDecimal(txtDPPAmount.Text) + ", " + Convert.ToDecimal(txtTaxAmount.Text) + ", '" + cmbPaymentMethod.SelectedItem.ToString() + "', '" + dtPaymentDueDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
            //    Query += "'" + txtNPWP.Text + "','" + dtTaxDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txtTaxNumber.Text + "', '" + txtTaxName.Text + "', '" + txtTaxAddress.Text + "', '" + txtNotes.Text + "', ";
            //    Query += "" + Convert.ToDecimal(txtCNAmount.Text) + ", " + Convert.ToDecimal(txtARAmount.Text) + ", 0, '" + cmbCurrency.SelectedItem.ToString() + "', " + Convert.ToDecimal(txtExchRate.Text) + ", '" + TransStatus + "', GETDATE(), '" + ControlMgr.UserId + "', '" + txtProformaID.Text + "', (SELECT Settle_Invoice_DP FROM CustInvoice_H WHERE Invoice_Id = '"+txtProformaID.Text+"') + " + Convert.ToDecimal(txtInvoiceAmount.Text) + ") ";
            //}
            //else
            //{
            //    Query = "INSERT INTO CustInvoice_H(Invoice_Date, Invoice_Id, Invoice_Type, Cust_Id, Cust_Name, PPN_Percent, ";
            //    Query += "DP_Amount, Invoice_Round_Amount, Invoice_Amount, Invoice_Tax_Base_Amount, Invoice_Tax_Amount, ";
            //    Query += "Payment_Method, Invoice_DueDate, NPWP, TaxDate, TaxNum, TaxName, TaxAddress, Notes, CN_Amount, AR_Amount, Settle_Amount, CurrencyID, ExchRate, TransStatus, CreatedDate, CreatedBy) ";
            //    Query += "VALUES('" + dtInvoiceDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + InvoiceId + "', '" + cmbInvoiceType.SelectedItem.ToString() + "', '" + txtCustID.Text + "', ";
            //    Query += "'" + txtCustName.Text + "', '" + cmbPPN.SelectedItem.ToString() + "', " + Convert.ToDecimal(txtDPAmount.Text) + ", " + Convert.ToDecimal(txtPembulatan.Text) + ", ";
            //    Query += "" + Convert.ToDecimal(txtInvoiceAmount.Text) + ", " + Convert.ToDecimal(txtDPPAmount.Text) + ", " + Convert.ToDecimal(txtTaxAmount.Text) + ", '" + cmbPaymentMethod.SelectedItem.ToString() + "', '" + dtPaymentDueDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
            //    Query += "'" + txtNPWP.Text + "','" + dtTaxDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txtTaxNumber.Text + "', '" + txtTaxName.Text + "', '" + txtTaxAddress.Text + "', '" + txtNotes.Text + "', ";
            //    Query += "" + Convert.ToDecimal(txtCNAmount.Text) + ", " + Convert.ToDecimal(txtARAmount.Text) + ", 0, '" + cmbCurrency.SelectedItem.ToString() + "', " + Convert.ToDecimal(txtExchRate.Text) + ", '" + TransStatus + "', GETDATE(), '" + ControlMgr.UserId + "') ";    
            //}

            if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment" && txtProformaID.Text != "")
            {
                Query = "INSERT INTO CustInvoice_H ";
                Query += "(Invoice_Date, Invoice_Id, Invoice_Type, Cust_Id, Cust_Name, PPN_Percent, ";
                Query += "Payment_Method, Invoice_DueDate, NPWP, TaxDate, TaxNum, TaxName, TaxAddress, Notes, ";
                Query += "DP_Amount, Invoice_Round_Amount, Invoice_Amount, Invoice_Tax_Base_Amount, Invoice_Tax_Amount, ";
                Query += "CN_Amount, AR_Amount, Settle_Amount, ExchRate, ";
                Query += "CurrencyID, TransStatus, CreatedDate, CreatedBy, ";
                Query += "Proforma_Id, Settle_Invoice_DP, SalesOrderNo, KodeKetTambahan, DPDescription) VALUES ";

                Query += "(@invdate, @invid, @invtype, @custid, @custname, @ppn, ";
                Query += "@paymentmethod, @invduedate, @npwp , @taxdate, @taxnum, @taxname, @taxaddress, @notes, ";
                Query += "" + Convert.ToDecimal(txtDPAmount.Text) + ",";
                Query += "" + Convert.ToDecimal(txtPembulatan.Text) + ","; //inv round amt
                Query += "" + Convert.ToDecimal(txtInvoiceAmount.Text) + ",";
                Query += "" + Convert.ToDecimal(txtDPPAmount.Text) + ","; //tax base
                Query += "" + Convert.ToDecimal(txtTaxAmount.Text) + ",";
                Query += "" + Convert.ToDecimal(txtCNAmount.Text) + ",";
                Query += "" + Convert.ToDecimal(txtARAmount.Text) + ",";
                Query += "0,"; //settle amount
                Query += "" + Convert.ToDecimal(txtExchRate.Text) + "',";
                Query += "@currid, @transstatus, GETDATE(), @UserId , @proformaid,";
                Query += "(SELECT Settle_Invoice_DP FROM CustInvoice_H WHERE Invoice_Id = @proformaid) + " + Convert.ToDecimal(txtInvoiceAmount.Text) + ", '" + txtSONo.Text + "', '" + txtKodeKetTambahan.Text + "', '" + txtDPDescription.Text + "') "; //settle invoice
            }
            else
            {
                Query = "INSERT INTO CustInvoice_H ";
                Query += "(Invoice_Date, Invoice_Id, Invoice_Type, Cust_Id, Cust_Name, PPN_Percent, ";
                Query += "Payment_Method, Invoice_DueDate, NPWP, TaxDate, TaxNum, TaxName, TaxAddress, Notes, ";
                Query += "DP_Amount, Invoice_Round_Amount, Invoice_Amount, Invoice_Tax_Base_Amount, Invoice_Tax_Amount, ";
                Query += "CN_Amount, AR_Amount, Settle_Amount, ExchRate, ";
                Query += "CurrencyID, TransStatus, CreatedDate, CreatedBy, SalesOrderNo, KodeKetTambahan, DPDescription) VALUES ";

                Query += "(@invdate, @invid, @invtype, @custid, @custname, @ppn, ";
                Query += "@paymentmethod, @invduedate, @npwp , @taxdate, @taxnum, @taxname, @taxaddress, @notes, ";
                Query += "" + Convert.ToDecimal(txtDPAmount.Text) + ",";
                Query += "" + Convert.ToDecimal(txtPembulatan.Text) + ","; //inv round amt
                Query += "" + Convert.ToDecimal(txtInvoiceAmount.Text) + ",";
                Query += "" + Convert.ToDecimal(txtDPPAmount.Text) + ","; //tax base
                Query += "" + Convert.ToDecimal(txtTaxAmount.Text) + ",";
                Query += "" + Convert.ToDecimal(txtCNAmount.Text) + ",";
                Query += "" + Convert.ToDecimal(txtARAmount.Text) + ",";
                Query += "0,"; //settle amount
                Query += "'" + Convert.ToDecimal(txtExchRate.Text) + "',";
                Query += "@currid, @transstatus, GETDATE(), @UserId, '" + txtSONo.Text + "', '" + txtKodeKetTambahan.Text + "', '" + txtDPDescription.Text + "')";
            }

            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.Parameters.AddWithValue("@invdate", dtInvoiceDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            Cmd.Parameters.AddWithValue("@invid", InvoiceId);
            Cmd.Parameters.AddWithValue("@invtype", cmbInvoiceType.SelectedItem.ToString());
            Cmd.Parameters.AddWithValue("@custid", txtCustID.Text);
            Cmd.Parameters.AddWithValue("@custname", txtCustName.Text);
            Cmd.Parameters.AddWithValue("@ppn", cmbPPN.SelectedItem.ToString());
            Cmd.Parameters.AddWithValue("@paymentmethod", cmbPaymentMethod.SelectedItem.ToString());
            Cmd.Parameters.AddWithValue("@invduedate", dtPaymentDueDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            Cmd.Parameters.AddWithValue("@npwp", txtNPWP.Text);
            Cmd.Parameters.AddWithValue("@taxdate", dtTaxDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            Cmd.Parameters.AddWithValue("@taxnum", txtTaxNumber.Text);
            Cmd.Parameters.AddWithValue("@taxname", txtTaxName.Text);
            Cmd.Parameters.AddWithValue("@taxaddress", txtTaxAddress.Text);
            Cmd.Parameters.AddWithValue("@notes", txtNotes.Text);
            Cmd.Parameters.AddWithValue("@currid", cmbCurrency.SelectedItem.ToString());
            Cmd.Parameters.AddWithValue("@transstatus", TransStatus);
            Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
            Cmd.Parameters.AddWithValue("@proformaid", txtProformaID.Text);
            Cmd.ExecuteNonQuery();
            //End Insert Customer Invoice Header
        }

        private void InsertCustomerInvoiceD()
        {
            //Insert Customer Invoice Details
            //IF Invoice Type = INVOICE
            if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
            {
                SeqNo = 1;
                SeqNo2 = 1;
                for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                {
                    string SODate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["SODate"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    string SODueDate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["SODueDate"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    string GIDate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["GIDate"].Value.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    //Insert Detail
                    //Query = "INSERT INTO CustInvoice_Dtl_SO(Invoice_Id, SeqNo, So_No, SO_Date, SO_DueDate, SO_Amount, GI_No, GI_Date, ";
                    //Query += "GI_Amount, Retur_Amount, PotDP, GI_Payable, PotDP2, Invoice_Amount, Invoice_Tax_Base_Amount, Invoice_Tax_Amount) ";
                    //Query += "VALUES('" + InvoiceId + "', '" + SeqNo + "', '" + dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString() + "', '" + SODate + "', ";
                    //Query += "'" + SODueDate + "', " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["SOAmount"].Value.ToString()) + ", '" + dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString() + "', ";
                    //Query += "'" + GIDate + "', " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIAmount"].Value.ToString()) + ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["ReturAmount"].Value.ToString()) + ", ";
                    //Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString()) + ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIPayable"].Value.ToString()) + ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP2"].Value.ToString()) + ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString()) + ", ";
                    //Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString()) + ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString()) +" ";
                    //Query += ")";

                    Query = "INSERT INTO CustInvoice_Dtl_SO(Invoice_Id, SeqNo, So_No, SO_Date, SO_DueDate, SO_Amount, GI_No, GI_Date, ";
                    Query += "GI_Amount, Retur_Amount, PotDP, GI_Payable, PotDP2, Invoice_Amount, Invoice_Tax_Base_Amount, Invoice_Tax_Amount) ";
                    Query += "VALUES(@invoiceid, @seqno, @SOno, @SOdate,@SOduedate, ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["SOAmount"].Value.ToString()) + ",";
                    Query += "@GIno,@GIdate,";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIAmount"].Value.ToString()) + "";
                    Query += ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["ReturAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIPayable"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP2"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString()) + " ";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);

                    Cmd.Parameters.AddWithValue("@invoiceid", InvoiceId);
                    Cmd.Parameters.AddWithValue("@seqno", SeqNo);
                    Cmd.Parameters.AddWithValue("@SOno", dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@SOdate", SODate);
                    Cmd.Parameters.AddWithValue("@SOduedate", SODueDate);
                    Cmd.Parameters.AddWithValue("@GIno", dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@GIdate", GIDate);

                    Cmd.ExecuteNonQuery();
                    //End Insert Detail

                    //Insert Item
                    string GINo = dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString();
                    string PotDP = dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString();
                    string PotDP2 = dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP2"].Value.ToString();
                    string SONo = dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString();
                    string Line_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString();
                    string Line_Tax_Base_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString();
                    string Line_Tax_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString();
                    Query = "SELECT * FROM vGI_Item WHERE GINo = '" + GINo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        //Query = "INSERT INTO CustInvoice_Dtl_SO_Dtl(Invoice_Id, SeqNo, SO_No, GI_No, FullItemId, ";
                        //Query += "Item_Name, GI_Qty, Retur_Qty, Qty, Unit, Price, Ratio, Total, Total_Disc, ";
                        //Query += "PPN_Percent, Line_Amount, Line_Tax_Base_Amount, Line_Tax_Amount, SO_SeqNo, ";
                        //Query += "GI_SeqNo, CreatedDate, CreatedBy) VALUES( ";
                        //Query += "'" + InvoiceId + "', '" + SeqNo2 + "', '" + SONo + "', '" + GINo + "', '" + Dr["FullItemId"].ToString() + "', ";
                        //Query += "'" + Dr["ItemName"].ToString() + "', " + Convert.ToDecimal(Dr["GIQty"].ToString()) + ", " + Convert.ToDecimal(Dr["ReturQty"].ToString()) + ", ";
                        //Query += "" + Convert.ToDecimal(Dr["SOQty"].ToString()) + ", '" + Dr["Unit"].ToString() + "', " + Convert.ToDecimal(Dr["Price"].ToString()) + ", ";
                        //Query += "" + Convert.ToDecimal(Dr["Ratio"].ToString()) + ", " + Convert.ToDecimal(Dr["Total"].ToString()) + ", " + Convert.ToDecimal(Dr["TotalDisc"].ToString()) + ", ";
                        //Query += "" + Convert.ToDecimal(Dr["PPN"].ToString()) + ", " + Convert.ToDecimal(Line_Amount) + ", " + Convert.ToDecimal(Line_Tax_Base_Amount) + ", " + Convert.ToDecimal(Line_Tax_Amount) + ", ";
                        //Query += "'" + Dr["SOSeqNo"].ToString() + "', '" + Dr["GISeqNo"].ToString() + "', GETDATE(), '" + ControlMgr.UserId + "'";
                        //Query += ")";

                        Query = "INSERT INTO CustInvoice_Dtl_SO_Dtl(Invoice_Id, SeqNo, SO_No, GI_No, FullItemId, ";
                        Query += "Item_Name, GI_Qty, Retur_Qty, Qty, Unit, Price, Ratio, Total, Total_Disc, ";
                        Query += "PPN_Percent, Line_Amount, Line_Tax_Base_Amount, Line_Tax_Amount, SO_SeqNo, ";
                        Query += "GI_SeqNo, CreatedDate, CreatedBy) VALUES( ";

                        Query += "@invoiceid, @seqno2, @SOno, @GIno, ";
                        Query += "'" + Dr["FullItemId"].ToString() + "', ";
                        Query += "'" + Dr["ItemName"].ToString() + "',";
                        Query += "" + Convert.ToDecimal(Dr["GIQty"].ToString()) + ",";
                        Query += "" + Convert.ToDecimal(Dr["ReturQty"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["SOQty"].ToString()) + ", ";
                        Query += "'" + Dr["Unit"].ToString() + "', ";
                        Query += "" + Convert.ToDecimal(Dr["Price"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["Ratio"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["Total"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["TotalDisc"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["PPN"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Amount) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Tax_Base_Amount) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Tax_Amount) + ", ";
                        Query += "'" + Dr["SOSeqNo"].ToString() + "', ";
                        Query += "'" + Dr["GISeqNo"].ToString() + "', ";
                        Query += "GETDATE(), '" + ControlMgr.UserId + "'";
                        Query += ")";

                        Cmd = new SqlCommand(Query, Conn, Trans);

                        Cmd.Parameters.AddWithValue("@invoiceid", InvoiceId);
                        Cmd.Parameters.AddWithValue("@seqno2", SeqNo2);
                        Cmd.Parameters.AddWithValue("@GIno", dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@SOno", dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString());

                        Cmd.ExecuteNonQuery();

                        SeqNo2++;
                    }
                    Dr.Close();
                    //End Insert Item  

                    //Update GI
                    Query = "UPDATE GoodsIssuedH SET InvoiceNo = '" + InvoiceId + "' WHERE GoodsIssuedId = '" + GINo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Update GI 

                    //Update DP Master
                    if (Convert.ToDecimal(PotDP) > 0)
                    {
                        Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct + " + Convert.ToDecimal(PotDP) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }

                    if (Convert.ToDecimal(PotDP2) > 0)
                    {
                        Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct + " + Convert.ToDecimal(PotDP2) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    //End Update DP Master

                    SeqNo++;
                }

            } //END IF Invoice Type = INVOICE
            else
            {
                //Invoice Type = DP
                SeqNo = 1;
                for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                {
                    Query = "INSERT INTO CustInvoice_Dtl_DP(Invoice_Id, SeqNo, So_No, Line_Amount, Line_Tax_Base_Amount, Tax_Percent, Line_Tax_Amount, Deduct, CreatedBy, CreatedDate) VALUES( ";
                    Query += "'" + InvoiceId + "', '" + SeqNo + "', '" + dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString() + "', ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString()) + ",";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["PPN"].Value.ToString()) + ",";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString()) + ", 0, ";
                    Query += "'" + ControlMgr.UserId + "', GETDATE()";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                //END Invoice Type = DP
            }
            //End Insert Customer Invoice Details
        }

        private void InsertCreditNote()
        {
            //Insert Credit Note
            SeqNo = 1;
            for (int i = 0; i < dgvCNReference.RowCount; i++)
            {
                string CNDate = DateTime.ParseExact(dgvCNReference.Rows[i].Cells["CNDate"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                Boolean Check = Convert.ToBoolean(dgvCNReference.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    Query = "INSERT INTO CustInvoice_Dtl_CreditNote (Invoice_Id, SeqNo, CN_No, CN_Date, Amount, Pot_CN, CreatedBy, CreatedDate) VALUES( ";
                    Query += "'" + InvoiceId + "', '" + SeqNo + "', '" + dgvCNReference.Rows[i].Cells["CNNo"].Value.ToString() + "', '" + CNDate + "', ";
                    Query += "" + Convert.ToDecimal(dgvCNReference.Rows[i].Cells["Amount"].Value.ToString()) + ",";
                    Query += "" + Convert.ToDecimal(dgvCNReference.Rows[i].Cells["PotonganCN"].Value.ToString()) + ", ";
                    Query += "'" + ControlMgr.UserId + "', GETDATE()";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //Update CN
                    Query = "UPDATE NotaCreditH SET Deduct = Deduct +  " + Convert.ToDecimal(dgvCNReference.Rows[i].Cells["PotonganCN"].Value.ToString()) + " WHERE CN_No = '" + dgvCNReference.Rows[i].Cells["CNNo"].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Update CN
                    SeqNo++;
                }
            }
            //End Insert Credit Note  
        }

        private void InsertAttachment()
        {
            //Insert Attachement
            for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, FileType, fileName, ContentType, fileSize, attachment, CreatedBy, CreatedDate) Values";
                Query += "( 'CustInvoiceH', '" + InvoiceId + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileSize"].Value.ToString();
                Query += "',@binaryValue, '" + ControlMgr.UserId + "', GETDATE()";
                Query += ");";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, attachByte[i].Length).Value = attachByte[i];
                Cmd.ExecuteNonQuery();
            }
            //End Insert Attachement
        }

        private void UpdateInvoiceTypeChanged()
        {
            if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
            {
                Query = "DELETE FROM CustInvoice_Dtl_DP WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                Query = "SELECT Proforma_Id FROM CustInvoice_H WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                string ProformaId = Convert.ToString(Cmd.ExecuteScalar());

                if (ProformaId != "")
                {
                    Query = "UPDATE CustInvoice_H SET Proforma_Id = '', Settle_Invoice_DP = Settle_Invoice_DP - Invoice_Amount WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }

                SeqNo = 1;
                SeqNo2 = 1;
                for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                {
                    string SODate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["SODate"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    string SODueDate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["SODueDate"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    string GIDate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["GIDate"].Value.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    //string SODate = dgvSoReferenceDetailsSO.Rows[i].Cells["SODate"].Value.ToString();
                    //string SODueDate = dgvSoReferenceDetailsSO.Rows[i].Cells["SODueDate"].Value.ToString();
                    //string GIDate = dgvSoReferenceDetailsSO.Rows[i].Cells["GIDate"].Value.ToString();

                    //Insert Detail
                    Query = "INSERT INTO CustInvoice_Dtl_SO(Invoice_Id, SeqNo, So_No, SO_Date, SO_DueDate, SO_Amount, GI_No, GI_Date, ";
                    Query += "GI_Amount, Retur_Amount, PotDP, GI_payable, PotDP2, Invoice_Amount, Invoice_Tax_Base_Amount, Invoice_Tax_Amount) ";
                    Query += "VALUES('" + InvoiceId + "', '" + SeqNo + "', '" + dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString() + "', '" + SODate + "', ";
                    Query += "'" + SODueDate + "', " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["SOAmount"].Value.ToString()) + ",";
                    Query += "'" + dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString() + "', ";
                    Query += "'" + GIDate + "', " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["ReturAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIPayable"].Value.ToString()) + ",";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP2"].Value.ToString()) + ",";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString()) + " ";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail

                    //Insert Item
                    string GINo = dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString();
                    string PotDP = dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString();
                    string PotDP2 = dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP2"].Value.ToString();
                    string SONo = dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString();
                    string Line_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString();
                    string Line_Tax_Base_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString();
                    string Line_Tax_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString();
                    Query = "SELECT * FROM vGI_Item WHERE GINo = '" + GINo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        Query = "INSERT INTO CustInvoice_Dtl_SO_Dtl(Invoice_Id, SeqNo, SO_No, GI_No, FullItemId, ";
                        Query += "Item_Name, GI_Qty, Retur_Qty, Qty, Unit, Price, Ratio, Total, Total_Disc, ";
                        Query += "PPN_Percent, Line_Amount, Line_Tax_Base_Amount, Line_Tax_Amount, SO_SeqNo, ";
                        Query += "GI_SeqNo, CreatedDate, CreatedBy) VALUES( ";
                        Query += "'" + InvoiceId + "', '" + SeqNo2 + "', '" + SONo + "', '" + GINo + "', '" + Dr["FullItemId"].ToString() + "', ";
                        Query += "'" + Dr["ItemName"].ToString() + "', ";
                        Query += "" + Convert.ToDecimal(Dr["GIQty"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["ReturQty"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["SOQty"].ToString()) + ",";
                        Query += "'" + Dr["Unit"].ToString() + "', ";
                        Query += "" + Convert.ToDecimal(Dr["Price"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["Ratio"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["Total"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["TotalDisc"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["PPN"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Amount) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Tax_Base_Amount) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Tax_Amount) + ", ";
                        Query += "'" + Dr["SOSeqNo"].ToString() + "', '" + Dr["GISeqNo"].ToString() + "', GETDATE(), '" + ControlMgr.UserId + "'";
                        Query += ")";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        SeqNo2++;
                    }
                    Dr.Close();
                    //End Insert Item  

                    //Update GI
                    Query = "UPDATE GoodsIssuedH SET InvoiceNo = '" + InvoiceId + "', UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsIssuedId = '" + GINo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Update GI 

                    //Update DP Master
                    if (Convert.ToDecimal(PotDP) > 0)
                    {
                        Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct + " + Convert.ToDecimal(PotDP) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }

                    if (Convert.ToDecimal(PotDP2) > 0)
                    {
                        Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct + " + Convert.ToDecimal(PotDP2) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    //End Update DP Master

                    SeqNo++;
                }
            }
            else
            { //Type DP
                Query = "SELECT GI_No, PotDP, SO_No, PotDP2 FROM CustInvoice_Dtl_SO WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string GINo = Dr["GI_No"].ToString();
                    string Query2 = "UPDATE GoodsIssuedH SET InvoiceNo = '', UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsIssuedId = '" + GINo + "'";
                    Cmd2 = new SqlCommand(Query2, Conn, Trans);
                    Cmd2.ExecuteNonQuery();

                    string PotDP = Dr["PotDP"].ToString();
                    string PotDP2 = Dr["PotDP2"].ToString();
                    string SONo = Dr["SO_No"].ToString();
                    //Update DP Master
                    if (Convert.ToDecimal(PotDP) > 0)
                    {
                        Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct - " + Convert.ToDecimal(PotDP) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }

                    if (Convert.ToDecimal(PotDP2) > 0)
                    {
                        Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct - " + Convert.ToDecimal(PotDP2) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    //End Update DP Master
                }
                Dr.Close();

                Query = "DELETE FROM CustInvoice_Dtl_SO WHERE Invoice_Id = '" + txtInvoiceId.Text + "';";
                Query += "DELETE FROM CustInvoice_Dtl_SO_Dtl WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                //Insert Detail
                SeqNo = 1;
                for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                {
                    Query = "INSERT INTO CustInvoice_Dtl_DP(Invoice_Id, SeqNo, So_No, Line_Amount, Line_Tax_Base_Amount, Tax_Percent, Line_Tax_Amount, Deduct, CreatedBy, CreatedDate) VALUES( ";
                    Query += "'" + InvoiceId + "', '" + SeqNo + "', '" + dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString() + "', ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["PPN"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString()) + ", 0, ";
                    Query += "'" + ControlMgr.UserId + "', GETDATE()";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                //Insert Detail
            }
        }

        private void UpdateInvoiceTypeNotChanged()
        {
            //if Invoice Type don't changed

            if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
            {
                Query = "SELECT GI_No, PotDP, SO_No, PotDP2 FROM CustInvoice_Dtl_SO WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string GINo = Dr["GI_No"].ToString();
                    string Query2 = "UPDATE GoodsIssuedH SET InvoiceNo = '', UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsIssuedId = '" + GINo + "'";
                    Cmd2 = new SqlCommand(Query2, Conn, Trans);
                    Cmd2.ExecuteNonQuery();

                    string PotDP = Dr["PotDP"].ToString();
                    string PotDP2 = Dr["PotDP2"].ToString();
                    string SONo = Dr["SO_No"].ToString();

                    //Update DP Master
                    if (Convert.ToDecimal(PotDP) > 0)
                    {
                        Query2 = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct - " + Convert.ToDecimal(PotDP) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd2 = new SqlCommand(Query2, Conn, Trans);
                        Cmd2.ExecuteNonQuery();
                    }

                    if (Convert.ToDecimal(PotDP2) > 0)
                    {
                        Query2 = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct - " + Convert.ToDecimal(PotDP2) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd2 = new SqlCommand(Query2, Conn, Trans);
                        Cmd2.ExecuteNonQuery();
                    }
                    //End Update DP Master
                }
                Dr.Close();

                Query = "DELETE FROM CustInvoice_Dtl_SO WHERE Invoice_Id = '" + txtInvoiceId.Text + "';";
                Query += "DELETE FROM CustInvoice_Dtl_SO_Dtl WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                SeqNo = 1;
                SeqNo2 = 1;
                for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                {
                    //string SODate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["SODate"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    //string SODueDate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["SODueDate"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    //string GIDate = DateTime.ParseExact(dgvSoReferenceDetailsSO.Rows[i].Cells["GIDate"].Value.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    string SODate = dgvSoReferenceDetailsSO.Rows[i].Cells["SODate"].Value.ToString();
                    string SODueDate = dgvSoReferenceDetailsSO.Rows[i].Cells["SODueDate"].Value.ToString();
                    string GIDate = dgvSoReferenceDetailsSO.Rows[i].Cells["GIDate"].Value.ToString();


                    //Insert Detail
                    Query = "INSERT INTO CustInvoice_Dtl_SO(Invoice_Id, SeqNo, So_No, SO_Date, SO_DueDate, SO_Amount, GI_No, GI_Date, ";
                    Query += "GI_Amount, Retur_Amount, PotDP, GI_Payable, PotDP2, Invoice_Amount, Invoice_Tax_Base_Amount, Invoice_Tax_Amount) ";
                    Query += "VALUES('" + InvoiceId + "', '" + SeqNo + "', '" + dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString() + "', '" + SODate + "', ";
                    Query += "'" + SODueDate + "', ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["SOAmount"].Value.ToString()) + ", ";
                    Query += "'" + dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString() + "', ";
                    Query += "'" + GIDate + "', " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["ReturAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIPayable"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP2"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString()) + ", ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString()) + " ";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Detail

                    //Insert Item
                    string GINo = dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString();
                    string PotDP = dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString();
                    string PotDP2 = dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP2"].Value.ToString();
                    string SONo = dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString();
                    string Line_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString();
                    string Line_Tax_Base_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString();
                    string Line_Tax_Amount = dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString();
                    Query = "SELECT * FROM vGI_Item WHERE GINo = '" + GINo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        Query = "INSERT INTO CustInvoice_Dtl_SO_Dtl(Invoice_Id, SeqNo, SO_No, GI_No, FullItemId, ";
                        Query += "Item_Name, GI_Qty, Retur_Qty, Qty, Unit, Price, Ratio, Total, Total_Disc, ";
                        Query += "PPN_Percent, Line_Amount, Line_Tax_Base_Amount, Line_Tax_Amount, SO_SeqNo, ";
                        Query += "GI_SeqNo, CreatedDate, CreatedBy) VALUES( ";
                        Query += "'" + InvoiceId + "', '" + SeqNo2 + "', '" + SONo + "', '" + GINo + "', '" + Dr["FullItemId"].ToString() + "', ";
                        Query += "'" + Dr["ItemName"].ToString() + "', ";
                        Query += "" + Convert.ToDecimal(Dr["GIQty"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["ReturQty"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["SOQty"].ToString()) + ", ";
                        Query += "'" + Dr["Unit"].ToString() + "', ";
                        Query += "" + Convert.ToDecimal(Dr["Price"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["Ratio"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["Total"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["TotalDisc"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Dr["PPN"].ToString()) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Amount) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Tax_Base_Amount) + ", ";
                        Query += "" + Convert.ToDecimal(Line_Tax_Amount) + ", ";
                        Query += "'" + Dr["SOSeqNo"].ToString() + "', '" + Dr["GISeqNo"].ToString() + "', GETDATE(), '" + ControlMgr.UserId + "'";
                        Query += ")";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        SeqNo2++;
                    }
                    Dr.Close();
                    //End Insert Item  

                    //Update GI
                    Query = "UPDATE GoodsIssuedH SET InvoiceNo = '" + InvoiceId + "', UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE GoodsIssuedId = '" + GINo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Update GI    

                    Decimal POTDP = Convert.ToDecimal(PotDP);
                    Decimal POTDP2 = Convert.ToDecimal(PotDP2);
                    //Update DP Master
                    if (Convert.ToDecimal(PotDP) > 0)
                    {
                        Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct + " + POTDP + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }

                    if (Convert.ToDecimal(PotDP2) > 0)
                    {
                        Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct + " + POTDP2 + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    //End Update DP Master

                    SeqNo++;
                }
            }
            else
            {
                //Invoice Type DP
                Query = "DELETE FROM CustInvoice_Dtl_DP WHERE Invoice_Id = '" + txtInvoiceId.Text + "';";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                //Insert Detail
                SeqNo = 1;
                for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                {
                    Query = "INSERT INTO CustInvoice_Dtl_DP(Invoice_Id, SeqNo, So_No, Line_Amount, Line_Tax_Base_Amount, Tax_Percent, Line_Tax_Amount, Deduct, CreatedBy, CreatedDate) VALUES( ";
                    Query += "'" + InvoiceId + "', '" + SeqNo + "', '" + dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString() + "', ";
                    Query += "" + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString()) + ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString()) + ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["PPN"].Value.ToString()) + ", " + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString()) + ", 0, ";
                    Query += "'" + ControlMgr.UserId + "', GETDATE()";
                    Query += ")";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                //Insert Detail
            }
        }

        private void UpdateAttachment()
        {
            //Delete Attachment
            Query = "DELETE FROM tblAttachments WHERE ReffTransID = '" + txtInvoiceId.Text + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
            //End Delete Attachment

            //Insert Attachement
            for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, FileType, fileName, ContentType, fileSize, attachment, CreatedBy, CreatedDate) Values";
                Query += "( 'CustInvoiceH', '" + txtInvoiceId.Text.Trim() + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileSize"].Value.ToString();
                Query += "',@binaryValue, '" + ControlMgr.UserId + "', GETDATE()";
                Query += ");";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, attachByte[i].Length).Value = attachByte[i];
                Cmd.ExecuteNonQuery();
            }
            //End Insert Attachement  
        }

        private void UpdateCreditNote()
        {
            //Update Credit Note
            Query = "SELECT CN_No, Pot_CN FROM CustInvoice_Dtl_CreditNote WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                string CNNo = Dr["CN_No"].ToString();
                string PotCN = Dr["Pot_CN"].ToString();

                //Update CN
                string Query2 = "UPDATE NotaCreditH SET Deduct = Deduct -  " + PotCN + ", UpdatedBy = '" + ControlMgr.UserId + "', UpdatedDate = GETDATE() WHERE CN_No = '" + CNNo + "'";
                Cmd2 = new SqlCommand(Query2, Conn, Trans);
                Cmd2.ExecuteNonQuery();
                //End Update CN

                Query2 = "DELETE FROM CustInvoice_Dtl_CreditNote WHERE CN_No = '" + CNNo + "' AND Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd2 = new SqlCommand(Query2, Conn, Trans);
                Cmd2.ExecuteNonQuery();
            }
            Dr.Close();


            SeqNo = 1;
            for (int i = 0; i < dgvCNReference.RowCount; i++)
            {
                string CNDate = DateTime.ParseExact(dgvCNReference.Rows[i].Cells["CNDate"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                Boolean Check = Convert.ToBoolean(dgvCNReference.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    string Query2 = "INSERT INTO CustInvoice_Dtl_CreditNote (Invoice_Id, SeqNo, CN_No, CN_Date, Amount, Pot_CN, CreatedBy, CreatedDate) VALUES( ";
                    Query2 += "'" + InvoiceId + "', '" + SeqNo + "', '" + dgvCNReference.Rows[i].Cells["CNNo"].Value.ToString() + "', '" + CNDate + "', ";
                    Query2 += "" + Convert.ToDecimal(dgvCNReference.Rows[i].Cells["Amount"].Value.ToString()) + ", " + Convert.ToDecimal(dgvCNReference.Rows[i].Cells["PotonganCN"].Value.ToString()) + ", ";
                    Query2 += "'" + ControlMgr.UserId + "', GETDATE()";
                    Query2 += ")";
                    Cmd2 = new SqlCommand(Query2, Conn, Trans);
                    Cmd2.ExecuteNonQuery();

                    //Update CN
                    Query2 = "UPDATE NotaCreditH SET Deduct = Deduct +  " + Convert.ToDecimal(dgvCNReference.Rows[i].Cells["PotonganCN"].Value.ToString()) + ", UpdatedBy = '" + ControlMgr.UserId + "', UpdatedDate = GETDATE() WHERE CN_No = '" + dgvCNReference.Rows[i].Cells["CNNo"].Value.ToString() + "'";
                    Cmd2 = new SqlCommand(Query2, Conn, Trans);
                    Cmd2.ExecuteNonQuery();
                    //End Update CN
                    SeqNo++;
                }
            }
            //End Update Credit Note 
        }

        private void UpdateHeader()
        {
            //update header

            if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment" && txtProformaID.Text != "")
            {
                Query = "SELECT Proforma_Id FROM CustInvoice_H WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                string ProformaId = Convert.ToString(Cmd.ExecuteScalar());

                if (ProformaId != txtProformaID.Text)
                {
                    Query = "UPDATE CustInvoice_H SET Settle_Invoice_DP = Settle_Invoice_DP - Invoice_Amount WHERE Invoice_Id = '" + txtInvoiceId.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE CustInvoice_H SET Invoice_Date = '" + dtInvoiceDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', Invoice_Type = '" + cmbInvoiceType.SelectedItem.ToString() + "', TransStatus = '" + TransStatus + "', ";
                    Query += "Cust_Id = '" + txtCustID.Text + "', Cust_Name = '" + txtCustName.Text + "', PPN_Percent = '" + cmbPPN.SelectedItem.ToString() + "', ";
                    Query += "DP_Amount = " + Convert.ToDecimal(txtDPAmount.Text) + ", Invoice_Round_Amount = " + Convert.ToDecimal(txtPembulatan.Text) + ", Invoice_Amount = " + Convert.ToDecimal(txtInvoiceAmount.Text) + ", ";
                    Query += "Invoice_Tax_Base_Amount = " + Convert.ToDecimal(txtDPPAmount.Text) + ", Invoice_Tax_Amount = " + Convert.ToDecimal(txtTaxAmount.Text) + ", Payment_Method = '" + cmbPaymentMethod.SelectedItem.ToString() + "', ";
                    Query += "Invoice_DueDate = '" + dtPaymentDueDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', NPWP = '" + txtNPWP.Text + "', TaxDate = '" + dtTaxDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                    Query += "TaxNum = '" + txtTaxNumber.Text + "', TaxName = '" + txtTaxName.Text + "', TaxAddress = '" + txtTaxAddress.Text + "', Notes = '" + txtNotes.Text + "', ";
                    Query += "CN_Amount = " + Convert.ToDecimal(txtCNAmount.Text) + ", AR_Amount = " + Convert.ToDecimal(txtARAmount.Text) + ", CurrencyID = '" + cmbCurrency.SelectedItem.ToString() + "', ExchRate = " + Convert.ToDecimal(txtExchRate.Text) + ", ";
                    Query += "UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "', Proforma_Id = '" + txtProformaID.Text + "', Settle_Invoice_DP = Settle_Invoice_DP + " + Convert.ToDecimal(txtInvoiceAmount.Text) + ", SalesOrderNo = '" + txtSONo.Text + "', KodeKetTambahan = '" + txtKodeKetTambahan.Text + "', DPDescription = '" + txtDPDescription.Text + "' WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    Query = "UPDATE CustInvoice_H SET Invoice_Date = '" + dtInvoiceDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', Invoice_Type = '" + cmbInvoiceType.SelectedItem.ToString() + "', TransStatus = '" + TransStatus + "', ";
                    Query += "Cust_Id = '" + txtCustID.Text + "', Cust_Name = '" + txtCustName.Text + "', PPN_Percent = '" + cmbPPN.SelectedItem.ToString() + "', ";
                    Query += "DP_Amount = " + Convert.ToDecimal(txtDPAmount.Text) + ", Invoice_Round_Amount = " + Convert.ToDecimal(txtPembulatan.Text) + ", Invoice_Amount = " + Convert.ToDecimal(txtInvoiceAmount.Text) + ", ";
                    Query += "Invoice_Tax_Base_Amount = " + Convert.ToDecimal(txtDPPAmount.Text) + ", Invoice_Tax_Amount = " + Convert.ToDecimal(txtTaxAmount.Text) + ", Payment_Method = '" + cmbPaymentMethod.SelectedItem.ToString() + "', ";
                    Query += "Invoice_DueDate = '" + dtPaymentDueDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', NPWP = '" + txtNPWP.Text + "', TaxDate = '" + dtTaxDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                    Query += "TaxNum = '" + txtTaxNumber.Text + "', TaxName = '" + txtTaxName.Text + "', TaxAddress = '" + txtTaxAddress.Text + "', Notes = '" + txtNotes.Text + "', ";
                    Query += "CN_Amount = " + Convert.ToDecimal(txtCNAmount.Text) + ", AR_Amount = " + Convert.ToDecimal(txtARAmount.Text) + ", CurrencyID = '" + cmbCurrency.SelectedItem.ToString() + "', ExchRate = " + Convert.ToDecimal(txtExchRate.Text) + ", ";
                    Query += "UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "', SalesOrderNo = '" + txtSONo.Text + "', KodeKetTambahan = '" + txtKodeKetTambahan.Text + "', DPDescription = '" + txtDPDescription.Text + "' WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }
            }
            else
            {
                Query = "UPDATE CustInvoice_H SET Invoice_Date = '" + dtInvoiceDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', Invoice_Type = '" + cmbInvoiceType.SelectedItem.ToString() + "', TransStatus = '" + TransStatus + "', ";
                Query += "Cust_Id = '" + txtCustID.Text + "', Cust_Name = '" + txtCustName.Text + "', PPN_Percent = '" + cmbPPN.SelectedItem.ToString() + "', ";
                Query += "DP_Amount = " + Convert.ToDecimal(txtDPAmount.Text) + ", Invoice_Round_Amount = " + Convert.ToDecimal(txtPembulatan.Text) + ", Invoice_Amount = " + Convert.ToDecimal(txtInvoiceAmount.Text) + ", ";
                Query += "Invoice_Tax_Base_Amount = " + Convert.ToDecimal(txtDPPAmount.Text) + ", Invoice_Tax_Amount = " + Convert.ToDecimal(txtTaxAmount.Text) + ", Payment_Method = '" + cmbPaymentMethod.SelectedItem.ToString() + "', ";
                Query += "Invoice_DueDate = '" + dtPaymentDueDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', NPWP = '" + txtNPWP.Text + "', TaxDate = '" + dtTaxDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                Query += "TaxNum = '" + txtTaxNumber.Text + "', TaxName = '" + txtTaxName.Text + "', TaxAddress = '" + txtTaxAddress.Text + "', Notes = '" + txtNotes.Text + "', ";
                Query += "CN_Amount = " + Convert.ToDecimal(txtCNAmount.Text) + ", AR_Amount = " + Convert.ToDecimal(txtARAmount.Text) + ", CurrencyID = '" + cmbCurrency.SelectedItem.ToString() + "', ExchRate = " + Convert.ToDecimal(txtExchRate.Text) + ", ";
                Query += "UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "', SalesOrderNo = '" + txtSONo.Text + "', KodeKetTambahan = '" + txtKodeKetTambahan.Text + "', DPDescription = '" + txtDPDescription.Text + "' WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }

            //update header
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" || ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
            {
                if (txtNPWP.Text == "")
                {
                    MessageBox.Show("NPWP harus diisi");
                    return;
                }
                else if (txtTaxNumber.Text == "")
                {
                    MessageBox.Show("Tax Number harus diisi");
                    return;
                }
                else if (txtTaxName.Text == "")
                {
                    MessageBox.Show("Tax Name harus diisi");
                    return;
                }
                else if (txtTaxAddress.Text == "")
                {
                    MessageBox.Show("Tax Address harus diisi");
                    return;
                }
            }
            
            if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
            {
                if (cmbPPN.SelectedItem.ToString() != "0.00")
                {
                    if (dgvAttachment.RowCount > 0)
                    {
                        int j = 0;
                        for (int i = 0; i < dgvAttachment.RowCount; i++)
                        {
                            if (dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() == "Faktur Pajak")
                            {
                                j++;
                            }
                        }

                        if (j == 0)
                        {
                            MessageBox.Show("Documents Faktur Pajak harus ada");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Documents Faktur Pajak harus ada");
                        return;
                    }
                }
            }
            
            if (ControlMgr.GroupName.ToUpper() == "AR MANAGER")
            {
                if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
                {
                    if (dgvAttachment.RowCount > 0)
                    {
                        int j = 0;
                        for (int i = 0; i < dgvAttachment.RowCount; i++)
                        {
                            if (dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() == "Surat Jalan")
                            {
                                j++;
                            }
                        }

                        if (j == 0)
                        {
                            MessageBox.Show("Documents Surat Jalan harus ada");
                            return;
                        }
                        else
                        {
                            if (j < dgvSoReferenceDetailsSO.RowCount)
                            {
                                MessageBox.Show("Documents Surat Jalan harus sebanyak " + dgvSoReferenceDetailsSO.RowCount + " dokumen");
                                return;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Documents Surat Jalan harus ada");
                        return;
                    }
                }                
            }

            //UPDATE STATUS

             DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "InvoiceNo = " + txtInvoiceId.Text + " akan diapprove ? ", "Approved Confirmation !", MessageBoxButtons.YesNo);   
             if (dialogResult == DialogResult.Yes)
             {
                 try
                 {
                     Conn = ConnectionString.GetConnection();
                     Trans = Conn.BeginTransaction();

                     Query = "SELECT TransStatus FROM CustInvoice_H WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                     Cmd = new SqlCommand(Query, Conn, Trans);
                     string Status = Convert.ToString(Cmd.ExecuteScalar());
                     string NewTransStatus = "";

                     if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && (Status == "01" ||Status == "17"))
                     {
                         NewTransStatus = "02";
                     }
                     else if (ControlMgr.GroupName.ToUpper() == "AR MANAGER" && Status == "02")
                     {
                         if (cmbPPN.SelectedItem.ToString() == "0.00")
                         {
                             NewTransStatus = "03";
                         }
                         else
                         {
                             NewTransStatus = "11";
                         }
                     }
                     else if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER" && Status == "11")
                     {
                         NewTransStatus = "13";
                     }
                     Query = "UPDATE CustInvoice_H SET TransStatus = '" + NewTransStatus + "', ApprovedBy = '"+ControlMgr.UserId+"' WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                     Cmd = new SqlCommand(Query, Conn, Trans);
                     Cmd.ExecuteNonQuery();

                     if (ControlMgr.GroupName.ToUpper() == "AR MANAGER" && Status == "02")
                     {
                         //Begin
                         //Created By : Joshua
                         //Created Date : 06 Sept 2018
                         //Desc : Create Journal
                         CreateJournal();
                         //End
                     }

                     InsertLog(InvoiceId, "CustInvoice", NewTransStatus, "", Conn, Trans, Cmd);

                    Trans.Commit();
                    MessageBox.Show("Data Invoice No : " + InvoiceId + " berhasil diapprove.");

                    TransStatus = NewTransStatus;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                 }
                 catch (Exception)
                 {
                     Trans.Rollback();
                     return;
                 }
                 finally
                 {
                     Conn.Close();
                 }  
            }
            //END UPDATE STATUS
        }

        private void CreateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date : 06 Sept 2018
            //Desc : Create Journal

            string CustomerType = cmbInvoiceType.SelectedItem.ToString();
            string JournalHID = "";

            if (CustomerType == "Down Payment" && txtProformaID.Text != "")
            {
                JournalHID = "AR12";
            }
            else if (CustomerType == "Down Payment" && txtProformaID.Text == "")
            {
                JournalHID = "AR11";
            }
            else
            {
                JournalHID = "AR01";            
            }

            //Get GLJournalHID
            Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + txtInvoiceId.Text + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'BATAL' AND GLJournalHID = '" + GLJournalHID + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

            string Notes = txtCustID.Text + " - " + txtCustName.Text + " - " + txtSONo.Text;

            if (CountData == 1)
            {
                Query = "UPDATE GLJournalDtl SET Status = 'Gunakan', Notes = '" + Notes + "' WHERE GLJournalHID = '" + GLJournalHID + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }
            else
            {
                string Jenis = "JN", Kode = "JN";
                GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);             

                //Insert Header GLJournal
                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + txtInvoiceId.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }    

            //Select Config Journal
            int SeqNo = 1;
            int JournalIDSeqNo = 0;
            string Type = "";
            string FQA_ID = "";
            string FQA_Desc = "";
            decimal AmountValue = 0;

            Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                Type = Convert.ToString(Dr["Type"]);
                FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                AmountValue = 0;

                if (JournalHID == "AR12" || JournalHID == "AR11")
                {
                    if (JournalIDSeqNo == 1)
                    {
                        AmountValue = Convert.ToDecimal(txtInvoiceAmount.Text);
                    }
                    else if (JournalIDSeqNo == 2)
                    {
                        AmountValue = Convert.ToDecimal(txtDPPAmount.Text);
                    }
                    else if (JournalIDSeqNo == 3)
                    {
                        AmountValue = Convert.ToDecimal(txtTaxAmount.Text);
                    }
                }
                if (JournalHID == "AR01")
                {
                    if (JournalIDSeqNo == 1)
                    {
                        AmountValue = Convert.ToDecimal(txtInvoiceAmount.Text) - Convert.ToDecimal(txtDPAmount.Text);
                    }
                    else if (JournalIDSeqNo == 2)
                    {
                        AmountValue = Convert.ToDecimal(txtDPPAmount.Text);
                    }
                    else if (JournalIDSeqNo == 3)
                    {
                        AmountValue = Convert.ToDecimal(txtTaxAmount.Text);
                    }
                    else if (JournalIDSeqNo == 4)
                    {
                        AmountValue = Convert.ToDecimal(txtDPAmount.Text);
                    }
                    else if (JournalIDSeqNo == 5)
                    {
                        AmountValue = Convert.ToDecimal(txtDPPAmount.Text);
                    }
                    else if (JournalIDSeqNo == 6)
                    {
                        AmountValue = Convert.ToDecimal(txtDPPAmount.Text);
                    }
                }

                if (AmountValue == 0)
                {
                    continue;
                }

                //Insert Detail GLJournal
                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
                SeqNo++;
            }
            Dr.Close();

            //End
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
        }

        private void btnNewSOReference_Click(object sender, EventArgs e)
        {
              if (txtCustID.Text == "")
              {
                  MessageBox.Show("Silahkan pilih Customer ID terlebih dahulu");
                  return;
              }
              else if (txtSONo.Text == "")
              {
                  MessageBox.Show("Silahkan pilih SO No terlebih dahulu");
                  return;
              }
              else
              {
                  AddNewItemSOGI addnewitm = new AddNewItemSOGI(txtCustID.Text, cmbPPN.SelectedItem.ToString(), cmbInvoiceType.SelectedItem.ToString(), txtSONo.Text);
                  addnewitm.SetParentForm(this);
                  addnewitm.ParamHeader(dgvSoReferenceDetailsSO);
                  addnewitm.ShowDialog();
              }
        }

        private void btnDeleteSOReference_Click(object sender, EventArgs e)
        {
            if (dgvSoReferenceDetailsSO.RowCount > 0)
            {
                Index = dgvSoReferenceDetailsSO.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvSoReferenceDetailsSO.Rows[Index].Cells["No"].Value.ToString() + " akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvSoReferenceDetailsSO.Rows.RemoveAt(Index);
                    SortNoDataGridSOReferenceDetailsSO();

                    FormulaFooter();
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        //private void btnNewCNReference_Click(object sender, EventArgs e)
        //{
        //      if (txtCustID.Text == "")
        //      {
        //          MessageBox.Show("Customer ID Tidak Boleh Kosong.");
        //          return;
        //      }
        //      else
        //      {
        //          AddNewItemCN addnewitm = new AddNewItemCN(txtCustID.Text);
        //          addnewitm.SetParentForm(this);
        //          addnewitm.ParamHeader(dgvCNReference);
        //          addnewitm.ShowDialog();
        //      }
        //}

        private void SortNoDataGridAttachment()
        {
            for (int i = 0; i < dgvAttachment.RowCount; i++)
            {
                dgvAttachment.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void SortNoDataGridCN()
        {
            for (int i = 0; i < dgvCNReference.RowCount; i++)
            {
                dgvCNReference.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void SortNoDataGridSOReferenceDetailsSO()
        {
            for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
            {
                dgvSoReferenceDetailsSO.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void cmbInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment")
            {
                btnLookupProforma.Enabled = true;
                btnLookupDPDescription.Enabled = true;
            }
            else
            {
                btnLookupProforma.Enabled = false;
                btnLookupDPDescription.Enabled = false;
                txtDPDescription.Text = "";


                if (txtProformaID.Text != "" && Mode != "BeforeEdit")
                {
                    txtProformaID.Text = "";

                    dgvSoReferenceDetailsSO.DefaultCellStyle.BackColor = Color.White;
                    dgvSoReferenceDetailsSO.ReadOnly = false;
                    setHeaderDgvSOReferenceDetailsSO();

                    dgvCNReference.DefaultCellStyle.BackColor = Color.White;
                    dgvCNReference.ReadOnly = false;
                    //setHeaderDgvCNReference();

                    for (int i = 0; i <= 7; i++)
                    {
                        if (i == 0 || i == 7)
                        {
                            dgvCNReference.Columns[i].ReadOnly = false;
                        }
                        else
                        {
                            dgvCNReference.Columns[i].ReadOnly = true;
                        }
                    }

                    btnNewSOReference.Visible = true;
                    btnDeleteSOReference.Visible = true;

                    FormulaCN();  
                }                     
            }

            ClearAllDgv();

            if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment" || cmbInvoiceType.SelectedItem.ToString() == "Proforma")
            {
                SetDetailSoReferenceByDPPorforma();
            }
        }

        private void ClearAllDgv()
        {
            dgvSoReferenceDetailsSO.Rows.Clear();         

            dtPaymentDueDate.MaxDate = Convert.ToDateTime("12/31/9998");
            dtPaymentDueDate.Value = DateTime.Today.Date;

            txtSOAmount.Text = "0.00";
            txtGIAmount.Text = "0.00";
            txtReturAmount.Text = "0.00";
            txtDPPAmount.Text = "0.00";
            txtDPAmount.Text = "0.00";
            txtTaxAmount.Text = "0.00";
            if (txtCNAmount.Text == "")
            {
                txtCNAmount.Text = "0.00";
            } 
          
            txtInvoiceAmount.Text = "0.00";

            if (txtPembulatan.Text == "")
            {
                txtPembulatan.Text = "0.00";
            }

            txtARAmount.Text =Convert.ToString((Convert.ToDecimal(txtInvoiceAmount.Text) + Convert.ToDecimal(txtPembulatan.Text)) - Convert.ToDecimal(txtCNAmount.Text));                    
        }

        private void cmbPPN_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearAllDgv();
        }

        private void dgvCNReference_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvCNReference.Columns[dgvCNReference.CurrentCell.ColumnIndex].Name == "PotonganCN")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
            }
        }

        private void dgvCNReference_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvCNReference_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvCNReference_KeyPress);
            }
        }

        private void dgvCNReference_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCNReference.Columns[e.ColumnIndex].Name.ToString() == "PotonganCN")
            {
                double d = double.Parse( dgvCNReference.CurrentCell.Value.ToString());
                dgvCNReference.CurrentCell.Value = d.ToString("N2");
               
                Boolean Check = Convert.ToBoolean(dgvCNReference.Rows[e.RowIndex].Cells["chk"].EditedFormattedValue);
                if (Check == true)
                {
                    if (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString() == "" ? "0" : dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString()) <= 0)
                    {
                        MessageBox.Show("Pot. CN harus lebih dari 0");
                        dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString();
                        txtCNAmount.Text = dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString();
                        FormulaCN();
                        return;
                    }
                    else if (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString()) > (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()) + Convert.ToDecimal(getCNDB())))
                    {
                        MessageBox.Show("Pot. CN tidak boleh lebih dari Amount Outstanding");
                        dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = Convert.ToString(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value) + Convert.ToDecimal(getCNDB()));
                        txtCNAmount.Text = dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString();
                        FormulaCN();
                        return;
                    }                   
                }
                //else
                //{
                //    dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = "0.0000";
                //}

                FormulaCN();                
            }            
        }

        private void GetCreditNote()
        {           
            Conn = ConnectionString.GetConnection();

            string query = "SELECT ROW_NUMBER() OVER (ORDER BY CN_No) No, CN_No AS CNNo, CONVERT(VARCHAR, CN_Date, 103) AS CNDate, TotalAmount AS Amount, Deduct AS AmountDeduct, TotalAmount - Deduct AS AmountOutstanding, '0.00' AS PotonganCN FROM NotaCreditH WHERE (TotalAmount - Deduct) > 0 AND AccountNum = '" + txtCustID.Text + "' ORDER BY CN_No ASC ";
            
            Da = new SqlDataAdapter(query, Conn);
            dgvCNReference.Columns.Clear();
            dgvCNReference.DataSource = null;
            DataTable Dt = new DataTable();
            Da.Fill(Dt);

            if (dgvCNReference.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvCNReference.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            dgvCNReference.DataSource = Dt;
            dgvCNReference.AllowUserToAddRows = false;
            dgvCNReference.AutoResizeColumns();

            for (int i = 0; i <= 7; i++)
            {
                if (i == 0 || i == 7)
                {
                    dgvCNReference.Columns[i].ReadOnly = false;
                }
                else
                {
                    dgvCNReference.Columns[i].ReadOnly = true;
                }                
            }

            if (dgvCNReference.RowCount - 1 >= 0)
            {
                dgvCNReference.Columns[2].HeaderText = "CN No";
                dgvCNReference.Columns[3].HeaderText = "CN Date";
                dgvCNReference.Columns[5].HeaderText = "Amount Deduct";
                dgvCNReference.Columns[6].HeaderText = "Amount Outstanding";
                dgvCNReference.Columns[7].HeaderText = "Pot. CN";

                dgvCNReference.Columns["CNDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                dgvCNReference.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvCNReference.Columns["AmountDeduct"].DefaultCellStyle.Format = "N2";
                dgvCNReference.Columns["AmountOutstanding"].DefaultCellStyle.Format = "N2";
                dgvCNReference.Columns["PotonganCN"].DefaultCellStyle.Format = "N2";
            }

            Conn.Close();
        }

        private void dgvCNReference_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (txtProformaID.Text == "")
            {
                if (dgvCNReference.Columns[e.ColumnIndex].Name.ToString() == "chk")
                {

                    Boolean Check = Convert.ToBoolean(dgvCNReference.Rows[e.RowIndex].Cells["chk"].EditedFormattedValue);
                    if (Check == true)
                    {
                        if (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString() == "" ? "0" : dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString()) <= 0)
                        {
                            MessageBox.Show("Pot. CN harus lebih dari 0");
                            dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = Convert.ToString(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()).ToString("N2"));
                            txtCNAmount.Text = Convert.ToString(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()).ToString("N2"));
                            FormulaCN();
                            return;
                        }
                        else if (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString()) > (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()) + Convert.ToDecimal(getCNDB())))
                        {
                            MessageBox.Show("Pot. CN tidak boleh lebih dari Amount Outstanding");
                            dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = Convert.ToString(Convert.ToDecimal(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value) + Convert.ToDecimal(getCNDB())).ToString("N2"));
                            txtCNAmount.Text = Convert.ToString(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()).ToString("N2"));
                            FormulaCN();
                            return;
                        } 
                    }
                    //else
                    //{
                    //    dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = "0.0000";                    
                    //}

                    FormulaCN();
                }
            }             
        }

        public void FormulaFooter()
        {
            decimal GIAmount = 0;
            decimal ReturAmount = 0;
            decimal DPP = 0;
            decimal DP = 0;
            decimal TaxAmount = 0;
            decimal InvoiceAmount = 0;
            decimal SOAmount = 0;
            string SONo = "";
            decimal SumSOAmount = 0;
            decimal ARAmount = 0;
            Dictionary<string, decimal> DictionarySOAmount = new Dictionary<string, decimal>();

            if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
            {
                for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                {
                    GIAmount = GIAmount + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIAmount"].Value.ToString());
                    ReturAmount = ReturAmount + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["ReturAmount"].Value.ToString());
                    DPP = DPP + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString());
                    DP = DP + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString());
                    TaxAmount = TaxAmount + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString());
                    InvoiceAmount = InvoiceAmount + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString());
                    SONo = dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString();
                    SOAmount = Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["SOAmount"].Value.ToString());
                    if (!DictionarySOAmount.ContainsKey(SONo))
                    {
                        DictionarySOAmount.Add(SONo, SOAmount);
                    }
                }

                if (dgvSoReferenceDetailsSO.RowCount > 0)
                {
                    //getSumSOAmount
                    foreach (var getSumSOAmount in DictionarySOAmount)
                    {
                        SumSOAmount = SumSOAmount + getSumSOAmount.Value;
                    }
                    //end getSumSOAmount
                }
            }
            else
            { 
                //Inoice Type = DP
                DictionarySOAmount.Clear();
                for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                {
                    GIAmount = GIAmount + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["GIAmount"].Value.ToString() == "" ? "0" : dgvSoReferenceDetailsSO.Rows[i].Cells["GIAmount"].Value);
                    ReturAmount = ReturAmount + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["ReturAmount"].Value.ToString() == "" ? "0" : dgvSoReferenceDetailsSO.Rows[i].Cells["ReturAmount"].Value);
                    InvoiceAmount = InvoiceAmount + Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value.ToString() == "" ? "0" : dgvSoReferenceDetailsSO.Rows[i].Cells["InvoiceAmount"].Value);
                    SONo = dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString();
                    SOAmount = Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[i].Cells["SOAmount"].Value.ToString() == "" ? 0 : dgvSoReferenceDetailsSO.Rows[i].Cells["SOAmount"].Value);
                    if (!DictionarySOAmount.ContainsKey(SONo))
                    {
                        DictionarySOAmount.Add(SONo, SOAmount);
                    }
                }

                DPP = Convert.ToDecimal(InvoiceAmount) / Convert.ToDecimal(1.1);
                DP = 0;
                TaxAmount = DPP * Convert.ToDecimal(0.1);

                if (dgvSoReferenceDetailsSO.RowCount > 0)
                {
                    //getSumSOAmount
                    foreach (var getSumSOAmount in DictionarySOAmount)
                    {
                        SumSOAmount = SumSOAmount + getSumSOAmount.Value;
                    }
                    //end getSumSOAmount
                }
                //End Invoice Type = DP
            }
            

            txtSOAmount.Text = Convert.ToString(SumSOAmount.ToString("N2"));
            txtGIAmount.Text = Convert.ToString(GIAmount.ToString("N2"));
            txtReturAmount.Text = Convert.ToString(ReturAmount.ToString("N2"));
            txtDPPAmount.Text = Convert.ToString(DPP.ToString("N2"));
            txtDPAmount.Text = Convert.ToString(DP.ToString("N2"));
            txtTaxAmount.Text = Convert.ToString(TaxAmount.ToString("N2"));
            txtInvoiceAmount.Text = Convert.ToString(InvoiceAmount.ToString("N2"));
            ARAmount = (Convert.ToDecimal(txtInvoiceAmount.Text) + Convert.ToDecimal(txtPembulatan.Text)) - Convert.ToDecimal(txtCNAmount.Text);
            txtARAmount.Text = Convert.ToString(ARAmount.ToString("N2"));
            txtPembulatan.Text = Convert.ToDecimal(txtPembulatan.Text).ToString("N2");
            txtCNAmount.Text = Convert.ToDecimal(txtCNAmount.Text).ToString("N2");
        }

        private void dgvCNReference_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (txtProformaID.Text == "")
            {
                if (dgvCNReference.Columns[e.ColumnIndex].Name.ToString() == "chk")
                {
                    Boolean Check = Convert.ToBoolean(dgvCNReference.Rows[e.RowIndex].Cells["chk"].EditedFormattedValue);
                    if (Check == true)
                    {
                        //if (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString() == "" ? "0" : dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString()) <= 0)
                        //{
                        //    MessageBox.Show("Pot. CN harus lebih dari 0");
                        //    dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString();
                        //    txtCNAmount.Text = dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString();
                        //    FormulaCN();
                        //    return;
                        //}
                        //else if (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString()) > (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()) + Convert.ToDecimal(getCNDB())))
                        //{
                        //    MessageBox.Show("Pot. CN tidak boleh lebih dari Amount Outstanding");
                        //    dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = Convert.ToString(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value) + Convert.ToDecimal(getCNDB()));
                        //    txtCNAmount.Text = dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString();
                        //    FormulaCN();
                        //    return;
                        //} 

                        if (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString() == "" ? "0" : dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString()) <= 0)
                        {
                            MessageBox.Show("Pot. CN harus lebih dari 0");
                            dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = Convert.ToString(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()).ToString("N2"));
                            txtCNAmount.Text = Convert.ToString(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()).ToString("N2"));
                            FormulaCN();
                            return;
                        }
                        else if (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value.ToString()) > (Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()) + Convert.ToDecimal(getCNDB())))
                        {
                            MessageBox.Show("Pot. CN tidak boleh lebih dari Amount Outstanding");
                            dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = Convert.ToString(Convert.ToDecimal(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value) + Convert.ToDecimal(getCNDB())).ToString("N2"));
                            txtCNAmount.Text = Convert.ToString(Convert.ToDecimal(dgvCNReference.Rows[e.RowIndex].Cells["AmountOutstanding"].Value.ToString()).ToString("N2"));
                            FormulaCN();
                            return;
                        } 
                    }
                    //else
                    //{
                    //    dgvCNReference.Rows[e.RowIndex].Cells["PotonganCN"].Value = "0.0000";
                    //}

                    FormulaCN();
                }
            }
        }

        private void FormulaCN()
        {
            decimal result = 0;
            for (int i = 0; i < dgvCNReference.RowCount; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvCNReference.Rows[i].Cells["chk"].EditedFormattedValue);
                if (Check == true)
                {
                    result = result + Convert.ToDecimal(dgvCNReference.Rows[i].Cells["PotonganCN"].Value.ToString());
                }
            }

            txtCNAmount.Text =  result.ToString("N2");
            decimal ARAmount = (Convert.ToDecimal(txtInvoiceAmount.Text) + Convert.ToDecimal(txtPembulatan.Text)) - Convert.ToDecimal(txtCNAmount.Text);
            txtARAmount.Text = Convert.ToString(ARAmount.ToString("N2"));
        }

        private void txtPembulatan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtPembulatan_Leave(object sender, EventArgs e)
        {
            double d = 0;
            if (txtPembulatan.Text != "")
            {
                decimal CheckPembulatan;
                bool isNumeric = decimal.TryParse(txtPembulatan.Text, out CheckPembulatan);
                if (isNumeric == false)
                {
                    MessageBox.Show("Pembulatan harus bilangan");
                    txtPembulatan.Text = "0.00";
                    return;
                }
                else
                {
                    d = double.Parse(txtPembulatan.Text);  
                }                            
            }
            txtPembulatan.Text = d.ToString("N2");
        }

        private void txtPembulatan_TextChanged(object sender, EventArgs e)
        {
            decimal CheckPembulatan;
            bool isNumeric = decimal.TryParse(txtPembulatan.Text, out CheckPembulatan);
            if (isNumeric == true)
            {
                decimal ARAmount = (Convert.ToDecimal(txtInvoiceAmount.Text) + Convert.ToDecimal(txtPembulatan.Text)) - Convert.ToDecimal(txtCNAmount.Text);
                txtARAmount.Text = Convert.ToString(ARAmount.ToString("N2"));
            }           
        }

        private void dgvSoReferenceDetailsSO_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSoReferenceDetailsSO.Columns[e.ColumnIndex].Name.ToString() == "InvoiceAmount")
            {
                if (Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value.ToString() == "" ? "0" : dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value.ToString()) <= 0)
                {
                    MessageBox.Show("Invoice Amount harus lebih dari 0");
                    dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value = dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SOUnInvoiced"].Value.ToString();
                    FormulaFooter();
                    return;
                }
                else if (Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value.ToString()) > (Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SOUnInvoiced"].Value.ToString()) + Convert.ToDecimal(getInvoiceAmountDB())))
                {
                    MessageBox.Show("Invoice Amount tidak boleh lebih dari SOUnInvoiced");
                    dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value = Convert.ToString(Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SOUnInvoiced"].Value) + Convert.ToDecimal(getInvoiceAmountDB()));
                    FormulaFooter();
                    return;
                }
                else
                {
                    if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment")
                    {
                        if (dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["DPRequired"].Value.ToString() == "Y")
                        {
                            if (GetFirstDP(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SONo"].Value.ToString()))
                            {
                                decimal SOAmount = Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SOAmount"].Value.ToString());
                                decimal DPPercent = Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["DPPercent"].Value.ToString());

                                if (DPPercent > 0)
                                {
                                    decimal MinDP = SOAmount * (DPPercent / 100);

                                    if (Convert.ToDecimal(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value.ToString()) < MinDP)
                                    {
                                        MessageBox.Show("Invoice Amount minimal " + Convert.ToString(MinDP.ToString("N2")));
                                        dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value = Convert.ToString(MinDP);                                        
                                    }
                                }
                            }
                        }
                    }                    

                    double d = double.Parse(dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value.ToString());
                    dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["InvoiceAmount"].Value = d.ToString("N2");

                    FormulaFooter();
                }        
            }
        }

        private bool GetFirstDP(string prmSONo)
        {
            bool result = false;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT COUNT(SO_No) FROM CustInvoice_Dtl_DP WHERE SO_No = '"+ prmSONo +"'";
            Cmd = new SqlCommand(Query, Conn);
            int CountData = Convert.ToInt32(Cmd.ExecuteScalar());
            Conn.Close();

            if (Mode == "New")
            {
                if (CountData == 0)
                {
                    result = true;
                }
            }
            else
            {
                if (CountData == 1)
                {
                    result = true;
                }
            }
            
            return result;
        }

        private decimal getInvoiceAmountDB()
        {
            decimal result = 0;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT Invoice_Amount FROM CustInvoice_H WHERE Invoice_Id = '"+ txtInvoiceId.Text +"'";
            Cmd = new SqlCommand(Query, Conn);
            string InvoiceAmount = Convert.ToString(Cmd.ExecuteScalar());
            Conn.Close();

            if (InvoiceAmount != "")
            {
                result = Convert.ToDecimal(InvoiceAmount);
            }

            return result;
        }

        private decimal getCNDB()
        {
            decimal result = 0;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT Pot_CN FROM CustInvoice_Dtl_CreditNote WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string PotCN = Convert.ToString(Cmd.ExecuteScalar());
            Conn.Close();

            if (PotCN != "")
            {
                result = Convert.ToDecimal(PotCN);
            }

            return result;
        }

        private void dgvSoReferenceDetailsSO_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvSoReferenceDetailsSO_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvSoReferenceDetailsSO_KeyPress);
            }
        }

        private void dgvSoReferenceDetailsSO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSoReferenceDetailsSO.Columns[dgvSoReferenceDetailsSO.CurrentCell.ColumnIndex].Name == "InvoiceAmount")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 09 Jun 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                if (txtInvoiceId.Text != "")
                {
                    Query = "SELECT TransStatus FROM CustInvoice_H WHERE Invoice_Id='" + txtInvoiceId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Check = Cmd.ExecuteScalar().ToString();
                    if (Check == "03" || Check == "05" || Check == "06" || Check == "09" || Check == "13" || Check == "20" || Check == "21" || Check == "22")
                    {
                        MessageBox.Show("InvoiceId = " + txtInvoiceId.Text + ".\n" + "Tidak dapat diedit karena sudah diproses.");
                        Conn.Close();
                        return;
                    }
                }

                Mode = "Edit";
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void btnRevisi_Click(object sender, EventArgs e)
        {
            string NewTransStatus = "";
            if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN")
            {
                NewTransStatus = "08";
            }
            else if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
            {
                NewTransStatus = "17";
            }
            else if (ControlMgr.GroupName.ToUpper() == "AR MANAGER")
            {
                NewTransStatus = "07";
            }

            //UPDATE STATUS

            DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "InvoiceNo = " + txtInvoiceId.Text + " akan diminta untuk revisi ? ", "Revision Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "UPDATE CustInvoice_H SET TransStatus = '" + NewTransStatus + "' WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    InsertLog(InvoiceId, "CustInvoice", NewTransStatus, "", Conn, Trans, Cmd);

                    Trans.Commit();
                    MessageBox.Show("Data Invoice No : " + InvoiceId + " berhasil diminta untuk revisi.");

                    TransStatus = NewTransStatus;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            //END UPDATE STATUS
        }

        private void btnCancelApprove_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                string NewTransStatus = "";
                if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN")
                {
                    NewTransStatus = "01";
                }

                if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
                {   
                    NewTransStatus = "11";
                }            
                
                if (ControlMgr.GroupName.ToUpper() == "AR MANAGER")
                {                      

                    Query = "SELECT Kwitansi_No FROM CustInvoice_H WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string KwitansiNo = Convert.ToString(Cmd.ExecuteScalar());

                    Query = " SELECT COUNT(Invoice_Id) AS CountDataRV FROM ReceiptVoucher_Dtl WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    int CountDataRV = Convert.ToInt32(Cmd.ExecuteScalar());

                    if (KwitansiNo != "" || CountDataRV > 0)
                    {
                        MessageBox.Show("Tidak bisa cancel approve karena sudah diproses");
                        return;
                    }  
               
                    NewTransStatus = "02";
                }

                //UPDATE STATUS

                DialogResult dialogResult;
             
                if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
                {
                    dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "InvoiceNo = " + txtInvoiceId.Text + " akan di cancel approve ? " + Environment.NewLine + "Cancel Approve akan membuat Dokumen Faktur Pajak Terhapus", "Cancel Approve Confirmation !", MessageBoxButtons.YesNo);
                }
                else
                {
                    dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "InvoiceNo = " + txtInvoiceId.Text + " akan di cancel approve ? ", "Cancel Approve Confirmation !", MessageBoxButtons.YesNo);
                }

                if (dialogResult == DialogResult.Yes)
                {                   
                    if (ControlMgr.GroupName.ToUpper() == "TAX MANAGER")
                    {
                        Query = "UPDATE CustInvoice_H SET TransStatus = '" + NewTransStatus + "', TaxNum = '' WHERE Invoice_Id = '" + txtInvoiceId.Text + "';";

                        for (int i = 0; i < dgvAttachment.RowCount; i++)
                        {
                            if (dgvAttachment.Rows[i].Cells["FileType"].Value.ToString() == "Faktur Pajak")
                            {
                                string Id = dgvAttachment.Rows[i].Cells["Id"].Value.ToString();
                                Query += "DELETE FROM tblAttachments WHERE Id = '" + Id + "' AND ReffTableName = 'CustInvoiceH' AND ReffTransID = '"+txtInvoiceId.Text+"';";
                            }
                        }                       
                    }
                    else
                    {
                        Query = "UPDATE CustInvoice_H SET TransStatus = '" + NewTransStatus + "' WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                    }
                    
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //Begin
                    //Created By : Joshua
                    //Created Date ; 06 Sept 2018
                    //Desc : Batal Journal
                    if (ControlMgr.GroupName.ToUpper() == "AR MANAGER")
                    {
                        BatalJournal();
                        if (Journal == true)
                        {
                            Journal = false;
                            goto Outer;
                        }
                        
                    }
                    //End

                    InsertLog(InvoiceId, "CustInvoice", NewTransStatus, "", Conn, Trans, Cmd);

                    Trans.Commit();
                    MessageBox.Show("Data Invoice No : " + InvoiceId + " berhasil di cancel approve.");
                    TransStatus = NewTransStatus;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();

                    Outer: ;
                }
                
            }
            //END UPDATE STATUS

            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void BatalJournal()
        {
            //Get GLJournalHID
            Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + txtInvoiceId.Text + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            if (GLJournalHID != "")
            {
                Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                Cmd = new SqlCommand(Query, Conn, Trans);
                int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                if (CountData == 1)
                {
                    //Batal Journal
                    Query = "UPDATE GLJournalH SET Status = 'Batal' WHERE GLJournalHID = '" + GLJournalHID + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }
                else
                {
                    MetroFramework.MetroMessageBox.Show(this, "Tidak dapat closed karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Journal = true;
                    return;
                }
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                string NewTransStatus = "";          

                Query = "SELECT Kwitansi_No FROM CustInvoice_H WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                string KwitansiNo = Convert.ToString(Cmd.ExecuteScalar());

                if (KwitansiNo != "")
                {
                    MessageBox.Show("Tidak bisa reject karena sudah diproses");
                    return;
                }
                else
                {
                    NewTransStatus = "05";
                }     

                //UPDATE STATUS

                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "InvoiceNo = " + txtInvoiceId.Text + " akan di reject ? ", "Reject Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    Query = "UPDATE CustInvoice_H SET TransStatus = '" + NewTransStatus + "' WHERE Invoice_Id = '" + txtInvoiceId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //Rollback Data
                    if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
                    {
                        for (int i = 0; i < dgvSoReferenceDetailsSO.RowCount; i++)
                        {
                            string GINo = dgvSoReferenceDetailsSO.Rows[i].Cells["GINo"].Value.ToString();
                            string PotDP = dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP"].Value.ToString();
                            string PotDP2 = dgvSoReferenceDetailsSO.Rows[i].Cells["POTDP2"].Value.ToString();
                            string SONo = dgvSoReferenceDetailsSO.Rows[i].Cells["SONo"].Value.ToString();

                            //Update GI 
                            Query = "UPDATE GoodsIssuedH SET InvoiceNo = '' WHERE GoodsIssuedId = '" + GINo + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            //End Update GI 

                            //Update DP Master
                            if (Convert.ToDecimal(PotDP) > 0)
                            {
                                Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct - " + Convert.ToDecimal(PotDP) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }

                            if (Convert.ToDecimal(PotDP2) > 0)
                            {
                                Query = "UPDATE CustDown_Payment SET DP_Deduct = DP_Deduct - " + Convert.ToDecimal(PotDP2) + ",Last_Payment_Date = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE SO_Id = '" + SONo + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }
                            //End Update DP Master
                        }
                    }

                    for (int i = 0; i <= dgvCNReference.RowCount - 1; i++)
                    {
                         Boolean Check = Convert.ToBoolean(dgvCNReference.Rows[i].Cells["chk"].Value);
                         if (Check == true)
                         {
                             string CNNo = dgvCNReference.Rows[i].Cells["CNNo"].Value.ToString();
                             string PotonganCN = dgvCNReference.Rows[i].Cells["PotonganCN"].Value.ToString();

                             Query = "UPDATE NotaCreditH SET Deduct = Deduct -  " + Convert.ToDecimal(PotonganCN) + " WHERE CN_No = '" + CNNo + "'";
                             Cmd = new SqlCommand(Query, Conn, Trans);
                             Cmd.ExecuteNonQuery();
                         }                       
                    }

                    //DP By Proforma
                    if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment" && txtProformaID.Text != "")
                    {
                        Query = "UPDATE CustInvoice_H SET Settle_Invoice_DP = Settle_Invoice_DP - Invoice_Amount WHERE Invoice_Id = '" + txtInvoiceId.Text + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    //End DP By Proforma
                    
                    //End Rollback Data

                    InsertLog(InvoiceId, "CustInvoice", NewTransStatus, "", Conn, Trans, Cmd);

                    Trans.Commit();
                    MessageBox.Show("Data Invoice No : " + InvoiceId + " berhasil di reject.");

                    TransStatus = NewTransStatus;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
            }
            //END UPDATE STATUS

            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void btnLookupProforma_Click(object sender, EventArgs e)
        {
            if (txtCustID.Text == "")
            {
                MessageBox.Show("Silahkan pilih Customer ID terlebih dahulu");
                return;
            }
            //else if (txtSONo.Text == "")
            //{
            //    MessageBox.Show("Silahkan pilih SO No terlebih dahulu");
            //    return;
            //}
            else
            {
                SearchQueryV1 tmpSearch = new SearchQueryV1();
                tmpSearch.PrimaryKey = "InvoiceId";
                tmpSearch.Order = "InvoiceId Asc";
                tmpSearch.Table = "[dbo].[CustInvoice_H]";
                tmpSearch.QuerySearch = "SELECT a.Invoice_Id AS InvoiceId, CONVERT(VARCHAR,a.Invoice_Date, 103) AS InvoiceDate, a.Invoice_Amount AS InvoiceAmount, a.CN_Amount AS CNAmount, a.AR_Amount AS ARAmount, a.Settle_Amount AS SettleAmount, (a.Settle_Amount - a.Settle_Invoice_DP) AS AmountOutstanding, b.Deskripsi AS Status FROM CustInvoice_H a ";
               // tmpSearch.QuerySearch += "INNER JOIN CustInvoice_Dtl_DP dtl ON a.Invoice_Id = dtl.Invoice_Id ";
                tmpSearch.QuerySearch += "INNER JOIN TransStatusTable b ON b.StatusCode = a.TransStatus AND b.TransCode = 'CustInvoice' AND a.Settle_Amount > 0 AND (a.Settle_Amount - a.Settle_Invoice_DP) > 0 AND a.Invoice_Type = 'Proforma' AND a.Cust_Id = '" + txtCustID.Text + "' AND a.PPN_Percent = '" + cmbPPN.SelectedItem.ToString() + "' ";
                tmpSearch.FilterText = new string[] { "InvoiceId", "Status" };
                tmpSearch.Select = new string[] { "InvoiceId", "InvoiceDate", "InvoiceAmount", "CNAmount", "ARAmount", "SettleAmount", "AmountOutstanding", "Status" };
                tmpSearch.ShowDialog();
                if (ConnectionString.Kodes != null)
                {
                    txtProformaID.Text = ConnectionString.Kodes[0];

                    ConnectionString.Kodes = null;

                    btnNewSOReference.Visible = false;
                    btnDeleteSOReference.Visible = false;

                    dgvSoReferenceDetailsSO.DefaultCellStyle.BackColor = Color.LightGray;
                    dgvSoReferenceDetailsSO.ReadOnly = true;
                    setHeaderDgvSOReferenceDetailsSO();

                    dgvCNReference.DefaultCellStyle.BackColor = Color.LightGray;
                    dgvCNReference.ReadOnly = true;

                    //reset Checklist Grid CN
                    for (int i = 0; i <= dgvCNReference.RowCount - 1; i++)
                    {
                        Boolean Check = Convert.ToBoolean(dgvCNReference.Rows[i].Cells["chk"].Value);
                        if (Check == true)
                        {
                            dgvCNReference.Rows[i].Cells["chk"].Value = false;
                        }
                    }

                    txtCNAmount.Text = "0.00";

                    //get data SO
                    getDataSOByProforma();
                }
            }            
        }

        private void getDataSOByProforma()
        {
            dgvSoReferenceDetailsSO.Rows.Clear();
            Conn = ConnectionString.GetConnection();

            Query = "SELECT D.SO_No AS SONO, V.SODate, V.SODueDate, V.SOReference, V.DPRequired, V.DPPercent, D.Tax_Percent AS PPN, ";
            Query += "V.SOAmount,  V.DPAmount, V.DPDeduct, V.DPOutstanding, V.SOInvoiced, V.SOUnInvoiced, '' AS GINo, GETDATE() AS GIDate, ";
            Query += "V.GIAmount, V.ReturAmount, V.GIPayable, V.POTDP, D.Line_Amount AS InvoiceAmount, D.Line_Tax_Base_Amount AS InvoiceTaxBaseAmount, D.Line_Tax_Amount AS InvoiceTaxAmount ";
            Query += "FROM CustInvoice_Dtl_DP D LEFT JOIN vSalesUnInvoiceTable_DPView V ON V.SONo = D.SO_No ";
            Query += "WHERE D.Invoice_Id='" + txtProformaID.Text + "' ORDER BY D.SeqNo ASC ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int i = 1;
            while (Dr.Read())
            {
                this.dgvSoReferenceDetailsSO.Rows.Add(i, Dr["SONo"], Dr["SODate"], Dr["SODueDate"], Dr["SOReference"], Dr["DPRequired"],
                Dr["DPPercent"], Dr["PPN"], Dr["SOAmount"], Dr["DPAmount"], Dr["DPDeduct"], Dr["DPOutstanding"], Dr["SOInvoiced"],
                Dr["SOUnInvoiced"], Dr["GINo"], Dr["GIDate"], Dr["GIAmount"], Dr["ReturAmount"], Dr["POTDP"], Dr["GIPayable"], "0.00", Dr["InvoiceAmount"], Dr["InvoiceTaxBaseAmount"], Dr["InvoiceTaxAmount"]);
                i++;

                txtSONo.Text = Convert.ToString(Dr["SONo"]);
            }
            Dr.Close();            

            dgvSoReferenceDetailsSO.AutoResizeColumns();

            string PaymentDueDate = "";
            if (i != 1)
            {
                var GetPaymentDueDate = from CustInovice in dgvSoReferenceDetailsSO.Rows.Cast<DataGridViewRow>()
                                        orderby CustInovice.Cells["SODueDate"].Value descending
                                        select CustInovice.Cells["SODueDate"].Value.ToString();

                PaymentDueDate = GetPaymentDueDate.First();
            }

            //getPaymentDueDate   
            if (PaymentDueDate == "")
            {
                dtPaymentDueDate.MaxDate = Convert.ToDateTime(DateTime.Now.ToString());
            }
            else
            {
                dtPaymentDueDate.MaxDate = Convert.ToDateTime(PaymentDueDate);
            }           
            //end getPaymentDueDate

            Query = "SELECT CN_Amount FROM CustInvoice_H WHERE Invoice_Id='" + txtProformaID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            txtCNAmount.Text = Convert.ToDecimal(Cmd.ExecuteScalar()).ToString("N2");
            
            Conn.Close();

            FormulaFooter();
        }
        //tia edit
        //klik kanan
       
        Sales.SalesOrder.SOHeader SOID = null;
        PopUp.CustomerID.Customer Cust = null;
        Sales.NotaCredit.FrmT_NotaCredit CN = null;
        AccountPayable.HeaderAccountsPayable APId = null;

        ARCollection.Kwitansi ParentToKwitansi;
        AccountsReceivable.ReceiptVoucher.HeaderReceiptVoucher ParentToRV;
        TaskList.AccountsReceivable.TasklistCustomerInvoice Parent2;

        public void SetParent2(TaskList.AccountsReceivable.TasklistCustomerInvoice F2)
        {
            Parent2 = F2;
        }

        public void ParentRefreshGrid(ARCollection.Kwitansi kw)
        {
            ParentToKwitansi = kw;
        }
       
        public void ParentRefreshGrid2(AccountsReceivable.ReceiptVoucher.HeaderReceiptVoucher rv)
        {
            ParentToRV = rv;
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

        private void dgvSoReferenceDetailsSO_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (SOID == null || SOID.Text == "")
                {
                    if (dgvSoReferenceDetailsSO.Columns[e.ColumnIndex].Name.ToString() == "SONo")
                    {
                        SOID = new  Sales.SalesOrder.SOHeader();
                        SOID.SetMode("PopUp", dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SONo"].Value.ToString());
                        SOID.ParentRefreshGrid7(this);
                        SOID.Show();
                    }
                }
                else if (CheckOpened(SOID.Name))
                {
                    SOID.WindowState = FormWindowState.Normal;
                    SOID.SetMode("PopUp", dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SONo"].Value.ToString());
                    SOID.ParentRefreshGrid7(this);                   
                    SOID.Show();
                    SOID.Focus();
                }

                //if (CN == null || CN.Text == "")
                //{
                //    if (dgvSoReferenceDetailsSO.Columns[e.ColumnIndex].Name.ToString() == "SONo")
                //    {
                //        CN = new Sales.NotaCredit.FrmT_NotaCredit();
                //        //CN.SetMode("PopUp", dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SONo"].Value.ToString());
                //        //CN.ParentRefreshGrid7(this);
                //        CN.Show();
                //    }
                //}
                //else if (CheckOpened(CN.Name))
                //{
                //    CN.WindowState = FormWindowState.Normal;
                //    //CN.FormNew("PopUp", dgvSoReferenceDetailsSO.Rows[e.RowIndex].Cells["SONo"].Value.ToString());
                //    //CN.ParentRefreshGrid(this);
                //    CN.Show();
                //    CN.Focus();
                //}
            }
        }

        private void txtSONo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SOID == null || SOID.Text == "")
                {
                   
                        SOID = new Sales.SalesOrder.SOHeader();
                        SOID.SetMode("PopUp", txtSONo.Text);
                        SOID.ParentRefreshGrid7(this);
                        SOID.Show();
                    
                }
                else if (CheckOpened(SOID.Name))
                {
                    SOID.WindowState = FormWindowState.Normal;
                    SOID.SetMode("PopUp", txtSONo.Text);
                    SOID.ParentRefreshGrid7(this);
                    SOID.Show();
                    SOID.Focus();
                }
            }
        }

        private void txtCustID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    txtCustID.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(txtCustID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        private void txtCustName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    txtCustName.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(txtCustID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        private void txtProformaID_MouseDown(object sender, MouseEventArgs e)
        {
            if (APId == null || APId.Text == "")
            {
                txtProformaID.Enabled = true;
                APId = new AccountPayable.HeaderAccountsPayable();
                APId.SetMode("BeforeEdit", txtProformaID.Text);
                    
                APId.Show();
            }
            else if (CheckOpened(APId.Name))
            {
                APId.WindowState = FormWindowState.Normal;
                APId.Show();
                APId.Focus();
            }
        }
        //tia edit end
        private void btnLookupSONo_Click(object sender, EventArgs e)
        {
            if (txtCustID.Text == "")
            {
                MessageBox.Show("Silahkan pilih Customer ID terlebih dahulu");
                return;
            }
            else
            {
                string ViewName = "";
                if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
                {
                    ViewName = "vSalesUnInvoiceTable_GIView";
                }
                else
                {
                    ViewName = "vSalesUnInvoiceTable_DPView";
                }

                SearchQueryV1 tmpSearch = new SearchQueryV1();
                tmpSearch.PrimaryKey = "SoNo";
                tmpSearch.Order = "SoNo DESC";
                tmpSearch.Table = "[dbo]." + ViewName + "";
                tmpSearch.QuerySearch = "SELECT DISTINCT SONo FROM " + ViewName + " WHERE PPN = '" + cmbPPN.SelectedItem.ToString() + "' AND CustId = '" + txtCustID.Text + "'";
                tmpSearch.FilterText = new string[] { "SoNo" };
                tmpSearch.Select = new string[] { "SONo" };
                tmpSearch.ShowDialog();
                if (ConnectionString.Kodes != null)
                {
                    txtSONo.Text = ConnectionString.Kodes[0];

                    ConnectionString.Kodes = null;

                    if (cmbInvoiceType.SelectedItem.ToString() == "Down Payment" || cmbInvoiceType.SelectedItem.ToString() == "Proforma")
                    {
                        SetDetailSoReferenceByDPPorforma();
                    }
                    else
                    {
                        ClearAllDgv();
                    }
                }  
            }            
        }
       
        private void SetDetailSoReferenceByDPPorforma()
        {
            if (txtCustID.Text != "" && txtSONo.Text != "")
            {
                txtProformaID.Text = "";
                ClearAllDgv();
                dgvCNReference.DataSource = null;
                GetCreditNote();

                dgvSoReferenceDetailsSO.DefaultCellStyle.BackColor = Color.White;
                dgvSoReferenceDetailsSO.ReadOnly = false;
                setHeaderDgvSOReferenceDetailsSO();

                dgvCNReference.DefaultCellStyle.BackColor = Color.White;
                dgvCNReference.ReadOnly = false;

                Conn = ConnectionString.GetConnection();
                string Query = "SELECT * FROM vSalesUnInvoiceTable_DPView WHERE CustID = '" + txtCustID.Text + "' AND PPN = '" + cmbPPN.SelectedItem.ToString() + "' AND SONo = '" + txtSONo.Text + "' AND SOUnInvoiced <> 0 ";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int No = 1;
                while (Dr.Read())
                {
                    dgvSoReferenceDetailsSO.Rows.Add(No, Convert.ToString(Dr["SoNo"]), Convert.ToDateTime(Dr["SODate"]).ToString("dd/MM/yyyy"), Convert.ToDateTime(Dr["SODueDate"]).ToString("dd/MM/yyyy"), Convert.ToString(Dr["SOReference"]),
                    Convert.ToString(Dr["DPRequired"]), Convert.ToString(Dr["DPPercent"]), Convert.ToString(Dr["PPN"]), Convert.ToDecimal(Dr["SOAmount"]), Convert.ToDecimal(Dr["DPAmount"]),
                    Convert.ToDecimal(Dr["DPDeduct"]), Convert.ToDecimal(Dr["DPOutstanding"]), Convert.ToString(Dr["SOInvoiced"]), Convert.ToString(Dr["SOUnInvoiced"]), "",
                    Convert.ToString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")), Convert.ToDecimal(Dr["GIAmount"]), Convert.ToDecimal(Dr["ReturAmount"]), Convert.ToDecimal(Dr["POTDP"]),
                    Convert.ToDecimal(Dr["GIPayable"]), "0.00", Convert.ToDecimal(Dr["InvoiceAmount"]), Convert.ToDecimal(Dr["InvoiceTaxBaseAmount"]), Convert.ToDecimal(Dr["InvoiceTaxAmount"]));
                    No++;
                }
                Dr.Close();
                Conn.Close();

                for (int i = 0; i < 24; i++)
                {
                    if (cmbInvoiceType.SelectedItem.ToString() == "Invoice")
                    {
                        dgvSoReferenceDetailsSO.Columns[i].ReadOnly = true;
                    }
                    else if (cmbInvoiceType.SelectedItem.ToString() != "Invoice" && i == 21)
                    {
                        dgvSoReferenceDetailsSO.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvSoReferenceDetailsSO.Columns[i].ReadOnly = true;
                    }
                }

                dgvSoReferenceDetailsSO.Columns["GINo"].Visible = false;
                dgvSoReferenceDetailsSO.Columns["POTDP2"].Visible = false;


                if(No != 1)
                {
                    //getPaymentDueDate
                    var GetPaymentDueDate = from CustInovice in dgvSoReferenceDetailsSO.Rows.Cast<DataGridViewRow>()
                                            orderby Convert.ToInt32(CustInovice.Cells["SODueDate"].Value.ToString().Substring(6, 4)) + "" +
                                            Convert.ToInt32(CustInovice.Cells["SODueDate"].Value.ToString().Substring(3, 2)) + "" +
                                            Convert.ToInt32(CustInovice.Cells["SODueDate"].Value.ToString().Substring(0, 2)) descending
                                            select CustInovice.Cells["SODueDate"].Value.ToString();


                    string PaymentDueDate = GetPaymentDueDate.First();
                    dtPaymentDueDate.MaxDate = Convert.ToDateTime(PaymentDueDate.Substring(3, 2) + "/" + PaymentDueDate.Substring(0, 2) + "/" + PaymentDueDate.Substring(6, 4));
                    //end getPaymentDueDate
                }
               

                FormulaFooter();
            }              
        }

        private void btnLookupKetTambahan_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "Kode";
            tmpSearch.Order = "Kode Asc";
            tmpSearch.Table = "[dbo].[KawasanBerikat]";
            tmpSearch.QuerySearch = "SELECT Kode, Keterangan FROM KawasanBerikat ";
            tmpSearch.FilterText = new string[] { "Kode", "Keterangan" };
            tmpSearch.Select = new string[] { "Kode", "Keterangan" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtKodeKetTambahan.Text = ConnectionString.Kodes[0];
                txtKetTambahan.Text = ConnectionString.Kodes[1];
            }

            ConnectionString.Kodes = null;
        }

        private void txtTaxNumber_Leave(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName.ToUpper() == "TAX ADMIN" && txtTaxNumber.Text.Trim() != "")
            {
                if (txtTaxNumber.Text.Trim().Substring(0, 2) == "07")
                {
                    btnLookupKetTambahan.Enabled = true;
                }
                else
                {
                    btnLookupKetTambahan.Enabled = false;
                    txtKetTambahan.Text = "";
                    txtKodeKetTambahan.Text = "";
                }
            }
        }

        private void btnLookupDPDescription_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "ID";
            tmpSearch.Order = "ID Asc";
            tmpSearch.Table = "[dbo].[ItemDP]";
            tmpSearch.QuerySearch = "SELECT ID, Description FROM ItemDP ";
            tmpSearch.FilterText = new string[] { "ID", "Description" };
            tmpSearch.Select = new string[] { "ID", "Description" };
            tmpSearch.Hide = new string[] { "ID" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtDPDescription.Text = ConnectionString.Kodes[1];
                IDItemDP = ConnectionString.Kodes[0];
            }

            ConnectionString.Kodes = null;
        }

       
    }
}
