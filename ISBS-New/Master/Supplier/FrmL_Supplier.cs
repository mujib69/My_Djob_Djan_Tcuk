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

namespace ISBS_New.Master.Supplier
{
    public partial class FrmL_Supplier : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        DataSet Ds = new DataSet();
        SqlCommand cmdSearch = new SqlCommand();
        SqlCommand cmdSupplier = new SqlCommand();
        SqlCommand cmdHistory = new SqlCommand();
        SqlCommand cmdSearchGrid = new SqlCommand();
        SqlDataAdapter DaSearch = new SqlDataAdapter();
        SqlDataReader Dr;
        string tblName = "";


        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FrmL_Supplier()
        {
            InitializeComponent();
        }

        private void FrmL_Supplier_Load(object sender, EventArgs e)
        {
            tblName = "tblSupplier";
            GC.Collect();
            string tmpFieldName = "";

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                cmdSearch.Connection = ConnMaster;
                cmdSearch.CommandType = CommandType.Text;
                cmdSearch.CommandText = "Select TblFind_FieldNameFilter from TblFind where TblFind_TblName = " + "'" + tblName + "'" + " order by TblFind_NoUrut";
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
                strSql = "Select " + tmpFieldName + " from " + tblName;

                cmdSearchGrid.CommandText = strSql;
                cmdSearchGrid.CommandType = CommandType.Text;
                cmdSearchGrid.Connection = ConnMaster;
                DaSearch.SelectCommand = cmdSearchGrid;
                DaSearch.Fill(Ds, "Result");
                cmdSearch.CommandText = "Select TblFind_CustomFieldNameFilter from TblFind where TblFind_TblName = " + "'" + tblName + "'" + " order by TblFind_NoUrut";
                Dr = cmdSearch.ExecuteReader();

                try
                {
                    int i = 0;
                    while (Dr.Read())
                    {
                        CboCriteria.Items.Add(Dr[0]);

                        if (i == 0)
                        {
                            CboCriteria.Text = Dr[0].ToString();
                        }
                        Ds.Tables["Result"].Columns[i].ColumnName = Dr[0].ToString();
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

                CboCriteria.Items.Add("*");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                ConnMaster.Close();
            }
            CboCriteria.DropDownStyle = ComboBoxStyle.DropDownList;

            txtSearch.Text = "*";
            BtnSearch.PerformClick();
        }

        private void FrmL_Supplier_FormClosed(object sender, FormClosedEventArgs e)
        {
            Ds.Clear();
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
            DtGridView_Supplier.DataSource = Dv;
            DefineWidthofDataGridView();   
        }

        private void DefineWidthofDataGridView()
        {
            ConnMaster = ConnectionString.GetConnection();
            cmdSearch.Connection = ConnMaster;
            cmdSearch.CommandText = "Select TblFind_NoUrut,TblFind_Width from TblFind where TblFind_TblName = " + "'" + tblName + "'" + " order by TblFind_NoUrut";
            Dr = cmdSearch.ExecuteReader();
            int i = 0;

            try
            {
                while (Dr.Read())
                {

                    DtGridView_Supplier.Columns[i].Width = Convert.ToInt32(Dr["tblFind_Width"]);
                    //if (Convert.ToInt32(Dr["tblFind_NoUrut"]) >= 20 && Convert.ToInt32(Dr["tblFind_NoUrut"]) < 30)
                    //{
                    //    DtGridview_Supplier.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    //    DtGridview_Supplier.Columns[i].DefaultCellStyle.Format="#,##0.00";
                    //} 
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
                ConnMaster.Close();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                BtnSearch.PerformClick();
            }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
                        if (this.PermissionAccess(Login.New) > 0)
            {

            Form frmM_Supplier = new Master.Supplier.FrmM_Supplier(true, "");
            frmM_Supplier.Show();
            }
                        else
                        {
                            MessageBox.Show(Login.PermissionDenied);
                        }
        
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
             if (PermissionAccess(Login.View) > 0)
            {
            var _kode = DtGridView_Supplier.CurrentRow.Cells["Kode Perusahaan"].Value.ToString();
            Form frmM_Supplier = new Master.Supplier.FrmM_Supplier(false, _kode);
            frmM_Supplier.Show();
            }
             else
             {
                 MessageBox.Show(Login.PermissionDenied);
             }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
             if (this.PermissionAccess(Login.Delete) > 0)
            {
           
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kode Supplier " + DtGridView_Supplier.CurrentRow.Cells["Kode Perusahaan"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            string strSql;
                            strSql = "DELETE FROM tblSupplier WHERE tblSupplier_Kode_Prsh='" + DtGridView_Supplier.CurrentRow.Cells["Kode Perusahaan"].Value.ToString() + "'";
                            using (cmdSupplier = new SqlCommand(strSql, ConnMaster))
                            {
                                cmdSupplier.ExecuteNonQuery();
                            }

                            string Sql = "INSERT INTO TblStatusLog_Supplier (TblStatusLog_FormName,tblStatusLog_PK1) VALUES(";
                            Sql += "'FrmM_Supplier','" + DtGridView_Supplier.CurrentRow.Cells["Kode Perusahaan"].Value.ToString() + "')";
                            using (cmdHistory = new SqlCommand(Sql, ConnMaster))
                            {
                                cmdHistory.ExecuteNonQuery();
                            }
                        }
                        scope.Complete();
                        MessageBox.Show("Berhasil Delete data..");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    ConnMaster.Close();
                    BtnReset.PerformClick();
                }
            }
            }
             else
             {
                 MessageBox.Show(Login.PermissionDenied);
             }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            Ds.Tables.Remove("Result");
            CboCriteria.Items.Clear();
            string tblName = "tblSupplier";
            GC.Collect();
            string tmpFieldName = "";

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                cmdSearch.Connection = ConnMaster;
                cmdSearch.CommandType = CommandType.Text;
                cmdSearch.CommandText = "Select TblFind_FieldNameFilter from TblFind where TblFind_TblName = " + "'" + tblName + "'" + " order by TblFind_NoUrut";
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
                strSql = "Select " + tmpFieldName + " from " + tblName;

                cmdSearchGrid.CommandText = strSql;
                cmdSearchGrid.CommandType = CommandType.Text;
                cmdSearchGrid.Connection = ConnMaster;
                DaSearch.SelectCommand = cmdSearchGrid;
                DaSearch.Fill(Ds, "Result");
                cmdSearch.CommandText = "Select TblFind_CustomFieldNameFilter from TblFind where TblFind_TblName = " + "'" + tblName + "'" + " order by TblFind_NoUrut";
                Dr = cmdSearch.ExecuteReader();

                try
                {
                    int i = 0;
                    while (Dr.Read())
                    {
                        CboCriteria.Items.Add(Dr[0]);

                        if (i == 0)
                        {
                            CboCriteria.Text = Dr[0].ToString();
                        }
                        Ds.Tables["Result"].Columns[i].ColumnName = Dr[0].ToString();
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

                CboCriteria.Items.Add("*");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                ConnMaster.Close();
            }
            CboCriteria.DropDownStyle = ComboBoxStyle.DropDownList;
            BtnSearch.PerformClick();
        }

        private void DtGridView_Supplier_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var _kode = DtGridView_Supplier.Rows[e.RowIndex].Cells["Kode Perusahaan"].Value.ToString();
                Form frmM_Supplier = new Master.Supplier.FrmM_Supplier(false, _kode);
                frmM_Supplier.Show();
            }
        }


    }
}
