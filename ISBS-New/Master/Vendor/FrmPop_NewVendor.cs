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

namespace ISBS_New.Master.Vendor
{
    public partial class FrmPop_NewVendor : MetroFramework.Forms.MetroForm
    {
        string strSql;
        SqlConnection ConMaster;
        SqlDataReader drGol_Prsh;
        SqlDataReader drCounter;
        SqlDataReader drVendTable;

        public FrmPop_NewVendor()
        {
            InitializeComponent();
        }

        private void FrmPop_NewVendor_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txtNama_Prsh;
        }

        private void btntxtGol_Prsh_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "Gol_Prsh";
            ControlMgr.tmpSort = "ORDER BY Gol_Prsh";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Type Vendor";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtGol_Prsh.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void txtGol_Prsh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Gol_Prsh();
            }
        }

        private void Ambil_Gol_Prsh()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Gol_Prsh WHERE Gol_Prsh=@Gol_Prsh";
            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
            {
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (drGol_Prsh.HasRows)
                {
                    while (drGol_Prsh.Read())
                    {
                        txtGol_Prsh.Text = Convert.IsDBNull(drGol_Prsh["Gol_Prsh"]) ? "" : (string)drGol_Prsh["Gol_Prsh"];
                    }
                }
                else
                {
                    MessageBox.Show("Type Perusahaan " + txtGol_Prsh.Text + " doesn't exist");
                }
                drGol_Prsh.Close();
                ConMaster.Close();
            }
        }

        private void txtGol_Prsh_Leave(object sender, EventArgs e)
        {
            Cek_Gol_Prsh();
        }

        private void Cek_Gol_Prsh()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM Gol_Prsh WHERE Gol_Prsh=@Gol_Prsh";
            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
            {
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (!drGol_Prsh.HasRows)
                {
                    MessageBox.Show("Type Perusahaan " + txtGol_Prsh.Text + " doesn't exist");
                    ControlMgr.TblName = "Gol_Prsh";
                    ControlMgr.tmpSort = "ORDER BY Gol_Prsh";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Type Customer";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txtGol_Prsh.Text = ControlMgr.Kode;
                        Ambil_Gol_Prsh();
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drGol_Prsh.Close();
                ConMaster.Close();
            }

            if (vBolFound)
            {
                Ambil_Gol_Prsh();
            }
            else
            {
                txtGol_Prsh.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtGol_Prsh.Text = "";
            txtNama_Prsh.Text = "";
            this.ActiveControl = txtNama_Prsh;
        }

        private void AmbilKodeVendor()
        {
            string strSql;
            strSql = "SELECT TOP 1 'V'+RIGHT('00000' + CAST((RIGHT(VendId,5)+1) AS NVARCHAR(5)),5) AS VendId ";
            strSql += "FROM VendTable ORDER BY VendId DESC";
            using (SqlCommand cmdCounter = new SqlCommand(strSql, ConMaster))
            {
                drCounter = cmdCounter.ExecuteReader();
                if (drCounter.HasRows)
                {
                    while (drCounter.Read())
                    {
                        txtKode_Prsh.Text = drCounter["VendId"].ToString();
                    }
                }
                else
                {
                    txtKode_Prsh.Text = "V00001";
                }
                drCounter.Close();
            }
        }

        private Boolean CekNamaVendor()
        {
            try
            {
                ConMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM VendTable WHERE VendName=@Nama";
                using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
                {
                    cmd.Parameters.AddWithValue("@Nama", txtNama_Prsh.Text.Trim());
                    drVendTable = cmd.ExecuteReader();
                    if (drVendTable.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return true;
            }
            finally
            {
                drVendTable.Close();
                ConMaster.Close();
            }
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txtNama_Prsh.Text.Trim() == "")
            {
                MessageBox.Show("Nama Vendor harus diisi..");
                vBol = false;
            }

            if (CekNamaVendor())
            {
                MessageBox.Show("Nama Vendor sudah ada..");
                vBol = false;
            }

            if (txtGol_Prsh.Text.Trim() == "")
            {
                MessageBox.Show("Type Vendor harus diisi..");
                vBol = false;
            }
            return vBol;
        }

        private Boolean SaveData()
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ConMaster = ConnectionString.GetConnection();
                    using (ConMaster)
                    {
                        AmbilKodeVendor();
                        strSql = "INSERT INTO VendTable(VendId,VendName,Gol_Prsh,CreatedBy,CreatedDate ";
                        strSql += ") VALUES(";
                        strSql += "@Kode_Prsh,@Nama_Prsh,@Gol_Prsh, ";
                        strSql += "@CreatedBy,@CreatedDate)";
                        using (SqlCommand cmdVendor = new SqlCommand(strSql, ConMaster))
                        {
                            cmdVendor.Parameters.AddWithValue("@Kode_Prsh", txtKode_Prsh.Text);
                            cmdVendor.Parameters.AddWithValue("@Nama_Prsh", txtNama_Prsh.Text);
                            cmdVendor.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text);
                            cmdVendor.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                            cmdVendor.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                            cmdVendor.ExecuteNonQuery();
                        }
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return false;
            }
            finally
            {
                ConMaster.Close();
            }
            return true;
        }

        private void btnSaveClose_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                return;
            }
            if (SaveData())
            {
                var FrmL_Vendor = Application.OpenForms.OfType<Master.Vendor.FrmL_Vendor>().FirstOrDefault();
                if (FrmL_Vendor != null)
                {
                    FrmL_Vendor.Activate();
                }
                else
                {
                    new Master.Vendor.FrmL_Vendor().Show();
                }
                this.Close();
            }
        }

        private void btnSaveEdit_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                return;
            }

            if (SaveData())
            {
                //Form frmM_Vendor = new Master.Vendor.FrmM_Vendor(txtKode_Prsh.Text);
                //frmM_Vendor.Show();
                this.Close();
            }
        }
    }
}
