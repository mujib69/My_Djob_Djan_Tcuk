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
    public partial class HdrResize : MetroFramework.Forms.MetroForm
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
        private string Mode;
        private string transID;

        int count; string id;

        public string ItemID;
        public string ItemName;
        public string ResizeType;

        public static List<string> gvFullItemID = new List<string>();

        InquiryResize Parent = new InquiryResize();
        public void SetParent(InquiryResize F)
        {
            Parent = F;
        }

        public HdrResize()
        {
            InitializeComponent();
        }

        private void ResizeForm_Load(object sender, EventArgs e)
        {
            GetDataHeader();
        }

        private void GetDataHeader()
        {
            txtItemId.Text = ItemID;
            txtItemName.Text = ItemName;
            txtResize.Text = ResizeType;

            using (Conn = ConnectionString.GetConnection())
            {
            dataGridView1.ColumnCount = 2;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns[0].Name = "To_FullItemId";
            dataGridView1.Columns[1].Name = "To_ItemName";

            Query = "Select To_FullItemId, To_ItemName from [InventResize] where [From_FullItemId] ='" + ItemID + "';";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            
            while (Dr.Read())
            {
                this.dataGridView1.Rows.Add(Dr[0], Dr[1]);
            }
            Dr.Close();

            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.AllowUserToAddRows = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtResize.Text.ToUpper() == "AUTO" && dataGridView1.RowCount > 0)
            {
                MessageBox.Show("Data yang bisa dipilih hanya 1, karena Resize Type Auto.");
                return;
            }
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "FullItemId";
            tmpSearch.Order = "FullItemId Asc";
            tmpSearch.QuerySearch = "SELECT FullItemId,ItemDeskripsi FROM [InventTable] where FullItemId <> '" + txtItemId.Text.Trim() + "'";
            tmpSearch.FilterText = new string[] { "FullItemId", "ItemDeskripsi" };
            tmpSearch.Select = new string[] { "FullItemId", "ItemDeskripsi" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                this.dataGridView1.Rows.Add(ConnectionString.Kodes[0], ConnectionString.Kodes[1]);
                ConnectionString.Kodes = null;
            }
        }

        private string Validation()
        {
            string check = "";
            if (txtItemId.Text == String.Empty || txtItemName.Text == String.Empty || txtResize.Text == String.Empty)
            {
                MetroFramework.MetroMessageBox.Show(this, "Data masih ada yang kosong.", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                check = "X";
            }
            return check;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Validation() != "X")
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();
                    Query = "Delete From [dbo].[InventResize] where From_FullItemId='" + txtItemId.Text + "'";
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        Query += "Insert into [dbo].[InventResize] (From_FullItemId, From_ItemName, To_FullItemId, To_ItemName) values ('" + txtItemId.Text + "', '" + txtItemName + "', '" + dataGridView1.Rows[i].Cells["To_FullItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["To_ItemName"].Value.ToString() + "');";
                    }
                    Query = "Update [dbo].[InventTable] set ResizeType='" + txtResize.Text + "' where FullItemId='" + txtItemId.Text + "'";
                    
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    Trans.Commit();
                    Conn.Close();
                    MetroFramework.MetroMessageBox.Show(this, "Save successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception Ex)
            {
                MetroFramework.MetroMessageBox.Show(this, Ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);  
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
