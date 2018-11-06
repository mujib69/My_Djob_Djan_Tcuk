using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.User.Group
{
    public partial class GroupForm : Form
    {
        public static string IdPrimary = "";
        //SQL Function
        private SqlConnection Conn;
        private SqlTransaction Trans;

        //private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        private int Index;

        //Param RefreshGrid
        public Form tmpForm;
        public string SchemaName = "User";
        public string TableName = "Group";
        public Boolean EditStatus = false;
        public GroupForm()
        {
            InitializeComponent();
        }

        //Mode
        string Variable = "";
        //Check Primary Id
        string CheckPKId = "";
        //Check Parent Class
        Master.User.Group.GroupDashboard Parent;

        private void Edit_Load(object sender, EventArgs e)
        {
            try
            {
                this.Location = new Point(148, 47);
                this.Text = "Form " + SchemaName + " " + TableName;
                CheckPKId = ConnectionString.IdSearchPK;
                if (ConnectionString.IdSearchPK != "") //Detect apakah new atau view
                {
                    ModeView();
                }
                else
                {
                    ModeNew();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Validasi jika kosong
                if (txtGroupId.Text == "")
                {
                    MessageBox.Show("Data Group Id tidak boleh kosong.");
                    return;
                }
                else if (txtGroupName.Text == "")
                {
                    MessageBox.Show("Data Group Name tidak boleh kosong.");
                    return;
                }

                //Cek Data apakah sudah ada
                string CekData;
                Conn = ConnectionString.GetConnection();

                Query = "Select GroupId from [" + SchemaName + "].[" + TableName + "] where GroupId='" + txtGroupId.Text.Trim() + "' and GroupId<>'" + CheckPKId + "'";
                
                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    CekData = Command.ExecuteScalar() == null ? "" : Command.ExecuteScalar().ToString();
                }


                //Jika UserId sudah ada
                if (CekData != "")
                {
                    MessageBox.Show("Group Id sudah digunakan, silahkan diganti dengan yang lain.");
                    Conn.Close();
                    return;
                }

                //Jika save baru
                if (Variable == "New")
                {
                    Query = "Insert into [" + SchemaName + "].[" + TableName + "] (GroupId,GroupName,GroupDesc,CreatedDate,CreatedBy) values ('" + txtGroupId.Text.Trim() + "','" + txtGroupName.Text.Trim() + "','" + txtGroupDesc.Text.Trim() + "',getdate(),'" + ControlMgr.UserId + "')";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("Group Id : " + txtGroupId.Text.Trim() + " berhasil ditambahkan.");
                }

                //Jika edit data
                else if (Variable == "Edit")
                {
                    Query = "Update [" + SchemaName + "].[" + TableName + "] set GroupId='" + txtGroupId.Text.Trim() + "', GroupName='" + txtGroupName.Text.Trim() + "', GroupDesc='" + txtGroupDesc.Text.Trim() + "', UpdatedDate =getdate(), UpdatedBy ='" + ControlMgr.UserId + "' where GroupId='" + CheckPKId + "'";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("Group Id : " + txtGroupId.Text.Trim() + " berhasil diedit.");
                }
                Conn.Close();
                this.Close();
                Parent.RefreshGrid();
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }       
        }

        public void ParentRefreshGrid(Master.User.Group.GroupDashboard F)
        {
            Parent = F;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void ModeView()
        {
            try
            {
                Variable = "View";
                txtGroupId.Enabled = false;
                txtGroupName.Enabled = false;
                txtGroupDesc.Enabled = false;

                btnSave.Visible = false;
                btnEdit.Visible = true;
                btnExit.Visible = true;

                Variable = "Edit";
                string tmpFieldPK = "";
                string tmpDisplay = "";
                int i = 0;

                //Get PK
                Conn = ConnectionString.GetConnection();
                Query = "Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = '" + TableName + "'" + " and PK=1 order by OrderNo";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 0;

                    while (Dr.Read())
                    {
                        if (i == 0)
                        {
                            //IdName = Dr[0].ToString();
                            tmpFieldPK = Dr[0].ToString();
                        }
                        else
                        {
                            tmpFieldPK += " or " + Dr[0].ToString();
                        }
                        i += 1;
                    }
                    Dr.Close();
                }

                //Get Field Display
                Conn = ConnectionString.GetConnection();
                Query = "Select FieldName from [User].[Table] where SchemaName='" + SchemaName + "' and TableName = '" + TableName + "'" + " order by OrderNo";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    i = 0;

                    while (Dr.Read())
                    {
                        if (i == 0)
                        {
                            tmpDisplay = Dr[0].ToString(); // + "='" + dgvSearch.Rows[Index].Cells[Dr[0].ToString()].Value.ToString() + "'";
                        }
                        else
                        {
                            tmpDisplay += "," + Dr[0].ToString(); //+ "='" + dgvSearch.Rows[Index].Cells[Dr[0].ToString()].Value.ToString() + "'";
                        }
                        i += 1;
                    }
                    Dr.Close();
                }

                //Get Data
                Query = "Select " + tmpDisplay + " from [" + SchemaName + "].[" + TableName + "] where " + tmpFieldPK + "='" + ConnectionString.IdSearchPK + "';";
                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();

                    while (Dr.Read())
                    {
                        txtGroupId.Text = Dr["GroupId"].ToString();
                        txtGroupName.Text = Dr["GroupName"].ToString();
                        txtGroupDesc.Text = Dr["GroupDesc"].ToString();
                    }
                    Dr.Close();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }
        }

        private void ModeNew()
        {
            Variable = "New";
            txtGroupId.Enabled = true;
            txtGroupName.Enabled = true;
            txtGroupDesc.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnExit.Visible = true;
        }

        private void ModeEdit()
        {
            Variable = "Edit";
            txtGroupId.Enabled = true;
            txtGroupName.Enabled = true;
            txtGroupDesc.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnExit.Visible = true;
        }
        
    }
}
