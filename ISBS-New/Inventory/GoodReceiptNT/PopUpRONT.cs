using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.GoodReceiptNT
{
    public partial class PopUpRONT : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;

        #region function return value
        private string mode;
        private string roNumber;
        private string grNumber;
        private static List<string> fullItemID = new List<string>();
        public string Mode { get { return mode; } set { mode = value; } }
        public string RONumber { get { return roNumber; } set { roNumber = value; } }
        public string GRNumber { get { return grNumber; } set { grNumber = value; } }
        public static List<string> FullItemID { get { return fullItemID; } set { fullItemID = value; } }
        #endregion

        public PopUpRONT()
        {
            InitializeComponent();
        }

        private void PopUpRONT_Load(object sender, EventArgs e)
        {
            GetDataHeader();
        }

        public void GetDataHeader()
        {
            dgvRODetails.Rows.Clear();
            if (dgvRODetails.RowCount - 1 <= 0)
            {
                dgvRODetails.ColumnCount = 3;
                dgvRODetails.Columns[0].Name = "Check";
                dgvRODetails.Columns[1].Name = "FullItemID";
                dgvRODetails.Columns[2].Name = "ItemName";
            }

            Conn = ConnectionString.GetConnection();
            //if (Mode == "New")
                Query = "select a.FullItemID, a.ItemDeskripsi from InventTable as a ";//where a.FullItemID NOT IN ( select FullItemID from ReceiptOrderD where ReceiptOrderId = '" + roNumber + "')";
            //else
            //    Query = "select a.FullItemID, a.ItemDeskripsi from InventTable as a ";//where a.FullItemID NOT IN ( select FullItemID from GoodsReceivedD where GoodsReceivedId = '" + grNumber + "')";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();

                this.dgvRODetails.Rows.Add("", Dr["FullItemID"], Dr["ItemDeskripsi"]);
                dgvRODetails.Rows[i].Cells["Check"] = chk;
                dgvRODetails.Rows[i].Cells["Check"].Value = false;
                i++;
            }
            Dr.Close();

            dgvRODetails.AutoResizeColumns();
            dgvRODetails.AllowUserToAddRows = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = checkBox1.Checked;
            for (int i = 0; i <= dgvRODetails.RowCount - 1; i++)
            {
                dgvRODetails.Rows[i].Cells["Check"].Value = Check;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            FullItemID.Clear();
            for (int i = 0; i < dgvRODetails.RowCount; i++)
            {
                if(dgvRODetails.Rows[i].Cells["Check"].Value.ToString() == "True")
                    FullItemID.Add(dgvRODetails.Rows[i].Cells["FullItemId"].Value.ToString());
            }
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
