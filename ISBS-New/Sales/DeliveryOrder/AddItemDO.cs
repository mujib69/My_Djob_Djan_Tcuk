using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New
{
    public partial class AddItemDO : MetroFramework.Forms.MetroForm
    {
        #region INISIASI AWAL
        //PENANDA UNTUK DATAGRID
        private int Limit1, Limit2, Total, Page1, Page2;

        //BERHUBUNGAN DENGAN SQL
        SqlConnection Conn;
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
        public string[] HideField;
        public string WherePlus;

        //UNTUK MENGEMBALIKAN NILAI YANG DISELECT
        public string[] Select;
        public string[] Select2;

        //UNTUK MENAMPILKAN PARAMETER YANG INGIN DITAMPILKAN SAAT DELETE
        public string[] Delete;

        //MASKING
        public string[] Mask;

        //LUPA....
        //string IdNamePK = "";
        //string Type = "";
        //private Boolean tmpAmbilKodeNo = false;

        //UNTUK CMBLOAD AGAR TIDAK TERJADI ERROR KARENA DATA BELUM ADA DISQL
        private Boolean RefreshGridStatus = false;

        //DIGUNAKAN OLEH PARENT AGAR FORM TERDETEKSI SEBAGAI CHILD        
        public Form Parent = new Form();
        public string Notes = "";
        public string FormName = "Search Query";

        public AddItemDO()
        {
            InitializeComponent();
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Convert.ToInt32(cmbShow.Text);
        }

        private void SearchV1_Load(object sender, EventArgs e)
        {
            this.Text = FormName;
            this.Location = new Point(148, 47);
            LoadCmbFilter();
            if (Mask != null)
            {
                LoadCmbMask();
                cmbFilter1.Visible = false;
                cmbMask.Visible = true;
            }
            else
            {
                cmbFilter1.Visible = true;
                cmbMask.Visible = false;
            }
            RefreshDataGrid();
            ToolTipNotes.SetToolTip(this.pictureBox1, Notes);
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
                    C.Dr  = C.ReturnDr(Query);
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
        private void RefreshDataGrid()
        {
            try
            {
                //Menampilkan data
                using(Method C = new Method ())
                {
                    Query = "Select * from (Select ROW_NUMBER() OVER (ORDER BY " + PrimaryKey + " asc) No, * From (" + QuerySearch + " ) a " + GetWhere() + " ) a ";
                    Query += "Where No Between " + Limit1 + " and " + Limit2 + Order + " ;";

                    dgvSearch.AutoGenerateColumns = true;

                    DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                    checkColumn.Name = "Check";
                    checkColumn.HeaderText = "Check";
                    //checkColumn.Width = 100;
                    checkColumn.ReadOnly = false;
                    checkColumn.FillWeight = 100; //if the datagridview is resized (on form resize) the checkbox won't take up too much; value is relative to the other columns' fill values
                    
                    //edited by Thaddaeus, 23 April 2018, begin
                    dgvSearch.Columns.Clear();
                    //end=================================
                    
                    dgvSearch.Columns.Add(checkColumn);
                    dgvSearch.DataSource = C.DgvAutoQuery(Query);
                    
                    dgvSearch.Refresh();
                    dgvSearch.AutoResizeColumns();
                    C.DgvVisible(dgvSearch, HideField);
                    GetTotalPaging();
                    dgvSearch.ReadOnly = false;
                    C.DgvCheckReadOnly(dgvSearch);
                    //temporary if, prevent error buat yg belom di mask
                    if (Mask != null)
                    {
                        dgvHeaderText();
                    }
                }
                if (HideField != null)
                {
                    if (HideField.Contains("Check"))
                    {
                        chkAll.Visible = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
        }

        //MENDAPATKAN JUMLAH TOTAL PAGING
        private void GetTotalPaging()
        {
            try
            {
                //Mengambil nilai total paging
                Method C = new Method ();
                Query = "Select Count(" + PrimaryKey + ") From (" + QuerySearch + ") a ";
                Query += GetWhere();

                Total = Convert.ToInt32(C.ReturnScalar(Query));
                //Conn.Close();

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling(Total / Convert.ToDecimal(cmbShow.Text));
                lblPage.Text = Page2.ToString();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                //Conn.Close();
            }
        }

        //DIGUNAKAN UNTUK WHERE QUERY
        private string GetWhere()
        {
            Where = "Where 1=1 and ( ";
            if (cmbFilter1.Text.Trim() == "All")
            {
                for (int i = 0; i<FilterText.Count(); i++)
                {
                    Where += FilterText[i].ToString() + " like '%" + txtFilter1.Text.Replace("'", "''") + "%' ";
                    if (i < FilterText.Count() - 1)
                    {
                        Where += " or ";
                    }
                }
            }
            else
            {
                Where += cmbFilter1.Text.Trim() + " like '%" + txtFilter1.Text.Replace("'", "''") + "%' ";
            }
            Where += ")";
            return Where + WherePlus;
        }

        //MENAMPILKAN FIELD FILTER YANG BISA DIGUNAKAN
        private void LoadCmbFilter()
        {
            try
            {
                cmbFilter1.Items.Add("All");
                for (int i = 0; i < FilterText.Count(); i++)
                {
                    cmbFilter1.Items.Add(FilterText[i]);
                }
                cmbFilter1.SelectedIndex = 0;
                RefreshGridStatus = true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
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
                int x = 0;
                for (int i = 0; i < dgvSearch.RowCount; i++)
                {
                    if (Convert.ToBoolean(dgvSearch.Rows[i].Cells["Check"].Value) == true)
                    {
                        x++;
                    }
                }

                //take only 1 data, edited by Thaddaeus 20 April 2018 thad*
                //if the column check is hide
                if (HideField != null)
                {
                    if (HideField.Contains("Check"))
                    {
                        if (x == 0)
                        {
                            x = 1;
                        }
                    }
                }
                //============================================

                Variable.Kode2 = new string[x, Select.Count()];
                int m, n;
                m = 0;
                for (int h = 0; h < dgvSearch.RowCount; h++)
                {
                    if (Convert.ToBoolean(dgvSearch.Rows[h].Cells["Check"].Value) == true)
                    {
                        n = 0;
                        for (int i = 0; i < Select.Count(); i++)
                        {
                            Variable.Kode2[m, n] = dgvSearch.Rows[h].Cells[Select[i]].Value.ToString();
                            n++;
                        }
                        m++;
                    }
                    //continue edit Thaddaeus thad*
                    else if (HideField != null)
                    {
                        if (HideField.Contains("Check") && x == 1)
                        {
                            for (int i = 0; i < Select.Count(); i++)
                            {
                                if (dgvSearch.Focused == false)
                                {
                                    Variable.Kode2 = null;
                                }
                                else
                                {
                                    int tes = dgvSearch.CurrentRow.Index;
                                    Variable.Kode2[0, i] = dgvSearch.Rows[dgvSearch.CurrentRow.Index].Cells[Select[i]].Value.ToString();
                                }
                            }
                        }
                    }
                    //==================================
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally { }
        }

        //STV EDIT start 25/07/2018
        //DO, Create DO bisa tarik item lain selain di SO dengan ketentuan : 4 suku item nya sama.
        //UNTUK MEMILIH ITEM
        private void SelectItem()
        {
            //Sales.DeliveryOrder.DOHeader form = new ISBS_New.Sales.DeliveryOrder.DOHeader();
            //form.AdditionalItemOverview("a","b","c","d","e");

            try
            {
                int x = 0;
                for (int i = 0; i < dgvItem.RowCount; i++)
                {
                    if (Convert.ToBoolean(dgvItem.Rows[i].Cells["Check"].Value) == true)
                    {
                        x++;
                    }
                }

                //take only 1 data, edited by Thaddaeus 20 April 2018 thad*
                //if the column check is hide
                if (HideField != null)
                {
                    if (HideField.Contains("Check"))
                    {
                        if (x == 0)
                        {
                            x = 1;
                        }
                    }
                }
                //============================================

                Variable.Kode2a = new string[x, Select2.Count()];
                int m, n;
                m = 0;
                for (int h = 0; h < dgvItem.RowCount; h++)
                {
                    if (Convert.ToBoolean(dgvItem.Rows[h].Cells["Check"].Value) == true)
                    {
                        n = 0;
                        for (int i = 0; i < Select2.Count(); i++)
                        {
                            Variable.Kode2a[m, n] = dgvItem.Rows[h].Cells[Select2[i]].Value.ToString();
                            n++;
                        }
                        m++;
                    }
                    //continue edit Thaddaeus thad*
                    else if (HideField != null)
                    {
                        if (HideField.Contains("Check") && x == 1)
                        {
                            for (int i = 0; i < Select2.Count(); i++)
                            {
                                if (dgvItem.Focused == false)
                                {
                                    Variable.Kode2a = null;
                                }
                                else
                                {
                                    int tes = dgvItem.CurrentRow.Index;
                                    Variable.Kode2a[0, i] = dgvItem.Rows[dgvItem.CurrentRow.Index].Cells[Select2[i]].Value.ToString();
                                }
                            }
                        }
                    }
                    //==================================
                }
            }
            //catch (Exception Ex)
            //{
            //    MessageBox.Show(ConnectionString.GlobalException(Ex));
            //}
            finally { }
 
        }
        //STV EDIT END

        //TXTPAGE UNTUK MELAKUKAN REFRESH
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

        //SET PARENT
        public void SetParent(Form F)
        {
            Parent = F;
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
            if (dgvSearch.RowCount > 0)
            {
                SelectData();
                SelectItem();
            }
            this.Close();
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
        
        private void dgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (HideField != null)
            {
                if (HideField.Contains("Check"))
                {
                    if (dgvSearch.RowCount > 0)
                    {
                        SelectData();
                    }
                    this.Close();
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtFilter1.Text = "";
            cmbFilter1.SelectedIndex = 0;
            if (Mask != null)
            {
                cmbMask.SelectedIndex = 0;
            }
            RefreshDataGrid();
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
        #endregion

        private void txtFilter1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                RefreshDataGrid();
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDataGrid();
            Boolean Check = chkAll.Checked;
            for (int i = 0; i <= dgvSearch.RowCount - 1; i++)
            {
                dgvSearch.Rows[i].Cells["Check"].Value = Check;
            }
        }

        private void AddItemDO_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void cmbMask_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SelectedMask = cmbMask.SelectedIndex;

            cmbFilter1.SelectedIndex = SelectedMask;
        }

        public void LoadCmbMask()
        {
            Conn = ConnectionString.GetConnection();
            try
            {
                cmbMask.Items.Add("All");
                for (int i = 0; i < Mask.Count(); i++)
                {
                    cmbMask.Items.Add(Mask[i]);
                }
                cmbMask.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }

        public void dgvHeaderText()
        {
            Conn = ConnectionString.GetConnection();
            try
            {
                for (int i = 2; i < Mask.Count() + 2; i++)
                {
                    dgvSearch.Columns[i].HeaderText = Mask[i - 2];
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }

        //DO, Create DO bisa tarik item lain selain di SO dengan ketentuan : 4 suku item nya sama.
        string FullItemId;
        string GroupS1S2;

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                RefreshItem();
            }
        }

        private void RefreshItem()
        {
            dgvItem.Columns.Clear();
            //Query = "Select * from [dbo].[InventTable] where FullItemId Like '%" + GroupS1S2 + "%'";
            for (int i = 0; i < dgvSearch.RowCount; i++)
            {
                if (Convert.ToBoolean(dgvSearch.Rows[i].Cells["Check"].Value) == true)
                {
                    FullItemId = dgvSearch.Rows[i].Cells["FullItemId"].Value.ToString();
                    GroupS1S2 = FullItemId.Substring(0, 12);

                    try
                    {
                        //Menampilkan data
                        using (Method C = new Method())
                        {
                            Query = "Select FullItemId, ItemDeskripsi, GroupDeskripsi, SubGroup1Deskripsi, SubGroup2Deskripsi, MerekID, UoM, UomAlt, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy  from [dbo].[InventTable] where FullItemId Like '%" + GroupS1S2 + "%'";

                            dgvItem.AutoGenerateColumns = true;

                            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
                            checkColumn.Name = "Check";
                            checkColumn.HeaderText = "Check";
                            checkColumn.ReadOnly = false;
                            checkColumn.FillWeight = 100;

                            dgvItem.Columns.Clear();
                            dgvItem.Columns.Add(checkColumn);
                            dgvItem.DataSource = C.DgvAutoQuery(Query);

                            dgvItem.Refresh();
                            dgvItem.AutoResizeColumns();
                            //C.DgvVisible(dgvItem, HideField);
                            GetTotalPaging();
                            dgvItem.ReadOnly = false;
                            C.DgvCheckReadOnly(dgvItem);
                            ////temporary if, prevent error buat yg belom di mask
                            //if (Mask != null)
                            //{
                            //    dgvHeaderText();
                            //}
                        }
                        if (HideField != null)
                        {
                            if (HideField.Contains("Check"))
                            {
                                chkAll.Visible = false;
                            }
                        }
                    }
                    //catch (Exception Ex)
                    //{
                    //    MessageBox.Show(ConnectionString.GlobalException(Ex));
                    //}
                    finally
                    {

                    }
                }
            }

        }

        private void grpFooter_Enter(object sender, EventArgs e)
        {

        }

        private void dgvSearch_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            

            
        }
        
    }
}
