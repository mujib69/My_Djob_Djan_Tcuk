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
    public partial class FrmM_Sales : MetroFramework.Forms.MetroForm
    {
        string Mode = "";

        SqlConnection ConnMaster;
        SqlDataReader drtblSales;
        SqlDataReader drtblContact;
        string strSql = "";

        DataTable namesTable_Contact = new DataTable("tmpContact");

        int RowAffected;

        Boolean vView, vNew, vEdit, vDelete;

        public FrmM_Sales(string _Mode, string _Kode)
        {
            InitializeComponent();
            Mode = _Mode;
            txttblSales_Kode_Sls.Text = _Kode;
        }

        private void Create_tmpContact()
        {
            DataColumn idColumn00 = new DataColumn();
            DataColumn idColumn01 = new DataColumn();
            DataColumn idColumn02 = new DataColumn();
            DataColumn idColumn03 = new DataColumn();
            DataColumn idColumn04 = new DataColumn();
            DataColumn idColumn05 = new DataColumn();
            DataColumn idColumn06 = new DataColumn();

            idColumn00.DataType = System.Type.GetType("System.String");
            idColumn00.ColumnName = "tblContact_Nama";
            idColumn01.DataType = System.Type.GetType("System.String");
            idColumn01.ColumnName = "tblContact_Jabatan";
            idColumn02.DataType = System.Type.GetType("System.String");
            idColumn02.ColumnName = "tblContact_Phone1";
            idColumn03.DataType = System.Type.GetType("System.String");
            idColumn03.ColumnName = "tblContact_Ext1";
            idColumn04.DataType = System.Type.GetType("System.String");
            idColumn04.ColumnName = "tblContact_HP";
            idColumn05.DataType = System.Type.GetType("System.String");
            idColumn05.ColumnName = "tblContact_Email";
            idColumn06.DataType = System.Type.GetType("System.Decimal");
            idColumn06.ColumnName = "tblContact_RecId";


            namesTable_Contact.Columns.Add(idColumn00);
            namesTable_Contact.Columns.Add(idColumn01);
            namesTable_Contact.Columns.Add(idColumn02);
            namesTable_Contact.Columns.Add(idColumn03);
            namesTable_Contact.Columns.Add(idColumn04);
            namesTable_Contact.Columns.Add(idColumn05);
            namesTable_Contact.Columns.Add(idColumn06);
        }

        private void BuatDtGridView_Contact()
        {
            namesTable_Contact.Clear();
            DtGridView_Contact.DataSource = namesTable_Contact;

            namesTable_Contact.Columns[0].ColumnName = "Nama";
            namesTable_Contact.Columns[1].ColumnName = "Jabatan";
            namesTable_Contact.Columns[2].ColumnName = "Phone";
            namesTable_Contact.Columns[3].ColumnName = "Ext";
            namesTable_Contact.Columns[4].ColumnName = "Handphone";
            namesTable_Contact.Columns[5].ColumnName = "Email";
            namesTable_Contact.Columns[6].ColumnName = "RecId";

            DtGridView_Contact.Columns[0].Width = 100;
            DtGridView_Contact.Columns[1].Width = 100;
            DtGridView_Contact.Columns[2].Width = 100;
            DtGridView_Contact.Columns[3].Width = 100;
            DtGridView_Contact.Columns[4].Width = 100;
            DtGridView_Contact.Columns[5].Width = 100;
            DtGridView_Contact.Columns[6].Width = 100;

            DtGridView_Contact.Columns[6].Visible = false;
            DtGridView_Contact.Refresh();
        }

        private void IsiDtGridView_Contact()
        {
            strSql = "SELECT * ";
            strSql += "FROM tblContact ";
            strSql += "WHERE tblContact_tblName='tblSales' ";
            strSql += "AND tblContact_Kode='" + txttblSales_Kode_Sls.Text + "'";
            namesTable_Contact.Clear();
            ConnMaster = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(strSql, ConnMaster))
            {
                drtblContact = cmd.ExecuteReader();
                while (drtblContact.Read())
                {
                    DataRow row;
                    row = namesTable_Contact.NewRow();
                    row["Nama"] = Convert.IsDBNull(drtblContact["tblContact_Nama"]) ? "" : (string)drtblContact["tblContact_Nama"];
                    row["Jabatan"] = Convert.IsDBNull(drtblContact["tblContact_Jabatan"]) ? "" : (string)drtblContact["tblContact_Jabatan"];
                    row["Phone"] = Convert.IsDBNull(drtblContact["tblContact_Phone1"]) ? "" : (string)drtblContact["tblContact_Phone1"];
                    row["Ext"] = Convert.IsDBNull(drtblContact["tblContact_Ext1"]) ? "" : (string)drtblContact["tblContact_Ext1"];
                    row["Handphone"] = Convert.IsDBNull(drtblContact["tblContact_HP"]) ? "" : (string)drtblContact["tblContact_HP"];
                    row["Email"] = Convert.IsDBNull(drtblContact["tblContact_Email"]) ? "" : (string)drtblContact["tblContact_Email"];
                    row["RecId"] = Convert.IsDBNull(drtblContact["tblContact_RecId"]) ? 0 : Convert.ToInt32(drtblContact["tblContact_RecId"]);
                    namesTable_Contact.Rows.Add(row);
                }
            }
            drtblContact.Close();
            ConnMaster.Close();
            DtGridView_Contact.Refresh();
        }

        private void FrmM_Sales_Load(object sender, EventArgs e)
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

            mTabCtrl_Sales.SelectedTab = mTabPage_General;

            cbxtblSales_Counter_Sls.SelectedItem = "";
            cbxtblSales_Toko_Proyek.SelectedItem = "";

            Create_tmpContact();
            DtGridView_Contact.DataSource = "";
            BuatDtGridView_Contact();

            if (Mode == "View")
            {
                ModeView();
            }
            else if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                FillForm();
                IsiDtGridView_Contact();
                ModeEdit();
            }
        }        

        private void FillForm()
        {
            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblSales WHERE tblSales_Kode_Sls=@Kode_Sls";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Kode_Sls", txttblSales_Kode_Sls.Text);
                    drtblSales = Cmd.ExecuteReader();
                    if (drtblSales.HasRows)
                    {
                        while (drtblSales.Read())
                        {
                            txttblSales_Kode_Sls.Text = Convert.IsDBNull(drtblSales["tblSales_Kode_Sls"]) ? "" : (string)drtblSales["tblSales_Kode_Sls"];
                            txttblSales_Nama_Sales.Text = Convert.IsDBNull(drtblSales["tblSales_Nama_Sales"]) ? "" : (string)drtblSales["tblSales_Nama_Sales"];
                            txttblSales_Group.Text = Convert.IsDBNull(drtblSales["tblSales_Group"]) ? "" : (string)drtblSales["tblSales_Group"];

                            cbxtblSales_Counter_Sls.SelectedItem = Convert.IsDBNull(drtblSales["tblSales_Counter_Sls"]) ? "" : (string)drtblSales["tblSales_Counter_Sls"];
                            cbxtblSales_Toko_Proyek.SelectedItem = Convert.IsDBNull(drtblSales["tblSales_Toko_Proyek"]) ? "" : (string)drtblSales["tblSales_Toko_Proyek"];

                            txttblSales_Persen.Text = string.Format("{0:#,##0.00}", Convert.IsDBNull(drtblSales["tblSales_Persen"]) ? 0 : (decimal)drtblSales["tblSales_Persen"]);                                                                                   
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data Kode Sales " + txttblSales_Kode_Sls.Text + " tidak ditemukan...");
                        return;
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
        }

        private void txt_ReadOnly(bool vBol)
        {
            txttblSales_Kode_Sls.ReadOnly = vBol;
            txttblSales_Nama_Sales.ReadOnly = vBol;
            txttblSales_Group.ReadOnly = vBol;
            txttblSales_Persen.ReadOnly = vBol;
        }

        private void cbx_Enable(bool vBol)
        {
            cbxtblSales_Counter_Sls.Enabled = vBol;
            cbxtblSales_Toko_Proyek.Enabled = vBol;        
        }

        private void ModeView()
        {
            txt_ReadOnly(true);
            cbx_Enable(false);
            btn_Enable(false); //contact
            Btn_EditCancelSaveDel(true, false, false, true);
        }

        private void btn_Enable(bool vBol)
        {
            btnNew_Ct.Enabled = vBol;
            btnEdit_Ct.Enabled = vBol;
            btnDelete_Ct.Enabled = vBol;
        }

        private void ModeNew()
        {
            this.ActiveControl = txttblSales_Kode_Sls;

            txt_ReadOnly(false);
            cbx_Enable(true);
            btn_Enable(true); //contact

            Btn_EditCancelSaveDel(false, true, true, false);
        }

        private bool Used()
        {
            bool vBol = false;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblCustomer WHERE tblCustomer_Kode_Sls=@Kode_Sls";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Kode_Sls", txttblSales_Kode_Sls.Text.Trim());
                    drtblSales = Cmd.ExecuteReader();
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
            }
            return vBol;
        }

        private void ModeEdit()
        {
            txt_ReadOnly(false);
            cbx_Enable(true);
            btn_Enable(true); //contact
        
            txttblSales_Kode_Sls.ReadOnly = true;
            
            Btn_EditCancelSaveDel(false, true, true, false);
        }

        private void Btn_EditCancelSaveDel(bool vEdit, bool vCancel, bool vSave, bool vDel)
        {
            BtnEdit.Enabled = vEdit;
            BtnCancel.Enabled = vCancel;
            BtnSave.Enabled = vSave;
            BtnDelete.Enabled = vDel;
        }

        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            if (Mode == "Edit" && vBol && !CekKodeSls())
            {
                MessageBox.Show("Kode Sales " + txttblSales_Kode_Sls + " tidak ditemukan..");
                vBol = false;
            }

            if (vBol && txttblSales_Kode_Sls.Text.Trim() == "")
            {
                MessageBox.Show("Kode Sales harus diisi..");
                this.ActiveControl = txttblSales_Kode_Sls;
                vBol = false;
            }

            if (vBol && txttblSales_Nama_Sales.Text.Trim() == "")
            {
                MessageBox.Show("Nama Sales harus diisi..");
                this.ActiveControl = txttblSales_Nama_Sales;
                vBol = false;
            }

            if (Mode == "New" && vBol && CekKodeSls())
            {
                MessageBox.Show("Kode Sales sudah ada..");
                this.ActiveControl = txttblSales_Kode_Sls;
                vBol = false;
            }

            if (vBol && cbxtblSales_Counter_Sls.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Counter / Sales harus dipilih..");
                this.ActiveControl = cbxtblSales_Counter_Sls;
                vBol = false;
            }

            if (vBol && cbxtblSales_Toko_Proyek.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Toko / Proyek harus dipilih..");
                this.ActiveControl = cbxtblSales_Toko_Proyek;
                vBol = false;
            }

            return vBol;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidGeneral())
            {
                return;
            }

            try
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ConnMaster = ConnectionString.GetConnection();
                    using (ConnMaster)
                    {
                        if (Mode == "New")
                        {
                            strSql = "INSERT INTO tblSales(";
                            strSql += "tblSales_Kode_Sls,tblSales_Nama_Sales,tblSales_Group,tblSales_Persen,tblSales_Counter_Sls,tblSales_Toko_Proyek,";
                            strSql += "tblSales_UInput) ";
                            strSql += "VALUES(@Kode_Sls,";
                            strSql += "@Nama_Sales,@Group,@Persen,@Counter_Sls,@Toko_Proyek,";
                            strSql += "@UInput)";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@Kode_Sls", txttblSales_Kode_Sls.Text);
                                Cmd.Parameters.AddWithValue("@Nama_Sales", txttblSales_Nama_Sales.Text);
                                Cmd.Parameters.AddWithValue("@Group", txttblSales_Group.Text);
                                Cmd.Parameters.AddWithValue("@Persen", txttblSales_Persen.Text);
                                Cmd.Parameters.AddWithValue("@Counter_Sls", cbxtblSales_Counter_Sls.SelectedItem.ToString());
                                Cmd.Parameters.AddWithValue("@Toko_Proyek", cbxtblSales_Toko_Proyek.SelectedItem.ToString());
                                Cmd.Parameters.AddWithValue("@UInput", "ITDIVISI");
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            strSql = "UPDATE tblSales SET ";
                            strSql += "tblSales_Nama_Sales=@Nama_Sales,";
                            strSql += "tblSales_Group=@Group,";
                            strSql += "tblSales_Counter_Sls=@Counter_Sls,";
                            strSql += "tblSales_Toko_Proyek=@Toko_Proyek,";
                            strSql += "tblSales_Persen=@Persen,";
                            strSql += "tblSales_UEdit=@UEdit,";
                            strSql += "tblSales_UDate_Edit=GETDATE() ";
                            strSql += "WHERE tblSales_Kode_Sls=@Kode_Sls";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@Kode_Sls", txttblSales_Kode_Sls.Text);
                                Cmd.Parameters.AddWithValue("@Nama_Sales", txttblSales_Nama_Sales.Text);
                                Cmd.Parameters.AddWithValue("@Group", txttblSales_Group.Text);
                                Cmd.Parameters.AddWithValue("@Persen", txttblSales_Persen.Text);
                                Cmd.Parameters.AddWithValue("@Counter_Sls", cbxtblSales_Counter_Sls.SelectedItem.ToString());
                                Cmd.Parameters.AddWithValue("@Toko_Proyek", cbxtblSales_Toko_Proyek.SelectedItem.ToString());
                                Cmd.Parameters.AddWithValue("@UEdit", "ITDIVISI");
                                RowAffected = Cmd.ExecuteNonQuery();
                            }                        
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

            if (Mode == "New")
            {
                Mode = "View";
                MessageBox.Show("Insert Success");
            }
            else
            {
                if (RowAffected == 0)
                {
                    MessageBox.Show("Record Data baru saja diupdate oleh orang lain, silihkan ulangi kembali");
                    this.Close();
                }
                else
                {
                    Mode = "View";
                    MessageBox.Show("Update Success");
                }
            }
            ModeView();
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

            ModeEdit();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean CekKodeSls()
        {
            Boolean vBol = true;

            try
            {
                ConnMaster = ConnectionString.GetConnection();
                strSql = "SELECT * FROM tblSales WHERE tblSales_Kode_Sls=@Kode_Sls";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Kode_Sls", txttblSales_Kode_Sls.Text);
                    drtblSales = Cmd.ExecuteReader();
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

            if (!CekKodeSls())
            {
                MessageBox.Show("Kode Sales " + txttblSales_Kode_Sls.Text + " tidak ditemukan..");
                return;
            }
            
            if (Used())
            {
                MessageBox.Show("Kode Sales " + txttblSales_Kode_Sls.Text + " sudah pernah digunakan..");
                return;
            }


            DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Kode Sales " + txttblSales_Kode_Sls.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "DELETE FROM tblContact WHERE tblContact_tblName='tblSales' AND tblContact_Kode=@Kode_Sls";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@Kode_Sls", txttblSales_Kode_Sls.Text);
                            }

                            strSql = "DELETE FROM tblSales WHERE tblSales_Kode_Sls=@Kode_Sls";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@Kode_Sls", txttblSales_Kode_Sls.Text);
                                RowAffected = Cmd.ExecuteNonQuery();
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

                if (RowAffected == 0)
                {
                    MessageBox.Show("Record Data baru saja diupdate oleh orang lain, silihkan ulangi kembali");
                }
                else
                {
                    MessageBox.Show("Kode Sales " + txttblSales_Kode_Sls.Text + ", berhasil dihapus..");
                }
                this.Close();
            }
        }

        private void btnNew_Ct_Click(object sender, EventArgs e)
        {
            if (txttblSales_Kode_Sls.Text.Trim() == "")
            {
                MessageBox.Show("Save Kode Sales terlebih dahulu..");
                return;
            }

            Form Frm_Contact = new PopUp.Contact.Frm_Contact(true, 0, txttblSales_Kode_Sls.Text.Trim(), "tblSales");
            Frm_Contact.Text = "Contact " + txttblSales_Kode_Sls.Text + " - " + txttblSales_Nama_Sales.Text;
            Frm_Contact.ShowDialog();
            IsiDtGridView_Contact();
        }

        private void btnEdit_Ct_Click(object sender, EventArgs e)
        {
            if (DtGridView_Contact.Rows.Count > 0)
            {
                var _RecId = (decimal)DtGridView_Contact.CurrentRow.Cells["RecId"].Value;
                Form Frm_Contact = new PopUp.Contact.Frm_Contact(false, _RecId, txttblSales_Kode_Sls.Text, "tblSales");
                Frm_Contact.ShowDialog();
                Frm_Contact.Text = "Contact " + txttblSales_Kode_Sls.Text + " - " + txttblSales_Nama_Sales.Text;
                IsiDtGridView_Contact();
            }
            else
            {
                MessageBox.Show("Tidak ada data untuk diedit");
            }
        }

        private void btnDelete_Ct_Click(object sender, EventArgs e)
        {
            if (DtGridView_Contact.Rows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Yakin ingin hapus Contact " + DtGridView_Contact.CurrentRow.Cells["Nama"].Value.ToString() + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
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
                                strSql = "DELETE FROM tblContact WHERE tblContact_RecId='" + DtGridView_Contact.CurrentRow.Cells["RecId"].Value.ToString() + "'";
                                using (SqlCommand cmdContact = new SqlCommand(strSql, ConnMaster))
                                {
                                    cmdContact.ExecuteNonQuery();
                                }
                            }
                            scope.Complete();
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

                    MessageBox.Show("Berhasil Delete data..");
                    IsiDtGridView_Contact();
                }
            }
            else
            {
                MessageBox.Show("Tidak ada data untuk dihapus..");
            }
        }

        private void FrmM_Sales_Shown(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && vView == false)
            {
                this.Close();
            } 
        }

    }
}
