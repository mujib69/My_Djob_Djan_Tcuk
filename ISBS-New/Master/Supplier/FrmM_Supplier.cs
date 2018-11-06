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
    public partial class FrmM_Supplier : MetroFramework.Forms.MetroForm
    {
        Boolean BolNew;
        SqlConnection ConMaster;
        SqlDataReader drSupplier,drCounter,drGol_Prsh,drKota,drToP,drMata_Uang,drPayment_Method;
        string strSql = "";
        string vOldtxttblSupplier_Nama_Prsh, vOldtxttblSupplier_Gol_Prsh, vOldtxttblSupplier_Status, vOldtxttblSupplier_Alamat1, vOldtxttblSupplier_Alamat2, vOldtxttblSupplier_Kode_Pos;
        string vOldtxttblSupplier_Kota, vOldtxttblSupplier_Provinsi, vOldtxttblSupplier_No_Telp, vOldtxttblSupplier_No_Fax, vOldtxttblSupplier_Email, vOldtxttblSupplier_Contact_Person;
        string vOldtxttblSupplier_NPWP, vOldtxttblSupplier_TaxName, vOldtxttblSupplier_TaxAddress, vOldtxttblSupplier_PKP, vOldtxttblSupplier_SIUP, vOldtxttblSupplier_TermOfPayment, vOldtxttblSupplier_Mata_Uang, vOldtxttblSupplier_Payment_Method;
        decimal vOldtxttblSupplier_Limit_Total, vOldtxttblSupplier_Limit_Per_PO, vOldtxttblSupplier_Sisa_Limit_Total, vOldtxttblSupplier_Deposito, vOldtxttblSupplier_PPN, vOldtxttblSupplier_PPH;
        decimal vOldtxttblSupplier_Downpayment, vOldtxttblSupplier_Cashback_Balance, vOldtxttblSupplier_Discount_Balance, vOldtxttblSupplier_Debit_Note_Balance, vOldtxttblSupplier_Bonus_Balance;


        public FrmM_Supplier(bool _BolNew, string _kode)
        {
            InitializeComponent();
            BolNew = _BolNew;
            txttblSupplier_Kode_Prsh.Text = _kode;
            MulaiDariAwal();
        }
     
        private void FrmM_Supplier_Load(object sender, EventArgs e)
        {
            if (BolNew)
            {
                EmptyTextBox();
                txt_ReadOnly(false);
                Chk_Enable(true);
                ButtonSearch(true);
                Btn_EditCancelSaveDel(false, true, true, false);
                this.ActiveControl = txttblSupplier_Nama_Prsh;
            }
            else
            {              
                try
                {
                    ConMaster = ConnectionString.GetConnection();
                    strSql = "SELECT * FROM tblSupplier ";
                    strSql += "WHERE tblSupplier_Kode_Prsh='" + txttblSupplier_Kode_Prsh.Text + "'";              
                    using(SqlCommand cmdSupplier = new SqlCommand(strSql,ConMaster))
                    {             
                        drSupplier = cmdSupplier.ExecuteReader();
                    if (drSupplier.HasRows)
                    {
                        while (drSupplier.Read())
                        {
                            txttblSupplier_Nama_Prsh.Text = Convert.IsDBNull(drSupplier["tblSupplier_Nama_Prsh"]) ? "" : (string)drSupplier["tblSupplier_Nama_Prsh"];
                            txttblSupplier_Gol_Prsh.Text = Convert.IsDBNull(drSupplier["tblSupplier_Gol_Prsh"]) ? "" : (string)drSupplier["tblSupplier_Gol_Prsh"];
                            txttblSupplier_Status.Text = Convert.IsDBNull(drSupplier["tblSupplier_Status"]) ? "" : (string)drSupplier["tblSupplier_Status"];                            
                            txttblSupplier_Alamat1.Text = Convert.IsDBNull(drSupplier["tblSupplier_Alamat1"]) ? "" : (string)drSupplier["tblSupplier_Alamat1"];
                            txttblSupplier_Alamat2.Text = Convert.IsDBNull(drSupplier["tblSupplier_Alamat2"]) ? "" : (string)drSupplier["tblSupplier_Alamat2"];
                            txttblSupplier_Kode_Pos.Text = Convert.IsDBNull(drSupplier["tblSupplier_Kode_Pos"]) ? "" : (string)drSupplier["tblSupplier_Kode_Pos"];
                            txttblSupplier_Kota.Text = Convert.IsDBNull(drSupplier["tblSupplier_Kota"]) ? "" : (string)drSupplier["tblSupplier_Kota"];

                            txttblSupplier_Provinsi.Text = Convert.IsDBNull(drSupplier["tblSupplier_Provinsi"]) ? "" : (string)drSupplier["tblSupplier_Provinsi"];
                            txttblSupplier_No_Telp.Text = Convert.IsDBNull(drSupplier["tblSupplier_No_Telp"]) ? "" : (string)drSupplier["tblSupplier_No_Telp"];
                            txttblSupplier_No_Fax.Text = Convert.IsDBNull(drSupplier["tblSupplier_No_Fax"]) ? "" : (string)drSupplier["tblSupplier_No_Fax"];
                            txttblSupplier_Email.Text = Convert.IsDBNull(drSupplier["tblSupplier_Email"]) ? "" : (string)drSupplier["tblSupplier_Email"];
                            txttblSupplier_Contact_Person.Text = Convert.IsDBNull(drSupplier["tblSupplier_Contact_Person"]) ? "" : (string)drSupplier["tblSupplier_Contact_Person"];
                            txttblSupplier_NPWP.Text = Convert.IsDBNull(drSupplier["tblSupplier_NPWP"]) ? "" : (string)drSupplier["tblSupplier_NPWP"];
                            txttblSupplier_TaxName.Text = Convert.IsDBNull(drSupplier["tblSupplier_TaxName"]) ? "" : (string)drSupplier["tblSupplier_TaxName"];
                            txttblSupplier_TaxAddress.Text = Convert.IsDBNull(drSupplier["tblSupplier_TaxAddress"]) ? "" : (string)drSupplier["tblSupplier_TaxAddress"];
                            txttblSupplier_PKP.Text = Convert.IsDBNull(drSupplier["tblSupplier_PKP"]) ? "" : (string)drSupplier["tblSupplier_PKP"];
                            txttblSupplier_SIUP.Text = Convert.IsDBNull(drSupplier["tblSupplier_SIUP"]) ? "" : (string)drSupplier["tblSupplier_SIUP"];
                            txttblSupplier_TermOfPayment.Text = Convert.IsDBNull(drSupplier["tblSupplier_TermOfPayment"]) ? "" : (string)drSupplier["tblSupplier_TermOfPayment"];
                            txttblSupplier_PPN.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_PPN"]) ? 0 : (decimal)drSupplier["tblSupplier_PPN"]);
                            txttblSupplier_PPH.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_PPH"]) ? 0 : (decimal)drSupplier["tblSupplier_PPH"]);
                            txttblSupplier_Mata_Uang.Text = Convert.IsDBNull(drSupplier["tblSupplier_Mata_Uang"]) ? "" : (string)drSupplier["tblSupplier_Mata_Uang"];
                            txttblSupplier_Payment_Method.Text = Convert.IsDBNull(drSupplier["tblSupplier_Payment_Method"]) ? "" : (string)drSupplier["tblSupplier_Payment_Method"];

                            txttblSupplier_Limit_Total.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Limit_Total"]) ? 0 : (decimal)drSupplier["tblSupplier_Limit_Total"]);
                            txttblSupplier_Limit_Per_PO.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Limit_Per_PO"]) ? 0 : (decimal)drSupplier["tblSupplier_Limit_Per_PO"]);
                            txttblSupplier_Sisa_Limit_Total.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Sisa_Limit_Total"]) ? 0 : (decimal)drSupplier["tblSupplier_Sisa_Limit_Total"]);
                            txttblSupplier_Deposito.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Deposito"]) ? 0 : (decimal)drSupplier["tblSupplier_Deposito"]);
                            txttblSupplier_Downpayment.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Downpayment"]) ? 0 : (decimal)drSupplier["tblSupplier_Downpayment"]);
                            txttblSupplier_Cashback_Balance.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Cashback_Balance"]) ? 0 : (decimal)drSupplier["tblSupplier_Cashback_Balance"]);
                            txttblSupplier_Discount_Balance.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Discount_Balance"]) ? 0 : (decimal)drSupplier["tblSupplier_Discount_Balance"]);
                            txttblSupplier_Debit_Note_Balance.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Debit_Note_Balance"]) ? 0 : (decimal)drSupplier["tblSupplier_Debit_Note_Balance"]);
                            txttblSupplier_Bonus_Balance.Text = string.Format("{0:0.0000}", Convert.IsDBNull(drSupplier["tblSupplier_Bonus_Balance"]) ? 0 : (decimal)drSupplier["tblSupplier_Bonus_Balance"]);
                        }
                        Btn_EditCancelSaveDel(true, false, false, true);
                    }
                    else
                    {
                        MessageBox.Show("Data Supplier " + txttblSupplier_Kode_Prsh.Text + " tidak ditemukan...");
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
                    drSupplier.Close();
                    ConMaster.Close();
                }
            }
        }

        private void MulaiDariAwal()
        {
            this.TabCtrl_tblSupplier.SelectedTab = TabPage_General;
            this.ActiveControl = btnEdit;
            txt_ReadOnly(true);
            EmptyTextBox();
            ButtonSearch(false);
            Chk_Enable(false);
            Btn_EditCancelSaveDel(false, false, false, false);
        }

        private void txt_ReadOnly(bool vbol)
        {
            txttblSupplier_Kode_Prsh.ReadOnly = true;
            txttblSupplier_Nama_Prsh.ReadOnly = vbol;
            txttblSupplier_Gol_Prsh.ReadOnly = vbol;
            txttblSupplier_Status.ReadOnly = true;
            txttblSupplier_Alamat1.ReadOnly = vbol;
            txttblSupplier_Alamat2.ReadOnly = vbol;
            txttblSupplier_Kode_Pos.ReadOnly = vbol;
            txttblSupplier_Kota.ReadOnly = vbol;
            txttblSupplier_Provinsi.ReadOnly = true;
            txttblSupplier_No_Telp.ReadOnly = vbol;
            txttblSupplier_No_Fax.ReadOnly = vbol;
            txttblSupplier_Email.ReadOnly = vbol;
            txttblSupplier_Contact_Person.ReadOnly = vbol;
            txttblSupplier_NPWP.ReadOnly = vbol;
            txttblSupplier_TaxName.ReadOnly = vbol;
            txttblSupplier_TaxAddress.ReadOnly = vbol;
            txttblSupplier_PKP.ReadOnly = vbol;
            txttblSupplier_SIUP.ReadOnly = vbol;
            txttblSupplier_TermOfPayment.ReadOnly = vbol;
            txttblSupplier_PPN.ReadOnly = vbol;
            txttblSupplier_PPH.ReadOnly = vbol;
            txttblSupplier_Mata_Uang.ReadOnly = vbol;
            txttblSupplier_Payment_Method.ReadOnly = vbol;
            txttblSupplier_Limit_Total.ReadOnly = vbol;
            txttblSupplier_Limit_Per_PO.ReadOnly = vbol;
            txttblSupplier_Sisa_Limit_Total.ReadOnly = true;
            txttblSupplier_Deposito.ReadOnly = true;
            txttblSupplier_Downpayment.ReadOnly = true;
            txttblSupplier_Cashback_Balance.ReadOnly = true;
            txttblSupplier_Discount_Balance.ReadOnly = true;
            txttblSupplier_Debit_Note_Balance.ReadOnly = true;
            txttblSupplier_Bonus_Balance.ReadOnly = true;
        }

        private void Chk_Enable(bool vbol)
        {

        }

        private void EmptyTextBox()
        {
            //txttblSupplier_Kode_Prsh.Text="";      
            txttblSupplier_Nama_Prsh.Text = "";
            txttblSupplier_Gol_Prsh.Text = "";
            txttblSupplier_Status.Text = "";
            txttblSupplier_Alamat1.Text = "";
            txttblSupplier_Alamat2.Text = "";
            txttblSupplier_Kode_Pos.Text = "";
            txttblSupplier_Kota.Text = "";
            txttblSupplier_Provinsi.Text = "";
            txttblSupplier_No_Telp.Text = "";
            txttblSupplier_No_Fax.Text = "";
            txttblSupplier_Email.Text = "";
            txttblSupplier_Contact_Person.Text = "";
            txttblSupplier_NPWP.Text = "";
            txttblSupplier_TaxName.Text = "";
            txttblSupplier_TaxAddress.Text = "";
            txttblSupplier_PKP.Text = "";
            txttblSupplier_SIUP.Text = "";
            txttblSupplier_TermOfPayment.Text = "";
            txttblSupplier_PPN.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_PPH.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Mata_Uang.Text = "";
            txttblSupplier_Payment_Method.Text = "";
            txttblSupplier_Limit_Total.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Limit_Per_PO.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Sisa_Limit_Total.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Deposito.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Downpayment.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Cashback_Balance.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Discount_Balance.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Debit_Note_Balance.Text = string.Format("{0:0.0000}", 0);
            txttblSupplier_Bonus_Balance.Text = string.Format("{0:0.0000}", 0);
        }

        private void ButtonSearch(bool vbol)
        {
            btntxttblSupplier_Gol_Prsh.Enabled = vbol;
            btntxttblSupplier_Kota.Enabled = vbol;
            btnBrowse.Enabled = vbol;

            btntxttblSupplier_TermOfPayment.Enabled = vbol;
            btntxttblSupplier_Mata_Uang.Enabled = vbol;
            btntxttblSupplier_Payment_Method.Enabled = vbol;
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
            this.TabCtrl_tblSupplier.SelectedTab = TabPage_General;
            this.ActiveControl = txttblSupplier_Nama_Prsh;
            txt_ReadOnly(false);
            Chk_Enable(true);
            ButtonSearch(true);
            Btn_EditCancelSaveDel(false, true, true, false);

            vOldtxttblSupplier_Nama_Prsh = txttblSupplier_Nama_Prsh.Text;
            vOldtxttblSupplier_Gol_Prsh = txttblSupplier_Gol_Prsh.Text;
            vOldtxttblSupplier_Status = txttblSupplier_Status.Text;
            vOldtxttblSupplier_Alamat1 = txttblSupplier_Alamat1.Text;
            vOldtxttblSupplier_Alamat2 = txttblSupplier_Alamat2.Text;
            vOldtxttblSupplier_Kode_Pos = txttblSupplier_Kode_Pos.Text;
            vOldtxttblSupplier_Kota = txttblSupplier_Kota.Text;
            vOldtxttblSupplier_Provinsi = txttblSupplier_Provinsi.Text;
            vOldtxttblSupplier_No_Telp = txttblSupplier_No_Telp.Text;
            vOldtxttblSupplier_No_Fax = txttblSupplier_No_Fax.Text;
            vOldtxttblSupplier_Email = txttblSupplier_Email.Text;
            vOldtxttblSupplier_Contact_Person = txttblSupplier_Contact_Person.Text;
            vOldtxttblSupplier_NPWP = txttblSupplier_NPWP.Text;
            vOldtxttblSupplier_TaxName = txttblSupplier_TaxName.Text;
            vOldtxttblSupplier_TaxAddress = txttblSupplier_TaxAddress.Text;
            vOldtxttblSupplier_PKP = txttblSupplier_PKP.Text;
            vOldtxttblSupplier_SIUP = txttblSupplier_SIUP.Text;
            vOldtxttblSupplier_TermOfPayment = txttblSupplier_TermOfPayment.Text;
            vOldtxttblSupplier_Mata_Uang = txttblSupplier_Mata_Uang.Text;
            vOldtxttblSupplier_Payment_Method = txttblSupplier_Payment_Method.Text;
            vOldtxttblSupplier_Limit_Total = Convert.ToDecimal(txttblSupplier_Limit_Total.Text);
            vOldtxttblSupplier_Limit_Per_PO = Convert.ToDecimal(txttblSupplier_Limit_Per_PO.Text);
            vOldtxttblSupplier_Sisa_Limit_Total = Convert.ToDecimal(txttblSupplier_Sisa_Limit_Total.Text);
            vOldtxttblSupplier_Deposito = Convert.ToDecimal(txttblSupplier_Deposito.Text);
            vOldtxttblSupplier_PPN = Convert.ToDecimal(txttblSupplier_PPN.Text);
            vOldtxttblSupplier_PPH = Convert.ToDecimal(txttblSupplier_PPH.Text);
            vOldtxttblSupplier_Downpayment = Convert.ToDecimal(txttblSupplier_Downpayment.Text);
            vOldtxttblSupplier_Cashback_Balance = Convert.ToDecimal(txttblSupplier_Cashback_Balance.Text);
            vOldtxttblSupplier_Discount_Balance = Convert.ToDecimal(txttblSupplier_Discount_Balance.Text);
            vOldtxttblSupplier_Debit_Note_Balance = Convert.ToDecimal(txttblSupplier_Debit_Note_Balance.Text);
            vOldtxttblSupplier_Bonus_Balance = Convert.ToDecimal(txttblSupplier_Bonus_Balance.Text);        
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txt_ReadOnly(true);
            Chk_Enable(false);
            ButtonSearch(false);
            Btn_EditCancelSaveDel(true, false, false, true);

            txttblSupplier_Nama_Prsh.Text = vOldtxttblSupplier_Nama_Prsh;
            txttblSupplier_Gol_Prsh.Text = vOldtxttblSupplier_Gol_Prsh;
            txttblSupplier_Status.Text = vOldtxttblSupplier_Status;
            txttblSupplier_Alamat1.Text = vOldtxttblSupplier_Alamat1;
            txttblSupplier_Alamat2.Text = vOldtxttblSupplier_Alamat2;
            txttblSupplier_Kode_Pos.Text = vOldtxttblSupplier_Kode_Pos;
            txttblSupplier_Kota.Text = vOldtxttblSupplier_Kota;
            txttblSupplier_Provinsi.Text = vOldtxttblSupplier_Provinsi;
            txttblSupplier_No_Telp.Text = vOldtxttblSupplier_No_Telp;
            txttblSupplier_No_Fax.Text = vOldtxttblSupplier_No_Fax;
            txttblSupplier_Email.Text = vOldtxttblSupplier_Email;
            txttblSupplier_Contact_Person.Text = vOldtxttblSupplier_Contact_Person;
            txttblSupplier_NPWP.Text = vOldtxttblSupplier_NPWP;
            txttblSupplier_TaxName.Text = vOldtxttblSupplier_TaxName;
            txttblSupplier_TaxAddress.Text = vOldtxttblSupplier_TaxAddress;
            txttblSupplier_PKP.Text = vOldtxttblSupplier_PKP;
            txttblSupplier_SIUP.Text = vOldtxttblSupplier_SIUP;
            txttblSupplier_TermOfPayment.Text = vOldtxttblSupplier_TermOfPayment;
            txttblSupplier_Mata_Uang.Text = vOldtxttblSupplier_Mata_Uang;
            txttblSupplier_Payment_Method.Text = vOldtxttblSupplier_Payment_Method;
            txttblSupplier_Limit_Total.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Limit_Total);
            txttblSupplier_Limit_Per_PO.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Limit_Per_PO);
            txttblSupplier_Sisa_Limit_Total.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Sisa_Limit_Total);
            txttblSupplier_Deposito.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Deposito);
            txttblSupplier_PPN.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_PPN);
            txttblSupplier_PPH.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_PPH);
            txttblSupplier_Downpayment.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Downpayment);
            txttblSupplier_Cashback_Balance.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Cashback_Balance);
            txttblSupplier_Discount_Balance.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Discount_Balance);
            txttblSupplier_Debit_Note_Balance.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Debit_Note_Balance);
            txttblSupplier_Bonus_Balance.Text = string.Format("{0:0.0000}", vOldtxttblSupplier_Bonus_Balance);

            this.ActiveControl = btnEdit;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kode Supplier " + txttblSupplier_Kode_Prsh.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                            strSql = "DELETE FROM tblSupplier WHERE tblSupplier_Kode_Prsh='" + txttblSupplier_Kode_Prsh.Text + "'";
                            using (SqlCommand cmdSupplier = new SqlCommand(strSql, ConMaster))
                            {
                                cmdSupplier.ExecuteNonQuery();
                            }

                            string Sql = "INSERT INTO TblStatusLog_Supplier (TblStatusLog_FormName,tblStatusLog_PK1) VALUES(";
                            Sql += "'FrmM_Supplier','" + txttblSupplier_Kode_Prsh.Text + "')";
                            using (SqlCommand cmdHistory = new SqlCommand(Sql, ConMaster))
                            {
                                cmdHistory.ExecuteNonQuery();
                            }
                        }
                        scope.Complete();
                        MessageBox.Show("Berhasil Delete data..");


                        var FrmL_Supplier = Application.OpenForms.OfType<Master.Supplier.FrmL_Supplier>().FirstOrDefault();
                        if (FrmL_Supplier != null)
                        {
                            FrmL_Supplier.Activate();
                        }
                        else
                        {
                            new Master.Supplier.FrmL_Supplier().Show();
                        }
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            } 
        }

        private void AmbilKodeSupplier()
        {
            string strSql;
            strSql = "SELECT TOP 1 'V'+RIGHT('00000' + CAST((RIGHT(tblSupplier_Kode_Prsh,5)+1) AS NVARCHAR(5)),5) AS Kode_Prsh ";
            strSql += "FROM tblSupplier ORDER BY tblSupplier_Kode_Prsh DESC";
            using (SqlCommand cmdCounter = new SqlCommand(strSql, ConMaster))
            {
                drCounter = cmdCounter.ExecuteReader();
                if (drCounter.HasRows)
                {
                    while (drCounter.Read())
                    {
                        txttblSupplier_Kode_Prsh.Text = drCounter["Kode_Prsh"].ToString();
                    }
                }
                else
                {
                    txttblSupplier_Kode_Prsh.Text = "V00001";
                }
                drCounter.Close();
            }
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (txttblSupplier_Nama_Prsh.Text == "")
            {
                MessageBox.Show("Nama Supplier harus diisi..");
                vBol = false;
            }

            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                this.TabCtrl_tblSupplier.SelectedTab = TabPage_General;
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
                            AmbilKodeSupplier();
                            strSql = "INSERT INTO tblSupplier(tblSupplier_Kode_Prsh,tblSupplier_Nama_Prsh,tblSupplier_Gol_Prsh,tblSupplier_Status, ";
                            strSql += "tblSupplier_Alamat1,tblSupplier_Alamat2,tblSupplier_Kode_Pos,tblSupplier_Kota,tblSupplier_Provinsi,";
                            strSql += "tblSupplier_No_Telp,tblSupplier_No_Fax,tblSupplier_Email,tblSupplier_Contact_Person,tblSupplier_NPWP,tblSupplier_TaxName,";
                            strSql += "tblSupplier_TaxAddress,tblSupplier_PKP,tblSupplier_SIUP,tblSupplier_TermOfPayment,";
                            strSql += "tblSupplier_PPN,tblSupplier_PPH,tblSupplier_Mata_Uang,tblSupplier_Payment_Method,";
                            strSql += "tblSupplier_UInput,tblSupplier_UDateInput";
                            strSql += ") VALUES(";
                            strSql += "@Kode_Prsh,@Nama_Prsh,@Gol_Prsh,@Status,@Alamat1,@Alamat2,@Kode_Pos,@Kota,@Provinsi,";
                            strSql += "@No_Telp,@No_Fax,@Email,@Contact,@NPWP,@TaxName,@TaxAddress,@PKP,@SIUP,@ToP,";
                            strSql += "@PPN,@PPH,@Mata_Uang,@Payment_Method,@Uinput,@UdateInput)";
                            using (SqlCommand cmdSupplier = new SqlCommand(strSql, ConMaster))
                            {
                                cmdSupplier.Parameters.AddWithValue("@Kode_Prsh", txttblSupplier_Kode_Prsh.Text);
                                cmdSupplier.Parameters.AddWithValue("@Nama_Prsh", txttblSupplier_Nama_Prsh.Text);
                                cmdSupplier.Parameters.AddWithValue("@Gol_Prsh", txttblSupplier_Gol_Prsh.Text);
                                cmdSupplier.Parameters.AddWithValue("@Status", txttblSupplier_Status.Text);
                                cmdSupplier.Parameters.AddWithValue("@Alamat1", txttblSupplier_Alamat1.Text);
                                cmdSupplier.Parameters.AddWithValue("@Alamat2", txttblSupplier_Alamat2.Text);
                                cmdSupplier.Parameters.AddWithValue("@Kode_Pos", txttblSupplier_Kode_Pos.Text);
                                cmdSupplier.Parameters.AddWithValue("@Kota", txttblSupplier_Kota.Text);
                                cmdSupplier.Parameters.AddWithValue("@Provinsi", txttblSupplier_Provinsi.Text);
                                cmdSupplier.Parameters.AddWithValue("@No_Telp", txttblSupplier_No_Telp.Text);
                                cmdSupplier.Parameters.AddWithValue("@No_Fax", txttblSupplier_No_Fax.Text);
                                cmdSupplier.Parameters.AddWithValue("@Email", txttblSupplier_Email.Text);
                                cmdSupplier.Parameters.AddWithValue("@Contact", txttblSupplier_Contact_Person.Text);
                                cmdSupplier.Parameters.AddWithValue("@NPWP", txttblSupplier_NPWP.Text);
                                cmdSupplier.Parameters.AddWithValue("@TaxName", txttblSupplier_TaxName.Text);
                                cmdSupplier.Parameters.AddWithValue("@TaxAddress", txttblSupplier_TaxAddress.Text);
                                cmdSupplier.Parameters.AddWithValue("@PKP", txttblSupplier_PKP.Text);
                                cmdSupplier.Parameters.AddWithValue("@SIUP", txttblSupplier_SIUP.Text);
                                cmdSupplier.Parameters.AddWithValue("@ToP", txttblSupplier_TermOfPayment.Text);
                                cmdSupplier.Parameters.AddWithValue("@PPN", txttblSupplier_PPN.Text);
                                cmdSupplier.Parameters.AddWithValue("@PPH", txttblSupplier_PPH.Text);
                                cmdSupplier.Parameters.AddWithValue("@Mata_Uang", txttblSupplier_Mata_Uang.Text);
                                cmdSupplier.Parameters.AddWithValue("@Payment_Method", txttblSupplier_Payment_Method.Text);
                                cmdSupplier.Parameters.AddWithValue("@Uinput", "ITDIVISI");
                                cmdSupplier.Parameters.AddWithValue("@UdateInput", DateTime.Now);
                                cmdSupplier.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            strSql = "UPDATE tblSupplier SET tblSupplier_Nama_Prsh=@Nama, ";
                            strSql += "tblSupplier_Gol_Prsh=@Gol_Prsh,tblSupplier_Status=@Status,tblSupplier_Alamat1=@Alamat1,tblSupplier_Alamat2=@Alamat1,tblSupplier_Kode_Pos=@Kode_Pos,tblSupplier_Kota=@Kota,tblSupplier_Provinsi=@Provinsi,";
                            strSql += "tblSupplier_No_Telp=@No_Telp,tblSupplier_No_Fax=@No_Fax,tblSupplier_Email=@Email,tblSupplier_Contact_Person=@Contact_Person,";
                            strSql += "tblSupplier_NPWP=@NPWP,tblSupplier_TaxName=@TaxName,tblSupplier_TaxAddress=@TaxAddress,tblSupplier_PKP=@PKP,";
                            strSql += "tblSupplier_SIUP=@SIUP,tblSupplier_TermOfPayment=@TermOfPayment,";
                            strSql += "tblSupplier_PPN=@PPN,tblSupplier_PPH=@PPH,tblSupplier_Mata_Uang=@Mata_Uang,";
                            strSql += "tblSupplier_Payment_Method=@Payment_Method,tblSupplier_Limit_Total=@Limit_Total,tblSupplier_Limit_Per_PO=@Limit_Per_PO,tblSupplier_UEdit=@UEdit,tblSupplier_UDateEdit=@UDateEdit ";
                            strSql += "WHERE tblSupplier_Kode_Prsh='" + txttblSupplier_Kode_Prsh.Text + "'";
                            using (SqlCommand cmdSupplier = new SqlCommand(strSql, ConMaster))
                            {
                                cmdSupplier.Parameters.AddWithValue("@Nama", txttblSupplier_Nama_Prsh.Text);
                                cmdSupplier.Parameters.AddWithValue("@Gol_Prsh", txttblSupplier_Gol_Prsh.Text);
                                cmdSupplier.Parameters.AddWithValue("@Status", txttblSupplier_Status.Text);
                                cmdSupplier.Parameters.AddWithValue("@Alamat1", txttblSupplier_Alamat1.Text);
                                cmdSupplier.Parameters.AddWithValue("@Alamat2", txttblSupplier_Alamat2.Text);
                                cmdSupplier.Parameters.AddWithValue("@Kode_Pos", txttblSupplier_Kode_Pos.Text);
                                cmdSupplier.Parameters.AddWithValue("@Kota", txttblSupplier_Kota.Text);
                                cmdSupplier.Parameters.AddWithValue("@Provinsi", txttblSupplier_Provinsi.Text);
                                cmdSupplier.Parameters.AddWithValue("@No_Telp", txttblSupplier_No_Telp.Text);
                                cmdSupplier.Parameters.AddWithValue("@No_Fax", txttblSupplier_No_Fax.Text);
                                cmdSupplier.Parameters.AddWithValue("@Email", txttblSupplier_Email.Text);
                                cmdSupplier.Parameters.AddWithValue("@Contact_Person", txttblSupplier_Contact_Person.Text);
                                cmdSupplier.Parameters.AddWithValue("@NPWP", txttblSupplier_NPWP.Text);
                                cmdSupplier.Parameters.AddWithValue("@TaxName", txttblSupplier_TaxName.Text);
                                cmdSupplier.Parameters.AddWithValue("@TaxAddress", txttblSupplier_TaxAddress.Text);
                                cmdSupplier.Parameters.AddWithValue("@PKP", txttblSupplier_PKP.Text);
                                cmdSupplier.Parameters.AddWithValue("@SIUP", txttblSupplier_SIUP.Text);
                                cmdSupplier.Parameters.AddWithValue("@TermOfPayment", txttblSupplier_TermOfPayment.Text);
                                cmdSupplier.Parameters.AddWithValue("@PPN", txttblSupplier_PPN.Text);
                                cmdSupplier.Parameters.AddWithValue("@PPH", txttblSupplier_PPH.Text);
                                cmdSupplier.Parameters.AddWithValue("@Mata_Uang", txttblSupplier_Mata_Uang.Text);
                                cmdSupplier.Parameters.AddWithValue("@Payment_Method", txttblSupplier_Payment_Method.Text);
                                cmdSupplier.Parameters.AddWithValue("@Limit_Total", txttblSupplier_Limit_Total.Text);
                                cmdSupplier.Parameters.AddWithValue("@Limit_Per_PO", txttblSupplier_Limit_Per_PO.Text);
                                cmdSupplier.Parameters.AddWithValue("@UEdit", "ITDIVISI");
                                cmdSupplier.Parameters.AddWithValue("@UDateEdit", DateTime.Now);
                                cmdSupplier.ExecuteNonQuery();
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

                    txt_ReadOnly(true);
                    ButtonSearch(false);
                    Btn_EditCancelSaveDel(true, false, false, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void txttblSupplier_PPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
      (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as MetroFramework.Controls.MetroTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txttblSupplier_PPN.Text = String.Format("{0:0.0000}", txttblSupplier_PPN.Text == "" ? 0 : Convert.ToDecimal(txttblSupplier_PPN.Text));
            }
        }

        private void txttblSupplier_PPN_Leave(object sender, EventArgs e)
        {
            txttblSupplier_PPN.Text = String.Format("{0:0.0000}", txttblSupplier_PPN.Text == "" ? 0 : Convert.ToDecimal(txttblSupplier_PPN.Text));
        }

        private void txttblSupplier_PPH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
     (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as MetroFramework.Controls.MetroTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txttblSupplier_PPH.Text = String.Format("{0:0.0000}", txttblSupplier_PPH.Text == "" ? 0 : Convert.ToDecimal(txttblSupplier_PPH.Text));
            }
        }

        private void txttblSupplier_PPH_Leave(object sender, EventArgs e)
        {
            txttblSupplier_PPH.Text = String.Format("{0:0.0000}", txttblSupplier_PPH.Text == "" ? 0 : Convert.ToDecimal(txttblSupplier_PPH.Text));
        
        }

        private void txttblSupplier_Limit_Total_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
     (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as MetroFramework.Controls.MetroTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txttblSupplier_Limit_Total.Text = String.Format("{0:0.0000}", txttblSupplier_Limit_Total.Text == "" ? 0 : Convert.ToDecimal(txttblSupplier_Limit_Total.Text));
            }
        }

        private void txttblSupplier_Limit_Total_Leave(object sender, EventArgs e)
        {
            txttblSupplier_Limit_Total.Text = String.Format("{0:0.0000}", txttblSupplier_Limit_Total.Text == "" ? 0 : Convert.ToDecimal(txttblSupplier_Limit_Total.Text));
        }

        private void txttblSupplier_Limit_Per_PO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
    (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as MetroFramework.Controls.MetroTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txttblSupplier_Limit_Per_PO.Text = String.Format("{0:0.0000}", txttblSupplier_Limit_Per_PO.Text == "" ? 0 : Convert.ToDecimal(txttblSupplier_Limit_Per_PO.Text));
            }
        }

        private void txttblSupplier_Limit_Per_PO_Leave(object sender, EventArgs e)
        {
            txttblSupplier_Limit_Per_PO.Text = String.Format("{0:0.0000}", txttblSupplier_Limit_Per_PO.Text == "" ? 0 : Convert.ToDecimal(txttblSupplier_Limit_Per_PO.Text));
        
        }

        private void Ambil_Gol_Prsh()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh=@Gol_Prsh";
            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
            {
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txttblSupplier_Gol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (drGol_Prsh.HasRows)
                {
                    while (drGol_Prsh.Read())
                    {
                        txttblSupplier_Gol_Prsh.Text = Convert.IsDBNull(drGol_Prsh["tblGol_Prsh_Gol_Prsh"]) ? "" : (string)drGol_Prsh["tblGol_Prsh_Gol_Prsh"];
                    }
                }
                else
                {
                    MessageBox.Show("Type Perusahaan " + txttblSupplier_Gol_Prsh.Text + " doesn't exist");
                }
                drGol_Prsh.Close();
                ConMaster.Close();
            }
        }

        private void Cek_Gol_Prsh()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblGol_Prsh WHERE tblGol_Prsh_Gol_Prsh=@Gol_Prsh";
            using (SqlCommand cmdGol_Prsh = new SqlCommand(strSql, ConMaster))
            {
                cmdGol_Prsh.Parameters.AddWithValue("@Gol_Prsh", txttblSupplier_Gol_Prsh.Text);
                drGol_Prsh = cmdGol_Prsh.ExecuteReader();
                if (!drGol_Prsh.HasRows)
                {
                    MessageBox.Show("Type Perusahaan " + txttblSupplier_Gol_Prsh.Text + " doesn't exist");
                    Methods.ControlMgr.TblName = "tblGol_Prsh";
                    Methods.ControlMgr.TblSort = "ORDER BY tblGol_Prsh_Gol_Prsh";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Type Supplier";
                    FrmSearch.ShowDialog();

                    if (Methods.ControlMgr.Kode != "")
                    {
                        txttblSupplier_Gol_Prsh.Text = Methods.ControlMgr.Kode;
                        Ambil_Gol_Prsh();
                    }

                    Methods.ControlMgr.TblName = "";
                    Methods.ControlMgr.TblSort = "";
                    Methods.ControlMgr.Kode = "";
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
                txttblSupplier_Gol_Prsh.Focus();
            }
        }

        private void Ambil_Kota()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblKota WHERE tblKota_Kota=@Kota";
            using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
            {
                cmdKota.Parameters.AddWithValue("@Kota", txttblSupplier_Kota.Text);
                drKota = cmdKota.ExecuteReader();
                if (drKota.HasRows)
                {
                    while (drKota.Read())
                    {
                        txttblSupplier_Kota.Text = Convert.IsDBNull(drKota["tblKota_Kota"]) ? "" : (string)drKota["tblKota_Kota"];
                        txttblSupplier_Provinsi.Text = Convert.IsDBNull(drKota["tblKota_Provinsi"]) ? "" : (string)drKota["tblKota_Provinsi"];
                    }
                }
                else
                {
                    MessageBox.Show("Kota " + txttblSupplier_Kota.Text + " doesn't exist");
                }
                drKota.Close();
                ConMaster.Close();
            }
        }

        private void Cek_Kota()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();

            strSql = "SELECT * FROM tblKota WHERE tblKota_Kota=@Kota";
            using (SqlCommand cmdKota = new SqlCommand(strSql, ConMaster))
            {
                cmdKota.Parameters.AddWithValue("@Kota", txttblSupplier_Kota.Text);
                drKota = cmdKota.ExecuteReader();

                if (!drKota.HasRows)
                {
                    MessageBox.Show("Kota " + txttblSupplier_Kota.Text + " doesn't exist");
                    Methods.ControlMgr.TblName = "tblKota";
                    Methods.ControlMgr.TblSort = "ORDER BY tblKota_Kota";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Kota";
                    FrmSearch.ShowDialog();

                    if (Methods.ControlMgr.Kode != "")
                    {
                        txttblSupplier_Kota.Text = Methods.ControlMgr.Kode;
                        Ambil_Kota();
                    }

                    Methods.ControlMgr.TblName = "";
                    Methods.ControlMgr.TblSort = "";
                    Methods.ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drKota.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_Kota();
                }
                else
                {
                    txttblSupplier_Kota.Focus();
                }
            }
        }

        private void Ambil_ToP()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblToP WHERE tblToP_ToP=@ToP";

            using (SqlCommand cmdToP = new SqlCommand(strSql, ConMaster))
            {
                cmdToP.Parameters.AddWithValue("@Top", txttblSupplier_TermOfPayment.Text);
                drToP = cmdToP.ExecuteReader();

                if (drToP.HasRows)
                {
                    while (drToP.Read())
                    {
                        txttblSupplier_TermOfPayment.Text = Convert.IsDBNull(drToP["tblToP_ToP"]) ? "" : (string)drToP["tblToP_ToP"];
                    }
                }
                else
                {
                    MessageBox.Show("Kode ToP " + txttblSupplier_TermOfPayment.Text + " doesn't exist");
                }
                drToP.Close();
                ConMaster.Close();
            }
        }

        private void Cek_ToP()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblToP WHERE tblToP_ToP=@ToP";

            using (SqlCommand cmdToP = new SqlCommand(strSql, ConMaster))
            {
                cmdToP.Parameters.AddWithValue("@ToP", txttblSupplier_TermOfPayment.Text);
                drToP = cmdToP.ExecuteReader();

                if (!drToP.HasRows)
                {
                    MessageBox.Show("Term of Payment " + txttblSupplier_TermOfPayment.Text + " doesn't exist");
                    Methods.ControlMgr.TblName = "tblToP";
                    Methods.ControlMgr.TblSort = "ORDER BY tblToP_ToP";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Term of Payment";
                    FrmSearch.ShowDialog();

                    if (Methods.ControlMgr.Kode != "")
                    {
                        txttblSupplier_TermOfPayment.Text = Methods.ControlMgr.Kode;
                        Ambil_ToP();
                    }

                    Methods.ControlMgr.TblName = "";
                    Methods.ControlMgr.TblSort = "";
                    Methods.ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drToP.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_ToP();
                }
                else
                {
                    txttblSupplier_TermOfPayment.Focus();
                }
            }
        }

        private void Ambil_Mata_Uang()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblMata_Uang WHERE tblMata_Uang_Mata_Uang=@Mata_Uang";

            using (SqlCommand cmdMata_Uang = new SqlCommand(strSql, ConMaster))
            {
                cmdMata_Uang.Parameters.AddWithValue("@Mata_Uang", txttblSupplier_Mata_Uang.Text);
                drMata_Uang = cmdMata_Uang.ExecuteReader();
                if (drMata_Uang.HasRows)
                {
                    while (drMata_Uang.Read())
                    {
                        txttblSupplier_Mata_Uang.Text = Convert.IsDBNull(drMata_Uang["tblMata_Uang_Mata_Uang"]) ? "" : (string)drMata_Uang["tblMata_Uang_Mata_Uang"];
                    }
                }
                else
                {
                    MessageBox.Show("Mata Uang " + txttblSupplier_Mata_Uang.Text + " doesn't exist");
                }
                drMata_Uang.Close();
                ConMaster.Close();
            }
        }

        private void Cek_Mata_Uang()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblMata_Uang WHERE tblMata_Uang_Mata_Uang=@Mata_Uang";

            using (SqlCommand cmdMata_Uang = new SqlCommand(strSql, ConMaster))
            {
                cmdMata_Uang.Parameters.AddWithValue("@Mata_Uang", txttblSupplier_Mata_Uang.Text);
                drMata_Uang = cmdMata_Uang.ExecuteReader();

                if (!drMata_Uang.HasRows)
                {
                    MessageBox.Show("Mata Uang " + txttblSupplier_Mata_Uang.Text + " doesn't exist");
                    Methods.ControlMgr.TblName = "tblMata_Uang";
                    Methods.ControlMgr.TblSort = "ORDER BY tblMata_Uang_Mata_Uang";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Mata Uang";
                    FrmSearch.ShowDialog();

                    if (Methods.ControlMgr.Kode != "")
                    {
                        txttblSupplier_Mata_Uang.Text = Methods.ControlMgr.Kode;
                        Ambil_Mata_Uang();
                    }

                    Methods.ControlMgr.TblName = "";
                    Methods.ControlMgr.TblSort = "";
                    Methods.ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drMata_Uang.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_Mata_Uang();
                }
                else
                {
                    txttblSupplier_Mata_Uang.Focus();
                }
            }
        }

        private void Ambil_Payment_Method()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblPayment_Method WHERE tblPayment_Method_Method=@Payment_Method";

            using (SqlCommand cmdPayment_Method = new SqlCommand(strSql, ConMaster))
            {
                cmdPayment_Method.Parameters.AddWithValue("@Payment_Method", txttblSupplier_Payment_Method.Text);
                drPayment_Method = cmdPayment_Method.ExecuteReader();

                if (drPayment_Method.HasRows)
                {
                    while (drPayment_Method.Read())
                    {
                        txttblSupplier_Payment_Method.Text = Convert.IsDBNull(drPayment_Method["tblPayment_Method_Method"]) ? "" : (string)drPayment_Method["tblPayment_Method_Method"];
                    }
                }
                else
                {
                    MessageBox.Show("Payment Method " + txttblSupplier_Payment_Method.Text + " doesn't exist");
                }
                drPayment_Method.Close();
                ConMaster.Close();
            }
        }

        private void Cek_Payment_Method()
        {
            Boolean vBolFound = false;
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblPayment_Method WHERE tblPayment_Method_Method=@Payment_Method";

            using (SqlCommand cmdPayment_Method = new SqlCommand(strSql, ConMaster))
            {
                cmdPayment_Method.Parameters.AddWithValue("@Payment_Method", txttblSupplier_Payment_Method.Text);
                drPayment_Method = cmdPayment_Method.ExecuteReader();
                if (!drPayment_Method.HasRows)
                {
                    MessageBox.Show("Payment Method " + txttblSupplier_Payment_Method.Text + " doesn't exist");

                    Methods.ControlMgr.TblName = "tblPayment_Method";
                    Methods.ControlMgr.TblSort = "ORDER BY tblPayment_Method_Method";

                    Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Payment Method";
                    FrmSearch.ShowDialog();

                    if (Methods.ControlMgr.Kode != "")
                    {
                        txttblSupplier_Payment_Method.Text = Methods.ControlMgr.Kode;
                        Ambil_Payment_Method();
                    }

                    Methods.ControlMgr.TblName = "";
                    Methods.ControlMgr.TblSort = "";
                    Methods.ControlMgr.Kode = "";
                }
                else
                {
                    vBolFound = true;
                }
                drPayment_Method.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_Payment_Method();
                }
                else
                {
                    txttblSupplier_Payment_Method.Focus();
                }
            }
        }

        private void txttblSupplier_Gol_Prsh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Gol_Prsh();
            }
        }

        private void txttblSupplier_Gol_Prsh_Leave(object sender, EventArgs e)
        {
            Cek_Gol_Prsh();
        }

        private void txttblSupplier_Kota_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Kota();
            }
        }

        private void txttblSupplier_Kota_Leave(object sender, EventArgs e)
        {
            Cek_Kota();
        }

        private void txttblSupplier_TermOfPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_ToP();
            }
        }

        private void txttblSupplier_TermOfPayment_Leave(object sender, EventArgs e)
        {
            Cek_ToP();
        }

        private void txttblSupplier_Mata_Uang_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Mata_Uang();
            }
        }

        private void txttblSupplier_Mata_Uang_Leave(object sender, EventArgs e)
        {
            Cek_Mata_Uang();
        }

        private void txttblSupplier_Payment_Method_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Payment_Method();
            }
        }

        private void txttblSupplier_Payment_Method_Leave(object sender, EventArgs e)
        {
            Cek_Payment_Method();
        }

        private void btntxttblSupplier_Gol_Prsh_Click(object sender, EventArgs e)
        {
            Methods.ControlMgr.TblName = "tblGol_Prsh";
            Methods.ControlMgr.TblSort = "ORDER BY tblGol_Prsh_Gol_Prsh";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Type Supplier";
            FrmSearch.ShowDialog();

            if (Methods.ControlMgr.Kode != "")
            {
                txttblSupplier_Gol_Prsh.Text = Methods.ControlMgr.Kode;
            }

            Methods.ControlMgr.TblName = "";
            Methods.ControlMgr.TblSort = "";
            Methods.ControlMgr.Kode = "";
        }

        private void btntxttblSupplier_Kota_Click(object sender, EventArgs e)
        {
            Methods.ControlMgr.TblName = "tblKota";
            Methods.ControlMgr.TblSort = "ORDER BY tblKota_Kota";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Kota";
            FrmSearch.ShowDialog();

            if (Methods.ControlMgr.Kode != "")
            {
                txttblSupplier_Kota.Text = Methods.ControlMgr.Kode;
            }

            Methods.ControlMgr.TblName = "";
            Methods.ControlMgr.TblSort = "";
            Methods.ControlMgr.Kode = "";
        }

        private void btntxttblSupplier_TermOfPayment_Click(object sender, EventArgs e)
        {
            Methods.ControlMgr.TblName = "tblToP";
            Methods.ControlMgr.TblSort = "ORDER BY tblToP_ToP";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Term of Payment";
            FrmSearch.ShowDialog();

            if (Methods.ControlMgr.Kode != "")
            {
                txttblSupplier_TermOfPayment.Text = Methods.ControlMgr.Kode;
            }

            Methods.ControlMgr.TblName = "";
            Methods.ControlMgr.TblSort = "";
            Methods.ControlMgr.Kode = "";
        }

        private void btntxttblSupplier_Mata_Uang_Click(object sender, EventArgs e)
        {
            Methods.ControlMgr.TblName = "tblMata_Uang";
            Methods.ControlMgr.TblSort = "ORDER BY tblMata_Uang_Mata_Uang";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Mata Uang";
            FrmSearch.ShowDialog();

            if (Methods.ControlMgr.Kode != "")
            {
                txttblSupplier_Mata_Uang.Text = Methods.ControlMgr.Kode;
            }

            Methods.ControlMgr.TblName = "";
            Methods.ControlMgr.TblSort = "";
            Methods.ControlMgr.Kode = "";
        }

        private void btntxttblSupplier_Payment_Method_Click(object sender, EventArgs e)
        {
            Methods.ControlMgr.TblName = "tblPayment_Method";
            Methods.ControlMgr.TblSort = "ORDER BY tblPayment_Method_Method";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Payment Method";
            FrmSearch.ShowDialog();

            if (Methods.ControlMgr.Kode != "")
            {
                txttblSupplier_Payment_Method.Text = Methods.ControlMgr.Kode;
            }

            Methods.ControlMgr.TblName = "";
            Methods.ControlMgr.TblSort = "";
            Methods.ControlMgr.Kode = "";
        }
    }
}
