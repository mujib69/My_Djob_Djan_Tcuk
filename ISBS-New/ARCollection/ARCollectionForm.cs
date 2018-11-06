using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Transactions;
using System.Data.SqlClient;

namespace ISBS_New.ARCollection
{
    public partial class ARCollectionForm : MetroFramework.Forms.MetroForm
    {
        SqlConnection Conn;
        SqlCommand Cmd;
        SqlDataReader Dr;
        TransactionScope scope;
        ARCollection.ARCollectionInquiry F = new ARCollectionInquiry();

        string Query;
        string Mode;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public ARCollectionForm(string mode)
        {
            InitializeComponent();
            Mode = mode;
        }

        public ARCollectionForm(string Mode,List<string> PassedData)
        {
            InitializeComponent();
        }

        public void Parent(ARCollectionInquiry F)
        {
            F = F;
        }

        private void ModeView()
        {
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnEdit.Enabled = true;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnSearchCust.Enabled = false;
            btnPaymentMethod.Enabled = false;

            cmbInvoiceType.Enabled = false;
            dtDueDate.Enabled = false;
            txtCollectionId.ReadOnly = true;
            txtInvoiceAmount.ReadOnly = true;
            txtNotes.ReadOnly = true;
        }

        private void ModeNew()
        {
            btnSave.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnEdit.Enabled = false;
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            btnSearchCust.Enabled = true;
            btnPaymentMethod.Enabled = true;

            cmbInvoiceType.Enabled = true;
            dtDueDate.Enabled = true;
            txtCollectionId.ReadOnly = false;
            txtInvoiceAmount.ReadOnly = false;
            txtNotes.ReadOnly = false;

            btnPrint.Visible = false;
            btnEmail.Visible = false;
            btnCancel.Visible = false;
            btnEdit.Visible = false;
        }

        private void ModeEdit()
        {
            btnSave.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnEdit.Enabled = false;
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            btnSearchCust.Enabled = true;
            btnPaymentMethod.Enabled = true;

            cmbInvoiceType.Enabled = true;
            dtDueDate.Enabled = true;
            txtCollectionId.ReadOnly = false;
            txtInvoiceAmount.ReadOnly = false;
            txtNotes.ReadOnly = false;
        }

        private void ModeInvoiced()
        {
            btnEmail.Visible = false;
            btnPrint.Visible = false;
            btnAdd.Visible = false;
            btnDelete.Visible = false;
            btnEdit.Visible = false;
            btnSave.Visible = false;
            btnCancel.Visible = false;

            txtCollectionId.Visible = false;
            lblCollectionId.Visible = false;
        }

        private void ModeEmail()
        {

        }

        private void ModeManual()
        {

        }

        private void metroTabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btnSearchCust_Click(object sender, EventArgs e)
        {
            Variable.Kode2 = null;
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = "[dbo].[CustTable]";
            F.QuerySearch = " SELECT [CustId],[CustName], [Gol_Prsh] FROM [dbo].[CustTable] ";
            F.FilterText = new string[] { "CustName", "Gol_Prsh" };
            F.Select = new string[] { "CustId", "CustName" };
            F.PrimaryKey = "CustId";
            F.HideField = new string[] {"Check"};
            F.Parent = this;
            F.ShowDialog();
            dataGridView1.Rows.Clear();
            if (Variable.Kode2 != null)
            {
                txtCustId.Text = Variable.Kode2[0, 0];
                txtCustName.Text = Variable.Kode2[0, 1];
            }
            Variable.Kode2 = null;
        }

        private void ARCollectionForm_Load(object sender, EventArgs e)
        {
            invoiceTypeAdd();
            if (Mode == "New")
            {
                ModeNew();
            }
            else
            {
                ModeView();
            }
        }

        private void invoiceTypeAdd()
        {
            cmbInvoiceType.Items.Add("Manual");
            cmbInvoiceType.Items.Add("Email / Mail");
        }

        private void btnPaymentMethod_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "PaymentMode";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
                txtPaymentMethod.Text = ConnectionString.Kode2;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtCustId.Text == null || txtCustId.Text.Trim() == "")
            {
                MessageBox.Show("Pilih Customer terlebih dahulu.");
                return;
            }

            Variable.Kode2 = null;
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = "[dbo].[CustInvoice_H]";
            F.QuerySearch = " SELECT b.Invoice_Id,b.Cust_Name,a.Invoice_Amount,b.Invoice_Date,b.Notes,b.Payment_Method,SO_Id,a.CreatedBy,a.CreatedDate,a.UpdatedBy,a.UpdatedDate FROM [dbo].[CustInvoice_Dtl] a LEFT JOIN [dbo].[CustInvoice_H] b ON a.[InvoiceId]=b.[Invoice_Id] WHERE b.TransStatus = '03' AND b.[Cust_Name] = '" + txtCustName.Text + "' AND b.[Cust_Id] = '"+txtCustId.Text+"'";
            F.FilterText = new string[] { "Invoice_Id", "Cust_Name", "Invoice_Amount", "Invoice_Date", "Payment_Method", "SO_Id", "CreatedBy", "CreatedDate", "UpdatedBy", "UpdatedDate" };
            F.Select = new string[] { "Invoice_Id", "Cust_Name", "Invoice_Amount", "Invoice_Date", "Payment_Method", "SO_Id" };
            F.PrimaryKey = "Invoice_Id";
            F.Parent = this;
            F.ShowDialog();
            dataGridView1.Rows.Clear();
            if (Variable.Kode2 != null)
            {
                if (dataGridView1.Columns.Count <= 0)
                {
                    dataGridView1.Columns[0].Name = "Invoice Id";
                    dataGridView1.Columns[1].Name = "Customer Name";
                    dataGridView1.Columns[2].Name = "Invoice Amount";
                    dataGridView1.Columns[3].Name = "Invoice Date";
                    dataGridView1.Columns[4].Name = "Payment Method";
                    dataGridView1.Columns[5].Name = "SO Id";
                }
                for (int i = 0; i <= ((Variable.Kode2.GetUpperBound(0))); i++)
                {
                    dataGridView1.Rows.Add(Variable.Kode2[i, 0], Variable.Kode2[i, 1], Variable.Kode2[i, 2], Variable.Kode2[i, 3], Variable.Kode2[i, 4], Variable.Kode2[i, 5], Variable.Kode2[i, 6]);
                }
            }
            Variable.Kode2 = null;

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                int index = dataGridView1.CurrentRow.Index;
                dataGridView1.Rows.RemoveAt(index);
            }
        }

        private bool validation()
        {
            if (dataGridView1.Rows.Count <= 0)
            {
                MessageBox.Show("Tidak ada Invoice pada data grid view");
                return false;
            }
            else if (txtPaymentMethod.Text == null || txtPaymentMethod.Text.Trim() == "")
            {
                MessageBox.Show("Pilihan Payment Method masih kosong.");
                return false;
            }
            else if (txtInvoiceAmount.Text == null || txtInvoiceAmount.Text.Trim() == "")
            {
                MessageBox.Show("Pilihan Invoice Amount masih kosong.");
                return false;
            }
            else if (txtCollectionId.Text == null || txtCollectionId.Text.Trim() == "")
            {
                MessageBox.Show("Pilihan Collection Id masih kosong.");
                return false;
            }
            else if (cmbInvoiceType.Text == null)
            {
                MessageBox.Show("Pilih Invoice Type terlebih dahulu.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (validation() == false)
            {
                return;
            }
            try
            {
                using (scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();

                    //

                    Conn.Close();
                }
                scope.Complete();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeView();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            F.refresh2();
        }
    }
}
