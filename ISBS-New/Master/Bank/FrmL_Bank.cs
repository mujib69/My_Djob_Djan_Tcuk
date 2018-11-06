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

namespace ISBS_New.Master.Bank
{
    public partial class FrmL_Bank : MetroFramework.Forms.MetroForm
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

        public FrmL_Bank()
        {
            InitializeComponent();
        }

        private void FrmL_Bank_Load(object sender, EventArgs e)
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

            tblName = "VIEW_tblBank";
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

        private void FrmL_Bank_FormClosed(object sender, FormClosedEventArgs e)
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
            DtGridView_Bank.DataSource = Dv;
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
                    DtGridView_Bank.Columns[i].Width = Convert.ToInt32(Dr["tblFind_Width"]);
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

            Form FrmM_Bank = new Master.Bank.FrmM_Bank("New", "");
            FrmM_Bank.Show();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean CekBank()
        {
            Boolean vBol = true;
            string Kode_Bank = DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString();

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblBank WHERE tblBank_Kode_Bank=@Kode_Bank";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Kode_Bank", Kode_Bank);
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

            if (DtGridView_Bank.RowCount >= 0)
            {
                if (!CekBank())
                {
                    MessageBox.Show("Kode Bank " + DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                string Kode_Bank = DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString();
                Form FrmM_Bank = new Master.Bank.FrmM_Bank("Edit", Kode_Bank);
                FrmM_Bank.Show();
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

            if (DtGridView_Bank.RowCount >= 0)
            {
                if (!CekBank())
                {
                    MessageBox.Show("Kode Bank " + DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kode Bank " + DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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

                                strSql = "DELETE FROM tblContact WHERE tblContact_tblName='tblBank' AND tblContact_Kode='" + DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString() + "'";
                                using (SqlCommand cmdtblBank_Kode_Bank = new SqlCommand(strSql, ConnMaster))
                                {
                                    cmdtblBank_Kode_Bank.ExecuteNonQuery();
                                }

                                strSql = "DELETE FROM tblBank WHERE tblBank_Kode_Bank='" + DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString() + "'";
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
            string tblName = "VIEW_tblBank";
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

        private void DtGridView_Bank_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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

            if (DtGridView_Bank.RowCount >= 0)
            {
                if (!CekBank())
                {
                    MessageBox.Show("Kode Bank " + DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString() + " tidak ditemukan..");
                    return;
                }
                string Kode_Bank = DtGridView_Bank.CurrentRow.Cells["Kode Bank"].Value.ToString();
                Form FrmM_Bank = new Master.Bank.FrmM_Bank("Edit", Kode_Bank);
                FrmM_Bank.Show();
            }
        }

        private void FrmL_Bank_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            }    
        }

    }
}
