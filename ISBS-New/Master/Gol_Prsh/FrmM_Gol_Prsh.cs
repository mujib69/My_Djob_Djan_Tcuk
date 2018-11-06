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

namespace ISBS_New.Master.Gol_Prsh
{
    public partial class FrmM_Gol_Prsh : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConMaster = new SqlConnection();
        SqlDataReader drGol_Prsh;
        string strSql, vOldtxtGol_Prsh;
        Boolean BolNew;

        FrmL_Gol_Prsh Parent;

        public FrmM_Gol_Prsh(bool _BolNew, string _kode)
        {
            InitializeComponent();
            BolNew = _BolNew;
            txtGol_Prsh.Text = _kode;
            MulaiDariAwal();
        }

        public void setParent(FrmL_Gol_Prsh F)
        {
            Parent = F;
        }

        private void FrmM_Gol_Prsh_Load(object sender, EventArgs e)
        {
            if (BolNew)
            {
                txt_ReadOnly(false);
                Btn_EditCancelSaveDel(false, true, true, false);
                this.ActiveControl = txtGol_Prsh;
            }
            else
            {
                try
                {
                    ConMaster = ConnectionString.GetConnection();
                    strSql = "SELECT * FROM Gol_Prsh ";
                    strSql += "WHERE Gol_Prsh='" + txtGol_Prsh.Text + "'";
                    using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
                    {
                        drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                        if (drGol_Prsh.HasRows)
                        {
                            while (drGol_Prsh.Read())
                            {
                                txtGol_Prsh.Text = Convert.IsDBNull(drGol_Prsh["Gol_Prsh"]) ? "" : (string)drGol_Prsh["Gol_Prsh"];
                            }
                            Btn_EditCancelSaveDel(true, false, false, true);
                        }
                        else
                        {
                            MessageBox.Show("Data Golongan " + txtGol_Prsh.Text + " tidak ditemukan...");
                            this.BeginInvoke(new MethodInvoker(this.Close));
                            return;
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    drGol_Prsh.Close();
                    ConMaster.Close();
                }
            }
        }

        private void MulaiDariAwal()
        {
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            Btn_EditCancelSaveDel(false, false, false, false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txtGol_Prsh.ReadOnly = vbol;
        }

        private void Btn_EditCancelSaveDel(bool vEdit, bool vCancel, bool vSave, bool vDel)
        {
            btnEdit.Enabled = vEdit;
            btnCancel.Enabled = vCancel;
            btnSave.Enabled = vSave;
            btnDelete.Enabled = vDel;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            this.ActiveControl = txtGol_Prsh;
            txt_ReadOnly(false);
            Btn_EditCancelSaveDel(false, true, true, false);

            vOldtxtGol_Prsh = txtGol_Prsh.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Btn_EditCancelSaveDel(true, false, false, true);

            txtGol_Prsh.Text = vOldtxtGol_Prsh;

            this.ActiveControl = btnEdit;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Golongan " + txtGol_Prsh.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        ConMaster = ConnectionString.GetConnection();
                        using (ConMaster)
                        {
                            string strSql;
                            strSql = "DELETE FROM Gol_Prsh WHERE Gol_Prsh=@golper";
                            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
                            {
                                cmdGol_Prsh.Parameters.AddWithValue("@golper", txtGol_Prsh.Text);
                                cmdGol_Prsh.ExecuteNonQuery();
                            }
                        }
                        scope.Complete();
                        MessageBox.Show("Berhasil Delete data..");


                        var FrmL_Gol_Prsh = Application.OpenForms.OfType<Master.Gol_Prsh.FrmL_Gol_Prsh>().FirstOrDefault();
                        if (FrmL_Gol_Prsh != null)
                        {
                            FrmL_Gol_Prsh.Activate();
                        }
                        else
                        {
                            new Master.Gol_Prsh.FrmL_Gol_Prsh().Show();
                        }
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    ConMaster.Close();


                    Form parentform = Application.OpenForms["FrmL_Gol_Prsh"];
                    if (parentform != null)
                    {
                        Parent.ResetGrid();
                    }
                }
            } 
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            try
            {
                if (txtGol_Prsh.Text == "")
                {
                    MessageBox.Show("Nama Golongan harus diisi..");
                    vBol = false;                    
                }

                else if (!BolNew)
                {
                    if (txtGol_Prsh.Text == vOldtxtGol_Prsh)
                        vBol = true;
                    else
                    {
                        strSql = "SELECT * FROM Gol_Prsh WHERE Gol_Prsh=@Gol_Prsh";
                        ConMaster = ConnectionString.GetConnection();
                        using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
                        {
                            cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text);
                            drGol_Prsh = cmdGol_Prsh.ExecuteReader();

                            if (drGol_Prsh.HasRows)
                            {
                                MessageBox.Show("Nama Golongan Sudah ada");
                                vBol = false;
                            }
                        }
                    }
                }
                else
                {
                    strSql = "SELECT * FROM Gol_Prsh WHERE Gol_Prsh=@Gol_Prsh";
                    ConMaster = ConnectionString.GetConnection();
                    using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
                    {
                        cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text);
                        drGol_Prsh = cmdGol_Prsh.ExecuteReader();

                        if (drGol_Prsh.HasRows)
                        {
                            MessageBox.Show("Nama Golongan Sudah ada");
                            vBol = false;
                        }
                    }
                }
                drGol_Prsh.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {                
                ConMaster.Close();
            }

            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidGeneral() == false)
            {
                return;
            }
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ConMaster = ConnectionString.GetConnection();
                    using (ConMaster)
                    {
                        string strSql;
                        if (BolNew)
                        {
                            strSql = "INSERT INTO Gol_Prsh(Gol_Prsh,CreatedBy,CreatedDate ";
                            strSql += ") VALUES(";
                            strSql += "@Gol_Prsh,@Uinput,@UdateInput)";
                            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
                            {
                                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text.ToUpper());
                                cmdGol_Prsh.Parameters.AddWithValue("@Uinput", ControlMgr.UserId);
                                cmdGol_Prsh.Parameters.AddWithValue("@UdateInput", DateTime.Now);
                                cmdGol_Prsh.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            strSql = "UPDATE Gol_Prsh SET Gol_Prsh=@Gol_Prsh, ";
                            strSql += "UpdatedBy=@UEdit,UpdatedDate=@UDateEdit ";
                            strSql += "WHERE Gol_Prsh='" + vOldtxtGol_Prsh + "'";
                            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
                            {
                                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txtGol_Prsh.Text.ToUpper());
                                cmdGol_Prsh.Parameters.AddWithValue("@UEdit", ControlMgr.UserId);
                                cmdGol_Prsh.Parameters.AddWithValue("@UDateEdit", DateTime.Now);
                                cmdGol_Prsh.ExecuteNonQuery();
                            }
                        }
                    }
                    scope.Complete();
                    if (BolNew)
                    {
                        MessageBox.Show("Berhasil Insert data..");
                        BolNew = false;
                    }
                    else
                    {
                        MessageBox.Show("Berhasil Update data..");
                    }
                    txtGol_Prsh.Text = txtGol_Prsh.Text.ToUpper();
                    txt_ReadOnly(true);
                    Btn_EditCancelSaveDel(true, false, false, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                ConMaster.Close();

                Form parentform = Application.OpenForms["FrmL_Gol_Prsh"];
                if (parentform != null)
                {
                    Parent.ResetGrid();
                }
            }
        }





    }
}
