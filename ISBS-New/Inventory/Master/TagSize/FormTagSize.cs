using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Invent.TagSize
{
    public partial class FormTagSize : MetroFramework.Forms.MetroForm
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

        string vOldDeskripsi;

        Master.Invent.TagSize.InquiryTagSize Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormTagSize()
        {
            InitializeComponent();
        }

        private void FormTagSize_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }

        //Mode
        string Mode = "";


        public void ModeNew()
        {
            Mode = "New";

            txtTagSizeId.Enabled = false;
            txtDeskripsi.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        private bool Used()
        {
            Boolean vBol = false;
            try
            {
                Query = "Select * From [dbo].[InventTable] Where TagSizeID='" + txtTagSizeId.Text.Trim() + "'";
                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();
                    if (Dr.Read())
                    {
                        vBol = true;
                    }
                    else
                    {
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

            return vBol;
        }

        public void ModeEdit()
        {
            if (Used() == true)
            {
                MessageBox.Show("Tidak boleh Edit, Group Name sudah pernah digunakan..");
            }
            else
            {
                Mode = "Edit";

                txtTagSizeId.Enabled = false;
                txtDeskripsi.Enabled = true;

                btnSave.Visible = true;
                btnEdit.Visible = false;
                btnCancel.Visible = true;
                btnExit.Visible = false;
            }
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            txtTagSizeId.Enabled = false;
            txtDeskripsi.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;

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

            vOldDeskripsi=txtDeskripsi.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            txtDeskripsi.Text=vOldDeskripsi;
        }


        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";           

            if (vBol == true && txtDeskripsi.Text.Trim() == "")
            {
                ErrMsg = "Deskripsi harus diisi..";
                vBol = false;
            }

            if (vBol == true )
            {
                try
                {
                    Query = "Select * From TagSize Where TagSize_Description=@txtDeskripsi ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Tag Size sudah ada..";
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
                    Query = "Select * From TagSize Where TagSize_Code=@txtTagSizeId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtTagSizeId", txtTagSizeId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Tag Size tidak ditemukan..";
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
                Conn = ConnectionString.GetConnection();
              
                //Jika New
                if (Mode=="New")
                {
                    Query = "Insert into dbo.TagSize ([TagSize_Code], [TagSize_Description], CreatedDate, CreatedBy) OUTPUT INSERTED.TagSize_Code values ";
                    Query += "((SELECT RIGHT('000'+ISNULL(CAST((Max(TagSize_Code)+1) AS VARCHAR(3)),1),3) from TagSize),@txtDeskripsi,getdate(),'" + ControlMgr.UserId + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        txtTagSizeId.Text = Command.ExecuteScalar().ToString();
                        //Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("Tag Size = " + txtTagSizeId.Text.Trim().ToUpper() + " Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.TagSize set TagSize_Description=@txtDeskripsi, UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where TagSize_Code=@txtTagSizeId";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtTagSizeId", txtTagSizeId.Text);
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("Tag Size = " + txtTagSizeId.Text.Trim().ToUpper() + " Berhasil diupdate.");
                }               
                
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();

                Form parentform = Application.OpenForms["InquiryTagSize"];
                if (parentform != null)
                    Parent.RefreshGrid();

                this.Close();
            }  
        }

        public void SetParent(Master.Invent.TagSize.InquiryTagSize F)
        {
            Parent = F;
        }

        public void GetDataHeader(string TagSize_Code)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select TagSize_Code, TagSize_Description From [dbo].[TagSize] where TagSize_Code=@TagSize_Code ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@TagSize_Code", TagSize_Code);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtTagSizeId.Text = Dr["TagSize_Code"].ToString();
                txtDeskripsi.Text = Dr["TagSize_Description"].ToString();
            }
            Dr.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }                        
    }
}
