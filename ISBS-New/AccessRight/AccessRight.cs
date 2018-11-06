using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace ISBS_New.AccessRight
{
    partial class AccessRight : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private string Query = null;
        private string oldPassword;
        private string oldGroupID;
        private string actionType;
        private bool userGroupStatus = false;
        private string MenuName = null;

        //begin
        //created by : joshua
        //created date : 26 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public AccessRight()
        {
            InitializeComponent();          
        }

        private void AccessRight_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
           
            ResetTabUserID();
            ResetTabGroupID();
            ResetTabUserGroup();
            ResetTabAuthority();
            ResetTabCopyAccessRight();
        }       

        private void AccessRight_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetTabUserID();
            ResetTabGroupID();
            ResetTabUserGroup();
            ResetTabAuthority();
            ResetTabCopyAccessRight();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion


        #region Tab UserID
        private void ResetTabUserID()
        {
            txtUserID.Enabled = false;
            txtPassword.Enabled = false;
            txtFullName.Enabled = false;
            txtAtasanKerja.Enabled = false;
            cbStatus.Enabled = false;
            cbStatus.Checked = false;
            btnCancelUserID.Enabled = false;
            btnSaveUserID.Enabled = false;
            btnNewUserID.Enabled = true;
            btnDeleteUserID.Enabled = true;
            btnEditUserID.Enabled = true;
            btnPopUpUserID.Enabled = true;
            txtUserID.Text = "";
            txtPassword.Text = "";
            txtFullName.Text = "";
            txtAtasanKerja.Text = "";
            oldPassword = "";
            actionType = "";

        }

        private void btnNewUserID_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                txtUserID.Text = "";
                txtPassword.Text = "";
                txtFullName.Text = "";
                txtAtasanKerja.Text = "";
                actionType = "new";
                cbStatus.Checked = false;
                txtUserID.Enabled = true;
                txtPassword.Enabled = true;
                txtFullName.Enabled = true;
                txtAtasanKerja.Enabled = true;
                cbStatus.Enabled = true;
                btnCancelUserID.Enabled = true;
                btnSaveUserID.Enabled = true;
                btnNewUserID.Enabled = false;
                btnDeleteUserID.Enabled = false;
                btnEditUserID.Enabled = false;
                btnPopUpUserID.Enabled = false;
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnCancelUserID_Click(object sender, EventArgs e)
        {
            ResetTabUserID();
        }

        private void btnSaveUserID_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (actionType == "new")
                {
                    if (txtUserID.Text == "")
                    {
                        MessageBox.Show("User ID tidak boleh kosong.");
                        return;
                    }
                    else if (CheckExistingUserID(txtUserID.Text) > 0)
                    {
                        MessageBox.Show("User ID sudah ada di database.");
                        return;
                    }
                    else if (txtPassword.Text == "")
                    {
                        MessageBox.Show("Password tidak boleh kosong.");
                        return;
                    }
                    
                    bool status = false;
                    if (cbStatus.Checked)
                    {
                        status = true;
                    }

                    Query = "INSERT INTO sysPass (UserID, PasswordUser, Aktif, FullName, Head_Department) VALUES ";
                    Query += "('" + txtUserID.Text + "',";
                    Query += "'" + txtPassword.Text + "',";
                    Query += "'" + status + "',";
                    Query += "'" + txtFullName.Text + "',";
                    Query += "'" + txtAtasanKerja.Text + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data User ID : " + txtUserID.Text + " berhasil disimpan.");
                    ResetTabUserID();
                }
                else {
                    if (txtUserID.Text == "")
                    {
                        MessageBox.Show("Pilih data yang akan diedit.");
                        return;
                    }
                    else if (CheckExistingUserID(txtUserID.Text) == 0)
                    {
                        MessageBox.Show("Data tidak ada di database.");
                        return;
                    }
                    else if (txtUserID.Text == "")
                    {
                        MessageBox.Show("User ID tidak boleh kosong.");
                        return;
                    }
                    else if (txtPassword.Text == "")
                    {
                        MessageBox.Show("Password tidak boleh kosong.");
                        return;
                    }
                    Query = "SELECT a.UserID AS UserId, b.GroupName AS UserGroup,Aktif,a.[PasswordUser] FROM  [ISBS-NEW4].[dbo].[sysPass] a INNER JOIN [ISBS-NEW4].[dbo].[sysUserGroup] b ON b.UserID = a.UserID WHERE a.UserID = '" + txtUserID.Text + "'";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Dr = Cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            if (txtPassword.Text == Dr["PasswordUser"].ToString())
                            {
                                MessageBox.Show("Password baru tidak boleh sama dengan password lama.");
                                Dr.Close();
                                return;
                            }
                        }
                    }

                    bool status = false;
                    if (cbStatus.Checked)
                    {
                        status = true;
                    }

                    DateTime? PwdDateChange = null;
                    if (txtPassword.Text != oldPassword)
                    {
                        PwdDateChange = DateTime.Now;
                    }

                    DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "User ID = " + txtUserID.Text + Environment.NewLine + "Akan diubah ? ", "Ubah Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Query = "UPDATE sysPass SET ";
                        Query += "PasswordUser = '" + txtPassword.Text + "', ";
                        if (PwdDateChange != null)
                        {
                            Query += "PwdDateChange = '" + PwdDateChange + "', ";
                        }
                        Query += "Aktif = '" + status + "', ";
                        Query += "FullName = '" + txtFullName.Text + "', ";
                        Query += "Head_Department = '" + txtAtasanKerja.Text + "' ";
                        Query += "WHERE UserID = '" + txtUserID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Trans.Commit();
                        MessageBox.Show("Data User ID : " + txtUserID.Text + " berhasil diupdate.");
                        ResetTabUserID();
                    }
                }                
            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private int CheckExistingUserID(string prmUserID)
        {
            int result = 0;
            Query = "SELECT COUNT(UserID) CountData FROM sysPass ";
            Query += "WHERE UPPER(UserID) = '" + prmUserID.ToUpper() + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToInt32(Dr["CountData"]);
            }
            Dr.Close();

            return result;
        }

        private void btnEditUserID_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                if (txtUserID.Text == "")
                {
                    MessageBox.Show("Pilih data yang akan diedit.");
                    return;
                }

                actionType = "edit";
                txtUserID.Enabled = false;
                txtPassword.Enabled = true;
                txtFullName.Enabled = true;
                txtAtasanKerja.Enabled = true;
                cbStatus.Enabled = true;
                btnCancelUserID.Enabled = true;
                btnSaveUserID.Enabled = true;
                btnNewUserID.Enabled = false;
                btnDeleteUserID.Enabled = false;
                btnEditUserID.Enabled = false;
                btnPopUpUserID.Enabled = true;
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnPopUpUserID_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "UserID";
            tmpSearch.Order = "UserId Asc";
            tmpSearch.Table = "[dbo].[sysPass]";
            tmpSearch.QuerySearch = "SELECT a.UserID, a.PasswordUser, a.FullName, a.Head_Department, a.Aktif FROM [dbo].[sysPass] a";
            tmpSearch.FilterText = new string[] { "UserID", "FullName" };
            tmpSearch.Select = new string[] { "UserID", "PasswordUser", "FullName", "Head_Department", "Aktif" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtUserID.Text = ConnectionString.Kodes[0];
                oldPassword = ConnectionString.Kodes[1];
                txtPassword.Text = ConnectionString.Kodes[1];
                txtFullName.Text = ConnectionString.Kodes[2];
                txtAtasanKerja.Text = ConnectionString.Kodes[3];
                if (ConnectionString.Kodes[4] == "True")
                {
                    cbStatus.Checked = true;
                }
                else
                {
                    cbStatus.Checked = false;
                }
                ConnectionString.Kodes = null;
            }
        }

        private void btnDeleteUserID_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    if (txtUserID.Text == "")
                    {
                        MessageBox.Show("Pilih data yang akan dihapus.");
                        return;
                    }
                    else if (CheckExistingUserID(txtUserID.Text) == 0)
                    {
                        MessageBox.Show("Data tidak ada di database.");
                        return;
                    }
                    else if (txtUserID.Text == "")
                    {
                        MessageBox.Show("User ID tidak boleh kosong.");
                        return;
                    }
                    else if (CheckExistingUserIDInUserGroup(txtUserID.Text) > 0)
                    {
                        MessageBox.Show("Data tidak dapat dihapus karena memiliki relasi.");
                        return;
                    }

                    DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "User ID = " + txtUserID.Text + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Query = "DELETE FROM sysPass ";
                        Query += "WHERE UserID = '" + txtUserID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Trans.Commit();
                        MessageBox.Show("Data User ID : " + txtUserID.Text + " berhasil dihapus.");
                        ResetTabUserID();
                    }

                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private int CheckExistingUserIDInUserGroup(string prmUserID)
        {
            int result = 0;
            Query = "SELECT COUNT(UserID) CountData FROM sysUserGroup ";
            Query += "WHERE UPPER(UserID) = '" + prmUserID.ToUpper() + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToInt32(Dr["CountData"]);
            }
            Dr.Close();

            return result;
        }
        #endregion


        #region Tab GroupID
        private int CheckExistingGroupID(string prmGroupID)
        {
            int result = 0;
            Query = "SELECT COUNT(GroupName) CountData FROM sysGroupMr ";
            Query += "WHERE UPPER(GroupName) = '" + prmGroupID.ToUpper() + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToInt32(Dr["CountData"]);
            }
            Dr.Close();

            return result;
        }

        private void SetlbGroupID()
        {
            lbGroupID.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Query = "SELECT GroupName FROM sysGroupMr";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lbGroupID.Items.Add(Convert.ToString(Dr["GroupName"]));
            }
            Dr.Close();
            Conn.Close();
        }

        private void ResetTabGroupID()
        {
            txtGroupID.Text = "";
            actionType = "";
            oldGroupID = "";
            txtGroupID.Enabled = false;
            btnCancelGroupID.Enabled = false;
            btnSaveGroupID.Enabled = false;
            btnEditGroupID.Enabled = true;
            btnDeleteGroupID.Enabled = true;
            btnNewGroupID.Enabled = true;
            SetlbGroupID();
        }

        private void btnNewGroupID_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                txtGroupID.Text = "";
                txtGroupID.Enabled = true;
                btnCancelGroupID.Enabled = true;
                btnSaveGroupID.Enabled = true;
                btnEditGroupID.Enabled = false;
                btnDeleteGroupID.Enabled = false;
                btnNewGroupID.Enabled = false;
                actionType = "new";
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnCancelGroupID_Click(object sender, EventArgs e)
        {
            ResetTabGroupID();
        }

        private void btnSaveGroupID_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (actionType == "new")
                {
                    if (txtGroupID.Text == "")
                    {
                        MessageBox.Show("User ID tidak boleh kosong.");
                        return;
                    }
                    else if (CheckExistingGroupID(txtGroupID.Text) > 0)
                    {
                        MessageBox.Show("Group ID sudah ada di database.");
                        return;
                    }

                    Query = "INSERT INTO sysGroupMr (GroupName) VALUES ";
                    Query += "('" + txtGroupID.Text + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data Group ID : " + txtGroupID.Text + " berhasil disimpan.");
                    ResetTabGroupID();
                }
                else
                {
                    if (txtGroupID.Text == "")
                    {
                        MessageBox.Show("User ID tidak boleh kosong.");
                        return;
                    }
                    else if (CheckExistingGroupID(oldGroupID) == 0)
                    {
                        MessageBox.Show("Group ID tidak ada di database.");
                        return;
                    }
                    else if (CheckExistingGroupIDInUserGroup(oldGroupID) > 0)
                    {
                        MessageBox.Show("Data tidak dapat diubah karena memiliki relasi.");
                        return;
                    }

                    DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "Group ID = " + oldGroupID + Environment.NewLine + "Akan diubah ? ", "Ubah Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Query = "UPDATE sysGroupMr SET GroupName = '" + txtGroupID.Text + "' WHERE GroupName = '" + oldGroupID + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Trans.Commit();
                        MessageBox.Show("Data Group ID : " + oldGroupID + " berhasil diubah.");
                        ResetTabGroupID();
                    }
                }

            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void btnEditGroupID_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                oldGroupID = Convert.ToString(lbGroupID.SelectedItem);
                txtGroupID.Text = oldGroupID;
                if (txtGroupID.Text == "")
                {
                    MessageBox.Show("Pilih data yang akan diedit.");
                    return;
                }
                txtGroupID.Enabled = true;
                btnCancelGroupID.Enabled = true;
                btnSaveGroupID.Enabled = true;
                btnEditGroupID.Enabled = false;
                btnDeleteGroupID.Enabled = false;
                btnNewGroupID.Enabled = false;
                actionType = "edit";
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void btnDeleteGroupID_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    oldGroupID = Convert.ToString(lbGroupID.SelectedItem);
                    if (oldGroupID == "")
                    {
                        MessageBox.Show("Pilih data yang akan dihapus.");
                        return;
                    }
                    else if (CheckExistingGroupIDInUserGroup(oldGroupID) > 0)
                    {
                        MessageBox.Show("Data tidak dapat dihapus karena memiliki relasi.");
                        return;
                    }

                    txtGroupID.Enabled = false;
                    btnCancelGroupID.Enabled = true;
                    btnSaveGroupID.Enabled = false;
                    btnEditGroupID.Enabled = false;
                    btnDeleteGroupID.Enabled = true;
                    btnNewGroupID.Enabled = false;

                    DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "Group ID = " + oldGroupID + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Query = "DELETE FROM sysGroupMr ";
                        Query += "WHERE GroupName = '" + oldGroupID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Trans.Commit();
                        MessageBox.Show("Data Group ID : " + oldGroupID + " berhasil dihapus.");
                        ResetTabGroupID();
                    }

                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private int CheckExistingGroupIDInUserGroup(string prmGroupID)
        {
            int result = 0;
            Query = "SELECT COUNT(GroupName) CountData FROM sysUserGroup ";
            Query += "WHERE UPPER(GroupName) = '" + prmGroupID.ToUpper() + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToInt32(Dr["CountData"]);
            }
            Dr.Close();

            return result;
        }
        #endregion


        #region Tab UserGroup
        private void SetcbUserID()
        {
            cbUserID.Items.Clear();
            cbUserID.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT UserID FROM sysPass WHERE Aktif = 1";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbUserID.Items.Add(Convert.ToString(Dr["UserID"]));
            }
            Dr.Close();
            Conn.Close();
        }

        private void SetcbGroupID()
        {
            cbGroupID.Items.Clear();
            cbGroupID.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT GroupName FROM sysGroupMr";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbGroupID.Items.Add(Convert.ToString(Dr["GroupName"]));
            }
            Dr.Close();
            Conn.Close();
        }

        private void cbUserID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string UserID = Convert.ToString(cbUserID.SelectedItem);

            string GroupID = CheckGroupName(UserID);
            if (GroupID == "")
            {
                MessageBox.Show("Group ID belum ditentukan");
                cbGroupID.SelectedItem = "-select-";
                userGroupStatus = false;
                btnDeleteUserGroup.Enabled = false;
            }
            else
            {
                cbGroupID.SelectedItem = GroupID;
                userGroupStatus = true;
                btnDeleteUserGroup.Enabled = true;
            }

        }

        private string CheckGroupName(string prmUserID)
        {
            string result = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT GroupName FROM sysUserGroup WHERE UserID = '" + prmUserID + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToString(Dr["GroupName"]);
            }
            Dr.Close();
            Conn.Close();

            return result;
        }

        private void btnSaveUserGroup_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();                 

                if (userGroupStatus == false)
                {
                    //begin
                    //updated by : joshua
                    //updated date : 26 feb 2018
                    //description : check permission access
                    if (this.PermissionAccess(ControlMgr.New) > 0)
                    {
                        if (Convert.ToString(cbUserID.SelectedItem) == "" || Convert.ToString(cbUserID.SelectedItem) == "-select-")
                        {
                            MessageBox.Show("User ID tidak boleh kosong.");
                            return;
                        }
                        else if (Convert.ToString(cbGroupID.SelectedItem) == "" || Convert.ToString(cbGroupID.SelectedItem) == "-select-")
                        {
                            MessageBox.Show("Group ID tidak boleh kosong.");
                            return;
                        }

                        Query = "INSERT INTO sysUserGroup (UserID, GroupName) VALUES ";
                        Query += "('" + Convert.ToString(cbUserID.SelectedItem) + "', '" + Convert.ToString(cbGroupID.SelectedItem) + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Trans.Commit();
                        MessageBox.Show("Data User ID : " + Convert.ToString(cbUserID.SelectedItem) + " dengan Group ID " + Convert.ToString(cbGroupID.SelectedItem) + " berhasil disimpan.");
                        ResetTabUserGroup();
                    }
                    else
                    {
                        MessageBox.Show(ControlMgr.PermissionDenied);
                    }
                    //end                    
                }
                else
                {
                    //begin
                    //updated by : joshua
                    //updated date : 26 feb 2018
                    //description : check permission access
                    if (this.PermissionAccess(ControlMgr.Edit) > 0)
                    {
                        if (Convert.ToString(cbUserID.SelectedItem) == "" || Convert.ToString(cbUserID.SelectedItem) == "-select-")
                        {
                            MessageBox.Show("User ID tidak boleh kosong.");
                            return;
                        }
                        else if (Convert.ToString(cbGroupID.SelectedItem) == "" || Convert.ToString(cbGroupID.SelectedItem) == "-select-")
                        {
                            MessageBox.Show("Group ID tidak boleh kosong.");
                            return;
                        }

                        Query = "UPDATE sysUserGroup SET GroupName = '" + Convert.ToString(cbGroupID.SelectedItem) + "' WHERE UserID = '" + Convert.ToString(cbUserID.SelectedItem) + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();


                        Trans.Commit();
                        MessageBox.Show("Data User ID : " + Convert.ToString(cbUserID.SelectedItem) + " dengan Group ID " + Convert.ToString(cbGroupID.SelectedItem) + " berhasil disimpan.");
                        ResetTabUserGroup();
                    }
                    else
                    {
                        MessageBox.Show(ControlMgr.PermissionDenied);
                    }
                    //end                     
                }

            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void ResetTabUserGroup()
        {
            if (tabControl1.SelectedIndex == 2)
            {
                SetcbUserID();
                SetcbGroupID();
                userGroupStatus = false;
                cbUserID.DropDownStyle = ComboBoxStyle.DropDownList;
                cbGroupID.DropDownStyle = ComboBoxStyle.DropDownList;
                btnDeleteUserGroup.Enabled = false;
            }
        }

        private void btnDeleteUserGroup_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "User ID = " + Convert.ToString(cbUserID.SelectedItem) + Environment.NewLine + "Group ID = " + Convert.ToString(cbGroupID.SelectedItem) + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Query = "DELETE FROM sysUserGroup ";
                    Query += "WHERE UserID = '" + Convert.ToString(cbUserID.SelectedItem) + "' AND GroupName = '" + Convert.ToString(cbGroupID.SelectedItem) + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data User ID : " + Convert.ToString(cbUserID.SelectedItem) + " Group ID : " + Convert.ToString(cbGroupID.SelectedItem) + " berhasil dihapus.");
                    ResetTabUserGroup();
                }

            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }
        }
        #endregion


        #region Tab Authority
        private void ResetTabAuthority()
        {
            if (tabControl1.SelectedIndex == 3)
            {
                SetcbGroupIDAuthority();
                cbGroupIDAuthority.SelectedItem = "";
                lbAuthority.SelectedItem = "";
                MenuName = "";
                lbUsers.Items.Clear();
                lbAuthority.Items.Clear();               
                ckView.Checked = false;
                ckNew.Checked = false;
                ckEdit.Checked = false;
                ckDelete.Checked = false;
                ckApprove.Checked = false;
                cbGroupIDAuthority.DropDownStyle = ComboBoxStyle.DropDownList;
                btnAccountPayable.BackColor = Color.LightGray;
                btnAccountPayable.ForeColor = Color.Black;
                btnPurchasing.BackColor = Color.LightGray;
                btnPurchasing.ForeColor = Color.Black;
                btnSales.BackColor = Color.LightGray;
                btnSales.ForeColor = Color.Black;
                btnInventory.BackColor = Color.LightGray;
                btnInventory.ForeColor = Color.Black;
                btnMaster.BackColor = Color.LightGray;
                btnMaster.ForeColor = Color.Black;
                btnAdministrator.BackColor = Color.LightGray;
                btnAdministrator.ForeColor = Color.Black;
            }
        }

        private void SetcbGroupIDAuthority()
        {
            cbGroupIDAuthority.Items.Clear();
            cbGroupIDAuthority.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT GroupName FROM sysGroupMr";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbGroupIDAuthority.Items.Add(Convert.ToString(Dr["GroupName"]));
            }
            Dr.Close();
            Conn.Close();
        }

        private void SetlbUsers(string prmGroupName)
        {
            lbUsers.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Query = "SELECT a.UserID FROM sysUserGroup a INNER JOIN sysPass b ON b.UserID = a.UserID WHERE a.GroupName = '" + prmGroupName + "' AND b.Aktif = 1";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lbUsers.Items.Add(Convert.ToString(Dr["UserID"]));
            }
            Dr.Close();
            Conn.Close();
        }

        private void lbAuthority_SelectedIndexChanged(object sender, EventArgs e)
        {
            string GroupName = Convert.ToString(cbGroupIDAuthority.SelectedItem);
            SetlbUsers(GroupName);

            string SubMenuName = Convert.ToString(lbAuthority.SelectedItem);
            SetAuthorityName(GroupName, SubMenuName, MenuName);
        }

        private void btnPurchasing_Click(object sender, EventArgs e)
        {
            MenuName = "Purchase";
            SetlbAuthority(MenuName);
            btnPurchasing.BackColor = Color.DeepSkyBlue;
            btnPurchasing.ForeColor = Color.White;
            btnSales.BackColor = Color.LightGray;
            btnSales.ForeColor = Color.Black;
            btnInventory.BackColor = Color.LightGray;
            btnInventory.ForeColor = Color.Black;
            btnMaster.BackColor = Color.LightGray;
            btnMaster.ForeColor = Color.Black;
            btnAdministrator.BackColor = Color.LightGray;
            btnAdministrator.ForeColor = Color.Black;
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            MenuName = "Sales";
            SetlbAuthority(MenuName);;
            btnAccountPayable.BackColor = Color.LightGray;
            btnAccountPayable.ForeColor = Color.Black;
            btnPurchasing.BackColor = Color.LightGray;
            btnPurchasing.ForeColor = Color.Black;  
            btnSales.BackColor = Color.DeepSkyBlue;
            btnSales.ForeColor = Color.White;
            btnInventory.BackColor = Color.LightGray;
            btnInventory.ForeColor = Color.Black;
            btnMaster.BackColor = Color.LightGray;
            btnMaster.ForeColor = Color.Black;
            btnAdministrator.BackColor = Color.LightGray;
            btnAdministrator.ForeColor = Color.Black;
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            MenuName = "Inventory";
            SetlbAuthority(MenuName);
            btnAccountPayable.BackColor = Color.LightGray;
            btnAccountPayable.ForeColor = Color.Black;
            btnPurchasing.BackColor = Color.LightGray;
            btnPurchasing.ForeColor = Color.Black;
            btnSales.BackColor = Color.LightGray;
            btnSales.ForeColor = Color.Black;
            btnInventory.BackColor = Color.DeepSkyBlue;
            btnInventory.ForeColor = Color.White;
            btnMaster.BackColor = Color.LightGray;
            btnMaster.ForeColor = Color.Black;
            btnAdministrator.BackColor = Color.LightGray;
            btnAdministrator.ForeColor = Color.Black;
        }

        private void btnAdministrator_Click(object sender, EventArgs e)
        {
            MenuName = "Administrator";
            SetlbAuthority(MenuName);
            btnAccountPayable.BackColor = Color.LightGray;
            btnAccountPayable.ForeColor = Color.Black;
            btnPurchasing.BackColor = Color.LightGray;
            btnPurchasing.ForeColor = Color.Black;
            btnSales.BackColor = Color.LightGray;
            btnSales.ForeColor = Color.Black;
            btnInventory.BackColor = Color.LightGray;
            btnInventory.ForeColor = Color.Black;
            btnMaster.BackColor = Color.LightGray;
            btnMaster.ForeColor = Color.Black;
            btnAdministrator.BackColor = Color.DeepSkyBlue;
            btnAdministrator.ForeColor = Color.White;
        }

        private void SetlbAuthority(string prmMenuName)
        {
            lbAuthority.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Query = "SELECT SubMenuName FROM sysMenu WHERE MenuName = '" + prmMenuName + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lbAuthority.Items.Add(Convert.ToString(Dr["SubMenuName"]));
            }
            Dr.Close();
            Conn.Close();
        }

        private void SetAuthorityName(string prmGroupName, string prmSubMenuName, string prmMenuName)
        {
            string SubMenuNameClass = "";                   
            ckView.Checked = false;
            ckNew.Checked = false;
            ckEdit.Checked = false;
            ckDelete.Checked = false;
            ckApprove.Checked = false;

            SubMenuNameClass = GetSubMenuNameClass(prmMenuName, prmSubMenuName);
            SetAuthorityAccess(SubMenuNameClass, prmGroupName);
            
        }

        private void btnSaveAuthority_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                string GroupName = Convert.ToString(cbGroupIDAuthority.SelectedItem);
                string AutorityNameView = ckView.Checked == true ? ControlMgr.View : "";
                string AutorityNameNew = ckNew.Checked == true ? ControlMgr.New : "";
                string AutorityNameEdit = ckEdit.Checked == true ? ControlMgr.Edit : "";
                string AutorityNameDelete = ckDelete.Checked == true ? ControlMgr.Delete : "";
                string AutorityNameApprove = ckApprove.Checked == true ? ControlMgr.Approve : "";
                string SubMenuName = Convert.ToString(lbAuthority.SelectedItem);
                string SubMenuNameClass = GetSubMenuNameClass(MenuName, SubMenuName);
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    if (GroupName == "" || GroupName == "-select-")
                    {
                        MessageBox.Show("Group ID tidak boleh kosong.");
                        return;
                    }
                    else if (SubMenuName == "")
                    {
                        MessageBox.Show("Menu Name tidak boleh kosong.");
                        return;
                    }
                    //else if (AutorityNameView == "" && AutorityNameNew == "" && AutorityNameEdit == "" && AutorityNameDelete == "")
                    //{
                    //    MessageBox.Show("Authority belum dipilih.");
                    //    return;
                    //}

                    Query = "DELETE FROM sysGroupAuthority WHERE GroupName = '" + cbGroupIDAuthority.SelectedItem.ToString() + "' AND SubMenuNameClass = '" + SubMenuNameClass + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    if (AutorityNameView != "")
                    {
                        InsertGroupAuthority(GroupName, AutorityNameView, SubMenuNameClass, Conn, Trans, Cmd);
                    }
                    if (AutorityNameNew != "")
                    {
                        InsertGroupAuthority(GroupName, AutorityNameNew, SubMenuNameClass, Conn, Trans, Cmd);
                    }
                    if (AutorityNameEdit != "")
                    {
                        InsertGroupAuthority(GroupName, AutorityNameEdit, SubMenuNameClass, Conn, Trans, Cmd);
                    }
                    if (AutorityNameDelete != "")
                    {
                        InsertGroupAuthority(GroupName, AutorityNameDelete, SubMenuNameClass, Conn, Trans, Cmd);
                    }
                    if (AutorityNameApprove != "")
                    {
                        InsertGroupAuthority(GroupName, AutorityNameApprove, SubMenuNameClass, Conn, Trans, Cmd);
                    }

                    Trans.Commit();
                    MessageBox.Show("Data Group ID : " + Convert.ToString(cbGroupIDAuthority.SelectedItem) + " dengan Menu Name " + Convert.ToString(lbAuthority.SelectedItem) + " berhasil disimpan.");
                    //ResetTabAuthority();
                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private string GetSubMenuNameClass(string prmMenuName, string prmSubMenuName)
        {
            string result = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT SubMenuNameClass FROM sysMenu WHERE MenuName = '" + prmMenuName + "' AND SubMenuName = '" + prmSubMenuName + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToString(Dr["SubMenuNameClass"]);
            }
            Dr.Close();
            Conn.Close();

            return result;
        }

        private void SetAuthorityAccess(string prmSubMenuNameClass, string prmGroupName)
        {
            string AuthorityName = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT AuthorityName FROM sysGroupAuthority WHERE SubMenuNameClass = '" + prmSubMenuNameClass + "' AND GroupName = '" + prmGroupName + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                AuthorityName = Convert.ToString(Dr["AuthorityName"]);

                if (AuthorityName == ControlMgr.View)
                {
                    ckView.Checked = true;
                }
                else if (AuthorityName == ControlMgr.New)
                {
                    ckNew.Checked = true;
                }
                else if (AuthorityName == ControlMgr.Edit)
                {
                    ckEdit.Checked = true;
                }
                else if (AuthorityName == ControlMgr.Delete)
                {
                    ckDelete.Checked = true;
                }
                else if (AuthorityName == ControlMgr.Approve)
                {
                    ckApprove.Checked = true;
                }
            }
            Dr.Close();
            Conn.Close();
        }

        private void InsertGroupAuthority(string prmGroupName, string prmAuthorityName, string prmSubMenuNameClass, SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            Query = "INSERT INTO sysGroupAuthority(GroupName, AuthorityName, SubMenuNameClass) ";
            Query += "VALUES ('" + prmGroupName + "', '" + prmAuthorityName + "', '" + prmSubMenuNameClass + "')";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery(); 
        }
        #endregion


        #region Tab CopyAccessRight
        private void ResetTabCopyAccessRight()
        {
            if (tabControl1.SelectedIndex == 4)
            {
                SetcbGroupIDFrom();
                SetcbGroupIDTo();
                cbGroupIDFrom.DropDownStyle = ComboBoxStyle.DropDownList;
                cbGroupIDTo.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private void SetcbGroupIDFrom()
        {
            cbGroupIDFrom.Items.Clear();
            cbGroupIDFrom.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT GroupName FROM sysGroupAuthority GROUP BY GroupName";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbGroupIDFrom.Items.Add(Convert.ToString(Dr["GroupName"]));
            }
            Dr.Close();
            Conn.Close();
        }
       
        private void SetcbGroupIDTo()
        {
            cbGroupIDTo.Items.Clear();
            cbGroupIDTo.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT GroupName FROM sysGroupMr";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cbGroupIDTo.Items.Add(Convert.ToString(Dr["GroupName"]));
            }
            Dr.Close();
            Conn.Close();
        }

        private void btnSaveCopyAccessRight_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                string GroupIDFrom = Convert.ToString(cbGroupIDFrom.SelectedItem);
                string GroupIDTo = Convert.ToString(cbGroupIDTo.SelectedItem);

                try
                {
                    if (GroupIDFrom == "" || GroupIDFrom == "-select-")
                    {
                        MessageBox.Show("Group ID From tidak boleh kosong.");
                        return;
                    }
                    else if (GroupIDTo == "" || GroupIDTo == "-select-")
                    {
                        MessageBox.Show("Group ID To tidak boleh kosong.");
                        return;
                    }

                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "DELETE FROM sysGroupAuthority WHERE GroupName = '" + GroupIDTo + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "INSERT INTO sysGroupAuthority (GroupName, AuthorityName, SubMenuNameClass) ";
                    Query += "SELECT  '" + GroupIDTo + "' AS GroupName, AuthorityName, SubMenuNameClass FROM sysGroupAuthority WHERE GroupName = '" + GroupIDFrom + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data Group ID : " + GroupIDTo + " berhasil dicopy dari Group ID " + GroupIDFrom + "");
                    ResetTabCopyAccessRight();

                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }
        #endregion

        private void btnMaster_Click(object sender, EventArgs e)
        {
            MenuName = "Master";
            SetlbAuthority(MenuName);
            btnAccountPayable.BackColor = Color.LightGray;
            btnAccountPayable.ForeColor = Color.Black;
            btnAccountReceivable.BackColor = Color.LightGray;
            btnAccountReceivable.ForeColor = Color.Black;
            btnPurchasing.BackColor = Color.LightGray;
            btnPurchasing.ForeColor = Color.Black;
            btnSales.BackColor = Color.LightGray;
            btnSales.ForeColor = Color.Black;
            btnInventory.BackColor = Color.LightGray;
            btnInventory.ForeColor = Color.Black;
            btnMaster.BackColor = Color.DeepSkyBlue;
            btnMaster.ForeColor = Color.White;
            btnAdministrator.BackColor = Color.LightGray;
            btnAdministrator.ForeColor = Color.Black;
        }

        private void btnAccountPayable_Click(object sender, EventArgs e)
        {
            MenuName = "AccountPayable";
            SetlbAuthority(MenuName);
            btnAccountPayable.BackColor = Color.DeepSkyBlue;
            btnAccountPayable.ForeColor = Color.White;
            btnAccountReceivable.BackColor = Color.LightGray;
            btnAccountReceivable.ForeColor = Color.Black;
            btnPurchasing.BackColor = Color.LightGray;
            btnPurchasing.ForeColor = Color.Black;
            btnSales.BackColor = Color.LightGray;
            btnSales.ForeColor = Color.Black;
            btnInventory.BackColor = Color.LightGray;
            btnInventory.ForeColor = Color.Black;
            btnMaster.BackColor = Color.LightGray;
            btnMaster.ForeColor = Color.Black;
            btnAdministrator.BackColor = Color.LightGray;
            btnAdministrator.ForeColor = Color.Black;
        }

        private void btnAccountReceivable_Click(object sender, EventArgs e)
        {
            MenuName = "AccountReceivable";
            SetlbAuthority(MenuName);
            btnAccountReceivable.BackColor = Color.DeepSkyBlue;
            btnAccountReceivable.ForeColor = Color.White;
            btnAccountPayable.BackColor = Color.LightGray;
            btnAccountPayable.ForeColor = Color.Black;
            btnPurchasing.BackColor = Color.LightGray;
            btnPurchasing.ForeColor = Color.Black;
            btnSales.BackColor = Color.LightGray;
            btnSales.ForeColor = Color.Black;
            btnInventory.BackColor = Color.LightGray;
            btnInventory.ForeColor = Color.Black;
            btnMaster.BackColor = Color.LightGray;
            btnMaster.ForeColor = Color.Black;
            btnAdministrator.BackColor = Color.LightGray;
            btnAdministrator.ForeColor = Color.Black;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MenuName = "ACCOUNTING";
            SetlbAuthority(MenuName);
            btnAccountReceivable.BackColor = Color.DeepSkyBlue;
            btnAccountReceivable.ForeColor = Color.White;
            btnAccountPayable.BackColor = Color.LightGray;
            btnAccountPayable.ForeColor = Color.Black;
            btnPurchasing.BackColor = Color.LightGray;
            btnPurchasing.ForeColor = Color.Black;
            btnSales.BackColor = Color.LightGray;
            btnSales.ForeColor = Color.Black;
            btnInventory.BackColor = Color.LightGray;
            btnInventory.ForeColor = Color.Black;
            btnMaster.BackColor = Color.LightGray;
            btnMaster.ForeColor = Color.Black;
            btnAdministrator.BackColor = Color.LightGray;
            btnAdministrator.ForeColor = Color.Black;
        }
    }
}
