using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;

namespace ISBS_New.TaskList
{
    public partial class TaskListApproval : MetroFramework.Forms.MetroForm
    {
        /* Created By : Steven
         * The only thing you will edit will be UserAction (to make this tidy)
         * NOTE : Ikutin contoh Request Cancel DO*/

        SqlConnection Conn;
        SqlCommand Cmd;
        SqlDataReader Dr;
        SqlDataAdapter Da;
        DataTable Dt;
        string Query;
        private string Mode, FormName, SchemaName, PK;
        private TransactionScope scope;
        public string[] FilterText;

        public TaskListApproval()
        {
            InitializeComponent();
        }

        private void TaskListApproval_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        public void SetMode(string Mode, string FormName, string SchemaName, string PK)
        {
            this.Mode = Mode;
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

            if (Mode == "ApproveReject")
            {
                DataGridViewButtonColumn buttonapprove = new DataGridViewButtonColumn();
                buttonapprove.Name = "Approve";
                buttonapprove.HeaderText = "Approve";
                buttonapprove.Text = "Approve";
                buttonapprove.UseColumnTextForButtonValue = true;

                DataGridViewButtonColumn buttonreject = new DataGridViewButtonColumn();
                buttonreject.Name = "Reject";
                buttonreject.HeaderText = "Reject";
                buttonreject.Text = "Reject";
                buttonreject.UseColumnTextForButtonValue = true;

                if (!dataGridView1.Columns.Contains("Approve"))
                    dataGridView1.Columns.Add(buttonapprove);
                if (!dataGridView1.Columns.Contains("Reject"))
                    dataGridView1.Columns.Add(buttonreject);
            }

            dataGridView1.Refresh();
            dataGridView1.AutoResizeColumns();
            dataGridView1.AllowUserToAddRows = false;
        }               

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                string currentRowPK = dataGridView1.Rows[e.RowIndex].Cells[PK].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells[PK].Value.ToString();

                if (dataGridView1.Columns[e.ColumnIndex].Name == "Approve")
                    UserAction("Approve", currentRowPK);
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Reject")
                    UserAction("Reject", currentRowPK);
            }
        }

        private void UserAction(string Action, string selectPK)
        {
            try
            {
                using (scope = new TransactionScope())
                {
                    if (FormName == "Request Cancel DO")
                        TaskListRequestCancelDO.actionRequestCancelDO(Action, selectPK); //Location : TaskList > Sales > DeliveryOrder
                    else
                        MessageBox.Show("Function belum dibuat untuk form " + FormName);
                    RefreshGrid();
                    scope.Complete();
                }
            }
            catch (Exception Ex)
            {
                MetroFramework.MetroMessageBox.Show(this, Ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            RefreshGrid();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                RefreshGrid();
        }        
    }
}
