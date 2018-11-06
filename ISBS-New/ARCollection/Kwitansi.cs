using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;

namespace ISBS_New.ARCollection
{
    public partial class Kwitansi : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        int Index;
        string Mode, Query = null;
        string InvoiceId, Notification;
        //tia edit
        ContextMenu vendid = new ContextMenu();
        //tia edit end
        public Kwitansi()
        {
            InitializeComponent();
        }

        //public void flag(string tmpKWNumber)
        //{
        //    parsedKWNo = tmpKWNumber;
        //}

        private void Kwitansi_Load(object sender, EventArgs e)
        {
            txtCustId.ReadOnly = true;
            txtCustName.ReadOnly = true;
            txtKwitansiId.ReadOnly = true;

            //tia edit
            txtCustId.Enabled = true;
            txtCustName.Enabled = true;
            txtCustId.ContextMenu = vendid;
            txtCustName.ContextMenu = vendid;
            //tia edit end
        }

        public void ModeNew()
        {
            Mode = "New";
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            dtKwitansiDate.Enabled = true;
            btnAddAR.Enabled = true;
            btnDeleteAR.Enabled = true;
            btnSearchCustomer.Enabled = true;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            dtKwitansiDate.Enabled = false;
            btnAddAR.Enabled = false;
            btnDeleteAR.Enabled = false;
            btnSearchCustomer.Enabled = false;
        }

        public void ModeEdit()
        {
            Mode = "Edit";
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnCancel.Enabled = true;
            btnExit.Enabled = false;
            dtKwitansiDate.Enabled = true;
            btnAddAR.Enabled = true;
            btnDeleteAR.Enabled = true;
            btnSearchCustomer.Enabled = true;
        }

        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            if (dgvDetail.RowCount == 0)
            {
                SearchCustomer();
            }
            else
            {
                DialogResult dr = MessageBox.Show("Apakah Anda Ingin Mengganti Customer?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    SearchCustomer();
                }
            }
        }

        private void SearchCustomer()
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "CustId";
            tmpSearch.Order = "CustId Asc";
            tmpSearch.Table = "[dbo].[CustTable]";
            tmpSearch.QuerySearch = "SELECT CustId, CustName FROM [dbo].[CustTable]";
            tmpSearch.FilterText = new string[] { "CustId", "CustName"};
            tmpSearch.Mask = new string[] { "Customer Id", "Customer Name" };
            tmpSearch.Select = new string[] { "CustId", "CustName"};
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                if (txtCustId.Text != ConnectionString.Kodes[0])
                {
                    dgvDetail.Rows.Clear();
                }
                txtCustId.Text = ConnectionString.Kodes[0];
                txtCustName.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }
        }

        private void btnAddAR_Click(object sender, EventArgs e)
        {
            if (txtCustId.Text.Trim() != "" && txtCustName.Text != null)
            {
                string Table = "[dbo].[CustInvoice_H]";
                string QuerySearch = "SELECT Invoice_Id, Invoice_Date, Invoice_Amount, Invoice_DueDate  FROM [dbo].[CustInvoice_H] c ";
                QuerySearch += "LEFT JOIN [Kwitansi] k ON c.Kwitansi_No = k.Kwt_Id ";
                QuerySearch += "WHERE c.[Cust_Id] = '" + txtCustId.Text + "' AND ";
                for (int i = 0; i < dgvDetail.Rows.Count; i++)
                {
                    QuerySearch += " Invoice_Id != '" + dgvDetail.Rows[i].Cells["Invoice_Id"].Value.ToString() + "' AND ";
                }
                if (Mode == "New")
                {
                    QuerySearch += " (Kwitansi_No = '' OR (Kwitansi_No != '' AND StatusCode != '01')) ";
                }
                if (Mode == "Edit")
                {
                    QuerySearch += " ((Kwitansi_No = '' OR StatusCode != '01') OR Kwitansi_No = '" + txtKwitansiId.Text + "') ";
                }
                //QuerySearch += " AND Invoice_Amount > 0 AND ";
                QuerySearch += " AND (Invoice_Amount - Settle_Amount) <> 0 AND ";
                QuerySearch += " 1=1 ";

                string[] FilterText = { "Invoice_Id", "Invoice_Date", "Invoice_Amount", "Invoice_DueDate" };
                string[] Mask = { "Invoice Id", "Invoice Date", "Invoice Amount", "Invoice DueDate" };
                string[] Select = { "Invoice_Id", "Invoice_Date", "Invoice_Amount", "Invoice_DueDate" };
                string PrimaryKey = "Invoice_Id";
                string[] HideField = { };
                callSearchQueryV2Form(Table, QuerySearch, FilterText,Mask, Select, PrimaryKey, HideField);
            }
            else
            {
                MessageBox.Show("Customer Id belum terpilih!");
            }
        }

        private void callSearchQueryV2Form(string Table, string QuerySearch, string[] FilterText, string[] Mask, string[] Select, string PrimaryKey, string[] HideField)
        {
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = Table;
            F.QuerySearch = QuerySearch;
            F.FilterText = FilterText;
            F.Mask = Mask;
            F.Select = Select;
            F.PrimaryKey = PrimaryKey;
            F.HideField = HideField;
            F.Parent = this;
            F.ShowDialog();

            populateAfterSearch(Table);
        }

        private void populateAfterSearch(string Table)
        {
            if (Variable.Kode2 == null)
            {
                return;
            }
            if (Table == "[dbo].[CustInvoice_H]")
            {
                using (Method C = new Method())
                {
                    if (dgvDetail.Rows.Count <= 0)
                    {
                        dgvDetail.ColumnCount = 4;
                        dgvDetail.Columns[0].Name = "No";
                        dgvDetail.Columns[1].Name = "Invoice_Id";
                        dgvDetail.Columns[2].Name = "Invoice_Date";
                        dgvDetail.Columns[3].Name = "Invoice_DueDate";
                    }

                    for (int i = 0; i <= ((Variable.Kode2.GetUpperBound(0))); i++)
                    {
                        dgvDetail.Rows.Add("", Variable.Kode2[i, 0], Variable.Kode2[i, 1], Variable.Kode2[i, 2], Variable.Kode2[i, 3]);
                    }

                    dgvDetail.ReadOnly = false;
                    string[] read = new string[] { "No", "Invoice_Id", "Invoice_Date", "Invoice_DueDate" };
                    for (int i = 0; i < read.Length; i++)
                    {
                        dgvDetail.Columns[read[i]].ReadOnly = true;
                    }

                    dgvDetail.AutoResizeColumns();
                    Variable.Kode2 = null;
                }
            }
            ReorderGridNo();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            //Query = "SELECT [Invoice_Id],[Invoice_Date],[Invoice_Amount] FROM [dbo].[CustInvoice_H] WHERE [Kwitansi_No] = '" + txtKwitansiId.Text + "'";
            //Da = new SqlDataAdapter(Query, Conn);
            //dgvDetail.Columns.Clear();
            //DataTable namatable = new DataTable();
            //namatable.Columns.Add("No", typeof(int));
            //Da.Fill(namatable);
            //dgvDetail.DataSource = namatable;
            //dgvDetail.AllowUserToAddRows = false;
            //dgvDetail.AutoResizeColumns();

            //INPUT SO RESERVE
            dgvDetail.Rows.Clear();
            if (dgvDetail.Rows.Count <= 0)
            {
                dgvDetail.ColumnCount = 4;
                dgvDetail.Columns[0].Name = "No";
                dgvDetail.Columns[1].Name = "Invoice_Id";
                dgvDetail.Columns[2].Name = "Invoice_Date";
                dgvDetail.Columns[3].Name = "Invoice_DueDate";
            }

            Query = "SELECT [Invoice_Id],[Invoice_Date],[Invoice_Amount] FROM [dbo].[CustInvoice_H] WHERE [Kwitansi_No] = '" + txtKwitansiId.Text + "'";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    this.dgvDetail.Rows.Add("", Dr["Invoice_Id"], Dr["Invoice_Date"], Dr["Invoice_Amount"]);
                }
                Dr.Close();
            }

            ReorderGridNo();
            Conn.Close();
        }

        private void ReorderGridNo()
        {
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                dgvDetail.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if (validate() == false)
            {
                return;
            }

            if (CheckKWPerInvoice() == false)
            {
                return;
            }

            try
            {
                if (Mode == "New" && txtKwitansiId.Text == "")
                {
                    string Jenis = "TT", Kode = "TT";
                    string KwitansiId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    
                    //query insert kwitansi per invoice
                    for (int i = 0; i < dgvDetail.Rows.Count; i++)
                    {
                        InvoiceId = dgvDetail.Rows[i].Cells["Invoice_Id"].Value.ToString();
                        Query = "UPDATE [dbo].[CustInvoice_H] SET [Kwitansi_No] = '" + KwitansiId + "' WHERE [Invoice_Id] = '" + InvoiceId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    
                    //query insert to kwitansi database
                    Query = "INSERT INTO [dbo].[Kwitansi] ([Kwt_Date],[Kwt_Id],[Cust_Id],[CreatedDate],[CreatedBy],[StatusCode]) ";
                    Query += "VALUES ('" + dtKwitansiDate.Value + "','" + KwitansiId + "','" + txtCustId.Text + "',getdate(),'" + ControlMgr.UserId + "','01')";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    txtKwitansiId.Text = KwitansiId;
                    Notification = "Tanda Terima " + KwitansiId + " Berhasil Dibuat";                   
                }
                if (Mode == "Edit" && txtKwitansiId.Text != "")
                {
                    //kosongin KwitansiNo di setiap AR
                    Query = "UPDATE [dbo].[CustInvoice_H] SET [Kwitansi_No] = '' WHERE [Kwitansi_No] = '" + txtKwitansiId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    //query insert kwitansi per invoice
                    for (int i = 0; i < dgvDetail.Rows.Count; i++)
                    {
                        InvoiceId = dgvDetail.Rows[i].Cells["Invoice_Id"].Value.ToString();
                        Query = "UPDATE [dbo].[CustInvoice_H] SET [Kwitansi_No] = '" + txtKwitansiId.Text + "' WHERE [Invoice_Id] = '" + InvoiceId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }

                    //query update to kwitansi database
                    Query = "UPDATE [dbo].[Kwitansi] SET [Kwt_Date] = '" + dtKwitansiDate.Value + "',[Cust_Id] = '" + txtCustId.Text + "',[UpdatedDate]=getdate(),[UpdatedBy]='" + ControlMgr.UserId + "' WHERE [Kwt_Id] = '" + txtKwitansiId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    Notification = "Tanda Terima " + txtKwitansiId.Text + " Berhasil DiUpdate";
                }
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Conn.Close();
                ModeBeforeEdit();
                if (Notification != "")
                {
                    MessageBox.Show(Notification);
                }
            }                       
        }
        
        private bool validate()
        {
            if (txtCustId.Text == "" || txtCustName.Text == "")
            {
                MessageBox.Show("Customer Id dan Customer Name tidak boleh kosong");
                return false;
            }

            else if (dgvDetail.Rows.Count == 0)
            {
                MessageBox.Show("Invoice tidak boleh kosong");
                return false;
            }

            else if (dtKwitansiDate.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Kwitansi date tidak boleh lebih kecil dari hari ini");
                return false;
            }     
            else
            {
                return true;
            }
        }

        private bool CheckKWPerInvoice()
        {          
            //re-check per-invoice uda ada KW ny ato belum
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                InvoiceId = dgvDetail.Rows[i].Cells["Invoice_Id"].Value.ToString();
                Query = "SELECT [Kwitansi_No] FROM [dbo].[CustInvoice_H] c ";
                Query += "LEFT JOIN [Kwitansi] k ON k.Kwt_Id = c.Kwitansi_No ";
                Query += "WHERE [Invoice_Id] = '" + InvoiceId + "' AND StatusCode = '01' ";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Mode == "New")
                        {
                            if (Dr["Kwitansi_No"].ToString() != "")
                            {
                                MessageBox.Show("Invoice " + InvoiceId + " sudah dibuat Tanda Terima");
                                return false;
                            }
                        }
                        if (Mode == "Edit")
                        {
                            if (Dr["Kwitansi_No"].ToString() == "")
                            {
                                return true;
                            }
                            if (Dr["Kwitansi_No"].ToString() == txtKwitansiId.Text)
                            {
                                return true;
                            }
                            else
                            {
                                MessageBox.Show("Invoice " + InvoiceId + " sudah dibuat Tanda Terima");
                                return false;
                            }
                        }
                    }
                    Dr.Close();
                }
            }
            return true;
        }            
       
        private void btnDeleteAR_Click(object sender, EventArgs e)
        {
            Index = dgvDetail.CurrentRow.Index;
            DialogResult dr = MessageBox.Show("Apakah Anda Ingin Menghapus Invoice" + dgvDetail.Rows[Index].Cells["Invoice_Id"].Value.ToString() + "?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (dgvDetail.RowCount > 0)
                {                 
                    dgvDetail.Rows.RemoveAt(Index);
                    for (int i = 0; i < dgvDetail.RowCount; i++)
                    {
                        dgvDetail.Rows[i].Cells["No"].Value = i + 1;
                    }
                }
            }
            ReorderGridNo();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select [StatusCode] from [Kwitansi] Where [Kwt_Id] = '"+txtKwitansiId.Text+"'";
            Cmd = new SqlCommand(Query, Conn);
            var tmpStatusCode = Cmd.ExecuteScalar();

            if (tmpStatusCode.ToString() == "01")
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show("Tanda Terima tidak bisa di-edit karena sudah diproses");
                return;
            }
            Conn.Close();
        }

        public void GetDataHeader()
        {
            Conn = ConnectionString.GetConnection();
            if (txtKwitansiId.Text != "")
            {
                Query = "SELECT [Cust_Id],[CustName],[Kwt_Date] FROM [Kwitansi] k LEFT JOIN [CustTable] c ON k.[Cust_Id] = c.[CustId] WHERE [Kwt_Id] = '" + txtKwitansiId.Text + "'";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        txtCustId.Text = Dr["Cust_Id"].ToString();
                        txtCustName.Text = Dr["CustName"].ToString();
                        dtKwitansiDate.Value = Convert.ToDateTime(Dr["Kwt_Date"].ToString());                       
                    }
                    Dr.Close();
                }
            }
            Conn.Close();    
        }

        public void GetDataHeader(string tmpKWNo)
        {
            txtKwitansiId.Text = tmpKWNo;
            Conn = ConnectionString.GetConnection();
            if (txtKwitansiId.Text != "")
            {
                Query = "SELECT [Cust_Id],[CustName],[Kwt_Date] FROM [Kwitansi] k LEFT JOIN [CustTable] c ON k.[Cust_Id] = c.[CustId] WHERE [Kwt_Id] = '" + txtKwitansiId.Text + "'";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        txtCustId.Text = Dr["Cust_Id"].ToString();
                        txtCustName.Text = Dr["CustName"].ToString();
                        dtKwitansiDate.Value = Convert.ToDateTime(Dr["Kwt_Date"].ToString());
                    }
                    Dr.Close();
                }
            }
            Conn.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if (CheckKWPerInvoice() == false)
            {
                return;
            }
            GetDataHeader();
            RefreshGrid();
            ModeBeforeEdit();
            Conn.Close();
        }          

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //tia edit
        //klik kanan
        PopUp.CustomerID.Customer Cust = null;

        AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice Arin=null;

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

        private void txtCustId_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    txtCustId.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(txtCustId.Text);
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
                    Cust.GetData(txtCustId.Text);
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

        private void dgvDetail_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (Arin == null || Arin.Text == "")
                {
                    if (dgvDetail.Columns[e.ColumnIndex].Name.ToString() == "Invoice_Id")
                    {
                        Arin = new AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice();
                        Arin.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["Invoice_Id"].Value.ToString());
                        Arin.ParentRefreshGrid(this);
                        Arin.Show();
                    }
                }
                else if (CheckOpened(Arin.Name))
                {
                    Arin.WindowState = FormWindowState.Normal;
                    Arin.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["Invoice_Id"].Value.ToString());
                    Arin.ParentRefreshGrid(this);
                    Arin.Show();
                    Arin.Focus();
                }
            }
        }
        //tia edit end
    }
}
