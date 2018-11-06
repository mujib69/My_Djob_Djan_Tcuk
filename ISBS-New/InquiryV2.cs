using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ISBS_New
{
    public partial class InquiryV2 : MetroFramework.Forms.MetroForm
    {
        #region INISIASI AWAL
        //PENANDA UNTUK DATAGRID
        private int Limit1, Limit2, Total, Page1, Page2;

        //BERHUBUNGAN DENGAN SQL
        public string Query;
        public string Table;
        public string Order;
        public string Fields;
        private string Where;

        //UNTUK MEMANGGIL QUERY
        public string PrimaryKey;
        public string[] Data;
        public string[] FilterDate;
        public string[] FilterText;
        public string WherePlus;

        //UNTUK MENGEMBALIKAN NILAI YANG DISELECT
        public string[] Select;

        //UNTUK MENAMPILKAN PARAMETER YANG INGIN DITAMPILKAN SAAT DELETE
        public string[] Delete;

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
        public string FormName = "";
        public string Notes = "";
        public string[] Status;
        public string[] OnProgress;
        public string[] Approval;
        public string[] Completed;
        public string[] Rejected;
        public string[] Draft;
        public string InquiryName = "";

        //TabPages & Where
        public string[] TabPageName;
        public string[] WhereTabPage;
        int TmpClick;
        //TabPages & Where

        //OuterField, pasti masuk filter
        public string[] Data2;
        //======================

        #region PERMISSION ACCESS
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(InquiryName, Authority);
        }
        #endregion

        public InquiryV2()
        {
            InitializeComponent();
            cmbShowLoad();
            ToolTipNotes.SetToolTip(this.pictureBox1, Notes);
        }

        private void InquiryV2_Load(object sender, EventArgs e)
        {
            Location = new Point(100, 48);
            LoadCmbFilter();
            RefreshDataGrid();
            ToolTipNotes.SetToolTip(this.pictureBox1, Notes);
            AddTabPage();
        }
        #endregion

        #region METHOD UNTUK MENAMPILKAN DATAGRID, CMBSHOW, REFRESH, DSB
        //MENAMPILKAN CMBSHOW
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
                    if (dtFrom.Enabled == true)
                    {
                        Query = "Select * from (Select ROW_NUMBER() OVER (ORDER BY " + PrimaryKey + " desc) No, " + Fields + " From " + Table + " Where 1=1 and (" + cmbFilter.Text + " >= '" + dtFrom.Value.ToShortDateString() + "' and " + cmbFilter.Text + " <='" + dtTo.Value.ToShortDateString() + "') and " + GetWhere() + " ) a " + Order;
                        Query += "Where No Between " + Limit1 + " and " + Limit2 + " ";
                    }
                    else
                    {
                        Query = "Select * from (Select ROW_NUMBER() OVER (ORDER BY " + PrimaryKey + " desc) No, " + Fields + " From " + Table + " Where 1=1 and (" + GetWhere() + ") ) a " + Order;
                        Query += "Where No Between " + Limit1 + " and " + Limit2 + " ";
                        
                    }
                    if (Data2 != null && Data2.Contains(cmbFilter.Text))
                    {
                        Query += " AND " + cmbFilter.Text + " like '%" + txtFilter.Text + "%' ";
                    }
                    Query += ";";
                    dgvInquiry.AutoGenerateColumns = true;
                    dgvInquiry.DataSource = C.DgvAutoQuery(Query);
                    dgvInquiry.Refresh();
                    dgvInquiry.AutoResizeColumns();
                    GetTotalPaging();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Connection.GlobalException(Ex));
            }
        }

        //MENDAPATKAN JUMLAH TOTAL PAGING
        private void GetTotalPaging()
        {
            try
            {
                //Mengambil nilai total paging
                Method C = new Method ();
                Query = "Select Count(" + PrimaryKey + ") From " + Table + " Where 1=1 ";
                Query += " and ( " + GetWhere() + " )";

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
            if (cmbFilter.Text.Trim() != "" && dtFrom.Enabled != true)
            {
                if (Data2 != null)
                {
                    if (Data2.Contains(cmbFilter.Text))
                    {
                        Where = "1=1";
                    }
                    else
                    {
                        Where = cmbFilter.Text + " like '%" + txtFilter.Text.Replace("'", "''") + "%' ";
                    }
                }
                
            }
            else if (cmbFilter.Text.Trim() == "" || dtFrom.Enabled == true)
            {
                Where = "";
                for (int i = 0; i < FilterText.Count(); i++)
                {
                    if (i != 0)
                        Where += "or ";
                    Where +=  FilterText[i] + " like '%" + txtFilter.Text.Replace("'", "''") + "%' ";
                }
            }
            return "(" + WhereTabPage[TmpClick] + ") and (" + Where + WherePlus + ")" ;
        }


        private void LoadCmbFilter()
        {
            try
            {
                for (int i = 0; i < FilterText.Count(); i++)
                {
                    cmbFilter.Items.Add(FilterText[i]);
                }
                if (Data2 != null)
                {
                    for (int i = 0; i < Data2.Count(); i++)
                    {
                        cmbFilter.Items.Add(Data2[i]);
                    }
                }
                RefreshGridStatus = true;
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


        private void AddTabPage()
        {
            for (int i = 0; i<TabPageName.Length; i++)
            {
                TabPage TmpTabPage = new TabPage(TabPageName[i].ToString());
                tabDgv.TabPages.Add(TmpTabPage);

               
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
                lblPage.Text = "1";
                txtPage.Text = lblPage.Text;
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
            PageRefresh();
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
                        using (Method C = new Method ())
                        {
                            C.ExecuteNonQuery(Query);
                            C.Dispose();
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
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < FilterDate.Length; i++)
            {
                if (cmbFilter.Text.Contains(FilterDate[i]))
                {
                    dtFrom.Enabled = true;
                    dtTo.Enabled = true;
                    return;
                }
                else
                {
                    dtFrom.Enabled = false;
                    dtTo.Enabled = false;
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtFilter.Text = "";
            cmbFilter.SelectedIndex = 0;
            dtFrom.Value = DateTime.Now;
            dtTo.Value = DateTime.Now;
            btnSearch.PerformClick();
        }

        private void InquiryV2_FormClosed(object sender, FormClosedEventArgs e)
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
                btnSearch.PerformClick();
            }
        }
        #endregion

        private void tabDgv_Click(object sender, EventArgs e)
        {
            TmpClick= tabDgv.SelectedIndex;
            RefreshDataGrid();
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (dgvInquiry.CurrentCell.ColumnIndex > -1 && dgvInquiry.CurrentCell.RowIndex > -1)
            {
                Parent.PreviewNew(this, FormName);
                MDI.Add(OpenForm);
                OpenForm.Show();
            }
        }

        
    }
}
