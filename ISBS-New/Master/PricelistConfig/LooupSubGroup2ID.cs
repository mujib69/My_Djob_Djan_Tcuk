using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.PricelistConfig
{
    public partial class LookupSubGroup2ID : MetroFramework.Forms.MetroForm
    {
        #region Inisiasi
        //SQL Function
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        private int Index;
        private Boolean RefreshGridStatus = false;

        //Flag New, Edit
        public string Variable;

        //Penanda untuk paging grid
        private int Limit1, Limit2, Total, Page1, Page2;

        public string QuerySearch;
        private string Where;
        public string Order;
        public string Field;
        public string FormName = "Search Query";

        public string Table;
        public string PrimaryKey;
        public string[] Select;
        public string[] Data;
        public string[] FilterDate;
        public string[] FilterText;
        public string[] Hide;
        public string WherePlus;
        
        //public string Field;
        //public string Select;

        //Flag Proses Sarch
        string IdNamePK = "";
        string Type = "";

        private Boolean tmpAmbilKodeNo = false;

        public LookupSubGroup2ID()
        {
            InitializeComponent();

            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Convert.ToInt32(cmbShow.Text);
        }

        private void LookupSubGroup2ID_Load(object sender, EventArgs e)
        {
            this.Text = FormName;
            LoadCmbFilter();
            RefreshDataGrid();
        }
        #endregion

        #region Function&Method
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
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }

        private void RefreshDataGrid()
        {
            try
            {
                //Menampilkan data
                Conn = ConnectionString.GetConnection();
                Query = "Select * from (Select ROW_NUMBER() OVER (ORDER BY " + Order + ") No, * From (" + QuerySearch +" ) a "+ GetWhere() +" ) a ";
               // Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

                Da = new SqlDataAdapter(Query, Conn);
                Dt = new DataTable();
                Da.Fill(Dt);

                dgvSearch.AutoGenerateColumns = true;
                dgvSearch.DataSource = Dt;
                dgvSearch.Refresh();
                dgvSearch.AutoResizeColumns();

                if (Hide != null)
                {
                    for (int i = 0; i < Hide.Length; i++)
                    {
                        dgvSearch.Columns[Hide[i].ToString()].Visible = false;
                    }
                }
                Conn.Close();
               // GetTotalPaging();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }

        private void GetTotalPaging()
        {
            try
            {
                //Mengambil nilai total paging
                Conn = ConnectionString.GetConnection();
                Query = "Select Count(" + PrimaryKey + ") From (" + QuerySearch + ") a ";
                Query += GetWhere();

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    Total = Convert.ToInt32(Command.ExecuteScalar());
                }
                //Conn.Close();

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling(Total / Convert.ToDecimal(cmbShow.Text));
                lblPage.Text = "/ " + Page2;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }

        private string GetWhere ()
        {
            Where = "Where 1=1 and ";
            if (cmbFilter1.Text.Trim() != "All")
            {
                Where += cmbFilter1.Text.Trim() + " like '%" + txtFilter1.Text + "%' ";
            }
            else
            {
                for (int i = 1; i < cmbFilter1.Items.Count; i++)
                {
                    if (i == 1)
                    {
                        Where += " (";
                    }
                    Where += cmbFilter1.Items[i].ToString() + " like '%" + txtFilter1.Text + "%' ";
                    if (i < cmbFilter1.Items.Count - 1)
                    {
                        Where += " or ";
                    }
                }
                Where += ")";
            }

            return Where + WherePlus ;
        }

        //private string FunctionSubGetWhere(string tmpWhere) //Menentukan Where atau and
        //{
        //    if (tmpWhere == "")
        //    {
        //        return "Where ";
        //    }
        //    else
        //    {
        //        return "And ";
        //    }
        //}

        private void LoadCmbFilter()
        {
            try
            {
                cmbFilter1.Items.Add("All");
                for (int i = 0; i < FilterText.Count(); i++)
                {
                    cmbFilter1.Items.Add(FilterText[i]);
                }
                cmbFilter1.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }


        private void SelectData()
        {
            try
            {
                Index = dgvSearch.CurrentRow.Index;
                ConnectionString.Kodes = new string[Select.Count()];
                for (int i = 0; i < Select.Count(); i++)
                {
                    ConnectionString.Kodes[i] = dgvSearch.Rows[Index].Cells[Select[i]].Value.ToString(); 
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }
        #endregion

        #region EventForm
        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text.Trim()) - 1) * Convert.ToInt32(cmbShow.Text.Trim()) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text.Trim()) * Convert.ToInt32(cmbShow.Text.Trim());
            RefreshDataGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Convert.ToInt32(cmbShow.Text) >= 1)
            {
                Limit1 -= Convert.ToInt32(cmbShow.Text);
                Limit2 -= Convert.ToInt32(cmbShow.Text);
                txtPage.Text = (Convert.ToInt32(txtPage.Text) - 1).ToString();
            }
            RefreshDataGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Convert.ToInt32(cmbShow.Text) <= Total)
            {
                Limit1 += Convert.ToInt32(cmbShow.Text);
                Limit2 += Convert.ToInt32(cmbShow.Text);
                txtPage.Text = (Convert.ToInt32(txtPage.Text) + 1).ToString();
            }
            RefreshDataGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = ((int)Math.Ceiling(Total / Convert.ToDecimal(cmbShow.Text))).ToString();

            Limit1 = (Convert.ToInt32(txtPage.Text) - 1) * Convert.ToInt32(cmbShow.Text) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text);
            RefreshDataGrid();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvSearch.RowCount > 0)
            {
                SelectData();
            }
            this.Close();
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text.Trim()) - 1) * Convert.ToInt32(cmbShow.Text.Trim()) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text.Trim()) * Convert.ToInt32(cmbShow.Text.Trim());
            if (RefreshGridStatus == true)
                RefreshDataGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshDataGrid();
        }

        #endregion

        private void dgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSearch.RowCount > 0)
            {
                SelectData();
            }
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            ConnectionString.Kodes = null;
        }

        private void LookupSubGroup2ID_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void txtFilter1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                RefreshDataGrid();
            }
        }

        

    }
}
