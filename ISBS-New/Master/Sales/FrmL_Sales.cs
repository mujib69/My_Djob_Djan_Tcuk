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

namespace ISBS_New.Master.Sales
{
    public partial class FrmL_Sales : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        SqlDataReader drtblSales;
        string strSql;
        DataTable dttblSales = new DataTable();

        Boolean vView, vNew, vEdit, vDelete;

        public FrmL_Sales()
        {
            InitializeComponent();
        }

        private void FrmL_Sales_Load(object sender, EventArgs e)
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

            IsiDtGridView_Sales();
        }

        private void IsiDtGridView_Sales()
        {
            dttblSales.Rows.Clear();
            try
            {
                strSql = "SELECT * FROM tblSales ORDER BY tblSales_UDate_Input DESC";
                ConnMaster = ConnectionString.GetConnection();
                using (SqlDataAdapter SqlDa = new SqlDataAdapter(strSql, ConnMaster))
                {
                    SqlDa.Fill(dttblSales);
                    DtGridView_Sales.DataSource = dttblSales;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); }
            finally { ConnMaster.Close(); }
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

            Form FrmM_Sales = new Master.Sales.FrmM_Sales("New", "");
            FrmM_Sales.Show();
        }

        private Boolean CekSales()
        {
            Boolean vBol = true;
            string Kode_Sales = DtGridView_Sales.CurrentRow.Cells["tblSales_Kode_Sls"].Value.ToString();

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblSales WHERE tblSales_Kode_Sls=@Kode_Sls";
                using (SqlCommand cmdtblSales = new SqlCommand(strSql, ConnMaster))
                {
                    cmdtblSales.Parameters.AddWithValue("@Kode_Sls", Kode_Sales);
                    drtblSales = cmdtblSales.ExecuteReader();
                    if (drtblSales.HasRows)
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
                drtblSales.Close();
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

            if (DtGridView_Sales.RowCount >= 0)
            {
                if (!CekSales())
                {
                    MessageBox.Show("Kode Sales " + DtGridView_Sales.CurrentRow.Cells["tblSales_Kode_Sls"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                string Kode_Sales = DtGridView_Sales.CurrentRow.Cells["tblSales_Kode_Sls"].Value.ToString();
                Form FrmM_Sales = new Master.Sales.FrmM_Sales("Edit", Kode_Sales);
                FrmM_Sales.Show();
            }
        }

        private Boolean Used()
        {
            Boolean vBol = false;
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblSales WHERE tblSales_Kode_Sls=@Kode_Sls";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Kode_Sls", DtGridView_Sales.CurrentRow.Cells["tblSales_Kode_Sls"].Value.ToString());
                    drtblSales = Cmd.ExecuteReader();
                    if (drtblSales.HasRows)
                    {
                        vBol = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                drtblSales.Close();
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

            if (DtGridView_Sales.RowCount >= 0)
            {
                if (!CekSales())
                {
                    MessageBox.Show("Kode Sales " + DtGridView_Sales.CurrentRow.Cells["tblSales_Kode_Sls"].Value.ToString() + " tidak ditemukan..");
                    return;
                }

                if (Used())
                {
                    MessageBox.Show("Kode Sales " + DtGridView_Sales.CurrentRow.Cells["tblSales_Kode_Sls"].Value.ToString() + " sudah pernah digunakan..");
                    return;
                }

                string Kode_Sales = DtGridView_Sales.CurrentRow.Cells["tblSales_Kode_Sls"].Value.ToString();
                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kode Sales " + Kode_Sales + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        using (TransactionScope Scope = new TransactionScope())
                        {
                            ConnMaster = ConnectionString.GetConnection();
                            using (ConnMaster)
                            {
                                strSql = "DELETE FROM tblSales WHERE tblSales_Kode_Sls=@Kode_Sls";
                                using (SqlCommand cmdtblSales = new SqlCommand(strSql, ConnMaster))
                                {
                                    cmdtblSales.Parameters.AddWithValue("@Kode_Sls", Kode_Sales);
                                    cmdtblSales.ExecuteNonQuery();
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
                    MessageBox.Show("Kode Sales " + Kode_Sales + ", berhasil dihapus..");
                    IsiDtGridView_Sales();
                }
            }
        }

        private void FrmL_Sales_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            } 
        }

    }
}
