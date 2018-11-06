using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Invent.Type
{
    public partial class FormType : MetroFramework.Forms.MetroForm
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

        Master.Invent.Type.InquiryType Parent;

        public FormType()
        {
            InitializeComponent();
        }

        //Mode
        string Mode = "";

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        private void FormCompanyInfo_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }

        public void ModeNew()
        {
            Mode = "New";
            txtInventTypeId.Enabled = true;
            txtDeskripsi.Enabled = true;
            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";
            txtInventTypeId.Enabled = false;
            txtDeskripsi.Enabled = true;
            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            txtInventTypeId.Enabled = false;
            txtDeskripsi.Enabled = false;
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

            vOldDeskripsi = txtDeskripsi.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            txtDeskripsi.Text = vOldDeskripsi;
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtInventTypeId.Text.Trim() == "")
            {
                ErrMsg = "Type harus diisi..";
                vBol = false;
            }
            
            if (vBol==true && txtDeskripsi.Text.Trim() == "")
            {
                ErrMsg = "Deskripsi harus diisi..";
                vBol = false;
            }

            if (vBol == true && Mode == "New")
            {
                try
                {
                    Query = "Select * From InventType Where InventTypeId=@txtInventTypeId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtInventTypeId", txtInventTypeId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Type sudah ada..";
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
                    Query = "Select * From InventType Where InventTypeId=@txtInventTypeId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtInventTypeId", txtInventTypeId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Type tidak ditemukan..";
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
            if(cekValidasi()==false)
            {
                return;
            }

            try
            {
                Conn = ConnectionString.GetConnection();
                //Jika New
                if (Mode=="New")
                {
                    Query = "Insert into dbo.InventType (InventTypeID, Deskripsi, CreatedDate, CreatedBy) values ";
                    Query += "(@txtInventTypeId,@txtDeskripsi,getdate(),'" + ControlMgr.UserId + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtInventTypeId", txtInventTypeId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("InventTypeID = " + txtInventTypeId.Text.Trim().ToUpper() + Environment.NewLine + "Deskripsi = " + txtDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");            
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventType set Deskripsi=@txtDeskripsi, UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where InventTypeID=@txtInventTypeId ";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtInventTypeId", txtInventTypeId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("InventTypeID = " + txtInventTypeId.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diedit.");
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

        public void SetParent(Master.Invent.Type.InquiryType F)
        {
            Parent = F;
        }

        private void FormCompanyInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form typeinquiry = Application.OpenForms["InquiryType"];
            if (typeinquiry != null)
                Parent.RefreshGrid();
        }

        public void GetDataHeader(string DimensionId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select InventTypeID, Deskripsi From [dbo].[InventType] where InventTypeID=@DimensionId ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DimensionId", DimensionId);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtInventTypeId.Text = Dr["InventTypeID"].ToString();
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
            }
            Dr.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
     
                        
    }
}
