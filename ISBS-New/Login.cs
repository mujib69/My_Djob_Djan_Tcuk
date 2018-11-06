using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;

namespace ISBS_New
{
    public partial class Login : MetroFramework.Forms.MetroForm
    {
        //SQL Function
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Command;
        private string Query;

        //User Profile
        //public static string UserId = "";
        public static string UserId = "";
        public static string UserGroup = "";
        //public static string UserId = "Admin";
        //public static string UserGroup = "PurchaseManager"; // SalesManager,PurchaseManager,PurchaseManager
        //public static string Password = "";
        public static string DesktopName;
        public static string IpAddress;
        public static string Email = "surya@intisumberbajasakti.com";
        public int Time;
        public static List<string> GroupId = new List<string>();
        MainMenu tmpMainMenu = new MainMenu();
        public static string PermissionDenied = "You Don’t Have Permission to Access";

        public static string Delete = "Delete";
        public static string Edit = "Edit";
        public static string View = "View";
        public static string New = "New";
        public static string Approve = "Approve";

        public Login()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (txtUserId.Text == "")
            {
                MessageBox.Show("UserId harus diisi.");
                txtUserId.BackColor = Color.Pink;
            }
            else if (txtPassword.Text == "")
            {
                MessageBox.Show("Password harus diisi.");
                txtPassword.BackColor = Color.Pink;
            }
            else {
                Query = "SELECT a.UserID AS UserId, b.GroupName AS UserGroup,Aktif,a.[PasswordUser] ";
                Query += "FROM sysPass a INNER JOIN sysUserGroup b ";
                Query += "ON b.UserID = a.UserID ";
                Query += "WHERE a.UserID = @userid AND a.PasswordUser = @pwd";
                //Query += "a.UserID = '" + txtUserId.Text.Trim() + "' ";
                //Query += "AND a.PasswordUser = '" + txtPassword.Text.Trim() + "'";
                Conn = ConnectionString.GetConnection();
                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    Command.Parameters.AddWithValue("@userid", txtUserId.Text);
                    Command.Parameters.AddWithValue("@pwd", txtPassword.Text);
                    Dr = Command.ExecuteReader();
                    if (Dr.Read())
                    {
                        if ((Boolean)Dr["Aktif"] == false)
                        {
                            MessageBox.Show("User belum diaktifkan..");
                            Dr.Close();
                            Conn.Close();
                            return;
                        }
                        if (Dr["PasswordUser"].ToString() != txtPassword.Text)
                        {
                            MessageBox.Show("Password Salah.");
                            txtPassword.BackColor = Color.Pink;
                            Dr.Close();
                            Conn.Close();
                            return;
                        }
                        UserId = Dr["UserId"].ToString();
                        UserGroup = Dr["UserGroup"].ToString();
                        }
                }
                Dr.Close();
                Conn.Close();

                if (UserId == "")
                {
                    MessageBox.Show("UserId atau Password salah.");
                    txtUserId.BackColor = Color.Pink;
                    txtPassword.BackColor = Color.Pink;
                }
                else {
                    GroupId.Clear();
                    Query = "SELECT GroupName FROM sysGroupMr";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand Command = new SqlCommand(Query, Conn))
                    {
                        Dr = Command.ExecuteReader();
                        while (Dr.Read())
                        {
                            GroupId.Add(Dr["GroupName"].ToString());                            
                        }
                    }
                    Dr.Close();
                    Conn.Close();

                    IpAddress = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
                    DesktopName = System.Windows.Forms.SystemInformation.ComputerName;
                    tmpMainMenu.Show();
                    tmpMainMenu.lblLogin.Text = UserId;
                    this.Visible = false;
                }          
            }
        }

        private void UserId_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtUserId.Text != "")
            {
                txtUserId.BackColor = Color.White;
            }
            else {
                txtUserId.BackColor = Color.Pink;
            }
        }

        private void Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtPassword.Text != "")
            {
                txtPassword.BackColor = Color.White;
            }
            else
            {
                txtPassword.BackColor = Color.Pink;
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            txtUserId.Focus();
        }

        private void txtUserId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (txtUserId.Text.Trim() != "")
                {
                    txtPassword.Focus();
                }
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (txtPassword.Text.Trim() != "")
                {
                    OK.PerformClick();
                }
            }
        }

        //private void FLogin(string txtUser, string txtPass)
        //{
        //    Query = "Select UserId, UserId, Password, DesktopName,IpAddress,case when LoginTime is null then 0 else DATEDIFF(ss,LoginTime, GETDATE()) end [LoginTime] From [User].[User] where UserId = '" + txtUserId.Text.Trim().ToUpper() + "' ";
        //    Conn = ConnectionString.GetConnection();

        //    using (SqlCommand Command = new SqlCommand(Query, Conn))
        //    {
        //        Dr = Command.ExecuteReader();
        //        if (Dr.Read())
        //        {
        //            UserId = Dr["UserId"].ToString();
        //            UserId = Dr["UserId"].ToString();
        //            Password = Dr["Password"].ToString();
        //            DesktopName = System.Net.Dns.GetHostName(); // Dr("DesktopName").ToString()
        //            IpAddress = "192.168.0.171"; // System.Net.Dns.GetHostByName(strHostName).AddressList(0).ToString() 'Dr("IpAddress").ToString()
        //            Time = Dr["LoginTime"].ToString() == null ? 0 : Convert.ToInt32(Dr["LoginTime"]);
        //        }
        //    }
           
        //}

        //private void ValidasiLogin()
        //{
        //    Query = "Select UserId,Password,DesktopName,IpAddress,case when LoginTime is null then 0 else DATEDIFF(ss,LoginTime, GETDATE()) end [LoginTime] From [User].[User] where UserId = '" + UserId + "' ";
        //    Conn = ConnectionString.GetConnection();

        //    using (SqlCommand Command = new SqlCommand(Query, Conn))
        //    {
        //        Dr = Command.ExecuteReader();
        //    }
        //    if (Dr.Read())
        //    {
        //        if (Password != Dr["Password"].ToString())
        //        {
        //            MessageBox.Show("Password salah / telah diganti.");
        //            Logout();
        //        }
        //        else if (Password != Dr["Password"].ToString())
        //        {
        //            if (DesktopName != Dr["DesktopName"].ToString() || IpAddress != Dr["IpAddress"].ToString())
        //            {
        //                DialogResult Result = MessageBox.Show("User masih aktif digunakan di : " + Environment.NewLine + Environment.NewLine + "Komputer = " + Dr["DesktopName"].ToString() + Environment.NewLine + "IPAddress = " + Dr["IpAddress"].ToString() + Environment.NewLine + Environment.NewLine + "Apakah anda akan tetap login ?", "Konfirmasi", MessageBoxButtons.YesNo);
        //                tmpMainMenu.Timer1.Enabled = true;
        //                if (Result == DialogResult.Yes)
        //                    UpdateLogin();
        //                else if (Result == DialogResult.No)
        //                    Logout();
        //            }
        //        }
        //        else
        //        {
        //            UpdateLogin();
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("UserId tidak ada / telah diganti.");
        //        Logout();
        //    }
        //    Conn.Close();

        //}

        //private Boolean UpdateLogin()
        //{
        //    Dr.Close();
        //    Query = "Update [User].[User] Set DesktopName='" + DesktopName + "',IpAddress='" + IpAddress + "', LoginTime=getdate(), UpdatedDate=getdate(), UpdatedBy='" + UserId + "' ";
        //    Query += "Where UserId='" + UserId + "' and Password='" + Password + "'";

        //    using (SqlCommand Command = new SqlCommand(Query, Conn))
        //    {
        //        Command.ExecuteNonQuery();
        //    }
        //    Conn.Close();
        //    tmpMainMenu.Show();
        //    tmpMainMenu.lblLogin.Text = UserId;
        //    tmpMainMenu.lblDesktop.Text = DesktopName;
        //    tmpMainMenu.lblIp.Text = IpAddress;
        //    tmpMainMenu.Timer1.Interval = 1000;
        //    tmpMainMenu.Timer1.Enabled = true;
        //    //tmpMainMenu.Timer1.Start();
        //    this.Visible = false;
        //    //this.Close();
        //    return true;
        //}

        //public Boolean Logout()
        //{
        //    UserId = "";
        //    UserId = "";
        //    Password = "";
        //    DesktopName = "";
        //    IpAddress = "";
        //    tmpMainMenu.Close();// = false;
        //    this.Visible = true;
        //    return true;
        //}

        //private void Cancel_Click(object sender, EventArgs e)
        //{
        //    Application.Exit();
        //}


        //private void OK_Click(object sender, EventArgs e)
        //{
        //    if (txtUserId.Text.Trim() == "")
        //        MessageBox.Show("UserId tidak boleh kosong.");
        //    else
        //        MessageBox.Show("Password tidak boleh kosong.");

        //    FLogin(txtUserId.Text.Trim(), txtPassword.Text.Trim());
        //    ValidasiLogin();
        //}

        //private void OK_Click_1(object sender, EventArgs e)
        //{
        //    //if (txtUserId.Text.Trim() == "")
        //    //{
        //    //    MessageBox.Show("UserId tidak boleh kosong.");
        //    //    return;
        //    //}
        //    //else if (txtUserId.Text.Trim() == "")
        //    //{
        //    //    MessageBox.Show("Password tidak boleh kosong.");
        //    //    return;
        //    //}

        //    //FLogin(txtUserId.Text.Trim(), txtPassword.Text.Trim());
        //    //ValidasiLogin();            
        //    Validasi();
            
            
        //}

        //private void Validasi()
        //{
        //    Methods.Validation Validation = new Methods.Validation();
        //    Validation.Validasi(txtUserId);
        //    Validation.Validasi(txtPassword);
        //    if (Validation.ValValidasi == false)
        //    {
        //        MessageBox.Show("Warna merah wajib diisi.");
        //        Validation.Dispose();
        //    }
        //    else
        //    {
        //        Validation.Dispose();
        //        tmpMainMenu.Show();
        //        this.Visible = false;
        //    }

        //}

        //private void Cancel_Click_1(object sender, EventArgs e)
        //{
        //    Application.Exit();
        //}

        //private void Login_Load(object sender, EventArgs e)
        //{
        //    lblForm.Location = new Point(16, 11);

        //    Methods.Mandatory Mandatory = new Methods.Mandatory();
        //    Mandatory.CreateMandatory(txtUserId);
        //    Mandatory.CreateMandatory(txtPassword);
        //    Mandatory.Dispose();
        //}

        //private void txtUserId_TextChanged(object sender, EventArgs e)
        //{

        //}
    }
}
