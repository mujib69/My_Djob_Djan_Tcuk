using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.PurchaseOrder
{
    public partial class PONumber : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;

        public PONumber()
        {
            InitializeComponent();
        }

        private void PONumber_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            dtPODate.Enabled = false;
            txtDeskripsi.ReadOnly = false;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void GetData(string PONumber)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select PurchId, OrderDate, DueDate, TransType, ReffTableName, ReffId, ReffId2, PH.CurrencyId, ExchRate, PH.VendId, VT.VendName, DP, total, Total_Disk, PH.PPN, Total_PPN, PH.PPH, Total_PPH, Deskripsi, Total_Nett, PH.TermofPayment, PaymentMode ";
            Query += "FROM PurchH PH ";
            Query += "LEFT JOIN VendTable VT ON PH.VendID = VT.VendId ";
            Query += "WHERE PurchId = '" + PONumber + "' ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                lblPOId.Text=Dr["PurchId"].ToString();
                lblRefType.Text = Dr["ReffTableName"].ToString();
                lblRefId.Text = Dr["ReffId"].ToString();
                lblVendid.Text=Dr["VendId"].ToString();
                lblVendName.Text=Dr["VendName"].ToString();
                dtPODate.Text = Dr["OrderDate"].ToString();
                lblCurrency.Text = Dr["CurrencyId"].ToString();
                lblEchRate.Text = Dr["ExchRate"].ToString();
                lblTermOfPayment.Text = Dr["TermofPayment"].ToString();
                lblPaymentMode.Text = Dr["PaymentMode"].ToString();
                //lblDPRequired.Text=Dr[""].ToString();
                lblDP1.Text = Convert.ToDecimal(Dr["DP"]).ToString("N4");
                lblPPN.Text=Dr["PPN"].ToString();
                lblPPH.Text=Dr["PPH"].ToString();
                lblTotal.Text = Convert.ToDecimal(Dr["Total"]).ToString("N4");
                lblTotalDiscount.Text = Convert.ToDecimal(Dr["Total_Disk"]).ToString("N4");
                lblTotalPPN.Text = Convert.ToDecimal(Dr["Total_PPN"]).ToString("N4");
                lblTotalPPH.Text = Convert.ToDecimal(Dr["Total_PPH"]).ToString("N4");
                lblGrandTotal.Text = Dr["Total_Nett"].ToString();
            }
            Dr.Close();
        }


    }
}
