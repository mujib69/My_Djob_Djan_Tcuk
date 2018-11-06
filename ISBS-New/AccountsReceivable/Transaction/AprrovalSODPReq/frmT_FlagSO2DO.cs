using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.AccountsReceivable.Transaction.AprrovalSODPReq
{
    public partial class frmT_FlagSO2DO : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        string Mode, Query = null;

        public frmT_FlagSO2DO()
        {
            InitializeComponent();
        }

        private void frmT_FlagSO2DO_Load(object sender, EventArgs e)
        {
            chkApproveSO2DO.Checked = false;
        }

        private void btnSearchSO_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "SalesOrderNo";
            tmpSearch.Order = "SalesOrderNo Asc";
            tmpSearch.Table = "[dbo].[SalesOrderH]";

            //NOTE : SalesOrderNo yang belum lunas atau belum dibuat invoice
            tmpSearch.QuerySearch = "SELECT [SalesOrderNo] FROM  [SalesOrderH] a WHERE ";
            tmpSearch.QuerySearch += "[DPType] = 'Y' AND ";
            tmpSearch.QuerySearch += "SalesOrderNo In ( ";
            tmpSearch.QuerySearch += "SELECT SO_No FROM CustInvoice_Dtl_DP x LEFT JOIN [CustInvoice_H] y ON x.Invoice_Id = y.Invoice_Id GROUP BY SO_No HAVING sum(Invoice_amount) - sum(Settle_Amount) !=0) GROUP  BY [SalesOrderNo] ";
            tmpSearch.QuerySearch += "UNION ";
            tmpSearch.QuerySearch += "SELECT SalesOrderNo from SalesOrderH a LEFT JOIN [CustInvoice_Dtl_DP] b ON a.SalesOrderNo = b.SO_No WHERE b.SO_No is null";

            string checkQuery = tmpSearch.QuerySearch;
            tmpSearch.FilterText = new string[] { "SalesOrderNo" };
            tmpSearch.Mask = new string[] { "SO No" };
            tmpSearch.Select = new string[] { "SalesOrderNo" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtSONo.Text = ConnectionString.Kodes[0];
                GetData();
                ConnectionString.Kodes = null;
            }
        }

        private void GetData()
        {
            Conn = ConnectionString.GetConnection();
            if (txtSONo.Text != "")
            {
                Query = "SELECT * FROM [SalesOrderH] WHERE [SalesOrderNo] = '" + txtSONo.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtpSODate.Text = Dr["OrderDate"].ToString();
                    txtSOAmount.Text = Dr["Total"].ToString();
                    txtDPRequired.Text = Dr["DPType"].ToString();
                    txtDPPercent.Text = Dr["DPPercent"].ToString();
                    txtDPAmount.Text = Dr["DPAmount"].ToString();
                    txtCustId.Text = Dr["CustID"].ToString();
                    txtCustName.Text = Dr["CustName"].ToString();
                    chkApproveSO2DO.Checked = Convert.ToBoolean(Dr["DP_Requried_UnCheck"]);
                }
            }
            Conn.Close();
        }                      
       

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if (txtSONo.Text == "")
            {
                MessageBox.Show("Pilih SO yang akan di approve!");
                return;
            }

            if (chkApproveSO2DO.Checked == true)
            {
                Query = "UPDATE [SalesOrderH] SET [DP_Requried_UnCheck] = 1 WHERE [SalesOrderNo] = '" + txtSONo.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                MessageBox.Show("Sales Order SO2DO Approved!");
            }
            else
            {
                Query = "UPDATE [SalesOrderH] SET [DP_Requried_UnCheck] = 0 WHERE [SalesOrderNo] = '" + txtSONo.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                MessageBox.Show("Sales Order SO2DO NOT Approved!");
            }
            Conn.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
