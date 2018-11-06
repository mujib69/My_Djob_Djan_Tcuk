using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace ISBS_New.ARCollection.CollectionResult
{
    public partial class CLR_Form : MetroFramework.Forms.MetroForm
    {
        public CLR_Form()
        {
            InitializeComponent();
        }

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr, Dr_Bantu1;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        TransactionScope Scope;

        //dgvInvoice field============================================// just add the new field or configuration in this array 
        string[] TblInvoiceName = new string[] { "No", "CustId","CustName", "Invoice_Id", "SeqNo", "Kwitansi_No", "Invoice_Date", "Invoice_Amount", "Invoice_DueDate", "Result", "Payment_Amount", "Comments", "Invoice_Type" };
        //string[] TblInvoiceName = new string[] { "No", "Customer", "Invoice_No","SeqNo", "TT_No", "Invoice_Date", "Invoice_Amount", "Invoice_Due_Date", "Result", "Paid_Amount", "Comments", "Invoice_Type" };
        string[] TblInvoiceHeaderText = new string[] { "No","CustId", "Customer", "Invoice No","SeqNo", "TT No", "Invoice Date", "Invoice Amount", "Invoice DueDate", "Result", "Paid Amount", "Comments", "Invoice Type" };
        string[] TblInvoiceVisibleFalse = new string[] { "SeqNo", "CustId", "Invoice_Type" };
        string[] TblInvoiceReadOnlyFalse = new string[] { "Result", "Comments" };
        //===========================================================//

        //dgvPayment field===========================================// just add the new field or configuration in this array 
        string[] tblPaymentName = new string[] { "No", "Invoice_Id", "Invoice_Type", "Payment_Method", "Payment_No", "Payment_Date", "Payment_Amount", "Payment_DueDate", "Assigned_Amount", "Notes","SeqNo" };
        string[] tblPaymentHeaderText = new string[] { "No", "Invoice Id", "Invoice Type", "Payment Method", "Payment No", "Payment Date", "Payment Amount", "Payment DueDate", "Assigned Amount", "Notes","SeqNo" };
        string[] tblPaymentVisibleFalse = new string[] { "Invoice_Id", "Invoice_Type"};
        string[] tblPaymentReadOnlyFalse = new string[] { "Payment_Method", "Payment_No", "Payment_Date", "Payment_Amount", "Payment_DueDate","Assigned_Amount", "Notes" };
        //===========================================================//

        //dgvSO field================================================//just add the new field or configuration in this array 
        string[] tblSOName = new string[] { "SO_No", "Line_Amount", "Outstanding", "Sign_Amount" };
        string[] tblSOHeaderText = new string[] { "SO No", "Line Amount", "Outstanding", "Sign Amount" };
        string[] tblSOVisibleFalse = new string[] { };
        string[] tblSOReadOnlyFalse = new string[] { };
        //===========================================================//

        //dgvAttachedFile field======================================//just add the new field or configuration in this array 
        string[] tblAttachedFileName = new string[] { "FileType", "ReffTransID", "Customer", "fileName", "ContentType", "fileSize", "id" };
        string[] tblAttachedFileHeaderText = new string[] { "File Type", "Reference No", "Customer", "File Name", "Content Type", "File Size(Kb)", "Id" };
        string[] tblAttachedVisibleFalse = new string[] { "id" };
        string[] tblAttachedReadOnlyFalse = new string[] { "ReffTransID", "ContentType" };
        //===========================================================//

        //datatable for payment table==================================//
        DataTable Payment = new DataTable();
        //===========================================================//

        //list for deleted invoice (to update the ref status)========//
        List<string> DeletedInvoice = new List<string>();
        //===========================================================//

        DateTimePicker dtp;
        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dgvPayment.CurrentCell.Value = dtp.Text;
        }

        string Mode, Query, CLNumber, Invoice_dgv;
        int Index;

        List<byte[]> test = new List<byte[]>();

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void CLR_Form_Load(object sender, EventArgs e)
        {
            TabCLR.SelectedIndex = 0;
            GetDataHeader();
            if (Mode == "Edit")
            {      
                ModeEdit();
            }
            else if (Mode == "BeforeEdit" || Mode == "View")
            {
                ModeView();
            }
            StorePaymentDataTable();

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy";
            dtp.Visible = false;
            dtp.Width = 100;

            dgvPayment.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;
            dtp.CloseUp += this.oDateTimePicker_CloseUp;
            dgvPayment.CellBeginEdit += this.dgvPayment_CellBeginEdit;
            dgvPayment.CellEndEdit += this.dgvPayment_CellEndEdit;
            dgvInvoice.CellEndEdit += this.dgvInvoice_CellEndEdit;
        }

        ARCollection.CollectionResult.CLR_Inquery Parent;
        ARCollection.Collection.CollectionTaskList  Parent2;
        public void setParent(ARCollection.CollectionResult.CLR_Inquery f)
        {
            Parent = f;
        }

        public void setParent2(ARCollection.Collection.CollectionTaskList f)
        {
            Parent2 = f;
        }

        private void oDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            // Hiding the control after use   
            dtp.Visible = false;
        }

        private void ParentRefreshGrid()
        {
            if (Parent != null)
            {
                Parent.RefreshGrid();
            }
            if (Parent2 != null)
            {
                Parent2.RefreshAccess();
            }
        }

        public void SetMode(string tmpMode, string tmpCLNumber)
        {
            Mode = tmpMode;
            CLNumber = tmpCLNumber;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            dtCLDate.Enabled = false;

            txtCLNo.ReadOnly = true;
            txtCollector.ReadOnly = true;
            txtStatus.ReadOnly = true;
            txtType.ReadOnly = true;

            btnAdd.Enabled = true;
            btnDeleteGrid1.Enabled = true;

            btnNew.Enabled = true;
            btnDelete.Enabled = true;
            btnNewFiles.Enabled = true;
            btnDownload.Enabled = true;
            btnDeleteFiles.Enabled = true;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnCancel.Enabled = true;

            TabCLR.SelectedIndex = 0;

            GridColumnsConfiguration();
        }

        public void ModeView()
        {
            Mode = "View";

            dtCLDate.Enabled = false;

            txtCLNo.ReadOnly = true;
            txtCollector.ReadOnly = true;
            txtStatus.ReadOnly = true;
            txtType.ReadOnly = true;

            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            btnNewFiles.Enabled = false;
            btnDownload.Enabled = false;
            btnDeleteFiles.Enabled = false;
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnCancel.Enabled = false;

            btnAdd.Enabled = false;
            btnDeleteGrid1.Enabled = false;

            dgvInvoice.ReadOnly = true;
            dgvPayment.ReadOnly = true;
            dgvSO.ReadOnly = true;
            dgvAttachedFile.ReadOnly = true;

            TabCLR.SelectedIndex = 0;

            GridColumnsConfiguration();
        }

        private void UpdateCollectionDtl()
        {
            for (int i = 0; i < dgvInvoice.Rows.Count; i++)
            {
                bool statusnew = true;
                Query = "SELECT * FROM [Collection_Dtl] WHERE [CL_No]=@CL_No AND [SeqNo]=@SeqNo";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@CL_No", txtCLNo.Text);
                    Cmd.Parameters.AddWithValue("@SeqNo", dgvInvoice.Rows[i].Cells["SeqNo"].Value);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        statusnew = false;
                    }
                    Dr.Close();
                }
                if (statusnew == true)
                {
                    Query = "INSERT INTO [Collection_Dtl]([CL_No],[SeqNo],[CustId],[Cust_Name],[Invoice_Type],[Invoice_Id],[TT_No],[Result],[Comments],[CreatedDate],[CreatedBy]) ";
                    Query += " VALUES(@CL_No,@SeqNo,@CustId,@Cust_Name,@Invoice_Type,@Invoice_Id,@TT_No,@Result,@Comments,getdate(),@CreatedBy) ";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        //{ "No", "CustId","CustName", "Invoice_Id", "SeqNo", "Kwitansi_No", "Invoice_Date", "Invoice_Amount", "Invoice_DueDate", "Result", "Payment_Amount", "Comments", "Invoice_Type" };
                        Cmd.Parameters.AddWithValue("@CL_No",txtCLNo.Text);
                        Cmd.Parameters.AddWithValue("@SeqNo",dgvInvoice.Rows[i].Cells["SeqNo"].Value);
                        Cmd.Parameters.AddWithValue("@CustId", dgvInvoice.Rows[i].Cells["CustId"].Value);
                        Cmd.Parameters.AddWithValue("@Cust_Name",dgvInvoice.Rows[i].Cells["CustName"].Value);
                        Cmd.Parameters.AddWithValue("@Invoice_Type",dgvInvoice.Rows[i].Cells["Invoice_Type"].Value);
                        Cmd.Parameters.AddWithValue("@Invoice_Id",dgvInvoice.Rows[i].Cells["Invoice_Id"].Value);
                        Cmd.Parameters.AddWithValue("@TT_No", dgvInvoice.Rows[i].Cells["Kwitansi_No"].Value);
                        Cmd.Parameters.AddWithValue("@Result",dgvInvoice.Rows[i].Cells["Result"].Value);
                        Cmd.Parameters.AddWithValue("@Comments",dgvInvoice.Rows[i].Cells["Comments"].Value.ToString().Trim());
                        Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();
                    }
                }
                else if (statusnew == false)
                {
                    Query = "UPDATE [Collection_Dtl] SET [Result]=@Result,[UpdatedDate]=getdate(),[UpdatedBy]=@UpdatedBy WHERE [CL_No]=@CL_No AND [SeqNo]=@SeqNo";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.Parameters.AddWithValue("@Result", dgvInvoice.Rows[i].Cells["Result"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@UpdatedBy", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@CL_No", txtCLNo.Text);
                        Cmd.Parameters.AddWithValue("@SeqNo", dgvInvoice.Rows[i].Cells["SeqNo"].Value);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private string validation()
        {
            string msg = "";
            if (dgvInvoice.Rows.Count == 0)
            {
                msg += "-Tidak ada invoice yang diproses.\n\r";
            }
            for (int i = 0; i < dgvInvoice.RowCount; i++)
            {
                if (dgvInvoice.Rows[i].Cells["Comments"].Value.ToString() == "")
                {
                    if (dgvInvoice.Rows[i].Cells["Result"].Value != null && dgvInvoice.Rows[i].Cells["Result"].Value.ToString() == "Tidak Tertagih")
                    {
                        msg += "-Invoice No." + dgvInvoice.Rows[i].Cells["Invoice_Id"].Value + ", COMENTS harus diisi Karena Result yang dipilih 'Tidak Tertagih'\n\r";
                    }
                }
                if (dgvInvoice.Rows[i].Cells["Result"].Value == null || dgvInvoice.Rows[i].Cells["Result"].Value.ToString() == "")
                {
                    msg += "-Invoice No." + dgvInvoice.Rows[i].Cells["Invoice_Id"].Value + ", RESULT harus dipilih. \n\r";
                }
                if (dgvInvoice.Rows[i].Cells["Result"].Value != null && dgvInvoice.Rows[i].Cells["Result"].Value.ToString() == "Tertagih")
                {
                    //DataRow Row = Payment.Rows.Cast<DataRow>().Where(r => r["Invoice_Id"].Equals(dgvInvoice.Rows[i].Cells["Invoice_Id"].Value.ToString())).First;
                    bool has = Payment.Rows.Cast<DataRow>().Any(r => r["Invoice_Id"].Equals(dgvInvoice.Rows[i].Cells["Invoice_Id"].Value.ToString()));
                    if (has == false)
                    {
                        msg += "-Invoice No." + dgvInvoice.Rows[i].Cells["Invoice_Id"].Value + ", belom mempunyai payment. \n\r";
                    }
                    else
                    {
                        DataRow[] Row = Payment.Rows.Cast<DataRow>().Where(r => r["Invoice_Id"].Equals(dgvInvoice.Rows[i].Cells["Invoice_Id"].Value.ToString())).ToArray();
                        for (int x = 0; x < Row.Count();x++ )
                        {
                            string submsg = "";
                            for (int j = 0; j < tblPaymentName.Count();j++ )
                            {
                                if (tblPaymentName[j].ToString() != "Notes" && tblPaymentName[j].ToString() != "No")
                                {
                                    if (Row[x][tblPaymentName[j]] == null || Row[x][tblPaymentName[j]].ToString() == "")
                                    {
                                        submsg += " '"+tblPaymentHeaderText[j].ToString()+"'";
                                    }
                                }
                            }
                            if (submsg != "")
                            {
                                msg += "-Invoice No." + dgvInvoice.Rows[i].Cells["Invoice_Id"].Value + ", row ke " + (x + 1) + " kolom" + submsg + " masih kosong.\n\r";
                            }
                            else if (Convert.ToDecimal(Row[x]["Payment_Amount"]) < Convert.ToDecimal(Row[x]["Assigned_Amount"]))
                            {
                                msg += "-Invoice No." + dgvInvoice.Rows[i].Cells["Invoice_Id"].Value + ", row ke " + (x + 1) + ", Assigned Amount tidak boleh melebihi Payment Amount.\n\r";
                            }
                        }
                    }
                    //bool has = list.Any(cus => cus.FirstName == "John");
                }
            }
            for (int i = 0; i < dgvAttachedFile.RowCount; i++)
            {
                //{ "FileType", "ReffTransID", "Customer", "fileName", "ContentType", "fileSize", "id" };
                if (dgvAttachedFile.Rows[i].Cells["FileType"].Value == null || dgvAttachedFile.Rows[i].Cells["FileType"].Value.ToString() == "")
                {
                    msg += "-Attchment row "+(i+1)+" belom memilih File Typenya. \n\r";
                }
                if (dgvAttachedFile.Rows[i].Cells["ReffTransID"].Value == null || dgvAttachedFile.Rows[i].Cells["ReffTransID"].Value.ToString() == "")
                {
                    msg += "-Attchment row " + (i + 1) + " belom memilih Invoice referensinya. \n\r";
                }
            }

            return msg;
        }

        private void PopulateCmbInvoicePayment()
        {
            cmbInvoicePayment.Items.Clear();
            for(int i=0; i< dgvInvoice.Rows.Count; i++)
            {
                if (dgvInvoice.Rows[i].Cells["Result"].Value != null && dgvInvoice.Rows[i].Cells["Result"].Value.ToString() == "Tertagih")
                {
                    cmbInvoicePayment.Items.Add(dgvInvoice.Rows[i].Cells["Invoice_Id"].Value.ToString());
                }
            }
        }

        private void EstablishedColumns()
        {
            if (dgvInvoice.RowCount == 0)
            {
                Create_dgvInvoice();
            }
            Create_dgvPayment();
            Create_dgvSO();
            Create_dgvAttachedFile();
        }

        public void GetDataHeader()
        {
            if (CLNumber != "")
            {
                dgvInvoice.Rows.Clear();
                dgvPayment.Rows.Clear();
                dgvSO.Rows.Clear();
                dgvAttachedFile.Rows.Clear();

                Conn = ConnectionString.GetConnection();

                Query = "SELECT CLH.CL_Date, CLH.CL_No, CLH.Collector, TST.Deskripsi, CLH.CL_Type FROM Collection_H CLH ";
                Query += "LEFT JOIN TransStatusTable TST ON CLH.Status_Code = TST.StatusCode AND TST.TransCode = 'Collection' ";
                Query += "WHERE CL_No = '" + CLNumber + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtCLDate.Text = Dr["CL_Date"].ToString();
                    txtCLNo.Text = CLNumber;
                    txtCollector.Text = Dr["Collector"].ToString();
                    txtStatus.Text = Dr["Deskripsi"].ToString();
                    txtType.Text = Dr["CL_Type"].ToString();
                }
                Dr.Close();

                EstablishedColumns();

                refreshGrid();

                Conn.Close();
            }
        }

        private void refreshGrid()
        {
            populateGridInvoice();
            PopulateCmbInvoicePayment();
            PopulateGridSO("");
            populateGridAttachedFile();
            populateGridPayment();
        }

        private void PopulateGridSO(string InvoiceId)
        {
            dgvSO.Rows.Clear();
            string Inv_No = "";
            string Inv_Type = "";
            string Customer = "";
            if (InvoiceId == "")
            {
                Inv_No = cmbInvoicePayment.Text;
            }
            else
            {
                cmbInvoicePayment.SelectedItem=InvoiceId;
                Inv_No = InvoiceId;
            }

            if (Inv_No != "Select" && Inv_No != "")
            {
                try
                {
                    DataGridViewRow row = dgvInvoice.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["Invoice_Id"].Value.ToString().Equals(Inv_No)).First();
                    if (row.Cells["Invoice_Type"].Value != null)
                    {
                        Inv_Type = row.Cells["Invoice_Type"].Value.ToString();
                    }
                    if (row.Cells["CustName"].Value != null)
                    {
                        Customer = row.Cells["CustName"].Value.ToString();
                    }
                    txtInvoiceType.Text = Inv_Type;
                    txtCustomer.Text = Customer;
                }
                catch
                {
                    MessageBox.Show("Invoice Id tidak ditemukan pada grid invoice.");
                }
                finally { }
            }
            else
            {
                txtInvoiceType.Text = "";
                txtCustomer.Text = "";
            }

            Query = "";
            if (Inv_Type == "Invoice")
            {
                Query = "SELECT CLD.CL_No, CLD.Invoice_Id, CID.SO_No, Invoice_Amount, PaymentAmount, ISNULL(Invoice_Amount,0)-ISNULL(PaymentAmount,0) AS Outstanding ";
                Query += "FROM Collection_Dtl CLD JOIN CustInvoice_Dtl_SO CID ON CLD.Invoice_Id = CID.Invoice_Id WHERE CLD.CL_No = '" + CLNumber + "' AND CID.Invoice_Id = '" + Inv_No + "' ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        this.dgvSO.Rows.Add(Dr["SO_No"], Dr["Invoice_Amount"], Dr["Outstanding"], "");
                    }
                    Dr.Close();
                }
                
            }
            else if (Inv_Type == "Down Payment")
            {
                Query = "SELECT CLD.CL_No, CLD.Invoice_Id, CID.SO_No, Line_Amount, PaymentAmount, ISNULL(Line_Amount,0)-ISNULL(PaymentAmount,0) AS Outstanding ";
                Query += "FROM Collection_Dtl CLD JOIN CustInvoice_Dtl_DP CID ON CLD.Invoice_Id = CID.Invoice_Id WHERE CLD.CL_No = '" + CLNumber + "' AND CID.Invoice_Id = '" + Inv_No + "' ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        this.dgvSO.Rows.Add(Dr["SO_No"], Dr["Line_Amount"], Dr["Outstanding"], "");
                    }
                    Dr.Close();
                }
            }

            dgvSO.AutoResizeColumns();
        }

        private void populateGridAttachedFile()
        {
            dgvAttachedFile.Rows.Clear();
            Query = "SELECT *,b.Invoice_Id,c.Cust_Name FROM [dbo].[tblAttachments] a LEFT JOIN [Collection_Payment] b ON a.id=b.AttachmentId LEFT JOIN [Collection_Dtl] c ON c.[Invoice_Id]=b.[Invoice_Id] WHERE a.[ReffTableName] = 'Collection_Payment' AND a.[ReffTransID] = @ReffTransID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@ReffTransID",txtCLNo.Text);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dgvAttachedFile.Rows.Add();
                    test.Add((byte[])Dr["attachment"]);
                    for (int i = 0; i < tblAttachedFileName.Count(); i++)
                    {
                        if (tblAttachedFileName[i] == "Customer")
                        {
                            dgvAttachedFile.Rows[dgvAttachedFile.Rows.Count - 1].Cells["Customer"].Value = Dr["Cust_Name"];
                        }
                        else if (tblAttachedFileName[i] == "ReffTransID")
                        {
                            DataGridViewComboBoxCell cmb = new DataGridViewComboBoxCell();
                            for (int x = 0; x < cmbInvoicePayment.Items.Count; x++)
                            {
                                cmb.Items.Add(cmbInvoicePayment.Items[x]);
                            }
                            if (cmbInvoicePayment.Items.Contains(Dr["Invoice_Id"].ToString()))
                            {
                                dgvAttachedFile.Rows[(dgvAttachedFile.Rows.Count - 1)].Cells["ReffTransID"] = cmb;
                                dgvAttachedFile.Rows[(dgvAttachedFile.Rows.Count - 1)].Cells["ReffTransID"].Value = Dr["Invoice_Id"];
                            }
                            else
                            {
                                dgvAttachedFile.Rows.RemoveAt(dgvAttachedFile.Rows.Count-1);
                                break;
                            }
                        }
                        else if (tblAttachedFileName[i] == "FileType")
                        {
                            DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                            cell.Items.Add("Document Nota");
                            cell.Items.Add("Cheque");
                            dgvAttachedFile.Rows[(dgvAttachedFile.Rows.Count - 1)].Cells["FileType"] = cell;
                            dgvAttachedFile.Rows[dgvAttachedFile.Rows.Count - 1].Cells["FileType"].Value = Dr["FileType"];
                        }
                        else
                        {
                            dgvAttachedFile.Rows[dgvAttachedFile.Rows.Count - 1].Cells[tblAttachedFileName[i]].Value = Dr[tblAttachedFileName[i]];
                        }
                    }
                }
                Dr.Close();
            }
            //using (cmd.Connection = ConnectionString.GetConnection())
            //if (cmbInvoicePayment.Items.Count > 0)
            //{
            //    var parameters = new string[cmbInvoicePayment.Items.Count];
            //    var cmd = new SqlCommand();
            //    for (int i = 0; i < cmbInvoicePayment.Items.Count; i++)
            //    {
            //        parameters[i] = string.Format("@ReffTableName{0}", i);
            //        cmd.Parameters.AddWithValue(parameters[i], cmbInvoicePayment.Items[i].ToString());
            //    }

            //    cmd.CommandText = string.Format("SELECT * FROM [dbo].[tblAttachments] WHERE [ReffTableName] = 'Collection_Payment' AND [ReffTransID] IN ({0})", string.Join(",", parameters));
            //    using (cmd.Connection = ConnectionString.GetConnection())
            //    {
            //        Dr = cmd.ExecuteReader();
            //        while (Dr.Read())
            //        {
            //            dgvAttachedFile.Rows.Add();
            //            for (int i = 0; i < tblAttachedFileName.Count(); i++)
            //            {
            //                if (tblAttachedFileName[i] != "Customer")
            //                {
            //                    dgvAttachedFile.Rows[dgvAttachedFile.Rows.Count - 1].Cells[tblAttachedFileName[i]].Value = Dr[tblAttachedFileName[i]];
            //                }
            //            }
            //        }
            //        Dr.Close();
            //    }
            //}
        }

        private void populateGridPayment()
        {
            dgvPayment.Rows.Clear();
            DataRow[] row = Payment.Rows.Cast<DataRow>().Where(r => r["Invoice_Id"].Equals(cmbInvoicePayment.Text)).ToArray();
            for (int i = 0; i < row.Count(); i++)
            {
                dgvPayment.Rows.Add();
                for (int x = 0; x < tblPaymentName.Count(); x++)
                {
                    if (tblPaymentName[x] == "No")
                    {
                        dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells[tblPaymentName[x]].Value = dgvPayment.Rows.Count;
                    }
                    else if (tblPaymentName[x] == "Payment_Method")
                    {
                        cmbPM = new DataGridViewComboBoxCell();
                        using (Cmd = new SqlCommand("select PaymentModeName from PaymentMode ", ConnectionString.GetConnection()))
                        {
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                cmbPM.Items.Add(Dr["PaymentModeName"].ToString());
                            }
                            Dr.Close();
                        }
                        dgvPayment.Rows[(dgvPayment.Rows.Count - 1)].Cells["Payment_Method"] = cmbPM;

                        dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells[tblPaymentName[x]].Value = row[i][tblPaymentName[x]];
                    }
                    //else if (tblPaymentName[x].Contains("Date"))
                    //{
                    //    dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells[tblPaymentName[x]].Value = (row[i][tblPaymentName[x]]);
                    //}
                    else if (tblPaymentName[x].Contains("Amount"))
                    {
                        dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells[tblPaymentName[x]].Value = Convert.ToDecimal(String.Format("{0:0.00}",Convert.ToDecimal(row[i][tblPaymentName[x]])));
                    }
                    else
                    {
                        dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells[tblPaymentName[x]].Value = row[i][tblPaymentName[x]];
                    }
                }
            }
        }

        DataGridViewComboBoxCell CmbResult;
        private void populateGridInvoice()
        {
            Query = "SELECT CASE WHEN CLD.Cust_Name = NULL OR CLD.Cust_Name = '' THEN CT.CustName ELSE CLD.Cust_Name END AS 'CustName',CASE WHEN CLD.CustId = NULL OR CLD.CustId = '' THEN CT.CustId ELSE CLD.CustId END AS 'CustId', CLD.Invoice_Id,CLD.SeqNo, CIH.Kwitansi_No, CIH.Invoice_Date, CIH.Invoice_Amount, CIH.Invoice_DueDate, CLD.Result, CLD.Payment_Amount, CLD.Comments, CLD.Invoice_Type ";
            Query += "FROM Collection_Dtl CLD LEFT JOIN CustInvoice_H CIH ON CLD.Invoice_Id = CIH.Invoice_Id LEFT JOIN CustTable CT ON CIH.Cust_Id = CT.CustId ";
            Query += "WHERE CL_No = '" + txtCLNo.Text + "' ";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dgvInvoice.Rows.Add();
                    for (int i = 0; i < TblInvoiceName.Count(); i++)
                    {
                        if (TblInvoiceName[i] == "No")
                        {
                            dgvInvoice.Rows[dgvInvoice.Rows.Count - 1].Cells["No"].Value = dgvInvoice.Rows.Count;
                        }
                        //else if (TblInvoiceName[i].Contains("Date"))
                        //{
                        //    dgvInvoice.Rows[dgvInvoice.Rows.Count - 1].Cells[TblInvoiceName[i]].Value = Convert.ToDateTime(Dr[TblInvoiceName[i]]).ToString("dd/MM/yyyy");
                        //}
                        else if (TblInvoiceName[i] == "Result")
                        {
                            CmbResult = new DataGridViewComboBoxCell();
                            CmbResult.Items.Add("Tertagih");
                            CmbResult.Items.Add("Tidak Tertagih");
                            if (Dr["Result"] != null || Dr["Result"].ToString() != "")
                            {
                                CmbResult.Value = Dr["Result"].ToString();
                            }
                            dgvInvoice.Rows[(dgvInvoice.Rows.Count - 1)].Cells["Result"] = CmbResult;
                        }
                        else
                        {
                            dgvInvoice.Rows[dgvInvoice.Rows.Count - 1].Cells[TblInvoiceName[i]].Value = Dr[TblInvoiceName[i]];
                        }
                    }
                }
                Dr.Close();
            }

            dgvInvoice.Columns["Invoice_Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvInvoice.Columns["Invoice_DueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void InsertCLPayment()
        {
            for (int i = 0; i < Payment.Rows.Count; i++)
            {
                DateTime date;
                Query = "INSERT INTO [dbo].[Collection_Payment] ([CL_No],[SeqNo],[Invoice_Id],[Invoice_Type],[Payment_Method],[Payment_No],[Payment_Date],[Payment_Amount],[Payment_DueDate],[Assigned_Amount],[Notes],[CreatedDate],[CreatedBy]) ";
                Query += " VALUES (@CL_No,@SeqNo,@Invoice_Id,@Invoice_Type,@Payment_Method,@Payment_No,@Payment_Date,@Payment_Amount,@Payment_DueDate,@Assigned_Amount,@Notes,getdate(),@CreatedBy)";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@CL_No", txtCLNo.Text);
                    Cmd.Parameters.AddWithValue("@SeqNo", Payment.Rows[i]["SeqNo"]);
                    Cmd.Parameters.AddWithValue("@Invoice_Id", Payment.Rows[i]["Invoice_Id"]);
                    Cmd.Parameters.AddWithValue("@Invoice_Type", Payment.Rows[i]["Invoice_Type"]);
                    Cmd.Parameters.AddWithValue("@Payment_Method", Payment.Rows[i]["Payment_Method"]);
                    Cmd.Parameters.AddWithValue("@Payment_No", Payment.Rows[i]["Payment_No"]);
                    date = new DateTime();
                    date = convertDateFormat((Payment.Rows[i]["Payment_Date"].ToString()));
                    Cmd.Parameters.AddWithValue("@Payment_Date", date);
                    Cmd.Parameters.AddWithValue("@Payment_Amount", (Payment.Rows[i]["Payment_Amount"]));
                    date = new DateTime();
                    date = convertDateFormat((Payment.Rows[i]["Payment_DueDate"].ToString()));
                    Cmd.Parameters.AddWithValue("@Payment_DueDate", date);
                    Cmd.Parameters.AddWithValue("@Assigned_Amount", Payment.Rows[i]["Assigned_Amount"]);
                    Cmd.Parameters.AddWithValue("@Notes", "");
                    Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private DateTime convertDateFormat(string date) //for dd/MM/yyyy format
        {
            DateTime Date = new DateTime();
            Date = DateTime.ParseExact(date, "dd/MM/yyyy", null);
            return Date;
        }

        private void InsertAttachment()
        {
            for (int i = 0; i < dgvAttachedFile.Rows.Count; i++)
            {
                int attachedid = 0;
                Query = "INSERT INTO [dbo].[tblAttachments] ([FileType],[ReffTableName],[ReffTransID],[fileName],[ContentType],[fileSize],[attachment],CreatedDate,CreatedBy) OUTPUT INSERTED.id VALUES (@FileType,@ReffTableName,@ReffTransID,@fileName,@ContentType,@fileSize,@attachment,getdate(),@CreatedBy) ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@ReffTableName", "Collection_Payment");
                    Cmd.Parameters.AddWithValue("@FileType", dgvAttachedFile.Rows[i].Cells["FileType"].Value);
                    Cmd.Parameters.AddWithValue("@ReffTransID", txtCLNo.Text);
                    Cmd.Parameters.AddWithValue("@fileName", dgvAttachedFile.Rows[i].Cells["fileName"].Value);
                    Cmd.Parameters.AddWithValue("@ContentType", dgvAttachedFile.Rows[i].Cells["ContentType"].Value);
                    Cmd.Parameters.AddWithValue("@fileSize", dgvAttachedFile.Rows[i].Cells["fileSize"].Value);
                    Cmd.Parameters.Add("@attachment", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                    Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                    attachedid = Convert.ToInt32(Cmd.ExecuteScalar());
                }

                Query = "UPDATE [dbo].[Collection_Payment] SET [AttachmentId]=@AttachmentId WHERE [CL_No]=@CL_No AND [Invoice_Id]=@Invoice_Id";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@AttachmentId", attachedid);
                    Cmd.Parameters.AddWithValue("@CL_No",txtCLNo.Text);
                    Cmd.Parameters.AddWithValue("@Invoice_Id",dgvAttachedFile.Rows[i].Cells["ReffTransID"].Value);
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Payment.Clear();
            dgvPayment.Rows.Clear();
            DeletedInvoice.Clear();
            GetDataHeader();
            StorePaymentDataTable();
            ModeView();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string msg = validation();
            if (msg != "")
            {
                MessageBox.Show(msg);
                return;
            }
            try
            {
                using (Scope = new TransactionScope())
                {
                    UpdateCollectionDtl();
                    //Delete previous collection payment
                    Query = "DELETE FROM [dbo].[Collection_Payment] WHERE CL_No = @CL_No";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.Parameters.AddWithValue("@CL_No", txtCLNo.Text);
                        Cmd.ExecuteNonQuery();
                    }
                    InsertCLPayment();
                    //Delete previous attachment
                    Query = "DELETE FROM [dbo].[tblAttachments] WHERE [ReffTableName] = @ReffTableName AND [ReffTransID]=@ReffTransID";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.Parameters.AddWithValue("@ReffTableName", "Collection_Payment");
                        Cmd.Parameters.AddWithValue("@ReffTransID",txtCLNo.Text);
                        Cmd.ExecuteNonQuery();
                    }
                    //insert hv to be after insertclpayment
                    InsertAttachment();

                    UpdateRefStatus();

                    InsertCLRLog("","","01","E");

                    UpdateHeaderStatus();

                    ModeView();
                    Scope.Complete();

                    MessageBox.Show("Collection berhasil di save.");
                }
                ParentRefreshGrid();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {

            }
        }

        private void UpdateHeaderStatus()
        {
            Query = "UPDATE [dbo].[Collection_H] SET [Status_Code]='03',UpdatedBy = @UpdatedBy,[UpdatedDate] = getdate() WHERE [CL_No]=@CL_No";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@UpdatedBy",ControlMgr.UserId);
                Cmd.Parameters.AddWithValue("@CL_No",txtCLNo.Text);
                Cmd.ExecuteNonQuery();
            }
        }

        private void UpdateRefStatus()
        {
            string transstatus = "";
            Conn = ConnectionString.GetConnection();
            for (int i = 0; i < DeletedInvoice.Count; i++)
            {
                Query = "SELECT * FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] = @Invoice_Id ";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@Invoice_Id", DeletedInvoice[i]);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if(Convert.ToDecimal(Dr["Settle_Amount"]) == 0)
                        {
                            transstatus = "03";
                        }
                        else if (Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"]) != 0)
                        {
                            transstatus = "42";
                        }
                        else
                        {
                            transstatus = "41";
                        }
                    }
                    Dr.Close();
                }

                Query = "UPDATE [dbo].[CustInvoice_H] SET [TransStatus] = @TransStatus,UpdatedDate = getdate(), UpdatedBy = @UpdatedBy WHERE [Invoice_Id] = @Invoice_Id ";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@TransStatus",transstatus);
                    Cmd.Parameters.AddWithValue("@Invoice_Id",DeletedInvoice[i]);
                    Cmd.Parameters.AddWithValue("@UpdatedBy",ControlMgr.UserId);
                    Cmd.ExecuteNonQuery();
                }
            }

            for (int i = 0; i < dgvInvoice.Rows.Count; i++)
            {
                string invoiceid = dgvInvoice.Rows[i].Cells["Invoice_Id"].Value.ToString();
                decimal invoiceAmount = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Invoice_Amount"].Value);
                decimal invoicePaid = Convert.ToDecimal(dgvInvoice.Rows[i].Cells["Payment_Amount"].Value);
                if (dgvInvoice.Rows[i].Cells["Result"].Value.ToString() == "Tertagih")
                {
                    decimal invoicepayment = Payment.Rows.Cast<DataRow>().Where(r => r["Invoice_Id"].Equals(invoiceid)).Select(r => Convert.ToDecimal(r["Assigned_Amount"])).Sum();
                    if ((invoicepayment + invoicePaid) == invoiceAmount)
                    {
                        transstatus = "21";
                    }
                    else
                    {
                        transstatus = "22";
                    }

                    Query = "UPDATE [dbo].[CustInvoice_H] SET [TransStatus] = @TransStatus WHERE [Invoice_Id] = @Invoice_Id ";
                    using (Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@TransStatus", transstatus);
                        Cmd.Parameters.AddWithValue("@Invoice_Id", invoiceid);
                        Cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    Query = "SELECT * FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] = @Invoice_Id ";
                    using (Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@Invoice_Id", invoiceid);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            if (Convert.ToDecimal(Dr["Settle_Amount"]) == 0)
                            {
                                transstatus = "03";
                            }
                            else if (Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"]) != 0)
                            {
                                transstatus = "42";
                            }
                            else
                            {
                                transstatus = "41";
                            }
                        }
                        Dr.Close();
                    }

                    Query = "UPDATE [dbo].[CustInvoice_H] SET [TransStatus] = @TransStatus WHERE [Invoice_Id] = @Invoice_Id ";
                    using (Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@TransStatus", transstatus);
                        Cmd.Parameters.AddWithValue("@Invoice_Id", invoiceid);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            Conn.Close();
        }

        private void InsertCLRLog(string Email, string Print, string status, string action)
        {
            bool email = false;
            if (Email.ToUpper().Trim() == "TRUE") { email = true; }
            else { Email = ""; }
            
            bool print = false;
            if (Print.ToUpper().Trim() == "TRUE") { print = true; }
            else { Print = ""; }

            string statusDesc = "";
            if(status != "")
            {
                Query = "SELECT [Deskripsi] FROM [dbo].[TransStatusTable] WHERE [TransCode] = 'Collection' AND [StatusCode]=@StatusCode";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@StatusCode",status);
                    statusDesc = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
                }
            }
            if (action == "" || Print == "" || Email == "")
            {
                Query = "SELECT TOP 1 [Action],[Email],[Print] FROM [dbo].[Collection_LogTable] WHERE [CL_No]=@CL_No ORDER BY LogDate DESC";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@CL_No",txtCLNo.Text);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (action == "")
                        {
                            action = Dr["Action"].ToString();
                        }
                        if (Print == "")
                        {
                            print = Convert.ToBoolean(Dr["Print"]);
                        }
                        if (Email == "")
                        {
                            email = Convert.ToBoolean(Dr["Email"]);
                        }
                    }
                    Dr.Close();
                }
            }
            Query = "INSERT INTO [dbo].[Collection_LogTable] ([CL_No],[CL_Date],[Type],[Email],[Print],[Deskripsi],[Status],[StatusDescription],[Action],[LogDate],[UserID]) VALUES (";
            Query += " @CL_No,@CL_Date,@Type,@Email,@Print,@Deskripsi,@Status,@StatusDescription,@Action,getdate(),@UserID)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CL_No",txtCLNo.Text);
                Cmd.Parameters.AddWithValue("@CL_Date",dtCLDate.Value);
                Cmd.Parameters.AddWithValue("@Type",txtType.Text);
                Cmd.Parameters.AddWithValue("@Email",email);
                Cmd.Parameters.AddWithValue("@Print",print);
                Cmd.Parameters.AddWithValue("@Deskripsi","");
                Cmd.Parameters.AddWithValue("@Status",status);
                Cmd.Parameters.AddWithValue("@StatusDescription", statusDesc);
                Cmd.Parameters.AddWithValue("@Action",action);
                Cmd.Parameters.AddWithValue("@UserID",ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        DataGridViewComboBoxCell cmbPM;
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (cmbInvoicePayment.Text == null || cmbInvoicePayment.Text == "")
            {
                MessageBox.Show("Pilih Invoice.");
                return;
            }
            dgvPayment.Rows.Add();
            for (int i = 0; i < tblPaymentName.Count(); i++)
            {
                //{ "No", "Invoice_Id", "Invoice_Type", "Payment_Method", "Payment_No", "Payment_Date", "Payment_Amount", "Payment_DueDate", "Assigned_Amount", "Notes","SeqNo" };
                if (tblPaymentName[i] == "No")
                {
                    dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells["No"].Value = dgvPayment.Rows.Count;
                }
                else if (tblPaymentName[i] == "Invoice_Id")
                {
                    dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells["Invoice_Id"].Value = cmbInvoicePayment.Text;
                }
                else if (tblPaymentName[i] == "Invoice_Type")
                {
                    dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells["Invoice_Type"].Value = txtInvoiceType.Text;
                }
                else if (tblPaymentName[i] == "SeqNo")
                {
                    int maxAccountLevel = int.MinValue;
                    DataRow[] rows = Payment.Rows.Cast<DataRow>().Where(r => r["Invoice_Id"].Equals(cmbInvoicePayment.Text)).ToArray();
                    foreach (DataRow dr in rows)
                    {
                        var seqno = Convert.ToInt32(dr["SeqNo"]);
                        maxAccountLevel = Math.Max(maxAccountLevel, seqno);
                    }
                    
                    foreach (DataGridViewRow dr in dgvPayment.Rows)
                    {
                        var seqno = Convert.ToInt32(dr.Cells["SeqNo"].Value);
                        maxAccountLevel = Math.Max(maxAccountLevel, seqno);
                    }
                    dgvPayment.Rows[dgvPayment.Rows.Count - 1].Cells["SeqNo"].Value = (maxAccountLevel+1);
                }
            }
            cmbPM = new DataGridViewComboBoxCell();
            using (Cmd = new SqlCommand("select PaymentModeName from PaymentMode ", ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                { 
                    cmbPM.Items.Add(Dr["PaymentModeName"].ToString()); 
                }
                Dr.Close();
            }
            dgvPayment.Rows[(dgvPayment.Rows.Count - 1)].Cells["Payment_Method"] = cmbPM;
        }

        private void Create_dgvInvoice()
        {
            dgvInvoice.Columns.Clear();
            dgvInvoice.ColumnCount = TblInvoiceName.Count();
            for (int i = 0; i < TblInvoiceName.Count(); i++)
            {
                dgvInvoice.Columns[i].Name = TblInvoiceName[i];
                dgvInvoice.Columns[i].HeaderText = TblInvoiceHeaderText[i];
            }
            dgvInvoice.AutoResizeColumns();
        }

        private void Create_dgvPayment()
        {
            dgvPayment.Columns.Clear();
            dgvPayment.ColumnCount = tblPaymentName.Count();
            for (int i = 0; i < tblPaymentName.Count(); i++)
            {
                dgvPayment.Columns[i].Name = tblPaymentName[i];
                dgvPayment.Columns[i].HeaderText = tblPaymentHeaderText[i];
            }
            dgvPayment.AutoResizeColumns();
        }

        private void Create_dgvSO()
        {
            dgvSO.Columns.Clear();
            dgvSO.ColumnCount = tblSOName.Count();
            for (int i = 0; i < tblSOName.Count(); i++)
            {
                dgvSO.Columns[i].Name = tblSOName[i];
                dgvSO.Columns[i].HeaderText = tblSOHeaderText[i];
            }
            dgvSO.AutoResizeColumns();
        }

        private void Create_dgvAttachedFile()
        {
            dgvAttachedFile.Columns.Clear();
            dgvAttachedFile.ColumnCount = tblAttachedFileHeaderText.Count();
            for (int i = 0; i < tblAttachedFileName.Count(); i++)
            {
                dgvAttachedFile.Columns[i].Name = tblAttachedFileName[i];
                dgvAttachedFile.Columns[i].HeaderText = tblAttachedFileHeaderText[i];
            }
            dgvAttachedFile.AutoResizeColumns();
        }

        private void StorePaymentDataTable()
        {
            if (Payment.Columns.Count == 0)
            {
                for (int x = 0; x < tblPaymentName.Count(); x++)
                {
                    Payment.Columns.Add(tblPaymentName[x]);
                }
            }
            if (Payment.Rows.Count <= 0)
            {
                Query = "SELECT * FROM [Collection_Payment] WHERE [CL_No] = @CL_No";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@CL_No", txtCLNo.Text);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        Payment.Rows.Add();
                        for (int x = 0; x < tblPaymentName.Count(); x++)
                        {
                            if (tblPaymentName[x].Contains("Date"))
                            {
                                Payment.Rows[Payment.Rows.Count - 1][x] = (Convert.ToDateTime(Dr[tblPaymentName[x]])).ToString("dd/MM/yyyy");
                                string tes = (Convert.ToDateTime(Dr[tblPaymentName[x]])).ToString("dd/MM/yyyy");
                            }
                            else if (tblPaymentName[x] != "No")
                            {
                                Payment.Rows[Payment.Rows.Count - 1][x] = Dr[tblPaymentName[x]];
                            }
                        }
                    }
                    Dr.Close();
                }
            }

            if (cmbInvoicePayment.Text != "")
            {
                DataRow[] table = Payment.Rows.Cast<DataRow>().Where(r => r["Invoice_Id"].Equals(cmbInvoicePayment.Text)).ToArray();
                for (int i = 0; i < table.Count(); i++)
                    Payment.Rows.Remove(table[i]);
                for (int i = 0; i < dgvPayment.Rows.Count; i++)
                {
                    Payment.Rows.Add();
                    for (int x = 0; x < tblPaymentName.Count(); x++)
                    {
                        Payment.Rows[Payment.Rows.Count - 1][x] = dgvPayment.Rows[i].Cells[tblPaymentName[x]].Value;
                    }
                }
            }
        }

        private void GridColumnsConfiguration()
        {
            //gridInvoice configuration
            if (Mode == "Edit")
            {
                dgvInvoice.ReadOnly = false;
                for (int i = 0; i < TblInvoiceName.Count(); i++)
                {
                    if (TblInvoiceVisibleFalse.Contains(TblInvoiceName[i]))
                    {
                        dgvInvoice.Columns[i].Visible = false;
                    }
                    if (TblInvoiceReadOnlyFalse.Contains(TblInvoiceName[i]))
                    {
                        dgvInvoice.Columns[i].ReadOnly = false;
                        dgvInvoice.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgvInvoice.Columns[i].ReadOnly = true;
                        dgvInvoice.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
            }
            else
            {
                dgvInvoice.ReadOnly = true;
                for (int i = 0; i < dgvInvoice.Columns.Count; i++)
                {
                    if (TblInvoiceVisibleFalse.Contains(TblInvoiceName[i]))
                    {
                        dgvInvoice.Columns[i].Visible = false;
                    }
                    dgvInvoice.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
            
            //gridAttachedFile configuration
            if (Mode == "Edit")
            {
                dgvAttachedFile.ReadOnly = false;
                for (int i = 0; i < tblAttachedFileName.Count(); i++)
                {
                    if (tblAttachedVisibleFalse.Contains(tblAttachedFileName[i]))
                    {
                        dgvAttachedFile.Columns[i].Visible = false;
                    }
                    if (tblAttachedReadOnlyFalse.Contains(tblAttachedFileName[i]))
                    {
                        dgvAttachedFile.Columns[i].ReadOnly = false;
                        dgvAttachedFile.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgvAttachedFile.Columns[i].ReadOnly = true;
                        dgvAttachedFile.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
            }
            else
            {
                dgvAttachedFile.ReadOnly = true;
                for (int i = 0; i < dgvAttachedFile.Columns.Count; i++)
                {
                    if (tblAttachedVisibleFalse.Contains(tblAttachedFileName[i]))
                    {
                        dgvAttachedFile.Columns[i].Visible = false;
                    }
                    dgvAttachedFile.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            //gridSO configuration
            if (Mode == "Edit")
            {
                for (int i = 0; i < tblSOName.Count(); i++)
                {
                    dgvSO.ReadOnly = false;
                    if (tblSOVisibleFalse.Contains(tblSOName[i]))
                    {
                        dgvSO.Columns[i].Visible = false;
                    }
                    if (tblSOReadOnlyFalse.Contains(tblSOName[i]))
                    {
                        dgvSO.Columns[i].ReadOnly = false;
                        dgvSO.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgvSO.Columns[i].ReadOnly = true;
                        dgvSO.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
            }
            else
            {
                dgvSO.ReadOnly = true;
                for (int i = 0; i < dgvSO.Columns.Count; i++)
                {
                    if (tblSOVisibleFalse.Contains(tblSOName[i]))
                    {
                        dgvSO.Columns[i].Visible = false;
                    }
                    dgvSO.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            //gridPayment configuration
            if (Mode == "Edit")
            {
                dgvPayment.ReadOnly = false;
                for (int i = 0; i < tblPaymentName.Count(); i++)
                {
                    if (tblPaymentVisibleFalse.Contains(tblPaymentName[i]))
                    {
                        dgvPayment.Columns[i].Visible = false;
                    }
                    if (tblPaymentReadOnlyFalse.Contains(tblPaymentName[i]))
                    {
                        dgvPayment.Columns[i].ReadOnly = false;
                        dgvPayment.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgvPayment.Columns[i].ReadOnly = true;
                        dgvPayment.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
            }
            else
            {
                dgvPayment.ReadOnly = true;
                for (int i = 0; i < dgvPayment.Columns.Count; i++)
                {
                    if (tblPaymentVisibleFalse.Contains(tblPaymentName[i]))
                    {
                        dgvPayment.Columns[i].Visible = false;
                    }
                    dgvPayment.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPayment.RowCount > 0)
            {
                string DeletedInvoiceId = dgvPayment.Rows[dgvPayment.CurrentRow.Index].Cells["Invoice_Id"].Value.ToString();
                string DeletedSeqNo = dgvPayment.Rows[dgvPayment.CurrentRow.Index].Cells["SeqNo"].Value.ToString();
                DataRow row = Payment.Rows.Cast<DataRow>().Where(r => r["SeqNo"].Equals(DeletedSeqNo) && r["Invoice_Id"].Equals(DeletedInvoiceId)).First();
                Payment.Rows.Remove(row);
                dgvPayment.Rows.RemoveAt(dgvPayment.CurrentRow.Index);
                
                for (int i = 0; i < dgvPayment.RowCount; i++)
                {
                    dgvPayment.Rows[i].Cells["No"].Value = i + 1;
                }
            }
            else
            {
                MessageBox.Show("Maaf tidak ada data yg dihapus.");
                return;
            }
        }

        private void dgvPayment_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvPayment.Columns[e.ColumnIndex].Name.ToString() == "Payment_Date" || dgvPayment.Columns[e.ColumnIndex].Name.ToString() == "Payment_DueDate")
            {
                dtp.Location = dgvPayment.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                dtp.Visible = true;
            }
        }

        private void dgvPayment_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPayment.Columns[e.ColumnIndex].Name.ToString() == "Payment_Date" || dgvPayment.Columns[e.ColumnIndex].Name.ToString() == "Payment_DueDate")
            {
                if (dgvPayment.CurrentCell.Value != "" && dgvPayment.CurrentCell.Value != null)
                {
                    dgvPayment.CurrentCell.Value = dtp.Value.Date.ToString("dd/MM/yyyy");
                    dtp.Visible = false;
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
            else if(dgvPayment.Columns[e.ColumnIndex].Name.ToString() == "Payment_Amount" || dgvPayment.Columns[e.ColumnIndex].Name.ToString() == "Assigned_Amount"  )
            {
                if (dgvPayment.CurrentCell.Value != "" && dgvPayment.CurrentCell.Value != null)
                {
                    string value = String.Format("{0:0.00}", ((Convert.ToDecimal(dgvPayment.CurrentCell.Value))));
                    dgvPayment.CurrentCell.Value = value;
                }
                else
                {
                    dgvPayment.CurrentCell.Value = "0.00";
                }
            }
            StorePaymentDataTable();
        }

        private void btnDeleteFiles_Click(object sender, EventArgs e)
        {
            if (dgvAttachedFile.RowCount > 0)
            {
                if (dgvAttachedFile.CurrentRow.Index > -1)
                {
                    test.RemoveAt(dgvAttachedFile.CurrentRow.Index);
                    dgvAttachedFile.Rows.RemoveAt(dgvAttachedFile.CurrentRow.Index);
                }
            }
            else
            {
                MessageBox.Show("Silahkan Pilih Data Untuk Dihapus");
                return;
            }
        }

        private void btnNewFiles_Click(object sender, EventArgs e)
        {
            if (cmbInvoicePayment.Items.Count == 0)
            {
                MessageBox.Show("Tidak ada invoice dengan result tertagih.");
                return;
            }
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            choofdlog.FilterIndex = 4;
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
                    //string[] tblAttachedFileName = new string[] { "FileType", "ReffTransID", "Customer", "fileName", "ContentType", "fileSize", "id" };
                    this.dgvAttachedFile.Rows.Add(System.Text.Encoding.UTF8.GetString(data), "","", FileName[i], Extension[i], filesize.ToString());
                    test.Add(data);


                    DataGridViewComboBoxCell cmb = new DataGridViewComboBoxCell();
                    for (int x = 0; x < cmbInvoicePayment.Items.Count; x++)
                    {
                        cmb.Items.Add(cmbInvoicePayment.Items[x]);
                    }
                    dgvAttachedFile.Rows[(dgvAttachedFile.Rows.Count - 1)].Cells["ReffTransID"] = cmb;

                    DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                    cell.Items.Add("Document Nota");
                    cell.Items.Add("Cheque");
                    dgvAttachedFile.Rows[(dgvAttachedFile.Rows.Count - 1)].Cells["FileType"] = cell;
                    i++;
                }
            }
            dgvAttachedFile.AutoResizeColumns();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvAttachedFile.RowCount > 0)
            {
                String fileid = dgvAttachedFile.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachedFile.CurrentRow.Cells["Id"].Value.ToString();
                String fileName = dgvAttachedFile.CurrentRow.Cells["File_Name"].Value == null ? "" : dgvAttachedFile.CurrentRow.Cells["File_Name"].Value.ToString();
                String ContentType = dgvAttachedFile.CurrentRow.Cells["Content_Type"].Value == null ? "" : dgvAttachedFile.CurrentRow.Cells["Content_Type"].Value.ToString();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ContentType;
                sfd.FileName = fileName + "." + ContentType;
                sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
                sfd.AddExtension = true;

                if (ContentType.ToUpper() == "PDF")
                {
                    sfd.FilterIndex = 1;
                }
                else if (ContentType.ToUpper() == "TXT")
                {
                    sfd.FilterIndex = 2;
                }
                else if (ContentType.ToUpper() == "BMP" || ContentType.ToUpper() == "JPG" || ContentType.ToUpper() == "GIF" || ContentType.ToUpper() == "PNG")
                {
                    sfd.FilterIndex = 3;
                }
                else
                {
                    sfd.FilterIndex = 4;
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

        private void dgvInvoice_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInvoice.Columns[e.ColumnIndex].Name != "Result" && (e.ColumnIndex >= 0) && (e.RowIndex >= 0))
            {
                if (dgvInvoice.Rows[e.RowIndex].Cells["Result"].Value != null && dgvInvoice.Rows[e.RowIndex].Cells["Result"].Value.ToString() == "Tertagih")
                {
                    Invoice_dgv = dgvInvoice.Rows[e.RowIndex].Cells["Invoice_Id"].Value.ToString();
                    PopulateGridSO(Invoice_dgv);
                    populateGridPayment();
                    TabCLR.SelectedIndex = 1;
                }
                else if (dgvInvoice.Rows[e.RowIndex].Cells["Result"].Value != null && dgvInvoice.Rows[e.RowIndex].Cells["Result"].Value.ToString() == "Tidak Tertagih")
                {
                    MessageBox.Show("Untuk Invoice yang Tidak Tertagih Tidak perlu mengisi pada Tab Payment,\ntapi kolom Coment harus diisi.");
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        private void TabCLR_Click(object sender, EventArgs e)
        {
            //if (TabCLR.SelectedIndex == 0)
            //{
            //    for (int i = 0; i < dgvPayment.RowCount; i++)
            //    {
            //        if (dgvPayment.Rows[i].Cells["Payment_Methode"].Value.ToString() == "TRANSFER" || dgvPayment.Rows[i].Cells["Payment_Methode"].Value.ToString() == "CHEQUE")
            //        {
            //            for (int k = 0; k < dgvAttachedFile.RowCount; k++)
            //            {
            //                if (dgvAttachedFile.Rows[k].Cells["Reference_No"].Value != Invoice_dgv)
            //                {
            //                    MessageBox.Show("Maaf no invoice:" + Invoice_dgv + " Belum upload bukti pembayaran");
            //                    return;
            //                }
            //            }
            //        }
            //    }

            //    dgvPayment.Rows.Clear();
            //    dgvSO.Rows.Clear();
            //}
        }
        
        private void dgvInvoice_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvPayment_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            if ((dgvPayment.CurrentCell.ColumnIndex == dgvPayment.Columns["Payment_Amount"].Index) || (dgvPayment.CurrentCell.ColumnIndex == dgvPayment.Columns["Assigned_Amount"].Index))
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }
        }

        private void Column_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void dgvSO_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            if (dgvSO.CurrentCell.ColumnIndex == dgvSO.Columns["Sign_Amount"].Index)
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PopulateCmbInvoicePayment();
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = "[dbo].[CustInvoice_H]";
            F.QuerySearch = "SELECT a.*,b.Deskripsi FROM [dbo].[CustInvoice_H] a LEFT JOIN TransStatusTable b ON a.TransStatus = b.StatusCode WHERE a.AR_Amount - a.Settle_Amount != 0 AND b.TransCode = 'CustInvoice' AND a.TransStatus IN ('03','13','22') ";
            for (int i = 0; i < dgvInvoice.Rows.Count; i++)
            {
                F.QuerySearch += " AND a.Invoice_Id != '" + dgvInvoice.Rows[i].Cells["Invoice_Id"].Value.ToString() + "' ";
            }
            F.FilterText = new string[] { "[Cust_Id]", "[Cust_Name]", "Invoice_Id", "Invoice_Date", "Invoice_DueDate", "Invoice_Amount", "Kwitansi_No", "Deskripsi" };
            F.Select = new string[] { "Cust_Name", "Invoice_Id", "Invoice_Date", "Invoice_Amount", "Invoice_DueDate", "AR_Amount", "Settle_Amount", "Kwitansi_No", "Invoice_Type", "Cust_Id" };
            F.PrimaryKey = "Invoice_Id";
            F.HideField = new string[] { };
            F.Parent = this;
            F.ShowDialog();
            if (Variable.Kode2 != null)
            {
                for (int i = 0; i <= ((Variable.Kode2.GetUpperBound(0))); i++)
                {
                    //{ "No","CustId", "CustName", "Invoice_Id", "SeqNo", "Kwitansi_No", "Invoice_Date", "Invoice_Amount", "Invoice_DueDate", "Result", "Payment_Amount", "Comments", "Invoice_Type" };
                    int seqno = 0;
                    for (int x = 0; x < dgvInvoice.Rows.Count; x++)
                    {
                        if (seqno < Convert.ToInt32(dgvInvoice.Rows[x].Cells["SeqNo"].Value))
                        {
                            seqno = Convert.ToInt32(dgvInvoice.Rows[x].Cells["SeqNo"].Value);
                        }
                    }
                    dgvInvoice.Rows.Add(dgvInvoice.Rows.Count + 1, Variable.Kode2[i, 9], Variable.Kode2[i, 0], Variable.Kode2[i, 1], (seqno + 1), Variable.Kode2[i, 7], Convert.ToDateTime(Variable.Kode2[i, 2]).ToShortDateString(), Variable.Kode2[i, 3], Convert.ToDateTime(Variable.Kode2[i, 4]).ToShortDateString(), "Result", (Convert.ToDecimal(Variable.Kode2[i, 5]) - Convert.ToDecimal(Variable.Kode2[i, 6])), "", Variable.Kode2[i, 8]);
                    DataGridViewComboBoxCell CmbResult = new DataGridViewComboBoxCell();
                    CmbResult.Items.Add("Tertagih");
                    CmbResult.Items.Add("Tidak Tertagih");
                    dgvInvoice.Rows[(dgvInvoice.Rows.Count - 1)].Cells["Result"] = CmbResult;
                }
            }
            Variable.Kode2 = null;
        }

        private void btnDeleteGrid1_Click(object sender, EventArgs e)
        {
            if (dgvInvoice.Rows.Count > 0)
            {
                DeletedInvoice.Add(dgvInvoice.Rows[dgvInvoice.CurrentRow.Index].Cells["Invoice_Id"].Value.ToString());
                dgvInvoice.Rows.RemoveAt(dgvInvoice.CurrentRow.Index);
                PopulateCmbInvoicePayment();
                dgvPayment.Rows.Clear();
            }
        }

        private void dgvInvoice_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            PopulateCmbInvoicePayment();
            if (dgvInvoice.Rows[e.RowIndex].Cells["Result"].Value.ToString() == "Tidak Tertagih")
            {
                dgvPayment.Rows.Clear();
                txtCustomer.Text = "";
                txtInvoiceType.Text = "";
                DataRow[] table = Payment.Rows.Cast<DataRow>().Where(r => r["Invoice_Id"].Equals(dgvInvoice.Rows[e.RowIndex].Cells["Invoice_Id"].Value.ToString())).ToArray();
                for (int i = 0; i < table.Count(); i++)
                    Payment.Rows.Remove(table[i]);
            }
        }

        private void cmbInvoicePayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateGridSO(cmbInvoicePayment.Text);
            populateGridPayment();
        }

        private void dgvPayment_Leave(object sender, EventArgs e)
        {
            StorePaymentDataTable();
        }

        private void cmbInvoicePayment_MouseClick(object sender, MouseEventArgs e)
        {
            if (cmbInvoicePayment.Items.Count <= 0)
            {
                MessageBox.Show("Tagihan pada tab Invoice belom ada yang result tertagih.");
            }
        }
    }
}
