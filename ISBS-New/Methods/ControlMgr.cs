using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroFramework.Forms;
using System.Windows.Forms;
using System.Data.SqlClient;

class ControlMgr
{
    public static string UserId = "";
    public static string GroupName = "";

    public static string DesktopName;
    public static string IpAddress;

    public static string tmpNamaFormSearch = "";
    public static string Kode = "";
    public static string Kode2 = "";
    public static string Kode3 = "";
    public static string Kode4 = "";

    public static string TblName = "";
    public static string tmpWhere = "";
    public static string tmpSort = "";
    public static Boolean tmpAmbilKodeNo = false;

    //-----------------------------------------------------------------------------
    public static string Delete = "Delete";
    public static string Edit = "Edit";
    public static string View = "View";
    public static string New = "New";
    public static string Approve = "Approve";
    public static string PermissionDenied = "You Don’t Have Permission to Access";
    //-----------------------------------------------------------------------------
    
    public static void CheckAccessRight(ref Boolean vView, ref Boolean vNew, ref Boolean vEdit, ref Boolean vDelete, String SubMenuClass)
    {
        vView = false;
        vNew = false;
        vEdit = false;
        vDelete = false;
        string strSql = "";
        SqlConnection ConnMaster;
        SqlDataReader drtblPassGroupAuthority;


        ConnMaster = ISBS_New.ConnectionString.GetConnection();
        strSql = "Select TblPassGroupAuthority_AuthorityName ";
        strSql = strSql + "from tblPassGroupAuthority ";
        strSql = strSql + "where TblPassGroupAuthority_GroupName = " + "'" + GroupName + "' and ";
        strSql = strSql + "TblPassGroupAuthority_SubMenuNameClass = " + "'" + SubMenuClass + "'";

        using (SqlCommand Cmd = new SqlCommand(strSql, ConnMaster))
        {
            drtblPassGroupAuthority = Cmd.ExecuteReader();
            if (drtblPassGroupAuthority.HasRows)
            {
                while (drtblPassGroupAuthority.Read())
                {
                    if ((string)drtblPassGroupAuthority["TblPassGroupAuthority_AuthorityName"] == "View")
                    {
                        vView = true;
                    }
                    else if ((string)drtblPassGroupAuthority["TblPassGroupAuthority_AuthorityName"] == "New")
                    {
                        vNew = true;
                    }
                    else if ((string)drtblPassGroupAuthority["TblPassGroupAuthority_AuthorityName"] == "Edit")
                    {
                        vEdit = true;
                    }
                    else if ((string)drtblPassGroupAuthority["TblPassGroupAuthority_AuthorityName"] == "Delete")
                    {
                        vDelete = true;
                    }
                }
            }
        }

        drtblPassGroupAuthority.Close();
        ConnMaster.Close();

    }

    public static void DisplayMsgAccessRight(string vAccessRight, string vFormName)
    {
        if (vAccessRight == "View")
        {
            MessageBox.Show("You don't have permission to View " + vFormName, vFormName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        else if (vAccessRight == "New")
        {
            MessageBox.Show("You don't have permission to Create New " + vFormName);
        }
        else if (vAccessRight == "Edit")
        {
            MessageBox.Show("You don't have permission to Edit " + vFormName);
        }
        else if (vAccessRight == "Delete")
        {
            MessageBox.Show("You don't have permission to Delete " + vFormName);
        }
        else if (vAccessRight == "Approve")
        {
            MessageBox.Show("You don't have permission to Approval " + vFormName);
        }
    } 
}


