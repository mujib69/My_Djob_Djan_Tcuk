using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Invent.SubGroup2
{
    public partial class FormSubGroup2 : MetroFramework.Forms.MetroForm
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

        string vOldGroupId, vOldSubGroup1Id, vOldDeskripsi;

        Master.Invent.SubGroup2.InquirySubGroup2 Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormSubGroup2()
        {
            InitializeComponent();
            GlobalSetting();
        }

        private void FormSubGroup2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }

        //Mode
        string Mode = "";

        public void GlobalSetting()
        {
            txtGroupId.Enabled = false;
            txtGroupDeskripsi.Enabled = false;
            txtSubGroup1Id.Enabled = false;            
            txtSubGroup1Deskripsi.Enabled = false;
        }

        public void ModeNew()
        {
            Mode = "New";

            btnSearchGroup.Enabled = true;
           
            txtSubGroup2Id.Enabled = false;
            txtSubGroup2Deskripsi.Enabled = true;

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
                Query = "Select * From [dbo].[InventTable] Where SubGroup2ID='" + txtSubGroup2Id.Text + "'";
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

                btnSearchGroup.Enabled = true;

                txtSubGroup2Id.Enabled = false;
                txtSubGroup2Deskripsi.Enabled = true;

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

            txtSubGroup2Id.Enabled = false;
            txtSubGroup2Deskripsi.Enabled = false;

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

            vOldGroupId =txtGroupId.Text;
            vOldSubGroup1Id = txtSubGroup1Id.Text;
            vOldDeskripsi=txtSubGroup2Deskripsi.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            txtGroupId.Text = vOldGroupId;
            txtSubGroup1Id.Text=vOldSubGroup1Id;
            txtSubGroup2Deskripsi.Text=vOldDeskripsi;
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtSubGroup1Id.Text.Trim() == "")
            {
                ErrMsg = "Sub Group Item harus dipilih..";
                vBol = false;
            }

            if (vBol == true && txtSubGroup2Deskripsi.Text.Trim() == "")
            {
                ErrMsg = "Nama Sub Group harus diisi..";
                vBol = false;
            }
            
            if(vBol==true )
            {
                try
                {
                    Query = "Select * From [dbo].[InventSubGroup2] Where SubGroup2Id<>'" + txtSubGroup2Id.Text + "' AND Deskripsi = @Deskripsi ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@Deskripsi", txtSubGroup2Deskripsi.Text);
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
                    Query = "Select * From [dbo].[InventSubGroup2] Where SubGroup2ID='" + txtSubGroup2Id.Text + "'";
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
                    Query = "Insert into dbo.InventSubGroup2 (GroupID,GroupDesc, SubGroup1ID,SubGroup1Desc, SubGroup2ID, Deskripsi, CreatedDate, CreatedBy) OUTPUT INSERTED.SubGroup2ID values ";
                    Query += "(@txtGroupId,@txtGroupDesc,@txtSubGroup1Id,@txtSubGroup1Desc,(SELECT RIGHT('000'+ISNULL(CAST((Max(SubGroup2ID)+1) AS VARCHAR(3)),1),3) from InventSubGroup2),@txtSubGroup2Deskripsi,getdate(),'" + ControlMgr.UserId + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtGroupId",txtGroupId.Text);
                        Command.Parameters.AddWithValue("@txtGroupDesc", txtGroupDeskripsi.Text);
                        Command.Parameters.AddWithValue("@txtSubGroup1Id",txtSubGroup1Id.Text);
                        Command.Parameters.AddWithValue("@txtSubGroup1Desc", txtSubGroup1Deskripsi.Text);
                        Command.Parameters.AddWithValue("@txtSubGroup2Deskripsi", txtSubGroup2Deskripsi.Text.Trim().ToUpper());
                        txtSubGroup2Id.Text = Command.ExecuteScalar().ToString();
                    }
                    Trans.Commit();
                    MessageBox.Show("SubGroup2ID = " + txtSubGroup2Id.Text.Trim().ToUpper() + Environment.NewLine + "Deskripsi = " + txtSubGroup2Deskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventSubGroup2 set SubGroup1ID=@txtSubGroup1Id,SubGroup1Desc=@txtSubGroup1Desc, Deskripsi=@txtSubGroup2Deskripsi, GroupId=@txtGroupId,GroupDesc=@txtGroupDesc, UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where SubGroup2ID=@txtSubGroup2Id ";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtSubGroup1Id", txtSubGroup1Id.Text);
                        Command.Parameters.AddWithValue("@txtSubGroup1Desc", txtSubGroup1Deskripsi.Text);
                        Command.Parameters.AddWithValue("@txtSubGroup2Deskripsi", txtSubGroup2Deskripsi.Text.Trim().ToUpper());
                        Command.Parameters.AddWithValue("@txtGroupId", txtGroupId.Text);
                        Command.Parameters.AddWithValue("@txtGroupDesc", txtGroupDeskripsi.Text);
                        Command.Parameters.AddWithValue("@txtSubGroup2Id", txtSubGroup2Id.Text);
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("SubGroup2ID = " + txtSubGroup2Id.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diupdate.");
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

        public void SetParent(Master.Invent.SubGroup2.InquirySubGroup2 F)
        {
            Parent = F;
        }

        public void GetDataHeader(string SubGroup2ID)
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT c.GroupId, a.Deskripsi AS GroupDeskripsi, c.SubGroup1ID, b.Deskripsi AS SubGroup1Deskripsi, c.SubGroup2ID, c.Deskripsi ";
            Query += "FROM [dbo].[InventSubGroup2] c ";
            Query += "LEFT JOIN [InventGroup] a ON a.GroupId = c.GroupId ";
            Query += "LEFT JOIN [InventSubGroup1] b ON b.SubGroup1ID = c.SubGroup1ID ";
            Query += "WHERE SubGroup2ID=@SubGroup2ID";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@SubGroup2ID", SubGroup2ID);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtGroupId.Text = Dr["GroupId"].ToString();
                txtGroupDeskripsi.Text = Dr["GroupDeskripsi"].ToString();
                txtSubGroup1Id.Text = Dr["SubGroup1ID"].ToString();
                txtSubGroup1Deskripsi.Text = Dr["SubGroup1Deskripsi"].ToString();
                txtSubGroup2Id.Text = Dr["SubGroup2ID"].ToString();
                txtSubGroup2Deskripsi.Text = Dr["Deskripsi"].ToString();
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
            string TableName = "InventSubGroup1";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if(ConnectionString.Kode!="")
            {
                txtGroupId.Text = ConnectionString.Kode;
                txtSubGroup1Id.Text = ConnectionString.Kode2;

                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("Select Deskripsi from InventGroup where GroupId = @txtGroupId", Conn);
                Cmd.Parameters.AddWithValue("@txtGroupId", txtGroupId.Text);
                txtGroupDeskripsi.Text = Cmd.ExecuteScalar().ToString();

                Cmd = new SqlCommand("Select Deskripsi from InventSubGroup1 where SubGroup1ID = @txtSubGroup1Id", Conn);
                Cmd.Parameters.AddWithValue("@txtSubGroup1Id", txtSubGroup1Id.Text);
                txtSubGroup1Deskripsi.Text = Cmd.ExecuteScalar().ToString();
                Conn.Close();
            }

            ConnectionString.Kode = "";
            ConnectionString.Kode2 = "";
        }

        private void btnSearchSubGroup1_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSubGroup1";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtSubGroup1Id.Text = ConnectionString.Kode;
        }

        private void FormSubGroup2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form parentform = Application.OpenForms["InquirySubGroup2"];
            if (parentform != null)
                Parent.RefreshGrid();
        }
                        
    }
}
