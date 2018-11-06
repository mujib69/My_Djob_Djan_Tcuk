using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Site
{
    public partial class FormSite : MetroFramework.Forms.MetroForm
    {
        //SQL Function
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private int Index;

        string vOldSiteName, vOldCmbSiteType, vOldSiteLocation, vOldDeskripsi, vOldLuas, vOldAlamat1, vOldAlamat2, vOldCity, vOldArea_Code, vOldRT, vOldRW, vOldProvince;

        Master.Site.InquirySite Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormSite()
        {
            InitializeComponent();
        }

        //Mode
        string Mode = "";

        public void ModeNew()
        {
            Mode = "New";

            txtInventSiteId.Enabled = true;
            txtInventSiteName.Enabled = true;
            CmbSiteType.Enabled = true;
            txtLokasi.Enabled = true;
            txtDeskripsi.Enabled = true;
            txtLuas.Enabled = true;
            txtJumlahBlok.Enabled = true;
            txtAlamat1.Enabled = true;
            txtAlamat2.Enabled = true;
            txtCity.Enabled = false;
            txtArea_Code.Enabled = true;
            txtRT.Enabled = true;
            txtRW.Enabled = true;
            txtProvince.Enabled = false;

            btntxttblCustomer_Kota.Enabled = true;
            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtInventSiteId.Enabled = false;
            txtInventSiteName.Enabled = true;
            CmbSiteType.Enabled = true;
            txtLokasi.Enabled = true;
            txtDeskripsi.Enabled = true;
            txtLuas.Enabled = true;
            txtJumlahBlok.Enabled = true;
            txtAlamat1.Enabled = true;
            txtAlamat2.Enabled = true;
            txtCity.Enabled = false;
            txtArea_Code.Enabled = true;
            txtRT.Enabled = true;
            txtRW.Enabled = true;
            txtProvince.Enabled = false;

            btntxttblCustomer_Kota.Enabled = true;
            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            txtInventSiteId.Enabled = false;
            txtInventSiteName.Enabled = false;
            CmbSiteType.Enabled = false;
            txtLokasi.Enabled = false;
            txtDeskripsi.Enabled = false;
            txtLuas.Enabled = false;
            txtJumlahBlok.Enabled = false;
            txtAlamat1.Enabled = false;
            txtAlamat2.Enabled = false;
            txtCity.Enabled = false;
            txtArea_Code.Enabled = false;
            txtRT.Enabled = false;
            txtRW.Enabled = false;
            txtProvince.Enabled = false;

            btntxttblCustomer_Kota.Enabled = false;
            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end  
            vOldSiteName = txtInventSiteName.Text;
            vOldCmbSiteType = CmbSiteType.SelectedItem.ToString();
            vOldSiteLocation = txtLokasi.Text;
            vOldDeskripsi = txtDeskripsi.Text;
            vOldLuas = txtLuas.Text;

            vOldAlamat1 = txtAlamat1.Text;
            vOldAlamat2 = txtAlamat2.Text;
            vOldCity = txtCity.Text;
            vOldArea_Code = txtArea_Code.Text; 
            vOldRT = txtRT.Text;
            vOldRW = txtRW.Text;
            vOldProvince = txtProvince.Text;
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();

            txtInventSiteName.Text = vOldSiteName;
            CmbSiteType.SelectedItem = vOldCmbSiteType;
            txtLokasi.Text = vOldSiteLocation;
            txtDeskripsi.Text = vOldDeskripsi;
            txtLuas.Text = vOldLuas;

            txtAlamat1.Text = vOldAlamat1;
            txtAlamat2.Text = vOldAlamat2;
            txtCity.Text = vOldCity;
            txtArea_Code.Text = vOldArea_Code;
            txtRT.Text = vOldRT;
            txtRW.Text = vOldRW;
            txtProvince.Text = vOldProvince;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Cek Data apakah sudah ada
                string CekData;
                Conn = ConnectionString.GetConnection();

                //Validasi jika kosong
                if (txtInventSiteId.Text == "")
                {
                    MessageBox.Show("Data Site ID tidak boleh kosong.");
                    return;
                }

                Query = "Select InventSiteID from InventSite where InventSiteID=@txtInventSiteId ";

                using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                {
                    Command.Parameters.AddWithValue("@txtInventSiteId", txtInventSiteId.Text.Trim().ToUpper());
                    CekData = Command.ExecuteScalar() == null ? "" : Command.ExecuteScalar().ToString();
                }

                if (CekData != "" && Mode != "Edit")
                {
                    MessageBox.Show("InventSiteId " + txtInventSiteId.Text.Trim().ToUpper() + " sudah digunakan, silahkan diganti dengan yang lain.");
                    Conn.Close();
                    return;
                }

                //Jika New
                if (Mode=="New")
                {
                    Query = "Insert into dbo.InventSite (InventSiteId, InventSiteName, SiteType, Lokasi, Alamat1, Alamat2, Province, Kota, Area_Code, RT, RW, Deskripsi, Luas, CreatedDate, CreatedBy) values ";
                    Query += "(@txtInventSiteId,";
                    Query += "@txtInventSiteName,";
                    Query += "'" + CmbSiteType.SelectedItem.ToString() + "',";
                    Query += "@txtLokasi,";
                    
                    Query += "@txtAlamat1 ,";
                    Query += "@txtAlamat2 ,";
                    Query += "@txtProvince ,";
                    Query += "@txtCity ,";
                    Query += "@txtArea_Code ,";
                    Query += "@txtRT ,";
                    Query += "@txtRW ,";

                    Query += "@txtDeskripsi ,";
                    Query += "@txtLuas ,";
                    Query += "getdate(),'" + ControlMgr.UserId + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        
                        Command.Parameters.AddWithValue("@txtInventSiteId", txtInventSiteId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtInventSiteName", txtInventSiteName.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtLokasi", txtLokasi.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtAlamat1", txtAlamat1.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtAlamat2", txtAlamat2.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtProvince", txtProvince.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtCity", txtCity.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtArea_Code", txtArea_Code.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtRT", txtRT.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtRW", txtRW.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtLuas", txtLuas.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("InventSiteId = " + txtInventSiteId.Text.Trim().ToUpper() + Environment.NewLine + "InventSiteName = " + txtInventSiteName.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventSite set ";
                    Query += "InventSiteName=@txtInventSiteName ,";
                    Query += "SiteType='" + CmbSiteType.SelectedItem.ToString() + "',";
                    Query += "Lokasi = @txtLokasi ,";
                    Query += "Alamat1 = @txtAlamat1 ,";
                    Query += "Alamat2 = @txtAlamat2 ,";
                    Query += "Kota = @txtCity ,";
                    Query += "Area_Code = @txtArea_Code ,";
                    Query += "RT = @txtRT ,";
                    Query += "RW = @txtRW ,";
                    Query += "Province = @txtProvince ,";

                    Query += "Deskripsi = @txtDeskripsi ,";
                    Query += "Luas = @txtLuas ,";
                    Query += "UpdatedDate = getdate(), UpdatedBy='" + ControlMgr.UserId + "'";
                    Query += "where InventSiteId=@txtInventSiteId ";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtInventSiteId", txtInventSiteId.Text);
                        Command.Parameters.AddWithValue("@txtInventSiteName", txtInventSiteName.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtLokasi", txtLokasi.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtAlamat1", txtAlamat1.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtAlamat2", txtAlamat2.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtProvince", txtProvince.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtCity", txtCity.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtArea_Code", txtArea_Code.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtRT", txtRT.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtRW", txtRW.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtLuas", txtLuas.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("InventSiteId = " + txtInventSiteId.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diupdate.");
                }
                Conn.Close();
                this.Close();
                
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }  
        }

        public void SetParent(Master.Site.InquirySite F)
        {
            Parent = F;
        }

        public void GetDataHeader(string InventSiteId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select InventSiteId, InventSiteName, SiteType, Lokasi, Alamat1, Alamat2, Kota, Area_Code, RT, RW, Province, Deskripsi, Luas, JumlahBlok  From [dbo].[InventSite] where InventSiteId=@InventSiteId ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@InventSiteId", InventSiteId);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtInventSiteId.Text = Dr["InventSiteId"].ToString();
                txtInventSiteName.Text = Dr["InventSiteName"].ToString();
                CmbSiteType.SelectedItem = Dr["SiteType"].ToString();
                txtLokasi.Text = Dr["Lokasi"].ToString();

                txtAlamat1.Text = Dr["Alamat1"].ToString();
                txtAlamat2.Text = Dr["Alamat2"].ToString();
                txtCity.Text = Dr["Kota"].ToString();
                txtArea_Code.Text = Dr["Area_Code"].ToString();
                txtRT.Text = Dr["RT"].ToString();
                txtRW.Text = Dr["RW"].ToString();
                txtProvince.Text = Dr["Province"].ToString();                
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                txtLuas.Text = Dr["Luas"].ToString();
                txtJumlahBlok.Text = Dr["JumlahBlok"].ToString();
            }
            Dr.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMeasurement_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshGrid();
        }

        private void FormMeasurement_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }       

        private void txtJumlahBlok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) )
            {
                e.Handled = true;
            }
        }

        private void btntxttblCustomer_Kota_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "Kota";

            Search tmpSearchCity = new Search();
            tmpSearchCity.SetSchemaTable(SchemaName, TableName);
            tmpSearchCity.Text = "Search City";
            tmpSearchCity.ShowDialog();
            if (ConnectionString.Kode2 != "")
            {
                txtCity.Text = ConnectionString.Kode2;
                txtProvince.Text = ConnectionString.Kode;
            }
            ConnectionString.Kode2 = "";
            ConnectionString.Kode = "";
        }               
    }
}
