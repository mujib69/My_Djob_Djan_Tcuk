using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Spec
{
    public partial class Spec : MetroFramework.Forms.MetroForm
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
        String SpecId = null;

        Master.Spec.InqSpec Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public Spec()
        {
            InitializeComponent();
        }

        public void flag(String specid, String mode)
        {
            SpecId = specid;
            Mode = mode;
        }

        public void setParent(Master.Spec.InqSpec F)
        {
            Parent = F;
        }

        private void Spec_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private bool cekValidasi(String SpecId)
        {
            Query = "Select * From [dbo].[InventSpec] Where SpecId = @SpecId ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@SpecId", SpecId);
                Dr = cmd.ExecuteReader();

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

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[InventSpec] Where SpecId = @SpecId ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@SpecId", SpecId);
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtSpecId.Text = Dr["SpecId"].ToString(); ;
                    txtDeskripsi.Text = Dr["Deskripsi"].ToString();
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
            txtSpecId.Text = "";
            txtDeskripsi.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtSpecId.Enabled = true;
            txtDeskripsi.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = false;
        }

        private void ModeEdit()
        {
            txtSpecId.Enabled = false;
            txtDeskripsi.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            Form parentform = Application.OpenForms["InqSpec"];
            if (parentform != null)
                Parent.RefreshGrid();

            this.Close();
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtSpecId.Text == "")
            {
                ErrMsg = "Spec harus diisi..";
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
                    Query = "Select * From InventSpec Where SpecID=@txtSpecId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtSpecId", txtSpecId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Spec sudah ada..";
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
                    Query = "Select * From InventSpec Where SpecID=@txtSpecId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtSpecId", txtSpecId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Spec tidak ditemukan..";
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
                    Query = "insert into [dbo].[InventSpec] (SpecId, Deskripsi) ";
                    Query += "values (@txtSpecId, @txtDeskripsi);";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@txtSpecId", txtSpecId.Text);
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
                MessageBox.Show("Data " + txtSpecId.Text + ", berhasil ditambahkan.");

                Form parentform = Application.OpenForms["InqSpec"];
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
                    Query = "update [dbo].[InventSpec] set Deskripsi=@txtDeskripsi where SpecId=@txtSpecId;";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@txtSpecId", txtSpecId.Text);
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
                MessageBox.Show("Data " + txtSpecId.Text + ", berhasil diupdate.");
                Parent.RefreshGrid();
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtSpecId.Enabled = false;
            txtDeskripsi.Enabled = false;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
            txtDeskripsi.Text = vOldDeskripsi;
        }
    }
}
