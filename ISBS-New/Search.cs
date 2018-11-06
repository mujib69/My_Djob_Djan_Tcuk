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
    public partial class Search : MetroFramework.Forms.MetroForm
    {
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

        public string SchemaName;
        public string TableName;
        public string WherePlus;

        //Flag Proses Sarch
        string IdNamePK = "";
        string Type = "";

        private Boolean tmpAmbilKodeNo = false;

        public Search()
        {
            InitializeComponent();
        }

        private void Search_Load(object sender, EventArgs e)
        {
            try
            {
                //hendry comment
                //this.Text = "Search Schema.Table = " + SchemaName + "." + TableName;
                Conn = ConnectionString.GetConnection();
                ModeLoad();
                //this.Location = new Point(148, 47);
                lblForm.Location = new Point(16, 11);
                lblForm.Text = this.Text;
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

        public void SetSchemaTable(string Schema, string Table)
        {
            SchemaName = Schema;
            TableName = Table;
        }

        public void SetSchemaTable(string Schema, string Table, string Where)
        {
            SchemaName = Schema;
            TableName = Table;
            WherePlus = Where;
        }

        private void LoadFilter()
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                int i = 1;
                Query = "Select DisplayName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = '" + TableName + "' order by OrderNo";

                if (tmpAmbilKodeNo == true)
                    cmbCriteria.Items.Add("*");

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 0;
                    while (Dr.Read())
                    {
                        cmbCriteria.Items.Add(Dr[0]);
                        if (tmpAmbilKodeNo == false)
                            if (i == 0)
                                cmbCriteria.Text = Dr[0].ToString();
                        i += 1;
                    }
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

        private void RefreshGrid()
        {
            try
            {
                RefreshGridStatus = true;
                int i = 1;
                string TempFilter = "";
                string tmpFieldName = "";

                Conn = ConnectionString.GetConnection();

                //Get PK
                Query = "Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = " + "'" + TableName + "'" + " and PK=1 order by OrderNo";
                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    IdNamePK = Command.ExecuteScalar().ToString();

                }

                //Get List Field
                Query = "Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = " + "'" + TableName + "'" + " order by OrderNo";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 0;

                    while (Dr.Read())
                    {
                        if (i == 0)
                        {
                            //IdName = Dr[0].ToString();
                            tmpFieldName = Dr[0].ToString();
                        }
                        else
                        {
                            tmpFieldName += "," + "cast(" + Dr[0].ToString() + " as varchar (100)) " + Dr[0].ToString() + " ";
                        }
                        i += 1;
                    }
                    Dr.Close();
                }

                //Get Data From Field
                //Query = "Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = " + "'" + TableName + "'" + " order by OrderNo";

                //using (SqlCommand Command = new SqlCommand(Query, Conn))
                //{
                //    SqlDataReader Dr = Command.ExecuteReader();
                //    i = 0;
                //    while (Dr.Read())
                //    {
                //        if (i == 0)
                //            TempFilter = Dr[0].ToString();
                //        else
                //            TempFilter += TempFilter + "," + Dr[0].ToString() + " like '%%'";
                //        i += 1;
                //    }
                //    Dr.Close();
                //}

                //Get FilterId Select
                Query = "Select FieldName From [User].[Table] where DisplayName = '" + cmbCriteria.Text + "'";
                string Filter;
                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    Filter = Command.ExecuteScalar().ToString();
                }

                //thad edit
                //Query to display datagrid
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY " + /*IdNamePK*/ "CreatedDate" + " desc ) No," + tmpFieldName + " From [" + SchemaName + "].[" + TableName + "] ";
                if (txtSearch.Text.Trim() != "")
                    Query += "where (" + Filter + " Like @search)" + WherePlus;
                else
                    Query += "where 1=1 " + WherePlus;
                Query += ") a Where No Between " + Limit1 + " and " + Limit2 + " ;";

                DataTable Dt = new DataTable();
                using (SqlDataAdapter Da = new SqlDataAdapter(Query, Conn))
                {
                    Da.SelectCommand.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
                //end=======
                    Da.Fill(Dt);
                }

                dgvSearch.AutoGenerateColumns = true;
                dgvSearch.DataSource = Dt;
                //dgvSearch.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                //dgvSearch.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
                dgvSearch.Refresh();
                

                Conn.Close();

                //thad edit
                //Mengambil nilai total paging
                Conn = ConnectionString.GetConnection();
                Query = "Select Count(" + IdNamePK + ") From [" + SchemaName + "].[" + TableName + "] ";

                //if (txtSearch.Text.Trim() != "")
                //    Query += "where (" + Filter + " Like '%" + txtSearch.Text.Trim() + "%')" + WherePlus;
                if (txtSearch.Text.Trim() != "")
                    Query += "where (" + Filter + " Like @search)" + WherePlus;
                else
                    Query += "where 1=1 " + WherePlus;
                Query += " ; ";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    Command.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
                    Total = Convert.ToInt32(Command.ExecuteScalar());
                }
                //end===============
                //Conn.Close();

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling(Total / Convert.ToDecimal(cmbShow.Text));
                lblPage.Text = "/ " + Page2;

                //Untuk mengatur width & header grid name
                Query = "Select Width,DisplayName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = '" + TableName + "' order by OrderNo";
                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 1;
                    while (Dr.Read())
                    {
                        //width
                        dgvSearch.Columns[i].Width = Convert.ToInt32(Dr[0].ToString() == "" ? "80" : Dr[0].ToString());
                        //header grid name
                        dgvSearch.Columns[i].HeaderText = Dr[1].ToString() == "" ? "" : Dr[1].ToString();
                        i += 1;
                    }
                    Dr.Close();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
                dgvSearch.AutoResizeColumns();
            }
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Convert.ToInt32(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
            LoadFilter();
            RefreshGrid();
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
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }


        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                RefreshGrid();
        }

        private void SelectData()
        {
            try
            {
                Variable = "Select";
                Index = dgvSearch.CurrentRow.Index;

                string Select = "";
                string Select2 = "";
                string Select3 = "";
                string Select4 = "";
                string Select5 = "";
                Conn = ConnectionString.GetConnection();
                Query = "Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName= '" + TableName + "' and Select1=1 order by OrderNo";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    Dr = Command.ExecuteReader();
                    int i = 0;
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            //ConnectionString.Kode[i] = dgvSearch.Rows[Index].Cells[Dr["FieldName"].ToString()].Value.ToString();
                            //Kode[i] = dgvSearch.Rows[Index].Cells[Dr["FieldName"].ToString()].ToString();
                            if (i == 0)
                            {
                                Select = Dr["FieldName"].ToString();
                                ConnectionString.Kode = dgvSearch.Rows[Index].Cells[Select].Value.ToString();
                            }
                            else if (i == 1)
                            {
                                Select2 = Dr["FieldName"].ToString();
                                ConnectionString.Kode2 = dgvSearch.Rows[Index].Cells[Select2].Value.ToString();
                            }
                            else if (i == 2)
                            {
                                Select3 = Dr["FieldName"].ToString();
                                ConnectionString.Kode3 = dgvSearch.Rows[Index].Cells[Select3].Value.ToString();
                            }
                            else if (i == 3)
                            {
                                Select4 = Dr["FieldName"].ToString();
                                ConnectionString.Kode4 = dgvSearch.Rows[Index].Cells[Select4].Value.ToString();
                            }
                            else if (i == 4)
                            {
                                Select5 = Dr["FieldName"].ToString();
                                ConnectionString.Kode5 = dgvSearch.Rows[Index].Cells[Select5].Value.ToString();
                            }

                            i += 1;
                        }
                    }
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


        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvSearch.RowCount > 0)
            {
                SelectData();
            }
            this.Close();
        }

        private void dgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSearch.RowCount > 0)
            {
                SelectData();
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Convert.ToInt32(cmbShow.Text) <= Total)
            {
                Limit1 += Convert.ToInt32(cmbShow.Text);
                Limit2 += Convert.ToInt32(cmbShow.Text);
                txtPage.Text = (Convert.ToInt32(txtPage.Text) + 1).ToString();
            }
            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = ((int)Math.Ceiling(Total / Convert.ToDecimal(cmbShow.Text))).ToString();

            Limit1 = (Convert.ToInt32(txtPage.Text) - 1) * Convert.ToInt32(cmbShow.Text) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text);
            RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Convert.ToInt32(cmbShow.Text) >= 1)
            {
                Limit1 -= Convert.ToInt32(cmbShow.Text);
                Limit2 -= Convert.ToInt32(cmbShow.Text);
                txtPage.Text = (Convert.ToInt32(txtPage.Text) - 1).ToString();
            }
            RefreshGrid();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text.Trim()) - 1) * Convert.ToInt32(cmbShow.Text.Trim()) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text.Trim()) * Convert.ToInt32(cmbShow.Text.Trim());
            RefreshGrid();
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text.Trim()) - 1) * Convert.ToInt32(cmbShow.Text.Trim()) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text.Trim()) * Convert.ToInt32(cmbShow.Text.Trim());
            if(RefreshGridStatus==true)
            RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            ConnectionString.Kode = "";
            ConnectionString.Kode2 = "";
            ConnectionString.Kode3 = "";
            ConnectionString.Kode4 = "";
            ConnectionString.Kode5 = "";
            this.Close();
            
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Limit1 = (Convert.ToInt32(txtPage.Text) - 1) * Convert.ToInt32(cmbShow.Text) + 1;
                Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text); ;
                RefreshGrid();
            }
        }

        private void Search_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Variable != "Select")
            {
                ConnectionString.Kode = "";
                ConnectionString.Kode2 = "";
                ConnectionString.Kode3 = "";
                ConnectionString.Kode4 = "";
                ConnectionString.Kode5 = "";
            }
        }

        private void Search_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void dgvSearch_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvSearch.Columns[e.ColumnIndex].Name.Contains("Luas"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            //DATE FORMAT
            if (dgvSearch.Columns[e.ColumnIndex].Name == "CreatedDate" || dgvSearch.Columns[e.ColumnIndex].Name == "UpdatedDate" || dgvSearch.Columns[e.ColumnIndex].Name.Contains("ValidTo"))
                dgvSearch.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            else if (dgvSearch.Columns[e.ColumnIndex].Name.Contains("Date"))
                dgvSearch.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        //private void btnSearch_Click(object sender, EventArgs e)
        //{
        //    RefreshGrid();
        //}

        //private void btnExit_Click(object sender, EventArgs e)
        //{
        //    ConnectionString.Kode = "";
        //    ConnectionString.Kode2 = "";
        //    ConnectionString.Kode3 = "";
        //    ConnectionString.Kode4 = "";
        //    ConnectionString.Kode5 = "";
        //    this.Close();

        //}

        //private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == 13)
        //    {
        //        Limit1 = (Convert.ToInt32(txtPage.Text) - 1) * Convert.ToInt32(cmbShow.Text) + 1;
        //        Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text); ;
        //        RefreshGrid();
        //    }
        //}

        //private void Search_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if (Variable != "Select")
        //    {
        //        ConnectionString.Kode = "";
        //        ConnectionString.Kode2 = "";
        //        ConnectionString.Kode3 = "";
        //        ConnectionString.Kode4 = "";
        //        ConnectionString.Kode5 = "";
        //    }
        //}

        //private void Search_Shown(object sender, EventArgs e)
        //{
        //    this.Location = new Point(170, 63);
        //}

    }
}
