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

namespace ISBS_New.AccountsReceivable.CustomerInvoice
{
    public partial class AddNewItemSOGI : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        DataGridView dgvDetailFromHeader;
        AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice Parent;       
        string CustID, PPN, InvoiceType, crit, SONo;
       
        public AddNewItemSOGI(string prmCustID, string prmPPN, string prmInvoiceType, string prmSoNo)
        {
            CustID = prmCustID;
            PPN = prmPPN;
            InvoiceType = prmInvoiceType;
            SONo = prmSoNo;
            InitializeComponent();
        }
        
        private void AddNewItemSOGI_Load(object sender, EventArgs e)
        {
            RefreshGrid();
            SetCmbCriteria();
        }

        private void SetCmbCriteria()
        {
            cmbCriteria.Items.Clear();
            cmbCriteria.Items.Add("All");
            //cmbCriteria.Items.Add("SONo");
            cmbCriteria.Items.Add("SODate");
            cmbCriteria.Items.Add("SOReference");

            if (InvoiceType == "Invoice")
            {
                cmbCriteria.Items.Add("GINo");
                cmbCriteria.Items.Add("GIDate");            
            }
           
            cmbCriteria.SelectedIndex = 0;
        }

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            string ViewName = "";
            if (InvoiceType == "Invoice")
            {
                ViewName = "vSalesUnInvoiceTable_GIView";
            }
            else
            {
                ViewName = "vSalesUnInvoiceTable_DPView";            
            }

            string query = "";
            if (crit == null)
            {
                query = "SELECT * FROM " + ViewName + " WHERE CustID = '" + CustID + "' AND PPN = '" + PPN + "' AND SONo = '" + SONo + "' ";
            }
            else if (crit == "All")
            {
                query = "SELECT * FROM " + ViewName + " WHERE CustID = '" + CustID + "' AND PPN = '" + PPN + "' AND SONo = '" + SONo + "' ";
                if (InvoiceType != "Invoice")
                {
                    // query += "AND (SONo LIKE '%" + txtSearch.Text + "%' OR SOReference LIKE '%" + txtSearch.Text + "%') ";
                    query += "AND (SOReference LIKE '%" + txtSearch.Text + "%') ";
                }
                else
                {
                    //query += "AND (SONo LIKE '%" + txtSearch.Text + "%' OR SOReference LIKE '%" + txtSearch.Text + "%' OR GINo LIKE '%" + txtSearch.Text + "%') ";
                    query += "AND (SOReference LIKE '%" + txtSearch.Text + "%' OR GINo LIKE '%" + txtSearch.Text + "%') ";
              
                }
            }
            //else if (crit.Equals("SONo"))
            //{
            //    query = "SELECT * FROM " + ViewName + " WHERE CustID = '" + CustID + "' AND PPN = '" + PPN + "' ";
            //    query += "AND SONo LIKE '%"+txtSearch.Text+"%'";
            //}
            else if (crit.Equals("SODate"))
            {
                query = "SELECT * FROM " + ViewName + " WHERE CustID = '" + CustID + "' AND PPN = '" + PPN + "' AND SONo = '" + SONo + "' ";
                query += "AND (CONVERT(VARCHAR(10),SODate,120) >= '" + dtFromDate.Value.Date.ToString("yyyy-MM-dd") + "' AND CONVERT(VARCHAR(10),SODate,120) <= '" + dtToDate.Value.Date.ToString("yyyy-MM-dd") + "') ";
            }
            else if (crit.Equals("SOReference")) 
            {
                query = "SELECT * FROM " + ViewName + " WHERE CustID = '" + CustID + "' AND PPN = '" + PPN + "' AND SONo = '" + SONo + "' ";
                query += "AND SOReference LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("GINo"))
            {
                query = "SELECT * FROM " + ViewName + " WHERE CustID = '" + CustID + "' AND PPN = '" + PPN + "' AND SONo = '" + SONo + "' ";
                query += "AND GINo LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit.Equals("GIDate"))
            {
                query = "SELECT * FROM " + ViewName + " WHERE CustID = '" + CustID + "' AND PPN = '" + PPN + "' AND SONo = '" + SONo + "' ";
                query += "AND (CONVERT(VARCHAR(10),GIDate,120) >= '" + dtFromDate.Value.Date.ToString("yyyy-MM-dd") + "' AND CONVERT(VARCHAR(10),GIDate,120) <= '" + dtToDate.Value.Date.ToString("yyyy-MM-dd") + "') ";
            }

            if (InvoiceType != "Invoice")
            {
                query += "AND SOUnInvoiced <> 0 ";}
            else
            {
                query += "AND GINo NOT IN (SELECT D.GI_No FROM CustInvoice_Dtl_SO D INNER JOIN CustInvoice_H H ON H.Invoice_Id = D.Invoice_Id AND H.Cust_Id = '" + CustID + "')";          
            }

            string SOHeader = "";
            string GIHeader = "";
            for (int i = 0; i < dgvDetailFromHeader.RowCount; i++)
            {
                if (InvoiceType == "Invoice")
                {
                    GIHeader = GIHeader + "'" + dgvDetailFromHeader.Rows[i].Cells["GINo"].Value.ToString() + "',";
                }
                else
                {
                    SOHeader = SOHeader + "'" + dgvDetailFromHeader.Rows[i].Cells["SONo"].Value.ToString() + "',";
                }
            }

            if (dgvDetailFromHeader.RowCount > 0)
            {    
                if (InvoiceType == "Invoice")
                {
                    if (GIHeader != "")
                    {
                        GIHeader = GIHeader.Remove(GIHeader.Length - 1);
                        query += "AND GINo NOT IN (" + GIHeader + ") ";
                    }                    
                }
                else
                {
                    if (SOHeader != "")
                    {
                        SOHeader = SOHeader.Remove(SOHeader.Length - 1);
                        query += "AND SONo NOT IN (" + SOHeader + ") ";
                    }                   
                }
            }   

            Da = new SqlDataAdapter(query, Conn);
            dgvItem.Columns.Clear();
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                if (dgvItem.Columns.Contains("chk") == false)
                {
                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                    dgvItem.Columns.Add(chk);
                    chk.HeaderText = "Check";
                    chk.Name = "chk";
                }               
            }           
           
            dgvItem.DataSource = Dt;
            dgvItem.AllowUserToAddRows = false;
            dgvItem.AutoResizeColumns();

            dgvItem.Columns[0].ReadOnly = false;

            dgvItem.Columns["CustID"].Visible = false;
            dgvItem.Columns["CustName"].Visible = false;
            dgvItem.Columns["InvoiceTaxBaseAmount"].Visible = false;
            dgvItem.Columns["InvoiceTaxAmount"].Visible = false;
            dgvItem.Columns["SODueDate"].Visible = false;
            dgvItem.Columns["SODate"].DefaultCellStyle.Format = "dd/MM/yyyy";

            int j = 21;
            if (InvoiceType == "Invoice")
            {
                j = 23;
                dgvItem.Columns["GIDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            }

            for (int i = 1; i < j; i++)
            {
                dgvItem.Columns[i].ReadOnly = true;
            }          
          
            //string SONoH = "";
            //string SoNo = "";
            //string GINoH = "";
            //string GINo = "";
            //Dictionary<string, string> RemoveSoNo = new Dictionary<string, string>();

            //for (int i = 0; i < dgvItem.RowCount; i++)
            //{
            //    SoNo = dgvItem.Rows[i].Cells["SONo"].Value.ToString();

            //    if (InvoiceType == "Invoice")
            //    {
            //        GINo = dgvItem.Rows[i].Cells["GINo"].Value.ToString();
            //    }

            //    for (int j = 0; j < dgvDetailFromHeader.RowCount; j++)
            //    {
            //        SONoH = dgvDetailFromHeader.Rows[j].Cells["SONo"].Value.ToString();
            //        if (InvoiceType == "Invoice")
            //        {
            //            GINoH = dgvDetailFromHeader.Rows[j].Cells["GINo"].Value.ToString();
            //        }

            //        if (InvoiceType == "Invoice")
            //        {
            //            if (SONoH == SoNo && GINoH == GINo)
            //            {
            //                RemoveSoNo.Add(SoNo, GINo);
            //            }
            //        }
            //        else
            //        {
            //            if (SONoH == SoNo)
            //            {
            //                RemoveSoNo.Add(SoNo, "");
            //            }
            //        }                    
            //    }
            //}

            //for (int i = 0; i < RemoveSoNo.Count; i++)
            //{
            //    for (int j = 0; j < dgvItem.RowCount; j++)
            //    {
            //        SoNo = dgvItem.Rows[j].Cells["SONo"].Value.ToString();  

            //        if (InvoiceType == "Invoice")
            //        {
            //            GINo = dgvItem.Rows[j].Cells["GINo"].Value.ToString();

            //            if (RemoveSoNo.ContainsKey(SoNo) && RemoveSoNo.ContainsValue(GINo))
            //            {
            //                dgvItem.Rows.RemoveAt(j);
            //            }

            //        }
            //        else
            //        {
            //            if (RemoveSoNo.ContainsKey(SoNo))
            //            {
            //                dgvItem.Rows.RemoveAt(j);
            //            }
            //        }                   
            //    }
            //}
           
            Conn.Close();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        public void ParamHeader(DataGridView prmDgvDetailFromHeader)
        {
            dgvDetailFromHeader = prmDgvDetailFromHeader;
        }

        public void SetParentForm(AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice P)
        {
            Parent = P;
        } 
       
        private void MethodSelectData()
        {
            int CountChk = 0;
            for (int i = 0; i <= dgvItem.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvItem.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    int No = 0;
                    if (Parent.dgvSoReferenceDetailsSO.RowCount == 0)
                    {
                        No = Convert.ToInt32(dgvItem.Rows[i].Cells[0].Value);
                    }
                    else
                    {
                        int LastNo = Parent.dgvSoReferenceDetailsSO.RowCount;
                        No = LastNo + 1;
                    }
                    string SoNo = dgvItem.Rows[i].Cells["SoNo"].Value.ToString();
                    string SODate = Convert.ToString(Convert.ToDateTime(dgvItem.Rows[i].Cells["SODate"].Value.ToString()).ToString("dd/MM/yyyy"));
                    string SODueDate = Convert.ToString(Convert.ToDateTime(dgvItem.Rows[i].Cells["SODueDate"].Value.ToString()).ToString("dd/MM/yyyy"));
                    string SOReference = dgvItem.Rows[i].Cells["SOReference"].Value.ToString();
                    string DPRequired = dgvItem.Rows[i].Cells["DPRequired"].Value.ToString();
                    string DPPercent = dgvItem.Rows[i].Cells["DPPercent"].Value.ToString();
                    string PPN = dgvItem.Rows[i].Cells["PPN"].Value.ToString();
                    string SOAmount = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["SOAmount"].Value.ToString()).ToString("N2"));
                    string DPAmount = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["DPAmount"].Value.ToString()).ToString("N2"));
                    string DPDeduct = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["DPDeduct"].Value.ToString()).ToString("N2"));
                    string DPOutstanding = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["DPOutstanding"].Value.ToString()).ToString("N2"));
                    string SOInvoiced = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["SOInvoiced"].Value.ToString()).ToString("N2"));
                    string SOUnInvoiced = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["SOUnInvoiced"].Value.ToString()).ToString("N2"));
                    string GINo = "";                    
                    string GIDate = Convert.ToString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    string POTDP2 = "0.00";
                    if (InvoiceType == "Invoice")
                    {
                        GINo = dgvItem.Rows[i].Cells["GINo"].Value.ToString();
                        GIDate = Convert.ToString(Convert.ToDateTime(dgvItem.Rows[i].Cells["GIDate"].Value.ToString()).ToString("dd/MM/yyyy HH:mm:ss"));
                        POTDP2 = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["POTDP2"].Value.ToString()).ToString("N2"));
                       
                    }
                    string GIPayable = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["GIPayable"].Value.ToString()).ToString("N2"));
                    string GIAmount = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["GIAmount"].Value.ToString()).ToString("N2"));
                    string ReturAmount = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["ReturAmount"].Value.ToString()).ToString("N2")); 
                    //string GINett = dgvItem.Rows[i].Cells["GINett"].Value.ToString();
                    string POTDP = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["POTDP"].Value.ToString()).ToString("N2"));
                    string InvoiceAmount = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["InvoiceAmount"].Value.ToString()).ToString("N2")); 
                    string InvoiceTaxBaseAmount = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["InvoiceTaxBaseAmount"].Value.ToString()).ToString("N2")); 
                    string InvoiceTaxAmount = Convert.ToString(Convert.ToDecimal(dgvItem.Rows[i].Cells["InvoiceTaxAmount"].Value.ToString()).ToString("N2"));
                    
                    Parent.dgvSoReferenceDetailsSO.Rows.Add(No, SoNo, SODate, SODueDate, SOReference, DPRequired, DPPercent, PPN, SOAmount, DPAmount,
                    DPDeduct, DPOutstanding, SOInvoiced, SOUnInvoiced, GINo, GIDate, GIAmount, ReturAmount, POTDP, GIPayable, POTDP2, InvoiceAmount,
                    InvoiceTaxBaseAmount, InvoiceTaxAmount);                   
                }
            }  

            if (CountChk == 0)
            {
                MessageBox.Show("Silahkan pilih data terlebih dahulu");
                return;
            }
            else
            {
                for (int i = 0; i < 24; i++)
                {
                    if (InvoiceType == "Invoice")
                    {
                        Parent.dgvSoReferenceDetailsSO.Columns[i].ReadOnly = true;
                    }
                    else if (InvoiceType != "Invoice" && i == 21)
                    {
                        Parent.dgvSoReferenceDetailsSO.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        Parent.dgvSoReferenceDetailsSO.Columns[i].ReadOnly = true;                   
                    }
                }

                if (InvoiceType == "Invoice")
                {
                    Parent.dgvSoReferenceDetailsSO.Columns["GINo"].Visible = true;
                    Parent.dgvSoReferenceDetailsSO.Columns["POTDP2"].Visible = true;
                }
                else
                {
                    Parent.dgvSoReferenceDetailsSO.Columns["GINo"].Visible = false;
                    Parent.dgvSoReferenceDetailsSO.Columns["POTDP2"].Visible = false;
                }                

                //getPaymentDueDate
                var GetPaymentDueDate = from CustInovice in Parent.dgvSoReferenceDetailsSO.Rows.Cast<DataGridViewRow>()
                                        orderby Convert.ToInt32(CustInovice.Cells["SODueDate"].Value.ToString().Substring(6, 4)) + "" +
                                        Convert.ToInt32(CustInovice.Cells["SODueDate"].Value.ToString().Substring(3, 2)) + "" +
                                        Convert.ToInt32(CustInovice.Cells["SODueDate"].Value.ToString().Substring(0, 2))  descending
                                        select CustInovice.Cells["SODueDate"].Value.ToString();

                string PaymentDueDate = GetPaymentDueDate.First();
                Parent.dtPaymentDueDate.MaxDate = Convert.ToDateTime(PaymentDueDate.Substring(3, 2) + "/" + PaymentDueDate.Substring(0, 2) + "/" + PaymentDueDate.Substring(6, 4));
                //end getPaymentDueDate

                Parent.FormulaFooter();

                this.Close();
            } 
        }
        
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            MethodSelectData();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            txtSearch.Enabled = true;
            dtFromDate.Value = DateTime.Today.Date;
            dtToDate.Value = DateTime.Today.Date;
            RefreshGrid();
            SetCmbCriteria();
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFromDate.Enabled = true;
                dtToDate.Enabled = true;
            }
            else
            {
                dtFromDate.Enabled = false;
                dtToDate.Enabled = false;
            }

            if (cmbCriteria.Text.Contains("Date"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }
            RefreshGrid();
        }
    }
}

