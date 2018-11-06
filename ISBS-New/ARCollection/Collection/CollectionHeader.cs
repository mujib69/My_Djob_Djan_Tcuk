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
    public partial class CollectionHeader : MetroFramework.Forms.MetroForm
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

        /*********datagridview cols name*********/
        string[] tableCols = new string[] { "No", "Customer", "Invoice No", "Invoice Date", "Invoice Amount", "Invoice Due", "Outstanding", "Kwitansi No","Payment Amount","Payment Date","Payment DueDate","Payment No","Comments" };
        string[] readOnlyNames = new string[] {"Payment Amount","cmbResult","cmbPaymentMethod","Comments","Payment Date","Payment DueDate","Payment No" };
        /*********datagridview cols name*********/

        /**********SET MODE*********/
        private string Mode;
        private string CLID;
        /**********SET MODE*********/

        /*********PARENT*********/
        Collection.CollectionTaskList Parent = new Collection.CollectionTaskList();
        public void SetParent(Collection.CollectionTaskList F) { Parent = F; }
        //List<NTForm> ListNTForm = new List<NTForm>();
        /*********PARENT*********/

        /************************/
        List<string> passedId = new List<string>();
        string passedClNo = "";
        string passedCollector = "";
        /************************/

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        bool check; //label
        /*********VALIDATION*********/

        /****Cmb Box in Grid List*****/
        List<string> cmbResult1 = new List<string>(){"Select","Tertagih","Tidak Tertagih","Tukar Bon"};
        List<string> cmbPaymentMethod = new List<string>(){"Select","CASH","TRANSFER","GIRO","CHEQUE"};
        /*****************************/

        int CurrentColorIndex = 0;

        public CollectionHeader()
        {
            InitializeComponent();
        }

        public CollectionHeader(List<string> InvoiceId)
        {
            InitializeComponent();
            passedId = InvoiceId;
        }

        public CollectionHeader(string Clno, string collector)
        {
            InitializeComponent();
            passedClNo = Clno;
            passedCollector = collector;
        }

        private void CollectionHeader_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }

            if (tbxCollectionType.Text.ToUpper() == "MANUAL")
            {
                btnSendEmail.Visible = false;
                btnCreateMailLetter.Visible = false;
                if (tbxStatus.Text.ToUpper() == "COLLECTION INITIATED")
                {
                    btnConfirm.Visible = true;
                }
                else
                {
                    btnConfirm.Visible = false;
                }
            }
            else if (tbxCollectionType.Text.ToUpper() == "EMAIL")
            {
                btnCreateMailLetter.Visible = false;

                btnConfirm.Visible = false;
                btnPrint.Visible = false;
                btnApprove.Visible = false;
            }
            else if (tbxCollectionType.Text.ToUpper() == "POST")
            {
                btnSendEmail.Visible = false;

                btnConfirm.Visible = false;
                btnPrint.Visible = true;
                btnApprove.Visible = false;
            }
            if (ControlMgr.GroupName.ToUpper() == "AR MANAGER")
            {
                btnApprove.Visible = true;
            }
            //if all invoice outstanding are 0, then it must mean it's for creating tukar bon so no other invoice with outstanding can be taken into this cl
            decimal TotalOutstanding = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                TotalOutstanding += Convert.ToDecimal(dataGridView1.Rows[i].Cells["Outstanding"].Value);
            }
            if (TotalOutstanding == 0)
            {
                btnAdd.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void ModeNew()
        {
            btnApprove.Enabled = false;
            btnConfirm.Enabled = false;
            btnPrint.Enabled = false;
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;

            dataGridView1.ColumnCount = tableCols.Length;
            for (int i = 0; i < tableCols.Length; i++)
            {
                dataGridView1.Columns[i].Name = tableCols[i];
                //to hide unneded columns
                for (int x = 0; x < readOnlyNames.Count(); x++)
                {
                    if (tableCols[i] == readOnlyNames[x])
                    {
                        dataGridView1.Columns[readOnlyNames[x]].Visible = false;
                    }
                }
            }

            for (int i = 0; i < passedId.Count; i++)
            {
                Query = "SELECT * FROM [ISBS-NEW4].[dbo].[CustInvoice_H] WHERE [Invoice_Id] = '"+passedId[i]+"'";
                using(Conn = ConnectionString.GetConnection())
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        dataGridView1.Rows.Add(0,Dr["Cust_Name"],Dr["Invoice_Id"],Convert.ToDateTime(Dr["Invoice_Date"]).ToString("dd/MM/yyyy"),Dr["Invoice_Amount"],Convert.ToDateTime(Dr["Invoice_DueDate"]).ToString("dd/MM/yyyy"),Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"]),Dr["Kwitansi_No"],"");
                    }
                    Dr.Close();
                }
            }
            dataGridView1.AllowUserToAddRows = false;
            AddKwitansiId();
        }

        //TO add the unchosen invoices but have the same kwitansi Id with the chosen kwitansi Id
        private void AddKwitansiId()
        {
            Color[] color = new Color[]{Color.LightBlue, Color.LightCoral, Color.LightCyan,Color.LightGoldenrodYellow, Color.LightGreen, Color.LightSalmon, Color.LightYellow, Color.LightSkyBlue, Color.LightPink, Color.LightSteelBlue};
            if (CurrentColorIndex == color.Count())
            {
                CurrentColorIndex = 0;
            }
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells["Kwitansi No"].Value != null && dataGridView1.Rows[i].Cells["Kwitansi No"].Value.ToString() != "")
                {
                    Query = " SELECT * FROM [ISBS-NEW4].[dbo].[CustInvoice_H] WHERE [Kwitansi_No] = '" + dataGridView1.Rows[i].Cells["Kwitansi No"].Value.ToString() + "' AND TransStatus IN(03,13,22) ";
                    for (int x = 0; x < dataGridView1.Rows.Count; x++)
                    {
                        Query += " AND Invoice_Id != '" + dataGridView1.Rows[x].Cells["Invoice No"].Value.ToString() + "' ";
                    }
                    using (Conn = ConnectionString.GetConnection())
                    using (Cmd = new SqlCommand(Query, Conn))
                    {    
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                dataGridView1.Rows.Add(1);
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Customer"].Value = Dr["Cust_Name"];
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Invoice Date"].Value = Convert.ToDateTime(Dr["Invoice_Date"]).ToShortDateString();
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Invoice No"].Value = Dr["Invoice_Id"];
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Invoice Due"].Value = Convert.ToDateTime(Dr["Invoice_DueDate"]).ToShortDateString();
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Invoice Amount"].Value = Dr["Invoice_Amount"];
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Outstanding"].Value = Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"]);
                                dataGridView1.Rows[dataGridView1.RowCount - 1].Cells["Kwitansi No"].Value = Dr["Kwitansi_No"];
                                dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.BackColor = color[CurrentColorIndex];
                            }
                            CurrentColorIndex++;
                        }
                        Dr.Close();
                    }
                }
            }
            dataGridView1.AutoResizeColumns();
            //this.dataGridView1.Sort(this.dataGridView1.Columns["Payment Due"], ListSortDirection.Descending);
            //this.dataGridView1.Sort(this.dataGridView1.Columns["Invoice Date"], ListSortDirection.Descending);
            this.dataGridView1.Sort(this.dataGridView1.Columns["Customer"], ListSortDirection.Descending);
        }

        private void ModeBeforeEdit()
        {
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnExit.Enabled = true;
            btnConfirm.Enabled = true;
            btnPrint.Enabled = true;
            btnApprove.Enabled = true;
            btnSCollector.Enabled = false;

            GetDataHeader();
            GetDataDtl();

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
            }
        }

        private void ModeEdit()
        {
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnExit.Enabled = false;
            btnConfirm.Enabled = false;
            btnPrint.Enabled = false;
            btnApprove.Enabled = false;
            btnSCollector.Enabled = true;

            if (tbxStatus.Text.ToUpper() == "COMPLETED")
            {
                ColorandReadOnlyColumns_BeforeEditMode();
            }
            else
            {
                for (int i = 0; i < tableCols.Count(); i++)
                {
                    dataGridView1.Columns[tableCols[i]].ReadOnly = true;
                }
                for (int x = 0; x < readOnlyNames.Count(); x++)
                {
                    dataGridView1.Columns[readOnlyNames[x]].ReadOnly = false;
                }
            }
        }

        //type value : #1 string #2 decimal #3 int
        private void createLabel(Control textbox, Control lblName, Control location, string type)
        {
            //if (validate == false)
            //{
            //    label[count] = new Label();
            //}
            if (label[count] == null)
            {
                label[count] = new Label();
            }
            if (type == "string")
            {
                if (textbox.Text == String.Empty || textbox.Text == "Select")
                {
                    textbox.BackColor = Color.LightGoldenrodYellow;

                    label[count].Text = "*";
                    label[count].ForeColor = Color.Red;
                    label[count].Width = 10;
                    label[count].Location = new System.Drawing.Point(lblName.Location.X - 9, lblName.Location.Y);
                    label[count].BringToFront();

                    location.Controls.Add(label[count]);
                    label[count].Visible = true;
                    flag = 'X';
                }
                else
                {
                    label[count].Visible = false;
                    textbox.BackColor = Color.Empty;
                }
            }
            else if (type == "decimal" || type == "int")
            {
                if (Convert.ToDecimal(textbox.Text) == 0)
                {
                    textbox.BackColor = Color.LightGoldenrodYellow;

                    label[count].Text = "*";
                    label[count].ForeColor = Color.Red;
                    label[count].Width = 10;
                    label[count].Location = new System.Drawing.Point(lblName.Location.X - 9, lblName.Location.Y);
                    label[count].BringToFront();

                    location.Controls.Add(label[count]);
                    label[count].Visible = true;
                    flag = 'X';
                }
                else
                {
                    label[count].Visible = false;
                    textbox.BackColor = Color.Empty;
                }
            }
            count++;
        }

        private char Validation()
        {
            try
            {
                flag = '\0';
                string message = "";
                if (dataGridView1.Rows.Count <= 0)
                {
                    flag = 'X';
                    message += "-Tidak terdapat item pada data grid.\n";
                }
                if (tbxName.Text == "" || tbxName.Text == null)
                {
                    flag = 'X';
                    message += "-Textbox Collector masih kosong.\n";
                }


                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    //CHECKING OUTSTANDING INVOICES
                    Query = "SELECT [AR_Amount],[Settle_Amount],[TransStatus] FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] = '" + dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString() + "' ";
                    using (Conn = ConnectionString.GetConnection())
                    using (Cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            if ((Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"])) == 0)
                            {
                                flag = 'X';
                                message += "-Outstanding tidak mencukupi untuk Invoice Id = " + dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString() + ".\n";
                            }
                            if ((Mode == "Edit")&&(Dr["TransStatus"].ToString() != "09" && Dr["TransStatus"].ToString() != "03" && Dr["TransStatus"].ToString() != "13" && Dr["TransStatus"].ToString() != "22"))
                            {
                                flag = 'X';
                                message += "-Status Invoice Id = " + dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString() + " sudah tidak dapat diedit.\n";
                            }
                        }
                        Dr.Close();
                    }

                    //CHECKING OUTSTANDING INVOICES

                    if (tbxStatus.Text.ToUpper() == "COLLECTION IN COLLECTOR" && tbxCollectionType.Text.ToUpper() == "MANUAL")
                    {
                        if (dataGridView1.Rows[i].Cells["cmbResult"].Value == null )
                        {
                            flag = 'X';
                            message += "-Row "+(i+1)+" Kolom Result harus dipilih result statusnya.\n";
                        }
                        else if (dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString() == "" || dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString() == "Select")
                        {
                            flag = 'X';
                            message += "-Row "+(i+1)+" Kolom Result harus dipilih result statusnya.\n";
                        }
                        //Check klo tidak tertagih/tukar bon hrs isi comments
                        else if (dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString().ToUpper() == "TIDAK TERTAGIH" || dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString().ToUpper() == "TUKAR BON")
                        {
                            if (dataGridView1.Rows[i].Cells["Comments"].Value.ToString().Trim() == "")
                            {
                                flag = 'X';
                                message += "-Row "+(i+1)+" dgn status Tak Tertagih/Tukar Bon harus diberi keterangan pada kolom Comments.\n";
                            }
                        }

                        //Check klo tertagih hrs isi payment kolom
                        else if (dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString().ToUpper() == "TERTAGIH")
                        {
                            if (dataGridView1.Rows[i].Cells["cmbPaymentMethod"].Value == null)
                            {
                                flag = 'X';
                                message += "-Row " + (i + 1) + " Payment Method harus dipilih tipenya.\n";
                            }
                            else if (dataGridView1.Rows[i].Cells["cmbPaymentMethod"].Value.ToString() == "" || dataGridView1.Rows[i].Cells["cmbPaymentMethod"].Value.ToString() == "Select")
                            {
                                flag = 'X';
                                message += "-Row " + (i + 1) + " Payment Method harus dipilih tipenya.\n";
                            }
                            if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Payment Amount"].Value) == 0)
                            {
                                flag = 'X';
                                message += "-Row " + (i + 1) + " Payment Amount tidak boleh bernilai 0.\n";
                            }
                            if (dataGridView1.Rows[i].Cells["Payment Date"].Value.ToString().Trim() == "")
                            {
                                flag = 'X';
                                message += "-Row " + (i + 1) + " Payment Date harus diisi.\n";
                            }
                            if (dataGridView1.Rows[i].Cells["Payment DueDate"].Value.ToString().Trim() == "")
                            {
                                flag = 'X';
                                message += "-Row " + (i + 1) + " Payment DueDate harus diisi.\n";
                            }
                            if (dataGridView1.Rows[i].Cells["Payment No"].Value.ToString().Trim() == "")
                            {
                                flag = 'X';
                                message += "-Row " +( i + 1) + " Payment No harus diisi.\n";
                            }
                        }
                    }
                }

                if (message != "")
                {
                    MessageBox.Show(message);
                }
                return flag;
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return '\0';
            }
            finally { }
        }

        public void SetMode(string tmpMode, string tmpNumber)
        {
            Mode = tmpMode;
            tbxCLNo.Text = tmpNumber;
            CLID = tmpNumber;
        }

        private void GetDataDtl()
        {
            dataGridView1.Rows.Clear();

            bool Result = false;
            bool PaymentMethod = false;
            for (int i = dataGridView1.Columns.Count - 1; i >= 0; i--)
            {
                if (dataGridView1.Columns[i].Name == "cmbResult")
                {
                    Result = true;
                }
                else if (dataGridView1.Columns[i].Name == "cmbPaymentMethod")
                {
                    PaymentMethod = true;
                }
            }
            if (Result == false)
            {
                DataGridViewComboBoxColumn cmbCell = new DataGridViewComboBoxColumn();
                cmbCell.Name = "cmbResult";
                cmbCell.DataSource = cmbResult1;
                cmbCell.HeaderText = "Result";
                cmbCell.Width = 100;
                cmbCell.DropDownWidth = 100;
                dataGridView1.Columns.Insert(9, cmbCell);
            }
            if (PaymentMethod == false)
            {
                DataGridViewComboBoxColumn cmbCell2 = new DataGridViewComboBoxColumn();
                cmbCell2.Name = "cmbPaymentMethod";
                cmbCell2.DataSource = cmbPaymentMethod;
                cmbCell2.HeaderText = "Payment Method";
                cmbCell2.Width = 100;
                cmbCell2.DropDownWidth = 100;
                dataGridView1.Columns.Insert(10, cmbCell2);
            }

            //List<string> invoiceid = new List<string>();

            //Query = "SELECT [Invoice_Id] FROM [dbo].[Collection_Dtl] WHERE [CL_No] = ";
            //using (Conn = ConnectionString.GetConnection())
            //using (Cmd= new SqlCommand(Query,Conn))
            //{
            //    Dr = Cmd.ExecuteReader();
            //    while (Dr.Read())
            //    {
            //        invoiceid.Add(Dr["Invoice_Id"].ToString());
            //    }
            //    Dr.Close();
            //}
            //for (int i = 0; i < invoiceid.Count; i++)
            //{
            Query = "SELECT a.Cust_Name,a.Invoice_Id,a.Invoice_Date,a.Invoice_Amount,a.Invoice_DueDate,a.AR_Amount,a.Settle_Amount,a.Kwitansi_No,b.* FROM [ISBS-NEW4].[dbo].[CustInvoice_H] a LEFT JOIN [dbo].[Collection_Dtl] b ON b.Invoice_Id= a.Invoice_Id WHERE b.CL_No = '" + tbxCLNo.Text + "' ";
            using (Conn = ConnectionString.GetConnection())
            using (Cmd = new SqlCommand(Query, Conn))
            {
                int i = 0;
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dataGridView1.Rows.Add(0, Dr["Cust_Name"], Dr["Invoice_Id"], Convert.ToDateTime(Dr["Invoice_Date"]).ToString("dd/MM/yyyy"), Dr["Invoice_Amount"], Convert.ToDateTime(Dr["Invoice_DueDate"]).ToString("dd/MM/yyyy"), Convert.ToDecimal(Dr["AR_Amount"]) - Convert.ToDecimal(Dr["Settle_Amount"]), Dr["Kwitansi_No"], "0.0000", "", "", "", "");
                    if ((tbxStatus.Text.ToUpper() == "COLLECTION IN COLLECTOR" || tbxStatus.Text.ToUpper() == "COMPLETED")&& tbxCollectionType.Text.ToUpper() == "MANUAL")
                    {
                        dataGridView1.Rows[i].Cells["cmbPaymentMethod"].Value = Dr["Payment_Method"];
                        dataGridView1.Rows[i].Cells["Payment Date"].Value = Dr["Payment_Date"] == System.DBNull.Value ? "" : Convert.ToDateTime(Dr["Payment_Date"]).ToString("dd/MM/yyyy");
                        dataGridView1.Rows[i].Cells["Payment No"].Value = Dr["Payment_No"] == System.DBNull.Value ? "" : Dr["Payment_No"].ToString();
                        dataGridView1.Rows[i].Cells["Payment Amount"].Value = Dr["Payment_Amount"] == System.DBNull.Value ? "" : String.Format("{0:#,##0.###0}", Dr["Payment_Amount"]);
                        dataGridView1.Rows[i].Cells["Payment DueDate"].Value = Dr["Payment_DueDate"] == System.DBNull.Value ? "" : Convert.ToDateTime(Dr["Payment_DueDate"]).ToString("dd/MM/yyyy");
                        dataGridView1.Rows[i].Cells["cmbResult"].Value = Dr["Result"];
                        dataGridView1.Rows[i].Cells["Comments"].Value = Dr["Comments"] == System.DBNull.Value ? "" : Dr["Comments"].ToString();
                    }
                    i++;
                }
                Dr.Close();
            }

            //}

            if (!((tbxStatus.Text.ToUpper() == "COLLECTION IN COLLECTOR" || tbxStatus.Text.ToUpper() == "COMPLETED") && tbxCollectionType.Text.ToUpper() == "MANUAL"))
            {
                for (int i = 0; i < readOnlyNames.Count(); i++)
                {
                    string tes = readOnlyNames[i];
                    dataGridView1.Columns[readOnlyNames[i]].Visible = false;
                }
            }
            else
            {
                ColorandReadOnlyColumns_BeforeEditMode();
            }
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoResizeColumns();
        }

        private void GetDataHeader()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.ColumnCount = tableCols.Length;
            for (int i = 0; i < tableCols.Length; i++)
                dataGridView1.Columns[i].Name = tableCols[i];

            if (tbxCLNo.Text == "")
            {
                tbxCLNo.Text = passedClNo;
            }
            if (tbxName.Text == "")
            {
                tbxName.Text = passedCollector;
            }
            Query = "SELECT b.[Deskripsi] FROM [dbo].[Collection_H] a LEFT JOIN [dbo].[TransStatusTable] b ON b.[StatusCode]= a.[Status_Code] WHERE b.[TransCode] = 'Collection' AND a.[CL_No] = '" + tbxCLNo.Text + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                tbxStatus.Text = Cmd.ExecuteScalar().ToString();
            }
        }

        private void btnSCollector_Click(object sender, EventArgs e)
        {
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = "[dbo].[Collector]";
            F.QuerySearch = "SELECT [Collector_Name] FROM [dbo].[Collector] WHERE 1=1 ";
            F.FilterText = new string[] { "Collector_Name" };
            F.Select = new string[] { "Collector_Name" };
            F.PrimaryKey = "Collector_Name";
            F.HideField = new string[] { "Check" };
            F.Parent = this;
            F.ShowDialog();
            if (Variable.Kode2 != null)
            {
                tbxName.Text = Variable.Kode2[0, 0];
            }
            Variable.Kode2 = null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation() == 'X')
            {
                return;
            }
            else
            {
                try
                {
                    using (scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();
                        if (Mode == "New")
                        {
                            string Jenis = "CL", Kode = "CL";
                            string CLNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);
                            tbxCLNo.Text = CLNo;
                            
                            //insert into Collection H
                            Query = "INSERT INTO [dbo].[Collection_H] ([CL_Date],[CL_No],[CL_Type],[Collector],[Email_Id],[Email_C],[Email_Ctr],[Mail_Address],[Print_C],[Print_Ctr],[Status_Code],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "','" + CLNo + "','" + tbxCollectionType.Text + "','" + tbxName.Text + "','0','0','0','','0','0','01','" + (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + "','" + ControlMgr.UserId + "','" + (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) + "','" + ControlMgr.UserId + "' );";
                            using (Cmd = new SqlCommand(Query, Conn))
                            {
                                Cmd.ExecuteNonQuery();
                            }

                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                string CustId = "";
                                string InvoiceType = "";
                                string TandaTerima = ""; //kwitansi
                                string InvoiceNo = dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString();

                                Query = "SELECT * FROM [CustInvoice_H] WHERE [Invoice_Id] = '" + InvoiceNo + "'";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Dr = Cmd.ExecuteReader();
                                    while (Dr.Read())
                                    {
                                        InvoiceType = Dr["Invoice_Type"].ToString();
                                        CustId = Dr["Cust_Id"].ToString();
                                        TandaTerima = Dr["Kwitansi_No"].ToString();
                                    }
                                    Dr.Close();
                                }

                                //Insert into collection Detail
                                Query = "INSERT INTO [dbo].[Collection_Dtl](CL_No,SeqNo,CustId,Invoice_Type,Invoice_Id,TT_No,Result,Comments,ConfirmedBy,CreatedDate,CreatedBy) VALUES ('" + CLNo + "'," + (i + 1) + ",'" + CustId + "','" + InvoiceType + "','" + InvoiceNo + "','" + TandaTerima + "','','','','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ControlMgr.UserId + "' );";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Cmd.ExecuteNonQuery();
                                }

                                //Update Invoice Status
                                Query = "UPDATE [dbo].[CustInvoice_H] SET [TransStatus] = '09' WHERE [Invoice_Id] = '" + InvoiceNo + "' ";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                            }
                            InsertCLRLog("01","N");
                        }
                        else
                        {
                            //Update Collection H 
                            if (tbxStatus.Text.ToUpper() == "COLLECTION IN COLLECTOR")
                            {
                                Query = "UPDATE [dbo].[Collection_H] SET [Status_Code] ='03',[UpdatedDate] = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', [UpdatedBy]='" + ControlMgr.UserId + "',[Collector]='" + tbxName.Text + "' WHERE [CL_No]='" + tbxCLNo.Text + "'";
                            }
                            else
                            {
                                Query = "UPDATE [dbo].[Collection_H] SET [UpdatedDate] = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', [UpdatedBy]='" + ControlMgr.UserId + "',[Collector]='" + tbxName.Text + "' WHERE [CL_No]='" + tbxCLNo.Text + "'";
                            }
                            using (Cmd = new SqlCommand(Query, Conn))
                            {
                                Cmd.ExecuteNonQuery();
                            }

                            int row = 0;
                            Query = "SELECT [Invoice_Id] FROM [dbo].[Collection_Dtl] WHERE [CL_No] = '" + tbxCLNo.Text + "'";
                            using (Cmd = new SqlCommand(Query, Conn))
                            {
                                string transstatus = "00";
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    //Finding the invoice status
                                    string Query2 = "SELECT [PPN_Percent],[Settle_Amount],[AR_Amount] FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] = '" + Dr["Invoice_Id"].ToString() + "'";
                                    using (SqlCommand Cmd2 = new SqlCommand(Query2, Conn))
                                    {
                                        SqlDataReader Dr2 = Cmd2.ExecuteReader();
                                        while (Dr2.Read())
                                        {
                                            if (Convert.ToDecimal(Dr2["AR_Amount"]) - Convert.ToDecimal(Dr2["Settle_Amount"]) != 0)
                                            {
                                                transstatus = "22";
                                            }
                                            else if (Convert.ToDecimal(Dr2["AR_Amount"]) - Convert.ToDecimal(Dr2["Settle_Amount"]) == 0)
                                            {
                                                transstatus = "21";
                                            }
                                            else if (Convert.ToDecimal(Dr2["PPN_Percent"]) != 0)
                                            {
                                                transstatus = "13";
                                            }
                                            else
                                            {
                                                transstatus = "03";
                                            }
                                        }
                                    }

                                    //Update Invoice Status
                                    Query2 = "UPDATE [dbo].[CustInvoice_H] SET [TransStatus] = '"+transstatus+"' WHERE [Invoice_Id] = '" + Dr["Invoice_Id"].ToString() + "'";
                                    using (SqlCommand Cmd2 = new SqlCommand(Query2, Conn))
                                    {
                                        Cmd2.ExecuteNonQuery();
                                    }
                                }
                                Dr.Close();
                            }

                            Query = "DELETE FROM [dbo].[Collection_Dtl] WHERE [CL_No]='" + tbxCLNo.Text + "'";
                            using (Cmd = new SqlCommand(Query, Conn))
                            {
                                Cmd.ExecuteNonQuery();
                            }

                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                string CustId = "";
                                string InvoiceType = "";
                                string TandaTerima = ""; //kwitansi
                                string InvoiceNo = dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString();

                                Query = "SELECT * FROM [CustInvoice_H] WHERE [Invoice_Id] = '" + InvoiceNo + "'";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Dr = Cmd.ExecuteReader();
                                    while (Dr.Read())
                                    {
                                        InvoiceType = Dr["Invoice_Type"].ToString();
                                        CustId = Dr["Cust_Id"].ToString();
                                        TandaTerima = Dr["Kwitansi_No"].ToString();
                                    }
                                    Dr.Close();
                                }
      
                                Query = "INSERT INTO [dbo].[Collection_Dtl](CL_No,SeqNo,CustId,Invoice_Type,Invoice_Id,TT_No,Result,Comments,ConfirmedBy,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) ";
                                Query += "VALUES ('" + tbxCLNo.Text + "'," + (i + 1) + ",'" + CustId + "','" + InvoiceType + "','" + InvoiceNo + "','" + TandaTerima + "','"+dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString()+"',@Comments,'','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ControlMgr.UserId + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ControlMgr.UserId + "');";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Cmd.Parameters.AddWithValue("@Comments", dataGridView1.Rows[i].Cells["Comments"].Value == null ? "":dataGridView1.Rows[i].Cells["Comments"].Value.ToString());
                                    Cmd.ExecuteNonQuery();
                                }
                                if (dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString().ToUpper() == "TERTAGIH")
                                {
                                    Query = "UPDATE [dbo].[Collection_Dtl] SET Payment_Method = @Payment_Method,Payment_Date=@Payment_Date,Payment_No=@Payment_No,Payment_Amount=@Payment_Amount,Payment_DueDate=@Payment_DueDate WHERE CL_No = '" + tbxCLNo.Text + "' AND SeqNo = " + (i + 1) + "";
                                    using (Cmd = new SqlCommand(Query, Conn))
                                    {
                                        Cmd.Parameters.AddWithValue("@Payment_Method", dataGridView1.Rows[i].Cells["cmbPaymentMethod"].Value == null ? null : dataGridView1.Rows[i].Cells["cmbPaymentMethod"].Value.ToString());
                                        Cmd.Parameters.AddWithValue("@Payment_Date", Convert.ToDateTime(dataGridView1.Rows[i].Cells["Payment Date"].Value));
                                        Cmd.Parameters.AddWithValue("@Payment_No", dataGridView1.Rows[i].Cells["Payment No"].Value == null ? null : dataGridView1.Rows[i].Cells["Payment No"].Value.ToString());
                                        Cmd.Parameters.AddWithValue("@Payment_Amount", Convert.ToDecimal(dataGridView1.Rows[i].Cells["Payment Amount"].Value));
                                        Cmd.Parameters.AddWithValue("@Payment_DueDate", Convert.ToDateTime(dataGridView1.Rows[i].Cells["Payment DueDate"].Value));
                                        Cmd.ExecuteNonQuery();
                                    }
                                }
                                else if (dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString().ToUpper() == "TIDAK TERTAGIH")
                                {
                                    Query = "UPDATE [dbo].[CustInvoice_H] SET [Kwitansi_No] = '' WHERE [Invoice_Id] = '" + InvoiceNo + "' ";
                                    using (Cmd = new SqlCommand(Query, Conn))
                                    {
                                        Cmd.ExecuteNonQuery();
                                    }
                                }
                                string transstatus = "";
                                if (tbxStatus.Text.ToUpper() == "COLLECTION IN COLLECTOR")
                                {
                                    //Finding the invoice status
                                    string Query2 = "SELECT [PPN_Percent],[Settle_Amount],[AR_Amount] FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] = '" + InvoiceNo.ToString() + "'";
                                    using (SqlCommand Cmd2 = new SqlCommand(Query2, Conn))
                                    {
                                        SqlDataReader Dr2 = Cmd2.ExecuteReader();
                                        while (Dr2.Read())
                                        {
                                            if (Convert.ToDecimal(Dr2["AR_Amount"]) - Convert.ToDecimal(Dr2["Settle_Amount"]) != 0)
                                            {
                                                transstatus = "22";
                                            }
                                            else if (Convert.ToDecimal(Dr2["AR_Amount"]) - Convert.ToDecimal(Dr2["Settle_Amount"]) == 0)
                                            {
                                                transstatus = "21";
                                            }
                                            else if (Convert.ToDecimal(Dr2["PPN_Percent"]) != 0)
                                            {
                                                transstatus = "13";
                                            }
                                            else
                                            {
                                                transstatus = "03";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    transstatus = "09";
                                }
                                Query = "UPDATE [dbo].[CustInvoice_H] SET [TransStatus] = '"+transstatus+"' WHERE [Invoice_Id] = '" + InvoiceNo + "' ";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        Conn.Close();
                        MessageBox.Show("Data Collection berhasil di Saved");
                        Mode = "BeforeEdit";
                        Parent.RefreshAccess();
                        Parent.RefreshGrid();
                        scope.Complete();
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString());
                    return;
                }
                finally
                {
                    this.Close();
                    ARCollection.CollectionResult.CLR_Form f = new ARCollection.CollectionResult.CLR_Form();
                    f.SetMode("BeforeEdit", tbxCLNo.Text);
                    f.setParent2(Parent);
                    f.Show();
                    f.Focus();
                }
            }
        }

        private void InsertCLRLog(string status, string action)
        {
            string statusDesc = "";
            if (status != "")
            {
                Query = "SELECT [Deskripsi] FROM [dbo].[TransStatusTable] WHERE [TransCode] = 'Collection' AND [StatusCode]=@StatusCode";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@StatusCode", status);
                    statusDesc = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
                }
            }
            if (action == "")
            {
                Query = "SELECT TOP 1 [Action],[Email],[Print] FROM [dbo].[Collection_LogTable] WHERE [CL_No]=@CL_No ORDER BY LogDate DESC";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@CL_No", tbxCLNo.Text);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (action == "")
                        {
                            action = Dr["Action"].ToString();
                        }
                    }
                    Dr.Close();
                }
            }
            Query = "INSERT INTO [dbo].[Collection_LogTable] ([CL_No],[CL_Date],[Type],[Email],[Print],[Deskripsi],[Status],[StatusDescription],[Action],[LogDate],[UserID]) VALUES (";
            Query += " @CL_No,@CL_Date,@Type,@Email,@Print,@Deskripsi,@Status,@StatusDescription,@Action,getdate(),@UserID)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CL_No", tbxCLNo.Text);
                Cmd.Parameters.AddWithValue("@CL_Date", dateTimePicker1.Value);
                Cmd.Parameters.AddWithValue("@Type", "Manual");
                Cmd.Parameters.AddWithValue("@Email", false);
                Cmd.Parameters.AddWithValue("@Print", false);
                Cmd.Parameters.AddWithValue("@Deskripsi", "");
                Cmd.Parameters.AddWithValue("@Status", status);
                Cmd.Parameters.AddWithValue("@StatusDescription", statusDesc);
                Cmd.Parameters.AddWithValue("@Action", action);
                Cmd.Parameters.AddWithValue("@UserID", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = "[dbo].[CustInvoice_H]";
            F.QuerySearch = "SELECT a.*,b.Deskripsi FROM [dbo].[CustInvoice_H] a LEFT JOIN TransStatusTable b ON a.TransStatus = b.StatusCode WHERE a.AR_Amount - a.Settle_Amount != 0 AND b.TransCode = 'CustInvoice' AND a.TransStatus IN ('03','13','22') ";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                F.QuerySearch += " AND a.Invoice_Id != '" + dataGridView1.Rows[i].Cells["Invoice No"].Value.ToString() + "' ";
            }
            F.FilterText = new string[] { "[Cust_Id]", "[Cust_Name]", "Invoice_Id", "Invoice_Date", "Invoice_DueDate", "Invoice_Amount", "Kwitansi_No", "Deskripsi" };
            F.Select = new string[] { "Cust_Name", "Invoice_Id", "Invoice_Date", "Invoice_Amount", "Invoice_DueDate", "AR_Amount", "Settle_Amount", "Kwitansi_No" };
            F.PrimaryKey = "Invoice_Id";
            F.HideField = new string[] {  };
            F.Parent = this;
            F.ShowDialog();
            if (Variable.Kode2 != null)
            {
                for (int i = 0; i <= ((Variable.Kode2.GetUpperBound(0))); i++)
                {
                    dataGridView1.Rows.Add(dataGridView1.Rows.Count + 1, Variable.Kode2[i, 0], Variable.Kode2[i, 1], Convert.ToDateTime(Variable.Kode2[i, 2]).ToShortDateString(), Variable.Kode2[i, 3], Convert.ToDateTime(Variable.Kode2[i, 4]).ToShortDateString(), (Convert.ToDecimal(Variable.Kode2[i, 5]) - Convert.ToDecimal(Variable.Kode2[i, 6])), Variable.Kode2[i, 7]);
                }
            }
            Variable.Kode2 = null;
            AddKwitansiId();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        { 
            int index = dataGridView1.CurrentRow.Index;
            if (dataGridView1.Rows[index].Cells["Kwitansi No"].Value != null && dataGridView1.Rows[index].Cells["Kwitansi No"].Value.ToString() != "")
            {
                string kwitansiid = dataGridView1.Rows[index].Cells["Kwitansi No"].Value.ToString();
                for (int i = dataGridView1.Rows.Count; i > 0; i--)
                {
                    if (kwitansiid == dataGridView1.Rows[i-1].Cells["Kwitansi No"].Value.ToString())
                    {
                        dataGridView1.Rows.RemoveAt(i-1);
                    }
                }
            }
            else
            {
                dataGridView1.Rows.RemoveAt(index);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (tbxCollectionType.Text == "Email")
            {
                MessageBox.Show("Collection tidak bisa di-edit karena sudah kirim email");
                return;
            }
            else if (tbxCollectionType.Text == "Post")
            {
                MessageBox.Show("Collection tidak bisa di-edit karena sudah kirim mail letter");
                return;
            }
            else
            {
                string statuscode = "";
                Query = "SELECT [Status_Code] FROM [dbo].[Collection_H] WHERE [CL_No] = '" + tbxCLNo.Text + "' ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    statuscode = Cmd.ExecuteScalar().ToString();
                }
                if (statuscode != "03")
                {
                    Mode = "Edit";
                    ModeEdit();
                }
                else
                {
                    MessageBox.Show("Collection sudah Completed.");
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";
            ModeBeforeEdit();        
            Query = "SELECT [Collector] FROM [dbo].[Collection_H] WHERE [CL_No] = @tbxCLId ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@tbxCLId",tbxCLNo.Text);
                tbxName.Text = Cmd.ExecuteScalar().ToString();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            //if (ControlMgr.UserId == "AR Manager")
            //{
            DialogResult res = MessageBox.Show("Apakah yakin untuk Confirm? Setelah di Confirm sudah tidak dapat di-edit kembali.", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (res == DialogResult.Cancel)
            {
                return;
            }  

                bool status = true;
                Query = "SELECT Status_Code FROM [dbo].[Collection_H] WHERE [CL_No] = '" + tbxCLNo.Text + "' ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    if (Cmd.ExecuteScalar().ToString() != "01")
                    {
                        status = false;
                    }
                }
                if (status == true)
                {
                    Query = "UPDATE [dbo].[Collection_H] SET [Status_Code] = '02' WHERE [CL_No] = '" + tbxCLNo.Text + "' ";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Collection berhasil diconfirm.");
                    btnConfirm.Visible = false;
                    Query = "SELECT b.[Deskripsi] FROM [dbo].[Collection_H] a LEFT JOIN [dbo].[TransStatusTable] b ON b.[StatusCode]= a.[Status_Code] WHERE b.[TransCode] = 'Collection' AND a.[CL_No] = '" + tbxCLNo.Text + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        tbxStatus.Text = Cmd.ExecuteScalar().ToString();
                    }

                    Query = "UPDATE [dbo].[Collection_Dtl] SET [ConfirmedBy] = '" + ControlMgr.UserId + "' WHERE [CL_No] = '" + tbxCLNo.Text + "' ";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                    ModeBeforeEdit();
                    Parent.RefreshAccess();
                }
                else
                {
                    MessageBox.Show("Hanya Collection dengan status Collection initiated yang dapat di confirm.");
                }
            //}
            //else
            //{
                
            //}
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (tbxCLNo.Text == "" || tbxCLNo.Text == null)
            {
                return;
            }
            else if (dataGridView1.Rows.Count <= 0 && tbxCollectionType.Text.ToString() == "MANUAL")
            {
                MessageBox.Show("Invoice pada detail kosong.");
                return;
            }
            else
            {
                if (tbxStatus.Text != "Collection in Collector" && tbxCollectionType.Text.ToString() == "MANUAL")
                {
                    MessageBox.Show("Collection perlu di-confirm terlebih dahulu.");
                    return;
                }
                else if (tbxCollectionType.Text.ToString().ToUpper() == "POST")
                {
                    Query = "SELECT [Print_C] FROM [dbo].[Collection_H] WHERE [CL_No] = '"+tbxCLNo.Text+"'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        if (Convert.ToBoolean(Cmd.ExecuteScalar()) == true && ControlMgr.GroupName.ToUpper() != "AR MANAGER")
                        {
                            MessageBox.Show("Re-Print membutuhkan approval dari AR Manager.");
                            return;
                        }
                        else
                        {
                            Collection.CollectionPopUpInputMailAddress PopUp = new CollectionPopUpInputMailAddress(this, tbxCLNo.Text);
                            PopUp.ShowDialog();
                        }
                    }
                }
                else
                {
                    string Query = "UPDATE [dbo].[Collection_H] SET [Print_C] = 1, [Print_Ctr] += 1 WHERE [CL_No]='" + tbxCLNo.Text + "'";
                    using (SqlCommand cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                if (tbxCollectionType.Text.ToString().ToUpper() == "MANUAL")
                {
                    ARCollection.Collection.PreviewCollectionPrintOut F = new PreviewCollectionPrintOut(tbxCLNo.Text, "Manual");
                    F.ShowDialog();
                }
            }
        }

        //stv edit start
        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            string CLNumber = tbxCLNo.Text;

            Query = "Select [Email_C] From [Collection_H] Where [CL_No] = '" + CLNumber + "' ";
            Cmd = new SqlCommand(Query, Conn);
            int SendEmailC = Convert.ToInt32(Cmd.ExecuteScalar());

            Query = "Select [CustId] From [Collection_Dtl] Where [CL_No] = '" + CLNumber + "' ";
            Cmd = new SqlCommand(Query, Conn);
            string CustomerId = Cmd.ExecuteScalar().ToString();

            if (SendEmailC == 0)
            {
                ISBS_New.ARCollection.SendEmail s = new ISBS_New.ARCollection.SendEmail();
                s.flag(CLNumber, CustomerId, "Resend");
                s.ShowDialog(); 
            }
            else
            {
                if (ControlMgr.GroupName == "Administrator")
                {
                    DialogResult resSendEmail = MessageBox.Show(CLNumber + Environment.NewLine + "Email already been Sent!" + Environment.NewLine + "Allow Email to be sent again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (resSendEmail == DialogResult.Yes)
                    {
                        Query = "Update [dbo].[Collection_H] Set [Email_C] = '0' Where CL_No = '" + CLNumber + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        #region Ask Resend Email Now?
                        DialogResult resSendEmailPrompt = MessageBox.Show("Do you want to resend email now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (resSendEmailPrompt == DialogResult.Yes)
                        {
                            ISBS_New.ARCollection.SendEmail s = new ISBS_New.ARCollection.SendEmail();
                            s.flag(CLNumber, CustomerId, "Resend");
                            s.ShowDialog(); 
                        }
                        if (resSendEmailPrompt == DialogResult.No)//Action not sending email immidiately
                        {
                            //Some task…  
                        }
                        #endregion
                    }
                    if (resSendEmail == DialogResult.No) //Action not allow email to be sent
                    {
                        //Some task…  
                    }
                }
                else
                {
                    MessageBox.Show("Email already been Sent!" + Environment.NewLine + "Contact Administrator");
                }
            }
            Conn.Close();
        }
        //stv edit end
        private void btnApprove_Click(object sender, EventArgs e)
        {
            
            if (tbxCollectionType.Text.ToString().ToUpper() == "POST")
            {
                Query = "UPDATE [dbo].[Collection_H] SET [Print_C]=false WHERE [CL_No] = '"+tbxCLNo.Text+"'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        string ResultChanged = "";
        DateTimePicker dtp = new DateTimePicker();
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //adding date time picker to datagridview
            if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Payment Date" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Payment DueDate")
            {
                dataGridView1.Controls.Add(dtp);
                dtp.Format = DateTimePickerFormat.Short;
                Rectangle oRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                dtp.Location = new Point(oRectangle.X, oRectangle.Y);
                dtp.Size = new Size(oRectangle.Width, oRectangle.Height); 
                dtp.Visible = true;

                // An event attached to dateTimePicker Control which is fired when DateTimeControl is closed  
                dtp.CloseUp += new EventHandler(oDateTimePicker_CloseUp);

                // An event attached to dateTimePicker Control which is fired when any date is selected  
                dtp.TextChanged += new EventHandler(dateTimePicker_OnTextChange);

                if (dataGridView1.CurrentCell.Value != "" && dataGridView1.CurrentCell.Value != null)
                {
                    DateTime dDate;
                    if (!DateTime.TryParse(dataGridView1.CurrentCell.Value.ToString(), out dDate))
                    {
                        //dtp.Value = Convert.ToDateTime(FormateDateddmmyyyy(dgvPrDetails.CurrentCell.Value.ToString()));
                    }
                    else
                    {
                        dtp.Value = Convert.ToDateTime(dataGridView1.CurrentCell.Value);
                    }
                }
                else
                {
                    //dtp.Value = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy"));
                }
            }
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "cmbResult")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["cmbResult"].Value != null)
                {
                    ResultChanged = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["cmbResult"].Value.ToString();
                }
            }
        }

        private void ColorandReadOnlyColumns_BeforeEditMode()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString().ToUpper() == "TIDAK TERTAGIH" || dataGridView1.Rows[i].Cells["cmbResult"].Value.ToString().ToUpper() == "TUKAR BON")
                {
                    for (int x = 0; x < dataGridView1.Columns.Count; x++)
                    {
                        if (dataGridView1.Columns[x].Name == "Comments" || dataGridView1.Columns[x].Name == "cmbResult")
                        {
                            dataGridView1.Rows[i].Cells[x].ReadOnly = false;
                            dataGridView1.Rows[i].Cells[x].Style.BackColor = Color.White;
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[x].ReadOnly = true;
                            dataGridView1.Rows[i].Cells[x].Style.BackColor = Color.LightGray;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        for (int x = 0; x < readOnlyNames.Count(); x++)
                        {
                            if (dataGridView1.Columns[j].Name == readOnlyNames[x])
                            {
                                dataGridView1.Rows[i].Cells[j].ReadOnly = false;
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                                break;
                            }
                            else
                            {
                                dataGridView1.Rows[i].Cells[j].ReadOnly = true;
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                            }
                        }
                    }
                }
            }
        }

        private void Result_OnTextChange()
        {
            int currentrow = dataGridView1.CurrentCell.RowIndex;
            if (dataGridView1.CurrentCell.Value.ToString().ToUpper() == "TIDAK TERTAGIH" || dataGridView1.CurrentCell.Value.ToString().ToUpper() == "TUKAR BON")
            {
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    if (dataGridView1.Columns[i].Name == "Comments" || dataGridView1.Columns[i].Name == "cmbResult")
                    {
                        dataGridView1.Rows[currentrow].Cells[i].ReadOnly = false;
                        dataGridView1.Rows[currentrow].Cells[i].Style.BackColor = Color.White;
                    }
                    else
                    {
                        dataGridView1.Rows[currentrow].Cells[i].ReadOnly = true;
                        dataGridView1.Rows[currentrow].Cells[i].Style.BackColor = Color.LightGray;
                    }
                    if (dataGridView1.Columns[i].Name == "cmbPaymentMethod" || dataGridView1.Columns[i].Name == "Payment Date" || dataGridView1.Columns[i].Name == "Payment DueDate" || dataGridView1.Columns[i].Name == "Payment No")
                    {
                        dataGridView1.Rows[currentrow].Cells[i].Value = "";
                    }
                    if (dataGridView1.Columns[i].Name == "Payment Amount")
                    {
                        dataGridView1.Rows[currentrow].Cells[i].Value = "0.0000";
                    }
                }
            }
            else
            {
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    for (int x = 0; x < readOnlyNames.Count(); x++)
                    {
                        if (dataGridView1.Columns[i].Name == readOnlyNames[x])
                        {
                            dataGridView1.Rows[currentrow].Cells[i].ReadOnly = false;
                            dataGridView1.Rows[currentrow].Cells[i].Style.BackColor = Color.White;
                            break;
                        }
                        else
                        {
                            dataGridView1.Rows[currentrow].Cells[i].ReadOnly = true;
                            dataGridView1.Rows[currentrow].Cells[i].Style.BackColor = Color.LightGray;
                        }
                    }
                }
            }
        }
        private void dateTimePicker_OnTextChange(object sender, EventArgs e)
        {
            // Saving the 'Selected Date on Calendar' into DataGridView current cell  
            dataGridView1.CurrentCell.Value = dtp.Text.ToString();
        }
        private void oDateTimePicker_CloseUp(object sender, EventArgs e)
        {
            // Hiding the control after use   
            dtp.Visible = false;
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Payment Date" || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Payment DueDate")
            {
                e.Handled = true;
            }
            else if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Payment Amount")
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

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Payment Date" || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Payment DueDate" || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Payment Amount")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dataGridView1_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dataGridView1_KeyPress);
            }

            //resize the cmbbox in datagridview
            ComboBox cmb = e.Control as ComboBox;
            if (cmb != null)
            {
                cmb.DropDown -= new EventHandler(cmb_DropDown);
                cmb.DropDown += new EventHandler(cmb_DropDown);
            }
        }

        private void cmb_DropDown(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            int width = cmb.DropDownWidth;
            Graphics g = cmb.CreateGraphics();
            Font font = cmb.Font;
            int vertScrollBarWidth = 0;
            if (cmb.Items.Count > cmb.MaxDropDownItems)
            {
                vertScrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            }

            int maxWidth;
            foreach (string s in cmb.Items)
            {
                maxWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;
                if (width < maxWidth)
                {
                    width = maxWidth;
                }
            }

            DataGridViewComboBoxColumn c =  this.dataGridView1.Columns["cmbResult"] as DataGridViewComboBoxColumn;
            DataGridViewComboBoxColumn c2 = this.dataGridView1.Columns["cmbPaymentMethod"] as DataGridViewComboBoxColumn;
            if (c != null)
            {
                c.Width = width;
            }
            if (c2 != null)
            {
                c2.Width = width;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Payment Amount")
            {
                int currentRow = dataGridView1.CurrentCell.RowIndex;
                if (dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value != null)
                {
                    if (dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value.ToString() == "")
                    {
                        dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value = "0.0000";
                    }
                    else if (dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value.ToString().IndexOf(".") == 1)
                    {
                        dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value = String.Format("{0:#,##0.###0}", Convert.ToDecimal("0" + dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value));
                    }
                    else
                    {
                        dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value = String.Format("{0:#,##0.###0}", Convert.ToDecimal(dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value));
                    }
                }
                else
                {
                    dataGridView1.Rows[currentRow].Cells["Payment Amount"].Value = "0.0000";
                }
            }
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "cmbResult")
            {
                if (dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["cmbResult"].Value != null)
                {
                    if (dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["cmbResult"].Value.ToString() != ResultChanged)
                    {
                        Result_OnTextChange();
                    }
                }
            }
        }

        //tia edit
        //klik kanan
        PopUp.CustomerID.Customer Cust = null;
        AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice ARId = null;
        ARCollection.Kwitansi TT = null;

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

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                //Cust belum bener
                if (Cust == null || Cust.Text == "")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Customer")
                    {
                        Cust = new PopUp.CustomerID.Customer();
                        Cust.GetData(dataGridView1.Rows[e.RowIndex].Cells["Customer"].Value.ToString());
                        Cust.Show();
                    }
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.GetData(dataGridView1.Rows[e.RowIndex].Cells["Customer"].Value.ToString());
                    Cust.Show();
                    Cust.Focus();
                }
                if (ARId == null || ARId.Text == "")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Invoice No")
                    {
                        ARId = new  AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice();
                        ARId.SetMode("BeforeEdit", dataGridView1.CurrentRow.Cells["Invoice No"].Value.ToString());
                        ARId.Show();
                    }
                }
                else if (CheckOpened(ARId.Name))
                {
                    ARId.WindowState = FormWindowState.Normal;
                    ARId.SetMode("BeforeEdit", dataGridView1.CurrentRow.Cells["Invoice No"].Value.ToString());
                    ARId.Show();
                    ARId.Focus();
                }
                if (TT == null || TT.Text == "")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "Kwitansi No")
                    {
                        TT = new ARCollection.Kwitansi();
                        TT.ModeBeforeEdit();
                        TT.GetDataHeader(dataGridView1.CurrentRow.Cells["Kwitansi No"].Value.ToString());
                        TT.Show();
                    }
                }
                else if (CheckOpened(ARId.Name))
                {
                    TT.WindowState = FormWindowState.Normal;
                      TT.ModeBeforeEdit();
                      TT.GetDataHeader(dataGridView1.CurrentRow.Cells["Kwitansi No"].Value.ToString());
                    TT.Show();
                    TT.Focus();
                }

            }
        }  
        //tia edit end
    }
}

    