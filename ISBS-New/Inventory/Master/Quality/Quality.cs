using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Quality
{
    public partial class Quality : MetroFramework.Forms.MetroForm
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
        String QualityId = null;

        Master.Quality.InqQuality Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public Quality()
        {
            InitializeComponent();
        }

        public void flag(String qualityid, String mode)
        {
            QualityId = qualityid;
            Mode = mode;
        }

        public void setParent(Master.Quality.InqQuality F)
        {
            Parent = F;
        }

        private void Quality_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private bool cekValidasi(String QualityId)
        {
            Query = "Select * From [dbo].[InventQuality] Where QualityId = @QualityId ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@QualityId", QualityId);
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
            Query = "Select * From [dbo].[InventQuality] Where QualityId = @QualityId ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@QualityId", QualityId);
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtQualityId.Text = Dr["QualityId"].ToString(); ;
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
            txtQualityId.Text = "";
            txtDeskripsi.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtQualityId.Enabled = true;
            txtDeskripsi.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = false;
        }

        private void ModeEdit()
        {
            txtQualityId.Enabled = false;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtQualityId.Text) || String.IsNullOrEmpty(txtDeskripsi.Text))
            {
                MessageBox.Show("Quality dan Deskripsi harus diisi");
                return;
            }

            if (Mode == "New")
            {             
                if (cekValidasi(txtQualityId.Text) == false)
                {
                    MessageBox.Show("Quality sudah ada..");
                }
                else
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    try
                    {
                        Query = "insert into [dbo].[InventQuality] (QualityId, Deskripsi, CreatedBy, CreatedDate) ";
                        Query += "values (@txtQualityId, @txtDeskripsi, '" + ControlMgr.UserId + "', getdate());";

                        using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Cmd.Parameters.AddWithValue("@txtQualityId", txtQualityId.Text);
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
                    MessageBox.Show("Data " + txtQualityId.Text + ", berhasil ditambahkan.");

                    Form parentform = Application.OpenForms["InqQuality"];
                    if (parentform != null)
                        Parent.RefreshGrid();

                    this.Close();
                }
            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "update [dbo].[InventQuality] set Deskripsi=@txtDeskripsi, UpdatedDate = getdate(), UpdatedBy = '"+ControlMgr.UserId+"' where QualityId=@txtQualityId ;";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@txtQualityId", txtQualityId.Text);
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
                MessageBox.Show("Data " + txtQualityId.Text + ", berhasil diupdate.");

                Form parentform = Application.OpenForms["InqQuality"];
                if (parentform != null)
                    Parent.RefreshGrid();

                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtQualityId.Enabled = false;
            txtDeskripsi.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;
            txtDeskripsi.Text = vOldDeskripsi;
        }
    }
}
