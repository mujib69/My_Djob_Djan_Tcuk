using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Transactions;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.Master.Bentuk
{
    public partial class FrmL_Bentuk : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        DataSet Ds = new DataSet();
        SqlCommand cmdSearch = new SqlCommand();
        SqlCommand cmdBentuk = new SqlCommand();
        SqlCommand cmdSearchGrid = new SqlCommand();
        SqlDataAdapter DaSearch = new SqlDataAdapter();
        SqlDataReader Dr;
        string tblName = "";

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FrmL_Bentuk()
        {
            InitializeComponent();
        }

        private void FrmL_Bentuk_Load(object sender, EventArgs e)
        {
            ModeLoad();
        }

        public void ModeLoad()
        {
            tblName = "InventBentuk";
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

        private void FrmL_Bentuk_FormClosed(object sender, FormClosedEventArgs e)
        {
            Ds.Clear();
            GC.Collect();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            string strSql = "";
            DataView Dv = new DataView();
            Dv.Table = Ds.Tables["Result"];

            if (CboCriteria.Text != "*")
            {
                strSql = "[" + CboCriteria.Text + "]" + " Like '%" + txtSearch.Text.Replace("'", "''") + "%'";

            }
            else
            {
                foreach (DataColumn col in Dv.Table.Columns)
                {
                    if (strSql == "")
                    {
                        strSql = "[" + col.ColumnName + "]" + " Like '%" + txtSearch.Text.Replace("'", "''") + "%'";
                    }
                    else
                    {
                        strSql += " OR " + "[" + col.ColumnName + "]" + " Like '%" + txtSearch.Text.Replace("'", "''") + "%'";
                    }
                }
            }
            Dv.RowFilter = strSql;
            DtGridView_Bentuk.DataSource = Dv;
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
                    DtGridView_Bentuk.Columns[i].Width = Convert.ToInt32(Dr["tblFind_Width"]);
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
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                Bentuk.FrmM_Bentuk F = new Bentuk.FrmM_Bentuk(true, "");
                F.setParent(this);
                F.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (DtGridView_Bentuk.RowCount > 0)
            {
                var _kode = DtGridView_Bentuk.CurrentRow.Cells["Bentuk"].Value.ToString();
                Bentuk.FrmM_Bentuk F = new Bentuk.FrmM_Bentuk(false, _kode);
                F.setParent(this);
                F.Show();
            }
            else
            {
                MessageBox.Show("Tidak ada data..");
            }
        }

        private bool cekValidasi(string Bentuk)
        {
            Boolean vBol = true;
            string ErrMsg = "";
            string strSql;

            if (vBol == true)
            {
                try
                {
                    strSql = "Select * From InventBentuk Where Bentuk='" + Bentuk + "'";
                    ConnMaster = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(strSql, ConnMaster))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Bentuk tidak ditemukan..";
                            vBol = false;
                            ResetGrid();
                        }
                        Dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    ConnMaster.Close();
                }
            }

            if (vBol == true)
            {
                try
                {
                    strSql = "Select * From InventTable Where Bentuk='" + Bentuk + "'";
                    ConnMaster = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(strSql, ConnMaster))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Bentuk sudah pernah digunakan..";
                            vBol = false;
                        }
                        else
                        {
                            vBol = true;
                        }
                        Dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    ConnMaster.Close();
                }
            }

            if (vBol == false) { MessageBox.Show(ErrMsg); }
            return vBol;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                if (DtGridView_Bentuk.RowCount > 0)
                {
                    var Bentuk = DtGridView_Bentuk.CurrentRow.Cells["Bentuk"].Value.ToString();
                    if (cekValidasi(Bentuk) == false)
                    {
                        return;
                    }

                    DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Bentuk " + Bentuk + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                                    strSql = "DELETE FROM InventBentuk WHERE Bentuk=@Bentuk";
                                    using (cmdBentuk = new SqlCommand(strSql, ConnMaster))
                                    {
                                        cmdBentuk.Parameters.AddWithValue("@Bentuk", DtGridView_Bentuk.CurrentRow.Cells["Bentuk"].Value.ToString());
                                        cmdBentuk.ExecuteNonQuery();
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
                            ResetGrid();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetGrid();
        }

        public void ResetGrid()
        {
            Ds.Tables.Remove("Result");
            CboCriteria.Items.Clear();
            string tblName = "InventBentuk";
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
            RefreshGrid();
        }

        private void DtGridView_Bentuk_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var _kode = DtGridView_Bentuk.Rows[e.RowIndex].Cells["Bentuk"].Value.ToString();
                Bentuk.FrmM_Bentuk F = new Bentuk.FrmM_Bentuk(false, _kode);
                F.setParent(this);
                F.Show();
            }
        }

    }
}
