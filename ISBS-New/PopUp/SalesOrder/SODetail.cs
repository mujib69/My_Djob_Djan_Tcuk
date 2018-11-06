using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.SalesOrder
{
    public partial class SODetail : MetroFramework.Forms.MetroForm
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
        private string SalesOrderNo;
        public SODetail()
        {
            InitializeComponent();
        }

        private void SODetail_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
        }

        public void GetData(string SalesOrderNo)
        {
            Conn = ConnectionString.GetConnection();

            dgvSODetail.DataSource = null;
            if (dgvSODetail.RowCount == 0)
            {
                dgvSODetail.Rows.Clear();
                dgvSODetail.ColumnCount = 5;
                dgvSODetail.Columns[0].Name = "No";
                dgvSODetail.Columns[1].Name = "SalesOrderNo";
                dgvSODetail.Columns[2].Name = "SeqNo";
                dgvSODetail.Columns[3].Name = "FullItemID";
                dgvSODetail.Columns[4].Name = "ItemName";
                dgvSODetail.Columns[4].Name = "Qty";
                dgvSODetail.Columns[4].Name = "Unit";
                dgvSODetail.Columns[4].Name = "Qty_Alt";
                dgvSODetail.Columns[4].Name = "Unit_Alt";
                dgvSODetail.Columns[4].Name = "Price";
                dgvSODetail.Columns[4].Name = "Price_Alt";
            }

            Query = "SELECT No, SalesOrderNo, SeqNo, FullItemID, ItemName, Qty, Unit, Qty_Alt, Unit_Alt, Price, Price_Alt  ";
            Query += "FROM [dbo].[SalesOrderD] WHERE SalesOrderNo='" + SalesOrderNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int i = 1;
            while (Dr.Read())
            {
                this.dgvSODetail.Rows.Add(i, Dr["SalesOrderNo"], Dr["SeqNo"], Dr["FullItemID"], Dr["ItemName"], Dr["Qty"], Dr["Unit"], Dr["Qty_Alt"], Dr["Unit_Alt"], Dr["Price"], Dr["Price_Alt"]);
                i++;
            }
            Dr.Close();

            dgvSODetail.AutoResizeColumns();
            dgvSODetail.ReadOnly = true;
        }



        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
