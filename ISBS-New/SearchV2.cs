using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

//BY: HC
namespace ISBS_New
{
    public partial class SearchV2 : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        /**********SQL*********/

        /**********PAGE*********/
        int Limit1, Limit2, Total, Page1, Page2, Index;
        public static int dataShow;
        /**********PAGE*********/

        public static List<string> data = new List<string>(); //PASS DATA
        public static List<string> data2 = new List<string>(); //PASS DATA
        public static List<string> data3 = new List<string>(); //PASS DATA

        List<string> tableCols = new List<string>();
        List<string> tableHeader = new List<string>();
        private string mode;
        private string SchemaName;
        private string TableName;
        private string WherePlus;
        private string tableName2;
        private string colsName;
        private List<string[]> criteria = new List<string[]>();
        List<string> PK = new List<string>();
        string colsString;
        int colsCount;
        int rowsCount;

        public SearchV2()
        {
            InitializeComponent();
        }

        private void PopUpItem_Load(object sender, EventArgs e)
        {
            data.Clear();
            data2.Clear();
            data3.Clear();
            cmbShowLoad();
            ModeLoad();
            RefreshGrid();
            gvFormat();
            if (mode == "Check")
            {
                cbCheck.Visible = true;

                lblPage.Visible = false; txtPage.Visible = false;
                btnPrev.Visible = false; btnMPrev.Visible = false;
                btnNext.Visible = false; btnMNext.Visible = false;
                cmbShow.Visible = false;
            }
            else
            {
                cbCheck.Visible = false;

                lblPage.Visible = true; txtPage.Visible = true;
                btnPrev.Visible = true; btnMPrev.Visible = true;
                btnNext.Visible = true; btnMNext.Visible = true;
                cmbShow.Visible = true;
            }
        }

        private void ModeLoad()
        {
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            //CRITERIA

            cbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName, FieldName From [User].[Table] Where SchemaName = '" + SchemaName + "' And TableName = '" + TableName + "' and Filter1 = 1";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                criteria.Add(new string[] { Dr[0].ToString(), Dr[1].ToString() });
            }
            Conn.Close();

            for (int i = 0; i < criteria.Count; i++)
                cbCriteria.Items.Add(criteria[i][0]);
            cbCriteria.SelectedIndex = 0;
        }

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox] ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            cmbShow.Items.Clear();
            while (Dr.Read())
            {
                cmbShow.Items.Add(Dr.GetInt32(0));
            }
            Dr.Close();
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShow.SelectedIndex = 0;
        }

        public void SetMode(string mode)
        {
            this.mode = mode;
        }

        public void SetSchemaTable(string Schema, string Table, string Where, string colsName, string tableName2)
        {
            SchemaName = Schema;
            TableName = Table;
            WherePlus = Where;
            this.colsName = colsName; //LOM KEPAKE
            this.tableName2 = tableName2;
        }

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            //SET PAGE
            Query = "Select count(*) from " + tableName2 + " where 1=1 ";
            if (tbxSearch.Text.Trim() != "")
            {
                if (cbCriteria.SelectedIndex == 0)
                {
                    if (criteria.Count != 0)
                    {
                        Query += " and (";
                        for (int i = 0; i < criteria.Count; i++)
                        {
                            if (i >= 1)
                                Query += " or ";
                            Query += criteria[i][1] + " like '%" + tbxSearch.Text + "%'";
                        }
                        Query += ") ";
                    }
                }
                else
                    Query += " and " + criteria[cbCriteria.SelectedIndex - 1][1] + " like '%" + tbxSearch.Text + "%' ";
            }
            Query += WherePlus;
            Cmd = new SqlCommand(Query, Conn);
            rowsCount = (Int32)Cmd.ExecuteScalar(); //COUNT ROWS
            Total = rowsCount; //Total = (Int32)Cmd.ExecuteScalar();
            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

            dataGridView1.Rows.Clear();
            tableCols.Clear();
            tableHeader.Clear();
            colsString = "";

            if (mode == "Check")
            {
                tableCols.Add("Check");
                tableHeader.Add("Check");
            }
            else
            {
                tableCols.Add("No");
                tableHeader.Add("No");
            }

            //GET PK & COLUMN NAME
            PK.Clear();
            Cmd = new SqlCommand("Select FieldName, PK, Select1, DisplayName from [ISBS-NEW4].[User].[Table] where SchemaName='" + SchemaName + "' and TableName = " + "'" + TableName + "'" + " order by OrderNo", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["PK"] != (object)DBNull.Value && Convert.ToInt32(Dr["PK"]) == 1)
                    PK.Add(Dr["FieldName"].ToString());
                if (Dr["Select1"] != (object)DBNull.Value && Convert.ToInt32(Dr["Select1"]) == 1)
                {
                    tableCols.Add(Dr["FieldName"].ToString());
                    tableHeader.Add(Dr["DisplayName"].ToString());
                }
            }
            Dr.Close();

            //COMBINE ALL COLUMN NAME INTO STRING
            for (int i = 1; i < tableCols.Count; i++)
            {
                if (!(String.IsNullOrEmpty(tableCols[i])))
                {
                    if (i >= 2)
                        colsString += ", ";
                    colsString += "a." + tableCols[i];
                }
            }

            //COUNT COLUMN
            Query = "SELECT Count(*) FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + TableName + "' and COLUMN_NAME in (";
            for (int i = 0; i < tableCols.Count; i++)
            {
                if (!(String.IsNullOrEmpty(tableCols[i])))
                {
                    if (i >= 1)
                        Query += ", ";
                    Query += "'" + tableCols[i] + "'";
                }
            }
            Query += "); ";
            Cmd = new SqlCommand(Query, Conn);
            colsCount = (Int32)Cmd.ExecuteScalar();
            colsCount += 1; //FOR NO OR CHECK

            //COUNT ROWS
            //Query = "Select count(*) from " + tableName2 + " where 1=1 ";
            //if (tbxSearch.Text.Trim() != "")
            //{
            //    if (cbCriteria.SelectedIndex == 0)
            //    {
            //        if (criteria.Count != 0)
            //        {
            //            Query += " and (";
            //            for (int i = 0; i < criteria.Count; i++)
            //            {
            //                if (i >= 1)
            //                    Query += " or ";
            //                Query += criteria[i][1] + " like '%" + tbxSearch.Text + "%'";
            //            }
            //            Query += ") ";
            //        }
            //    }
            //    else
            //        Query += " and " + criteria[cbCriteria.SelectedIndex - 1][1] + " like '%" + tbxSearch.Text + "%' ";
            //}
            //Query += WherePlus;
            //Cmd = new SqlCommand(Query, Conn);
            //rowsCount = (Int32)Cmd.ExecuteScalar();

            //SET GV COLS HEADER
            dataGridView1.ColumnCount = colsCount;
            for (int i = 0; i < tableCols.Count; i++)
            {
                if (!(String.IsNullOrEmpty(tableCols[i])))
                {
                    dataGridView1.Columns[i].Name = tableCols[i];
                    dataGridView1.Columns[i].HeaderText = tableHeader[i];
                }
            }

            if (mode == "Check")
            {
                if (rowsCount != 0)
                    dataGridView1.Rows.Add(rowsCount);
            }
            else
            {
                //Cmd = new SqlCommand(Query, Conn);
                //rowsCount = (Int32)Cmd.ExecuteScalar();

                if (Convert.ToInt32(txtPage.Text) == Page2)
                {
                    if (Total == Convert.ToInt32(cmbShow.Text) || Total % Convert.ToInt32(cmbShow.Text) == 0)
                        dataGridView1.Rows.Add(Convert.ToInt32(cmbShow.Text));
                    else
                        dataGridView1.Rows.Add(Total % Convert.ToInt32(cmbShow.Text));
                }
                else
                {
                    if (rowsCount != 0)
                    {
                        if (rowsCount > Int32.Parse(cmbShow.Text))
                            rowsCount = Int32.Parse(cmbShow.Text);
                        dataGridView1.Rows.Add(Convert.ToInt32(rowsCount));
                    }
                }
            }

            //LOAD DATA
            Query = "Select * from ( Select ROW_NUMBER() OVER (ORDER BY a." + PK[0] + " desc) No, * from ( Select " + colsString + " from " + tableName2 + " where 1=1 " + WherePlus + ")a where 1=1 ";
            if (tbxSearch.Text.Trim() != "")
            {
                if (cbCriteria.SelectedIndex == 0)
                {
                    if (criteria.Count != 0)
                    {
                        Query += " and (";
                        for (int i = 0; i < criteria.Count; i++)
                        {
                            if (i >= 1)
                                Query += " or ";
                            Query += criteria[i][1] + " like '%" + tbxSearch.Text + "%'";
                        }
                        Query += ") ";
                    }
                }
                else
                    Query += " and " + criteria[cbCriteria.SelectedIndex - 1][1] + " like '%" + tbxSearch.Text + "%' ";
            }
            if (mode == "Check")
                Query += " )a Where No Between " + Limit1 + " and " + rowsCount + " ;";
            else
                Query += " )a Where No Between " + Limit1 + " and " + Limit2 + " ;";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int RowIndex = 0;
            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                for (int i = 0; i < colsCount; i++)
                {
                    if (i == 0 && mode == "Check")
                    {
                        dataGridView1.Rows[RowIndex].Cells[i] = chk;
                        dataGridView1.Rows[RowIndex].Cells[i].Value = false;
                    }
                    else
                        dataGridView1.Rows[RowIndex].Cells[i].Value = Dr[i];
                }
                RowIndex++;
            }
            Dr.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            cbCheck.Checked = false;
            RefreshGrid();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {          
            SelectData();
        }

        private void cbCheck_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = cbCheck.Checked;
            for (int i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                dataGridView1.Rows[i].Cells["Check"].Value = Check;
            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            cmbShow_SelectedIndexChanged(new object(), new EventArgs());
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

            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void SelectData()
        {
            if (dataGridView1.RowCount < 1)
            {
                MessageBox.Show("Datagrid empty, no data was selected!");
                return;
            }

            data.Clear();
            data2.Clear();
            PK.Clear();
            if (mode != "Check") //NO
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select FieldName from [ISBS-NEW4].[User].[Table] where SchemaName='" + SchemaName + "' and TableName= '" + TableName + "' and Select1=1 order by OrderNo";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    data.Add(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[Dr["FieldName"].ToString()].Value.ToString());
                }
                Dr.Close();
                Conn.Close();
            }
            else //CHECK
            {
                //GET PK
                Cmd = new SqlCommand("Select FieldName from [ISBS-NEW4].[User].[Table] where SchemaName='" + SchemaName + "' and TableName = " + "'" + TableName + "'" + " and PK=1 order by OrderNo", Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    PK.Add(Dr["FieldName"].ToString());
                }

                //GET PK
                //Cmd = new SqlCommand("Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = " + "'" + TableName + "'" + " and PK=1 order by OrderNo", Conn);
                //PK = Cmd.ExecuteScalar().ToString();

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["Check"].Value.ToString() == "True")
                    {
                        //for (int j = 0; j < PK.Count; j++)
                        //{
                        data.Add(dataGridView1.Rows[i].Cells[PK[0]].Value.ToString());
                        if (PK.Count == 2)
                            data2.Add(dataGridView1.Rows[i].Cells[PK[1]].Value.ToString());
                        else if (PK.Count == 3)
                        {
                            data2.Add(dataGridView1.Rows[i].Cells[PK[1]].Value.ToString());
                            data3.Add(dataGridView1.Rows[i].Cells[PK[2]].Value.ToString());
                        }
                        //}
                    }
                }
            }
            this.Close();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (mode != "Check")
            {
                if (dataGridView1.RowCount > 0)
                {
                    SelectData();
                }
                this.Close();
            }
        }

        private void tbxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyData)
            {
                btnSearch_Click(new object(), new EventArgs());
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (TableName == "Pricelist_Dtl") //SQ && SA
                dataGridView1.Columns["SeqNo"].Visible = false;
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Base") && mode == "Check") //SQ && SA
            {
                if (dataGridView1.Rows[e.RowIndex].Cells["Base"].Value.ToString() == "Y")
                {
                    dataGridView1.Rows[e.RowIndex].Cells["Check"].Value = true;
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView1.Rows[e.RowIndex].ReadOnly = true;
                }
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Check"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Qty_Alt") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ConvertionRatio") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DiscPercent") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("LockQty"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if ((dataGridView1.Columns[e.ColumnIndex].Name.Contains("Amount") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Price") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Total")) && (!(dataGridView1.Columns[e.ColumnIndex].Name.Contains("PriceListNo") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("PriceType"))))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";

                double num;
                bool isNum = double.TryParse(e.Value.ToString(), out num);
                if (isNum)
                {
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N4");
                    dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Filter f = new Filter();
            f.SetSchemaTable(SchemaName, TableName, WherePlus);
            f.ShowDialog();
            if (f.where != null)
                RefreshGrid2(f.where);
        }

        private void RefreshGrid2(string whereQuery)
        {
            Conn = ConnectionString.GetConnection();
            //SET PAGE
            Query = "Select count(*) from (Select " + colsString + " from " + tableName2 + " where 1=1 " + WherePlus + ") a " + whereQuery;
            Cmd = new SqlCommand(Query, Conn);
            Total = (Int32)Cmd.ExecuteScalar();
            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

            dataGridView1.Rows.Clear();
            tableCols.Clear();
            colsString = "";

            if (mode == "Check")
                tableCols.Add("Check");
            else
                tableCols.Add("No");

            //GET PK & COLUMN NAME
            PK.Clear();
            Cmd = new SqlCommand("Select FieldName, PK, Select1 from [ISBS-NEW4].[User].[Table] where SchemaName='" + SchemaName + "' and TableName = " + "'" + TableName + "'" + " order by OrderNo", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["PK"] != (object)DBNull.Value && Convert.ToInt32(Dr["PK"]) == 1)
                    PK.Add(Dr["FieldName"].ToString());
                if (Dr["Select1"] != (object)DBNull.Value && Convert.ToInt32(Dr["Select1"]) == 1)
                    tableCols.Add(Dr["FieldName"].ToString());
            }
            Dr.Close();

            //COMBINE ALL COLUMN NAME INTO STRING
            for (int i = 1; i < tableCols.Count; i++)
            {
                if (!(String.IsNullOrEmpty(tableCols[i])))
                {
                    if (i >= 2)
                        colsString += ", ";
                    colsString += "a." + tableCols[i];
                }
            }

            //COUNT COLUMN
            Query = "SELECT Count(*) FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + TableName + "' and COLUMN_NAME in (";
            for (int i = 0; i < tableCols.Count; i++)
            {
                if (!(String.IsNullOrEmpty(tableCols[i])))
                {
                    if (i >= 1)
                        Query += ", ";
                    Query += "'" + tableCols[i] + "'";
                }
            }
            Query += "); ";
            Cmd = new SqlCommand(Query, Conn);
            colsCount = (Int32)Cmd.ExecuteScalar();
            colsCount += 1; //FOR NO OR CHECK

            //SET GV COLS HEADER
            dataGridView1.ColumnCount = colsCount;
            for (int i = 0; i < tableCols.Count; i++)
            {
                if (!(String.IsNullOrEmpty(tableCols[i])))
                    dataGridView1.Columns[i].Name = tableCols[i];
            }

            //COUNT ROWS
            Query = "Select count(*) from (Select " + colsString + " from " + tableName2 + " where 1=1 " + WherePlus + ") a " + whereQuery;
            Cmd = new SqlCommand(Query, Conn);
            rowsCount = (Int32)Cmd.ExecuteScalar();

            if (mode == "Check")
            {
                if (rowsCount != 0)
                    dataGridView1.Rows.Add(rowsCount);
            }
            else
            {
                Cmd = new SqlCommand(Query, Conn);
                rowsCount = (Int32)Cmd.ExecuteScalar();

                if (Convert.ToInt32(txtPage.Text) == Page2)
                {
                    if (Total == Convert.ToInt32(cmbShow.Text) || Total % Convert.ToInt32(cmbShow.Text) == 0)
                        dataGridView1.Rows.Add(Convert.ToInt32(cmbShow.Text));
                    else
                        dataGridView1.Rows.Add(Total % Convert.ToInt32(cmbShow.Text));
                }
                else
                {
                    if (rowsCount != 0)
                    {
                        if (rowsCount > Int32.Parse(cmbShow.Text))
                            rowsCount = Int32.Parse(cmbShow.Text);
                        dataGridView1.Rows.Add(Convert.ToInt32(rowsCount));
                    }
                }
            }

            //LOAD DATA
            Query = "Select * from ( Select ROW_NUMBER() OVER (ORDER BY a." + PK[0] + " desc) No, * from ( Select " + colsString + " from " + tableName2 + " where 1=1 " + WherePlus;
            if (mode == "Check")
                Query += ")a " + whereQuery + " )a; ";
            else
                Query += ")a " + whereQuery + " ) a Where No Between " + Limit1 + " and " + Limit2 + " ;";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int RowIndex = 0;
            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                for (int i = 0; i < colsCount; i++)
                {
                    if (i == 0 && mode == "Check")
                    {
                        dataGridView1.Rows[RowIndex].Cells[i] = chk;
                        dataGridView1.Rows[RowIndex].Cells[i].Value = false;
                    }
                    else
                        dataGridView1.Rows[RowIndex].Cells[i].Value = Dr[i];
                }
                RowIndex++;
            }
            Dr.Close();
        }

        private void gvFormat()
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (i == 0 && mode == "Check")
                    dataGridView1.Columns[i].ReadOnly = false;
                else
                    dataGridView1.Columns[i].ReadOnly = true;

            }
            dataGridView1.AllowUserToAddRows = false;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                if (dataGridView1.Columns[i].Name.Contains("Check"))
                    dataGridView1.Columns[i].Width = 45;
                else
                    dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void cbCriteria_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
