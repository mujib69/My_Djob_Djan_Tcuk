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
    public partial class SiteBlok : MetroFramework.Forms.MetroForm
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

        string vOldSiteID, vOldSiteName, vOldSiteBlokID, vOldDeskripsi;

        Master.Site.SiteBlokInq Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

      
        public SiteBlok()
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
            txtInventSiteBlokId.Enabled = true;
            txtDeskripsi.Enabled = true;


            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtInventSiteId.Enabled = false;
            txtInventSiteName.Enabled = false;
            txtInventSiteBlokId.Enabled = false;
            txtDeskripsi.Enabled = true;

            btnSCust.Enabled = true;
            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;

            GetNamaGudang();
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            txtInventSiteId.Enabled = false;
            txtInventSiteName.Enabled = false;
            txtInventSiteBlokId.Enabled = false;
            txtDeskripsi.Enabled = false;

            btnSCust.Enabled = false;
            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;

            GetNamaGudang();
        }

        private void SiteBlok_Load(object sender, EventArgs e)
        {
            txtInventSiteId.Enabled = false;
            txtInventSiteName.Enabled = false;
            this.Location = new Point(148, 47);
        }

        private void btnSCust_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSite";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.Text = "Search Site Blok";
            tmpSearch.ShowDialog();
            if (ConnectionString.Kode != "")
            {
                txtInventSiteId.Text = ConnectionString.Kode;
                txtInventSiteName.Text = ConnectionString.Kode2;
            }
            ConnectionString.Kode = "";
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

                if (txtInventSiteName.Text == "")
                {
                    MessageBox.Show("Data Site Name tidak boleh kosong.");
                    return;
                }

                if (txtInventSiteBlokId.Text == "")
                {
                    MessageBox.Show("Data Site Blok tidak boleh kosong.");
                    return;
                }

                if (txtDeskripsi.Text == "")
                {
                    MessageBox.Show("Data Site Deskripsi tidak boleh kosong.");
                    return;
                }

                Query = "Select InventSiteBlokID from InventSiteBlok where InventSiteBlokID=@txtInventSiteBlokId";

                using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                {
                    Command.Parameters.AddWithValue("@txtInventSiteBlokId", txtInventSiteBlokId.Text.Trim().ToUpper());
                    CekData = Command.ExecuteScalar() == null ? "" : Command.ExecuteScalar().ToString();
                }

                if (CekData != "" && Mode != "Edit")
                {
                    MessageBox.Show("InventSiteBlokID " + txtInventSiteId.Text.Trim().ToUpper() + " sudah digunakan, silahkan diganti dengan yang lain.");
                    Conn.Close();
                    return;
                }

                //Jika New
                if (Mode == "New")
                {
                    Query = "Insert into dbo.InventSiteBlok (InventSiteId, InventSiteBlokId, Deskripsi, CreatedDate, CreatedBy) values ";
                    Query += "(@txtInventSiteId,@txtInventSiteBlokId,@txtDeskripsi,getdate(),'" + ControlMgr.UserId + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtInventSiteId", txtInventSiteId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtInventSiteBlokId", txtInventSiteBlokId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }

                    //Increase JumlahBlok in InventSite
                    Query = "Update dbo.InventSite set JumlahBlok = JumlahBlok + 1, UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where InventSiteId = @txtInventSiteId ";                   
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtInventSiteId",txtInventSiteId.Text);
                        Command.ExecuteScalar();
                    }

                    Trans.Commit();
                    MessageBox.Show("InventSiteId = " + txtInventSiteId.Text.Trim().ToUpper() + Environment.NewLine + "InventSiteName = " + txtInventSiteName.Text.Trim().ToUpper() + Environment.NewLine + "InventSiteBlok = " + txtInventSiteBlokId.Text.Trim().ToUpper() + Environment.NewLine + "InventSiteDeskripsi = " + txtDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode == "Edit")
                {

                    Query = "Update dbo.InventSiteBlok set InventSiteId=@txtInventSiteId, Deskripsi=@txtDeskripsi , UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where InventSiteBlokId=@txtInventSiteBlokId ";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtInventSiteId", txtInventSiteId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtInventSiteBlokId", txtInventSiteBlokId.Text);
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }

                    //Decrease JumlahBlok in InventSite based on old
                    Query = "Update dbo.InventSite set JumlahBlok = JumlahBlok - 1, UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where InventSiteId = @vOldSiteID ";
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@vOldSiteID", vOldSiteID);
                        Command.ExecuteScalar();
                    }

                    //Increase JumlahBlok in InventSite
                    Query = "Update dbo.InventSite set JumlahBlok = JumlahBlok + 1, UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where InventSiteId = @txtInventSiteId ";
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtInventSiteId", txtInventSiteId.Text);
                        Command.ExecuteScalar();
                    }

                    Trans.Commit();
                    MessageBox.Show("InventSiteBlokId = " + txtInventSiteBlokId.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diupdate.");
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
        
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }     

        private void grpCompanyInfo_Enter(object sender, EventArgs e)
        {

        }

        public void SetParent(Master.Site.SiteBlokInq F)
        {
            Parent = F;
        }

        public void GetDataHeader(string InventSiteBlokID)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select [InventSiteID], [InventSiteBlokID], [Deskripsi], [CreatedDate], [CreatedBy], [UpdatedDate],[UpdatedBy]  From [dbo].[InventSiteBlok] where InventSiteBlokID =@InventSiteBlokID ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@InventSiteBlokID", InventSiteBlokID);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtInventSiteId.Text = Dr["InventSiteId"].ToString();
                txtInventSiteBlokId.Text = Dr["InventSiteBlokID"].ToString();
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
            }
            Dr.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }

            vOldSiteID = txtInventSiteId.Text;
            vOldSiteName = txtInventSiteName.Text;
            vOldSiteBlokID = txtInventSiteBlokId.Text;
            vOldDeskripsi = txtDeskripsi.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();

            txtInventSiteName.Text = vOldSiteName;
            txtInventSiteBlokId.Text = vOldSiteBlokID;
            txtDeskripsi.Text = vOldDeskripsi;
        }

        private void SiteBlok_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshGrid();
        }

        public void GetNamaGudang()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT InventSiteName FROM [dbo].[InventSite] Where InventSiteId = @txtInventSiteId ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@txtInventSiteId", txtInventSiteId.Text);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtInventSiteName.Text = Dr["InventSiteName"].ToString();
            }
            Dr.Close();
        }
    }
}
