using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Invent.SubGroup1
{
    public partial class FormSubGroup1 : MetroFramework.Forms.MetroForm
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

        string vOldGroupId,vOldDeskripsi;

        Master.Invent.SubGroup1.InquirySubGroup1 Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormSubGroup1()
        {
            InitializeComponent();
        }

        private void FormSubGroup1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }

        //Mode
        string Mode = "";


        public void ModeNew()
        {
            Mode = "New";

            btnSearchGroup.Enabled = true;

            txtGroupId.Enabled = false;
            txtGroupDeskripsi.Enabled = false;
            txtSubGroup1Id.Enabled = false;
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
                Query = "Select * From [dbo].[InventSubGroup2] Where SubGroup1ID='" + txtSubGroup1Id.Text + "'";
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
            Mode = "Edit";

            if (Used() == true)
            {
                MessageBox.Show("Tidak boleh Edit, Sub Group Name sudah pernah digunakan..");
            }
            else
            {
                btnSearchGroup.Enabled = true;

                txtGroupId.Enabled = false;
                txtGroupDeskripsi.Enabled = false;
                txtSubGroup1Id.Enabled = false;
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
            btnSearchGroup.Enabled = false;
            txtGroupId.Enabled = false;
            txtGroupDeskripsi.Enabled = false;
            txtSubGroup1Id.Enabled = false;
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

            vOldGroupId = txtGroupId.Text;
            vOldDeskripsi = txtDeskripsi.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();

            txtGroupId.Text = vOldGroupId;
            txtDeskripsi.Text=vOldDeskripsi;
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtGroupId.Text.Trim() == "")
            {
                ErrMsg = "Group Item harus dipilih..";
                vBol = false;
            }
            
            if (vBol==true && txtDeskripsi.Text.Trim() == "")
            {
                ErrMsg = "Nama Sub Group harus diisi..";
                vBol = false;
            }
            
            if (vBol==true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventSubGroup1] Where SubGroup1Id<>@txtSubGroup1Id AND Deskripsi = @Deskripsi ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text);
                        cmd.Parameters.AddWithValue("@txtSubGroup1Id", txtSubGroup1Id.Text);
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Nama Sub Group sudah ada..";
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

            if (Mode == "Edit" && vBol == true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventSubGroup1] Where SubGroup1ID='" + txtSubGroup1Id.Text + "'";
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
                            ErrMsg = "Group Name tidak ditemukan..";
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
                    Query = "Insert into dbo.InventSubGroup1 (GroupID,GroupDesc, SubGroup1ID, Deskripsi, CreatedDate, CreatedBy) OUTPUT INSERTED.SubGroup1ID values ";
                    Query += "(@txtGroupId,@txtGroupDesc,(SELECT RIGHT('00'+ISNULL(CAST((Max(SubGroup1ID)+1) AS VARCHAR(2)),1),2) from InventSubGroup1),@txtDeskripsi,getdate(),'" + ControlMgr.UserId + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtGroupId",txtGroupId.Text);
                        Command.Parameters.AddWithValue("@txtGroupDesc", txtGroupDeskripsi.Text);
                        Command.Parameters.AddWithValue("@txtDeskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        txtSubGroup1Id.Text = Command.ExecuteScalar().ToString();
                    }
                    Trans.Commit();
                    MessageBox.Show("SubGroup1ID = " + txtSubGroup1Id.Text.Trim().ToUpper() + Environment.NewLine + "Deskripsi = " + txtDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventSubGroup1 set Deskripsi=@txtDeskripsi, GroupId=@txtGroupId, GroupDesc=@txtGroupDesc, UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where SubGroup1ID=@txtSubGroup1Id";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtDeskripsi",txtDeskripsi.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtGroupId",txtGroupId.Text);
                        Command.Parameters.AddWithValue("@txtSubGroup1Id", txtSubGroup1Id.Text);
                        Command.Parameters.AddWithValue("@txtGroupDesc", txtGroupDeskripsi.Text);
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("SubGroup1ID = " + txtSubGroup1Id.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diupdate.");
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

        public void SetParent(Master.Invent.SubGroup1.InquirySubGroup1 F)
        {
            Parent = F;
        }

        public void GetDataHeader(string SubGroup1ID)
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT b.GroupId, a.Deskripsi AS GroupDeskripsi, b.SubGroup1ID, b.Deskripsi ";
            Query += "FROM [dbo].[InventSubGroup1] b ";
            Query += "LEFT JOIN [InventGroup] a ON a.GroupId = b.GroupId ";
            Query += "WHERE b.SubGroup1ID = @SubGroup1ID ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@SubGroup1ID", SubGroup1ID);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtGroupId.Text = Dr["GroupId"].ToString();
                txtGroupDeskripsi.Text = Dr["GroupDeskripsi"].ToString();
                txtSubGroup1Id.Text = Dr["SubGroup1ID"].ToString();
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
            }
            Dr.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchGroup_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventGroup";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if(ConnectionString.Kode!="")
            {
                txtGroupId.Text = ConnectionString.Kode;
                txtGroupDeskripsi.Text = ConnectionString.Kode2;
            }
            ConnectionString.Kode="";
        }

        private void FormSubGroup1_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            Form parentform = Application.OpenForms["InquirySubGroup1"];
            if (parentform != null)
                Parent.RefreshGrid();
        }                            
    }
}
