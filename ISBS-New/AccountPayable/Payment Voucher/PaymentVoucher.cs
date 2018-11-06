using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Transactions;
using System.Data.SqlClient;

namespace ISBS_New.AccountPayable.Payment_Voucher
{
    public partial class PaymentVoucher : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        InquiryV1 Parent = new InquiryV1();
        InquiryV2 Parent1 = new InquiryV2();
        DataGridViewCheckBoxCell TmpCheckBox ;
        //GlobalInquiry Parent1 = new GlobalInquiry();

        //MODE DIGUNAKAN UNTUK NEW, EDIT, DELETE
        string Mode = "";
        string Query = "";

        int Index = -1;
        //MODE DIGUNAKAN UNTUK NEW, EDIT, DELETE
        //tia edit
         ContextMenu CM = new ContextMenu();
        //tia edit end

        //Attachment
        List<string> sSelectedFile, FileName, Extension; //untuk attachement
        List<byte[]> test = new List<byte[]>();
        DataGridViewComboBoxCell cell;
        //Attachment

        #region Function

        //public void SetParent(GlobalInquiry F)
        //{ 
        //    Parent1 = F; 
        //}

        //public void SetMode(string tmpMode, string tmpNumber)
        //{
        //    Mode = tmpMode;
        //    txtPVNo.Text = tmpNumber;
        //}

        public void AddDN()
        {
            int OldDNRowCount = dgvDN.RowCount;
            List<bool> OldCheckStatus = new List<bool>(); 

            dgvDN.ColumnCount = 9;
            dgvDN.Columns[0].Name = "Check";
            dgvDN.Columns[1].Name = "No";
            dgvDN.Columns[2].Name = "DN_No";
            dgvDN.Columns[3].Name = "DN_Date";
            dgvDN.Columns[4].Name = "Account_Num";
            dgvDN.Columns[5].Name = "Account_Name";
            dgvDN.Columns[6].Name = "Currency";
            dgvDN.Columns[7].Name = "ExchRate";
            dgvDN.Columns[8].Name = "TotalAmount";

            if (dgvDN.RowCount > 0)
            {
                for (int i = 0; i < dgvDN.RowCount; i++)
                {
                    OldCheckStatus.Add(Convert.ToBoolean(dgvDN.Rows[i].Cells["Check"].Value));
                }
                dgvDN.Rows.Clear();
            }

            string ListPO = "";
            for (int i = 0; i < dgvDN.RowCount; i++)
            {
                ListPO += "'" + dgvDN.Rows[i].Cells["PONo"].Value.ToString() + "'";
                if (i != dgvDN.RowCount - 1)
                {
                    ListPO += ",";
                }
            }

            using (Conn = ConnectionString.GetConnection())
            {
                Query = "SELECT [DN_No],[DN_Date],[AccountNum],[AccountName],[CurrencyId],[ExchRate],[TotalAmount] FROM [dbo].[NotaDebetH] where AccountNum='" + txtVendorId.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                int i = 1;
                while (Dr.Read())
                {
                    TmpCheckBox = new DataGridViewCheckBoxCell();
                    dgvDN.Rows.Add("", i, Dr["DN_No"], Dr["DN_Date"], Dr["AccountNum"], Dr["AccountName"], Dr["CurrencyId"], Dr["ExchRate"], Dr["TotalAmount"]);
                    dgvDN.Rows[(dgvDN.Rows.Count - 1)].Cells["Check"] = TmpCheckBox;
                    if (OldDNRowCount == dgvDN.RowCount)
                    {
                        dgvDN.Rows[(dgvDN.Rows.Count - 1)].Cells["Check"].Value = OldCheckStatus[(dgvDN.Rows.Count - 1)];
                    }
                    else
                    {
                        dgvDN.Rows[(dgvDN.Rows.Count - 1)].Cells["Check"].Value = false;
                    }
                    i++;
                }
                Dr.Close();
                dgvDN.AutoResizeColumns();
                CheckedDN();
            }

            dgvReadOnlyFalse();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void dgvReadOnlyFalse()
        {
            //dgvInvoice
            dgvInvoice.ReadOnly = false;
            dgvInvoice.Columns["No"].ReadOnly = true;
            dgvInvoice.Columns["Invoice No"].ReadOnly = true;
            dgvInvoice.Columns["Invoice Type"].ReadOnly = true;
            dgvInvoice.Columns["Invoice Date"].ReadOnly = true;
            dgvInvoice.Columns["Invoice Amount"].ReadOnly = true;
            dgvInvoice.Columns["Due Date"].ReadOnly = true;
            dgvInvoice.Columns["Paid"].ReadOnly = true;
            dgvInvoice.Columns["Outstanding"].ReadOnly = true;
            //dgvInvoice.Columns["Payment"].ReadOnly = false;

            //dgvAttachment
            dgvAttachment.ReadOnly = false;
            dgvAttachment.Columns["FileName"].ReadOnly = true;
            dgvAttachment.Columns["ContentType"].ReadOnly = true;
            dgvAttachment.Columns["File Size (kb)"].ReadOnly = true;
            dgvAttachment.Columns["Attachment"].ReadOnly = true;
            //dgvAttachment.Columns["FileType"].ReadOnly = true;

            dgvDN.ReadOnly = false;
            dgvDN.Columns["No"].ReadOnly = true;
            dgvDN.Columns["DN_No"].ReadOnly = true;
            dgvDN.Columns["DN_Date"].ReadOnly = true;
            dgvDN.Columns["Account_Num"].ReadOnly = true;
            dgvDN.Columns["Account_Name"].ReadOnly = true;
            dgvDN.Columns["Currency"].ReadOnly = true;
            dgvDN.Columns["ExchRate"].ReadOnly = true;
            dgvDN.Columns["TotalAmount"].ReadOnly = true;
        }

        private void cmbPaymentMethod_Load()
        {
            Query = "SELECT PaymentModeID, PaymentModeName FROM [dbo].[PaymentMode]";
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbPaymentMethod.Items.Add(Dr["PaymentModeID"].ToString());
            }
            Dr.Close();
        }

        private void ModeNew()
        {
            dtPVDate.Enabled = true;
            dtJatuhTempo.Enabled = true;
            dtCair.Enabled = false;
            dtTolak.Enabled = true;
            dtPending.Enabled = true;
            cmbPaymentMethod.Enabled = true;
            txtNoGiro.ReadOnly = false;
            txtNominal.ReadOnly = false;
            txtNotes.ReadOnly = false;

            dtCair.Enabled = false;
            dtPending.Enabled = false;
            dtTolak.Enabled = false;
            dgvInvoice.ReadOnly = false;
            dgvDN.ReadOnly = false;
            dgvAttachment.ReadOnly = false;

            btnVendorId.Enabled = true;
            btnBankId.Enabled = true;
            btnRekVendor.Enabled = true;
            btnDeleteFile.Enabled = true;
            btnDownloadFile.Enabled = true;
            btnAddNewFile.Enabled = true;
            btnNewInvoice.Enabled = true;
            btnDeleteInvoice.Enabled = true;

            btnApprove.Enabled = false;
            btnRevisi.Enabled = false;
            btnReject.Enabled = false;
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnSave.Enabled = true;
            btnExit.Enabled = true;

            dtCair.CustomFormat = " ";
            dtJatuhTempo.CustomFormat = " ";
            dtPending.CustomFormat = " ";
            dtPVDate.CustomFormat = " ";
            dtTolak.CustomFormat = " ";

            dgvReadOnlyFalse();
        }

        private void ModeEdit()
        {
            dtPVDate.Enabled = true;
            dtJatuhTempo.Enabled = true;
            dtCair.Enabled = false;
            dtTolak.Enabled = false;
            dtPending.Enabled = false;
            if (cmbPaymentMethod.Text.ToUpper() == "CHEQUE")
            {
                dtCair.Enabled = true;
            }
            txtNoGiro.ReadOnly = false;
            txtNominal.ReadOnly = false;
            txtNotes.ReadOnly = false;

            dgvInvoice.ReadOnly = false;
            dgvDN.ReadOnly = false;
            dgvAttachment.ReadOnly = false;

            btnVendorId.Enabled = true;
            btnBankId.Enabled = true;
            btnRekVendor.Enabled = true;
            btnDeleteFile.Enabled = true;
            btnDownloadFile.Enabled = true;
            btnAddNewFile.Enabled = true;
            btnNewInvoice.Enabled = true;
            btnDeleteInvoice.Enabled = true;

            btnApprove.Enabled = false;
            btnRevisi.Enabled = false;
            btnReject.Enabled = false;
            btnEdit.Enabled = false;
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
            btnExit.Enabled = false ;

            dgvReadOnlyFalse();
        }

        private void ModeView()
        {
            Mode = "View";
            dtPVDate.Enabled = false;
            dtJatuhTempo.Enabled = false;
            dtCair.Enabled = false;
            dtTolak.Enabled = false;
            dtPending.Enabled = false;
            cmbPaymentMethod.Enabled = false;
            txtNoGiro.ReadOnly = true;
            txtNominal.ReadOnly = true;
            txtNotes.ReadOnly = true;
            txtAlasanPending.ReadOnly = true;
            txtAlasanTolak.ReadOnly = true;

            dgvInvoice.ReadOnly = true;
            dgvDN.ReadOnly = true;
            dgvAttachment.ReadOnly = true;

            btnVendorId.Enabled = false;
            btnBankId.Enabled = false;
            btnRekVendor.Enabled = false;
            btnDeleteFile.Enabled = false;
            btnDownloadFile.Enabled = false;
            btnAddNewFile.Enabled = false;
            btnNewInvoice.Enabled = false;
            btnDeleteInvoice.Enabled = false;

            btnApprove.Enabled = false;
            btnRevisi.Enabled = false;
            btnReject.Enabled = false;
            btnEdit.Enabled = true;
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnExit.Enabled = true;

            if(dtCair.Value.Year <= 1990)
                dtCair.CustomFormat = " ";
            if (dtJatuhTempo.Value.Year <= 1990)
                dtJatuhTempo.CustomFormat = " ";
            if (dtPending.Value.Year <= 1990)
                dtPending.CustomFormat = " ";
            if (dtTolak.Value.Year <= 1990)
                dtTolak.CustomFormat = " ";
        }

        public void ModeApprove()
        {
            dtPVDate.Enabled = false;
            dtJatuhTempo.Enabled = false;
            dtCair.Enabled = false;
            dtTolak.Enabled = false;
            dtPending.Enabled = false;
            cmbPaymentMethod.Enabled = false;
            txtNoGiro.ReadOnly = true;
            txtNominal.ReadOnly = true;
            txtNotes.ReadOnly = true;

            dgvInvoice.ReadOnly = true;
            dgvDN.ReadOnly = true;
            dgvAttachment.ReadOnly = true;

            btnVendorId.Enabled = false;
            btnBankId.Enabled = false;
            btnRekVendor.Enabled = false;
            btnDeleteFile.Enabled = false;
            btnDownloadFile.Enabled = false;
            btnAddNewFile.Enabled = false;
            btnNewInvoice.Enabled = false;
            btnDeleteInvoice.Enabled = false;

            Query = " SELECT StatusCode FROM PaymentVoucher_H  where PV_No='" + txtPVNo.Text + "';";
            using(Cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
            {
                if (Cmd.ExecuteScalar().ToString() == "02" || Cmd.ExecuteScalar().ToString() == "03" || Cmd.ExecuteScalar().ToString() == "04")
                {
                    btnRevisi.Enabled = false;
                    btnReject.Enabled = false;
                    btnApprove.Enabled = false;
                }
                else
                {
                    btnRevisi.Enabled = true;
                    btnReject.Enabled = true;
                    btnApprove.Enabled = true;
                }
            }
        
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnExit.Enabled = true;

            if (dtCair.Value.Year <= 1990)
                dtCair.CustomFormat = " ";
            if (dtJatuhTempo.Value.Year <= 1990)
                dtJatuhTempo.CustomFormat = " ";
            if (dtPending.Value.Year <= 1990)
                dtPending.CustomFormat = " ";
            if (dtTolak.Value.Year <= 1990)
                dtTolak.CustomFormat = " ";
        }


        private void CreatedDgvColumn()
        {
            if (dgvInvoice.RowCount == 0)
            {
                dgvInvoice.ColumnCount = 9;
                dgvInvoice.Rows.Clear();
                dgvInvoice.Columns[0].Name = "No";
                dgvInvoice.Columns[1].Name = "Invoice No";
                dgvInvoice.Columns[2].Name = "Invoice Type";
                dgvInvoice.Columns[3].Name = "Invoice Date";
                dgvInvoice.Columns[4].Name = "Invoice Amount";
                dgvInvoice.Columns[5].Name = "Due Date";
                dgvInvoice.Columns[6].Name = "Paid";
                dgvInvoice.Columns[7].Name = "Outstanding";
                dgvInvoice.Columns[8].Name = "Payment";
                dgvInvoice.AutoResizeColumns();
            }

            if (dgvDN.RowCount == 0)
            {
                dgvDN.ColumnCount = 9;
                dgvDN.Rows.Clear();
                dgvDN.Columns[0].Name = "Check";
                dgvDN.Columns[1].Name = "No";
                dgvDN.Columns[2].Name = "DN_No";
                dgvDN.Columns[3].Name = "DN_Date";
                dgvDN.Columns[4].Name = "Account_Num";
                dgvDN.Columns[5].Name = "Account_Name";
                dgvDN.Columns[6].Name = "Currency";
                dgvDN.Columns[7].Name = "ExchRate";
                dgvDN.Columns[8].Name = "TotalAmount";
                dgvDN.AutoResizeColumns();
            }

            if (dgvAttachment.RowCount == 0)
            {
                dgvAttachment.ColumnCount = 6;
                dgvAttachment.Rows.Clear();
                dgvAttachment.Columns[0].Name = "FileName";
                dgvAttachment.Columns[1].Name = "ContentType";
                dgvAttachment.Columns[2].Name = "File Size (kb)";
                dgvAttachment.Columns[3].Name = "Attachment";
                dgvAttachment.Columns[4].Name = "Id";
                dgvAttachment.Columns[5].Name = "FileType";
                dgvAttachment.AutoResizeColumns();

                dgvAttachment.Columns[3].Visible = false;
                dgvAttachment.Columns[4].Visible = false;
            }
            dgvReadOnlyFalse();
        }

        private bool Validasi()
        {
            if (dgvInvoice.RowCount == 0)
            {
                MessageBox.Show("Detail Invoice harus diisi terlebih dahulu.");
                return false;
            }
            if (dgvAttachment.RowCount == 0)
            {
                MessageBox.Show("File Attachment harus diisi terlebih dahulu.");
                return false;
            }
            if (cmbPaymentMethod.Text == "")
            {
                MessageBox.Show("Payment Method harus dipilih terlebih dahulu.");
                return false;
            }
            if (cmbPaymentMethod.Text.ToUpper() == "TRANSFER")
            {
                if (txtBankId.Text == "")
                {
                    MessageBox.Show("Bank harus diisi terlebih dahulu.");
                    return false;
                }
                if (txtNoRekVendor.Text == "")
                {
                    MessageBox.Show("Rekening Vendor harus diisi terlebih dahulu.");
                    return false;
                }
            }
            if (cmbPaymentMethod.Text.ToUpper() == "CHEQUE")
            {
                if (txtNoGiro.Text == "")
                {
                    MessageBox.Show("No Giro harus diisi terlebih dahulu.");
                    return false;
                }
                if (Convert.ToDateTime(dtJatuhTempo.Value) < DateTime.Now && txtPVNo.Text == "")
                {
                    MessageBox.Show("Jatuh tempo harus lebih besar dari hari ini.");
                    return false;
                }
                if (dtTolak.Value.Year > 1900 && Mode != "View")
                {
                    if (txtAlasanTolak.Text == "")
                    {
                        MessageBox.Show("Alasan tolak harus diisi.");
                        return false;
                    }
                }
                if (dtPending.Value.Year > 1900 && Mode != "View")
                {
                    if (txtAlasanPending.Text == "")
                    {
                        MessageBox.Show("Alasan pending harus diisi.");
                        return false;
                    }
                }
            }

            if (dtPVDate.CustomFormat == " ")
            {
                MessageBox.Show("PV date harus diisi.");
                return false;
            }
            
            for (int i = 0; i < dgvInvoice.RowCount; i++)
            {
                if (Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment"].Value) <= 0)
                {
                    MessageBox.Show("Invoice baris ke-" + (i+1) + " harus diisi terlebih dahulu.");
                    return false;
                }
            }

            if (dtCair.CustomFormat == " ")
            {
                dtCair.CustomFormat = "dd/MM/yyyy";
                dtCair.Value = Convert.ToDateTime("1990-01-01");
            }
            if (dtJatuhTempo.CustomFormat == " ")
            {
                dtJatuhTempo.CustomFormat = "dd/MM/yyyy";
                dtJatuhTempo.Value = Convert.ToDateTime("1990-01-01");
            }
            if (dtPending.CustomFormat == " ")
            {
                dtPending.CustomFormat = "dd/MM/yyyy";
                dtPending.Value = Convert.ToDateTime("1990-01-01");
            }
            if (dtTolak.CustomFormat == " ")
            {
                dtTolak.CustomFormat = "dd/MM/yyyy";
                dtTolak.Value = Convert.ToDateTime("1990-01-01");
            }
            return true;
        }

        private void CheckedDN()
        {
            //Get DN
            Query = "SELECT [PV_No],[SeqNo],[DN_No],[DN_Date],[Account_Num],[Account_Name],[Currency],[ExchRate],[TotalAmount],[CreatedDate],[CreatedBy] FROM [dbo].[PaymentVoucherDN] ";
            Query += "where PV_No='" + txtPVNo.Text + "' order by SeqNo asc ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                for (int i = 0; i < dgvDN.RowCount; i++)
                {
                    if (Dr["DN_No"].ToString() == dgvDN.Rows[i].Cells["DN_No"].Value.ToString())
                    {
                        dgvDN.Rows[i].Cells["Check"].Value = true;
                    }
                }
            }
            Dr.Close();
        }

        private void SaveNew()
        { 
            string Jenis = "PV", kode = "PV";
            string TmpPV = ConnectionString.GenerateSequenceNo(Jenis, kode, "", "", Conn, Cmd);

            //Save Header
            Query = "INSERT INTO [dbo].[PaymentVoucher_H] ";
            Query += "(PV_Date";
            Query += ",PV_No";
            Query += ",Vend_Id";
            Query += ",Vend_Name";
            Query += ",PaymentMethod";
            Query += ",Bank_Id";
            Query += ",Bank_Name";
            Query += ",NoGiro";
            //Query += ",Receipt_Date";
            Query += ",Payment_DueDate";
            Query += ",Tgl_Cair";
            Query += ",Tgl_Tolak";
            Query += ",Tgl_Pending";
            Query += ",Total_Payment";
            Query += ",Signed_Amount";
            Query += ",StatusCode";
            Query += ",Notes";
            Query += ",NamaBankVend";
            Query += ",NoRekVend";
            Query += ",NamaRekVend";
            Query += ",CreatedDate";
            Query += ",CreatedBy) ";
            Query += "VALUES ";
            Query += "(@PV_Date";
            Query += ",@PV_No";
            Query += ",@Vend_Id";
            Query += ",@Vend_Name";
            Query += ",@PaymentMethod";
            Query += ",@Bank_Id";
            Query += ",@Bank_Name";
            Query += ",@NoGiro";
            //Query += ",@Receipt_Date";
            Query += ",@Payment_DueDate";
            Query += ",@Tgl_Cair";
            Query += ",@Tgl_Tolak";
            Query += ",@Tgl_Pending";
            Query += ",@Total_Payment";
            Query += ",@Signed_Amount";
            Query += ",@StatusCode";
            Query += ",@Notes";
            Query += ",@NamaBankVend";
            Query += ",@NoRekVend";
            Query += ",@NamaRekVend";
            Query += ",getdate()";
            Query += ",@CreatedBy) ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.Add("@PV_Date", SqlDbType.Date).Value = dtPVDate.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@PV_No", SqlDbType.VarChar, 25).Value = TmpPV;
            Cmd.Parameters.Add("@Vend_Id", SqlDbType.VarChar, 8).Value = txtVendorId.Text;
            Cmd.Parameters.Add("@Vend_Name", SqlDbType.VarChar, 150).Value = txtVendorName.Text;
            Cmd.Parameters.Add("@PaymentMethod", SqlDbType.VarChar, 10).Value = cmbPaymentMethod.Text;
            Cmd.Parameters.Add("@Bank_Id", SqlDbType.VarChar, 8).Value = txtBankId.Text;
            Cmd.Parameters.Add("@Bank_Name", SqlDbType.VarChar, 100).Value = txtBankName.Text;
            Cmd.Parameters.Add("@NoGiro", SqlDbType.VarChar, 50).Value = txtNoGiro.Text;
            Cmd.Parameters.Add("@Payment_DueDate", SqlDbType.Date).Value = dtJatuhTempo.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@Tgl_Cair", SqlDbType.Date).Value = dtCair.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@Tgl_Tolak", SqlDbType.Date).Value = dtTolak.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@Tgl_Pending", SqlDbType.Date).Value = dtPending.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@Total_Payment", SqlDbType.VarChar, 22).Value = Convert.ToDecimal(txtNominal.Text);
            Cmd.Parameters.Add("@Signed_Amount", SqlDbType.VarChar, 22).Value = Convert.ToDecimal(txtSignedAmount.Text);
            Cmd.Parameters.Add("@StatusCode", SqlDbType.VarChar, 2).Value = "01";
            Cmd.Parameters.Add("@Notes", SqlDbType.VarChar, txtNotes.Text.Length).Value = txtNotes.Text;
            Cmd.Parameters.Add("@NamaBankVend", SqlDbType.VarChar, 50).Value = txtNamaBankVendor.Text;
            Cmd.Parameters.Add("@NoRekVend", SqlDbType.VarChar, 20).Value = txtNoRekVendor.Text;
            Cmd.Parameters.Add("@NamaRekVend", SqlDbType.VarChar, 50).Value = txtNamaRekVendor.Text;
            //Cmd.Parameters.Add("@CreatedDate", SqlDbType.Date, ).Value = test[i];
            Cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, 20).Value = ControlMgr.UserId;
            Cmd.ExecuteNonQuery();
            //Save Header

            //Save Detail PV
            for(int i=0; i<dgvInvoice.RowCount; i++)
            {
                Query = "INSERT INTO [dbo].[PaymentVoucher_Dtl]";
                Query += "([PV_No]";
                Query += ",[SeqNo]";
                Query += ",[Vend_Id]";
                Query += ",[Vend_Name]";
                Query += ",[Invoice_Date]";
                Query += ",[Invoice_Due_Date]";
                Query += ",[Invoice_Id]";
                Query += ",[Invoice_Type]";
                Query += ",[Invoice_Amount]";
                //Query += ",[DN_Amount]";
                Query += ",[AP_Amount]";
                //Query += ",[Pelunasan]";
                Query += ",[CreatedDate]";
                Query += ",[CreatedBy]) ";
                Query += "VALUES (@PV_No";
                Query += ",@SeqNo";
                Query += ",@Vend_Id";
                Query += ",@Vend_Name";
                Query += ",@Invoice_Date";
                Query += ",@Invoice_Due_Date";
                Query += ",@Invoice_Id";
                Query += ",@Invoice_Type";
                Query += ",@Invoice_Amount";
                //Query = ",@DN_Amount";
                Query += ",@AP_Amount";
                //Query += ",@Pelunasan";
                Query += ",getdate()";
                Query += ",@CreatedBy);";
                //Melakukan Update PaymentInvoice
                Query += "Update[dbo].[VendInvoiceH] set TransStatus='11' where InvoiceId=@Invoice_Id;";
                //Update
                //Query += "Update VendInvoiceH set Settle_Amount=(@SettleAmount) where InvoiceId=@Invoice_Id;";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.Add("@PV_No", SqlDbType.VarChar, TmpPV.Length).Value = TmpPV;
                Cmd.Parameters.Add("@SeqNo", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["No"].Value.ToString().Length).Value = dgvInvoice.Rows[i].Cells["No"].Value.ToString();
                Cmd.Parameters.Add("@Vend_Id", SqlDbType.VarChar, txtVendorId.Text.Length).Value = txtVendorId.Text;
                Cmd.Parameters.Add("@Vend_Name", SqlDbType.VarChar, txtVendorName.Text.Length).Value = txtVendorName.Text;
                Cmd.Parameters.Add("@Invoice_Date", SqlDbType.Date).Value = Convert.ToDateTime(dgvInvoice.Rows[i].Cells["Invoice Date"].Value).ToString("yyyy-MM-dd");
                Cmd.Parameters.Add("@Invoice_Due_Date", SqlDbType.Date).Value = Convert.ToDateTime(dgvInvoice.Rows[i].Cells["Due Date"].Value).ToString("yyyy-MM-dd");
                Cmd.Parameters.Add("@Invoice_Id", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Invoice No"].Value.ToString().Length).Value = dgvInvoice.Rows[i].Cells["Invoice No"].Value.ToString();
                Cmd.Parameters.Add("@Invoice_Type", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Invoice Type"].Value.ToString().Length).Value = dgvInvoice.Rows[i].Cells["Invoice Type"].Value.ToString();
                Cmd.Parameters.Add("@Invoice_Amount", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Invoice Amount"].Value.ToString().Length).Value = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Invoice Amount"].Value.ToString());
                //Cmd.Parameters.Add("@DN_Amount", SqlDbType.VarChar, txtVendorName.Length).Value = dgvInvoide.Rows[i].Cells["InvoiceAmount"].Value.ToString();
                Cmd.Parameters.Add("@Paid", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Paid"].Value.ToString().Length).Value = Convert.ToDecimal(Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Paid"].Value.ToString()));
                Cmd.Parameters.Add("@AP_Amount", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Payment"].Value.ToString().Length).Value = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment"].Value.ToString());
                Cmd.Parameters.Add("@SettleAmount", SqlDbType.VarChar, 22).Value = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment"].Value.ToString()) + Convert.ToDecimal(Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Paid"].Value.ToString()));
                //Cmd.Parameters.Add("@Pelunasan", SqlDbType.VarChar, TmpPV.Length).Value = TmpPV;
                //Cmd.Parameters.Add("@CreatedDate", SqlDbType.VarChar, txtVendorId.Length).Value = txtVendorId.Text;
                Cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, ControlMgr.UserId.Length).Value = ControlMgr.UserId;
                Cmd.ExecuteNonQuery();
            }
            //Save Detail PV

            //Save Detail DN
            for (int i = 0; i < dgvDN.RowCount; i++)
            {
                if (Convert.ToBoolean(dgvDN.Rows[i].Cells["Check"].Value) == true)
                {
                    Query = "INSERT INTO [dbo].[PaymentVoucherDN]";
                    Query += "([PV_No]";
                    Query += ",[SeqNo]";
                    Query += ",[DN_No]";
                    Query += ",[DN_Date]";
                    Query += ",[Account_Num]";
                    Query += ",[Account_Name]";
                    Query += ",[Currency]";
                    Query += ",[ExchRate]";
                    Query += ",[TotalAmount]";
                    Query += ",[CreatedDate]";
                    Query += ",[CreatedBy]) ";
                    Query += "VALUES (@PV_No";
                    Query += ",@SeqNo";
                    Query += ",@DN_No";
                    Query += ",@DN_Date";
                    Query += ",@Account_Num";
                    Query += ",@Account_Name";
                    Query += ",@Currency";
                    Query += ",@ExchRate";
                    Query += ",@TotalAmount";
                    Query += ",getdate()";
                    Query += ",@CreatedBy);";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.Add("@PV_No", SqlDbType.VarChar, TmpPV.Length).Value = TmpPV;
                    Cmd.Parameters.Add("@SeqNo", SqlDbType.VarChar, dgvDN.Rows[i].Cells["No"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["No"].Value.ToString();
                    Cmd.Parameters.Add("@DN_No", SqlDbType.VarChar, dgvDN.Rows[i].Cells["DN_No"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["DN_No"].Value.ToString();
                    Cmd.Parameters.Add("@DN_Date", SqlDbType.Date, dgvDN.Rows[i].Cells["DN_Date"].Value.ToString().Length).Value = Convert.ToDateTime(dgvDN.Rows[i].Cells["DN_Date"].Value).ToString("yyyy-MM-dd");
                    Cmd.Parameters.Add("@Account_Num", SqlDbType.VarChar, dgvDN.Rows[i].Cells["Account_Num"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["Account_Num"].Value.ToString();
                    Cmd.Parameters.Add("@Account_Name", SqlDbType.VarChar, dgvDN.Rows[i].Cells["Account_Name"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["Account_Name"].Value.ToString();
                    Cmd.Parameters.Add("@Currency", SqlDbType.VarChar, dgvDN.Rows[i].Cells["Currency"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["Currency"].Value.ToString();
                    Cmd.Parameters.Add("@ExchRate", SqlDbType.VarChar, dgvDN.Rows[i].Cells["ExchRate"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["ExchRate"].Value.ToString();
                    Cmd.Parameters.Add("@TotalAmount", SqlDbType.VarChar, dgvDN.Rows[i].Cells["TotalAmount"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["TotalAmount"].Value.ToString();
                    Cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, ControlMgr.UserId.Length).Value = ControlMgr.UserId;
                    Cmd.ExecuteNonQuery();
                }
            }
            //Save Detail DN

            //Save Attachment
            for (int i = 0; i < dgvAttachment.RowCount; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, filetype,fileSize, attachment) Values";
                Query += "( 'PaymentVoucher', @TmpPV, ";
                Query += "@FileName, ";
                Query += "@ContentType, ";
                Query += "@Filetype, ";
                Query += "@FileSize, ";
                Query += "@binaryValue";
                Query += ");";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.Add("@TmpPV", TmpPV);
                Cmd.Parameters.Add("@FileName", dgvAttachment.Rows[i].Cells["FileName"].Value.ToString());
                Cmd.Parameters.Add("@ContentType", dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString());
                Cmd.Parameters.Add("@Filetype", dgvAttachment.Rows[i].Cells["Filetype"].Value.ToString());
                Cmd.Parameters.Add("@FileSize", dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString());
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }
            txtPVNo.Text = TmpPV;
            //Save Attachment

            //Save VendInvoice_Logs
            Query = "Select Deskripsi from TransStatusTable where TransCode='PaymentVoucher' and StatusCode='01'";
            Cmd = new SqlCommand(Query, Conn);
            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
            SaveVendInvoiceLogs(TmpPV, "-", LogsStatusDesc, "N");


            //InsertBankSystem(Conn);
        }

        private void InsertBankSystem(SqlConnection Con)
        {
            string PVNo = txtPVNo.Text;
            string PVDate = dtPVDate.Value.ToString("yyyy-MM-dd");
            string PVDue = dtJatuhTempo.Value.ToString("yyyy-MM-dd");
            string AccNo = txtNoRekVendor.Text;
            string AccName = txtVendorName.Text;
            decimal TotalAmount = Convert.ToDecimal(txtNominal.Text);

            Query = "INSERT INTO [dbo].[BankSystem] ([VoucherNo],[VoucherDate],[VoucherDueDate],[AccountNo],[AccountName],[AccountAmount],[Type],CreatedDate,[CreatedBy]) VALUES (@VoucherNo,@VoucherDate,@VoucherDueDate,@AccountNo,@AccountName,@AccountAmount,getdate(),'" + ControlMgr.UserId + "')";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Cmd.Parameters.AddWithValue("@VoucherNo", PVNo);
                Cmd.Parameters.AddWithValue("@VoucherDate", PVDate);
                Cmd.Parameters.AddWithValue("@VoucherDueDate", PVDue);
                Cmd.Parameters.AddWithValue("@AccountNo", AccNo);
                Cmd.Parameters.AddWithValue("@AccountName", AccName);
                Cmd.Parameters.AddWithValue("@AccountAmount", TotalAmount);
                Cmd.Parameters.AddWithValue("@Type", "DB");
                Cmd.ExecuteNonQuery();
            }
        }

        private void UpdateBankSystem(SqlConnection Con)
        {
            string PVNo = txtPVNo.Text;
            string PVDate = dtPVDate.Value.ToString("yyyy-MM-dd");
            string PVDue = dtJatuhTempo.Value.ToString("yyyy-MM-dd");
            string AccNo = txtNoRekVendor.Text;
            string AccName = txtVendorName.Text;
            decimal TotalAmount = Convert.ToDecimal(txtNominal.Text);

            Query = "UPDATE [dbo].[BankSystem] SET [VoucherDate]=@VoucherDate,[VoucherDueDate]=@VoucherDueDate,[AccountNo]=@AccountNo,[AccountName]=@AccountName,[AccountAmount]=@AccountAmount,[UpdatedDate] = getdate(),[UpdatedBy]='" + ControlMgr.UserId + "' WHERE [VoucherNo]=@VoucherNo";
            using (Cmd = new SqlCommand(Query, Con))
            {
                Cmd.Parameters.AddWithValue("@VoucherNo", PVNo);
                Cmd.Parameters.AddWithValue("@VoucherDate", PVDate);
                Cmd.Parameters.AddWithValue("@VoucherDueDate", PVDue);
                Cmd.Parameters.AddWithValue("@AccountNo", AccNo);
                Cmd.Parameters.AddWithValue("@AccountName", AccName);
                Cmd.Parameters.AddWithValue("@AccountAmount", TotalAmount);
                Cmd.ExecuteNonQuery();
            }
        }

        private void SaveEdit()
        {
            //Save Header
            Query = "Update [dbo].[PaymentVoucher_H] Set ";
            Query += "PV_Date=@PV_Date";
            //Query += ",PV_No='" + txtPVNo.Text + "'";
            Query += ",Vend_Id=@Vend_Id";
            Query += ",Vend_Name=@Vend_Name";
            Query += ",PaymentMethod=@PaymentMethod";
            Query += ",Bank_Id=@Bank_Id";
            Query += ",Bank_Name=@Bank_Name";
            Query += ",NoGiro=@NoGiro";
            //Query += ",Receipt_No=@Receipt_No";
            //Query += ",Receipt_Date=@Receipt_Date";
            Query += ",Payment_DueDate=@Payment_DueDate";
            Query += ",Tgl_Cair=@Tgl_Cair";
            Query += ",Tgl_Tolak=@Tgl_Tolak";
            Query += ",Tgl_Pending=@Tgl_Pending";
            Query += ",Total_Payment=@Total_Payment";
            Query += ",Signed_Amount=@Signed_Amount";
            //Query += ",StatusCode='" + "'";
            Query += ",Notes=@Notes";
            Query += ",NamaBankVend=@NamaBankVend";
            Query += ",NoRekVend=@NoRekVend";
            Query += ",NamaRekVend=@NamaRekVend";
            Query += ",UpdatedDate=getdate()";
            Query += ",CreatedBy=@CreatedBy where PV_NO ='" + txtPVNo.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.Add("@PV_Date", SqlDbType.Date).Value = dtPVDate.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@PV_No", SqlDbType.VarChar, txtPVNo.Text.Length).Value = txtPVNo.Text;
            Cmd.Parameters.Add("@Vend_Id", SqlDbType.VarChar, txtVendorId.Text.Length).Value = txtVendorId.Text;
            Cmd.Parameters.Add("@Vend_Name", SqlDbType.VarChar, txtVendorName.Text.Length).Value = txtVendorName.Text;
            Cmd.Parameters.Add("@PaymentMethod", SqlDbType.VarChar, cmbPaymentMethod.Text.Length).Value = cmbPaymentMethod.Text;
            Cmd.Parameters.Add("@Bank_Id", SqlDbType.VarChar, txtBankId.Text.Length).Value = txtBankId.Text;
            Cmd.Parameters.Add("@Bank_Name", SqlDbType.VarChar, txtNoGiro.Text.Length).Value = txtNoGiro.Text;
            Cmd.Parameters.Add("@NoGiro", SqlDbType.VarChar, txtBankName.Text.Length).Value = txtBankName.Text;
            Cmd.Parameters.Add("@Payment_DueDate", SqlDbType.Date).Value = dtJatuhTempo.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@Tgl_Cair", SqlDbType.Date).Value = dtCair.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@Tgl_Tolak", SqlDbType.Date).Value = dtTolak.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@Tgl_Pending", SqlDbType.Date).Value = dtPending.Value.ToString("yyyy-MM-dd");
            Cmd.Parameters.Add("@Total_Payment", SqlDbType.VarChar, 22).Value = Convert.ToDecimal(txtNominal.Text);
            Cmd.Parameters.Add("@Signed_Amount", SqlDbType.VarChar, 22).Value = Convert.ToDecimal(txtSignedAmount.Text);
            //Cmd.Parameters.Add("@StatusCode", SqlDbType.VarChar, 2).Value = "01";
            Cmd.Parameters.Add("@Notes", SqlDbType.VarChar, txtNotes.Text.Length).Value = txtNotes.Text;
            Cmd.Parameters.Add("@NamaBankVend", SqlDbType.VarChar, 50).Value = txtNamaBankVendor.Text;
            Cmd.Parameters.Add("@NoRekVend", SqlDbType.VarChar, 20).Value = txtNoRekVendor.Text;
            Cmd.Parameters.Add("@NamaRekVend", SqlDbType.VarChar, 50).Value = txtNamaRekVendor.Text;
            //Cmd.Parameters.Add("@CreatedDate", SqlDbType.Date, ).Value = test[i];
            Cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, ControlMgr.UserId.Length).Value = ControlMgr.UserId;
            Cmd.ExecuteNonQuery();
            //Save Header

            //Delete Detail & Attachment
            Query = "Delete from [dbo].[PaymentVoucher_Dtl] where PV_No='" + txtPVNo.Text + "';";
            Query += "Delete from [dbo].[tblAttachments] where ReffTransID='" + txtPVNo.Text + "';";
            Query += "Delete from [dbo].PaymentVoucherDN where PV_No='" + txtPVNo.Text + "';";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            //Delete Detail & Attachment

            //Save Detail PV
            for (int i = 0; i < dgvInvoice.RowCount; i++)
            {
                Query = "INSERT INTO [dbo].[PaymentVoucher_Dtl]";
                Query += "([PV_No]";
                Query += ",[SeqNo]";
                Query += ",[Vend_Id]";
                Query += ",[Vend_Name]";
                Query += ",[Invoice_Date]";
                Query += ",[Invoice_Due_Date]";
                Query += ",[Invoice_Id]";
                Query += ",[Invoice_Type]";
                Query += ",[Invoice_Amount]";
                //Query += ",[DN_Amount]";
                Query += ",[AP_Amount]";
                //Query += ",[Pelunasan]";
                Query += ",[CreatedDate]";
                Query += ",[CreatedBy]";
                Query += ",[UpdatedDate]";
                Query += ",[UpdatedBy]) ";
                Query += "VALUES (@PV_No";
                Query += ",@SeqNo";
                Query += ",@Vend_Id";
                Query += ",@Vend_Name";
                Query += ",@Invoice_Date";
                Query += ",@Invoice_Due_Date";
                Query += ",@Invoice_Id";
                Query += ",@Invoice_Type";
                Query += ",@Invoice_Amount";
                //Query = ",@DN_Amount";
                Query += ",@AP_Amount";
                //Query += ",@Pelunasan";
                Query += ",(select CreatedDate from PaymentVoucher_H where PV_NO='"+ txtPVNo.Text +"')";
                Query += ",@CreatedBy";
                Query += ",getdate()";
                Query += ",@UpdatedBy);";

                //Update
                //Query += "Update VendInvoiceH set Settle_Amount=(@SettleAmount) where InvoiceId=@Invoice_Id;";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.Add("@PV_No", SqlDbType.VarChar, txtPVNo.Text.Length).Value = txtPVNo.Text;
                Cmd.Parameters.Add("@SeqNo", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["No"].Value.ToString().Length).Value = dgvInvoice.Rows[i].Cells["No"].Value.ToString();
                Cmd.Parameters.Add("@Vend_Id", SqlDbType.VarChar, txtVendorId.Text.Length).Value = txtVendorId.Text;
                Cmd.Parameters.Add("@Vend_Name", SqlDbType.VarChar, txtVendorName.Text.Length).Value = txtVendorName.Text;
                Cmd.Parameters.Add("@Invoice_Date", SqlDbType.Date).Value = Convert.ToDateTime(dgvInvoice.Rows[i].Cells["Invoice Date"].Value).ToString("yyyy-MM-dd");
                Cmd.Parameters.Add("@Invoice_Due_Date", SqlDbType.Date).Value = Convert.ToDateTime(dgvInvoice.Rows[i].Cells["Due Date"].Value).ToString("yyyy-MM-dd");
                Cmd.Parameters.Add("@Invoice_Id", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Invoice No"].Value.ToString().Length).Value = dgvInvoice.Rows[i].Cells["Invoice No"].Value.ToString();
                Cmd.Parameters.Add("@Invoice_Type", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Invoice Type"].Value.ToString().Length).Value = dgvInvoice.Rows[i].Cells["Invoice Type"].Value.ToString();
                Cmd.Parameters.Add("@Invoice_Amount", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Invoice Amount"].Value.ToString().Length).Value = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Invoice Amount"].Value.ToString());
                Cmd.Parameters.Add("@Paid", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Paid"].Value.ToString().Length).Value = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Paid"].Value.ToString());
                //Cmd.Parameters.Add("@DN_Amount", SqlDbType.VarChar, txtVendorName.Length).Value = dgvInvoide.Rows[i].Cells["InvoiceAmount"].Value.ToString();
                Cmd.Parameters.Add("@AP_Amount", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Payment"].Value.ToString().Length).Value = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment"].Value.ToString());
                //Cmd.Parameters.Add("@Pelunasan", SqlDbType.VarChar, txtPVNo.Text).Value = txtPVNo.Text;
                //Cmd.Parameters.Add("@CreatedDate", SqlDbType.VarChar, txtVendorId.Length).Value = txtVendorId.Text;
                Cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, ControlMgr.UserId.Length).Value = ControlMgr.UserId;
                //Cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, ControlMgr.UserId.Length).Value = ControlMgr.UserId.Text;
                Cmd.Parameters.Add("@UpdatedBy", SqlDbType.VarChar, ControlMgr.UserId.Length).Value = ControlMgr.UserId;
                Cmd.Parameters.AddWithValue("@SettleAmount", Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Paid"].Value) + Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment"].Value));
                
                Cmd.ExecuteNonQuery();
            }
            //Save Detail PV

            //Save Detail DN
            for (int i = 0; i < dgvDN.RowCount; i++)
            {
                if (Convert.ToBoolean(dgvDN.Rows[i].Cells["Check"].Value) == true)
                {
                    Query = "INSERT INTO [dbo].[PaymentVoucherDN]";
                    Query += "([PV_No]";
                    Query += ",[SeqNo]";
                    Query += ",[DN_No]";
                    Query += ",[DN_Date]";
                    Query += ",[Account_Num]";
                    Query += ",[Account_Name]";
                    Query += ",[Currency]";
                    Query += ",[ExchRate]";
                    Query += ",[TotalAmount]";
                    Query += ",[CreatedDate]";
                    Query += ",[CreatedBy]) ";
                    Query += "VALUES (@PV_No";
                    Query += ",@SeqNo";
                    Query += ",@DN_No";
                    Query += ",@DN_Date";
                    Query += ",@Account_Num";
                    Query += ",@Account_Name";
                    Query += ",@Currency";
                    Query += ",@ExchRate";
                    Query += ",@TotalAmount";
                    Query += ",getdate()";
                    Query += ",@CreatedBy)";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.Add("@PV_No", SqlDbType.VarChar, txtPVNo.Text.Length).Value = txtPVNo.Text;
                    Cmd.Parameters.Add("@SeqNo", SqlDbType.VarChar, dgvDN.Rows[i].Cells["No"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["No"].Value.ToString();
                    Cmd.Parameters.Add("@DN_No", SqlDbType.VarChar, dgvDN.Rows[i].Cells["DN_No"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["DN_No"].Value.ToString();
                    Cmd.Parameters.Add("@DN_Date", SqlDbType.Date, dgvDN.Rows[i].Cells["DN_Date"].Value.ToString().Length).Value = Convert.ToDateTime(dgvDN.Rows[i].Cells["DN_Date"].Value).ToString("yyyy-MM-dd");
                    Cmd.Parameters.Add("@Account_Num", SqlDbType.VarChar, dgvDN.Rows[i].Cells["Account_Num"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["Account_Num"].Value.ToString();
                    Cmd.Parameters.Add("@Account_Name", SqlDbType.VarChar, dgvDN.Rows[i].Cells["Account_Name"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["Account_Name"].Value.ToString();
                    Cmd.Parameters.Add("@Currency", SqlDbType.VarChar, dgvDN.Rows[i].Cells["Currency"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["Currency"].Value.ToString();
                    Cmd.Parameters.Add("@ExchRate", SqlDbType.VarChar, dgvDN.Rows[i].Cells["ExchRate"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["ExchRate"].Value.ToString();
                    Cmd.Parameters.Add("@TotalAmount", SqlDbType.VarChar, dgvDN.Rows[i].Cells["TotalAmount"].Value.ToString().Length).Value = dgvDN.Rows[i].Cells["TotalAmount"].Value.ToString();
                    Cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar, ControlMgr.UserId.Length).Value = ControlMgr.UserId;
                    Cmd.ExecuteNonQuery();
                }
            }
            //Save Detail DN

            //Save Attachment
            for (int i = 0; i < dgvAttachment.RowCount; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, filetype, fileSize, attachment) Values";
                Query += "( 'PaymentVoucher', @TmpPV, ";
                Query += "@FileName, ";
                Query += "@ContentType, ";
                Query += "@Filetype, ";
                Query += "@FileSize, ";
                Query += "@binaryValue";
                Query += ");";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.Add("@TmpPV", txtPVNo.Text);
                Cmd.Parameters.Add("@FileName", dgvAttachment.Rows[i].Cells["FileName"].Value.ToString());
                Cmd.Parameters.Add("@ContentType", dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString());
                Cmd.Parameters.Add("@Filetype", dgvAttachment.Rows[i].Cells["Filetype"].Value.ToString());
                Cmd.Parameters.Add("@FileSize", dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString());
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }
            //Save Attachment

            //Save VendInvoice_Logs
            Query = "Select Deskripsi from TransStatusTable where TransCode='PaymentVoucher' and StatusCode='01'";
            Cmd = new SqlCommand(Query, Conn);
            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
            SaveVendInvoiceLogs(txtPVNo.Text, "-", LogsStatusDesc, "E");


            //UpdateBankSystem(Conn);
        }

        private void GetDataHeader()
        {
            try
            {
                if (txtPVNo.Text != "")
                {
                    using (Conn = ConnectionString.GetConnection())
                    {
                        //Get Header
                        Query = "Select [PV_Date],[PV_No],[Vend_Id],[Vend_Name],[PaymentMethod],[Bank_Id],[Bank_Name],[Receipt_No]";
                        Query += ",[Receipt_Date],[Payment_DueDate],[Tgl_Cair],[Tgl_Tolak],[Tgl_Pending],[Total_Payment],[Signed_Amount]";
                        Query += ",a.[StatusCode],b.Deskripsi,[Notes],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],NamaBankVend,NoRekVend,NamaRekVend,AlasanTolak,AlasanPending,NoGiro ";
                        Query += "FROM [dbo].[PaymentVoucher_H] a ";
                        Query += "Inner Join [TransStatusTable] b on a.StatusCode=b.StatusCode and b.TransCode='PaymentVoucher' ";
                        Query += "WHERE PV_No='" + txtPVNo.Text + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
           
                        while (Dr.Read())
                        {
                            dtPVDate.Text = Dr["PV_Date"].ToString();
                            txtPVNo.Text = Dr["PV_No"].ToString();
                            txtStatusDesc.Text = Dr["Deskripsi"].ToString();
                            txtKodeStatus.Text = Dr["StatusCode"].ToString();
                            txtVendorId.Text = Dr["Vend_Id"].ToString();
                            txtVendorName.Text = Dr["Vend_Name"].ToString();
                            cmbPaymentMethod.Text = Dr["PaymentMethod"].ToString();
                            txtNoGiro.Text =  Dr["NoGiro"].ToString();
                            txtNamaBankVendor.Text = Dr["NamaBankVend"].ToString();
                            txtNoRekVendor.Text = Dr["NoRekVend"].ToString();
                            txtNamaRekVendor.Text = Dr["NamaRekVend"].ToString();
                            dtJatuhTempo.Text = Dr["Payment_DueDate"].ToString();
                            dtCair.Text = Dr["Tgl_Cair"].ToString();
                            dtTolak.Text = Dr["Tgl_Tolak"].ToString();
                            txtAlasanTolak.Text = Dr["AlasanTolak"].ToString();
                            dtPending.Text = Dr["Tgl_Pending"].ToString();
                            txtAlasanPending.Text = Dr["AlasanPending"].ToString();
                            txtBankId.Text = Dr["Bank_Id"].ToString();
                            txtBankName.Text = Dr["Bank_Name"].ToString();
                            txtNominal.Text = Dr["Total_Payment"].ToString();
                            txtSignedAmount.Text = Dr["Signed_Amount"].ToString();
                            txtNotes.Text = Dr["Notes"].ToString();
                        }
                        Dr.Close();
                        dgvInvoice.DataSource = null;

                        //Set Header Detail
                        if (dgvInvoice.RowCount == 0)
                        {  
                            dgvInvoice.Rows.Clear();
                            dgvInvoice.ColumnCount = 9;
                            dgvInvoice.Columns[0].Name = "No";
                            dgvInvoice.Columns[1].Name = "Invoice No";
                            dgvInvoice.Columns[2].Name = "Invoice Type";
                            dgvInvoice.Columns[3].Name = "Invoice Date";

                            dgvInvoice.Columns[4].Name = "Invoice Amount";
                            dgvInvoice.Columns[5].Name = "Due Date";
                            dgvInvoice.Columns[6].Name = "Paid";

                            dgvInvoice.Columns[7].Name = "Outstanding";
                            dgvInvoice.Columns[8].Name = "Payment";
                        }
                        
                        //Get Detail
                        Query = "SELECT [PV_No],[SeqNo],[Vend_Id],[Vend_Name],[Invoice_Date],[Invoice_Id],[Invoice_Type],[Invoice_Amount]";
                        Query += ",[DN_Amount],[AP_Amount],[Pelunasan],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]";
                        Query += ",[Invoice_Due_Date],(select ApAmount from SumPaidPaymentVoucher b where b.Invoice_Id=a.Invoice_Id) Paid, (a.Invoice_Amount-(select ApAmount from SumPaidPaymentVoucher b where b.Invoice_Id=a.Invoice_Id)) Outstanding FROM [dbo].[PaymentVoucher_Dtl] a ";
                        Query += "where PV_No='" + txtPVNo.Text + "' order by SeqNo asc ";
                        
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();

                        int i = 0;

                        dgvInvoice.Rows.Clear();

                        while (Dr.Read())
                        {
                            this.dgvInvoice.Rows.Add(Dr["SeqNo"], Dr["Invoice_Id"], Dr["Invoice_Type"], Dr["Invoice_Date"]
                            , Dr["Invoice_Amount"], Dr["Invoice_Due_Date"], Dr["Paid"], Dr["Outstanding"], Dr["AP_Amount"]);
                            i++;
                        }
                        Dr.Close();

                        dgvInvoice.AutoResizeColumns();

                        //Get DN
                        Query = "SELECT [PV_No],[SeqNo],[DN_No],[DN_Date],[Account_Num],[Account_Name],[Currency],[ExchRate],[TotalAmount],[CreatedDate],[CreatedBy] FROM [dbo].[PaymentVoucherDN] ";
                        Query += "where PV_No='" + txtPVNo.Text + "' order by SeqNo asc ";

                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();

                        i = 0;

                        dgvDN.Rows.Clear();

                        while (Dr.Read())
                        {
                            TmpCheckBox = new DataGridViewCheckBoxCell();
                            this.dgvDN.Rows.Add("", Dr["SeqNo"], Dr["DN_No"], Dr["DN_Date"], Dr["Account_Num"], Dr["Account_Name"], Dr["Currency"], Dr["ExchRate"], Dr["TotalAmount"]);
                            dgvDN.Rows[(dgvDN.Rows.Count - 1)].Cells["Check"] = TmpCheckBox;
                            dgvDN.Rows[(dgvDN.Rows.Count - 1)].Cells["Check"].Value = false;
                            i++;
                        }
                        Dr.Close();

                        CheckedDN();

                        dgvInvoice.AutoResizeColumns();

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

                        Query = "Select * From [tblAttachments] Where ReffTableName = 'PaymentVoucher' And ReffTransId = '" + txtPVNo.Text + "'";
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
                }
                dgvAttachment.AutoResizeColumns();
                HitungTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HitungTotal()
        {
            decimal OldDnAmount = Convert.ToDecimal(txtDNAmount.Text);
            //Isi DN
            decimal TmpDN = 0;
            for (int i = 0; i < dgvDN.RowCount; i++)
            {
                if (Convert.ToBoolean(dgvDN.Rows[i].Cells["Check"].Value) == true)
                    TmpDN += Convert.ToDecimal(dgvDN.Rows[i].Cells["TotalAmount"].Value);
            }
            if (TmpDN < OldDnAmount)
            {
                for (int i = 0; i < dgvInvoice.Rows.Count; i++)
                {
                    dgvInvoice.Rows[i].Cells["Payment"].Value = 0;
                }
            }
            txtDNAmount.Text = TmpDN.ToString("N2");

            //Isi Text Payment
            decimal TmpPayment = 0;
            for (int i = 0; i < dgvInvoice.RowCount; i++)
            {
                TmpPayment += Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment"].Value);
            }
            txtPaymentAmount.Text = TmpPayment.ToString("N2");

            //Isi Text Invoice Amount
            decimal TmpInvoiceAmount = 0;
            for (int i = 0; i < dgvInvoice.RowCount; i++)
            {
                TmpInvoiceAmount += Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Invoice Amount"].Value);
            }
            txtInvoiceAmount.Text = TmpInvoiceAmount.ToString("N2");

            //Isi Text Paid
            decimal TmpPaid = 0;
            for (int i = 0; i < dgvInvoice.RowCount; i++)
            {
                TmpPaid += Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Paid"].Value);
            }
            txtInvoicePaidAmount.Text = TmpPaid.ToString("N2");

            txtSignedAmount.Text = (Convert.ToDecimal(txtNominal.Text) - Convert.ToDecimal(TmpPayment - TmpDN)).ToString("N2");
            txtInvoiceOutstandingAmount.Text = (Convert.ToDecimal(txtInvoiceAmount.Text) - Convert.ToDecimal(txtInvoicePaidAmount.Text)).ToString("N2");
        }
        #endregion

        public PaymentVoucher()
        {
            InitializeComponent();
        }

        public void SetParent(InquiryV1 F)
        {
            Parent = F;
        }

        public void SetParent(InquiryV2 F)
        {
            Parent1 = F;
        }

        private void PaymentVoucher_Load(object sender, EventArgs e)
        {
            this.Location = new Point(170, 23);
            CreatedDgvColumn();
            cmbPaymentMethod_Load();

            if (Variable.Kode != null)
            {
                txtPVNo.Text = Variable.Kode[0];
                GetDataHeader();
            }

            if (txtPVNo.Text == "")
            {
                ModeNew();
            }
            else
            {
                ModeView();
            }

            if (Parent1.btnApproval.Visible == true)
            {
                ModeApprove();
            }
            //tia edit
            txtVendorId.ContextMenu = CM;
            txtVendorName.ContextMenu = CM;
            //tia edit end
        }

        private void btnVendorId_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "VendId";
            tmpSearch.Order = "VendId Asc";
            tmpSearch.Table = "[dbo].[VendTable]";
            tmpSearch.QuerySearch = "SELECT a.VendId, a.VendName, a.PPN,a.PPH, a.NPWP, a.TaxName,a.TaxAddress FROM [dbo].[VendTable] a";
            tmpSearch.FilterText = new string[] { "VendId", "VendName", "PPN", "PPH", "NPWP", "TaxName", "TaxAddress" };
            tmpSearch.Select = new string[] { "VendId", "VendName" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtVendorId.Text = ConnectionString.Kodes[0];
                txtVendorName.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }
        }

        private void btnBankId_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "Kode_Bank";
            tmpSearch.Order = "Kode_Bank Asc";
            tmpSearch.Table = "VIEW_tblBank";
            tmpSearch.QuerySearch = "SELECT a.Kode_Bank,a.Nama_Bank,a.No_Rek,a.Atas_Nama FROM VIEW_tblBank a";
            tmpSearch.FilterText = new string[] { "Kode_Bank", "Nama_Bank", "No_Rek", "Atas_Nama" };
            //tmpSearch.Hide = new string[] { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Select = new string[] { "Kode_Bank", "No_Rek" };

            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtBankId.Text = ConnectionString.Kodes[0];
                txtBankName.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }
        }

        private void btnRekVendor_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "No_Rekening";
            tmpSearch.Order = "BankGroupId Asc";
            tmpSearch.Table = "[dbo].[Rekening]";
            tmpSearch.QuerySearch = "SELECT a.BankGroupId, a.BankGroupName, a.Cabang, a.No_Rekening, a.Pemilik  FROM [dbo].[Rekening] a";
            tmpSearch.FilterText = new string[] { "BankGroupID", "BankGroupName", "Cabang", "No_Rekening", "Pemilik"};
            //tmpSearch.Hide = new string[] { "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Select = new string[] { "BankGroupName", "No_Rekening", "Pemilik" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtNamaBankVendor.Text = ConnectionString.Kodes[0];
                txtNoRekVendor.Text = ConnectionString.Kodes[1];
                txtNamaRekVendor.Text = ConnectionString.Kodes[2];
                ConnectionString.Kodes = null;
            }
        }

        private void btnAddNewFile_Click(object sender, EventArgs e)
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

        private void btnNewInvoice_Click(object sender, EventArgs e)
        {
            if (txtVendorId.Text == "")
            {
                MessageBox.Show("Vendor Id harus diisi terlebih dahulu.");
                return;
            }
            
            SearchQueryV2 tmpSearch = new SearchQueryV2();
            tmpSearch.PrimaryKey = "InvoiceId";
            tmpSearch.Order = " Order by InvoiceId Asc";
            tmpSearch.Table = "[dbo].[VendInvoiceH]";
            //tmpSearch.QuerySearch = "SELECT a.InvoiceId,a.InvoiceType,a.InvoiceDate,CONVERT(money, a.InvoiceAmount) InvoiceAmount,a.DueDate,case when a.Settle_Amount is null then CONVERT(money, 0) else Convert(money,a.Settle_Amount) end Paid, case when a.Settle_Amount is null then Convert(money,a.InvoiceAmount) else Convert(money,(a.InvoiceAmount-a.Settle_Amount)) end Outstanding  FROM [dbo].[VendInvoiceH] a where a.Settle_Amount != a.InvoiceAmount and VendId='" + txtVendorId.Text + "'";
            tmpSearch.QuerySearch = "SELECT a.InvoiceId,a.InvoiceType,Convert(varchar,a.InvoiceDate,103) as InvoiceDate , Format(a.InvoiceAmount,'N2') InvoiceAmount, Convert(varchar,a.DueDate,103) as DueDate, format(case when (select ApAmount from SumPaidPaymentVoucher b where b.Invoice_Id=a.InvoiceId) is null  then 0 else (select ApAmount from SumPaidPaymentVoucher b where b.Invoice_Id=a.InvoiceId) end ,'N2')Paid, format ((a.InvoiceAmount- case when (select ApAmount from SumPaidPaymentVoucher b where b.Invoice_Id=a.InvoiceId) is null then 0 else (select ApAmount from SumPaidPaymentVoucher b where b.Invoice_Id=a.InvoiceId) end ),'N2') Outstanding FROM [dbo].[VendInvoiceH] a where a.TransStatus='03' and a.Settle_Amount != a.InvoiceAmount and VendId='" + txtVendorId.Text + "' and (a.InvoiceAmount- case when (select ApAmount from SumPaidPaymentVoucher b where b.Invoice_Id=a.InvoiceId) is null then 0 else (select ApAmount from SumPaidPaymentVoucher b where b.Invoice_Id=a.InvoiceId) end) >0";
            
            tmpSearch.FilterText = new string[] { "InvoiceId", "InvoiceType", "InvoiceDate", "DueDate" };
            //tmpSearch.Hide = new string[] { "InvoiceId", "InvoiceType", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Select = new string[] { "InvoiceId", "InvoiceType", "InvoiceDate", "InvoiceAmount", "DueDate", "Paid", "Outstanding" };
            tmpSearch.ShowDialog();
            if (Variable.Kode2 != null)
            {
                List<string> InvoiceId = new List<string>();
                List<string> InvoiceType = new List<string>();
                List<string> InvoiceDate = new List<string>();
                List<string> InvoiceAmount = new List<string>();
                List<string> DueDate = new List<string>();
                List<string> Paid = new List<string>();
                List<string> Outstanding = new List<string>();

                for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
                {
                    InvoiceId.Add(Variable.Kode2[i, 0]);
                    InvoiceType.Add(Variable.Kode2[i, 1]);
                    InvoiceDate.Add(Variable.Kode2[i, 2]);
                    InvoiceAmount.Add(Variable.Kode2[i, 3] == "" ? "0.000" : Variable.Kode2[i, 3]);
                    DueDate.Add(Variable.Kode2[i, 4]);
                    Paid.Add(Variable.Kode2[i, 5]);
                    Outstanding.Add(Variable.Kode2[i, 6]);
                }

                for (int j = 0; j < Variable.Kode2.GetLength(0); j++)
                {
                    dgvInvoice.Rows.Add(dgvInvoice.RowCount + 1, InvoiceId[j], InvoiceType[j], InvoiceDate[j], InvoiceAmount[j], DueDate[j], Paid[j], Outstanding[j], "0.00");
                }
                Variable.Kode2 = null;
            }
            AddDN();
            dgvInvoice.AutoResizeColumns();
        }

        private void btnDeleteInvoiceReference_Click(object sender, EventArgs e)
        {
            if (dgvInvoice.RowCount > 0)
            {
                Index = dgvInvoice.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data: " + Environment.NewLine + "No = " + dgvInvoice.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "Invoice No = " + dgvInvoice.Rows[Index].Cells["Invoice No"].Value.ToString() + Environment.NewLine + "Akan dihapus?", "Delete Confirmation!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvInvoice.Rows.RemoveAt(Index);
                    for (int i = 0; i < dgvInvoice.RowCount; i++)
                    {
                        dgvInvoice.Rows[i].Cells["No"].Value = i + 1;
                    }
                }
            }
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                Index = dgvAttachment.CurrentCell.RowIndex;
                DialogResult dialogResult = MessageBox.Show("Apakah data: " + Environment.NewLine + "FileName = " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString() + Environment.NewLine + Environment.NewLine + "Akan dihapus?", "Delete Confirmation!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (dgvAttachment.CurrentRow.Index > -1)
                    {
                        test.RemoveAt(dgvAttachment.CurrentRow.Index);
                        dgvAttachment.Rows.RemoveAt(dgvAttachment.CurrentRow.Index);
                    }
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
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
                    MessageBox.Show("File tidak ada dalam database / belum di masukkan.");
                    return;
                }

                using (Method C = new Method())
                {
                    Query = "Select Attachment From tblAttachments Where Id = '" + fileid + "'";
                    byte[] data = (byte[])C.ReturnScalar(Query);

                    if (sfd.ShowDialog() != DialogResult.Cancel)
                    {
                        FileStream objFileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                        objFileStream.Write(data, 0, data.Length);
                        objFileStream.Close();
                        MessageBox.Show("Data tersimpan!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk didownload");
                return;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Validasi() == true)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        if (txtPVNo.Text == "")
                        {
                            SaveNew();
                            //created by Thaddaeus, 25 Sept 2018
                            ListMethod.StatusLogVendor("PaymentVoucher", "PaymentVoucher", txtVendorId.Text, "01", "", txtPVNo.Text, "", "", "");
                            //end==============================
                            scope.Complete();
                            MessageBox.Show("Data '" + txtPVNo.Text + "' berhasil ditambahkan.");
                        }
                        else
                        {
                            SaveEdit();
                            //created by Thaddaeus, 25 Sept 2018
                            ListMethod.StatusLogVendor("PaymentVoucher", "PaymentVoucher", txtVendorId.Text, "01", "Edit", txtPVNo.Text, "", "", "");
                            //end==============================
                            scope.Complete();
                            MessageBox.Show("Data '" + txtPVNo.Text + "' berhasil diedit.");
                        }
              
                    }
                    ModeView();
                    GetDataHeader();
                    Parent1.RefreshDataGrid();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally { }
        }

        private void SaveVendInvoiceLogs(string InvoiceID, string Deskripsi, string Status, string Action)
        {
            string Query = "INSERT INTO PaymentVoucer_Logs VALUES ";
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeView();
        }

        private void dgvInvoice_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.ToString() != "")
                {
                    if (
                        e.ColumnIndex == dgvInvoice.Columns["Invoice Date"].Index ||
                        e.ColumnIndex == dgvInvoice.Columns["Due Date"].Index
                        )
                        {
                            DateTime d = DateTime.Parse(e.Value.ToString());
                            e.Value = d.ToString("dd/MM/yyyy");
                        }

                    if (
                        e.ColumnIndex == dgvInvoice.Columns["Invoice Amount"].Index ||
                        e.ColumnIndex == dgvInvoice.Columns["Paid"].Index ||
                        e.ColumnIndex == dgvInvoice.Columns["Outstanding"].Index ||
                        e.ColumnIndex == dgvInvoice.Columns["Payment"].Index
                        )
                        {
                            double d = double.Parse(e.Value.ToString());
                            e.Value = d.ToString("N2");
                        }
                }
            }
        }

        private void dgvInvoice_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInvoice.RowCount > 0)
            {
                if (dgvInvoice.Columns[dgvInvoice.CurrentCell.ColumnIndex].Name == "Payment")
                {
                    decimal TmpPayment = 0;
                    decimal DNAmount = 0;
                    for (int i = 0; i < dgvInvoice.RowCount; i++)
                    {
                        TmpPayment += Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment"].Value);
                    }
                    for (int i = 0; i < dgvDN.RowCount; i++)
                    {
                        if (Convert.ToBoolean(dgvDN.Rows[i].Cells["Check"].Value) == true)
                        {
                            DNAmount += Convert.ToDecimal(dgvDN.Rows[i].Cells["TotalAmount"].Value);
                        }
                    }
                    if (txtPVNo.Text == "")
                    {
                        if (TmpPayment > (Convert.ToDecimal(txtNominal.Text)+DNAmount))
                        {
                            MessageBox.Show("Nilai tidak boleh melebihi nilai Nominal.");
                            dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value = (Convert.ToDecimal(txtNominal.Text)+DNAmount - (TmpPayment - Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value)));

                            if (Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value) > Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Outstanding"].Value))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi nilai Outstanding.");
                                dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value = Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Outstanding"].Value);
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value) > Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Outstanding"].Value))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi nilai Outstanding.");
                                dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value = Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Outstanding"].Value);
                            }
                        }
                    }
                    else
                    {
                        //select nilai lama 
                        Conn = ConnectionString.GetConnection();
                        Query = "Select AP_Amount from PaymentVoucher_Dtl where Invoice_id='" + dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Invoice No"].Value.ToString() + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal AP_Amount = Convert.ToDecimal(Cmd.ExecuteScalar());

                        if (TmpPayment > Convert.ToDecimal(txtNominal.Text))
                        {
                            MessageBox.Show("Nilai tidak boleh melebihi nilai Nominal.");
                            dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value = (Convert.ToDecimal(txtNominal.Text) - (TmpPayment - Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value)));

                            if (Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value) > (Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Outstanding"].Value) + AP_Amount))
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi nilai Outstanding.");
                                dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value = Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Outstanding"].Value) + AP_Amount;
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value) > Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Outstanding"].Value) + AP_Amount)
                            {
                                MessageBox.Show("Nilai tidak boleh melebihi nilai Outstanding.");
                                dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Payment"].Value = Convert.ToDecimal(dgvInvoice.Rows[dgvInvoice.CurrentCell.RowIndex].Cells["Outstanding"].Value) + AP_Amount;
                            }
                        }
                    }
                    HitungTotal();
                }
            }
        }

        private void dgvDN_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            HitungTotal();
        }

        private void txtNominal_Leave(object sender, EventArgs e)
        {
            if (txtNominal.Text == "")
                txtNominal.Text = 0.ToString("N2");
            txtNominal.Text = Convert.ToDecimal(txtNominal.Text).ToString("N2");
        }

        private void txtNominal_KeyPress(object sender, KeyPressEventArgs e)
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

        private void dtTolak_ValueChanged(object sender, EventArgs e)
        {
            if (dtTolak.Value.Year > 1990 && Mode != "View")
                txtAlasanTolak.ReadOnly = false;
            else
                txtAlasanTolak.ReadOnly = true;
            dtTolak.CustomFormat = "dd/MM/yyyy";
        }

        private void dtPending_ValueChanged(object sender, EventArgs e)
        {
            if (dtPending.Value.Year > 1990 && Mode != "View")
                txtAlasanPending.ReadOnly = false;
            else
                txtAlasanPending.ReadOnly = true;
            dtPending.CustomFormat = "dd/MM/yyyy";
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (Validasi() == true)
                {
                    if (ControlMgr.GroupName.ToUpper() != "TREASURY MANAGER")
                    {
                        MessageBox.Show("User yang bisa melakukan approve hanya TREASURY MANAGER.");
                        return;
                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                        //created by Thaddaeus, 25 Sept 2018
                        if (txtPVNo.Text != "")
                        {
                            ListMethod.StatusLogVendor("PaymentVoucher", "PaymentVoucher", txtVendorId.Text, "04", "", txtPVNo.Text, "", "", "");
                        }
                        //end==============================

                        Conn = ConnectionString.GetConnection();                        

                        if (txtPVNo.Text != "")
                        {
                            Query = "Update PaymentVoucher_H set StatusCode='04' where PV_No='" + txtPVNo.Text + "';";

                            for (int i = 0; i < dgvInvoice.RowCount; i++)
                            {
                                //Update
                                Query += "Update VendInvoiceH set UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "', Settle_Amount=@Paid where InvoiceId=@Invoice_Id;";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.Add("@Paid", SqlDbType.VarChar, 22).Value = Convert.ToDecimal(Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Paid"].Value.ToString()));
                                Cmd.Parameters.Add("@Invoice_Id", SqlDbType.VarChar, dgvInvoice.Rows[i].Cells["Invoice No"].Value.ToString().Length).Value = dgvInvoice.Rows[i].Cells["Invoice No"].Value.ToString();
                                Cmd.ExecuteNonQuery();
                            }

                            if (cmbPaymentMethod.Text.ToUpper() == "TRANSFER")
                            {
                                InsertBankSystem(Conn);
                            }

                            //Save VendInvoice_Logs
                                //Get Status
                            Query = "Select Deskripsi from TransStatusTable where TransCode='PaymentVoucher' and StatusCode='04'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                                //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(txtPVNo.Text, "-", LogsStatusDesc, LogsAction);

                            //Cmd = new SqlCommand(Query, Conn);
                            //Cmd.ExecuteNonQuery();

                            //Proses PostingJournal
                            PostingJournal();
                        }
                        scope.Complete();
                        MessageBox.Show("Data '" + txtPVNo.Text + "' berhasil diapprove.");


                        btnRevisi.Enabled = false;
                        btnReject.Enabled = false;
                        btnApprove.Enabled = false;
                    }
                    //ModeView();
                    GetDataHeader();
                    Parent1.RefreshDataGrid();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void PostingJournal()
        {
            //Begin
            //Updated by : Joshua
            //Updated date : 04 Aug 2018
            //Description : Create Journal
            for (int i = 0; i < dgvInvoice.RowCount; i++)
            {
                string InvoiceNo = Convert.ToString(dgvInvoice.Rows[i].Cells["Invoice No"].Value);
                string InvoiceType = Convert.ToString(dgvInvoice.Rows[i].Cells["Invoice Type"].Value);
                decimal Payment = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment"].Value);

                string Jenis = "JN", Kode = "JN";
                string JournalID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);

                string JournalHID = "BCP01";

                //Begin Insert Header
                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID] ,[Referensi],[Posting], [Status],[CreatedDate],[CreatedBy]) ";
                Query += "VALUES ('" + JournalID + "', '" + JournalHID + "', '" + InvoiceNo + "', 0, 'Gunakan', GETDATE(), '" + ControlMgr.UserId + "') ";
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
                    decimal PPN = 0;
                    decimal PPH = 0;
                    if (InvoiceType.ToUpper() == "INVOICE")
                    {
                        Query = "select a.PV_No, a.Invoice_id, a.AP_Amount, b.TaxPercent, b.PPHTaxPercent from PaymentVoucher_Dtl a inner join [VendInvoiceH] b on a.Invoice_Id=b.InvoiceId WHERE a.Invoice_Id = '" + InvoiceNo + "' and a.PV_No='" + txtPVNo.Text + "' ";
                        Cmd = new SqlCommand(Query, Conn);
                        SqlDataReader Dr2 = Cmd.ExecuteReader();
                        while (Dr2.Read())
                        {
                            PPN = Convert.ToDecimal(Dr2["TaxPercent"]);
                            PPH = Convert.ToDecimal(Dr2["PPHTaxPercent"]);
                            Amount = Payment - PPN - PPH;

                            if (Convert.ToInt32(Dr["SeqNo"]) == 1)
                            {
                                Amount = Payment;
                                
                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                            if (Convert.ToInt32(Dr["SeqNo"]) == 4 && cmbPaymentMethod.Text.ToUpper() == "TRANSFER")
                            {
                                Amount = Payment;

                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                            if (Convert.ToInt32(Dr["SeqNo"]) == 5 && cmbPaymentMethod.Text.ToUpper() == "CASH")
                            {
                                Amount = Payment;

                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                            if (Convert.ToInt32(Dr["SeqNo"]) == 6 && cmbPaymentMethod.Text.ToUpper() == "CHEQUE")
                            {
                                Amount = Payment;

                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                        }
                        Dr2.Close();
                    }
                    else
                    {
                        Query = "select a.PV_No, a.Invoice_id, a.AP_Amount, b.TaxPercent, b.PPHTaxPercent from PaymentVoucher_Dtl a inner join [VendInvoiceH] b on a.Invoice_Id=b.InvoiceId WHERE a.Invoice_Id = '" + InvoiceNo + "' and a.PV_No='" + txtPVNo.Text + "' ";
                        Cmd = new SqlCommand(Query, Conn);
                        SqlDataReader Dr2 = Cmd.ExecuteReader();
                        while (Dr2.Read())
                        {
                            PPN = Convert.ToDecimal(Dr2["TaxPercent"]);
                            PPH = Convert.ToDecimal(Dr2["PPHTaxPercent"]);
                            Amount = Payment - PPN - PPH;

                            if (Convert.ToInt32(Dr["SeqNo"]) == 2)
                            {
                                Amount = Amount;

                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                            if (Convert.ToInt32(Dr["SeqNo"]) == 3)
                            {
                                Amount = PPN + PPH;

                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                            if (Convert.ToInt32(Dr["SeqNo"]) == 4 && cmbPaymentMethod.Text.ToUpper() == "TRANSFER")
                            {
                                Amount = Payment;

                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                            if (Convert.ToInt32(Dr["SeqNo"]) == 5 && cmbPaymentMethod.Text.ToUpper() == "CASH")
                            {
                                Amount = Payment;

                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                            if (Convert.ToInt32(Dr["SeqNo"]) == 6 && cmbPaymentMethod.Text.ToUpper() == "CHEQUE")
                            {
                                Amount = Payment;

                                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo], [JournalHID], [JournalIDSeqNo], [FQAID],[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                                Query += "VALUES ('" + JournalID + "', " + SeqNo + ", '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', ";
                                Query += "'" + Convert.ToString(Dr["FQA_Desc"]) + "', '" + Convert.ToString(Dr["Type"]) + "', 'Auto', " + Amount + ", GETDATE(), '" + ControlMgr.UserId + "' ) ";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                SeqNo++;
                            }
                        }
                        Dr2.Close();
                    }
                }
                Dr.Close();
                //End Insert Detail
                //End
            }
        }

        private void btnRevisi_Click(object sender, EventArgs e)
        {
            try
            {
                if (Validasi() == true)
                {
                    if (ControlMgr.GroupName.ToUpper() != "TREASURY MANAGER")
                    {
                        MessageBox.Show("User yang bisa melakukan approve hanya TREASURY MANAGER.");
                        return;
                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        //created by Thaddaeus, 25 Sept 2018
                        ListMethod.StatusLogVendor("PaymentVoucher", "PaymentVoucher", txtVendorId.Text, "03", "", txtPVNo.Text, "", "", "");
                        //end==============================
                        
                        if (txtPVNo.Text != "")
                        {
                            Query = "Update PaymentVoucher_H set StatusCode='03' where PV_No=@txtPVNo;";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.Add("@txtPVNo", txtPVNo.Text);
                            Cmd.ExecuteNonQuery();
                        }

                        //Save VendInvoice_Logs
                            //Get Status
                        Query = "Select Deskripsi from TransStatusTable where TransCode='PaymentVoucher' and StatusCode='03'";
                        Cmd = new SqlCommand(Query, Conn);
                        string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                            //Get Last Action
                        Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                        Cmd = new SqlCommand(Query, Conn);
                        string LogsAction = Cmd.ExecuteScalar().ToString();
                        SaveVendInvoiceLogs(txtPVNo.Text, "-", LogsStatusDesc, LogsAction);

                        scope.Complete();
                        MessageBox.Show("Data '" + txtPVNo.Text + "' berhasil diRevisi.");
                        

                        btnRevisi.Enabled = false;
                        btnReject.Enabled = false;
                        btnApprove.Enabled = false;
                    }
                    //ModeView();
                    GetDataHeader();
                    Parent1.RefreshDataGrid();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                if (Validasi() == true)
                {
                    if (ControlMgr.GroupName.ToUpper() != "TREASURY MANAGER")
                    {
                        MessageBox.Show("User yang bisa melakukan approve hanya TREASURY MANAGER.");
                        return;
                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                        //created by Thaddaeus, 25 Sept 2018
                        if (txtPVNo.Text != "")
                        {
                            ListMethod.StatusLogVendor("PaymentVoucher", "PaymentVoucher", txtVendorId.Text, "02", "", txtPVNo.Text, "", "", "");
                        }
                        //end==============================

                        Conn = ConnectionString.GetConnection();
                        if (txtPVNo.Text != "")
                        {
                            Query = "Update PaymentVoucher_H set StatusCode='02' where PV_No='" + txtPVNo.Text + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            //Save VendInvoice_Logs
                                //Get Status
                            Query = "Select Deskripsi from TransStatusTable where TransCode='PaymentVoucher' and StatusCode='02'";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                                //Get Last Action
                            Query = "Select Top 1 Action from VendInvoice_Logs order by LogDateTime desc";
                            Cmd = new SqlCommand(Query, Conn);
                            string LogsAction = Cmd.ExecuteScalar().ToString();
                            SaveVendInvoiceLogs(txtPVNo.Text, "-", LogsStatusDesc, LogsAction);

                        }
                        scope.Complete();

                        MessageBox.Show("Data '" + txtPVNo.Text + "' berhasil diReject.");

                        btnRevisi.Enabled = false;
                        btnReject.Enabled = false;
                        btnApprove.Enabled = false;
                    }
                    //ModeView();
                    GetDataHeader();
                    Parent1.RefreshDataGrid();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void dgvInvoice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvInvoice.Columns[dgvInvoice.CurrentCell.ColumnIndex].Name == "Payment")
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

        private void dgvDN_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.ToString() != "")
                {
                    if (
                        e.ColumnIndex == dgvDN.Columns["DN_Date"].Index
                        )
                    {
                        DateTime d = DateTime.Parse(e.Value.ToString());
                        e.Value = d.ToString("dd/MM/yyyy");
                    }
                    if (
                        e.ColumnIndex == dgvDN.Columns["TotalAmount"].Index
                        )
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }

                    if (
                        e.ColumnIndex == dgvDN.Columns["ExchRate"].Index
                        )
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
            }
        }

        private void dgvDN_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvInvoice_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvInvoice_KeyPress);
            }
        }

        private void TabReference_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvDN.Columns.Count != 0)
            {
                if (TabReference.SelectedTab.Text == "DN Reference")
                {
                    dgvDN.AutoResizeColumns();
                }
            }
        }

        private void TablHeaderCustomerInvoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvInvoice.Columns.Count != 0)
            {
                if (TablHeaderCustomerInvoice.SelectedTab.Text == "Payment")
                {
                    dgvInvoice.AutoResizeColumns();
                }
            }
        }

        private void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMethod.Text.ToUpper() == "GIRO")
            {
                txtNoGiro.Enabled = false;
            }
        }
        //tia edit
        //klik kanan
       // PopUp.Vendor.Vendor Vendor = null;
        AccountPayable.HeaderAccountsPayable APId = null;
        Purchase.NotaDebet.FrmT_NotaDebet DN = null;
        PopUp.Vendor.Vendor Vendor = null;

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

        private void txtVendorName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Right)
            {
                if (Vendor==null||Vendor.Text=="")
                {
                    txtVendorName.Enabled = true;
                    Vendor = new ISBS_New.PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendorId.Text);
                    Vendor.Show();
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void txtVendorId_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    txtVendorId.Enabled = true;
                    Vendor = new ISBS_New.PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendorId.Text);
                    Vendor.Show();
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }
        //masih salah untu APId
        private void dgvInvoice_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                //ApId
                if (APId == null || APId.Text == "")
                {
                    if (dgvInvoice.Columns[e.ColumnIndex].Name.ToString() == "Invoice No")
                    {
                        APId = new AccountPayable.HeaderAccountsPayable();
                        APId.SetMode("BeforeEdit", dgvInvoice.Rows[e.RowIndex].Cells["Invoice No"].Value.ToString());
                        APId.ParentRefreshGrid(this);
                        APId.Show();
                    }
                }
                else if (CheckOpened(APId.Name))
                {
                    APId.WindowState = FormWindowState.Normal;
                    APId.SetMode("BeforeEdit", dgvInvoice.Rows[e.RowIndex].Cells["Invoice No"].Value.ToString());
                    APId.ParentRefreshGrid(this);
                    APId.Show();
                    APId.Focus();
                }
               
            }
        }

        private void dgvDN_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                //AccountNum, AccountName
                if (Vendor == null || Vendor.Text == "")
                {
                    if (dgvDN.Columns[e.ColumnIndex].Name.ToString() == "Account_Num" || dgvDN.Columns[e.ColumnIndex].Name.ToString() == "Account_Name")
                    {
                        Vendor = new PopUp.Vendor.Vendor();
                        Vendor.GetData(dgvDN.Rows[e.RowIndex].Cells["Account_Num"].Value.ToString());

                        Vendor.Show();
                    }
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.GetData(dgvDN.Rows[e.RowIndex].Cells["Account_Num"].Value.ToString());
                    Vendor.Show();
                    Vendor.Focus();
                }
                //DN_No
                if (DN==null || DN.Text=="")
                {
                    if (dgvDN.Columns[e.ColumnIndex].Name.ToString()=="DN_No")
                    {
                        try
                        {
                            Variable.Kode = new string[2];
                            DN = new ISBS_New.Purchase.NotaDebet.FrmT_NotaDebet();
                            Variable.Kode[0] = dgvDN.Rows[e.RowIndex].Cells["DN_No"].Value.ToString();
                            Variable.Kode[1] = dgvDN.Rows[e.RowIndex].Cells["DN_Date"].Value.ToString();
                            DN.Show();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            Variable.Kode = null;
                        }
                    }
                }
                else if (CheckOpened(DN.Name))
                {
                     DN.WindowState = FormWindowState.Normal;
                   // DN.GetData(dgvDN.Rows[e.RowIndex].Cells["Account_Num"].Value.ToString());
                    DN.Show();
                    DN.Focus();
                }
            }
        }

        //tia edit end

        private void cmbPaymentMethod_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMethod.Text.ToUpper() == "CHEQUE")
            {
                //dtCair.Enabled = true;
                groupCheque.Enabled = true;
                groupISBS.Enabled = false;
                groupVendor.Enabled = false;
                ClearGroup();
            }
            else if (cmbPaymentMethod.Text.ToUpper() == "CASH")
            {
                //dtCair.Enabled = false;
                groupCheque.Enabled = false;
                groupISBS.Enabled = false;
                groupVendor.Enabled = false;
                ClearGroup();
            }
            else if (cmbPaymentMethod.Text.ToUpper() == "TRANSFER")
            {
                //dtCair.Enabled = false;
                groupCheque.Enabled = false;
                groupISBS.Enabled = true;
                groupVendor.Enabled = true;
                ClearGroup();
            }
        }

        private void ClearGroup()
        {
            txtBankName.Text = "";
            txtBankId.Text = "";
            txtNamaBankVendor.Text = "";
            txtNoRekVendor.Text = "";
            txtNamaRekVendor.Text = "";
            txtNoGiro.Text = "";
            dtJatuhTempo.Value = Convert.ToDateTime("01-01-1900");
            dtCair.Value = Convert.ToDateTime("01-01-1900");
            dtTolak.Value = Convert.ToDateTime("01-01-1900");
            dtPending.Value = Convert.ToDateTime("01-01-1900");
            txtAlasanTolak.Text = "";
            txtAlasanPending.Text = "";

        }

        private void dtJatuhTempo_ValueChanged(object sender, EventArgs e)
        {
            dtJatuhTempo.CustomFormat = "dd/MM/yyyy";
        }

        private void dtCair_ValueChanged(object sender, EventArgs e)
        {
            dtCair.CustomFormat = "dd/MM/yyyy";
        }

        private void dtPVDate_ValueChanged(object sender, EventArgs e)
        {
            dtPVDate.CustomFormat = "dd/MM/yyyy";
        }

    }
}
