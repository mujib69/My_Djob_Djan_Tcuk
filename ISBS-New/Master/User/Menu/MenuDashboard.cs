﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.User.Menu
{
    public partial class MenuDashboard : Form
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
        Timer timerRefresh = new Timer();

        //Penanda untuk paging grid
        private int Limit1, Limit2, Total, Page1, Page2;

        public string SchemaName;
        public string TableName;

        //Flag Proses Sarch
        string IdNamePK = "";
        string Type = "";

        private Boolean tmpAmbilKodeNo = false;
        public string Kode, Kode2, Kode3, Kode4;


        public MenuDashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            try
            {
                SchemaName = "User";
                TableName = "Menu";
                this.Text = "Dashboard " + SchemaName + " " + TableName;
                Conn = ConnectionString.GetConnection();
                ModeLoad();
                this.Location = new Point(148, 47);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
        }


        public void SetSchemaTable(Form Forms, string Schema, string Table)
        {
            SchemaName = Schema;
            TableName = Table;
        }

        private void LoadFilter()
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                int i = 1;
                Query = "Select DisplayName from [User].[Table] where TableName = '" + TableName + "' order by OrderNo";

                if (tmpAmbilKodeNo == true)
                    cmbCriteria.Items.Add("*");

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 0;
                    cmbCriteria.Items.Add("All");
                    while (Dr.Read())
                    {
                        cmbCriteria.Items.Add(Dr[0]);
                        if (tmpAmbilKodeNo == false)
                            if (i == 0)
                                cmbCriteria.Text = "All";
                        i += 1;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
        }

        public void RefreshGrid()
        {
            try
            {
                int i = 1;
                string TempFilter = "";
                string tmpFieldName = "";
                RefreshGridStatus = true;

                Conn = ConnectionString.GetConnection();

                //Get PK
                Query = "Select FieldName from [User].[Table] where TableName = " + "'" + TableName + "'" + " and PK=1 order by OrderNo";
                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    IdNamePK = Command.ExecuteScalar().ToString();

                }

                //Get List Field
                Query = "Select FieldName from [User].[Table] where TableName = " + "'" + TableName + "'" + " order by OrderNo";

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
                Query = "Select FieldName from [User].[Table] where TableName = " + "'" + TableName + "'" + " order by OrderNo";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 0;
                    while (Dr.Read())
                    {
                        if (i == 0)
                            TempFilter = Dr[0].ToString();
                        else
                            TempFilter += TempFilter + "," + Dr[0].ToString() + " like '%%'";
                        i += 1;
                    }
                    Dr.Close();
                }

                string Filter = "";
                if (cmbCriteria.Text == "All")
                {
                    Query = "Select FieldName From [User].[Table] where SchemaName='" + SchemaName + "' and TableName='" + TableName + "'";
                    using (SqlCommand Command = new SqlCommand(Query, Conn))
                    {
                        SqlDataReader Dr = Command.ExecuteReader();
                        i = 0;
                        while (Dr.Read())
                        {
                            if (i == 0)
                                Filter = Dr[0].ToString() + " like '%" + txtSearch.Text.Trim() + "%'";
                            else
                                Filter += " or " + Dr[0].ToString() + " like '%" + txtSearch.Text.Trim() + "%'";
                            i += 1;
                        }
                        Dr.Close();
                    }
                }
                else
                {
                    //Get FilterId Select
                    Query = "Select FieldName From [User].[Table] where DisplayName = '" + cmbCriteria.Text + "' and SchemaName='" + SchemaName + "' and TableName='" + TableName + "'";
                    using (SqlCommand Command = new SqlCommand(Query, Conn))
                    {
                        Filter = Command.ExecuteScalar().ToString() + " like '%" + txtSearch.Text.Trim() + "%'";
                    }
                }

                //Query to display datagrid
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY " + IdNamePK + " ) No," + tmpFieldName + " From [" + SchemaName + "].[" + TableName + "] ";
                if (txtSearch.Text.Trim() != "")
                    Query += "where " + Filter;
                Query += ") a Where No Between " + Limit1 + " and " + Limit2 + " ;";

                DataTable Dt = new DataTable();
                using (SqlDataAdapter Da = new SqlDataAdapter(Query, Conn))
                {
                    Da.Fill(Dt);
                }
                dgvSearch.AutoGenerateColumns = true;
                dgvSearch.DataSource = Dt;
                dgvSearch.Refresh();
                dgvSearch.AutoResizeColumns();

                //Mengambil nilai total paging
                Query = "Select Count(" + IdNamePK + ") From [" + SchemaName + "].[" + TableName + "] ";
                if (txtSearch.Text.Trim() != "")
                    Query += "where " + Filter;
                Query += " ; ";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    Total = Convert.ToInt32(Command.ExecuteScalar());
                }

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling(Total / Convert.ToDecimal(cmbShow.Text));
                lblPage.Text = "/ " + Page2;

                //Untuk mengatur width & header grid name
                Query = "Select Width,DisplayName from [User].[Table] where TableName = '" + TableName + "' order by OrderNo";
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
            }
        }

        private void ModeLoad()
        {
            setTimer();
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

        private void SelectData()
        {
            try
            {
                if (dgvSearch.RowCount >= 1)
                {
                    Index = dgvSearch.CurrentRow.Index;

                    Conn = ConnectionString.GetConnection();
                    int i = 0;
                    Query = "Select FieldName from [User].[Table] where SchemaName ='" + SchemaName + "' and TableName = '" + TableName + "'" + " and PK=1 order by OrderNo";

                    string tmpFieldName = "";

                    using (SqlCommand Command = new SqlCommand(Query, Conn))
                    {
                        SqlDataReader Dr = Command.ExecuteReader();
                        i = 0;

                        while (Dr.Read())
                        {
                            if (i == 0)
                            {
                                //IdName = Dr[0].ToString();
                                tmpFieldName = Dr[0].ToString() + "='" + dgvSearch.Rows[Index].Cells[Dr[0].ToString()].Value.ToString() + "'";
                            }
                            else
                            {
                                tmpFieldName += "or " + Dr[0].ToString() + "='" + dgvSearch.Rows[Index].Cells[Dr[0].ToString()].Value.ToString() + "'";
                            }
                            i += 1;
                        }
                        tmpFieldName += ";";
                        Dr.Close();
                    }

                    //Get ID
                    Query = "Select " + IdNamePK + " from [" + SchemaName + "].[" + TableName + "] where " + tmpFieldName;
                    using (SqlCommand Command = new SqlCommand(Query, Conn))
                    {
                        ConnectionString.IdSearchPK = Command.ExecuteScalar().ToString();
                    }

                    Master.User.Menu.MenuForm F = new Master.User.Menu.MenuForm();
                    F.Show();
                    F.ParentRefreshGrid(this);
                    //F.id = ConnectionString.IdSearchPK;
                    ConnectionString.IdSearchPK = "";
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

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                RefreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text.Trim()) - 1) * Convert.ToInt32(cmbShow.Text.Trim()) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text.Trim()) * Convert.ToInt32(cmbShow.Text.Trim());
            RefreshGrid();
        }

        private void dgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectData();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text.Trim()) - 1) * Convert.ToInt32(cmbShow.Text.Trim()) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text.Trim()) * Convert.ToInt32(cmbShow.Text.Trim());
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

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Convert.ToInt32(cmbShow.Text) <= Total)
            {
                //Limit1 += Convert.ToInt32(cmbShow.Text);
                //Limit2 += Convert.ToInt32(cmbShow.Text);
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

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text) - 1) * Convert.ToInt32(cmbShow.Text) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text);
            if (RefreshGridStatus == true)
                RefreshGrid();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Master.User.Menu.MenuForm F = new Master.User.Menu.MenuForm();
            F.Show();
            F.ParentRefreshGrid(this);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Index = dgvSearch.CurrentRow.Index;
                if (Index >= 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Apakah data " + TableName + Environment.NewLine + dgvSearch.Columns[1].Name.ToString() + " : " + dgvSearch[1, Index].Value.ToString() + Environment.NewLine +
                        dgvSearch.Columns[2].Name.ToString() + " : " + dgvSearch[2, Index].Value.ToString() + " akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Conn = ConnectionString.GetConnection();
                        Query = "Delete from [" + SchemaName + "].[" + TableName + "] where " + IdNamePK + " = '" + dgvSearch[1, Index].Value.ToString() + "'";
                        Trans = Conn.BeginTransaction();
                        using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                        {
                            Command.ExecuteNonQuery();
                        }
                        Trans.Commit();
                        MessageBox.Show("Data " + TableName + Environment.NewLine + dgvSearch.Columns[1].Name.ToString() + " : " + dgvSearch[1, Index].Value.ToString() + Environment.NewLine +
                        dgvSearch.Columns[2].Name.ToString() + " : " + dgvSearch[2, Index].Value.ToString() + " berhasil dihapus.");
                    }
                    RefreshGrid();
                }
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Limit1 = (Convert.ToInt32(txtPage.Text)-1) * Convert.ToInt32(cmbShow.Text) +1;
                Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text); ;
                RefreshGrid();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1 == null)
            {

            }
            else
            {
                RefreshGrid();
            }
        }

        private void setTimer()
        {
            timerRefresh.Interval = (10 * 1000);//milisecond
            timerRefresh.Tick += new EventHandler(timer1_Tick);
            timerRefresh.Start();
        }

        private void MenuDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerRefresh.Stop();
        }
       
    }
}
