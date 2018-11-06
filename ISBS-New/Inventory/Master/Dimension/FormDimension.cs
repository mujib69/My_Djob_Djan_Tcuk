using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Invent.Dimension
{
    public partial class FormDimension : MetroFramework.Forms.MetroForm
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
        Master.Invent.Dimension.InquiryDimension Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end


        public FormDimension()
        {
            InitializeComponent();
        }

        //Mode
        string Mode = "";

        private void FormCompanyInfo_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }

        public void ModeNew()
        {
            Mode = "New";

            txtDimensionId.Enabled = true;
            txtDimensionName.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtDimensionId.Enabled = false;
            txtDimensionName.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            txtDimensionId.Enabled = false;
            txtDimensionName.Enabled = false;

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
            vOldDeskripsi = txtDimensionName.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();

            txtDimensionName.Text = vOldDeskripsi;
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtDimensionId.Text == "")
            {
                ErrMsg = "Dimension Code harus diisi..";
                vBol = false;
            }

            if (vBol == true && txtDimensionName.Text.Trim() == "")
            {
                ErrMsg = "Dimension Name harus diisi..";
                vBol = false;
            }

            if (vBol == true && Mode == "New")
            {
                try
                {
                    Query = "Select * From InventDimension Where DimensionID=@txtDimensionId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtDimensionId", txtDimensionId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Dimension sudah ada..";
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
                    Query = "Select * From InventDimension Where DimensionID=@txtDimensionId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtDimensionId", txtDimensionId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Dimension Code tidak ditemukan..";
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
                //Cek Data apakah sudah ada
                string CekData;
                Conn = ConnectionString.GetConnection();

                //Jika New
                if (Mode=="New")
                {
                    Query = "Insert into dbo.InventDimension (DimensionID, DimensionName, CreatedBy) values ";
                    Query += "(@txtDimensionId,@txtDimensionName, '" + ControlMgr.UserId + "')";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtDimensionId", txtDimensionId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDimensionName", txtDimensionName.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("DimensionID = " + txtDimensionId.Text.Trim().ToUpper() + Environment.NewLine + "CompanyName = " + txtDimensionName.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventDimension set DimensionName=@txtDimensionName, UpdatedBy = '" + ControlMgr.UserId + "' where DimensionID=@txtDimensionId ";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtDimensionName", txtDimensionName.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDimensionId", txtDimensionId.Text);
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("DimensionID = " + txtDimensionId.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diedit.");
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
                Form parentform = Application.OpenForms["InquiryDimension"];
                if (parentform != null)
                    Parent.RefreshGrid();

                Conn.Close();
            }  
        }

        public void SetParent(Master.Invent.Dimension.InquiryDimension F)
        {
            Parent = F;
        }

        //private void FormCompanyInfo_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    Parent.RefreshGrid();
        //}

        public void GetDataHeader(string DimensionId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select DimensionID, DimensionName From [dbo].[InventDimension] where DimensionID=@DimensionId ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DimensionId", DimensionId);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtDimensionId.Text = Dr["DimensionID"].ToString();
                txtDimensionName.Text = Dr["DimensionName"].ToString();
            }
            Dr.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
     
                        
    }
}
