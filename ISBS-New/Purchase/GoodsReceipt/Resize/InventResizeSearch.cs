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

namespace ISBS_New.Purchase.GoodsReceipt.Resize
{
    public partial class InventResizeSearch : MetroFramework.Forms.MetroForm
    {
        FormResizeGR owner;
        string Query;
        string fullitemid;
        int page = 1;
        int totalrows = 0;
        SqlDataAdapter adapter;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader Dr;
        DataTable namatable;

        public InventResizeSearch(FormResizeGR form, string id)
        {
            InitializeComponent();
            owner = form;
            addcmb();
            cmbshowload();
            fullitemid = id;
            txtPage.Text = page.ToString();
            refreshgrid(fullitemid);
        }


        private void refreshgrid(string fullitemid)
        {
            string query;
            string Query2;
            txtPage.Text = page.ToString();


            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    con = ConnectionString.GetConnection();
                    query = "(SELECT * FROM [dbo].[InventResize] WHERE [From_FullItemId] LIKE '%" + fullitemid + "%') a ";
                    Query = "SELECT a.*, cast(ROW_NUMBER() OVER (ORDER BY [RecId] desc) as int) as 'RowNumber' INTO #Temp FROM " + query + " WHERE ";
                    if (cmbFilter1.Text != "All")
                    {
                        Query += " " + cmbFilter1.Text + " LIKE '%" + txtFilter1.Text + "%'";
                    }
                    else
                    {
                        Query += " To_FullItemId LIKE '%" + txtFilter1.Text + "%' OR To_ItemName LIKE '%" + txtFilter1.Text + "%' ";
                    }

                    cmd = new SqlCommand(Query, con);
                    cmd.ExecuteNonQuery();

                    int cmb = Convert.ToInt32(cmbShow.Text);
                    Query2 = "SELECT COUNT(RowNumber) FROM #Temp";
                    SqlCommand cmd2 = new SqlCommand(Query2, con);
                    int limit = Convert.ToInt32(cmd2.ExecuteScalar().ToString());
                    if (limit % cmb != 0) lblPage.Text = ((limit / cmb) + 1).ToString();
                    else lblPage.Text = (limit / cmb).ToString();
                    lblTotal.Text = "Total Rows :" + limit;

                    int num1 = (Int32.Parse(txtPage.Text) * cmb) - (cmb - 1);
                    int num2 = Int32.Parse(txtPage.Text) * cmb;
                    string Query3 = "SELECT * from #Temp WHERE RowNumber BETWEEN " + num1 + " AND " + num2 + " DROP table #Temp";
                    using (adapter = new SqlDataAdapter(Query3, con))
                    {
                        namatable = new DataTable();
                        adapter.Fill(namatable);
                        dgvSearch.Columns.Clear();
                        dgvSearch.DataSource = namatable;
                        dgvSearch.Columns["RowNumber"].Visible = false;
                        dgvSearch.AllowUserToAddRows = false;
                        dgvSearch.AutoResizeColumns();
                    }
                    con.Close();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                dgvSearch.Columns["From_FullItemId"].Visible = false;
                dgvSearch.Columns["From_ItemName"].Visible = false;
            }
        }

        private void addcmb()
        {
            cmbFilter1.Items.Add("All");
            con = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where TableName = 'InventResize' AND FieldName LIKE '%to%'" ;

            cmd = new SqlCommand(Query, con);
            Dr = cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbFilter1.Items.Add(Dr[0]);
            }
            cmbFilter1.SelectedIndex = 0;
            con.Close();
        }

        private void cmbshowload()
        {
            con = ConnectionString.GetConnection();
            Query = "SELECT CmbValue FROM [Setting].[CmbBox]";
            cmd = new SqlCommand(Query, con);
            Dr = cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbShow.Items.Add(Dr[0]);
            }
            cmbShow.SelectedIndex = 0;
            con.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            refreshgrid(fullitemid);
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            if (txtPage.Text != string.Empty)
            {
                page = Convert.ToInt32(txtPage.Text);
            }
            if (Convert.ToInt32(txtPage.Text) != 1)
            {
                page = 1;
                txtPage.Text = page.ToString();
                refreshgrid(fullitemid);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (txtPage.Text != string.Empty)
            {
                page = Convert.ToInt32(txtPage.Text);
            }
            if (Convert.ToInt32(txtPage.Text) > 1)
            {
                page -= 1;
                txtPage.Text = page.ToString();
                refreshgrid(fullitemid);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (txtPage.Text != string.Empty)
            {
                page = Convert.ToInt32(txtPage.Text);
            }
            if (Convert.ToInt32(lblPage.Text) > page)
            {
                page += 1;
                txtPage.Text = page.ToString();
                refreshgrid(fullitemid);
            }
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            if (txtPage.Text != string.Empty)
            {
                page = Convert.ToInt32(txtPage.Text);
            }
            if (Convert.ToInt32(lblPage.Text) != page)
            {
                page = Convert.ToInt32(lblPage.Text);
                txtPage.Text = page.ToString();
                refreshgrid(fullitemid);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvSearch.Rows.Count > 0 && dgvSearch.SelectedCells != null)
            {
                string id = dgvSearch.CurrentRow.Cells["To_FullItemId"].Value.ToString();
                string name = dgvSearch.CurrentRow.Cells["To_ItemName"].Value.ToString();
                owner.passedToId(id, name);
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Tidak ada item");
            }
        }

        private void cmbShow_SelectedValueChanged(object sender, EventArgs e)
        {
            refreshgrid(fullitemid);
        }

        private void InventResizeSearch_Load(object sender, EventArgs e)
        {
            dgvSearch.AutoResizeColumns();
        }

        private void dgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string id = dgvSearch.CurrentRow.Cells["To_FullItemId"].Value.ToString();
            string name = dgvSearch.CurrentRow.Cells["To_ItemName"].Value.ToString();
            owner.passedToId(id, name);
            this.Dispose();
        }

        private void txtFilter1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 39)
            {
                e.Handled = true;
            }
        }

        private void txtPage_TextChanged(object sender, EventArgs e)
        {
            if (txtPage.Text.Trim() != string.Empty && lblPage.Text.Trim() != string.Empty)
            {
                if (Convert.ToInt32(txtPage.Text) > Convert.ToInt32(lblPage.Text))
                {
                    page = Convert.ToInt32(lblPage.Text);
                }
                else
                {
                    page = Convert.ToInt32(txtPage.Text);
                }
                refreshgrid(fullitemid);
            }
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (int)e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

    }
}
