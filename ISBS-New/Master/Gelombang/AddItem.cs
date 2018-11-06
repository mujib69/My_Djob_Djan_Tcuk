using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Gelombang
{
    public partial class AddItem : MetroFramework.Forms.MetroForm
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

        Master.Gelombang.GelombangForm Parent;

        private void CreateTable()
        {
            try
            {
                dgvItem.ColumnCount = 2;
                dgvItem.Columns[0].Name = "ItemId";
                dgvItem.Columns[1].Name = "ItemName";
                
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvItem.Columns.Add(chk);
                chk.HeaderText = "";
                chk.Name = "chk";
                dgvItem.Columns[2].Width = 40;

                
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        public AddItem()
        {
            InitializeComponent();
        }

        public void ParentRefreshGrid(Master.Gelombang.GelombangForm F)
        {
            Parent = F;
        }

        private void RefreshGrid()
        {
            dgvItem.AutoGenerateColumns = true;
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select FullItemId, ItemDeskripsi,InventTypeID,ManufacturerID,MerekID From [dbo].[InventTable] Where ";
                //Query += "FullItemId NOT IN (Select ItemId From [dbo].[InventGelombangD] ) And "; 
                Query += Parent.getItemId() + " ";
                Query += " AND TagSizeID='000'; ";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select FullItemId, ItemDeskripsi,InventTypeID,ManufacturerID,MerekID From [dbo].[InventTable] ";
                Query += "Where (FullItemId Like '%" + txtSearch.Text + "%' or ItemDeskripsi Like '%" + txtSearch.Text + "%' ) And ";
                //Query += "FullItemId NOT IN (Select ItemId From [dbo].[InventGelombangD] ) And ";
                Query += Parent.getItemId() + " ";
                Query += " AND TagSizeID='000';";

            }
            else
            {
                Query = "Select FieldName from [User].[Table] Where DisplayName = '" + cmbCriteria.Text.ToString() + "' AND TableName = 'GelombangAddItem'";
                Cmd = new SqlCommand(Query, Conn);
                crit = Cmd.ExecuteScalar().ToString();
                Query = "Select FullItemId, ItemDeskripsi,InventTypeID,ManufacturerID,MerekID From [dbo].[InventTable] ";
                Query += "Where (" + crit + " Like '%" + txtSearch.Text + "%') And ";
                //Query += "FullItemId NOT IN (Select ItemId From [dbo].[InventGelombangD] ) And ";
                Query += Parent.getItemId() + " ";
                Query += " AND TagSizeID='000'; ";

            }
            
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            if (dgvItem.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvItem.Columns.Add(chk);
                chk.HeaderText = "";
                chk.Name = "chk";
            }

            dgvItem.AutoGenerateColumns = true;
            dgvItem.DataSource = Dt;
            dgvItem.Refresh();
            dgvSetting();
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void dgvSetting()
        {           
            dgvItem.ReadOnly = false;
            dgvItem.Columns["FullItemID"].ReadOnly = true;
            dgvItem.Columns["ItemDeskripsi"].ReadOnly = true;
            dgvItem.AutoResizeColumns();

            dgvItem.Columns["FullItemID"].HeaderText = "FullItemID";
            dgvItem.Columns["ItemDeskripsi"].HeaderText = "Item Name";
            dgvItem.Columns["InventTypeID"].HeaderText = "Type";
            dgvItem.Columns["ManufacturerID"].HeaderText = "Manufacturer";
            dgvItem.Columns["MerekID"].HeaderText = "Brand";
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

        private void AddItem_Load(object sender, EventArgs e)
        {
            ModeLoad();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            flagRefresh++;
            RefreshGrid();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            flagRefresh++;
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
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'GelombangAddItem' order by OrderNo";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
            
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

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    Total = Convert.ToInt32(Command.ExecuteScalar());
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
            if (txtSearch.Text == null || txtSearch.Text.Equals(""))
            {
                MessageBox.Show("Masukkan Kata Kunci");
            }
            else if (cmbCriteria.SelectedIndex == -1)
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
            List<string> ItemId = new List<string>();

            for (int i = 0; i <= dgvItem.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvItem.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    ItemId.Add(dgvItem.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvItem.Rows[i].Cells["FullItemId"].Value.ToString());
                }
            }
            Parent.AddDataGridFromDetail(ItemId);
          
            this.Close();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (txtSearch.Text == null || txtSearch.Text.Equals(""))
                {
                    MessageBox.Show("Masukkan Kata Kunci");
                }
                else if (cmbCriteria.SelectedIndex == -1)
                {
                    crit = "All";
                }
                else
                {
                    crit = cmbCriteria.SelectedItem.ToString();
                }

                RefreshGrid();
            }
        }
    }
}
