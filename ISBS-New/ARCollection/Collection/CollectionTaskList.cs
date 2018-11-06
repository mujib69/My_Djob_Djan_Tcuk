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

namespace ISBS_New.ARCollection.Collection
{
    public partial class CollectionTaskList : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
        private SqlCommand Cmd;
        //private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        private TransactionScope scope;
        /**********SQL*********/

        private List<string[]> criteria = new List<string[]>();
        int count = 0;

        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "Check","Customer Id", "Customer", "Invoice Date", "Invoice No","Invoice Type", "Payment Due", "Invoice Amount", "CN Amount", "AR Amount", "Outstanding", "Kwitansi No", "Status" };
        string[] tableColsName = new string[] { "Check","Cust_Id", "Cust_Name", "Invoice_Date", "Invoice_Id","Invoice_Type", "Invoice_DueDate", "Invoice_Amount", "CN_Amount", "AR_Amount", "Outstanding", "Kwitansi_No", "TransStatus" };
        string[] tableCols2 = new string[] { "No", "CL No", "CL Date", "Collection Type", "Collector","Mail Address", "Status", "Action", "Comments" };
        string[] tableCols2Name = new string[] { "No", "CL_No", "CL_Date", "CL_Type", "Collector", "Mail_Address", "Status_Code", "Action", "Comments" };
        string[] tableCols3 = new string[] { "No", "File Type", "Reference No", "File Name", "Content Type", "File Size (kb)" };
        /*********datagridview cols name*********/

        /***********Saved Id For Check***********/
        List<string> CheckedId = new List<string>();
        List<string> TransStatus = new List<string>();
        /****************************************/

        /************For Tab ***********************/
        string InvoiceStat = "Invoice On Progress";
        string CollectionStat = "Collection On Progress";
        /*******************************************/

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);  
        }

        public CollectionTaskList()
        {
            InitializeComponent();            
        }

        private void CollectionTaskList_Load(object sender, EventArgs e)
        {
            CmbCriteria();
            metroTabControl1.SelectedIndex = 0;
            btnInvoiceOnProgress.PerformClick();
        }

        private void CmbCriteria()
        {
            if (cmbCrit.Items.Count == 0)
            {
                cmbCrit.Items.Add("All");
                Query = "SELECT [DisplayName] FROM [User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'Collection_H'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        cmbCrit.Items.Add(Dr["DisplayName"].ToString());
                    }
                    Dr.Close();
                }
                cmbCrit.SelectedIndex = 0;
            }
        }

        public void RefreshGrid()
        {
            RefreshGrid1(InvoiceStat);
            RefreshGrid2(CollectionStat);
        }

        //refresh grid in Collection Log tab
        private void RefreshGrid2(string CollectionStatus)
        {
            if (cmbCrit.Text == "" || cmbCrit.Text == null)
            {
                CmbCriteria();
            }
            dataGridView2.Columns.Clear(); dataGridView2.Rows.Clear();
            Conn = ConnectionString.GetConnection();
            dataGridView2.ColumnCount = tableCols2.Length;
            for (int i = 0; i < tableCols2.Length; i++)
            {
                dataGridView2.Columns[i].Name = tableCols2[i];
            }
            Query = "select a.*,b.Deskripsi from Collection_H a LEFT JOIN  [dbo].[TransStatusTable] b ON b.[StatusCode]= a.[Status_Code] where b.[TransCode] = 'Collection' AND Status_Code in ";
            if (CollectionStatus == "Collection Completed")
            {
                Query += "('03')";
            }
            else
            {
                Query += "('01', '02')";
            }
            Query += "AND (";
            if (cmbCrit.Text == "All")
            {
                string QuerySearch = "SELECT [FieldName] FROM [ISBS-NEW4].[User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'Collection_H'";
                using (Cmd = new SqlCommand(QuerySearch, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (!(Dr["FieldName"].ToString().Contains("Date")))
                        {
                            Query += " " + Dr["FieldName"] + " LIKE @txtSearch OR ";
                        }
                    }
                    Dr.Close();
                }
                Query += " 1=2 ) ";
            }
            else if (cmbCrit.Text.Contains("Date"))
            {
                string QuerySearch = " SELECT FieldName FROM [ISBS-NEW4].[User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'Collection_H' AND [DisplayName] = '" + cmbCrit.Text + "'";
                using (Cmd = new SqlCommand(QuerySearch, Conn))
                {
                    Query += " " + Cmd.ExecuteScalar().ToString() + " BETWEEN '" + dtFromLog.Value.Date.ToString("yyyy-MM-dd") + "' AND '" + dtToLog.Value.Date.ToString("yyyy-MM-dd") + "') ";
                }
            }
            else
            {
                string QuerySearch = " SELECT FieldName FROM [ISBS-NEW4].[User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'Collection_H' AND [DisplayName] = '" + cmbCrit.Text + "'";
                using (Cmd = new SqlCommand(QuerySearch, Conn))
                {
                    Query += " " + Cmd.ExecuteScalar().ToString() + " LIKE @txtSearch )";
                }
            }
            Cmd = new SqlCommand(Query, Conn);
            if (!(cmbCrit.Text.Contains("Date")))
            {
                Cmd.Parameters.AddWithValue("@txtSearch", "%" + tbxSearhFilter.Text + "%");
            }
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                dataGridView2.Rows.Add(1);
                for (int i = 0; i < tableCols2.Count(); i++)
                {
                    if (tableCols2[i] == "No")
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["No"].Value = dataGridView2.RowCount;
                    }
                    else if (tableCols2[i].Contains("Date"))
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[tableCols2[i]].Value = Convert.ToDateTime(Dr[tableCols2Name[i]]);
                    }
                    else if (tableCols2[i] == "Status")
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["Status"].Value = Dr["Deskripsi"];
                    }
                    else if (tableCols2[i] == "Comments")
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["Comments"].Value = "";
                    }
                    else if (tableCols2[i] == "Action")
                    {
                        if (Dr["CL_Type"].ToString().ToUpper() == "EMAIL")
                        {
                            if (Convert.ToBoolean(Dr["Email_C"]) == true)
                                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["Action"].Value = "Email Sent";
                            else
                                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["Action"].Value = "Email Not Sent";
                        }
                        else
                        {
                            if (Convert.ToBoolean(Dr["Print_C"]) == true)
                                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["Action"].Value = "Printed";
                            else
                                dataGridView2.Rows[dataGridView2.RowCount - 1].Cells["Action"].Value = "Not Printed";
                        }
                    }
                    else
                    {
                        dataGridView2.Rows[dataGridView2.RowCount - 1].Cells[tableCols2[i]].Value = Dr[tableCols2Name[i]];
                    }
                }
            }
            Dr.Close();
            this.dataGridView2.Sort(this.dataGridView2.Columns["CL Date"], ListSortDirection.Ascending);
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                dataGridView2.Rows[i].Cells["No"].Value = i + 1;
            }

            Conn.Close();
        }

        //refresh grid in invoice tab
        private void RefreshGrid1(string InvoiceStatus)
        {
            //clear grid
            dataGridView1.Columns.Clear(); dataGridView1.Rows.Clear();

            //get where
            string where = " and (";
            if (tbxCustId.Text != "" && tbxCustId.Text != null)
            {
                where += " Cust_Id = @CustId and ";
            }
            if ((tbxInvoiceFrom.Text != "" && tbxInvoiceFrom.Text != null) && (tbxInvoiceTo.Text != "" && tbxInvoiceTo.Text != null))
            {
                where += " [Invoice_Id] BETWEEN @InvoiceFrom AND @InvoiceTo and ";
            }
            if (count == 0)
            {
                count++;
                where += " 1=1 AND ";
            }
            else
            {
                if (dtFrom.Enabled == true && dtTo.Enabled == true)
                {
                    where += " [Invoice_Date] BETWEEN @dtFrom AND @dtTo AND";
                }
                if (dtDueFrom.Enabled == true && dtDueTo.Enabled == true)
                {
                    where += " [Invoice_DueDate] BETWEEN @dtFromDue AND @dtToDue AND";
                }
            }
            where += " 1=1 )";

            //established columns
            dataGridView1.ColumnCount = tableCols.Length;
            for (int i = 0; i < tableCols.Length; i++)
            {
                dataGridView1.Columns[i].Name = tableCols[i];
            }

            //populate row
            Conn = ConnectionString.GetConnection();
            Query = "Select a.*, b.Deskripsi from [CustInvoice_H] a left join TransStatusTable b on a.TransStatus = b.StatusCode where  b.TransCode = 'CustInvoice' and a.TransStatus IN ";
            if (InvoiceStatus == "Invoice Completed")
            {
                Query += "('21','41') and a.AR_Amount - a.Settle_Amount = 0 ";
            }
            else
            {
                //Query += "('03','13','22','42') and a.AR_Amount - a.Settle_Amount != 0 ";
                //untuk paksa membuat receipt voucher setelah sudah di collect sekali, jdi collection partial tidak diberi ijin untuk membuat collection lgi harus reconciled partial 
                Query += "('03','13','42') and a.AR_Amount - a.Settle_Amount != 0 ";
            }
            Query += " " + where + " order by a.Cust_Name, Kwitansi_No asc";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@dtFrom", dtFrom.Value.ToShortDateString());
                Cmd.Parameters.AddWithValue("@dtTo", dtTo.Value.ToShortDateString());
                Cmd.Parameters.AddWithValue("@dtFromDue", dtDueFrom.Value.ToShortDateString());
                Cmd.Parameters.AddWithValue("@dtToDue", dtDueTo.Value.ToShortDateString());
                if (tbxCustId.Text != "" && tbxCustId.Text != null)
                {
                    Cmd.Parameters.AddWithValue("@CustId", tbxCustId.Text);
                }
                if ((tbxInvoiceFrom.Text != "" && tbxInvoiceFrom.Text != null) && (tbxInvoiceTo.Text != "" && tbxInvoiceTo.Text != null))
                {
                    Cmd.Parameters.AddWithValue("@InvoiceFrom", tbxInvoiceFrom.Text);
                    Cmd.Parameters.AddWithValue("@InvoiceTo", tbxInvoiceTo.Text);
                }
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dataGridView1.Rows.Add(1);
                    DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                    if (CheckedId.Contains(Dr["Invoice_Id"].ToString()))
                    {
                        chk.Value = true;
                    }
                    else
                    {
                        chk.Value = false;
                    }

                    for (int i = 0; i < tableCols.Length; i++)
                    {
                        if (tableCols[i] == "Check")
                        {
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Check"] = chk;
                        }
                        else if (tableCols[i].Contains("Date"))
                        {
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Convert.ToDateTime(Dr[tableColsName[i]]);
                        }
                        else if (tableCols[i] == "Outstanding")
                        {
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Outstanding"].Value = Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"]);
                        }
                        else if (tableCols[i] == "Status")
                        {
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Status"].Value = Dr["Deskripsi"];
                        }
                        else
                        {
                            dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[i]].Value = Dr[tableColsName[i]];
                        }
                    }
                }
                Dr.Close();
            }

            //create Send Email and Create Mail Letter Columns
            //stv edit start
            DataGridViewButtonColumn buttonCreateMailLetter = new DataGridViewButtonColumn();
            buttonCreateMailLetter.Name = "Create Mail Letter";
            buttonCreateMailLetter.HeaderText = "Create Mail Letter";
            buttonCreateMailLetter.Text = "Create Mail Letter";
            buttonCreateMailLetter.UseColumnTextForButtonValue = true;

            DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            buttonSend.Name = "Send Email";
            buttonSend.HeaderText = "Send Email";
            buttonSend.Text = "Send Email";
            buttonSend.UseColumnTextForButtonValue = true;

            if (!dataGridView1.Columns.Contains("Create Mail Letter"))
                dataGridView1.Columns.Add(buttonCreateMailLetter);
            if (!dataGridView1.Columns.Contains("Send Email"))
                dataGridView1.Columns.Add(buttonSend);
            //stv edit end

            dataGridView1.ReadOnly = false;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                if (!(dataGridView1.Columns[i].Name == "Check"))
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                }
            }
            Conn.Close();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Amount") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Outstanding"))
            {
                if (e.Value == "" || e.Value == null || e.Value == (object)DBNull.Value)
                {
                    e.Value = "0";
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Due"))
            {
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView2.Columns[e.ColumnIndex].Name.Contains("Date"))
            {
                dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }

        private void CallForm(string CollectionType, List<string> InvoiceId)
        {
            CollectionHeader f = new CollectionHeader(InvoiceId);
            f.tbxCollectionType.Text = CollectionType;
            f.SetMode("New", "");
            f.ShowDialog();
            RefreshAccess();
        }

        private void CallForm(string CollectionType, string CLno, string collector)
        {
            ARCollection.CollectionResult.CLR_Form f = new ARCollection.CollectionResult.CLR_Form();
            string CL_NO = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["CL No"].Value.ToString();
            f.SetMode("BeforeEdit", CL_NO);
            f.setParent2(this);
            f.Show();

            //CollectionHeader f = new CollectionHeader(CLno,collector);
            //f.tbxCollectionType.Text = CollectionType;
            //f.SetMode("BeforeEdit", "");
            //f.ShowDialog();
            RefreshAccess();
        }

        private void btnCollector_Click(object sender, EventArgs e)
        {
            if (Validation() == 'X')
            {
                return;
            }
            List<string> InvoiceId = new List<string>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells["Check"].Value) == true)
                {
                    InvoiceId.Add(dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString());
                }
            }
            CallForm("Manual", InvoiceId);
        }

        private char Validation()
        {
            string message = "";
            char flag = 'X';
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells["Check"].Value) == true)
                {
                    flag = '\0';
                }
            }
            if (flag == 'X')
            {
                message += "-Invoice Belom dipilih.";
            }

            if (message != "")
            {
                MessageBox.Show(message);
            }
            return flag;
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {

        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            //CallForm("Mail");
        }

        private void cbCriteria_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //before clear grid check for checked row and store in CheckedId array
            CheckedId.Clear();
            TransStatus.Clear();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells["Check"].Value) == true)
                {
                    CheckedId.Add(dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString());
                    TransStatus.Add(dataGridView1.Rows[i].Cells["Status"].Value.ToString());
                }
            }

            RefreshGrid1(InvoiceStat);
            
            //retain checked row to be always be present and the top of the list/grid
            for (int i = 0; i < CheckedId.Count; i++)
            {
                int h = 0;
                for (int x = 0; x < dataGridView1.Rows.Count; x++)
                {
                    if (CheckedId[i] == dataGridView1.Rows[x].Cells["Invoice No"].Value.ToString())
                    {
                        dataGridView1.Rows[x].Cells["Check"].Value = true;
                        break;
                    }
                    else
                    {
                        h++;
                    }
                }
                if (h == dataGridView1.Rows.Count)
                {
                    Query = "SELECT a.*, b.Deskripsi FROM [CustInvoice_H] a LEFT JOIN TransStatusTable b ON a.TransStatus = b.StatusCode WHERE a.Invoice_Id = '" + CheckedId[i] + "' AND b.Deskripsi = '" + TransStatus[i] + "' AND b.TransCode = 'CustInvoice'";
                    using (Conn = ConnectionString.GetConnection())
                    using (Cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            dataGridView1.Rows.Add(1);
                            DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                            chk.Value = true;
                            for (int j = 0; j < tableCols.Length; j++)
                            {
                                if (tableCols[j] == "Check")
                                {
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Check"] = chk;
                                }
                                else if (tableCols[j].Contains("Date"))
                                {
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[j]].Value = Convert.ToDateTime(Dr[tableColsName[j]]);
                                }
                                else if (tableCols[j] == "Outstanding")
                                {
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Outstanding"].Value = Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"]);
                                }
                                else if (tableCols[j] == "Status")
                                {
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Status"].Value = Dr["Deskripsi"];
                                }
                                else
                                {
                                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[tableCols[j]].Value = Dr[tableColsName[j]];
                                }
                            }
                        }
                    }
                }
            }
            this.dataGridView1.Sort(this.dataGridView1.Columns["Payment Due"], ListSortDirection.Descending);
            this.dataGridView1.Sort(this.dataGridView1.Columns["Invoice Date"], ListSortDirection.Descending);
            this.dataGridView1.Sort(this.dataGridView1.Columns["Customer"], ListSortDirection.Descending);
            this.dataGridView1.Sort(this.dataGridView1.Columns["Check"], ListSortDirection.Descending);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //before clear grid check for checked row and store in CheckedId array
            CheckedId.Clear();
            TransStatus.Clear();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells["Check"].Value) == true)
                {
                    CheckedId.Add(dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString());
                    TransStatus.Add(dataGridView1.Rows[i].Cells["Status"].Value.ToString());
                }
            }

            tbxCustId.Text = "";
            tbxCustName.Text = "";
            tbxInvoiceFrom.Text = "";
            tbxInvoiceTo.Text = "";
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;
            dtDueFrom.Value = DateTime.Now;
            dtDueTo.Value = DateTime.Now;
            count = 0;
            RefreshGrid();

            this.dataGridView1.Sort(this.dataGridView1.Columns["Payment Due"], ListSortDirection.Descending);
            this.dataGridView1.Sort(this.dataGridView1.Columns["Invoice Date"], ListSortDirection.Descending);
            this.dataGridView1.Sort(this.dataGridView1.Columns["Customer"], ListSortDirection.Descending);
            this.dataGridView1.Sort(this.dataGridView1.Columns["Check"], ListSortDirection.Descending);
        }

        public void RefreshAccess()
        {
            tbxCustId.Text = "";
            tbxCustName.Text = "";
            tbxInvoiceFrom.Text = "";
            tbxInvoiceTo.Text = "";
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;
            dtDueFrom.Value = DateTime.Now;
            dtDueTo.Value = DateTime.Now;
            CheckedId.Clear();
            TransStatus.Clear();
            count = 0;
            RefreshGrid();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void to_Click(object sender, EventArgs e)
        {

        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroTabControl1.SelectedIndex == 1)
            {
                btnSelect.Visible = true;
                btnCollectionOnProgress.PerformClick();
            }
            else
            {
                btnSelect.Visible = false;
            }
        }

        private void btnCustSearch_Click(object sender, EventArgs e)
        {
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = "[dbo].[CustTable]";
            F.QuerySearch = "SELECT [CustId], [CustName] FROM [dbo].[CustTable] WHERE 1=1 ";
            F.FilterText = new string[] { "CustId", "CustName" };
            F.Select = new string[] { "CustId", "CustName" };
            F.Mask = new string[] { "Customer Id", "Customer Name" };
            F.PrimaryKey = "CustId";
            F.HideField = new string[] { "Check" };
            F.Parent = this;
            F.ShowDialog();
            if (Variable.Kode2 != null)
            {
                tbxCustId.Text = Variable.Kode2[0, 0];
                tbxCustName.Text = Variable.Kode2[0, 1];
            }
            Variable.Kode2 = null;
        }

        private void tbxInvoiceFrom_Leave(object sender, EventArgs e)
        {
            if (tbxInvoiceFrom.Text == "" || tbxInvoiceFrom.Text == null)
            {
                return;
            }
            tbxInvoiceFromPopulate();
        }

        private void tbxInvoiceFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                tbxInvoiceFromPopulate();
            }
        }

        private void tbxInvoiceFromPopulate()
        {
            int i = 0;
            Query = "SELECT COUNT([Invoice_Id]) FROM [ISBS-NEW4].[dbo].[CustInvoice_H] WHERE [Invoice_Id] LIKE @tbxSearch ";
            using (Conn = ConnectionString.GetConnection())
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@tbxSearch", "%" + tbxInvoiceFrom.Text + "%");
                i = Convert.ToInt32(Cmd.ExecuteScalar());
            }

            if (i == 0 || i < 0)
            {
                MessageBox.Show("Tidak ada Invoice Id dengan filter " + tbxInvoiceFrom.Text + ". ");
                tbxInvoiceFrom.Text = "";
            }
            else if (i == 1)
            {
                Query = "SELECT [Invoice_Id] FROM [ISBS-NEW4].[dbo].[CustInvoice_H] WHERE [Invoice_Id] LIKE @tbxSearch ";
                using (Conn = ConnectionString.GetConnection())
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@tbxSearch", "%" + tbxInvoiceFrom.Text + "%");
                    tbxInvoiceFrom.Text = Cmd.ExecuteScalar().ToString();
                }
            }
            else
            {
                ISBS_New.SearchQueryV2 F = new SearchQueryV2();
                F.Table = "[dbo].[CustInvoice_H]";
                F.QuerySearch = "SELECT [Invoice_Id],[Invoice_Date],[Invoice_Type],[Invoice_Amount],[Cust_Name],[Invoice_DueDate],[TransStatus] FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] LIKE '%" + tbxInvoiceFrom.Text + "%' ";
                F.FilterText = new string[] { "Invoice_Id", "Invoice_Date", "Invoice_Type", "Invoice_Amount", "Cust_Name", "Invoice_DueDate", "TransStatus" };
                F.Select = new string[] { "Invoice_Id" };
                F.PrimaryKey = "Invoice_Id";
                F.HideField = new string[] { "Check" };
                F.Parent = this;
                F.ShowDialog();
                if (Variable.Kode2 != null)
                {
                    tbxInvoiceFrom.Text = Variable.Kode2[0, 0];
                }
                else
                {
                    tbxInvoiceFrom.Text = "";
                }
                Variable.Kode2 = null;

            }
        }

        private void tbxInvoiceTo_Leave(object sender, EventArgs e)
        {
            if (tbxInvoiceTo.Text == "" || tbxInvoiceTo.Text == null)
            {
                return;
            }
            tbxInvoiceToPopulate();
        }

        private void tbxInvoiceTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                tbxInvoiceToPopulate();
            }
        }

        private void tbxInvoiceToPopulate()
        {
            int i = 0;
            Query = "SELECT COUNT([Invoice_Id]) FROM [ISBS-NEW4].[dbo].[CustInvoice_H] WHERE [Invoice_Id] LIKE @tbxSearch ";
            using (Conn = ConnectionString.GetConnection())
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@tbxSearch", "%" + tbxInvoiceTo.Text + "%");
                i = Convert.ToInt32(Cmd.ExecuteScalar());
            }

            if (i == 0 || i < 0)
            {
                MessageBox.Show("Tidak ada Invoice Id dengan filter " + tbxInvoiceTo.Text + ". ");
                tbxInvoiceTo.Text = "";
            }
            else if (i == 1)
            {
                Query = "SELECT [Invoice_Id] FROM [ISBS-NEW4].[dbo].[CustInvoice_H] WHERE [Invoice_Id] LIKE @tbxSearch ";
                using (Conn = ConnectionString.GetConnection())
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@tbxSearch", "%" + tbxInvoiceTo.Text + "%");
                    tbxInvoiceTo.Text = Cmd.ExecuteScalar().ToString();
                }
            }
            else
            {
                ISBS_New.SearchQueryV2 F = new SearchQueryV2();
                F.Table = "[dbo].[CustInvoice_H]";
                F.QuerySearch = "SELECT [Invoice_Id],[Invoice_Date],[Invoice_Type],[Invoice_Amount],[Cust_Name],[Invoice_DueDate],[TransStatus] FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] LIKE '%" + tbxInvoiceTo.Text + "%' ";
                F.FilterText = new string[] { "Invoice_Id", "Invoice_Date", "Invoice_Type", "Invoice_Amount", "Cust_Name", "Invoice_DueDate", "TransStatus" };
                F.Select = new string[] { "Invoice_Id" };
                F.PrimaryKey = "Invoice_Id";
                F.HideField = new string[] { "Check" };
                F.Parent = this;
                F.ShowDialog();
                if (Variable.Kode2 != null)
                {
                    tbxInvoiceTo.Text = Variable.Kode2[0, 0];
                }
                else
                {
                    tbxInvoiceTo.Text = "";
                }
                Variable.Kode2 = null;

            }
        }

        private void chkInvoiceDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkInvoiceDate.Checked == true)
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }
        }

        private void chkDueDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDueDate.Checked == true)
            {
                dtDueFrom.Enabled = true;
                dtDueTo.Enabled = true;
            }
            else
            {
                dtDueFrom.Enabled = false;
                dtDueTo.Enabled = false;
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            RefreshGrid2(CollectionStat);
        }

        private void btnReset2_Click(object sender, EventArgs e)
        {
            tbxSearhFilter.Text = "";
            dtFromLog.Value = DateTime.Now;
            dtToLog.Value = DateTime.Now;
            cmbCrit.SelectedIndex = 0;
            RefreshGrid2(CollectionStat);
        }

        private void cmbCrit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCrit.Text.Contains("Date"))
            {
                dtFromLog.Enabled = true;
                dtToLog.Enabled = true;
                tbxSearhFilter.Enabled = false;
                tbxSearhFilter.Text = "";
            }
            else
            {
                dtFromLog.Enabled = false;
                dtToLog.Enabled = false;
                tbxSearhFilter.Enabled = true;
            }
        }

        //stv edit start
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dataGridView1.Columns[e.ColumnIndex].Name == "Send Email")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Status"].Value.ToString() == "Reconciled Full" || dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Status"].Value.ToString() == "Collection Full")
                {
                    MessageBox.Show("Invoice sudah Complete.");
                    return;
                }
                Conn = ConnectionString.GetConnection();
                string Jenis = "CL", Kode = "CL";
                string CLNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);
                string InvoiceNumber = dataGridView1.Rows[e.RowIndex].Cells["Invoice No"].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells["Invoice No"].Value.ToString();
                string CustomerId = dataGridView1.Rows[e.RowIndex].Cells["Customer Id"].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells["Customer Id"].Value.ToString();

                ISBS_New.ARCollection.SendEmail s = new ISBS_New.ARCollection.SendEmail();
                s.setParent(this);
                s.flag(CLNumber,CustomerId,"New");
                s.getARNo(InvoiceNumber);
                s.ShowDialog();            
                Conn.Close();
                RefreshAccess();
            }
            if (e.RowIndex > -1 && dataGridView1.Columns[e.ColumnIndex].Name == "Create Mail Letter")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Status"].Value.ToString() == "Reconciled Full" || dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Status"].Value.ToString() == "Collection Full")
                {
                    MessageBox.Show("Invoice sudah Complete.");
                    return;
                }
                try
                {
                    Collection.CollectionPopUpInputMailAddress PopUp = new CollectionPopUpInputMailAddress(this, dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Invoice No"].Value.ToString());
                    PopUp.ShowDialog();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.ToString());
                }
                finally { }
            }
        }
        //stv edit end

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string CLNo = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["CL No"].Value.ToString();
            string collector = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["Collector"].Value.ToString();
            string Type = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["Collection Type"].Value.ToString();
            CallForm(Type, CLNo, collector);
        }

        private void btnInvoiceOnProgress_Click(object sender, EventArgs e)
        {
            btnInvoiceOnProgress.BackColor = Color.Black;
            btnInvoiceOnProgress.ForeColor = Color.White;
            btnInvoiceCompleted.BackColor = Color.LightGray;
            btnInvoiceCompleted.ForeColor = Color.Black;
            InvoiceStat = "Invoice On Progress";
            RefreshGrid1("Invoice On Progress");
        }

        private void btnInvoiceCompleted_Click(object sender, EventArgs e)
        {
            btnInvoiceCompleted.BackColor = Color.Black;
            btnInvoiceCompleted.ForeColor = Color.White;
            btnInvoiceOnProgress.BackColor = Color.LightGray;
            btnInvoiceOnProgress.ForeColor = Color.Black;
            InvoiceStat = "Invoice Completed";
            RefreshGrid1("Invoice Completed");
        }

        private void btnCollectionOnProgress_Click(object sender, EventArgs e)
        {
            btnCollectionOnProgress.BackColor = Color.Black;
            btnCollectionOnProgress.ForeColor = Color.White;
            btnCollectionCompleted.BackColor = Color.LightGray;
            btnCollectionCompleted.ForeColor = Color.Black;
            CollectionStat = "Collection On Progress";
            RefreshGrid2("Collection On Progress");
        }

        private void btnCollectionCompleted_Click(object sender, EventArgs e)
        {
            btnCollectionCompleted.BackColor = Color.Black;
            btnCollectionCompleted.ForeColor = Color.White;
            btnCollectionOnProgress.BackColor = Color.LightGray;
            btnCollectionOnProgress.ForeColor = Color.Black;
            CollectionStat = "Collection Completed";
            RefreshGrid2("Collection Completed");
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ARCollection.CollectionResult.CLR_Form f = new ARCollection.CollectionResult.CLR_Form();
            string CL_NO = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["CL No"].Value.ToString();
            f.SetMode("BeforeEdit", CL_NO);
            f.setParent2(this);
            f.Show();

            //string CLNo = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["CL No"].Value.ToString();
            //string collector = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["Collector"].Value.ToString();
            //string Type = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["Collection Type"].Value.ToString();
            //CallForm(Type, CLNo,collector);
        }



    }
}
