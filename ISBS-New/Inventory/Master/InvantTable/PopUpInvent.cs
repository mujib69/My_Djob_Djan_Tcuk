using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.Master.InvantTable
{
    public partial class PopUpInvent : MetroFramework.Forms.MetroForm
    {
        //SQL Function
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private int Index;
        public string ResizeType;

        private static string itemID;
        public static string ItemID { get { return itemID; } set { itemID = value; } }
        private static List<string> fullItemID = new List<string>();
        public static List<string> FullItemID { get { return fullItemID; } set { fullItemID = value; } }

        public PopUpInvent()
        {
            InitializeComponent();
        }

        private void PopUpInvent_Load(object sender, EventArgs e)
        {
            exit = '\0';
            GetDataHeader();
        }

        public void GetDataHeader()
        {
            dataGridView1.Rows.Clear();
            if (dataGridView1.RowCount - 1 <= 0)
            {
                dataGridView1.ColumnCount = 3;
                dataGridView1.Columns[0].Name = "Check";
                dataGridView1.Columns[1].Name = "FullItemID";
                dataGridView1.Columns[2].Name = "ItemName";
            }

            Conn = ConnectionString.GetConnection();
            Query = "select [GroupID], [SubGroup1ID], [SubGroup2ID], [ItemID], FullItemID, ItemDeskripsi from InventTable where FullItemID != '" + ItemID + "' ";
            for(int x = 0; x < HeaderResize.gvFullItemID.Count ; x++)
            {
                Query += "and FullItemID != '" + HeaderResize.gvFullItemID[x] + "' ";
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();

                this.dataGridView1.Rows.Add("", Dr["FullItemID"], Dr["ItemDeskripsi"]);
                dataGridView1.Rows[i].Cells["Check"] = chk;
                dataGridView1.Rows[i].Cells["Check"].Value = false;
                i++;
            }
            Dr.Close();

            for (int x = 1; x < dataGridView1.ColumnCount; x++)
            {
                dataGridView1.Columns[x].ReadOnly = true;
                dataGridView1.Columns[x].DefaultCellStyle.BackColor = Color.LightGray;
                dataGridView1.Columns[x].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dataGridView1.AutoResizeColumns();
            dataGridView1.AllowUserToAddRows = false;
        }

        public static char exit;
        private void btnExit_Click(object sender, EventArgs e)
        {
            exit = 'X';
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = checkBox1.Checked;
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                dataGridView1.Rows[i].Cells["Check"].Value = Check;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {

            FullItemID.Clear();
            int Checked =0;
            if (ResizeType == "A" )
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["Check"].Value.ToString() == "True")
                        Checked++;
                }
                if (Checked > 1)
                {
                MessageBox.Show("Data yang bisa dipilih hanya 1, karena Resize Type = A.");
                goto Outer;
                }
            }

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells["Check"].Value.ToString() == "True")
                    FullItemID.Add(dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString());
            }
            this.Close();
            Outer:;
        }

        private void PopUpInvent_FormClosed(object sender, FormClosedEventArgs e)
        {
            ItemID = null;
        }
    }
}
