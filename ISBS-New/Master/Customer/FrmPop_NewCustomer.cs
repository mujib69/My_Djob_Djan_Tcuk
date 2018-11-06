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


namespace ISBS_New.Master.Customer
{
    public partial class FrmPop_NewCustomer : MetroFramework.Forms.MetroForm
    {
        string strSql;
        SqlConnection ConMaster;
        SqlDataReader drGol_Prsh;
        SqlDataReader drCounter;
        SqlDataReader drCustTable;

        public FrmPop_NewCustomer()
        {
            InitializeComponent();
        }

        private void FrmPop_NewCustomer_Load(object sender, EventArgs e)
        {
            this.ActiveControl = txttblCustomer_Nama_Prsh;
        }

        private void btntxttblCustomer_Gol_Prsh_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "Gol_Prsh";
            ControlMgr.tmpSort = "ORDER BY Gol_Prsh";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Type Customer";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txttblCustomer_Gol_Prsh.Text = ControlMgr.Kode;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.Kode = "";
        }

        private void txttblCustomer_Gol_Prsh_KeyPress(object sender, KeyPressEventArgs e)
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
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txttblCustomer_Gol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (drGol_Prsh.HasRows)
                {
                    while (drGol_Prsh.Read())
                    {
                        txttblCustomer_Gol_Prsh.Text = Convert.IsDBNull(drGol_Prsh["Gol_Prsh"]) ? "" : (string)drGol_Prsh["Gol_Prsh"];
                    }
                }
                else
                {
                    MessageBox.Show("Type Perusahaan " + txttblCustomer_Gol_Prsh.Text + " doesn't exist");
                }
                drGol_Prsh.Close();
                ConMaster.Close();
            }
        }
      
        private void txttblCustomer_Gol_Prsh_Leave(object sender, EventArgs e)
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
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txttblCustomer_Gol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (!drGol_Prsh.HasRows)
                {
                    MessageBox.Show("Type Perusahaan " + txttblCustomer_Gol_Prsh.Text + " doesn't exist");
                    ControlMgr.TblName = "Gol_Prsh";
                    ControlMgr.tmpSort = "ORDER BY Gol_Prsh";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Type Customer";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        txttblCustomer_Gol_Prsh.Text = ControlMgr.Kode;
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
                txttblCustomer_Gol_Prsh.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txttblCustomer_Gol_Prsh.Text = "";
            txttblCustomer_Nama_Prsh.Text = "";
            this.ActiveControl = txttblCustomer_Nama_Prsh;
        }

        private void AmbilKodeCustomer()
        {
            string strSql;
            strSql = "SELECT TOP 1 'C'+RIGHT('00000' + CAST((RIGHT(CustId,5)+1) AS NVARCHAR(5)),5) AS CustId ";
            strSql += "FROM CustTable ORDER BY CustId DESC";
            using (SqlCommand cmdCounter = new SqlCommand(strSql, ConMaster))
            {
                drCounter = cmdCounter.ExecuteReader();
                if (drCounter.HasRows)
                {
                    while (drCounter.Read())
                    {
                        txttblCustomer_Kode_Prsh.Text = drCounter["CustId"].ToString();
                    }
                }
                else
                {
                    txttblCustomer_Kode_Prsh.Text = "C00001";
                }
                drCounter.Close();
            }
        }

        private Boolean CekNamaCustomer()
        {
            try
            {
                ConMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM CustTable WHERE CustName=@Nama";
                using (SqlCommand cmd = new SqlCommand(strSql,ConMaster))
                {
                    cmd.Parameters.AddWithValue("@Nama", txttblCustomer_Nama_Prsh.Text.Trim());
                    drCustTable = cmd.ExecuteReader();
                    if (drCustTable.HasRows)
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
                drCustTable.Close();
                ConMaster.Close();
            }
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txttblCustomer_Nama_Prsh.Text.Trim() == "")
            {
                MessageBox.Show("Nama Customer harus diisi..");
                vBol = false;
            }

            else if(CekNamaCustomer())
            {
                MessageBox.Show("Nama Customer sudah ada..");
                vBol = false;
            }

            else if (txttblCustomer_Gol_Prsh.Text.Trim() == "")
            {
                MessageBox.Show("Type Customer harus diisi..");
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
                        AmbilKodeCustomer();
                        strSql = "INSERT INTO CustTable(CustId,CustName,Gol_Prsh,CreatedBy,CreatedDate ";
                        strSql += ") VALUES(";
                        strSql += "@Kode_Prsh,@Nama_Prsh,@Gol_Prsh, ";
                        strSql += "@CreatedBy,@CreatedDate)";
                        using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                        {
                            cmdCustomer.Parameters.AddWithValue("@Kode_Prsh", txttblCustomer_Kode_Prsh.Text);
                            cmdCustomer.Parameters.AddWithValue("@Nama_Prsh", txttblCustomer_Nama_Prsh.Text);
                            cmdCustomer.Parameters.AddWithValue("@Gol_Prsh", txttblCustomer_Gol_Prsh.Text);
                            cmdCustomer.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                            cmdCustomer.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                            cmdCustomer.ExecuteNonQuery();
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

        private void btnSaveEdit_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                return;
            }

            if (SaveData())
            {
                //Form frmM_Customer = new Master.Customer.FrmM_Customer(txttblCustomer_Kode_Prsh.Text);
                //frmM_Customer.Show();
                this.Close();
            }
        }
    }
}
