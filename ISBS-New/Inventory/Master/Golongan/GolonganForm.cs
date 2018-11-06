using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.Golongan
{
    public partial class GolonganForm : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string vOldDeskripsi;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String GolonganId = null;
        Master.Golongan.GolonganInquiry Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void flag(String golonganid, String mode)
        {
            GolonganId = golonganid;
            Mode = mode;
        }

        public GolonganForm()
        {
            InitializeComponent();
        }

        private void GologanForm_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
            }
            else
            {
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[InventGolongan] Where GolonganId = @GolonganId ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@GolonganId", GolonganId);
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtGolonganId.Text = GolonganId.ToString();
                    txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                }
            }
            Conn.Close();
        }

        private void ModeNew()
        {
            txtGolonganId.Enabled = true;
            txtDeskripsi.Enabled = true;
           
            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtGolonganId.Enabled = false;
            txtDeskripsi.Enabled = true;
           
            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnCancel.Visible = true;
        }

        public void ParentRefreshGrid(Master.Golongan.GolonganInquiry f)
        {
            Parent = f;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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

            vOldDeskripsi = txtDeskripsi.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtGolonganId.Enabled = false;
            txtDeskripsi.Enabled = false;
            
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
            txtDeskripsi.Text = vOldDeskripsi;
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtGolonganId.Text == "")
            {
                ErrMsg = "Golongan harus diisi..";
                vBol = false;
            }

            if (vBol == true && txtDeskripsi.Text.Trim() == "")
            {
                ErrMsg = "Deskripsi harus diisi..";
                vBol = false;
            }

            if (vBol == true && Mode == "New")
            {
                try
                {
                    Query = "Select * From InventGolongan Where GolonganID=@txtGolonganId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtGolonganId", txtGolonganId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Golongan sudah ada..";
                            vBol = false;
                        }
                        else
                        {
                            vBol = true;
                        }
                        Dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Conn.Close();
                }
            }

            if (vBol == true && Mode == "Edit")
            {
                try
                {
                    Query = "Select * From InventGolongan Where GolonganID=@txtGolonganId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtGolonganId", txtGolonganId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Golongan tidak ditemukan..";
                            vBol = false;
                        }
                        Dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Conn.Close();
                }
            }

            if (vBol == false) { MessageBox.Show(ErrMsg); }
            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cekValidasi() == false)
            {
                return;
            }


            if (Mode == "New")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "insert into [dbo].[InventGolongan] (GolonganId, Deskripsi, CreatedDate, CreatedBy) ";
                    Query += "values (@txtGolonganId, @txtDeskripsi, getDate(), '" + ControlMgr.UserId + "');";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@txtGolonganId", txtGolonganId.Text);
                        Cmd.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text);
                        Cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + txtGolonganId.Text + ", berhasil ditambahkan.");

                Form parentform = Application.OpenForms["GolonganInquiry"];
                if (parentform != null)
                    Parent.RefreshGrid();

                this.Close();

            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "update [dbo].[InventGolongan] set Deskripsi =@txtDeskripsi, UpdatedDate = getDate(), UpdatedBy = '" + ControlMgr.UserId + "' where GolonganId =@txtGolonganId ;";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@txtGolonganId", txtGolonganId.Text);
                    Cmd.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text);
                    Cmd.ExecuteScalar();
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + txtGolonganId.Text + " , berhasil diupdate.");

                Form parentform = Application.OpenForms["GolonganInquiry"];
                if (parentform != null)
                    Parent.RefreshGrid();

                this.Close();
            }
        }


    }
}
