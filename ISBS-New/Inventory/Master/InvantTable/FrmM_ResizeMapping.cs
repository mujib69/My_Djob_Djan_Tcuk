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
    public partial class FrmM_ResizeMapping : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;

        public string ItemID;
        public string ItemName;
        public string JenisBrg;

        public FrmM_ResizeMapping()
        {
            InitializeComponent();
        }

        private void FrmM_ResizeMapping_Load(object sender, EventArgs e)
        {
            txtFullItemId.Text = ItemID;
            txtItemName.Text = ItemName;
            txtJenisBrg.Text = JenisBrg;

            GetDataHeader();
        }

        private void GetDataHeader()
        {
            using (Conn = ConnectionString.GetConnection())
            {
               dtGridViewResize.ColumnCount = 2;
               dtGridViewResize.Rows.Clear();
               dtGridViewResize.Columns[0].Name = "To_FullItemId";
               dtGridViewResize.Columns[1].Name = "To_ItemName";

                Query = "Select To_FullItemId, To_ItemName from [InventResize] where [From_FullItemId] ='" + txtFullItemId.Text + "';";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    this.dtGridViewResize.Rows.Add(Dr[0], Dr[1]);
                }
                Dr.Close();

                dtGridViewResize.ReadOnly = true;
                dtGridViewResize.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dtGridViewResize.DefaultCellStyle.BackColor = Color.LightGray;
                dtGridViewResize.AllowUserToAddRows = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "FullItemId";
            tmpSearch.Order = "FullItemId Asc";
            tmpSearch.QuerySearch = "SELECT FullItemId,ItemDeskripsi FROM InventTable where GroupID=(SELECT GroupID FROM InventTable WHERE FullItemId='" + txtFullItemId.Text + "') AND SubGroup1ID=(SELECT SubGroup1ID FROM [InventTable] WHERE FullItemId='" + txtFullItemId.Text + "') AND SubGroup2ID=(SELECT SubGroup2ID FROM InventTable WHERE FullItemId='" + txtFullItemId.Text + "') AND FullItemId NOT IN (SELECT To_FullItemId FROM InventResize WHERE From_FullItemId='" + txtFullItemId.Text + "') ";
            
            if (dtGridViewResize.RowCount > 0)
            {
                string Items = "";

                for (int i = 0; i < dtGridViewResize.RowCount; i++)
                {
                    if (Items == "")
                    {
                        Items = "'" + dtGridViewResize.Rows[i].Cells["To_FullItemId"].Value.ToString() + "'";
                    }
                    else
                    {
                        Items += ",'" + dtGridViewResize.Rows[i].Cells["To_FullItemId"].Value.ToString() + "'";                    
                    }
                }
                tmpSearch.QuerySearch += "AND FullItemId NOT IN (" + Items + ")";

            }

            tmpSearch.FilterText = new string[] { "FullItemId", "ItemDeskripsi" };
            tmpSearch.Select = new string[] { "FullItemId", "ItemDeskripsi" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                this.dtGridViewResize.Rows.Add(ConnectionString.Kodes[0], ConnectionString.Kodes[1]);
                ConnectionString.Kodes = null;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            dtGridViewResize.Rows.RemoveAt(dtGridViewResize.CurrentRow.Index);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                Query = "Delete From [dbo].[InventResize] where From_FullItemId='" + txtFullItemId.Text + "'";
                for (int i = 0; i < dtGridViewResize.RowCount; i++)
                {
                    Query += "Insert into [dbo].[InventResize] (From_FullItemId, From_ItemName, To_FullItemId, To_ItemName) values ('" + txtFullItemId.Text + "', '" + txtItemName.Text + "', '" + dtGridViewResize.Rows[i].Cells["To_FullItemId"].Value.ToString() + "', '" + dtGridViewResize.Rows[i].Cells["To_ItemName"].Value.ToString() + "');";
                }

                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
                Trans.Commit();
                Conn.Close();
                MetroFramework.MetroMessageBox.Show(this, "Save successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MetroFramework.MetroMessageBox.Show(this, Ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.Close();
        }
    }
}
