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
    public partial class CollectionPopUpInputMailAddress : MetroFramework.Forms.MetroForm
    {
        Collection.CollectionTaskList Parent;
        Collection.CollectionHeader Parent2;
        SqlConnection Conn;
        SqlCommand Cmd;
        SqlDataReader Dr;
        TransactionScope scope;
        string Query;
        string InvoiceId;
        string ColletionId;

        public CollectionPopUpInputMailAddress(CollectionTaskList Form, string PassedId)
        {
            InitializeComponent();
            Parent = Form;
            InvoiceId = PassedId;
        }

        public CollectionPopUpInputMailAddress(CollectionHeader Form, string PassedId)
        {
            InitializeComponent();
            Parent2 = Form;
            ColletionId = PassedId;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text == "")
            {
                MessageBox.Show("Alamat masih kosong.");
                return;
            }
        }

        private void GetHeader()
        {
            if (InvoiceId != null)
            {
                Query = "SELECT * FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] = '" + InvoiceId + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        txtCustId.Text = Dr["Cust_Id"].ToString();
                        txtCustName.Text = Dr["Cust_Name"].ToString();
                    }
                    Dr.Close();
                }
            }
            else if(ColletionId != null)
            {
                Query = "SELECT * FROM [dbo].[Collection_Dtl] WHERE [CL_No] = '" + ColletionId + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        txtCustId.Text = Dr["CustId"].ToString();
                        txtCustName.Text = Dr["Cust_Name"].ToString();
                    }
                    Dr.Close();
                }
            }
        }

        private void GetDetail()
        {
            string InvoiceNo = "";
            if (InvoiceId == null)
            {
                Query = "SELECT [Invoice_Id] FROM [dbo].[Collection_Dtl] WHERE [CL_No] = '"+ColletionId+"'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        InvoiceNo = Dr["Invoice_Id"] == null ? "" : Dr["Invoice_Id"].ToString();
                    }
                    Dr.Close();
                }
            }
            if (InvoiceId != null || InvoiceNo != "")
            {
                string Currency = "";
                groupBox2.Text = "Invoice Info";
                InvoiceNo = InvoiceId;
                dataGridView1.ColumnCount=10;
                dataGridView1.Columns[0].Name = "Invoice Date";
                dataGridView1.Columns[1].Name = "Invoice Id";
                dataGridView1.Columns[2].Name = "Invoice Amount";
                dataGridView1.Columns[3].Name = "Invoice DueDate";
                dataGridView1.Columns[4].Name = "Notes";
                dataGridView1.Columns[5].Name = "CN Amount";
                dataGridView1.Columns[6].Name = "AR Amount";
                dataGridView1.Columns[7].Name = "Settle Amount";
                dataGridView1.Columns[8].Name = "Settle Invoice DP";
                dataGridView1.Columns[9].Name = "Kwitansi No";

                Query = "SELECT * FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id] = '" + InvoiceNo + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        Currency = Dr["CurrencyID"].ToString();
                        dataGridView1.Rows.Add(Convert.ToDateTime(Dr["Invoice_Date"]).ToString("dd/MM/yyyy"), Dr["Invoice_Id"], Dr["Invoice_Amount"], Convert.ToDateTime(Dr["Invoice_DueDate"]).ToString("dd/MM/yyyy"), Dr["Notes"], Dr["CN_Amount"], Dr["AR_Amount"], Dr["Settle_Amount"], Dr["Settle_Invoice_DP"], Dr["Kwitansi_No"]);
                    }
                    Dr.Close();
                }

                decimal SettleAmount = Convert.ToDecimal(dataGridView1.Rows[0].Cells["Settle Amount"].Value);
                decimal ARAmount = Convert.ToDecimal(dataGridView1.Rows[0].Cells["AR Amount"].Value);
                string DueDate = dataGridView1.Rows[0].Cells["Invoice DueDate"].Value.ToString();
                string Date = dataGridView1.Rows[0].Cells["Invoice Date"].Value.ToString();
                txtBodyText.Text = "";
                txtBodyText.Text += "Berdasarkan catatan keuangan kami pertanggal " + Date + ". Saudara " + txtCustName.Text + " memiliki tunggakan sebesar " + String.Format("{0:#,##0.###0}", (ARAmount - SettleAmount)) + " " + Currency + " atas Invoice " + InvoiceNo + ".\n\n";
                txtBodyText.AppendText(Environment.NewLine);
                txtBodyText.Text += "Oleh karena itu kami selaku pihak penagih yaitu PT. Intisumber Bajasakti atas Invoice dengan nomor "+InvoiceNo+", meminta kpeada pihak yang ditagih yaitu "+txtCustName.Text+" untuk pelunasannya sesuai dengan perjanjian yang ada.\n";
                txtBodyText.AppendText(Environment.NewLine);
                txtBodyText.Text += "Dengan nilai tagihan sebesar "+String.Format("{0:#,##0.###0}",(ARAmount - SettleAmount))+" "+Currency+" dan tanggal jatuh tempo pada "+DueDate+".\n";
                txtBodyText.AppendText(Environment.NewLine);
                txtBodyText.Text += "Atas kerjasamanya dan pengertiannya kami ucapkan terima kasih.\n";
            }
        }

        private void CallPreview()
        {

            string ClNo = "";
            try
            {
                using (scope = new TransactionScope())
                {
                    if (InvoiceId != null)
                    {
                        ClNo = GenerateId();
                        InsertCollectionH(ClNo);
                        InsertCollectionDtl(ClNo);
                        InsertCLRLog(ClNo);
                        scope.Complete();
                    }
                    else if (ColletionId != null)
                    {
                        ClNo = ColletionId;
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
            finally
            {
                ARCollection.Collection.PreviewCollectionPrintOut F = new PreviewCollectionPrintOut(ClNo, "Post", txtBodyText.Text);
                F.ShowDialog();
            }
        }

        private void InsertCLRLog(string ClId)
        {
            string action = "";
            bool email = false;
            DateTime cldate = new DateTime();
            Query = "SELECT TOP 1 CL_Date,Type,[Action],[Email],[Print] FROM [dbo].[Collection_LogTable] WHERE [CL_No]=@CL_No ORDER BY LogDate DESC";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CL_No", ClId);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        action = Dr["Action"].ToString();
                        email = Convert.ToBoolean(Dr["Print"]);
                        cldate = Convert.ToDateTime(Dr["CL_Date"]);
                    }
                }
                else
                {
                    action = "N";
                    email = false;
                    cldate = DateTime.Now;
                }
                Dr.Close();
            }

            Query = "INSERT INTO [dbo].[Collection_LogTable] ([CL_No],[CL_Date],[Type],[Email],[Print],[Deskripsi],[Status],[StatusDescription],[Action],[LogDate],[UserID]) VALUES (";
            Query += " @CL_No,getdate(),@Type,@Email,@Print,@Deskripsi,@Status,@StatusDescription,@Action,getdate(),@UserID)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CL_No", ClId);
                Cmd.Parameters.AddWithValue("@Type", "Post");
                Cmd.Parameters.AddWithValue("@Email", email);
                Cmd.Parameters.AddWithValue("@Print", true);
                Cmd.Parameters.AddWithValue("@Deskripsi", "Print");
                Cmd.Parameters.AddWithValue("@Status", "03");
                Cmd.Parameters.AddWithValue("@StatusDescription", "Completed");
                Cmd.Parameters.AddWithValue("@Action", action);
                Cmd.Parameters.AddWithValue("@UserID", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        private string GenerateId()
        {
            Conn = ConnectionString.GetConnection();
            string Jenis = "CL", Kode = "CL";
            string CLNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);
            Conn.Close();
            return CLNumber;
        }

        private void InsertCollectionH(string Clnumber)
        {
            Query = "INSERT INTO [dbo].[Collection_H] ([CL_Date],[CL_No],[CL_Type],[Mail_Address],[Print_C],[Print_Ctr],[Status_Code],[CreatedDate],[CreatedBy]) ";
            Query += " VALUES (@CL_Date,@CL_No,@CL_Type,@Mail_Address,@Print_C,@Print_Ctr,@Status_Code,@CreatedDate,@CreatedBy)";
            using (SqlCommand cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@CL_Date", DateTime.Now);
                cmd.Parameters.AddWithValue("@CL_No", Clnumber);
                cmd.Parameters.AddWithValue("@CL_Type", "Post");
                cmd.Parameters.AddWithValue("@Mail_Address", txtAddress.Text);
                cmd.Parameters.AddWithValue("@Print_C", 1);
                cmd.Parameters.AddWithValue("@Print_Ctr", 1);
                cmd.Parameters.AddWithValue("@Status_Code", "03");
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                cmd.ExecuteNonQuery();
            }
        }

        private void InsertCollectionDtl(string ClNumber)
        {
            Query = "INSERT INTO [dbo].[Collection_Dtl] ([CL_No],[SeqNo],[CustId],[Cust_Name],[Invoice_Id],[TT_No],[CreatedDate],[CreatedBy]) ";
            Query += " VALUES (@CL_No,@SeqNo,@CustId,@Cust_Name,@Invoice_Id,@TT_No,@CreatedDate,@CreatedBy)";
            using (SqlCommand cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@CL_No", ClNumber);
                cmd.Parameters.AddWithValue("@SeqNo", "1");
                cmd.Parameters.AddWithValue("@CustId", txtCustId.Text);
                cmd.Parameters.AddWithValue("@Cust_Name", txtCustName.Text);
                cmd.Parameters.AddWithValue("@Invoice_Id", dataGridView1.Rows[0].Cells["Invoice Id"].Value.ToString());
                cmd.Parameters.AddWithValue("@TT_No", "");
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnSearchAddress_Click(object sender, EventArgs e)
        {

            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = "[dbo].[Address]";
            F.QuerySearch = "SELECT * FROM [dbo].[Address] WHERE [PurposeType] = 'OFFICE' AND [ReffID] = '"+txtCustId.Text+"'";
            F.FilterText = new string[] { "Name", "Address", "Provinsi", "Kota" };
            F.Select = new string[] { "Address","Provinsi","Kota","Kode_Pos","RT","RW"};
            F.Mask = new string[] { };
            F.PrimaryKey = "Address";
            F.HideField = new string[] { "Check" };
            F.Parent = this;
            F.ShowDialog();
            if (Variable.Kode2 != null)
            {
                txtAddress.Text = Variable.Kode2[0, 0];
                txtProvinsi.Text = Variable.Kode2[0, 1];
                txtKota.Text = Variable.Kode2[0, 2];
                txtKodePos.Text = Variable.Kode2[0, 3];
                txtRT.Text = Variable.Kode2[0, 4];
                txtRW.Text = Variable.Kode2[0, 5];
            }
            Variable.Kode2 = null;
        }

        private void CollectionPopUpInputMailAddress_Load(object sender, EventArgs e)
        {
            GetHeader();
            GetDetail();
        }
    }
}
