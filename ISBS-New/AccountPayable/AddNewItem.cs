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

namespace ISBS_New.AccountPayable
{
    public partial class AddNewItem : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        DataGridView dgvDetailFromHeader;
        AccountPayable.HeaderAccountsPayable Parent;
       
        string terimaVendAccount;
        DataTable passedtable = new DataTable();

       
        public AddNewItem(string vendorAccount)
        {
            terimaVendAccount = vendorAccount;
            InitializeComponent();
        }
        
        private void AddNewItem_Load(object sender, EventArgs e)
        {
            refreshgrid();
        }

        private void refreshgrid()
        {
            if (Parent.cmbInvoiceType.Text != "")
            {
                Conn = ConnectionString.GetConnection();
                string query = "";

                if (Parent.cmbInvoiceType.Text == "Invoice")
                {
                    query = "select PONo, PODate, POAmount, PODueDate, GRNo, DPRequired, DPPercent, DPAmount, DPOutstanding, POInvoiced, POUnInvoice, GRAmount, PotDP, GRPayable from PO_UnInvoiced_GR_View ";//where VendId='" + Parent.txtVendAccount.Text + "'";
                }
                else if (Parent.cmbInvoiceType.Text == "Uang Muka")
                {
                    query = "select PONo, PODate, POAmount, PODueDate, DPRequired, DPPercent, DPAmount, DPDeduct, DPOutstanding, POInvoiced, POUnInvoice from PO_UnInvoiced_DP_View ";//where VendId='" + Parent.txtVendAccount.Text + "'";
                }

                Da = new SqlDataAdapter(query, Conn);
                dgvItem.Columns.Clear();
                DataTable namatable = new DataTable();
                if (dgvItem.Columns.Contains("chk") == false)
                {
                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                    dgvItem.Columns.Add(chk);
                    chk.HeaderText = "Check";
                    chk.Name = "chk";
                }
                Da.Fill(namatable);
                dgvItem.DataSource = namatable;
                dgvItem.AllowUserToAddRows = false;
                dgvItem.AutoResizeColumns();

                //tia 040518
                //remove?
                string PurchIdH = "";
                string PurchId = "";
                List<string> RemovePurchId = new List<string>();

                for (int i = 0; i < dgvItem.RowCount; i++)
                {
                    PurchId = dgvItem.Rows[i].Cells["PONo"].Value.ToString();

                    for (int j = 0; j < dgvDetailFromHeader.RowCount; j++)
                    {
                        PurchIdH = dgvDetailFromHeader.Rows[j].Cells["PONo"].Value.ToString();

                        if (PurchIdH == PurchId)
                        {
                            RemovePurchId.Add(PurchId);
                        }
                    }
                }

                for (int i = 0; i < RemovePurchId.Count; i++)
                {
                    for (int j = 0; j < dgvItem.RowCount; j++)
                    {
                        PurchId = dgvItem.Rows[j].Cells["PONo"].Value.ToString();
                        if (PurchId == RemovePurchId[i])
                        {
                            dgvItem.Rows.RemoveAt(j);
                        }
                    }
                }

                //close
                Conn.Close();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        public void ParamHeader(DataGridView prmDgvDetailFromHeader)
        {
            dgvDetailFromHeader = prmDgvDetailFromHeader;
        }

        public void SetParentForm(AccountPayable.HeaderAccountsPayable P)
        {
            Parent = P;
        } 
        private void MethodSelectData()
        {
            if (Parent.cmbInvoiceType.Text == "Invoice")
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

                for (int i = 0; i <= dgvItem.RowCount - 1; i++)
                {
                    Boolean Check = Convert.ToBoolean(dgvItem.Rows[i].Cells["chk"].Value);
                    if (Check == true)
                    {
                        PONo.Add(dgvItem.Rows[i].Cells["PONo"].Value.ToString());
                        PODate.Add(dgvItem.Rows[i].Cells["PODate"].Value.ToString());
                        POAmount.Add(dgvItem.Rows[i].Cells["POAmount"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["POAmount"].Value.ToString());
                        PODueDate.Add(dgvItem.Rows[i].Cells["PODueDate"].Value.ToString());
                        PODueDate.Add(dgvItem.Rows[i].Cells["GRNo"].Value.ToString());
                        DPAmount.Add(dgvItem.Rows[i].Cells["DPAmount"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["DPAmount"].Value.ToString());

                        DPOutstanding.Add(dgvItem.Rows[i].Cells["DPOutstanding"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["DPOutstanding"].Value.ToString());
                        POInvoiced.Add(dgvItem.Rows[i].Cells["POInvoiced"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["POInvoiced"].Value.ToString());
                        POUnInvoiced.Add(dgvItem.Rows[i].Cells["POUnInvoice"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["POUnInvoice"].Value.ToString());
                        GRAmount.Add(dgvItem.Rows[i].Cells["GRAmount"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["GRAmount"].Value.ToString());

                        PotDP.Add(dgvItem.Rows[i].Cells["PotDP"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["PotDP"].Value.ToString());
                        GRPayable.Add(dgvItem.Rows[i].Cells["GRPayable"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["GRPayable"].Value.ToString());
                        
                    }
                }
                Parent.AddDataGridInvoice(PONo, PODate, POAmount, PODueDate, GRNo, DPAmount, DPOutstanding, POInvoiced, POUnInvoiced, GRAmount, PotDP, GRPayable);
            }
            else if (Parent.cmbInvoiceType.Text == "Uang Muka")
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

                for (int i = 0; i <= dgvItem.RowCount - 1; i++)
                {
                    Boolean Check = Convert.ToBoolean(dgvItem.Rows[i].Cells["chk"].Value);
                    if (Check == true)
                    {
                        PONo.Add(dgvItem.Rows[i].Cells["PONo"].Value.ToString());
                        PODate.Add(dgvItem.Rows[i].Cells["PODate"].Value.ToString());
                        POAmount.Add(dgvItem.Rows[i].Cells["POAmount"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["POAmount"].Value.ToString());
                        PODueDate.Add(dgvItem.Rows[i].Cells["PODueDate"].Value.ToString());
                        DPRequired.Add(dgvItem.Rows[i].Cells["DPRequired"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["DPRequired"].Value.ToString());

                        DPPercent.Add(dgvItem.Rows[i].Cells["DPPercent"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["DPPercent"].Value.ToString());
                        DPAmount.Add(dgvItem.Rows[i].Cells["DPAmount"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["DPAmount"].Value.ToString());
                        DPDeduct.Add(dgvItem.Rows[i].Cells["DPDeduct"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["DPDeduct"].Value.ToString());
                        DPOutstanding.Add(dgvItem.Rows[i].Cells["DPOutstanding"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["DPOutstanding"].Value.ToString());

                        POInvoice.Add(dgvItem.Rows[i].Cells["POInvoiced"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["POInvoiced"].Value.ToString());
                        POUnInvoiced.Add(dgvItem.Rows[i].Cells["POUnInvoice"].Value.ToString() == "" ? "0.000" : dgvItem.Rows[i].Cells["POUnInvoice"].Value.ToString());
}
                }
                Parent.AddDataGridUangMuka(PONo, PODate, POAmount, PODueDate, DPRequired, DPPercent, DPAmount, DPDeduct, DPOutstanding, POInvoice, POUnInvoiced);
            }
            
            
            Parent.AddReturNotaBeli();
            this.Close();
          
            Parent.WarnaGrid();
            Parent.InputNilai();
        }
        
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            MethodSelectData();
        }
    }
}

