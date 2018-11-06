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

namespace ISBS_New
{
    public partial class InquiryV1 : MetroFramework.Forms.MetroForm
    {
        #region INISIASI AWAL
        //PENANDA UNTUK DATAGRID
        private int Limit1, Limit2, Total, Page1, Page2;

        //BERHUBUNGAN DENGAN SQL
        public string Query;
        public string QuerySearch;
        public string Order;
        public string Field;
        private string Where;

        //UNTUK MEMANGGIL QUERY
        public string Table;
        public string PrimaryKey;
        public string[] Data;
        public string[] FilterDate;
        public string[] FilterText;
        public string WherePlus;

        //UNTUK MENGEMBALIKAN NILAI YANG DISELECT
        public string[] Select;

        //UNTUK MENAMPILKAN PARAMETER YANG INGIN DITAMPILKAN SAAT DELETE
        public string[] Delete;

        //Hide column 
        public string[] HideField;

        //LUPA....
        //string IdNamePK = "";
        //string Type = "";
        //private Boolean tmpAmbilKodeNo = false;
        
        //UNTUK CMBLOAD AGAR TIDAK TERJADI ERROR KARENA DATA BELUM ADA DISQL
        private Boolean RefreshGridStatus = false;

        //DIGUNAKAN OLEH PARENT AGAR FORM TERDETEKSI SEBAGAI CHILD
        private List<Form> MDI = new List<Form>();
        public Form OpenForm = new Form();
        public MainMenu Parent = new MainMenu();
        public string InquiryName = "";
        public string FormName = "";
        public string Notes = "";
        
        //CreateJournal, when batalkan/ gunakan status
        private bool CrtJournal = false;
        public void CreateJournal(bool status)
        {
            CrtJournal = status;
        }

        public InquiryV1()
        {
            InitializeComponent();
            cmbShowLoad();
            ToolTipNotes.SetToolTip(this.pictureBox1, Notes);
        }

        private void InquiryV1_Load(object sender, EventArgs e)
        {
            Location = new Point(100, 48);
            LoadCmbFilter();
            RefreshDataGrid();
            ToolTipNotes.SetToolTip(this.pictureBox1, Notes);
        }
        #endregion

        #region METHOD UNTUK MENAMPILKAN DATAGRID, CMBSHOW, REFRESH, DSB
        private void cmbShowLoad()
        {
            try
            {
                using (Method C = new Method ())
                {
                    Query = "Select CmbValue From [Setting].[CmbBox] ";
                    C.Dr = C.ReturnDr(Query);
                    while (C.Dr.Read())
                    {
                        cmbShow.Items.Add(C.Dr["CmbValue"].ToString());
                    }

                    Total = Convert.ToInt32(C.ReturnScalar(Query));

                    cmbShow.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        //MELAKUKAN REFRESH DATAGRID
        public void RefreshDataGrid()
        {
            try
            {
                //Menampilkan data
                using (Method C = new Method ())
                {
                    Query = "Select * from (Select ROW_NUMBER() OVER (ORDER BY " + Order + ") No, * From (" + QuerySearch + " ) a Where 1=1 and (" + GetWhere() + ") ) a ";
                    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

                    dgvInquiry.AutoGenerateColumns = true;
                    dgvInquiry.DataSource = C.DgvAutoQuery(Query);
                    dgvInquiry.Refresh();
                    
                    GetTotalPaging();

                    if (HideField != null)
                    {
                        C.DgvVisible(dgvInquiry, HideField);
                    }
                    for (int i = 0; i < FilterText.Length; i++)
                    {
                        string text = FilterText[i];
                        if (text.Contains("Date"))
                        {
                            dgvInquiry.Columns[FilterText[i]].DefaultCellStyle.Format = "dd/MM/yyyy";
                        }
                    }
                    dgvInquiry.AutoResizeColumns();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Connection.GlobalException(Ex));
            }
            finally{}
        }

        //MENDAPATKAN JUMLAH TOTAL PAGING
        private void GetTotalPaging()
        {
            try
            {
                //Mengambil nilai total paging
                Method C = new Method ();

                Query = "Select Count(" + PrimaryKey + ") From (" + QuerySearch + ") a Where 1=1 and (" + GetWhere() + ")";
                Total = Convert.ToInt32(C.ReturnScalar(Query));
                //Conn.Close();

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling(Total / Convert.ToDecimal(cmbShow.Text));
                lblPage.Text = Page2.ToString();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Connection.GlobalException(Ex));
            }
            finally
            {
                //Conn.Close();
            }
        }

        //DIGUNAKAN UNTUK WHERE QUERY
        private string GetWhere()
        {
            Where = "";
            if (cmbFilter.Text.Trim() == "All")
            {
                
                for (int i = 0; i < FilterText.Count(); i++)
                {
                    Where += FilterText[i] + " like '%" + txtFilter.Text.Replace("'", "''") + "%' ";
                    if (i < FilterText.Count() - 1)
                    {
                        Where += " or ";
                    }
                }
            }
            //edited Thaddaeus Matthias, 3 April 2018
            else if (dtFrom.Enabled == true) 
            {
                Where += " " + cmbFilter.Text + " >= '" + dtFrom.Value.ToShortDateString() + "' and " + cmbFilter.Text + " <= '" + dtTo.Value.ToShortDateString() + "'";
            } 
                //end=======================
            else
            {
                Where += cmbFilter.Text + " like '%" + txtFilter.Text.Replace("'", "''") + "%' ";
            }
            return Where + WherePlus;
        }


        private void LoadCmbFilter()
        {
            try
            {
                cmbFilter.Items.Add("All");
                for (int i = 0; i < FilterText.Count(); i++)
                {
                    cmbFilter.Items.Add(FilterText[i]);
                }
                RefreshGridStatus = true;
                cmbFilter.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Connection.GlobalException(Ex));
            }
            finally
            {
                //Conn.Close();
            }
        }

        //UNTUK MEMILIH DATA
        private void SelectData()
        {
            try
            {
                int Index = dgvInquiry.CurrentRow.Index;

                Variable.Kode = new string[Select.Count()];
                for (int i = 0; i < Select.Count(); i++)
                {
                    Variable.Kode[i] = dgvInquiry.Rows[Index].Cells[Select[i]].Value.ToString();
                }
                Parent.FormNew(this, FormName);
                MDI.Add(OpenForm);
                OpenForm.Show();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Connection.GlobalException(Ex));
            }
            finally
            {
                Variable.Kode = null;
            }
        }

        private string DisplayFieldDelete(int TmpIndex)
        {
            string TmpReturn = "";
            for (int i = 0; i < Delete.Count(); i++)
            {
                TmpReturn = Delete[i] + " = " + dgvInquiry.Rows[TmpIndex].Cells[Delete[i].ToString()].Value.ToString() + "\n";
            }
            return TmpReturn;
        }

        private string DeleteQueryWhere(int TmpIndex)
        {
            string TmpReturn = "";
            for (int i = 0; i < Delete.Count(); i++)
            {
                TmpReturn = Delete[i] + " = '" + dgvInquiry.Rows[TmpIndex].Cells[Delete[i].ToString()].Value.ToString() + "'";
                if (i != (Delete.Count() - 1))
                {
                    TmpReturn = " and ";
                }
            }
            return TmpReturn;
        }

        private void PageRefresh()
        {
            if (txtPage.Text == "")
            {
                txtPage.Text = "0";
            }

            if (Convert.ToDecimal(txtPage.Text) > Convert.ToDecimal(lblPage.Text))
            {
                txtPage.Text = lblPage.Text;
                Limit1 = (Convert.ToInt32(txtPage.Text) - 1) * Convert.ToInt32(cmbShow.Text) + 1;
                Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text);
                RefreshDataGrid();
            }
            else
            {
                Limit1 = (Convert.ToInt32(txtPage.Text) - 1) * Convert.ToInt32(cmbShow.Text) + 1;
                Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text);
                RefreshDataGrid();
            }
        }
        #endregion

        #region ACTION BUTTON PADA FORM
        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text.Trim()) - 1) * Convert.ToInt32(cmbShow.Text.Trim()) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text.Trim()) * Convert.ToInt32(cmbShow.Text.Trim());
            RefreshDataGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Convert.ToInt32(cmbShow.Text) >= 1)
            {
                Limit1 -= Convert.ToInt32(cmbShow.Text);
                Limit2 -= Convert.ToInt32(cmbShow.Text);
                txtPage.Text = (Convert.ToInt32(txtPage.Text) - 1).ToString();
            }
            RefreshDataGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Convert.ToInt32(cmbShow.Text) <= Total)
            {
                Limit1 += Convert.ToInt32(cmbShow.Text);
                Limit2 += Convert.ToInt32(cmbShow.Text);
                txtPage.Text = (Convert.ToInt32(txtPage.Text) + 1).ToString();
            }
            RefreshDataGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = ((int)Math.Ceiling(Total / Convert.ToDecimal(cmbShow.Text))).ToString();

            Limit1 = (Convert.ToInt32(txtPage.Text) - 1) * Convert.ToInt32(cmbShow.Text) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text);
            RefreshDataGrid();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvInquiry.RowCount > 0)
            {
                SelectData();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Convert.ToInt32(txtPage.Text.Trim()) - 1) * Convert.ToInt32(cmbShow.Text.Trim()) + 1;
            Limit2 = Convert.ToInt32(txtPage.Text.Trim()) * Convert.ToInt32(cmbShow.Text.Trim());
            if (RefreshGridStatus == true)
                PageRefresh();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshDataGrid();
        }

        private void dgvInquiry_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvInquiry.RowCount > 0)
            {
                SelectData();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Parent.FormNew(this, FormName);
            MDI.Add(OpenForm);
            OpenForm.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInquiry.RowCount > 0)
                {
                    int Index = dgvInquiry.CurrentRow.Index;
                    DialogResult dr = MessageBox.Show(DisplayFieldDelete(Index) + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Query = "Delete From " + Table + " Where " + DeleteQueryWhere(Index);
                        using (Method C = new Method())
                        {
                            C.ExecuteNonQuery(Query);
                            C.Dispose();
                        }

                        if (Table == "NotaDebetH" || Table =="NotaCreditH")
                        {
                            if (Table == "NotaDebetH")
                            {
                                Query = "DELETE From NotaDebet_Dtl WHERE " + DeleteQueryWhere(Index);
                            }
                            else if (Table == "NotaCreditH")
                            {
                                Query = "DELETE From NotaCredit_Dtl WHERE " + DeleteQueryWhere(Index);
                            }
                            using (Method C = new Method())
                            {
                                C.ExecuteNonQuery(Query);
                                C.Dispose();
                            }
                        }

                        MessageBox.Show(DisplayFieldDelete(dgvInquiry.CurrentRow.Index) + "Data berhasil dihapus.");
                        RefreshDataGrid();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { }
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbFilter.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtFilter.Text = "";
            cmbFilter.SelectedIndex = 0;
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;
        }

        private void InquiryV1_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < MDI.Count(); i++)
            {
                MDI[i].Close();
            }
        }

        private void txtPage_Leave(object sender, EventArgs e)
        {
            PageRefresh();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)13)
            {
                PageRefresh();
            }
        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                RefreshDataGrid();
            }
        }
        #endregion

        #region PERMISSION ACCESS
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(InquiryName, Authority);
        }
        #endregion

        private void InquiryV1_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        public void visibleBtn()
        {
            btnBatal.Visible = true;
            btnGunakan.Visible = true;
            btnDelete.Visible = false;
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInquiry.RowCount > 0)
                {
                    if (dgvInquiry.Rows[dgvInquiry.CurrentRow.Index].Cells["Deskripsi"].Value.ToString().ToUpper() == "CREATED")
                    {
                        
                        using (TransactionScope scope = new TransactionScope())
                        {
                            SqlConnection Conn = ConnectionString.GetConnection();
                            int Index = dgvInquiry.CurrentRow.Index;
                            DialogResult dr = MessageBox.Show(DisplayFieldDelete(Index) + "Apakah data diatas akan dibatalkan ?", "Konfirmasi", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                Query = "UPDATE " + Table + " SET [TransStatus]='XX',UpdatedDate = getdate(), UpdatedBy = '"+ControlMgr.UserId+"' Where " + DeleteQueryWhere(Index);
                                using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                                {
                                    Cmd.ExecuteNonQuery();
                                }

                                if (CrtJournal == true)
                                {
                                    if (Table == "NotaCreditH")
                                    {
                                        string CNNo = dgvInquiry.Rows[Index].Cells[PrimaryKey.ToString()].Value.ToString();
                                    }
                                    else if (Table == "NotaDebetH")
                                    {

                                    }

                                }

                                MessageBox.Show(DisplayFieldDelete(dgvInquiry.CurrentRow.Index) + "Data berhasil dibatalkan.");
                                RefreshDataGrid();
                            }
                            Conn.Close();
                            scope.Complete();
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("Tidak dapat dibatalakan karena status bukan Created.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { }
        }

        private void btnGunakan_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInquiry.RowCount > 0)
                {
                    if (dgvInquiry.Rows[dgvInquiry.CurrentRow.Index].Cells["Deskripsi"].Value.ToString().ToUpper() == "DELETED")
                    {
                        int Index = dgvInquiry.CurrentRow.Index;
                        //DialogResult dr = MessageBox.Show(DisplayFieldDelete(Index) + "Apakah data diatas akan digunakan kembali ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        //if (dr == DialogResult.Yes)
                        //{
                        //    Query = "UPDATE " + Table + " SET [TransStatus]='01' Where " + DeleteQueryWhere(Index);
                        //    using (SqlCommand Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        //    {
                        //        Cmd.ExecuteNonQuery();
                        //    }

                        //    MessageBox.Show(DisplayFieldDelete(dgvInquiry.CurrentRow.Index) + "Data berhasil dikembalikan.");
                        //    RefreshDataGrid();
                        //}

                        if (CrtJournal == true)
                        {
                            if (Table == "NotaCreditH")
                            {

                            }
                            else if (Table == "NotaDebetH")
                            {

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tidak dapat digunakan karena status bukan Deleted.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { }
        }

        private void btnApproval_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                if (dgvInquiry.RowCount > 0)
                {
                    SelectData();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }
    }
}
