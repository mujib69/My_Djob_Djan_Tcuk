using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;

namespace ISBS_New.Utility
{
    public partial class Frm_UDataPemakai : MetroFramework.Forms.MetroForm
    {
        SqlConnection ConnMaster;
        SqlDataReader Dr;
        string strSql = "";
        
        Boolean BolNew;
        String tmpPassword;
        String tmpFullName;
        String tmpHeadDepartment;
        bool tmpAktif;
        String tmpUserId;
        String tmpGroupId;
        String tmpMenuName = "Master";
        string tmpSubMenuClass = "";
        Boolean BolViewAuth  = false;
        Boolean BolNewAuth = false;
        Boolean BolEditAuth = false;
        Boolean BolDeleteAuth = false;
        Boolean BolApproveAuth = false;
        int i = 0;

        public Frm_UDataPemakai()
        {
            InitializeComponent();
        }

        private void Frm_UDataPemakai_Load(object sender, EventArgs e)
        {
            mTabCtrl_AccessRights.SelectedTab = mTabPage_UserId;
            txtUserId.Focus();
            this.ActiveControl = txtUserId;

            txtUserId.Enabled = true;
            txtPassword.Enabled = false;
            txtFullName.Enabled = false;
            txtHead_Department.Enabled = false;

            cbxtblPass_Aktif.Enabled = false;

            DisableButtonUserId();
            DisableButtonGroupId();

            FillLstGroupId();
            Fillcbogroupid_userid();
            Fillcbouserid_groupid();
            FillMenu("master");
            BtnGetForeColorOriginal();
            BtnMaster.ForeColor = Color.Blue;
        }

        private void DisableButtonUserId()
        {
            BtnEditUserID.Enabled = false;
            BtnCancelUserID.Enabled = false;
            BtnSaveUserID.Enabled = false;        
            BtnDeleteUserID.Enabled = false;
        }

        private void DisableButtonGroupId()
        {        
            BtnEditGroupID.Enabled = false;
            BtnCancelGroupId.Enabled = false;
            BtnSaveGroupId.Enabled = false;
            BtnDeleteGroupId.Enabled = false;
        }

        private void FillLstGroupId()
        {
            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "SELECT tblPassGroupMr_GroupName FROM tblPassGroupMr";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {                   
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            LstGroupId.Items.Add(Dr["tblPassGroupMr_GroupName"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }
            txtGroupId.Enabled = false;
        }

        private void Fillcbogroupid_userid()
        {
            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "SELECT tblPassGroupMr_GroupName FROM tblPassGroupMr";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            CboGroupId_UserId.Items.Add(Dr["tblPassGroupMr_GroupName"]);
                            CboGroupId.Items.Add(Dr["tblPassGroupMr_GroupName"]);
                            CboGroupId_From.Items.Add(Dr["tblPassGroupMr_GroupName"]);
                            CboGroupId_To.Items.Add(Dr["tblPassGroupMr_GroupName"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }
        }

        private void Fillcbouserid_groupid()
        {
            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "SELECT tblPass_UserID FROM tblPass";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            CboUserId_GroupId.Items.Add(Dr["tblPass_UserID"]);
                        }
                    }
                }
            }
            catch(Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }
        }

        private void FillMenu(string NmMenu)         
        {
            LstAuthorityGroupId.Items.Clear();
        
            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "SELECT tblMenu_SubMenuName FROM tblMenu WHERE tblMenu_MenuName = " + "'" + NmMenu + "'" + " ORDER BY tblMenu_NoUrut";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            LstAuthorityGroupId.Items.Add(Dr["tblMenu_SubMenuName"]);
                        }
                    }
                }
            }
            catch(Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }        
        }

        private void BtnGetForeColorOriginal()
        {
            BtnBeliGudang.ForeColor = Color.Black;
            BtnHutang.ForeColor = Color.Black; 
            BtnJualGudang.ForeColor = Color.Black;      
            BtnPiutang.ForeColor = Color.Black;      
            BtnStock.ForeColor = Color.Black;     
            BtnMaster.ForeColor = Color.Black;      
            BtnBank.ForeColor = Color.Black;      
            BtnUtility.ForeColor = Color.Black;
        }

        private void txtUserId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter) && txtUserId.Text.Trim()!="")
            {
                Boolean vBol=true;
                try
                {
                    ConnMaster = ISBS_New.ConnectionString.GetConnection();
                    strSql = "SELECT tblPass_PasswordUser,tblPass_FullName,tblPass_Head_Department,tblPass_Aktif FROM tblPass WHERE tblPass_UserID = @UserId";
                    using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                    {
                        Cmd.Parameters.AddWithValue("@UserId",txtUserId.Text.Trim());
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                vBol = true;
                                txtPassword.Text =(string)Dr["tblPass_PasswordUser"];
                                txtFullName.Text =(string)Dr["tblPass_FullName"];
                                txtHead_Department.Text = (string)Dr["tblPass_Head_Department"];
                                cbxtblPass_Aktif.Checked = (bool)Dr["tblPass_Aktif"];                                
                            }
                        }
                        else
                        {
                            vBol = false;
                        }
                    }
                }
                catch(Exception ex)
                { 
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
                finally
                { 
                    Dr.Close();
                    ConnMaster.Close();
                }

                if (vBol)
                {
                    BtnEditUserID.Enabled = true;
                    BtnDeleteUserID.Enabled = true;
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("UserID " + txtUserId.Text + " doesn't exist, create a new one?", "Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        BolNew = true;
                        txtPassword.Text = "";
                        txtFullName.Text = "";
                        txtHead_Department.Text = "";
                        cbxtblPass_Aktif.Checked = false;
                        txtPassword.Enabled = true;
                        txtFullName.Enabled = true;
                        txtHead_Department.Enabled = true;
                        cbxtblPass_Aktif.Enabled = true;

                        BtnEditUserID.Enabled = false;
                        BtnCancelUserID.Enabled = true;
                        BtnSaveUserID.Enabled = true;
                        BtnDeleteUserID.Enabled = false;
                        txtUserId.Enabled = false;
                        this.ActiveControl = txtPassword;
                        tmpPassword = "";
                        tmpFullName = "";
                        tmpHeadDepartment = "";
                        tmpAktif = false;
                    }
                    else
                    {
                        txtPassword.Text = "";
                        txtFullName.Text = "";
                        cbxtblPass_Aktif.Checked = false;
                        txtHead_Department.Text = "";
                        BtnEditUserID.Enabled = false;
                        BtnDeleteUserID.Enabled = false;
                    }
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            ControlMgr.TblName = "tblPass";
            Form Frm_SearchMgr = new Utility.Frm_SearchMgr();
            Frm_SearchMgr.Text = "Search UserID";
            Frm_SearchMgr.ShowDialog();

            if(ControlMgr.Kode != "")
            {
                txtUserId.Text =ControlMgr.Kode.ToString();            
                FillForm();
            }
            ControlMgr.TblName="";
            ControlMgr.Kode = "";
        }

        private void FillForm()
        {
            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "SELECT tblPass_FullName,TblPass_Head_Department,tblPass_PasswordUser,tblPass_Aktif FROM tblPass WHERE tblPass_UserID = @Kode";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Cmd.Parameters.AddWithValue("@Kode", ControlMgr.Kode.ToString());
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            txtPassword.Text = (string)Dr["tblPass_PasswordUser"];
                            txtFullName.Text = (string)Dr["tblPass_FullName"];
                            txtHead_Department.Text = (string)Dr["tblPass_Head_Department"];
                            cbxtblPass_Aktif.Checked = (bool)Dr["tblPass_Aktif"];
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }
        }

        private void BtnEditUserID_Click(object sender, EventArgs e)
        {
            BolNew = false;
            txtPassword.Enabled = true;
            txtFullName.Enabled = true;
            txtHead_Department.Enabled = true;

            cbxtblPass_Aktif.Enabled = true;

            txtUserId.Enabled = false;
            this.ActiveControl = txtUserId;        
            BtnEditUserID.Enabled = false;
            BtnCancelUserID.Enabled = true;
            BtnSaveUserID.Enabled = true;
            BtnDeleteUserID.Enabled = false;
            tmpUserId = txtUserId.Text;
            tmpPassword = txtPassword.Text;
            tmpFullName = txtFullName.Text;
            tmpHeadDepartment = txtHead_Department.Text;
            tmpAktif = cbxtblPass_Aktif.Checked;
        }

        private void BtnCancelUserID_Click(object sender, EventArgs e)
        {
            txtPassword.Enabled = false;        
            txtFullName.Enabled = false;                    
            txtHead_Department.Enabled = false;

            cbxtblPass_Aktif.Enabled = false;

            BtnEditUserID.Enabled = false;       
            BtnCancelUserID.Enabled = false;       
            BtnSaveUserID.Enabled = false;       
            txtUserId.Enabled = true;       
            txtUserId.Text = tmpUserId;       
            txtPassword.Text = tmpPassword;       
            txtFullName.Text = tmpFullName;      
            txtHead_Department.Text = tmpHeadDepartment;
            cbxtblPass_Aktif.Checked = tmpAktif;
            this.ActiveControl = txtUserId;
       
            if (BolNew == false)       
            {
                BtnDeleteUserID.Enabled = true;        
            }        
            else       
            {        
                BtnDeleteUserID.Enabled = false;      
            }
        }

        private void BtnDeleteUserID_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete UserId " + txtUserId.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);                     
            if (dialogResult == DialogResult.Yes)             
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ISBS_New.ConnectionString.GetConnection();
                        using (ConnMaster)
                        { 
                            strSql = "DELETE FROM tblPass WHERE tblPass_UserID = @UserId";
                            using(SqlCommand Cmd=new SqlCommand(strSql,ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@UserId", txtUserId.Text.Trim());
                                Cmd.ExecuteNonQuery();
                            }
                          
                            strSql = "DELETE FROM tblPassUserGroup WHERE tblPassUserGroup_UserID = @UserId";         
                            using(SqlCommand Cmd=new SqlCommand(strSql,ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@UserId", txtUserId.Text.Trim());
                                Cmd.ExecuteNonQuery();
                            }

                        }
                        Scope.Complete();
                    }
                }
                catch(Exception ex)
                { 
                    MessageBox.Show(ex.Message.ToString()); 
                    return;
                }
                finally
                {
                    ConnMaster.Close();
                }

                MessageBox.Show("UserId " + tmpUserId + " has been deleted");
                txtUserId.Text = "";
                txtPassword.Text = "";
                txtFullName.Text = "";
                txtHead_Department.Text = "";
                cbxtblPass_Aktif.Checked = false;
                txtPassword.Enabled = false;
                txtFullName.Enabled = false;
                txtHead_Department.Enabled = false;

                cbxtblPass_Aktif.Enabled = false;
                DisableButtonUserId();
                txtUserId.Enabled = true;
                this.ActiveControl = txtUserId;
                CboUserId_GroupId.Items.Clear();
                Fillcbouserid_groupid();
            }
        }

        private void BtnSaveUserID_Click(object sender, EventArgs e)
        {
            try
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ConnMaster = ISBS_New.ConnectionString.GetConnection();
                    using (ConnMaster)
                    {
                        if (BolNew)
                        {
                            strSql = "INSERT INTO tblPass(tblPass_UserID,tblPass_PasswordUser,tblPass_FullName,tblPass_Head_Department,tblPass_PwdDateChange,tblPass_Aktif) VALUES (";
                            strSql += "@UserId,@PasswordUser,@FullName,@Head_Department,GETDATE(),@Aktif)";
                        }
                        else
                        {
                            strSql = "UPDATE tblPass SET tblPass_PasswordUser = @PasswordUser,";
                            strSql +=  "tblPass_FullName = @FullName,";
                            strSql +=  "tblPass_Head_Department = @Head_Department, ";
                            strSql += "tblPass_Aktif = @Aktif ";
                            strSql +=  "WHERE tblPass_UserID = @UserId";
                        }

                        using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                        {
                            Cmd.Parameters.AddWithValue("@UserId",txtUserId.Text.Trim());
                            Cmd.Parameters.AddWithValue("@PasswordUser",txtPassword.Text.Trim());
                            Cmd.Parameters.AddWithValue("@FullName",txtFullName.Text.Trim());
                            Cmd.Parameters.AddWithValue("@Head_Department",txtHead_Department.Text.Trim());
                            Cmd.Parameters.AddWithValue("@Aktif", cbxtblPass_Aktif.Checked);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    Scope.Complete();
                }
            }
            catch(Exception ex)
            { 
                MessageBox.Show(ex.Message.ToString());            
                return;
            }
            finally
            {
                ConnMaster.Close();
            }

             if(BolNew)
             {
                MessageBox.Show("Insert Success");
             }
             else
             {
                 MessageBox.Show("Update Success");
             }

            BtnSaveUserID.Enabled = false;
            BtnCancelUserID.Enabled = false;
            BtnEditUserID.Enabled = false;
            BtnDeleteUserID.Enabled = true;
            txtPassword.Enabled = false;
            txtFullName.Enabled = false;
            txtHead_Department.Enabled = false;
            cbxtblPass_Aktif.Enabled = false;
            txtUserId.Enabled = true;
            this.ActiveControl = txtUserId;
            CboUserId_GroupId.Items.Clear();        
            Fillcbouserid_groupid();
        }

        private void BtnNewGroupId_Click(object sender, EventArgs e)
        {
            BolNew = true;        
            txtGroupId.Text = "";   
            txtGroupId.Enabled = true;        
            BtnCancelGroupId.Enabled = true; 
            BtnSaveGroupId.Enabled = true;
            BtnNewGroupId.Enabled = false;
            BtnEditGroupID.Enabled = false;
            BtnDeleteGroupId.Enabled = false;
            tmpGroupId = "";
            this.ActiveControl = txtGroupId;
        }

        private void BtnEditGroupID_Click(object sender, EventArgs e)
        {
            BolNew = false;
            txtGroupId.Enabled = true;
            BtnEditGroupID.Enabled = false;
            BtnCancelGroupId.Enabled = true;
            BtnSaveGroupId.Enabled = true;
            BtnDeleteGroupId.Enabled = false;
            this.ActiveControl = txtGroupId;
            tmpGroupId = txtGroupId.Text;
        }

        private void BtnCancelGroupId_Click(object sender, EventArgs e)
        {
            txtGroupId.Enabled = false;
            BtnEditGroupID.Enabled = false;
            BtnCancelGroupId.Enabled = false;
            BtnSaveGroupId.Enabled = false;
            BtnNewGroupId.Enabled = true;
            txtGroupId.Text = tmpGroupId;
            this.ActiveControl = LstGroupId;
            BtnDeleteGroupId.Enabled = false;
        }

        private Boolean CekGroupId()
        {
            Boolean vBol = false;
            ConnMaster = ISBS_New.ConnectionString.GetConnection();
            strSql = "SELECT * FROM tblPassGroupMr WHERE TblPassGroupMr_GroupName=@GroupName";
            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
            {
                Cmd.Parameters.AddWithValue("@GroupName", txtGroupId.Text.Trim());
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    vBol = true;
                }
            }
            return vBol;
        }

        private void BtnSaveGroupId_Click(object sender, EventArgs e)
        {
            if (txtGroupId.Text.Trim() == "")
            {
                MessageBox.Show("Please enter Groupname First..");
                return;
            }

            if(CekGroupId())
            {
                MessageBox.Show("Group Id Already Exist..");
                return;            
            }

            try
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ConnMaster = ISBS_New.ConnectionString.GetConnection();
                    using (ConnMaster)
                    { 
                        if(BolNew)
                        {
                            strSql = "INSERT INTO tblPassGroupMr VALUES(@GroupId)";                        
                        }
                        else
                        {
                            strSql = "UPDATE TblPassGroupMr SET tblPassGroupMr_GroupName = @GroupId ";
                            strSql += "WHERE TblPassGroupMr_GroupName = @tmpGroupId";  
                  
                        }
                        using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                        {
                            Cmd.Parameters.AddWithValue("@GroupId", txtGroupId.Text.Trim());
                            if (!BolNew) { Cmd.Parameters.AddWithValue("@tmpGroupId", tmpGroupId); }
                            Cmd.ExecuteNonQuery();
                        }

                        strSql = "UPDATE tblPassGroupAuthority SET tblPassGroupAuthority_GroupName = @GroupId ";
                        strSql += "WHERE tblPassGroupAuthority_GroupName =@tmpGroupId";
                        using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                        {
                            Cmd.Parameters.AddWithValue("@GroupId", txtGroupId.Text.Trim());
                            Cmd.Parameters.AddWithValue("@tmpGroupId", tmpGroupId); 
                            Cmd.ExecuteNonQuery();
                        }

                        strSql = "UPDATE TblPassUserGroup SET TblPassUserGroup_GroupName = @GroupId ";
                        strSql += "WHERE TblPassUserGroup_GroupName = @tmpGroupId";
                        using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                        {
                            Cmd.Parameters.AddWithValue("@GroupId", txtGroupId.Text.Trim());
                            Cmd.Parameters.AddWithValue("@tmpGroupId", tmpGroupId); 
                            Cmd.ExecuteNonQuery();
                        }

                    }
                    Scope.Complete();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                ConnMaster.Close();
            }

            if(BolNew)
            {
                MessageBox.Show("Insert Success");
            }
            else
            {
                MessageBox.Show("Update Success");
            }

            LstGroupId.Items.Clear();
            FillLstGroupId();

            BtnSaveGroupId.Enabled = false;
            BtnCancelGroupId.Enabled = false;
            BtnEditGroupID.Enabled = false;
            BtnDeleteGroupId.Enabled = false;
            BtnNewGroupId.Enabled = true;
            txtGroupId.Enabled = false;
            this.ActiveControl = LstGroupId;

            CboGroupId_UserId.Items.Clear();
            CboGroupId.Items.Clear();
            CboGroupId_From.Items.Clear();
            CboGroupId_To.Items.Clear();
            Fillcbogroupid_userid();
        }

        private void BtnDeleteGroupId_Click(object sender, EventArgs e)
        {                        
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete GroupId " + txtGroupId.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);         
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ISBS_New.ConnectionString.GetConnection();
                        using (ConnMaster)
                        {                                        
                            tmpGroupId = txtGroupId.Text;
                            strSql = "DELETE FROM tblPassGroupAuthority WHERE tblPassGroupAuthority_GroupName = @GroupId";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@GroupId", txtGroupId.Text.Trim());
                                Cmd.ExecuteNonQuery();
                            }

                            strSql = "Delete from TblPassGroupMr where TblPassGroupMr_GroupName = @GroupId";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.Parameters.AddWithValue("@GroupId", txtGroupId.Text.Trim());
                                Cmd.ExecuteNonQuery();
                            }                           
                        }
                        Scope.Complete();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
                finally
                {
                    ConnMaster.Close();
                }

                MessageBox.Show("GroupId = " + tmpGroupId + " has been deleted");

                DisableButtonUserId();
                LstGroupId.Items.Clear();
                FillLstGroupId();
                txtGroupId.Text = "";
                BtnDeleteGroupId.Enabled = false;
                BtnEditGroupID.Enabled = false;
                this.ActiveControl = LstGroupId;
                CboGroupId_UserId.Items.Clear();
                CboGroupId.Items.Clear();
                CboGroupId_From.Items.Clear();
                CboGroupId_To.Items.Clear();
                Fillcbogroupid_userid();
            }
        }

        private void txtGroupId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter) && txtGroupId.Text.Trim() != "")
            {
                try
                {
                    ConnMaster = ISBS_New.ConnectionString.GetConnection();
                    strSql = "SELECT * FROM tblPassGroupMr WHERE tblPassGroupMr_GroupName = @GroupId";
                    using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                    {
                        Cmd.Parameters.AddWithValue("@GroupId", txtGroupId.Text.Trim());
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                MessageBox.Show("GroupID " + " already exist");
                                return;
                            }
                        }
                    }
                }
                catch(Exception ex)
                { 
                    MessageBox.Show(ex.Message.ToString());                
                }
                finally
                {
                    Dr.Close();
                    ConnMaster.Close();
                }

                BtnEditGroupID.Enabled = false;
                BtnDeleteGroupId.Enabled = false;
                BtnSaveGroupId.Enabled = true;
                BtnCancelGroupId.Enabled = true;
            }
        }

        private void LstGroupId_DoubleClick(object sender, EventArgs e)
        {
            txtGroupId.Text = LstGroupId.SelectedItem.ToString();
            BtnEditGroupID.Enabled = true;
            BtnDeleteGroupId.Enabled = true;
        }

        private void CboUserId_GroupId_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "SELECT tblPassUserGroup_GroupName FROM tblPassUserGroup WHERE tblPassUserGroup_UserID = " + "'" + CboUserId_GroupId.Text + "'";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        BolNew = false;
                        while (Dr.Read())
                        {
                            CboGroupId_UserId.Text = Convert.IsDBNull(Dr["tblPassUserGroup_GroupName"]) ? "" : (string)Dr["tblPassUserGroup_GroupName"];
                        }
                    }
                    else
                    { 
                        BolNew = true;
                        MessageBox.Show("User Id " + CboUserId_GroupId.Text + " Haven't been setup the group");               
                        CboGroupId_UserId.Text = "";
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }
        }

        private void BtnSaveUserIdGroupId_Click(object sender, EventArgs e)
        {
            try
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ConnMaster = ISBS_New.ConnectionString.GetConnection();
                    using (ConnMaster)
                    {
                        if (BolNew)
                        {
                            strSql = "Insert into TblPassUserGroup values (" + "'" + CboUserId_GroupId.Text + "'" + ",";
                            strSql += "'" + CboGroupId_UserId.Text + "'" + ")";
                        }
                        else
                        {
                            strSql = "Update TblPassUserGroup set TblPassUserGroup_GroupName = " + "'" + CboGroupId_UserId.Text + "'";
                            strSql +=  " Where TblPassUserGroup_UserID = " + "'" + CboUserId_GroupId.Text + "'";
                        }

                        using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    Scope.Complete();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                ConnMaster.Close();
            }

            if(BolNew)
            {
                MessageBox.Show("Insert Success");
                BolNew = false;
            }
            else
            {
                MessageBox.Show("Update Success");
            }

            BtnSaveUserID.Enabled = false;
            BtnCancelUserID.Enabled = false;
            BtnEditUserID.Enabled = false;
            BtnDeleteUserID.Enabled = true;
            txtPassword.Enabled = false;
            txtFullName.Enabled = false;
            txtHead_Department.Enabled = false;
            cbxtblPass_Aktif.Enabled = false;
            txtUserId.Enabled = true;
            this.ActiveControl = txtUserId;
        }

        private void BtnDeleteUserIdGroupId_Click(object sender, EventArgs e)
        {
            String tmpUserId_GroupId  = CboUserId_GroupId.Text;
            String tmpGroupId_UserId = CboGroupId_UserId.Text;

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete UserId = " + CboUserId_GroupId.Text + " with GroupId = " + CboGroupId_UserId.Text + " ?", "Delete Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ISBS_New.ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "Delete from TblPassUserGroup where TblPassUserGroup_UserID = " + "'" + CboUserId_GroupId.Text + "'";
                            using(SqlCommand Cmd = new SqlCommand(strSql,ConnMaster))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
                finally
                {
                    ConnMaster.Close();
                }

                MessageBox.Show("UserId " + tmpUserId_GroupId + " with groupid " + tmpGroupId_UserId + " has been deleted");
                CboUserId_GroupId.Text = "";
                CboGroupId_UserId.Text = "";
                this.ActiveControl = CboUserId_GroupId;
            }
        }

        private void BtnBeliGudang_Click(object sender, EventArgs e)
        {
            BtnGetForeColorOriginal();
            BtnBeliGudang.ForeColor = Color.Blue;
            tmpMenuName = "Beli Gudang";
            FillMenu(tmpMenuName);
        }

        private void BtnHutang_Click(object sender, EventArgs e)
        {
            BtnGetForeColorOriginal();        
            BtnHutang.ForeColor = Color.Blue;
            tmpMenuName = "Hutang";
            FillMenu(tmpMenuName);
        }

        private void BtnJualGudang_Click(object sender, EventArgs e)
        {
            BtnGetForeColorOriginal();
            BtnJualGudang.ForeColor = Color.Blue;
            tmpMenuName = "Jual Gudang";
            FillMenu(tmpMenuName);
        }

        private void BtnPiutang_Click(object sender, EventArgs e)
        {
            BtnGetForeColorOriginal();
            BtnPiutang.ForeColor = Color.Blue;
            tmpMenuName = "Piutang";
            FillMenu(tmpMenuName);
        }

        private void BtnStock_Click(object sender, EventArgs e)
        {
            BtnGetForeColorOriginal();
            BtnStock.ForeColor = Color.Blue;
            tmpMenuName = "Stock";
            FillMenu(tmpMenuName);
        }

        private void BtnMaster_Click(object sender, EventArgs e)
        {
            BtnGetForeColorOriginal();
            BtnMaster.ForeColor = Color.Blue;
            tmpMenuName = "Master";
            FillMenu(tmpMenuName);
        }

        private void BtnBank_Click(object sender, EventArgs e)
        {
            BtnGetForeColorOriginal();
            BtnBank.ForeColor = Color.Blue;
            tmpMenuName = "Bank";
            FillMenu(tmpMenuName);
        }

        private void BtnUtility_Click(object sender, EventArgs e)
        {
            BtnGetForeColorOriginal();
            BtnUtility.ForeColor = Color.Blue;
            tmpMenuName = "Utility";
            FillMenu(tmpMenuName);
        }

        private void CboGroupId_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChkView.Checked = false;
            ChkNew.Checked = false;
            ChkEdit.Checked = false;
            ChkDelete.Checked = false;
            ChkApprove.Checked = false;
            LstAuthorityGroupId.SelectedIndex = -1;
            LstUserId.Items.Clear();

            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "Select tblPassUserGroup_UserID from TblPassUserGroup where TblPassUserGroup_GroupName = " + "'" + CboGroupId.Text + "'";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            LstUserId.Items.Add((string)Dr["tblPassUserGroup_UserID"]);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }
        }

        private void LstAuthorityGroupId_DoubleClick(object sender, EventArgs e)
        {              
            if(CboGroupId.Text.Trim() == "")          
            {
                  MessageBox.Show("Please fill in GroupId first");
                  return;             
            }
            ClearCheckBox();


            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "Select TblMenu_SubMenuNameClass from TblMenu ";
                strSql +=   "where TblMenu_MenuName = " + "'" + tmpMenuName + "'";
                strSql +=   " and TblMenu_SubMenuName = " + "'" + LstAuthorityGroupId.SelectedItem.ToString() + "'";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        { 
                            tmpSubMenuClass=Convert.IsDBNull(Dr["TblMenu_SubMenuNameClass"])? "" : (string)Dr["TblMenu_SubMenuNameClass"];
                            if(tmpSubMenuClass=="")
                            {
                                MessageBox.Show("Form hasn't been created"); 
                                return;                            
                            }
                        }
                    }
                }

            }
            catch(Exception ex)
            { 
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                Dr.Close();
                ConnMaster.Close();
            }


            DataSet Ds = new DataSet();
            SqlDataAdapter SqlDa = new SqlDataAdapter();

            try
            {
                ConnMaster = ISBS_New.ConnectionString.GetConnection();
                strSql = "Select TblPassGroupAuthority_AuthorityName from TblPassGroupAuthority ";
                strSql += "where TblPassGroupAuthority_GroupName = " + "'" + CboGroupId.Text + "'";
                strSql +=" and TblPassGroupAuthority_SubMenuNameClass = " + "'" + tmpSubMenuClass + "'";
                using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                {
                    SqlDa.SelectCommand = Cmd;
                    SqlDa.Fill(Ds,"Authority");

                    int i;
                    if (Ds.Tables["Authority"].Rows.Count == 0)
                    {
                        BolNew = true;
                    }
                    else
                    {
                        BolNew = false;
                    }

                    for ( i = 1; i <= Ds.Tables["Authority"].Rows.Count; i++)
                    {
                        if (Ds.Tables["Authority"].Rows[i - 1][0].ToString() == "View")
                        {
                            ChkView.Checked = true;
                            BolViewAuth = true;
                        }
                        else if (Ds.Tables["Authority"].Rows[i - 1][0].ToString() == "New")
                        {
                            ChkNew.Checked = true;
                            BolNewAuth = true;
                        }
                        else if (Ds.Tables["Authority"].Rows[i - 1][0].ToString() == "Edit")
                        {
                            ChkEdit.Checked = true;
                            BolEditAuth = true;
                        }
                        else if (Ds.Tables["Authority"].Rows[i - 1][0].ToString() == "Delete")
                        {
                            ChkDelete.Checked = true;
                            BolDeleteAuth = true;
                        }
                        else if (Ds.Tables["Authority"].Rows[i - 1][0].ToString() == "Approve")
                        {
                            ChkApprove.Checked = true;
                            BolApproveAuth = true;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                ConnMaster.Close();
            }
        }
           
        private void ClearCheckBox()         
        {
            ChkView.Checked = false;        
            ChkNew.Checked = false;
            ChkEdit.Checked = false;
            ChkDelete.Checked = false;
            ChkApprove.Checked = false;
        }

        private void BtnUndoAuthority_Click(object sender, EventArgs e)
        {
            if (BolViewAuth)         
            {
                ChkView.Checked = true;             
            }
            else
            {
                ChkView.Checked = false;            
            }

            if (BolNewAuth)            
            {
                ChkNew.Checked = true;
            }
            else
            {
                ChkNew.Checked = false;
            }

            if (BolEditAuth)
            {
                ChkEdit.Checked = true;        
            }
            else             
            {
                ChkEdit.Checked = false;
            }

            if (BolDeleteAuth)
            {
                ChkDelete.Checked = true;
            }
            else
            {            
                ChkDelete.Checked = false;
            }

            if (BolApproveAuth)
            {
                ChkApprove.Checked = true;
            }
            else
            {
                ChkApprove.Checked = false;
            }
        }

        private void BtnSaveAuthority_Click(object sender, EventArgs e)
        {
            if( tmpSubMenuClass == "" || CboGroupId.Text == "" )
            {
                MessageBox.Show("Please fill in GroupId or Select Menu first");
                return;
            }

            try
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    ConnMaster = ISBS_New.ConnectionString.GetConnection();
                    using (ConnMaster)
                    {
                        strSql = "Delete from TblPassGroupAuthority where TblPassGroupAuthority_GroupName = " + "'" + CboGroupId.Text + "'";
                        strSql +=  " and TblPassGroupAuthority_SubMenuNameClass = " + "'" + tmpSubMenuClass + "'";
                        using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                        {
                            Cmd.ExecuteNonQuery();
                        }

                        if(ChkNew.Checked)
                        {
                            strSql = "Insert into TblPassGroupAuthority values (" + "'" + CboGroupId.Text + "'" + ",";
                            strSql +=  "'" + "New" + "'" + ",";
                            strSql +=  "'" + tmpSubMenuClass + "'" + ")";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }

                        if(ChkView.Checked)
                        { 
                            strSql = "Insert into TblPassGroupAuthority values (" + "'" + CboGroupId.Text + "'" + ",";
                            strSql +=  "'" + "View" + "'" + ",";
                            strSql +=  "'" + tmpSubMenuClass + "'" + ")";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }

                        if(ChkEdit.Checked)
                        {
                            strSql = "Insert into TblPassGroupAuthority values (" + "'" + CboGroupId.Text + "'" + ",";
                            strSql +=  "'" + "Edit" + "'" + ",";
                            strSql +=  "'" + tmpSubMenuClass + "'" + ")";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }

                        if(ChkDelete.Checked)
                        {
                            strSql = "Insert into TblPassGroupAuthority values (" + "'" + CboGroupId.Text + "'" + ",";
                            strSql +=  "'" + "Delete" + "'" + ",";
                            strSql +=  "'" + tmpSubMenuClass + "'" + ")";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }

                        if (ChkApprove.Checked == true)
                        {
                            strSql = "Insert into TblPassGroupAuthority values (" + "'" + CboGroupId.Text + "'" + ",";
                            strSql +=  "'" + "Approve" + "'" + ",";
                            strSql +=  "'" + tmpSubMenuClass + "'" + ")";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    Scope.Complete();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                ConnMaster.Close();
            }


            if(strSql != "" )
            {
                if(BolNew)            
                {
                    MessageBox.Show("Insert Success");
                }                 
                else
                {
                    MessageBox.Show("Update Success");
                }
            }
            this.ActiveControl = CboGroupId;
        }

        private void BtnSaveCopyAccessRight_Click(object sender, EventArgs e)
        {
            strSql = "Are you sure you want to copy access right from group ";
            strSql +=  CboGroupId_From.Text + " to group " + CboGroupId_To.Text + " ? ";
            strSql +=  "It will delete existing access right " + CboGroupId_To.Text;

            DialogResult dialogResult = MessageBox.Show(strSql, "Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        ConnMaster = ISBS_New.ConnectionString.GetConnection();
                        using (ConnMaster)
                        {
                            strSql = "Delete from tblPassGroupAuthority where TblPassGroupAuthority_GroupName = " + "'" + CboGroupId_To.Text + "'";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            { Cmd.ExecuteNonQuery(); }

                            strSql = "INSERT INTO TblPassGroupAuthority(TblPassGroupAuthority_GroupName, ";
                            strSql +=  "TblPassGroupAuthority_AuthorityName, ";
                            strSql +=  "TblPassGroupAuthority_SubMenuNameClass) ";
                            strSql +=  "SELECT '" + CboGroupId_To.Text + "', TblPassGroupAuthority_AuthorityName, ";
                            strSql +=  "TblPassGroupAuthority_SubMenuNameClass ";
                            strSql +=  "FROM TblPassGroupAuthority ";
                            strSql +=  "where TblPassGroupAuthority_GroupName = '" + CboGroupId_From.Text + "'";
                            using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
                            { Cmd.ExecuteNonQuery(); }
                        }
                        Scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
                finally
                {
                    ConnMaster.Close();
                }

                MessageBox.Show("Copy access right succes");
            }
        }
    }
}
