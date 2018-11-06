using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.TaskList
{
    public partial class TaskListView : MetroFramework.Forms.MetroForm
    {
        SqlConnection Conn;
        SqlCommand Cmd;
        SqlDataReader Dr;
        SqlDataAdapter Da;
        DataTable Dt;
        string Query;
        private string FormName, SchemaName, PK;
        public string[] FilterText;

        public TaskListView()
        {
            InitializeComponent();
        }

        private void TaskListView_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        public void SetMode(string FormName, string SchemaName, string PK)
        {
            this.FormName = FormName;
            lblFormName.Text = FormName;
            this.SchemaName = SchemaName;
            this.PK = PK;
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            Query = SchemaName;
            if (txtSearch.Text != "" && FilterText.Count() > 0)
            {
                Query += " and ( ";
                for (int i = 0; i < FilterText.Count(); i++)
                {
                    Query += FilterText[i].ToString() + " like @search ";
                    if (i < FilterText.Count() - 1)
                    {
                        Query += " or ";
                    }
                }
                Query += " )";
            }            

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
            Dt = new DataTable();
            Da.Fill(Dt);

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = Dt;
            dataGridView1.Refresh();
            dataGridView1.AutoResizeColumns();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            RefreshGrid();
        }
    }
}
