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
using System.IO;

namespace ISBS_New.AccountsReceivable.ReceiptVoucher
{
    public partial class HeaderReceiptVoucher : MetroFramework.Forms.MetroForm
    {
        
        //Sql Variable//
        SqlConnection Con;
        SqlCommand Cmd;
        SqlDataReader Dr;
        TransactionScope Scope;
        string Query;
        //============//

        //Global Variable//
        string Mode;
        string RefId;
        //===============//

        bool Journal = false;

        //Grid Variable, setting visible sama ReadOnly========//
        string[] TableColHeaderText = { "No", "RV No", "SeqNo", "Customer Id", "Customer", "Invoice Date", "Invoice No", "ToP", "Invoice DueDate", "Invoice Amount", "CN Amount", "AR Amount", "Payment", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
        string[] TableColName = { "No", "RV_No", "SeqNo", "Cust_Id", "Cust_Name", "Invoice_Date", "Invoice_Id", "TermOfPayment", "DueDate", "Invoice_Amount", "CN_Amount", "AR_Amount", "Payment_Amount", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
        string[] TableColVisibleTrue = { "No", "Invoice No", "Invoice Date", "ToP", "Invoice DueDate", "Invoice Amount","AR Amount", "Payment" };
        string[] TableColReadOnlyFalse = { "Payment" };
        //====================================================//

        //Grid Credit Note====================================//
        string[] TableColCNHeaderText = { "No","Check","CN Date","CN No","CNMode","NRJId","DueDate","AccountNum","AccountName","CurrencyId","ExchRate","TaxStatusCode","TaxPercent","PPHTaxStatusCode","PPHTaxPercent","TotalAmount","Remaining Amount","Amount","TaxBaseAmount","TaxAmount","PPHTaxAmount","NPWP","TaxNum","TaxAddress","TaxName","TaxDate","TransStatus","Notes","CreatedDate","CreatedBy","UpdatedDate","UpdatedBy","Deduct","SO_Id" };
        string[] TableColCNName = { "No","Check","CN_Date","CN_No","CNMode","NRJId","DueDate","AccountNum","AccountName","CurrencyId","ExchRate","TaxStatusCode","TaxPercent","PPHTaxStatusCode","PPHTaxPercent","TotalAmount","Remaining Amount","Amount","TaxBaseAmount","TaxAmount","PPHTaxAmount","NPWP","TaxNum","TaxAddress","TaxName","TaxDate","TransStatus","Notes","CreatedDate","CreatedBy","UpdatedDate","UpdatedBy","Deduct","SO_Id"};
        string[] TableColCNVisibleTrue = { "No", "Check", "CN Date", "CN No", "NRJId", "DueDate", "CurrencyId", "TotalAmount", "Remaining Amount", "Amount", "Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy", "Deduct" };
        string[] TableColCNReadOnlyFalse = { "Check","Amount" };
        //====================================================//

        //Grid Value, before edit=====================// assign in refreshgrid
        List<string> InvoiceId = new List<string>();
        List<int> SeqNo = new List<int>();
        List<decimal> Amount = new List<decimal>();
        //============================================//

        //Grid Value CN, before edit==================// assign in setValueBeforeEdit
        List<string> CNID = new List<string>();
        List<decimal> AmountCN = new List<decimal>();
        //============================================//

        //tia edit
        ContextMenu vendid = new ContextMenu();
        //tia edit end

        //setting Global Inquiry as parent form//
        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F) 
        { 
            Parent = F; 
        }
        //=====================================//

        //checking permission access for user (this access can be modified in [dbo].[sysGroupAuthority])//
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //==============================================================================================//

        private bool PermissionTreasury()
        {
            if (ControlMgr.GroupName.ToUpper().Contains("TREASURY"))
            {
                return true;
            }
            return false;
        }

        public HeaderReceiptVoucher()
        {
            InitializeComponent();
        }

        //setting mode from global inquiry or from the inside=======//
        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            RefId = id;
        }
        //===========================================================//

        //for attachment=============================================//
        List<byte[]> attachment = new List<byte[]>();
        //===========================================================//

        private void ModeNew()
        {
            Mode = "New";
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnSave.Enabled = true;
            btnExit.Enabled = true;

            btnSearchCustomer.Enabled = true;
            btnSearchKodeBank.Enabled = true;
            btnUploadFile.Enabled = true;
            btnDownloadFile.Enabled = true;
            btnDeleteFile.Enabled = true;
            txtCustomer.Enabled = true;
            txtNameCustomer.Enabled = true;
            cmbPaymentMethod.Enabled = true;


        }

        private void ModeEdit()
        {
            Mode = "Edit";

            lblChequeResult.Visible = true;
            cmbChequeResult.Visible = true;
            cmbChequeResult.Enabled = true;
            dtTglCheque.Enabled = true;
            //if (PermissionTreasury() == true)
            //{
            //    LockControlValues(this);
            //    btnSave.Enabled = true;
            //    btnCancel.Enabled = true;
            //    btnDownloadFile.Enabled = true;
            //    if (cmbPaymentMethod.Text.ToUpper() == "CHEQUE" || cmbPaymentMethod.Text.ToUpper() == "GIRO")
            //    {
            //        lblTglTolak.Visible = true;
            //        lblTglCair.Visible = true;
            //        dtTglCair.Visible = true;
            //        dtTglCair.Enabled = true;
            //        dtTglTolak.Visible = true;
            //        dtTglTolak.Enabled = true;
            //    }
            //    dgvReceiptVoucherDtl.ReadOnly = false;
            //    for (int i = 0; i < dgvReceiptVoucherDtl.Columns.Count; i++)
            //    {
            //        dgvReceiptVoucherDtl.Columns[i].ReadOnly = true;
            //    }
            //    dgvReceiptVoucherDtl.Columns["ActualAmount"].DefaultCellStyle.BackColor = Color.White;
            //    dgvReceiptVoucherDtl.Columns["Notes"].DefaultCellStyle.BackColor = Color.White;
            //    dgvReceiptVoucherDtl.Columns["Check"].ReadOnly = false;
            //    dgvReceiptVoucherDtl.Columns["ActualAmount"].ReadOnly = false;
            //    dgvReceiptVoucherDtl.Columns["Notes"].ReadOnly = false;
            //}
            //else
            //{
                btnExit.Enabled = false;
                btnEdit.Enabled = false;
                btnCancel.Enabled = true;
                btnSave.Enabled = true;

                btnSearchCustomer.Enabled = true;
                btnSearchKodeBank.Enabled = true;

                btnUploadFile.Enabled = true;
                btnDownloadFile.Enabled = true;
                btnDeleteFile.Enabled = true;
                txtCustomer.Enabled = true;
                txtNameCustomer.Enabled = true;
                txtNotes.Enabled = true;
                cmbPaymentMethod.Enabled = true;
                txtNominal.Enabled = true;
                dtRVDate.Enabled = true;
                if (cmbPaymentMethod.Text.ToUpper() == "TRANSFER")
                {
                    btnSearchRekCust.Enabled = true;
                }
                else if (cmbPaymentMethod.Text.ToUpper() == "GIRO")
                {
                    txtKetTransfer.Enabled = false;
                    dtJatuhTempo.Enabled = true;
                    txtNoGiro.Enabled = true;
                }
                else if (cmbPaymentMethod.Text.ToUpper() == "CHEQUE")
                {
                    txtKetTransfer.Enabled = false;
                    dtJatuhTempo.Enabled = true;
                    txtNoGiro.Enabled = true;
                }
                else if (cmbPaymentMethod.Text.ToUpper() == "CHEQUE")
                {
                    txtKetTransfer.Enabled = true;
                }
                dgvReceiptVoucherDtl.Columns["Payment_Amount"].ReadOnly = false;

                for (int i = 0; i < dgvReceiptVoucherDtl.Columns.Count; i++)
                {
                    for (int j = 0; j < TableColReadOnlyFalse.Count(); j++)
                    {
                        if (dgvReceiptVoucherDtl.Columns[i].HeaderText == TableColReadOnlyFalse[j])
                        {
                            dgvReceiptVoucherDtl.Columns[i].ReadOnly = false;
                            dgvReceiptVoucherDtl.Columns[i].DefaultCellStyle.BackColor = Color.White;
                            break;
                        }
                        else
                        {
                            dgvReceiptVoucherDtl.Columns[i].ReadOnly = true;
                            dgvReceiptVoucherDtl.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                    }
                }

                for (int i = 0; i < dgvCreditNote.Columns.Count; i++)
                {
                    for (int j = 0; j < TableColCNReadOnlyFalse.Count(); j++)
                    {
                        if (dgvCreditNote.Columns[i].HeaderText == TableColCNReadOnlyFalse[j])
                        {
                            dgvCreditNote.Columns[i].ReadOnly = false;
                            dgvCreditNote.Columns[i].DefaultCellStyle.BackColor = Color.White;
                            break;
                        }
                        else
                        {
                            dgvCreditNote.Columns[i].ReadOnly = true;
                            dgvCreditNote.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                    }
                }
            //}
        }

        private void ModeView()
        {
            Mode = "View";
            populateCmbPaymentMethod();

            txtCustomer.ContextMenu = vendid;
            txtNameCustomer.ContextMenu = vendid;
            //tia edit end
            GetDataHeader();
            RefreshGrid();
            refreshGridCreditNote();
            RefreshGridAttachment();


            lblTglCair.Visible = true;
            cmbChequeResult.Visible = true;
            lblChequeResult.Visible = true;
            dtTglCheque.Visible = true;
            cmbChequeResult.Enabled = false;
            dtTglCheque.Enabled = false;

            if (PermissionTreasury() == true)
            {
                if ((cmbPaymentMethod.Text.ToUpper().Trim() == "CHEQUE" || cmbPaymentMethod.Text.ToUpper().Trim() == "GIRO")&&(cmbChequeResult.Text.ToUpper().Trim() == "TOLAK" || cmbChequeResult.Text.ToUpper().Trim() == "TUNDA"))
                {
                    btnApprove.Visible = true;
                    btnApprove.Enabled = true;
                }
            }
            
            //if (PermissionTreasury() == true)
            //{
            //    LockControlValues(this);
            //    btnEdit.Enabled = true;
            //    btnExit.Enabled = true;
            //    btnDownloadFile.Enabled = true;
            //    lblTglTolak.Visible = true;
            //    lblTglCair.Visible = true;
            //    dtTglCair.Visible = true;
            //    dtTglTolak.Visible = true;
            //}
            //else
            //{
                btnCancel.Enabled = false;
                btnSave.Enabled = false;
                btnExit.Enabled = true;
                btnEdit.Enabled = true;

                btnUploadFile.Enabled = false;
                btnDownloadFile.Enabled = false;
                btnDeleteFile.Enabled = false;

                btnSearchCustomer.Enabled = false;
                btnSearchKodeBank.Enabled = false;
                btnSearchRekCust.Enabled = false;
                btnUploadFile.Enabled = false;
                btnDownloadFile.Enabled = true;
                btnDeleteFile.Enabled = false;
                //txtCustomer.Enabled = false;
                // txtNameCustomer.Enabled = false;
                cmbPaymentMethod.Enabled = false;
                btnSearchRekCust.Enabled = false;
                txtNoGiro.Enabled = false;
                txtNotes.Enabled = false;
                txtKetTransfer.Enabled = false;
                dtJatuhTempo.Enabled = false;
                txtNominal.Enabled = false;
                dtRVDate.Enabled = false;
                //tia edit
                txtCustomer.Enabled = true;
                txtNameCustomer.Enabled = true;
                txtCustomer.ReadOnly = true;
                txtNameCustomer.ReadOnly = true;

                for (int i = 0; i < dgvReceiptVoucherDtl.Columns.Count; i++)
                {
                    dgvReceiptVoucherDtl.Columns[i].ReadOnly = true;
                    dgvReceiptVoucherDtl.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
                for (int i = 0; i < dgvCreditNote.Columns.Count; i++)
                {
                    dgvCreditNote.Columns[i].ReadOnly = true;
                    dgvCreditNote.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            //}
        }

        private void setValueBeforeEdit()
        {
            CNID.Clear();
            AmountCN.Clear();
            //get value before edit
            Query = "SELECT * FROM [ReceiptVoucher_Dtl] WHERE [RV_No] = @RV_No AND Jenis = 'NOTA CREDIT'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@RV_No", txtRVNo.Text);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    CNID.Add(Dr["Invoice_Id"].ToString());
                    AmountCN.Add(Convert.ToDecimal(Dr["Payment_Amount"]));
                }
                Dr.Close();
            }
        }

        private void populateCmbPaymentMethod()
        {
            cmbPaymentMethod.Items.Clear();
            //if (Mode == "New")
            //{
                Query = "SELECT * FROM [ISBS-NEW4].[dbo].[PaymentMode] WHERE PaymentModeName != 'TRANSFER'";
            //}
            //else
            //{
            //    Query = "SELECT * FROM [ISBS-NEW4].[dbo].[PaymentMode]";
            //}
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                        cmbPaymentMethod.Items.Add(Dr["PaymentModeName"].ToString());
                }
                Dr.Close();
            }
            if (cmbPaymentMethod.Text == "")
            {
                cmbPaymentMethod.SelectedIndex = 0;
            }
        }

        public void LockControlValues(System.Windows.Forms.Control Container)
        {
            try
            {
                foreach (Control ctrl in Container.Controls)
                {
                    if (ctrl.GetType() == typeof(TextBox))
                        ((TextBox)ctrl).ReadOnly = true;
                    if (ctrl.GetType() == typeof(ComboBox))
                        ((ComboBox)ctrl).Enabled = false;
                    if (ctrl.GetType() == typeof(CheckBox))
                        ((CheckBox)ctrl).Enabled = false;
                    if (ctrl.GetType() == typeof(Button))
                        ((Button)ctrl).Enabled = false;
                    if (ctrl.GetType() == typeof(DateTimePicker))
                        ((DateTimePicker)ctrl).Enabled = false;

                    dgvCreditNote.ReadOnly = true;
                    dgvCreditNote.DefaultCellStyle.BackColor = Color.LightGray;
                    dgvFileAttached.ReadOnly = true;
                    dgvFileAttached.DefaultCellStyle.BackColor = Color.LightGray;
                    dgvReceiptVoucherDtl.ReadOnly = true;
                    dgvReceiptVoucherDtl.DefaultCellStyle.BackColor = Color.LightGray;

                    if (ctrl.Controls.Count > 0)
                        LockControlValues(ctrl);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void populateCmbChequeResult()
        {
            cmbChequeResult.Items.Add("Cair");
            cmbChequeResult.Items.Add("Tunda");
            cmbChequeResult.Items.Add("Tolak");
        }

        private void HeaderReceiptVoucher_Load(object sender, EventArgs e)
        {
            populateCmbPaymentMethod();
            metroTabControl1.SelectedIndex = 0;
            populateCmbChequeResult();
            switch (Mode)
            {
                case "New":
                    ModeNew();
                    break;
                case "Edit":
                    ModeEdit();
                    break;
                case "BeforeEdit":
                    ModeView();
                    break;
            }
            CalculateRemainingPayment();
        }

        private void GetDataHeader()
        {
            if (RefId != "" && RefId != null)
            {
                txtRVNo.Text = RefId;
            }
            Query = "SELECT * FROM [ISBS-NEW4].[dbo].[ReceiptVoucher_H] WHERE [RV_No] = '" + txtRVNo.Text + "'";
            Con = ConnectionString.GetConnection();
            using (Cmd = new SqlCommand(Query, Con))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtRVDate.Value = Convert.ToDateTime(Dr["RV_Date"]);
                    txtCustomer.Text = Dr["Cust_Id"].ToString();
                    txtNameCustomer.Text = Dr["Cust_Name"].ToString();
                    cmbPaymentMethod.Text = Dr["Payment_Method"].ToString();
                    txtNominal.Text = String.Format("{0:#,##0.#0}",(Convert.ToDecimal(Dr["Total_Payment"])));
                    txtPaymentAmount.Text = String.Format("{0:#,##0.#0}", Convert.ToDecimal(Dr["Signed_Amount"]));
                    txtNotes.Text = Dr["Notes"].ToString();
                    txtNoGiro.Text = Dr["Giro_No"].ToString();
                    txtKetTransfer.Text = Dr["Receipt_No"].ToString();
                    txtNoRekCustomer.Text = Dr["Rek_Cust"].ToString();
                    txtKodeBank.Text = Dr["Bank_Id"].ToString();
                    txtBankName.Text = Dr["Bank_Name"].ToString();
                    dtJatuhTempo.Value = Dr["Payment_DueDate"] == System.DBNull.Value ? DateTime.Now : Convert.ToDateTime(Dr["Payment_DueDate"]);
                    dtDocDate.Value = Dr["CreatedDate"] == System.DBNull.Value ? DateTime.Now : Convert.ToDateTime(Dr["CreatedDate"]);
                    if (Dr["Tgl_Cair"] != System.DBNull.Value)
                    {
                        cmbChequeResult.Text = "Cair";
                        dtTglCheque.Value = (DateTime)Dr["Tgl_Cair"];
                    }
                    else if(Dr["Tgl_Tolak"] != System.DBNull.Value)
                    {
                        cmbChequeResult.Text = "Tolak";
                        dtTglCheque.Value = (DateTime)Dr["Tgl_Tolak"];
                    }
                    else if(Dr["Tgl_Pending"] != System.DBNull.Value)
                    {
                        cmbChequeResult.Text = "Tunda";
                        dtTglCheque.Value = (DateTime)Dr["Tgl_Pending"];
                    }
                }
                Dr.Close();
            }
            Con.Close();

        }

        private void RefreshGrid()
        {
            dgvReceiptVoucherDtl.Columns.Clear();

            //Established column header
            dgvReceiptVoucherDtl.ColumnCount = TableColName.Count();
            for (int i = 0; i < TableColName.Count(); i++)
            {
                dgvReceiptVoucherDtl.Columns[i].Name = TableColName[i];
                dgvReceiptVoucherDtl.Columns[i].HeaderText = TableColHeaderText[i];
                for (int j = 0; j < TableColVisibleTrue.Count(); j++)
                {
                    if(dgvReceiptVoucherDtl.Columns[i].HeaderText == TableColVisibleTrue[j])
                    {
                        dgvReceiptVoucherDtl.Columns[i].Visible = true;
                        break;
                    }
                    else
                    {
                        dgvReceiptVoucherDtl.Columns[i].Visible = false;
                    }
                }    
            }
            Con = ConnectionString.GetConnection();
            //populate datagridview
            Query = "SELECT * FROM [dbo].[ReceiptVoucher_Dtl] WHERE [RV_No] = '" + txtRVNo.Text + "' AND Jenis = 'INVOICE'";
            int x = 0;
            using (Cmd = new SqlCommand(Query, Con))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dgvReceiptVoucherDtl.Rows.Add(1);
                    for (int i = 0; i < TableColName.Count(); i++)
                    {
                        if (TableColName[i].Contains("Date"))
                        {
                            dgvReceiptVoucherDtl.Rows[x].Cells[TableColName[i]].Value = Convert.ToDateTime(Dr[TableColName[i]]).ToString("dd/MM/yyyy");
                        }
                        else if (TableColName[i].Contains("Amount"))
                        {
                            dgvReceiptVoucherDtl.Rows[x].Cells[TableColName[i]].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(Dr[TableColName[i]]));
                            if (TableColName[i] == "Payment_Amount")
                            {
                                Amount.Add(Convert.ToDecimal(Dr[TableColName[i]]));
                            }
                        }
                        else if (TableColName[i] != "No")
                        {
                            dgvReceiptVoucherDtl.Rows[x].Cells[TableColName[i]].Value = Dr[TableColName[i]];
                            if (TableColName[i] == "Invoice_Id")
                            {
                                InvoiceId.Add(Dr[TableColName[i]].ToString());
                            }
                            if (TableColName[i] == "SeqNo")
                            {
                                SeqNo.Add(Convert.ToInt32(Dr[TableColName[i]]));
                            }
                        }
                    }
                    x++;
                }
                Dr.Close();
            }
            Con.Close();

            //assign value to invoice amount and remaning amount from existing row in dtagridview
            if (dgvReceiptVoucherDtl.Rows.Count > 0)
            {
                decimal InvoiceAmountTotal = 0;
                for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
                {
                    dgvReceiptVoucherDtl.Rows[i].Cells["No"].Value = i + 1;
                    InvoiceAmountTotal += Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["AR_Amount"].Value);
                }
                txtInvoiceAmount.Text = String.Format("{0:#,##0.#0}",InvoiceAmountTotal);
                if (txtNominal.Text != "")
                {
                    txtRemainingAmount.Text = String.Format("{0:#,##0.#0}", (Convert.ToDecimal(txtNominal.Text) - Convert.ToDecimal(txtPaymentAmount.Text) + Convert.ToDecimal(txtCreditAmount.Text)));
                }
            }

            if (PermissionTreasury() == true)
            {
                //DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                //chk.Name = "Check";
                //chk.DefaultCellStyle.BackColor = Color.White;
                //dgvReceiptVoucherDtl.Columns.Insert(0, chk);

                //DataGridViewCell cell = new DataGridViewTextBoxCell();

                //DataGridViewColumn ActualAmount = new DataGridViewColumn();
                //ActualAmount.CellTemplate = cell;
                //ActualAmount.Name = "ActualAmount";
                //ActualAmount.HeaderText = "Actual Amount";
                //ActualAmount.ValueType = typeof(string);
                //dgvReceiptVoucherDtl.Columns.Add(ActualAmount);

                //DataGridViewColumn Notes = new DataGridViewColumn();
                //Notes.CellTemplate = cell;
                //Notes.Name = "Notes";
                //Notes.HeaderText = "Notes";
                //Notes.ValueType = typeof(string);
                //dgvReceiptVoucherDtl.Columns.Add(Notes);

                //dgvReceiptVoucherDtl.Columns[3].Frozen = true;
            }
        }

        private void refreshGridCreditNote()
        {
            dgvCreditNote.Columns.Clear();
            
            //Established column header
            dgvCreditNote.ColumnCount = TableColCNName.Count();
            for (int i = 0; i < TableColCNName.Count(); i++)
            {
                dgvCreditNote.Columns[i].Name = TableColCNName[i];
                dgvCreditNote.Columns[i].HeaderText = TableColCNHeaderText[i];
                for (int j = 0; j < TableColCNVisibleTrue.Count(); j++)
                {
                    if(dgvCreditNote.Columns[i].HeaderText == TableColCNVisibleTrue[j])
                    {
                        dgvCreditNote.Columns[i].Visible = true;
                        break;
                    }
                    else
                    {
                        dgvCreditNote.Columns[i].Visible = false;
                    }
                }
                
            }
            if (txtCustomer.Text == "")
            {
                return;
            }
            //Get all Nota Credit from the targeted customer
            Query = "SELECT * FROM [dbo].[NotaCreditH] WHERE [AccountNum] = @AccountNum AND [TotalAmount] > [Deduct]";
            int x = 0;
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@AccountNum",txtCustomer.Text);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dgvCreditNote.Rows.Add(1);
                    for (int i = 0; i < TableColCNName.Count(); i++)
                    {
                        if (TableColCNName[i].Contains("Date"))
                        {
                            dgvCreditNote.Rows[x].Cells[TableColCNName[i]].Value = Dr[TableColCNName[i]]==System.DBNull.Value ?"":Convert.ToDateTime(Dr[TableColCNName[i]]).ToString("dd/MM/yyyy");
                        }
                        else if (TableColCNName[i] == "Amount")
                        {
                            dgvCreditNote.Rows[x].Cells[TableColCNName[i]].Value = "0.00";
                        }
                        else if (TableColCNName[i] == "Remaining Amount")
                        {
                            dgvCreditNote.Rows[x].Cells[TableColCNName[i]].Value =  String.Format("{0:#,##0.#0}",(Convert.ToDecimal(Dr["TotalAmount"])-Convert.ToDecimal(Dr["Deduct"]))) ;
                        }
                        else if (TableColCNName[i].Contains("Amount"))
                        {
                            dgvCreditNote.Rows[x].Cells[TableColCNName[i]].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(Dr[TableColCNName[i]]));
                        }
                        else if (TableColCNName[i] == "Check")
                        {
                            DataGridViewCheckBoxCell chkBox = new DataGridViewCheckBoxCell();
                            chkBox.Value = false;
                            dgvCreditNote.Rows[x].Cells[TableColCNName[i]] = chkBox;
                        }
                        else if (TableColCNName[i] != "No")
                        {
                            dgvCreditNote.Rows[x].Cells[TableColCNName[i]].Value = Dr[TableColCNName[i]];
                        }
                        else if (TableColCNName[i] == "No")
                        {
                            dgvCreditNote.Rows[x].Cells[TableColCNName[i]].Value = (x + 1);
                        }
                    }
                    x++;
                }
                Dr.Close();
            }
            
            
            //Set check box from the notacredit that are used in the this RV
            if (txtRVNo.Text != null && txtRVNo.Text != "")
            {
                Query = "SELECT * FROM [ReceiptVoucher_Dtl] WHERE [Jenis] = 'NOTA CREDIT' AND [RV_No] = @RV_No";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@RV_No", txtRVNo.Text);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            bool hasrow = dgvCreditNote.Rows.Cast<DataGridViewRow>().Any(r => r.Cells["CN_No"].Value.ToString().Equals(Dr["Invoice_Id"].ToString()));
                            if (hasrow == true)
                            {
                                DataGridViewRow row = dgvCreditNote.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["CN_No"].Value.ToString().Equals(Dr["Invoice_Id"].ToString())).First();
                                if (Dr["Invoice_Id"].ToString() == row.Cells["CN_No"].Value.ToString())
                                {
                                    row.Cells["Check"].Value = true;
                                    row.Cells["Amount"].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(Dr["Payment_Amount"]));
                                    row.Cells["Remaining Amount"].Value = String.Format("{0:#,##0.#0}", (Convert.ToDecimal(row.Cells["Remaining Amount"].Value) + Convert.ToDecimal(Dr["Payment_Amount"])));
                                }
                                row.Cells["Remaining Amount"].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(row.Cells["Remaining Amount"].Value) + Convert.ToDecimal(Dr["Payment_Amount"]));
                            }
                            else
                            {
                                //{ "No", "Check", "CN Date", "CN No", "CNMode", "NRJId", "DueDate", "AccountNum", "AccountName", "CurrencyId", "ExchRate", "TaxStatusCode", "TaxPercent", "PPHTaxStatusCode", "PPHTaxPercent", "TotalAmount", "Remaining Amount", "Amount", "TaxBaseAmount", "TaxAmount", "PPHTaxAmount", "NPWP", "TaxNum", "TaxAddress", "TaxName", "TaxDate", "TransStatus", "Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy", "Deduct", "SO_Id" };
                                Query = "SELECT * FROM [dbo].[NotaCreditH] WHERE [CN_No] = @CN_No";
                                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                                {
                                    Cmd.Parameters.AddWithValue("@CN_No", Dr["Invoice_Id"]);
                                    SqlDataReader Dr2 = Cmd.ExecuteReader();
                                    while (Dr2.Read())
                                    {
                                        dgvCreditNote.Rows.Add(1);
                                        for (int i = 0; i < TableColCNName.Count(); i++)
                                        {
                                            if (TableColCNName[i].Contains("Date"))
                                            {
                                                dgvCreditNote.Rows[dgvCreditNote.Rows.Count - 1].Cells[TableColCNName[i]].Value = Dr2[TableColCNName[i]] == System.DBNull.Value ? "" : Convert.ToDateTime(Dr2[TableColCNName[i]]).ToString("dd/MM/yyyy");
                                            }
                                            else if (TableColCNName[i] == "Amount")
                                            {
                                                dgvCreditNote.Rows[dgvCreditNote.Rows.Count - 1].Cells[TableColCNName[i]].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(Dr["Payment_Amount"]));
                                            }
                                            else if (TableColCNName[i] == "Remaining Amount")
                                            {
                                                dgvCreditNote.Rows[dgvCreditNote.Rows.Count - 1].Cells[TableColCNName[i]].Value = String.Format("{0:#,##0.#0}", (Convert.ToDecimal(Dr2["TotalAmount"]) - Convert.ToDecimal(Dr2["Deduct"])));
                                            }
                                            else if (TableColCNName[i].Contains("Amount"))
                                            {
                                                dgvCreditNote.Rows[dgvCreditNote.Rows.Count - 1].Cells[TableColCNName[i]].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(Dr2[TableColCNName[i]]));
                                            }
                                            else if (TableColCNName[i] == "Check")
                                            {
                                                DataGridViewCheckBoxCell chkBox = new DataGridViewCheckBoxCell();
                                                chkBox.Value = true;
                                                dgvCreditNote.Rows[dgvCreditNote.Rows.Count - 1].Cells[TableColCNName[i]] = chkBox;
                                            }
                                            else if (TableColCNName[i] != "No")
                                            {
                                                dgvCreditNote.Rows[dgvCreditNote.Rows.Count - 1].Cells[TableColCNName[i]].Value = Dr2[TableColCNName[i]];
                                            }
                                            else if (TableColCNName[i] == "No")
                                            {
                                                dgvCreditNote.Rows[dgvCreditNote.Rows.Count - 1].Cells[TableColCNName[i]].Value = dgvCreditNote.Rows.Count;
                                            }
                                        }
                                    }
                                    Dr2.Close();
                                }
                            }

                            //for (int i = 0; i < dgvCreditNote.Rows.Count; i++)
                            //{
                            //    DataGridViewCheckBoxCell chkBox = new DataGridViewCheckBoxCell();
                            //    chkBox.Value = false;
                            //    if (Dr["Invoice_Id"].ToString() == dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString())
                            //    {
                            //        chkBox.Value = true;
                            //        dgvCreditNote.Rows[i].Cells["Amount"].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(Dr["Payment_Amount"]));
                            //        dgvCreditNote.Rows[i].Cells["Remaining Amount"].Value = String.Format("{0:#,##0.#0}",(Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Remaining Amount"].Value) + Convert.ToDecimal(Dr["Payment_Amount"])));
                            //    }
                            //    dgvCreditNote.Rows[i].Cells["Check"] = chkBox;
                            //    dgvCreditNote.Rows[i].Cells["Remaining Amount"].Value = String.Format("{0:#,##0.#0}",Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Remaining Amount"].Value) + Convert.ToDecimal(Dr["Payment_Amount"]));
                            //}
                        }
                    }
                    Dr.Close();
                }
            }
            dgvCreditNote.Columns["Check"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvCreditNote.Columns["CN_No"].Frozen = true;
            CalculateCreditAmount();
        }

        private void UpdateCNReff(SqlConnection con)
        {
            //update new value
            for (int i = 0; i < dgvCreditNote.Rows.Count; i++)
            {
                //uncheck
                if (Convert.ToBoolean(dgvCreditNote.Rows[i].Cells["Check"].Value) != true)
                {
                    if (CNID.Contains(dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString()))
                    {
                        int x = CNID.IndexOf(dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString(), 0);
                        Query = "UPDATE [dbo].[NotaCreditH] SET [Deduct] =([Deduct] - @Amount2) WHERE [CN_No] = @CN_No";
                        using (Cmd = new SqlCommand(Query, con))
                        {
                            Cmd.Parameters.AddWithValue("@Amount2", AmountCN[x]);
                            Cmd.Parameters.AddWithValue("@CN_No", dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString());
                            Cmd.ExecuteNonQuery();
                        }
                    }
                }
                //checked but value changed
                else
                {
                    if (CNID.Contains(dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString()))
                    {
                        int x = CNID.IndexOf(dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString(), 0);
                        Query = "UPDATE [dbo].[NotaCreditH] SET [Deduct] =([Deduct] + @Amount - @Amount2) WHERE [CN_No] = @CN_No";
                        using (Cmd = new SqlCommand(Query, con))
                        {
                            Cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Amount"].Value));
                            Cmd.Parameters.AddWithValue("@Amount2", AmountCN[x]);
                            Cmd.Parameters.AddWithValue("@CN_No", dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString());
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        Query = "UPDATE [dbo].[NotaCreditH] SET [Deduct] =([Deduct] + @Amount) WHERE [CN_No] = @CN_No";
                        using (Cmd = new SqlCommand(Query, con))
                        {
                            Cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Amount"].Value));
                            Cmd.Parameters.AddWithValue("@CN_No", dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString());
                            Cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void RefreshGridAttachment()
        {
            if (dgvFileAttached.Columns.Count == 0)
            {
                dgvFileAttached.ColumnCount = 6;
                dgvFileAttached.Columns[0].Name = "No";
                dgvFileAttached.Columns[2].Name = "FileName"; dgvFileAttached.Columns[2].HeaderText = "File Name";
                dgvFileAttached.Columns[3].Name = "ContentType";
                dgvFileAttached.Columns[4].Name = "FileSize"; dgvFileAttached.Columns[4].HeaderText = "File Size (Kb)";
                dgvFileAttached.Columns[5].Name = "Attachment";
                dgvFileAttached.Columns[1].Name = "FileId";
            }

            if (txtRVNo.Text != "")
            {
                dgvFileAttached.Rows.Clear();
                Query = "SELECT * FROM [dbo].[tblAttachments] WHERE [ReffTableName] = 'ReceiptVoucherH' AND [ReffTransID] = '" + txtRVNo.Text + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    int i = 0;
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        dgvFileAttached.Rows.Add(1);
                        dgvFileAttached.Rows[i].Cells["No"].Value = dgvFileAttached.Rows.Count;
                        dgvFileAttached.Rows[i].Cells["FileId"].Value = Dr["id"];
                        dgvFileAttached.Rows[i].Cells["FileName"].Value = Dr["fileName"];
                        dgvFileAttached.Rows[i].Cells["ContentType"].Value = Dr["ContentType"];
                        dgvFileAttached.Rows[i].Cells["FileSize"].Value = Dr["fileSize"];
                        attachment.Add((byte[])Dr["attachment"]);
                        i++;
                    }
                    Dr.Close();
                }
            }
            dgvFileAttached.Columns["FileId"].Visible = false;
        }

        private string checkRVStatus()
        {
            string status = "01";
            Query = "SELECT TOP 1 [StatusCode] FROM [dbo].[ReceiptVoucher_H] WHERE [RV_No]=@RV_No;";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@RV_No",txtRVNo.Text);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        status = Dr["StatusCode"] == System.DBNull.Value ? "01" : Dr["StatusCode"].ToString();
                    }
                }
                Dr.Close();
            }
            return status;
        }

        private string validation()
        {
            string msg = "";

            if (checkRVStatus() != "01")
            {
                msg += "-RV sudah tidak dapat diedit.\n";
            }

            switch (cmbPaymentMethod.Text.ToUpper())
            {
                case "TRANSFER":
                    if (dgvFileAttached.Rows.Count < 1)
                    {
                        msg += "-Belum terlampir document.\n";
                    }
                    if (txtNoRekCustomer.Text == "")
                    {
                        msg += "-No Rek Customer masih kosong.\n";
                    }
                    break;
                case "CHEQUE":
                    if (dgvFileAttached.Rows.Count < 1)
                    {
                        msg += "-Belum terlampir document.\n";
                    }
                    //if (txtKetTransfer.Text == "" || txtKetTransfer == null)
                    //{
                    //    msg += "-Keterangan Transfer masih kosong.\n";
                    //}
                    if (txtNoGiro.Text == "" || txtNoGiro == null)
                    {
                        msg += "-No Giro/Cheque masih kosong.\n";
                    }
                    break;
                case "GIRO":
                    if (dgvFileAttached.Rows.Count < 1)
                    {
                        msg += "-Belum terlampir document.\n";
                    }
                    if (txtNoGiro.Text == "" || txtNoGiro == null)
                    {
                        msg += "-No Giro masih kosong.\n";
                    }
                    break;
                case "CASH":
                    if (txtKetTransfer.Text == "" || txtKetTransfer == null)
                    {
                        msg += "-Keterangan Transfer masih kosong.\n";
                    }
                    break;
            }
            
            if (txtKodeBank.Text == "" || txtKodeBank == null)
            {
                msg += "-Kode Bank masih kosong.\n";
            }
            if (txtCustomer.Text == "" || txtCustomer == null)
            {
                msg += "-Customer masih belum dipilih.\n";
            }
            if (dgvReceiptVoucherDtl.Rows.Count < 1)
            {
                msg += "-Invoice belum dipilih.\n";
            }

            if (txtNominal.Text == "" || Convert.ToDecimal(txtNominal.Text) == 0)
            {
                msg += "-Nominal Amount tidak boleh kosong/0.\n";
            }
            for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value) == 0)
                {
                    msg += "-Payment Invoice pada Row '" + i.ToString() + "' tidak boleh bernilai 0.\n";
                }
            }
            decimal totalpayment = dgvReceiptVoucherDtl.Rows.Cast<DataGridViewRow>().Select(r => Convert.ToDecimal(r.Cells["Payment_Amount"].Value)).Sum();
            //if (totalpayment == 0)
            //{
            //    bool HasCreditNote = dgvCreditNote.Rows.Cast<DataGridViewRow>().Any(r => Convert.ToBoolean(r.Cells["Check"].Value) == true);
            //    decimal CreditNoteSum = dgvCreditNote.Rows.Cast<DataGridViewRow>().Select(r => Convert.ToDecimal(r.Cells["Amount"].Value)).Sum();
            //    if (HasCreditNote == true)
            //    {
            //        if (CreditNoteSum <= 0)
            //        {
            //            msg += "-Nilai payment masih 0.\n";
            //        }
            //        else
            //        {
            //            string submsg = "(Payment sepenuhnya dari Credit Note)";
            //            if (!(txtNotes.Text.Contains(submsg)))
            //                txtNotes.Text += submsg;
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
            //        {
            //            if (Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value) == 0)
            //            {
            //                msg += "-Payment Invoice pada Row '" + i.ToString() + "' tidak boleh bernilai 0.\n";
            //            }
            //        }
            //    }
            //}

            //if (totalpayment > 0)
            //{
            //    if (txtNominal.Text == "" || txtNominal == null)
            //    {
            //        msg += "-Total Payment masih kosong.\n";
            //    }
            //    else if (Convert.ToDecimal(txtNominal.Text) == 0)
            //    {
            //        msg += "-Total Payment bernilai 0.\n";
            //    }
            //}

            if (Convert.ToDecimal(txtRemainingAmount.Text) < 0)
            {
                    msg += "-Amount payment berlebihan, silhakan kurangi amount payment atau amount Credit nota.\n";
            }

            //compare value before edit and after edit, and check the remaning is it still enough or not
            for (int i = 0; i < dgvCreditNote.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvCreditNote.Rows[i].Cells["Check"].Value) != true)
                {
                    continue;
                }
                if (Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Amount"].Value) == 0)
                {
                    msg += "-Amount payment Credit nota row "+(i+1)+" bernilai 0.\n";
                    continue;
                }

                Query = "SELECT * FROM [NotaCreditH] WHERE [AccountNum]=@AccountNum ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@AccountNum",txtCustomer.Text);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (CNID.Contains(dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString()))
                        {
                            int x = CNID.IndexOf(dgvCreditNote.Rows[i].Cells["CN_No"].Value.ToString(), 0);
                            decimal tes = AmountCN[x];
                            if (Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Amount"].Value) > (Convert.ToDecimal(Dr["TotalAmount"]) - Convert.ToDecimal(Dr["Deduct"]) + AmountCN[x]))
                            {
                                msg += "-Remaining amount untuk "+Dr["CN_No"].ToString()+" tidak mencukupi.\n\r";
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Amount"].Value) > (Convert.ToDecimal(Dr["TotalAmount"]) - Convert.ToDecimal(Dr["Deduct"])))
                            {
                                msg += "-Remaining amount untuk " + Dr["CN_No"].ToString() + " tidak mencukupi.\n\r";
                            }
                        }
                    }
                    Dr.Close();
                }
            }

            if ((Convert.ToDecimal(txtRemainingAmount.Text)) > 0)
            {
                msg += "-Amount pembayaran kelebihan (RemainingAmountTotal tidak 0).\n\r";
            }

            if (cmbChequeResult.Visible == true && (cmbChequeResult.Text.ToUpper().Trim() == "TUNDA" || cmbChequeResult.Text.ToUpper().Trim() == "TOLAK"))
            {
                if (txtNotes.Text == "")
                {
                    msg += "-Notes alasan tunda/tolak dibutuhkan.\n";
                }
            }

            return msg;
        }

        private void CalculateRemainingPayment()
        {
            if (dgvReceiptVoucherDtl.Rows.Count > 0)
            {
                decimal InvoiceAmountTotal = 0;
                decimal PaymentAmountTotal = 0;

                for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
                {
                    dgvReceiptVoucherDtl.Rows[i].Cells["No"].Value = i + 1;
                    InvoiceAmountTotal += Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["AR_Amount"].Value);
                    if (dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value != null && dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value.ToString() != ".")
                    {
                        PaymentAmountTotal += Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value);
                    }
                    else
                    {
                        dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value = 0;
                    }
                }
                
                txtInvoiceAmount.Text = String.Format("{0:#,##0.#0}", Convert.ToDecimal(InvoiceAmountTotal));
                txtPaymentAmount.Text = String.Format("{0:#,##0.#0}", Convert.ToDecimal(PaymentAmountTotal));
                if (txtNominal.Text != "")
                {
                    txtRemainingAmount.Text = String.Format("{0:#,##0.#0}", (Convert.ToDecimal(txtNominal.Text) - Convert.ToDecimal(txtPaymentAmount.Text) + Convert.ToDecimal(txtCreditAmount.Text)));
                }
            }
        }

        private void UpdateInvoiceSettleAmount(SqlConnection Con)
        {
            if (InvoiceId.Count > 0)
            {
                for (int i = 0; i < InvoiceId.Count; i++)
                {
                    Query = "UPDATE [dbo].[CustInvoice_H] SET [Settle_Amount]-=" + Amount[i] + " WHERE [Invoice_Id] = '" + InvoiceId[i] + "'";
                    using (Cmd = new SqlCommand(Query, Con))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
            }

            for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
            {
                Query = "UPDATE [dbo].[CustInvoice_H] SET [Settle_Amount]+=" + Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value) + " WHERE [Invoice_Id] = '" + dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Id"].Value + "'";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateInvoiceStatus(SqlConnection Con)
        {
            string Status="";
            for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
            {
                Query = "SELECT * FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] = '"+dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Id"].Value.ToString()+"'";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"])==0)
                        {
                            Status = "41";
                        }
                        else
                        {
                            Status = "42";
                        }
                    }
                    Dr.Close();
                }
                if (Status != "")
                {
                    Query = "UPDATE [dbo].[CustInvoice_H] SET [TransStatus]='" + Status + "' WHERE [Invoice_Id] = '" + dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Id"].Value + "'";
                    using (Cmd = new SqlCommand(Query, Con))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void dgvReceiptVoucherDtl_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //CalculateRemainingPayment();
        }

        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            string Cust = txtCustomer.Text;
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "CustTable", "and 1=1", "a.*", "CustTable a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                txtCustomer.Text = SearchV2.data[0];
                if(Cust != txtCustomer.Text)
                {
                    dgvReceiptVoucherDtl.Rows.Clear();
                }
                Con = ConnectionString.GetConnection();
                Query = "select [CustName] from CustTable where [CustId] = '" + txtCustomer.Text + "'";
                Cmd = new SqlCommand(Query, Con);
                txtNameCustomer.Text = Cmd.ExecuteScalar().ToString();
                Con.Close();
            }
            refreshGridCreditNote();
            for (int i = 0; i < dgvCreditNote.Columns.Count; i++)
            {
                for (int j = 0; j < TableColCNReadOnlyFalse.Count(); j++)
                {
                    if (dgvCreditNote.Columns[i].HeaderText == TableColCNReadOnlyFalse[j])
                    {
                        dgvCreditNote.Columns[i].ReadOnly = false;
                        dgvCreditNote.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        break;
                    }
                    else
                    {
                        dgvCreditNote.Columns[i].ReadOnly = true;
                        dgvCreditNote.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
            }
        }

        private void btnSearchRekCust_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "Rekening", "and 1=1", "a.*", "Rekening a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                txtNoRekCustomer.Text = SearchV2.data[1];
                txtBankCustomer.Text = SearchV2.data[3];
            }
        }

        private void btnSearchKodeBank_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "VIEW_tblBank", "and 1=1", "a.*", "VIEW_tblBank a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                txtKodeBank.Text = SearchV2.data[0];
                txtBankName.Text = SearchV2.data[1];
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (checkRVStatus() != "01")
            {
                MessageBox.Show("RV sudah tidak dapat diedit.");
                return;
            }
            if (PermissionTreasury() == true)
            {
                return;
            }
            Mode = "Edit";
            ModeEdit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";
            ModeView();
            CalculateRemainingPayment();
        }

        
        private void btnUploadFile_Click(object sender, EventArgs e)
        {

            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            choofdlog.FilterIndex = 3;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                List<string> FileName = new List<string>();
                List<string> Extension = new List<string>();
                List<string> sSelectedFile = new List<string>();

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

                    if (dgvFileAttached.Columns.Count == 0)
                    {
                        dgvFileAttached.ColumnCount = 6;
                        dgvFileAttached.Columns[0].Name = "No"; 
                        dgvFileAttached.Columns[2].Name = "FileName"; dgvFileAttached.Columns[2].HeaderText = "File Name";
                        dgvFileAttached.Columns[3].Name = "ContentType"; 
                        dgvFileAttached.Columns[4].Name = "FileSize"; dgvFileAttached.Columns[4].HeaderText = "File Size (Kb)";
                        dgvFileAttached.Columns[5].Name = "Attachment"; 
                        dgvFileAttached.Columns[1].Name = "FileId"; 
                    }
                    this.dgvFileAttached.Rows.Add(dgvFileAttached.Rows.Count+1,"", FileName[i], Extension[i], filesize.ToString(), System.Text.Encoding.UTF8.GetString(data));
                    attachment.Add(data);
                    i++;
                }
                dgvFileAttached.Columns["FileId"].Visible = false;
            }
            
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (dgvFileAttached.RowCount > 0)
            {
                if (dgvFileAttached.CurrentRow.Index > -1)
                {
                    attachment.RemoveAt(dgvFileAttached.CurrentRow.Index);
                    dgvFileAttached.Rows.RemoveAt(dgvFileAttached.CurrentRow.Index);
                }
            }
        }

        private void btnDownloadFile_Click(object sender, EventArgs e)
        {
            if (dgvFileAttached.Rows.Count > 0)
            {
                if (dgvFileAttached.SelectedRows != null && dgvFileAttached.CurrentRow.Cells["FileId"].Value != null && dgvFileAttached.CurrentRow.Cells["FileId"].Value != "")
                {
                    String fileid = dgvFileAttached.CurrentRow.Cells["FileId"].Value == null ? "" : dgvFileAttached.CurrentRow.Cells["FileId"].Value.ToString();
                    String fileName = dgvFileAttached.CurrentRow.Cells["FileName"].Value == null ? "" : dgvFileAttached.CurrentRow.Cells["FileName"].Value.ToString();
                    String ContentType = dgvFileAttached.CurrentRow.Cells["ContentType"].Value == null ? "" : dgvFileAttached.CurrentRow.Cells["ContentType"].Value.ToString();

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

                    Con = ConnectionString.GetConnection();
                    Query = "Select Attachment From tblAttachments Where Id = '" + fileid + "'";
                    Cmd = new SqlCommand(Query, Con);

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
                    MessageBox.Show("Attachment belom tersimpan pada database, sehingga tidak dapat di-download.");
                }
            }
        }

        private void txtCustomer_Leave(object sender, EventArgs e)
        {
            if (txtCustomer.Text != "" && CustEnter != txtCustomer.Text)
            {
                int i = 0;
                Query = "Select Count(*) From CustTable WHERE [CustId] like '%" + txtCustomer.Text + "%'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    i = Convert.ToInt32(Cmd.ExecuteScalar());
                }
                if (i == 1)
                {
                    Query = "Select [CustId],[CustName] From CustTable WHERE [CustId] like '%" + txtCustomer.Text + "%'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            txtCustomer.Text = Dr["CustId"].ToString();
                            txtNameCustomer.Text = Dr["CustName"].ToString();
                            if (txtCustomer.Text != CustEnter)
                            {
                                dgvReceiptVoucherDtl.Rows.Clear();
                            }
                        }
                        Dr.Close();
                    }
                }
                else if (i != 0 && i > 1)
                {
                    SearchV2 f = new SearchV2();
                    f.SetMode("No");
                    f.SetSchemaTable("dbo", "CustTable", "and 1=1", "a.*", "CustTable a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        txtNameCustomer.Text = SearchV2.data[1];
                        txtCustomer.Text = SearchV2.data[0];
                        if (txtCustomer.Text != CustEnter)
                        {
                            dgvReceiptVoucherDtl.Rows.Clear();
                        }
                    }
                    else
                    {
                        txtNameCustomer.Text = "";
                        txtCustomer.Text = "";
                        dgvReceiptVoucherDtl.Rows.Clear();
                    }
                }
                else
                {
                    txtNameCustomer.Text = "";
                    txtCustomer.Text = "";
                    dgvReceiptVoucherDtl.Rows.Clear();
                }
            }
        }

        private void txtNameCustomer_Leave(object sender, EventArgs e)
        {
            if (txtNameCustomer.Text != "" && NameCustEnter != txtNameCustomer.Text)
            {
                int i = 0;
                Query = "Select Count(*) From CustTable WHERE [CustName] like '%" + txtNameCustomer.Text + "%'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    i = Convert.ToInt32(Cmd.ExecuteScalar());
                }
                if (i == 1)
                {
                    Query = "Select [CustId],[CustName] From CustTable WHERE [CustName] like '%" + txtNameCustomer.Text + "%'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            txtCustomer.Text = Dr["CustId"].ToString();
                            txtNameCustomer.Text = Dr["CustName"].ToString();
                            if (txtNameCustomer.Text != NameCustEnter)
                            {
                                dgvReceiptVoucherDtl.Rows.Clear();
                            }
                        }
                        Dr.Close();
                    }
                }
                else if (i != 0 && i > 1)
                {
                    SearchV2 f = new SearchV2();
                    f.SetMode("No");
                    f.SetSchemaTable("dbo", "CustTable", "and 1=1", "a.*", "CustTable a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        txtNameCustomer.Text = SearchV2.data[1];
                        txtCustomer.Text = SearchV2.data[0];
                        if (txtNameCustomer.Text != NameCustEnter)
                        {
                            dgvReceiptVoucherDtl.Rows.Clear();
                        }
                    }
                    else
                    {
                        txtNameCustomer.Text = "";
                        txtCustomer.Text = "";
                        dgvReceiptVoucherDtl.Rows.Clear();
                    }
                }
                else
                {
                    txtNameCustomer.Text = "";
                    txtCustomer.Text = "";
                    dgvReceiptVoucherDtl.Rows.Clear();
                }
            }
            
        }

        string CustEnter = "";
        private void txtCustomer_Enter(object sender, EventArgs e)
        {
            CustEnter = txtCustomer.Text;
        }

        string NameCustEnter = "";
        private void txtNameCustomer_Enter(object sender, EventArgs e)
        {
            NameCustEnter = txtNameCustomer.Text;
        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtKetTransfer.Text = "";
            dtJatuhTempo.Value = DateTime.Now;
            txtKetTransfer.Text = "";
            txtNoRekCustomer.Text = "";
            txtBankCustomer.Text = "";
            if (cmbPaymentMethod.Text.ToUpper() == "TRANSFER")
            {
                btnSearchRekCust.Enabled = true;

                txtNoGiro.Enabled = false;
                dtJatuhTempo.Enabled = false;
                txtKetTransfer.Enabled = false;
            }
            else if (cmbPaymentMethod.Text.ToUpper() == "GIRO")
            {
                txtNoGiro.Enabled = true;
                dtJatuhTempo.Enabled = true;

                btnSearchRekCust.Enabled = false;
                txtKetTransfer.Enabled = false;
            }
            else if (cmbPaymentMethod.Text.ToUpper() == "CHEQUE")
            {
                txtKetTransfer.Enabled = false;
                dtJatuhTempo.Enabled = true;

                btnSearchRekCust.Enabled = false;
                txtNoGiro.Enabled = true;
            }
            else if (cmbPaymentMethod.Text.ToUpper() == "CASH")
            {
                txtKetTransfer.Enabled = true;

                txtNoGiro.Enabled = false;
                btnSearchRekCust.Enabled = false;
                dtJatuhTempo.Enabled = false;
            }
            dgvReceiptVoucherDtl.Rows.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            setValueBeforeEdit(); //to trace the value before being edited, so it can return the previous value
            string msg = validation();
            if (msg != "")
            {
                MetroFramework.MetroMessageBox.Show(Owner, msg);
                return;
            }
            using (Scope = new TransactionScope())
            {
                Con = ConnectionString.GetConnection();
                try
                {
                    if (Mode.ToUpper() == "NEW")
                    {
                        string Jenis = "RV", Kode = "RV";
                        string RVNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Con, Cmd);
                        txtRVNo.Text = RVNumber;
                        CreditLimit(Con);
                        InsertReceiptVoucherH(Con);
                        InsertReceiptVoucherDtl(Con);
                        UpdateCNReff(Con);
                        InsertAttachment(Con);
                        UpdateInvoiceSettleAmount(Con);
                        UpdateInvoiceStatus(Con);
                        InsertBankSystem(Con);
                        
                        //Begin
                        //Created By : Joshua
                        //Created Date : 07 Sept 2018
                        //Desc : Create Journal
                        CreateJournal(RVNumber);
                        //End
                        ListMethod.StatusLogCustomer("HeaderReceiptVoucher", "ReceiptVoucher", txtCustomer.Text, "01", "", "", "", "", "");
                        InsertRVLog(RVNumber, "N", "01", "", Con);

                        Con.Close();
                        ModeView();
                        Parent.RefreshGrid();
                        Scope.Complete();
                        MessageBox.Show("Receipt Voucher Berhasil di-Create.");
                    }
                    else if (Mode.ToUpper() == "EDIT")
                    {
                        CreditLimit(Con);
                        UpdateReceiptVoucherH(Con);
                        //delete previous attachment amad RV Detail
                        Query = "DELETE FROM [dbo].[tblAttachments] WHERE [ReffTransID] = '" + txtRVNo.Text + "'";
                        using (Cmd = new SqlCommand(Query, Con))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        Query = "DELETE FROM [dbo].[ReceiptVoucher_Dtl] WHERE [RV_No] = '" + txtRVNo.Text + "'";
                        using (Cmd = new SqlCommand(Query, Con))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        //end======================================
                        InsertReceiptVoucherDtl(Con);
                        UpdateCNReff(Con);
                        InsertAttachment(Con);
                        UpdateInvoiceSettleAmount(Con);
                        UpdateInvoiceStatus(Con);
                        UpdateBankSystem(Con);

                        UpdateJournal();
                        if (Journal == true)
                        {
                            Journal = false;
                            goto Outer;
                        }
                        ListMethod.StatusLogCustomer("HeaderReceiptVoucher", "ReceiptVoucher", txtCustomer.Text, "01", "", "", "", "", "");
                        InsertRVLog(txtRVNo.Text, "E", "01", "", Con);

                        Con.Close();
                        ModeView();
                        Parent.RefreshGrid();
                        Scope.Complete();
                        MessageBox.Show("Receipt Voucher Berhasil di-Update.");

                        Outer: ;
                    }
                }
                catch (TransactionException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally 
                {
                    
                }
            }
        }

        private void InsertRVLog(string RVID, string action, string status, string deskripsi, SqlConnection Con)
        {
            string statustransaksi = "";
            Query = "SELECT [Deskripsi] FROM [dbo].[TransStatusTable] WHERE [TransCode] = 'ReceiptVoucher' AND [StatusCode]=@StatusCode";
            using (Cmd = new SqlCommand(Query,Con))
            {
                Cmd.Parameters.AddWithValue("@StatusCode", status);
                statustransaksi = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
            }

            Query = "INSERT INTO [dbo].[ReceiptVoucher_LogTable] ([TransaksiID],[Deskripsi],[StatusTransaksi],[Action],[UserID],[LogDatetime]) ";
            Query += " VALUES (@TransaksiID,@Deskripsi,@StatusTransaksi,@Action,@UserID,@LogDatetime) ";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Cmd.Parameters.AddWithValue("@TransaksiID", RVID);
                Cmd.Parameters.AddWithValue("@Deskripsi", deskripsi);
                Cmd.Parameters.AddWithValue("@StatusTransaksi", statustransaksi);
                Cmd.Parameters.AddWithValue("@Action", action);
                Cmd.Parameters.AddWithValue("@UserID", ControlMgr.UserId);
                Cmd.Parameters.AddWithValue("@LogDatetime", DateTime.Now);
                Cmd.ExecuteNonQuery();
            }
        }

        private void CreateJournal(string RVNumber)
        {
            //Begin
            //Created By : Joshua
            //Created Date : 07 Sept 2018
            //Desc : Create Journal

            string PaymentMethod = cmbPaymentMethod.SelectedItem.ToString();
            decimal Nominal = Convert.ToDecimal(txtNominal.Text);
            decimal CN = Convert.ToDecimal(txtCreditAmount.Text);
            decimal Invoice = 0, DP = 0, PF = 0;

            for (int i = 0; i < dgvReceiptVoucherDtl.RowCount; i++)
            {
                decimal Payment = Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value);
                string Invoice_Id = Convert.ToString(dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Id"].Value);
                string InvoiceType = GetInvoiceType(Invoice_Id);
                if (InvoiceType.ToUpper() == "INVOICE")
                {
                    Invoice = Invoice + Payment;
                }
                if (InvoiceType.ToUpper() == "DOWN PAYMENT")
                {
                    DP = DP + Payment;
                }
                else
                {
                    PF = PF + Payment;
                }
            }

            if (Invoice != 0 || DP != 0 || PF != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "BCR01";
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Con, Cmd);
                string Notes = txtCustomer.Text + " - " + txtNameCustomer.Text;

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + RVNumber + "', 'Gunakan', 0, GETDATE(), @CreatedBy, '1900/01/01', @Notes)";
                Cmd = new SqlCommand(Query, Con);
                Cmd.Parameters.AddWithValue("@Notes",Notes);
                Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                Cmd.ExecuteNonQuery();

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                Cmd = new SqlCommand(Query, Con);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                    Type = Convert.ToString(Dr["Type"]);
                    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                    AmountValue = 0;

                    if (JournalHID == "BCR01")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            if (PaymentMethod.ToUpper() == "TRANSFER")
                            {
                                AmountValue = Nominal - CN;
                            }
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            if (PaymentMethod.ToUpper() == "CASH")
                            {
                                AmountValue = Nominal - CN;
                            }
                        }
                        else if (JournalIDSeqNo == 3)
                        {
                            if (PaymentMethod.ToUpper() == "CHEQUE")
                            {
                                AmountValue = Nominal - CN;
                            }
                        }
                        else if (JournalIDSeqNo == 4)
                        {
                            AmountValue = Invoice;
                        }
                        else if (JournalIDSeqNo == 5)
                        {
                            AmountValue = CN;
                        }
                        else if (JournalIDSeqNo == 6)
                        {
                            AmountValue = DP;
                        }
                        else if (JournalIDSeqNo == 7)
                        {
                            AmountValue = PF;
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
                    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), @CreatedBy)";
                    Cmd = new SqlCommand(Query, Con);
                    Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                Dr.Close();
            }
            else
            {
                //Insert Header GLJournal
                string JournalHID = "BCR13";
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Con, Cmd);
                string Notes = txtCustomer.Text + " - " + txtNameCustomer.Text;

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + RVNumber + "', 'Gunakan', 0, GETDATE(), @CreatedBy, '1900/01/01', @Notes)";
                Cmd = new SqlCommand(Query, Con);
                Cmd.Parameters.AddWithValue("@Notes",Notes);
                Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                Cmd.ExecuteNonQuery();

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                Cmd = new SqlCommand(Query, Con);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                    Type = Convert.ToString(Dr["Type"]);
                    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                    AmountValue = 0;

                    if (JournalHID == "BCR13")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            if (PaymentMethod.ToUpper() == "TRANSFER")
                            {
                                AmountValue = Nominal;
                            }
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            if (PaymentMethod.ToUpper() == "CASH")
                            {
                                AmountValue = Nominal;
                            }
                        }
                        else if (JournalIDSeqNo == 3)
                        {
                            if (PaymentMethod.ToUpper() == "CHEQUE")
                            {
                                AmountValue = Nominal;
                            }
                        }
                        else if (JournalIDSeqNo == 4)
                        {
                            AmountValue = Nominal;
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
                    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), @CreatedBy)";
                    Cmd = new SqlCommand(Query, Con);
                    Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                Dr.Close();
            }


            //End
        }

        private void UpdateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date : 07 Sept 2018
            //Desc : Update Journal

            string PaymentMethod = cmbPaymentMethod.SelectedItem.ToString();
            string RVNo = txtRVNo.Text;
            decimal Nominal = Convert.ToDecimal(txtNominal.Text);
            decimal CN = Convert.ToDecimal(txtCreditAmount.Text);
            decimal Invoice = 0, DP = 0, PF = 0;

            for (int i = 0; i < dgvReceiptVoucherDtl.RowCount; i++)
            {
                decimal Payment = Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value);
                string Invoice_Id = Convert.ToString(dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Id"].Value);
                string InvoiceType = GetInvoiceType(Invoice_Id);
                if (InvoiceType.ToUpper() == "INVOICE")
                {
                    Invoice = Invoice + Payment;
                }
                if (InvoiceType.ToUpper() == "DOWN PAYMENT")
                {
                    DP = DP + Payment;
                }
                else
                {
                    PF = PF + Payment;
                }
            }

            //Get GLJournalHID
            Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + RVNo + "' ";
            Cmd = new SqlCommand(Query, Con);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
            Cmd = new SqlCommand(Query, Con);
            int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

            if (CountData == 1)
            {
                //Delete Journal Detail
                Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
                Cmd = new SqlCommand(Query, Con);
                Cmd.ExecuteNonQuery();
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Journal = true;
                return;
            }

            if (Invoice != 0 || DP != 0 || PF != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "BCR01";               

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                Cmd = new SqlCommand(Query, Con);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                    Type = Convert.ToString(Dr["Type"]);
                    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                    AmountValue = 0;

                    if (JournalHID == "BCR01")
                    {
                        if (JournalIDSeqNo == 1)
                        {
                            if (PaymentMethod.ToUpper() == "TRANSFER")
                            {
                                AmountValue = Nominal;
                            }
                        }
                        else if (JournalIDSeqNo == 2)
                        {
                            if (PaymentMethod.ToUpper() == "CASH")
                            {
                                AmountValue = Nominal;
                            }
                        }
                        else if (JournalIDSeqNo == 3)
                        {
                            if (PaymentMethod.ToUpper() == "CHEQUE")
                            {
                                AmountValue = Nominal;
                            }
                        }
                        else if (JournalIDSeqNo == 4)
                        {
                            AmountValue = Invoice;
                        }
                        else if (JournalIDSeqNo == 5)
                        {
                            AmountValue = CN;
                        }
                        else if (JournalIDSeqNo == 6)
                        {
                            AmountValue = DP;
                        }
                        else if (JournalIDSeqNo == 7)
                        {
                            AmountValue = PF;
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
                    Cmd = new SqlCommand(Query, Con);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                Dr.Close();
            }

            //End
        }

        private string GetInvoiceType(string InvoiceId)
        {
            Query = "SELECT Invoice_Type FROM CustInvoice_H WHERE Invoice_Id = '" + InvoiceId + "'";
            Cmd = new SqlCommand(Query, Con);
            string result = Convert.ToString(Cmd.ExecuteScalar());

            return result;
        }

        private void InsertBankSystem(SqlConnection Con)
        {
            string RVNo = txtRVNo.Text;
            string RVDate = dtRVDate.Value.ToString();
            string RVDue = dtJatuhTempo.Value.ToString();
            string AccNo = txtNoRekCustomer.Text;
            string AccName = txtCustomer.Text;
            decimal TotalAmount = Convert.ToDecimal(txtNominal.Text);

            Query = "INSERT INTO [dbo].[BankSystem] ([VoucherNo],[VoucherDate],[VoucherDueDate],[AccountNo],[AccountName],[AccountAmount],CreatedDate,[CreatedBy]) VALUES (@VoucherNo,@VoucherDate,@VoucherDueDate,@AccountNo,@AccountName,@AccountAmount,getdate(),'"+ControlMgr.UserId+"')";
            using(Cmd = new SqlCommand(Query, Con))
            {
                Cmd.Parameters.AddWithValue("@VoucherNo",RVNo);
                Cmd.Parameters.AddWithValue("@VoucherDate", RVDate);
                Cmd.Parameters.AddWithValue("@VoucherDueDate", RVDue);
                Cmd.Parameters.AddWithValue("@AccountNo", AccNo);
                Cmd.Parameters.AddWithValue("@AccountName", AccName);
                Cmd.Parameters.AddWithValue("@AccountAmount", TotalAmount);
                Cmd.ExecuteNonQuery();
            }
        }

        private void UpdateBankSystem(SqlConnection Con)
        {
            string RVNo = txtRVNo.Text;
            string RVDate = dtRVDate.Value.ToString();
            string RVDue = dtJatuhTempo.Value.ToString();
            string AccNo = txtNoRekCustomer.Text;
            string AccName = txtCustomer.Text;
            decimal TotalAmount = Convert.ToDecimal(txtNominal.Text);

            Query = "UPDATE [dbo].[BankSystem] SET [VoucherDate]=@VoucherDate,[VoucherDueDate]=@VoucherDueDate,[AccountNo]=@AccountNo,[AccountName]=@AccountName,[AccountAmount]=@AccountAmount,[UpdatedDate] = getdate(),[UpdatedBy]='"+ControlMgr.UserId+"' WHERE [VoucherNo]=@VoucherNo";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Cmd.Parameters.AddWithValue("@VoucherNo", RVNo);
                Cmd.Parameters.AddWithValue("@VoucherDate", RVDate);
                Cmd.Parameters.AddWithValue("@VoucherDueDate", RVDue);
                Cmd.Parameters.AddWithValue("@AccountNo", AccNo);
                Cmd.Parameters.AddWithValue("@AccountName", AccName);
                Cmd.Parameters.AddWithValue("@AccountAmount", TotalAmount);
                Cmd.ExecuteNonQuery();
            }
        }

        private void CreditLimit(SqlConnection Con)
        {
            decimal PaymentAmount = Convert.ToDecimal(txtPaymentAmount.Text);
            decimal OldPaymentAmount = 0;
            string OldCustId = "";
            Query = "SELECT [Cust_Id] FROM [dbo].[ReceiptVoucher_H] WHERE RV_No = '" + txtRVNo.Text + "'";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        OldCustId = Dr["Cust_Id"] == System.DBNull.Value ? "" : Dr["Cust_Id"].ToString();
                    }
                }
                Dr.Close();
            }
            Query = "SELECT SUM(Payment_Amount) FROM  [dbo].[ReceiptVoucher_Dtl] WHERE RV_No = '" + txtRVNo.Text + "' AND [Cust_Id] = '" + OldCustId + "' ";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        OldPaymentAmount = Dr[0] == System.DBNull.Value ? 0:Convert.ToDecimal(Dr[0]);
                    }
                }
                Dr.Close();
            }
            if (OldPaymentAmount != 0)
            {
                Query = "UPDATE [dbo].[CustTable] SET [Sisa_Limit_Total] = Sisa_Limit_Total +" + OldPaymentAmount + " WHERE [CustId] = '" + OldCustId + "' ";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.ExecuteNonQuery();
                }
            }
            Query = "UPDATE [dbo].[CustTable] SET [Sisa_Limit_Total] = Sisa_Limit_Total -" + PaymentAmount + " WHERE [CustId] = '" + txtCustomer.Text + "' ";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private void InsertReceiptVoucherH(SqlConnection Con)
        {
            Query = "INSERT [dbo].[ReceiptVoucher_H] ([RV_No],[RV_Date],[Cust_Id],[Cust_Name],[Payment_Method],[Bank_Id],[Bank_Name],[Giro_No],[Receipt_No],[Rek_Cust],[Payment_DueDate],[Total_Payment],[Signed_Amount],[Notes],[CreatedDate],[CreatedBy],[StatusCode]) VALUES (@RV_No,@RV_Date,@Cust_Id,@Cust_Name,@Payment_Method,@Bank_Id,@Bank_Name,@Giro_No,@Receipt_No,@Rek_Cust,@Payment_DueDate,@Total_Payment,@Signed_Amount,@Notes,getdate(),'" + ControlMgr.UserId + "','01')";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Cmd.Parameters.AddWithValue("@RV_No", txtRVNo.Text);
                Cmd.Parameters.AddWithValue("@RV_Date", dtRVDate.Value);
                Cmd.Parameters.AddWithValue("@Cust_Id", txtCustomer.Text);
                Cmd.Parameters.AddWithValue("@Cust_Name", txtNameCustomer.Text);
                Cmd.Parameters.AddWithValue("@Payment_Method", cmbPaymentMethod.Text);
                Cmd.Parameters.AddWithValue("@Bank_Id", txtKodeBank.Text);
                Cmd.Parameters.AddWithValue("@Bank_Name", txtBankName.Text);
                Cmd.Parameters.AddWithValue("@Receipt_No", txtKetTransfer.Text.Trim());
                Cmd.Parameters.AddWithValue("@Giro_No", txtNoGiro.Text.Trim()); ;
                Cmd.Parameters.AddWithValue("@Rek_Cust", txtNoRekCustomer.Text);
                Cmd.Parameters.AddWithValue("@Payment_DueDate", dtJatuhTempo.Value);
                Cmd.Parameters.AddWithValue("@Total_Payment", Convert.ToDecimal(txtNominal.Text));
                Cmd.Parameters.AddWithValue("@Signed_Amount", Convert.ToDecimal(txtPaymentAmount.Text));
                Cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                Cmd.ExecuteNonQuery();
            }
        }

        private void InsertReceiptVoucherDtl(SqlConnection Con)
        {
            int seqnoforCN = 0;
            for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
            {
                Query = "INSERT [dbo].[ReceiptVoucher_Dtl] ([RV_No],[SeqNo],[Cust_Id],[Cust_Name],[Invoice_Date],[Invoice_Id],[TermOfPayment],[DueDate],[Invoice_Amount],[CN_Amount],[AR_Amount],[Payment_Amount],[CreatedDate],[CreatedBy],Jenis) VALUES(@RV_No,@SeqNo,@Cust_Id,@Cust_Name,@Invoice_Date,@Invoice_Id,@TermOfPayment,@DueDate,@Invoice_Amount,@CN_Amount,@AR_Amount,@Payment_Amount,getdate(),'" + ControlMgr.UserId + "',@Jenis)";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.Parameters.AddWithValue("@RV_No", txtRVNo.Text);
                    Cmd.Parameters.AddWithValue("@SeqNo", dgvReceiptVoucherDtl.Rows[i].Cells["SeqNo"].Value);
                    Cmd.Parameters.AddWithValue("@Cust_Id", dgvReceiptVoucherDtl.Rows[i].Cells["Cust_Id"].Value);
                    Cmd.Parameters.AddWithValue("@Cust_Name", dgvReceiptVoucherDtl.Rows[i].Cells["Cust_Name"].Value);
                    DateTime InvDate = DateTime.ParseExact(dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Date"].Value.ToString(), "dd/MM/yyyy", null);
                    Cmd.Parameters.AddWithValue("@Invoice_Date", InvDate);
                    Cmd.Parameters.AddWithValue("@Invoice_Id", dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Id"].Value);
                    Cmd.Parameters.AddWithValue("@TermOfPayment", dgvReceiptVoucherDtl.Rows[i].Cells["TermOfPayment"].Value);
                    DateTime InvDue = DateTime.ParseExact(dgvReceiptVoucherDtl.Rows[i].Cells["DueDate"].Value.ToString(), "dd/MM/yyyy", null);
                    Cmd.Parameters.AddWithValue("@DueDate", InvDue);
                    Cmd.Parameters.AddWithValue("@Invoice_Amount", Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Amount"].Value));
                    Cmd.Parameters.AddWithValue("@CN_Amount", Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["CN_Amount"].Value));
                    Cmd.Parameters.AddWithValue("@AR_Amount", Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["AR_Amount"].Value));
                    Cmd.Parameters.AddWithValue("@Payment_Amount", Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value));
                    Cmd.Parameters.AddWithValue("@Jenis","INVOICE");
                    Cmd.ExecuteNonQuery();
                }
                if (Convert.ToInt32(dgvReceiptVoucherDtl.Rows[i].Cells["SeqNo"].Value) > seqnoforCN)
                {
                    seqnoforCN = Convert.ToInt32(dgvReceiptVoucherDtl.Rows[i].Cells["SeqNo"].Value);
                }
            }

            for (int i = 0; i < dgvCreditNote.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvCreditNote.Rows[i].Cells["Check"].Value) != true)
                {
                    continue;
                }
                seqnoforCN += 1;
                Query = "INSERT [dbo].[ReceiptVoucher_Dtl] ([RV_No],[SeqNo],[Cust_Id],[Cust_Name],[Invoice_Date],[Invoice_Id],[DueDate],[Invoice_Amount],[CN_Amount],[AR_Amount],[Payment_Amount],[CreatedDate],[CreatedBy],[Jenis]) VALUES(@RV_No,@SeqNo,@Cust_Id,@Cust_Name,@Invoice_Date,@Invoice_Id,@DueDate,@Invoice_Amount,@CN_Amount,@AR_Amount,@Payment_Amount,getdate(),'" + ControlMgr.UserId + "',@Jenis)";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.Parameters.AddWithValue("@RV_No", txtRVNo.Text);
                    Cmd.Parameters.AddWithValue("@SeqNo", seqnoforCN);
                    Cmd.Parameters.AddWithValue("@Cust_Id", txtCustomer.Text);
                    Cmd.Parameters.AddWithValue("@Cust_Name", txtNameCustomer.Text);
                    DateTime InvDate = DateTime.ParseExact(dgvCreditNote.Rows[i].Cells["CN_Date"].Value.ToString(), "dd/MM/yyyy", null);
                    Cmd.Parameters.AddWithValue("@Invoice_Date", InvDate);
                    Cmd.Parameters.AddWithValue("@Invoice_Id", dgvCreditNote.Rows[i].Cells["CN_No"].Value);
                    if (dgvCreditNote.Rows[i].Cells["DueDate"].Value != null && dgvCreditNote.Rows[i].Cells["DueDate"].Value != String.Empty)
                    {
                        DateTime InvDue = DateTime.ParseExact(dgvCreditNote.Rows[i].Cells["DueDate"].Value.ToString(), "dd/MM/yyyy", null);
                        Cmd.Parameters.AddWithValue("@DueDate", InvDue);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@DueDate", "");
                    }
                    Cmd.Parameters.AddWithValue("@Invoice_Amount", 0);
                    Cmd.Parameters.AddWithValue("@CN_Amount", 0);
                    Cmd.Parameters.AddWithValue("@AR_Amount", 0);
                    Cmd.Parameters.AddWithValue("@Payment_Amount", Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Amount"].Value));
                    Cmd.Parameters.AddWithValue("@Jenis","NOTA CREDIT");
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateReceiptVoucherH(SqlConnection Con)
        {
            Query = "UPDATE [dbo].[ReceiptVoucher_H] SET [RV_Date]=@RV_Date,[Cust_Id]=@Cust_Id,[Cust_Name]=@Cust_Name,[Payment_Method]=@Payment_Method,[Bank_Id]=@Bank_Id,[Bank_Name]=@Bank_Name,[Giro_No]=@Giro_No,[Receipt_No]=@Receipt_No,[Rek_Cust]=@Rek_Cust,[Payment_DueDate]=@Payment_DueDate,[Total_Payment]=@Total_Payment,[Signed_Amount]=@Signed_Amount,[Notes]=@Notes,UpdatedDate = getdate(),[UpdatedBy]='" + ControlMgr.UserId + "' WHERE [RV_No]= @RV_No ";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Cmd.Parameters.AddWithValue("@RV_No", txtRVNo.Text);
                Cmd.Parameters.AddWithValue("@RV_Date", Convert.ToDateTime(dtRVDate.Value));
                Cmd.Parameters.AddWithValue("@Cust_Id", txtCustomer.Text);
                Cmd.Parameters.AddWithValue("@Cust_Name", txtNameCustomer.Text);
                Cmd.Parameters.AddWithValue("@Payment_Method", cmbPaymentMethod.Text);
                Cmd.Parameters.AddWithValue("@Bank_Id", txtKodeBank.Text);
                Cmd.Parameters.AddWithValue("@Bank_Name", txtBankName.Text);
                Cmd.Parameters.AddWithValue("@Receipt_No", txtKetTransfer.Text.Trim());
                Cmd.Parameters.AddWithValue("@Giro_No", txtNoGiro.Text.Trim()); ;
                Cmd.Parameters.AddWithValue("@Rek_Cust", txtNoRekCustomer.Text);
                Cmd.Parameters.AddWithValue("@Payment_DueDate", Convert.ToDateTime(dtJatuhTempo.Value));
                Cmd.Parameters.AddWithValue("@Total_Payment", Convert.ToDecimal(txtNominal.Text));
                Cmd.Parameters.AddWithValue("@Signed_Amount", Convert.ToDecimal(txtPaymentAmount.Text));
                Cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                Cmd.ExecuteNonQuery();
            }

            if (cmbChequeResult.Text.ToUpper().Trim() == "CAIR")
            {
                Query = "UPDATE [dbo].[ReceiptVoucher_H] SET [Tgl_Cair]=@Tgl_Cair,[Tgl_Pending]=NULL,[Tgl_Tolak]=NULL WHERE [RV_No]= @RV_No ";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.Parameters.AddWithValue("@RV_No",txtRVNo.Text);
                    Cmd.Parameters.AddWithValue("@Tgl_Cair", dtTglCheque.Value);
                    Cmd.ExecuteNonQuery();
                }
            }
            else if (cmbChequeResult.Text.ToUpper().Trim() == "TUNDA")
            {
                Query = "UPDATE [dbo].[ReceiptVoucher_H] SET [Tgl_Cair]=NULL,[Tgl_Pending]=@Tgl_Pending,[Tgl_Tolak]=NULL WHERE [RV_No]= @RV_No ";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.Parameters.AddWithValue("@RV_No", txtRVNo.Text);
                    Cmd.Parameters.AddWithValue("@Tgl_Pending", dtTglCheque.Value);
                    Cmd.ExecuteNonQuery();
                }
            }
            else if (cmbChequeResult.Text.ToUpper().Trim() == "TOLAK")
            {
                Query = "UPDATE [dbo].[ReceiptVoucher_H] SET [Tgl_Cair]=NULL,[Tgl_Pending]=NULL,[Tgl_Tolak]=@Tgl_Tolak WHERE [RV_No]= @RV_No ";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.Parameters.AddWithValue("@RV_No", txtRVNo.Text);
                    Cmd.Parameters.AddWithValue("@Tgl_Tolak", dtTglCheque.Value);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertAttachment(SqlConnection Con)
        {
            for (int i = 0; i < dgvFileAttached.Rows.Count; i++)
            {
                Query = "INSERT INTO [dbo].[tblAttachments] ([ReffTableName],[ReffTransID],[fileName],[ContentType],[fileSize],[attachment]) VALUES (@ReffTableName,@ReffTransID,@fileName,@ContentType,@fileSize,@attachment)";
                using (Cmd = new SqlCommand(Query, Con))
                {
                    Cmd.Parameters.AddWithValue("@ReffTableName", "ReceiptVoucherH");
                    Cmd.Parameters.AddWithValue("@ReffTransID", txtRVNo.Text);
                    Cmd.Parameters.AddWithValue("@fileName", dgvFileAttached.Rows[i].Cells["FileName"].Value);
                    Cmd.Parameters.AddWithValue("@ContentType", dgvFileAttached.Rows[i].Cells["ContentType"].Value);
                    Cmd.Parameters.AddWithValue("@fileSize", dgvFileAttached.Rows[i].Cells["FileSize"].Value);
                    Cmd.Parameters.Add("@attachment", SqlDbType.VarBinary, attachment[i].Length).Value = attachment[i];
                    Cmd.ExecuteNonQuery();
                }
            }
        }
        
        private void btnAddDgv_Click(object sender, EventArgs e)
        {
            if (txtCustomer.Text == "" || cmbPaymentMethod.Text == "")
            {
                MessageBox.Show("Pilih Customer terlebih dahulu.");
                return;
            }
            Variable.Kode2 = null;
            //search invoice
            string Table = "[dbo].[CustInvoice_H]";
            string QuerySearch = "SELECT a.* FROM [dbo].[CustInvoice_H] a WHERE [Cust_Id] ='" + txtCustomer.Text + "' AND ([AR_Amount]-[Settle_Amount] > 0) AND [TransStatus] NOT IN ('01','02','05','07','08','11','17','30','09') ";
            for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
            {
                QuerySearch += " AND NOT a.[Invoice_Id] = '" + dgvReceiptVoucherDtl.Rows[i].Cells["Invoice_Id"].Value.ToString() + "'";
            }
            string[] FilterText = {"Cust_Id","Cust_Name","Invoice_Date","Invoice_Id","Invoice_DueDate","Invoice_Amount","CN_Amount","AR_Amount","Payment_Method" };
            string[] Select = { "Cust_Id", "Cust_Name", "Invoice_Date","Invoice_Id", "Invoice_DueDate", "Invoice_Amount", "CN_Amount", "AR_Amount", "Payment_Method" };
            string PrimaryKey = "Invoice_Id";
            string[] HideField = { "Invoice_Type", "PPN_Percent", "DP_Amount", "Invoice_Round_Amount","Invoice_Tax_Base_Amount","Invoice_Tax_Amount","NPWP","TaxDate","TaxNum","TaxName","TaxAddress","Notes","Settle_Amount","Settle_Invoice_DP","TransStatus","ApprovedBy","CreatedDate","CreatedBy","UpdatedDate","UpdatedBy","CurrencyID","ExchRate","Kwitansi_No","Proforma_Id" };

            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = Table;
            F.QuerySearch = QuerySearch;
            F.FilterText = FilterText;
            F.Select = Select;
            F.PrimaryKey = PrimaryKey;
            F.HideField = HideField;
            F.Parent = this;
            F.ShowDialog();

            if (dgvReceiptVoucherDtl.Rows.Count < 1)
            {
                //Established column header
                dgvReceiptVoucherDtl.ColumnCount = TableColName.Count();
                for (int i = 0; i < TableColName.Count(); i++)
                {
                    dgvReceiptVoucherDtl.Columns[i].Name = TableColName[i];
                    dgvReceiptVoucherDtl.Columns[i].HeaderText = TableColHeaderText[i];
                    for (int j = 0; j < TableColVisibleTrue.Count(); j++)
                    {
                        if (dgvReceiptVoucherDtl.Columns[i].HeaderText == TableColVisibleTrue[j])
                        {
                            dgvReceiptVoucherDtl.Columns[i].Visible = true;
                            break;
                        }
                        else
                        {
                            dgvReceiptVoucherDtl.Columns[i].Visible = false;
                        }
                    }
                    for (int j = 0; j < TableColReadOnlyFalse.Count(); j++)
                    {
                        if (dgvReceiptVoucherDtl.Columns[i].HeaderText == TableColReadOnlyFalse[j])
                        {
                            dgvReceiptVoucherDtl.Columns[i].ReadOnly = false;
                            dgvReceiptVoucherDtl.Columns[i].DefaultCellStyle.BackColor = Color.White;
                            break;
                        }
                        else
                        {
                            dgvReceiptVoucherDtl.Columns[i].ReadOnly = true;
                            dgvReceiptVoucherDtl.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                    }
                }
            }
            if (Variable.Kode2 != null)
            {
                int seqno = 1;
                if (txtRVNo.Text != "")
                {
                    Query = "SELECT MAX(SeqNo) FROM [dbo].[ReceiptVoucher_Dtl] WHERE [RV_No] = '" + txtRVNo.Text + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                seqno = Convert.ToInt32(Dr[0])+1;
                            }
                        }
                        Dr.Close();
                    }
                }
                //populate datagridview
                for (int i = 0; i <= Variable.Kode2.GetUpperBound(0); i++)
                {
                    for(int x = 0; x< dgvReceiptVoucherDtl.Rows.Count;x++)
                    {
                        if (Convert.ToInt32(dgvReceiptVoucherDtl.Rows[x].Cells["SeqNo"].Value) >= seqno)
                        {
                            seqno = Convert.ToInt32(dgvReceiptVoucherDtl.Rows[x].Cells["SeqNo"].Value) + 1;
                        }
                    }
                    // { 0="Cust_Id", 1="Cust_Name", 2="Invoice_Date", 3="Invoice_Id", 4="Invoice_DueDate", 5="Invoice_Amount", 6="CN_Amount", 7="AR_Amount", 8="Payment_Method" };
                    dgvReceiptVoucherDtl.Rows.Add(dgvReceiptVoucherDtl.Rows.Count + 1, "", seqno, Variable.Kode2[i, 0], Variable.Kode2[i, 1], Convert.ToDateTime(Variable.Kode2[i, 2]).ToString("dd/MM/yyyy"), Variable.Kode2[i, 3], "", Convert.ToDateTime(Variable.Kode2[i, 4]).ToString("dd/MM/yyyy"), String.Format("{0:#,##0.#0}", Convert.ToDecimal(Variable.Kode2[i, 5])), String.Format("{0:#,##0.#0}", Convert.ToDecimal(Variable.Kode2[i, 6])), String.Format("{0:#,##0.#0}", Convert.ToDecimal(Variable.Kode2[i, 7])), "0.00", DateTime.Now, ControlMgr.UserId, "", "");
                }
            }
            CalculateRemainingPayment();
        }

        private void txtTotalPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' )|| txtNominal.Text.Length > 27)
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtTotalPayment_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtNominal.Text.IndexOf('.') == 0)
            {
                if (txtNominal.Text.Length > 1)
                {
                    txtNominal.Text = "0" + txtNominal.Text;
                }
                else
                {
                    txtNominal.Text = "0" + txtNominal.Text+"0000";
                }
            }
        }

        private void txtTotalPayment_Leave(object sender, EventArgs e)
        {
            if (txtNominal.Text != "")
            {
                txtNominal.Text = String.Format("{0:#,##0.#0}", Convert.ToDecimal(txtNominal.Text));
            }
            else
            {
                txtNominal.Text = "0.00";
            }
        }

        private void txtCustomer_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void btnDeleteDgv_Click(object sender, EventArgs e)
        {
            if (dgvReceiptVoucherDtl.Rows.Count > 0)
            {
                dgvReceiptVoucherDtl.Rows.RemoveAt(dgvReceiptVoucherDtl.CurrentRow.Index);
            }
            CalculateRemainingPayment();
        }

        private void txtNominal_TextChanged(object sender, EventArgs e)
        {
            txtNominal.BackColor = Color.White;
            for (int i = 0; i < dgvReceiptVoucherDtl.Rows.Count; i++)
            {
                dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value = 0.00;
            }
            CalculateRemainingPayment();
        }

        private void dgvReceiptVoucherDtl_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        private void dgvReceiptVoucherDtl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvReceiptVoucherDtl.Rows.Count > 0)
            {
                if (dgvReceiptVoucherDtl.Columns[dgvReceiptVoucherDtl.CurrentCell.ColumnIndex].Name == "Payment_Amount")
                {
                    if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.') || txtNominal.Text.Length > 27)
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

        private void dgvReceiptVoucherDtl_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
            tb.KeyPress += new KeyPressEventHandler(dgvReceiptVoucherDtl_KeyPress);

            e.Control.KeyPress += new KeyPressEventHandler(dgvReceiptVoucherDtl_KeyPress);
        }

        private void dgvReceiptVoucherDtl_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int i = dgvReceiptVoucherDtl.CurrentCell.RowIndex;
            if (dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value == null || dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value.ToString() == "")
            {
                dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value = "0.00";
            }
            int Length = dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value.ToString().Length;
            if ((dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value).ToString() == ".")
            {
                dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value = 0.00;
            }
            else
            {
                dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value.ToString()));
            }
            if (Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value) > Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[i].Cells["AR_Amount"].Value))
            {
                dgvReceiptVoucherDtl.Rows[i].Cells["Payment_Amount"].Value = 0.00;
                MessageBox.Show("Payment Amount tidak boleh melebihi AR Amount.");
            }
            else
            {
                CalculateRemainingPayment();
                if (txtNominal.Text == "")
                {
                    txtNominal.Text = "0.00";
                }
                decimal PaymentAmountTotal = (Convert.ToDecimal(txtPaymentAmount.Text));
                decimal CreditAmountTotal = Convert.ToDecimal(txtCreditAmount.Text);
                if ((dgvReceiptVoucherDtl.Rows.Count > 0) && (PaymentAmountTotal > (Convert.ToDecimal(txtNominal.Text) + CreditAmountTotal)))
                {
                    PaymentAmountTotal -= Convert.ToDecimal(dgvReceiptVoucherDtl.Rows[dgvReceiptVoucherDtl.CurrentRow.Index].Cells["Payment_Amount"].Value);
                    dgvReceiptVoucherDtl.Rows[dgvReceiptVoucherDtl.CurrentRow.Index].Cells["Payment_Amount"].Value = 0.00;
                    metroTabControl1.SelectedIndex = 0;
                    txtNominal.BackColor = Color.LightYellow;
                    txtPaymentAmount.Text = String.Format("{0:#,##0.#0}", Convert.ToDecimal(PaymentAmountTotal));
                    MessageBox.Show("Total Payment Tidak boleh lebih besar dari Amount (Nominal+CreditNote).");
                }
            }
        }
        //tia edit
        //klik kanan
        PopUp.CustomerID.Customer Cust = null;
        AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice CINumber = null;
        Sales.NotaReturJual.NRJHeader NrjId = null;
        TaskList.GlobalTasklist Parent2 = new TaskList.GlobalTasklist();

        public void SetParent2(TaskList.GlobalTasklist Tl)
        {
            Parent2 = Tl;
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

        private void txtCustomer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    txtCustomer.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(txtCustomer.Text);
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

        private void txtNameCustomer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    txtNameCustomer.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(txtCustomer.Text);
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

        private void dgvReceiptVoucherDtl_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (CINumber == null || CINumber.Text == "")
                {
                    if (dgvReceiptVoucherDtl.Columns[e.ColumnIndex].Name.ToString() == "Invoice_Id")
                    {
                        CINumber = new AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice();
                        CINumber.SetMode("PopUp", dgvReceiptVoucherDtl.Rows[e.RowIndex].Cells["Invoice_Id"].Value.ToString());
                        CINumber.ParentRefreshGrid2(this);
                        CINumber.Show();
                    }
                }
                else if (CheckOpened(CINumber.Name))
                {
                    CINumber.WindowState = FormWindowState.Normal;
                    CINumber.SetMode("PopUp", dgvReceiptVoucherDtl.Rows[e.RowIndex].Cells["Invoice_Id"].Value.ToString());
                    CINumber.ParentRefreshGrid2(this);
                    CINumber.Show();
                    CINumber.Focus();
                }
            }
        }

        private void dgvCreditNote_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (NrjId == null || NrjId.Text == "")
                {
                    if (dgvCreditNote.Columns[e.ColumnIndex].Name.ToString() == "NRJId")
                    {
                        NrjId = new Sales.NotaReturJual.NRJHeader();
                        NrjId.SetMode("PopUp", dgvCreditNote.Rows[e.RowIndex].Cells["NRJId"].Value.ToString());
                        NrjId.ParentRefreshGrid2(this);
                        NrjId.Show();
                    }
                }
                else if (CheckOpened(NrjId.Name))
                {
                    NrjId.WindowState = FormWindowState.Normal;
                    NrjId.SetMode("PopUp", dgvCreditNote.Rows[e.RowIndex].Cells["NRJId"].Value.ToString());
                    NrjId.Show();
                    NrjId.Focus();
                }
            }
        }

        //tia edit end

        private void CalculateCreditAmount()
        {
            decimal amount = 0;
            for (int i = 0; i < dgvCreditNote.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvCreditNote.Rows[i].Cells["Check"].Value) == true)
                {
                    amount += Convert.ToDecimal(dgvCreditNote.Rows[i].Cells["Amount"].Value);
                }
            }
            txtCreditAmount.Text = String.Format("{0:#,##0.#0}", amount);
            txtRemainingAmount.Text = String.Format("{0:#,##0.#0}", (Convert.ToDecimal(txtNominal.Text) - Convert.ToDecimal(txtPaymentAmount.Text) + Convert.ToDecimal(txtCreditAmount.Text)));
        }

        private void dgvCreditNote_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CalculateCreditAmount();
            int rowindex = dgvCreditNote.CurrentCell.RowIndex;
            
            if (dgvCreditNote.Rows[rowindex].Cells["Amount"].Value == null)
            {
                dgvCreditNote.Rows[rowindex].Cells["Amount"].Value = String.Format("{0:#,##0.#0}", 0);
            }
            else if (dgvCreditNote.Rows[rowindex].Cells["Amount"].Value.ToString() == "")
            {
                dgvCreditNote.Rows[rowindex].Cells["Amount"].Value = String.Format("{0:#,##0.#0}", 0);
            }
            else
            {
                dgvCreditNote.Rows[rowindex].Cells["Amount"].Value = String.Format("{0:#,##0.#0}", Convert.ToDecimal(dgvCreditNote.Rows[rowindex].Cells["Amount"].Value));
            }

            if (Convert.ToBoolean(dgvCreditNote.Rows[rowindex].Cells["Check"].Value) == true)
            {
                if (Convert.ToDecimal(dgvCreditNote.Rows[rowindex].Cells["Amount"].Value) > Convert.ToDecimal(dgvCreditNote.Rows[rowindex].Cells["Remaining Amount"].Value))
                {
                    dgvCreditNote.Rows[rowindex].Cells["Amount"].Value = String.Format("{0:#,##0.#0}", 0);
                    MessageBox.Show("Amount CN tidak boleh lebih besar dibanding remaining amount.");
                }
            }
        }

        private void dgvCreditNote_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
            tb.KeyPress += new KeyPressEventHandler(dgvCreditNote_KeyPress);

            e.Control.KeyPress += new KeyPressEventHandler(dgvCreditNote_KeyPress);
        }

        private void dgvCreditNote_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvCreditNote.Rows.Count > 0)
            {
                if (dgvCreditNote.Columns[dgvCreditNote.CurrentCell.ColumnIndex].Name == "Amount")
                {
                    if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.') || txtNominal.Text.Length > 27)
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

        private void cmbChequeResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChequeResult.Text.ToUpper().Trim() == "CAIR")
            {
                lblTglCair.Visible = true;
                dtTglCheque.Visible = true;
            }
            else if (cmbChequeResult.Text.ToUpper().Trim() == "TUNDA")
            {
                lblTglCair.Text = "Tgl Tunda";
                lblTglCair.Visible = true;
                dtTglCheque.Visible = true;
            }
            else if (cmbChequeResult.Text.ToUpper().Trim() == "TOLAK")
            {
                lblTglCair.Text = "Tgl Tolak";
                lblTglCair.Visible = true;
                dtTglCheque.Visible = true;
            }
            else
            {
                lblTglCair.Visible = false;
                dtTglCheque.Visible = false;
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            Query = "UPDATE [dbo].[ReceiptVoucher_H] SET [StatusCode]='02' WHERE [RV_No]=@RV_No";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@RV_No",txtRVNo.Text);
                Cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Berhasil di approve.");
        }

      
        
    }
}
