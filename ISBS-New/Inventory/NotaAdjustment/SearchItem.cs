using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.NotaAdjustment
{
    public partial class SearchItem : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        int flagRefresh = 0;
        String ItemId = null;

        Inventory.NotaAdjustment.HeaderNotaAdjust Parent;

        string[] columnName = new string[] { "No", "FullItemId", "ItemDeskripsi", "SpecID", "UoM", "UoMAlt", "Ratio" };
        string[] columnNameText = new string[] { "No", "FullItemId", "Item Name", "Spec", "UoM Unit", "Alt Unit", "Ratio" };

        public SearchItem()
        {
            InitializeComponent();
        }

        public void setParent(Inventory.NotaAdjustment.HeaderNotaAdjust F)
        {
            Parent = F;
        }

        public void flag(String tmpItemId)
        {
            ItemId = tmpItemId;
        }

        private void RefreshGrid()
        {
            dgvItem.AutoGenerateColumns = true;
            Conn = ConnectionString.GetConnection();

            int maxno = Convert.ToInt32(txtPage.Text)*Convert.ToInt32(cmbShow.Text);
            int minno = maxno - Convert.ToInt32(cmbShow.Text) + 1;
            //edit Thaddaeus
            if (crit == null)
            {
                Query = "SELECT * FROM (Select ROW_NUMBER() OVER (ORDER BY a.FullItemId) [No], a.FullItemId, a.ItemDeskripsi AS 'Item Name', a.SpecID AS 'Spec', a.UoM AS 'UoM Unit', a.UoMAlt AS 'Alt Unit', b.Ratio From [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID Where ";
                Query += Parent.getFullItemID() + ")";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT * FROM (Select ROW_NUMBER() OVER (ORDER BY a.FullItemId) [No], a.FullItemId, a.ItemDeskripsi AS 'Item Name', a.SpecID AS 'Spec', a.UoM AS 'UoM Unit', a.UoMAlt AS 'Alt Unit', b.Ratio From [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID ";
                Query += "Where (";
                for (int i = 0; i < cmbCriteria.Items.Count; i++)
                {
                    if (cmbCriteria.Items[i].ToString() == "All")
                    {
                        continue;
                    }
                    if (i == (cmbCriteria.Items.Count - 1))
                    {
                        Query += "a." + GetFieldName(cmbCriteria.Items[i].ToString()) + " LIKE @search) AND ";
                    }
                    else
                    {
                        Query += "a." + GetFieldName(cmbCriteria.Items[i].ToString()) + " LIKE @search OR ";
                    }
                }
                Query += Parent.getFullItemID() + ")";
            }
            else
            {
                Query = "SELECT * FROM (Select ROW_NUMBER() OVER (ORDER BY a.FullItemId) [No], a.FullItemId, a.ItemDeskripsi AS 'Item Name', a.SpecID AS 'Spec', a.UoM AS 'UoM Unit', a.UoMAlt AS 'Alt Unit', b.Ratio From [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID ";
                Query += "Where (a."+ GetFieldName(crit) + " Like @search) And ";
                Query += Parent.getFullItemID() + ")";
            }
            Query += " a WHERE No BETWEEN " + minno + " AND " + maxno + " ";
            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
            Dt = new DataTable();

            if (crit == null)
            {
                Query = "SELECT COUNT(*) FROM [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID where ";
                Query += Parent.getFullItemID() + ";";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(*) FROM [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID ";
                Query += "Where (";
                for (int i = 0; i < cmbCriteria.Items.Count; i++)
                {
                    if (cmbCriteria.Items[i].ToString() == "All")
                    {
                        continue;
                    }
                    if (i == (cmbCriteria.Items.Count - 1))
                    {
                        Query += "a." + GetFieldName(cmbCriteria.Items[i].ToString()) + " LIKE @search) AND ";
                    }
                    else
                    {
                        Query += "a." + GetFieldName(cmbCriteria.Items[i].ToString()) + " LIKE @search OR ";
                    }
                }
                Query += Parent.getFullItemID() + ";";
            }
            else
            {
                Query = "SELECT COUNT(*) FROM [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID ";
                Query += " Where (a." + GetFieldName(crit) + " Like @search) And ";
                Query += Parent.getFullItemID() + ";";
            }
            using (SqlCommand Command = new SqlCommand(Query, Conn))
            {
                Command.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                Total = Convert.ToInt32(Command.ExecuteScalar());
            }
            //end=============
            dgvItem.AutoGenerateColumns = true;
            dgvItem.DataSource = Dt;
            dgvItem.Refresh();
            if (dgvItem.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvItem.Columns.Add(chk);
                chk.HeaderText = "";
                chk.Name = "chk";
            }
            Da.Fill(Dt);

            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    dgvItem.Columns[i].ReadOnly = false;
                }
                else
                {
                    dgvItem.Columns[i].ReadOnly = true;
                }
            }
            dgvItem.AutoResizeColumns();

            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private string GetFieldName(string critt)
        {
            string fieldname = critt;
            string subquery = "SELECT FieldName FROM [User].[Table] WHERE [TableName] = 'SearchItem' AND [SchemaName]='dbo' AND [DisplayName]=@crit";
            using (Cmd = new SqlCommand(subquery, Conn))
            {
                Cmd.Parameters.AddWithValue("@crit", critt);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        fieldname = Dr["FieldName"] == System.DBNull.Value? critt :Dr["FieldName"].ToString();
                    }
                }
                Dr.Close();
            }
            return fieldname;
        }

        private void cmbAllCriteria()
        {
            cmbCriteria.Items.Add("All");
            using (Cmd = new SqlCommand())
            {
                Query = "SELECT DisplayName FROM [ISBS-NEW4].[User].[Table] WHERE [SchemaName]='dbo' AND [TableName]='InventTable' AND FieldName IN (";
                for (int i = 0; i < columnName.Count(); i++)
                {
                    if (i == (columnName.Count() - 1))
                    {
                        Query += "@array" + i + "";
                    }
                    else
                    {
                        Query += "@array" + i + ",";
                    }
                    Cmd.Parameters.AddWithValue("@array" + i + "", columnName[i]);
                }
                Query += ") order by [OrderNo];";
                Cmd.CommandText = Query;
                using (Cmd.Connection = ConnectionString.GetConnection())
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        cmbCriteria.Items.Add(Dr["DisplayName"].ToString());
                    }
                    Dr.Close();
                }
            }
            cmbCriteria.SelectedIndex = 0;
        }

        private void ModeLoad()
        {
            addCmbCrit();
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text == "" ? "0" : cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
        }

        private void SearchItem_Load(object sender, EventArgs e)
        {
            ModeLoad();
            cmbAllCriteria();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            flagRefresh++;
            RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            RefreshGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }
            flagRefresh++;
            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            flagRefresh++;
            RefreshGrid();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                flagRefresh++;
                RefreshGrid();
            }
        }

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                txtPage.Text = "1";
                flagRefresh++;
                RefreshGrid();
            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
            flagRefresh++;
        }

        private void addCmbCrit()
        {
            //cmbCriteria.Items.Add("All");
            //Conn = ConnectionString.GetConnection();
            //Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'VendTable'";

            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();

            //while (Dr.Read())
            //{
            //    cmbCriteria.Items.Add(Dr[0]);
            //}
            //cmbCriteria.SelectedIndex = 0;
            //Conn.Close();
            //cmbCriteria.Items.Add("VendID");
            //cmbCriteria.Items.Add("VendName");
            //cmbCriteria.Items.Add("TaxName");
            //cmbCriteria.Items.Add("NPWP");
            //cmbCriteria.Items.Add("PKP");
            //cmbCriteria.Items.Add("Type");
            //cmbCriteria.Items.Add("TempoBayar");
            //cmbCriteria.Items.Add("PaymentModeID");
            //cmbCriteria.Items.Add("TaxGroup");
            //cmbCriteria.Items.Add("ReffFullItemID");

        }

        private void cmbShowLoad()
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select CmbValue From [Setting].[CmbBox] ";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    cmbShow.Items.Clear();
                    while (Dr.Read())
                        cmbShow.Items.Add(Dr["CmbValue"].ToString());
                    Dr.Close();
                }
                cmbShow.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                Conn.Close();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            /*if (txtSearch.Text == null || txtSearch.Text.Equals(""))
            {
                MessageBox.Show("Masukkan Kata Kunci");
            }
            else */if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }
            RefreshGrid();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> FullItemId = new List<string>();
            List<string> ItemName = new List<string>();

            for (int i = 0; i <= dgvItem.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvItem.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    FullItemId.Add(dgvItem.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvItem.Rows[i].Cells["FullItemId"].Value.ToString());
                    ItemName.Add(dgvItem.Rows[i].Cells["Item Name"].Value == null ? "" : dgvItem.Rows[i].Cells["Item Name"].Value.ToString());
                }
            }
            Parent.AddDataGridFromDetail(FullItemId,ItemName);

            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            RefreshGrid();

            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvItem.RowCount - 1; i++)
            {
                dgvItem.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                btnSearch_Click(this, new EventArgs());
            }
        }
    }
}
