using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Merek
{
    public partial class Merek : MetroFramework.Forms.MetroForm
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
        String MerekId = null;

        Master.Merek.InqMerek Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public Merek()
        {
            InitializeComponent();
        }

        public void flag(String merekid, String mode)
        {
            MerekId = merekid;
            Mode = mode;
        }

        public void setParent(Master.Merek.InqMerek F)
        {
            Parent = F;
        }

        private void Merek_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private bool cekValidasi(String MerekId)
        {
            Query = "Select * From [dbo].[InventMerek] Where MerekId = @MerekId";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@MerekId", MerekId);
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
            Query = "Select * From [dbo].[InventMerek] Where MerekId = @MerekId ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                cmd.Parameters.AddWithValue("@MerekId", MerekId);
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtMerekId.Text = Dr["MerekId"].ToString(); ;
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
            txtMerekId.Text = "";
            txtDeskripsi.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtMerekId.Enabled = true;
            txtDeskripsi.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = false;
        }

        private void ModeEdit()
        {
            txtMerekId.Enabled = false;
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

            if (txtMerekId.Text == "")
            {
                ErrMsg = "Brand harus diisi..";
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
                    Query = "Select * From InventMerek Where MerekID=@txtMerekId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtMerekId", txtMerekId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Brand sudah ada..";
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
                    Query = "Select * From InventMerek Where MerekID=@txtMerekId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtMerekId", txtMerekId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Brand tidak ditemukan..";
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

            try
            {
                string CekData;
                Conn = ConnectionString.GetConnection();


                //Jika New
                if (Mode == "New")
                {
                    Query = "Insert into dbo.InventMerek (MerekID,Deskripsi,CreatedBy) values ";
                    Query += "(@txtMerekId,@txtDeskripsi, '" + ControlMgr.UserId + "')";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtMerekId", txtMerekId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("Brand Id = " + txtMerekId.Text.Trim().ToUpper() + Environment.NewLine + " Deskripsi = " + txtDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode == "Edit")
                {
                    Query = "Update dbo.InventMerek set Deskripsi=@txtDeskripsi, UpdatedBy = '" + ControlMgr.UserId + "' where MerekID=@txtMerekId";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtMerekId", txtMerekId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("Brand = " + txtMerekId.Text.Trim().ToUpper() + Environment.NewLine + " Berhasil diedit..");
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

                Form parentform = Application.OpenForms["InqMerek"];
                if (parentform != null)
                    Parent.RefreshGrid();
            }  
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtMerekId.Enabled = false;
            txtDeskripsi.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;
            txtDeskripsi.Text = vOldDeskripsi;
        }
    }
}
