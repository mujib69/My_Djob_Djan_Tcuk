using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.User.GroupMenu
{
    public partial class GroupMenuForm : Form
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
        public string TableName = "GroupMenu";
        public Boolean EditStatus = false;
        public GroupMenuForm()
        {
            InitializeComponent();
        }

        //Mode
        string Variable = "";
        //Check Primary Id
        string CheckPKId = "";
        //Check Parent Class
        Master.User.GroupMenu.GroupMenuDashboard Parent;

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

            //OleDbCommand cmd = new OleDbCommand("Select Code,Description,Qnty,Rate from PurchaseTable'", con);
            //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            //da.Fill(dtusers);
            //dataGridView1.DataSource = dtusers;
            //dataGridView1.Columns[0].Name = "Code ";

            //DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn("colBestBefore", typeof(DateTime)));
            //dt.Columns.Add(new DataColumn("colStatus", typeof(string)));
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
                
                //Jika save/edit
                Query = "Delete from [" + SchemaName + "].[" + TableName + "] where GroupId='" + CheckPKId + "';";
                for (int i = 0; i <= dgvList.RowCount-1; i++)
                {
                    Query += "Insert into [" + SchemaName + "].[" + TableName + "] (GroupId,GroupName,MenuId,MenuName,Status,CreatedDate,CreatedBy) values ('" + txtGroupId.Text.Trim() + "','" + txtGroupName.Text.Trim() + "','" + (dgvList.Rows[i].Cells["MenuId"].Value == null ? "" : dgvList.Rows[i].Cells["MenuId"].Value.ToString()) + "','" + (dgvList.Rows[i].Cells["MenuName"].Value == null ? "" : dgvList.Rows[i].Cells["MenuName"].Value.ToString()) + "','" + (dgvList.Rows[i].Cells["Status"].Value == null ? "false" : dgvList.Rows[i].Cells["Status"].Value.ToString()) + "',getdate(),'" + ControlMgr.UserId + "');";
                }
                Trans = Conn.BeginTransaction();
                using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                {
                    Command.ExecuteScalar();
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Group Id : " + txtGroupId.Text.Trim() + " berhasil ditambahkan.");
                Parent.RefreshGrid();
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

        public void ParentRefreshGrid(Master.User.GroupMenu.GroupMenuDashboard F)
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

                btnSave.Visible = false;
                btnEdit.Visible = true;
                btnExit.Visible = true;

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
                    }
                    Dr.Close();
                }

                Query = "Select a.MenuId,a.MenuName, [Status] From [User].[GroupMenu] a ";
                Query += "where GroupId='" + ConnectionString.IdSearchPK + "'";
                DataTable Dt = new DataTable();
                using (SqlDataAdapter Da = new SqlDataAdapter(Query, Conn))
                {
                    Da.Fill(Dt);
                    dgvList.DataSource = Dt;
                }

                txtGroupId.Enabled = false;
                dgvList.ReadOnly = true;
                dgvList.DefaultCellStyle.BackColor = Color.LightGray;
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
            try
            {
                Variable = "New";

                Conn = ConnectionString.GetConnection();
                Query = "Select a.MenuId,a.MenuName, CAST(0 AS BIT) [Status] From [User].[Menu] a ";
                Query += "Group By a.MenuId,a.MenuName";
                DataTable Dt = new DataTable();
                using (SqlDataAdapter Da = new SqlDataAdapter(Query, Conn))
                {
                    Da.Fill(Dt);
                    dgvList.DataSource = Dt;
                }

                dgvList.ReadOnly = false;
                dgvList.Columns["MenuId"].ReadOnly = true;
                dgvList.Columns["MenuName"].ReadOnly = true;

                btnSave.Visible = true;
                btnEdit.Visible = false;
                btnExit.Visible = true;
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

        private void ModeEdit()
        {
            Variable = "Edit";
            dgvList.ReadOnly = false;
            dgvList.Columns["MenuId"].ReadOnly = true;
            dgvList.Columns["MenuName"].ReadOnly = true;
            dgvList.DefaultCellStyle.BackColor = Color.White;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnExit.Visible = true;
        }

        private void btnSearchUserId_Click(object sender, EventArgs e)
        {
            string SchemaName = "User";
            string TableName = "Group";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtGroupId.Text = ConnectionString.Kode;
            txtGroupName.Text = ConnectionString.Kode2;
        }
    
    }
}
