using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.Group
{
    public partial class GroupForm : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string strSql;
        private SqlConnection ConMaster;

        string vOldDeskripsi,vOldType;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String GroupId = null;
        Master.Group.GroupInquiry Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void flag(String groupid, String mode)
        {
            GroupId = groupid;
            Mode = mode;
        }

        public GroupForm()
        {
            InitializeComponent();
        }

        private void GroupForm_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
            }
            else
            {
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[InventGroup] Where GroupId = '" + GroupId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtGroupId.Text = GroupId.ToString();
                    txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                    txtInventTypeId.Text = Dr["InventTypeId"].ToString();
                }
            }
            Conn.Close();
        }

        private void ModeNew()
        {
            txtGroupId.Enabled = false;
            txtDeskripsi.Enabled = true;
            txtInventTypeId.Enabled = true;

            btnSearch.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private bool Used()
        {
            Boolean vBol = false;
            try
            {
                Query = "Select * From [dbo].[InventSubGroup1] Where GroupId='" + GroupId + "'";
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

        private void ModeEdit()
        {
            if (Used() == true)
            {
                MessageBox.Show("Tidak boleh Edit, Group Name sudah pernah digunakan..");
            }
            else
            {
                txtGroupId.Enabled = false;
                txtDeskripsi.Enabled = true;
                txtInventTypeId.Enabled = true;

                btnSearch.Enabled = true;
                btnEdit.Visible = false;
                btnSave.Visible = true;
                btnExit.Visible = false;
                btnCancel.Visible = true;
            }            
        }

        public void ParentRefreshGrid(Master.Group.GroupInquiry f)
        {
            Parent = f;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
            vOldType = txtInventTypeId.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtGroupId.Enabled = false;
            txtDeskripsi.Enabled = false;
            txtInventTypeId.Enabled = false;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;

            txtDeskripsi.Text = vOldDeskripsi;
            txtInventTypeId.Text = vOldType;
        }

        private bool cekValidasi()
        {                              
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtDeskripsi.Text.Trim() == "")
            {
                ErrMsg = "Group Name harus diisi..";
                vBol = false;
            }
           
            if (vBol==true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventGroup] Where GroupId<>'" + txtGroupId.Text + "' AND Deskripsi = @Deskripsi ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text);
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Group Name sudah ada..";
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
                    Query = "Select * From [dbo].[InventGroup] Where GroupId='" + txtGroupId.Text + "'";
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
                if (Mode == "New")
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();
                    try
                    {
                        Query = "insert into [dbo].[InventGroup] (GroupId, Deskripsi, InventTypeId, CreatedDate, CreatedBy) OUTPUT INSERTED.GroupId ";
                        Query += "values ((SELECT RIGHT('00'+ISNULL(CAST((Max(GroupId)+1) AS VARCHAR(2)),1),2) from InventGroup), @deskripsi, @InventTypeId, getDate(), '" + ControlMgr.UserId + "');";
                        using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text.Trim().ToUpper());
                            Cmd.Parameters.AddWithValue("@InventTypeId", txtInventTypeId.Text);
                            txtGroupId.Text = Cmd.ExecuteScalar().ToString();
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
                    MessageBox.Show("Data " + txtGroupId.Text + " - " + txtDeskripsi.Text + ", berhasil ditambahkan..");

                    Form groupinquiry = Application.OpenForms["GroupInquiry"];
                    if (groupinquiry != null)
                        Parent.RefreshGrid();

                    this.Close();
                }
                else if (Mode == "Edit")
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();
                    try
                    {
                        Query = "update [dbo].[InventGroup] set Deskripsi =@Deskripsi ,InventTypeId =@InventTypeId, UpdatedDate = getDate(), UpdatedBy = '" + ControlMgr.UserId + "' where GroupId =@GroupId ;";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.Parameters.AddWithValue("@Deskripsi", txtDeskripsi.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@InventTypeId", txtInventTypeId.Text);
                        Cmd.Parameters.AddWithValue("@GroupId", txtGroupId.Text);
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
                    MessageBox.Show("Data " + txtGroupId.Text + " , berhasil diupdate.");
                    Parent.RefreshGrid();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally { }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "InventTypeID";
            tmpSearch.Order = "InventTypeID";
            tmpSearch.Table = "[dbo].[InventType]";
            tmpSearch.QuerySearch = "SELECT * FROM [dbo].[InventType] a";
            tmpSearch.FilterText = new string[] { "InventTypeID", "Deskripsi", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "Type", "Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
            tmpSearch.Select = new string[] { "InventTypeID" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtInventTypeId.Text = ConnectionString.Kodes[0];
                ConnectionString.Kodes = null;
            }
        }

        private void txtInventTypeId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                Ambil_Type();
            }
        }

        private void Ambil_Type()
        {
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM InventType WHERE InventTypeId=@Type";
            using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
            {
                cmd.Parameters.AddWithValue("@Type", txtInventTypeId.Text);
                Dr = cmd.ExecuteReader();

                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        txtInventTypeId.Text = Convert.IsDBNull(Dr["InventTypeId"]) ? "" : (string)Dr["InventTypeId"];
                    }
                }
                else
                {
                    MessageBox.Show("Type " + txtInventTypeId.Text + " doesn't exist");
                }
                Dr.Close();
                ConMaster.Close();
            }
        }

        private void txtInventTypeId_Leave(object sender, EventArgs e)
        {
            if(txtInventTypeId.Text.Trim()!="")
            {
                Cek_Type();
            }
        }

        private void Cek_Type()
        {
            Boolean vBolFound = false;
          
            ConMaster = ConnectionString.GetConnection();
            strSql = "SELECT * FROM InventType WHERE InventTypeId=@Type";
            using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
            {
                cmd.Parameters.AddWithValue("@Type", txtInventTypeId.Text);
                Dr = cmd.ExecuteReader();
                if (!Dr.HasRows)
                {
                    MessageBox.Show("Type " + txtInventTypeId.Text + " doesn't exist");

                    string SchemaName = "dbo";
                    string TableName = "InventType";

                    Search tmpSearch = new Search();
                    tmpSearch.SetSchemaTable(SchemaName, TableName);
                    tmpSearch.Text = "Search Type";
                    tmpSearch.ShowDialog();


                    if (ConnectionString.Kode != "")
                    {
                        txtInventTypeId.Text = ConnectionString.Kode;
                    }
                    ConnectionString.Kode="";
                }
                else
                {
                    vBolFound = true;
                }
                Dr.Close();
                ConMaster.Close();

                if (vBolFound)
                {
                    Ambil_Type();
                }
                else
                {
                    txtInventTypeId.Focus();
                }
            }
        }


    }
}
