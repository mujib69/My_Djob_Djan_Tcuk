using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.CustomerID
{
    public partial class Customer : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private int Index;
        private string CustomerID;
        Sales.SalesOrder.SOHeader PUSO = null;

        public Customer()
        {
            InitializeComponent();
        }

        private void CustomerId_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);

            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void refreshgridStatusLog(string CustomerID)
        {
            DtGridView_StatusLog.Columns.Clear();
            string query2;
    
            SqlConnection con = ConnectionString.GetConnection();
            query2 = "select top 3 [SalesOrderNo] from [dbo].[SalesOrderH] WHERE CustID='" + CustomerID + "' ORDER BY [SalesOrderNo] desc";
            
            DataTable namatable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(query2, con);
            adapter.Fill(namatable);
            DtGridView_StatusLog.DataSource = namatable;
            DtGridView_StatusLog.AutoResizeColumns();
            DtGridView_StatusLog.ReadOnly = true;
            DtGridView_StatusLog.AllowUserToAddRows = false;
            DtGridView_StatusLog.Columns["SalesOrderNo"].HeaderText = "SO ID";
            con.Close();

        }

        public void GetData(string CustomerID)
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT CustId, CustName, Gol_Prsh, Status, Kode_Sls, NPWP, TaxName, TaxAddress, PKP, SIUP, TermOfPayment, PPN, PPH, Limit_Total,Limit_Per_PO,Sisa_Limit_Total, CurrencyId, PaymentModeId ";
            Query += "FROM [dbo].[CustTable]";
            Query += "WHERE CustId='" + CustomerID + "'";


            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lblCustID.Text = " " + Dr["CustId"].ToString();
                lblCustName.Text = " " + Dr["CustName"].ToString();
                lblGolPrsh.Text = " " + Dr["Gol_Prsh"].ToString();
                lblStatus.Text = " " + Dr["Status"].ToString();
                lblKodeSls.Text = " " + Dr["Kode_Sls"].ToString();
                lblNPWP.Text = " " + Dr["NPWP"].ToString();
                lblTaxName.Text = " " + Dr["TaxName"].ToString();
                lblTaxAddress.Text = " " + Dr["TaxAddress"].ToString();
                lblPKP.Text = " " + Dr["PKP"].ToString();
                lblSIUP.Text = " " + Dr["SIUP"].ToString();
                lblTermOfPayment.Text = " " + Dr["TermOfPayment"].ToString();
                lblPPN.Text = " " + Dr["PPN"].ToString();
                lblPPH.Text = " " + Dr["PPH"].ToString();
                lblLimitTotal.Text = " " + Dr["Limit_Total"].ToString();
                lblLimitPO.Text = " " + Dr["Limit_Per_PO"].ToString();
                lblSisaLimitTotal.Text = " " + Dr["Sisa_Limit_Total"].ToString();
                lblCurrencyID.Text = " " + Dr["CurrencyId"].ToString();
                lblPaymentModeID.Text = " " + Dr["PaymentModeId"].ToString();

            }
            Dr.Close();
            refreshgridStatusLog(CustomerID);
        }

        private void DtGridView_StatusLog_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
             {
             if (e.Button == MouseButtons.Right && e.RowIndex>-1 && e.ColumnIndex>-1)
            {
                if (PUSO == null || PUSO.Text == "")
                {
                    //if (DtGridView_StatusLog.Columns[e.ColumnIndex].Name.ToString() == "SalesOrederNo")
                    //{
                        PUSO = new Sales.SalesOrder.SOHeader();
                        PUSO.SetMode("PopUp", DtGridView_StatusLog.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString());
                        PUSO.ParentRefreshGrid8(this);
                        PUSO.Show();
                   // }
                }
                else if (CheckOpened(PUSO.Name))
                {
                    PUSO.WindowState = FormWindowState.Normal;
                    PUSO.SetMode("PopUp", DtGridView_StatusLog.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString());
                    PUSO.ParentRefreshGrid8(this);
                    PUSO.Show();
                    PUSO.Focus();
                }
            }
        }
        private bool CheckOpened(string name)
        {
            // FormCollection FC = Application.OpenForms;
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
    }
}
