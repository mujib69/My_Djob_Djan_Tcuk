using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Counter
{
    public partial class Counter : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String Jenis, Kode = null;

        //InqCounter Parent;

        //begin
        //created by : joshua
        //created date : 26 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        public void SetMode(string passedMode, string id, string id2)
        {
            Mode = passedMode;
            Jenis = id;
            Kode = id2;
        }

        public Counter()
        {
            InitializeComponent();
        }

        //public void SetParent(InqCounter F)
        //{
        //    Parent = F;
        //}

        //public void flag(String jenis, String kode, String mode)
        //{
        //    Jenis = jenis;
        //    Kode = kode;
        //    Mode = mode;
        //}

        private void Counter_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
        }

        

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[Counter] Where Jenis = @jenis And Kode = @kode";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@jenis", Jenis);
                Cmd.Parameters.AddWithValue("@kode", Kode);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtJenis.Text = Dr["Jenis"].ToString();
                    txtKode.Text = Dr["Kode"].ToString();
                    txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                    txtCounter.Text = Dr["Counter"].ToString();
                }
            }
            Conn.Close();
        }

        private void ModeLoad()
        {
            RefreshGrid();
        }

        private void resetText()
        {
            txtJenis.Text = "";
            txtKode.Text = "";
            txtCounter.Text = "";
            txtDeskripsi.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtJenis.Enabled = true;
            txtKode.Enabled = true;
            txtCounter.Enabled = false;
            txtDeskripsi.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtJenis.Enabled = false;
            txtKode.Enabled = false;
            txtCounter.Enabled = true;
            txtDeskripsi.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
        }

        private void ModeBeforeEdit()
        {
            txtJenis.Enabled = false;
            txtKode.Enabled = false;
            txtCounter.Enabled = false;
            txtDeskripsi.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
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
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean Validasi()
        {
            if (txtJenis.Text.Trim() == "" || txtKode.Text.Trim() == "")
            {
                MessageBox.Show("Data harus diisi");
                return false;
            }
            else if (Mode == "New" && cekValidasi(txtJenis.Text, txtKode.Text) == false)
            {
                MessageBox.Show("Data sudah ada di database.");
                return false;
            }
            else
                return true;
        }

        private bool cekValidasi(String Jenis, String Kode)
        {
            Query = "Select * From [dbo].[Counter] Where Jenis = @jenis And Kode = @kode";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@jenis", Jenis);
                Cmd.Parameters.AddWithValue("@kode", Kode);

                Dr = Cmd.ExecuteReader();

                if (Dr.Read())//sama dengan di database
                {
                    Conn.Close();
                    return false;
                }
                else
                {
                    Conn.Close();
                    return true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {           
            if (Validasi() == false)
                return;

            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (Mode == "New")
                {
                    int Counter = String.IsNullOrEmpty(txtCounter.Text) ? 1 : Int32.Parse(txtCounter.Text);

                    Query = "insert into [dbo].[Counter] (Jenis, Kode, Counter, Deskripsi) ";
                    Query += "values (@jenis, @kode, @counter, @deskripsi);";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@jenis", txtJenis.Text);
                        Cmd.Parameters.AddWithValue("@kode", txtKode.Text);
                        Cmd.Parameters.AddWithValue("@counter", Counter);
                        Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);

                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("Data " + txtJenis.Text + " " + txtKode.Text + ", berhasil ditambahkan.");

                }
                else if (Mode == "Edit")
                {
                    Query = "update [dbo].[Counter] set Counter=@counter, Deskripsi=@deskripsi where Jenis=@jenis And Kode=@kode;";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@jenis", txtJenis.Text);
                    Cmd.Parameters.AddWithValue("@kode", txtKode.Text);
                    Cmd.Parameters.AddWithValue("@counter", txtCounter.Text);
                    Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);

                    Cmd.ExecuteScalar();
                    Trans.Commit();
                    MessageBox.Show("Data " + txtJenis.Text + " " + txtKode.Text + ", berhasil diupdate.");

                }
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
           
            Conn.Close();
            ModeBeforeEdit();

            Form f = Application.OpenForms["GlobalInquiry"];
            if (f != null)
                if (f.Text == "Counter")
                    Parent.RefreshGrid();
        }
    }
}
