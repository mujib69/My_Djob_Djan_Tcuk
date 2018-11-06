using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Invent.Unit
{
    public partial class FormUnit : MetroFramework.Forms.MetroForm
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

        Master.Invent.Unit.InquiryUnit Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormUnit()
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

            txtUnitId.Enabled = true;
            txtUnitName.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtUnitId.Enabled = false;
            txtUnitName.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            txtUnitId.Enabled = false;
            txtUnitName.Enabled = false;

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

            vOldDeskripsi = txtUnitName.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            txtUnitName.Text = vOldDeskripsi;
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtUnitId.Text == "")
            {
                ErrMsg = "Unit harus diisi..";
                vBol = false;
            }

            if (vBol == true && txtUnitName.Text.Trim() == "")
            {
                ErrMsg = "Deskripsi harus diisi..";
                vBol = false;
            }

            if (vBol == true && Mode == "New")
            {
                try
                {
                    Query = "Select * From InventUnit Where UnitID=@txtUnitId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtUnitId", txtUnitId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Unit sudah ada..";
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
                    Query = "Select * From InventUnit Where UnitID=@txtUnitId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtUnitId", txtUnitId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Unit tidak ditemukan..";
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
                if (Mode=="New")
                {
                    Query = "Insert into dbo.InventUnit (UnitID, UnitName, CreatedBy) values ";
                    Query += "(@txtUnitId,@txtUnitName, '" + ControlMgr.UserId + "')";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtUnitId", txtUnitId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtUnitName", txtUnitName.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("UnitID = " + txtUnitId.Text.Trim().ToUpper() + Environment.NewLine + " UnitName = " + txtUnitName.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventUnit set UnitName=@txtUnitName, UpdatedBy = '"+ ControlMgr.UserId +"' where UnitID=@txtUnitId";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtUnitId", txtUnitId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtUnitName", txtUnitName.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("UnitID = " + txtUnitId.Text.Trim().ToUpper() + Environment.NewLine + " Berhasil diedit..");
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

                Form parentform = Application.OpenForms["InquiryUnit"];
                if (parentform != null)
                    Parent.RefreshGrid();
            }  
        }

        public void SetParent(Master.Invent.Unit.InquiryUnit F)
        {
            Parent = F;
        }

        public void GetDataHeader(string DimensionId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select UnitId, UnitName From [dbo].[InventUnit] where UnitId=@DimensionId ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@DimensionId", DimensionId);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtUnitId.Text = Dr["UnitId"].ToString();
                txtUnitName.Text = Dr["UnitName"].ToString();
            }
            Dr.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
     
                        
    }
}
