using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.Vendor
{
    public partial class Vendor : MetroFramework.Forms.MetroForm
    {

        #region Inisiasi

        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt = new DataTable();
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private int Index;

        private string vendorID;

        public int Y = 29;

        public Vendor()
        {
            InitializeComponent();
        }

        private void Vendor_Load(object sender, EventArgs e)
        {
            //BY: HC NANTI DI UNCOMMENT
            //if (Purchase.ReceiptOrder.InquiryReceiptOrder.vendID != null)
            //    lblVendorId.Text = vendorID = Purchase.ReceiptOrder.InquiryReceiptOrder.vendID;
            if (Purchase.PurchaseQuotation.InquiryPQ.vendID != null)
                lblVendorId.Text = vendorID = Purchase.PurchaseQuotation.InquiryPQ.vendID;
            if (Purchase.RFQ.RFQInquiry.vendID != null)
                lblVendorId.Text = vendorID = Purchase.RFQ.RFQInquiry.vendID;
            if (Purchase.PurchaseOrderNew.POInquiry.vendID != null)
                lblVendorId.Text = vendorID = Purchase.PurchaseOrderNew.POInquiry.vendID;
            if (Purchase.PurchaseAgreement.PAInq.vendID != null)
                lblVendorId.Text = vendorID = Purchase.PurchaseAgreement.PAInq.vendID;

            //this.Location = new Point(928, Y);
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }

            if (vendorID == null)
                vendorID = lblVendorId.Text;

            //lblVendorId.Text = ": " + lblVendorId.Text;

            Conn = ConnectionString.GetConnection();
            Query = "select * from [dbo].[VendTable] where [VendID] = '" + vendorID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lblVendorId.Text = Dr["VendId"].ToString();
                lblVendorName.Text = Dr["VendName"].ToString();
                lblGolPerusahaan.Text = Dr["Gol_Prsh"].ToString();
                lblStatus.Text = Dr["Status"].ToString();
                lblNPWP.Text = Dr["NPWP"].ToString();
                lblTaxName.Text = Dr["TaxName"].ToString();
                lblTaxAddress.Text = Dr["TaxAddress"].ToString();
                lblPaymentTerms.Text = Dr["TermOfPayment"].ToString();
            }

            Query = "select count(*) from [dbo].[PurchH] as A left join [dbo].[VendTable] as C on C.[VendID] = A.[VendID] where C.[VendID] = '" + vendorID + "'";
            Cmd = new SqlCommand(Query, Conn);
            lblAmountPO.Text = ": " + Cmd.ExecuteScalar().ToString();

            Query = "select top 3 [PurchID] from [dbo].[PurchH] where [VendID] = '" + vendorID + "' order by [PurchID] desc";
            Da = new SqlDataAdapter(Query, Conn);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(Da);
            Dt.Locale = System.Globalization.CultureInfo.InvariantCulture;
            Da.Fill(Dt);
            dataGridView1.DataSource = Dt;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns["PurchID"].HeaderText = "PO ID";
        }
        #endregion

        #region Funtion
        public void GetData(string VendId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select VendId, VendName, Gol_Prsh, Status, NPWP, TaxName, TaxAddress, TermOfPayment From [dbo].[VendTable] where VendId='" + VendId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lblVendorId.Text = Dr["VendId"].ToString();
                lblVendorName.Text = Dr["VendName"].ToString();
                lblGolPerusahaan.Text = Dr["Gol_Prsh"].ToString();
                lblStatus.Text = Dr["Status"].ToString();
                lblNPWP.Text = Dr["NPWP"].ToString();
                lblTaxName.Text = Dr["TaxName"].ToString();
                lblTaxAddress.Text = Dr["TaxAddress"].ToString();
                lblPaymentTerms.Text = Dr["TermOfPayment"].ToString();
            }
            Dr.Close();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Vendor_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(750, Y);
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "PurchID")
                {
                    poID = dataGridView1.Rows[e.RowIndex].Cells["PurchID"].Value.ToString();
                    Purchase.PurchaseOrderNew.POForm f = new Purchase.PurchaseOrderNew.POForm();
                    f.SetMode("BeforeEdit", poID, "");
                    f.Show();
                }
            }
        }

        public static string poID;
        public string PoID { get { return poID; } set { poID = value; } }

        private void Vendor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Purchase.PurchaseQuotation.InquiryPQ.vendID = null;
            Purchase.RFQ.RFQInquiry.vendID = null;;
            Purchase.PurchaseOrderNew.POInquiry.vendID = null;
            Purchase.PurchaseAgreement.PAInq.vendID = null;
            //BY: HC NANTI DI UNCOMMENT
            //Purchase.ReceiptOrder.InquiryReceiptOrder.vendID = null;
        }

    }
}
