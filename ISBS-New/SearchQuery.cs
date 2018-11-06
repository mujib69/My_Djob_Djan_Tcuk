using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New
{
    public partial class SearchQuery : MetroFramework.Forms.MetroForm
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

        Label[] label;
        TextBox[] textbox;

        //Flag New, Edit
        public string Variable;

        //Penanda untuk paging grid
        private int Limit1, Limit2, Total, Page1, Page2;

        public string QuerySearch;
        private string Where;
        public string Order;
        public string Field;

        public string Table;
        public string PrimaryKey;
        public string[] Select;
        public string[] Data;
        public string[] FilterDate;
        public string[] FilterText;
        public string WherePlus;

        //public string Field;
        //public string Select;

        //Flag Proses Sarch
        string IdNamePK = "";
        string Type = "";

        private Boolean tmpAmbilKodeNo = false;

        public SearchQuery()
        {
            InitializeComponent();

            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Convert.ToInt32(cmbShow.Text);
        }

        private void SearchQuery_Load(object sender, EventArgs e)
        {
            LoadCmbFilter();

            int pos = 0;
            label = new Label[Convert.ToInt32(FilterText.Length)];
            textbox = new TextBox[Convert.ToInt32(FilterText.Length)];
            for (int i = 0; i < Convert.ToInt32(FilterText.Length); i++)
            {
                label[i] = new Label();
                /*#1*/
                label[i].Text = FilterText[i]; //#2 String.Format("Label {0}", "AAAAA");
                /*#1*/
                label[i].Location = new System.Drawing.Point(6, 22 + pos); //#2 label[i].Left = 500 + pos; label[i].Top = 460;
                this.groupBox2.Controls.Add(label[i]);
                textbox[i] = new TextBox();
                textbox[i].Location = new System.Drawing.Point(106, 15 + pos);
                textbox[i].Width = 200;
                this.groupBox2.Controls.Add(textbox[i]);
                pos += 26;
            }
            dateTimePicker2.Text = DateTime.Now.ToString();
            RefreshDataGrid();
            CheckValidationFilter();
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
            //try
            //{
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            Query = "Select * from (Select ROW_NUMBER() OVER (ORDER BY " + PrimaryKey + " asc) No, * From (" + QuerySearch + " ) a " + GetWhere() + " ) a " + Order;
            Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvSearch.AutoGenerateColumns = true;
            dgvSearch.DataSource = Dt;
            dgvSearch.Refresh();
            dgvSearch.AutoResizeColumns();
            Conn.Close();
            GetTotalPaging();
            //}
            //catch (Exception Ex)
            //{
            //    MessageBox.Show(ConnectionString.GlobalException(Ex));
            //}
            //finally
            //{
            //    Conn.Close();
            //}
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

        //private string GetWhere2()
        //{
        //    Where = "Where 1=1 and ( PurchReqID like '%" + tbxPRID.Text + "%' or ";
        //    Where += "TransType like '%" + tbxTType.Text + "%' or ";
        //    Where += "TransStatus like '%" + tbxTStatus1.Text + "%' or ";
        //    Where += "OrderDate between '" + dateTimePicker1.Value.Date.ToString("yyyy-MM-dd") + "' and '" + dateTimePicker2.Value.Date.ToString("yyyy-MM-dd") + "' )";
        //    return Where + WherePlus;
        //}


        private string GetWhere()
        {
            Where = "Where 1=1 ";
            //if (chkFilter1.Checked == true && cmbFilter1.Text.Trim() != "")
            //{
            //    Where += "and " + cmbFilter1.Text.Trim() + " like '%" + txtFilter1.Text + "%' ";
            //}
            //else if (chkFilter2.Checked == true && cmbFilter2.Text.Trim() != "")
            //{
            //    Where += "and " + cmbFilter2.Text.Trim() + " like '%" + txtFilter2.Text + "%' ";
            //}
            //else if (chkFilter3.Checked == true && cmbFilter3.Text.Trim() != "")
            //{
            //    Where += "and " + cmbFilter3.Text.Trim() + " like '%" + txtFilter3.Text + "%' ";
            //}
            //else if (chkFilter4.Checked == true && cmbFilter4.Text.Trim() != "")
            //{
            //    Where += "and " + cmbFilter4.Text.Trim() + " like '%" + txtFilter4.Text + "%' ";
            //}
            //else if (chkFilter5.Checked == true && cmbFilter5.Text.Trim() != "")
            //{
            //    Where += "and " + cmbFilter4.Text.Trim() + " like '%" + txtFilter4.Text + "%' ";
            //}
            //else if (chkFilter6.Checked == true && cmbFilter6.Text.Trim() != "")
            //{
            //    Where += "and " + cmbFilter5.Text.Trim() + " like '%" + txtFilter5.Text + "%' ";
            //}
            //else if (chkDate1.Checked == true && cmbDate1.Text.Trim() != "")
            //{
            //    Where += "and " + " ((CONVERT(VARCHAR(10)," + cmbDate1.Text.Trim() + ",120) >= '" + DateFrom1.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10)," + cmbDate1.Text.Trim() + ",120) <= '" + DateTo1.Value.Date.ToString("yyyy-MM-dd") + "')) ";
            //}
            //else if (chkDate2.Checked == true && cmbDate2.Text.Trim() != "")
            //{
            //    Where += "and " + " ((CONVERT(VARCHAR(10)," + cmbDate2.Text.Trim() + ",120) >= '" + DateFrom2.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10)," + cmbDate2.Text.Trim() + ",120) <= '" + DateTo2.Value.Date.ToString("yyyy-MM-dd") + "')) ";
            //}

            for (int i = 0; i < Convert.ToInt32(FilterText.Length); i++)
            {
                if (textbox[i].Text != "")
                    Where += " and " + FilterText[i].ToString() + " like '%" + textbox[i].Text + "%'";
            }

            if (FilterDate != null)
                Where += " and " + FilterDate[0] + " between '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + "' and '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + "'";

            return Where + WherePlus;
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
            //try
            //{
            for (int i = 0; i < FilterText.Count(); i++)
            {
                cmbFilter1.Items.Add(FilterText[i]);
                cmbFilter2.Items.Add(FilterText[i]);
                cmbFilter3.Items.Add(FilterText[i]);
                cmbFilter4.Items.Add(FilterText[i]);
                cmbFilter5.Items.Add(FilterText[i]);
                cmbFilter6.Items.Add(FilterText[i]);
            }

            if (FilterDate != null)
            {
                for (int i = 0; i < FilterDate.Count(); i++)
                {
                    cmbDate1.Items.Add(FilterDate[i]);
                    cmbDate2.Items.Add(FilterDate[i]);
                }
            }
            if (FilterText.Count() == 0)
            {
                cmbFilter1.Enabled = false;
                cmbFilter2.Enabled = false;
                cmbFilter3.Enabled = false;
                cmbFilter4.Enabled = false;
                cmbFilter5.Enabled = false;
                cmbFilter6.Enabled = false;

                chkFilter1.Enabled = false;
                chkFilter2.Enabled = false;
                chkFilter3.Enabled = false;
                chkFilter4.Enabled = false;
                chkFilter5.Enabled = false;
                chkFilter6.Enabled = false;

                txtFilter1.Enabled = false;
                txtFilter2.Enabled = false;
                txtFilter3.Enabled = false;
                txtFilter4.Enabled = false;
                txtFilter5.Enabled = false;
                txtFilter6.Enabled = false;
            }

            if (FilterText.Count() == 0)
            {
                cmbDate1.Enabled = false;
                cmbDate2.Enabled = false;

                chkDate1.Enabled = false;
                chkDate2.Enabled = false;

                DateFrom1.Enabled = false;
                DateTo1.Enabled = false;
                DateFrom2.Enabled = false;
                DateTo2.Enabled = false;
            }
            //}
            //catch (Exception Ex)
            //{
            //    MessageBox.Show(ConnectionString.GlobalException(Ex));
            //}
            //finally
            //{
            //    Conn.Close();
            //}
        }

        private void CheckValidationFilter()
        {
            if (FilterText.Count() == 0)
            {
                cmbFilter1.Enabled = false;
                cmbFilter2.Enabled = false;
                cmbFilter3.Enabled = false;
                cmbFilter4.Enabled = false;
                cmbFilter5.Enabled = false;
                cmbFilter6.Enabled = false;

                chkFilter1.Enabled = false;
                chkFilter2.Enabled = false;
                chkFilter3.Enabled = false;
                chkFilter4.Enabled = false;
                chkFilter5.Enabled = false;
                chkFilter6.Enabled = false;

                txtFilter1.Enabled = false;
                txtFilter2.Enabled = false;
                txtFilter3.Enabled = false;
                txtFilter4.Enabled = false;
                txtFilter5.Enabled = false;
                txtFilter6.Enabled = false;
            }
            else
            {
                if (chkFilter1.Checked == false)
                {
                    cmbFilter1.Enabled = false;
                    txtFilter1.Enabled = false;
                }
                else
                {
                    cmbFilter1.Enabled = true;
                    txtFilter1.Enabled = true;
                }

                if (chkFilter2.Checked == false)
                {
                    cmbFilter2.Enabled = false;
                    txtFilter2.Enabled = false;
                }
                else
                {
                    cmbFilter2.Enabled = true;
                    txtFilter2.Enabled = true;
                }

                if (chkFilter3.Checked == false)
                {
                    cmbFilter3.Enabled = false;
                    txtFilter3.Enabled = false;
                }
                else
                {
                    cmbFilter3.Enabled = true;
                    txtFilter3.Enabled = true;
                }

                if (chkFilter4.Checked == false)
                {
                    cmbFilter4.Enabled = false;
                    txtFilter4.Enabled = false;
                }
                else
                {
                    cmbFilter4.Enabled = true;
                    txtFilter4.Enabled = true;
                }

                if (chkFilter5.Checked == false)
                {
                    cmbFilter5.Enabled = false;
                    txtFilter5.Enabled = false;
                }
                else
                {
                    cmbFilter5.Enabled = true;
                    txtFilter5.Enabled = true;
                }

                if (chkFilter6.Checked == false)
                {
                    cmbFilter6.Enabled = false;
                    txtFilter6.Enabled = false;
                }
                else
                {
                    cmbFilter6.Enabled = true;
                    txtFilter6.Enabled = true;
                }

            }

            if (FilterText.Count() == 0)
            {
                cmbDate1.Enabled = false;
                cmbDate2.Enabled = false;

                chkDate1.Enabled = false;
                chkDate2.Enabled = false;

                DateFrom1.Enabled = false;
                DateTo1.Enabled = false;
                DateFrom2.Enabled = false;
                DateTo2.Enabled = false;
            }
            else
            {
                if (chkDate1.Checked == false)
                {
                    DateFrom1.Enabled = false;
                    DateTo1.Enabled = false;
                    cmbDate1.Enabled = false;
                }
                else
                {
                    DateFrom1.Enabled = true;
                    DateTo1.Enabled = true;
                    cmbDate1.Enabled = true;
                }

                if (chkDate2.Checked == false)
                {
                    DateFrom2.Enabled = false;
                    DateTo2.Enabled = false;
                    cmbDate2.Enabled = false;
                }
                else
                {
                    DateFrom2.Enabled = true;
                    DateTo2.Enabled = true;
                    cmbDate2.Enabled = true;
                }
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


        private void chkFilter1_CheckedChanged(object sender, EventArgs e)
        {
            CheckValidationFilter();
        }

        private void chkFilter2_CheckedChanged(object sender, EventArgs e)
        {
            CheckValidationFilter();
        }

        private void chkFilter3_CheckedChanged(object sender, EventArgs e)
        {
            CheckValidationFilter();
        }

        private void chkFilter4_CheckedChanged(object sender, EventArgs e)
        {
            CheckValidationFilter();
        }

        private void chkFilter5_CheckedChanged(object sender, EventArgs e)
        {
            CheckValidationFilter();
        }

        private void chkFilter6_CheckedChanged(object sender, EventArgs e)
        {
            CheckValidationFilter();
        }

        private void chkDate1_CheckedChanged(object sender, EventArgs e)
        {
            CheckValidationFilter();
        }

        private void chkDate2_CheckedChanged(object sender, EventArgs e)
        {
            CheckValidationFilter();
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

        private void SearchQuery_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void btnSearchTStatus_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "TransStatusTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            tbxTStatus1.Text = ConnectionString.Kode;
            tbxTStatus2.Text = ConnectionString.Kode2;
        }




    }
}
