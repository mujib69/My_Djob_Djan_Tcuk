using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Utility
{
    public partial class Frm_SearchMgr : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConTrans;
        DataSet Ds = new DataSet();
        SqlCommand cmdSearch = new SqlCommand();
        SqlCommand cmdSearchGrid = new SqlCommand();
        SqlDataAdapter DaSearch = new SqlDataAdapter();
        SqlDataReader Dr;

        public Frm_SearchMgr()
        {
            InitializeComponent();
        }

        private void Frm_SearchMgr_Load(object sender, EventArgs e)
        {
            if (ControlMgr.tmpNamaFormSearch != "")
            {
                this.Text = ControlMgr.tmpNamaFormSearch;
            }
            GC.Collect();

            //string tmpFieldNameComma = "";
            string tmpFieldName = "";

            try
            {
                ConTrans = ISBS_New.ConnectionString.GetConnection();
                cmdSearch.Connection = ConTrans;
                cmdSearch.CommandType = CommandType.Text;
                cmdSearch.CommandText = "Select TblFind_FieldNameFilter from TblFind where TblFind_TblName = " + "'" + ControlMgr.TblName + "'" + " order by TblFind_NoUrut";
                Dr = cmdSearch.ExecuteReader();
                try
                {
                    while (Dr.Read())
                    {
                        if (tmpFieldName == "")
                        {
                            tmpFieldName = "cast(" + Dr[0] + " as varchar (100))";
                        }
                        else
                        {
                            tmpFieldName += "," + "cast(" + Dr[0] + " as varchar (100))";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Dr.Close();
                }

                string strSql = "";
                strSql = "Select " + tmpFieldName + " from " + ControlMgr.TblName;
                if (ControlMgr.tmpWhere != "")
                {
                    strSql = strSql + " " + ControlMgr.tmpWhere;
                }
                if (ControlMgr.tmpSort != "")
                {
                    strSql = strSql + " " + ControlMgr.tmpSort;
                }

                cmdSearchGrid.CommandText = strSql;
                cmdSearchGrid.CommandType = CommandType.Text;
                cmdSearchGrid.Connection = ConTrans;
                DaSearch.SelectCommand = cmdSearchGrid;
                DaSearch.Fill(Ds, "Result");
                cmdSearch.CommandText = "Select TblFind_CustomFieldNameFilter from TblFind where TblFind_TblName = " + "'" + ControlMgr.TblName + "'" + " order by TblFind_NoUrut";
                Dr = cmdSearch.ExecuteReader();

                try
                {
                    int i = 0;
                    while (Dr.Read())
                    {
                        CboCriteria.Items.Add(Dr[0]);
                        if (!ControlMgr.tmpAmbilKodeNo)
                        {
                            if (i == 0)
                            {
                                CboCriteria.Text = Dr[0].ToString();
                            }
                            Ds.Tables["Result"].Columns[i].ColumnName = Dr[0].ToString();
                            i += 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Dr.Close();
                }

                CboCriteria.Items.Add("*");
                if (ControlMgr.tmpAmbilKodeNo)
                {
                    DataGridView1.DataSource = Ds.Tables["Result"];
                    CboCriteria.Text = "*";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            CboCriteria.DropDownStyle = ComboBoxStyle.DropDownList;
            ControlMgr.tmpAmbilKodeNo = false; 
        }

        private void Frm_SearchMgr_FormClosed(object sender, FormClosedEventArgs e)
        {
            Ds.Clear();
            ConTrans.Close();
            GC.Collect();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string strSql = "";

            if (txtSearch.Text == "")
            {
                MessageBox.Show("Please fill in Criteria first");
                return;
            }

            DataView Dv = new DataView();
            Dv.Table = Ds.Tables["Result"];

            if (CboCriteria.Text != "*")
            {
                strSql = "[" + CboCriteria.Text + "]" + " Like '%" + txtSearch.Text + "%'";
            }
            else
            {
                foreach (DataColumn col in Dv.Table.Columns)
                {
                    if (strSql == "")
                    {
                        strSql = "[" + col.ColumnName + "]" + " Like '%" + txtSearch.Text + "%'";
                    }
                    else
                    {
                        strSql += " OR " + "[" + col.ColumnName + "]" + " Like '%" + txtSearch.Text + "%'";
                    }
                }
            }
            Dv.RowFilter = strSql;
            DataGridView1.DataSource = Dv;
            DefineWidthofDataGridView();
        }

        private void DefineWidthofDataGridView()
        {
            ConTrans = ISBS_New.ConnectionString.GetConnection();
            cmdSearch.Connection = ConTrans;
            cmdSearch.CommandType = CommandType.Text;
            cmdSearch.CommandText = "Select TblFind_NoUrut,TblFind_Width from TblFind where TblFind_TblName = " + "'" + ControlMgr.TblName + "'" + " order by TblFind_NoUrut";
            Dr = cmdSearch.ExecuteReader();
            int i = 0;

            try
            {
                while (Dr.Read())
                {
                    DataGridView1.Columns[i].Width = Convert.ToInt32(Dr["tblFind_Width"]);
                    if (Convert.ToInt32(Dr["tblFind_NoUrut"]) >= 20 && Convert.ToInt32(Dr["tblFind_NoUrut"]) < 30)
                    {
                        DataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        DataGridView1.Columns[i].DefaultCellStyle.Format = "#,##0.00";
                    }
                    i += 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Dr.Close();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                BtnSearch.PerformClick();
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string tmpPK = "";
                string tmpPK2 = "";
                string tmpPK3 = "";
                string tmpPK4 = "";

                DataSet Ds = new DataSet();
                cmdSearch.Connection = ConTrans;
                cmdSearch.CommandType = CommandType.Text;
                cmdSearch.CommandText = "Select TblFind_CustomFieldNameFilter from TblFind where TblFind_TblName = " + "'" + ControlMgr.TblName + "'" + " and TblFind_PrimaryKey = 'PK' order by TblFind_NoUrut";

                DaSearch.SelectCommand = cmdSearch;
                DaSearch.Fill(Ds, "tblPK");

                if (Ds.Tables["tblPK"].Rows.Count == 1)
                {
                    tmpPK = Ds.Tables["TblPK"].Rows[0][0].ToString();
                    ControlMgr.Kode = DataGridView1.Rows[e.RowIndex].Cells[tmpPK].Value.ToString();
                }
                else if (Ds.Tables["TblPK"].Rows.Count == 2)
                {
                    tmpPK = Ds.Tables["TblPK"].Rows[0][0].ToString();
                    ControlMgr.Kode = DataGridView1.Rows[e.RowIndex].Cells[tmpPK].Value.ToString();
                    tmpPK2 = Ds.Tables["TblPK"].Rows[1][0].ToString();
                    ControlMgr.Kode2 = DataGridView1.Rows[e.RowIndex].Cells[tmpPK2].Value.ToString();
                }
                else if (Ds.Tables["TblPK"].Rows.Count == 3)
                {
                    tmpPK = Ds.Tables["TblPK"].Rows[0][0].ToString();
                    ControlMgr.Kode = DataGridView1.Rows[e.RowIndex].Cells[tmpPK].Value.ToString();
                    tmpPK2 = Ds.Tables["TblPK"].Rows[1][0].ToString();
                    ControlMgr.Kode2 = DataGridView1.Rows[e.RowIndex].Cells[tmpPK2].Value.ToString();
                    tmpPK3 = Ds.Tables["TblPK"].Rows[2][0].ToString();
                    ControlMgr.Kode3 = DataGridView1.Rows[e.RowIndex].Cells[tmpPK3].Value.ToString();
                }
                else if (Ds.Tables["TblPK"].Rows.Count == 4)
                {
                    tmpPK = Ds.Tables["TblPK"].Rows[0][0].ToString();
                    ControlMgr.Kode = DataGridView1.Rows[e.RowIndex].Cells[tmpPK].Value.ToString();
                    tmpPK2 = Ds.Tables["TblPK"].Rows[1][0].ToString();
                    ControlMgr.Kode2 = DataGridView1.Rows[e.RowIndex].Cells[tmpPK2].Value.ToString();
                    tmpPK3 = Ds.Tables["TblPK"].Rows[2][0].ToString();
                    ControlMgr.Kode3 = DataGridView1.Rows[e.RowIndex].Cells[tmpPK3].Value.ToString();
                    tmpPK4 = Ds.Tables["TblPK"].Rows[3][0].ToString();
                    ControlMgr.Kode4 = DataGridView1.Rows[e.RowIndex].Cells[tmpPK4].Value.ToString();
                }
                this.Close();
            }
        }
    }
}
