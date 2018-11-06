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


namespace ISBS_New.Master.Golongan
{
    public partial class FrmL_GolPrsh : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        DataSet Ds = new DataSet();
        SqlCommand cmdSearch = new SqlCommand();
        SqlCommand Cmd = new SqlCommand();
        SqlCommand cmdSearchGrid = new SqlCommand();
        SqlDataAdapter DaSearch = new SqlDataAdapter();
        SqlDataReader Dr;
        string tblName = "";
        string strSql = "";

        Boolean vView, vNew, vEdit, vDelete;

        public FrmL_GolPrsh()
        {
            InitializeComponent();
        }

        private void FrmL_GolPrsh_Load(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator")
            {
                ControlMgr.CheckAccessRight(ref vView, ref vNew, ref vEdit, ref vDelete, this.Name);
                if (vView == false)
                {
                    ControlMgr.DisplayMsgAccessRight("View", this.Text);
                    return;
                }
            }

            tblName = "tblGol_Prsh";
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

        private void FrmL_GolPrsh_FormClosed(object sender, FormClosedEventArgs e)
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
            DtGridView_GolPrsh.DataSource = Dv;
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
                    DtGridView_GolPrsh.Columns[i].Width = Convert.ToInt32(Dr["tblFind_Width"]);
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
            if (ControlMgr.GroupName != "Administrator")
            {
                ControlMgr.CheckAccessRight(ref vView, ref vNew, ref vEdit, ref vDelete, this.Name);
                if (vNew == false)
                {
                    ControlMgr.DisplayMsgAccessRight("New", this.Text);
                    return;
                }
            }

            Form FrmM_GolPrsh = new Master.Golongan.FrmM_GolPrsh("New", "");
            FrmM_GolPrsh.Show();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean CekGolongan()
        {
            Boolean vBol = true;
            string Gol_Prsh = DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString();

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh=@Gol_Prsh";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Gol_Prsh", Gol_Prsh);
                    Dr = Cmd.ExecuteReader();
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

            if (DtGridView_GolPrsh.RowCount >= 0)
            {
                if (!CekGolongan())
                {
                    MessageBox.Show("Golongan " + DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                string Gol_Prsh = DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString();
                Form FrmM_GolPrsh = new Master.Golongan.FrmM_GolPrsh("Edit", Gol_Prsh);
                FrmM_GolPrsh.Show();
            }
        }

        private Boolean Used()
        {
            Boolean vBol = true;
            string Gol_Prsh = DtGridView_GolPrsh.CurrentRow.Cells["tblGol_Prsh_Gol_Prsh"].Value.ToString();
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM CustTable WHERE Gol_Prsh=@Gol_Prsh";
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

        private Boolean CekBank_Group()
        {
            Boolean vBol = true;
            string Gol_Prsh = DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString();

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh=@Gol_Prsh";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Gol_Prsh", Gol_Prsh);
                    Dr = Cmd.ExecuteReader();
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

            if (DtGridView_GolPrsh.RowCount >= 0)
            {
                if (!CekBank_Group())
                {
                    MessageBox.Show("Golongan " + DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                if (Used())
                {
                    MessageBox.Show("Golongan " + DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString() + " sudah pernah digunakan..");
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Golongan " + DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                                strSql = "DELETE FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh='" + DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString() + "'";
                                using (Cmd = new SqlCommand(strSql, ConnMaster))
                                {
                                    Cmd.ExecuteNonQuery();
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
        }

        public void ResetGrid()
        {
            Ds.Tables.Remove("Result");
            CboCriteria.Items.Clear();
            string tblName = "tblGol_Prsh";
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

        private void DtGridView_GolPrsh_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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

            if (DtGridView_GolPrsh.RowCount >= 0)
            {
                if (!CekGolongan())
                {
                    MessageBox.Show("Golongan " + DtGridView_GolPrsh.CurrentRow.Cells["Group"].Value.ToString() + " tidak ditemukan..");
                    return;
                }
                string Gol_Prsh = DtGridView_GolPrsh.CurrentRow.Cells["Golongan"].Value.ToString();
                Form FrmM_GolPrsh = new Master.Golongan.FrmM_GolPrsh("Edit", Gol_Prsh);
                FrmM_GolPrsh.Show();
            }  
        }

        private void FrmL_GolPrsh_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            } 
        }
    }
}
