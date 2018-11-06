using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


//Form created by Thaddaeus Matthias, 16 March 2018

namespace ISBS_New.Inventory.Master.KodeBerat
{
    public partial class FormKodeBerat : MetroFramework.Forms.MetroForm
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

        KodeBerat.InquiryKodeBerat Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormKodeBerat()
        {
            InitializeComponent();
        }

        //Mode
        string Mode = "";

        private void FormKodeBerat_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }

        public void ModeNew()
        {
            Mode = "New";

            txtKodeBeratId.Enabled = true;
            txtDeskripsi.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtKodeBeratId.Enabled = false;
            txtDeskripsi.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            txtKodeBeratId.Enabled = false;
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

            if (txtKodeBeratId.Text == "")
            {
                ErrMsg = "Weight Code harus diisi..";
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
                    Query = "Select * From InventBerat Where BeratID=@txtBeratId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtBeratId", txtKodeBeratId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Weight Code sudah ada..";
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
                    Query = "Select * From InventBerat Where BeratID=@txtBeratId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtBeratId", txtKodeBeratId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Weight Code tidak ditemukan..";
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
                    Query = "Insert into dbo.InventBerat (BeratID, Deskripsi, CreatedBy) values ";
                    Query += "(@txtBeratId,@txtDeskripsi, '" + ControlMgr.UserId + "')";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtBeratId", txtKodeBeratId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("BeratID = " + txtKodeBeratId.Text.Trim().ToUpper() + Environment.NewLine + " Deskripsi = " + txtDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventBerat set Deskripsi=@txtDeskripsi, UpdatedBy = '"+ ControlMgr.UserId +"' where BeratID=@txtBeratId";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtBeratId", txtKodeBeratId.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("BeratID = " + txtKodeBeratId.Text.Trim().ToUpper() + Environment.NewLine + " Berhasil diedit..");
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

                Form parentform = Application.OpenForms["InquiryKodeBerat"];
                if (parentform != null)
                    Parent.RefreshGrid();
            }  
        }

        public void SetParent(KodeBerat.InquiryKodeBerat F)
        {
            Parent = F;
        }

        public void GetDataHeader(string BeratId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select BeratId, Deskripsi From [dbo].[InventBerat] where BeratId=@BeratId ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@BeratId", BeratId);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtKodeBeratId.Text = Dr["BeratId"].ToString();
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
