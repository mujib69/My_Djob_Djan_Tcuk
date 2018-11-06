using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Sales.BBK
{
    public partial class UbahReceiptDate : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        String Query = null;

        public UbahReceiptDate()
        {
            InitializeComponent();
        }

        public void SetMode(string tmpGINumber, DateTime tmpDate)
        {
            txtGINo.Text = tmpGINumber;
            dtGIDate.Value = tmpDate.Date;
            txtGINo.ReadOnly = true;
            dtGIDate.Enabled = false;
            dtReceiptDate.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (dtGIDate.Value < dtReceiptDate.Value)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "UPDATE [dbo].[GoodsIssuedH] SET [Receipt_Date] = '" + dtReceiptDate.Value + "' WHERE [GoodsIssuedId] = '" + txtGINo.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    Conn.Close();

                    MessageBox.Show("Receipt Date Updated!");
                }
                catch (Exception ex)
                {
                    MetroFramework.MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
               MessageBox.Show("Receipt Date tidak boleh lebih kecil dari GI Date.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
