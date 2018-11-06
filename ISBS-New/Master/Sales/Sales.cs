using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Master.Sales
{
    public partial class Sales : Form
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Index, Total, Page1, Page2;
        int flagRefresh = 0;
        String KodeSls;

        //Master.Sales.InqSales Parent;

        //begin
        //created by : joshua
        //created date : 01 mar 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //public void SetParent(Master.Sales.InqSales F)
        //{
        //    Parent = F;
        //}

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            KodeSls = id;
            txtKode.Text = id;
        }

        public Sales()
        {
            InitializeComponent(); 
        }

        private void Sales_Load(object sender, EventArgs e)
        {
            GetDataHeader();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
        }

        private void Sales_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void Sales_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Master.Sales.InqSales f = new Master.Sales.InqSales();
            //f.RefreshGrid();
        }

        private void resetForm()
        {
            txtKode.Text = "";
            txtNama.Text = "";
            txtAlamat.Text = "";
            txtTelepon.Text = "";
            txtHandphone.Text = "";
            txtPersen.Text = "";
            txtGroup.Text = "";
            txtKategori.Text = "";
            rdTokoProyek1.Checked = false;
            rdTokoProyek2.Checked = false;
            rdTokoProyek3.Checked = false;
            ckKunci.Checked = false;
            dgvUser.Rows.Clear();
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void ModeNew()
        {
            resetForm();  
            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnAdd.Enabled = true;
            btnDelete.Enabled = true;

            btnGroup.Enabled = true;

            txtKode.Enabled = false;
            txtNama.Enabled = true;
            txtAlamat.Enabled = true;
            txtTelepon.Enabled = true;
            txtHandphone.Enabled = true;
            txtPersen.Enabled = true;
            txtGroup.Enabled = true;
            txtKategori.Enabled = true;
            rdTokoProyek1.Enabled = true;
            rdTokoProyek2.Enabled = true;
            rdTokoProyek3.Enabled = true;
            ckKunci.Enabled = true;

            dgvUser.ReadOnly = false;
            dgvUser.Columns["No"].ReadOnly = true;
            dgvUser.Columns["UserID"].ReadOnly = true;

            dgvUser.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvUser.Columns["UserID"].SortMode = DataGridViewColumnSortMode.NotSortable;


            dgvUser.AutoResizeColumns();
            dgvUser.DefaultCellStyle.BackColor = Color.White;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnGroup.Enabled = false;

            txtKode.Enabled = false;
            txtNama.Enabled = false;
            txtAlamat.Enabled = false;
            txtTelepon.Enabled = false;
            txtHandphone.Enabled = false;
            txtPersen.Enabled = false;
            txtGroup.Enabled = false;
            txtKategori.Enabled = false;
            rdTokoProyek1.Enabled = false;
            rdTokoProyek2.Enabled = false;
            rdTokoProyek3.Enabled = false;
            ckKunci.Enabled = false;

            dgvUser.ReadOnly = true;
            BeforeEditColor();
            dgvUser.DefaultCellStyle.BackColor = Color.LightGray;
        }

        private void BeforeEditColor()
        {
            for (int i = 0; i < dgvUser.RowCount; i++)
            {
                dgvUser.Rows[i].Cells["UserID"].Style.BackColor = Color.LightGray;
            }
        }
       
        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvUser.RowCount; i++)
            {
                dgvUser.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        //public void SetMode(string tmpMode, string tmpKodeSls)
        //{
        //    Mode = tmpMode;
        //    KodeSls = tmpKodeSls;
        //    txtKode.Text = tmpKodeSls;
        //}

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void GetDataHeader()
        {
            try
            {
                if (KodeSls == "")
                {
                    KodeSls = txtKode.Text.Trim();
                }
                Conn = ConnectionString.GetConnection();

                Query = "Select Kode_Sls, Nama_Sls, Alamat, Telepon, HP, Persen, [Group_Brg], Toko_Proyek, Kategori, IkatUserID From [Sales]  ";
                Query += "Where Kode_Sls = @kodesls";

                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@kodesls", KodeSls);
                Dr = Cmd.ExecuteReader();

                string TokoProyek = "";
                string IkatUserID = "";
                while (Dr.Read())
                {
                    txtKode.Text = Dr["Kode_Sls"].ToString();
                    txtNama.Text = Dr["Nama_Sls"].ToString();
                    txtAlamat.Text = Dr["Alamat"].ToString();
                    txtTelepon.Text = Dr["Telepon"].ToString();
                    txtHandphone.Text = Dr["HP"].ToString();
                    txtPersen.Text = Convert.ToString(Convert.ToInt32(Dr["Persen"]));
                    txtGroup.Text = Dr["Group_Brg"].ToString();
                    TokoProyek = Dr["Toko_Proyek"].ToString();
                    IkatUserID = Dr["IkatUserID"].ToString();
                    txtKategori.Text = Dr["Kategori"].ToString();
                }
                Dr.Close();

                if (IkatUserID == "False" || IkatUserID == "")
                {
                    ckKunci.Checked = false;
                }
                else {
                    ckKunci.Checked = true;
                }

                if (TokoProyek.ToUpper() == "TOKO")
                {
                    rdTokoProyek1.Checked = true;
                    rdTokoProyek2.Checked = false;
                    rdTokoProyek3.Checked = false;
                }
                else if (TokoProyek.ToUpper() == "PROYEK")
                {
                    rdTokoProyek1.Checked = false;
                    rdTokoProyek2.Checked = true;
                    rdTokoProyek3.Checked = false;
                }
                else if (TokoProyek.ToUpper() == "LAIN")
                {
                    rdTokoProyek1.Checked = false;
                    rdTokoProyek2.Checked = false;
                    rdTokoProyek3.Checked = true;
                }

                Conn = ConnectionString.GetConnection();

                Query = "Select Deskripsi From [InventGroup] Where GroupID = @groupid ";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@groupid", txtGroup.Text);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtGroupName.Text = Dr["Deskripsi"].ToString();
                }
                Dr.Close();



                dgvUser.Rows.Clear();
                if (dgvUser.RowCount - 1 <= 0)
                {
                    dgvUser.ColumnCount = 2;
                    dgvUser.Columns[0].Name = "No";
                    dgvUser.Columns[1].Name = "UserID";
                }

                Query = "Select UserID From [ListKunci_Sales_UserId] Where Kode_Sls = @kodesls order by SeqNo asc";

                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@kodesls", KodeSls);

                Dr = Cmd.ExecuteReader();
                int i = 0;
                while (Dr.Read())
                {

                    this.dgvUser.Rows.Add(i + 1, Dr[0]);
                    i++;
                }
                Dr.Close();

                dgvUser.ReadOnly = false;
                dgvUser.Columns["No"].ReadOnly = true;
                dgvUser.Columns["UserID"].ReadOnly = true;

                dgvUser.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvUser.Columns["UserID"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvUser.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //foreach (TextBox tbValue in this.Controls.OfType<TextBox>())
            //{
            //    var value = tbValue.Text;
            //    MessageBox.Show(value.ToString());
            //}

            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            string TokoProyek = "";
            bool IkatUser = false;
            if (rdTokoProyek1.Checked)
            {
                TokoProyek = "TOKO";
            }
            else if (rdTokoProyek2.Checked)
            {
                TokoProyek = "PROYEK";
            }
            else if (rdTokoProyek3.Checked)
            {
                TokoProyek = "LAIN";
            }  

            if (ckKunci.Checked)
            {
                IkatUser = true;
            }

            if (txtKode.Text.Trim() == "")
            {
                MessageBox.Show("Kode Sales harus diisi");
                return;
            }

            else if (txtNama.Text.Trim() == "")
            {
                MessageBox.Show("Nama Sales harus diisi");
                return;
            }
            else if (checkExistingData(txtKode.Text.Trim(), Conn, Trans, Cmd) > 0 && Mode == "New")
            {
                MessageBox.Show("Kode Sales sudah ada di database");
                return;
            }

            if (txtPersen.Text == "")
            {
                txtPersen.Text = "0";
            }             
            else            
            {
                if (Convert.ToDecimal(txtPersen.Text) > 100)
                {
                    MessageBox.Show("Persen harus lebih kecil atau sama dengan 100");
                    return;
            
                }
            }

            if (ckKunci.Checked)
            {
                if (dgvUser.Rows.Count == 0)
                {
                    MessageBox.Show("User ID harus diisi");
                    return;
                }
            }

            try
            {                

                if (Mode == "New" ||txtKode.Text == "")
                {
                    int Persen;
                    if (txtPersen.Text == "")
                    {
                        Persen = 0;
                    }
                    else
                    {
                        Persen = Convert.ToInt32(txtPersen.Text);
                    }

                    Query = "Insert into Sales (Kode_Sls, Nama_Sls, Alamat, Telepon, HP, Persen, [Group_Brg], Toko_Proyek, Kategori, IkatUserID, CreatedBy, CreatedDate) values ";
                    Query += "(@kode, @nama, @alamat, @telepon, @handphone, @persen, @group, @tokoproyek, @kategori, @ikatuser, @UserId, getdate())";
                    Cmd = new SqlCommand(Query, Conn, Trans);

                    Cmd.Parameters.AddWithValue("@kode", txtKode.Text);
                    Cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    Cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                    Cmd.Parameters.AddWithValue("@telepon", txtTelepon.Text);
                    Cmd.Parameters.AddWithValue("@handphone", txtHandphone.Text);
                    Cmd.Parameters.AddWithValue("@persen", Persen);
                    Cmd.Parameters.AddWithValue("@group", txtGroup.Text);
                    Cmd.Parameters.AddWithValue("@tokoproyek", TokoProyek);
                    Cmd.Parameters.AddWithValue("@kategori", txtKategori.Text);
                    Cmd.Parameters.AddWithValue("@ikatuser", IkatUser);
                    Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);

                    Cmd.ExecuteNonQuery();

                    Query = "";
                    for (int i = 0; i <= dgvUser.RowCount - 1; i++)
                    {
                        Query += "Insert ListKunci_Sales_UserId (Kode_Sls, UserID, SeqNo) Values ";
                        Query += "('" + txtKode.Text + "','";
                        Query += (dgvUser.Rows[i].Cells["UserID"].Value == null ? "" : dgvUser.Rows[i].Cells["UserID"].Value.ToString()) + "','";
                        Query += "" + (i + 1) + "')";
                        //Query += "(@kode, @userid, @)";

                        if (i % 5 == 0 && i > 0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            //Cmd.Parameters.AddWithValue("@kode", txtKode.Text);
                            //Cmd.Parameters.AddWithValue("@userid", dgvUser.Rows[i].Cells["UserID"].Value == null ? "" : dgvUser.Rows[i].Cells["UserID"].Value.ToString());
                            //Cmd.Parameters.AddWithValue("@SeqNo", i + 1);

                            Cmd.ExecuteNonQuery();
                            Query = "";
                        }
                    }

                    //if (Query != "")
                    //{
                    //    Cmd = new SqlCommand(Query, Conn, Trans);
                    //    Cmd.ExecuteNonQuery();
                    //    Query = "";
                    //}

                    Trans.Commit();
                    MessageBox.Show("Data Kode Sales : " + txtKode.Text + " berhasil ditambahkan.");
                    //MainMenu f = new MainMenu();
                }
                else
                {
                    Query = "Update Sales set ";
                    Query += "Nama_Sls = @nama,";
                    Query += "Alamat = @alamat,";
                    Query += "Telepon = @telepon,";
                    Query += "HP = @handphone,";
                    Query += "Persen = @persen,";
                    Query += "Group_Brg = @group,";
                    Query += "Toko_Proyek = @tokoproyek,";
                    Query += "Kategori = @kategori,";
                    Query += "IkatUserID = @ikatuser,";
                    Query += "UpdatedBy = @UserId,";
                    Query += "UpdatedDate = getdate() ";
                    Query += "WHERE Kode_Sls = @kode";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@kode", txtKode.Text);
                    Cmd.Parameters.AddWithValue("@nama", txtNama.Text);
                    Cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                    Cmd.Parameters.AddWithValue("@telepon", txtTelepon.Text);
                    Cmd.Parameters.AddWithValue("@handphone", txtHandphone.Text);
                    Cmd.Parameters.AddWithValue("@persen", txtPersen.Text);
                    Cmd.Parameters.AddWithValue("@group", txtGroup.Text);
                    Cmd.Parameters.AddWithValue("@tokoproyek", TokoProyek);
                    Cmd.Parameters.AddWithValue("@kategori", txtKategori.Text);
                    Cmd.Parameters.AddWithValue("@ikatuser", IkatUser);
                    Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);

                    Cmd.ExecuteNonQuery();

                    Query = "Delete from ListKunci_Sales_UserId where Kode_Sls='" + txtKode.Text.Trim() + "';";
                    for (int i = 0; i <= dgvUser.RowCount - 1; i++)
                    {
                        Query += "Insert ListKunci_Sales_UserId (Kode_Sls, UserID, SeqNo) Values ";
                        Query += "('" + txtKode.Text + "','";
                        Query += (dgvUser.Rows[i].Cells["UserID"].Value == null ? "" : dgvUser.Rows[i].Cells["UserID"].Value.ToString()) + "','";
                        Query += "" + (i + 1) + "')";

                        if (i % 5 == 0 && i > 0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            Query = "";
                        }
                    }
                    //if (Query != "")
                    //{
                    //    Cmd = new SqlCommand(Query, Conn, Trans);
                    //    Cmd.ExecuteNonQuery();
                    //}

                    Trans.Commit();
                    MessageBox.Show("Data Kode Sales : " + txtKode.Text + " berhasil diupdate.");

                }
            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
                
                //Master.Sales.InqSales f = new Master.Sales.InqSales();
                //f.RefreshGrid();
                
            }
            GetDataHeader();
            ModeBeforeEdit();

            Form f = Application.OpenForms["GlobalInquiry"];
            if (f != null)
                if (f.Text == "Sales")
                    Parent.RefreshGrid();
        }

        private void Persen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private int checkExistingData(string Kode, SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            int result = 0;
            Query = "SELECT COUNT(Kode_Sls) CountData FROM Sales WHERE UPPER(Kode_Sls) = @kode";
           
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.Parameters.AddWithValue("@kode", Kode.ToUpper());

            Dr = Cmd.ExecuteReader();

            string BankGuarantee = "";
            if (Dr.Read())
            {
                result = Convert.ToInt32(Dr["CountData"]);
            }
            Dr.Close();

            return result;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Mode = "Edit";
                btnSave.Visible = true;
                btnExit.Visible = false;
                btnEdit.Visible = false;
                btnCancel.Visible = true;
                btnGroup.Enabled = true;

                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end   
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            ModeBeforeEdit();
            GetDataHeader();
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "GroupID";
            tmpSearch.Order = "GroupId Asc";
            tmpSearch.Table = "[dbo].[InventGroup]";
            tmpSearch.QuerySearch = "SELECT a.GroupID, a.Deskripsi FROM [dbo].[InventGroup] a";
            tmpSearch.FilterText = new string[] { "GroupID", "Deskripsi" };
            tmpSearch.Select = new string[] { "GroupID", "Deskripsi" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtGroup.Text = ConnectionString.Kodes[0];
                txtGroupName.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PopUp.User.User Us = new PopUp.User.User();
            List<PopUp.User.User> ListUser = new List<PopUp.User.User>();
            Us.ParentRefreshGrid(this);
            Us.ParamHeader(dgvUser);
            Us.ShowDialog();
            EditColorUser();
        }

        public void AddDataGridUser(List<string> UserID)
        {
            if (dgvUser.RowCount - 1 <= 0)
            {
                dgvUser.ColumnCount = 2;
                dgvUser.Columns[0].Name = "No";
                dgvUser.Columns[1].Name = "UserID";
            }

            for (int i = 0; i < UserID.Count; i++)
            {
                this.dgvUser.Rows.Add((dgvUser.RowCount + 1).ToString(), UserID[i]);
            }

            dgvUser.ReadOnly = false;
            dgvUser.Columns["No"].ReadOnly = true;
            dgvUser.Columns["UserID"].ReadOnly = true;

            dgvUser.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvUser.Columns["UserID"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvUser.AutoResizeColumns();
        }

        private void EditColorUser()
        {
            for (int i = 0; i < dgvUser.RowCount; i++)
            {

                dgvUser.Rows[i].Cells["UserID"].Style.BackColor = Color.White;
            }
        }

        private void KunciUser_Click(object sender, EventArgs e)
        {
            if (ckKunci.Checked)
            {
                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = false;
                btnDelete.Enabled = false;
                dgvUser.Rows.Clear();
            }  
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUser.RowCount > 0)
            {
                Index = dgvUser.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvUser.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " UserID = " + dgvUser.Rows[Index].Cells["UserID"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvUser.Rows.RemoveAt(Index);
                    SortNoDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        private void txtTelepon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtHandphone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


    }    
}
