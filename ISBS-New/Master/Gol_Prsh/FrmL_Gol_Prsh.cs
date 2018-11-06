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

namespace ISBS_New.Master.Gol_Prsh
{
    public partial class FrmL_Gol_Prsh : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        DataSet Ds = new DataSet();
        SqlCommand cmdSearch = new SqlCommand();
        SqlCommand cmdGol_Prsh = new SqlCommand();
        SqlCommand cmdSearchGrid = new SqlCommand();
        SqlDataAdapter DaSearch = new SqlDataAdapter();
        SqlDataReader Dr;
        string tblName = "";
        string strSql = "";

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FrmL_Gol_Prsh()
        {
            InitializeComponent();
        }

        private void FrmL_Gol_Prsh_Load(object sender, EventArgs e)
        {
            ModeLoad();
        }

        public void ModeLoad()
        {
            tblName = "Gol_Prsh";
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
        private void FrmL_Gol_Prsh_FormClosed(object sender, FormClosedEventArgs e)
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

            //if (txtSearch.Text == "")
            //{
            //    MessageBox.Show("Please fill in Criteria first");
            //    return;
            //}

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
            DtGridView_Gol_Prsh.DataSource = Dv;
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
                    DtGridView_Gol_Prsh.Columns[i].Width = Convert.ToInt32(Dr["tblFind_Width"]);
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
            //Form frmM_Gol_Prsh = new Master.Gol_Prsh.FrmM_Gol_Prsh(true, "");
            FrmM_Gol_Prsh F = new FrmM_Gol_Prsh(true, "");
            F.setParent(this);
            F.Show();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (DtGridView_Gol_Prsh.RowCount >= 0)
            {
                if (!CekGolongan())
                {
                    MessageBox.Show("Golongan " + DtGridView_Gol_Prsh.CurrentRow.Cells["tblGol_Prsh_Gol_Prsh"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                var _kode = DtGridView_Gol_Prsh.CurrentRow.Cells["Gol Prsh"].Value.ToString();
                //Form frmM_Gol_Prsh = new Master.Gol_Prsh.FrmM_Gol_Prsh(false, _kode);
                FrmM_Gol_Prsh F = new Master.Gol_Prsh.FrmM_Gol_Prsh(false, _kode);
                F.setParent(this);
                F.Show();
            }
        }

        private Boolean CekGolongan()
        {
            Boolean vBol = true;
            string Gol_Prsh = DtGridView_Gol_Prsh.CurrentRow.Cells["tblGol_Prsh_Gol_Prsh"].Value.ToString();

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh=@Gol_Prsh";
                using (SqlCommand cmdtblGol_Prsh = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", Gol_Prsh);
                    Dr = cmdtblGol_Prsh.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        vBol = true;
                    }
                    else
                    {
                        vBol = false;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }

            return vBol;
        }

        private Boolean Used()
        {
            Boolean vBol = true;
            string Gol_Prsh = DtGridView_Gol_Prsh.CurrentRow.Cells["tblGol_Prsh_Gol_Prsh"].Value.ToString();
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblCustomer WHERE tblCustomer_Gol_Prsh=@Gol_Prsh";
                using (SqlCommand cmdtblGol_Prsh = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", Gol_Prsh);
                    Dr = cmdtblGol_Prsh.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        vBol = true;
                    }
                    else { vBol = false; }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }
            return vBol;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (DtGridView_Gol_Prsh.RowCount >= 0)
            {
                if (!CekGolongan())
                {
                    MessageBox.Show("Golongan " + DtGridView_Gol_Prsh.CurrentRow.Cells["Gol Prsh"].Value.ToString() + " tidak ditemukan..");
                    return;
                }
                if (Used())
                {
                    MessageBox.Show("Golongan " + DtGridView_Gol_Prsh.CurrentRow.Cells["Gol Prsh"].Value.ToString() + " sudah pernah digunakan..");
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Golongan " + DtGridView_Gol_Prsh.CurrentRow.Cells["Gol Prsh"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                                strSql = "DELETE FROM Gol_Prsh WHERE Gol_Prsh=@golper";
                                using (cmdGol_Prsh = new SqlCommand(strSql, ConnMaster))
                                {
                                    cmdGol_Prsh.Parameters.AddWithValue("@golper", DtGridView_Gol_Prsh.CurrentRow.Cells["Gol Prsh"].Value.ToString());
                                    cmdGol_Prsh.ExecuteNonQuery();
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
                        //BtnReset.PerformClick();
                        ResetGrid();
                    }
                }
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
            string tblName = "Gol_Prsh";
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
            //BtnSearch.PerformClick();
            RefreshGrid();
        }

        private void DtGridView_Gol_Prsh_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (!CekGolongan())
                {
                    MessageBox.Show("Golongan " + DtGridView_Gol_Prsh.CurrentRow.Cells["tblGol_Prsh_Gol_Prsh"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                var _kode = DtGridView_Gol_Prsh.Rows[e.RowIndex].Cells["Gol Prsh"].Value.ToString();
                //Form frmM_Gol_Prsh = new Master.Gol_Prsh.FrmM_Gol_Prsh(false, _kode);
                FrmM_Gol_Prsh F = new Master.Gol_Prsh.FrmM_Gol_Prsh(false, _kode);
                F.setParent(this);
                F.Show();
            }
        }
    }
}
