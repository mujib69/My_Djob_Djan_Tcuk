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

namespace ISBS_New.Master.Kota
{
    public partial class FrmL_Kota : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        DataSet Ds = new DataSet();
        SqlCommand cmdSearch = new SqlCommand();
        SqlCommand cmdKota = new SqlCommand();
        SqlCommand cmdSearchGrid = new SqlCommand();
        SqlDataAdapter DaSearch = new SqlDataAdapter();
        SqlDataReader Dr;
        string tblName = "";
        string strSql = "";

        Boolean vView, vNew, vEdit, vDelete;

        public FrmL_Kota()
        {
            InitializeComponent();
        }

        private void FrmL_Kota_Load(object sender, EventArgs e)
        {
            tblName = "tblKota";
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

        private void FrmL_Kota_FormClosed(object sender, FormClosedEventArgs e)
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

            if (txtSearch.Text == "")
            {
                MessageBox.Show("Please fill in Criteria first");
                return;
            }

            DataView Dv = new DataView();
            Dv.Table = Ds.Tables["Result"];

            if (CboCriteria.Text != "*")
            {
                strSql = "[" + CboCriteria.Text + "]" + " Like '%" + txtSearch.Text.Trim().Replace("'", "''") + "%'";
            }
            else
            {
                foreach (DataColumn col in Dv.Table.Columns)
                {
                    if (strSql == "")
                    {
                        strSql = "[" + col.ColumnName + "]" + " Like '%" + txtSearch.Text.Trim().Replace("'", "''") + "%'";
                    }
                    else
                    {
                        strSql += " OR " + "[" + col.ColumnName + "]" + " Like '%" + txtSearch.Text.Trim().Replace("'", "''") + "%'";
                    }
                }
            }
            Dv.RowFilter = strSql;
            DtGridView_Kota.DataSource = Dv;
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
                    DtGridView_Kota.Columns[i].Width = Convert.ToInt32(Dr["tblFind_Width"]);                    
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
            Form FrmM_Kota = new Master.Kota.FrmM_Kota("New", 0);
            FrmM_Kota.Show();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean CekKota()
        {
            Boolean vBol = true;
            string Kota_Kabupaten = DtGridView_Kota.CurrentRow.Cells["Kota/Kabupaten"].Value.ToString();
            string Nama_Kota_Kabupaten = DtGridView_Kota.CurrentRow.Cells["Nama Kota/Kabupaten"].Value.ToString();

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblKota WHERE tblKota_Kota_Kabupaten=@Kota_Kabupaten";
                strSql += " AND tblKota_Nama_Kota_Kabupaten=@Nama_Kota_Kabupaten";
                using (SqlCommand cmdtblKota = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblKota.Parameters.AddWithValue("@Kota_Kabupaten", Kota_Kabupaten);
                    cmdtblKota.Parameters.AddWithValue("@Nama_Kota_Kabupaten", Nama_Kota_Kabupaten);
                    Dr = cmdtblKota.ExecuteReader();
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

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator")
            {
                ControlMgr.CheckAccessRight(ref vView, ref vNew, ref vEdit, ref vDelete, this.Name);
                if (vEdit == false)
                {
                    ControlMgr.DisplayMsgAccessRight("Edit", this.Text);
                    return;
                }
            }

            if (DtGridView_Kota.RowCount >= 0)
            {
                if (!CekKota())
                {
                    MessageBox.Show("Nama Kota " + DtGridView_Kota.CurrentRow.Cells["Nama Kota/Kabupaten"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                int RecId = (int)DtGridView_Kota.CurrentRow.Cells["Rec Id"].Value;
                Form FrmM_Kota = new Master.Kota.FrmM_Kota("Edit", RecId);
                FrmM_Kota.Show();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator")
            {
                ControlMgr.CheckAccessRight(ref vView, ref vNew, ref vEdit, ref vDelete, this.Name);
                if (vDelete == false)
                {
                    ControlMgr.DisplayMsgAccessRight("Delete", this.Text);
                    return;
                }
            }

            if (DtGridView_Kota.RowCount >= 0)
            {
                if (!CekKota())
                {
                    MessageBox.Show("Nama Kota " + DtGridView_Kota.CurrentRow.Cells["tblKota_Nama_Kota_Kabupaten"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                string Nama_Kota_Kabupaten = DtGridView_Kota.CurrentRow.Cells["tblKota_Nama_Kota_Kabupaten"].Value.ToString();
                string Kota_Kabupaten = DtGridView_Kota.CurrentRow.Cells["tblKota_Kota_Kabupaten"].Value.ToString();
                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Nama Kota " + Nama_Kota_Kabupaten + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        using (TransactionScope Scope = new TransactionScope())
                        {
                            ConnMaster = ConnectionString.GetConnection();
                            using (ConnMaster)
                            {
                                strSql = "DELETE FROM tblKota WHERE tblKota_Kota_Kabupaten=@Kota_Kabupaten";
                                strSql += " AND tblKota_Nama_Kota_Kabupaten=@Nama_Kota_Kabupaten";
                                using (SqlCommand cmdtblKota = new SqlCommand(strSql, ConnMaster))
                                {
                                    cmdtblKota.Parameters.AddWithValue("@Kota_Kabupaten", Kota_Kabupaten);
                                    cmdtblKota.Parameters.AddWithValue("@Nama_Kota_Kabupaten", Nama_Kota_Kabupaten);
                                    cmdtblKota.ExecuteNonQuery();
                                }
                            }
                            Scope.Complete();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                        return;
                    }
                    finally
                    {
                        ConnMaster.Close();
                    }
                    MessageBox.Show("Nama Kota " + Nama_Kota_Kabupaten + ", berhasil dihapus..");
                    RefreshGrid();
                }
            }
        }

        public void ResetGrid()
        {
            Ds.Tables.Remove("Result");
            CboCriteria.Items.Clear();
            string tblName = "tblKota";
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

            //BtnSearch.PerformClick();
            txtSearch.Text = "*";
            RefreshGrid();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetGrid();
        }

        private void DtGridView_Kota_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator")
            {
                ControlMgr.CheckAccessRight(ref vView, ref vNew, ref vEdit, ref vDelete, this.Name);
                if (vEdit == false)
                {
                    ControlMgr.DisplayMsgAccessRight("Edit", this.Text);
                    return;
                }
            }

            if (DtGridView_Kota.RowCount >= 0)
            {
                if (!CekKota())
                {
                    MessageBox.Show("Nama Kota " + DtGridView_Kota.CurrentRow.Cells["Nama Kota/Kabupaten"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                int RecId =Convert.ToInt32(DtGridView_Kota.CurrentRow.Cells["Rec Id"].Value);
                Form FrmM_Kota = new Master.Kota.FrmM_Kota("View", RecId);
                FrmM_Kota.Show();
            }
        }

        private void FrmL_Kota_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            }   
        }
    }
}