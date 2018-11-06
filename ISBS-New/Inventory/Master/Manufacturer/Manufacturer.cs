using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Manufacturer
{
    public partial class Manufacturer : MetroFramework.Forms.MetroForm
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
        String ManufacturerId = null;

        Master.Manufacturer.InqManufacturer Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public Manufacturer()
        {
            InitializeComponent();
        }

        public void flag(String manufacturerid, String mode)
        {
            ManufacturerId = manufacturerid;
            Mode = mode;
        }

        public void setParent(Master.Manufacturer.InqManufacturer F)
        {
            Parent = F;
        }

        private void Manufacturer_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private bool cekValidasi(String ManufacturerId)
        {
            Query = "Select * From [dbo].[InventManufacturer] Where ManufacturerId = @ManufacturerId";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@ManufacturerId", ManufacturerId);
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
            Query = "Select * From [dbo].[InventManufacturer] Where ManufacturerId = @ManufacturerId ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@ManufacturerId", ManufacturerId);
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtManufacturerId.Text = Dr["ManufacturerId"].ToString();;
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
            txtManufacturerId.Text = "";
            txtDeskripsi.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtManufacturerId.Enabled = true;
            txtDeskripsi.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = false;
        }

        private void ModeEdit()
        {
            txtManufacturerId.Enabled = false;
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
            this.Close();
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtManufacturerId.Text == "")
            {
                MessageBox.Show("Manufacturer Id harus diisi..");
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
                    Query = "Select * From InventManufacturer Where ManufacturerID=@txtManufacturerId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtManufacturerId", txtManufacturerId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Manufacturer Id sudah ada..";
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
                    Query = "Select * From InventManufacturer Where ManufacturerID=@txtManufacturerId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtManufacturerId", txtManufacturerId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Manufacturer Id tidak ditemukan..";
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
                    Query = "insert into [dbo].[InventManufacturer] (ManufacturerId, Deskripsi, CreatedBy, CreatedDate ) ";
                    Query += "values (@txtManufacturerId, @txtDeskripsi,'" + ControlMgr.UserId + "', getdate())";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@txtManufacturerId", txtManufacturerId.Text);
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
                MessageBox.Show("Data " + txtManufacturerId.Text + ", berhasil ditambahkan.");

                Form parentform = Application.OpenForms["InqManufacturer"];
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
                    Query = "update [dbo].[InventManufacturer] set Deskripsi=@txtDeskripsi, UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where ManufacturerId=@txtManufacturerId ";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@txtManufacturerId", txtManufacturerId.Text);
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
                MessageBox.Show("Data " + txtManufacturerId.Text + ", berhasil diupdate.");

                Form parentform = Application.OpenForms["InqManufacturer"];
                if (parentform != null)
                    Parent.RefreshGrid();

                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtManufacturerId.Enabled = false;
            txtDeskripsi.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;
            txtDeskripsi.Text = vOldDeskripsi;
        }
    }
}
